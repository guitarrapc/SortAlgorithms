using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 配列を再帰的に半分に分割し、左半分の最大値と右半分の最大値を比較・交換した後、最大値を除いた部分を再帰的にソートする極めて非効率なソートアルゴリズムです。
/// "Multiply and Surrender"戦略（分割統治の非効率版）を用いた教育目的のアルゴリズムで、実用性は皆無です。
/// <br/>
/// Recursively divides the array in half, compares and swaps the maximum values from the left and right halves,
/// then recursively sorts the remaining portion excluding the maximum value. This is an extremely inefficient sorting algorithm
/// using the "Multiply and Surrender" strategy (an inefficient variant of divide-and-conquer), intended purely for educational purposes with no practical use.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Slow Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Divide Step:</strong> The array [start, end] is divided into two halves:
/// <list type="bullet">
/// <item><description>Left half: [start, m] where m = ⌊(start + end) / 2⌋</description></item>
/// <item><description>Right half: [m + 1, end]</description></item>
/// </list>
/// This division ensures all elements are processed.</description></item>
/// <item><description><strong>Recursive Sort of Halves:</strong> Both halves are recursively sorted using the same SlowSort algorithm.
/// After these recursive calls:
/// <list type="bullet">
/// <item><description>The maximum element of the left half is at position m</description></item>
/// <item><description>The maximum element of the right half is at position end</description></item>
/// </list>
/// This property holds because each recursive call places the maximum element at the rightmost position of its subarray.</description></item>
/// <item><description><strong>Maximum Element Comparison and Swap:</strong> After sorting both halves, the algorithm compares the maximum elements:
/// <list type="bullet">
/// <item><description>If array[m] > array[end], swap them</description></item>
/// <item><description>This ensures the overall maximum element is at position end</description></item>
/// </list>
/// This is the key step that guarantees the maximum element "bubbles" to the end of the current subarray.</description></item>
/// <item><description><strong>Recursive Sort of Remaining Elements:</strong> After placing the maximum element at position end,
/// recursively sort [start, end - 1] to sort the remaining elements.
/// This is similar to selection sort's strategy but implemented recursively with divide-and-conquer.</description></item>
/// <item><description><strong>Base Case:</strong> The recursion terminates when start >= end (subarray has 0 or 1 element).
/// Such subarrays are already sorted by definition.</description></item>
/// </list>
/// <para><strong>Correctness Proof Sketch:</strong></para>
/// <list type="bullet">
/// <item><description><strong>Invariant:</strong> After SlowSort(start, end) completes, the maximum element in [start, end] is at position end, and all elements in [start, end] are sorted.</description></item>
/// <item><description><strong>Induction:</strong> Assume the algorithm works correctly for all subarrays smaller than n.
/// For a subarray of size n:
/// <list type="number">
/// <item><description>SlowSort(start, m) and SlowSort(m+1, end) correctly sort their respective halves (by inductive hypothesis)</description></item>
/// <item><description>The swap ensures max(array[start..end]) is at position end</description></item>
/// <item><description>SlowSort(start, end-1) correctly sorts the remaining elements (by inductive hypothesis)</description></item>
/// <item><description>Result: array[start..end] is sorted with maximum at end</description></item>
/// </list>
/// </description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Joke / Divide-and-Conquer (Multiply and Surrender)</description></item>
/// <item><description>Stable      : No (swapping non-adjacent elements does not preserve relative order)</description></item>
/// <item><description>In-place    : Yes (O(log n) recursive stack space only)</description></item>
/// <item><description>Best case   : Ω(n^(log₂ n / 2)) ≈ Ω(n^(log n)) - Recurrence: T(n) = 2T(n/2) + T(n-1) + O(1)</description></item>
/// <item><description>Average case: Θ(n^(log₂ n / 2)) ≈ Θ(n^(log n)) - Data-independent recursion structure</description></item>
/// <item><description>Worst case  : O(n^(log₂ n / 2)) ≈ O(n^(log n)) - Asymptotically worse than O(n²), one of the slowest known sorting algorithms</description></item>
/// <item><description>Comparisons : Θ(n^(log n)) - One comparison at each of the exponentially many recursive calls</description></item>
/// <item><description>Swaps       : O(n^(log n)) - At most one swap per recursive call, but depends on data ordering</description></item>
/// </list>
/// <para><strong>Note:</strong> Slow Sort is a deliberately inefficient algorithm created by Andrei Broder and Jorge Stolfi in 1984
/// to demonstrate the "Multiply and Surrender" paradigm—a pessimal variant of divide-and-conquer.
/// Unlike efficient divide-and-conquer algorithms (like merge sort) that have non-overlapping subproblems,
/// Slow Sort's third recursive call overlaps with the previous two, leading to exponential time complexity.
/// It has no practical use and exists purely for educational and humorous purposes.</para>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Slowsort</para>
/// </remarks>
public static class SlowSort
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
        => Sort(span, new ComparableComparer<T>(), NullContext.Default);

    /// <summary>
    /// Sorts the elements in the specified span using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}\"/>.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T, TContext>(Span<T> span, TContext context)
        where T : IComparable<T>
        where TContext : ISortContext
        => Sort(span, new ComparableComparer<T>(), context);

    /// <summary>
    /// Sorts the elements in the specified span using the provided comparer and sort context.
    /// This is the full-control version with explicit TContext type parameter.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use. Must implement <see cref="IComparer{T}"/>.</typeparam>
    /// <typeparam name="TContext">The type of the sort context.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="comparer">The comparer to use for element comparisons.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        SortCore(s, 0, span.Length - 1);
    }

    private static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> span, int start, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (start >= end) return;

        var m = (start + end) / 2;

        // Recursively sort left half: maximum element will be at position m
        SortCore(span, start, m);

        // Recursively sort right half: maximum element will be at position end
        SortCore(span, m + 1, end);

        // Ensure the overall maximum is at position end
        if (span.Compare(m, end) > 0)
        {
            span.Swap(m, end);
        }

        // Recursively sort the remaining elements (excluding the maximum at end)
        SortCore(span, start, end - 1);
    }
}
