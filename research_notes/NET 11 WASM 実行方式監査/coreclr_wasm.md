# CoreCLR on WebAssembly の現状（.NET 11 GA / .NET 12 開発ライン、確認日 2026-11-19）

凡例: 「merged」= main 等へマージ済み、「in-progress」= 未完了の PR/issue、「planned」= epic 上の未着手項目。ライブ確認（NuGet、feed、ブランチ内容）はすべて 2026-11-19 時点。原文の長い引用は避け、日本語で要約している。識別子・プロパティ名・フラグは原文のまま。

## 1. 正確なステータス: in-tree のみか、nightly か、.NET 11 preview か、.NET 12 予定か。パックは公開されているか。.NET 11 SDK で選択できるか

### Takeaway
「公開利用可能なリリースは存在しない」という認識は誤り。.NET 11 GA（11.0.0 / SDK 11.0.100、2026-11-10）で、NuGet.org 上の CoreCLR 版 runtime pack と `UseMonoRuntime=false` によるオプトインが使える。ただし既定は Mono のままで、位置付けは実験的。本命の R2R 経路は .NET 12 開発ライン（main）にしかない。

### Cited Findings
- **GA 状況**: .NET 11 の最新は 11.0.0 / SDK 11.0.100、リリース日 2026-11-10、STS、active。同インデックスに 12.0 チャネルは未掲載 — [releases-index.json](https://builds.dotnet.microsoft.com/dotnet/release-metadata/releases-index.json)
- **CoreCLR 版 runtime pack は NuGet.org で公開済み**: `Microsoft.NETCore.App.Runtime.browser-wasm` 11.0.0（published 2026-11-10T13:11:56Z、45,821,999 bytes）。中身に `libcoreclr_static.a`、`libcoreclrpal.a`、`libBrowserHost.a` を含むので Mono ではなく CoreCLR — [NuGet catalog](https://api.nuget.org/v3/catalog0/data/2026.11.10.14.36.27/microsoft.netcore.app.runtime.browser-wasm.11.0.0.json)
- **Mono 版は別パッケージ**: `Microsoft.NETCore.App.Runtime.Mono.browser-wasm` 11.0.0（25,357,229 bytes）— [NuGet catalog](https://api.nuget.org/v3/catalog0/data/2026.11.10.14.46.48/microsoft.netcore.app.runtime.mono.browser-wasm.11.0.0.json)
- **公開プレビュー系列**: 両パッケージとも 11.0.0-preview.6 / preview.7 / rc.1 / rc.2 / rc.3 / 11.0.0 が NuGet.org に存在 — [CoreCLR 版 index](https://api.nuget.org/v3-flatcontainer/microsoft.netcore.app.runtime.browser-wasm/index.json)、[Mono 版 index](https://api.nuget.org/v3-flatcontainer/microsoft.netcore.app.runtime.mono.browser-wasm/index.json)
- **選択プロパティ**: .NET 11 Preview 5 のリリースノートが、Blazor WebAssembly および `Microsoft.NET.Sdk.WebAssembly` プロジェクトで `<UseMonoRuntime>false</UseMonoRuntime>`（または `/p:UseMonoRuntime=false`）によりオプトインできると記載。確認方法として `globalThis.getDotnetRuntime(0).INTERNAL.GetDotNetRuntimeHeap()` が CoreCLR でのみ成功することも記載 — [Preview 5 runtime notes](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview5/runtime.md)
- **SDK 11.0.100 の既定は Mono**: `Sdks\Microsoft.NET.Sdk.WebAssembly\Sdk\Sdk.props` 18 行目で `UseMonoRuntime` 未指定時に `true`。`Microsoft.NETCoreSdk.BundledVersions.props` では net11.0 の `KnownFrameworkReference` が `Microsoft.NETCore.App.Runtime.**RID**`、`RuntimePackLabels="Mono"` の `KnownRuntimePack` が `Microsoft.NETCore.App.Runtime.Mono.**RID**` を指す（ローカルの `C:\Program Files\dotnet\sdk\11.0.100` を直接確認、URL なし）。この解決の仕組みは [PR #133728](https://github.com/dotnet/runtime/pull/133728) の説明と一致
- **既定は変わらないという公式声明**: 2026-05-13 の MAUI 記事が Blazor WebAssembly は影響を受けないとし、"WASM continues to use Mono" と記述、.NET 11 で変更なしとする — [.NET Blog](https://devblogs.microsoft.com/dotnet/dotnet-maui-moves-to-coreclr-in-dotnet-11/)
- **GA ドキュメントの扱い**: What's new（ms.date 2026-11-10）は CoreCLR-on-WebAssembly の改善と、libraries テストスイートが end-to-end で走ることを記載するが、有効化方法は書いていない — [What's new in .NET 11 runtime](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-11/runtime)。ASP.NET Core 11 の What's new は先頭 100,000 文字に CoreCLR / Mono / `UseMonoRuntime` の記載なし（残り約 6,800 文字は未読）— [ASP.NET Core 11 release notes](https://learn.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-11?view=aspnetcore-11.0)
- **in-tree ドキュメントの位置付け**: `wasm.md` は実験的サポートで開発中と明記し、ソースビルド手順のみを記載 — [wasm.md](https://github.com/dotnet/runtime/blob/main/docs/workflow/building/coreclr/wasm.md)。一方 WASI 側は、出荷版 WASI SDK がまだ CoreCLR の app builder を選択しない in-tree 限定の手順 — [wasi-r2r.md](https://github.com/dotnet/runtime/blob/main/docs/workflow/building/coreclr/wasi-r2r.md)
- **nightly**: dotnet12 feed に `Microsoft.NETCore.App.Runtime.browser-wasm` 12.0.0-alpha が 140 版、最新 `12.0.0-alpha.1.26568.107`。`Microsoft.NETCore.App.Crossgen2.win-x64`、`Microsoft.NET.Sdk.WebAssembly.Pack` も同版あり — [dotnet12 feed index](https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet12/nuget/v3/flat2/microsoft.netcore.app.runtime.browser-wasm/index.json)
- **実利用の痕跡**: 外部ユーザーが Preview 6/7 以降で `dotnet new blazorwasm` + `UseMonoRuntime=false` の再現手順を報告している — [#131874](https://github.com/dotnet/runtime/issues/131874)（2026-08-05 起票、open）
- **ユーザー指定の issue の性質**: [#135046](https://github.com/dotnet/runtime/issues/135046)（2026-10-01 起票、open、milestone 12.0.0）は `Wasm.Build.Tests` のカバレッジ追跡であり、全体トラッカーではない。全体 epic は [#134337](https://github.com/dotnet/runtime/issues/134337)（2026-09-21 起票、open、milestone 12.0.0）

### Inferences
- 区分としては「.NET 11 GA に同梱されたオプトインの実験的機能」が最も正確。in-tree 限定でも nightly 限定でもない。
- `wasm.md` は `UseMonoRuntime` に触れていないため、この文書だけを読むとソースビルド必須に見える（誤認の原因になり得る）。
- epic #134337 の未チェック項目「CoreCLR WebAssembly パックのビルドと公開」は、NuGet 上にパックが既に存在する事実と字面上は食い違う。ワークロード解決や正式な出荷形態まで含む項目と解釈するのが妥当だが、issue 本文からは断定できない。

### Gaps
- `UseMonoRuntime=false` のブラウザ向け利用に関する Microsoft の正式なサポート声明（supported / unsupported の明言）は見つからなかった。dotnet/docs のコード検索ヒットは 2 ファイルのみ（ファイル名からはモバイル関連）、dotnet/AspNetCore.Docs はヒットなし。検索インデックスの網羅性は保証されない。
- 本調査ではビルド・実行をしていないため、GA 版で実際に動作するかは計測タスクでの確認が必要。

## 2. 実行方式: インタープリタ、RyuJIT Wasm バックエンド、crossgen2/R2R、その他。実装済みと計画の区別

### Takeaway
設計は「CoreCLR インタープリタ + RyuJIT による事前コンパイル（R2R）」で、実行時 JIT はない。.NET 11 GA の SDK 経路で配線されているのはインタープリタのみと読める。ブラウザ向け R2R は main にマージ済みだがオプトインで、既定化は未完了。

### Cited Findings
- **設計**: Preview 7 ノートは、ポータブルなインタープリタと R2R の組み合わせで、RyuJIT を事前コード生成に再利用するモデルと説明 — [Preview 7 runtime notes](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview7/runtime.md)
- **第 1 段階はインタープリタ**: 2025-11-10 起票の [#121511](https://github.com/dotnet/runtime/issues/121511) は .NET 11 時間枠の early preview をインタープリタで開始し、R2R / RyuJIT の土台にすると記述。2026-08-11 に pavelsavara が「ほぼ完了」として close を提案し同日 close。残項目はデバッガと全テスト通過
- **RyuJIT Wasm（merged、.NET 11 分）**: [#121141](https://github.com/dotnet/runtime/issues/121141) は 2026-07-30 に「.NET 11 分は完了」として close。ベースライン、EH/GC、Interop、SIMD & Intrinsics、Runtime Async はチェック済み。Profiling、Diagnostics、少数シナリオの性能は未チェック。ブラウザのマルチスレッド、WASI、WASM64 はスコープ外
- **RyuJIT Wasm は crossgen2 に同梱**: `Microsoft.NETCore.App.Crossgen2.win-x64` 11.0.0 に `tools/clrjit_universal_wasm_x64.dll`（1,857,832 bytes）が存在 — [NuGet catalog](https://api.nuget.org/v3/catalog0/data/2026.11.10.14.13.09/microsoft.netcore.app.crossgen2.win-x64.11.0.0.json)
- **11.0 のブラウザ SDK には R2R の配線がない**: release/11.0 の `Microsoft.NET.Sdk.WebAssembly.Browser.targets` / `.props` に `ReadyToRun` / `Crossgen` の参照は 0 件（main は targets 15 件、props 4 件）— [release/11.0 targets](https://github.com/dotnet/runtime/blob/release/11.0/src/mono/nuget/Microsoft.NET.Sdk.WebAssembly.Pack/build/Microsoft.NET.Sdk.WebAssembly.Browser.targets)、[main targets](https://github.com/dotnet/runtime/blob/main/src/mono/nuget/Microsoft.NET.Sdk.WebAssembly.Pack/build/Microsoft.NET.Sdk.WebAssembly.Browser.targets)。`BrowserWasmApp.CoreCLR.targets` も release/11.0 では R2R / composite / crossgen の記述なし — [release/11.0](https://github.com/dotnet/runtime/blob/release/11.0/src/mono/browser/build/BrowserWasmApp.CoreCLR.targets)
- **11.0.0 の runtime pack に R2R イメージなし**: パック内のファイル名に `r2r` を含むものは 0 件 — [NuGet catalog](https://api.nuget.org/v3/catalog0/data/2026.11.10.14.36.27/microsoft.netcore.app.runtime.browser-wasm.11.0.0.json)
- **R2R の SDK 対応（merged、main のみ）**: [PR #133378](https://github.com/dotnet/runtime/pull/133378)（2026-09-17 merge、`PublishReadyToRun` 既定 false、publish はアプリごとに閉包全体を crossgen）、[PR #134618](https://github.com/dotnet/runtime/pull/134618)（2026-10-01 merge、composite）、[dotnet/sdk#56395](https://github.com/dotnet/sdk/pull/56395)（2026-10-02 merge、SDK 側の出力名対応）
- **main の現在の既定**: `PublishReadyToRun` は既定 `false`、`-p:PublishReadyToRun=true` でオプトイン。コメントは既定化を #132466 で追跡とする — [Browser.CoreCLR.targets](https://github.com/dotnet/runtime/blob/main/src/mono/nuget/Microsoft.NET.Sdk.WebAssembly.Pack/build/Microsoft.NET.Sdk.WebAssembly.Browser.CoreCLR.targets)。`PublishReadyToRunComposite` は既定 `true` — [Browser.props](https://github.com/dotnet/runtime/blob/main/src/mono/nuget/Microsoft.NET.Sdk.WebAssembly.Pack/build/Microsoft.NET.Sdk.WebAssembly.Browser.props)
- **記述の食い違い**: 既定化 issue [#132466](https://github.com/dotnet/runtime/issues/132466) は 2026-11-03 に close 済みだが、上記 main のコードは既定 false のままで、epic [#134337](https://github.com/dotnet/runtime/issues/134337) の該当項目も未チェック
- **混在実行**: R2R に含まれないメソッドはインタープリタで動く。[#137553](https://github.com/dotnet/runtime/issues/137553) は composite に含まれない generic な `Vector128<T>` インスタンス化を例に挙げ、[#130524](https://github.com/dotnet/runtime/issues/130524)（2026-07-10）は全 IL をダウンロードしてインタープリタで起動する R1 構想を説明
- **Mono の AOT 経路は CoreCLR に適用されない**: SDK 11.0.100 のワークロードマニフェストは AOT パックの import を `UseMonoRuntime != false` に限定（ローカルの `sdk-manifests\11.0.100\microsoft.net.workload.mono.toolchain.current\11.0.100\WorkloadManifest.targets` 161 行目、URL なし）。[#135046](https://github.com/dotnet/runtime/issues/135046) も、CoreCLR の事前コンパイルは `PublishReadyToRun` を明示して検証すべきで、`RunAOTCompilation=true` が受理されることは根拠にならないとする

### Inferences
- .NET 11 GA + `UseMonoRuntime=false` の実行はインタープリタのみ、と判断するのが妥当（SDK の配線なし + パックに R2R なし）。crossgen2 側にはバックエンドがあるので、GA で `PublishReadyToRun=true` を手動で通せる可能性は残るが、公式経路ではない。
- したがって CoreCLR の R2R 性能を測るには .NET 12 alpha パックかソースビルドが必要。

### Gaps
- GA SDK で `PublishReadyToRun=true` を指定した場合の実挙動（無視、エラー、部分動作）は未確認。計測タスクの確認事項。
- #132466 が close された理由は issue 上に説明がなく、timeline にも関連 PR を確認できなかった。

## 3. 必要な Wasm 機能と Vector128 / PackedSimd

### Takeaway
SIMD128 と標準化版 EH（exnref）は必須で無効化できない。`Vector128` / `PackedSimd` のハードウェア実行は R2R コードのみで、インタープリタでは非対応として振る舞う。つまり .NET 11 GA の経路では、マネージドコードの SIMD パスは実質使われない。

### Cited Findings
- **SIMD128 と EH は必須**: release/11.0 の `BrowserWasmApp.CoreCLR.targets` の既定フラグは `-fwasm-exceptions -sWASM_LEGACY_EXCEPTIONS=0 -msimd128`。`WasmEnableExceptionHandling` / `WasmEnableSIMD` を無効化するとビルドエラー — [release/11.0](https://github.com/dotnet/runtime/blob/release/11.0/src/mono/browser/build/BrowserWasmApp.CoreCLR.targets)
- **ランタイム本体のビルドフラグ**: `-fwasm-exceptions`、`-sWASM_LEGACY_EXCEPTIONS=0`、`-mbulk-memory`、`-msimd128` — [configureplatform.cmake](https://github.com/dotnet/runtime/blob/release/11.0/eng/native/configureplatform.cmake)
- **EH flavour は exnref**: `try_table` / `throw_ref` へ移行（[PR #129851](https://github.com/dotnet/runtime/pull/129851)、2026-06-26 merge）。Node 実行には `--experimental-wasm-exnref` を指定 — [wasm.md](https://github.com/dotnet/runtime/blob/main/docs/workflow/building/coreclr/wasm.md)
- **wasm-opt のベースライン機能（main）**: exception-handling、reference-types、multivalue、simd、bulk-memory、nontrapping-float-to-int。relaxed SIMD と threads は利用側が個別に追加する扱い — [eng/native.wasm.targets](https://github.com/dotnet/runtime/blob/main/eng/native.wasm.targets)
- **Relaxed SIMD**: ブラウザの CoreCLR R2R は既定オフで `PublishReadyToRunInstructionSet=relaxed-simd` によりオプトイン、WASI は既定オン — [PR #137112](https://github.com/dotnet/runtime/pull/137112)（2026-11-15 merge、main）
- **Tail call**: 呼び出し先がインタープリタの場合の問題により、Wasm では全 tail call が無効。暗黙 tail call の効果調査は未着手 — [#135985](https://github.com/dotnet/runtime/issues/135985)（2026-10-16 起票、open）
- **スレッド**: ブラウザのマルチスレッドは RyuJIT 作業のスコープ外 — [#121141](https://github.com/dotnet/runtime/issues/121141)。.NET 12 epic にもスレッド項目はない — [#134337](https://github.com/dotnet/runtime/issues/134337)
- **Memory64**: WASM64 はスコープ外 — [#121141](https://github.com/dotnet/runtime/issues/121141)、汎用の追跡は [#94108](https://github.com/dotnet/runtime/issues/94108)（open）。`corerun` は `-sMAXIMUM_MEMORY=4294967296`（4 GiB）— [CMakeLists.txt](https://github.com/dotnet/runtime/blob/release/11.0/src/coreclr/hosts/corerun/CMakeLists.txt)
- **インタープリタの SIMD**: `System.Numerics.Vector` と `System.Runtime.Intrinsics.Vector*` の `IsHardwareAccelerated` は false、他はマネージド実装へフォールバック。アーキテクチャ固有 intrinsics は `IsSupported` が false で、それ以外は `PlatformNotSupportedException`。release/11.0 と main で同じ — [release/11.0 intrinsics.cpp](https://github.com/dotnet/runtime/blob/release/11.0/src/coreclr/interpreter/intrinsics.cpp)、[main intrinsics.cpp](https://github.com/dotnet/runtime/blob/main/src/coreclr/interpreter/intrinsics.cpp)
- **JIT/R2R の SIMD**: JIT は `PackedSimd` と `Vector128` を必須ベースラインとして扱い、実質オフにできない。インタープリタが false を返す点が定数畳み込みの前提条件として残り、`TensorPrimitives` で約 100 倍の低速化があると記載。Mono のインタープリタはこれらを実行する — [#137553](https://github.com/dotnet/runtime/issues/137553)（2026-11-14 起票、open、milestone 12.0.0）
- **RyuJIT の PackedSimd 対応**: Preview 7 で大半の `PackedSimd` intrinsics に対応 — [Preview 7 runtime notes](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview7/runtime.md)。BCL 側の活用は継続中（[PR #129838](https://github.com/dotnet/runtime/pull/129838) 2026-06-25、[PR #135896](https://github.com/dotnet/runtime/pull/135896) / [PR #137562](https://github.com/dotnet/runtime/pull/137562) 2026-11-17、いずれも merge）
- **自動ベクトル化なし**: `ArraySum` で CoreCLR R2R はスカラーのまま、Mono LLVM AOT は 4 レーン化 — [#135981](https://github.com/dotnet/runtime/issues/135981)（2026-10-16）

### Inferences
- `Vector128.IsHardwareAccelerated` で分岐するコードは、.NET 11 GA のインタープリタ経路ではスカラー側に落ちる。Mono（インタープリタでも SIMD を実行）との比較では、この点が結果を大きく左右する。
- R2R でも、事前生成されなかった generic インスタンス化はインタープリタに落ちて SIMD を失う。`TComparer` / `TContext` を struct generic で流す本リポジトリの構成は影響を受ける可能性があり、計測時は当該メソッドが実際に R2R で動いたかの確認が必要（[#134559](https://github.com/dotnet/runtime/issues/134559) が同型の事例）。

### Gaps
- Wasm GC の採用計画は見つからなかった。ベースライン機能一覧にも含まれない。
- CoreCLR 版の最低対応ブラウザバージョンを定めた公式記述は見つからなかった。

## 4. JS interop: [JSExport]/[JSImport] は動くか、dotnet.js は Mono と共有か

### Takeaway
JS interop は CoreCLR 向けに実装済みで、テスト対象にも入っている。`dotnet.js` はファイル名と役割は同じだが実装は Mono と別物。

### Cited Findings
- **実装状況**: [#121511](https://github.com/dotnet/runtime/issues/121511) の Stage 3「Initial JS interop」はチェック済み、Stage 4/5 の Blazor 対応もチェック済み
- **CoreCLR 専用のマネージド実装**: `JavaScriptExports.CoreCLR.cs`、`JSHostImplementation.CoreCLR.cs` が Mono 版と並存 — [release/11.0 ディレクトリ](https://github.com/dotnet/runtime/tree/release/11.0/src/libraries/System.Runtime.InteropServices.JavaScript/src/System/Runtime/InteropServices/JavaScript)
- **パック同梱**: `System.Runtime.InteropServices.JavaScript.dll` と `libSystem.Runtime.InteropServices.JavaScript.Native.a/.js` が CoreCLR パックに存在 — [NuGet catalog](https://api.nuget.org/v3/catalog0/data/2026.11.10.14.36.27/microsoft.netcore.app.runtime.browser-wasm.11.0.0.json)
- **テスト**: release/11.0 の `tests.proj` は browser + CoreCLR のスモークテストに `System.Runtime.InteropServices.JavaScript.UnitTests` を含む — [tests.proj](https://github.com/dotnet/runtime/blob/release/11.0/src/libraries/tests.proj)
- **R2R 固有の修正例**: 64-bit 値（long/BigInt、double）の破損 [#129802](https://github.com/dotnet/runtime/issues/129802) は 2026-06-25 close
- **ホストは別実装**: CoreCLR は `src/native/corehost/browserhost` と `src/native/libs/Common/JavaScript`（loader 等）、Mono は `src/mono/browser/runtime/loader` — [browserhost ReadMe](https://github.com/dotnet/runtime/blob/release/11.0/src/native/corehost/browserhost/ReadMe.md)、[Mono loader](https://github.com/dotnet/runtime/tree/release/11.0/src/mono/browser/runtime/loader)
- **サイズにも差**: `dotnet.js` は CoreCLR 33,464 / Mono 36,540 bytes、`dotnet.runtime.js` は 49,175 / 197,545 bytes、`dotnet.d.ts` は 25,101 / 25,928 bytes — [CoreCLR catalog](https://api.nuget.org/v3/catalog0/data/2026.11.10.14.36.27/microsoft.netcore.app.runtime.browser-wasm.11.0.0.json)、[Mono catalog](https://api.nuget.org/v3/catalog0/data/2026.11.10.14.46.48/microsoft.netcore.app.runtime.mono.browser-wasm.11.0.0.json)
- **互換の追い上げ**: `invokeLibraryInitializers` を CoreCLR ローダーに実装（Preview 5）— [Preview 5 runtime notes](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview5/runtime.md)
- **再リンクとの関係**: runtime pack はフレームワーク自身の `[UnmanagedCallersOnly]` 用 thunk のみを持ち、アプリ側の reverse thunk は Release の trimmed publish で既定となる再リンクで組み込まれる — [BrowserWasmApp.CoreCLR.targets](https://github.com/dotnet/runtime/blob/release/11.0/src/mono/browser/build/BrowserWasmApp.CoreCLR.targets)

### Inferences
- 公開 API は揃える方向だが実装が別なので、Mono の内部 API や `INTERNAL` に依存するコードは移植確認が必要。

### Gaps
- `[JSExport]` が再リンクなしの構成で動くかは確認できていない。計測タスクで実機確認が必要。
- `dotnet.d.ts` の差分（API 単位の互換性）は未精査。

## 5. 公開されているサイズ・性能データ

### Takeaway
データは存在するが大半は開発途中のビルドに対するもので、GA 版インタープリタの公式ベンチマークは見つからなかった。傾向としては、CoreCLR R2R は Mono AOT より遅いベンチマークが多数で、ランタイム本体も Mono より大きい。

### Cited Findings
**サイズ（11.0.0 パック、2026-11-10。再リンク・トリム前。圧縮値は nupkg 内 deflate）**
- `dotnet.native.wasm`: CoreCLR 4,754,941 bytes（圧縮 1,822,400）/ Mono 3,150,989 bytes（圧縮 1,253,035）— [CoreCLR catalog](https://api.nuget.org/v3/catalog0/data/2026.11.10.14.36.27/microsoft.netcore.app.runtime.browser-wasm.11.0.0.json)、[Mono catalog](https://api.nuget.org/v3/catalog0/data/2026.11.10.14.46.48/microsoft.netcore.app.runtime.mono.browser-wasm.11.0.0.json)
- `System.Private.CoreLib.dll`: CoreCLR 5,642,024 bytes / Mono 5,422,888 bytes — 同上

**R2R のサイズ・起動（[#124218](https://github.com/dotnet/runtime/issues/124218)、2026-11-06 更新。いずれもローカル計測）**
- Havit アプリ、runtime `cfe8a6c4`、PGO なし: composite の起動は per-assembly 比で cold 542 vs 467 ms、warm 318 vs 275 ms（約 16% 遅い）。ダウンロード 7.5% 増、最大モジュール 5.3 MiB vs 2.1 MiB
- runtime `8970fe8`（Release、macOS/arm64）: デバッグ情報の除去等でマネージドコード到達が cold/warm 1006/519 ms → 721/286 ms、ダウンロード 12.6% 減。ホストが高負荷の同一セッション比較との注記あり
- davidwrighton（2026-10-05）: composite は非圧縮サイズが大幅に増えるが、圧縮後は per-assembly より小さいと報告

**スループット**
- CoreCLR R2R vs Mono AOT（build `3074629 / 20260913.1`、2026-09-13。gist に runtime commit の記載なし）: 比較可能 5,260 件中 4,936 件で CoreCLR が遅い。ただし参照値がほぼ 0 の行を含み、issue 側も再計測と検証を求めている — [gist](https://gist.github.com/radekdoulik/2e9d3b63bff325d632b840c5264ac6d3)、[#124218](https://github.com/dotnet/runtime/issues/124218)
- コード生成比較（revision `29d644d2`、Edge 154、Emscripten 6.0.2、2026-10-16）: `ArraySum`（4096 要素）は CoreCLR R2R 8,651.73 ns / Mono LLVM AOT 2,118.49 ns（約 4.07 倍）。`CountedLoop` の約 302 倍は閉形式化の有無による差で、一般的な数値性能ではない — [#135981](https://github.com/dotnet/runtime/issues/135981)
- `Span<Char>.CopyToOverlap(3000)` は Mono インタープリタ比で CoreCLR composite R2R が約 20 倍遅い、`Span<Byte>` は約 13.6 倍（build `20261006.3`）。原因は emsdk 6.0.2 の `memmove`。ブラウザ限定の回避ラッパーが入っており、ローカル再現で char 約 2800-2900 → 650-800 ns — [#136012](https://github.com/dotnet/runtime/issues/136012)（2026-10-17、open）
- `TensorPrimitives.IndexOfMax<double>(3079)`: インタープリタ 626,058 ns / per-assembly R2R 11,736,502 ns（18.75 倍遅い、build `20260923.2`、runtime `806095e5`）。R2R なのに当該メソッドが解釈実行に残るのが原因 — [#134559](https://github.com/dotnet/runtime/issues/134559)。composite では 4,040 ns vs per-assembly 10,652,060 ns（PR の初期リビジョン）— [#124218](https://github.com/dotnet/runtime/issues/124218)
- ダッシュボード: [.NET 12 focus](https://blazor-playground.github.io/macro-benchmarks/?redirect=%2Fmacro-benchmarks%2Fnet12-focus)（[#124218](https://github.com/dotnet/runtime/issues/124218) から参照、内容は未取得）

### Inferences
- 上記の R2R 数値は 2026-09〜10 の開発ビルドのもので、その後も最適化が続いている（例: [PR #137770](https://github.com/dotnet/runtime/pull/137770) 2026-11-18 merge）。現時点の性能として引用する場合は計測し直すべき。
- ランタイム単体のサイズ差（非圧縮で約 1.6 MB）はアプリ全体のサイズ差を意味しない。再リンクとトリムの影響が未反映。

### Gaps
- .NET 11 GA のインタープリタ対 Mono インタープリタ（jiterpreter あり）の公式比較は見つからなかった。
- GA 版 Blazor テンプレートの CoreCLR / Mono ダウンロードサイズ比較も見つからなかった。

## 6. Mono 置換の計画と、runtime チームが挙げるリスク・ブロッカー

### Takeaway
epic #134337 の目標は .NET 12 で CoreCLR 上で WebAssembly アプリを動かすことで、既定化と Mono パック出荷停止が項目に入っている。いずれも未完了の計画で、確定した出荷約束ではない。

### Cited Findings
- **段階の説明**: .NET 11 は early preview、.NET 12 で安定性・性能・円滑な移行に注力 — [#121511](https://github.com/dotnet/runtime/issues/121511)。.NET 12 以降で本番品質を目指す — [#121141](https://github.com/dotnet/runtime/issues/121141)
- **planned（epic、2026-10-14 時点の本文）**: Release publish での R2R 既定化、macrobenchmark 検証、PGO、CoreCLR パックのビルドと公開、Debug / Release / CI の既定ランタイム化、デバッグと Hot Reload、VS / VS Code 統合、移行ガイド公開 — [#134337](https://github.com/dotnet/runtime/issues/134337)
- **Mono 削除**: epic の Mono 項目は Mono WebAssembly パックの出荷停止で、[#134397](https://github.com/dotnet/runtime/issues/134397)（2026-09-22 起票、2026-11-12 更新、open、milestone 12.0.0）が .NET 12 の Mono runtime / cross-AOT パック削除を追跡。Mono が必要な箇所はバージョン固定で対応する方針。SDK の整理は [#133411](https://github.com/dotnet/runtime/issues/133411)
- **feed の状況**: dotnet12 feed の Mono パックは 130 版で最新 `12.0.0-alpha.1.26563.108`、CoreCLR パックは 140 版で最新 `12.0.0-alpha.1.26568.107` — [Mono 版 index](https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet12/nuget/v3/flat2/microsoft.netcore.app.runtime.mono.browser-wasm/index.json)、[CoreCLR 版 index](https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet12/nuget/v3/flat2/microsoft.netcore.app.runtime.browser-wasm/index.json)
- **未解決のリスク（すべて open、確認 2026-11-19 時点の検索結果に基づく）**
  - デバッグ・診断: epic の Diagnostics 項目が未完了 — [#134337](https://github.com/dotnet/runtime/issues/134337)
  - テスト: ランタイムテスト [#120708](https://github.com/dotnet/runtime/issues/120708)、再リンク [#123946](https://github.com/dotnet/runtime/issues/123946)、低速でスキップ中の libraries テスト [#129973](https://github.com/dotnet/runtime/issues/129973)、WBT カバレッジ [#135046](https://github.com/dotnet/runtime/issues/135046)（CoreCLR 406 件 vs Mono 参考値 539 件、ただし差がそのまま不足テスト数ではない）
  - ネイティブライブラリ: SkiaSharp 等の P/Invoke でビルド失敗 [#131874](https://github.com/dotnet/runtime/issues/131874)、[#135200](https://github.com/dotnet/runtime/issues/135200)、`NativeFileReference` の同等性 [#134119](https://github.com/dotnet/runtime/issues/134119)
  - ランタイム機能差: `AssemblyLoadContext` のアンロード [#134656](https://github.com/dotnet/runtime/issues/134656)、VFS フォールバック欠如 [#132308](https://github.com/dotnet/runtime/issues/132308)
  - ワークロード: Mono ビルドへの依存 [#132699](https://github.com/dotnet/runtime/issues/132699)、内容の整理 [#134074](https://github.com/dotnet/runtime/issues/134074)、パック解決の修正 [PR #133728](https://github.com/dotnet/runtime/pull/133728)（2026-09-11 作成、open）
  - 性能・サイズ: [#124218](https://github.com/dotnet/runtime/issues/124218)、[#135985](https://github.com/dotnet/runtime/issues/135985)、emsdk 更新 [#136012](https://github.com/dotnet/runtime/issues/136012)
- **件数の目安**: arch-wasm かつ milestone 12.0.0 の open issue は 142 件、うち検索語 CoreCLR に一致するものは 88 件 — [GitHub 検索](https://github.com/dotnet/runtime/issues?q=is%3Aissue+is%3Aopen+label%3Aarch-wasm+milestone%3A12.0.0)

### Inferences
- Mono パックの feed 上の最新が CoreCLR より 5 日古い（Arcade の日付符号を 2026-11-13 / 2026-11-18 と解釈）のは削除作業の進行と整合するが、これだけでは停止の証拠にならない。
- Mono の browser-wasm は .NET 11（STS）で引き続き既定なので、.NET 12 で置換が完了しても .NET 11 向けの Mono は残る。

### Gaps
- .NET 12 での既定切り替えを確約する公式声明は見つからなかった。milestone と epic の記述のみ。
- .NET 12 の GA 日程の一次情報はこの調査では取得していない（releases-index に 12.0 未掲載）。
- Mono の jiterpreter / LLVM AOT の性能水準を置換の合格基準にするかは未確認。epic は性能目標の策定自体を未完了としている。

## 7. ソースビルドの要件と作業量

### Takeaway
インタープリタ版を試すだけならソースビルドは不要（.NET 11 SDK のオプトインで足りる）。R2R を試すには .NET 12 alpha パックかソースビルドが必要。公式の所要時間の記載はない。

### Cited Findings
- **ビルド**: リポジトリルートで `./build.sh -os browser -c Debug -subset clr+libs`（Windows は `.\build.cmd`）。Emscripten SDK は共有キャッシュへ自動導入され、初回は時間がかかる — [wasm.md](https://github.com/dotnet/runtime/blob/main/docs/workflow/building/coreclr/wasm.md)
- **実行**: ブラウザは `src/coreclr/hosts/corerun/CMakeLists.txt` の `CORERUN_IN_BROWSER` を 1 に編集し、`WASM_PRELOAD_DIR` に DLL を用意して再ビルド、`dotnet-serve` で配信。Node は `node --experimental-wasm-exnref ./corerun.js ...` または corehost の `main.mjs` — 同上
- **Emscripten**: `EmsdkVersion` は release/11.0、main とも 6.0.2 — [release/11.0 Versions.props](https://github.com/dotnet/runtime/blob/release/11.0/eng/Versions.props)、[main Versions.props](https://github.com/dotnet/runtime/blob/main/eng/Versions.props)
- **SDK 経由の再リンク条件**: Release の trimmed publish では `WasmBuildNative` が既定で true、ツールチェーン不足時は Emscripten SDK 必須のエラー — [BrowserWasmApp.CoreCLR.targets](https://github.com/dotnet/runtime/blob/release/11.0/src/mono/browser/build/BrowserWasmApp.CoreCLR.targets)
- **ツールチェーンの記述に時期差**: Preview 4 は再リンクが動くと記載 — [Preview 4 runtime notes](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview4/runtime.md)、Preview 5 は専用のネイティブツールチェーン/ワークロードがまだなく、AOT とネイティブビルドには Mono が必要と記載 — [Preview 5 runtime notes](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview5/runtime.md)
- **この環境**: SDK 11.0.100 と `wasm-tools` ワークロードが導入済み（`dotnet --list-sdks` / `dotnet workload list` で確認、URL なし）
- **R2R（main）**: `PublishReadyToRun=true` でオプトイン — [Browser.CoreCLR.targets](https://github.com/dotnet/runtime/blob/main/src/mono/nuget/Microsoft.NET.Sdk.WebAssembly.Pack/build/Microsoft.NET.Sdk.WebAssembly.Browser.CoreCLR.targets)。パックは dotnet12 feed に存在 — [feed index](https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet12/nuget/v3/flat2/microsoft.netcore.app.runtime.browser-wasm/index.json)

### Inferences
- 計測の現実的な順序は、(1) SDK 11.0.100 + `UseMonoRuntime=false` でインタープリタ、(2) dotnet12 feed の alpha パックで R2R / composite、(3) それでも不足する場合のみソースビルド。
- Release の trimmed publish は再リンクを伴うので、`wasm-tools` が CoreCLR 向けの Emscripten を供給できるかが GA での最初の確認点になる（Preview 4 と 5 の記述差を実機で解消する）。

### Gaps
- ビルド時間、ディスク、メモリの公式な目安は `wasm.md` に記載がない。WASI の composite で数 GiB のメモリを要する旨の記述はあるが、ブラウザ向けの値ではない — [wasi-r2r.md](https://github.com/dotnet/runtime/blob/main/docs/workflow/building/coreclr/wasi-r2r.md)
- alpha パックを使うのに .NET 12 SDK が必須か、11.0.100 からパック上書きで足りるかは未確認。
