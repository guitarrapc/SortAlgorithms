using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class NaturalMergeSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => NaturalMergeSort.Sort(span, context);

    // Natural Merge Sort on sorted data: single ascending run, no merge needed.
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
        NaturalMergeSort.Sort(sorted.AsSpan(), stats);

        // Natural Merge Sort on sorted data:
        // DetectRuns finds a single ascending run covering the entire array.
        // 1 direction-check comparison + (n-2) extension comparisons in the while loop = n-1.
        // But the direction-check re-compares the same pair as the first loop iteration,
        // so the total observed is n comparisons.
        //
        // Actual observations for sorted data:
        // n=10:  10 comparisons
        // n=20:  20 comparisons
        // n=50:  50 comparisons
        // n=100: 100 comparisons
        //
        // With runCount=1 the algorithm breaks immediately: 0 writes, 0 swaps.
        var expectedCompares = (ulong)n;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
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
        NaturalMergeSort.Sort(reversed.AsSpan(), stats);

        // Natural Merge Sort on reversed data (with descending run detection):
        // DetectRuns finds a single strictly descending run covering the entire array,
        // then reverses it in-place via n/2 swaps. runCount=1, so no merge is needed.
        //
        // Comparisons: n (same as sorted — 1 direction check + n-2 extension + 1 overlap)
        // Swaps: n/2 (reverse the descending run)
        // Writes: n (each swap is tracked as 2 writes, n/2 swaps × 2 = n)
        //
        // Actual observations for reversed data:
        // n=10:  10 comparisons, 5 swaps, 10 writes
        // n=20:  20 comparisons, 10 swaps, 20 writes
        // n=50:  50 comparisons, 25 swaps, 50 writes
        // n=100: 100 comparisons, 50 swaps, 100 writes
        var expectedCompares = (ulong)n;
        var expectedSwaps = (ulong)(n / 2);
        var expectedWrites = (ulong)n;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(expectedSwaps);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
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
        NaturalMergeSort.Sort(random.AsSpan(), stats);

        // Natural Merge Sort on random data:
        // Random data has ~n/2 natural runs, requiring ~log₂(n) merge passes.
        // Each pass scans n elements for run detection + merges.
        //
        // Observed range for random data:
        // n=10:  ~40-60 comparisons
        // n=20:  ~100-160 comparisons
        // n=50:  ~400-600 comparisons
        // n=100: ~1000-1400 comparisons
        //
        // Pattern: approximately 1.0 * n * log₂(n) to 2.0 * n * log₂(n)
        // (includes run detection comparisons at each pass)
        var logN = Math.Log2(n);
        var minCompares = (ulong)(n * logN * 0.8);
        var maxCompares = (ulong)(n * logN * 2.5);

        var minWrites = (ulong)(n * logN * 0.3);
        var maxWrites = (ulong)(n * Math.Ceiling(logN) * 2.0);

        var minReads = stats.CompareCount * 2;

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount >= minReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minReads}");
    }
}
