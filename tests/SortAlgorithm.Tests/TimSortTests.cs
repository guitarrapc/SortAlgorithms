using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class TimSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => TimSort.Sort(span, context);

    // TimSort on already-sorted input detects a single ascending run: no writes, no swaps.
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
        TimSort.Sort(sorted.AsSpan(), stats);

        // TimSort for sorted data:
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
        TimSort.Sort(reversed.AsSpan(), stats);

        // TimSort for reversed data:
        // Detects single descending run, reverses it with swaps: O(n) comparisons, O(n/2) swaps
        // This applies to all sizes since run detection is performed even for n < MIN_MERGE
        //
        // Actual observations for reversed data:
        // n=10:  10 comparisons, 10 writes, 5 swaps   (Single descending run + reverse)
        // n=20:  20 comparisons, 20 writes, 10 swaps  (Single descending run + reverse)
        // n=50:  50 comparisons, 50 writes, 25 swaps  (Single descending run + reverse)
        // n=100: 100 comparisons, 100 writes, 50 swaps (Single descending run + reverse)
        //
        // Pattern: Run detection + reverse for all sizes
        ulong minCompares, maxCompares, minWrites, maxWrites, minSwaps, maxSwaps;

        // Single descending run + reverse
        minCompares = (ulong)(n - 1);
        maxCompares = (ulong)(n + 1);
        minWrites = (ulong)(n - 1);
        maxWrites = (ulong)(n + 1);
        minSwaps = (ulong)(n / 2 - 1);
        maxSwaps = (ulong)(n / 2 + 1);

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
        TimSort.Sort(random.AsSpan(), stats);

        // TimSort for random data:
        // Performance depends on natural run detection and merging.
        // Random data may contain some ascending/descending runs that are exploited.
        //
        // Observed range for random data (10 trial average):
        // n=10:  ~22 comparisons, ~30 writes, ~0 swaps
        // n=20:  ~63 comparisons, ~118 writes, ~0 swaps
        // n=50:  ~220 comparisons, ~416 writes, ~1 swap
        // n=100: ~542 comparisons, ~947 writes, ~2 swaps
        //
        // Comparisons: approximately 0.4 * n * log₂(n) to 1.5 * n * log₂(n)
        // Swaps occur when descending runs are detected and reversed
        //
        // Writes: For n < 2*MIN_MERGE (=64), ComputeMinRun returns n itself, so SortCore
        // extends the entire array via BinaryInsertionSort — O(n²) writes worst case.
        // n=10, 20 also take the direct BIS path (n < MIN_MERGE=32).
        // Upper bound is therefore n*(n-1)/2 ≈ n*n/2, not O(n log n).
        var logN = Math.Log2(n);
        var minCompares = (ulong)(n * logN * 0.4);
        var maxCompares = (ulong)(n * logN * 1.5);

        var minWrites = (ulong)(n * logN * 0.3);
        var maxWrites = (ulong)(n * n / 2.0);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount > 0).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be > 0");
        // Random data may have descending runs that get reversed with swaps
        await Assert.That(stats.SwapCount <= (ulong)(n / 4)).IsTrue().Because($"SwapCount ({stats.SwapCount}) should be less than or equal to n/4 ({n / 4})");
    }
}
