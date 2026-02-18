using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class StableQuickSortTests
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

        StableQuickSort.Sort(array.AsSpan(), stats);

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

        StableQuickSort.Sort(array.AsSpan(), stats);

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

        StableQuickSort.Sort(array.AsSpan(), stats);

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

        StableQuickSort.Sort(array.AsSpan(), stats);

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

        StableQuickSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    public async Task EdgeCaseEmptyArrayTest()
    {
        var stats = new StatisticsContext();
        var empty = Array.Empty<int>();
        StableQuickSort.Sort(empty.AsSpan(), stats);
    }

    [Test]
    public async Task EdgeCaseSingleElementTest()
    {
        var stats = new StatisticsContext();
        var single = new[] { 42 };
        StableQuickSort.Sort(single.AsSpan(), stats);

        await Assert.That(single[0]).IsEqualTo(42);
    }

    [Test]
    public async Task EdgeCaseTwoElementsSortedTest()
    {
        var stats = new StatisticsContext();
        var twoSorted = new[] { 1, 2 };
        StableQuickSort.Sort(twoSorted.AsSpan(), stats);

        await Assert.That(twoSorted).IsEquivalentTo([1, 2], CollectionOrdering.Matching);
    }

    [Test]
    public async Task EdgeCaseTwoElementsReversedTest()
    {
        var stats = new StatisticsContext();
        var twoReversed = new[] { 2, 1 };
        StableQuickSort.Sort(twoReversed.AsSpan(), stats);

        await Assert.That(twoReversed).IsEquivalentTo([1, 2], CollectionOrdering.Matching);
    }

    [Test]
    public async Task EdgeCaseThreeElementsTest()
    {
        var stats = new StatisticsContext();
        var three = new[] { 3, 1, 2 };
        StableQuickSort.Sort(three.AsSpan(), stats);

        await Assert.That(three).IsEquivalentTo([1, 2, 3], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        StableQuickSort.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6 ], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        StableQuickSort.Sort(array.AsSpan(), 0, array.Length, stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task SortedArrayTest()
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(1, 100).ToArray();
        StableQuickSort.Sort(sorted.AsSpan(), stats);

        await Assert.That(sorted).IsEquivalentTo(Enumerable.Range(1, 100).ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    public async Task ReverseSortedArrayTest()
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(1, 100).Reverse().ToArray();
        StableQuickSort.Sort(reversed.AsSpan(), stats);

        await Assert.That(reversed).IsEquivalentTo(Enumerable.Range(1, 100).ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    public async Task AllEqualElementsTest()
    {
        var stats = new StatisticsContext();
        var allEqual = Enumerable.Repeat(42, 100).ToArray();
        StableQuickSort.Sort(allEqual.AsSpan(), stats);

        await Assert.That(allEqual).IsEquivalentTo(Enumerable.Repeat(42, 100).ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    public async Task ManyDuplicatesTest()
    {
        var stats = new StatisticsContext();
        var duplicates = new[] { 1, 2, 1, 3, 2, 1, 4, 3, 2, 1, 5, 4, 3, 2, 1 };
        StableQuickSort.Sort(duplicates.AsSpan(), stats);

        await Assert.That(duplicates).IsEquivalentTo([1, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 4, 4, 5 ], CollectionOrdering.Matching);
    }

    [Test]
    public async Task LargeArrayTest()
    {
        var stats = new StatisticsContext();
        var random = new Random(42);
        var large = Enumerable.Range(0, 10000).OrderBy(_ => random.Next()).ToArray();
        var expected = large.OrderBy(x => x).ToArray();

        StableQuickSort.Sort(large.AsSpan(), stats);

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

        StableQuickSort.Sort(nearlySorted.AsSpan(), stats);

        await Assert.That(nearlySorted).IsEquivalentTo(Enumerable.Range(1, 100).ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    public async Task SmallArrayInsertionSortThresholdTest()
    {
        var stats = new StatisticsContext();
        var small = new[] { 5, 2, 8, 1, 9, 3, 7, 4, 6, 10, 15, 12, 18, 11, 19, 13, 17, 14, 16, 20 };
        StableQuickSort.Sort(small.AsSpan(), stats);

        await Assert.That(small).IsEquivalentTo(Enumerable.Range(1, 20).ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    public async Task StringSortTest()
    {
        var stats = new StatisticsContext();
        var strings = new[] { "zebra", "apple", "mango", "banana", "cherry" };
        StableQuickSort.Sort(strings.AsSpan(), stats);

        await Assert.That(strings).IsEquivalentTo(["apple", "banana", "cherry", "mango", "zebra"], CollectionOrdering.Matching);
    }

#if DEBUG

    [Test]
    [MethodDataSource(typeof(MockSortedData), nameof(MockSortedData.Generate))]
    public async Task StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        StableQuickSort.Sort(array.AsSpan(), stats);

        await Assert.That((ulong)array.Length).IsEqualTo((ulong)inputSample.Samples.Length);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsNotEqualTo(0UL);
        await Assert.That(stats.CompareCount).IsNotEqualTo(0UL);
        // StableQuickSort doesn't use Swap - it uses Read/Write to copy via temporary buffers
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
        StableQuickSort.Sort(sorted.AsSpan(), stats);

        // Stable QuickSort on sorted data:
        // - Using middle element as pivot
        // - For sorted data, this provides balanced partitions
        // - Each partition reads all elements, compares with pivot, writes them back
        // - Uses temporary buffers (no swaps)
        //
        // Expected behavior:
        // - Comparisons: O(n log n) - Each element compared with pivot at each level
        //   For sorted data with middle pivot: approximately n log n comparisons
        // - Reads: O(n log n) - Each partitioning level reads all n elements + pivot
        //   Approximately (n+1) log n reads total
        // - Writes: O(n log n) - Each partitioning level writes all n elements back
        //   Approximately n log n writes total
        // - Swaps: 0 - This algorithm uses Read/Write, not Swap
        // - Recursion depth: O(log n) with balanced partitions
        var minCompares = (ulong)(n); // At minimum, each element visited once
        var maxCompares = (ulong)(n * n); // Worst case O(n²) if partitioning fails

        // StableQuickSort doesn't use Swap - it copies via temporary buffers
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);

        // IndexReads: Each recursion level reads pivot + all elements in range
        // For balanced partitioning: approximately (n+1) * log₂(n) reads
        var minIndexReads = (ulong)n; // At least read all elements once
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        // IndexWrites: Each recursion level writes all elements in range back
        // For balanced partitioning: approximately n * log₂(n) writes
        var minIndexWrites = (ulong)n; // At least write all elements once
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
        StableQuickSort.Sort(reversed.AsSpan(), stats);

        // Stable QuickSort on reversed data:
        // - Using middle element as pivot
        // - For reversed data with middle pivot, partitioning is still balanced
        // - Uses temporary buffers to rearrange elements (no swaps)
        //
        // Expected behavior:
        // - Comparisons: O(n log n) average case
        //   Similar to sorted data since middle pivot provides balance
        // - Reads: O(n log n) - Each partitioning level reads all elements
        // - Writes: O(n log n) - Each partitioning level writes elements back
        // - Swaps: 0 - This algorithm uses Read/Write, not Swap
        var minCompares = (ulong)(n);
        var maxCompares = (ulong)(n * n); // Worst case (rare with middle pivot)

        // StableQuickSort doesn't use Swap
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);

        // IndexReads: At least read all elements once
        var minIndexReads = (ulong)n;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        // IndexWrites: At least write all elements once
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
        StableQuickSort.Sort(random.AsSpan(), stats);

        // Stable QuickSort on random data: average case O(n log n)
        // - Middle element as pivot provides decent balance on average
        // - Partitioning divides array into approximately two halves
        // - Uses temporary buffers for stable partitioning
        //
        // Expected behavior:
        // - Comparisons: O(n log n) average
        //   Approximately n log₂ n comparisons
        // - Reads: O(n log n) average - each level reads all elements
        // - Writes: O(n log n) average - each level writes all elements back
        // - Swaps: 0 - This algorithm uses Read/Write, not Swap
        var minCompares = (ulong)(n);
        var maxCompares = (ulong)(n * n); // Worst case (very rare with random data)

        // StableQuickSort doesn't use Swap
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);

        // IndexReads: At least read all elements once
        var minIndexReads = (ulong)n;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        // IndexWrites: At least write all elements once
        var minIndexWrites = (ulong)n;
        await Assert.That(stats.IndexWriteCount >= minIndexWrites).IsTrue().Because($"IndexWriteCount ({stats.IndexWriteCount}) should be >= {minIndexWrites}");
    }

    [Test]
    [Arguments(5)]
    [Arguments(10)]
    [Arguments(20)]
    public async Task TheoreticalValuesSameElementsTest(int n)
    {
        var stats = new StatisticsContext();
        var sameValues = Enumerable.Repeat(42, n).ToArray();
        StableQuickSort.Sort(sameValues.AsSpan(), stats);

        // Stable QuickSort on all equal elements:
        // - All elements equal to pivot
        // - Three-way partitioning puts all elements in the "equal" partition
        // - Equal partition is already sorted, so no recursion occurs
        // - This is an OPTIMIZATION: O(n) time instead of O(n log n)
        //
        // Expected behavior with SortSpan-based temporary buffer (index-based pivot):
        //
        // MedianOf3Index (all elements equal):
        //   - s.Compare(lowIdx, midIdx): comparison between indices
        //   - low == mid, so else branch
        //   - s.Compare(midIdx, highIdx): comparison between indices
        //   - mid == high, so else branch
        //   - Returns midIdx (no value read required)
        //   Total: 2 compares (no extra reads for pivot value)
        //
        // StablePartition Phase 1 (count):
        //   - for loop: n iterations
        //   - s.Compare(i, pivotIndex): n comparisons (index-based)
        //   Total: n compares
        //
        // StablePartition Phase 2 (distribute to temp):
        //   - for loop: n iterations
        //   - s.Read(i): n reads (store to element)
        //   - s.Compare(i, pivotIndex): n comparisons (index-based, 1x per element)
        //   - tempSortSpan.Write(equalIdx++, element): n writes to temp buffer
        //   Total: n reads, n compares, n writes to temp
        //
        // StablePartition Phase 3 (copy back):
        //   - tempSortSpan.CopyTo(...): n reads from temp, n writes to main
        //   Total: n reads from temp, n writes to main
        //
        // Grand Total:
        //   - Compares: 2 (median) + n (phase1) + n (phase2) = 2 + 2n
        //   - Swaps: 0

        // Comparisons: 2 (median-of-3) + 2n (partition phases 1+2)
        var expectedCompares = (ulong)(2 * n) + 2;
        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);

        // StableQuickSort doesn't use Swap
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);

        // Verify the array is still correct (all values unchanged)
        foreach(var item in sameValues) await Assert.That(item).IsEqualTo(42);

        // Note: Index-based pivot avoids reading pivot value, reducing read count
        // IndexReads breakdown:
        // - MedianOf3Index: 2 compares × 2 reads/compare = 4 reads
        // - Phase1 (count): n iterations × 2 reads/compare = 2n reads
        // - Phase2 (distribute): n reads (element) + n compares × 2 reads/compare = 3n reads
        // - Phase3 (CopyTo): n reads from temp buffer
        // Total IndexReads = 4 + 2n + 3n + n = 4 + 6n
        var expectedIndexReads = (ulong)(4 + 6 * n);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedIndexReads);

        // IndexWrites: n (phase2 to temp) + n (phase3 to main) = 2n
        var expectedIndexWrites = (ulong)(2 * n);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedIndexWrites);
    }

#endif

}
