using SortAlgorithm.Contexts;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// American Flag Sort - In-place MSD Radix Sortの実装。
/// 値をビット列として扱い、2ビットずつ（4種類）の桁に分けて要素を分類し、in-placeで並び替えます。
/// 最上位桁（Most Significant Digit）から最下位桁へ向かって処理することで、再帰的にソートを実現します。
/// RadixMSDSortと異なり、補助バッファの使用を最小限に抑え、配列内で要素をスワップすることでin-placeソートを実現します。
/// <br/>
/// American Flag Sort - An in-place MSD Radix Sort implementation.
/// Treats values as bit sequences, dividing them into 2-bit digits (4 buckets) and classifying elements in-place.
/// Processing from the Most Significant Digit to the least significant ensures a recursive sort.
/// Unlike RadixMSDSort, this implementation minimizes auxiliary buffer usage and achieves in-place sorting by swapping elements within the array.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct American Flag Sort (Base-4):</strong></para>
/// <list type="number">
/// <item><description><strong>Sign-Bit Flipping for Signed Integers:</strong> For signed types, the sign bit is flipped to convert signed values to unsigned keys:
/// - 32-bit: key = (uint)value ^ 0x8000_0000
/// - 64-bit: key = (ulong)value ^ 0x8000_0000_0000_0000
/// This ensures negative values are ordered correctly before positive values without separate processing.</description></item>
/// <item><description><strong>Digit Extraction Correctness:</strong> For each digit position d (from digitCount-1 down to 0), extract the d-th 2-bit digit using bitwise operations:
/// digit = (key >> (d × 2)) &amp; 0b11. This ensures each 2-bit segment of the integer is processed independently.</description></item>
/// <item><description><strong>In-Place Permutation:</strong> Elements are rearranged in-place using a two-pass approach:
/// 1. Count phase: Count occurrences of each digit value
/// 2. Permutation phase: Place each element in its correct bucket position using bucket offsets</description></item>
/// <item><description><strong>MSD Processing Order:</strong> Digits must be processed from most significant (d=digitCount-1) to least significant (d=0).
/// This top-down approach partitions the array into buckets recursively, processing each bucket independently for subsequent digits.</description></item>
/// <item><description><strong>Recursive Bucket Processing:</strong> After permuting elements based on the current digit, each bucket must be recursively sorted for the remaining digits.
/// Base cases: buckets with 0 or 1 elements are already sorted; buckets where all remaining digits are the same are also sorted.</description></item>
/// <item><description><strong>Cutoff to Insertion Sort:</strong> For small buckets (typically &lt; 16 elements), switching to insertion sort can improve performance due to lower overhead.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution (Radix Sort, MSD variant, American Flag Sort)</description></item>
/// <item><description>Stable      : No (in-place permutation does not preserve relative order)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space, excluding recursion stack)</description></item>
/// <item><description>Best case   : Θ(n) - When all elements fall into one bucket early</description></item>
/// <item><description>Average case: Θ(d × n) - d = ⌈bitSize/2⌉ is constant for fixed-width integers</description></item>
/// <item><description>Worst case  : Θ(d × n) - Same complexity regardless of input order</description></item>
/// <item><description>Comparisons : 0 (Non-comparison sort, uses bitwise operations only)</description></item>
/// <item><description>Digit Passes: up to d = ⌈bitSize/2⌉ (4 for byte, 8 for short, 16 for int, 32 for long), but can terminate early</description></item>
/// <item><description>Memory      : O(1) auxiliary space (excluding recursion stack which is O(log n) expected, O(n) worst case)</description></item>
/// </list>
/// <para><strong>American Flag Sort vs MSD Radix Sort:</strong></para>
/// <list type="bullet">
/// <item><description>American Flag Sort is in-place, while MSD Radix Sort requires O(n) auxiliary buffer</description></item>
/// <item><description>American Flag Sort is not stable, while MSD Radix Sort can be implemented as stable</description></item>
/// <item><description>American Flag Sort has better cache locality due to in-place permutation</description></item>
/// <item><description>American Flag Sort has similar time complexity but better space complexity</description></item>
/// <item><description>American Flag Sort requires more swap operations, which may be slower for large element types</description></item>
/// </list>
/// <para><strong>Algorithm Overview:</strong></para>
/// <para>The algorithm consists of three phases per digit level:</para>
/// <list type="number">
/// <item><description><strong>Count Phase:</strong> Count occurrences of each digit value (0-3)</description></item>
/// <item><description><strong>Offset Calculation:</strong> Compute bucket offsets (cumulative sum)</description></item>
/// <item><description><strong>Permutation Phase:</strong> Rearrange elements into their buckets in-place using cyclic permutations</description></item>
/// <item><description><strong>Recursive Phase:</strong> Recursively sort each non-empty bucket for the next digit</description></item>
/// </list>
/// <para><strong>Supported Types:</strong></para>
/// <list type="bullet">
/// <item><description><strong>Supported:</strong> byte, sbyte, short, ushort, int, uint, long, ulong, nint, nuint (up to 64-bit)</description></item>
/// <item><description><strong>Not Supported:</strong> Int128, UInt128, BigInteger (&gt;64-bit types)</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/American_flag_sort</para>
/// <para>Paper: "Engineering Radix Sort" by McIlroy, Bostic, and McIlroy (1993)</para>
/// </remarks>
public static class AmericanFlagSort
{
    private const int RadixBits = 2;        // 2 bits per digit
    private const int RadixSize = 4;        // 2^2 = 4 buckets
    private const int InsertionSortCutoff = 16; // Switch to insertion sort for small buckets

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array

    private readonly struct AmericanFlagSortAction<T, TComparer> : ContextDispatcher.SortAction<T, TComparer>
        where T : IBinaryInteger<T>, IMinMaxValue<T>
        where TComparer : IComparer<T>
    {
        public void Invoke<TContext>(Span<T> span, TComparer comparer, TContext context)
            where TContext : ISortContext
        {
            Sort<T, TComparer, TContext>(span, comparer, context);
        }
    }

    /// <summary>
    /// Sorts the elements in the specified span using American Flag Sort.
    /// Uses NullContext for zero-overhead fast path.
    /// </summary>
    public static void Sort<T>(Span<T> span) where T : IBinaryInteger<T>, IMinMaxValue<T>
        => Sort<T, ComparableComparer<T>, NullContext>(span, new ComparableComparer<T>(), NullContext.Default);

    /// <summary>
    /// Sorts the elements in the specified span using American Flag Sort with sort context.
    /// </summary>
    public static void Sort<T>(Span<T> span, ISortContext context) where T : IBinaryInteger<T>, IMinMaxValue<T>
        => ContextDispatcher.DispatchSort(span, new ComparableComparer<T>(), context, new AmericanFlagSortAction<T, ComparableComparer<T>>());

    /// <summary>
    /// Sorts the elements in the specified span using American Flag Sort with comparer and sort context.
    /// </summary>
    public static void Sort<T, TComparer>(Span<T> span, TComparer comparer, ISortContext context)
        where T : IBinaryInteger<T>, IMinMaxValue<T>
        where TComparer : IComparer<T>
        => ContextDispatcher.DispatchSort(span, comparer, context, new AmericanFlagSortAction<T, TComparer>());

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

        // Rent bucket offsets array from ArrayPool
        var bucketOffsetsArray = ArrayPool<int>.Shared.Rent(RadixSize + 1);

        try
        {
            var bucketOffsets = bucketOffsetsArray.AsSpan(0, RadixSize + 1);
            SortCore(span, bucketOffsets, comparer, context);
        }
        finally
        {
            ArrayPool<int>.Shared.Return(bucketOffsetsArray);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SortCore<T, TComparer, TContext>(Span<T> span, Span<int> bucketOffsets, TComparer comparer, TContext context)
        where T : IBinaryInteger<T>, IMinMaxValue<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);

        // Determine the number of digits based on type size
        // GetBitSize throws NotSupportedException for unsupported types (>64-bit)
        var bitSize = GetBitSize<T>();

        // Calculate digit count from bit size (2 bits per digit)
        var digitCount = (bitSize + RadixBits - 1) / RadixBits;

        // Start American Flag Sort from the most significant digit
        AmericanFlagSortRecursive(s, 0, s.Length, digitCount - 1, bitSize, bucketOffsets);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AmericanFlagSortRecursive<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int start, int length, int digit, int bitSize, Span<int> bucketOffsets)
        where T : IBinaryInteger<T>, IMinMaxValue<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Base case: if length is small, use insertion sort
        if (length <= InsertionSortCutoff)
        {
            InsertionSort.SortCore(s, start, start + length);
            return;
        }

        // Base case: if we've processed all digits, we're done
        if (digit < 0)
        {
            return;
        }

        var shift = digit * RadixBits;

        // Phase 1: Count occurrences of each digit value
        bucketOffsets.Clear();

        for (var i = 0; i < length; i++)
        {
            var value = s.Read(start + i);
            var key = GetUnsignedKey(value, bitSize);
            var digitValue = (int)((key >> shift) & 0b11);  // Extract 2-bit digit
            bucketOffsets[digitValue + 1]++;
        }

        // Phase 2: Calculate bucket offsets (prefix sum) and save bucket start positions
        Span<int> bucketStarts = stackalloc int[RadixSize];
        bucketStarts[0] = 0;
        for (var i = 1; i <= RadixSize; i++)
        {
            bucketOffsets[i] += bucketOffsets[i - 1];
            if (i < RadixSize)
            {
                bucketStarts[i] = bucketOffsets[i];
            }
        }

        // Phase 3: In-place permutation
        // Rearrange elements into their correct buckets using cyclic permutation
        PermuteInPlace(s, start, length, shift, bitSize, bucketOffsets, bucketStarts);

        // Phase 4: Recursively sort each bucket for the next digit
        for (var i = 0; i < RadixSize; i++)
        {
            var bucketStart = bucketStarts[i];
            var bucketEnd = (i == RadixSize - 1) ? length : bucketStarts[i + 1];
            var bucketLength = bucketEnd - bucketStart;

            if (bucketLength > 1)
            {
                AmericanFlagSortRecursive(s, start + bucketStart, bucketLength, digit - 1, bitSize, bucketOffsets);
            }
        }
    }

    /// <summary>
    /// Permutes elements in-place into their correct buckets.
    /// Uses a technique similar to cyclic permutation to avoid using auxiliary buffer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void PermuteInPlace<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int start, int length, int shift, int bitSize, Span<int> bucketOffsets, Span<int> bucketStarts)
        where T : IBinaryInteger<T>, IMinMaxValue<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Reset bucket offsets to their starting positions
        bucketStarts.CopyTo(bucketOffsets);

        // In-place permutation using bucket positions
        // Process each bucket sequentially
        for (var bucket = 0; bucket < RadixSize; bucket++)
        {
            // Get the range for this bucket
            var bucketStart = bucketStarts[bucket];
            var bucketEnd = (bucket == RadixSize - 1) ? length : bucketStarts[bucket + 1];

            // Move elements to their correct positions within and across buckets
            while (bucketOffsets[bucket] < bucketEnd)
            {
                var currentPos = start + bucketOffsets[bucket];
                var currentValue = s.Read(currentPos);
                var currentKey = GetUnsignedKey(currentValue, bitSize);
                var currentDigit = (int)((currentKey >> shift) & 0b11);

                // If element is already in correct bucket, advance
                if (currentDigit == bucket)
                {
                    bucketOffsets[bucket]++;
                    continue;
                }

                // Swap current element to its correct bucket
                var targetPos = start + bucketOffsets[currentDigit];
                s.Swap(currentPos, targetPos);
                bucketOffsets[currentDigit]++;
            }
        }
    }

    /// <summary>
    /// Get bit size of the type T.
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
    /// Convert a signed or unsigned value to an unsigned key for radix sorting.
    /// For signed types, flips the sign bit to ensure correct ordering (negative values sort before positive).
    /// For unsigned types, returns the value as-is.
    /// </summary>
    /// <remarks>
    /// Sign-bit flipping technique:
    /// - 32-bit signed: key = (uint)value ^ 0x8000_0000
    /// - 64-bit signed: key = (ulong)value ^ 0x8000_0000_0000_0000
    ///
    /// This ensures:
    /// - int.MinValue (-2147483648) → 0x0000_0000 (sorts first)
    /// - -1 → 0x7FFF_FFFF (sorts before 0)
    /// - 0 → 0x8000_0000 (sorts after negatives)
    /// - int.MaxValue (2147483647) → 0xFFFF_FFFF (sorts last)
    ///
    /// Advantages:
    /// - No Abs() needed, avoids MinValue overflow
    /// - Single unified pass for all values
    /// - Maintains correct ordering for signed types
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong GetUnsignedKey<T>(T value, int bitSize) where T : IBinaryInteger<T>
    {
        if (bitSize <= 8)
        {
            // byte or sbyte
            if (typeof(T) == typeof(sbyte))
            {
                var sbyteValue = sbyte.CreateTruncating(value);
                return (ulong)((byte)sbyteValue ^ 0x80);
            }
            else
            {
                return byte.CreateTruncating(value);
            }
        }
        else if (bitSize <= 16)
        {
            // short or ushort
            if (typeof(T) == typeof(short))
            {
                var shortValue = short.CreateTruncating(value);
                return (ulong)((ushort)shortValue ^ 0x8000);
            }
            else
            {
                return ushort.CreateTruncating(value);
            }
        }
        else if (bitSize <= 32)
        {
            // int, uint, or nint/nuint on 32-bit platform
            if (typeof(T) == typeof(int))
            {
                var intValue = int.CreateTruncating(value);
                return (uint)intValue ^ 0x8000_0000;
            }
            else if (typeof(T) == typeof(nint))
            {
                // nint is signed, needs sign-bit flip
                var nintValue = nint.CreateTruncating(value);
                return (uint)nintValue ^ 0x8000_0000;
            }
            else
            {
                // uint or nuint (unsigned, no flip needed)
                return uint.CreateTruncating(value);
            }
        }
        else if (bitSize <= 64)
        {
            // long, ulong, or nint/nuint on 64-bit platform
            if (typeof(T) == typeof(long))
            {
                var longValue = long.CreateTruncating(value);
                return (ulong)longValue ^ 0x8000_0000_0000_0000;
            }
            else if (typeof(T) == typeof(nint))
            {
                // nint is signed, needs sign-bit flip (64-bit platform)
                var nintValue = nint.CreateTruncating(value);
                return (ulong)nintValue ^ 0x8000_0000_0000_0000;
            }
            else
            {
                // ulong or nuint (unsigned, no flip needed)
                return ulong.CreateTruncating(value);
            }
        }
        else
        {
            throw new NotSupportedException($"Bit size {bitSize} is not supported");
        }
    }
}
