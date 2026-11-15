using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class LibrarySortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => LibrarySort.Sort(span, context);


    // Library sort always writes to the auxiliary array (placement + shifts + extraction) and never swaps.
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var input = Enumerable.Range(0, n).ToArray();
        var expected = input.Order().ToArray();
        var stats = new StatisticsContext();
        LibrarySort.Sort(input.AsSpan(), stats);
        await Assert.That(input.SequenceEqual(expected)).IsTrue();
        // Each insertion bisects a prefix of at most 2n slots.
        var perInsertion = (ulong)Math.Ceiling(Math.Log2(2L * n + 1));
        await Assert.That(stats.CompareCount).IsBetween((ulong)(n - 1), (ulong)(n - 1) * perInsertion);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        // Input capture and extraction are separate from the search reads and shifts.
        await Assert.That(stats.IndexReadCount).IsGreaterThanOrEqualTo(stats.CompareCount + (ulong)(2 * n));
        await Assert.That(stats.IndexWriteCount).IsGreaterThanOrEqualTo((ulong)(2 * n));
    }

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesReversedTest(int n)
    {
        var input = Enumerable.Range(0, n).Reverse().ToArray();
        var expected = input.Order().ToArray();
        var stats = new StatisticsContext();
        LibrarySort.Sort(input.AsSpan(), stats);
        await Assert.That(input.SequenceEqual(expected)).IsTrue();
        // Each insertion bisects a prefix of at most 2n slots.
        var perInsertion = (ulong)Math.Ceiling(Math.Log2(2L * n + 1));
        await Assert.That(stats.CompareCount).IsBetween((ulong)(n - 1), (ulong)(n - 1) * perInsertion);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        // Input capture and extraction are separate from the search reads and shifts.
        await Assert.That(stats.IndexReadCount).IsGreaterThanOrEqualTo(stats.CompareCount + (ulong)(2 * n));
        await Assert.That(stats.IndexWriteCount).IsGreaterThanOrEqualTo((ulong)(2 * n));
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
        var input = TestHelpers.ShuffledRange(n, seed);
        var expected = input.Order().ToArray();
        var stats = new StatisticsContext();
        LibrarySort.Sort(input.AsSpan(), stats);
        await Assert.That(input.SequenceEqual(expected)).IsTrue();
        // Each insertion bisects a prefix of at most 2n slots.
        var perInsertion = (ulong)Math.Ceiling(Math.Log2(2L * n + 1));
        await Assert.That(stats.CompareCount).IsBetween((ulong)(n - 1), (ulong)(n - 1) * perInsertion);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        // Input capture and extraction are separate from the search reads and shifts.
        await Assert.That(stats.IndexReadCount).IsGreaterThanOrEqualTo(stats.CompareCount + (ulong)(2 * n));
        await Assert.That(stats.IndexWriteCount).IsGreaterThanOrEqualTo((ulong)(2 * n));
    }

    [Test]
    [Arguments(5)]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    public async Task TheoreticalValuesSameElementsTest(int n)
    {
        var input = Enumerable.Repeat(42, n).ToArray();
        var expected = input.Order().ToArray();
        var stats = new StatisticsContext();
        LibrarySort.Sort(input.AsSpan(), stats);
        await Assert.That(input.SequenceEqual(expected)).IsTrue();
        // Each insertion bisects a prefix of at most 2n slots.
        var perInsertion = (ulong)Math.Ceiling(Math.Log2(2L * n + 1));
        await Assert.That(stats.CompareCount).IsBetween((ulong)(n - 1), (ulong)(n - 1) * perInsertion);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        // Input capture and extraction are separate from the search reads and shifts.
        await Assert.That(stats.IndexReadCount).IsGreaterThanOrEqualTo(stats.CompareCount + (ulong)(2 * n));
        await Assert.That(stats.IndexWriteCount).IsGreaterThanOrEqualTo((ulong)(2 * n));
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
    /// gap range, the rebalance thresholds, and the 64-slot word boundaries of the occupancy bitset.
    /// </summary>
    [Test]
    [Arguments(33)]
    [Arguments(34)]
    [Arguments(43)]
    [Arguments(48)]
    [Arguments(49)]
    [Arguments(63)]
    [Arguments(64)]
    [Arguments(65)]
    [Arguments(127)]
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
    /// Stability rests on the binary search returning an upper bound over the gapped array: a probe
    /// that compares equal must move the boundary past that element, not before it. These layouts
    /// put equal keys in the arrangements most likely to expose a wrong bound - all equal, equal
    /// runs in ascending and descending order, and keys alternating around the midpoint - at sizes
    /// above the InsertionSort fallback so the gap machinery actually runs.
    /// </summary>
    [Test]
    [Arguments(40)]
    [Arguments(64)]
    [Arguments(300)]
    [Arguments(4000)]
    public async Task EqualKeyLayoutsRemainStableTest(int n)
    {
        var layouts = new (string Name, Func<int, int> Key)[]
        {
            ("allEqual", _ => 0),
            ("ascendingRuns", i => i / 7),
            ("descendingRuns", i => (n - 1 - i) / 7),
            ("alternating", i => i % 2),
            ("sortedHalves", i => i < n / 2 ? 0 : 1),
            ("reversedHalves", i => i < n / 2 ? 1 : 0),
            ("sawtoothDuplicates", i => i % 13 / 3),
        };

        foreach (var (name, key) in layouts)
        {
            var items = Enumerable.Range(0, n).Select(i => new StabilityTestItem(key(i), i)).ToArray();
            var expected = items.OrderBy(x => x.Value).ToArray(); // OrderBy is a stable sort

            LibrarySort.Sort(items.AsSpan(), new StatisticsContext());

            for (var i = 0; i < n; i++)
            {
                await Assert.That(items[i].Value).IsEqualTo(expected[i].Value).Because($"layout={name}, n={n}, i={i}");
                await Assert.That(items[i].OriginalIndex).IsEqualTo(expected[i].OriginalIndex).Because($"layout={name}, n={n}, i={i}");
            }
        }
    }

    /// <summary>
    /// The paper's guarantee is one binary search per inserted element. The search bisects slot
    /// indices over the gapped array rather than element ranks, so a probe landing in a gap must
    /// resolve without costing extra comparisons; if it degenerated to a scan this count would blow
    /// past the bound. Measured ratio is about 0.95 x n log2 n.
    /// </summary>
    [Test]
    [Arguments(1000)]
    [Arguments(10000)]
    public async Task ComparisonCountStaysWithinNLogNTest(int n)
    {
        var stats = new StatisticsContext();
        var array = TestHelpers.ShuffledRange(n, 42);

        LibrarySort.Sort(array.AsSpan(), stats);

        var bound = (ulong)(2.0 * n * Math.Log2(n));
        await Assert.That(stats.CompareCount).IsLessThan(bound);
    }

    /// <summary>
    /// The binary search over the gapped array performs the bulk of the comparisons.
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
