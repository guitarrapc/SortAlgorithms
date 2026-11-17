using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class FluxSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => FluxSort.Sort(span, context);

    // Up to 132 elements go to quadsort, whose tiny sort writes the n % 8 remainder of sorted input. Above that the analysis
    // recognises sorted input without writing.
    protected override CountExpectation SortedInputWrites => CountExpectation.Any;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    /// <summary>Records of the given keys, each carrying its position, sorted stably by key as the expected result.</summary>
    static (StabilityTestItem[] Input, StabilityTestItem[] Expected) Keyed(int[] keys)
    {
        var input = keys.Select((k, i) => new StabilityTestItem(k, i)).ToArray();
        return (input, input.OrderBy(x => x.Value).ToArray());
    }

    static int[] Pattern(string pattern, int n, int seed)
    {
        var random = new Random(seed);
        return pattern switch
        {
            "random" => Enumerable.Range(0, n).Select(_ => random.Next()).ToArray(),
            "sorted" => Enumerable.Range(0, n).ToArray(),
            "reversed" => Enumerable.Range(0, n).Reverse().ToArray(),
            "reversedDuplicates" => Enumerable.Range(0, n).Select(i => (n - i) / 3).ToArray(),
            "fewDistinct" => Enumerable.Range(0, n).Select(_ => random.Next(4)).ToArray(),
            "someDistinct" => Enumerable.Range(0, n).Select(_ => random.Next(100)).ToArray(),
            "saw" => Enumerable.Range(0, n).Select(i => i % 37).ToArray(),
            "pipeOrgan" => Enumerable.Range(0, n).Select(i => i < n / 2 ? i : n - i).ToArray(),
            "randomTail" => Enumerable.Range(0, n).Select(i => i < n * 3 / 4 ? i : random.Next(n)).ToArray(),
            "randomHalf" => Enumerable.Range(0, n).Select(i => i < n / 2 ? i : random.Next(n)).ToArray(),
            _ => throw new ArgumentException(pattern),
        };
    }

    static readonly string[] Patterns = ["random", "sorted", "reversed", "reversedDuplicates", "fewDistinct", "someDistinct", "saw", "pipeOrgan", "randomTail", "randomHalf"];

    /// <summary>Records every phase with its parameters, and the pivot roles, while delegating counts to a statistics context.</summary>
    sealed class PhaseLog
    {
        public readonly List<(SortPhase Phase, int P1, int P2, int P3)> Phases = [];
        public readonly List<(int Index, int BufferId, RoleType Role)> Roles = [];
        public readonly StatisticsContext Stats = new();
        public CompositeContext Context => new(Stats, new VisualizationContext(
            onPhase: (phase, p1, p2, p3) => Phases.Add((phase, p1, p2, p3)),
            onRole: (i, b, role) => Roles.Add((i, b, role))));

        public IEnumerable<(int Start, int End)> RangesOf(SortPhase phase) => Phases.Where(p => p.Phase == phase).Select(p => (p.P1, p.P2));
    }

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        FluxSort.Sort(array.AsSpan(), 2, 6, stats);

        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6], CollectionOrdering.Matching);
    }

    [Test]
    public async Task CustomComparerDescendingTest()
    {
        var array = TestHelpers.ShuffledRange(5000, 42);
        var descending = Comparer<int>.Create(static (a, b) => b.CompareTo(a));

        FluxSort.Sort(array.AsSpan(), descending, new StatisticsContext());

        await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, 5000).Reverse().ToArray(), CollectionOrdering.Matching);
    }

    /// <summary>
    /// Elements that hold references are partitioned through pooled swap memory and handed to quadsort's inline stack array.
    /// </summary>
    [Test]
    [Arguments(100)]
    [Arguments(5000)]
    public async Task ReferenceContainingElementsSortStably(int n)
    {
        var random = new Random(n);
        var items = Enumerable.Range(0, n).Select(i => new StabilityTestItemWithId(random.Next(64), $"id{i}")).ToArray();
        var expected = items.OrderBy(x => x.Key).ToArray();

        FluxSort.Sort(items.AsSpan());

        await Assert.That(items).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    /// <summary>
    /// Every length up to 300 and the lengths around each threshold: quadsort up to 132, the analysis from 133, the pseudomedian
    /// of nine up to 2,048 and the sampled median above it, under both contexts.
    /// </summary>
    [Test]
    public async Task EveryShapeAndLengthSortsStably()
    {
        var lengths = Enumerable.Range(0, 301).Concat([511, 512, 513, 2047, 2048, 2049, 2050, 5000, 20_000]);

        foreach (var n in lengths)
        {
            foreach (var pattern in Patterns)
            {
                var (items, expected) = Keyed(Pattern(pattern, n, n));
                var optimized = items.ToArray();

                FluxSort.Sort(items.AsSpan(), new StatisticsContext());
                FluxSort.Sort(optimized.AsSpan());

                await Assert.That(items).IsEquivalentTo(expected, CollectionOrdering.Matching).Because($"{pattern} n={n}");
                await Assert.That(optimized).IsEquivalentTo(expected, CollectionOrdering.Matching).Because($"{pattern} n={n} (NullContext)");
            }
        }
    }

    /// <summary>
    /// The analysis classifies each quarter as streaky (more than n / 256 blocks of 32 pairs that all ascend or all descend) or
    /// not. Every one of the 16 combinations is built here from ascending saw-tooth quarters (streaky, with descending pairs) and
    /// random quarters. A streaky quarter must be handed to quadsort as exactly its own range; the random quarters must be
    /// partitioned, adjacent ones together as one range.
    /// </summary>
    [Test]
    [MethodDataSource(nameof(QuarterMasks))]
    public async Task AnalysisDispatchesEachQuarterCombination(int mask)
    {
        const int n = 8192;
        const int quarter = n / 4;
        var random = new Random(mask);
        var keys = new int[n];
        for (var i = 0; i < n; i++)
        {
            var q = i / quarter;
            keys[i] = (mask >> q & 1) != 0 ? i % 256 : random.Next(512);
        }
        var (items, expected) = Keyed(keys);
        var log = new PhaseLog();

        FluxSort.Sort(items.AsSpan(), log.Context);

        await Assert.That(items).IsEquivalentTo(expected, CollectionOrdering.Matching);

        var quadsorted = log.RangesOf(SortPhase.FluxSortQuadsort).ToHashSet();
        var partitioned = log.RangesOf(SortPhase.FluxSortPartition).ToHashSet();

        // Contiguous groups of random quarters.
        var groups = new List<(int Start, int End)>();
        for (var q = 0; q < 4; q++)
        {
            if ((mask >> q & 1) != 0) continue;
            if (groups.Count > 0 && groups[^1].End == q * quarter - 1 && (mask >> (q - 1) & 1) == 0)
                groups[^1] = (groups[^1].Start, (q + 1) * quarter - 1);
            else
                groups.Add((q * quarter, (q + 1) * quarter - 1));
        }

        for (var q = 0; q < 4; q++)
        {
            var range = (q * quarter, (q + 1) * quarter - 1);
            if ((mask >> q & 1) != 0)
            {
                await Assert.That(quadsorted.Contains(range)).IsTrue().Because($"mask {mask}: streaky quarter {q} goes to quadsort");
            }
            else
            {
                await Assert.That(quadsorted.Contains(range)).IsFalse().Because($"mask {mask}: random quarter {q} is partitioned");
            }
        }
        foreach (var group in groups)
        {
            await Assert.That(partitioned.Contains(group)).IsTrue().Because($"mask {mask}: random quarters {group} are partitioned as one range");
        }
    }

    public static IEnumerable<int> QuarterMasks() => Enumerable.Range(0, 16);

    /// <summary>
    /// A streaky quarter without a single descending pair is already sorted, so it is neither quadsorted nor partitioned;
    /// only the final merge touches it.
    /// </summary>
    [Test]
    public async Task AscendingQuarterIsLeftAlone()
    {
        const int n = 8192;
        const int quarter = n / 4;
        var random = new Random(7);
        var keys = Enumerable.Range(0, n).Select(i => i < quarter ? i : random.Next(n)).ToArray();
        var (items, expected) = Keyed(keys);
        var log = new PhaseLog();

        FluxSort.Sort(items.AsSpan(), log.Context);

        await Assert.That(items).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Assert.That(log.Phases.Any(p => p.Phase is SortPhase.FluxSortQuadsort or SortPhase.FluxSortPartition && p.P1 < quarter)).IsFalse();
        await Assert.That(log.RangesOf(SortPhase.FluxSortPartition)).Contains((quarter, n - 1));
    }

    /// <summary>
    /// Quarters that descend strictly throughout are reversed during the analysis, joined with a neighbour when the boundary
    /// between them descends as well. Every combination of descending quarters is built both with descending boundaries (one
    /// global descending sequence) and with ascending ones (each quarter in a higher band than the one before); the other
    /// quarters hold repeated random keys, so stability is checked across the reversal.
    /// </summary>
    [Test]
    [MethodDataSource(nameof(QuarterMasks))]
    public async Task DescendingQuartersAreReversedStably(int mask)
    {
        const int n = 4096;
        const int quarter = n / 4;

        foreach (var joined in new[] { true, false })
        {
            var random = new Random(mask * 2 + (joined ? 1 : 0));
            var keys = new int[n];
            for (var i = 0; i < n; i++)
            {
                var q = i / quarter;
                keys[i] = (mask >> q & 1) != 0
                    ? (joined ? 4 * n - i : q * n + quarter - i % quarter)
                    : random.Next(64);
            }
            var (items, expected) = Keyed(keys);
            var stats = new StatisticsContext();

            FluxSort.Sort(items.AsSpan(), stats);

            await Assert.That(items).IsEquivalentTo(expected, CollectionOrdering.Matching).Because($"mask {mask}, joined {joined}");
            if (mask != 0)
            {
                await Assert.That(stats.SwapCount).IsGreaterThanOrEqualTo((ulong)quarter / 2).Because("each descending quarter is reversed by exchanges");
            }
        }
    }

    /// <summary>
    /// A pivot sample whose first half is constant and whose second half does not exceed it marks low-cardinality data, and the
    /// whole range goes to quadsort instead of being partitioned. With n = 8192 the sample is 32 elements taken every 256 from
    /// offset 128 (the middle of the first stride); those positions hold the maximum and the rest is random below it.
    /// </summary>
    [Test]
    public async Task ConstantPivotSampleHandsTheRangeToQuadsort()
    {
        const int n = 8192;
        var random = new Random(3);
        var keys = Enumerable.Range(0, n).Select(i => i % 256 == 128 ? 1000 : random.Next(1000)).ToArray();
        var (items, expected) = Keyed(keys);
        var log = new PhaseLog();

        FluxSort.Sort(items.AsSpan(), log.Context);

        await Assert.That(items).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Assert.That(log.Phases.Where(p => p.Phase.IsScopePhase()).Select(p => p.Phase).ToArray())
            .IsEquivalentTo([SortPhase.FluxSortAnalyze, SortPhase.FluxSortPivotSample, SortPhase.FluxSortQuadsort], CollectionOrdering.Matching);
        await Assert.That(log.RangesOf(SortPhase.FluxSortQuadsort)).Contains((0, n - 1));
    }

    /// <summary>
    /// The same data with an ordinary sample (positions not constant) is partitioned: the classification has a false class too.
    /// </summary>
    [Test]
    public async Task VariedPivotSampleIsPartitioned()
    {
        const int n = 8192;
        var random = new Random(3);
        var keys = Enumerable.Range(0, n).Select(_ => random.Next(1000)).ToArray();
        var (items, expected) = Keyed(keys);
        var log = new PhaseLog();

        FluxSort.Sort(items.AsSpan(), log.Context);

        await Assert.That(items).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Assert.That(log.RangesOf(SortPhase.FluxSortPartition)).Contains((0, n - 1));
        await Assert.That(log.RangesOf(SortPhase.FluxSortQuadsort)).DoesNotContain((0, n - 1));
    }

    /// <summary>
    /// A leading run of elements not greater than the pivot that covers more than a quarter of the range means the range is partly
    /// ordered: both sides of that partition go to quadsort instead of being partitioned further. The first 1,200 of 4,096 keys are
    /// small, so the whole run falls below any pivot drawn from the mostly larger rest.
    /// </summary>
    [Test]
    public async Task LongLeadingRunHandsBothSidesToQuadsort()
    {
        const int n = 4096;
        var random = new Random(5);
        var keys = Enumerable.Range(0, n).Select(i => i < 1200 ? random.Next(500) : random.Next(10_000)).ToArray();
        var (items, expected) = Keyed(keys);
        var log = new PhaseLog();

        FluxSort.Sort(items.AsSpan(), log.Context);

        await Assert.That(items).IsEquivalentTo(expected, CollectionOrdering.Matching);

        var scope = log.Phases.Where(p => p.Phase.IsScopePhase()).ToList();
        var partition = scope.FindIndex(p => p.Phase == SortPhase.FluxSortPartition && (p.P1, p.P2) == (0, n - 1));
        await Assert.That(partition).IsGreaterThanOrEqualTo(0);
        var next = scope.Skip(partition + 1).Take(2).ToList();
        await Assert.That(next.All(p => p.Phase == SortPhase.FluxSortQuadsort)).IsTrue();
        await Assert.That(next[0].P2).IsEqualTo(n - 1);
        await Assert.That(next[1].P1).IsEqualTo(0);
        await Assert.That(next[1].P2 + 1).IsEqualTo(next[0].P1);
    }

    /// <summary>
    /// With four distinct keys, a range of up to 2,048 elements takes the pseudomedian of nine, and the not-greater side soon picks
    /// the pivot it was split by; the reverse partition then takes the elements equal to it out in one pass.
    /// </summary>
    [Test]
    [Arguments(1000)]
    [Arguments(2000)]
    public async Task RepeatedPivotIsReversePartitioned(int n)
    {
        var random = new Random(n);
        var keys = Enumerable.Range(0, n).Select(_ => random.Next(4)).ToArray();
        var (items, expected) = Keyed(keys);
        var log = new PhaseLog();

        FluxSort.Sort(items.AsSpan(), log.Context);

        await Assert.That(items).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Assert.That(log.Phases.Any(p => p.Phase == SortPhase.FluxSortReversePartition)).IsTrue();
    }

    /// <summary>
    /// Few distinct keys cost a constant number of passes per key rather than n log n comparisons: below 2,048 elements through the
    /// reverse partition, above it because a range of one key has a constant pivot sample and is handed to quadsort, which finds it
    /// ordered in one pass.
    /// </summary>
    [Test]
    [Arguments(1000)]
    [Arguments(20_000)]
    [Arguments(100_000)]
    public async Task FewDistinctKeysStayLinear(int n)
    {
        var random = new Random(n);
        var keys = Enumerable.Range(0, n).Select(_ => random.Next(4)).ToArray();
        var (items, expected) = Keyed(keys);
        var stats = new StatisticsContext();

        FluxSort.Sort(items.AsSpan(), stats);

        await Assert.That(items).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Assert.That(stats.CompareCount).IsLessThan((ulong)n * 8);
    }

    /// <summary>
    /// The greater side of a partition lives in swap memory and is partitioned from there back into place, which the phase reports
    /// with source buffer 1. The pivot carries <see cref="RoleType.Pivot"/> in swap memory, inside the n elements it was given, and
    /// every role it sets is cleared.
    /// </summary>
    [Test]
    public async Task PartitionsFromSwapMemoryAndMarksThePivotThere()
    {
        const int n = 20_000;
        var array = TestHelpers.ShuffledRange(n, 42);
        var log = new PhaseLog();

        FluxSort.Sort(array.AsSpan(), log.Context);

        await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, n).ToArray(), CollectionOrdering.Matching);
        var partitions = log.Phases.Where(p => p.Phase == SortPhase.FluxSortPartition).ToList();
        await Assert.That(partitions.Any(p => p.P3 == 0)).IsTrue();
        await Assert.That(partitions.Any(p => p.P3 == 1)).IsTrue();

        var pivots = log.Roles.Where(r => r.Role == RoleType.Pivot).ToList();
        await Assert.That(pivots).IsNotEmpty();
        await Assert.That(pivots.All(r => r.BufferId == 1 && r.Index >= 0 && r.Index < n)).IsTrue();
        await Assert.That(log.Roles.Count(r => r.Role == RoleType.None)).IsEqualTo(pivots.Count);
    }

    /// <summary>
    /// Sorted input above 132 elements: the analysis compares every adjacent pair once (n - 4 pairs inside the quarters and the 3
    /// quarter boundaries), finds no descending pair, and stops without writing.
    /// </summary>
    [Test]
    [Arguments(133)]
    [Arguments(1000)]
    [Arguments(4096)]
    [Arguments(10_001)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var log = new PhaseLog();
        var sorted = Enumerable.Range(0, n).ToArray();

        FluxSort.Sort(sorted.AsSpan(), log.Context);

        await Assert.That(log.Stats.CompareCount).IsEqualTo((ulong)n - 1);
        await Assert.That(log.Stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(log.Stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(log.Phases.Select(p => p.Phase).ToArray()).IsEquivalentTo([SortPhase.FluxSortAnalyze]);
    }

    /// <summary>
    /// Strictly descending input above 132 elements: every quarter descends throughout and so does every boundary, so the analysis
    /// (n - 1 comparisons) reverses the whole input in one quad reversal of ⌈n / 2⌉ exchanges, the middle one with itself when n is odd.
    /// </summary>
    [Test]
    [Arguments(133)]
    [Arguments(1000)]
    [Arguments(4096)]
    [Arguments(10_001)]
    public async Task TheoreticalValuesReversedTest(int n)
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();

        FluxSort.Sort(reversed.AsSpan(), stats);

        await Assert.That(reversed).IsEquivalentTo(Enumerable.Range(0, n).ToArray(), CollectionOrdering.Matching);
        await Assert.That(stats.CompareCount).IsEqualTo((ulong)n - 1);
        await Assert.That(stats.SwapCount).IsEqualTo((ulong)(n + 1) / 2);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(2 * stats.SwapCount);
    }

    /// <summary>
    /// Random input: the sampled pivots keep the partitions balanced, so comparisons stay within a small constant of n log₂ n.
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

        FluxSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, n).ToArray(), CollectionOrdering.Matching);
        await Assert.That(stats.CompareCount).IsLessThan((ulong)(1.25 * n * Math.Log2(n)));
    }

    /// <summary>
    /// Sorting a subrange: every FluxSort phase names positions inside it. (Quadsort's detail phases inside the pivot sample sort
    /// are in the coordinates of the buffer holding the sample, so only FluxSort's own phases are checked here.)
    /// </summary>
    [Test]
    public async Task PhasesUseRangeCoordinates()
    {
        const int first = 13;
        const int n = 5000;
        var array = new int[first + n + 7];
        TestHelpers.ShuffledRange(n, 42).CopyTo(array, first);
        var log = new PhaseLog();

        FluxSort.Sort(array.AsSpan(), first, first + n, log.Context);

        await Assert.That(array.Skip(first).Take(n).ToArray()).IsEquivalentTo(Enumerable.Range(0, n).ToArray(), CollectionOrdering.Matching);
        var own = log.Phases.Where(p => p.Phase.IsScopePhase()).ToList();
        await Assert.That(own).IsNotEmpty();
        await Assert.That(own.All(p => p.P1 >= first && p.P2 <= first + n - 1 && p.P1 <= p.P2)).IsTrue()
            .Because($"[{string.Join(", ", own.Select(p => $"{p.Phase}({p.P1},{p.P2})").Distinct().Take(20))}]");
    }
}
