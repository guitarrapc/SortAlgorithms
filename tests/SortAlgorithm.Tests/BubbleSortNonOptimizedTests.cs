using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class BubbleSortNonOptimizedTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => BubbleSortNonOptimized.Sort(span, context);

    // O(n^2) algorithm: keep data-driven inputs small.
    protected override int MaxOrderTestSize => 512;

    // Sorted input still compares every pair but never swaps, hence no writes.
    protected override CountExpectation SortedInputWrites => CountExpectation.Zero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        BubbleSortNonOptimized.Sort(sorted.AsSpan(), stats);

        // Bubble Sort always performs n(n-1)/2 comparisons regardless of input order
        // For sorted data, no swaps are needed since all elements are already in order
        var expectedCompares = (ulong)(n * (n - 1) / 2);
        var expectedSwaps = 0UL;
        var expectedWrites = 0UL; // No swaps = no writes

        // Each comparison reads 2 elements (positions j and j-1)
        // Total reads = 2 * number of comparisons
        var expectedReads = expectedCompares * 2;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(expectedSwaps);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
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
        BubbleSortNonOptimized.Sort(reversed.AsSpan(), stats);

        // Bubble Sort worst case: reversed array
        // Comparisons: n(n-1)/2 (all adjacent pairs are compared)
        // Swaps: n(n-1)/2 (every comparison results in a swap)
        var expectedCompares = (ulong)(n * (n - 1) / 2);
        var expectedSwaps = (ulong)(n * (n - 1) / 2);

        // Each swap writes 2 elements (swap reads and writes both positions)
        var expectedWrites = expectedSwaps * 2;

        // Each comparison reads 2 elements + each swap reads 2 elements
        // Total reads = 2 * comparisons + 2 * swaps = 4 * n(n-1)/2
        var expectedReads = expectedCompares * 2 + expectedSwaps * 2;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(expectedSwaps);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
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
        BubbleSortNonOptimized.Sort(random.AsSpan(), stats);

        // Bubble Sort always performs n(n-1)/2 comparisons regardless of input
        // For random data, swap count varies based on the number of inversions
        // - Best case: 0 swaps (already sorted by chance)
        // - Average case: n(n-1)/4 swaps (approximately half of comparisons result in swaps)
        // - Worst case: n(n-1)/2 swaps (reversed)
        var expectedCompares = (ulong)(n * (n - 1) / 2);
        var minSwaps = 0UL;
        var maxSwaps = (ulong)(n * (n - 1) / 2);

        // Each comparison reads 2 elements
        var minReads = expectedCompares * 2;
        // Maximum reads occur when all comparisons result in swaps
        var maxReads = expectedCompares * 2 + maxSwaps * 2;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);
        await Assert.That(stats.IndexReadCount).IsBetween(minReads, maxReads);

        // Writes = 2 * swaps (each swap writes 2 elements)
        var expectedMinWrites = minSwaps * 2;
        var expectedMaxWrites = maxSwaps * 2;
        await Assert.That(stats.IndexWriteCount).IsBetween(expectedMinWrites, expectedMaxWrites);
    }
}
