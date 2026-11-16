using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using SortAlgorithm.Utils;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class SampleSortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => SampleSort.Sort(span, context);

    // Every range without a sample swaps its random draw to the front, so sorted input is still moved.
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.NonZero;

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        SampleSort.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        SampleSort.Sort(array.AsSpan(), 0, array.Length, stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9], CollectionOrdering.Matching);
    }

    /// <summary>
    /// Lengths up to 7 cover a one-element sample (n ≤ 4), a three-element sample (n = 5..7), a sample that is
    /// most of its range (3 of 5), and subranges that consist of nothing but their sorted sample.
    /// </summary>
    [Test]
    public async Task EveryPermutationUpToSevenSorts()
    {
        for (var n = 0; n <= 7; n++)
        {
            foreach (var permutation in Permutations(n))
            {
                var array = permutation.ToArray();
                SampleSort.Sort(array.AsSpan(), new StatisticsContext());
                await Assert.That(IsSorted(array)).IsTrue().Because($"[{string.Join(", ", permutation)}]");
            }
        }
    }

    /// <summary>Duplicates make pivots equal to sample neighbours and to elements of the rest.</summary>
    [Test]
    public async Task EveryThreeValueSequenceUpToSevenSorts()
    {
        for (var n = 0; n <= 7; n++)
        {
            var count = (int)Math.Pow(3, n);
            for (var code = 0; code < count; code++)
            {
                var array = new int[n];
                for (int i = 0, c = code; i < n; i++, c /= 3) array[i] = c % 3;
                var expected = array.Order().ToArray();

                SampleSort.Sort(array.AsSpan(), new StatisticsContext());

                await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
            }
        }
    }

    [Test]
    public async Task CustomComparerDescendingTest()
    {
        var array = TestHelpers.ShuffledRange(1000, 42);
        var descending = Comparer<int>.Create(static (a, b) => b.CompareTo(a));

        SampleSort.Sort(array.AsSpan(), descending, new StatisticsContext());

        await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, 1000).Reverse().ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    public async Task DuplicateHeavyInputStaysNLogN()
    {
        const int n = 20_000;
        var random = new Random(42);
        var array = Enumerable.Range(0, n).Select(_ => random.Next(4)).ToArray();
        var expected = array.Order().ToArray();
        var stats = new StatisticsContext();

        SampleSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
        // The bidirectional scan stops on keys equal to the pivot, so four distinct values do not unbalance the partitions.
        await Assert.That(stats.CompareCount).IsLessThan((ulong)(2 * n * Math.Log2(n)));
    }

    /// <summary>
    /// The first draw is the whole range, so its announced sample count is the rule applied to n:
    /// 2^k − 1 with 2^k the power of two closest to n / ln n. The lengths sit on both sides of the first cutoffs.
    /// </summary>
    [Test]
    [Arguments(5)]
    [Arguments(16)]
    [Arguments(17)]
    [Arguments(45)]
    [Arguments(46)]
    [Arguments(113)]
    [Arguments(114)]
    [Arguments(268)]
    [Arguments(269)]
    [Arguments(616)]
    [Arguments(617)]
    [Arguments(1389)]
    [Arguments(1390)]
    [Arguments(3085)]
    [Arguments(3086)]
    [Arguments(100_000)]
    public async Task FirstSampleIsClosestPowerOfTwoToNOverLnN(int n)
    {
        (int Left, int Right, int Count)? first = null;
        var context = new VisualizationContext(onPhase: (phase, a, b, c) =>
        {
            if (phase == SortPhase.SampleSortSample && first is null) first = (a, b, c);
        });

        SampleSort.Sort(TestHelpers.ShuffledRange(n, 42).AsSpan(), context);

        var target = n / Math.Log(n);
        var k = Enumerable.Range(1, 30).MinBy(k => Math.Abs(Math.Pow(2, k) - target));
        await Assert.That(first).IsEqualTo((0, n - 1, (1 << k) - 1));
    }

    /// <summary>
    /// The draw phase is announced at its first moving swap, so an announced draw covers at least one swap and at most
    /// one swap per sample element.
    /// </summary>
    [Test]
    [Arguments(5)]
    [Arguments(500)]
    public async Task SampleDrawPhaseCoversItsSwaps(int n)
    {
        var events = RecordPhasesAndSwaps(TestHelpers.ShuffledRange(n, 1234), out var array);
        await Assert.That(IsSorted(array)).IsTrue();

        var draws = 0;
        for (var i = 0; i < events.Count; i++)
        {
            if (events[i].IsSwap || events[i].Phase != SortPhase.SampleSortSample) continue;
            draws++;
            var swaps = 0;
            for (var j = i + 1; j < events.Count && events[j].IsSwap; j++) swaps++;
            await Assert.That(swaps).IsBetween(1, events[i].SampleCount);
        }
        await Assert.That(draws).IsGreaterThan(0);
    }

    /// <summary>
    /// A draw that picks the elements already at the front moves nothing and announces nothing. With the default seed the
    /// one-element draw of a two-element range picks index 0, so the run goes straight to the partition phase.
    /// </summary>
    [Test]
    public async Task DrawThatMovesNothingAnnouncesNothing()
    {
        var events = RecordPhasesAndSwaps([1, 0], out var array);

        await Assert.That(array).IsEquivalentTo([0, 1], CollectionOrdering.Matching);
        await Assert.That(events.Any(e => e.Phase == SortPhase.SampleSortSample)).IsFalse();
        await Assert.That(events.First(e => !e.IsSwap).Phase).IsEqualTo(SortPhase.QuickSortPartition);
    }

    static List<(SortPhase Phase, int SampleCount, bool IsSwap)> RecordPhasesAndSwaps(int[] input, out int[] array)
    {
        var events = new List<(SortPhase Phase, int SampleCount, bool IsSwap)>();
        var context = new VisualizationContext(
            onSwap: (_, _, _) => events.Add((SortPhase.None, 0, true)),
            onPhase: (phase, _, _, count) => events.Add((phase, count, false)));
        array = input;
        SampleSort.Sort(array.AsSpan(), context);
        return events;
    }

    [Test]
    public async Task SameSeedReproducesTheRun()
    {
        var input = TestHelpers.ShuffledRange(5000, 42);
        var first = input.ToArray();
        var second = input.ToArray();
        var statsFirst = new StatisticsContext();
        var statsSecond = new StatisticsContext();

        SampleSort.Sort(first.AsSpan(), Comparer<int>.Default, statsFirst, 12345);
        SampleSort.Sort(second.AsSpan(), Comparer<int>.Default, statsSecond, 12345);

        await Assert.That(IsSorted(first)).IsTrue();
        await Assert.That(statsSecond.CompareCount).IsEqualTo(statsFirst.CompareCount);
        await Assert.That(statsSecond.SwapCount).IsEqualTo(statsFirst.SwapCount);
        await Assert.That(statsSecond.IndexReadCount).IsEqualTo(statsFirst.IndexReadCount);
        await Assert.That(statsSecond.IndexWriteCount).IsEqualTo(statsFirst.IndexWriteCount);
    }

    [Test]
    public async Task SeedZeroSelectsTheDefaultSeed()
    {
        var input = TestHelpers.ShuffledRange(5000, 42);
        var withZero = input.ToArray();
        var withDefault = input.ToArray();
        var statsZero = new StatisticsContext();
        var statsDefault = new StatisticsContext();

        SampleSort.Sort(withZero.AsSpan(), Comparer<int>.Default, statsZero, 0);
        SampleSort.Sort(withDefault.AsSpan(), statsDefault);

        await Assert.That(statsZero.CompareCount).IsEqualTo(statsDefault.CompareCount);
        await Assert.That(statsZero.SwapCount).IsEqualTo(statsDefault.SwapCount);
    }

    [Test]
    public async Task EverySeedSortsAndTheSeedReachesTheDraw()
    {
        var input = TestHelpers.ShuffledRange(5000, 42);
        var compareCounts = new HashSet<ulong>();
        foreach (var seed in new uint[] { 1, 2, 3, 0xDEADBEEF, uint.MaxValue })
        {
            var array = input.ToArray();
            var stats = new StatisticsContext();
            SampleSort.Sort(array.AsSpan(), Comparer<int>.Default, stats, seed);
            await Assert.That(IsSorted(array)).IsTrue().Because($"seed {seed}");
            compareCounts.Add(stats.CompareCount);
        }
        await Assert.That(compareCounts.Count).IsGreaterThan(1);
    }

    /// <summary>
    /// The defining property (Frazer &amp; McKellar): pivots taken from one sorted sample keep the partitions balanced, so the
    /// comparison count on random input stays close to log₂ n! — below what single-pivot quicksort with median-of-3 needs.
    /// </summary>
    [Test]
    public async Task ComparisonsApproachTheInformationTheoreticBound()
    {
        const int n = 4096;
        var bound = Log2Factorial(n);
        double sample = 0, median3 = 0;
        int[] seeds = [42, 1234, 7, 99, 2026];
        foreach (var seed in seeds)
        {
            var a = TestHelpers.ShuffledRange(n, seed);
            var b = a.ToArray();
            var statsSample = new StatisticsContext();
            var statsMedian3 = new StatisticsContext();
            SampleSort.Sort(a.AsSpan(), statsSample);
            QuickSortMedian3.Sort(b.AsSpan(), statsMedian3);
            sample += statsSample.CompareCount / bound;
            median3 += statsMedian3.CompareCount / bound;
        }
        sample /= seeds.Length;
        median3 /= seeds.Length;

        await Assert.That(sample).IsBetween(1.0, 1.10);
        await Assert.That(sample).IsLessThan(median3);
    }

    /// <summary>
    /// McIlroy's adversary can be built against a fixed seed because the draws never look at the values. Every round
    /// still fixes its whole sample of about n / ln n elements, so the input it builds is costly but not quadratic.
    /// Sorting the same input with another seed is no worse than random input.
    /// </summary>
    [Test]
    public async Task KillerAdversaryAgainstTheDefaultSeedStaysSubquadratic()
    {
        const int n = 4000;
        var killer = KillerAdversaryGenerator.Generate(n, static (span, comparer) => SampleSort.Sort(span, comparer, NullContext.Default));
        var log2 = Math.Log2(n);
        var bound = Log2Factorial(n);

        var againstDefault = killer.ToArray();
        var statsDefault = new StatisticsContext();
        SampleSort.Sort(againstDefault.AsSpan(), statsDefault);

        var againstOther = killer.ToArray();
        var statsOther = new StatisticsContext();
        SampleSort.Sort(againstOther.AsSpan(), Comparer<int>.Default, statsOther, 1);

        await Assert.That(IsSorted(againstDefault)).IsTrue();
        await Assert.That(IsSorted(againstOther)).IsTrue();
        // The adversary does hurt the seed it was built against...
        await Assert.That(statsDefault.CompareCount / bound).IsGreaterThan(3.0);
        // ...but along n log³ n, far below the n²/2 a quadratic worst case would reach.
        await Assert.That((double)statsDefault.CompareCount).IsLessThan(0.1 * n * log2 * log2 * log2);
        await Assert.That(statsOther.CompareCount / bound).IsLessThan(1.2);
    }

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        SampleSort.Sort(sorted.AsSpan(), stats);

        // Not adaptive: sorted input is partitioned like any other permutation, so the comparison count is n log₂ n order.
        await AssertCounts(stats, n, minCompares: (ulong)(n - 1));
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
        SampleSort.Sort(reversed.AsSpan(), stats);

        await AssertCounts(stats, n, minCompares: (ulong)(n - 1));
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
        SampleSort.Sort(random.AsSpan(), stats);

        await AssertCounts(stats, n, minCompares: (ulong)(n - 1));
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
        SampleSort.Sort(sameValues.AsSpan(), stats);

        foreach (var item in sameValues) await Assert.That(item).IsEqualTo(42);
        // Equal keys stop both scans, so each partition splits the range near its middle instead of degenerating.
        await AssertCounts(stats, n, minCompares: 1);
    }

    /// <summary>
    /// Every write comes from a swap (draw, partition exchange, block shift), and every comparison reads one element
    /// against the pivot or sample element held in a local.
    /// </summary>
    static async Task AssertCounts(StatisticsContext stats, int n, ulong minCompares)
    {
        var nLogN = n * Math.Log2(Math.Max(n, 2));
        await Assert.That(stats.CompareCount).IsBetween(minCompares, (ulong)(2 * nLogN));
        await Assert.That(stats.SwapCount).IsLessThanOrEqualTo((ulong)(2 * nLogN));
        await Assert.That(stats.IndexWriteCount).IsEqualTo(2 * stats.SwapCount);
        await Assert.That(stats.IndexReadCount).IsGreaterThanOrEqualTo(stats.CompareCount + 2 * stats.SwapCount);
    }

    static double Log2Factorial(int n)
    {
        var sum = 0.0;
        for (var i = 2; i <= n; i++) sum += Math.Log2(i);
        return sum;
    }

    static bool IsSorted(int[] array)
    {
        for (var i = 1; i < array.Length; i++)
            if (array[i - 1] > array[i]) return false;
        return true;
    }

    static IEnumerable<int[]> Permutations(int n)
    {
        var a = Enumerable.Range(0, n).ToArray();
        if (n == 0)
        {
            yield return a;
            yield break;
        }

        // Heap's algorithm, iterative.
        var c = new int[n];
        yield return a.ToArray();
        var i = 1;
        while (i < n)
        {
            if (c[i] < i)
            {
                if ((i & 1) == 0) (a[0], a[i]) = (a[i], a[0]);
                else (a[c[i]], a[i]) = (a[i], a[c[i]]);
                yield return a.ToArray();
                c[i]++;
                i = 1;
            }
            else
            {
                c[i] = 0;
                i++;
            }
        }
    }
}
