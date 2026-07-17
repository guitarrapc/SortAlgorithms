using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class CocktailShakerSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => CocktailShakerSort.Sort(span, context);

    // O(n^2) algorithm: keep data-driven inputs small.
    protected override int MaxOrderTestSize => 512;

    // Sorted input terminates after the first forward pass with no swaps, hence no writes.
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
        CocktailShakerSort.Sort(sorted.AsSpan(), stats);

        // Cocktail Shaker Sort (Optimized) - Sorted case:
        // First forward pass: n-1 comparisons, 0 swaps
        // Since lastSwapIndex remains at min, max becomes min and loop exits
        // Total: n-1 comparisons, 0 swaps, 0 writes
        var expectedCompares = (ulong)(n - 1);
        var expectedSwaps = 0UL;
        var expectedWrites = 0UL;

        // Each comparison reads 2 elements
        // No swaps, so reads only from comparisons
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
        CocktailShakerSort.Sort(reversed.AsSpan(), stats);

        // Cocktail Shaker Sort - Reversed case (worst case):
        // Same as bubble sort: n(n-1)/2 comparisons and swaps
        // Each swap writes 2 elements
        var expectedCompares = (ulong)(n * (n - 1) / 2);
        var expectedSwaps = (ulong)(n * (n - 1) / 2);
        var expectedWrites = expectedSwaps * 2;

        // Each comparison reads 2 elements: Compare(i, j) reads i and j
        // Each swap also reads 2 elements: Swap(i, j) reads i and j before writing
        // Total reads = (compares * 2) + (swaps * 2)
        var expectedReads = (expectedCompares * 2) + (expectedSwaps * 2);

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
        CocktailShakerSort.Sort(random.AsSpan(), stats);

        // Cocktail Shaker Sort - Random case:
        // Comparisons: O(n²), typically less than worst case due to optimization
        // Swaps: Average n(n-1)/4 for random data
        var minCompares = (ulong)(n - 1);  // Best case (already sorted)
        var maxCompares = (ulong)(n * (n - 1) / 2);  // Worst case
        var maxSwaps = (ulong)(n * (n - 1) / 2);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(0UL, maxSwaps);

        // IndexReadCount = (CompareCount * 2) + (SwapCount * 2)
        // Because both Compare and Swap read 2 elements each
        var expectedReads = (stats.CompareCount * 2) + (stats.SwapCount * 2);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(stats.SwapCount * 2);
    }
}
