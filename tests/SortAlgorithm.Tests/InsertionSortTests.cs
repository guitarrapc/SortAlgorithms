using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class InsertionSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => InsertionSort.Sort(span, context);

    // O(n^2) algorithm: keep data-driven inputs small.
    protected override int MaxOrderTestSize => 512;

    // Sorted input needs no shifts: every element stays in place, and insertion sort never swaps.
    protected override CountExpectation SortedInputWrites => CountExpectation.Zero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        InsertionSort.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        InsertionSort.Sort(array.AsSpan(), 0, array.Length, stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortSingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9 };

        // Sort a single element range [2, 3)
        InsertionSort.Sort(array.AsSpan(), 2, 3, stats);

        // Array should be unchanged (single element is already sorted)
        await Assert.That(array).IsEquivalentTo([5, 3, 8, 1, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortBeginningTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 9, 7, 5, 3, 1, 2, 4, 6, 8 };

        // Sort only the first 5 elements [0, 5)
        InsertionSort.Sort(array.AsSpan(), 0, 5, stats);

        // Expected: first 5 sorted, last 4 unchanged
        await Assert.That(array).IsEquivalentTo([1, 3, 5, 7, 9, 2, 4, 6, 8], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortEndTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 3, 5, 7, 9, 8, 6, 4, 2 };

        // Sort only the last 4 elements [5, 9)
        InsertionSort.Sort(array.AsSpan(), 5, 9, stats);

        // Expected: first 5 unchanged, last 4 sorted
        await Assert.That(array).IsEquivalentTo([1, 3, 5, 7, 9, 2, 4, 6, 8], CollectionOrdering.Matching);
    }

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        InsertionSort.Sort(sorted.AsSpan(), stats);

        // Insertion Sort on sorted data: best case O(n)
        // - For each position i (from 1 to n-1), we compare once with the previous element
        // - Since the current element is >= the previous element, no shifting occurs
        // - Total comparisons: n-1
        // - Total writes: 0 (already sorted)
        var expectedCompares = (ulong)(n - 1);
        var expectedWrites = 0UL;

        // Optimized implementation: For each position, Read(i) for tmp + Read(j) once for comparison = 2 reads
        var expectedIndexReads = (ulong)(2 * (n - 1));

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedIndexReads);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesReversedTest(int n)
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        InsertionSort.Sort(reversed.AsSpan(), stats);

        // Insertion Sort on reversed data: worst case O(n^2)
        // - Position 1: 1 comparison, 1 shift
        // - Position 2: 2 comparisons, 2 shifts
        // - ...
        // - Position n-1: (n-1) comparisons, (n-1) shifts
        // - Total comparisons: 1 + 2 + ... + (n-1) = n(n-1)/2
        // - Total shifts: same as comparisons = n(n-1)/2
        // - Each shift writes 1 element, plus final write for tmp = shift + 1 write per position
        // - Total writes: For each position i (1 to n-1):
        //   - i shifts (each shift is 1 write: s.Write(j+1, s.Read(j)))
        //   - 1 final write for tmp
        //   - Total: sum from i=1 to n-1 of (i+1) = sum(i) + (n-1) = n(n-1)/2 + (n-1) = (n-1)(n+2)/2
        var expectedCompares = (ulong)(n * (n - 1) / 2);
        var expectedWrites = (ulong)((n - 1) * (n + 2) / 2);

        // Optimized implementation: Read(j) once per loop iteration, then use the value for both comparison and write
        // Total reads = n(n-1)/2 (for comparisons) + (n-1) (for tmp reads) = (n-1)(n+2)/2
        var expectedIndexReads = (ulong)((n - 1) * (n + 2) / 2);

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedIndexReads);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL); // Insertion sort uses shifts, not swaps
    }

    [Test]
    [Arguments(10, 42)]
    [Arguments(10, 1234)]
    [Arguments(20, 42)]
    [Arguments(20, 1234)]
    [Arguments(50, 42)]
    [Arguments(50, 1234)]
    [Arguments(100, 42)]
    [Arguments(100, 1234)]
    public async Task TheoreticalValuesRandomTest(int n, int seed)
    {
        var stats = new StatisticsContext();
        var random = TestHelpers.ShuffledRange(n, seed);
        InsertionSort.Sort(random.AsSpan(), stats);

        // Insertion Sort on random data: average case O(n^2)
        // - Average comparisons: approximately n^2/4
        // - Comparisons range from best case (n-1) to worst case (n(n-1)/2)
        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(n * (n - 1) / 2);

        // Writes vary based on how many elements need to be shifted
        var minWrites = 0UL; // Best case (already sorted by chance)
        var maxWrites = (ulong)((n - 1) * (n + 2) / 2); // Worst case (reversed)

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL); // Insertion sort uses shifts, not swaps
    }

    // ------------------------------------------------------------------
    // Internal cores. Only the public Sort entry point is exercised above;
    // SortCore(start), UnguardedSortCore and SortIncomplete reach the caller
    // solely through IntroSort / PowerSort / PDQSort, so their own branches
    // have no direct coverage otherwise.
    // ------------------------------------------------------------------

    // SortSpan is a ref struct, so the calls live in non-async helpers.
    private static void RunSortCore<TContext>(int[] array, int first, int last, int start, TContext context)
        where TContext : ISortContext
    {
        var s = new SortSpan<int, ComparableComparer<int>, TContext>(array.AsSpan(), context, new ComparableComparer<int>(), 0);
        InsertionSort.SortCore(s, first, last, start);
    }

    private static void RunUnguardedSortCore<TContext>(int[] array, int first, int last, TContext context)
        where TContext : ISortContext
    {
        var s = new SortSpan<int, ComparableComparer<int>, TContext>(array.AsSpan(), context, new ComparableComparer<int>(), 0);
        InsertionSort.UnguardedSortCore(s, first, last);
    }

    private static bool RunSortIncomplete<TContext>(int[] array, int first, int last, bool leftmost, TContext context)
        where TContext : ISortContext
    {
        var s = new SortSpan<int, ComparableComparer<int>, TContext>(array.AsSpan(), context, new ComparableComparer<int>(), 0);
        return InsertionSort.SortIncomplete(s, first, last, leftmost);
    }

    /// <summary>0..n-1 の全順列。小さい n の網羅列挙にのみ使う。</summary>
    private static IEnumerable<int[]> Permutations(int n)
    {
        var items = Enumerable.Range(0, n).ToArray();
        return Permute(items, 0);

        static IEnumerable<int[]> Permute(int[] a, int k)
        {
            if (k == a.Length)
            {
                yield return a.ToArray();
                yield break;
            }
            for (var i = k; i < a.Length; i++)
            {
                (a[k], a[i]) = (a[i], a[k]);
                foreach (var p in Permute(a, k + 1)) yield return p;
                (a[k], a[i]) = (a[i], a[k]);
            }
        }
    }

    /// <summary>先頭 <paramref name="pairs"/> 組の隣接ペアを入れ替えた 0..n-1。</summary>
    /// <remarks>
    /// 入れ替えたペアの右側要素だけが「移動を要する要素」になり、挿入後は後続に影響しないので、
    /// SortIncomplete が検出する挿入回数がちょうど <paramref name="pairs"/> になる。
    /// </remarks>
    private static int[] WithSwappedPairs(int n, int pairs)
    {
        var a = Enumerable.Range(0, n).ToArray();
        for (var t = 0; t < pairs; t++)
        {
            (a[2 * t], a[2 * t + 1]) = (a[2 * t + 1], a[2 * t]);
        }
        return a;
    }

    // ---- SortCore(first, last, start): presorted prefix ----

    /// <summary>
    /// PowerSort と TimSort は自然ランを見つけたあと [first, start) がソート済みであることを根拠に
    /// start から挿入を始めさせる。前置ランの長さを変えても結果が変わらないことを確かめる。
    /// </summary>
    [Test]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(3)]
    [Arguments(7)]
    [Arguments(11)]
    public async Task SortCorePresortedPrefixTest(int prefixLength)
    {
        // [0, prefixLength) はソート済み、残りは降順。
        var array = Enumerable.Range(0, prefixLength)
            .Concat(Enumerable.Range(prefixLength, 12 - prefixLength).Reverse())
            .ToArray();
        var expected = array.OrderBy(x => x).ToArray();

        RunSortCore(array, 0, array.Length, prefixLength, new StatisticsContext());

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    /// <summary>
    /// start == first は「ソート済み前置が 1 要素だけ」を意味し、start++ で先頭要素をスキップする。
    /// これは 3 引数版と同一の動作でなければならない。
    /// </summary>
    [Test]
    public async Task SortCoreStartEqualsFirstMatchesFullSortTest()
    {
        var viaStart = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };
        var viaPublic = viaStart.ToArray();

        RunSortCore(viaStart, 0, viaStart.Length, 0, new StatisticsContext());
        InsertionSort.Sort(viaPublic.AsSpan(), NullContext.Default);

        await Assert.That(viaStart).IsEquivalentTo(viaPublic, CollectionOrdering.Matching);
        await Assert.That(viaStart).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9], CollectionOrdering.Matching);
    }

    /// <summary>start が範囲外（last 以上）なら 1 要素も挿入されない。</summary>
    [Test]
    public async Task SortCoreStartAtOrPastLastLeavesRangeUntouchedTest()
    {
        var array = new[] { 5, 3, 8, 1, 9 };

        RunSortCore(array, 0, array.Length, array.Length, new StatisticsContext());

        await Assert.That(array).IsEquivalentTo([5, 3, 8, 1, 9], CollectionOrdering.Matching);
    }

    /// <summary>部分範囲でも [first, last) の外側には触れない。</summary>
    [Test]
    public async Task SortCoreSubrangeLeavesOutsideUntouchedTest()
    {
        var array = new[] { 9, 9, 5, 3, 1, 4, 2, 9, 9 };

        RunSortCore(array, 2, 7, 2, new StatisticsContext());

        await Assert.That(array).IsEquivalentTo([9, 9, 1, 2, 3, 4, 5, 9, 9], CollectionOrdering.Matching);
    }

    // ---- UnguardedSortCore: sentinel at first-1 ----

    /// <summary>
    /// 事前条件は first > 0 かつ s[first-1] &lt;= [first, last) の全要素。
    /// 内側ループの (j >= first) を落としても番兵で停止し、番兵自身は動かない。
    /// </summary>
    [Test]
    [Arguments(2)]
    [Arguments(3)]
    [Arguments(8)]
    [Arguments(33)]
    public async Task UnguardedSortCoreSortsWithSentinelTest(int length)
    {
        // index 0 が番兵（範囲内の最小値未満）。
        var array = new int[length + 1];
        array[0] = int.MinValue;
        var payload = Enumerable.Range(0, length).Reverse().ToArray();
        payload.CopyTo(array, 1);

        RunUnguardedSortCore(array, 1, array.Length, new StatisticsContext());

        await Assert.That(array[0]).IsEqualTo(int.MinValue).Because("番兵は移動してはならない");
        await Assert.That(array.Skip(1).ToArray())
            .IsEquivalentTo(Enumerable.Range(0, length).ToArray(), CollectionOrdering.Matching);
    }

    /// <summary>番兵と等しい要素があっても（strict &gt; で停止するため）境界を越えない。</summary>
    [Test]
    public async Task UnguardedSortCoreStopsAtEqualSentinelTest()
    {
        // 番兵と同じ値 0 が範囲内に複数ある。
        var array = new[] { 0, 3, 0, 2, 0, 1 };

        RunUnguardedSortCore(array, 1, array.Length, new StatisticsContext());

        await Assert.That(array).IsEquivalentTo([0, 0, 0, 1, 2, 3], CollectionOrdering.Matching);
    }

    /// <summary>ソート済み入力では 1 要素も書き込まない（guarded 版と同じ最適化）。</summary>
    [Test]
    public async Task UnguardedSortCoreSortedInputWritesNothingTest()
    {
        var array = Enumerable.Range(0, 32).ToArray();
        var stats = new StatisticsContext();

        RunUnguardedSortCore(array, 1, array.Length, stats);

        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

    /// <summary>
    /// guarded 版と unguarded 版のオペレーション数の関係を固定する。
    /// 書き込みは一致する。逆順入力では全要素が first まで移動するので、unguarded 版は
    /// 範囲外に出る代わりに番兵と比較して停止し、要素あたり read と compare が 1 回ずつ多い。
    /// </summary>
    [Test]
    [Arguments(8)]
    [Arguments(16)]
    [Arguments(32)]
    public async Task UnguardedSortCoreCostsOneExtraCompareForElementsReachingFirstTest(int length)
    {
        var payload = Enumerable.Range(0, length).Reverse().ToArray();

        var guardedArray = new int[length + 1];
        guardedArray[0] = int.MinValue;
        payload.CopyTo(guardedArray, 1);
        var guarded = new StatisticsContext();
        RunSortCore(guardedArray, 1, guardedArray.Length, 1, guarded);

        var unguardedArray = new int[length + 1];
        unguardedArray[0] = int.MinValue;
        payload.CopyTo(unguardedArray, 1);
        var unguarded = new StatisticsContext();
        RunUnguardedSortCore(unguardedArray, 1, unguardedArray.Length, unguarded);

        await Assert.That(unguardedArray).IsEquivalentTo(guardedArray, CollectionOrdering.Matching);

        var extra = (ulong)(length - 1); // first まで到達する要素の数
        await Assert.That(unguarded.IndexWriteCount).IsEqualTo(guarded.IndexWriteCount);
        await Assert.That(unguarded.CompareCount).IsEqualTo(guarded.CompareCount + extra);
        await Assert.That(unguarded.IndexReadCount).IsEqualTo(guarded.IndexReadCount + extra);
    }

    // ---- SortIncomplete: switch arms 0..5, the limit, and leftmost true/false ----

    /// <summary>length 0 と 1 は常に true で、配列を変えない。</summary>
    [Test]
    [Arguments(0, true)]
    [Arguments(0, false)]
    [Arguments(1, true)]
    [Arguments(1, false)]
    public async Task SortIncompleteTrivialLengthsTest(int length, bool leftmost)
    {
        var array = new[] { int.MinValue, 7, 7, 7 };
        var before = array.ToArray();

        var sorted = RunSortIncomplete(array, 1, 1 + length, leftmost, new StatisticsContext());

        await Assert.That(sorted).IsTrue();
        await Assert.That(array).IsEquivalentTo(before, CollectionOrdering.Matching);
    }

    /// <summary>length 2 は専用の 1 比較分岐。両方の同値クラス（交換要/不要）を通す。</summary>
    [Test]
    [Arguments(1, 2, 1, 2)]
    [Arguments(2, 1, 1, 2)]
    [Arguments(1, 1, 1, 1)]
    public async Task SortIncompleteLengthTwoTest(int a, int b, int expectedFirst, int expectedSecond)
    {
        foreach (var leftmost in new[] { true, false })
        {
            var array = new[] { int.MinValue, a, b };

            var sorted = RunSortIncomplete(array, 1, 3, leftmost, new StatisticsContext());

            await Assert.That(sorted).IsTrue();
            await Assert.That(array[1]).IsEqualTo(expectedFirst);
            await Assert.That(array[2]).IsEqualTo(expectedSecond);
        }
    }

    /// <summary>
    /// length 3/4/5 はソーティングネットワーク分岐。全順列を網羅し、常に true かつ整列済みで返ることを確かめる。
    /// </summary>
    [Test]
    [Arguments(3)]
    [Arguments(4)]
    [Arguments(5)]
    public async Task SortIncompleteSortingNetworkLengthsTest(int length)
    {
        var expected = Enumerable.Range(0, length).ToArray();

        foreach (var permutation in Permutations(length))
        {
            foreach (var leftmost in new[] { true, false })
            {
                var array = new int[length + 1];
                array[0] = int.MinValue;
                permutation.CopyTo(array, 1);

                var sorted = RunSortIncomplete(array, 1, array.Length, leftmost, new StatisticsContext());

                await Assert.That(sorted).IsTrue()
                    .Because($"length {length} は常に完了する: [{string.Join(",", permutation)}]");
                await Assert.That(array.Skip(1).ToArray()).IsEquivalentTo(expected, CollectionOrdering.Matching)
                    .Because($"入力 [{string.Join(",", permutation)}] (leftmost={leftmost})");
            }
        }
    }

    /// <summary>重複を含むソーティングネットワーク分岐。</summary>
    [Test]
    public async Task SortIncompleteSortingNetworkWithDuplicatesTest()
    {
        int[][] inputs =
        [
            [2, 1, 2],
            [1, 1, 1],
            [2, 2, 1, 1],
            [1, 2, 1, 2],
            [3, 1, 2, 1, 3],
            [1, 1, 1, 1, 1],
        ];

        foreach (var input in inputs)
        {
            var array = new int[input.Length + 1];
            array[0] = int.MinValue;
            input.CopyTo(array, 1);
            var expected = input.OrderBy(x => x).ToArray();

            var sorted = RunSortIncomplete(array, 1, array.Length, leftmost: true, new StatisticsContext());

            await Assert.That(sorted).IsTrue();
            await Assert.That(array.Skip(1).ToArray()).IsEquivalentTo(expected, CollectionOrdering.Matching)
                .Because($"入力 [{string.Join(",", input)}]");
        }
    }

    /// <summary>
    /// 上限未満（7 要素が移動を要する）なら最後まで走り切って true を返し、範囲は完全に整列している。
    /// </summary>
    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task SortIncompleteUnderLimitCompletesTest(bool leftmost)
    {
        var array = new int[21];
        array[0] = int.MinValue;
        WithSwappedPairs(20, pairs: 7).CopyTo(array, 1);

        var sorted = RunSortIncomplete(array, 1, array.Length, leftmost, new StatisticsContext());

        await Assert.That(sorted).IsTrue();
        await Assert.That(array.Skip(1).ToArray())
            .IsEquivalentTo(Enumerable.Range(0, 20).ToArray(), CollectionOrdering.Matching);
    }

    /// <summary>
    /// 8 要素目が移動を要した時点で false を返す。整列は完了しないが、内容は入力の順列のまま。
    /// </summary>
    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task SortIncompleteAtLimitGivesUpTest(bool leftmost)
    {
        var array = new int[21];
        array[0] = int.MinValue;
        WithSwappedPairs(20, pairs: 8).CopyTo(array, 1);

        var sorted = RunSortIncomplete(array, 1, array.Length, leftmost, new StatisticsContext());

        await Assert.That(sorted).IsFalse();
        // 途中で諦めても要素を落としたり複製したりしない。
        await Assert.That(array.Skip(1).OrderBy(x => x).ToArray())
            .IsEquivalentTo(Enumerable.Range(0, 20).ToArray(), CollectionOrdering.Matching);
    }

    /// <summary>逆順は 6 要素以上なら必ず上限に達する。</summary>
    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task SortIncompleteReversedGivesUpTest(bool leftmost)
    {
        var array = new int[21];
        array[0] = int.MinValue;
        Enumerable.Range(0, 20).Reverse().ToArray().CopyTo(array, 1);

        var sorted = RunSortIncomplete(array, 1, array.Length, leftmost, new StatisticsContext());

        await Assert.That(sorted).IsFalse();
        await Assert.That(array[0]).IsEqualTo(int.MinValue).Because("番兵は移動してはならない");
    }

    /// <summary>既にソート済みなら 1 度も挿入せず true。</summary>
    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task SortIncompleteSortedInputCompletesWithoutWritesTest(bool leftmost)
    {
        var array = Enumerable.Range(0, 21).ToArray();
        var stats = new StatisticsContext();

        var sorted = RunSortIncomplete(array, 1, array.Length, leftmost, stats);

        await Assert.That(sorted).IsTrue();
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }
}
