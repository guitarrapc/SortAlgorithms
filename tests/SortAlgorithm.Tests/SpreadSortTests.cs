using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class SpreadSortTests : IntegerSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => SpreadSort.Sort(span, context);

    // Delegates to PDQSort below MinSortSize (comparison-based), so compares occur even on sorted input;
    // writes/swaps vary with pattern detection.
    protected override CountExpectation SortedInputCompares => CountExpectation.NonZero;

    // SpreadSort is UNSTABLE: bucket distribution and the pdqsort fallback may reorder
    // equal keys, so keySelector tests assert key order and permutation integrity only.

    [Test]
    public async Task KeySelectorSortsByKeyTest()
    {
        // Unstable sort: assert key order only, not tie order.
        // 2000 elements exceeds MinSortSize (1000), so the spread path runs (not just the pdqsort fallback).
        var random = new Random(42);
        var records = Enumerable.Range(0, 2000).Select(i => (Key: random.Next(-10000, 10000), Index: i)).ToArray();

        SpreadSort.SortBy(records.AsSpan(), x => x.Key);

        var keys = records.Select(x => x.Key).ToArray();
        var expectedKeys = keys.OrderBy(x => x).ToArray();
        await Assert.That(keys).IsEquivalentTo(expectedKeys, CollectionOrdering.Matching);
        // All 2000 original records must still be present exactly once
        await Assert.That(records.Select(x => x.Index).OrderBy(x => x).ToArray())
            .IsEquivalentTo(Enumerable.Range(0, 2000).ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    public async Task KeySelectorSmallInputFallbackTest()
    {
        // 500 elements is below MinSortSize (1000), covering the pdqsort fallback path
        var random = new Random(42);
        var records = Enumerable.Range(0, 500).Select(i => (Key: random.Next(-10000, 10000), Index: i)).ToArray();

        SpreadSort.SortBy(records.AsSpan(), x => x.Key);

        var keys = records.Select(x => x.Key).ToArray();
        var expectedKeys = keys.OrderBy(x => x).ToArray();
        await Assert.That(keys).IsEquivalentTo(expectedKeys, CollectionOrdering.Matching);
        // All 500 original records must still be present exactly once
        await Assert.That(records.Select(x => x.Index).OrderBy(x => x).ToArray())
            .IsEquivalentTo(Enumerable.Range(0, 500).ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    public async Task KeySelectorNegativeKeysTest()
    {
        // Keys spanning negative/zero/positive; unstable sort, so assert key order only
        var records = new (int Key, string Name)[] { (3, "c"), (-5, "a"), (0, "b"), (-5, "a2"), (3, "c2"), (int.MinValue, "min"), (int.MaxValue, "max") };
        SpreadSort.SortBy(records.AsSpan(), x => x.Key);

        await Assert.That(records.Select(x => x.Key).ToArray())
            .IsEquivalentTo([int.MinValue, -5, -5, 0, 3, 3, int.MaxValue], CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateHalf))]
    public async Task SortHalfResultOrderTest(IInputSample<Half> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        SpreadSort.Sort(array.AsSpan(), stats);

        // Check is sorted (NaN-first total order, same as Array.Sort)
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateFloat))]
    public async Task SortFloatResultOrderTest(IInputSample<float> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        SpreadSort.Sort(array.AsSpan(), stats);

        // Check is sorted (NaN-first total order, same as Array.Sort)
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateDouble))]
    public async Task SortDoubleResultOrderTest(IInputSample<double> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        SpreadSort.Sort(array.AsSpan(), stats);

        // Check is sorted (NaN-first total order, same as Array.Sort)
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
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
    public async Task IntegerTypeBoundaryValuesTest(Type type)
    {
        // Kept in addition to the base SortDifferentIntegerTypes: exercises per-type
        // min/max boundary values, which stress SpreadSort's radix-shift key handling.
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

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        SpreadSort.Sort(sorted.AsSpan(), stats);

        // For n < MinSortSize (1000), SpreadSort delegates entirely to PDQSort.
        // PDQSort detects sorted input via partial insertion sort optimization:
        // - Small n (10, 20): detects sorted in a single pass → n-1 comparisons
        // - Larger n (50, 100): one partition attempt + sorted detection → ~2n comparisons
        // For n >= MinSortSize, IsSortedOrFindExtremes detects sorted in n-1 comparisons.
        var logN = Math.Log(n + 1, 2);
        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(n * logN * 1.5 + n);

        // Sorted arrays should have very few swaps (0 or 1 from pivot placement)
        var maxSwaps = (ulong)(n * 0.5);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(0UL, maxSwaps);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
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
        SpreadSort.Sort(reversed.AsSpan(), stats);

        // For n < MinSortSize, SpreadSort delegates to PDQSort.
        // PDQSort handles reverse-sorted input via partitioning and insertion sort.
        // Reverse-sorted input causes more work than sorted but is still detected
        // as a pattern by PDQSort's adaptive mechanisms.
        var logN = Math.Log(n + 1, 2);
        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(n * logN * 3.0 + n);

        var maxSwaps = (ulong)(n * logN);

        var minWrites = 0UL;
        var maxWrites = (ulong)(n * logN * 3.0);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(0UL, maxSwaps);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
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
        SpreadSort.Sort(random.AsSpan(), stats);

        // For n < MinSortSize, SpreadSort delegates to PDQSort.
        // PDQSort on random data achieves O(n log n) average case.
        // Reads and writes scale with comparisons and swaps.
        var logN = Math.Log(n + 1, 2);
        var minCompares = 0UL;
        var maxCompares = (ulong)(n * logN * 3.0 + n);

        var maxSwaps = (ulong)(n * logN * 1.5);
        var maxWrites = (ulong)(n * logN * 4.0);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(0UL, maxSwaps);
        await Assert.That(stats.IndexWriteCount).IsBetween(0UL, maxWrites);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
    }

    [Test]
    public async Task TheoreticalValuesAllSameTest()
    {
        var stats = new StatisticsContext();
        var n = 100;
        var allSame = Enumerable.Repeat(42, n).ToArray();
        SpreadSort.Sort(allSame.AsSpan(), stats);

        // For n < MinSortSize, SpreadSort delegates to PDQSort.
        // PDQSort detects all-equal elements through partition_left optimization,
        // achieving near-linear behavior with ~2n comparisons.
        var logN = Math.Log(n + 1, 2);
        var maxCompares = (ulong)(n * logN * 2.0 + n);

        var maxSwaps = (ulong)n;
        var maxWrites = (ulong)(n * 2.5);

        await Assert.That(stats.CompareCount).IsBetween(0UL, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(0UL, maxSwaps);
        await Assert.That(stats.IndexWriteCount).IsBetween(0UL, maxWrites);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
    }
}
