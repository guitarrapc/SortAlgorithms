using SortAlgorithm.Contexts;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 値の分布状況を数え上げることを利用してインデックスを導きソートします。
/// 各要素からキーを抽出し、その出現回数をカウントして累積和を計算し、正しい位置に配置する安定なソートアルゴリズムです。
/// キーの範囲が狭い場合に非常に高速ですが、範囲が広いとメモリを大量に消費します。
/// <br/>
/// Sorts elements by counting the distribution of extracted keys.
/// A stable sorting algorithm that extracts keys, counts occurrences, and uses cumulative sums to place elements.
/// Very fast when the key range is narrow, but consumes significant memory for wide ranges.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Counting Sort (Generic, Key-based):</strong></para>
/// <list type="number">
/// <item><description><strong>Key Extraction:</strong> Each element must have a deterministic integer key obtained via the key selector function.
/// The key must be stable (same element always produces the same key).</description></item>
/// <item><description><strong>Range Determination:</strong> The algorithm finds min and max keys to determine the range [min, max].
/// A count array of size (max - min + 1) is allocated to track occurrences.</description></item>
/// <item><description><strong>Offset Normalization:</strong> Keys are normalized using offset = -min, mapping keys to array indices [0, range-1].
/// This allows handling negative keys correctly.</description></item>
/// <item><description><strong>Counting Phase:</strong> For each element, its key is extracted and countArray[key + offset] is incremented.
/// This records how many times each key appears.</description></item>
/// <item><description><strong>Cumulative Sum:</strong> The count array is transformed into cumulative counts.
/// countArray[i] becomes the number of elements with keys ≤ i, indicating the final position.</description></item>
/// <item><description><strong>Placement Phase:</strong> Elements are placed in reverse order (for stability).
/// For each element with key k, it is placed at position countArray[k + offset] - 1, then the count is decremented.</description></item>
/// <item><description><strong>Stability:</strong> Processing elements in reverse order ensures that elements with equal keys maintain their original relative order.</description></item>
/// <item><description><strong>Range Limitation:</strong> The key range must be reasonable (≤ {MaxCountArraySize}).
/// Excessive ranges cause memory allocation failures.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution</description></item>
/// <item><description>Stable      : Yes (reverse-order placement preserves relative order)</description></item>
/// <item><description>In-place    : No (O(n + k) where k = range of keys)</description></item>
/// <item><description>Comparisons : 0 (No comparison operations between keys)</description></item>
/// <item><description>Time        : O(n + k) where k is the range of keys</description></item>
/// <item><description>Memory      : O(n + k)</description></item>
/// <item><description>Note        : キーの範囲が大きいとメモリ使用量が膨大になります。最大範囲は{MaxCountArraySize}です。</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Counting_sort</para>
/// </remarks>
public static class CountingSort
{
    private const int MaxCountArraySize = 10_000_000; // Maximum allowed count array size
    private const int StackAllocThreshold = 1024; // Use stackalloc for count arrays smaller than this

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Temporary buffer for sorted elements

    /// <summary>
    /// Sorts the elements in the specified span using a key selector function.
    /// </summary>
    public static void Sort<T>(Span<T> span, Func<T, int> keySelector) where T : IComparable<T>
        => Sort(span, keySelector, Comparer<T>.Default, NullContext.Default);

    /// <summary>
    /// Sorts the elements in the specified span using a key selector function and sort context.
    /// </summary>
    public static void Sort<T>(Span<T> span, Func<T, int> keySelector, ISortContext context) where T : IComparable<T>
        => Sort(span, keySelector, Comparer<T>.Default, context);

    /// <summary>
    /// Sorts the elements in the specified span using a key selector function, comparer, and sort context.
    /// </summary>
    public static void Sort<T, TComparer>(Span<T> span, Func<T, int> keySelector, TComparer comparer, ISortContext context) where TComparer : IComparer<T>
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T, TComparer>(span, context, comparer, BUFFER_MAIN);

        // Rent arrays from ArrayPool for temporary storage
        var keysArray = ArrayPool<int>.Shared.Rent(span.Length);
        var tempArray = ArrayPool<T>.Shared.Rent(span.Length);
        try
        {
            // Create SortSpan for temp buffer to track operations
            var tempSpan = new SortSpan<T, TComparer>(tempArray.AsSpan(0, span.Length), context, comparer, BUFFER_MAIN);
            var keys = keysArray.AsSpan(0, span.Length);

            SortCore(span, keySelector, s, tempSpan, keys);
        }
        finally
        {
            ArrayPool<int>.Shared.Return(keysArray);
            ArrayPool<T>.Shared.Return(tempArray, clearArray: true);
        }
    }

    private static void SortCore<T, TComparer>(Span<T> span, Func<T, int> keySelector, SortSpan<T, TComparer> s, SortSpan<T, TComparer> tempSpan, Span<int> keys) where TComparer : IComparer<T>
    {
        // Find min/max and cache keys in single pass
        var min = int.MaxValue;
        var max = int.MinValue;

        for (var i = 0; i < span.Length; i++)
        {
            var key = keySelector(s.Read(i));
            keys[i] = key;
            if (key < min) min = key;
            if (key > max) max = key;
        }

        // If all keys are the same, no need to sort
        if (min == max) return;

        // Check for overflow and validate range
        long range = (long)max - (long)min + 1;
        if (range > int.MaxValue)
            throw new ArgumentException($"Key range is too large for CountingSort: {range}. Maximum supported range is {int.MaxValue}.");
        if (range > MaxCountArraySize)
            throw new ArgumentException($"Key range ({range}) exceeds maximum count array size ({MaxCountArraySize}). Consider using QuickSort or another comparison-based sort.");

        var offset = -min; // Offset to normalize keys to 0-based index
        var size = (int)range;

        // Use stackalloc for small count arrays, ArrayPool for larger ones
        int[]? rentedCountArray = null;
        Span<int> countArray = size <= StackAllocThreshold
            ? stackalloc int[size]
            : (rentedCountArray = ArrayPool<int>.Shared.Rent(size)).AsSpan(0, size);
        countArray.Clear();
        try
        {
            CountSort(s, keys, tempSpan, countArray, offset);
        }
        finally
        {
            if (rentedCountArray is not null)
            {
                ArrayPool<int>.Shared.Return(rentedCountArray);
            }
        }
    }

    /// <summary>
    /// Core counting sort implementation.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CountSort<T, TComparer>(SortSpan<T, TComparer> s, Span<int> keys, SortSpan<T, TComparer> tempSpan, Span<int> countArray, int offset) where TComparer : IComparer<T>
    {
        // Count occurrences of each key
        for (var i = 0; i < s.Length; i++)
        {
            countArray[keys[i] + offset]++;
        }

        // Calculate cumulative counts (for stable sort)
        for (var i = 1; i < countArray.Length; i++)
        {
            countArray[i] += countArray[i - 1];
        }

        // Build result array in reverse order to maintain stability
        for (var i = s.Length - 1; i >= 0; i--)
        {
            var key = keys[i];
            var index = key + offset;
            var pos = countArray[index] - 1;
            tempSpan.Write(pos, s.Read(i));
            countArray[index]--;
        }

        // Write sorted data back to original span using CopyTo for efficiency
        tempSpan.CopyTo(0, s, 0, s.Length);
    }
}

/// <summary>
/// 整数値を直接カウンティングソートでソートします。
/// 各値の出現回数をカウントし、累積和を計算して正しい位置に配置する安定なソートアルゴリズムです。
/// 値の範囲が狭い場合に非常に高速ですが、範囲が広いとメモリを大量に消費します。
/// <br/>
/// Directly sorts integer values using counting sort.
/// A stable sorting algorithm that counts occurrences and uses cumulative sums to place elements.
/// Very fast when the value range is narrow, but consumes significant memory for wide ranges.
/// </summary>
/// <remarks>
/// <para><strong>Supported Types:</strong></para>
/// <list type="bullet">
/// <item><description><strong>Supported:</strong> byte, sbyte, short, ushort, int, uint, long, ulong, nint, nuint (up to 64-bit)</description></item>
/// <item><description><strong>Not Supported:</strong> Int128, UInt128, BigInteger (>64-bit types)</description></item>
/// </list>
/// <para><strong>Why Int128/UInt128 are not supported:</strong></para>
/// <para>This implementation uses long for range calculation. Supporting 128-bit types would require significantly more complex
/// logic and is not practical for counting sort (the count array would be enormous). If you need to sort Int128/UInt128,
/// consider using a comparison-based sort like QuickSort or IntroSort.</para>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution</description></item>
/// <item><description>Stable      : Yes</description></item>
/// <item><description>In-place    : No (O(n + k) where k = range of values)</description></item>
/// <item><description>Comparisons : 0 (No comparison operations)</description></item>
/// <item><description>Swaps       : 0</description></item>
/// <item><description>Time        : O(n + k) where k is the range of values</description></item>
/// <item><description>Memory      : O(n + k)</description></item>
/// <item><description>Note        : 値の範囲が大きいとメモリ使用量が膨大になります。最大範囲は{MaxCountArraySize}です。</description></item>
/// </list>
/// </remarks>
public static class CountingSortInteger
{
    private const int MaxCountArraySize = 10_000_000; // Maximum allowed count array size
    private const int StackAllocThreshold = 1024; // Use stackalloc for count arrays smaller than this

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Temporary buffer for sorted elements

    /// <summary>
    /// Sorts integer values in the specified span (generic version for IBinaryInteger types).
    /// </summary>
    public static void Sort<T>(Span<T> span)
        where T : IBinaryInteger<T>, IMinMaxValue<T>, IComparable<T>
    {
        Sort(span, Comparer<T>.Default, NullContext.Default);
    }

    /// <summary>
    /// Sorts integer values in the specified span with sort context (generic version for IBinaryInteger types).
    /// </summary>
    public static void Sort<T>(Span<T> span, ISortContext context)
        where T : IBinaryInteger<T>, IMinMaxValue<T>, IComparable<T>
        => Sort(span, Comparer<T>.Default, context);

    /// <summary>
    /// Sorts integer values in the specified span with comparer and sort context (generic version for IBinaryInteger types).
    /// </summary>
    public static void Sort<T, TComparer>(Span<T> span, TComparer comparer, ISortContext context)
        where T : IBinaryInteger<T>, IMinMaxValue<T>
        where TComparer : IComparer<T>
    {
        if (span.Length <= 1) return;

        // Check if type is supported (64-bit or less)
        var bitSize = GetBitSize<T>();

        var s = new SortSpan<T, TComparer>(span, context, comparer, BUFFER_MAIN);

        // Rent arrays from ArrayPool for temporary storage
        var tempArray = ArrayPool<T>.Shared.Rent(span.Length);
        try
        {
            // Create SortSpan for temp buffer to track operations
            var tempSpan = new SortSpan<T, TComparer>(tempArray.AsSpan(0, span.Length), context, comparer, BUFFER_TEMP);

            SortCore(span, s, tempSpan);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(tempArray, clearArray: true);
        }
    }

    private static void SortCore<T, TComparer>(Span<T> span, SortSpan<T, TComparer> s, SortSpan<T, TComparer> tempSpan)
        where T : IBinaryInteger<T>, IMinMaxValue<T>
        where TComparer : IComparer<T>
    {
        // Find min and max to determine range
        // Convert to long for range calculation
        var minValue = T.MaxValue;
        var maxValue = T.MinValue;

        for (var i = 0; i < s.Length; i++)
        {
            var value = s.Read(i);
            if (s.Compare(value, minValue) < 0) minValue = value;
            if (s.Compare(value, maxValue) > 0) maxValue = value;
        }

        // If all elements are the same, no need to sort
        if (s.Compare(minValue, maxValue) == 0) return;

        // Convert to long for range calculation
        var min = ConvertToLong(minValue);
        var max = ConvertToLong(maxValue);

        // Check for overflow and validate range
        long range = max - min + 1;
        if (range > int.MaxValue)
            throw new ArgumentException($"Value range is too large for CountingSort: {range}. Maximum supported range is {int.MaxValue}.");
        if (range > MaxCountArraySize)
            throw new ArgumentException($"Value range ({range}) exceeds maximum count array size ({MaxCountArraySize}). Consider using QuickSort or another comparison-based sort.");

        var offset = -min; // Offset to normalize values to 0-based index
        var size = (int)range;

        // Use stackalloc for small count arrays, ArrayPool for larger ones
        int[]? rentedCountArray = null;
        Span<int> countArray = size <= StackAllocThreshold
            ? stackalloc int[size]
            : (rentedCountArray = ArrayPool<int>.Shared.Rent(size)).AsSpan(0, size);
        countArray.Clear();
        try
        {
            CountSort(s, tempSpan, countArray, offset);
        }
        finally
        {
            if (rentedCountArray is not null)
            {
                ArrayPool<int>.Shared.Return(rentedCountArray);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CountSort<T, TComparer>(SortSpan<T, TComparer> s, SortSpan<T, TComparer> tempSpan, Span<int> countArray, long offset)
        where T : IBinaryInteger<T>
        where TComparer : IComparer<T>
    {
        // Count occurrences
        for (var i = 0; i < s.Length; i++)
        {
            var value = s.Read(i);
            var index = (int)(ConvertToLong(value) + offset);
            countArray[index]++;
        }

        // Calculate cumulative counts (for stable sort)
        for (var i = 1; i < countArray.Length; i++)
        {
            countArray[i] += countArray[i - 1];
        }

        // Build result array in reverse order to maintain stability
        for (var i = s.Length - 1; i >= 0; i--)
        {
            var value = s.Read(i);
            var index = (int)(ConvertToLong(value) + offset);
            var pos = countArray[index] - 1;
            tempSpan.Write(pos, value);
            countArray[index]--;
        }

        // Write sorted data back to original span using CopyTo for efficiency
        tempSpan.CopyTo(0, s, 0, s.Length);
    }

    /// <summary>
    /// Get bit size of the type T and validate support.
    /// Throws NotSupportedException for types larger than 64-bit.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetBitSize<T>() where T : System.Numerics.IBinaryInteger<T>
    {
        if (typeof(T) == typeof(byte) || typeof(T) == typeof(sbyte))
            return 8;
        else if (typeof(T) == typeof(short) || typeof(T) == typeof(ushort))
            return 16;
        else if (typeof(T) == typeof(int) || typeof(T) == typeof(uint))
            return 32;
        else if (typeof(T) == typeof(long) || typeof(T) == typeof(ulong))
            return 64;
        else if (typeof(T) == typeof(nint) || typeof(T) == typeof(nuint))
            return IntPtr.Size * 8;
        else if (typeof(T) == typeof(System.Int128) || typeof(T) == typeof(System.UInt128))
            throw new NotSupportedException($"Type {typeof(T).Name} with 128-bit size is not supported. Maximum supported bit size is 64.");
        else
            throw new NotSupportedException($"Type {typeof(T).Name} is not supported.");
    }

    /// <summary>
    /// Convert IBinaryInteger value to long for range calculation.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long ConvertToLong<T>(T value) where T : System.Numerics.IBinaryInteger<T>
    {
        return long.CreateTruncating(value);
    }
}
