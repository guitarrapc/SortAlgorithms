using SortAlgorithm.Contexts;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// ヒープ構築と削除の両方で「ボトムアップ」アプローチを使用するヒープソートの最適化版です。
/// sift のたびに、まず値と比較せずに大きい方の子をたどって葉まで降り、そこから値を上へ戻す位置を探すことで、標準的なヒープソートよりも比較回数を大幅に削減します。
/// <br/>
/// An optimized variant of heap sort that uses a "bottom-up" approach for both heap construction and deletion.
/// Every sift first descends to a leaf along the larger children without comparing the sifted value, then climbs back to the value's position, significantly reducing the number of comparisons compared to standard heap sort.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Bottom-Up Heapsort:</strong></para>
/// <list type="number">
/// <item><description><strong>Heap Property Maintenance:</strong> For a max-heap, every parent node must be greater than or equal to its children.
/// For array index i (0-based), left child is at 2i+1 and right child is at 2i+2.
/// This implementation maintains this property through the bottom-up sift in both phases.</description></item>
/// <item><description><strong>Build Heap Phase:</strong> Floyd's construction sifts every internal node, from the last one (⌊n/2⌋ - 1) to the root, using the bottom-up sift. It runs in O(n) time.</description></item>
/// <item><description><strong>Extract Max Phase (Bottom-Up Deletion):</strong> Repeatedly performs the following:
/// (a) Read the last element, then move the root (maximum) into the last position, which leaves a hole at the root.
/// (b) Apply the bottom-up sift from the root hole:
///     Phase 1 - Leaf search: descend to a leaf by moving the larger child up into the hole at each level (one comparison per level, never against the sifted element).
///     Phase 2 - Bottom-up search: climb from the leaf while the parent is smaller than the sifted element, moving each such parent back down, then write the element.</description></item>
/// <item><description><strong>Key Difference from Standard HeapSort:</strong> Standard heap sort uses top-down sift-down (comparing the sinking element at each level).
/// Bottom-up heap sort defers element comparison: it first descends to a leaf unconditionally, then performs upward comparisons only.
/// The element being sifted is usually a former leaf and belongs near the bottom, so the climb is short on average.
/// This asymmetry (down without comparing the element, up with it) is the defining characteristic.</description></item>
/// <item><description><strong>Termination Guarantee:</strong> After n-1 extract-max operations, the heap is reduced to size 1, and all elements are in sorted order.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Heap / Selection</description></item>
/// <item><description>Stable      : No (heap operations do not preserve relative order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space - uses only a few temporary variables)</description></item>
/// <item><description>Best case   : Ω(n log n) - every sift descends to a leaf, even when all keys are equal</description></item>
/// <item><description>Average case: Θ(n log n) - Build heap O(n) + n-1 deletions of log n + O(1) comparisons each</description></item>
/// <item><description>Worst case  : O(n log n) - at most 1.5n log n + O(n) comparisons (Wegener)</description></item>
/// <item><description>Comparisons : ~n log n - one per level on the way down, plus on average a constant number on the way up</description></item>
/// <item><description>Swaps       : 0 - the maximum reaches the end by one read and one write, and every sift moves elements through a hole</description></item>
/// <item><description>Index Reads : Θ(n log n) - the descent to a leaf and the climb back up each touch one node per level</description></item>
/// <item><description>Index Writes: Θ(n log n) - the descent writes one element per level and the climb moves back the few it passes, plus n root-to-end writes</description></item>
/// <item><description>Space       : O(1) - the heap occupies the input span itself; only a few temporary variables are used</description></item>
/// </list>
/// <para><strong>Cache behaviour:</strong> poor locality — the heap's parent/child index arithmetic jumps through memory, so extraction misses cache far more than a sequential algorithm.</para>
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
/// <item><description>Standard sift-down: 2 comparisons per level (the two children, then the sinking element against the larger one) × log n levels = 2 log n comparisons</description></item>
/// <item><description>Bottom-up sift-down: 1 comparison per level during descent (child-child only) × log n levels, plus on average O(1) comparisons during ascent = log n + O(1) comparisons</description></item>
/// <item><description>Overall reduction: From ~2n log n to ~n log n comparisons, about half</description></item>
/// </list>
/// <para><strong>Difference from Standard HeapSort:</strong></para>
/// <list type="bullet">
/// <item><description>Standard <see cref="HeapSort"/>: top-down sift-down in both phases</description></item>
/// <item><description>Bottom-Up HeapSort: bottom-up sift in both phases</description></item>
/// <item><description>Key innovation: Applying the bottom-up principle to the deletion phase as well</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>The maximum moves to the end and the former last element is sifted from the root hole, as in libstdc++'s <c>__pop_heap</c>, so no swap is needed</description></item>
/// <item><description>The descent moves each larger child up as it goes, as libstdc++'s <c>__adjust_heap</c> and libc++'s <c>__floyd_sift_down</c> do.
/// Wegener's formulation finds the leaf first and shifts the path once at the end; both take the same comparisons.</description></item>
/// <item><description>The larger child is selected by adding the comparison result to the index rather than branching on it, as in <see cref="HeapSort"/>
/// (float, double and Half keep the branch). The descent then has no data-dependent branch at all, because it never compares the sifted element.</description></item>
/// <item><description>Builds max-heap for ascending sort (min-heap would produce descending order)</description></item>
/// <item><description>All operations are iterative (no recursion) for better performance and stack safety</description></item>
/// <item><description>Saving comparisons pays off most when a comparison is expensive; with cheap keys the extra moves of the climb can cancel the gain</description></item>
/// </list>
/// <para><strong>Historical Context:</strong></para>
/// <para>
/// Bottom-up heap sort was presented by Ingo Wegener at MFCS 1990, with the journal version in Theoretical Computer Science
/// in 1993, as an improvement over Floyd's heap construction algorithm.
/// While Floyd (1964) optimized the heap construction phase, Wegener extended the bottom-up principle to the
/// extraction phase as well, achieving further comparison savings. This makes it one of the most
/// comparison-efficient variants of heap sort, though it remains primarily of theoretical interest due to
/// poor cache behavior inherent in heap structures.
/// </para>
/// <para>
/// The idea is older than Wegener's papers. D. J. Bernstein notes that it "is the same as Floyd's bottom-up algorithm in Knuth
/// exercise 5.2.3-18" (TAOCP Vol. 3, 1973), and Carlsson published a related variant in 1987 ("A variant of heapsort with almost
/// optimal number of comparisons", Information Processing Letters 24 (4)).
/// </para>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Heapsort</para>
/// <para>Paper: Wegener, I. (1990). "Bottom-up-heap sort, a new variant of heap sort beating on average quick sort (if n is not very small)". MFCS 1990, LNCS 452: 516-522. https://doi.org/10.1007/BFb0029650</para>
/// <para>Paper: Wegener, I. (1993). "BOTTOM-UP-HEAPSORT, a new variant of HEAPSORT beating, on an average, QUICKSORT (if n is not very small)". Theoretical Computer Science 118 (1): 81-98. https://doi.org/10.1016/0304-3975(93)90364-Y</para>
/// </remarks>
public static class BottomupHeapSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by BottomupHeapSortTests, which derives from SortTestsBase.</remarks>
    public static bool IsStable => false;

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

        // Build heap using Floyd's construction with the bottom-up sift
        s.Context.OnPhase(SortPhase.HeapBuild, first, last - 1);
        for (var i = first + n / 2; i-- > first;)
        {
            BottomUpSift(s, i, n, first, s.Read(i));
        }

        // Extract elements from heap using bottom-up deletion
        var totalExtractions = n - 1;
        for (var i = last - 1; i > first; i--)
        {
            s.Context.OnPhase(SortPhase.HeapExtract, last - i, totalExtractions);

            // Move the current root (max) to the end; the former last element is sifted from the root hole
            var value = s.Read(i);
            s.Write(i, s.Read(first));

            // Index i now holds the confirmed max value
            s.Context.OnRole(i, BUFFER_MAIN, RoleType.CurrentMax);

            BottomUpSift(s, first, i - first, first, value);

            s.Context.OnRole(i, BUFFER_MAIN, RoleType.None);
        }
    }

    /// <summary>
    /// Places <paramref name="value"/> into the heap whose root position <paramref name="root"/> is a hole.
    /// The hole first descends to a leaf along the larger children, then <paramref name="value"/> climbs from that leaf to its position.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <param name="s">The SortSpan containing the elements and context for tracking operations.</param>
    /// <param name="root">The index of the hole where the sift starts.</param>
    /// <param name="size">The size of the heap (number of elements to consider).</param>
    /// <param name="offset">The starting index offset for the heap within the span.</param>
    /// <param name="value">The element to place. Whatever <paramref name="root"/> currently holds is overwritten.</param>
    /// <remarks>
    /// This is the defining step of Bottom-Up HeapSort. The descent never compares <paramref name="value"/>;
    /// only the climb does, and it is short on average because a former leaf belongs near the bottom.
    /// <para>Time Complexity: O(log n) - log n comparisons on the way down, O(1) on average on the way up.</para>
    /// <para>Space Complexity: O(1) - Uses iteration with constant extra space.</para>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void BottomUpSift<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int root, int size, int offset, T value)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var end = offset + size;
        var hole = root;

        // Phase 1: Descend to a leaf, moving the larger child up into the hole at each level
        var child = 2 * (hole - offset) + 1 + offset;
        while (child < end)
        {
            child = LargerChild(s, child, end);
            s.Write(hole, s.Read(child));
            hole = child;
            child = 2 * (hole - offset) + 1 + offset;
        }

        // Phase 2: Climb while the parent is smaller than value, moving each such parent back down
        while (hole > root)
        {
            var parent = offset + (hole - offset - 1) / 2;
            if (s.IsValueLessOrEqual(value, parent)) break;
            s.Write(hole, s.Read(parent));
            hole = parent;
        }

        s.Write(hole, value);
    }

    /// <summary>
    /// Returns the index of the larger child of a node whose left child is <paramref name="left"/>.
    /// The right child exists only when it lies below <paramref name="end"/>.
    /// </summary>
    /// <remarks>
    /// Only the bounds check branches. The comparison result is added to the index, as in Rust's heapsort,
    /// because a branch on it is mispredicted about half the time on random input.
    /// Floating-point types keep the branch for the reason given in <see cref="HeapSort"/>'s LargerChild.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int LargerChild<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (left + 1 >= end) return left;
        if (typeof(T) == typeof(float) || typeof(T) == typeof(double) || typeof(T) == typeof(Half))
        {
            if (s.IsGreaterAt(left + 1, left)) left++;
            return left;
        }
        return left + (s.IsGreaterAt(left + 1, left) ? 1 : 0);
    }
}
