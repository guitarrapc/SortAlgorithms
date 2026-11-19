using SortAlgorithm.Contexts;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 整列していない配列にあえて「二分探索」を行い、探索が着いた位置に従って要素を交換する挿入ソートの変種です。
/// フェーズ i ではセル i の値を配列全体から二分探索し、探索がセル i に戻ってくるまで、外れた探索が示す逆順の組を交換し続けます。
/// 配列が整列していないので探索は見当違いの位置に着きますが、外れた探索は必ず逆順の組を指すため、最後には整列します。
/// <br/>
/// A variant of insertion sort that deliberately runs "binary search" on an array that is not sorted and swaps according to where the search lands.
/// Phase i binary-searches the whole array for the content of cell i and, until a search comes back to cell i, swaps the out-of-order pair each failed search points at.
/// Because the array is unsorted the searches land in the wrong places, but every failed search identifies an inversion, so the array ends up sorted.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Fun-Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Unordered Binary Search:</strong> A search for x keeps indices l &lt; h with a[l] &lt; x ≤ a[h], starting from the virtual sentinels
/// a[-1] = -∞ and a[n] = +∞. Each step compares x with a[m], m = ⌊(l + h) / 2⌋, and sets h = m if x ≤ a[m], otherwise l = m, until h = l + 1; the result is h.
/// Every update is justified by the comparison just made, so the search ends with a[h-1] &lt; x ≤ a[h] whatever the contents; only the claim that h is x's
/// sorted position needs a sorted array. With equal keys these relations are taken in the tie order described under Implementation Notes.</description></item>
/// <item><description><strong>Phase i:</strong> Search for the current content x of cell i. If the search returns h = i, the phase ends.
/// If h &gt; i, swap cells i and h - 1: a[h-1] &lt; x, and h - 1 ≠ i because that would need x &lt; x. If h &lt; i, swap cells h and i: x &lt; a[h].
/// Then search again for the new content of cell i. Phases run for i = 0, 1, ..., n-1.</description></item>
/// <item><description><strong>Progress:</strong> Every failed search names a pair that is out of order, and swapping an out-of-order pair removes at least one inversion
/// (Biedl et al., Lemma 1). Each phase therefore ends, and there are at most F failed searches in total, where F is the number of inversions of the input.</description></item>
/// <item><description><strong>Invariant:</strong> When phase i ends, cells 0..i are in order among themselves, though they need not hold the i + 1 smallest elements.
/// A successful search gives a[i-1] ≤ a[i]. A swap with h - 1 &gt; i leaves cells 0..i-1 untouched, and a swap with h &lt; i writes x into cell h,
/// where a[h-1] ≤ x &lt; a[h] ≤ a[h+1], so the prefix stays in order. After phase n-1 the whole array is sorted.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Joke / Insertion</description></item>
/// <item><description>Stable      : No (a swap carries an element past equal keys)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space)</description></item>
/// <item><description>Best case   : Θ(n log n) - Already sorted, every phase ends with its first search; the searches still cost Θ(log n) each</description></item>
/// <item><description>Average case: O(n² log n) - At most n + F searches with F = n(n-1)/4 expected inversions; Biedl et al. leave the exact average open</description></item>
/// <item><description>Worst case  : Θ(n² log n) - O((n + F) log n) in general, and Biedl et al. give an input family that needs it</description></item>
/// <item><description>Comparisons : O(n² log n) - ⌊log₂(n+1)⌋ or ⌈log₂(n+1)⌉ per search, over n successful and at most F failed searches; Θ(n log n) when already sorted</description></item>
/// <item><description>Swaps       : O(n²) - One per failed search, at most F; Θ(n²) on the worst-case family; 0 when already sorted</description></item>
/// <item><description>Index Reads : O(n² log n) - One per comparison, one per search to load the key, and two per swap</description></item>
/// <item><description>Index Writes: O(n²) - Two per swap; 0 when already sorted</description></item>
/// <item><description>Space       : O(1) - The search uses two indices and the key</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description><strong>Equal keys:</strong> Biedl et al. assume distinct elements and state that the results hold in general without giving the tie rule.
/// Comparing values alone breaks one of the two steps above. If a search that lands on an equal copy below cell i ends the phase, [1, 5, 1] finishes unsorted.
/// If that search counts as a failure, the prescribed swap exchanges two equal values, the next search repeats it, and the phase never ends.
/// This implementation makes the elements distinct by their current position: when x equals a[m], the search takes x ≤ a[m] exactly when i ≤ m,
/// so equal copies to the left of cell i count as smaller and those at or to its right as larger. Under that order every failed search names a strict inversion
/// and the invariant above holds. The tie is settled by comparing indices, so it costs no element comparison or read. With distinct keys it never applies, and the algorithm is exactly the paper's.</description></item>
/// <item><description>The paper also allows ties to continue right instead of left. This implementation uses the paper's main rule (x ≤ a[m] goes left).</description></item>
/// <item><description>Each phase announces <see cref="SortPhase.FunSortPhase"/> with param1 = i and param2 = n - 1, and cell i carries <see cref="RoleType.Inserting"/> until the phase ends.
/// The key is read once per search and compared as a value, so each probe of the search shows up as a comparison against cell m.</description></item>
/// </list>
/// <para><strong>Comparer contract:</strong></para>
/// <para>Each swap exchanges a strictly inverted pair, so under a consistent comparer the sort makes at most n(n - 1) / 2 swaps. A comparer
/// that drives it past that makes it throw <see cref="InvalidOperationException"/> instead of searching forever. Either way the range keeps
/// its elements, and nothing outside it is read or written.</para>
/// <para><strong>History:</strong></para>
/// <para>Biedl, Chan, Demaine, Fleischer, Golin and Munro presented Fun-Sort at FUN with Algorithms 2001, named after the conference series. The 2004 journal version,
/// with King, adds the observation that for n = 2^k - 1 a binary search for the element of rank i in a random permutation ends, on average, exactly at cell i,
/// which is why the chaotic searches tend to move elements toward their final positions.</para>
/// <para><strong>Reference:</strong></para>
/// <para>Biedl, T.; Chan, T.; Demaine, E. D.; Fleischer, R.; Golin, M.; King, J. A.; Munro, J. I. (2004). "Fun-Sort—or the chaos of unordered binary search".
/// Discrete Applied Mathematics 144 (3): 231-236. doi:10.1016/j.dam.2004.01.003</para>
/// <para>Biedl, T.; Chan, T.; Demaine, E. D.; Fleischer, R.; Golin, M.; Munro, J. I. (2001). "Fun-Sort". FUN with Algorithms 2, Proceedings in Informatics 10: 15-26.</para>
/// </remarks>
public static class FunSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by FunSortTests, which derives from SortTestsBase.</remarks>
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
        => Sort(span, new ComparableComparer<T>(), NullContext.Default);

    /// <summary>
    /// Sorts the elements in the specified span using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
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
        var last = span.Length - 1;

        // Every swap exchanges a strictly inverted pair, so a consistent comparer allows at most n(n - 1) / 2 of them. More can only come
        // from a comparer that is not consistent, which would keep the searches going forever. The natural order of a primitive type is
        // consistent, so the count folds away for it.
        var checkProgress = !s.UsesNaturalPrimitiveOrder;
        var swapBudget = (long)span.Length * (span.Length - 1) / 2;

        for (var i = 0; i <= last; i++)
        {
            context.OnPhase(SortPhase.FunSortPhase, i, last);
            context.OnRole(i, BUFFER_MAIN, RoleType.Inserting);

            while (true)
            {
                var h = Search(s, i);
                if (h == i) break;
                if (checkProgress && --swapBudget < 0) ThrowHelper.ThrowInconsistentComparer();

                if (h > i)
                {
                    // a[h-1] < x: move x right to h-1 and bring the smaller element into cell i.
                    s.Swap(i, h - 1);
                }
                else
                {
                    // x < a[h]: move x left to h and bring the larger element into cell i.
                    s.Swap(h, i);
                }
            }

            context.OnRole(i, BUFFER_MAIN, RoleType.None);
        }
    }

    /// <summary>
    /// Binary search for the content of cell <paramref name="i"/> over the whole span, sorted or not.
    /// Returns h with a[h-1] &lt; x ≤ a[h], using the virtual sentinels a[-1] = -∞ and a[n] = +∞;
    /// equal values are ordered by position, so x ≤ a[m] holds for an equal a[m] exactly when i ≤ m.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Search<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int i)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var x = s.Read(i);
        var l = -1;
        var h = s.Length;
        while (h - l > 1)
        {
            // ⌊(l + h) / 2⌋ without overflow; h - l ≥ 2 keeps m within [0, n-1].
            var m = l + ((h - l) >> 1);
            var c = s.Compare(x, m);
            if (c < 0 || (c == 0 && i <= m))
                h = m;
            else
                l = m;
        }
        return h;
    }
}
