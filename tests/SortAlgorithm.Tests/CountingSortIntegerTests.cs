using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class CountingSortIntegerTests : IntegerSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => CountingSortInteger.Sort(span, context);

    // Throws ArgumentException on excessive key ranges (see RangeLimitTest), so full-integer-range inputs are rejected by contract.
    protected override bool SupportsFullIntegerRange => false;

    // Counting sort: no comparisons or swaps; distribute/copy-back passes always write.
    protected override CountExpectation SortedInputCompares => CountExpectation.Zero;
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    [Arguments(10_000_001)]
    public async Task RangeLimitTest(int range)
    {
        // Test that excessive range throws ArgumentException
        var array = new[] { 0, range };
        Assert.Throws<ArgumentException>(() => CountingSortInteger.Sort(array.AsSpan()));
    }

    [Test]
    [Arguments(2, 10_000)]    // range=10,001 > MaxRangeFactor*n=64,  but < MaxCountArraySize
    [Arguments(100, 5_000)]   // range=5,001  > MaxRangeFactor*n=3200, but < MaxCountArraySize
    public async Task RelativeRangeLimitTest(int n, int maxValue)
    {
        // range is well within the absolute cap but too large relative to n: O(range) would dominate O(n)
        var array = new int[n];
        array[n - 1] = maxValue;
        Assert.Throws<ArgumentException>(() => CountingSortInteger.Sort(array.AsSpan()));
    }

    [Test]
    public async Task NegativeValuesTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { -5, -1, -10, 3, 0, -3 };
        CountingSortInteger.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo([-10, -5, -3, -1, 0, 3], CollectionOrdering.Matching);
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
        CountingSortInteger.Sort(sorted.AsSpan(), stats);

        // CountingSortInteger with temp buffer tracking:
        // 1. Find min/max: n reads (s.Read), using direct operators (not tracked as comparisons)
        // 2. Count occurrences: n reads (s.Read)
        // 3. Build result in reverse: n reads (s.Read) + n writes (tempSpan.Write)
        // 4. Write back: n reads (tempSpan.Read) + n writes (s.Write)
        //  Total: 4n reads, 2n writes, 0 comparisons
        var expectedReads = (ulong)(4 * n);
        var expectedWrites = (ulong)(2 * n);
        var expectedCompare = 0UL;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompare);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
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
        CountingSortInteger.Sort(reversed.AsSpan(), stats);

        // CountingSortInteger complexity is O(n + k) regardless of input order
        // With temp buffer tracking: 4n reads, 2n writes, 0 comparisons
        var expectedReads = (ulong)(4 * n);
        var expectedWrites = (ulong)(2 * n);
        var expectedcompare = 0UL;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedcompare);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
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
        CountingSortInteger.Sort(random.AsSpan(), stats);

        // CountingSortInteger has same complexity regardless of input distribution
        // 4n reads due to temp buffer tracking, 2n writes, 0 comparisons
        var expectedReads = (ulong)(4 * n);
        var expectedWrites = (ulong)(2 * n);
        var expectedCompare = 0UL;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompare);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
    }

    [Test]
    public async Task TheoreticalValuesAllSameTest()
    {
        var stats = new StatisticsContext();
        var n = 100;
        var allSame = Enumerable.Repeat(42, n).ToArray();
        CountingSortInteger.Sort(allSame.AsSpan(), stats);

        // When all values are the same (min == max), early return after min/max scan
        // Only n reads for finding min/max (direct operators, not tracked), then early return (no writes)
        var expectedReads = (ulong)n;
        var expectedWrites = 0UL;
        var expectedCompare = 0UL;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompare);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
    }
}
