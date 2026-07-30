using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 1つのピボットを使用して等値要素を中央にまとめる 3-way 分割統治法のソートアルゴリズムです。
/// 通常の2分割QuickSortとは異なり、配列を「ピボット未満」「ピボット等値」「ピボット超過」の3領域に分割します。
/// Bentley-McIlroy 方式（交差スキャン + 等値要素の両端退避）を採用しており、等値要素が多い場合に特に高速で、
/// 等値の中央領域は再帰から除外されます。ソート済み・ほぼソート済み入力でもソート済み区間を破壊しません。
/// <br/>
/// A divide-and-conquer sorting algorithm that collects equal elements in the center using a single pivot
/// (3-way partitioning). Unlike standard 2-partition QuickSort, it partitions the array into three regions:
/// less-than, equal-to, and greater-than the pivot. It uses the Bentley-McIlroy scheme (Hoare-style crossing
/// scans with equal elements parked at both ends), so the equal center region is excluded from further
/// recursion while sorted runs survive partitioning intact.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct 3-Way QuickSort:</strong></para>
/// <list type="number">
/// <item><description><strong>Pivot Selection (Median-of-3):</strong> The leftmost, middle, and rightmost elements
/// are sorted in place and a[mid] then provides the pivot VALUE; the pivot element is not relocated.
/// Sorting the samples absorbs stray out-of-place values at the sampled endpoints (preventing near-minimum
/// pivot chains on nearly-sorted input). Keeping the pivot in place avoids the textbook variant's left-end
/// pivot placement, whose final swap-back deposits a near-pivot element at the left end and re-poisons the
/// next level's sample on nearly-sorted input containing stray large values.</description></item>
/// <item><description><strong>Bentley-McIlroy Partitioning:</strong> Hoare-style crossing pointers with equal-element
/// parking maintain the following invariant during the scan:
/// <list type="bullet">
/// <item><description>[left, p]   : elements equal to pivot (parked at the left end; starts empty)</description></item>
/// <item><description>(p, i)      : elements strictly less than pivot</description></item>
/// <item><description>[i, j]      : unexamined elements</description></item>
/// <item><description>(j, q)      : elements strictly greater than pivot</description></item>
/// <item><description>[q, right]  : elements equal to pivot (parked at the right end; starts empty)</description></item>
/// </list>
/// i scans forward to the first element ≥ pivot, j scans backward to the last element ≤ pivot; the pair is
/// exchanged, and any element equal to the pivot is parked at the nearest end. Elements already on the correct
/// side are never moved — this is what preserves sorted runs. (A Dijkstra DNF loop instead swaps every
/// greater-than element via its gt pointer, scrambling sorted runs into rotated patterns that poison later
/// pivot samples; measured ~683 compares and ~681 swaps per element on sorted input at n=1M.)</description></item>
/// <item><description><strong>Equal-Region Swap-Back:</strong> After the scans cross, the parked equal elements at
/// both ends are swapped into the middle: [left, j] &lt; pivot, (j, i) == pivot, [i, right] &gt; pivot.
/// Only the strictly-less and strictly-greater regions are recursed. The equal region is permanently in place.</description></item>
/// <item><description><strong>Tail Recursion Optimization:</strong> Always recurse on the smaller of the two
/// non-equal regions and loop on the larger, bounding the call stack to O(log n).</description></item>
/// <item><description><strong>Termination:</strong> Each call reduces the active region by at least the size
/// of the equal partition (≥1, the pivot itself). The inner scans stop on the pivot value (present in the range,
/// with the left-end pivot acting as the j-scan sentinel) and carry explicit boundary guards. The base cases are
/// handled by the insertion sort threshold and the outer while loop guard (right &gt; left).</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Partitioning (Divide and Conquer)</description></item>
/// <item><description>Partition   : Bentley-McIlroy 3-way (crossing scans + equal-element parking)</description></item>
/// <item><description>Stable      : No (swaps do not preserve relative order)</description></item>
/// <item><description>In-place    : Yes (O(log n) stack for recursion)</description></item>
/// <item><description>Best case   : Θ(n) - All elements equal (entire array becomes the equal region in one pass;
/// parking and swap-back cost Θ(n) swaps, unlike DNF's zero, but recursion still ends immediately)</description></item>
/// <item><description>Average case: Θ(n log n) - Random distinct elements</description></item>
/// <item><description>Worst case  : O(n²) - Heavily skewed partitions with poor pivot selection</description></item>
/// <item><description>Duplicate keys: Θ(n log k) where k is the number of distinct values (k &lt;&lt; n → near O(n))</description></item>
/// <item><description>Sorted input: Θ(n log n) with ~2 swaps per partition (sorted runs are preserved)</description></item>
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
/// <para>Bentley &amp; McIlroy, "Engineering a Sort Function", Software: Practice and Experience 23(11), 1993</para>
/// <para>Sedgewick &amp; Wayne, Algorithms 4th Ed., Section 2.3 (Quicksort with 3-way partitioning)</para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Dutch_national_flag_problem</para>
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

            // Median-of-3 pivot selection: sort left, mid, right in place, then use a[mid] as the pivot VALUE
            // without relocating it. Sorting the samples (rather than just picking the median index) absorbs
            // stray out-of-place values at the sampled endpoints, which prevents near-minimum pivot chains on
            // nearly-sorted input. Not relocating the pivot matters too: the textbook Bentley-McIlroy variant
            // stores the pivot at the left end and swaps it back to the partition boundary at the end, which
            // deposits a near-pivot element at the left end — on nearly-sorted input with a stray large value
            // that re-poisons the next level's sample every level (measured ~612 compares per element at n=1M
            // with 0.1% disorder) — the same failure mode fixed in QuickSortMedian3.
            int mid = left + ((right - left) >> 1);
            if (s.IsGreaterAt(left, mid)) s.Swap(left, mid);
            if (s.IsGreaterAt(left, right)) s.Swap(left, right);
            if (s.IsGreaterAt(mid, right)) s.Swap(mid, right);

            var pivot = s.Read(mid);
            s.Context.OnPhase(SortPhase.QuickSort3wayPartition, left, right, mid);

            // Bentley-McIlroy 3-way partition.
            // Hoare-style crossing scans exchange only elements that are on the wrong side, so sorted runs
            // survive partitioning (a Dijkstra DNF loop instead swaps every greater-than element downward,
            // scrambling sorted runs into rotated patterns that poison later pivot samples — measured
            // ~683 compares and ~681 swaps per element on sorted input at n=1M before this change).
            // Elements equal to the pivot (including the pivot element itself, discovered mid-scan) are parked
            // at the two ends during the scan and swapped back into the middle afterwards, preserving the
            // Θ(n log k) duplicate-key behavior:
            //   [left, p]  : == pivot (parked; starts empty)
            //   (p, i)     : < pivot
            //   (j, q)     : > pivot
            //   [q, right] : == pivot (parked; starts empty)
            var i = left - 1;
            var j = right + 1;
            var p = left - 1;
            var q = right + 1;

            while (true)
            {
                // Move i forward to the first element >= pivot
                i++;
                while (s.IsLessThan(s.Read(i), pivot))
                {
                    if (i == right) break;
                    i++;
                }

                // Move j backward to the last element <= pivot
                // (the pivot value is present in the range, and the explicit guard bounds the scan)
                j--;
                while (s.IsLessThan(pivot, s.Read(j)))
                {
                    if (j == left) break;
                    j--;
                }

                // Pointers met on an element equal to the pivot: park it before finishing
                if (i == j && s.Compare(i, pivot) == 0)
                {
                    p++;
                    if (p != i) s.Swap(p, i);
                }
                if (i >= j) break;

                s.Swap(i, j);
                // Park equal elements at the nearest end (self-swaps are skipped)
                if (s.Compare(i, pivot) == 0)
                {
                    p++;
                    if (p != i) s.Swap(p, i);
                }
                if (s.Compare(j, pivot) == 0)
                {
                    q--;
                    if (q != j) s.Swap(q, j);
                }
            }

            // Swap the parked equal elements back into the middle:
            //   before: [left, p] == pivot, (p, j] < pivot, [i, q) > pivot, [q, right] == pivot  (i == j + 1)
            //   after : [left, j] < pivot, (j, i) == pivot, [i, right] > pivot
            i = j + 1;
            for (var k = left; k <= p; k++)
            {
                if (k != j) s.Swap(k, j);
                j--;
            }
            for (var k = right; k >= q; k--)
            {
                if (k != i) s.Swap(k, i);
                i++;
            }

            // The equal region (j, i) is permanently in place; recurse only on < and > regions.
            // Tail recursion: recurse on the smaller partition, loop on the larger
            int leftSize = j - left + 1;
            int rightSize = right - i + 1;

            if (leftSize <= rightSize)
            {
                SortCore(s, left, j);
                left = i;
            }
            else
            {
                SortCore(s, i, right);
                right = j;
            }
        }
    }
}
