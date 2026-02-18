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
/// <item><description><strong>Run Detection Phase (Natural Sorted Subsequence Identification):</strong> The algorithm scans the array from left to right,
/// extending each run as far as possible in one direction. A non-descending run grows while arr[i+1] &gt;= arr[i];
/// a strictly descending run grows while arr[i+1] &lt; arr[i] and is then reversed in-place to produce an ascending run.
/// This approach captures the longest possible natural runs (both ascending and descending) and has O(n) time complexity
/// with at most n/2 swaps (one pass of in-place reversal).</description></item>
/// <item><description><strong>Run Boundary Registration:</strong> Detected run boundaries are stored in an ascending index array (zeroIndices) as [0, b₁, b₂, …, bₖ, n].
/// Each adjacent pair (zeroIndices[m], zeroIndices[m+1]) defines one sorted run. The maximum number of runs is ceil(n/2),
/// so the array needs at most n/2 + 2 entries. For small arrays (≤256 elements) the index array is allocated on the stack
/// using stackalloc; for larger arrays ArrayPool&lt;int&gt;.Shared is used to avoid stack overflow.</description></item>
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
/// <item><description>Swaps       : O(n/2) worst case - only during run detection phase for in-place reversal of descending runs</description></item>
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
    /// Sorts the elements in the specified span using the provided comparer and context.
    /// This is the full-control version with explicit TContext type parameter.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use for element comparisons.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="comparer">The comparer to use for element comparisons.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation.</param>
    public static void Sort<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
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
    private static void SortCore<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context, Span<int> zeroIndices)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);

        // Phase 1: Natural Run Detection - scan left to right
        // Extends each run as far as possible: non-descending runs grow as-is,
        // strictly descending runs are reversed in-place to produce ascending runs.
        // Builds the ascending boundary sequence directly: [0, b₁, …, bₖ, n]
        var endTracker = 0;
        zeroIndices[endTracker++] = 0;

        var i = 0;
        while (i < s.Length - 1)
        {
            var runStart = i;
            if (s.Compare(i + 1, i) < 0) // strictly descending run: extend then reverse
            {
                i++; // consume the outer comparison's pair
                while (i < s.Length - 1 && s.Compare(i + 1, i) < 0)
                    i++;
                ReverseRun(s, runStart, i);
            }
            else // non-descending run: extend
            {
                i++; // consume the outer comparison's pair
                while (i < s.Length - 1 && s.Compare(i + 1, i) >= 0)
                    i++;
            }

            i++;
            if (i < s.Length)
                zeroIndices[endTracker++] = i;
        }

        zeroIndices[endTracker] = s.Length;

        // Phase 2: Adaptive Merge - Recursively merge detected runs
        Split(s, zeroIndices, 0, endTracker);
    }

    /// <summary>
    /// Reverses the run between indices <paramref name="lo"/> and <paramref name="hi"/> (inclusive) in-place.
    /// Used to convert a strictly descending run into ascending order during run detection.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ReverseRun<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int lo, int hi)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (lo < hi)
        {
            s.Swap(lo, hi);
            lo++;
            hi--;
        }
    }

    /// <summary>
    /// Recursively divides the boundary range [lo, hi] and merges sorted runs bottom-up.
    /// zeroIndices is an ascending boundary sequence where zeroIndices[lo] is the inclusive start
    /// and zeroIndices[hi] is the exclusive end of the region being sorted.
    /// Each adjacent pair (zeroIndices[k], zeroIndices[k+1]) represents one sorted run.
    /// </summary>
    private static void Split<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, Span<int> zeroIndices, int lo, int hi)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Base case: 2 runs - merge them directly
        if ((hi - lo) == 2)
        {
            Merge(s, zeroIndices[lo], zeroIndices[lo + 1], zeroIndices[hi]);
            return;
        }
        else if ((hi - lo) < 2)
        {
            // Base case: 0 or 1 run - already sorted
            return;
        }

        // Recursive case: split at midpoint boundary
        var mid = lo + (hi - lo) / 2;

        // Recursively sort left half: [zeroIndices[lo], zeroIndices[mid])
        Split(s, zeroIndices, lo, mid);
        // Recursively sort right half: [zeroIndices[mid], zeroIndices[hi])
        Split(s, zeroIndices, mid, hi);

        // Merge the two sorted halves into [zeroIndices[lo], zeroIndices[hi])
        Merge(s, zeroIndices[lo], zeroIndices[mid], zeroIndices[hi]);
    }

    /// <summary>
    /// Merges two adjacent sorted runs using adaptive direction based on partition sizes.
    /// The smaller partition is buffered to minimize memory allocation and write operations.
    /// </summary>
    private static void Merge<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int second, int third)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (second - first > third - second)
        {
            // Second partition is smaller - buffer it and merge backward
            var bufferSize = third - second;
            var tmp2nd = ArrayPool<T>.Shared.Rent(bufferSize);
            try
            {
                var tmp2ndSpan = new SortSpan<T, TComparer, TContext>(tmp2nd.AsSpan(0, bufferSize), s.Context, s.Comparer, BUFFER_TEMP_SECOND);

                // Copy second partition to buffer using CopyTo for efficiency
                s.CopyTo(second, tmp2ndSpan, 0, bufferSize);

                // Merge from right to left (backward merge)
                // Layout: [first .. second-1][second .. third-1]
                //         |<--- Left run --->||<-- Right run -->|
                // Right run is buffered as tmp2nd[0..bufferSize-1]
                // Write position: left + secondCounter (decreases from third-1 to first)
                //
                // Stability condition:
                //   When Compare(left_elem, right_elem) == 0:
                //     - Use '>' (not '>=') to force else branch
                //     - else branch writes right_elem first (to higher position)
                //     - left_elem is written later (to lower position)
                //     => left_elem appears before right_elem in final output ✓
                //
                // Proof:
                //   Let left_elem = s[left], right_elem = tmp2nd[secondCounter-1]
                //   Case A: left_elem > right_elem  => write left_elem to writePos, left--
                //   Case B: left_elem == right_elem => write right_elem to writePos, secondCounter--
                //           Next iteration writes left_elem to writePos-1
                //           => left_elem (originally at lower index) is placed before right_elem ✓
                //   Case C: left_elem < right_elem  => write right_elem to writePos, secondCounter--
                var secondCounter = bufferSize;
                var left = second - 1;
                while (secondCounter > 0)
                {
                    // Stability: use '>' (not '>=') to ensure left < right in final output when equal
                    if (left >= first && s.Compare(left, tmp2ndSpan.Read(secondCounter - 1)) > 0)
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
                ArrayPool<T>.Shared.Return(tmp2nd, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
            }
        }
        else
        {
            // First partition is smaller - buffer it and merge forward
            var bufferSize = second - first;
            var tmp1st = ArrayPool<T>.Shared.Rent(bufferSize);
            try
            {
                var tmp1stSpan = new SortSpan<T, TComparer, TContext>(tmp1st.AsSpan(0, bufferSize), s.Context, s.Comparer, BUFFER_TEMP_FIRST);

                // Copy first partition to buffer using CopyTo for efficiency
                s.CopyTo(first, tmp1stSpan, 0, bufferSize);

                // Merge from left to right (forward merge)
                // Layout: [first .. second-1][second .. third-1]
                //         |<--- Left run --->||<-- Right run -->|
                // Left run is buffered as tmp1st[0..bufferSize-1]
                // Write position: starts at 'first', increments by 1 each iteration
                //
                // Stability condition:
                //   When Compare(right_elem, left_elem) == 0:
                //     - Use '<' (not '<=') to force else branch
                //     - else branch writes left_elem (from buffer)
                //     - left_elem (originally at lower index) is written before right_elem
                //     => left_elem appears before right_elem in final output ✓
                //
                // Proof:
                //   Let left_elem = tmp1st[firstCounter], right_elem = s[right]
                //   Write position = first + (firstCounter + (right - second))
                //                  = first + (total_elements_written)
                //   Case A: right_elem < left_elem  => write right_elem to writePos, right++
                //   Case B: right_elem == left_elem => write left_elem to writePos, firstCounter++
                //           Next iteration writes right_elem to writePos+1
                //           => left_elem (originally at lower index) is placed before right_elem ✓
                //   Case C: right_elem > left_elem  => write left_elem to writePos, firstCounter++
                var firstCounter = 0;
                var right = second;
                var writePos = first;
                while (firstCounter < bufferSize)
                {
                    // Stability: use '<' (not '<=') to ensure left < right in final output when equal
                    if (right < third && s.Compare(right, tmp1stSpan.Read(firstCounter)) < 0)
                    {
                        s.Write(writePos, s.Read(right));
                        right++;
                    }
                    else
                    {
                        s.Write(writePos, tmp1stSpan.Read(firstCounter));
                        firstCounter++;
                    }
                    writePos++;
                }
            }
            finally
            {
                ArrayPool<T>.Shared.Return(tmp1st, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
            }
        }
    }
}
