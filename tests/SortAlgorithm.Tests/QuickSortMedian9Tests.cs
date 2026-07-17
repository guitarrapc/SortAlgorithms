using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class QuickSortMedian9Tests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => QuickSortMedian9.Sort(span, context);

    // Hoare partition swaps elements even on sorted input (swaps occur while l <= r during scan crossing).
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.NonZero;

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        QuickSortMedian9.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        QuickSortMedian9.Sort(array.AsSpan(), 0, array.Length, stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9], CollectionOrdering.Matching);
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
        QuickSortMedian9.Sort(sorted.AsSpan(), stats);

        // QuickSort Median-of-9 with Hoare partition on sorted data:
        // - Median-of-9 selects median of (left, mid, right) as pivot
        // - For sorted data, median of (first, middle, last) is the middle element
        // - Hoare partition performs bidirectional scanning
        //
        // Best case complexity analysis:
        // - Each partition divides array roughly in half
        // - Recursion depth: O(log n)
        // - At each level, all n elements are processed
        //
        // Comparisons:
        // - Median-of-9: 2-3 comparisons per partition call
        // - Hoare partition: Each element compared with pivot once
        // - Total: approximately 2n log n comparisons
        //
        // Swaps:
        // - For sorted data, Hoare partition still performs swaps when l <= r
        // - Even when elements are in correct relative positions, swaps occur
        // - Total: approximately 0.5n log n swaps

        var minCompares = (ulong)(n); // At minimum, some comparisons occur
        var maxCompares = (ulong)(3 * n * Math.Log(n, 2)); // Upper bound

        // For sorted data with Hoare partition, swaps still occur
        var minSwaps = 0UL;
        var maxSwaps = (ulong)(n * Math.Log(n, 2)); // Upper bound

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);

        // IndexReads: Each comparison reads elements, each swap reads and writes
        var minIndexReads = stats.CompareCount;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        // IndexWrites: Each swap writes 2 elements
        var minIndexWrites = stats.SwapCount * 2;
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
        QuickSortMedian9.Sort(reversed.AsSpan(), stats);

        // QuickSort Median-of-9 with Hoare partition on reversed data:
        // - Median-of-9 helps avoid worst-case by selecting better pivots
        // - For reversed data, median of (first, middle, last) is still reasonable
        // - Hoare partition is more efficient than Lomuto for reversed data
        //
        // Average case complexity (Median-of-9 avoids O(n²)):
        // - Recursion depth: O(log n) on average
        // - Comparisons: approximately 1.386n log₂ n
        // - Swaps: approximately 0.33n log₂ n
        //
        // For reversed data specifically:
        // - Most elements need to be swapped during partitioning
        // - More swaps than sorted data, but still O(n log n)

        var minCompares = (ulong)(n);
        var maxCompares = (ulong)(3 * n * Math.Log(n, 2));

        var minSwaps = (ulong)(n / 4); // At least some swaps needed
        var maxSwaps = (ulong)(2 * n * Math.Log(n, 2)); // Allow for more swaps

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);

        // IndexReads and IndexWrites should be proportional to operations
        var minIndexReads = stats.CompareCount;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        var minIndexWrites = stats.SwapCount * 2;
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
        QuickSortMedian9.Sort(random.AsSpan(), stats);

        // QuickSort Median-of-9 with Hoare partition on random data:
        // - Average case O(n log n) complexity
        // - Median-of-9 pivot selection provides good partitioning
        // - Hoare partition performs approximately 1.386n log₂ n comparisons
        // - Swaps: approximately 0.33n log₂ n on average

        var minCompares = (ulong)(n);
        var maxCompares = (ulong)(3 * n * Math.Log(n, 2) + 2);

        var minSwaps = 0UL;
        var maxSwaps = (ulong)(2 * n * Math.Log(n, 2));

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);

        var minIndexReads = stats.CompareCount;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        var minIndexWrites = stats.SwapCount * 2;
        await Assert.That(stats.IndexWriteCount >= minIndexWrites).IsTrue().Because($"IndexWriteCount ({stats.IndexWriteCount}) should be >= {minIndexWrites}");
    }

    [Test]
    [Arguments(5)]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    public async Task TheoreticalValuesSameElementsTest(int n)
    {
        var stats = new StatisticsContext();
        var sameValues = Enumerable.Repeat(42, n).ToArray();
        QuickSortMedian9.Sort(sameValues.AsSpan(), stats);

        // QuickSort Median-of-9 with Hoare partition on all equal elements:
        // - Median-of-9: all three sampled elements are equal
        // - Pivot equals all elements in the array
        // - Hoare partition: elements equal to pivot are distributed between partitions
        // - This causes many swaps even though all elements are equal
        //
        // Expected behavior:
        // - Comparisons: O(n log n) - partitioning still occurs at each level
        // - Swaps: O(n log n) - Hoare partition swaps elements even when equal to pivot
        //   This is a known characteristic: condition is "l <= r" not "l < r"
        // - The algorithm still terminates correctly

        var minCompares = (ulong)(n - 1); // At minimum, some comparisons
        var maxCompares = (ulong)(3 * n * Math.Log(Math.Max(n, 2), 2));

        var minSwaps = 0UL;
        var maxSwaps = (ulong)(2 * n * Math.Log(Math.Max(n, 2), 2));

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);

        // Verify array is still correct (all values unchanged)
        foreach (var item in sameValues) await Assert.That(item).IsEqualTo(42);

        var minIndexReads = stats.CompareCount;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        var minIndexWrites = stats.SwapCount * 2;
        await Assert.That(stats.IndexWriteCount >= minIndexWrites).IsTrue().Because($"IndexWriteCount ({stats.IndexWriteCount}) should be >= {minIndexWrites}");
    }
}
