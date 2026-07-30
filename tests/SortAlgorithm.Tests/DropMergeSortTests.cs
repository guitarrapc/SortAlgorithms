using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class DropMergeSortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => DropMergeSort.Sort(span, context);

    // Already sorted input keeps every element in the LNS, so no writes are needed (optimized away).
    protected override CountExpectation SortedInputWrites => CountExpectation.Zero;

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

        // DropMergeSort for reversed data (matching the reference implementation's behavior):
        // Reversed input is a bad case for the LNS heuristic. Two regimes exist, depending on
        // where the one-shot early-out check (loop iteration == n/4) happens to land:
        //
        // 1. Early-out fires -> dropped elements are restored and the whole array is sorted
        //    by QuickSortMedian3: ~n*log2(n) comparisons.
        // 2. Early-out misses -> the LNS phase runs RECENCY-undo cycles: each cycle accepts one
        //    element, drops RECENCY (8) elements, then undoes (max-scan costs another RECENCY
        //    comparisons). This is O(RECENCY * n) ~ 17-22n comparisons, plus O(K log K) for
        //    sorting the ~n dropped elements and O(n) for the merge.
        //
        // Actual observations (deterministic input, no seed):
        // n=10:  55 comparisons   (regime 2: earlyOutStop=2 lands before any drop)
        // n=20:  96 comparisons   (regime 1: early-out fires at iteration 5)
        // n=50:  1077 comparisons (regime 2: at iteration 12, dropped(1) <= read(2)*0.6)
        // n=100: 949 comparisons  (regime 1: early-out fires at iteration 25)
        //
        // Bounds below cover both regimes with margin.
        var logN = Math.Log2(n);
        var minCompares = (ulong)n;  // Can be as low as n for small sizes
        var maxCompares = (ulong)(n * 22 + n * logN * 2);

        // Writes: regime 2 rewrites the kept prefix on every undo cycle (observed up to ~18n),
        // plus QuickSort partitioning of the dropped elements and the merge.
        var minWrites = (ulong)(n * 0.5);
        var maxWrites = (ulong)(n * 20);

        // Reads: 2 reads per comparison plus LNS shifting / max-scan / merge traffic (observed up to ~54n).
        var minReads = (ulong)n * 2;
        var maxReads = (ulong)(n * logN * 6 + n * 40);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount).IsBetween(minReads, maxReads);
        // DropMergeSort uses swaps in QuickSort for dropped elements
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
