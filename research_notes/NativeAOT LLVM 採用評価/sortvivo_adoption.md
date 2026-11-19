# SortVivo の WebAssembly エンジンに NativeAOT-LLVM を採用できるか (コード監査)

監査日: 2026-11-19。読み取り専用で行い、どちらのリポジトリでもビルド・実行・変更をしていない (書いたのはこのファイルだけ)。Sortvivo2 はブランチを切り替えず、`main` は `git show` で読んだ。開始時の `git status --porcelain -uall` は空だった。監査の途中で、Sortvivo2 の作業ツリーに本監査によらない未コミットの変更が現れた。`engine-contract.json` 2 件 (revision `6b3fbfd1…` → `67477ea4…`)、`initial-preferences.js`、そのテストで、`OUT/` も 15:46 に再生成されていた。本文の引用はコミット済みの HEAD と、変更前に読んだ内容に基づく。

出典の表記 (本文の `path:line` は次の略号を起点とする):

| 略号 | 実体 | 版 |
|---|---|---|
| `SV/` | `D:\github\guitarrapc\Sortvivo2` | ブランチ `v2`、HEAD `0f772f78` (2026-11-19 04:18 +0900) |
| `SV@main:` | 同リポジトリの `main` (`git show main:<path>`) | `dad7393c` (2026-11-14) |
| `WEB/` | `SV/src/SortVivo.Web` | 同上 |
| `ENG/` | `SV/src/SortVivo.Engine` | 同上 |
| `OUT/` | `SV/artifacts/migration/engine/browser` (gitignore 対象の既存ビルド出力) | ファイル時刻 2026-11-18 23:24〜23:26 |
| `SVsub/` | `SV/SortAlgorithms` (submodule) | `526498a` |
| `AUD/` | `D:\github\guitarrapc\SortAlgorithms\research_notes\NET 11 WASM 実行方式監査` | 先行調査 (`repo_audit.md`, `measurements.md`, `nativeaot_llvm.md`, `wasm_standards.md`) |
| `PoC` | `D:\github\test\Netwasm\investigation\architecture\README.md` | リポジトリ外の過去 PoC 記録 |

用語: **高速経路** = コンテキストなし (`NullContext`)。`Dispatch.Plain` から `XxxSort.Sort(arr)` を呼ぶ経路。**観測経路** = `TContext = ISortContext`。`Dispatch.Observe` から `XxxSort.Sort(arr, ctx)` を呼ぶ経路で、記録 (`CompositeContext(StatisticsContext, OperationRecordingContext)`)、操作数の事前計数 (`OperationCountingContext`)、サムネイル (`ThumbnailContext`) がここを通る。**NAOT-LLVM** = NativeAOT-LLVM。

「確認済み」はコードや文書で直接確かめた事実である。「推定」は確かめた事実からの推論である。NAOT-LLVM の数値はすべて `AUD/measurements.md` の 9 算法カーネルで測った値で、SortVivo の全 87 算法エンジンをビルドした実績はない。

---

## Q1. ワークロードの構成: どの機能がどの export をどの経路で呼ぶか

### Takeaway
製品が呼ぶ export は 9 個のうち 7 個 (`Input`, `Shuffle`, `Thumbnail`, `Batch`, `CopyBatch`, `Count`, `Record`) で、`Sort` と `LastMetrics` は検証ツールしか呼ばない。高速経路の速さは、画面に出る数値と正誤を決める。Visualizer と Sortube のソート時間、Sortube の勝敗、Ranking の順位、Quiz「いちばん速いのは?」の正解がこれに当たる。観測経路の速さは、記録を待つ時間を決める。ただし 4,096 要素以下では、ジョブ時間 (約 85〜125 ms) の大半を時間予算型の較正 (約 80〜100 ms) が占め、記録は 0.3〜20 ms しかない。この較正の所要時間はエンジンの速さにほとんど依存しない。観測経路が待ち時間を支配するのは、大きな入力や O(n²) 算法の上限付近だけである。How It Works はエンジンを呼ばない。

### Cited Findings (確認済み)

**機能 → export → 経路**

| 機能 | 呼び出し元 | Worker | 呼ぶ export | 経路 | 体感への効き方 |
|---|---|---|---|---|---|
| Visualizer のカード生成・再生成 | `WEB/src/components/SortCard.tsx:77` (カードごとに `PlayerSession`)、`WEB/src/lib/player.ts:30,76` → `WEB/src/lib/engine-client.ts:22-32` | `WEB/public/engine-worker.mjs:44-47,72-79` | `Input`、`Batch` と `CopyBatch` (較正 `:94-129`、5 ラウンド `:131-151`)、`Record` (`:155`) | Batch/CopyBatch は高速、Record は観測 | カードに出る時間 (`sortMs`、`player.ts:142`) は高速経路。待ち時間は較正 + 記録 |
| Sortube の動画・対決 | `WEB/src/lib/sortube-preparation.ts:72-83` | `engine-worker.mjs:57-70` (pair) | `Input`、2 レーン分の `Batch`/`CopyBatch`、`Record` ×2 | 両方 | 勝敗と再生時間の比は `sortMs` で決まる (`WEB/src/lib/sortube-player.ts:233,324,623`) |
| Explore のサムネイル | `WEB/src/lib/sortube-explore.ts:283-287` (カードごとに逐次) | `engine-worker.mjs:48-55` | `Input`、`Thumbnail` (中で `Count` と `Observe(ThumbnailContext)`、`ENG/Exports.cs:43-56`) | 観測のみ (1 枚につき 2 パス) | カード列が埋まるまでの時間 |
| Sort Basics の本棚 | `WEB/src/lib/basics-bookshelf.ts:149-152,217-220` | `engine-worker.mjs:14-29`、値を渡すので較正なし (`:72-74`) | `Shuffle`、`Record` | 観測 | 記録待ちだけ (配列は小さい) |
| Sort Basics の対決演習 | `WEB/src/lib/basics-races.ts:219-223` | 同上 (値を渡す) | `Record` | 観測 | 同上 |
| 絵あて | `WEB/src/lib/painting-quiz.ts:272-280` | `engine-worker.mjs:72-79` | `Input`、`Batch`/`CopyBatch`、`Record` | 両方。計測結果は使わない | 記録 + 使われない較正 |
| Quiz のプレビュー・解説・結果・記録庫 | `WEB/src/lib/quiz-preview.ts:48-54`。値ありは `quiz-play.ts:474-486,765`、値なしは `quiz-explanation.ts:111-118`、`quiz-result.ts:526-536`、`world-dex.ts:162` | engine-worker | 値ありは `Record`。値なしは `Input` + `Batch`/`CopyBatch` + `Record` | 観測 (値なしは使われない較正も走る) | 記録待ち |
| Quiz の出題「いちばん速いのは?」 | `WEB/src/lib/quiz-computation.ts:7-15` → `WEB/src/lib/quiz-worker.ts:79-116` | quiz-worker | `Input`、`Batch`、`CopyBatch` | 高速 | 正解そのもの |
| Quiz の計算量問題 | `quiz-worker.ts:42-72` | quiz-worker | `Input`、`Count` を候補 3〜4 回 | 観測 (`OperationCountingContext`) | 出題までの待ち |
| Quiz の入力生成 | `quiz-worker.ts:74-78` | quiz-worker | `Input` | ソートなし | — |
| Ranking のレース | `WEB/src/lib/ranking-client.ts:14-21` → `WEB/src/lib/ranking-worker.ts:48-60` | ranking-worker | `Input`、`CopyBatch`、`Batch` | 高速 | 順位と時間そのもの |
| How It Works | `WEB/src/components/HowItWorks.tsx:7` → `WEB/src/lib/lesson-player.ts:1-20` (`fetchContent` と生成済みデータだけ) | なし | なし | — | 生成済みの差分記録を配信する (`SV/.github/docs/specs/static-ui-engine.md:86`) |
| 検証ツールのみ | `WEB/tools/m2-contracts.mjs:55-59,166`、`m4-contracts.mjs:29`、`m8-dx-registry.mjs:161`、`m5-oz-performance.mjs:46` | ページ直下 | `Sort`、`LastMetrics` | — | 製品からは呼ばない |

**経路の実体**
- `Dispatch.Plain` は各算法を `NoInlining` の別関数で `Sort(arr)` として呼ぶ高速経路である — `ENG/Dispatch.cs:8,278-451`。`Batch` と `CopyBatch` はこれと `Array.CopyTo` だけを使う — `ENG/Exports.cs:83-117`。
- `Dispatch.Observe(int, Span<int>, ISortContext)` は観測経路である — `ENG/Dispatch.cs:452-720`。`Record` は `CompositeContext(stats, record)` を渡す — `ENG/Exports.cs:165-171`。`Count` は `OperationCountingContext` を渡す — `ENG/Exports.cs:121-137`。`Thumbnail` は `Count` の後に `ThumbnailContext` で観測する — `ENG/Exports.cs:43-56`、`ENG/ThumbnailContext.cs:4-27`。
- 値を渡さない `record` は必ず較正 (`measureTogether`) を伴い、値を渡す `recordValues` は較正しない — `engine-worker.mjs:71-75`。Quiz のプレビュー・絵あて・記録庫のコードは `sortMs` を読まない (`quiz-preview.ts`、`painting-quiz.ts`、`world-dex.ts` に `sortMs` の参照は 0 件)。

**待ち時間の内訳 (NetWasm の実測、v2 の文書)**
- ジョブ時間 (Worker 応答全体)、記録、純ソートの中央値: 128 要素でジョブ 84〜94 ms に対して記録 0.3〜0.8 ms。4,096 要素で QuickSort はジョブ 100.8 ms・記録 8.2 ms、HeapSort はジョブ 101.1 ms・記録 19.3 ms、TimSort はジョブ 124.9 ms・記録 13.3 ms — `SV/.github/docs/references/static-ui-m4-final-comparison-2026-11-16.md:94-103`。同じ傾向は `static-ui-m3-comparison-2026-11-15.md:122-131` にもある。
- 記録 API を単体で測った値は、QuickSort/random/4,096 が 7.9 ms、HeapSort/random/4,096 が 18.3 ms — `SV/.github/docs/references/static-ui-m8-final-comparison-2026-11-17.md:182,185`。
- 較正は 10 ms 以上になるバッチの長さを目標に反復数を決め、本計測は 5 ラウンドある — `engine-worker.mjs:94-151`。Ranking は 9 ms 以上のバッチを 1 本取り、最大 5 標本か累計 50 ms まで測る。全体の上限は `max(900, 算法数×200)` ms — `WEB/src/lib/ranking-measurement.ts:24-28,76-101,109-122`。
- Visualizer の既定サイズは 1,024 — `WEB/src/lib/visualizer-url.ts:32`。カードは最大 9 枚 — `WEB/src/lib/catalog.ts:3`。

**数値の忠実度 (v2 の現状)**
- 速度ランキングは旧 Blazor 比の幾何平均で random 1.54 倍、sorted 1.70 倍遅い — `SV/.github/plans/v2_issues.md:312`。
- Quiz「いちばん速いのは?」の正解は旧版と約 86% 一致する。ユーザーは「Visualizer とランキングの時間が旧版に近いこと」を優先して判断した — `v2_issues.md:291-304`。
- 遅れの原因は 3 つある。V8 の未最適化 tier、`WebAssembly.compile` (streaming でない) によって code cache が効かないこと、NetWasm のコード生成 (24 B の `SortSpan` の複製) — `v2_issues.md:206-214`。

**NAOT-LLVM の測定値 (`AUD/measurements.md`)**
- 高速経路は n=100,000 random、6 算法の幾何平均の生値で、NAOT-LLVM 35.9 ns/要素、NetWasm O3 59.7、NetWasm `IgnoreAggressive` (v2 の設定) 66.3、Mono AOT 58.7 — `AUD/measurements.md:187`。
- 観測経路 (`ISortContext`、n=100,000 random、ns/要素) は NAOT-LLVM と NetWasm `IgnoreAggressive` の順に、PDQSort 355 / 113、TimSort 679 / 216、Driftsort 517 / 165、HeapSort 782 / 404、ShellSortCiura2001 603 / 183、RadixLSD256Sort 79.8 / 23.5。Mono AOT は 1,362〜3,078 — `AUD/measurements.md:304-311`。
- 初回と定常の差 (PDQSort、n=100,000) は NAOT-LLVM 4.88 / 3.99 ms、NetWasm 10.2 / 5.46 ms — `AUD/measurements.md:205,207`。

### Inferences (推定)
- **高速経路** (NAOT-LLVM は v2 比約 1.8 倍速い): 表示される時間、Ranking の順位、Quiz の正解、Sortube の勝敗が変わる。Mono AOT (旧 Blazor) と比べても約 1.6 倍速い (35.9 対 58.7) ので、値は旧版より速い方向へずれる。v2 で問題になった「旧版との一致」(約 86%) は、別の歪み方の下で測り直すことになる。目標を「旧版に近い」と「ネイティブに近い」のどちらに置くかは製品判断である。
- **高速経路と待ち時間**: 較正も Ranking も時間予算型なので、高速経路が速くなっても待ち時間はほとんど縮まない。縮むのは初回の立ち上がり (V8 の階層化、起動) である。LLVM で最適化済みのコードは Liftoff でも比較的速い、という v2 文書の説明 (`v2_issues.md:209`) は NAOT-LLVM にも当てはまるはずで、測定値の初回/定常比 1.22 (NetWasm は 1.87) もこれと整合する。
- **観測経路** (NAOT-LLVM は v2 比約 3 倍遅い。6 算法の幾何平均で 2.9 倍): この比が記録にもそのまま当てはまると、4,096 要素の HeapSort の記録は 19 ms から約 55 ms に、ジョブは約 100 ms から約 135 ms になる。900 万操作近い記録 (NetWasm で約 1 秒と見積もる) は約 3 秒になる。ただし実際の記録は `List<SortOperation>.Add` と read の吸収処理が操作ごとに入り、測定したカウンタだけのコンテキストとはコストの構成が違う (Gaps)。
- **観測経路の影響を受けやすい機能**: Explore のサムネイル (1 枚 2 パス、逐次)、Quiz の計算量問題 (`Count` 3〜4 回)、大きな入力の Visualizer と Sortube。既定サイズ前後の Visualizer、Basics、Quiz プレビューでは、較正か配列の小ささのために差が見えにくい。
- **エンジンに依存しない改善点**: Quiz プレビューの値なし経路、絵あて、記録庫では、使われない較正が毎回約 80〜100 ms 走っていると見られる。`recordValues` 相当の「較正なし記録」にすれば、どのツールチェーンでも待ち時間が縮む (本監査の範囲外)。
- **Mono AOT (`main`) との比較**: NAOT-LLVM は高速経路で約 1.6 倍、観測経路で約 3.4〜7.6 倍速い (PDQSort 1,362 対 355 など)。ただし NAOT-LLVM は Blazor を動かせないので (`AUD/nativeaot_llvm.md:259`)、`main` の Mono AOT を置き換えるには v2 の構成 (独立エンジン + Worker) へ移ることが前提になる。

### Gaps
- `OperationRecordingContext` (`List.Add` と `ReadCoalescer`) を NAOT-LLVM で動かした記録時間は測っていない。観測経路の倍率が記録にもそのまま当てはまるかは不明である。
- 観測経路が遅い原因は特定していない。`AUD` の表では sealed class の `StatisticsContext` でも同じ倍率 (PDQSort 8.69 倍) なので、インターフェース呼び出しよりも、観測経路の `SortSpan` の処理そのものが原因と見られる。候補は 2 つある。`ReportCompare*` は `#if NETWASM` のときだけ `AggressiveInlining` が付くので (`SVsub/src/SortAlgorithm/Algorithms/SortSpan.cs:451-503`)、NAOT-LLVM ではこの指定がない。もう 1 つは、GC 参照を含む struct を shadow stack で渡す ABI (`AUD/nativeaot_llvm.md:248`) である。
- 32,768 要素の記録時間と待ち時間の分布は、v2 の文書に体系的な表がない。

---

## Q2. export 面の移植

### Takeaway
9 個の `[JSExport]` の引数と戻り値は、`int`、`bool`、`int[]`、`byte[]`、`double[]` だけで、文字列はない。JS から呼ぶ import は時計 1 つだけである。`[UnmanagedCallersOnly]` へ移すときの方針は 4 つ。(1) ポインタは C# から JS への一方向だけに流し、JS は長さと整数だけを渡す。(2) 入力は C# が持つ pinned の作業領域へ JS が書き込む。(3) 可変長の結果は C# が持つ「次の呼び出しまで有効」な結果領域に置き、JS がすぐに複製する。(4) 例外は export 内ですべて状態コードに変換し、境界を越えさせない。NetWasm と同じ形の `open()` を返す JS ファサードを置けば、TypeScript 側の変更は Worker のごく一部、Quiz Worker、ツール 2 本、`prepare.mjs` に限られる。

### Cited Findings (確認済み)

**現行の export 面** (C# は `ENG/Exports.cs`、JS の型は NetWasm が生成した `OUT/SortKernel.d.mts`)

| export | C# のシグネチャと行 | JS 側の型 | 検証と失敗時の動作 |
|---|---|---|---|
| `Input` | `int[] Input(int size, int seed, int pattern)` `:17-23` | `(number, number, number) => Int32Array` | `size` は 1 から `PatternMax(pattern)` まで。未知の pattern は `ArgumentOutOfRangeException` (`ENG/Dispatch.cs:964-1037`) |
| `Shuffle` | `int[] Shuffle(int size, int seed)` `:29-39` | `=> Int32Array` | `size` は 1〜32,768 |
| `Thumbnail` | `int[] Thumbnail(int id, int[] input)` `:43-56` | `(number, Int32Array) => Int32Array` | `Validate` (長さは `AlgorithmMax(id)` 以下、未知の id は例外、`:66-70`、`ENG/Dispatch.cs:874-963`) |
| `LastMetrics` | `double[] LastMetrics()` `:63-64` | `() => Float64Array` | 直前の記録の値 5 個 (`:175`) |
| `Sort` | `int[] Sort(int id, int[] input)` `:73-79` | `=> Int32Array` | `Validate`。引数の配列をその場でソートして返す |
| `Batch` | `double[] Batch(int id, int[] input, int runs)` `:83-100` | `=> Float64Array` (ms、先頭、末尾) | `runs` は 1〜100,000 |
| `CopyBatch` | `double[] CopyBatch(int[] input, int runs)` `:106-117` | `=> Float64Array` | 長さ 32,768 以下、`runs` は 1〜100,000 |
| `Count` | `int Count(int id, int[] input, bool coalesce)` `:121-137` | `=> number` | 上限超過は -1。**入力をその場でソートする** |
| `Record` | `byte[] Record(int id, int[] input, bool coalesce, int count)` `:145-182` | `=> Uint8Array` | `count` は -1 から 9,000,000。上限超過は空配列 |
| import `now` | `[JSImport("now", "poc.clock")] double Now()` `:58-59` | `performance.now()` (`ENG/clock.mjs:1-3`、`ENG/SortKernel.csproj:21`) | — |

- 返すバイナリは 1 本だけで、version 付きの Int32 word 列である。統計 4 個は 64 bit を 2 word に分けて入れる — `ENG/PocCodec.cs:15-25,60-65`。デコーダーは構造を検証する — `WEB/src/lib/frame.ts:7-37`。
- Worker は戻り値を `frame.slice().buffer` で複製してから transfer する。理由はコメントに「実行時のメモリを detach させない」とある — `engine-worker.mjs:158-160`。
- 引数の例外は JS に `NetWasmManagedError` として届き、検証ツールはこの名前に依存している — `WEB/tools/m2-contracts.mjs:83,136-137`。ツールは同じインスタンスで不正入力を 13 回呼んだ後も、そのまま 1,775 件と 16,704 件の照合を続ける — `m2-contracts.mjs:44-94,110-180`。
- `Record` の空配列は Worker が `"Recording limit exceeded"` に変換する — `engine-worker.mjs:157`。Sortube はこの文言を見て配列を半分にして再試行する — `sortube-preparation.ts:78-100`。
- モジュールは `contract.module` を動的 import し、`open()` で開く — `engine-worker.mjs:22,44`、`ranking-worker.ts:48`、`quiz-worker.ts:40-41`。Worker 側の型は `any` で、生成された `.d.mts` は使っていない — `ranking-worker.ts:11`。
- テストは `open()` が返すモック kernel を差し込む — `WEB/src/lib/engine-worker-timing.test.mjs:14-38`。

**NAOT-LLVM の制約** (`AUD/`)
- export は static でなければならず、引数はプリミティブか値型だけ、例外を外へ漏らせない — `AUD/nativeaot_llvm.md:122`。ジェネリックにもできない (`:136`、一般知識)。
- `Module.HEAP32` は `-sEXPORTED_RUNTIME_METHODS` で公開し、メモリが拡張されると張り替えられる — `AUD/measurements.md:460,484`、`AUD/nativeaot_llvm.md:129-131`。
- 空の export の呼び出しは 9 ns (NetWasm は 104 ns) — `AUD/measurements.md:424-426`。
- `DotNetJsApi=true` にすれば `[JSExport]` も動いた。ただし Brotli で 545 KiB、起動 125 ms、空 export 87 ns で、文書にない機能である — `AUD/measurements.md:346,477`。

### 設計案: `[UnmanagedCallersOnly]` と線形メモリのプロトコル (推定を含む設計)

**バッファの所有**
- すべてのバッファを C# が所有する。モジュールの寿命の間、場所を変えない領域を 2 つ用意する。
  - 入力用の作業領域 `staging`: `int[32768]`。`GC.AllocateArray<int>(32768, pinned: true)` で確保する。
  - 数値の返却領域 `metrics`: `double[8]`。`Batch` と `CopyBatch` の 3 値と、`LastMetrics` の 5 値に使う。
- 可変長の結果は、C# が持つ `result` スロット (POH 上の pinned 配列) 1 つに置く。結果は **次の export 呼び出しまで有効** とする。
- JS は `sv_staging()`、`sv_metrics()`、`sv_result()` から**アドレスを受け取るだけ**で、JS からポインタを渡す引数は 1 つもない。任意のアドレスを渡せる、という種類の誤りが設計上存在しなくなる。
- `staging` は全操作にとって読み取り専用とする。`Count` と `Sort` は現在、引数をその場で書き換えるので、作業用の配列へ複製してから処理する。NetWasm は呼び出しのたびに `int[]` を複製するので、JS 側の配列が書き換わらない。その意味論をここで保つ。

**長さ**
- 計算する export は `(id, length, ...)` を取り、C# が `0 ≤ length ≤ staging.Length` と `AlgorithmMax` を検査してから `staging.AsSpan(0, length)` を作る。
- 結果の長さは戻り値で返す。0 以上なら要素数かバイト数、負なら状態コードである。

**可変長の記録の返却**
- `PocCodec` を 2 段に分ける。長さの計算 (今の `count` の計算、`checked` 付き、`ENG/PocCodec.cs:22-25`) と、`Span<int>` への書き込みである。
- `sv_record` は計算した長さで `GC.AllocateUninitializedArray<byte>(n, pinned: true)` を確保し、`result` へ直接エンコードしてバイト数を返す。
- JS は `HEAPU8.slice(p, p + n)` で自分の `ArrayBuffer` に複製し、その後 `sv_release()` を呼んで大きな記録を GC に返す。
- 複製は 1 回で済む。今の Worker の `frame.slice()` (`engine-worker.mjs:160`) は不要になる。NetWasm の実行時の複製 (`AUD/repo_audit.md:173`、推定) と合わせて 2 回あった複製が 1 回に減る。
- 線形メモリのビューは transfer list に入れない。

**例外を越えさせない失敗の通知**
- 各 export の本体全体を `try/catch` で囲み、状態コードを返す。
  - `-1` = 上限超過。今の `Count` の -1 と同じ意味にそろえる。
  - `-2` = 引数の誤り。
  - `-3` = メモリ不足。
  - `-4` = 内部エラー。
- 診断のために `sv_last_error()` と、任意で UTF-8 のメッセージ領域を置く。
- `OperationLimitExceededException` は今どおりエンジンの内部で捕まえる (`ENG/Exports.cs:133,178`)。
- export 呼び出しが JS 例外になった場合は、モジュール全体を使用不能と見なす。これは Wasm の trap、`abort`、スタックの枯渇など、C# の `catch` が届かない失敗である。ファサードは以後すべての呼び出しで `EngineFatalError` を投げる。

**ファサード**
- 出力ディレクトリに `SortKernel.mjs` を手書きで置き、`open()` が NetWasm 版と同じメソッド名と戻り値の型を返すようにする。メソッドは `Input`、`Shuffle`、`Thumbnail`、`Sort`、`Batch`、`CopyBatch`、`Count`、`Record`、`LastMetrics` で、戻り値は `Int32Array`、`Float64Array`、`Uint8Array`、`number` である。
- 引数の誤りは名前付きの `Error` を投げる。上限超過は今どおり空の `Uint8Array` か -1 を返す。

```js
// 設計スケッチ (未ビルド)。Emscripten は MODULARIZE + EXPORT_ES6 で出力する。
import createNative from "./SortKernel.native.mjs";
export async function open() {
  const M = await createNative();
  const CAP = 32768, staging = M._sv_staging() >>> 2, metrics = M._sv_metrics() >>> 3;
  let fatal = null;
  const i32 = (v) => { if (!Number.isInteger(v) || v < -2147483648 || v > 2147483647) throw argument(); return v; };
  const put = (a) => { if (!(a instanceof Int32Array) || a.length > CAP) throw argument(); M.HEAP32.set(a, staging); return a.length; };
  const call = (f) => { if (fatal) throw fatal; try { return f(); } catch (e) { fatal = engineFatal(e); throw fatal; } };
  // HEAP32/HEAPU8 は呼び出しのたびに読み直す。メモリ拡張でビューが張り替えられるため。
  const ints = (n) => { status(n); const p = M._sv_result() >>> 2; return M.HEAP32.slice(p, p + n); };
  return {
    Input: (size, seed, pattern) => call(() => ints(M._sv_input(i32(size), i32(seed), i32(pattern)))),
    Record: (id, input, coalesce, count) => call(() => {
      const n = M._sv_record(i32(id), put(input), coalesce ? 1 : 0, i32(count));
      if (n === -1) return new Uint8Array(0);
      status(n);
      const p = M._sv_result(), bytes = M.HEAPU8.slice(p, p + n);
      M._sv_release();
      return bytes;
    }),
    // Shuffle / Thumbnail / Sort / Batch / CopyBatch / Count / LastMetrics も同じ形
  };
}
```

```csharp
// 設計スケッチ (未ビルド)。検証と処理は共通のコアに置き、ここは変換だけを担う。
[UnmanagedCallersOnly(EntryPoint = "sv_record")]
public static int Record(int id, int length, int coalesce, int count)
{
    try { return EngineCore.Record(id, Staging(length), Flag(coalesce), count, ref result, metrics); }
    catch (ArgumentException) { return Status.Argument; }
    catch (OutOfMemoryException) { return Status.OutOfMemory; }
    catch (Exception) { return Status.Internal; }
}
```

**時計**
- 選択肢は 2 つある。
  - (a) `emscripten_get_now` を `DirectPInvoke` で静的にリンクする。Emscripten の JS 連携 API は DirectPInvoke の対象であり、`AUD/nativeaot_llvm.md:124` に記載がある。
  - (b) `Stopwatch` を使う。
- 計測の意味を保つには、ループの内側で時間を測る今の形 (`ENG/Exports.cs:91-98`) を各ヘッドで維持するのがよい。JS 側で時間を測る形にすると、NetWasm では呼び出しごとの `int[]` の複製が計測に入るので、ツールチェーンをまたいで比較できなくなる。

### TypeScript 側の変更の見積もり (推定)

| 対象 | 変更 | 規模の目安 |
|---|---|---|
| `SortKernel.mjs` (新規ファサード) と型定義 | 上記。Emscripten の factory を包み、検証、状態コードの変換、使用不能状態の管理を担う | 150〜250 行 + `.d.ts` 約 40 行 + 単体テスト 100〜200 行 |
| `public/engine-worker.mjs` | `options.size`/`seed` の整数検査を足す (今は `app.Input` へそのまま渡している、`:44-47`)。`frame.slice()` を外す (`:160`) | 5〜15 行 |
| `src/lib/quiz-worker.ts` | `"reject"` モードで実行時のインスタンスを使い回すので (`quiz-computation.ts:12-14`、`quiz-worker.ts:40-41`)、使用不能エラーのときに `runtime` を捨てる | 約 5 行 |
| `src/lib/ranking-worker.ts` | なし (`"cancel"` モードで、エラー時に Worker を終了する、`worker-channel.ts:56-64,114-123`) | 0 |
| `src/lib/engine-client.ts`、`worker-channel.ts` | なし (プロトコル、版、タイムアウト、終了は今のまま) | 0 |
| `tools/m2-contracts.mjs` | `NetWasmManagedError` の判定を中立の名前に変える (`:83,136-137`) | 約 4 行 |
| `tools/m8-dx-registry.mjs` | 一時 Registry でエンジンをビルドする箇所 (`:131`) を NAOT ヘッドと Emscripten 環境に合わせる | 15〜30 行 |
| `tools/m5-oz-performance.mjs` 等 | `/SortKernel.mjs` の `open()` を呼ぶだけなので、ファサードが互換なら変更なし (`:21-23`) | 0 |
| `tools/prepare.mjs` | `publishEngine` (`:148-204`) のビルド対象、RID、環境変数を変え、ファサードを複写する | 30〜60 行 |

### きれいに対応するものがない機能
- NetWasm は managed 例外を、型名つきの JS 例外 (`NetWasmManagedError`、`managedType`) にして返す。NAOT-LLVM では状態コードとメッセージ領域を自作する必要がある。
- NetWasm では GC 配列を「所有権付きの型付き配列」として返せる。NAOT-LLVM では pinned 領域から JS が複製する形になり、「次の呼び出しまで有効」という寿命の規則を自分で守る必要がある。
- `[JSImport]` の自動マーシャリングはなく、DirectPInvoke か `WasmImportLinkage` に置き換える。
- `bool` の受け渡しは int の 0/1 になる。
- NetWasm の `open(options)` のフック (`platform.now/random`、`stdout`/`stderr`、`onNotification`)、`close()`、`failure` は、Emscripten の Module オプション (`print`/`printErr`) で部分的にしか代替できない (`OUT/SortKernel.d.mts`)。製品のコードは `close()` も `failure` も使っていない (grep で 0 件)。
- `.d.mts` の自動生成はなく、型定義は手書きになる。

### Gaps
- Emscripten 3.1.56 の `EXPORT_ES6` と `ENVIRONMENT=web,worker` の組み合わせで、モジュール Worker から動的 import できるかは、実際に確かめていない。
- pinned の POH 配列に対するアドレスの取得と、NAOT-LLVM の保守的 GC が組み合わさったときの挙動は確かめていない。
- `UnmanagedCallersOnly` から `nint` を返したときに、JS に負の数として見えないか (アドレスが 2 GiB 以上になる場合) は確かめていない。上限が 2 GiB なら問題にならないはずである。

---

## Q3. 信頼できない入力とセキュリティ上の位置づけ

### Takeaway
利用者が操作できる値 (URL、共有リンク、localStorage と sessionStorage) は、UI 層で「許可リストに載った ID と範囲の決まった整数」に変換されてから Worker に届く。任意の配列は URL からは入らない。Worker に届く配列は、エンジン自身が生成したものか、同じ origin の生成データである。検証は 4 層ある。EngineClient、Worker、C# の export、生成された上限表である。NAOT-LLVM へ移ってもこの攻撃面は変わらない。増えるのは堅牢性の不変条件で、ポインタ、長さ、ビューの鮮度、使用不能状態の扱いである。CSP も COOP/COEP もリポジトリには設定がない。

### Cited Findings (確認済み)

**利用者が操作できる入口 → エンジン**
- `/visualizer` の URL: `algo`、`cards`、`pattern`、`mode` はカタログの許可リストで照合する。`size` は 1〜32,768、`seed` は 0〜2³¹−1 の整数だけを受け付ける。`cards` は 9 枚まで、値は 2,048 文字まで — `WEB/src/lib/visualizer-url.ts:17-27,55-91`。最後に算法と入力パターンの上限で絞る — `:97-103`。
- Sortube の URL と動画 ID: 形式は `v1:algorithm:pattern:size:mode` で、算法、パターン、mode は許可リストで照合する。`size` は 2³¹−1 以下 — `WEB/src/lib/sortube-url.ts:39-56`。記録の前に、算法とパターンの上限の最小値まで縮める — `sortube-preparation.ts:72-77`。
- 保存された状態: 絵あてのセッションは長さ、ID、`size`、`seed` の範囲を厳しく検証する — `WEB/src/lib/painting-quiz.ts:180-210`。
- Quiz の解説: `arraySize` は 12〜64 に丸める。`arraySeed` は `Number(...)` で、NaN や小数なら EngineClient が拒否する — `quiz-explanation.ts:111-118`、`engine-client.ts:85-103`。
- 任意の値の配列が Worker に届く経路は 3 つ。いずれも URL からは入らない。
  - 本棚の並び: エンジンの `Shuffle` の結果と生成データから作る — `basics-bookshelf.ts:149-152,217-220`。
  - 対決演習: 生成データの `definition.values` を使う — `basics-races.ts:219-223`。
  - Quiz: `Input` で生成した値を使う — `quiz-play.ts:474-486`。

**検証の層**
1. EngineClient: 算法とパターンは契約、`size` は min(32,768, 算法の上限, パターンの上限)、`seed` は int32、値の配列は長さと int32 を検査する。穴のある配列も弾く — `engine-client.ts:85-125,175-183`。応答も検証する — `:146-157`。
2. Worker: プロトコルと revision、算法とパターンが契約に含まれるか、配列は `Array`・長さ 1〜32,768・int32 かを検査する — `engine-worker.mjs:12-13,31-43,168-172`。**ただし `options.size` と `options.seed` は検査せずに `app.Input` へ渡す** — `:45-47`。Ranking Worker と Quiz Worker は自分で範囲を検査する — `ranking-worker.ts:16-30`、`quiz-worker.ts:20-39`。
3. C# の export: `size`、`AlgorithmMax`、`runs`、`count` の範囲を検査する — `ENG/Exports.cs:20-21,32-33,66-70,87-88,109-110,149-150`。
4. 上限表は Registry から生成する (`ENG/Dispatch.cs:874-1037`、`// <auto-generated />` `:1`)。記録は 900 万操作で止まる — `ENG/Exports.cs:9`。

**DoS の上限**: 120 秒でタイムアウトし、取り消しは Worker の terminate で行う — `worker-channel.ts:78-81,114-123`。

**出力の受け手**
- フレームのデコーダーは version、範囲、操作種別、末尾を検証する — `frame.ts:7-37`。
- エンジンの出力が HTML へ流れる経路は見つからなかった。`innerHTML` の書き込み先 (`basics-*.ts`、`lesson-player.ts`、`browse-controller.ts` ほか) に渡るのは生成データであり、エンジンの出力ではない。

**ヘッダーと CSP**
- v2 の `WEB/public/_headers` は `Cache-Control` しか設定していない。`/engine-worker.mjs` は `max-age=0, must-revalidate`、`/engine-contract.json` は `no-store` — `WEB/public/_headers:1-11`。
- `SV@main:src/SortVivo/wwwroot/_headers` も `Cache-Control` と、`.wasm.br` の `Content-Type`/`Content-Encoding` だけである。
- `Content-Security-Policy`、`wasm-unsafe-eval`、`Cross-Origin-Opener-Policy`/`Embedder-Policy` は、両ブランチの設定・HTML・Functions に 0 件 (`git grep`)。COOP/COEP が出てくるのは仕様書の OffscreenCanvas の脚注だけである — `SV/.github/docs/specs/performance-rendering.md:369`。
- `SharedArrayBuffer` は必須にしない方針 — `SV/.github/docs/plans/static-ui-wasm-migration.md:151`。

### 生のポインタと長さを使う export で必要になる不変条件と、守る場所 (設計、推定)

| # | 不変条件 | 守る場所 |
|---|---|---|
| I1 | JS はポインタを渡さない。アドレスは C# から JS への一方向で、C# が所有する pinned 領域だけを指す | C# のヘッド (API の形)、ファサード |
| I2 | C# は JS から来た長さで `Span` を作る前に、自分が持つ容量と `AlgorithmMax` に照らして検査する。`new Span<int>(ptr, jsLength)` は書かない | C# のヘッドと共通コア |
| I3 | JS の数値は int32 の整数かどうかをファサードが検査する。Wasm の JS API は i32 の引数を ToInt32 で切り詰めるので (一般知識)、`size = 2**32 + 5` が黙って 5 になる。NetWasm で同じ入力を与えたときの挙動は確かめていない | ファサード、`engine-worker.mjs` (`options.size`/`seed` の検査を足す) |
| I4 | 作業領域 `staging` は読み取り専用で、書き換える処理は複製してから行う。JS は `CAP` を超えて `HEAP32.set` しない | C# のヘッド、ファサード |
| I5 | 結果は呼び出しの直後に JS が複製する。線形メモリのビューを呼び出しをまたいで持たない。メモリ拡張で張り替わるので、`M.HEAP32` を毎回読み直す。ビューを transfer しない | ファサード |
| I6 | 例外は export の境界を越えない。trap、abort、スタックの枯渇のあとはモジュールを捨てる | C# のヘッド (全体を catch)、ファサード (使用不能状態)、Quiz Worker (`runtime` を捨てる) |
| I7 | 上限 (算法とパターンの上限、`runs`、`count`、900 万) は C# のコアが正であり、JS 側の検査は早く失敗させるための重複にすぎない | 共通コア |
| I8 | サイズの計算は `checked` のまま (`ENG/PocCodec.cs:22-25`) | 共通コア |
| I9 | 初期メモリを小さくする。Worker が最大 9 個並ぶ (Q5)。メモリ不足は状態コードで返す | リンク引数、C# のヘッド |

### 本当のセキュリティリスクと、堅牢性・メモリ破壊のリスクの区別 (推定)
- **本当のセキュリティリスク (小さい)**
  - JS と Wasm は同じ origin、同じ Worker の信頼ドメインにある。線形メモリが壊れても Wasm のサンドボックスは越えず、得られる能力は同じ Worker の JS がもともと持つ能力を超えない。エンジンは import を時計しか持たず、ネットワークにも DOM にも触れない。URL から届くのは許可リストの ID と範囲の決まった整数だけなので、ツールチェーンを替えても攻撃面は同じである。
  - 新しく増える実質的なセキュリティ論点は**ビルドのサプライチェーン**である。実験版の feed (`dotnet-experimental`) と、Emscripten 一式 (clang、binaryen、node、python) を CI に入れることになる (Q4)。今は `NU1507` (複数 feed で対応付けがない、という警告) を抑止している (`SV/Directory.Packages.props:5`) ので、`packageSourceMapping` を明示しないと依存関係の取り違えに弱くなる。
  - CSP を入れる場合は、NetWasm (`WebAssembly.compile`) も Emscripten (`instantiateStreaming`) も `script-src 'wasm-unsafe-eval'` を必要とする (一般知識)。Emscripten のグルーコードが `eval` を使わないように `-sDYNAMIC_EXECUTION=0` を付けるべきかも確認が要る (一般知識、未検証)。
- **堅牢性とメモリ破壊のリスク (NAOT-LLVM で増える)**
  - I2〜I6 が破られると、線形メモリの GC ヒープが壊れる。その結果は誤ったフレームになるか trap になる。誤ったフレームでも、構造が整っていれば `frame.ts` の検査を通るので、全件照合のような体系的な検査でしか見つからない。
  - 使用不能になったモジュールを `"reject"` モードの Quiz Worker が使い続ける問題も、NAOT-LLVM 特有の新しい失敗の形である。
  - DoS は今と同じ上限 (900 万操作、120 秒、terminate) で抑えられる。

### Gaps
- NetWasm の生成した JS が、`int` 引数に小数や 2³² 以上の数が来たときにどう変換するかは確かめていない。v2 でも Worker 内の `options.size` は C# の範囲検査だけで守られている。
- Cloudflare Pages が `.wasm` を `application/wasm` で配信するかを、v2 の公開物で確かめていない。`instantiateStreaming` はこの MIME を必要とする (一般知識)。

---

## Q4. ビルドと配信への影響

### Takeaway
今のエンジンは NuGet だけで完結する。`Sdk="NetWasm.Sdk/0.14.2"` を nuget.org から取得し、`npm run prepare` が単一プロジェクトを publish する。CI は ubuntu-24.04 で .NET 11 と Node 24 を使う。NAOT-LLVM に替えると、4 つが新たに必要になる。実験版 feed とパッケージの対応付け、Emscripten 3.1.56 の用意 (CI と全開発者)、net10.0 の別ヘッド、Emscripten 出力を包むファサードである。ハッシュ付きパスと revision の仕組みはそのまま使える。

### Cited Findings (確認済み): 今のパイプライン

**エンジンのプロジェクト** — `ENG/SortKernel.csproj:1-23`
- 設定: `Sdk="NetWasm.Sdk/0.14.2"`、`TargetFramework=netwasm0.1`、`OutputType=JsModule`、`NetWasmJavaScriptHosts=page`、`AllowUnsafeBlocks`、`NetWasmOptimization=O3`、`NetWasmManagedInliningHints=IgnoreAggressive`、`NetWasmGarbageCollector=NetWasm`。
- ソースの取り込み: SortAlgorithm のソースを `Compile Include` で取り込む (`:12`)。`SortVivo.Core` の記録関連 8 ファイルも同様 (`:13-20`)。JS モジュール `poc.clock` を登録する (`:21`)。
- NetWasm 専用のソース:
  - `ENG/InputSequences.cs:3-5`: 対象のランタイムに `Enumerable` がないための互換層。
  - `ENG/GlobalUsings.cs:1`: `global using System.Linq;`。
- ソリューションとの関係: `SV/SortVivo.slnx` にエンジンは含まれない (Blazor 3 プロジェクト、submodule、テストだけ)。
- 全体設定:
  - `SV/Directory.Build.props:3`: 全体を `net11.0` にする。
  - `SV/Directory.Packages.props:3`: 中央パッケージ管理を有効にする。
  - `SV/global.json:2-5`: SDK を `11.0.100` に固定し、`rollForward=latestFeature`。
  - リポジトリに `NuGet.config` はない。

**生成とビルド** — `WEB/package.json` の `"build": "node tools/prepare.mjs && astro build && node tools/localized-pages.mjs"`
- Registry と dispatch の生成: `dotnet run --project tools/MigrationCatalog` — `WEB/tools/prepare.mjs:134-146`。
- エンジン: `dotnet publish ../SortVivo.Engine/SortKernel.csproj -c Release -o ../../artifacts/migration/engine` — `prepare.mjs:148-156`。
- revision: 出力ディレクトリ全体、`catalog`/`patterns` の JSON、`engine-worker.mjs` の SHA-256 から作る (`prepare.mjs:158-178`)。`public/engine/<revision>/` に複写し、`engine-contract.json` に `module: "/engine/<rev>/SortKernel.mjs"` を書く (`:179-203`)。
- 出力ファイル: `SortKernel.mjs`、`.wasm`、`.raw-adapter.mjs`、`.wasm.type-symbols`、`interop.json`、`runtime-layout.json`、`deployment.json`、`netwasm.runtime.*.mjs`、`netwasm.page.mjs` — `OUT/` の一覧。
- Vite は Worker を ES 形式にし (`vite.worker.format: "es"`)、`public/` はそのまま複写する。`minifyPublicScripts` の対象は `js/` と `vendor/` だけ — `WEB/astro.config.mjs:32-35,49`。
- サイズの集計では、`engine/`、`engine-worker.mjs`、`engine-contract.json` をエンジン一式として数える — `WEB/tools/delivery-report.mjs:56-59`。

**CI と配信**

| workflow | 対象 | ランナーと時間 | 内容 | キャッシュ |
|---|---|---|---|---|
| `static-ui.yaml` | PR と push (`main`, `v2`) | ubuntu-24.04、120 分 | `setup-dotnet` (外部アクション `guitarrapc/actions`、`restore-wasm-workload: true`、11.0.x)、Node 24、`npm ci`、format、Playwright **chromium のみ**、`npm run quality`、旧版の固定参照を publish、`quality:compatibility` | `setup-node` の npm キャッシュだけを確認 (NuGet キャッシュの有無は外部アクションの中身を読んでいないので不明) |
| `deploy-static-preview.yaml` | push (`v2`) と同一リポジトリの PR | ubuntu-24.04、90 分 | `publish:local` (prepare とエンジンのビルドを含む)、`prepare:fixtures`、`test:release`、`verify:local` の後、`tools/deploy-pages-preview.mjs` で Cloudflare Pages の preview ブランチ `netwasm-*` へ配信し、検証する | 同上 |
| `deploy.yaml` | push (`main`) | ubuntu-24.04、15 分 | Blazor を publish して `wrangler pages deploy ./publish/wwwroot --project-name=sortvivo` | — |
| `build.yaml` | `main` | ubuntu-24.04 | `dotnet build`/`test`、E2E を 4 分割 | — |

- 配信先は Cloudflare Pages で、1 ファイル 25 MiB の制限がある — `SV/.github/docs/architecture.md:27`。本番の切り替えはまだしていない — `static-ui-wasm-migration.md:5`。

**NAOT-LLVM の前提** (`AUD/`)
- feed は `dotnet-experimental` で、最新は `10.0.0-rc.1.26357.1`。11.x のパッケージはない — `AUD/nativeaot_llvm.md:8,19,50`。
- ホスト用パッケージは `runtime.<host RID>.Microsoft.DotNet.ILCompiler.LLVM` で、Windows x64 と Linux x64 がある — `AUD/nativeaot_llvm.md:49,52`。
- Emscripten は 3.1.56 にほぼ固定で、runtimelab の CI は emsdk のコミット `ca7b40ae22` を使う。Windows では新しい emsdk で `emcc.bat` がない問題がある — `AUD/nativeaot_llvm.md:53-54`。
- 手順: `PublishTrimmed=true`、`SelfContained=true`、`MSBuildEnableWorkloadResolver=false`、`dotnet publish -r browser-wasm` — `AUD/nativeaot_llvm.md:57-63`。
- `net11.0` ではビルドできない (CoreLib の上書きエラー)。`OptimizationPreference=Speed` は ILC の例外で落ちる — `AUD/measurements.md:34-35`。
- Emscripten は、Windows でインストール済みの `wasm-tools-net10` workload の 3.1.56 パックを使い回し、`EM_CONFIG` と `EM_CACHE` をプロセス内で設定して動かした — `AUD/measurements.md:36,642-666`。
- 9 算法のクリーンビルドは 5.9〜9.6 秒 (NetWasm は 8.7 秒) — `AUD/measurements.md:61,64-65`。
- 既定のリンクは `-O1` で、wasm-opt を走らせない — `AUD/nativeaot_llvm.md:155`。
- 出力: ライブラリなら `.js` と `.wasm` — `AUD/nativeaot_llvm.md:66`。測定用の csproj は `-sMODULARIZE=1 -sEXPORT_NAME=... -sENVIRONMENT=node,web -sEXPORTED_RUNTIME_METHODS=HEAP32,HEAPU8 -sINITIAL_MEMORY=134217728` を使った — `AUD/measurements.md:864-868`。

### Inferences (推定): NAOT-LLVM で変わること

1. **実験版 feed**
   - ルートに `NuGet.config` を新設し、`dotnet-experimental` に 4 パッケージ ID だけを対応付ける (`packageSourceMapping`、`AUD/measurements.md:670-692` と同じ形)。
   - このファイルはリポジトリ全体の restore に効くので、Blazor (`main`) やテストの依存がこの feed から来ないことを対応付けで保証する。
   - `Directory.Packages.props` に、版を厳密に固定したエントリを足す。
2. **Emscripten 3.1.56 (CI)**: ubuntu-24.04 では次のどちらかが要る。
   - (a) emsdk を 3.1.56 で install と activate し、`actions/cache` で emsdk と `EM_CACHE` を保持する。
   - (b) `dotnet workload install wasm-tools-net10` の Emscripten パックを使い回し、`AUD` の `emenv.sh` 相当で環境変数を設定する。Linux で使い回せるかは未確認で、Linux 用の Python パックの有無も確かめていない。
   - どちらでも、初回は Emscripten がシステムライブラリをビルドするので時間がかかり、`EM_CACHE` のキャッシュが要る。ILCompiler.LLVM のホスト用パッケージも大きいので、NuGet のキャッシュを足すとよい。
3. **Emscripten 3.1.56 (開発環境)**
   - `npm run prepare` と `predev` (`tools/ensure-generated.mjs`) がエンジンを再生成する場面では、全開発者に Emscripten が要る。今の NetWasm は NuGet だけで済み、計画の「SDK/Node/package manager の版、生成コマンド、追加手順をリポジトリ内で統一」(`static-ui-wasm-migration.md:174`) を満たしている。この利点が失われるので、環境を用意するスクリプトが必要になる。
   - Windows では #3273 (`emcc.bat` の問題) を避けるため、3.1.56 に固定する。
4. **TFM**
   - NAOT ヘッドは `TargetFramework=net10.0` で、`Directory.Build.props` の `net11.0` を上書きする。
   - ライブラリはソースで取り込むので (`SortKernel.csproj:12`)、submodule の csproj の TFM (`SVsub/Directory.Build.props:3` は `net10.0`、SortAlgorithms の HEAD は `net11.0`) は直接は関係しない。制約になるのは、**ライブラリのソースが net10 の BCL と言語版でコンパイルできること**である。HEAD `f674c43` は net10.0 と `LangVersion=preview` で、ソースの非互換なしにビルドできた (`AUD/measurements.md:34,834`)。
   - 将来 SortAlgorithms が net11 にしかない API を使うと、NAOT ヘッドが壊れる。SortAlgorithms 側に net10 でコンパイルできるかを確かめる仕組みが要る。リポジトリをまたぐ制約である。
5. **`MSBuildEnableWorkloadResolver=false`**: NAOT ヘッドの csproj にだけ書く。このヘッドは `SortVivo.slnx` の外にあり、`prepare.mjs` が単独で publish するので、ソリューションのビルドや `restore-wasm-workload` とは干渉しないと見られる。SDK 11.0.100 の上でこの設定を使った net10.0 の publish は、`AUD` の環境 (`AUD/measurements.md:571`) で通った。
6. **出力形式と Worker での読み込み**
   - リンク引数は `-sMODULARIZE=1 -sEXPORT_ES6=1 -sENVIRONMENT=web,worker -sEXPORTED_RUNTIME_METHODS=HEAP32,HEAPU8,HEAPF64` と、小さめの `-sINITIAL_MEMORY` にする。
   - 出力にファサード `SortKernel.mjs` を複写すれば、`engine-contract.json` の `module` も `engine-worker.mjs:22,44` の `import(...).open()` も変えなくてよい。
   - Emscripten は `.wasm` を `import.meta.url` からの相対 URL で探すので、`/engine/<rev>/` 以下でもそのまま動く。
   - Emscripten は既定で `instantiateStreaming` を使う (一般知識)。そのため、v2 の課題 U1 の (b) (code cache が効かないこと、`v2_issues.md:236,241`) が NAOT-LLVM では自然に解けるかもしれない。Worker 内で V8 の code cache が効くかは、v2 の文書でも未確認である。
7. **キャッシュとハッシュ**: revision はディレクトリ全体のハッシュなので、仕組みはそのまま使える。ただし NAOT-LLVM と Emscripten の出力が**ビルドごとに同じバイト列になるか**は確かめる必要がある。v2 でも revision が意味なく変わる問題を直した経緯がある (`v2_issues.md:321-322`)。`_headers` は変えなくてよい。
8. **最適化の設定**: `OptimizationPreference=Speed` は使えない。リンク時の `-O3` と wasm-opt は文書化されていない `EmccFlags` で足すことになる (`AUD/measurements.md:852-856`)。
9. **ツール**: `m8-dx-registry.mjs:131` のエンジンのビルドも、NAOT 用の環境で動かす必要がある。

### Gaps
- `guitarrapc/actions` の `setup-dotnet` が NuGet をキャッシュするか、`restore-wasm-workload` が何を入れるかは確認していない (外部リポジトリ)。
- 87 算法 × 2 系統の NAOT-LLVM ビルドの時間と、Linux CI での成否は測っていない。

---

## Q5. SortVivo 自身の要件と NAOT-LLVM の適合

### Takeaway
最も確度の高い障害は**配信サイズ**である。全体予算の残りは約 2 KB しかない。単純な外挿では、NAOT-LLVM のエンジンは今の NetWasm のエンジン (約 471 KB) より約 190 KB 大きく、エンジン単体の予算 (614,400 B) と「旧版比 70% 減」も割る見込みである。ブラウザーの下限は下がる (legacy EH)。900 万操作の上限は理論上は 2 GiB に収まるが、実測はない。NAOT-LLVM を評価した記録は Sortvivo2 にもリポジトリ外の PoC 記録にもない。

### Cited Findings (確認済み): 要件

| 要件 | 出典 |
|---|---|
| エンジン一式 ≤ 614,400 B、初期 Visualizer ≤ 1,048,576 B、全体 ≤ 1,589,248 B、旧版比 70% 減 (旧版は 5,569,166 B) | `WEB/tools/delivery-budgets.json:2-13`、`static-ui-wasm-migration.md:58-66` |
| 全体予算は 2026-11-18 に一時的に 1,589,248 B へ上げた | `static-ui-wasm-migration.md:66` |
| 0.14.2 にした後の全体は 1,587,152 B で、**残りは 2,096 B** | `v2_issues.md:310` |
| エンジン一式: 461,598 B (M8) → 466,440 B (dispatch 分割後) → さらに wasm +4,876 B (0.14.2) | `static-ui-m8-final-comparison-2026-11-17.md:49`、`v2_issues.md:290,310` |
| wasm の中身: 観測経路のソート約 49%、高速経路のソート約 31%、BCL 相当約 9% | `v2_issues.md:199` |
| 初期 Visualizer は今 738,660 B | `static-ui-m8-final-comparison-2026-11-17.md:49` |
| Mono/CoreCLR、Blazor boot、公式の .NET browser runtime、Webcil の配信はゼロ | `static-ui-wasm-migration.md:64` |
| ブラウザー: 今の仕様を引き継ぎ、命令セットへの対応を検証する。Chromium だけの合格を全ブラウザー対応としない | `static-ui-wasm-migration.md:125` |
| .NET 11 の Mono は exnref を必要とし、Chrome/Edge 137+、Firefox 131+、Safari 18.4+ が要る | `SV/.github/docs/architecture.md:37` |
| 実機、対応ブラウザー、実 CDN の確認は M9 で、まだ行っていない | `static-ui-wasm-migration.md:5,231` |
| 記録の上限は 900 万操作。全記録を確保してから再生する | `ENG/Exports.cs:9`、`static-ui-engine.md:11` |
| これまでの全件照合の最大は 8,386,559 操作 | `static-ui-wasm-migration.md:284` |
| 全件照合: 入力 1,775 件、完全記録 16,704 条件 (有効 16,664、長さで拒否 40)。0.14.2 にした後と dispatch 分割後も通過 | `static-ui-wasm-migration.md:284,296`、`v2_issues.md:290,309` |
| UI から生の export を呼ばない。`SharedArrayBuffer` は必須にしない | `static-ui-wasm-migration.md:147,151` |
| カードごとに `PlayerSession` と `EngineClient` (つまり計算 Worker) を持ち、最大 9 枚 | `SortCard.tsx:77`、`player.ts:30`、`catalog.ts:3` |

**NAOT-LLVM の既知の値**: 9 算法カーネルは Brotli で 308 KiB (速度向けの変種で 289 KiB)。同じカーネルの NetWasm は 107 KiB — `AUD/measurements.md:338-341`。Blazor なしの Mono AOT 単体エンジンの試作は 2,124,713 B — `SV/tools/wasm-engine-prototype/README.md:20-25`。

**先行評価の有無**
- Sortvivo2 の全ブランチ (`v2`、`main`、`exp/u1-warm-split`) を `git grep` した。`nativeaot|ilcompiler|runtimelab|aot-llvm|bootsharp|UnmanagedCallersOnly` に当たったのは、「Native AOT の一般的なサイズ設定を Mono WASM の設定とみなさない」という 1 文だけである — `SV/.github/docs/references/wasm-size-investigation-2026-11-13.md:101`。
- PoC の候補表 (公式 .NET WASM AOT、NetWasm、NetWasm.UI、C++/Rust、JS/TS) にも NAOT-LLVM はない — `PoC:124-130`。
- CoreCLR はブラウザー構成で 262 倍遅く、不採用の記録がある — `wasm-size-investigation-2026-11-13.md:95`。

### Inferences (推定): 適合表

| 要件 | NAOT-LLVM | 根拠と確度 |
|---|---|---|
| エンジン一式 ≤ 614,400 B | **超過の恐れ** | NetWasm の wasm は、9 算法の 61 KiB から本番の約 408 KiB (412,716 B に 0.14.2 の +4,876 B) に増えた。この増分約 347 KiB が NAOT-LLVM でも同じだと仮定する (加算モデル) と、NAOT-LLVM の wasm は 280〜289 + 347 ≈ 627〜636 KiB になる。JS は 9〜20 KiB、Worker・契約・ファサードは数 KiB で、合計約 640〜660 KiB (約 655〜675 KB)。今のエンジン一式 (約 471 KB) より約 +185〜205 KB 大きい。NAOT-LLVM の算法 1 つ分のコードが NetWasm と比べてどれだけ大きいかは不明なので、確度は低い |
| 全体 ≤ 1,589,248 B (残り 2,096 B) | **ほぼ確実に超過** | エンジンが今より 2 KB 余り大きくなるだけで超える。加算モデルでは約 +185〜205 KB |
| 旧版比 70% 減 (全体 ≤ 1,670,750 B) | **割る見込み** | 今の 1,587,152 B に約 +190 KB を足すと約 1,777 KB で、削減率は約 68% |
| 初期 Visualizer ≤ 1 MiB | 収まる見込み | 今 738,660 B で、約 +200 KB でも 1 MiB 未満 |
| 公式 runtime の配信ゼロ | 解釈が必要 | Mono、CoreCLR、Blazor、Webcil は含まない (`DotNetJsApi` を使わない場合)。runtimelab の NativeAOT ランタイムを「公式 .NET browser runtime」に数えるかは判断が要る |
| ブラウザーの下限 | **下がる** | NAOT-LLVM は legacy EH (`try`) と SIMD128 を使う (`AUD/measurements.md:502,524`)。目安は Chrome 95+、Firefox 100+、Safari 16.4+ (`AUD/wasm_standards.md:56-62`)。NetWasm は `try_table` を使うので (`AUD/measurements.md:504`)、Mono と同じく Safari 18.4+ である。ただし legacy EH の提案は inactive に分類されていて、将来残るかは未確認。CI は Chromium しか動かさないので、下限は CI で検証できない |
| 900 万操作 | 理論上は収まるが未検証 | List 約 216 MB + フレーム約 360 MB (`AUD/repo_audit.md:172`) は 2 GiB の範囲内である。NAOT の GC が wasm32 で 200〜360 MB の連続した配列を確保できるか、保守的 GC がどれだけ保持するかは測っていない |
| 複数 Worker (最大 9) | **リスク** | 計算 Worker はカードごとに独立したインスタンスと線形メモリを持つ。NetWasm の線形メモリは最小 3 ページから始まる (`AUD/repo_audit.md:82`)。NAOT-LLVM は最大ヒープ 16 MB で `unreachable` になる報告 (#3150、`AUD/nativeaot_llvm.md:251`) があり、ランタイムの最低限の使用量が数十 MB 規模の可能性がある。その場合、9 枚で数百 MB になる |
| 決定性 (1,775 / 16,704) | 一致が期待されるが要検証 | ソースは同じである。`Random(seed)` の系列は .NET のバージョンをまたいで同じ (`architecture.md:40`)。ただし入力生成 `ArrayPatterns` は `Math.Log` ×10、`Pow` ×8、`Sin`、`Cos` を使う (`SVsub/src/SortAlgorithm/Utils/ArrayPatterns.cs` を grep)。NAOT-LLVM ではこれらが Emscripten の libm を通ると見られ、最下位ビットが違えば入力がずれうる。入力照合が判定の門になる |
| 照合ハーネスの流用 | 可能 | `m2-contracts.mjs` 等は `kernel` の JS API だけを使うので、互換なファサードがあれば、エラー名の 2 行を直すだけで使える |
| 応答 p95 の基準 (`max(10%, 1 frame)`、`static-ui-wasm-migration.md:120`) | 直接は無関係 | 計算は Worker で行う。再生成の完了時間は、記録が長くなれば悪化しうる (Q1) |

### Gaps
- サイズは**最初に測るべき門**である。87 算法の NAOT-LLVM エンジンを 1 回ビルドすれば確定する。
- Safari と Firefox の実機、iOS のメモリ制限での動作は未確認である。v2 自身も M9 で未完了である。

---

## Q6. 移行の手間と後戻りのしやすさ

### Takeaway
今のエンジンは、ソースを取り込む形 (`Compile Include`) と生成された `Dispatch.cs` のおかげで、すでに大部分が中立である。NetWasm に結びついているのは 3 か所だけで、`Exports.cs` の属性と型、時計の import、`Enumerable` の互換層である。共通コア、ツールチェーンごとの薄い export 層、同じ形の JS ファサードの 3 段に分ければ、ツールチェーンの切り替えはビルド時の 1 つの選択になる。照合ハーネスもそのまま使える。この整理は、NAOT-LLVM を採らない場合でも価値がある。

### Cited Findings (確認済み): 結合している場所
- 中立なもの:
  - `ENG/Dispatch.cs` (生成物。`NoInlining` も普通の C#、`:278-451`)。
  - `ENG/ThumbnailContext.cs`。
  - `ENG/PocCodec.cs` (ただし `byte[]` を確保して返す、`:25`)。
  - 記録関連の `SortVivo.Core` 8 ファイル (`SortKernel.csproj:13-20`)。
  - SortAlgorithms のソース。`#if NETWASM` は属性だけの差である (`SVsub/.../SortSpan.cs:451-503`)。
- NetWasm に結びついたもの:
  - `ENG/Exports.cs:1,17,29,43,58,63,73,83,106,121,145` (`JSExport` と `JSImport`、GC 配列の返却、例外による失敗の通知)。
  - `ENG/clock.mjs`。
  - `ENG/InputSequences.cs` (NetWasm に `Enumerable` がないための層)。
  - `ENG/SortKernel.csproj`。
  - JS では `m2-contracts.mjs` の `NetWasmManagedError` と、`prepare.mjs` の `publishEngine`。
- UI はエンジンを `EngineClient`、`RankingClient`、`QuizComputation` だけで扱い、生の export に触れない (`static-ui-engine.md:17-21`、`static-ui-wasm-migration.md:147`)。Worker は `open()` の形にだけ依存している。
- WASM ABI の数値 ID は永続化しない — `static-ui-wasm-migration.md:135`。PoC も「同じ UI から NetWasm、公式 .NET、C++/Rust、JS のエンジンを選べるようにする」方針を記している — `PoC:140`。

### Inferences (推定): ツールチェーンに中立な構成

```
src/SortVivo.Engine/              ツールチェーンに依存しないコア (ソースを取り込むだけで、SDK を持たない)
  Dispatch.cs (生成)、ThumbnailContext.cs
  EngineCore.cs (新規)            検証と処理を Span<int> で行う。状態コードを返す。時計は TClock : struct, IEngineClock
  PocCodec.cs                     Length(...) と Encode(Span<int>, ...) の 2 段にする
  InputSequences.cs               全ヘッドに残す (入力生成を同じコードにして決定性をそろえる)
src/SortVivo.Engine.NetWasm/      今の SortKernel.csproj と JsExports.cs (薄い変換)、clock.mjs
src/SortVivo.Engine.NativeAot/    net10.0 と ILCompiler.LLVM の csproj、NativeExports.cs (UnmanagedCallersOnly と pinned 領域)、
                                  facade/SortKernel.mjs、Emscripten のリンク引数
(src/SortVivo.Engine.Mono/)       比較用だけ。単体でも 2.1 MB でエンジン予算の 3.5 倍
WEB/src/lib/engine-kernel.d.ts    Worker とツールが見る唯一の型 (open() と 9 メソッド)
WEB/tools/prepare.mjs             SORTVIVO_ENGINE=netwasm|naot-llvm で切り替え、同じ出力先に置く。revision はそのまま
```

- コアは `int[]` ではなく `Span<int>` と「確保の仕方」を受け取る。これで NetWasm (GC 配列) と NAOT-LLVM (pinned 領域) の両方を同じコードで扱える。
- 上限、検証、900 万操作、エンコードを 1 か所 (コア) に置くと、ヘッドごとの挙動の違いは「失敗をどう運ぶか」だけになる。NetWasm は例外、NAOT-LLVM は状態コードである。
- NetWasm のヘッドも状態コードで返し、ファサードで例外に直す形に統一すれば、JS から見た失敗の意味論もそろう。
- 後戻りのしやすさは高い。
  - 永続化データに ABI は含まれない。
  - revision はハッシュ付きのパスなので、切り替えればキャッシュは自動で無効になる。
  - 戻すときは、もう一方のヘッドで再ビルドするか、Cloudflare Pages のロールバックを使う。
  - 戻せないものは製品の数値だけである。表示される時間、順位、Quiz の正解の分布が変わる。
- 両方のヘッドを CI で常にビルドし、照合を通し続けることが、後戻りのしやすさを保つ条件になる。NAOT-LLVM は Emscripten の分だけ CI が重くなる。

**手間の目安** (行数はコードからの見積もり、日数は推定)

| 段階 | 内容 | 目安 |
|---|---|---|
| 0. 門の測定 | 87 算法の NAOT-LLVM エンジンを試作し、Brotli のサイズ、実際の記録時間 (4,096 要素の Heap/Quick/Tim と大きな入力)、初期メモリ、9 Worker 同時の挙動、入力 1,775 件の照合を測る | 1〜2 日。**ここで予算を超えれば中止** |
| 1. 中立化 (NetWasm のまま) | `Exports.cs` (183 行) をコアと薄い層に分け、`PocCodec` を Span 化し、状態コードを導入する。照合と単体テストは変わらず通ることを確かめる | C# 200〜300 行。1〜2 日 |
| 2. NAOT ヘッド | `NativeExports` 150〜250 行、csproj 約 40 行、`NuGet.config` 約 20 行、ファサード 150〜250 行 + 型定義 + テスト、Worker 2 本で約 20 行、ツール約 40 行、`prepare.mjs` 30〜60 行 | 2〜4 日 |
| 3. CI | 2 本の workflow に Emscripten の用意と、emsdk、`EM_CACHE`、NuGet のキャッシュを足す | 1 日 + 調整 |
| 4. 品質ゲート | 全件照合、互換性ゲート、サイズ、M8 の性能一式、U1 (Ranking と Quiz の正解一致) の再測定 | 1〜3 日 (計算時間を含む) |

### Gaps
- NetWasm SDK が普通の .NET クラスライブラリを `ProjectReference` で参照できるかは確認していない。そのため、共通コアもソースを取り込む形 (`.projitems` か glob) を前提にした。
- 中立化したあと、NetWasm の生成コードと速度が変わらないか (inline の判断が変わる可能性) は、照合と性能比較で確かめる必要がある。

---

## 総括

1. **ワークロード**: 高速経路は、画面に出る数値と正誤を決める。Visualizer と Sortube の時間、Sortube の勝敗、Ranking、Quiz の正解である。観測経路は記録を待つ時間を決める。4,096 要素以下の待ち時間は、エンジンの速さに依存しない時間予算型の較正 (約 80〜100 ms) が大半を占め、記録は 1〜2 割にとどまる。NAOT-LLVM は高速経路を約 1.8 倍速くし (旧版よりも速くなる)、観測経路を約 3 倍遅くする (測定値)。その結果、数値の分布が変わり、大きな入力の記録待ちが延びる、と見込む。
2. **export の移植**: 方針は 4 つ。ポインタは C# から JS への一方向にする。入力は C# が持つ pinned の作業領域、結果は「次の呼び出しまで有効」な結果領域に置き、JS が複製する。失敗は状態コードで返す。trap のあとはモジュールを捨てる。NetWasm と同じ形のファサードを置けば、TypeScript 側の変更は限られる。
3. **セキュリティ**: 攻撃面は変わらない。URL と保存データは、許可リストの ID と範囲の決まった整数に変換されてから届く。本当のリスクとして増えるのは、実験版 feed と Emscripten によるビルドのサプライチェーンである。他は堅牢性の問題で、長さ、ビューの鮮度、使用不能状態、i32 への切り詰めの扱いである。CSP も COOP/COEP も設定がない。
4. **ビルドと配信**: 実験版 feed と対応付け、Emscripten 3.1.56 (CI と全開発者)、net10.0 の別ヘッド、Emscripten 出力のファサードが新たに必要になる。ハッシュ付きの配信と revision の仕組みはそのまま使える。
5. **要件**: 最大の障害は配信サイズである。全体予算の残りは 2,096 B で、外挿では約 +190 KB。エンジン予算と旧版比 70% 減も割る見込みである。ブラウザーの下限は下がるが、legacy EH の将来性は未確認である。900 万操作と 9 Worker 同時のメモリは未検証である。先行する NAOT-LLVM の評価記録はない。
6. **判断**: **今の予算のままでは採用を勧めない (条件付き保留)**。代わりに次の 2 つを勧める。
   - (a) ツールチェーンに中立な分割を先に行う。安く、NetWasm のままでも価値がある。
   - (b) 87 算法の NAOT-LLVM エンジンで、サイズと実際の記録時間の 2 つを門として測る。
   - 両方を通り、かつ「旧版に近い数値」ではなく「ネイティブに近い数値」を製品の目標として受け入れる場合に限り、採用を再検討する。
   - 2 つのエンジンを併用する案は、全体予算に両方の大きさが乗るので、今の予算では成り立たない。
