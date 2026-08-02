using System.Buffers;
using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 二項ヒープ（binomial heap）を優先度付きキューとして用いるソート。全要素を挿入して二項木の森を構築し、
/// 最小値の取り出しを n 回繰り返して配列へ書き戻します。
/// 二項ヒープは次数の異なる二項木の集まりで、要素数 n の 2 進表現がそのまま森の形（次数 k の木の有無 = n の第 k ビット）になります。
/// <br/>
/// A sort that uses a binomial heap as its priority queue: every element is inserted to build a forest of
/// binomial trees, then the minimum is extracted n times and written back to the array.
/// A binomial heap is a set of binomial trees of distinct degrees, so the binary representation of the element
/// count is the shape of the forest — bit k of n is set exactly when a tree of degree k is present.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Binomial Heap Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Binomial Tree Structure:</strong> A binomial tree B₀ is a single node; B_k is two B_{k-1} trees
/// where one root becomes the leftmost child of the other. B_k therefore has exactly 2^k nodes, height k,
/// and its root has k children of degrees k-1, k-2, ..., 0.</description></item>
/// <item><description><strong>Min-Heap Order:</strong> Within every tree, a node's key is less than or equal to each of its children's keys.
/// This is what makes the minimum of a tree its root, so the global minimum is always one of the roots.</description></item>
/// <item><description><strong>Distinct Degrees:</strong> The root list holds at most one tree of each degree, in increasing degree order.
/// Union restores this by repeatedly linking two roots of equal degree, exactly as binary addition carries.
/// A heap of n elements therefore has at most ⌊log₂ n⌋ + 1 roots.</description></item>
/// <item><description><strong>Linking Rule:</strong> Linking two roots of equal degree keeps the smaller key as the parent,
/// which preserves min-heap order and produces a tree of degree k+1.</description></item>
/// <item><description><strong>Extraction Correctness:</strong> Removing the minimum root leaves its children — trees of degrees
/// k-1, ..., 0 — which form a valid heap once reversed into increasing degree order, and are unioned back.
/// Extracting n times therefore yields the elements in ascending order.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Heap</description></item>
/// <item><description>Stable      : No (equal keys are ordered by root-list position, which is degree order rather than insertion order; see the stability note below)</description></item>
/// <item><description>In-place    : No (requires O(n) auxiliary space for the node arena)</description></item>
/// <item><description>Best case   : Θ(n log n) - every extraction pays for the root-list scan and the union, regardless of input order</description></item>
/// <item><description>Average case: Θ(n log n)</description></item>
/// <item><description>Worst case  : Θ(n log n) - the root list never exceeds ⌊log₂ n⌋ + 1 trees</description></item>
/// <item><description>Comparisons : O(n log n) - building by n inserts costs O(n) amortized; the n extractions dominate at O(log n) each</description></item>
/// <item><description>Index Reads : Θ(n) main + O(n log n) heap - each element is read once from the input; the pointer walks are counted on the heap buffer</description></item>
/// <item><description>Index Writes: Θ(n) main + O(n log n) heap - each element is written back once; node creation and pointer updates are counted on the heap buffer</description></item>
/// <item><description>Swaps       : 0 - elements move through heap nodes, never by swapping array slots</description></item>
/// <item><description>Space       : O(n) - one struct node per element, rented from <see cref="System.Buffers.ArrayPool{T}"/></description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>Nodes live in an arena rented from <see cref="System.Buffers.ArrayPool{T}"/>; <c>Child</c> and <c>Sibling</c> are
/// arena indices (-1 = none), so no per-node GC allocation occurs. Each node caches its value, which lets the
/// extraction phase overwrite the input span freely once the build phase has consumed it.</description></item>
/// <item><description>The forest uses the left-child / right-sibling representation, so <c>Sibling</c> serves double duty:
/// it chains a node's siblings inside a parent's child list, and it chains the roots of the root list.
/// That representation is a binary tree, which is why the structural events report <c>Child</c> as
/// <see cref="LinkSide.Left"/> and <c>Sibling</c> as <see cref="LinkSide.Right"/>: an observer that replays them
/// gets the forest back without knowing anything about linking or union.</description></item>
/// <item><description>Insertion is a union with a single-node heap rather than a special case, which is what makes the
/// carry propagation — and therefore the O(1) amortized insert — visible in the operation stream.</description></item>
/// <item><description>No parent pointers are kept. They are only needed for decrease-key, which sorting never performs.</description></item>
/// </list>
/// <para><strong>Stability:</strong></para>
/// <para>
/// Not stable, and the reason is that neither of the two places where equal keys meet consults insertion order.
/// Union links equal roots by keeping the first of the pair as parent, and extraction scans the root list keeping
/// the first strict minimum; both tie-breaks resolve by position in the root list, and that list is ordered by
/// degree. Inserting <c>[2, 1a, 1b]</c> is enough to see it: <c>1a</c> ends up buried as the root of the degree-1
/// tree while <c>1b</c> sits at the front of the root list as a degree-0 tree, so extraction returns <c>1b</c> first.
/// </para>
/// <para><strong>Reference:</strong></para>
/// <para>Vuillemin, Jean (1978). "A data structure for manipulating priority queues". Communications of the ACM 21 (4): 309-315.</para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Binomial_heap</para>
/// </remarks>
public static class BinomialHeapSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by BinomialHeapSortTests, which derives from SortTestsBase and pins the reordering with an explicit counterexample.</remarks>
    public static bool IsStable => false;

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_HEAP = 1;       // Heap nodes (auxiliary buffer for the arena; tracked in statistics like merge sort's auxiliary buffer)
    private const int NULL_INDEX = -1;       // Represents "no node" in the arena

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

        // One node per element. Cannot use stackalloc: Node<T> holds a T, which may be a reference type.
        var arena = ArrayPool<Node<T>>.Shared.Rent(span.Length);
        try
        {
            var nodes = arena.AsSpan(0, span.Length);
            var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
            var head = NULL_INDEX;
            var nodeCount = 0;

            // Build phase: insert every element. Each insert is a union with a single-node heap,
            // so the root list carries like a binary counter and the whole phase costs O(n) amortized.
            for (var i = 0; i < s.Length; i++)
            {
                context.OnPhase(SortPhase.BinomialHeapInsert, i, s.Length - 1);
                context.OnRole(i, BUFFER_MAIN, RoleType.Inserting);

                var value = s.Read(i);
                var node = CreateNode(nodes, value, ref nodeCount, context);
                head = Union(nodes, head, node, comparer, context);

                context.OnRole(i, BUFFER_MAIN, RoleType.None);
            }

            // Extract phase: the heap now holds every value, so the input span is free to be overwritten.
            for (var i = 0; i < s.Length; i++)
            {
                context.OnPhase(SortPhase.BinomialHeapExtract, i + 1, s.Length);

                head = ExtractMin(nodes, head, comparer, context, out var min);
                s.Write(i, min);
            }
        }
        finally
        {
            ArrayPool<Node<T>>.Shared.Return(arena, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    /// <summary>
    /// Reports the head of the root list when it changed, and returns the new head.
    /// </summary>
    /// <remarks>
    /// The head is the one pointer that does not live in the arena, so no <c>Child</c>/<c>Sibling</c> write
    /// describes it and an observer replaying the links would otherwise lose track of where the forest starts.
    /// It is reported at each point the head actually moves rather than once per insert or extraction: an
    /// observer stepping through single operations would otherwise spend the middle of every extraction
    /// reading the root list through the node that has just been removed from it.
    /// <para>
    /// A new head of <c>-1</c> means the heap became empty, and is reported as such — the contract's
    /// "child <c>-1</c> empties the slot" applied to the root slot.
    /// </para>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int PublishRoot<TContext>(int oldHead, int newHead, TContext context)
        where TContext : ISortContext
    {
        if (newHead != oldHead)
        {
            context.OnLink(NULL_INDEX, newHead, BUFFER_HEAP, LinkSide.None);
        }
        return newHead;
    }

    /// <summary>
    /// Unions two binomial heaps: merges their root lists by degree, then links equal-degree roots
    /// until every degree occurs at most once.
    /// </summary>
    /// <param name="head1">Head of the root list currently published to the context; the merged head is reported relative to it.</param>
    /// <returns>Index of the head of the resulting root list, or <c>-1</c> when both heaps are empty.</returns>
    private static int Union<T, TComparer, TContext>(Span<Node<T>> nodes, int head1, int head2, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var head = PublishRoot(head1, MergeRootLists(nodes, head1, head2, context), context);
        if (head == NULL_INDEX) return NULL_INDEX;

        // The merged list can hold up to three roots of the same degree (two from the inputs plus one carry),
        // so the sweep looks one root ahead before deciding to link.
        //
        // Two values are carried across iterations rather than re-read. Both are known exactly, so reading
        // them again would report memory traffic the algorithm does not need:
        //   - x's degree: every branch either leaves x alone, moves x to next (whose degree was just read),
        //     or links, which raises the degree of whichever node stays a root by exactly one.
        //   - x's sibling after the branch: advancing makes it next's sibling, adopting sets it to next's
        //     sibling, and linking x under next leaves next's sibling untouched. In all three it is the
        //     nextSibling already in hand.
        var prev = NULL_INDEX;
        var x = head;
        var degreeX = ReadDegree(nodes, x, context);
        var next = ReadSibling(nodes, x, context);

        while (next != NULL_INDEX)
        {
            var nextSibling = ReadSibling(nodes, next, context);
            var degreeNext = ReadDegree(nodes, next, context);

            if (degreeX != degreeNext || (nextSibling != NULL_INDEX && ReadDegree(nodes, nextSibling, context) == degreeX))
            {
                // Different degrees, or three in a row: leave x alone and advance.
                prev = x;
                x = next;
                degreeX = degreeNext;
            }
            else if (CompareNodes(nodes, x, next, comparer, context) <= 0)
            {
                // x keeps the smaller key, so it adopts next and stays in the root list.
                SetSibling(nodes, x, nextSibling, context);
                Link(nodes, next, x, context);
                degreeX++;
            }
            else
            {
                // next holds the smaller key, so x leaves the root list and becomes its child.
                if (prev == NULL_INDEX)
                {
                    head = PublishRoot(head, next, context);
                }
                else
                {
                    SetSibling(nodes, prev, next, context);
                }
                Link(nodes, x, next, context);
                x = next;
                degreeX = degreeNext + 1;
            }

            next = nextSibling;
        }

        return head;
    }

    /// <summary>
    /// Merges two root lists into one list ordered by non-decreasing degree.
    /// Degrees may still repeat afterwards; <see cref="Union"/> is what removes the duplicates.
    /// </summary>
    /// <returns>Index of the head of the merged list, or <c>-1</c> when both lists are empty.</returns>
    private static int MergeRootLists<T, TContext>(Span<Node<T>> nodes, int a, int b, TContext context)
        where TContext : ISortContext
    {
        if (a == NULL_INDEX) return b;
        if (b == NULL_INDEX) return a;

        int head;
        if (ReadDegree(nodes, a, context) <= ReadDegree(nodes, b, context))
        {
            head = a;
            a = ReadSibling(nodes, a, context);
        }
        else
        {
            head = b;
            b = ReadSibling(nodes, b, context);
        }

        var tail = head;
        while (a != NULL_INDEX && b != NULL_INDEX)
        {
            if (ReadDegree(nodes, a, context) <= ReadDegree(nodes, b, context))
            {
                SetSibling(nodes, tail, a, context);
                tail = a;
                a = ReadSibling(nodes, a, context);
            }
            else
            {
                SetSibling(nodes, tail, b, context);
                tail = b;
                b = ReadSibling(nodes, b, context);
            }
        }

        SetSibling(nodes, tail, a != NULL_INDEX ? a : b, context);
        return head;
    }

    /// <summary>
    /// Removes the root holding the minimum key, returns its value, and unions the orphaned subtrees back in.
    /// </summary>
    /// <param name="value">Receives the extracted minimum.</param>
    /// <returns>Index of the head of the resulting root list, or <c>-1</c> when the heap became empty.</returns>
    private static int ExtractMin<T, TComparer, TContext>(Span<Node<T>> nodes, int head, TComparer comparer, TContext context, out T value)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Min-heap order puts each tree's minimum at its root, so scanning the root list finds the global minimum.
        // The scan keeps the first strict minimum, which is one of the two tie-breaks that cost this sort its stability.
        //
        // Both operands are read here even though the best key so far could be carried in a local. Reporting a
        // comparison with only one read makes the recorder's read coalescing ambiguous: it absorbs a pending read
        // whose index and buffer match either operand, and cannot tell a pointer read from a value read. The
        // sibling read that walks this loop names the very node that is usually the current minimum, so it would
        // be absorbed as that operand's read and the expanded stream would come back in a different order.
        // The tree sorts get away with the one-read form because their two operands live in different buffers.
        var minPrev = NULL_INDEX;
        var min = head;
        var prev = head;
        var current = ReadSibling(nodes, head, context);
        while (current != NULL_INDEX)
        {
            if (CompareNodes(nodes, current, min, comparer, context) < 0)
            {
                min = current;
                minPrev = prev;
            }
            prev = current;
            current = ReadSibling(nodes, current, context);
        }

        context.OnRole(min, BUFFER_HEAP, RoleType.CurrentMin);
        value = ReadValue(nodes, min, context);

        // Detach the minimum root from the root list.
        var minSibling = ReadSibling(nodes, min, context);
        if (minPrev == NULL_INDEX)
        {
            head = PublishRoot(head, minSibling, context);
        }
        else
        {
            SetSibling(nodes, minPrev, minSibling, context);
        }

        // Its children are trees of degrees k-1, ..., 0 held in decreasing order; reversing the list
        // puts them in the increasing degree order a root list requires.
        var child = ReadChild(nodes, min, context);
        var childHead = NULL_INDEX;
        while (child != NULL_INDEX)
        {
            var nextChild = ReadSibling(nodes, child, context);
            SetSibling(nodes, child, childHead, context);
            childHead = child;
            child = nextChild;
        }

        // Retire the extracted node so nothing still points into the heap through it.
        SetChild(nodes, min, NULL_INDEX, context);
        SetSibling(nodes, min, NULL_INDEX, context);
        context.OnRole(min, BUFFER_HEAP, RoleType.None);

        return Union(nodes, head, childHead, comparer, context);
    }

    /// <summary>
    /// Makes <paramref name="child"/> the first child of <paramref name="parent"/>, producing a tree of
    /// one greater degree. Both must be roots of the same degree, and <paramref name="parent"/> must hold
    /// the smaller key so min-heap order survives.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Link<T, TContext>(Span<Node<T>> nodes, int child, int parent, TContext context)
        where TContext : ISortContext
    {
        SetSibling(nodes, child, ReadChild(nodes, parent, context), context);
        SetChild(nodes, parent, child, context);

        nodes[parent].Degree++;
        context.OnIndexWrite(parent, BUFFER_HEAP); // write Degree
    }

    // Helper methods for node operations (encapsulates visualization tracking)

    /// <summary>
    /// Creates a new single-node heap in the arena and records its creation for visualization.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CreateNode<T, TContext>(Span<Node<T>> nodes, T value, ref int nodeCount, TContext context)
        where TContext : ISortContext
    {
        var nodeId = nodeCount++;
        nodes[nodeId] = new Node<T>(value);
        context.OnIndexWrite(nodeId, BUFFER_HEAP, value);
        return nodeId;
    }

    /// <summary>
    /// Reads a node's value and records the access for visualization.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T ReadValue<T, TContext>(Span<Node<T>> nodes, int nodeIndex, TContext context)
        where TContext : ISortContext
    {
        context.OnIndexRead(nodeIndex, BUFFER_HEAP);
        return nodes[nodeIndex].Value;
    }

    /// <summary>
    /// Reads a node's first-child pointer and records the access for visualization.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ReadChild<T, TContext>(Span<Node<T>> nodes, int nodeIndex, TContext context)
        where TContext : ISortContext
    {
        context.OnIndexRead(nodeIndex, BUFFER_HEAP);
        return nodes[nodeIndex].Child;
    }

    /// <summary>
    /// Reads a node's sibling pointer and records the access for visualization.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ReadSibling<T, TContext>(Span<Node<T>> nodes, int nodeIndex, TContext context)
        where TContext : ISortContext
    {
        context.OnIndexRead(nodeIndex, BUFFER_HEAP);
        return nodes[nodeIndex].Sibling;
    }

    /// <summary>
    /// Reads a node's degree and records the access for visualization.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ReadDegree<T, TContext>(Span<Node<T>> nodes, int nodeIndex, TContext context)
        where TContext : ISortContext
    {
        context.OnIndexRead(nodeIndex, BUFFER_HEAP);
        return nodes[nodeIndex].Degree;
    }

    /// <summary>
    /// Writes a node's first-child pointer, reporting it as the left slot of the left-child / right-sibling tree.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SetChild<T, TContext>(Span<Node<T>> nodes, int nodeIndex, int childIndex, TContext context)
        where TContext : ISortContext
    {
        nodes[nodeIndex].Child = childIndex;
        context.OnIndexWrite(nodeIndex, BUFFER_HEAP);
        context.OnLink(nodeIndex, childIndex, BUFFER_HEAP, LinkSide.Left);
    }

    /// <summary>
    /// Writes a node's sibling pointer, reporting it as the right slot of the left-child / right-sibling tree.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SetSibling<T, TContext>(Span<Node<T>> nodes, int nodeIndex, int siblingIndex, TContext context)
        where TContext : ISortContext
    {
        nodes[nodeIndex].Sibling = siblingIndex;
        context.OnIndexWrite(nodeIndex, BUFFER_HEAP);
        context.OnLink(nodeIndex, siblingIndex, BUFFER_HEAP, LinkSide.Right);
    }

    /// <summary>
    /// Compares two nodes' cached values and records both the accesses and the comparison.
    /// Returns: nodes[a].Value.CompareTo(nodes[b].Value)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CompareNodes<T, TComparer, TContext>(Span<Node<T>> nodes, int a, int b, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        context.OnIndexRead(a, BUFFER_HEAP);
        context.OnIndexRead(b, BUFFER_HEAP);
        var cmp = comparer.Compare(nodes[a].Value, nodes[b].Value);
        context.OnCompare(a, b, cmp, BUFFER_HEAP, BUFFER_HEAP);
        return cmp;
    }

    /// <summary>
    /// Arena-based node in the left-child / right-sibling representation of the binomial forest.
    /// </summary>
    /// <remarks>
    /// Struct-based to eliminate GC pressure (allocated via ArrayPool).
    /// <c>Child</c> points at the first (highest-degree) child and <c>Sibling</c> at the next node in the
    /// enclosing list — the parent's child list for a child, the root list for a root; -1 means none.
    /// <c>Value</c> caches the T instance so the extraction phase never has to read back through the input span.
    /// <c>Degree</c> is the child count, which union needs in order to know what may be linked with what.
    /// The node's identity is its position in the arena array, so no separate Id field is needed.
    /// </remarks>
    private struct Node<T>
    {
        public T Value;         // Cached value for direct comparison (avoids span indirection)
        public int Child;       // Index in arena of the first child, -1 = none
        public int Sibling;     // Index in arena of the next sibling / next root, -1 = none
        public int Degree;      // Number of children

        public Node(T value)
        {
            Value = value;
            Child = -1;
            Sibling = -1;
            Degree = 0;
        }
    }
}
