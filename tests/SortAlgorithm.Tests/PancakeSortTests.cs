using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class PancakeSortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => PancakeSort.Sort(span, context);

    // O(n²) FindMaxIndex scans make large inputs slow.
    protected override int MaxOrderTestSize => 512;

    // On sorted input the max is already in place each pass, so no flips occur.
    protected override CountExpectation SortedInputWrites => CountExpectation.Zero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        PancakeSort.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        PancakeSort.Sort(array.AsSpan(), 0, array.Length, stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortSingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9 };

        // Sort a single element range [2, 3)
        PancakeSort.Sort(array.AsSpan(), 2, 3, stats);

        // Array should be unchanged (single element is already sorted)
        await Assert.That(array).IsEquivalentTo([5, 3, 8, 1, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortBeginningTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 9, 7, 5, 3, 1, 2, 4, 6, 8 };

        // Sort only the first 5 elements [0, 5)
        PancakeSort.Sort(array.AsSpan(), 0, 5, stats);

        // Expected: first 5 sorted, last 4 unchanged
        await Assert.That(array).IsEquivalentTo([1, 3, 5, 7, 9, 2, 4, 6, 8], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortEndTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 3, 5, 7, 9, 8, 6, 4, 2 };

        // Sort only the last 4 elements [5, 9)
        PancakeSort.Sort(array.AsSpan(), 5, 9, stats);

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
        PancakeSort.Sort(sorted.AsSpan(), stats);

        // PancakeSort performs FindMaxIndex comparisons: sum(i) for i=2 to n = n(n-1)/2
        // For sorted data [0,1,2,...,n-1], max is always at the end (currentSize-1)
        // - Iteration 1 (currentSize=n): Find max in [0..n), max at n-1 → no flips (skip)
        // - Iteration 2 (currentSize=n-1): Find max in [0..n-1), max at n-2 → no flips (skip)
        // - ...
        // - All iterations skip because max is already in place
        var expectedCompares = (ulong)(n * (n - 1) / 2);
        var expectedSwaps = 0UL;
        var expectedWrites = 0UL; // No swaps = no writes

        // Each comparison reads 2 elements (i and maxIndex)
        var minIndexReads = expectedCompares * 2;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(expectedSwaps);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
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
        PancakeSort.Sort(reversed.AsSpan(), stats);

        // For reversed data [n-1, n-2, ..., 1, 0]:
        // - Iteration 1 (currentSize=n): Find max in [0..n), max at position 0 (value n-1)
        //   → Flip(0, 0) does nothing (start==end), then Flip(0, n-1) reverses entire array
        // - After first flip: [0, 1, 2, ..., n-1] (becomes sorted!)
        // - Remaining iterations: max is always at currentSize-1, so no more flips
        //
        // Comparisons: sum(i) for i=2 to n = n(n-1)/2
        // Flips: Only the first iteration performs Flip(0, n-1)
        // Swaps in one flip of length n: n/2 swaps
        var expectedCompares = (ulong)(n * (n - 1) / 2);
        var expectedSwaps = (ulong)(n / 2);
        var expectedWrites = expectedSwaps * 2; // Each swap writes 2 elements

        // Each comparison reads 2 elements
        var minIndexReads = expectedCompares * 2;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(expectedSwaps);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
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
        PancakeSort.Sort(random.AsSpan(), stats);

        // PancakeSort always performs n(n-1)/2 comparisons to find max in each iteration
        var expectedCompares = (ulong)(n * (n - 1) / 2);

        // For random data, flip count varies:
        // - Best case: max is already at the end each time → 0 flips
        // - Worst case: max is at position 0 each time → 2n flips
        //   (one flip to bring to front, one to place at end, for each of n iterations)
        // Each flip of average length performs O(n/2) swaps
        //
        // We expect:
        // - Minimum swaps: 0 (lucky sorted arrangement)
        // - Maximum swaps: roughly n² in pathological cases
        var minSwaps = 0UL;
        var maxSwaps = (ulong)(n * n);

        // Each comparison reads 2 elements
        var minIndexReads = expectedCompares * 2;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
    }
}
