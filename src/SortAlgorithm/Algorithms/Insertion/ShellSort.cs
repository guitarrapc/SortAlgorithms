using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// シェルソートアルゴリズム - ギャップベースの比較を使用した改良版挿入ソート。
/// 配列を「ギャップ」で区切られたサブ配列に分割し、各サブ配列を挿入ソートでソートします。
/// ギャップが1に減少すると配列はほぼソート済みになり、最終的な挿入ソートパスが非常に効率的になります。
/// <br/>
/// Shell sort algorithm - an improved insertion sort using gap-based comparisons.
/// Shell sort divides the array into sub-arrays separated by a "gap" and sorts each sub-array using insertion sort.
/// As the gap reduces to 1, the array becomes nearly sorted, making the final insertion sort pass very efficient.
/// </summary>
/// <remarks>
/// <para>Uses h[0] = 1 and h[k+1] = 3h[k] + 1.</para>
/// <para>Each pass is a straight insertion sort of the elements h apart: it holds an element, moves larger ones h places up and writes it
/// into the slot left free (Knuth's Algorithm D). Gaps are used in decreasing order, ending at 1. Equal keys may move past one another,
/// so this algorithm is unstable.</para>
/// <para>Implementation note: the immutable table covers every gap selectable for an int-sized
/// range using gap &lt;= length/2. Gap selection uses the requested subrange length.</para>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Insertion (gap-based)</description></item>
/// <item><description>Stable      : No - gap passes may reorder equal keys</description></item>
/// <item><description>In-place    : Yes</description></item>
/// <item><description>Best case   : Θ(n log n) - ordered input</description></item>
/// <item><description>Average case: Unknown; O(n^{3/2}) upper bound</description></item>
/// <item><description>Worst case  : Θ(n^{3/2})</description></item>
/// <item><description>Comparisons : O(n^{3/2}) worst-case upper bound</description></item>
/// <item><description>Swaps       : 0 - elements move h places up and the held element drops into the slot left free; nothing is exchanged</description></item>
/// <item><description>Index Reads : O(n^{3/2}) worst-case upper bound - one per comparison and one per element a pass holds</description></item>
/// <item><description>Index Writes: O(n^{3/2}) worst-case upper bound - one per moved element and one per held element that leaves its slot</description></item>
/// <item><description>Space       : O(1) per call - immutable gaps and scalar indices</description></item>
/// </list>
/// <para>Average-case exponents and relative performance are not asserted as general guarantees.</para>
/// <para>Reference: Knuth, The Art of Computer Programming, Volume 3, Shellsort.</para>
/// </remarks>
public static class ShellSortKnuth1973
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by ShellSortKnuth1973Tests, which derives from SortTestsBase.</remarks>
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
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="first">The zero-based index of the first element in the range to sort.</param>
    /// <param name="last">The exclusive upper bound of the range to sort (one past the last element).</param>
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        => ShellSortPasses.SortCore(s, first, last, ShellGapSequences.Knuth);
}

/// <summary>
/// シェルソートアルゴリズム - ギャップベースの比較を使用した改良版挿入ソート。
/// 配列を「ギャップ」で区切られたサブ配列に分割し、各サブ配列を挿入ソートでソートします。
/// ギャップが1に減少すると配列はほぼソート済みになり、最終的な挿入ソートパスが非常に効率的になります。
/// <br/>
/// Shell sort algorithm - an improved insertion sort using gap-based comparisons.
/// Shell sort divides the array into sub-arrays separated by a "gap" and sorts each sub-array using insertion sort.
/// As the gap reduces to 1, the array becomes nearly sorted, making the final insertion sort pass very efficient.
/// </summary>
/// <remarks>
/// <para>Uses 9(2^k - 2^(k/2)) + 1 for even k and 8·2^k - 6·2^((k+1)/2) + 1 for odd k. This is the merged sequence 1, 5, 19, 41, 109, ... .</para>
/// <para>Each pass is a straight insertion sort of the elements h apart: it holds an element, moves larger ones h places up and writes it
/// into the slot left free (Knuth's Algorithm D). Gaps are used in decreasing order, ending at 1. Equal keys may move past one another,
/// so this algorithm is unstable.</para>
/// <para>Implementation note: the immutable table covers every gap selectable for an int-sized
/// range using gap &lt;= length/2. Gap selection uses the requested subrange length.</para>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Insertion (gap-based)</description></item>
/// <item><description>Stable      : No - gap passes may reorder equal keys</description></item>
/// <item><description>In-place    : Yes</description></item>
/// <item><description>Best case   : Θ(n log n) - ordered input</description></item>
/// <item><description>Average case: Unknown; O(n²) upper bound</description></item>
/// <item><description>Worst case  : O(n²) - general upper bound</description></item>
/// <item><description>Comparisons : O(n²) worst-case upper bound</description></item>
/// <item><description>Swaps       : 0 - elements move h places up and the held element drops into the slot left free; nothing is exchanged</description></item>
/// <item><description>Index Reads : O(n²) worst-case upper bound - one per comparison and one per element a pass holds</description></item>
/// <item><description>Index Writes: O(n²) worst-case upper bound - one per moved element and one per held element that leaves its slot</description></item>
/// <item><description>Space       : O(1) per call - immutable gaps and scalar indices</description></item>
/// </list>
/// <para>Average-case exponents and relative performance are not asserted as general guarantees.</para>
/// <para>Reference: Sedgewick, A New Upper Bound for Shellsort (1986), merged experimental sequence.</para>
/// </remarks>
/// <typeparam name="T"></typeparam>
public static class ShellSortSedgewick1986
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by ShellSortSedgewick1986Tests, which derives from SortTestsBase.</remarks>
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
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="first">The zero-based index of the first element in the range to sort.</param>
    /// <param name="last">The exclusive upper bound of the range to sort (one past the last element).</param>
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        => ShellSortPasses.SortCore(s, first, last, ShellGapSequences.Sedgewick);
}

/// <summary>
/// シェルソートアルゴリズム - ギャップベースの比較を使用した改良版挿入ソート。
/// 配列を「ギャップ」で区切られたサブ配列に分割し、各サブ配列を挿入ソートでソートします。
/// ギャップが1に減少すると配列はほぼソート済みになり、最終的な挿入ソートパスが非常に効率的になります。
/// <br/>
/// Shell sort algorithm - an improved insertion sort using gap-based comparisons.
/// Shell sort divides the array into sub-arrays separated by a "gap" and sorts each sub-array using insertion sort.
/// As the gap reduces to 1, the array becomes nearly sorted, making the final insertion sort pass very efficient.
/// </summary>
/// <remarks>
/// <para>Uses floor((4/5)((9/4)^(k+1) - 1)), starting at k = 0. This floor variant starts 1, 3, 8, 19, ... and is distinct from the ceiling variant.</para>
/// <para>Each pass is a straight insertion sort of the elements h apart: it holds an element, moves larger ones h places up and writes it
/// into the slot left free (Knuth's Algorithm D). Gaps are used in decreasing order, ending at 1. Equal keys may move past one another,
/// so this algorithm is unstable.</para>
/// <para>Implementation note: the immutable table covers every gap selectable for an int-sized
/// range using gap &lt;= length/2. Gap selection uses the requested subrange length.</para>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Insertion (gap-based)</description></item>
/// <item><description>Stable      : No - gap passes may reorder equal keys</description></item>
/// <item><description>In-place    : Yes</description></item>
/// <item><description>Best case   : Θ(n log n) - ordered input</description></item>
/// <item><description>Average case: Unknown; O(n²) upper bound</description></item>
/// <item><description>Worst case  : O(n²) - general upper bound</description></item>
/// <item><description>Comparisons : O(n²) worst-case upper bound</description></item>
/// <item><description>Swaps       : 0 - elements move h places up and the held element drops into the slot left free; nothing is exchanged</description></item>
/// <item><description>Index Reads : O(n²) worst-case upper bound - one per comparison and one per element a pass holds</description></item>
/// <item><description>Index Writes: O(n²) worst-case upper bound - one per moved element and one per held element that leaves its slot</description></item>
/// <item><description>Space       : O(1) per call - immutable gaps and scalar indices</description></item>
/// </list>
/// <para>Average-case exponents and relative performance are not asserted as general guarantees.</para>
/// <para>Reference: Tokuda gap family; the floor variant is specified by OEIS A361506.</para>
/// </remarks>
/// <typeparam name="T"></typeparam>
public static class ShellSortTokuda1992
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by ShellSortTokuda1992Tests, which derives from SortTestsBase.</remarks>
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
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="first">The zero-based index of the first element in the range to sort.</param>
    /// <param name="last">The exclusive upper bound of the range to sort (one past the last element).</param>
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        => ShellSortPasses.SortCore(s, first, last, ShellGapSequences.Tokuda);
}

/// <summary>
/// シェルソートアルゴリズム - ギャップベースの比較を使用した改良版挿入ソート。
/// 配列を「ギャップ」で区切られたサブ配列に分割し、各サブ配列を挿入ソートでソートします。
/// ギャップが1に減少すると配列はほぼソート済みになり、最終的な挿入ソートパスが非常に効率的になります。
/// <br/>
/// Shell sort algorithm - an improved insertion sort using gap-based comparisons.
/// Shell sort divides the array into sub-arrays separated by a "gap" and sorts each sub-array using insertion sort.
/// As the gap reduces to 1, the array becomes nearly sorted, making the final insertion sort pass very efficient.
/// </summary>
/// <remarks>
/// <para>Retains the prefix 1, 4, 10, 23, 57, 132, 301, 701, 1750. After 1750, the chosen empirical extension is h[k+1] = floor(9h[k]/4). The extension is not a uniquely defined infinite sequence from the original study.</para>
/// <para>Each pass is a straight insertion sort of the elements h apart: it holds an element, moves larger ones h places up and writes it
/// into the slot left free (Knuth's Algorithm D). Gaps are used in decreasing order, ending at 1. Equal keys may move past one another,
/// so this algorithm is unstable.</para>
/// <para>Implementation note: the immutable table covers every gap selectable for an int-sized
/// range using gap &lt;= length/2. Gap selection uses the requested subrange length.</para>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Insertion (gap-based)</description></item>
/// <item><description>Stable      : No - gap passes may reorder equal keys</description></item>
/// <item><description>In-place    : Yes</description></item>
/// <item><description>Best case   : Θ(n log n) - ordered input</description></item>
/// <item><description>Average case: Unknown; O(n²) upper bound</description></item>
/// <item><description>Worst case  : O(n²) - general upper bound</description></item>
/// <item><description>Comparisons : O(n²) worst-case upper bound</description></item>
/// <item><description>Swaps       : 0 - elements move h places up and the held element drops into the slot left free; nothing is exchanged</description></item>
/// <item><description>Index Reads : O(n²) worst-case upper bound - one per comparison and one per element a pass holds</description></item>
/// <item><description>Index Writes: O(n²) worst-case upper bound - one per moved element and one per held element that leaves its slot</description></item>
/// <item><description>Space       : O(1) per call - immutable gaps and scalar indices</description></item>
/// </list>
/// <para>Average-case exponents and relative performance are not asserted as general guarantees.</para>
/// <para>Reference: Ciura, Best Increments for the Average Case of Shellsort (2001).</para>
/// </remarks>
public static class ShellSortCiura2001
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by ShellSortCiura2001Tests, which derives from SortTestsBase.</remarks>
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
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="first">The zero-based index of the first element in the range to sort.</param>
    /// <param name="last">The exclusive upper bound of the range to sort (one past the last element).</param>
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        => ShellSortPasses.SortCore(s, first, last, ShellGapSequences.Ciura);
}

/// <summary>
/// シェルソートアルゴリズム - ギャップベースの比較を使用した改良版挿入ソート。
/// 配列を「ギャップ」で区切られたサブ配列に分割し、各サブ配列を挿入ソートでソートします。
/// ギャップが1に減少すると配列はほぼソート済みになり、最終的な挿入ソートパスが非常に効率的になります。
/// <br/>
/// Shell sort algorithm - an improved insertion sort using gap-based comparisons.
/// Shell sort divides the array into sub-arrays separated by a "gap" and sorts each sub-array using insertion sort.
/// As the gap reduces to 1, the array becomes nearly sorted, making the final insertion sort pass very efficient.
/// </summary>
/// <remarks>
/// <para>Uses ceil((γ^k - 1)/(γ - 1)) for k >= 1, with γ fixed to the displayed decimal 2.243609061420001. The decimal is interpreted exactly when deriving the rounded gaps.</para>
/// <para>Each pass is a straight insertion sort of the elements h apart: it holds an element, moves larger ones h places up and writes it
/// into the slot left free (Knuth's Algorithm D). Gaps are used in decreasing order, ending at 1. Equal keys may move past one another,
/// so this algorithm is unstable.</para>
/// <para>Implementation note: the immutable table covers every gap selectable for an int-sized
/// range using gap &lt;= length/2. Gap selection uses the requested subrange length.</para>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Insertion (gap-based)</description></item>
/// <item><description>Stable      : No - gap passes may reorder equal keys</description></item>
/// <item><description>In-place    : Yes</description></item>
/// <item><description>Best case   : Θ(n log n) - ordered input</description></item>
/// <item><description>Average case: Unknown; O(n²) upper bound</description></item>
/// <item><description>Worst case  : O(n²) - general upper bound</description></item>
/// <item><description>Comparisons : O(n²) worst-case upper bound</description></item>
/// <item><description>Swaps       : 0 - elements move h places up and the held element drops into the slot left free; nothing is exchanged</description></item>
/// <item><description>Index Reads : O(n²) worst-case upper bound - one per comparison and one per element a pass holds</description></item>
/// <item><description>Index Writes: O(n²) worst-case upper bound - one per moved element and one per held element that leaves its slot</description></item>
/// <item><description>Space       : O(1) per call - immutable gaps and scalar indices</description></item>
/// </list>
/// <para>Average-case exponents and relative performance are not asserted as general guarantees.</para>
/// <para>Reference: Lee, Empirically Improved Tokuda Gap Sequence in Shellsort (2021), arXiv:2112.11112.</para>
/// </remarks>
/// <typeparam name="T"></typeparam>
public static class ShellSortLee2021
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by ShellSortLee2021Tests, which derives from SortTestsBase.</remarks>
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
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="first">The inclusive start index of the range to sort.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        => ShellSortPasses.SortCore(s, first, last, ShellGapSequences.Lee);
}

/// <summary>
/// The gap passes the Shell sort classes share. They differ only in their gap sequence.
/// </summary>
internal static class ShellSortPasses
{
    /// <summary>
    /// Sorts [first..last) with one gapped straight insertion pass per gap, from the largest gap no greater than half the range down to 1.
    /// </summary>
    /// <remarks>
    /// Each pass holds an element, moves the larger elements of its chain h places up, and writes it into the slot left free, as in
    /// Knuth's Algorithm D. Exchanging neighbours in the chain instead makes the same comparisons but two writes per move.
    /// </remarks>
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last, ReadOnlySpan<int> gaps)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var length = last - first;
        if (length < 2) return;

        var gapIndex = gaps.Length - 1;
        ShellGapSequences.FindLast(gaps, length, ref gapIndex);
        var totalGaps = gapIndex + 1;

        for (; gapIndex >= 0; gapIndex--)
        {
            var h = gaps[gapIndex];
            s.Context.OnPhase(SortPhase.ShellGapPass, h, totalGaps - gapIndex, totalGaps);

            for (var i = first + h; i < last; i++)
            {
                var tmp = s.Read(i);
                // An element no smaller than the one h below it stays where it is.
                if (s.IsElementLessOrEqual(i - h, tmp, out var e)) continue;

                var j = i;
                do
                {
                    s.Write(j, e);
                    j -= h;
                }
                // j - h >= first keeps the chain inside the subrange.
                while (j - h >= first && s.IsElementGreaterThan(j - h, tmp, out e));
                s.Write(j, tmp);
            }
        }
    }
}
