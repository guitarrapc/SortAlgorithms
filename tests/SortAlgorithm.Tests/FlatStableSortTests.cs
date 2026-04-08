using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class FlatStableSortTests
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

        FlatStableSort.Sort(array.AsSpan(), stats);

        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateHalf))]
    public async Task SortHalfResultOrderTest(IInputSample<Half> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        FlatStableSort.Sort(array.AsSpan(), stats);

        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateFloat))]
    public async Task SortFloatResultOrderTest(IInputSample<float> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        FlatStableSort.Sort(array.AsSpan(), stats);

        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateDouble))]
    public async Task SortDoubleResultOrderTest(IInputSample<double> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        FlatStableSort.Sort(array.AsSpan(), stats);

        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockIntKeyRandomData), nameof(MockIntKeyRandomData.Generate))]
    public async Task SortIntStructResultOrderTest(IInputSample<Utils.IntKey> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        FlatStableSort.Sort(array.AsSpan(), stats);

        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    // Tail block RearrangeWithIndex edge-case tests
    // These six scenarios exercise the invariant that RearrangeWithIndex never copies
    // a shorter tail block (ntail elements) into a full-size block slot.
    // That invariant requires _index[nblock-1] == nblock-1 after all MergeRangePos calls.

    /// <summary>
    /// n % blockSize == 1 and n % blockSize == blockSize-1:
    /// boundary tail sizes with random data and stability check.
    /// </summary>
    [Test]
    [Arguments(1)]
    [Arguments(IntBlockSize - 1)]
    public async Task TailBoundarySizeStabilityTest(int tailSize)
    {
        var n = (2 * IntBlockSize) + tailSize;
        var rng = new Random(42);
        var items = Enumerable.Range(0, n)
            .Select(i => new StabilityTestItem(rng.Next(0, 50), i))
            .ToArray();
        var expected = items.OrderBy(x => x.Value).ToArray();

        FlatStableSort.Sort(items.AsSpan(), new StatisticsContext());

        for (var i = 0; i < items.Length; i++)
        {
            await Assert.That(items[i].Value).IsEqualTo(expected[i].Value);
            await Assert.That(items[i].OriginalIndex).IsEqualTo(expected[i].OriginalIndex);
        }
    }

    /// <summary>
    /// Tail block contains the maximum values: tail data stays at the physical end.
    /// Exercises the case where the tail block does NOT need to permute across block boundaries.
    /// </summary>
    [Test]
    [Arguments(1)]
    [Arguments(128)]
    [Arguments(IntBlockSize - 1)]
    public async Task TailContainsMaxValuesRearrangeTest(int tailSize)
    {
        var totalLength = (2 * IntBlockSize) + tailSize;
        var items = new StabilityTestItem[totalLength];

        // Block 0: small values [1..10]
        for (var i = 0; i < IntBlockSize; i++)
            items[i] = new StabilityTestItem(i / 100 + 1, i);

        // Block 1: medium values [20..30]
        for (var i = 0; i < IntBlockSize; i++)
            items[IntBlockSize + i] = new StabilityTestItem(i / 100 + 20, IntBlockSize + i);

        // Tail: large unique values [100..100+tailSize) — always sort to the end
        var tailStart = 2 * IntBlockSize;
        for (var i = 0; i < tailSize; i++)
            items[tailStart + i] = new StabilityTestItem(100 + i, tailStart + i);

        var expected = items.OrderBy(x => x.Value).ToArray();
        FlatStableSort.Sort(items.AsSpan(), new StatisticsContext());

        for (var i = 0; i < items.Length; i++)
        {
            await Assert.That(items[i].Value).IsEqualTo(expected[i].Value);
            await Assert.That(items[i].OriginalIndex).IsEqualTo(expected[i].OriginalIndex);
        }
    }

    /// <summary>
    /// All elements equal: every block (including the tail) carries the same value.
    /// Only OriginalIndex order can distinguish correctness; tests full-array stability.
    /// </summary>
    [Test]
    [Arguments(1)]
    [Arguments(100)]
    [Arguments(IntBlockSize - 1)]
    public async Task AllEqualElementsWithTailStabilityTest(int tailSize)
    {
        var n = (2 * IntBlockSize) + tailSize;
        var items = Enumerable.Range(0, n)
            .Select(i => new StabilityTestItem(42, i))
            .ToArray();

        FlatStableSort.Sort(items.AsSpan(), new StatisticsContext());

        for (var i = 0; i < items.Length; i++)
            await Assert.That(items[i].Value).IsEqualTo(42);

        for (var i = 1; i < items.Length; i++)
            await Assert.That(items[i - 1].OriginalIndex).IsLessThan(items[i].OriginalIndex);
    }

    /// <summary>
    /// Equal keys that span the boundary between the last full block and the tail block.
    /// Verifies that MergeRangePos + RearrangeWithIndex preserve stable order across that boundary.
    /// </summary>
    [Test]
    [Arguments(1)]
    [Arguments(50)]
    [Arguments(IntBlockSize - 1)]
    public async Task EqualKeysCrossingTailBoundaryStabilityTest(int tailSize)
    {
        const int equalValue = 5;
        const int equalInBlock = 100; // how many elements at the end of block 1 get equalValue
        var totalLength = (2 * IntBlockSize) + tailSize;
        var items = new StabilityTestItem[totalLength];

        // Block 0: distinct values well above equalValue
        for (var i = 0; i < IntBlockSize; i++)
            items[i] = new StabilityTestItem(100 + i, i);

        // Block 1 first part: distinct values
        for (var i = 0; i < IntBlockSize - equalInBlock; i++)
            items[IntBlockSize + i] = new StabilityTestItem(200 + i, IntBlockSize + i);

        // Block 1 last part: equalValue (these straddle the block/tail boundary)
        for (var i = IntBlockSize - equalInBlock; i < IntBlockSize; i++)
            items[IntBlockSize + i] = new StabilityTestItem(equalValue, IntBlockSize + i);

        // Tail first part: equalValue (continuation of the equal run from block 1)
        var tailStart = 2 * IntBlockSize;
        var equalInTail = Math.Min(equalInBlock, tailSize);
        for (var i = 0; i < equalInTail; i++)
            items[tailStart + i] = new StabilityTestItem(equalValue, tailStart + i);

        // Tail remainder: distinct values
        for (var i = equalInTail; i < tailSize; i++)
            items[tailStart + i] = new StabilityTestItem(300 + i, tailStart + i);

        var expected = items.OrderBy(x => x.Value).ToArray();
        FlatStableSort.Sort(items.AsSpan(), new StatisticsContext());

        for (var i = 0; i < items.Length; i++)
        {
            await Assert.That(items[i].Value).IsEqualTo(expected[i].Value);
            await Assert.That(items[i].OriginalIndex).IsEqualTo(expected[i].OriginalIndex);
        }
    }

    // Stability tests

    [Test]
    [MethodDataSource(typeof(MockStabilityData), nameof(MockStabilityData.Generate))]
    public async Task StabilityTest(StabilityTestItem[] items)
    {
        var stats = new StatisticsContext();

        FlatStableSort.Sort(items.AsSpan(), stats);

        await Assert.That(items.Select(x => x.Value).ToArray()).IsEquivalentTo(MockStabilityData.Sorted, CollectionOrdering.Matching);

        var value1Indices = items.Where(x => x.Value == 1).Select(x => x.OriginalIndex).ToArray();
        var value2Indices = items.Where(x => x.Value == 2).Select(x => x.OriginalIndex).ToArray();
        var value3Indices = items.Where(x => x.Value == 3).Select(x => x.OriginalIndex).ToArray();

        await Assert.That(value1Indices).IsEquivalentTo(MockStabilityData.Sorted1, CollectionOrdering.Matching);
        await Assert.That(value2Indices).IsEquivalentTo(MockStabilityData.Sorted2, CollectionOrdering.Matching);
        await Assert.That(value3Indices).IsEquivalentTo(MockStabilityData.Sorted3, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockStabilityWithIdData), nameof(MockStabilityWithIdData.Generate))]
    public async Task StabilityTestWithComplex(StabilityTestItemWithId[] items)
    {
        var stats = new StatisticsContext();

        FlatStableSort.Sort(items.AsSpan(), stats);

        for (var i = 0; i < items.Length; i++)
        {
            await Assert.That(items[i].Key).IsEqualTo(MockStabilityWithIdData.Sorted[i].Key);
            await Assert.That(items[i].Id).IsEqualTo(MockStabilityWithIdData.Sorted[i].Id);
        }
    }

    [Test]
    [MethodDataSource(typeof(MockStabilityAllEqualsData), nameof(MockStabilityAllEqualsData.Generate))]
    public async Task StabilityTestWithAllEqual(StabilityTestItem[] items)
    {
        var stats = new StatisticsContext();

        FlatStableSort.Sort(items.AsSpan(), stats);

        foreach (var item in items)
            await Assert.That(item.Value).IsEqualTo(1);

        await Assert.That(items.Select(x => x.OriginalIndex).ToArray()).IsEquivalentTo(MockStabilityAllEqualsData.Sorted, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockSortedData), nameof(MockSortedData.Generate))]
    public async Task StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        FlatStableSort.Sort(array.AsSpan(), stats);

        await Assert.That((ulong)array.Length).IsEqualTo((ulong)inputSample.Samples.Length);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(stats.CompareCount).IsNotEqualTo(0UL);
    }

    [Test]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(10)]
    [Arguments(36)]
    [Arguments(72)]
    [Arguments(73)]
    [Arguments(100)]
    [Arguments(256)]
    [Arguments(1024)]
    [Arguments(4096)]
    [Arguments(10000)]
    public async Task SortSmallAndBoundaryTest(int n)
    {
        var stats = new StatisticsContext();
        var rng = new Random(42);
        var array = Enumerable.Range(0, n).Select(_ => rng.Next(0, n)).ToArray();
        var expected = array.ToArray();
        Array.Sort(expected);

        FlatStableSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    private const int IntBlockSize = 1024;

    [Test]
    [Arguments(1)]
    [Arguments(17)]
    [Arguments(257)]
    [Arguments(IntBlockSize - 1)]
    public async Task TailBlockRearrangePreservesStableOrderTest(int tailLength)
    {
        var items = CreateTailBlockStressItems(tailLength);
        var expected = items.OrderBy(x => x.Value).ToArray();

        FlatStableSort.Sort(items.AsSpan(), new StatisticsContext());

        for (var i = 0; i < items.Length; i++)
        {
            await Assert.That(items[i].Value).IsEqualTo(expected[i].Value);
            await Assert.That(items[i].OriginalIndex).IsEqualTo(expected[i].OriginalIndex);
        }

        for (var i = 1; i < items.Length; i++)
            await Assert.That(items[i - 1].Value).IsLessThanOrEqualTo(items[i].Value);

        foreach (var group in items.GroupBy(x => x.Value))
        {
            var indices = group.Select(x => x.OriginalIndex).ToArray();
            for (var i = 1; i < indices.Length; i++)
                await Assert.That(indices[i - 1]).IsLessThan(indices[i]);
        }

        var firstTailIndex = (2 * IntBlockSize);
        var lowValueIndices = items.Where(x => x.Value <= 1).Select(x => x.OriginalIndex).ToArray();
        foreach (var originalIndex in lowValueIndices)
            await Assert.That(originalIndex).IsGreaterThanOrEqualTo(firstTailIndex);
    }

    [Test]
    [Arguments(1)]
    [Arguments(17)]
    [Arguments(257)]
    [Arguments(IntBlockSize - 1)]
    public async Task TailBlockStatisticsStaySwapFreeTest(int tailLength)
    {
        var stats = new StatisticsContext();
        var items = CreateTailBlockStressItems(tailLength);

        FlatStableSort.Sort(items.AsSpan(), stats);

        await Assert.That(stats.CompareCount).IsGreaterThanOrEqualTo((ulong)(items.Length - 1));
        await Assert.That(stats.IndexReadCount).IsGreaterThanOrEqualTo((ulong)(2 * tailLength));
        await Assert.That(stats.IndexWriteCount).IsGreaterThanOrEqualTo((ulong)tailLength);
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
        FlatStableSort.Sort(sorted.AsSpan(), stats);

        // The Boost-like implementation no longer performs a whole-range sorted fast exit.
        // Small ranges use range_sort_data/range_sort_buffer and larger ranges may use the
        // partial-sorted block fast path, so already-sorted input still performs structured
        // merge-sort work. Guard against accidental quadratic regressions while preserving
        // the expectation that stable merge-based paths do not swap.
        var maxBound = (ulong)(Math.Max(1, n) * Math.Max(1, (int)Math.Ceiling(Math.Log2(Math.Max(2, n)))) * 4);
        await Assert.That(stats.CompareCount).IsBetween((ulong)(n - 1), maxBound);
        await Assert.That(stats.IndexWriteCount).IsBetween(0UL, (ulong)(n * n));
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
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
        FlatStableSort.Sort(reversed.AsSpan(), stats);

        // The Boost-like path also removed the whole-range reverse detection shortcut.
        // Reversed input therefore exercises the same recursive machinery as arbitrary data,
        // with insertion-sort leaves making the constants somewhat larger on small ranges.
        var maxBound = (ulong)(Math.Max(1, n) * Math.Max(1, (int)Math.Ceiling(Math.Log2(Math.Max(2, n)))) * 4);
        await Assert.That(stats.CompareCount).IsBetween((ulong)(n - 1), (ulong)(n * n));
        await Assert.That(stats.IndexWriteCount).IsBetween((ulong)n, (ulong)(n * n));
        await Assert.That(stats.IndexReadCount).IsBetween((ulong)(2 * (n - 1)), (ulong)(n * n * 2));
        await Assert.That(stats.CompareCount).IsLessThanOrEqualTo(maxBound + (ulong)(n * n / 8));
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
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
        FlatStableSort.Sort(random.AsSpan(), stats);

        // Random input should stay within the expected sub-quadratic region for comparisons,
        // although writes can still be larger because the Boost-style insertion helpers shift
        // blocks when exploiting partially sorted prefixes/suffixes.
        var logN = Math.Max(1D, Math.Log2(Math.Max(2, n)));
        var minBound = (ulong)Math.Max(n - 1, 1);
        var maxCompares = (ulong)(n * logN * 4);
        await Assert.That(stats.CompareCount).IsBetween(minBound, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(1UL, (ulong)(n * n));
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexReadCount > 0).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be > 0");
    }

    private static StabilityTestItem[] CreateTailBlockStressItems(int tailLength)
    {
        var totalLength = (2 * IntBlockSize) + tailLength;
        var items = new StabilityTestItem[totalLength];

        for (var i = 0; i < IntBlockSize; i++)
        {
            var value = 6 + ((i / 64) & 1);
            items[i] = new StabilityTestItem(value, i);
        }

        for (var i = 0; i < IntBlockSize; i++)
        {
            var index = IntBlockSize + i;
            var value = 2 + ((i / 64) & 1);
            items[index] = new StabilityTestItem(value, index);
        }

        for (var i = 0; i < tailLength; i++)
        {
            var index = (2 * IntBlockSize) + i;
            var value = i % 3 switch
            {
                0 => 0,
                1 => 1,
                _ => 2,
            };
            items[index] = new StabilityTestItem(value, index);
        }

        return items;
    }
}
