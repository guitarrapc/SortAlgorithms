using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 2つのピボットを使用して配列を3つの領域に分割する分割統治法のソートアルゴリズムです。
/// Vladimir Yaroslavskiy の2009年の論文に書かれたとおりのデュアルピボット・クイックソートで、ピボットは範囲の約 1/div と (div-1)/div の位置の2要素、
/// 中央領域が小さい分割が続くと div を増やし、大きな中央領域ではピボットと等しい要素を両端へ寄せてから再帰します。
/// <br/>
/// A divide-and-conquer sorting algorithm that uses two pivots to partition the array into three regions.
/// It is the dual-pivot quicksort of Vladimir Yaroslavskiy's 2009 paper as written: the pivots are the two elements about 1/div and
/// (div-1)/div into the range, div grows after a partition with a small center, and a large center first moves the elements equal to
/// the pivots to its ends.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Dual-Pivot QuickSort:</strong></para>
/// <list type="number">
/// <item><description><strong>Tiny ranges:</strong> A range with len = right - left &lt; 27 is sorted by insertion sort.</description></item>
/// <item><description><strong>Pivot selection:</strong> With third = len / div (div starts at 3), m1 = left + third and m2 = right - third,
/// kept inside (left, right). One comparison of a[m1] and a[m2] decides which goes to a[left] and which to a[right], so the pivots
/// p1 = a[left] and p2 = a[right] satisfy p1 ≤ p2.</description></item>
/// <item><description><strong>Three-way partition:</strong> One scan with k between less and great. a[k] &lt; p1 is exchanged to less;
/// a[k] &gt; p2 makes great step down over elements greater than p2, is exchanged with a[great], and the element it receives is checked
/// against p1 once. The regions are then [left+1, less) &lt; p1, [less, k) between the pivots, and (great, right) &gt; p2.</description></item>
/// <item><description><strong>Adapting div:</strong> dist = great - less is the size of the center minus one. When dist &lt; 13 the center was
/// small, the pivots were poor, and div grows by one for all three subranges.</description></item>
/// <item><description><strong>Pivot placement and recursion:</strong> The pivots are exchanged into less - 1 and great + 1, and the parts
/// left of p1 and right of p2 are sorted. The center is sorted only when p1 &lt; p2: with p1 = p2 every element there equals the pivots.</description></item>
/// <item><description><strong>Equal elements:</strong> When dist &gt; len - 13 and p1 ≠ p2, the center held nearly the whole range. Its
/// elements equal to p1 are exchanged to its left end, those equal to p2 to its right end (and the element received is checked against p1
/// once), and only the part between them is sorted.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Partitioning (Divide and Conquer)</description></item>
/// <item><description>Stable      : No (partitioning does not preserve relative order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(log n) auxiliary space for recursion stack)</description></item>
/// <item><description>Best case   : Θ(n) - all elements equal: the pivots are equal, the center is never sorted, and the outer parts are empty</description></item>
/// <item><description>Average case: Θ(n log n) - 1.9 n ln n ≈ 1.32 n log₂ n comparisons on random distinct keys (Wild and Nebel)</description></item>
/// <item><description>Worst case  : O(n²) - pivots that keep splitting off a constant number of elements</description></item>
/// <item><description>Comparisons : Θ(n log n) average, O(n²) worst - an element below p1 is compared once, every other element at least twice; 1.9 n ln n on average</description></item>
/// <item><description>Swaps       : Θ(n log n) average, O(n²) worst - 0.6 n ln n on average (Wild and Nebel), plus the pivot exchanges and the insertion sort leaves' moves</description></item>
/// <item><description>Index Reads : Θ(n log n) average, O(n²) worst - every partition level reads each element of the range it splits</description></item>
/// <item><description>Index Writes: Θ(n log n) average, O(n²) worst - each swap writes two positions, and the insertion sort leaves write the elements they shift</description></item>
/// <item><description>Space       : O(log n) - recursion stack only</description></item>
/// </list>
/// <para><strong>Partition scheme:</strong> Yaroslavskiy's 3-way dual-pivot partition — neither Hoare nor Lomuto.</para>
/// <para><strong>Implementation notes:</strong></para>
/// <list type="bullet">
/// <item><description>The paper sorts the left part, the right part, and then the center. Here the two smaller of the three parts are
/// sorted recursively and the largest by the loop, which bounds the stack at O(log n); every part is partitioned the same way.</description></item>
/// <item><description>The paper tests p1 ≠ p2 and p1 &lt; p2 separately. Because p1 ≤ p2, one answer serves both, and when a[m1] &lt; a[m2]
/// placed the pivots it is already known.</description></item>
/// <item><description>The tiny ranges use <see cref="InsertionSort"/>, which shifts where the paper's loop swaps neighbours; the
/// comparisons are the same.</description></item>
/// </list>
/// <para><strong>Differences from Java's DualPivotQuicksort:</strong></para>
/// <para>Java 7 takes the pivots as the 2nd and 4th of five sorted samples around the middle, sorts ranges below 47 by insertion sort, and
/// drops the div rule; JDK 14 rewrote it again. Those are later variants with other constants: with Java 7's sampling the averages fall to
/// about 1.70 n ln n comparisons and 0.55 n ln n swaps (Nebel and Wild). This class follows the 2009 paper.</para>
/// <para><strong>Reference:</strong></para>
/// <para>Paper: Vladimir Yaroslavskiy, "Dual-Pivot Quicksort" (2009). https://web.archive.org/web/20151002230717/http://iaroslavski.narod.ru/quicksort/DualPivotQuicksort.pdf</para>
/// <para>Paper: https://arxiv.org/abs/1310.7409 Average Case Analysis of Java 7's Dual Pivot Quicksort / Sebastian Wild, Markus E. Nebel</para>
/// <para>Paper: https://arxiv.org/abs/1403.6602 Analysis of Pivot Sampling in Dual-Pivot Quicksort / Markus E. Nebel, Sebastian Wild</para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Quicksort</para>
/// </remarks>
public static class DualPivotQuickSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by DualPivotQuickSortTests, which derives from SortTestsBase.</remarks>
    public static bool IsStable => false;

    // A range with right - left below this is sorted by insertion sort (the paper's tiny-array test).
    private const int TinySize = 27;

    // A center with dist = great - less below this makes div grow; a center with dist above len - DistSize gets the equal-elements pass.
    private const int DistSize = 13;

    // The initial divisor of the pivot positions.
    private const int InitialDiv = 3;

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
    /// <typeparam name="TComparer">The type of comparer to use. Must implement <see cref="IComparer{T}"/>.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="span">The span containing elements to sort.</param>
    /// <param name="first">The inclusive start index of the range to sort.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    /// <param name="comparer">The comparer used to compare elements.</param>
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
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use. Must implement <see cref="IComparer{T}"/>.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="left">The inclusive start index of the range to sort.</param>
    /// <param name="right">The inclusive end index of the range to sort.</param>
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int right)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        => DualPivotQuicksort(s, left, right, InitialDiv);

    private static void DualPivotQuicksort<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int right, int div)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (true)
        {
            var len = right - left;

            // Insertion sort for tiny arrays.
            if (len < TinySize)
            {
                if (len > 0)
                {
                    s.Context.OnPhase(SortPhase.HybridToInsertionSort, left, right, TinySize);
                    InsertionSort.SortCore(s, left, right + 1);
                }
                return;
            }

            // "Medians": the elements about 1/div and (div-1)/div into the range, the smaller to the left end.
            var third = len / div;
            var m1 = left + third;
            var m2 = right - third;
            if (m1 <= left) m1 = left + 1;
            if (m2 >= right) m2 = right - 1;
            bool diffPivots;
            if (s.IsLessAt(m1, m2))
            {
                s.Swap(m1, left);
                s.Swap(m2, right);
                diffPivots = true;
            }
            else
            {
                s.Swap(m1, right);
                s.Swap(m2, left);
                diffPivots = false;
            }

            s.Context.OnPhase(SortPhase.QuickSortPartition, left, right, left);
            s.Context.OnRole(left, BUFFER_MAIN, RoleType.Pivot);
            s.Context.OnRole(right, BUFFER_MAIN, RoleType.Pivot);

            // Pivots. a[m1] < a[m2] already showed p1 < p2; otherwise p1 ≤ p2, and one comparison tells whether they differ.
            T pivot1, pivot2;
            if (diffPivots)
            {
                pivot1 = s.Read(left);
                pivot2 = s.Read(right);
            }
            else
            {
                diffPivots = s.IsLessAt(left, right, out pivot1, out pivot2);
            }

            // Sorting: a[left+1, less) < p1, a[less, k) between the pivots, a(great, right) > p2.
            var less = left + 1;
            var great = right - 1;
            for (var k = less; k <= great; k++)
            {
                // a[k] is read once; its second comparison uses the held value.
                if (s.IsElementLessThan(k, pivot1, out var ak))
                {
                    s.Swap(k, less++);
                }
                else if (s.IsGreaterThan(ak, pivot2))
                {
                    while (k < great && s.IsElementGreaterThan(great, pivot2))
                    {
                        great--;
                    }
                    s.Swap(k, great--);
                    if (s.IsElementLessThan(k, pivot1))
                    {
                        s.Swap(k, less++);
                    }
                }
            }

            // Swaps
            var dist = great - less;
            if (dist < DistSize) div++;
            s.Swap(less - 1, left);
            s.Swap(great + 1, right);

            s.Context.OnRole(left, BUFFER_MAIN, RoleType.None);
            s.Context.OnRole(right, BUFFER_MAIN, RoleType.None);

            // The outer parts are fixed now; the equal-elements pass below moves only elements of the center.
            var leftEnd = less - 2;
            var rightStart = great + 2;

            // Equal elements
            if (dist > len - DistSize && diffPivots)
            {
                for (var k = less; k <= great; k++)
                {
                    if (s.Compare(k, pivot1, out var ak) == 0)
                    {
                        s.Swap(k, less++);
                    }
                    else if (s.Compare(ak, pivot2) == 0)
                    {
                        s.Swap(k, great--);
                        if (s.Compare(k, pivot1) == 0)
                        {
                            s.Swap(k, less++);
                        }
                    }
                }
            }

            // Subarrays: the center is sorted only when the pivots differ. The two smaller parts are sorted recursively and the
            // largest by the loop, which bounds the stack at O(log n).
            var leftCount = leftEnd - left + 1;
            var midCount = diffPivots ? great - less + 1 : 0;
            var rightCount = right - rightStart + 1;

            if (leftCount >= midCount && leftCount >= rightCount)
            {
                if (diffPivots) DualPivotQuicksort(s, less, great, div);
                DualPivotQuicksort(s, rightStart, right, div);
                right = leftEnd;
            }
            else if (rightCount >= leftCount && rightCount >= midCount)
            {
                DualPivotQuicksort(s, left, leftEnd, div);
                if (diffPivots) DualPivotQuicksort(s, less, great, div);
                left = rightStart;
            }
            else
            {
                DualPivotQuicksort(s, left, leftEnd, div);
                DualPivotQuicksort(s, rightStart, right, div);
                left = less;
                right = great;
            }
        }
    }
}
