using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 配列から四分位位置の中央値を求めてピボットとし、このピボットを基準に配列を左右に分割する分割統治法のソートアルゴリズムです。
/// Hoare partition schemeを使用し、四分位ベースのMedian-of-3法でピボットを選択することで様々なデータパターンに対して安定した性能を実現します。
/// <br/>
/// A divide-and-conquer sorting algorithm that selects the pivot as the median of quartile positions in the array and partitions the array into left and right subarrays based on that pivot.
/// It uses the Hoare partition scheme and selects the pivot via a quartile-based median-of-three method to achieve stable performance across various data patterns.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct QuickSort with Quartile-Based Median-of-3:</strong></para>
/// <list type="number">
/// <item><description><strong>Quartile-Based Median-of-3 Pivot Selection:</strong> The pivot value is selected as the median of three sampled elements
/// at quartile positions: array[q1], array[mid], and array[q3], where q1 = left + length/4, mid = left + length/2, q3 = left + 3*length/4.
/// This selection method is computed using 2-3 comparisons and ensures better pivot quality than random selection or simple left/mid/right sampling.
/// The quartile-based median-of-3 strategy provides robust performance across various data patterns including mountain-shaped, valley-shaped,
/// and partially sorted arrays, while maintaining the O(1/n³) probability of worst-case partitioning.</description></item>
/// <item><description><strong>Hoare Partition Scheme:</strong> The array is partitioned into two regions using bidirectional scanning:
/// <list type="bullet">
/// <item><description>Initialize pointers: l = left, r = right</description></item>
/// <item><description>Left scan: advance l rightward while array[l] &lt; pivot, with boundary check l &lt; right to prevent overflow</description></item>
/// <item><description>Right scan: advance r leftward while array[r] &gt; pivot, with boundary check r &gt; left to prevent underflow</description></item>
/// <item><description>Swap and advance: if l ≤ r, swap array[l] with array[r], then increment l and decrement r</description></item>
/// <item><description>Termination: loop exits when l &gt; r, ensuring proper partitioning</description></item>
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
/// <item><description><strong>Recursive Subdivision:</strong> The algorithm recursively sorts two independent subranges:
/// <list type="bullet">
/// <item><description>Left subrange: [left, r] is sorted only if left &lt; r (contains 2+ elements)</description></item>
/// <item><description>Right subrange: [l, right] is sorted only if l &lt; right (contains 2+ elements)</description></item>
/// </list>
/// Base case: when right ≤ left, the range contains ≤ 1 element and is trivially sorted.
/// The conditional checks (left &lt; r and l &lt; right) prevent unnecessary recursion on empty or single-element ranges.</description></item>
/// <item><description><strong>Termination Guarantee:</strong> The algorithm terminates for all inputs because:
/// <list type="bullet">
/// <item><description>Progress property: After each partition, r &lt; l, so both subranges [left, r] and [l, right] are strictly smaller than [left, right]</description></item>
/// <item><description>Minimum progress: Even when all elements equal the pivot, the Hoare partition scheme ensures at least one element is excluded from each subrange</description></item>
/// <item><description>Base case reached: The recursion depth is bounded, and each recursive call eventually reaches the base case (right ≤ left)</description></item>
/// <item><description>Expected recursion depth: O(log n) with quartile-based median-of-3 pivot selection</description></item>
/// <item><description>Worst-case recursion depth: O(log n) with tail recursion optimization (always recurse on smaller partition)</description></item>
/// <item><description>Tail recursion optimization: The implementation recursively processes only the smaller partition and loops on the larger one, guaranteeing O(log n) stack depth even in adversarial cases</description></item>
/// </list>
/// The Hoare partition scheme guarantees progress even on arrays with many duplicate elements.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Partitioning (Divide and Conquer)</description></item>
/// <item><description>Partition   : Hoare partition scheme (bidirectional scan)</description></item>
/// <item><description>Stable      : No (partitioning does not preserve relative order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(log n) auxiliary space for recursion stack, O(1) for partitioning)</description></item>
/// <item><description>Best case   : Θ(n log n) - Occurs when pivot consistently divides array into balanced partitions</description></item>
/// <item><description>Average case: Θ(n log n) - Expected ~1.386n log₂ n comparisons with Hoare partition</description></item>
/// <item><description>Worst case  : O(n²) - Occurs when partitioning is maximally unbalanced (probability ~1/n³ with median-of-3)</description></item>
/// <item><description>Comparisons : ~1.386n log₂ n (average) - Hoare partition uses fewer comparisons than Lomuto partition</description></item>
/// <item><description>Swaps       : ~0.33n log₂ n (average) - Hoare partition performs ~3× fewer swaps than Lomuto partition</description></item>
/// </list>
/// <para><strong>Quartile-Based Median-of-3 Pivot Selection Benefits:</strong></para>
/// <list type="bullet">
/// <item><description>Worst-case probability reduction: From O(1/n) with random pivot to O(1/n³) with median-of-3</description></item>
/// <item><description>Improved pivot quality: Median-of-3 tends to select pivots closer to the true median of the array</description></item>
/// <item><description>Minimal overhead: Requires only 2-3 additional comparisons per partitioning step</description></item>
/// <item><description>Robust data pattern handling: Efficiently handles sorted, reverse-sorted, mountain-shaped, and nearly-sorted arrays</description></item>
/// <item><description>Better distribution: Quartile sampling (1/4, 1/2, 3/4) provides more representative samples than edge sampling (0, 1/2, 1)</description></item>
/// <item><description>Cache efficiency: Samples elements from distributed positions, improving spatial locality</description></item>
/// </list>
/// <para><strong>Comparison with Other Sorting Algorithms:</strong></para>
/// <list type="bullet">
/// <item><description>vs. Random Pivot QuickSort: Median-of-3 provides more consistent performance with minimal overhead</description></item>
/// <item><description>vs. Lomuto Partition QuickSort: Hoare partition performs ~3× fewer swaps and better handles duplicates</description></item>
/// <item><description>vs. Dual-Pivot QuickSort: Simpler implementation, but dual-pivot can be ~5-10% faster on modern CPUs</description></item>
/// <item><description>vs. IntroSort: This is the core algorithm; IntroSort adds HeapSort fallback for worst-case protection</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Quicksort</para>
/// </remarks>
public static class QuickSortMedian3
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
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <typeparam name="TComparer">The type of the comparer</typeparam>
    /// <typeparam name="TContext">The type of the sort context.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="comparer">The comparer to use for element comparisons.</param>
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
    /// Uses tail recursion optimization to limit stack depth to O(log n) by recursing only on smaller partition.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use for element comparisons.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="left">The inclusive start index of the range to sort.</param>
    /// <param name="right">The exclusive end index of the range to sort.</param>
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int right)
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
            var q3 = left + (length * 3) / 4;
            var pivot = MedianOf3Value(s, q1, mid, q3);

            // Phase 2. Partition array using Hoare partition scheme
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

            // Phase 3. Tail recursion optimization: always process left first, then loop on right
            // This ensures consistent left-to-right ordering for visualization
            // After partitioning: r is the last index of left partition, l is the first index of right partition
            if (left < r)
            {
                // Recurse on left partition
                SortCore(s, left, r);
            }
            // Tail recursion: continue loop with right partition
            left = l;
        }
    }

    /// <summary>
    /// Returns the median value among three elements at specified indices.
    /// This method performs exactly 2-3 comparisons to determine the median value.
    /// </summary>
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
}
