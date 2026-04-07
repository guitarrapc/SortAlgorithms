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

    [Test]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        FlashSort.Sort(sorted.AsSpan(), stats);

        // FlashSort on sorted data (n > InsertionSortThreshold=16, uniform distribution):
        //
        // Swaps: exactly 1 — always from s.Swap(maxIdx, 0) that moves the maximum element to
        //   index 0 to anchor the permutation cycle (counted even if maxIdx == 0).
        //
        // Comparisons: come only from insertion sort within each class (not from the permutation phase).
        //   With m=⌊0.43n⌋ classes and ~n/m ≈ 2.3 elements per class, total < n.
        //   Observations: n=20 → 17, n=50 → 38, n=100 → 72.
        //
        // Writes: initial swap (2) + permutation writes (~n-1) + insertion sort writes (~n) ≈ 2n.
        //   Observations: n=20 → 41, n=50 → 101, n=100 → 201.
        //
        // Reads: min/max scan (n) + count scan (n) + permutation + insertion sort ≥ 2n.
        //   Observations: n=20 → 122, n=50 → 306, n=100 → 612.
        await Assert.That(stats.SwapCount).IsEqualTo(1UL);
        await Assert.That(stats.CompareCount).IsBetween(0UL, (ulong)n);
        await Assert.That(stats.IndexWriteCount).IsBetween((ulong)n, (ulong)(4 * n));
        await Assert.That(stats.IndexReadCount).IsBetween((ulong)(2 * n), (ulong)(10 * n));
    }

    [Test]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesReversedTest(int n)
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        FlashSort.Sort(reversed.AsSpan(), stats);

        // FlashSort on reversed data (n > InsertionSortThreshold=16, uniform distribution):
        // Class assignment depends only on key values, not their order, so the statistical
        // profile is similar to sorted data despite the different element arrangement.
        //
        // Swaps: exactly 1 — s.Swap(maxIdx, 0); for reversed input maxIdx=0, a self-swap.
        // Comparisons: < n (from per-class insertion sort; uniform key distribution).
        //   Observations: n=20 → 15, n=50 → 34, n=100 → 65.
        // Writes: ~2n.  Observations: n=20 → 43, n=50 → 97, n=100 → 184.
        // Reads: ≥ 2n.  Observations: n=20 → 106, n=50 → 258, n=100 → 514.
        await Assert.That(stats.SwapCount).IsEqualTo(1UL);
        await Assert.That(stats.CompareCount).IsBetween(0UL, (ulong)n);
        await Assert.That(stats.IndexWriteCount).IsBetween((ulong)n, (ulong)(4 * n));
        await Assert.That(stats.IndexReadCount).IsBetween((ulong)(2 * n), (ulong)(10 * n));
    }

    [Test]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesRandomTest(int n)
    {
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        FlashSort.Sort(random.AsSpan(), stats);

        // FlashSort on random data (n > InsertionSortThreshold=16, uniform distribution):
        // Range(0, n) shuffled has the same key-value set as sorted/reversed, so class
        // distribution is identical and the statistics fall in the same range.
        //
        // Swaps: exactly 1 (initial s.Swap(maxIdx, 0)).
        // Comparisons: < n (per-class insertion sort with ~2.3 elements per class on average).
        //   Observations: n=20 → 15-16, n=50 → 35-36, n=100 → 65-68.
        // Writes: ~2n.  Observations: n=20 → 38-43, n=50 → 85-92, n=100 → 171-179.
        // Reads: ≥ 2n.  Observations: n=20 → 92-100, n=50 → 218-271, n=100 → 472-493.
        await Assert.That(stats.SwapCount).IsEqualTo(1UL);
        await Assert.That(stats.CompareCount).IsBetween(0UL, (ulong)n);
        await Assert.That(stats.IndexWriteCount).IsBetween((ulong)n, (ulong)(4 * n));
        await Assert.That(stats.IndexReadCount).IsBetween((ulong)(2 * n), (ulong)(10 * n));
    }
}
