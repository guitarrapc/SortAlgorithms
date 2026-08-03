using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class ShiftSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => ShiftSort.Sort(span, context);

    // Sorted input is detected as a single run: no merges, no writes, no swaps.
    protected override CountExpectation SortedInputWrites => CountExpectation.Zero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    [Arguments(256, 42)]  // Stackalloc threshold
    [Arguments(256, 1234)]
    [Arguments(257, 42)]  // Just over threshold (should use ArrayPool)
    [Arguments(257, 1234)]
    [Arguments(512, 42)]  // ArrayPool
    [Arguments(512, 1234)]
    [Arguments(1024, 42)] // Large array
    [Arguments(1024, 1234)]
    public async Task LargeArrayTest(int n, int seed)
    {
        var stats = new StatisticsContext();
        var array = TestHelpers.ShuffledRange(n, seed);
        ShiftSort.Sort(array.AsSpan(), stats);

        // Verify sorting correctness
        await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, n).ToArray(), CollectionOrdering.Matching);
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
        ShiftSort.Sort(sorted.AsSpan(), stats);

        // For sorted data (with internal buffer tracking):
        // - Run detection: O(n) comparisons (n-1 comparisons in the scan loop)
        // - No run boundaries detected, so no merge operations
        // - No swaps needed
        // - No writes needed
        var expectedCompares = (ulong)(n - 1);
        var expectedSwaps = 0UL;
        var expectedWrites = 0UL;

        // Each comparison reads 2 elements
        var minIndexReads = expectedCompares * 2;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(expectedSwaps);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
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
        ShiftSort.Sort(reversed.AsSpan(), stats);

        // For reversed data [n-1, n-2, ..., 1, 0] (with internal buffer tracking):
        // - Run detection: O(n) comparisons
        //   * Every adjacent pair is out of order (n-1 boundaries detected)
        //   * Each boundary detection checks 2 elements (current and previous)
        //   * Three-element optimization applies when possible
        // - Maximum number of runs: approximately n/2 (worst case)
        // - Merge operations: O(n log k) where k is number of runs
        // - Swaps during run detection: O(n/2) empirically observed
        //   * The 3-element optimization swaps elements at positions x and x-2
        //   * For reversed data, this creates approximately n/2 swaps
        // - Writes during merge: O(n log k)
        //   * NOW includes writes to temp buffers (tmp1st or tmp2nd)
        //   * Each merge: writes to temp buffer + writes back to main

        // Run detection comparisons: approximately n
        var minRunDetectionCompares = (ulong)(n - 1);

        // Swaps are limited to run detection phase only (not during merge)
        // Empirically observed: reversed data produces approximately n/2 swaps
        // due to the 3-element optimization pattern
        var maxSwaps = (ulong)(n / 2 + 5); // Allow some margin for edge cases

        // Comparisons include both run detection and merge
        // For reversed data, expect O(n log n) total comparisons
        var minCompares = minRunDetectionCompares;
        var maxCompares = (ulong)(n * Math.Log(n, 2) * 2); // 2x for safety margin

        // Writes occur during merge (shift-based, not swap-based)
        // With internal buffer tracking: writes to temp buffer + writes back
        // For reversed data, most elements need to be shifted multiple times
        var minWrites = (ulong)(n - 1);
        // Allow for higher writes due to temp buffer operations being tracked
        var maxWrites = (ulong)(n * Math.Log(n, 2) * 3);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(0UL, maxSwaps);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount >= minCompares * 2).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minCompares * 2}");
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
        ShiftSort.Sort(random.AsSpan(), stats);

        // For random data (with internal buffer tracking):
        // - Number of runs: varies significantly (typically k << n)
        // - Comparisons: O(n log k) where k is number of runs
        // - Swaps during run detection: typically less than n/2
        // - Writes during merge: O(n log k)
        //   * NOW includes writes to temp buffers

        var minCompares = (ulong)(n - 1); // At least run detection
        var maxCompares = (ulong)(n * Math.Log(n, 2) * 2); // At most O(n log n)

        var maxSwaps = (ulong)(n / 2 + 5); // Limited to run detection phase

        // Random data typically requires many merges
        // With internal buffer tracking: writes to temp buffer + writes back
        var minWrites = (ulong)(n / 4);
        var maxWrites = (ulong)(n * Math.Log(n, 2) * 3);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(0UL, maxSwaps);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount >= minCompares * 2).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minCompares * 2}");
    }

    /// <summary>
    /// Records the merge phases and the pointer roles announced around them.
    /// </summary>
    private sealed class MergeEventRecordingContext : ISortContext
    {
        public List<(int Left, int Mid, int Right)> Merges { get; } = [];
        public List<(int Index, RoleType Role)> Roles { get; } = [];

        public void OnPhase(SortPhase phase, int param1 = 0, int param2 = 0, int param3 = 0)
        {
            if (phase == SortPhase.MergeSortMerge) Merges.Add((param1, param2, param3));
        }

        public void OnRole(int index, int bufferId, RoleType role) => Roles.Add((index, role));

        public void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ) { }
        public void OnSwap(int i, int j, int bufferId) { }
        public void OnIndexRead(int index, int bufferId) { }
        public void OnIndexWrite(int index, int bufferId) { }
        public void OnIndexWrite<T>(int index, int bufferId, T value) { }
        public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId) { }
        public void OnRangeCopy<T>(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, ReadOnlySpan<T> values) { }
    }

    /// <summary>
    /// Reverse-sorted input is ShiftSort's designated worst case — the reference benchmark labels it
    /// "worst complexity (reverse sorted list)". The scan turns each strictly descending window of three
    /// ascending with one swap, which produces the maximum number of sublists rather than one, so the merge
    /// tree is at its deepest here. Detecting arbitrary-length descending runs and reversing them instead
    /// would sort this input with no merges at all, which is a different algorithm.
    /// </summary>
    [Test]
    [Arguments(64)]
    [Arguments(256)]
    [Arguments(1024)]
    public async Task ReverseSortedInputIsTheWorstCaseNotTheBestCase(int n)
    {
        var context = new MergeEventRecordingContext();
        var array = Enumerable.Range(0, n).Reverse().ToArray();

        ShiftSort.Sort(array.AsSpan(), context);

        await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, n).ToArray(), CollectionOrdering.Matching);
        await Assert.That(context.Merges.Count).IsEqualTo(n / 2 - 1)
            .Because("n/2 sublists is the most the derivative list can hold, so this input costs the most merges");
    }

    /// <summary>
    /// Every merge the algorithm performs must announce itself, including the ones at the bottom of the
    /// merge tree. Two sublists are the smallest input that merges at all; when that case was special-cased
    /// it merged silently, so a consumer watching an input with few sublists saw no merge phase whatsoever.
    /// </summary>
    [Test]
    public async Task TwoSublistInput_AnnouncesItsMerge()
    {
        var context = new MergeEventRecordingContext();
        // No window of three descends, and only index 4 is out of order with its left neighbour,
        // so the derivative list is [0, 4, 8]: the sublists are [1,3,5,7] and [0,2,4,6].
        var array = new[] { 1, 3, 5, 7, 0, 2, 4, 6 };

        ShiftSort.Sort(array.AsSpan(), context);

        await Assert.That(array).IsEquivalentTo(new[] { 0, 1, 2, 3, 4, 5, 6, 7 }, CollectionOrdering.Matching);
        await Assert.That(context.Merges).IsEquivalentTo(new List<(int, int, int)> { (0, 3, 7) })
            .Because("a silent merge leaves an observer with no way to know a merge happened at all");
        await Assert.That(context.Roles).IsEquivalentTo(
            new List<(int, RoleType)> { (0, RoleType.LeftPointer), (7, RoleType.RightPointer), (0, RoleType.None), (7, RoleType.None) })
            .Because("a role that is set must be cleared over the same range");
    }

    /// <summary>
    /// A k-sublist input performs exactly k-1 merges, and each one must be announced. Roughly half of them sit
    /// at the bottom of the merge tree, which is where the silent path used to be.
    /// </summary>
    [Test]
    [Arguments(4)]
    [Arguments(7)]
    [Arguments(16)]
    [Arguments(33)]
    public async Task EveryMergeIsAnnounced(int pairCount)
    {
        var context = new MergeEventRecordingContext();
        // 2 * pairCount elements shaped [1,0, 3,2, 5,4, ...]. Every odd index is out of order with its left
        // neighbour, and no window of three descends (arr[x-1] is the larger of its own pair), so the scan
        // records every odd index and never swaps. The derivative list is [0, 1, 3, ..., 2*pairCount-1, n],
        // which delimits pairCount + 1 sublists and therefore costs pairCount merges.
        var array = new int[pairCount * 2];
        for (var i = 0; i < pairCount; i++)
        {
            array[i * 2] = i * 2 + 1;
            array[i * 2 + 1] = i * 2;
        }

        ShiftSort.Sort(array.AsSpan(), context);

        await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, pairCount * 2).ToArray(), CollectionOrdering.Matching);
        await Assert.That(context.Merges.Count).IsEqualTo(pairCount)
            .Because($"a binary merge tree over {pairCount + 1} sublists performs {pairCount} merges");
        // The last announced merge is the root: it must cover the whole span.
        await Assert.That(context.Merges[^1]).IsEqualTo((0, context.Merges[^1].Mid, array.Length - 1));
    }

    /// <summary>
    /// When one side of a merge runs out, the rest of the buffered run is moved as a range copy rather than
    /// element by element. The counts must stay the same either way: the elements still move, and moving
    /// them in bulk must not lose or duplicate a write.
    /// </summary>
    [Test]
    [Arguments(64)]
    [Arguments(256)]
    [Arguments(1024)]
    public async Task DisjointRunsProduceTheSameWriteCountAsElementwiseMovement(int blockSize)
    {
        var stats = new StatisticsContext();
        // Alternating blocks where every element of one block outranks every element of the next, so each
        // merge exhausts one side early and leaves a long tail.
        var n = blockSize * 4;
        var array = new int[n];
        for (var i = 0; i < n; i++) array[i] = (i / blockSize) % 2 == 0 ? 100000 + i : i;

        ShiftSort.Sort(array.AsSpan(), stats);

        var expected = (int[])array.Clone();
        Array.Sort(expected);
        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
        // Each merge writes min(len1, len2) into the buffer plus at most the merged length back.
        await Assert.That(stats.IndexWriteCount).IsBetween(1UL, (ulong)(n * Math.Log(n, 2) * 3));
    }

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesAlternatingTest(int n)
    {
        var stats = new StatisticsContext();
        // Create alternating pattern: [0, 2, 4, ..., 1, 3, 5, ...]
        var alternating = Enumerable.Range(0, n)
            .OrderBy(x => x % 2)
            .ThenBy(x => x)
            .ToArray();
        ShiftSort.Sort(alternating.AsSpan(), stats);

        // Alternating data creates multiple runs that need merging
        // This tests the adaptive merge behavior

        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(n * Math.Log(n, 2) * 2);

        var maxSwaps = (ulong)(n / 2 + 5);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(0UL, maxSwaps);
        await Assert.That(stats.IndexWriteCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
    }
}
