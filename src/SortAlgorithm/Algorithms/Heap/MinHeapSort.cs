using SortAlgorithm.Contexts;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 配列から、常に最小の要素をルートにもつヒープ（最小二分ヒープ）を作成します（この時点で不安定）。
/// その後、ルート（最小値）を末尾要素と交換し、ヒープサイズを縮小してヒープ構造を再維持します。
/// これにより降順の配列が得られるため、最後に反転して昇順にします。
/// <br/>
/// Builds a min-heap (binary heap) from the array where the root always contains the minimum element (which is inherently unstable).
/// Then, swaps the root (minimum) with the last element, reduces the heap size, and re-establishes the min-heap property.
/// This produces a descending array, which is then reversed to obtain ascending order.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct MinHeapSort:</strong></para>
/// <list type="number">
/// <item><description><strong>Min-Heap Property Maintenance:</strong> Every parent node must be less than or equal to its children.
/// For array index i, left child is at 2i+1 and right child is at 2i+2. This implementation correctly maintains this property through iterative heapify operations.</description></item>
/// <item><description><strong>Build Heap Phase:</strong> The initial min-heap construction starts from the last non-leaf node (n/2 - 1) and heapifies downward to index 0.
/// This implementation uses Floyd's improved heap construction algorithm, which reduces comparisons by ~25% compared to standard bottom-up heapify.
/// This bottom-up approach runs in O(n) time.</description></item>
/// <item><description><strong>Extract Min Phase:</strong> Repeatedly reads the root (minimum) and the last element, sifts the last element down via Heapify, then writes the min to the end.
/// This produces a descending order because the smallest elements are moved to the end first.</description></item>
/// <item><description><strong>Reverse Phase:</strong> After all extractions, the array is in descending order. A simple O(n) reversal converts it to ascending order.</description></item>
/// <item><description><strong>Floyd's Heapify Optimization:</strong> During heap construction, uses Floyd's two-phase algorithm:
/// Phase 1 percolates down to a leaf by selecting smaller children without comparing keys.
/// Phase 2 sifts the original value up to its correct position. This reduces the average number of comparisons.</description></item>
/// <item><description><strong>Standard Heapify:</strong> During extraction phase, uses hole-based iterative sift-down.
/// The replacement value (read from the last position) is passed in directly; it descends by moving the smaller child up into the hole, then writes the value at its correct position.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Heap / Selection</description></item>
/// <item><description>Stable      : No (heap operations do not preserve relative order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space)</description></item>
/// <item><description>Best case   : Ω(n log n) - Even for sorted input, heap construction, extraction, and reversal are required</description></item>
/// <item><description>Average case: Θ(n log n) - Build heap O(n) + n-1 extractions with O(log n) heapify each + O(n) reversal</description></item>
/// <item><description>Worst case  : O(n log n) - Guaranteed upper bound regardless of input distribution</description></item>
/// <item><description>Comparisons : ~2n log n - Approximately 2 comparisons per heapify (left and right child checks)</description></item>
/// <item><description>Swaps       : ~n/2 - Only from the final reversal step; extraction uses Read+Heapify+Write pattern with 0 swaps</description></item>
/// <item><description>Cache       : Poor locality - Heap structure causes frequent cache misses due to non-sequential access</description></item>
/// </list>
/// <para><strong>Difference from <see cref="HeapSort"/>:</strong></para>
/// <list type="bullet">
/// <item><description>Uses min-heap (parent ≤ children) instead of max-heap (parent ≥ children)</description></item>
/// <item><description>Extraction phase produces descending order, requiring a final O(n) reversal step</description></item>
/// <item><description>Total work is O(n log n) + O(n) = O(n log n), same asymptotic complexity as HeapSort</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>Uses iterative heapify (loop) instead of recursive for better performance and stack safety</description></item>
/// <item><description>Builds min-heap then reverses for ascending sort (max-heap directly produces ascending order without reversal)</description></item>
/// <item><description>Comparison-based algorithm: requires O(n log n) comparisons in all cases</description></item>
/// <item><description>Despite O(n log n) guarantee, often slower than HeapSort in practice due to extra reversal step and poor cache performance</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Heapsort</para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Heap_(data_structure)</para>
/// </remarks>
public static class MinHeapSort
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

        // Build min-heap
        s.Context.OnPhase(SortPhase.HeapBuild, first, last - 1);
        for (var i = first + (n / 2) - 1; i >= first; i--)
            FloydHeapify(s, i, n, first);

        // Extract elements from min-heap (produces descending order)
        var totalExtractions = n - 1;
        for (var i = last - 1; i > first; i--)
        {
            s.Context.OnPhase(SortPhase.HeapExtract, last - i, totalExtractions);
            s.Context.OnRole(first, BUFFER_MAIN, RoleType.CurrentMin);

            // Save min (root) and the last element, then sift down the last element
            var min = s.Read(first);
            var lastVal = s.Read(i);
            Heapify(s, first, i - first, first, lastVal);
            s.Write(i, min);

            s.Context.OnRole(first, BUFFER_MAIN, RoleType.None);
        }

        // Reverse the range to convert descending order to ascending order
        s.Context.OnPhase(SortPhase.Reverse, first, last - 1);
        Reverse(s, first, last - 1);
    }

    /// <summary>
    /// Restores the min-heap property using Floyd's improved heap construction algorithm.
    /// This method first descends to a leaf level by always selecting the smaller child,
    /// then sifts the original value up to its correct position.
    /// This typically reduces the number of comparisons compared to standard sift-down heap construction.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of the comparer.</typeparam>
    /// <typeparam name="TContext">The type of the sort context.</typeparam>
    /// <param name="s">The SortSpan containing the elements and context for tracking operations.</param>
    /// <param name="root">The index of the root node of the subtree to heapify.</param>
    /// <param name="size">The size of the heap (number of elements to consider).</param>
    /// <param name="offset">The starting index offset for the heap within the span.</param>
    /// <remarks>
    /// Floyd's algorithm reduces the number of comparisons during heap construction by ~25%.
    /// Phase 1: Percolate down to a leaf by always taking the smaller child (no key comparison).
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

        // Phase 1: Percolate down to a leaf, always taking the smaller child
        var child = 2 * (hole - offset) + 1 + offset;
        while (child < offset + size)
        {
            // Find smaller child
            if (child + 1 < offset + size && s.IsLessAt(child + 1, child))
            {
                child++;
            }

            // Move smaller child up
            s.Write(hole, s.Read(child));
            hole = child;
            child = 2 * (hole - offset) + 1 + offset;
        }

        // Phase 2: Sift up the original root value to its correct position
        var parent = offset + (hole - offset - 1) / 2;
        while (hole > root && s.IsLessThan(rootValue, s.Read(parent)))
        {
            s.Write(hole, s.Read(parent));
            hole = parent;
            parent = offset + (hole - offset - 1) / 2;
        }
        s.Write(hole, rootValue);
    }

    /// <summary>
    /// Restores the min-heap property for a subtree rooted at the specified index using iterative sift-down.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of the comparer.</typeparam>
    /// <typeparam name="TContext">The type of the sort context.</typeparam>
    /// <param name="s">The SortSpan containing the elements and context for tracking operations.</param>
    /// <param name="root">The index of the root node of the subtree to heapify.</param>
    /// <param name="size">The size of the heap (number of elements to consider).</param>
    /// <param name="offset">The starting index offset for the heap within the span.</param>
    /// <param name="value">The value to sift down from the root position. Passed by the caller to avoid a redundant Read inside this method.</param>
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

            // Find smaller child
            var smallest = (right < offset + size && s.IsLessAt(right, left)) ? right : left;

            // If value is already <= smallest child, min-heap property is satisfied
            if (s.Compare(value, smallest) <= 0) break;

            // Move smaller child up to fill the hole
            s.Write(hole, s.Read(smallest));
            hole = smallest;
        }

        s.Write(hole, value);
    }

    /// <summary>
    /// Reverses the elements in the range [lo..hi] (inclusive) in-place.
    /// Used to convert the descending order produced by min-heap extraction into ascending order.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Reverse<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int lo, int hi)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (lo < hi)
        {
            s.Swap(lo, hi);
            lo++;
            hi--;
        }
    }
}
