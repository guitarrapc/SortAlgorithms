using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class DestswapStableQuickSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => DestswapStableQuickSort.Sort(span, context);

    // No write/swap knob overrides: the old statistics test only asserted reads and compares.

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        DestswapStableQuickSort.Sort(array.AsSpan(), 2, 6, stats);

        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        DestswapStableQuickSort.Sort(array.AsSpan(), 0, array.Length, stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9], CollectionOrdering.Matching);
    }

#if DEBUG

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        DestswapStableQuickSort.Sort(sorted.AsSpan(), stats);

        // DestswapStableQuickSort on sorted data:
        // - For n <= SMALL_SORT (16): Uses BottomUp merge sort fallback
        //   Sorted data: O(n) comparisons (efficient merge on already-sorted runs)
        // - For n > SMALL_SORT: Uses DestSwap 2-way stable quicksort
        //   Writes directly to destination; no copy-back per recursion level
        //   Sorted data with adaptive pivot selection: O(n log n) comparisons
        // - No Swap operations: DestSwap uses Read/Write/CopyTo directly
        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(n * n);

        // DestswapStableQuickSort does not use Swap at all
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);

        var minIndexReads = (ulong)n;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        var minIndexWrites = (ulong)n;
        await Assert.That(stats.IndexWriteCount >= minIndexWrites).IsTrue().Because($"IndexWriteCount ({stats.IndexWriteCount}) should be >= {minIndexWrites}");
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
        DestswapStableQuickSort.Sort(reversed.AsSpan(), stats);

        // DestswapStableQuickSort on reversed data:
        // - For n <= SMALL_SORT (16): BottomUp fallback; O(n log n) comparisons
        // - For n > SMALL_SORT: DestSwap QuickSort; adaptive strategy handles reversed patterns
        // - No Swap operations
        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(n * n);

        await Assert.That(stats.SwapCount).IsEqualTo(0UL);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);

        var minIndexReads = (ulong)n;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        var minIndexWrites = (ulong)n;
        await Assert.That(stats.IndexWriteCount >= minIndexWrites).IsTrue().Because($"IndexWriteCount ({stats.IndexWriteCount}) should be >= {minIndexWrites}");
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
        DestswapStableQuickSort.Sort(random.AsSpan(), stats);

        // DestswapStableQuickSort on random data:
        // - Average case O(n log n) with adaptive pivot selection
        // - No Swap operations: uses DestSwap (Read/Write/CopyTo)
        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(n * n);

        await Assert.That(stats.SwapCount).IsEqualTo(0UL);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);

        var minIndexReads = (ulong)n;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        var minIndexWrites = (ulong)n;
        await Assert.That(stats.IndexWriteCount >= minIndexWrites).IsTrue().Because($"IndexWriteCount ({stats.IndexWriteCount}) should be >= {minIndexWrites}");
    }

    [Test]
    [Arguments(10, 9)]    // n <= SMALL_SORT=16: BottomUp fallback, sorted data → n-1=9 comparisons
    [Arguments(20, 41)]   // n > SMALL_SORT: DestSwap QuickSort
    [Arguments(50, 178)]  // n > SMALL_SORT: DestSwap QuickSort
    [Arguments(100, 469)] // n > SMALL_SORT: DestSwap QuickSort
    public async Task TheoreticalComparisonCountTest(int n, int expectedComparisons)
    {
        // Test the exact comparison count for sorted data.
        // DestswapStableQuickSort on sorted data is fully deterministic:
        // - n <= SMALL_SORT (16): BottomUp merge sort → efficient merge on already-sorted runs
        // - n > SMALL_SORT: DestSwap 2-way QuickSort with adaptive pivot selection
        // No Swap operations in any case.
        //
        // Empirical values for sorted data:
        // n=10:  Compare=9   (BottomUp fallback: n-1)
        // n=20:  Compare=41
        // n=50:  Compare=178
        // n=100: Compare=469
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        DestswapStableQuickSort.Sort(sorted.AsSpan(), stats);

        await Assert.That(stats.CompareCount).IsEqualTo((ulong)expectedComparisons);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexReadCount >= stats.CompareCount).IsTrue()
            .Because($"IndexReadCount ({stats.IndexReadCount}) should be >= CompareCount ({stats.CompareCount})");
    }

#endif
}
