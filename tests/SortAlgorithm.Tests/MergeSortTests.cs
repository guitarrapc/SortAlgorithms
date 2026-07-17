using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class MergeSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => MergeSort.Sort(span, context);

    // Sorted input passes the "skip merge if already sorted" check at every level: no writes.
    protected override CountExpectation SortedInputWrites => CountExpectation.Zero;
    // MergeSort copies through the buffer with Read/Write; it never uses Swap.
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
        MergeSort.Sort(sorted.AsSpan(), stats);

        // Merge Sort with optimization for sorted data:
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

        // Merge Sort writes with optimization:
        // For sorted data, merges are skipped, so writes = 0
        var minWrites = 0UL;
        var maxWrites = 0UL;

        // Reads for sorted data: Only skip-check comparisons
        // Each comparison reads 2 elements
        var minReads = stats.CompareCount * 2;

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount >= minReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minReads}");
        await Assert.That(stats.SwapCount).IsEqualTo(0UL); // Merge Sort doesn't use swaps
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
        MergeSort.Sort(reversed.AsSpan(), stats);

        // Merge Sort comparisons for reversed data with optimization:
        // Reversed data cannot skip merges, so all merge operations occur.
        // However, some small partitions might already be sorted after recursion.
        //
        // Actual observations for reversed data with optimization:
        // n=10:  28 comparisons   (includes skip checks for small partitions)
        // n=20:  ~60-70 comparisons
        // n=50:  ~180-220 comparisons
        // n=100: ~420-500 comparisons
        //
        // Pattern for reversed: approximately 0.5 * n * log₂(n) to 1.0 * n * log₂(n)
        var logN = Math.Log2(n);
        var minCompares = (ulong)(n * logN * 0.5);
        var maxCompares = (ulong)(n * logN * 1.0);

        var minWrites = (ulong)(n * logN * 0.3);
        var maxWrites = (ulong)(n * Math.Ceiling(logN) * 1.5);

        var minReads = stats.CompareCount * 2;

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount >= minReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minReads}");
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
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
        MergeSort.Sort(random.AsSpan(), stats);

        // Merge Sort with optimization for random data:
        // Random data can have some sorted partitions, allowing skip optimization.
        // Comparisons vary based on how many partitions are already sorted.
        //
        // Observed range for random data with optimization:
        // n=10:  ~20-35 comparisons (some partitions may be sorted)
        // n=20:  ~50-80 comparisons
        // n=50:  ~150-250 comparisons
        // n=100: ~350-600 comparisons
        //
        // Pattern for random: approximately 0.5 * n * log₂(n) to 1.0 * n * log₂(n)
        // (wider range due to randomness and optimization)
        var logN = Math.Log2(n);
        var minCompares = (ulong)(n * logN * 0.5);
        var maxCompares = (ulong)(n * logN * 1.1);

        var minWrites = (ulong)(n * logN * 0.3);
        var maxWrites = (ulong)(n * Math.Ceiling(logN) * 1.5);

        var minReads = stats.CompareCount * 2;

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount >= minReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minReads}");
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }
}
