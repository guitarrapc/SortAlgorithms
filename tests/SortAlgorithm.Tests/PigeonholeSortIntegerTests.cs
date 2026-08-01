using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using System.Numerics;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class PigeonholeSortIntegerTests : IntegerSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => PigeonholeSortInteger.Sort(span, context);

    // Throws ArgumentException on excessive key ranges (see RangeLimitTest), so full-integer-range inputs are rejected by contract.
    protected override bool SupportsFullIntegerRange => false;

    // Min/max scan uses tracked comparisons (exact 2n+1 asserted in TheoreticalValues tests); distribute/place always write; no swaps.
    protected override CountExpectation SortedInputCompares => CountExpectation.NonZero;
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    [Arguments(10_000_001)]
    public async Task RangeLimitTest(int range)
    {
        // Test that excessive range throws ArgumentException
        var array = new[] { 0, range };
        Assert.Throws<ArgumentException>(() => PigeonholeSortInteger.Sort(array.AsSpan()));
    }

    [Test]
    [Arguments(2, 10_000)]    // range=10,001 > MaxRangeFactor*n=64,   but < MaxHoleArraySize
    [Arguments(100, 5_000)]   // range=5,001  > MaxRangeFactor*n=3200, but < MaxHoleArraySize
    [Arguments(100, 3_200)]   // range=3,201: one past MaxRangeFactor*n, the smallest rejected range
    public async Task RelativeRangeLimitTest(int n, int maxValue)
    {
        // range is well within the absolute cap but too large relative to n: collecting from the holes
        // walks the whole range, so O(range) would dominate O(n).
        var array = new int[n];
        array[n - 1] = maxValue;
        Assert.Throws<ArgumentException>(() => PigeonholeSortInteger.Sort(array.AsSpan()));
    }

    [Test]
    [Arguments(100, 3_199)]   // range=3,200 == MaxRangeFactor*n exactly: at the limit, still accepted
    public async Task RelativeRangeAtTheLimitIsAccepted(int n, int maxValue)
    {
        var array = new int[n];
        array[n - 1] = maxValue;
        PigeonholeSortInteger.Sort(array.AsSpan());

        var expected = new int[n];
        expected[n - 1] = maxValue;
        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task NegativeValuesTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { -5, -1, -10, 3, 0, -3 };
        PigeonholeSortInteger.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo([-10, -5, -3, -1, 0, 3], CollectionOrdering.Matching);
    }

    /// <summary>
    /// Collection rebuilds each value from its hole index (<c>umin + h</c> narrowed back to T) rather than
    /// reading it out of a buffer, so the 2's complement round-trip has to hold at both ends of every
    /// supported type — a value near a type's minimum and one near its maximum sit at opposite ends of
    /// the wrapping ulong arithmetic.
    /// </summary>
    [Test]
    public async Task RebuildsValuesAtTypeBoundaries()
    {
        // sbyte / byte: the arrays span the type's whole range, so the hole index walks from one end to the other.
        await Roundtrip<sbyte>([sbyte.MaxValue, sbyte.MinValue + 2, sbyte.MinValue, sbyte.MaxValue - 3,
                                sbyte.MinValue + 1, sbyte.MaxValue - 1, sbyte.MinValue + 3, sbyte.MaxValue - 2]);
        await Roundtrip<byte>([byte.MaxValue, 2, byte.MinValue, byte.MaxValue - 3,
                               1, byte.MaxValue - 1, 3, byte.MaxValue - 2]);

        // Wider types: the full range is rejected by the range/n guard, so probe a narrow window at each end.
        await Roundtrip<short>([short.MinValue + 2, short.MinValue, short.MinValue + 3, short.MinValue + 1]);
        await Roundtrip<short>([short.MaxValue, short.MaxValue - 2, short.MaxValue - 3, short.MaxValue - 1]);
        await Roundtrip<ushort>([ushort.MaxValue, ushort.MaxValue - 2, ushort.MaxValue - 3, ushort.MaxValue - 1]);
        await Roundtrip<int>([int.MinValue + 2, int.MinValue, int.MinValue + 3, int.MinValue + 1]);
        await Roundtrip<int>([int.MaxValue, int.MaxValue - 2, int.MaxValue - 3, int.MaxValue - 1]);
        await Roundtrip<uint>([uint.MaxValue, uint.MaxValue - 2, uint.MaxValue - 3, uint.MaxValue - 1]);
        await Roundtrip<long>([long.MinValue + 2, long.MinValue, long.MinValue + 3, long.MinValue + 1]);
        await Roundtrip<long>([long.MaxValue, long.MaxValue - 2, long.MaxValue - 3, long.MaxValue - 1]);
        await Roundtrip<ulong>([ulong.MaxValue, ulong.MaxValue - 2, ulong.MaxValue - 3, ulong.MaxValue - 1]);
        await Roundtrip<nint>([nint.MinValue + 2, nint.MinValue, nint.MinValue + 3, nint.MinValue + 1]);
        await Roundtrip<nuint>([nuint.MaxValue, nuint.MaxValue - 2, nuint.MaxValue - 3, nuint.MaxValue - 1]);

        static async Task Roundtrip<T>(T[] values) where T : IBinaryInteger<T>, IMinMaxValue<T>
        {
            var actual = values.ToArray();
            var expected = values.ToArray();
            Array.Sort(expected);

            PigeonholeSortInteger.Sort(actual.AsSpan());

            await Assert.That(actual).IsEquivalentTo(expected, CollectionOrdering.Matching)
                .Because($"{typeof(T).Name}: [{string.Join(", ", values)}]");
        }
    }

    /// <summary>
    /// A hole records occupancy, not elements, so the sort has no auxiliary element buffer to report.
    /// Every element operation must therefore land on the main array. This is the observable consequence
    /// of the integer specialization, and a consumer laying out buffers depends on it.
    /// </summary>
    [Test]
    public async Task UsesNoAuxiliaryElementBuffer()
    {
        var buffersTouched = new HashSet<int>();
        var rangeCopies = 0;

        var context = new VisualizationContext(
            onCompare: (_, _, _, bi, bj) => { if (bi >= 0) buffersTouched.Add(bi); if (bj >= 0) buffersTouched.Add(bj); },
            onIndexRead: (_, b) => buffersTouched.Add(b),
            onIndexWrite: (_, b, _) => buffersTouched.Add(b),
            onRangeCopy: (_, _, _, _, _, _) => rangeCopies++);

        var random = new Random(42);
        var array = Enumerable.Range(0, 300).Select(_ => random.Next(0, 200)).ToArray();
        var expected = array.OrderBy(x => x).ToArray();

        PigeonholeSortInteger.Sort(array.AsSpan(), context);

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching)
            .Because("観測を変えてもソート結果は変わらないこと");
        await Assert.That(buffersTouched.Order().ToList()).IsEquivalentTo(new List<int> { 0 })
            .Because($"観測されたバッファー: [{string.Join(",", buffersTouched.Order())}]");
        await Assert.That(rangeCopies).IsEqualTo(0)
            .Because("穴から直接書き戻すため、一時バッファーからの一括コピーは存在しない");
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
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        PigeonholeSortInteger.Sort(sorted.AsSpan(), stats);

        // Pigeonhole Sort over integers (via SortSpan). A hole stores occupancy, not elements, so
        // there is no auxiliary buffer and no second pass over the input:
        // 1. Find min/max        : n reads (main buffer)
        // 2. Drop into holes     : n reads (main buffer), no writes
        // 3. Empty holes in order: n writes (main buffer), no reads - the hole index gives the value
        //
        // Total reads: n + n = 2n
        // Total writes: n
        var expectedReads = (ulong)(2 * n);
        var expectedWrites = (ulong)n;
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
        // Same operation counts for reversed as for sorted
        var expectedReads = (ulong)(2 * n);
        var expectedWrites = (ulong)n;
        var expectedCompares = (ulong)(2 * n) + 1;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
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
        PigeonholeSortInteger.Sort(random.AsSpan(), stats);

        // Pigeonhole Sort has same complexity regardless of input distribution: 2n reads, n writes
        var expectedReads = (ulong)(2 * n);
        var expectedWrites = (ulong)n;
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
