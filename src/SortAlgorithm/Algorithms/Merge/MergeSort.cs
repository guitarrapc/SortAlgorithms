using System.Buffers;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 配列を再帰的に半分に分割し、それぞれをソートした後、ソート済みの部分配列をマージして全体をソートする分割統治アルゴリズムです。
/// 安定ソートであり、最悪・平均・最良のすべてのケースでO(n log n)の性能を保証します。
/// <br/>
/// Recursively divides the array in half, sorts each part, then merges the sorted subarrays to produce a fully sorted result.
/// This divide-and-conquer algorithm is stable and guarantees O(n log n) performance in all cases (worst, average, best).
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Merge Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Divide Step (Binary Partitioning):</strong> The array must be divided into two roughly equal halves at each recursion level.
/// The midpoint is calculated as mid = (left + right) / 2, ensuring balanced subdivision.
/// This guarantees a recursion depth of ⌈log₂(n)⌉.</description></item>
/// <item><description><strong>Base Case (Termination Condition):</strong> Recursion must terminate when a subarray has size ≤ 1.
/// An array of size 0 or 1 is trivially sorted and requires no further processing.</description></item>
/// <item><description><strong>Conquer Step (Recursive Sorting):</strong> Each half must be sorted independently via recursive calls.
/// The left subarray [left..mid] and right subarray [mid+1..right] are sorted before merging.</description></item>
/// <item><description><strong>Merge Step (Sorted Subarray Combination):</strong> Two sorted subarrays must be merged into a single sorted array.
/// This requires O(n) auxiliary space to temporarily hold one half during the merge operation.
/// The merge compares elements from both halves and writes them in ascending order.</description></item>
/// <item><description><strong>Stability Preservation (Equal Element Ordering):</strong> When merging, elements from the left subarray must be taken first
/// when both sides have equal values (using &lt;= comparison). This preserves the relative order of equal elements, ensuring stability.</description></item>
/// <item><description><strong>Comparison Count:</strong> At each recursion level, merging n elements requires at most n-1 comparisons.
/// With ⌈log₂(n)⌉ levels, total comparisons: n⌈log₂(n)⌉ - 2^⌈log₂(n)⌉ + 1 (worst case).
/// Best case (sorted data): approximately n⌈log₂(n)⌉ / 2.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Merge</description></item>
/// <item><description>Stable      : Yes (equal elements maintain relative order via &lt;= comparison during merge)</description></item>
/// <item><description>In-place    : No (requires O(n) auxiliary space for merging)</description></item>
/// <item><description>Best case   : O(n log n) - Even sorted data requires ⌈log₂(n)⌉ levels of merging</description></item>
/// <item><description>Average case: O(n log n) - Balanced recursion tree with n work per level</description></item>
/// <item><description>Worst case  : O(n log n) - Guaranteed balanced partitioning regardless of input</description></item>
/// <item><description>Comparisons : O(n log n) - At most n⌈log₂(n)⌉ - 2^⌈log₂(n)⌉ + 1 comparisons</description></item>
/// <item><description>Writes      : O(n log n) - Each level writes all n elements, ⌈log₂(n)⌉ levels total</description></item>
/// <item><description>Space       : O(n) - Auxiliary buffer of size n/2 for merging (this implementation uses ArrayPool for efficiency)</description></item>
/// </list>
/// <para><strong>Advantages of Merge Sort:</strong></para>
/// <list type="bullet">
/// <item><description>Predictable performance - O(n log n) guaranteed in all cases</description></item>
/// <item><description>Stable - Preserves relative order of equal elements</description></item>
/// <item><description>Parallelizable - Independent recursive branches can be processed concurrently</description></item>
/// <item><description>External sorting - Well-suited for sorting data that doesn't fit in memory</description></item>
/// </list>
/// <para><strong>Use Cases:</strong></para>
/// <list type="bullet">
/// <item><description>When stability is required (e.g., sorting records by multiple keys)</description></item>
/// <item><description>When predictable O(n log n) performance is essential</description></item>
/// <item><description>Linked list sorting (can be done in-place with O(1) space)</description></item>
/// <item><description>External sorting of large datasets</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Merge_sort</para>
/// </remarks>
public static class MergeSort
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

        // Rent buffer from ArrayPool for O(n) auxiliary space (instead of O(n log n) stack allocations)
        var buffer = ArrayPool<T>.Shared.Rent(span.Length);
        try
        {
            var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
            var b = new SortSpan<T, TComparer, TContext>(buffer.AsSpan(0, span.Length), context, comparer, BUFFER_MERGE);
            SortCore(s, b, 0, span.Length - 1);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(buffer, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    /// <summary>
    /// Core recursive merge sort implementation.
    /// </summary>
    /// <param name="s">The SortSpan wrapping the span to sort</param>
    /// <param name="b">The SortSpan wrapping the auxiliary buffer for merging</param>
    /// <param name="left">The inclusive start index of the range to sort</param>
    /// <param name="right">The inclusive end index of the range to sort</param>
    private static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> b, int left, int right)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (right <= left) return; // Base case: array of size 0 or 1 is sorted

        var mid = left + (right - left) / 2;

        // Conquer: Recursively sort left and right halves
        SortCore(s, b, left, mid);
        SortCore(s, b, mid + 1, right);

        // Optimization: Skip merge if already sorted (left[last] <= right[first])
        // This dramatically improves performance on nearly-sorted data
        if (s.Compare(mid, mid + 1) <= 0)
        {
            return; // Already sorted, no merge needed
        }

        // Merge: Combine two sorted halves
        Merge(s, b, left, mid, right);
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
