using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class BPlusTreeSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => BPlusTreeSort.Sort(span, context);

    // Node creation and the write-back pass always write, even for input that is already sorted.
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    // Elements move through node slots; array slots are never swapped.
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    /// <summary>
    /// Occupancy alone bounds the height, so no input degenerates the way an unbalanced binary search tree
    /// does on sorted input. The bound is the theoretical one: each of the n descents visits at most one node
    /// per level, paying at most ⌈log₂(2t)⌉ comparisons inside it plus one after a split.
    /// </summary>
    [Test]
    [Arguments(1000)]
    [Arguments(4096)]
    public async Task ComparisonsStayWithinTheHeightBoundForEveryPattern(int n)
    {
        var inputs = new (string Name, int[] Data)[]
        {
            ("sorted", [.. Enumerable.Range(0, n)]),
            ("reversed", [.. Enumerable.Range(0, n).Reverse()]),
            ("random", TestHelpers.ShuffledRange(n, 20260803)),
            ("allEqual", [.. Enumerable.Repeat(7, n)]),
        };

        const int MinDegree = 8;
        const int MaxComparisonsPerNode = 5;
        var maxHeight = (int)Math.Floor(Math.Log((n + 1) / 2.0, MinDegree)) + 1;
        var bound = (ulong)n * MaxComparisonsPerNode * (ulong)maxHeight;

        foreach (var (name, data) in inputs)
        {
            var stats = new StatisticsContext();
            var array = data.ToArray();
            BPlusTreeSort.Sort(array.AsSpan(), stats);

            await Assert.That(array).IsEquivalentTo([.. data.Order()], CollectionOrdering.Matching)
                .Because($"{name} input must sort");
            await Assert.That(stats.CompareCount).IsLessThanOrEqualTo(bound)
                .Because($"{name} input must stay within the O(n log n) comparison bound (height <= {maxHeight})");
            await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        }
    }

    /// <summary>
    /// The output phase is the property that distinguishes a B+ tree: the leaves are chained, so writing the
    /// result back reads each element once, in leaf order, and touches no internal node. Nothing about the
    /// sorted array shows that, but the operation stream does — during the scan phase every tree read must
    /// land in a leaf, and the number of tree reads must be linear in n rather than proportional to the
    /// tree's internal size.
    /// </summary>
    [Test]
    [Arguments(1000)]
    [Arguments(4096)]
    public async Task ScanPhaseWalksTheLeavesLinearly(int n)
    {
        var recorder = new ScanPhaseRecorder();
        var array = TestHelpers.ShuffledRange(n, 20260805);
        BPlusTreeSort.Sort(array.AsSpan(), recorder);

        // Per leaf: one key count read, one chain read, and one read per key. Plus the final chain read that
        // ends the walk. With at least t-1 = 7 keys per leaf that is comfortably under 2n for any n >= 15.
        await Assert.That(recorder.ScanTreeReads).IsGreaterThanOrEqualTo((ulong)n);
        await Assert.That(recorder.ScanTreeReads).IsLessThanOrEqualTo((ulong)n * 2);
        await Assert.That(recorder.ScanMainWrites).IsEqualTo((ulong)n);
        await Assert.That(recorder.ScanComparisons).IsEqualTo(0UL)
            .Because("the leaf chain is already in order, so the output phase compares nothing");
    }

    /// <summary>
    /// A B+ tree stores every element in a leaf and every separator as a copy, so the tree buffer legitimately
    /// holds more keys than the input has elements. What must not happen is the opposite mistake — an element
    /// being moved up into an internal node and therefore leaving the leaf level — which would show up as a
    /// missing or duplicated element in the output.
    /// </summary>
    [Test]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(15)]   // exactly one full leaf, no split yet
    [Arguments(16)]   // the first root split
    [Arguments(17)]
    [Arguments(121)]  // enough for a third level
    [Arguments(1000)]
    public async Task DuplicateHeavyInputIsSortedAndPreserved(int n)
    {
        var random = new Random(20260804);
        var data = Enumerable.Range(0, n).Select(_ => random.Next(0, 4)).ToArray();
        var array = data.ToArray();

        BPlusTreeSort.Sort(array.AsSpan(), new StatisticsContext());

        await Assert.That(array).IsEquivalentTo([.. data.Order()], CollectionOrdering.Matching);
    }

    /// <summary>
    /// Counts what happens after the scan phase is announced, which is where the leaf walk lives.
    /// </summary>
    private sealed class ScanPhaseRecorder : ISortContext
    {
        private const int TreeBufferId = 1;
        private bool _scanning;

        public ulong ScanTreeReads { get; private set; }
        public ulong ScanMainWrites { get; private set; }
        public ulong ScanComparisons { get; private set; }

        public void OnPhase(SortPhase phase, int param1 = 0, int param2 = 0, int param3 = 0)
        {
            if (phase == SortPhase.BPlusTreeScan) _scanning = true;
        }

        public void OnIndexRead(int index, int bufferId)
        {
            if (_scanning && bufferId == TreeBufferId) ScanTreeReads++;
        }

        public void OnIndexWrite<T>(int index, int bufferId, T value)
        {
            if (_scanning && bufferId == 0) ScanMainWrites++;
        }

        public void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ)
        {
            if (_scanning) ScanComparisons++;
        }

        public void OnSwap(int i, int j, int bufferId) { }
        public void OnIndexWrite(int index, int bufferId) { }
        public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId) { }
        public void OnRangeCopy<T>(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, ReadOnlySpan<T> values) { }
        public void OnRole(int index, int bufferId, RoleType role) { }
    }
}
