using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class PairingHeapSortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => PairingHeapSort.Sort(span, context);

    // Node creation and the write-back pass always write, even for input that is already sorted.
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    // Elements move through heap nodes; array slots are never swapped.
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
        PairingHeapSort.Sort(sorted.AsSpan(), stats);

        await AssertOperationCounts(stats, n);
        await Assert.That(sorted).IsEquivalentTo(Enumerable.Range(0, n).ToArray(), CollectionOrdering.Matching);
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
        PairingHeapSort.Sort(reversed.AsSpan(), stats);

        await AssertOperationCounts(stats, n);
        await Assert.That(reversed).IsEquivalentTo(Enumerable.Range(0, n).ToArray(), CollectionOrdering.Matching);
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
        PairingHeapSort.Sort(random.AsSpan(), stats);

        await AssertOperationCounts(stats, n);
        await Assert.That(random).IsEquivalentTo(Enumerable.Range(0, n).ToArray(), CollectionOrdering.Matching);
    }

    private static async Task AssertOperationCounts(StatisticsContext stats, int n)
    {
        var nLogN = n * Math.Log2(Math.Max(n, 2));

        // One comparison per insert, plus one per meld during the pairing passes.
        await Assert.That(stats.CompareCount).IsBetween((ulong)(n - 1), (ulong)(nLogN * 4) + 10);

        // Reads: n from the input plus the pointer walks over the heap buffer (children, siblings, values).
        await Assert.That(stats.IndexReadCount).IsBetween((ulong)n, (ulong)(nLogN * 20) + 10);

        // Writes: n node creations plus n write-backs, plus the pointer updates the melds perform.
        await Assert.That(stats.IndexWriteCount).IsBetween((ulong)(2 * n), (ulong)(nLogN * 10) + 10);

        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

    /// <summary>
    /// A descending run costs exactly one comparison per element and nothing else.
    ///
    /// <para>
    /// This is the Θ(n) best case the type summary claims, and it is not a loose bound: every insert finds a key
    /// smaller than the current root, so the new element becomes the root and the old one its only child. The heap
    /// ends up a chain, and a chain gives every extraction exactly one child — a single-child pairing pass performs
    /// no comparison at all. Asserting the exact count rather than an order pins both halves: if insertion ever
    /// compared more than once, or if the passes started comparing when there is nothing to pair, this moves.
    /// </para>
    /// </summary>
    [Test]
    [Arguments(2)]
    [Arguments(10)]
    [Arguments(100)]
    [Arguments(1000)]
    public async Task DescendingInputCostsOneComparisonPerElement(int n)
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        PairingHeapSort.Sort(reversed.AsSpan(), stats);

        await Assert.That(stats.CompareCount).IsEqualTo((ulong)(n - 1))
            .Because("each insert takes the new element to the root in one comparison, and every extraction then finds a single child to pair");
        await Assert.That(reversed).IsEquivalentTo(Enumerable.Range(0, n).ToArray(), CollectionOrdering.Matching);
    }

    /// <summary>
    /// Pins the reordering that <c>IsStable => false</c> declares, using the counterexample the type's summary names.
    /// Nothing else in the suite can see it: a wrong stability claim still sorts perfectly, and
    /// <see cref="StabilityDeclarationTests"/> only checks that the claim agrees with the base class chosen here.
    /// </summary>
    [Test]
    public async Task EqualKeysAreReorderedByChildListPosition()
    {
        var items = new[]
        {
            new StabilityTestItem(1, 0),
            new StabilityTestItem(1, 1),
            new StabilityTestItem(1, 2),
        };

        PairingHeapSort.Sort(items.AsSpan(), new StatisticsContext());

        await Assert.That(items.Select(x => x.Value).ToArray()).IsEquivalentTo([1, 1, 1], CollectionOrdering.Matching);

        // Meld keeps the first of an equal pair as the parent and pushes the second onto the front of its child
        // list, so the root ends up with children [1c, 1b] and extraction reaches 1c first.
        await Assert.That(items.Select(x => x.OriginalIndex).ToArray()).IsEquivalentTo([0, 2, 1], CollectionOrdering.Matching);
    }

    /// <summary>
    /// Insertion never restructures: melding a single-node heap either replaces the root or hangs the new node off it.
    ///
    /// <para>
    /// An ascending run makes every new element lose its comparison, so it becomes another child of the same root
    /// and the tree after the build phase is a star of n-1 children. That shape is the whole reason a pairing heap
    /// is cheap to insert into and why the cost has to come back somewhere else — the first extraction then faces
    /// all n-1 children at once. An implementation that quietly kept the tree tidy on insert would fail here while
    /// still sorting correctly.
    /// </para>
    /// </summary>
    [Test]
    [Arguments(8)]
    [Arguments(64)]
    public async Task AscendingInputLeavesTheRootHoldingEveryOtherElement(int n)
    {
        var shape = new HeapShapeContext(n);
        var ascending = Enumerable.Range(1, n).ToArray();
        PairingHeapSort.Sort(ascending.AsSpan(), shape);

        await Assert.That(shape.Error).IsNull();
        await Assert.That(shape.RootDegreeAtBuildEnd).IsEqualTo(n - 1)
            .Because("every element after the first loses its meld against the same root and joins its child list");
    }

    /// <summary>
    /// Verifies the structure the algorithm actually built, rebuilt from the events it published and nothing else.
    ///
    /// <para>
    /// The sorted output alone would look identical for any priority queue, so what is checked here is what makes
    /// this one a heap at all: one root, every element present exactly once, and no node holding a key smaller than
    /// its parent's. The check runs at every extraction boundary, not just at the end, because the two pairing
    /// passes rebuild the tree from scratch each time — a pass that dropped a subtree, or attached one under the
    /// wrong side of a meld, would still produce a heap that yields <em>some</em> ascending sequence for a while.
    /// </para>
    /// </summary>
    [Test]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(3)]
    [Arguments(15)]
    [Arguments(64)]
    public async Task RebuiltTreeIsAMinHeapAtEveryExtraction(int n)
    {
        var problems = new List<string>();

        foreach (var (source, pattern) in Patterns(n))
        {
            var span = source.ToArray();
            var shape = new HeapShapeContext(n);
            PairingHeapSort.Sort(span.AsSpan(), shape);

            if (shape.Error is not null) { problems.Add($"({pattern}, n={n}): {shape.Error}"); continue; }

            // A single element is already sorted, so the algorithm returns before building a heap.
            if (n <= 1)
            {
                if (shape.NodeCount != 0) problems.Add($"({pattern}, n={n}): built a heap for an input that needs no sorting");
                continue;
            }

            foreach (var problem in shape.Problems) problems.Add($"({pattern}, n={n}): {problem}");
        }

        await Assert.That(problems).IsEmpty()
            .Because($"""
                The tree rebuilt from the published node creations and link events is not a valid pairing heap.
                Check the two pairing passes in ExtractMin and the child/sibling writes in Meld:
                {string.Join("\n", problems.Take(8))}
                """);
    }

    private static (int[] Source, string Name)[] Patterns(int n)
    {
        var rnd = new Random(20260803);
        var shuffled = Enumerable.Range(1, n).OrderBy(_ => rnd.Next()).ToArray();
        var fewUnique = Enumerable.Range(0, n).Select(_ => rnd.Next(1, 5)).ToArray();
        return
        [
            (shuffled, "shuffled"),
            (Enumerable.Range(1, n).ToArray(), "sorted"),
            (Enumerable.Range(1, n).Reverse().ToArray(), "reversed"),
            (fewUnique, "fewUnique"),
        ];
    }

    /// <summary>
    /// Rebuilds the pairing heap from node creations and link events, knowing nothing about how the algorithm
    /// builds it, and checks the heap invariant at every extraction boundary.
    /// </summary>
    private sealed class HeapShapeContext : ISortContext
    {
        private const int HeapBufferId = 1;
        private const int NullIndex = -1;

        private readonly int[] _value;
        private readonly int[] _child;
        private readonly int[] _sibling;
        private readonly bool[] _retired;
        private int _size;
        private int _extractions;

        public HeapShapeContext(int capacity)
        {
            _value = new int[Math.Max(capacity, 1)];
            _child = new int[Math.Max(capacity, 1)];
            _sibling = new int[Math.Max(capacity, 1)];
            _retired = new bool[Math.Max(capacity, 1)];
            Array.Fill(_child, NullIndex);
            Array.Fill(_sibling, NullIndex);
        }

        public string? Error { get; private set; }

        public int NodeCount => _size;

        /// <summary>Number of children the root held when the build phase ended.</summary>
        public int RootDegreeAtBuildEnd { get; private set; } = -1;

        public List<string> Problems { get; } = [];

        public void OnPhase(SortPhase phase, int param1 = 0, int param2 = 0, int param3 = 0)
        {
            if (phase != SortPhase.PairingHeapExtract || Error is not null) return;

            // Announced before anything is touched, so the heap is whole here.
            if (RootDegreeAtBuildEnd < 0) RootDegreeAtBuildEnd = Degree(FindRoot());
            Validate(param1 - 1);
            _extractions = param1;
        }

        public void OnIndexWrite<T>(int index, int bufferId, T value)
        {
            if (bufferId != HeapBufferId || Error is not null) return;
            if (typeof(T) != typeof(int)) return;

            if (index != _size) { Error = $"node {index} was created out of arena order (expected {_size})"; return; }
            _value[index] = (int)(object)value!;
            _size++;
        }

        public void OnLink(int parentIndex, int childIndex, int bufferId, LinkSide side)
        {
            if (bufferId != HeapBufferId || Error is not null) return;

            // The root is derived from the links, so the announced root is not needed here.
            if (parentIndex == NullIndex) return;

            if (parentIndex < 0 || parentIndex >= _size) { Error = $"link from unknown node {parentIndex}"; return; }
            if (childIndex >= _size) { Error = $"link to unknown node {childIndex}"; return; }

            if (side == LinkSide.Left) _child[parentIndex] = childIndex;
            else if (side == LinkSide.Right) _sibling[parentIndex] = childIndex;
            else Error = $"link to node {parentIndex} without a child slot";
        }

        public void OnRole(int index, int bufferId, RoleType role)
        {
            // The node named as the current minimum is the one leaving the heap. Which node that is cannot be
            // derived from the tree once it has been detached, so the algorithm's own naming is used.
            if (bufferId == HeapBufferId && role == RoleType.CurrentMin && index >= 0 && index < _size)
                _retired[index] = true;
        }

        /// <summary>Runs at each extraction boundary: the heap must hold every element not yet extracted.</summary>
        private void Validate(int alreadyExtracted)
        {
            var roots = new List<int>();
            for (var i = 0; i < _size; i++)
                if (!_retired[i] && !IsChild(i)) roots.Add(i);

            if (roots.Count != 1)
            {
                Problems.Add($"extraction {alreadyExtracted + 1}: {roots.Count} root(s) — a pairing heap is a single tree");
                return;
            }

            var seen = new HashSet<int>();
            var stack = new Stack<int>();
            stack.Push(roots[0]);
            while (stack.Count > 0)
            {
                var node = stack.Pop();
                if (!seen.Add(node)) { Problems.Add($"extraction {alreadyExtracted + 1}: node {node} is reachable more than once"); return; }

                var guard = _size;
                for (var c = _child[node]; c >= 0 && c < _size && guard-- > 0; c = _sibling[c])
                {
                    if (_value[c] < _value[node])
                        Problems.Add($"extraction {alreadyExtracted + 1}: min-heap order broken (parent {_value[node]} > child {_value[c]})");
                    stack.Push(c);
                }
            }

            var expected = _size - alreadyExtracted;
            if (seen.Count != expected)
                Problems.Add($"extraction {alreadyExtracted + 1}: {seen.Count} node(s) in the heap, expected {expected}");
        }

        private int FindRoot()
        {
            for (var i = 0; i < _size; i++)
                if (!_retired[i] && !IsChild(i)) return i;
            return NullIndex;
        }

        /// <summary>True when the node appears in some live node's child chain.</summary>
        private bool IsChild(int node)
        {
            for (var p = 0; p < _size; p++)
            {
                if (_retired[p]) continue;
                var guard = _size;
                for (var c = _child[p]; c >= 0 && c < _size && guard-- > 0; c = _sibling[c])
                    if (c == node) return true;
            }
            return false;
        }

        private int Degree(int node)
        {
            if (node < 0) return 0;
            var count = 0;
            var guard = _size;
            for (var c = _child[node]; c >= 0 && c < _size && guard-- > 0; c = _sibling[c]) count++;
            return count;
        }

        // The shape is fully described by node creation, link and role events; nothing else is observed.
        public void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ) { }
        public void OnSwap(int i, int j, int bufferId) { }
        public void OnIndexRead(int index, int bufferId) { }
        public void OnIndexWrite(int index, int bufferId) { }
        public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId) { }
        public void OnRangeCopy<T>(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, ReadOnlySpan<T> values) { }
    }
}
