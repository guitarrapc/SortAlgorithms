using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class WeakHeapSortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => WeakHeapSort.Sort(span, context);

    // Weak heap merges and extraction swap elements even for sorted input.
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.NonZero;

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        WeakHeapSort.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        WeakHeapSort.Sort(array.AsSpan(), 0, array.Length, stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortSingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9 };

        // Sort a single element range [2, 3)
        WeakHeapSort.Sort(array.AsSpan(), 2, 3, stats);

        // Array should be unchanged (single element is already sorted)
        await Assert.That(array).IsEquivalentTo([5, 3, 8, 1, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortBeginningTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 9, 7, 5, 3, 1, 2, 4, 6, 8 };

        // Sort only the first 5 elements [0, 5)
        WeakHeapSort.Sort(array.AsSpan(), 0, 5, stats);

        // Expected: first 5 sorted, last 4 unchanged
        await Assert.That(array).IsEquivalentTo([1, 3, 5, 7, 9, 2, 4, 6, 8], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortEndTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 3, 5, 7, 9, 8, 6, 4, 2 };

        // Sort only the last 4 elements [5, 9)
        WeakHeapSort.Sort(array.AsSpan(), 5, 9, stats);

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
        WeakHeapSort.Sort(sorted.AsSpan(), stats);

        // Weak Heap Sort characteristics:
        // Theoretical comparisons: ~n log n - 0.9n (fewer than standard heap sort)
        // Build weak heap phase: (n-1) merges, each with 1 comparison
        // Extract phase: (n-2) extractions, each requiring path descent and merges
        //
        // WeakHeapSort uses fewer comparisons than HeapSort due to:
        // - Only one comparison per merge (not two like standard heap)
        // - Reverse bits encode structure, eliminating redundant comparisons
        //
        // Expected pattern: approximately n * log2(n) - 0.9n comparisons

        var logN = Math.Log(n + 1, 2);
        // WeakHeapSort has fewer comparisons than HeapSort
        var minCompares = (ulong)(n * logN * 0.4);
        var maxCompares = (ulong)(n * logN * 2.0 + n);

        // Swaps are similar to HeapSort
        var minSwaps = (ulong)(n * logN * 0.3);
        var maxSwaps = (ulong)(n * logN * 1.5);

        // Each swap writes 2 elements
        var minWrites = minSwaps * 2;
        var maxWrites = maxSwaps * 2;

        // Each comparison reads 2 elements
        var minIndexReads = minCompares * 2;

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);
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
        WeakHeapSort.Sort(reversed.AsSpan(), stats);

        // Weak Heap Sort has O(n log n) time complexity regardless of input order
        // Reversed data shows similar patterns to sorted data
        //
        // Expected pattern: approximately n * log2(n) - 0.9n comparisons

        var logN = Math.Log(n + 1, 2);
        // WeakHeapSort has fewer comparisons than HeapSort
        var minCompares = (ulong)(n * logN * 0.4);
        var maxCompares = (ulong)(n * logN * 2.0 + n);

        // Swaps are similar to HeapSort
        var minSwaps = (ulong)(n * logN * 0.3);
        var maxSwaps = (ulong)(n * logN * 1.5);

        // Each swap writes 2 elements
        var minWrites = minSwaps * 2;
        var maxWrites = maxSwaps * 2;

        // Each comparison reads 2 elements
        var minIndexReads = minCompares * 2;

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);
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
        WeakHeapSort.Sort(random.AsSpan(), stats);

        // Weak Heap Sort has consistent O(n log n) time complexity for random data
        // The comparison count is approximately n * log2(n) - 0.9n
        // This is fewer than standard HeapSort (~2n log n)
        //
        // Expected pattern: approximately n * log2(n) - 0.9n, with variation due to randomness

        var logN = Math.Log(n + 1, 2);
        // WeakHeapSort has fewer comparisons than HeapSort
        var minCompares = (ulong)(n * logN * 0.4);
        var maxCompares = (ulong)(n * logN * 2.0 + n);

        // Swaps are similar to HeapSort
        var minSwaps = (ulong)(n * logN * 0.3);
        var maxSwaps = (ulong)(n * logN * 1.5);

        // Each swap writes 2 elements
        var minWrites = minSwaps * 2;
        var maxWrites = maxSwaps * 2;

        // Each comparison reads 2 elements
        var minIndexReads = minCompares * 2;

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
    }
}
