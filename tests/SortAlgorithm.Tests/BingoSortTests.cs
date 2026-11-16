using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class BingoSortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => BingoSort.Sort(span, context);

    // Distinct keys cost one pass per element, Θ(n²) like selection sort: skip large inputs in data-driven tests.
    protected override int MaxOrderTestSize => 512;

    // Sorted input: every pass finds its maximum already at the end and retires it without a swap.
    protected override CountExpectation SortedInputWrites => CountExpectation.Zero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    /// <summary>
    /// Records the phase stream and checks the two observation properties the algorithm promises:
    /// at every collecting pass the suffix past the announced region already holds its final values,
    /// and the CurrentMax role always marks the element that holds the candidate value.
    /// </summary>
    /// <remarks>
    /// Roles are index markers that do not follow element movement, so the second property fails
    /// silently if a swap carries the candidate away without the role moving with it. Requiring that
    /// no swap touches a marked index, and that the marked element is unchanged when the mark is
    /// cleared, catches that without depending on the order the algorithm happens to scan in.
    /// </remarks>
    private sealed class BingoObserver(int[] array, int[] expected) : ISortContext
    {
        private int _marked = -1;
        private int _markedValue;

        public List<(SortPhase Phase, int First, int End)> Phases { get; } = [];
        public List<string> Problems { get; } = [];
        public bool RoleLeftBehind => _marked >= 0;

        public int PassCount => Phases.Count(p => p.Phase is SortPhase.BingoFindMax or SortPhase.BingoPass);

        public void OnPhase(SortPhase phase, int param1 = 0, int param2 = 0, int param3 = 0)
        {
            Phases.Add((phase, param1, param2));
            if (phase != SortPhase.BingoPass) return;

            for (var k = param2 + 1; k < array.Length; k++)
            {
                if (array[k] != expected[k])
                {
                    Problems.Add($"BingoPass(end={param2}): position {k} holds {array[k]}, expected its final value {expected[k]}");
                    return;
                }
            }
        }

        public void OnRole(int index, int bufferId, RoleType role)
        {
            if (role == RoleType.CurrentMax)
            {
                if (_marked >= 0) Problems.Add($"CurrentMax set at {index} while {_marked} is still marked");
                _marked = index;
                _markedValue = array[index];
            }
            else if (role == RoleType.None)
            {
                if (index != _marked) Problems.Add($"role cleared at {index}, but the mark is at {_marked}");
                else if (array[index] != _markedValue) Problems.Add($"marked element at {index} changed from {_markedValue} to {array[index]}");
                _marked = -1;
            }
        }

        public void OnSwap(int i, int j, int bufferId)
        {
            if (_marked >= 0 && (i == _marked || j == _marked))
                Problems.Add($"swap({i}, {j}) moves the marked candidate at {_marked} without moving the role");
        }

        public void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ) { }
        public void OnIndexRead(int index, int bufferId) { }
        public void OnIndexWrite(int index, int bufferId) { }
        public void OnIndexWrite<T>(int index, int bufferId, T value) { }
        public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId) { }
        public void OnRangeCopy<T>(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, ReadOnlySpan<T> values) { }
        public void OnLink(int parentIndex, int childIndex, int bufferId, LinkSide side) { }
    }

    private static int[] FewUnique(int n, int m, int seed)
    {
        var random = new Random(seed);
        var array = new int[n];
        for (var i = 0; i < n; i++) array[i] = random.Next(m);
        return array;
    }

    private static BingoObserver Observe(int[] array)
    {
        var expected = array.ToArray();
        Array.Sort(expected);
        var observer = new BingoObserver(array, expected);
        BingoSort.Sort(array.AsSpan(), observer);
        return observer;
    }

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        BingoSort.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        BingoSort.Sort(array.AsSpan(), 0, array.Length, stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortSingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9 };

        // A single-element range is already sorted.
        BingoSort.Sort(array.AsSpan(), 2, 3, stats);

        await Assert.That(array).IsEquivalentTo([5, 3, 8, 1, 9], CollectionOrdering.Matching);
        await Assert.That(stats.CompareCount).IsEqualTo(0UL);
    }

    [Test]
    public async Task RangeSortBeginningTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 9, 7, 5, 3, 1, 2, 4, 6, 8 };

        BingoSort.Sort(array.AsSpan(), 0, 5, stats);

        await Assert.That(array).IsEquivalentTo([1, 3, 5, 7, 9, 2, 4, 6, 8], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortEndTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 3, 5, 7, 9, 8, 6, 4, 2 };

        BingoSort.Sort(array.AsSpan(), 5, 9, stats);

        await Assert.That(array).IsEquivalentTo([1, 3, 5, 7, 9, 2, 4, 6, 8], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortDuplicatesDoNotLeakAcrossBoundaryTest()
    {
        var stats = new StatisticsContext();
        // The range [2, 7) holds copies of 9 and 1 that also appear outside it.
        var array = new[] { 9, 1, 1, 9, 5, 9, 1, 9, 1 };

        BingoSort.Sort(array.AsSpan(), 2, 7, stats);

        await Assert.That(array).IsEquivalentTo([9, 1, 1, 1, 5, 9, 9, 9, 1], CollectionOrdering.Matching);
    }

    [Test]
    public async Task CustomComparerTest()
    {
        var array = new[] { 3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5 };
        var descending = Comparer<int>.Create(static (a, b) => b.CompareTo(a));

        BingoSort.Sort(array.AsSpan(), descending, NullContext.Default);

        // The comparer defines both the order and which elements count as copies of the same value.
        await Assert.That(array).IsEquivalentTo([9, 6, 5, 5, 5, 4, 3, 3, 2, 1, 1], CollectionOrdering.Matching);
    }

    [Test]
    public async Task EqualKeysAreReorderedTest()
    {
        // [P, x1, x2] with x1 == x2: the pass that collects P swaps it with x2, which carries x2 to the
        // front, past x1. This is the reason IsStable is false.
        var items = new StabilityTestItem[] { new(2, 0), new(1, 1), new(1, 2) };

        BingoSort.Sort(items.AsSpan());

        await Assert.That(items.Select(x => x.Value)).IsEquivalentTo([1, 1, 2], CollectionOrdering.Matching);
        await Assert.That(items.Select(x => x.OriginalIndex)).IsEquivalentTo([2, 1, 0], CollectionOrdering.Matching);
        await Assert.That(BingoSort.IsStable).IsFalse();
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
        BingoSort.Sort(sorted.AsSpan(), stats);

        // With distinct keys each pass retires one value. Sorted input finds that value already at the
        // end, so no pass ever swaps, and the region shrinks by exactly one per pass: n-1 passes.
        // A pass over [0..E] scans E elements (one comparison each; nothing beats the candidate A[E]),
        // then the trailing check retires A[E] (1) and stops at A[E-1] (1), the second check only
        // happening while E-1 > 0. Summed over E = n-1 .. 1:
        //   n(n-1)/2 + 2(n-2) + 1 = n(n-1)/2 + 2n - 3
        var expectedCompares = (ulong)(n * (n - 1) / 2 + 2 * n - 3);
        var expectedPasses = (ulong)(n - 1);

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        // Every comparison here reads one element; each pass also reads its starting candidate A[E].
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedCompares + expectedPasses);
    }

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesAllEqualTest(int n)
    {
        var stats = new StatisticsContext();
        var array = Enumerable.Repeat(7, n).ToArray();
        BingoSort.Sort(array.AsSpan(), stats);

        // The best case: one distinct key, one pass. The first pass compares the other n-1 elements
        // against the candidate, and the trailing check then retires all n-1 positions above first.
        var expectedCompares = (ulong)(2 * (n - 1));

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedCompares + 1);

        var observer = Observe(Enumerable.Repeat(7, n).ToArray());
        await Assert.That(observer.PassCount).IsEqualTo(1);
    }

    [Test]
    [Arguments(10)]
    [Arguments(11)]
    [Arguments(20)]
    [Arguments(51)]
    [Arguments(100)]
    public async Task TheoreticalValuesReversedTest(int n)
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        BingoSort.Sort(reversed.AsSpan(), stats);

        // Reversed input [n-1, ..., 0]. The first pass finds n-1 at index 0. Pass k (k = 1 .. ⌊n/2⌋)
        // starts its candidate at A[end] = k-1, sees every value of the reversed middle beat the candidate
        // (two comparisons each), swaps prevMax = n-k from index k-1 to end, which drops k-1 into its own
        // final place, and spends one comparison on each of the k-1 values already at the front. After
        // ⌊n/2⌋ such passes the array is sorted and the remaining passes cost what sorted input does.
        // Summing the two stages gives, for n >= 4:
        //   n even: 3n²/4 + 3n/2 - 3        n odd: (3n² + 6n - 13) / 4
        // which is ⌊(3n² + 6n - 12) / 4⌋ for both parities.
        var expectedCompares = (ulong)((3 * n * n + 6 * n - 12) / 4);
        var expectedSwaps = (ulong)(n / 2);

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(expectedSwaps);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedSwaps * 2);
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
        BingoSort.Sort(random.AsSpan(), stats);

        // Each swap puts one copy of the previous maximum into its final position, which no later
        // swap touches again, and position first is never a swap destination: at most n-1 swaps.
        var maxSwaps = (ulong)(n - 1);
        var observer = Observe(TestHelpers.ShuffledRange(n, seed));

        await Assert.That(stats.SwapCount).IsBetween(0UL, maxSwaps);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(stats.SwapCount * 2);
        // Distinct keys: at most one pass per key.
        await Assert.That(observer.PassCount).IsLessThanOrEqualTo(n);
    }

    [Test]
    [Arguments(200, 4, 42)]
    [Arguments(200, 4, 1234)]
    [Arguments(200, 16, 42)]
    [Arguments(200, 16, 1234)]
    public async Task TheoreticalValuesFewUniqueTest(int n, int m, int seed)
    {
        var stats = new StatisticsContext();
        var array = FewUnique(n, m, seed);
        var distinct = array.Distinct().Count();
        BingoSort.Sort(array.AsSpan(), stats);

        var observer = Observe(FewUnique(n, m, seed));
        var passes = observer.PassCount;

        // The defining property: one pass per distinct value, not per element.
        await Assert.That(passes).IsLessThanOrEqualTo(distinct);

        // A pass over a region of R elements makes at most 2(R-1) scan comparisons. Each trailing check
        // either retires a position (at most n-1 over the whole sort) or ends its loop (once per pass).
        var maxCompares = (ulong)(passes * 2 * (n - 1) + (n - 1) + passes);
        await Assert.That(stats.CompareCount).IsLessThanOrEqualTo(maxCompares);
        // For these inputs that bound is well below selection sort's n(n-1)/2.
        await Assert.That(stats.CompareCount).IsLessThan((ulong)(n * (n - 1) / 2));

        await Assert.That(stats.SwapCount).IsLessThanOrEqualTo((ulong)(n - 1));
    }

    [Test]
    [Arguments(64, 8, 42)]
    [Arguments(64, 8, 1234)]
    [Arguments(100, 100, 42)]
    [Arguments(100, 3, 7)]
    public async Task ObservationTracksSuffixAndCandidateTest(int n, int m, int seed)
    {
        var observer = Observe(FewUnique(n, m, seed));

        await Assert.That(observer.Problems).IsEmpty()
            .Because(string.Join("\n", observer.Problems));
        await Assert.That(observer.RoleLeftBehind).IsFalse();

        // The first pass is announced once, before any collecting pass, and covers the whole range.
        await Assert.That(observer.Phases[0]).IsEqualTo((SortPhase.BingoFindMax, 0, n - 1));
        await Assert.That(observer.Phases.Count(p => p.Phase == SortPhase.BingoFindMax)).IsEqualTo(1);

        // Each collecting pass covers a strictly smaller unsorted region than the one before it. The first
        // collecting pass may cover the same region as the first pass, which moves nothing.
        var ends = observer.Phases.Where(p => p.Phase == SortPhase.BingoPass).Select(p => p.End).ToList();
        for (var i = 0; i < ends.Count; i++)
        {
            await Assert.That(ends[i]).IsLessThan(i == 0 ? n : ends[i - 1]);
            await Assert.That(ends[i]).IsGreaterThan(0);
        }
    }

    [Test]
    public async Task RangeSortPhaseCoordinatesTest()
    {
        var array = new[] { 9, 1, 4, 1, 4, 3, 4, 2, 0 };
        var expected = array.ToArray();
        Array.Sort(expected, 2, 5);
        var observer = new BingoObserver(array, expected);

        // Sort only [2, 7): phases must name indices of the caller's span, not of the subrange.
        BingoSort.Sort(array.AsSpan(), 2, 7, observer);

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Assert.That(observer.Phases[0]).IsEqualTo((SortPhase.BingoFindMax, 2, 6));
        foreach (var (_, first, end) in observer.Phases)
        {
            await Assert.That(first).IsEqualTo(2);
            await Assert.That(end).IsBetween(3, 6);
        }
    }
}
