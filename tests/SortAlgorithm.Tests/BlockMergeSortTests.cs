using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class BlockMergeSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => BlockMergeSort.Sort(span, context);

    // No write/swap knob overrides: the old statistics test only asserted reads and compares.

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        BlockMergeSort.Sort(sorted.AsSpan(), stats);
        await Assert.That(stats.CompareCount).IsEqualTo((ulong)(n switch { 10 => 20, 20 => 40, 50 => 114, _ => 228 }));
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
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
        BlockMergeSort.Sort(reversed.AsSpan(), stats);
        var (expectedC, expectedW, expectedS) = n switch
        {
            10 => (19UL, 50UL, 25UL),
            20 => (39UL, 104UL, 32UL),
            50 => (111UL, 358UL, 129UL),
            _ => (223UL, 720UL, 160UL),
        };
        await Assert.That(stats.CompareCount).IsEqualTo(expectedC);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedW);
        await Assert.That(stats.SwapCount).IsEqualTo(expectedS);
    }

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesRandomTest(int n)
    {
        var stats = new StatisticsContext();
        var rng = new Random(42 + n);
        var random = Enumerable.Range(0, n).OrderBy(_ => rng.Next()).ToArray();
        BlockMergeSort.Sort(random.AsSpan(), stats);
        // The lower bound is n - 1 (the comparison-sort minimum), not an average-case
        // n * log2(n) estimate: run detection lets a shuffle containing long sorted or
        // reversed runs finish with far fewer comparisons (e.g. 19 for reversed n=10).
        // The RNG is seeded so any future failure is reproducible.
        var logN = Math.Log2(n);
        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(n * logN * 2.5);
        var minWrites = 0UL;
        var maxWrites = (ulong)(n * logN * 5.0);
        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount > 0).IsTrue();
    }

    [Test]
    [Arguments(10, 20)]
    [Arguments(20, 40)]
    [Arguments(50, 114)]
    [Arguments(100, 228)]
    public async Task TheoreticalComparisonCountTest(int n, int expectedComparisons)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        BlockMergeSort.Sort(sorted.AsSpan(), stats);
        await Assert.That(stats.CompareCount).IsEqualTo((ulong)expectedComparisons);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
    }
}
