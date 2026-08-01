using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class AmericanFlagSortTests : IntegerSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => AmericanFlagSort.Sort(span, context);

    // No knob overrides: in-place permutation on sorted input may skip writes/swaps,
    // and per-bucket insertion sort makes compares data-dependent.

    // AmericanFlagSort is UNSTABLE: the in-place permutation may reorder equal keys,
    // so keySelector tests assert key order and permutation integrity only.

    [Test]
    public async Task KeySelectorSortsByKeyTest()
    {
        // Unstable sort: assert key order only, not tie order
        var random = new Random(42);
        var records = Enumerable.Range(0, 2000).Select(i => (Key: random.Next(-10000, 10000), Index: i)).ToArray();

        AmericanFlagSort.SortBy(records.AsSpan(), x => x.Key);

        var keys = records.Select(x => x.Key).ToArray();
        var expectedKeys = keys.OrderBy(x => x).ToArray();
        await Assert.That(keys).IsEquivalentTo(expectedKeys, CollectionOrdering.Matching);
        // All 2000 original records must still be present exactly once
        await Assert.That(records.Select(x => x.Index).OrderBy(x => x).ToArray())
            .IsEquivalentTo(Enumerable.Range(0, 2000).ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    public async Task KeySelectorNegativeKeysTest()
    {
        // Keys spanning negative/zero/positive; unstable sort, so assert key order only
        var records = new (int Key, string Name)[] { (3, "c"), (-5, "a"), (0, "b"), (-5, "a2"), (3, "c2"), (int.MinValue, "min"), (int.MaxValue, "max") };
        AmericanFlagSort.SortBy(records.AsSpan(), x => x.Key);

        await Assert.That(records.Select(x => x.Key).ToArray())
            .IsEquivalentTo([int.MinValue, -5, -5, 0, 3, 3, int.MaxValue], CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateHalf))]
    public async Task SortHalfResultOrderTest(IInputSample<Half> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        AmericanFlagSort.Sort(array.AsSpan(), stats);

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

        AmericanFlagSort.Sort(array.AsSpan(), stats);

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

        AmericanFlagSort.Sort(array.AsSpan(), stats);

        // Check is sorted (NaN-first total order, same as Array.Sort)
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    public async Task DecimalDigitBoundaryTest()
    {
        // Test values that cross decimal digit boundaries (9→10, 99→100, etc.)
        var array = new[] { 100, 9, 99, 10, 1, 999, 1000 };
        var expected = new[] { 1, 9, 10, 99, 100, 999, 1000 };
        AmericanFlagSort.Sort(array.AsSpan());
        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task InsertionSortCutoffTest()
    {
        // Test with array smaller than insertion sort cutoff (64)
        var array = new[] { 10, 5, 3, 8, 1, 9, 2, 7, 4, 6 };
        var expected = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        AmericanFlagSort.Sort(array.AsSpan());
        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    /// <summary>
    /// 桁数はキー幅ではなくキー範囲の幅から決まる。範囲スキャンが無い実装は常にキー幅由来の
    /// 4 桁（int / 8bit 桁）を報告するため、狭い範囲のケースがここで落ちる。
    /// 期待値は ⌈bitlength(max-min) / 8⌉。
    /// </summary>
    [Test]
    [Arguments("range 0..999 (10 bits)", 0, 1000, 2)]
    [Arguments("range 0..15 (4 bits)", 0, 16, 1)]
    [Arguments("range 0..(1<<20) (21 bits)", 0, 1 << 20, 3)]
    [Arguments("range spanning zero -500..500 (10 bits)", -500, 501, 2)]
    public async Task RangeScanDerivesDigitCountFromKeyRange(string label, int minInclusive, int maxExclusive, int expectedDigitCount)
    {
        var random = new Random(42);
        var array = new int[2000];
        for (var i = 0; i < array.Length; i++) array[i] = random.Next(minInclusive, maxExclusive);
        // 範囲の両端を必ず含め、min/max スキャンの結果を確定させる
        array[0] = minInclusive;
        array[1] = maxExclusive - 1;

        var totals = new List<int>();
        AmericanFlagSort.Sort(array.AsSpan(), new VisualizationContext(onPhase: (phase, _, p2, _) =>
        {
            if (phase == SortPhase.RadixPass) totals.Add(p2);
        }));

        await Assert.That(IsSorted(array)).IsTrue().Because(label);
        await Assert.That(totals).IsNotEmpty();
        await Assert.That(totals.Distinct().ToArray()).IsEquivalentTo([expectedDigitCount]);
    }

    /// <summary>
    /// 符号ビット反転キーが 0x8000_0000 をまたぐ入力では (max XOR min) がキー幅いっぱいを報告してしまう。
    /// 減算正規化はまたぎに影響されないので、同じ幅の範囲なら 0 起点でも 0 をまたいでも桁数は一致する。
    /// </summary>
    [Test]
    public async Task RangeScanIsUnaffectedByKeysStraddlingTheSignBoundary()
    {
        static int[] DigitCounts(int minInclusive, int maxExclusive)
        {
            var random = new Random(42);
            var array = new int[2000];
            for (var i = 0; i < array.Length; i++) array[i] = random.Next(minInclusive, maxExclusive);
            array[0] = minInclusive;
            array[1] = maxExclusive - 1;

            var totals = new List<int>();
            AmericanFlagSort.Sort(array.AsSpan(), new VisualizationContext(onPhase: (phase, _, p2, _) =>
            {
                if (phase == SortPhase.RadixPass) totals.Add(p2);
            }));
            return totals.Distinct().ToArray();
        }

        // どちらも max-min == 1000。前者は 0 起点、後者は符号境界をまたぐ
        await Assert.That(DigitCounts(0, 1001)).IsEquivalentTo(DigitCounts(-500, 501));
    }

    /// <summary>
    /// 全キーが等しい入力は範囲スキャンで打ち切られ、桁パスに一切入らない。
    /// スキャン導入前は 8 回の数え上げパス（8n reads）を回していた。
    /// </summary>
    [Test]
    public async Task AllEqualKeysSkipDigitPassesEntirely()
    {
        const int n = 1000;
        var array = Enumerable.Repeat(42, n).ToArray();

        var radixPasses = 0;
        AmericanFlagSort.Sort(array.AsSpan(), new VisualizationContext(onPhase: (phase, _, _, _) =>
        {
            if (phase == SortPhase.RadixPass) radixPasses++;
        }));
        await Assert.That(radixPasses).IsEqualTo(0);

        var stats = new StatisticsContext();
        AmericanFlagSort.Sort(array.AsSpan(), stats);

        // スキャンの n reads のみ。要素は 1 つも動かない
        await Assert.That(stats.IndexReadCount).IsEqualTo((ulong)n);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.CompareCount).IsEqualTo(0UL);
        await Assert.That(array).IsEquivalentTo(Enumerable.Repeat(42, n).ToArray(), CollectionOrdering.Matching);
    }

    /// <summary>
    /// 減算正規化は order-preserving でなければならない。キー空間の両端を含む入力で確認する。
    /// </summary>
    [Test]
    public async Task KeySpaceExtremesSortCorrectly()
    {
        var random = new Random(42);
        var array = new int[500];
        for (var i = 0; i < array.Length; i++) array[i] = random.Next(int.MinValue, int.MaxValue);
        array[0] = int.MinValue;
        array[1] = int.MaxValue;
        array[2] = 0;
        array[3] = -1;

        var expected = array.ToArray();
        Array.Sort(expected);

        AmericanFlagSort.Sort(array.AsSpan());

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task LargeRangeTest()
    {
        var stats = new StatisticsContext();
        // Test with values spanning a large range
        var array = new[] { 1000000, -1000000, 0, 500000, -500000 };
        var expected = new[] { -1000000, -500000, 0, 500000, 1000000 };
        AmericanFlagSort.Sort(array.AsSpan(), stats);
        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task InPlacePermutationTest()
    {
        var stats = new StatisticsContext();
        // Verify that the sort is performed in-place (no auxiliary array).
        // The array must exceed InsertionSortCutoff (64), otherwise the range never reaches the
        // in-place permutation and this test would silently be an InsertionSort test.
        var random = new Random(42);
        var array = Enumerable.Range(11, 200).OrderBy(_ => random.Next()).ToArray();
        var expected = Enumerable.Range(11, 200).ToArray();

        AmericanFlagSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
        // In-place permutation moves elements with Swap on the main buffer only
        await Assert.That(stats.SwapCount).IsNotEqualTo(0UL);
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
        AmericanFlagSort.Sort(sorted.AsSpan(), stats);

        // American Flag Sort is an in-place MSD Radix Sort variant
        // For sorted data:
        // - Elements distribute into buckets
        // - Small buckets (<=64) use insertion sort
        // - In-place permutation minimizes writes
        await Assert.That((ulong)sorted.Length).IsEqualTo((ulong)n);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);

        // For sorted data, verify that the sort completes successfully
        await Assert.That(IsSorted(sorted)).IsTrue();
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
        AmericanFlagSort.Sort(reversed.AsSpan(), stats);

        // American Flag Sort on reversed data:
        // - In-place permutation requires swaps to rearrange elements
        // - Insertion sort for small buckets has more operations
        await Assert.That((ulong)reversed.Length).IsEqualTo((ulong)n);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(IsSorted(reversed)).IsTrue();
    }

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesRandomTest(int n)
    {
        var stats = new StatisticsContext();
        var random = new Random(42);
        var array = Enumerable.Range(0, n).OrderBy(_ => random.Next()).ToArray();
        AmericanFlagSort.Sort(array.AsSpan(), stats);

        // American Flag Sort on random data:
        // - Bucket distribution varies
        // - In-place permutation requires swaps
        // - Combination of MSD partitioning and insertion sort
        await Assert.That((ulong)array.Length).IsEqualTo((ulong)n);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(IsSorted(array)).IsTrue();

        // Random data should require swap operations when n > InsertionSortCutoff (64).
        // At or below the cutoff the range never reaches the in-place permutation: InsertionSort
        // shifts with Write, so SwapCount legitimately stays 0 there.
        if (n > 64)
        {
            await Assert.That(stats.SwapCount).IsNotEqualTo(0UL);
        }
    }
}
