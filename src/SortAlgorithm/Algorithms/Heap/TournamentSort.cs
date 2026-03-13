using SortAlgorithm.Contexts;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 入力配列からトーナメントツリー（勝者木）を構築し、最小値を繰り返し取り出すことで昇順ソートを行います。
/// ヒープソートと同じ選択系ソートファミリーですが、ヒープの代わりに勝者木（外部ソートで使われる木構造）を用います。
/// <br/>
/// Builds a tournament tree (winner tree) from the input array, then repeatedly extracts the minimum element to produce an ascending sort.
/// This is the same Selection-family as HeapSort, but uses a winner tree — the structure found in external sort merge phases — instead of a heap.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Tournament Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Tournament Tree Structure:</strong> A complete binary tree with <c>size</c> leaves (next power of two ≥ n).
/// Each leaf stores the original span index of one element. Each internal node stores the index of the winner (smaller element) of its two children.
/// Padding leaves beyond n are set to <c>int.MaxValue</c> (sentinel for "no element").</description></item>
/// <item><description><strong>Build Phase:</strong> Bottom-up construction starting from the last internal node down to the root.
/// Each internal node is set to the winner of its two children. Total cost: O(n).</description></item>
/// <item><description><strong>Extract Phase:</strong> The root always holds the index of the current global minimum.
/// The minimum is swapped to its final sorted position, the now-sorted leaf is replaced with the sentinel,
/// and the tournament is replayed only along the two affected leaf-to-root paths in O(log n).</description></item>
/// <item><description><strong>In-Place Strategy:</strong> After extracting the minimum at index <c>p</c> for sorted position <c>s</c>:
/// (1) Swap span[p] with span[s] if p ≠ s. (2) Set leaf[s] = Sentinel and replay its path to root.
/// (3) If p ≠ s, replay leaf[p]'s path to root because the value at that position changed after the swap.
/// This keeps the tree consistent without allocating a separate output buffer.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Heap / Selection</description></item>
/// <item><description>Stable      : No (swaps during extraction do not preserve relative order)</description></item>
/// <item><description>In-place    : Yes (O(1) sorted output in span; O(n) auxiliary for the winner tree)</description></item>
/// <item><description>Best case   : Ω(n log n)</description></item>
/// <item><description>Average case: Θ(n log n)</description></item>
/// <item><description>Worst case  : O(n log n)</description></item>
/// <item><description>Comparisons : ~n log n — one winner comparison per node in each replay path</description></item>
/// <item><description>Swaps       : ≤ n-1 — at most one swap per extraction step</description></item>
/// <item><description>Auxiliary   : O(n) — integer winner tree allocated via ArrayPool</description></item>
/// <item><description>Cache       : Poor — tree traversal jumps through non-sequential memory</description></item>
/// </list>
/// <para><strong>Difference from HeapSort:</strong></para>
/// <list type="bullet">
/// <item><description>Uses an explicit winner-tree (separate int[] array) instead of the in-place max-heap.</description></item>
/// <item><description>Extracts the minimum each round (ascending sort) rather than the maximum.</description></item>
/// <item><description>Requires O(n) auxiliary memory; HeapSort is fully in-place (O(1) aux).</description></item>
/// <item><description>Replay cost is exactly <c>log2(size)</c> comparisons per extraction vs. ~2 log n for sift-down.</description></item>
/// <item><description>Historically used as the merge-phase structure in external sorting (tape/disk merge).</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Tournament_sort</para>
/// </remarks>
public static class TournamentSort
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

        // size = next power of two >= n (tree leaf count; pads to a full binary tree)
        var size = (int)BitOperations.RoundUpToPowerOf2((uint)n);
        var treeSize = size << 1; // 2 * size; tree is 1-indexed, tree[0] unused

        const int Sentinel = int.MaxValue; // "no element" / infinity marker

        int[]? rentedArray = null;
        try
        {
            var tree = (rentedArray = ArrayPool<int>.Shared.Rent(treeSize)).AsSpan(0, treeSize);

            // Build
            // Leaves: tree[size + i] = i (span-relative index) for i in [0, n),
            //         tree[size + i] = Sentinel for i in [n, size).
            for (var i = 0; i < n; i++)
                tree[size + i] = i;
            for (var i = n; i < size; i++)
                tree[size + i] = Sentinel;

            // Internal nodes built bottom-up (index size-1 down to 1).
            for (var i = size - 1; i >= 1; i--)
                tree[i] = TournamentWinner(s, first, tree[i << 1], tree[(i << 1) | 1], Sentinel);

            // Extract
            // Repeat n-1 times: extract min → place at sorted position → replay two paths.
            for (var sortedCount = 0; sortedCount < n - 1; sortedCount++)
            {
                var winner = tree[1]; // root = index of current global minimum
                s.Context.OnRole(first + winner, BUFFER_MAIN, RoleType.CurrentMin);

                // Move the minimum to its final sorted position.
                if (winner != sortedCount)
                    s.Swap(first + winner, first + sortedCount);

                s.Context.OnRole(first + winner, BUFFER_MAIN, RoleType.None);
                if (winner != sortedCount)
                    s.Context.OnRole(first + sortedCount, BUFFER_MAIN, RoleType.None);

                // Retire the sorted leaf and restore the tournament tree.
                tree[size + sortedCount] = Sentinel;
                ReplayPath(s, first, tree, size + sortedCount, Sentinel);

                // When winner ≠ sortedCount the swap changed span[winner]; replay its path too.
                if (winner != sortedCount)
                    ReplayPath(s, first, tree, size + winner, Sentinel);
            }
        }
        finally
        {
            if (rentedArray != null)
                ArrayPool<int>.Shared.Return(rentedArray);
        }
    }

    /// <summary>
    /// Returns the index of the winner (smaller element) between two tournament contestants.
    /// Returns the non-sentinel index when one contestant is exhausted.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int TournamentWinner<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int a, int b, int sentinel)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (a == sentinel) return b;
        if (b == sentinel) return a;
        return s.Compare(first + a, first + b) <= 0 ? a : b;
    }

    /// <summary>
    /// Replays the tournament from a changed leaf up to the root,
    /// recomputing each internal node along the path.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ReplayPath<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, Span<int> tree, int leaf, int sentinel)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        for (var node = leaf >> 1; node >= 1; node >>= 1)
        {
            tree[node] = TournamentWinner(s, first, tree[node << 1], tree[(node << 1) | 1], sentinel);
        }
    }
}
