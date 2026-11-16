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
/// <para>Each pass applies swap-based gapped insertion sort. Gaps are used in decreasing order,
/// ending at 1. Equal keys may move past one another, so this algorithm is unstable.</para>
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
/// <item><description>Swaps       : O(n^{3/2}) worst-case upper bound</description></item>
/// <item><description>Index Reads : O(n^{3/2}) worst-case upper bound - comparisons and swaps</description></item>
/// <item><description>Index Writes: O(n^{3/2}) worst-case upper bound - two per swap</description></item>
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
    /// <remarks>
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var length = last - first;
        if (length < 2) return;

        var knuthSequence = ShellGapSequences.Knuth;
        var gapIndex = knuthSequence.Length - 1;
        ShellGapSequences.FindLast(knuthSequence, length, ref gapIndex);

        // Count usable gaps for display
        int totalGaps = gapIndex + 1;

        // Decrease gap by moving to previous index
        for (; gapIndex >= 0; gapIndex--)
        {
            var h = knuthSequence[gapIndex];
            s.Context.OnPhase(SortPhase.ShellGapPass, h, totalGaps - gapIndex, totalGaps);

            // Swap based Insertion sort with gap h.
            for (var i = first + h; i < last; i++)
            {
                // Ensure j >= first + h to stay within the subrange.
                for (int j = i; j >= first + h && s.IsGreaterAt(j - h, j); j -= h)
                {
                    s.Swap(j, j - h);
                }
            }
        }
    }
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
/// <para>Each pass applies swap-based gapped insertion sort. Gaps are used in decreasing order,
/// ending at 1. Equal keys may move past one another, so this algorithm is unstable.</para>
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
/// <item><description>Swaps       : O(n²) worst-case upper bound</description></item>
/// <item><description>Index Reads : O(n²) worst-case upper bound - comparisons and swaps</description></item>
/// <item><description>Index Writes: O(n²) worst-case upper bound - two per swap</description></item>
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
    /// <remarks>
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var length = last - first;
        if (length < 2) return;

        var sedgewickSequence = ShellGapSequences.Sedgewick;
        var gapIndex = sedgewickSequence.Length - 1;
        ShellGapSequences.FindLast(sedgewickSequence, length, ref gapIndex);

        int totalGaps = gapIndex + 1;

        // Decrease gap by moving to previous index
        for (; gapIndex >= 0; gapIndex--)
        {
            var h = sedgewickSequence[gapIndex];
            s.Context.OnPhase(SortPhase.ShellGapPass, h, totalGaps - gapIndex, totalGaps);

            // Swap based Insertion sort with gap h.
            for (var i = first + h; i < last; i++)
            {
                // Ensure j >= first + h to stay within the subrange.
                for (var j = i; j >= first + h && s.IsGreaterAt(j - h, j); j -= h)
                {
                    s.Swap(j, j - h);
                }
            }
        }
    }
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
/// <para>Each pass applies swap-based gapped insertion sort. Gaps are used in decreasing order,
/// ending at 1. Equal keys may move past one another, so this algorithm is unstable.</para>
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
/// <item><description>Swaps       : O(n²) worst-case upper bound</description></item>
/// <item><description>Index Reads : O(n²) worst-case upper bound - comparisons and swaps</description></item>
/// <item><description>Index Writes: O(n²) worst-case upper bound - two per swap</description></item>
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
    /// <remarks>
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var length = last - first;
        if (length < 2) return;

        var tokudaSequence = ShellGapSequences.Tokuda;
        var gapIndex = tokudaSequence.Length - 1;
        ShellGapSequences.FindLast(tokudaSequence, length, ref gapIndex);

        int totalGaps = gapIndex + 1;

        // Decrease gap by moving to previous index
        for (; gapIndex >= 0; gapIndex--)
        {
            var h = tokudaSequence[gapIndex];
            s.Context.OnPhase(SortPhase.ShellGapPass, h, totalGaps - gapIndex, totalGaps);

            // Swap based Insertion sort with gap h.
            for (int i = first + h; i < last; i++)
            {
                // Ensure j >= first + h to stay within the subrange.
                for (int j = i; j >= first + h && s.IsGreaterAt(j - h, j); j -= h)
                {
                    s.Swap(j, j - h);
                }
            }
        }
    }
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
/// <para>Each pass applies swap-based gapped insertion sort. Gaps are used in decreasing order,
/// ending at 1. Equal keys may move past one another, so this algorithm is unstable.</para>
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
/// <item><description>Swaps       : O(n²) worst-case upper bound</description></item>
/// <item><description>Index Reads : O(n²) worst-case upper bound - comparisons and swaps</description></item>
/// <item><description>Index Writes: O(n²) worst-case upper bound - two per swap</description></item>
/// <item><description>Space       : O(1) per call - immutable gaps and scalar indices</description></item>
/// </list>
/// <para>Average-case exponents and relative performance are not asserted as general guarantees.</para>
/// <para>Reference: Ciura, Best Increments for the Average Case of Shellsort (2001).</para>
/// </remarks>
/// <typeparam name="T"></typeparam>
/// </remarks>
/// <typeparam name="T"></typeparam>
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
    /// <remarks>
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var length = last - first;
        if (length < 2) return;

        var ciuraSequence = ShellGapSequences.Ciura;
        var gapIndex = ciuraSequence.Length - 1;
        ShellGapSequences.FindLast(ciuraSequence, length, ref gapIndex);

        int totalGaps = gapIndex + 1;

        // Decrease gap by moving to previous index
        for (; gapIndex >= 0; gapIndex--)
        {
            var h = ciuraSequence[gapIndex];
            s.Context.OnPhase(SortPhase.ShellGapPass, h, totalGaps - gapIndex, totalGaps);

            // Swap based Insertion sort with gap h.
            for (var i = first + h; i < last; i++)
            {
                // Ensure j >= first + h to stay within the subrange.
                for (var j = i; j >= first + h && s.IsGreaterAt(j - h, j); j -= h)
                {
                    s.Swap(j, j - h);
                }
            }
        }
    }
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
/// <para>Each pass applies swap-based gapped insertion sort. Gaps are used in decreasing order,
/// ending at 1. Equal keys may move past one another, so this algorithm is unstable.</para>
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
/// <item><description>Swaps       : O(n²) worst-case upper bound</description></item>
/// <item><description>Index Reads : O(n²) worst-case upper bound - comparisons and swaps</description></item>
/// <item><description>Index Writes: O(n²) worst-case upper bound - two per swap</description></item>
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
    {
        var length = last - first;
        if (length < 2) return;

        // Lee's empirically determined sequence based on gamma = 2.243609061420001
        var leeSequence = ShellGapSequences.Lee;
        var gapIndex = leeSequence.Length - 1;
        ShellGapSequences.FindLast(leeSequence, length, ref gapIndex);

        int totalGaps = gapIndex + 1;

        // Decrease gap by moving to previous index
        for (; gapIndex >= 0; gapIndex--)
        {
            var h = leeSequence[gapIndex];
            s.Context.OnPhase(SortPhase.ShellGapPass, h, totalGaps - gapIndex, totalGaps);

            // Swap based Insertion sort with gap h.
            for (var i = first + h; i < last; i++)
            {
                // Ensure j >= first + h to stay within the subrange.
                for (var j = i; j >= first + h && s.IsGreaterAt(j - h, j); j -= h)
                {
                    s.Swap(j, j - h);
                }
            }
        }
    }
}
