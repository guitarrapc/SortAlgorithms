using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 配列の先頭と末尾を比較し、必要に応じて交換した後、再帰的に部分配列をソートする極めて非効率なソートアルゴリズムです。
/// 配列が3要素以上の場合、最初の2/3、最後の2/3、再び最初の2/3の順にソートを行います。
/// <br/>
/// Compares the first and last elements of the array, swaps them if necessary, then recursively sorts subarrays.
/// For arrays with 3 or more elements, it sorts the first 2/3, then the last 2/3, and finally the first 2/3 again.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Stooge Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Boundary Swap:</strong> If the first element is greater than the last element, they must be swapped.
/// This ensures the smallest and largest elements move toward their correct positions.</description></item>
/// <item><description><strong>Recursive Division (2/3 Rule):</strong> For subarrays with 3 or more elements, the algorithm recursively sorts:
/// <list type="bullet">
/// <item><description>The first ⌈2n/3⌉ elements (first 2/3)</description></item>
/// <item><description>The last ⌈2n/3⌉ elements (last 2/3)</description></item>
/// <item><description>The first ⌈2n/3⌉ elements again (first 2/3)</description></item>
/// </list>
/// This triple recursion ensures the maximum element "bubbles" to the end.</description></item>
/// <item><description><strong>Base Case:</strong> Subarrays with 1 or 2 elements are already sorted after the boundary swap (if any).
/// The recursion terminates when start >= end.</description></item>
/// <item><description><strong>Overlapping Subarrays:</strong> The first 2/3 and last 2/3 must overlap by at least 1/3 of the elements.
/// This overlap is critical for correctness, ensuring elements are properly compared and positioned.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Joke / Exchange</description></item>
/// <item><description>Stable      : No (swapping non-adjacent elements does not preserve relative order)</description></item>
/// <item><description>In-place    : Yes (O(log n) recursive stack space only)</description></item>
/// <item><description>Best case   : Ω(n^(log 3 / log 1.5)) ≈ Ω(n^2.71) - Recurrence relation T(n) = 3T(2n/3) + O(1)</description></item>
/// <item><description>Average case: Θ(n^(log 3 / log 1.5)) ≈ Θ(n^2.71) - Same as best case, data-independent</description></item>
/// <item><description>Worst case  : O(n^(log 3 / log 1.5)) ≈ O(n^2.71) - Asymptotically slower than even bubble sort O(n²)</description></item>
/// <item><description>Comparisons : Θ(n^2.71) - One comparison at each of the ~n^2.71 recursive calls</description></item>
/// <item><description>Swaps       : O(n^2.71) - At most one swap per recursive call</description></item>
/// </list>
/// <para><strong>Note:</strong> Stooge Sort is a deliberately inefficient algorithm used for educational purposes to demonstrate
/// that not all recursive divide-and-conquer algorithms are efficient. The "multiply and surrender" strategy (triple recursion on overlapping 2/3 segments)
/// leads to excessive redundant work, making it impractical for any real-world use.</para>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Stooge_sort</para>
/// </remarks>
public static class StoogeSort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array

    /// <summary>
    /// Sorts the elements in the specified span in ascending order using the default comparer.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort in place.</param>
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
        => Sort(span, Comparer<T>.Default, NullContext.Default);

    /// <summary>
    /// Sorts the elements in the specified span using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
        => Sort(span, Comparer<T>.Default, context);

    /// <summary>
    /// Sorts the elements in the specified span using the provided comparer and sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use. Must implement <see cref="IComparer{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="comparer">The comparer to use for element comparisons.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T, TComparer>(Span<T> span, TComparer comparer, ISortContext context) where TComparer : IComparer<T>
    {
        if (span.Length <= 1) return;
        var s = new SortSpan<T, TComparer>(span, context, comparer, BUFFER_MAIN);

        SortCore(s, 0, span.Length - 1);
    }

    private static void SortCore<T, TComparer>(SortSpan<T, TComparer> s, int start, int end) where TComparer : IComparer<T>
    {
        if (start >= end) return;

        // If first element is larger than last, swap them
        if (s.Compare(start, end) > 0)
        {
            s.Swap(start, end);
        }

        // If there are 3 or more elements
        if (end - start + 1 >= 3)
        {
            var third = (end - start + 1) / 3;

            // Sort first 2/3
            SortCore(s, start, end - third);

            // Sort last 2/3
            SortCore(s, start + third, end);

            // Sort first 2/3 again
            SortCore(s, start, end - third);
        }
    }
}
