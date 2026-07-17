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

        // Cycle Sort performs FindPosition comparisons: n(n-1)/2
        // For sorted data, SkipDuplicates is called but no actual duplicates exist,
        // so it performs minimal additional comparisons (1 per call to verify no match)
        var findPositionCompares = (ulong)(n * (n - 1) / 2);

        // Sorted data: no writes needed (all elements already in correct positions)
        var expectedWrites = 0UL;

        // For sorted data, FindPosition is called n-1 times (once per cycleStart)
        // Each call results in pos == cycleStart, so no SkipDuplicates is invoked
        var expectedCompares = findPositionCompares;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.IndexReadCount >= findPositionCompares).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {findPositionCompares}");
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

        // Cycle Sort performs FindPosition comparisons: n(n-1)/2 as the base
        var findPositionCompares = (ulong)(n * (n - 1) / 2);

        // However, FindPosition is called multiple times per cycle:
        // 1. Once before the initial write
        // 2. Multiple times in the while loop until the cycle completes
        //
        // For reversed data, the actual number of comparisons is approximately
        // 2x the base due to cycle rotations and SkipDuplicates calls.
        // Each element that moves requires additional FindPosition calls within its cycle.
        //
        // Empirical observations:
        // - n=10: ~90 comparisons (2.0x base)
        // - n=20: ~355 comparisons (1.87x base)
        // - n=50: ~2200 comparisons (1.80x base)
        //
        // We use a range to accommodate variations in cycle lengths.
        var minCompares = findPositionCompares;
        var maxCompares = findPositionCompares * 3; // Allow up to 3x for safety

        // Reversed data: each element needs to be moved to its correct position
        // In the worst case (reversed), n-1 elements need to be written
        var minWrites = (ulong)(n - 1);
        var maxWrites = (ulong)n;

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount >= minCompares).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minCompares}");
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
