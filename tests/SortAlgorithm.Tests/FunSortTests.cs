using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class FunSortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => FunSort.Sort(span, context);

    // O((n + F) log n) with up to n(n-1)/2 inversions F: skip large inputs in data-driven tests.
    protected override int MaxOrderTestSize => 512;

    // Sorted input: every phase ends with its first search, so nothing moves.
    protected override CountExpectation SortedInputWrites => CountExpectation.Zero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    public async Task PaperExampleTest()
    {
        // Biedl et al., Section 4: phase 1 on (12, 14, 8, 1, 20). The first search probes 8, 1, 20 and ends at
        // position 5 (1-based), so cells 1 and 4 are swapped, giving (1, 14, 8, 12, 20); the search for 1 then
        // succeeds. In 0-based cells: probes 2, 3, 4, swap (0, 3), probes 2, 0.
        var array = new[] { 12, 14, 8, 1, 20 };
        var recorder = new FunRecorder(array);
        FunSort.Sort(array.AsSpan(), recorder);

        await Assert.That(recorder.ProbesInPhase(0)).IsEquivalentTo([2, 3, 4, 2, 0], CollectionOrdering.Matching);
        await Assert.That(recorder.SwapsInPhase(0)).IsEquivalentTo([(0, 3)], CollectionOrdering.Matching);
        await Assert.That(recorder.SnapshotAtPhase(1)).IsEquivalentTo([1, 14, 8, 12, 20], CollectionOrdering.Matching);
        await Assert.That(array).IsEquivalentTo([1, 8, 12, 14, 20], CollectionOrdering.Matching);
    }

    [Test]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(3)]
    [Arguments(5)]
    [Arguments(10)]
    public async Task WorstCaseFamilyPhaseOneTest(int k)
    {
        // Biedl et al., Theorem 7: on k+2, ..., 2k+1, 1, ..., k+1 the first phase makes k+1 failed searches and
        // leaves 1, k+3, ..., 2k+1, 2, ..., k+1, k+2.
        var n = 2 * k + 1;
        int[] input = [.. Enumerable.Range(k + 2, k), .. Enumerable.Range(1, k + 1)];
        int[] afterPhaseOne = [1, .. Enumerable.Range(k + 3, k - 1), .. Enumerable.Range(2, k), k + 2];
        var inversions = CountInversions(input);

        var array = input.ToArray();
        var recorder = new FunRecorder(array);
        var stats = new StatisticsContext();
        FunSort.Sort(array.AsSpan(), new CompositeContext(stats, recorder));

        await Assert.That(recorder.SwapsInPhase(0).Count).IsEqualTo(k + 1);
        await Assert.That(recorder.SnapshotAtPhase(1)).IsEquivalentTo(afterPhaseOne, CollectionOrdering.Matching);
        await Assert.That(array).IsEquivalentTo(Enumerable.Range(1, n), CollectionOrdering.Matching);
        await Assert.That(stats.SwapCount <= (ulong)inversions).IsTrue()
            .Because($"SwapCount ({stats.SwapCount}) should not exceed the input's inversions ({inversions})");
    }

    [Test]
    [Arguments(7, 3)]
    [Arguments(15, 4)]
    [Arguments(31, 5)]
    public async Task TheoreticalValuesSortedTest(int n, int log)
    {
        // n = 2^log - 1: the search interval starts at 2^log and halves exactly, so every search makes log comparisons.
        // Sorted input ends every phase with its first search: n searches, each reading the key once and every probe once.
        var stats = new StatisticsContext();
        var array = Enumerable.Range(0, n).ToArray();
        FunSort.Sort(array.AsSpan(), stats);

        await Assert.That(stats.CompareCount).IsEqualTo((ulong)(n * log));
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexReadCount).IsEqualTo((ulong)(n + n * log));
    }

    [Test]
    [Arguments(5, 42)]
    [Arguments(5, 1234)]
    [Arguments(15, 42)]
    [Arguments(15, 1234)]
    [Arguments(33, 42)]
    [Arguments(33, 1234)]
    [Arguments(100, 42)]
    [Arguments(100, 1234)]
    public async Task TheoreticalValuesRandomTest(int n, int seed)
    {
        // Invariants that hold for every input:
        // - every swap exchanges a strictly out-of-order pair, so swaps <= input inversions (Lemma 1)
        // - each phase i starts with cells 0..i-1 in order, phases are announced 0..n-1, and cell i carries Inserting for the phase
        // - one search per swap plus one successful search per phase, each making floor or ceil of log2(n+1) comparisons
        // - reads: one key read per search, one per comparison, two per swap; writes: two per swap
        var input = TestHelpers.ShuffledRange(n, seed);
        var inversions = CountInversions(input);
        var array = input.ToArray();
        var recorder = new FunRecorder(array);
        var stats = new StatisticsContext();
        FunSort.Sort(array.AsSpan(), new CompositeContext(stats, recorder));

        await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, n), CollectionOrdering.Matching);
        await Assert.That(recorder.Problems).IsEmpty();
        await Assert.That(recorder.Phases).IsEquivalentTo(Enumerable.Range(0, n), CollectionOrdering.Matching);
        await Assert.That(recorder.Roles).IsEquivalentTo(
            Enumerable.Range(0, n).SelectMany(i => new[] { (i, RoleType.Inserting), (i, RoleType.None) }), CollectionOrdering.Matching);

        var swaps = stats.SwapCount;
        await Assert.That(swaps <= (ulong)inversions).IsTrue()
            .Because($"SwapCount ({swaps}) should not exceed the input's inversions ({inversions})");

        var searches = (ulong)n + swaps;
        var floorLog = (ulong)Math.Floor(Math.Log2(n + 1));
        var ceilLog = (ulong)Math.Ceiling(Math.Log2(n + 1));
        await Assert.That(stats.CompareCount >= searches * floorLog && stats.CompareCount <= searches * ceilLog).IsTrue()
            .Because($"CompareCount ({stats.CompareCount}) should be within [{searches * floorLog}, {searches * ceilLog}] for {searches} searches");
        if (floorLog == ceilLog)
            await Assert.That(stats.CompareCount).IsEqualTo(searches * floorLog);

        await Assert.That(stats.IndexReadCount).IsEqualTo(searches + stats.CompareCount + 2 * swaps);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(2 * swaps);
    }

    [Test]
    [Arguments("EqualCopyLeftOfCell")]
    [Arguments("EqualCopyRightOfCell")]
    [Arguments("AllEqual")]
    [Arguments("ThreeValues")]
    [Arguments("TwoValuesReversed")]
    public async Task EqualKeysTest(string shape)
    {
        // With values compared alone, [1, 5, 1] either ends unsorted or searches forever: the search for the last 1
        // lands on the first. Ordering equal values by position keeps every swap a strict inversion.
        // Both tie directions are covered: an equal copy left of the searched cell counts as smaller, one to its right as larger.
        int[] input = shape switch
        {
            "EqualCopyLeftOfCell" => [1, 5, 1],
            "EqualCopyRightOfCell" => [2, 1, 2],
            "AllEqual" => Enumerable.Repeat(7, 16).ToArray(),
            "ThreeValues" => Enumerable.Range(0, 40).Select(i => (i * 7 + 3) % 3).ToArray(),
            "TwoValuesReversed" => [.. Enumerable.Repeat(1, 10), .. Enumerable.Repeat(0, 10)],
            _ => throw new ArgumentOutOfRangeException(nameof(shape)),
        };
        var inversions = CountInversions(input);
        var array = input.ToArray();
        var recorder = new FunRecorder(array);
        var stats = new StatisticsContext();
        FunSort.Sort(array.AsSpan(), new CompositeContext(stats, recorder));

        await Assert.That(array).IsEquivalentTo(input.Order(), CollectionOrdering.Matching);
        await Assert.That(recorder.Problems).IsEmpty();
        await Assert.That(stats.SwapCount <= (ulong)inversions).IsTrue()
            .Because($"SwapCount ({stats.SwapCount}) should not exceed the input's inversions ({inversions})");
    }

    [Test]
    public async Task UnstableForEqualKeysTest()
    {
        // IsStable is false: phase 3 searches for (0, 2), lands on cell 0 and swaps it with the first (1, 0),
        // carrying that record past its equal (1, 1).
        var byKey = Comparer<(int Key, int Id)>.Create((x, y) => x.Key.CompareTo(y.Key));
        (int Key, int Id)[] array = [(1, 0), (1, 1), (0, 2)];
        FunSort.Sort(array.AsSpan(), byKey, NullContext.Default);

        await Assert.That(array.Select(x => x.Key)).IsEquivalentTo([0, 1, 1], CollectionOrdering.Matching);
        await Assert.That(array.Select(x => x.Id)).IsEquivalentTo([2, 1, 0], CollectionOrdering.Matching);
    }

    [Test]
    public async Task CustomComparerTest()
    {
        var stats = new StatisticsContext();
        var array = TestHelpers.ShuffledRange(50, 42);
        var descending = Comparer<int>.Create((x, y) => y.CompareTo(x));
        FunSort.Sort(array.AsSpan(), descending, stats);

        await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, 50).Reverse(), CollectionOrdering.Matching);
        await Assert.That(stats.SwapCount).IsNotEqualTo(0UL);
    }

    [Test]
    public async Task NullContextTest()
    {
        var array = TestHelpers.ShuffledRange(200, 1234);
        FunSort.Sort(array.AsSpan());

        await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, 200), CollectionOrdering.Matching);
    }

    static long CountInversions(int[] values)
    {
        long count = 0;
        for (var i = 0; i < values.Length; i++)
            for (var j = i + 1; j < values.Length; j++)
                if (values[i] > values[j]) count++;
        return count;
    }

    /// <summary>
    /// Watches the sort through the array it is sorting: checks each swap and each phase start against the
    /// current contents, and records probes, swaps, roles and the array at every phase start.
    /// </summary>
    sealed class FunRecorder(int[] array) : ISortContext
    {
        int phase = -1;
        readonly Dictionary<int, List<int>> probes = [];
        readonly Dictionary<int, List<(int, int)>> swaps = [];
        readonly Dictionary<int, int[]> snapshots = [];

        public List<int> Phases { get; } = [];
        public List<(int Index, RoleType Role)> Roles { get; } = [];
        public List<string> Problems { get; } = [];

        public List<int> ProbesInPhase(int i) => probes.TryGetValue(i, out var list) ? list : [];
        public List<(int, int)> SwapsInPhase(int i) => swaps.TryGetValue(i, out var list) ? list : [];
        public int[] SnapshotAtPhase(int i) => snapshots[i];

        public void OnPhase(SortPhase sortPhase, int param1 = 0, int param2 = 0, int param3 = 0)
        {
            if (sortPhase != SortPhase.FunSortPhase) return;
            phase = param1;
            Phases.Add(param1);
            snapshots[param1] = array.ToArray();
            if (param2 != array.Length - 1) Problems.Add($"phase {param1}: param2 {param2} is not the last index");
            for (var k = 1; k < param1; k++)
                if (array[k - 1] > array[k]) Problems.Add($"phase {param1}: prefix out of order at {k - 1}..{k}");
        }

        public void OnSwap(int i, int j, int bufferId)
        {
            // Announced before the swap: the pair must be strictly out of order now.
            var (lo, hi) = i < j ? (i, j) : (j, i);
            if (!(array[lo] > array[hi])) Problems.Add($"phase {phase}: swap ({i}, {j}) of {array[lo]} and {array[hi]} is not an inversion");
            if (lo != phase && hi != phase) Problems.Add($"phase {phase}: swap ({i}, {j}) does not involve cell {phase}");
            if (!swaps.TryGetValue(phase, out var list)) swaps[phase] = list = [];
            list.Add((i, j));
        }

        public void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ)
        {
            if (!probes.TryGetValue(phase, out var list)) probes[phase] = list = [];
            list.Add(j);
        }

        public void OnRole(int index, int bufferId, RoleType role) => Roles.Add((index, role));

        public void OnIndexRead(int index, int bufferId) { }
        public void OnIndexWrite(int index, int bufferId) { }
        public void OnIndexWrite<T>(int index, int bufferId, T value) { }
        public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId) { }
        public void OnRangeCopy<T>(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, ReadOnlySpan<T> values) { }
    }
}
