using SortAlgorithm.Contexts;
using System.Numerics;

namespace SortAlgorithm.Algorithms;


/// <summary>
/// LLVM libc++ std::sort implementation in C#.
/// Implements Introsort algorithm combining quicksort, heapsort, and insertion sort
/// with advanced optimizations including Tuckey's ninther, sorting networks, and
/// detection of already-partitioned data.
/// </summary>
/// <remarks>
/// <para><strong>Algorithm Overview:</strong></para>
/// <list type="bullet">
/// <item><description>Introsort: QuickSort with HeapSort fallback at depth limit</description></item>
/// <item><description>Sorting Networks: Optimized 2-5 element sorts</description></item>
/// <item><description>Insertion Sort: For small subarrays (< 24 elements)</description></item>
/// <item><description>Tuckey's Ninther: Advanced pivot selection for large arrays (>= 128)</description></item>
/// <item><description>Partition Optimizations: Equal element handling, already-partitioned detection</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Best case:    O(n) - Already sorted with partition detection</description></item>
/// <item><description>Average case:  O(n log n) - Typical random data</description></item>
/// <item><description>Worst case:    O(n log n) - Guaranteed by HeapSort fallback</description></item>
/// <item><description>Space:         O(log n) - Recursion stack depth</description></item>
/// <item><description>Stable:        No</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>LLVM libc++: https://github.com/llvm/llvm-project/blob/llvmorg-21.1.8/libcxx/include/__algorithm/sort.h</para>
/// <para>Danila Kutenin: Changing std::sort at Google’s Scale and Beyond https://danlark.org/2022/04/20/changing-stdsort-at-googles-scale-and-beyond/comment-page-1/</para>
/// </remarks>
public static class StdSort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    
    // Upper bound for using insertion sort for sorting
    private const int INSERTION_SORT_LIMIT = 24;
    // Lower bound for using Tuckey's ninther technique for median computation
    private const int NINTHER_THRESHOLD = 128;

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
    /// <param name="context">The sort context that tracks statistics and provides sorting operations. Cannot be null.</param>
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
    public static void Sort<T>(Span<T> span, int first, int last, ISortContext context) where T : IComparable<T>
    {
        ArgumentOutOfRangeException.ThrowIfNegative(first);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(last, span.Length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(first, last);

        if (last - first <= 1) return;

        var s = new SortSpan<T>(span, context, BUFFER_MAIN);
        SortCore(s, first, last, context);
    }

    /// <summary>
    /// Sorts the subrange [first..last) using the provided sort context.
    /// This overload accepts a SortSpan directly for use by other algorithms that already have a SortSpan instance.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="first">The inclusive start index of the range to sort.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    /// <param name="context">The sort context for tracking statistics and observations.</param>
    internal static void SortCore<T>(SortSpan<T> s, int first, int last, ISortContext context) where T : IComparable<T>
    {
        if (last - first <= 1) return;

        // Calculate depth limit: 2 * log2(n)
        var len = last - first;
        var depthLimit = 2 * Log2((uint)len);
        
        IntroSort(s, first, last, depthLimit, context, leftmost: true);
    }

    /// <summary>
    /// Computes log2 of an unsigned integer (bit width)
    /// </summary>
    private static int Log2(uint n)
    {
        // var log = 0;
        // while (n > 1)
        // {
        //     n >>= 1;
        //     log++;
        // }
        // return log;
        return BitOperations.Log2((uint)n);
    }

    /// <summary>
    /// Sorts 3 elements. Stable, 2-3 compares, 0-2 swaps.
    /// </summary>
    private static void Sort3<T>(SortSpan<T> s, int x, int y, int z) where T : IComparable<T>
    {
        // if x <= y
        if (s.Compare(y, x) >= 0)
        {
            // if y <= z: x <= y <= z (already sorted)
            if (s.Compare(z, y) >= 0)
                return;
            
            // x <= y && y > z
            s.Swap(y, z);   // x <= z && y < z
            if (s.Compare(y, x) < 0)  // if x > y
                s.Swap(x, y); // x < y && y <= z
            return;
        }
        
        // x > y
        if (s.Compare(z, y) < 0) // if y > z
        {
            s.Swap(x, z); // x < y && y < z
            return;
        }
        
        s.Swap(x, y); // x > y && y <= z -> x < y && x <= z
        if (s.Compare(z, y) < 0)  // if y > z
            s.Swap(y, z); // x <= y && y < z
    }

    /// <summary>
    /// Sorts 4 elements. Stable, 3-6 compares, 0-5 swaps.
    /// </summary>
    private static void Sort4<T>(SortSpan<T> s, int x1, int x2, int x3, int x4) where T : IComparable<T>
    {
        Sort3(s, x1, x2, x3);
        if (s.Compare(x4, x3) < 0)
        {
            s.Swap(x3, x4);
            if (s.Compare(x3, x2) < 0)
            {
                s.Swap(x2, x3);
                if (s.Compare(x2, x1) < 0)
                {
                    s.Swap(x1, x2);
                }
            }
        }
    }

    /// <summary>
    /// Sorts 5 elements. Stable, 4-10 compares, 0-9 swaps.
    /// </summary>
    private static void Sort5<T>(SortSpan<T> s, int x1, int x2, int x3, int x4, int x5) where T : IComparable<T>
    {
        Sort4(s, x1, x2, x3, x4);
        if (s.Compare(x5, x4) < 0)
        {
            s.Swap(x4, x5);
            if (s.Compare(x4, x3) < 0)
            {
                s.Swap(x3, x4);
                if (s.Compare(x3, x2) < 0)
                {
                    s.Swap(x2, x3);
                    if (s.Compare(x2, x1) < 0)
                    {
                        s.Swap(x1, x2);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Main Introsort algorithm combining quicksort, heapsort, and insertion sort.
    /// </summary>
    private static void IntroSort<T>(SortSpan<T> s, int first, int last, int depth, ISortContext context, bool leftmost) where T : IComparable<T>
    {
        while (true)
        {
            var len = last - first;
            
            // Handle small arrays with specialized sorting networks
            switch (len)
            {
                case 0:
                case 1:
                    return;
                case 2:
                    if (s.Compare(last - 1, first) < 0)
                    {
                        s.Swap(first, last - 1);
                    }
                    return;
                case 3:
                    Sort3(s, first, first + 1, last - 1);
                    return;
                case 4:
                    Sort4(s, first, first + 1, first + 2, last - 1);
                    return;
                case 5:
                    Sort5(s, first, first + 1, first + 2, first + 3, last - 1);
                    return;
            }

            // Use insertion sort for small arrays
            if (len < INSERTION_SORT_LIMIT)
            {
                if (leftmost)
                {
                    InsertionSort.SortCore(s, first, last);
                }
                else
                {
                    InsertionSortUnguarded(s, first, last);
                }
                return;
            }

            // Fallback to heapsort if recursion depth is too deep
            if (depth == 0)
            {
                HeapSort.SortCore(s, first, last);
                return;
            }
            depth--;

            // Pivot selection: Tuckey's ninther for large arrays, median-of-3 otherwise
            var halfLen = len / 2;
            if (len > NINTHER_THRESHOLD)
            {
                // Ninther: median of medians
                Sort3(s, first, first + halfLen, last - 1);
                Sort3(s, first + 1, first + (halfLen - 1), last - 2);
                Sort3(s, first + 2, first + (halfLen + 1), last - 3);
                Sort3(s, first + (halfLen - 1), first + halfLen, first + (halfLen + 1));
                s.Swap(first, first + halfLen);
            }
            else
            {
                // Median-of-3
                Sort3(s, first + halfLen, first, last - 1);
            }

            // Partition optimization: skip equal elements on left if not leftmost
            if (!leftmost && s.Compare(first - 1, first) >= 0)
            {
                first = PartitionWithEqualsOnLeft(s, first, last);
                continue;
            }

            // Partition
            var (pivotPos, alreadyPartitioned) = PartitionWithEqualsOnRight(s, first, last);

            // Check if already sorted using insertion sort heuristic
            if (alreadyPartitioned)
            {
                var leftSorted = InsertionSortIncomplete(s, first, pivotPos);
                var rightSorted = InsertionSortIncomplete(s, pivotPos + 1, last);
                
                if (leftSorted && rightSorted)
                    return;
                if (leftSorted)
                {
                    first = pivotPos + 1;
                    continue;
                }
                if (rightSorted)
                {
                    last = pivotPos;
                    continue;
                }
            }

            // Recursively sort left partition, loop on right (tail recursion elimination)
            IntroSort(s, first, pivotPos, depth, context, leftmost);
            leftmost = false;
            first = pivotPos + 1;
        }
    }

    /// <summary>
    /// Insertion sort without bounds checking (unguarded).
    /// Assumes there is an element at position (first - 1) smaller than all elements in range.
    /// </summary>
    private static void InsertionSortUnguarded<T>(SortSpan<T> s, int first, int last) where T : IComparable<T>
    {
        if (first == last) return;

        for (var i = first + 1; i < last; i++)
        {
            var j = i - 1;
            if (s.Compare(i, j) < 0)
            {
                var tmp = s.Read(i);
                var k = j;
                j = i;
                do
                {
                    s.Write(j, s.Read(k));
                    j = k;
                } while (s.Compare(tmp, --k) < 0);
                s.Write(j, tmp);
            }
        }
    }

    /// <summary>
    /// Attempts insertion sort and returns true if array is already sorted or nearly sorted.
    /// Returns false if too many inversions are found (limit of 8 inversions).
    /// </summary>
    private static bool InsertionSortIncomplete<T>(SortSpan<T> s, int first, int last) where T : IComparable<T>
    {
        var len = last - first;
        switch (len)
        {
            case 0:
            case 1:
                return true;
            case 2:
                if (s.Compare(last - 1, first) < 0)
                {
                    s.Swap(first, last - 1);
                }
                return true;
            case 3:
                Sort3(s, first, first + 1, last - 1);
                return true;
            case 4:
                Sort4(s, first, first + 1, first + 2, last - 1);
                return true;
            case 5:
                Sort5(s, first, first + 1, first + 2, first + 3, last - 1);
                return true;
        }

        // Try insertion sort with inversion limit
        var j = first + 2;
        Sort3(s, first, first + 1, j);
        
        const int limit = 8;
        var count = 0;
        
        for (var i = j + 1; i < last; i++)
        {
            if (s.Compare(i, j) < 0)
            {
                var tmp = s.Read(i);
                var k = j;
                j = i;
                do
                {
                    s.Write(j, s.Read(k));
                    j = k;
                } while (j != first && s.Compare(tmp, --k) < 0);
                s.Write(j, tmp);

                if (++count == limit)
                {
                    return i + 1 == last;
                }
            }
            j = i;
        }
        return true;
    }

    /// <summary>
    /// Partitions range with equal elements kept to the right of pivot.
    /// Returns (pivot position, already partitioned flag).
    /// </summary>
    private static (int pivotPos, bool alreadyPartitioned) PartitionWithEqualsOnRight<T>(SortSpan<T> s, int first, int last) where T : IComparable<T>
    {
        var pivot = s.Read(first);
        
        // Find first element >= pivot
        var i = first;
        do
        {
            i++;
        } while (i < last && s.Compare(i, pivot) < 0);

        // Find last element < pivot
        var j = last - 1;
        if (i < j)
        {
            while (j > first && s.Compare(j, pivot) >= 0)
            {
                j--;
            }
        }

        var alreadyPartitioned = i >= j;

        // Partition loop
        while (i < j)
        {
            s.Swap(i, j);
            
            do { i++; } while (s.Compare(i, pivot) < 0);
            do { j--; } while (s.Compare(j, pivot) >= 0);
        }

        // Place pivot in correct position
        var pivotPos = i - 1;
        if (first != pivotPos)
        {
            s.Write(first, s.Read(pivotPos));
        }
        s.Write(pivotPos, pivot);

        return (pivotPos, alreadyPartitioned);
    }

    /// <summary>
    /// Partitions range with equal elements kept to the left of pivot.
    /// Returns the first index of the right partition.
    /// </summary>
    private static int PartitionWithEqualsOnLeft<T>(SortSpan<T> s, int first, int last) where T : IComparable<T>
    {
        var pivot = s.Read(first);
        
        // Find first element > pivot
        var i = first;
        do
        {
            i++;
        } while (i < last && s.Compare(pivot, i) >= 0);

        // Find last element <= pivot
        var j = last - 1;
        if (i < j)
        {
            while (j > first && s.Compare(pivot, j) < 0)
            {
                j--;
            }
        }

        // Partition loop
        while (i < j)
        {
            s.Swap(i, j);
            
            do { i++; } while (s.Compare(pivot, i) >= 0);
            do { j--; } while (s.Compare(pivot, j) < 0);
        }

        // Place pivot in correct position
        var pivotPos = i - 1;
        if (first != pivotPos)
        {
            s.Write(first, s.Read(pivotPos));
        }
        s.Write(pivotPos, pivot);

        return i;
    }
}
