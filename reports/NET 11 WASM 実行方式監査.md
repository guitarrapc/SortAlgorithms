# 観測経路が NetWasm を選ばせる: .NET 11 Wasm 実行方式監査

**推奨は、NetWasm 0.14.2 (`NetWasmOptimization=O3`、`NetWasmGarbageCollector=NetWasm`、版固定) を SortAlgorithms の主たる Wasm 実行方式とし、Mono AOT を公式サポート付きの比較基準として残すことである。** これはノートに記録されたユーザー決定ではなく、本監査の提案である。決め手は操作観測の経路で、n=100,000 random の `ISortContext` は CoreCLR JIT 比で **NetWasm 0.64 倍、Mono AOT 9.08 倍**、両者の差は約 14 倍あった ([実測][m])。高速経路の random は NetWasm 1.97 倍、Mono AOT 1.88 倍で実質同着であり、配布サイズは Brotli 107 KiB 対 1,540 KiB、Node の起動は 59 ms 対 95 ms である。NetWasm の弱点は sorted と reversed で Mono AOT より 1.28〜1.44 倍遅いこと、開発がほぼ 1 人に集中した 1.0 前の第三者製品であること、コンパイラ部分が OSI オープンソースではないことである。NativeAOT-LLVM は高速経路が最速 (1.09〜1.13 倍) だが `net10.0` でしか動かず、CoreCLR Wasm は .NET 11 GA では interpreter だけで 328 倍かかる。判断を変えるのは、ライセンスと継続性の評価、実際の記録コンテキストでの再測定、Firefox と Safari の結果、.NET 12 の CoreCLR R2R の 4 点である。

| 起きたら | 切り替え先 | 確認方法 |
| --- | --- | --- |
| NetWasm の無償利用条件から外れる、または単独開発への依存を許容できない | Mono AOT (.NET 11 のサポートは 2028-11-14 まで) | [Community License][nw-license] と利用主体の照合。法的判断は別途必要 |
| `CompositeContext` + 実記録で NetWasm と Mono AOT の差が 25% 未満に縮む、または Mono AOT の観測経路が JIT 比 2 倍台まで改善する | Mono AOT | 下の所見 R4、R5 の測定 |
| Firefox または Safari で観測経路の順位が逆転する | その環境で勝つ方式を再評価 | 同一ハーネスを各エンジンで実行 |
| NativeAOT-LLVM に `net11.0` 対応パッケージが出て `OptimizationPreference=Speed` が通り、用途が高速経路中心になる | NativeAOT-LLVM | feed の 11.x 版と再測定 |
| .NET 12 で Mono パック停止が確定し、CoreCLR R2R が Mono AOT に並ぶ | CoreCLR Wasm | [#134337][i134337] と [#134397][i134397] の完了、R2R の再測定 |

## 4 方式を同じ表に並べると、差は実行モデルと EH 形式に集約される

本レポートは証拠を 3 段階に分ける。**【実測】** はこのマシンで実行した [`measurements.md`][m] の結果、**【一次】** はコード、公式文書、パッケージ、リポジトリ内の記録を読んだもの、**【ベンダー】** は製品の開発元自身の主張である。【推定】は上記からの推論で、測定値としては扱わない。SortVivo の文書に残る数値は【一次】の記録であり、本監査では再測定していない。Mono は実行形態で性質が大きく変わるので、AOT と interpreter を分けて示す。

| 項目 | Mono AOT | Mono interpreter | CoreCLR Wasm | NativeAOT-LLVM | NetWasm |
| --- | --- | --- | --- | --- | --- |
| 対応状況【一次】 | .NET 11 の既定ランタイム。[STS、2028-11-14 まで][dotnet-support] | 同左。AOT なしの既定動作 | [GA パックあり][coreclr-pack]、`UseMonoRuntime=false` で opt-in、実験的 | [`dotnet/runtimelab` の実験的 fork][naot-readme]。[11.x パッケージは 0 件][naot-feed] | 第三者製の 1.0 前。[コミット 473 件中 472 件が 1 名][nw-contrib]、[パス別ライセンス][nw-license] |
| 実際に使った版【実測】 | `net11.0`、SDK 11.0.100、Emscripten 6.0.2 パック | 同左 | `net11.0`、SDK 11.0.100、runtime pack 11.0.0 | **`net10.0`**、`10.0.0-rc.1.26357.1`、Emscripten 3.1.56 | `netwasm0.1`、`NetWasm.Sdk` 0.14.2 (SDK 11.0.100 上) |
| 速度優先の設定【実測】 | `RunAOTCompilation=true`、`WasmBuildNative=true`、compile / link / bitcode `-O3`、`WasmStripILAfterAOT=true`、SIMD、Wasm EH、`TrimMode=full` | AOT 関連を除き同左 | `UseMonoRuntime=false`、`TrimMode=full`、**`WasmBuildNative=false`** | clang `-O3`、link `-O1`。**`OptimizationPreference=Speed` なし**。speed 版は非公式に `-msimd128` + link `-O3` | `NetWasmOptimization=O3`、`NetWasmGarbageCollector=NetWasm`、hints `Auto` |
| 実行モデル【一次】 | LLVM AOT。`LLVMOnlyInterp` で、AOT できないメソッドは interpreter | IL interpreter + jiterpreter | CoreCLR interpreter。実行時 JIT なし | ILC + LLVM の事前コンパイル | `nwc` の閉世界 AOT + `wasm-opt` |
| バイナリ内の Wasm 機能【実測】 | SIMD128 32,963、`try_table` 351、`throw_ref` 65、bulk 774 | SIMD128 950、`try_table` 6、`throw_ref` 1、bulk 394 | SIMD128 6,067、`try_table` 10,587、`throw_ref` 6,612、bulk 1,632 | SIMD128 476、旧式 `try` 493、bulk 89 (speed 版は 1,857 / 455 / 176) | SIMD128 1,765、`try_table` 36、bulk 47 |
| `Vector128.IsHardwareAccelerated`【実測】 | true | true | false | false | true |
| 最低ランタイム【一次から導出】 | Chrome/Edge 137、Firefox 131、Safari 18.4、Node 24.15 | 同左 | 同左 (公式表なし) | Chrome 96、Firefox 100、Safari 16.4、Node 17.2 (未検証) | [同左を文書化][nw-targets] |
| JS 呼び出し機構 | `[JSExport]` / `[JSImport]`、`dotnet.js` | 同左 | 同 API、別実装の `dotnet.js` | `[UnmanagedCallersOnly]` の直接 export + `Module.HEAP32` | `OutputType=JsModule`、`[JSExport]` / `[JSImport]`、生成 `.mjs` |
| 空 export 呼び出し【実測】 | 160 ns | 190 ns | 5,331 ns | 9.1 ns | 104 ns |
| `Int32Array` n=100,000: `int[]` 往復【実測】 | 126.7 µs | 190.8 µs | 123.1 µs | 経路なし | 121.3 µs |
| 同: 線形メモリ書込 + 呼出【実測】 | 6.3 µs | 6.3 µs | 12.2 µs | 5.8 µs | 6.7 µs (`[JSImport]` の span view) |
| 同: 書込 + 呼出 + 読戻し【実測】 | 106.9 µs | 110.4 µs | 123.6 µs | 116.5 µs | 110.9 µs (非公式手段) |
| 速度優先 Brotli 合計【実測】 | 1,540 KiB | 814 KiB | 1,840 KiB (relink なし) | 308 KiB (speed 版 289) | 107 KiB |
| 起動→初回ソート完了【実測】 | Node 94.5 ms、Edge 236 ms | Node 106 ms、Edge 233 ms | Node 197 ms、Edge 357 ms | Node 35.5 ms、Edge 152 ms (speed 版 31.9 / 151) | Node 59.2 ms、Edge 189 ms |
| 高速経路 CoreCLR JIT 比【実測】 | 1.88 | 4.13 | 328 | 1.09 (speed 版 1.13) | 1.97 |
| 観測経路 CoreCLR JIT 比、`ISortContext`【実測】 | 9.08 | 9.76 | 69.5 | 1.96 (speed 版 2.29) | 0.64 |
| 観測経路 CoreCLR JIT 比、`StatisticsContext`【実測】 | 9.27 | 3.28 | 84.6 | 2.23 (speed 版 2.55) | 0.72 |

速度は int32、random、n=100,000、ライブラリ 6 算法 (`PDQSort`、`TimSort`、`Driftsort`、`HeapSort`、`ShellSortCiura2001`、`RadixLSD256Sort`) の幾何平均で、同一プロセスの JS `plainQuick` でドリフトを補正した。基準は `DOTNET_TieredCompilation=0`、`DOTNET_TieredPGO=0` の win-x64 CoreCLR JIT である。観測経路の幾何平均は、実測の `summary-speed.csv` から本レポートで算出した。

**これらの数値は x64 の V8 だけの結果である。** Node 24.18.0 と Edge 157 headless で測定し、Firefox、Safari、Wasmtime、ARM は測っていない。セルごとのラウンド間差 ((最大−最小)/中央値) の中央値は CPU 固定なしで 24.8%、固定後も 23.2% あり、**単一セルの 25% 未満の差は順位の根拠にならない**。ばらつきは方式でも異なり、中央値で `netwasm-o3` 11%、`mono-aot` 47% である。NativeAOT-LLVM の値は .NET 11 の BCL ではなく `net10.0` の結果で、`OptimizationPreference=Speed` は ILC の例外で使えなかった。speed 版は非公式に `-msimd128` と link `-O3` を足した構成である。CoreCLR は `WasmBuildNative=false` で、Node は 1 ラウンド、ウォームアップ 1 回、最大 3 サンプル、一部の n=1,000,000 は時間上限で欠け、Edge は 5 算法の縮小範囲である。全構成の出力検証は失敗 0 件で、比較回数などの統計は Node の 240 セルすべてで一致した ([実測][m])。

## CoreCLR Wasm は公開済みだが、この環境の速度優先構成は失敗した

「CoreCLR Wasm に一般利用できるリリースはない」という認識は正しくない。**.NET 11 GA には opt-in の経路がある。** `Microsoft.NETCore.App.Runtime.browser-wasm` 11.0.0 は NuGet で公開され、中身は `libcoreclr_static.a` を含む CoreCLR 版である ([NuGet catalog][coreclr-pack])。`Microsoft.NET.Sdk.WebAssembly` で `UseMonoRuntime=false` を指定すれば選択でき ([Preview 5 notes][coreclr-p5])、このマシンでも Node と Edge で全出力検証に合格し、`[JSExport]` も動いた ([実測][m])。

同時に、**既定のトリム付き Release relink はこのマシンで失敗した。** `PublishTrimmed=true` では SDK が `WasmBuildNative` を有効にし、`wasm-ld` が `undefined symbol: SystemJS_ExecuteTimerCallback` で停止した。動いたのは `-p:WasmBuildNative=false` の prebuilt ランタイムだけである。トリムなしの relink はビルドに成功したが、速度は測っていない。`PublishReadyToRun=true` は `NETSDK1095` で拒否された ([実測][m])。したがって今回の 328 倍は「CoreCLR Wasm の最終性能」ではなく、「GA SDK で動く interpreter 構成の性能」である。

事前コンパイルの本命である R2R は、ブラウザ向け SDK 配線が `main` にあり `release/11.0` にはない ([11.0 targets][coreclr-11-targets]、[main targets][coreclr-main-targets])。.NET 12 では既定化と Mono パック停止が Issue に載るが、どちらも open の計画で、製品としての確約ではない ([#134337][i134337]、[#134397][i134397])。正式なサポート声明は見つからなかったので、現時点の位置付けは実験的機能である。

## 高速経路は random で同着、入力の形で Mono AOT が先行する

**Mono AOT と NetWasm は random の高速経路で実質同着である。** 1.88 倍と 1.97 倍の差は 5% で、n=1,000,000 の生値では Node が 75.3 ns 対 72.7 ns と順序が逆転し、Edge の n=100,000 は 53.9 ns 対 59.4 ns である。ただし同着は全入力には広がらない。sorted と reversed では 6 算法の幾何平均で NetWasm が 1.44 倍、1.28 倍遅く、25% のノイズ目安を超える。

| n=100,000、CoreCLR JIT 比【実測】 | Mono AOT | NetWasm O3 | NetWasm ÷ Mono AOT | NetWasm `IgnoreAggressive` | NativeAOT-LLVM | speed 版 | Mono interpreter | CoreCLR |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| random | 1.88 | 1.97 | 1.05 | 2.16 | 1.09 | 1.13 | 4.13 | 328 |
| sorted | 2.10 | 3.02 | 1.44 | 3.50 | 1.30 | 1.19 | 6.36 | 1,096 |
| reversed | 2.26 | 2.89 | 1.28 | 3.34 | 1.27 | 1.26 | 6.85 | 963 |
| duplicates | 2.24 | 2.53 | 1.13 | 2.77 | 1.33 | 1.32 | 4.51 | 227 |

幾何平均は算法ごとの大きな差を隠している。ランキング機能では、この差がそのまま順位の差になる。

| random n=100,000、ns/要素【実測】 | CoreCLR JIT | Mono AOT | NetWasm O3 | NativeAOT-LLVM |
| --- | --- | --- | --- | --- |
| `PDQSort` | 40.6 | 52.2 | 56.2 | 43.4 |
| `TimSort` | 60.9 | 79.8 | 109 | 64.5 |
| `Driftsort` | 14.7 | 39.2 | 100 | 29.4 |
| `HeapSort` | 56.2 | 52.2 | 87.3 | 38.7 |
| `ShellSortCiura2001` | 70.4 | 115 | 88.6 | 70.4 |
| `RadixLSD256Sort` | 6.55 | 41.8 | 9.56 | 9.53 |

抽象のない同一の `PlainQuick(C#)` は AOT 系 3 種で JIT 比 1.03〜1.07 倍に収まる。【推定】ライブラリ算法の差は平均的なコード生成力より、`SortSpan`、比較子、バッファの展開度合いから来ている。`Driftsort` の比は割り引いて読む必要がある。JIT 基準だけが `Avx512F.IsSupported` の分割経路を使い、Wasm はスカラー経路へ落ちるからである。`RadixLSD256Sort` の Mono AOT は Node 41.8 ns に対し Edge 23.8 ns で開きがあり、原因は未特定である。

JS の組み込みソートと比べると、n=100,000 random の 6 算法は `Int32Array.prototype.sort` に対して NativeAOT-LLVM 0.81 倍、Mono AOT 1.38 倍、NetWasm 1.46 倍だった。起動直後の 1 回目は別の話で、同じ 6 算法の初回実行は幾何平均で NativeAOT-LLVM 7.48 ms、Mono AOT 9.61 ms、NetWasm 11.55 ms、Mono interpreter 63.2 ms となり、定常値の 1.7〜3.7 倍かかった ([実測][m])。V8 の Wasm は実行中の呼び出しを OSR しない ([V8][v8-pipeline]) ので、計測 UI では初回と定常を分ける必要がある。

## 観測経路では順位が反転し、Mono AOT の利点が消える

記録や統計を伴う実行では、高速経路の順位は当てにならない。NetWasm は CoreCLR JIT 基準より速く、Mono AOT は interpreter と同じ桁まで落ちる。

| n=100,000 random【実測】 | `ISortContext` ns/要素 | JIT 比 | `StatisticsContext` ns/要素 | JIT 比 |
| --- | --- | --- | --- | --- |
| CoreCLR JIT | 207 | 1.00 | 183 | 1.00 |
| NetWasm O3 | 130 | 0.64 | 128 | 0.72 |
| NativeAOT-LLVM | 409 | 1.96 | 417 | 2.23 |
| Mono AOT | 1,879 | 9.08 | 1,683 | 9.27 |
| Mono interpreter | 2,013 | 9.76 | 617 | 3.28 |
| CoreCLR interpreter | 17,189 | 69.5 | 18,532 | 84.6 |

Mono AOT のラウンド間差はこの経路で大きいが (算法別に 33〜100%)、NetWasm との約 14 倍の差はノイズでは説明できない。Edge の縮小測定でも `ISortContext` の `PDQSort` は NetWasm 102 ns、NativeAOT-LLVM 206 ns、Mono AOT 991 ns、`TimSort` は 210、385、2,180 ns と同じ順序だった。注目すべきは、`StatisticsContext` で **Mono AOT が Mono interpreter より 2.7 倍遅い**ことである。【推定】参照型の `TContext` を共有ジェネリックとして扱う経路で interpreter への遷移が起きている可能性があるが、ログでは確認していない。

この結果を「NetWasm は実アプリの記録でも JIT より速い」と読んではならない。ハーネスのコンテキストはカウンタ加算だけの `StatisticsContext` で、呼び出し先が 1 種類に定まる。SortVivo の記録は `CompositeContext(stats, record)` を使い、`OperationRecordingContext` は `List.Add` と上限判定を行う (`Sortvivo2/src/SortVivo.Engine/Exports.cs:166-169`)。JIT 基準も階層化 PGO を切っている。【推定】NetWasm の優位の一部は閉世界の脱仮想化に由来し、実記録では縮む。方向は強い根拠があるが、倍率は未確定である。

NetWasm 開発元の数値は別枠で扱う。【ベンダー】NetWasm 0.14.0 は Wasmtime 50.0.1、Apple M1 Max で 2,213 ベンチの幾何平均が Mono AOT の 0.28 倍、desktop JIT の 1.51 倍と公表している ([micro-benchmarks][nw-micro])。開発元の独自ワークロードの quicksort は 53.9 ms 対 Mono AOT 59.7 ms である ([performance][nw-perf])。実行エンジンも CPU も異なり、第三者の再現は見つからなかったので、上の実測値とは比較しない。

## Wasm 規格の分かれ目は exnref と旧式 EH である

W3C の勧告は Wasm 1.0 までで、最新の `wasm-core-2` CRD 2026-11-16 は Release 3.0 を記述する living standard である ([W3C][w3c-core])。今回の 4 方式はすべて wasm32 で、**relaxed SIMD、tail call、GC 命令と GC 型、atomics、shared memory、memory64 はどのバイナリでも 0 件だった**。Safari 安定版に relaxed SIMD と memory64 がないことは、現時点の成果物には影響しない ([機能表][wasm-features])。

実質的な分岐は例外処理である。Mono、CoreCLR、NetWasm は Wasm 3.0 の `try_table` を含むので、Chrome/Edge 137、Firefox 131、Safari 18.4、Node 24.15 が下限になる。NativeAOT-LLVM だけは旧式 `try` を使い、古いブラウザに届く代わりに提案上 inactive の形式へ依存する ([機能表][wasm-features])。SIMD も命令の有無と C# の経路を分ける必要がある。CoreCLR と NativeAOT-LLVM のバイナリは SIMD128 命令を含むが、C# からは `Vector128.IsHardwareAccelerated=false` に見える。ライブラリ自体に `Vector128` や `PackedSimd` の経路はなく、明示 SIMD は x64 の AVX-512 カーネルだけなので (`src/SortAlgorithm/Algorithms/Merge/Driftsort.cs:705`、`src/SortAlgorithm/Algorithms/Merge/Glidesort.cs:1449`)、この差がソート本体へ直接効く場面は限られる。

## JS 境界の差は配列より呼び出し回数に出る

1 回の大きなソートでは、JS 境界はほとんど順位を動かさない。n=100,000 の `int[]` 往復は Mono interpreter を除き 121〜127 µs で、線形メモリへの書き込みは 6〜12 µs、結果を `slice` で読み戻すと全方式が 107〜124 µs に揃う。NetWasm の `PDQSort` 本体は約 5.6 ms なので、往復コピーは約 2% である。差が出るのは呼び出し回数が多い設計で、空 export は NativeAOT-LLVM 9.1 ns、NetWasm 104 ns、Mono AOT 160 ns、CoreCLR 5,331 ns だった。要素ごとに JS と往復する API は避けるべきである。

API の制約は実装前に確認が要る。JS の `Int32Array` を `[JSExport] Span<int>` へ渡す操作は Mono と CoreCLR で失敗し、`ArraySegment<int>` へ渡すと **Mono はランタイムごと致命的 assert で停止**した。NetWasm は `[JSExport]` の `Span<int>` をビルド時に `NW1003` で拒否する。表の NetWasm の読戻し値は、`[JSImport]` の span view から得た `.buffer` を呼び出し後も保持する非公式手段であり、製品コードの根拠にはしない ([実測][m]、[NetWasm JS interop][nw-js])。NativeAOT-LLVM の `DotNetJsApi=true` は `[JSExport]` を動かしたが、未文書で、Brotli は 545 KiB に増え、空 export は 87 ns になった。

サイズの比も条件付きで読む。表の 107 KiB 対 1,540 KiB は 9 算法だけを到達可能にしたハーネスの値である。【一次】SortVivo の全 87 算法級の記録では NetWasm エンジン一式 466,440 B (`Sortvivo2/.github/plans/v2_issues.md:290`)、Blazor なしの Mono AOT 試作は `_framework` 2,124,713 B (`Sortvivo2/tools/wasm-engine-prototype/README.md:25`) で、差は約 4.6 倍に縮む。測定日と構成が異なるので目安であるが、製品判断にはこちらのほうが近い。

## ノートの食い違いはこのマシンの実測を優先して解消した

| 論点 | ノートや文書の記述 | このマシンの実測 | 扱い |
| --- | --- | --- | --- |
| NativeAOT-LLVM の SIMD | [C# の SIMD は未対応][naot-simd]、`-msimd128` は渡されない | 既定でも SIMD128 476 命令、`target_features` に `+simd128`。C# 側は `Vector128` false | 両立する。C# 経路は非加速だが、バイナリは SIMD128 対応ホストを要する【推定】 |
| exnref の Node 下限 | Emscripten 表は 22.0 (フラグ前提)、CoreCLR 文書は [`--experimental-wasm-exnref`][coreclr-wasm-md]、V8 定義は [24.15 で既定有効][node-2415] | Node 24.18.0 はフラグなしで `try_table` を検証・実行 | 24.15 以上を採用。24.15 未満は未測定 |
| NativeAOT-LLVM の速度設定 | [`OptimizationPreference=Speed` が実効プロパティ][naot-targets] | `getPgoInstrumentationResults` の `NullReferenceException` で ILC が失敗 | 今回の値は Speed なし。Speed 時の性能は不明 |
| CoreCLR の GA 構成 | Release のトリム publish は relink が既定で、アプリの thunk もそこで組み込む ([targets][coreclr-11-targets]) | 既定 relink は失敗、relink なしでも `[JSExport]` は動作、R2R は `NETSDK1095` | 動作確認済みの構成は `WasmBuildNative=false` のみ |
| JS import の `Span` | [公式文書は `Span` 引数の import 不可と記載][ms-interop] | `[JSMarshalAs<JSType.MemoryView>] Span<int>` の `[JSImport]` は Mono と CoreCLR で動作 | 文書より実測を優先。JS から C# へ渡す方向は不可のまま |
| NetWasm の必須機能 | [文書は multi-value など][nw-targets]、SortVivo の `deployment.json:7` は `featureSet: "none"` | 今回のバイナリは multi-value 0/0、SIMD128 1,765、`try_table` 36 | 文書は下限、`featureSet` は実機能一覧として使わない |
| inlining hints | `Auto` は `IgnoreAggressive` より 1.08 倍遅く、Brotli +62.7 KB (`Sortvivo2/.github/plans/v2_issues.md:214`) | `IgnoreAggressive` が 1.08〜1.18 倍遅い。`HeapSort` は 1.61 倍、サイズ差は 2 KiB | 方向が逆。対象規模が違うので SortVivo 全体で再測定 |
| NetWasm と旧 Blazor の差 | SortVivo 記録は random 1.54 倍、sorted 1.70 倍 (`Sortvivo2/.github/plans/v2_issues.md:312`) | 暖機後の `IgnoreAggressive` ÷ Mono AOT は random 1.13、sorted 1.66 | sorted は整合。random の差は未最適化 tier と起動込みの測定差【推定】 |
| NetWasm の速度主張 | 【ベンダー】Wasmtime では Mono AOT の 0.28 倍 | V8、x64 の random 高速経路は同着 | 実行環境が異なるため再現とも反証とも扱わない |

## リポジトリの所見と次の行動

行番号は監査時点の作業ツリーで確認した。`performance.md` は未コミット変更により HEAD より 1 行ずれている。以下の行動はどれも算法の定義を変えない。Wasm での遅さを補うためのカットオフ、別算法への切替、ハイブリッド化は、[`AGENTS.md`](../AGENTS.md) の方針どおり推奨しない。

| ID | 状態 | 所見 | 根拠 | 次の行動 |
| --- | --- | --- | --- | --- |
| R1 | 確認済み | 仕様が `NetWasm SDK 0.13.0` を名指しするが、消費側は 0.14.2 である。`NETWASM` 自体は 0.14.2 でも定義される | `.github/docs/specs/performance.md:189` (HEAD は `:188`)、`src/SortAlgorithm/Algorithms/SortSpan.cs:447`、`Sortvivo2/src/SortVivo.Engine/SortKernel.csproj:1`、`NetWasm.Sdk/0.14.2/Sdk/NetWasm.Sdk.Tfm.props:49` | 仕様を「`netwasm0.1` では SDK が `NETWASM` を定義する」と版非依存に直し、コメントからも版番号を外す |
| R2 | 確認済み / 効果は要計測 | 4 helper の `#if NETWASM` inline 指定は、SortVivo の `IgnoreAggressive` 下で効かない可能性が高い。今回の観測経路差は 1.06 倍でノイズ内 | `src/SortAlgorithm/Algorithms/SortSpan.cs:450,468,484,500`、`Sortvivo2/src/SortVivo.Engine/SortKernel.csproj:8` | `#if` の有無と hints 2 種の 4 通りで生成物と `ISortContext` を比較し、効果がなければ条件分岐の削除を検討 |
| R3 | 確認済み | SortVivo の submodule が `526498a` のままで、`f674c43` より 32 commit 遅れ、`src/SortAlgorithm` は 58 ファイル +5,842 / −1,286 行の差がある。`main` も `v2` も同じ commit で、TFM は `net10.0` | `Sortvivo2` の `HEAD`、`origin/main`、`origin/v2` の gitlink、`Sortvivo2/SortAlgorithms/Directory.Build.props:3`、`Directory.Build.props:3` | コミット済みの新しい版へ更新し、`1,775` 入力 / `16,704` 記録の照合 (`Sortvivo2/.github/docs/plans/static-ui-wasm-migration.md:284`) とサイズ・速度比較を再実行 |
| R4 | 実測で確認 / 原因は要計測 | Mono AOT の観測経路は JIT 比 9.08〜9.27 倍で、`StatisticsContext` は interpreter より遅い。本番の入口は `ISortContext` である | `src/SortAlgorithm/Contexts/ISortContext.cs:55,82`、`src/SortAlgorithm/Algorithms/SortSpan.cs:129,824,852`、`Sortvivo2/src/SortVivo.Core/Models/AlgorithmMetadata.cs:24` | AOT 出力でメソッド単位に interpreter への遷移を確認し、値型 `TContext` の専用インスタンス化を試してサイズ増と速度を比べる |
| R5 | 要計測 | 観測経路の順位は counter のみの単一コンテキストで得た。実アプリは `CompositeContext` と記録処理を通る | `src/SortAlgorithm/Contexts/CompositeContext.cs:23-29`、`Sortvivo2/src/SortVivo.Engine/Exports.cs:166-169`、`Sortvivo2/src/SortVivo.Core/Services/OperationRecordingContext.cs:39-56` | `CompositeContext(StatisticsContext, OperationRecordingContext)` を NetWasm、Mono AOT、JIT で測り、方式選定の前提を確定する |
| R6 | 実測と記録が矛盾 | `IgnoreAggressive` はハーネスでは遅く、SortVivo の記録では速い | `Sortvivo2/src/SortVivo.Engine/SortKernel.csproj:8`、`Sortvivo2/.github/plans/v2_issues.md:214` | 0.14.2 と全 87 算法の dispatch で `Auto` と比較し、算法別に確認する。`HeapSort` と sorted を重点にする |
| R7 | 確認済み / 本番効果は要計測 | SortVivo の Mono AOT は bitcode だけ `-O3` で、compile と link の指定がない。ハーネスでは 3 箇所の `-O3` が既定より 1.13 倍速く、Brotli 増は 0.7% だった | `Sortvivo2/src/SortVivo/SortVivo.csproj:17` | Mono を維持する間は `EmccCompileOptimizationFlag` と `EmccLinkOptimizationFlag` の追加効果を本番構成で測る |
| R8 | 確認済み | ライブラリに Wasm の CI と trim/AOT 宣言がなく、消費側の `TrimmableAssembly` が補っている | `src/SortAlgorithm/SortAlgorithm.csproj:1-19`、`Sortvivo2/src/SortVivo/SortVivo.csproj:24` | `IsTrimmable` と `IsAotCompatible` の警告を確認し、Wasm 出力ダイジェストの smoke test を CI 候補にする。NetWasm を CI に入れるかはプロジェクト判断とする |
| R9 | 要計測 | HEAD 固有の `PreparedKey` と `[InlineArray]` は今回の 9 算法の実行経路に入っていない | `src/SortAlgorithm/Algorithms/PreparedKey.cs:19-23`、`src/SortAlgorithm/Algorithms/Merge/QuadSort.cs:110`、`src/SortAlgorithm/Algorithms/Partition/QuickSort.cs` | `QuickSort`、`BinaryInsertionSort`、`SampleSort`、`QuadSort`、`FluxSort` をハーネスに加えて Wasm で検証する |
| R10 | 確認済み / 改善は要計測 | `FlashSort` は要素ごとに `UInt128` の乗除算を行い、NetWasm は JIT 比 4.37 倍、Mono AOT は 2.41 倍だった | `src/SortAlgorithm/Algorithms/Distribution/FlashSort.cs:134,163,178,212-213` | 32-bit 以下の鍵では積が `ulong` に収まるので、同じ商を `ulong` で計算する案を測る。分類式は変えない |
| R11 | 要計測 | Firefox、Safari、ARM、Wasmtime の順位は不明で、推奨の適用範囲を限定している | [`measurements.md`][m] の Gaps | 同一ハーネスを Firefox と Safari で実行し、起動・高速経路・観測経路の順位を確認する |

NetWasm を採用する場合は、`NetWasm.Sdk` の版固定を維持し、パッケージハッシュをロックし、利用主体が [Community License][nw-license] の無償条件に入るかを確認する。公開された [`release.yml`][nw-release] に build provenance の記述は確認できず、ビルド時に単一発行者のネイティブツールを実行する点も受け入れ条件に含めるべきである。

## Conclusion

この監査で変わった問いは「どの Wasm ランタイムが速いか」ではなく、「ライブラリの `TContext` をどの程度まで具象化できるか」である。`NullContext` の経路は JIT の定数畳み込みを前提に設計され、AOT 系でも JIT 比 1.09〜1.97 倍に収まるが、観測経路では NetWasm と Mono AOT の間に 14 倍の差が開く。【推定】その主因は共有ジェネリックとインターフェース呼び出しの扱いであり、SortAlgorithms の方式選択は、ソート算法の速さよりも、可視化と統計という本来の用途を各コンパイラがどう実体化するかで決まる。

したがって次の投資先は、新しいアルゴリズム変更ではなく、実記録コンテキストの測定、`Sortvivo2` の submodule 更新、Firefox と Safari の確認である。NetWasm の優位がそこでも残れば採用の根拠は強まり、残らなければ公式サポートと sorted 入力に強い Mono AOT が現実的な選択へ戻る。どちらの場合も、.NET 12 の CoreCLR R2R が出た時点で同じハーネスを再実行できることが、今回の測定資産の最大の価値である。

[m]: ../research_notes/NET%2011%20WASM%20実行方式監査/measurements.md
[dotnet-support]: https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core
[coreclr-pack]: https://api.nuget.org/v3/catalog0/data/2026.11.10.14.36.27/microsoft.netcore.app.runtime.browser-wasm.11.0.0.json
[coreclr-p5]: https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview5/runtime.md
[coreclr-11-targets]: https://github.com/dotnet/runtime/blob/release/11.0/src/mono/browser/build/BrowserWasmApp.CoreCLR.targets
[coreclr-main-targets]: https://github.com/dotnet/runtime/blob/main/src/mono/nuget/Microsoft.NET.Sdk.WebAssembly.Pack/build/Microsoft.NET.Sdk.WebAssembly.Browser.CoreCLR.targets
[coreclr-wasm-md]: https://github.com/dotnet/runtime/blob/main/docs/workflow/building/coreclr/wasm.md
[i134337]: https://github.com/dotnet/runtime/issues/134337
[i134397]: https://github.com/dotnet/runtime/issues/134397
[naot-readme]: https://github.com/dotnet/runtimelab/blob/feature/NativeAOT-LLVM/README.md
[naot-feed]: https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-experimental/nuget/v3/flat2/microsoft.dotnet.ilcompiler.llvm/index.json
[naot-simd]: https://github.com/dotnet/runtimelab/issues/2515
[naot-targets]: https://github.com/dotnet/runtimelab/blob/527a565752062b20bb9693eea6c884b34ecce7bf/src/coreclr/nativeaot/BuildIntegration/Microsoft.NETCore.Native.targets#L314-L315
[nw-contrib]: https://api.github.com/repos/zion-sati/NetWasm/contributors?per_page=20
[nw-license]: https://github.com/zion-sati/NetWasm/blob/main/LICENSES/LicenseRef-NetWasm-Community-1.0.txt
[nw-targets]: https://github.com/zion-sati/NetWasm/blob/main/docs/targets-and-outputs.md
[nw-js]: https://github.com/zion-sati/NetWasm/blob/main/docs/javascript-interop.md
[nw-micro]: https://github.com/zion-sati/NetWasm/blob/main/docs/micro-benchmarks.md
[nw-perf]: https://github.com/zion-sati/NetWasm/blob/main/docs/performance.md
[nw-release]: https://github.com/zion-sati/NetWasm/blob/main/.github/workflows/release.yml
[w3c-core]: https://www.w3.org/TR/2026/CRD-wasm-core-2-20261116/
[wasm-features]: https://github.com/WebAssembly/website/blob/main/features.json
[node-2415]: https://github.com/nodejs/node/blob/v24.15.0/deps/v8/src/wasm/wasm-feature-flags.h
[v8-pipeline]: https://v8.dev/docs/wasm-compilation-pipeline
[ms-interop]: https://learn.microsoft.com/en-us/aspnet/core/client-side/dotnet-interop/?view=aspnetcore-11.0
