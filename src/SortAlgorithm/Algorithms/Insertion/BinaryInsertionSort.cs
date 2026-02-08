using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/*
Ref span ...

| Method           | Number | Mean       | Error       | StdDev    | Median      | Min         | Max        | Allocated |
|----------------- |------- |-----------:|------------:|----------:|------------:|------------:|-----------:|----------:|
| BinaryInsertSort | 100    |   112.1 us | 2,820.79 us | 154.62 us |    31.50 us |    14.50 us |   290.4 us |     736 B |
| BinaryInsertSort | 1000   |   112.4 us |    55.38 us |   3.04 us |   112.90 us |   109.10 us |   115.1 us |     736 B |
| BinaryInsertSort | 10000  | 6,902.2 us |   897.73 us |  49.21 us | 6,929.30 us | 6,845.40 us | 6,931.9 us |     736 B |

Ref span (start) ...

| Method           | Number | Mean       | Error       | StdDev    | Median      | Min         | Max        | Allocated |
|----------------- |------- |-----------:|------------:|----------:|------------:|------------:|-----------:|----------:|
| BinaryInsertSort | 100    |   122.6 us | 3,383.21 us | 185.45 us |    15.70 us |    15.30 us |   336.7 us |     448 B |
| BinaryInsertSort | 1000   |   121.1 us |    55.55 us |   3.04 us |   121.70 us |   117.80 us |   123.8 us |     448 B |
| BinaryInsertSort | 10000  | 8,673.2 us | 4,841.61 us | 265.38 us | 8,720.50 us | 8,387.30 us | 8,911.7 us |     736 B |

Span ...

| Method                 | Number | Mean          | Error        | StdDev      | Median        | Min           | Max          | Allocated |
|----------------------- |------- |--------------:|-------------:|------------:|--------------:|--------------:|-------------:|----------:|
| BinaryInsertSort       | 100    |    135.333 us | 3,753.441 us | 205.7388 us |     16.600 us |     16.500 us |    372.90 us |     736 B |
| BinaryInsertSort       | 1000   |    122.133 us |    39.312 us |   2.1548 us |    122.300 us |    119.900 us |    124.20 us |     736 B |
| BinaryInsertSort       | 10000  |  6,903.433 us |   742.971 us |  40.7247 us |  6,898.500 us |  6,865.400 us |  6,946.40 us |     736 B |

*/

/// <summary>
/// 挿入位置を二分探索で決定することで、通常の挿入ソートの比較回数を O(n log n) に削減した安定ソートアルゴリズムです。
/// ただし、要素のシフト操作は依然として O(n^2) であるため、全体の時間計算量は O(n^2) となります。
/// <br/>
/// Uses binary search to determine the insertion position, reducing comparison count to O(n log n) compared to traditional insertion sort.
/// However, element shifting remains O(n^2), so overall time complexity is still O(n^2). Stable sort algorithm.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Binary Insertion Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Binary Search for Insertion Position:</strong> For each element at position i (from 1 to n-1),
/// perform binary search in the sorted range [0..i) to find the correct insertion position.
/// The search must maintain stability by placing equal elements after existing ones (using &lt;= comparison).</description></item>
/// <item><description><strong>Stable Insertion:</strong> When inserting element at position pos, shift all elements in range [pos..i-1]
/// one position to the right, then place the element at position pos. This preserves the relative order of equal elements.</description></item>
/// <item><description><strong>Optimization for Sorted Elements:</strong> If binary search returns pos == i, the element is already
/// in the correct position, so no shifts or writes are necessary. This optimization achieves O(n) best case for sorted arrays.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Insertion</description></item>
/// <item><description>Stable      : Yes (binary search uses &lt;= to preserve order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space, only temp variable for element being inserted)</description></item>
/// <item><description>Best case   : O(n) - Already sorted array, no shifts needed (only n-1 reads + comparisons)</description></item>
/// <item><description>Average case: O(n²) - Comparisons: O(n log n), Shifts: O(n²) dominate overall complexity</description></item>
/// <item><description>Worst case  : O(n²) - Reverse sorted array, maximum shifts for each element</description></item>
/// <item><description>Comparisons : O(n log n) - Binary search performs Σ(i=1 to n-1) ceiling(log₂(i+1)) comparisons ≈ n log n</description></item>
/// <item><description>Writes      : O(n²) - Each element may require shifting up to i elements (Σ(i=1 to n-1) i = n(n-1)/2)</description></item>
/// <item><description>Reads       : O(n²) - Binary search reads: O(n log n), Shift reads: O(n²)</description></item>
/// </list>
/// <para><strong>Comparison with Standard Insertion Sort:</strong></para>
/// <list type="bullet">
/// <item><description>Insertion Sort: O(n²) comparisons, O(n²) writes - Linear search for insertion position</description></item>
/// <item><description>Binary Insertion Sort: O(n log n) comparisons, O(n²) writes - Binary search reduces comparisons but not writes</description></item>
/// <item><description>Practical benefit: More significant when comparisons are expensive (e.g., string comparison, complex objects)</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Insertion_sort</para>
/// </remarks>
public static class BinaryInsertionSort
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
    public static void Sort<T>(Span<T> span, int first, int last, ISortContext context) where T : IComparable<T>
    {
        ArgumentOutOfRangeException.ThrowIfNegative(first);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(last, span.Length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(first, last);

        if (last - first <= 1) return;

        var s = new SortSpan<T>(span, context, BUFFER_MAIN);
        SortCore(s, first, last, first);
    }

    /// <summary>
    /// Sorts the subrange [first..last) using the provided sort context.
    /// Elements in [first..start) are assumed to already be sorted.
    /// This is useful for hybrid algorithms that pre-sort a portion of the array.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="first">The inclusive start index of the sorted range.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    /// <param name="start">The position from which to start inserting elements. Elements before this are already sorted.</param>
    internal static void SortCore<T>(SortSpan<T> s, int first, int last, int start) where T : IComparable<T>
    {
        // If 'start' equals 'first', move it forward to begin insertion from the next element
        if (start == first)
            start++;

        for (var i = start; i < last; i++)
        {
            var tmp = s.Read(i);

            // Find the insertion position using binary search in the sorted range [first..i)
            var pos = BinarySearch(s, tmp, first, i);

            // Only perform shift and write if the element needs to move
            // Optimization: if pos == i, element is already in correct position
            if (pos != i)
            {
                // Shift elements [pos..i-1] one position to the right to make room
                for (var j = i - 1; j >= pos; j--)
                {
                    s.Write(j + 1, s.Read(j));
                }

                // Insert the element at its correct position
                s.Write(pos, tmp);
            }
        }
    }

    /// <summary>
    /// Performs a stable binary search in the sorted range [first..index) to find the insertion point for the given value.
    /// Returns the position where the value should be inserted to maintain sort order and stability.
    /// </summary>
    /// <typeparam name="T">The type of elements being compared. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="s">The SortSpan containing the elements to search.</param>
    /// <param name="tmp">The value to find the insertion position for.</param>
    /// <param name="first">The inclusive start index of the sorted range to search.</param>
    /// <param name="index">The exclusive end index of the sorted range to search.</param>
    /// <returns>The index where the value should be inserted. If equal elements exist, returns position after them (stable).</returns>
    /// <remarks>
    /// Uses the condition `s.Compare(mid, tmp) &lt;= 0` to ensure stability:
    /// When the element at mid equals tmp, we continue searching to the right (left = mid + 1),
    /// ensuring that tmp is inserted after all equal elements, preserving their original order.
    /// </remarks>
    private static int BinarySearch<T>(SortSpan<T> s, T tmp, int first, int index) where T : IComparable<T>
    {
        var left = first;
        var right = index;
        while (left < right)
        {
            var mid = (left + right) / 2;
            if (s.Compare(mid, tmp) <= 0)
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
}
