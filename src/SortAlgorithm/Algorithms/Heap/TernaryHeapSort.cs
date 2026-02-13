using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 配列から、常に最大の要素をルートにもつ3分木ヒープ（ternary heap）を作成します（この時点で不安定）。
/// その後、ルート要素をソート済み配列の末尾に移動し、ヒープの末端をルートに持ってきて再度ヒープ構造を維持します。これを繰り返すことで、ヒープの最大値が常にルートに保たれ、ソート済み配列に追加されることで自然とソートが行われます。
/// <br/>
/// Builds a ternary heap (3-ary heap) from the array where the root always contains the maximum element (which is inherently unstable).
/// Then, the root element is moved to the end of the sorted array, the last element is moved to the root, and the heap structure is re-established. 
/// Repeating this process ensures that the maximum value in the heap is always at the root, allowing elements to be naturally sorted as they are moved to the sorted array.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Ternary Heapsort:</strong></para>
/// <list type="number">
/// <item><description><strong>Heap Property Maintenance:</strong> For a max-heap, every parent node must be greater than or equal to its children.
/// For array index i, children are at 3i+1, 3i+2, and 3i+3. This implementation correctly maintains this property through the iterative heapify operation.</description></item>
/// <item><description><strong>Build Heap Phase:</strong> The initial heap construction starts from the last non-leaf node ((n-1)/3) and heapifies downward to index 0.
/// This bottom-up approach runs in O(n) time.</description></item>
/// <item><description><strong>Extract Max Phase:</strong> Repeatedly swap the root (maximum element) with the last element, reduce heap size, and re-heapify.
/// This phase performs n-1 extractions, each requiring O(log₃ n) heapify operations, totaling O(n log n).</description></item>
/// <item><description><strong>Heapify Operation:</strong> Uses an iterative (non-recursive) sift-down approach to restore heap property.
/// Compares parent with all three children, swaps with the largest child if needed, and continues down the tree until heap property is satisfied.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Heap / Selection</description></item>
/// <item><description>Stable      : No (swapping elements by index breaks relative order)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space)</description></item>
/// <item><description>Best case   : Ω(n log n) - Even for sorted input, heap construction and extraction are required</description></item>
/// <item><description>Average case: Θ(n log n) - Build heap O(n) + n-1 extractions with O(log₃ n) heapify each</description></item>
/// <item><description>Worst case  : O(n log n) - Guaranteed upper bound regardless of input distribution</description></item>
/// <item><description>Comparisons : ~3n log₃ n - Approximately 3 comparisons per heapify (three child checks)</description></item>
/// <item><description>Swaps       : ~n log₃ n - One swap per level during heapify, averaged across all operations</description></item>
/// <item><description>Cache       : Slightly better than binary heap due to shallower tree, but still poor compared to sequential algorithms</description></item>
/// </list>
/// <para><strong>Ternary Heap vs Binary Heap:</strong></para>
/// <list type="bullet">
/// <item><description>Tree Height: log₃ n vs log₂ n (ternary is shallower)</description></item>
/// <item><description>Comparisons per level: 3 vs 2 (ternary needs more comparisons)</description></item>
/// <item><description>Trade-off: Fewer levels but more comparisons per level often results in similar or slightly worse performance in practice</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>Uses Floyd's improved heap construction during build phase, reducing comparisons by ~25-30%</description></item>
/// <item><description>Uses iterative heapify (loop) instead of recursive for better performance and stack safety</description></item>
/// <item><description>Builds max-heap for ascending sort (min-heap would produce descending order)</description></item>
/// <item><description>Child indices: For parent i (offset-adjusted), children are at 3*(i-offset)+1+offset, 3*(i-offset)+2+offset, 3*(i-offset)+3+offset</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Heapsort</para>
/// </remarks>
public static class TernaryHeapSort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array

    /// <summary>
    /// Sorts the elements in the specified span in ascending order using the default comparer.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort in place.</param>
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
        => Sort(span, 0, span.Length, Comparer<T>.Default, NullContext.Default);

    /// <summary>
    /// Sorts the elements in the specified span using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
        => Sort(span, 0, span.Length, Comparer<T>.Default, context);

    /// <summary>
    /// Sorts the subrange [first..last) using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span containing the elements to sort.</param>
    /// <param name="first">The zero-based index of the first element in the range to sort.</param>
    /// <param name="last">The exclusive upper bound of the range to sort (one past the last element).</param>
    /// <param name="context">The sort context to use during the sorting operation for tracking statistics and visualization.</param>
    public static void Sort<T>(Span<T> span, int first, int last, ISortContext context) where T : IComparable<T>
        => Sort(span, first, last, Comparer<T>.Default, context);

    /// <summary>
    /// Sorts the subrange [first..last) using the provided comparer and sort context.
    /// </summary>
    public static void Sort<T, TComparer>(Span<T> span, int first, int last, TComparer comparer, ISortContext context) where TComparer : IComparer<T>
    {
        ArgumentOutOfRangeException.ThrowIfNegative(first);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(last, span.Length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(first, last);

        if (last - first <= 1) return;

        var s = new SortSpan<T, TComparer>(span, context, comparer, BUFFER_MAIN);
        SortCore(s, first, last);
    }

    /// <summary>
    /// Sorts the subrange [first..last) using the provided sort context.
    /// This overload accepts a SortSpan directly for use by other algorithms that already have a SortSpan instance.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="first">The inclusive start index of the range to sort.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    internal static void SortCore<T, TComparer>(SortSpan<T, TComparer> s, int first, int last) where TComparer : IComparer<T>
    {
        var n = last - first;

        // Build ternary heap using Floyd's improved heap construction
        // This reduces comparisons by ~25-30% compared to standard bottom-up heapify
        // In ternary heap, parent of node i is at (i-1)/3
        // So last non-leaf is at (n-2)/3
        for (var i = first + (n - 2) / 3; i >= first; i--)
        {
            FloydHeapify(s, i, n, first);
        }

        // Extract elements from heap
        for (var i = last - 1; i > first; i--)
        {
            // Move current root to end
            s.Swap(first, i);

            // Re-heapify the reduced heap (standard sift-down)
            Heapify(s, first, i - first, first);
        }
    }

    /// <summary>
    /// Restores the ternary heap property using Floyd's improved heap construction algorithm.
    /// This method first descends to a leaf level by always selecting the largest child among three,
    /// then sifts the original value up to its correct position.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="s">The SortSpan containing the elements and context for tracking operations.</param>
    /// <param name="root">The index of the root node of the subtree to heapify.</param>
    /// <param name="size">The size of the heap (number of elements to consider).</param>
    /// <param name="offset">The starting index offset for the heap within the span.</param>
    /// <remarks>
    /// Floyd's algorithm reduces the number of comparisons during heap construction by ~25-30%.
    /// Phase 1: Percolate down to a leaf by always taking the largest of three children (no key comparison).
    /// Phase 2: Sift up the original root value to its correct position.
    /// <para>Time Complexity: O(log₃ n) - Same asymptotic complexity but fewer comparisons in practice.</para>
    /// <para>Space Complexity: O(1) - Uses iteration instead of recursion.</para>
    /// </remarks>
    private static void FloydHeapify<T, TComparer>(SortSpan<T, TComparer> s, int root, int size, int offset) where TComparer : IComparer<T>
    {
        var rootValue = s.Read(root);
        var hole = root;
        
        // Phase 1: Percolate down to a leaf, always taking the largest of three children
        var child = 3 * (hole - offset) + 1 + offset;
        while (child < offset + size)
        {
            // Find largest among three children
            var maxChild = child;
            if (child + 1 < offset + size && s.Compare(child + 1, maxChild) > 0)
            {
                maxChild = child + 1;
            }
            if (child + 2 < offset + size && s.Compare(child + 2, maxChild) > 0)
            {
                maxChild = child + 2;
            }
            
            // Move largest child up
            s.Write(hole, s.Read(maxChild));
            hole = maxChild;
            child = 3 * (hole - offset) + 1 + offset;
        }
        
        // Phase 2: Sift up the original root value to its correct position
        var parent = offset + (hole - offset - 1) / 3;
        while (hole > root && s.Compare(rootValue, s.Read(parent)) > 0)
        {
            s.Write(hole, s.Read(parent));
            hole = parent;
            parent = offset + (hole - offset - 1) / 3;
        }
        s.Write(hole, rootValue);
    }

    /// <summary>
    /// Restores the ternary heap property for a subtree rooted at the specified index using iterative sift-down.
    /// Used during the extraction phase after removing the root element.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="s">The SortSpan containing the elements and context for tracking operations.</param>
    /// <param name="root">The index of the root node of the subtree to heapify.</param>
    /// <param name="size">The size of the heap (number of elements to consider).</param>
    /// <param name="offset">The starting index offset for the heap within the span.</param>
    /// <remarks>
    /// This method implements the standard sift-down operation to maintain the max-heap property for a ternary heap.
    /// It iteratively compares the parent node with its three children (left, middle, right), swapping with the largest child if needed,
    /// and continues down the tree until the heap property is satisfied or a leaf node is reached.
    /// <para>In a ternary heap, for parent index i (offset-adjusted): children are at 3*(i-offset)+1+offset, 3*(i-offset)+2+offset, 3*(i-offset)+3+offset</para>
    /// <para>Time Complexity: O(log₃ n) - Worst case traverses from root to leaf (height of the tree is log₃ n).</para>
    /// <para>Space Complexity: O(1) - Uses iteration instead of recursion.</para>
    /// </remarks>
    private static void Heapify<T, TComparer>(SortSpan<T, TComparer> s, int root, int size, int offset) where TComparer : IComparer<T>
    {
        while (true)
        {
            var largest = root;
            var child1 = 3 * (root - offset) + 1 + offset; // First child
            var child2 = 3 * (root - offset) + 2 + offset; // Second child
            var child3 = 3 * (root - offset) + 3 + offset; // Third child

            // If first child is larger than root
            if (child1 < offset + size && s.Compare(child1, largest) > 0)
            {
                largest = child1;
            }

            // If second child is larger than largest so far
            if (child2 < offset + size && s.Compare(child2, largest) > 0)
            {
                largest = child2;
            }

            // If third child is larger than largest so far
            if (child3 < offset + size && s.Compare(child3, largest) > 0)
            {
                largest = child3;
            }

            // If largest is not root, swap and heapify the affected sub-tree
            if (largest != root)
            {
                s.Swap(root, largest);
                root = largest;
            }
            else
            {
                break;
            }
        }
    }
}
