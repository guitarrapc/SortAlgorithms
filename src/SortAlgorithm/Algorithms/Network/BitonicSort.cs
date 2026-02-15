using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// バイトニックソート（2のべき乗専用版） - バイトニック列を構築し、再帰的にマージして整列列に変換するソーティングネットワークアルゴリズムです。
/// 入力サイズは2のべき乗（2^n）でなければなりません。任意のサイズに対応する場合は BitonicSortFill を使用してください。
/// <br/>
/// Bitonic Sort (Power-of-2 Only) - A sorting network algorithm that builds a bitonic sequence and recursively merges it into a sorted sequence.
/// Input length must be a power of 2 (2^n). For arbitrary sizes, use BitonicSortFill instead.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Bitonic Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Bitonic Sequence Definition:</strong> A sequence is bitonic if it first monotonically increases then monotonically decreases,
/// or can be circularly rotated to achieve this property. For example: [3,7,9,5,2,1] is bitonic.</description></item>
/// <item><description><strong>Power-of-Two Requirement:</strong> The input length must be a power of 2 (n = 2^m for some integer m ≥ 0).
/// This ensures the divide-and-conquer structure maintains balanced splits at each recursive level.
/// If n is not a power of 2, this implementation throws ArgumentException. Use BitonicSortFill for arbitrary sizes.</description></item>
/// <item><description><strong>Recursive Bitonic Construction:</strong> Divide the input into two halves. Recursively sort the first half in ascending order
/// and the second half in descending order. The concatenation of these two sorted subsequences forms a bitonic sequence.
/// This is because the first half increases (a₁ ≤ a₂ ≤ ... ≤ aₖ) and the second half decreases (bₖ ≥ ... ≥ b₂ ≥ b₁),
/// creating the pattern: increasing then decreasing.</description></item>
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
/// <item><description><strong>Recursion Termination:</strong> Base case: sequences of length 1 are trivially sorted. Recursion depth is log₂ n,
/// and at each level, O(n) comparisons are performed, yielding O(n log² n) total comparisons.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Network Sort / Exchange</description></item>
/// <item><description>Stable      : No (swapping non-adjacent elements can change relative order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space - sorts directly on input array)</description></item>
/// <item><description>Best case   : Θ(n log² n) - Data-independent comparison count (same as worst case)</description></item>
/// <item><description>Average case: Θ(n log² n) - Performs the same comparisons for all inputs</description></item>
/// <item><description>Worst case  : Θ(n log² n) - Comparison count: ½ × n × log² n for n = 2^k</description></item>
/// <item><description>Comparisons : Θ(n log² n) - Exactly (log² n × (log n + 1)) / 4 × n comparisons for n = 2^k</description></item>
/// <item><description>Swaps       : O(n log² n) - At most equal to comparison count; depends on input disorder</description></item>
/// <item><description>Parallel depth: O(log² n) - Network depth allows O(log² n) parallel time with O(n log n) processors</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>Strictly requires power-of-2 input length. Throws ArgumentException otherwise.</description></item>
/// <item><description>True in-place sorting with zero heap allocation (all work done on input span).</description></item>
/// <item><description>Sequential implementation: Does not exploit parallelism (comparisons are executed sequentially).</description></item>
/// <item><description>Uses aggressive inlining for performance-critical helper methods (CompareAndSwap, IsPowerOfTwo, etc.).</description></item>
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
/// </remarks>
public static class BitonicSort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array

    private readonly struct BitonicSortAction<T, TComparer> : ContextDispatcher.SortAction<T, TComparer>
        where TComparer : IComparer<T>
    {
        public void Invoke<TContext>(Span<T> span, TComparer comparer, TContext context)
            where TContext : ISortContext
        {
            Sort<T, TComparer, TContext>(span, comparer, context);
        }
    }

    /// <summary>
    /// Sorts the elements in the specified span in ascending order using the default comparer.
    /// Uses NullContext for zero-overhead fast path.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort in place.</param>
    /// <exception cref="ArgumentException">Thrown when the span length is not a power of 2.</exception>
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
        => Sort<T, ComparableComparer<T>, NullContext>(span, new ComparableComparer<T>(), NullContext.Default);

    /// <summary>
    /// Sorts the elements in the specified span using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    /// <exception cref="ArgumentException">Thrown when the span length is not a power of 2.</exception>
    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
        => ContextDispatcher.DispatchSort(span, new ComparableComparer<T>(), context, new BitonicSortAction<T, ComparableComparer<T>>());

    /// <summary>
    /// Sorts the elements in the specified span using the provided comparer and sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use for element comparisons.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="comparer">The comparer to use for element comparisons.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    /// <exception cref="ArgumentException">Thrown when the span length is not a power of 2.</exception>
    public static void Sort<T, TComparer>(Span<T> span, TComparer comparer, ISortContext context)
        where TComparer : IComparer<T>
        => ContextDispatcher.DispatchSort(span, comparer, context, new BitonicSortAction<T, TComparer>());

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
    /// <param name="span">The span to sort.</param>
    /// <param name="low">The starting index of the sequence.</param>
    /// <param name="count">The length of the sequence.</param>
    /// <param name="ascending">True to sort in ascending order, false for descending.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> span, int low, int count, bool ascending)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (count > 1)
        {
            int k = count / 2;

            // Recursively sort first half in ascending order
            SortCore(span, low, k, ascending: true);
            // Recursively sort second half in descending order to create bitonic sequence
            SortCore(span, low + k, k, ascending: false);

            // Merge the bitonic sequence in the desired order
            BitonicMerge(span, low, count, ascending);
        }
    }

    /// <summary>
    /// Merges a bitonic sequence into a sorted sequence.
    /// </summary>
    /// <param name="span">The span containing the bitonic sequence.</param>
    /// <param name="low">The starting index of the bitonic sequence.</param>
    /// <param name="count">The length of the bitonic sequence.</param>
    /// <param name="ascending">True to merge in ascending order, false for descending.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void BitonicMerge<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> span, int low, int count, bool ascending)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (count > 1)
        {
            int k = count / 2;

            // Compare and swap elements at distance k apart
            for (int i = low; i < low + k; i++)
            {
                CompareAndSwap(span, i, i + k, ascending);
            }

            // Recursively merge both halves
            BitonicMerge(span, low, k, ascending);
            BitonicMerge(span, low + k, k, ascending);
        }
    }

    /// <summary>
    /// Compares two elements and swaps them if they are in the wrong order.
    /// </summary>
    /// <param name="span">The span containing the elements.</param>
    /// <param name="i">The index of the first element.</param>
    /// <param name="j">The index of the second element.</param>
    /// <param name="ascending">True if elements should be in ascending order, false for descending.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CompareAndSwap<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> span, int i, int j, bool ascending)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        int cmp = span.Compare(i, j);

        // If ascending and i > j, or if descending and i < j, then swap
        if ((ascending && cmp > 0) || (!ascending && cmp < 0))
        {
            span.Swap(i, j);
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
