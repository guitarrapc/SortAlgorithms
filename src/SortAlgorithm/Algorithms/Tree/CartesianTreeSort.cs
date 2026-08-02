using System.Buffers;
using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// デカルト木(Cartesian tree)を用いたアダプティブなソートアルゴリズム。
/// 入力配列から、値について最小ヒープ順序を満たし、かつ中間順序走査が元の配列順に一致する二分木を線形時間で構築し、
/// 根を優先度付きキューに入れて「最小を取り出し、その子をキューに入れる」を繰り返すことで昇順に出力する。
/// 他の木ソートが要素を1つずつ木に挿入するのに対し、本アルゴリズムは既存の順序をそのまま木の形に写し取るため、
/// 入力に含まれる整列済みの並びがそのまま木の一本道となり、整列済み入力・逆順入力に対して O(n) で動作する。
/// <br/>
/// Adaptive sorting algorithm based on the Cartesian tree of the input. The Cartesian tree is the binary tree
/// that is min-heap ordered on values while its in-order traversal reproduces the original array order; it is
/// built in linear time with a stack. Sorted output is then produced by seeding a priority queue with the root
/// and repeatedly extracting the minimum and inserting its children.
/// Unlike the other tree sorts, which insert elements one by one, this algorithm transcribes the order already
/// present in the input into the tree shape, so a run in the input becomes a path in the tree.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Cartesian Tree Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Heap Property:</strong> Every node's value is less than or equal to the values of both its children.
/// Established during construction by popping the right spine while its values are strictly greater than the incoming element.</description></item>
/// <item><description><strong>In-order == Input order:</strong> An in-order traversal of the tree yields the elements in their original array order.
/// Maintained because the nodes popped for element i (all of which precede i in the array) become the left subtree of i,
/// and i becomes the right child of the node that remains below them.</description></item>
/// <item><description><strong>Priority Queue Extraction:</strong> A node becomes available only after its parent has been extracted.
/// The heap property guarantees that the minimum of the available set is the global minimum of all not-yet-extracted elements,
/// so repeated extract-min emits the elements in non-decreasing order.</description></item>
/// <item><description><strong>Comparison Consistency:</strong> The comparison operation must be consistent and transitive.
/// Construction relies on a strict comparison (pop only when strictly greater) so that equal elements form a right-descending chain
/// rather than being separated into different subtrees.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Tree / Adaptive</description></item>
/// <item><description>Stable      : Yes (see the stability note below)</description></item>
/// <item><description>In-place    : No (requires O(n) auxiliary space for tree nodes and O(n) for the stack / priority queue)</description></item>
/// <item><description>Best case   : Θ(n) - the Cartesian tree is a path (sorted or reverse-sorted input), so the priority queue never holds more than one node</description></item>
/// <item><description>Average case: Θ(n log n)</description></item>
/// <item><description>Worst case  : Θ(n log n) - construction is always linear; extraction is bounded by n extract-min operations on a queue of at most n nodes</description></item>
/// <item><description>Comparisons : Construction performs fewer than 2n comparisons (each element is pushed and popped at most once, plus one failed pop test).
/// Extraction performs O(n log w) comparisons, where w is the largest number of simultaneously available nodes (w = 1 for a path, w = O(n) for a balanced tree)</description></item>
/// <item><description>Index Reads : Θ(n) main + O(comparisons) tree - each element is read once from the input; each comparison and each child lookup reads a tree node</description></item>
/// <item><description>Index Writes: Θ(2n) main+tree baseline - n node creations, at most 2n-2 child-pointer writes, and n writes back to the input</description></item>
/// <item><description>Swaps       : 0 - elements are copied into tree nodes and written back during extraction; the priority queue permutes node indices, not elements</description></item>
/// <item><description>Space       : O(n) - one struct node per element plus one int array of length n; both are rented from <see cref="System.Buffers.ArrayPool{T}"/></description></item>
/// </list>
/// <para><strong>Adaptivity:</strong></para>
/// <para>
/// Construction is linear for every input, so the whole cost is the extraction, and the extraction cost is governed by
/// how wide the tree is rather than by n alone: the priority queue holds exactly the nodes whose parents have been
/// extracted and which have not been extracted themselves. An input that is already ordered (in either direction)
/// produces a path, the queue never exceeds one node, and no comparison is performed during extraction at all.
/// The more the input alternates between ascending and descending, the bushier the tree and the larger the queue.
/// </para>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>Tree nodes are struct-based and allocated via <see cref="System.Buffers.ArrayPool{T}"/> (arena); Left/Right are integer indices into the arena (-1 = null).
/// Node <c>i</c> holds the element originally at index <c>i</c>, because nodes are created in a single left-to-right pass.</description></item>
/// <item><description>The construction stack and the extraction priority queue never coexist, so a single rented <c>int</c> buffer of length n serves both.
/// Both hold arena indices, not elements, and are therefore treated as internal bookkeeping rather than as an observable buffer -
/// the same treatment the in-order traversal stack receives in <see cref="BinaryTreeSort"/>. Every element comparison and every node
/// access they cause is still reported against the tree arena.</description></item>
/// <item><description><strong>Stability:</strong> the priority queue breaks ties by arena index, which is the element's original position.
/// That is enough to make the sort stable. For two equal elements u (earlier) and v (later), construction never places u in the left
/// subtree of v: every node popped for v compares strictly greater than v, and by the heap property so does everything beneath it,
/// so no element equal to v can be in that subtree. Hence u is either a proper ancestor of v - and an ancestor is always extracted
/// first - or the two are in disjoint subtrees, in which case whichever becomes available second still loses the tie to the other
/// only if its index is larger. Either way u is written first.</description></item>
/// <item><description>The priority queue is an implicit binary min-heap over arena indices; sift-up and sift-down move indices, which is why the sort reports no swaps.</description></item>
/// <item><description>Each extracted node is announced as <see cref="RoleType.CurrentMin"/> on the tree buffer for the duration of its step.
/// The tree shape reaches an observer through the link events and the queue's membership follows from that shape plus the extraction
/// order, so the order is the only part of this phase that is not derivable — and without the role the only way to recover it is to
/// assume the node read immediately before each write-back is the extracted one, which is a coupling to the statement order here.</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Cartesian_tree</para>
/// <para>Original paper: Levcopoulos, C.; Petersson, O. (1989). "Heapsort - Adapted for Presorted Files". WADS 1989, LNCS 382, pp. 499-509.</para>
/// </remarks>
public static class CartesianTreeSort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TREE = 1;       // Tree nodes (auxiliary buffer for arena; tracked in statistics like merge sort's auxiliary buffer)
    private const int NULL_INDEX = -1;       // Represents null reference in arena

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
    /// <param name="context">The sort context for tracking operations. Cannot be null.</param>
    public static void Sort<T, TContext>(Span<T> span, TContext context)
        where T : IComparable<T>
        where TContext : ISortContext
        => Sort(span, new ComparableComparer<T>(), context);

    /// <summary>
    /// Sorts the elements in the specified span using the provided comparer and sort context.
    /// This is the full-control version with explicit TComparer and TContext type parameters.
    /// </summary>
    public static void Sort<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;

        var arena = ArrayPool<Node<T>>.Shared.Rent(span.Length);
        // The construction stack and the extraction priority queue are disjoint in time, so one buffer serves both.
        var scratch = ArrayPool<int>.Shared.Rent(span.Length);
        try
        {
            var arenaSpan = arena.AsSpan(0, span.Length);
            var scratchSpan = scratch.AsSpan(0, span.Length);
            var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);

            var rootIndex = Build(s, arenaSpan, scratchSpan);
            Extract(s, arenaSpan, scratchSpan, rootIndex);
        }
        finally
        {
            ArrayPool<int>.Shared.Return(scratch);
            ArrayPool<Node<T>>.Shared.Return(arena, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<Node<T>>());
        }
    }

    /// <summary>
    /// Builds the Cartesian tree in a single left-to-right pass and returns the root index.
    /// </summary>
    /// <remarks>
    /// <paramref name="stack"/> holds the current right spine (the path from the root to the most recently
    /// appended node). Each element is pushed once and popped at most once, so the pass is linear.
    /// </remarks>
    private static int Build<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, Span<Node<T>> arena, Span<int> stack)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var context = s.Context;
        var rootIndex = NULL_INDEX;
        var stackTop = 0;
        var nodeCount = 0;

        for (var i = 0; i < s.Length; i++)
        {
            context.OnPhase(SortPhase.CartesianTreeBuild, i, s.Length - 1);
            context.OnRole(i, BUFFER_MAIN, RoleType.Inserting);

            var value = s.Read(i);

            // Pop the right spine while its values are strictly greater than the new element.
            // The last node popped brings its whole subtree along and becomes the new node's left child,
            // which is what keeps the in-order sequence equal to the input order.
            // The comparison is strict, so an element equal to the spine top stays to its right.
            var lastPopped = NULL_INDEX;
            while (stackTop > 0 && CompareWithNode(arena, stack[stackTop - 1], i, value, s.Comparer, context) < 0)
            {
                lastPopped = stack[--stackTop];
            }

            var newIndex = CreateNode(arena, value, ref nodeCount, context);

            if (lastPopped != NULL_INDEX)
            {
                arena[newIndex].Left = lastPopped;
                context.OnIndexWrite(newIndex, BUFFER_TREE); // write Left pointer
                context.OnLink(newIndex, lastPopped, BUFFER_TREE, LinkSide.Left);
            }

            if (stackTop > 0)
            {
                // The node still on the spine adopts the new node as its right child, replacing whatever
                // was popped from that slot; the popped subtree now hangs off the new node's left.
                var parent = stack[stackTop - 1];
                arena[parent].Right = newIndex;
                context.OnIndexWrite(parent, BUFFER_TREE); // write Right pointer
                context.OnLink(parent, newIndex, BUFFER_TREE, LinkSide.Right);
            }
            else
            {
                // The spine was emptied, so the new node is smaller than everything seen so far.
                rootIndex = newIndex;
                context.OnLink(NULL_INDEX, newIndex, BUFFER_TREE, LinkSide.None);
            }

            stack[stackTop++] = newIndex;
            context.OnRole(i, BUFFER_MAIN, RoleType.None);
        }

        return rootIndex;
    }

    /// <summary>
    /// Emits the elements in ascending order by repeatedly extracting the minimum available node and
    /// making its children available.
    /// </summary>
    private static void Extract<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, Span<Node<T>> arena, Span<int> heap, int rootIndex)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (rootIndex == NULL_INDEX) return;

        var context = s.Context;
        var heapCount = 0;
        Push(arena, heap, ref heapCount, rootIndex, s.Comparer, context);

        var writeIndex = 0;
        while (heapCount > 0)
        {
            context.OnPhase(SortPhase.CartesianTreeExtract, writeIndex + 1, s.Length);

            var node = Pop(arena, heap, ref heapCount, s.Comparer, context);
            // Which node the queue chose is the one thing about this phase that an observer cannot derive.
            // The queue's membership follows from the tree and the extraction order, and the tree follows
            // from the link events, but the order itself is decided inside the queue. Naming the node here
            // is what keeps an observer from having to infer it from the position of the read below.
            context.OnRole(node, BUFFER_TREE, RoleType.CurrentMin);

            s.Write(writeIndex++, ReadNodeValue(arena, node, context));

            context.OnIndexRead(node, BUFFER_TREE); // read Left pointer
            var left = arena[node].Left;
            if (left != NULL_INDEX) Push(arena, heap, ref heapCount, left, s.Comparer, context);

            context.OnIndexRead(node, BUFFER_TREE); // read Right pointer
            var right = arena[node].Right;
            if (right != NULL_INDEX) Push(arena, heap, ref heapCount, right, s.Comparer, context);

            context.OnRole(node, BUFFER_TREE, RoleType.None);
        }
    }

    /// <summary>
    /// Inserts an arena index into the implicit binary min-heap and sifts it up into place.
    /// </summary>
    private static void Push<T, TComparer, TContext>(Span<Node<T>> arena, Span<int> heap, ref int count, int nodeIndex, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var child = count++;
        heap[child] = nodeIndex;
        while (child > 0)
        {
            var parent = (child - 1) >> 1;
            if (!Precedes(arena, heap[child], heap[parent], comparer, context)) break;
            (heap[child], heap[parent]) = (heap[parent], heap[child]);
            child = parent;
        }
    }

    /// <summary>
    /// Removes and returns the arena index of the smallest available node.
    /// </summary>
    private static int Pop<T, TComparer, TContext>(Span<Node<T>> arena, Span<int> heap, ref int count, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var min = heap[0];
        heap[0] = heap[--count];

        var parent = 0;
        while (true)
        {
            var left = 2 * parent + 1;
            if (left >= count) break;

            var candidate = left;
            var right = left + 1;
            if (right < count && Precedes(arena, heap[right], heap[left], comparer, context)) candidate = right;

            if (!Precedes(arena, heap[candidate], heap[parent], comparer, context)) break;
            (heap[candidate], heap[parent]) = (heap[parent], heap[candidate]);
            parent = candidate;
        }

        return min;
    }

    // Helper methods for node operations (encapsulates visualization tracking)

    /// <summary>
    /// Returns true when node <paramref name="a"/> must be extracted before node <paramref name="b"/>.
    /// Records both node accesses and the comparison for visualization and statistics.
    /// </summary>
    /// <remarks>
    /// Equal values are ordered by arena index, which is the element's original position in the input.
    /// The priority queue is free to break ties however it likes without affecting the sorted values, and
    /// this choice is what makes the sort stable. The tie-break itself compares indices, not elements, so
    /// it is not reported as a comparison.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool Precedes<T, TComparer, TContext>(Span<Node<T>> arena, int a, int b, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        context.OnIndexRead(a, BUFFER_TREE);
        context.OnIndexRead(b, BUFFER_TREE);
        var cmp = comparer.Compare(arena[a].Value, arena[b].Value);
        context.OnCompare(a, b, cmp, BUFFER_TREE, BUFFER_TREE);
        return cmp < 0 || (cmp == 0 && a < b);
    }

    /// <summary>
    /// Compares <paramref name="value"/> against the cached value of the node at <paramref name="nodeIndex"/>.
    /// Records both the node access and the comparison for visualization and statistics.
    /// Returns negative if value &lt; node, zero if equal, positive if value &gt; node.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CompareWithNode<T, TComparer, TContext>(
        Span<Node<T>> arena, int nodeIndex, int itemIndex, T value, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        context.OnIndexRead(nodeIndex, BUFFER_TREE);
        var cmp = comparer.Compare(value, arena[nodeIndex].Value);
        context.OnCompare(itemIndex, nodeIndex, cmp, BUFFER_MAIN, BUFFER_TREE);
        return cmp;
    }

    /// <summary>
    /// Allocates a new arena node, caches <paramref name="value"/>, and records its creation for visualization.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CreateNode<T, TContext>(Span<Node<T>> arena, T value, ref int nodeCount, TContext context)
        where TContext : ISortContext
    {
        var nodeIndex = nodeCount++;
        arena[nodeIndex] = new Node<T>(value);
        context.OnIndexWrite(nodeIndex, BUFFER_TREE, value);
        return nodeIndex;
    }

    /// <summary>
    /// Reads a node's cached value and records the access for visualization.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T ReadNodeValue<T, TContext>(Span<Node<T>> arena, int nodeIndex, TContext context)
        where TContext : ISortContext
    {
        context.OnIndexRead(nodeIndex, BUFFER_TREE);
        return arena[nodeIndex].Value;
    }

    /// <summary>
    /// Arena-based node structure for the Cartesian tree.
    /// </summary>
    /// <remarks>
    /// Struct-based to eliminate GC pressure (allocated via ArrayPool).
    /// Left and Right are indices into the arena array (-1 represents null).
    /// Value caches the T instance directly to avoid span indirection on every comparison.
    /// No parent pointer is needed: construction walks the right spine on an explicit stack and
    /// extraction only ever descends. The node's identity is its position in the arena array, which is
    /// also the element's original index, so no separate Id field is needed.
    /// </remarks>
    private struct Node<T>
    {
        public T Value;     // Cached value for direct comparison
        public int Left;    // Index in arena, -1 = null
        public int Right;   // Index in arena, -1 = null

        public Node(T value)
        {
            Value = value;
            Left = NULL_INDEX;
            Right = NULL_INDEX;
        }
    }
}
