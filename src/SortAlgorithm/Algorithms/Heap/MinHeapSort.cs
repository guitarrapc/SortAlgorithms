using SortAlgorithm.Contexts;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 配列から、常に最小の要素をルートにもつヒープ（最小二分ヒープ）を作成します（この時点で不安定）。
/// その後、ルート（最小値）を末尾へ移し、末尾にあった要素をルートから sift-down してヒープサイズを縮小します。
/// これにより降順の配列が得られるため、最後に反転して昇順にします。
/// <br/>
/// Builds a min-heap (binary heap) from the array where the root always contains the minimum element (which is inherently unstable).
/// Then, moves the root (minimum) to the end, sifts the former last element down from the root, and reduces the heap size.
/// This produces a descending array, which is then reversed to obtain ascending order.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct MinHeapSort:</strong></para>
/// <list type="number">
/// <item><description><strong>Min-Heap Property Maintenance:</strong> Every parent node must be less than or equal to its children.
/// For array index i, left child is at 2i+1 and right child is at 2i+2. This implementation correctly maintains this property through iterative heapify operations.</description></item>
/// <item><description><strong>Build Heap Phase:</strong> Floyd's construction sifts down every internal node, from the last one (n/2 - 1) to the root, in O(n) time.</description></item>
/// <item><description><strong>Extract Min Phase:</strong> Repeatedly reads the root (minimum) and the last element, sifts the last element down via Heapify, then writes the min to the end.
/// This produces a descending order because the smallest elements are moved to the end first.</description></item>
/// <item><description><strong>Reverse Phase:</strong> After all extractions, the array is in descending order. A simple O(n) reversal converts it to ascending order.</description></item>
/// <item><description><strong>Sift-Down:</strong> Top-down in both phases, as in <see cref="HeapSort"/>. At each level the two children are compared, then the sifted value is compared with the smaller one;
/// the sift stops as soon as the value is not larger. The value is held aside while smaller children move up into the hole, and it is written once where it stops.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Heap / Selection</description></item>
/// <item><description>Stable      : No (heap operations do not preserve relative order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space)</description></item>
/// <item><description>Best case   : Θ(n) - when all keys are equal, every sift stops at its first level; with distinct keys the best case is Θ(n log n)</description></item>
/// <item><description>Average case: Θ(n log n) - Build heap O(n) + n-1 extractions with O(log n) heapify each + O(n) reversal</description></item>
/// <item><description>Worst case  : O(n log n) - Guaranteed upper bound regardless of input distribution</description></item>
/// <item><description>Comparisons : ~2n log n - two per level: one between the children, one between the sifted value and the smaller child</description></item>
/// <item><description>Swaps       : ~n/2 - Only from the final reversal step; extraction uses Read+Heapify+Write pattern with 0 swaps</description></item>
/// <item><description>Index Reads : Θ(n log n) - every sift-down reads the children at each level it descends, plus n reads for the final reversal</description></item>
/// <item><description>Index Writes: Θ(n log n) - every sift-down writes each level it descends, plus the reversal's n writes</description></item>
/// <item><description>Space       : O(1) - the heap occupies the input span itself</description></item>
/// </list>
/// <para><strong>Cache behaviour:</strong> poor locality — the heap's parent/child index arithmetic jumps through memory, so extraction misses cache far more than a sequential algorithm.</para>
/// <para><strong>Difference from <see cref="HeapSort"/>:</strong></para>
/// <list type="bullet">
/// <item><description>Uses min-heap (parent ≤ children) instead of max-heap (parent ≥ children)</description></item>
/// <item><description>Extraction phase produces descending order, requiring a final O(n) reversal step</description></item>
/// <item><description>Total work is O(n log n) + O(n) = O(n log n), same asymptotic complexity as HeapSort</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>Uses iterative heapify (loop) instead of recursive for better performance and stack safety</description></item>
/// <item><description>Builds min-heap then reverses for ascending sort (max-heap directly produces ascending order without reversal)</description></item>
/// <item><description>As in <see cref="HeapSort"/>, the smaller child is selected by adding the comparison result to the index (except for float, double and Half),
/// a node already in heap order is left in place during the build, and the build runs in its own method</description></item>
/// <item><description>Despite O(n log n) guarantee, often slower than HeapSort in practice due to extra reversal step and poor cache performance</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Heapsort</para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Heap_(data_structure)</para>
/// </remarks>
public static class MinHeapSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by MinHeapSortTests, which derives from SortTestsBase.</remarks>
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
    /// <typeparam name="TComparer">The type of comparer to use for element comparisons.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="first">The inclusive start index of the range to sort.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = last - first;

        // Build min-heap
        s.Context.OnPhase(SortPhase.HeapBuild, first, last - 1);
        BuildHeap(s, first, last);

        // Extract elements from min-heap (produces descending order)
        var totalExtractions = n - 1;
        for (var i = last - 1; i > first; i--)
        {
            s.Context.OnPhase(SortPhase.HeapExtract, last - i, totalExtractions);
            s.Context.OnRole(first, BUFFER_MAIN, RoleType.CurrentMin);

            // Save min (root) and the last element, then sift down the last element
            var min = s.Read(first);
            var lastVal = s.Read(i);
            Heapify(s, first, i - first, first, lastVal);
            s.Write(i, min);

            s.Context.OnRole(first, BUFFER_MAIN, RoleType.None);
        }

        // Reverse the range to convert descending order to ascending order
        s.Context.OnPhase(SortPhase.Reverse, first, last - 1);
        Reverse(s, first, last - 1);
    }

    /// <summary>
    /// Builds a min-heap on [first..last) by sifting down every internal node, from the last one to the root.
    /// </summary>
    /// <remarks>
    /// Kept out of line so that each phase gets its own inlining budget, as in <see cref="HeapSort"/>.
    /// </remarks>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void BuildHeap<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = last - first;
        for (var i = first + n / 2; i-- > first;)
        {
            // Every node in this range has a left child. A node already in heap order is left in place,
            // so it is neither read out nor written back.
            var child = SmallerChild(s, 2 * (i - first) + 1 + first, last);
            if (!s.IsLessAt(child, i, out var childValue, out var value)) continue;
            s.Write(i, childValue);
            Heapify(s, child, n, first, value);
        }
    }

    /// <summary>
    /// Restores the min-heap property for a subtree rooted at the specified index using iterative sift-down.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of the comparer.</typeparam>
    /// <typeparam name="TContext">The type of the sort context.</typeparam>
    /// <param name="s">The SortSpan containing the elements and context for tracking operations.</param>
    /// <param name="root">The index of the root node of the subtree to heapify.</param>
    /// <param name="size">The size of the heap (number of elements to consider).</param>
    /// <param name="offset">The starting index offset for the heap within the span.</param>
    /// <param name="value">The value to sift down from the root position. Passed by the caller to avoid a redundant Read inside this method.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Heapify<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int root, int size, int offset, T value)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var hole = root;

        if (typeof(T) == typeof(float) || typeof(T) == typeof(double) || typeof(T) == typeof(Half))
        {
            if (s.IsNaturalFloatingNaN(value))
            {
                while (true)
                {
                    var left = 2 * (hole - offset) + 1 + offset;

                    if (left >= offset + size) break;

                    // Find smaller child
                    var smallest = SmallerChild(s, left, offset + size);

                    // If value is already <= smallest child, min-heap property is satisfied
                    if (s.IsValueLessOrEqualWithNaNState(value, smallest, true)) break;

                    // Move smaller child up to fill the hole
                    s.Write(hole, s.Read(smallest));
                    hole = smallest;
                }
            }
            else
            {
                while (true)
                {
                    var left = 2 * (hole - offset) + 1 + offset;

                    if (left >= offset + size) break;

                    // Find smaller child
                    var smallest = SmallerChild(s, left, offset + size);

                    // If value is already <= smallest child, min-heap property is satisfied
                    if (s.IsValueLessOrEqualWithNaNState(value, smallest, false)) break;

                    // Move smaller child up to fill the hole
                    s.Write(hole, s.Read(smallest));
                    hole = smallest;
                }
            }
        }
        else
        {
            while (true)
            {
                var left = 2 * (hole - offset) + 1 + offset;

                if (left >= offset + size) break;

                // Find smaller child
                var smallest = SmallerChild(s, left, offset + size);

                // If value is already <= smallest child, min-heap property is satisfied
                if (s.IsValueLessOrEqual(value, smallest)) break;

                // Move smaller child up to fill the hole
                s.Write(hole, s.Read(smallest));
                hole = smallest;
            }
        }

        s.Write(hole, value);
    }

    /// <summary>
    /// Returns the index of the smaller child of a node whose left child is <paramref name="left"/>.
    /// The right child exists only when it lies below <paramref name="end"/>.
    /// </summary>
    /// <remarks>
    /// Only the bounds check branches. The comparison result is added to the index, as in Rust's heapsort,
    /// because a branch on it is mispredicted about half the time on random input.
    /// Floating-point types keep the branch for the reason given in <see cref="HeapSort"/>'s LargerChild.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int SmallerChild<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (left + 1 >= end) return left;
        if (typeof(T) == typeof(float) || typeof(T) == typeof(double) || typeof(T) == typeof(Half))
        {
            if (s.IsLessAt(left + 1, left)) left++;
            return left;
        }
        return left + (s.IsLessAt(left + 1, left) ? 1 : 0);
    }

    /// <summary>
    /// Reverses the elements in the range [lo..hi] (inclusive) in-place.
    /// Used to convert the descending order produced by min-heap extraction into ascending order.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Reverse<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int lo, int hi)
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
}
