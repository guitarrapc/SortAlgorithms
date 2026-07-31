using SortAlgorithm.Contexts;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// Ford-Johnsonアルゴリズム（マージ挿入ソート）。
/// 比較回数を最小化するために設計されたアルゴリズムで、要素をペア化し、大きい要素を再帰的にソート後、
/// 小さい要素をJacobsthal数列の順序で二分挿入します。
/// <br/>
/// Ford-Johnson algorithm (Merge Insertion Sort).
/// Designed to minimize comparisons by pairing elements, recursively sorting larger elements,
/// then binary-inserting smaller elements in Jacobsthal sequence order.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Merge Insertion Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Pairwise Comparison:</strong> Compare elements in pairs (2i, 2i+1) to determine larger and smaller elements.
/// This guarantees that each pair requires exactly one comparison.</description></item>
/// <item><description><strong>Recursive Sorting of Larger Elements:</strong> Sort the larger elements from each pair recursively.
/// Recursion bottoms out at n &lt;= 2, which is resolved with a single comparison. A larger base case such as
/// binary insertion sort would be cheaper in data movement but would no longer attain the Ford-Johnson comparison bound.</description></item>
/// <item><description><strong>Main Chain Construction:</strong> Build a sorted chain from the larger elements.
/// The smaller elements from each pair are pended for insertion.</description></item>
/// <item><description><strong>Jacobsthal Sequence Insertion:</strong> Insert pended elements in an order determined by the Jacobsthal sequence (1, 3, 5, 11, 21, 43, ...).
/// This ordering minimizes the number of comparisons needed during binary insertion.
/// The sequence ensures that when inserting element at position k, the search space has size 2^m - 1 for some m, optimal for binary search.</description></item>
/// <item><description><strong>Binary Insertion:</strong> For each pended element, use binary search to find its insertion position within the sorted main chain.
/// The search range is bounded above by the current position of the element's pair partner: pairing guarantees the pended element is
/// no greater than its partner, so the insertion point cannot lie beyond it. Combined with the Jacobsthal order this keeps every
/// search range at most 2^m - 1 wide.</description></item>
/// <item><description><strong>Stability:</strong> Not preserved. The Jacobsthal order inserts pended elements out of their original
/// sequence, so a later equal key can end up before an earlier one.</description></item>
/// </list>
/// <para>
/// The comparison count of this implementation matches F(n) = &#x3A3;&#x2096; &#x2308;log&#x2082;(3k/4)&#x2309; exactly
/// (verified exhaustively for n &#x2264; 9), which is the Ford-Johnson worst-case optimum.
/// </para>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Insertion</description></item>
/// <item><description>Stable      : No (the Jacobsthal insertion order can place a later equal key before an earlier one)</description></item>
/// <item><description>In-place    : No (requires O(n) auxiliary space for pended elements and indices)</description></item>
/// <item><description>Best case   : O(n log n) - Optimal comparison count, approaches ceiling(log₂(n!)) comparisons</description></item>
/// <item><description>Average case: O(n log n) - Comparisons remain near-optimal, but data movement is O(n²)</description></item>
/// <item><description>Worst case  : O(n²) - Due to insertion shifts, though comparison count remains O(n log n)</description></item>
/// <item><description>Comparisons : Approximately n⌈log₂ n⌉ - 2^⌈log₂ n⌉ + 1, which is close to the information-theoretic minimum</description></item>
/// <item><description>Writes      : O(n²) - Binary insertion still requires O(n²) element shifts in worst case</description></item>
/// <item><description>Space       : O(n) - Additional arrays needed for tracking indices and pended elements</description></item>
/// </list>
/// <para><strong>Jacobsthal Sequence:</strong></para>
/// <para>J(n) = J(n-1) + 2×J(n-2), with J(0)=0, J(1)=1</para>
/// <para>Sequence: 0, 1, 1, 3, 5, 11, 21, 43, 85, 171, 341, 683, ...</para>
/// <para>The insertion order based on Jacobsthal numbers ensures optimal binary search tree depth.</para>
/// <para><strong>Reference:</strong></para>
/// <para>Paper: Ford, L. R., &amp; Johnson, S. M. (1959). "A Tournament Problem". The American Mathematical Monthly.</para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Merge-insertion_sort</para>
/// </remarks>
public static class MergeInsertionSort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_CHAIN = 1;      // Evolving sorted chain (Ford-Johnson chain construction)

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
    /// <typeparam name="TContext">The type of the sort context.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T, TContext>(Span<T> span, TContext context)
        where T : IComparable<T>
        where TContext : ISortContext
        => Sort(span, new ComparableComparer<T>(), context);

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
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        SortCore(s);
    }

    /// <summary>
    /// Core implementation of Ford-Johnson merge insertion sort algorithm.
    /// Works by reading all elements into an auxiliary index list, performing
    /// the Ford-Johnson algorithm on the index list, then writing back the
    /// sorted order to the span.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of the comparer.</typeparam>
    /// <typeparam name="TContext">The type of the sort context.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = s.Length;

        // The chain buffer only backs visualization writes, so the NullContext fast path
        // never touches it. typeof(TContext) is a JIT constant, so this rental disappears there.
        var needsChain = typeof(TContext) != typeof(NullContext);

        // avoid per-call list/dictionary allocations
        var rentedValues = ArrayPool<T>.Shared.Rent(n);
        var rentedSorted = ArrayPool<int>.Shared.Rent(n);
        var rentedIndices = ArrayPool<int>.Shared.Rent(n);
        var rentedPartners = ArrayPool<int>.Shared.Rent(n);
        var rentedChain = needsChain ? ArrayPool<T>.Shared.Rent(n) : [];
        try
        {
            var values = rentedValues.AsSpan(0, n);
            var chainSpan = new SortSpan<T, TComparer, TContext>(needsChain ? rentedChain.AsSpan(0, n) : default, s.Context, s.Comparer, BUFFER_CHAIN);
            var sorted = rentedSorted.AsSpan(0, n);
            var indices = rentedIndices.AsSpan(0, n);

            // Copy all values to plain temp buffer (untracked) and build initial identity indices
            for (var i = 0; i < n; i++)
            {
                values[i] = s.Read(i);
                indices[i] = i;
            }

            // Build the sorted index order using Ford-Johnson over the original data
            FordJohnson(s, indices, sorted, chainSpan, rentedPartners.AsSpan(0, n));

            // Write back in sorted order (reads from temp buffer, writes to main buffer)
            s.Context.OnPhase(SortPhase.MergeInsertionRearrange, 0, n - 1);
            for (var i = 0; i < n; i++)
            {
                s.Write(i, values[sorted[i]]);
            }
        }
        finally
        {
            ArrayPool<T>.Shared.Return(rentedValues, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
            if (needsChain)
                ArrayPool<T>.Shared.Return(rentedChain, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
            ArrayPool<int>.Shared.Return(rentedSorted);
            ArrayPool<int>.Shared.Return(rentedIndices);
            ArrayPool<int>.Shared.Return(rentedPartners);
        }
    }

    /// <summary>
    /// Recursive Ford-Johnson implementation operating on a span of indices into the values array.
    /// </summary>
    /// <param name="indices">Input indices to sort (read-only).</param>
    /// <param name="outChain">Pre-allocated output buffer that receives the sorted chain. Must have at least <c>indices.Length</c> elements.</param>
    /// <param name="partnerOf">Shared scratch of length <c>values.Length</c> mapping a larger element's original index to its pair partner. Written after the recursive call so deeper levels can reuse the same buffer.</param>
    /// <param name="isTopLevel">When <c>true</c>, chain buffer writes are emitted for visualization. Recursive calls pass <c>false</c> to avoid overwriting the top-level chain state.</param>
    private static void FordJohnson<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, ReadOnlySpan<int> indices, Span<int> outChain, SortSpan<T, TComparer, TContext> chain, Span<int> partnerOf, bool isTopLevel = true)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = indices.Length;

        // Phase announcements and chain-buffer writes are both top-level only. The Merge Insertion
        // phases are scope phases (see SortPhaseExtensions.IsDetailPhase), so a nested level
        // announcing MergeInsertionPairing would overwrite the caller's MergeInsertionSortLarger
        // before a single operation is observed under it, and would also scribble intermediate
        // state over the top-level chain buffer.
        var visualize = typeof(TContext) != typeof(NullContext) && isTopLevel;

        // Base case: single element
        if (n <= 1)
        {
            if (n == 1)
            {
                outChain[0] = indices[0];
                ChainWrite(s, chain, 0, indices[0], visualize);
            }
            return;
        }

        // Base case: two elements
        if (n == 2)
        {
            if (s.IsLessOrEqualAt(indices[0], indices[1]))
            {
                outChain[0] = indices[0];
                outChain[1] = indices[1];
            }
            else
            {
                outChain[0] = indices[1];
                outChain[1] = indices[0];
            }
            ChainWrite(s, chain, 0, outChain[0], visualize);
            ChainWrite(s, chain, 1, outChain[1], visualize);
            return;
        }

        var pairs = n / 2;
        var hasStraggler = (n & 1) == 1;

        // Rent working buffers for this recursion level
        var rentedLarger = ArrayPool<int>.Shared.Rent(pairs);
        var rentedSmaller = ArrayPool<int>.Shared.Rent(pairs);
        var rentedSortedLarger = ArrayPool<int>.Shared.Rent(pairs);
        var rentedPendPartners = ArrayPool<int>.Shared.Rent(pairs);
        try
        {
            var larger = rentedLarger.AsSpan(0, pairs);
            var smaller = rentedSmaller.AsSpan(0, pairs);
            var sortedLarger = rentedSortedLarger.AsSpan(0, pairs);
            var pendPartners = rentedPendPartners.AsSpan(0, pairs);

            // Step 1: Pair elements and compare each pair
            if (visualize) s.Context.OnPhase(SortPhase.MergeInsertionPairing, 0, pairs - 1);

            for (var i = 0; i < pairs; i++)
            {
                var a = indices[2 * i];
                var b = indices[2 * i + 1];

                if (s.IsLessOrEqualAt(a, b))
                {
                    smaller[i] = a;
                    larger[i] = b;
                }
                else
                {
                    smaller[i] = b;
                    larger[i] = a;
                }
            }

            // Step 2: Recursively sort the larger elements.
            // pairs == 1 has nothing to sort, and the recursion emits no observable operation for it,
            // so announcing the phase there would leave it starved.
            if (visualize && pairs > 1) s.Context.OnPhase(SortPhase.MergeInsertionSortLarger, 0, pairs - 1);
            FordJohnson(s, larger, sortedLarger, chain, partnerOf, isTopLevel: false);

            // Map each larger element back to its pair partner for O(1) lookup below.
            // Filled only after the recursive call returns: deeper levels overwrite the
            // same shared buffer for their own pairs, which overlap this level's larger set.
            for (var i = 0; i < pairs; i++)
                partnerOf[larger[i]] = smaller[i];

            // Step 3: Build main chain directly in outChain
            // Main chain starts with b1 (smallest's partner), then all sorted larger elements
            // b1 < a1 <= a2 <= ... <= an, so b1 is known to be smaller than a1
            outChain[0] = partnerOf[sortedLarger[0]];
            sortedLarger.CopyTo(outChain.Slice(1));
            var chainLen = 1 + pairs;

            // Write the initial chain state to the chain buffer for visualization
            // (b1 at position 0, followed by all sorted-larger elements)
            for (var i = 0; i < chainLen; i++)
                ChainWrite(s, chain, i, outChain[i], visualize);

            // Pend list: remaining smaller elements paired with their partner (larger element),
            // plus the straggler (no partner) if n is odd.
            // Reuse sortedLarger buffer for pendValues (safe: values already copied to outChain[1..pairs])
            // Read original sortedLarger values from outChain[1+i] to avoid aliasing.
            var pendValues = sortedLarger;
            var pendCount = 0;

            for (var i = 1; i < pairs; i++)
            {
                var origIdx = outChain[1 + i];
                pendValues[pendCount] = partnerOf[origIdx];
                pendPartners[pendCount] = origIdx;
                pendCount++;
            }

            if (hasStraggler)
            {
                pendValues[pendCount] = indices[n - 1];
                pendPartners[pendCount] = -1;
                pendCount++;
            }

            // Step 4: Insert pend elements using Jacobsthal sequence order
            if (pendCount > 0)
            {
                if (visualize) s.Context.OnPhase(SortPhase.MergeInsertionInsertPend, 0, pendCount - 1);
                InsertPendElements(s, outChain, ref chainLen, pendValues.Slice(0, pendCount), pendPartners.Slice(0, pendCount), chain, isTopLevel);
            }
        }
        finally
        {
            ArrayPool<int>.Shared.Return(rentedLarger);
            ArrayPool<int>.Shared.Return(rentedSmaller);
            ArrayPool<int>.Shared.Return(rentedSortedLarger);
            ArrayPool<int>.Shared.Return(rentedPendPartners);
        }
    }

    /// <summary>
    /// Insert pended elements into the main chain using Jacobsthal sequence order
    /// and binary insertion.
    /// For each pend item with a partner, the upper bound is the partner's current position.
    /// Uses manual element shift on <see cref="Span{T}"/> instead of <c>List.Insert</c> to avoid allocation.
    /// </summary>
    private static void InsertPendElements<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, Span<int> mainChain, ref int chainLen, ReadOnlySpan<int> pendValues, ReadOnlySpan<int> pendPartners, SortSpan<T, TComparer, TContext> chain, bool isTopLevel)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // For each pend item with a partner, the upper bound is the partner's current
        // position in the main chain(exclusive).The pairing step guarantees
        // values[pend] <= values[partner], so the insertion point must lie before
        // or at the partner's current position. Therefore the binary search range
        // can be restricted to[0, upperBound).

        var pendCount = pendValues.Length;
        var visualize = typeof(TContext) != typeof(NullContext) && isTopLevel;

        // Generate Jacobsthal insertion order
        int[]? rentedOrder = null;
        try
        {
            Span<int> insertionOrder = pendCount <= 128
                ? stackalloc int[pendCount]
                : (rentedOrder = ArrayPool<int>.Shared.Rent(pendCount)).AsSpan(0, pendCount);

            FillJacobsthalInsertionOrder(insertionOrder, pendCount);

            for (var i = 0; i < pendCount; i++)
            {
                var idx = insertionOrder[i];
                var valueIdx = pendValues[idx];
                var partnerIdx = pendPartners[idx];

                // Upper bound: partner's current position in the main chain (exclusive).
                // Straggler (partnerIdx == -1) has no partner so it can go anywhere.
                int upperBound;
                if (partnerIdx >= 0)
                {
                    // pend[j]'s partner is chain element j + 2 when the chain is first built
                    // (b1, a1 .. a_(j+1)), and an element's position only grows as later
                    // elements are inserted, so the scan can skip the first j + 2 slots.
                    var lo = idx + 2;
                    Debug.Assert(lo < chainLen);
                    var offset = mainChain.Slice(lo, chainLen - lo).IndexOf(partnerIdx);
                    Debug.Assert(offset >= 0);
                    upperBound = lo + offset;
                }
                else
                {
                    upperBound = chainLen;
                }

                s.Context.OnRole(valueIdx, BUFFER_MAIN, RoleType.Inserting);

                var pos = BinarySearchInChain(s, mainChain, valueIdx, 0, upperBound);

                // Shift right to make room for insertion (replaces List.Insert).
                if (visualize)
                {
                    for (var k = chainLen; k > pos; k--)
                    {
                        mainChain[k] = mainChain[k - 1];
                        ChainShift(chain, k, k - 1, visualize);
                    }
                }
                else
                {
                    // Span.CopyTo forwards to memmove, which handles the overlap.
                    mainChain.Slice(pos, chainLen - pos).CopyTo(mainChain.Slice(pos + 1, chainLen - pos));
                }
                mainChain[pos] = valueIdx;
                ChainWrite(s, chain, pos, valueIdx, visualize);
                s.Context.OnRole(valueIdx, BUFFER_MAIN, RoleType.None);
                chainLen++;
            }
        }
        finally
        {
            if (rentedOrder != null)
                ArrayPool<int>.Shared.Return(rentedOrder);
        }
    }

    /// <summary>
    /// Writes <c>s[valueIdx]</c> into <c>chain[pos]</c> when <paramref name="visualize"/> is <c>true</c>.
    /// JIT eliminates this call entirely on the <see cref="NullContext"/> fast path.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ChainWrite<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> chain, int pos, int valueIdx, bool visualize)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (visualize)
            chain.Write(pos, s.Read(valueIdx));
    }

    /// <summary>
    /// Copies <c>chain[from]</c> to <c>chain[to]</c> when <paramref name="visualize"/> is <c>true</c>.
    /// Used during the shift-right phase of binary insertion into the chain buffer.
    /// JIT eliminates this call entirely on the <see cref="NullContext"/> fast path.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ChainShift<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> chain, int to, int from, bool visualize)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (visualize)
            chain.Write(to, chain.Read(from));
    }

    /// <summary>
    /// Binary search for the insertion position of values[valueIdx] within mainChain[left..right).
    /// This binary search returns the upper-bound.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int BinarySearchInChain<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, ReadOnlySpan<int> mainChain, int valueIdx, int left, int right)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (left < right)
        {
            var mid = (left + right) >>> 1;
            if (s.IsLessOrEqualAt(mainChain[mid], valueIdx))
            {
                left = mid + 1;
            }
            else
            {
                right = mid;
            }
        }
        return left;
    }

    /// <summary>
    /// Fill the insertion order for pended elements using Jacobsthal numbers.
    /// The pend list holds b2, b3, ... (b1 is already in the chain), so pend position <c>i</c>
    /// (1-based) is b(i+1). The classic Ford-Johnson order groups the b subscripts as
    /// (J(k-1), J(k)] and walks each group downwards:
    /// <c>b3 b2 | b5 b4 | b11 .. b6 | b21 .. b12 | ...</c>
    /// In pend positions that group is <c>[J(k-1), J(k) - 1]</c>, walked from the top.
    /// Getting these boundaries wrong still sorts correctly but costs more than F(n) comparisons.
    /// Uses <c>stackalloc</c> for the small Jacobsthal number sequence (max ~34 entries for int range).
    /// </summary>
    /// <param name="order">Pre-allocated output span to receive 0-based indices into the pend array in insertion order.</param>
    /// <param name="count">Number of pended elements.</param>
    private static void FillJacobsthalInsertionOrder(Span<int> order, int count)
    {
        if (count == 0) return;

        // Generate Jacobsthal numbers until we exceed count
        // J(n) = J(n-1) + 2*J(n-2); for any practical int, at most ~34 entries needed
        Span<int> jacobsthal = stackalloc int[64];
        var jacLen = 3;
        jacobsthal[0] = 0;
        jacobsthal[1] = 1;
        jacobsthal[2] = 1;
        while (jacobsthal[jacLen - 1] - 1 < count)
        {
            jacobsthal[jacLen] = jacobsthal[jacLen - 1] + 2 * jacobsthal[jacLen - 2];
            jacLen++;
        }

        var filled = 0;
        for (var k = 3; k < jacLen && filled < count; k++)
        {
            var groupEnd = Math.Min(jacobsthal[k] - 1, count);
            var groupStart = jacobsthal[k - 1];

            // Insert in reverse within this group
            for (var i = groupEnd; i >= groupStart && filled < count; i--)
            {
                // pend is 0-indexed, so subtract 1
                order[filled++] = i - 1;
            }
        }
    }
}
