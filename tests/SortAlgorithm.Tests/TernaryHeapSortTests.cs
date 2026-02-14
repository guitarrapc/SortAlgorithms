using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class TernaryHeapSortTests
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

        TernaryHeapSort.Sort(array.AsSpan(), stats);

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

        TernaryHeapSort.Sort(array.AsSpan(), stats);

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

        TernaryHeapSort.Sort(array.AsSpan(), stats);

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

        TernaryHeapSort.Sort(array.AsSpan(), stats);

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

        TernaryHeapSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        TernaryHeapSort.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6 ], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        TernaryHeapSort.Sort(array.AsSpan(), 0, array.Length, stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortSingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9 };

        // Sort a single element range [2, 3)
        TernaryHeapSort.Sort(array.AsSpan(), 2, 3, stats);

        // Array should be unchanged (single element is already sorted)
        await Assert.That(array).IsEquivalentTo([5, 3, 8, 1, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortBeginningTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 9, 7, 5, 3, 1, 2, 4, 6, 8 };

        // Sort only the first 5 elements [0, 5)
        TernaryHeapSort.Sort(array.AsSpan(), 0, 5, stats);

        // Expected: first 5 sorted, last 4 unchanged
        await Assert.That(array).IsEquivalentTo([1, 3, 5, 7, 9, 2, 4, 6, 8], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortEndTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 3, 5, 7, 9, 8, 6, 4, 2 };

        // Sort only the last 4 elements [5, 9)
        TernaryHeapSort.Sort(array.AsSpan(), 5, 9, stats);

        // Expected: first 5 unchanged, last 4 sorted
        await Assert.That(array).IsEquivalentTo([1, 3, 5, 7, 9, 2, 4, 6, 8], CollectionOrdering.Matching);
    }

    [Test]
    public async Task BasicSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 3, 1, 4, 1, 5, 9, 2, 6, 5, 3 };

        TernaryHeapSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo([1, 1, 2, 3, 3, 4, 5, 5, 6, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task EmptyArrayTest()
    {
        var stats = new StatisticsContext();
        var array = Array.Empty<int>();

        TernaryHeapSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEmpty();
    }

    [Test]
    public async Task SingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 42 };

        TernaryHeapSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo([42], CollectionOrdering.Matching);
    }

    [Test]
    public async Task TwoElementsTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 2, 1 };

        TernaryHeapSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo([1, 2], CollectionOrdering.Matching);
    }

    [Test]
    public async Task AlreadySortedTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        TernaryHeapSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task ReverseSortedTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 9, 8, 7, 6, 5, 4, 3, 2, 1 };

        TernaryHeapSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task AllSameElementsTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 5, 5, 5, 5 };

        TernaryHeapSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo([5, 5, 5, 5, 5], CollectionOrdering.Matching);
    }

#if DEBUG

    [Test]
    [MethodDataSource(typeof(MockSortedData), nameof(MockSortedData.Generate))]
    public async Task StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        TernaryHeapSort.Sort(array.AsSpan(), stats);

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
        TernaryHeapSort.Sort(sorted.AsSpan(), stats);

        // Ternary Heap Sort characteristics:
        // Build heap phase: O(n) comparisons with some swaps even for sorted data
        // Extract phase: (n-1) extractions, each requiring O(log₃ n) heapify
        // Each heapify compares with 3 children instead of 2
        //
        // Empirical observations for sorted data:
        // n=10:  Compare=42,  Swap=26
        // n=20:  Compare=126, Swap=64
        // n=50:  Compare=449, Swap=198
        // n=100: Compare=1101, Swap=464
        //
        // Pattern: approximately n * log₃(n) * 3 ≈ n * log₂(n) * 1.9 for compares

        var logN = Math.Log(n + 1, 3); // log base 3 for ternary heap
        var minCompares = (ulong)(n * logN * 1.0);
        var maxCompares = (ulong)(n * logN * 4.5 + n);

        var minSwaps = (ulong)(n * logN * 0.5);
        var maxSwaps = (ulong)(n * logN * 2.5);

        // Each swap writes 2 elements
        var minWrites = minSwaps * 2;
        var maxWrites = maxSwaps * 2;

        // Each comparison reads 2 elements
        var minIndexReads = minCompares * 2;

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
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
        TernaryHeapSort.Sort(reversed.AsSpan(), stats);

        // Ternary Heap Sort has O(n log n) time complexity regardless of input order
        // Reversed data shows similar patterns to sorted data due to heap property
        //
        // Empirical observations for reversed data:
        // n=10:  Compare=37,  Swap=19
        // n=20:  Compare=110, Swap=46
        // n=50:  Compare=393, Swap=159
        // n=100: Compare=986, Swap=382
        //
        // Pattern: approximately n * log₃(n) * 3 for compares

        var logN = Math.Log(n + 1, 3); // log base 3 for ternary heap
        var minCompares = (ulong)(n * logN * 1.0);
        var maxCompares = (ulong)(n * logN * 4.5 + n);

        var minSwaps = (ulong)(n * logN * 0.4);
        var maxSwaps = (ulong)(n * logN * 2.2);

        // Each swap writes 2 elements
        var minWrites = minSwaps * 2;
        var maxWrites = maxSwaps * 2;

        // Each comparison reads 2 elements
        var minIndexReads = minCompares * 2;

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
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
        TernaryHeapSort.Sort(random.AsSpan(), stats);

        // Ternary Heap Sort has consistent O(n log n) time complexity for random data
        // The values are similar to sorted/reversed cases
        //
        // Empirical observations for random data (averaged over 5 runs):
        // n=10:  Compare=40,  Swap=22
        // n=20:  Compare=117, Swap=57
        // n=50:  Compare=415, Swap=180
        // n=100: Compare=1015, Swap=422
        //
        // Pattern: approximately n * log₃(n) * 3, with variation due to randomness

        var logN = Math.Log(n + 1, 3); // log base 3 for ternary heap
        var minCompares = (ulong)(n * logN * 1.0);
        var maxCompares = (ulong)(n * logN * 4.5 + n);

        var minSwaps = (ulong)(n * logN * 0.5);
        var maxSwaps = (ulong)(n * logN * 2.5);

        // Each swap writes 2 elements
        var minWrites = minSwaps * 2;
        var maxWrites = maxSwaps * 2;

        // Each comparison reads 2 elements
        var minIndexReads = minCompares * 2;

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
    }

#endif
}
