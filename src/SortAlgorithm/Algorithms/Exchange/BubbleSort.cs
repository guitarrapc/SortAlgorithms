using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 配列の末尾から、隣接する要素を比較して交換を繰り返すことでソートを行います。各パスで最小値が配列の先頭に「浮かび上がる（bubble up）」ように移動します。
/// シンプルで理解しやすいですが、実用的には非効率なソートアルゴリズムです。
/// <br/>
/// Sorts by repeatedly comparing and swapping adjacent elements from the end of the array. Each pass causes the smallest value to "bubble up" to the front of the array.
/// Simple and easy to understand, but inefficient for practical use.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Bubble Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Adjacent Element Comparison:</strong> Only adjacent elements (at positions j and j-1) are compared.
/// This ensures the sorting is achieved purely through local exchanges.</description></item>
/// <item><description><strong>Conditional Swap:</strong> Elements are swapped only when they are out of order (s.Compare(j, j-1) &lt; 0).
/// This preserves the relative order of equal elements, making the algorithm stable.</description></item>
/// <item><description><strong>Bounded Iteration:</strong> The outer loop runs n-1 times, and each iteration settles at least one element into its final position.
/// After i iterations, the first i elements are guaranteed to be in their final sorted positions.</description></item>
/// <item><description><strong>Complete Coverage:</strong> Each pass examines all unsorted elements from the end to position i+1.
/// This guarantees that all inversions are eventually corrected.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Exchange</description></item>
/// <item><description>Stable      : Yes (equal elements are never swapped due to strict comparison)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space)</description></item>
/// <item><description>Best case   : O(n²) - Without early termination optimization, all comparisons are performed even for sorted input</description></item>
/// <item><description>Average case: O(n²) - Expected n(n-1)/4 swaps for random input</description></item>
/// <item><description>Worst case  : O(n²) - Reverse-sorted input requires n(n-1)/2 swaps</description></item>
/// <item><description>Comparisons : Exactly n(n-1)/2 comparisons in all cases</description></item>
/// <item><description>Swaps       : Best 0, Average n(n-1)/4, Worst n(n-1)/2</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Bubble_sort</para>
/// </remarks>
/// <typeparam name="T"></typeparam>
public static class BubbleSort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array

    /// <summary>
    /// Sorts the elements in the specified span in ascending order using the default comparer.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort in place.</param>
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
        => Sort(span, new ComparableComparer<T>(), NullContext.Default);

    /// <summary>
    /// Sorts the elements in the specified span using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
        => Sort(span, new ComparableComparer<T>(), context);

    /// <summary>
    /// Sorts the elements in the specified span using the provided comparer and sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use for element comparisons.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="comparer">The comparer to use for element comparisons.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T, TComparer>(Span<T> span, TComparer comparer, ISortContext context) where TComparer : IComparer<T>
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T, TComparer>(span, context, comparer, BUFFER_MAIN);

        for (var i = 0; i < s.Length; i++)
        {
            for (var j = s.Length - 1; j > i; j--)
            {
                if (s.Compare(j, j - 1) < 0)
                {
                    s.Swap(j, j - 1);
                }
            }
        }
    }
}
