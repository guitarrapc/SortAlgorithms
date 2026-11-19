using System.Buffers;
using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 配列を再帰的に半分に分割し、各再帰レベルで送信元バッファ（ping）と書き込み先バッファ（pong）を交互に入れ替えながらマージするトップダウン型マージソートです。
/// 各マージの結果を、その階層の書き込み先バッファへ直接格納します。
/// 安定ソートであり、最悪・平均・最良のすべてのケースでO(n log n)の性能を保証します。
/// <br/>
/// A top-down recursive merge sort that alternates source (ping) and destination (pong) buffers at each recursion level.
/// Each merge writes its result directly into the destination buffer for that recursion level.
/// Stable and guarantees O(n log n) performance in all cases.
/// </summary>
/// <remarks>
/// <para><strong>Ping-Pong Strategy:</strong></para>
/// <para>Two full-size buffers s (main span) and b (auxiliary) are used.
/// <c>SortCore(dst, src, left, right)</c> sorts src[left..right] and writes the result into dst[left..right].
/// Recursive calls swap dst and src so the halves end up sorted in src, ready for the final merge into dst.
/// The merge reads entirely from src and writes entirely to dst — no intermediate buffer copy needed.</para>
/// <code>
/// SortCore(dst=s, src=b, left, right):
///   SortCore(dst=b, src=s, left, mid)      // left half sorted into b
///   SortCore(dst=b, src=s, mid+1, right)   // right half sorted into b
///   Merge b[left..mid] + b[mid+1..right] → s[left..right]   // pure ping-pong
/// </code>
/// <para>Initial setup copies the original span into b, so both buffers hold every element at the recursion leaves
/// regardless of recursion depth parity, and a leaf has nothing to copy.</para>
/// <para><strong>Theoretical Conditions for Correct Pingpong Merge Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Initial Copy (Bootstrap):</strong> Before recursion, the original span is copied into the auxiliary buffer b.
/// Because recursion alternates which buffer is the destination, a leaf's destination is b at some depths and s at others.
/// Having the input in both means the element is already in the destination at any depth.</description></item>
/// <item><description><strong>Divide Step (Binary Partitioning):</strong> The range is divided at mid = left + (right - left) / 2,
/// giving a balanced recursion tree of depth ⌈log₂(n)⌉, same as standard top-down merge sort.</description></item>
/// <item><description><strong>Base Case (Termination):</strong> When left == right, the function returns. No merge writes a slot
/// before the leaf that holds it returns, so the slot still holds the input element in both buffers.</description></item>
/// <item><description><strong>Conquer Step with Buffer Alternation:</strong> Both recursive calls swap dst and src.
/// This guarantees that after both calls, the sorted halves reside in src (the current call's source buffer),
/// which is exactly what the merge step needs to read from.</description></item>
/// <item><description><strong>Merge Step (Pure Ping-Pong):</strong> Merges src[left..mid] and src[mid+1..right] into dst[left..right].
/// Reads exclusively from src and writes exclusively to dst — no extra buffer needed within the merge itself.
/// Each merge writes its result directly to the destination buffer.</description></item>
/// <item><description><strong>Stability Preservation:</strong> During merge, elements from the left half are taken first on equal values
/// (using &lt;= comparison), preserving the original relative order of equal elements.</description></item>
/// <item><description><strong>Element Movement:</strong> The bootstrap and the merges copy elements.
/// Sorted subranges still copy from the source to the destination buffer.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Merge</description></item>
/// <item><description>Stable      : Yes (equal elements maintain relative order via &lt;= comparison during merge)</description></item>
/// <item><description>In-place    : No (requires O(n) auxiliary space — one full-size buffer)</description></item>
/// <item><description>Best case   : O(n log n) writes, O(n) comparisons — sorted data skips merges but still propagates through levels</description></item>
/// <item><description>Average case: O(n log n) — balanced recursion tree with n writes per level</description></item>
/// <item><description>Worst case  : O(n log n) — guaranteed balanced partitioning regardless of input</description></item>
/// <item><description>Comparisons : O(n log n) — at most n⌈log₂(n)⌉ - 2^⌈log₂(n)⌉ + 1 in the merges, plus one per merge (n - 1 in all) for the already-sorted check</description></item>
/// <item><description>Swaps       : 0 — elements move by copying between the input and the auxiliary buffer, never by exchange</description></item>
/// <item><description>Index Reads : Θ(n log n) — each merge level reads all n elements</description></item>
/// <item><description>Index Writes: Θ(n log n) — the bootstrap copy plus the output of every merge level</description></item>
/// <item><description>Space       : O(n) — full-size auxiliary buffer (uses ArrayPool for efficiency)</description></item>
/// </list>
/// <para><strong>Comparison with Related Algorithms:</strong></para>
/// <list type="bullet">
/// <item><description>vs MergeSort       : Source and destination buffers alternate between recursion levels</description></item>
/// <item><description>vs BottomupMergeSort: Both use ping-pong buffering; BottomupMergeSort is iterative (bottom-up), PingpongMergeSort is recursive (top-down)</description></item>
/// <item><description>vs NaturalMergeSort : NaturalMergeSort achieves O(n) on pre-sorted data; PingpongMergeSort is always O(n log n) writes but simpler</description></item>
/// </list>
/// <para><strong>History:</strong></para>
/// <para>The name comes from Chandramouli and Goldstein (SIGMOD 2014), who present "ping-pong merge" as a new technique for
/// merging sorted runs in main memory. Alternating merge passes between the array and a buffer is older: the SGI STL
/// <c>algo.h</c> (copyright Hewlett-Packard 1994, Silicon Graphics 1996) already merges array → buffer → array in
/// <c>__merge_sort_with_buffer</c>, the routine <see cref="StdStableSort"/> describes for libstdc++.</para>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Merge_sort#Top-down_implementation</para>
/// <para>Paper: "Patience is a Virtue: Revisiting Merge and Sort on Modern Processors" by Badrish Chandramouli and Jonathan Goldstein (SIGMOD 2014) https://www.microsoft.com/en-us/research/wp-content/uploads/2016/02/patsort-sigmod14.pdf</para>
/// </remarks>
public static class PingpongMergeSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by PingpongMergeSortTests, which derives from StableSortTestsBase.</remarks>
    public static bool IsStable => true;

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array (destination at top level)
    private const int BUFFER_MERGE = 1;      // Auxiliary buffer (initial source / ping buffer)

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
    public static void Sort<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;

        // Rent a full-size auxiliary buffer for ping-pong passes
        var buffer = ArrayPool<T>.Shared.Rent(span.Length);
        try
        {
            // Copy span into buffer (b becomes the initial source / "ping" buffer).
            // With the input in both buffers, a leaf's element is already in its destination at any recursion depth.
            var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
            var b = new SortSpan<T, TComparer, TContext>(buffer.AsSpan(0, span.Length), context, comparer, BUFFER_MERGE);
            s.CopyTo(0, b, 0, span.Length);

            // Sort b[0..n-1] into s[0..n-1]: b is the source (ping), s is the destination (pong).
            // The result lands in s (the original span) — no copy-back needed.
            SortCore(s, b, 0, span.Length - 1);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(buffer, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    /// <summary>
    /// Core recursive ping-pong merge sort.
    /// Sorts src[left..right] and writes the result into dst[left..right].
    /// Recursive calls swap dst and src so that after both calls the sorted halves reside in src,
    /// ready to be merged into dst without any intermediate buffer copy.
    /// </summary>
    /// <param name="dst">The SortSpan to write the sorted result into</param>
    /// <param name="src">The SortSpan to read the input data from</param>
    /// <param name="left">The inclusive start index of the range to sort</param>
    /// <param name="right">The inclusive end index of the range to sort</param>
    private static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> dst, SortSpan<T, TComparer, TContext> src, int left, int right)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Base case: a single element is already in dst. The bootstrap gave both buffers the input,
        // and no merge touches this slot before its leaf returns.
        if (left == right) return;

        var mid = left + (right - left) / 2;

        // Recursively sort halves by swapping dst and src.
        // After these calls, src[left..mid] and src[mid+1..right] are each sorted
        // and ready to be merged into dst.
        SortCore(src, dst, left, mid);
        SortCore(src, dst, mid + 1, right);

        // Optimization: if src[mid] <= src[mid+1] the range is already sorted.
        // Copy src directly to dst instead of merging (1 comparison, n writes).
        if (src.IsLessOrEqualAt(mid, mid + 1))
        {
            src.CopyTo(left, dst, left, right - left + 1);
            return;
        }

        // Merge src[left..mid] and src[mid+1..right] into dst[left..right] (ping → pong).
        // Reads entirely from src, writes entirely to dst — no extra buffer needed.
        src.Context.OnPhase(SortPhase.MergeSortMerge, left, mid, right);
        src.Context.OnRole(left, src.BufferId, RoleType.LeftPointer);
        src.Context.OnRole(mid + 1, src.BufferId, RoleType.RightPointer);
        Merge(src, dst, left, mid, right);
        src.Context.OnRole(left, src.BufferId, RoleType.None);
        src.Context.OnRole(mid + 1, src.BufferId, RoleType.None);
    }

    /// <summary>
    /// Merges two sorted subarrays src[left..mid] and src[mid+1..right] into dst[left..right].
    /// Reads entirely from src and writes entirely to dst (pure ping-pong — no extra buffer needed).
    /// </summary>
    /// <param name="src">The SortSpan to read from (contains two adjacent sorted halves)</param>
    /// <param name="dst">The SortSpan to write the merged result into</param>
    /// <param name="left">The inclusive start index of the left subarray</param>
    /// <param name="mid">The inclusive end index of the left subarray</param>
    /// <param name="right">The inclusive end index of the right subarray</param>
    private static void Merge<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> src, SortSpan<T, TComparer, TContext> dst, int left, int mid, int right)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var l = left;      // pointer into left half  src[left..mid]
        var r = mid + 1;   // pointer into right half src[mid+1..right]
        var k = left;      // pointer into dst

        // The head that was not taken stays in its local instead of being read again.
        var leftValue = src.Read(l);
        var rightValue = src.Read(r);
        while (true)
        {
            // Stability: use <= to take from left when equal
            if (src.IsLessOrEqual(leftValue, rightValue))
            {
                dst.Write(k++, leftValue);
                if (++l > mid)
                {
                    src.CopyTo(r, dst, k, right - r + 1);
                    return;
                }
                leftValue = src.Read(l);
            }
            else
            {
                dst.Write(k++, rightValue);
                if (++r > right)
                {
                    src.CopyTo(l, dst, k, mid - l + 1);
                    return;
                }
                rightValue = src.Read(r);
            }
        }
    }
}
