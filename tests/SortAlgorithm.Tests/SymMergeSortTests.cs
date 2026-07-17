using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class SymMergeSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => SymMergeSort.Sort(span, context);

    // Sorted data: InsertionSort blocks and merge skip-checks only compare, no writes needed.
    protected override CountExpectation SortedInputWrites => CountExpectation.Zero;
    // Sorted data: no rotations, so no swaps needed.
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
        SymMergeSort.Sort(sorted.AsSpan(), stats);

        // Bottom-up SymMergeSort on sorted data:
        //
        // Phase 1 (InsertionSort blocks of size 20): for sorted data, each block costs
        //   (blockSize-1) comparisons with no writes or swaps.
        //   Sum across all blocks = n - ⌈n/threshold⌉.
        //
        // Phase 2 (merge passes): each adjacent pair passes the already-sorted skip-check
        //   (s[mid-1] ≤ s[mid]) in exactly 1 comparison, so SymMerge is never entered.
        //   Total skip-check comparisons ≈ ⌊n/40⌋ + ⌊n/80⌋ + … ≈ n/threshold.
        //
        // Combined total is always n-1 comparisons.
        //
        // Observed: n=10 → 9, n=20 → 19, n=50 → 49, n=100 → 99
        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(n);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);  // No writes on sorted data
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);        // No rotations on sorted data
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
        SymMergeSort.Sort(reversed.AsSpan(), stats);

        // Bottom-up SymMergeSort on reversed data:
        //
        // Phase 1 (InsertionSort blocks of size 20): n ≤ 20 → single block, all work done here.
        //   n*(n-1)/2 comparisons and writes, 0 swaps (InsertionSort uses Write, not Swap).
        //
        // Phase 2 (SymMerge passes): symmetric binary search (O(log n) per call) + rotation.
        //   GCD block-swap uses Swap for the general case; k==1/k==n-1 fast paths use Write.
        //
        // Actual observations for reversed data:
        //   n=10:    45 comparisons (n*(n-1)/2, InsertionSort only)
        //   n=20:   190 comparisons (n*(n-1)/2, InsertionSort only, single block with threshold=20)
        //   n=50:  ~453 comparisons (~1.61 * n*log₂n)
        //   n=100: ~821 comparisons (~1.24 * n*log₂n)
        var logN = Math.Log2(n);
        var minCompares = n <= 20 ? (ulong)(n * 4.0) : (ulong)(n * logN * 0.9);
        var maxCompares = n <= 20 ? (ulong)(n * 10.0) : (ulong)(n * logN * 2.5);

        var minWrites = n <= 20 ? (ulong)(n * 4.0) : (ulong)(n * logN * 1.0);
        var maxWrites = n <= 20 ? (ulong)(n * 11.0) : (ulong)(n * logN * 20.0);

        var minSwaps = 0UL;
        var maxSwaps = n <= 20 ? 0UL : (ulong)(n * logN * 2.0);

        var minReads = (ulong)(stats.CompareCount * 1.0);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);
        await Assert.That(stats.IndexReadCount >= minReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minReads}");
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
        SymMergeSort.Sort(random.AsSpan(), stats);

        // Bottom-up SymMergeSort on random data:
        //
        // Phase 1 (InsertionSort blocks of size 20): n ≤ 20 → single block, variance depends on order.
        // Phase 2 (SymMerge passes): symmetric binary search does not benefit from galloping,
        //   so comparisons are higher than RotateMerge for these sizes.
        //
        // Actual observations over 5 random runs:
        //   n=10:    27–34  comparisons (InsertionSort only, varies with order)
        //   n=20:    ~100–190 comparisons (InsertionSort only, single block with threshold=20)
        //   n=50:   421–496 comparisons (~1.5–1.8 * n*log₂n)
        //   n=100: 1213–1313 comparisons (~1.8–2.0 * n*log₂n)
        var logN = Math.Log2(n);
        var minCompares = n <= 20 ? (ulong)(n * 0.8) : (ulong)(n * logN * 0.7);
        var maxCompares = n <= 20 ? (ulong)(n * 10.0) : (ulong)(n * logN * 2.5);

        var minWrites = n <= 20 ? (ulong)(n * 0.8) : (ulong)(n * logN * 0.5);
        var maxWrites = n <= 20 ? (ulong)(n * 10.0) : (ulong)(n * logN * 10.0);

        var minSwaps = 0UL;
        var maxSwaps = n <= 20 ? 0UL : (ulong)(n * logN * 1.5);

        var minReads = (ulong)(stats.CompareCount * 1.0);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);
        await Assert.That(stats.IndexReadCount >= minReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minReads}");
    }
}
