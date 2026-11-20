using SortAlgorithm.Contexts;
using System.Numerics;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// Musser (1997) のイントロソートです。中央値3点のクイックソートで分割を進め、分割の深さが 2⌊log₂ n⌋ に達した範囲はヒープソートに切り替え、
/// 16要素以下の範囲は残しておき、最後に全体を1回の挿入ソートで仕上げます。
/// <br/>
/// Musser's (1997) introsort. It partitions with median-of-3 quicksort, switches a range to heapsort once the partitioning depth reaches
/// 2⌊log₂ n⌋, leaves ranges of 16 or fewer elements unsorted, and finishes with one insertion sort over the whole range.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Introsort:</strong></para>
/// <list type="number">
/// <item><description><strong>Introsort loop:</strong> While the range holds more than size_threshold = 16 elements, a depth counter starting
/// at 2⌊log₂ n⌋ is decremented for every partition on the current path. When it reaches 0, the range is sorted by heapsort and the loop
/// ends. Otherwise the range is partitioned, the loop recurses on the right part and continues with the left part. The depth limit bounds
/// the recursion depth, so the paper recurses on the right part without comparing the sizes of the two parts.</description></item>
/// <item><description><strong>Median-of-3 pivot:</strong> The pivot is the median value of A[f], A[f + (b - f)/2] and A[b - 1], found
/// with 2-3 comparisons. The samples are not moved; only the value is used.</description></item>
/// <item><description><strong>Unguarded partition:</strong> The scan from the left stops at an element not less than the pivot, the scan
/// from the right at an element not greater than it; if they have not crossed, the two elements are exchanged and the left scan moves on.
/// The median sample and the elements already exchanged stop the scans, so they need no bound. Elements equal to the pivot stop both scans
/// and are exchanged, which keeps the split balanced on duplicate-heavy input.</description></item>
/// <item><description><strong>Final insertion sort:</strong> The loop leaves every element within its range of at most 16 elements, and
/// every element of a range precedes every element of the next one. One insertion sort over the whole range then moves each element
/// only within its range, so the pass costs O(n) for a fixed size_threshold.</description></item>
/// <item><description><strong>Worst-case bound:</strong> Partitioning costs O(n) per level and there are at most 2⌊log₂ n⌋ levels, and
/// heapsort sorts each range it is handed in O(m log m), so the total is O(n log n).</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Hybrid (Partition (base) + Heap + Insertion)</description></item>
/// <item><description>Stable      : No (partitioning and heapsort exchange elements across equal keys)</description></item>
/// <item><description>In-place    : Yes (O(log n) recursion depth, bounded by the depth limit)</description></item>
/// <item><description>Best case   : Θ(n log n) - every range above the threshold is partitioned; sorted input splits evenly at every level</description></item>
/// <item><description>Average case: Θ(n log n) - median-of-3 pivots split near the median on average</description></item>
/// <item><description>Worst case  : O(n log n) - the depth limit hands a range that keeps splitting badly to heapsort</description></item>
/// <item><description>Comparisons : Θ(n log n) - each partition compares every element of its range with the pivot, about (12/7) n ln n ≈ 1.19 n log₂ n on distinct keys, less the levels below the threshold; the final insertion sort adds O(n)</description></item>
/// <item><description>Swaps       : O(n log n) - the partitions exchange misplaced pairs, and elements equal to the pivot are exchanged too; the heapsort fallback swaps each extracted maximum</description></item>
/// <item><description>Index Reads : Θ(n log n) - every partition level reads each element of the range it splits, and the final insertion sort reads each element at least once</description></item>
/// <item><description>Index Writes: O(n log n) - each swap writes two positions, the final insertion sort writes the elements it shifts, and the heapsort fallback writes through its sift holes</description></item>
/// <item><description>Space       : O(log n) - recursion only, at most 2⌊log₂ n⌋ levels deep</description></item>
/// </list>
/// <para><strong>Implementation notes:</strong></para>
/// <list type="bullet">
/// <item><description>size_threshold is 16, the value of the HP STL's <c>__stl_threshold</c> that the paper's code uses.</description></item>
/// <item><description>The heapsort fallback is <see cref="HeapSort"/>'s top-down heapsort; the paper only requires an O(n log n) stopper.</description></item>
/// <item><description>A natural-order floating-point range first moves its NaNs to the front, without comparisons, as the other
/// partition sorts here do; the depth limit is taken from the length that remains.</description></item>
/// </list>
/// <para><strong>Comparer contract:</strong></para>
/// <para>The partition scans stop at the median sample and at the elements already exchanged, which only a consistent comparer guarantees.
/// Element access is unchecked, so the cursors are also checked against the range. The natural order of a primitive type is always
/// consistent, and the checks are compiled out for it. A comparer that is not a consistent total order can make the sort throw
/// <see cref="InvalidOperationException"/>. Either way the range keeps its elements, in an unspecified order, and nothing outside the range
/// is read or written.</para>
/// <para><strong>Reference:</strong></para>
/// <para>Paper: David R. Musser, "Introspective Sorting and Selection Algorithms", Software: Practice and Experience 27(8), 983-993 (1997). https://webpages.charlotte.edu/rbunescu/courses/ou/cs4040/introsort.pdf</para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Introsort</para>
/// </remarks>
public static class IntroSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by IntroSortTests, which derives from SortTestsBase.</remarks>
    public static bool IsStable => false;

    // Ranges of at most this many elements are left to the final insertion sort (the HP STL's __stl_threshold).
    private const int SizeThreshold = 16;

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

        IntroSortLoop(s, nanEnd, last, 2 * BitOperations.Log2((uint)(last - nanEnd)));

        s.Context.OnPhase(SortPhase.HybridToInsertionSort, nanEnd, last - 1, SizeThreshold);
        InsertionSort.SortCore(s, nanEnd, last);
    }

    /// <summary>
    /// Partitions [f, b) until every range holds at most <see cref="SizeThreshold"/> elements, sorting a range by heapsort once
    /// <paramref name="depthLimit"/> partitions on its path have been made. Recurses on the right part and continues with the left part.
    /// </summary>
    private static void IntroSortLoop<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int f, int b, int depthLimit)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (b - f > SizeThreshold)
        {
            if (depthLimit == 0)
            {
                s.Context.OnPhase(SortPhase.HybridToHeapSort, f, b - 1);
                HeapSort.SortFallbackCore(s, f, b);
                return;
            }

            depthLimit--;

            var pivotIndex = MedianOf3Index(s, f, f + (b - f) / 2, b - 1);
            s.Context.OnPhase(SortPhase.QuickSortPartition, f, b - 1, pivotIndex);
            s.Context.OnRole(pivotIndex, BUFFER_MAIN, RoleType.Pivot);
            var p = UnguardedPartition(s, f, b, s.Read(pivotIndex));
            s.Context.OnRole(pivotIndex, BUFFER_MAIN, RoleType.None);

            IntroSortLoop(s, p, b, depthLimit);
            b = p;
        }
    }

    /// <summary>
    /// Partitions [first, last) around <paramref name="pivot"/>, a value with an element not less than it and an element not greater than
    /// it in the range. Returns the cut: no element before it is greater than the pivot, and no element from it on is less.
    /// </summary>
    private static int UnguardedPartition<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last, T pivot)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // The median sample and the elements already exchanged stop the scans, which only a consistent comparer guarantees. Element access is
        // unchecked, so the cursors are also checked against the range. The natural order of a primitive type is consistent, so the checks
        // fold away for it.
        var checkCursors = !s.UsesNaturalPrimitiveOrder;
        var begin = first;
        var end = last;

        while (true)
        {
            while (s.IsElementLessThan(first, pivot))
            {
                first++;
                if (checkCursors && first == end) ThrowHelper.ThrowInconsistentComparer();
            }

            last--;
            while (s.IsValueLessThan(pivot, last))
            {
                last--;
                if (checkCursors && last < begin) ThrowHelper.ThrowInconsistentComparer();
            }

            if (!(first < last)) return first;

            s.Swap(first, last);
            first++;
        }
    }

    /// <summary>
    /// Returns the index of the median of the elements at <paramref name="a"/>, <paramref name="b"/> and <paramref name="c"/>, with the
    /// comparisons of the HP STL's <c>__median</c>.
    /// </summary>
    private static int MedianOf3Index<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int a, int b, int c)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (s.IsLessAt(a, b))
        {
            if (s.IsLessAt(b, c)) return b;
            if (s.IsLessAt(a, c)) return c;
            return a;
        }
        if (s.IsLessAt(a, c)) return a;
        if (s.IsLessAt(b, c)) return c;
        return b;
    }
}
