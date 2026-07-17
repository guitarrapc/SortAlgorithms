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

    // Early continuation on sorted input skips binary search and insertion: no writes, no swaps.
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

        // With early continuation optimization:
        // For sorted data, each element at position i is compared with element at i-1
        // If comparison shows i-1 <= i (sorted), we skip binary search and insertion
        // This results in exactly (n-1) comparisons for sorted data
        //
        // Early continuation check: if (i > first && s.Compare(i - 1, i) <= 0) continue;
        // - For sorted data, all elements pass this check
        // - Each element from position 1 to n-1 requires exactly 1 comparison
        var expectedCompares = (ulong)(n - 1);

        // Sorted data: no shifts needed (all elements are already in correct positions)
        var expectedWrites = 0UL;

        // IndexReadCount: 2 reads per comparison (read i-1 and i)
        // For sorted data with early continuation, (n-1) comparisons × 2 reads = 2(n-1) reads
        var expectedIndexReads = (ulong)(2 * (n - 1));

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

        // Binary Insertion Sort comparisons: at most ceiling(log2(i+1)) per search
        // For reversed data, binary search may take more comparisons than sorted data
        var expectedCompares = CalculateBinaryInsertSortComparisons(n);

        // Reversed data can cause more comparisons, allow wider range (50% to 200%)
        var minCompares = expectedCompares / 2;
        var maxCompares = expectedCompares * 2;

        // Reversed data: worst case for shifts
        // Element at position i needs to be shifted to position 0, requiring i shifts
        // For each element at position i (from 1 to n-1):
        // - Shift i elements to the right (i writes)
        // - Write the current element at position 0 (1 write)
        // Total: sum from i=1 to n-1 of (i+1) = n(n+1)/2 - 1
        var minWrites = (ulong)((n * (n + 1)) / 2 - 2);
        var maxWrites = (ulong)((n * (n + 1)) / 2 + 1);

        // IndexReadCount: Each comparison reads 1 element during binary search
        // Plus reads during shifting: each shift reads the element being moved
        // Total shift reads = n(n-1)/2
        var minShiftReads = (ulong)(n * (n - 1) / 2);
        var minIndexReads = minCompares + minShiftReads;

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
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
        BinaryInsertionSort.Sort(random.AsSpan(), stats);

        // Binary Insertion Sort comparisons: at most ceiling(log2(i+1)) per search
        // Random data can vary widely, allow very wide range
        var expectedCompares = CalculateBinaryInsertSortComparisons(n);
        var minCompares = expectedCompares / 2;
        var maxCompares = expectedCompares * 2;

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
    /// Calculate theoretical number of comparisons for Binary Insertion Sort
    /// Uses the maximum number of comparisons per binary search (worst case)
    /// For a range of size n, binary search takes at most ceiling(log2(n+1)) comparisons
    /// </summary>
    private ulong CalculateBinaryInsertSortComparisons(int n)
    {
        ulong totalCompares = 0;
        for (int i = 1; i < n; i++)
        {
            // Binary search in range [0..i) can take up to ceiling(log2(i+1)) comparisons
            // This is the worst-case number of iterations for the while loop
            if (i == 1)
                totalCompares += 1;
            else
                totalCompares += (ulong)Math.Ceiling(Math.Log2(i + 1));
        }
        return totalCompares;
    }
}
