using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class QuickSort3wayTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => QuickSort3way.Sort(span, context);

    // 3-way partition performs pivot placement and classification swaps even on sorted input
    // (MockSortedData sizes exceed the InsertionSort threshold, so the 3-way path runs).
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.NonZero;

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        QuickSort3way.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        QuickSort3way.Sort(array.AsSpan(), 0, array.Length, stats);

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
        // Use array of size n so bounds are meaningful for each argument
        var sorted = Enumerable.Range(1, n).ToArray();
        QuickSort3way.Sort(sorted.AsSpan(), stats);

        // QuickSort 3-way (Dutch National Flag) on sorted data:
        // - n < 16 (e.g. n=10): InsertionSort fallback
        //   Sorted array → each element compared once with predecessor, no shifts → n-1 comparisons, 0 swaps
        // - n >= 16: 3-way partition with median-of-3 pivot
        //   Median of (left, mid, right) = mid → balanced partition
        //   Equal region has exactly 1 element per level (all values distinct)
        //   Recursion depth: O(log n); each level does n comparisons → O(n log n) total
        //
        // Comparisons:
        // - InsertionSort path (n=10): n-1 = 9
        // - 3-way path: median-of-3 (2-3) + partition scan (n-1) per level × log₂(n) levels
        //
        // Swaps:
        // - InsertionSort path: 0 (uses Write, not Swap; sorted input needs no writes)
        // - 3-way path: ~n/2 per level × log₂(n) levels

        var minCompares = (ulong)(n - 1); // InsertionSort sorted: n-1; 3-way: >= n-1
        var maxCompares = (ulong)(3 * n * Math.Log(n, 2)); // Upper bound

        var minSwaps = 0UL; // InsertionSort uses Write not Swap; sorted 3-way starts with pivot placement
        var maxSwaps = (ulong)(1.5 * n * Math.Log(n, 2)); // Upper bound

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);

        // IndexReads: each comparison reads at least one element
        var minIndexReads = stats.CompareCount;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        // IndexWrites: each swap writes 2 elements
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
        QuickSort3way.Sort(reversed.AsSpan(), stats);

        // QuickSort 3-way (Dutch National Flag) on reversed data:
        // - n < 16 (e.g. n=10): InsertionSort fallback
        //   Reversed array → each element shifts to front → n(n-1)/2 comparisons, 0 Swaps (uses Write)
        // - n >= 16: 3-way partition with median-of-3 pivot
        //   Median of (first, middle, last) selects a reasonable pivot → balanced partition
        //   All values distinct → equal region = 1 element per level
        //   O(n log n) comparisons and swaps; more swaps than sorted since elements need rearranging
        //
        // Swaps:
        // - InsertionSort path: 0 (uses Write not Swap, even for reversed input)
        // - 3-way path: O(n log n)

        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(3 * n * Math.Log(n, 2));

        var minSwaps = 0UL; // n=10 uses InsertionSort (SwapCount=0); n>=16 uses 3-way (SwapCount>0)
        var maxSwaps = (ulong)(2 * n * Math.Log(n, 2));

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);

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
        QuickSort3way.Sort(random.AsSpan(), stats);

        // QuickSort 3-way (Dutch National Flag) on random data:
        // - n < 16: InsertionSort fallback; O(n²) worst but small constant for tiny n
        // - n >= 16: 3-way QuickSort
        //   Average case O(n log n); median-of-3 provides good pivot selection
        //   3-way advantage appears when duplicates exist; for random distinct data behavior
        //   is similar to standard QuickSort

        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(3 * n * Math.Log(n, 2));

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
        QuickSort3way.Sort(sameValues.AsSpan(), stats);

        // QuickSort 3-way (Dutch National Flag) key advantage: all-equal input is O(n)
        //
        // - n < 16 (n=5, n=10): InsertionSort fallback
        //   All equal → each element already in place → n-1 comparisons, 0 swaps, 0 writes
        //
        // - n >= 16 (n=20, n=50): 3-way QuickSort
        //   Median-of-3: all three sampled elements are equal → 0 comparison-swaps; 1 Swap(mid, left)
        //   Partition scan: ALL elements compare == pivot → only i++ branch fires, 0 swaps
        //   Equal region [lt, gt] covers the entire array after one O(n) scan
        //   Both sub-partitions are empty → NO further recursion
        //   Total: n-1 partition comparisons + 3 median comparisons = O(n), 1 swap
        //
        // This is the fundamental advantage of 3-way DNF over Hoare/Lomuto:
        //   Hoare on all-equal: O(n log n) swaps (equal elements distributed to both sides)
        //   3-way DNF on all-equal: O(n) comparisons, O(1) swaps

        var minCompares = (ulong)(n - 1); // InsertionSort: n-1; 3-way: n+2 median comparisons
        var maxCompares = (ulong)(2 * n); // O(n) — NOT O(n log n)

        var minSwaps = 0UL; // InsertionSort: 0 (equal elements are already in place)
        var maxSwaps = 3UL; // 3-way: only pivot placement Swap(mid, left); no partition swaps

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);

        // Verify all values are unchanged
        foreach (var item in sameValues) await Assert.That(item).IsEqualTo(42);

        var minIndexReads = stats.CompareCount;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        var minIndexWrites = stats.SwapCount * 2;
        await Assert.That(stats.IndexWriteCount >= minIndexWrites).IsTrue().Because($"IndexWriteCount ({stats.IndexWriteCount}) should be >= {minIndexWrites}");
    }
}
