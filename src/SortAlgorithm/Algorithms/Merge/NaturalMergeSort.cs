using System.Buffers;
using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 入力配列中の自然に発生する昇順の連続部分列（ラン）を検出し、それらを反復的にマージしてソートする適応型マージソートです。
/// データが部分的にソートされている場合に効率的であり、すでにソート済みのデータに対しては最良ケースO(n)で動作します。
/// 安定ソートです。
/// <br/>
/// An adaptive merge sort that detects naturally occurring ascending runs in the input,
/// then iteratively merges adjacent pairs of runs until the entire array is sorted.
/// Efficient on partially sorted data, achieving O(n) best case on already-sorted input.
/// Stable sort.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Natural Merge Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Run Detection:</strong> Scan the array to identify maximal non-decreasing subsequences (natural runs).
/// Each run is a contiguous region [start..end] where s[i] &lt;= s[i+1] for all consecutive pairs.</description></item>
/// <item><description><strong>Pairwise Merging:</strong> In each pass, merge adjacent pairs of runs.
/// If an odd number of runs exists, the last run is carried forward unmerged.</description></item>
/// <item><description><strong>Termination:</strong> Repeat passes until only a single run remains, covering the entire array.</description></item>
/// <item><description><strong>Stability Preservation:</strong> During merge, when elements from both runs are equal,
/// the element from the left run is taken first, preserving relative order.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Merge</description></item>
/// <item><description>Stable      : Yes (equal elements maintain relative order via &lt;= comparison during merge)</description></item>
/// <item><description>In-place    : No (requires O(n) auxiliary space for merging)</description></item>
/// <item><description>Adaptive    : Yes (exploits existing sorted runs in the input)</description></item>
/// <item><description>Best case   : O(n) - Already sorted data is a single run; only one detection scan needed</description></item>
/// <item><description>Average case: O(n log n) - Random data has ~n/2 runs, requiring ~log₂(n) merge passes</description></item>
/// <item><description>Worst case  : O(n log n) - Fully descending data has n runs of size 1, equivalent to bottom-up merge sort</description></item>
/// <item><description>Space       : O(n) - Auxiliary buffer for merging (this implementation uses ArrayPool for efficiency)</description></item>
/// </list>
/// <para><strong>Advantages over Standard Merge Sort:</strong></para>
/// <list type="bullet">
/// <item><description>Adaptive - O(n) on already-sorted or nearly-sorted data (standard merge sort is always O(n log n))</description></item>
/// <item><description>Non-recursive - Iterative bottom-up approach avoids stack overflow on large inputs</description></item>
/// <item><description>Fewer merges - Exploits pre-existing order, skipping unnecessary work</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Merge_sort#Natural_merge_sort</para>
/// </remarks>
public static class NaturalMergeSort
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

        // Rent buffer from ArrayPool for O(n) auxiliary space
        var buffer = ArrayPool<T>.Shared.Rent(span.Length);
        try
        {
            var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
            var b = new SortSpan<T, TComparer, TContext>(buffer.AsSpan(0, span.Length), context, comparer, BUFFER_MERGE);
            SortCore(s, b, span.Length);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(buffer, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    /// <summary>
    /// Core iterative natural merge sort implementation.
    /// Repeatedly detects natural runs and merges adjacent pairs until a single run remains.
    /// </summary>
    /// <param name="s">The SortSpan wrapping the span to sort</param>
    /// <param name="b">The SortSpan wrapping the auxiliary buffer for merging</param>
    /// <param name="length">The number of elements to sort</param>
    private static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> b, int length)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var pass = 0;

        while (true)
        {
            pass++;
            s.Context.OnPhase(SortPhase.MergeRunDetect);

            var left = 0;
            var mergeCount = 0;

            while (left < length)
            {
                // Find end of first natural run [left..mid] (non-decreasing)
                var mid = left;
                while (mid + 1 < length && s.Compare(mid, mid + 1) <= 0)
                {
                    mid++;
                }

                // If this run extends to the end, no second run to merge with
                if (mid + 1 >= length)
                {
                    break;
                }

                // Find end of second natural run [mid+1..right] (non-decreasing)
                var right = mid + 1;
                while (right + 1 < length && s.Compare(right, right + 1) <= 0)
                {
                    right++;
                }

                // Merge the two adjacent runs
                s.Context.OnPhase(SortPhase.MergeSortMerge, left, mid, right);
                s.Context.OnRole(left, BUFFER_MAIN, RoleType.LeftPointer);
                s.Context.OnRole(right, BUFFER_MAIN, RoleType.RightPointer);
                Merge(s, b, left, mid, right);
                s.Context.OnRole(left, BUFFER_MAIN, RoleType.None);
                s.Context.OnRole(right, BUFFER_MAIN, RoleType.None);
                mergeCount++;

                left = right + 1;
            }

            // If no merges were performed, the entire array is a single sorted run
            if (mergeCount == 0)
            {
                break;
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
