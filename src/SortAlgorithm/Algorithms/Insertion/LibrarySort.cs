using System.Buffers;
using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// ギャップベースの挿入ソートです。
/// 図書館の本棚のように、要素間に適度な隙間(ギャップ)を保持することで、挿入時のシフト量を大幅に削減します。
/// 定期的なリバランス操作により、ギャップを均等に再配置し、効率的な挿入を維持します。
/// <br/>
/// A gap-based insertion sort. Like library bookshelves, it maintains gaps between elements to
/// reduce the amount of shifting during insertions. Periodic rebalancing redistributes gaps evenly
/// to maintain efficient insertion performance. Comparisons and element moves follow the paper's
/// O(n log n) expectation; see Performance Characteristics for the cost of this implementation's
/// position index.
/// </summary>
/// <remarks>
/// <para><strong>Core Principles of Library Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Gap Allocation:</strong> Uses (1 + ε)n space where ε is the gap ratio.
/// The extra εn positions serve as gaps, allowing insertions without shifting all subsequent elements.
/// Typical values: ε = 0.5 to 1.0, trading memory for performance.</description></item>
/// <item><description><strong>Binary Search for Position:</strong> Each new element's position is found
/// via binary search among existing elements in O(log n) time, ignoring gap positions.
/// This is significantly faster than linear search in standard insertion sort.</description></item>
/// <item><description><strong>Limited Shift Range:</strong> When inserting, shift elements right only
/// until the nearest gap is reached. With well-distributed gaps, average shift distance is O(log n)
/// rather than O(n), reducing insertion cost from O(n) to O(log n) per element.</description></item>
/// <item><description><strong>Periodic Rebalancing:</strong> When gaps become unevenly distributed,
/// rebalance the entire array to restore uniform gap distribution. Rebalancing occurs every 2^i or 4^i elements
/// (doubling strategy) so the amortized cost remains O(1) per insertion.</description></item>
/// <item><description><strong>Randomization (Theoretical):</strong> The O(n log n) guarantee assumes
/// random input order or shuffling. Without randomization, worst-case remains O(n²) when gaps cluster badly.
/// In practice, for general unsorted data, randomization is often unnecessary.</description></item>
/// </list>
/// <para><strong>Algorithm Overview:</strong></para>
/// <list type="number">
/// <item><description><strong>Initialization:</strong> Create auxiliary array of size (1+ε)n.
/// Mark every slot as a gap in the occupancy map. Start with small sorted region using standard insertion sort.</description></item>
/// <item><description><strong>Insertion Loop:</strong> For each new element:
/// - Binary search among non-gap elements to find insertion position
/// - If position has gap, write directly; otherwise shift right until gap found
/// - Equal elements are placed after their equals (upper bound), which keeps the sort stable</description></item>
/// <item><description><strong>Rebalancing:</strong> When element count reaches rebalance threshold (2x or 4x):
/// - Collect all non-gap elements
/// - Redistribute into auxiliary array with evenly spaced gaps
/// - Rebalance factor: spread elements across (2+2ε) times current size
/// - Reset counters and continue insertion</description></item>
/// <item><description><strong>Final Extraction:</strong> After all insertions, extract non-gap elements
/// back to original array in sorted order.</description></item>
/// </list>
/// <para><strong>Gap Management Strategy:</strong></para>
/// <list type="bullet">
/// <item><description>Gap Ratio (ε): 0.5 provides good balance (1.5n total space, 0.5n gaps)</description></item>
/// <item><description>Initial Size: Start small (e.g., 32 elements) with standard insertion sort</description></item>
/// <item><description>Growth Factor: Rebalance every 4x elements (more practical than 2x from paper)</description></item>
/// <item><description>Gap Representation: Occupancy is tracked in a separate bitmap, not by a sentinel value
/// inside the auxiliary buffer. The auxiliary buffer therefore holds plain elements,
/// so its writes carry real element values to an observing context and marking a slot as a gap
/// costs no element write at all.</description></item>
/// <item><description>Spacing: After rebalancing, distribute elements uniformly with gap:element ratio = ε:1</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family        : Insertion (gap-based variant)</description></item>
/// <item><description>Stable        : Yes (equal elements maintain relative order during shifts)</description></item>
/// <item><description>In-place      : No (requires (1+ε)n auxiliary space for gaps)</description></item>
/// <item><description>Comparisons   : O(n log n) - one binary search per inserted element</description></item>
/// <item><description>Element moves : O(log n) average per insertion with good gaps; O(n) when gaps cluster</description></item>
/// <item><description>Space         : O(n) - Auxiliary array of size (1+ε)n ≈ 1.5n to 2n, plus an
/// occupancy byte per slot and an int position index per element</description></item>
/// <item><description>Rebalance     : O(n) per rebalance, triggered on 4x growth or on a long shift</description></item>
/// <item><description>Running time  : Θ(n²) as built. The comparison and element-move counts match the
/// paper, but the dense position index is kept sorted by memmove, so each insertion moves up to
/// posCount ints. The constant is small because the index is cache-resident: measured 15 ms at
/// n=10,000 and 8.2 s at n=800,000, with time quadrupling each time n doubles.</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Paper: https://arxiv.org/abs/cs/0407003 "Insertion Sort is O(n log n)" by Michael A. Bender, Martín Farach-Colton, and Miguel Mosteiro</para>
/// <para>Conference: Proceedings of the Third International Conference on Fun With Algorithms (FUN 2004)</para>
/// </remarks>
public static class LibrarySort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;        // Main input array
    private const int BUFFER_AUX = 1;         // Auxiliary array with gaps
    // Note: the position index and the occupancy map are algorithm metadata rather than element
    // storage, so they carry no buffer identifier and are not reported to the context.

    // Gap ratio: ε = 0.5 means (1+ε)n = 1.5n space
    private const double GapRatio = 0.5;

    // Rebalance every R times growth
    private const int RebalanceFactor = 4;

    // Small array threshold for fallback to InsertionSort
    private const int SmallSortThreshold = 32;

    // Maximum distance to search for a gap during insertion
    private const int MaxGapSearchDistance = 20;

    // Trigger early rebalance if shift distance exceeds this threshold
    private const int MaxShiftDistanceBeforeRebalance = 64;

    // Safety margin for auxiliary buffer size (1.05 = 5% extra space)
    private const double AuxSizeSafetyMargin = 1.05;

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
    private static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int length, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Auxiliary array size: (1+ε)n with safety margin
        // With ε=0.5: (1.5 * 1.05)n ≈ 1.575n
        var auxSize = (int)Math.Ceiling(length * (1 + GapRatio) * AuxSizeSafetyMargin);

        var auxArray = ArrayPool<T>.Shared.Rent(auxSize);
        var occupiedArray = ArrayPool<bool>.Shared.Rent(auxSize);
        var positionsArray = ArrayPool<int>.Shared.Rent(length);
        var tempArray = ArrayPool<T>.Shared.Rent(length);

        try
        {
            var aux = new SortSpan<T, TComparer, TContext>(auxArray.AsSpan(0, auxSize), context, comparer, BUFFER_AUX);
            // occupied[i] tells whether aux[i] holds an element or is a gap. Keeping it out of the
            // element buffer means a gap is created by clearing a byte, not by writing a sentinel
            // element, so gap bookkeeping produces no observable element operations.
            var occupied = occupiedArray.AsSpan(0, auxSize);
            // Note: positions uses Span<int> (not SortSpan) for O(1) memcpy performance
            var positions = positionsArray.AsSpan(0, length);
            var temp = tempArray.AsSpan(0, length);

            // A pooled bool[] arrives with arbitrary content: every slot starts as a gap.
            occupied.Clear();

            // Phase 1: Initial sort
            context.OnPhase(SortPhase.LibrarySortPhase, 1);
            var initSize = Math.Min(SmallSortThreshold, length);
            InsertionSort.SortCore(s, 0, initSize);

            // Place with gaps and build initial position buffer
            var auxEnd = PlaceWithGaps(aux, s, 0, initSize, 0, auxSize, occupied, positions, out var posCount);

            var sorted = initSize;
            var nextRebalance = initSize * RebalanceFactor;

            // Phase 2: Insert remaining
            context.OnPhase(SortPhase.LibrarySortPhase, 2);
            for (var i = initSize; i < length; i++)
            {
                if (sorted >= nextRebalance)
                {
                    auxEnd = Rebalance(aux, auxSize, occupied, positions, ref posCount, temp);
                    nextRebalance = sorted * RebalanceFactor;
                }

                context.OnRole(i, BUFFER_MAIN, RoleType.Inserting);
                var elem = s.Read(i);
                var insertIdx = BinarySearchPositions(aux, positions, posCount, elem);

                if (!TryInsert(aux, ref auxEnd, auxSize, elem, occupied, positions, ref posCount, insertIdx, out var largeShift))
                {
                    // Gaps have clustered away from the insertion point. Rebalancing restores a
                    // uniform gap every few slots; it preserves element order, so insertIdx stays
                    // valid and the retry is guaranteed to find a gap.
                    auxEnd = Rebalance(aux, auxSize, occupied, positions, ref posCount, temp);
                    nextRebalance = sorted * RebalanceFactor;

                    if (!TryInsert(aux, ref auxEnd, auxSize, elem, occupied, positions, ref posCount, insertIdx, out largeShift))
                        throw new InvalidOperationException($"No gap available after rebalance (posCount={posCount}, auxSize={auxSize}, insertIdx={insertIdx})");
                }
                context.OnRole(i, BUFFER_MAIN, RoleType.None);
                sorted++;

                // Early rebalance if large shift was detected (gaps are clustering)
                if (largeShift && sorted < nextRebalance)
                {
                    auxEnd = Rebalance(aux, auxSize, occupied, positions, ref posCount, temp);
                    nextRebalance = sorted * RebalanceFactor;
                }
            }

            // Phase 3: Extract
            context.OnPhase(SortPhase.LibrarySortPhase, 3);
            if (posCount != length)
                throw new InvalidOperationException($"Data loss detected: expected {length} elements in the position index, but found {posCount}");

            for (var i = 0; i < length; i++)
            {
                s.Write(i, aux.Read(positions[i]));
            }
        }
        finally
        {
            var clearElements = RuntimeHelpers.IsReferenceOrContainsReferences<T>();
            ArrayPool<T>.Shared.Return(auxArray, clearElements);
            ArrayPool<bool>.Shared.Return(occupiedArray);
            ArrayPool<int>.Shared.Return(positionsArray);
            ArrayPool<T>.Shared.Return(tempArray, clearElements);
        }
    }

    /// <summary>
    /// Places elements with dynamic gap distribution and builds position buffer.
    /// Returns one past the highest occupied slot.
    /// </summary>
    private static int PlaceWithGaps<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> aux, SortSpan<T, TComparer, TContext> src,
        int srcStart, int count, int auxStart, int auxSize, Span<bool> occupied, Span<int> positions, out int posCount)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        posCount = 0;
        if (count == 0) return auxStart;

        // Range needed: (1+ε) * count
        var rangeNeeded = (int)Math.Ceiling(count * (1 + GapRatio));
        var rangeAvailable = auxSize - auxStart;

        // Strict validation: must have enough space for all elements
        if (rangeAvailable < count)
            throw new InvalidOperationException($"Insufficient auxiliary buffer space: need at least {count} positions, but only {rangeAvailable} available (auxStart={auxStart}, auxSize={auxSize})");

        // Use the minimum of needed and available, but ensure it's at least count
        var range = Math.Min(rangeNeeded, rangeAvailable);

        // Clear range (occupancy only; the element buffer keeps whatever it held)
        occupied.Slice(auxStart, range).Clear();

        // Distribute: pos[i] = floor(i * range / count)
        // This guarantees no collisions since range >= count
        var maxUsedPos = auxStart;
        for (var i = 0; i < count; i++)
        {
            var pos = auxStart + (int)((long)i * range / count);

            // Defensive check (should never happen with range >= count)
            if (pos >= auxSize)
                throw new InvalidOperationException($"Position overflow: calculated pos={pos}, but auxSize={auxSize} (i={i}, count={count}, range={range}, auxStart={auxStart})");

            aux.Write(pos, src.Read(srcStart + i));
            occupied[pos] = true;
            positions[posCount++] = pos;
            maxUsedPos = pos;
        }

        // Verify all elements were placed
        if (posCount != count)
            throw new InvalidOperationException($"Data loss detected: expected {count} elements, but only placed {posCount}");

        return maxUsedPos + 1;
    }

    /// <summary>
    /// Binary search in position buffer (O(log n)).
    /// </summary>
    private static int BinarySearchPositions<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> aux,
        Span<int> positions, int count, T value)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var left = 0;
        var right = count;

        while (left < right)
        {
            var mid = left + (right - left) / 2;
            // Routed through SortSpan so the read and the comparison stay observable. These are the
            // bulk of the algorithm's comparisons; calling the comparer directly hid them from
            // statistics and visualization consumers.
            var cmp = aux.Compare(value, positions[mid]);

            if (cmp < 0)
            {
                right = mid;
            }
            else
            {
                left = mid + 1; // Stable: insert after equal elements
            }
        }

        return left;
    }

    /// <summary>
    /// Inserts an element at <paramref name="insertIdx"/> in the position index and updates it incrementally.
    /// Returns false when no usable gap exists at or after the insertion point; the caller must then
    /// rebalance and retry with the same <paramref name="insertIdx"/>, which rebalancing keeps valid.
    /// <paramref name="largeShift"/> reports that a long shift was needed, suggesting an early rebalance.
    /// </summary>
    private static bool TryInsert<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> aux, ref int auxEnd, int maxSize,
        T value, Span<bool> occupied, Span<int> positions, ref int posCount, int insertIdx, out bool largeShift)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        largeShift = false;

        // insertIdx is the index in positions[], not the position in aux[]
        // We need to find the actual insertion position in aux[] based on the range
        int targetPos;
        int searchStart, searchEnd;

        if (insertIdx >= posCount)
        {
            // Insert at end (after last element)
            // Use maxSize instead of auxEnd to utilize all available space
            searchStart = posCount > 0 ? positions[posCount - 1] + 1 : 0;
            searchEnd = maxSize;
        }
        else if (insertIdx == 0)
        {
            // Insert at beginning (before first element)
            searchStart = 0;
            searchEnd = positions[0];
        }
        else
        {
            // Insert between positions[insertIdx-1] and positions[insertIdx]
            searchStart = positions[insertIdx - 1] + 1;
            searchEnd = positions[insertIdx];
        }

        // LibrarySort principle: larger range = more gaps available
        // Choose target based on insertion position to balance gap consumption and prevent clustering:
        // - Front insertion: prefer left side (near searchStart)
        // - Back insertion: prefer right side, but be careful with maxSize range
        // - Middle insertion: use midpoint to balance gap usage
        var rangeSize = searchEnd - searchStart;

        int gapTarget;
        if (insertIdx == 0)
        {
            // Front insertion: search from left to avoid clustering on right
            gapTarget = searchStart;
        }
        else if (insertIdx >= posCount)
        {
            // Back insertion: start from just after last element
            gapTarget = searchStart;
        }
        else
        {
            // Middle insertion: use midpoint to balance gap consumption
            gapTarget = searchStart + rangeSize / 2;
        }

        // Two-stage search to leverage large ranges:
        // Stage 1: Fast search with standard radius (O(1) expected for well-distributed gaps)
        // For back insertion with large range, cap the search radius to avoid excessive scanning
        // Protect against negative values when auxEnd < searchStart (can happen after rebalance with sparse tail)
        var effectiveRangeSize = insertIdx >= posCount
            ? Math.Min(rangeSize, Math.Max(0, auxEnd - searchStart) + MaxGapSearchDistance)
            : rangeSize;
        var searchRadius = Math.Min(effectiveRangeSize / 2, MaxGapSearchDistance);
        var gapPos = FindGapNear(occupied, gapTarget, searchStart, searchEnd, searchRadius);

        // Stage 2: If no gap found and range is large, expand search radius
        // This exploits LibrarySort's strength: larger range = more gaps available
        if (gapPos == -1 && effectiveRangeSize > MaxGapSearchDistance * 2)
        {
            var expandedRadius = Math.Min(effectiveRangeSize / 2, MaxGapSearchDistance * 2);
            gapPos = FindGapNear(occupied, gapTarget, searchStart, searchEnd, expandedRadius);
        }

        if (gapPos != -1)
        {
            // Gap found - use it directly
            aux.Write(gapPos, value);
            occupied[gapPos] = true;
            InsertPosition(positions, ref posCount, insertIdx, gapPos);
            // auxEnd must stay one past the highest occupied slot: back insertions routinely land
            // beyond it, and the shift path below reads it as the end of the populated region.
            if (gapPos >= auxEnd) auxEnd = gapPos + 1;
            return true;
        }

        // No gap in range - need to shift elements
        // Target position is determined by insertion index
        // For back insertion, use gapTarget (positions[posCount-1]+1) for consistency
        if (insertIdx >= posCount)
        {
            targetPos = gapTarget; // Consistent with gap search target
        }
        else
        {
            targetPos = positions[insertIdx];
        }

        // Find gap for shifting using local search from target position
        // LibrarySort principle: gaps should be nearby after proper rebalancing
        var shiftGap = FindGapNear(occupied, targetPos, targetPos, maxSize, MaxGapSearchDistance);

        if (shiftGap == -1)
        {
            // Gaps have clustered away from targetPos. Free slots may all lie to its left, so a
            // right shift cannot make room here; report failure and let the caller rebalance.
            return false;
        }

        // Check if shift distance is too large
        var shiftDistance = shiftGap - targetPos;
        largeShift = shiftDistance > MaxShiftDistanceBeforeRebalance;

        // Shift elements from targetPos to shiftGap.
        // FindGapNear returns the nearest gap at or after targetPos, so [targetPos, shiftGap) is
        // fully occupied and every element moved here is a real element.
        for (var i = shiftGap; i > targetPos; i--)
        {
            aux.Write(i, aux.Read(i - 1));
            occupied[i] = true;
        }

        // Update positions that were shifted
        // Optimization: positions is monotonically increasing, so only scan from insertIdx onwards
        // and break early when we pass shiftGap
        for (var i = insertIdx; i < posCount; i++)
        {
            var pos = positions[i];
            if (pos >= shiftGap)
                break; // Positions are sorted, no more updates needed

            if (pos >= targetPos)
            {
                positions[i] = pos + 1;
            }
        }

        // Write the new element
        aux.Write(targetPos, value);
        occupied[targetPos] = true;
        InsertPosition(positions, ref posCount, insertIdx, targetPos);

        // The shift consumed the gap at shiftGap, extending the populated region.
        if (shiftGap >= auxEnd) auxEnd = shiftGap + 1;

        return true;
    }

    /// <summary>
    /// Finds a gap near the target position using local search (expanding left and right).
    /// This approach aligns with LibrarySort's assumption that gaps are nearby,
    /// and is effective for detecting clustering.
    /// Returns -1 if no gap found within the search radius.
    /// Scans the occupancy map rather than the element buffer, so it neither reports reads
    /// nor touches element-sized memory.
    /// </summary>
    private static int FindGapNear(ReadOnlySpan<bool> occupied, int target, int start, int end, int maxRadius)
    {
        // Check target position first
        if (target >= start && target < end && !occupied[target])
            return target;

        // Expand search radius alternating left and right
        for (var radius = 1; radius <= maxRadius; radius++)
        {
            // Check right
            var right = target + radius;
            if (right >= start && right < end && !occupied[right])
                return right;

            // Check left
            var left = target - radius;
            if (left >= start && left < end && !occupied[left])
                return left;
        }

        return -1;
    }

    /// <summary>
    /// Inserts a position into the sorted position buffer.
    /// Uses Span.CopyTo for efficient bulk memory copy instead of element-by-element iteration.
    /// Note: Statistics tracking is skipped for performance.
    /// </summary>
    private static void InsertPosition(Span<int> positions, ref int count, int idx, int pos)
    {
        // Shift elements: use Span.CopyTo for efficient memory copy
        if (idx < count)
        {
            var source = positions.Slice(idx, count - idx);
            var dest = positions.Slice(idx + 1, count - idx);
            source.CopyTo(dest);
        }
        positions[idx] = pos;
        count++;
    }

    /// <summary>
    /// Rebalances with dynamic spacing to prevent data loss.
    /// Stages every element through <paramref name="tempBuffer"/>, marks the whole auxiliary buffer
    /// as gaps, then redistributes with uniform spacing over range = min((1+ε)*count, auxSize).
    /// Element order is preserved, so a position index computed before the call stays valid.
    /// Returns the maximum used position + 1 for auxEnd tracking.
    /// </summary>
    private static int Rebalance<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> aux, int auxSize,
        Span<bool> occupied, Span<int> positions, ref int posCount, Span<T> tempBuffer)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Collect elements. Every entry of positions[] refers to an occupied slot by construction.
        var count = posCount;
        for (var i = 0; i < count; i++)
        {
            tempBuffer[i] = aux.Read(positions[i]);
        }

        // Calculate new range: (1+ε) * count
        var rangeNeeded = (int)Math.Ceiling(count * (1 + GapRatio));

        // Strict validation: must have enough space for all elements
        if (auxSize < count)
        {
            throw new InvalidOperationException(
                $"Insufficient auxiliary buffer space for rebalance: need at least {count} positions, " +
                $"but auxSize={auxSize}. This indicates the buffer was too small from the start.");
        }

        var range = Math.Min(rangeNeeded, auxSize);

        // Clear the whole map, not just [0, range): back insertions can occupy slots beyond range,
        // and leaving those marked would strand usable space and block later gap searches.
        occupied.Clear();

        // Redistribute: pos[i] = floor(i * range / count)
        // This guarantees no collisions since range >= count
        posCount = 0;
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
            occupied[pos] = true;
            positions[posCount++] = pos;
            maxUsedPos = pos;
        }

        // Verify all elements were placed
        if (posCount != count)
        {
            throw new InvalidOperationException(
                $"Data loss detected in rebalance: expected {count} elements, but only placed {posCount}");
        }

        // Return the maximum used position + 1
        // This represents the true auxEnd after rebalancing
        return maxUsedPos + 1;
    }
}
