using System.Numerics;
using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// ペアワイズソーティングネットワーク - 隣り合う2要素の組を整列し、奇数番目と偶数番目の部分列を再帰的に整列して組を辞書式順序に並べ、最後に整列済みの組の列を整列するソーティングネットワークです。
/// 任意のサイズの入力に対応します。
/// <br/>
/// Pairwise Sorting Network - A sorting network that sorts adjacent pairs, puts the pairs into lexicographic order by recursively sorting
/// the even-indexed and odd-indexed subsequences, and finally sorts the resulting sequence of sorted pairs.
/// Supports arbitrary input sizes.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Pairwise Sorting Network:</strong></para>
/// <list type="number">
/// <item><description><strong>Zero-One Principle:</strong> A comparator network sorts every input if and only if it sorts every input of zeros and ones.
/// Parberry designs each step against 0-1 inputs, so the steps below are stated in terms of bits.</description></item>
/// <item><description><strong>Step 1 - Sort pairs internally:</strong> For n a power of two, compare x[2i] with x[2i+1] (0-based) and put the smaller one first.
/// Every pair is then 00, 01 or 11.</description></item>
/// <item><description><strong>Step 2 - Sort the pairs into lexicographic order:</strong> Recursively sort the even-indexed subsequence y = (x[0], x[2], ...)
/// and the odd-indexed subsequence z = (x[1], x[3], ...). Step 1 left every pair's first element ≤ its second, so z holds at least as many ones as y,
/// and after both are sorted the string (y₀, z₀, y₁, z₁, ...) consists of sorted pairs with every 00 before every 01 before every 11.</description></item>
/// <item><description><strong>Step 3 - Sort the sorted pairs:</strong> Write the result as k = n/2 pairs (aₜ, bₜ). Its unsorted part is the run of 01 pairs,
/// whose length Parberry calls the amplitude. Comparing every bₜ with aₜ₊ₛ turns an amplitude of at most 2s into one of at most s. Applying this for
/// s = k/2, k/4, ..., 1 leaves at most one 01 pair, which is sorted, after log k layers of k − s comparators each, k log k − k + 1 comparators in all.</description></item>
/// <item><description><strong>Unrolled Layer Order:</strong> Step 1 of a recursion level precedes its recursive calls, and Step 3 follows them. Unrolling the recursion
/// therefore puts every Step 1 first — the first log n layers compare positions 1, 2, 4, ..., n/2 apart — followed by the Step 3 networks from the deepest
/// recursion level to the top. A recursion level at stride σ consists of σ interleaved subsequences (positions congruent mod σ) that share no position,
/// so one layer serves all of them: Step 1 at stride σ compares positions σ apart, and Step 3 at stride σ compares bₜ with aₜ₊ₛ, positions σ(2s − 1) apart.</description></item>
/// <item><description><strong>Arbitrary Size Support:</strong> Parberry defines the network for n a power of two. For other n this implementation takes the network for
/// k = the smallest power of two ≥ n and drops every comparator whose upper position is ≥ n. Every comparator puts its minimum on the lower position, so
/// +∞ padding at positions ≥ n would never move: the comparators that touch it never exchange, and dropping them leaves a network that sorts the first n positions.</description></item>
/// <item><description><strong>Comparison Network Property:</strong> The pairwise sorting network is a comparison network (data-oblivious). The positions compared
/// depend only on n, never on the values, so every input of length n runs the same comparators.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Network Sort / Merge</description></item>
/// <item><description>Stable      : No (pair layers past the first compare non-adjacent positions: [1a, 1b, 0] ends as [0, 1b, 1a])</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space)</description></item>
/// <item><description>Best case   : Θ(n log² n) - Data-independent comparison count (same as worst case)</description></item>
/// <item><description>Average case: Θ(n log² n) - Performs the same comparisons for all inputs</description></item>
/// <item><description>Worst case  : Θ(n log² n) - For n = 2^k: n × log n × (log n − 1) / 4 + n − 1 comparisons</description></item>
/// <item><description>Comparisons : Θ(n log² n) - For n = 2^k: n × log n × (log n − 1) / 4 + n − 1, the same size as Batcher's odd-even merge sort; the network runs the same comparators whatever the input</description></item>
/// <item><description>Swaps       : O(n log² n) - At most one per comparator, so at most the comparison count; how many actually fire depends on the input</description></item>
/// <item><description>Index Reads : Θ(n log² n) - every comparator reads the two positions it tests</description></item>
/// <item><description>Index Writes: O(n log² n) - each swap writes two positions, so the writes follow the swap count</description></item>
/// <item><description>Space       : O(1) - the comparators act directly on the input span</description></item>
/// <item><description>Depth       : O(log² n) - For n = 2^k: log n × (log n + 1) / 2 layers, the same depth as Batcher's odd-even merge sort</description></item>
/// </list>
/// <para><strong>Relation to Batcher Odd-Even Merge Sort:</strong></para>
/// <para>Both networks have the same size and depth, but they are not isomorphic: Parberry shows it for 16 inputs by examining the third layer, and for larger n by induction.
/// Batcher's network alternates subdivision and merging at every scale. The pairwise network performs all of its subdivision first and all of its merging
/// at the end, in the reverse order.</para>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>Iterative unrolling of Parberry's recursion, in the paper's layer order: all Step 1 layers (distances 1, 2, 4, ...), then the Step 3 layers
/// grouped by recursion level from the deepest (stride k/4) to the top (stride 1), each level with s descending.</description></item>
/// <item><description>Each layer announces one phase before its comparators run: <see cref="SortPhase.PairwisePairLayer"/> for Step 1 (param1 = distance, param2 = count)
/// and <see cref="SortPhase.PairwiseSortedPairsLayer"/> for Step 3 (param1 = distance σ(2s − 1), param2 = stride σ, param3 = count). Every announced layer contains
/// at least one comparator, so for n = 2^k the number of announcements equals the network depth.</description></item>
/// <item><description>The pseudocode in the Wikipedia article runs the same comparators in a different order: for each stride from n/2 down to 1 it runs that
/// level's pair layer and then its Step 3 layers, so a level's pairs are compared after its subsequences have been sorted rather than before. That order also
/// sorts — comparing two sorted sequences position by position leaves both sorted, so it reaches the same sorted-pairs state Step 3 needs — but it is not the
/// layer structure Parberry describes.</description></item>
/// <item><description>Index arithmetic uses <c>uint</c>, so loop bounds and the next stride or block start stay representable for any <c>int</c> length.</description></item>
/// <item><description>Uses only the input buffer (buffer ID 0); no heap allocation.</description></item>
/// </list>
/// <para><strong>History:</strong></para>
/// <para>Parberry published the network in 1992 as the first serious rival to Batcher's odd-even sorting network to appear in over twenty years:
/// a different construction with exactly the same size and depth, designed from the zero-one principle. The paper does not claim it is better than
/// Batcher's network; its Step 3 (sorting sorted pairs) is also used to reduce the depth of a reduction in Parberry's earlier work on sorting network verification.</para>
/// <para><strong>Reference:</strong></para>
/// <para>Parberry, I. (1992). "The Pairwise Sorting Network". Parallel Processing Letters, 2(2, 3), 205–211. https://doi.org/10.1142/S0129626492000337</para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Pairwise_sorting_network</para>
/// </remarks>
public static class PairwiseSortingNetwork
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
        SortCore(s, (uint)span.Length);
    }

    /// <summary>
    /// Runs the pairwise sorting network for <paramref name="count"/> positions, one layer at a time.
    /// </summary>
    /// <param name="s">The span to sort.</param>
    /// <param name="count">The number of positions; at least 2.</param>
    private static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, uint count)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Steps 1 and 2: the pair layer of every recursion level. The top level pairs adjacent positions;
        // the level at stride σ pairs positions σ apart within each of its σ interleaved subsequences.
        for (uint distance = 1; distance < count; distance = NextStride(distance))
        {
            s.Context.OnPhase(SortPhase.PairwisePairLayer, (int)distance, (int)count);
            PairLayer(s, distance, count);
        }

        // Step 3 for every recursion level, deepest first. halfWidth is k/2 for k = the smallest power of
        // two >= count; the level at stride σ holds k/(2σ) pairs, so its pair offsets s run from k/(4σ) to 1.
        var halfWidth = 1u << BitOperations.Log2(count - 1);
        for (var stride = halfWidth >> 1; stride > 0; stride >>= 1)
        {
            for (var offset = halfWidth / (stride << 1); offset > 0; offset >>= 1)
            {
                // b_t sits at σ(2t + 1) and a_(t+s) at σ(2t + 2s), so the comparator spans σ(2s - 1).
                var distance = (offset << 1) * stride - stride;
                s.Context.OnPhase(SortPhase.PairwiseSortedPairsLayer, (int)distance, (int)stride, (int)count);
                SortedPairsLayer(s, stride, distance, count);
            }
        }
    }

    /// <summary>
    /// Step 1 at stride <paramref name="distance"/>: compares every position whose <paramref name="distance"/> bit is clear
    /// with the position <paramref name="distance"/> above it.
    /// </summary>
    private static void PairLayer<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, uint distance, uint count)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var lowerLimit = count - distance; // a lower position at or past this has no upper partner
        for (uint block = 0; block < lowerLimit; block = NextBlock(block, distance))
        {
            var end = Math.Min(block + distance, lowerLimit);
            for (var i = block; i < end; i++)
            {
                CompareAndSwap(s, (int)i, (int)(i + distance));
            }
        }
    }

    /// <summary>
    /// One Step 3 layer at stride <paramref name="stride"/>: compares the larger element of every pair with the smaller
    /// element of the pair s positions ahead in the same subsequence. The larger elements are the positions whose
    /// <paramref name="stride"/> bit is set; each one's partner lies <paramref name="distance"/> = σ(2s − 1) above it.
    /// </summary>
    private static void SortedPairsLayer<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, uint stride, uint distance, uint count)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var lowerLimit = count - distance; // a lower position at or past this has no upper partner
        for (var lower = stride; lower < lowerLimit; lower = NextBlock(lower, stride))
        {
            var end = Math.Min(lower + stride, lowerLimit);
            for (var i = lower; i < end; i++)
            {
                CompareAndSwap(s, (int)i, (int)(i + distance));
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
    internal static uint NextStride(uint stride) => stride << 1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint NextBlock(uint start, uint stride) => start + (stride << 1);
}
