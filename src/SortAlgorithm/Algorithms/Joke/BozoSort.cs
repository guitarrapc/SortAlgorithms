using SortAlgorithm.Contexts;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 配列がソート済みかを確認し、ソートされていなければ異なる2つの位置をランダムに選んで交換する、という手順を繰り返す非常に非効率なソートアルゴリズムです。
/// Bogo Sortが毎回配列全体をシャッフルするのに対し、Bozo Sortは1回の互換（2要素の交換）しか行わないため、直前の並びから1手ずつランダムに歩いて昇順にたどり着くまで続けます。
/// <br/>
/// Repeatedly checks whether the array is sorted and, if it is not, swaps the elements at two distinct positions chosen at random.
/// Where Bogo Sort reshuffles the whole array each time, Bozo Sort applies a single transposition, so it random-walks one exchange at a time from the current arrangement until it reaches ascending order.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Bozo Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Sortedness Check:</strong> Before every transposition the array is scanned left to right comparing adjacent elements,
/// stopping at the first descent. A check therefore costs between 1 and n-1 comparisons, and exactly n-1 when it succeeds.</description></item>
/// <item><description><strong>Random Transposition:</strong> When the check fails, a pair of distinct positions {i, j} is chosen uniformly at random
/// (i from all n positions, j from the remaining n-1) and the two elements are swapped. Every iteration is a genuine exchange; i = j never occurs.</description></item>
/// <item><description><strong>Termination:</strong> Transpositions generate the symmetric group, so the arrangements of the input form a finite Markov chain
/// in which the sorted arrangement is reachable from every state. The sorted arrangement is reached with probability 1 and the expected number of swaps is finite,
/// although no single run has a finite upper bound.</description></item>
/// <item><description><strong>Not Stateless:</strong> Unlike Bogo Sort, the next arrangement depends on the current one. For distinct keys, an input whose permutation
/// decomposes into c cycles needs at least n - c swaps however lucky the choices are, and every swap flips the permutation's parity, so the swap count has the parity of the input permutation.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Joke / Exchange</description></item>
/// <item><description>Stable      : No (a random transposition can carry an element past equal keys)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space)</description></item>
/// <item><description>Best case   : Θ(n) - Already sorted, one sortedness check of n-1 comparisons and no swaps</description></item>
/// <item><description>Average case: Ω(n!) and O(n × n!) - Θ(n!) expected swaps, each preceded by a check of 1 to n-1 comparisons; Gruber et al. conjecture Θ(n!) overall</description></item>
/// <item><description>Worst case  : Unbounded - No run has a finite upper bound; Gruber et al. leave the expected swap count on the worst input open</description></item>
/// <item><description>Comparisons : Ω(n!) and O(n × n!) on average - between n! + 2(n-2)! + o((n-2)!) and (n-1)n! + 2(n-1)! + o((n-1)!) expected (Gruber et al., Theorem 12); n-1 when already sorted</description></item>
/// <item><description>Swaps       : Θ(n!) on average - n! + 2(n-2)! + o((n-2)!) expected (Flatto, Odlyzko and Wales, as stated in Gruber et al., Theorem 11); 0 when already sorted</description></item>
/// <item><description>Index Reads : Ω(n!) and O(n × n!) on average - two per comparison and two per swap</description></item>
/// <item><description>Index Writes: Θ(n!) on average - two per swap; 0 when already sorted</description></item>
/// <item><description>Space       : O(1) - the transpositions permute the input span</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>Gruber, Holzer and Ruepp point out that "swap two random elements" is ambiguous and name the two readings:
/// bozo-sort draws i and j independently, so i = j makes an iteration a no-op, and bozo-sort+ draws a pair of distinct positions.
/// This type implements bozo-sort+, the reading for which the expected swap count is known; every swap it reports moves two elements.</description></item>
/// <item><description>The variant that swaps a pair only when it is an inversion (bozo-sort_opt in the same paper) runs in expected polynomial time.
/// It is a different algorithm and is deliberately not used here.</description></item>
/// <item><description>Positions come from a xorshift32 generator with a fixed default seed, so a given input and seed always produce the same sequence of
/// swaps, which keeps statistics, visualizations, and benchmarks reproducible. The <c>seed</c> parameter selects another sequence; 0 falls back to the default.</description></item>
/// <item><description>Each transposition announces <see cref="SortPhase.BozoSwap"/> with param1 = attempt number (1-based), param2 = i, param3 = j, before the swap.</description></item>
/// </list>
/// <para><strong>History:</strong></para>
/// <para>Gruber, Holzer and Ruepp (2007) present bozo-sort as a variation of bogo-sort that replaces the shuffle with a random transposition and bound its cost.
/// The expected swap count rests on the hitting-time result for random transpositions by Flatto, Odlyzko and Wales (1985).</para>
/// <para><strong>Reference:</strong></para>
/// <para>Gruber, H.; Holzer, M.; Ruepp, O. (2007). "Sorting the Slow Way: An Analysis of Perversely Awful Randomized Sorting Algorithms".
/// Fun with Algorithms (FUN 2007), LNCS 4475: 183-197. doi:10.1007/978-3-540-72914-3_17</para>
/// <para>Flatto, L.; Odlyzko, A. M.; Wales, D. B. (1985). "Random Shuffles and Group Presentations". Annals of Probability 13 (1): 154-178.</para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Bogosort#Related_algorithms</para>
/// </remarks>
public static class BozoSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>See the stability note in this type's summary.</remarks>
    public static bool IsStable => false;

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const uint XORSHIFT_SEED = 0x9E3779B9u; // Golden ratio derived; deterministic seed for a reproducible swap sequence

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
    /// <param name="seed">Seed for the position generator. 0 selects the default seed.</param>
    public static void Sort<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context, uint seed = XORSHIFT_SEED)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        var rngState = seed == 0 ? XORSHIFT_SEED : seed;
        var length = (uint)span.Length;

        var attempt = 0;
        while (!IsSorted(s))
        {
            // Uniform pair of distinct positions: i from n positions, j from the other n-1.
            var i = (int)(NextRandom(ref rngState) % length);
            var j = (int)(NextRandom(ref rngState) % (length - 1));
            if (j >= i) j++;

            attempt++;
            context.OnPhase(SortPhase.BozoSwap, attempt, i, j);
            s.Swap(i, j);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSorted<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var length = s.Length;
        for (var i = 0; i < length - 1; i++)
        {
            if (s.Compare(i, i + 1) > 0)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Generates the next pseudo-random value using xorshift32.
    /// Deterministic: same seed always produces the same sequence, ensuring reproducible
    /// swap sequences for consistent visualization, statistics, and benchmarks.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint NextRandom(ref uint state)
    {
        state ^= state << 13;
        state ^= state >> 17;
        state ^= state << 5;
        return state;
    }
}
