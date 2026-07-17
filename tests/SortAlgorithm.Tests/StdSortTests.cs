using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class StdSortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => StdSort.Sort(span, context);

    // No writes/swaps knobs: sorted input may take the InsertionSort path (0 writes),
    // so only the base IndexRead/Compare non-zero assertions apply.

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        StdSort.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        StdSort.Sort(array.AsSpan(), 0, array.Length, stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9], CollectionOrdering.Matching);
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
        StdSort.Sort(sorted.AsSpan(), stats);

        // StdSort behavior on sorted data:
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
        StdSort.Sort(reversed.AsSpan(), stats);

        // StdSort behavior on reversed data:
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
        StdSort.Sort(random.AsSpan(), stats);

        // StdSort behavior on random data:
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
        StdSort.Sort(sameValues.AsSpan(), stats);

        // StdSort behavior on same elements:
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
}
