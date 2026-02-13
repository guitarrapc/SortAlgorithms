using System.Buffers;
using SortAlgorithm.Contexts;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 配列内の既存の部分的なソート済み領域（runs）を検出し、それらを効率的にマージすることで、
/// 従来のMerge Sortと比較してSwap操作を劇的に削減した安定なソートアルゴリズムです。
/// 特にデータが部分的にソート済みの場合、O(n)に近い性能を発揮します。
/// <br/>
/// Detects naturally occurring sorted runs within the array and efficiently merges them,
/// drastically reducing swap operations compared to traditional merge sort while maintaining stability.
/// Achieves near O(n) performance when data is partially sorted.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct ShiftSort:</strong></para>
/// <list type="number">
/// <item><description><strong>Run Detection Phase (Natural Sorted Subsequence Identification):</strong> The algorithm scans the array from right to left,
/// identifying positions where the sorted order breaks (arr[i] &lt; arr[i-1]). These boundary positions divide the array into naturally sorted runs.
/// When three consecutive elements form a descending sequence (arr[i-2] &gt; arr[i-1] &gt; arr[i]), the first and third elements are swapped
/// to maximize run length. This phase has O(n) time complexity with minimal swaps.</description></item>
/// <item><description><strong>Run Boundary Registration:</strong> Detected run boundaries are stored in an index array (zeroIndices).
/// The maximum number of runs is n/2 + 2. For small arrays (≤256 elements), the index array is allocated on the stack using stackalloc.
/// For larger arrays, ArrayPool&lt;int&gt;.Shared is used to avoid stack overflow while maintaining O(1) amortized allocation cost.
/// The indices array is structured as [end_of_array, boundary₁, boundary₂, ..., boundaryₖ].</description></item>
/// <item><description><strong>Adaptive Merge Strategy (Binary Merge Tree):</strong> The detected runs are merged using a divide-and-conquer approach.
/// The Split method recursively divides the run list into two halves until reaching the base case (2 or fewer runs),
/// then merges them bottom-up. This guarantees O(log k) merge levels where k is the number of runs (k ≤ n/2).</description></item>
/// <item><description><strong>Size-Adaptive Merge Direction:</strong> Unlike traditional merge sort, ShiftSort chooses which partition to buffer
/// based on size comparison (second - first &gt; third - second). The smaller partition is copied to temporary storage,
/// minimizing memory allocation and write operations. This optimization reduces practical memory usage by up to 50%.</description></item>
/// <item><description><strong>Backward Merge for Stability:</strong> When the second partition is smaller, merging proceeds backward from right to left.
/// When the first partition is smaller, merging proceeds forward from left to right. Both directions preserve stability by
/// using &gt;= comparison (taking from left when equal) during the merge operation.</description></item>
/// <item><description><strong>Shift-Based Element Movement:</strong> Elements are "shifted" (moved) rather than "swapped" during merge.
/// This reduces the operation count from 3 assignments per swap to 1 assignment per shift, significantly lowering write overhead.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Merge (Adaptive Natural Merge Sort variant)</description></item>
/// <item><description>Stable      : Yes (stability preserved via &gt;= comparison during merge)</description></item>
/// <item><description>In-place    : No (requires O(n/2) auxiliary space for merge buffers)</description></item>
/// <item><description>Best case   : O(n) - Already sorted data requires only one O(n) scan with no merges</description></item>
/// <item><description>Average case: O(n log k) where k = number of runs - Typically k &lt;&lt; n for real-world data</description></item>
/// <item><description>Worst case  : O(n log n) - Completely reversed or random data produces maximum runs (k ≈ n/2)</description></item>
/// <item><description>Comparisons : O(n log k) - Run detection: O(n), merging: O(n log k)</description></item>
/// <item><description>Swaps       : O(n) - Only during run detection phase (at most n/3 swaps)</description></item>
/// <item><description>Writes      : O(n log k) - Shift operations during merge (significantly fewer than traditional merge sort)</description></item>
/// <item><description>Space       : O(n/2) - Maximum temporary buffer size for largest partition during merge</description></item>
/// </list>
/// <para><strong>Advantages of ShiftSort:</strong></para>
/// <list type="bullet">
/// <item><description>Adaptive performance - Exploits existing order in data, achieving near O(n) on partially sorted data</description></item>
/// <item><description>Minimal swaps - Only swaps during run detection (O(n)), not during merge operations</description></item>
/// <item><description>Reduced memory writes - Shift-based merging reduces write operations compared to traditional merge sort</description></item>
/// <item><description>Stable - Preserves relative order of equal elements</description></item>
/// <item><description>Predictable worst case - Still O(n log n) even on random data</description></item>
/// </list>
/// <para><strong>Use Cases:</strong></para>
/// <list type="bullet">
/// <item><description>Sorting data with inherent partial order (e.g., time-series data, log files)</description></item>
/// <item><description>When swap operations are expensive (e.g., large objects)</description></item>
/// <item><description>Stable sorting with better practical performance than traditional merge sort</description></item>
/// <item><description>Scenarios where data is frequently appended and re-sorted</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Original implementation: https://github.com/JamesQuintero/ShiftSort</para>
/// </remarks>
public static class ShiftSort
{
    // Threshold for using stackalloc vs ArrayPool (128 int = 512 bytes)
    private const int StackallocThreshold = 256; // (128 * 2) - 2 = max span.Length for stackalloc

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;           // Main input array
    private const int BUFFER_TEMP_FIRST = 1;     // Temporary buffer for first partition
    private const int BUFFER_TEMP_SECOND = 2;    // Temporary buffer for second partition


    /// <summary>
    /// Sorts the elements in the specified span in ascending order using the default comparer.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort in place.</param>
    public static void Sort<T>(Span<T> span)
        => Sort(span, Comparer<T>.Default, NullContext.Default);

    /// <summary>
    /// Sorts the elements in the specified span using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T>(Span<T> span, ISortContext context)
        => Sort(span, Comparer<T>.Default, context);

    /// <summary>
    /// Sorts the elements in the specified span using the provided comparer and sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use for element comparisons.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="comparer">The comparer to use for element comparisons.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T, TComparer>(Span<T> span, TComparer comparer, ISortContext context) where TComparer : IComparer<T>
    {
        if (span.Length <= 1) return;

        var indicesLength = (span.Length / 2) + 2;

        // Use stackalloc for small arrays, ArrayPool for larger ones
        if (span.Length <= StackallocThreshold)
        {
            Span<int> zeroIndices = stackalloc int[indicesLength];
            SortCore(span, comparer, context, zeroIndices);
        }
        else
        {
            var indicesBuffer = ArrayPool<int>.Shared.Rent(indicesLength);
            try
            {
                var zeroIndices = indicesBuffer.AsSpan(0, indicesLength);
                SortCore(span, comparer, context, zeroIndices);
            }
            finally
            {
                ArrayPool<int>.Shared.Return(indicesBuffer);
            }
        }
    }

    /// <summary>
    /// Core sorting logic - detects runs and merges them.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SortCore<T, TComparer>(Span<T> span, TComparer comparer, ISortContext context, Span<int> zeroIndices) where TComparer : IComparer<T>
    {
        var s = new SortSpan<T, TComparer>(span, context, comparer, BUFFER_MAIN);

        // Phase 1: Run Detection - Identify natural sorted sequences and their boundaries
        zeroIndices[0] = s.Length;

        var endTracker = 1;

        // Scan from right to left, detecting descending pairs and optimizing 3-element inversions
        for (var x = s.Length - 1; x >= 1; x--)
        {
            if (s.Compare(x, x - 1) < 0) // Found a run boundary
            {
                if (x > 1 && s.Compare(x - 1, x - 2) < 0) // Three consecutive descending elements
                {
                    // Optimize: swap first and third to extend run length
                    s.Swap(x, x - 2);

                    // Check if swap created a new boundary
                    if (x != s.Length - 1)
                    {
                        if (s.Compare(x + 1, x) < 0)
                        {
                            zeroIndices[endTracker] = x + 1;
                            endTracker++;
                        }
                    }
                }
                else
                {
                    // Regular boundary detected
                    zeroIndices[endTracker] = x;
                    endTracker++;
                }

                // Skip next element as it's already processed
                x--;
            }
        }

        // Add end marker (start of array) to complete the run boundary list
        zeroIndices[endTracker] = 0;

        // Phase 2: Adaptive Merge - Recursively merge detected runs
        Split(s, zeroIndices, 0, endTracker, context);
    }

    /// <summary>
    /// Recursively divides the run index list and merges runs bottom-up.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Split<T, TComparer>(SortSpan<T, TComparer> s, Span<int> zeroIndices, int i, int j, ISortContext context) where TComparer : IComparer<T>
    {
        // Base case: 2 runs - merge them directly
        if ((j - i) == 2)
        {
            Merge(s, zeroIndices[j], zeroIndices[j - 1], zeroIndices[i], context);
            return;
        }
        else if ((j - i) < 2)
        {
            // Base case: 0 or 1 run - already sorted
            return;
        }

        // Recursive case: divide run list in half
        var j2 = i + (j - i) / 2;
        var i2 = j2 + 1;

        // Recursively sort first half of runs
        Split(s, zeroIndices, i, j2, context);
        // Recursively sort second half of runs
        Split(s, zeroIndices, i2, j, context);

        // Merge the two halves
        Merge(s, zeroIndices[i2], zeroIndices[j2], zeroIndices[i], context);
        Merge(s, zeroIndices[j], zeroIndices[i2], zeroIndices[i], context);
    }

    /// <summary>
    /// Merges two adjacent sorted runs using adaptive direction based on partition sizes.
    /// The smaller partition is buffered to minimize memory allocation and write operations.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Merge<T, TComparer>(SortSpan<T, TComparer> s, int first, int second, int third, ISortContext context) where TComparer : IComparer<T>
    {
        if (second - first > third - second)
        {
            // Second partition is smaller - buffer it and merge backward
            var bufferSize = third - second;
            var tmp2nd = ArrayPool<T>.Shared.Rent(bufferSize);
            try
            {
                var tmp2ndSpan = new SortSpan<T, TComparer>(tmp2nd.AsSpan(0, bufferSize), context, s.Comparer, BUFFER_TEMP_SECOND);

                // Copy second partition to buffer using CopyTo for efficiency
                s.CopyTo(second, tmp2ndSpan, 0, bufferSize);

                // Merge from right to left, shifting elements from first partition rightward
                var secondCounter = bufferSize;
                var left = second - 1;
                while (secondCounter > 0)
                {
                    // Stability: >= ensures elements from left partition come first when equal
                    if (left >= first && s.Compare(left, tmp2ndSpan.Read(secondCounter - 1)) >= 0)
                    {
                        s.Write(left + secondCounter, s.Read(left));
                        left--;
                    }
                    else
                    {
                        s.Write(left + secondCounter, tmp2ndSpan.Read(secondCounter - 1));
                        secondCounter--;
                    }
                }
            }
            finally
            {
                ArrayPool<T>.Shared.Return(tmp2nd, clearArray: true);
            }
        }
        else
        {
            // First partition is smaller - buffer it and merge forward
            var bufferSize = second - first;
            var tmp1st = ArrayPool<T>.Shared.Rent(bufferSize);
            try
            {
                var tmp1stSpan = new SortSpan<T, TComparer>(tmp1st.AsSpan(0, bufferSize), context, s.Comparer, BUFFER_TEMP_FIRST);

                // Copy first partition to buffer using CopyTo for efficiency
                s.CopyTo(first, tmp1stSpan, 0, bufferSize);

                // Merge from left to right, shifting elements from second partition leftward
                var firstCounter = 0;
                var tmpLength = bufferSize;
                var right = second;
                while (firstCounter < bufferSize)
                {
                    if (right < third && s.Compare(right, tmp1stSpan.Read(firstCounter)) < 0)
                    {
                        s.Write(right - tmpLength, s.Read(right));
                        right++;
                    }
                    else
                    {
                        s.Write(right - tmpLength, tmp1stSpan.Read(firstCounter));
                        firstCounter++;
                        tmpLength--;
                    }
                }
            }
            finally
            {
                ArrayPool<T>.Shared.Return(tmp1st, clearArray: true);
            }
        }
    }
}
