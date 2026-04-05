using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class DestswapStableQuickSortTests
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

        DestswapStableQuickSort.Sort(array.AsSpan(), stats);

        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateHalf))]
    public async Task SortHalfResultOrderTest(IInputSample<Half> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        DestswapStableQuickSort.Sort(array.AsSpan(), stats);

        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateFloat))]
    public async Task SortFloatResultOrderTest(IInputSample<float> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        DestswapStableQuickSort.Sort(array.AsSpan(), stats);

        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateDouble))]
    public async Task SortDoubleResultOrderTest(IInputSample<double> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        DestswapStableQuickSort.Sort(array.AsSpan(), stats);

        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockIntKeyRandomData), nameof(MockIntKeyRandomData.Generate))]
    public async Task SortIntStructResultOrderTest(IInputSample<Utils.IntKey> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        DestswapStableQuickSort.Sort(array.AsSpan(), stats);

        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    public async Task EdgeCaseEmptyArrayTest()
    {
        var stats = new StatisticsContext();
        var empty = Array.Empty<int>();
        DestswapStableQuickSort.Sort(empty.AsSpan(), stats);
    }

    [Test]
    public async Task EdgeCaseSingleElementTest()
    {
        var stats = new StatisticsContext();
        var single = new[] { 42 };
        DestswapStableQuickSort.Sort(single.AsSpan(), stats);

        await Assert.That(single[0]).IsEqualTo(42);
    }

    [Test]
    public async Task EdgeCaseTwoElementsSortedTest()
    {
        var stats = new StatisticsContext();
        var twoSorted = new[] { 1, 2 };
        DestswapStableQuickSort.Sort(twoSorted.AsSpan(), stats);

        await Assert.That(twoSorted).IsEquivalentTo([1, 2], CollectionOrdering.Matching);
    }

    [Test]
    public async Task EdgeCaseTwoElementsReversedTest()
    {
        var stats = new StatisticsContext();
        var twoReversed = new[] { 2, 1 };
        DestswapStableQuickSort.Sort(twoReversed.AsSpan(), stats);

        await Assert.That(twoReversed).IsEquivalentTo([1, 2], CollectionOrdering.Matching);
    }

    [Test]
    public async Task EdgeCaseThreeElementsTest()
    {
        var stats = new StatisticsContext();
        var three = new[] { 3, 1, 2 };
        DestswapStableQuickSort.Sort(three.AsSpan(), stats);

        await Assert.That(three).IsEquivalentTo([1, 2, 3], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        DestswapStableQuickSort.Sort(array.AsSpan(), 2, 6, stats);

        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        DestswapStableQuickSort.Sort(array.AsSpan(), 0, array.Length, stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task SortedArrayTest()
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(1, 100).ToArray();
        DestswapStableQuickSort.Sort(sorted.AsSpan(), stats);

        await Assert.That(sorted).IsEquivalentTo(Enumerable.Range(1, 100).ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    public async Task ReverseSortedArrayTest()
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(1, 100).Reverse().ToArray();
        DestswapStableQuickSort.Sort(reversed.AsSpan(), stats);

        await Assert.That(reversed).IsEquivalentTo(Enumerable.Range(1, 100).ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    public async Task AllEqualElementsTest()
    {
        var stats = new StatisticsContext();
        var allEqual = Enumerable.Repeat(42, 100).ToArray();
        DestswapStableQuickSort.Sort(allEqual.AsSpan(), stats);

        await Assert.That(allEqual).IsEquivalentTo(Enumerable.Repeat(42, 100).ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    public async Task ManyDuplicatesTest()
    {
        var stats = new StatisticsContext();
        var duplicates = new[] { 1, 2, 1, 3, 2, 1, 4, 3, 2, 1, 5, 4, 3, 2, 1 };
        DestswapStableQuickSort.Sort(duplicates.AsSpan(), stats);

        await Assert.That(duplicates).IsEquivalentTo([1, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 4, 4, 5], CollectionOrdering.Matching);
    }

    [Test]
    public async Task LargeArrayTest()
    {
        var stats = new StatisticsContext();
        var random = new Random(42);
        var large = Enumerable.Range(0, 10000).OrderBy(_ => random.Next()).ToArray();
        var expected = large.OrderBy(x => x).ToArray();

        DestswapStableQuickSort.Sort(large.AsSpan(), stats);

        await Assert.That(large).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task NearlySortedArrayTest()
    {
        var stats = new StatisticsContext();
        var nearlySorted = Enumerable.Range(1, 100).ToArray();
        (nearlySorted[10], nearlySorted[20]) = (nearlySorted[20], nearlySorted[10]);
        (nearlySorted[50], nearlySorted[60]) = (nearlySorted[60], nearlySorted[50]);

        DestswapStableQuickSort.Sort(nearlySorted.AsSpan(), stats);

        await Assert.That(nearlySorted).IsEquivalentTo(Enumerable.Range(1, 100).ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    public async Task StringSortTest()
    {
        var stats = new StatisticsContext();
        var strings = new[] { "zebra", "apple", "mango", "banana", "cherry" };
        DestswapStableQuickSort.Sort(strings.AsSpan(), stats);

        await Assert.That(strings).IsEquivalentTo(["apple", "banana", "cherry", "mango", "zebra"], CollectionOrdering.Matching);
    }

#if DEBUG

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        DestswapStableQuickSort.Sort(sorted.AsSpan(), stats);

        // DestswapStableQuickSort on sorted data:
        // - For n <= SMALL_SORT (16): Uses BottomUp merge sort fallback
        //   Sorted data: O(n) comparisons (efficient merge on already-sorted runs)
        // - For n > SMALL_SORT: Uses DestSwap 2-way stable quicksort
        //   Writes directly to destination; no copy-back per recursion level
        //   Sorted data with adaptive pivot selection: O(n log n) comparisons
        // - No Swap operations: DestSwap uses Read/Write/CopyTo directly
        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(n * n);

        // DestswapStableQuickSort does not use Swap at all
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);

        var minIndexReads = (ulong)n;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        var minIndexWrites = (ulong)n;
        await Assert.That(stats.IndexWriteCount >= minIndexWrites).IsTrue().Because($"IndexWriteCount ({stats.IndexWriteCount}) should be >= {minIndexWrites}");
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
        DestswapStableQuickSort.Sort(reversed.AsSpan(), stats);

        // DestswapStableQuickSort on reversed data:
        // - For n <= SMALL_SORT (16): BottomUp fallback; O(n log n) comparisons
        // - For n > SMALL_SORT: DestSwap QuickSort; adaptive strategy handles reversed patterns
        // - No Swap operations
        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(n * n);

        await Assert.That(stats.SwapCount).IsEqualTo(0UL);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);

        var minIndexReads = (ulong)n;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        var minIndexWrites = (ulong)n;
        await Assert.That(stats.IndexWriteCount >= minIndexWrites).IsTrue().Because($"IndexWriteCount ({stats.IndexWriteCount}) should be >= {minIndexWrites}");
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
        DestswapStableQuickSort.Sort(random.AsSpan(), stats);

        // DestswapStableQuickSort on random data:
        // - Average case O(n log n) with adaptive pivot selection
        // - No Swap operations: uses DestSwap (Read/Write/CopyTo)
        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(n * n);

        await Assert.That(stats.SwapCount).IsEqualTo(0UL);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);

        var minIndexReads = (ulong)n;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        var minIndexWrites = (ulong)n;
        await Assert.That(stats.IndexWriteCount >= minIndexWrites).IsTrue().Because($"IndexWriteCount ({stats.IndexWriteCount}) should be >= {minIndexWrites}");
    }

    [Test]
    [Arguments(10, 9)]    // n <= SMALL_SORT=16: BottomUp fallback, sorted data → n-1=9 comparisons
    [Arguments(20, 41)]   // n > SMALL_SORT: DestSwap QuickSort
    [Arguments(50, 178)]  // n > SMALL_SORT: DestSwap QuickSort
    [Arguments(100, 469)] // n > SMALL_SORT: DestSwap QuickSort
    public async Task TheoreticalComparisonCountTest(int n, int expectedComparisons)
    {
        // Test the exact comparison count for sorted data.
        // DestswapStableQuickSort on sorted data is fully deterministic:
        // - n <= SMALL_SORT (16): BottomUp merge sort → efficient merge on already-sorted runs
        // - n > SMALL_SORT: DestSwap 2-way QuickSort with adaptive pivot selection
        // No Swap operations in any case.
        //
        // Empirical values for sorted data:
        // n=10:  Compare=9   (BottomUp fallback: n-1)
        // n=20:  Compare=41
        // n=50:  Compare=178
        // n=100: Compare=469
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        DestswapStableQuickSort.Sort(sorted.AsSpan(), stats);

        await Assert.That(stats.CompareCount).IsEqualTo((ulong)expectedComparisons);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexReadCount >= stats.CompareCount).IsTrue()
            .Because($"IndexReadCount ({stats.IndexReadCount}) should be >= CompareCount ({stats.CompareCount})");
    }

#endif
}
