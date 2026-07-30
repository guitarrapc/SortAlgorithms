using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class LibrarySortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => LibrarySort.Sort(span, context);

    // The position index is maintained by memmove, so the sort is quadratic in n.
    // 10000 is the largest standard input and still runs in ~15 ms; 100000 already takes ~135 ms.
    protected override int MaxOrderTestSize => 10000;

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
        //   - CompareCount covers the warmup plus one binary search per inserted element
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
        //     plus one binary search per inserted element
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

    /// <summary>
    /// Regression: the shift path used to fall back to <c>shiftGap = auxEnd++</c> without checking
    /// that the slot was a gap, while the gap path never advanced auxEnd past a slot it had filled.
    /// This input drove both, overwriting element 41 with a copy of 40.
    /// </summary>
    [Test]
    public async Task GapPathDoesNotStrandAuxEndTest()
    {
        int[] array = [20, 18, 26, 30, 32, 23, 8, 2, 38, 3, 37, 1, 34, 17, 5, 14, 19, 27, 31, 22, 36, 4, 33, 12, 40, 24, 35, 0, 29, 28, 16, 25, 13, 39, 10, 9, 41, 15, 7, 11, 21, 6];

        LibrarySort.Sort(array.AsSpan());

        await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, 42).ToArray(), CollectionOrdering.Matching);
    }

    /// <summary>
    /// Every element must survive the auxiliary buffer: gap clustering must never let an insertion
    /// overwrite an occupied slot. Sweeps sizes around the InsertionSort threshold, the initial
    /// gap range, and the rebalance thresholds.
    /// </summary>
    [Test]
    [Arguments(33)]
    [Arguments(34)]
    [Arguments(48)]
    [Arguments(49)]
    [Arguments(64)]
    [Arguments(128)]
    [Arguments(129)]
    [Arguments(512)]
    [Arguments(2048)]
    public async Task NoElementLossTest(int n)
    {
        var expected = Enumerable.Range(0, n).ToArray();

        // Multiple seeds: gap clustering depends on the permutation, not just the size.
        foreach (var seed in (int[])[42, 1234, 20250731, 7, 99])
        {
            var array = TestHelpers.ShuffledRange(n, seed);

            LibrarySort.Sort(array.AsSpan());

            await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching).Because($"n={n}, seed={seed}");
        }
    }

    /// <summary>
    /// Duplicate-heavy input packs many equal keys into the same gap region, which is where
    /// clustering is worst. Verifies both the multiset and the stable order of equal keys
    /// on inputs large enough to leave the InsertionSort fallback. The element type carries a
    /// reference, so this also exercises the pooled-buffer clearing path.
    /// </summary>
    [Test]
    [Arguments(64)]
    [Arguments(200)]
    [Arguments(1000)]
    public async Task StabilityAboveSmallSortThresholdTest(int n)
    {
        foreach (var seed in (int[])[42, 1234, 7])
        {
            var random = new Random(seed);
            var items = Enumerable.Range(0, n).Select(i => new StabilityTestItemWithId(random.Next(0, Math.Max(2, n / 8)), i.ToString())).ToArray();
            var expected = items.OrderBy(x => x.Key).ToArray(); // OrderBy is a stable sort

            LibrarySort.Sort(items.AsSpan(), new StatisticsContext());

            for (var i = 0; i < n; i++)
            {
                await Assert.That(items[i].Key).IsEqualTo(expected[i].Key).Because($"n={n}, seed={seed}, i={i}");
                await Assert.That(items[i].Id).IsEqualTo(expected[i].Id).Because($"n={n}, seed={seed}, i={i}");
            }
        }
    }

    /// <summary>
    /// The binary search over the position index performs the bulk of the comparisons.
    /// It must report them, otherwise statistics and visualization consumers see almost nothing.
    /// </summary>
    [Test]
    public async Task BinarySearchComparisonsAreReportedTest()
    {
        const int n = 1000;
        var invoked = 0UL;
        var comparer = Comparer<int>.Create((a, b) => { invoked++; return a.CompareTo(b); });
        var stats = new StatisticsContext();
        var array = TestHelpers.ShuffledRange(n, 42);

        LibrarySort.Sort(array.AsSpan(), comparer, stats);

        // Every comparison the algorithm makes is routed through the context.
        await Assert.That(stats.CompareCount).IsEqualTo(invoked);
        // Sanity check that the binary searches are actually in there (warmup alone is < 32^2/2).
        await Assert.That(stats.CompareCount).IsGreaterThan((ulong)(n * 4));
    }

    /// <summary>
    /// The auxiliary buffer holds plain elements, not a private gap wrapper, so a visualization
    /// consumer can render its writes. Gaps are tracked outside the element buffer and therefore
    /// produce no element writes of their own.
    /// </summary>
    [Test]
    public async Task AuxiliaryBufferReportsElementValuesTest()
    {
        const int auxBufferId = 1;
        var auxWrites = 0;
        var mainWrites = 0;
        var nonElementValues = 0;
        var context = new VisualizationContext(onIndexWrite: (index, bufferId, value) =>
        {
            if (bufferId == auxBufferId) auxWrites++;
            else if (bufferId == 0) mainWrites++;
            if (value is not int) nonElementValues++;
        });
        var array = TestHelpers.ShuffledRange(200, 42);

        LibrarySort.Sort(array.AsSpan(), context);

        await Assert.That(auxWrites).IsGreaterThan(0);
        await Assert.That(mainWrites).IsGreaterThan(0);
        await Assert.That(nonElementValues).IsEqualTo(0).Because("aux writes must carry the element value, not a gap wrapper");
    }
}
