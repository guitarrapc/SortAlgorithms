using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class NaturalMergeSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => NaturalMergeSort.Sort(span, context);

    // Natural Merge Sort on sorted data: single ascending run, no merge needed.
    protected override CountExpectation SortedInputWrites => CountExpectation.Zero;
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
        NaturalMergeSort.Sort(sorted.AsSpan(), stats);

        // Natural Merge Sort on sorted data:
        // DetectRuns finds a single ascending run covering the entire array.
        // 1 direction-check comparison + (n-2) extension comparisons = n-1.
        // With runCount=1 there is nothing to merge: 0 writes, 0 swaps.
        var expectedCompares = (ulong)(n - 1);

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
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
        NaturalMergeSort.Sort(reversed.AsSpan(), stats);

        // Natural Merge Sort on reversed data (with descending run detection):
        // DetectRuns finds a single strictly descending run covering the entire array,
        // then reverses it in-place via n/2 swaps. runCount=1, so no merge is needed.
        //
        // Comparisons: n-1 (same as sorted — 1 direction check + n-2 extension)
        // Swaps: n/2 (reverse the descending run)
        // Writes: n (each swap is tracked as 2 writes, n/2 swaps × 2 = n)
        var expectedCompares = (ulong)(n - 1);
        var expectedSwaps = (ulong)(n / 2);
        var expectedWrites = (ulong)n;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(expectedSwaps);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
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
        NaturalMergeSort.Sort(random.AsSpan(), stats);

        // Natural Merge Sort on random data:
        // Run detection scans once (n-1 comparisons) and finds at most ⌈n/2⌉ runs, so at most
        // ⌈log₂ ⌈n/2⌉⌉ merge passes follow, each costing at most n comparisons including the
        // boundary checks. Observed: 0.87-1.08 n log₂ n for these inputs.
        var logN = Math.Log2(n);
        var maxPasses = (int)Math.Ceiling(Math.Log2((n + 1) / 2));
        var minCompares = (ulong)(n * logN * 0.8);
        var maxCompares = (ulong)(n - 1 + n * maxPasses);

        var minWrites = (ulong)(n * logN * 0.3);
        var maxWrites = (ulong)(n * Math.Ceiling(logN) * 2.0);

        // Every comparison reads at least one element it has not read before: detection and the boundary
        // checks read two, and each merge step reads only the head that replaces the one just taken.
        var minReads = stats.CompareCount;

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount >= minReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minReads}");
    }

    [Test]
    public async Task MergeReadsEachHeadOnceTest()
    {
        var stats = new StatisticsContext();
        int[] data = [0, 2, 1, 3];
        NaturalMergeSort.Sort(data.AsSpan(), stats);

        // Detection finds [0, 2] and [1, 3] (3 comparisons, 6 reads), and the boundary check fails (1, 2).
        // The runs have equal length, so the left one is buffered (2 reads, 2 writes). The merge reads both heads (2),
        // then takes 0, 1 and 2, reading only the replacement head after the first two (2 reads, 3 comparisons, 3 writes).
        // When the left run runs out, 3 is already in place.
        await Assert.That(data).IsEquivalentTo([0, 1, 2, 3], CollectionOrdering.Matching);
        await Assert.That(stats.CompareCount).IsEqualTo(7UL);
        await Assert.That(stats.IndexReadCount).IsEqualTo(14UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(5UL);
    }

    [Test]
    public async Task InOrderBoundaryAfterReversalSkipsMergeTest()
    {
        var stats = new StatisticsContext();
        int[] data = [5, 3, 7, 8];
        NaturalMergeSort.Sort(data.AsSpan(), stats);

        // Detection reverses [5, 3] into [3, 5] (2 comparisons, 1 swap) and finds [7, 8] (1 comparison).
        // 5 <= 7, so the two runs are already one and the merge is skipped after one comparison.
        await Assert.That(data).IsEquivalentTo([3, 5, 7, 8], CollectionOrdering.Matching);
        await Assert.That(stats.CompareCount).IsEqualTo(4UL);
        await Assert.That(stats.SwapCount).IsEqualTo(1UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(2UL);
    }

    [Test]
    public async Task EveryJoinedPairIsAnnouncedTest()
    {
        var recorder = new PhaseRecorder();
        int[] data = [5, 3, 7, 8, 1, 2];
        NaturalMergeSort.Sort(data.AsSpan(), recorder);

        // Runs [3, 5], [7, 8] and [1, 2] are detected once. Pass 1 joins the first two without merging (5 <= 7)
        // and carries [1, 2]; pass 2 merges [3, 5, 7, 8] with [1, 2]. Run-tracking consumers rely on both joins.
        await Assert.That(data).IsEquivalentTo([1, 2, 3, 5, 7, 8], CollectionOrdering.Matching);
        await Assert.That(recorder.Phases).IsEquivalentTo(
            [(SortPhase.MergeRunDetect, 0, 0, 0), (SortPhase.MergeSortMerge, 0, 1, 3), (SortPhase.MergeSortMerge, 0, 3, 5)],
            CollectionOrdering.Matching);
    }

    [Test]
    [Arguments(10)]
    [Arguments(100)]
    [Arguments(1000)]
    public async Task ShorterRightRunIsBufferedTest(int leftLength)
    {
        // A long ascending run of even numbers followed by one element that belongs just before its last element.
        var data = Enumerable.Range(0, leftLength).Select(i => 2 * i).Append(2 * leftLength - 3).ToArray();
        var stats = new StatisticsContext();
        NaturalMergeSort.Sort(data.AsSpan(), stats);

        // Detection compares every adjacent pair of the long run and its boundary (leftLength comparisons), then the
        // boundary check fails (1). Only the single right element is buffered (1 write). Merging from the back moves the
        // last left element up (1 comparison, 1 write) and places the right element (1 comparison, 1 write); the rest
        // of the left run is already in place.
        var expected = data.Order().ToArray();
        await Assert.That(data).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Assert.That(stats.CompareCount).IsEqualTo((ulong)leftLength + 3UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(3UL);
    }

    [Test]
    public async Task BackwardMergeKeepsEqualKeysInOrderTest()
    {
        // Left run keys [0, 1, 1, 1, 3] (ids 0-4), right run keys [1, 1] (ids 5-6): the right run is shorter and merged from the back.
        (int Key, int Id)[] data = [(0, 0), (1, 1), (1, 2), (1, 3), (3, 4), (1, 5), (1, 6)];
        NaturalMergeSort.Sort(data.AsSpan(), new KeyComparer(), NullContext.Default);

        await Assert.That(data.Select(x => x.Id).ToArray()).IsEquivalentTo([0, 1, 2, 3, 5, 6, 4], CollectionOrdering.Matching);
    }

    [Test]
    public async Task ForwardMergeOfEqualLengthRunsKeepsEqualKeysInOrderTest()
    {
        // Runs of equal length buffer the left one and merge from the front.
        (int Key, int Id)[] data = [(1, 0), (2, 1), (1, 2), (2, 3)];
        NaturalMergeSort.Sort(data.AsSpan(), new KeyComparer(), NullContext.Default);

        await Assert.That(data.Select(x => x.Id).ToArray()).IsEquivalentTo([0, 2, 1, 3], CollectionOrdering.Matching);
    }

    /// <summary>
    /// The pass count depends only on the recorded runs, so a comparer that never reports a consistent order still ends the sort,
    /// and every merge moves elements only within the range.
    /// </summary>
    [Test]
    [Arguments(3)]
    [Arguments(17)]
    [Arguments(40)]
    [Arguments(100)]
    public async Task InconsistentComparerFinishesWithTheSameElementsTest(int n)
    {
        for (var seed = 1; seed <= 8; seed++)
        {
            var input = TestHelpers.ShuffledRange(n, seed);
            IComparer<int>[] comparers = [new ConstantComparer(-1), new ConstantComparer(1), new CoinFlipComparer(new Random(seed))];
            foreach (var comparer in comparers)
            {
                var data = input.ToArray();
                NaturalMergeSort.Sort(data.AsSpan(), comparer, NullContext.Default);

                await Assert.That(data.Order().SequenceEqual(input.Order())).IsTrue().Because($"n={n} seed={seed} {comparer.GetType().Name}");
            }
        }
    }

    readonly struct KeyComparer : IComparer<(int Key, int Id)>
    {
        public int Compare((int Key, int Id) x, (int Key, int Id) y) => x.Key.CompareTo(y.Key);
    }

    sealed class ConstantComparer(int answer) : IComparer<int>
    {
        public int Compare(int x, int y) => answer;
    }

    sealed class CoinFlipComparer(Random random) : IComparer<int>
    {
        public int Compare(int x, int y) => random.Next(3) - 1;
    }

    sealed class PhaseRecorder : ISortContext
    {
        public List<(SortPhase Phase, int P1, int P2, int P3)> Phases { get; } = [];

        public void OnPhase(SortPhase phase, int param1 = 0, int param2 = 0, int param3 = 0) => Phases.Add((phase, param1, param2, param3));
        public void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ) { }
        public void OnSwap(int i, int j, int bufferId) { }
        public void OnIndexRead(int index, int bufferId) { }
        public void OnIndexWrite(int index, int bufferId) { }
        public void OnIndexWrite<T>(int index, int bufferId, T value) { }
        public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId) { }
        public void OnRangeCopy<T>(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, ReadOnlySpan<T> values) { }
        public void OnRole(int index, int bufferId, RoleType role) { }
    }
}
