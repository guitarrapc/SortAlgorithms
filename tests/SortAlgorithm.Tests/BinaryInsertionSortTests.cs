using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class BinaryInsertionSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => BinaryInsertionSort.Sort(span, context);

    // O(n^2) shifts: keep data-driven inputs small.
    protected override int MaxOrderTestSize => 512;

    // On sorted input every search returns the element's own position: no writes, no swaps.
    protected override CountExpectation SortedInputWrites => CountExpectation.Zero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        BinaryInsertionSort.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        BinaryInsertionSort.Sort(array.AsSpan(), 0, array.Length, stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortSingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9 };

        // Sort a single element range [2, 3)
        BinaryInsertionSort.Sort(array.AsSpan(), 2, 3, stats);

        // Array should be unchanged (single element is already sorted)
        await Assert.That(array).IsEquivalentTo([5, 3, 8, 1, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortBeginningTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 9, 7, 5, 3, 1, 2, 4, 6, 8 };

        // Sort only the first 5 elements [0, 5)
        BinaryInsertionSort.Sort(array.AsSpan(), 0, 5, stats);

        // Expected: first 5 sorted, last 4 unchanged
        await Assert.That(array).IsEquivalentTo([1, 3, 5, 7, 9, 2, 4, 6, 8], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortEndTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 3, 5, 7, 9, 8, 6, 4, 2 };

        // Sort only the last 4 elements [5, 9)
        BinaryInsertionSort.Sort(array.AsSpan(), 5, 9, stats);

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
        BinaryInsertionSort.Sort(sorted.AsSpan(), stats);

        // Every element is binary-searched, even when it is already in place.
        // On sorted input each search over i elements goes right at every step: floor(log2(i+1)) comparisons.
        var expectedCompares = SearchComparisons(n, sorted: true);

        // Sorted data: no shifts needed (all elements are already in correct positions)
        var expectedWrites = 0UL;

        // IndexReadCount: the held element, plus one read per comparison
        var expectedIndexReads = expectedCompares + (ulong)(n - 1);

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedIndexReads);
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
        BinaryInsertionSort.Sort(reversed.AsSpan(), stats);

        // On reversed input each search over i elements goes left at every step: ceiling(log2(i+1)) comparisons.
        var expectedCompares = SearchComparisons(n, sorted: false);

        // Reversed data: worst case for shifts
        // Element at position i needs to be shifted to position 0, requiring i shifts
        // For each element at position i (from 1 to n-1):
        // - Shift i elements to the right (i writes)
        // - Write the current element at position 0 (1 write)
        // Total: n(n-1)/2 + (n-1)
        var shifts = (ulong)(n * (n - 1) / 2);
        var expectedWrites = shifts + (ulong)(n - 1);

        // IndexReadCount: the held element, one read per comparison, and one per shifted element
        var expectedIndexReads = (ulong)(n - 1) + expectedCompares + shifts;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedIndexReads);
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
        BinaryInsertionSort.Sort(random.AsSpan(), stats);

        // A search over i elements costs floor(log2(i+1)) or ceiling(log2(i+1)) comparisons,
        // so the total lies between the sorted and the reversed counts.
        var minCompares = SearchComparisons(n, sorted: true);
        var maxCompares = SearchComparisons(n, sorted: false);

        // Random data: varies significantly based on arrangement
        // Best case: nearly sorted (minimal shifts, close to 0 writes)
        // Worst case: reverse sorted (maximum shifts, n(n+1)/2 writes)
        var minWrites = 0UL;
        var maxWrites = (ulong)((n * (n + 1)) / 2 + 1);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount >= minCompares / 2).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minCompares / 2}");
    }

    /// <summary>
    /// Total comparisons of the searches over 1..n-1 sorted elements: floor(log2(i+1)) each when every step goes right,
    /// ceiling(log2(i+1)) each when every step goes left.
    /// </summary>
    static ulong SearchComparisons(int n, bool sorted)
    {
        ulong total = 0;
        for (var i = 1; i < n; i++)
        {
            var floor = (ulong)System.Numerics.BitOperations.Log2((uint)(i + 1));
            total += sorted || System.Numerics.BitOperations.IsPow2(i + 1) ? floor : floor + 1;
        }
        return total;
    }
}
