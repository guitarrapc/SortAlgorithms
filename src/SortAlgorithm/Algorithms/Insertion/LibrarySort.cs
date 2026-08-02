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
/// <para><strong>Core Principles of Library Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Gap Allocation:</strong> Uses (1 + ε)n space where ε is the gap ratio.
/// The extra εn positions serve as gaps, allowing insertions without shifting all subsequent elements.
/// Typical values: ε = 0.5 to 1.0, trading memory for performance.</description></item>
/// <item><description><strong>Binary Search for Position:</strong> Each new element's position is found
/// by bisecting the gapped array itself in O(log n) time. There is no separate rank index: a probe
/// that lands on a gap snaps to the next element to its right, which is O(1) while gaps stay short.</description></item>
/// <item><description><strong>Limited Shift Range:</strong> When inserting, shift elements right only
/// until the nearest gap is reached. With well-distributed gaps, average shift distance is O(log n)
/// rather than O(n), reducing insertion cost from O(n) to O(log n) per element.</description></item>
/// <item><description><strong>Periodic Rebalancing:</strong> A round ends when its gaps are used up,
/// at which point the whole array is respread uniformly. With ε=1 that is the paper's doubling
/// schedule, and the amortized rebalancing cost is O(1) per insertion.</description></item>
/// <item><description><strong>Randomization (Theoretical):</strong> The O(n log n) guarantee assumes
/// random insertion order, which spreads gap consumption evenly. This implementation does not
/// shuffle, because reordering equal keys would destroy stability, so the worst case stays O(n²)
/// on input that drives every insertion into the same region - reversed input above all.</description></item>
/// </list>
/// <para><strong>Algorithm Overview:</strong></para>
/// <list type="number">
/// <item><description><strong>Initialization:</strong> Create auxiliary array of size (1+ε)n.
/// Mark every slot as a gap in the occupancy bitset. Start with small sorted region using standard insertion sort.</description></item>
/// <item><description><strong>Insertion Loop:</strong> For each new element:
/// - Bisect the gapped array to find the neighbouring pair it belongs between
/// - If that interval holds a gap, write into the one nearest the target; otherwise shift right until a gap is reached
/// - Equal elements are placed after their equals (upper bound), which keeps the sort stable</description></item>
/// <item><description><strong>Rebalancing:</strong> When the round's gaps are exhausted, at (1+ε)*count elements:
/// - Collect all non-gap elements
/// - Redistribute over (1+ε) times the new count, with evenly spaced gaps
/// - Reset counters and continue insertion</description></item>
/// <item><description><strong>Final Extraction:</strong> After all insertions, extract non-gap elements
/// back to original array in sorted order.</description></item>
/// </list>
/// <para><strong>Gap Management Strategy:</strong></para>
/// <list type="bullet">
/// <item><description>Gap Ratio (ε): 1.0, i.e. 2n total space with n gaps. ε is the only tuning knob:
/// it fixes both the space and the round length, because a round can only run as long as its gaps last</description></item>
/// <item><description>Initial Size: Start small (e.g., 32 elements) with standard insertion sort</description></item>
/// <item><description>Growth Factor: A round ends at (1+ε)*count elements, so ε=1 gives the paper's doubling</description></item>
/// <item><description>Gap Representation: Occupancy is tracked in a separate bitset, not by a sentinel value
/// inside the auxiliary buffer. The auxiliary buffer therefore holds plain elements,
/// so its writes carry real element values to an observing context and marking a slot as a gap
/// costs no element write at all. Word-at-a-time bit scans give the nearest element or gap in O(1)
/// per 64 slots, which is what makes bisecting the gapped array practical.</description></item>
/// <item><description>Spacing: After rebalancing, distribute elements uniformly with gap:element ratio = ε:1</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family        : Insertion (gap-based variant)</description></item>
/// <item><description>Stable        : Yes (equal keys are inserted after their equals and never shift past one another)</description></item>
/// <item><description>In-place      : No (requires (1+ε)n auxiliary space for gaps)</description></item>
/// <item><description>Best case     : O(n log n) - Every element costs a binary search regardless of
/// how sorted the input is; sorted input merely adds no shifting on top</description></item>
/// <item><description>Average case  : O(n log n) - With random input and good gap distribution</description></item>
/// <item><description>Worst case    : O(n²) - Pathological gap clustering without randomization</description></item>
/// <item><description>Space         : O(n) - Auxiliary array of (1+ε)n elements, an n-element staging
/// buffer for rebalancing, and one occupancy bit per auxiliary slot</description></item>
/// <item><description>Binary Search : O(log n) per insertion to find position</description></item>
/// <item><description>Shift Cost    : O(log n) average per insertion with good gaps</description></item>
/// <item><description>Rebalance     : O(n) per rebalance, at each round end or on a long shift</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Paper: https://arxiv.org/abs/cs/0407003 "Insertion Sort is O(n log n)" by Michael A. Bender, Martín Farach-Colton, and Miguel Mosteiro</para>
/// <para>Conference: Proceedings of the Third International Conference on Fun With Algorithms (FUN 2004)</para>
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
    // Note: the occupancy bitset is algorithm metadata rather than element storage, so it carries
    // no buffer identifier and is not reported to the context.

    // Gap ratio: ε = 1.0 means (1+ε)n = 2n space
    private const double GapRatio = 1.0;

    // Small array threshold for fallback to InsertionSort
    private const int SmallSortThreshold = 32;

    // Trigger early rebalance if the shift distance exceeds this threshold.
    //
    // This is the one mechanism here that the paper does not have. The paper reaches O(n log n) by
    // inserting in random order, which spreads gap consumption evenly; this implementation cannot
    // shuffle because that would destroy stability, so a run of insertions into the same region can
    // exhaust that region's gaps long before the round ends. Rebalancing when a shift gets long
    // restores the uniform spacing that randomization would have maintained. Removing it measures
    // 1.5x slower on both reversed and random input at n=131,072.
    private const int MaxShiftDistanceBeforeRebalance = 64;

    /// <summary>
    /// Element count that ends the current round, i.e. when the next rebalance is due.
    /// </summary>
    /// <remarks>
    /// A round starts with <paramref name="count"/> elements spread over (1+ε)*count slots, so it
    /// starts with ε*count gaps, and every insertion consumes exactly one gap. The round therefore
    /// runs until the gaps are used up, at (1+ε)*count elements - the round length is a consequence
    /// of ε, not a free parameter. With ε=1 that is the paper's doubling schedule.
    /// <para>
    /// Letting a round run longer than its gaps last is what makes gap-based insertion degrade:
    /// once a region is full, every further insertion there shifts its way to a gap that keeps
    /// getting further away. The original ε=0.5 with 4x rounds inserted 3*count elements into
    /// 0.5*count gaps, and measured 998 ms on pipe-organ input at n=131,072 where this ends at 7 ms.
    /// </para>
    /// </remarks>
    private static int RoundEnd(int count) => (int)(count * (1 + GapRatio));

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

        // For very small arrays, use standard insertion sort
        if (length <= SmallSortThreshold)
        {
            InsertionSort.Sort(span, 0, span.Length, comparer, context);
            return;
        }

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
        // Auxiliary array size: exactly (1+ε)n, which is 2n at ε=1. No slack is needed on top:
        // a round that starts with c elements spreads them over (1+ε)c slots and appends at most
        // (n-c) more past that spread, and (1+ε)c + (n-c) never exceeds (1+ε)n for c <= n.
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
            var temp = tempArray.AsSpan(0, length);

            // A pooled array arrives with arbitrary content: every slot starts as a gap.
            bits.Clear();

            // Phase 1: Initial sort
            context.OnPhase(SortPhase.LibrarySortPhase, 1);
            var initSize = Math.Min(SmallSortThreshold, length);
            InsertionSort.SortCore(s, 0, initSize);

            // Spread the sorted prefix over the auxiliary buffer with uniform gaps
            var auxEnd = PlaceWithGaps(aux, s, 0, initSize, auxSize, bits);

            var sorted = initSize;
            var nextRebalance = RoundEnd(initSize);

            // Phase 2: Insert remaining
            context.OnPhase(SortPhase.LibrarySortPhase, 2);
            for (var i = initSize; i < length; i++)
            {
                if (sorted >= nextRebalance)
                {
                    auxEnd = Rebalance(aux, auxSize, bits, auxEnd, sorted, temp);
                    nextRebalance = RoundEnd(sorted);
                }

                context.OnRole(i, BUFFER_MAIN, RoleType.Inserting);
                var elem = s.Read(i);

                if (!TryInsert(aux, ref auxEnd, auxSize, elem, bits, out var largeShift))
                {
                    // Gaps have clustered away from the insertion point. Rebalancing restores a
                    // uniform gap every few slots, so the retry is guaranteed to find one. It moves
                    // every element, so TryInsert re-runs its own search rather than reusing bounds.
                    auxEnd = Rebalance(aux, auxSize, bits, auxEnd, sorted, temp);
                    nextRebalance = RoundEnd(sorted);

                    if (!TryInsert(aux, ref auxEnd, auxSize, elem, bits, out largeShift))
                        throw new InvalidOperationException($"No gap available after rebalance (count={sorted}, auxSize={auxSize})");
                }
                context.OnRole(i, BUFFER_MAIN, RoleType.None);
                sorted++;

                // Early rebalance if large shift was detected (gaps are clustering)
                if (largeShift && sorted < nextRebalance)
                {
                    auxEnd = Rebalance(aux, auxSize, bits, auxEnd, sorted, temp);
                    nextRebalance = RoundEnd(sorted);
                }
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
    /// Returns false when no usable gap exists at or after the insertion point; the caller must then
    /// rebalance and call again. <paramref name="largeShift"/> reports that a long shift was needed,
    /// suggesting an early rebalance.
    /// </summary>
    private static bool TryInsert<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> aux, ref int auxEnd, int maxSize,
        T value, Span<ulong> bits, out bool largeShift)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        largeShift = false;

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

        // The interval between the neighbours is completely full. Shift succ and everything packed
        // behind it one slot right, into the nearest gap, and take succ's slot.
        //
        // This branch is unreachable when succ is absent: then pred is the last element overall, so
        // slot pred+1 is a gap and the search above would have used it. targetPos is therefore an
        // occupied slot, and NextGap guarantees [targetPos, shiftGap) is fully occupied too.
        var targetPos = succ >= 0 ? succ : searchStart;
        var shiftGap = NextGap(bits, targetPos, maxSize);
        if (shiftGap < 0) return false;

        largeShift = shiftGap - targetPos > MaxShiftDistanceBeforeRebalance;

        for (var i = shiftGap; i > targetPos; i--)
        {
            aux.Write(i, aux.Read(i - 1));
        }
        // Only the consumed gap changes state; every slot the shift walked over was already occupied.
        SetOccupied(bits, shiftGap);

        aux.Write(targetPos, value);
        if (shiftGap >= auxEnd) auxEnd = shiftGap + 1;
        return true;
    }

    /// <summary>
    /// Collects every element in slot order and redistributes it with uniform spacing over
    /// range = min((1+ε)*count, auxSize). Element order is preserved, so the sort stays stable.
    /// Returns the maximum used position + 1 for auxEnd tracking.
    /// </summary>
    private static int Rebalance<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> aux, int auxSize,
        Span<ulong> bits, int auxEnd, int expectedCount, Span<T> tempBuffer)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Collect elements
        var count = 0;
        for (var p = NextOccupied(bits, 0, auxEnd); p >= 0; p = NextOccupied(bits, p + 1, auxEnd))
        {
            if (count == expectedCount)
                throw new InvalidOperationException($"Rebalance found more than the expected {expectedCount} occupied slots below auxEnd={auxEnd}");
            tempBuffer[count++] = aux.Read(p);
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

            aux.Write(pos, tempBuffer[i]);
            SetOccupied(bits, pos);
            maxUsedPos = pos;
        }

        // Return the maximum used position + 1
        // This represents the true auxEnd after rebalancing
        return maxUsedPos + 1;
    }

    /// <summary>
    /// Number of slots <paramref name="count"/> elements are spread over at the start of a round.
    /// </summary>
    /// <remarks>
    /// The paper's invariant is that the elements present occupy a (1+ε)-sized region, so a round
    /// starts with count elements spread over (1+ε)*count slots and therefore ε*count gaps.
    /// <see cref="RoundEnd"/> derives the round length from exactly those gaps.
    /// </remarks>
    private static int SpreadRange(int count, int auxSize)
        => Math.Min((int)Math.Ceiling(count * (1 + GapRatio)), auxSize);

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
