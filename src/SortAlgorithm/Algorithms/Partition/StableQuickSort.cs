using SortAlgorithm.Contexts;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 安定ソート版のクイックソートです。
/// 配列から四分位位置の中央値を求めてピボットとし、このピボットを基準に配列を左右に分割する分割統治法のソートアルゴリズムです。
/// 四分位ベースのMedian-of-3法でピボットを選択し、外部バッファへの2パス（件数の数え上げ、次に振り分け）で未満・等値・超過の3領域へ安定に分割します。
/// <br/>
/// A stable variant of quicksort.
/// A divide-and-conquer sorting algorithm that selects the pivot as the median of quartile positions in the array and partitions the array into less, equal and greater regions based on that pivot.
/// To ensure stability, it partitions through an external buffer in two passes: one counts the three groups, the next distributes them.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Stable QuickSort:</strong></para>
/// <list type="number">
/// <item><description><strong>Median-of-3 Pivot Selection:</strong> The pivot value is selected as the median of three sampled elements
/// at quartile positions: array[q1], array[mid], and array[q3], where q1 = left + length/4, mid = left + length/2, q3 = right - length/4.
/// This selection method is computed using 2-3 comparisons and ensures better pivot quality than random selection or simple left/mid/right sampling.
/// The quartile-based median-of-3 strategy provides robust performance across various data patterns including mountain-shaped, valley-shaped,
/// and partially sorted arrays, while maintaining the O(1/n³) probability of worst-case partitioning.</description></item>
/// <item><description><strong>Stable Three-Way Partitioning:</strong> The array is partitioned into three groups while maintaining stability:
/// Elements are classified into three categories (less than, equal to, greater than pivot) and copied to temporary storage.
/// Within each category, the relative order of elements from the original array is preserved.
/// The three groups are then copied back sequentially: less-than group, equal group, then greater-than group.</description></item>
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
/// <item><description>Average case: Θ(n log n) - Median-of-3 pivots split near the median on average</description></item>
/// <item><description>Worst case  : O(n²) - Occurs when partitioning is maximally unbalanced (probability ~1/n³ with median-of-3)</description></item>
/// <item><description>Comparisons : Θ(n log n) average, O(n²) worst - every element is compared with the pivot twice per level (count, then distribute); with a median-of-3 pivot on distinct keys that is about 2 × (12/7) n ln n ≈ 2.38 n log₂ n</description></item>
/// <item><description>Swaps       : 0 - elements move by copying to and from the scratch buffer, never by exchange</description></item>
/// <item><description>Index Reads : Θ(n log n) average, O(n²) worst - the count pass and the distribute pass each read every element of the range once per level, and the copy back reads the scratch</description></item>
/// <item><description>Index Writes: Θ(n log n) average, O(n²) worst - each element is written into scratch and back at every level</description></item>
/// <item><description>Space       : O(n) total auxiliary across all active recursion levels + O(log n) recursion stack</description></item>
/// <item><description>Copies      : ~2n log₂ n (average) - Each element copied to/from temporary storage at each recursion level</description></item>
/// </list>
/// <para><strong>Comparer contract:</strong></para>
/// <para>The distribution pass compares every element with the pivot again and relies on the answers of the counting pass. Scratch access
/// is unchecked, so each write is checked against its region, and a comparer that answers differently can make the sort throw
/// <see cref="InvalidOperationException"/> before the range is written back. The natural order of a primitive type always repeats its
/// answers, and the checks are compiled out for it. Either way the range keeps its elements, in an unspecified order, and nothing outside
/// the range is read or written.</para>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Quicksort</para>
/// </remarks>
public static class StableQuickSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by StableQuickSortTests, which derives from StableSortTestsBase.</remarks>
    public static bool IsStable => true;

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Temporary partition buffer

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
    /// Sorts the inclusive subrange [left..right].
    /// This overload accepts a SortSpan directly for use by other algorithms that already have a SortSpan instance.
    /// Uses tail recursion optimization to limit stack depth to O(log n) by recursing on the smaller partition.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use for element comparisons.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="left">The inclusive start index of the range to sort.</param>
    /// <param name="right">The inclusive end index of the range to sort.</param>
    /// <param name="scratch">Scratch buffer for stable partitioning; must be at least as long as the range being sorted.</param>
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
            var q3 = right - length / 4;   // upper-quartile sample (≈ left + length*3/4) without the int overflow of 3*length
            var pivotIndex = MedianOf3Index(s, q1, mid, q3);
            s.Context.OnPhase(SortPhase.QuickSortPartition, left, right, pivotIndex);
            s.Context.OnRole(pivotIndex, BUFFER_MAIN, RoleType.Pivot);

            // Phase 2. Stable partition using pre-allocated scratch buffer
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
    /// Uses the caller-provided scratch buffer (size ≥ n, allocated once at the Sort entry point).
    /// StablePartition completes before any recursion, so the same scratch is safely reused at all recursion depths.
    /// </summary>
    private static (int lessEnd, int greaterStart) StablePartition<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int right, int pivotIndex, Span<T> scratch)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var length = right - left + 1;
        var tempSortSpan = new SortSpan<T, TComparer, TContext>(scratch.Slice(0, length), s.Context, s.Comparer, BUFFER_TEMP);

        // The range is not written until Phase 3, so the pivot is read once and held.
        var pivot = s.Read(pivotIndex);
        var lessIdx = 0;
        var equalIdx = 0;

        // Phase 1: Count elements in each partition
        // Short-circuit when i == pivotIndex: pivot always compares equal to itself
        for (var i = left; i <= right; i++)
        {
            var cmp = i == pivotIndex ? 0 : s.Compare(i, pivot);
            if (cmp < 0)
            {
                lessIdx++;
            }
            else if (cmp == 0)
            {
                equalIdx++;
            }
        }

        var lessEnd = lessIdx;
        var equalEnd = lessIdx + equalIdx;
        lessIdx = 0;
        equalIdx = lessEnd;
        var greaterIdx = equalEnd;

        // Phase 2: Distribute elements to buffer maintaining order (SortSpan経由)
        // Short-circuit when i == pivotIndex: pivot always compares equal to itself
        // A comparer that does not repeat its Phase 1 answers can overfill a region, and scratch access is unchecked, so each write is
        // checked against its region. The range is untouched until Phase 3. The natural order of a primitive type repeats its answers,
        // so the checks fold away for it.
        var checkRegions = !s.UsesNaturalPrimitiveOrder;
        for (var i = left; i <= right; i++)
        {
            var element = s.Read(i);
            // Second comparison per element; it repeats Phase 1 because the range is not modified until Phase 3.
            // The element is already held, so it is compared by value instead of being read again.
            var cmp = i == pivotIndex ? 0 : s.Compare(element, pivot);
            if (cmp < 0)
            {
                if (checkRegions && lessIdx == lessEnd) ThrowHelper.ThrowInconsistentComparer();
                tempSortSpan.Write(lessIdx++, element);
            }
            else if (cmp == 0)
            {
                if (checkRegions && equalIdx == equalEnd) ThrowHelper.ThrowInconsistentComparer();
                tempSortSpan.Write(equalIdx++, element);
            }
            else
            {
                if (checkRegions && greaterIdx == length) ThrowHelper.ThrowInconsistentComparer();
                tempSortSpan.Write(greaterIdx++, element);
            }
        }

        // Phase 3: Copy back to original array using CopyTo for efficiency
        tempSortSpan.CopyTo(0, s, left, length);

        return (left + lessEnd, left + equalEnd);
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
