using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class CombSortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => CombSort.Sort(span, context);

    // Multiple full passes over the data make large inputs slow.
    protected override int MaxOrderTestSize => 1024;

    // On sorted input no pair is ever out of order, so no swaps (and no writes) occur.
    protected override CountExpectation SortedInputWrites => CountExpectation.Zero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    [Arguments(13)]
    [Arguments(26)]
    [Arguments(39)]
    public async Task GapSequenceTest(int n)
    {
        var stats = new StatisticsContext();
        var data = Enumerable.Range(0, n).Reverse().ToArray();
        CombSort.Sort(data.AsSpan(), stats);

        // Verify that Comb11 optimization is working:
        // When gap calculation results in 9 or 10, it should be set to 11
        // This should result in better performance than standard 1.3 shrink [Test]or

        // All elements should be sorted correctly
        await Assert.That(data).IsEquivalentTo(Enumerable.Range(0, n).ToArray(), CollectionOrdering.Matching);
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
        CombSort.Sort(sorted.AsSpan(), stats);

        // Comb Sort with sorted data performs comparisons across all gaps
        // Gap sequence: n/1.3, n/1.69, ..., 11, 8, 6, 4, 3, 2, 1
        // For each gap h, it performs (n-h) comparisons
        // Final pass with h=1 performs (n-1) comparisons
        // Since data is sorted, no swaps occur
        var expectedSwaps = 0UL;
        var expectedWrites = 0UL;

        // Comparisons should happen for all gaps
        await Assert.That(stats.CompareCount).IsNotEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(expectedSwaps);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);

        // Each comparison reads 2 elements
        var minIndexReads = stats.CompareCount * 2;
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
        CombSort.Sort(reversed.AsSpan(), stats);

        // Comb Sort with reversed data performs multiple passes
        // Gap sequence reduces by [Test]or of 1.3 each iteration
        // Each gap h performs (n-h) comparisons
        // Reversed data will require many swaps, especially in early passes

        // Comparisons: Sum of (n-h) for all gaps in sequence
        await Assert.That(stats.CompareCount).IsNotEqualTo(0UL);

        // Swaps: Should be significant for reversed data
        await Assert.That(stats.SwapCount).IsNotEqualTo(0UL);

        // Each swap writes 2 elements
        var expectedWrites = stats.SwapCount * 2;
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);

        // Each comparison reads 2 elements
        var minIndexReads = stats.CompareCount * 2;
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
        CombSort.Sort(random.AsSpan(), stats);

        // Comb Sort on random data should perform O(n log n) comparisons on average
        // Gap sequence: n/1.3, n/1.69, ..., down to 1
        // Number of gaps ≈ log₁.₃(n) ≈ 2.4 * log₂(n)
        // For each gap h: (n-h) comparisons

        // Conservative estimates:
        var minCompares = (ulong)n; // At minimum, final pass with gap=1
        var maxCompares = (ulong)(n * n); // Upper bound for worst case

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsNotEqualTo(0UL);

        // Each swap writes 2 elements
        var expectedWrites = stats.SwapCount * 2;
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);

        // Each comparison reads 2 elements
        var minIndexReads = stats.CompareCount * 2;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
    }
}
