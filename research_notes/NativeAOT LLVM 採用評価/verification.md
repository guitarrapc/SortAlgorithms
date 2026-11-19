# NativeAOT-LLVM 採用評価: SortVivo の SortAlgorithms エンジン向け技術検証 (実機)

検証日: 2026-11-19。対象ライブラリは `D:\github\guitarrapc\SortAlgorithms` の `f674c43` を `git archive` で作業領域へ展開したもの (作業ツリーは不使用)。SortVivo 側の記録ロジックは `D:\github\guitarrapc\Sortvivo2` (HEAD `0f772f78`、読み取りのみ) からファイルをコピーした。この文書の数値はすべてこのマシンで自分が実行した結果であり、実行していない項目は「未測定」と理由を書いた。

- 作業領域: `C:\Users\guitarrapc\AppData\Local\Temp\claude\8ffd9513d941f0a1\4ac9fb56-b669-4e11-a61a-d1ccca49da70\scratchpad\wasm-audit\naot-eval` (以下 `E`)
- 前回の監査ノート: `D:\github\guitarrapc\SortAlgorithms\research_notes\NET 11 WASM 実行方式監査\measurements.md`, `nativeaot_llvm.md`
- 環境: Windows 11 Pro 10.0.26200、Ryzen 9 7950X3D、.NET SDK 11.0.100 (ほかに 10.0.404 / 8.0.416)、workload `wasm-tools` と `wasm-tools-net10` (11.0.100)、Node v24.18.0、Edge 157.0.4341、WSL Ubuntu 24.04.5 (dotnet 10.0.115)
- NativeAOT-LLVM: `Microsoft.DotNet.ILCompiler.LLVM` / `runtime.win-x64.Microsoft.DotNet.ILCompiler.LLVM` / `runtime.browser-wasm.Microsoft.DotNet.ILCompiler.LLVM` `10.0.0-rc.1.26357.1` (dotnet-experimental)。Emscripten 3.1.56 は workload パック `Microsoft.NET.Runtime.Emscripten.3.1.56.*.win-x64\10.0.15` を再利用
- パッケージ: `E\pkgs\<set>` を `NUGET_PACKAGES` に指定 (前回の `wasm-audit\pkgs` からコピー)。NuGet.config は `E\NuGet.config` (nuget.org + dotnet-experimental、packageSourceMapping 付き)

## 要約

| # | 項目 | 結果 |
| --- | --- | --- |
| 1 | NullContext の定数畳み込み | **完全に消える**。最終 `.wasm` の NullContext 系 45 関数で `call_indirect` 0、文脈・`Report*`・インターフェース解決への呼び出し 0。`StatisticsContext` と `ISortContext` は同一の共有コード (`__Canon`) で、観測ごとに `RhpResolveInterfaceDispatch` + `call_indirect`、generic なものは `GVMLookupForSlot` が残る。脱仮想化はされない |
| 2 | JSExport なしの境界 | 動くが規律が要る。例外は export 内で必ず `catch` (漏らすと `Aborted()`)。ポインタはモジュールが渡したバッファだけ受け付ける (検証しないと managed ヒープを黙って壊せる)。**Wasm トラップ (範囲外アクセス、スタック溢れ、ヒープ破壊後の GC) が 1 回起きるとそのインスタンスの全 export が以後 `Aborted()`**。呼び出し後は `HEAP*` を取り直す (detach したビューへの書き込みは黙って捨てられる)。JS に渡すバッファは pinned 配列か `NativeMemory` (通常の配列は GC で 5/5 移動し、古いアドレスへ書くと GC がハング)。Node `worker_threads` と Edge の classic / module worker で動作。`DotNetJsApi` は例外の型とメッセージが失われ、`[JSExport]` の `Span` 引数はコンパイルを通るのに実行時にランタイムを壊す |
| 3 | ビルド | Windows で SDK 11.0.100 と 10.0.404 の両方で成功。Emscripten 3.1.56 は workload パックを `EmscriptenSdkToolsPath` で指定し、Python・設定・書き込み可能なキャッシュをプロセス環境変数で与える必要がある。クリーン 9〜10 秒、変更なし 0.7 秒、1 ファイル変更 8.3 秒。`MSBuildEnableWorkloadResolver=false` は同じソリューションの Mono プロジェクトを壊さないが、3.1.56 用の環境変数は .NET 11 Mono の relink を壊す。Linux (WSL) は `/mnt/c` 上でのタスク読み込み失敗で**未検証** |
| 4 | .NET 10 互換 | `f674c43` のライブラリは net10.0 / C# 14 でそのままビルド (エラー 0、警告 0)。TUnit は net10.0 で 12,520 件中成功 11,349、失敗 0、スキップ 1,171 (net11.0 と同数) |
| 5 | 記録経路の速度とメモリ | SortVivo の記録経路 (Composite + 記録、`Record(count=-1)`) では **NativeAOT-LLVM と NetWasm がほぼ同等** (native-jit 比 NativeAOT-LLVM 1.28〜1.55、NetWasm 1.09〜1.63)。高速経路は NativeAOT-LLVM 1.01〜1.62 対 NetWasm 1.81〜4.09。数えるだけの観測は NetWasm 0.89〜1.33 対 NativeAOT-LLVM 1.75〜2.63。Mono AOT は観測系・記録経路で native-jit 比 3.7〜9.7 倍。線形メモリは NativeAOT-LLVM が初期化直後 18 MB → 最初の確保で 62 MB、n=10,000 で最大 129 MB。NetWasm は 1.3 MB 開始で最大 66 MB |
| 6 | `OptimizationPreference=Speed` | **`IlcPgoOptimize=false` を足せばビルドできる**。原因は `--Ot` ではなく `.mibc` の読み込み (`IlcPgoOptimize=true` 単独でも同じ NRE)。Speed はインライン展開を増やすだけで脱仮想化はしない。高速経路の改善なし、StatisticsContext は約 11%、記録経路は 2〜4% 速く、サイズは +3〜7% |
| 7 | サイズゲート (proxy) | SortVivo と同じ 87 算法・71 パターンの proxy で、Brotli 合計は NetWasm 465,102 B (実エンジン 469,666 B と 1% 差) に対し NativeAOT-LLVM 既定 528,910 B、Speed 563,100 B、最小構成 (`Size` + `-Oz`) 486,016 B。エンジン予算 614,400 B には入るが、NetWasm 比 +21〜98 KB で全体予算の余裕約 2 KB を超える。87 算法すべてコンパイル・実行でき、並べ替え結果と記録フレームは CoreCLR / NativeAOT-LLVM / NetWasm で一致 |
| 8 | libm 決定性 | 71 パターン × 13 サイズの 923 入力で 3 ツールチェーンが完全一致、不一致 0 |
| 追加 | `IlcLlvmExceptionHandlingModel=emulated` | legacy EH 命令が 0 になる。Brotli +13 KB (ハーネス) / +30 KB (proxy)、観測系 +10〜18%、高速経路は同等、2a の挙動は不変。Edge 157 は既定ビルドで legacy EH の警告を出さなかった |

採用判断への含意 (推論): SortVivo の主な負荷である記録経路では NativeAOT-LLVM の速度上の利点はほぼなく、サイズ (+21〜98 KB) とメモリ (約 2 倍) で不利になる。利点が大きいのは高速経路 (`Sort` / `Batch`) と境界呼び出しのコストである。採用する場合は、export を全例外捕捉 + ポインタ検証で包み、トラップ後は Worker ごと作り直す運用が前提になる。

## 1. NullContext 高速経路の定数畳み込み (生成バイナリの検査)

### 結論
- **NullContext 実体化では観測呼び出しが完全に消えている**。最終 `.wasm` の NullContext 系 45 関数 (PDQSort / TimSort / HeapSort / RadixLSD256Sort / Driftsort / ShellSortCiura2001 / FlashSort / BucketSortInteger とその補助) に `call_indirect` は 0 個、`ISortContext`・`StatisticsContext`・`SortSpan` の `Report*` 補助・`RhpResolveInterfaceDispatch`・`GVMLookupForSlot` への呼び出しも 0 個だった。NullContext のメソッド本体自体がバイナリに存在しない。
- **StatisticsContext と ISortContext は同じ共有コード (`__Canon`) を使い、脱仮想化されていない**。`StatisticsContext` は sealed class だが参照型なので、`PDQSort.Sort<int, StatisticsContext>` と `Sort<int, ISortContext>` は同一の `PDQSort__Sort_0<Int32, System.__Canon>` を呼ぶ (`Kernel.SortObserved` から同じ関数への呼び出しが 2 箇所)。違いは generic dictionary (`__GenericDict_..._StatisticsContext` / `_ISortContext`) だけである。
- 共有コードでは観測 1 回ごとに `RhpResolveInterfaceDispatch` → `call_indirect` が入り、`OnIndexWrite<T>` / `OnRangeCopy<T>` (generic virtual method) は `GVMLookupForSlot` を経由する。`SortSpan` の `ReportCompare*` もインライン展開されず関数呼び出しで残る。
- NullContext 経路に残る文脈と無関係なオーバーヘッドとして、要素アクセスごとに `i32.const 1024; i32.lt_u; br_if` の明示 null チェック (アドレスが先頭 1 KiB 未満なら NullReferenceException 経路) が入っている。

### 方法
1. 既定構成 (`tc\naot-llvm`, clang `-O3` / link `-O1`) を `-p:IlcGenerateMapFile=true` 付きで publish。ILC が出す LLVM モジュール 16 個の再配置可能オブジェクト `E\art\naot-llvm\obj\naot-llvm\release_browser-wasm\native\audit.{0..15}.o` と `audit.o` にはシンボル表があるので、`llvm-objdump -d -r` (workload パック `...\Emscripten.3.1.56.Sdk.win-x64\10.0.15\tools\bin\llvm-objdump.exe`) で逆アセンブルし、`E\scripts\objdis.mjs` で関数ごとの呼び出し先・`call_indirect` 数を集計した。
2. 最終バイナリでも確かめるため、同じ `link.rsp` から `-Wl,--strip-all` だけを外し `--profiling-funcs` を足して emcc で再リンクした (`E\raw\naot-llvm\named\link-named.rsp`)。**コードセクションは元の `audit.wasm` とバイト一致** (サイズ 922,926 B、SHA-256 先頭 `08fbdbcfd6acaeb3` が一致、差は name セクション 260,237 B のみ)。この名前付き版を `wasm-dis` でテキスト化し、`E\scripts\watcalls.mjs` で関数ごとの `call` / `call_indirect` を数えた。
3. ILC のオプションとしては `IlcGenerateMapFile` (`--map`)、`IlcGenerateMstatFile`、`IlcGenerateDgmlFile`、`IlcDumpIL`、`NativeDebugSymbols` (`-g`/`-g3`) が targets にある。LLVM IR そのものは中間の `audit.N.bc` に残る。今回は `.o` のシンボルと名前付き再リンクで足りたので IR ダンプは使っていない。

### 集計 (`.o` 段階。`E\raw\naot-llvm\funcs.json`)

| 対象 | 関数数 | `call_indirect` | `RhpResolveInterfaceDispatch` 呼び出し | `GVMLookupForSlot` 呼び出し | `SortSpan.Report*` 呼び出し |
| --- | --- | --- | --- | --- | --- |
| NullContext 実体化 (名前に `NullContext` を含む) | 41 | 0 | 0 | 0 | 0 |
| 共有実体化 (`Algorithms_*<...System___Canon>`) | 75 | 528 | 265 | 126 | 124 |

NullContext 関数群の呼び出し先 (同じ NullContext 実体化の関数を除く) は `ThrowNullReferenceException` などの例外送出補助、`SpanHelpers.Memmove` / `ClearWithoutReferences`、`ArrayPool<int>.Rent/Return`、`BitOperations.Log2SoftwareFallback`、`UInt128.op_Division`、`Math.BigMul` のソフトウェア実装、`RhpNewFast` (例外オブジェクト生成) だけだった。

### 最終バイナリ (名前付き再リンク) の抜粋 (`E\raw\naot-llvm\named\calls-final.txt`)

| 関数 | 行数 | `call_indirect` | 主な呼び出し先 |
| --- | --- | --- | --- |
| `PDQSort.PDQSortLoop<int, CC, NullContext>` | 2,237 | 0 | 自身 ×2、`PartitionLeft`/`PartitionRightSkipEquals`/`HeapSort.SortFallbackCore` (いずれも NullContext 版)、`ThrowNullReferenceException` |
| `PDQSort.PDQSortLoop<int, CC, __Canon>` | 6,633 | 66 | `RhpResolveInterfaceDispatch` ×48、`GVMLookupForSlot` ×9、`SortSpan.ReportCompareAt` ×18、`ReportCompareElement` ×4、`ReportCompareValue` ×2 |
| `PDQSort.PartitionRight<…, NullContext>` | 327 | 0 | `ThrowNullReferenceException` のみ |
| `PDQSort.PartitionRight<…, __Canon>` | 699 | 7 | `RhpResolveInterfaceDispatch` ×3、`ReportCompareElement` ×5、`GVMLookupForSlot` ×2 |
| `TimSort.MergeLow<…, __Canon>` | 2,907 | 38 | `RhpResolveInterfaceDispatch` ×8、`GVMLookupForSlot` ×15 |
| `HeapSort.Sort<int>` (NullContext、本体をインライン展開) | 542 | 0 | `ThrowNullReferenceException`、`ThrowGreater` |
| `HeapSort.Sort_0<int, __Canon>` | 1,497 | 25 | `RhpResolveInterfaceDispatch` ×13、`GVMLookupForSlot` ×6 |
| `RadixLSD256Sort.LSDSort<…, __Canon>` | 1,062 | 7 | `RhpResolveInterfaceDispatch` ×3、`GVMLookupForSlot` ×2、`ReportBuckets<__Canon>` |
| `WasmAudit.Kernel.SortObserved` | 318 | 0 | `PDQSort__Sort_0<Int32, __Canon>` ×2 (Statistics 用と ISortContext 用が同じ関数) |
| `CompositeContext.OnCompare` | 96 | 1 | `RhpResolveInterfaceDispatch` ×1 (内側の各コンテキストへ再度インターフェース呼び出し) |
| `CompositeContext.OnIndexWrite<int>` | 132 | 2 | `GVMLookupForSlot` ×1 (内側でも GVM 解決) |
| `StatisticsContext.OnCompare` | 22 | 0 | 本体は単純な加算。呼び出し元から直接は呼ばれない |

`PartitionRight<…, NullContext>` の内側ループ (`while (s.Less(++first, pivot))` に相当、`E\raw\naot-llvm\excerpt-PartitionRight-NullContext.txt`):

```
loop
  local.get 5 ; local.get 2 ; local.tee 9 ; i32.const 1 ; i32.add ; local.tee 2
  i32.const 2 ; i32.shl ; i32.add ; local.tee 10
  i32.const 1024 ; i32.lt_u ; br_if 1        ;; null チェック (文脈とは無関係)
  local.get 8 ; i32.const 1 ; i32.add ; local.set 8
  local.get 10 ; i32.load 0 ; local.get 7 ; i32.lt_s
  br_if 0                                    ;; 比較して継続
end
```

同じ位置の共有実体化 (`E\raw\naot-llvm\excerpt-PartitionRight-Canon.txt`) は、先頭で SortSpan から文脈参照を読み (`local.get 3; i32.load 0`)、`i32.eqz` で null 判定し、`OnIndexRead` を次の形で呼ぶ:

```
i32.const 0 <__InterfaceDispatchCell_ISortContext__OnIndexRead_PDQSort__PartitionRight<…__Canon>>
call RhpResolveInterfaceDispatch
call_indirect 12
```

### Mono AOT / NetWasm との対比
- **NetWasm 0.14.2: 未測定 (関数名を得られなかった)**。`audit.wasm` に name セクションがない。`NetWasmManagedStackTrace=true` で `audit.wasm.method-symbols` (独自のバイナリ形式、先頭 `NWSY`) を出せることは確認してビルドした (`E\out\netwasm-o3-sym`) が、形式が文書化されておらず、関数インデックスとの対応付けは安価な範囲を超えるため行っていない。
- **Mono AOT (関数一覧のみ)**: `WasmNativeStrip=false` で名前付きの `dotnet.native.wasm` を作り (`E\out\mono-aot-named`。コードセクションは計測用の `mono-aot` と一致しなかったので参考値)、`wasm-dis` で関数名を列挙した。PDQSort / TimSort / HeapSort の AOT 実体は、NullContext 版 (`aot_instances_SortAlgorithm_Algorithms_PDQSort_PDQSortLoop_int_…_NullContext_…` など)、入口の `PDQSort_Sort_T_INT_TContext_REF`、参照型要素向けの `…_T_REF_TComparer_REF_TContext_REF` 版だけだった。**`<int, ComparableComparer<int>, 参照型コンテキスト>` の `PDQSortLoop`、`TimSort.MergeLow`、`HeapSort.SortCore` などの AOT 実体は存在しなかった** (`E\raw\mono-aot-named\pdq-funcs.txt`)。命令単位の確認はしていない。

### 推論
- `StatisticsContext` を sealed class のまま使う限り、NativeAOT-LLVM でも CoreCLR と同様に共有ジェネリックになり、インターフェース呼び出しが残る。SortVivo の記録経路 (`Dispatch.Observe(int, Span<int>, ISortContext)` → `CompositeContext`) も同じ共有コードを通り、さらに Composite 内で 2 回目のインターフェース/GVM 解決が入る。
- 観測経路の倍率が NativeAOT-LLVM で 8〜20 倍と大きかった前回の結果 (measurements.md Q3) は、この `RhpResolveInterfaceDispatch` + `call_indirect` と `GVMLookupForSlot` が操作ごとに入ることと矛盾しない。ただし関数単位の時間計測はしていない。

## 2. JSExport を使わないモデルの境界挙動

検証用 export は `E\tc\boundary\Boundary.cs` (`[UnmanagedCallersOnly(EntryPoint = "bnd_*")]`、既定 `INITIAL_MEMORY` 16 MiB)。JS 側は `E\scripts\boundary.mjs` (a〜c)、`E\scripts\bnd-own.mjs` + `bnd-own-trial.mjs` (d)、`E\scripts\worker-node.mjs` / `edge-worker.mjs` (e)、`E\scripts\jsapi-probe.mjs` / `jsapi-spanexport.mjs` (f)。結果は `E\results\boundary.json`, `boundary-d.json`, `worker-node.json`, `worker-edge.json`, `jsapi.json`, `jsapi-spanexport.json`。各ケースは新しいモジュールインスタンスで実行した。

### (a) export 内で例外を投げた場合

| 例外 | JS が受け取るもの | その後の同一インスタンス |
| --- | --- | --- |
| `InvalidOperationException` / `NullReferenceException` / `IndexOutOfRangeException` / `DivideByZeroException` を export の外へ漏らす | `RuntimeError: Aborted(). Build with -sASSERTIONS for more info.`。`printErr` に `Unhandled exception. System.InvalidOperationException: managed throw from export` とスタック (`at NaotEval.Boundary.Thrower(Int32):0x3a93e` など) | `bnd_ok`、確保 + `GC.Collect` + PDQSort を行う `bnd_health`、`try/catch` 付き export はすべて正常に動いた。同じインスタンスで 300 回続けて漏らしても変化なし |
| export 内で `catch` して戻り値で返す | 戻り値 `-1`、例外型名も取得できた | 正常 |
| 無限再帰 (`stackalloc` 256 B/段) | `RuntimeError: null function or function signature mismatch` (スタックオーバーフローの検出ではなく、メモリ破壊後のトラップ) | 以後すべての export が `Aborted()`。`catch` で包んでも同じで、回復不能 |

- 漏れた例外は C++/Wasm 例外として JS へは出ず、ランタイムの未処理例外処理が `abort()` を呼ぶ形になる。Emscripten は `ASSERTIONS` なしではアボート後の呼び出しを拒否しないので、その後も呼べてしまう。これは「動き続ける保証」ではなく、未処理例外の扱いとしては FailFast 後の状態である。
- 1 MiB の既定スタック (`IlcWasmStackSize`) を超える再帰は検出されずに線形メモリを壊す。SortVivo の算法で再帰が深くなる入力 (クイックソートの敵対入力など) は、この上限を前提に確認が要る。

### (b) 不正なポインタ・長さ

| ケース (検証なしの `bnd_sum_raw` / `bnd_fill_raw` / `bnd_sort_raw`) | 結果 | その後 |
| --- | --- | --- |
| 確保したバッファ外だがメモリ内を読む | 例外なしで値を返した (無関係なメモリの合計) | 正常 |
| managed 配列のデータ領域へ書く (`victim` 配列のアドレスへ 16 要素) | 例外なし。managed 配列の 16 要素すべてが書き換わった | 正常 (静かな破壊) |
| managed 配列のヘッダ (MethodTable*・Length) を上書き | 書き込みは成功、`victim.Length` が化けた (検査値 1002)。次の GC で `RuntimeError: memory access out of bounds` | 以後の呼び出しは `Fatal error. Invalid Program: attempted to call a UnmanagedCallersOnly method from managed code.` → `Aborted()` で全滅 |
| 長さが負 | `new Span<int>(p, -1)` が `ArgumentOutOfRangeException` → 未処理例外で `Aborted()` | 呼び出しは継続可能 (a と同じ) |
| ポインタ 0 / 512 (先頭 1 KiB) | 明示 null チェックにより `NullReferenceException` → `Aborted()` | 継続可能 |
| メモリ末尾を越えて読む・書く、長さ `0x7fffffff`、末尾越えのソート | `RuntimeError: memory access out of bounds` (トラップ) | **以後すべての export が `Fatal error. Invalid Program: …` で `Aborted()`**。トラップが reverse P/Invoke の後始末を飛ばすため、スレッド状態が「managed 実行中」のまま残る |

検証付き export (`bnd_sort_checked`) は全ケースで期待どおりのコードを返し、その後の `bnd_health` も正常だった: 正常 → `0`、別のポインタ → `-14`、負の長さ → `-22`、容量超過 → `-22`、null → `-14`、メモリ末尾越え → `-14`。検証は次のとおり (ポインタはモジュールが渡したもの以外を受け付けない)。

```csharp
[UnmanagedCallersOnly(EntryPoint = "bnd_sort_checked")]
public static int SortChecked(int* p, int n)
{
    if (p == null || p != owned) return EFault;              // only the buffer this module handed out
    if ((uint)n > (uint)ownedCap) return EInval;              // also rejects n < 0
    try { PDQSort.Sort(new Span<int>(p, n)); return 0; }
    catch { return EUnknown; }
}
```

### (c) 呼び出し中のメモリ拡張

| 拡張の原因 | 拡張前 | 拡張後 | 呼び出し前に取った `HEAP32` | `subarray` で作ったビュー | `M.HEAP32` |
| --- | --- | --- | --- | --- | --- |
| `NativeMemory.Alloc(64 MiB)` | 18,087,936 B | 120,127,488 B | `byteLength` 0、`length` 0 (detach) | `length` 0 | 新しいビューに差し替わっていた |
| `new byte[64 MiB]` (GC ヒープ) | 18,087,936 B | 164,364,288 B | 同上 | 同上 | 同上 |

- detach したビューへの書き込み (`sub[0] = 12345`) は**例外にならず黙って捨てられた**。
- 正しい形は「export を呼んだ後は必ず `M.HEAP32` / `M.HEAPU8` を取り直す」か、`new Int32Array(M.HEAPU8.buffer, ptr, n)` を呼び出し後に作ること。`wasmMemory` も `-sEXPORTED_RUNTIME_METHODS` に足せば取得できた (`typeof M.wasmMemory === 'object'`)。

```js
const p = M._eng_buffer(n);            // may grow memory
M.HEAP32.set(input, p >> 2);           // read M.HEAP32 after the call, never cache it across calls
M._eng_sort(id, p, n);                 // may grow memory again
const sorted = M.HEAP32.slice(p >> 2, (p >> 2) + n);
```

### (d) バッファの持ち方と GC 移動

各 5 試行、試行ごとに別プロセス (`bnd-own.mjs`、15 秒でタイムアウト)。手順: 4096 要素を確保 → JS が最初に得たアドレスへ 1 を書く → 確保・`GC.Collect(0/1/2, compacting)` を 4 巡 → JS が同じアドレスへ 2 を書く → 所有側の参照から合計を読む。

| 持ち方 | GC 後にアドレスが動いた | 古いアドレスへの書き込み | アドレスを取り直した場合 |
| --- | --- | --- | --- |
| `int[]` を `fixed` で export 内だけ固定し、アドレスを返す | 5/5 で移動 | 書いた値は配列に届かず (合計 4096 のまま)、**直後の `bnd_health` (GC) が 5/5 でハング** | 5/5 で正常 (合計 8192) |
| `GC.AllocateArray<int>(n, pinned: true)` | 0/5 | 正常 | 正常 |
| `NativeMemory.Alloc` | 0/5 | 正常 | 正常 |

- 保守的 GC であっても、静的フィールドから参照される通常の配列は圧縮 GC で動く。JS に渡すアドレスは pinned 配列かネイティブメモリに限るべきである。

### (e) Worker と出力形式

| 環境 | 読み込み方 | 結果 |
| --- | --- | --- |
| Node `worker_threads` ×2 同時 | MODULARIZE ビルドを `require` | 両方で PDQSort n=100,000 (4.9 ms) と記録経路 n=10,000 (13.4 ms) が成功、検証 OK。インスタンスは worker ごとに独立 |
| Node `worker_threads` | `-sEXPORT_ES6=1` ビルドを `import()` | 成功 (`export default createBoundary`) |
| Edge 157 headless、classic dedicated worker ×1 と ×2 同時 | `importScripts('/out/naot-llvm-bench/audit.js')` | 成功 (PDQSort n=100,000 5.0〜6.0 ms、記録 n=10,000 15〜17 ms)。`crossOriginIsolated=false` でも動作 |
| Edge 157 headless、module worker | `import createBoundary from '/out/boundary-es6/boundary.js'` | 成功 |

- 既定の出力は Emscripten の `<AssemblyName>.js` + `.wasm`。今回のプロジェクトは `LinkerArg` で `-sMODULARIZE=1 -sEXPORT_NAME=createAudit` を渡しており、生成 JS は UMD 形式 (`module.exports` / AMD / グローバル `createAudit`)。
- Emscripten のリンク引数は MSBuild から 2 通りで渡せることを確認した: アイテム `<LinkerArg Include="-sEXPORT_ES6=1" />` (`CustomLinkerArg` に連結される) とプロパティ `EmccFlags` (例: `-p:EmccFlags=-O3`。`-O1` の後ろに付く)。`-sEXPORT_ES6=1` は `.js` 出力のまま ES module になり、Node では `.mjs` にコピーして読み込んだ。
- Web worker で使うには `-sENVIRONMENT` に `worker` を含める必要がある (今回の計測ビルドは `node,web,worker`)。

### (f) `DotNetJsApi=true` (`[JSExport]`/`[JSImport]`)

| 項目 | 結果 |
| --- | --- |
| C# 例外 → JS | JS 側は汎用の `Error`、メッセージは常に `C# exception from NativeAOT`。**例外型と元のメッセージは失われる**。その後の呼び出しと GC は正常 |
| JS 例外 → C# (`[JSImport]` 先で throw) | C# 側で `JSException`、メッセージ `Error: thrown in a JS import` |
| JS → C#: `int[]` 引数 | `Int32Array` と通常の `Array` の両方を受け付けた (コピー) |
| C# → JS: `int[]` 戻り値 | `Int32Array` で返る (コピー)。入力の `Int32Array` は変更されない |
| C# → JS: `Span<int>` / `ArraySegment<int>` を `[JSImport]` の `MemoryView` 引数で渡す | 動作。JS 側は `Span` / `ArraySegment` オブジェクト (`set`、`slice`、`dispose`、`isDisposed`)、`slice()` は `Int32Array`。呼び出し後に保持したビューへの `slice`/`length`/`set` は `Assert failed: ObjectDisposedException` |
| JS → C#: 配列を返す `[JSImport]` | 動作 (`int[]` へコピー) |
| JS → C#: `[JSExport]` の `Span<int>` / `ArraySegment<int>` 引数 | **コンパイルは警告なしで通るが実行時に失敗**。`Span` は `Assert failed: Expected MemoryViewType.Int32`、`ArraySegment` は `Assert failed: Only roundtrip of ArraySegment instance created by C#` の後に内部エラー (`Te.abort is not a function`) となり、**以後ランタイムが使えなくなった** (`.NET runtime didn't start yet`) |
| 安定性 | 64 要素の `SortCopy` 10 万回 + `Span` 経由の import 約 100 回 + 例外 21 回 + GC を混ぜて誤り 0、148 ms、終了後も正常 |

- 前回の値 (Brotli 545 KiB、空 export 87 ns) と合わせると、`DotNetJsApi` は動くが、サイズの増分が大きく、例外情報が失われ、生成器が拒否しない不正なシグネチャがランタイムを壊す。直接 export + 線形メモリの形の方が SortVivo の用途 (配列を渡して記録列を返す) では扱いやすい。

## 3. ビルドの流れ

### 前提条件 (このマシンで通った組み合わせ)

| 項目 | 内容 |
| --- | --- |
| SDK | .NET SDK 11.0.100 で `net10.0` を対象に publish。SDK 10.0.404 (`E\sdk10\global.json` で固定) でも同じ `tc\naot-llvm` の publish が 8 秒で成功し、PDQSort n=100,000 と記録経路 n=10,000 の検証に通った (`E\logs\build-naot-sdk10.log`) |
| パッケージ | `Microsoft.DotNet.ILCompiler.LLVM` と `runtime.win-x64.Microsoft.DotNet.ILCompiler.LLVM` を `10.0.0-rc.1.26357.1` で参照。`runtime.browser-wasm.Microsoft.DotNet.ILCompiler.LLVM` は自動で復元される。ほかに nuget.org から `Microsoft.NET.ILLink.Tasks` 10.0.15 |
| フィード | `https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-experimental/nuget/v3/index.json` (上記 3 パッケージだけを packageSourceMapping で割り当て) + nuget.org |
| 必須プロパティ | `PublishTrimmed=true`、`SelfContained=true`、`MSBuildEnableWorkloadResolver=false`、`-r browser-wasm`、`EmscriptenSdkToolsPath` |
| Emscripten | 3.1.56。`wasm-tools-net10` workload が入れたパック `C:\Program Files\dotnet\packs\Microsoft.NET.Runtime.Emscripten.3.1.56.Sdk.win-x64\10.0.15\tools\` を `-p:EmscriptenSdkToolsPath=...\` で指定 (EMSDK 環境変数は使っていない)。clang は `bin\clang.exe`、リンカは `emscripten\emcc.bat` |
| プロセス環境変数 | `E\scripts\emenv.sh`: `EMSDK_PYTHON` (同 Python パックの `python.exe`)、`EM_CONFIG` (LLVM_ROOT / BINARYEN_ROOT / NODE_JS を書いた自前の設定)、`EM_CACHE` (Cache パックの書き込み可能なコピー)、`PATH` |

ターゲットの探索ロジック (`Microsoft.NETCore.Native.Wasm.targets` の `_InitializeEmsdk`): `EMSDK` があれば `$(EMSDK)\upstream\bin` と `upstream\emscripten`、なければ `$(EmscriptenSdkToolsPath)\bin` と `\emscripten`。Windows では `_EmccScriptExt=.bat` なので `emcc.bat` が必須。

環境変数を減らした試行:
- `EmscriptenSdkToolsPath` だけ: clang による `.bc`→`.o` は通るが、リンクで `Python was not found` (このマシンの `python` は Microsoft Store のエイリアス) → 失敗 (`E\logs\build-naot-noenv.log`)。
- `EMSDK_PYTHON` を足しただけ: `emcc : error : LLVM_ROOT is set to empty value in ...\emscripten\.emscripten`。workload パックの `.emscripten` は `DOTNET_EMSCRIPTEN_LLVM_ROOT` / `DOTNET_EMSCRIPTEN_BINARYEN_ROOT` / `DOTNET_EMSCRIPTEN_NODE_JS` を読み、`FROZEN_CACHE` の既定が真である。したがって Python、ツールのパス 3 つ、書き込み可能なキャッシュの指定が要る。

### 同じソリューション内の他プロジェクトとの干渉
`E\slntest\mixed.slnx` に NativeAOT-LLVM の size proxy (`MSBuildEnableWorkloadResolver=false`)、Mono `Microsoft.NET.Sdk.WebAssembly` (`net11.0`、`WasmBuildNative=true`、workload 必須)、通常の `SortAlgorithm.csproj` を入れて `dotnet build` した。
- Emscripten 用の環境変数なし: **3 プロジェクトとも成功** (59 秒)。`MSBuildEnableWorkloadResolver=false` はプロジェクト単位の評価で効き、Mono プロジェクトの workload 解決を妨げなかった (`E\logs\slntest-build-noenv.log`)。
- `emenv.sh` の環境 (3.1.56 向け `EM_CONFIG` / `PATH`) を同じプロセスで有効にした場合: **Mono の native relink が失敗**。.NET 11 の Mono は Emscripten 6.0.3 用のフラグ (`-wasm-use-legacy-eh=0`) を 3.1.56 の clang 19.1.0 に渡し、`Unknown command line argument` になった (`E\logs\slntest-build.log`)。
- 推論: 干渉の原因はプロジェクトプロパティではなくプロセス環境変数である。NativeAOT-LLVM の publish は専用のステップ (環境変数をそのステップに閉じる) に分けるべきである。なお SortVivo の `SortVivo.slnx` は NetWasm エンジン (`SortKernel.csproj`) を含んでおらず、エンジンは別にビルドされている。

### ビルド時間 (NuGet 取得済み、計測中は他の作業なし)

| 対象 | クリーン | 変更なし再実行 | 1 ファイルを `touch` |
| --- | --- | --- | --- |
| NativeAOT-LLVM size proxy (SortVivo の 87 算法) | 9.4 秒、`--disable-build-servers` で 10.1 秒 | 0.7 秒 | 8.3 秒 |
| NativeAOT-LLVM 計測ハーネス | 7.2 秒 | 0.6 秒 | 未測定 |
| NetWasm 0.14.2 size proxy (同じソース) | 30.5 秒 | 1.2 秒 | 未測定 |

`E\results\build-times-flow*.json`。1 ファイルの変更でも ILC → clang (16 モジュール) → emcc のリンクがすべてやり直しになる。表の「未測定」は、`touch` 後の再ビルドを size proxy の NativeAOT-LLVM 版だけで代表させ、他の 2 つは実行しなかったため。

### 出力と読み込み
- 出力: `<AssemblyName>.js` (Emscripten グルー) と `<AssemblyName>.wasm` の 2 ファイル (+ `.deps.json`、配信不要)。
- 読み込み:

```js
// Node (CommonJS factory from -sMODULARIZE=1 -sEXPORT_NAME=createAudit)
const create = require('./audit.js');
const M = await create({ locateFile: (f) => dir + '/' + f });
// Browser main thread: <script src="audit.js"></script>, then createAudit({ locateFile })
// Worker: importScripts('audit.js'); createAudit(...)
// -sEXPORT_ES6=1: import createX from './x.js'; const M = await createX();
const p = M._audit_bufptr(n); M.HEAP32.set(src, p >> 2); M._audit_run(algo, ctx);
```

### Windows 固有の事項
- `emcc.bat` が前提 (targets が `.bat` を付ける)。3.1.56 のパックには `emcc.bat` がある。
- パスの空白 (`C:\Program Files\...`) は問題にならなかった。
- `python` が Store エイリアスに解決されるため `EMSDK_PYTHON` の指定が必要だった。
- 誤操作の記録: ビルド時間を測る前に `dotnet build-server shutdown` を一度実行し、応答がなかったため強制終了した。これはユーザー全体のビルドサーバーに作用するコマンドであり、以後は使っていない。

### WSL Ubuntu (Linux CI の代わり)
- WSL Ubuntu 24.04.5 に `/usr/bin/dotnet` (SDK 10.0.115)、`python3` 3.12.3 がある。node、emcc、clang はない。
- 何もインストールせずに試すため、linux-x64 用の Emscripten 3.1.56 workload パック (`Microsoft.NET.Runtime.Emscripten.3.1.56.Sdk/Node/Cache.linux-x64` 10.0.15、nuget.org) を `PackageDownload` で `E\pkgs\linux` へ取得し、`E\wsl\wsl-build.sh` で `/mnt/c/...` 上のまま publish した。
- 結果: 復元 (dotnet-experimental から `runtime.linux-x64.Microsoft.DotNet.ILCompiler.LLVM` を含む) と C# コンパイルは成功。その後 MSBuild がタスクアセンブリを `/mnt/c` から読み込む段階で `System.IO.IOException: No such device` と `System.OutOfMemoryException` (MSBuild の未処理例外) になり、ILC まで到達しなかった (`E\logs\wsl-build.log`)。
- パッケージを WSL のファイルシステムへ置けば回避できる可能性が高いが、作業領域の外に書くことになるため行っていない。**Linux CI での NativeAOT-LLVM ビルドは未検証**。

## 4. ライブラリの .NET 10 互換性

`f674c43` を `E\repo-net10` に別途展開し、そのコピーの `Directory.Build.props` の `TargetFramework` だけを `net11.0` → `net10.0` に変えた (`E\repo` は無変更)。

| 確認 | コマンド (要旨) | 結果 |
| --- | --- | --- |
| `src\SortAlgorithm` を SDK 11.0.100 で net10.0 ビルド | `dotnet build src/SortAlgorithm/SortAlgorithm.csproj -c Release` | 成功、エラー 0、警告 0。実効 `LangVersion` は `14.0` (`MaxSupportedLangVersion` も 14.0) |
| 同 SDK 10.0.404 | `E\sdk10\global.json` (`10.0.404`, `rollForward: disable`) から同じプロジェクトをビルド | 成功、エラー 0、警告 0 |
| TUnit テストを net10.0 で実行 | `dotnet build tests/SortAlgorithm.Tests -c Release` → `SortAlgorithm.Tests.exe --progress off --no-ansi` | **total 12,520 / 成功 11,349 / 失敗 0 / スキップ 1,171**、8.6 秒 (Microsoft.NETCore.App 10.0.15 上) |
| 比較: 同じテストを net11.0 (`E\repo`) | 同上 | total 12,520 / 成功 11,349 / 失敗 0 / スキップ 1,171、8.3 秒 |

- テストビルドの警告 6 件はいずれも `CS0618` (TUnit の旧 API `HasSingleItem` 系の使用) で、TFM とは無関係。
- **.NET 11 / C# 15 専用の API・構文による不一致は 1 件もなかった**。NativeAOT-LLVM のハーネス (`LangVersion` 既定 = 14) でも同じソースがそのままコンパイルされた。
- スキップ 1,171 件はテスト側の条件 (`Skip large inputs for order test` など) で、net10/net11 で同数。
- ログ: `E\logs\net10-lib-sdk11.log`, `net10-lib-sdk10.log`, `net10-tests-run.log`, `net11-tests-run.log`。HTML レポート: `E\raw\net10\SortAlgorithm.Tests-windows-net10.0-report.html`。

## 5. SortVivo に近い記録経路のベンチマーク

### 方法
- 記録コンテキストは SortVivo のファイルをコピーして使った (`E\shared\vivo\`: `OperationRecordingContext`, `OperationCountingContext`, `ReadCoalescer`, `SortOperation`, `OperationType`, `AbsorbedReadFlags`, `OperationLimitExceededException`。Sortvivo2 `0f772f78`)。`CompositeContext` はライブラリ (`f674c43`) のもの。
- `E\shared\Kernel.cs` に 2 つの経路を足した。どちらも `Dispatch.Observe(int, Span<int>, ISortContext)` と同じく `XSort.Sort(arr, ctx)` を `ISortContext` 型の変数で呼ぶ (TContext = `ISortContext`)。
  - ctx 3 `record(Composite)`: `Exports.cs` 165〜171 行そのもの (`new List<SortOperation>(count + 16)`、`new StatisticsContext()`、`new OperationRecordingContext(ops, 9_000_000, coalesce: true)`、`new CompositeContext(stats, record)` で Observe、`FlushPendingReads()`)。`count` は計測区間の外で `OperationCountingContext` により事前に数えた (`Precount`)。
  - ctx 4 `Record(count=-1)`: SortVivo の `engine-worker.mjs` が実際に呼ぶ `Record(id, input, true, -1)` の、エンコード (`PocCodec.Encode`) を除いた部分。入力のコピー、`OperationCountingContext` による数え上げ、記録パスまでを計測する。
  - 記録数とプリフライトの数が一致しなければ例外にした (全セルで一致)。
- 比較のため同じプロセスで ctx 0 (NullContext)、ctx 1 (`StatisticsContext` を型引数に指定)、ctx 2 (`ISortContext`) も測った。
- ツールチェーン (すべて `f674c43` のソースから今回ビルド):
  - `native-jit`: CoreCLR JIT win-x64 net11.0、`DOTNET_TieredCompilation=0`
  - `naot-llvm`: NativeAOT-LLVM 既定 (clang `-O3`、link `-O1`)、Emscripten 既定の INITIAL_MEMORY
  - `naot-llvm-speed`: 上記 + `OptimizationPreference=Speed` + `IlcPgoOptimize=false` (6 章)
  - `naot-llvm-emueh`: 上記既定 + `IlcLlvmExceptionHandlingModel=emulated` (追加章)
  - `mono-aot`: Mono full AOT net11.0、`-O3`、SIMD、既定の初期ヒープ
  - `netwasm-o3`: NetWasm 0.14.2 O3、NetWasm GC、inlining hints `Auto`
  - `netwasm-o3-ignore`: 同上 + `IgnoreAggressive` (SortVivo エンジンと同じ設定)
- 計画 (`plan vivo`): n = 1,000 (1 回の呼び出しで 64 配列) と 10,000 (1 配列)、random と sorted、算法は PDQSort / TimSort / Driftsort / HeapSort / ShellSortCiura2001 / RadixLSD256Sort (+ ctx 0 のみ C# の素朴なクイックソート)。1 セルはウォームアップ 3 回 + 5 サンプル、毎回別シード。
- ノイズ対策は前回と同じ: 7 ツールチェーンを round robin で 3 ラウンド、1 プロセスずつ直列、親プロセスの affinity を論理 CPU 0〜15 に固定、同一プロセスで JS の plainQuick と `Int32Array.sort` をアンカーとして計測。表の ns/要素はラウンド中央値を「同一プロセスの plainQuick(JS) 比 × 全プロセスの plainQuick(JS) 中央値」でドリフト補正した値の中央値。計測中は他の作業をしていない。
- 実行: `node scripts/orchestrate-vivo.mjs vivo,mem --rounds 3` (07:25〜07:36 UTC)。生データ `E\results\vivo-speed.jsonl`、集計 `E\results\vivo-report.md` (`E\scripts\analyze-vivo.mjs`)。

### 正しさ
- 21 プロセス、プロセスエラー 0、検証失敗 0 (整列判定、多重集合チェックサム、JS 側ソートとの FNV ダイジェスト照合)。
- 比較回数・交換回数・読み書き回数・記録操作数は、同じラウンド・同じセルの 288 キーすべてで 7 ツールチェーンが一致した。

### 結果 1: 幾何平均 (6 算法、ns/要素、括弧は native-jit 比)

| n / パターン / 経路 | native-jit | naot-llvm | naot-llvm-speed | naot-llvm-emueh | mono-aot | netwasm-o3 | netwasm-o3-ignore |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 1,000 random NullContext | 21.2 | 21.9 (1.03) | 21.9 (1.03) | 22.2 (1.05) | 37.1 (1.74) | 38.5 (1.81) | 42.0 (1.98) |
| 1,000 random StatisticsContext | 117 | 261 (2.22) | 229 (1.95) | 298 (2.54) | 1120 (9.54) | 119 (1.01) | 116 (0.99) |
| 1,000 random ISortContext | 132 | 261 (1.97) | 251 (1.89) | 308 (2.32) | 1290 (9.74) | 132 (0.99) | 134 (1.01) |
| 1,000 random record(Composite) | 872 | 1116 (1.28) | 1074 (1.23) | 1234 (1.42) | 4244 (4.87) | 1184 (1.36) | 1092 (1.25) |
| 1,000 random Record(count=-1) | 1171 | 1540 (1.32) | 1477 (1.26) | 1687 (1.44) | 6089 (5.20) | 1603 (1.37) | 1597 (1.36) |
| 10,000 random NullContext | 25.9 | 28.1 (1.08) | 29.4 (1.14) | 27.6 (1.07) | 52.5 (2.03) | 49.4 (1.91) | 55.9 (2.16) |
| 10,000 random StatisticsContext | 169 | 328 (1.94) | 293 (1.74) | 376 (2.23) | 1337 (7.92) | 157 (0.93) | 151 (0.89) |
| 10,000 random ISortContext | 185 | 324 (1.75) | 312 (1.69) | 377 (2.04) | 1694 (9.16) | 195 (1.05) | 181 (0.98) |
| 10,000 random record(Composite) | 968 | 1305 (1.35) | 1277 (1.32) | 1441 (1.49) | 5462 (5.64) | 1357 (1.40) | 1412 (1.46) |
| 10,000 random Record(count=-1) | 1306 | 1829 (1.40) | 1809 (1.39) | 2030 (1.55) | 7160 (5.48) | 1921 (1.47) | 2094 (1.60) |
| 1,000 sorted NullContext | 1.41 | 2.29 (1.62) | 2.02 (1.43) | 2.27 (1.60) | 3.94 (2.79) | 5.04 (3.56) | 5.78 (4.09) |
| 1,000 sorted record(Composite) | 180 | 262 (1.46) | 214 (1.19) | 239 (1.33) | 660 (3.67) | 221 (1.23) | 196 (1.09) |
| 1,000 sorted Record(count=-1) | 227 | 332 (1.46) | 305 (1.34) | 361 (1.58) | 954 (4.19) | 293 (1.29) | 287 (1.26) |
| 10,000 sorted NullContext | 2.23 | 2.26 (1.01) | 2.30 (1.03) | 2.19 (0.98) | 4.35 (1.95) | 6.33 (2.83) | 6.36 (2.84) |
| 10,000 sorted record(Composite) | 159 | 246 (1.55) | 243 (1.53) | 283 (1.78) | 727 (4.57) | 246 (1.54) | 223 (1.40) |
| 10,000 sorted Record(count=-1) | 221 | 337 (1.53) | 345 (1.56) | 390 (1.76) | 997 (4.51) | 320 (1.45) | 359 (1.63) |

sorted の StatisticsContext / ISortContext と算法別の全表は `E\results\vivo-report.md`。

### 結果 2: n=10,000 random の算法別 (ns/要素)

| 算法 | 経路 | native-jit | naot-llvm | naot-llvm-speed | naot-llvm-emueh | mono-aot | netwasm-o3 | netwasm-o3-ignore |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| PDQSort | NullContext | 31.5 | 33.7 | 35.0 | 32.9 | 41.1 | 45.2 | 46.0 |
| PDQSort | record | 715 | 1014 | 888 | 1084 | 3527 | 1034 | 1058 |
| PDQSort | Record(-1) | 938 | 1488 | 1248 | 1502 | 4783 | 1509 | 1551 |
| TimSort | NullContext | 46.8 | 48.1 | 53.3 | 48.6 | 71.2 | 88.1 | 87.9 |
| TimSort | record | 1416 | 1897 | 1741 | 2046 | 8532 | 1844 | 1942 |
| TimSort | Record(-1) | 1861 | 2626 | 2531 | 2849 | 11464 | 2783 | 2533 |
| Driftsort | NullContext | 14.4 | 26.6 | 26.7 | 25.7 | 43.8 | 79.2 | 86.2 |
| Driftsort | record | 1367 | 1827 | 1781 | 1957 | 9033 | 2062 | 2069 |
| Driftsort | Record(-1) | 1754 | 2576 | 2322 | 2743 | 11738 | 2571 | 2545 |
| HeapSort | NullContext | 41.6 | 27.5 | 31.0 | 25.9 | 45.8 | 69.2 | 112 |
| HeapSort | record | 1820 | 2472 | 2526 | 2774 | 8889 | 2353 | 2579 |
| HeapSort | Record(-1) | 2282 | 3348 | 3739 | 3645 | 13312 | 3491 | 4283 |
| ShellSortCiura2001 | NullContext | 51.8 | 50.7 | 50.6 | 50.1 | 83.4 | 68.0 | 67.6 |
| ShellSortCiura2001 | record | 1065 | 1388 | 1507 | 1644 | 4049 | 1465 | 1547 |
| ShellSortCiura2001 | Record(-1) | 1537 | 2041 | 2174 | 2297 | 5761 | 2191 | 2487 |
| RadixLSD256Sort | NullContext | 6.58 | 8.16 | 8.27 | 8.29 | 43.0 | 9.84 | 11.6 |
| RadixLSD256Sort | record | 306 | 408 | 415 | 452 | 2715 | 462 | 468 |
| RadixLSD256Sort | Record(-1) | 462 | 544 | 588 | 712 | 2730 | 608 | 792 |

1 回の記録あたりの操作数 (ctx 3、全ツールチェーン共通): n=10,000 random で PDQSort 232,901、TimSort 447,453、Driftsort 516,869、HeapSort 551,670、ShellSortCiura2001 293,499、RadixLSD256Sort 131,029 (`SortOperation` は 24 B なので List 本体だけで最大約 13.2 MB)。

### 結果 3: 線形メモリ

新規プロセスで 1 配列 (batch 1) を NullContext ×3 → ctx 3 ×3 → ctx 4 ×3 の順に実行し、各段の後の `memory.buffer.byteLength` を記録した (`E\results\vivo-mem.jsonl`)。Wasm のメモリは縮まないので、最後の値がそのプロセスの最大である。表は 6 算法の最大値。

| ツールチェーン | 初期化後 | n=1,000 ctx3 後 / ctx4 後 | n=10,000 ctx3 後 / ctx4 後 | 最大を出した算法 |
| --- | --- | --- | --- | --- |
| naot-llvm (speed / emueh も同値) | 18,087,936 | 62,062,592 / 62,062,592 | 89,456,640 / 128,843,776 | TimSort と Driftsort |
| mono-aot | 33,554,432 | 33,554,432 / 33,554,432 | 70,778,880 / 101,974,016 | Driftsort |
| netwasm-o3 (ignore も同値) | 1,310,720 | 10,420,224 / 11,272,192 | 53,673,984 / 66,060,288 | Driftsort |
| native-jit (線形メモリではなくプロセスのピークワーキングセット) | 約 27.9 MB | 35,721,216 (ctx4 後の最大) | 82,300,928 (HeapSort) | — |

- NativeAOT-LLVM は最初の managed 確保 (高速経路の 1 回目) で 18 MB → 62 MB に拡張した。以後 n=10,000 の PDQSort / HeapSort / Shell / Radix の記録は 62 MB に収まり、TimSort と Driftsort だけが 89 MB → 129 MB まで拡張した。
- NetWasm は 1.3 MB から始まり必要に応じて拡張する。同じ負荷での最大は NativeAOT-LLVM の約 1/2 (66 MB 対 129 MB)。
- `GC.GetTotalMemory(false)` の最後の値 (n=10,000 Driftsort) は naot-llvm 12.9 MB、netwasm 25.5 MB、mono-aot 42.1 MB だった。

### 推論
- **高速経路**は前回の結論どおり NativeAOT-LLVM が最速の Wasm 系 (native-jit 比 1.01〜1.08、sorted n=1,000 のみ 1.62)。NetWasm は 1.8〜4.1 倍 (sorted n=1,000 が最大)。
- **SortVivo の記録経路では NativeAOT-LLVM と NetWasm の差がほぼ消える**。record / Record(-1) の幾何平均 (8 行) は NativeAOT-LLVM 既定 1.28〜1.55、NetWasm `Auto` 1.23〜1.54、NetWasm `IgnoreAggressive` 1.09〜1.63 で、セルごとのラウンド間ばらつき (中央値 8.2%、90 パーセンタイル 48%) を考えると優劣は付けられない。記録のコストは `List<SortOperation>` への追記、`ReadCoalescer`、`RangeCopy` の配列確保など、どのツールチェーンでも同じ仕事が支配的で、1 章のディスパッチ差の影響は小さい。
- **数えるだけの観測 (Statistics / ISortContext)** は NetWasm が native-jit 並み (0.89〜1.33) で、NativeAOT-LLVM 既定は約 2 倍 (1.75〜2.63)。1 章の `RhpResolveInterfaceDispatch` + `call_indirect`、`GVMLookupForSlot` と合う。
- Mono AOT は観測系と記録経路が native-jit 比 3.7〜9.7 倍遅い。名前付き Mono AOT ビルド (`E\out\mono-aot-named`) の関数一覧には NullContext 版の `aot_instances_*` と、エントリの `PDQSort.Sort<T_INT, TContext_REF>` しかなく、`PDQSortLoop` / `TimSort.MergeLow` などの `<int, …, 参照型コンテキスト>` 版の AOT 実体が見つからなかった (`E\raw\mono-aot-named\pdq-funcs.txt`)。内側がインタープリターで実行されているという前回の推定と矛盾しない (ただし名前付きビルドのコードセクションは計測ビルドと一致しなかった)。
- メモリは NativeAOT-LLVM の方が大きい。初期化直後に 62 MB まで確保するので、複数の Worker を同時に立てる設計ではその分が積み上がる。

## 6. `OptimizationPreference=Speed` のクラッシュと回避策

### 原因の切り分け
- 再現: `-p:AuditSpeedPref=true` (`OptimizationPreference=Speed`) で ILC が `System.AggregateException` を出して失敗。内訳は 14 メソッド分の `Code generation failed for method ...` (`System.FlagsAttribute..ctor()`、`<Module>.DynamicInvoke`、`TypeCast.AreTypesAssignable` など、特定のメソッドではなくコード生成するメソッド全般) で、どれも次のスタックだった (`E\logs\build-naot-llvm-speed-repro.log`)。

```
System.NullReferenceException: Object reference not set to an instance of an object.
   at Internal.JitInterface.CorInfoImpl.getPgoInstrumentationResults(CORINFO_METHOD_STRUCT_*, PgoInstrumentationSchema*&, UInt32&, Byte**, PgoSource&, Boolean&) + 0x8e
   at Internal.JitInterface.CorInfoImpl._getPgoInstrumentationResults(...) + 0x60
   at Internal.JitInterface.CorInfoImpl.CompileMethodInternal(IMethodNode, MethodIL) + 0x26a
   at ILCompiler.LLVMCodegenCompilation.CompileSingleMethod(CorInfoImpl, LLVMMethodCodeNode) + 0xf6
```

- `Microsoft.NETCore.Native.targets` 33 行で `OptimizationPreference=Speed` は `IlcPgoOptimize=true` を既定にし、206/278 行でランタイムパックの `mibc\*.mibc` 6 ファイルを `--mibc:` として ILC に渡す。
- `-p:IlcPgoOptimize=true` だけ (`--Ot` なし、`--mibc` 6 本あり) でも同じ NRE で失敗した (`E\logs\build-naot-llvm-pgo-only.log`)。
- `-p:AuditSpeedPref=true -p:IlcPgoOptimize=false` は成功した (`--Ot` あり、`--mibc` なし。`E\logs\build-naot-llvm-speed-nopgo.log`)。
- **結論: クラッシュの原因は `--Ot` ではなく `.mibc` の読み込み (`IlcPgoOptimize`) である。回避策は `OptimizationPreference=Speed` + `IlcPgoOptimize=false`**。前回試した `--codegenopt:JitDisablePGO=1` は JIT 側の設定で、ILC が mibc を読む経路を止めないため効かなかったと考えられる。コーディネーターから伝えられた「profile data manager が null」という原因説明 (runtimelab PR #3319) は、ソースを確認していないので未検証だが、観測と矛盾しない。

### Speed が PGO 以外に変えるもの
- ILC の応答ファイルの差は `--Ot` の 1 行だけ (`audit.ilc.rsp` を比較)。clang は両方 `-O3` (`WasmOptimizationSetting` は `Size` 以外で `-O3`)。リンクも同じ `-O1`。
- 生成コード (計測ハーネスの `.o` を比較。`E\raw\naot-llvm-bench\funcs.json` と `E\raw\naot-llvm-speed\funcs.json`):

| 指標 | 既定 | Speed |
| --- | --- | --- |
| 関数数 | 2,862 | 2,582 |
| 命令数 (全体) | 309,792 | 333,797 (+7.7%) |
| NullContext 算法関数 (関数数 / 命令数) | 41 / 15,025 | 32 / 14,931 |
| 共有 (`__Canon`) 算法関数 (関数数 / 命令数 / `call_indirect`) | 75 / 35,833 / 528 | 62 / 36,444 / 594 |
| `SortSpan.Report*` への呼び出し | 128 | 102 |
| `Kernel.SortObserved` などハーネス (命令数 / `call_indirect`) | 2,254 / 1 | 7,327 / 84 |

  RyuJIT のインライン展開が積極的になり、`PDQSort.Sort_0<int, __Canon>` などの入口や `SortSpan.Report*` が呼び出し元へ展開された。インターフェース呼び出しの箇所はインライン展開で複製されて増え (`RhpResolveInterfaceDispatch` 呼び出し 360 → 474)、**脱仮想化は起きていない**。
- サイズ (Brotli): 計測ハーネス 318,931 → 328,870 B (+3.1%)、size proxy 528,910 → 563,100 B (+6.5%)。

### 速度の再測定 (5 章と同じ実行)
- 高速経路の幾何平均は既定比で n=1,000 random 1.00、n=10,000 random 1.05 (遅い側)、n=1,000 sorted 0.88、n=10,000 sorted 1.02。セルのばらつきを考えると**高速経路の改善はない**。
- 観測経路は改善した: StatisticsContext n=1,000 random 261 → 229 ns (0.88)、n=10,000 random 328 → 293 ns (0.89)。HeapSort の StatisticsContext は 438 → 260 ns (n=1,000)。
- SortVivo の記録経路は小さい改善: record n=1,000 random 1,116 → 1,074 ns (0.96)、n=10,000 random 1,305 → 1,277 ns (0.98)。Record(-1) 1,540 → 1,477 (0.96)、1,829 → 1,809 (0.99)。
- 前回の `naot-llvm-speed` は `-msimd128` + link `-O3` の構成で、`OptimizationPreference=Speed` ではない点に注意。

## 7. サイズゲート (SortVivo エンジン相当の size proxy)

**これは proxy であり、SortVivo の実エンジンではない。** 違いは次のとおり。
- コンパイル対象は SortVivo.Engine と同じ組 (`Dispatch.cs`、`InputSequences.cs`、`PocCodec.cs`、`ThumbnailContext.cs`、`GlobalUsings.cs`、`AlgorithmId.cs`、記録系 7 ファイル) を Sortvivo2 `0f772f78` から `E\sizeproxy\engine-copy` と `E\shared\vivo` へコピーしたもの。`Dispatch.Plain` (NullContext 高速経路) と `Dispatch.Observe` (`ISortContext` 経路) の両方で **87 算法 (id 0〜86) すべてと入力パターン 71 種**が到達可能。
- SortAlgorithm は SortVivo のサブモジュール `526498a` ではなく `f674c43` (32 コミット先)。
- NetWasm 版は `Exports.cs` をそのまま使う (`[JSExport]`、`clock.mjs`)。NativeAOT-LLVM 版は `Exports.cs` の代わりに同じ操作を `[UnmanagedCallersOnly]` + 線形メモリで実装した `E\sizeproxy\naot\EngineExports.cs` を使う。
- `engine-worker.mjs` (5,906 B raw) と `engine-contract.json` (673 B) は含めていない。

圧縮は SortVivo の `delivery-report.mjs` と同じ Brotli (品質 11、lgwin 24)。gzip は `-9`。`E\scripts\sizes.mjs`、結果 `E\results\size-proxy.json`。単位はバイト、`raw / gzip / Brotli`。

| 構成 | ファイル数 | 合計 | .wasm | JS | その他 (json 等) |
| --- | --- | --- | --- | --- | --- |
| NetWasm 0.14.2 O3, `IgnoreAggressive` (SortVivo と同設定) | 15 | 3,561,394 / 717,096 / **465,102** | 3,347,253 / 662,413 / 417,882 | 201,760 / 51,578 / 44,593 | 12,381 / 3,105 / 2,627 |
| NetWasm 0.14.2 O3, hints `Auto` | 15 | 5,321,387 / 896,397 / 546,545 | 5,107,246 / 841,712 / 499,312 | 201,760 / 51,579 / 44,604 | 12,381 / 3,106 / 2,629 |
| NativeAOT-LLVM 既定 (clang `-O3`, link `-O1`) | 2 | 2,192,109 / 725,008 / **528,910** | 2,111,276 / 702,182 / 509,494 | 80,833 / 22,826 / 19,416 | 0 |
| NativeAOT-LLVM `Speed` (+`IlcPgoOptimize=false`) | 2 | 2,341,819 / 776,368 / 563,100 | 2,260,986 / 753,542 / 543,688 | 80,833 / 22,826 / 19,412 | 0 |
| NativeAOT-LLVM `Size` (clang `-Oz`) | 2 | 2,089,649 / 698,870 / 509,879 | 2,008,816 / 676,045 / 490,471 | 80,833 / 22,825 / 19,408 | 0 |
| NativeAOT-LLVM 既定 + `EmccFlags=-O3` (wasm-opt と JS 圧縮が走る) | 2 | 1,803,734 / 683,820 / 512,388 | 1,767,851 / 673,361 / 503,016 | 35,883 / 10,459 / 9,372 | 0 |
| NativeAOT-LLVM `Speed` + `EmccFlags=-O3` | 2 | 1,940,289 / 730,217 / 544,779 | 1,904,406 / 719,757 / 535,401 | 35,883 / 10,460 / 9,378 | 0 |
| NativeAOT-LLVM `Size` + `EmccFlags=-Oz` | 2 | 1,681,816 / 645,993 / **486,016** | 1,646,112 / 635,576 / 476,690 | 35,704 / 10,417 / 9,326 | 0 |
| NativeAOT-LLVM 既定 + `IlcLlvmExceptionHandlingModel=emulated` | 2 | 2,483,090 / 774,978 / 558,912 | 2,402,257 / 752,152 / 539,496 | 80,833 / 22,826 / 19,416 | 0 |
| 参考: 実エンジンのディレクトリ `Sortvivo2\src\SortVivo.Web\public\engine\` (16:00 時点の読み取り。別セッションが更新中) | 15 | 3,601,098 / 723,293 / 469,666 | 3,387,085 / 668,611 / 422,440 | 201,760 / 51,577 / 44,592 | 12,253 / 3,105 / 2,634 |

- NetWasm proxy は実エンジンのディレクトリより Brotli で 4,564 B 小さく (-1.0%)、ファイル構成は同一だった。記録値 466,440 B とも 0.3% 差であり、proxy は実エンジンのサイズをよく近似している。
- **NativeAOT-LLVM はどの構成でも NetWasm より大きい**。差は既定 +63,808 B (+13.7%)、`Speed` +97,998 B、最小の `Size + -Oz` でも +20,914 B (+4.5%)。
- エンジン予算 614,400 B に対しては全構成が収まる (最大の `Speed` で残り 51,300 B)。一方、全体予算の余裕は約 2 KB なので、**NetWasm からの置き換えはどの構成でも全体予算を超える** (他の削減と組み合わせない限り)。
- 監査の外挿値 655〜675 KB は実測より大きかった。実測は 486,016〜563,100 B (Brotli)。
- `EmccFlags=-O3/-Oz` は wasm-opt を走らせる構成で、NativeAOT-LLVM の文書は wasm-opt がスタックトレース情報を壊すことを理由に既定で走らせていない (nativeaot_llvm.md §5)。`StackTraceSupport=false` の今回のビルドで実行時の不具合は見ていないが、`-Oz` 版の全算法検証はしていない (全算法検証は既定構成のみ)。

### 全算法のコンパイル・実行確認
`E\scripts\vivo-check.mjs` (NativeAOT-LLVM: `E\out\sp-naot-node`、NetWasm: `E\out\sp-netwasm-node\browser`) と `E\sizeproxy\native\Program.cs` (CoreCLR JIT) で、87 算法それぞれを pattern 0 (random) の入力 (`min(512, AlgorithmMax)` 要素、seed 7) で並べ替え、`Record(id, input, coalesce: true, count: -1)` 相当の記録フレームを作った。

- NativeAOT-LLVM: **コンパイル失敗 0、実行時エラー 0、未整列 0** (87/87)。
- 3 ツールチェーン間で、並べ替え結果の FNV-1a、操作数、フレーム長、フレームの FNV-1a が **87 算法すべてで一致** (`E\results\vivo-compare.json`)。BogoSort などの乱数を使う算法も一致した。

## 8. libm と入力生成の決定性

- `ArrayPatterns` は `f674c43` の `src\SortAlgorithm\Utils\ArrayPatterns.cs` にあり、`Math.Sin` (SineWave)、`Math.Cos` (CosineWave)、`Math.Pow(Math.E, …)` (BellCurve)、`Math.Pow(x, 1/3)` / `1/5` (CubeRoot / FifthRoot)、`Math.Pow(x, 5)` (Quintic)、`Math.Log(…)/Math.Log(2)` (ShuffledFinalMerge など)、`Math.Sqrt` を使う。SortVivo は `Dispatch.Input` からこれを呼ぶ (`InputSequences.cs` は `Enumerable` の代替のみで数値計算はしない)。
- 方法: size proxy と同じ `Dispatch.Input(pattern, size, 42)` を CoreCLR JIT (win-x64)、NativeAOT-LLVM、NetWasm 0.14.2 で実行し、出力配列を比較した。比較は FNV-1a ダイジェストと生バイト列 (`E\raw\vivo\<toolchain>\input-<p>-<n>.bin`) で、不一致なら最初に異なるインデックスを出す (`E\scripts\vivo-compare.mjs`)。
- 範囲: 入力パターン 71 種 (id 0〜70) × サイズ 13 種 (1, 2, 3, 7, 16, 100, 1,000, 1,023, 1,024, 1,025, 4,096, 10,000, 32,768。`PatternMax` で頭打ち) = 923 入力、seed 42。加えて 87 算法の並べ替え結果と記録フレーム (7 章)。
- **結果: 923 入力すべてで 3 ツールチェーンのバイト列が一致し、不一致 0 件**。87 算法の並べ替え結果・操作数・フレームも一致した (`E\results\vivo-compare.json`、`E\results\vivo-check-{native,naot,netwasm}.json`)。
- 限界: seed は 1 つだけ。出力は `(int)` へ切り捨てた後の整数なので、1 ulp 程度の libm の差は整数の境界をまたがない限り現れない。今回の範囲では境界をまたぐ例はなかった。

## 追加: 例外処理モデル `IlcLlvmExceptionHandlingModel=emulated` (legacy Wasm EH の回避)

- ビルド: `-p:IlcLlvmExceptionHandlingModel=emulated` を計測ハーネス (`E\out\naot-llvm-emueh`)、境界プローブ (`E\out\boundary-emueh`)、size proxy (`E\out\sp-naot-emueh`) に付けた。ILC に `--codegenopt:LlvmExceptionHandlingModel=emulated` が渡り、clang から `-fwasm-exceptions` が消え、`libWasmExceptionHandling.a` の代わりに `libEmulatedExceptionHandling.a` がリンクされた。

| バイナリ | 既定 (wasm EH) の `try` / `catch` / `rethrow` / `delegate` / `throw` / tag | emulated |
| --- | --- | --- |
| 計測ハーネス | 497 / 157 / 158 / 340 / 1 / 1 | すべて 0 |
| size proxy | 1,208 / 283 / 283 / 925 / 1 / 1 | すべて 0 |
| 境界プローブ | 316 / 119 / 120 / 197 / 1 / 1 | すべて 0 |

`try_table` / `throw_ref` / exnref 型は既定でも 0 (legacy 形式のみ)。emulated でも `target_features` セクションには `+exception-handling` が残るが、命令としての EH は 0 になった (`E\results\eh-audit.json`)。

| サイズ (Brotli) | 既定 | emulated | 差 |
| --- | --- | --- | --- |
| 計測ハーネス (.wasm + JS) | 318,931 B | 331,739 B | +12,808 B |
| size proxy | 528,910 B | 558,912 B | +30,002 B |

- 2a の挙動は変わらなかった: 漏れた 4 種の例外は同じ `Aborted()` と未処理例外のログ、`catch` 版は `-1`、無限再帰は `null function or function signature mismatch` の後に回復不能 (`E\results\boundary-emueh-a.json`)。
- Edge 157 headless で既定 (legacy EH) ビルドを main thread と ES module 版の両方で読み込み、`--enable-logging=stderr` でコンソールを取ったが、legacy EH に関する警告は出なかった (自分の `console.log` は取得できている。`E\results\eh-edge-console.json`)。Firefox は未測定 (このマシンにない)。
- 速度 (5 章と同じ実行、既定 NativeAOT-LLVM 比): 高速経路は n=1,000 random 1.01、n=10,000 random 0.98、sorted 0.99〜0.97 で差なし。観測系は遅くなった: StatisticsContext 1.14 (n=1,000) / 1.15 (n=10,000)、ISortContext 1.18 / 1.16、record 1.11 / 1.10、Record(-1) 1.10 / 1.11。

## 付録: 再現コマンドとファイル

Git Bash で `E` を作業領域とする。NativeAOT-LLVM のビルドは `source scripts/emenv.sh` 後に `T="-p:EmscriptenSdkToolsPath=$EMSCRIPTEN_TOOLS_WIN"`。`scripts/build.sh <id> <dir> [args]` は `NUGET_PACKAGES=$E/pkgs/$PKGSET` で `dotnet publish -c Release -o $E/out/<id> --artifacts-path $E/art/<id>` を実行し、ログを `$E/logs/build-<id>.log` に残す。

```bash
cd "$E"
git -C /d/github/guitarrapc/SortAlgorithms archive --format=tar f674c43 | tar -x -C "$E/repo"
source scripts/emenv.sh; T="-p:EmscriptenSdkToolsPath=$EMSCRIPTEN_TOOLS_WIN"
# 1. analysis build + disassembly
PKGSET=naot-llvm bash scripts/build.sh naot-llvm naot-llvm -r browser-wasm "$T" -p:IlcGenerateMapFile=true
"$EMSDK_TOOLS/bin/llvm-objdump.exe" -d -r art/naot-llvm/obj/naot-llvm/release_browser-wasm/native/audit.N.o > raw/naot-llvm/dis/audit.N.dis.txt
node scripts/objdis.mjs raw/naot-llvm/dis raw/naot-llvm/funcs.json
"$EMSDK_TOOLS/emscripten/emcc.bat" @raw/naot-llvm/named/link-named.rsp      # link.rsp minus -Wl,--strip-all, plus --profiling-funcs
node scripts/wasmsec.mjs out/naot-llvm/audit.wasm raw/naot-llvm/named/audit.wasm
"$EMSDK_TOOLS/bin/wasm-dis.exe" raw/naot-llvm/named/audit.wasm -o raw/naot-llvm/named/audit.wat
node scripts/watcalls.mjs raw/naot-llvm/named/audit.wat "<regex>" out.json
# benchmark builds (scripts/build-eval.sh)
PKGSET=naot-llvm bash scripts/build.sh naot-llvm-bench naot-llvm -r browser-wasm "$T" -p:AuditInitMem=default
PKGSET=naot-llvm bash scripts/build.sh naot-llvm-speed naot-llvm -r browser-wasm "$T" -p:AuditInitMem=default -p:AuditSpeedPref=true -p:IlcPgoOptimize=false
PKGSET=naot-llvm bash scripts/build.sh naot-llvm-emueh naot-llvm -r browser-wasm "$T" -p:AuditInitMem=default -p:IlcLlvmExceptionHandlingModel=emulated
PKGSET=netwasm-0.14.2 bash scripts/build.sh netwasm-o3 netwasm
PKGSET=netwasm-0.14.2 bash scripts/build.sh netwasm-o3-ignore netwasm -p:NetWasmManagedInliningHints=IgnoreAggressive
PKGSET=mono bash scripts/build.sh mono-aot mono -p:RunAOTCompilation=true -p:AuditInitMem=default
PKGSET=native-jit bash scripts/build.sh native-jit native
# 2. boundary probes
PKGSET=naot-llvm bash scripts/build.sh boundary boundary -r browser-wasm "$T"
PKGSET=naot-llvm bash scripts/build.sh boundary-es6 boundary -r browser-wasm "$T" -p:AuditEs6=true
PKGSET=naot-llvm bash scripts/build.sh jsapi jsapi -r browser-wasm "$T"
PKGSET=naot-llvm bash scripts/build.sh jsapi-spanexport jsapi -r browser-wasm "$T" -p:ProbeSpanExport=true
node scripts/boundary.mjs out/boundary results/boundary.json          # sections a,b,c
node scripts/bnd-own.mjs "$E/out/boundary" results/boundary-d.json   # section d, one process per trial
node scripts/worker-node.mjs "$E" results/worker-node.json
node scripts/edge-worker.mjs results/worker-edge.json
node scripts/jsapi-probe.mjs out/jsapi results/jsapi.json
node scripts/jsapi-spanexport.mjs out/jsapi-spanexport results/jsapi-spanexport.json
# 5. recording benchmark (no other work in parallel)
node scripts/orchestrate-vivo.mjs vivo,mem --rounds 3
node scripts/analyze-vivo.mjs "$E"
# 6. Speed workaround
PKGSET=naot-llvm bash scripts/build.sh naot-llvm-speed-repro naot-llvm -r browser-wasm "$T" -p:AuditSpeedPref=true            # crash
PKGSET=naot-llvm bash scripts/build.sh naot-llvm-pgo-only naot-llvm -r browser-wasm "$T" -p:IlcPgoOptimize=true               # crash
PKGSET=naot-llvm bash scripts/build.sh naot-llvm-speed-nopgo naot-llvm -r browser-wasm "$T" -p:AuditSpeedPref=true -p:IlcPgoOptimize=false  # OK
# 7. size proxy
PKGSET=naot-llvm bash scripts/build.sh sp-naot ../sizeproxy/naot -r browser-wasm "$T"            # + -p:AuditSpeedPref=true, -p:OptimizationPreference=Size, -p:EmccFlags=-O3 / -Oz
PKGSET=netwasm-0.14.2 bash scripts/build.sh sp-netwasm ../sizeproxy/netwasm                       # + -p:NetWasmManagedInliningHints=Auto
node scripts/sizes.mjs results/size-proxy.json sp-netwasm=out/sp-netwasm/browser sp-naot=out/sp-naot ...
# 7/8. all-algorithm and input checks
node scripts/vivo-check.mjs naot out/sp-naot-node results/vivo-check-naot.json raw/vivo/naot
node scripts/vivo-check.mjs netwasm out/sp-netwasm-node/browser results/vivo-check-netwasm.json raw/vivo/netwasm
out/vivo-native/VivoNative.exe raw/vivo/native > results/vivo-check-native.json
node scripts/vivo-compare.mjs "$E" results/vivo-compare.json
```

主なファイル:
- ハーネス: `E\shared\Kernel.cs` (ctx 3/4 = SortVivo の記録経路)、`E\shared\vivo\*.cs` (Sortvivo2 からのコピー)、`E\tc\*\*.csproj`
- 境界プローブ: `E\tc\boundary\Boundary.cs`、`E\tc\jsapi\JsProbe.cs`
- size proxy: `E\sizeproxy\engine-copy\` (Sortvivo2 からのコピー)、`E\sizeproxy\naot\EngineExports.cs`、`E\sizeproxy\netwasm\SortKernel.csproj`、`E\sizeproxy\native\Program.cs`
- 結果: `E\results\*.json[l]`、`E\results\vivo-report.md`、生の逆アセンブル `E\raw\naot-llvm\`
