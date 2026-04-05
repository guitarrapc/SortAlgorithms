using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class RotateMergeSortNonOptimizedTests
{
    [Test, SkipCI]
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
        Skip.When(inputSample.Samples.Length > 1024, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        RotateMergeSortNonOptimized.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateHalf))]
    public async Task SortHalfResultOrderTest(IInputSample<Half> inputSample)
    {
        Skip.When(inputSample.Samples.Length > 1024, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        RotateMergeSortNonOptimized.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateFloat))]
    public async Task SortFloatResultOrderTest(IInputSample<float> inputSample)
    {
        Skip.When(inputSample.Samples.Length > 1024, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        RotateMergeSortNonOptimized.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateDouble))]
    public async Task SortDoubleResultOrderTest(IInputSample<double> inputSample)
    {
        Skip.When(inputSample.Samples.Length > 1024, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        RotateMergeSortNonOptimized.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockIntKeyRandomData), nameof(MockIntKeyRandomData.Generate))]
    public async Task SortIntStructResultOrderTest(IInputSample<Utils.IntKey> inputSample)
    {
        Skip.When(inputSample.Samples.Length > 1024, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        RotateMergeSortNonOptimized.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test, SkipCI]
    [MethodDataSource(typeof(MockStabilityData), nameof(MockStabilityData.Generate))]
    public async Task StabilityTest(StabilityTestItem[] items)
    {
        // Test stability: equal elements should maintain relative order
        var stats = new StatisticsContext();

        RotateMergeSortNonOptimized.Sort(items.AsSpan(), stats);

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

    [Test, SkipCI]
    [MethodDataSource(typeof(MockStabilityWithIdData), nameof(MockStabilityWithIdData.Generate))]
    public async Task StabilityTestWithComplex(StabilityTestItemWithId[] items)
    {
        // Test stability with more complex scenario - multiple equal values
        var stats = new StatisticsContext();

        RotateMergeSortNonOptimized.Sort(items.AsSpan(), stats);

        // Expected: [2:B, 2:D, 2:F, 5:A, 5:C, 5:G, 8:E]
        // Keys are sorted, and elements with the same key maintain original order

        for (var i = 0; i < items.Length; i++)
        {
            await Assert.That(items[i].Key).IsEqualTo(MockStabilityWithIdData.Sorted[i].Key);
            await Assert.That(items[i].Id).IsEqualTo(MockStabilityWithIdData.Sorted[i].Id);
        }
    }

    [Test, SkipCI]
    [MethodDataSource(typeof(MockStabilityAllEqualsData), nameof(MockStabilityAllEqualsData.Generate))]
    public async Task StabilityTestWithAllEqual(StabilityTestItem[] items)
    {
        // Edge case: all elements have the same value
        // They should remain in original order
        var stats = new StatisticsContext();

        var originalOrder = items.Select(x => x.OriginalIndex).ToArray();

        RotateMergeSortNonOptimized.Sort(items.AsSpan(), stats);

        var resultOrder = items.Select(x => x.OriginalIndex).ToArray();
        await Assert.That(resultOrder).IsEquivalentTo(originalOrder, CollectionOrdering.Matching);
    }


    [Test, SkipCI]
    [MethodDataSource(typeof(MockSortedData), nameof(MockSortedData.Generate))]
    public async Task StatisticsSortedTest(IInputSample<int> inputSample)
    {
        if (inputSample.Samples.Length > 1024)
            return;

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        RotateMergeSortNonOptimized.Sort(array.AsSpan(), stats);

        await Assert.That((ulong)array.Length).IsEqualTo((ulong)inputSample.Samples.Length);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);  // Sorted data: no writes needed
        await Assert.That(stats.CompareCount).IsNotEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);  // Sorted data: no swaps needed
    }

    [Test, SkipCI]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        RotateMergeSortNonOptimized.Sort(sorted.AsSpan(), stats);

        // Rotate Merge Sort with optimization for sorted data:
        // With the "skip merge if already sorted" optimization,
        // sorted data only requires skip-check comparisons (one per recursive call).
        //
        // Theoretical bounds with optimization:
        // - Sorted data: n-1 comparisons (one skip-check per partition boundary)
        //   At each recursion level with k partitions, we do k-1 skip checks.
        //   Total: (n-1) comparisons for completely sorted data
        //
        // Actual observations with optimization for sorted data:
        // n=10:  9 comparisons    (n-1)
        // n=20:  19 comparisons   (n-1)
        // n=50:  49 comparisons   (n-1)
        // n=100: 99 comparisons   (n-1)
        //
        // Pattern for sorted data: n-1 comparisons (skip checks only)
        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(n);

        // Rotate Merge Sort writes with optimization:
        // For sorted data, merges are skipped, so writes = 0
        var minWrites = 0UL;
        var maxWrites = 0UL;

        // Reads for sorted data: Only skip-check comparisons
        // Each comparison reads 2 elements
        var minReads = stats.CompareCount * 2;

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount >= minReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minReads}");
        await Assert.That(stats.SwapCount).IsEqualTo(0UL); // Sorted data: no rotation needed
    }

    [Test, SkipCI]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesReversedTest(int n)
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        RotateMergeSortNonOptimized.Sort(reversed.AsSpan(), stats);

        // Rotate Merge Sort comparisons for reversed data:
        // Reversed data requires all merge operations with binary search + rotation.
        // n ≤ 16: entirely handled by InsertionSort
        // n > 16: divide-and-conquer merge with binary search + GCD-cycle rotation
        var logN = Math.Log2(n);
        var minCompares = n <= 16 ? (ulong)(n * 4.0) : (ulong)(n * logN * 0.8);
        var maxCompares = n <= 16 ? (ulong)(n * 5.5) : (ulong)(n * logN * 2.5);

        // Writes: GCD-cycle rotation uses Write operations only
        // n ≤ 16: InsertionSort writes only
        // n > 16: GCD-cycle rotation adds many writes
        var minWrites = n <= 16 ? (ulong)(n * 4.0) : (ulong)(n * logN * 1.4);
        var maxWrites = n <= 16 ? (ulong)(n * 6.0) : (ulong)(n * logN * 20.0);

        // Swaps: GCD-cycle rotation uses Write only, but divide-and-conquer merge
        // base case (len1==1 && len2==1) uses Swap. For n ≤ 16 InsertionSort does 0 swaps.
        var minSwaps = 0UL;
        var maxSwaps = n <= 16 ? 0UL : (ulong)(n * logN * 2.0);

        // IndexReads: Reduced due to InsertionSort optimization (caching values to reduce repeated reads)
        // Expected: approximately 1.2x comparisons (down from 2x)
        var minReads = (ulong)(stats.CompareCount * 1.2);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);
        await Assert.That(stats.IndexReadCount >= minReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minReads}");
    }

    [Test, SkipCI]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesRandomTest(int n)
    {
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        RotateMergeSortNonOptimized.Sort(random.AsSpan(), stats);

        // Rotate Merge Sort (NonOptimized) for random data:
        // This version uses divide-and-conquer merge (median of smaller side + binary search + rotate),
        // InsertionSort for small subarrays, and GCD-cycle rotation.
        //
        // For n ≤ 16: entirely handled by InsertionSort (no merge occurs)
        // For n > 16: divide-and-conquer merge with binary search + rotation
        //
        // Pattern for random: approximately 0.8 * n * log₂(n) to 2.5 * n * log₂(n) for n > 16
        // For n ≤ 16: InsertionSort profile applies
        var logN = Math.Log2(n);
        var minCompares = n <= 16 ? (ulong)(n * 1.0) : (ulong)(n * logN * 0.4);
        var maxCompares = n <= 16 ? (ulong)(n * 5.0) : (ulong)(n * logN * 2.5);

        // Writes vary based on how much rotation is needed
        // n ≤ 16: InsertionSort writes only, can be low for partially sorted data
        // n > 16: GCD-cycle rotation adds many writes
        var minWrites = n <= 16 ? (ulong)(n * 0.5) : (ulong)(n * logN * 0.5);
        var maxWrites = n <= 16 ? (ulong)(n * 5.0) : (ulong)(n * logN * 15.0);

        // Swaps: GCD-cycle rotation uses Write only; however the divide-and-conquer merge
        // base case (len1==1 && len2==1) uses Swap. For n ≤ 16 InsertionSort does 0 swaps.
        var minSwaps = 0UL;  // Could be low if data is partially sorted
        var maxSwaps = n <= 16 ? 0UL : (ulong)(n * logN * 8.0);

        // IndexReads: Reduced due to InsertionSort optimization (caching values to reduce repeated reads)
        // Expected: approximately 1.2x comparisons (down from 2x)
        var minReads = (ulong)(stats.CompareCount * 1.2);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);
        await Assert.That(stats.IndexReadCount >= minReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minReads}");
    }

}
