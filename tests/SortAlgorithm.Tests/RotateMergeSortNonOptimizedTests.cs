using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

// Class-level SkipCI: this reference (non-optimized) variant is skipped in CI, including all inherited base tests.
[InheritsTests]
[SkipCI]
public class RotateMergeSortNonOptimizedTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => RotateMergeSortNonOptimized.Sort(span, context);

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
        RotateMergeSortNonOptimized.Sort(sorted.AsSpan(), stats);

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
        RotateMergeSortNonOptimized.Sort(reversed.AsSpan(), stats);

        // Rotate Merge Sort comparisons for reversed data:
        // Reversed data requires all merge operations with binary search + rotation.
        // n ≤ 16: entirely handled by InsertionSort
        // n > 16: divide-and-conquer merge with binary search + GCD-cycle rotation
        var logN = Math.Log2(n);
        var minCompares = n <= 16 ? (ulong)(n * 4.0) : (ulong)(n * logN * 0.8);
        var maxCompares = n <= 16 ? (ulong)(n * 5.5) : (ulong)(n * logN * 2.5);

        // Writes: GCD-cycle rotation uses Write operations only
        // n ≤ 16: InsertionSort writes only
        // n > 16: GCD-cycle rotation adds many writes
        var minWrites = n <= 16 ? (ulong)(n * 4.0) : (ulong)(n * logN * 1.4);
        var maxWrites = n <= 16 ? (ulong)(n * 6.0) : (ulong)(n * logN * 20.0);

        // Swaps: GCD-cycle rotation uses Write only, but divide-and-conquer merge
        // base case (len1==1 && len2==1) uses Swap. For n ≤ 16 InsertionSort does 0 swaps.
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
        RotateMergeSortNonOptimized.Sort(random.AsSpan(), stats);

        // Rotate Merge Sort (NonOptimized) for random data:
        // This version uses divide-and-conquer merge (median of smaller side + binary search + rotate),
        // InsertionSort for small subarrays, and GCD-cycle rotation.
        //
        // For n ≤ 16: entirely handled by InsertionSort (no merge occurs)
        // For n > 16: divide-and-conquer merge with binary search + rotation
        //
        // Pattern for random: approximately 0.8 * n * log₂(n) to 2.5 * n * log₂(n) for n > 16
        // For n ≤ 16: InsertionSort profile applies
        var logN = Math.Log2(n);
        var minCompares = n <= 16 ? (ulong)(n * 1.0) : (ulong)(n * logN * 0.4);
        var maxCompares = n <= 16 ? (ulong)(n * 5.0) : (ulong)(n * logN * 2.5);

        // Writes vary based on how much rotation is needed
        // n ≤ 16: InsertionSort writes only, can be low for partially sorted data
        // n > 16: GCD-cycle rotation adds many writes
        var minWrites = n <= 16 ? (ulong)(n * 0.5) : (ulong)(n * logN * 0.5);
        var maxWrites = n <= 16 ? (ulong)(n * 5.0) : (ulong)(n * logN * 15.0);

        // Swaps: GCD-cycle rotation uses Write only; however the divide-and-conquer merge
        // base case (len1==1 && len2==1) uses Swap. For n ≤ 16 InsertionSort does 0 swaps.
        var minSwaps = 0UL;  // Could be low if data is partially sorted
        var maxSwaps = n <= 16 ? 0UL : (ulong)(n * logN * 8.0);

        // IndexReads: Reduced due to InsertionSort optimization (caching values to reduce repeated reads)
        // Expected: approximately 1.2x comparisons (down from 2x)
        var minReads = (ulong)(stats.CompareCount * 1.2);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);
        await Assert.That(stats.IndexReadCount >= minReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minReads}");
    }
}
