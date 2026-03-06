using SortAlgorithm.Contexts;
using System.Diagnostics;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// ピボット要素を基準に配列を2つの領域に分割し、各領域をイテラティブにソートする分割統治法のソートアルゴリズムです。
/// 実用的なソートアルゴリズムの基礎として広く知られており、平均的には高速ですが最悪ケースでO(n²)の性能となります。
/// 本実装はDual-Pivotや3-way partitioningなどの高度な手法は使用せず、またInsertionSortなどの他アルゴリズムへの切り替えも行わない、ごく一般的なQuickSortです。
/// 再帰の代わりに明示的スタック（stackalloc）を用いたイテラティブ実装により、AntiQuickSortなどの逆境入力でもStackOverflowが発生しません。
/// <br/>
/// A divide-and-conquer sorting algorithm that partitions the array into two regions based on a pivot element and iteratively sorts each region.
/// Widely known as the foundation of practical sorting algorithms, it is fast on average but has O(n²) performance in the worst case.
/// This implementation is a basic QuickSort without advanced techniques such as dual-pivot or 3-way partitioning, and does not switch to other algorithms like InsertionSort.
/// An iterative implementation with an explicit stack-allocated stack replaces recursion to prevent stack overflow on adversarial inputs such as AntiQuickSort.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct QuickSort:</strong></para>
/// <list type="number">
/// <item><description><strong>Pivot Selection:</strong> A pivot element is selected from the array.
/// This implementation uses the middle element of the range as the pivot.
/// While this is simple, it can lead to worst-case O(n²) performance on sorted or reverse-sorted arrays.
/// More sophisticated implementations use median-of-three, median-of-nine, or random pivot selection to reduce worst-case probability.</description></item>
/// <item><description><strong>Partitioning (Hoare Partition Scheme):</strong> The array is rearranged so that elements less than the pivot are on the left,
/// and elements greater than the pivot are on the right. This implementation uses Hoare's partitioning scheme:
/// <list type="bullet">
/// <item><description>Two pointers (i, j) start from opposite ends and move toward each other</description></item>
/// <item><description>Left pointer i advances while elements are less than pivot</description></item>
/// <item><description>Right pointer j retreats while elements are greater than pivot</description></item>
/// <item><description>When both pointers stop, elements at i and j are swapped</description></item>
/// <item><description>Process continues until pointers cross (i &gt; j)</description></item>
/// </list>
/// After partitioning, all elements in [left, j] are ≤ pivot, and all elements in [i, right] are ≥ pivot.
/// Note: Hoare's scheme does not guarantee the pivot ends up at its final position, unlike Lomuto's scheme.</description></item>
/// <item><description><strong>Iterative Division:</strong> Instead of recursing, pending subranges are managed on an explicit stack-allocated stack.
/// The "smaller partition first" strategy immediately continues with the smaller partition and defers the larger one,
/// bounding the explicit stack depth to O(log n) regardless of pivot quality.
/// - Left region: [left, j]
/// - Right region: [i, right]
/// Base case: when right ≤ left, the range has ≤ 1 element and is trivially sorted.</description></item>
/// <item><description><strong>Termination:</strong> The algorithm terminates because:
/// - Each iteration operates on a strictly smaller subarray (at least one element is partitioned out)
/// - The base case (right ≤ left) is eventually reached for all subarrays
/// - Maximum explicit stack depth: O(log n) guaranteed by the smaller-partition-first strategy</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Partitioning (Divide and Conquer)</description></item>
/// <item><description>Partition   : Hoare partition scheme (bidirectional scan)</description></item>
/// <item><description>Stable      : No (partitioning does not preserve relative order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(log n) auxiliary space for explicit stack, guaranteed by smaller-partition-first strategy)</description></item>
/// <item><description>Best case   : Θ(n log n) - Balanced partitions (pivot divides array into two equal halves)</description></item>
/// <item><description>Average case: Θ(n log n) - Expected number of comparisons: 2n ln n ≈ 1.39n log₂ n</description></item>
/// <item><description>Worst case  : O(n²) - Occurs when pivot is always the smallest or largest element (e.g., sorted or reverse-sorted arrays with poor pivot selection)</description></item>
/// <item><description>Comparisons : 2n ln n (average) - Each partitioning pass compares elements with the pivot</description></item>
/// <item><description>Swaps       : n ln n / 3 (average) - Hoare's scheme performs fewer swaps than Lomuto's scheme</description></item>
/// </list>
/// <para><strong>Advantages of Hoare Partition Scheme:</strong></para>
/// <list type="bullet">
/// <item><description>Fewer swaps than Lomuto's scheme: approximately 3 times fewer on average</description></item>
/// <item><description>Better performance on arrays with many duplicate elements</description></item>
/// <item><description>Bidirectional scanning improves cache locality</description></item>
/// </list>
/// <para><strong>Disadvantages and Limitations:</strong></para>
/// <list type="bullet">
/// <item><description>Worst-case O(n²) performance on sorted or reverse-sorted arrays with poor pivot selection</description></item>
/// <item><description>Not stable: relative order of equal elements is not preserved</description></item>
/// <item><description>Worst-case O(n²) comparisons/swaps on already-sorted or reverse-sorted arrays with poor pivot selection</description></item>
/// <item><description>Poor performance on arrays with many duplicate elements (use 3-way partitioning instead)</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Quicksort</para>
/// </remarks>
public static class QuickSort
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
        => Sort(span, 0, span.Length, new ComparableComparer<T>(), NullContext.Default);

    /// <summary>
    /// Sorts the elements in the specified span using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <typeparam name="TContext">The type of the sort context.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T, TContext>(Span<T> span, TContext context)
        where T : IComparable<T>
        where TContext : ISortContext
        => Sort(span, 0, span.Length, new ComparableComparer<T>(), context);

    /// <summary>
    /// Sorts the elements in the specified span using the provided comparer and sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of the comparer</typeparam>
    /// <typeparam name="TContext">The type of the sort context.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="comparer">The comparer to use for element comparisons.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        => Sort(span, 0, span.Length, comparer, context);

    /// <summary>
    /// Sorts the subrange [first..last) using the provided comparer and sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <typeparam name="TContext">The type of the sort context.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="first">The inclusive start index of the range to sort.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T, TContext>(Span<T> span, int first, int last, TContext context)
        where T : IComparable<T>
        where TContext : ISortContext
        => Sort(span, first, last, new ComparableComparer<T>(), context);

    /// <summary>
    /// Sorts the subrange [first..last) using the provided comparer and context.
    /// This is the full-control version with explicit TContext type parameter.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use for element comparisons.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="span">The span containing elements to sort.</param>
    /// <param name="first">The inclusive start index of the range to sort.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    /// <param name="comparer">The comparer to use for element comparisons.</param>
    /// <param name="context">The sort context for tracking statistics and observations.</param>
    public static void Sort<T, TComparer, TContext>(Span<T> span, int first, int last, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        ArgumentOutOfRangeException.ThrowIfNegative(first);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(last, span.Length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(first, last);

        if (last - first <= 1) return;

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        SortCore(s, first, last - 1);
    }

    /// <summary>
    /// Sorts the subrange [left..right] (inclusive) using the provided sort context.
    /// This overload accepts a SortSpan directly for use by other algorithms that already have a SortSpan instance.
    /// Uses Hoare's partitioning scheme with the middle element as pivot.
    /// Iterative implementation with an explicit stack avoids call-stack overflow on adversarial inputs.
    /// The "smaller partition first" strategy guarantees the explicit stack depth stays within O(log n).
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use for element comparisons.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="left">The inclusive start index of the range to sort.</param>
    /// <param name="right">The inclusive end index of the range to sort.</param>
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int right)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Explicit stack for iterative QuickSort.
        // With smaller-partition-first, stack depth is bounded by O(log n).
        // 64 entries are more than enough for any practical Span<T> length addressable by int indexing.
        Span<int> stackL = stackalloc int[64];
        Span<int> stackR = stackalloc int[64];
        var top = -1;

        while (true)
        {
            if (left < right)
            {
                // Select pivot as the middle element (same as `s.Read((left + right) / 2)`)
                var pivotIdx = left + ((right - left) >> 1);
                s.Context.OnPhase(SortPhase.QuickSortPartition, left, right, pivotIdx);
                s.Context.OnRole(pivotIdx, BUFFER_MAIN, RoleType.Pivot);
                var pivot = s.Read(pivotIdx);

                // Uses a Hoare-style bidirectional partitioning scheme
                var i = left;
                var j = right;

                while (i <= j)
                {
                    // Move i forward while elements are less than pivot
                    while (s.Compare(i, pivot) < 0)
                    {
                        i++;
                    }

                    // Move j backward while elements are greater than pivot
                    while (s.Compare(pivot, j) < 0)
                    {
                        j--;
                    }

                    // Swap if pointers haven't crossed
                    if (i <= j)
                    {
                        s.Swap(i, j);
                        i++;
                        j--;
                    }
                }

                s.Context.OnRole(pivotIdx, BUFFER_MAIN, RoleType.None);

                // After partitioning, no element in [left..j] is greater than the pivot value,
                // and no element in [i..right] is less than the pivot value.
                if (j - left < right - i)
                {
                    // Left partition [left, j] is smaller → continue with it; defer right [i, right]
                    if (i < right)
                    {
                        Debug.Assert(top + 1 < stackL.Length);
                        stackL[++top] = i;
                        stackR[top] = right;
                    }
                    right = j;
                }
                else
                {
                    // Right partition [i, right] is smaller → continue with it; defer left [left, j]
                    if (left < j)
                    {
                        Debug.Assert(top + 1 < stackL.Length);
                        stackL[++top] = left;
                        stackR[top] = j;
                    }
                    left = i;
                }
            }
            else
            {
                if (top < 0) break;
                left = stackL[top];
                right = stackR[top--];
            }
        }
    }
}
