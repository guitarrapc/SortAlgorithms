using SortAlgorithm.Contexts;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// ピジョンホールソート（鳩の巣ソート）は、分布ソートの一種で、各値を対応する「穴」（バケット）に配置してソートします。
/// キーの範囲が狭い場合に非常に高速ですが、範囲が広いとメモリを大量に消費します。
/// <br/>
/// Pigeonhole sort is a distribution sort that places each value into its corresponding "hole" (bucket) for sorting.
/// Very fast when the key range is narrow, but consumes significant memory for wide ranges.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Pigeonhole Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Key Extraction:</strong> Each element must have a deterministic integer key obtained via the key selector function.
/// The key must be stable (same element always produces the same key).</description></item>
/// <item><description><strong>Range Determination:</strong> The algorithm finds min and max keys to determine the range [min, max].
/// A hole array of size (max - min + 1) is allocated. Each index corresponds to one unique key value.</description></item>
/// <item><description><strong>Offset Normalization:</strong> Keys are normalized using offset = -min, mapping keys to array indices [0, range-1].
/// This allows handling negative keys correctly: holes[key + offset] maps key to its hole.</description></item>
/// <item><description><strong>Distribution Phase:</strong> Each element is placed into its corresponding hole based on its key.
/// For element e with key k, increment holes[k + offset] to count occurrences.</description></item>
/// <item><description><strong>Collection Phase:</strong> Iterate through holes array in ascending order.
/// For each hole i with count c > 0, write the key (i - offset) back to the original array c times.
/// This reconstructs the sorted sequence in O(n + k) time.</description></item>
/// <item><description><strong>Correctness Guarantee:</strong> Since holes are traversed in index order (0 to range-1),
/// and each index i corresponds to key (i - offset), elements are written back in ascending key order.
/// The algorithm correctly sorts as long as the key selector function produces consistent integer keys.</description></item>
/// <item><description><strong>Stability:</strong> This implementation IS stable because elements with equal keys
/// are written back in their original relative order. By iterating forward through the temp array and incrementing hole positions,
/// elements with the same key are placed in the order they appeared in the input.</description></item>
/// <item><description><strong>Range Limitation:</strong> The key range must be reasonable (≤ {MaxHoleArraySize}).
/// Excessive ranges cause memory allocation failures or out-of-memory errors.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution</description></item>
/// <item><description>Stable      : Yes (preserves relative order of elements with equal keys)</description></item>
/// <item><description>In-place    : No (O(n + k) auxiliary space where k = range of keys)</description></item>
/// <item><description>Best case   : O(n + k) - All cases have the same complexity</description></item>
/// <item><description>Average case: O(n + k) - Linear in input size plus key range</description></item>
/// <item><description>Worst case  : O(n + k) - Even with all elements having different keys</description></item>
/// <item><description>Comparisons : 0 - No comparison operations between keys (distribution sort)</description></item>
/// <item><description>IndexReads  : 3n - n reads for key extraction, n reads for copying to temp, n reads for writing back</description></item>
/// <item><description>IndexWrites : 2n - n writes to temp, n writes back to original array</description></item>
/// <item><description>Memory      : O(n + k) - Temporary arrays for elements, keys, and hole counts</description></item>
/// <item><description>Note        : キーの範囲が大きいとメモリ使用量が膨大になります。最大範囲は{MaxHoleArraySize}です。</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Pigeonhole_sort</para>
/// </remarks>
public static class PigeonholeSort
{
    private const int MaxHoleArraySize = 10_000_000; // Maximum allowed hole array size
    private const int StackAllocThreshold = 1024; // Use stackalloc for hole arrays smaller than this

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Temporary buffer for elements

    /// <summary>
    /// Sorts the elements in the specified span using a key selector function.
    /// Uses NullContext for zero-overhead fast path.
    /// </summary>
    public static void Sort<T>(Span<T> span, Func<T, int> keySelector) where T : IComparable<T>
        => Sort(span, keySelector, new ComparableComparer<T>(), NullContext.Default);

    /// <summary>
    /// Sorts the elements in the specified span using a key selector function and sort context.
    /// </summary>
    public static void Sort<T, TContext>(Span<T> span, Func<T, int> keySelector, TContext context)
        where T : IComparable<T>
        where TContext : ISortContext
        => Sort(span, keySelector, new ComparableComparer<T>(), context);

    /// <summary>
    /// Sorts the elements in the specified span using a key selector function, comparer, and sort context.
    /// This is the full-control version with explicit TContext type parameter.
    /// </summary>
    public static void Sort<T, TComparer, TContext>(Span<T> span, Func<T, int> keySelector, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);

        // Rent arrays from ArrayPool for temporary storage
        var keysArray = ArrayPool<int>.Shared.Rent(span.Length);
        var tempArray = ArrayPool<T>.Shared.Rent(span.Length);
        try
        {
            // Create SortSpan for temp buffer to track operations
            var tempSpan = new SortSpan<T, TComparer, TContext>(tempArray.AsSpan(0, span.Length), context, comparer, BUFFER_TEMP);
            var keys = keysArray.AsSpan(0, span.Length);

            SortCore(span, keySelector, s, tempSpan, keys);
        }
        finally
        {
            ArrayPool<int>.Shared.Return(keysArray);
            ArrayPool<T>.Shared.Return(tempArray, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    private static void SortCore<T, TComparer, TContext>(Span<T> span, Func<T, int> keySelector, SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> tempSpan, Span<int> keys)
        where TComparer : IComparer<T>
        where TContext : ISortContext
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
            throw new ArgumentException($"Key range is too large for PigeonholeSort: {range}. Maximum supported range is {int.MaxValue}.");
        if (range > MaxHoleArraySize)
            throw new ArgumentException($"Key range ({range}) exceeds maximum hole array size ({MaxHoleArraySize}). Consider using QuickSort or another comparison-based sort.");

        var offset = -min; // Offset to normalize keys to 0-based index
        var size = (int)range;

        // Use stackalloc for small hole arrays, ArrayPool for larger ones
        int[]? rentedHoleArray = null;
        Span<int> holes = size <= StackAllocThreshold
            ? stackalloc int[size]
            : (rentedHoleArray = ArrayPool<int>.Shared.Rent(size)).AsSpan(0, size);
        holes.Clear();
        try
        {
            PigeonholeDistribute(s, tempSpan, keys, holes, offset);
        }
        finally
        {
            if (rentedHoleArray is not null)
            {
                ArrayPool<int>.Shared.Return(rentedHoleArray);
            }
        }
    }

    /// <summary>
    /// Pigeonhole distribution implementation.
    /// Achieves O(n + k) complexity by processing elements in a single pass.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void PigeonholeDistribute<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> source, SortSpan<T, TComparer, TContext> temp, Span<int> keys, Span<int> holes, int offset)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Phase 1: Copy elements to temp array and count occurrences (O(n))
        for (var i = 0; i < source.Length; i++)
        {
            temp.Write(i, source.Read(i));
            holes[keys[i] + offset]++;
        }

        // Phase 2: Calculate starting positions for each key (O(k))
        // Transform counts into cumulative positions
        // After this, holes[i] indicates where the next element with key (i - offset) should be placed
        var position = 0;
        for (var i = 0; i < holes.Length; i++)
        {
            var count = holes[i];
            holes[i] = position;
            position += count;
        }

        // Phase 3: Place elements in sorted order (O(n))
        // Iterate through temp array and place each element at its correct position
        for (var i = 0; i < source.Length; i++)
        {
            var holeIndex = keys[i] + offset;
            var targetPos = holes[holeIndex];
            source.Write(targetPos, temp.Read(i));
            holes[holeIndex]++; // Move to next position for this key
        }
    }
}

/// <summary>
/// 整数値を直接ピジョンホールソートでソートします。
/// 各値を対応する「穴」（バケット）に配置してソートする、安定なソートアルゴリズムです。
/// 値の範囲が狭い場合に非常に高速ですが、範囲が広いとメモリを大量に消費します。
/// <br/>
/// Directly sorts integer values using pigeonhole sort.
/// A stable sorting algorithm that places each value into its corresponding "hole" (bucket).
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
/// logic and is not practical for pigeonhole sort (the hole array would be enormous). If you need to sort Int128/UInt128,
/// consider using a comparison-based sort like QuickSort or IntroSort.</para>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution</description></item>
/// <item><description>Stable      : Yes (preserves relative order of elements with equal values)</description></item>
/// <item><description>In-place    : No (O(n + k) where k = range of values)</description></item>
/// <item><description>Comparisons : 0 (No comparison operations)</description></item>
/// <item><description>Swaps       : 0</description></item>
/// <item><description>Time        : O(n + k) where k is the range of values</description></item>
/// <item><description>Memory      : O(n + k)</description></item>
/// <item><description>Note        : 値の範囲が大きいとメモリ使用量が膨大になります。最大範囲は{MaxHoleArraySize}です。</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Pigeonhole_sort</para>
/// </remarks>
public static class PigeonholeSortInteger
{
    private const int MaxHoleArraySize = 10_000_000; // Maximum allowed hole array size
    private const int StackAllocThreshold = 1024; // Use stackalloc for hole arrays smaller than this

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Temporary buffer for elements

    /// <summary>
    /// Sorts the elements in the specified span using American Flag Sort.
    /// Uses NullContext for zero-overhead fast path.
    /// </summary>
    /// <typeparam name="T"> The type of elements to sort. Must be a binary integer type with defined min/max values.</typeparam>
    /// <param name="span"> The span of elements to sort.</param>
    public static void Sort<T>(Span<T> span) where T : IBinaryInteger<T>, IMinMaxValue<T>
        => Sort(span, new ComparableComparer<T>(), NullContext.Default);

    /// <summary>
    /// Sorts the elements in the specified span using American Flag Sort with sort context.
    /// </summary>
    /// <typeparam name="T"> The type of elements to sort. Must be a binary integer type with defined min/max values.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="span"> The span of elements to sort.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation.     
    public static void Sort<T, TContext>(Span<T> span, TContext context)
        where T : IBinaryInteger<T>, IMinMaxValue<T>
        where TContext : ISortContext
        => Sort(span, new ComparableComparer<T>(), context);

    /// <summary>
    /// Sorts integer values in the specified span with comparer and sort context.
    /// This is the full-control version with explicit TContext type parameter.
    /// </summary>
    public static void Sort<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context)
        where T : IBinaryInteger<T>, IMinMaxValue<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;

        // Check if type is supported (64-bit or less)
        var bitSize = GetBitSize<T>();

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);

        // Rent arrays from ArrayPool for temporary storage
        var tempArray = ArrayPool<T>.Shared.Rent(span.Length);
        try
        {
            // Create SortSpan for temp buffer to track operations
            var tempSpan = new SortSpan<T, TComparer, TContext>(tempArray.AsSpan(0, span.Length), context, comparer, BUFFER_TEMP);

            SortCore(span, s, tempSpan);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(tempArray, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    private static void SortCore<T, TComparer, TContext>(Span<T> span, SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> tempSpan)
        where T : IBinaryInteger<T>, IMinMaxValue<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Find min and max to determine range
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
            throw new ArgumentException($"Value range is too large for PigeonholeSort: {range}. Maximum supported range is {int.MaxValue}.");
        if (range > MaxHoleArraySize)
            throw new ArgumentException($"Value range ({range}) exceeds maximum hole array size ({MaxHoleArraySize}). Consider using QuickSort or another comparison-based sort.");

        var offset = -min; // Offset to normalize values to 0-based index
        var size = (int)range;

        // Use stackalloc for small hole arrays, ArrayPool for larger ones
        int[]? rentedHoleArray = null;
        Span<int> holes = size <= StackAllocThreshold
            ? stackalloc int[size]
            : (rentedHoleArray = ArrayPool<int>.Shared.Rent(size)).AsSpan(0, size);
        holes.Clear();
        try
        {
            PigeonholeDistribute(s, tempSpan, holes, offset);
        }
        finally
        {
            if (rentedHoleArray is not null)
            {
                ArrayPool<int>.Shared.Return(rentedHoleArray);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void PigeonholeDistribute<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> source, SortSpan<T, TComparer, TContext> temp, Span<int> holes, long offset)
        where T : IBinaryInteger<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Phase 1: Copy elements to temp array and count occurrences (O(n))
        for (var i = 0; i < source.Length; i++)
        {
            var value = source.Read(i);
            temp.Write(i, value);
            var index = (int)(ConvertToLong(value) + offset);
            holes[index]++;
        }

        // Phase 2: Calculate starting positions for each value (O(k))
        // Transform counts into cumulative positions
        var position = 0;
        for (var i = 0; i < holes.Length; i++)
        {
            var count = holes[i];
            holes[i] = position;
            position += count;
        }

        // Phase 3: Place elements in sorted order (O(n))
        // Stability is preserved by iterating forward and incrementing hole positions
        for (var i = 0; i < source.Length; i++)
        {
            var value = temp.Read(i);
            var index = (int)(ConvertToLong(value) + offset);
            var targetPos = holes[index];
            source.Write(targetPos, value);
            holes[index]++; // Move to next position for this value
        }
    }

    /// <summary>
    /// Get bit size of the type T and validate support.
    /// Throws NotSupportedException for types larger than 64-bit.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetBitSize<T>() where T : IBinaryInteger<T>
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
        else if (typeof(T) == typeof(Int128) || typeof(T) == typeof(UInt128))
            throw new NotSupportedException($"Type {typeof(T).Name} with 128-bit size is not supported. Maximum supported bit size is 64.");
        else
            throw new NotSupportedException($"Type {typeof(T).Name} is not supported.");
    }

    /// <summary>
    /// Convert IBinaryInteger value to long for range calculation.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long ConvertToLong<T>(T value) where T : IBinaryInteger<T>
    {
        return long.CreateTruncating(value);
    }
}
