using SortAlgorithm.Contexts;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 10進数基数のMSD（Most Significant Digit）基数ソート。
/// 値を10進数の桁として扱い、最上位桁から最下位桁まで再帰的にバケットソートを行います。
/// 人間が理解しやすい10進数ベースのアルゴリズムで、デバッグや教育目的に適しています。
/// <br/>
/// Decimal-based MSD (Most Significant Digit) radix sort.
/// Treats values as decimal digits and performs bucket sorting recursively from the most significant digit to the least significant digit.
/// This decimal-based algorithm is easy for humans to understand and is suitable for debugging and educational purposes.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct MSD Radix Sort (Decimal Base):</strong></para>
/// <list type="number">
/// <item><description><strong>Sign-Bit Flipping for Signed Integers:</strong> For signed types, the sign bit is flipped to convert signed values to unsigned keys.
/// This ensures negative values are ordered correctly before positive values without separate processing.
/// This technique avoids the MinValue overflow issue with Abs() and maintains stability.</description></item>
/// <item><description><strong>Digit Extraction Consistency:</strong> For a given position from most significant digit, extract the digit using (key / divisor) % 10
/// where divisor = 10^(digitCount - 1 - d) for digit position d.</description></item>
/// <item><description><strong>MSD Processing Order:</strong> Digits must be processed from most significant (d=digitCount-1) to least significant (d=0).
/// This top-down approach partitions the array into buckets recursively, processing each bucket independently for subsequent digits.</description></item>
/// <item><description><strong>Recursive Bucket Processing:</strong> After distributing elements based on the current digit, each bucket must be recursively sorted for the remaining digits.
/// Base cases: buckets with 0 or 1 elements are already sorted; buckets where all remaining digits are the same are also sorted.</description></item>
/// <item><description><strong>Cutoff to Insertion Sort:</strong> For small buckets (typically &lt; 16 elements), switching to insertion sort can improve performance due to lower overhead.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution (Radix Sort, MSD variant)</description></item>
/// <item><description>Stable      : Yes (maintains relative order of elements with equal keys)</description></item>
/// <item><description>In-place    : No (O(n) auxiliary space for temporary buffer)</description></item>
/// <item><description>Best case   : Θ(n) - When all elements fall into one bucket early</description></item>
/// <item><description>Average case: Θ(d × n) - d = ⌈log₁₀(max)⌉ + 1 (number of decimal digits)</description></item>
/// <item><description>Worst case  : Θ(d × n) - Same complexity regardless of input order</description></item>
/// <item><description>Comparisons : 0 (Non-comparison sort, uses arithmetic operations only)</description></item>
/// <item><description>Digit Passes: up to d = ⌈log₁₀(max)⌉ + 1, but can terminate early</description></item>
/// <item><description>Memory      : O(n) for temporary buffer</description></item>
/// </list>
/// <para><strong>MSD vs LSD (Decimal):</strong></para>
/// <list type="bullet">
/// <item><description>MSD processes high-order digits first, enabling early termination when buckets are fully sorted</description></item>
/// <item><description>MSD is cache-friendlier for partially sorted data as it localizes accesses within buckets</description></item>
/// <item><description>MSD requires recursive processing of buckets, adding overhead compared to LSD's iterative approach</description></item>
/// <item><description>Both MSD and LSD can be implemented as stable sorts (this implementation maintains stability)</description></item>
/// </list>
/// <para><strong>Note:</strong> Uses decimal arithmetic (division and modulo), which may be slower than binary-based radix sorts (e.g., RadixMSD4Sort with bit shifts).
/// However, it is more intuitive for understanding and debugging.</para>
/// <para><strong>Supported Types:</strong></para>
/// <list type="bullet">
/// <item><description><strong>Supported:</strong> byte, sbyte, short, ushort, int, uint, long, ulong, nint, nuint (up to 64-bit)</description></item>
/// <item><description><strong>Not Supported:</strong> Int128, UInt128, BigInteger (&gt;64-bit types)</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Radix_sort#Most_significant_digit</para>
/// </remarks>
public static class RadixMSD10Sort
{
    private const int RadixBase = 10;       // Decimal base
    private const int InsertionSortCutoff = 16; // Switch to insertion sort for small buckets

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Temporary buffer for digit redistribution

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

    /// <summary>
    /// Sorts the elements in the specified span using the provided comparer and sort context.
    /// This is the full-control version with explicit TContext type parameter.
    /// </summary>
    public static void Sort<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context)
        where T : IBinaryInteger<T>, IMinMaxValue<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;

        // Rent buffers from ArrayPool
        var tempArray = ArrayPool<T>.Shared.Rent(span.Length);
        var bucketOffsetsArray = ArrayPool<int>.Shared.Rent(RadixBase + 1);

        try
        {
            var tempBuffer = tempArray.AsSpan(0, span.Length);
            var bucketOffsets = bucketOffsetsArray.AsSpan(0, RadixBase + 1);

            SortCore<T, TComparer, TContext>(span, tempBuffer, bucketOffsets, comparer, context);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(tempArray, clearArray: true);
            ArrayPool<int>.Shared.Return(bucketOffsetsArray);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SortCore<T, TComparer, TContext>(Span<T> span, Span<T> tempBuffer, Span<int> bucketOffsets, TComparer comparer, TContext context)
        where T : IBinaryInteger<T>, IMinMaxValue<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        var temp = new SortSpan<T, TComparer, TContext>(tempBuffer, context, comparer, BUFFER_TEMP);

        // Determine the bit size for sign-bit flipping
        var bitSize = GetBitSize<T>();

        // Calculate maximum number of decimal digits based on the type's bit size
        // MSD doesn't need to scan for min/max - empty buckets are naturally skipped
        var digitCount = GetMaxDigitCountFromBitSize(bitSize);

        // Start MSD radix sort from the most significant digit
        MSDSort(s, temp, 0, s.Length, digitCount - 1, bitSize, bucketOffsets);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void MSDSort<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> temp, int start, int length, int digit, int bitSize, Span<int> bucketOffsets)
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

        // Calculate divisor for current digit position
        var divisor = 1UL;
        for (var i = 0; i < digit; i++)
        {
            divisor *= 10;
        }

        // Clear bucket counts
        bucketOffsets.Clear();

        // Count occurrences of each digit in the current range
        for (var i = 0; i < length; i++)
        {
            var value = s.Read(start + i);
            var key = GetUnsignedKey(value, bitSize);
            var digitValue = (int)((key / divisor) % 10);
            bucketOffsets[digitValue + 1]++;
        }

        // Calculate prefix sum and save bucket start positions
        Span<int> bucketStarts = stackalloc int[RadixBase];
        bucketStarts[0] = 0;
        for (var i = 1; i <= RadixBase; i++)
        {
            bucketOffsets[i] += bucketOffsets[i - 1];
            if (i < RadixBase)
            {
                bucketStarts[i] = bucketOffsets[i];
            }
        }

        // Distribute elements into temp buffer based on current digit
        for (var i = 0; i < length; i++)
        {
            var value = s.Read(start + i);
            var key = GetUnsignedKey(value, bitSize);
            var digitValue = (int)((key / divisor) % 10);
            var destIndex = bucketOffsets[digitValue]++;
            temp.Write(start + destIndex, value);
        }

        // Copy back from temp to source
        temp.CopyTo(start, s, start, length);

        // Recursively sort each bucket for the next digit
        for (var i = 0; i < RadixBase; i++)
        {
            var bucketStart = bucketStarts[i];
            var bucketEnd = (i == RadixBase - 1) ? length : bucketStarts[i + 1];
            var bucketLength = bucketEnd - bucketStart;

            if (bucketLength > 1)
            {
                MSDSort(s, temp, start + bucketStart, bucketLength, digit - 1, bitSize, bucketOffsets);
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
    /// - Maintains stability
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

    /// <summary>
    /// Get the maximum number of decimal digits for a given bit size.
    /// Returns the digit count needed to represent the maximum unsigned value for that bit size.
    /// </summary>
    /// <remarks>
    /// Examples:
    /// - 8-bit: max 255 → 3 digits
    /// - 16-bit: max 65,535 → 5 digits
    /// - 32-bit: max 4,294,967,295 → 10 digits
    /// - 64-bit: max 18,446,744,073,709,551,615 → 20 digits
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetMaxDigitCountFromBitSize(int bitSize)
    {
        return bitSize switch
        {
            8 => 3,   // byte/sbyte: max 255
            16 => 5,  // short/ushort: max 65535
            32 => 10, // int/uint: max 4294967295
            64 => 20, // long/ulong: max 18446744073709551615
            _ => throw new NotSupportedException($"Bit size {bitSize} is not supported")
        };
    }
}
