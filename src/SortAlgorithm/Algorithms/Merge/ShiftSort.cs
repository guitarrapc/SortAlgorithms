using System.Buffers;
using SortAlgorithm.Contexts;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 配列を後ろから走査し、左隣の要素に対して順序が崩れている位置を導関数リスト（derivative list）に記録します。
/// 記録された位置はすでに昇順になっている部分列（sublist）の開始位置であり、走査中に長さ 3 の厳密降順の窓を見つけた場合は
/// 1 回のスワップで昇順に変えて部分列の数を減らします。その後、配列ではなく導関数リストを分割統治でたどりながら
/// 部分列同士をシフトでマージします。導関数リストが空であれば配列はすでにソート済みで、走査 1 回の O(n) で終了します。
/// 安定ソートです。
/// <br/>
/// Scans the array backwards and records, in a derivative list, the index of every element that is out of order
/// with respect to its left neighbour; those indices are the starts of the already-ascending sublists. A window of
/// three in strictly descending order is turned ascending by a single swap instead of being recorded, which shortens
/// the derivative list. The derivative list — not the array — is then split in half recursively, and the sublists it
/// delimits are merged by shifting. An empty derivative list means the array was already sorted, so the scan is all
/// the work. Stable sort.
/// </summary>
/// <remarks>
/// <para><strong>Algorithm Overview:</strong></para>
/// <para>
/// ShiftSort is similar to merge sort but more selective about what it merges. Merge sort splits the array in half
/// until it reaches a base case of 2 elements and merges as it returns; ShiftSort splits a derivative array instead,
/// uses the result to decide which parts of the array to merge, and then merges as it returns. Splitting the derivative
/// list rather than the array is what makes the algorithm adaptive: the shorter the derivative list, the less recursion
/// and merging there is.
/// </para>
/// <para><strong>Core Algorithm Steps:</strong></para>
/// <list type="number">
/// <item><description><strong>Derivative List Creation:</strong> The scan runs from the last index down to the first.
/// Where arr[x] &lt; arr[x-1], the element at x is out of order and therefore starts a sorted sublist. Before recording it,
/// the scan checks arr[x-1] &lt; arr[x-2]: if that also holds, arr[x-2..x] is a strictly descending window of three, and one
/// swap of arr[x-2] and arr[x] turns it ascending. That swap can fuse the window into the sublist on its right, so a boundary
/// is then recorded only at x+1 and only if arr[x+1] &lt; arr[x] still holds after the swap. Either branch consumes two indices,
/// so the scan is O(n) and performs at most n/2 swaps. The window length of three is not a tunable cutoff: it is exactly what
/// the two comparisons the scan already performs can establish.</description></item>
/// <item><description><strong>Derivative List Registration:</strong> The scan produces the boundaries in descending order,
/// [n, bₖ, …, b₁, 0]; they are reversed once into the ascending form [0, b₁, …, bₖ, n] that the split expects.
/// Each adjacent pair (zeroIndices[m], zeroIndices[m+1]) delimits one sorted sublist. Because each recorded boundary consumes
/// two indices, the maximum number of sublists is ⌈n/2⌉ and the array needs at most n/2 + 2 entries. For small arrays
/// (≤256 elements) it is allocated on the stack using stackalloc; for larger arrays ArrayPool&lt;int&gt;.Shared is used
/// to avoid stack overflow.</description></item>
/// <item><description><strong>Splitting of the Derivative List:</strong> The derivative list is divided in half recursively
/// until a half delimits fewer than 2 sublists and needs no merge, then the sublists are merged bottom-up.
/// This gives O(log k) merge levels and exactly k-1 merges for k sublists (k ≤ ⌈n/2⌉).</description></item>
/// <item><description><strong>Size-Adaptive Merge Direction:</strong> Unlike traditional merge sort, ShiftSort chooses which partition to buffer
/// based on size comparison (second - first &gt; third - second). The smaller partition is copied to temporary storage,
/// so a merge buffers min(len₁, len₂) elements rather than always buffering the left run. The work buffer is sized once
/// at ⌈n/2⌉ elements and reused by every merge, so the direction choice reduces the elements copied, not the space held.</description></item>
/// <item><description><strong>Stability-Preserving Merge:</strong> When the second partition is smaller, merging proceeds backward from right to left
/// using '&gt;' comparison (not '&gt;=') to ensure left elements are written to lower positions when equal.
/// When the first partition is smaller, merging proceeds forward from left to right using '&lt;' comparison (not '&lt;=')
/// to ensure left elements are written first when equal. Both directions preserve stability by prioritizing left-side elements.</description></item>
/// <item><description><strong>Shift-Based Element Movement:</strong> During merge operations, elements are "shifted" (single assignment) rather than "swapped" (three assignments).
/// This reduces write operations compared to traditional merge sort, particularly benefiting scenarios with expensive write operations
/// or cache-sensitive workloads. Once one side of a merge is exhausted the remainder of the buffered run needs no further comparison,
/// so it is moved as a single range copy rather than one element at a time.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Merge (adaptive, divide and conquer over a derivative list)</description></item>
/// <item><description>Stable      : Yes (the three-element swap only reorders a strictly descending window, and merges break ties toward the left sublist)</description></item>
/// <item><description>In-place    : No (requires the derivative list and a merge list alongside the input)</description></item>
/// <item><description>Best case   : O(n) - an empty derivative list means the array is already sorted, so the scan is all the work</description></item>
/// <item><description>Average case: O(n log n) - the same order as merge sort; a shorter derivative list only lowers the constant</description></item>
/// <item><description>Worst case  : O(n log n) - a reverse-sorted array, whose descending windows yield the maximum ⌈n/2⌉ sublists</description></item>
/// <item><description>Comparisons : O(n log n) - derivative list creation: O(n), merging: O(n log k) for k sublists</description></item>
/// <item><description>Swaps       : O(n) - at most n/2, only the three-element swap during derivative list creation</description></item>
/// <item><description>Index Reads : O(n log n) - every merge level reads each element of the sublists it joins</description></item>
/// <item><description>Index Writes: O(n log n) - shift-based merge operations (fewer than swap-based approaches)</description></item>
/// <item><description>Space       : O(n) - the derivative list and the merge list are each about n/2</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>The merge follows the C++ reference, which breaks ties with '&gt;' and '&lt;'. The Java reference uses '&gt;=' in the
/// backward merge, which is not stable and contradicts the paper's own claim that ShiftSort is stable.</description></item>
/// <item><description>The reference splits with a gap (new_i = new_j + 1), which leaves one sublist unclaimed by either recursive call
/// and needs two merges per node to pick it up. Sharing the boundary needs one, and both shapes perform the same k-1 merges.</description></item>
/// <item><description>Uses ArrayPool&lt;T&gt; for zero-allocation operation on repeated sorts</description></item>
/// <item><description>Stack-allocates the derivative list for small inputs (≤256 elements) to avoid heap pressure</description></item>
/// <item><description>Integrates with SortSpan pattern for comprehensive statistics tracking and visualization</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>https://github.com/JamesQuintero/ShiftSort — see ShiftSort-Analysis.pdf for the algorithm's definition.</para>
/// </remarks>
public static class ShiftSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by ShiftSortTests, which derives from StableSortTestsBase.</remarks>
    public static bool IsStable => true;

    // Longest span whose boundary array is stack-allocated instead of rented.
    // The boundary array holds (length / 2) + 2 ints, so this bound costs at most 130 ints (520 bytes) of stack.
    private const int StackallocThreshold = 256;

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
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
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
        var workBufferLength = (span.Length + 1) / 2;

        // Use stackalloc for small arrays, ArrayPool for larger ones
        // Note: workBuffer cannot use stackalloc as T may be a managed type
        if (span.Length <= StackallocThreshold)
        {
            Span<int> zeroIndices = stackalloc int[indicesLength];
            var workBufferArray = ArrayPool<T>.Shared.Rent(workBufferLength);
            try
            {
                var workBuffer = workBufferArray.AsSpan(0, workBufferLength);
                SortCore(span, comparer, context, zeroIndices, workBuffer);
            }
            finally
            {
                ArrayPool<T>.Shared.Return(workBufferArray, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
            }
        }
        else
        {
            var indicesBuffer = ArrayPool<int>.Shared.Rent(indicesLength);
            var workBufferArray = ArrayPool<T>.Shared.Rent(workBufferLength);
            try
            {
                var zeroIndices = indicesBuffer.AsSpan(0, indicesLength);
                var workBuffer = workBufferArray.AsSpan(0, workBufferLength);
                SortCore(span, comparer, context, zeroIndices, workBuffer);
            }
            finally
            {
                ArrayPool<int>.Shared.Return(indicesBuffer);
                ArrayPool<T>.Shared.Return(workBufferArray, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
            }
        }
    }

    /// <summary>
    /// Core sorting logic - builds the derivative list and merges the sublists it delimits.
    /// </summary>
    private static void SortCore<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context, Span<int> zeroIndices, Span<T> workBuffer)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);

        // Phase 1: Derivative List Creation - scan from the last index down to the first.
        context.OnPhase(SortPhase.MergeRunDetect);
        // An element out of order with its left neighbour starts a sorted sublist, so its index is recorded.
        // Where the two elements to its left continue descending, the window of three is turned ascending by
        // one swap instead; that can fuse the window into the sublist on its right, in which case a boundary
        // is recorded only at x+1 and only if the order is still broken there.
        //
        // The scan produces the boundaries in descending order, [n, bₖ, …, b₁, 0], because it walks backwards.
        // They are reversed once below into the [0, b₁, …, bₖ, n] form the split expects.
        var endTracker = 0;
        zeroIndices[endTracker++] = s.Length;

        for (var x = s.Length - 1; x >= 1; x--)
        {
            if (s.IsLessAt(x, x - 1))
            {
                if (x > 1 && s.IsLessAt(x - 1, x - 2))
                {
                    // s[x-2] > s[x-1] > s[x], so one swap makes the three ascending. The window is strictly
                    // descending, so no two equal elements move past each other and stability is preserved.
                    s.Swap(x - 2, x);

                    // s[x] now holds the largest of the three, so the element to its right may have become
                    // the one out of order.
                    if (x != s.Length - 1 && s.IsLessAt(x + 1, x))
                    {
                        zeroIndices[endTracker++] = x + 1;
                    }
                }
                else
                {
                    zeroIndices[endTracker++] = x;
                }

                // x-1 is in order either way — the else branch tested it, and the swap made it so — so skip it.
                x--;
            }
        }

        zeroIndices[endTracker] = 0;
        zeroIndices[..(endTracker + 1)].Reverse();

        // Phase 2: Splitting of the Derivative List - merge the sublists it delimits
        Split(s, zeroIndices, 0, endTracker, workBuffer);
    }

    /// <summary>
    /// Recursively divides the boundary range [lo, hi] and merges sorted runs bottom-up.
    /// zeroIndices is an ascending boundary sequence where zeroIndices[lo] is the inclusive start
    /// and zeroIndices[hi] is the exclusive end of the region being sorted.
    /// Each adjacent pair (zeroIndices[k], zeroIndices[k+1]) represents one sorted run.
    /// </summary>
    private static void Split<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, Span<int> zeroIndices, int lo, int hi, Span<T> workBuffer)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Base case: 0 or 1 run - already sorted
        if ((hi - lo) < 2)
        {
            return;
        }

        // Split at the midpoint boundary. Two runs give mid == lo + 1, so both recursive calls return
        // immediately and only the merge below runs; that case is not special-cased separately, because
        // a separate base case would emit the merge without the phase and role events announced here.
        var mid = lo + (hi - lo) / 2;

        // Recursively sort left half: [zeroIndices[lo], zeroIndices[mid])
        Split(s, zeroIndices, lo, mid, workBuffer);
        // Recursively sort right half: [zeroIndices[mid], zeroIndices[hi])
        Split(s, zeroIndices, mid, hi, workBuffer);

        // Merge the two sorted halves into [zeroIndices[lo], zeroIndices[hi])
        s.Context.OnPhase(SortPhase.MergeSortMerge, zeroIndices[lo], zeroIndices[mid] - 1, zeroIndices[hi] - 1);
        s.Context.OnRole(zeroIndices[lo], BUFFER_MAIN, RoleType.LeftPointer);
        s.Context.OnRole(zeroIndices[hi] - 1, BUFFER_MAIN, RoleType.RightPointer);
        Merge(s, zeroIndices[lo], zeroIndices[mid], zeroIndices[hi], workBuffer);
        s.Context.OnRole(zeroIndices[lo], BUFFER_MAIN, RoleType.None);
        s.Context.OnRole(zeroIndices[hi] - 1, BUFFER_MAIN, RoleType.None);
    }

    /// <summary>
    /// Merges two adjacent sorted runs using adaptive direction based on partition sizes.
    /// The smaller partition is buffered, so a merge copies min(len₁, len₂) elements.
    /// Uses the provided workBuffer (pre-allocated in SortCore) instead of ArrayPool for zero-allocation merging.
    /// </summary>
    private static void Merge<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int second, int third, Span<T> workBuffer)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (second - first > third - second)
        {
            // Second partition is smaller - buffer it and merge backward
            var bufferSize = third - second;
            var tmp2ndSpan = new SortSpan<T, TComparer, TContext>(workBuffer[..bufferSize], s.Context, s.Comparer, BUFFER_TEMP_SECOND);

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
            while (secondCounter > 0 && left >= first)
            {
                // Stability: use '>' (not '>=') to ensure left < right in final output when equal.
                // The comparison hands back both operands so the write below does not read either a second time.
                if (s.IsGreaterAcross(left, tmp2ndSpan, secondCounter - 1, out var leftValue, out var rightValue))
                {
                    s.Write(left + secondCounter, leftValue);
                    left--;
                }
                else
                {
                    s.Write(left + secondCounter, rightValue);
                    secondCounter--;
                }
            }

            // The left run is exhausted. Every remaining buffer element is smaller than what is already
            // placed above it, so tmp2nd[0..secondCounter) lands contiguously at [first, first + secondCounter)
            // with no further comparison.
            if (secondCounter > 0)
            {
                tmp2ndSpan.CopyTo(0, s, first, secondCounter);
            }
        }
        else
        {
            // First partition is smaller - buffer it and merge forward
            var bufferSize = second - first;
            var tmp1stSpan = new SortSpan<T, TComparer, TContext>(workBuffer[..bufferSize], s.Context, s.Comparer, BUFFER_TEMP_FIRST);

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
            while (firstCounter < bufferSize && right < third)
            {
                // Stability: use '<' (not '<=') to ensure left < right in final output when equal.
                // The comparison hands back both operands so the write below does not read either a second time.
                if (s.IsLessAcross(right, tmp1stSpan, firstCounter, out var rightValue, out var leftValue))
                {
                    s.Write(writePos, rightValue);
                    right++;
                }
                else
                {
                    s.Write(writePos, leftValue);
                    firstCounter++;
                }
                writePos++;
            }

            // The right run is exhausted. Every remaining buffer element is larger than what is already
            // placed below it, so tmp1st[firstCounter..bufferSize) lands contiguously at writePos
            // with no further comparison.
            if (firstCounter < bufferSize)
            {
                tmp1stSpan.CopyTo(firstCounter, s, writePos, bufferSize - firstCounter);
            }
        }
    }
}
