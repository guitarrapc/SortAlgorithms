using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// バイナリ検索木(Binary Search Tree, BST)を使用したソートアルゴリズム、二分木ソートとも呼ばれる。
/// バイナリ検索木では、左の子ノードは親ノードより小さく、右の子ノードは親ノードより大きいことが保証される。
/// この特性により、木の中間順序走査 (in-order traversal) を行うことで配列がソートされる。
/// ただし、木が不均衡になると最悪ケースでO(n²)の時間がかかる可能性がある。また、ノードごとにクラスインスタンスを生成するためメモリアロケーションが多く、現実的なソートとしてはQuickSortやMergeSortを用いることが多い。
/// <br/>
/// Non-optimized version of Binary Tree Sort using class-based nodes.
/// A sorting algorithm that uses a binary search tree. In a binary search tree, the left child node is guaranteed to be smaller than the parent node, and the right child node is guaranteed to be larger.
/// This property ensures that performing an in-order traversal of the tree results in a sorted array.
/// However, an unbalanced tree can lead to O(n²) worst-case time complexity. Additionally, because each node allocates a class instance, memory allocations are high, making QuickSort or MergeSort more practical for real-world sorting.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Binary Tree Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Binary Search Tree Property:</strong> For every node, all values in the left subtree must be less than the node's value, and all values in the right subtree must be greater than or equal to the node's value.
/// This implementation maintains this invariant during insertion (value &lt; current goes left, value ≥ current goes right).</description></item>
/// <item><description><strong>Complete Tree Construction:</strong> All n elements must be inserted into the BST.
/// Each insertion reads one element from the array (n reads total).</description></item>
/// <item><description><strong>In-Order Traversal:</strong> The tree must be traversed in in-order (left → root → right) to produce sorted output.
/// This traversal visits each node exactly once, writing n elements back to the array.</description></item>
/// <item><description><strong>Comparison Consistency:</strong> The comparison operation must be consistent and transitive.
/// For all elements a, b, c: if a &lt; b and b &lt; c, then a &lt; c.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Tree-based sorting</description></item>
/// <item><description>Stable      : No (equal elements may be reordered based on insertion order)</description></item>
/// <item><description>In-place    : No (Requires O(n) auxiliary space for tree nodes)</description></item>
/// <item><description>Best case   : Θ(n log n) - Balanced tree (e.g., random input or middle-out insertion)</description></item>
/// <item><description>Average case: Θ(n log n) - Tree height is O(log n), each insertion takes O(log n) comparisons</description></item>
/// <item><description>Worst case  : Θ(n²) - Completely unbalanced tree (e.g., sorted or reverse-sorted input forms a linear chain)</description></item>
/// <item><description>Comparisons : Best Θ(n log n), Average Θ(n log n), Worst Θ(n²)</description></item>
/// <item><description>  - Sorted input: n(n-1)/2 comparisons (each insertion compares with all previous elements)</description></item>
/// <item><description>  - Random input: ~1.39n log n comparisons (empirically, for balanced trees)</description></item>
/// <item><description>Index Reads : Θ(n) - Each element is read once during tree construction</description></item>
/// <item><description>Index Writes: Θ(n) - Each element is written once during in-order traversal</description></item>
/// <item><description>Swaps       : 0 (No swapping; elements are copied to tree nodes and then back to array)</description></item>
/// <item><description>Space       : O(n) - One node allocated per element (worst case: n allocations of ~24-32 bytes each)</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>Uses iterative insertion instead of recursive insertion to reduce call stack overhead</description></item>
/// <item><description>Tree nodes are implemented as reference types (class) because C# structs cannot contain self-referencing fields</description></item>
/// <item><description>Equal elements are inserted to the right subtree (value ≥ current), making the sort unstable</description></item>
/// <item><description>No tree balancing is performed; for guaranteed O(n log n) performance, consider using AVL or Red-Black tree variants</description></item>
/// <item><description><strong>Non-Optimized:</strong> This version uses class-based nodes with reference type overhead. See <see cref="BinaryTreeSort"/> for an arena-based optimized version.</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Tree_sort</para>
/// </remarks>
public static class BinaryTreeSort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TREE = -1;      // Tree nodes (virtual buffer for visualization, negative to exclude from statistics)

    /// <summary>
    /// Sorts the elements in the specified span in ascending order using the default comparer.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort in place.</param>
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
    {
        Sort(span, NullContext.Default);
    }

    /// <summary>
    /// Sorts the elements in the specified span using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T>(span, context, BUFFER_MAIN);

        // The root node of the binary tree (null == the tree is empty).
        Node<T>? root = null;

        // Node counter for visualization (assigns unique IDs to each node)
        var nodeCounter = 0;

        for (var i = 0; i < s.Length; i++)
        {
            var value = s.Read(i);
            InsertIterative(ref root, value, context, ref nodeCounter);
        }

        // Traverse the tree in inorder and write elements back into the array.
        var n = 0;
        Inorder(s, root, ref n, context);
    }

    /// <summary>
    /// Iterative insertion. Instead of using recursion, it loops to find the child nodes.
    /// </summary>
    private static void InsertIterative<T>(ref Node<T>? node, T value, ISortContext context, ref int nodeCounter) where T : IComparable<T>
    {
        // If the tree is empty, create a new root and return.
        if (node is null)
        {
            node = CreateNode(value, ref nodeCounter, context);
            return;
        }

        // Iterate left & right node and insert.
        // If there's an existing tree, use 'current' to traverse down the children.
        Node<T> current = node;
        while (true)
        {
            // Compare value with current node's item (reads node value for visualization)
            var cmp = CompareWithNode(value, current, context);

            // If the value is smaller than the current node, go left.
            if (cmp < 0)
            {
                // If the left child is null, insert here.
                if (current.Left is null)
                {
                    current.Left = CreateNode(value, ref nodeCounter, context);
                    break;
                }
                // Otherwise, move further down to the left child.
                current = current.Left;
            }
            else
            {
                // If the value is greater or equal, go right.
                if (current.Right is null)
                {
                    current.Right = CreateNode(value, ref nodeCounter, context);
                    break;
                }
                // Otherwise, move further down to the right child.
                current = current.Right;
            }
        }
    }

    private static void Inorder<T>(SortSpan<T> s, Node<T>? node, ref int i, ISortContext context) where T : IComparable<T>
    {
        if (node is null) return;

        Inorder(s, node.Left, ref i, context);
        
        // Read node value for visualization and write to array
        var value = ReadNodeValue(node, context);
        s.Write(i++, value);
        
        Inorder(s, node.Right, ref i, context);
    }

    // Helper methods for node operations (encapsulates visualization tracking)

    /// <summary>
    /// Creates a new tree node and records its creation for visualization.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Node<T> CreateNode<T>(T value, ref int nodeCounter, ISortContext context)
    {
        var nodeId = nodeCounter++;
        var node = new Node<T>(value, nodeId);
        // Record node creation in the tree buffer for visualization
        context.OnIndexWrite(nodeId, BUFFER_TREE, value);
        return node;
    }

    /// <summary>
    /// Reads a node's value and records the access for visualization.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T ReadNodeValue<T>(Node<T> node, ISortContext context)
    {
        // Visualize node access during traversal
        context.OnIndexRead(node.Id, BUFFER_TREE);
        return node.Item;
    }

    /// <summary>
    /// Compares a value with a node's value and records the comparison for statistics.
    /// Also records the node access for visualization.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CompareWithNode<T>(T value, Node<T> node, ISortContext context) where T : IComparable<T>
    {
        // Visualize node access during tree traversal
        context.OnIndexRead(node.Id, BUFFER_TREE);
        
        // Compare value with node's item
        // Note: This comparison is counted as a main array comparison (bufferId 0)
        // because the values originated from the main array
        var cmp = value.CompareTo(node.Item);
        context.OnCompare(-1, -1, cmp, 0, 0);
        
        return cmp;
    }

    /// <summary>
    /// Represents a node in a binary tree structure that stores a value and references to left and right child nodes.
    /// </summary>
    /// <remarks>
    /// Class-based node with reference type overhead. Each node allocation incurs GC pressure.
    /// </remarks>
    /// <typeparam name="T">The type of the value stored in the node.</typeparam>
    /// <param name="value">The value to store in the node.</param>
    /// <param name="id">The unique identifier for this node (used for visualization).</param>
    private class Node<T>(T value, int id)
    {
        public int Id = id;          // Unique node ID for visualization
        public T Item = value;
        public Node<T>? Left;
        public Node<T>? Right;
    }
}
