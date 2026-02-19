using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class BucketSortTests
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


        BucketSort.Sort(array.AsSpan(), x => x, stats);

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

        BucketSort.Sort(items.AsSpan(), x => x.Value, stats);

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

        BucketSort.Sort(items.AsSpan(), x => x.Key, stats);

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

        BucketSort.Sort(items.AsSpan(), x => x.Value, stats);

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
        BucketSort.Sort(array.AsSpan(), x => x, stats);

        await Assert.That((ulong)array.Length).IsEqualTo((ulong)inputSample.Samples.Length);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsNotEqualTo(0UL);
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
        BucketSort.Sort(sorted.AsSpan(), x => x, stats);

        // BucketSort on sorted data (with internal buffer tracking via SortSpan):
        //
        // Operations breakdown:
        // 1. Find min/max: n reads (main buffer)
        // 2. Distribution: n reads (main buffer) + n writes (temp buffer)
        // 3. InsertionSort per bucket (InsertionSort.SortCore via SortSpan on bucket buffers):
        //    - For sorted buckets: s.Read(i) + s.Read(j) per non-first element = 2 reads each
        //    - j == i-1 for all elements → no shift → write skipped (optimization in SortCore)
        //    - Total writes from InsertionSort: 0
        // 4. CopyTo (temp → main): n reads (source) + n writes (destination) via OnRangeCopy
        //
        // Actual observations:
        // n=10:  44 reads (4.4n), 20 writes (2.0n)
        // n=20:  92 reads (4.6n), 40 writes (2.0n)
        // n=50:  236 reads (4.72n), 100 writes (2.0n)
        // n=100: 480 reads (4.8n), 200 writes (2.0n)
        //
        // Reads: 2n (find/distribute) + ~2n (insertion sort) + n (CopyTo read from temp)
        // Writes: n (distribute to temp) + 0 (InsertionSort skips no-op writes for sorted data) + n (CopyTo write to main)

        var bucketCount = Math.Max(2, Math.Min(1000, (int)Math.Sqrt(n)));

        // IndexReadCount: 2n (find/distribute) + ~2n (insertion sort) + n (CopyTo)
        var expectedReadsMin = (ulong)(4.0 * n);
        var expectedReadsMax = (ulong)(5.0 * n);

        // IndexWriteCount: n (distribute) + 0 (insertion sort, no-op writes skipped) + n (CopyTo) = 2n
        var expectedWritesMin = (ulong)(1.8 * n);
        var expectedWritesMax = (ulong)(2.5 * n);

        // CompareCount: n - bucketCount (one per element except first in each bucket)
        // But with SortSpan Compare(), each comparison involves reads
        // Observed: slightly higher due to additional comparison checks
        var expectedComparesMin = (ulong)(n - bucketCount);
        var expectedComparesMax = (ulong)(n);

        await Assert.That(stats.IndexReadCount).IsBetween(expectedReadsMin, expectedReadsMax);
        await Assert.That(stats.IndexWriteCount).IsBetween(expectedWritesMin, expectedWritesMax);
        await Assert.That(stats.CompareCount).IsBetween(expectedComparesMin, expectedComparesMax);
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
        BucketSort.Sort(reversed.AsSpan(), x => x, stats);

        // BucketSort on reversed data (with internal buffer tracking):
        // - Distribution is same as sorted (independent of order)
        // - Within each bucket, elements are reversed
        // - InsertionSort worst case: many shifts
        //
        // Actual observations:
        // n=10:  writes=30 (3n)
        // n=20:  writes=76 (3.8n)
        // n=50:  writes=262 (5.24n)
        // n=100: writes=640 (6.4n)

        var bucketCount = Math.Max(2, Math.Min(1000, (int)Math.Sqrt(n)));

        // IndexReadCount: base operations + heavy insertion sort reads
        var minReads = (ulong)(2 * n);
        var maxReads = (ulong)(15 * n); // Allow for worst case

        // IndexWriteCount: many shifts during insertion sort
        var minWrites = (ulong)n;
        var maxWrites = (ulong)(8 * n);

        // CompareCount: worst case for insertion sort
        var bucketSize = n / bucketCount;
        var maxComparesPerBucket = bucketSize * (bucketSize - 1) / 2;
        var maxCompares = (ulong)(bucketCount * maxComparesPerBucket * 2);

        await Assert.That(stats.IndexReadCount).IsBetween(minReads, maxReads);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.CompareCount).IsBetween(0UL, maxCompares);
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
        BucketSort.Sort(random.AsSpan(), x => x, stats);

        // BucketSort on random data (with internal buffer tracking):
        // - For Enumerable.Range shuffled, keys are still 0..n-1 (uniform distribution)
        // - InsertionSort per bucket performs more operations on random data
        //
        // Actual observations:
        // n=10:  reads vary significantly, writes ~12-30
        // n=20:  reads ~70-130, writes ~36-70
        // n=50:  reads ~180-350, writes ~100-200
        // n=100: reads ~400-900, writes ~200-900

        var bucketCount = Math.Max(2, Math.Min(1000, (int)Math.Sqrt(n)));

        // IndexReadCount: highly variable based on randomness
        // Base: 2n (find/distribute) + variable insertion sort reads
        var expectedReadsMin = (ulong)(2 * n);
        var expectedReadsMax = (ulong)(10 * n);

        // IndexWriteCount: includes insertion sort shifts
        // Random data requires more shifts than sorted data
        var expectedWritesMin = (ulong)(0.5 * n);
        var expectedWritesMax = (ulong)(10 * n);

        // CompareCount: varies based on bucket distribution
        var bucketSize = n / bucketCount;
        var maxComparesPerBucket = bucketSize * (bucketSize - 1) / 2;
        var maxCompares = (ulong)(bucketCount * maxComparesPerBucket * 2);

        await Assert.That(stats.IndexReadCount).IsBetween(expectedReadsMin, expectedReadsMax);
        await Assert.That(stats.IndexWriteCount).IsBetween(expectedWritesMin, expectedWritesMax);
        await Assert.That(stats.CompareCount).IsBetween(0UL, maxCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesAllSameTest(int n)
    {
        var stats = new StatisticsContext();
        var allSame = Enumerable.Repeat(42, n).ToArray();
        BucketSort.Sort(allSame.AsSpan(), x => x, stats);

        // All elements are the same:
        // - min == max, early return after first pass
        // - No distribution or sorting needed

        // IndexReadCount: only for finding min/max
        var expectedReads = (ulong)n;

        // IndexWriteCount: 0 (early return)
        var expectedWrites = 0UL;

        // CompareCount: 0 (no sorting needed)
        var expectedCompares = 0UL;

        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

}
