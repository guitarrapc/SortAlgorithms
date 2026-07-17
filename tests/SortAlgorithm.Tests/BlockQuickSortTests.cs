using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class BlockQuickSortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => BlockQuickSort.Sort(span, context);

    // No writes/swaps knobs: sorted input may take the InsertionSort path (0 writes),
    // so only the base IndexRead/Compare non-zero assertions apply.

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateHalf))]
    public async Task SortHalfNullContextResultOrderTest(IInputSample<Half> inputSample)
    {
        var array = inputSample.Samples.ToArray();

        // Default overload uses NullContext, whose Release fast path compares primitives with
        // IEEE operators (NaN unordered). Verifies the NaN pre-pass keeps the result ordered.
        BlockQuickSort.Sort(array.AsSpan());

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateFloat))]
    public async Task SortFloatNullContextResultOrderTest(IInputSample<float> inputSample)
    {
        var array = inputSample.Samples.ToArray();

        BlockQuickSort.Sort(array.AsSpan());

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateDouble))]
    public async Task SortDoubleNullContextResultOrderTest(IInputSample<double> inputSample)
    {
        var array = inputSample.Samples.ToArray();

        BlockQuickSort.Sort(array.AsSpan());

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
        BlockQuickSort.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        BlockQuickSort.Sort(array.AsSpan(), 0, array.Length, stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task VeryLargeArrayMedianOfSqrtPathTest()
    {
        // n > 20000 exercises the median-of-sqrt(n) pivot selection (MedianOfK + PartialSort)
        var stats = new StatisticsContext();
        var random = new Random(42);
        var large = Enumerable.Range(0, 25000).OrderBy(_ => random.Next()).ToArray();
        var expected = large.OrderBy(x => x).ToArray();

        BlockQuickSort.Sort(large.AsSpan(), stats);

        await Assert.That(large).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task VeryLargeArrayWithDuplicatesTest()
    {
        // n > 20000 with heavy duplicates: median-of-sqrt(n) path combined with duplicate-dense input
        var stats = new StatisticsContext();
        var random = new Random(42);
        var large = Enumerable.Range(0, 25000).Select(_ => random.Next(0, 16)).ToArray();
        var expected = large.OrderBy(x => x).ToArray();

        BlockQuickSort.Sort(large.AsSpan(), stats);

        await Assert.That(large).IsEquivalentTo(expected, CollectionOrdering.Matching);
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
        BlockQuickSort.Sort(sorted.AsSpan(), stats);

        // BlockQuickSort behavior on sorted data:
        // - For small arrays (n ≤ 20): Uses InsertionSort
        //   - Best case O(n): n-1 comparisons, 0 swaps
        // - For larger arrays (n > 20): Uses QuickSort with adaptive pivot selection
        //   - For sorted data with median-of-3 or better pivot selection,
        //     partitioning creates relatively balanced partitions
        //   - InsertionSort is used for final small partitions (≤ 20 elements)
        //   - Block partitioning comparisons + InsertionSort for small partitions

        ulong minCompares, maxCompares, minSwaps, maxSwaps;

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
        BlockQuickSort.Sort(reversed.AsSpan(), stats);

        // BlockQuickSort behavior on reversed data:
        // - For small arrays (n ≤ 20): Uses InsertionSort
        //   - Worst case O(n²): n(n-1)/2 comparisons, (n-1)(n+2)/2 writes
        // - For larger arrays (n > 20): Uses QuickSort with adaptive pivot selection
        //   - Median-of-3 or better pivot selection helps avoid worst-case
        //   - Block partitioning occurs, then InsertionSort for final small partitions
        //   - Overall: O(n log n) behavior with good pivot selection

        ulong minCompares, maxCompares;

        // Conservative bounds that handle both paths
        if (n <= 20)
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
    [Arguments(10, 42)]
    [Arguments(10, 1234)]
    [Arguments(20, 42)]
    [Arguments(20, 1234)]
    [Arguments(50, 42)]
    [Arguments(50, 1234)]
    [Arguments(100, 42)]
    [Arguments(100, 1234)]
    public async Task TheoreticalValuesRandomTest(int n, int seed)
    {
        var stats = new StatisticsContext();
        var random = TestHelpers.ShuffledRange(n, seed);
        BlockQuickSort.Sort(random.AsSpan(), stats);

        // BlockQuickSort behavior on random data:
        // - For small arrays (n ≤ 20): Uses InsertionSort
        //   - Average case O(n²): approximately n²/4 comparisons
        // - For larger arrays: Uses QuickSort with adaptive pivot selection + InsertionSort for small partitions
        //   - Average case: ~1.2-1.4n log₂ n comparisons (better than standard QuickSort)
        //   - Block partitioning improves cache efficiency without changing comparison count
        //   - Combined complexity: O(n log n) with InsertionSort overhead

        // Conservative bounds for both paths
        ulong minCompares = (ulong)(n - 1); // Minimum: at least n-1 comparisons
        ulong maxCompares = (ulong)(n * n); // Allow for InsertionSort worst-case on partitions

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);

        var minIndexReads = stats.CompareCount;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
    }
}
