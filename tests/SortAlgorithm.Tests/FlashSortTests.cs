using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class FlashSortTests
{
    [Test]
    [MethodDataSource(typeof(MockRandomData), nameof(MockRandomData.Generate))]
    [MethodDataSource(typeof(MockNegativePositiveRandomData), nameof(MockNegativePositiveRandomData.Generate))]
    [MethodDataSource(typeof(MockNegativeRandomData), nameof(MockNegativeRandomData.Generate))]
    [MethodDataSource(typeof(MockReversedData), nameof(MockReversedData.Generate))]
    [MethodDataSource(typeof(MockReversedWithDuplicatesData), nameof(MockReversedWithDuplicatesData.Generate))]
    [MethodDataSource(typeof(MockPipeorganData), nameof(MockPipeorganData.Generate))]
    [MethodDataSource(typeof(MockNearlySortedData), nameof(MockNearlySortedData.Generate))]
    [MethodDataSource(typeof(MockAllSameData), nameof(MockAllSameData.Generate))]
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

        FlashSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    public async Task SortWithNegativeNumbers()
    {
        var stats = new StatisticsContext();
        var array = new[] { -5, 3, -1, 0, 2, -3, 1 };
        var expected = new[] { -5, -3, -1, 0, 1, 2, 3 };
        FlashSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task SortWithAllSameValues()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 5, 5, 5, 5 };
        FlashSort.Sort(array.AsSpan(), stats);

        foreach (var item in array)
            await Assert.That(item).IsEqualTo(5);
    }

    [Test]
    public async Task MinValueHandlingTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { int.MinValue, -1, 0, 1, int.MaxValue };
        FlashSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo([int.MinValue, -1, 0, 1, int.MaxValue], CollectionOrdering.Matching);
    }

    [Test]
    public async Task SortTwoElements()
    {
        var stats = new StatisticsContext();
        var array = new[] { 2, 1 };
        FlashSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo([1, 2], CollectionOrdering.Matching);
    }

    [Test]
    public async Task SortAlreadySortedArray()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        FlashSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9, 10], CollectionOrdering.Matching);
    }

    [Test]
    public async Task SortReversedArray()
    {
        var stats = new StatisticsContext();
        var array = new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };
        FlashSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9, 10], CollectionOrdering.Matching);
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
    [Arguments(typeof(nint))]
    [Arguments(typeof(nuint))]
    public async Task SortDifferentIntegerTypes(Type type)
    {
        var stats = new StatisticsContext();

        if (type == typeof(byte))
        {
            var array = new byte[] { 5, 2, 8, 1, 9 };
            FlashSort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(sbyte))
        {
            var array = new sbyte[] { -5, 2, -8, 1, 9 };
            FlashSort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(short))
        {
            var array = new short[] { -5, 2, -8, 1, 9 };
            FlashSort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(ushort))
        {
            var array = new ushort[] { 5, 2, 8, 1, 9 };
            FlashSort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(int))
        {
            var array = new int[] { -5, 2, -8, 1, 9 };
            FlashSort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(uint))
        {
            var array = new uint[] { 5, 2, 8, 1, 9 };
            FlashSort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(long))
        {
            var array = new long[] { -5, 2, -8, 1, 9 };
            FlashSort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(ulong))
        {
            var array = new ulong[] { 5, 2, 8, 1, 9 };
            FlashSort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(nint))
        {
            var array = new nint[] { -5, 2, -8, 1, 9 };
            FlashSort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(nuint))
        {
            var array = new nuint[] { 5, 2, 8, 1, 9 };
            FlashSort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
    }

    private static bool IsSorted<T>(T[] array) where T : IComparable<T>
    {
        for (var i = 1; i < array.Length; i++)
            if (array[i - 1].CompareTo(array[i]) > 0)
                return false;
        return true;
    }
}
