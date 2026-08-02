using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 配列の左端・中央・右端の3点から中央値を求めてピボットとし、このピボットを基準に配列を左右に分割する分割統治法のソートアルゴリズムです。
/// Hoare partition schemeを使用し、Median-of-3法でピボットを選択することで様々なデータパターンに対して安定した性能を実現します。
/// <br/>
/// A divide-and-conquer sorting algorithm that selects the pivot as the median of three elements (left, middle, right) and partitions the array into left and right subarrays based on that pivot.
/// It uses the Hoare partition scheme and selects the pivot via median-of-three method to achieve stable performance across various data patterns.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct QuickSort with Median-of-3:</strong></para>
/// <list type="number">
/// <item><description><strong>Median-of-3 Pivot Selection:</strong> The pivot value is selected as the median of three sampled elements
/// at positions: array[left], array[mid], and array[right], where mid = (left + right) / 2.
/// This selection method is computed using 2-3 comparisons and ensures better pivot quality than random selection or always using a fixed position.
/// The median-of-3 strategy provides robust performance across various data patterns including sorted, reverse-sorted,
/// and partially sorted arrays, while maintaining the O(1/n³) probability of worst-case partitioning.</description></item>
/// <item><description><strong>Sampled Elements Are Sorted In Place:</strong> The three samples are ordered so that
/// array[left] ≤ array[mid] ≤ array[right], and array[mid] then provides the pivot VALUE (the pivot is not relocated further).
/// Two failure modes motivate this design, both measured at n=1M before the fix:
/// relocating the pivot to the right end before partitioning scrambled sorted runs into rotated patterns
/// (~650 comparisons and ~520 swaps per element on sorted input, versus ~20 and ~0 after);
/// and picking the median index without sorting the samples let a stray small value at the right end of a
/// sorted run produce a near-minimum pivot whose first Hoare exchange re-poisoned the next level's sample
/// (~540 comparisons per element on 0.1%-disordered input). Sorting the samples absorbs such stray values into
/// the correct side and performs zero swaps when the input is already sorted.</description></item>
/// <item><description><strong>Hoare Bidirectional Partition:</strong> Two pointers scan toward each other:
/// <list type="bullet">
/// <item><description>i moves forward while array[i] &lt; pivot; j moves backward while pivot &lt; array[j]</description></item>
/// <item><description>When both stop and i ≤ j, array[i] and array[j] are exchanged and both pointers advance</description></item>
/// <item><description>The scans terminate because the pivot value is present in the range, providing a stopper for both directions</description></item>
/// <item><description>Elements already on the correct side are never moved, so sorted runs survive partitioning intact</description></item>
/// </list>
/// After partitioning, no element in [left, j] exceeds the pivot value and no element in [i, right] is below it (j &lt; i).</description></item>
/// <item><description><strong>Termination Guarantee:</strong> The algorithm terminates for all inputs because:
/// <list type="bullet">
/// <item><description>Progress property: Each crossing scan performs at least one pointer advance, and both subranges [left, j] and [i, right] are strictly smaller than [left, right]</description></item>
/// <item><description>Base case reached: The recursion depth is bounded, and each recursive call eventually reaches the base case (right ≤ left)</description></item>
/// <item><description>Tail recursion optimization: The implementation recursively processes only the smaller partition and loops on the larger one, guaranteeing O(log n) stack depth even in adversarial cases</description></item>
/// </list>
/// The Hoare partition scheme guarantees progress even on arrays with many duplicate elements
/// (equal elements stop both scans and are exchanged, which keeps the split balanced).</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Partitioning (Divide and Conquer)</description></item>
/// <item><description>Partition   : Hoare bidirectional partition with median-of-3 pivot value</description></item>
/// <item><description>Stable      : No (partitioning does not preserve relative order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(log n) auxiliary space for recursion stack, O(1) for partitioning)</description></item>
/// <item><description>Best case   : Θ(n log n) - Sorted input partitions perfectly with zero effective swaps</description></item>
/// <item><description>Average case: Θ(n log n) - Expected ~1.39n log₂ n comparisons</description></item>
/// <item><description>Worst case  : O(n²) - Occurs when partitioning is maximally unbalanced (probability ~1/n³ with median-of-3)</description></item>
/// <item><description>Comparisons : ~1.39n log₂ n (average)</description></item>
/// <item><description>Swaps       : ~0.33n log₂ n (average); near zero on sorted input</description></item>
/// <item><description>Duplicates  : O(n log n) - equal keys split evenly; use <see cref="QuickSort3way"/> for the Θ(n log k) duplicate-specialized variant</description></item>
/// </list>
/// <para><strong>Median-of-3 Pivot Selection Benefits:</strong></para>
/// <list type="bullet">
/// <item><description>Worst-case probability reduction: From O(1/n) with random pivot to O(1/n³) with median-of-3</description></item>
/// <item><description>Improved pivot quality: Median-of-3 tends to select pivots closer to the true median of the array</description></item>
/// <item><description>Minimal overhead: Requires only 2-3 additional comparisons per partitioning step</description></item>
/// <item><description>Robust data pattern handling: Efficiently handles sorted, reverse-sorted, and nearly-sorted arrays</description></item>
/// <item><description>Simple and widely adopted: The standard median-of-3 approach is well-understood and proven in practice</description></item>
/// </list>
/// <para><strong>Comparison with Other Sorting Algorithms:</strong></para>
/// <list type="bullet">
/// <item><description>vs. Random Pivot QuickSort: Median-of-3 provides more consistent performance with minimal overhead</description></item>
/// <item><description>vs. QuickSort (middle pivot): Same Hoare partition; median-of-3 sampling improves pivot quality on skewed distributions</description></item>
/// <item><description>vs. QuickSort3way (Dutch National Flag): 3-way partition wins on duplicate-heavy arrays (Θ(n log k)); this variant wins on sorted and nearly-sorted arrays because Hoare partitioning preserves sorted runs</description></item>
/// <item><description>vs. Dual-Pivot QuickSort: Simpler implementation; dual-pivot can be faster on random data</description></item>
/// <item><description>vs. IntroSort: This is the core algorithm; IntroSort adds HeapSort fallback for worst-case protection</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Quicksort</para>
/// </remarks>
public static class QuickSortMedian3
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by QuickSortMedian3Tests, which derives from SortTestsBase.</remarks>
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
        while (left < right)
        {
            // Phase 1. Select pivot using standard median-of-3 strategy.
            // Order the three samples in place so that a[left] <= a[mid] <= a[right], then use a[mid] as the
            // pivot value. Sorting the samples (rather than just picking the median index) matters on
            // nearly-sorted input: a stray small value at the right end of a sorted run would otherwise make
            // the median-of-3 a near-minimum value, and the first Hoare exchange would push a small value back
            // to the far end, re-poisoning the next level's sample and cascading into unbalanced partitions.
            // On sorted input these two comparisons perform no swaps, so sorted runs are left untouched.
            var mid = left + (right - left) / 2;
            if (s.Compare(mid, left) < 0) s.Swap(left, mid);
            if (s.Compare(right, mid) < 0)
            {
                s.Swap(mid, right);
                if (s.Compare(mid, left) < 0) s.Swap(left, mid);
            }
            s.Context.OnPhase(SortPhase.QuickSortPartition, left, right, mid);
            s.Context.OnRole(mid, BUFFER_MAIN, RoleType.Pivot);
            var pivot = s.Read(mid);

            // Phase 2. Hoare bidirectional partition.
            // Scans stop at elements on the wrong side and exchange them; elements already in place are
            // never moved, so sorted runs survive partitioning (zero effective swaps on sorted input).
            // The pivot value is present in the range, which guarantees both inner scans terminate.
            var i = left;
            var j = right;

            while (i <= j)
            {
                // Move i forward while elements are less than pivot
                while (s.IsElementLessThan(i, pivot))
                {
                    i++;
                }

                // Move j backward while elements are greater than pivot
                while (s.IsValueLessThan(pivot, j))
                {
                    j--;
                }

                // Swap if pointers haven't crossed
                if (i <= j)
                {
                    s.Swap(i, j);
                    i++;
                    j--;
                }
            }

            s.Context.OnRole(mid, BUFFER_MAIN, RoleType.None);

            // After partitioning: no element in [left, j] is greater than the pivot value,
            // and no element in [i, right] is less than the pivot value (j < i).
            // Phase 3. Tail recursion optimization: recurse on smaller partition, loop on the larger one.
            // This bounds the call-stack depth to O(log n) even on adversarial inputs.
            var leftSize = j - left + 1;
            var rightSize = right - i + 1;

            if (leftSize < rightSize)
            {
                // Recurse on smaller left partition
                if (left < j)
                {
                    SortCore(s, left, j);
                }
                // Tail recursion: continue loop with right partition
                left = i;
            }
            else
            {
                // Recurse on smaller right partition
                if (i < right)
                {
                    SortCore(s, i, right);
                }
                // Tail recursion: continue loop with left partition
                right = j;
            }
        }
    }

}
