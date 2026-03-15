using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class SpreadSortTests
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

        SpreadSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    public async Task MinValueHandlingTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { int.MinValue, -1, 0, 1, int.MaxValue };
        SpreadSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo([int.MinValue, -1, 0, 1, int.MaxValue], CollectionOrdering.Matching);
    }

    [Test]
    public async Task SortWithNegativeNumbers()
    {
        var stats = new StatisticsContext();
        var array = new[] { -5, 3, -1, 0, 2, -3, 1 };
        var expected = new[] { -5, -3, -1, 0, 1, 2, 3 };
        SpreadSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task SortWithAllSameValues()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 5, 5, 5, 5 };
        SpreadSort.Sort(array.AsSpan(), stats);

        foreach (var item in array) await Assert.That(item).IsEqualTo(5);
    }

    [Test]
    [Arguments(typeof(byte))]
    [Arguments(typeof(sbyte))]
    [Arguments(typeof(short))]
    [Arguments(typeof(ushort))]
    [Arguments(typeof(int))]
    [Arguments(typeof(uint))]
    [Arguments(typeof(long))]
    [Arguments(typeof(ulong))]
    public async Task SortDifferentIntegerTypes(Type type)
    {
        var stats = new StatisticsContext();

        if (type == typeof(byte))
        {
            byte[] array = [200, 50, 100, 150, 0, 255, 1];
            SpreadSort.Sort(array.AsSpan(), stats);
            await Assert.That(array).IsEquivalentTo((byte[])[0, 1, 50, 100, 150, 200, 255], CollectionOrdering.Matching);
        }
        else if (type == typeof(sbyte))
        {
            sbyte[] array = [-128, 127, 0, -1, 1, 50, -50];
            SpreadSort.Sort(array.AsSpan(), stats);
            await Assert.That(array).IsEquivalentTo((sbyte[])[-128, -50, -1, 0, 1, 50, 127], CollectionOrdering.Matching);
        }
        else if (type == typeof(short))
        {
            short[] array = [-32768, 32767, 0, -1, 1, 100, -100];
            SpreadSort.Sort(array.AsSpan(), stats);
            await Assert.That(array).IsEquivalentTo((short[])[-32768, -100, -1, 0, 1, 100, 32767], CollectionOrdering.Matching);
        }
        else if (type == typeof(ushort))
        {
            ushort[] array = [65535, 0, 100, 200, 1, 50000, 30000];
            SpreadSort.Sort(array.AsSpan(), stats);
            await Assert.That(array).IsEquivalentTo((ushort[])[0, 1, 100, 200, 30000, 50000, 65535], CollectionOrdering.Matching);
        }
        else if (type == typeof(int))
        {
            int[] array = [int.MinValue, int.MaxValue, 0, -1, 1, 1000, -1000];
            SpreadSort.Sort(array.AsSpan(), stats);
            await Assert.That(array).IsEquivalentTo((int[])[int.MinValue, -1000, -1, 0, 1, 1000, int.MaxValue], CollectionOrdering.Matching);
        }
        else if (type == typeof(uint))
        {
            uint[] array = [uint.MaxValue, 0, 100, 200, 1, 500000, 300000];
            SpreadSort.Sort(array.AsSpan(), stats);
            await Assert.That(array).IsEquivalentTo((uint[])[0, 1, 100, 200, 300000, 500000, uint.MaxValue], CollectionOrdering.Matching);
        }
        else if (type == typeof(long))
        {
            long[] array = [long.MinValue, long.MaxValue, 0, -1, 1, 100000, -100000];
            SpreadSort.Sort(array.AsSpan(), stats);
            await Assert.That(array).IsEquivalentTo((long[])[long.MinValue, -100000, -1, 0, 1, 100000, long.MaxValue], CollectionOrdering.Matching);
        }
        else if (type == typeof(ulong))
        {
            ulong[] array = [ulong.MaxValue, 0, 100, 200, 1, 500000, 300000];
            SpreadSort.Sort(array.AsSpan(), stats);
            await Assert.That(array).IsEquivalentTo((ulong[])[0, 1, 100, 200, 300000, 500000, ulong.MaxValue], CollectionOrdering.Matching);
        }
    }
}
