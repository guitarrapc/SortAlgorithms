using System.Buffers;
using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// スプレー木(Splay tree)を使用したアダプティブなソートアルゴリズム。
/// スプレー木は自己調整型の二分探索木で、直近にアクセスした要素が「スプレー操作」によって根に移動する。
/// この特性により、ソート済みや部分的にソートされた入力に対して良好な性能を発揮するアダプティブなソートとなる。
/// <br/>
/// Splay tree based adaptive sorting algorithm. A splay tree is a self-adjusting binary search tree
/// where recently accessed elements are moved to the root via splay rotations (zig, zig-zig, zig-zag).
/// This makes SplaySort adaptive: inputs with existing order patterns sort faster due to shorter traversal paths.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Splay Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>BST Property:</strong> For every node, all values in the left subtree are less than the node's value,
/// and all values in the right subtree are greater than or equal to the node's value.
/// Maintained by comparing values during insertion (go left if value &lt; node, right otherwise).</description></item>
/// <item><description><strong>Splay Property:</strong> After each insertion, the newly inserted node is splayed to the root.
/// This is achieved by repeatedly applying zig, zig-zig, or zig-zag rotations until the node reaches the root.</description></item>
/// <item><description><strong>Rotation Correctness:</strong> All rotations preserve the BST in-order property:
/// <list type="bullet">
/// <item><description>Zig: single rotation when the node's parent is the root</description></item>
/// <item><description>Zig-zig: rotate grandparent first, then parent (same-direction case)</description></item>
/// <item><description>Zig-zag: rotate parent first, then grandparent (opposite-direction case)</description></item>
/// </list></description></item>
/// <item><description><strong>In-order Traversal:</strong> Iterative left→root→right traversal writes elements in sorted order.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Tree / Adaptive</description></item>
/// <item><description>Stable      : Yes (equal elements go right during BST insertion; BST rotations preserve in-order traversal, so insertion order is maintained)</description></item>
/// <item><description>In-place    : No (requires O(n) auxiliary space for tree nodes with parent pointers)</description></item>
/// <item><description>Best case   : Θ(n log n) amortized - sorted or reverse-sorted inputs benefit from spatial locality</description></item>
/// <item><description>Average case: Θ(n log n) amortized - guaranteed by splay tree amortized analysis</description></item>
/// <item><description>Worst case  : O(n²) per-operation worst case, but O(n log n) amortized over all n insertions</description></item>
/// <item><description>Comparisons : O(n log n) amortized</description></item>
/// <item><description>Index Reads : Θ(n) main + O(comparisons) tree - each element read once from main array; each comparison reads a tree node; n traversal reads</description></item>
/// <item><description>Index Writes: Θ(2n) - each element written once to the tree (CreateNode) and once during in-order traversal</description></item>
/// <item><description>Swaps       : 0 - no swapping; elements are copied to tree nodes and written back during traversal</description></item>
/// <item><description>Space       : O(n) - one node per element; each node holds value, left/right/parent indices</description></item>
/// </list>
/// <para><strong>Adaptive Behavior:</strong></para>
/// <list type="bullet">
/// <item><description>Sorted input: recently inserted nodes remain near root → shorter traversal paths → O(n) amortized</description></item>
/// <item><description>Reversed input: each insertion reaches the opposite end, but splay moves it to root → O(n log n)</description></item>
/// <item><description>Random input: O(n log n) amortized, similar to unbalanced BST with self-adjustment benefit</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Splay_tree</para>
/// <para>Original paper: Sleator, D. D.; Tarjan, R. E. (1985). "Self-Adjusting Binary Search Trees"</para>
/// </remarks>
public static class SplaySort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TREE = 1;       // Tree nodes (auxiliary buffer for arena; tracked in statistics like merge sort's auxiliary buffer)
    private const int NULL_INDEX = -1;       // Represents null reference in arena

    // Note: Arena (Node array) operations are tracked via context callbacks with BUFFER_TREE.
    // This ensures tree node reads/writes are reflected in statistics and visualization,
    // consistent with how merge sort tracks auxiliary buffer operations.
    // Nodes cache values (T) directly for performance (avoiding indirection on every comparison).

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
        try
        {
            var arenaSpan = arena.AsSpan(0, span.Length);
            var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
            var rootIndex = NULL_INDEX;
            var nodeCount = 0;

            // Insert each element into the splay tree; after each insertion the node is splayed to the root
            for (var i = 0; i < s.Length; i++)
            {
                context.OnPhase(SortPhase.TreeSortInsert, i, s.Length - 1);
                context.OnRole(i, BUFFER_MAIN, RoleType.Inserting);
                rootIndex = Insert(arenaSpan, rootIndex, ref nodeCount, i, s);
                context.OnRole(i, BUFFER_MAIN, RoleType.None);
            }

            // Traverse the splay tree in-order and write sorted elements back into the span
            context.OnPhase(SortPhase.TreeSortExtract);
            var writeIndex = 0;
            Inorder(s, arenaSpan, rootIndex, ref writeIndex);
        }
        finally
        {
            ArrayPool<Node<T>>.Shared.Return(arena, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    /// <summary>
    /// Inserts the element at <paramref name="itemIndex"/> into the splay tree via standard BST insertion,
    /// then splays the newly inserted node to the root and returns the new root index.
    /// </summary>
    private static int Insert<T, TComparer, TContext>(
        Span<Node<T>> arena, int rootIndex, ref int nodeCount, int itemIndex,
        SortSpan<T, TComparer, TContext> s)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var value = s.Read(itemIndex);

        // Empty tree: create root directly
        if (rootIndex == NULL_INDEX)
            return CreateNode(arena, value, ref nodeCount, s.Context);

        // Standard BST traversal to find the insertion point
        var current = rootIndex;
        while (true)
        {
            var cmp = CompareWithNode(arena, current, itemIndex, value, s.Comparer, s.Context);
            if (cmp < 0)
            {
                // value < node → go left
                s.Context.OnIndexRead(current, BUFFER_TREE); // read Left pointer
                if (arena[current].Left == NULL_INDEX)
                {
                    var newIndex = CreateNode(arena, value, ref nodeCount, s.Context);
                    arena[current].Left = newIndex;
                    s.Context.OnIndexWrite(current, BUFFER_TREE); // write Left pointer
                    arena[newIndex].Parent = current;
                    s.Context.OnIndexWrite(newIndex, BUFFER_TREE); // write Parent pointer
                    current = newIndex;
                    break;
                }
                current = arena[current].Left;
            }
            else
            {
                // value >= node → go right
                // Equal keys are inserted into the right subtree.
                // Because rotations preserve in-order order, this keeps equal elements
                // in insertion order, making the overall sort stable.
                s.Context.OnIndexRead(current, BUFFER_TREE); // read Right pointer
                if (arena[current].Right == NULL_INDEX)
                {
                    var newIndex = CreateNode(arena, value, ref nodeCount, s.Context);
                    arena[current].Right = newIndex;
                    s.Context.OnIndexWrite(current, BUFFER_TREE); // write Right pointer
                    arena[newIndex].Parent = current;
                    s.Context.OnIndexWrite(newIndex, BUFFER_TREE); // write Parent pointer
                    current = newIndex;
                    break;
                }
                current = arena[current].Right;
            }
        }

        // Splay the newly inserted node to the root
        return Splay(arena, current, s.Context);
    }

    /// <summary>
    /// Brings node <paramref name="x"/> to the root via bottom-up splay rotations.
    /// <list type="bullet">
    /// <item><description>Zig: parent is the root → single rotation</description></item>
    /// <item><description>Zig-zig: x and parent are same-direction children → rotate grandparent first, then parent</description></item>
    /// <item><description>Zig-zag: x and parent are opposite-direction children → rotate parent first, then grandparent</description></item>
    /// </list>
    /// Returns <paramref name="x"/>, which is now the root (Parent == NULL_INDEX).
    /// </summary>
    private static int Splay<T, TContext>(Span<Node<T>> arena, int x, TContext context)
        where TContext : ISortContext
    {
        context.OnIndexRead(x, BUFFER_TREE); // read Parent
        while (arena[x].Parent != NULL_INDEX)
        {
            var p = arena[x].Parent;
            context.OnIndexRead(p, BUFFER_TREE); // read Parent of p (grandparent)
            var g = arena[p].Parent;

            if (g == NULL_INDEX)
            {
                // Zig: p is the root, single rotation suffices
                context.OnIndexRead(p, BUFFER_TREE); // read Left to check direction
                if (arena[p].Left == x)
                    RotateRight(arena, p, context);
                else
                    RotateLeft(arena, p, context);
            }
            else
            {
                context.OnIndexRead(g, BUFFER_TREE); // read g's Left/Right
                context.OnIndexRead(p, BUFFER_TREE); // read p's Left/Right
                if (arena[g].Left == p && arena[p].Left == x)
                {
                    // Zig-zig left-left: rotate grandparent right first, then parent right
                    RotateRight(arena, g, context);
                    RotateRight(arena, p, context);
                }
                else if (arena[g].Right == p && arena[p].Right == x)
                {
                    // Zig-zig right-right: rotate grandparent left first, then parent left
                    RotateLeft(arena, g, context);
                    RotateLeft(arena, p, context);
                }
                else if (arena[g].Left == p && arena[p].Right == x)
                {
                    // Zig-zag left-right: rotate parent left, then grandparent right
                    RotateLeft(arena, p, context);
                    RotateRight(arena, g, context);
                }
                else
                {
                    // Zig-zag right-left: rotate parent right, then grandparent left
                    RotateRight(arena, p, context);
                    RotateLeft(arena, g, context);
                }
            }

            context.OnIndexRead(x, BUFFER_TREE); // read Parent for next iteration
        }
        return x;
    }

    /// <summary>
    /// Left rotation: y = x.Right becomes the new subtree root; x becomes y.Left.
    /// Updates parent pointers for x, y, and y's former left child.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void RotateLeft<T, TContext>(Span<Node<T>> arena, int x, TContext context)
        where TContext : ISortContext
    {
        context.OnIndexRead(x, BUFFER_TREE); // read Right
        var y = arena[x].Right;

        // x.Right = y.Left
        context.OnIndexRead(y, BUFFER_TREE); // read Left
        arena[x].Right = arena[y].Left;
        context.OnIndexWrite(x, BUFFER_TREE); // write Right
        if (arena[y].Left != NULL_INDEX)
        {
            arena[arena[y].Left].Parent = x;
            context.OnIndexWrite(arena[x].Right, BUFFER_TREE); // write Parent
        }

        // y inherits x's parent
        context.OnIndexRead(x, BUFFER_TREE); // read Parent
        arena[y].Parent = arena[x].Parent;
        context.OnIndexWrite(y, BUFFER_TREE); // write Parent
        if (arena[x].Parent != NULL_INDEX)
        {
            context.OnIndexRead(arena[x].Parent, BUFFER_TREE); // read parent's Left
            if (arena[arena[x].Parent].Left == x)
            {
                arena[arena[x].Parent].Left = y;
                context.OnIndexWrite(arena[x].Parent, BUFFER_TREE);
            }
            else
            {
                arena[arena[x].Parent].Right = y;
                context.OnIndexWrite(arena[x].Parent, BUFFER_TREE);
            }
        }

        arena[y].Left = x;
        context.OnIndexWrite(y, BUFFER_TREE);
        arena[x].Parent = y;
        context.OnIndexWrite(x, BUFFER_TREE);
    }

    /// <summary>
    /// Right rotation: y = x.Left becomes the new subtree root; x becomes y.Right.
    /// Updates parent pointers for x, y, and y's former right child.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void RotateRight<T, TContext>(Span<Node<T>> arena, int x, TContext context)
        where TContext : ISortContext
    {
        context.OnIndexRead(x, BUFFER_TREE); // read Left
        var y = arena[x].Left;

        // x.Left = y.Right
        context.OnIndexRead(y, BUFFER_TREE); // read Right
        arena[x].Left = arena[y].Right;
        context.OnIndexWrite(x, BUFFER_TREE); // write Left
        if (arena[y].Right != NULL_INDEX)
        {
            arena[arena[y].Right].Parent = x;
            context.OnIndexWrite(arena[x].Left, BUFFER_TREE); // write Parent
        }

        // y inherits x's parent
        context.OnIndexRead(x, BUFFER_TREE); // read Parent
        arena[y].Parent = arena[x].Parent;
        context.OnIndexWrite(y, BUFFER_TREE); // write Parent
        if (arena[x].Parent != NULL_INDEX)
        {
            context.OnIndexRead(arena[x].Parent, BUFFER_TREE); // read parent's Right
            if (arena[arena[x].Parent].Right == x)
            {
                arena[arena[x].Parent].Right = y;
                context.OnIndexWrite(arena[x].Parent, BUFFER_TREE);
            }
            else
            {
                arena[arena[x].Parent].Left = y;
                context.OnIndexWrite(arena[x].Parent, BUFFER_TREE);
            }
        }

        arena[y].Right = x;
        context.OnIndexWrite(y, BUFFER_TREE);
        arena[x].Parent = y;
        context.OnIndexWrite(x, BUFFER_TREE);
    }

    /// <summary>
    /// Iterative in-order traversal (left → root → right) using an explicit stack.
    /// Writes sorted elements back into the original span via <paramref name="s"/>.
    /// </summary>
    private static void Inorder<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, Span<Node<T>> arena, int rootIndex, ref int writeIndex)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (rootIndex == NULL_INDEX) return;

        // Stack depth bounded by tree height; use stackalloc for small trees, ArrayPool for large
        int[]? rented = null;
        Span<int> stack = s.Length <= 128
            ? stackalloc int[s.Length]
            : (rented = ArrayPool<int>.Shared.Rent(s.Length)).AsSpan(0, s.Length);
        try
        {
            var stackTop = 0;
            var current = rootIndex;

            while (stackTop > 0 || current != NULL_INDEX)
            {
                // Push all left descendants onto the stack
                while (current != NULL_INDEX)
                {
                    stack[stackTop++] = current;
                    s.Context.OnIndexRead(current, BUFFER_TREE); // read Left pointer
                    current = arena[current].Left;
                }

                // Visit the node at the top of the stack
                current = stack[--stackTop];
                var value = ReadNodeValue(arena, current, s.Context);
                s.Write(writeIndex++, value);

                // Move to the right subtree
                s.Context.OnIndexRead(current, BUFFER_TREE); // read Right pointer
                current = arena[current].Right;
            }
        }
        finally
        {
            if (rented is not null)
                ArrayPool<int>.Shared.Return(rented);
        }
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
    /// Arena-based node structure with value caching and parent pointer for splay operations.
    /// </summary>
    /// <remarks>
    /// Struct-based to eliminate GC pressure (allocated via ArrayPool).
    /// Left, Right, and Parent are indices into the arena array (-1 represents null).
    /// Value caches the T instance directly to avoid span[index] indirection on every comparison.
    /// Parent pointer enables bottom-up splay without a separate path stack.
    /// The node's identity is its position in the arena array, so no separate Id field is needed.
    /// </remarks>
    private struct Node<T>
    {
        public T Value;     // Cached value for direct comparison (avoids span indirection)
        public int Left;    // Index in arena, -1 = null
        public int Right;   // Index in arena, -1 = null
        public int Parent;  // Index in arena, -1 = no parent (root)

        public Node(T value)
        {
            Value = value;
            Left = -1;
            Right = -1;
            Parent = -1;
        }
    }
}
