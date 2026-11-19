# .NET 11 GA の Mono ベース browser-wasm ランタイム調査ノート

調査日は 2026-11-19。出典の区分は次のとおり。【公式Doc】は Microsoft Learn と .NET Blog、【公式ソース】は dotnet/runtime の `release/11.0` ブランチ上のファイル、【公式Issue】は dotnet/runtime の Issue と PR（計画や作業記録であり、製品としての確約ではない）、【Emscripten】は Emscripten プロジェクトのソースと PR、【コミュニティ】は第三者の発言や記事、【検索要約】は検索結果の要約でしか確認しておらず本文を開いていないもの。版の指定がない記述は .NET 11 の状態を指す。

## 1. .NET 11 でのサポート状況と、.NET 10 からの変更点

### Takeaway
.NET 11 は 2026-11-10 リリースの STS（サポート終了 2028-11-14）で、browser-wasm の既定ランタイムは Mono のまま。CoreCLR 版は opt-in で、.NET 12 に向けて CoreCLR への移行と Mono パック削除が dotnet/runtime の Issue で計画されているが、Mono 廃止の正式発表は確認できなかった。

### Cited Findings

サポート状況

- 【公式Doc】.NET 11 は STS、リリース日 2026-11-10、最新パッチ 11.0.0、サポートフェーズ Active、サポート終了 2028-11-14。STS の無償サポートは 2 年、LTS は 3 年 — [.NET support policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core)
- 【公式Doc】David Ortinau による 2026-05-13 の投稿は、Blazor WebAssembly は影響を受けないとして "WASM continues to use Mono, and that is not changing in .NET 11." と述べている（GA 前の記述） — [.NET MAUI Moves to CoreCLR in .NET 11](https://devblogs.microsoft.com/dotnet/dotnet-maui-moves-to-coreclr-in-dotnet-11/)
- 【公式ソース】ワークロードの targets では `UseMonoRuntime` が `false` のときだけ CoreCLR 側の分岐に入り、未設定なら Mono 側の分岐に入る — [WorkloadManifest.targets.in](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/nuget/Microsoft.NET.Workload.Mono.Toolchain.Current.Manifest/WorkloadManifest.targets.in)、[Microsoft.NET.Sdk.WebAssembly.Browser.targets](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/nuget/Microsoft.NET.Sdk.WebAssembly.Pack/build/Microsoft.NET.Sdk.WebAssembly.Browser.targets)
- 【公式ソース】`wasm-tools` ワークロードは `Microsoft.NETCore.App.Runtime.Mono.*.browser-wasm`、`Microsoft.NETCore.App.Runtime.*.browser-wasm`、`Microsoft.NETCore.App.Runtime.AOT.Cross.*.browser-wasm` などを含む — [WorkloadManifest.json.in](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/nuget/Microsoft.NET.Workload.Mono.Toolchain.Current.Manifest/WorkloadManifest.json.in)
- 【公式Doc】.NET 11 Preview 5 時点で browser CoreCLR は opt-in（`UseMonoRuntime=false`）で、AOT とネイティブビルドには Mono が必要だった（Preview 時点の情報） — [.NET 11 Preview 5 runtime notes](https://raw.githubusercontent.com/dotnet/core/main/release-notes/11.0/preview/preview5/runtime.md)

CoreCLR への移行計画

- 【公式Issue】#121511 は CoreCLR インタープリターでの WebAssembly 実行を扱い、.NET 11 期は初期プレビュー、.NET 12 期は安定性・性能・移行に注力するとしている。2026-08-11 に完了扱いでクローズ — [dotnet/runtime#121511](https://github.com/dotnet/runtime/issues/121511)
- 【公式Issue】#134337（2026-09-21 起票、Open、マイルストーン 12.0.0）は .NET 12 で WebAssembly アプリを CoreCLR で動かすことを目標とする。Release での full R2R 既定化、CoreCLR の既定ランタイム化、そして #134397 を通じた Mono の WebAssembly パック出荷停止を項目に含む — [dotnet/runtime#134337](https://github.com/dotnet/runtime/issues/134337)
- 【コミュニティ】Discussion #129902 では、投稿者が Microsoft 社員から .NET 11 が Mono の最終版と聞いたと述べ、正式発表がないことを指摘している。Collaborator の返信に Mono ランタイムの終息に触れるものはあるが、.NET 11 が最終版だとの確認や日程の提示はない — [dotnet/runtime discussion #129902](https://github.com/dotnet/runtime/discussions/129902)
- 【コミュニティ】Uno Platform の判断記録（2026-09-17 起票）は、4 つのゲートが満たされなければ browser-wasm を .NET 11 系の Mono に留めるとしている — [Uno.Wasm.Bootstrap#1074](https://github.com/unoplatform/Uno.Wasm.Bootstrap/issues/1074)

.NET 10 からの変更点（Mono 経路に影響するもの）

- 【公式ソース】同梱 Emscripten が 3.1.56（.NET 10）から 6.0.2（.NET 11）に更新 — [release/10.0 emscripten-version.txt](https://raw.githubusercontent.com/dotnet/runtime/release/10.0/src/mono/browser/emscripten-version.txt)、[release/11.0 emscripten-version.txt](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/browser/emscripten-version.txt)
- 【公式ソース】例外処理の既定フラグが `-fwasm-exceptions`（.NET 10）から `-fwasm-exceptions -s WASM_LEGACY_EXCEPTIONS=0`（.NET 11）に変更 — [release/10.0 BrowserWasmApp.targets](https://raw.githubusercontent.com/dotnet/runtime/release/10.0/src/mono/browser/build/BrowserWasmApp.targets)、[release/11.0 BrowserWasmApp.targets](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/browser/build/BrowserWasmApp.targets)
- 【公式Doc】What's new に挙がる WebAssembly 改善は、WebCIL ペイロードの直接読み込み、デバッグシンボルの改善、`float[]` / `Span<float>` / `ArraySegment<float>` のマーシャリング改善。WebCIL V1 既定化、ネイティブ再リンク、JS 最小化は CoreCLR 向けの項目 — [What's new in .NET 11 runtime](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-11/runtime)
- 【公式Doc】`Single` の `Span` / `ArraySegment` / 配列マッピングは aspnetcore-11.0 の表にあり、aspnetcore-10.0 以前の表にはない — [JSImport/JSExport interop](https://learn.microsoft.com/en-us/aspnet/core/client-side/dotnet-interop/?view=aspnetcore-11.0)

### Inferences
- .NET 12 で計画どおり Mono パックが外れれば、.NET 11 が browser-wasm Mono の最後の系列になり、公式サポートの上限は 2028-11-14 になる。ただし根拠は未完了の追跡 Issue であり、確定事項として扱うべきではない。
- .NET 10 の `-fwasm-exceptions` は、Emscripten 3.1.56 の既定からレガシー形式の Wasm EH を生成していたと考えられる。.NET 11 への更新は実質的に exnref への移行で、最低ブラウザーが上がる（第 3 節）。

### Gaps
- .NET 11 GA のアナウンス記事は検索で見つからず、GA 時点で Mono が既定であることの明文は確認できなかった。上記はソースの分岐と GA 前の公式記述からの判断。
- `release/11.0` はブランチの先端を読んだもので、11.0.0 のタグそのものとは差がありうる。ローカルの SDK 11.0.100 での実ビルド確認はしていない。
- ソースファイルは取得ツールの抽出結果を通して読んでおり、全文を目視したものではない。

## 2. 実行方式（インタープリター、Jiterpreter、フル AOT、Profiled AOT）

### Takeaway
既定は IL インタープリター + Jiterpreter で、`RunAOTCompilation=true` の publish 時のみ LLVM 経由の AOT が走る。AOT の既定モードは `LLVMOnlyInterp` で、AOT できないメソッドはインタープリターへフォールバックする。つまり「フル AOT」でもインタープリターは残る。

### Cited Findings

インタープリターと Jiterpreter

- 【公式Doc】AOT なしの Blazor WebAssembly は、部分的な JIT 機能（Jiterpreter）を持つ WebAssembly 実装の IL インタープリターで動き、サーバー側 JIT より一般に遅い — [Blazor build tools and AOT](https://learn.microsoft.com/en-us/aspnet/core/blazor/webassembly-build-tools-and-aot?view=aspnetcore-11.0)
- 【公式ソース】Jiterpreter の構成要素は、インタープリターのオペコード列を Wasm 関数へ変換する traces、AOT コードからインタープリターへの遷移を最適化する interp_entry thunk、インタープリターからネイティブコードへの遷移を扱う jit_call thunk の 3 つ — [jiterpreter.md](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/docs/design/mono/jiterpreter.md)
- 【公式ソース】`jiterpreter-traces-enabled` はシングルスレッドビルドで既定有効、スレッド有効時は無効。`jiterpreter-interp-entry-enabled` と `jiterpreter-jit-call-enabled` は AOT 時にのみ意味を持つ。無効化は `BlazorWebAssemblyJiterpreter=false` または `--no-jiterpreter-traces-enabled` — [jiterpreter.md](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/docs/design/mono/jiterpreter.md)
- 【公式ソース】制約として、ブラウザーの同期コンパイルモジュールには 4KB 上限があること、`jiterpreter-wasm-bytes-limit` で生成総量に上限があること、短すぎる trace は棄却されること、コンパイルエラーが重なると自動で無効化されることが記載されている。Blazor では publish 済みアプリでのみ動作し、デバッグ中は動かない — [jiterpreter.md](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/docs/design/mono/jiterpreter.md)
- 【公式ソース】Jiterpreter は性能と引き換えにメモリ使用量が増える — [features.md](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/wasm/features.md)

AOT

- 【公式ソース】`RunAOTCompilation` は既定 false。publish 時のみ適用され、生成コードは `dotnet.native.wasm` に入り、`wasm-tools` ワークロードが必要 — [features.md](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/wasm/features.md)、[WasmApp.Common.targets](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/wasm/build/WasmApp.Common.targets)
- 【公式ソース】`AOTMode` の既定は `LLVMOnlyInterp`、`MonoAOTCompiler` タスクには `UseLLVM=true` が渡る — [WasmApp.Common.targets](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/wasm/build/WasmApp.Common.targets)
- 【公式ソース】`LLVMOnlyInterp` は AOT 引数 `llvmonly` と `interp` に展開され、`full` は付かない。`interp` は AOT できないメソッドをインタープリターに回すための指定 — [MonoAOTCompiler.cs](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/tasks/AotCompilerTask/MonoAOTCompiler.cs)
- 【公式ソース】`WasmDedup` は既定 true で、AOT 時にジェネリックインスタンスを重複排除する — [WasmApp.Common.targets](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/wasm/build/WasmApp.Common.targets)
- 【公式Doc】`WasmStripILAfterAOT` はコンパイル済みメソッドの IL を除去するが、インタープリターが実行時に必要とするメソッドは除去できない — [Blazor WebAssembly runtime performance](https://learn.microsoft.com/en-us/aspnet/core/blazor/performance/webassembly-runtime-performance?view=aspnetcore-11.0)

Profiled AOT

- 【公式ソース】`AotProfilePath` を指定すると AOT 引数に `profile-only` と `profile=<path>` が追加される。`MibcProfilePath` では `profile-only` と `mibc-profile=<path>` — [MonoAOTCompiler.cs](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/tasks/AotCompilerTask/MonoAOTCompiler.cs)
- 【公式ソース】targets のヘッダーには `AOTProfilePath` と `WasmAotProfilePath` の説明があり、`WasmProfilers` と `RunAOTCompilation` の併用を拒否するエラー文言もある — [WasmApp.Common.targets](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/wasm/build/WasmApp.Common.targets)

ジェネリクスと Span の既知の制約

- 【公式Issue】（.NET 6〜7 期、2021〜2022 年）#60247 では、例外節を持つメソッド、値型のジェネリックインスタンスで AOT コードがないもの、`IEnumerable` への foreach / using、gsharedvt コード上の仮想呼び出しなどがインタープリターに落ちるとされた。当時は wasm で gsharedvt が無効で、インタープリターと AOT の遷移が高コストと報告された。Issue は修正済みとしてクローズ — [dotnet/runtime#60247](https://github.com/dotnet/runtime/issues/60247)
- 【公式Issue】（.NET 10.0.10 と .NET 11 Preview 5 で再現）#130545 は、`Span<TPixel>` を `stackalloc` し static abstract を使うジェネリックメソッドを `MakeGenericMethod` 経由で呼ぶと、wasm AOT の gsharedvt 共有コードで function signature mismatch のトラップが起きるというもの。同じメソッドの直接呼び出しではトラップしない。PR #131880 で 2026-08-06 にクローズ、マイルストーン 11.0.0 — [dotnet/runtime#130545](https://github.com/dotnet/runtime/issues/130545)
- 【公式Issue】Mono AOT のサイズ削減の追跡 Issue には、不要な gsharedvt フォールバックの抑止と、ジェネリクスの既定フォールバックをインタープリターにする調査が項目として並ぶ — [dotnet/runtime#80938](https://github.com/dotnet/runtime/issues/80938)
- 【検索要約】AOT 済み corelib 環境での `Nullable<独自struct>` のボックス化クラッシュ（#131945）、インターフェイスメソッドへの open-instance delegate のクラッシュ（#134707）が報告されている — [dotnet/runtime#131945](https://github.com/dotnet/runtime/issues/131945)、[dotnet/runtime#134707](https://github.com/dotnet/runtime/issues/134707)

Vector128 / PackedSimd

- 【公式ソース】インタープリターが組み込み処理の対象にする型は `Vector128`、`Vector128<T>`、`System.Numerics.Vector`、`Vector<T>`、`Vector4`、`PackedSimd`。`Vector64`、`Vector256`、`WasmBase` のディスパッチはない — [transform-simd.c](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/mono/mini/interp/transform-simd.c)
- 【公式ソース】同ファイルでは `Vector128.IsHardwareAccelerated` を定数 1 として処理し、`PackedSimd` の `IsSupported` / `IsHardwareAccelerated` は browser / WASI で 1 になる。この処理全体は `interp_simd_enabled` が前提 — [transform-simd.c](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/mono/mini/interp/transform-simd.c)
- 【公式ソース】Jiterpreter は SIMD 操作を Wasm ネイティブ命令へ変換し、Wasm SIMD がない場合は C または C# のスカラー実装に落ちる — [jiterpreter.md](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/docs/design/mono/jiterpreter.md)
- 【公式ソース】AOT では `WasmEnableSIMD=true` のとき `mattr=simd` が AOT 引数に加わる — [WasmApp.Common.targets](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/wasm/build/WasmApp.Common.targets)
- 【公式ソース】`PackedSimd` は `[Intrinsic]` `[CLSCompliant(false)]` の abstract class。`Swizzle`、`Bitmask`、`Min`、`Max`、`CompareLessThan`、`BitwiseSelect`、`AllTrue`、`AnyTrue` は public、`Shuffle` は internal — [PackedSimd.cs](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/libraries/System.Private.CoreLib/src/System/Runtime/Intrinsics/Wasm/PackedSimd.cs)

### Inferences
- `SortSpan<T, TComparer, TContext>` のような struct ジェネリックは、呼び出し元から閉じた型が静的に見える経路なら AOT 対象になると期待できる。一方、リフレクションや `MakeGenericMethod` を経由する経路は gsharedvt やインタープリターへ流れる可能性があり、ベンチマーク入口では避けるのが安全。
- インタープリターでは JIT のような struct 特殊化とインライン化の効果を前提にできないので、比較器を struct にする設計の利点は主に AOT 側で現れるはず。実測で確認が必要。
- #60247 の「gsharedvt は wasm で無効」は当時の記述で、#130545 のスタックに gsharedvt 共有コードが現れることと合わない。現行の挙動は後者を優先して考えるべき。
- `Vector256` / `Vector512` を使うコード経路は、少なくともインタープリターでは加速対象外。Wasm 向けの SIMD 実装は `Vector128` か `PackedSimd` に寄せるのが自然。

### Gaps
- 特定のメソッドが AOT 済みかインタープリター実行かを、現行版で一覧確認する公式手順は見つからなかった。
- .NET 11 の AOT における gsharedvt の有効条件、`Span<T>` 固有の制約、ジェネリック仮想メソッドの扱いを述べた公式文書は見つからなかった。
- AOT 側で `Vector128` の各 API がどこまで Wasm SIMD 命令に落ちるかは未確認（`simd-intrinsics.c` は読んでいない）。.NET 11 新設の `Vector128.Zip` / `Unzip` / `Concat` 系が Mono で加速されるかも未確認。
- `features.md` に Profiled AOT の節はなく、browser-wasm でのプロファイル収集手順は確認できなかった。`WasmProfilers` 併用エラーの発火条件の詳細も未確認。

## 3. Wasm 機能の利用状況と MSBuild プロパティ

### Takeaway
.NET 11 の Mono は SIMD128 と exnref 形式の Wasm 例外処理を既定で使い、スレッドは実験的で既定無効。relaxed SIMD は Mono では提供されず、tail call、memory64 を有効化するフラグは該当ファイルに見当たらなかった。最低ブラウザーは exnref により Chrome 137 / Firefox 131 / Safari 18.4 相当に上がる。

### Cited Findings

| 機能 | .NET 11 Mono の状態 | 制御 | 既定 | 出典 |
| --- | --- | --- | --- | --- |
| SIMD128 | 使用。`-msimd128`、AOT は `mattr=simd` | `WasmEnableSIMD` | 有効（未指定時は `$(WasmEnableExceptionHandling)` を継承） | 【公式ソース】[BrowserWasmApp.targets](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/browser/build/BrowserWasmApp.targets)、[features.md](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/wasm/features.md) |
| Relaxed SIMD | Mono では未提供 | なし | なし | 【公式Issue】[PR #130050](https://github.com/dotnet/runtime/pull/130050) |
| 例外処理 | exnref（`-fwasm-exceptions -s WASM_LEGACY_EXCEPTIONS=0`）。無効時は `-fexceptions` | `WasmEnableExceptionHandling` | 有効 | 【公式ソース】[BrowserWasmApp.targets](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/browser/build/BrowserWasmApp.targets)、[browser.proj](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/browser/browser.proj) |
| Tail call | `browser.proj` と `BrowserWasmApp.targets` に該当語なし | 確認できず | 確認できず | 【公式ソース】[browser.proj](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/browser/browser.proj)、[BrowserWasmApp.targets](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/browser/build/BrowserWasmApp.targets) |
| Bulk memory | wasm-opt のベースラインに `--enable-bulk-memory` | 専用プロパティは確認できず | 有効 | 【公式Issue】[#136857](https://github.com/dotnet/runtime/issues/136857) |
| スレッド | 実験的。`-pthread`、専用ランタイムパック | `WasmEnableThreads` | 無効 | 【公式ソース】[features.md](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/wasm/features.md)、[browser.proj](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/browser/browser.proj) |
| Memory64 | `MEMORY64` は両ファイルに該当なし | 確認できず | 確認できず | 【公式ソース】[browser.proj](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/browser/browser.proj)、[BrowserWasmApp.targets](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/browser/build/BrowserWasmApp.targets) |
| Wasm GC | 利用の根拠を確認できず | 確認できず | 確認できず | Gaps 参照 |

- 【公式Issue】wasm-opt のベースラインは `--enable-exception-handling`、`--enable-reference-types`、`--enable-multivalue`、`--enable-simd`、`--enable-bulk-memory`、`--enable-nontrapping-float-to-int`。Mono では `src/mono/browser/runtime/CMakeLists.txt` と `_RunWasmOptPostLink` に同じ一覧がハードコードされている。sign-ext と mutable-globals は binaryen の既定で有効 — [dotnet/runtime#136857](https://github.com/dotnet/runtime/issues/136857)
- 【公式ソース】ランタイム自体のリンクには `-s ALLOW_MEMORY_GROWTH=1`、`-s WASM_BIGINT=1`、`-s INITIAL_MEMORY=$(EmccInitialHeapSize)`、`-s STACK_SIZE=$(EmccStackSize)` が使われる — [browser.proj](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/browser/browser.proj)
- 【公式ソース】`WasmEnableSIMD=false`、`WasmEnableExceptionHandling=false`、`InvariantTimezone=true`、`EmccInitialHeapSize` / `EmccMaximumHeapSize` の指定、`RunAOTCompilation=true` などはネイティブワークロードを必要とする条件に含まれる — [WorkloadManifest.targets.in](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/nuget/Microsoft.NET.Workload.Mono.Toolchain.Current.Manifest/WorkloadManifest.targets.in)
- 【公式ソース】スレッド有効時は COOP / COEP ヘッダーが必要で、`[JSExport]` / `[JSImport]` はメインスレッドに限られ、`WasmEnableJsInteropByValue` が既定で true になる。専用パック `Microsoft.NETCore.App.Runtime.Mono.multithread.*` は `wasm-experimental` に含まれる — [features.md](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/wasm/features.md)、[BrowserWasmApp.targets](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/browser/build/BrowserWasmApp.targets)、[WorkloadManifest.json.in](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/nuget/Microsoft.NET.Workload.Mono.Toolchain.Current.Manifest/WorkloadManifest.json.in)
- 【公式Issue】`RelaxedSimd` API の PR #130050 はマイルストーン 12.0-preview1 で、最終的に Mono 実装を削除して CoreCLR 専用となり、Mono では `IsSupported` が false — [PR #130050](https://github.com/dotnet/runtime/pull/130050)。`release/11.0` の対応パスに `RelaxedSimd.cs` は存在しなかった（HTTP 404） — [RelaxedSimd.cs のパス](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/libraries/System.Private.CoreLib/src/System/Runtime/Intrinsics/Wasm/RelaxedSimd.cs)

最低実行環境

- 【Emscripten】Emscripten 6.0.2 の機能表では、`WASM_EXCEPTIONS` が Chrome 137 / Firefox 131 / Safari 18.4 / Node 22.0、`WASM_LEGACY_EXCEPTIONS` が Chrome 95 / Firefox 100 / Safari 15.2 / Node 17.0。Node 22.0 は `--experimental-wasm-exnref` が前提の暫定値と注記されている — [emscripten feature_matrix.py](https://raw.githubusercontent.com/emscripten-core/emscripten/6.0.2/tools/feature_matrix.py)
- 【Emscripten】Emscripten の未マージ PR #27575 のデバッグ出力では exnref の Node 最低版が 24.15 相当と表示され、上の機能表の Node 22.0 と食い違う — [emscripten#27575](https://github.com/emscripten-core/emscripten/pull/27575)
- 【公式Doc】Blazor のサポートブラウザーは Safari、Chrome、Edge、Firefox の最新版で、具体的な最低バージョンの記載はない — [Blazor supported platforms](https://learn.microsoft.com/en-us/aspnet/core/blazor/supported-platforms?view=aspnetcore-11.0)

### Inferences
- `WasmEnableExceptionHandling=false` だけを指定すると、`WasmEnableSIMD` を明示しない限り SIMD も無効になると読める。速度検証では両方を明示するのが安全。
- `WasmApp.Common.targets` には `WasmEnableSIMD` を false にするフォールバック行もあるが、`BrowserWasmApp.targets` 側の既定が先に効くとみて、browser-wasm の実効既定は features.md のとおり有効と解釈した（import 順は未確認）。
- 既定構成のままだと Safari 18.4 未満などでは起動できない可能性が高い。`WasmEnableExceptionHandling=false` は互換性の回避策になりうるが、例外処理の性能と SIMD 既定に影響する。
- tail call と memory64 は、該当ファイルに指定がないことから既定で使っていないと推定する。アドレス空間は wasm32 前提と考えられる。

### Gaps
- Wasm GC を Mono が使うかどうかを明示した資料は見つからなかった。検索でも該当なし。
- Mono 向けの正式な最低ブラウザー表は見つからなかった。上の数値は Emscripten の機能表からの導出で、実機では確認していない。
- `src/mono/browser/runtime/CMakeLists.txt` と LLVM 側の target feature は直接読んでおらず、tail call の不使用は確定できない。

## 4. JS interop

### Takeaway
`int[]` は双方向ともコピーされ、`Span<int>` と `ArraySegment<int>` はコピーなしの `MemoryView` になる。ただし `MemoryView` は JS 側で生成できないため、JS が持つデータをゼロコピーで C# に渡す単純な経路はない。テンプレート `wasmbrowser` / `wasmconsole` は実験的扱いのまま。

### Cited Findings

型マッピングとコピーの有無

- 【公式Doc】.NET 11 の表で `Span` / `ArraySegment` に対応する要素型は `Byte`、`Int32`、`Single`、`Double`。配列（`Array of`）は `Byte`、`Int32`、`Single`、`Double`、`JSObject`、`String`、`Object`。`Int64` は `Number` または `BigInt` のスカラーのみで配列は非対応 — [JSImport/JSExport interop](https://learn.microsoft.com/en-us/aspnet/core/client-side/dotnet-interop/?view=aspnetcore-11.0)
- 【公式Doc】配列のマーシャリングはコピーを作り、`Span` / `ArraySegment` はコピーを作らない。`Span` の `MemoryView` は呼び出しの間だけ有効で、`ArraySegment` の `MemoryView` は呼び出し後も生存する。後者の `dispose()` で proxy が破棄され配列の pin が解除される — [JSImport/JSExport interop](https://learn.microsoft.com/en-us/aspnet/core/client-side/dotnet-interop/?view=aspnetcore-11.0)
- 【公式Doc】`JSObject`、`Exception`、`Task`、`ArraySegment` は `GCHandle` と proxy を作り、大きな性能オーバーヘッドがある — [JSImport/JSExport interop](https://learn.microsoft.com/en-us/aspnet/core/client-side/dotnet-interop/?view=aspnetcore-11.0)
- 【公式Doc】`MemoryView` はランタイムだけが正しく生成でき、`Span` / `ArraySegment` を引数に持つ JS メソッドの import はできず、`Span` を返す .NET メソッドの export もできないと記載されている — [JSImport/JSExport interop](https://learn.microsoft.com/en-us/aspnet/core/client-side/dotnet-interop/?view=aspnetcore-11.0)

実装の挙動

- 【公式ソース】JS から C# への配列は `marshal_array_to_cs_impl` で処理され、`malloc` したバッファーへ `TypedArray.set()` でコピーされる。`Int32` は `Array` または `Int32Array`、`Byte` は `Array` または `Uint8Array`、`Double` は `Array` または `Float64Array`、`Single` は `Array` または `Float32Array` を受け付ける — [marshal-to-cs.ts](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/browser/runtime/marshal-to-cs.ts)
- 【公式ソース】`_marshal_span_to_cs` は値の `_viewType` と `_pointer` を読むため `MemoryView` 相当が必要。`_marshal_array_segment_to_cs` は C# 側で作られた `ArraySegment` の往復だけを認める — [marshal-to-cs.ts](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/browser/runtime/marshal-to-cs.ts)
- 【公式ソース】C# から JS への `int[]` は Wasm ヒープの `subarray` を `.slice()` した `Int32Array` になり、その後ネイティブバッファーが解放される。`Span` は `_marshal_span_to_js` で `new Span(ptr, length, type)`、`ArraySegment` は GC ハンドル付きの proxy になる — [marshal-to-js.ts](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/browser/runtime/marshal-to-js.ts)
- 【公式ソース】`IMemoryView` のメンバーは `set(source, targetOffset?)`、`copyTo(target, sourceOffset?)`、`slice(start?, end?)`、`length`、`byteLength`、`dispose()`、`isDisposed` — [dotnet.d.ts](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/browser/runtime/dotnet.d.ts)

ホストとテンプレート

- 【公式Doc】`dotnet.js` がランタイムを生成・起動し、`dotnet.create()`、`setModuleImports`、`getAssemblyExports`、`runMain` を使う。プロジェクトは `Microsoft.NET.Sdk.WebAssembly` SDK で `AllowUnsafeBlocks=true` が必要 — [WebAssembly Browser App interop](https://learn.microsoft.com/en-us/aspnet/core/client-side/dotnet-interop/wasm-browser-app?view=aspnetcore-11.0)
- 【公式Doc】`wasm-experimental` ワークロードまたは NuGet の `Microsoft.NET.Runtime.WebAssembly.Templates` で `wasmbrowser` と `wasmconsole` が入る。テンプレートは実験的と明記され、`wasmconsole` は Node.js または V8 のコンソールアプリとして動き、`main.mjs` を起点にする — [WebAssembly Browser App interop](https://learn.microsoft.com/en-us/aspnet/core/client-side/dotnet-interop/wasm-browser-app?view=aspnetcore-11.0)
- 【公式Doc】JS interop 自体は `wasm-experimental` がなくても既存 JS アプリへ組み込める。必要なのは `wasm-tools` — [WebAssembly Browser App interop](https://learn.microsoft.com/en-us/aspnet/core/client-side/dotnet-interop/wasm-browser-app?view=aspnetcore-11.0)

### Inferences
- 公式文書の記述（`Span` 引数の import 不可）と実装（`_marshal_span_to_js` が存在し、C# から JS へ `Span` を渡す経路がある）は素直に一致しない。実際に制約を受けるのは JS から C# へ `Span` を渡す方向と読めるので、実装前に小さな検証が必要。
- `Span<int>` を引数に取る `[JSExport]` を JS から任意のデータで呼ぶのは難しい。実用的な経路は次の 2 つと考えられる。(a) `Int32Array` を `int[]` 引数で渡し、結果を `int[]` で受ける（往復コピー）。(b) C# 側で確保した配列を `ArraySegment<int>` の `MemoryView` として JS に返し、`set()` で書き込み、ソート後に `copyTo()` や `slice()` で読む（JS 側のコピーは残るが、proxy と pin の管理が必要）。
- ソート時間を測るなら、interop のコピーが小さな n の結果を支配しないよう、C# 内で配列準備後のソート区間だけを計測するのが妥当。
- `long[]` / `Span<long>` は直接渡せないので、64 ビット要素の検証は C# 側でデータ生成するか `double` などで代替する必要がある。
- `wasmconsole` を Node で動かす場合、exnref 対応の Node が必要になる。必要な版は資料間で食い違う（第 3 節）ため、実機確認が前提。

### Gaps
- 質問にある `MemorySpan` という名前の型は一次資料に見当たらず、対応する概念は `MemoryView` / `IMemoryView` だった。
- interop 呼び出しのコストの公式実測値は見つからなかった。
- JS から C# へ配列を渡す際、`malloc` バッファーから .NET 配列への追加コピーがあるかは C# 側を読んでおらず未確認。
- Node.js の公式サポート範囲と最低版を明記した文書は見つからなかった。`wasmconsole` テンプレートの定義は想定パスで 404 となり、中身を確認していない。

## 5. 速度優先のビルド構成とサイズ

### Takeaway
速度優先の基本は `RunAOTCompilation=true` の Release publish で、SIMD、Wasm EH、`WasmStripILAfterAOT` は既定で有効。ネイティブの既定最適化はコンパイルが `-Oz` とサイズ寄りなので変更の余地があるが、効果の公式データはない。サイズの公式な目安は「AOT で IL 版の約 2 倍」のみで、.NET 11 の具体値は見つからなかった。

### Cited Findings

ビルド設定

- 【公式Doc】AOT は `dotnet publish -c Release` で実行され、開発時の実行では使われない。小規模プロジェクトでも数分かかることがある — [Blazor build tools and AOT](https://learn.microsoft.com/en-us/aspnet/core/blazor/webassembly-build-tools-and-aot?view=aspnetcore-11.0)
- 【公式ソース】Emscripten の最適化既定は、`_WasmDevel` で `-O0`、Debug で `-O1`、それ以外（Release を含む）で `-Oz`。Release の `WasmBitcodeCompileOptimizationFlag` と `EmccLinkOptimizationFlag` は `-O2` — [BrowserWasmApp.targets](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/browser/build/BrowserWasmApp.targets)
- 【公式ソース】`EmccCompileOptimizationFlag` と `EmccLinkOptimizationFlag` は targets のヘッダーに説明がある。`WasmNativeStrip` は既定 true で、true なら `WasmRunWasmOpt` も true になる — [WasmApp.Common.targets](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/wasm/build/WasmApp.Common.targets)、[BrowserWasmApp.targets](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/browser/build/BrowserWasmApp.targets)
- 【公式ソース】ランタイム本体の Release ビルドはコンパイル `-Oz`、リンク `-O2` — [browser.proj](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/browser/browser.proj)
- 【公式ソース】`WasmStripILAfterAOT` の既定は true — [WasmApp.Common.targets](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/wasm/build/WasmApp.Common.targets)。【公式Doc】.NET 8 では false で、.NET 10 で AOT ビルドの既定が有効になった — [Copilot Studio .NET 10 migration](https://devblogs.microsoft.com/dotnet/copilot-studio-dotnet-10-migration/)

トリミング、グローバリゼーション、圧縮

- 【公式ソース】トリミングは `PublishTrimmed` と `TrimMode` で制御し、起動時間とメモリを減らし AOT 時間も短縮する。`InvariantGlobalization` で ICU を外せる。`InvariantTimezone` はタイムゾーンデータを省く。publish 成果物には既定で gzip と Brotli の圧縮版が含まれる — [features.md](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/wasm/features.md)
- 【公式Doc】`wasm-tools` が入っていれば Release publish で自動的にランタイム再リンクが行われ、グローバリゼーション無効時のサイズ削減が特に大きい — [Blazor WebAssembly runtime performance](https://learn.microsoft.com/en-us/aspnet/core/blazor/performance/webassembly-runtime-performance?view=aspnetcore-11.0)
- 【公式ソース】WebCIL は既定有効、アセットのフィンガープリントは .NET 9 以降で既定有効 — [Microsoft.NET.Sdk.WebAssembly.Browser.targets](https://raw.githubusercontent.com/dotnet/runtime/release/11.0/src/mono/nuget/Microsoft.NET.Sdk.WebAssembly.Pack/build/Microsoft.NET.Sdk.WebAssembly.Browser.targets)

公表されているサイズ

- 【公式Doc】多くの AOT アプリは IL 版の約 2 倍のサイズ。理由は IL より Wasm の表現が大きいことと、管理 DLL が残ること（版を問わない一般的記述） — [Blazor build tools and AOT](https://learn.microsoft.com/en-us/aspnet/core/blazor/webassembly-build-tools-and-aot?view=aspnetcore-11.0)
- 【公式Doc】（.NET 10、2026-05-07）Copilot Studio は JIT 版と AOT 版の同梱構成で、両者の同一ファイルが 59（.NET 8）から 22（.NET 10）に減り、パッケージは約 15% 増。AOT のダウンロードは高速 LAN で約 6%（約 200 ms）、4G で約 17%（約 5 秒）遅い — [Copilot Studio .NET 10 migration](https://devblogs.microsoft.com/dotnet/copilot-studio-dotnet-10-migration/)
- 【コミュニティ】（.NET 8 / 9、2024 年）#110143 の報告では `dotnet.native.wasm.gz` が .NET 8 で 3.7 MB（strip なし）、2.5 MB（strip あり、ただし起動失敗）、.NET 9 で 4 MB（strip あり）。Issue は not planned でクローズ — [dotnet/runtime#110143](https://github.com/dotnet/runtime/issues/110143)
- 【コミュニティ】CODE Magazine（2026-08-24 更新）は、トリミングと圧縮後の初回ダウンロードを約 2〜4 MB、Brotli の削減を約 60〜70% としているが、.NET の版や測定条件の記載はない — [CODE Magazine](https://www.codemag.com/Article/2610051/Building-Cross-Platform-High-Performance-Apps-Using-WASM-and-.NET)

### Inferences
速度優先の出発点としては次の構成が妥当と考える。既定値も意図を明示するため記載している。`-O3` の 2 行は未検証の候補で、サイズ増に見合うかは計測で判断する。

```xml
<Project Sdk="Microsoft.NET.Sdk.WebAssembly">
  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
    <RunAOTCompilation>true</RunAOTCompilation>
    <WasmEnableSIMD>true</WasmEnableSIMD>
    <WasmEnableExceptionHandling>true</WasmEnableExceptionHandling>
    <PublishTrimmed>true</PublishTrimmed>
    <InvariantGlobalization>true</InvariantGlobalization>
    <InvariantTimezone>true</InvariantTimezone>
    <!-- 未検証: 既定は -Oz / -O2 -->
    <EmccCompileOptimizationFlag>-O3</EmccCompileOptimizationFlag>
    <EmccLinkOptimizationFlag>-O3</EmccLinkOptimizationFlag>
  </PropertyGroup>
</Project>
```

- AOT 済みコードのビットコード最適化には `WasmBitcodeCompileOptimizationFlag` が関係すると名前から推定できるが、利用者が上書きする前提のプロパティかは未確認。
- `EmccCompileOptimizationFlag` はネイティブファイルのコンパイル用なので、AOT 済みマネージドコードの速度にどこまで効くかは別途確認が必要。

### Gaps
- `wasmbrowser` や AOT 構成の .NET 11 での公式サイズ実測値は見つからなかった。ローカルでの publish 測定が必要。
- `-O3` などの速度寄りフラグに関する公式の推奨や効果の数値は見つからなかった。
- `PublishTrimmed` が SDK 既定で true になるかは未確認。`Microsoft.NET.Sdk.WebAssembly.Browser.targets` では設定されていない。同ファイルの内部変数 `_WasmInvariantGlobalization` の意味も未確認。

## 6. 公表されている性能値（JIT / JS との比較を含む）

### Takeaway
.NET 11 の Mono browser-wasm について、JIT や JS と比較した公式の数値は見つからなかった。公開値は旧版の機能別改善率や特定アプリの値に限られ、ソート性能の見積もりには使えないため、自前の計測が必要。

### Cited Findings

- 【公式Doc】AOT は CPU 負荷の高いアプリで大きな改善をもたらしうるとの定性的な記述のみで、倍率の記載はない — [Blazor build tools and AOT](https://learn.microsoft.com/en-us/aspnet/core/blazor/webassembly-build-tools-and-aot?view=aspnetcore-11.0)
- 【公式Doc】（.NET 8 Preview 2、2023-03-14）Jiterpreter により `Span<byte>.Reverse()` が約 46.7%、`String.Normalize()` が約 86.9%、JSON シリアライズのテストが約 40.8% 高速化。最終版では変わりうるとの注記あり — [ASP.NET Core updates in .NET 8 Preview 2](https://devblogs.microsoft.com/dotnet/asp-net-core-updates-in-dotnet-8-preview-2/)
- 【検索要約】（.NET 8 GA）コンポーネント描画が 20% 高速、JSON デシリアライズが 2 倍との記述 — [Announcing ASP.NET Core in .NET 8](https://devblogs.microsoft.com/dotnet/announcing-asp-net-core-in-dotnet-8/)
- 【公式Doc】（.NET 8 から .NET 10、2026-05-07）Copilot Studio で初回呼び出しが約 20%、以降の呼び出しが約 5% 高速化。アプリ固有の値 — [Copilot Studio .NET 10 migration](https://devblogs.microsoft.com/dotnet/copilot-studio-dotnet-10-migration/)
- 【公式Doc】Microsoft Edge の強化セキュリティモードでは Wasm がインタープリター実行となり、性能が下がる可能性がある — [Blazor WebAssembly runtime performance](https://learn.microsoft.com/en-us/aspnet/core/blazor/performance/webassembly-runtime-performance?view=aspnetcore-11.0)
- 【公式Issue】（.NET 6 期、2021 年）#60247 の当初の報告では、あるテストがインタープリター約 1.4 秒に対し AOT 約 1.7 秒と AOT の方が遅かった。その後の修正で AOT が大幅に速くなったとメンテナーが述べているが、修正後の数値はない — [dotnet/runtime#60247](https://github.com/dotnet/runtime/issues/60247)
- 【コミュニティ】DevExpress の表では AOT 無効 / 有効で、DxGrid の 10 万行フィルター 220 / 117 ms、行選択 320 / 80 ms、ページ移動 900 / 570 ms、10 万行ソート 1000 / 950 ms、別ページへの遷移 735 / 420 ms。ハードウェア、ブラウザー、.NET 版の記載はない — [DevExpress Blazor docs](https://docs.devexpress.com/Blazor/404490/common-concepts/optimize-performance-and-size-of-blazor-wasm-applications)
- 【コミュニティ】CODE Magazine は CPU 負荷処理で Wasm が JS より 1.5〜3 倍速いとするが、.NET の版や測定条件の記載はなく、.NET 固有の値とも言えない — [CODE Magazine](https://www.codemag.com/Article/2610051/Building-Cross-Platform-High-Performance-Apps-Using-WASM-and-.NET)

### Inferences
- DevExpress の 10 万行ソートで AOT の差が小さいのは、UI 処理やフレームワーク側の時間が支配的な可能性がある。純粋なソートアルゴリズムの AOT 効果の指標にはならない。
- SortAlgorithms の評価には、同一マシンで少なくとも「インタープリター + Jiterpreter」「AOT + SIMD」「ネイティブ .NET JIT」「JS の `TypedArray.prototype.sort`」を、同一データパターンと同一ブラウザーで比較する必要がある。Jiterpreter は実行回数に応じて trace を作るため、ウォームアップの扱いを明示する。

### Gaps
- .NET 11 の Mono インタープリター、Jiterpreter、AOT と CoreCLR JIT、または JS を比較した公式ベンチマークは見つからなかった。
- `Span<T>` と struct ジェネリック比較器を使うソート処理に関する公開ベンチマークは見つからなかった。
- 検索で見つかった Medium の 6.2 倍という主張と Syncfusion のサイズ値は、方法論や妥当性を確認できないため採用しなかった。
