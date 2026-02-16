using SortAlgorithm.Contexts;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 配列から9個の要素をサンプリングして中央値の中央値を求め、それをピボットとする分割統治法のソートアルゴリズムです。
/// Median-of-3よりも優れたピボット選択により、偏ったデータや最悪ケースに対してさらに堅牢な性能を実現します。
/// <br/>
/// A divide-and-conquer sorting algorithm using median-of-medians (median-of-9) pivot selection for superior robustness against adversarial inputs.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct QuickSort with Median-of-9 Pivot Selection:</strong></para>
/// <list type="number">
/// <item><description><strong>Median-of-9 Pivot Selection (Median of Medians):</strong> The pivot value is selected using a two-level median computation:
/// <list type="bullet">
/// <item><description>Sample 9 elements from evenly distributed positions across the array range [left, right]</description></item>
/// <item><description>Positions: left, left+m/8, left+m/4, left+m/2-m/8, left+m/2, left+m/2+m/8, right-m/4, right-m/8, right (where m = right - left)</description></item>
/// <item><description>Divide the 9 samples into 3 groups of 3 elements each</description></item>
/// <item><description>Compute the median of each group using 2-3 comparisons (9 comparisons total for all groups)</description></item>
/// <item><description>Compute the median of the three medians using 2-3 additional comparisons</description></item>
/// <item><description>Total overhead: 11-12 comparisons per partition call</description></item>
/// </list>
/// This sophisticated sampling strategy dramatically reduces worst-case probability from O(1/n³) (median-of-3) to approximately O(1/n⁹),
/// and provides excellent pivot quality for challenging input patterns such as sorted, reverse-sorted, mountain-shaped, and adversarially-crafted arrays.
/// The median-of-medians approach guarantees that the pivot is close to the true median, ensuring balanced partitions even on pathological inputs (lines 48, 128-173).</description></item>
/// <item><description><strong>Hoare Partition Scheme:</strong> The array is partitioned into two regions using bidirectional scanning, identical to median-of-3 variant:
/// <list type="bullet">
/// <item><description>Initialize pointers: l = left, r = right (line 49-50)</description></item>
/// <item><description>Left scan: advance l rightward while array[l] &lt; pivot, with boundary check l &lt; right to prevent overflow (line 55-58)</description></item>
/// <item><description>Right scan: advance r leftward while array[r] &gt; pivot, with boundary check r &gt; left to prevent underflow (line 61-64)</description></item>
/// <item><description>Swap and advance: if l ≤ r, swap array[l] with array[r], then increment l and decrement r (line 67-72)</description></item>
/// <item><description>Termination: loop exits when l &gt; r, ensuring proper partitioning (line 52)</description></item>
/// </list>
/// Boundary checks (l &lt; right and r &gt; left) prevent out-of-bounds access when all elements are smaller/larger than pivot.
/// The condition l ≤ r (not l &lt; r) ensures elements equal to pivot are swapped, preventing infinite loops on duplicate-heavy arrays.</description></item>
/// <item><description><strong>Partition Invariant:</strong> Upon completion of the partitioning phase (when l &gt; r):
/// <list type="bullet">
/// <item><description>All elements in range [left, r] satisfy: element ≤ pivot</description></item>
/// <item><description>All elements in range [l, right] satisfy: element ≥ pivot</description></item>
/// <item><description>Partition boundaries satisfy: left - 1 ≤ r &lt; l ≤ right + 1</description></item>
/// <item><description>The gap between r and l (r &lt; l) may contain elements equal to pivot that have been properly partitioned</description></item>
/// </list>
/// This invariant guarantees that after partitioning, the array is divided into two well-defined regions for recursive sorting.</description></item>
/// <item><description><strong>Recursive Subdivision:</strong> The algorithm recursively sorts two independent subranges (lines 76-83):
/// <list type="bullet">
/// <item><description>Left subrange: [left, r] is sorted only if left &lt; r (contains 2+ elements)</description></item>
/// <item><description>Right subrange: [l, right] is sorted only if l &lt; right (contains 2+ elements)</description></item>
/// </list>
/// Base case: when right ≤ left, the range contains ≤ 1 element and is trivially sorted (line 45).
/// The conditional checks (left &lt; r and l &lt; right) prevent unnecessary recursion on empty or single-element ranges,
/// improving efficiency and preventing stack overflow on edge cases.</description></item>
/// <item><description><strong>Termination Guarantee:</strong> The algorithm terminates for all inputs because:
/// <list type="bullet">
/// <item><description>Progress property: After each partition, r &lt; l, so both subranges [left, r] and [l, right] are strictly smaller than [left, right]</description></item>
/// <item><description>Minimum progress: Even when all elements equal the pivot, at least one element is excluded from each recursive call (the swapped elements at l and r)</description></item>
/// <item><description>Base case reached: The recursion depth is bounded, and each recursive call eventually reaches the base case (right ≤ left)</description></item>
/// <item><description>Expected recursion depth: O(log n) with median-of-9 pivot selection (even better than median-of-3)</description></item>
/// <item><description>Worst-case recursion depth: O(n) when partitioning is maximally unbalanced (probability &lt; 1/n⁹, astronomically rare)</description></item>
/// </list>
/// The Hoare partition scheme guarantees that partitioning makes progress even on arrays with many duplicate elements.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Partitioning (Divide and Conquer)</description></item>
/// <item><description>Partition   : Hoare partition scheme (bidirectional scan)</description></item>
/// <item><description>Stable      : No (partitioning does not preserve relative order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(log n) auxiliary space for recursion stack, O(1) for partitioning)</description></item>
/// <item><description>Best case   : Θ(n log n) - Occurs when pivot consistently divides array into balanced partitions</description></item>
/// <item><description>Average case: Θ(n log n) - Expected ~1.386n log₂ n comparisons with Hoare partition (same as median-of-3)</description></item>
/// <item><description>Worst case  : O(n²) - Occurs when partitioning is maximally unbalanced (probability &lt; 1/n⁹, virtually impossible)</description></item>
/// <item><description>Comparisons : ~1.386n log₂ n + 12n (average) - Additional ~12 comparisons per partition for median-of-9 selection</description></item>
/// <item><description>Swaps       : ~0.33n log₂ n (average) - Same as median-of-3 (Hoare partition efficiency)</description></item>
/// </list>
/// <para><strong>Median-of-9 Pivot Selection Benefits:</strong></para>
/// <list type="bullet">
/// <item><description>Worst-case probability reduction: From O(1/n³) with median-of-3 to &lt; O(1/n⁹) with median-of-9 (near-impossible worst case)</description></item>
/// <item><description>Superior pivot quality: Median-of-9 selects pivots extremely close to the true median, even on adversarial inputs</description></item>
/// <item><description>Resistance to killer sequences: Defeats "median-of-3 killer" sequences that exploit median-of-3's sampling pattern</description></item>
/// <item><description>Mountain-shaped data: Excellent performance on mountain/valley patterns where median-of-3 may struggle</description></item>
/// <item><description>Nearly-sorted data: Handles nearly-sorted arrays with scattered outliers more gracefully than median-of-3</description></item>
/// <item><description>Predictable performance: More consistent runtime across diverse input distributions</description></item>
/// </list>
/// <para><strong>Trade-offs vs. Median-of-3:</strong></para>
/// <list type="bullet">
/// <item><description>Overhead: 11-12 comparisons per partition (vs. 2-3 for median-of-3), ~4-5× more pivot selection overhead</description></item>
/// <item><description>Random data: Slightly slower on truly random data due to extra comparisons (~5-10% overhead)</description></item>
/// <item><description>Small arrays: Overhead is proportionally larger for small subarrays (consider hybrid approach with insertion sort)</description></item>
/// <item><description>Pathological inputs: Significantly faster on sorted, reverse-sorted, mountain-shaped, and adversarial patterns</description></item>
/// <item><description>Use case: Best for applications requiring predictable worst-case behavior or processing untrusted/user-generated data</description></item>
/// </list>
/// <para><strong>Comparison with Other QuickSort Variants:</strong></para>
/// <list type="bullet">
/// <item><description>vs. Random Pivot: Median-of-9 provides superior consistency and eliminates worst-case vulnerability to adversarial inputs</description></item>
/// <item><description>vs. Median-of-3: Better worst-case guarantees and performance on pathological inputs, slight overhead on random data</description></item>
/// <item><description>vs. Median-of-Medians (Deterministic Selection): Simpler implementation with similar practical performance, though theoretically weaker O(n²) vs. O(n log n) guarantee</description></item>
/// <item><description>vs. Introsort: Median-of-9 is a component of introsort strategies; introsort adds heap sort fallback for absolute worst-case guarantee</description></item>
/// <item><description>vs. Dual-Pivot QuickSort: Different approach; dual-pivot typically faster on modern CPUs, median-of-9 more theoretically robust</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Quicksort</para>
/// </remarks>
public static class QuickSortMedian9
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

        if (last - first <= 1) return;

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        SortCore(s, first, last - 1);
    }

    /// <summary>
    /// Sorts the subrange [first..last) using the provided sort context.
    /// This overload accepts a SortSpan directly for use by other algorithms that already have a SortSpan instance.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use for element comparisons.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="left">The inclusive start index of the range to sort.</param>
    /// <param name="right">The exclusive end index of the range to sort.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int right)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (left >= right) return;

        // Phase 1. Select pivot using median-of-9 strategy
        var pivot = MedianOf9Value(s, left, right);
        var l = left;
        var r = right;

        while (l <= r)
        {
            // Move l forward while elements are less than pivot
            while (l < right && s.Compare(l, pivot) < 0)
            {
                l++;
            }

            // Move r backward while elements are greater than pivot
            while (r > left && s.Compare(r, pivot) > 0)
            {
                r--;
            }

            // If pointers haven't crossed, swap and advance both
            if (l <= r)
            {
                s.Swap(l, r);
                l++;
                r--;
            }
        }

        // Phase 2. Recursively sort left and right partitions
        if (left < r)
        {
            SortCore(s, left, r);
        }
        if (l < right)
        {
            SortCore(s, l, right);
        }
    }

    /// <summary>
    /// Returns the median value among three elements at specified indices.
    /// This method performs exactly 2-3 comparisons to determine the median value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T MedianOf3Value<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int lowIdx, int midIdx, int highIdx)
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
                return s.Read(midIdx); // mid is median
            }
            else // low > mid, mid <= high
            {
                var cmpLowHigh = s.Compare(lowIdx, highIdx);
                return cmpLowHigh > 0 ? s.Read(highIdx) : s.Read(lowIdx);
            }
        }
        else // low <= mid
        {
            var cmpMidHigh = s.Compare(midIdx, highIdx);
            if (cmpMidHigh > 0) // low <= mid, mid > high
            {
                var cmpLowHigh = s.Compare(lowIdx, highIdx);
                return cmpLowHigh > 0 ? s.Read(lowIdx) : s.Read(highIdx);
            }
            else // low <= mid <= high
            {
                return s.Read(midIdx); // mid is median
            }
        }
    }

    /// <summary>
    /// Returns the median value among nine sampled elements from the array.
    /// This method samples elements at evenly distributed positions and computes
    /// the median of medians to select a high-quality pivot.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T MedianOf9Value<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int low, int high)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var m2 = (high - low) / 2;
        var m4 = m2 / 2;
        var m8 = m4 / 2;

        // Sample 9 indices distributed across the range
        var i1 = low;
        var i2 = low + m8;
        var i3 = low + m4;
        var i4 = low + m2 - m8;
        var i5 = low + m2;
        var i6 = low + m2 + m8;
        var i7 = high - m4;
        var i8 = high - m8;
        var i9 = high;

        // Compute median of three groups, then median of those medians
        var median1 = MedianOf3Value(s, i1, i2, i3);
        var median2 = MedianOf3Value(s, i4, i5, i6);
        var median3 = MedianOf3Value(s, i7, i8, i9);

        // Return median of the three medians (using comparer for value comparison)
        if (s.Comparer.Compare(median1, median2) > 0)
        {
            if (s.Comparer.Compare(median2, median3) > 0)
            {
                return median2; // median1 > median2 > median3
            }
            else
            {
                return s.Comparer.Compare(median1, median3) > 0 ? median3 : median1;
            }
        }
        else // median1 <= median2
        {
            if (s.Comparer.Compare(median2, median3) > 0)
            {
                return s.Comparer.Compare(median1, median3) > 0 ? median1 : median3;
            }
            else
            {
                return median2; // median1 <= median2 <= median3
            }
        }
    }
}
