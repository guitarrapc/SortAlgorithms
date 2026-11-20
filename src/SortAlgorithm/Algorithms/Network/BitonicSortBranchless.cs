using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// バイトニックソート（分岐なし版） - BitonicSort と同じソーティングネットワークを、比較器ごとに最小値と最大値を常に両方の位置へ書き込む形で実行します。
/// 入力サイズは2のべき乗（2^n）でなければなりません。
/// <br/>
/// Bitonic Sort (Branchless) - Runs the same sorting network as BitonicSort, but every comparator writes its minimum and its maximum
/// to both of its positions instead of exchanging only an out-of-order pair.
/// Input length must be a power of 2 (2^n).
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Bitonic Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Same Network:</strong> The levels, stages, comparators, their order and their directions are those of BitonicSort:
/// level k = 2, 4, ..., n runs stages j = k/2, ..., 1, and stage j compares every position i whose j bit is clear with i + j, ascending when
/// (i &amp; k) == 0 and descending otherwise. Correctness, the power-of-two requirement and the comparison count therefore carry over unchanged.</description></item>
/// <item><description><strong>Comparator as a Module:</strong> A comparator in Batcher's and Knuth's networks is a module with two inputs and two outputs
/// that always delivers the minimum on one output and the maximum on the other. This class carries out each comparator that way: it reads both
/// positions, compares them once, and writes the minimum to one and the maximum to the other, whether or not they were already in order.
/// BitonicSort carries out the same comparator as a conditional exchange. Both leave the same values in the same positions after every comparator.</description></item>
/// <item><description><strong>Data-Independent Execution:</strong> Which positions are compared, read and written depends only on n. When no context
/// observes the sort, the selection for primitive keys in their natural order does not branch either, so the work per comparator is the same for
/// every input. This is the form libc++'s <c>__cond_swap</c> and Rust's <c>swap_if_less</c> give a sorting network's comparators for primitive types.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Network Sort / Exchange</description></item>
/// <item><description>Stable      : No (comparators join non-adjacent positions; equal keys may change order)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space - sorts directly on input array)</description></item>
/// <item><description>Best case   : Θ(n log² n) - Data-independent comparison count (same as worst case)</description></item>
/// <item><description>Average case: Θ(n log² n) - Performs the same comparisons for all inputs</description></item>
/// <item><description>Worst case  : Θ(n log² n) - For n = 2^k: n × log n × (log n + 1) / 4 comparisons</description></item>
/// <item><description>Comparisons : Θ(n log² n) - Exactly n × log n × (log n + 1) / 4 for n = 2^k, one per comparator, as in BitonicSort</description></item>
/// <item><description>Swaps       : 0 - a comparator writes its minimum and maximum instead of exchanging</description></item>
/// <item><description>Index Reads : Θ(n log² n) - exactly two per comparator, n × log n × (log n + 1) / 2 for n = 2^k</description></item>
/// <item><description>Index Writes: Θ(n log² n) - exactly two per comparator, n × log n × (log n + 1) / 2 for n = 2^k, whatever the input</description></item>
/// <item><description>Space       : O(1) - the comparators act directly on the input span</description></item>
/// <item><description>Depth       : O(log² n) - For n = 2^k: log n × (log n + 1) / 2 layers of n/2 comparators each, as in BitonicSort</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>Strictly requires power-of-2 input length. Throws ArgumentException otherwise.</description></item>
/// <item><description>Every context sees the same operations for every element type: two reads and one comparison, then a write to the lower
/// position and a write to the upper one.</description></item>
/// <item><description>Without an observing context, primitive integer, <c>float</c> and <c>double</c> keys in their natural order select the
/// minimum and maximum in a 128-bit vector register: integer minimum and maximum instructions, and for <c>float</c> and <c>double</c> a compare
/// and blend that keeps <see cref="IComparable{T}.CompareTo"/>'s order, NaN below every number. Other keys and comparers, and observed sorts,
/// select with the comparison's result, which the JIT may compile to a branch.</description></item>
/// <item><description>Uses only the input buffer (buffer ID 0); no heap allocation.</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Batcher, K. E. (1968). "Sorting networks and their applications". AFIPS Spring Joint Computer Conference. https://doi.org/10.1145/1468075.1468121</para>
/// <para>libc++ <c>__cond_swap</c> (include/__algorithm/sort.h): https://github.com/llvm/llvm-project/blob/main/libcxx/include/__algorithm/sort.h</para>
/// <para>Rust <c>swap_if_less</c> (core/src/slice/sort/shared/smallsort.rs): https://github.com/rust-lang/rust/blob/master/library/core/src/slice/sort/shared/smallsort.rs</para>
/// </remarks>
public static class BitonicSortBranchless
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

        if (!int.IsPow2(span.Length))
            throw new ArgumentException($"Bitonic sort requires input length to be a power of 2. Actual length: {span.Length}", nameof(span));

        SortCore(span, comparer, context);
    }

    /// <summary>
    /// Runs the bitonic sorting network level by level, in BitonicSort's order.
    /// </summary>
    /// <param name="span">The span to sort; a power of two, at least 2 elements.</param>
    /// <param name="comparer">The comparer to use for element comparisons.</param>
    /// <param name="context">The sort context.</param>
    private static void SortCore<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // The SortSpan is a local of the method that runs the loops, so the JIT can keep the span's base pointer in a
        // register across the writes.
        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        var count = (uint)span.Length;

        // The next level is 2^31 after the largest valid network.
        for (uint k = 2; k <= count; k <<= 1)
        {
            s.Context.OnPhase(SortPhase.BitonicLevel, (int)k, (int)count);
            for (var j = k >> 1; j > 0; j >>= 1)
            {
                s.Context.OnPhase(SortPhase.BitonicStage, (int)j, (int)k, (int)count);

                // Runs of k positions alternate ascending (k bit clear) and descending.
                for (uint run = 0; run < count; run += k)
                {
                    var end = run + k;
                    if ((run & k) == 0)
                    {
                        for (var group = run; group < end; group += j << 1)
                        {
                            for (var i = group; i < group + j; i++)
                            {
                                BranchlessComparator.Ascending(s, (int)i, (int)(i + j));
                            }
                        }
                    }
                    else
                    {
                        for (var group = run; group < end; group += j << 1)
                        {
                            for (var i = group; i < group + j; i++)
                            {
                                BranchlessComparator.Descending(s, (int)i, (int)(i + j));
                            }
                        }
                    }
                }
            }
        }
    }
}
