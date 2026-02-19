using SortAlgorithm.Contexts;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 値域を複数のバケットに分割し、各要素をキーに基づいてバケットに配置します。
/// 各バケット内をソートした後、バケットを順番に連結すればソートが完了します。
/// 値の分布が均等な場合に高速に動作する、汎用型対応のバケットソートアルゴリズムです。
/// <br/>
/// Divides the value range into multiple buckets and distributes elements based on their keys.
/// After sorting each bucket, concatenating them in order completes the sort.
/// A generic bucket sort algorithm that performs optimally when values are uniformly distributed.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Bucket Sort (Generic, Range-based):</strong></para>
/// <list type="number">
/// <item><description><strong>Key Extraction:</strong> Each element must have a deterministic integer key obtained via the key selector function.
/// The key must be stable (same element always produces the same key).</description></item>
/// <item><description><strong>Range Partitioning:</strong> The key range [min, max] is divided into k equal-sized buckets.
/// Each bucket i handles keys in the range [min + i×bucketSize, min + (i+1)×bucketSize).</description></item>
/// <item><description><strong>Distribution Function:</strong> Elements are distributed to buckets using: bucketIndex = ⌊(key - min) / bucketSize⌋.
/// This ensures all elements with similar keys go to the same bucket.</description></item>
/// <item><description><strong>Bucket Count Heuristic:</strong> This implementation uses k = min(max(√n, {MinBucketCount}), {MaxBucketCount}) buckets.
/// The √n heuristic balances distribution overhead and per-bucket sorting cost.</description></item>
/// <item><description><strong>Per-Bucket Sorting:</strong> Each bucket is sorted using Insertion Sort (stable, O(m²) for m elements).
/// This ensures stability if the inner sort is stable.</description></item>
/// <item><description><strong>Concatenation Order:</strong> Buckets are concatenated in ascending order (bucket 0, 1, 2, ...).
/// Since bucket i contains only keys less than bucket i+1, concatenation produces a sorted sequence.</description></item>
/// <item><description><strong>Uniform Distribution Assumption:</strong> Optimal performance (O(n)) requires uniform key distribution.
/// Worst case O(n²) occurs when all elements fall into a single bucket.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution</description></item>
/// <item><description>Stable      : Yes (Insertion Sort preserves relative order)</description></item>
/// <item><description>In-place    : No (O(n + k) auxiliary space for buckets)</description></item>
/// <item><description>Best case   : Ω(n + k) - Uniform distribution, each bucket has ~n/k elements</description></item>
/// <item><description>Average case: Θ(n + k) - Assumes uniform distribution, total sort cost n×(n/k)²/k + n + k ≈ n</description></item>
/// <item><description>Worst case  : O(n²) - All elements in one bucket, degenerates to Insertion Sort</description></item>
/// <item><description>Comparisons : O(n log(n/k)) on average - Each bucket sorted independently</description></item>
/// <item><description>Memory      : O(n + k) - k bucket lists plus n elements total</description></item>
/// <item><description>Note        : バケット数は√n (最小{MinBucketCount}、最大{MaxBucketCount}) に自動調整されます。キーの分布が偏るとパフォーマンスが低下します。</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Bucket_sort</para>
/// </remarks>
public static class BucketSort
{
    private const int MaxBucketCount = 1000; // Maximum number of buckets
    private const int MinBucketCount = 2;    // Minimum number of buckets

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Temporary buffer for sorted elements
    private const int BUFFER_BUCKET_BASE = 100; // Base ID for individual buckets (100, 101, 102, ...)

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

            SortCore(span, keySelector, s, tempSpan, tempArray.AsSpan(0, span.Length), keys, context);
        }
        finally
        {
            ArrayPool<int>.Shared.Return(keysArray);
            ArrayPool<T>.Shared.Return(tempArray, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    private static void SortCore<T, TComparer, TContext>(Span<T> span, Func<T, int> keySelector, SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> tempSpan, Span<T> tempArray, Span<int> keys, TContext context)
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

        // Determine bucket count based on input size and range
        long range = (long)max - (long)min + 1;

        // Calculate optimal bucket count (sqrt(n) is a common heuristic)
        var bucketCount = Math.Max(MinBucketCount, Math.Min(MaxBucketCount, (int)Math.Sqrt(span.Length)));

        // Adjust bucket count if range is smaller
        if (range < bucketCount)
        {
            bucketCount = (int)range;
        }

        // Calculate bucket size (range divided by bucket count)
        var bucketSize = Math.Max(1, (range + bucketCount - 1) / bucketCount);

        // Perform bucket distribution and sorting
        BucketDistribute(s, tempSpan, tempArray, keys, bucketCount, bucketSize, min);
    }

    private static void BucketDistribute<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> temp, Span<T> tempArray, Span<int> keys, int bucketCount, long bucketSize, int min)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Count elements per bucket and calculate bucket positions (stackalloc)
        Span<int> bucketCounts = stackalloc int[bucketCount];
        Span<int> bucketStarts = stackalloc int[bucketCount];
        Span<int> bucketPositions = stackalloc int[bucketCount];
        bucketCounts.Clear();

        // First pass: convert keys to bucket indices and count
        // Reuse keys array to store bucket indices (eliminates division in second pass)
        for (var i = 0; i < s.Length; i++)
        {
            var key = keys[i];
            var bucketIndex = (int)((key - min) / bucketSize);

            // Handle edge case where key == max
            if (bucketIndex >= bucketCount)
            {
                bucketIndex = bucketCount - 1;
            }

            keys[i] = bucketIndex; // Overwrite with bucket index
            bucketCounts[bucketIndex]++;
        }

        // Calculate starting position for each bucket in the temp array
        var offset = 0;
        for (var i = 0; i < bucketCount; i++)
        {
            bucketStarts[i] = offset;
            bucketPositions[i] = offset;
            offset += bucketCounts[i];
        }

        // Second pass: distribute elements using cached bucket indices
        for (var i = 0; i < s.Length; i++)
        {
            var bucketIndex = keys[i]; // Reuse bucket index (no division)
            var pos = bucketPositions[bucketIndex]++;
            temp.Write(pos, s.Read(i));
        }

        // Sort each bucket using Span slicing with SortSpan for tracking
        for (var i = 0; i < bucketCount; i++)
        {
            var count = bucketCounts[i];
            if (count > 1)
            {
                var start = bucketStarts[i];
                var bucketSpan = new SortSpan<T, TComparer, TContext>(tempArray.Slice(start, count), s.Context, s.Comparer, BUFFER_BUCKET_BASE + i);
                InsertionSortBucket(bucketSpan);
            }
        }

        // Write sorted data back to original span using CopyTo for better performance
        temp.CopyTo(0, s, 0, s.Length);
    }

    /// <summary>
    /// Insertion sort for bucket contents (stable sort)
    /// Uses SortSpan to track all operations for statistics and visualization
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void InsertionSortBucket<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        for (var i = 1; i < s.Length; i++)
        {
            var key = s.Read(i);
            var j = i - 1;

            while (j >= 0 && s.Compare(j, key) > 0)
            {
                s.Write(j + 1, s.Read(j));
                j--;
            }
            s.Write(j + 1, key);
        }
    }
}

/// <summary>
/// 整数専用のバケットソート。
/// 値域を複数のバケットに分割し、各要素を値に応じてバケットに配置後、各バケット内をソートしてから結合します。
/// 値の分布が均等な場合に高速に動作します。
/// <br/>
/// Integer-specific bucket sort.
/// Divides the value range into multiple buckets, distributes elements by value, sorts each bucket, then concatenates.
/// Performs optimally when values are uniformly distributed.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Bucket Sort (Range-based):</strong></para>
/// <list type="number">
/// <item><description><strong>Range Partitioning:</strong> The value range [min, max] is divided into k equal-sized buckets.
/// Each bucket i handles values in the range [min + i×bucketSize, min + (i+1)×bucketSize).</description></item>
/// <item><description><strong>Distribution Function:</strong> Elements are distributed to buckets using: bucketIndex = ⌊(value - min) / bucketSize⌋.
/// This ensures all elements with similar values go to the same bucket.</description></item>
/// <item><description><strong>Bucket Count Heuristic:</strong> This implementation uses k = min(max(√n, {MinBucketCount}), {MaxBucketCount}) buckets.
/// The √n heuristic balances distribution overhead and per-bucket sorting cost.</description></item>
/// <item><description><strong>Per-Bucket Sorting:</strong> Each bucket is sorted using Insertion Sort (stable, O(m²) for m elements).
/// This ensures stability if the inner sort is stable.</description></item>
/// <item><description><strong>Concatenation Order:</strong> Buckets are concatenated in ascending order (bucket 0, 1, 2, ...).
/// Since bucket i contains only values less than bucket i+1, concatenation produces a sorted sequence.</description></item>
/// <item><description><strong>Uniform Distribution Assumption:</strong> Optimal performance (O(n)) requires uniform distribution.
/// Worst case O(n²) occurs when all elements fall into a single bucket.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution</description></item>
/// <item><description>Stable      : Yes (Insertion Sort preserves relative order)</description></item>
/// <item><description>In-place    : No (O(n + k) auxiliary space for buckets)</description></item>
/// <item><description>Best case   : Ω(n + k) - Uniform distribution, each bucket has ~n/k elements</description></item>
/// <item><description>Average case: Θ(n + k) - Assumes uniform distribution, total sort cost n×(n/k)²/k + n + k ≈ n</description></item>
/// <item><description>Worst case  : O(n²) - All elements in one bucket, degenerates to Insertion Sort</description></item>
/// <item><description>Comparisons : O(n log(n/k)) on average - Each bucket sorted independently</description></item>
/// <item><description>Memory      : O(n + k) - k bucket lists plus n elements total</description></item>
/// <item><description>Note        : バケット数は√n (最小{MinBucketCount}、最大{MaxBucketCount}) に自動調整されます。</description></item>
/// </list>
/// </remarks>
public static class BucketSortInteger
{
    private const int MaxBucketCount = 1000; // Maximum number of buckets
    private const int MinBucketCount = 2;    // Minimum number of buckets

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Temporary buffer

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
        var indicesArray = ArrayPool<int>.Shared.Rent(span.Length);
        var tempArray = ArrayPool<T>.Shared.Rent(span.Length);
        try
        {
            // Create SortSpan for temp buffer to track operations
            var tempSpan = new SortSpan<T, TComparer, TContext>(tempArray.AsSpan(0, span.Length), context, comparer, BUFFER_TEMP);
            var indices = indicesArray.AsSpan(0, span.Length);

            SortCore(s, tempSpan, tempArray.AsSpan(0, span.Length), indices, context);
        }
        finally
        {
            ArrayPool<int>.Shared.Return(indicesArray);
            ArrayPool<T>.Shared.Return(tempArray, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    private static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> tempSpan, Span<T> tempArray, Span<int> bucketIndices, TContext context)
        where T : IBinaryInteger<T>, IMinMaxValue<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Find min and max
        var minValue = s.Read(0);
        var maxValue = s.Read(0);

        for (var i = 1; i < s.Length; i++)
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

        // Determine bucket count based on input size and range
        long range = max - min + 1;

        // Calculate optimal bucket count (sqrt(n) is a common heuristic)
        var bucketCount = Math.Max(MinBucketCount, Math.Min(MaxBucketCount, (int)Math.Sqrt(s.Length)));

        // Adjust bucket count if range is smaller
        if (range < bucketCount)
        {
            bucketCount = (int)range;
        }

        // Calculate bucket size (range divided by bucket count)
        var bucketSize = Math.Max(1, (range + bucketCount - 1) / bucketCount);

        // Perform bucket distribution and sorting
        BucketDistribute(s, tempSpan, tempArray, bucketIndices, bucketCount, bucketSize, min);
    }

    private static void BucketDistribute<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> source, SortSpan<T, TComparer, TContext> tempSpan, Span<T> tempArray, Span<int> bucketIndices, int bucketCount, long bucketSize, long min)
        where T : IBinaryInteger<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Count elements per bucket and calculate bucket positions (stackalloc)
        Span<int> bucketCounts = stackalloc int[bucketCount];
        Span<int> bucketStarts = stackalloc int[bucketCount];
        Span<int> bucketPositions = stackalloc int[bucketCount];
        bucketCounts.Clear();

        // First pass: calculate bucket indices and count
        // Cache bucket indices to avoid division in second pass
        for (var i = 0; i < source.Length; i++)
        {
            var value = source.Read(i);
            var valueLong = ConvertToLong(value);
            var bucketIndex = (int)((valueLong - min) / bucketSize);

            // Handle edge case where value == max
            if (bucketIndex >= bucketCount)
            {
                bucketIndex = bucketCount - 1;
            }

            bucketIndices[i] = bucketIndex; // Cache bucket index
            bucketCounts[bucketIndex]++;
        }

        // Calculate starting position for each bucket in the temp array
        var offset = 0;
        for (var i = 0; i < bucketCount; i++)
        {
            bucketStarts[i] = offset;
            bucketPositions[i] = offset;
            offset += bucketCounts[i];
        }

        // Second pass: distribute elements using cached bucket indices
        for (var i = 0; i < source.Length; i++)
        {
            var bucketIndex = bucketIndices[i]; // Reuse cached index (no division)
            var pos = bucketPositions[bucketIndex]++;
            tempSpan.Write(pos, source.Read(i));
        }

        // Sort each bucket using Span slicing
        for (var i = 0; i < bucketCount; i++)
        {
            var count = bucketCounts[i];
            if (count > 1)
            {
                var start = bucketStarts[i];
                InsertionSortGenericBucket(tempArray.Slice(start, count), source.Comparer);
            }
        }

        // Write sorted data back to original span using CopyTo for better performance
        tempSpan.CopyTo(0, source, 0, source.Length);
    }

    /// <summary>
    /// Insertion sort for generic bucket contents (stable sort)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void InsertionSortGenericBucket<T, TComparer>(Span<T> bucket, TComparer comparer)
        where TComparer : IComparer<T>
    {
        for (var i = 1; i < bucket.Length; i++)
        {
            var key = bucket[i];
            var j = i - 1;

            while (j >= 0 && comparer.Compare(bucket[j], key) > 0)
            {
                bucket[j + 1] = bucket[j];
                j--;
            }
            bucket[j + 1] = key;
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
