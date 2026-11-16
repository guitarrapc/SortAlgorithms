using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 選択ソートの変種です。未ソート領域の最大値を見つけるたびに1要素だけを動かすのではなく、その値と等しい要素をすべて1回の走査で末尾へ集め、同じ走査の中で次の最大値を見つけます。
/// 1回の走査が要素1つではなく値1つを片付けるため、走査回数は要素数ではなく異なる値の数に従い、重複の多い入力では選択ソートよりはるかに少ない走査で済みます。
/// <br/>
/// A selection sort variant that, instead of moving one element per pass, moves every copy of the current maximum to the end of the unsorted region
/// in a single scan and finds the next maximum during that same scan. Each pass retires one key value rather than one element,
/// so the number of passes follows the number of distinct values, and duplicate-heavy input needs far fewer passes than selection sort.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Bingo Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Partition Invariant:</strong> Maintain an unsorted prefix [first..end] and a sorted suffix (end..last).
/// Every suffix element is greater than or equal to every prefix element, so the suffix is already in its final position.</description></item>
/// <item><description><strong>First Pass:</strong> Scan the whole range once to find its maximum, moving nothing.
/// This value becomes the "previous maximum" that the next pass collects.</description></item>
/// <item><description><strong>Collecting Pass:</strong> Each later pass starts its next-maximum candidate at A[end] and scans [first..end) downward.
/// An element that is not greater than the candidate is skipped after one comparison. An element that is greater is either a copy of the previous maximum
/// ("Bingo!"), which is swapped into position end before end shrinks by one, or a new candidate. Because the previous maximum is strictly greater than
/// every other element left in the region, only elements that beat the candidate need the second comparison.</description></item>
/// <item><description><strong>Displaced Element:</strong> A swap carries the element at end back to the scan position. That element was already examined —
/// it was the starting candidate or was scanned earlier in the same pass — so the downward scan never needs to revisit it.</description></item>
/// <item><description><strong>Trailing Retirement:</strong> After each pass, copies of the newly found maximum that already sit at the end of the region are in their
/// final positions; shrink end over them. This also guarantees that A[end] differs from the previous maximum when the next pass starts,
/// so the starting candidate is a legitimate candidate.</description></item>
/// <item><description><strong>Termination:</strong> Stop when end reaches first: a single remaining element is in place.</description></item>
/// <item><description><strong>Comparison Consistency:</strong> All element comparisons must use a total order relation (transitive, antisymmetric, total).
/// The equality tests use the comparer's own notion of equal (a comparison result of zero).</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Selection</description></item>
/// <item><description>Stable      : No (a swap that sends a copy of the previous maximum to the end carries the end element back to the scan position, past elements equal to it)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space)</description></item>
/// <item><description>Best case   : Θ(n + m²) - m is the number of distinct keys; reached when the largest keys hold most of the copies, so the region shrinks to about m elements after the first passes; Θ(n) when every key is equal</description></item>
/// <item><description>Average case: Θ(n·m) - one pass over the unsorted region per distinct key</description></item>
/// <item><description>Worst case  : Θ(n·m) - Θ(n²) when every key is distinct (m = n), where it makes as many passes as selection sort</description></item>
/// <item><description>Comparisons : O(n·m) - one per scanned element against the next-maximum candidate, a second against the previous maximum only for elements that beat the candidate, and one per trailing equality check</description></item>
/// <item><description>Swaps       : O(n) - at most n-1; each swap puts a copy of the previous maximum into its final position, and copies already at the end are retired without one</description></item>
/// <item><description>Index Reads : O(n·m) - each pass reads every element of the unsorted region once, plus the starting candidate and the trailing checks; each swap adds two</description></item>
/// <item><description>Index Writes: O(n) - two per swap; nothing else writes</description></item>
/// <item><description>Space       : O(1) - the previous and next maximum are held in two variables</description></item>
/// <item><description>Passes      : ≤ m - one per distinct key; a key whose copies the trailing check retires does not need a pass of its own</description></item>
/// </list>
/// <para><strong>Use Cases:</strong></para>
/// <list type="bullet">
/// <item><description>Inputs with few distinct keys, where one pass per value replaces one pass per element</description></item>
/// <item><description>Write-constrained settings that can afford Θ(n·m) comparisons: like selection sort, it performs at most n-1 swaps</description></item>
/// <item><description>Educational contrast with selection sort: the same scan, with the unit of work changed from an element to a value</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <para>This follows the Wikipedia pseudocode, which collects maxima at the end of the range; the NIST definition states the mirror image that
/// collects minima at the front. The candidate is held by value, as in the pseudocode, so the scan reads one element per comparison.
/// The <c>CurrentMax</c> role marks the element that holds the candidate value and follows it when a swap moves it.</para>
/// <para><strong>History:</strong></para>
/// <para>Paul E. Black's entry in NIST's Dictionary of Algorithms and Data Structures gives the definition used here, names no inventor,
/// and points to Berman and Paul (1996), §4.6, for more information. The name refers to the exclamation "Bingo!" when an item with the right value is found.</para>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Selection_sort#Variants</para>
/// <para>NIST: Black, P. E. "bingo sort". Dictionary of Algorithms and Data Structures. https://xlinux.nist.gov/dads/HTML/bingosort.html</para>
/// <para>Berman, K. A.; Paul, J. L. (1996). Fundamentals of Sequential and Parallel Algorithms, §4.6, pp. 137-139. PWS Publishing.</para>
/// </remarks>
public static class BingoSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by BingoSortTests, which derives from SortTestsBase and pins a reordering of equal keys.</remarks>
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
        // Inclusive end of the unsorted region [first..end]; (end..last) is in its final position.
        var end = last - 1;
        if (end <= first) return;

        // Phases and roles are reported through s.Offset and s.BufferId, the coordinates that Read,
        // Compare and Swap already use, so they stay correct if a caller hands this core a slice.
        // First pass: find the maximum without moving anything.
        s.Context.OnPhase(SortPhase.BingoFindMax, s.Offset + first, s.Offset + end);
        var nextMax = s.Read(end);
        var maxIndex = end; // Where the candidate value sits; only the CurrentMax role uses it.
        s.Context.OnRole(s.Offset + maxIndex, s.BufferId, RoleType.CurrentMax);
        for (var i = end - 1; i >= first; i--)
        {
            if (s.IsElementGreaterThan(i, nextMax, out var element))
            {
                nextMax = element;
                s.Context.OnRole(s.Offset + maxIndex, s.BufferId, RoleType.None);
                maxIndex = i;
                s.Context.OnRole(s.Offset + maxIndex, s.BufferId, RoleType.CurrentMax);
            }
        }
        s.Context.OnRole(s.Offset + maxIndex, s.BufferId, RoleType.None);

        // Copies of the maximum that already end the region are in place.
        while (end > first && s.Compare(end, nextMax) == 0)
        {
            end--;
        }

        while (end > first)
        {
            // Collect every copy of the previous maximum at the end while finding the next maximum.
            s.Context.OnPhase(SortPhase.BingoPass, s.Offset + first, s.Offset + end);
            var prevMax = nextMax;
            nextMax = s.Read(end);
            maxIndex = end;
            s.Context.OnRole(s.Offset + maxIndex, s.BufferId, RoleType.CurrentMax);

            for (var i = end - 1; i >= first; i--)
            {
                // Everything left in the region is <= prevMax, so only an element that beats the
                // candidate can be a copy of prevMax; the rest are dismissed after one comparison.
                if (s.IsElementGreaterThan(i, nextMax, out var element))
                {
                    if (s.Compare(element, prevMax) != 0)
                    {
                        nextMax = element;
                        s.Context.OnRole(s.Offset + maxIndex, s.BufferId, RoleType.None);
                        maxIndex = i;
                        s.Context.OnRole(s.Offset + maxIndex, s.BufferId, RoleType.CurrentMax);
                    }
                    else
                    {
                        // Bingo: a copy of prevMax belongs at end. The element it displaces was the starting
                        // candidate or was scanned earlier in this pass, so nothing is skipped by moving it below i.
                        var candidateMoves = maxIndex == end;
                        if (candidateMoves) s.Context.OnRole(s.Offset + maxIndex, s.BufferId, RoleType.None);
                        s.Swap(i, end);
                        end--;
                        if (candidateMoves)
                        {
                            maxIndex = i;
                            s.Context.OnRole(s.Offset + maxIndex, s.BufferId, RoleType.CurrentMax);
                        }
                    }
                }
            }
            s.Context.OnRole(s.Offset + maxIndex, s.BufferId, RoleType.None);

            // Copies of the new maximum that already end the region are in place. This also leaves
            // A[end] != nextMax, which the next pass relies on when it starts its candidate there.
            while (end > first && s.Compare(end, nextMax) == 0)
            {
                end--;
            }
        }
    }
}
