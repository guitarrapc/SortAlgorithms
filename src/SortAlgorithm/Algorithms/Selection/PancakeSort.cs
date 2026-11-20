using SortAlgorithm.Contexts;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 未ソート部分から最大の要素を見つけ、それを先頭にフリップ（逆転）し、次に先頭から未ソート部分の最後の要素までフリップして配置します。
/// このプロセスを繰り返すことで、配列をソートします。
/// <br/>
/// Finds the maximum element in the unsorted portion, flips (reverses) the array to bring it to the front, and then flips the array up to the last unsorted element to place the maximum element in its correct position. This process is repeated until the array is sorted.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Pancake Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Maximum Finding:</strong> For each unsorted subarray [0..currentSize), find the index of the maximum element.
/// This requires comparing all elements in the range, performing (currentSize - 1) comparisons per iteration.
/// Among equal maxima the first is taken, which keeps the first flip short, except that the last element also wins a tie:
/// a copy of the maximum that already ends the subarray is in its final position and costs no flip.</description></item>
/// <item><description><strong>Two-Flip Positioning:</strong> To move the maximum element to its correct position at (currentSize - 1):
/// <list type="bullet">
/// <item>First flip: Reverse [0..maxIndex] to bring the maximum to position 0</item>
/// <item>Second flip: Reverse [0..currentSize-1] to place the maximum at its final position</item>
/// </list>
/// If the maximum is already at (currentSize - 1), skip both flips.</description></item>
/// <item><description><strong>Range Reduction:</strong> After placing the maximum, reduce the unsorted range by 1 and repeat.
/// The algorithm terminates when currentSize reaches 1 (entire array sorted).</description></item>
/// <item><description><strong>Flip Correctness:</strong> Each flip reverses a subarray by swapping elements symmetrically:
/// swap(start, end), swap(start+1, end-1), ..., until start ≥ end. This preserves all elements while changing their order.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Selection (selects maximum and places it in correct position)</description></item>
/// <item><description>Stable      : No (flipping changes relative order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space)</description></item>
/// <item><description>Best case   : Θ(n²) - Already sorted, still must find maximum in each iteration (no flips needed)</description></item>
/// <item><description>Average case: Θ(n²) - Comparisons are always n(n-1)/2; close to 2n flips, since the maximum ends its prefix only by chance</description></item>
/// <item><description>Worst case  : Θ(n²) - At most 2n - 3 flips; each pass flips at most the whole remaining prefix</description></item>
/// <item><description>Comparisons : Θ(n²) - Exactly n(n-1)/2: one scan of each prefix of n, n-1, ..., 2 elements, regardless of input</description></item>
/// <item><description>Swaps       : O(n²) - A flip of k elements exchanges ⌊k/2⌋ pairs; 0 on sorted input, duplicates included</description></item>
/// <item><description>Index Reads : Θ(n²) - Each comparison reads the scanned element and the candidate maximum; each swap adds two</description></item>
/// <item><description>Index Writes: O(n²) - Two per swap; nothing else writes</description></item>
/// <item><description>Space       : O(1) - the flips reverse ranges inside the input span</description></item>
/// </list>
/// <para><strong>Note:</strong> The Pancake Sorting Problem asks for the minimum number of flips to sort any array.
/// This implementation uses a simple greedy approach (not necessarily optimal) that guarantees at most 2n - 3 flips.
/// The optimal bound is between (15n/14) and (18n/11) flips for worst-case inputs.</para>
/// <para><strong>Implementation Notes:</strong></para>
/// <para>Without an observer a flip is <see cref="MemoryExtensions.Reverse{T}(Span{T})"/>, which the runtime vectorizes for primitive types.
/// An observed sort performs the same exchanges as one swap per pair.</para>
/// <para><strong>History:</strong></para>
/// <para>The problem was posed in 1975 as Problem E2569 of the American Mathematical Monthly, under the pseudonym Harry Dweighter
/// (Jacob E. Goodman). Gates and Papadimitriou (1979), often cited for pancake sorting, came later and proved bounds on the
/// number of flips.</para>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Pancake_sorting</para>
/// <para>Gates, W. H.; Papadimitriou, C. H. (1979). "Bounds for Sorting by Prefix Reversal". Discrete Mathematics 27 (1): 47-57. https://doi.org/10.1016/0012-365X(79)90068-2</para>
/// </remarks>
public static class PancakeSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by PancakeSortTests, which derives from SortTestsBase.</remarks>
    public static bool IsStable => false;

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
    /// Sorts the subrange [first..last) using the provided comparer and sort context.
    /// This is the full-control version with explicit TContext type parameter.
    /// </summary>
    public static void Sort<T, TComparer, TContext>(Span<T> span, int first, int last, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        ArgumentOutOfRangeException.ThrowIfNegative(first);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(last, span.Length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(first, last);

        if (last - first <= 1) return;

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        SortCore(s, first, last);
    }

    /// <summary>
    /// Sorts the subrange [first..last) using the provided sort context.
    /// This overload accepts a SortSpan directly for use by other algorithms that already have a SortSpan instance.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="first">The inclusive start index of the range to sort.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // A one-element prefix is already in place, so the last pass flips a two-element prefix.
        for (var currentSize = last; currentSize > first + 1; currentSize--)
        {
            var end = currentSize - 1;
            s.Context.OnPhase(SortPhase.PancakeFindMax, first, end);
            var maxIndex = first;
            s.Context.OnRole(maxIndex, BUFFER_MAIN, RoleType.CurrentMax);
            for (var i = first + 1; i < end; i++)
            {
                if (s.IsGreaterAt(i, maxIndex))
                {
                    s.Context.OnRole(maxIndex, BUFFER_MAIN, RoleType.None);
                    maxIndex = i;
                    s.Context.OnRole(maxIndex, BUFFER_MAIN, RoleType.CurrentMax);
                }
            }

            // The first maximum keeps the first flip short, but a copy that already ends the prefix is in place, so the last element also wins a tie.
            if (s.IsGreaterOrEqualAt(end, maxIndex))
            {
                s.Context.OnRole(maxIndex, BUFFER_MAIN, RoleType.None);
                continue;
            }
            s.Context.OnRole(maxIndex, BUFFER_MAIN, RoleType.None);

            // Move the maximum element to the front, then flip to the right position
            Flip(s, first, maxIndex);
            Flip(s, first, end);
        }
    }

    /// <summary>
    /// Reverses the elements in the span from start to end.
    /// </summary>
    /// <remarks>
    /// Without an observer the BCL reverse performs the same exchanges, vectorized for primitive types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Flip<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int start, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (typeof(TContext) == typeof(NullContext))
        {
            s.RawSpan.Slice(start, end - start + 1).Reverse();
            return;
        }

        while (start < end)
        {
            s.Swap(start, end);
            start++;
            end--;
        }
    }
}
