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

        // FlatStableSort on sorted data is fully deterministic regardless of n:
        //
        // n <= SORT_MIN*2 (72) → BinaryInsertionSort:
        //   Each element passes the early-termination check s.IsLessOrEqualAt(i-1, i) immediately.
        //   → exactly n-1 comparisons (one check per element), 0 writes, 0 swaps.
        //
        // n > 72 → SortCore → IsAscending scans all n-1 pairs, finds them in order, returns early.
        //   → exactly n-1 comparisons, 0 writes, 0 swaps.
        //
        // Both paths: each comparison reads 2 elements → 2*(n-1) reads total.
        await Assert.That(stats.CompareCount).IsEqualTo((ulong)(n - 1));
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexReadCount).IsEqualTo((ulong)(2 * (n - 1)));
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

        // FlatStableSort on strictly reversed data splits into two deterministic paths:
        //
        // n <= SORT_MIN*2 (72) → BinaryInsertionSort (no pre-sort detection):
        //   Each element at position i must shift all i preceding elements right and insert at 0.
        //   - Early-termination check fails for every element: n-1 total checks (n-1 compares)
        //   - Binary search finds insertion point at 0: ceil(log₂(i+1)) compares per element
        //   - Total compares: Σ(i=1..n-1)[1 + ceil(log₂(i+1))]  ≤ n·(ceil(log₂n) + 1)
        //   - Writes (exact): each element i shifts i positions (i writes) + 1 insert write
        //     Total: Σ(i=1..n-1)(i+1) = n·(n+1)/2 - 1
        //   - Swaps: 0 (BinaryInsertionSort uses shifts, not swaps)
        //
        //   Observations: n=10 → writes=54, n=20 → writes=209, n=50 → writes=1274
        //
        // n > 72 → SortCore detects IsDescending (strict), calls Reverse (all exact):
        //   - IsAscending: 1 compare, 2 reads (fails immediately at first pair)
        //   - IsDescending: n-1 compares, 2·(n-1) reads (all pairs pass)
        //   - Reverse: n/2 swaps → n writes, n reads
        //   - Total: compares=n, writes=n, reads=3n, swaps=n/2
        //
        //   Observations: n=100 → compares=100, writes=100, reads=300, swaps=50
        if (n <= 72)
        {
            var maxCompares = (ulong)(n * ((int)Math.Ceiling(Math.Log2(n)) + 1));
            var expectedWrites = (ulong)(n * (n + 1) / 2 - 1);
            await Assert.That(stats.CompareCount).IsBetween((ulong)(n - 1), maxCompares);
            await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
            await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        }
        else
        {
            await Assert.That(stats.CompareCount).IsEqualTo((ulong)n);
            await Assert.That(stats.IndexWriteCount).IsEqualTo((ulong)n);
            await Assert.That(stats.IndexReadCount).IsEqualTo((ulong)(3 * n));
            await Assert.That(stats.SwapCount).IsEqualTo((ulong)(n / 2));
        }
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

        // FlatStableSort on random data:
        //
        // n <= SORT_MIN*2 (72) → BinaryInsertionSort:
        //   - Comparisons: O(n log n) via binary search per insertion
        //     Range: [n-1 (sorted), n·(ceil(log₂n)+1) (reversed worst case)]
        //   - Writes: O(n²) worst case; [0 (sorted), n·(n+1)/2 (reversed)]
        //   - Swaps: 0 always (BinaryInsertionSort uses shifts, not swaps)
        //
        // n > 72 → recursive merge sort O(n log n):
        //   - Comparisons and writes: both in [n·log₂n·0.5, n·log₂n·2.5]
        //   - Swaps: 0 always (copy-merge, no in-place swaps)
        //
        //   Observations for n=100 random: compares≈866-925, writes≈860-937
        var logN = Math.Log2(n);
        if (n <= 72)
        {
            var maxCompares = (ulong)(n * ((int)Math.Ceiling(Math.Log2(n)) + 1));
            var maxWrites = (ulong)(n * (n + 1) / 2);
            await Assert.That(stats.CompareCount).IsBetween((ulong)(n - 1), maxCompares);
            await Assert.That(stats.IndexWriteCount).IsBetween(0UL, maxWrites);
        }
        else
        {
            var minBound = (ulong)(n * logN * 0.5);
            var maxBound = (ulong)(n * logN * 2.5);
            await Assert.That(stats.CompareCount).IsBetween(minBound, maxBound);
            await Assert.That(stats.IndexWriteCount).IsBetween(minBound, maxBound);
        }
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexReadCount > 0).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be > 0");
    }
}
