using SortAlgorithm.Contexts;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 配列から、常に最大の要素をルートにもつヒープ（二分ヒープ）を作成します（この時点で不安定）。
/// その後、ルート要素をソート済み配列の末尾に移動し、ヒープの末端をルートに持ってきて再度ヒープ構造を維持します。これを繰り返すことで、ヒープの最大値が常にルートに保たれ、ソート済み配列に追加されることで自然とソートが行われます。
/// <br/>
/// Builds a heap (binary heap) from the array where the root always contains the maximum element (which is inherently unstable).
/// Then, the root element is moved to the end of the sorted array, the last element is moved to the root, and the heap structure is re-established. Repeating this process ensures that the maximum value in the heap is always at the root, allowing elements to be naturally sorted as they are moved to the sorted array.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Heapsort:</strong></para>
/// <list type="number">
/// <item><description><strong>Heap Property Maintenance:</strong> For a max-heap, every parent node must be greater than or equal to its children.
/// For array index i, left child is at 2i+1 and right child is at 2i+2. This implementation correctly maintains this property through the iterative heapify operation.</description></item>
/// <item><description><strong>Build Heap Phase:</strong> The initial heap construction starts from the last non-leaf node (n/2 - 1) and heapifies downward to index 0.
/// This implementation uses Floyd's improved heap construction algorithm, which reduces comparisons by ~25% compared to standard bottom-up heapify.
/// This bottom-up approach runs in O(n) time, which is more efficient than the naive O(n log n) top-down construction.</description></item>
/// <item><description><strong>Extract Max Phase:</strong> Repeatedly reads the root (maximum) and the last element, sifts the last element down via Heapify, then writes the max to the end.
/// No swap is needed; this reduces memory writes and eliminates all Swap operations from the extraction phase.
/// This phase performs n-1 extractions, each requiring O(log n) heapify operations, totaling O(n log n).</description></item>
/// <item><description><strong>Floyd's Heapify Optimization:</strong> During heap construction, uses Floyd's two-phase algorithm:
/// Phase 1 percolates down to a leaf by selecting larger children without comparing keys.
/// Phase 2 sifts the original value up to its correct position. This reduces the average number of comparisons.</description></item>
/// <item><description><strong>Standard Heapify:</strong> During extraction phase, uses hole-based iterative sift-down.
/// Saves the root value, descends by moving the larger child up into the hole, then writes the saved value at its correct position.
/// This reduces memory writes compared to swap-based sift-down.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Heap / Selection</description></item>
/// <item><description>Stable      : No (swapping elements by index breaks relative order)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space)</description></item>
/// <item><description>Best case   : Ω(n log n) - Even for sorted input, heap construction and extraction are required</description></item>
/// <item><description>Average case: Θ(n log n) - Build heap O(n) + n-1 extractions with O(log n) heapify each</description></item>
/// <item><description>Worst case  : O(n log n) - Guaranteed upper bound regardless of input distribution</description></item>
/// <item><description>Comparisons : ~2n log n - Approximately 2 comparisons per heapify (left and right child checks)</description></item>
/// <item><description>Swaps       : 0 - No swaps; extraction reads root+last, sifts last down, writes root to end</description></item>
/// <item><description>Cache       : Poor locality - Heap structure causes frequent cache misses due to non-sequential access</description></item>
/// </list>
/// <para><strong>Why "Heap / Selection" Family?:</strong></para>
/// <para>
/// HeapSort belongs to the Selection sort family. Like Selection Sort, it repeatedly
/// selects the maximum element and places it at the end of the sorted portion.
/// The key difference is the selection mechanism:
/// </para>
/// <list type="bullet">
/// <item><description>Selection Sort: Linear search O(n) to find maximum</description></item>
/// <item><description>Heap Sort: Heap structure O(log n) to extract maximum</description></item>
/// </list>
/// <para>
/// Thus, HeapSort is essentially an optimized Selection Sort using a heap data structure,
/// improving time complexity from O(n²) to O(n log n).
/// </para>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>Uses iterative heapify (loop) instead of recursive for better performance and stack safety</description></item>
/// <item><description>Builds max-heap for ascending sort (min-heap would produce descending order)</description></item>
/// <item><description>Comparison-based algorithm: requires O(n log n) comparisons in all cases</description></item>
/// <item><description>Despite O(n log n) guarantee, often slower than Quicksort in practice due to poor cache performance</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Heapsort</para>
/// </remarks>
public static class HeapSort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array

    /// <summary>
    /// Sorts the elements in the specified span in ascending order using the default comparer.
    /// Uses NullContext for zero-overhead fast path.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort in place.</param>
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
        => Sort(span, 0, span.Length, new ComparableComparer<T>(), NullContext.Default);

    /// <summary>
    /// Sorts the elements in the specified span using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <typeparam name="TContext">The type of the sort context.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T, TContext>(Span<T> span, TContext context)
        where T : IComparable<T>
        where TContext : ISortContext
        => Sort(span, 0, span.Length, new ComparableComparer<T>(), context);

    /// <summary>
    /// Sorts the elements in the specified span using the provided comparer and sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of the comparer</typeparam>
    /// <typeparam name="TContext">The type of the sort context.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="comparer">The comparer to use for element comparisons.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        => Sort(span, 0, span.Length, comparer, context);

    /// <summary>
    /// Sorts the subrange [first..last) using the provided comparer and sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <typeparam name="TContext">The type of the sort context.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="first">The inclusive start index of the range to sort.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T, TContext>(Span<T> span, int first, int last, TContext context)
        where T : IComparable<T>
        where TContext : ISortContext
        => Sort(span, first, last, new ComparableComparer<T>(), context);

    /// <summary>
    /// Sorts the subrange [first..last) using the provided comparer and sort context.
    /// This is the full-control version with explicit TContext type parameter.
    /// </summary>
    public static void Sort<T, TComparer, TContext>(Span<T> span, int first, int last, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        ArgumentOutOfRangeException.ThrowIfNegative(first);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(last, span.Length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(first, last);

        if (last - first <= 1) return;

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        SortCore(s, first, last);
    }

    /// <summary>
    /// Sorts the subrange [first..last) using the provided sort context.
    /// This overload accepts a SortSpan directly for use by other algorithms that already have a SortSpan instance.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use for element comparisons.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="first">The inclusive start index of the range to sort.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = last - first;

        // Build heap
        for (var i = first + n / 2 - 1; i >= first; i--)
        {
            FloydHeapify(s, i, n, first);
        }

        // Extract elements from heap
        for (var i = last - 1; i > first; i--)
        {
            // Save max (root) and the last element, then sift down the last element
            var max = s.Read(first);
            var lastVal = s.Read(i);
            Heapify(s, first, i - first, first, lastVal);
            s.Write(i, max);
        }
    }

    /// <summary>
    /// Restores the heap property using Floyd's improved heap construction algorithm.
    /// This method first descends to a leaf level by always selecting the larger child,
    /// then sifts the original value up to its correct position.
    /// This reduces comparisons by ~25% compared to standard bottom-up heapify
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="s">The SortSpan containing the elements and context for tracking operations.</param>
    /// <param name="root">The index of the root node of the subtree to heapify.</param>
    /// <param name="size">The size of the heap (number of elements to consider).</param>
    /// <param name="offset">The starting index offset for the heap within the span.</param>
    /// <remarks>
    /// Floyd's algorithm reduces the number of comparisons during heap construction by ~25%.
    /// Phase 1: Percolate down to a leaf by always taking the larger child (no key comparison).
    /// Phase 2: Sift up the original root value to its correct position.
    /// <para>Time Complexity: O(log n) - Same asymptotic complexity but fewer comparisons in practice.</para>
    /// <para>Space Complexity: O(1) - Uses iteration instead of recursion.</para>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void FloydHeapify<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int root, int size, int offset)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var rootValue = s.Read(root);
        var hole = root;

        // Phase 1: Percolate down to a leaf, always taking the larger child
        var child = 2 * (hole - offset) + 1 + offset;
        while (child < offset + size)
        {
            // Find larger child
            if (child + 1 < offset + size && s.Compare(child + 1, child) > 0)
            {
                child++;
            }

            // Move larger child up
            s.Write(hole, s.Read(child));
            hole = child;
            child = 2 * (hole - offset) + 1 + offset;
        }

        // Phase 2: Sift up the original root value to its correct position
        var parent = offset + (hole - offset - 1) / 2;
        while (hole > root && s.Compare(rootValue, s.Read(parent)) > 0)
        {
            s.Write(hole, s.Read(parent));
            hole = parent;
            parent = offset + (hole - offset - 1) / 2;
        }
        s.Write(hole, rootValue);
    }

    /// <summary>
    /// Restores the heap property for a subtree rooted at the specified index using iterative sift-down.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="s">The SortSpan containing the elements and context for tracking operations.</param>
    /// <param name="root">The index of the root node of the subtree to heapify.</param>
    /// <param name="size">The size of the heap (number of elements to consider).</param>
    /// <param name="offset">The starting index offset for the heap within the span.</param>
    /// <remarks>
    /// This method implements the sift-down operation to maintain the max-heap property using the hole-based approach.
    /// It saves the root value, descends by moving the larger child up into the hole at each level,
    /// then writes the saved value at its final position. This avoids triple-write swaps.
    /// <para>Time Complexity: O(log n) - Worst case traverses from root to leaf (height of the tree).</para>
    /// <para>Space Complexity: O(1) - Uses iteration instead of recursion.</para>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Heapify<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int root, int size, int offset, T value)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var hole = root;

        while (true)
        {
            var left = 2 * (hole - offset) + 1 + offset;
            var right = left + 1;

            if (left >= offset + size) break;

            // Find larger child
            var largest = (right < offset + size && s.Compare(right, left) > 0) ? right : left;

            // If value is already >= largest child, heap property is satisfied
            if (s.Compare(value, largest) >= 0) break;

            // Move larger child up to fill the hole
            s.Write(hole, s.Read(largest));
            hole = largest;
        }

        s.Write(hole, value);
    }
}
