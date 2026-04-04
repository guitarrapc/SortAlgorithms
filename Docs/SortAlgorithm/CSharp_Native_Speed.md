# C# Low-Level Performance Optimization Techniques

本プロジェクト (SortAlgorithm) で使用中・検討可能な C# ネイティブ寄り高速化手法の一覧。

---

## 現状使用済みの手法

### 1. `Span<T>` + `ref struct` ラッパー

- **使用箇所**: `SortSpan<T, TComparer, TContext>` (全アルゴリズムの基盤)
- **効果**: ヒープ割り当てゼロで配列スライスを扱う。`ref struct` により JIT がスタック上保証を利用して最適化可能。
- **状態**: ✅ 全面採用済み

### 2. struct comparer による仮想呼び出し除去 (Devirtualization)

- **使用箇所**: `TComparer : IComparer<T>` (struct) を全アルゴリズムのジェネリック引数に使用
- **効果**: JIT がジェネリック特殊化時に `Compare` メソッドを直接呼び出し → インライン化。`IComparer<T>` のインターフェースディスパッチを完全に除去。
- **状態**: ✅ 全面採用済み

### 3. `typeof(T) == typeof(int)` 型特殊化パス

- **使用箇所**: `SortSpan.IsLessThan`, `IsLessOrEqual`, `IsGreaterThan`, `IsGreaterOrEqual` および各 `*At` バリアント
- **効果**: JIT が `typeof(T)` を定数畳み込み → 該当しない型の分岐を Dead Code Elimination。プリミティブ型では `IComparable.CompareTo` → `int` → `< 0` チェーンを回避し、直接 CPU 比較命令 1 つに。
- **状態**: ✅ 全 13 プリミティブ型 (byte, sbyte, short, ushort, int, uint, long, ulong, nint, nuint, float, double, Half)

### 4. `System.Runtime.CompilerServices.Unsafe.As<TFrom, TTo>`

- **使用箇所**: `SortSpan` の型特殊化パス、`FloatingPointUtils.IsNaN`
- **効果**: ボクシングなしで `T` をプリミティブ型として再解釈。JIT は `Unsafe.As` を no-op (ゼロコスト) にコンパイル。
- **状態**: ✅ 採用済み

### 5. `IComparableComparer` マーカーインターフェース

- **使用箇所**: `ComparableComparer<T>` が実装、`SortSpan` の `IsLessThan` 系で条件分岐
- **効果**: struct comparer の `is IComparableComparer` チェックは JIT が定数畳み込み → カスタム comparer のとき型特殊化パスをバイパスしない安全性を保証しつつ、デフォルト comparer 時はプリミティブ直接比較へ。
- **状態**: ✅ 採用済み

### 6. `typeof(TContext) != typeof(NullContext)` Dead Code Elimination

- **使用箇所**: `SortSpan` の `Read`, `Write`, `Compare`, `Swap`, `CopyTo` 全メソッド
- **効果**: `NullContext` 使用時、統計トラッキング・可視化コードを JIT が完全除去。ベンチマーク時は直接配列操作と同等のコードに。
- **状態**: ✅ 全面採用済み

### 7. `[MethodImpl(MethodImplOptions.AggressiveInlining)]`

- **使用箇所**: `SortSpan` 全メソッド、`FloatingPointUtils`、各ユーティリティ
- **効果**: ホットパスの関数呼び出しオーバーヘッド除去。呼び出し元での定数畳み込みやレジスタ割り当て最適化を促進。
- **状態**: ✅ 採用済み

### 8. `stackalloc` によるスタック割り当て

- **使用箇所**: `AmericanFlagSort`, `BucketSort`, `CountingSort`, `RadixLSD10Sort` 等の基数・バケットカウント
- **効果**: 小さな固定サイズバッファを GC ヒープを経由せずスタックに確保。
- **状態**: ✅ 小バッファで採用済み (閾値超過時は `ArrayPool` にフォールバック)

### 9. `ArrayPool<T>.Shared` プーリング

- **使用箇所**: `TimSort`, `PowerSort`, `Glidesort`, `SpinSort`, `PatienceSort`, `DropMergeSort`, `RotateMergeSort`, tree sorts 等
- **効果**: 大きな作業バッファの再利用により GC 圧力を低減。
- **状態**: ✅ 大バッファで採用済み (`RuntimeHelpers.IsReferenceOrContainsReferences<T>()` によるクリア制御も実施)

### 10. `BitOperations` (ハードウェア命令マッピング)

- **使用箇所**: `PDQSort`, `IntroSort`, `StdSort`, `BlockQuickSort` (Log2), `PowerSort`, `Glidesort`, `SpreadSort` (LeadingZeroCount), `TournamentSort` (RoundUpToPowerOf2)
- **効果**: `BitOperations.Log2` → `lzcnt`/`bsr` 命令、`LeadingZeroCount` → `lzcnt` 命令に JIT がコンパイル。ソフトウェアループ不要。
- **状態**: ✅ 採用済み

---

## 未使用・検討可能な手法

### 11. `[SkipLocalsInit]` — ローカル変数のゼロ初期化スキップ

- **現状**: ✅ 採用済み (`[module: SkipLocalsInit]` を `globals.cs` に追加、全アセンブリ適用)
- **概要**: メソッドまたはアセンブリ単位で `localsinit` フラグを除去。ローカル変数の .NET 暗黙ゼロ埋めをスキップ。
- **適用内容**:
  - `globals.cs` に `[module: System.Runtime.CompilerServices.SkipLocalsInit]` を追加 (アセンブリ全体)
  - 事前に全 `stackalloc` サイトを監査し、ゼロ初期化が必要なサイトに明示 `Clear()` を追加:
    - `RadixMSD4Sort.cs`: `bucketCounts = stackalloc int[RadixSize + 1]` → `bucketCounts.Clear()` 追加
    - `RadixMSD10Sort.cs`: `counts = stackalloc int[RadixBase]` → `counts.Clear()` 追加
  - その他のサイトはすべて安全 — 既存の `.Clear()` / `.Fill()` 、接頭和での全書き込み、または push 前読み取りなしのスタックバッファ
- **期待効果**: `stackalloc` を使う Radix/Bucket/Distribution 系ソートのメソッドで改善。特に大きなバッファ (CountingSort の最大 1024 int = 4KB) で顕著。
- **教訓**: 適用前の全 `stackalloc` サイト監査が必須。`++` で加算するバケットカウント (RadixMSD4Sort / RadixMSD10Sort) が見落としやすい危険パターン。前向きループで全要素を書く prefix sum は安全。

### 12. `GC.AllocateUninitializedArray<T>` — ゼロ初期化スキップ配列

- **現状**: 未使用 (`new T[]` または `ArrayPool` を使用)
- **概要**: `new T[n]` の暗黙ゼロ初期化をスキップした配列を確保。非参照型 (int, byte 等) のみ効果。
- **適用候補**:
  - `ArrayPool` を使わないケースで一時配列を確保するとき
  - マージソートの一時バッファ確保 (書き込み前にゼロ初期化が不要なケース)
- **期待効果**: 大きな `int[]` バッファで数%改善。小さな配列では効果微小。
- **リスク**: 初期化前の読み取りは未定義値。参照型配列には使えない (GC の安全性)。
- **優先度**: 低 — `ArrayPool` と競合するケースが限定的

### 13. `MemoryMarshal` による低レベルメモリ操作

- **現状**: ✅ 採用済み
- **概要**: `Span<T>` の先頭要素への `ref T` を取得し、`Unsafe.Add` でポインタ算術的にアクセス。境界チェックをバイパス。
- **適用内容**:
  - `SortSpan<T, TComparer, TContext>` に `private readonly ref T _ref` フィールドを追加 (C# 11 ref フィールド)
  - コンストラクタで `_ref = ref MemoryMarshal.GetReference(span)` により先頭要素 ref を保存
  - `Read`, `Write`, `Compare(int,int)`, `Compare(int,T)`, `Compare(T,int)` の NullContext fast path を `Unsafe.Add(ref _ref, i)` に変更
  - `IsLessAt`, `IsLessOrEqualAt`, `IsGreaterAt`, `IsGreaterOrEqualAt` の IComparableComparer ブロック: `ref T ai = ref Unsafe.Add(ref _ref, i)` / `ref T aj` ローカル ref を導入、全 13 型特殊化で `ref _span[i]` → `ref ai` に変更
  - `Swap`: `(_span[i], _span[j]) = (...)` → `ref T si`/`sj` を介した tuple swap に変更
- **実測結果** (N=4096, Release, dotnet run -c Release):
  - Sequential scan: ~1% 改善 (JIT が元から range analysis で境界チェック除去)
  - Compare loop (i vs i+1): ~5% 改善
  - InsertionSort (不規則な後ろ向きスキャン): **~10% 改善** (1762µs → 1593µs)
- **教訓**: JIT は単純な前向きループでは既に境界チェックを除去する。後ろ向き不規則アクセス (InsertionSort の `while (j >= 0 ...)`) で最も効果が出る。

### 14. `ref T` / `Unsafe.Add` によるポインタ算術的アクセス

- **現状**: ✅ 採用済み (#13 の SortSpan 実装により全カバー)
- **概要**: `ref T start = ref MemoryMarshal.GetReference(span)` + `Unsafe.Add(ref start, i)` で境界チェックなしインデックスアクセス。
- **適用内容**:
  - 全アルゴリズムが `SortSpan` メソッド (`Read`, `Write`, `Compare`, `IsLessAt`, `Swap` 等) を介してスパンにアクセスする
  - `SortSpan` の `NullContext` fast path はすべて `Unsafe.Add(ref _ref, i)` を使用 (#13 で実装) → JIT インライン展開後にアルゴリズムコードで直接 `Unsafe.Add` を使うのと等価
  - **InsertionSort の内部ループ** (`s.Read(j)` / `s.Write(j+1, a)`) → インライン後に `Unsafe.Add(ref _ref, j)` / `Unsafe.Add(ref _ref, j+1)` となり境界チェックなし
  - **PDQSort の partition スキャン** (`s.Compare(first, pivot)` の `do { first++; } while (...)` ループ) → 同様にインライン後 `Unsafe.Add` 使用
  - **TimSort/PowerSort の merge** (temp buffer も `SortSpan` でラップ) → `t.Read(cursor1)` 等もすべて `Unsafe.Add(ref _ref, i)` に展開
- **教訓**: #13 で `SortSpan` に `_ref` + `Unsafe.Add` を導入した時点でアルゴリズム側の変更は不要になる。`AggressiveInlining` により呼び出しオーバーヘッドがなく、JIT は `Unsafe.Add(ref _ref, i)` をアルゴリズムコードに直接埋め込むのと同等のコードを生成する。`NullContext` パスの統計トラッキング除去 (#6) と組み合わさり、ベンチマーク時は生の配列アクセスと事実上同等の機械語になる。
- **実測結果**: #13 の実測と同一 (InsertionSort ~10% 改善等) — #13 と #14 は同一実装により達成

### 15. SIMD / Hardware Intrinsics (`Vector128`, `Vector256`, `Avx2`, `AdvSimd`)

- **現状**: 未使用
- **概要**: SIMD 命令で複数要素を並列処理。

#### 15a. 小要素数のソーティングネットワーク (SIMD Sorting Network)

- **適用候補**: 4, 8, 16 要素の `int`/`float` ソートを SIMD min/max でブランチレスに実行。
- **参考**: Intel の `x86-simd-sort`, `vxsort` — `Vector128.Min/Max` をソーティングネットワークのコンパレータに使用。
- **期待効果**: 小配列ソートが 2-4x 高速化。InsertionSort fallback の代替。
- **リスク**: 型ごとの実装が必要。`T` がジェネリックのため `Vector128<int>` 等への変換レイヤーが必要。

#### 15b. Partition の SIMD 化 (Vectorized Partition)

- **適用候補**: QuickSort/PDQSort の partition ステップで、ピボットとの比較を 8/16 要素同時実行。
- **参考**: `vqsort` (Google Highway), `ips4o` — `Vector256.CompareGreaterThan` + `MoveMask` + `Compress` パターン。
- **期待効果**: Partition が 2-3x 高速化。ただし分岐予測ミスが少ない nearly-sorted データでは効果減。
- **リスク**: AVX-512 の `Compress` がないと条件付き書き込みのコストが大きい。ARM/x86 両対応が必要。

#### 15c. Merge の SIMD 化 (Vectorized Merge)

- **適用候補**: 両ランがソート済みの merge ステップで、ブロック単位の比較・コピー。
- **期待効果**: merge-heavy アルゴリズム (TimSort, PowerSort) で改善。
- **リスク**: 条件分岐が多くベクタ化しにくい部分もある。

#### 15d. Radix Sort のヒストグラム作成

- **適用候補**: digit 抽出 + バケットカウントを SIMD gather/scatter で並列化。
- **期待効果**: RadixSort のカウントフェーズが高速化。
- **リスク**: gather/scatter は AVX-512 で高速だが、AVX2 では遅い。

- **優先度**: 低〜中 — 実装コスト大。`int`/`float` 特殊化との相性は良いが、ジェネリック `T` との統合が課題。

### 16. Branchless Conditional Move パターン

- **現状**: 一部の比較で暗黙的 (JIT が最適化)、明示的なブランチレス手法は未使用
- **概要**: 分岐予測ミスを回避するため、`cmov` 命令に展開される条件式を使う。
- **適用候補**:
  ```csharp
  // 分岐あり (分岐予測ミスペナルティ)
  if (a < b) { min = a; } else { min = b; }

  // ブランチレス — JIT が cmov に展開しやすい
  // .NET 10 の ConditionalSelect も活用可能
  min = a < b ? a : b;
  ```
  - InsertionSort の挿入位置探索
  - Partition のスキャンでの条件付きポインタ進行
  - MedianOfThree の実装
- **期待効果**: ランダムデータでの分岐予測ミスが多いソートで 5-15% 改善。
- **リスク**: JIT の cmov 生成は C# 側からの制御が限定的。`Unsafe` + 算術トリックが必要な場合あり。
- **優先度**: 中 — Partition のホットパスで最も効果的

### 17. `[MethodImpl(MethodImplOptions.NoInlining)]` — コールドパスの分離

- **現状**: 未使用
- **概要**: 例外スローやエラーハンドリングなどのコールドパスに `NoInlining` を付けて、ホットパスのインライン展開サイズを縮小。
- **適用候補**:
  - 境界チェック失敗時の throw ヘルパー (`ThrowHelper` パターン)
  - 稀にしか実行されない fallback パス
- **期待効果**: ホットパスのコード密度向上 → instruction cache 効率改善。
- **優先度**: 低 — 本プロジェクトではエラーパスが少ない

### 18. `Unsafe.CopyBlock` / `Unsafe.CopyBlockUnaligned` — バルクメモリコピー

- **現状**: 未使用 (`Span.CopyTo` を使用)
- **概要**: `memcpy` 相当の低レベルコピー。`Span.CopyTo` より薄いラッパーで、アライメント保証がない場合にも使える。
- **適用候補**:
  - `SortSpan.CopyTo` の `NullContext` パスで `Span.CopyTo` の代わりに使用
  - merge のブロックコピー
- **期待効果**: `Span.CopyTo` は内部で同等の処理をするため、実質的な差は微小。JIT がインライン化する場合のコードサイズが若干小さくなる程度。
- **優先度**: 低 — `Span.CopyTo` で十分

### 19. `CollectionsMarshal.AsSpan(List<T>)` — List 内部配列への直接アクセス

- **現状**: 未使用 (そもそも `List<T>` を使用していない)
- **概要**: `List<T>` の内部配列を `Span<T>` として取得。
- **適用**:  ソートアルゴリズムの入力が `List<T>` の場合の API 拡張に使える。
- **優先度**: 低 — 現状のアーキテクチャでは不要

### 20. `Unsafe.BitCast<TFrom, TTo>` (.NET 8+)

- **現状**: `Unsafe.As` を使用
- **概要**: .NET 8+ で追加された値型の再解釈キャスト。`Unsafe.As<T, int>(ref value)` の代替で、引数が `ref` でなく値渡し。
- **適用候補**:
  - `SortSpan.IsLessThan` 等での型特殊化パス
  - `FloatingPointUtils` での NaN チェック
- **期待効果**: コードの意図が明確になる。パフォーマンスは `Unsafe.As` と同等 (JIT は同じコードを生成)。
- **リスク**: ref を取る必要がなくなるため、一時変数のスタック確保が不要に。
- **優先度**: 低 — セマンティクス改善のみ、速度変化なし

### 21. `[InlineArray(N)]` (.NET 8+) — 固定サイズインラインバッファ

- **現状**: 未使用 (`stackalloc` で代替)
- **概要**: struct 内に固定サイズの配列を埋め込む。`stackalloc` と異なり struct のフィールドとして使える。
- **適用候補**:
  - ソーティングネットワーク (BitonicSort, OddEvenMergeSort) の固定サイズ中間バッファ
  - 小配列ソート (3-16 要素) の一時保存
  - BlockQuickSort のオフセットバッファ (`stackalloc int[BlockSize]` の代替)
- **期待効果**: `stackalloc` と同等。struct に埋め込めるため、再帰で渡す場合に便利。
- **優先度**: 低 — `stackalloc` で十分カバー

### 22. `Vector.IsHardwareAccelerated` ガード付き SIMD フォールバック

- **現状**: 未使用
- **概要**: `Vector<T>` (ポータブル SIMD) を使い、ハードウェア SIMD がない環境ではスカラーフォールバック。
- **適用候補**:
  - `Span<T>.Fill()` / `Span<T>.Clear()` の高速化 (ただし .NET ランタイムが既に最適化)
  - Radix sort のヒストグラムスキャン
  - ソート済み判定 (is-sorted check) を `Vector` で一括比較
- **期待効果**: 15 の SIMD 案よりポータブルで実装が簡潔。
- **優先度**: 低 — ランタイムの既存最適化と重複する部分が多い

---

## 優先度まとめ

| 優先度 | 手法 | 難易度 | 期待効果 |
|:---:|:---|:---:|:---:|
| **✅** | 13. `MemoryMarshal.GetReference` | 中 | SortSpan の全 NullContext fast path で境界チェック除去 ✅ |
| **✅** | 14. `ref T` + `Unsafe.Add` ポインタ算術 | 中 | #13 の SortSpan 実装により全アルゴリズムで達成 ✅ |
| **✅** | 11. `[SkipLocalsInit]` | 低 | stackalloc バッファのゼロ初期化回避 ✅ |
| **中** | 16. Branchless conditional move | 中 | Partition の分岐予測ミス回避 |
| **中** | 15a. SIMD Sorting Network (小要素) | 高 | 小配列ソート 2-4x 高速化 |
| **低** | 15b-d. SIMD Partition/Merge/Radix | 高 | アルゴリズム固有の大幅改善 |
| **低** | 12. `GC.AllocateUninitializedArray` | 低 | 大配列確保の初期化スキップ |
| **低** | 17. `NoInlining` コールドパス分離 | 低 | ICache 効率改善 |
| **低** | 18. `Unsafe.CopyBlock` | 低 | `Span.CopyTo` と差がほぼない |
| **低** | 20. `Unsafe.BitCast` | 低 | セマンティクスのみ |
| **低** | 21. `InlineArray` | 低 | `stackalloc` と同等 |
| **低** | 22. ポータブル SIMD | 中 | ランタイム既存最適化と重複 |
| N/A | 19. `CollectionsMarshal` | 低 | 現アーキテクチャでは不要 |

## 方針

- **実装済み**: #11 (`SkipLocalsInit`) は `globals.cs` 1行 + 安全でない2サイトへの明示 `Clear()` 追加で全体適用。#13 (`MemoryMarshal.GetReference`) と #14 (`ref` + `Unsafe.Add`) は同一の SortSpan 実装で達成。`NullContext` + `AggressiveInlining` + `Unsafe.Add(ref _ref, i)` の組み合わせにより、アルゴリズムコードの変更不要で全ホットループの境界チェックを除去。
- **低コスト**: #11 (`SkipLocalsInit`) はアセンブリレベルで 1 行追加するだけ。
- **高コスト高効果**: #15 (SIMD) は `int`/`float` 特殊化として `typeof(T) == typeof(int)` パターンの延長で導入可能だが、実装・テスト・保守コストが大きい。
