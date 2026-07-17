using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class BottomupMergeSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => BottomupMergeSort.Sort(span, context);

    // No write/swap knob overrides: the old statistics test asserted no operation counters.
    // Ping-pong buffering copies all n elements every pass even when merges are skipped.

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        BottomupMergeSort.Sort(sorted.AsSpan(), stats);

        // Bottom-Up Merge Sort (Ping-Pong) with optimization for sorted data:
        // With ping-pong buffering, each pass writes all n elements to dst buffer (via CopyTo).
        // Even when merges are skipped, the entire array is copied to maintain ping-pong invariant.
        //
        // Comparisons for sorted data with optimization:
        // - Pass 1 (size 1→2): n/2 skip checks
        // - Pass 2 (size 2→4): n/4 skip checks
        // - Pass k: n/2^k skip checks
        // Total: n/2 + n/4 + n/8 + ... ≈ n-1 comparisons
        //
        // Pattern for sorted data: n-1 comparisons (skip checks only)
        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(n);

        // Writes for sorted data (Ping-Pong):
        // Each pass copies all n elements even when skipping merges.
        // Total passes: ⌈log₂(n)⌉
        // Total writes: n * ⌈log₂(n)⌉
        // n=10:  30-40 (n * 3-4 passes)
        // n=20:  80-100 (n * 4-5 passes)
        // n=50:  280-300 (n * 5.6-6 passes)
        // n=100: 600-700 (n * 6-7 passes)
        var logN = Math.Ceiling(Math.Log2(n));
        var minWrites = (ulong)(n * logN * 0.9);
        var maxWrites = (ulong)(n * (logN + 1) * 1.1);

        // Reads for sorted data: Skip-check comparisons (2 reads per compare) + CopyTo reads
        // CopyTo reads: n * ⌈log₂(n)⌉
        var minReads = (ulong)(n * logN * 0.9);

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
        BottomupMergeSort.Sort(reversed.AsSpan(), stats);

        // Bottom-Up Merge Sort (Ping-Pong) for reversed data:
        // Reversed data requires all merges (no skipping).
        // Each pass performs merges and writes all n elements to dst.
        //
        // Comparisons: ~0.5-0.75 * n * log₂(n) (unchanged from non-ping-pong)
        var logN = Math.Log2(n);
        var minCompares = (ulong)(n * logN * 0.5);
        var maxCompares = (ulong)(n * logN * 0.8);

        // Writes for reversed data (Ping-Pong):
        // Each pass writes all n elements (merge writes + remaining CopyTo).
        // Total: n * ⌈log₂(n)⌉
        // n=10:  30-40, n=20:  80-100, n=50:  280-300, n=100: 600-700
        var ceilLogN = Math.Ceiling(logN);
        var minWrites = (ulong)(n * ceilLogN * 0.9);
        var maxWrites = (ulong)(n * (ceilLogN + 1) * 1.1);

        // Reads: comparisons (2 per compare) + merge reads + CopyTo reads
        // Total reads: approximately n * log₂(n) * 1.5-2.5
        var minReads = (ulong)(n * logN * 1.0);

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
        BottomupMergeSort.Sort(random.AsSpan(), stats);

        // Bottom-Up Merge Sort (Ping-Pong) for random data:
        // Random data has some skip opportunities but mostly requires merges.
        // Comparisons: ~0.75-1.1 * n * log₂(n) (unchanged)
        var logN = Math.Log2(n);
        var minCompares = (ulong)(n * logN * 0.5);
        var maxCompares = (ulong)(n * logN * 1.15);

        // Writes for random data (Ping-Pong):
        // Each pass writes all n elements.
        // Total: n * ⌈log₂(n)⌉
        // n=10:  30-40, n=20:  80-100, n=50:  280-300, n=100: 600-700
        var ceilLogN = Math.Ceiling(logN);
        var minWrites = (ulong)(n * ceilLogN * 0.9);
        var maxWrites = (ulong)(n * (ceilLogN + 1) * 1.1);

        // Reads: comparisons + merge reads + CopyTo reads
        // Total: approximately n * log₂(n) * 1.5-2.5
        var minReads = (ulong)(n * logN * 1.0);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount >= minReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minReads}");
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }
}
