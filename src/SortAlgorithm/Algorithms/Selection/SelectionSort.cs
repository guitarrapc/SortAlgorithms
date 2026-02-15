using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 配列を境界で2つの部分（ソート済み部分と未ソート部分）に分割し、未ソート部分から最小要素を見つけて境界位置と交換します。
/// この操作を境界を進めながら繰り返すことでソートを完了します。インデックスベースの交換により不安定なソートアルゴリズムです。
/// <br/>
/// Divides the array into two parts (sorted and unsorted) at a boundary, finds the minimum element in the unsorted portion,
/// and swaps it with the element at the boundary position. Repeats this operation while advancing the boundary to complete sorting.
/// Index-based swapping makes this an unstable sorting algorithm.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Selection Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Partition Invariant:</strong> Maintain two regions in the array: a sorted prefix [0..i) and an unsorted suffix [i..n).
/// After iteration k, the first k elements contain the k smallest elements in sorted order.
/// This invariant must hold at the start and end of each iteration.</description></item>
/// <item><description><strong>Minimum Selection:</strong> For each position i in [0..n-1), correctly identify the minimum element
/// in the unsorted region [i..n). This requires comparing the candidate minimum with every element in the unsorted portion,
/// ensuring no smaller element is overlooked.</description></item>
/// <item><description><strong>Swap Operation:</strong> Exchange the minimum element from the unsorted region with the element at position i.
/// This places the (i+1)-th smallest element at index i, extending the sorted region by one.
/// Skip the swap if the minimum is already at position i (optimization that doesn't affect correctness).</description></item>
/// <item><description><strong>Boundary Advancement:</strong> After each swap, increment the boundary index i by 1.
/// This shrinks the unsorted region and grows the sorted region until the entire array is sorted.
/// Terminate when i reaches n-1 (only one element remains, which is automatically in place).</description></item>
/// <item><description><strong>Comparison Consistency:</strong> All element comparisons must use a total order relation (transitive, antisymmetric, total).
/// The IComparable&lt;T&gt;.CompareTo implementation must satisfy these properties for correctness.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Selection</description></item>
/// <item><description>Stable      : No (swapping non-adjacent elements can change relative order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space)</description></item>
/// <item><description>Best case   : Θ(n²) - Always performs n(n-1)/2 comparisons regardless of input order</description></item>
/// <item><description>Average case: Θ(n²) - Same comparison count; swap count varies but doesn't dominate</description></item>
/// <item><description>Worst case  : Θ(n²) - Same comparison count; maximum n-1 swaps when reverse sorted</description></item>
/// <item><description>Comparisons : Θ(n²) - Exactly n(n-1)/2 comparisons in all cases (input-independent)</description></item>
/// <item><description>Swaps       : O(n) - At most n-1 swaps; best case 0 (already sorted), worst case n-1</description></item>
/// <item><description>Writes      : O(n) - 2 writes per swap (via tuple deconstruction or temp variable)</description></item>
/// </list>
/// <para><strong>Use Cases:</strong></para>
/// <list type="bullet">
/// <item><description>Small datasets where simplicity is valued over performance</description></item>
/// <item><description>Situations where write operations are expensive (minimizes swaps compared to bubble sort)</description></item>
/// <item><description>Educational purposes to teach fundamental sorting concepts</description></item>
/// <item><description>When memory writes are costly but comparisons are cheap</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Selection_sort</para>
/// </remarks>
public static class SelectionSort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array

    private readonly struct SelectionSortAction<T, TComparer> : ContextDispatcher.SortAction<T, TComparer>
        where TComparer : IComparer<T>
    {
        private readonly int _first;
        private readonly int _last;

        public SelectionSortAction(int first, int last)
        {
            _first = first;
            _last = last;
        }

        public void Invoke<TContext>(Span<T> span, TComparer comparer, TContext context)
            where TContext : ISortContext
        {
            Sort<T, TComparer, TContext>(span, _first, _last, comparer, context);
        }
    }

    /// <summary>
    /// Sorts the elements in the specified span in ascending order using the default comparer.
    /// Uses NullContext for zero-overhead fast path.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort in place.</param>
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
        => Sort<T, ComparableComparer<T>, NullContext>(span, 0, span.Length, new ComparableComparer<T>(), NullContext.Default);

    /// <summary>
    /// Sorts the elements in the specified span using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
        => ContextDispatcher.DispatchSort(span, new ComparableComparer<T>(), context, new SelectionSortAction<T, ComparableComparer<T>>(0, span.Length));

    /// <summary>
    /// Sorts the subrange [first..last) using the provided sort context.
    /// Elements in [first..start) are assumed to already be sorted.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span containing the elements to sort.</param>
    /// <param name="first">The zero-based index of the first element in the range to sort.</param>
    /// <param name="last">The exclusive upper bound of the range to sort (one past the last element).</param>
    /// <param name="context">The sort context to use during the sorting operation for tracking statistics and visualization.</param>
    public static void Sort<T>(Span<T> span, int first, int last, ISortContext context) where T : IComparable<T>
        => ContextDispatcher.DispatchSort(span, new ComparableComparer<T>(), context, new SelectionSortAction<T, ComparableComparer<T>>(first, last));

    /// <summary>
    /// Sorts the subrange [first..last) using the provided comparer and sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use for element comparisons.</typeparam>
    /// <param name="span">The span containing the elements to sort.</param>
    /// <param name="first">The zero-based index of the first element in the range to sort.</param>
    /// <param name="last">The exclusive upper bound of the range to sort (one past the last element).</param>
    /// <param name="comparer">The comparer to use for element comparisons.</param>
    /// <param name="context">The sort context to use during the sorting operation for tracking statistics and visualization.</param>
    public static void Sort<T, TComparer>(Span<T> span, int first, int last, TComparer comparer, ISortContext context)
        where TComparer : IComparer<T>
        => ContextDispatcher.DispatchSort(span, comparer, context, new SelectionSortAction<T, TComparer>(first, last));

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
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(first, last);

        if (span.Length <= 1) return;

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
        for (var i = first; i < last - 1; i++)
        {
            var min = i;

            // Find the index of the minimum element
            for (var j = i + 1; j < last; j++)
            {
                if (s.Compare(j, min) < 0)
                {
                    min = j;
                }
            }

            // Swap the found minimum element with the first element of the unsorted part
            if (min != i)
            {
                s.Swap(min, i);
            }
        }
    }
}
