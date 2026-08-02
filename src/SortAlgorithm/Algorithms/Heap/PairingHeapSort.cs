using System.Buffers;
using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// ペアリングヒープ（pairing heap）を優先度付きキューとして用いるソート。全要素を挿入して 1 本の多分木を作り、
/// 最小値の取り出しを n 回繰り返して配列へ書き戻します。
/// 挿入は根同士を 1 回比べて負けた方を子にするだけ（O(1)）で、木を整える仕事は取り出しのときの
/// 「二段のペアリング」にすべて先送りされます。
/// <br/>
/// A sort that uses a pairing heap as its priority queue: every element is inserted to build a single multiway
/// tree, then the minimum is extracted n times and written back to the array.
/// Insertion is one comparison between two roots, with the loser becoming the winner's first child (O(1));
/// all of the work of keeping the tree shallow is deferred to the two-pass pairing performed on extraction.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Pairing Heap Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Min-Heap Order:</strong> A node's key is less than or equal to each of its children's keys.
/// The tree is a general multiway tree — no shape, degree, or balance condition is imposed — so the minimum is always the root.</description></item>
/// <item><description><strong>Meld:</strong> Two heaps are combined by comparing their roots and making the larger root
/// the first child of the smaller. One comparison, two pointer writes, and min-heap order is preserved.</description></item>
/// <item><description><strong>Insertion:</strong> A single-node heap melded with the current heap. Nothing is searched and
/// nothing is restructured, which is why a run of increasing keys leaves the root holding every other element as a child.</description></item>
/// <item><description><strong>Two-Pass Pairing:</strong> Removing the root leaves its children as independent heaps.
/// The first pass melds them in pairs from left to right; the second pass melds those results from right to left into one heap.
/// Both passes are required: melding the children one at a time left to right would rebuild the same long chain and
/// give O(n) amortized extraction.</description></item>
/// <item><description><strong>Extraction Correctness:</strong> The root is the minimum, so extracting n times yields the elements
/// in ascending order.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Heap</description></item>
/// <item><description>Stable      : No (equal keys are ordered by child-list position, which the pairing passes reorder; see the stability note below)</description></item>
/// <item><description>In-place    : No (requires O(n) auxiliary space for the node arena)</description></item>
/// <item><description>Best case   : Θ(n) - a descending run makes every insert the new root, leaving a chain whose every extraction has a single child to pair</description></item>
/// <item><description>Average case: Θ(n log n)</description></item>
/// <item><description>Worst case  : O(n log n) - extraction is O(log n) amortized; a single extraction may cost O(n)</description></item>
/// <item><description>Comparisons : O(n log n) - one per insert, plus one per meld during the pairing passes</description></item>
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
/// <item><description>The tree uses the left-child / right-sibling representation, so <c>Sibling</c> serves double duty:
/// it chains a node's siblings inside a parent's child list, and it chains the intermediate heaps of the pairing passes.
/// That representation is a binary tree, which is why the structural events report <c>Child</c> as
/// <see cref="LinkSide.Left"/> and <c>Sibling</c> as <see cref="LinkSide.Right"/>.</description></item>
/// <item><description>Both pairing passes are iterative. The recursive formulation is the one usually written down, but its
/// depth is the root's child count, which a run of increasing keys drives to n-1.</description></item>
/// <item><description>The second pass needs the first pass's results in reverse. They are pushed onto a stack threaded through
/// the unused <c>Sibling</c> slots of the intermediate roots, so the passes need no auxiliary storage at all.</description></item>
/// <item><description>No parent pointers are kept. They are only needed for decrease-key, which sorting never performs.</description></item>
/// </list>
/// <para><strong>Difference from BinomialHeapSort:</strong></para>
/// <list type="bullet">
/// <item><description>One tree of arbitrary shape, not a forest of trees whose degrees are fixed by the element count.</description></item>
/// <item><description>Nothing is restructured on insert; the binomial heap carries immediately, like incrementing a counter.</description></item>
/// <item><description>The bound is amortized rather than worst-case per operation: one extraction may touch every child.</description></item>
/// </list>
/// <para><strong>Stability:</strong></para>
/// <para>
/// Not stable. Meld keeps the first of an equal pair as the parent and pushes the second onto the front of its child
/// list, so among equal keys the later arrival ends up <em>earlier</em> in the child list; the pairing passes then
/// reorder that list again. Three equal keys are enough to see it: inserting <c>1a, 1b, 1c</c> leaves <c>1a</c> as the
/// root with children <c>[1c, 1b]</c>, and extraction returns <c>1a, 1c, 1b</c>.
/// </para>
/// <para><strong>Reference:</strong></para>
/// <para>Fredman, M. L.; Sedgewick, R.; Sleator, D. D.; Tarjan, R. E. (1986). "The pairing heap: a new form of self-adjusting heap". Algorithmica 1 (1): 111-129.</para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Pairing_heap</para>
/// </remarks>
public static class PairingHeapSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by PairingHeapSortTests, which derives from SortTestsBase and pins the reordering with an explicit counterexample.</remarks>
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
            var root = NULL_INDEX;
            var nodeCount = 0;

            // Build phase: insert every element. Each insert is one meld with a single-node heap,
            // so the whole phase costs exactly one comparison per element and never restructures.
            for (var i = 0; i < s.Length; i++)
            {
                context.OnPhase(SortPhase.PairingHeapInsert, i, s.Length - 1);
                context.OnRole(i, BUFFER_MAIN, RoleType.Inserting);

                var value = s.Read(i);
                var node = CreateNode(nodes, value, ref nodeCount, context);
                root = PublishRoot(root, Meld(nodes, root, node, comparer, context), context);

                context.OnRole(i, BUFFER_MAIN, RoleType.None);
            }

            // Extract phase: the heap now holds every value, so the input span is free to be overwritten.
            for (var i = 0; i < s.Length; i++)
            {
                context.OnPhase(SortPhase.PairingHeapExtract, i + 1, s.Length);

                root = ExtractMin(nodes, root, comparer, context, out var min);
                s.Write(i, min);
            }
        }
        finally
        {
            ArrayPool<Node<T>>.Shared.Return(arena, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    /// <summary>
    /// Reports the root of the heap when it changed, and returns the new root.
    /// </summary>
    /// <remarks>
    /// The root is the one pointer that does not live in the arena, so no <c>Child</c>/<c>Sibling</c> write
    /// describes it and an observer replaying the links would otherwise lose track of where the tree starts.
    /// A new root of <c>-1</c> means the heap became empty, and is reported as such — the contract's
    /// "child <c>-1</c> empties the slot" applied to the root slot.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int PublishRoot<TContext>(int oldRoot, int newRoot, TContext context)
        where TContext : ISortContext
    {
        if (newRoot != oldRoot)
        {
            context.OnLink(NULL_INDEX, newRoot, BUFFER_HEAP, LinkSide.None);
        }
        return newRoot;
    }

    /// <summary>
    /// Combines two heaps by comparing their roots and making the larger root the first child of the smaller.
    /// </summary>
    /// <remarks>
    /// This is the whole of the data structure. Insertion, the pairing passes, and (in the general structure)
    /// melding two heaps are all this one step, which is why nothing here depends on the shape of either tree.
    /// The tie goes to <paramref name="a"/>, and that choice is one of the two reasons the sort is not stable.
    /// </remarks>
    /// <returns>Index of the root of the combined heap.</returns>
    private static int Meld<T, TComparer, TContext>(Span<Node<T>> nodes, int a, int b, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (a == NULL_INDEX) return b;
        if (b == NULL_INDEX) return a;

        // The loser is pushed onto the front of the winner's child list, so its sibling slot takes the
        // winner's previous first child. Both writes are on the loser and the winner; nothing else moves.
        if (CompareNodes(nodes, a, b, comparer, context) <= 0)
        {
            SetSibling(nodes, b, ReadChild(nodes, a, context), context);
            SetChild(nodes, a, b, context);
            return a;
        }

        SetSibling(nodes, a, ReadChild(nodes, b, context), context);
        SetChild(nodes, b, a, context);
        return b;
    }

    /// <summary>
    /// Removes the root, returns its value, and rebuilds a single heap from its children using two-pass pairing.
    /// </summary>
    /// <param name="value">Receives the extracted minimum.</param>
    /// <returns>Index of the root of the resulting heap, or <c>-1</c> when the heap became empty.</returns>
    private static int ExtractMin<T, TComparer, TContext>(Span<Node<T>> nodes, int root, TComparer comparer, TContext context, out T value)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        context.OnRole(root, BUFFER_HEAP, RoleType.CurrentMin);
        value = ReadValue(nodes, root, context);

        var child = ReadChild(nodes, root, context);

        // Retire the extracted node so nothing still points into the heap through it.
        SetChild(nodes, root, NULL_INDEX, context);
        SetSibling(nodes, root, NULL_INDEX, context);
        context.OnRole(root, BUFFER_HEAP, RoleType.None);

        // Report that the heap has no single root for the duration of the passes. It genuinely does not:
        // the children are independent heaps until the second pass folds them back into one, and an observer
        // that kept reading through the node just retired would spend the whole extraction looking at nothing.
        var published = PublishRoot(root, NULL_INDEX, context);

        // Pass 1: meld the children in pairs from left to right, pushing each result onto a stack.
        // The stack is threaded through the sibling slots of the intermediate roots, which are free:
        // a root has no siblings, and every node here is the root of its own heap for the moment.
        var stack = NULL_INDEX;
        var current = child;
        while (current != NULL_INDEX)
        {
            var a = current;
            var b = ReadSibling(nodes, a, context);
            if (b == NULL_INDEX)
            {
                // Odd child out: it carries into pass 2 unpaired.
                SetSibling(nodes, a, stack, context);
                stack = a;
                break;
            }

            current = ReadSibling(nodes, b, context);

            // Neither sibling is cleared first. The usual formulation detaches both before melding, but
            // every one of those writes is overwritten before it can be read: the meld's loser has its
            // sibling set to the winner's first child, and the winner's is set by the push on the next line.
            var paired = Meld(nodes, a, b, comparer, context);
            SetSibling(nodes, paired, stack, context);
            stack = paired;
        }

        // Pass 2: meld the results from right to left. Popping the stack yields exactly that order,
        // because pass 1 pushed them left to right.
        var result = NULL_INDEX;
        while (stack != NULL_INDEX)
        {
            var top = stack;
            stack = ReadSibling(nodes, top, context);
            result = Meld(nodes, result, top, comparer, context);
        }

        // The popped node keeps its stack link only while it wins its melds, and a winner is a root — no
        // child list runs through it — so the stale link is unreachable until the fold ends. Clearing it
        // once here costs one write instead of one per pop.
        if (result != NULL_INDEX) SetSibling(nodes, result, NULL_INDEX, context);

        return PublishRoot(published, result, context);
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
    /// Arena-based node in the left-child / right-sibling representation of the pairing heap.
    /// </summary>
    /// <remarks>
    /// Struct-based to eliminate GC pressure (allocated via ArrayPool).
    /// <c>Child</c> points at the first (most recently melded) child and <c>Sibling</c> at the next node in the
    /// enclosing list — the parent's child list, or the pass-1 stack while the pairing passes run; -1 means none.
    /// <c>Value</c> caches the T instance so the extraction phase never has to read back through the input span.
    /// No degree or rank is stored: the pairing heap imposes no shape condition, so nothing about a node's
    /// subtree needs to be known in order to meld it.
    /// The node's identity is its position in the arena array, so no separate Id field is needed.
    /// </remarks>
    private struct Node<T>
    {
        public T Value;         // Cached value for direct comparison (avoids span indirection)
        public int Child;       // Index in arena of the first child, -1 = none
        public int Sibling;     // Index in arena of the next sibling, -1 = none

        public Node(T value)
        {
            Value = value;
            Child = -1;
            Sibling = -1;
        }
    }
}
