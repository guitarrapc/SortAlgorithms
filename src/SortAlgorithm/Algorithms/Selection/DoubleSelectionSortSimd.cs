using SortAlgorithm.Contexts;
using System.Runtime.Intrinsics;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 両方向選択ソートの最小値・最大値探索をSIMD命令で行う版です。各パスで未ソート領域の最小値と最大値を求めて左端と右端に置く点は両方向選択ソートと同じです。
/// 隣り合う2本のベクトルの要素をレーンごとに組にし、組の中で1回比較して小さい方をレーンごとの最小値と、大きい方をレーンごとの最大値と比べます。
/// 求めた値が最初に現れる位置と最後に現れる位置を探すため、交換の列は両方向選択ソートと一致します。
/// <br/>
/// Double selection sort whose minimum and maximum search runs on SIMD instructions. Each pass, as in double selection sort, finds the minimum
/// and the maximum of the unsorted region and places them at its left and right ends. Elements are paired lane by lane across two adjacent vectors:
/// each pair is compared once, and its smaller side is compared with the lane minima and its larger with the lane maxima. It then locates the
/// first occurrence of the minimum and the last of the maximum, so it performs the same exchanges as double selection sort.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct SIMD Double Selection Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Dual Partition Invariant:</strong> Maintain a sorted prefix [0..left), an unsorted middle [left..right], and a sorted suffix (right..n).
/// After iteration k, the first k elements are the k smallest in sorted order and the last k the k largest.</description></item>
/// <item><description><strong>Pairwise Vector Min-Max:</strong> Take the region two vectors at a time and pair lane k of the first with lane k of the second.
/// Each pair is compared once; its smaller element is folded into the lane minima and its larger into the lane maxima, three comparisons per two elements
/// as in the scalar pairwise scan. The lanes are reduced to one minimum and one maximum, and the elements after the last pair of whole vectors are
/// paired with their neighbours, with an element left over compared with both.</description></item>
/// <item><description><strong>Located Extremes:</strong> Find the first element equal to the minimum by testing whole vectors from the left, and the last
/// element equal to the maximum by testing them from the right. These are the elements double selection sort takes, so equal keys are exchanged
/// no differently, and a region of equal keys swaps nothing.</description></item>
/// <item><description><strong>Index-Aware Placement:</strong> Swap the minimum to left first. If the maximum sat at left, that swap carried it to the
/// minimum's old position, so follow it there before swapping it to right. Then advance left and retreat right.</description></item>
/// <item><description><strong>Comparison Consistency:</strong> The vector comparisons order the elements as their natural order does, which holds for the
/// primitive integer types the scan runs on. Every other type, and every other comparer, runs double selection sort. A region narrower than two
/// vectors is finished by double selection sort as well.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Selection</description></item>
/// <item><description>Stable      : No (swapping non-adjacent elements can change relative order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space)</description></item>
/// <item><description>Best case   : Θ(n²) - the pairwise scan covers the whole region on every input; sorted input stops both location scans at the region's ends</description></item>
/// <item><description>Average case: Θ(n²) - each location scan stops at its extreme</description></item>
/// <item><description>Worst case  : Θ(n²) - extremes far from the ends they are searched from make the location scans cover the region again</description></item>
/// <item><description>Comparisons : Θ(n²) - per region of k elements, about 3k/2 for the pairwise scan and the lane reductions, plus the two location scans up to the vectors holding the extremes</description></item>
/// <item><description>Swaps       : O(n) - at most two per pass over ⌊n/2⌋ passes; 0 on sorted input and on equal keys</description></item>
/// <item><description>Index Reads : Θ(n²) - the pairwise scan reads every element of the region once, and each location scan again up to its extreme's vector; each swap adds two</description></item>
/// <item><description>Index Writes: O(n) - two per swap; nothing else writes</description></item>
/// <item><description>Space       : O(1) - vectors of lane minima and maxima, broadcasts of the extremes, and their indices</description></item>
/// <item><description>Passes      : ⌊n/2⌋ - each pass settles two elements</description></item>
/// </list>
/// <para><strong>Use Cases:</strong></para>
/// <list type="bullet">
/// <item><description>Integer keys where double selection sort's few writes are wanted and its scans dominate the time</description></item>
/// <item><description>Comparing a vector scan with double selection sort's scalar scan: the same exchanges, more comparisons, far fewer instructions</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <para>The scan runs on <c>byte</c>, <c>sbyte</c>, <c>short</c>, <c>ushort</c>, <c>int</c>, <c>uint</c>, <c>long</c>, <c>ulong</c>, <c>nint</c> and
/// <c>nuint</c> compared by their natural order, with the widest of 512-, 256- and 128-bit vectors that the runtime accelerates.
/// With W lanes a pass runs the vector scans while the region holds 2W elements; the rest of the sort, and every pass on a machine without vector
/// acceleration, runs <see cref="DoubleSelectionSort"/>. The lane minima and maxima are reduced one lane at a time.</para>
/// <para>An observed sort runs the same scans. A vector load is reported as W reads and a vector comparison as W comparisons, in index order.
/// A pair comparison names both elements; the lane minima and maxima, the smaller and larger side of each pair, and the broadcast extremes are held in
/// registers, so they are reported as -1. The counts therefore depend on W, which depends on the machine.</para>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Selection_sort</para>
/// <para>Cormen, T. H.; Leiserson, C. E.; Rivest, R. L.; Stein, C. (2009). Introduction to Algorithms (3rd ed.), §9.1 "Minimum and maximum". MIT Press.</para>
/// <para>Microsoft STL vectorizes <c>std::min_element</c> and <c>std::minmax_element</c> for arithmetic element types: https://github.com/microsoft/STL/blob/main/stl/src/vector_algorithms.cpp</para>
/// </remarks>
public static class DoubleSelectionSortSimd
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by DoubleSelectionSortSimdTests, which derives from SortTestsBase.</remarks>
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
    public static void Sort<T, TComparer, TContext>(Span<T> span, int first, int last, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        ArgumentOutOfRangeException.ThrowIfNegative(first);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(last, span.Length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(first, last);

        if (last - first <= 1) return;

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        SortCore(s, first, last);
    }

    /// <summary>
    /// Sorts the subrange [first..last) using the provided sort context.
    /// This overload accepts a SortSpan directly for use by other algorithms that already have a SortSpan instance.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use for element comparisons.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="first">The inclusive start index of the range to sort.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var left = first;
        var right = last - 1;
        if (SelectionVectorScan.IsVectorizable<T, TComparer>())
        {
            if (Vector512.IsHardwareAccelerated) SortVectors<T, TComparer, TContext, Vector512Lanes<T>, Vector512<T>>(s, ref left, ref right);
            else if (Vector256.IsHardwareAccelerated) SortVectors<T, TComparer, TContext, Vector256Lanes<T>, Vector256<T>>(s, ref left, ref right);
            else if (Vector128.IsHardwareAccelerated) SortVectors<T, TComparer, TContext, Vector128Lanes<T>, Vector128<T>>(s, ref left, ref right);
        }

        // A region narrower than two vectors, or of a type the scan does not run on, is searched one pair at a time.
        DoubleSelectionSort.SortCore(s, left, right + 1);
    }

    /// <summary>
    /// Runs the passes whose region holds at least two vectors, leaving <paramref name="left"/> and <paramref name="right"/> at the region that remains.
    /// </summary>
    static void SortVectors<T, TComparer, TContext, TLanes, TVec>(SortSpan<T, TComparer, TContext> s, ref int left, ref int right)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        where TLanes : struct, IVectorLanes<TVec, T>
    {
        while (right - left + 1 >= 2 * TLanes.Count)
        {
            s.Context.OnPhase(SortPhase.DoubleSelectionFindMinMax, left, right);
            s.Context.OnRole(left, BUFFER_MAIN, RoleType.LeftPointer);
            s.Context.OnRole(right, BUFFER_MAIN, RoleType.RightPointer);

            var (minValue, maxValue) = SelectionVectorScan.MinMax<T, TComparer, TContext, TLanes, TVec>(s, left, right + 1);
            var min = SelectionVectorScan.IndexOfFirst<T, TComparer, TContext, TLanes, TVec>(s, left, right + 1, minValue);
            s.Context.OnRole(min, BUFFER_MAIN, RoleType.CurrentMin);
            var max = SelectionVectorScan.IndexOfLast<T, TComparer, TContext, TLanes, TVec>(s, left, right + 1, maxValue);
            s.Context.OnRole(max, BUFFER_MAIN, RoleType.CurrentMax);

            // Clear all roles before swaps
            s.Context.OnRole(left, BUFFER_MAIN, RoleType.None);
            s.Context.OnRole(right, BUFFER_MAIN, RoleType.None);
            s.Context.OnRole(min, BUFFER_MAIN, RoleType.None);
            s.Context.OnRole(max, BUFFER_MAIN, RoleType.None);

            // Moving the minimum carries whatever sat at left to min's old position, so a maximum at left follows it there.
            if (min != left)
            {
                s.Swap(min, left);
                if (max == left) max = min;
            }
            if (max != right)
            {
                s.Swap(max, right);
            }

            left++;
            right--;
        }
    }
}
