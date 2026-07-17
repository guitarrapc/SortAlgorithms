using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class SpinSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => SpinSort.Sort(span, context);

    // Sorted input is detected as pre-sorted: no writes, no swaps.
    protected override CountExpectation SortedInputWrites => CountExpectation.Zero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(10)]
    [Arguments(36)]
    [Arguments(72)]
    [Arguments(73)]
    [Arguments(100)]
    [Arguments(200)]
    [Arguments(500)]
    [Arguments(1000)]
    [Arguments(2000)]
    public async Task SortSmallAndBoundaryTest(int n)
    {
        var stats = new StatisticsContext();
        var rng = new Random(42);
        var array = Enumerable.Range(0, n).Select(_ => rng.Next(0, n)).ToArray();
        var expected = array.ToArray();
        Array.Sort(expected);

        SpinSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
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
        SpinSort.Sort(sorted.AsSpan(), stats);

        // Loose bounds: a comparison sort needs at least n - 1 comparisons to
        // establish order, and SpinSort stays far below the quadratic ceiling.
        await Assert.That(stats.CompareCount).IsBetween((ulong)(n - 1), (ulong)(n * n));
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
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
        SpinSort.Sort(reversed.AsSpan(), stats);

        await Assert.That(stats.CompareCount).IsBetween((ulong)(n - 1), (ulong)(n * n));
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsNotEqualTo(0UL);
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
        SpinSort.Sort(random.AsSpan(), stats);

        await Assert.That(stats.CompareCount).IsBetween((ulong)(n - 1), (ulong)(n * n));
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsNotEqualTo(0UL);
        await Assert.That(random).IsEquivalentTo(Enumerable.Range(0, n).ToArray(), CollectionOrdering.Matching);
    }
}
