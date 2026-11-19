using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class TournamentSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => TournamentSort.Sort(span, context);

    // Winners are written to the output area and copied back, so even sorted input is written, but nothing is swapped.
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        TournamentSort.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        TournamentSort.Sort(array.AsSpan(), 0, array.Length, stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortSingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9 };

        // Sort a single element range [2, 3)
        TournamentSort.Sort(array.AsSpan(), 2, 3, stats);

        // Array should be unchanged (single element is already sorted)
        await Assert.That(array).IsEquivalentTo([5, 3, 8, 1, 9], CollectionOrdering.Matching);
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
        TournamentSort.Sort(sorted.AsSpan(), stats);

        await AssertTournamentCounts(stats, n);
        await Assert.That(sorted).IsEquivalentTo(Enumerable.Range(0, n).ToArray(), CollectionOrdering.Matching);
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
        TournamentSort.Sort(reversed.AsSpan(), stats);

        await AssertTournamentCounts(stats, n);
        await Assert.That(reversed).IsEquivalentTo(Enumerable.Range(0, n).ToArray(), CollectionOrdering.Matching);
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
        TournamentSort.Sort(random.AsSpan(), stats);

        await AssertTournamentCounts(stats, n);
        await Assert.That(random).IsEquivalentTo(Enumerable.Range(0, n).ToArray(), CollectionOrdering.Matching);
    }

    /// <summary>
    /// The build is a knockout of n contestants, so it compares exactly n - 1 times. Every later round replays one
    /// leaf-to-root path, comparing at most once per level, and the last winner needs no replay.
    /// </summary>
    static async Task AssertTournamentCounts(StatisticsContext stats, int n)
    {
        var size = (int)System.Numerics.BitOperations.RoundUpToPowerOf2((uint)n);
        var levels = System.Numerics.BitOperations.Log2((uint)size);

        await Assert.That(stats.CompareCount).IsBetween((ulong)(n - 1), (ulong)(n - 1 + (n - 1) * levels));
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);

        // Tree: size leaves and size - 1 internal nodes to build, then per replay the retired leaf and one node per level.
        // Elements: one output write per winner and one write per element copied back.
        var treeWrites = 2 * size - 1 + (n - 1) * (1 + levels);
        await Assert.That(stats.IndexWriteCount).IsEqualTo((ulong)(treeWrites + 2 * n));

        // Each comparison reads both contestants; each winner is read once, and the copy back reads the output area.
        await Assert.That(stats.IndexReadCount).IsEqualTo(2 * stats.CompareCount + (ulong)(2 * n));
    }
}
