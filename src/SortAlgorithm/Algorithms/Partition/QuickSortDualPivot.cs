using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 2つのピボットを使用して配列を3つの領域に分割する分割統治法のソートアルゴリズムです。
/// 単一ピボットのQuickSortと比較して、より均等な分割により再帰の深さを浅くし、キャッシュ効率を高めることで高速化を実現します。
/// 本実装はVladimir Yaroslavskiy (2009)のDual-Pivot QuickSort論文に基づいています。
/// <br/>
/// A divide-and-conquer sorting algorithm that uses two pivots to partition the array into three regions.
/// Compared to single-pivot QuickSort, it achieves faster performance through more balanced partitioning, reducing recursion depth and improving cache efficiency.
/// This implementation is based on Vladimir Yaroslavskiy's (2009) Dual-Pivot QuickSort paper.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Dual-Pivot QuickSort:</strong></para>
/// <list type="number">
/// <item><description><strong>Pivot Selection and Ordering (Adaptive 5-Sample Method):</strong> Two pivots (p1, p2) are selected using an adaptive strategy:
/// <list type="bullet">
/// <item><description><strong>For arrays &lt; 47 elements:</strong> Use simple method (leftmost and rightmost elements as pivots, ensuring p1 ≤ p2)</description></item>
/// <item><description><strong>For arrays ≥47 elements:</strong> Use Java's proven 5-sample strategy:
/// <list type="bullet">
/// <item><description>Sample 5 elements at evenly distributed positions: left+length/7, left+2*length/7, middle, right-2*length/7, right-length/7</description></item>
/// <item><description>Sort these 5 elements using 2-pass bubble sort (7-9 comparisons, same as Java's implementation)</description></item>
/// <item><description>Choose the 2nd smallest and 4th smallest as pivot1 and pivot2</description></item>
/// <item><description>This yields approximately 1/7, 3/7, 5/7 division ratios, close to the ideal 1/3, 2/3 for dual-pivot</description></item>
/// </list>
/// </description></item>
/// </list>
/// The pivots satisfy p1 ≤ p2 by construction. This method dramatically reduces worst-case probability and handles sorted/reverse-sorted data efficiently.</description></item>
/// <item><description><strong>Three-Way Partitioning (Neither Hoare nor Lomuto):</strong> This algorithm uses a specialized 3-way partitioning scheme designed for dual-pivot quicksort.
/// Unlike Hoare partition (bidirectional scan with two pointers) or Lomuto partition (single-direction scan),
/// this approach performs a left-to-right scan with three boundary pointers (l, k, g) to partition the array into three regions:
/// <list type="bullet">
/// <item><description>Left region: elements &lt; p1 (indices [left, l-1])</description></item>
/// <item><description>Middle region: elements where p1 ≤ element ≤ p2 (indices [l+1, g-1])</description></item>
/// <item><description>Right region: elements &gt; p2 (indices [g+1, right])</description></item>
/// </list>
/// The partitioning loop (lines 53-70) maintains these invariants:
/// - Elements in [left+1, l-1] are &lt; p1
/// - Elements in [l, k-1] are in [p1, p2]
/// - Elements in [g+1, right-1] are &gt; p2
/// - Element at index k is currently being examined
/// This is the standard dual-pivot partitioning method introduced by Yaroslavskiy.</description></item>
/// <item><description><strong>Pivot Placement:</strong> After partitioning, pivots are moved to their final positions (lines 74-75):
/// - p1 is swapped with the element at position l (boundary of left region)
/// - p2 is swapped with the element at position g (boundary of right region)
/// This ensures pivots are correctly positioned between their respective regions.</description></item>
/// <item><description><strong>Recursive Division:</strong> The algorithm recursively sorts three independent regions (lines 78-83):
/// - Left region: [left, l-1]
/// - Middle region: [l+1, g-1] (only if p1 &lt; p2, i.e., pivots are distinct)
/// - Right region: [g+1, right]
/// Base case: when right ≤ left, the region has ≤ 1 element and is trivially sorted.</description></item>
/// <item><description><strong>Termination:</strong> The algorithm terminates because:
/// - Each recursive call operates on a strictly smaller subarray (at least 2 elements are pivots)
/// - The base case (right ≤ left) is eventually reached for all subarrays
/// - Maximum recursion depth: O(log₃ n) on average, O(n) in worst case</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Partitioning (Divide and Conquer)</description></item>
/// <item><description>Partition   : 3-way partition (Yaroslavskiy's method - neither Hoare nor Lomuto)</description></item>
/// <item><description>Stable      : No (partitioning does not preserve relative order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(log n) auxiliary space for recursion stack)</description></item>
/// <item><description>Best case   : Θ(n log₃ n) - Balanced partitions (each region ≈ n/3)</description></item>
/// <item><description>Average case: Θ(n log₃ n) - Expected number of comparisons: 1.9n ln n ≈ 1.37n log₂ n (vs 2n ln n for single-pivot)</description></item>
/// <item><description>Worst case  : O(n²) - Occurs when partitioning is highly unbalanced (rare with dual pivots)</description></item>
/// <item><description>Comparisons : 1.9n ln n (average) - Each element compared with both pivots during partitioning</description></item>
/// <item><description>Swaps       : 0.6n ln n (average) - Fewer swaps than single-pivot due to better partitioning</description></item>
/// </list>
/// <para><strong>Advantages over Single-Pivot QuickSort:</strong></para>
/// <list type="bullet">
/// <item><description>More balanced partitions: log₃ n vs log₂ n recursion depth (≈37% reduction)</description></item>
/// <item><description>Fewer comparisons on average: 1.9n ln n vs 2n ln n (≈5% reduction)</description></item>
/// <item><description>Better cache locality: three regions fit better in CPU cache than two</description></item>
/// <item><description>Lower probability of worst-case behavior: dual pivots provide better sampling</description></item>
/// </list>
/// <para><strong>Yaroslavskiy 2009 Optimizations Implemented:</strong></para>
/// <list type="bullet">
/// <item><description><strong>Insertion Sort Fallback (TINY_SIZE = 17):</strong> Arrays smaller than 17 elements are sorted using insertion sort for better constant-factor performance.</description></item>
/// <item><description><strong>5-Sample Pivot Selection:</strong> For arrays ≥47 elements, uses 5-sample method to select pivots, reducing worst-case probability.</description></item>
/// <item><description><strong>Inner While Loop:</strong> Partitioning uses inner while loop to scan from right when element &gt; pivot2, matching Yaroslavskiy's specification.</description></item>
/// <item><description><strong>Equal Elements Optimization (DIST_SIZE = 13):</strong> When center region is large (&gt; length - 13) and pivots are different,
/// segregates elements equal to pivots from the center region before recursing. This improves performance on arrays with many duplicate values.</description></item>
/// <item><description><strong>Dual-Pivot Partitioning:</strong> Separate handling for equal pivots vs. different pivots cases.</description></item>
/// <item><description>Reference: https://web.archive.org/web/20151002230717/http://iaroslavski.narod.ru/quicksort/DualPivotQuicksort.pdf</description></item>
/// </list>
/// <para><strong>Differences from Java's DualPivotQuicksort (Java 7+):</strong></para>
/// <list type="bullet">
/// <item><description><strong>Core Algorithm:</strong> This implementation matches Yaroslavskiy's 2009 paper specification.</description></item>
/// <item><description><strong>Adaptive Algorithm Selection:</strong> Java's implementation adaptively selects from multiple algorithms:
/// <list type="bullet">
/// <item><description>Insertion Sort: Arrays ≤47 elements (we use ≤17)</description></item>
/// <item><description>Merge Sort: 47-286 elements with detected sorted runs (partial ordering)</description></item>
/// <item><description>Dual-Pivot QuickSort: ≥286 elements (general case)</description></item>
/// <item><description>Counting Sort: ≥3000 elements with small value range (e.g., byte arrays)</description></item>
/// </list>
/// This implementation uses Yaroslavskiy's original algorithm without additional adaptive selection.</description></item>
/// <item><description><strong>Duplicate Handling:</strong> Java uses 5-way partitioning to segregate elements equal to pivots.
/// This implementation uses Yaroslavskiy's 2009 approach with equal elements optimization.</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Quicksort</para>
/// <para>Paper: https://arxiv.org/abs/1310.7409 Average Case Analysis of Java 7's Dual Pivot Quicksort / Sebastian Wild, Markus E. Nebel</para>
/// </remarks>
public static class QuickSortDualPivot
{
    // Threshold for switching to 5-sample pivot selection
    // Below this size, simple pivot selection (left, right) is used
    // This value ensures sufficient spacing for 5-sample method (requires ~7 positions)
    private const int PivotThreshold = 47;

    // Threshold for switching to insertion sort (Yaroslavskiy 2009)
    // Arrays smaller than this size are sorted using insertion sort
    private const int TINY_SIZE = 17;

    // Threshold for equal elements optimization (Yaroslavskiy 2009)
    // When center region is larger than (length - DIST_SIZE) and pivots are different,
    // segregate elements equal to pivots from the center region
    private const int DIST_SIZE = 13;

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
    /// Sorts the subrange [first..last) using the provided comparer and context.
    /// This is the full-control version with explicit TContext type parameter.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use. Must implement <see cref="IComparer{T}"/>.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="span">The span containing elements to sort.</param>
    /// <param name="first">The inclusive start index of the range to sort.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    /// <param name="comparer">The comparer used to compare elements.</param>
    /// <param name="context">The sort context for tracking statistics and observations.</param>
    public static void Sort<T, TComparer, TContext>(Span<T> span, int first, int last, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        ArgumentOutOfRangeException.ThrowIfNegative(first);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(last, span.Length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(first, last);

        if (last - first <= 1) return;

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        SortCore(s, first, last - 1);
    }

    /// <summary>
    /// Sorts the subrange [first..last) using the provided sort context.
    /// This overload accepts a SortSpan directly for use by other algorithms that already have a SortSpan instance.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use. Must implement <see cref="IComparer{T}"/>.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="left">The inclusive start index of the range to sort.</param>
    /// <param name="right">The exclusive end index of the range to sort.</param>
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int right)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (right <= left) return;

        int length = right - left + 1;

        // For tiny arrays, use insertion sort (Yaroslavskiy 2009 optimization)
        if (length < TINY_SIZE)
        {
            InsertionSort.SortCore(s, left, right + 1);
            return;
        }

        // For small arrays, use simple pivot selection (left and right)
        if (length < PivotThreshold)
        {
            // Simple pivot selection: use left and right as pivots
            if (s.Compare(left, right) > 0)
            {
                s.Swap(left, right);
            }
        }
        else
        {
            // Phase 0. Choose pivots using 5-sample method (Java's DualPivotQuicksort)
            int seventh = (length >> 3) + (length >> 6) + 1; // ≈ length/7

            // Sample 5 evenly distributed elements
            int e3 = (left + right) >> 1; // middle
            int e2 = e3 - seventh;
            int e1 = e2 - seventh;
            int e4 = e3 + seventh;
            int e5 = e4 + seventh;

            // Sort these 5 elements using bubble sort (2 passes)
            // This is the same approach as Java's DualPivotQuicksort
            // Pass 1: bubble largest elements to the right
            if (s.Compare(e2, e1) < 0) s.Swap(e2, e1);
            if (s.Compare(e3, e2) < 0) s.Swap(e3, e2);
            if (s.Compare(e4, e3) < 0) s.Swap(e4, e3);
            if (s.Compare(e5, e4) < 0) s.Swap(e5, e4);

            // Pass 2: ensure complete ordering
            if (s.Compare(e2, e1) < 0) s.Swap(e2, e1);
            if (s.Compare(e3, e2) < 0) s.Swap(e3, e2);
            if (s.Compare(e4, e3) < 0) s.Swap(e4, e3);

            // Now: e1 <= e2 <= e3 <= e4 <= e5
            // Move pivots to the edges (will be swapped to final positions later)
            s.Swap(e2, left);
            s.Swap(e4, right);
        }

        // Phase 1. Partition array into three regions using dual pivots
        var less = left + 1;
        var great = right - 1;

        // Check if pivots are different (used for optimization later)
        var diffPivots = s.Compare(left, right) != 0;

        if (diffPivots)
        {
            // Partitioning with distinct pivots
            for (int k = less; k <= great; k++)
            {
                if (s.Compare(k, left) < 0)
                {
                    // Element < pivot1: move to left region
                    s.Swap(k, less);
                    less++;
                }
                else if (s.Compare(k, right) > 0)
                {
                    // Element > pivot2: scan from right to find position
                    while (s.Compare(great, right) > 0 && k < great)
                    {
                        great--;
                    }
                    s.Swap(k, great);
                    great--;

                    // Re-check swapped element
                    if (s.Compare(k, left) < 0)
                    {
                        s.Swap(k, less);
                        less++;
                    }
                }
                // else: pivot1 <= element <= pivot2, stays in middle
            }
        }
        else
        {
            // Partitioning with equal pivots (all elements either < or > pivot)
            for (int k = less; k <= great; k++)
            {
                if (s.Compare(k, left) == 0)
                    continue; // Element equals pivot, skip

                if (s.Compare(k, left) < 0)
                {
                    s.Swap(k, less);
                    less++;
                }
                else // element > pivot
                {
                    while (s.Compare(great, right) > 0 && k < great)
                    {
                        great--;
                    }
                    s.Swap(k, great);
                    great--;

                    if (s.Compare(k, left) < 0)
                    {
                        s.Swap(k, less);
                        less++;
                    }
                }
            }
        }

        // Swap pivots into their final positions
        s.Swap(left, less - 1);
        s.Swap(right, great + 1);

        // Phase 2. Sort left part
        SortCore(s, left, less - 2);

        // Phase 3. Equal elements optimization (Yaroslavskiy 2009)
        // When center region is large and pivots are different,
        // segregate elements equal to pivots before sorting center
        int centerLen = great - less + 1;
        if (centerLen > length - DIST_SIZE && diffPivots)
        {
            for (int k = less; k <= great; k++)
            {
                if (s.Compare(k, less - 1) == 0) // equals pivot1
                {
                    s.Swap(k, less);
                    less++;
                }
                else if (s.Compare(k, great + 1) == 0) // equals pivot2
                {
                    s.Swap(k, great);
                    great--;

                    // Re-check swapped element
                    if (s.Compare(k, less - 1) == 0)
                    {
                        s.Swap(k, less);
                        less++;
                    }
                }
            }
        }

        // Phase 4. Sort center part (only if pivots are different)
        if (diffPivots)
        {
            SortCore(s, less, great);
        }

        // Phase 5. Sort right part
        SortCore(s, great + 2, right);
    }
}
