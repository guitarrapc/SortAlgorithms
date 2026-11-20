using SortAlgorithm.Contexts;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 選択ソートの両方向版です。各パスで未ソート領域の最小値と最大値を同時に求め、それぞれ左端と右端に置きます。
/// 要素を2つずつ組にして組の中で1回比較し、小さい方だけを最小値と、大きい方だけを最大値と比べるため、比較は2要素あたり3回で済みます。
/// パス数は選択ソートの約半分です。
/// <br/>
/// A bidirectional selection sort. Each pass finds both the minimum and the maximum of the unsorted region and places them at its left and right ends.
/// Elements are taken in pairs: a pair is compared once, then only its smaller element is compared with the minimum and only its larger with the maximum,
/// so a pass costs three comparisons per two elements. It needs about half as many passes as selection sort.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Double Selection Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Dual Partition Invariant:</strong> Maintain three regions in the array: a sorted prefix [0..left), an unsorted middle [left..right], and a sorted suffix (right..n).
/// After iteration k, the first k elements contain the k smallest elements in sorted order, and the last k elements contain the k largest elements in sorted order.
/// This invariant must hold at the start and end of each iteration.</description></item>
/// <item><description><strong>Pairwise Min-Max Selection:</strong> An odd-sized region starts both candidates at its first element, an even-sized one at its first pair,
/// and the rest of the region is taken in pairs. Each pair is compared once; then its smaller element is compared with the minimum candidate and its larger
/// element with the maximum candidate. An element that loses inside its pair cannot be the extreme it lost toward, so three comparisons per pair overlook nothing.
/// The minimum is the first smallest element and the maximum the last largest, the tie rule of C++ <c>std::minmax_element</c>.</description></item>
/// <item><description><strong>Index-Aware Placement:</strong> Swap the minimum to left first. If the maximum sat at left, that swap carried it to the minimum's
/// old position, so follow it there before swapping it to right. A region of equal keys has its first copy at left and its last at right, so it swaps nothing.</description></item>
/// <item><description><strong>Dual Boundary Advancement:</strong> After each iteration, increment left by 1 and decrement right by 1.
/// This simultaneously shrinks the unsorted region from both ends until left >= right (0 or 1 element remains, which is automatically in place).</description></item>
/// <item><description><strong>Comparison Consistency:</strong> All element comparisons must use a total order relation (transitive, antisymmetric, total).
/// The IComparable&lt;T&gt;.CompareTo implementation must satisfy these properties for correctness.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Selection</description></item>
/// <item><description>Stable      : No (swapping non-adjacent elements can change relative order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space)</description></item>
/// <item><description>Best case   : Θ(n²) - the comparisons do not depend on the input; sorted input swaps nothing</description></item>
/// <item><description>Average case: Θ(n²) - about 3n²/8 comparisons, a quarter fewer than selection sort's n²/2</description></item>
/// <item><description>Worst case  : Θ(n²) - the same comparisons; at most two swaps per pass</description></item>
/// <item><description>Comparisons : Θ(n²) - ⌈3k/2⌉ - 2 for a pass over k elements, with k = n, n-2, n-4, ...; about 3n²/8 in total</description></item>
/// <item><description>Swaps       : O(n) - at most two per pass over ⌊n/2⌋ passes; 0 on sorted input and on equal keys</description></item>
/// <item><description>Index Reads : Θ(n²) - each pass reads every element of its region once or twice (see Implementation Notes); each swap adds two</description></item>
/// <item><description>Index Writes: O(n) - two per swap; nothing else writes</description></item>
/// <item><description>Space       : O(1) - the two candidates and their indices</description></item>
/// <item><description>Passes      : ⌊n/2⌋ - each pass settles two elements</description></item>
/// </list>
/// <para><strong>Use Cases:</strong></para>
/// <list type="bullet">
/// <item><description>Situations where reducing iteration overhead is beneficial (even with same asymptotic complexity)</description></item>
/// <item><description>Educational purposes to demonstrate algorithm optimization techniques</description></item>
/// <item><description>When write operations are expensive and you want to minimize swap count compared to bubble sort</description></item>
/// <item><description>Expensive comparisons, where the pairwise scan saves a quarter of selection sort's comparisons</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <para>The pair comparison picks the smaller and larger index by adding its result to the index, as Rust's heapsort picks a child, so random input
/// does not mispredict a branch on it. Both elements are then read again by index for their candidate comparisons. <c>float</c>, <c>double</c> and
/// <c>Half</c> branch on the pair comparison instead and compare the values already read, because their natural comparison branches on NaN internally.
/// The comparisons are the same either way; the floating-point form reads each element once and reports its candidate comparisons with both operands as -1.
/// The candidates are held by value, and the <c>CurrentMin</c> and <c>CurrentMax</c> roles mark where they sit.</para>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Selection_sort</para>
/// <para>Cormen, T. H.; Leiserson, C. E.; Rivest, R. L.; Stein, C. (2009). Introduction to Algorithms (3rd ed.), §9.1 "Minimum and maximum". MIT Press.</para>
/// </remarks>
public static class DoubleSelectionSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by DoubleSelectionSortTests, which derives from SortTestsBase.</remarks>
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

        if (span.Length <= 1) return;

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        SortCore(s, first, last);
    }

    /// <summary>
    /// Sorts the subrange [first..last) using the provided sort context.
    /// This overload accepts a SortSpan directly for use by other algorithms that already have a SortSpan instance.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="first">The inclusive start index of the range to sort.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var left = first;
        var right = last - 1;

        while (left < right)
        {
            s.Context.OnPhase(SortPhase.DoubleSelectionFindMinMax, left, right);
            s.Context.OnRole(left, BUFFER_MAIN, RoleType.LeftPointer);
            s.Context.OnRole(right, BUFFER_MAIN, RoleType.RightPointer);

            // An odd-sized region starts both candidates at its first element, an even-sized one at its first pair,
            // so the rest of the region splits into whole pairs.
            int min, max, i;
            T minValue, maxValue;
            if (((right - left) & 1) == 0)
            {
                min = max = left;
                minValue = maxValue = s.Read(left);
                s.Context.OnRole(min, BUFFER_MAIN, RoleType.CurrentMin);
                i = left + 1;
            }
            else
            {
                if (s.IsLessAt(left + 1, left, out var second, out var head))
                {
                    (min, minValue, max, maxValue) = (left + 1, second, left, head);
                }
                else
                {
                    (min, minValue, max, maxValue) = (left, head, left + 1, second);
                }
                s.Context.OnRole(min, BUFFER_MAIN, RoleType.CurrentMin);
                s.Context.OnRole(max, BUFFER_MAIN, RoleType.CurrentMax);
                i = left + 2;
            }

            // Three comparisons per pair: the pair itself, its smaller element against the minimum and its larger against the maximum.
            // Strict "less" keeps the first smallest as the minimum and "not less" takes the last largest as the maximum.
            for (; i < right; i += 2)
            {
                if (typeof(T) == typeof(float) || typeof(T) == typeof(double) || typeof(T) == typeof(Half))
                {
                    // The natural comparison of these types branches on NaN internally, so picking by arithmetic removes no misprediction.
                    if (s.IsLessAt(i + 1, i, out var b, out var a))
                    {
                        if (s.IsLessThan(b, minValue)) TakeMin(s, ref min, ref minValue, max, i + 1, b);
                        if (!s.IsLessThan(a, maxValue)) TakeMax(s, ref max, ref maxValue, min, i, a);
                    }
                    else
                    {
                        if (s.IsLessThan(a, minValue)) TakeMin(s, ref min, ref minValue, max, i, a);
                        if (!s.IsLessThan(b, maxValue)) TakeMax(s, ref max, ref maxValue, min, i + 1, b);
                    }
                }
                else
                {
                    // Adding the result to the index avoids a branch that random input mispredicts about half the time.
                    var step = s.IsLessAt(i + 1, i) ? 1 : 0;
                    var lo = i + step;
                    var hi = i + 1 - step;
                    if (s.IsElementLessThan(lo, minValue, out var loValue)) TakeMin(s, ref min, ref minValue, max, lo, loValue);
                    if (!s.IsElementLessThan(hi, maxValue, out var hiValue)) TakeMax(s, ref max, ref maxValue, min, hi, hiValue);
                }
            }

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

    /// <summary>Makes the element at <paramref name="index"/> the minimum candidate.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void TakeMin<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, ref int min, ref T minValue, int max, int index, T value)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // The first element of an odd-sized region starts as both candidates.
        s.Context.OnRole(min, BUFFER_MAIN, min == max ? RoleType.CurrentMax : RoleType.None);
        min = index;
        minValue = value;
        s.Context.OnRole(min, BUFFER_MAIN, RoleType.CurrentMin);
    }

    /// <summary>Makes the element at <paramref name="index"/> the maximum candidate.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void TakeMax<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, ref int max, ref T maxValue, int min, int index, T value)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        s.Context.OnRole(max, BUFFER_MAIN, max == min ? RoleType.CurrentMin : RoleType.None);
        max = index;
        maxValue = value;
        s.Context.OnRole(max, BUFFER_MAIN, RoleType.CurrentMax);
    }
}
