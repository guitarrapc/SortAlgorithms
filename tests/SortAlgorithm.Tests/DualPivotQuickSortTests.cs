using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class DualPivotQuickSortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => DualPivotQuickSort.Sort(span, context);

    // Dual-pivot partitioning performs pivot placement swaps at each recursion level even on sorted input.
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.NonZero;

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        DualPivotQuickSort.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        DualPivotQuickSort.Sort(array.AsSpan(), 0, array.Length, stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9], CollectionOrdering.Matching);
    }

    [Test]
    [Arguments(30)]
    [Arguments(50)]
    [Arguments(100)]
    [Arguments(200)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        DualPivotQuickSort.Sort(sorted.AsSpan(), stats);

        // QuickSort Dual Pivot on sorted data:
        // - Best case behavior when data is already sorted
        // - The algorithm uses two pivots (leftmost and rightmost elements)
        // - For sorted data:
        //   * Initial comparison between left and right pivots
        //   * Partitioning scans through all elements
        //   * Each element is compared with both pivots
        //   * Minimal swaps (only final pivot placements)
        //
        // Expected behavior:
        // - Comparisons: O(n log n) in best case
        //   Each level of recursion processes all n elements with 2 pivot comparisons each
        //   With 3-way partitioning, depth is approximately log3(n)
        // - Swaps: O(log n) - only pivot placements at each recursion level
        //   2 swaps per level (placing both pivots) * log3(n) levels

        var minCompares = (ulong)(n * 2); // At minimum, each element compared with both pivots once

        // Swaps: For sorted data, only pivot placements
        // Each recursion level: 2 swaps (left and right pivots)
        // Depth: approximately log3(n)
        var recursionDepth = (int)Math.Ceiling(Math.Log(n, 3));
        var expectedSwaps = (ulong)(recursionDepth * 2);

        await Assert.That(stats.CompareCount >= minCompares).IsTrue().Because($"CompareCount ({stats.CompareCount}) should be >= {minCompares}");
        await Assert.That(stats.SwapCount >= expectedSwaps).IsTrue().Because($"SwapCount ({stats.SwapCount}) should be >= {expectedSwaps}");

        // IndexReads: Reduced due to pivot caching — inner-loop comparisons now use Compare(int, T)
        // which records only the scanned element (1 read), not the pivot index.
        // InsertionSort fallback still records 2 reads per compare, so the overall ratio
        // varies by input but is always >= 1 read per comparison.
        var minIndexReads = stats.CompareCount;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
    }

    [Test]
    [Arguments(30)]
    [Arguments(50)]
    [Arguments(100)]
    [Arguments(200)]
    public async Task TheoreticalValuesReversedTest(int n)
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        DualPivotQuickSort.Sort(reversed.AsSpan(), stats);

        // QuickSort Dual Pivot on reversed data:
        // - Initially, left > right, so first swap occurs
        // - During partitioning:
        //   * Elements smaller than left pivot go to left section
        //   * Elements larger than right pivot go to right section
        //   * Elements between pivots stay in middle
        // - For reversed data, most elements need repositioning
        //
        // Expected behavior:
        // - Comparisons: O(n log n) average case
        //   With dual pivots, partitioning is more balanced than single pivot
        // - Swaps: O(n log n) average case
        //   Many elements need to be moved during partitioning

        var minCompares = (ulong)(n * 2); // At minimum, each element compared with both pivots
        var maxCompares = (ulong)(n * n); // Worst case (though rare with dual pivot)

        var minSwaps = (ulong)(n / 2); // At least half the elements need swapping
        var maxSwaps = (ulong)(n * n); // Worst case

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);

        // IndexReads: Reduced due to pivot caching — inner-loop comparisons now use Compare(int, T)
        // which records only the scanned element (1 read), not the pivot index.
        // InsertionSort fallback still records 2 reads per compare, so the overall ratio
        // varies by input but is always >= 1 read per comparison.
        var minIndexReads = stats.CompareCount;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
    }

    [Test]
    [Arguments(30, 42)]
    [Arguments(30, 1234)]
    [Arguments(50, 42)]
    [Arguments(50, 1234)]
    [Arguments(100, 42)]
    [Arguments(100, 1234)]
    [Arguments(200, 42)]
    [Arguments(200, 1234)]
    public async Task TheoreticalValuesRandomTest(int n, int seed)
    {
        var stats = new StatisticsContext();
        var random = TestHelpers.ShuffledRange(n, seed);
        DualPivotQuickSort.Sort(random.AsSpan(), stats);

        // QuickSort Dual Pivot on random data: average case O(n log n)
        // - Dual pivot partitioning divides array into 3 parts
        // - Average recursion depth: log3(n) (more balanced than single pivot)
        // - Each partition level processes all n elements
        //
        // Expected behavior:
        // - Comparisons: O(n log n) average
        //   Approximately 2n * log3(n) comparisons
        // - Swaps: O(n log n) average
        //   Varies based on distribution

        var minCompares = (ulong)(n * 2); // Minimum: each element compared once
        var maxCompares = (ulong)(n * n); // Maximum: worst case (rare)

        var minSwaps = (ulong)Math.Log(n, 3) * 2; // Best case: only pivot placements
        var maxSwaps = (ulong)(n * Math.Log(n, 3) * 2); // Average case estimate

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount >= minSwaps).IsTrue().Because($"SwapCount ({stats.SwapCount}) should be >= {minSwaps}");

        // IndexReads: Reduced due to pivot caching — inner-loop comparisons now use Compare(int, T)
        // which records only the scanned element (1 read), not the pivot index.
        // InsertionSort fallback still records 2 reads per compare, so the overall ratio
        // varies by input but is always >= 1 read per comparison.
        var minIndexReads = stats.CompareCount;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
    }

    [Test]
    [Arguments(5)]
    [Arguments(10)]
    [Arguments(20)]
    public async Task TheoreticalValuesSameElementsTest(int n)
    {
        var stats = new StatisticsContext();
        var sameValues = Enumerable.Repeat(42, n).ToArray();
        DualPivotQuickSort.Sort(sameValues.AsSpan(), stats);

        // QuickSort Dual Pivot with all same values:
        // - Left and right pivots are equal
        // - All elements equal to pivots
        // - Partitioning still occurs but elements don't move much
        // - Middle section contains most/all elements
        //
        // Expected behavior:
        // - Comparisons: O(n) for partitioning in each recursion level
        //   In the partitioning loop, each element (except pivots) is compared
        //   For n elements, that's roughly n-2 comparisons per level
        // - Swaps: 2 swaps per recursion level (final pivot placements at lines 75-76)
        //   Plus the recursive calls on left/middle/right sections

        var minCompares = (ulong)(n - 2); // At minimum, partitioning comparisons for one level

        // Swaps: Very few needed since all values are equal
        // Only pivot positioning swaps at each level
        var recursionDepth = (int)Math.Ceiling(Math.Log(Math.Max(n, 2), 3));
        var maxSwaps = (ulong)(recursionDepth * 4); // Allow some extra for partitioning

        await Assert.That(stats.CompareCount >= minCompares).IsTrue().Because($"CompareCount ({stats.CompareCount}) should be >= {minCompares}");
        await Assert.That(stats.SwapCount <= maxSwaps).IsTrue().Because($"SwapCount ({stats.SwapCount}) should be <= {maxSwaps} for all equal elements");

        // Verify the array is still correct (all values unchanged)
        foreach (var item in sameValues) await Assert.That(item).IsEqualTo(42);
    }
}
