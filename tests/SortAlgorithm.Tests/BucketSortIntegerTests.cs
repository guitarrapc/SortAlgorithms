using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class BucketSortIntegerTests : IntegerSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => BucketSortInteger.Sort(span, context);

    // Distribution + per-bucket insertion sort: compares and writes always occur; no swaps.
    protected override CountExpectation SortedInputCompares => CountExpectation.NonZero;
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
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
        BucketSortInteger.Sort(sorted.AsSpan(), stats);

        var minReads = (ulong)(2 * n);
        var expectedWrites = (ulong)(2 * n); // n (distribute to temp) + n (CopyTo to main)
        var bucketCount = Math.Max(2, Math.Min(1000, (int)Math.Sqrt(n)));
        var expectedCompares = (ulong)n * 2 - 1;

        await Assert.That(stats.IndexReadCount >= minReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minReads}");
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
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
        BucketSortInteger.Sort(reversed.AsSpan(), stats);

        var minReads = (ulong)(2 * n);
        var expectedWrites = (ulong)(2 * n); // n (distribute to temp) + n (CopyTo to main)
        var bucketCount = Math.Max(2, Math.Min(1000, (int)Math.Sqrt(n)));
        var bucketSize = n / bucketCount;
        var maxComparesPerBucket = bucketSize * (bucketSize - 1) / 2;
        var maxCompares = (ulong)(bucketCount * maxComparesPerBucket) * 2 + 1;

        await Assert.That(stats.IndexReadCount >= minReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minReads}");
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.CompareCount <= maxCompares).IsTrue().Because($"CompareCount ({stats.CompareCount}) should be <= {maxCompares * 2}");
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
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
        BucketSortInteger.Sort(random.AsSpan(), stats);

        var minReads = (ulong)(2 * n);
        var expectedWrites = (ulong)(2 * n); // n (distribute to temp) + n (CopyTo to main)
        var bucketCount = Math.Max(2, Math.Min(1000, (int)Math.Sqrt(n)));
        var minCompares = 0UL;
        var bucketSize = n / bucketCount;
        var maxComparesPerBucket = bucketSize * (bucketSize - 1) / 2;
        var maxCompares = (ulong)(bucketCount * maxComparesPerBucket) * 2 + 1;

        await Assert.That(stats.IndexReadCount >= minReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minReads}");
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesAllSameTest(int n)
    {
        var stats = new StatisticsContext();
        var allSame = Enumerable.Repeat(42, n).ToArray();
        BucketSortInteger.Sort(allSame.AsSpan(), stats);

        var expectedReads = (ulong)n + 1;
        var expectedWrites = 0UL;
        var expectedCompares = (ulong)n * 2 - 1;

        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }
}
