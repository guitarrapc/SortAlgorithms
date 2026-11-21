using SortAlgorithm.Contexts;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 任意長のバイトニックソート（Lang, 1998）です。範囲を ⌊n/2⌋ と残りに分け、前半を逆向き、後半を目的の向きに再帰的に整列してから、
/// n 未満で最大の2のべき乗 m を距離として i と i + m（i &lt; n - m）を比べ、[0, m) と [m, n) を再帰的にマージします。
/// 2のべき乗の長さ p のネットワークから比較器を抜いたもので、足りない位置を最大値とみなせるので、長さに制約がありません。
/// <br/>
/// Bitonic sort for any input length (Lang, 1998). A range is split into its first ⌊n/2⌋ elements and the rest; the first part is sorted
/// in the opposite direction and the second in the target direction, recursively, and the result is merged: with m the largest power of two
/// below n, position i is compared with i + m for every i &lt; n - m, and then [0, m) and [m, n) are merged recursively. The network is the
/// power-of-two network with the comparators that would touch missing positions left out, so it sorts every length.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Bitonic Sort of Any Length:</strong></para>
/// <list type="number">
/// <item><description><strong>Bitonic Decreasing Input to the Merge:</strong> Sorting the first part descending and the second ascending makes
/// the range, read as a 0-1 sequence, a run of 1s, then 0s, then 1s. Filling it up with 1s to the next power of two p keeps that shape, and the
/// added 1s sit at the end. (For a descending merge the parts are sorted the other way round and the filling is with 0s.)</description></item>
/// <item><description><strong>Merge for Any Length:</strong> On the filled sequence, the power-of-two half-cleaner at distance m = p / 2 leaves
/// two bitonic halves with every element of the lower half not greater than every element of the upper half. Its comparators whose upper
/// position lies at or past n would compare with an added 1 and change nothing, so only the n - m comparators (i, i + m) with i &lt; n - m are
/// kept. The lower half [0, m) is bitonic of power-of-two length and the upper half [m, n) is again bitonic decreasing, so both are merged the
/// same way.</description></item>
/// <item><description><strong>Recursion:</strong> A range of one element is sorted. Since n - ⌊n/2⌋ ≥ ⌊n/2⌋, every part is smaller than its
/// range, and the network is the power-of-two network for the next power of two with comparators removed, so it has
/// ⌈log₂ n⌉ (⌈log₂ n⌉ + 1) / 2 stages of at most n / 2 comparators each.</description></item>
/// <item><description><strong>Comparison Network Property:</strong> Which positions are compared depends only on n, never on the keys, so by
/// the zero-one principle a network that sorts every 0-1 input of length n sorts every input of length n.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Network Sort / Exchange</description></item>
/// <item><description>Stable      : No (comparators exchange non-adjacent elements)</description></item>
/// <item><description>In-place    : Yes (the comparators act on the input; O(log n) recursion stack)</description></item>
/// <item><description>Best case   : Θ(n log² n) - the network runs the same comparators on every input</description></item>
/// <item><description>Average case: Θ(n log² n) - the same comparators as the best case</description></item>
/// <item><description>Worst case  : Θ(n log² n) - the same comparators as the best case</description></item>
/// <item><description>Comparisons : Θ(n log² n) - one per comparator, the same count for every input of length n; n log₂ n (log₂ n + 1) / 4 when n is a power of two</description></item>
/// <item><description>Swaps       : O(n log² n) - one for each comparator whose pair is out of order; sorted input is not free, because every first part is sorted descending</description></item>
/// <item><description>Index Reads : Θ(n log² n) - every comparator reads its two positions, and each swap reads them again</description></item>
/// <item><description>Index Writes: O(n log² n) - two per swap; nothing else writes</description></item>
/// <item><description>Space       : O(log n) - the recursion stack</description></item>
/// <item><description>Depth       : ⌈log₂ n⌉ (⌈log₂ n⌉ + 1) / 2 stages, the depth of the power-of-two network it is cut from</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>The comparators run in the order of the program on Lang's page: the first part, the second part, then each merge's
/// comparators in index order followed by its two halves. For a power-of-two length the comparator pairs are those of
/// <see cref="BitonicSort"/>, but each first half is sorted the opposite way, so the directions are mirrored.</description></item>
/// <item><description>The program on Lang's page exchanges a pair of equal keys in a descending comparator; this class exchanges a pair only
/// when it is out of order, which leaves the same values in every position.</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>H. W. Lang, "Bitonic sorting network for n not a power of 2", 1998: https://hwlang.de/algorithmen/sortieren/bitonic/oddn.htm</para>
/// <para>K. E. Batcher, "Sorting Networks and their Applications", Proc. AFIPS Spring Joint Computer Conference 32, 1968, pp. 307-314</para>
/// </remarks>
public static class BitonicSortAnyLength
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by BitonicSortAnyLengthTests, which derives from SortTestsBase.</remarks>
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
    /// <typeparam name="TContext">The type of the sort context.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T, TContext>(Span<T> span, TContext context)
        where T : IComparable<T>
        where TContext : ISortContext
        => Sort(span, new ComparableComparer<T>(), context);

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
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        SortCore(s, 0, span.Length, ascending: true);
    }

    /// <summary>
    /// Sorts the <paramref name="count"/> elements from <paramref name="low"/> in the given direction: the first ⌊count/2⌋ in the opposite
    /// direction, the rest in this one, and then the merge.
    /// </summary>
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int low, int count, bool ascending)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (count <= 1) return;

        // A part of one element is already sorted; testing that here saves a call for it.
        var half = count / 2;
        if (half > 1) SortCore(s, low, half, !ascending);
        SortCore(s, low + half, count - half, ascending);
        Merge(s, low, count, ascending);
    }

    /// <summary>
    /// Merges the <paramref name="count"/> elements from <paramref name="low"/>, which form a bitonic sequence of the shape the sort leaves,
    /// in the given direction.
    /// </summary>
    static void Merge<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int low, int count, bool ascending)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (count <= 1) return;

        var m = GreatestPowerOfTwoLessThan(count);
        if (ascending)
        {
            for (var i = low; i < low + count - m; i++)
            {
                if (s.IsGreaterAt(i, i + m)) s.Swap(i, i + m);
            }
        }
        else
        {
            for (var i = low; i < low + count - m; i++)
            {
                if (s.IsLessAt(i, i + m)) s.Swap(i, i + m);
            }
        }

        if (m > 1) Merge(s, low, m, ascending);
        if (count - m > 1) Merge(s, low + m, count - m, ascending);
    }

    /// <summary>The largest power of two less than <paramref name="n"/>, which is at least 2.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GreatestPowerOfTwoLessThan(int n) => 1 << System.Numerics.BitOperations.Log2((uint)(n - 1));
}
