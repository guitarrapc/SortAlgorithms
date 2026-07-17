using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class PowerSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => PowerSort.Sort(span, context);

    // Sorted input is detected as a single ascending run (or handled by BinaryInsertSort
    // with nothing to move): no writes, no swaps.
    protected override CountExpectation SortedInputWrites => CountExpectation.Zero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        PowerSort.Sort(sorted.AsSpan(), stats);

        // PowerSort for sorted data:
        // Small arrays (n < MIN_MERGE=32) use BinaryInsertSort directly,
        // which requires O(n log n) comparisons for binary search.
        // Larger arrays are detected as a single ascending run with O(n) comparisons.
        //
        // Actual observations for sorted data:
        // n=10:  19 comparisons   (BinaryInsertSort: binary search comparisons)
        // n=20:  54 comparisons   (BinaryInsertSort: binary search comparisons)
        // n=50:  50 comparisons   (Single run detection: n comparisons)
        // n=100: 100 comparisons  (Single run detection: n comparisons)
        //
        // Pattern: For n < 32, uses BinaryInsertSort; for n >= 32, run detection
        ulong minCompares, maxCompares;
        if (n < 32)
        {
            // BinaryInsertSort has variable comparisons due to binary search
            minCompares = (ulong)(n - 1);
            maxCompares = (ulong)(n * Math.Ceiling(Math.Log2(n + 1)));
        }
        else
        {
            // Single run detection
            minCompares = (ulong)(n - 1);
            maxCompares = (ulong)(n + 1);
        }

        // For sorted data, no writes needed (already sorted)
        var minWrites = 0UL;
        var maxWrites = 0UL;

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount > 0).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be > 0");
        await Assert.That(stats.SwapCount).IsEqualTo(0UL); // Sorted data doesn't need swaps
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
        PowerSort.Sort(reversed.AsSpan(), stats);

        // PowerSort for reversed data:
        // Small arrays (n < MIN_MERGE=32) use BinaryInsertSort: O(n²) writes, O(n log n) comparisons
        // Larger arrays detect single descending run, reverse it with swaps: O(n) comparisons, O(n/2) swaps
        //
        // Actual observations for reversed data:
        // n=10:  45 comparisons, 54 writes, 0 swaps   (InsertionSort)
        // n=20:  190 comparisons, 209 writes, 0 swaps  (InsertionSort)
        // n=50:  50 comparisons, 50 writes, 25 swaps  (Single descending run + reverse)
        // n=100: 100 comparisons, 100 writes, 50 swaps (Single descending run + reverse)
        //
        // Pattern: For n < MIN_RUN(24), uses InsertionSort (O(n²) comparisons); for n >= MIN_RUN, run detection + reverse
        ulong minCompares, maxCompares, minWrites, maxWrites, minSwaps, maxSwaps;
        if (n < 32)
        {
            // InsertionSort: reversed data causes each element to shift all the way left
            // Comparisons: up to n*(n-1)/2 (linear search per element)
            minCompares = (ulong)(n);
            maxCompares = (ulong)(n * (n - 1) / 2 + n);
            minWrites = (ulong)(n * (n - 1) / 4); // At least n²/4 writes
            maxWrites = (ulong)(n * (n + 1) / 2); // Up to n²/2 writes
            minSwaps = 0UL;
            maxSwaps = 0UL; // InsertionSort doesn't use swaps
        }
        else
        {
            // Single descending run + reverse
            minCompares = (ulong)(n - 1);
            maxCompares = (ulong)(n + 1);
            minWrites = (ulong)(n - 1);
            maxWrites = (ulong)(n + 1);
            minSwaps = (ulong)(n / 2 - 1);
            maxSwaps = (ulong)(n / 2 + 1);
        }

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);
        await Assert.That(stats.IndexReadCount > 0).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be > 0");
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
        PowerSort.Sort(random.AsSpan(), stats);

        // PowerSort for random data:
        // Performance depends on natural run detection and merging.
        // Random data may contain some ascending/descending runs that are exploited.
        //
        // Observed range for random data (10 trial average):
        // n=10:  ~30 comparisons, ~30 writes, ~0 swaps   (InsertionSort)
        // n=20:  ~90 comparisons, ~118 writes, ~0 swaps  (InsertionSort)
        // n=50:  ~250 comparisons, ~416 writes, ~1 swap
        // n=100: ~542 comparisons, ~947 writes, ~2 swaps
        //
        // Pattern: InsertionSort for short runs increases comparison count
        // approximately 0.4 * n * log₂(n) to 2.0 * n * log₂(n)
        // Swaps occur when descending runs are detected and reversed
        var logN = Math.Log2(n);
        var minCompares = (ulong)(n * logN * 0.4);
        var maxCompares = (ulong)(n * logN * 2.0);

        var minWrites = (ulong)(n * logN * 0.3);
        var maxWrites = (ulong)(n * logN * 2.5);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount > 0).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be > 0");
        // Random data may have descending runs that get reversed with swaps
        await Assert.That(stats.SwapCount < (ulong)(n / 4)).IsTrue().Because($"SwapCount ({stats.SwapCount}) should be less than n/4 ({n / 4})");
    }
}
