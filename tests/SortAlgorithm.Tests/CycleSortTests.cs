using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class CycleSortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => CycleSort.Sort(span, context);

    // O(n²) FindPosition scans make large inputs slow.
    protected override int MaxOrderTestSize => 512;

    // On sorted input every element is already at its cycle position, so no writes occur.
    protected override CountExpectation SortedInputWrites => CountExpectation.Zero;
    // CycleSort moves elements via writes, never swaps.
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
        CycleSort.Sort(sorted.AsSpan(), stats);

        // Each start counts once over the rest of the range and finds its item in place,
        // so nothing is skipped or written. Each start also reads its item.
        var findPositionCompares = (ulong)(n * (n - 1) / 2);

        await Assert.That(stats.CompareCount).IsEqualTo(findPositionCompares);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexReadCount).IsEqualTo((ulong)(n - 1) + findPositionCompares);
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
        CycleSort.Sort(reversed.AsSpan(), stats);

        // Reversed input is k = ⌊n/2⌋ two-element cycles, the one from start i swapping i and n-1-i.
        // Each start counts over the rest of the range; a two-element cycle counts once more for its second item.
        // Each of its two placements ends its duplicate skip with one comparison and overwrites one cell.
        var k = n / 2;
        var counts = (ulong)(n * (n - 1) / 2 + k * (n - 1) - k * (k - 1) / 2);
        var placements = (ulong)(2 * k);

        await Assert.That(stats.CompareCount).IsEqualTo(counts + placements);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(placements);
        // Item read per start, one read per counted element, skip read and overwrite read per placement.
        await Assert.That(stats.IndexReadCount).IsEqualTo((ulong)(n - 1) + counts + 2 * placements);
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
        CycleSort.Sort(random.AsSpan(), stats);

        // Cycle Sort performs FindPosition comparisons: n(n-1)/2 as the base
        var findPositionCompares = (ulong)(n * (n - 1) / 2);

        // For random data, the actual number of comparisons varies significantly
        // based on the random arrangement and resulting cycle lengths.
        // FindPosition is called multiple times per cycle (once initially, then
        // repeatedly in the while loop until the cycle completes).
        // Additionally, SkipDuplicates is called for each position found.
        //
        // Empirical observations show random data can require 2-3x the base comparisons.
        // We use a generous range to account for different random arrangements.
        var minCompares = findPositionCompares;
        var maxCompares = findPositionCompares * 4; // Allow up to 4x for random variation

        // Random data: most elements need to be moved
        // Typically between n/2 and n writes
        var minWrites = (ulong)(n / 2);
        var maxWrites = (ulong)n;

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount >= minCompares).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minCompares}");
    }
}
