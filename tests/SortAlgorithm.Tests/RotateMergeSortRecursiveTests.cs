using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class RotateMergeSortRecursiveTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => RotateMergeSortRecursive.Sort(span, context);

    // Rotation-based merge is O(n log^2 n); keep data-driven tests on small inputs.
    protected override int MaxOrderTestSize => 1024;

    // Sorted input triggers the skip-merge optimization at every recursion level: no writes, no rotations.
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
        RotateMergeSortRecursive.Sort(sorted.AsSpan(), stats);

        // Rotate Merge Sort with optimization for sorted data:
        // With the "skip merge if already sorted" optimization,
        // sorted data only requires skip-check comparisons (one per recursive call).
        //
        // Theoretical bounds with optimization:
        // - Sorted data: n-1 comparisons (one skip-check per partition boundary)
        //   At each recursion level with k partitions, we do k-1 skip checks.
        //   Total: (n-1) comparisons for completely sorted data
        //
        // Actual observations with optimization for sorted data:
        // n=10:  9 comparisons    (n-1)
        // n=20:  19 comparisons   (n-1)
        // n=50:  49 comparisons   (n-1)
        // n=100: 99 comparisons   (n-1)
        //
        // Pattern for sorted data: n-1 comparisons (skip checks only)
        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(n);

        // Rotate Merge Sort writes with optimization:
        // For sorted data, merges are skipped, so writes = 0
        var minWrites = 0UL;
        var maxWrites = 0UL;

        // Reads for sorted data: Only skip-check comparisons
        // Each comparison reads 2 elements
        var minReads = stats.CompareCount * 2;

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount >= minReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minReads}");
        await Assert.That(stats.SwapCount).IsEqualTo(0UL); // Sorted data: no rotation needed
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
        RotateMergeSortRecursive.Sort(reversed.AsSpan(), stats);

        // Rotate Merge Sort with divide-and-conquer merge for reversed data:
        // Picks median of smaller side, binary search in opposite side, rotate, recurse.
        // Small subarrays (≤16) use insertion sort.
        // Completely-disjoint skip: when every left element > every right element, a single rotation
        // resolves the merge, reducing comparisons noticeably on reversed inputs.
        //
        // n≤16: ~4.0n to ~5.5n (insertion sort)
        // n>16: ~0.8 * n * log₂(n) to ~2.0 * n * log₂(n)
        var logN = Math.Log2(n);
        var minCompares = n <= 16 ? (ulong)(n * 4.0) : (ulong)(n * logN * 0.8);
        var maxCompares = n <= 16 ? (ulong)(n * 5.5) : (ulong)(n * logN * 2.0);

        // Writes: 3-reversal rotation and merge base case
        var minWrites = n <= 16 ? (ulong)(n * 4.0) : (ulong)(n * logN * 1.0);
        var maxWrites = n <= 16 ? (ulong)(n * 6.0) : (ulong)(n * logN * 20.0);

        // Swaps: 3-reversal rotation uses swaps; merge base case (len1==1 && len2==1) uses Swap
        // n≤16: insertion sort handles everything - 0 swaps
        var minSwaps = 0UL;
        var maxSwaps = n <= 16 ? 0UL : (ulong)(n * logN * 2.0);

        // IndexReads: Reduced due to InsertionSort optimization (caching values to reduce repeated reads)
        // Expected: approximately 1.2x comparisons (down from 2x)
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
        RotateMergeSortRecursive.Sort(random.AsSpan(), stats);

        // Rotate Merge Sort with divide-and-conquer merge for random data:
        // Picks median of smaller side, binary search in opposite side, rotate, recurse.
        // Small subarrays (≤16) use insertion sort.
        // Performance varies based on initial order.
        var logN = Math.Log2(n);
        var minCompares = n <= 16 ? (ulong)(n * 1.5) : (ulong)(n * logN * 0.7);
        var maxCompares = n <= 16 ? (ulong)(n * 4.0 * 1.2) : (ulong)(n * logN * 2.0);

        // Writes vary based on how much rotation is needed
        var minWrites = n <= 16 ? (ulong)(n * 1.5 * 0.6) : (ulong)(n * logN * 0.5);
        var maxWrites = n <= 16 ? (ulong)(n * 4.0 * 1.2) : (ulong)(n * logN * 15.0);

        // Swaps: 3-reversal rotation uses swaps; merge base case uses Swap
        // n≤16: insertion sort handles everything - 0 swaps
        var minSwaps = 0UL;
        var maxSwaps = n <= 16 ? 0UL : (ulong)(n * logN * 2.0);

        // IndexReads: Reduced due to InsertionSort optimization (caching values to reduce repeated reads)
        // Expected: approximately 1.2x comparisons (down from 2x)
        var minReads = (ulong)(stats.CompareCount * 1.2);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);
        await Assert.That(stats.IndexReadCount >= minReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minReads}");
    }
}
