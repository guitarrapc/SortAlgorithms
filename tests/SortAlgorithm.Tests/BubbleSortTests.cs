using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class BubbleSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => BubbleSort.Sort(span, context);

    // O(n^2) algorithm: keep data-driven inputs small.
    protected override int MaxOrderTestSize => 512;

    // Early termination on sorted input: a single pass with no swaps, hence no writes.
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
        BubbleSort.Sort(sorted.AsSpan(), stats);

        // Optimized Bubble Sort with early termination and last swap position tracking
        // For sorted data, only one pass is needed with n-1 comparisons
        var expectedCompares = (ulong)(n - 1);
        var expectedSwaps = 0UL;
        var expectedWrites = 0UL; // No swaps = no writes

        // Each comparison reads 2 elements (positions j and j+1)
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
        BubbleSort.Sort(reversed.AsSpan(), stats);

        // Optimized Bubble Sort with last swap position tracking
        // For reversed array, still performs O(n²) comparisons but may reduce slightly
        // Swaps: n(n-1)/2 (every comparison results in a swap in first passes)
        var maxCompares = (ulong)(n * (n - 1) / 2);
        var expectedSwaps = (ulong)(n * (n - 1) / 2);

        // Each swap writes 2 elements (swap reads and writes both positions)
        var expectedWrites = expectedSwaps * 2;

        // Optimized version may do fewer comparisons, so check it's at most the theoretical maximum
        await Assert.That(stats.CompareCount).IsLessThanOrEqualTo(maxCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(expectedSwaps);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
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
        BubbleSort.Sort(random.AsSpan(), stats);

        // Optimized Bubble Sort with early termination and last swap position tracking
        // For random data, comparison count is reduced compared to naive implementation
        // - Maximum: n(n-1)/2 comparisons
        // - Actual: depends on data distribution and swap positions
        var maxCompares = (ulong)(n * (n - 1) / 2);
        var minSwaps = 0UL;
        var maxSwaps = (ulong)(n * (n - 1) / 2);

        await Assert.That(stats.CompareCount).IsLessThanOrEqualTo(maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);

        // Writes = 2 * swaps (each swap writes 2 elements)
        var expectedMinWrites = minSwaps * 2;
        var expectedMaxWrites = maxSwaps * 2;
        await Assert.That(stats.IndexWriteCount).IsBetween(expectedMinWrites, expectedMaxWrites);
    }
}
