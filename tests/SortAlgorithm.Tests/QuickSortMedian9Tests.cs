using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class QuickSortMedian9Tests
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

        QuickSortMedian9.Sort(array.AsSpan(), stats);

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

        QuickSortMedian9.Sort(array.AsSpan(), stats);

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

        QuickSortMedian9.Sort(array.AsSpan(), stats);

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

        QuickSortMedian9.Sort(array.AsSpan(), stats);

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

        QuickSortMedian9.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    public async Task EdgeCaseEmptyArrayTest()
    {
        var stats = new StatisticsContext();
        var empty = Array.Empty<int>();
        QuickSortMedian9.Sort(empty.AsSpan(), stats);
    }

    [Test]
    public async Task EdgeCaseSingleElementTest()
    {
        var stats = new StatisticsContext();
        var single = new[] { 42 };
        QuickSortMedian9.Sort(single.AsSpan(), stats);

        await Assert.That(single[0]).IsEqualTo(42);
    }

    [Test]
    public async Task EdgeCaseTwoElementsSortedTest()
    {
        var stats = new StatisticsContext();
        var twoSorted = new[] { 1, 2 };
        QuickSortMedian9.Sort(twoSorted.AsSpan(), stats);

        await Assert.That(twoSorted).IsEquivalentTo([1, 2], CollectionOrdering.Matching);
    }

    [Test]
    public async Task EdgeCaseTwoElementsReversedTest()
    {
        var stats = new StatisticsContext();
        var twoReversed = new[] { 2, 1 };
        QuickSortMedian9.Sort(twoReversed.AsSpan(), stats);

        await Assert.That(twoReversed).IsEquivalentTo([1, 2], CollectionOrdering.Matching);
    }

    [Test]
    public async Task EdgeCaseThreeElementsTest()
    {
        var stats = new StatisticsContext();
        var three = new[] { 3, 1, 2 };
        QuickSortMedian9.Sort(three.AsSpan(), stats);

        await Assert.That(three).IsEquivalentTo([1, 2, 3], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        QuickSortMedian9.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6 ], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        QuickSortMedian9.Sort(array.AsSpan(), 0, array.Length, stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task SortedArrayTest()
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(1, 100).ToArray();
        QuickSortMedian9.Sort(sorted.AsSpan(), stats);

        await Assert.That(sorted).IsEquivalentTo(Enumerable.Range(1, 100).ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    public async Task ReverseSortedArrayTest()
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(1, 100).Reverse().ToArray();
        QuickSortMedian9.Sort(reversed.AsSpan(), stats);

        await Assert.That(reversed).IsEquivalentTo(Enumerable.Range(1, 100).ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    public async Task AllEqualElementsTest()
    {
        var stats = new StatisticsContext();
        var allEqual = Enumerable.Repeat(42, 100).ToArray();
        QuickSortMedian9.Sort(allEqual.AsSpan(), stats);

        await Assert.That(allEqual).IsEquivalentTo(Enumerable.Repeat(42, 100).ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    public async Task ManyDuplicatesTest()
    {
        var stats = new StatisticsContext();
        var duplicates = new[] { 1, 2, 1, 3, 2, 1, 4, 3, 2, 1, 5, 4, 3, 2, 1 };
        QuickSortMedian9.Sort(duplicates.AsSpan(), stats);

        await Assert.That(duplicates).IsEquivalentTo([1, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 4, 4, 5 ], CollectionOrdering.Matching);
    }

    [Test]
    public async Task LargeArrayTest()
    {
        var stats = new StatisticsContext();
        var random = new Random(42);
        var large = Enumerable.Range(0, 10000).OrderBy(_ => random.Next()).ToArray();
        var expected = large.OrderBy(x => x).ToArray();

        QuickSortMedian9.Sort(large.AsSpan(), stats);

        await Assert.That(large).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task NearlySortedArrayTest()
    {
        var stats = new StatisticsContext();
        var nearlySorted = Enumerable.Range(1, 100).ToArray();
        // Swap a few elements to make it nearly sorted
        (nearlySorted[10], nearlySorted[20]) = (nearlySorted[20], nearlySorted[10]);
        (nearlySorted[50], nearlySorted[60]) = (nearlySorted[60], nearlySorted[50]);

        QuickSortMedian9.Sort(nearlySorted.AsSpan(), stats);

        await Assert.That(nearlySorted).IsEquivalentTo(Enumerable.Range(1, 100).ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    public async Task SmallArrayInsertionSortThresholdTest()
    {
        var stats = new StatisticsContext();
        var small = new[] { 5, 2, 8, 1, 9, 3, 7, 4, 6, 10, 15, 12, 18, 11, 19, 13, 17, 14, 16, 20 };
        QuickSortMedian9.Sort(small.AsSpan(), stats);

        await Assert.That(small).IsEquivalentTo(Enumerable.Range(1, 20).ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    public async Task StringSortTest()
    {
        var stats = new StatisticsContext();
        var strings = new[] { "zebra", "apple", "mango", "banana", "cherry" };
        QuickSortMedian9.Sort(strings.AsSpan(), stats);

        await Assert.That(strings).IsEquivalentTo(["apple", "banana", "cherry", "mango", "zebra"], CollectionOrdering.Matching);
    }

#if DEBUG

    [Test]
    [MethodDataSource(typeof(MockSortedData), nameof(MockSortedData.Generate))]
    public async Task StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        QuickSortMedian9.Sort(array.AsSpan(), stats);

        await Assert.That((ulong)array.Length).IsEqualTo((ulong)inputSample.Samples.Length);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsNotEqualTo(0UL);
        await Assert.That(stats.CompareCount).IsNotEqualTo(0UL);
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
        QuickSortMedian9.Sort(sorted.AsSpan(), stats);

        // QuickSort Median-of-9 with Hoare partition on sorted data:
        // - Median-of-9 selects median of (left, mid, right) as pivot
        // - For sorted data, median of (first, middle, last) is the middle element
        // - Hoare partition performs bidirectional scanning
        //
        // Best case complexity analysis:
        // - Each partition divides array roughly in half
        // - Recursion depth: O(log n)
        // - At each level, all n elements are processed
        //
        // Comparisons:
        // - Median-of-9: 2-3 comparisons per partition call
        // - Hoare partition: Each element compared with pivot once
        // - Total: approximately 2n log n comparisons
        //
        // Swaps:
        // - For sorted data, Hoare partition still performs swaps when l <= r
        // - Even when elements are in correct relative positions, swaps occur
        // - Total: approximately 0.5n log n swaps

        var minCompares = (ulong)(n); // At minimum, some comparisons occur
        var maxCompares = (ulong)(3 * n * Math.Log(n, 2)); // Upper bound

        // For sorted data with Hoare partition, swaps still occur
        var minSwaps = 0UL;
        var maxSwaps = (ulong)(n * Math.Log(n, 2)); // Upper bound

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);

        // IndexReads: Each comparison reads elements, each swap reads and writes
        var minIndexReads = stats.CompareCount;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        // IndexWrites: Each swap writes 2 elements
        var minIndexWrites = stats.SwapCount * 2;
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
        QuickSortMedian9.Sort(reversed.AsSpan(), stats);

        // QuickSort Median-of-9 with Hoare partition on reversed data:
        // - Median-of-9 helps avoid worst-case by selecting better pivots
        // - For reversed data, median of (first, middle, last) is still reasonable
        // - Hoare partition is more efficient than Lomuto for reversed data
        //
        // Average case complexity (Median-of-9 avoids O(n²)):
        // - Recursion depth: O(log n) on average
        // - Comparisons: approximately 1.386n log₂ n
        // - Swaps: approximately 0.33n log₂ n
        //
        // For reversed data specifically:
        // - Most elements need to be swapped during partitioning
        // - More swaps than sorted data, but still O(n log n)

        var minCompares = (ulong)(n);
        var maxCompares = (ulong)(3 * n * Math.Log(n, 2));

        var minSwaps = (ulong)(n / 4); // At least some swaps needed
        var maxSwaps = (ulong)(2 * n * Math.Log(n, 2)); // Allow for more swaps

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);

        // IndexReads and IndexWrites should be proportional to operations
        var minIndexReads = stats.CompareCount;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        var minIndexWrites = stats.SwapCount * 2;
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
        QuickSortMedian9.Sort(random.AsSpan(), stats);

        // QuickSort Median-of-9 with Hoare partition on random data:
        // - Average case O(n log n) complexity
        // - Median-of-9 pivot selection provides good partitioning
        // - Hoare partition performs approximately 1.386n log₂ n comparisons
        // - Swaps: approximately 0.33n log₂ n on average

        var minCompares = (ulong)(n);
        var maxCompares = (ulong)(3 * n * Math.Log(n, 2) + 2);

        var minSwaps = 0UL;
        var maxSwaps = (ulong)(2 * n * Math.Log(n, 2));

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);

        var minIndexReads = stats.CompareCount;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        var minIndexWrites = stats.SwapCount * 2;
        await Assert.That(stats.IndexWriteCount >= minIndexWrites).IsTrue().Because($"IndexWriteCount ({stats.IndexWriteCount}) should be >= {minIndexWrites}");
    }

    [Test]
    [Arguments(5)]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    public async Task TheoreticalValuesSameElementsTest(int n)
    {
        var stats = new StatisticsContext();
        var sameValues = Enumerable.Repeat(42, n).ToArray();
        QuickSortMedian9.Sort(sameValues.AsSpan(), stats);

        // QuickSort Median-of-9 with Hoare partition on all equal elements:
        // - Median-of-9: all three sampled elements are equal
        // - Pivot equals all elements in the array
        // - Hoare partition: elements equal to pivot are distributed between partitions
        // - This causes many swaps even though all elements are equal
        //
        // Expected behavior:
        // - Comparisons: O(n log n) - partitioning still occurs at each level
        // - Swaps: O(n log n) - Hoare partition swaps elements even when equal to pivot
        //   This is a known characteristic: condition is "l <= r" not "l < r"
        // - The algorithm still terminates correctly

        var minCompares = (ulong)(n - 1); // At minimum, some comparisons
        var maxCompares = (ulong)(3 * n * Math.Log(Math.Max(n, 2), 2));

        var minSwaps = 0UL;
        var maxSwaps = (ulong)(2 * n * Math.Log(Math.Max(n, 2), 2));

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);

        // Verify array is still correct (all values unchanged)
        foreach(var item in sameValues) await Assert.That(item).IsEqualTo(42);

        var minIndexReads = stats.CompareCount;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        var minIndexWrites = stats.SwapCount * 2;
        await Assert.That(stats.IndexWriteCount >= minIndexWrites).IsTrue().Because($"IndexWriteCount ({stats.IndexWriteCount}) should be >= {minIndexWrites}");
    }

#endif

}
