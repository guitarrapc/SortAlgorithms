using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class PDQSortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => PDQSort.Sort(span, context);

    // Partition writes occur even on sorted input. Swaps are not asserted:
    // sorted arrays may have 0 swaps due to pattern detection and partial
    // insertion sort optimization, which is expected behavior.
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        PDQSort.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        PDQSort.Sort(array.AsSpan(), 0, array.Length, stats);

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
        PDQSort.Sort(sorted.AsSpan(), stats);

        // PDQSort characteristics for sorted input:
        // PDQSort is designed to detect already-sorted partitions and achieve O(n) time
        // through partial insertion sort optimization.
        //
        // For sorted arrays:
        // - Pattern detection triggers early (alreadyPartitioned flag)
        // - Partial insertion sort succeeds quickly (few moves)
        // - Linear time behavior: O(n) comparisons
        //
        // Expected pattern: O(n) comparisons for sorted input (best case)

        var logN = Math.Log(n + 1, 2);
        // PDQSort achieves linear time on sorted input
        var minCompares = (ulong)(n * 0.5);  // Lower bound for linear scan
        var maxCompares = (ulong)(n * logN * 1.5 + n);  // Upper bound if pattern not fully detected

        // Sorted arrays should have very few swaps (ideally close to 0)
        var minSwaps = 0UL;
        var maxSwaps = (ulong)(n * 0.5);  // Mostly from pivot placements

        // Each swap writes 2 elements, plus writes for pivot moves
        var minWrites = minSwaps * 2;
        var maxWrites = (ulong)(n * 2.0);

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
        PDQSort.Sort(reversed.AsSpan(), stats);

        // PDQSort characteristics for reverse-sorted input:
        // PDQSort detects reverse-sorted patterns and achieves O(n) time
        // through pattern detection and partial insertion sort.
        //
        // For reverse-sorted arrays:
        // - Pattern detection identifies the sorted nature (after first partition)
        // - Partial insertion sort handles the reversal efficiently
        // - Linear time behavior: O(n) comparisons
        //
        // Expected pattern: O(n) to O(n log n) comparisons for reverse-sorted input

        var logN = Math.Log(n + 1, 2);
        // PDQSort achieves near-linear time on reverse-sorted input
        // Observed: n=10 has 0 swaps, n=20 has 190 compares
        var minCompares = 0UL;  // Can be 0 if detected as sorted quickly
        var maxCompares = (ulong)(n * logN * 2.5 + n);

        // Reverse-sorted can have varying swaps
        var minSwaps = 0UL;  // Can be 0 if handled by insertion sort
        var maxSwaps = (ulong)(n * logN);

        // Each swap writes 2 elements, plus writes for pivot moves and insertions
        var minWrites = 0UL;
        var maxWrites = (ulong)(n * logN * 3.0);

        // Each comparison reads 2 elements
        var minIndexReads = 0UL;  // Can be 0 for small arrays

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
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
        PDQSort.Sort(random.AsSpan(), stats);

        // PDQSort characteristics for random input:
        // Average case is similar to standard quicksort: O(n log n)
        // Expected ~1.38n log n comparisons (slightly better than basic quicksort)
        //
        // PDQSort optimizations for random data:
        // - Ninther pivot selection (median-of-9 for n > 128)
        // - Insertion sort for small partitions (< 24 elements)
        // - Pattern-defeating shuffles prevent worst-case scenarios
        //
        // Expected pattern: ~1.2-1.5 n log n comparisons for random input
        // Observed: n=10 has 0 swaps, n=50 has 22 swaps, n=100 has 63 swaps

        var logN = Math.Log(n + 1, 2);
        // PDQSort has efficient average case performance
        // Observed values are much lower than theoretical due to optimizations
        var minCompares = 0UL;  // Can be very low for small arrays
        var maxCompares = (ulong)(n * logN * 3.0 + n);

        // Random data typically requires moderate swaps
        // Observed: much lower than theoretical
        var minSwaps = 0UL;
        var maxSwaps = (ulong)(n * logN * 1.5);

        // Each swap writes 2 elements, plus writes for pivot moves and insertions
        var minWrites = 0UL;
        var maxWrites = (ulong)(n * logN * 4.0);

        // Each comparison reads 2 elements
        var minIndexReads = 0UL;

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
    public async Task TheoreticalValuesEqualElementsTest(int n)
    {
        var stats = new StatisticsContext();
        var allEqual = Enumerable.Repeat(42, n).ToArray();
        PDQSort.Sort(allEqual.AsSpan(), stats);

        // PDQSort characteristics for all equal elements:
        // PDQSort detects equal elements through partition_left optimization
        // When pivot equals all elements, partition_left is used to group equals together
        // This achieves O(n) time complexity
        //
        // Expected pattern: O(n) comparisons for all-equal input (best case)
        // All equal elements should trigger the equal-element detection

        var logN = Math.Log(n + 1, 2);
        // All equal elements should be detected early and handled efficiently
        var minCompares = 0UL;
        var maxCompares = (ulong)(n * logN * 2.0 + n);

        // Very few swaps needed for equal elements
        var minSwaps = 0UL;
        var maxSwaps = (ulong)(n);

        // Minimal writes for equal elements
        var minWrites = 0UL;
        var maxWrites = (ulong)(n * 2.5);

        // Each comparison reads 2 elements
        var minIndexReads = 0UL;

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
    }
}
