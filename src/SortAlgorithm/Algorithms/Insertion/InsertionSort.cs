using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// シンプルで効率的な挿入ソートアルゴリズムです。配列の先頭部分をソート済み領域として維持し、
/// 未ソート領域の各要素を適切な位置に挿入していきます。シフト操作により安定性を保ちながら効率的にソートします。
/// <br/>
/// A simple and efficient insertion sort algorithm that maintains a sorted region at the beginning of the array,
/// inserting each element from the unsorted region into its appropriate position. Uses shift operations to maintain stability efficiently.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Insertion Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Maintain Sorted Invariant:</strong> For each position i (from 1 to n-1), 
/// the subarray [0..i-1] must remain sorted before inserting element at position i.
/// This invariant is established initially (single element is sorted) and preserved through each iteration.</description></item>
/// <item><description><strong>Linear Search and Shift:</strong> For each element at position i, 
/// compare it with elements in the sorted region [0..i-1] from right to left.
/// Shift elements greater than the current element one position to the right until finding the correct insertion position.
/// This ensures all elements remain in order.</description></item>
/// <item><description><strong>Stable Insertion via Comparison:</strong> Use strict inequality (Compare(j, tmp) &gt; 0) 
/// to determine if an element should be shifted. Equal elements are NOT shifted, preserving their original relative order.
/// This guarantees stability of the sort.</description></item>
/// <item><description><strong>Optimization for Already-Sorted Elements:</strong> If no elements were shifted (j == i-1),
/// the element is already in the correct position, so skip the write operation.
/// This optimization achieves O(n) best case for sorted arrays with zero writes.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Insertion</description></item>
/// <item><description>Stable      : Yes (strict inequality &gt; preserves order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space, only temp variable for element being inserted)</description></item>
/// <item><description>Best case   : O(n) - Already sorted array, only n-1 comparisons, zero writes</description></item>
/// <item><description>Average case: O(n²) - On average, each element shifts halfway back (n²/4 comparisons and writes)</description></item>
/// <item><description>Worst case  : O(n²) - Reverse sorted array, maximum shifts: n(n-1)/2 comparisons and writes</description></item>
/// <item><description>Comparisons : O(n²) - Sum of comparisons for each insertion, worst case: n(n-1)/2</description></item>
/// <item><description>Writes      : O(n²) - Each shift writes one element, worst case: n(n-1)/2 shifts + (n-1) final insertions</description></item>
/// <item><description>Reads       : O(n²) - Each comparison reads one element, each shift reads the element being moved</description></item>
/// </list>
/// <para><strong>Advantages of Insertion Sort:</strong></para>
/// <list type="bullet">
/// <item><description>Simple implementation - Easy to understand and code correctly</description></item>
/// <item><description>Efficient for small datasets - Lower constant factors than O(n log n) algorithms</description></item>
/// <item><description>Adaptive - Runs in O(n) time for nearly sorted data</description></item>
/// <item><description>Stable - Preserves relative order of equal elements</description></item>
/// <item><description>In-place - Requires only O(1) additional memory</description></item>
/// <item><description>Online - Can sort data as it arrives (streaming)</description></item>
/// </list>
/// <para><strong>Use Cases:</strong></para>
/// <list type="bullet">
/// <item><description>Small arrays (typically n &lt; 10-50, depending on hardware)</description></item>
/// <item><description>Nearly sorted data (append operations, slightly shuffled data)</description></item>
/// <item><description>As the final step in hybrid algorithms (e.g., Timsort, Introsort)</description></item>
/// <item><description>When stability is required with minimal memory overhead</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Insertion_sort</para>
/// </remarks>
public static class InsertionSort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array

    /// <summary>
    /// Sorts the elements in the specified span in ascending order using the default comparer.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort in place.</param>
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
    {
        Sort(span, 0, span.Length, NullContext.Default);
    }

    /// <summary>
    /// Sorts the elements in the specified span using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context for tracking statistics and observations during sorting. Cannot be null.</param>
    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        Sort(span, 0, span.Length, context);
    }

    /// <summary>
    /// Sorts the subrange [first..last) using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span containing elements to sort.</param>
    /// <param name="first">The inclusive start index of the range to sort.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    /// <param name="context">The sort context for tracking statistics and observations.</param>
    /// <remarks>
    /// This implementation uses shift operations instead of swaps for better performance:
    /// - Reads the element to be inserted into a temporary variable
    /// - Shifts larger elements one position to the right
    /// - Writes the element to its correct position
    /// This reduces the number of writes compared to swap-based insertion sort.
    /// </remarks>
    public static void Sort<T>(Span<T> span, int first, int last, ISortContext context) where T : IComparable<T>
    {
        ArgumentOutOfRangeException.ThrowIfNegative(first);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(last, span.Length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(first, last);

        if (last - first <= 1) return;

        var s = new SortSpan<T>(span, context, BUFFER_MAIN);
        SortCore(s, first, last);
    }

    /// <summary>
    /// Sorts the subrange [first..last) using the provided sort context.
    /// This overload accepts a SortSpan directly for use by other algorithms that already have a SortSpan instance.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="first">The inclusive start index of the range to sort.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    internal static void SortCore<T>(SortSpan<T> s, int first, int last) where T : IComparable<T>
    {
        for (var i = first + 1; i < last; i++)
        {
            // Temporarily store the value to be inserted
            var tmp = s.Read(i);

            // Shift elements larger than tmp to the right
            // Use strict inequality (>) to maintain stability
            var j = i - 1;
            while (j >= first && s.Compare(j, tmp) > 0)
            {
                s.Write(j + 1, s.Read(j));
                j--;
            }

            // Insert tmp into the correct position only if elements were shifted
            // Optimization: if j == i-1, element is already in correct position
            if (j != i - 1)
            {
                s.Write(j + 1, tmp);
            }
        }
    }

    /// <summary>
    /// Unguarded insertion sort: assumes that there is an element at position (first - 1) 
    /// that is less than or equal to all elements in the range [first, last).
    /// This assumption allows us to skip boundary checks in the inner loop, improving performance.
    /// This is used by IntroSort for sorting non-leftmost partitions where the pivot acts as a sentinel.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="first">The inclusive start index of the range to sort. Must be > 0.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    /// <remarks>
    /// PRECONDITION: first > 0 and s[first-1] <= s[i] for all i in [first, last)
    /// This precondition is guaranteed by IntroSort's partitioning scheme.
    /// Violating this precondition will cause out-of-bounds access.
    /// 
    /// Performance improvement: Removes the (j >= first) boundary check from the inner loop,
    /// reducing branch mispredictions and improving CPU pipeline efficiency.
    /// Typical speedup: 10-20% for small arrays compared to guarded version.
    /// </remarks>
    internal static void UnguardedSortCore<T>(SortSpan<T> s, int first, int last) where T : IComparable<T>
    {
        for (var i = first + 1; i < last; i++)
        {
            var tmp = s.Read(i);
            var j = i - 1;
            
            // No boundary check (j >= first) needed because of the sentinel at (first - 1)
            // The element at (first - 1) is guaranteed to be <= tmp, so the loop will stop
            while (s.Compare(j, tmp) > 0)
            {
                s.Write(j + 1, s.Read(j));
                j--;
            }

            if (j != i - 1)
            {
                s.Write(j + 1, tmp);
            }
        }
    }

    /// <summary>
    /// Attempts to sort the subrange [first..last) using insertion sort, but gives up early if the array appears unsorted.
    /// This is an optimization for nearly-sorted arrays: if too many insertions are required, returns false
    /// to indicate the array is not nearly sorted and a different algorithm should be used.
    /// This is similar to C++ std::introsort's __insertion_sort_incomplete optimization.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="first">The inclusive start index of the range to sort.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    /// <param name="leftmost">True if this is the leftmost partition (requires boundary checks),
    /// false otherwise (can use unguarded version with sentinel).</param>
    /// <returns>True if the range was successfully sorted or is very small (&lt;= 5 elements),
    /// false if sorting appears to require significant work (more than 8 insertions detected).</returns>
    /// <remarks>
    /// This method implements the "give up early" optimization from LLVM libcxx's __insertion_sort_incomplete:
    /// - For very small arrays (0-5 elements): Always completes sorting and returns true
    /// - For larger arrays: Tracks insertion count; if more than 8 insertions are needed, gives up and returns false
    /// 
    /// The threshold of 8 insertions is based on empirical observation that:
    /// - Nearly sorted arrays typically need very few insertions (0-8)
    /// - Truly unsorted arrays need many insertions, making insertion sort inefficient
    /// - Giving up early prevents wasting time on insertion sort when QuickSort/HeapSort would be faster
    /// 
    /// This is particularly effective for IntroSort when swap count is zero (potential nearly-sorted partition):
    /// - If both partitions return true: entire range is sorted, done
    /// - If one returns false: that partition needs further QuickSort/HeapSort
    /// - If one returns true: one partition done, only recurse on the other
    /// </remarks>
    internal static bool SortIncomplete<T>(SortSpan<T> s, int first, int last, bool leftmost) where T : IComparable<T>
    {
        var length = last - first;

        // Handle small cases directly (0-5 elements)
        switch (length)
        {
            case 0:
            case 1:
                return true;
            case 2:
                if (s.Compare(first + 1, first) < 0)
                {
                    s.Swap(first, first + 1);
                }
                return true;
            case 3:
                // Sort 3 elements using sorting network
                Sort3(s, first, first + 1, first + 2);
                return true;
            case 4:
                // Sort 4 elements using sorting network
                Sort4(s, first, first + 1, first + 2, first + 3);
                return true;
            case 5:
                // Sort 5 elements using sorting network
                Sort5(s, first, first + 1, first + 2, first + 3, first + 4);
                return true;
        }

        // For larger arrays, perform insertion sort but give up if too many insertions are needed
        const int insertionLimit = 8;
        var insertionCount = 0;

        if (leftmost)
        {
            // Guarded version for leftmost partition
            for (var i = first + 1; i < last; i++)
            {
                var tmp = s.Read(i);
                var j = i - 1;

                if (s.Compare(j, tmp) > 0)
                {
                    // Element needs to be inserted (not already in correct position)
                    if (++insertionCount >= insertionLimit)
                    {
                        return false; // Too many insertions, give up
                    }

                    while (j >= first && s.Compare(j, tmp) > 0)
                    {
                        s.Write(j + 1, s.Read(j));
                        j--;
                    }

                    s.Write(j + 1, tmp);
                }
            }
        }
        else
        {
            // Unguarded version for non-leftmost partition (has sentinel)
            for (var i = first + 1; i < last; i++)
            {
                var tmp = s.Read(i);
                var j = i - 1;

                if (s.Compare(j, tmp) > 0)
                {
                    // Element needs to be inserted
                    if (++insertionCount >= insertionLimit)
                    {
                        return false; // Too many insertions, give up
                    }

                    while (s.Compare(j, tmp) > 0)
                    {
                        s.Write(j + 1, s.Read(j));
                        j--;
                    }

                    s.Write(j + 1, tmp);
                }
            }
        }

        return true; // Successfully sorted without exceeding insertion limit
    }

    /// <summary>
    /// Sorts exactly 3 elements using a sorting network (2-3 comparisons, 0-2 swaps).
    /// </summary>
    private static void Sort3<T>(SortSpan<T> s, int i0, int i1, int i2) where T : IComparable<T>
    {
        if (s.Compare(i1, i0) < 0) s.Swap(i0, i1);
        if (s.Compare(i2, i1) < 0)
        {
            s.Swap(i1, i2);
            if (s.Compare(i1, i0) < 0) s.Swap(i0, i1);
        }
    }

    /// <summary>
    /// Sorts exactly 4 elements using a sorting network (3-6 comparisons, 0-5 swaps).
    /// </summary>
    private static void Sort4<T>(SortSpan<T> s, int i0, int i1, int i2, int i3) where T : IComparable<T>
    {
        Sort3(s, i0, i1, i2);
        if (s.Compare(i3, i2) < 0)
        {
            s.Swap(i2, i3);
            if (s.Compare(i2, i1) < 0)
            {
                s.Swap(i1, i2);
                if (s.Compare(i1, i0) < 0) s.Swap(i0, i1);
            }
        }
    }

    /// <summary>
    /// Sorts exactly 5 elements using a sorting network (4-10 comparisons, 0-9 swaps).
    /// </summary>
    private static void Sort5<T>(SortSpan<T> s, int i0, int i1, int i2, int i3, int i4) where T : IComparable<T>
    {
        Sort4(s, i0, i1, i2, i3);
        if (s.Compare(i4, i3) < 0)
        {
            s.Swap(i3, i4);
            if (s.Compare(i3, i2) < 0)
            {
                s.Swap(i2, i3);
                if (s.Compare(i2, i1) < 0)
                {
                    s.Swap(i1, i2);
                    if (s.Compare(i1, i0) < 0) s.Swap(i0, i1);
                }
            }
        }
    }
}

/// <summary>
/// 教育目的の非最適化版挿入ソートです。シフト操作の代わりに隣接要素のスワップを繰り返す古典的な実装で、
/// アルゴリズムの基本原理を理解しやすい反面、最適化版に比べて約2倍の書き込み操作が発生します。
/// <br/>
/// Educational non-optimized insertion sort implementation using adjacent element swaps instead of shifts.
/// This classical approach makes the algorithm's basic principles easier to understand, but performs approximately
/// twice as many write operations compared to the optimized version.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Non-Optimized Insertion Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Maintain Sorted Invariant:</strong> For each position i (from 1 to n-1), 
/// the subarray [0..i-1] must remain sorted before processing element at position i.
/// This invariant is established initially (single element is sorted) and preserved through each iteration.</description></item>
/// <item><description><strong>Backward Bubble via Adjacent Swaps:</strong> For each element at position i,
/// repeatedly swap it with its left neighbor while it is smaller than that neighbor.
/// This "bubbles" the element backward to its correct position in the sorted region [0..i-1].
/// Continue until element is in correct position (no longer smaller than left neighbor).</description></item>
/// <item><description><strong>Stable Swap via Comparison:</strong> Use strict inequality (Compare(j-1, j) &gt; 0)
/// to determine if adjacent elements should be swapped. Equal elements are NOT swapped, preserving their original relative order.
/// This guarantees stability of the sort.</description></item>
/// <item><description><strong>No Optimization:</strong> Unlike optimized insertion sort, this version does NOT:
/// - Use shift operations (always uses swaps, even when element needs to move multiple positions)
/// - Skip writes for already-sorted elements (no optimization check)
/// This results in more write operations but simpler, more intuitive code.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Insertion</description></item>
/// <item><description>Stable      : Yes (strict inequality &gt; preserves order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space, no temporary variables needed for element storage)</description></item>
/// <item><description>Best case   : O(n) - Already sorted array, only n-1 comparisons, zero swaps</description></item>
/// <item><description>Average case: O(n²) - On average, each element swaps halfway back (n²/4 comparisons and swaps)</description></item>
/// <item><description>Worst case  : O(n²) - Reverse sorted array, maximum swaps: n(n-1)/2 comparisons and swaps</description></item>
/// <item><description>Comparisons : O(n²) - Same as optimized version: worst case n(n-1)/2</description></item>
/// <item><description>Swaps       : O(n²) - Each swap involves 2 reads + 2 writes = 4 operations total</description></item>
/// <item><description>Writes      : O(n²) - Approximately 2x optimized version (swaps use 2 writes vs shift's 1 write)</description></item>
/// </list>
/// <para><strong>Comparison with Optimized Insertion Sort:</strong></para>
/// <list type="bullet">
/// <item><description>Optimized Version: Uses shift operations - reads element, shifts larger elements right, writes once</description></item>
/// <item><description>Non-Optimized (This): Uses swaps - repeatedly exchanges adjacent elements</description></item>
/// <item><description>Write Operations: Non-optimized performs ~2x writes (each swap = 2 writes vs shift = 1 write)</description></item>
/// <item><description>Code Simplicity: Non-optimized is more intuitive - clearly shows element "bubbling" backward</description></item>
/// <item><description>Educational Value: Better for learning algorithm fundamentals before optimization techniques</description></item>
/// </list>
/// <para><strong>Educational Purpose:</strong></para>
/// <list type="bullet">
/// <item><description>Demonstrates classic insertion sort as taught in textbooks and courses</description></item>
/// <item><description>Shows clear connection between insertion sort and bubble sort (backward bubbling)</description></item>
/// <item><description>Makes the sorting process visually intuitive - element swaps backward step-by-step</description></item>
/// <item><description>Helps understand trade-offs between code simplicity and performance optimization</description></item>
/// <item><description>Useful for benchmarking impact of shift optimization (compare with InsertionSort class)</description></item>
/// </list>
/// <para><strong>Use Cases:</strong></para>
/// <list type="bullet">
/// <item><description>Educational purposes - teaching basic sorting algorithms</description></item>
/// <item><description>Algorithm visualization - easier to animate swap-by-swap movement</description></item>
/// <item><description>Performance comparison - baseline to measure optimization benefits</description></item>
/// <item><description>Code readability - when clarity is more important than performance</description></item>
/// </list>
/// </remarks>
/// <typeparam name="T"></typeparam>
public static class InsertionSortNonOptimized
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array

    /// <summary>
    /// Sorts the elements in the specified span in ascending order using the default comparer.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort in place.</param>
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
    {
        Sort(span, 0, span.Length, NullContext.Default);
    }

    /// <summary>
    /// Sorts the elements in the specified span using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context for tracking statistics and observations during sorting. Cannot be null.</param>
    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        Sort(span, 0, span.Length, context);
    }

    /// <summary>
    /// Sorts the subrange [first..last) using the provided sort context.
    /// Elements in [first..start) are assumed to already be sorted.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span containing elements to sort.</param>
    /// <param name="first">The inclusive start index of the range to sort.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    /// <param name="context">The sort context for tracking statistics and observations.</param>
    /// <remarks>
    /// This implementation uses adjacent element swaps instead of shift operations:
    /// - For each element at position i, compare with left neighbor
    /// - If smaller, swap with left neighbor and move pointer left
    /// - Repeat until element is in correct position (no longer smaller than left neighbor)
    /// This approach is intuitive but performs more write operations than the optimized shift-based version.
    /// Each swap requires 2 reads + 2 writes, while a shift requires 1 read + 1 write.
    /// </remarks>
    public static void Sort<T>(Span<T> span, int first, int last, ISortContext context) where T : IComparable<T>
    {
        ArgumentOutOfRangeException.ThrowIfNegative(first);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(last, span.Length);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(first, last);

        if (span.Length <= 1) return;

        var s = new SortSpan<T>(span, context, BUFFER_MAIN);
        SortCore(s, first, last);
    }

    /// <summary>
    /// Sorts the subrange [first..last) using the provided sort context.
    /// This overload accepts a SortSpan directly for use by other algorithms that already have a SortSpan instance.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="first">The inclusive start index of the range to sort.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    internal static void SortCore<T>(SortSpan<T> s, int first, int last) where T : IComparable<T>
    {
        for (var i = first + 1; i < last; i++)
        {
            // Move the element at position i backward until it's in the correct position
            // Use strict inequality (>) to maintain stability - equal elements are not swapped
            var j = i;
            while (j > first && s.Compare(j - 1, j) > 0)
            {
                s.Swap(j - 1, j);
                j--;
            }
        }
    }
}
