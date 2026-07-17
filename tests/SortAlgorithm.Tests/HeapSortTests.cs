using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class HeapSortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => HeapSort.Sort(span, context);

    // Hole-based sift-down writes elements even for sorted input, but performs no swaps.
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        HeapSort.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        HeapSort.Sort(array.AsSpan(), 0, array.Length, stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortSingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9 };

        // Sort a single element range [2, 3)
        HeapSort.Sort(array.AsSpan(), 2, 3, stats);

        // Array should be unchanged (single element is already sorted)
        await Assert.That(array).IsEquivalentTo([5, 3, 8, 1, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortBeginningTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 9, 7, 5, 3, 1, 2, 4, 6, 8 };

        // Sort only the first 5 elements [0, 5)
        HeapSort.Sort(array.AsSpan(), 0, 5, stats);

        // Expected: first 5 sorted, last 4 unchanged
        await Assert.That(array).IsEquivalentTo([1, 3, 5, 7, 9, 2, 4, 6, 8], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortEndTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 3, 5, 7, 9, 8, 6, 4, 2 };

        // Sort only the last 4 elements [5, 9)
        HeapSort.Sort(array.AsSpan(), 5, 9, stats);

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
        HeapSort.Sort(sorted.AsSpan(), stats);

        // Heap Sort characteristics:
        // Build heap phase: O(n) comparisons with some writes even for sorted data
        // Extract phase: (n-1) extractions, each requiring O(log n) heapify
        //
        // Empirical observations for sorted data:
        // n=10:  Compare=41,  Swap=0
        // n=20:  Compare=121, Swap=0
        // n=50:  Compare=405, Swap=0
        // n=100: Compare=1031, Swap=0
        //
        // SwapCount: 0 - No swaps; extraction uses Read+Heapify+Write pattern
        // IndexWriteCount: ~n*log(n) from hole-based sift-down writes + (n-1) writes for max placement

        var logN = Math.Log(n + 1, 2);
        var minCompares = (ulong)(n * logN * 0.5);
        var maxCompares = (ulong)(n * logN * 2.5 + n);

        var minWrites = (ulong)(n * logN * 0.4);
        var maxWrites = (ulong)(n * logN * 2.5 + n);

        // Each comparison reads 2 elements
        var minIndexReads = minCompares * 2;

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
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
        HeapSort.Sort(reversed.AsSpan(), stats);

        // Heap Sort has O(n log n) time complexity regardless of input order
        // Reversed data shows similar patterns to sorted data due to heap property
        //
        // Empirical observations for reversed data:
        // n=10:  Compare=38,  Swap=0
        // n=20:  Compare=115, Swap=0
        // n=50:  Compare=401, Swap=0
        // n=100: Compare=1023, Swap=0
        //
        // SwapCount: 0 - No swaps; extraction uses Read+Heapify+Write pattern
        // IndexWriteCount: ~n*log(n) from hole-based sift-down writes + (n-1) writes for max placement

        var logN = Math.Log(n + 1, 2);
        var minCompares = (ulong)(n * logN * 0.5);
        var maxCompares = (ulong)(n * logN * 2.5 + n);

        var minWrites = (ulong)(n * logN * 0.4);
        var maxWrites = (ulong)(n * logN * 2.5 + n);

        // Each comparison reads 2 elements
        var minIndexReads = minCompares * 2;

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
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
        HeapSort.Sort(random.AsSpan(), stats);

        // Heap Sort has consistent O(n log n) time complexity for random data
        // The values are similar to sorted/reversed cases
        //
        // Empirical observations for random data (example):
        // n=10:  Compare=38,  Swap=0
        // n=20:  Compare=113, Swap=0
        // n=50:  Compare=405, Swap=0
        // n=100: Compare=1031, Swap=0
        //
        // SwapCount: 0 - No swaps; extraction uses Read+Heapify+Write pattern
        // IndexWriteCount: ~n*log(n) from hole-based sift-down writes + (n-1) writes for max placement

        var logN = Math.Log(n + 1, 2);
        var minCompares = (ulong)(n * logN * 0.5);
        var maxCompares = (ulong)(n * logN * 2.5 + n);

        var minWrites = (ulong)(n * logN * 0.4);
        var maxWrites = (ulong)(n * logN * 2.5 + n);

        // Each comparison reads 2 elements
        var minIndexReads = minCompares * 2;

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
    }
}
