using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// ギャップベースの挿入ソートで、理論上O(n log n)の期待計算量を持ちます。
/// 図書館の本棚のように、要素間に適度な隙間(ギャップ)を保持することで、挿入時のシフト量を大幅に削減します。
/// 定期的なリバランス操作により、ギャップを均等に再配置し、効率的な挿入を維持します。
/// <br/>
/// A gap-based insertion sort with O(n log n) expected time complexity.
/// Like library bookshelves, it maintains gaps between elements to reduce
/// the amount of shifting during insertions. Periodic rebalancing redistributes
/// gaps evenly to maintain efficient insertion performance.
/// </summary>
/// <remarks>
/// <para>Starts with one support element. A round inserts up to the current support count, then doubles it.
/// Before the next round, c support elements are spread over min((2+2ε)c, (1+ε)n) slots.
/// Rebalancing occurs only at doubling boundaries. An insertion uses an adjacent gap or shifts toward
/// the closest gap on either side, without triggering an early rebalance.</para>
/// <para>The O(n log n) expected bound assumes random arrival order. Input is not shuffled,
/// preserving stable order for equal keys. Ordered and adversarial arrivals may take O(n²).</para>
/// <para>Occupancy bits are metadata. Element buffer 1 contains the gapped sequence; buffer 2
/// stages its elements during redistribution. Extraction writes the final sequence to buffer 0.</para>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Insertion (gapped array)</description></item>
/// <item><description>Stable      : Yes - upper-bound insertion and ordered redistribution</description></item>
/// <item><description>In-place    : No - gapped element storage and dense staging</description></item>
/// <item><description>Best case   : Θ(n log n) - binary searches with short shifts</description></item>
/// <item><description>Average case: O(n log n) - random arrival order</description></item>
/// <item><description>Worst case  : O(n²) - clustered insertions can require long shifts</description></item>
/// <item><description>Comparisons : O(n log n) - binary search for each insertion</description></item>
/// <item><description>Swaps       : 0 - shifts and placement move elements</description></item>
/// <item><description>Index Reads : O(n log n) average, O(n²) worst - search, shifts, and redistribution</description></item>
/// <item><description>Index Writes: O(n log n) average, O(n²) worst - placement, shifts, and redistribution</description></item>
/// <item><description>Space       : O(n) - gapped array, staging, and occupancy bits</description></item>
/// </list>
/// <para>Reference: Bender, Farach-Colton, and Mosteiro, "Insertion Sort is O(n log n)",
/// https://arxiv.org/abs/cs/0407003.</para>
/// </remarks>
public static class LibrarySort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by LibrarySortTests, which derives from StableSortTestsBase.</remarks>
    public static bool IsStable => true;

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;        // Main input array
    private const int BUFFER_AUX = 1;         // Auxiliary array with gaps
    private const int BUFFER_STAGING = 2;      // Dense elements during rebalance
    // Note: the occupancy bitset is algorithm metadata rather than element storage, so it carries
    // no buffer identifier and is not reported to the context.

    // Gap ratio: ε = 1.0 means (1+ε)n = 2n space
    private const double GapRatio = 1.0;

    // A round doubles the support count, independently of the gap ratio.
    private static int RoundEnd(int count, int length) => (int)Math.Min(2L * count, length);

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
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="comparer">The comparer to use for element comparisons.</param>
    /// <param name="context">The sort context for tracking statistics and observations during sorting.</param>
    public static void Sort<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var length = span.Length;
        if (length <= 1) return;

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        SortCore(s, length, comparer, context);
    }

    /// <summary>
    /// Core sorting logic with proper gap management and O(log n) search.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The gapped auxiliary buffer is the only index the algorithm keeps. <c>auxEnd</c> is one past
    /// the highest occupied slot and bounds every search; keeping it exact is a correctness
    /// requirement, not an optimization, because the binary search uses it as its upper bound.
    /// </para>
    /// </remarks>
    private static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int length, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // The active prefix is (1+ε)*min(2c,n) for c support elements, so every round fits
        // within the full (1+ε)n allocation without appending beyond its active prefix.
        var auxSize = (int)Math.Ceiling(length * (1 + GapRatio));
        var wordCount = (auxSize + 63) >> 6;

        var auxArray = ArrayPool<T>.Shared.Rent(auxSize);
        var bitsArray = ArrayPool<ulong>.Shared.Rent(wordCount);
        var tempArray = ArrayPool<T>.Shared.Rent(length);

        try
        {
            var aux = new SortSpan<T, TComparer, TContext>(auxArray.AsSpan(0, auxSize), context, comparer, BUFFER_AUX);
            // bits[i] tells whether aux slot i holds an element or is a gap. Keeping occupancy out of
            // the element buffer means a gap is created by clearing a bit, not by writing a sentinel
            // element, so gap bookkeeping produces no observable element operations.
            var bits = bitsArray.AsSpan(0, wordCount);
            var temp = new SortSpan<T, TComparer, TContext>(tempArray.AsSpan(0, length), context, comparer, BUFFER_STAGING);

            // A pooled array arrives with arbitrary content: every slot starts as a gap.
            bits.Clear();

            // Phase 1: Seed one support element
            context.OnPhase(SortPhase.LibrarySortPhase, 1);
            const int initSize = 1;

            // Spread the sorted prefix over the auxiliary buffer with uniform gaps
            var auxEnd = PlaceWithGaps(aux, s, 0, initSize, auxSize, bits);

            var sorted = initSize;
            var nextRebalance = RoundEnd(initSize, length);
            var activeSize = SpreadRange(initSize, auxSize);

            // Phase 2: Insert remaining
            context.OnPhase(SortPhase.LibrarySortPhase, 2);
            for (var i = initSize; i < length; i++)
            {
                if (sorted >= nextRebalance)
                {
                    auxEnd = Rebalance(aux, auxSize, bits, auxEnd, sorted, temp);
                    nextRebalance = RoundEnd(sorted, length);
                    activeSize = SpreadRange(sorted, auxSize);
                }

                context.OnRole(i, BUFFER_MAIN, RoleType.Inserting);
                var elem = s.Read(i);

                if (!TryInsert(aux, ref auxEnd, activeSize, elem, bits))
                    throw new InvalidOperationException("No gap in the active library-sort round.");
                context.OnRole(i, BUFFER_MAIN, RoleType.None);
                sorted++;
            }

            // Phase 3: Extract. Verify the occupancy count first: the write loop below trusts it to
            // stay inside the caller's span.
            context.OnPhase(SortPhase.LibrarySortPhase, 3);
            var occupiedCount = 0;
            for (var w = 0; w < bits.Length; w++)
            {
                occupiedCount += BitOperations.PopCount(bits[w]);
            }
            if (occupiedCount != length)
                throw new InvalidOperationException($"Data loss detected: expected {length} elements in the auxiliary buffer, but {occupiedCount} slots are occupied");

            var written = 0;
            for (var p = NextOccupied(bits, 0, auxEnd); p >= 0; p = NextOccupied(bits, p + 1, auxEnd))
            {
                s.Write(written++, aux.Read(p));
            }
            if (written != length)
                throw new InvalidOperationException($"Data loss detected: {occupiedCount} slots are occupied but only {written} lie below auxEnd={auxEnd}");
        }
        finally
        {
            var clearElements = RuntimeHelpers.IsReferenceOrContainsReferences<T>();
            ArrayPool<T>.Shared.Return(auxArray, clearElements);
            ArrayPool<ulong>.Shared.Return(bitsArray);
            ArrayPool<T>.Shared.Return(tempArray, clearElements);
        }
    }

    /// <summary>
    /// Spreads <paramref name="count"/> already-sorted elements over the auxiliary buffer with
    /// uniform gaps. Returns one past the highest occupied slot.
    /// Assumes the occupancy bitset is already clear.
    /// </summary>
    private static int PlaceWithGaps<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> aux, SortSpan<T, TComparer, TContext> src,
        int srcStart, int count, int auxSize, Span<ulong> bits)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (count == 0) return 0;

        // Strict validation: must have enough space for all elements
        if (auxSize < count)
            throw new InvalidOperationException($"Insufficient auxiliary buffer space: need at least {count} positions, but auxSize={auxSize}");

        var range = SpreadRange(count, auxSize);

        // Distribute: pos[i] = floor(i * range / count)
        // This guarantees no collisions since range >= count
        var maxUsedPos = 0;
        for (var i = 0; i < count; i++)
        {
            var pos = (int)((long)i * range / count);

            // Defensive check (should never happen with range >= count)
            if (pos >= auxSize)
                throw new InvalidOperationException($"Position overflow: calculated pos={pos}, but auxSize={auxSize} (i={i}, count={count}, range={range})");

            aux.Write(pos, src.Read(srcStart + i));
            SetOccupied(bits, pos);
            maxUsedPos = pos;
        }

        return maxUsedPos + 1;
    }

    /// <summary>
    /// Inserts <paramref name="value"/> into the gapped auxiliary buffer.
    /// Returns false only if the active prefix contains no gap.
    /// </summary>
    private static bool TryInsert<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> aux, ref int auxEnd, int maxSize,
        T value, Span<ulong> bits)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {

        // Bisect slot indices over [0, auxEnd] for the smallest boundary `lo` such that the first
        // element at or after `lo` compares strictly greater than value. The predicate is monotone
        // in the slot index because the occupied slots are sorted, so this is an ordinary halving
        // search even though the array is sparse.
        //
        // Stability: a probe that compares equal takes the "not greater" branch and moves `lo` past
        // it, so `lo` always lands after every element equal to value and before every greater one.
        // That is the upper bound, which is what preserves the order of equal keys.
        var lo = 0;
        var hi = auxEnd;
        while (lo < hi)
        {
            var mid = lo + ((hi - lo) >> 1);
            // A probe landing in a gap resolves to the next element on its right. Every slot in
            // [mid, probe] therefore has the same predicate value.
            var probe = NextOccupied(bits, mid, auxEnd);

            if (probe < 0 || aux.Compare(value, probe) < 0)
            {
                hi = mid;
            }
            else
            {
                // Skipping to probe+1 rather than mid+1 avoids re-probing the same element.
                lo = probe + 1;
            }
        }

        // The two elements the value belongs between. Either may be absent at the ends.
        var succ = NextOccupied(bits, lo, auxEnd);
        var pred = PrevOccupied(bits, lo - 1);

        // Any slot strictly between them keeps the buffer sorted and the sort stable.
        var searchStart = pred + 1;
        var searchEnd = succ >= 0 ? succ : maxSize;

        // Choose where in that interval to aim, to keep gap consumption balanced:
        // - at either end, stay next to the existing neighbour so the far side keeps its gaps
        // - in the middle, aim at the midpoint
        var gapTarget = (pred < 0 || succ < 0) ? searchStart : searchStart + ((searchEnd - searchStart) >> 1);

        var gapPos = FindNearestGap(bits, gapTarget, searchStart, searchEnd);
        if (gapPos >= 0)
        {
            aux.Write(gapPos, value);
            SetOccupied(bits, gapPos);
            // auxEnd must stay one past the highest occupied slot: appends routinely land beyond it.
            if (gapPos >= auxEnd) auxEnd = gapPos + 1;
            return true;
        }

        // With no gap between the neighbours, shift toward the closest available gap.
        // A left shift also handles appends when the active prefix ends in occupied slots.
        var targetPos = succ >= 0 ? succ : searchStart;
        var rightGap = NextGap(bits, targetPos, maxSize);
        var leftGap = PrevGap(bits, targetPos - 1, 0);
        if (rightGap >= 0 && (leftGap < 0 || rightGap - targetPos <= targetPos - leftGap - 1))
        {
            for (var i = rightGap; i > targetPos; i--)
                aux.Write(i, aux.Read(i - 1));
            SetOccupied(bits, rightGap);
            aux.Write(targetPos, value);
            if (rightGap >= auxEnd) auxEnd = rightGap + 1;
            return true;
        }
        if (leftGap < 0) return false;
        for (var i = leftGap; i < targetPos - 1; i++)
            aux.Write(i, aux.Read(i + 1));
        SetOccupied(bits, leftGap);
        aux.Write(targetPos - 1, value);
        return true;
    }

    /// <summary>
    /// Collects every element in slot order and redistributes it with uniform spacing over
    /// range = min((2+2ε)*count, auxSize). Element order is preserved, so the sort stays stable.
    /// Returns the maximum used position + 1 for auxEnd tracking.
    /// </summary>
    private static int Rebalance<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> aux, int auxSize,
        Span<ulong> bits, int auxEnd, int expectedCount, SortSpan<T, TComparer, TContext> tempBuffer)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Collect elements
        var count = 0;
        for (var p = NextOccupied(bits, 0, auxEnd); p >= 0; p = NextOccupied(bits, p + 1, auxEnd))
        {
            if (count == expectedCount)
                throw new InvalidOperationException($"Rebalance found more than the expected {expectedCount} occupied slots below auxEnd={auxEnd}");
            tempBuffer.Write(count++, aux.Read(p));
        }

        if (count != expectedCount)
            throw new InvalidOperationException($"Rebalance expected {expectedCount} elements but found {count} below auxEnd={auxEnd}");
        if (count == 0) return 0;

        // Strict validation: must have enough space for all elements
        if (auxSize < count)
        {
            throw new InvalidOperationException(
                $"Insufficient auxiliary buffer space for rebalance: need at least {count} positions, " +
                $"but auxSize={auxSize}. This indicates the buffer was too small from the start.");
        }

        var range = SpreadRange(count, auxSize);

        // Every slot becomes a gap: elements can sit beyond `range` after a run of appends, and
        // leaving those marked would strand usable space and block later searches.
        bits.Clear();

        // Redistribute: pos[i] = floor(i * range / count)
        // This guarantees no collisions since range >= count
        var maxUsedPos = 0;
        for (var i = 0; i < count; i++)
        {
            var pos = (int)((long)i * range / count);

            // Defensive check (should never happen with range >= count)
            if (pos >= auxSize)
            {
                throw new InvalidOperationException(
                    $"Position overflow in rebalance: calculated pos={pos}, but auxSize={auxSize} " +
                    $"(i={i}, count={count}, range={range})");
            }

            aux.Write(pos, tempBuffer.Read(i));
            SetOccupied(bits, pos);
            maxUsedPos = pos;
        }

        // Return the maximum used position + 1
        // This represents the true auxEnd after rebalancing
        return maxUsedPos + 1;
    }

    /// <summary>
    /// Spreads the support elements over the prefix available after the round doubles their count.
    /// </summary>
    private static int SpreadRange(int count, int auxSize)
        => (int)Math.Min(Math.Ceiling(count * (2 + 2 * GapRatio)), auxSize);

    /// <summary>
    /// Returns the gap closest to <paramref name="target"/> within [start, end), preferring the
    /// right side on a tie, or -1 when the interval is completely occupied.
    /// </summary>
    private static int FindNearestGap(ReadOnlySpan<ulong> bits, int target, int start, int end)
    {
        if (start >= end) return -1;

        if (target < start) target = start;
        else if (target >= end) target = end - 1;

        var right = NextGap(bits, target, end);
        var left = PrevGap(bits, target, start);

        if (right < 0) return left;
        if (left < 0) return right;
        return right - target <= target - left ? right : left;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SetOccupied(Span<ulong> bits, int index) => bits[index >> 6] |= 1UL << index;

    /// <summary>
    /// Returns the smallest occupied slot in [from, limit), or -1 when there is none.
    /// </summary>
    private static int NextOccupied(ReadOnlySpan<ulong> bits, int from, int limit)
    {
        if (from < 0) from = 0;
        if (from >= limit) return -1;

        var lastWord = (limit - 1) >> 6;
        var w = from >> 6;
        // The shift count is taken modulo 64, so this masks off the bits below `from`.
        var word = bits[w] & (ulong.MaxValue << from);

        while (true)
        {
            if (word != 0)
            {
                var pos = (w << 6) + BitOperations.TrailingZeroCount(word);
                return pos < limit ? pos : -1;
            }
            if (++w > lastWord) return -1;
            word = bits[w];
        }
    }

    /// <summary>
    /// Returns the largest occupied slot in [0, upTo], or -1 when there is none.
    /// </summary>
    private static int PrevOccupied(ReadOnlySpan<ulong> bits, int upTo)
    {
        if (upTo < 0) return -1;

        var w = upTo >> 6;
        var word = bits[w] & (ulong.MaxValue >> (63 - (upTo & 63)));

        while (true)
        {
            if (word != 0) return (w << 6) + (63 - BitOperations.LeadingZeroCount(word));
            if (--w < 0) return -1;
            word = bits[w];
        }
    }

    /// <summary>
    /// Returns the smallest gap in [from, limit), or -1 when the range is completely occupied.
    /// </summary>
    private static int NextGap(ReadOnlySpan<ulong> bits, int from, int limit)
    {
        if (from < 0) from = 0;
        if (from >= limit) return -1;

        var lastWord = (limit - 1) >> 6;
        var w = from >> 6;
        var word = ~bits[w] & (ulong.MaxValue << from);

        while (true)
        {
            if (word != 0)
            {
                // Slots past the end of the buffer read as gaps in the final word; the bound check
                // rejects them.
                var pos = (w << 6) + BitOperations.TrailingZeroCount(word);
                return pos < limit ? pos : -1;
            }
            if (++w > lastWord) return -1;
            word = ~bits[w];
        }
    }

    /// <summary>
    /// Returns the largest gap in [floor, upTo], or -1 when the range is completely occupied.
    /// </summary>
    private static int PrevGap(ReadOnlySpan<ulong> bits, int upTo, int floor)
    {
        if (upTo < floor) return -1;

        var firstWord = floor >> 6;
        var w = upTo >> 6;
        var word = ~bits[w] & (ulong.MaxValue >> (63 - (upTo & 63)));

        while (true)
        {
            if (word != 0)
            {
                var pos = (w << 6) + (63 - BitOperations.LeadingZeroCount(word));
                return pos >= floor ? pos : -1;
            }
            if (--w < firstWord) return -1;
            word = ~bits[w];
        }
    }
}
