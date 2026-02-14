using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class StoogeSortTests
{
    [Test, SkipCI]
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
        // Stooge Sort is extremely slow, so we limit to small arrays
        Skip.When(inputSample.Samples.Length > 10, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        StoogeSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

#if DEBUG

    [Test, SkipCI]
    [MethodDataSource(typeof(MockSortedData), nameof(MockSortedData.Generate))]
    public async Task StatisticsSortedTest(IInputSample<int> inputSample)
    {
        // Stooge Sort is extremely slow, so we limit to small arrays
        Skip.When(inputSample.Samples.Length > 10, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        StoogeSort.Sort(array.AsSpan(), stats);

        await Assert.That((ulong)array.Length).IsEqualTo((ulong)inputSample.Samples.Length);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL); // Already sorted, no swaps needed
        await Assert.That(stats.CompareCount).IsNotEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL); // Already sorted
    }

    [Test, SkipCI]
    public async Task TheoreticalValuesSortedTest()
    {
        // Stooge Sort on sorted data still performs all comparisons but no swaps
        // T(n) = 3T(⌈2n/3⌉) + 1 (comparison)
        // For n=5: Expected structure:
        // - Initial comparison (0,4): 1 comparison, 0 swaps
        // - Recursive calls follow the same pattern
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, 5).ToArray();
        StoogeSort.Sort(sorted.AsSpan(), stats);

        var n = sorted.Length;
        // For sorted data: comparisons still occur, but no swaps
        var expectedSwaps = 0UL;
        var expectedWrites = 0UL; // No swaps means no writes

        // Number of comparisons follows the recurrence: T(n) = 3T(⌈2n/3⌉) + 1
        // For n=5, this evaluates to a specific value
        // We verify that comparisons occurred but no swaps/writes
        await Assert.That(stats.CompareCount).IsNotEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(expectedSwaps);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL); // Reads occur during comparisons
        await Assert.That(IsSorted(sorted)).IsTrue().Because("Array should remain sorted");
    }

    [Test, SkipCI]
    [Arguments(3)]
    [Arguments(4)]
    [Arguments(5)]
    public async Task TheoreticalValuesReversedTest(int n)
    {
        // Stooge Sort on reversed data performs maximum swaps
        // The algorithm always performs the same number of comparisons regardless of input,
        // but the number of swaps varies based on data arrangement
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        StoogeSort.Sort(reversed.AsSpan(), stats);

        // Verify the array is sorted
        await Assert.That(reversed).IsEquivalentTo(Enumerable.Range(0, n), CollectionOrdering.Matching);

        // For reversed data, many swaps will occur
        // Number of comparisons: O(n^2.71) - follows T(n) = 3T(⌈2n/3⌉) + 1
        await Assert.That(stats.CompareCount).IsNotEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);

        // Each swap involves 2 reads + 2 writes
        await Assert.That(stats.IndexWriteCount).IsEqualTo(stats.SwapCount * 2);
        // IndexReadCount includes reads from comparisons (2 per compare) and swaps (2 per swap)
        var expectedReads = stats.CompareCount * 2 + stats.SwapCount * 2;
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
    }

    [Test, SkipCI]
    [Arguments(3)]
    [Arguments(4)]
    [Arguments(5)]
    public async Task TheoreticalValuesRandomTest(int n)
    {
        // Stooge Sort has data-independent comparison count
        // but data-dependent swap count
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        StoogeSort.Sort(random.AsSpan(), stats);

        // Verify the array is sorted
        await Assert.That(random).IsEquivalentTo(Enumerable.Range(0, n), CollectionOrdering.Matching);

        // Verify operations were performed
        await Assert.That(stats.CompareCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexReadCount > 0).IsTrue();

        // For non-sorted input, there should be some swaps
        // (unless the random input happens to already be sorted, very unlikely)
        // We verify the relationship between swaps and writes
        if (stats.SwapCount > 0)
        {
            await Assert.That(stats.IndexWriteCount).IsEqualTo(stats.SwapCount * 2);
        }

        // IndexReadCount = CompareCount * 2 + SwapCount * 2
        var expectedReads = stats.CompareCount * 2 + stats.SwapCount * 2;
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
    }

    [Test, SkipCI]
    [Arguments(2, 1)]  // Base case: 2 elements
    [Arguments(3, 4)]  // T(3) = 3×T(2) + 1 = 3×1 + 1 = 4
    [Arguments(4, 13)] // T(4) = 3×T(3) + 1 = 3×4 + 1 = 13
    [Arguments(5, 40)] // T(5) = 3×T(4) + 1 = 3×13 + 1 = 40
    public async Task TheoreticalComparisonCountTest(int n, int expectedComparisons)
    {
        // Test the theoretical comparison count for Stooge Sort
        // Comparison count follows: T(n) = 3T(⌈2n/3⌉) + 1 when n >= 3
        // T(1) = 0, T(2) = 1
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        StoogeSort.Sort(sorted.AsSpan(), stats);

        // For sorted data, all comparisons occur but no swaps
        await Assert.That(stats.CompareCount).IsEqualTo((ulong)expectedComparisons);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL); // Sorted data has no swaps
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL); // No swaps means no writes

        // IndexReadCount = CompareCount * 2 (each comparison reads 2 elements)
        var expectedReads = (ulong)(expectedComparisons * 2);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
    }

    [Test, SkipCI]
    public async Task EdgeCaseSingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 42 };
        StoogeSort.Sort(array.AsSpan(), stats);

        // Single element: no comparisons, no operations
        await Assert.That(stats.CompareCount).IsEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexReadCount).IsEqualTo(0UL);
        await Assert.That(array).IsEquivalentTo([42], CollectionOrdering.Matching);
    }

    [Test, SkipCI]
    public async Task EdgeCaseEmptyTest()
    {
        var stats = new StatisticsContext();
        var array = Array.Empty<int>();
        StoogeSort.Sort(array.AsSpan(), stats);

        // Empty array: no operations
        await Assert.That(stats.CompareCount).IsEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexReadCount).IsEqualTo(0UL);
    }

    [Test, SkipCI]
    public async Task EdgeCaseTwoElementsSortedTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 2 };
        StoogeSort.Sort(array.AsSpan(), stats);

        // Two sorted elements: 1 comparison, no swap
        await Assert.That(stats.CompareCount).IsEqualTo(1UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexReadCount).IsEqualTo(2UL); // 1 comparison = 2 reads
        await Assert.That(array).IsEquivalentTo([1, 2], CollectionOrdering.Matching);
    }

    [Test, SkipCI]
    public async Task EdgeCaseTwoElementsReversedTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 2, 1 };
        StoogeSort.Sort(array.AsSpan(), stats);

        // Two reversed elements: 1 comparison, 1 swap
        await Assert.That(stats.CompareCount).IsEqualTo(1UL);
        await Assert.That(stats.SwapCount).IsEqualTo(1UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(2UL); // 1 swap = 2 writes
        await Assert.That(stats.IndexReadCount).IsEqualTo(4UL); // 1 comparison (2 reads) + 1 swap (2 reads) = 4 reads
        await Assert.That(array).IsEquivalentTo([1, 2], CollectionOrdering.Matching);
    }

#endif

    private static bool IsSorted<T>(T[] array) where T : IComparable<T>
    {
        for (int i = 1; i < array.Length; i++)
        {
            if (new ComparableComparer<T>().Compare(array[i - 1], array[i]) > 0)
                return false;
        }
        return true;
    }
}
