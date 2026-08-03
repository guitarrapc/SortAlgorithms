using System.Buffers;
using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// B+木（B+ tree）を用いたソート。全要素を最小次数 t の B+木へ挿入し、連結された葉を左から順に走査して配列へ書き戻します。
/// B木と違い、要素は葉だけに置かれ、内部ノードが持つのは子を選ぶための区切りキー（葉のキーの複製）だけです。
/// 葉は次の葉へのリンクを持つため、書き戻しは木を辿り直さない一直線の走査になります。
/// <br/>
/// A sort that inserts every element into a B+ tree of minimum degree t and writes the elements back by walking the
/// linked list of leaves. Unlike a B-tree, every element lives in a leaf and an internal node holds only separator
/// keys — copies of leaf keys used to choose a child. Because the leaves are chained, the output phase is a single
/// linear scan that never revisits an internal node.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct B+Tree Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Elements Live in Leaves:</strong> Every element is stored in exactly one leaf. An internal
/// node holding separators s₀ ... s_{m-1} has m+1 children, and every key in child c_i satisfies s_{i-1} ≤ key &lt; s_i.
/// A separator is a copy, so an internal key is never an element of the input.</description></item>
/// <item><description><strong>Node Occupancy:</strong> Every node except the root holds at least t-1 keys and at most
/// 2t-1, which bounds the height by log_t((n+1)/2) + 1 for every input.</description></item>
/// <item><description><strong>Uniform Leaf Depth:</strong> All leaves are at the same depth; the tree grows only at the root.</description></item>
/// <item><description><strong>Split Correctness:</strong> Splitting a leaf leaves the right half's first key in place and
/// <em>copies</em> it into the parent as a separator, so no element leaves the leaf level. Splitting an internal node
/// promotes its median, which is a separator and therefore not an element. Both preserve the left-to-right key order.</description></item>
/// <item><description><strong>Leaf Chain:</strong> Each leaf points at the leaf that follows it in key order, so the
/// concatenation of the leaves from the leftmost one is the sorted sequence.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Tree</description></item>
/// <item><description>Stable      : Yes (equal keys are inserted to the right of every equal key already present, and splitting preserves key order; see the stability note below)</description></item>
/// <item><description>In-place    : No (requires O(n) auxiliary space for the nodes)</description></item>
/// <item><description>Best case   : Θ(n log n) - the tree is built by n insertions regardless of input order</description></item>
/// <item><description>Average case: Θ(n log n)</description></item>
/// <item><description>Worst case  : Θ(n log n) - occupancy bounds the height at log_t((n+1)/2) + 1 for every input</description></item>
/// <item><description>Comparisons : Θ(n log n) - a binary search inside a node of m keys costs ⌈log₂(m+1)⌉ and the descent visits Θ(log_t n) nodes, so the base t cancels</description></item>
/// <item><description>Swaps       : 0 - elements move through node slots, never by swapping array slots</description></item>
/// <item><description>Index Reads : Θ(n) main + Θ(n log n) tree</description></item>
/// <item><description>Index Writes: Θ(n) main + O(n t) tree - each insertion shifts at most 2t-2 keys and each split moves about t</description></item>
/// <item><description>Space       : O(n) - the leaves hold n keys and the internal levels add a fraction of that, all rented from <see cref="System.Buffers.ArrayPool{T}"/></description></item>
/// </list>
/// <para><strong>What a B+ tree buys over a B-tree:</strong></para>
/// <list type="bullet">
/// <item><description>The output phase is a linear walk of the leaf chain: no traversal stack, no internal node touched
/// twice, and the keys are read in the order they are laid out inside each leaf. The B-tree's in-order traversal has to
/// climb back into every internal node between two leaves.</description></item>
/// <item><description>The descent reads only separators, which are denser than elements in a B-tree's internal nodes,
/// so a level fits in fewer cache lines.</description></item>
/// <item><description>The price is that the tree stores about n/(t-1) separator copies on top of the n elements, and
/// that a descent always runs to the leaf level — a B-tree can stop early when the key it wants sits in an internal node,
/// which a sort never benefits from because it never searches.</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description><strong>Minimum degree:</strong> t = <see cref="MinDegree"/>, matching <see cref="BTreeSort"/> so that the
/// two can be compared at the same node width. Leaves and internal nodes use the same capacity; giving leaves a larger
/// one (they need no child pointers) is a common refinement, and is left out because it would make the two sorts differ
/// in more than the property under comparison.</description></item>
/// <item><description><strong>Proactive splitting:</strong> the descent splits any full node it is about to enter, so an
/// insertion is a single pass down with no parent pointers and no second pass back up.</description></item>
/// <item><description><strong>Binary search inside a node:</strong> the position within a node is found by an upper-bound
/// binary search. A linear scan would make the comparison count Θ(n t log_t n), well above the Θ(n log n) the algorithm
/// is supposed to achieve.</description></item>
/// <item><description><strong>Layout:</strong> keys live in one flat array rented from <see cref="System.Buffers.ArrayPool{T}"/>,
/// node <c>j</c> owning the <see cref="MaxKeys"/> consecutive slots at <c>j * MaxKeys</c>; child pointers and the leaf
/// chain live in parallel int arrays. Nothing is allocated per node, and opening a slot for a key is one
/// <see cref="Span{T}.CopyTo"/>.</description></item>
/// <item><description><strong>The leftmost leaf is node 0:</strong> the tree starts as a single leaf and every split
/// allocates the <em>right</em> half as the new node, so the first node allocated stays the head of the leaf chain for
/// the whole run and the output phase needs no search to find it.</description></item>
/// </list>
/// <para><strong>What the observation stream describes:</strong></para>
/// <list type="bullet">
/// <item><description>Buffer 1 is the flat key array, holding both the elements (in leaves) and the separator copies
/// (in internal nodes). Key writes, key reads and the block moves of insertions and splits are all reported against it.</description></item>
/// <item><description>The shape is published through <see cref="ISortContext.OnLink"/> in the binary encoding a multiway
/// node has: the left slot of a key names the child before it, and its right slot names the next key of the same node,
/// or — for the last key — the child after it. Node membership is recoverable because a node's key slots are a block of
/// <see cref="MaxKeys"/> consecutive indices, and that width is announced as param3 of <see cref="SortPhase.BPlusTreeInsert"/>.</description></item>
/// <item><description>In-order over that encoding is <em>not</em> the sorted output, and a consumer must not assume it is.
/// It interleaves the separators with the elements, so it holds n elements plus one copy per separator. The sorted
/// sequence is what the sort writes to buffer 0, and the leaf-only subsequence of the traversal is what matches it.</description></item>
/// <item><description>The leaf chain is deliberately not reported as links. It is derivable — the leaves in left-to-right
/// order are the chain — and reporting it would give a leaf two incoming edges, which is not a tree.</description></item>
/// <item><description>Child pointers, key counts and chain pointers are node bookkeeping rather than elements, so their
/// reads and writes are reported at the node's first key slot.</description></item>
/// </list>
/// <para><strong>Stability:</strong></para>
/// <para>
/// The descent takes the upper bound — the first key strictly greater than the incoming element — at every node, so the
/// element passes to the right of every key equal to it and descends into the child that follows them. Since the
/// separator between two children is a copy of the right child's smallest key, comparing equal to a separator sends the
/// element right, which is where the equal elements already are; the equal elements to the left of that separator are
/// all in earlier leaves. Within the destination leaf the same upper bound places the element after the equal ones
/// there, and the block move that opens the slot preserves the order of everything it shifts.
/// </para>
/// <para>
/// Splitting does not disturb this: it partitions the keys without reordering them, and the copied-up separator is
/// compared with the same right-on-equal rule. The result is that an element never overtakes an element inserted
/// before it, which is exactly stability.
/// </para>
/// <para><strong>Reference:</strong></para>
/// <para>Comer, Douglas (1979). "The Ubiquitous B-Tree". ACM Computing Surveys 11 (2): 121-137.</para>
/// <para>Wiki: https://en.wikipedia.org/wiki/B%2B_tree</para>
/// </remarks>
public static class BPlusTreeSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by BPlusTreeSortTests, which derives from StableSortTestsBase.</remarks>
    public static bool IsStable => true;

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TREE = 1;       // Flat key array of the B+ tree nodes (auxiliary buffer)
    private const int NULL_INDEX = -1;       // Represents "no node" in the arena

    /// <summary>Minimum degree t: a node holds t-1..2t-1 keys and t..2t children.</summary>
    private const int MinDegree = 8;

    /// <summary>Maximum keys per node (2t-1). Also the width of a node's block in the key array.</summary>
    private const int MaxKeys = 2 * MinDegree - 1;

    /// <summary>Maximum children per node (2t).</summary>
    private const int MaxChildren = 2 * MinDegree;

    /// <summary>
    /// Split point (t-1). An internal node promotes the key at this index; a leaf keeps this many keys and
    /// hands the rest to its new right sibling, whose first key is copied up as the separator.
    /// </summary>
    private const int SplitIndex = MinDegree - 1;

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

        // The leaves hold n keys and every leaf but the root holds at least t-1, so there are at most
        // n/(t-1) + 1 of them; the internal levels above add a geometrically smaller number. Doubling the
        // leaf bound covers the whole tree with room to spare.
        var maxNodes = span.Length / (MinDegree - 1) * 2 + 8;
        var keys = ArrayPool<T>.Shared.Rent(maxNodes * MaxKeys);
        var children = ArrayPool<int>.Shared.Rent(maxNodes * MaxChildren);
        var keyCounts = ArrayPool<int>.Shared.Rent(maxNodes);
        var nextLeaf = ArrayPool<int>.Shared.Rent(maxNodes);
        try
        {
            var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
            var tree = new BPlusTree<T, TComparer, TContext>(
                keys.AsSpan(0, maxNodes * MaxKeys),
                children.AsSpan(0, maxNodes * MaxChildren),
                keyCounts.AsSpan(0, maxNodes),
                nextLeaf.AsSpan(0, maxNodes),
                comparer,
                context);

            // The first node allocated is the root, and it is a leaf: it stays the head of the leaf chain.
            var root = tree.CreateNode();
            for (var i = 0; i < s.Length; i++)
            {
                context.OnPhase(SortPhase.BPlusTreeInsert, i, s.Length - 1, MaxKeys);
                context.OnRole(i, BUFFER_MAIN, RoleType.Inserting);
                root = tree.Insert(root, i, s.Read(i));
                context.OnRole(i, BUFFER_MAIN, RoleType.None);
            }

            // Every element is in a leaf now, so the input span is free to be overwritten.
            context.OnPhase(SortPhase.BPlusTreeScan, 0, 0, MaxKeys);
            tree.WriteLeafChain(s);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(keys, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
            ArrayPool<int>.Shared.Return(children);
            ArrayPool<int>.Shared.Return(keyCounts);
            ArrayPool<int>.Shared.Return(nextLeaf);
        }
    }

    /// <summary>
    /// The B+ tree itself: a flat key array, a parallel child-pointer array, the per-node key counts and the
    /// leaf chain.
    /// </summary>
    /// <remarks>
    /// A ref struct so the rented buffers travel together as spans without being re-passed to every helper, and
    /// so <typeparamref name="TComparer"/> and <typeparamref name="TContext"/> stay generic and therefore
    /// JIT-specialized on the hot path.
    /// </remarks>
    private ref struct BPlusTree<T, TComparer, TContext>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        /// <summary>The head of the leaf chain: the first node allocated is a leaf and never stops being the leftmost one.</summary>
        private const int FirstLeaf = 0;

        private readonly Span<T> _keys;
        private readonly Span<int> _children;
        private readonly Span<int> _keyCount;
        private readonly Span<int> _nextLeaf;
        private readonly TComparer _comparer;
        private readonly TContext _context;
        private int _nodeCount;
        private int _publishedRoot;

        public BPlusTree(Span<T> keys, Span<int> children, Span<int> keyCount, Span<int> nextLeaf, TComparer comparer, TContext context)
        {
            _keys = keys;
            _children = children;
            _keyCount = keyCount;
            _nextLeaf = nextLeaf;
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
        /// </remarks>
        public int CreateNode()
        {
            var node = _nodeCount++;
            _keyCount[node] = 0;
            // A leaf is a node whose first child slot is empty; no separate flag is needed.
            _children[node * MaxChildren] = NULL_INDEX;
            _nextLeaf[node] = NULL_INDEX;
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
                // The only case where a B+ tree grows taller.
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
        /// Descends from a node that is known not to be full, splitting any full child on the way, and inserts the
        /// value into the leaf it reaches.
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
                    // The new separator now occupies position i. Equal keys go to its right, which is where the
                    // elements it separates from already are.
                    if (CompareWithKey(node, i, itemIndex, value) >= 0) i++;
                    child = ReadChild(node, i);
                }

                node = child;
            }
        }

        /// <summary>
        /// Splits the full child at position <paramref name="childPosition"/> of <paramref name="parent"/> and installs
        /// the resulting separator in the parent.
        /// </summary>
        /// <remarks>
        /// A leaf and an internal node split differently, and the difference is the whole point of a B+ tree: a leaf
        /// <em>copies</em> its right half's first key up, so no element leaves the leaf level, while an internal node
        /// <em>moves</em> its median up because that key is a separator and not an element. The parent must not be
        /// full; the descent guarantees that by splitting top-down.
        /// </remarks>
        private void SplitChild(int parent, int childPosition)
        {
            var full = ReadChild(parent, childPosition);
            var sibling = CreateNode();
            T separator;

            if (IsLeaf(full))
            {
                MoveKeys(full, SplitIndex, sibling, 0, MaxKeys - SplitIndex);
                SetKeyCount(sibling, MaxKeys - SplitIndex);
                SetKeyCount(full, SplitIndex);

                // The separator is a copy: the element itself stays in the leaf.
                separator = ReadKey(sibling, 0);

                // Splice the new leaf into the chain right after the one it was split from.
                SetNextLeaf(sibling, ReadNextLeaf(full));
                SetNextLeaf(full, sibling);
            }
            else
            {
                MoveKeys(full, MinDegree, sibling, 0, MaxKeys - MinDegree);
                for (var c = 0; c < MaxChildren - MinDegree; c++)
                {
                    SetChild(sibling, c, ReadChild(full, MinDegree + c));
                }
                SetKeyCount(sibling, MaxKeys - MinDegree);

                separator = ReadKey(full, SplitIndex);
                SetKeyCount(full, SplitIndex);
            }

            // Open one key slot and one child slot in the parent.
            var parentKeys = ReadKeyCount(parent);
            for (var c = parentKeys; c > childPosition; c--)
            {
                SetChild(parent, c + 1, ReadChild(parent, c));
            }
            MoveKeys(parent, childPosition, parent, childPosition + 1, parentKeys - childPosition);
            WriteKey(parent, childPosition, separator);
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
        /// Writes every element back to the span by walking the leaf chain from the leftmost leaf.
        /// </summary>
        /// <remarks>
        /// No stack and no internal node: this is what the chain exists for, and it is the one part of the algorithm a
        /// B-tree cannot do.
        /// </remarks>
        public readonly void WriteLeafChain(SortSpan<T, TComparer, TContext> s)
        {
            var writeIndex = 0;
            for (var leaf = FirstLeaf; leaf != NULL_INDEX; leaf = ReadNextLeaf(leaf))
            {
                var keys = ReadKeyCount(leaf);
                for (var i = 0; i < keys; i++)
                {
                    s.Write(writeIndex++, ReadKey(leaf, i));
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
        /// <see cref="Span{T}.CopyTo"/> is used rather than a loop because the source and destination overlap when a
        /// key is inserted into the middle of a node, and its move semantics handle that in one memmove.
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
        /// The pointer arrays are node bookkeeping rather than buffers of elements, so they have no identifier of
        /// their own; the node's first key slot is the finest location an observer can act on.
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

        /// <summary>Reads a leaf's successor in the chain.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly int ReadNextLeaf(int node)
        {
            _context.OnIndexRead(node * MaxKeys, BUFFER_TREE);
            return _nextLeaf[node];
        }

        /// <summary>Writes a leaf's successor in the chain.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly void SetNextLeaf(int node, int next)
        {
            _nextLeaf[node] = next;
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
        /// Publishes the edges of one node in the binary encoding described in the class remarks: the left slot of a
        /// key names the child before it, and its right slot names the next key of the same node, or the child after
        /// it for the last key.
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
