using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class PowerSortTests
{
    [Test]
    [MethodDataSource(typeof(MockRandomData), nameof(MockRandomData.Generate))]
    [MethodDataSource(typeof(MockNegativePositiveRandomData), nameof(MockNegativePositiveRandomData.Generate))]
    [MethodDataSource(typeof(MockNegativeRandomData), nameof(MockNegativeRandomData.Generate))]
    [MethodDataSource(typeof(MockReversedData), nameof(MockReversedData.Generate))]
    [MethodDataSource(typeof(MockPipeorganData), nameof(MockPipeorganData.Generate))]
    [MethodDataSource(typeof(MockNearlySortedData), nameof(MockNearlySortedData.Generate))]
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

        PowerSort.Sort(array.AsSpan(), stats);

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

        PowerSort.Sort(array.AsSpan(), stats);

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

        PowerSort.Sort(array.AsSpan(), stats);

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

        PowerSort.Sort(array.AsSpan(), stats);

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

        PowerSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockStabilityData), nameof(MockStabilityData.Generate))]
    public async Task StabilityTest(StabilityTestItem[] items)
    {
        // Test stability: equal elements should maintain relative order
        var stats = new StatisticsContext();

        PowerSort.Sort(items.AsSpan(), stats);

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

        PowerSort.Sort(items.AsSpan(), stats);

        // Expected: [2:B, 2:D, 2:F, 5:A, 5:C, 5:G, 8:E]
        // Keys are sorted, and elements with the same key maintain original order

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
        // Edge case: all elements have the same value
        // They should remain in original order
        var stats = new StatisticsContext();

        PowerSort.Sort(items.AsSpan(), stats);

        // All values are 1
        foreach (var item in items) await Assert.That(item.Value).IsEqualTo(1);

        // Original order should be preserved: 0, 1, 2, 3, 4
        await Assert.That(items.Select(x => x.OriginalIndex).ToArray()).IsEquivalentTo(MockStabilityAllEqualsData.Sorted, CollectionOrdering.Matching);
    }

#if DEBUG

    [Test]
    [MethodDataSource(typeof(MockSortedData), nameof(MockSortedData.Generate))]
    public async Task StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        PowerSort.Sort(array.AsSpan(), stats);

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
        PowerSort.Sort(sorted.AsSpan(), stats);

        // PowerSort for sorted data:
        // Small arrays (n < MIN_MERGE=32) use BinaryInsertSort directly,
        // which requires O(n log n) comparisons for binary search.
        // Larger arrays are detected as a single ascending run with O(n) comparisons.
        //
        // Actual observations for sorted data:
        // n=10:  19 comparisons   (BinaryInsertSort: binary search comparisons)
        // n=20:  54 comparisons   (BinaryInsertSort: binary search comparisons)
        // n=50:  50 comparisons   (Single run detection: n comparisons)
        // n=100: 100 comparisons  (Single run detection: n comparisons)
        //
        // Pattern: For n < 32, uses BinaryInsertSort; for n >= 32, run detection
        ulong minCompares, maxCompares;
        if (n < 32)
        {
            // BinaryInsertSort has variable comparisons due to binary search
            minCompares = (ulong)(n - 1);
            maxCompares = (ulong)(n * Math.Ceiling(Math.Log2(n + 1)));
        }
        else
        {
            // Single run detection
            minCompares = (ulong)(n - 1);
            maxCompares = (ulong)(n + 1);
        }

        // For sorted data, no writes needed (already sorted)
        var minWrites = 0UL;
        var maxWrites = 0UL;

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount > 0).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be > 0");
        await Assert.That(stats.SwapCount).IsEqualTo(0UL); // Sorted data doesn't need swaps
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
        PowerSort.Sort(reversed.AsSpan(), stats);

        // PowerSort for reversed data:
        // Small arrays (n < MIN_MERGE=32) use BinaryInsertSort: O(n²) writes, O(n log n) comparisons
        // Larger arrays detect single descending run, reverse it with swaps: O(n) comparisons, O(n/2) swaps
        //
        // Actual observations for reversed data:
        // n=10:  25 comparisons, 54 writes, 0 swaps   (BinaryInsertSort)
        // n=20:  69 comparisons, 209 writes, 0 swaps  (BinaryInsertSort)
        // n=50:  50 comparisons, 50 writes, 25 swaps  (Single descending run + reverse)
        // n=100: 100 comparisons, 100 writes, 50 swaps (Single descending run + reverse)
        //
        // Pattern: For n < 32, uses BinaryInsertSort; for n >= 32, run detection + reverse
        ulong minCompares, maxCompares, minWrites, maxWrites, minSwaps, maxSwaps;
        if (n < 32)
        {
            // BinaryInsertSort
            minCompares = (ulong)(n);
            maxCompares = (ulong)(n * Math.Ceiling(Math.Log2(n + 1)));
            minWrites = (ulong)(n * (n - 1) / 4); // At least n²/4 writes
            maxWrites = (ulong)(n * (n + 1) / 2); // Up to n²/2 writes
            minSwaps = 0UL;
            maxSwaps = 0UL; // BinaryInsertSort doesn't use swaps
        }
        else
        {
            // Single descending run + reverse
            minCompares = (ulong)(n - 1);
            maxCompares = (ulong)(n + 1);
            minWrites = (ulong)(n - 1);
            maxWrites = (ulong)(n + 1);
            minSwaps = (ulong)(n / 2 - 1);
            maxSwaps = (ulong)(n / 2 + 1);
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
        PowerSort.Sort(random.AsSpan(), stats);

        // PowerSort for random data:
        // Performance depends on natural run detection and merging.
        // Random data may contain some ascending/descending runs that are exploited.
        //
        // Observed range for random data (10 trial average):
        // n=10:  ~22 comparisons, ~30 writes, ~0 swaps
        // n=20:  ~63 comparisons, ~118 writes, ~0 swaps
        // n=50:  ~220 comparisons, ~416 writes, ~1 swap
        // n=100: ~542 comparisons, ~947 writes, ~2 swaps
        //
        // Pattern: approximately 0.4 * n * log₂(n) to 1.5 * n * log₂(n)
        // Swaps occur when descending runs are detected and reversed
        var logN = Math.Log2(n);
        var minCompares = (ulong)(n * logN * 0.4);
        var maxCompares = (ulong)(n * logN * 1.5);

        var minWrites = (ulong)(n * logN * 0.3);
        var maxWrites = (ulong)(n * logN * 2.5);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount > 0).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be > 0");
        // Random data may have descending runs that get reversed with swaps
        await Assert.That(stats.SwapCount < (ulong)(n / 4)).IsTrue().Because($"SwapCount ({stats.SwapCount}) should be less than n/4 ({n / 4})");
    }

#endif

}
