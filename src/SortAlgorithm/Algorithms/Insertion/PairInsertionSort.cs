using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// Pair Insertion Sort（ペア挿入ソート）は、隣り合う2要素を1組として挿入する挿入ソートです。
/// 組を1回の比較で並べ、大きい方を先に挿入して両方より大きい要素を2つずつ後ろへずらし、小さい方はその停止位置から続けて挿入します。
/// <br/>
/// Pair Insertion Sort is an insertion sort that inserts two adjacent elements per step.
/// One comparison orders the pair. The larger element is inserted first, moving every element greater than it two places up,
/// and the smaller element continues down from where the larger one stopped.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Pair Insertion Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Sorted Prefix:</strong> Before each step the elements to the left of the pair are sorted.</description></item>
/// <item><description><strong>Larger Element First:</strong> The pair is ordered with one comparison. The larger element scans the prefix
/// from the right, and each element greater than it moves two places up, which leaves two free slots below them.</description></item>
/// <item><description><strong>Smaller Element Continues:</strong> The smaller element scans on from the element that stopped the larger one,
/// moving elements one place up. Elements greater than both are compared and moved once per pair, where straight insertion would compare
/// and move them once for each element.</description></item>
/// <item><description><strong>Stability:</strong> On a tie the later element of the pair counts as the larger one, and both scans stop at an
/// equal element, so equal keys keep their order.</description></item>
/// <item><description><strong>Unpaired Element:</strong> The first element is a sorted prefix of one, so an even length leaves the last element
/// without a partner. It is inserted on its own.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Insertion</description></item>
/// <item><description>Stable      : Yes (the later element of a tied pair is inserted above the earlier one, and no scan passes an equal key)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space)</description></item>
/// <item><description>Best case   : O(n) - sorted input costs three comparisons per pair and moves nothing</description></item>
/// <item><description>Average case: O(n²) - about n²/6 comparisons and writes on random input, against n²/4 for straight insertion</description></item>
/// <item><description>Worst case  : O(n²) - reverse-sorted input: the larger element of each pair passes the whole prefix, about n²/4 comparisons and writes</description></item>
/// <item><description>Comparisons : O(n²) - one per pair, then one per element greater than the smaller element of the pair, plus the elements that stop each scan; about 3n/2 on sorted input, n²/6 on random input and n²/4 on reverse-sorted input</description></item>
/// <item><description>Swaps       : 0 - elements move up and the held pair drops into the slots left free; nothing is exchanged</description></item>
/// <item><description>Index Reads : O(n²) - one per comparison, plus one per pair and one for an unpaired last element; a moved element is written from the read its comparison made</description></item>
/// <item><description>Index Writes: O(n²) - one per element moved up and one per held element that does not stay in its slot; none on sorted input, about n²/4 on reverse-sorted input</description></item>
/// <item><description>Space       : O(1) - only the two held elements of the pair</description></item>
/// </list>
/// <para><strong>Comparer contract:</strong></para>
/// <para>Both scans of the public sort stop at the lower end of the range, so a comparer that is not a consistent total order cannot move
/// them outside it. The range keeps its elements, in an unspecified order, and nothing outside it is read or written.</para>
/// <para><strong>History:</strong></para>
/// <para>The earliest source found is JDK 7's <c>DualPivotQuicksort</c> (version 2011.02.11, by Vladimir Yaroslavskiy, Jon Bentley
/// and Josh Bloch). It sorts the non-leftmost parts with "the more optimized algorithm, so called pair insertion sort", where the element
/// before the part stops both scans. The name suggests the technique may be older, but no earlier source was found. It is not Ford and
/// Johnson's merge-insertion (1959).</para>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Insertion_sort</para>
/// </remarks>
public static class PairInsertionSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by PairInsertionSortTests, which derives from StableSortTestsBase.</remarks>
    public static bool IsStable => true;

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
    /// Sorts the subrange [first..last) using pair insertion sort with a custom comparer and sort context.
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
    /// Core pair insertion sort implementation that processes elements in pairs.
    /// This is the guarded version suitable for sorting from the beginning of an array.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use. Must implement <see cref="IComparer{T}"/>.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="first">The inclusive start index of the range to sort.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var length = last - first;
        if (length <= 1) return;

        // The first element is a sorted prefix of one; pairs start after it.
        var i = first + 1;
        while (i + 1 < last)
        {
            s.Context.OnPhase(SortPhase.PairInsertionPass, i, last - 1);

            // On a tie the later element is the larger one, so equal keys keep their order.
            var swapped = s.IsGreaterAt(i, i + 1, out var small, out var large);
            if (swapped)
            {
                (small, large) = (large, small);
            }

            // The larger element goes in first. Every element above it moves two places, which leaves room for both.
            // The prefix is never empty, so its top element needs no range check.
            var j = i - 1;
            if (s.IsElementGreaterThan(j, large, out var e))
            {
                do
                {
                    s.Write(j + 2, e);
                    j--;
                }
                while (j >= first && s.IsElementGreaterThan(j, large, out e));
                s.Write(j + 2, large);
            }
            else if (swapped)
            {
                s.Write(i + 1, large);
            }

            // The smaller element continues down from where the larger one stopped, moving elements one place.
            var k = j;
            if (k >= first && s.IsElementGreaterThan(k, small, out e))
            {
                do
                {
                    s.Write(k + 1, e);
                    k--;
                }
                while (k >= first && s.IsElementGreaterThan(k, small, out e));
                s.Write(k + 1, small);
            }
            else if (swapped || k != i - 1)
            {
                s.Write(k + 1, small);
            }

            i += 2;
        }

        // An even length leaves the last element without a partner.
        if (i < last)
        {
            var tmp = s.Read(i);
            var j = i - 1;
            while (j >= first && s.IsElementGreaterThan(j, tmp, out var e))
            {
                s.Write(j + 1, e);
                j--;
            }
            if (j != i - 1)
            {
                s.Write(j + 1, tmp);
            }
        }
    }

    /// <summary>
    /// Unguarded pair insertion sort: assumes that there is an element at position (first - 1)
    /// that is less than or equal to all elements in the range [first, last).
    /// That element stops both scans of every pair, so neither tests the lower end of the range.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use. Must implement <see cref="IComparer{T}"/>.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="first">The inclusive start index of the range to sort. Must be > 0.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    /// <remarks>
    /// PRECONDITION: first > 0 and s[first-1] <= s[i] for all i in [first, last)
    /// This is the form pair insertion sort was published in: a quicksort partition that is not the leftmost one has such an element.
    /// Comparisons and writes are the same as in <see cref="SortCore{T, TComparer, TContext}"/>.
    ///
    /// Only a consistent comparer stops these loops at their sentinel. Element access is unchecked, so a shift that would pass s[first-1]
    /// throws <see cref="InvalidOperationException"/> and leaves the range a permutation of its input. The natural order of a primitive type
    /// is consistent, and the checks fold away for it.
    /// </remarks>
    internal static void UnguardedSortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var length = last - first;
        if (length <= 1) return;

        var i = first + 1;
        while (i + 1 < last)
        {
            var swapped = s.IsGreaterAt(i, i + 1, out var small, out var large);
            if (swapped)
            {
                (small, large) = (large, small);
            }

            var j = i - 1;
            while (s.IsElementGreaterThan(j, large, out var e))
            {
                // Passing the sentinel would leave the two slots above it empty.
                if (!s.UsesNaturalPrimitiveOrder && j < first)
                {
                    s.Write(j + 1, small);
                    ThrowHelper.RestoreAndThrowInconsistentComparer(s, j + 2, large);
                }
                s.Write(j + 2, e);
                j--;
            }
            if (swapped || j != i - 1)
            {
                s.Write(j + 2, large);
            }

            var k = j;
            while (s.IsElementGreaterThan(k, small, out var e))
            {
                if (!s.UsesNaturalPrimitiveOrder && k < first) ThrowHelper.RestoreAndThrowInconsistentComparer(s, k + 1, small);
                s.Write(k + 1, e);
                k--;
            }
            if (swapped || k != i - 1)
            {
                s.Write(k + 1, small);
            }

            i += 2;
        }

        if (i < last)
        {
            var tmp = s.Read(i);
            var j = i - 1;
            while (s.IsElementGreaterThan(j, tmp, out var e))
            {
                if (!s.UsesNaturalPrimitiveOrder && j < first) ThrowHelper.RestoreAndThrowInconsistentComparer(s, j + 1, tmp);
                s.Write(j + 1, e);
                j--;
            }
            if (j != i - 1)
            {
                s.Write(j + 1, tmp);
            }
        }
    }
}
