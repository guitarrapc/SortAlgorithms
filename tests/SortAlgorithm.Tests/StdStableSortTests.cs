using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class StdStableSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => StdStableSort.Sort(span, context);

    // No write/swap knob overrides: the old statistics test only checked the array length,
    // and sorted input above InPlaceThreshold still performs writes in the ping-pong merge.

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        StdStableSort.Sort(sorted.AsSpan(), stats);

        // StdStableSort for sorted data:
        // - n <= 16 (InPlaceThreshold): bypasses ArrayPool and calls InsertionSort directly.
        //   Corresponds to LLVM's __stable_sort_switch optimization.
        //   InsertionSort on sorted data does n-1 comparisons and 0 writes.
        // - n > 16: ping-pong recursive merge. Each merge of sorted halves exhausts the left
        //   half first (l2 comparisons), then CopyTo for the right half. All merges execute
        //   (no skip-merge optimization like MergeSort).
        //
        // Actual observations:
        // n=10:  9 comparisons, 0 writes    (InsertionSort path, n-1)
        // n=20:  40 comparisons, 80 writes
        // n=50:  115 comparisons, 200 writes
        // n=100: 312 comparisons, 600 writes
        //
        // Pattern for n > 16: comparisons ≈ 0.35–0.60 × n×log₂n; writes ≈ 0.60–1.05 × n×log₂n
        ulong minCompares, maxCompares, minWrites, maxWrites;
        if (n <= 16)
        {
            // InsertionSort on sorted data: exactly n-1 comparisons, 0 writes
            minCompares = (ulong)(n - 1);
            maxCompares = (ulong)(n - 1);
            minWrites = 0UL;
            maxWrites = 0UL;
        }
        else
        {
            var logN = Math.Log2(n);
            minCompares = (ulong)(n * logN * 0.35);
            maxCompares = (ulong)(n * logN * 0.60);
            minWrites = (ulong)(n * logN * 0.60);
            maxWrites = (ulong)(n * logN * 1.05);
        }

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount > 0).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be > 0");
        await Assert.That(stats.SwapCount).IsEqualTo(0UL); // StdStableSort never uses swaps
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
        StdStableSort.Sort(reversed.AsSpan(), stats);

        // StdStableSort for reversed data:
        // - n <= 16 (InPlaceThreshold): InsertionSort on fully reversed [n-1,...,0].
        //   Every element shifts all the way left: comparisons = n*(n-1)/2, writes ≈ n*(n+1)/2.
        // - n > 16: ping-pong merge. Fully reversed input causes near-maximum interleaving
        //   during merges, yielding more comparisons and n writes per merge level.
        //
        // Actual observations:
        // n=10:  45 comparisons, 54 writes   (InsertionSort: n*(n-1)/2, n*(n+1)/2-1)
        // n=20:  48 comparisons, 92 writes
        // n=50:  209 comparisons, 332 writes
        // n=100: 364 comparisons, 708 writes
        //
        // Pattern for n > 16: comparisons ≈ 0.45–0.85 × n×log₂n; writes ≈ 0.85–1.25 × n×log₂n
        ulong minCompares, maxCompares, minWrites, maxWrites;
        if (n <= 16)
        {
            // InsertionSort on reversed data: exactly n*(n-1)/2 comparisons
            minCompares = (ulong)(n * (n - 1) / 2);
            maxCompares = (ulong)(n * (n - 1) / 2);
            minWrites = (ulong)(n * (n - 1) / 2);
            maxWrites = (ulong)(n * (n + 1) / 2);
        }
        else
        {
            var logN = Math.Log2(n);
            minCompares = (ulong)(n * logN * 0.45);
            maxCompares = (ulong)(n * logN * 0.85);
            minWrites = (ulong)(n * logN * 0.85);
            maxWrites = (ulong)(n * logN * 1.25);
        }

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount > 0).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be > 0");
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
        StdStableSort.Sort(random.AsSpan(), stats);

        // StdStableSort for random data:
        // - n <= 16 (InPlaceThreshold): InsertionSort on arbitrary permutation.
        //   Comparisons range 0 (already sorted) to n*(n-1)/2 (fully reversed).
        // - n > 16: ping-pong recursive merge runs all merge levels (no skip optimization).
        //   Comparisons and writes depend on element interleaving within each merge.
        //
        // Observed range for random data:
        // n=10:  0–45 comparisons, 0–55 writes    (InsertionSort any permutation)
        // n=20:  30–60 comparisons, 50–100 writes
        // n=50:  100–280 comparisons, 160–350 writes
        // n=100: 235–580 comparisons, 390–800 writes
        //
        // Pattern for n > 16: comparisons ≈ 0.35–1.05 × n×log₂n; writes ≈ 0.55–1.20 × n×log₂n
        ulong minCompares, maxCompares, minWrites, maxWrites;
        if (n <= 16)
        {
            minCompares = 0UL;
            maxCompares = (ulong)(n * (n - 1) / 2);
            minWrites = 0UL;
            maxWrites = (ulong)(n * (n + 1) / 2);
        }
        else
        {
            var logN = Math.Log2(n);
            minCompares = (ulong)(n * logN * 0.35);
            maxCompares = (ulong)(n * logN * 1.05);
            minWrites = (ulong)(n * logN * 0.55);
            maxWrites = (ulong)(n * logN * 1.20);
        }

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount > 0).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be > 0");
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

    [Test]
    [Arguments(10, 9)]    // n <= 16: InsertionSort path, n-1 comparisons
    [Arguments(20, 40)]   // ping-pong merge
    [Arguments(50, 115)]
    [Arguments(100, 312)]
    public async Task TheoreticalComparisonCountTest(int n, int expectedComparisons)
    {
        // Test the exact comparison count for sorted data.
        // StdStableSort on sorted data is fully deterministic:
        // - n <= 16: InsertionSort → exactly n-1 comparisons (one check per element)
        // - n > 16: ping-pong recursive merge → each merge exhausts the left half first
        //   (l2 comparisons per merge), so the total is determined by the recursion tree.
        //
        // Recurrence (n > 16, sorted data):
        //   C(n) = C_Move(⌊n/2⌋) + C_Move(⌈n/2⌉) + ⌊n/2⌋   (StableSort)
        //   C_Move(n) = C(⌊n/2⌋) + C(⌈n/2⌉) + ⌊n/2⌋          (StableSortMove)
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        StdStableSort.Sort(sorted.AsSpan(), stats);

        await Assert.That(stats.CompareCount).IsEqualTo((ulong)expectedComparisons);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexReadCount >= stats.CompareCount * 2).IsTrue()
            .Because($"IndexReadCount ({stats.IndexReadCount}) should be >= CompareCount*2 ({stats.CompareCount * 2})");
    }
}
