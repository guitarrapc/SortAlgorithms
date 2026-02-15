using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class DropMergeSortTests
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

        DropMergeSort.Sort(array.AsSpan(), stats);

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

        DropMergeSort.Sort(array.AsSpan(), stats);

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

        DropMergeSort.Sort(array.AsSpan(), stats);

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

        DropMergeSort.Sort(array.AsSpan(), stats);

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

        DropMergeSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    public async Task EmptyArrayTest()
    {
        var stats = new StatisticsContext();
        var array = Array.Empty<int>();
        DropMergeSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEmpty();
    }

    [Test]
    public async Task SingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 42 };
        DropMergeSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsSingleElement();
        await Assert.That(array[0]).IsEqualTo(42);
    }

    [Test]
    public async Task TwoElementsSortedTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 2 };
        DropMergeSort.Sort(array.AsSpan(), stats);

        await Assert.That(array.Length).IsEqualTo(2);
        await Assert.That(array[0]).IsEqualTo(1);
        await Assert.That(array[1]).IsEqualTo(2);
    }

    [Test]
    public async Task TwoElementsReversedTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 2, 1 };
        DropMergeSort.Sort(array.AsSpan(), stats);

        await Assert.That(array.Length).IsEqualTo(2);
        await Assert.That(array[0]).IsEqualTo(1);
        await Assert.That(array[1]).IsEqualTo(2);
    }

    [Test]
    public async Task AlreadySortedTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 2, 3, 4, 5 };
        DropMergeSort.Sort(array.AsSpan(), stats);

        await Assert.That(array.Length).IsEqualTo(5);
        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5], CollectionOrdering.Matching);
    }

    [Test]
    public async Task ReverseSortedTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 4, 3, 2, 1 };
        DropMergeSort.Sort(array.AsSpan(), stats);

        await Assert.That(array.Length).IsEqualTo(5);
        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5], CollectionOrdering.Matching);
    }

    [Test]
    public async Task SingleOutlierTest()
    {
        // Test the "quick undo" optimization path
        var stats = new StatisticsContext();
        var array = new[] { 0, 1, 2, 3, 9, 5, 6, 7 };
        DropMergeSort.Sort(array.AsSpan(), stats);

        await Assert.That(array.Length).IsEqualTo(8);
        await Assert.That(array).IsEquivalentTo([0, 1, 2, 3, 5, 6, 7, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task NearlySortedWithFewOutliersTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 2, 15, 3, 4, 5, 20, 6, 7, 8, 9, 10 };
        DropMergeSort.Sort(array.AsSpan(), stats);

        await Assert.That(array.Length).IsEqualTo(12);
        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 15, 20], CollectionOrdering.Matching);
    }


    [Test]
    [MethodDataSource(typeof(MockSortedData), nameof(MockSortedData.Generate))]
    public async Task StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        DropMergeSort.Sort(array.AsSpan(), stats);

        await Assert.That((ulong)array.Length).IsEqualTo((ulong)inputSample.Samples.Length);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL); // Already sorted, no writes needed (optimized away)
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
        DropMergeSort.Sort(sorted.AsSpan(), stats);

        // DropMergeSort for sorted data:
        // For already sorted data, DropMergeSort achieves O(n) best case.
        // It extracts the Longest Nondecreasing Subsequence (LNS) in a single pass.
        // Since the data is already sorted, all elements are kept in the LNS,
        // no elements are dropped, and no merge is needed.
        //
        // Theoretical bounds for sorted data:
        // - Comparisons: n-1 (one comparison per element to verify it maintains order)
        // - Writes: 0 (no elements need to be moved)
        // - Reads: Each comparison reads 2 elements
        //
        // Actual observations for sorted data:
        // n=10:  9 comparisons    (n-1)
        // n=20:  19 comparisons   (n-1)
        // n=50:  49 comparisons   (n-1)
        // n=100: 99 comparisons   (n-1)
        //
        // Pattern for sorted data: n-1 comparisons (LNS extraction only)
        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(n);

        // DropMergeSort writes for sorted data:
        // For sorted data, no elements are dropped, so writes = 0
        var minWrites = 0UL;
        var maxWrites = 0UL;

        // Reads for sorted data: Each comparison reads 2 elements
        var minReads = stats.CompareCount * 2;

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount >= minReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minReads}");
        await Assert.That(stats.SwapCount).IsEqualTo(0UL); // DropMergeSort doesn't use swaps for sorted data
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
        DropMergeSort.Sort(reversed.AsSpan(), stats);

        // DropMergeSort for reversed data:
        // For reversed data, DropMergeSort's LNS extraction keeps only the first element,
        // and all other n-1 elements are dropped into the temporary buffer.
        // The dropped elements are then sorted using QuickSort (O(K log K) where K = n-1),
        // and finally merged with the single-element LNS.
        //
        // Theoretical bounds for reversed data:
        // - LNS extraction: n-1 comparisons (all fail, all elements dropped except first)
        // - Sorting dropped: ~(n-1) * log₂(n-1) comparisons (QuickSort)
        // - Merge: n-1 comparisons (merging single element with n-1 sorted elements)
        //
        // Actual observations for reversed data (highly adaptive):
        // n=10:  37 comparisons  (ratio 1.114)
        // n=20:  30 comparisons  (ratio 0.347) - surprisingly efficient!
        // n=50:  125 comparisons (ratio 0.443)
        // n=100: 427 comparisons (ratio 0.643)
        //
        // Pattern: DropMergeSort shows highly variable performance on reversed data.
        // Small sizes can be nearly linear, larger sizes approach n*log(n).
        // Range: approximately n to 1.2 * n * log₂(n)
        var logN = Math.Log2(n);
        var minCompares = (ulong)n;  // Can be as low as n for small sizes
        var maxCompares = (ulong)(n * logN * 2);

        // Writes include moving dropped elements and merge operations
        var minWrites = (ulong)(n * 0.5);
        var maxWrites = (ulong)(n * Math.Ceiling(logN) * 1.5);

        var minReads = (ulong)n * 2;
        var maxReads = (ulong)(n * logN * 5);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount).IsBetween(minReads, maxReads);
        // DropMergeSort uses swaps in QuickSort for dropped elements
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
        DropMergeSort.Sort(random.AsSpan(), stats);

        // DropMergeSort for random data:
        // For random data, DropMergeSort's performance depends on the disorder level (K).
        // The algorithm extracts an LNS heuristically, drops out-of-order elements (K elements),
        // sorts them using QuickSort, and merges the results.
        // Average case: O(n + K log K) where K is the number of dropped elements.
        //
        // For random data, K varies widely (could be anywhere from 20% to 80% of n).
        // If K > 60%, early-out heuristic may trigger and fall back to QuickSort.
        // However, DropMergeSort's RECENCY backtracking and other optimizations make it
        // highly adaptive to the actual data distribution.
        //
        // Actual observations for random data (highly variable due to randomness):
        // n=10:  33 comparisons  (ratio 0.993)
        // n=20:  91 comparisons  (ratio 1.053)
        // n=50:  283 comparisons (ratio 1.003)
        // n=100: 265 comparisons (ratio 0.399) - can vary widely!
        //
        // Pattern: DropMergeSort is extremely adaptive on random data.
        // Performance ranges from nearly linear to n*log(n) depending on randomness.
        // Range: approximately n to 1.2 * n * log₂(n)
        var logN = Math.Log2(n);
        var minCompares = (ulong)n;  // Can be as low as n when lucky with LNS
        var maxCompares = (ulong)(n * logN * 2.7);

        // Writes include LNS extraction, sorting dropped elements, and merge
        var minWrites = (ulong)(n * 0.3);
        var maxWrites = (ulong)(n * Math.Ceiling(logN) * 2.0);

        var minReads = (ulong)(n * logN * 1.5);
        var maxReads = (ulong)(n * logN * 5);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount).IsBetween(minReads, maxReads);
        // DropMergeSort may use swaps in QuickSort for dropped elements
    }

}
