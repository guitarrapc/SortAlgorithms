using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// バイトニックソート（2のべき乗専用版・最適化版） - バイトニック列を構築し、イテレーティブにマージして整列列に変換するソーティングネットワークアルゴリズムです。
/// 入力サイズは2のべき乗（2^n）でなければなりません。任意のサイズには BitonicSortAnyLength か BatcherOddEvenMergeSort を使用してください。
/// 再帰版の実装は BitonicSortNonOptimized を参照してください。
/// <br/>
/// Bitonic Sort (Power-of-2 Only, Optimized) - A sorting network algorithm that builds a bitonic sequence and iteratively merges it into a sorted sequence.
/// Input length must be a power of 2 (2^n). For arbitrary sizes, use BitonicSortAnyLength or BatcherOddEvenMergeSort instead.
/// For recursive implementation, see BitonicSortNonOptimized.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Bitonic Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Bitonic Sequence Definition:</strong> A sequence is bitonic if it first monotonically increases then monotonically decreases,
/// or can be circularly rotated to achieve this property. For example: [3,7,9,5,2,1] is bitonic.</description></item>
/// <item><description><strong>Power-of-Two Requirement:</strong> The input length must be a power of 2 (n = 2^m for some integer m ≥ 0).
/// This ensures the divide-and-conquer structure maintains balanced splits at each stage.
/// If n is not a power of 2, this implementation throws ArgumentException. Use BitonicSortAnyLength or BatcherOddEvenMergeSort for arbitrary sizes.</description></item>
/// <item><description><strong>Iterative Bitonic Construction:</strong> The network runs levels k = 2, 4, ..., n. Level k runs stages
/// j = k/2, k/4, ..., 1, and stage j compares every position i whose j bit is clear with i ^ j = i + j. The comparator sorts ascending
/// when (i &amp; k) == 0 and descending otherwise, so after level k the runs of k positions alternate ascending and descending and every
/// pair of neighbouring runs forms a bitonic run of 2k positions for the next level. At the last level (k = n) every comparator is ascending.
/// This is the unrolled form of the recursive construction in BitonicSortNonOptimized: the same comparators, grouped into layers.</description></item>
/// <item><description><strong>Bitonic Merge Correctness:</strong> Stage j acts on runs of 2j positions as a half-cleaner: it compares each
/// position of the lower half with the position j above it. Batcher shows that on a bitonic run this leaves both halves bitonic with
/// every element of the lower half ≤ every element of the upper half (≥ for a descending comparator). The stages j = k/2, ..., 1 therefore
/// sort each bitonic run of k positions.
/// <para><strong>Proof sketch:</strong> Let S = [a₁,...,aₘ, b₁,...,bₘ] be bitonic. After comparing aᵢ with bᵢ for every i:
/// <list type="bullet">
/// <item>min(aᵢ, bᵢ) is in the lower half and max(aᵢ, bᵢ) in the upper half.
/// All elements in the lower half are ≤ all elements in the upper half, and both halves remain bitonic,
/// so the next stage can apply the same step to each half.</item>
/// </list></para></description></item>
/// <item><description><strong>Comparison Network Property:</strong> Bitonic sort is a comparison network, meaning the sequence of comparisons
/// is data-independent (oblivious). The same comparisons are performed regardless of input values, making it ideal for parallel hardware
/// implementations, SIMD optimizations, and worst-case guarantees.</description></item>
/// <item><description><strong>Iteration Termination:</strong> Base case: sequences of length 1 are trivially sorted. The iterative implementation
/// processes log₂ n levels k = 2, 4, ..., n. Level k has log₂ k stages of n/2 comparisons each, so the total is
/// n/2 × (1 + 2 + ... + log₂ n) = n × log n × (log n + 1) / 4 comparisons, which is Θ(n log² n).</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Network Sort / Exchange</description></item>
/// <item><description>Stable      : No (swapping non-adjacent elements can change relative order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space - sorts directly on input array)</description></item>
/// <item><description>Best case   : Θ(n log² n) - Data-independent comparison count (same as worst case)</description></item>
/// <item><description>Average case: Θ(n log² n) - Performs the same comparisons for all inputs</description></item>
/// <item><description>Worst case  : Θ(n log² n) - For n = 2^k: n × log n × (log n + 1) / 4 comparisons</description></item>
/// <item><description>Comparisons : Θ(n log² n) - Exactly n × log n × (log n + 1) / 4 for n = 2^k (28,160 for n = 1024, the figure in Batcher's paper)</description></item>
/// <item><description>Swaps       : O(n log² n) - At most equal to comparison count; depends on input disorder</description></item>
/// <item><description>Index Reads : Θ(n log² n) - every comparator reads the two positions it tests, and the network runs the same comparators whatever the input</description></item>
/// <item><description>Index Writes: O(n log² n) - each swap writes two positions, so the writes follow the swap count</description></item>
/// <item><description>Space       : O(1) - the comparators act directly on the input span</description></item>
/// <item><description>Depth       : O(log² n) - For n = 2^k: log n × (log n + 1) / 2 layers of n/2 comparators each, so O(log² n) parallel time with n/2 comparators</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>Strictly requires power-of-2 input length. Throws ArgumentException otherwise.</description></item>
/// <item><description>Each stage enumerates its comparators directly: runs of k positions in order, groups of 2j within a run, and the lower
/// position of every pair. That is the same comparators in the same order as the loop in the Wikipedia article, which visits every i and
/// skips the half whose partner i ^ j lies below it, and the direction is chosen once per run of k positions rather than once per comparator.</description></item>
/// <item><description>Comparators run one at a time; the parallelism within a layer is not exploited.</description></item>
/// <item><description>Uses only the input buffer (buffer ID 0); no heap allocation.</description></item>
/// <item><description>For the recursive formulation, see BitonicSortNonOptimized. For the same network with comparators that always write
/// both positions, see BitonicSortBranchless.</description></item>
/// </list>
/// <para><strong>Use Cases:</strong></para>
/// <list type="bullet">
/// <item><description>Parallel sorting on GPUs or multi-core systems (when parallelized via SIMD or threads)</description></item>
/// <item><description>Hardware sorting networks (FPGA, ASIC implementations)</description></item>
/// <item><description>Educational purposes to demonstrate oblivious sorting algorithms</description></item>
/// <item><description>Scenarios where input size is guaranteed to be a power of 2 and zero allocation is required</description></item>
/// </list>
/// <para><strong>History:</strong></para>
/// <para>Batcher published bitonic sorters in "Sorting networks and their applications" (AFIPS SJCC, 1968). The paper cites his
/// earlier Goodyear Aerospace report GER-11869 "Bitonic sorting" (1964), which was not found online.</para>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Bitonic_sorter</para>
/// <para>Batcher, K. E. (1968). "Sorting networks and their applications". AFIPS Spring Joint Computer Conference. https://doi.org/10.1145/1468075.1468121</para>
/// </remarks>
public static class BitonicSort
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

        // Verify that length is a power of 2
        if (!IsPowerOfTwo(span.Length))
            throw new ArgumentException($"Bitonic sort requires input length to be a power of 2. Actual length: {span.Length}", nameof(span));

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        SortCore(s, (uint)span.Length);
    }

    /// <summary>
    /// Runs the bitonic sorting network level by level. Level k merges bitonic runs of k positions into sorted runs,
    /// and stage j of a level compares every position i whose j bit is clear with i ^ j = i + j.
    /// </summary>
    /// <param name="s">The span to sort.</param>
    /// <param name="count">The number of positions; a power of two, at least 2.</param>
    private static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, uint count)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // The next level is 2^31 after the largest valid network.
        for (uint k = 2; k <= count; k = NextLevel(k))
        {
            s.Context.OnPhase(SortPhase.BitonicLevel, (int)k, (int)count);
            for (var j = k >> 1; j > 0; j >>= 1)
            {
                s.Context.OnPhase(SortPhase.BitonicStage, (int)j, (int)k, (int)count);

                // A comparator sorts ascending when its lower position has the k bit clear. That bit is the same
                // across each run of k positions, so the direction is chosen once per run.
                for (uint run = 0; run < count; run += k)
                {
                    if ((run & k) == 0)
                    {
                        AscendingStage(s, run, run + k, j);
                    }
                    else
                    {
                        DescendingStage(s, run, run + k, j);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Runs stage <paramref name="j"/> on [<paramref name="start"/>, <paramref name="end"/>) in ascending order:
    /// in each group of 2j positions, the lower j are compared with the position j above them.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AscendingStage<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, uint start, uint end, uint j)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        for (var group = start; group < end; group += j << 1)
        {
            for (var i = group; i < group + j; i++)
            {
                CompareAndSwapAscending(s, (int)i, (int)(i + j));
            }
        }
    }

    /// <summary>
    /// Runs stage <paramref name="j"/> on [<paramref name="start"/>, <paramref name="end"/>) in descending order.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DescendingStage<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, uint start, uint end, uint j)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        for (var group = start; group < end; group += j << 1)
        {
            for (var i = group; i < group + j; i++)
            {
                CompareAndSwapDescending(s, (int)i, (int)(i + j));
            }
        }
    }

    /// <summary>
    /// Compares two elements and swaps them if they are in the wrong order for ascending order.
    /// </summary>
    /// <param name="s">The span containing the elements.</param>
    /// <param name="i">The index of the first element.</param>
    /// <param name="j">The index of the second element.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CompareAndSwapAscending<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int i, int j)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Swap if i > j (wrong order for ascending)
        if (s.IsGreaterAt(i, j))
        {
            s.Swap(i, j);
        }
    }

    /// <summary>
    /// Compares two elements and swaps them if they are in the wrong order for descending order.
    /// </summary>
    /// <param name="s">The span containing the elements.</param>
    /// <param name="i">The index of the first element.</param>
    /// <param name="j">The index of the second element.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CompareAndSwapDescending<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int i, int j)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Swap if i < j (wrong order for descending)
        if (s.IsLessAt(i, j))
        {
            s.Swap(i, j);
        }
    }

    /// <summary>
    /// Checks if a number is a power of 2.
    /// </summary>
    /// <param name="n">The number to check.</param>
    /// <returns>True if n is a power of 2, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsPowerOfTwo(int n)
    {
        return n > 0 && (n & (n - 1)) == 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint NextLevel(uint level) => level << 1;
}

/// <summary>
/// 非最適化バージョンのバイトニックソート - 再帰的な実装で、アルゴリズムの基本構造を明確に示しています。
/// バイトニックソート（2のべき乗専用版） - バイトニック列を構築し、再帰的にマージして整列列に変換するソーティングネットワークアルゴリズムです。
/// 入力サイズは2のべき乗（2^n）でなければなりません。任意のサイズには BitonicSortAnyLength か BatcherOddEvenMergeSort を使用してください。
/// <br/>
/// Bitonic Sort (Power-of-2 Only) - A sorting network algorithm that builds a bitonic sequence and recursively merges it into a sorted sequence.
/// Input length must be a power of 2 (2^n). For arbitrary sizes, use BitonicSortAnyLength or BatcherOddEvenMergeSort instead.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Bitonic Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Bitonic Sequence Definition:</strong> A sequence is bitonic if it first monotonically increases then monotonically decreases,
/// or can be circularly rotated to achieve this property. For example: [3,7,9,5,2,1] is bitonic.</description></item>
/// <item><description><strong>Power-of-Two Requirement:</strong> The input length must be a power of 2 (n = 2^m for some integer m ≥ 0).
/// This ensures the divide-and-conquer structure maintains balanced splits at each recursive level.
/// If n is not a power of 2, this implementation throws ArgumentException. Use BitonicSortAnyLength or BatcherOddEvenMergeSort for arbitrary sizes.</description></item>
/// <item><description><strong>Recursive Bitonic Construction:</strong> Divide the input into two halves. Recursively sort the first half in ascending order
/// and the second half in descending order. The concatenation of these two sorted subsequences forms a bitonic sequence.
/// This is because the first half increases (a₁ ≤ a₂ ≤ ... ≤ aₖ) and the second half decreases (bₖ ≥ ... ≥ b₂ ≥ b₁),
/// creating the pattern: increasing then decreasing. The halves get these directions at every level, including inside a region
/// that is itself merged in descending order, so the comparators and their directions are exactly those of BitonicSort:
/// a run of k positions is ascending when its k bit is clear.
/// Batcher's comparators all put the low value on top; he makes a merge input bitonic by joining two sorted lists with the second reversed.
/// Sorting the second half descending is that reversal written in array positions.</description></item>
/// <item><description><strong>Bitonic Merge Correctness:</strong> Given a bitonic sequence of length n = 2k, compare and conditionally swap elements
/// at distance k apart (i.e., compare element[i] with element[i+k] for i ∈ [0, k)). This operation, called a bitonic split,
/// partitions the sequence into two bitonic subsequences of length k each, where all elements in the first half are ≤ all elements
/// in the second half. Recursively applying bitonic merge to each half produces a fully sorted sequence.
/// <para><strong>Proof sketch:</strong> Let S = [a₁,...,aₖ, b₁,...,bₖ] be bitonic. After comparing and swapping (aᵢ, bᵢ) for all i:
/// <list type="bullet">
/// <item>If S is increasing then decreasing: aᵢ ≤ aᵢ₊₁ ≤ ... ≤ aₖ and bₖ ≥ ... ≥ b₂ ≥ b₁.
/// After swap: min(aᵢ, bᵢ) goes to first half, max(aᵢ, bᵢ) to second half.
/// All elements in first half ≤ all elements in second half.
/// Both halves remain bitonic.</item>
/// </list></para></description></item>
/// <item><description><strong>Comparison Network Property:</strong> Bitonic sort is a comparison network, meaning the sequence of comparisons
/// is data-independent (oblivious). The same comparisons are performed regardless of input values, making it ideal for parallel hardware
/// implementations, SIMD optimizations, and worst-case guarantees.</description></item>
/// <item><description><strong>Recursion Termination:</strong> Base case: sequences of length 1 are trivially sorted. Recursion depth is log₂ n.
/// A bitonic merge of m elements runs log₂ m stages of m/2 comparisons each, so the merges at recursion level i (blocks of n/2^i)
/// cost n/2 × log₂(n/2^i). Summed over the levels this is n × log n × (log n + 1) / 4 comparisons, which is Θ(n log² n).</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Network Sort / Exchange</description></item>
/// <item><description>Stable      : No (swapping non-adjacent elements can change relative order of equal elements)</description></item>
/// <item><description>In-place    : Yes (no auxiliary buffer; the recursion keeps O(log n) frames on the call stack)</description></item>
/// <item><description>Best case   : Θ(n log² n) - Data-independent comparison count (same as worst case)</description></item>
/// <item><description>Average case: Θ(n log² n) - Performs the same comparisons for all inputs</description></item>
/// <item><description>Worst case  : Θ(n log² n) - For n = 2^k: n × log n × (log n + 1) / 4 comparisons</description></item>
/// <item><description>Comparisons : Θ(n log² n) - Exactly n × log n × (log n + 1) / 4 for n = 2^k (28,160 for n = 1024, the figure in Batcher's paper)</description></item>
/// <item><description>Swaps       : O(n log² n) - At most equal to comparison count; depends on input disorder</description></item>
/// <item><description>Index Reads : Θ(n log² n) - every comparator reads the two positions it tests, and the network runs the same comparators whatever the input</description></item>
/// <item><description>Index Writes: O(n log² n) - each swap writes two positions, so the writes follow the swap count</description></item>
/// <item><description>Space       : O(log n) - the recursion stack; the comparators act directly on the input span</description></item>
/// <item><description>Depth       : O(log² n) - For n = 2^k: log n × (log n + 1) / 2 layers of n/2 comparators each, so O(log² n) parallel time with n/2 comparators</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>Strictly requires power-of-2 input length. Throws ArgumentException otherwise.</description></item>
/// <item><description>Runs the comparators depth first, in recursion order, so a layer's comparators are not adjacent in time.
/// BitonicSort runs the same network layer by layer.</description></item>
/// <item><description>Each merge chooses its direction once and runs a direction-specific comparator loop.</description></item>
/// <item><description>Uses only the input buffer (buffer ID 0); no heap allocation.</description></item>
/// </list>
/// <para><strong>Use Cases:</strong></para>
/// <list type="bullet">
/// <item><description>Parallel sorting on GPUs or multi-core systems (when parallelized via SIMD or threads)</description></item>
/// <item><description>Hardware sorting networks (FPGA, ASIC implementations)</description></item>
/// <item><description>Educational purposes to demonstrate oblivious sorting algorithms</description></item>
/// <item><description>Scenarios where input size is guaranteed to be a power of 2 and zero allocation is required</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Bitonic_sorter</para>
/// <para>Batcher, K. E. (1968). "Sorting networks and their applications". AFIPS Spring Joint Computer Conference. https://doi.org/10.1145/1468075.1468121</para>
/// </remarks>
public static class BitonicSortNonOptimized
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

        // Verify that length is a power of 2
        if (!IsPowerOfTwo(span.Length))
            throw new ArgumentException($"Bitonic sort requires input length to be a power of 2. Actual length: {span.Length}", nameof(span));

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        SortCore(s, 0, span.Length, ascending: true);
    }

    /// <summary>
    /// Recursively builds and merges a bitonic sequence.
    /// </summary>
    /// <param name="s">The span to sort.</param>
    /// <param name="low">The starting index of the sequence.</param>
    /// <param name="count">The length of the sequence.</param>
    /// <param name="ascending">True to sort this region in ascending order, false for descending.</param>
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int low, int count, bool ascending)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (count > 1)
        {
            int k = count / 2;

            // An ascending half followed by a descending half is bitonic, whichever direction this region is merged in.
            SortCore(s, low, k, ascending: true);
            SortCore(s, low + k, k, ascending: false);

            // Merge the bitonic sequence in the desired order
            BitonicMerge(s, low, count, ascending);
        }
    }

    /// <summary>
    /// Merges a bitonic sequence into a sorted sequence.
    /// </summary>
    /// <param name="s">The span containing the bitonic sequence.</param>
    /// <param name="low">The starting index of the bitonic sequence.</param>
    /// <param name="count">The length of the bitonic sequence.</param>
    /// <param name="ascending">True to merge in ascending order, false for descending.</param>
    private static void BitonicMerge<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int low, int count, bool ascending)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (count > 1)
        {
            int k = count / 2;

            // Compare and swap elements at distance k apart using specialized methods
            if (ascending)
            {
                for (int i = low; i < low + k; i++)
                {
                    CompareAndSwapAscending(s, i, i + k);
                }
            }
            else
            {
                for (int i = low; i < low + k; i++)
                {
                    CompareAndSwapDescending(s, i, i + k);
                }
            }

            // Recursively merge both halves
            BitonicMerge(s, low, k, ascending);
            BitonicMerge(s, low + k, k, ascending);
        }
    }

    /// <summary>
    /// Compares two elements and swaps them if they are in the wrong order for ascending order.
    /// </summary>
    /// <param name="s">The span containing the elements.</param>
    /// <param name="i">The index of the first element.</param>
    /// <param name="j">The index of the second element.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CompareAndSwapAscending<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int i, int j)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Swap if i > j (wrong order for ascending)
        if (s.IsGreaterAt(i, j))
        {
            s.Swap(i, j);
        }
    }

    /// <summary>
    /// Compares two elements and swaps them if they are in the wrong order for descending order.
    /// </summary>
    /// <param name="s">The span containing the elements.</param>
    /// <param name="i">The index of the first element.</param>
    /// <param name="j">The index of the second element.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CompareAndSwapDescending<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int i, int j)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Swap if i < j (wrong order for descending)
        if (s.IsLessAt(i, j))
        {
            s.Swap(i, j);
        }
    }

    /// <summary>
    /// Checks if a number is a power of 2.
    /// </summary>
    /// <param name="n">The number to check.</param>
    /// <returns>True if n is a power of 2, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsPowerOfTwo(int n)
    {
        return n > 0 && (n & (n - 1)) == 0;
    }
}
