using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class PingpongMergeSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => PingpongMergeSort.Sort(span, context);

    // Pingpong MergeSort always has writes: the bootstrap copy into the buffer plus
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

    [Test]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(7)]
    [Arguments(64)]
    public async Task LeavesCopyNothingTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        PingpongMergeSort.Sort(sorted.AsSpan(), stats);

        // The bootstrap puts the input in both buffers, so a leaf finds its element already in place.
        // On sorted input every merge is skipped and copies its range instead, so only the bootstrap
        // and the n - 1 merge nodes move anything.
        var expectedWrites = 0UL;
        if (n > 1)
        {
            expectedWrites = (ulong)n + CopiedBySkippedMerges(0, n - 1);
        }

        await Assert.That(stats.CompareCount).IsEqualTo((ulong)Math.Max(0, n - 1));
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedWrites + 2UL * stats.CompareCount);

        static ulong CopiedBySkippedMerges(int left, int right)
        {
            if (left == right) return 0;
            var mid = left + (right - left) / 2;
            return (ulong)(right - left + 1) + CopiedBySkippedMerges(left, mid) + CopiedBySkippedMerges(mid + 1, right);
        }
    }

    [Test]
    public async Task MergeReadsEachHeadOnceTest()
    {
        var stats = new StatisticsContext();
        int[] data = [0, 2, 1, 3];
        PingpongMergeSort.Sort(data.AsSpan(), stats);

        // The bootstrap copies the input to the buffer (4 reads, 4 writes). [0, 2] and [1, 3] pass their skip checks
        // (2 comparisons, 4 reads) and are copied to the buffer (4 reads, 4 writes). The top check fails (1, 2).
        // The merge reads both heads (2), then takes 0, 1 and 2, reading only the replacement head after the first two
        // (2 reads, 3 comparisons, 3 writes), and copies the remaining 3 (1 read, 1 write).
        await Assert.That(data).IsEquivalentTo([0, 1, 2, 3], CollectionOrdering.Matching);
        await Assert.That(stats.CompareCount).IsEqualTo(6UL);
        await Assert.That(stats.IndexReadCount).IsEqualTo(19UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(12UL);
    }
}
