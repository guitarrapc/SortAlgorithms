using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class CircleSortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => CircleSort.Sort(span, context);

    // Repeated O(n log n) passes until swap-free make large inputs slow.
    protected override int MaxOrderTestSize => 1024;

    // On sorted input the first pass finds no out-of-order pair, so no swaps (and no writes) occur.
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
        CircleSort.Sort(sorted.AsSpan(), stats);

        // Circle Sort on sorted data terminates after one outer pass (no swaps occur)
        // One pass performs T(n) ≈ (n/2) * log₂(n) comparisons via the recursive halving structure
        // Since the data is sorted, no swaps occur
        var minCompares = (ulong)(n / 2);
        var maxCompares = (ulong)(n * Math.Log(n, 2) * 2);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);

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
        CircleSort.Sort(reversed.AsSpan(), stats);

        // Circle Sort on reversed data requires multiple outer passes
        // Overall complexity is O(n log n log n), so comparisons exceed a single pass
        var minCompares = (ulong)(n * Math.Log(n, 2) / 2);
        var maxCompares = (ulong)(n * n / 2);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
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
        CircleSort.Sort(random.AsSpan(), stats);

        // Circle Sort on random data performs O(n log n log n) comparisons on average
        // At minimum one outer pass completes, contributing at least n/2 comparisons
        var minCompares = (ulong)(n / 2);
        var maxCompares = (ulong)(n * n);

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
