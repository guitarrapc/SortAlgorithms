using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class SpinSortVariantTests
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

        SpinSortVariant.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateHalf))]
    public async Task SortHalfResultOrderTest(IInputSample<Half> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        SpinSortVariant.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateFloat))]
    public async Task SortFloatResultOrderTest(IInputSample<float> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        SpinSortVariant.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateDouble))]
    public async Task SortDoubleResultOrderTest(IInputSample<double> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        SpinSortVariant.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockIntKeyRandomData), nameof(MockIntKeyRandomData.Generate))]
    public async Task SortIntStructResultOrderTest(IInputSample<Utils.IntKey> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        SpinSortVariant.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(10)]
    [Arguments(36)]
    [Arguments(72)]
    [Arguments(73)]
    [Arguments(100)]
    [Arguments(200)]
    public async Task SortSmallAndBoundaryTest(int n)
    {
        var stats = new StatisticsContext();
        var rng = new Random(42);
        var array = Enumerable.Range(0, n).Select(_ => rng.Next(0, n)).ToArray();
        var expected = array.ToArray();
        Array.Sort(expected);

        SpinSortVariant.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockStabilityData), nameof(MockStabilityData.Generate))]
    public async Task StabilityTest(StabilityTestItem[] items)
    {
        // Test stability: equal elements should maintain relative order
        var stats = new StatisticsContext();

        SpinSortVariant.Sort(items.AsSpan(), stats);

        // Verify sorting correctness - values should be in ascending order
        await Assert.That(items.Select(x => x.Value).ToArray()).IsEquivalentTo(MockStabilityData.Sorted, CollectionOrdering.Matching);

        // Verify stability: for each group of equal values, original order is preserved
        var value1Indices = items.Where(x => x.Value == 1).Select(x => x.OriginalIndex).ToArray();
        var value2Indices = items.Where(x => x.Value == 2).Select(x => x.OriginalIndex).ToArray();
        var value3Indices = items.Where(x => x.Value == 3).Select(x => x.OriginalIndex).ToArray();

        // Value 1 appeared at original indices 0, 2, 4 - should remain in this order
        await Assert.That(value1Indices).IsEquivalentTo(MockStabilityData.Sorted1, CollectionOrdering.Matching);

        // Value 2 appeared at original indices 1, 5 - should remain in this order
        await Assert.That(value2Indices).IsEquivalentTo(MockStabilityData.Sorted2, CollectionOrdering.Matching);

        // Value 3 appeared at original index 3
        await Assert.That(value3Indices).IsEquivalentTo(MockStabilityData.Sorted3, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockStabilityWithIdData), nameof(MockStabilityWithIdData.Generate))]
    public async Task StabilityTestWithComplex(StabilityTestItemWithId[] items)
    {
        // Test stability with more complex scenario - multiple equal values
        var stats = new StatisticsContext();

        SpinSortVariant.Sort(items.AsSpan(), stats);

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
        // Edge case: all elements have the same value - original order should be preserved
        var stats = new StatisticsContext();

        SpinSortVariant.Sort(items.AsSpan(), stats);

        // All values are 1
        foreach (var item in items) await Assert.That(item.Value).IsEqualTo(1);

        // Original order should be preserved: 0, 1, 2, 3, 4
        await Assert.That(items.Select(x => x.OriginalIndex).ToArray()).IsEquivalentTo(MockStabilityAllEqualsData.Sorted, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockSortedData), nameof(MockSortedData.Generate))]
    public async Task StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        SpinSortVariant.Sort(array.AsSpan(), stats);

        await Assert.That((ulong)array.Length).IsEqualTo((ulong)inputSample.Samples.Length);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.CompareCount).IsNotEqualTo(0UL);
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
        SpinSortVariant.Sort(sorted.AsSpan(), stats);

        // SpinSort for sorted data:
        // n ≤ 72: BinaryInsertionSort detects each element is already in place with a single comparison,
        //         yielding exactly n-1 comparisons, 0 writes, 0 swaps.
        // n > 72: CheckPreSorted detects ascending order with n-1 comparisons.
        // Both paths produce identical statistics.
        //
        // Observed:
        // n=10:  9 comparisons,  0 writes, 18 reads, 0 swaps
        // n=20:  19 comparisons, 0 writes, 38 reads, 0 swaps
        // n=50:  49 comparisons, 0 writes, 98 reads, 0 swaps
        // n=100: 99 comparisons, 0 writes, 198 reads, 0 swaps
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
        SpinSortVariant.Sort(reversed.AsSpan(), stats);

        // SpinSort for reversed data:
        // n ≤ 72: BinaryInsertionSort handles it directly.
        //         O(n log n) comparisons for binary search, O(n²) writes for shifting elements.
        // n > 72: CheckPreSorted detects strictly descending → Reverse in-place.
        //         Exactly n-1 comparisons, n writes (2 per swap), n/2 swaps.
        //
        // Observed:
        // n=10:  34 comparisons, 54 writes, 0 swaps    (BinaryInsertionSort)
        // n=20:  88 comparisons, 209 writes, 0 swaps   (BinaryInsertionSort)
        // n=50:  286 comparisons, 1274 writes, 0 swaps (BinaryInsertionSort)
        // n=100: 99 comparisons, 100 writes, 50 swaps  (CheckPreSorted + Reverse)
        ulong minCompares, maxCompares, minWrites, maxWrites, minSwaps, maxSwaps;
        if (n <= 72)
        {
            // BinaryInsertionSort: O(n log n) compares, O(n²) writes
            minCompares = (ulong)(n - 1);
            maxCompares = (ulong)(n * Math.Ceiling(Math.Log2(n + 1)));
            minWrites = (ulong)(n);
            maxWrites = (ulong)((long)n * n);
            minSwaps = 0UL;
            maxSwaps = 0UL; // BinaryInsertionSort uses shifts, not swaps
        }
        else
        {
            // CheckPreSorted → Reverse: exact values
            minCompares = (ulong)(n - 1);
            maxCompares = (ulong)(n - 1);
            minWrites = (ulong)n;
            maxWrites = (ulong)n;
            minSwaps = (ulong)(n / 2);
            maxSwaps = (ulong)(n / 2);
        }

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);
        await Assert.That(stats.IndexReadCount > 0).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be > 0");
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
        SpinSortVariant.Sort(random.AsSpan(), stats);

        // SpinSort for random data:
        // n ≤ 72: BinaryInsertionSort — O(n log n) comparisons, O(n²) writes.
        // n > 72: Full SpinSort — O(n log n) comparisons and writes.
        //         CheckPreSorted returns 0, then half-buffer + ping-pong merge.
        //
        // Observed ranges (20 seeds):
        // n=10:  Compares=[20..31],     Writes=[16..36],       Swaps=0
        // n=20:  Compares=[65..81],     Writes=[81..140],      Swaps=0
        // n=50:  Compares=[243..268],   Writes=[512..844],     Swaps=0
        // n=100: Compares=[831..933],   Writes=[933..1033],    Swaps=0
        ulong minCompares, maxCompares, minWrites, maxWrites;
        if (n <= 72)
        {
            // BinaryInsertionSort: O(n log n) comparisons, up to O(n²) writes
            minCompares = (ulong)(n - 1);
            maxCompares = (ulong)(n * Math.Ceiling(Math.Log2(n + 1)));
            minWrites = 0UL;
            maxWrites = (ulong)((long)n * n);
        }
        else
        {
            // Full SpinSort: O(n log n) comparisons and writes
            var logN = Math.Log2(n);
            minCompares = (ulong)(n * logN * 0.5);
            maxCompares = (ulong)(n * logN * 2.0);
            minWrites = (ulong)n;
            maxWrites = (ulong)(n * logN * 2.0);
        }

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount > 0).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be > 0");
        // SpinSort uses merges (reads + writes), not swaps — swaps only occur during Reverse in CheckPreSorted
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }
}
