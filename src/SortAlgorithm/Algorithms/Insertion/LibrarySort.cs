using System.Buffers;
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
/// Mark gap positions (null or sentinel). Start with small sorted region using standard insertion sort.</description></item>
/// <item><description><strong>Insertion Loop:</strong> For each new element:
/// - Binary search among non-gap elements to find insertion position
/// - If position has gap, write directly; otherwise shift right until gap found
/// - Handle equal elements with randomization to maintain gap distribution</description></item>
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
/// <item><description>Gap Representation: Use nullable wrapper or max value as sentinel for gaps</description></item>
/// <item><description>Spacing: After rebalancing, distribute elements uniformly with gap:element ratio = ε:1</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family        : Insertion (gap-based variant)</description></item>
/// <item><description>Stable        : Yes (equal elements maintain relative order during shifts)</description></item>
/// <item><description>In-place      : No (requires (1+ε)n auxiliary space for gaps)</description></item>
/// <item><description>Best case     : O(n) - Already sorted with uniformly distributed gaps</description></item>
/// <item><description>Average case  : O(n log n) - With random input and good gap distribution</description></item>
/// <item><description>Worst case    : O(n²) - Pathological gap clustering without randomization</description></item>
/// <item><description>Space         : O(n) - Auxiliary array of size (1+ε)n ≈ 1.5n to 2n</description></item>
/// <item><description>Binary Search : O(log n) per insertion to find position</description></item>
/// <item><description>Shift Cost    : O(log n) average per insertion with good gaps</description></item>
/// <item><description>Rebalance     : O(n) total across all rebalancing operations (amortized O(1))</description></item>
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
    private const int BUFFER_POSITIONS = 2;   // Position buffer (tracks non-gap element positions)

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
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort in place.</param>
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
    {
        Sort(span, NullContext.Default);
    }

    /// <summary>
    /// Sorts the elements in the specified span using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context for tracking statistics and observations during sorting. Cannot be null.</param>
    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        var length = span.Length;
        if (length <= 1) return;

        // For very small arrays, use standard insertion sort
        if (length <= SmallSortThreshold)
        {
            InsertionSort.Sort(span, context);
            return;
        }

        var s = new SortSpan<T>(span, context, BUFFER_MAIN);
        SortCore(s, length, context);
    }

    /// <summary>
    /// Core sorting logic with proper gap management and O(log n) search.
    /// </summary>
    private static void SortCore<T>(SortSpan<T> s, int length, ISortContext context) where T : IComparable<T>
    {
        // Auxiliary array size: (1+ε)n with safety margin
        // With ε=0.5: (1.5 * 1.05)n ≈ 1.575n
        var auxSize = (int)Math.Ceiling(length * (1 + GapRatio) * AuxSizeSafetyMargin);

        var auxArray = ArrayPool<LibraryElement<T>>.Shared.Rent(auxSize);
        var positionsArray = ArrayPool<int>.Shared.Rent(length);
        var tempArray = ArrayPool<T>.Shared.Rent(length);

        try
        {
            var aux = new SortSpan<LibraryElement<T>>(auxArray.AsSpan(0, auxSize), context, BUFFER_AUX);
            var positions = new SortSpan<int>(positionsArray.AsSpan(0, length), context, BUFFER_POSITIONS);

            // Initialize as gaps
            var gap = new LibraryElement<T>();
            for (var i = 0; i < auxSize; i++)
            {
                aux.Write(i, gap);
            }

            // Phase 1: Initial sort
            var initSize = Math.Min(SmallSortThreshold, length);
            InsertionSort.SortCore(s, 0, initSize);

            // Place with gaps and build initial position buffer
            var auxEnd = PlaceWithGaps(aux, s, 0, initSize, 0, auxSize, positions, out var posCount);

            var sorted = initSize;
            var nextRebalance = initSize * RebalanceFactor;

            // Phase 2: Insert remaining
            for (var i = initSize; i < length; i++)
            {
                if (sorted >= nextRebalance)
                {
                    auxEnd = Rebalance(aux, auxSize, positions, ref posCount, tempArray);
                    nextRebalance = sorted * RebalanceFactor;
                }

                var elem = s.Read(i);
                var insertIdx = BinarySearchPositions(aux, positions, posCount, elem);

                var needsRebalance = InsertAndUpdate(aux, ref auxEnd, auxSize, elem, positions, ref posCount, insertIdx);
                sorted++;

                // Early rebalance if large shift was detected (gaps are clustering)
                if (needsRebalance && sorted < nextRebalance)
                {
                    auxEnd = Rebalance(aux, auxSize, positions, ref posCount, tempArray);
                    nextRebalance = sorted * RebalanceFactor;
                }
            }

            // Phase 3: Extract
            for (var i = 0; i < posCount && i < length; i++)
            {
                var pos = positions.Read(i);
                s.Write(i, aux.Read(pos).Value);
            }
        }
        finally
        {
            ArrayPool<LibraryElement<T>>.Shared.Return(auxArray, RuntimeHelpers.IsReferenceOrContainsReferences<LibraryElement<T>>());
            ArrayPool<int>.Shared.Return(positionsArray);
            ArrayPool<T>.Shared.Return(tempArray, RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    /// <summary>
    /// Places elements with dynamic gap distribution and builds position buffer.
    /// </summary>
    private static int PlaceWithGaps<T>(SortSpan<LibraryElement<T>> aux, SortSpan<T> src,
        int srcStart, int count, int auxStart, int auxSize, SortSpan<int> positions, out int posCount)
        where T : IComparable<T>
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

        // Clear range
        var gap = new LibraryElement<T>();
        for (var i = 0; i < range; i++)
        {
            aux.Write(auxStart + i, gap);
        }

        // Distribute: pos[i] = floor(i * range / count)
        // This guarantees no collisions since range >= count
        for (var i = 0; i < count; i++)
        {
            var pos = auxStart + (int)((long)i * range / count);
            
            // Defensive check (should never happen with range >= count)
            if (pos >= auxSize)
                throw new InvalidOperationException($"Position overflow: calculated pos={pos}, but auxSize={auxSize} (i={i}, count={count}, range={range}, auxStart={auxStart})");

            aux.Write(pos, new LibraryElement<T>(src.Read(srcStart + i)));
            positions.Write(posCount++, pos);
        }
        
        // Verify all elements were placed
        if (posCount != count)
            throw new InvalidOperationException($"Data loss detected: expected {count} elements, but only placed {posCount}");

        return auxStart + range;
    }

    /// <summary>
    /// Binary search in position buffer (O(log n)).
    /// </summary>
    private static int BinarySearchPositions<T>(SortSpan<LibraryElement<T>> aux,
        SortSpan<int> positions, int count, T value) where T : IComparable<T>
    {
        var left = 0;
        var right = count;

        while (left < right)
        {
            var mid = left + (right - left) / 2;
            var cmp = value.CompareTo(aux.Read(positions.Read(mid)).Value);

            if (cmp < 0)
            {
                right = mid;
            }
            else
            {
                left = mid + 1; // Stable
            }
        }

        return left;
    }

    /// <summary>
    /// Inserts element and updates position buffer incrementally.
    /// Returns true if a large shift occurred (suggesting rebalance is needed).
    /// </summary>
    private static bool InsertAndUpdate<T>(SortSpan<LibraryElement<T>> aux, ref int auxEnd, int maxSize,
        T value, SortSpan<int> positions, ref int posCount, int insertIdx) where T : IComparable<T>
    {
        // insertIdx is the index in positions[], not the position in aux[]
        // We need to find the actual insertion position in aux[] based on the range
        int targetPos;
        int searchStart, searchEnd;

        if (insertIdx >= posCount)
        {
            // Insert at end (after last element)
            searchStart = posCount > 0 ? positions.Read(posCount - 1) + 1 : 0;
            searchEnd = auxEnd;
        }
        else if (insertIdx == 0)
        {
            // Insert at beginning (before first element)
            searchStart = 0;
            searchEnd = positions.Read(0);
        }
        else
        {
            // Insert between positions[insertIdx-1] and positions[insertIdx]
            searchStart = positions.Read(insertIdx - 1) + 1;
            searchEnd = positions.Read(insertIdx);
        }

        // Try to find a gap in the target range
        var gapPos = FindGap(aux, searchStart, searchEnd);

        if (gapPos != -1)
        {
            // Gap found - use it directly
            aux.Write(gapPos, new LibraryElement<T>(value));
            InsertPosition(positions, ref posCount, insertIdx, gapPos);
            return false; // No large shift
        }

        // No gap in range - need to shift elements
        // Target position is where we want to insert (at searchEnd, which is positions[insertIdx])
        targetPos = insertIdx < posCount ? positions.Read(insertIdx) : auxEnd;

        // Find gap to the right for shifting
        var shiftGap = FindGap(aux, targetPos, Math.Min(auxEnd + MaxGapSearchDistance, maxSize));

        if (shiftGap == -1)
        {
            // No gap found - extend array
            if (auxEnd >= maxSize)
                throw new InvalidOperationException("No gap and buffer full");
            shiftGap = auxEnd++;
        }

        // Check if shift distance is too large
        var shiftDistance = shiftGap - targetPos;
        var largeShift = shiftDistance > MaxShiftDistanceBeforeRebalance;

        // Shift elements from targetPos to shiftGap
        for (var i = shiftGap; i > targetPos; i--)
        {
            aux.Write(i, aux.Read(i - 1));
        }

        // Update positions that were shifted
        // Optimization: positions is monotonically increasing, so only scan from insertIdx onwards
        // and break early when we pass shiftGap
        for (var i = insertIdx; i < posCount; i++)
        {
            var pos = positions.Read(i);
            if (pos >= shiftGap)
                break; // Positions are sorted, no more updates needed
            if (pos >= targetPos)
            {
                positions.Write(i, pos + 1);
            }
        }

        // Write the new element
        aux.Write(targetPos, new LibraryElement<T>(value));
        InsertPosition(positions, ref posCount, insertIdx, targetPos);

        // Update auxEnd to include the shift gap position
        // Use Math.Max to handle the case where we extended the array (shiftGap = auxEnd++ above)
        auxEnd = Math.Max(auxEnd, shiftGap + 1);

        return largeShift; // Return true if rebalance is recommended
    }

    private static int FindGap<T>(SortSpan<LibraryElement<T>> aux, int start, int end)
        where T : IComparable<T>
    {
        for (var i = start; i < end; i++)
        {
            if (!aux.Read(i).HasValue)
            {
                return i;
            }
        }
        return -1;
    }

    private static void InsertPosition(SortSpan<int> positions, ref int count, int idx, int pos)
    {
        for (var i = count; i > idx; i--)
        {
            positions.Write(i, positions.Read(i - 1));
        }
        positions.Write(idx, pos);
        count++;
    }

    /// <summary>
    /// Rebalances with dynamic spacing to prevent data loss.
    /// </summary>
    private static int Rebalance<T>(SortSpan<LibraryElement<T>> aux, int auxSize,
        SortSpan<int> positions, ref int posCount, T[] tempBuffer) where T : IComparable<T>
    {
        // Collect elements
        var count = 0;
        for (var i = 0; i < posCount; i++)
        {
            var elem = aux.Read(positions.Read(i));
            if (elem.HasValue)
            {
                tempBuffer[count++] = elem.Value;
            }
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

        // Clear
        var gap = new LibraryElement<T>();
        for (var i = 0; i < range; i++)
        {
            aux.Write(i, gap);
        }

        // Redistribute: pos[i] = floor(i * range / count)
        // This guarantees no collisions since range >= count
        posCount = 0;
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

            aux.Write(pos, new LibraryElement<T>(tempBuffer[i]));
            positions.Write(posCount++, pos);
        }
        
        // Verify all elements were placed
        if (posCount != count)
        {
            throw new InvalidOperationException(
                $"Data loss detected in rebalance: expected {count} elements, but only placed {posCount}");
        }

        return range;
    }

    /// <summary>
    /// Wrapper struct for gaps vs elements.
    /// </summary>
    private readonly struct LibraryElement<T> : IComparable<LibraryElement<T>> where T : IComparable<T>
    {
        public readonly T Value;
        public readonly bool HasValue;

        public LibraryElement(T value)
        {
            Value = value;
            HasValue = true;
        }

        public int CompareTo(LibraryElement<T> other)
        {
            if (!HasValue && !other.HasValue) return 0;
            if (!HasValue) return -1;
            if (!other.HasValue) return 1;
            return Value.CompareTo(other.Value);
        }
    }
}
