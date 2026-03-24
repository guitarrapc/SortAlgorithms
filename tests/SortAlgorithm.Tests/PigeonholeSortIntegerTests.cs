using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class PigeonholeSortIntegerTests
{
    [Test]
    [MethodDataSource(typeof(MockRandomData), nameof(MockRandomData.Generate))]
    [MethodDataSource(typeof(MockNegativePositiveRandomData), nameof(MockNegativePositiveRandomData.Generate))]
    [MethodDataSource(typeof(MockNegativeRandomData), nameof(MockNegativeRandomData.Generate))]
    [MethodDataSource(typeof(MockReversedData), nameof(MockReversedData.Generate))]
    [MethodDataSource(typeof(MockPipeorganData), nameof(MockPipeorganData.Generate))]
    [MethodDataSource(typeof(MockNearlySortedData), nameof(MockNearlySortedData.Generate))]
    [MethodDataSource(typeof(MockSameValuesData), nameof(MockSameValuesData.Generate))]
    [MethodDataSource(typeof(MockQuickSortWorstCaseData), nameof(MockQuickSortWorstCaseData.Generate))]
    [MethodDataSource(typeof(MockTwoDistinctValuesData), nameof(MockTwoDistinctValuesData.Generate))]
    [MethodDataSource(typeof(MockHalfZeroHalfOneData), nameof(MockHalfZeroHalfOneData.Generate))]
    [MethodDataSource(typeof(MockValleyRandomData), nameof(MockValleyRandomData.Generate))]
    [MethodDataSource(typeof(MockHighlySkewedData), nameof(MockHighlySkewedData.Generate))]
    public async Task SortResultOrderTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();


        PigeonholeSortInteger.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [Arguments(10_000_001)]
    public async Task RangeLimitTest(int range)
    {
        // Test that excessive range throws ArgumentException
        var array = new[] { 0, range };
        Assert.Throws<ArgumentException>(() => PigeonholeSortInteger.Sort(array.AsSpan()));
    }

    [Test]
    public async Task NegativeValuesTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { -5, -1, -10, 3, 0, -3 };
        PigeonholeSortInteger.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo([-10, -5, -3, -1, 0, 3], CollectionOrdering.Matching);
    }

    [Test]
    public async Task ULongLargeValuesTest()
    {
        // ulong values near ulong.MaxValue were broken by ConvertToLong (CreateTruncating -> negative long)
        var stats = new StatisticsContext();
        var array = new ulong[] { ulong.MaxValue, ulong.MaxValue - 2, ulong.MaxValue - 1, ulong.MaxValue - 4 };
        PigeonholeSortInteger.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(new ulong[] { ulong.MaxValue - 4, ulong.MaxValue - 2, ulong.MaxValue - 1, ulong.MaxValue }, CollectionOrdering.Matching);
    }

    [Test]
    public async Task UIntLargeValuesTest()
    {
        // uint values in the upper half of the range (> int.MaxValue) were broken by ConvertToLong
        var stats = new StatisticsContext();
        var array = new uint[] { uint.MaxValue, uint.MaxValue - 2, uint.MaxValue - 1, uint.MaxValue - 4 };
        PigeonholeSortInteger.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(new uint[] { uint.MaxValue - 4, uint.MaxValue - 2, uint.MaxValue - 1, uint.MaxValue }, CollectionOrdering.Matching);
    }

    [Test]
    public async Task EmptyArrayTest()
    {
        var stats = new StatisticsContext();
        var array = Array.Empty<int>();
        PigeonholeSortInteger.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEmpty();
    }

    [Test]
    public async Task SingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 42 };
        PigeonholeSortInteger.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsSingleElement();
        await Assert.That(array[0]).IsEqualTo(42);
    }

    [Test]
    [Arguments(2)]
    [Arguments(5)]
    [Arguments(10)]
    public async Task DuplicateValuesTest(int duplicateCount)
    {
        var stats = new StatisticsContext();
        var array = Enumerable.Repeat(5, duplicateCount).Concat(Enumerable.Repeat(3, duplicateCount)).ToArray();
        PigeonholeSortInteger.Sort(array.AsSpan(), stats);

        var expected = Enumerable.Repeat(3, duplicateCount).Concat(Enumerable.Repeat(5, duplicateCount)).ToArray();
        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }


    [Test]
    [MethodDataSource(typeof(MockSortedData), nameof(MockSortedData.Generate))]
    public async Task StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        PigeonholeSortInteger.Sort(array.AsSpan(), stats);

        var expectedCompare = (ulong)inputSample.Samples.Length * 2 + 1;

        await Assert.That((ulong)array.Length).IsEqualTo((ulong)inputSample.Samples.Length);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsNotEqualTo(0UL);
        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompare);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
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
        PigeonholeSortInteger.Sort(sorted.AsSpan(), stats);

        // Pigeonhole Sort with internal buffer tracking (via SortSpan):
        // 1. Find min/max: n reads (main buffer)
        // 2. Copy to temp and count: n reads (main) + n writes (temp)
        // 3. Calculate positions: 0 reads/writes (transform holes array in-place)
        // 4. Place elements: n reads (temp) + n writes (main)
        //
        // Total reads: n + n + n = 3n
        // Total writes: n + n = 2n
        var expectedReads = (ulong)(3 * n);
        var expectedWrites = (ulong)(2 * n);
        var expectedCompares = (ulong)(2 * n) + 1;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
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
        PigeonholeSortInteger.Sort(reversed.AsSpan(), stats);

        // Pigeonhole Sort complexity is O(n + k) regardless of input order
        // Same operation counts for reversed as for sorted (with internal buffer tracking)
        var expectedReads = (ulong)(3 * n);
        var expectedWrites = (ulong)(2 * n);
        var expectedCompares = (ulong)(2 * n) + 1;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
    }

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesRandomTest(int n)
    {
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        PigeonholeSortInteger.Sort(random.AsSpan(), stats);

        // Pigeonhole Sort has same complexity regardless of input distribution
        // With internal buffer tracking: 3n reads, 2n writes
        var expectedReads = (ulong)(3 * n);
        var expectedWrites = (ulong)(2 * n);
        var expectedCompares = (ulong)(2 * n) + 1;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
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
        PigeonholeSortInteger.Sort(allSame.AsSpan(), stats);

        // When all values are the same (min == max), early return after min/max scan
        // Only n reads for finding min/max, then early return
        var expectedReads = (ulong)n;
        var expectedWrites = 0UL;
        var expectedCompares = (ulong)n * 2 + 1;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
    }

}
