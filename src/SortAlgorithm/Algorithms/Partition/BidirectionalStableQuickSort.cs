using SortAlgorithm.Contexts;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 双方向コピーバックを用いた安定ソート版クイックソートです。
/// 安定ソートのため外部バッファを使用しますが、分割時に less を scratch 先頭・greater を scratch 末尾（逆順）へ1スキャンで書き込み、その後 greater セクションをリバースすることで安定性を保ちながらカウントパスを不要にします。
/// <br/>
/// A stable variant of quicksort that uses a bidirectional copy-back strategy.
/// To ensure stability it uses an external buffer, but eliminates the count pass by writing less elements to the scratch front and greater elements to the scratch back (reversed) in a single forward scan, then reversing the greater section to restore left-to-right order.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Bidirectional Stable QuickSort:</strong></para>
/// <list type="number">
/// <item><description><strong>Median-of-3 Pivot Selection:</strong> The pivot value is selected as the median of three sampled elements
/// at quartile positions: array[q1], array[mid], and array[q3], where q1 = left + length/4, mid = left + length/2, q3 = left + 3*length/4.
/// This selection method is computed using 2-3 comparisons and ensures better pivot quality than random selection or simple left/mid/right sampling.
/// The quartile-based median-of-3 strategy provides robust performance across various data patterns including mountain-shaped, valley-shaped,
/// and partially sorted arrays, while maintaining the O(1/n³) probability of worst-case partitioning.</description></item>
/// <item><description><strong>Bidirectional Stable Three-Way Partitioning (1-pass):</strong> The array is partitioned into three groups while maintaining stability
/// using a single forward scan instead of the traditional two-pass (count + distribute) approach:
/// <list type="bullet">
/// <item><description>The pivot value is read once upfront from the array.</description></item>
/// <item><description>Single forward scan: less-than elements are written to the front of scratch in order;
/// greater-than elements are written to the back of scratch in reverse order; equal elements are skipped.</description></item>
/// <item><description>The reversed greater-than section in scratch is corrected by a short in-place reverse sweep to restore left-to-right stability.</description></item>
/// <item><description>A second targeted scan fills only equal elements into the gap between the less and greater sections.</description></item>
/// <item><description>The populated scratch buffer is copied back to the original array.</description></item>
/// </list>
/// Compared to the 2-pass approach, less-than elements cost 1 comparison and greater/equal elements cost 2,
/// reducing total comparison work when duplicate elements are rare.</description></item>
/// <item><description><strong>Partition Invariant:</strong> Upon completion of the stable partitioning phase:
/// <list type="bullet">
/// <item><description>All elements in the left partition are strictly less than pivot</description></item>
/// <item><description>All elements in the middle partition are equal to pivot</description></item>
/// <item><description>All elements in the right partition are strictly greater than pivot</description></item>
/// <item><description>Relative order of elements within each partition is preserved from the original array</description></item>
/// </list>
/// This invariant guarantees stability and ensures that only the less-than and greater-than regions require further sorting.</description></item>
/// <item><description><strong>Recursive Subdivision with Tail Recursion Optimization:</strong> The algorithm recursively sorts the less-than and greater-than partitions:
/// <list type="bullet">
/// <item><description>The middle partition (equal to pivot) is already sorted and excluded from further recursion</description></item>
/// <item><description>Between the two remaining partitions, the smaller one is sorted recursively</description></item>
/// <item><description>The larger partition is sorted via iteration (tail recursion elimination)</description></item>
/// <item><description>This ensures recursion depth is O(log n) even in worst-case scenarios</description></item>
/// </list>
/// Base case: when the partition contains ≤ 1 element, it is trivially sorted.
/// Tail recursion optimization prevents stack overflow by limiting stack depth to the smaller partition.</description></item>
/// <item><description><strong>Termination Guarantee:</strong> The algorithm terminates for all inputs because:
/// <list type="bullet">
/// <item><description>Progress property: After each partition, both subranges (less-than and greater-than) are strictly smaller than the original range</description></item>
/// <item><description>Minimum progress: Elements equal to the pivot are excluded from recursion, guaranteeing size reduction</description></item>
/// <item><description>Base case reached: Each recursive call eventually reaches a trivially sorted partition (≤ 1 element)</description></item>
/// <item><description>Expected recursion depth: O(log n) with median-of-3 pivot selection and tail recursion optimization</description></item>
/// <item><description>Worst-case recursion depth: O(log n) due to tail recursion optimization (not O(n) like naive QuickSort)</description></item>
/// <item><description>Tail recursion optimization: The implementation recursively processes only the smaller partition and loops on the larger one, guaranteeing O(log n) stack depth even in adversarial cases</description></item>
/// </list>
/// The three-way partition guarantees progress even on arrays with many duplicate elements.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Partitioning (Divide and Conquer)</description></item>
/// <item><description>Stable      : Yes (preserves relative order of equal elements)</description></item>
/// <item><description>In-place    : No (O(n) total auxiliary space across all active recursion levels, O(log n) recursion stack)</description></item>
/// <item><description>Best case   : Θ(n log n) - Occurs when pivot consistently divides array into balanced partitions</description></item>
/// <item><description>Average case: Θ(n log n) - Expected ~n log₂ n comparisons with median-of-3 pivot selection</description></item>
/// <item><description>Worst case  : O(n²) - Occurs when partitioning is maximally unbalanced (probability ~1/n³ with median-of-3)</description></item>
/// <item><description>Comparisons : ~n log₂ n (average) - Less elements cost 1 comparison; greater/equal cost 2; fewer total comparisons than 2-pass when duplicates are rare</description></item>
/// <item><description>Copies      : ~2n log₂ n (average) - Each element copied to/from temporary storage at each recursion level</description></item>
/// <item><description>Swaps       : ~(greaterCount/2) per partition level - In-place reversal of the greater section in scratch to restore stability</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Quicksort</para>
/// </remarks>
public static class BidirectionalStableQuickSort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Temporary merge buffer

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

        var n = last - first;
        if (n <= 1) return;

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        var scratchBuffer = ArrayPool<T>.Shared.Rent(n);
        try
        {
            SortCore(s, first, last - 1, scratchBuffer.AsSpan(0, n));
        }
        finally
        {
            ArrayPool<T>.Shared.Return(scratchBuffer, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    /// <summary>
    /// Sorts the subrange [first..last) using the provided sort context.
    /// This overload accepts a SortSpan directly for use by other algorithms that already have a SortSpan instance.
    /// Uses tail recursion optimization to limit stack depth to O(log n) by recursing on the smaller partition.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use for element comparisons.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="left">The inclusive start index of the range to sort.</param>
    /// <param name="right">The inclusive end index of the range to sort.</param>
    /// <param name="context">The sort context for tracking statistics and observations.</param>
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int right, Span<T> scratch)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (left < right)
        {
            // Phase 1. Select pivot using median-of-3 strategy with improved sampling
            // Use quartile positions (1/4, 1/2, 3/4) instead of (left, mid, right)
            // This provides better pivot selection for mountain-shaped and similar patterns
            var length = right - left + 1;
            var q1 = left + length / 4;
            var mid = left + length / 2;
            var q3 = right - length / 4;   // overflow-safe: equivalent to left + length*3/4
            var pivotIndex = MedianOf3Index(s, q1, mid, q3);
            s.Context.OnPhase(SortPhase.QuickSortPartition, left, right, pivotIndex);
            s.Context.OnRole(pivotIndex, BUFFER_MAIN, RoleType.Pivot);

            // Phase 2. Bidirectional stable partition (1-pass) using pre-allocated scratch buffer
            // Single scan writes less to scratch front, greater to scratch back (reversed, then corrected).
            var (lessEnd, greaterStart) = StablePartition(s, left, right, pivotIndex, scratch);

            // Phase 3. Recursively sort partitions with tail recursion optimization
            s.Context.OnRole(pivotIndex, BUFFER_MAIN, RoleType.None);
            // Recurse on smaller partition, loop on larger to ensure O(log n) stack depth
            var leftLen = lessEnd - left;
            var rightLen = right + 1 - greaterStart;

            if (leftLen < rightLen)
            {
                // Left is smaller: recurse on left, loop on right
                if (leftLen > 1)
                {
                    SortCore(s, left, lessEnd - 1, scratch);
                }
                // Tail recursion: continue loop with right partition
                left = greaterStart;
            }
            else
            {
                // Right is smaller or equal: recurse on right, loop on left
                if (rightLen > 1)
                {
                    SortCore(s, greaterStart, right, scratch);
                }
                // Tail recursion: continue loop with left partition
                right = lessEnd - 1;
            }
        }
    }

    /// <summary>
    /// Performs stable partitioning of the range [left..right] around the pivot element at pivotIndex.
    /// Returns (lessEnd, greaterStart) where:
    /// - [left, lessEnd): elements less than pivot
    /// - [lessEnd, greaterStart): elements equal to pivot
    /// - [greaterStart, right + 1): elements greater than pivot
    /// <para>
    /// Uses a bidirectional single-scan approach that eliminates the count pass:
    /// 1. Single forward scan writes less to scratch front and greater to scratch back (reversed).
    ///    Pivot value is read once upfront; no pre-counting needed — positions are resolved by
    ///    the pointers after the scan.
    /// 2. The reversed greater section in scratch is corrected by a short in-place reverse sweep.
    /// 3. Equal elements are filled into the gap with a targeted forward scan.
    /// </para>
    /// Compared to the 2-pass approach, main-scan comparisons drop from 2 per element to 1 for
    /// less/greater elements, improving throughput especially when equal elements are rare.
    /// </summary>
    private static (int lessEnd, int greaterStart) StablePartition<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int right, int pivotIndex, Span<T> scratch)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var length = right - left + 1;
        var tempSortSpan = new SortSpan<T, TComparer, TContext>(scratch.Slice(0, length), s.Context, s.Comparer, BUFFER_TEMP);

        var pivotVal = s.Read(pivotIndex);

        // Bidirectional single scan: less → scratch front (in order), greater → scratch back (reversed).
        // Equal elements are skipped here and filled into the gap in a subsequent targeted pass.
        var lessIdx = 0;
        var greaterIdx = length - 1;

        for (var i = left; i <= right; i++)
        {
            var elem = s.Read(i);
            if (s.IsLessThan(elem, pivotVal))
            {
                tempSortSpan.Write(lessIdx++, elem);     // less → scratch[lessIdx++]
            }
            else if (s.IsLessThan(pivotVal, elem))
            {
                tempSortSpan.Write(greaterIdx--, elem);  // greater → scratch[greaterIdx--] (reversed)
            }
            // equal: skip
        }

        var lessCount = lessIdx;
        var greaterStart = greaterIdx + 1; // scratch[greaterStart..length) contains greater in reverse order

        // Reverse the greater section to restore left-to-right stability.
        // During the forward scan each later-encountered greater element was placed at a lower
        // scratch index, so reversing the section restores the original left-to-right order.
        {
            var lo = greaterStart;
            var hi = length - 1;
            while (lo < hi)
            {
                tempSortSpan.Swap(lo++, hi--);
            }
        }

        // Fill equal elements into scratch[lessCount..greaterStart) in left-to-right order.
        var equalDst = lessCount;
        for (var i = left; i <= right; i++)
        {
            var elem = s.Read(i);
            if (!s.IsLessThan(elem, pivotVal) && !s.IsLessThan(pivotVal, elem))
            {
                tempSortSpan.Write(equalDst++, elem);
            }
        }

        // Copy back to original array.
        tempSortSpan.CopyTo(0, s, left, length);

        return (left + lessCount, left + greaterStart);
    }

    /// <summary>
    /// Returns the median index among three elements at specified indices.
    /// This method performs exactly 2-3 comparisons to determine the median index.
    /// Returns index instead of value to avoid copying large struct pivot values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int MedianOf3Index<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int lowIdx, int midIdx, int highIdx)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Use SortSpan.Compare to track statistics
        var cmpLowMid = s.Compare(lowIdx, midIdx);

        if (cmpLowMid > 0) // low > mid
        {
            var cmpMidHigh = s.Compare(midIdx, highIdx);
            if (cmpMidHigh > 0) // low > mid > high
            {
                return midIdx; // mid is median
            }
            else // low > mid, mid <= high
            {
                var cmpLowHigh = s.Compare(lowIdx, highIdx);
                return cmpLowHigh > 0 ? highIdx : lowIdx;
            }
        }
        else // low <= mid
        {
            var cmpMidHigh = s.Compare(midIdx, highIdx);
            if (cmpMidHigh > 0) // low <= mid, mid > high
            {
                var cmpLowHigh = s.Compare(lowIdx, highIdx);
                return cmpLowHigh > 0 ? lowIdx : highIdx;
            }
            else // low <= mid <= high
            {
                return midIdx; // mid is median
            }
        }
    }
}
