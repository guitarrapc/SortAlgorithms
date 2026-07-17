using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class PingpongMergeSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => PingpongMergeSort.Sort(span, context);

    // Pingpong MergeSort always has writes: leaf copies (n writes at base case) plus
    // skip-sorted CopyTo calls for already-ordered ranges.
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    // Merges copy through the ping-pong buffers with Read/Write; no swaps.
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
        PingpongMergeSort.Sort(sorted.AsSpan(), stats);

        // Loose bounds: a comparison sort needs at least n - 1 comparisons to
        // establish order, and PingpongMergeSort stays far below the quadratic ceiling.
        await Assert.That(stats.CompareCount).IsBetween((ulong)(n - 1), (ulong)(n * n));
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
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
        PingpongMergeSort.Sort(reversed.AsSpan(), stats);

        await Assert.That(stats.CompareCount).IsBetween((ulong)(n - 1), (ulong)(n * n));
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsNotEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
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
        PingpongMergeSort.Sort(random.AsSpan(), stats);

        await Assert.That(stats.CompareCount).IsBetween((ulong)(n - 1), (ulong)(n * n));
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsNotEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(random).IsEquivalentTo(Enumerable.Range(0, n).ToArray(), CollectionOrdering.Matching);
    }
}
