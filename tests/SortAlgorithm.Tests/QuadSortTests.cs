using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class QuadSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => QuadSort.Sort(span, context);

    // Sorted input is recognised by the quad swap without moving anything, but the remainder of n % 8 elements is sorted by the
    // branchless tiny sort, which writes both positions of every pair it compares.
    protected override CountExpectation SortedInputWrites => CountExpectation.Any;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    /// <summary>Records of the given keys, each carrying its position, sorted stably by key as the expected result.</summary>
    static (StabilityTestItem[] Input, StabilityTestItem[] Expected) Keyed(int[] keys)
    {
        var input = keys.Select((k, i) => new StabilityTestItem(k, i)).ToArray();
        return (input, input.OrderBy(x => x.Value).ToArray());
    }

    /// <summary>
    /// Input shapes that drive the quad swap into each of its states (ordered, strictly descending, neither) and the merges
    /// into their block-copy and interleaving paths.
    /// </summary>
    static int[] Pattern(string pattern, int n, int seed)
    {
        var random = new Random(seed);
        return pattern switch
        {
            "random" => Enumerable.Range(0, n).Select(_ => random.Next()).ToArray(),
            "sorted" => Enumerable.Range(0, n).ToArray(),
            "reversed" => Enumerable.Range(0, n).Reverse().ToArray(),
            // Descending with runs of equal keys: a stretch is reversed only while it descends strictly.
            "reversedDuplicates" => Enumerable.Range(0, n).Select(i => (n - i) / 3).ToArray(),
            "fewDistinct" => Enumerable.Range(0, n).Select(_ => random.Next(4)).ToArray(),
            "saw" => Enumerable.Range(0, n).Select(i => i % 37).ToArray(),
            "descendingSaw" => Enumerable.Range(0, n).Select(i => 37 - i % 37).ToArray(),
            "pipeOrgan" => Enumerable.Range(0, n).Select(i => i < n / 2 ? i : n - i).ToArray(),
            "randomTail" => Enumerable.Range(0, n).Select(i => i < n * 3 / 4 ? i : random.Next(n)).ToArray(),
            _ => throw new ArgumentException(pattern),
        };
    }

    static readonly string[] Patterns = ["random", "sorted", "reversed", "reversedDuplicates", "fewDistinct", "saw", "descendingSaw", "pipeOrgan", "randomTail"];

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        QuadSort.Sort(array.AsSpan(), 2, 6, stats);

        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6], CollectionOrdering.Matching);
    }

    [Test]
    public async Task CustomComparerDescendingTest()
    {
        var array = TestHelpers.ShuffledRange(1000, 42);
        var descending = Comparer<int>.Create(static (a, b) => b.CompareTo(a));

        QuadSort.Sort(array.AsSpan(), descending, new StatisticsContext());

        await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, 1000).Reverse().ToArray(), CollectionOrdering.Matching);
    }

    /// <summary>
    /// Elements that hold references pass through the 32-element stack array (an inline array of <c>T</c>) in the quad swap and the
    /// small-input tail swap, and through pooled swap memory in the merges; all of them must keep the references intact and in order.
    /// </summary>
    [Test]
    [Arguments(31)]
    [Arguments(1000)]
    [Arguments(5000)]
    public async Task ReferenceContainingElementsSortStably(int n)
    {
        var random = new Random(n);
        var items = Enumerable.Range(0, n).Select(i => new StabilityTestItemWithId(random.Next(16), $"id{i}")).ToArray();
        var expected = items.OrderBy(x => x.Key).ToArray();

        QuadSort.Sort(items.AsSpan());

        await Assert.That(items).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    /// <summary>
    /// Every sequence over three keys up to nine elements: the tiny sort's fixed networks (2 to 7 elements) and the first
    /// tail swap split must order every input and keep equal keys in input order.
    /// </summary>
    [Test]
    public async Task EveryThreeKeySequenceUpToNineSortsStably()
    {
        for (var n = 0; n <= 9; n++)
        {
            var count = (int)Math.Pow(3, n);
            for (var code = 0; code < count; code++)
            {
                var keys = new int[n];
                for (int i = 0, c = code; i < n; i++, c /= 3) keys[i] = c % 3;
                var (items, expected) = Keyed(keys);

                QuadSort.Sort(items.AsSpan(), new StatisticsContext());

                await Assert.That(items).IsEquivalentTo(expected, CollectionOrdering.Matching).Because($"keys [{string.Join(", ", keys)}]");
            }
        }
    }

    /// <summary>
    /// Every length up to 300 and the lengths around each threshold: the stack tail swap (n &lt; 32), the quad swap's remainder of
    /// n % 8, the blocks of 32 and their tail merge (n % 32 &gt; 8), and the quad merge levels (block · 4 ≤ n).
    /// </summary>
    [Test]
    public async Task EveryShapeAndLengthSortsStably()
    {
        var lengths = Enumerable.Range(0, 301).Concat([511, 512, 513, 1023, 1024, 1025, 4095, 4096, 4097, 10_000]);

        foreach (var n in lengths)
        {
            foreach (var pattern in Patterns)
            {
                var (items, expected) = Keyed(Pattern(pattern, n, n));
                var optimized = items.ToArray();

                QuadSort.Sort(items.AsSpan(), new StatisticsContext());
                QuadSort.Sort(optimized.AsSpan());

                await Assert.That(items).IsEquivalentTo(expected, CollectionOrdering.Matching).Because($"{pattern} n={n}");
                await Assert.That(optimized).IsEquivalentTo(expected, CollectionOrdering.Matching).Because($"{pattern} n={n} (NullContext)");
            }
        }
    }

    /// <summary>
    /// Sequences of eight-element groups in each quad swap state, so every transition between the ordered, reversed and unordered
    /// states occurs, followed by every remainder length. The group values are offset at random so the boundaries between groups are
    /// sometimes in order and sometimes not, which decides whether an ordered or reversed stretch continues.
    /// </summary>
    [Test]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(3)]
    public async Task QuadSwapStateTransitionsSortStably(int seed)
    {
        var random = new Random(seed);
        string[] states = ["ordered", "reversed", "neither"];

        // Every sequence of four group states: 3^4 combinations, decoded from one counter.
        for (var combination = 0; combination < 81; combination++)
        {
            var sequence = new string[4];
            for (int g = 0, c = combination; g < 4; g++, c /= 3) sequence[g] = states[c % 3];

            for (var remainder = 0; remainder < 8; remainder++)
            {
                var keys = new List<int>();
                foreach (var state in sequence)
                {
                    var start = random.Next(-8, 9) + keys.Count;
                    for (var k = 0; k < 8; k++)
                    {
                        keys.Add(state switch
                        {
                            "ordered" => start + k,
                            "reversed" => start + 8 - k,
                            _ => start + random.Next(8),
                        });
                    }
                }
                // The remainder continues descending half of the time, so a reversed stretch sometimes reaches the end.
                var last = keys[^1];
                var descending = random.Next(2) == 0;
                for (var k = 1; k <= remainder; k++) keys.Add(descending ? last - k : last + random.Next(-3, 4));

                var (items, expected) = Keyed([.. keys]);
                QuadSort.Sort(items.AsSpan(), new StatisticsContext());

                await Assert.That(items).IsEquivalentTo(expected, CollectionOrdering.Matching)
                    .Because($"{string.Join(",", sequence)} + {remainder} ({(descending ? "descending" : "mixed")})");
            }
        }
    }

    /// <summary>
    /// With swap memory shorter than the runs (the reference's <c>quadsort_swap</c> entry with a small <c>swap_size</c>), the last
    /// levels are rotate merges: trinity rotations (through swap memory, as a bridge rotation, or by in-place cycles), binary
    /// searches, and partial forward and backward merges. Keys repeat so the binary search meets equal keys.
    /// </summary>
    [Test]
    [Arguments(32)]
    [Arguments(64)]
    [Arguments(100)]
    [Arguments(512)]
    public async Task LimitedSwapMemoryRotateMergesSortStably(int swapSize)
    {
        foreach (var n in new[] { 2, 50, 96, 97, 300, 1000, 4096, 20_000 })
        {
            foreach (var pattern in new[] { "random", "fewDistinct", "saw", "reversedDuplicates", "randomTail" })
            {
                var (items, expected) = Keyed(Pattern(pattern, n, n + swapSize));
                var observed = items.ToArray();

                QuadSort.SortWithSwapSize(items.AsSpan(), swapSize, new ComparableComparer<StabilityTestItem>(), NullContext.Default);
                QuadSort.SortWithSwapSize(observed.AsSpan(), swapSize, new ComparableComparer<StabilityTestItem>(), new StatisticsContext());

                await Assert.That(items).IsEquivalentTo(expected, CollectionOrdering.Matching).Because($"{pattern} n={n} swap={swapSize}");
                await Assert.That(observed).IsEquivalentTo(expected, CollectionOrdering.Matching).Because($"{pattern} n={n} swap={swapSize} (observed)");
            }
        }
    }

    [Test]
    public async Task LimitedSwapMemoryAnnouncesRotateMerges()
    {
        var phases = new List<SortPhase>();
        var context = new VisualizationContext(onPhase: (phase, _, _, _) => phases.Add(phase));
        var array = TestHelpers.ShuffledRange(4096, 42);

        QuadSort.SortWithSwapSize(array.AsSpan(), 64, new ComparableComparer<int>(), context);

        await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, 4096).ToArray(), CollectionOrdering.Matching);
        await Assert.That(phases).Contains(SortPhase.QuadSortRotateMerge);
        // With 64 elements of swap memory no level of four 32-element blocks fits, so nothing is quad merged.
        await Assert.That(phases).DoesNotContain(SortPhase.QuadSortQuadMerge);
    }

    /// <summary>
    /// Swap memory is n up to 4,194,304 elements; above that it is 4,194,304 · 4^k for the smallest k with more than n / 8 elements.
    /// The cases sit on both sides of the cap and of each step of k.
    /// </summary>
    [Test]
    [Arguments(0, 0)]
    [Arguments(1000, 1000)]
    [Arguments(4_194_304, 4_194_304)]
    [Arguments(4_194_305, 4_194_304)]
    [Arguments(33_554_431, 4_194_304)]
    [Arguments(33_554_432, 16_777_216)]
    [Arguments(134_217_727, 16_777_216)]
    [Arguments(134_217_728, 67_108_864)]
    [Arguments(int.MaxValue, 268_435_456)]
    public async Task SwapMemoryIsCappedAboveFourMillionElements(int n, int expected)
    {
        await Assert.That(QuadSort.SwapSize(n)).IsEqualTo(expected);
    }

    /// <summary>
    /// Sorted input: the quad swap compares each pair within a group of eight and the three pairs between them (7 of 8), and each
    /// later stage checks one boundary per pair of runs before skipping the merge, which adds up to n - 1 or n comparisons.
    /// For n a multiple of 8 there is no remainder for the tiny sort, so nothing is written.
    /// </summary>
    [Test]
    [Arguments(32)]
    [Arguments(64)]
    [Arguments(96)]
    [Arguments(128)]
    [Arguments(1000)]
    [Arguments(1024)]
    [Arguments(4096)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();

        QuadSort.Sort(sorted.AsSpan(), stats);

        await Assert.That(stats.CompareCount).IsBetween((ulong)n - 1, (ulong)n);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

    /// <summary>
    /// Strictly descending input: the quad swap extends one descending stretch across the whole input, comparing every adjacent
    /// pair once (n - 1), and reverses it in place. The reversal exchanges ⌈n / 2⌉ pairs: on an odd length the reference's
    /// <c>quad_reversal</c> exchanges the middle element with itself. Nothing reaches swap memory.
    /// </summary>
    [Test]
    [Arguments(32)]
    [Arguments(33)]
    [Arguments(100)]
    [Arguments(133)]
    [Arguments(1000)]
    [Arguments(4096)]
    public async Task TheoreticalValuesReversedTest(int n)
    {
        var stats = new StatisticsContext();
        var buffers = new HashSet<int>();
        var tracker = new VisualizationContext(
            onIndexWrite: (_, b, _) => buffers.Add(b),
            onSwap: (_, _, b) => buffers.Add(b),
            onRangeCopy: (_, _, _, _, b, _) => buffers.Add(b));
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();

        QuadSort.Sort(reversed.AsSpan(), new CompositeContext(stats, tracker));

        await Assert.That(reversed).IsEquivalentTo(Enumerable.Range(0, n).ToArray(), CollectionOrdering.Matching);
        await Assert.That(stats.CompareCount).IsEqualTo((ulong)n - 1);
        await Assert.That(stats.SwapCount).IsEqualTo((ulong)(n + 1) / 2);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(2 * stats.SwapCount);
        await Assert.That(buffers).IsEquivalentTo([0]);
    }

    /// <summary>
    /// Random input: no comparison sort averages fewer than log₂ n! comparisons. A parity or cross merge compares once for every
    /// element it merges, with no early exit when one run runs out, so each of the about log₂ n merge levels (the three inside the quad
    /// swap's groups of eight included) costs about n comparisons, and the guards that look for ordered boundaries and eight-element
    /// blocks add a fraction of n; the total stays below n (log₂ n + 1). The merges move every element through swap memory and back,
    /// so writes outnumber comparisons.
    /// </summary>
    [Test]
    [Arguments(1000, 42)]
    [Arguments(1000, 1234)]
    [Arguments(10_000, 42)]
    [Arguments(10_000, 1234)]
    public async Task TheoreticalValuesRandomTest(int n, int seed)
    {
        var stats = new StatisticsContext();
        var array = TestHelpers.ShuffledRange(n, seed);

        QuadSort.Sort(array.AsSpan(), stats);

        var log2Factorial = Enumerable.Range(2, n - 1).Sum(i => Math.Log2(i));
        await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, n).ToArray(), CollectionOrdering.Matching);
        await Assert.That(stats.CompareCount).IsGreaterThan((ulong)log2Factorial);
        await Assert.That(stats.CompareCount).IsLessThan((ulong)(n * (Math.Log2(n) + 1)));
        await Assert.That(stats.IndexWriteCount).IsGreaterThan(stats.CompareCount);
    }

    /// <summary>
    /// The buffers a consumer sees: the input (0), swap memory (1), and the 32-element stack array (2) that the quad swap and the
    /// small-input tail swap merge through. A short input never allocates swap memory.
    /// </summary>
    [Test]
    [Arguments(31, new[] { 0, 2 })]
    [Arguments(1000, new[] { 0, 1, 2 })]
    public async Task ReportsOnlyItsThreeBuffers(int n, int[] expectedBuffers)
    {
        var buffers = new HashSet<int>();
        var context = new VisualizationContext(
            onCompare: (_, _, _, bi, bj) => { buffers.Add(bi); buffers.Add(bj); },
            onIndexRead: (_, b) => buffers.Add(b),
            onIndexWrite: (_, b, _) => buffers.Add(b),
            onSwap: (_, _, b) => buffers.Add(b),
            onRangeCopy: (_, _, _, sb, db, _) => { buffers.Add(sb); buffers.Add(db); });
        var array = TestHelpers.ShuffledRange(n, 42);

        QuadSort.Sort(array.AsSpan(), context);

        await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, n).ToArray(), CollectionOrdering.Matching);
        await Assert.That(buffers.Order().ToArray()).IsEquivalentTo(expectedBuffers, CollectionOrdering.Matching);
    }

    /// <summary>
    /// Quadsort's phases are detail phases, because FluxSort delegates ranges to it; at the top level they are announced alone.
    /// Their coordinates cover the range being sorted, including when that range starts inside the span.
    /// </summary>
    [Test]
    [Arguments(20)]
    [Arguments(1000)]
    public async Task PhasesAreDetailPhasesInRangeCoordinates(int n)
    {
        const int first = 7;
        var phases = new List<(SortPhase Phase, int Start, int End)>();
        var context = new VisualizationContext(onPhase: (phase, p1, p2, _) => phases.Add((phase, p1, p2)));
        var array = new int[first + n + 5];
        TestHelpers.ShuffledRange(n, 42).CopyTo(array, first);

        QuadSort.Sort(array.AsSpan(), first, first + n, context);

        await Assert.That(array.Skip(first).Take(n).ToArray()).IsEquivalentTo(Enumerable.Range(0, n).ToArray(), CollectionOrdering.Matching);
        await Assert.That(phases).IsNotEmpty();
        await Assert.That(phases.All(p => p.Phase.IsDetailPhase())).IsTrue();
        await Assert.That(phases.All(p => p.Start >= first && p.End <= first + n - 1 && p.Start <= p.End)).IsTrue()
            .Because($"[{string.Join(", ", phases.Select(p => $"{p.Phase}({p.Start},{p.End})").Distinct())}]");
    }
}
