using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 最適化したRotate Merge Sortです。
/// 配列を再帰的に半分に分割し、それぞれをソートした後、回転アルゴリズムを使用してインプレースでマージする分割統治アルゴリズムです。
/// 安定ソートであり、追加メモリを使用せずにO(n log² n)の性能を保証します。
/// 小さい配列（≤16要素）ではInsertionSortを使用、ローテートにGCD-cycle、連続ブロック検索にGallopingを用いる実用的な最適化を含みます。
/// <br/>
/// Optimized Rotate Merge Sort.
/// Recursively divides the array in half, sorts each part, then merges sorted subarrays in-place using array rotation.
/// This divide-and-conquer algorithm is stable and guarantees O(n log² n) performance without requiring auxiliary space.
/// Includes practical optimizations: insertion sort for small subarrays (≤16 elements), GCD-cycle rotation, and galloping for finding consecutive blocks.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Rotate Merge Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Hybrid Optimization:</strong> For subarrays with ≤16 elements, insertion sort is used instead of rotation-based merge.
/// This is a practical optimization similar to TimSort and IntroSort, reducing overhead for small sizes.</description></item>
/// <item><description><strong>Galloping Optimization:</strong> Uses exponential search (1, 2, 4, 8, ...) followed by binary search to efficiently find
/// long runs of consecutive elements from the right partition. This is similar to TimSort's galloping mode and reduces comparisons
/// when merging data with long consecutive blocks.</description></item>
/// <item><description><strong>Divide Step (Binary Partitioning):</strong> The array must be divided into two roughly equal halves at each recursion level.
/// The midpoint is calculated as mid = (left + right) / 2, ensuring balanced subdivision.
/// This guarantees a recursion depth of ⌈log₂(n)⌉.</description></item>
/// <item><description><strong>Base Case (Termination Condition):</strong> Recursion must terminate when a subarray has size ≤ 1.
/// An array of size 0 or 1 is trivially sorted and requires no further processing.</description></item>
/// <item><description><strong>Conquer Step (Recursive Sorting):</strong> Each half must be sorted independently via recursive calls.
/// The left subarray [left..mid] and right subarray [mid+1..right] are sorted before merging.</description></item>
/// <item><description><strong>In-Place Merge Step:</strong> Two sorted subarrays must be merged without using additional memory.
/// This is achieved using array rotation, which rearranges elements by shifting blocks of the array.</description></item>
/// <item><description><strong>Rotation Algorithm (GCD-Cycle / Juggling):</strong> Array rotation is implemented using GCD-based cycle detection.
/// To rotate array A of length n by k positions: Find GCD(n, k) cycles, and for each cycle, move elements using assignments.
/// This achieves O(n) time rotation with O(1) space using only writes (no swaps needed).
/// The algorithm divides rotation into GCD(n,k) independent cycles, rotating elements within each cycle.</description></item>
/// <item><description><strong>Merge via Rotation:</strong> During merge, find the position where the first element of the right partition
/// should be inserted in the left partition (using binary search). Rotate elements to place it correctly, then recursively
/// merge the remaining elements. This maintains sorted order while being in-place.</description></item>
/// <item><description><strong>Stability Preservation:</strong> Binary search uses &lt;= comparison to find the insertion position,
/// ensuring equal elements from the left partition appear before equal elements from the right partition.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Hybrid (Merge + Insertion + Galloping)</description></item>
/// <item><description>Stable      : Yes (binary search with &lt;= comparison preserves relative order)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space, uses rotation instead of buffer)</description></item>
/// <item><description>Best case   : O(n) - Sorted data with insertion sort optimization for small partitions</description></item>
/// <item><description>Average case: O(n log² n) - Binary search (log n) + rotation (n) per merge level (log n levels)</description></item>
/// <item><description>Worst case  : O(n log² n) - Rotation adds O(n) factor to each merge operation</description></item>
/// <item><description>Comparisons : Best O(n), Average/Worst O(n log² n) - Galloping reduces comparisons for consecutive blocks</description></item>
/// <item><description>Writes      : Best O(n), Average/Worst O(n² log n) - GCD-cycle rotation uses assignments only (no swaps)</description></item>
/// <item><description>Swaps       : 0 - GCD-cycle rotation uses only write operations, no swaps needed</description></item>
/// <item><description>Space       : O(log n) - Only recursion stack space, no auxiliary buffer needed</description></item>
/// </list>
/// <para><strong>Advantages of Rotate Merge Sort:</strong></para>
/// <list type="bullet">
/// <item><description>True in-place sorting - O(1) auxiliary space (only recursion stack)</description></item>
/// <item><description>Stable - Preserves relative order of equal elements</description></item>
/// <item><description>Hybrid optimization - Insertion sort improves performance for small subarrays</description></item>
/// <item><description>Galloping search - Efficiently finds consecutive blocks (TimSort-style)</description></item>
/// <item><description>GCD-cycle rotation - Efficient assignment-based rotation without swaps</description></item>
/// </list>
/// <para><strong>Disadvantages:</strong></para>
/// <list type="bullet">
/// <item><description>Slower than buffer-based merge sort - Additional log n factor from binary search and rotation overhead</description></item>
/// <item><description>More writes than standard merge sort - Rotation requires multiple element movements</description></item>
/// <item><description>Complexity - Multiple optimizations (insertion sort, galloping, GCD-cycle) increase code complexity</description></item>
/// </list>
/// <para><strong>Use Cases:</strong></para>
/// <list type="bullet">
/// <item><description>When memory is extremely constrained (embedded systems, real-time systems)</description></item>
/// <item><description>When stability is required but auxiliary memory is not available</description></item>
/// <item><description>Educational purposes - Understanding in-place merging techniques</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Merge_sort#Variants</para>
/// <para>Rotation-based in-place merge: Practical In-Place Merging (Geffert et al.)</para>
/// </remarks>
public static class RotateMergeSort
{
    // Threshold for using insertion sort instead of rotation-based merge
    // Small subarrays benefit from insertion sort's lower overhead
    private const int InsertionSortThreshold = 16;

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array (in-place operations only)

    /// <summary>
    /// Sorts the elements in the specified span in ascending order using the default comparer.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort in place.</param>
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
        => Sort(span, new ComparableComparer<T>(), NullContext.Default);

    /// <summary>
    /// Sorts the elements in the specified span using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
        => Sort(span, new ComparableComparer<T>(), context);

    /// <summary>
    /// Sorts the elements in the specified span using the provided comparer and sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use for element comparisons.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="comparer">The comparer to use for element comparisons.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T, TComparer>(Span<T> span, TComparer comparer, ISortContext context) where TComparer : IComparer<T>
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T, TComparer>(span, context, comparer, BUFFER_MAIN);
        SortCore(s, 0, span.Length - 1);
    }

    /// <summary>
    /// Core recursive merge sort implementation.
    /// </summary>
    /// <param name="s">The SortSpan wrapping the span to sort</param>
    /// <param name="left">The inclusive start index of the range to sort</param>
    /// <param name="right">The inclusive end index of the range to sort</param>
    private static void SortCore<T, TComparer>(SortSpan<T, TComparer> s, int left, int right) where TComparer : IComparer<T>
    {
        if (right <= left) return; // Base case: array of size 0 or 1 is sorted

        var length = right - left + 1;

        // Optimization: Use insertion sort for small subarrays
        // Rotation overhead is too high for small sizes, and insertion sort has better cache locality
        if (length <= InsertionSortThreshold)
        {
            // Reuse existing InsertionSort.SortCore
            // Note: SortCore uses exclusive end index [first, last), so we pass right + 1
            InsertionSort.SortCore(s, left, right + 1);
            return;
        }

        var mid = left + (right - left) / 2;

        // Conquer: Recursively sort left and right halves
        SortCore(s, left, mid);
        SortCore(s, mid + 1, right);

        // Optimization: Skip merge if already sorted (left[last] <= right[first])
        if (s.Compare(mid, mid + 1) <= 0)
        {
            return; // Already sorted, no merge needed
        }

        // Merge: Combine two sorted halves in-place using rotation
        MergeInPlace(s, left, mid, right);
    }

    /// <summary>
    /// Merges two sorted subarrays [left..mid] and [mid+1..right] in-place using rotation.
    /// Uses binary search to find insertion points and rotation to rearrange elements.
    /// Optimization: Uses galloping (exponential search + binary search) to efficiently find
    /// long runs of consecutive elements from the right partition.
    /// </summary>
    /// <param name="s">The SortSpan wrapping the array</param>
    /// <param name="left">The inclusive start index of the left subarray</param>
    /// <param name="mid">The inclusive end index of the left subarray</param>
    /// <param name="right">The inclusive end index of the right subarray</param>
    private static void MergeInPlace<T, TComparer>(SortSpan<T, TComparer> s, int left, int mid, int right) where TComparer : IComparer<T>
    {
        var start1 = left;
        var start2 = mid + 1;

        // Main merge loop using rotation algorithm with galloping optimization
        while (start1 <= mid && start2 <= right)
        {
            // If element at start1 is in correct position
            if (s.Compare(start1, start2) <= 0)
            {
                start1++;
            }
            else
            {
                // Use binary search to find the position where start2 element should be inserted in [start1..mid]
                var value = s.Read(start2);
                var insertPos = BinarySearch(s, start1, mid, value);

                // Galloping optimization: Find the end of consecutive elements in right partition
                // that belong before insertPos using exponential search + binary search
                var start2End = GallopingSearchEnd(s, insertPos, start2, right);

                var blockSize = start2End - start2 + 1;
                var rotateDistance = start2 - insertPos;

                // Rotate the block [insertPos..start2End] to move all elements at once
                Rotate(s, insertPos, start2End, rotateDistance);

                // Update pointers after moving the block
                start1 = insertPos + blockSize;
                mid += blockSize;
                start2 = start2End + 1;
            }
        }
    }

    /// <summary>
    /// Finds the end position of consecutive elements from the right partition using galloping.
    /// Uses exponential search followed by binary search for efficiency.
    /// This is similar to TimSort's galloping mode.
    /// </summary>
    /// <param name="s">The SortSpan wrapping the array</param>
    /// <param name="insertPos">The position where elements should be inserted</param>
    /// <param name="start">The start position in the right partition</param>
    /// <param name="end">The end position in the right partition</param>
    /// <returns>The last index where elements should still be inserted before insertPos</returns>
    private static int GallopingSearchEnd<T, TComparer>(SortSpan<T, TComparer> s, int insertPos, int start, int end) where TComparer : IComparer<T>
    {
        // Phase 1: Exponential search (galloping) - find rough upper bound
        // Step size: 1, 2, 4, 8, 16, ... (exponentially increasing)
        var lastGood = start;
        var step = 1;

        while (start + step <= end && s.Compare(insertPos, start + step) > 0)
        {
            lastGood = start + step;
            step *= 2;  // Exponential growth
        }

        // Phase 2: Binary search for exact boundary in [lastGood..min(start+step, end)]
        var low = lastGood;
        var high = Math.Min(start + step, end);

        // Binary search to find the last element that should be before insertPos
        while (low < high)
        {
            var mid = low + (high - low + 1) / 2;

            if (s.Compare(insertPos, mid) > 0)
            {
                low = mid;
            }
            else
            {
                high = mid - 1;
            }
        }

        return low;
    }

    /// <summary>
    /// Rotates a subarray by k positions to the left using the GCD-cycle (Juggling) algorithm.
    /// This algorithm divides the rotation into GCD(n, k) independent cycles and moves elements
    /// within each cycle using assignments only (no swaps needed).
    /// </summary>
    /// <param name="s">The SortSpan wrapping the array</param>
    /// <param name="left">The start index of the subarray to rotate</param>
    /// <param name="right">The end index of the subarray to rotate</param>
    /// <param name="k">The number of positions to rotate left</param>
    private static void Rotate<T, TComparer>(SortSpan<T, TComparer> s, int left, int right, int k) where TComparer : IComparer<T>
    {
        if (k == 0 || left >= right) return;

        var n = right - left + 1;
        k = k % n;
        if (k == 0) return;

        // GCD-cycle rotation (Juggling algorithm)
        // Divide rotation into gcd(n, k) independent cycles
        var cycles = GCD(n, k);

        for (var cycle = 0; cycle < cycles; cycle++)
        {
            // Save the first element of this cycle
            var startIdx = left + cycle;
            var temp = s.Read(startIdx);
            var currentIdx = startIdx;

            // Move elements in this cycle
            while (true)
            {
                var nextIdx = currentIdx + k;
                if (nextIdx > right)
                    nextIdx = left + (nextIdx - right - 1);

                // If we've completed the cycle, break
                if (nextIdx == startIdx)
                    break;

                // Move element from nextIdx to currentIdx
                s.Write(currentIdx, s.Read(nextIdx));
                currentIdx = nextIdx;
            }

            // Place the saved element in its final position
            s.Write(currentIdx, temp);
        }
    }

    /// <summary>
    /// Calculates the greatest common divisor (GCD) of two numbers using Euclid's algorithm.
    /// </summary>
    /// <param name="a">First number</param>
    /// <param name="b">Second number</param>
    /// <returns>GCD of a and b</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GCD(int a, int b)
    {
        while (b != 0)
        {
            var temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    /// <summary>
    /// Performs binary search to find the insertion position for a value in a sorted range.
    /// Uses &lt;= comparison to maintain stability (insert after equal elements).
    /// </summary>
    /// <param name="s">The SortSpan wrapping the array</param>
    /// <param name="left">The start index of the search range</param>
    /// <param name="right">The end index of the search range</param>
    /// <param name="value">The value to search for</param>
    /// <returns>The index where the value should be inserted</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int BinarySearch<T, TComparer>(SortSpan<T, TComparer> s, int left, int right, T value) where TComparer : IComparer<T>
    {
        while (left <= right)
        {
            var mid = left + (right - left) / 2;
            var cmp = s.Compare(s.Read(mid), value);

            if (cmp <= 0)
            {
                left = mid + 1;
            }
            else
            {
                right = mid - 1;
            }
        }

        return left;
    }
}


/// <summary>
/// 最適化していないRotate Merge Sortです。
/// 配列を再帰的に半分に分割し、それぞれをソートした後、回転アルゴリズムを使用してインプレースでマージする分割統治アルゴリズムです。
/// 安定ソートであり、追加メモリを使用せずにO(n log n)の性能を保証します。
/// 回転をするため、要素の移動が多くなるため、標準のマージソートよりも遅くなります。
/// <br/>
/// Non-Optimized Rotate Merge Sort.
/// Recursively divides the array in half, sorts each part, then merges sorted subarrays in-place using array rotation.
/// This divide-and-conquer algorithm is stable and guarantees O(n log n) performance without requiring auxiliary space.
/// However, due to the rotations, it involves more element movements and is slower than standard merge sort.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Rotate Merge Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Divide Step (Binary Partitioning):</strong> The array must be divided into two roughly equal halves at each recursion level.
/// The midpoint is calculated as mid = (left + right) / 2, ensuring balanced subdivision.
/// This guarantees a recursion depth of ⌈log₂(n)⌉.</description></item>
/// <item><description><strong>Base Case (Termination Condition):</strong> Recursion must terminate when a subarray has size ≤ 1.
/// An array of size 0 or 1 is trivially sorted and requires no further processing.</description></item>
/// <item><description><strong>Conquer Step (Recursive Sorting):</strong> Each half must be sorted independently via recursive calls.
/// The left subarray [left..mid] and right subarray [mid+1..right] are sorted before merging.</description></item>
/// <item><description><strong>In-Place Merge Step:</strong> Two sorted subarrays must be merged without using additional memory.
/// This is achieved using array rotation, which rearranges elements by shifting blocks of the array.</description></item>
/// <item><description><strong>Rotation Algorithm (Block Reversal):</strong> Array rotation is implemented using three reversals:
/// To rotate array A of length n by k positions: Reverse(A[0..k-1]), Reverse(A[k..n-1]), Reverse(A[0..n-1]).
/// This achieves O(n) time rotation with O(1) space and preserves element stability.</description></item>
/// <item><description><strong>Merge via Rotation:</strong> During merge, find the position where the first element of the right partition
/// should be inserted in the left partition (using binary search). Rotate elements to place it correctly, then recursively
/// merge the remaining elements. This maintains sorted order while being in-place.</description></item>
/// <item><description><strong>Stability Preservation:</strong> Binary search uses &lt;= comparison to find the insertion position,
/// ensuring equal elements from the left partition appear before equal elements from the right partition.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Merge (In-Place variant)</description></item>
/// <item><description>Stable      : Yes (binary search with &lt;= comparison preserves relative order)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space, uses rotation instead of buffer)</description></item>
/// <item><description>Best case   : O(n log n) - Even sorted data requires ⌈log₂(n)⌉ levels of merging</description></item>
/// <item><description>Average case: O(n log² n) - Binary search (log n) + rotation (n) per merge level (log n levels)</description></item>
/// <item><description>Worst case  : O(n log² n) - Rotation adds O(n) factor to each merge operation</description></item>
/// <item><description>Comparisons : O(n log² n) - Binary search adds log n comparisons per merge</description></item>
/// <item><description>Writes      : O(n² log n) - Rotation requires multiple element movements (n writes per level)</description></item>
/// <item><description>Space       : O(log n) - Only recursion stack space, no auxiliary buffer needed</description></item>
/// </list>
/// <para><strong>Advantages of Rotate Merge Sort:</strong></para>
/// <list type="bullet">
/// <item><description>True in-place sorting - O(1) auxiliary space (only recursion stack)</description></item>
/// <item><description>Stable - Preserves relative order of equal elements</description></item>
/// <item><description>Predictable performance - O(n log² n) guaranteed in all cases</description></item>
/// <item><description>Cache-friendly - Better locality than standard merge sort with buffer</description></item>
/// </list>
/// <para><strong>Disadvantages:</strong></para>
/// <list type="bullet">
/// <item><description>Slower than buffer-based merge sort - Additional log n factor from binary search and rotation overhead</description></item>
/// <item><description>More writes than standard merge sort - Rotation requires multiple element movements</description></item>
/// <item><description>Not adaptive - Doesn't exploit existing order in data</description></item>
/// </list>
/// <para><strong>Use Cases:</strong></para>
/// <list type="bullet">
/// <item><description>When memory is extremely constrained (embedded systems, real-time systems)</description></item>
/// <item><description>When stability is required but auxiliary memory is not available</description></item>
/// <item><description>Educational purposes - Understanding in-place merging techniques</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Merge_sort#Variants</para>
/// <para>Rotation-based in-place merge: Practical In-Place Merging (Geffert et al.)</para>
/// </remarks>
public static class RotateMergeSortNonOptimized
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array (in-place operations only)

    /// <summary>
    /// Sorts the elements in the specified span in ascending order using the default comparer.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort in place.</param>
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
        => Sort(span, new ComparableComparer<T>(), NullContext.Default);

    /// <summary>
    /// Sorts the elements in the specified span using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
        => Sort(span, new ComparableComparer<T>(), context);

    /// <summary>
    /// Sorts the elements in the specified span using the provided comparer and sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use for element comparisons.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="comparer">The comparer to use for element comparisons.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T, TComparer>(Span<T> span, TComparer comparer, ISortContext context) where TComparer : IComparer<T>
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T, TComparer>(span, context, comparer, BUFFER_MAIN);
        SortCore(s, 0, span.Length - 1);
    }

    /// <summary>
    /// Core recursive merge sort implementation.
    /// </summary>
    /// <param name="s">The SortSpan wrapping the span to sort</param>
    /// <param name="left">The inclusive start index of the range to sort</param>
    /// <param name="right">The inclusive end index of the range to sort</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SortCore<T, TComparer>(SortSpan<T, TComparer> s, int left, int right) where TComparer : IComparer<T>
    {
        if (right <= left) return; // Base case: array of size 0 or 1 is sorted

        var mid = left + (right - left) / 2;

        // Conquer: Recursively sort left and right halves
        SortCore(s, left, mid);
        SortCore(s, mid + 1, right);

        // Optimization: Skip merge if already sorted (left[last] <= right[first])
        if (s.Compare(mid, mid + 1) <= 0)
        {
            return; // Already sorted, no merge needed
        }

        // Merge: Combine two sorted halves in-place using rotation
        MergeInPlace(s, left, mid, right);
    }

    /// <summary>
    /// Merges two sorted subarrays [left..mid] and [mid+1..right] in-place using rotation.
    /// Uses binary search to find insertion points and rotation to rearrange elements.
    /// Optimization: Processes multiple consecutive elements from the right partition at once.
    /// </summary>
    /// <param name="s">The SortSpan wrapping the array</param>
    /// <param name="left">The inclusive start index of the left subarray</param>
    /// <param name="mid">The inclusive end index of the left subarray</param>
    /// <param name="right">The inclusive end index of the right subarray</param>
    private static void MergeInPlace<T, TComparer>(SortSpan<T, TComparer> s, int left, int mid, int right) where TComparer : IComparer<T>
    {
        var start1 = left;
        var start2 = mid + 1;

        // Main merge loop using rotation algorithm
        while (start1 <= mid && start2 <= right)
        {
            // If element at start1 is in correct position
            if (s.Compare(start1, start2) <= 0)
            {
                start1++;
            }
            else
            {
                // Optimization: Find how many consecutive elements from right partition
                // can be moved to the current position in left partition
                var value = s.Read(start2);
                var insertPos = BinarySearch(s, start1, mid, value);

                // Find the end of consecutive elements in right partition that belong here
                var start2End = start2;
                while (start2End < right && s.Compare(insertPos, start2End + 1) > 0)
                {
                    start2End++;
                }

                var blockSize = start2End - start2 + 1;
                var rotateDistance = start2 - insertPos;

                // Rotate the block [insertPos..start2End] to move all elements at once
                Rotate(s, insertPos, start2End, rotateDistance);

                // Update pointers after moving the block
                start1 = insertPos + blockSize;
                mid += blockSize;
                start2 = start2End + 1;
            }
        }
    }

    /// <summary>
    /// Rotates a subarray by k positions to the left using the reversal algorithm.
    /// Rotation is achieved by three reversals: Reverse[0..k-1], Reverse[k..n-1], Reverse[0..n-1].
    /// </summary>
    /// <param name="s">The SortSpan wrapping the array</param>
    /// <param name="left">The start index of the subarray to rotate</param>
    /// <param name="right">The end index of the subarray to rotate</param>
    /// <param name="k">The number of positions to rotate left</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Rotate<T, TComparer>(SortSpan<T, TComparer> s, int left, int right, int k) where TComparer : IComparer<T>
    {
        if (k == 0 || left >= right) return;

        // Normalize k to be within range
        var n = right - left + 1;
        k = k % n;
        if (k == 0) return;

        // Three-reversal rotation algorithm
        Reverse(s, left, left + k - 1);
        Reverse(s, left + k, right);
        Reverse(s, left, right);
    }

    /// <summary>
    /// Reverses a subarray in-place.
    /// </summary>
    /// <param name="s">The SortSpan wrapping the array</param>
    /// <param name="left">The start index of the subarray to reverse</param>
    /// <param name="right">The end index of the subarray to reverse</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Reverse<T, TComparer>(SortSpan<T, TComparer> s, int left, int right) where TComparer : IComparer<T>
    {
        while (left < right)
        {
            s.Swap(left, right);
            left++;
            right--;
        }
    }

    /// <summary>
    /// Performs binary search to find the insertion position for a value in a sorted range.
    /// Uses &lt;= comparison to maintain stability (insert after equal elements).
    /// </summary>
    /// <param name="s">The SortSpan wrapping the array</param>
    /// <param name="left">The start index of the search range</param>
    /// <param name="right">The end index of the search range</param>
    /// <param name="value">The value to search for</param>
    /// <returns>The index where the value should be inserted</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int BinarySearch<T, TComparer>(SortSpan<T, TComparer> s, int left, int right, T value) where TComparer : IComparer<T>
    {
        while (left <= right)
        {
            var mid = left + (right - left) / 2;
            var cmp = s.Compare(s.Read(mid), value);

            if (cmp <= 0)
            {
                left = mid + 1;
            }
            else
            {
                right = mid - 1;
            }
        }

        return left;
    }
}
