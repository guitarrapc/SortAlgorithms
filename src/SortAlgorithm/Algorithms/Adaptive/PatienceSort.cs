using System.Buffers;
using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// PatienceSortはトランプのソリティアゲーム（ペイシェンス）から着想を得たソートアルゴリズムです。
/// 要素を山札（パイル）に配り、次にmin-heapを使ったk-wayマージで整列済み列を構築します。
/// パイル数は最長増加部分列（LIS）の長さに等しく、パイル構築・マージどちらもO(n log k)で動作します。
/// <br/>
/// Patience Sort is inspired by the patience card game (solitaire). Elements are dealt into piles,
/// then a k-way merge using a min-heap produces the sorted output.
/// The number of piles equals the length of the Longest Increasing Subsequence (LIS) of the input.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Patience Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Phase 1 - Pile Building (Dealing):</strong> For each element, find the leftmost pile whose top ≥ the element using binary search (lower bound).
/// Place the element on that pile (it becomes the new top). If no pile qualifies, start a new pile.
/// This maintains the invariant: pile tops are in non-decreasing order from left to right.</description></item>
/// <item><description><strong>Binary Search Invariant:</strong> Pile tops always form a non-decreasing sequence.
/// Binary search finds the leftmost pile with top ≥ element in O(log k) per element, where k is the current pile count.</description></item>
/// <item><description><strong>Pile Structure (Stack):</strong> Each pile is a LIFO stack tracked via a linked-list stored in a flat int[] buffer.
/// Newer tops are always ≤ older tops (guaranteed by the dealing rule), so popping a pile yields elements in non-decreasing order.</description></item>
/// <item><description><strong>Phase 2 - K-Way Merge:</strong> A min-heap of size k holds the current top index of each pile.
/// Repeatedly extract the global minimum, pop it from its pile, and re-heapify in O(log k) per step.
/// The result is written to an auxiliary buffer, then copied back to the original span.</description></item>
/// <item><description><strong>Number of Piles = LIS Length:</strong> The number of piles equals the length of the Longest Increasing Subsequence.
/// Best case (already sorted): 1 pile → O(n). Worst case (reverse sorted): n piles → O(n log n).</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Insertion / Merge hybrid</description></item>
/// <item><description>Stable      : No (relative order of equal elements may not be preserved)</description></item>
/// <item><description>In-place    : No (requires O(n) auxiliary space)</description></item>
/// <item><description>Best case   : O(n) - Already sorted input produces 1 pile; merge is trivial</description></item>
/// <item><description>Average case: Θ(n log k) - where k is the average LIS length</description></item>
/// <item><description>Worst case  : O(n log n) - Reverse-sorted input produces n piles</description></item>
/// <item><description>Comparisons : O(n log k) - binary search per element + heap operations</description></item>
/// <item><description>Writes      : O(n) - each element written once to merge buffer, then copied back</description></item>
/// <item><description>Space       : O(n) - four flat ArrayPool buffers: pile links, pile heads, heap, merge output</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Patience_sorting</para>
/// </remarks>
public static class PatienceSort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;        // Main input array
    private const int BUFFER_AUX = 1;         // Auxiliary merge buffer

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
    /// <typeparam name="TComparer">The type of the comparer. Must implement <see cref="IComparer{T}"/>.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="comparer">The comparer used to determine the order of elements.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation.</param>
    public static void Sort<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        SortCore(s, comparer, context);
    }

    private static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = s.Length;

        var mergeBuffer = ArrayPool<T>.Shared.Rent(n);
        var pileLinksBuffer = ArrayPool<int>.Shared.Rent(n);
        var pileHeadsBuffer = ArrayPool<int>.Shared.Rent(n);
        var heapBuffer = ArrayPool<int>.Shared.Rent(n);
        try
        {
            // pileLinks[i] = index of the element below element i in its pile (-1 if bottom of pile)
            var pileLinks = pileLinksBuffer.AsSpan(0, n);
            // pileHeads[p] = index in s of the current top element of pile p (-1 if pile unused)
            var pileHeads = pileHeadsBuffer.AsSpan(0, n);
            pileLinks.Fill(-1);
            pileHeads.Fill(-1);

            var pileCount = 0;

            // Phase 1: Build piles (dealing phase)
            // Invariant: s[pileHeads[0]] <= s[pileHeads[1]] <= ... <= s[pileHeads[pileCount-1]]
            context.OnPhase(SortPhase.PatienceSortDeal);
            for (var i = 0; i < n; i++)
            {
                // Binary search: lower bound of s[i] in pile tops.
                // Find the leftmost pile p such that s[pileHeads[p]] >= s[i].
                var lo = 0;
                var hi = pileCount;
                while (lo < hi)
                {
                    var mid = lo + (hi - lo) / 2;
                    if (s.IsLessAt(pileHeads[mid], i))   // s[pileHeads[mid]] < s[i]
                        lo = mid + 1;
                    else
                        hi = mid;
                }

                if (lo == pileCount)
                {
                    // No qualifying pile found - start a new pile
                    pileHeads[pileCount] = i;
                    pileLinks[i] = -1;
                    pileCount++;
                }
                else
                {
                    // Push element i onto pile lo (i becomes the new top)
                    pileLinks[i] = pileHeads[lo];
                    pileHeads[lo] = i;
                }
            }

            // Phase 2: K-way merge using a min-heap of pile tops
            context.OnPhase(SortPhase.PatienceSortMerge, pileCount);

            var merge = new SortSpan<T, TComparer, TContext>(mergeBuffer.AsSpan(0, n), context, comparer, BUFFER_AUX);
            var heap = heapBuffer.AsSpan(0, pileCount);
            for (var i = 0; i < pileCount; i++) heap[i] = i;

            // Build min-heap (Floyd's bottom-up heapify: O(k))
            for (var i = pileCount / 2 - 1; i >= 0; i--)
                HeapifyDown(heap, i, pileCount, pileHeads, s);

            var heapSize = pileCount;
            for (var i = 0; i < n; i++)
            {
                // The root of the min-heap is the pile with the smallest current top
                var topPile = heap[0];
                var topIdx = pileHeads[topPile];

                // Write the minimum to the merge buffer
                merge.Write(i, s.Read(topIdx));

                // Pop the top element from the pile
                pileHeads[topPile] = pileLinks[topIdx];

                if (pileHeads[topPile] < 0)
                {
                    // Pile exhausted - remove it from the heap
                    heap[0] = heap[--heapSize];
                    if (heapSize > 0)
                        HeapifyDown(heap, 0, heapSize, pileHeads, s);
                }
                else
                {
                    // Pile still has elements - restore heap property at root
                    HeapifyDown(heap, 0, heapSize, pileHeads, s);
                }
            }

            // Copy sorted result back to the original span
            merge.CopyTo(0, s, 0, n);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(mergeBuffer, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
            ArrayPool<int>.Shared.Return(pileLinksBuffer);
            ArrayPool<int>.Shared.Return(pileHeadsBuffer);
            ArrayPool<int>.Shared.Return(heapBuffer);
        }
    }

    /// <summary>
    /// Sifts the element at position <paramref name="i"/> down the min-heap to restore the heap property.
    /// The comparison key for each heap entry is the value at the top of the corresponding pile: s[pileHeads[heap[i]]].
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void HeapifyDown<T, TComparer, TContext>(Span<int> heap, int i, int size, Span<int> pileHeads, SortSpan<T, TComparer, TContext> s)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (true)
        {
            var smallest = i;
            var left = 2 * i + 1;
            var right = 2 * i + 2;

            if (left < size && s.IsLessAt(pileHeads[heap[left]], pileHeads[heap[smallest]]))
                smallest = left;
            if (right < size && s.IsLessAt(pileHeads[heap[right]], pileHeads[heap[smallest]]))
                smallest = right;

            if (smallest == i) break;

            (heap[i], heap[smallest]) = (heap[smallest], heap[i]);
            i = smallest;
        }
    }
}
