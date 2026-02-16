using System.Buffers;
using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// バイトニックソート - バイトニック列（単調増加後に単調減少する列、またはその逆）を構築し、再帰的にマージして整列列に変換するソーティングネットワークアルゴリズムです。
/// 入力サイズが2のべき乗でない場合、最大値でパディングして次の2のべき乗まで拡張し、ソート後に元のサイズにトリミングします。
/// <br/>
/// Bitonic Sort - A sorting network algorithm that builds a bitonic sequence (monotonically increasing then decreasing, or vice versa)
/// and recursively merges it into a sorted sequence. For non-power-of-2 inputs, pads with the maximum value to the next power of 2,
/// sorts, and then trims back to the original size.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Bitonic Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Bitonic Sequence Definition:</strong> A sequence is bitonic if it first monotonically increases then monotonically decreases,
/// or can be circularly rotated to achieve this property. For example: [3,7,9,5,2,1] is bitonic.</description></item>
/// <item><description><strong>Recursive Bitonic Construction:</strong> Divide the input into two halves. Recursively sort the first half in ascending order
/// and the second half in descending order. The concatenation of these two sorted subsequences forms a bitonic sequence.
/// This is because the first half increases (a₁ ≤ a₂ ≤ ... ≤ aₖ) and the second half decreases (bₖ ≥ ... ≥ b₂ ≥ b₁),
/// creating the pattern: increasing then decreasing.</description></item>
/// <item><description><strong>Bitonic Merge Correctness:</strong> Given a bitonic sequence of length n = 2k, compare and conditionally swap elements
/// at distance k apart (i.e., compare element[i] with element[i+k] for i ∈ [0, k)). This operation, called a bitonic split,
/// partitions the sequence into two bitonic subsequences of length k each, where all elements in the first half are ≤ all elements
/// in the second half. Recursively applying bitonic merge to each half produces a fully sorted sequence.</description></item>
/// <item><description><strong>Power-of-Two Requirement:</strong> The classic bitonic sort requires input length to be a power of 2 (n = 2^m) to maintain
/// balanced recursive splitting. This implementation handles arbitrary lengths by padding to the next power of 2.</description></item>
/// <item><description><strong>Padding Strategy for Non-Power-of-2:</strong> When input length n is not a power of 2:
/// <list type="bullet">
/// <item>Find the maximum value max in the input array.</item>
/// <item>Pad the array to the next power of 2 (2^⌈log₂ n⌉) by appending max values.</item>
/// <item>Sort the padded array using bitonic sort.</item>
/// <item>Since max is the largest value, all padding elements will be sorted to the end of the array.</item>
/// <item>Trim the result back to the original length n, discarding the padding elements.</item>
/// </list>
/// This preserves correctness because padding with max ensures that the original n elements maintain their relative order
/// and are not affected by the padding values during comparison-based sorting.</description></item>
/// <item><description><strong>Comparison Network Property:</strong> Bitonic sort is a comparison network, meaning the sequence of comparisons
/// is data-independent. The same comparisons are performed regardless of input values, making it ideal for parallel hardware
/// implementations and SIMD optimizations.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Network Sort / Exchange</description></item>
/// <item><description>Stable      : No (swapping non-adjacent elements can change relative order of equal elements)</description></item>
/// <item><description>In-place    : Yes for power-of-2 sizes (O(1) auxiliary space); O(n) auxiliary space for non-power-of-2 due to padding array</description></item>
/// <item><description>Best case   : Θ(n log² n) - Data-independent comparison count (same as worst case)</description></item>
/// <item><description>Average case: Θ(n log² n) - Performs the same comparisons for all inputs</description></item>
/// <item><description>Worst case  : Θ(n log² n) - Comparison count: ½ × n × log² n for power-of-2 inputs</description></item>
/// <item><description>Comparisons : Θ(n log² n) - Exactly (log² n × (log n + 1)) / 4 × n comparisons for n = 2^k</description></item>
/// <item><description>Swaps       : O(n log² n) - At most equal to comparison count; depends on input disorder</description></item>
/// <item><description>Parallel depth: O(log² n) - Network depth allows O(log² n) parallel time with O(n log n) processors</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>Power-of-2 inputs (n = 2^k): Sorted in-place with zero additional heap allocation.</description></item>
/// <item><description>Non-power-of-2 inputs: Creates a temporary array of size 2^⌈log₂ n⌉ for padding, then copies back n elements.</description></item>
/// <item><description>Sequential implementation: Does not exploit parallelism (comparisons are executed sequentially).</description></item>
/// <item><description>Uses aggressive inlining for performance-critical helper methods (CompareAndSwap, IsPowerOfTwo, etc.).</description></item>
/// </list>
/// <para><strong>Use Cases:</strong></para>
/// <list type="bullet">
/// <item><description>Parallel sorting on GPUs or multi-core systems (when parallelized via SIMD or threads)</description></item>
/// <item><description>Hardware sorting networks (FPGA, ASIC implementations)</description></item>
/// <item><description>Educational purposes to demonstrate oblivious sorting algorithms</description></item>
/// <item><description>Scenarios where predictable, data-independent execution time is required</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Bitonic_sorter</para>
/// </remarks>
public static class BitonicSortFill
{
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
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}\"/>.</typeparam>
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

        int originalLength = span.Length;

        // If length is already a power of 2, sort directly
        if (IsPowerOfTwo(originalLength))
        {
            var sortSpan = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
            SortCore(sortSpan, 0, originalLength, ascending: true);
            return;
        }

        // Calculate next power of 2
        int paddedLength = NextPowerOfTwo(originalLength);

        // Find the maximum value in the input for padding
        // Create SortSpan for statistics tracking during max value search
        var tempSortSpan = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        T maxValue = tempSortSpan.Read(0);
        for (int i = 1; i < originalLength; i++)
        {
            if (tempSortSpan.Compare(i, maxValue) > 0)
            {
                maxValue = tempSortSpan.Read(i);
            }
        }

        // Create temporary array with padding
        var workingArray = ArrayPool<T>.Shared.Rent(paddedLength);
        Span<T> workingSpan = workingArray.AsSpan();

        try
        {
            // Copy original data
            span.CopyTo(workingSpan);

            // Pad with max value (will be sorted to the end)
            for (int i = originalLength; i < paddedLength; i++)
            {
                workingSpan[i] = maxValue;
            }

            // Sort the padded array
            var paddedSortSpan = new SortSpan<T, TComparer, TContext>(workingSpan, context, comparer, BUFFER_MAIN);
            SortCore(paddedSortSpan, 0, paddedLength, ascending: true);

            // Copy back only the original elements
            workingSpan.Slice(0, originalLength).CopyTo(span);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(workingArray);
        }
    }

    /// <summary>
    /// Recursively builds and merges a bitonic sequence.
    /// </summary>
    /// <param name="span">The span to sort.</param>
    /// <param name="low">The starting index of the sequence.</param>
    /// <param name="count">The length of the sequence.</param>
    /// <param name="ascending">True to sort in ascending order, false for descending.</param>
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

    /// <summary>
    /// Calculates the next power of 2 greater than or equal to n.
    /// </summary>
    /// <param name="n">The input number.</param>
    /// <returns>The next power of 2.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int NextPowerOfTwo(int n)
    {
        if (n <= 0) return 1;

        n--;
        n |= n >> 1;
        n |= n >> 2;
        n |= n >> 4;
        n |= n >> 8;
        n |= n >> 16;
        return n + 1;
    }
}
