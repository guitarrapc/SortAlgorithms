using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class GnomeSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => GnomeSort.Sort(span, context);

    // O(n^2) algorithm: keep data-driven inputs small.
    protected override int MaxOrderTestSize => 512;

    // Sorted input walks forward without ever swapping: no swaps, no writes.
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
        GnomeSort.Sort(sorted.AsSpan(), stats);

        // GnomeSort (optimized with position memory) - Sorted case:
        // Compares: n-1 (each element checked once, all in correct position)
        // Swaps: 0 (no swaps needed)
        // Writes: 0 (no swaps = no writes)
        var expectedCompares = (ulong)(n - 1);
        var expectedSwaps = 0UL;
        var expectedWrites = 0UL;

        // Each comparison reads both neighbours
        var expectedIndexReads = expectedCompares * 2;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(expectedSwaps);
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
        GnomeSort.Sort(reversed.AsSpan(), stats);

        // GnomeSort (optimized) - Reversed case (worst case):
        // Every pot is carried to the front, one swap per step: 1 + 2 + ... + (n-1) = n(n-1)/2 swaps.
        // Each step costs the one comparison that called for its swap, and no walk back ends above the front,
        // so the comparisons equal the swaps. The plain walk would compare once more per swap on the way back up.
        var expectedSwaps = (ulong)(n * (n - 1) / 2);
        var expectedCompares = (ulong)(n * (n - 1) / 2);
        var expectedWrites = (ulong)(n * (n - 1));

        // Each comparison reads both neighbours, and each swap reads its two operands
        var expectedIndexReads = expectedCompares * 2 + expectedSwaps * 2;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(expectedSwaps);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedIndexReads);
    }

    /// <summary>
    /// Position memory removes exactly the forward steps of the plain walk: one comparison for every swap, with the same swaps.
    /// </summary>
    [Test]
    [Arguments(10, 42)]
    [Arguments(50, 1234)]
    [Arguments(100, 42)]
    public async Task PositionMemorySavesOneComparisonPerSwap(int n, int seed)
    {
        var input = TestHelpers.ShuffledRange(n, seed);
        var remembered = new StatisticsContext();
        var plain = new StatisticsContext();
        GnomeSort.Sort(input.ToArray().AsSpan(), remembered);
        GnomeSortNonOptimized.Sort(input.ToArray().AsSpan(), plain);

        await Assert.That(remembered.SwapCount).IsEqualTo(plain.SwapCount);
        await Assert.That(remembered.CompareCount).IsEqualTo(plain.CompareCount - plain.SwapCount);
    }

    /// <summary>
    /// Each walk back ends at the front at the latest, so a comparer that never reports order still lets the sort finish.
    /// </summary>
    [Test]
    public async Task FinishesWhenTheComparerNeverReportsOrder()
    {
        const int n = 40;
        var stats = new StatisticsContext();
        var data = Enumerable.Range(0, n).ToArray();
        GnomeSort.Sort(data.AsSpan(), Comparer<int>.Create(static (_, _) => 1), stats);

        // Every pot is carried all the way to the front.
        await Assert.That(stats.SwapCount).IsEqualTo((ulong)(n * (n - 1) / 2));
        await Assert.That(stats.CompareCount).IsEqualTo((ulong)(n * (n - 1) / 2));
        await Assert.That(data.Order().SequenceEqual(Enumerable.Range(0, n))).IsTrue();
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
        GnomeSort.Sort(random.AsSpan(), stats);

        // GnomeSort - Random case:
        // Average comparisons: O(n^2)
        // Minimum: n-1 (best case, already sorted by chance)
        // Maximum: n(n-1)/2 (worst case, reversed - one comparison per swap)
        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(n * (n - 1) / 2);

        // Swaps vary depending on initial disorder
        var minSwaps = 0UL;
        var maxSwaps = (ulong)(n * (n - 1) / 2);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);
        await Assert.That(stats.IndexReadCount > 0).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be > 0");
    }
}
