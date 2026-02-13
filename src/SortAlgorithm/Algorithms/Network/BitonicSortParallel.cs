using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// バイトニックソート（並列版・2のべき乗専用） - バイトニック列を構築し、並列実行で再帰的にマージして整列列に変換するソーティングネットワークアルゴリズムです。
/// 入力サイズは2のべき乗（2^n）でなければなりません。対象の要素が大きい場合、並列実行を活用します。
/// <br/>
/// Bitonic Sort (Parallel, Power-of-2 Only) - A sorting network algorithm that builds a bitonic sequence and merges it using parallel divide-and-conquer.
/// Input length must be a power of 2 (2^n). Leverages parallel execution for large datasets.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Bitonic Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Bitonic Sequence Definition:</strong> A sequence is bitonic if it first monotonically increases then monotonically decreases,
/// or can be circularly rotated to achieve this property.</description></item>
/// <item><description><strong>Power-of-Two Requirement:</strong> The input length must be a power of 2 (n = 2^m for some integer m ≥ 0).
/// This ensures the divide-and-conquer structure maintains balanced splits at each recursive level.</description></item>
/// <item><description><strong>Recursive Bitonic Construction:</strong> Divide the input into two halves. Recursively sort the first half in ascending order
/// and the second half in descending order to form a bitonic sequence.</description></item>
/// <item><description><strong>Parallel Divide-and-Conquer:</strong> Uses Parallel.Invoke to process left and right halves concurrently
/// in both the sort phase and merge phase, maximizing parallelism at the recursion level.</description></item>
/// <item><description><strong>Sequential Compare-and-Swap:</strong> Individual comparison operations within BitonicMerge are executed sequentially
/// to avoid excessive thread creation overhead, as each comparison is a lightweight operation.</description></item>
/// <item><description><strong>Thread Safety:</strong> Since Parallel cannot capture ref struct (Span/SortSpan), this implementation accepts T[] array
/// and creates SortSpan instances at appropriate granularity for statistics tracking.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Network Sort / Exchange</description></item>
/// <item><description>Stable      : No (swapping non-adjacent elements can change relative order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space - sorts directly on input array)</description></item>
/// <item><description>Sequential time: Θ(n log² n) - Same comparison count as non-parallel version</description></item>
/// <item><description>Parallel time: O(log³ n) - Theoretical parallel depth with O(n) processors</description></item>
/// <item><description>Comparisons : Θ(n log² n) - Exactly (log² n × (log n + 1)) / 4 × n comparisons for n = 2^k</description></item>
/// <item><description>Parallelism : High - Divide-and-conquer recursion is parallelized, not individual comparisons</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>Accepts T[] array instead of Span due to Parallel.Invoke limitation with ref struct</description></item>
/// <item><description>Uses Parallel.Invoke for recursive divide-and-conquer (threshold: 256 elements minimum for parallelization)</description></item>
/// <item><description>Compare-and-swap loops are executed sequentially to minimize thread overhead</description></item>
/// <item><description>Creates SortSpan instances at merge level (not per-comparison) for efficient statistics tracking</description></item>
/// <item><description>Automatically detects WebAssembly and single-core environments: falls back to sequential execution</description></item>
/// <item><description>Strictly requires power-of-2 input length. Throws ArgumentException otherwise.</description></item>
/// </list>
/// <para><strong>Use Cases:</strong></para>
/// <list type="bullet">
/// <item><description>Large datasets (≥ 1024 elements) on multi-core systems</description></item>
/// <item><description>Scenarios where predictable parallel performance is required</description></item>
/// <item><description>When input size is guaranteed to be a power of 2</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Bitonic_sorter</para>
/// </remarks>
public static class BitonicSortParallel
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array

    // Threshold for parallelization - below this size, use sequential sorting
    // This threshold is for parallelizing recursive divide-and-conquer, not individual comparisons
    // Empirical testing on 32-core system shows:
    // - With recursive-only parallelization: lower thresholds (128-256) work better
    // - Below threshold: sequential execution to avoid thread creation overhead
    private const int PARALLEL_THRESHOLD = 256;

    // Detect if parallel execution is actually beneficial
    // WebAssembly and single-core systems should use sequential execution
    private static readonly bool _useParallelExecution = Environment.ProcessorCount > 1 && !IsWebAssembly();

    // Parallel options with max degree of parallelism set to number of processors
    private static readonly ParallelOptions parallelOptions = new ParallelOptions
    {
        MaxDegreeOfParallelism = Environment.ProcessorCount,
    };

    /// <summary>
    /// Detects if running in a WebAssembly environment.
    /// WebAssembly is single-threaded and Parallel.Invoke executes sequentially.
    /// </summary>
    private static bool IsWebAssembly()
    {
        // Check for WebAssembly runtime
        // In Blazor WebAssembly, RuntimeInformation.OSDescription contains "Browser"
        try
        {
            return System.Runtime.InteropServices.RuntimeInformation.OSDescription.Contains("Browser", StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Sorts the elements in the specified array in ascending order using the default comparer.
    /// Uses parallel execution for improved performance on multi-core systems.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="array">The array of elements to sort in place.</param>
    /// <exception cref="ArgumentException">Thrown when the array length is not a power of 2.</exception>
    public static void Sort<T>(T[] array)
    {
        Sort(array, NullContext.Default);
    }

    /// <summary>
    /// Sorts the elements in the specified array using the provided sort context and parallel execution.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="array">The array of elements to sort. The elements within this array will be reordered in place.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    /// <exception cref="ArgumentException">Thrown when the array length is not a power of 2.</exception>
    public static void Sort<T>(T[] array, ISortContext context)
        => Sort(array, Comparer<T>.Default, context);

    /// <summary>
    /// Sorts the elements in the specified array using the provided comparer, sort context, and parallel execution.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use for element comparisons.</typeparam>
    /// <param name="array">The array of elements to sort. The elements within this array will be reordered in place.</param>
    /// <param name="comparer">The comparer to use for element comparisons.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    /// <exception cref="ArgumentException">Thrown when the array length is not a power of 2.</exception>
    public static void Sort<T, TComparer>(T[] array, TComparer comparer, ISortContext context) where TComparer : IComparer<T>
    {
        ArgumentNullException.ThrowIfNull(array);
        ArgumentNullException.ThrowIfNull(context);

        if (array.Length <= 1) return;

        // Verify that length is a power of 2
        if (!IsPowerOfTwo(array.Length))
            throw new ArgumentException($"Bitonic sort requires input length to be a power of 2. Actual length: {array.Length}", nameof(array));

        SortCore(array, 0, array.Length, ascending: true, comparer, context);
    }

    /// <summary>
    /// Recursively builds and merges a bitonic sequence with parallel execution.
    /// </summary>
    /// <param name="array">The array to sort.</param>
    /// <param name="low">The starting index of the sequence.</param>
    /// <param name="count">The length of the sequence.</param>
    /// <param name="ascending">True to sort in ascending order, false for descending.</param>
    /// <param name="context">The sort context for statistics tracking.</param>
    internal static void SortCore<T, TComparer>(T[] array, int low, int count, bool ascending, TComparer comparer, ISortContext context) where TComparer : IComparer<T>
    {
        if (count > 1)
        {
            int k = count / 2;

            // For large enough sequences, use parallel tasks for left and right halves
            // Only parallelize if running on multi-core non-WebAssembly environment
            if (_useParallelExecution && count >= PARALLEL_THRESHOLD)
            {
                Parallel.Invoke(
                    parallelOptions,
                    () => SortCore(array, low, k, ascending: true, comparer, context),
                    () => SortCore(array, low + k, k, ascending: false, comparer, context)
                );
            }
            else
            {
                // Recursively sort first half in ascending order
                SortCore(array, low, k, ascending: true, comparer, context);
                // Recursively sort second half in descending order to create bitonic sequence
                SortCore(array, low + k, k, ascending: false, comparer, context);
            }

            // Merge the bitonic sequence in the desired order
            BitonicMerge(array, low, count, ascending, comparer, context);
        }
    }

    /// <summary>
    /// Merges a bitonic sequence into a sorted sequence with parallel execution.
    /// </summary>
    /// <param name="array">The array containing the bitonic sequence.</param>
    /// <param name="low">The starting index of the bitonic sequence.</param>
    /// <param name="count">The length of the bitonic sequence.</param>
    /// <param name="ascending">True to merge in ascending order, false for descending.</param>
    /// <param name="context">The sort context for statistics tracking.</param>
    private static void BitonicMerge<T, TComparer>(T[] array, int low, int count, bool ascending, TComparer comparer, ISortContext context) where TComparer : IComparer<T>
    {
        if (count > 1)
        {
            int k = count / 2;

            // Perform compare-and-swap sequentially
            // Parallelizing this loop has too much overhead for the lightweight comparison operations
            var span = new SortSpan<T, TComparer>(array.AsSpan(), context, comparer, BUFFER_MAIN);
            for (int i = low; i < low + k; i++)
            {
                CompareAndSwap(span, i, i + k, ascending);
            }

            // Recursively merge both halves - parallelize if large enough
            // Only parallelize if running on multi-core non-WebAssembly environment
            if (_useParallelExecution && count >= PARALLEL_THRESHOLD)
            {
                Parallel.Invoke(
                    parallelOptions,
                    () => BitonicMerge(array, low, k, ascending, comparer, context),
                    () => BitonicMerge(array, low + k, k, ascending, comparer, context)
                );
            }
            else
            {
                BitonicMerge(array, low, k, ascending, comparer, context);
                BitonicMerge(array, low + k, k, ascending, comparer, context);
            }
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
    private static void CompareAndSwap<T, TComparer>(SortSpan<T, TComparer> span, int i, int j, bool ascending) where TComparer : IComparer<T>
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
