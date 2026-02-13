using SortAlgorithm.Contexts;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 2^2 (4) 基数のLSD基数ソート。
/// 値をビット列として扱い、2ビットずつ（4種類）の桁に分けてバケットソートを行います。
/// 最下位桁（Least Significant Digit）から最上位桁へ向かって処理することで、安定なソートを実現します。
/// 符号付き整数は符号ビット反転により、負数も含めて正しくソートされます。
/// <br/>
/// LSD Radix Sort with radix 2^2 (4).
/// Treats values as bit sequences, dividing them into 2-bit digits (4 buckets) and performing bucket sort for each digit.
/// Processing from the Least Significant Digit to the most significant ensures a stable sort.
/// Signed integers are handled via sign-bit flipping to maintain correct ordering including negative values.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct LSD Radix Sort (Base-4):</strong></para>
/// <list type="number">
/// <item><description><strong>Sign-Bit Flipping for Signed Integers:</strong> For signed types, the sign bit is flipped to convert signed values to unsigned keys:
/// - 32-bit: key = (uint)value ^ 0x8000_0000
/// - 64-bit: key = (ulong)value ^ 0x8000_0000_0000_0000
/// This ensures negative values are ordered correctly before positive values without separate processing.
/// This technique avoids the MinValue overflow issue with Abs() and maintains stability.</description></item>
/// <item><description><strong>Digit Extraction Correctness:</strong> For each digit position d (from 0 to digitCount-1), extract the d-th 2-bit digit using bitwise operations:
/// digit = (key >> (d × 2)) &amp; 0b11. This ensures each 2-bit segment of the integer is processed independently.</description></item>
/// <item><description><strong>Stable Distribution (Counting Sort per Digit):</strong> Within each digit pass, elements are distributed into 4 buckets (0-3) based on the current digit value.
/// The distribution must preserve the relative order of elements with the same digit value (stable). This is achieved by processing elements in forward order and appending to buckets.</description></item>
/// <item><description><strong>LSD Processing Order:</strong> Digits must be processed from least significant (d=0) to most significant (d=digitCount-1).
/// This bottom-up approach ensures that after processing digit d, all digits 0 through d are correctly sorted, with stability maintained by previous passes.</description></item>
/// <item><description><strong>Digit Count Determination:</strong> The number of passes (digitCount) must cover all significant bits of the type.
/// digitCount = ⌈bitSize / 2⌉ where bitSize is the bit width of type T (8, 16, 32, or 64 bits).</description></item>
/// <item><description><strong>Bucket Collection Order:</strong> After distributing elements for a digit, buckets must be collected in ascending order (bucket 0, 1, 2, 3).
/// Due to sign-bit flipping, negative values naturally sort before positive values.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution (Radix Sort, LSD variant)</description></item>
/// <item><description>Stable      : Yes (maintains relative order of elements with equal keys)</description></item>
/// <item><description>In-place    : No (O(n) auxiliary space for temporary buffer)</description></item>
/// <item><description>Best case   : Θ(d × n) - d = ⌈bitSize/2⌉ is constant for fixed-width integers</description></item>
/// <item><description>Average case: Θ(d × n) - Linear in input size, independent of value distribution</description></item>
/// <item><description>Worst case  : Θ(d × n) - Same complexity regardless of input order</description></item>
/// <item><description>Comparisons : 0 (Non-comparison sort, uses bitwise operations only)</description></item>
/// <item><description>Digit Passes: d = ⌈bitSize/2⌉ (4 for byte, 8 for short, 16 for int, 32 for long)</description></item>
/// <item><description>Reads       : d × n (one read per element per digit pass)</description></item>
/// <item><description>Writes      : d × n (one write per element per digit pass)</description></item>
/// <item><description>Memory      : O(n) for temporary buffer</description></item>
/// </list>
/// <para><strong>Radix-4 Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>More passes than radix-256: 16 passes for 32-bit vs 4 passes for radix-256</description></item>
/// <item><description>Smaller bucket size: 4 buckets fit entirely in CPU registers/L1 cache</description></item>
/// <item><description>Simple bit operations: Only 2-bit shift and mask (&amp; 0b11)</description></item>
/// <item><description>Good for educational purposes: Shows radix sort with minimal buckets</description></item>
/// <item><description>Trade-off: More passes vs simpler bucket management</description></item>
/// </list>
/// <para><strong>Supported Types:</strong></para>
/// <list type="bullet">
/// <item><description><strong>Supported:</strong> byte, sbyte, short, ushort, int, uint, long, ulong, nint, nuint (up to 64-bit)</description></item>
/// <item><description><strong>Not Supported:</strong> Int128, UInt128, BigInteger (&lt;64-bit types)</description></item>
/// </list>
/// <para><strong>Why Int128/UInt128 are not supported:</strong></para>
/// <para>This implementation uses ulong (64-bit) as the key type for radix sorting. Supporting 128-bit types would require
/// UInt128 keys and significantly more complex logic. Since 128-bit integer sorting is a rare use case (mainly cryptography
/// and scientific computing), we intentionally limit support to 64-bit and below for simplicity and performance.
/// If you need to sort Int128/UInt128, consider implementing a custom comparer with Array.Sort or similar.</para>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Radix_sort#Least_significant_digit</para>
/// </remarks>
public static class RadixLSD4Sort
{
    private const int RadixBits = 2;        // 2 bits per digit
    private const int RadixSize = 4;        // 2^2 = 4 buckets

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Temporary buffer for digit redistribution

    /// <summary>
    /// Sorts the elements in the specified span using a key selector function.
    /// </summary>
    public static void Sort<T>(Span<T> span) where T : IBinaryInteger<T>, IMinMaxValue<T>
    {
        Sort(span, Comparer<T>.Default, NullContext.Default);
    }

    /// <summary>
    /// Sorts the elements in the specified span using a key selector function and sort context.
    /// </summary>
    public static void Sort<T>(Span<T> span, ISortContext context) where T : IBinaryInteger<T>, IMinMaxValue<T>
    {
        Sort(span, Comparer<T>.Default, context);
    }

    /// <summary>
    /// Sorts the elements in the specified span using the provided comparer and sort context.
    /// </summary>
    public static void Sort<T, TComparer>(Span<T> span, TComparer comparer, ISortContext context) where T : IBinaryInteger<T>, IMinMaxValue<T> where TComparer : IComparer<T>
    {
        if (span.Length <= 1) return;

        // Rent buffers from ArrayPool
        var tempArray = ArrayPool<T>.Shared.Rent(span.Length);
        var bucketOffsetsArray = ArrayPool<int>.Shared.Rent(RadixSize + 1);

        try
        {
            var tempBuffer = tempArray.AsSpan(0, span.Length);
            var bucketOffsets = bucketOffsetsArray.AsSpan(0, RadixSize + 1);

            SortCore<T, TComparer>(span, tempBuffer, bucketOffsets, comparer, context);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(tempArray, clearArray: true);
            ArrayPool<int>.Shared.Return(bucketOffsetsArray);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SortCore<T, TComparer>(Span<T> span, Span<T> tempBuffer, Span<int> bucketOffsets, TComparer comparer, ISortContext context)
        where T : IBinaryInteger<T>, IMinMaxValue<T> where TComparer : IComparer<T>
    {
        var s = new SortSpan<T, TComparer>(span, context, comparer, BUFFER_MAIN);
        var temp = new SortSpan<T, TComparer>(tempBuffer, context, comparer, BUFFER_TEMP);

        // Determine the number of digits based on type size
        // GetBitSize throws NotSupportedException for unsupported types (>64-bit)
        var bitSize = GetBitSize<T>();

        // Find min and max to determine actual required passes
        // This optimization skips unnecessary high-order digit passes
        var minKey = ulong.MaxValue;
        var maxKey = ulong.MinValue;

        for (var i = 0; i < s.Length; i++)
        {
            var value = s.Read(i);
            var key = GetUnsignedKey(value, bitSize);
            if (key < minKey) minKey = key;
            if (key > maxKey) maxKey = key;
        }

        // Calculate required number of passes based on the range
        // XOR to find differing bits, then count bits needed
        var range = maxKey ^ minKey;
        var requiredBits = range == 0 ? 0 : (64 - System.Numerics.BitOperations.LeadingZeroCount(range));
        var digitCount = Math.Max(1, (requiredBits + RadixBits - 1) / RadixBits);

        // Start LSD radix sort from the least significant digit
        LSDSort(s, temp, digitCount, bitSize, bucketOffsets);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void LSDSort<T, TComparer>(SortSpan<T, TComparer> s, SortSpan<T, TComparer> temp, int digitCount, int bitSize, Span<int> bucketOffsets)
        where T : IBinaryInteger<T>, IMinMaxValue<T> where TComparer : IComparer<T>
    {
        // Perform LSD radix sort (only required passes)
        for (int d = 0; d < digitCount; d++)
        {
            var shift = d * RadixBits;

            // Clear bucket offsets
            bucketOffsets.Clear();

            // Count occurrences of each digit
            for (var i = 0; i < s.Length; i++)
            {
                var value = s.Read(i);
                var key = GetUnsignedKey(value, bitSize);
                var digit = (int)((key >> shift) & 0b11);  // Extract 2-bit digit
                bucketOffsets[digit + 1]++;
            }

            // Calculate cumulative offsets (prefix sum)
            for (var i = 1; i <= RadixSize; i++)
            {
                bucketOffsets[i] += bucketOffsets[i - 1];
            }

            // Distribute elements into temp buffer based on current digit
            for (var i = 0; i < s.Length; i++)
            {
                var value = s.Read(i);
                var key = GetUnsignedKey(value, bitSize);
                var digit = (int)((key >> shift) & 0b11);  // Extract 2-bit digit
                var destIndex = bucketOffsets[digit]++;
                temp.Write(destIndex, value);
            }

            // Copy back from temp to source using CopyTo for efficiency
            temp.CopyTo(0, s, 0, s.Length);
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
}
