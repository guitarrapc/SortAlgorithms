using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class TernaryHeapSortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => TernaryHeapSort.Sort(span, context);

    // Hole-based heapify writes elements even for sorted input; Floyd-style extraction eliminates all swaps.
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        TernaryHeapSort.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        TernaryHeapSort.Sort(array.AsSpan(), 0, array.Length, stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortSingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9 };

        // Sort a single element range [2, 3)
        TernaryHeapSort.Sort(array.AsSpan(), 2, 3, stats);

        // Array should be unchanged (single element is already sorted)
        await Assert.That(array).IsEquivalentTo([5, 3, 8, 1, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortBeginningTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 9, 7, 5, 3, 1, 2, 4, 6, 8 };

        // Sort only the first 5 elements [0, 5)
        TernaryHeapSort.Sort(array.AsSpan(), 0, 5, stats);

        // Expected: first 5 sorted, last 4 unchanged
        await Assert.That(array).IsEquivalentTo([1, 3, 5, 7, 9, 2, 4, 6, 8], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortEndTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 3, 5, 7, 9, 8, 6, 4, 2 };

        // Sort only the last 4 elements [5, 9)
        TernaryHeapSort.Sort(array.AsSpan(), 5, 9, stats);

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
        TernaryHeapSort.Sort(sorted.AsSpan(), stats);

        // Ternary Heap Sort with Floyd-style extraction:
        // Build heap phase: O(n) comparisons using Floyd's heap construction
        // Extract phase: (n-1) extractions, each requiring O(log₃ n) heapify
        // Floyd-style extraction: Read root + last, sift down last, write root to end
        // No swaps needed in extraction phase
        //
        // Each heapify compares with 3 children instead of 2
        //
        // Pattern: approximately n * log₃(n) * 3 ≈ n * log₂(n) * 1.9 for compares

        var logN = Math.Log(n + 1, 3); // log base 3 for ternary heap
        var minCompares = (ulong)(n * logN * 1.0);
        var maxCompares = (ulong)(n * logN * 4.5 + n);

        // Floyd-style extraction eliminates all swaps
        var expectedSwaps = 0UL;

        // Writes: FloydHeapify during build + hole-based heapify during extraction
        var minWrites = (ulong)(n * logN * 1.0);
        var maxWrites = (ulong)(n * logN * 4.0 + n * 2);

        // Each comparison reads 2 elements
        var minIndexReads = minCompares * 2;

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(expectedSwaps);
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
        TernaryHeapSort.Sort(reversed.AsSpan(), stats);

        // Ternary Heap Sort with Floyd-style extraction has O(n log n) time complexity regardless of input order
        // Reversed data shows similar patterns to sorted data due to heap property
        // Floyd-style extraction: Read root + last, sift down last, write root to end
        // No swaps needed in extraction phase
        //
        // Pattern: approximately n * log₃(n) * 3 for compares

        var logN = Math.Log(n + 1, 3); // log base 3 for ternary heap
        var minCompares = (ulong)(n * logN * 1.0);
        var maxCompares = (ulong)(n * logN * 4.5 + n);

        // Floyd-style extraction eliminates all swaps
        var expectedSwaps = 0UL;

        // Writes: FloydHeapify during build + hole-based heapify during extraction
        var minWrites = (ulong)(n * logN * 1.0);
        var maxWrites = (ulong)(n * logN * 4.0 + n * 2);

        // Each comparison reads 2 elements
        var minIndexReads = minCompares * 2;

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(expectedSwaps);
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
        TernaryHeapSort.Sort(random.AsSpan(), stats);

        // Ternary Heap Sort with Floyd-style extraction has consistent O(n log n) time complexity for random data
        // The values are similar to sorted/reversed cases
        // Floyd-style extraction: Read root + last, sift down last, write root to end
        // No swaps needed in extraction phase
        //
        // Pattern: approximately n * log₃(n) * 3, with variation due to randomness

        var logN = Math.Log(n + 1, 3); // log base 3 for ternary heap
        var minCompares = (ulong)(n * logN * 1.0);
        var maxCompares = (ulong)(n * logN * 4.5 + n);

        // Floyd-style extraction eliminates all swaps
        var expectedSwaps = 0UL;

        // Writes: FloydHeapify during build + hole-based heapify during extraction
        var minWrites = (ulong)(n * logN * 1.0);
        var maxWrites = (ulong)(n * logN * 4.0 + n * 2);

        // Each comparison reads 2 elements
        var minIndexReads = minCompares * 2;

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(expectedSwaps);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
    }
}
