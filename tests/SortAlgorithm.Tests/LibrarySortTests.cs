using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class LibrarySortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => LibrarySort.Sort(span, context);

    // Rebalancing cost grows quickly: keep data-driven inputs small.
    protected override int MaxOrderTestSize => 512;

    // Library sort always writes to the auxiliary array (gap init + placement + extraction) and never swaps.
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
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
        LibrarySort.Sort(sorted.AsSpan(), stats);

        // LibrarySort behavior on sorted data:
        // - For small arrays (n ≤ 32): Falls back to InsertionSort
        //   - Best case O(n): n-1 comparisons, 0 writes (no shifts needed)
        // - For larger arrays (n > 32): LibrarySort with InsertionSort warmup
        //   - CompareCount comes only from warmup phase (BinarySearch uses plain comparer, not tracked)
        ulong minCompares, maxCompares;
        if (n <= 32)
        {
            minCompares = (ulong)(n - 1);
            maxCompares = (ulong)(n - 1); // Exact: sorted InsertionSort = n-1 comparisons
        }
        else
        {
            minCompares = 1UL;
            maxCompares = (ulong)(3 * n * Math.Max(1, Math.Log(n, 2)));
        }

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL); // LibrarySort never uses swaps

        if (n <= 32)
        {
            // Sorted InsertionSort: no element shifts → no writes to main span
            await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
            // IndexReadCount: 2*(n-1) reads (1 for tmp + 1 for first comparison per iteration)
            await Assert.That(stats.IndexReadCount).IsEqualTo((ulong)(2 * (n - 1)));
        }
        else
        {
            // LibrarySort writes extensively to aux array (gap init + element placement + extraction)
            await Assert.That(stats.IndexWriteCount).IsGreaterThan(0UL);
            await Assert.That(stats.IndexReadCount).IsGreaterThan(0UL);
        }
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
        LibrarySort.Sort(reversed.AsSpan(), stats);

        // LibrarySort behavior on reversed data:
        // - For small arrays (n ≤ 32): Falls back to InsertionSort
        //   - Worst case O(n²): n(n-1)/2 comparisons exactly
        // - For larger arrays (n > 32): LibrarySort with InsertionSort warmup on reversed prefix
        //   - CompareCount from warmup = 32*31/2 = 496 (reversed InsertionSort worst case)
        //   - BinarySearch uses plain comparer (not tracked in CompareCount)
        ulong minCompares, maxCompares;
        if (n <= 32)
        {
            minCompares = (ulong)(n * (n - 1) / 2);
            maxCompares = (ulong)(n * (n - 1) / 2); // Exact: reversed InsertionSort = n(n-1)/2 comparisons
        }
        else
        {
            minCompares = (ulong)n;
            maxCompares = (ulong)(n * n);
        }

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL); // LibrarySort never uses swaps

        // IndexReads: each comparison reads an element (plus extra reads for shifts in InsertionSort path)
        var minIndexReads = stats.CompareCount;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= CompareCount ({minIndexReads})");
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
        LibrarySort.Sort(random.AsSpan(), stats);

        // LibrarySort behavior on random data:
        // - For small arrays (n ≤ 32): Falls back to InsertionSort
        //   - Average case O(n²): approximately n²/4 comparisons
        // - For larger arrays (n > 32): LibrarySort with InsertionSort warmup
        //   - O(n log n) expected due to binary search and gap-based insertion
        //   - CompareCount from warmup only: min=31 (sorted warmup), max=496 (reversed warmup)
        // Use Math.Min(n, 32) - 1 as lower bound to handle both the InsertionSort and LibrarySort paths
        ulong minCompares = (ulong)(Math.Min(n, 32) - 1);
        ulong maxCompares = (ulong)(n * n);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL); // LibrarySort never uses swaps

        var minIndexReads = stats.CompareCount;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= CompareCount ({minIndexReads})");
    }

    [Test]
    [Arguments(5)]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    public async Task TheoreticalValuesSameElementsTest(int n)
    {
        var stats = new StatisticsContext();
        var sameValues = Enumerable.Repeat(42, n).ToArray();
        LibrarySort.Sort(sameValues.AsSpan(), stats);

        // LibrarySort behavior on same elements:
        // - For small arrays (n ≤ 32): Falls back to InsertionSort
        //   - Best case O(n): n-1 comparisons (equal elements never shift)
        // - For larger arrays (n > 32): LibrarySort with warmup on same elements
        //   - Warmup on same elements = 31 comparisons (same as sorted warmup)
        // Use Math.Min(n, 32) - 1 as lower bound to handle both paths uniformly
        ulong minCompares = (ulong)(Math.Min(n, 32) - 1);
        ulong maxCompares = (ulong)(n * Math.Max(1, (int)Math.Log(n, 2)) * 3);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);

        // Verify all values remain correct
        foreach (var item in sameValues) await Assert.That(item).IsEqualTo(42);

        await Assert.That(stats.SwapCount).IsEqualTo(0UL); // LibrarySort never uses swaps
    }
}
