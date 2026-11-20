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
    // SortCore(start) and UnguardedSortCore reach the caller solely through
    // PowerSort / PDQSort, so their own branches have no direct coverage
    // otherwise.
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
}
