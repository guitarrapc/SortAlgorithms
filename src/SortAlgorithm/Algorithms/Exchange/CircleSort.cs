using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 配列の両端の要素を比較・交換し、内側に向かって繰り返すことでソートを行います。
/// スワップが発生しなくなるまで外側のループを繰り返します。
/// <br/>
/// Sorts by comparing and swapping elements at both ends of an interval and moving inward,
/// then recursively applying the same logic to each half.
/// The outer loop repeats until no swaps occur.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Circle Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Endpoint Comparison:</strong> Elements at the current low and high positions are compared,
/// and swapped if out of order. Both pointers then converge inward until they meet.</description></item>
/// <item><description><strong>Odd-Length Middle Element:</strong> When the two pointers meet at the same index (odd-length range),
/// the middle element is compared with its right neighbor and swapped if needed.</description></item>
/// <item><description><strong>Recursive Halving:</strong> After the circular pass over [lo, hi], the algorithm recursively
/// sorts [lo, lo+mid] and [lo+mid+1, hi] independently.</description></item>
/// <item><description><strong>Outer Loop Repetition:</strong> The full pass is repeated until no swaps occur,
/// guaranteeing convergence to a sorted array.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Exchange</description></item>
/// <item><description>Stable      : No (long-distance swaps do not preserve relative order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(log n) stack space for recursion)</description></item>
/// <item><description>Best case   : O(n log n) - Sorted input still requires one full pass</description></item>
/// <item><description>Average case: O(n log n log n)</description></item>
/// <item><description>Worst case  : O(n log n log n)</description></item>
/// <item><description>Comparisons : O(n log n log n)</description></item>
/// <item><description>Swaps       : O(n log n log n) worst case</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://sourceforge.net/p/forth-4th/wiki/Circle%20sort/</para>
/// </remarks>
public static class CircleSort
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
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use for element comparisons.</typeparam>
    /// <typeparam name="TContext">The type of sort context.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="comparer">The comparer to use for element comparisons.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation.</param>
    public static void Sort<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);

        var pass = 1;
        bool swapped;
        do
        {
            context.OnPhase(SortPhase.CircleSortPass, pass, 0, 0);
            swapped = CircleSortCore(s, 0, s.Length - 1, 0);
            pass++;
        }
        while (swapped);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool CircleSortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int lo, int hi, int depth)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (lo >= hi)
            return false;

        s.Context.OnPhase(SortPhase.CircleSortInterval, lo, hi, depth);

        var swapped = false;
        var low = lo;
        var high = hi;
        var midOffset = (hi - lo) / 2;

        while (low < high)
        {
            if (s.IsGreaterAt(low, high))
            {
                s.Swap(low, high);
                swapped = true;
            }
            low++;
            high--;
        }

        // When the two pointers meet (odd-length range), compare the middle element with its right neighbor
        if (low == high && s.IsGreaterAt(low, high + 1))
        {
            s.Swap(low, high + 1);
            swapped = true;
        }

        swapped |= CircleSortCore(s, lo, lo + midOffset, depth + 1);
        swapped |= CircleSortCore(s, lo + midOffset + 1, hi, depth + 1);

        return swapped;
    }
}
