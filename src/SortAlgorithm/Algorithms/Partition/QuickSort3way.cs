using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 1つのピボットを使用して等値要素を中央にまとめる「Dutch National Flag」分割統治法のソートアルゴリズムです。
/// 通常の2分割QuickSortとは異なり、配列を「ピボット未満」「ピボット等値」「ピボット超過」の3領域に分割します。
/// 等値要素が多い場合に特に高速で、等値の中央領域は再帰から除外されます。
/// <br/>
/// A divide-and-conquer sorting algorithm that collects equal elements in the center using a single pivot
/// (Dutch National Flag / 3-way partitioning). Unlike standard 2-partition QuickSort, it partitions the
/// array into three regions: less-than, equal-to, and greater-than the pivot. The equal center region
/// is excluded from further recursion, making it especially efficient when many duplicate keys are present.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct 3-Way QuickSort:</strong></para>
/// <list type="number">
/// <item><description><strong>Pivot Selection (Median-of-3):</strong> A pivot is chosen as the median of the
/// leftmost, middle, and rightmost elements. This simple median gives O(n log n) average performance and
/// avoids degenerate O(n²) behaviour on sorted or reverse-sorted inputs.</description></item>
/// <item><description><strong>Dutch National Flag Partitioning (Dijkstra):</strong> Three pointers (lt, i, gt)
/// scan the array and maintain the following invariant throughout the loop:
/// <list type="bullet">
/// <item><description>[left, lt-1] : elements strictly less than pivot</description></item>
/// <item><description>[lt, i-1]   : elements equal to pivot</description></item>
/// <item><description>[i, gt]     : unexamined elements</description></item>
/// <item><description>[gt+1, right]: elements strictly greater than pivot</description></item>
/// </list>
/// At each step: if s[i] &lt; pivot → swap(lt,i), lt++, i++;
/// if s[i] &gt; pivot → swap(i,gt), gt-- (do NOT advance i; the element from gt is unexamined);
/// if s[i] == pivot → i++.</description></item>
/// <item><description><strong>After Partition:</strong> [left, lt-1] &lt; pivot, [lt, gt] == pivot, [gt+1, right] &gt; pivot.
/// Only the strictly-less and strictly-greater regions are recursed. The equal region is permanently in place.</description></item>
/// <item><description><strong>Tail Recursion Optimization:</strong> Always recurse on the smaller of the two
/// non-equal regions and loop on the larger, bounding the call stack to O(log n).</description></item>
/// <item><description><strong>Termination:</strong> Each call reduces the active region by at least the size
/// of the equal partition (≥1). The base cases are handled by the insertion sort threshold and the
/// outer while loop guard (right &gt; left).</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Partitioning (Divide and Conquer)</description></item>
/// <item><description>Partition   : Dutch National Flag (Dijkstra's 3-way)</description></item>
/// <item><description>Stable      : No (swaps do not preserve relative order)</description></item>
/// <item><description>In-place    : Yes (O(log n) stack for recursion)</description></item>
/// <item><description>Best case   : Θ(n) - All elements equal (entire array is the equal region)</description></item>
/// <item><description>Average case: Θ(n log n) - Random distinct elements</description></item>
/// <item><description>Worst case  : O(n²) - Heavily skewed partitions with poor pivot selection</description></item>
/// <item><description>Duplicate keys: Θ(n log k) where k is the number of distinct values (k &lt;&lt; n → near O(n))</description></item>
/// </list>
/// <para><strong>Advantage over Standard QuickSort:</strong></para>
/// <list type="bullet">
/// <item><description>Standard 2-way QuickSort degrades to O(n²) on all-equal or few-distinct-key inputs.
/// 3-way partition solves this by segregating equal elements into a region that is never revisited.</description></item>
/// <item><description>Compared to DualPivot (2 pivots, 3 regions): DualPivot is faster on random data;
/// 3-way is faster when duplicates dominate. Conceptually, DualPivot is a "2-pivot strategy" while
/// 3-way is a "1-pivot equal-element strategy" — different goals.</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Dutch_national_flag_problem</para>
/// <para>Sedgewick &amp; Wayne, Algorithms 4th Ed., Section 2.3 (3-way partitioning)</para>
/// </remarks>
public static class QuickSort3way
{
    // Threshold for switching to insertion sort
    private const int InsertionSortThreshold = 16;

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
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use for element comparisons.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="span">The span containing elements to sort.</param>
    /// <param name="first">The inclusive start index of the range to sort.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    /// <param name="comparer">The comparer to use for element comparisons.</param>
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
    /// Sorts the subrange [left..right] (both inclusive) using the provided sort context.
    /// This overload accepts a SortSpan directly for use by other algorithms that already have a SortSpan instance.
    /// Uses tail recursion optimization to limit stack depth to O(log n) by recursing only on smaller partition.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use for element comparisons.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="left">The inclusive start index of the range to sort.</param>
    /// <param name="right">The inclusive end index of the range to sort.</param>
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int right)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (right > left)
        {
            int length = right - left + 1;

            // For small subarrays, use insertion sort
            if (length <= InsertionSortThreshold)
            {
                InsertionSort.SortCore(s, left, right + 1);
                return;
            }

            // Median-of-3 pivot selection: sort left, mid, right in place
            int mid = left + ((right - left) >> 1);
            if (s.Compare(left, mid) > 0) s.Swap(left, mid);
            if (s.Compare(left, right) > 0) s.Swap(left, right);
            if (s.Compare(mid, right) > 0) s.Swap(mid, right);
            // Bring median to left as the pivot
            s.Swap(mid, left);

            var pivot = s.Read(left);
            s.Context.OnPhase(SortPhase.QuickSortPartition, left, right, left);

            // Dutch National Flag 3-way partition (Dijkstra)
            // Invariant:
            //   [left, lt-1] : elements < pivot
            //   [lt,   i-1 ] : elements == pivot (initially contains the chosen pivot value)
            //   [i,    gt  ] : unexamined
            //   [gt+1, right]: elements > pivot
            int lt = left;
            int gt = right;
            int i = left + 1;

            while (i <= gt)
            {
                int cmp = s.Compare(i, pivot);
                if (cmp < 0)
                {
                    s.Swap(lt, i);
                    lt++;
                    i++;
                }
                else if (cmp > 0)
                {
                    s.Swap(i, gt);
                    gt--;
                    // Do not advance i: the element moved from gt is unexamined
                }
                else
                {
                    i++;
                }
            }

            // After partition:
            //   [left, lt-1] < pivot  (left partition)
            //   [lt,   gt  ] == pivot (already sorted, skip)
            //   [gt+1, right] > pivot (right partition)

            // Tail recursion: recurse on the smaller partition, loop on the larger
            int leftSize = lt - left;
            int rightSize = right - gt;

            if (leftSize <= rightSize)
            {
                SortCore(s, left, lt - 1);
                left = gt + 1;
            }
            else
            {
                SortCore(s, gt + 1, right);
                right = lt - 1;
            }
        }
    }
}
