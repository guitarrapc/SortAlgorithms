using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// バッチャー奇偶マージソート（分岐なし版） - BatcherOddEvenMergeSort と同じソーティングネットワークを、比較器ごとに最小値と最大値を常に両方の位置へ書き込む形で実行します。
/// 任意のサイズの入力に対応します。
/// <br/>
/// Batcher Odd-Even Merge Sort (Branchless) - Runs the same sorting network as BatcherOddEvenMergeSort, but every comparator writes its
/// minimum and its maximum to both of its positions instead of exchanging only an out-of-order pair.
/// Supports arbitrary input sizes.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Batcher Odd-Even Merge Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Same Network:</strong> The passes, stages, comparators and their order are those of BatcherOddEvenMergeSort:
/// the iterative formulation of the Wikipedia article for k = the smallest power of two ≥ n, without the comparators whose upper position
/// is ≥ n. Correctness for every n and the comparison count therefore carry over unchanged.</description></item>
/// <item><description><strong>Comparator as a Module:</strong> A comparator in Batcher's and Knuth's networks is a module with two inputs and two outputs
/// that always delivers the minimum on one output and the maximum on the other. This class carries out each comparator that way: it reads both
/// positions, compares them once, and writes the minimum to the lower position and the maximum to the upper one, whether or not they were
/// already in order. BatcherOddEvenMergeSort carries out the same comparator as a conditional exchange. Both leave the same values in the same
/// positions after every comparator.</description></item>
/// <item><description><strong>Data-Independent Execution:</strong> Which positions are compared, read and written depends only on n. When no context
/// observes the sort, the selection for primitive keys in their natural order does not branch either, so the work per comparator is the same for
/// every input. This is the form libc++'s <c>__cond_swap</c> and Rust's <c>swap_if_less</c> give a sorting network's comparators for primitive types.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Network Sort / Merge</description></item>
/// <item><description>Stable      : No (comparators join non-adjacent positions; equal keys may change order)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space)</description></item>
/// <item><description>Best case   : Θ(n log² n) - Data-independent comparison count (same as worst case)</description></item>
/// <item><description>Average case: Θ(n log² n) - Performs the same comparisons for all inputs</description></item>
/// <item><description>Worst case  : Θ(n log² n) - For n = 2^k: n × log n × (log n − 1) / 4 + n − 1 comparisons</description></item>
/// <item><description>Comparisons : Θ(n log² n) - For n = 2^k: n × log n × (log n − 1) / 4 + n − 1, one per comparator, as in BatcherOddEvenMergeSort</description></item>
/// <item><description>Swaps       : 0 - a comparator writes its minimum and maximum instead of exchanging</description></item>
/// <item><description>Index Reads : Θ(n log² n) - exactly two per comparator</description></item>
/// <item><description>Index Writes: Θ(n log² n) - exactly two per comparator, whatever the input</description></item>
/// <item><description>Space       : O(1) - the comparators act directly on the input span</description></item>
/// <item><description>Depth       : O(log² n) - For n = 2^k: log n × (log n + 1) / 2 layers, as in BatcherOddEvenMergeSort</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>Every context sees the same operations for every element type: two reads and one comparison, then a write to the lower
/// position and a write to the upper one.</description></item>
/// <item><description>Without an observing context, primitive integer, <c>float</c> and <c>double</c> keys in their natural order select the
/// minimum and maximum in a 128-bit vector register, keeping <see cref="IComparable{T}.CompareTo"/>'s order for NaN. Other keys and comparers,
/// and observed sorts, select with the comparison's result, which the JIT may compile to a branch.</description></item>
/// <item><description>Uses only the input buffer (buffer ID 0); no heap allocation.</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Batcher, K. E. (1968). "Sorting networks and their applications". AFIPS Spring Joint Computer Conference. https://www.cs.kent.edu/~batcher/sort.pdf</para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Batcher_odd%E2%80%93even_mergesort (source of the iterative formulation)</para>
/// <para>libc++ <c>__cond_swap</c> (include/__algorithm/sort.h): https://github.com/llvm/llvm-project/blob/main/libcxx/include/__algorithm/sort.h</para>
/// <para>Rust <c>swap_if_less</c> (core/src/slice/sort/shared/smallsort.rs): https://github.com/rust-lang/rust/blob/master/library/core/src/slice/sort/shared/smallsort.rs</para>
/// </remarks>
public static class BatcherOddEvenMergeSortBranchless
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

        SortCore(span, comparer, context);
    }

    /// <summary>
    /// Runs Batcher's odd-even merge network in BatcherOddEvenMergeSort's order. The loop control and its group check are
    /// explained there.
    /// </summary>
    /// <param name="span">The span to sort; at least 2 elements.</param>
    /// <param name="comparer">The comparer to use for element comparisons.</param>
    /// <param name="context">The sort context.</param>
    private static void SortCore<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // The SortSpan is a local of the method that runs the loops. Passed in, it is reached through a reference, and the
        // JIT reloaded the span's base pointer after every write.
        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        var n = (uint)span.Length;

        // The next pass and group offset can exceed int.MaxValue before the loop exits.
        for (uint p = 1; p < n; p = BatcherOddEvenMergeSort.NextPass(p))
        {
            s.Context.OnPhase(SortPhase.OddEvenMergeSortPass, (int)p, (int)n);
            var blockMask = (p << 1) - 1;

            for (var k = p; k >= 1; k >>= 1)
            {
                s.Context.OnPhase(SortPhase.OddEvenMergeSortStage, (int)k, (int)p, (int)n);
                var rightLimit = n - k;

                for (var j = k % p; j < rightLimit; j = BatcherOddEvenMergeSort.NextGroup(j, k))
                {
                    // A group whose pairs straddle a merge block of size 2p has no comparators.
                    if (((j + k) & blockMask) == 0) continue;

                    var end = j + Math.Min(k, rightLimit - j);
                    for (var left = j; left < end; left++)
                    {
                        BranchlessComparator.Ascending(s, (int)left, (int)(left + k));
                    }
                }
            }
        }
    }
}
