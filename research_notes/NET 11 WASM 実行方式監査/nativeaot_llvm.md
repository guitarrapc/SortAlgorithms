# NativeAOT-LLVM for WebAssembly（dotnet/runtimelab `feature/NativeAOT-LLVM`）調査ノート

調査日: 2026-11-19。ブランチ先頭 `527a565752`（2026-09-22）のソースと、同日の `dotnet-experimental` feed を確認した。ビルド・実行はしていない。ソース読解による記述は「実機未検証」である。

## 1. プロジェクト状況: 実験的か、サポート対象か。最新の活動、追従する .NET バージョン、.NET 11 / net11.0 対応、将来計画

### Takeaway
2026-11-19 時点でも実験的な fork であり、公式サポート製品ではない。配布物は `10.0.0-rc.1.*` 系のみで 11.x パッケージは feed に存在せず、`net11.0` を対象とする手順・サンプル・利用例も確認できなかった。上流 dotnet/runtime では別系統の NativeAOT WASM が 12.0.0 マイルストーンで追跡され始めている。

### Cited Findings
- **位置付け（2026-11-19 確認）**: ブランチ README は本ブランチを LLVM を使う Native AOT の実験的 fork と説明し、現在サポートするターゲットは WebAssembly のみ、他ターゲットには投資していないとしている — [README.md](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/README.md)
- **ブランチ先頭（2026-09-22）**: 最新コミットは `527a565752`（#3307、IntermediateArtifacts からの追加ファイル publish を抑止）— [commit](https://github.com/dotnet/runtimelab/commit/527a565752062b20bb9693eea6c884b34ecce7bf)、[コミット一覧](https://github.com/dotnet/runtimelab/commits/feature/NativeAOT-LLVM)
- **直近の製品コード変更（2026-07）**: WASI の async Main 対応（#3277、2026-07-07 マージ）、`cpobj` の write barrier 欠落修正（#3274、2026-07-05）— [#3277](https://github.com/dotnet/runtimelab/pull/3277)、[#3274](https://github.com/dotnet/runtimelab/pull/3274)
- **2026 年前半の修正**: GC 参照とパディングを含む struct コピーの修正（#3259、2026-04-23）、`LinkerFlavor=lld` 対応（#3241、2026-03-12）、wasi-sdk 29 化（#3219、2026-01-09）— [#3259](https://github.com/dotnet/runtimelab/pull/3259)、[#3241](https://github.com/dotnet/runtimelab/pull/3241)、[#3219](https://github.com/dotnet/runtimelab/pull/3219)
- **最新 issue（2026-11-02）**: 最新ビルド `10.0.0-rc.1.26357.1` で ILC がクラッシュする報告が open。issue 活動は続いている — [#3329](https://github.com/dotnet/runtimelab/issues/3329)
- **製品バージョン（2026-09-22 先頭）**: `ProductVersion` は `10.0.0`、プレリリースラベルは `rc.1`、`NetCoreAppCurrentVersion` は `10.0` — [eng/Versions.props](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/eng/Versions.props)、[Directory.Build.props](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/Directory.Build.props)
- **リポジトリ自身のビルド SDK（2026-02-04 更新）**: `global.json` は `11.0.100-preview.1.26078.121` を指定 — [global.json](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/global.json)、[#3233](https://github.com/dotnet/runtimelab/pull/3233)
- **上流取り込み（2025-12-04 マージ）**: 最後の大型マージ #3206 は dotnet/runtime の `a67d3886ad`（2025-11-08、main 上のコミット）まで取り込んだ — [#3206](https://github.com/dotnet/runtimelab/pull/3206)、[上流コミット](https://github.com/dotnet/runtime/commit/a67d3886ad148bd3a3d57082c88497803fc6bf7b)
- **feed（2026-11-19 取得）**: `Microsoft.DotNet.ILCompiler.LLVM` の最新は `10.0.0-rc.1.26357.1`。メジャー別件数は 6.x: 16、7.x: 81、8.x: 76、9.x: 60、10.x: 61 で、11.x は 0 件 — [flat index](https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-experimental/nuget/v3/flat2/microsoft.dotnet.ilcompiler.llvm/index.json)
- **最新パッケージの出所**: nuspec の repository commit は `9954350a58`（2026-07-07 のマージコミット）。2026-09-22 の先頭コミットに対応する新しいパッケージは feed に無い — [nuspec](https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-experimental/nuget/v3/flat2/microsoft.dotnet.ilcompiler.llvm/10.0.0-rc.1.26357.1/microsoft.dotnet.ilcompiler.llvm.nuspec)、[commit](https://github.com/dotnet/runtimelab/commit/9954350a58ede8b8eaaeb24112ca4f1e78cc527c)
- **ドキュメント・サンプルの TFM（2026-09-22 先頭）**: `compiling.md` の参照は `Version="10.0.0-*"`、`HelloWorld` / `NativeLibrary` サンプルは `net10.0` — [compiling.md](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/docs/using-nativeaot/compiling.md)、[HelloWorld.csproj](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/samples/HelloWorld/HelloWorld.csproj)、[NativeLibrary.csproj](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/samples/NativeLibrary/NativeLibrary.csproj)
- **最新の不具合報告の TFM（2026-11-02）**: #3329 の再現プロジェクトも `net10.0` と `10.0.0-rc.1.26357.1` を使用 — [#3329](https://github.com/dotnet/runtimelab/issues/3329)
- **.NET 11 の状況**: .NET 11.0.0 / SDK 11.0.100 は 2026-11-10 にリリース済み（STS）— [releases-index.json](https://dotnetcli.blob.core.windows.net/dotnet/release-metadata/releases-index.json)
- **上流の別系統（2026-08-26 起票、2026-10-01 更新、open）**: dotnet/runtime #132798 が NativeAOT の WASM 対応を追跡し、マイルストーンは `12.0.0`。CoreLib・ランタイムのビルド通過、`[JSExport]` 検証などが未完で、LLVM 実験ブランチとの性能・サイズ比較は低優先度の未完項目として載っている — [dotnet/runtime#132798](https://github.com/dotnet/runtime/issues/132798)
- **上流系統は RyuJIT ベース（2026-11-06）**: 単一スレッド WebAssembly 向けの RyuJIT/NativeAOT のコード品質 issue が起票されている — [dotnet/runtime#137061](https://github.com/dotnet/runtime/issues/137061)
- **Microsoft 側メンバーの発言（2025-03-28）**: pavelsavara は、当面の主眼は NativeAOT と相性の悪い Blazor であり、NAOT-LLVM とは協力しつつ注視しているが将来は未定、という趣旨を述べた — [dotnet/runtime#113979](https://github.com/dotnet/runtime/issues/113979)
- **利用プロジェクト**:
  - SpacetimeDB は .NET 10 以降の C# モジュール向けに Windows/Linux で対応し、文書で実験的と警告（文書の最終更新 2026-11-05）— [NATIVEAOT-LLVM.md](https://github.com/clockworklabs/SpacetimeDB/blob/master/crates/bindings-csharp/NATIVEAOT-LLVM.md)
  - Bootsharp は 0.8.0 以降 Release publish で自動的に有効化し、スクリプトは `10.0.0-rc.1.26117.1` を固定 — [Bootsharp guide](https://bootsharp.com/guide/llvm)、[llvm.sh](https://github.com/elringus/bootsharp/blob/main/src/cs/.scripts/llvm.sh)
  - componentize-dotnet は WASI 0.2 コンポーネント生成に NativeAOT-LLVM を使用（2026-06-12 時点で最終 push）— [README](https://github.com/bytecodealliance/componentize-dotnet)

### Inferences
- コードは .NET 11 開発期の main（2025-11-08 時点）を含むが、製品としての版は 10.0.0-rc.1 のままである。「.NET 11 追従版」とは言えず、.NET 11 GA の API・ランタイムとも一致しない。
- 本リポジトリは [Directory.Build.props](D:/github/guitarrapc/SortAlgorithms/Directory.Build.props) で `net11.0` を指定している。NativeAOT-LLVM で試すなら `net10.0` 向けにビルドする経路が現実的で、`net11.0` のままでは 10.0 系ランタイムパックとの不整合が予想される（実機未検証）。
- 2026-07-07 より後のブランチ変更は #3307 のみで、タイトルから見て publish 周りの変更である。放棄の宣言はないが、.NET 11 に合わせた更新は確認できない。
- 上流の #132798 は NativeAOT-LLVM の後継候補に見えるが、置き換えを明言した一次情報は見つかっていない。

### Gaps
- `net11.0` + SDK 11.0.100 + `10.0.0-rc.1.26357.1` の組み合わせが動くかを示す一次情報は見つからなかった。GitHub コード検索でも `net11.0` と `ILCompiler.LLVM` の併用例は 0 件だった（検索インデックスの範囲に限られる）。別タスクの実測で確認が必要。
- NativeAOT-LLVM の公式ロードマップ、11.x パッケージ公開予定、上流系統への移行計画は見つからなかった。

## 2. 利用方法: パッケージ、feed、必要ツールチェーン、RID、Windows での MSBuild 手順

### Takeaway
`dotnet-experimental` feed の 2 パッケージを `10.0.0-*` で参照し、`dotnet publish -r browser-wasm -c Release` で発行する。Windows の browser-wasm では Emscripten 3.1.56 の固定が事実上必須である。

### Cited Findings
- **feed URL**: `https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-experimental/nuget/v3/index.json` を `nuget.org` と併記する — [compiling.md](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/docs/using-nativeaot/compiling.md)
- **参照パッケージ**: `Microsoft.DotNet.ILCompiler.LLVM` と `runtime.$(NETCoreSdkPortableRuntimeIdentifier).Microsoft.DotNet.ILCompiler.LLVM`（Windows x64 なら `runtime.win-x64...`）。両者は同一バージョンにする — [compiling.md](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/docs/using-nativeaot/compiling.md)
- **feed 上の最新（2026-11-19）**: 次の 4 パッケージはいずれも `10.0.0-rc.1.26357.1` — [共通](https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-experimental/nuget/v3/flat2/microsoft.dotnet.ilcompiler.llvm/index.json)、[win-x64](https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-experimental/nuget/v3/flat2/runtime.win-x64.microsoft.dotnet.ilcompiler.llvm/index.json)、[browser-wasm](https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-experimental/nuget/v3/flat2/runtime.browser-wasm.microsoft.dotnet.ilcompiler.llvm/index.json)、[wasi-wasm](https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-experimental/nuget/v3/flat2/runtime.wasi-wasm.microsoft.dotnet.ilcompiler.llvm/index.json)
- **RID**: `browser-wasm` と `wasi-wasm` — [compiling.md](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/docs/using-nativeaot/compiling.md)
- **ビルドホスト**: 文書上は Windows x64 と Linux x64。#3279 は linux-arm64 のホストパックが最近追加された一方、osx 向けは 10.x で公開されていないと述べる — [compiling.md](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/docs/using-nativeaot/compiling.md)、[#3279](https://github.com/dotnet/runtimelab/issues/3279)
- **Emscripten**: CI は emsdk をコミット `ca7b40ae22` に固定して `3.1.56` を install/activate する。文書は同じ版の使用を強く推奨 — [install-emscripten.ps1](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/eng/pipelines/runtimelab/install-emscripten.ps1)、[prerequisites.md](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/docs/using-nativeaot/prerequisites.md)
- **Windows 固有の注意（2026-06-24、open）**: ビルドターゲットは Windows で `emcc.bat` を呼ぶが、新しい Emscripten は `emcc.exe` を提供する。メンテナは現状は古い版のみ対応と回答 — [#3273](https://github.com/dotnet/runtimelab/issues/3273)、[Wasm.targets](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.Wasm.targets)
- **環境変数**: browser は `EMSDK`（または `EmscriptenSdkToolsPath`）、WASI は `WASI_SDK_PATH` が必要 — [Wasm.targets](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.Wasm.targets)
- **wasi-sdk**: 期待バージョンは `29.0`。相違時は警告 — [install-wasi-sdk.ps1](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/eng/pipelines/runtimelab/install-wasi-sdk.ps1)、[Wasm.targets](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.Wasm.targets)
- **文書化された手順**:
  1. Emscripten を導入する（wasi-wasm の場合は wasi-sdk も）
  2. `nuget.config` に上記 2 つの source を追加する
  3. 上記 2 パッケージを `PackageReference` で追加する
  4. `PublishAot` を削除し、`PublishTrimmed=true`、`SelfContained=true`、`MSBuildEnableWorkloadResolver=false` を設定する
  5. `dotnet publish -r browser-wasm -c Release` を実行する

  — [compiling.md](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/docs/using-nativeaot/compiling.md)
- **サンプルの追加設定**: `HelloWorld.csproj` は `UseAppHost=false` も指定 — [HelloWorld.csproj](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/samples/HelloWorld/HelloWorld.csproj)
- **出力**: browser の Exe は `.html`（と `.js`/`.wasm`）、browser の共有ライブラリは `.js`、WASI は `.wasm` — [Native.targets L89-L91](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.targets#L89-L91)
- **文書の古さ（2026-11-19 確認）**: `compiling.md` の最終更新は 2024-12-18 で、出力パス例は `net9.0` のまま。`HelloWorld` README は 2026-06-22 に `net10.0` へ更新済み — [compiling.md 履歴](https://github.com/dotnet/runtimelab/commits/feature/NativeAOT-LLVM/docs/using-nativeaot/compiling.md)、[HelloWorld README](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/samples/HelloWorld/README.md)

### Inferences
- バージョン指定は `10.0.0-*` より `10.0.0-rc.1.26357.1` への固定が再現性の面で望ましい。#3329 の再現も固定指定である。
- Windows で最新の emsdk を入れると #3273 に当たる可能性が高いので、計測環境では 3.1.56 を明示する。

### Gaps
- SDK 11.0.100 環境で `MSBuildEnableWorkloadResolver=false` を含む手順がそのまま通るかは未確認。
- `net10.0` 向けに SDK 11 で publish する構成の公式な可否は記載がない。

## 3. Wasm 機能: SIMD128 / Vector128 / PackedSimd、例外処理、スレッド、tail call、memory64、WASI、Component Model

### Takeaway
**Wasm SIMD は C# コードに対して未対応**で、`-msimd128` も渡されない。例外処理は browser が Wasm EH、WASI は emulated が既定。スレッドは基盤のみで実用段階ではなく、ターゲットは wasm32 のみ、WASI は preview 2 (`wasip2`) が既定である。

### Cited Findings
- **SIMD 未対応の明言（2024-02-11）**: `Vector.IsHardwareAccelerated` が false になる報告に対し、SingleAccretion は "Yes, we don't support SIMD yet." と回答。issue は 2026-11-19 時点も open — [#2515](https://github.com/dotnet/runtimelab/issues/2515)
- **SIMD 追跡 issue（2024-05-24 起票、コメント 0、2026-11-19 時点 open）**: 実現可能性を調査すべきという段階 — [#2593](https://github.com/dotnet/runtimelab/issues/2593)
- **現行ソース（2026-09-22 先頭）**:
  - JIT の `FEATURE_SIMD` / `FEATURE_HW_INTRINSICS` は x64 / arm64 / x86 でのみ定義 — [jit/CMakeLists.txt L76-L80](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/jit/CMakeLists.txt#L76-L80)
  - clang への `CompileWasmArgs` に `-msimd128` は無い — [Native.targets L421-L426](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.targets#L421-L426)
  - `DotNetJsApi` 経路は `WASM_ENABLE_SIMD=0` を設定 — [DotNetJsApi.targets](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/DotNetJsApi.targets)
  - LLVM lowering には SIMD を将来 bitcast で扱う旨のコメントがあるのみ — [llvmlower.cpp L1099](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/jit/llvmlower.cpp#L1099)
- **`PackedSimd`**: CoreLib は `Platform=wasm` かつ `RuntimeFlavor != CoreCLR` のときのみ実装版を含み、それ以外は `IsSupported=false` の PlatformNotSupported 版を使う — [projitems L18](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/libraries/System.Private.CoreLib/src/System.Private.CoreLib.Shared.projitems#L18)、[L2797-L2803](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/libraries/System.Private.CoreLib/src/System.Private.CoreLib.Shared.projitems#L2797-L2803)、[PlatformNotSupported 版](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/libraries/System.Private.CoreLib/src/System/Runtime/Intrinsics/Wasm/PackedSimd.PlatformNotSupported.cs)
- **回避策（2024-02-12、2024-02-17）**: C を `emcc -msimd128` でコンパイルし `NativeLibrary` + `DirectPInvoke` で静的リンクする方法がメンテナから示され、報告者が動作を確認 — [#2515](https://github.com/dotnet/runtimelab/issues/2515)
- **例外処理**: browser は `WasmEnableExceptionHandling=true` が既定で `IlcLlvmExceptionHandlingModel=wasm`、`-fwasm-exceptions` を付与。それ以外は `cpp` / `emulated` にフォールバックし、WASI の既定は `emulated` — [Native.targets L121-L122, L135-L137](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.targets#L121-L137)、[L425](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.targets#L425)
- **既定で有効な他の機能**: `WasmEnableJSBigIntIntegration=true`、`WasmEnableNonTrappingFloatToIntConversions=true`（`-mnontrapping-fptoint`）— [Native.targets L119-L120](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.targets#L119-L120)
- **スレッド（2025-07-22）**: #3060 はマルチスレッド用パッケージのビルド基盤のみを追加し、GC polling などの実機能は含まないと明記。追加文書も作業途中で機能しない段階としている — [#3060](https://github.com/dotnet/runtimelab/pull/3060)
- **スレッドの未完了項目**: GC poll 挿入などが open — [#2806](https://github.com/dotnet/runtimelab/issues/2806)、[#1828](https://github.com/dotnet/runtimelab/issues/1828)
- **tail call**: `FEATURE_FASTTAILCALL` と `FEATURE_TAILCALL_OPT` は 0。shadow stack 上の tail call は別途実装済み。コンパイル引数に `-mtail-call` は無い — [targetllvm.h L18-L19](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/jit/targetllvm.h#L18-L19)、[#2501](https://github.com/dotnet/runtimelab/pull/2501)、[Native.targets L421-L426](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.targets#L421-L426)
- **memory64**: ILC と JIT には `Wasm64` の分岐があるが、ビルド統合が設定するターゲットは `wasm32-unknown-emscripten` と `wasm32-unknown-wasip2` のみ — [Program.cs L363-L364](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/tools/aot/ILCompiler/Program.cs#L363-L364)、[Native.targets L130-L131](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.targets#L130-L131)
- **メモリ上限**: WASI は `--max-memory=2147483648`、browser は `ALLOW_MEMORY_GROWTH=1` で、`EmccMaximumHeapSize` / `EmccInitialHeapSize` を指定可能 — [LinkNativeLlvm](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.targets#L521)
- **WASI / Component Model**: 既定ターゲットは preview 2。`WasmComponentTypeWit` の WIT を `--component-type` でリンカに渡す。`LinkerFlavor=lld` のときはこの引数を渡さない — [Native.targets L131](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.targets#L131)、[LinkNativeLlvm](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.targets#L521)、[compiling.md](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/docs/using-nativeaot/compiling.md)
- **WASI preview 1**: `wasi_snapshot_preview1` の関数を `WasmImportLinkage` で import する例が文書にある — [compiling.md](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/docs/using-nativeaot/compiling.md)

### Inferences
- `Vector128<T>` / `Vector<T>` の API は、#2515 の実行例から見て動作はするがソフトウェアフォールバックになると考えられる。`Vector128.IsHardwareAccelerated` は false の見込み（実機未検証）。
- `CompileWasmArgs` はプロパティとして無条件に代入されるので、ユーザーが `-msimd128` を足す公式の拡張点は見当たらない。仮に足せても JIT が SIMD IR を生成しないため、効果は LLVM の自動ベクトル化に限られるはずである。
- 本リポジトリの `src/SortAlgorithm` で Intrinsics を参照するのは `Driftsort.cs` と `Glidesort.cs` の `using`（`X86` 含む）のみで、`Vector128` / `Vector256` の実使用は検出されなかった。比較・交換主体のスカラー処理なので、SIMD 非対応の直接影響は限定的と見られる。ただし BCL 内部の `Span` ヘルパーの性能には影響しうる。
- 単一スレッドのソートにはスレッド未対応は問題にならない。

### Gaps
- NativeAOT-LLVM の CoreLib ビルド時の `RuntimeFlavor` 値は確定できなかった。`PackedSimd.IsSupported` の実行時値は実測が必要。
- `-fwasm-exceptions` が Emscripten 3.1.56 で生成する EH 形式（legacy か exnref か）は一次情報を確認していない。
- `Span.CopyTo` / `Fill` などが Wasm bulk memory 命令に落ちるかは未確認。関連して bulk memory の有効化状況を問う [#2906](https://github.com/dotnet/runtimelab/issues/2906) が open。
- マルチスレッド版パッケージの正式名と feed 公開の有無は確認できなかった（推定名は 404）。

## 4. JS 相互運用: エクスポート、WasmImport、Emscripten 関数、JSExport/JSImport、配列・Span の受け渡し

### Takeaway
最も単純で文書化された経路は `[UnmanagedCallersOnly(EntryPoint=...)]` と線形メモリ上のポインタ渡しである。`DotNetJsApi=true` で `[JSExport]` / `[JSImport]` も使えるが、`compiling.md` には記載がなく、未解決 issue が残る。

### Cited Findings
- **エクスポート**: `OutputType=library` と `[UnmanagedCallersOnly(EntryPoint = "...")]` を使い、`LinkerArg` で `-sEXPORTED_RUNTIME_METHODS=cwrap` や `--post-js` を渡す — [compiling.md](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/docs/using-nativeaot/compiling.md)
- **参照アセンブリのエクスポート**: publish 対象以外のアセンブリのメソッドを公開するには `UnmanagedEntryPointsAssembly` で明示する — [compiling.md](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/docs/using-nativeaot/compiling.md)
- **エクスポートの制約**: static のみ、プリミティブまたは値型のみ、managed から直接呼べない、例外を外へ漏らせない — [NativeLibrary README](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/samples/NativeLibrary/README.md)、[NativeLibrary.cs](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/samples/NativeLibrary/NativeLibrary.cs)
- **呼び出し形式**: browser 向けでは Emscripten 生成 JS が wasm を読み込み、サンプルは先頭に `_` が付いたグローバル関数を直接呼ぶ。WASI では単体モジュールの export を呼ぶ — [NativeLibrary README](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/samples/NativeLibrary/README.md)
- **インポート**: 実行時に与える import は `[WasmImportLinkage]` + `DllImport`、静的リンクするネイティブコード（Emscripten の JS 連携 API を含む）は `DirectPInvoke`。どちらも無い P/Invoke は既定で `PlatformNotSupportedException` — [wasm-interop.md](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/docs/using-nativeaot/wasm-interop.md)、[compiling.md](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/docs/using-nativeaot/compiling.md)
- **JSExport / JSImport**: `DotNetJsApi=true` で `DotNetJsApi.targets` が読み込まれ、`dotnet.native.js` を生成し、JSImport generator を置き換える — [Native.targets L68](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.targets#L68)、[DotNetJsApi.targets](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/DotNetJsApi.targets)
- **JSExport / JSImport の検証状況**: スモークテストが両属性を使用。追跡 issue では主要項目が完了済みだが issue 自体は open — [DotnetJs/Program.cs](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/tests/nativeaot/SmokeTests/DotnetJs/Program.cs)、[#2434](https://github.com/dotnet/runtimelab/issues/2434)
- **既知の問題（open）**: クラスライブラリ内の JSExport の失敗、Mono 版 WASM SDK との差異 — [#2626](https://github.com/dotnet/runtimelab/issues/2626)、[#2627](https://github.com/dotnet/runtimelab/issues/2627)
- **ポインタ渡しの実例**: 公式サンプルは `byte*` を受け取り `ReadOnlySpan<byte>` を作る。#2515 の報告者は `NativeMemory.AlignedAlloc` を返す `Alloc` と、ポインタ・長さを受ける処理関数を export している — [NativeLibrary.cs](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/samples/NativeLibrary/NativeLibrary.cs)、[#2515](https://github.com/dotnet/runtimelab/issues/2515)
- **HEAP ビュー**: Emscripten は 4.0.7 で `HEAP8` / `HEAP32` などの既定エクスポートを廃止し、3.1.55 では STRICT モード時のみオンデマンド化した — [Emscripten ChangeLog](https://raw.githubusercontent.com/emscripten-core/emscripten/main/ChangeLog.md)
- **メモリ拡張**: `WebAssembly.Memory.grow` が成功すると以前の `ArrayBuffer` は detach され、既存の typed array は空になる — [MDN](https://developer.mozilla.org/en-US/docs/WebAssembly/Reference/JavaScript_interface/Memory/grow)
- **リンク設定との関係**: browser ビルドは `ALLOW_MEMORY_GROWTH=1` を常に渡す — [LinkNativeLlvm](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.targets#L521)
- **境界越えコスト（2025-09-06、open）**: Reverse P/Invoke では shadow stack 初期化や未処理例外処理のためのランタイムヘルパー呼び出しが入口・出口に発生する。改善案は issue に挙がっているが open のまま — [#3163](https://github.com/dotnet/runtimelab/issues/3163)

### Inferences
- ソートライブラリの推奨形は、`Alloc` / `Free` / `Sort(int* ptr, int length)` のような具象型の export を用意し、JS から線形メモリへ書き込んで 1 回だけ呼ぶ形である。export 内で `new Span<int>(ptr, length)` を作れば、generic なソート本体は managed 側で完結する。
- `[UnmanagedCallersOnly]` は C# の規則上ジェネリックにできないので（一般知識、本調査では未検証）、要素型・比較器の組み合わせごとに具象 export が必要になる。
- `SortAlgorithm` を参照プロジェクトにして export を publish プロジェクト側に置けば `UnmanagedEntryPointsAssembly` は不要である。
- JS 側は呼び出しのたびに `HEAP*` を取り直すか、export 実行中にメモリ拡張が起きない前提を確認する必要がある。

### Gaps
- `[JSExport]` の `Span<T>` / `ArraySegment<T>`（`JSType.MemoryView`）が NativeAOT-LLVM で動くかは確認できなかった。スモークテストの grep では該当使用が見つからない。
- Emscripten 3.1.56 の非 STRICT ビルドで `Module.HEAP32` が確実に公開されるかは実機確認が必要。

## 5. 速度優先の構成: OptimizationPreference、命令セット、LTO、Emscripten -O、トリミング、サイズ

### Takeaway
実際に効くプロパティは `IlcOptimizationPreference` ではなく **`OptimizationPreference=Speed`** である。LLVM 段は browser `-O3` / WASI `-O2` が既定で、リンク段は `-O1`（`wasm-opt` なし）である。`-flto` は無く、`IlcInstructionSet` に Wasm 用の値は見当たらない。

### Cited Findings
- **ILC への反映**: `OptimizationPreference=Speed` で `--Ot`、`Size` で `--Os` — [Native.targets L314-L315](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.targets#L314-L315)
- **文書との不一致**: `optimizing.md` は `IlcOptimizationPreference` を記載するが、`Native.targets` / `Native.Publish.targets` / `SingleEntry.targets` に該当参照は無い — [optimizing.md](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/docs/using-nativeaot/optimizing.md)、[Native.targets](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.targets)
- **LLVM 最適化レベル**: `Optimize=true` かつ Size 以外で browser は `-O3`、WASI は `-O2`。Size は `-Oz`。ビルド定義はこれらを内部・非サポート扱いとコメントしている — [Native.targets L124-L128](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.targets#L124-L128)
- **固定のコード品質回避策**: 常に `-mllvm -combiner-global-alias-analysis=false -mllvm -disable-lsr` を渡す — [Native.targets L421](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.targets#L421)
- **PGO データ**: `OptimizationPreference=Speed` で `IlcPgoOptimize=true` が既定となり、`.mibc` を読み込む — [L33](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.targets#L33)、[L206](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.targets#L206)
- **リンク最適化**: browser は `Optimize=true` で `-O1` を渡し、その後に `EmccFlags` を追加する。コメントは `wasm-opt` を走らせない理由をスタックトレース情報の破壊としている — [LinkNativeLlvm](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.targets#L521)、[#2357](https://github.com/dotnet/runtimelab/issues/2357)
- **利用者の実例**: Bootsharp は `EmccFlags` に `-O3` を追加し、さらに `wasm-opt -O3 --all-features --strip-dwarf --strip-debug --vacuum` を実行する — [Bootsharp.props](https://github.com/elringus/bootsharp/blob/main/src/cs/Bootsharp/Build/Bootsharp.props)、[Bootsharp.targets](https://github.com/elringus/bootsharp/blob/main/src/cs/Bootsharp/Build/Bootsharp.targets)
- **LTO / モジュール分割**: ビルド定義に `-flto` は無い。ILC は LLVM bitcode を既定で最大 16 モジュールに分割し、各 `.bc` を clang が個別にコンパイルする。`IlcMaxLlvmModuleCount` で変更可能 — [LLVMCodegenCompilationBuilder.cs L68-L93](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/tools/aot/ILCompiler.LLVM/Compiler/LLVMCodegenCompilationBuilder.cs#L68-L93)、[Native.targets L346](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.targets#L346)
- **`IlcInstructionSet`**: `--instruction-set` として ILC に渡るが、`InstructionSetHelpers.cs` と `InstructionSetDesc.txt` に wasm の記述は無い — [Native.targets L317](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.targets#L317)、[InstructionSetHelpers.cs](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/tools/Common/InstructionSetHelpers.cs)、[InstructionSetDesc.txt](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/tools/Common/JitInterface/ThunkGenerator/InstructionSetDesc.txt)
- **サイズ関連の主要設定**: 既定ではデバッグ情報が入るため `DebugType=none`、ICU を外す `InvariantGlobalization=true` — [compiling.md](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/docs/using-nativeaot/compiling.md)
- **その他の ILC 設定**: `IlcGenerateStackTraceData=false`、`IlcTrimMetadata=true`、`IlcDisableReflection=true`、`IlcFoldIdenticalMethodBodies=true` — [optimizing.md](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/docs/using-nativeaot/optimizing.md)
- **Bootsharp の Release 構成**: `DebugType=none`、`StackTraceSupport=false`、`PublishTrimmed=true`、`TrimMode=full`、`InvariantGlobalization`、`InvariantTimezone` など — [Bootsharp.props](https://github.com/elringus/bootsharp/blob/main/src/cs/Bootsharp/Build/Bootsharp.props)
- **公開サイズ値**:

| 時点 | 条件 | サイズ | 出典 |
|---|---|---:|---|
| 2024-11-19 | HelloWorld、browser-wasm、`DebugType=none` + `InvariantGlobalization=true` | 約 900 KB | [#2789](https://github.com/dotnet/runtimelab/issues/2789) |
| 2025-03-28 | 利用者アプリ（NativeAOT-LLVM） | 1.7 MB（非 AOT Mono は 2.1 MB） | [dotnet/runtime#113979](https://github.com/dotnet/runtime/issues/113979) |
| 2023-12-29 | WASI、.NET 8、AOT Release | 6.7 MB | [Kozachuk blog](https://byandriykozachuk.wordpress.com/2023/12/29/net-wasi-native-aot-performance/) |
| 同上 | デバッグシンボルなし | 3.4 MB | 同上 |
| 同上 | さらに機能無効化 | 1.4 MB | 同上 |

- **サイズ最適化の効果**:
  - `wasm-opt` は HelloWasm で 8% 超の削減（2023-07-24）— [#2357](https://github.com/dotnet/runtimelab/issues/2357)
  - null check を全廃した実験では 675,104 → 620,384 bytes、-8.11%（2025-07-06）— [#3128](https://github.com/dotnet/runtimelab/issues/3128)

### Inferences
速度優先の候補構成は次のとおりである（実機未検証）。

- `Configuration=Release`、`OptimizationPreference=Speed`
- `DebugType=none`、`InvariantGlobalization=true`、`InvariantTimezone=true`
- `StackTraceSupport=false`、`IlcGenerateStackTraceData=false`
- `EmccFlags=-O3`（リンク時最適化の比較用）
- 比較軸として `IlcMaxLlvmModuleCount=1` と既定 16

補足:

- generic struct の実体化と RyuJIT のインライン展開は ILC 側で行われる。モジュール分割は LLVM 段のモジュール間インライン機会を狭める可能性があるので、`IlcMaxLlvmModuleCount=1` は計測価値がある。ただし文書化された設定ではない。
- `-disable-lsr` が常時指定されるので、ポインタ・インデックス中心のループでは LLVM の loop strength reduction を期待できない。
- `EmccFlags=-O3` で `wasm-opt` が走る場合、#2357 のスタックトレース問題に当たりうるので、スタックトレース無効化と組み合わせるのが安全である。

### Gaps
- `OptimizationPreference=Speed` と既定（blended）の差を WASM で示す公開データは見つからなかった。
- 速度優先構成のサイズを、ソートのような小規模ライブラリで示す公開値は無い。
- `IlcInstructionSet` に値を渡したときの Wasm での挙動（無視かエラーか）は未確認。

## 6. 公開された性能値（バージョン・日付つき）

### Takeaway
公開値は少なく、多くは利用者によるマイクロベンチである。再帰計算では Rust 比 1.0〜1.2 倍、Mono AOT より速いという報告がある一方、SIMD 依存処理では遅い。**ソートを NativeAOT-LLVM で測った公開値は見つからなかった。**

### Cited Findings
- **Bootsharp ベンチ（Rust = 1.0、小さいほど速い。readme の最終更新 2026-05-20、ホスト非記載）**。`Fibonacci` は重い再帰、`Echo` は相互運用の計測 — [samples/bench/readme.md](https://github.com/elringus/bootsharp/blob/main/samples/bench/readme.md)

| 構成 | ベンチ | .NET LLVM | .NET AOT |
|---|---|---:|---:|
| Bootsharp 0.3、.NET 9、Node.js 22 | Fibonacci | 1.0 | 1.6 |
| Bootsharp 0.7、.NET 10、Node.js 24 | Fibonacci | 1.2 | 2.1 |
| Bootsharp 0.8、.NET 10、Node.js 24 | Fibonacci | 1.2 | 2.1 |
| Bootsharp 0.8、.NET 10、Node.js 24 | Echo Number | 1.6 | 28.2 |
| Bootsharp 0.8、.NET 10、Node.js 24 | Echo Struct | 2.2 | 7.7 |

- **同ベンチの 2025-03-29 時点値**: Fibonacci は .NET LLVM 1.1、.NET AOT 1.5。Echo Number は 14.4 / 21.1、Echo Struct は 1.5 / 4.3 — [dotnet/runtime#113979](https://github.com/dotnet/runtime/issues/113979)
- **Vector ループ（2024-02、`9.0.0-*`、browser 系、報告者環境）**: `Vector<byte>` の反転処理はネイティブ 120 ms、Blazor AOT 2400 ms、NativeAOT-LLVM 1600 ms。C + Wasm SIMD への置換で約 330 ms — [#2515](https://github.com/dotnet/runtimelab/issues/2515)
- **WASI の CPU ベンチ（2023-12-29、.NET 8、Wasmtime、LeetCode 問題を 1000 回）**: — [Kozachuk blog](https://byandriykozachuk.wordpress.com/2023/12/29/net-wasi-native-aot-performance/)

| 実行形態 | 時間 |
|---|---:|
| Wasmtime、NativeAOT-LLVM Release | 2245 ms |
| Wasmtime、Mono 系 Release | 13679 ms |
| Wasmtime、Rust Release | 215 ms |
| Windows、.NET 8 Release | 958 ms |
| Windows、Rust Release | 246 ms |

- **同ブログの補足**: `IlcOptimizationPreference` の Speed / Size は差が出なかった。最小構成 1.4 MB では 2162 ms — [Kozachuk blog](https://byandriykozachuk.wordpress.com/2023/12/29/net-wasi-native-aot-performance/)
- **SpacetimeDB（2024-04-02）**: 開発者が `fib 25` の手動計測で約 50% 高速と報告。翌日、CLI の release ビルドでは差がさらに大きいと追記 — [SpacetimeDB#713](https://github.com/clockworklabs/SpacetimeDB/pull/713)
- **公式比較の不在（2026-10-01 時点）**: 上流 #132798 で LLVM ブランチとの比較は低優先度の未完項目 — [dotnet/runtime#132798](https://github.com/dotnet/runtime/issues/132798)
- **関連するが NativeAOT-LLVM ではない値**: NetWasm の性能文書は Mono AOT と desktop .NET 11 NativeAOT を比較するが、NativeAOT-LLVM は計測対象外 — [NetWasm performance.md](https://github.com/zion-sati/NetWasm/blob/main/docs/performance.md)

### Inferences
- `.NET AOT` 列は文脈上 Mono AOT と解釈できるが、readme は明記していない。
- Fibonacci は再帰と呼び出し ABI に偏るので、配列アクセス・比較・swap が支配的なソートの性能を直接は予測しない。
- 2023 年の Rust 比約 10 倍と 2025〜2026 年の Rust 比 1.0〜1.2 倍は、負荷もランタイムも版も異なるので、単純比較できない。

### Gaps
- ソート、`Span<T>`、struct generic comparer を対象にした NativeAOT-LLVM の公開ベンチは見つからなかった。別タスクの実測が一次データになる。
- NativeAOT-LLVM `10.0.0-rc.1.26357.1` と .NET 11 の CoreCLR / Mono WASM を同条件で比べた公開データは見つからなかった。

## 7. 既知の制約: リフレクション、generic virtual method、GC、スタック、未対応 API、デバッグ

### Takeaway
通常の Native AOT の制約に加え、shadow stack と保守的 GC、既定 1 MB のスタック、struct / Span 周辺のコンパイラ不具合が実務上の注意点である。特に再帰の深いソートと Span のコレクション式は事前検証が必要である。

### Cited Findings
- **基本制約**: 動的ロード不可、`System.Reflection.Emit` 不可、C++/CLI・組み込み COM・WinRT 不可、無制限のリフレクション不可 — [limitations.md](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/docs/using-nativeaot/limitations.md)
- **リフレクション**: `IlcTrimMetadata` や `IlcDisableReflection` で範囲をさらに狭められる — [optimizing.md](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/docs/using-nativeaot/optimizing.md)
- **generic virtual method**: `CORINFO_HELP_GVMLOOKUP_FOR_SLOT` は追跡 issue で実装済み。Wasm 固有の性能情報は見つからない — [#1828](https://github.com/dotnet/runtimelab/issues/1828)
- **GC（2025-07-23、open）**: 保守的 GC を使用する理由は、GC 情報を安価に符号化できないことと、参照の再読み込みを省けてコードが小さくなることと説明されている — [#3140](https://github.com/dotnet/runtimelab/issues/3140)
- **managed ABI（2023-02-25 起票、open）**: GC 参照を含まない型は LLVM 引数で渡し、GC プリミティブや GC フィールドを持つ struct は shadow stack で渡す — [#2214](https://github.com/dotnet/runtimelab/issues/2214)
- **null check のコスト（2025-07、open）**: 明示 null check がコードサイズに影響しており、long form だけで 1.7% 超との測定がある — [#3127](https://github.com/dotnet/runtimelab/issues/3127)、[#3128](https://github.com/dotnet/runtimelab/issues/3128)
- **スタックサイズ**: `IlcWasmStackSize` は既定 1,048,576 bytes（1 MB）で、`EmccStackSize` で上書きする — [Native.targets L132-L133](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.targets#L132-L133)
- **ヒープ設定の不具合（2025-08-20、open）**: `EmccMaximumHeapSize` が 16 MB だと `unreachable`、32 MB 超程度なら動作するという報告 — [#3150](https://github.com/dotnet/runtimelab/issues/3150)
- **コンパイラ / コード生成の不具合**:
  - `ImmutableArray<T>` を伴う特定形で ILC がクラッシュ（2026-11-02、最新ビルド、open）— [#3329](https://github.com/dotnet/runtimelab/issues/3329)
  - `[..spanA, ..spanB]` が `InvalidProgramException`（2026-03-04、`10.0.0-alpha.1.25075.1`、wasi-wasm、open）— [#3246](https://github.com/dotnet/runtimelab/issues/3246)
  - WASI コンポーネント export の反復呼び出しで thread static が破損（2026-03-04 起票、2026-03-31 更新、open）— [#3247](https://github.com/dotnet/runtimelab/issues/3247)
- **修正済みの struct 関連不具合**: GC 参照とパディングを含むコピー（2026-04-23）、`cpobj` の write barrier（2026-07-05）— [#3259](https://github.com/dotnet/runtimelab/pull/3259)、[#3274](https://github.com/dotnet/runtimelab/pull/3274)
- **例外とスタックトレース**: JS へ漏れた例外の情報欠落、managed スタックトレース実装、EH の残作業が open — [#2710](https://github.com/dotnet/runtimelab/issues/2710)、[#2404](https://github.com/dotnet/runtimelab/issues/2404)、[#2169](https://github.com/dotnet/runtimelab/issues/2169)
- **デバッグ**: `DebugType` が none 以外なら `NativeDebugSymbols=true` となり、clang / リンカへ `-g3` を渡す。`debugging.md` に Wasm の記述は無い。外部 DWARF 用 PR は open — [Native.targets L421-L426](https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.targets#L421-L426)、[debugging.md](https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/docs/using-nativeaot/debugging.md)、[#2786](https://github.com/dotnet/runtimelab/pull/2786)
- **Blazor**: コントリビューターの yowl は、自身の知る限り Blazor アプリの起動に取り組む人はいないと回答（2024-12-09）— [#2866](https://github.com/dotnet/runtimelab/issues/2866)
- **未解決の `MemoryMarshal` 型ロード報告（2024-10-28、open）**: browser-wasm で `System.TypeLoadException` — [#2731](https://github.com/dotnet/runtimelab/issues/2731)

### Inferences
- `Span<T>` は byref を含むので、#2214 の ABI が現在も同じなら、インライン展開されない呼び出しでは shadow stack 経由になる可能性がある。再帰や非インライン helper が多いソートでは、この差が CoreCLR ネイティブとの性能差に現れやすい。
- GC フィールドを持たない struct comparer / context は LLVM 引数として渡され、インライン展開とも相性が良いと見込まれる。一方、参照を持つ context（統計・可視化用など）は別の性能特性になりうる。
- 本リポジトリは [AGENTS.md](D:/github/guitarrapc/SortAlgorithms/AGENTS.md) の方針で quicksort の敵対的入力を残すので、再帰深度が大きい入力は 1 MB スタックで失敗する可能性がある。実測では `EmccStackSize` を記録し、スタック不足と性能差を分けて扱う必要がある。
- `#3246` は `Span` のコレクション式 spread を直撃するので、ライブラリと計測ハーネスで該当構文の有無を確認すべきである。

### Gaps
- shadow stack の既定サイズと拡張挙動は、確認したランタイムファイルからは特定できなかった。
- #2214 の ABI 説明（2023）が現行コードと完全に一致するかは未検証。
- #3246 が `10.0.0-rc.1.26357.1` でも再現するかは未確認。
- GC の世代構成、allocation fast path、`ArrayPool<T>` の性能に関する Wasm 固有の公開データは見つからなかった。
