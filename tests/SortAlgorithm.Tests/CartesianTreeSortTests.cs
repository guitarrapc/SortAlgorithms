using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class CartesianTreeSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => CartesianTreeSort.Sort(span, context);

    // Node allocation and priority queue traffic make large inputs slow, matching the other tree sorts.
    protected override int MaxOrderTestSize => 1024;

    // Tree construction (CreateNode, linking) and the write-back always write, even for sorted input.
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    // Elements move by node writes; the priority queue only permutes arena indices.
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    /// <summary>
    /// Counts comparisons per phase so the linear construction and the adaptive extraction can be
    /// asserted separately. A single total hides both: a build that degraded to a search would still
    /// land inside any O(n log n) bound the total is checked against.
    /// </summary>
    private sealed class PhaseCompareCountingContext : ISortContext
    {
        private SortPhase _current = SortPhase.None;

        public ulong BuildCompares { get; private set; }
        public ulong ExtractCompares { get; private set; }
        public ulong OtherCompares { get; private set; }
        public int BuildPhaseCount { get; private set; }
        public int ExtractPhaseCount { get; private set; }

        public void OnPhase(SortPhase phase, int param1 = 0, int param2 = 0, int param3 = 0)
        {
            _current = phase;
            if (phase == SortPhase.CartesianTreeBuild) BuildPhaseCount++;
            else if (phase == SortPhase.CartesianTreeExtract) ExtractPhaseCount++;
        }

        public void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ)
        {
            switch (_current)
            {
                case SortPhase.CartesianTreeBuild: BuildCompares++; break;
                case SortPhase.CartesianTreeExtract: ExtractCompares++; break;
                default: OtherCompares++; break;
            }
        }

        public void OnSwap(int i, int j, int bufferId) { }
        public void OnIndexRead(int index, int bufferId) { }
        public void OnIndexWrite(int index, int bufferId) { }
        public void OnIndexWrite<T>(int index, int bufferId, T value) { }
        public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId) { }
        public void OnRangeCopy<T>(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, ReadOnlySpan<T> values) { }
        public void OnLink(int parentIndex, int childIndex, int bufferId, LinkSide side) { }
        public void OnRole(int index, int bufferId, RoleType role) { }
    }

    /// <summary>
    /// Pairs each extraction's announced node with the value the same step writes back, so the role can be
    /// checked against what actually left the tree.
    /// </summary>
    private sealed class ExtractionRoleContext : ISortContext
    {
        private const int BufferTree = 1;

        private readonly Dictionary<int, int> _nodeValues = [];
        private int _current = -1;

        public List<string> Problems { get; } = [];
        public List<int> ExtractedNodes { get; } = [];
        public List<int> WrittenValues { get; } = [];

        public void OnIndexWrite<T>(int index, int bufferId, T value)
        {
            if (typeof(T) != typeof(int)) return;
            var v = (int)(object)value!;

            if (bufferId == BufferTree)
            {
                _nodeValues[index] = v;
                return;
            }

            if (bufferId != 0) return;
            if (_current < 0)
            {
                Problems.Add($"write of {v} to index {index} happened with no node announced as the current minimum");
                return;
            }
            if (!_nodeValues.TryGetValue(_current, out var nodeValue) || nodeValue != v)
                Problems.Add($"node {_current} was announced as the current minimum but the write carried {v}, not {nodeValue}");

            ExtractedNodes.Add(_current);
            WrittenValues.Add(v);
        }

        public void OnRole(int index, int bufferId, RoleType role)
        {
            if (bufferId != BufferTree) return;
            switch (role)
            {
                case RoleType.CurrentMin:
                    if (_current >= 0) Problems.Add($"node {index} was announced while node {_current} still held the role");
                    _current = index;
                    break;
                case RoleType.None:
                    if (_current != index) Problems.Add($"role cleared on node {index} while node {_current} held it");
                    _current = -1;
                    break;
            }
        }

        public void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ) { }
        public void OnSwap(int i, int j, int bufferId) { }
        public void OnIndexRead(int index, int bufferId) { }
        public void OnIndexWrite(int index, int bufferId) { }
        public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId) { }
        public void OnRangeCopy<T>(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, ReadOnlySpan<T> values) { }
        public void OnLink(int parentIndex, int childIndex, int bufferId, LinkSide side) { }
        public void OnPhase(SortPhase phase, int param1 = 0, int param2 = 0, int param3 = 0) { }
    }

    private static int[] PipeOrgan(int n)
        => Enumerable.Range(0, n).Select(i => i < (n + 1) / 2 ? i : n - 1 - i).ToArray();

    // Descending comparer, to check the algorithm never assumes an ascending comparer.
    private sealed class DescendingComparer : IComparer<int>
    {
        public int Compare(int x, int y) => y.CompareTo(x);
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
        CartesianTreeSort.Sort(sorted.AsSpan(), stats);

        // Sorted input builds the right spine: every element stops the pop loop on its first test, so
        // construction costs exactly n-1 comparisons, and the tree is a path, so the priority queue never
        // holds two nodes and the extraction compares nothing at all.
        await Assert.That(stats.CompareCount).IsEqualTo((ulong)(n - 1));

        // Reads: n main + (n-1) comparison reads + 3n extraction reads (Value, Left, Right per node).
        await Assert.That(stats.IndexReadCount).IsEqualTo((ulong)(5 * n - 1));

        // Writes: n node creations + (n-1) right-child links + n write-backs.
        await Assert.That(stats.IndexWriteCount).IsEqualTo((ulong)(3 * n - 1));

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
        CartesianTreeSort.Sort(reversed.AsSpan(), stats);

        // Reverse-sorted input builds the left spine: each element pops the single node on the stack and
        // then finds it empty, so construction again costs exactly n-1 comparisons and the tree is a path.
        // Descending input is as cheap as ascending input here, which is what separates this sort from the
        // search-tree sorts, where reverse order is the degenerate case.
        await Assert.That(stats.CompareCount).IsEqualTo((ulong)(n - 1));
        await Assert.That(stats.IndexReadCount).IsEqualTo((ulong)(5 * n - 1));

        // Writes: n node creations + (n-1) left-child links + n write-backs. The root moves n times but a
        // root promotion is a link event only, so the count matches the sorted case.
        await Assert.That(stats.IndexWriteCount).IsEqualTo((ulong)(3 * n - 1));

        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

    [Test]
    [Arguments(10, 42)]
    [Arguments(20, 42)]
    [Arguments(50, 42)]
    [Arguments(100, 42)]
    [Arguments(100, 1234)]
    [Arguments(512, 42)]
    [Arguments(512, 1234)]
    public async Task TheoreticalValuesRandomTest(int n, int seed)
    {
        var stats = new StatisticsContext();
        var random = TestHelpers.ShuffledRange(n, seed);
        CartesianTreeSort.Sort(random.AsSpan(), stats);

        // Construction pushes and pops each element at most once and adds one failed test per element,
        // so it stays below 2n comparisons for every input. Extraction performs n pops and n pushes on a
        // queue of at most n nodes, each costing at most 2*log2(n) comparisons.
        var buildBound = (ulong)(2 * n);
        var extractBound = (ulong)(4 * n * Math.Log2(Math.Max(n, 2))) + 1;
        await Assert.That(stats.CompareCount).IsGreaterThanOrEqualTo((ulong)(n - 1));
        await Assert.That(stats.CompareCount).IsLessThanOrEqualTo(buildBound + extractBound);

        // Reads: n main + one per comparison operand + 3n during extraction. Construction compares against
        // one node (1 read per comparison), extraction compares two nodes (2 reads per comparison), so the
        // total is bounded by the comparison count either way.
        var minReads = (ulong)(4 * n) + stats.CompareCount;
        await Assert.That(stats.IndexReadCount).IsGreaterThanOrEqualTo(minReads);
        await Assert.That(stats.IndexReadCount).IsLessThanOrEqualTo((ulong)(4 * n) + 2 * stats.CompareCount);

        // Writes: n node creations + n write-backs + the child-pointer writes. An element writes its own
        // Left slot when it adopts a popped subtree and its parent's Right slot when the spine is non-empty,
        // and both can happen in the same step, so the link writes range from n-1 (every step does exactly
        // one, as in the two spine cases above) to 2n-2 (every step does both).
        await Assert.That(stats.IndexWriteCount).IsGreaterThanOrEqualTo((ulong)(3 * n - 1));
        await Assert.That(stats.IndexWriteCount).IsLessThanOrEqualTo((ulong)(4 * n - 2));

        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

    /// <summary>
    /// The whole point of building the tree from a stack rather than by insertion is that construction is
    /// linear no matter what the input looks like. Only the extraction is allowed to grow with n log n.
    /// </summary>
    [Test]
    [Arguments("sorted")]
    [Arguments("reversed")]
    [Arguments("random")]
    [Arguments("pipeOrgan")]
    [Arguments("duplicates")]
    public async Task ConstructionIsLinearForEveryPattern(string pattern)
    {
        const int n = 1000;
        var input = pattern switch
        {
            "sorted" => Enumerable.Range(0, n).ToArray(),
            "reversed" => Enumerable.Range(0, n).Reverse().ToArray(),
            "random" => TestHelpers.ShuffledRange(n, 20260802),
            "pipeOrgan" => PipeOrgan(n),
            _ => Enumerable.Range(0, n).Select(i => i % 5).ToArray(),
        };

        var context = new PhaseCompareCountingContext();
        var array = input.ToArray();
        CartesianTreeSort.Sort(array.AsSpan(), context);

        var expected = input.ToArray();
        Array.Sort(expected);
        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);

        // Every element is announced once during the build and extracted once.
        await Assert.That(context.BuildPhaseCount).IsEqualTo(n);
        await Assert.That(context.ExtractPhaseCount).IsEqualTo(n);

        // Each element is pushed once, popped at most once, and costs at most one failed pop test.
        await Assert.That(context.BuildCompares).IsLessThan((ulong)(2 * n));

        // No comparison may happen outside the two phases; one that does would be invisible to a consumer
        // trying to attribute work to a stage.
        await Assert.That(context.OtherCompares).IsEqualTo(0UL);
    }

    /// <summary>
    /// Presortedness is spent entirely in the extraction: an input whose Cartesian tree is a path keeps the
    /// priority queue at a single node, so extraction compares nothing. This is the adaptive claim, and it
    /// is invisible in the total comparison count, which the linear build dominates for these inputs.
    /// </summary>
    [Test]
    [Arguments("sorted")]
    [Arguments("reversed")]
    public async Task ExtractionIsFreeWhenTheTreeIsAPath(string pattern)
    {
        const int n = 512;
        var input = pattern == "sorted"
            ? Enumerable.Range(0, n).ToArray()
            : Enumerable.Range(0, n).Reverse().ToArray();

        var context = new PhaseCompareCountingContext();
        CartesianTreeSort.Sort(input.AsSpan(), context);

        await Assert.That(context.ExtractCompares).IsEqualTo(0UL);
    }

    /// <summary>
    /// A bushy tree must cost more in the extraction than a path does, otherwise the adaptivity above is an
    /// artifact of the measurement rather than a property of the algorithm.
    /// </summary>
    [Test]
    public async Task ExtractionGrowsWhenTheTreeIsBushy()
    {
        const int n = 512;
        var context = new PhaseCompareCountingContext();
        CartesianTreeSort.Sort(TestHelpers.ShuffledRange(n, 20260803).AsSpan(), context);

        await Assert.That(context.ExtractCompares).IsGreaterThan((ulong)n);
    }

    /// <summary>
    /// The extraction order is decided inside the priority queue, so it is the one thing about that phase an
    /// observer cannot derive from the tree. It is published as a role on the tree buffer; without it the only
    /// way to recover the order is to assume the tree node read just before each write-back is the extracted
    /// one, which is a guess about statement order rather than a contract.
    /// </summary>
    [Test]
    [Arguments("sorted")]
    [Arguments("reversed")]
    [Arguments("random")]
    [Arguments("pipeOrgan")]
    [Arguments("duplicates")]
    public async Task EachExtractionAnnouncesTheNodeItEmits(string pattern)
    {
        const int n = 256;
        var input = pattern switch
        {
            "sorted" => Enumerable.Range(0, n).ToArray(),
            "reversed" => Enumerable.Range(0, n).Reverse().ToArray(),
            "random" => TestHelpers.ShuffledRange(n, 20260805),
            "pipeOrgan" => PipeOrgan(n),
            _ => Enumerable.Range(0, n).Select(i => i % 5).ToArray(),
        };

        var context = new ExtractionRoleContext();
        var array = input.ToArray();
        CartesianTreeSort.Sort(array.AsSpan(), context);

        await Assert.That(context.Problems).IsEmpty();

        // Every element leaves the tree exactly once, and the announced nodes are all distinct.
        await Assert.That(context.ExtractedNodes.Count).IsEqualTo(n);
        await Assert.That(context.ExtractedNodes.Distinct().Count()).IsEqualTo(n);

        // The announced sequence is the sorted output, not the arena order.
        var expected = input.ToArray();
        Array.Sort(expected);
        await Assert.That(context.WrittenValues).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    /// <summary>
    /// Stability with heavy duplicates over several seeds. The base suite covers the canonical cases; this
    /// pushes many equal keys through the priority queue at once, which is where the arena-index tie-break
    /// is the only thing keeping equal elements in input order.
    /// </summary>
    [Test]
    [Arguments(64, 3, 42)]
    [Arguments(64, 3, 1234)]
    [Arguments(256, 4, 42)]
    [Arguments(256, 4, 1234)]
    [Arguments(256, 16, 987654)]
    [Arguments(1000, 2, 42)]
    public async Task StabilityWithManyDuplicatesTest(int n, int distinctKeys, int seed)
    {
        var random = new Random(seed);
        var items = Enumerable.Range(0, n)
            .Select(i => new StabilityTestItem(random.Next(distinctKeys), i))
            .ToArray();

        CartesianTreeSort.Sort(items.AsSpan(), new StatisticsContext());

        var expected = items.OrderBy(x => x.Value).ThenBy(x => x.OriginalIndex).ToArray();
        for (var i = 0; i < items.Length; i++)
        {
            await Assert.That(items[i].Value).IsEqualTo(expected[i].Value);
            await Assert.That(items[i].OriginalIndex).IsEqualTo(expected[i].OriginalIndex);
        }
    }

    /// <summary>
    /// The pop loop compares the incoming element against the spine, so a comparer with the opposite
    /// ordering must reverse the tree and the output rather than degrade or misorder it.
    /// </summary>
    [Test]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(17)]
    [Arguments(256)]
    public async Task CustomComparerTest(int n)
    {
        var array = TestHelpers.ShuffledRange(n, 20260804);
        var expected = array.OrderByDescending(x => x).ToArray();

        CartesianTreeSort.Sort(array.AsSpan(), new DescendingComparer(), new StatisticsContext());

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    /// <summary>
    /// The construction stack and the extraction priority queue share one rented buffer of length n, so an
    /// input that makes the queue as wide as possible must still fit. A pipe organ maximizes the branching.
    /// </summary>
    [Test]
    [Arguments(3)]
    [Arguments(64)]
    [Arguments(1001)]
    public async Task WideTreeFitsTheSharedScratchBufferTest(int n)
    {
        var array = PipeOrgan(n);
        var expected = array.ToArray();
        Array.Sort(expected);

        CartesianTreeSort.Sort(array.AsSpan(), new StatisticsContext());

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }
}
