using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class RotateMergeSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => RotateMergeSort.Sort(span, context);

    // Rotation-based merge is O(n log^2 n); keep data-driven tests on small inputs.
    protected override int MaxOrderTestSize => 1024;

    // Sorted input passes the skip-check in every merge pass: no writes, no rotations.
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
        RotateMergeSort.Sort(sorted.AsSpan(), stats);

        // Bottom-up RotateMergeSort on sorted data:
        //
        // Phase 1 (InsertionSort blocks): for sorted data, each block costs (blockSize-1) comparisons
        //   with no writes or swaps. Sum across all blocks = n - ⌈n/threshold⌉.
        //
        // Phase 2 (merge passes): each adjacent pair passes the already-sorted skip-check
        //   (s[mid] ≤ s[mid+1]) in exactly 1 comparison, so no rotation is performed.
        //   Total skip-check comparisons ≈ ⌊n/32⌋ + ⌊n/64⌋ + … ≈ n/threshold.
        //
        // Combined total is always n-1 comparisons, matching the recursive variant.
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
        RotateMergeSort.Sort(reversed.AsSpan(), stats);

        // Bottom-up RotateMergeSort on reversed data:
        //
        // Phase 1 (InsertionSort blocks): n ≤ 16 → single block, all work done here.
        //   n*(n-1)/2 comparisons and writes, 0 swaps (InsertionSort uses Write, not Swap).
        //
        // Phase 2 (divide-and-conquer merge): picks median of smaller side, binary search
        //   in opposite side, rotate, recurse. 3-reversal uses Swap; k==1/k==n-1 fast paths use Write.
        var logN = Math.Log2(n);
        var minCompares = n <= 16 ? (ulong)(n * 4.0) : (ulong)(n * logN * 0.9);
        var maxCompares = n <= 16 ? (ulong)(n * 5.5) : (ulong)(n * logN * 2.0);

        var minWrites = n <= 16 ? (ulong)(n * 4.0) : (ulong)(n * logN * 1.0);
        var maxWrites = n <= 16 ? (ulong)(n * 6.0) : (ulong)(n * logN * 20.0);

        var minSwaps = 0UL;
        var maxSwaps = n <= 16 ? 0UL : (ulong)(n * logN * 2.0);

        var minReads = (ulong)(stats.CompareCount * 1.2);

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
        RotateMergeSort.Sort(random.AsSpan(), stats);

        // Bottom-up RotateMergeSort on random data:
        //
        // Phase 1 (InsertionSort blocks): n ≤ 16 → single block, variance depends on order.
        // Phase 2 (divide-and-conquer merge): picks median of smaller side, binary search
        //   in opposite side, rotate, recurse.
        var logN = Math.Log2(n);
        var minCompares = n <= 16 ? (ulong)(n * 1.5) : (ulong)(n * logN * 0.7);
        var maxCompares = n <= 16 ? (ulong)(n * 4.8) : (ulong)(n * logN * 2.0);

        var minWrites = n <= 16 ? (ulong)(n * 0.9) : (ulong)(n * logN * 0.5);
        var maxWrites = n <= 16 ? (ulong)(n * 4.8) : (ulong)(n * logN * 15.0);

        var minSwaps = 0UL;
        var maxSwaps = n <= 16 ? 0UL : (ulong)(n * logN * 2.0);

        var minReads = (ulong)(stats.CompareCount * 1.2);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);
        await Assert.That(stats.IndexReadCount >= minReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minReads}");
    }
}
