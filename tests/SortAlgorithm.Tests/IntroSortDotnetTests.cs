using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class IntroSortDotnetTests
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

        IntroSortDotnet.Sort(array.AsSpan(), stats);

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

        IntroSortDotnet.Sort(array.AsSpan(), stats);

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

        IntroSortDotnet.Sort(array.AsSpan(), stats);

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

        IntroSortDotnet.Sort(array.AsSpan(), stats);

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

        IntroSortDotnet.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    public async Task EdgeCaseEmptyArrayTest()
    {
        var stats = new StatisticsContext();
        var empty = Array.Empty<int>();
        IntroSortDotnet.Sort(empty.AsSpan(), stats);
    }

    [Test]
    public async Task EdgeCaseSingleElementTest()
    {
        var stats = new StatisticsContext();
        var single = new[] { 42 };
        IntroSortDotnet.Sort(single.AsSpan(), stats);

        await Assert.That(single[0]).IsEqualTo(42);
    }

    [Test]
    public async Task EdgeCaseTwoElementsSortedTest()
    {
        var stats = new StatisticsContext();
        var twoSorted = new[] { 1, 2 };
        IntroSortDotnet.Sort(twoSorted.AsSpan(), stats);

        await Assert.That(twoSorted).IsEquivalentTo([1, 2], CollectionOrdering.Matching);
    }

    [Test]
    public async Task EdgeCaseTwoElementsReversedTest()
    {
        var stats = new StatisticsContext();
        var twoReversed = new[] { 2, 1 };
        IntroSortDotnet.Sort(twoReversed.AsSpan(), stats);

        await Assert.That(twoReversed).IsEquivalentTo([1, 2], CollectionOrdering.Matching);
    }

    [Test]
    public async Task EdgeCaseThreeElementsTest()
    {
        var stats = new StatisticsContext();
        var three = new[] { 3, 1, 2 };
        IntroSortDotnet.Sort(three.AsSpan(), stats);

        await Assert.That(three).IsEquivalentTo([1, 2, 3], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        IntroSortDotnet.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        IntroSortDotnet.Sort(array.AsSpan(), 0, array.Length, stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task SortedArrayTest()
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(1, 100).ToArray();
        IntroSortDotnet.Sort(sorted.AsSpan(), stats);

        await Assert.That(sorted).IsEquivalentTo(Enumerable.Range(1, 100).ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    public async Task ReverseSortedArrayTest()
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(1, 100).Reverse().ToArray();
        IntroSortDotnet.Sort(reversed.AsSpan(), stats);

        await Assert.That(reversed).IsEquivalentTo(Enumerable.Range(1, 100).ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    public async Task AllEqualElementsTest()
    {
        var stats = new StatisticsContext();
        var allEqual = Enumerable.Repeat(42, 100).ToArray();
        IntroSortDotnet.Sort(allEqual.AsSpan(), stats);

        await Assert.That(allEqual).IsEquivalentTo(Enumerable.Repeat(42, 100).ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    public async Task ManyDuplicatesTest()
    {
        var stats = new StatisticsContext();
        var duplicates = new[] { 1, 2, 1, 3, 2, 1, 4, 3, 2, 1, 5, 4, 3, 2, 1 };
        IntroSortDotnet.Sort(duplicates.AsSpan(), stats);

        await Assert.That(duplicates).IsEquivalentTo([1, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 4, 4, 5], CollectionOrdering.Matching);
    }

    [Test]
    public async Task LargeArrayTest()
    {
        var stats = new StatisticsContext();
        var random = new Random(42);
        var large = Enumerable.Range(0, 10000).OrderBy(_ => random.Next()).ToArray();
        var expected = large.OrderBy(x => x).ToArray();

        IntroSortDotnet.Sort(large.AsSpan(), stats);

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

        IntroSortDotnet.Sort(nearlySorted.AsSpan(), stats);

        await Assert.That(nearlySorted).IsEquivalentTo(Enumerable.Range(1, 100).ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    public async Task SmallArrayInsertionSortThresholdTest()
    {
        var stats = new StatisticsContext();
        var small = new[] { 5, 2, 8, 1, 9, 3, 7, 4, 6, 10, 15, 12, 18, 11, 19, 13, 17, 14, 16, 20 };
        IntroSortDotnet.Sort(small.AsSpan(), stats);

        await Assert.That(small).IsEquivalentTo(Enumerable.Range(1, 20).ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    public async Task StringSortTest()
    {
        var stats = new StatisticsContext();
        var strings = new[] { "zebra", "apple", "mango", "banana", "cherry" };
        IntroSortDotnet.Sort(strings.AsSpan(), stats);

        await Assert.That(strings).IsEquivalentTo(["apple", "banana", "cherry", "mango", "zebra"], CollectionOrdering.Matching);
    }

#if DEBUG

    [Test]
    [MethodDataSource(typeof(MockSortedData), nameof(MockSortedData.Generate))]
    public async Task StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        IntroSortDotnet.Sort(array.AsSpan(), stats);

        await Assert.That((ulong)array.Length).IsEqualTo((ulong)inputSample.Samples.Length);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(stats.CompareCount).IsNotEqualTo(0UL);
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
        IntroSortDotnet.Sort(sorted.AsSpan(), stats);

        // IntroSortDotnet behavior on sorted data:
        // - For small arrays (n ≤ 16): Uses InsertionSort
        //   - Best case O(n): n-1 comparisons, 0 swaps
        // - For larger arrays (n > 16): Uses QuickSort with median-of-three pivot
        //   - For sorted data, InsertionSort is still used for final small partitions
        //   - Comparisons include: median-of-3 selections + partitioning + InsertionSort for small partitions
        //   - Swaps may be minimal due to sorted nature

        ulong minCompares, maxCompares, minSwaps, maxSwaps;

        // Note: Even for n > 16, the entire array may still go through InsertionSort
        // if the initial partition immediately becomes small enough
        // For sorted data specifically, partitioning creates small partitions quickly

        // Conservative bounds that work for both InsertionSort and QuickSort paths
        minCompares = (ulong)(n - 1); // Minimum: at least n-1 comparisons
        maxCompares = (ulong)(3 * n * Math.Max(1, Math.Log(n, 2))); // Upper bound for QuickSort
        minSwaps = 0UL; // Sorted data may need no swaps
        maxSwaps = (ulong)(n * Math.Max(1, Math.Log(n, 2))); // Upper bound

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);

        // IndexReads: Each comparison reads elements, each swap reads and writes
        var minIndexReads = stats.CompareCount;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
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
        IntroSortDotnet.Sort(reversed.AsSpan(), stats);

        // IntroSortDotnet behavior on reversed data:
        // - For small arrays (n ≤ 16): Uses InsertionSort
        //   - Worst case O(n²): n(n-1)/2 comparisons, (n-1)(n+2)/2 writes
        // - For larger arrays (n > 16): Uses QuickSort with median-of-three pivot
        //   - Partitioning occurs, then InsertionSort for final small partitions
        //   - Overall: O(n log n) behavior

        ulong minCompares, maxCompares;

        // Conservative bounds that handle both paths
        if (n <= 16)
        {
            // InsertionSort worst case for small arrays
            minCompares = (ulong)(n * (n - 1) / 2);
            maxCompares = (ulong)(n * (n - 1) / 2);
        }
        else
        {
            // Larger arrays: combination of QuickSort partitioning + InsertionSort for small partitions
            minCompares = (ulong)(n);
            maxCompares = (ulong)(n * n); // Allow for worst-case InsertionSort on partitions
        }

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);

        // IndexReads and IndexWrites should be proportional to operations
        var minIndexReads = stats.CompareCount;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
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
        IntroSortDotnet.Sort(random.AsSpan(), stats);

        // IntroSortDotnet behavior on random data:
        // - For small arrays (n ≤ 16): Uses InsertionSort
        //   - Average case O(n²): approximately n²/4 comparisons
        // - For larger arrays: Uses QuickSort + InsertionSort for small partitions
        //   - Combined complexity: O(n log n) with InsertionSort overhead

        // Conservative bounds for both paths
        ulong minCompares = (ulong)(n - 1); // Minimum: at least n-1 comparisons
        ulong maxCompares = (ulong)(n * n); // Allow for InsertionSort worst-case on partitions

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);

        var minIndexReads = stats.CompareCount;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
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
        IntroSortDotnet.Sort(sameValues.AsSpan(), stats);

        // IntroSortDotnet behavior on same elements:
        // - For small arrays (n ≤ 16): Uses InsertionSort
        //   - Best case O(n): n-1 comparisons (all equal, no shifts)
        // - For larger arrays: Uses QuickSort + InsertionSort
        //   - All elements equal to pivot
        //   - Hoare partition may still swap elements (l ≤ r condition)
        //   - Combined complexity varies

        // Conservative bounds for all cases
        ulong minCompares = (ulong)(n - 1);
        ulong maxCompares = (ulong)(n * Math.Max(1, (int)Math.Log(n, 2)) * 3);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);

        // Verify array is still correct (all values unchanged)
        foreach (var item in sameValues) await Assert.That(item).IsEqualTo(42);

        var minIndexReads = stats.CompareCount;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
    }

#endif

}
