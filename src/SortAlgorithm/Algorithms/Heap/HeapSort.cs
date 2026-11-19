using SortAlgorithm.Contexts;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 配列から、常に最大の要素をルートにもつヒープ（二分ヒープ）を作成します（この時点で不安定）。
/// その後、ルート（最大値）と末尾要素を読み取り、末尾要素をヒープに sift-down してからルート値を末尾に書き込みます。これを繰り返すことで、ヒープの最大値が常にルートに保たれ、ソート済み配列に追加されることで自然とソートが行われます。
/// <br/>
/// Builds a heap (binary heap) from the array where the root always contains the maximum element (which is inherently unstable).
/// Then, the root (maximum) and the last element are read; the last element is sifted down into the heap, and the maximum is written to the end. Repeating this process ensures that the maximum value in the heap is always at the root, allowing elements to be naturally sorted as they are moved to the sorted array.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Heapsort:</strong></para>
/// <list type="number">
/// <item><description><strong>Heap Property Maintenance:</strong> For a max-heap, every parent node must be greater than or equal to its children.
/// For array index i, left child is at 2i+1 and right child is at 2i+2. This implementation maintains this property through the iterative sift-down.</description></item>
/// <item><description><strong>Build Heap Phase:</strong> Floyd's construction (Treesort 3, 1964) sifts down every internal node, from the last one (n/2 - 1) to the root.
/// A sift costs at most twice the node's height, so the phase runs in O(n), where Williams' original construction by n insertions takes O(n log n).</description></item>
/// <item><description><strong>Extract Max Phase:</strong> Repeatedly reads the root (maximum) and the last element, sifts the last element down from the root, then writes the max to the end.
/// No swap is needed; this reduces memory writes and eliminates all Swap operations from the extraction phase.
/// This phase performs n-1 extractions, each requiring O(log n) heapify operations, totaling O(n log n).</description></item>
/// <item><description><strong>Sift-Down:</strong> Top-down, as in Knuth's Algorithm H. At each level the two children are compared, then the sifted value is compared with the larger one;
/// the sift stops as soon as the value is not smaller. Both phases use it. The sifted value is held aside while larger children move up into the hole, and it is written once where it stops.
/// Descending to a leaf first and climbing back up is <see cref="BottomupHeapSort"/>'s defining step, so it is not used here.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Heap / Selection</description></item>
/// <item><description>Stable      : No (heap operations do not preserve relative order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space)</description></item>
/// <item><description>Best case   : Θ(n) - when all keys are equal, every sift stops at its first level; with distinct keys the best case is Θ(n log n)</description></item>
/// <item><description>Average case: Θ(n log n) - Build heap O(n) + n-1 extractions with O(log n) heapify each</description></item>
/// <item><description>Worst case  : O(n log n) - Guaranteed upper bound regardless of input distribution</description></item>
/// <item><description>Comparisons : ~2n log n - two per level: one between the children, one between the sifted value and the larger child</description></item>
/// <item><description>Swaps       : 0 - No swaps; extraction reads root+last, sifts last down, writes root to end</description></item>
/// <item><description>Index Reads : Θ(n log n) - every sift-down reads the children at each level it descends</description></item>
/// <item><description>Index Writes: Θ(n log n) - every sift-down writes each level it descends, plus n root-to-end writes</description></item>
/// <item><description>Space       : O(1) - the heap occupies the input span itself</description></item>
/// </list>
/// <para><strong>Cache behaviour:</strong> poor locality — the heap's parent/child index arithmetic jumps through memory, so extraction misses cache far more than a sequential algorithm.</para>
/// <para><strong>Why "Heap / Selection" Family?:</strong></para>
/// <para>
/// HeapSort belongs to the Selection sort family. Like Selection Sort, it repeatedly
/// selects the maximum element and places it at the end of the sorted portion.
/// The key difference is the selection mechanism:
/// </para>
/// <list type="bullet">
/// <item><description>Selection Sort: Linear search O(n) to find maximum</description></item>
/// <item><description>Heap Sort: Heap structure O(log n) to extract maximum</description></item>
/// </list>
/// <para>
/// Thus, HeapSort is essentially an optimized Selection Sort using a heap data structure,
/// improving time complexity from O(n²) to O(n log n).
/// </para>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>Uses iterative heapify (loop) instead of recursive for better performance and stack safety</description></item>
/// <item><description>Builds max-heap for ascending sort (min-heap would produce descending order)</description></item>
/// <item><description>The larger child is selected by adding the comparison result to the index rather than branching on it, as Rust's heapsort does.
/// The comparisons and their order are unchanged. float, double and Half keep the branch, because their NaN-aware comparison branches internally.</description></item>
/// <item><description>The build runs in its own method so that each phase's sift loop gets its own JIT inlining budget.</description></item>
/// <item><description>During the build, a node already in heap order is left in place without being read out and written back, as in libc++'s <c>__sift_down</c>.</description></item>
/// <item><description>Despite O(n log n) guarantee, often slower than Quicksort in practice due to poor cache performance</description></item>
/// </list>
/// <para><strong>History:</strong></para>
/// <para>Tree selection (<see cref="TournamentSort"/>) came first: Friend described it in 1956, and Williams published heapsort
/// in 1964. Knuth, TAOCP Vol. 3 (2nd ed.) §5.2.3, pp. 143-145, introduces heapsort as the way to remove the -∞ "holes"
/// that tree selection leaves behind, and the separate output area it needs. On p. 144 he credits J. W. J. Williams
/// (CACM 7 (1964), 347-348) with the discovery. That order is how Knuth explains it. No record shows that Williams started
/// from tree selection. So heapsort borrows the idea of selecting the winner with a tree; it is not a direct descendant
/// of tournament sort.</para>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Heapsort</para>
/// </remarks>
public static class HeapSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by HeapSortTests, which derives from SortTestsBase.</remarks>
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

        // Build heap.
        // Read/Write/Compare report through the span they were given, so a caller that hands this core
        // a slice already gets its own Offset and BufferId on every element operation. Index-valued phase
        // parameters and roles must use the same coordinates, or they name a different element.
        // HeapExtract's parameters are step counts rather than indices, so they are reported as-is.
        s.Context.OnPhase(SortPhase.HeapBuild, s.Offset + first, s.Offset + last - 1);
        BuildHeap(s, first, last);

        // Extract elements from heap
        var totalExtractions = n - 1;
        for (var i = last - 1; i > first; i--)
        {
            s.Context.OnPhase(SortPhase.HeapExtract, last - i, totalExtractions);
            s.Context.OnRole(s.Offset + first, s.BufferId, RoleType.CurrentMax);

            // Save max (root) and the last element, then sift down the last element
            var max = s.Read(first);
            var lastVal = s.Read(i);
            Heapify(s, first, i - first, first, lastVal);
            s.Write(i, max);

            s.Context.OnRole(s.Offset + first, s.BufferId, RoleType.None);
        }
    }

    /// <summary>
    /// Builds a max-heap on [first..last) by sifting down every internal node, from the last one to the root.
    /// </summary>
    /// <remarks>
    /// Kept out of line so that each phase gets its own inlining budget. With both phases' sift loops in one method,
    /// the JIT left the comparison helpers as calls for floating-point, decimal and reference types, which measured slower.
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
            // so it is neither read out nor written back (libc++'s __sift_down does the same).
            var child = LargerChild(s, 2 * (i - first) + 1 + first, last);
            if (!s.IsGreaterAt(child, i, out var childValue, out var value)) continue;
            s.Write(i, childValue);
            Heapify(s, child, n, first, value);
        }
    }

    /// <summary>
    /// Restores the heap property for a subtree rooted at the specified index using iterative sift-down.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
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

        if (typeof(T) == typeof(Half))
        {
            if (s.IsNaturalFloatingNaN(value))
            {
                while (true)
                {
                    var left = 2 * (hole - offset) + 1 + offset;

                    if (left >= offset + size) break;

                    // Find larger child
                    var largest = LargerChild(s, left, offset + size);

                    // If value is already >= largest child, heap property is satisfied
                    if (s.IsValueGreaterOrEqualWithNaNState(value, largest, true)) break;

                    // Move larger child up to fill the hole
                    s.Write(hole, s.Read(largest));
                    hole = largest;
                }
            }
            else
            {
                while (true)
                {
                    var left = 2 * (hole - offset) + 1 + offset;

                    if (left >= offset + size) break;

                    // Find larger child
                    var largest = LargerChild(s, left, offset + size);

                    // If value is already >= largest child, heap property is satisfied
                    if (s.IsValueGreaterOrEqualWithNaNState(value, largest, false)) break;

                    // Move larger child up to fill the hole
                    s.Write(hole, s.Read(largest));
                    hole = largest;
                }
            }
        }
        else
        {
            if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
            {
                while (true)
                {
                    var left = 2 * (hole - offset) + 1 + offset;

                    if (left >= offset + size) { s.Write(hole, value); return; }

                    // Find larger child
                    var largest = LargerChild(s, left, offset + size);

                    // If value is already >= largest child, heap property is satisfied
                    if (s.IsValueGreaterOrEqual(value, largest)) { s.Write(hole, value); return; }

                    // Move larger child up to fill the hole
                    s.Write(hole, s.Read(largest));
                    hole = largest;
                    break;
                }
                if (s.IsNaturalFloatingNaN(value))
                {
                    while (true)
                    {
                        var left = 2 * (hole - offset) + 1 + offset;

                        if (left >= offset + size) break;

                        // Find larger child
                        var largest = LargerChild(s, left, offset + size);

                        // If value is already >= largest child, heap property is satisfied
                        if (s.IsValueGreaterOrEqualWithNaNState(value, largest, true)) break;

                        // Move larger child up to fill the hole
                        s.Write(hole, s.Read(largest));
                        hole = largest;
                    }
                }
                else
                {
                    while (true)
                    {
                        var left = 2 * (hole - offset) + 1 + offset;

                        if (left >= offset + size) break;

                        // Find larger child
                        var largest = LargerChild(s, left, offset + size);

                        // If value is already >= largest child, heap property is satisfied
                        if (s.IsValueGreaterOrEqualWithNaNState(value, largest, false)) break;

                        // Move larger child up to fill the hole
                        s.Write(hole, s.Read(largest));
                        hole = largest;
                    }
                }
            }
            else
            {
                while (true)
                {
                    var left = 2 * (hole - offset) + 1 + offset;

                    if (left >= offset + size) break;

                    // Find larger child
                    var largest = LargerChild(s, left, offset + size);

                    // If value is already >= largest child, heap property is satisfied
                    if (s.IsValueGreaterOrEqual(value, largest)) break;

                    // Move larger child up to fill the hole
                    s.Write(hole, s.Read(largest));
                    hole = largest;
                }
            }
        }

        s.Write(hole, value);
    }

    /// <summary>
    /// Returns the index of the larger child of a node whose left child is <paramref name="left"/>.
    /// The right child exists only when it lies below <paramref name="end"/>.
    /// </summary>
    /// <remarks>
    /// Only the bounds check branches. The comparison result is added to the index, as in Rust's heapsort,
    /// because a branch on it is mispredicted about half the time on random input.
    /// Floating-point types keep the branch: their natural comparison branches on NaN internally, so adding
    /// its result removes no misprediction and measured slower.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int LargerChild<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (left + 1 >= end) return left;
        if (typeof(T) == typeof(float) || typeof(T) == typeof(double) || typeof(T) == typeof(Half))
        {
            if (s.IsGreaterAt(left + 1, left)) left++;
            return left;
        }
        return left + (s.IsGreaterAt(left + 1, left) ? 1 : 0);
    }

    /// <summary>Runs a heap fallback without expanding its sift loops into the calling partition sort.</summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void SortFallbackCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var length = last - first;
        SortCore(s.Slice(first, length, s.BufferId), 0, length);
    }
}

/// <summary>
/// 配列から、常に最大の要素をルートにもつヒープ（二分ヒープ）を作成します（この時点で不安定）。
/// その後、ルート要素を末尾要素と交換し、ヒープサイズを縮小してヒープ構造を再維持します。これを繰り返すことで、ヒープの最大値が常にルートに保たれ、ソート済み配列に追加されることで自然とソートが行われます。
/// 最適化なしのテキストブック実装であり、<see cref="HeapSort"/> の参照基準として機能します。
/// <br/>
/// Builds a heap (binary heap) from the array where the root always contains the maximum element (which is inherently unstable).
/// Then, swaps the root (maximum) with the last element, reduces the heap size, and re-establishes the heap property. Repeating this process produces a sorted array.
/// This is the textbook non-optimized implementation that serves as a reference baseline for <see cref="HeapSort"/>.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Heapsort:</strong></para>
/// <list type="number">
/// <item><description><strong>Heap Property Maintenance:</strong> For a max-heap, every parent node must be greater than or equal to its children.
/// For array index i, left child is at 2i+1 and right child is at 2i+2. This implementation correctly maintains this property through the iterative swap-based sift-down.</description></item>
/// <item><description><strong>Build Heap Phase:</strong> The initial heap construction starts from the last non-leaf node (n/2 - 1) and heapifies downward to index 0.
/// Uses the standard top-down sift-down at each node: compare root with both children, swap with the larger child if needed, and repeat until the heap property is satisfied.
/// This bottom-up traversal runs in O(n) time, which is more efficient than the naive O(n log n) top-down construction.</description></item>
/// <item><description><strong>Extract Max Phase:</strong> Repeatedly swaps the root (maximum element) with the last element, reduces the heap size by one, and re-heapifies from the new root.
/// This phase performs n-1 extractions, each requiring O(log n) heapify operations, totaling O(n log n).</description></item>
/// <item><description><strong>Standard Heapify:</strong> Uses the textbook swap-based iterative sift-down.
/// Compares the root with its left and right children, identifies the largest, swaps root with that child if needed, and continues downward until the heap property is satisfied or a leaf is reached.
/// Each level requires 2 comparisons (left vs. current largest, right vs. current largest), giving approximately 2 log n comparisons per call.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Heap / Selection</description></item>
/// <item><description>Stable      : No (heap operations do not preserve relative order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space)</description></item>
/// <item><description>Best case   : Θ(n) - when all keys are equal, every sift stops at its first level; with distinct keys the best case is Θ(n log n)</description></item>
/// <item><description>Average case: Θ(n log n) - Build heap O(n) + n-1 extractions with O(log n) heapify each</description></item>
/// <item><description>Worst case  : O(n log n) - Guaranteed upper bound regardless of input distribution</description></item>
/// <item><description>Comparisons : ~2n log n - 2 comparisons per level during sift-down (left child check + right child check)</description></item>
/// <item><description>Swaps       : ~n log n - One swap per level during sift-down, plus n-1 root-to-end swaps in extraction</description></item>
/// <item><description>Index Reads : Θ(n log n) - every sift-down reads the children at each level it descends</description></item>
/// <item><description>Index Writes: Θ(n log n) - each swap writes two positions, so the writes follow the swap count</description></item>
/// <item><description>Space       : O(1) - the heap occupies the input span itself</description></item>
/// </list>
/// <para><strong>Cache behaviour:</strong> poor locality — the heap's parent/child index arithmetic jumps through memory, so extraction misses cache far more than a sequential algorithm.</para>
/// <para><strong>Difference from <see cref="HeapSort"/>:</strong></para>
/// <list type="bullet">
/// <item><description>Both phases: Sift-down swaps the node with its larger child at each level instead of moving children into a hole (more memory writes)</description></item>
/// <item><description>Extract phase: Uses Swap(root, last) + sift-down instead of Read+Heapify(value)+Write (more memory writes)</description></item>
/// <item><description>Larger child: Selected with a branch on each comparison, not by adding the comparison result to the index</description></item>
/// <item><description>No <c>[MethodImpl(AggressiveInlining)]</c> applied (intentionally unoptimized for reference clarity)</description></item>
/// <item><description>Uses <c>i >= first</c> loop form in build phase (less safe under type changes than the <c>i-- &gt; first</c> idiom)</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>This is the textbook algorithm without any micro-optimizations</description></item>
/// <item><description>Useful as a correctness baseline and educational reference for HeapSort variants</description></item>
/// <item><description>Uses iterative heapify (loop) instead of recursive for stack safety</description></item>
/// <item><description>Builds max-heap for ascending sort (min-heap would produce descending order)</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Heapsort</para>
/// </remarks>
public static class HeapSortNonOptimized
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by HeapSortNonOptimizedTests, which derives from SortTestsBase.</remarks>
    public static bool IsStable => false;

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
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = last - first;

        // Build heap.
        // Index-valued phase parameters and roles use the span's own coordinates, matching what
        // Read/Write/Compare/Swap report. HeapExtract's parameters are step counts, not indices.
        s.Context.OnPhase(SortPhase.HeapBuild, s.Offset + first, s.Offset + last - 1);
        for (var i = first + n / 2 - 1; i >= first; i--)
        {
            Heapify(s, i, n, first);
        }

        // Extract elements from heap
        var totalExtractions = n - 1;
        for (var i = last - 1; i > first; i--)
        {
            s.Context.OnPhase(SortPhase.HeapExtract, last - i, totalExtractions);
            s.Context.OnRole(s.Offset + first, s.BufferId, RoleType.CurrentMax);

            // Move current root to end
            s.Swap(first, i);

            s.Context.OnRole(s.Offset + first, s.BufferId, RoleType.None);

            // Re-heapify the reduced heap (standard sift-down)
            Heapify(s, first, i - first, first);
        }
    }

    /// <summary>
    /// Restores the heap property for a subtree rooted at the specified index using iterative sift-down.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="s">The SortSpan containing the elements and context for tracking operations.</param>
    /// <param name="root">The index of the root node of the subtree to heapify.</param>
    /// <param name="size">The size of the heap (number of elements to consider).</param>
    /// <param name="offset">The starting index offset for the heap within the span.</param>
    /// <remarks>
    /// This method implements the sift-down operation to maintain the max-heap property.
    /// It iteratively compares the parent node with its left and right children, swapping with the larger child if needed,
    /// and continues down the tree until the heap property is satisfied or a leaf node is reached.
    /// <para>Time Complexity: O(log n) - Worst case traverses from root to leaf (height of the tree).</para>
    /// <para>Space Complexity: O(1) - Uses iteration instead of recursion.</para>
    /// </remarks>
    private static void Heapify<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int root, int size, int offset)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (true)
        {
            var largest = root;
            var left = 2 * (root - offset) + 1 + offset;
            var right = 2 * (root - offset) + 2 + offset;

            // If left child is larger than root
            if (left < offset + size && s.IsGreaterAt(left, largest))
            {
                largest = left;
            }

            // If right child is larger than largest so far
            if (right < offset + size && s.IsGreaterAt(right, largest))
            {
                largest = right;
            }

            // If largest is not root, swap and heapify the affected sub-tree
            if (largest != root)
            {
                s.Swap(root, largest);
                root = largest;
            }
            else
            {
                break;
            }
        }
    }
}
