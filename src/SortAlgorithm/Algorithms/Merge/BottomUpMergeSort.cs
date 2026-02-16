using System.Buffers;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// ボトムアップ方式でマージソートを実装した反復的なソートアルゴリズムです。
/// MergeSortと異なり再帰を使用せず、サイズ1のサブ配列から開始し、反復的にサイズを2倍にしながらマージしていきます。
/// 安定ソートであり、すべてのケースでO(n log n)の性能を保証します。
/// <br/>
/// An iterative merge sort implementation that uses a bottom-up approach.
/// Unlike MergeSort, instead of recursion, it starts with subarrays of size 1 and iteratively merges them while doubling the size.
/// This algorithm is stable and guarantees O(n log n) performance in all cases.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Bottom-Up Merge Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Iterative Subdivision (Bottom-Up Strategy):</strong> Unlike top-down merge sort, which recursively divides the array,
/// bottom-up merge sort starts with subarrays of size 1 (trivially sorted) and iteratively merges adjacent pairs.
/// The merge size doubles at each pass: 1 → 2 → 4 → 8 → ... until the entire array is sorted.
/// This eliminates recursion overhead and provides better stack usage (O(1) auxiliary stack space).</description></item>
/// <item><description><strong>Pass Structure (Logarithmic Iterations):</strong> The algorithm performs ⌈log₂(n)⌉ passes over the data.
/// Each pass merges subarrays of size 2^k into subarrays of size 2^(k+1), where k starts at 0.
/// Pass 1: Merge pairs [0,1], [2,3], [4,5], ... (size 1 → 2)
/// Pass 2: Merge pairs [0,3], [4,7], [8,11], ... (size 2 → 4)
/// Pass k: Merge pairs of size 2^k into size 2^(k+1)
/// This guarantees exactly ⌈log₂(n)⌉ passes regardless of input order.</description></item>
/// <item><description><strong>Merge Step (Sorted Subarray Combination):</strong> Each merge combines two adjacent sorted subarrays [left..mid] and [mid+1..right].
/// The merge requires O(n) auxiliary space to temporarily hold one subarray during combination.
/// Elements are compared and written in ascending order, preserving stability by taking from left first when equal (using &lt;= comparison).</description></item>
/// <item><description><strong>Boundary Handling:</strong> For arrays whose length is not a power of 2, the final subarray in each pass may be smaller.
/// The algorithm must correctly handle three cases:
/// (a) Two complete subarrays of size s
/// (b) One complete subarray (size s) and one partial subarray (size &lt; s)
/// (c) One remaining subarray (no merge partner) - left as-is</description></item>
/// <item><description><strong>Stability Preservation:</strong> When merging, elements from the left subarray must be taken first
/// when both sides have equal values (using &lt;= comparison). This preserves the relative order of equal elements, ensuring stability.</description></item>
/// <item><description><strong>Comparison Count:</strong> Each element participates in at most ⌈log₂(n)⌉ merge operations.
/// Each merge performs at most n-1 comparisons per pass.
/// Total comparisons: approximately n⌈log₂(n)⌉ - 2^⌈log₂(n)⌉ + 1 (same as top-down merge sort).</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Merge</description></item>
/// <item><description>Stable      : Yes (equal elements maintain relative order via &lt;= comparison during merge)</description></item>
/// <item><description>In-place    : No (requires O(n) auxiliary space for merging)</description></item>
/// <item><description>Best case   : O(n log n) - Even sorted data requires ⌈log₂(n)⌉ passes of merging</description></item>
/// <item><description>Average case: O(n log n) - ⌈log₂(n)⌉ passes with n work per pass</description></item>
/// <item><description>Worst case  : O(n log n) - Guaranteed performance regardless of input</description></item>
/// <item><description>Comparisons : O(n log n) - At most n⌈log₂(n)⌉ - 2^⌈log₂(n)⌉ + 1 comparisons</description></item>
/// <item><description>Writes      : O(n log n) - Each pass writes all n elements, ⌈log₂(n)⌉ passes total</description></item>
/// <item><description>Space       : O(n) - Auxiliary buffer of size n/2 for merging (uses ArrayPool for efficiency)</description></item>
/// <item><description>Stack       : O(1) - No recursion, constant stack usage (unlike O(log n) for top-down)</description></item>
/// </list>
/// <para><strong>Advantages Over Top-Down Merge Sort:</strong></para>
/// <list type="bullet">
/// <item><description>No recursion overhead - Eliminates function call stack and return address management</description></item>
/// <item><description>O(1) stack space - Uses only a few local variables instead of O(log n) recursion frames</description></item>
/// <item><description>Predictable memory access - More sequential passes can improve cache performance</description></item>
/// <item><description>Easier to parallelize - Each pass has independent merge operations that can run concurrently</description></item>
/// <item><description>Simpler to analyze - Explicit loop structure makes performance characteristics more obvious</description></item>
/// </list>
/// <para><strong>Use Cases:</strong></para>
/// <list type="bullet">
/// <item><description>When stability is required and stack space is limited</description></item>
/// <item><description>Embedded systems or environments where recursion depth is restricted</description></item>
/// <item><description>External sorting where iterative passes over disk data are natural</description></item>
/// <item><description>Parallel sorting where independent merges can be distributed across processors</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Merge_sort#Bottom-up_implementation</para>
/// </remarks>
public static class BottomupMergeSort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_MERGE = 1;      // Merge buffer (auxiliary space)

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
    public static void Sort<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;

        // Rent buffer from ArrayPool for O(n) auxiliary space
        var buffer = ArrayPool<T>.Shared.Rent(span.Length);
        try
        {
            var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
            var b = new SortSpan<T, TComparer, TContext>(buffer.AsSpan(0, span.Length), context, comparer, BUFFER_MERGE);
            SortCore(s, b);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(buffer, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    /// <summary>
    /// Core bottom-up merge sort implementation.
    /// Iteratively merges subarrays of size 1, 2, 4, 8, ... until the entire array is sorted.
    /// </summary>
    /// <param name="s">The SortSpan wrapping the span to sort</param>
    /// <param name="b">The SortSpan wrapping the auxiliary buffer for merging</param>
    private static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> b)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = s.Length;

        // Iterate through merge sizes: 1, 2, 4, 8, ..., until size >= n
        // Each pass doubles the size of sorted subarrays
        for (var size = 1; size < n; size *= 2)
        {
            // Merge adjacent pairs of subarrays of 'size'
            for (var left = 0; left < n - size; left += size * 2)
            {
                // Calculate mid and right boundaries
                var mid = left + size - 1;
                var right = Math.Min(left + size * 2 - 1, n - 1);

                // Optimization: Skip merge if already sorted
                if (s.Compare(mid, mid + 1) <= 0)
                    continue;

                // Merge two adjacent sorted subarrays
                Merge(s, b, left, mid, right);
            }
        }
    }

    /// <summary>
    /// Merges two sorted subarrays [left..mid] and [mid+1..right] using buffer as auxiliary space.
    /// </summary>
    /// <param name="s">The SortSpan wrapping the main array</param>
    /// <param name="b">The SortSpan wrapping the auxiliary buffer</param>
    /// <param name="left">The inclusive start index of the left subarray</param>
    /// <param name="mid">The inclusive end index of the left subarray</param>
    /// <param name="right">The inclusive end index of the right subarray</param>
    private static void Merge<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> b, int left, int mid, int right)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var leftLength = mid - left + 1;

        // Copy left partition to buffer to avoid overwriting during merge
        s.CopyTo(left, b, 0, leftLength);

        var l = 0;           // Index in buffer (left partition copy, 0-based)
        var r = mid + 1;     // Index in span (right partition starts after mid)
        var k = left;        // Index in result (span, starts at left)

        // Merge: compare elements from buffer (left) and right partition
        while (l < leftLength && r <= right)
        {
            var leftValue = b.Read(l);
            var rightValue = s.Read(r);

            // Stability: use <= to take from left when equal
            if (s.Compare(leftValue, rightValue) <= 0)
            {
                s.Write(k, leftValue);
                l++;
            }
            else
            {
                s.Write(k, rightValue);
                r++;
            }
            k++;
        }

        // Copy remaining elements from buffer (left partition) if any
        if (l < leftLength)
        {
            b.CopyTo(l, s, k, leftLength - l);
        }

        // Right partition elements are already in place, no need to copy
    }
}
