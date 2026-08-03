using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

/// <summary>
/// Verifies the structural contract <see cref="BTreeSort"/> and <see cref="BPlusTreeSort"/> publish through
/// <see cref="ISortContext.OnLink"/>.
///
/// <para>
/// These two are multiway trees reported through the two child slots the contract has, using the binary
/// encoding a node of many keys allows: the left slot of a key names the child before it, and its right slot
/// names the next key of the same node — or, for the last key, the child after it. That encoding is a binary
/// tree whose in-order is the multiway tree's in-order, and node membership survives it because a node's key
/// slots are a block of <c>maxKeys</c> consecutive indices. The width is announced as param3 of the insert
/// phase, so this test never hard-codes it: everything below is derived from the stream.
/// </para>
///
/// <para>
/// The checks are the properties that define a B-tree, because those are what an observer cannot recover if a
/// link is missing while the sorted output stays correct: all leaves at one depth, every non-root node between
/// t-1 and 2t-1 keys, and the traversal matching the sorted output. <see cref="BPlusTreeSort"/> is checked
/// against the leaf-only traversal instead, since its internal keys are separator copies and its full in-order
/// therefore contains more keys than the input has elements — asserting equality there would be asserting the
/// wrong thing, exactly as it would be for <see cref="CartesianTreeSort"/>.
/// </para>
/// </summary>
public class BTreeLinkEventTests
{
    private const int TreeBufferId = 1;

    /// <summary>
    /// Rebuilds a multiway tree from the observation stream, knowing nothing about splitting or descent.
    /// </summary>
    private sealed class MultiwayReplayContext : ISortContext
    {
        private int[] _value = new int[64];
        private int[] _left = new int[64];
        private int[] _right = new int[64];
        private int _maxKeys = -1;
        private int _root = -1;

        public MultiwayReplayContext()
        {
            Array.Fill(_left, -1);
            Array.Fill(_right, -1);
        }

        public string? Error { get; private set; }

        /// <summary>Node capacity, as announced by the algorithm rather than assumed by this test.</summary>
        public int MaxKeys => _maxKeys;

        /// <summary>Minimum degree t, derived from the announced capacity 2t-1.</summary>
        public int MinDegree => (_maxKeys + 1) / 2;

        public int Root => _root;

        public void OnPhase(SortPhase phase, int param1 = 0, int param2 = 0, int param3 = 0)
        {
            if (phase is not (SortPhase.BTreeInsert or SortPhase.BPlusTreeInsert)) return;
            if (_maxKeys < 0) _maxKeys = param3;
            else if (_maxKeys != param3) Error = $"node capacity changed mid-run ({_maxKeys} then {param3})";
        }

        public void OnIndexWrite<T>(int index, int bufferId, T value)
        {
            if (bufferId != TreeBufferId || typeof(T) != typeof(int)) return;
            Grow(index);
            _value[index] = (int)(object)value!;
        }

        public void OnRangeCopy<T>(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, ReadOnlySpan<T> values)
        {
            if (destinationBufferId != TreeBufferId || typeof(T) != typeof(int)) return;
            Grow(destinationIndex + length);
            // Boxing per element is fine here: this replay only ever runs on int inputs in tests.
            for (var i = 0; i < length; i++) _value[destinationIndex + i] = (int)(object)values[i]!;
        }

        public void OnLink(int parentIndex, int childIndex, int bufferId, LinkSide side)
        {
            if (bufferId != TreeBufferId || Error is not null) return;

            if (parentIndex == -1)
            {
                if (childIndex < 0) { Error = $"root link to invalid slot {childIndex}"; return; }
                Grow(childIndex);
                _root = childIndex;
                return;
            }

            Grow(Math.Max(parentIndex, childIndex));
            switch (side)
            {
                case LinkSide.Left: _left[parentIndex] = childIndex; break;
                case LinkSide.Right: _right[parentIndex] = childIndex; break;
                default: Error = $"link from slot {parentIndex} without a child slot"; return;
            }
        }

        private void Grow(int index)
        {
            if (index < _value.Length) return;

            var previous = _value.Length;
            var size = previous;
            while (size <= index) size *= 2;

            Array.Resize(ref _value, size);
            Array.Resize(ref _left, size);
            Array.Resize(ref _right, size);
            for (var i = previous; i < size; i++) { _left[i] = -1; _right[i] = -1; }
        }

        public int KeyValue(int slot) => _value[slot];

        public int Left(int slot) => slot >= 0 && slot < _left.Length ? _left[slot] : -1;

        public int Right(int slot) => slot >= 0 && slot < _right.Length ? _right[slot] : -1;

        /// <summary>
        /// How many keys the node starting at <paramref name="nodeBase"/> holds, read off the right chain: a
        /// key's right slot names the next key of the same node, and the last one names a child or nothing.
        /// The block width terminates the walk, so a child that happens to start at the next slot cannot be
        /// mistaken for a sibling key.
        /// </summary>
        public int KeyCount(int nodeBase)
        {
            var count = 1;
            while (count < _maxKeys && Right(nodeBase + count - 1) == nodeBase + count) count++;
            return count;
        }

        public bool IsLeaf(int nodeBase) => Left(nodeBase) == -1;

        /// <summary>In-order traversal of the encoding, returning the key slots in order.</summary>
        public List<int> InorderSlots()
        {
            var result = new List<int>();
            if (_root < 0) return result;

            var stack = new Stack<int>();
            var current = _root;
            var guard = _value.Length + 1;
            while ((stack.Count > 0 || current != -1) && guard-- > 0)
            {
                while (current != -1) { stack.Push(current); current = Left(current); }
                current = stack.Pop();
                result.Add(current);
                current = Right(current);
            }
            return result;
        }

        // Nothing else is needed to describe the shape.
        public void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ) { }
        public void OnSwap(int i, int j, int bufferId) { }
        public void OnIndexRead(int index, int bufferId) { }
        public void OnIndexWrite(int index, int bufferId) { }
        public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId) { }
        public void OnRole(int index, int bufferId, RoleType role) { }
    }

    private static (int[] Source, string Name)[] Patterns(int n)
    {
        var rnd = new Random(20260806);
        var shuffled = Enumerable.Range(1, n).ToArray();
        for (var i = n - 1; i > 0; i--) { var j = rnd.Next(i + 1); (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]); }
        var fewUnique = Enumerable.Range(0, n).Select(_ => rnd.Next(1, 5)).ToArray();
        return
        [
            (shuffled, "shuffled"),
            ([.. Enumerable.Range(1, n)], "sorted"),
            ([.. Enumerable.Range(1, n).Reverse()], "reversed"),
            (fewUnique, "fewUnique"),
        ];
    }

    /// <summary>
    /// Walks the replayed tree and reports every way it fails to be a B-tree of the announced degree.
    /// </summary>
    private static List<string> BTreeShapeProblems(MultiwayReplayContext ctx, string label)
    {
        var problems = new List<string>();
        if (ctx.Root < 0)
        {
            problems.Add($"{label}: no root was ever announced");
            return problems;
        }
        if (ctx.MaxKeys <= 0)
        {
            problems.Add($"{label}: the node capacity was never announced");
            return problems;
        }

        var leafDepths = new HashSet<int>();
        var visited = new HashSet<int>();
        var queue = new Queue<(int NodeBase, int Depth)>();
        queue.Enqueue((ctx.Root, 0));

        while (queue.Count > 0)
        {
            var (nodeBase, depth) = queue.Dequeue();
            if (!visited.Add(nodeBase))
            {
                problems.Add($"{label}: node at slot {nodeBase} is reachable twice, so the structure is not a tree");
                continue;
            }
            if (nodeBase % ctx.MaxKeys != 0)
            {
                problems.Add($"{label}: a child link points at slot {nodeBase}, which is not the start of a node block");
                continue;
            }

            var keys = ctx.KeyCount(nodeBase);
            if (nodeBase != ctx.Root && (keys < ctx.MinDegree - 1 || keys > ctx.MaxKeys))
                problems.Add($"{label}: node at slot {nodeBase} holds {keys} keys, outside [{ctx.MinDegree - 1}, {ctx.MaxKeys}]");
            if (nodeBase == ctx.Root && keys < 1)
                problems.Add($"{label}: the root holds no keys");

            // Keys must be non-decreasing inside a node.
            for (var i = 1; i < keys; i++)
            {
                if (ctx.KeyValue(nodeBase + i - 1) > ctx.KeyValue(nodeBase + i))
                    problems.Add($"{label}: node at slot {nodeBase} holds keys out of order");
            }

            if (ctx.IsLeaf(nodeBase))
            {
                leafDepths.Add(depth);
                continue;
            }

            for (var i = 0; i < keys; i++)
            {
                var child = ctx.Left(nodeBase + i);
                if (child < 0) { problems.Add($"{label}: node at slot {nodeBase} is internal but its key {i} has no left child"); continue; }
                queue.Enqueue((child, depth + 1));
            }
            var last = ctx.Right(nodeBase + keys - 1);
            if (last < 0) problems.Add($"{label}: node at slot {nodeBase} is internal but has no rightmost child");
            else queue.Enqueue((last, depth + 1));
        }

        if (leafDepths.Count > 1)
            problems.Add($"{label}: leaves sit at {leafDepths.Count} different depths ({string.Join(", ", leafDepths.Order())})");

        return problems;
    }

    [Test]
    [Arguments(2)]
    [Arguments(15)]
    [Arguments(16)]
    [Arguments(128)]
    [Arguments(1000)]
    public async Task BTreeLinkEventsDescribeTheTreeItTraversed(int n)
    {
        var problems = new List<string>();

        foreach (var (source, pattern) in Patterns(n))
        {
            var span = source.ToArray();
            var ctx = new MultiwayReplayContext();
            BTreeSort.Sort(span.AsSpan(), ctx);

            var label = $"({pattern}, n={n})";
            if (ctx.Error is not null) { problems.Add($"{label}: {ctx.Error}"); continue; }

            problems.AddRange(BTreeShapeProblems(ctx, label));

            // A B-tree holds every element exactly once, so the in-order of the reported structure is the
            // sorted output the algorithm produced by traversing it.
            var traversal = ctx.InorderSlots().Select(ctx.KeyValue).ToArray();
            if (!traversal.SequenceEqual(span))
                problems.Add($"{label}: the rebuilt tree traverses to a different sequence than the sort produced");
        }

        await Assert.That(problems).IsEmpty()
            .Because($"""
                BTreeSort is not reporting every structural change through ISortContext.OnLink, so an observer
                cannot reconstruct the tree without reimplementing the descent and the split:
                {string.Join("\n", problems)}
                """);
    }

    [Test]
    [Arguments(2)]
    [Arguments(15)]
    [Arguments(16)]
    [Arguments(128)]
    [Arguments(1000)]
    public async Task BPlusTreeLinkEventsDescribeTheTreeItScanned(int n)
    {
        var problems = new List<string>();

        foreach (var (source, pattern) in Patterns(n))
        {
            var span = source.ToArray();
            var ctx = new MultiwayReplayContext();
            BPlusTreeSort.Sort(span.AsSpan(), ctx);

            var label = $"({pattern}, n={n})";
            if (ctx.Error is not null) { problems.Add($"{label}: {ctx.Error}"); continue; }

            problems.AddRange(BTreeShapeProblems(ctx, label));

            // Elements live only in leaves; internal keys are separator copies, so the full in-order carries
            // more keys than the input has elements and only the leaf subsequence can equal the output.
            var slots = ctx.InorderSlots();
            var leafKeys = slots.Where(slot => ctx.IsLeaf(slot / ctx.MaxKeys * ctx.MaxKeys)).Select(ctx.KeyValue).ToArray();
            if (!leafKeys.SequenceEqual(span))
                problems.Add($"{label}: the leaves of the rebuilt tree hold a different sequence than the sort produced");

            var separators = slots.Count - leafKeys.Length;
            if (separators > 0 && n < ctx.MaxKeys)
                problems.Add($"{label}: a tree that fits in one leaf must have no separators, found {separators}");
        }

        await Assert.That(problems).IsEmpty()
            .Because($"""
                BPlusTreeSort is not reporting every structural change through ISortContext.OnLink, so an observer
                cannot reconstruct the tree without reimplementing the descent and the split:
                {string.Join("\n", problems)}
                """);
    }

    /// <summary>
    /// A composite observer must receive the link events; forwarding them is easy to forget because the
    /// interface carries a no-op default implementation for source compatibility.
    /// </summary>
    [Test]
    public async Task CompositeContextForwardsLinkEvents()
    {
        var replay = new MultiwayReplayContext();
        var span = Patterns(64)[0].Source.ToArray();
        BTreeSort.Sort(span.AsSpan(), new CompositeContext(new StatisticsContext(), replay));

        await Assert.That(replay.Error).IsNull();
        await Assert.That(replay.Root).IsGreaterThanOrEqualTo(0);
        await Assert.That(replay.InorderSlots().Select(replay.KeyValue).ToArray()).IsEquivalentTo(span);
    }
}
