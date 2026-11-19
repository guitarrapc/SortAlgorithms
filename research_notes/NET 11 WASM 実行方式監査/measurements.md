# .NET 11 WASM 実行方式監査: SortAlgorithms の実測 (ビルド可否・速度・サイズ・起動・JS 境界・Wasm 機能)

## Q1. このマシンで各 .NET WebAssembly ツールチェーンはビルド・実行できたか

### Takeaway
Wasm の 4 系統 (Mono、CoreCLR、NativeAOT-LLVM、NetWasm) はすべて Node 24 と Edge headless で動き、全セルの出力検証に合格した。ただし CoreCLR Wasm は既定のトリム付き relink が失敗し、prebuilt ランタイムのみで動作した。NativeAOT-LLVM は net10.0 ベースで `OptimizationPreference=Speed` がコンパイラ例外になるため、これを外した構成の値である。

### Cited Findings

**出典の読み方**: この文書の一次資料はすべて自分で実行した結果であり、Web 出典はない。出典リンクは作業領域 `C:\Users\guitarrapc\AppData\Local\Temp\claude\8ffd9513d941f0a1\4ac9fb56-b669-4e11-a61a-d1ccca49da70\scratchpad\wasm-audit` 内のファイルを指す。測定対象のライブラリは `D:\github\guitarrapc\SortAlgorithms` の HEAD `f674c43`。

| ID | 系統 | 実行形態 | 速度優先の設定 (全文は Q8) | 結果 |
| --- | --- | --- | --- | --- |
| `native-jit` | CoreCLR JIT win-x64, net11.0 | 基準。`DOTNET_TieredCompilation=0`, `DOTNET_TieredPGO=0` | Release | 成功 |
| `native-jit-tiered` | 同上 | 既定の階層化 + PGO (コールド計測用) | Release | 成功 |
| `native-aot` | NativeAOT win-x64, net11.0 | 自己完結 exe | `OptimizationPreference=Speed`, `IlcInstructionSet=native` | 成功 |
| `mono-interp` | Mono browser-wasm, net11.0 | interpreter + jiterpreter | `WasmBuildNative=true`, compile/link `-O3`, SIMD, Wasm EH, full trim | 成功 |
| `mono-aot` | Mono browser-wasm, net11.0 | full AOT (`AOTMode=LLVMOnlyInterp` は SDK 既定) | 上記 + `RunAOTCompilation=true`, bitcode `-O3`, `WasmStripILAfterAOT=true` | 成功 |
| `coreclr-interp` | CoreCLR browser-wasm, net11.0 | CoreCLR interpreter, prebuilt `dotnet.native.wasm` | `UseMonoRuntime=false`, `WasmBuildNative=false`, full trim | 成功 (relink なし) |
| `naot-llvm` | NativeAOT-LLVM `10.0.0-rc.1.26357.1`, **net10.0**, browser-wasm | Emscripten 3.1.56、直接 export | 文書どおりの既定 (clang `-O3`, link `-O1`) | 成功 |
| `naot-llvm-speed` | 同上 | 同上 | 上記 + 非公式に `-msimd128` と link `-O3` (wasm-opt) | 成功 |
| `netwasm-o3` | NetWasm 0.14.2, `netwasm0.1` | `JsModule`, hosts `node;page` | `NetWasmOptimization=O3`, `NetWasmGarbageCollector=NetWasm`, hints `Auto` | 成功 |
| `netwasm-o3-ignore` | 同上 | 同上 | 上記 + `NetWasmManagedInliningHints=IgnoreAggressive` (Sortvivo2 v2 の設定) | 成功 |

- 派生ビルドも測定した: `mono-interp-default`, `mono-aot-default` (SDK 既定フラグ)、`mono-interp-nojiterp` (jiterpreter 無効)、`naot-llvm-1mod` (`IlcMaxLlvmModuleCount=1`)、`naot-llvm-size`、`naot-llvm-jsapi` (`DotNetJsApi=true`)、`netwasm-o3-converge`、`netwasm-o3-compact`、`netwasm-o3-bump`、`netwasm-oz`、`netwasm-0.13-o3`。— [results/build-times.json](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/build-times.json), [results/build-times-clean.json](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/build-times-clean.json)
- 全ビルドの実行ログは `C:\Users\guitarrapc\AppData\Local\Temp\claude\8ffd9513d941f0a1\4ac9fb56-b669-4e11-a61a-d1ccca49da70\scratchpad\wasm-audit\logs\build-<ID>.log`、MSBuild binlog は同じ場所の `.binlog`。

**失敗とその記録 (原文)**

- **CoreCLR Wasm: トリム付き Release の既定 relink が失敗**。`PublishTrimmed=true` の Release では SDK が `WasmBuildNative` を true にし、`wasm-ld` が次の未定義シンボルで止まった (同一シンボルで 19 行)。`undefined symbol: SystemJS_ExecuteTimerCallback. Required by SystemJS_MarkAsyncMain`。`-p:WasmBuildNative=false` で prebuilt ランタイムを使うと成功し、`[JSExport]` はこの relink なし構成で動いた。— [logs/build-coreclr-relink-trimmed-FAILED.log](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/logs/build-coreclr-relink-trimmed-FAILED.log)
- **CoreCLR Wasm: トリムなしの relink は成功**。`-p:PublishTrimmed=false -p:WasmBuildNative=true` は workload の Emscripten で完了した (出力 `dotnet.native.wasm` 2,990,057 B、`System.Private.CoreLib` 5,631,773 B)。この構成はビルド確認のみで速度は未測定。
- **CoreCLR Wasm: `PublishReadyToRun=true` は不可**。`NETSDK1095: Optimizing assemblies for performance is not supported for the selected target platform or architecture.` — [logs/build-coreclr-r2r.log](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/logs/build-coreclr-r2r.log)
- **CoreCLR ランタイムパックの入手元**: `Microsoft.NETCore.App.Runtime.browser-wasm` 11.0.0 は workload 由来で `C:\Program Files\dotnet\packs` に既に存在し (`libcoreclr_static.a` を含む)、NuGet からの取得は不要だった。Crossgen2 パッケージ `microsoft.netcore.app.crossgen2.win-x64` 11.0.0 だけが R2R 試行で nuget.org から取得された。— [results/env.txt](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/env.txt)
- **NativeAOT-LLVM: `net11.0` は不可**。`Overriding System.Private.CoreLib.dll with a newer version is not supported. Attempted to use ...\Microsoft.NETCore.App.Runtime.browser-wasm\11.0.0\...\System.Private.CoreLib.dll instead of ...\runtime.browser-wasm.microsoft.dotnet.ilcompiler.llvm\10.0.0-rc.1.26357.1\...`。このためライブラリのソースを `net10.0` プロジェクトへ `Compile Include` した。ソース側の非互換は発生しなかった。— [logs/build-naot-llvm-net11b.log](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/logs/build-naot-llvm-net11b.log)
- **NativeAOT-LLVM: `OptimizationPreference=Speed` (ILC `--Ot`) はコンパイラ例外**。全メソッドで `System.NullReferenceException` at `Internal.JitInterface.CorInfoImpl.getPgoInstrumentationResults`。`--codegenopt:JitDisablePGO=1` を足しても、1 つ古い `10.0.0-rc.1.26322.1` でも同じだった。— [logs/build-naot-llvm-speedpref.log](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/logs/build-naot-llvm-speedpref.log), [logs/build-naot-llvm-Ot-JitDisablePGO-FAILED.log](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/logs/build-naot-llvm-Ot-JitDisablePGO-FAILED.log), [logs/build-naot-llvm-Ot-older.log](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/logs/build-naot-llvm-Ot-older.log)
- **NativeAOT-LLVM: Emscripten は workload を再利用できた**。`Microsoft.NET.Runtime.Emscripten.3.1.56.*` (net10 workload、`emcc.bat` あり) を `EmscriptenSdkToolsPath` に指定し、キャッシュは作業領域にコピーした。Emscripten SDK や wasi-sdk のダウンロードは行っていない。— [scripts/emenv.sh](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/scripts/emenv.sh)
- **Mono/CoreCLR の relink は `Microsoft.NET.Runtime.Emscripten.6.0.2.*` を使用**。パック名は 6.0.2 だが `emscripten-version.txt` は `6.0.3`。— [results/env.txt](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/env.txt)
- **NetWasm 0.13.0**: `System.Linq` が解決されず `ArrayPatterns.cs` で `CS0103: The name 'Enumerable' does not exist`。このファイル (ソートからは未使用) だけ除外してビルドした。0.14.2 では問題なし。— [tc/netwasm-0.13.0/netwasm.csproj](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/tc/netwasm-0.13.0/netwasm.csproj)
- **トリム/AOT 警告**: `native-aot`, `mono-interp`, `mono-aot`, `coreclr-interp`, `naot-llvm`, `naot-llvm-speed`, `netwasm-0.14.2-o3`, `netwasm-0.13.0-o3` のログに `: warning` 行は 0 件。

**取得したパッケージ** (ダウンロードはすべて nuget.org または dotnet-experimental フィードから `pkgs\` へ。バイナリの別途ダウンロードはなし)

| 取得元 | パッケージ | 版 |
| --- | --- | --- |
| dotnet-experimental (`pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-experimental/nuget/v3/index.json`) | `Microsoft.DotNet.ILCompiler.LLVM`, `runtime.win-x64.Microsoft.DotNet.ILCompiler.LLVM`, `runtime.browser-wasm.Microsoft.DotNet.ILCompiler.LLVM` | `10.0.0-rc.1.26357.1` (ほかに切り分け用 `10.0.0-rc.1.26322.1`) |
| nuget.org | `NetWasm.Sdk`, `NetWasm.Toolchain`, `NetWasm.Runtime.Wasm32`, `NetWasm.Runtime.Pack`, `NetWasm.Compiler.Tasks`, `NetWasm.Hosting`, `NetWasm.Hosting.Build`, `NetWasm.HostTools.win-x64`, `NetWasm.Ref` (0.14.2 と 0.13.0)、`NetWasm.App.Ref`, `NetWasm.App.Runtime` (0.14.2) | 0.14.2 / 0.13.0 |
| nuget.org | `Microsoft.NET.ILLink.Tasks` (11.0.0, 10.0.15), `Microsoft.NET.Sdk.WebAssembly.Pack` 11.0.0, `Microsoft.DotNet.ILCompiler` + `runtime.win-x64.Microsoft.DotNet.ILCompiler` 11.0.0, `Microsoft.NETCore.App.Runtime.NativeAOT.win-x64` 11.0.0 ほか win-x64 ランタイムパック, `Microsoft.NETCore.App.Crossgen2.win-x64` 11.0.0 | 11.0.0 / 10.0.15 |

**ビルド時間**

| ID | 結果 | 秒 |
| --- | --- | --- |
| native-jit | OK | 0.8 |
| native-aot | OK | 6.8 |
| mono-interp | OK | 69 |
| mono-interp-default | OK | 22.2 |
| mono-aot | OK | 210.3 |
| mono-aot-default | OK | 162.7 |
| coreclr-interp | OK | 16.5 |
| netwasm-0.14.2-o3 | OK | 31 |
| netwasm-0.14.2-o3-ignore | OK | 8.7 |
| netwasm-0.14.2-oz | OK | 9 |
| netwasm-0.13.0-o3 | OK | 7.4 |
| naot-llvm | OK | 5.9 |
| naot-llvm-speed | OK | 9.6 |
| naot-llvm-1mod | OK | 18.1 |

上表は NuGet 取得済み・MSBuild/Roslyn サーバー起動済みの状態で、`art-clean` と `out-clean` を消してから直列に行った再ビルドの所要時間である ([results/build-times-clean.json](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/build-times-clean.json))。パッケージ取得や Emscripten のキャッシュ生成を含む初回ビルドはこれより長く、計測用ビルド時の値は [results/build-times.json](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/build-times.json) にある。

### Inferences
- 既定の設定でそのまま使えるのは Mono と NetWasm であり、CoreCLR Wasm と NativeAOT-LLVM は構成の回避策を要した。
- NativeAOT-LLVM の結果は net10.0 ベースであり、.NET 11 の BCL で動いた数値ではない。

### Gaps
- NativeAOT-LLVM の WASI 版は未測定。`wasm32-unknown-wasip2` のコンポーネント出力になり、実行に Wasmtime などが要る一方、このマシンに `wasmtime` はなく、Node で直接実行できないため。
- `OptimizationPreference=Speed` が動いた場合の NativeAOT-LLVM の速度は不明。
- CoreCLR の trim + relink が成功した場合のサイズ・速度は不明。

## Q2. 高速経路 (コンテキストなし) の実行速度はネイティブ JIT と JS アンカーに対してどの程度か

### Takeaway
n=100,000 random の幾何平均 (ライブラリ 6 算法、native-jit=1、同一プロセスの JS plainQuick でドリフト正規化) は NativeAOT-LLVM が 1.09〜1.13 倍で最も近い。Mono AOT 1.88 倍と NetWasm O3 1.97 倍はほぼ同等で (n=1,000,000 の生値では順序が逆転する)、NetWasm の `IgnoreAggressive` は 2.16 倍、Mono jiterpreter は 4.13 倍、CoreCLR interpreter は約 328 倍だった。Edge の値は Node の 0.93〜1.05 倍 (正規化後、CoreCLR を除く) で傾向は同じだった。

### Cited Findings
**算法の選定** (公開 API のみを呼ぶ): `PDQSort` (比較・分岐多いハイブリッド quick)、`TimSort` (安定マージ・適応型)、`Driftsort` (安定マージ。ネイティブでは `Avx512F.IsSupported` 時のみ AVX-512 分割を使い Wasm では同経路が動かない)、`HeapSort` (キャッシュに不利・非適応)、`ShellSortCiura2001` (挿入系)、`RadixLSD256Sort` (整数分布)。追加で `FlashSort` (要素ごとに `UInt128` 乗除算)、`BucketSortInteger` (要素ごとに `long` 除算)、および抽象化なしの `PlainQuick(C#)` (JS アンカーと同一アルゴリズムの C# 版)。

**データと手順**: int32、n = 1,000 / 100,000 / 1,000,000、random / sorted / reversed / duplicates (40 種)。生成は xorshift32 を C# と JS に同一実装。n=1,000 は入力 64 本 (シード別) を 1 回の呼び出しでまとめて整列する。1 セルは 3 回のウォームアップ (n=1,000,000 は 2 回) と 5 サンプル (同 3 回)、毎回異なるシード。主測定はプロセス分離の round robin で 5 ラウンド (CoreCLR は 1 ラウンド、ウォームアップ 1、最大 3 サンプル)。表の値はラウンド中央値の中央値。

**検証**: 全サンプルで整列済み判定と順序非依存チェックサムを確認し、各セルの先頭と末尾のサンプルで出力の FNV-1a ダイジェストを JS 側の `Int32Array.prototype.sort` の結果と照合した。

**正当性チェック結果**

- Node 主測定: 集計セル 1222、検証失敗 0、時間上限でスキップ 2、比較回数等の統計が食い違ったセル 0 / 240
- Node reduced: 集計セル 600、検証失敗 0、時間上限でスキップ 0、比較回数等の統計が食い違ったセル 0 / 32
- Edge: 集計セル 652、検証失敗 0、時間上限でスキップ 0、比較回数等の統計が食い違ったセル 4 / 16

Edge の統計不一致 4 セルは、`coreclr-interp` のみ縮小プラン (`lite`) でセル順が異なりシード列が違うためで、同じプランの 6 ツールチェーン間では一致している。比較回数・交換回数などの統計は、Node の 240 セルすべてで全ツールチェーンが一致した。— [results/summary-speed.json](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/summary-speed.json)

**高速経路 (NullContext) random n=1,000: ns/要素 (括弧内は native-jit 比。同一プロセスの JS plainQuick で正規化)**

| アルゴリズム | native-jit | native-aot | mono-interp | mono-aot | coreclr-interp | naot-llvm | naot-llvm-speed | netwasm-o3 | netwasm-o3-ignore |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| PDQSort | 24.0 (1.00) | 25.8 (1.03) | 59.4 (2.46) | 32.1 (1.33) | 7875 (316) | 26.0 (1.11) | 27.4 (1.16) | 35.5 (1.50) | 37.0 (1.53) |
| TimSort | 38.2 (1.00) | 42.5 (1.06) | 154 (4.05) | 50.7 (1.32) | 13616 (346) | 40.4 (1.10) | 40.0 (1.05) | 67.4 (1.82) | 69.0 (1.86) |
| Driftsort | 13.7 (1.00) | 16.2 (1.15) | 107 (8.81) | 28.6 (2.35) | 8372 (664) | 21.2 (1.75) | 21.4 (1.79) | 63.7 (5.15) | 68.0 (5.66) |
| HeapSort | 32.4 (1.00) | 33.2 (1.10) | 102 (3.45) | 28.2 (0.97) | 11031 (360) | 20.7 (0.69) | 20.5 (0.70) | 50.5 (1.76) | 83.2 (2.82) |
| ShellSortCiura2001 | 37.5 (1.00) | 37.9 (1.04) | 97.5 (2.69) | 58.4 (1.62) | 8114 (217) | 37.0 (1.03) | 37.3 (1.02) | 46.5 (1.32) | 47.9 (1.33) |
| RadixLSD256Sort | 6.98 (1.00) | 8.54 (1.38) | 77.8 (12.0) | 41.5 (6.63) | 2200 (341) | 9.25 (1.47) | 9.01 (1.45) | 10.2 (1.67) | 10.2 (1.66) |
| PlainQuick(C#) | 24.3 (1.00) | 25.3 (1.11) | 48.8 (2.16) | 25.9 (1.14) | 538 (23.0) | 25.2 (1.14) | 25.6 (1.14) | 24.4 (1.11) | 24.4 (1.11) |
| FlashSort | 35.9 (1.00) | 38.0 (1.05) | 481 (14.1) | 107 (2.91) | 4090 (111) | 17.8 (0.50) | 21.4 (0.60) | 183 (5.16) | 184 (5.30) |
| BucketSortInteger | 6.56 (1.00) | 7.20 (1.09) | 25.9 (4.02) | 21.4 (3.32) | 1217 (182) | 7.59 (1.17) | 7.06 (1.08) | 9.64 (1.51) | 9.60 (1.51) |

**高速経路 (NullContext) random n=100,000: ns/要素 (括弧内は native-jit 比。同一プロセスの JS plainQuick で正規化)**

| アルゴリズム | native-jit | native-aot | mono-interp | mono-aot | coreclr-interp | naot-llvm | naot-llvm-speed | netwasm-o3 | netwasm-o3-ignore |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| PDQSort | 40.6 (1.00) | 41.0 (1.01) | 87.5 (2.10) | 52.2 (1.30) | 11299 (230) | 43.4 (1.03) | 43.8 (1.07) | 56.2 (1.42) | 56.4 (1.43) |
| TimSort | 60.9 (1.00) | 66.3 (1.08) | 188 (2.90) | 79.8 (1.31) | 27428 (371) | 64.5 (1.03) | 65.4 (1.07) | 109 (1.83) | 109 (1.85) |
| Driftsort | 14.7 (1.00) | 18.6 (1.26) | 140 (9.33) | 39.2 (2.65) | 13135 (734) | 29.4 (1.94) | 30.1 (2.03) | 100 (6.98) | 108 (7.49) |
| HeapSort | 56.2 (1.00) | 58.4 (1.07) | 173 (2.96) | 52.2 (0.93) | 18905 (278) | 38.7 (0.67) | 37.8 (0.68) | 87.3 (1.60) | 141 (2.57) |
| ShellSortCiura2001 | 70.4 (1.00) | 75.6 (1.06) | 188 (2.62) | 115 (1.61) | 15316 (179) | 70.4 (0.97) | 69.3 (0.98) | 88.6 (1.30) | 92.7 (1.29) |
| RadixLSD256Sort | 6.55 (1.00) | 10.3 (1.64) | 75.4 (11.4) | 41.8 (6.48) | 3111 (399) | 9.53 (1.28) | 8.43 (1.32) | 9.56 (1.53) | 9.84 (1.55) |
| PlainQuick(C#) | 40.2 (1.00) | 42.7 (1.07) | 74.7 (1.85) | 42.1 (1.06) | 1970 (40.8) | 45.3 (1.05) | 42.6 (1.07) | 39.6 (1.03) | 39.6 (1.04) |
| FlashSort | 45.7 (1.00) | 47.1 (1.04) | 495 (11.1) | 108 (2.41) | 4238 (78.6) | 27.6 (0.53) | 30.2 (0.65) | 196 (4.37) | 193 (4.40) |
| BucketSortInteger | 7.06 (1.00) | 7.32 (1.06) | 25.5 (3.57) | 22.1 (3.12) | 1193 (139) | 7.63 (1.06) | 7.11 (1.01) | 10.1 (1.45) | 10.0 (1.44) |

**高速経路 (NullContext) random n=1,000,000: ns/要素 (括弧内は native-jit 比。同一プロセスの JS plainQuick で正規化)**

| アルゴリズム | native-jit | native-aot | mono-interp | mono-aot | coreclr-interp | naot-llvm | naot-llvm-speed | netwasm-o3 | netwasm-o3-ignore |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| PDQSort | 48.6 (1.00) | 49.3 (1.02) | 102 (2.06) | 62.9 (1.29) | 16357 (291) | 54.2 (1.03) | 51.3 (1.03) | 69.5 (1.43) | 71.0 (1.41) |
| TimSort | 74.8 (1.00) | 74.9 (1.01) | 217 (2.88) | 97.4 (1.34) | 未測定(時間上限) | 78.0 (1.02) | 79.4 (1.04) | 135 (1.83) | 139 (1.82) |
| Driftsort | 15.8 (1.00) | 15.5 (1.01) | 163 (9.99) | 43.8 (2.79) | 18700 (1008) | 34.1 (2.06) | 33.2 (2.05) | 118 (7.44) | 138 (8.05) |
| HeapSort | 74.1 (1.00) | 73.1 (1.01) | 256 (3.41) | 112 (1.24) | 29624 (347) | 62.7 (0.82) | 60.7 (0.81) | 119 (1.69) | 181 (2.58) |
| ShellSortCiura2001 | 91.4 (1.00) | 88.4 (0.97) | 236 (2.53) | 145 (1.55) | 19717 (186) | 88.7 (0.94) | 83.2 (0.90) | 112 (1.27) | 115 (1.24) |
| RadixLSD256Sort | 6.78 (1.00) | 6.91 (1.04) | 81.7 (11.8) | 41.8 (6.14) | 2160 (275) | 9.27 (1.32) | 8.18 (1.20) | 10.0 (1.54) | 10.7 (1.56) |
| PlainQuick(C#) | 47.7 (1.00) | 51.3 (1.02) | 93.9 (1.80) | 50.5 (1.01) | 925 (16.2) | 49.4 (0.99) | 48.8 (1.01) | 47.4 (1.00) | 48.7 (0.97) |

**native-jit 比の幾何平均 (ライブラリ 6 アルゴリズム、高速経路、JS plainQuick によるドリフト正規化)**

| n | パターン | native-jit | native-aot | mono-interp | mono-aot | coreclr-interp | naot-llvm | naot-llvm-speed | netwasm-o3 | netwasm-o3-ignore |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 1,000 | random | 1.00 | 1.12 | 4.63 | 1.87 | 353 | 1.15 | 1.15 | 1.95 | 2.15 |
| 1,000 | sorted | 1.00 | 1.15 | 8.23 | 2.93 | 1239 | 1.72 | 1.51 | 3.80 | 4.46 |
| 1,000 | reversed | 1.00 | 1.18 | 7.25 | 2.74 | 569 | 1.51 | 1.45 | 3.33 | 3.86 |
| 1,000 | duplicates | 1.00 | 1.09 | 4.24 | 1.85 | 253 | 1.07 | 1.07 | 1.93 | 2.15 |
| 100,000 | random | 1.00 | 1.17 | 4.13 | 1.88 | 328 | 1.09 | 1.13 | 1.97 | 2.16 |
| 100,000 | sorted | 1.00 | 1.04 | 6.36 | 2.10 | 1096 | 1.30 | 1.19 | 3.02 | 3.50 |
| 100,000 | reversed | 1.00 | 1.13 | 6.85 | 2.26 | 963 | 1.27 | 1.26 | 2.89 | 3.34 |
| 100,000 | duplicates | 1.00 | 1.14 | 4.51 | 2.24 | 227 | 1.33 | 1.32 | 2.53 | 2.77 |
| 1,000,000 | random | 1.00 | 1.01 | 4.27 | 1.96 | 349* | 1.14 | 1.12 | 2.00 | 2.17 |
| 1,000,000 | sorted | 1.00 | 0.96 | 6.24 | 2.11 | 1035 | 1.24 | 1.19 | 3.03 | 3.50 |
| 1,000,000 | reversed | 1.00 | 0.94 | 6.32 | 2.10 | 751* | 1.23 | 1.20 | 2.81 | 3.10 |
| 1,000,000 | duplicates | 1.00 | 0.96 | 5.12 | 2.31 | 404 | 1.33 | 1.27 | 2.58 | 2.87 |

`*` は一部アルゴリズムのみの幾何平均 (時間上限で欠けたセルあり)。

**JS アンカー比の幾何平均 (ライブラリ 6 アルゴリズム、高速経路 random。1 未満は JS アンカーより速い)**

| n | アンカー | native-jit | native-aot | mono-interp | mono-aot | coreclr-interp | naot-llvm | naot-llvm-speed | netwasm-o3 | netwasm-o3-ignore |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 1,000 | Int32Array.prototype.sort | 0.84 | 0.96 | 3.78 | 1.60 | 291 | 0.95 | 0.97 | 1.67 | 1.84 |
| 1,000 | plain JS quicksort | 0.65 | 0.73 | 3.00 | 1.21 | 229 | 0.74 | 0.74 | 1.26 | 1.40 |
| 100,000 | Int32Array.prototype.sort | 0.71 | 0.86 | 3.04 | 1.38 | 251 | 0.81 | 0.82 | 1.46 | 1.63 |
| 100,000 | plain JS quicksort | 0.59 | 0.69 | 2.42 | 1.10 | 192 | 0.64 | 0.66 | 1.15 | 1.27 |
| 1,000,000 | Int32Array.prototype.sort | 0.71 | 0.74 | 3.01 | 1.42 | 125 | 0.83 | 0.83 | 1.45 | 1.60 |
| 1,000,000 | plain JS quicksort | 0.59 | 0.59 | 2.50 | 1.15 | 178 | 0.67 | 0.65 | 1.17 | 1.27 |

**JS アンカー自体の ns/要素 (全 Node 実行の中央値)**

| n | パターン | Int32Array.sort | plainQuick(JS) |
| --- | --- | --- | --- |
| 1,000 | random | 24.3 | 31.8 |
| 1,000 | sorted | 2.22 | 5.96 |
| 1,000 | reversed | 3.40 | 6.39 |
| 1,000 | duplicates | 15.1 | 25.7 |
| 100,000 | random | 42.7 | 53.9 |
| 100,000 | sorted | 4.10 | 11.0 |
| 100,000 | reversed | 5.37 | 11.5 |
| 100,000 | duplicates | 13.2 | 31.1 |
| 1,000,000 | random | 52.3 | 64.1 |
| 1,000,000 | sorted | 4.81 | 13.0 |
| 1,000,000 | reversed | 6.45 | 13.6 |
| 1,000,000 | duplicates | 12.8 | 34.8 |

- 全セルの生データは [results/speed.jsonl](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/speed.jsonl)、集計は [results/summary-speed.csv](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/summary-speed.csv) (列: ns/要素、ラウンド最小・最大、JS アンカー比、native 比、ドリフト正規化比)。
- ライブラリ 6 算法の ns/要素の幾何平均 (random、生値): n=100,000 は Node で `naot-llvm` 35.9、`naot-llvm-speed` 35.2、`mono-aot` 58.7、`netwasm-o3` 59.7、`netwasm-o3-ignore` 66.3、`mono-interp` 133.3、`coreclr-interp` 12,418。Edge で 33.3、33.2、53.9、59.4、66.5、129.9 (`coreclr-interp` は 6 算法そろわず未算出)。n=1,000,000 は Node で `mono-aot` 75.3、`netwasm-o3` 72.7、Edge で 65.3、71.3 であり、この 2 者の順序は n とホストで入れ替わる。
- n=1,000 のセルは 1 回の処理が短くウォームアップの総時間も短いため、V8 の最適化層への昇格が途中の可能性がある。
- `Driftsort` のネイティブ値は AVX-512 経路を含むため、Wasm との比は他算法より大きく出る (実行時に `Avx512F.IsSupported=true`、Wasm はすべて false)。
- `RadixLSD256Sort` は `native-jit` が 6.55 ns に対し `mono-aot` 41.8 ns (6.48 倍)、`mono-interp` 75.4 ns (11.4 倍) で、他の算法より差が大きい。`naot-llvm` (9.53 ns) と `netwasm-o3` (9.56 ns) ではこの開きが出ていない。
- `PlainQuick(C#)` の n=100,000 random は `mono-aot` 42.1 ns、`naot-llvm` 45.3 ns、`netwasm-o3` 39.6 ns で native-jit 40.2 ns とほぼ同等である。

**コールド (初回) と定常**

**コールド: 新規プロセスで最初の 1 回 (n=100,000 random) と定常値 (ms、3 プロセスの中央値。定常 = 後半 4 回の中央値)**

| ツールチェーン | PDQSort 初回/定常 | TimSort 初回/定常 | Driftsort 初回/定常 | HeapSort 初回/定常 | ShellSortCiura2001 初回/定常 | RadixLSD256Sort 初回/定常 | PlainQuick(C#) 初回/定常 |
| --- | --- | --- | --- | --- | --- | --- | --- |
| native-jit-tiered | 21.2/15.2 | 40.7/5.89 | 21.1/10.9 | 8.78/5.72 | 8.51/6.72 | 5.76/0.71 | 9.92/6.67 |
| native-jit | 11.7/3.99 | 17.8/5.99 | 14.9/1.52 | 8.59/5.25 | 9.46/6.98 | 5.30/0.71 | 5.09/4.12 |
| native-aot | 3.86/3.90 | 6.05/5.85 | 1.64/1.42 | 5.09/5.18 | 6.80/6.80 | 1.01/0.63 | 3.86/3.95 |
| mono-interp | 92.6/15.1 | 131/38.6 | 82.0/20.8 | 69.1/16.4 | 37.2/17.6 | 24.9/7.24 | 14.2/7.20 |
| mono-aot | 7.49/4.90 | 12.8/7.57 | 8.84/3.62 | 8.65/4.91 | 13.9/10.8 | 7.72/3.98 | 4.91/3.93 |
| coreclr-interp | 989/1020 | 1167/1153 | 1336/1304 | 1787/1802 | 1539/1513 | 207/199 | 84.3/81.8 |
| naot-llvm | 4.88/3.99 | 10.8/5.86 | 8.20/2.77 | 6.23/3.56 | 10.8/6.74 | 6.04/0.79 | 4.65/3.93 |
| naot-llvm-speed | 6.03/4.02 | 11.4/6.06 | 8.37/2.83 | 6.93/3.55 | 11.3/6.48 | 6.40/0.79 | 4.96/3.92 |
| netwasm-o3 | 10.2/5.46 | 14.8/10.7 | 14.6/9.41 | 14.4/8.73 | 15.1/8.67 | 4.96/0.96 | 6.46/3.88 |
| netwasm-o3-ignore | 9.38/5.47 | 14.8/10.7 | 15.3/10.6 | 17.6/13.6 | 15.0/8.75 | 4.85/0.93 | 6.49/3.90 |

- `mono-interp` は初回と定常の差が最大 (PDQSort 92.6 → 15.1 ms)。AOT 系 (`naot-llvm`, `mono-aot`, `netwasm-o3`) も V8 の階層化で 1.3〜2 倍程度の差がある。`coreclr-interp` は初回と定常の差がほぼない (PDQSort 989 → 1020 ms)。— [results/cold.jsonl](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/cold.jsonl)

**派生ビルド** (同一プランの基準ビルドとの比)

**派生ビルドの比較 (reduced プラン: n=1,000/100,000、random/sorted、6 アルゴリズム。基準ビルド比の幾何平均。JS plainQuick で正規化。1 未満が高速)**

| 比較 | 派生 | 基準 | 高速経路 (比) | セル数 | ISortContext (比) |
| --- | --- | --- | --- | --- | --- |
| mono-aot を基準とした interpreter (jiterpreter 無効) | mono-interp-nojiterp | mono-aot | 13.7 | 24 | 1.48 |
| mono-aot を基準とした interpreter (SDK 既定フラグ) | mono-interp-default | mono-aot | 2.52 | 24 | 1.13 |
| AOT: SDK 既定 (-Oz/-O2/-O2) vs -O3 | mono-aot-default | mono-aot | 1.13 | 24 | 1.10 |
| NativeAOT-LLVM: LLVM module 1 個 vs 既定 | naot-llvm-1mod | naot-llvm-speed | 0.98 | 24 | 0.88 |
| NativeAOT-LLVM: OptimizationPreference=Size vs speed 版 | naot-llvm-size | naot-llvm-speed | 1.04 | 24 | 0.88 |
| NetWasm: Converge | netwasm-o3-converge | netwasm-o3 | 1.00 | 24 | 0.99 |
| NetWasm: Compact GC | netwasm-o3-compact | netwasm-o3 | 0.98 | 24 | 1.00 |
| NetWasm: Bump GC | netwasm-o3-bump | netwasm-o3 | 0.98 | 24 | 0.98 |
| NetWasm: Oz | netwasm-oz | netwasm-o3 | 1.41 | 24 | 1.16 |
| NetWasm 0.13.0 vs 0.14.2 | netwasm-0.13-o3 | netwasm-o3 | 0.97 | 24 | 0.99 |

**参考: native-jit-tiered (既定 tiering) 対 native-jit (TC=0)。ウォームアップ 3 回では階層化が定常に達しないため定常性能の比較ではない**

幾何平均比: 2.85 (セル数 24)

**ブラウザー (Edge 157 headless)**

**Edge headless: 高速経路 random n=100,000 ns/要素 (括弧は同一ページの JS plainQuick 比)**

| アルゴリズム | mono-interp | mono-aot | coreclr-interp | naot-llvm | naot-llvm-speed | netwasm-o3 | netwasm-o3-ignore |
| --- | --- | --- | --- | --- | --- | --- | --- |
| PDQSort | 85.3 (1.63) | 55.4 (1.05) | 10760 (186) | 40.8 (0.79) | 40.8 (0.80) | 55.6 (1.11) | 56.3 (1.12) |
| TimSort | 172 (3.28) | 82.9 (1.58) | 12075 (208) | 59.9 (1.16) | 59.7 (1.17) | 112 (2.24) | 111 (2.22) |
| Driftsort | 143 (2.73) | 39.3 (0.75) | 未測定 | 28.1 (0.54) | 28.2 (0.55) | 96.6 (1.93) | 111 (2.22) |
| HeapSort | 168 (3.20) | 51.9 (0.99) | 19104 (329) | 36.5 (0.71) | 36.7 (0.72) | 83.0 (1.66) | 141 (2.82) |
| ShellSortCiura2001 | 184 (3.50) | 109 (2.08) | 未測定 | 66.1 (1.28) | 65.2 (1.28) | 88.4 (1.77) | 88.4 (1.77) |
| RadixLSD256Sort | 74.6 (1.42) | 23.8 (0.45) | 2220 (38.3) | 8.30 (0.16) | 8.15 (0.16) | 9.95 (0.20) | 10.0 (0.20) |
| PlainQuick(C#) | 74.3 (1.42) | 42.6 (0.81) | 773 (13.3) | 40.9 (0.79) | 39.7 (0.78) | 40.2 (0.80) | 40.1 (0.80) |

**Edge headless: 高速経路 random n=1,000,000 ns/要素 (括弧は同一ページの JS plainQuick 比)**

| アルゴリズム | mono-interp | mono-aot | coreclr-interp | naot-llvm | naot-llvm-speed | netwasm-o3 | netwasm-o3-ignore |
| --- | --- | --- | --- | --- | --- | --- | --- |
| PDQSort | 101 (1.62) | 61.7 (0.99) | 未測定 | 49.4 (0.84) | 50.6 (0.86) | 65.7 (1.10) | 67.0 (1.14) |
| TimSort | 204 (3.27) | 99.2 (1.59) | 未測定 | 72.9 (1.23) | 73.6 (1.25) | 135 (2.26) | 134 (2.29) |
| Driftsort | 159 (2.55) | 44.2 (0.71) | 未測定 | 35.4 (0.60) | 33.8 (0.57) | 114 (1.91) | 130 (2.21) |
| HeapSort | 209 (3.35) | 85.5 (1.37) | 未測定 | 58.2 (0.99) | 60.7 (1.03) | 114 (1.92) | 173 (2.96) |
| ShellSortCiura2001 | 232 (3.72) | 139 (2.23) | 未測定 | 81.9 (1.39) | 81.9 (1.39) | 111 (1.85) | 111 (1.90) |
| RadixLSD256Sort | 79.1 (1.27) | 24.2 (0.39) | 未測定 | 8.19 (0.14) | 8.07 (0.14) | 10.3 (0.17) | 10.3 (0.18) |
| PlainQuick(C#) | 87.9 (1.41) | 51.8 (0.83) | 未測定 | 48.3 (0.82) | 47.6 (0.81) | 47.9 (0.80) | 47.6 (0.81) |

**Edge と Node の比較: ns/要素 比 (Edge ÷ Node、ライブラリ 6 アルゴリズムの幾何平均。random, n=100,000)**

| ツールチェーン | Edge ÷ Node (生値) | Edge ÷ Node (JS plainQuick 正規化) |
| --- | --- | --- |
| mono-interp | 0.97 | 1.02 |
| mono-aot | 0.92 | 0.93 |
| coreclr-interp | 0.74 | 0.83 |
| naot-llvm | 0.93 | 1.01 |
| naot-llvm-speed | 0.94 | 0.99 |
| netwasm-o3 | 0.99 | 1.03 |
| netwasm-o3-ignore | 1.00 | 1.05 |

- Edge の生データは [results/browser-speed.jsonl](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/browser-speed.jsonl)。ウォームアップ 2、サンプル 3、1 ラウンド。`coreclr-interp` は n=1,000/100,000 の 5 算法 (`lite`) のみで、表の「未測定」は意図的な範囲外である。

### Inferences
- 同じ C# の素朴なクイックソートは AOT 系 3 種でネイティブ比 1.03〜1.07 倍に収まるので (ドリフト正規化値)、ライブラリ算法での差はコード生成の平均性能よりも、抽象 (`SortSpan`、比較子、バッファ) の展開度合いと SIMD 経路の有無に由来する部分が大きいと考えられる。
- `naot-llvm-speed` は `naot-llvm` に対し n=100,000 で大きな改善がなく、この算法群では `-msimd128` と wasm-opt `-O3` の効果は限定的と考えられる。
- Mono の `-O3` は既定フラグ比 1/1.13 の改善で、サイズ増は raw で約 1.2%。

### Gaps
- Firefox と Safari は未測定 (このマシンに測定環境がない)。Chromium 系は Edge のみ。
- 階層化の定常値は、このプロセス内のウォームアップ回数の範囲での値であり、ページの長時間動作後の挙動は確認していない。
- `mono-aot` の `RadixLSD256Sort` が相対的に遅い原因は特定していない。

## Q3. 観測経路 (コンテキストコールバック) のコストはツールチェーンでどう変わるか

### Takeaway
n=100,000 random の PDQSort で、`ISortContext` 経由は NetWasm 103 ns/要素と最も低く、native-jit の 146 ns を下回った。一方で Mono AOT は高速経路の 26 倍 (1,362 ns)、Mono jiterpreter は 19.5 倍 (1,703 ns) に膨らみ、Mono での記録処理は AOT の利点がほぼ消える。

### Cited Findings
`StatisticsContext` (sealed class) と `ISortContext` (インターフェース型引数) を、各呼び出し箇所で型引数を明示して呼んだ (`shared/Kernel.cs` の `SortObserved`)。

**観測経路の倍率 (n=100,000 random。StatisticsContext / ISortContext の ns/要素 ÷ 同ツールチェーン高速経路)**

| アルゴリズム | native-jit Stat | native-jit IF | native-aot Stat | native-aot IF | mono-interp Stat | mono-interp IF | mono-aot Stat | mono-aot IF | coreclr-interp Stat | coreclr-interp IF | naot-llvm Stat | naot-llvm IF | naot-llvm-speed Stat | naot-llvm-speed IF | netwasm-o3 Stat | netwasm-o3 IF | netwasm-o3-ignore Stat | netwasm-o3-ignore IF |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| PDQSort | 3.72 | 3.61 | 4.12 | 3.63 | 8.49 | 19.5 | 24.7 | 26.1 | 0.79 | 0.77 | 8.69 | 8.19 | 8.66 | 8.73 | 1.89 | 1.84 | 1.86 | 2.00 |
| TimSort | 5.97 | 6.16 | 5.84 | 5.71 | 5.12 | 14.9 | 30.4 | 34.8 | 1.01 | 0.95 | 10.8 | 10.5 | 10.4 | 10.7 | 1.97 | 2.06 | 1.96 | 1.98 |
| Driftsort | 17.9 | 18.8 | 14.3 | 13.4 | 3.62 | 18.2 | 59.7 | 76.6 | 3.82 | 3.45 | 17.7 | 17.6 | 18.6 | 18.7 | 1.70 | 1.66 | 1.54 | 1.53 |
| HeapSort | 3.30 | 6.68 | 4.08 | 3.42 | 7.41 | 20.9 | 50.5 | 59.0 | 1.35 | 1.51 | 20.6 | 20.2 | 22.8 | 22.8 | 3.27 | 3.34 | 2.77 | 2.87 |
| ShellSortCiura2001 | 4.18 | 4.17 | 4.45 | 3.72 | 6.77 | 15.8 | 19.4 | 18.1 | 0.79 | 0.84 | 8.50 | 8.56 | 11.9 | 12.2 | 2.00 | 2.07 | 1.91 | 1.98 |
| RadixLSD256Sort | 7.17 | 7.21 | 3.89 | 3.37 | 1.24 | 6.78 | 12.7 | 14.5 | 3.45 | 2.20 | 8.35 | 8.38 | 11.3 | 11.3 | 2.37 | 2.47 | 2.41 | 2.38 |

**観測経路の ns/要素 (n=100,000 random, ISortContext)**

| アルゴリズム | native-jit | native-aot | mono-interp | mono-aot | coreclr-interp | naot-llvm | naot-llvm-speed | netwasm-o3 | netwasm-o3-ignore |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| PDQSort | 146 | 149 | 1703 | 1362 | 8709 | 355 | 382 | 103 | 113 |
| TimSort | 375 | 379 | 2796 | 2774 | 26092 | 679 | 696 | 224 | 216 |
| Driftsort | 277 | 249 | 2553 | 3005 | 45302 | 517 | 562 | 167 | 165 |
| HeapSort | 375 | 200 | 3609 | 3078 | 28505 | 782 | 865 | 292 | 404 |
| ShellSortCiura2001 | 293 | 281 | 2968 | 2080 | 12849 | 603 | 842 | 184 | 183 |
| RadixLSD256Sort | 47.2 | 34.6 | 511 | 606 | 6843 | 79.8 | 95.3 | 23.6 | 23.5 |

- 全算法の Node 値は [results/summary-speed.csv](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/summary-speed.csv) の `ctx=1,2` 行にある。比較回数・交換回数などの統計は全ツールチェーンで一致した。
- `coreclr-interp` は観測経路が高速経路より遅くならない算法がある (PDQSort 比 0.77〜0.79)。CoreCLR の高速経路は 1 ラウンド・ウォームアップ 1 回のみで、この比の精度は他より低い。

### Inferences
- Mono AOT の観測経路の値は Mono interpreter と同じ桁にある。`StatisticsContext`/`ISortContext` の共有ジェネリックのインスタンス化が AOT されず interpreter へフォールバックしている可能性があるが、ログでは未確認であり推定である。
- NetWasm は観測経路の倍率が 1.5〜3.3 倍と小さく、記録を多用する用途では高速経路の順位と異なる結論になる。

### Gaps
- フォールバックの有無を確認していない (Mono の AOT ログ・メソッド単位の計測は未実施)。
- 実アプリの記録コンテキスト (配列へのイベント追記) は測っていない。`StatisticsContext` はカウンタ加算のみである。

## Q4. 配布サイズはどの程度か

### Takeaway
Brotli 合計は NetWasm O3 が約 107 KiB、NativeAOT-LLVM が約 289〜308 KiB、Mono interpreter が約 814 KiB、Mono AOT が約 1,540 KiB、CoreCLR interpreter が約 1,840 KiB だった。全構成で ICU を含まない (`InvariantGlobalization=true`、NetWasm は対象外)。

### Cited Findings
**配布サイズ (KiB、raw / gzip -9 / Brotli q11 lgwin24)**

| ツールチェーン | ファイル数 | 合計 | runtime/app .wasm | managed (webcil) | JS | ICU | その他 |
| --- | --- | --- | --- | --- | --- | --- | --- |
| native-aot | 1 | 1500 / 722 / 592 | 1500 / 722 / 592 | 0.0 / 0.0 / 0.0 | 0.0 / 0.0 / 0.0 | 0.0 / 0.0 / 0.0 | 0.0 / 0.0 / 0.0 |
| mono-interp | 7 | 2529 / 997 / 814 | 1134 / 488 / 394 | 1065 / 416 / 340 | 331 / 93 / 80 | 0.0 / 0.0 / 0.0 | 0.0 / 0.0 / 0.0 |
| mono-aot | 7 | 6476 / 2098 / 1540 | 5079 / 1680 / 1200 | 1065 / 324 / 261 | 331 / 93 / 80 | 0.0 / 0.0 / 0.0 | 0.0 / 0.0 / 0.0 |
| coreclr-interp | 7 | 6393 / 2403 / 1840 | 4643 / 1763 / 1319 | 1549 / 585 / 473 | 200 / 55 / 48 | 0.0 / 0.0 / 0.0 | 0.0 / 0.0 / 0.0 |
| naot-llvm | 2 | 1150 / 407 / 308 | 1069 / 384 / 289 | 0.0 / 0.0 / 0.0 | 81 / 23 / 20 | 0.0 / 0.0 / 0.0 | 0.0 / 0.0 / 0.0 |
| naot-llvm-speed | 2 | 927 / 373 / 289 | 891 / 362 / 280 | 0.0 / 0.0 / 0.0 | 36 / 10 / 9.4 | 0.0 / 0.0 / 0.0 | 0.0 / 0.0 / 0.0 |
| netwasm-o3 | 14 | 561 / 142 / 107 | 353 / 90 / 61 | 0.0 / 0.0 / 0.0 | 197 / 50 / 43 | 0.0 / 0.0 / 0.0 | 11 / 2.5 / 2.1 |
| netwasm-o3-ignore | 14 | 523 / 133 / 105 | 315 / 81 / 59 | 0.0 / 0.0 / 0.0 | 197 / 50 / 43 | 0.0 / 0.0 / 0.0 | 11 / 2.5 / 2.1 |
| mono-interp-default | 7 | 2550 / 983 / 815 | 1155 / 474 / 395 | 1065 / 416 / 341 | 331 / 93 / 80 | 0.0 / 0.0 / 0.0 | 0.0 / 0.0 / 0.0 |
| mono-aot-default | 7 | 6400 / 2125 / 1529 | 5004 / 1708 / 1189 | 1065 / 324 / 261 | 331 / 93 / 80 | 0.0 / 0.0 / 0.0 | 0.0 / 0.0 / 0.0 |
| naot-llvm-1mod | 2 | 1005 / 395 / 305 | 969 / 385 / 296 | 0.0 / 0.0 / 0.0 | 36 / 10 / 9.4 | 0.0 / 0.0 / 0.0 | 0.0 / 0.0 / 0.0 |
| naot-llvm-size | 2 | 1106 / 395 / 299 | 1025 / 372 / 280 | 0.0 / 0.0 / 0.0 | 81 / 23 / 20 | 0.0 / 0.0 / 0.0 | 0.0 / 0.0 / 0.0 |
| naot-llvm-jsapi | 5 | 2155 / 732 / 545 | 1740 / 618 / 448 | 0.0 / 0.0 / 0.0 | 416 / 114 / 96 | 0.0 / 0.0 / 0.0 | 0.0 / 0.0 / 0.0 |
| netwasm-o3-converge | 14 | 559 / 141 / 106 | 351 / 89 / 61 | 0.0 / 0.0 / 0.0 | 197 / 50 / 43 | 0.0 / 0.0 / 0.0 | 11 / 2.5 / 2.1 |
| netwasm-o3-compact | 14 | 549 / 139 / 104 | 341 / 86 / 59 | 0.0 / 0.0 / 0.0 | 197 / 50 / 43 | 0.0 / 0.0 / 0.0 | 11 / 2.5 / 2.1 |
| netwasm-o3-bump | 14 | 539 / 135 / 101 | 331 / 82 / 55 | 0.0 / 0.0 / 0.0 | 197 / 50 / 43 | 0.0 / 0.0 / 0.0 | 11 / 2.5 / 2.1 |
| netwasm-oz | 14 | 425 / 115 / 94 | 217 / 62 / 49 | 0.0 / 0.0 / 0.0 | 197 / 50 / 43 | 0.0 / 0.0 / 0.0 | 11 / 2.5 / 2.1 |
| netwasm-0.13-o3 | 14 | 542 / 137 / 103 | 336 / 84 / 58 | 0.0 / 0.0 / 0.0 | 196 / 50 / 43 | 0.0 / 0.0 / 0.0 | 11 / 2.3 / 2.0 |

- 集計対象は実際に配信するファイル。`dotnet.js` 系は `_framework` の全ファイル、NativeAOT-LLVM は `audit.js` + `audit.wasm`、NetWasm は `audit.mjs` からの import 連鎖に、Edge が実際に要求した `audit.wasm`, `audit.raw-adapter.mjs`, `audit.wasm.type-symbols`, `interop.json`, `runtime-layout.json` を加えた。Edge の要求ログとの突き合わせは [results/browser-startup.jsonl](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/browser-startup.jsonl) の `served`。
- 圧縮は Node `zlib` の gzip level 9 と Brotli quality 11, lgwin 24。ファイル別の値は [results/size.json](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/size.json)。

### Inferences
- NetWasm の差は Mono/CoreCLR のような IL・ランタイム一式を配らないことによる。Mono AOT は `.wasm` が大きく、webcil の分も残る。
- `naot-llvm-speed` が `naot-llvm` より小さいのは wasm-opt と JS 最小化の差である。

### Gaps
- 実アプリ規模のライブラリ追加時の増分は測っていない。今回の入口はソート数種と計測用 export に限られる。
- HTTP 配信時の実転送量や CDN 圧縮設定の影響は未測定。

## Q5. 起動時間はどの程度か

### Takeaway
Node でプロセス起動から最初のソート完了まで、NativeAOT-LLVM 約 32〜36 ms、NetWasm 約 59 ms、Mono AOT 約 95 ms、Mono interpreter 約 106 ms、CoreCLR interpreter 約 197 ms だった。Edge (新規プロファイル、ローカル配信) ではナビゲーション開始から NativeAOT-LLVM 約 151 ms、NetWasm 約 189 ms、Mono interpreter 233 ms、Mono AOT 236 ms、CoreCLR interpreter 357 ms だった。

### Cited Findings
**起動 (Node 24、新規プロセス 12 回の中央値 [最小-最大]、ms。原点はプロセス起動)**

| ツールチェーン | 起動→初回ソート完了 | エントリ到達(Node 起動) | glue import | runtime 生成 | exports 準備 | WebAssembly.instantiate 等 合計 | 初回ソート(n=1,000) |
| --- | --- | --- | --- | --- | --- | --- | --- |
| native-jit-tiered | 68.7 [67.4-74.4] | 19.1 | 42.9 | 0.00 | 0.00 | 未測定 | 3.30 |
| native-jit | 79.5 [77.7-126] | 19.5 | 48.4 | 0.00 | 0.00 | 未測定 | 7.75 |
| native-aot | 35.3 [34.1-36.4] | 19.1 | 12.9 | 0.00 | 0.00 | 未測定 | 0.03 |
| mono-interp | 106 [104-110] | 19.0 | 4.20 | 62.8 | 11.8 | 2.37 | 5.16 |
| mono-aot | 94.5 [93.7-100] | 19.6 | 4.26 | 61.7 | 5.52 | 8.28 | 1.45 |
| coreclr-interp | 197 [195-203] | 20.0 | 4.28 | 105 | 46.1 | 147 | 16.5 |
| naot-llvm | 35.5 [32.4-40.3] | 20.3 | 2.29 | 3.94 | 0.00 | 2.53 | 0.23 |
| naot-llvm-speed | 31.9 [30.3-34.3] | 19.0 | 1.95 | 3.06 | 0.00 | 1.81 | 0.39 |
| netwasm-o3 | 59.2 [58.4-64.2] | 19.1 | 5.14 | 30.5 | 0.00 | 1.43 | 2.40 |
| netwasm-o3-ignore | 58.8 [57.7-60.6] | 18.9 | 5.18 | 30.3 | 0.00 | 1.44 | 2.14 |
| mono-aot-default | 95.1 [94.1-99.2] | 19.7 | 4.24 | 61.7 | 5.51 | 8.12 | 1.48 |
| naot-llvm-1mod | 32.5 [31.5-35.5] | 19.3 | 1.99 | 3.19 | 0.00 | 1.95 | 0.41 |
| netwasm-oz | 57.5 [56.3-64.8] | 18.7 | 5.15 | 30.4 | 0.00 | 1.46 | 1.29 |
| netwasm-0.13-o3 | 59.7 [58.0-62.8] | 19.7 | 5.14 | 30.2 | 0.00 | 1.46 | 2.44 |

**起動 (Edge headless、新規プロファイル 8 回の中央値、ms。原点はナビゲーション開始、ローカル HTTP 配信で非圧縮)**

| ツールチェーン | ナビゲーション→初回ソート完了 | glue import | runtime 生成 | WebAssembly.* 合計 | 初回ソート(n=1,000) | 取得ファイル数 | エラー |
| --- | --- | --- | --- | --- | --- | --- | --- |
| mono-interp | 233 | 3.64 | 67.0 | 5.26 | 6.69 | 7 | 0 |
| mono-aot | 236 | 4.27 | 89.3 | 16.7 | 1.62 | 7 | 0 |
| coreclr-interp | 357 | 4.54 | 120 | 142 | 22.9 | 7 | 0 |
| naot-llvm | 152 | 4.93 | 11.8 | 8.26 | 0.23 | 2 | 0 |
| naot-llvm-speed | 151 | 3.96 | 9.95 | 7.15 | 0.50 | 2 | 0 |
| netwasm-o3 | 189 | 17.0 | 26.7 | 2.88 | 2.70 | 14 | 0 |
| netwasm-o3-ignore | 190 | 16.7 | 28.1 | 5.34 | 2.57 | 14 | 0 |

- Node 実行の `WebAssembly.*` 合計は `WebAssembly.instantiate` などの Promise が解決するまでの時間の合計で、V8 の遅延コンパイルにより本体コンパイル全体は含まない。生データは [results/startup.jsonl](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/startup.jsonl), [results/browser-startup.jsonl](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/browser-startup.jsonl)。
- Edge の起点はナビゲーション開始で、ページと計測スクリプト読込 (glue の import 開始まで中央値 128〜137 ms) を含む。配信は `127.0.0.1` の無圧縮 HTTP、`Cross-Origin-Opener-Policy: same-origin` と `Cross-Origin-Embedder-Policy: require-corp` 付き。

### Inferences
- `coreclr-interp` は `WebAssembly.*` に 147 ms、`runtime 生成` に 105 ms を要し、Wasm 系で最も起動が重い。
- 起動の差は runtime 初期化 (Mono 系約 62 ms、NetWasm 約 30 ms、NativeAOT-LLVM 3〜4 ms) が主因である。

### Gaps
- 実ネットワーク遅延、Service Worker、コードキャッシュ有効時の再訪問は未測定 (毎回新規プロファイル)。

## Q6. JS 境界のコストと Int32Array の受け渡し手段

### Takeaway
空 export の呼び出しは NativeAOT-LLVM の直接 export が約 9 ns (JS 関数呼び出し約 5 ns の 2 倍弱) で、NetWasm 約 104 ns、Mono AOT 約 160 ns、Mono interpreter 約 190 ns、CoreCLR 約 5,331 ns だった。大きな配列は「線形メモリへ直接書く」手段が全系統で最速で、n=100,000 の書き込み + 呼び出しは 5.8〜12 µs、`int[]` コピー往復は 120〜190 µs だった。

### Cited Findings
**JS 境界コスト (Node 24、ns/呼び出し、3 プロセスの中央値。各値は 7 バッチ中央値)**

| ツールチェーン | JS 関数呼出 (基準) | 空 export | int→int export |
| --- | --- | --- | --- |
| mono-interp | 5.23 | 190 | 219 |
| mono-aot | 5.10 | 160 | 172 |
| coreclr-interp | 5.30 | 5331 | 5445 |
| naot-llvm | 6.20 | 9.10 | 8.25 |
| naot-llvm-speed | 5.63 | 8.83 | 7.84 |
| netwasm-o3 | 5.41 | 104 | 187 |
| netwasm-o3-ignore | 5.27 | 101 | 177 |
| mono-aot-default | 5.20 | 166 | 180 |
| mono-interp-default | 5.44 | 196 | 218 |
| naot-llvm-1mod | 5.67 | 8.70 | 7.56 |
| netwasm-oz | 5.42 | 116 | 192 |
| netwasm-0.13-o3 | 5.34 | 104 | 178 |

**Int32Array 受け渡し n=1,000 (ns/呼び出し。括弧は要素あたり ns)**

| 機構 | mono-interp | mono-aot | coreclr-interp | naot-llvm | naot-llvm-speed | netwasm-o3 | netwasm-o3-ignore |
| --- | --- | --- | --- | --- | --- | --- | --- |
| copy-in int[] (JSExport) | 998 (0.998) | 479 (0.479) | 7439 (7.439) | 該当なし | 該当なし | 2024 (2.024) | 2002 (2.002) |
| copy-roundtrip int[] (JSExport) | 2914 (2.914) | 2292 (2.292) | 10024 (10.024) | 該当なし | 該当なし | 3463 (3.463) | 3478 (3.478) |
| exported-buffer write+call | 302 (0.302) | 236 (0.236) | 5302 (5.302) | 43.2 (0.043) | 45.7 (0.046) | 該当なし | 該当なし |
| exported-buffer write+call+read | 1320 (1.320) | 1891 (1.891) | 7077 (7.077) | 1250 (1.250) | 1204 (1.204) | 該当なし | 該当なし |
| span view via JSImport (MemoryView.set) | 628 (0.628) | 368 (0.368) | 7021 (7.021) | 該当なし | 該当なし | 該当なし | 該当なし |
| exported-buffer write+call (memory captured from a Span view; unsupported use) | 該当なし | 該当なし | 該当なし | 該当なし | 該当なし | 226 (0.226) | 219 (0.219) |
| exported-buffer write+call+read (same) | 該当なし | 該当なし | 該当なし | 該当なし | 該当なし | 1418 (1.418) | 1521 (1.521) |
| span view via JSImport (Int32Array.set) | 該当なし | 該当なし | 該当なし | 該当なし | 該当なし | 412 (0.412) | 403 (0.403) |

**Int32Array 受け渡し n=100,000 (ns/呼び出し。括弧は要素あたり ns)**

| 機構 | mono-interp | mono-aot | coreclr-interp | naot-llvm | naot-llvm-speed | netwasm-o3 | netwasm-o3-ignore |
| --- | --- | --- | --- | --- | --- | --- | --- |
| copy-in int[] (JSExport) | 47352 (0.474) | 19118 (0.191) | 27251 (0.273) | 該当なし | 該当なし | 13190 (0.132) | 13067 (0.131) |
| copy-roundtrip int[] (JSExport) | 190755 (1.908) | 126744 (1.267) | 123105 (1.231) | 該当なし | 該当なし | 121305 (1.213) | 119744 (1.197) |
| exported-buffer write+call | 6287 (0.063) | 6312 (0.063) | 12204 (0.122) | 5805 (0.058) | 5830 (0.058) | 該当なし | 該当なし |
| exported-buffer write+call+read | 110364 (1.104) | 106871 (1.069) | 123574 (1.236) | 116549 (1.165) | 109480 (1.095) | 該当なし | 該当なし |
| span view via JSImport (MemoryView.set) | 6702 (0.067) | 6530 (0.065) | 14190 (0.142) | 該当なし | 該当なし | 該当なし | 該当なし |
| exported-buffer write+call (memory captured from a Span view; unsupported use) | 該当なし | 該当なし | 該当なし | 該当なし | 該当なし | 6404 (0.064) | 6291 (0.063) |
| exported-buffer write+call+read (same) | 該当なし | 該当なし | 該当なし | 該当なし | 該当なし | 110856 (1.109) | 110972 (1.110) |
| span view via JSImport (Int32Array.set) | 該当なし | 該当なし | 該当なし | 該当なし | 該当なし | 6673 (0.067) | 6657 (0.067) |

- 「該当なし」はその機構を公開 API で提供しないか、実装しなかった組み合わせである。NativeAOT-LLVM は `DotNetJsApi` なしでは `[JSExport]` のコピー経路がなく、`Module.HEAP32` と export だけを使う。
- 生データは [results/boundary.jsonl](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/boundary.jsonl)。各値は 3 プロセスの中央値、各プロセス内は 7 バッチの中央値。

**実際の export コード** (全文は Q8): Mono / CoreCLR / NetWasm は `shared/JsExports.cs` の `[JSExport]` (`CopyIn(int[])`, `CopyRoundtrip(int[])`, `BufPtr`, `TouchBuf`, `SpanImport`)、NativeAOT-LLVM は `shared/LlvmExports.cs` の `[UnmanagedCallersOnly(EntryPoint = "audit_*")]`。

**MemoryView の方向の確認** ([results/probe.jsonl](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/probe.jsonl), [logs/probe-mono-fatal.out](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/logs/probe-mono-fatal.out))

| 操作 | Mono (`mono-interp` 相当の probe) | CoreCLR (`coreclr-interp` 相当の probe) |
| --- | --- | --- |
| JS の `Int32Array` を `[JSExport] Span<int>` へ渡す | 失敗 `Assert failed: Expected MemoryViewType.Int32` | 同じ |
| JS の `Int32Array` を `[JSExport] ArraySegment<int>` へ渡す | **ランタイムが致命的 assert で停止** `Only roundtrip of ArraySegment instance created by C#` | 例外 (同文言、プロセスは継続) |
| C# が返した `ArraySegment<int>` の MemoryView に `set()` | 成功 (C# 側で値を確認) | 成功 |
| 同じ MemoryView を `slice()` | 成功 (コピー) | 成功 |
| 同じ MemoryView を `ArraySegment<int>` / `Span<int>` 引数へ戻す | 成功 | 成功 |

NetWasm は `[JSExport]` の `Span<int>` 引数がビルド時に `NW1003: ... JSExport currently supports primitive scalars, ...` で拒否された。`[JSImport]` の `Span<int>` 引数は動き、JS 側は線形メモリ上の `Int32Array` を受け取る。その `.buffer` を保持すれば export 外からも書き込めるが、これは文書化された使い方ではない (ビュー有効期間は呼び出し中のみ)。表の該当行はこの非公式手段である。— [logs/build-probe-netwasm.log](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/logs/build-probe-netwasm.log), [scripts/adapters.mjs](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/scripts/adapters.mjs)

NativeAOT-LLVM の `DotNetJsApi=true` (未文書) は `[JSExport]` が動いた: 起動 125 ms、空 export 87 ns、int 往復 120 ns、`int[]` コピー入力 506 ns、往復 1,917 ns (n=1,000)、PDQSort n=100,000 は 5.03 ms (いずれも 1 プロセス 1 回の測定)。— [results/naot-llvm-jsapi.json](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/naot-llvm-jsapi.json)

### Inferences
- 実アプリでは `int[]` コピー往復より、線形メモリのバッファへ JS が直接書く方式が n が大きいほど有利である。
- 呼び出し回数が多い用途 (要素ごとのコールバック等) では、NativeAOT-LLVM の直接 export は他系統の 1/10 以下の呼び出しコストで済む (NetWasm 比約 12 倍、Mono 比約 18〜21 倍、CoreCLR 比約 590 倍)。

### Gaps
- `Module.HEAP32` は NativeAOT-LLVM の `ALLOW_MEMORY_GROWTH` によるメモリ拡張で張り替えられるため、拡張を挟むケースの実測はしていない (ビューは都度 `M.HEAP32` を読む実装にした)。
- ブラウザーの境界コストは測定したが ([results/browser-boundary.jsonl](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/browser-boundary.jsonl)) 本文の表は Node の値のみ。

## Q7. 生成物に実際に含まれる Wasm 機能

### Takeaway
すべての `.wasm` は Node 24 で `WebAssembly.validate` が真だった。Mono/CoreCLR/NetWasm は `try_table` (exnref 系 EH)、NativeAOT-LLVM は従来の `try`/`catch` を使い、全系統で relaxed SIMD・tail call・GC 命令・atomics・shared memory・memory64 は 0 だった。SIMD128 は全 Wasm で存在するが、命令数と実行時の `PackedSimd.IsSupported` の値は別物である。

### Cited Findings
独自デコーダ [scripts/wasm-audit.mjs](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/scripts/wasm-audit.mjs) でセクションとコードセクション全命令を走査した (関数本体の末尾に一致しない場合は例外で停止するので、集計は全関数を完走した結果である)。

**Wasm 機能監査 (主モジュールのコードセクションを独自デコーダで全数走査した命令数)**

| ツールチェーン | モジュール | validate | 関数数 | 全命令数 | SIMD128 | relaxed | legacy try | try_table | throw_ref | tail call | GC 命令 | GC 型 | bulk | atomics | shared mem | memory64 | multi-value (型/ブロック) | ref 命令 | import/export 関数 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| mono-interp | dotnet.native.wasm | OK | 3323 | 473863 | 950 | 0 | 0 | 6 | 1 | 0 | 0 | 0 | 394 | 0 | no | no | 1/1 | 0 | 53/189 |
| mono-aot | dotnet.native.wasm | OK | 17283 | 1704413 | 32963 | 0 | 0 | 351 | 65 | 0 | 0 | 0 | 774 | 0 | no | no | 1/1 | 0 | 53/193 |
| coreclr-interp | dotnet.native.wasm | OK | 9647 | 1753073 | 6067 | 0 | 0 | 10587 | 6612 | 0 | 0 | 0 | 1632 | 0 | no | no | 1/890 | 0 | 72/33 |
| naot-llvm | audit.wasm | OK | 3551 | 405530 | 476 | 0 | 493 | 0 | 0 | 0 | 0 | 0 | 89 | 0 | no | no | 0/0 | 0 | 15/14 |
| naot-llvm-speed | audit.wasm | OK | 1831 | 359191 | 1857 | 0 | 455 | 0 | 0 | 0 | 0 | 0 | 176 | 0 | no | no | 0/0 | 0 | 15/11 |
| netwasm-o3 | audit.wasm | OK | 247 | 166016 | 1765 | 0 | 0 | 36 | 0 | 0 | 0 | 0 | 47 | 0 | no | no | 0/0 | 0 | 13/106 |
| netwasm-o3-ignore | audit.wasm | OK | 260 | 147442 | 1765 | 0 | 0 | 36 | 0 | 0 | 0 | 0 | 47 | 0 | no | no | 0/0 | 0 | 13/106 |
| mono-interp-default | dotnet.native.wasm | OK | 3494 | 481771 | 947 | 0 | 0 | 7 | 1 | 0 | 0 | 0 | 387 | 0 | no | no | 1/1 | 0 | 53/189 |
| mono-aot-default | dotnet.native.wasm | OK | 17463 | 1670976 | 32034 | 0 | 0 | 352 | 65 | 0 | 0 | 0 | 766 | 0 | no | no | 1/1 | 0 | 53/193 |
| naot-llvm-1mod | audit.wasm | OK | 1628 | 399314 | 1851 | 0 | 513 | 0 | 0 | 0 | 0 | 0 | 183 | 0 | no | no | 0/0 | 0 | 15/11 |
| naot-llvm-size | audit.wasm | OK | 3627 | 383741 | 476 | 0 | 490 | 0 | 0 | 0 | 0 | 0 | 89 | 0 | no | no | 0/0 | 0 | 15/14 |
| naot-llvm-jsapi | dotnet.native.wasm | OK | 6048 | 626309 | 476 | 0 | 899 | 0 | 0 | 0 | 0 | 0 | 89 | 0 | no | no | 0/0 | 0 | 19/37 |
| netwasm-o3-converge | audit.wasm | OK | 247 | 165221 | 1765 | 0 | 0 | 36 | 0 | 0 | 0 | 0 | 47 | 0 | no | no | 0/0 | 0 | 13/106 |
| netwasm-o3-compact | audit.wasm | OK | 240 | 160361 | 1735 | 0 | 0 | 36 | 0 | 0 | 0 | 0 | 50 | 0 | no | no | 0/0 | 0 | 13/106 |
| netwasm-o3-bump | audit.wasm | OK | 217 | 155381 | 1735 | 0 | 0 | 36 | 0 | 0 | 0 | 0 | 41 | 0 | no | no | 0/0 | 0 | 13/106 |
| netwasm-oz | audit.wasm | OK | 312 | 100400 | 844 | 0 | 0 | 35 | 0 | 0 | 0 | 0 | 450 | 0 | no | no | 0/0 | 0 | 13/106 |
| netwasm-0.13-o3 | audit.wasm | OK | 224 | 158143 | 1753 | 0 | 0 | 30 | 0 | 0 | 0 | 0 | 46 | 0 | no | no | 0/0 | 0 | 12/100 |

**target_features カスタムセクション**

| ツールチェーン | target_features |
| --- | --- |
| mono-interp | なし (除去済み) |
| mono-aot | なし (除去済み) |
| coreclr-interp | なし (除去済み) |
| naot-llvm | +bulk-memory +exception-handling +multivalue +mutable-globals +nontrapping-fptoint +reference-types +sign-ext +simd128 |
| naot-llvm-speed | なし (除去済み) |
| netwasm-o3 | なし (除去済み) |
| netwasm-o3-ignore | なし (除去済み) |
| mono-interp-default | なし (除去済み) |
| mono-aot-default | なし (除去済み) |
| naot-llvm-1mod | なし (除去済み) |
| naot-llvm-size | +bulk-memory +exception-handling +multivalue +mutable-globals +nontrapping-fptoint +reference-types +sign-ext +simd128 |
| naot-llvm-jsapi | +bulk-memory +exception-handling +multivalue +mutable-globals +nontrapping-fptoint +reference-types +sign-ext +simd128 |
| netwasm-o3-converge | なし (除去済み) |
| netwasm-o3-compact | なし (除去済み) |
| netwasm-o3-bump | なし (除去済み) |
| netwasm-oz | なし (除去済み) |
| netwasm-0.13-o3 | なし (除去済み) |

**実行時に報告された機能 (Vector128 / PackedSimd など)**

| ツールチェーン | Vector128.IsHardwareAccelerated | PackedSimd.IsSupported | Vector.IsHardwareAccelerated | Vector256.IsHardwareAccelerated | Vector512.IsHardwareAccelerated | Avx512F.IsSupported | RuntimeFeature.IsDynamicCodeSupported | RuntimeFeature.IsDynamicCodeCompiled |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| native-jit | true | false | true | true | true | true | true | true |
| native-aot | true | false | true | true | true | true | false | false |
| mono-interp | true | true | true | false | false | false | true | false |
| mono-aot | true | true | true | false | false | false | true | false |
| coreclr-interp | false | false | false | false | false | false | true | false |
| naot-llvm | false | false | false | false | false | false | false | false |
| naot-llvm-speed | false | false | false | false | false | false | false | false |
| netwasm-o3 | true | true | true | false | false | false | false | false |
| netwasm-o3-ignore | true | true | true | false | false | false | false | false |

- `dotnet.js` 系の `audit.wasm` などの webcil はコード 2 関数のみのラッパーで機能監査の対象ではない。全モジュールの結果は [results/audit.json](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/audit.json)。
- `Vector128.IsHardwareAccelerated` と `PackedSimd.IsSupported` は `mono-*` と `netwasm-*` で true、`coreclr-interp` と `naot-llvm*` で false。`naot-llvm` には SIMD128 命令 476 個があるが、C# の `Vector128` 経路が有効という意味ではない (ランタイムや libc 側のコードを含む)。`naot-llvm-speed` は `-msimd128` により 1,857 個に増えた。
- Node 24 (V8 13.6) は `try_table` を含むモジュールをフラグなしで検証・実行できた。

### Inferences
- NativeAOT-LLVM は従来型 EH のみで、exnref 前提のホストが無くても動く。
- GC 命令が 0 なので、どの構成も Wasm GC を使わず、自前のメモリ管理を線形メモリ上で行っている。

### Gaps
- ブラウザーごとの機能対応表 (Safari など) との突き合わせは未実施。
- 命令数は静的な数であり、実行頻度は別問題である。

## Q8. 再現手順、計測条件、結果の限界

### Takeaway
すべての測定は作業領域 `C:\Users\guitarrapc\AppData\Local\Temp\claude\8ffd9513d941f0a1\4ac9fb56-b669-4e11-a61a-d1ccca49da70\scratchpad\wasm-audit` のスクリプトで再実行できる。計測は論理 CPU 0〜15 に固定し、1 プロセスずつ直列に実行した。結果はこのマシンの x64 V8 (Node 24 と Edge 157) に限られる。CPU 固定後もラウンド間のばらつきは大きく (セルごとの (最大−最小)/中央値 の中央値 23%)、単一セルの 25% 未満の差は順位の根拠にならない。

### Cited Findings
**環境**: Windows 11 Pro 10.0.26200、AMD Ryzen 9 7950X3D、.NET SDK 11.0.100 (workload 11.0.100.1, `wasm-tools`, `wasm-tools-net10`)、Node v24.18.0 (V8 13.6.233.17-node.50)、Edge 157 (`Edg/157.0.0.0`)、Emscripten 3.1.56 / 6.0.3 (workload パック)。— [results/env.txt](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/env.txt)

**実施の経緯 (結果の扱いに影響する事項)**
1. 最初の計測 (3 ラウンド) は CPU 固定なしで行い、セルごとのラウンド間差 ((最大−最小)/中央値) の中央値が 24.8% だった。CCD 間の移動を疑って論理 CPU 0〜15 に固定し 5 ラウンドで取り直したが、差の中央値は 23.2% でほとんど縮まらなかった。したがって CPU 固定はばらつきの解消策としては効いていない。本文の数値は固定後の 5 ラウンドの結果で、固定前の結果は [results-run1-unpinned](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results-run1-unpinned) に保存した (本文では不使用)。
2. 2 回目の開始直後、停止したはずの 1 回目のスクリプトが生き残り、ブラウザー計測が重なった。重なった `caps/startup/boundary/cold` は捨てて再計測し、ブラウザー結果も分離した ([results-contaminated](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results-contaminated))。速度の主測定はこの重なりの後に開始している。
3. 実行中に別セッションがリポジトリの `QuadSort.cs`, `FluxSort.cs`, `SortPhase.cs` (SortPhase はコメントのみ) を 03:57 JST に変更した。主測定に使った `out/` は全てそれ以前 (03:18〜03:38 JST) のビルドで、HEAD `f674c43` のソースと一致する。変更後にビルドした `naot-llvm-size`, `naot-llvm-jsapi`, probe も、計測対象の算法のソースは同一である。

**ノイズ対策**: `benchmark-noise-protocol` に従い、同一実行内の JS アンカー、ウォームアップ、シード別入力、中央値を用いた。`native-jit` は階層化の混入を避けるため `DOTNET_TieredCompilation=0`。各プロセスの親 (`orchestrate.mjs`) の affinity を `0xFFFF` に設定し、子プロセスへ継承させた。ビルドと計測は同時に実行していない。

**結果ファイル**: 生データ [results/speed.jsonl](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/speed.jsonl), [results/speed-reduced.jsonl](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/speed-reduced.jsonl), [results/browser-speed.jsonl](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/browser-speed.jsonl), [results/cold.jsonl](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/cold.jsonl), [results/startup.jsonl](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/startup.jsonl), [results/boundary.jsonl](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/boundary.jsonl), [results/caps.jsonl](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/caps.jsonl); 集計 [results/summary-speed.csv](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/summary-speed.csv), [results/summary-speed-reduced.csv](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/summary-speed-reduced.csv), [results/summary-speed-edge.csv](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/summary-speed-edge.csv), [results/summary-speed.json](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/summary-speed.json), [results/summary-startup.json](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/summary-startup.json), [results/summary-cold.json](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/summary-cold.json), [results/summary-boundary.json](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/summary-boundary.json), [results/summary-caps.json](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/summary-caps.json), [results/size.json](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/size.json), [results/audit.json](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/audit.json), 本文の表 [results/report-tables.md](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/report-tables.md)。

**再実行コマンド** (Git Bash、`W` は作業領域)

```bash
cd "$W"
bash scripts/build-rest.sh                 # NetWasm と NativeAOT-LLVM の派生 (build.sh を呼ぶ)
bash scripts/run-all.sh                    # static-report → caps,startup,boundary,cold,speed,speed-coreclr,speed-secondary → Edge
bash scripts/run-after.sh                  # caps,startup,boundary,cold の再計測とクリーンビルド時間
bash scripts/run-extra.sh                  # interop probe、naot-llvm-size、naot-llvm-jsapi
node scripts/static-report.mjs && node scripts/analyze.mjs && node scripts/report.mjs
```

**実行したビルドコマンド** (各ログ先頭の `CMD:` 行。`$W` は作業領域)

```bash
# native-jit
cd tc/native; NUGET_PACKAGES=pkgs/native-jit dotnet publish -c Release -o $W/out-clean/native-jit --artifacts-path $W/art-clean/native-jit
# native-aot
cd tc/native; NUGET_PACKAGES=pkgs/native-aot dotnet publish -c Release -o $W/out-clean/native-aot --artifacts-path $W/art-clean/native-aot -p:PublishAot=true -p:SelfContained=true
# mono-interp
cd tc/mono; NUGET_PACKAGES=pkgs/mono dotnet publish -c Release -o $W/out-clean/mono-interp --artifacts-path $W/art-clean/mono-interp
# mono-interp-default
cd tc/mono; NUGET_PACKAGES=pkgs/mono dotnet publish -c Release -o $W/out-clean/mono-interp-default --artifacts-path $W/art-clean/mono-interp-default -p:AuditDefaultOpt=true
# mono-aot
cd tc/mono; NUGET_PACKAGES=pkgs/mono dotnet publish -c Release -o $W/out-clean/mono-aot --artifacts-path $W/art-clean/mono-aot -p:RunAOTCompilation=true
# mono-aot-default
cd tc/mono; NUGET_PACKAGES=pkgs/mono dotnet publish -c Release -o $W/out-clean/mono-aot-default --artifacts-path $W/art-clean/mono-aot-default -p:RunAOTCompilation=true -p:AuditDefaultOpt=true
# coreclr-interp
cd tc/coreclr; NUGET_PACKAGES=pkgs/coreclr dotnet publish -c Release -o $W/out-clean/coreclr-interp --artifacts-path $W/art-clean/coreclr-interp -p:WasmBuildNative=false
# naot-llvm
cd tc/naot-llvm; NUGET_PACKAGES=pkgs/naot-llvm dotnet publish -c Release -o $W/out-clean/naot-llvm --artifacts-path $W/art-clean/naot-llvm -r browser-wasm -p:EmscriptenSdkToolsPath=C:\Program Files\dotnet\packs\Microsoft.NET.Runtime.Emscripten.3.1.56.Sdk.win-x64\10.0.15\tools\
# naot-llvm-speed
cd tc/naot-llvm; NUGET_PACKAGES=pkgs/naot-llvm dotnet publish -c Release -o $W/out-clean/naot-llvm-speed --artifacts-path $W/art-clean/naot-llvm-speed -r browser-wasm -p:EmscriptenSdkToolsPath=C:\Program Files\dotnet\packs\Microsoft.NET.Runtime.Emscripten.3.1.56.Sdk.win-x64\10.0.15\tools\ -p:AuditSimd=true
# naot-llvm-1mod
cd tc/naot-llvm; NUGET_PACKAGES=pkgs/naot-llvm dotnet publish -c Release -o $W/out-clean/naot-llvm-1mod --artifacts-path $W/art-clean/naot-llvm-1mod -r browser-wasm -p:EmscriptenSdkToolsPath=C:\Program Files\dotnet\packs\Microsoft.NET.Runtime.Emscripten.3.1.56.Sdk.win-x64\10.0.15\tools\ -p:AuditSimd=true -p:IlcMaxLlvmModuleCount=1
# naot-llvm-size
cd tc/naot-llvm; NUGET_PACKAGES=pkgs/naot-llvm dotnet publish -c Release -o $W/out/naot-llvm-size --artifacts-path $W/art/naot-llvm-size -r browser-wasm -p:EmscriptenSdkToolsPath=C:\Program Files\dotnet\packs\Microsoft.NET.Runtime.Emscripten.3.1.56.Sdk.win-x64\10.0.15\tools\ -p:OptimizationPreference=Size
# naot-llvm-jsapi
cd tc/naot-llvm; NUGET_PACKAGES=pkgs/naot-llvm dotnet publish -c Release -o $W/out/naot-llvm-jsapi --artifacts-path $W/art/naot-llvm-jsapi -r browser-wasm -p:EmscriptenSdkToolsPath=C:\Program Files\dotnet\packs\Microsoft.NET.Runtime.Emscripten.3.1.56.Sdk.win-x64\10.0.15\tools\ -p:DotNetJsApi=true
# netwasm-0.14.2-o3
cd tc/netwasm; NUGET_PACKAGES=pkgs/netwasm-0.14.2 dotnet publish -c Release -o $W/out-clean/netwasm-0.14.2-o3 --artifacts-path $W/art-clean/netwasm-0.14.2-o3
# netwasm-0.14.2-o3-ignore
cd tc/netwasm; NUGET_PACKAGES=pkgs/netwasm-0.14.2 dotnet publish -c Release -o $W/out-clean/netwasm-0.14.2-o3-ignore --artifacts-path $W/art-clean/netwasm-0.14.2-o3-ignore -p:NetWasmManagedInliningHints=IgnoreAggressive
# netwasm-0.14.2-o3-converge
cd tc/netwasm; NUGET_PACKAGES=pkgs/netwasm-0.14.2 dotnet publish -c Release -o $W/out/netwasm-0.14.2-o3-converge --artifacts-path $W/art/netwasm-0.14.2-o3-converge -p:NetWasmOptimizerConverge=true
# netwasm-0.14.2-o3-compact
cd tc/netwasm; NUGET_PACKAGES=pkgs/netwasm-0.14.2 dotnet publish -c Release -o $W/out/netwasm-0.14.2-o3-compact --artifacts-path $W/art/netwasm-0.14.2-o3-compact -p:NetWasmGarbageCollector=Compact
# netwasm-0.14.2-o3-bump
cd tc/netwasm; NUGET_PACKAGES=pkgs/netwasm-0.14.2 dotnet publish -c Release -o $W/out/netwasm-0.14.2-o3-bump --artifacts-path $W/art/netwasm-0.14.2-o3-bump -p:NetWasmGarbageCollector=Bump
# netwasm-0.14.2-oz
cd tc/netwasm; NUGET_PACKAGES=pkgs/netwasm-0.14.2 dotnet publish -c Release -o $W/out-clean/netwasm-0.14.2-oz --artifacts-path $W/art-clean/netwasm-0.14.2-oz -p:NetWasmOptimization=Oz -p:AuditDefaultGc=true
# netwasm-0.13.0-o3
cd tc/netwasm-0.13.0; NUGET_PACKAGES=pkgs/netwasm-0.13.0 dotnet publish -c Release -o $W/out-clean/netwasm-0.13.0-o3 --artifacts-path $W/art-clean/netwasm-0.13.0-o3
# probe-mono
cd tc/probe; NUGET_PACKAGES=pkgs/mono dotnet publish -c Release -o $W/out/probe-mono --artifacts-path $W/art/probe-mono
# probe-coreclr
cd tc/probe; NUGET_PACKAGES=pkgs/coreclr dotnet publish -c Release -o $W/out/probe-coreclr --artifacts-path $W/art/probe-coreclr -p:AuditCoreClr=true
# probe-netwasm
cd tc/probe-netwasm; NUGET_PACKAGES=pkgs/netwasm-0.14.2 dotnet publish -c Release -o $W/out/probe-netwasm --artifacts-path $W/art/probe-netwasm
```

**NativeAOT-LLVM の環境変数** (プロセス内のみ。ユーザー/マシン設定は変更していない)

`scripts/emenv.sh`

```bash
# Process-local environment that reuses the Emscripten 3.1.56 packs of the installed wasm-tools-net10 workload.
# The packs stay untouched: the cache is a writable copy under emsdk-work, the config file lives there too.
W='C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit'
PK='C:/Program Files/dotnet/packs'
V=10.0.15
export EMSDK_TOOLS="$PK/Microsoft.NET.Runtime.Emscripten.3.1.56.Sdk.win-x64/$V/tools"
export EMSDK_PYTHON="$PK/Microsoft.NET.Runtime.Emscripten.3.1.56.Python.win-x64/$V/tools/python.exe"
EM_NODE="$PK/Microsoft.NET.Runtime.Emscripten.3.1.56.Node.win-x64/$V/tools/bin/node.exe"
export EM_CACHE="$W/emsdk-work/cache"
export EM_CONFIG="$W/emsdk-work/emscripten-config.py"
cat > "$EM_CONFIG" <<EOF
LLVM_ROOT = r'$EMSDK_TOOLS/bin'
BINARYEN_ROOT = r'$EMSDK_TOOLS'
NODE_JS = r'$EM_NODE'
COMPILER_ENGINE = NODE_JS
JS_ENGINES = [NODE_JS]
EOF
export PATH="$EMSDK_TOOLS/bin:$EMSDK_TOOLS/emscripten:$(dirname "$EMSDK_PYTHON"):$PATH"
export EMSCRIPTEN_TOOLS_WIN="$(cygpath -w "$EMSDK_TOOLS")\\"
```

**プロジェクトファイル全文**

`NuGet.config`

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="dotnet-experimental" value="https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-experimental/nuget/v3/index.json" />
  </packageSources>
  <packageSourceMapping>
    <packageSource key="dotnet-experimental">
      <package pattern="Microsoft.DotNet.ILCompiler.LLVM" />
      <package pattern="runtime.win-x64.Microsoft.DotNet.ILCompiler.LLVM" />
      <package pattern="runtime.browser-wasm.Microsoft.DotNet.ILCompiler.LLVM" />
      <package pattern="runtime.wasi-wasm.Microsoft.DotNet.ILCompiler.LLVM" />
    </packageSource>
    <packageSource key="nuget.org">
      <package pattern="*" />
    </packageSource>
  </packageSourceMapping>
</configuration>
```

`shared/Library.props`

```xml
<Project>
  <PropertyGroup>
    <AuditRepo>D:\github\guitarrapc\SortAlgorithms\src\SortAlgorithm\</AuditRepo>
    <AuditShared>$(MSBuildThisFileDirectory)</AuditShared>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
    <NoWarn>$(NoWarn);CA1416;CS8002</NoWarn>
    <InvariantGlobalization>true</InvariantGlobalization>
    <SatelliteResourceLanguages>en</SatelliteResourceLanguages>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="$(AuditRepo)**\*.cs" Exclude="$(AuditRepo)obj\**;$(AuditRepo)bin\**" LinkBase="lib" />
    <Compile Include="$(AuditShared)Kernel.cs" Link="Kernel.cs" />
  </ItemGroup>
</Project>
```

`tc/native/native.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <Import Project="..\..\shared\Library.props" />
  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <OutputType>Exe</OutputType>
    <AssemblyName>native</AssemblyName>
    <RuntimeIdentifier>win-x64</RuntimeIdentifier>
    <SelfContained>false</SelfContained>
    <Optimize>true</Optimize>
    <StartupObject>WasmAudit.NativeServer</StartupObject>
    <!-- Both publish flavours come from this project: -p:PublishAot=true selects NativeAOT. -->
    <OptimizationPreference Condition="'$(PublishAot)' == 'true'">Speed</OptimizationPreference>
    <IlcInstructionSet Condition="'$(PublishAot)' == 'true'">native</IlcInstructionSet>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="..\..\shared\NativeServer.cs" Link="NativeServer.cs" />
  </ItemGroup>
</Project>
```

`tc/mono/mono.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk.WebAssembly">
  <Import Project="..\..\shared\Library.props" />
  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <OutputType>Exe</OutputType>
    <AssemblyName>audit</AssemblyName>
    <StartupObject>WasmAudit.Program</StartupObject>

    <!-- Common to both Mono flavours: full trimming, no diagnostics, no ICU. -->
    <PublishTrimmed>true</PublishTrimmed>
    <TrimMode>full</TrimMode>
    <InvariantTimezone>true</InvariantTimezone>
    <DebuggerSupport>false</DebuggerSupport>
    <EventSourceSupport>false</EventSourceSupport>
    <MetricsSupport>false</MetricsSupport>
    <UseSystemResourceKeys>true</UseSystemResourceKeys>
    <CompressionEnabled>false</CompressionEnabled>
    <WasmEmitSourceMap>false</WasmEmitSourceMap>
    <WasmEmitSymbolMap>false</WasmEmitSymbolMap>
    <WasmNativeDebugSymbols>false</WasmNativeDebugSymbols>
    <WasmDebugLevel>0</WasmDebugLevel>

    <!-- Speed-priority feature selection (SDK defaults, stated explicitly). -->
    <WasmEnableSIMD>true</WasmEnableSIMD>
    <WasmEnableExceptionHandling>true</WasmEnableExceptionHandling>

    <WasmBuildNative>true</WasmBuildNative>
    <WasmInitialHeapSize>134217728</WasmInitialHeapSize>
  </PropertyGroup>

  <!-- AuditDefaultOpt=true keeps the SDK's Release defaults (compile -Oz, link -O2, bitcode -O2) for comparison. -->
  <PropertyGroup Condition="'$(AuditDefaultOpt)' != 'true'">
    <EmccCompileOptimizationFlag>-O3</EmccCompileOptimizationFlag>
    <EmccLinkOptimizationFlag>-O3</EmccLinkOptimizationFlag>
  </PropertyGroup>

  <PropertyGroup Condition="'$(RunAOTCompilation)' == 'true'">
    <WasmBitcodeCompileOptimizationFlag Condition="'$(AuditDefaultOpt)' != 'true'">-O3</WasmBitcodeCompileOptimizationFlag>
    <WasmStripILAfterAOT>true</WasmStripILAfterAOT>
  </PropertyGroup>

  <ItemGroup>
    <Compile Include="..\..\shared\JsExports.cs" Link="JsExports.cs" />
    <Compile Include="..\..\shared\JsEntry.cs" Link="JsEntry.cs" />
  </ItemGroup>
</Project>
```

`tc/coreclr/coreclr.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk.WebAssembly">
  <Import Project="..\..\shared\Library.props" />
  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <OutputType>Exe</OutputType>
    <AssemblyName>audit</AssemblyName>
    <StartupObject>WasmAudit.Program</StartupObject>

    <!-- Selects the CoreCLR browser-wasm runtime pack instead of Mono. -->
    <UseMonoRuntime>false</UseMonoRuntime>

    <PublishTrimmed>true</PublishTrimmed>
    <TrimMode>full</TrimMode>
    <InvariantTimezone>true</InvariantTimezone>
    <DebuggerSupport>false</DebuggerSupport>
    <EventSourceSupport>false</EventSourceSupport>
    <MetricsSupport>false</MetricsSupport>
    <UseSystemResourceKeys>true</UseSystemResourceKeys>
    <CompressionEnabled>false</CompressionEnabled>
    <WasmEmitSourceMap>false</WasmEmitSourceMap>
    <WasmNativeDebugSymbols>false</WasmNativeDebugSymbols>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="..\..\shared\JsExports.cs" Link="JsExports.cs" />
    <Compile Include="..\..\shared\JsEntry.cs" Link="JsEntry.cs" />
  </ItemGroup>
</Project>
```

`tc/naot-llvm/naot-llvm.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <Import Project="..\..\shared\Library.props" />
  <PropertyGroup>
    <AuditIlcVer Condition="'$(AuditIlcVer)' == ''">10.0.0-rc.1.26357.1</AuditIlcVer>
    <AuditTfm Condition="'$(AuditTfm)' == ''">net10.0</AuditTfm>
    <TargetFramework>$(AuditTfm)</TargetFramework>
    <OutputType>Library</OutputType>
    <AssemblyName>audit</AssemblyName>
    <LangVersion>preview</LangVersion>

    <!-- Required by docs/using-nativeaot/compiling.md (feature/NativeAOT-LLVM). -->
    <PublishTrimmed>true</PublishTrimmed>
    <SelfContained>true</SelfContained>
    <MSBuildEnableWorkloadResolver>false</MSBuildEnableWorkloadResolver>

    <!-- AuditSpeedPref=true adds ILC's Dash-Ot. It crashes ILC unless JIT PGO queries are disabled (see notes). -->
    <OptimizationPreference Condition="'$(AuditSpeedPref)' == 'true'">Speed</OptimizationPreference>

    <DebugType>none</DebugType>
    <DebuggerSupport>false</DebuggerSupport>
    <EventSourceSupport>false</EventSourceSupport>
    <UseSystemResourceKeys>true</UseSystemResourceKeys>
    <StackTraceSupport>false</StackTraceSupport>
    <InvariantTimezone>true</InvariantTimezone>
  </PropertyGroup>

  <!-- Not documented: passes -msimd128 to the bitcode-to-object clang step and runs wasm-opt -O3 at link. -->
  <PropertyGroup Condition="'$(AuditSimd)' == 'true'">
    <CppCompiler>&quot;$(EmscriptenSdkToolsPath)bin\clang&quot; -msimd128</CppCompiler>
    <EmccFlags>-O3 -msimd128</EmccFlags>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.DotNet.ILCompiler.LLVM" Version="$(AuditIlcVer)" />
    <PackageReference Include="runtime.win-x64.Microsoft.DotNet.ILCompiler.LLVM" Version="$(AuditIlcVer)" />
    <Compile Include="..\..\shared\LlvmExports.cs" Link="LlvmExports.cs" />
    <Compile Condition="'$(DotNetJsApi)' == 'true'" Include="..\..\shared\JsExports.cs" Link="JsExports.cs" />
    <IlcArg Condition="'$(AuditJitNoPgo)' == 'true'" Include="--codegenopt:JitDisablePGO=1" />
    <LinkerArg Condition="'$(DotNetJsApi)' != 'true'" Include="-sMODULARIZE=1" />
    <LinkerArg Condition="'$(DotNetJsApi)' != 'true'" Include="-sEXPORT_NAME=createAudit" />
    <LinkerArg Condition="'$(DotNetJsApi)' != 'true'" Include="-sENVIRONMENT=node,web" />
    <LinkerArg Condition="'$(DotNetJsApi)' != 'true'" Include="-sEXPORTED_RUNTIME_METHODS=HEAP32,HEAPU8" />
    <LinkerArg Include="-sINITIAL_MEMORY=134217728" />
  </ItemGroup>
</Project>
```

`tc/netwasm/netwasm.csproj`

```xml
<Project Sdk="NetWasm.Sdk/0.14.2">
  <PropertyGroup>
    <TargetFramework>netwasm0.1</TargetFramework>
    <OutputType>JsModule</OutputType>
    <AssemblyName>audit</AssemblyName>
    <NetWasmJavaScriptHosts>node;page</NetWasmJavaScriptHosts>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <NoWarn>$(NoWarn);CA1416;CS8002</NoWarn>
    <NetWasmOptimization Condition="'$(NetWasmOptimization)' == ''">O3</NetWasmOptimization>
    <NetWasmGarbageCollector Condition="'$(NetWasmGarbageCollector)' == '' and '$(AuditDefaultGc)' != 'true'">NetWasm</NetWasmGarbageCollector>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="D:\github\guitarrapc\SortAlgorithms\src\SortAlgorithm\**\*.cs" Exclude="D:\github\guitarrapc\SortAlgorithms\src\SortAlgorithm\obj\**;D:\github\guitarrapc\SortAlgorithms\src\SortAlgorithm\bin\**" LinkBase="lib" />
    <Compile Include="..\..\shared\Kernel.cs" Link="Kernel.cs" />
    <Compile Include="..\..\shared\JsExports.cs" Link="JsExports.cs" />
  </ItemGroup>
</Project>
```

`tc/netwasm-0.13.0/netwasm.csproj`

```xml
<Project Sdk="NetWasm.Sdk/0.13.0">
  <PropertyGroup>
    <TargetFramework>netwasm0.1</TargetFramework>
    <OutputType>JsModule</OutputType>
    <AssemblyName>audit</AssemblyName>
    <NetWasmJavaScriptHosts>node;page</NetWasmJavaScriptHosts>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <NoWarn>$(NoWarn);CA1416;CS8002</NoWarn>
    <NetWasmOptimization Condition="'$(NetWasmOptimization)' == ''">O3</NetWasmOptimization>
    <NetWasmGarbageCollector Condition="'$(NetWasmGarbageCollector)' == '' and '$(AuditDefaultGc)' != 'true'">NetWasm</NetWasmGarbageCollector>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="D:\github\guitarrapc\SortAlgorithms\src\SortAlgorithm\**\*.cs" Exclude="D:\github\guitarrapc\SortAlgorithms\src\SortAlgorithm\obj\**;D:\github\guitarrapc\SortAlgorithms\src\SortAlgorithm\bin\**;D:\github\guitarrapc\SortAlgorithms\src\SortAlgorithm\Utils\ArrayPatterns.cs" LinkBase="lib" />
    <Compile Include="..\..\shared\Kernel.cs" Link="Kernel.cs" />
    <Compile Include="..\..\shared\JsExports.cs" Link="JsExports.cs" />
  </ItemGroup>
</Project>
```

**ワークロードと export の全文**

`shared/Kernel.cs`

```csharp
// Shared workload kernel. Compiled by every toolchain from the same source file.
// All entry points use scalar arguments so that every host can call them the same way.
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace WasmAudit;

public static unsafe class Kernel
{
    public const int AlgoPdq = 0, AlgoTim = 1, AlgoDrift = 2, AlgoHeap = 3, AlgoShell = 4, AlgoRadix = 5, AlgoPlainQuick = 6, AlgoFlash = 7, AlgoBucket = 8;
    public const int PatRandom = 0, PatSorted = 1, PatReversed = 2, PatDup = 3;

    static int* buf;
    static int capElems;
    static int curN, curBatch;
    static uint sumA, sumB;
    static double cmp, swp, rd, wr;

    public static int Noop() => 0;
    public static int Echo(int x) => x + 1;

    // Runtime-reported capabilities, as a bit mask (decoded by the driver).
    public static int Caps()
    {
        var c = 0;
        if (System.Runtime.Intrinsics.Vector128.IsHardwareAccelerated) c |= 1;
#if !NO_PACKEDSIMD
        if (System.Runtime.Intrinsics.Wasm.PackedSimd.IsSupported) c |= 2;
#endif
        if (System.Numerics.Vector.IsHardwareAccelerated) c |= 4;
        if (System.Runtime.Intrinsics.Vector256.IsHardwareAccelerated) c |= 8;
        if (System.Runtime.Intrinsics.Vector512.IsHardwareAccelerated) c |= 16;
        if (System.Runtime.Intrinsics.X86.Avx512F.IsSupported) c |= 32;
        if (RuntimeFeature.IsDynamicCodeSupported) c |= 64;
        if (RuntimeFeature.IsDynamicCodeCompiled) c |= 128;
        return c;
    }

    static void Ensure(int elems)
    {
        if (elems <= capElems) return;
        if (buf != null) NativeMemory.AlignedFree(buf);
        buf = (int*)NativeMemory.AlignedAlloc((nuint)elems * sizeof(int), 64);
        capElems = elems;
    }

    public static int BufPtr(int elems) { Ensure(elems); return (int)(nint)buf; }
    public static int* BufRaw(int elems) { Ensure(elems); return buf; }

    // O(1) touch used by the JS-boundary tests: reads both ends and swaps them.
    public static int TouchBuf(int n)
    {
        var a = buf[0]; var b = buf[n - 1];
        buf[0] = b; buf[n - 1] = a;
        return a ^ b ^ n;
    }

    public static int TouchSpan(Span<int> s)
    {
        var a = s[0]; var b = s[^1];
        s[0] = b; s[^1] = a;
        return a ^ b ^ s.Length;
    }

    static uint Mix(uint x) => unchecked((x ^ (x >> 15)) * 0x9E3779B1u);

    // xorshift32; the JS driver implements exactly the same sequence.
    public static int Prepare(int pattern, int n, int batch, int seed)
    {
        Ensure(checked(n * batch));
        curN = n; curBatch = batch; sumA = 0; sumB = 0; cmp = swp = rd = wr = 0;
        var p = buf;
        for (var b = 0; b < batch; b++)
        {
            var s = unchecked((uint)(seed + b) * 2654435761u + 1u);
            if (s == 0) s = 1;
            for (var i = 0; i < n; i++)
            {
                s ^= s << 13; s ^= s >> 17; s ^= s << 5;
                var v = pattern switch
                {
                    PatRandom => (int)s,
                    PatSorted => i,
                    PatReversed => n - 1 - i,
                    _ => (int)(s % 40u),
                };
                p[i] = v;
                sumA = unchecked(sumA + (uint)v);
                sumB = unchecked(sumB + Mix((uint)v));
            }
            p += n;
        }
        return n * batch;
    }

    public static int Run(int algo, int ctx)
    {
        var p = buf;
        for (var b = 0; b < curBatch; b++)
        {
            var s = new Span<int>(p, curN);
            if (ctx == 0) SortFast(algo, s, p, curN);
            else SortObserved(algo, ctx, s, p, curN);
            p += curN;
        }
        return 0;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    static void SortFast(int algo, Span<int> s, int* p, int n)
    {
        switch (algo)
        {
            case AlgoPdq: PDQSort.Sort(s); break;
            case AlgoTim: TimSort.Sort(s); break;
            case AlgoDrift: Driftsort.Sort(s); break;
            case AlgoHeap: HeapSort.Sort(s); break;
            case AlgoShell: ShellSortCiura2001.Sort(s); break;
            case AlgoRadix: RadixLSD256Sort.Sort(s); break;
            case AlgoPlainQuick: PlainQuick(p, 0, n - 1); break;
            case AlgoFlash: FlashSort.Sort(s); break;
            case AlgoBucket: BucketSortInteger.Sort(s); break;
            default: throw new ArgumentOutOfRangeException(nameof(algo));
        }
    }

    // ctx 1: TContext = StatisticsContext (sealed class). ctx 2: TContext = ISortContext (interface-typed).
    // Each call site names its type argument directly, so no extra generic layer is added here.
    [MethodImpl(MethodImplOptions.NoInlining)]
    static void SortObserved(int algo, int ctx, Span<int> s, int* p, int n)
    {
        var c = new StatisticsContext();
        if (ctx == 1)
        {
            switch (algo)
            {
                case AlgoPdq: PDQSort.Sort<int, StatisticsContext>(s, c); break;
                case AlgoTim: TimSort.Sort<int, StatisticsContext>(s, c); break;
                case AlgoDrift: Driftsort.Sort<int, StatisticsContext>(s, c); break;
                case AlgoHeap: HeapSort.Sort<int, StatisticsContext>(s, c); break;
                case AlgoShell: ShellSortCiura2001.Sort<int, StatisticsContext>(s, c); break;
                case AlgoRadix: RadixLSD256Sort.Sort<int, StatisticsContext>(s, c); break;
                case AlgoPlainQuick: PlainQuick(p, 0, n - 1); break;
                case AlgoFlash: FlashSort.Sort<int, StatisticsContext>(s, c); break;
                case AlgoBucket: BucketSortInteger.Sort<int, StatisticsContext>(s, c); break;
                default: throw new ArgumentOutOfRangeException(nameof(algo));
            }
        }
        else
        {
            ISortContext i = c;
            switch (algo)
            {
                case AlgoPdq: PDQSort.Sort<int, ISortContext>(s, i); break;
                case AlgoTim: TimSort.Sort<int, ISortContext>(s, i); break;
                case AlgoDrift: Driftsort.Sort<int, ISortContext>(s, i); break;
                case AlgoHeap: HeapSort.Sort<int, ISortContext>(s, i); break;
                case AlgoShell: ShellSortCiura2001.Sort<int, ISortContext>(s, i); break;
                case AlgoRadix: RadixLSD256Sort.Sort<int, ISortContext>(s, i); break;
                case AlgoPlainQuick: PlainQuick(p, 0, n - 1); break;
                case AlgoFlash: FlashSort.Sort<int, ISortContext>(s, i); break;
                case AlgoBucket: BucketSortInteger.Sort<int, ISortContext>(s, i); break;
                default: throw new ArgumentOutOfRangeException(nameof(algo));
            }
        }
        cmp += c.CompareCount; swp += c.SwapCount; rd += c.IndexReadCount; wr += c.IndexWriteCount;
    }

    public static double Stat(int i) => i switch { 0 => cmp, 1 => swp, 2 => rd, _ => wr };

    // 1 = sorted ascending and multiset checksums match the prepared input; negative = failure.
    public static int Verify()
    {
        uint a = 0, b = 0;
        var p = buf;
        for (var k = 0; k < curBatch; k++)
        {
            for (var i = 0; i < curN; i++)
            {
                var v = p[i];
                if (i > 0 && p[i - 1] > v) return -1;
                a = unchecked(a + (uint)v);
                b = unchecked(b + Mix((uint)v));
            }
            p += curN;
        }
        return a == sumA && b == sumB ? 1 : -2;
    }

    // FNV-1a over the int32 values of all arrays; the JS driver computes the same digest over its own sort.
    public static int Digest()
    {
        var h = 2166136261u;
        var total = curN * curBatch;
        for (var i = 0; i < total; i++) h = unchecked((h ^ (uint)buf[i]) * 16777619u);
        return (int)h;
    }

    // Same algorithm as plainQuick in scripts/driver-core.mjs (no library abstractions).
    static void PlainQuick(int* a, int lo, int hi)
    {
        while (hi - lo > 16)
        {
            var mid = lo + ((hi - lo) >> 1);
            int t;
            if (a[mid] < a[lo]) { t = a[mid]; a[mid] = a[lo]; a[lo] = t; }
            if (a[hi] < a[lo]) { t = a[hi]; a[hi] = a[lo]; a[lo] = t; }
            if (a[hi] < a[mid]) { t = a[hi]; a[hi] = a[mid]; a[mid] = t; }
            var pivot = a[mid];
            int i = lo, j = hi;
            while (i <= j)
            {
                while (a[i] < pivot) i++;
                while (a[j] > pivot) j--;
                if (i <= j) { t = a[i]; a[i] = a[j]; a[j] = t; i++; j--; }
            }
            if (j - lo < hi - i) { PlainQuick(a, lo, j); lo = i; }
            else { PlainQuick(a, i, hi); hi = j; }
        }
        for (var i = lo + 1; i <= hi; i++)
        {
            var x = a[i];
            var j = i - 1;
            while (j >= lo && a[j] > x) { a[j + 1] = a[j]; j--; }
            a[j + 1] = x;
        }
    }
}
```

`shared/JsExports.cs`

```csharp
// [JSExport]/[JSImport] surface shared by Mono, CoreCLR Wasm and NetWasm builds.
using System.Runtime.InteropServices.JavaScript;

namespace WasmAudit;

public static unsafe partial class JsApi
{
    [JSExport] public static int Caps() => Kernel.Caps();
    [JSExport] public static int Noop() => Kernel.Noop();
    [JSExport] public static int Echo(int x) => Kernel.Echo(x);
    [JSExport] public static int Prepare(int pattern, int n, int batch, int seed) => Kernel.Prepare(pattern, n, batch, seed);
    [JSExport] public static int Run(int algo, int ctx) => Kernel.Run(algo, ctx);
    [JSExport] public static int Verify() => Kernel.Verify();
    [JSExport] public static int Digest() => Kernel.Digest();
    [JSExport] public static double Stat(int i) => Kernel.Stat(i);

    // Mechanism: caller writes into an exported linear-memory buffer.
    [JSExport] public static int BufPtr(int n) => Kernel.BufPtr(n);
    [JSExport] public static int TouchBuf(int n) => Kernel.TouchBuf(n);

    // Mechanism: copying marshalling of int[] (one way, and both ways).
    [JSExport] public static int CopyIn(int[] a) => Kernel.TouchSpan(a);
    [JSExport] public static int[] CopyRoundtrip(int[] a) { Kernel.TouchSpan(a); return a; }

    // Mechanism: span view over linear memory handed to a JS import, which fills it in place.
#if NETWASM
    [JSImport("fill", "audit")] private static partial void JsFill(Span<int> s);
#else
    [JSImport("fill", "audit")] private static partial void JsFill([JSMarshalAs<JSType.MemoryView>] Span<int> s);
#endif
    [JSExport]
    public static int SpanImport(int n)
    {
        var s = new Span<int>(Kernel.BufRaw(n), n);
        JsFill(s);
        return Kernel.TouchSpan(s);
    }
}
```

`shared/LlvmExports.cs`

```csharp
// Direct Wasm exports for NativeAOT-LLVM. No marshalling layer: scalars and linear-memory addresses only.
using System.Runtime.InteropServices;

namespace WasmAudit;

public static unsafe class LlvmApi
{
    [UnmanagedCallersOnly(EntryPoint = "audit_caps")] public static int Caps() => Kernel.Caps();
    [UnmanagedCallersOnly(EntryPoint = "audit_noop")] public static int Noop() => Kernel.Noop();
    [UnmanagedCallersOnly(EntryPoint = "audit_echo")] public static int Echo(int x) => Kernel.Echo(x);
    [UnmanagedCallersOnly(EntryPoint = "audit_prepare")] public static int Prepare(int pattern, int n, int batch, int seed) => Kernel.Prepare(pattern, n, batch, seed);
    [UnmanagedCallersOnly(EntryPoint = "audit_run")] public static int Run(int algo, int ctx) => Kernel.Run(algo, ctx);
    [UnmanagedCallersOnly(EntryPoint = "audit_verify")] public static int Verify() => Kernel.Verify();
    [UnmanagedCallersOnly(EntryPoint = "audit_digest")] public static int Digest() => Kernel.Digest();
    [UnmanagedCallersOnly(EntryPoint = "audit_stat")] public static double Stat(int i) => Kernel.Stat(i);
    [UnmanagedCallersOnly(EntryPoint = "audit_bufptr")] public static int BufPtr(int n) => Kernel.BufPtr(n);
    [UnmanagedCallersOnly(EntryPoint = "audit_touch")] public static int TouchBuf(int n) => Kernel.TouchBuf(n);
}
```

**JS 側の呼び出し** (`scripts/adapters.mjs` から抜粋、全文は同ファイル)

```js
// dotnet.js 系
ex.CopyIn(src); ex.CopyRoundtrip(src);
const p = ex.BufPtr(n) >> 2, h = rt.localHeapViewI32(); h.set(src, p); ex.TouchBuf(n); h.slice(p, p + n);
// fill は [JSImport] で受けた MemoryView に src を書く
rt.setModuleImports('audit', { fill: (s) => { s.set(fillSrc); } }); ex.SpanImport(n);
// NetWasm
const app = await open({ modules: { audit: { fill: (s) => { captured = s.buffer; if (fillSrc) s.set(fillSrc); } } } });
// NativeAOT-LLVM
const p = M._audit_bufptr(n) >> 2; M.HEAP32.set(src, p); M._audit_touch(n); M.HEAP32.slice(p, p + n);
```

### Inferences
- 表の比は同一マシン・同一 V8 系列の相対値としてのみ使える。

### Gaps
- x64 の V8 のみ。ARM、Safari (JavaScriptCore)、Firefox (SpiderMonkey)、Wasmtime は未測定。
- ラウンド間のばらつきの原因は特定していない。3 ラウンド以上あるセルの (最大−最小)/中央値 は、高速経路で n=1,000 が中央値 21% (90 パーセンタイル 76%)、n=100,000 が 16% (67%)、n=1,000,000 が 29% (89%)、観測経路で n=1,000 が 27% (72%)、n=100,000 が 24% (63%)。ツールチェーン別の中央値は `netwasm-o3` 11% から `mono-aot` 47% まで幅がある。極端な例として `netwasm-o3` の `RadixLSD256Sort` (StatisticsContext, sorted, n=1,000) はラウンド中央値が 13〜134 ns/要素に分かれた。各セルの値は [results/summary-speed.csv](file:///C:/Users/guitarrapc/AppData/Local/Temp/claude/8ffd9513d941f0a1/4ac9fb56-b669-4e11-a61a-d1ccca49da70/scratchpad/wasm-audit/results/summary-speed.csv) の `roundMin`, `roundMax` を参照。幾何平均はこの影響を受けにくいが、単一セルの比較には使わないこと。
- `coreclr-interp` は 1 ラウンドのみで、`TimSort`/`HeapSort` の一部の n=1,000,000 セルは時間上限のため未測定である (Q2 の幾何平均に `*` を付けた)。
