using System.Buffers;
using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// B木（B-tree）を用いたソート。全要素を最小次数 t の B木へ挿入し、中順巡回で配列へ書き戻します。
/// B木の各ノードは t-1 個以上 2t-1 個以下のキーを持ち、葉はすべて同じ深さにあるため、
/// 入力がどんな順序でも木の高さは Θ(log_t n) に収まります。
/// 二分探索木と違い平衡回復のための回転を持たず、代わりに満杯のノードを分割して中央キーを親へ押し上げます。
/// <br/>
/// A sort that inserts every element into a B-tree of minimum degree t and writes the elements back with an
/// in-order traversal. Every node holds between t-1 and 2t-1 keys and every leaf sits at the same depth, so
/// the height is Θ(log_t n) for any input order. There are no rotations: balance is maintained by splitting a
/// full node and pushing its median key into the parent.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct B-Tree Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Search Tree Property:</strong> A node holding keys k₀ ≤ k₁ ≤ ... ≤ k_{m-1} has m+1 children
/// c₀ ... c_m, and every key in c_i lies between k_{i-1} and k_i. In-order — c₀, k₀, c₁, k₁, ..., k_{m-1}, c_m —
/// therefore visits the keys in ascending order.</description></item>
/// <item><description><strong>Node Occupancy:</strong> Every node except the root holds at least t-1 keys and at most
/// 2t-1; the root holds at least one. This is what bounds the height by log_t((n+1)/2) + 1 and makes the
/// bound hold for every input, without any rebalancing step.</description></item>
/// <item><description><strong>Uniform Leaf Depth:</strong> All leaves are at the same depth. A B-tree grows only at the
/// root — the single case where the height increases — which is why an insertion can never lengthen one path
/// alone.</description></item>
/// <item><description><strong>Split Correctness:</strong> Splitting a node of 2t-1 keys leaves t-1 keys on each side and
/// promotes the median into the parent, together with a pointer to the new right node. The in-order sequence
/// is unchanged, so the search tree property survives the split.</description></item>
/// <item><description><strong>Comparison Consistency:</strong> The comparison operation must be consistent and transitive.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Tree</description></item>
/// <item><description>Stable      : Yes (equal keys are inserted to the right of every equal key already present, and splitting preserves in-order; see the stability note below)</description></item>
/// <item><description>In-place    : No (requires O(n) auxiliary space for the nodes)</description></item>
/// <item><description>Best case   : Θ(n log n) - the tree is built by n insertions regardless of input order</description></item>
/// <item><description>Average case: Θ(n log n)</description></item>
/// <item><description>Worst case  : Θ(n log n) - occupancy bounds the height at log_t((n+1)/2) + 1 for every input</description></item>
/// <item><description>Comparisons : Θ(n log n) - searching a node of m keys by binary search costs ⌈log₂(m+1)⌉, and the descent visits Θ(log_t n) nodes, so the base t cancels</description></item>
/// <item><description>Swaps       : 0 - elements move through node slots, never by swapping array slots</description></item>
/// <item><description>Index Reads : Θ(n) main + Θ(n log n) tree</description></item>
/// <item><description>Index Writes: Θ(n) main + O(n t) tree - each insertion shifts at most 2t-2 keys and each split moves t-1</description></item>
/// <item><description>Space       : O(n) - at most n/(t-1) + 2 nodes, rented from <see cref="System.Buffers.ArrayPool{T}"/></description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description><strong>Minimum degree:</strong> t = <see cref="MinDegree"/>, so a node holds up to <see cref="MaxKeys"/> keys.
/// The theory leaves t free, and it does not affect the comparison count: a larger node costs more comparisons per
/// node and proportionally fewer nodes on the path. What it does change is memory traffic, and the two costs pull in
/// opposite directions — a larger node means fewer cache misses on the descent but more keys to shift when one is
/// inserted. This value keeps a node's keys in the region of a cache line for the primitive element types the
/// benchmarks use, while keeping the shift at a handful of elements.</description></item>
/// <item><description><strong>Proactive splitting:</strong> the descent splits any full node it is about to enter, so an
/// insertion is a single pass down. Splitting after the fact would need either parent pointers or a second pass
/// back up the path; neither is required here, and neither appears in the operation stream.</description></item>
/// <item><description><strong>Binary search inside a node:</strong> the insertion position within a node is found by an
/// upper-bound binary search rather than a linear scan. Without it the comparison count would be Θ(n t log_t n),
/// which for a large t is far above the Θ(n log n) the algorithm is supposed to achieve.</description></item>
/// <item><description><strong>Layout:</strong> keys live in one flat array rented from <see cref="System.Buffers.ArrayPool{T}"/>,
/// with node <c>j</c> owning the <see cref="MaxKeys"/> consecutive slots starting at <c>j * MaxKeys</c>; child
/// pointers live in a parallel int array. Nothing is allocated per node, and making room for a key is a single
/// <see cref="Span{T}.CopyTo"/> rather than an element-by-element loop.</description></item>
/// </list>
/// <para><strong>What the observation stream describes:</strong></para>
/// <list type="bullet">
/// <item><description>Buffer 1 is the flat key array. A key write, a key read and the block moves performed by an
/// insertion or a split are all reported against it, so an observer sees keys physically move between nodes, which
/// is what a B-tree does. Slots are reused: after a split the upper half of a node's block is stale until later
/// insertions overwrite it, exactly as in any auxiliary buffer.</description></item>
/// <item><description>The shape is published through <see cref="ISortContext.OnLink"/> in the binary encoding a
/// multiway node has: the left slot of a key names the child that lies before it, and the right slot names the next
/// key of the same node, or — for the last key — the child that lies after it. In-order over that encoding is exactly
/// the in-order of the B-tree, so an observer that replays the links and traverses them gets the sorted sequence
/// without knowing what a split is. Node membership is recoverable because a node's key slots are a block of
/// <see cref="MaxKeys"/> consecutive indices, and that width is announced as param3 of
/// <see cref="SortPhase.BTreeInsert"/> rather than left for a consumer to hard-code.</description></item>
/// <item><description>A node's links are republished whenever its key set or child set changes, rather than one link
/// per pointer write. Link events are free for a counting context — the writes themselves are reported separately —
/// and republishing spares a consumer from deriving which keys are still adjacent after a block move.</description></item>
/// <item><description>Child pointers and key counts are node bookkeeping rather than elements, so their reads and
/// writes are reported at the node's first key slot. That is the finest location an observer can use, and it keeps
/// the pointer array out of the set of buffers a consumer has to render.</description></item>
/// </list>
/// <para><strong>Stability:</strong></para>
/// <para>
/// A search tree is not stable or unstable in itself: the search tree property leaves the placement of equal keys
/// free, and that choice is the whole of it. The descent here takes the upper bound — the first key strictly greater
/// than the incoming element — so within a node the element passes to the right of every equal key, and it descends
/// into the child that follows them. Every equal key already in the tree is therefore met and passed, never skipped:
/// going left at key k means the element is smaller than k, so the subtrees to k's right hold strictly larger keys;
/// going right means the subtrees to its left hold keys no greater than k ≤ the element.
/// </para>
/// <para>
/// Splitting does not disturb this. A split promotes the median and partitions the rest without reordering anything,
/// so the in-order sequence is identical before and after — the same reason a rotation cannot break the stability of
/// <see cref="BalancedBinaryTreeSort"/>. The one place the descent has to look twice is right after it splits the
/// child it was about to enter: the promoted median now sits on the path, and the element takes the right side when
/// it compares equal, which is the same rule applied to a key that has just moved.
/// </para>
/// <para><strong>Reference:</strong></para>
/// <para>Bayer, R.; McCreight, E. (1972). "Organization and Maintenance of Large Ordered Indexes". Acta Informatica 1 (3): 173-189.</para>
/// <para>Cormen, Leiserson, Rivest, Stein. "Introduction to Algorithms", chapter 18 (B-Trees).</para>
/// <para>Wiki: https://en.wikipedia.org/wiki/B-tree</para>
/// </remarks>
public static class BTreeSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by BTreeSortTests, which derives from StableSortTestsBase.</remarks>
    public static bool IsStable => true;

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TREE = 1;       // Flat key array of the B-tree nodes (auxiliary buffer)
    private const int NULL_INDEX = -1;       // Represents "no node" in the arena

    /// <summary>Minimum degree t: a node holds t-1..2t-1 keys and t..2t children.</summary>
    private const int MinDegree = 8;

    /// <summary>Maximum keys per node (2t-1). Also the width of a node's block in the key array.</summary>
    private const int MaxKeys = 2 * MinDegree - 1;

    /// <summary>Maximum children per node (2t).</summary>
    private const int MaxChildren = 2 * MinDegree;

    /// <summary>Index of the key promoted to the parent when a full node is split (t-1).</summary>
    private const int MedianIndex = MinDegree - 1;

    /// <summary>
    /// Upper bound on the number of levels, used to size the traversal stack.
    /// </summary>
    /// <remarks>
    /// Occupancy gives n ≥ 2·t^(h-2)·(t-1) for a tree of h levels, which for t = 8 exceeds
    /// <see cref="int.MaxValue"/> at h = 12. 24 is therefore unreachable for any span this library can be handed.
    /// </remarks>
    private const int MaxDepth = 24;

    /// <summary>
    /// Sorts the elements in the specified span in ascending order using the default comparer.
    /// Uses NullContext for zero-overhead fast path.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort in place.</param>
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
        => Sort(span, new ComparableComparer<T>(), NullContext.Default);

    /// <summary>
    /// Sorts the elements in the specified span using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T, TContext>(Span<T> span, TContext context)
        where T : IComparable<T>
        where TContext : ISortContext
        => Sort(span, new ComparableComparer<T>(), context);

    /// <summary>
    /// Sorts the elements in the specified span using the provided comparer and sort context.
    /// This is the full-control version with explicit TContext type parameter.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use for element comparisons.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="comparer">The comparer to use for element comparisons.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;

        // Every node but the root holds at least t-1 keys and the tree holds exactly n of them,
        // so the node count cannot exceed n/(t-1) + 1.
        var maxNodes = span.Length / (MinDegree - 1) + 2;
        var keys = ArrayPool<T>.Shared.Rent(maxNodes * MaxKeys);
        var children = ArrayPool<int>.Shared.Rent(maxNodes * MaxChildren);
        var keyCounts = ArrayPool<int>.Shared.Rent(maxNodes);
        try
        {
            var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
            var tree = new BTree<T, TComparer, TContext>(
                keys.AsSpan(0, maxNodes * MaxKeys),
                children.AsSpan(0, maxNodes * MaxChildren),
                keyCounts.AsSpan(0, maxNodes),
                comparer,
                context);

            var root = tree.CreateNode();
            for (var i = 0; i < s.Length; i++)
            {
                context.OnPhase(SortPhase.BTreeInsert, i, s.Length - 1, MaxKeys);
                context.OnRole(i, BUFFER_MAIN, RoleType.Inserting);
                root = tree.Insert(root, i, s.Read(i));
                context.OnRole(i, BUFFER_MAIN, RoleType.None);
            }

            // Every element is in the tree now, so the input span is free to be overwritten.
            context.OnPhase(SortPhase.BTreeExtract, 0, 0, MaxKeys);
            tree.WriteInorder(root, s);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(keys, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
            ArrayPool<int>.Shared.Return(children);
            ArrayPool<int>.Shared.Return(keyCounts);
        }
    }

    /// <summary>
    /// The B-tree itself: a flat key array, a parallel child-pointer array, and the per-node key counts.
    /// </summary>
    /// <remarks>
    /// A ref struct so the three rented buffers travel together as spans without being re-passed to every
    /// helper, and so <typeparamref name="TComparer"/> and <typeparamref name="TContext"/> stay generic and
    /// therefore JIT-specialized on the hot path.
    /// </remarks>
    private ref struct BTree<T, TComparer, TContext>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        private readonly Span<T> _keys;
        private readonly Span<int> _children;
        private readonly Span<int> _keyCount;
        private readonly TComparer _comparer;
        private readonly TContext _context;
        private int _nodeCount;
        private int _publishedRoot;

        public BTree(Span<T> keys, Span<int> children, Span<int> keyCount, TComparer comparer, TContext context)
        {
            _keys = keys;
            _children = children;
            _keyCount = keyCount;
            _comparer = comparer;
            _context = context;
            _nodeCount = 0;
            // A node's base slot is never negative, so -1 cannot collide with a published root.
            _publishedRoot = NULL_INDEX;
        }

        /// <summary>
        /// Allocates an empty leaf.
        /// </summary>
        /// <remarks>
        /// Only bookkeeping is written — no key exists yet, so there is no slot an observer could be pointed at.
        /// The node becomes visible when its first key is written, and part of the tree when it is linked.
        /// </remarks>
        public int CreateNode()
        {
            var node = _nodeCount++;
            _keyCount[node] = 0;
            // A leaf is a node whose first child slot is empty; no separate flag is needed.
            _children[node * MaxChildren] = NULL_INDEX;
            return node;
        }

        /// <summary>
        /// Inserts one element, splitting the root first when it is full, and returns the (possibly new) root.
        /// </summary>
        /// <param name="root">Index of the current root node.</param>
        /// <param name="itemIndex">Index in the input span the value came from, used to locate comparisons.</param>
        /// <param name="value">The value to insert.</param>
        public int Insert(int root, int itemIndex, T value)
        {
            if (ReadKeyCount(root) == MaxKeys)
            {
                // The only case where a B-tree grows taller: the old root becomes the sole child of a new
                // root, which is then split so that the descent below can assume a non-full node.
                var newRoot = CreateNode();
                SetChild(newRoot, 0, root);
                SplitChild(newRoot, 0);
                root = newRoot;
            }

            InsertNonFull(root, itemIndex, value);
            PublishRoot(root);
            return root;
        }

        /// <summary>
        /// Descends from a node that is known not to be full, splitting any full child on the way, and inserts
        /// the value into the leaf it reaches.
        /// </summary>
        private void InsertNonFull(int node, int itemIndex, T value)
        {
            while (true)
            {
                var m = ReadKeyCount(node);
                var i = UpperBound(node, m, itemIndex, value);

                if (IsLeaf(node))
                {
                    // One block move opens the slot; the shifted keys keep their order, so stability is untouched.
                    MoveKeys(node, i, node, i + 1, m - i);
                    WriteKey(node, i, value);
                    SetKeyCount(node, m + 1);
                    PublishNode(node);
                    return;
                }

                var child = ReadChild(node, i);
                if (ReadKeyCount(child) == MaxKeys)
                {
                    SplitChild(node, i);
                    // The promoted median now occupies position i. Equal keys go to its right, which is the
                    // same rule the upper-bound search applied to the keys that were already there.
                    if (CompareWithKey(node, i, itemIndex, value) >= 0) i++;
                    child = ReadChild(node, i);
                }

                node = child;
            }
        }

        /// <summary>
        /// Splits the full child at position <paramref name="childPosition"/> of <paramref name="parent"/> into two
        /// nodes of t-1 keys each and promotes the median into the parent.
        /// </summary>
        /// <remarks>The parent must not be full; the descent guarantees that by splitting top-down.</remarks>
        private void SplitChild(int parent, int childPosition)
        {
            var full = ReadChild(parent, childPosition);
            var sibling = CreateNode();
            var fullIsLeaf = IsLeaf(full);

            // Upper half of the keys moves to the new sibling; the median stays in place until it is copied up.
            MoveKeys(full, MinDegree, sibling, 0, MaxKeys - MinDegree);
            if (!fullIsLeaf)
            {
                for (var c = 0; c < MaxChildren - MinDegree; c++)
                {
                    SetChild(sibling, c, ReadChild(full, MinDegree + c));
                }
            }
            SetKeyCount(sibling, MaxKeys - MinDegree);

            var median = ReadKey(full, MedianIndex);
            SetKeyCount(full, MedianIndex);

            // Open one key slot and one child slot in the parent.
            var parentKeys = ReadKeyCount(parent);
            for (var c = parentKeys; c > childPosition; c--)
            {
                SetChild(parent, c + 1, ReadChild(parent, c));
            }
            MoveKeys(parent, childPosition, parent, childPosition + 1, parentKeys - childPosition);
            WriteKey(parent, childPosition, median);
            SetChild(parent, childPosition + 1, sibling);
            SetKeyCount(parent, parentKeys + 1);

            PublishNode(full);
            PublishNode(sibling);
            PublishNode(parent);
        }

        /// <summary>
        /// Returns the position of the first key of <paramref name="node"/> that is strictly greater than
        /// <paramref name="value"/>, which is both the insertion position within a leaf and the child to descend into.
        /// </summary>
        /// <remarks>
        /// Taking the upper bound rather than the lower bound is what makes the sort stable: the incoming element
        /// passes to the right of every key equal to it.
        /// </remarks>
        private readonly int UpperBound(int node, int keyCount, int itemIndex, T value)
        {
            var lo = 0;
            var hi = keyCount;
            while (lo < hi)
            {
                var mid = (int)(((uint)lo + (uint)hi) >> 1);
                if (CompareWithKey(node, mid, itemIndex, value) < 0) hi = mid;
                else lo = mid + 1;
            }
            return lo;
        }

        /// <summary>
        /// Writes every key of the tree back to the span in ascending order.
        /// </summary>
        /// <remarks>
        /// Iterative rather than recursive: the stack holds one frame per level, and a frame's position counter
        /// alternates between the children and the keys of its node (even = child, odd = key), which is the
        /// multiway form of "left, root, right".
        /// </remarks>
        public readonly void WriteInorder(int root, SortSpan<T, TComparer, TContext> s)
        {
            Span<int> stackNode = stackalloc int[MaxDepth];
            Span<int> stackPosition = stackalloc int[MaxDepth];
            Span<int> stackKeys = stackalloc int[MaxDepth];
            Span<bool> stackLeaf = stackalloc bool[MaxDepth];

            var top = 0;
            stackNode[top] = root;
            stackPosition[top] = 0;
            stackKeys[top] = ReadKeyCount(root);
            stackLeaf[top] = IsLeaf(root);
            top++;

            var writeIndex = 0;
            while (top > 0)
            {
                var frame = top - 1;
                var position = stackPosition[frame];
                if (position > 2 * stackKeys[frame])
                {
                    top--;
                    continue;
                }
                stackPosition[frame] = position + 1;

                if ((position & 1) == 0)
                {
                    if (stackLeaf[frame]) continue;

                    var child = ReadChild(stackNode[frame], position >> 1);
                    stackNode[top] = child;
                    stackPosition[top] = 0;
                    stackKeys[top] = ReadKeyCount(child);
                    stackLeaf[top] = IsLeaf(child);
                    top++;
                }
                else
                {
                    s.Write(writeIndex++, ReadKey(stackNode[frame], position >> 1));
                }
            }
        }

        // Helper methods for node operations (encapsulates visualization tracking)

        /// <summary>Reads a key and records the access for visualization.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly T ReadKey(int node, int i)
        {
            var slot = node * MaxKeys + i;
            _context.OnIndexRead(slot, BUFFER_TREE);
            return _keys[slot];
        }

        /// <summary>Writes a key and records the write, carrying the value.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly void WriteKey(int node, int i, T value)
        {
            var slot = node * MaxKeys + i;
            _keys[slot] = value;
            _context.OnIndexWrite(slot, BUFFER_TREE, value);
        }

        /// <summary>
        /// Moves a run of keys within the key array, reporting it as one range copy.
        /// </summary>
        /// <remarks>
        /// <see cref="Span{T}.CopyTo"/> is used rather than a loop because the source and destination overlap when
        /// a key is inserted into the middle of a node, and its move semantics handle that in one memmove.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly void MoveKeys(int sourceNode, int sourceIndex, int destinationNode, int destinationIndex, int length)
        {
            if (length <= 0) return;

            var source = sourceNode * MaxKeys + sourceIndex;
            var destination = destinationNode * MaxKeys + destinationIndex;
            _context.OnRangeCopy<T>(source, destination, length, BUFFER_TREE, BUFFER_TREE, _keys.Slice(source, length));
            _keys.Slice(source, length).CopyTo(_keys.Slice(destination, length));
        }

        /// <summary>
        /// Reads a child pointer, reporting the access at the node's first key slot.
        /// </summary>
        /// <remarks>
        /// The pointer array is node bookkeeping rather than a buffer of elements, so it has no identifier of its
        /// own; the node's first key slot is the finest location an observer can act on.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly int ReadChild(int node, int i)
        {
            _context.OnIndexRead(node * MaxKeys, BUFFER_TREE);
            return _children[node * MaxChildren + i];
        }

        /// <summary>Writes a child pointer, reporting the write at the node's first key slot.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly void SetChild(int node, int i, int child)
        {
            _children[node * MaxChildren + i] = child;
            _context.OnIndexWrite(node * MaxKeys, BUFFER_TREE);
        }

        /// <summary>Reads a node's key count, reported like any other node bookkeeping access.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly int ReadKeyCount(int node)
        {
            _context.OnIndexRead(node * MaxKeys, BUFFER_TREE);
            return _keyCount[node];
        }

        /// <summary>Writes a node's key count.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly void SetKeyCount(int node, int count)
        {
            _keyCount[node] = count;
            _context.OnIndexWrite(node * MaxKeys, BUFFER_TREE);
        }

        /// <summary>Returns true when the node has no children.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly bool IsLeaf(int node)
        {
            _context.OnIndexRead(node * MaxKeys, BUFFER_TREE);
            return _children[node * MaxChildren] == NULL_INDEX;
        }

        /// <summary>
        /// Compares <paramref name="value"/> against the key at position <paramref name="i"/> of
        /// <paramref name="node"/>, recording both the access and the comparison.
        /// Returns: value.CompareTo(key)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly int CompareWithKey(int node, int i, int itemIndex, T value)
        {
            var slot = node * MaxKeys + i;
            _context.OnIndexRead(slot, BUFFER_TREE);
            var cmp = _comparer.Compare(value, _keys[slot]);
            _context.OnCompare(itemIndex, slot, cmp, BUFFER_MAIN, BUFFER_TREE);
            return cmp;
        }

        // Structural reporting. These read the node arrays directly and announce nothing but links:
        // they are the observation of the structure, not work the algorithm performs.

        /// <summary>
        /// Publishes the edges of one node in the binary encoding described in the class remarks: the left slot of
        /// a key names the child before it, and its right slot names the next key of the same node, or the child
        /// after it for the last key.
        /// </summary>
        private readonly void PublishNode(int node)
        {
            if (typeof(TContext) == typeof(NullContext)) return;

            var keys = _keyCount[node];
            if (keys == 0) return;

            var keyBase = node * MaxKeys;
            var childBase = node * MaxChildren;
            var leaf = _children[childBase] == NULL_INDEX;

            for (var i = 0; i < keys; i++)
            {
                var left = leaf ? NULL_INDEX : _children[childBase + i] * MaxKeys;
                _context.OnLink(keyBase + i, left, BUFFER_TREE, LinkSide.Left);

                var right = i + 1 < keys
                    ? keyBase + i + 1
                    : (leaf ? NULL_INDEX : _children[childBase + keys] * MaxKeys);
                _context.OnLink(keyBase + i, right, BUFFER_TREE, LinkSide.Right);
            }
        }

        /// <summary>
        /// Announces the root when it moves. The root is the one edge that no node holds, so nothing else in the
        /// stream describes it.
        /// </summary>
        private void PublishRoot(int root)
        {
            if (typeof(TContext) == typeof(NullContext)) return;

            var keyBase = root * MaxKeys;
            if (_publishedRoot == keyBase) return;

            _context.OnLink(NULL_INDEX, keyBase, BUFFER_TREE, LinkSide.None);
            _publishedRoot = keyBase;
        }
    }
}
