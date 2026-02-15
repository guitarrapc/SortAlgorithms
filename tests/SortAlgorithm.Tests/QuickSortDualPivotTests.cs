using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class QuickSortDualPivotTests
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

        QuickSortDualPivot.Sort(array.AsSpan(), stats);

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

        QuickSortDualPivot.Sort(array.AsSpan(), stats);

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

        QuickSortDualPivot.Sort(array.AsSpan(), stats);

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

        QuickSortDualPivot.Sort(array.AsSpan(), stats);

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

        QuickSortDualPivot.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    public async Task EdgeCaseEmptyArrayTest()
    {
        var stats = new StatisticsContext();
        var empty = Array.Empty<int>();
        QuickSortDualPivot.Sort(empty.AsSpan(), stats);
    }

    [Test]
    public async Task EdgeCaseSingleElementTest()
    {
        var stats = new StatisticsContext();
        var single = new[] { 42 };
        QuickSortDualPivot.Sort(single.AsSpan(), stats);

        await Assert.That(single[0]).IsEqualTo(42);
    }

    [Test]
    public async Task EdgeCaseTwoElementsSortedTest()
    {
        var stats = new StatisticsContext();
        var twoSorted = new[] { 1, 2 };
        QuickSortDualPivot.Sort(twoSorted.AsSpan(), stats);

        await Assert.That(twoSorted).IsEquivalentTo([1, 2], CollectionOrdering.Matching);
    }

    [Test]
    public async Task EdgeCaseTwoElementsReversedTest()
    {
        var stats = new StatisticsContext();
        var twoReversed = new[] { 2, 1 };
        QuickSortDualPivot.Sort(twoReversed.AsSpan(), stats);

        await Assert.That(twoReversed).IsEquivalentTo([1, 2], CollectionOrdering.Matching);
    }

    [Test]
    public async Task EdgeCaseThreeElementsTest()
    {
        var stats = new StatisticsContext();
        var three = new[] { 3, 1, 2 };
        QuickSortDualPivot.Sort(three.AsSpan(), stats);

        await Assert.That(three).IsEquivalentTo([1, 2, 3], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        QuickSortDualPivot.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6 ], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        QuickSortDualPivot.Sort(array.AsSpan(), 0, array.Length, stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task SortedArrayTest()
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(1, 100).ToArray();
        QuickSortDualPivot.Sort(sorted.AsSpan(), stats);

        await Assert.That(sorted).IsEquivalentTo(Enumerable.Range(1, 100).ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    public async Task ReverseSortedArrayTest()
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(1, 100).Reverse().ToArray();
        QuickSortDualPivot.Sort(reversed.AsSpan(), stats);

        await Assert.That(reversed).IsEquivalentTo(Enumerable.Range(1, 100).ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    public async Task AllEqualElementsTest()
    {
        var stats = new StatisticsContext();
        var allEqual = Enumerable.Repeat(42, 100).ToArray();
        QuickSortDualPivot.Sort(allEqual.AsSpan(), stats);

        await Assert.That(allEqual).IsEquivalentTo(Enumerable.Repeat(42, 100).ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    public async Task ManyDuplicatesTest()
    {
        var stats = new StatisticsContext();
        var duplicates = new[] { 1, 2, 1, 3, 2, 1, 4, 3, 2, 1, 5, 4, 3, 2, 1 };
        QuickSortDualPivot.Sort(duplicates.AsSpan(), stats);

        await Assert.That(duplicates).IsEquivalentTo([1, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 4, 4, 5 ], CollectionOrdering.Matching);
    }

    [Test]
    public async Task LargeArrayTest()
    {
        var stats = new StatisticsContext();
        var random = new Random(42);
        var large = Enumerable.Range(0, 10000).OrderBy(_ => random.Next()).ToArray();
        var expected = large.OrderBy(x => x).ToArray();

        QuickSortDualPivot.Sort(large.AsSpan(), stats);

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

        QuickSortDualPivot.Sort(nearlySorted.AsSpan(), stats);

        await Assert.That(nearlySorted).IsEquivalentTo(Enumerable.Range(1, 100).ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    public async Task SmallArrayInsertionSortThresholdTest()
    {
        var stats = new StatisticsContext();
        var small = new[] { 5, 2, 8, 1, 9, 3, 7, 4, 6, 10, 15, 12, 18, 11, 19, 13, 17, 14, 16, 20 };
        QuickSortDualPivot.Sort(small.AsSpan(), stats);

        await Assert.That(small).IsEquivalentTo(Enumerable.Range(1, 20).ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    public async Task StringSortTest()
    {
        var stats = new StatisticsContext();
        var strings = new[] { "zebra", "apple", "mango", "banana", "cherry" };
        QuickSortDualPivot.Sort(strings.AsSpan(), stats);

        await Assert.That(strings).IsEquivalentTo(["apple", "banana", "cherry", "mango", "zebra"], CollectionOrdering.Matching);
    }


    [Test]
    [MethodDataSource(typeof(MockSortedData), nameof(MockSortedData.Generate))]
    public async Task StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        QuickSortDualPivot.Sort(array.AsSpan(), stats);

        await Assert.That((ulong)array.Length).IsEqualTo((ulong)inputSample.Samples.Length);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsNotEqualTo(0UL);
        await Assert.That(stats.CompareCount).IsNotEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsNotEqualTo(0UL);
    }

    [Test]
    [Arguments(30)]
    [Arguments(50)]
    [Arguments(100)]
    [Arguments(200)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        QuickSortDualPivot.Sort(sorted.AsSpan(), stats);

        // QuickSort Dual Pivot on sorted data:
        // - Best case behavior when data is already sorted
        // - The algorithm uses two pivots (leftmost and rightmost elements)
        // - For sorted data:
        //   * Initial comparison between left and right pivots
        //   * Partitioning scans through all elements
        //   * Each element is compared with both pivots
        //   * Minimal swaps (only final pivot placements)
        //
        // Expected behavior:
        // - Comparisons: O(n log n) in best case
        //   Each level of recursion processes all n elements with 2 pivot comparisons each
        //   With 3-way partitioning, depth is approximately log3(n)
        // - Swaps: O(log n) - only pivot placements at each recursion level
        //   2 swaps per level (placing both pivots) * log3(n) levels

        var minCompares = (ulong)(n * 2); // At minimum, each element compared with both pivots once

        // Swaps: For sorted data, only pivot placements
        // Each recursion level: 2 swaps (left and right pivots)
        // Depth: approximately log3(n)
        var recursionDepth = (int)Math.Ceiling(Math.Log(n, 3));
        var expectedSwaps = (ulong)(recursionDepth * 2);

        await Assert.That(stats.CompareCount >= minCompares).IsTrue().Because($"CompareCount ({stats.CompareCount}) should be >= {minCompares}");
        await Assert.That(stats.SwapCount >= expectedSwaps).IsTrue().Because($"SwapCount ({stats.SwapCount}) should be >= {expectedSwaps}");

        // IndexReads: At least as many as comparisons (each compare reads 2 elements)
        var minIndexReads = stats.CompareCount * 2;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
    }

    [Test]
    [Arguments(30)]
    [Arguments(50)]
    [Arguments(100)]
    [Arguments(200)]
    public async Task TheoreticalValuesReversedTest(int n)
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        QuickSortDualPivot.Sort(reversed.AsSpan(), stats);

        // QuickSort Dual Pivot on reversed data:
        // - Initially, left > right, so first swap occurs
        // - During partitioning:
        //   * Elements smaller than left pivot go to left section
        //   * Elements larger than right pivot go to right section
        //   * Elements between pivots stay in middle
        // - For reversed data, most elements need repositioning
        //
        // Expected behavior:
        // - Comparisons: O(n log n) average case
        //   With dual pivots, partitioning is more balanced than single pivot
        // - Swaps: O(n log n) average case
        //   Many elements need to be moved during partitioning

        var minCompares = (ulong)(n * 2); // At minimum, each element compared with both pivots
        var maxCompares = (ulong)(n * n); // Worst case (though rare with dual pivot)

        var minSwaps = (ulong)(n / 2); // At least half the elements need swapping
        var maxSwaps = (ulong)(n * n); // Worst case

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);

        // IndexReads: Significantly higher due to partitioning and swapping
        var minIndexReads = stats.CompareCount * 2;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
    }

    [Test]
    [Arguments(30)]
    [Arguments(50)]
    [Arguments(100)]
    [Arguments(200)]
    public async Task TheoreticalValuesRandomTest(int n)
    {
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        QuickSortDualPivot.Sort(random.AsSpan(), stats);

        // QuickSort Dual Pivot on random data: average case O(n log n)
        // - Dual pivot partitioning divides array into 3 parts
        // - Average recursion depth: log3(n) (more balanced than single pivot)
        // - Each partition level processes all n elements
        //
        // Expected behavior:
        // - Comparisons: O(n log n) average
        //   Approximately 2n * log3(n) comparisons
        // - Swaps: O(n log n) average
        //   Varies based on distribution

        var minCompares = (ulong)(n * 2); // Minimum: each element compared once
        var maxCompares = (ulong)(n * n); // Maximum: worst case (rare)

        var minSwaps = (ulong)Math.Log(n, 3) * 2; // Best case: only pivot placements
        var maxSwaps = (ulong)(n * Math.Log(n, 3) * 2); // Average case estimate

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount >= minSwaps).IsTrue().Because($"SwapCount ({stats.SwapCount}) should be >= {minSwaps}");

        // IndexReads: Should be proportional to comparisons and swaps
        var minIndexReads = stats.CompareCount * 2;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
    }

    [Test]
    [Arguments(5)]
    [Arguments(10)]
    [Arguments(20)]
    public async Task TheoreticalValuesSameElementsTest(int n)
    {
        var stats = new StatisticsContext();
        var sameValues = Enumerable.Repeat(42, n).ToArray();
        QuickSortDualPivot.Sort(sameValues.AsSpan(), stats);

        // QuickSort Dual Pivot with all same values:
        // - Left and right pivots are equal
        // - All elements equal to pivots
        // - Partitioning still occurs but elements don't move much
        // - Middle section contains most/all elements
        //
        // Expected behavior:
        // - Comparisons: O(n) for partitioning in each recursion level
        //   In the partitioning loop, each element (except pivots) is compared
        //   For n elements, that's roughly n-2 comparisons per level
        // - Swaps: 2 swaps per recursion level (final pivot placements at lines 75-76)
        //   Plus the recursive calls on left/middle/right sections

        var minCompares = (ulong)(n - 2); // At minimum, partitioning comparisons for one level

        // Swaps: Very few needed since all values are equal
        // Only pivot positioning swaps at each level
        var recursionDepth = (int)Math.Ceiling(Math.Log(Math.Max(n, 2), 3));
        var maxSwaps = (ulong)(recursionDepth * 4); // Allow some extra for partitioning

        await Assert.That(stats.CompareCount >= minCompares).IsTrue().Because($"CompareCount ({stats.CompareCount}) should be >= {minCompares}");
        await Assert.That(stats.SwapCount <= maxSwaps).IsTrue().Because($"SwapCount ({stats.SwapCount}) should be <= {maxSwaps} for all equal elements");

        // Verify the array is still correct (all values unchanged)
        foreach(var item in sameValues) await Assert.That(item).IsEqualTo(42);
    }

}
