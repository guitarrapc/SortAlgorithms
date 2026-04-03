using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class GlidesortTests
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

        Glidesort.Sort(array.AsSpan(), stats);

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

        Glidesort.Sort(array.AsSpan(), stats);

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

        Glidesort.Sort(array.AsSpan(), stats);

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

        Glidesort.Sort(array.AsSpan(), stats);

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

        Glidesort.Sort(array.AsSpan(), stats);

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

        Glidesort.Sort(items.AsSpan(), stats);

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

        Glidesort.Sort(items.AsSpan(), stats);

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

        Glidesort.Sort(items.AsSpan(), stats);

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
        Glidesort.Sort(array.AsSpan(), stats);

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
        Glidesort.Sort(sorted.AsSpan(), stats);

        // Glidesort for sorted data:
        // Small arrays (n < SMALL_SORT=32) use InsertionSort directly,
        // which detects already-sorted data with n-1 comparisons and 0 writes.
        // Larger arrays are detected as a single ascending run with n-1 comparisons.
        //
        // Actual observations for sorted data:
        // n=10:  9 comparisons, 0 writes, 0 swaps   (InsertionSort: already sorted)
        // n=20:  19 comparisons, 0 writes, 0 swaps   (InsertionSort: already sorted)
        // n=50:  49 comparisons, 0 writes, 0 swaps   (Single ascending run detected)
        // n=100: 99 comparisons, 0 writes, 0 swaps   (Single ascending run detected)
        //
        // Pattern: Always exactly n-1 comparisons (run detection scan), 0 writes, 0 swaps
        var expectedCompares = (ulong)(n - 1);

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexReadCount > 0).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be > 0");
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
        Glidesort.Sort(reversed.AsSpan(), stats);

        // Glidesort for reversed data:
        // Small arrays (n < SMALL_SORT=32) use InsertionSort: O(n²) comparisons and writes.
        // Larger arrays detect single strictly descending run and reverse via swaps: n-1 comparisons, n/2 swaps.
        //
        // Actual observations for reversed data:
        // n=10:  45 comparisons, 54 writes, 0 swaps   (InsertionSort: reversed causes max shifts)
        // n=20:  190 comparisons, 209 writes, 0 swaps  (InsertionSort: reversed causes max shifts)
        // n=50:  49 comparisons, 50 writes, 25 swaps   (Single descending run + reverse)
        // n=100: 99 comparisons, 100 writes, 50 swaps  (Single descending run + reverse)
        //
        // Pattern: For n < 32, InsertionSort O(n²); for n >= 32, run detection + reverse with n/2 swaps
        ulong minCompares, maxCompares, minWrites, maxWrites, minSwaps, maxSwaps;
        if (n < 32)
        {
            // InsertionSort: reversed data causes each element to shift all the way left
            minCompares = (ulong)n;
            maxCompares = (ulong)(n * (n - 1) / 2 + n);
            minWrites = (ulong)(n * (n - 1) / 4);
            maxWrites = (ulong)(n * (n + 1) / 2);
            minSwaps = 0UL;
            maxSwaps = 0UL; // InsertionSort doesn't use swaps
        }
        else
        {
            // Single descending run detected + reverse
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
        Glidesort.Sort(random.AsSpan(), stats);

        // Glidesort for random data:
        // Small arrays (n < SMALL_SORT=48) use InsertionSort at the top-level fast path.
        // Larger arrays use the powersort merge tree + stable quicksort for unsorted blocks.
        // Unsorted blocks (up to SMALL_SORT=48 elements) are sorted with BlockInsertionSort,
        // which uses Sort4/Sort8/Sort16/Sort32 sorting networks, then merged via powersort.
        //
        // Observed range for random data:
        // n=10:  ~29-37 comparisons, ~28-38 writes, 0 swaps   (InsertionSort top-level fast path)
        // n=20:  ~113-141 comparisons, ~114-140 writes, 0 swaps (InsertionSort top-level fast path)
        // n=50:  ~360-406 comparisons, ~408-472 writes, small swaps from Sort4 sorting network
        // n=100: ~785-891 comparisons, ~982-1105 writes, small swaps from Sort4 sorting network
        //
        // Pattern: approximately 1.0-2.0 * n * log₂(n) comparisons and writes
        // Swaps occur from Sort4 sorting network (up to 5 swaps per 4 elements) and
        // from reversing descending runs. Total Sort4 swaps ≤ 5*n/4 < 2*n.
        var logN = Math.Log2(n);
        ulong minCompares, maxCompares, minWrites, maxWrites;
        if (n < 48)
        {
            // InsertionSort top-level fast path: O(n) to O(n²)
            minCompares = (ulong)(n - 1);
            maxCompares = (ulong)(n * (n - 1) / 2 + n);
            minWrites = 0UL;
            maxWrites = (ulong)(n * (n + 1) / 2);
        }
        else
        {
            // Powersort merge tree + stable quicksort with BlockInsertionSort
            minCompares = (ulong)(n * logN * 0.5);
            maxCompares = (ulong)(n * logN * 2.5);
            minWrites = (ulong)(n * logN * 0.5);
            maxWrites = (ulong)(n * logN * 3.0);
        }

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount > 0).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be > 0");
        // Sort4 sorting network does up to 5 swaps per 4 elements → at most 5n/4 total < 2n.
        // For n < SMALL_SORT=48 (top-level InsertionSort): 0 swaps.
        await Assert.That(stats.SwapCount < (ulong)(2 * n)).IsTrue().Because($"SwapCount ({stats.SwapCount}) should be less than 2*n ({2 * n})");
    }
}
