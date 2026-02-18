using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// ヒープ構築と削除の両方で「ボトムアップ」アプローチを使用するヒープソートの最適化版です。
/// ヒープ構築にはFloydのアルゴリズムを、削除フェーズには真のボトムアップsift-downを使用し、標準的なヒープソートよりも比較回数を大幅に削減します。
/// <br/>
/// An optimized variant of heap sort that uses a "bottom-up" approach for both heap construction and deletion.
/// It employs Floyd's algorithm for heap construction and true bottom-up sift-down for the deletion phase, significantly reducing the number of comparisons compared to standard heap sort.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Bottom-Up Heapsort:</strong></para>
/// <list type="number">
/// <item><description><strong>Heap Property Maintenance:</strong> For a max-heap, every parent node must be greater than or equal to its children.
/// For array index i (0-based), left child is at 2i+1 and right child is at 2i+2.
/// This implementation maintains this property through both Floyd's heapify and bottom-up sift-down operations.</description></item>
/// <item><description><strong>Build Heap Phase (Floyd's Algorithm):</strong> The initial heap construction starts from the last non-leaf node (⌊n/2⌋ - 1) and processes nodes toward the root.
/// For each node, Floyd's two-phase approach is used:
/// Phase 1 - Percolate down to a leaf by always selecting the larger child (without comparing the node's key).
/// Phase 2 - Sift up the original node value to its correct position.
/// This runs in O(n) time with approximately 1.5n comparisons (25% fewer than standard heapify).</description></item>
/// <item><description><strong>Extract Max Phase (Bottom-Up Deletion):</strong> Repeatedly performs the following:
/// (a) Swap the root (maximum) with the last element and reduce heap size.
/// (b) Apply bottom-up sift-down to restore heap property:
///     Phase 1 - Create a "hole" at root and descend to a leaf by selecting larger children (without comparing the replacement element).
///     Phase 2 - Sift up the replacement element (formerly last element) from the leaf to its correct position.
/// This reduces comparisons from 2 log n (standard sift-down) to approximately log n + log log n per deletion.</description></item>
/// <item><description><strong>Key Difference from Standard HeapSort:</strong> Standard heap sort uses top-down sift-down (comparing the sinking element at each level).
/// Bottom-up heap sort defers element comparison: it first descends to a leaf unconditionally, then performs upward comparisons only.
/// This asymmetry (down without comparison, up with comparison) is the defining characteristic.</description></item>
/// <item><description><strong>Termination Guarantee:</strong> After n-1 extract-max operations, the heap is reduced to size 1, and all elements are in sorted order.
/// The algorithm terminates in exactly O(n log n) time with provably fewer comparisons than standard heap sort.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Heap / Selection</description></item>
/// <item><description>Stable      : No (swapping elements by index breaks relative order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space - uses only a few temporary variables)</description></item>
/// <item><description>Best case   : Ω(n log n) - Even for sorted input, heap construction O(n) and extraction O(n log n) are required</description></item>
/// <item><description>Average case: Θ(n log n) - Build heap 1.5n comparisons + extraction (n-1)(log n + log log n) comparisons</description></item>
/// <item><description>Worst case  : O(n log n) - Guaranteed upper bound regardless of input distribution</description></item>
/// <item><description>Comparisons : ~n log n - Approximately 30-40% fewer comparisons than standard heap sort</description></item>
/// <item><description>Swaps       : ~n log n - Similar to standard heap sort (dominated by extraction phase)</description></item>
/// <item><description>Cache       : Poor locality - Heap structure causes frequent cache misses due to non-sequential access</description></item>
/// </list>
/// <para><strong>Why "Bottom-Up"?:</strong></para>
/// <para>
/// The term "bottom-up" refers to the strategy of descending to a leaf level first (the "bottom" of the heap)
/// before comparing the element being inserted/deleted. This contrasts with "top-down" sift-down, which
/// compares the sinking element at each level during descent.
/// </para>
/// <para>
/// In bottom-up sift-down:
/// 1. A "hole" is created and pushed down to a leaf by selecting larger children (no key comparison).
/// 2. The element is then inserted at the leaf and sifted up to its correct position (with key comparisons).
/// This approach reduces comparisons because most elements in a heap are near leaves (2^(h-1) nodes at depth h-1),
/// and deferring comparisons until the upward phase exploits this structure.
/// </para>
/// <para><strong>Comparison Reduction Analysis:</strong></para>
/// <list type="bullet">
/// <item><description>Standard sift-down: 2 comparisons per level (left child, right child, then max comparison) × log n levels = 2 log n comparisons</description></item>
/// <item><description>Bottom-up sift-down: 1 comparison per level during descent (child-child only) × log n levels + log log n comparisons during ascent = log n + log log n comparisons</description></item>
/// <item><description>Overall reduction: From ~2n log n to ~n log n comparisons (approximately 30-40% improvement)</description></item>
/// </list>
/// <para><strong>Difference from Standard HeapSort:</strong></para>
/// <list type="bullet">
/// <item><description>Standard HeapSort: Uses Floyd's algorithm for construction, standard top-down sift-down for extraction</description></item>
/// <item><description>Bottom-Up HeapSort: Uses Floyd's algorithm for construction, bottom-up sift-down for extraction</description></item>
/// <item><description>Key innovation: Applying the bottom-up principle to the deletion phase as well</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>Uses Floyd's two-phase heapify during heap construction for optimal comparison count</description></item>
/// <item><description>Uses true bottom-up sift-down during extraction phase (descend to leaf, then sift up)</description></item>
/// <item><description>Builds max-heap for ascending sort (min-heap would produce descending order)</description></item>
/// <item><description>All operations are iterative (no recursion) for better performance and stack safety</description></item>
/// <item><description>Comparison-based algorithm: requires O(n log n) comparisons but fewer than other O(n log n) sorts</description></item>
/// <item><description>Despite fewer comparisons, may be slower than Quicksort in practice due to poor cache performance</description></item>
/// </list>
/// <para><strong>Historical Context:</strong></para>
/// <para>
/// Bottom-up heap sort was introduced by Ingo Wegener in 1993 as an improvement over Floyd's heap construction algorithm.
/// While Floyd (1964) optimized the heap construction phase, Wegener extended the bottom-up principle to the
/// extraction phase as well, achieving further comparison savings. This makes it one of the most
/// comparison-efficient variants of heap sort, though it remains primarily of theoretical interest due to
/// poor cache behavior inherent in heap structures.
/// </para>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Heapsort</para>
/// <para>Paper: https://pdf.sciencedirectassets.com/271538/1-s2.0-S0304397500X04148/1-s2.0-030439759390364Y/main.pdf</para>
/// </remarks>
public static class BottomupHeapSort
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
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T, TContext>(Span<T> span, TContext context)
        where T : IComparable<T>
        where TContext : ISortContext
        => Sort(span, 0, span.Length, new ComparableComparer<T>(), context);

    /// <summary>
    /// Sorts the elements in the specified span using the provided comparer and sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context)
        where T : IComparable<T>
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
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = last - first;

        // Build heap using Floyd's bottom-up algorithm
        for (var i = first + n / 2 - 1; i >= first; i--)
        {
            FloydHeapify(s, i, n, first);
        }

        // Extract elements from heap using bottom-up deletion
        for (var i = last - 1; i > first; i--)
        {
            // Move current root (max) to end
            s.Swap(first, i);

            // Re-heapify using true bottom-up sift-down
            BottomUpSiftDown(s, first, i - first, first);
        }
    }

    /// <summary>
    /// Restores the heap property using true bottom-up sift-down for deletion phase.
    /// This method implements the characteristic bottom-up deletion:
    /// 1. Save the element at root (which is the former last element after swap in SortCore)
    /// 2. Create a "hole" at root and descend to a leaf by selecting larger children (no key comparison)
    /// 3. Place the saved element at the leaf position
    /// 4. Sift up the element to its correct position
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="s">The SortSpan containing the elements and context for tracking operations.</param>
    /// <param name="root">The index of the root node to start sift-down.</param>
    /// <param name="size">The size of the heap (number of elements to consider).</param>
    /// <param name="offset">The starting index offset for the heap within the span.</param>
    /// <remarks>
    /// This is the defining characteristic of Bottom-Up HeapSort's deletion phase.
    /// Unlike Floyd's heapify (used in construction), this method:
    /// - Descends to a leaf without comparing the element being inserted
    /// - Only performs comparisons during the upward sift phase
    /// - Reduces comparisons in deletion from 2 log n to log n + log log n on average
    /// <para>Time Complexity: O(log n) - But with fewer comparisons than standard sift-down</para>
    /// <para>Space Complexity: O(1) - Uses iteration with constant extra space</para>
    /// </remarks>
    private static void BottomUpSiftDown<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int root, int size, int offset)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (size <= 1) return;

        // Save the element at root after swap (which is the former last element that needs to be reinserted)
        var value = s.Read(root);
        var hole = root;

        // Phase 1: Descend to a leaf level by always selecting the larger child (no key comparison with 'value')
        while (true)
        {
            var left = 2 * (hole - offset) + 1 + offset;
            var right = left + 1;

            // If no children, we've reached a leaf
            if (left >= offset + size) break;

            // Select the larger child to continue descent
            var largerChild = left;
            if (right < offset + size && s.Compare(right, left) > 0)
            {
                largerChild = right;
            }

            // Move the larger child up to fill the hole
            s.Write(hole, s.Read(largerChild));
            hole = largerChild;
        }

        // Phase 2: Sift up the saved value from the leaf position to its correct position
        // This is similar to insertion into a max-heap
        while (hole > root)
        {
            var parent = offset + (hole - offset - 1) / 2;

            // If parent is greater than or equal to value, we found the correct position
            // Use Compare(value, parent) which compares value against span[parent]
            if (s.Compare(value, parent) <= 0)
            {
                break;
            }

            // Move parent down
            s.Write(hole, s.Read(parent));
            hole = parent;
        }

        // Place the value in its final position
        s.Write(hole, value);
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
        while (hole > root && s.Compare(rootValue, parent) > 0)
        {
            s.Write(hole, s.Read(parent));
            hole = parent;
            parent = offset + (hole - offset - 1) / 2;
        }
        s.Write(hole, rootValue);
    }
}
