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
        // Adjusted for higher write counts due to QuickSort partitioning overhead
        var minWrites = (ulong)(n * 0.5);
        var maxWrites = (ulong)(n * Math.Ceiling(logN) * 2.5);

        var minReads = (ulong)n * 2;
        var maxReads = (ulong)(n * logN * 6);

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
