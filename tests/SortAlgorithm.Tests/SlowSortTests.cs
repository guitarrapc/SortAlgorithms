using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class SlowSortTests
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
        // Slow Sort is extremely slow, so we limit to small arrays
        Skip.When(inputSample.Samples.Length > 10, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        SlowSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }


    [Test, SkipCI]
    [MethodDataSource(typeof(MockSortedData), nameof(MockSortedData.Generate))]
    public async Task StatisticsSortedTest(IInputSample<int> inputSample)
    {
        // Slow Sort is extremely slow, so we limit to small arrays
        Skip.When(inputSample.Samples.Length > 10, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        SlowSort.Sort(array.AsSpan(), stats);

        await Assert.That((ulong)array.Length).IsEqualTo((ulong)inputSample.Samples.Length);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL); // Already sorted, no swaps needed
        await Assert.That(stats.CompareCount).IsNotEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL); // Already sorted
    }

    [Test, SkipCI]
    public async Task TheoreticalValuesSortedTest()
    {
        // Slow Sort on sorted data still performs all comparisons but no swaps
        // T(n) = T(⌊n/2⌋) + T(⌈n/2⌉) + T(n-1) + 1
        // For n=5: T(5) = T(2) + T(3) + T(4) + 1 = 1 + 3 + 6 + 1 = 11
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, 5).ToArray();
        SlowSort.Sort(sorted.AsSpan(), stats);

        var n = sorted.Length;
        // For sorted data: comparisons still occur, but no swaps
        var expectedCompares = 11UL; // T(5) = 11
        var expectedSwaps = 0UL;
        var expectedWrites = 0UL; // No swaps means no writes
        var expectedReads = expectedCompares * 2; // Each comparison reads 2 elements

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(expectedSwaps);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(IsSorted(sorted)).IsTrue().Because("Array should remain sorted");
    }

    [Test, SkipCI]
    [Arguments(3)]
    [Arguments(4)]
    [Arguments(5)]
    public async Task TheoreticalValuesReversedTest(int n)
    {
        // Slow Sort on reversed data performs all comparisons and many swaps
        // The number of comparisons is data-independent and follows T(n) = 2T(⌊n/2⌋) + T(n-1) + 1
        // But the number of swaps is data-dependent
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        SlowSort.Sort(reversed.AsSpan(), stats);

        // Verify the array is sorted
        await Assert.That(reversed).IsEquivalentTo(Enumerable.Range(0, n), CollectionOrdering.Matching);

        // For reversed data, many swaps will occur
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
        // Slow Sort has data-independent comparison count
        // but data-dependent swap count
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        SlowSort.Sort(random.AsSpan(), stats);

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
    [Arguments(2, 1)]   // T(2) = T(1) + T(1) + T(1) + 1 = 0 + 0 + 0 + 1 = 1
    [Arguments(3, 3)]   // T(3) = T(1) + T(2) + T(2) + 1 = 0 + 1 + 1 + 1 = 3
    [Arguments(4, 6)]   // T(4) = T(2) + T(2) + T(3) + 1 = 1 + 1 + 3 + 1 = 6
    [Arguments(5, 11)]  // T(5) = T(2) + T(3) + T(4) + 1 = 1 + 3 + 6 + 1 = 11
    [Arguments(6, 18)]  // T(6) = T(3) + T(3) + T(5) + 1 = 3 + 3 + 11 + 1 = 18
    [Arguments(7, 28)]  // T(7) = T(3) + T(4) + T(6) + 1 = 3 + 6 + 18 + 1 = 28
    [Arguments(8, 41)]  // T(8) = T(4) + T(4) + T(7) + 1 = 6 + 6 + 28 + 1 = 41
    [Arguments(9, 59)]  // T(9) = T(4) + T(5) + T(8) + 1 = 6 + 11 + 41 + 1 = 59
    [Arguments(10, 82)] // T(10) = T(5) + T(5) + T(9) + 1 = 11 + 11 + 59 + 1 = 82
    public async Task TheoreticalComparisonCountTest(int n, int expectedComparisons)
    {
        // Test the theoretical comparison count for Slow Sort
        // Comparison count follows: T(n) = T(⌊n/2⌋) + T(⌈n/2⌉) + T(n-1) + 1 when n >= 2
        // T(0) = 0, T(1) = 0
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        SlowSort.Sort(sorted.AsSpan(), stats);

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
        SlowSort.Sort(array.AsSpan(), stats);

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
        SlowSort.Sort(array.AsSpan(), stats);

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
        SlowSort.Sort(array.AsSpan(), stats);

        // Two sorted elements: T(2) = 1 comparison, no swap
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
        SlowSort.Sort(array.AsSpan(), stats);

        // Two reversed elements: T(2) = 1 comparison, 1 swap
        await Assert.That(stats.CompareCount).IsEqualTo(1UL);
        await Assert.That(stats.SwapCount).IsEqualTo(1UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(2UL); // 1 swap = 2 writes
        await Assert.That(stats.IndexReadCount).IsEqualTo(4UL); // 1 comparison (2 reads) + 1 swap (2 reads) = 4 reads
        await Assert.That(array).IsEquivalentTo([1, 2], CollectionOrdering.Matching);
    }

    [Test, SkipCI]
    [Arguments(3)]
    [Arguments(5)]
    [Arguments(7)]
    [Arguments(10)]
    public async Task IndexReadWriteConsistencyTest(int n)
    {
        // Verify that IndexRead and IndexWrite counts are consistent with Compare and Swap counts
        var stats = new StatisticsContext();
        var array = Enumerable.Range(0, n).Reverse().ToArray();
        SlowSort.Sort(array.AsSpan(), stats);

        // Each comparison reads 2 elements
        // Each swap reads 2 elements and writes 2 elements
        var expectedReads = stats.CompareCount * 2 + stats.SwapCount * 2;
        var expectedWrites = stats.SwapCount * 2;

        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(IsSorted(array)).IsTrue().Because("Array should be sorted");
    }

    [Test, SkipCI]
    [Arguments(2)]
    [Arguments(3)]
    [Arguments(4)]
    [Arguments(5)]
    [Arguments(6)]
    [Arguments(7)]
    public async Task ReversedDataMaximumSwapsTest(int n)
    {
        // Test that reversed data produces the maximum number of swaps
        // This validates that the algorithm is performing swaps correctly
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        SlowSort.Sort(reversed.AsSpan(), stats);

        // Verify the array is sorted
        await Assert.That(reversed).IsEquivalentTo(Enumerable.Range(0, n), CollectionOrdering.Matching);

        // For reversed data, we expect many swaps
        // The exact number depends on the recursive structure, but it should be > 0
        await Assert.That(stats.SwapCount > 0).IsTrue().Because($"Expected swaps for reversed data of size {n}, got {stats.SwapCount}");
        await Assert.That(stats.IndexWriteCount > 0).IsTrue().Because("Expected writes for reversed data");

        // Verify consistency: each swap = 2 writes
        await Assert.That(stats.IndexWriteCount).IsEqualTo(stats.SwapCount * 2);
    }

    [Test, SkipCI]
    [Arguments(3)]
    [Arguments(5)]
    [Arguments(7)]
    public async Task ComparisonCountDataIndependentTest(int n)
    {
        // Verify that comparison count is data-independent
        // Sorted, reversed, and random data should all have the same comparison count
        var statsSorted = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        SlowSort.Sort(sorted.AsSpan(), statsSorted);

        var statsReversed = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        SlowSort.Sort(reversed.AsSpan(), statsReversed);

        var statsRandom = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        SlowSort.Sort(random.AsSpan(), statsRandom);

        // All should have the same comparison count (data-independent)
        await Assert.That(statsSorted.CompareCount).IsEqualTo(statsReversed.CompareCount);
        await Assert.That(statsSorted.CompareCount).IsEqualTo(statsRandom.CompareCount);

        // But different swap counts (data-dependent)
        await Assert.That(statsSorted.SwapCount).IsEqualTo(0UL); // Sorted has no swaps
        await Assert.That(statsSorted.SwapCount).IsNotEqualTo(statsReversed.SwapCount); // Reversed has many swaps
    }

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
