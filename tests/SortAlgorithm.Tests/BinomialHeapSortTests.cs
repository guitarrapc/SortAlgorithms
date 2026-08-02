using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class BinomialHeapSortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => BinomialHeapSort.Sort(span, context);

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
        BinomialHeapSort.Sort(sorted.AsSpan(), stats);

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
        BinomialHeapSort.Sort(reversed.AsSpan(), stats);

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
        BinomialHeapSort.Sort(random.AsSpan(), stats);

        await AssertOperationCounts(stats, n);
        await Assert.That(random).IsEquivalentTo(Enumerable.Range(0, n).ToArray(), CollectionOrdering.Matching);
    }

    /// <summary>
    /// The heap bounds every operation by the root list, which never exceeds ⌊log₂ n⌋ + 1 trees regardless of
    /// input order. The point of these bounds is that the same ones hold for sorted, reversed and random input:
    /// a binomial heap has no adversarial pattern, unlike the unbalanced search tree it is often compared to.
    /// </summary>
    private static async Task AssertOperationCounts(StatisticsContext stats, int n)
    {
        var nLogN = n * Math.Log2(Math.Max(n, 2));

        // A comparison sort needs at least n-1 comparisons; extraction costs O(log n) each.
        await Assert.That(stats.CompareCount).IsBetween((ulong)(n - 1), (ulong)(nLogN * 4) + 10);

        // Reads: n from the input plus the pointer walks over the heap buffer (degrees, siblings, children, values).
        await Assert.That(stats.IndexReadCount).IsBetween((ulong)n, (ulong)(nLogN * 30) + 10);

        // Writes: n node creations plus n write-backs, plus the pointer updates union and extraction perform.
        await Assert.That(stats.IndexWriteCount).IsBetween((ulong)(2 * n), (ulong)(nLogN * 10) + 10);

        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

    /// <summary>
    /// Pins the reordering that <c>IsStable => false</c> declares, using the counterexample the type's summary
    /// names. Nothing else in the suite can see it: a wrong stability claim still sorts perfectly, and
    /// <see cref="StabilityDeclarationTests"/> only checks that the claim agrees with the base class chosen here.
    /// If a future change makes equal keys keep their order, this test fails and says which of the two tie-breaks
    /// — the link in union, the scan in extraction — has started consulting insertion order.
    /// </summary>
    [Test]
    public async Task EqualKeysAreReorderedByRootListPosition()
    {
        var items = new[]
        {
            new StabilityTestItem(2, 0),
            new StabilityTestItem(1, 1),
            new StabilityTestItem(1, 2),
        };

        BinomialHeapSort.Sort(items.AsSpan(), new StatisticsContext());

        // Sorting is still correct.
        await Assert.That(items.Select(x => x.Value).ToArray()).IsEquivalentTo([1, 1, 2], CollectionOrdering.Matching);

        // The first 1 inserted becomes the root of the degree-1 tree; the second stays a degree-0 root at the
        // front of the root list, so extraction reaches it first.
        await Assert.That(items.Select(x => x.OriginalIndex).ToArray()).IsEquivalentTo([2, 1, 0], CollectionOrdering.Matching);
    }

    /// <summary>
    /// Verifies the structure the algorithm actually built, rebuilt from the events it published and nothing else.
    ///
    /// <para>
    /// Every other test here can only see the sorted output, which a plain heap — or a merge sort — would produce
    /// just as well. What makes this a binomial heap is the shape: distinct root degrees, a tree of degree k holding
    /// exactly 2^k nodes with children of degrees k-1..0, and min-heap order throughout. That is also what the
    /// complexity rests on, since it is the distinct-degree invariant that bounds the root list at ⌊log₂ n⌋ + 1 and
    /// therefore bounds every extraction. A union that failed to carry correctly would leave duplicate degrees, a
    /// longer root list, and a slower sort that still returns the right answer.
    /// </para>
    /// </summary>
    [Test]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(3)]
    [Arguments(7)]
    [Arguments(8)]
    [Arguments(17)]
    [Arguments(100)]
    [Arguments(128)]
    public async Task BuiltForestIsABinomialHeap(int n)
    {
        var problems = new List<string>();

        foreach (var (source, pattern) in Patterns(n))
        {
            var span = source.ToArray();
            var forest = new ForestReplayContext(n);
            BinomialHeapSort.Sort(span.AsSpan(), forest);

            // A single element is already sorted, so the algorithm returns before building a heap.
            if (n <= 1)
            {
                if (forest.NodeCount != 0) problems.Add($"({pattern}, n={n}): built a heap for an input that needs no sorting");
                continue;
            }

            foreach (var problem in forest.Validate(n))
            {
                problems.Add($"({pattern}, n={n}): {problem}");
            }
        }

        await Assert.That(problems).IsEmpty()
            .Because($"""
                The forest rebuilt from the published node creations and link events is not a binomial heap.
                Check the carry loop in Union (duplicate degrees), Link (child order), and the child-list
                reversal in ExtractMin:
                {string.Join("\n", problems)}
                """);
    }

    private static (int[] Source, string Name)[] Patterns(int n)
    {
        var rnd = new Random(20260802);
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
    /// Rebuilds the binomial forest from node creations and link events, knowing nothing about how the
    /// algorithm builds it. Recording stops when the extraction phase starts, so what is validated is the
    /// completed heap rather than a half-dismantled one.
    /// </summary>
    private sealed class ForestReplayContext : ISortContext
    {
        private const int HeapBufferId = 1;
        private const int NullIndex = -1;

        private readonly int[] _value;
        private readonly int[] _child;
        private readonly int[] _sibling;
        private int _size;
        private int _head = NullIndex;
        private bool _frozen;

        public ForestReplayContext(int capacity)
        {
            _value = new int[Math.Max(capacity, 1)];
            _child = new int[Math.Max(capacity, 1)];
            _sibling = new int[Math.Max(capacity, 1)];
            Array.Fill(_child, NullIndex);
            Array.Fill(_sibling, NullIndex);
        }

        public string? Error { get; private set; }

        public int NodeCount => _size;

        public void OnPhase(SortPhase phase, int param1 = 0, int param2 = 0, int param3 = 0)
        {
            // The heap is complete the moment the first extraction is announced.
            if (phase == SortPhase.BinomialHeapExtract) _frozen = true;
        }

        public void OnIndexWrite<T>(int index, int bufferId, T value)
        {
            if (_frozen || bufferId != HeapBufferId || Error is not null) return;
            if (typeof(T) != typeof(int)) return;

            if (index != _size) { Error = $"node {index} was created out of arena order (expected {_size})"; return; }
            _value[index] = (int)(object)value!;
            _size++;
        }

        public void OnLink(int parentIndex, int childIndex, int bufferId, LinkSide side)
        {
            if (_frozen || bufferId != HeapBufferId || Error is not null) return;

            if (parentIndex == NullIndex)
            {
                if (childIndex < 0 || childIndex >= _size) { Error = $"root-list head set to unknown node {childIndex}"; return; }
                _head = childIndex;
                return;
            }

            if (parentIndex < 0 || parentIndex >= _size) { Error = $"link from unknown node {parentIndex}"; return; }
            if (childIndex >= _size) { Error = $"link to unknown node {childIndex}"; return; }

            switch (side)
            {
                case LinkSide.Left: _child[parentIndex] = childIndex; break;
                case LinkSide.Right: _sibling[parentIndex] = childIndex; break;
                default: Error = $"link to node {parentIndex} without a child slot"; break;
            }
        }

        /// <summary>Checks every property that defines a binomial heap, reporting all violations found.</summary>
        public List<string> Validate(int expectedElements)
        {
            var problems = new List<string>();
            if (Error is not null) return [Error];

            if (_size != expectedElements) problems.Add($"rebuilt {_size} nodes, expected {expectedElements}");
            if (_head == NullIndex) { problems.Add("the root-list head was never reported"); return problems; }

            // Roots: degrees strictly increasing, and one tree per set bit of n.
            var roots = new List<int>();
            var seen = new HashSet<int>();
            for (var r = _head; r != NullIndex; r = _sibling[r])
            {
                if (!seen.Add(r)) { problems.Add("the root list is cyclic"); return problems; }
                roots.Add(r);
            }

            var degrees = roots.Select(Degree).ToArray();
            for (var i = 1; i < degrees.Length; i++)
            {
                if (degrees[i] <= degrees[i - 1])
                    problems.Add($"root degrees are not strictly increasing: [{string.Join(", ", degrees)}]");
            }

            var expectedDegrees = Enumerable.Range(0, 31).Where(k => (expectedElements & (1 << k)) != 0).ToArray();
            if (!degrees.SequenceEqual(expectedDegrees))
                problems.Add($"root degrees [{string.Join(", ", degrees)}] do not match the binary representation of {expectedElements} [{string.Join(", ", expectedDegrees)}]");

            // Trees: each root of degree k is a B_k, and min-heap order holds everywhere.
            var visited = new HashSet<int>();
            foreach (var root in roots)
            {
                CheckBinomialTree(root, Degree(root), visited, problems);
            }

            if (visited.Count != _size)
                problems.Add($"{_size - visited.Count} node(s) are not reachable from the root list");

            return problems;
        }

        private int Degree(int node)
        {
            var degree = 0;
            for (var c = _child[node]; c != NullIndex; c = _sibling[c]) degree++;
            return degree;
        }

        /// <summary>
        /// B_k's root has children of degrees k-1, k-2, ..., 0 in that order, so verifying the child list
        /// degree by degree pins the shape exactly; 2^k nodes then follows by induction.
        /// </summary>
        private void CheckBinomialTree(int node, int expectedDegree, HashSet<int> visited, List<string> problems)
        {
            if (!visited.Add(node)) { problems.Add($"node {node} is reachable more than once"); return; }

            var expectedChildDegree = expectedDegree - 1;
            for (var c = _child[node]; c != NullIndex; c = _sibling[c])
            {
                if (expectedChildDegree < 0)
                {
                    problems.Add($"node {node} has more children than its degree {expectedDegree} allows");
                    return;
                }
                if (_value[c] < _value[node])
                    problems.Add($"min-heap order broken: child {c} ({_value[c]}) is smaller than parent {node} ({_value[node]})");
                if (Degree(c) != expectedChildDegree)
                    problems.Add($"child {c} of node {node} has degree {Degree(c)}, expected {expectedChildDegree}");

                CheckBinomialTree(c, expectedChildDegree, visited, problems);
                expectedChildDegree--;
            }

            if (expectedChildDegree != -1)
                problems.Add($"node {node} has {expectedDegree - 1 - expectedChildDegree} children, expected {expectedDegree}");
        }

        // The shape is fully described by node creation and link events; nothing else is observed.
        public void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ) { }
        public void OnSwap(int i, int j, int bufferId) { }
        public void OnIndexRead(int index, int bufferId) { }
        public void OnIndexWrite(int index, int bufferId) { }
        public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId) { }
        public void OnRangeCopy<T>(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, ReadOnlySpan<T> values) { }
        public void OnRole(int index, int bufferId, RoleType role) { }
    }
}
