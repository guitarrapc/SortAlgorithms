using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class StdStableSortTests : StableSortTestsBase
{
    // LLVM's __stable_sort_switch<T>::value for trivially-copyable T. int and StabilityTestItem
    // (two ints, no references) both take this branch, so every size below is measured against 128.
    private const int SwitchThreshold = 128;

    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => StdStableSort.Sort(span, context);

    // No write/swap knob overrides: the old statistics test only checked the array length,
    // and sorted input above SwitchThreshold still performs writes in the ping-pong merge.

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    [Arguments(129)]
    [Arguments(200)]
    [Arguments(300)]
    [Arguments(500)]
    [Arguments(1000)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        StdStableSort.Sort(sorted.AsSpan(), stats);

        // StdStableSort for sorted data:
        // - n <= 128 (__stable_sort_switch<int>::value): no buffer is rented and the range is
        //   insertion-sorted in place. InsertionSort on sorted data does n-1 comparisons and 0 writes.
        // - n > 128: ping-pong recursive merge down to in-place insertion sorts of 33..128 elements.
        //   Each merge of sorted halves exhausts the left half first (l2 comparisons), then CopyTo
        //   for the right half. All merges execute (no skip-merge optimization like MergeSort).
        //
        // Actual observations:
        // n=10:   9 comparisons, 0 writes     (InsertionSort path, n-1)
        // n=100:  99 comparisons, 0 writes
        // n=129:  253 comparisons, 258 writes (first size above the switch)
        // n=200:  396 comparisons, 400 writes
        // n=500:  996 comparisons, 1000 writes
        // n=1000: 2980 comparisons, 4000 writes
        //
        // Pattern for n > 128: comparisons ≈ 0.22–0.30 × n×log₂n; writes ≈ 0.22–0.41 × n×log₂n
        ulong minCompares, maxCompares, minWrites, maxWrites;
        if (n <= SwitchThreshold)
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
            minCompares = (ulong)(n * logN * 0.18);
            maxCompares = (ulong)(n * logN * 0.38);
            minWrites = (ulong)(n * logN * 0.18);
            maxWrites = (ulong)(n * logN * 0.50);
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
    [Arguments(129)]
    [Arguments(200)]
    [Arguments(300)]
    [Arguments(500)]
    [Arguments(1000)]
    public async Task TheoreticalValuesReversedTest(int n)
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        StdStableSort.Sort(reversed.AsSpan(), stats);

        // StdStableSort for reversed data:
        // - n <= 128: InsertionSort on fully reversed [n-1,...,0].
        //   Every element shifts all the way left: comparisons = n*(n-1)/2, writes ≈ n*(n+1)/2.
        // - n > 128: the recursion bottoms out at in-place insertion sorts of L ∈ (32,128] elements,
        //   and reversed input makes each leaf pay L*(L-1)/2 comparisons. Per element that is between
        //   33*32/2/33 = 16 and 128*127/2/128 ≈ 64, so the leaves alone contribute 16n..64n — a large
        //   constant factor, but still linear. The merge levels add at most n comparisons each.
        //   This mirrors LLVM: the 128 cut trades comparisons for cheap, cache-friendly shifting.
        //
        // Actual observations:
        // n=10:   45 comparisons, 54 writes      (InsertionSort: n*(n-1)/2, n*(n+1)/2-1)
        // n=100:  4950 comparisons, 5049 writes
        // n=129:  2146 comparisons, 2399 writes  (leaves are 33 elements → far cheaper than n=128)
        // n=200:  5100 comparisons, 5496 writes  (leaves are 50)
        // n=500:  31500 comparisons, 32496 writes (leaves are 125 → close to the 64n ceiling)
        // n=1000: 32756 comparisons, 35736 writes (leaves are 63)
        ulong minCompares, maxCompares, minWrites, maxWrites;
        if (n <= SwitchThreshold)
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
            minCompares = (ulong)(n * 16);
            maxCompares = (ulong)(n * 64 + n * logN);
            minWrites = (ulong)(n * 16);
            maxWrites = (ulong)(n * 64 + n * logN * 2);
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
    [Arguments(129, 42)]
    [Arguments(129, 1234)]
    [Arguments(200, 42)]
    [Arguments(200, 1234)]
    [Arguments(500, 42)]
    [Arguments(500, 1234)]
    [Arguments(1000, 42)]
    [Arguments(1000, 1234)]
    public async Task TheoreticalValuesRandomTest(int n, int seed)
    {
        var stats = new StatisticsContext();
        var random = TestHelpers.ShuffledRange(n, seed);
        StdStableSort.Sort(random.AsSpan(), stats);

        // StdStableSort for random data:
        // - n <= 128: InsertionSort on an arbitrary permutation.
        //   Comparisons range 0 (already sorted) to n*(n-1)/2 (fully reversed).
        // - n > 128: leaves are insertion sorts of L ∈ (32,128] on random data, costing ≈ L²/4 each
        //   (≈ 8..32 comparisons per element), plus at most n comparisons per merge level.
        //   The bounds below keep the reversed-case ceiling and drop the floor to allow lucky runs.
        //
        // Observed range for random data:
        // n=10:   0–45 comparisons, 0–55 writes        (InsertionSort, any permutation)
        // n=129:  ~1300 comparisons, ~1300 writes
        // n=500:  ~16300 comparisons, ~16300 writes
        // n=1000: ~20300 comparisons, ~20400 writes
        ulong minCompares, maxCompares, minWrites, maxWrites;
        if (n <= SwitchThreshold)
        {
            minCompares = 0UL;
            maxCompares = (ulong)(n * (n - 1) / 2);
            minWrites = 0UL;
            maxWrites = (ulong)(n * (n + 1) / 2);
        }
        else
        {
            var logN = Math.Log2(n);
            minCompares = (ulong)(n * 4);
            maxCompares = (ulong)(n * 64 + n * logN);
            minWrites = (ulong)(n * 4);
            maxWrites = (ulong)(n * 64 + n * logN * 2);
        }

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount > 0).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be > 0");
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

    [Test]
    [Arguments(10, 9)]      // n <= 128: InsertionSort path, n-1 comparisons
    [Arguments(100, 99)]
    [Arguments(128, 127)]   // last size handled entirely in place
    [Arguments(129, 253)]   // first size that rents a buffer and merges
    [Arguments(200, 396)]
    [Arguments(500, 996)]
    [Arguments(1000, 2980)]
    public async Task TheoreticalComparisonCountTest(int n, int expectedComparisons)
    {
        // Test the exact comparison count for sorted data.
        // StdStableSort on sorted data is fully deterministic:
        // - n <= 128: InsertionSort → exactly n-1 comparisons (one check per element)
        // - n > 128: ping-pong recursive merge → each merge exhausts the left half first
        //   (l2 comparisons per merge), so the total is determined by the recursion tree.
        //
        // Recurrence (sorted data), with B = __stable_sort_switch<int>::value = 128:
        //   C(n)      = n - 1                                     (n <= B, in-place insertion sort)
        //   C(n)      = C_Move(⌊n/2⌋) + C_Move(⌈n/2⌉) + ⌊n/2⌋      (StableSort, n > B)
        //   C_Move(n) = C(⌊n/2⌋) + C(⌈n/2⌉) + ⌊n/2⌋                (StableSortMove, n > 8)
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        StdStableSort.Sort(sorted.AsSpan(), stats);

        await Assert.That(stats.CompareCount).IsEqualTo((ulong)expectedComparisons);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexReadCount >= stats.CompareCount * 2).IsTrue()
            .Because($"IndexReadCount ({stats.IndexReadCount}) should be >= CompareCount*2 ({stats.CompareCount * 2})");
    }

    [Test]
    [Arguments(129)]
    [Arguments(1000)]
    [Arguments(5000)]
    public async Task StabilityAboveSwitchThresholdTest(int n)
    {
        // The inherited stability tests use 6-element inputs, which stay below the 128 switch and
        // therefore only exercise the in-place insertion sort. This covers the ping-pong merge path:
        // duplicate-heavy keys force interleaving in every merge, where taking from the right half on
        // ties would break stability.
        var items = Enumerable.Range(0, n)
            .Select(i => new StabilityTestItem(i % 10, i))
            .ToArray();

        StdStableSort.Sort(items.AsSpan(), new StatisticsContext());

        for (var i = 1; i < items.Length; i++)
        {
            await Assert.That(items[i].Value).IsGreaterThanOrEqualTo(items[i - 1].Value);
            if (items[i].Value == items[i - 1].Value)
            {
                await Assert.That(items[i].OriginalIndex).IsGreaterThan(items[i - 1].OriginalIndex);
            }
        }
    }
}
