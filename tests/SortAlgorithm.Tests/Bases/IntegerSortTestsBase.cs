using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using System.Numerics;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

/// <summary>
/// Standard test suite for integer-key distribution sorts (radix, counting,
/// pigeonhole, bucket, flash, spread). Derive, add <c>[InheritsTests]</c>, and
/// implement <see cref="Sort{T, TContext}"/>.
/// Stability is not observable through the integer-only API (equal keys are
/// indistinguishable), so this base provides <see cref="SortIdempotencyTest"/>
/// instead of stability tests.
/// </summary>
public abstract class IntegerSortTestsBase
{
    /// <summary>Invokes the sort under test.</summary>
    protected abstract void Sort<T, TContext>(Span<T> span, TContext context)
        where T : IBinaryInteger<T>, IMinMaxValue<T>
        where TContext : ISortContext;

    /// <summary>
    /// False for range-limited algorithms (e.g. counting/pigeonhole variants that
    /// throw when the key range is too large); skips <see cref="MinValueHandlingTest"/>.
    /// </summary>
    protected virtual bool SupportsFullIntegerRange => true;

    /// <summary>Expected CompareCount after sorting already-sorted input.</summary>
    protected virtual CountExpectation SortedInputCompares => CountExpectation.Any;

    /// <summary>Expected IndexWriteCount after sorting already-sorted input.</summary>
    protected virtual CountExpectation SortedInputWrites => CountExpectation.Any;

    /// <summary>Expected SwapCount after sorting already-sorted input.</summary>
    protected virtual CountExpectation SortedInputSwaps => CountExpectation.Any;

    [Test]
    [MethodDataSource(typeof(MockStandardData), nameof(MockStandardData.Generate))]
    public async Task SortResultOrderTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    public async Task EdgeCaseEmptyArrayTest()
    {
        var stats = new StatisticsContext();
        var empty = Array.Empty<int>();
        Sort(empty.AsSpan(), stats);
    }

    [Test]
    public async Task EdgeCaseSingleElementTest()
    {
        var stats = new StatisticsContext();
        var single = new[] { 42 };
        Sort(single.AsSpan(), stats);

        await Assert.That(single[0]).IsEqualTo(42);
    }

    [Test]
    public async Task EdgeCaseTwoElementsSortedTest()
    {
        var stats = new StatisticsContext();
        var twoSorted = new[] { 1, 2 };
        Sort(twoSorted.AsSpan(), stats);

        await Assert.That(twoSorted).IsEquivalentTo([1, 2], CollectionOrdering.Matching);
    }

    [Test]
    public async Task EdgeCaseTwoElementsReversedTest()
    {
        var stats = new StatisticsContext();
        var twoReversed = new[] { 2, 1 };
        Sort(twoReversed.AsSpan(), stats);

        await Assert.That(twoReversed).IsEquivalentTo([1, 2], CollectionOrdering.Matching);
    }

    [Test]
    public async Task EdgeCaseThreeElementsTest()
    {
        var stats = new StatisticsContext();
        var three = new[] { 3, 1, 2 };
        Sort(three.AsSpan(), stats);

        await Assert.That(three).IsEquivalentTo([1, 2, 3], CollectionOrdering.Matching);
    }

    [Test]
    public async Task SortIdempotencyTest()
    {
        // NOTE: stability is not observable through the integer-only API - equal keys
        // are indistinguishable, so no test can detect equal-key reordering here.
        // This only verifies that re-sorting already-sorted data leaves it unchanged.
        var firstSort = new[] { 5, 3, 5, 3, 5 };
        Sort(firstSort.AsSpan(), NullContext.Default);

        var secondSort = firstSort.ToArray();
        Sort(secondSort.AsSpan(), NullContext.Default);

        await Assert.That(secondSort).IsEquivalentTo(firstSort, CollectionOrdering.Matching);
    }

    [Test]
    public async Task MinValueHandlingTest()
    {
        Skip.When(!SupportsFullIntegerRange, "Algorithm rejects full-integer-range inputs by contract");

        var stats = new StatisticsContext();
        // Test that int.MinValue is handled correctly (no overflow)
        var array = new[] { int.MinValue, -1, 0, 1, int.MaxValue };
        Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo([int.MinValue, -1, 0, 1, int.MaxValue], CollectionOrdering.Matching);
    }

    [Test]
    public async Task SortWithNegativeNumbers()
    {
        var stats = new StatisticsContext();
        var array = new[] { -5, 3, -1, 0, 2, -3, 1 };
        var expected = new[] { -5, -3, -1, 0, 1, 2, 3 };
        Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task SortWithAllSameValues()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 5, 5, 5, 5 };
        Sort(array.AsSpan(), stats);

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
            var array = new byte[] { 5, 2, 8, 1, 9 };
            Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(sbyte))
        {
            var array = new sbyte[] { -5, 2, -8, 1, 9 };
            Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(short))
        {
            var array = new short[] { -5, 2, -8, 1, 9 };
            Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(ushort))
        {
            var array = new ushort[] { 5, 2, 8, 1, 9 };
            Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(int))
        {
            var array = new int[] { -5, 2, -8, 1, 9 };
            Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(uint))
        {
            var array = new uint[] { 5, 2, 8, 1, 9 };
            Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(long))
        {
            var array = new long[] { -5, 2, -8, 1, 9 };
            Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(ulong))
        {
            var array = new ulong[] { 5, 2, 8, 1, 9 };
            Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
    }

    protected static bool IsSorted<T>(T[] array) where T : IComparable<T>
    {
        for (int i = 1; i < array.Length; i++)
        {
            if (new ComparableComparer<T>().Compare(array[i - 1], array[i]) > 0)
                return false;
        }
        return true;
    }

    [Test]
    [MethodDataSource(typeof(MockSortedData), nameof(MockSortedData.Generate))]
    public async Task StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        Sort(array.AsSpan(), stats);

        await Assert.That((ulong)array.Length).IsEqualTo((ulong)inputSample.Samples.Length);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await StatAssert.Count(stats.CompareCount, SortedInputCompares, "CompareCount");
        await StatAssert.Count(stats.IndexWriteCount, SortedInputWrites, "IndexWriteCount");
        await StatAssert.Count(stats.SwapCount, SortedInputSwaps, "SwapCount");
    }
}
