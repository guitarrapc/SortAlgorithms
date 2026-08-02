using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

/// <summary>
/// Verifies the structural contract published through <see cref="ISortContext.OnLink"/>.
///
/// <para>
/// Tree sorts are the only algorithms whose result is not observable from index operations alone:
/// an observer sees that a node was written but not which edge was created, so without link events it
/// has to reimplement the insertion and rebalancing rules to know the shape. The link events exist so
/// that no observer ever has to do that — which only holds if every child-pointer write is reported.
/// A single missing link (a rotation branch, a root promotion) leaves an observer with a tree that is
/// still plausible but no longer the one the algorithm built.
/// </para>
///
/// <para>
/// The check is end-to-end: replay the links into an independent tree and require that its in-order
/// traversal equals the sorted output. Since the algorithms produce their output by traversing their
/// own tree in order, a rebuilt tree that traverses to the same sequence is the same tree.
/// </para>
/// </summary>
public class TreeLinkEventTests
{
    private const int TreeBufferId = 1;

    /// <summary>Rebuilds a tree from link events, knowing nothing about how the algorithm builds it.</summary>
    private sealed class LinkReplayContext : ISortContext
    {
        private readonly int[] _value;
        private readonly int[] _left;
        private readonly int[] _right;
        private readonly int[] _parent;
        private int _size;
        private int _root = -1;

        public LinkReplayContext(int capacity)
        {
            _value = new int[capacity];
            _left = new int[capacity];
            _right = new int[capacity];
            _parent = new int[capacity];
            Array.Fill(_left, -1);
            Array.Fill(_right, -1);
            Array.Fill(_parent, -1);
        }

        public string? Error { get; private set; }

        public int Size => _size;

        public int Root => _root;

        public void OnIndexWrite<T>(int index, int bufferId, T value)
        {
            if (bufferId != TreeBufferId || Error is not null) return;
            if (typeof(T) != typeof(int)) return;

            if (index != _size)
            {
                Error = $"node {index} was created out of arena order (expected {_size})";
                return;
            }
            _value[index] = (int)(object)value!;
            _size++;
        }

        public void OnLink(int parentIndex, int childIndex, int bufferId, LinkSide side)
        {
            if (bufferId != TreeBufferId || Error is not null) return;

            if (parentIndex == -1)
            {
                if (childIndex < 0 || childIndex >= _size) { Error = $"root link to unknown node {childIndex}"; return; }
                _root = childIndex;
                _parent[childIndex] = -1;
                return;
            }

            if (parentIndex < 0 || parentIndex >= _size) { Error = $"link from unknown node {parentIndex}"; return; }

            switch (side)
            {
                case LinkSide.Left: _left[parentIndex] = childIndex; break;
                case LinkSide.Right: _right[parentIndex] = childIndex; break;
                default: Error = $"link to node {parentIndex} without a child slot"; return;
            }

            if (childIndex == -1) return; // clearing a slot is legal (a rotation detaching a subtree)
            if (childIndex >= _size) { Error = $"link to unknown node {childIndex}"; return; }
            _parent[childIndex] = parentIndex;
        }

        /// <summary>In-order traversal of the rebuilt tree.</summary>
        public List<int> Inorder()
        {
            var result = new List<int>(_size);
            if (_root < 0) return result;

            var stack = new Stack<int>();
            var current = _root;
            var guard = _size + 1;
            while ((stack.Count > 0 || current != -1) && guard-- > 0)
            {
                while (current != -1) { stack.Push(current); current = _left[current]; }
                current = stack.Pop();
                result.Add(_value[current]);
                current = _right[current];
            }
            return result;
        }

        /// <summary>Nodes that are neither the root nor a child of any node.</summary>
        public int OrphanCount()
            => Enumerable.Range(0, _size).Count(i => i != _root && _parent[i] < 0);

        /// <summary>Parent-child pairs that violate min-heap order on values.</summary>
        public int MinHeapViolationCount()
            => Enumerable.Range(0, _size).Count(i =>
                (_left[i] != -1 && _value[_left[i]] < _value[i]) ||
                (_right[i] != -1 && _value[_right[i]] < _value[i]));

        // The shape is fully described by node creation and link events; nothing else is observed.
        public void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ) { }
        public void OnSwap(int i, int j, int bufferId) { }
        public void OnIndexRead(int index, int bufferId) { }
        public void OnIndexWrite(int index, int bufferId) { }
        public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId) { }
        public void OnRangeCopy<T>(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, ReadOnlySpan<T> values) { }
        public void OnPhase(SortPhase phase, int param1 = 0, int param2 = 0, int param3 = 0) { }
        public void OnRole(int index, int bufferId, RoleType role) { }
    }

    private delegate void SortAction(Span<int> span, LinkReplayContext context);

    private static readonly (string Name, SortAction Sort)[] TreeSorts =
    [
        ("BinaryTreeSort", (span, ctx) => BinaryTreeSort.Sort(span, ctx)),
        ("BalancedBinaryTreeSort", (span, ctx) => BalancedBinaryTreeSort.Sort(span, ctx)),
        ("SplaySort", (span, ctx) => SplaySort.Sort(span, ctx)),
        ("TreapSort", (span, ctx) => TreapSort.Sort(span, ctx)),
    ];

    private static int[] Shuffled(int n)
    {
        var rnd = new Random(20260801);
        var a = new int[n];
        for (var i = 0; i < n; i++) a[i] = i + 1;
        for (var i = n - 1; i > 0; i--) { var j = rnd.Next(i + 1); (a[i], a[j]) = (a[j], a[i]); }
        return a;
    }

    private static (int[] Source, string Name)[] Patterns(int n)
    {
        var sorted = Enumerable.Range(1, n).ToArray();
        var reversed = Enumerable.Range(1, n).Reverse().ToArray();
        var rnd = new Random(20260802);
        var fewUnique = Enumerable.Range(0, n).Select(_ => rnd.Next(1, 5)).ToArray();
        return [(Shuffled(n), "shuffled"), (sorted, "sorted"), (reversed, "reversed"), (fewUnique, "fewUnique")];
    }

    [Test]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(17)]
    [Arguments(128)]
    public async Task LinkEventsDescribeTheTreeTheAlgorithmTraversed(int n)
    {
        var problems = new List<string>();

        foreach (var (name, sort) in TreeSorts)
        {
            foreach (var (source, pattern) in Patterns(n))
            {
                var span = source.ToArray();
                var context = new LinkReplayContext(source.Length);
                sort(span.AsSpan(), context);

                if (context.Error is not null)
                {
                    problems.Add($"{name} ({pattern}, n={n}): {context.Error}");
                    continue;
                }

                // A single element is already sorted, so the algorithms return before building a tree.
                if (source.Length <= 1)
                {
                    if (context.Size != 0)
                        problems.Add($"{name} ({pattern}, n={n}): built a tree for an input that needs no sorting");
                    continue;
                }

                if (context.Size != source.Length)
                    problems.Add($"{name} ({pattern}, n={n}): rebuilt {context.Size} nodes, expected {source.Length}");
                if (context.OrphanCount() > 0)
                    problems.Add($"{name} ({pattern}, n={n}): {context.OrphanCount()} node(s) never linked into the tree");

                // The algorithm writes its output by traversing its own tree in order, so the sorted
                // span is exactly the in-order sequence of the tree that was actually built.
                if (!context.Inorder().SequenceEqual(span))
                    problems.Add($"{name} ({pattern}, n={n}): rebuilt tree traverses to a different sequence than the sort produced");
            }
        }

        await Assert.That(problems).IsEmpty()
            .Because($"""
                A tree sort is not reporting every child-pointer write through ISortContext.OnLink,
                so an observer cannot reconstruct the tree without reimplementing the algorithm.
                Check the rotation branches and the root promotion for a missing OnLink:
                {string.Join("\n", problems)}
                """);
    }

    /// <summary>
    /// <see cref="CartesianTreeSort"/> publishes the same link contract but its tree is not a search tree,
    /// so it is checked against a different invariant and cannot join the list above. Its in-order sequence
    /// is the <em>input</em> order, and the sorted output comes from a priority queue walking that tree — so
    /// asserting "in-order equals the sorted span" would be asserting the wrong thing, and would pass only
    /// on inputs that are already sorted.
    ///
    /// <para>
    /// The two properties checked here are exactly the ones that define a Cartesian tree, and together they
    /// pin the shape down completely: in-order equals the input, and every node is no greater than its
    /// children. An observer that replays the links gets the tree the algorithm actually built only if both
    /// survive, and a missing link on the pop path breaks the first while leaving the second intact.
    /// </para>
    /// </summary>
    [Test]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(17)]
    [Arguments(128)]
    public async Task CartesianTreeLinkEventsDescribeTheTreeBuiltFromTheInput(int n)
    {
        var problems = new List<string>();

        foreach (var (source, pattern) in Patterns(n))
        {
            var span = source.ToArray();
            var context = new LinkReplayContext(source.Length);
            CartesianTreeSort.Sort(span.AsSpan(), context);

            if (context.Error is not null)
            {
                problems.Add($"({pattern}, n={n}): {context.Error}");
                continue;
            }

            // A single element is already sorted, so the algorithm returns before building a tree.
            if (source.Length <= 1)
            {
                if (context.Size != 0)
                    problems.Add($"({pattern}, n={n}): built a tree for an input that needs no sorting");
                continue;
            }

            if (context.Size != source.Length)
                problems.Add($"({pattern}, n={n}): rebuilt {context.Size} nodes, expected {source.Length}");
            if (context.OrphanCount() > 0)
                problems.Add($"({pattern}, n={n}): {context.OrphanCount()} node(s) never linked into the tree");
            if (!context.Inorder().SequenceEqual(source))
                problems.Add($"({pattern}, n={n}): rebuilt tree traverses to [{string.Join(", ", context.Inorder())}], expected the input order");
            if (context.MinHeapViolationCount() > 0)
                problems.Add($"({pattern}, n={n}): {context.MinHeapViolationCount()} node(s) hold a value greater than a child's");
        }

        await Assert.That(problems).IsEmpty()
            .Because($"""
                CartesianTreeSort is not reporting every child-pointer write through ISortContext.OnLink,
                so an observer cannot reconstruct the tree without reimplementing the construction.
                Check the pop path (left-child adoption) and the root promotion for a missing OnLink:
                {string.Join("\n", problems)}
                """);
    }

    /// <summary>
    /// Link events describe a write that is still announced through
    /// <see cref="ISortContext.OnIndexWrite(int, int)"/> at the same call site, so counting them here
    /// as well would double-count every tree pointer write and silently change the published
    /// statistics of every tree sort.
    /// </summary>
    [Test]
    public async Task StatisticsIgnoreLinkEvents()
    {
        var stats = new StatisticsContext();

        stats.OnLink(0, 1, TreeBufferId, LinkSide.Left);
        stats.OnLink(2, -1, TreeBufferId, LinkSide.Right);
        stats.OnLink(-1, 3, TreeBufferId, LinkSide.None);

        await Assert.That(stats.CompareCount).IsEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexReadCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
    }

    /// <summary>
    /// A composite observer must receive link events; forwarding them is easy to forget because the
    /// interface carries a no-op default implementation for source compatibility.
    /// </summary>
    [Test]
    public async Task CompositeContextForwardsLinkEvents()
    {
        var replay = new LinkReplayContext(8);
        var span = Shuffled(8);
        BinaryTreeSort.Sort(span.AsSpan(), new CompositeContext(new StatisticsContext(), replay));

        await Assert.That(replay.Error).IsNull();
        await Assert.That(replay.Size).IsEqualTo(8);
        await Assert.That(replay.Inorder()).IsEquivalentTo(span);
    }
}
