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
    public async Task BinCacheCapacitySparseBinsTest()
    {
        // Regression: the bin cache used to be sized at n, on the assumption that the binCounts
        // along a root-to-leaf path sum to at most n. binCount comes from the KEY RANGE, not the
        // element count, so a level whose bins are mostly empty claims far more slots than it has
        // elements. This input drives:
        //   level 0: logRange=21 -> logDivisor=11, binCount~1023
        //           bin 0 holds 2048 elements >= get_min_count(11)=2048 -> recurse
        //   level 1: logRange=11 -> logDivisor=0,  binCount=2048
        //           cacheEnd = 1023 + 2048 = 3071 > n = 3000  -> threw ArgumentOutOfRangeException
        var values = new List<int>();
        for (var i = 0; i < 2048; i++) values.Add(i);              // all land in top-level bin 0
        for (var i = 0; i < 952; i++) values.Add(2048 + i * 2200); // sparse over the remaining bins
        var random = new Random(12345);
        var array = values.OrderBy(_ => random.Next()).ToArray();

        var expected = array.OrderBy(x => x).ToArray();
        SpreadSort.Sort(array.AsSpan());

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(3)]
    public async Task SparseValueRangeTest(int seed)
    {
        // Broader coverage of the same equivalence class: many elements clustered into a narrow
        // low range plus a thin tail spread over a wide range, so most bins stay empty at every level.
        var random = new Random(seed);
        var n = 20000;
        var array = new int[n];
        for (var i = 0; i < n; i++)
        {
            array[i] = i % 2 == 0
                ? random.Next(0, 4096)                 // dense cluster
                : random.Next(4096, int.MaxValue);     // sparse tail
        }

        var expected = array.OrderBy(x => x).ToArray();
        SpreadSort.Sort(array.AsSpan());

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    [Arguments(500, 7)]     // below MinSortSize: PDQSort.SortCore fallback
    [Arguments(2000, 7)]    // above MinSortSize: spread path
    [Arguments(1200, 2)]    // above MinSortSize, but half NaN: drops below it after partitioning
    [Arguments(1200, 1)]    // all NaN
    public async Task NaNWithDefaultContextSizesTest(int n, int nanEvery)
    {
        // Every entry path has to partition NaN: the spread path derives it from the extremes,
        // the small-input path needs an explicit pre-pass because PDQSort.SortCore (the internal
        // entry) does not perform one unlike PDQSort's public Sort, and removing the NaN values
        // can itself drop the remainder below MinSortSize.
        var random = new Random(1234);
        var array = new float[n];
        for (var i = 0; i < n; i++)
            array[i] = i % nanEvery == 0 ? float.NaN : (float)random.NextDouble() * 1000f;

        var expected = (float[])array.Clone();
        Array.Sort(expected);

        SpreadSort.Sort(array.AsSpan()); // no-context overload

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task NaNWithDefaultContextTest()
    {
        // Regression: the float overloads passed ComparableComparer, which SortSpan specializes to
        // raw IEEE 754 operators on the NullContext (no-context) fast path. NaN is unordered under
        // those operators so it was never chosen as the minimum, while its radix key is 0 - below
        // every non-NaN key - which put bin indices outside [0, binCount) and threw
        // IndexOutOfRangeException. Only reproducible via the no-context overload in Release:
        // the StatisticsContext overloads used by the other float tests take the comparer path,
        // and SortSpan's DEBUG build does too.
        var random = new Random(42);
        var floats = new float[2000];
        var doubles = new double[2000];
        var halves = new Half[2000];
        for (var i = 0; i < floats.Length; i++)
        {
            var isNaN = i % 10 == 0;
            floats[i] = isNaN ? float.NaN : (float)random.NextDouble() * 1000f;
            doubles[i] = isNaN ? double.NaN : random.NextDouble() * 1000d;
            halves[i] = isNaN ? Half.NaN : (Half)(random.NextDouble() * 100d);
        }

        var expectedFloats = (float[])floats.Clone();
        var expectedDoubles = (double[])doubles.Clone();
        var expectedHalves = (Half[])halves.Clone();
        Array.Sort(expectedFloats);
        Array.Sort(expectedDoubles);
        Array.Sort(expectedHalves);

        // No-context overloads: this is the path that used to throw.
        SpreadSort.Sort(floats.AsSpan());
        SpreadSort.Sort(doubles.AsSpan());
        SpreadSort.Sort(halves.AsSpan());

        await Assert.That(floats).IsEquivalentTo(expectedFloats, CollectionOrdering.Matching);
        await Assert.That(doubles).IsEquivalentTo(expectedDoubles, CollectionOrdering.Matching);
        await Assert.That(halves).IsEquivalentTo(expectedHalves, CollectionOrdering.Matching);
    }

    [Test]
    [Arguments(4096)]
    [Arguments(65536)]
    public async Task HalfOnePassCompletionTest(int n)
    {
        // The floating-point overloads use Boost's float_* get_min_count constants
        // (float_log_min_split_count=8, float_log_finishing_count=4), which enable the
        // one-pass-completion branch that the int_* constants deliberately disable.
        // Half is the width where that branch is actually reachable: 16-bit keys leave a
        // log_divisor of 3-4 at the recursion decision, well inside the branch's gate, so a bin
        // is finished with one more distribution pass instead of falling back to pdqsort.
        // Half also has few distinct values, so these arrays are duplicate-heavy by construction.
        var random = new Random(99);
        var array = new Half[n];
        for (var i = 0; i < n; i++) array[i] = (Half)(random.NextDouble() * 60000d);

        var expected = (Half[])array.Clone();
        Array.Sort(expected);

        SpreadSort.Sort(array.AsSpan());

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task SignedZeroTest()
    {
        // Regression: -0.0 and +0.0 compare EQUAL under both IEEE '<' and double.CompareTo, so the
        // comparison-driven minimum could land on +0.0 while -0.0 carries a strictly smaller radix
        // key. The comparison-derived minimum then failed to bound the keys and the bin index went
        // negative, throwing IndexOutOfRangeException. The extremes are now found by key.
        //
        // The two zeros are a genuine tie, so their relative order is NOT asserted: SpreadSort is
        // unstable, and Array.Sort leaves the same tie unspecified. Assert what is guaranteed.
        var random = new Random(7);
        var array = new double[2000];
        for (var i = 0; i < array.Length; i++) array[i] = random.NextDouble() * 1000d + 1d;
        array[0] = 0.0;                                  // IEEE minimum, becomes the comparison min
        for (var i = 1; i < array.Length; i += 97) array[i] = -0.0;
        for (var i = 2; i < array.Length; i += 97) array[i] = 0.0;

        var negativeZeros = array.Count(double.IsNegative);
        var zeros = array.Count(x => x == 0d);

        SpreadSort.Sort(array.AsSpan());

        // Non-decreasing under IComparable
        for (var i = 1; i < array.Length; i++)
        {
            await Assert.That(array[i - 1].CompareTo(array[i])).IsLessThanOrEqualTo(0);
        }
        // Every zero (of either sign) is at the front, and the signed-zero counts are preserved
        // exactly — an element must not be rewritten from -0.0 to +0.0 or vice versa.
        await Assert.That(array.Take(zeros).All(x => x == 0d)).IsTrue();
        await Assert.That(array.Count(double.IsNegative)).IsEqualTo(negativeZeros);
        await Assert.That(array.Count(x => x == 0d)).IsEqualTo(zeros);
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
