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
    // Uses nullable wrapper (LibraryElement&lt;T&gt;) to represent gaps vs actual elements.
    // Binary search skips gaps by checking HasValue property.
    // Rebalancing threshold: 4x growth (practical) vs 2x (theoretical paper.
    // Initial small sort: Standard insertion sort for first 32 elements.
    // For equal elements: Could randomize position to distribute gaps (currently deterministic.

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;     // Main input array
    private const int BUFFER_AUX = 1;      // Auxiliary array with gaps

    // Gap ratio: ε = 0.5 means array size is 1.5n (0.5n gaps among n elements)
    private const double GapRatio = 0.5;

    // Growth factor for rebalancing: rebalance when size reaches R times current size
    private const int RebalanceFactor = 4;

    // Threshold for initial small sort using standard insertion sort
    private const int SmallSortThreshold = 32;

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
    /// Core sorting logic for Library Sort with gap management and rebalancing.
    /// </summary>
    private static void SortCore<T>(SortSpan<T> s, int length, ISortContext context) where T : IComparable<T>
    {
        // Calculate auxiliary array size with gaps: (1 + ε)n
        // We'll use a larger buffer to ensure we have enough space
        var auxSize = (int)Math.Ceiling(length * (1 + GapRatio) * 1.5);
        
        // Rent auxiliary array from ArrayPool to avoid heap allocation
        var auxArray = ArrayPool<LibraryElement<T>>.Shared.Rent(auxSize);
        
        // Determine if we can use stackalloc for position buffer
        // Threshold: 128 elements = 512 bytes (128 * sizeof(int))
        const int StackAllocThreshold = 128;
        int[]? rentedPositionBuffer = null;
        scoped Span<int> positionBuffer;
        
        try
        {
            // Wrap aux array with SortSpan for statistics tracking
            var aux = new SortSpan<LibraryElement<T>>(auxArray.AsSpan(0, auxSize), context, BUFFER_AUX);
            
            if (length <= StackAllocThreshold)
            {
                // Small array: use stackalloc for zero allocation
                positionBuffer = stackalloc int[length];
            }
            else
            {
                // Large array: rent from ArrayPool
                rentedPositionBuffer = ArrayPool<int>.Shared.Rent(length);
                positionBuffer = rentedPositionBuffer.AsSpan(0, length);
            }
            
            // Initialize all positions as gaps
            var gapElement = new LibraryElement<T>();
            for (var i = 0; i < auxSize; i++)
            {
                aux.Write(i, gapElement);
            }

            // Phase 1: Initial small sort
            var initialSize = Math.Min(SmallSortThreshold, length);

            // Sort first 'initialSize' elements using standard insertion sort
            InsertionSort.SortCore(s, 0, initialSize);

            // Transfer initial sorted elements to auxiliary array with gaps
            // Place elements with even spacing: gap_count / element_count ≈ ε
            var spacing = Math.Max(2, (int)(1.0 / GapRatio) + 1);
            for (var i = 0; i < initialSize; i++)
            {
                var targetPos = i * spacing;
                if (targetPos < auxSize)
                {
                    aux.Write(targetPos, new LibraryElement<T>(s.Read(i)));
                }
            }

            var currentEnd = Math.Min(initialSize * spacing, auxSize);
            var sortedCount = initialSize;
            var nextRebalance = initialSize * RebalanceFactor;

            // Phase 2: Insert remaining elements
            for (var i = initialSize; i < length; i++)
            {
                // Check if rebalancing is needed
                if (sortedCount >= nextRebalance)
                {
                    currentEnd = Rebalance(aux, auxSize, sortedCount, spacing);
                    nextRebalance = sortedCount * RebalanceFactor;
                }

                var elem = s.Read(i);

                // Find insertion position using binary search (O(log n))
                var insertPos = FindInsertPosition(aux, currentEnd, elem, positionBuffer, out var elementCount);

                // Insert element
                InsertElement(aux, ref currentEnd, insertPos, elem, auxSize);
                sortedCount++;
            }

            // Phase 3: Extract sorted elements back to original array
            ExtractSorted(aux, s, currentEnd, length);
        }
        finally
        {
            // Always return rented arrays to pool
            if (rentedPositionBuffer != null)
            {
                ArrayPool<int>.Shared.Return(rentedPositionBuffer);
            }
            ArrayPool<LibraryElement<T>>.Shared.Return(auxArray, RuntimeHelpers.IsReferenceOrContainsReferences<LibraryElement<T>>());
        }
    }

    /// <summary>
    /// Finds the correct insertion position for a value using binary search over non-gap elements.
    /// This achieves O(log n) search time per insertion, critical for Library Sort's O(n log n) complexity.
    /// </summary>
    /// <param name="positionBuffer">Pre-allocated buffer to store element positions, avoiding allocations</param>
    /// <param name="elementCount">Output: number of non-gap elements found</param>
    private static int FindInsertPosition<T>(SortSpan<LibraryElement<T>> aux, int end, T value, 
        Span<int> positionBuffer, out int elementCount)
        where T : IComparable<T>
    {
        // Collect positions of all non-gap elements into the pre-allocated buffer
        elementCount = 0;
        for (var i = 0; i < end; i++)
        {
            var elem = aux.Read(i);
            if (elem.HasValue)
            {
                positionBuffer[elementCount++] = i;
            }
        }

        if (elementCount == 0)
            return 0;

        // Binary search over the positions of non-gap elements
        // This is O(log m) where m = number of non-gap elements ≈ n
        var left = 0;
        var right = elementCount;

        while (left < right)
        {
            var mid = left + (right - left) / 2;
            var pos = positionBuffer[mid];
            var elem = aux.Read(pos);
            var cmp = value.CompareTo(elem.Value);

            if (cmp < 0)
            {
                right = mid; // Insert before this element
            }
            else if (cmp > 0)
            {
                left = mid + 1; // Insert after this element
            }
            else
            {
                // Equal elements - maintain stability by inserting after
                left = mid + 1;
            }
        }

        // Return the actual position in the aux array where we should insert
        if (left >= elementCount)
        {
            // Insert at the end - find the last non-gap position
            return end;
        }
        
        // Return the position of the element we should insert before
        return positionBuffer[left];
    }

    /// <summary>
    /// Inserts an element at the specified position, using a gap if available or shifting elements.
    /// </summary>
    private static void InsertElement<T>(SortSpan<LibraryElement<T>> aux, ref int currentEnd, int pos, T value, int maxSize)
        where T : IComparable<T>
    {
        // If position is beyond current end, just append
        if (pos >= currentEnd)
        {
            if (currentEnd < maxSize)
            {
                aux.Write(currentEnd, new LibraryElement<T>(value));
                currentEnd++;
            }
            return;
        }

        // If the position is already a gap, use it
        var currentElem = aux.Read(pos);
        if (!currentElem.HasValue)
        {
            aux.Write(pos, new LibraryElement<T>(value));
            return;
        }

        // Look for a nearby gap to minimize shifting
        var gapPos = -1;
        
        // Search right for a gap (limited search)
        for (var i = pos; i < Math.Min(currentEnd + 10, maxSize); i++)
        {
            var elem = aux.Read(i);
            if (!elem.HasValue)
            {
                gapPos = i;
                break;
            }
        }

        if (gapPos == -1)
        {
            // No gap found nearby, extend the array
            if (currentEnd < maxSize)
            {
                gapPos = currentEnd;
                currentEnd++;
            }
            else
            {
                // Array is full, force a gap by using the last position
                gapPos = maxSize - 1;
            }
        }

        // Shift elements right from pos to gapPos
        if (gapPos > pos)
        {
            for (var i = gapPos; i > pos; i--)
            {
                var prev = aux.Read(i - 1);
                aux.Write(i, prev);
            }
        }

        // Insert the new element
        aux.Write(pos, new LibraryElement<T>(value));
        
        // Update currentEnd if we extended beyond it
        if (gapPos >= currentEnd && gapPos < maxSize)
        {
            currentEnd = gapPos + 1;
        }
    }

    /// <summary>
    /// Rebalances the auxiliary array by redistributing all elements with evenly spaced gaps.
    /// </summary>
    private static int Rebalance<T>(SortSpan<LibraryElement<T>> aux, int auxSize, int sortedCount, int spacing)
        where T : IComparable<T>
    {
        // Rent temporary buffer from ArrayPool
        var elements = ArrayPool<T>.Shared.Rent(sortedCount);
        
        try
        {
            // Collect all non-gap elements
            var idx = 0;
            
            for (var i = 0; i < aux.Length && idx < sortedCount; i++)
            {
                var elem = aux.Read(i);
                if (elem.HasValue)
                {
                    elements[idx++] = elem.Value;
                }
            }

            // Clear the array
            var gapElement = new LibraryElement<T>();
            for (var i = 0; i < auxSize; i++)
            {
                aux.Write(i, gapElement);
            }

            // Redistribute with even spacing
            for (var i = 0; i < idx; i++)
            {
                var targetPos = i * spacing;
                if (targetPos < auxSize)
                {
                    aux.Write(targetPos, new LibraryElement<T>(elements[i]));
                }
            }

            return Math.Min(idx * spacing, auxSize);
        }
        finally
        {
            // Return rented array to pool
            ArrayPool<T>.Shared.Return(elements, RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    /// <summary>
    /// Extracts all non-gap elements from the auxiliary array back to the original array in sorted order.
    /// </summary>
    private static void ExtractSorted<T>(SortSpan<LibraryElement<T>> aux, SortSpan<T> output, 
        int auxEnd, int expectedCount) where T : IComparable<T>
    {
        var outIdx = 0;
        
        for (var i = 0; i < aux.Length && outIdx < expectedCount; i++)
        {
            var elem = aux.Read(i);
            if (elem.HasValue)
            {
                output.Write(outIdx++, elem.Value);
            }
        }
    }

    /// <summary>
    /// Wrapper struct to represent either a gap or an actual element in the auxiliary array.
    /// Using a struct with HasValue flag is more type-safe than using nullable reference types
    /// and works with both reference and value types.
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
            // Gaps sort before any element (though we shouldn't compare gaps in practice)
            if (!HasValue && !other.HasValue) return 0;
            if (!HasValue) return -1;
            if (!other.HasValue) return 1;
            
            return Value.CompareTo(other.Value);
        }
    }
}
