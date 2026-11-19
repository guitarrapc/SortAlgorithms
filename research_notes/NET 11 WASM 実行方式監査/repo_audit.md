# SortAlgorithms の WebAssembly 適合性監査と SortVivo の利用実態

監査日: 2026-11-19。読み取り専用。どちらのリポジトリでもビルド・実行・変更をしていない。

出典の表記(本文の `path:line` が researcher.md の [Source](URL) に相当する):

| 略号 | 実体 | 版 |
|---|---|---|
| `SA/` | `D:\github\guitarrapc\SortAlgorithms` | HEAD `f674c43` (2026-11-18) |
| `Alg/` | `SA/src/SortAlgorithm/Algorithms` | 同上 |
| `SV/` | `D:\github\guitarrapc\Sortvivo2` | branch `v2`、HEAD `3ba63024` (2026-11-18) |
| `SVsub/` | `SV/SortAlgorithms` (submodule) | `526498a` (2026-11-14) |
| `ENG/` | `SV/artifacts/migration/engine/browser` (gitignore 対象の既存ビルド出力) | ファイル時刻 2026-11-18 23:24〜23:26 |
| `NW/` | `C:\Users\guitarrapc\.nuget\packages` の復元済み NetWasm パッケージ | 0.14.2 |

「実物解析」と書いた項目は、既存の `ENG/SortKernel.wasm` と `SV/src/SortVivo.Engine/bin/Release/netwasm0.1/SortKernel.core.wasm` を、NuGet キャッシュ内の `wasm-tools 1.256.0 print` と自作のセクション解析で読んだ結果である。出力はセッションの scratchpad のみに置いた。この 2 つのバイナリは `SVsub/` (`526498a`) から作られており、SA の HEAD ではない。

トゥールチェーン略号: **Mono** = .NET browser-wasm (Mono AOT / interpreter)、**CoreCLR** = CoreCLR Wasm、**NAOT-LLVM** = NativeAOT-LLVM、**NetWasm** = NetWasm SDK。

## Q1. ライブラリのプロジェクト設定 (TFM、trim/AOT、nullable/unsafe)

### Takeaway
単一 TFM `net11.0` の通常ライブラリで、trim/AOT の宣言も解析器も一切ない。実際には反射を使っていないので宣言の欠落は静的な保証の欠落にとどまるが、消費側が個別に補っている。

### Cited Findings
- TFM は `net11.0` のみ。`Nullable=enable`、`ImplicitUsings=enable` — `SA/Directory.Build.props:3-5`
- `IsPackable=true`、`AllowUnsafeBlocks=true`。csproj の設定はこの 2 つと `InternalsVisibleTo` 3 件だけ — `SA/src/SortAlgorithm/SortAlgorithm.csproj:3-4,8-16`
- `IsAotCompatible`、`IsTrimmable`、`EnableTrimAnalyzer`、`EnableAotAnalyzer`、`PublishAot` は csproj/props に 0 件 — `git grep` 結果(`SA` の `*.csproj`, `*.props`)。影響: 全トゥールチェーン。trim/AOT 警告がライブラリのビルドで出ない。
- 消費側は `IsTrimmable` がないことを明記し、`TrimmableAssembly` で補っている。理由として x86 の Avx512 経路まで AOT されていたことを挙げる — `SV/src/SortVivo/SortVivo.csproj:20-25`。影響: Mono。
- `InternalsVisibleTo` に `SortVivo` がある — `SA/src/SortAlgorithm/SortAlgorithm.csproj:14-16`
- モジュール全体に `SkipLocalsInit` — `SA/src/SortAlgorithm/globals.cs:7`。`stackalloc` の未初期化読みは各所の `Clear`/`Fill` に依存する(`Alg/Distribution/CountingSort.cs:175`, `Alg/Distribution/PigeonholeSort.cs:155`)。
- ライブラリの `global.json` は SDK を固定しない(test runner のみ) — `SA/global.json:1-5`
- ライブラリの tests / workflows / Benchmark / sandbox に Wasm 実行の設定はない — `SA` の `git grep -i wasm -- .github/workflows/*` は 0 件、`NETWASM` は `src/**/*.csproj` と `tests/**` に 0 件。
- 第三者ランタイム依存は csproj にない — `SA/src/SortAlgorithm/SortAlgorithm.csproj:1-19`

### Inferences
- `IsAotCompatible=true` を付けても、Q7 の調査範囲では反射系の警告源が見当たらないため、警告はほぼ出ないと見込む。ただし未ビルドなので推定である。
- Wasm 上の正しさの担保はライブラリ側になく、SortVivo の契約テスト(Q12)に依存している。

### Gaps
- trim/AOT 解析器を実際に有効化した場合の警告数は未確認(ビルド禁止のため)。

## Q2. ハードウェア依存経路 (SIMD、BitOperations)

### Takeaway
明示的な SIMD は Driftsort と Glidesort の AVX-512 カーネルだけで、x64 専用である。Wasm では全トゥールチェーンでスカラーの branchless 経路へ落ちる。`Vector128`、`PackedSimd`、`IsHardwareAccelerated` の経路は存在しない。

### Cited Findings
**確認済み**
- `System.Runtime.Intrinsics` を使うのは 2 ファイルのみ — `Alg/Merge/Driftsort.cs:7-8`, `Alg/Merge/Glidesort.cs:7-8`。他は `Alg/Partition/StdSort.cs:664` のコメントだけ。
- Driftsort: `NullContext` かつ `T=int` かつ自然順かつ `Avx512F.IsSupported` で `StablePartitionAvx512Int`、それ以外は `Unsafe.SizeOf<T>() <= 16` で `StablePartitionBranchless` — `Alg/Merge/Driftsort.cs:703-712`。Wasm 影響: AVX-512 は存在しないのでスカラー。全トゥールチェーン。
- Glidesort も同形で、スカラー側は `PartitionBranchless` — `Alg/Merge/Glidesort.cs:1448-1459`
- `Vector128/256`、`Vector<T>`、`Sse`、`Avx2`、`AdvSimd`、`PackedSimd`、`IsHardwareAccelerated` は `src/SortAlgorithm` に 0 件(上記 grep)。仕様上も ISA 別 SIMD は共通基盤として不採用 — `SA/.github/docs/specs/performance.md:150`
- NetWasm の実エンジンに AVX-512 経路は残っていない。`SortKernel.core.wasm` の name section 1,657 関数中、`Avx512`、`Vector512`、`PartitionVec16`/`PartitionVector16`、`PackedSimd` はいずれも 0 件 — 実物解析。
- `BitOperations` は `Log2`/`LeadingZeroCount`/`TrailingZeroCount`/`PopCount`/`RoundUpToPowerOf2` を使用 — `Alg/Partition/PDQSort.cs:571`, `Alg/Partition/StdSort.cs:780`, `Alg/Merge/Glidesort.cs:337`, `Alg/Insertion/LibrarySort.cs:179`, `Alg/Heap/TournamentSort.cs:153`
- NetWasm では対応する Wasm 命令に落ちている: `i32.clz` 61、`i64.clz` 30、`i32.ctz` 11、`i64.ctz` 12、`i32.popcnt` 5、`i64.popcnt` 2 — 実物解析(`ENG/SortKernel.wasm`)。

**SIMD128 の依存(NetWasm)**
- `ENG/SortKernel.wasm` には `v128.store` 6,182、`v128.load` 3,499、`v128.const` 2,690 があり、整数・浮動小数の SIMD 演算は合計約 100 命令しかない — 実物解析。
- 消費側の記録は、inline 後も `SortSpan` の 24 byte 構造体を毎反復 v128/i64 で linear memory へ複製していると述べる — `SV/.github/plans/v2_issues.md:202`

### Inferences
- NetWasm の v128 は主に構造体の複製・初期化に使われ、ソートのループを SIMD 化したものではない(命令の内訳と上記記録からの推定)。
- `deployment.json` の `featureSet` は `"none"`(`ENG/deployment.json:7`)だが、バイナリには v128 命令があるので、SIMD128 非対応の実行環境ではモジュールを compile できないはずである。
- Wasm で SIMD の恩恵を得るには `Vector128` か `PackedSimd` の別カーネルが必要になる。現状の AVX-512 カーネルは Wasm の速度に寄与しない。

### Gaps
- Mono / CoreCLR / NAOT-LLVM で `Avx512F.IsSupported` が定数 false に畳まれ、カーネル本体が除去されるかは未確認。Mono については `SV/src/SortVivo/SortVivo.csproj:20-21` が「trim 前は AOT されていた」と記すのみ。
- `BitOperations` の lowering は NetWasm 以外では未確認。
- BCL 内部(`Span.CopyTo`、`Array.Sort` 等)の SIMD 利用はトゥールチェーンごとに要計測。

## Q3. 32-bit 前提 (`nint`、ポインタ幅、64-bit 演算、Unsafe オフセット)

### Takeaway
ポインタ幅に起因する誤りは見つからなかった。主な論点は、`long`/`ulong`/`UInt128` を使う毎要素の計算コストと、Release の無検査アクセスが wasm32 では不正アクセスとして検出されにくいことである。

### Cited Findings
**確認済み(wasm32 で正しく動く設計)**
- Release の要素アクセスは `Unsafe.Add(ref _ref, (nint)(uint)i)` — `Alg/SortSpan.cs:114,134,165,195,225,443,803-804`。同形が `Alg/Distribution/SpreadSort.cs:573`。
- この形の理由はコメント上 x64 の `movsxd` 回避である — `Alg/SortSpan.cs:111-113`。wasm32 では `nint` が 32 bit なので利点は発生しない。
- NetWasm の wasm32 レイアウトは参照 4 byte、`Span` の長さオフセット 4 — `ENG/interop.json` の `targetLayout`(`managedReferenceSize: 4`, `spanLengthOffset: 4`)。ポインタ長 4 — `NW/netwasm.runtime.pack/0.14.2/runtime/runtime-policy.json` の `targets.wasm32.pointerSizeBytes`。
- NetWasm のメモリ上限は 2 GiB — `runtime-policy.json:57`(`maximumMemorySizeBytes: 2147483648`)、実物の memory section は min 3 / max 32,768 pages(実物解析)。バイトオフセットが 32 bit を超える経路はない。
- `nint`/`nuint` の鍵幅は `IntPtr.Size` で分岐する — `Alg/IRadixKeySelector.cs:99-100,146-148`, `Alg/Distribution/FlashSort.cs:230-231,263,270`, `Alg/Distribution/BucketSort.cs:576`。SpreadSort は `nint`/`nuint` を明示的に拒否 — `Alg/Distribution/SpreadSort.cs:107,773`。

**確認済み(64-bit 以上の演算が毎要素に載る箇所)**
- ラジックス鍵は常に `ulong`。`int` 入力でも `ulong` へ広げる — `Alg/IRadixKeySelector.cs:37,129`
- BucketSort は毎要素で `long` の除算 — `Alg/Distribution/BucketSort.cs:234`。ジェネリック側も `ulong` — `Alg/Distribution/BucketSort.cs:455`
- **FlashSort は毎要素で `UInt128` の乗算と除算** — `Alg/Distribution/FlashSort.cs:212-213`。呼び出しは 3 つのループ内 — `Alg/Distribution/FlashSort.cs:134,163,178`。Wasm に 128-bit 命令はないため全トゥールチェーンでソフトウェア処理になる。
- 実行回数が run 単位のもの: `Alg/Merge/Glidesort.cs:335-337`, `Alg/Merge/PowerSort.cs:411-412`
- 統計カウンタは `ulong` — `SA/src/SortAlgorithm/Contexts/StatisticsContext.cs:17-26,42`
- NetWasm の実エンジンでの出現数: `i64.mul` 59、`i64.div_s` 16、`i64.div_u` 27、`i64.rem_u` 14 — 実物解析。

### Inferences
- 前提(Wasm 仕様の一般知識、本監査では一次資料を取得していない): wasm32 でも `i64` は値型として直接扱えるので、`long` 演算が 32-bit ペアに分解されるとは限らない。アドレス幅と整数幅は別である。
- 同じく前提: 線形メモリ内の範囲外アクセスは、メモリ全体の範囲を超えない限り trap しない。`(nint)(uint)i` は wasm32 で負の `i` を負オフセットとして扱うため、アルゴリズムの添字バグは x64 より静かにメモリを壊しうる。`SortSpan` の Release 経路は検査を持たない(`Alg/SortSpan.cs:108-115`)。Debug だけが `Span` の範囲検査を行う。
- `IntPtr.Size == 4` の分岐は x64 の実行では通らない。32-bit 実行の CI がライブラリにないので(Q1)、この分岐の実行確認は SortVivo でも `int` 以外を流さない限り得られない。

### Gaps
- Mono / CoreCLR / NAOT-LLVM の `UInt128` と `long` 除算の実コストは要計測。特に FlashSort と BucketSort。
- 32-bit ホストの Wasm エンジンでの i64 コストは未確認。
- Mono の wasm32 最大メモリ(`SortVivo.csproj` は heap 系プロパティを設定していない)は既定値未確認。

## Q4. スタック (再帰深さ、stackalloc、ジェネリック再帰)

### Takeaway
quicksort 系は小さい側を再帰する形か明示スタックで深さが O(log n) に抑えられており、敵対的入力でスタックは伸びない。深さ O(n) を確認したのは SlowSort だけで、SortVivo は 16 要素に制限している。最大の `stackalloc` は SpreadSort の 16 KiB である。NetWasm は管理フレームを 64 KiB のネイティブスタックではなく伸長可能な value stack に置く。

### Cited Findings
**再帰深さ(確認済み)**
- 小さい側を再帰、大きい側をループ: `Alg/Partition/QuickSortMedian3.cs:241-265`, `Alg/Partition/QuickSortMedian9.cs:235-257`, `Alg/Partition/QuickSort3way.cs:279-292`, `Alg/Partition/StableQuickSort.cs:207-230`, `Alg/Partition/BidirectionalStableQuickSort.cs:224-242`, `Alg/Partition/DestswapStableQuickSort.cs:397-423`, `Alg/Partition/IntroSort.cs:417-444`, `Alg/Partition/PDQSort.cs:348-368`, `Alg/Partition/BlockQuickSort.cs:297-313`, `Alg/Merge/Glidesort.cs:1554-1589`
- DualPivot は最大区画をループし、他の 2 区画を再帰 — `Alg/Partition/DualPivotQuickSort.cs:451-471`
- `QuickSort` は再帰せず、64 要素の明示スタック 2 本 — `Alg/Partition/QuickSort.cs:181-186,212-233`
- 深さ上限で打ち切る形: `Alg/Partition/IntroSortDotnet.cs:131,177,183,190`, `Alg/Partition/Ipnsort.cs:349,355,386`, `Alg/Merge/Driftsort.cs:622,665`
- ラジックス系は桁数で上限が決まる。AmericanFlag は 1 段 2,052 byte、32-bit 鍵で最大 4 段 — `Alg/Distribution/AmericanFlagSort.cs:244-249,279,325`
- **SlowSort は `end-1` の再帰で深さ Θ(n)** — `Alg/Joke/SlowSort.cs:153`。SortVivo の上限は SlowSort / StoogeSort が 16、BogoSort が 8 — `SV/src/SortVivo.Engine/Dispatch.cs:874` 以降の生成表。

**stackalloc の最大量(int は 4 byte で換算)**

| 箇所 | 量 | 根拠 |
|---|---:|---|
| SpreadSort `binSizes` | 16,384 B (固定) | `Alg/Distribution/SpreadSort.cs:164-165,382` |
| Radsort `bucketState` | 5,120 B | `Alg/Distribution/Radsort.cs:132,344` |
| Counting / Bucket / Pigeonhole | 最大 4,096 B | `Alg/Distribution/CountingSort.cs:91,173`, `Alg/Distribution/BucketSort.cs:81,193`, `Alg/Distribution/PigeonholeSort.cs:67,153` |
| AmericanFlag | 2,052 B/段 | `Alg/Distribution/AmericanFlagSort.cs:244-249` |
| BlockQuickSort | 1,024 B | `Alg/Partition/BlockQuickSort.cs:118,446-447` |
| RadixLSD10 | 880 B | `Alg/Distribution/RadixLSD10Sort.cs:100,103,239` |
| Binary/Treap/Splay の走査スタック | 最大 512 B | `Alg/Tree/BinaryTreeSort.cs:209-211`, `Alg/Tree/TreapSort.cs:388-390`, `Alg/Tree/SplaySort.cs:425` |

- `stackalloc T` はなく、ジェネリック要素の固定バッファは HEAD の QuadSort の `[InlineArray(32)]` のみ — `Alg/Merge/QuadSort.cs:91,106-110`。これは `SVsub/` に存在しない(Q13)。

**NetWasm のスタック構造(実物解析)**
- ネイティブスタックは 65,536 byte — `NW/netwasm.runtime.pack/0.14.2/runtime/runtime-policy.json:6`。スタックポインタと見られる唯一の可変 global の初期値は 88,160、静的データ終端は 20,236 — `ENG/runtime-layout.json`、実物解析。
- 管理コードはこのスタックをほぼ使わない。837 関数のうち global 0 を使う典型的な prologue は 14 関数、`global.get 0` は全体で 15 回 — 実物解析。
- 管理フレームは `value_frame_enter` 系(export 名、func 73)で、線形メモリ上の cursor(アドレス 21664/21668)を進め、不足時に拡張関数 func 71 を呼ぶ。この処理は `runtime-policy.json` で `Aggressive` inline 指定され、469 関数に展開されている — 実物解析、`runtime-policy.json` の `inlineIntents`。
- 推定フレーム長の上位は 16,744 / 14,224 / 11,236 / 2,432 / 2,160 byte。16,744 は SpreadSort の 16,384 byte を含む値と整合する — 実物解析(ヒューリスティック抽出)。
- 直接の自己再帰関数は 138 個残っている — 実物解析。

**実行時の失敗処理(消費側)**
- エンジンの例外は Worker で捕捉して返し、チャネルが Worker を破棄する — `SV/src/SortVivo.Web/public/engine-worker.mjs:80-87`, `SV/src/SortVivo.Web/src/lib/worker-channel.ts:56-58,114-117`

### Inferences
- NetWasm では `stackalloc` と管理フレームが伸長可能な value stack に載るので、再帰の上限はブラウザの Wasm 呼び出しスタック側で決まると考えられる。64 KiB は主に C ランタイム側の制約である。
- 今回確認した quicksort 系では、敵対的入力の影響は深さではなく O(n²) の実行時間に出る。SortVivo の純ソート経路には操作数上限がなく、120 秒のタイムアウトと Worker 破棄で止まる(`worker-channel.ts:78-81`)。
- Mono で本番の記録が UI スレッド上で動く(Q10)ため、深い再帰や大きな `stackalloc` の余裕はトゥールチェーンだけでなく呼び出し元の深さにも依存する。

### Gaps
- 個別に深さを証明していない再帰: `Alg/Merge/RotateMergeSort.cs:674-675,1015-1016`, `Alg/Merge/SymMergeSort.cs:338`, `Alg/Merge/Glidesort.cs:604`, `Alg/Merge/QuadSort.cs:1639,1658`, `Alg/Partition/FluxSort.cs:722`, `Alg/Partition/StdSort.cs:545`, `Alg/Distribution/RotateLSDRadixSort.cs:275-276,303-304`, `Alg/Network/BitonicSort.cs:401-443`。再帰候補の一覧は機械抽出で、相互再帰は含まない。
- 実スタック上限は全トゥールチェーンで未測定。Mono の Emscripten スタック量、CoreCLR、NAOT-LLVM の値はこのリポジトリから確定できない。
- 14,224 / 11,236 byte のフレームが対応するアルゴリズムは未同定。

## Q5. メモリ (ArrayPool、補助バッファ、wasm32 ヒープ)

### Takeaway
ライブラリの補助領域は ArrayPool と `stackalloc` に集約され、`Algorithms` に `new T[]` はない。実運用で大きいのはソート本体より SortVivo の全操作記録で、上限 900 万操作では wasm32 の線形メモリを約 550 MiB 使う計算になる。

### Cited Findings
- `Algorithms` 配下の `.Rent(` は 99 行、`ArrayPool` を使うファイルは 47 — grep 結果。`new (T|int|uint|long|ulong|byte|bool|Node<T>)[` は 0 件 — 同。
- 木・ヒープ系は要素ごとのノードを arena に取る。例: `Node<T>` は `T` と `int` 2 個 — `Alg/Tree/BinaryTreeSort.cs:285-289`、返却 `Alg/Tree/BinaryTreeSort.cs:136`
- CountingSort と PigeonholeSort の範囲上限は 10,000,000 — `Alg/Distribution/CountingSort.cs:90,165`, `Alg/Distribution/PigeonholeSort.cs:66,143`。`int` の count 配列だけで 40 MB になる。
- Driftsort の全量確保の閾値は 8,000,000 byte の固定値でアドレス幅に依存しない — `Alg/Merge/Driftsort.cs:106,206`
- HEAD の QuadSort には `SWAP_CAP = 4194304` — `Alg/Merge/QuadSort.cs:100`(`SVsub/` には未収載)

**SortVivo の記録(NetWasm)**
- 記録上限は 9,000,000 操作 — `SV/src/SortVivo.Engine/Exports.cs:9`
- 事前カウント後に `List<SortOperation>(count + 16)` を確保 — `SV/src/SortVivo.Engine/Exports.cs:155-165`
- `SortOperation` のフィールドは int 3、参照 1、short 2、byte 4 — `SV/src/SortVivo.Core/Models/SortOperation.cs:21-38`。wasm32 では 24 byte と計算できる。
- 転送形式は 1 操作 10 word(40 byte)— `SV/src/SortVivo.Engine/PocCodec.cs:11-13,22-25`
- これまでの全件照合の最大は 8,386,559 操作で、上限超過は発生していない — `SV/.github/docs/plans/static-ui-wasm-migration.md:284`
- NetWasm のメモリは最小 3 pages、最大 2 GiB — 実物解析、`runtime-policy.json:57`

### Inferences
- 上限ちょうどの場合、線形メモリ内に `List` 216 MB と転送用 `byte[]` 360 MB が同時に存在し、合計約 576 MB(約 549 MiB)に入力などが加わる。2 GiB 上限の範囲内である。
- JS 側では、runtime の配列返却が `memory.buffer.slice` でコピーを作り(`ENG/netwasm.runtime.NZe7Bam3.mjs` の `Ca`)、さらに Worker が `frame.slice()` する(`SV/src/SortVivo.Web/public/engine-worker.mjs:158-160`)。`byte[]` が `Ca` を通るならピーク時に同じ内容が JS 側に 2 つ存在する。経路はミニファイ済みコードからの読み取りである。
- 前提(Wasm 仕様の一般知識): 線形メモリは縮小しない。一度大きな記録をした Worker は破棄するまで確保済みメモリを保持する可能性が高い。

### Gaps
- 実ピークメモリはトゥールチェーン別に未測定。既存の記録も、メインスレッド側の計測で Worker/WASM/GPU 全体ではないと断っている — `SV/.github/docs/plans/static-ui-wasm-migration.md:334`, `SV/.github/docs/references/static-ui-m8-final-comparison-2026-11-17.md:189`
- `ArrayPool<T>.Shared` のサイズ丸めと保持量は各ランタイムで未確認。
- NetWasm GC (`NetWasmGarbageCollector=NetWasm`) の断片化は未確認。

## Q6. スレッド (Parallel、Task、Thread、lock、並列アルゴリズム)

### Takeaway
ライブラリにスレッド依存はなく、並列アルゴリズムもない。Wasm のスレッド有無は動作に影響しない。並列化しない代わりに、計算の隔離は消費側の Worker が担っている。

### Cited Findings
- `Parallel.`、`Task`、`new Thread`、`ThreadPool`、`Interlocked`、`Volatile.`、`lock`、`Monitor.`、`ThreadStatic`、`async`、`await` などの検索で、`src/SortAlgorithm` の一致はコメント 1 行のみ — `SA/src/SortAlgorithm/Contexts/StatisticsContext.cs:10`
- ソートは同期実行で、統計コンテキストは単一所有 — `SA/.github/docs/specs/performance.md:32-34`。並行共有は対象外 — `SA/.github/docs/spec.md:29`
- NetWasm 経路は専用 Worker 内で同期実行し、途中キャンセルは Worker の terminate で行う — `SV/src/SortVivo.Web/src/lib/worker-channel.ts:114-117`
- `SharedArrayBuffer` は初期構成の必須条件にしない方針 — `SV/.github/docs/plans/static-ui-wasm-migration.md:151`。`engine-client.ts`、`worker-channel.ts`、`ranking-worker.ts`、`quiz-worker.ts` に使用はない — grep 結果。
- Blazor 版は `SortVivo.csproj` に `WasmEnableThreads` がなく(`SV/src/SortVivo/SortVivo.csproj:3-18`)、`Task.Yield()` を挟むが記録本体は同期でブロックしうる — `SV/src/SortVivo.Core/Services/SortExecutor.cs:189-199,226,246,272`

### Inferences
- 全トゥールチェーンでシングルスレッド構成を選べる。COOP/COEP を要求する機能はソート側からは発生しない。

### Gaps
- `ArrayPool<T>.Shared` など BCL 内部の同期処理のコストは未確認。

## Q7. AOT / trim の危険箇所

### Takeaway
反射、動的コード、関数ポインタ、再帰的に拡大するジェネリックは見つからなかった。残る論点は `ISortContext` のジェネリック仮想メソッドで、SortVivo はインターフェース型のまま渡すため実際に使っている。もう一つは `T × TComparer × TContext` のインスタンス数である。

### Cited Findings
- `MakeGenericType`、`MakeGenericMethod`、`Activator.`、`GetMethod(`、`System.Reflection`、`Reflection.Emit`、`DynamicMethod`、`Expression.`、`delegate*`、`DllImport`、`LibraryImport`、`RequiresUnreferencedCode`、`RequiresDynamicCode` は `src/SortAlgorithm` に 0 件 — grep 結果。
- 型名の利用は例外メッセージのみ — `Alg/IRadixKeySelector.cs:52,102,104,156`
- **ジェネリック仮想メソッド**: `OnIndexWrite<T>`、`OnRangeCopy<T>` — `SA/src/SortAlgorithm/Contexts/ISortContext.cs:55,82`。呼び出しは `Alg/SortSpan.cs:129,824,852`。`CompositeContext` も転送する — `SA/src/SortAlgorithm/Contexts/CompositeContext.cs:53-59,69-75`
- SortVivo は両方の実行方式で `TContext = ISortContext` を使う — `SV/src/SortVivo.Engine/Dispatch.cs:452`, `SV/src/SortVivo.Core/Models/AlgorithmMetadata.cs:24`
- 既定インターフェース実装: `OnLink` — `SA/src/SortAlgorithm/Contexts/ISortContext.cs:129`
- static abstract: `IRadixKeySelector<T>.KeyBits` — `Alg/IRadixKeySelector.cs:34`(`SVsub/` にもあり NetWasm でビルド済み)。`IPreparedKeyLoop.Kind` — `Alg/PreparedKey.cs:23`(HEAD のみ)。使用箇所 `Alg/SortSpan.cs:994-1003`。
- `unsafe` は AVX-512 カーネルのみ — `Alg/Merge/Driftsort.cs:857,935,956`, `Alg/Merge/Glidesort.cs:1783,1862,1879`
- 型引数に自身を包んで拡大する形(`SortSpan<X<`、`Sort<X<`、`new *Comparer<...Comparer`)は 0 件 — grep 結果。
- インスタンス規模: NetWasm の core.wasm は名前付き 1,657 関数で、名前に `NullContext` を含むものが 391、`ISortContext` を含むものが 378 — 実物解析。最終 wasm は code section 3,363,883 byte、最大関数は WAT で 132,461 行 — 実物解析。
- 計測用 dispatch では、全ソートが 466 KB の 1 関数へ inline されて V8 の最適化が遅れる問題が起き、生成コードに `NoInlining` を入れて回避した — `SV/.github/docs/plans/static-ui-wasm-migration.md:68`, `SV/src/SortVivo.Engine/Dispatch.cs:278-280`
- Mono 版は generic 呼び出し側を `SortVivo.Core` に残して specialization を出す。SDK 内部項目 `_AOT_InternalForceInterpretAssemblies` に依存する — `SV/src/SortVivo/SortVivo.csproj:36-47`, `SV/.github/docs/specs/performance-wasm-size.md:16`
- `ArrayPatterns` は LINQ を使うが NetWasm 構成には `Enumerable` がなく、消費側が互換層で補う — `SV/src/SortVivo.Engine/InputSequences.cs:3-5`。`Algorithms` と `Contexts` の LINQ 呼び出しは対象パターンで 0 件 — grep 結果。

### Inferences
- `ENG/SortKernel.wasm` の `call_indirect` は 12 箇所だけなので(実物解析)、NetWasm はインターフェース呼び出しの多くを whole-program で解決していると見られる。個別の呼び出しは未追跡である。
- `int` のみを流す現在の SortVivo では GVM のインスタンスは限定的だが、ライブラリ単体としては `T` ごとに必要になる。
- `[InlineArray]` と `IPreparedKeyLoop` は HEAD のみで、Wasm ビルド実績がない(Q13)。

### Gaps
- Mono AOT が `TContext = ISortContext` の GVM を native で処理するか interpreter へ落とすかは未確認。観測経路の速度差に直結するため要計測。
- NAOT-LLVM、CoreCLR でのビルド可否とサイズはこのリポジトリに実績がない。

## Q8. 浮動小数点 (NaN、ビットパターン)

### Takeaway
NaN の判定はペイロードに依存せず、Wasm の NaN 正規化で順序は変わらないと判断できる。ただし SortVivo は `int` しか流さないので、浮動小数経路の Wasm 実行実績はない。

### Cited Findings
- `IsNaturalFloatingNaN` は符号を除いて無限大より大きいかを見る — `Alg/SortSpan.cs:859-868`
- 比較述語は「NaN が先頭」の順序を明示する — `Alg/SortSpan.cs:287-289,328-330,367-369,406-408`
- 仕様: NaN は数値より前、NaN 同士は等しい、符号付きゼロは等しい — `SA/.github/docs/specs/performance.md:56`
- ラジックス鍵は NaN を一律 0 に、`-0` を `+0` に正規化 — `Alg/IRadixKeySelector.cs:199-205,219-226,239-250`
- NaN の前処理は `IsNaN` で判定 — `Alg/FloatingPointUtils.cs:53-64,72-84,90-101`
- SortVivo の入口は `int[]` / `Span<int>` のみ — `SV/src/SortVivo.Engine/Exports.cs:74-79`, `SV/src/SortVivo.Engine/Dispatch.cs:8,452`
- 実エンジンに `f32` の比較命令は 0 — 実物解析。`f64` の比較は 43 あるが、ソート要素由来とは確認していない。

### Inferences
- 前提(Wasm 仕様の一般知識): 算術結果の NaN ビットは非決定的だが、ロード・ストアとローカル移動はビットを保つ。ソートは要素を移動するだけなので、ペイロードも通常は保たれる。
- JS 境界を `number` で通すとペイロードは保証されにくい。`Float64Array` の転送とは区別が必要である。

### Gaps
- `float`/`double`/`Half` のソートを各 Wasm トゥールチェーンで実行した記録は見つからなかった。
- NaN ペイロードの保存をライブラリの契約とするかは文書から読み取れない。

## Q9. 観測経路 (`ISortContext`、`NETWASM`、JIT 依存)

### Takeaway
ゼロコスト設計は JIT の定数畳み込みと struct 昇格を前提に書かれており、Wasm の AOT では同じ結果にならない。実測でも NetWasm は `SortSpan` の複製が残っている。`#if NETWASM` の inline 指定は、現在の SortVivo のビルド設定 `IgnoreAggressive` の下では効かないと考えられる。

### Cited Findings
**NullContext 経路**
- 分岐は `typeof(TContext) != typeof(NullContext)` と `_comparer is IComparableComparer` で、コメントは JIT の畳み込みを前提とする — `Alg/SortSpan.cs:103-107,272-276`。仕様も同様 — `SA/.github/docs/specs/performance.md:17-21`
- 仕様は NativeAOT 等での同一機械語を保証しない — `SA/.github/docs/specs/performance.md:156`
- `SortSpan` は `Span<T>`、`ref T`、`TContext`、`TComparer`、`int` 2 個を持つ — `Alg/SortSpan.cs:21-26`。同じ先頭参照を `_span` と `_ref` の両方に持つ — `Alg/SortSpan.cs:38-39`
- `SortSpan` は多くの内部関数へ値渡しされる(例 `Alg/Partition/QuickSortMedian3.cs:183`)。
- NetWasm では inline 後も 24 byte の `SortSpan` 相当を毎反復複製するとの解析 — `SV/.github/plans/v2_issues.md:202`。上記フィールドの wasm32 での合計(8+4+1+1+埋め2+4+4)も 24 byte である。
- struct/address-taken/allocation の scalar replacement を強制しても 0.98〜1.03 倍で変化なし — `SV/.github/plans/v2_issues.md:203`

**観測あり経路**
- `ReportCompare*` は `_span[i]` で範囲検査付きに読み、コールバックを呼ぶ — `Alg/SortSpan.cs:453-512`
- `Record` は `CompositeContext(stats, record)` を使う — `SV/src/SortVivo.Engine/Exports.cs:166-169`。1 イベントごとに配列を回して各コンテキストへインターフェース呼び出しする — `SA/src/SortAlgorithm/Contexts/CompositeContext.cs:23-29`
- 記録側は `List.Add` と上限判定を毎操作で行う — `SV/src/SortVivo.Core/Services/OperationRecordingContext.cs:39-56`

**`NETWASM`**
- 条件付き inline は 4 helper — `Alg/SortSpan.cs:447-453,468-471,484-487,500-503`。`SVsub/` では 1 行ずれて `:451,469,485,501`。
- `NETWASM;NETWASM0_1` は SDK が定義 — `NW/netwasm.sdk/0.14.2/Sdk/NetWasm.Sdk.Tfm.props:49`。通常の browser runtime に定義しない方針 — `SA/.github/docs/specs/performance.md:192`
- SortVivo は `NetWasmManagedInliningHints=IgnoreAggressive` — `SV/src/SortVivo.Engine/SortKernel.csproj:8`。SDK 既定は `Auto` — `NW/netwasm.compiler.tasks/0.14.2/buildTransitive/NetWasm.Compiler.Tasks.targets:92`
- 採用理由は、managed の Aggressive ヒントだけを抑えて O3 を維持すること — `SV/.github/docs/references/static-ui-m8-final-comparison-2026-11-17.md:23`, `SV/.github/docs/plans/static-ui-wasm-migration.md:915`
- `Auto` に戻すと幾何平均 1.08 倍遅く、Brotli +62.7 KB — `SV/.github/plans/v2_issues.md:203`
- `core.wasm` の name section に `ReportCompare` は 0 件 — 実物解析。
- 仕様は `NetWasm SDK 0.13.0` と記すが、消費側は 0.14.2 — `SA/.github/docs/specs/performance.md:188`, `SV/src/SortVivo.Engine/SortKernel.csproj:1`

### Inferences
- `IgnoreAggressive` の名前と採用記録から、`#if NETWASM` の属性は現在の SortKernel では効力を持たない可能性が高い。helper が単独関数として残っていないのは、コンパイラ自身の判断で inline された結果と見られる。SDK の公開文書でこの設定の厳密な意味は確認できなかった。
- `SortSpan.cs:447` のコメントと `performance.md:188` は 0.13 時点の根拠で、現在の構成を説明していない。
- 値渡しの `SortSpan` を前提にした設計は、struct 昇格が弱いコンパイラでそのままコストになる。Heapsort や Timsort sorted の差はこの類型と記録されている(Q12)。

### Gaps
- Mono AOT、CoreCLR、NAOT-LLVM で `typeof` 分岐と `is IComparableComparer` が畳まれるか、`_comparer is` で boxing が発生しないかは未確認。
- `#if NETWASM` の有無で生成物が同一かは、ビルド比較が必要。

## Q10. SortVivo が実際に使うトゥールチェーン

### Takeaway
SortVivo には 2 つの実行系がある。本番は `main` の Blazor WebAssembly で、.NET 11 の Mono AOT を使う。チェックアウト中の `v2` は NetWasm SDK 0.14.2 の独立エンジンで、プレビュー配信までで本番切替はしていない。NativeAOT-LLVM の設定はなく、CoreCLR は過去の比較実験のみである。

### Cited Findings
**本番: Blazor + Mono AOT**
- `Microsoft.NET.Sdk.BlazorWebAssembly`、`net11.0` — `SV/src/SortVivo/SortVivo.csproj:1,4`
- `InvariantGlobalization=true`、`InvariantTimezone=true`、`RunAOTCompilation=true`、`WasmStripILAfterAOT=true`、`WasmNativeDebugSymbols=false`、`BlazorWebAssemblyDiagnosticsEnabled=false`、`WasmBitcodeCompileOptimizationFlag=-O3`、`OverrideHtmlAssetPlaceholders=true` — `SV/src/SortVivo/SortVivo.csproj:7-17`
- `TrimmableAssembly` は `SortAlgorithm`、`SortVivo`、`SortVivo.Core`、`TrimmerRootDescriptor` は `TrimmerRoots.xml` — `SV/src/SortVivo/SortVivo.csproj:23-34`
- AOT から外す `_AOT_InternalForceInterpretAssemblies` の一覧 — `SV/src/SortVivo/SortVivo.csproj:38-47`
- `SortAlgorithm` は ProjectReference — `SV/src/SortVivo/SortVivo.csproj:78`。submodule 側の TFM は `net10.0` — `SVsub/Directory.Build.props:3`
- SDK は `11.0.100`、`rollForward=latestFeature` — `SV/global.json:2-5`。パッケージは `Microsoft.AspNetCore.Components.WebAssembly 11.0.0`、`BlazorWasmPreRendering.Build 11.0.0-rc.3` — `SV/Directory.Packages.props:11-13`
- 本番 workflow は wasm workload を復元して publish — `SV/.github/workflows/deploy.yaml:41-46`
- `origin/main` の `src/` は `SortVivo` と `SortVivo.Core` のみで `SortVivo.Engine` がない — `git ls-tree origin/main`
- 記録時の環境: SDK 11.0.100、workload 11.0.100.1、`wasm-tools` — `SV/artifacts/migration/dotnet-info.txt:2-4,39-43`(2026-11-14)

**`v2`: NetWasm エンジン**
- `Sdk="NetWasm.Sdk/0.14.2"`、`TargetFramework=netwasm0.1`、`OutputType=JsModule`、`NetWasmJavaScriptHosts=page`、`AllowUnsafeBlocks=true`、`NetWasmOptimization=O3`、`NetWasmManagedInliningHints=IgnoreAggressive`、`NetWasmGarbageCollector=NetWasm` — `SV/src/SortVivo.Engine/SortKernel.csproj:1-10`
- SortAlgorithm をソースとして取り込む。`SortVivo.Core` の記録関連 8 ファイルも同様 — `SV/src/SortVivo.Engine/SortKernel.csproj:12-20`
- SDK 既定の最適化は Release で `Oz`、GC の既定は `Compact` — `NW/netwasm.compiler.tasks/0.14.2/buildTransitive/NetWasm.Compiler.Tasks.targets:87`, `NW/netwasm.runtime.pack/0.14.2/runtime/runtime-policy.json:3`
- 配布物は `profile=netwasm0.1`、`target=wasm32`、SDK/compiler/runtime 0.14.2 — `ENG/deployment.json:5-13`
- publish は `dotnet publish ... -c Release` — `SV/src/SortVivo.Web/tools/prepare.mjs:148-156`
- プレビュー専用 workflow — `SV/.github/workflows/deploy-static-preview.yaml:1-8`。本番切替はまだしていない — `SV/.github/docs/plans/static-ui-wasm-migration.md:5`
- 0.13.0 から 0.14.2 への更新は 2026-11-18 — `SV/.github/docs/plans/static-ui-wasm-migration.md:70`

**その他**
- `tools/wasm-engine-prototype` は Mono AOT の試作(本番ではない) — `SV/tools/wasm-engine-prototype/PlainEngine.csproj:1-19`
- CoreCLR は `UseMonoRuntime=false` などで比較実験のみ、不採用 — `SV/.github/docs/references/wasm-size-investigation-2026-11-13.md:95,103`
- NativeAOT-LLVM 関連(`NativeAOT`、`PublishAot`、`ILCompiler`)は csproj、props、targets、specs、plans、`wasm-*` 文書に 0 件 — `git grep` 結果。

### Inferences
- 「SortVivo の Wasm」と言うときは Mono AOT(本番)と NetWasm(移行先)を分ける必要がある。ライブラリの最適化判断は両方に効き方が異なる。

### Gaps
- `SortVivo.csproj` に `WasmEnableSIMD`、heap、stack の設定はなく、実効値は SDK 既定値の確認が必要。

## Q11. JS からの呼び出し (export、マーシャリング、Worker、メモリ)

### Takeaway
`v2` は専用 Worker で NetWasm モジュールを開き、9 個の `JSExport` を型付き配列で呼ぶ。記録結果は単一のバイナリフレームとして Transferable で返す。`SharedArrayBuffer` は使わない。本番 Blazor は `IJSRuntime` 経由で、計算と記録は .NET 側、再生は JS/Worker 側である。

### Cited Findings
**NetWasm**
- Export は `Input`、`Shuffle`、`Thumbnail`、`LastMetrics`、`Sort`、`Batch`、`CopyBatch`、`Count`、`Record` — `SV/src/SortVivo.Engine/Exports.cs:17-18,29-30,43-44,63-64,73-74,83-84,106-107,121-122,145-146`、型は `ENG/SortKernel.d.mts`。
- JS からの import は `poc.clock.now` のみ — `SV/src/SortVivo.Engine/Exports.cs:58-59`, `SV/src/SortVivo.Engine/clock.mjs:1-3`, `SV/src/SortVivo.Engine/SortKernel.csproj:21`
- 開き方は `import(contract.module)` の `open()` で、idle の Worker では再利用 — `SV/src/SortVivo.Web/public/engine-worker.mjs:22,44`
- 入力は `Int32Array.from` または `app.Input` — `SV/src/SortVivo.Web/public/engine-worker.mjs:45-47`
- 結果は `frame.slice().buffer` を作り、`postMessage` の transfer list で返す — `SV/src/SortVivo.Web/public/engine-worker.mjs:65-68,76-79,158-165`
- フレームは version 付きの Int32 word 列で、統計は 64-bit を 2 word に分割 — `SV/src/SortVivo.Engine/PocCodec.cs:15-20,60-65`
- runtime の配列型対応は `bytes`、`i32[]`、`f64[]` など、`WebAssembly.compile` を使い `compileStreaming` はない — `ENG/netwasm.runtime.NZe7Bam3.mjs`(実物解析)、`SV/.github/plans/v2_issues.md:198`
- Worker は要求ごとに protocol / revision を照合 — `SV/src/SortVivo.Web/public/engine-worker.mjs:12-13`, `SV/src/SortVivo.Web/src/lib/worker-channel.ts:49-55`
- タイムアウトは 120 秒、取消は terminate — `SV/src/SortVivo.Web/src/lib/worker-channel.ts:78-81,114-117`
- UI は生の export を呼ばず `EngineClient` を通す — `SV/src/SortVivo.Web/src/lib/engine-client.ts:13-21`, `SV/.github/docs/specs/static-ui-engine.md:15-21`
- 入力長の上限は 32,768、算法別上限あり — `SV/src/SortVivo.Web/public/engine-worker.mjs:35-41`, `SV/src/SortVivo.Engine/Dispatch.cs:874`

**本番 Blazor**
- 純計測は `MeasureAction`、記録は `SortAction(Span<int>, ISortContext)` — `SV/src/SortVivo.Core/Models/AlgorithmMetadata.cs:24,40`, `SV/src/SortVivo.Core/Services/SortExecutor.cs:156-163`
- 操作列は `OperationStreamEncoder` で `byte[]` にして JS Worker へ渡す — `SV/src/SortVivo.Core/Services/OperationStreamEncoder.cs:33,42,52-59`, `SV/src/SortVivo.Core/Services/PlaybackService.cs:586-592`

### Inferences
- 境界のコピーは入力で JS から managed、結果で managed から JS、さらに Worker の `slice` と続く。大きな記録では転送よりコピー回数が効く可能性がある。
- `input: Array.from(input)` と UI 側の検証で、32,768 要素の `number[]` を毎回作る(`engine-worker.mjs:161`)。

### Gaps
- JS↔Wasm の 1 呼び出しあたりのコストは個別に測定されていない。
- Blazor の `byte[]` interop の実コピー回数は確認していない。

## Q12. 既存の計測・ビルド出力

### Takeaway
サイズと速度の記録は豊富にある。NetWasm は配信量で大きく勝る一方、未最適化 tier とコード生成の差で、最新の記録(0.14.2)でも速度ランキングの幾何平均は旧 Blazor 比で random 1.54 倍、sorted 1.70 倍遅い。今回は再実行しておらず、以下はすべて記録値である。

### Cited Findings
**ローカルのビルド出力(未再実行)**

| 対象 | 値 | 出所・日時 |
|---|---:|---|
| `SortKernel.wasm` 展開後 | 3,387,085 B | `ENG/`、2026-11-18 23:24 |
| `SortKernel.core.wasm` | 5,590,366 B | `SV/src/SortVivo.Engine/bin/Release/netwasm0.1/`、同上 |
| Blazor `dotnet.native.*.wasm` | 13,771,950 B、Brotli 2,886,322 B | `SV/artifacts/migration/baseline-size.json`、基点 commit `a2d50189`、2026-11-14〜15 |
| Blazor の `SortAlgorithm` Webcil | 262,937 B、Brotli 74,917 B | 同上 |

**配信サイズ(文書記録)**

| 項目 | 値 | 出所 |
|---|---:|---|
| 旧 Blazor JS+WASM | 5,046,392 B | `SV/.github/docs/references/static-ui-m8-final-comparison-2026-11-17.md:45`、`baseline-size.json` の `codeBr` と一致 |
| M8 JS+WASM | 909,990 B | 同 `:45` |
| M8 JS+WASM+必須JSON+inline | 1,569,633 B (旧 5,569,166 B) | 同 `:46` |
| `SortKernel.wasm` Brotli(0.13.0 時) | 412,716 B | `SV/.github/docs/references/static-ui-publish-timing-2026-11-18.md:47`, `SV/.github/plans/v2_issues.md:167` |
| エンジン一式 | 461,591 B → 466,440 B(dispatch 分割後) | `static-ui-publish-timing-2026-11-18.md:47`, `v2_issues.md:279` |
| 0.14.2 更新の増分 | wasm +4,876 B、配信全体 1,587,152 B | `v2_issues.md:299` |
| Mono 最終構成 | native 13,749,607 B、framework Brotli 4,797,342 B | `SV/.github/docs/references/wasm-size-investigation-2026-11-13.md:24` |
| Mono 試作(Blazor なし) | native 7,388,408 B、Brotli 1,678,198 B、framework 計 2,124,713 B | `SV/tools/wasm-engine-prototype/README.md:20-25`(2026-11-13) |

**速度(文書記録、いずれも 2026-11-13〜18)**
- Visualizer、4,096 要素、初回/再訪問、µs。Quicksort random は旧 83/68、v2 152/244。Heapsort random は旧 154/164、v2 522/710。Timsort sorted は旧 168/163、v2 261/274 — `SV/.github/plans/v2_issues.md:209,211,213,216`(dispatch 分割前)
- dispatch 分割後のランキング幾何平均(旧版比): random 1.61→1.52 倍、sorted 2.06→1.85 倍 — `SV/.github/plans/v2_issues.md:275-276`, `SV/.github/docs/plans/static-ui-wasm-migration.md:68`
- 0.14.2 後: random 1.54 倍、sorted 1.70 倍 — `SV/.github/plans/v2_issues.md:301`, `SV/.github/docs/plans/static-ui-wasm-migration.md:70`
- 最適化 tier 同士でも Heapsort 約 2〜3 倍、Timsort sorted 約 6 倍。Quicksort random は v2 70 µs、旧版約 100 µs — `SV/.github/plans/v2_issues.md:202`
- 差の 3 要因は起動コスト、V8 の未最適化 tier、NetWasm のコード生成。最適化 tier まで約 0.4〜0.7 秒 — `SV/.github/plans/v2_issues.md:195-202`
- 旧版より速い例: Bucket sort 0.31 倍、Pancake 0.49 倍、Selection 0.57 倍 — `SV/.github/plans/v2_issues.md:291`
- Mono の全 AOT 廃止(interpreter/Jiterpreter)は幾何平均 3.019 倍遅い。CoreCLR browser 構成は 262.259 倍遅く不採用 — `SV/.github/docs/references/wasm-size-investigation-2026-11-13.md:94-95`
- `Os` は WASM が 475,425→177,827 B になるが速度退行で不採用 — `SV/.github/docs/plans/static-ui-wasm-migration.md:849`

**正しさの検証記録**
- 通常 .NET と WASM の照合: 入力 1,775 件、完全記録 16,704 条件(有効 16,664、長さ拒否 40) — `SV/.github/docs/plans/static-ui-wasm-migration.md:284,296`。0.14.2 後も通過 — `SV/.github/plans/v2_issues.md:298`

### Inferences
- 比較元はすべて SortVivo の Blazor 版と NetWasm 版であり、NAOT-LLVM の数字はこのリポジトリにない。CoreCLR の 262 倍は当該 SDK・構成(interpreter 相当の設定)の値で、AOT 付き CoreCLR の評価ではない。
- 算法ごとの差が一様でないため、NetWasm 上の順位は旧版と変わりうる。実際に「いちばん速いソート」の正解一致率は約 86%(169 問中 145 問)と記録されている — `SV/.github/plans/v2_issues.md:280,287`

### Gaps
- HEAD の SortAlgorithms (`f674c43`) を Wasm で計測した記録はない。
- 9,000,000 操作超過の例外経路は全件照合で発生していない(`static-ui-wasm-migration.md:284`)。NetWasm 上の例外巻き戻し性能は未確認。
- SortVivo 以外の要素型、comparer、実機モバイルの計測はない。M9 の実端末確認は未完了 — `SV/.github/docs/plans/static-ui-wasm-migration.md:5,231`

## Q13. submodule の同期状態

### Takeaway
同期していない。Sortvivo2 の submodule は `526498a` (2026-11-14) で、このチェックアウトの HEAD `f674c43` より 32 commit 古い。`main` と `v2` は同じ commit を指すので、本番も NetWasm も HEAD のライブラリを使っていない。

### Cited Findings
- `git submodule status` は ` 526498a9ec16729692758300fa584e6e5577f7c4 SortAlgorithms (1.3.0-139-g526498a)` で、先頭の空白が示すとおり Sortvivo2 の記録と作業ツリーは一致 — `SV` の git 出力。`SV/SortAlgorithms` の `git rev-parse HEAD` も同値。
- URL は `https://github.com/guitarrapc/SortAlgorithms.git` — `SV/.gitmodules`
- `origin/main`、`origin/v2`、`HEAD` の gitlink はすべて `526498a` — `git ls-tree`
- SA の HEAD は `f674c43c4296c8529e463bdfb687ed29cd9f5d84` (2026-11-18 03:59 +0900) — `SA` の git 出力
- `526498a` は `f674c43` の祖先で、ahead/behind は 0 / 32 — `git merge-base --is-ancestor`, `git rev-list --left-right --count`
- `src/SortAlgorithm` の差分は 58 ファイル、+5,842 / −1,286 行 — `git diff --stat 526498a f674c43 -- src/SortAlgorithm`
- HEAD にだけあるもの: `net11.0` 化(`d473858`、submodule は `net10.0`)、`Alg/PreparedKey.cs`、`Alg/Merge/QuadSort.cs`、`Alg/Partition/FluxSort.cs`、`Alg/Partition/SampleSort.cs`、`Alg/Network/PairwiseSortingNetwork.cs`、`Alg/Selection/BingoSort.cs` — 同 diff、`SVsub/` の存在確認。
- 最後の submodule 更新は `dad7393c` (2026-11-14) — `SV` の `git log -- SortAlgorithms`
- 監査中に `SV` の作業ツリーへ `src/SortVivo.Web/src/lib/quiz-play.ts`、`quiz-result.ts` の未コミット変更が現れた。開始時の `git status --short` は空だった。エンジンと submodule には無関係で、本監査による変更ではない — `git status`

### Inferences
- `ENG/` の計測・サイズ・実物解析は 526498a の結果であり、HEAD の `PreparedKey`(`WithPreparedKey` のジェネリックループ)と `[InlineArray]` の影響を含まない。
- submodule を更新すると、NetWasm の 24 byte `SortSpan` 複製問題に対して、`SortSpan` を引数に取る `IPreparedKeyLoop.Run` が増える。HEAD の変更は x64 の JIT 前提で評価されているので、Wasm で同じ結果になるとは限らない。

### Gaps
- HEAD のライブラリが NetWasm 0.14.2 と Mono AOT でビルドできるか、速度とサイズがどう変わるかは未確認。ビルド禁止のため本監査では検証していない。
- 追加アルゴリズムを SortVivo の Registry に載せる影響は未評価。
