using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// Iterative (bottom-up) Rotate Merge Sort.
/// Bottom-up variant of <see cref="RotateMergeSort"/> that replaces recursion with two explicit phases:
/// Phase 1 sorts every block of ≤<c>InsertionSortThreshold</c> elements with insertion sort,
/// Phase 2 merges adjacent run pairs in-place using rotation, doubling the run width on each pass.
/// Stability, O(1) auxiliary space, and O(n log² n) performance are preserved from the recursive variant,
/// while eliminating the O(log n) call-stack overhead and any risk of stack overflow on large inputs.
/// <br/>
/// Iterative (bottom-up) Rotate Merge Sort.
/// Eliminates recursion by processing the array in two phases:
/// (1) sort each block of ≤InsertionSortThreshold elements with insertion sort,
/// (2) merge adjacent run pairs using in-place rotation, doubling run width each pass until the array is fully sorted.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Iterative Rotate Merge Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Phase 1 – Insertion Sort Seeding:</strong> Every contiguous block of size
/// InsertionSortThreshold is sorted independently with insertion sort.
/// The last block may be shorter; its size is clamped to the remaining element count.</description></item>
/// <item><description><strong>Phase 2 – Bottom-Up Merge Passes:</strong> Starting from
/// <c>width = InsertionSortThreshold</c>, each pass merges adjacent run pairs
/// [left..left+width-1] and [left+width..left+2*width-1], then doubles <c>width</c>.
/// The outer loop runs ⌈log₂(n/InsertionSortThreshold)⌉ times.</description></item>
/// <item><description><strong>End-of-Array Clamping:</strong> The right boundary of the last pair is
/// clamped: <c>right = Math.Min(left + 2*width - 1, n - 1)</c>.
/// The loop condition <c>left &lt; n - width</c> guarantees a non-empty right run exists before merging.</description></item>
/// <item><description><strong>Already-Sorted Skip:</strong> Before each merge, if
/// <c>s[mid] ≤ s[mid+1]</c> the two runs are already in order and the merge is skipped,
/// reducing work on nearly-sorted inputs.</description></item>
/// <item><description><strong>Galloping Optimization:</strong> Within each merge, exponential search
/// (1, 2, 4, 8, …) followed by binary search efficiently finds long runs of consecutive right-partition
/// elements, similar to TimSort's galloping mode.</description></item>
/// <item><description><strong>Rotation Algorithm (Left-Rotate by k, 3-Reversal with fast paths):</strong>
/// Left-rotates A[left..right] by k positions: [left_k_elems | rest] → [rest | left_k_elems].
/// Fast path k==1: move leftmost element to right end.
/// Fast path k==n-1: move rightmost element to left end (left rotate n-1 = right rotate 1).
/// General case uses 3-reversal: Reverse(A[left..left+k-1]), Reverse(A[left+k..right]), Reverse(A[left..right]).</description></item>
/// <item><description><strong>Stability Preservation:</strong> Binary search inside the merge uses ≤ comparison,
/// ensuring equal elements from the left run appear before those from the right run.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Hybrid (Merge + Insertion + Galloping), Iterative</description></item>
/// <item><description>Stable      : Yes (≤ comparison in merge preserves relative order)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space)</description></item>
/// <item><description>Best case   : O(n) – Sorted data: insertion sort is O(n), all phase-2 merges are skipped</description></item>
/// <item><description>Average case: O(n log² n) – Binary search (log n) + rotation (n) per merge × log n passes</description></item>
/// <item><description>Worst case  : O(n log² n)</description></item>
/// <item><description>Space       : O(1) – No recursion stack; only a constant number of loop variables</description></item>
/// </list>
/// <para><strong>Iterative vs Recursive:</strong></para>
/// <list type="bullet">
/// <item><description>Eliminates O(log n) call-stack depth; safe for arbitrarily large arrays</description></item>
/// <item><description>Merge order differs: bottom-up processes fixed-width blocks rather than balanced halves,
/// but total work and asymptotic complexity are identical</description></item>
/// <item><description>Slightly simpler control flow; easier to reason about run boundaries</description></item>
/// </list>
/// </remarks>
public static class RotateMergeSortIterative
{
    // Threshold for using insertion sort instead of rotation-based merge
    private const int InsertionSortThreshold = 16;

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;

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
    /// <param name="span">The span of elements to sort in place.</param>
    /// <param name="context">The sort context for tracking operations. Cannot be null.</param>
    public static void Sort<T, TContext>(Span<T> span, TContext context)
        where T : IComparable<T>
        where TContext : ISortContext
        => Sort(span, new ComparableComparer<T>(), context);

    /// <summary>
    /// Sorts the elements in the specified span using the provided comparer and sort context.
    /// This is the full-control version with explicit TComparer and TContext type parameters.
    /// </summary>
    public static void Sort<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        SortCore(s, span.Length);
    }

    /// <summary>
    /// Bottom-up iterative sort core: Phase 1 seeds sorted runs with insertion sort,
    /// Phase 2 merges adjacent run pairs with doubling widths until fully sorted.
    /// </summary>
    /// <param name="s">The SortSpan wrapping the span to sort</param>
    /// <param name="n">Total number of elements (span.Length)</param>
    private static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Phase 1: sort every block of size InsertionSortThreshold with insertion sort.
        // InsertionSort.SortCore uses exclusive end [first, last), so pass Math.Min(i + threshold, n).
        for (var i = 0; i < n; i += InsertionSortThreshold)
            InsertionSort.SortCore(s, i, Math.Min(i + InsertionSortThreshold, n));

        // Phase 2: bottom-up merge passes.
        // Each pass merges adjacent pairs of runs of length `width`, then doubles width.
        for (var width = InsertionSortThreshold; width < n; width *= 2)
        {
            // left + width < n guarantees a non-empty right run ([mid+1..right]) exists.
            for (var left = 0; left < n - width; left += width * 2)
            {
                // mid: inclusive end of left run — always exactly `width` elements from `left`.
                var mid = left + width - 1;
                // right: inclusive end of right run — clamped to last valid index for the final pair.
                var right = Math.Min(left + width * 2 - 1, n - 1);

                // Already-sorted skip: left run's max ≤ right run's min → no merge needed.
                if (s.Compare(mid, mid + 1) <= 0)
                    continue;

                MergeInPlace(s, left, mid, right);
            }
        }
    }

    /// <summary>
    /// Merges two sorted subarrays [left..mid] and [mid+1..right] in-place using rotation.
    /// Uses galloping (exponential search + binary search) to efficiently find long consecutive blocks.
    /// </summary>
    private static void MergeInPlace<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int mid, int right)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var start1 = left;
        var start2 = mid + 1;

        while (start1 <= mid && start2 <= right)
        {
            if (s.Compare(start1, start2) <= 0)
            {
                start1++;
            }
            else
            {
                // s[start1] > s[start2]: gallop to find how many right-partition elements
                // are all less than s[start1], then rotate the entire block at once.
                var start2End = GallopingSearchEnd(s, start1, start2, right);

                var blockSize = start2End - start2 + 1;
                var rotateDistance = start2 - start1;

                // Left-rotate [start1..start2End] by rotateDistance: [left_part | right_block] → [right_block | left_part]
                // right_block elements (all < s[start1]) are moved to the front; left_part shifts right.
                Rotate(s, start1, start2End, rotateDistance);

                start1 += blockSize;
                mid += blockSize;
                start2 = start2End + 1;
            }
        }
    }

    /// <summary>
    /// Finds the end position of consecutive right-partition elements that are all less than s[leftBoundary].
    /// Phase 1: exponential search (1, 2, 4, 8, …) to find a rough upper bound.
    /// Phase 2: binary search to pinpoint the exact boundary.
    /// </summary>
    /// <returns>The last index in [start..end] where s[i] &lt; s[leftBoundary]</returns>
    private static int GallopingSearchEnd<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int leftBoundary, int start, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var lastGood = start;
        var step = 1;

        while (start + step <= end && s.Compare(leftBoundary, start + step) > 0)
        {
            lastGood = start + step;
            step *= 2;
        }

        var low = lastGood;
        var high = Math.Min(start + step, end);

        while (low < high)
        {
            var mid = low + (high - low + 1) / 2;

            if (s.Compare(leftBoundary, mid) > 0)
                low = mid;
            else
                high = mid - 1;
        }

        return low;
    }

    /// <summary>
    /// Left-rotates A[left..right] by k positions: [left_k_elems | rest] → [rest | left_k_elems].
    /// Fast path k==1 (left rotate 1): move leftmost element to right end.
    /// Fast path k==n-1 (left rotate n-1 = right rotate 1): move rightmost element to left end.
    /// General case uses 3-reversal: Reverse[left..left+k-1], Reverse[left+k..right], Reverse[left..right].
    /// All paths are linear scans, enabling hardware prefetching without GCD or modulo overhead.
    /// </summary>
    /// <param name="k">The number of positions to rotate left</param>
    private static void Rotate<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int right, int k)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (k == 0 || left >= right) return;

        var n = right - left + 1;
        k = k % n;
        if (k == 0) return;

        // Fast path: k==1 (left rotate 1) - move leftmost element to right end, shift rest left (sequential read/write, no swap)
        if (k == 1)
        {
            var tmp = s.Read(left);
            for (var i = left; i < right; i++)
                s.Write(i, s.Read(i + 1));
            s.Write(right, tmp);
            return;
        }

        // Fast path: k==n-1 (left rotate n-1 = right rotate 1) - move rightmost element to left end, shift rest right (sequential read/write, no swap)
        if (k == n - 1)
        {
            var tmp = s.Read(right);
            for (var i = right; i > left; i--)
                s.Write(i, s.Read(i - 1));
            s.Write(left, tmp);
            return;
        }

        // General case: left rotate by k via 3-reversal (linear scans, cache-friendly, no GCD overhead)
        // [A|B] → Reverse(A), Reverse(B), Reverse(AB) → [B|A]
        Reverse(s, left, left + k - 1);
        Reverse(s, left + k, right);
        Reverse(s, left, right);
    }

    /// <summary>
    /// Reverses a subarray in-place using swaps.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Reverse<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int right)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (left < right)
        {
            s.Swap(left, right);
            left++;
            right--;
        }
    }
}
