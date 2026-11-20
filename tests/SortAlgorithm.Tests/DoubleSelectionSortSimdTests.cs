using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class DoubleSelectionSortSimdTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => DoubleSelectionSortSimd.Sort(span, context);

    // O(n^2) comparisons: skip large inputs in data-driven tests.
    protected override int MaxOrderTestSize => 512;

    // Sorted input keeps min at left and max at right each pass: no writes, no swaps.
    protected override CountExpectation SortedInputWrites => CountExpectation.Zero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    public async Task SameSwapsAsDoubleSelectionSortOnEveryVectorizedType()
    {
        List<string> problems =
        [
            .. Check(x => (int)x), .. Check(x => (uint)x), .. Check(x => x), .. Check(x => (ulong)x),
            .. Check(x => (short)x), .. Check(x => (ushort)x), .. Check(x => (byte)x), .. Check(x => (sbyte)x),
            .. Check(x => (nint)x), .. Check(x => (nuint)x),
        ];

        // The location scans take the first minimum and the last maximum, so every exchange must match double selection sort's.
        await Assert.That(problems).IsEmpty();

        static List<string> Check<T>(Func<long, T> convert) where T : IComparable<T>
            => SimdSelectionEquivalence.Mismatches(42, convert,
                static (span, context) => DoubleSelectionSort.Sort(span, context),
                static (span, context) => DoubleSelectionSortSimd.Sort(span, context),
                static span => DoubleSelectionSortSimd.Sort(span));
    }

    [Test]
    [Arguments(2)]
    [Arguments(31)]
    [Arguments(32)]
    [Arguments(33)]
    [Arguments(64)]
    [Arguments(101)]
    [Arguments(257)]
    public async Task SortedInputCountsFollowTheLaneCount(int n)
    {
        var w = SelectionVectorScan.LaneCount<int>();
        var stats = new StatisticsContext();
        DoubleSelectionSortSimd.Sort(Enumerable.Range(0, n).ToArray().AsSpan(), stats);

        // A pass over a region of k >= 2w elements reads all k once. Its first pair of vectors costs w pair comparisons and every
        // further pair 3w; the lane reductions cost 2(w - 1); the t elements after the last pair of vectors cost three comparisons
        // per pair and two for an element left over. On sorted input both location scans stop at their first vector (w reads and
        // w comparisons each). The region left narrower than two vectors is double selection sort's.
        ulong compares = 0, reads = 0;
        var k = n;
        for (; w > 0 && k >= 2 * w; k -= 2)
        {
            var pairs = k / (2 * w);
            var t = k - 2 * w * pairs;
            compares += (ulong)(w + (pairs - 1) * 3 * w + 2 * (w - 1) + 3 * (t / 2) + 2 * (t % 2) + 2 * w);
            reads += (ulong)(k + 2 * w);
        }
        var rest = new StatisticsContext();
        DoubleSelectionSort.Sort(Enumerable.Range(0, k).ToArray().AsSpan(), rest);

        await Assert.That(stats.CompareCount).IsEqualTo(compares + rest.CompareCount);
        await Assert.That(stats.IndexReadCount).IsEqualTo(reads + rest.IndexReadCount);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

    [Test]
    public async Task PairComparisonsReportTheOrderOfTheElementsTheyName()
    {
        var w = SelectionVectorScan.LaneCount<int>();
        Skip.When(w == 0, "No vector width is accelerated on this machine.");

        var random = new Random(5);
        var shadow = Enumerable.Range(0, 300).Select(_ => random.Next(40)).ToArray();
        var array = shadow.ToArray();
        var problems = new List<string>();
        var vectorPairs = 0;
        var context = new VisualizationContext(
            onCompare: (i, j, result, _, _) =>
            {
                if (i < 0 || j < 0) return;
                if (j - i == w) vectorPairs++;
                if (Math.Sign(shadow[i].CompareTo(shadow[j])) != Math.Sign(result))
                    problems.Add($"({i}, {j}) reported {result} for {shadow[i]} and {shadow[j]}");
            },
            onSwap: (i, j, _) => (shadow[i], shadow[j]) = (shadow[j], shadow[i]));
        DoubleSelectionSortSimd.Sort(array.AsSpan(), context);

        // Replaying the swaps on a copy gives the elements each comparison names when it is reported.
        await Assert.That(problems).IsEmpty();
        await Assert.That(vectorPairs).IsGreaterThan(0);
        await Assert.That(array).IsEquivalentTo(shadow, CollectionOrdering.Matching);
    }

    [Test]
    public async Task CustomComparerRunsDoubleSelectionSort()
    {
        var source = TestHelpers.ShuffledRange(200, 3);
        var expected = source.ToArray();
        var expectedSwaps = new List<(int, int)>();
        DoubleSelectionSort.Sort(expected.AsSpan(), new DescendingInt(), new VisualizationContext(onSwap: (i, j, _) => expectedSwaps.Add((i, j))));
        var actual = source.ToArray();
        var actualSwaps = new List<(int, int)>();
        DoubleSelectionSortSimd.Sort(actual.AsSpan(), new DescendingInt(), new VisualizationContext(onSwap: (i, j, _) => actualSwaps.Add((i, j))));

        await Assert.That(actual).IsEquivalentTo(Enumerable.Range(0, 200).Reverse().ToArray(), CollectionOrdering.Matching);
        await Assert.That(actualSwaps).IsEquivalentTo(expectedSwaps, CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortLeavesOutsideUntouched()
    {
        var source = TestHelpers.ShuffledRange(160, 11);
        var expected = source.ToArray();
        DoubleSelectionSort.Sort(expected.AsSpan(), 3, 153, NullContext.Default);
        var actual = source.ToArray();
        DoubleSelectionSortSimd.Sort(actual.AsSpan(), 3, 153, NullContext.Default);

        await Assert.That(actual).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    readonly struct DescendingInt : IComparer<int>
    {
        public int Compare(int x, int y) => y.CompareTo(x);
    }
}
