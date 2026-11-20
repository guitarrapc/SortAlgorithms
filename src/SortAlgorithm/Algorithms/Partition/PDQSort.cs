using System.Numerics;
using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// ランダム化quicksortの高速な平均ケースとheapsortの高速な最悪ケースを組み合わせ、特定のパターンを持つ入力に対して線形時間を実現する改良されたquicksort変種で、David MusserのIntrosortの拡張および改善版です。
/// <br/>
/// Improved quicksort variant and extension and improvement of David Musser's introsort, that combines the fast average case of randomized quicksort with the fast worst case of heapsort, while achieving linear time on
/// inputs with certain patterns.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct PDQSort:</strong></para>
/// <list type="number">
/// <item><description><strong>Adaptive Algorithm Selection:</strong> PDQSort must correctly choose between sub-algorithms:
/// <list type="bullet">
/// <item><description>InsertionSort when partition size &lt; 24 (InsertionSortThreshold)</description></item>
/// <item><description>HeapSort when the bad partitions on one recursion path reach badAllowed = ⌊log₂(n)⌋</description></item>
/// <item><description>PartialInsertionSort when already-partitioned pattern detected</description></item>
/// <item><description>QuickSort with adaptive pivot selection for all other cases</description></item>
/// </list>
/// This adaptive selection ensures O(n log n) worst-case while maintaining O(n) best-case for patterns.</description></item>
/// <item><description><strong>Bad Partition Detection and Limit:</strong> The bad partition limit must be set to log₂(n).
/// A partition is "bad" (highly unbalanced) when either side is &lt; n/8. When this limit is exceeded,
/// it indicates adversarial input patterns, triggering a switch to HeapSort which guarantees O(n log n).</description></item>
/// <item><description><strong>Pivot Selection - Ninther for Large Arrays:</strong> For arrays larger than 128 elements (NintherThreshold),
/// the pivot is selected as the median-of-medians (pseudomedian of 9 elements):
/// <list type="bullet">
/// <item><description>Three groups: {begin, mid, end-1}, {begin+1, mid-1, end-2}, {begin+2, mid+1, end-3}</description></item>
/// <item><description>Each group is sorted (Sort3), then the medians are sorted</description></item>
/// <item><description>The middle of the three medians becomes the pivot</description></item>
/// </list>
/// This ninther selection reduces the probability of worst-case partitioning to O(1/n⁹) from O(1/n³) (median-of-3).</description></item>
/// <item><description><strong>Partition Right Scheme:</strong> The primary partitioning scheme places equal elements in the right partition:
/// <list type="bullet">
/// <item><description>Bidirectional scan: left pointer advances while element &lt; pivot, right pointer retreats while element ≥ pivot</description></item>
/// <item><description>Boundary guards prevent out-of-bounds access when all elements are smaller/larger than pivot</description></item>
/// <item><description>Returns (pivotPos, alreadyPartitioned flag) for pattern detection</description></item>
/// <item><description>alreadyPartitioned flag is true when first ≥ last before any swaps, indicating pre-sorted partition</description></item>
/// </list>
/// This scheme enables detection of already-sorted sequences for O(n) behavior.</description></item>
/// <item><description><strong>Partition Left for Equal Elements:</strong> When pivot equals previous partition boundary (begin-1),
/// switch to PartitionLeft which places equal elements in the left partition:
/// <list type="bullet">
/// <item><description>This optimization handles inputs with many duplicate elements</description></item>
/// <item><description>Left partition becomes fully sorted (all equal to pivot), requiring no recursion</description></item>
/// <item><description>Achieves O(n) time for arrays with all equal elements</description></item>
/// </list>
/// This is triggered by comparing the pivot with *(begin-1), the pivot of an earlier partition, which no element of [begin, end) is smaller than.</description></item>
/// <item><description><strong>Partial Insertion Sort for Pattern Detection:</strong> When a partition appears already sorted (alreadyPartitioned flag),
/// attempt insertion sort with a limit of 8 element moves (PartialInsertionSortLimit):
/// <list type="bullet">
/// <item><description>If ≤ 8 elements are moved, the partition is nearly sorted → complete the insertion sort (return true)</description></item>
/// <item><description>If &gt; 8 elements are moved, the partition is not sorted → abort and use quicksort (return false)</description></item>
/// <item><description>Applied to both left and right partitions; if both succeed, entire range is sorted</description></item>
/// </list>
/// This achieves O(n) time for sorted and nearly-sorted inputs.</description></item>
/// <item><description><strong>Pattern-Defeating Shuffles:</strong> When a bad partition is detected, shuffle elements to break adversarial patterns:
/// <list type="bullet">
/// <item><description>Swap elements at positions: begin ↔ begin+n/4, pivotPos-1 ↔ pivotPos-n/4 (and similar for right partition)</description></item>
/// <item><description>For large partitions (&gt; 128), perform additional swaps at +1, +2 offsets</description></item>
/// <item><description>The swaps are deterministic; they break the patterns that keep producing bad pivots (e.g., anti-quicksort permutations)</description></item>
/// </list>
/// Without shuffling, such inputs would keep producing bad partitions until the heapsort fallback.</description></item>
/// <item><description><strong>Unguarded Insertion Sort Optimization:</strong> For non-leftmost partitions, use unguarded insertion sort:
/// <list type="bullet">
/// <item><description>*(begin-1) acts as a sentinel (guaranteed ≤ any element in [begin, end))</description></item>
/// <item><description>Eliminates boundary check (sift != begin) in the inner loop</description></item>
/// <item><description>Removes the index test from the inner loop; the comparisons are the same</description></item>
/// </list>
/// This optimization is safe because quicksort's partitioning ensures *(begin-1) ≤ pivot ≤ all elements in right partition.</description></item>
/// <item><description><strong>Tail Recursion Elimination:</strong> After partitioning into [begin, pivotPos) and [pivotPos+1, end),
/// the algorithm recursively processes the smaller partition and loops on the larger partition, which bounds the call stack at
/// O(log n). pdqsort itself recurses on the left partition; both sides see the same range, bad-partition budget and leftmost flag
/// either way, so the comparisons and moves are the same, and only the order in which the two sides are sorted differs.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Hybrid (Partition (base) + Heap + Insertion)</description></item>
/// <item><description>Stable      : No (partitioning and heapsort are unstable)</description></item>
/// <item><description>In-place    : Yes (O(log n) auxiliary space for recursion stack)</description></item>
/// <item><description>Best case   : Θ(n) - Sorted, reverse sorted, all equal elements (pattern detection + partial insertion sort)</description></item>
/// <item><description>Average case: Θ(n log n) - Median-of-3 and ninther pivots split near the median on average</description></item>
/// <item><description>Worst case  : O(n log n) - Guaranteed by HeapSort fallback when the bad partition limit is reached</description></item>
/// <item><description>Comparisons : Θ(n log n) average, Θ(n) best, O(n log n) worst - each partition compares every element of its range with the pivot</description></item>
/// <item><description>Swaps       : Θ(n log n) average, O(1) best - the partition exchanges misplaced pairs; pivot selection and the pattern-breaking shuffles add a constant number per partition</description></item>
/// <item><description>Index Reads : Θ(n log n) average, Θ(n) best - every partition level reads each element of the range it splits; a detected pattern is settled by the partial insertion sort instead</description></item>
/// <item><description>Index Writes: Θ(n log n) average, O(1) best - each swap writes two positions, and the pivot placement and the insertion sorts add their own writes; sorted input is partitioned once, and the ninther swap and the pivot placement write four positions</description></item>
/// <item><description>Space       : O(log n) - recursion stack only</description></item>
/// </list>
/// <para><strong>Comparer contract:</strong></para>
/// <para>The unguarded partition scans and the unguarded insertion sort stop at elements that earlier comparisons put in their way, which
/// only a consistent comparer guarantees. Element access is unchecked, so those cursors are also checked against their range. The natural
/// order of a primitive type is always consistent, and the checks are compiled out for it. A comparer that is not a consistent total order
/// can make the sort throw <see cref="InvalidOperationException"/>. Either way the range keeps its elements, in an unspecified order, and
/// nothing outside the range is read or written.</para>
/// <para><strong>Reference:</strong></para>
/// <para>Paper: https://arxiv.org/abs/2106.05123</para>
/// <para>Other implementation: https://github.com/orlp/pdqsort</para>
/// </remarks>
public static class PDQSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by PDQSortTests, which derives from SortTestsBase.</remarks>
    public static bool IsStable => false;

    // InsertionSortThreshold: 24 elements (empirically optimal, balances overhead vs. efficiency)
    // NintherThreshold: 128 elements (above this, use ninther; below this, use median-of-3)
    // PartialInsertionSortLimit: 8 element moves (threshold for detecting nearly-sorted partitions)
    // Bad partition limit: log₂(n) (allows some imbalance before triggering HeapSort)
    // Bad partition criterion: Either partition &lt; n/8 (highly unbalanced)

    // Constants
    private const int InsertionSortThreshold = 24;
    private const int NintherThreshold = 128;
    private const int PartialInsertionSortLimit = 8;

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
    /// <typeparam name="TComparer">The type of the comparer.</typeparam>
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
    /// <typeparam name="TContext">The type of the sort context.</typeparam>
    /// <param name="span">The span containing elements to sort.</param>
    /// <param name="first">The inclusive start index of the range to sort.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    /// <param name="comparer">The comparer to use for element comparisons.</param>
    /// <param name="context">The sort context that tracks statistics and provides sorting operations.</param>
    public static void Sort<T, TComparer, TContext>(Span<T> span, int first, int last, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        ArgumentOutOfRangeException.ThrowIfNegative(first);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(last, span.Length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(first, last);

        if (last - first <= 1) return;

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);

        // A natural-order NaN prefix needs no further partitioning.
        int nanEnd = FloatingPointUtils.MoveNaNsToFront(s, first, last);

        if (nanEnd >= last)
        {
            // All values are NaN, already "sorted"
            return;
        }

        // Sort the remaining range.
        var badAllowed = Log2(last - nanEnd);
        PDQSortLoop(s, nanEnd, last, badAllowed, true);
    }

    /// <summary>
    /// Internal entry point for other algorithms (e.g., SpreadSort) that already hold a <see cref="SortSpan{T,TComparer,TContext}"/>.
    /// Sorts the subrange [first..last) using PDQSort.
    /// </summary>
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (last - first <= 1) return;
        var badAllowed = Log2(last - first);
        PDQSortLoop(s, first, last, badAllowed, true);
    }

    /// <summary>
    /// Main PDQSort loop with tail recursion elimination.
    /// </summary>
    private static void PDQSortLoop<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int begin, int end, int badAllowed, bool leftmost)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (true)
        {
            var size = end - begin;

            // Use insertion sort for small arrays
            if (size < InsertionSortThreshold)
            {
                s.Context.OnRole(begin, BUFFER_MAIN, RoleType.None);
                s.Context.OnPhase(SortPhase.HybridToInsertionSort, begin, end - 1, InsertionSortThreshold);

                if (leftmost)
                {
                    InsertionSort.SortCore(s, begin, end);
                }
                else
                {
                    InsertionSort.UnguardedSortCore(s, begin, end);
                }
                return;
            }

            // Choose pivot as median of 3 or pseudomedian of 9 (ninther)
            var s2 = size / 2;
            if (size > NintherThreshold)
            {
                // Ninther: median of medians for better pivot selection
                Sort3(s, begin, begin + s2, end - 1);
                Sort3(s, begin + 1, begin + (s2 - 1), end - 2);
                Sort3(s, begin + 2, begin + (s2 + 1), end - 3);
                Sort3(s, begin + (s2 - 1), begin + s2, begin + (s2 + 1));
                s.Swap(begin, begin + s2);
            }
            else
            {
                Sort3(s, begin + s2, begin, end - 1);
            }

            // If *(begin - 1) is the end of the right partition of a previous partition operation,
            // there is no element in [begin, end) that is smaller than *(begin - 1).
            // Then if our pivot compares equal to *(begin - 1) we change strategy.
            if (!leftmost && s.IsGreaterOrEqualAt(begin - 1, begin))
            {
                s.Context.OnPhase(SortPhase.QuickSortPartition, begin, end - 1, begin);
                s.Context.OnRole(begin, BUFFER_MAIN, RoleType.Pivot);
                var pivotBegin = begin;
                begin = PartitionLeft(s, begin, end) + 1;
                s.Context.OnRole(pivotBegin, BUFFER_MAIN, RoleType.None);
                continue;
            }

            // Partition and get results.
            s.Context.OnPhase(SortPhase.QuickSortPartition, begin, end - 1, begin);
            s.Context.OnRole(begin, BUFFER_MAIN, RoleType.Pivot);
            var (pivotPos, alreadyPartitioned) = PartitionRight(s, begin, end);
            s.Context.OnRole(begin, BUFFER_MAIN, RoleType.None);

            // Check for a highly unbalanced partition.
            var lSize = pivotPos - begin;
            var rSize = end - (pivotPos + 1);
            var highlyUnbalanced = lSize < size / 8 || rSize < size / 8;

            // If we got a highly unbalanced partition we shuffle elements to break many patterns
            if (highlyUnbalanced)
            {
                s.Context.OnPhase(SortPhase.PDQPatternShuffle, begin, end - 1, badAllowed);
                // If we had too many bad partitions, switch to heapsort to guarantee O(n log n)
                if (--badAllowed == 0)
                {
                    s.Context.OnPhase(SortPhase.HybridToHeapSort, begin, end - 1);
                    HeapSort.SortFallbackCore(s, begin, end);
                    return;
                }

                if (lSize >= InsertionSortThreshold)
                {
                    s.Swap(begin, begin + lSize / 4);
                    s.Swap(pivotPos - 1, pivotPos - lSize / 4);

                    if (lSize > NintherThreshold)
                    {
                        s.Swap(begin + 1, begin + (lSize / 4 + 1));
                        s.Swap(begin + 2, begin + (lSize / 4 + 2));
                        s.Swap(pivotPos - 2, pivotPos - (lSize / 4 + 1));
                        s.Swap(pivotPos - 3, pivotPos - (lSize / 4 + 2));
                    }
                }

                if (rSize >= InsertionSortThreshold)
                {
                    s.Swap(pivotPos + 1, pivotPos + (1 + rSize / 4));
                    s.Swap(end - 1, end - rSize / 4);

                    if (rSize > NintherThreshold)
                    {
                        s.Swap(pivotPos + 2, pivotPos + (2 + rSize / 4));
                        s.Swap(pivotPos + 3, pivotPos + (3 + rSize / 4));
                        s.Swap(end - 2, end - (1 + rSize / 4));
                        s.Swap(end - 3, end - (2 + rSize / 4));
                    }
                }
            }
            else
            {
                // If we were decently balanced and we tried to sort an already partitioned
                // sequence try to use insertion sort.
                if (alreadyPartitioned)
                {
                    s.Context.OnPhase(SortPhase.PDQPartialInsertionSort, begin, end - 1);
                    if (PartialInsertionSort(s, begin, pivotPos) &&
                        PartialInsertionSort(s, pivotPos + 1, end))
                    {
                        return;
                    }
                }
            }

            // Recurse on the smaller side and loop on the larger, which bounds the call stack at O(log n). pdqsort recurses on the left
            // side; both sides see the same range, bad-partition budget and leftmost flag either way, so the comparisons are the same.
            if (lSize < rSize)
            {
                PDQSortLoop(s, begin, pivotPos, badAllowed, leftmost);
                begin = pivotPos + 1;
                leftmost = false;
            }
            else
            {
                PDQSortLoop(s, pivotPos + 1, end, badAllowed, false);
                end = pivotPos;
            }
        }
    }

    /// <summary>
    /// Partitions [begin, end) around pivot *begin. Elements equal to the pivot are put in the right-hand partition.
    /// Returns the position of the pivot after partitioning and whether the passed sequence already was correctly partitioned.
    /// Assumes the pivot is a median of at least 3 elements and that [begin, end) is at least
    /// insertion_sort_threshold long.
    /// </summary>
    private static (int pivotPos, bool alreadyPartitioned) PartitionRight<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int begin, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Move pivot into local for speed
        var pivot = s.Read(begin);

        var first = begin;
        var last = end;

        // The unguarded scans stop at elements that earlier comparisons put in their way. Only a consistent comparer guarantees that,
        // and element access is unchecked, so they also check their cursor against the range. The natural order of a primitive type is
        // consistent, so the checks fold away for it.
        var checkCursors = !s.UsesNaturalPrimitiveOrder;

        // Find the first element greater than or equal to the pivot (the median of 3 guarantees this exists).
        do { first++; if (checkCursors && first == end) ThrowHelper.ThrowInconsistentComparer(); } while (s.IsElementLessThan(first, pivot));

        // Find the first element strictly smaller than the pivot. We have to guard this search if
        // there was no element before *first.
        if (first - 1 == begin)
        {
            while (first < last && s.IsElementGreaterOrEqual(--last, pivot)) { }
        }
        else
        {
            do { last--; if (checkCursors && last < begin) ThrowHelper.ThrowInconsistentComparer(); } while (s.IsElementGreaterOrEqual(last, pivot));
        }

        // If the first pair of elements that should be swapped to partition are the same element,
        // the passed in sequence already was correctly partitioned
        var alreadyPartitioned = first >= last;

        // Keep swapping pairs of elements that are on the wrong side of the pivot. Previously
        // swapped pairs guard the searches, which is why the first iteration is special-cased
        // above.
        while (first < last)
        {
            s.Swap(first, last);
            do { first++; if (checkCursors && first == end) ThrowHelper.ThrowInconsistentComparer(); } while (s.IsElementLessThan(first, pivot));
            do { last--; if (checkCursors && last < begin) ThrowHelper.ThrowInconsistentComparer(); } while (s.IsElementGreaterOrEqual(last, pivot));
        }

        // Put the pivot in the right place
        var pivotPos = first - 1;
        s.Write(begin, s.Read(pivotPos));
        s.Write(pivotPos, pivot);

        return (pivotPos, alreadyPartitioned);
    }

    /// <summary>
    /// Partitions [begin, end) around pivot *begin. Elements equal to the pivot are put to the left.
    /// Used when many equal elements are detected.
    /// Since this is rarely used (the many equal case), and in that case pdqsort already has O(n)
    /// performance, no block quicksort is applied here for simplicity.
    /// </summary>
    private static int PartitionLeft<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int begin, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var pivot = s.Read(begin);
        var first = begin;
        var last = end;

        // As in PartitionRight, the unguarded scans also check their cursor against the range.
        var checkCursors = !s.UsesNaturalPrimitiveOrder;

        // Find the first element less than or equal to pivot from the right.
        // *begin == pivot, so the scan is guaranteed to terminate at begin at the latest.
        do { last--; if (checkCursors && last < begin) ThrowHelper.ThrowInconsistentComparer(); } while (s.IsValueLessThan(pivot, last));

        // Find the first element strictly greater than pivot from the left. We have to guard
        // this search if there was no element after *last.
        if (last + 1 == end)
        {
            while (first < last && s.IsValueGreaterOrEqual(pivot, ++first)) { }
        }
        else
        {
            do { first++; if (checkCursors && first == end) ThrowHelper.ThrowInconsistentComparer(); } while (s.IsValueGreaterOrEqual(pivot, first));
        }

        // Keep swapping pairs. Previously swapped pairs guard the searches.
        while (first < last)
        {
            s.Swap(first, last);
            do { last--; if (checkCursors && last < begin) ThrowHelper.ThrowInconsistentComparer(); } while (s.IsValueLessThan(pivot, last));
            do { first++; if (checkCursors && first == end) ThrowHelper.ThrowInconsistentComparer(); } while (s.IsValueGreaterOrEqual(pivot, first));
        }

        var pivotPos = last;
        s.Write(begin, s.Read(pivotPos));
        s.Write(pivotPos, pivot);

        return pivotPos;
    }

    /// <summary>
    /// Attempts to use insertion sort on [begin, end). Will return false if more than
    /// PartialInsertionSortLimit elements were moved, and abort sorting. Otherwise it will
    /// successfully sort and return true.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool PartialInsertionSort<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int begin, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (begin == end) return true;

        var limit = 0;
        for (var cur = begin + 1; cur < end; cur++)
        {
            var sift = cur;

            // Compare first so we can avoid 2 moves for an element already positioned correctly.
            // The comparisons hand back the elements they read, so each shift writes a value already held.
            if (s.IsLessAt(sift, sift - 1, out var siftValue, out var shifted))
            {
                while (true)
                {
                    s.Write(sift, shifted);
                    sift--;
                    if (sift == begin || !s.IsValueLessThan(siftValue, sift - 1, out shifted)) break;
                }

                s.Write(sift, siftValue);
                limit += cur - sift;
            }

            if (limit > PartialInsertionSortLimit)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Sorts 3 elements at positions a, b, c.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Sort3<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int a, int b, int c)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (s.IsLessAt(b, a)) s.Swap(a, b);
        if (s.IsLessAt(c, b)) s.Swap(b, c);
        if (s.IsLessAt(b, a)) s.Swap(a, b);
    }

    /// <summary>
    /// Returns floor(log2(n)), assumes n > 0.
    /// Uses hardware-accelerated bit operations for optimal performance.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Log2(int n)
    {
        // var log = 0;
        // while (n > 1)
        // {
        //     n >>= 1;
        //     log++;
        // }
        // return log;
        return BitOperations.Log2((uint)n);
    }
}
