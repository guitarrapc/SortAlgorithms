using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class BidirectionalStableQuickSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => BidirectionalStableQuickSort.Sort(span, context);

    // Every partition level copies elements through the scratch buffer, even on sorted input.
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    // SwapCount not asserted: swaps only occur when reversing the greater section, which can be empty.

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        BidirectionalStableQuickSort.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        BidirectionalStableQuickSort.Sort(array.AsSpan(), 0, array.Length, stats);

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
        BidirectionalStableQuickSort.Sort(sorted.AsSpan(), stats);

        // BidirectionalStableQuickSort on sorted data:
        // - Median-of-3 pivot at quartile positions provides balanced partitions
        // - Single forward scan: less to scratch front, greater to scratch back (reversed), then reverse greater section
        // - Swaps are used to reverse the greater section in scratch buffer
        // - Uses Read/Write for copying to/from scratch buffer
        //
        // For sorted data: O(n log n) comparisons
        // Less elements cost 1 comparison, greater/equal cost 2 comparisons
        // Swaps ≈ n*log₂(n)/7 (only for reversing greater section)
        var minCompares = (ulong)n;
        var maxCompares = (ulong)(n * n);
        var minSwaps = 0UL;
        var maxSwaps = (ulong)(n * Math.Max(1, Math.Log(n, 2)));

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);

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
        BidirectionalStableQuickSort.Sort(reversed.AsSpan(), stats);

        // BidirectionalStableQuickSort on reversed data:
        // - Median-of-3 quartile pivot still provides balanced partitions
        // - More swaps than sorted data (larger greater sections need reversing)
        // - Similar O(n log n) overall complexity
        var minCompares = (ulong)n;
        var maxCompares = (ulong)(n * n) + 2;
        var minSwaps = 0UL;
        var maxSwaps = (ulong)(n * Math.Max(1, Math.Log(n, 2)));

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);

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
    public async Task TheoreticalValuesRandomTest(int n)
    {
        var stats = new StatisticsContext();
        var rng = new Random(42 + n);
        var random = Enumerable.Range(0, n).OrderBy(_ => rng.Next()).ToArray();
        BidirectionalStableQuickSort.Sort(random.AsSpan(), stats);

        // BidirectionalStableQuickSort on random data:
        // - Average case O(n log n) with median-of-3 pivot selection
        // - Bidirectional partitioning handles various data distributions efficiently
        //
        // Upper bound derived from StablePartition: a partition of length m costs at most
        // 3 comparisons (median-of-3) + 2m (main scan) + 2m (equal-fill scan) = 4m + 3.
        // In the worst case each partition removes only the pivot, so the total over the
        // chain m = n, n-1, ..., 2 is below 2n^2 + 5n.
        var minCompares = (ulong)n;
        var maxCompares = (ulong)(2 * n * n + 5 * n);
        var minSwaps = 0UL;
        var maxSwaps = (ulong)(n * Math.Max(1, Math.Log(n, 2)));

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);

        var minIndexReads = (ulong)n;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        var minIndexWrites = (ulong)n;
        await Assert.That(stats.IndexWriteCount >= minIndexWrites).IsTrue().Because($"IndexWriteCount ({stats.IndexWriteCount}) should be >= {minIndexWrites}");
    }

    [Test]
    [Arguments(10, 88, 3)]
    [Arguments(20, 224, 9)]
    [Arguments(50, 744, 39)]
    [Arguments(100, 1776, 101)]
    public async Task TheoreticalComparisonCountTest(int n, int expectedComparisons, int expectedSwaps)
    {
        // Test the exact comparison and swap counts for sorted data.
        // BidirectionalStableQuickSort on sorted data is fully deterministic:
        // - Median-of-3 at quartile positions picks a balanced pivot
        // - Bidirectional partitioning: less to front, greater to back (reversed), then reverse greater section
        // - Swaps occur during the reversal of the greater section in scratch buffer
        //
        // Empirical values for sorted data:
        // n=10:  Compare=88,   Swap=3
        // n=20:  Compare=224,  Swap=9
        // n=50:  Compare=744,  Swap=39
        // n=100: Compare=1776, Swap=101
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        BidirectionalStableQuickSort.Sort(sorted.AsSpan(), stats);

        await Assert.That(stats.CompareCount).IsEqualTo((ulong)expectedComparisons);
        await Assert.That(stats.SwapCount).IsEqualTo((ulong)expectedSwaps);
        await Assert.That(stats.IndexReadCount >= stats.CompareCount).IsTrue()
            .Because($"IndexReadCount ({stats.IndexReadCount}) should be >= CompareCount ({stats.CompareCount})");
    }

#endif
}
