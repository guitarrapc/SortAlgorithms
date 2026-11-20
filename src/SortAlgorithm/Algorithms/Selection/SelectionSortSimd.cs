using SortAlgorithm.Contexts;
using System.Runtime.Intrinsics;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 選択ソートの最小値探索をSIMD命令で行う版です。各パスは未ソート部分の最小値を求めて境界と交換する点で選択ソートと同じですが、
/// 最小値をベクトル単位の走査で求めます。1回目の走査でレーンごとの最小値をまとめて全体の最小値を求め、2回目の走査でその値が最初に現れる位置を探します。
/// 選ぶ要素は選択ソートと同じ最初の最小値なので、交換の列も選択ソートと一致します。
/// <br/>
/// Selection sort whose minimum search runs on SIMD instructions. Each pass, as in selection sort, finds the minimum of the unsorted part
/// and exchanges it with the boundary, but it finds that minimum by scanning whole vectors: a first scan folds the elements into lane minima
/// and reduces them to the minimum value, and a second scan finds where that value first occurs. It picks the same first minimum as
/// selection sort, so it performs the same exchanges.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct SIMD Selection Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Partition Invariant:</strong> Maintain a sorted prefix [0..i) and an unsorted suffix [i..n).
/// After iteration k, the first k elements are the k smallest elements in sorted order.</description></item>
/// <item><description><strong>Vector Minimum:</strong> Load the first vector of the suffix as the lane minima, and fold each further whole vector into
/// them with a lane-wise minimum. The smallest lane is the minimum of every element loaded, and the elements after the last whole vector are
/// compared with it one by one. No element is overlooked, because every element is either in a lane or compared after the reduction.</description></item>
/// <item><description><strong>First Occurrence:</strong> Test whole vectors from the front of the suffix for elements equal to the minimum, then the
/// remaining elements one by one, and take the first match. This is the element selection sort takes, so equal keys are exchanged no differently.</description></item>
/// <item><description><strong>Swap and Advance:</strong> Exchange the minimum with the element at i unless it is already there, then advance i.
/// A suffix narrower than one vector is searched one element at a time, as in selection sort.</description></item>
/// <item><description><strong>Comparison Consistency:</strong> The vector comparisons order the elements as their natural order does, which holds for the
/// primitive integer types the scan runs on. Every other type, and every other comparer, runs selection sort.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Selection</description></item>
/// <item><description>Stable      : No (swapping non-adjacent elements can change relative order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space)</description></item>
/// <item><description>Best case   : Θ(n²) - the first scan covers the whole suffix on every input; sorted input stops the second scan at its first vector</description></item>
/// <item><description>Average case: Θ(n²) - the second scan stops at the minimum, halfway on average</description></item>
/// <item><description>Worst case  : Θ(n²) - a minimum at the end of every suffix makes the second scan cover it too</description></item>
/// <item><description>Comparisons : Θ(n²) - per suffix of k elements, about k for the first scan and the lane reduction, plus the second scan up to the vector holding the minimum; n²/2 to n² in total</description></item>
/// <item><description>Swaps       : O(n) - At most n-1 swaps, one per pass whose minimum is not already at the boundary; 0 on sorted input</description></item>
/// <item><description>Index Reads : Θ(n²) - the first scan reads every element of the suffix once and the second scan again up to the minimum's vector; each swap adds two</description></item>
/// <item><description>Index Writes: O(n) - two per swap; nothing else writes</description></item>
/// <item><description>Space       : O(1) - a vector of lane minima, a broadcast of the minimum, and the minimum's index</description></item>
/// </list>
/// <para><strong>Use Cases:</strong></para>
/// <list type="bullet">
/// <item><description>Integer keys where selection sort's few writes are wanted and its scans dominate the time</description></item>
/// <item><description>Comparing a vector scan with selection sort's scalar scan: the same exchanges, more comparisons, far fewer instructions</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <para>The scan runs on <c>byte</c>, <c>sbyte</c>, <c>short</c>, <c>ushort</c>, <c>int</c>, <c>uint</c>, <c>long</c>, <c>ulong</c>, <c>nint</c> and
/// <c>nuint</c> compared by their natural order, with the widest of 512-, 256- and 128-bit vectors that the runtime accelerates.
/// A vector of W lanes is loaded only when the rest of the suffix holds W elements, so a suffix shorter than W, and every pass on a machine without
/// vector acceleration, runs <see cref="SelectionSort"/>'s scan. The lane minima are reduced one lane at a time.</para>
/// <para>An observed sort runs the same scans. A vector load is reported as W reads and a vector comparison as W comparisons, in index order;
/// the lane minima and the broadcast minimum are held in registers, so they are reported as -1. The counts therefore depend on W, which depends on the machine.</para>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Selection_sort</para>
/// <para>Microsoft STL vectorizes <c>std::min_element</c> and <c>std::minmax_element</c> for arithmetic element types: https://github.com/microsoft/STL/blob/main/stl/src/vector_algorithms.cpp</para>
/// </remarks>
public static class SelectionSortSimd
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by SelectionSortSimdTests, which derives from SortTestsBase.</remarks>
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
        if (SelectionVectorScan.IsVectorizable<T, TComparer>())
        {
            if (Vector512.IsHardwareAccelerated) first = SortVectors<T, TComparer, TContext, Vector512Lanes<T>, Vector512<T>>(s, first, last);
            else if (Vector256.IsHardwareAccelerated) first = SortVectors<T, TComparer, TContext, Vector256Lanes<T>, Vector256<T>>(s, first, last);
            else if (Vector128.IsHardwareAccelerated) first = SortVectors<T, TComparer, TContext, Vector128Lanes<T>, Vector128<T>>(s, first, last);
        }

        // A suffix narrower than one vector, or of a type the scan does not run on, is searched one element at a time.
        SelectionSort.SortCore(s, first, last);
    }

    /// <summary>
    /// Runs the passes whose suffix holds at least one vector and returns where the remaining suffix starts.
    /// </summary>
    static int SortVectors<T, TComparer, TContext, TLanes, TVec>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        where TLanes : struct, IVectorLanes<TVec, T>
    {
        var i = first;
        for (; last - i >= TLanes.Count; i++)
        {
            s.Context.OnPhase(SortPhase.SelectionFindMin, i, last - 1);
            var min = SelectionVectorScan.IndexOfFirstMinimum<T, TComparer, TContext, TLanes, TVec>(s, i, last);
            s.Context.OnRole(min, BUFFER_MAIN, RoleType.CurrentMin);
            s.Context.OnRole(min, BUFFER_MAIN, RoleType.None);

            if (min != i)
            {
                s.Swap(min, i);
            }
        }
        return i;
    }
}
