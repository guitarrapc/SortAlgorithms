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
    [Arguments(10, 67)]
    [Arguments(20, 162)]
    [Arguments(50, 480)]
    [Arguments(100, 1077)]
    public async Task TheoreticalComparisonCountTest(int n, int expectedComparisons)
    {
        // The exact comparison count for sorted data, which is deterministic. Every range down to two elements is partitioned: no other
        // algorithm takes over. On sorted distinct keys each median of three costs three comparisons, the partition compares each element
        // of its range once, and a range checked for equal keys compares its new pivot with the previous one once. For n = 10 the ranges
        // of more than one element hold 10, 4, 2, 2, 6, 3, 2, 3 and 2 elements, in sort order, and cost 3 + 10, 3 + 4, 3 + 2, 3 + 1 + 2,
        // 3 + 1 + 6, 3 + 1 + 3, 3 + 1 + 2, 3 + 1 + 3 and 3 + 1 + 2 comparisons: 67. The larger sizes are pinned from the same implementation.
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        DestswapStableQuickSort.Sort(sorted.AsSpan(), stats);

        await Assert.That(stats.CompareCount).IsEqualTo((ulong)expectedComparisons);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);

        // Every partition reads each element of its range once and each median of three reads its three samples; one-element ranges
        // copied into place add reads on top. Comparisons are no bound on reads: the pivot is held, and so is the previous one it is
        // checked against.
        ulong partitioned = 0;
        DestswapStableQuickSort.Sort(Enumerable.Range(0, n).ToArray().AsSpan(), new VisualizationContext(onPhase: (phase, first, last, _) =>
        {
            if (phase == SortPhase.QuickSortPartition) partitioned += (ulong)(last - first + 1) + 3;
        }));
        await Assert.That(stats.IndexReadCount).IsGreaterThanOrEqualTo(partitioned);
    }

#endif
}
