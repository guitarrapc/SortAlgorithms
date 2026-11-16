using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// バッチャー奇偶マージソート - 奇偶マージネットワークを使って2つの整列済み列をマージするソーティングネットワークアルゴリズムです。
/// 任意のサイズの入力に対応します。
/// <br/>
/// Batcher Odd-Even Merge Sort - A sorting network algorithm that uses odd-even merge networks to merge two sorted sequences.
/// Supports arbitrary input sizes.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Batcher Odd-Even Merge Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Odd-Even Merge Definition:</strong> Given two sorted sequences A and B,
/// the odd-even merge recursively merges the odd-indexed elements of A and B, then merges the even-indexed elements,
/// then performs a sequence of compare-and-swap operations on adjacent interleaved pairs.
/// This produces a sorted merged sequence.</description></item>
/// <item><description><strong>Arbitrary Size Support:</strong> Unlike Bitonic Sort (which requires power-of-2 sizes),
/// this implementation accepts any n. It runs Batcher's network for k = the smallest power of two ≥ n and drops every
/// comparator whose upper position is ≥ n. Every comparator puts its minimum on the lower position, so +∞ padding at
/// positions ≥ n would never move: the comparators that touch it never exchange, and the rest sort the first n positions.
/// In the iterative formulation this is the bounds check together with the group check <c>(j+i)/(p+p) == (j+i+k)/(p+p)</c>,
/// which keeps both ends of every comparator in the same merge block.</description></item>
/// <item><description><strong>Iterative Network Construction:</strong> The algorithm uses the iterative formulation given in the
/// Wikipedia article. The outer loop <c>p</c> controls the merge group size (doubling each time).
/// For each merge size, the inner loops <c>k</c> and <c>j</c> generate all compare-and-swap pairs
/// required by the odd-even merge network at that level. For n = 2^k this is exactly the network of Batcher's recursive
/// construction: the same comparators in the same layers.</description></item>
/// <item><description><strong>Comparison Network Property:</strong> Like Bitonic Sort, Batcher Odd-Even Merge Sort
/// is a comparison network (data-oblivious). The same comparisons are performed regardless of input values,
/// making it suitable for parallel implementations and providing worst-case guarantees.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Network Sort / Merge</description></item>
/// <item><description>Stable      : No (non-adjacent swaps can change relative order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space)</description></item>
/// <item><description>Best case   : Θ(n log² n) - Data-independent comparison count (same as worst case)</description></item>
/// <item><description>Average case: Θ(n log² n) - Performs the same comparisons for all inputs</description></item>
/// <item><description>Worst case  : Θ(n log² n) - For n = 2^k: n × log n × (log n − 1) / 4 + n − 1 comparisons</description></item>
/// <item><description>Comparisons : Θ(n log² n) - For n = 2^k: n × log n × (log n − 1) / 4 + n − 1 (24,063 for n = 1024, the figure in Batcher's paper); the network runs the same comparators whatever the input</description></item>
/// <item><description>Swaps       : O(n log² n) - At most one per comparator, so at most the comparison count; how many actually fire depends on the input</description></item>
/// <item><description>Index Reads : Θ(n log² n) - every comparator reads the two positions it tests</description></item>
/// <item><description>Index Writes: O(n log² n) - each swap writes two positions, so the writes follow the swap count</description></item>
/// <item><description>Space       : O(1) - the comparators act directly on the input span</description></item>
/// <item><description>Depth       : O(log² n) - For n = 2^k: log n × (log n + 1) / 2 layers, so O(log² n) parallel time</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>Supports arbitrary input sizes (not restricted to power of 2).</description></item>
/// <item><description>True in-place sorting with zero heap allocation.</description></item>
/// <item><description>Uses the iterative formulation from the Wikipedia article (four nested loops).</description></item>
/// <item><description>The group check <c>(j+i)/(p+p) == (j+i+k)/(p+p)</c> has the same answer for every i of a group [j, j+2k):
/// the group straddles a merge-block boundary exactly when j+k is a multiple of 2p. The implementation therefore tests
/// <c>(j+k) &amp; (2p-1)</c> once per group and bounds i by the array end, so the inner loop runs only the comparators the
/// formulation accepts, in the same order, without dividing.</description></item>
/// <item><description>Generates fewer total comparisons than Bitonic Sort for the same input size.</description></item>
/// <item><description>For n that is not a power of two, the truncated network is larger than the one Batcher's own "s by t" merging rule
/// gives when applied to n directly. Under that rule a list with no partner passes through unchanged. The truncated network still runs
/// every merge, or recursive sub-merge, whose second list lies entirely at positions ≥ n, restricted to positions below n. Those
/// comparators compare positions of a list that is already sorted, so they never exchange. They are part of the formulation and are kept:
/// n = 12 runs (9, 10) inside the merge of [8, 12) with the empty [12, 16), and n = 300 runs 5,417 comparators where the s-by-t
/// construction uses 5,205.</description></item>
/// </list>
/// <para><strong>History:</strong></para>
/// <para>Odd-even merging is not derived from bitonic sorting; the two are siblings. Batcher (1968) presents odd-even merging first
/// and then introduces bitonic sorters as "another way of constructing merging networks". He builds a full sorter from either one
/// "using the well-known sorting-by-merging scheme". The 1968 paper cites his earlier Goodyear Aerospace report GER-11759
/// "A new internal sorting method" (1964) for odd-even merging; that report was not found online.</para>
/// <para><strong>Reference:</strong></para>
/// <para>Batcher, K. E. (1968). "Sorting networks and their applications". AFIPS Spring Joint Computer Conference. https://www.cs.kent.edu/~batcher/sort.pdf</para>
/// <para>Knuth, D. E. The Art of Computer Programming, Vol. 3: Sorting and Searching (2nd ed.), Section 5.3.4: Networks for Sorting.</para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Batcher_odd%E2%80%93even_mergesort (source of the iterative formulation)</para>
/// </remarks>
public static class BatcherOddEvenMergeSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>See the stability note in this type's summary.</remarks>
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
    /// <typeparam name="TComparer">The type of comparer to use for element comparisons.</typeparam>
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
        SortCoreIterative(s, span.Length);
    }

    /// <summary>
    /// Iterative Batcher odd-even merge sorting network.
    /// Repeatedly merges adjacent sorted blocks using odd-even compare-exchange stages.
    /// The comparison pattern is data-oblivious and determined solely by the network structure.
    /// </summary>
    /// <param name="s">The span to sort.</param>
    /// <param name="count">The length of the sequence.</param>
    private static void SortCoreIterative<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int count)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Iterative Batcher odd-even merge sorting network, in the iterative formulation of the Wikipedia article.
        // Repeatedly merges adjacent sorted blocks using data-oblivious compare-exchange stages.
        // The next pass and group offset can exceed int.MaxValue before the loop exits.
        var n = (uint)count;
        for (uint p = 1; p < n; p = NextPass(p))
        {
            s.Context.OnPhase(SortPhase.OddEvenMergeSortPass, (int)p, count);

            // Merge blocks have size 2p, a power of two, so a position's block offset is a mask away.
            var blockMask = (p << 1) - 1;

            // Inner loop: k is the comparison distance within the odd-even merge network.
            // Starts at p (full merge distance) and halves down to 1.
            for (var k = p; k >= 1; k >>= 1)
            {
                s.Context.OnPhase(SortPhase.OddEvenMergeSortStage, (int)k, (int)p, count);
                var rightLimit = n - k; // a left position at or past this has its partner outside the array

                // j iterates over the starting positions of each comparison group [j, j + 2k):
                // left positions j .. j+k-1, each compared with the position k above it.
                // k % p gives the correct starting offset for each stage.
                for (var j = k % p; j < rightLimit; j = NextGroup(j, k))
                {
                    // Group check: both ends of a comparator must lie in the same merge block of size 2p.
                    // It is the same answer for every pair of the group. 2k divides 2p, and j + k is the only
                    // multiple of 2k in [j, j + 2k) (j is an odd multiple of k when k < p, a multiple of 2p when
                    // k == p), so the group straddles a block boundary exactly when j + k is a multiple of 2p, and
                    // then every one of its pairs does. Testing once per group replaces two divisions per pair.
                    if (((j + k) & blockMask) == 0) continue;

                    // Bounds check: the group's last pairs may reach past the end of an incomplete final block.
                    var end = j + Math.Min(k, rightLimit - j);
                    for (var left = j; left < end; left++)
                    {
                        CompareAndSwap(s, (int)left, (int)(left + k));
                    }
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CompareAndSwap<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int i, int j)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (s.IsGreaterAt(i, j))
        {
            s.Swap(i, j);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint NextPass(uint pass) => pass << 1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint NextGroup(uint start, uint distance) => start + (distance << 1);
}
