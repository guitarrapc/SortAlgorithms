using SortAlgorithm.Contexts;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 2^8 (256) 基数のLSD基数ソート。
/// 値をビット列として扱い、8ビットずつ（256種類）の桁に分けてバケットソートを行います。
/// 最下位桁（Least Significant Digit）から最上位桁へ向かって処理することで、安定なソートを実現します。
/// 符号付き整数は符号ビット反転により、負数も含めて正しくソートされます。
/// <br/>
/// LSD Radix Sort with radix 2^8 (256).
/// Treats values as bit sequences, dividing them into 8-bit digits (256 buckets) and performing bucket sort for each digit.
/// Processing from the Least Significant Digit to the most significant ensures a stable sort.
/// Signed integers are handled via sign-bit flipping to maintain correct ordering including negative values.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct LSD Radix Sort (Base-256):</strong></para>
/// <list type="number">
/// <item><description><strong>Order-Preserving Key Mapping:</strong> Elements are mapped to fixed-width unsigned keys through
/// <see cref="IRadixKeySelector{T}"/>. Signed integers flip the sign bit (e.g. 32-bit: key = (uint)value ^ 0x8000_0000),
/// floating-point values use the IEEE 754 total-order bit transform, and key-selector overloads extract an int key from arbitrary elements.
/// This ensures ordering correctness without separate sign handling and avoids the MinValue overflow issue with Abs().</description></item>
/// <item><description><strong>Digit Extraction Correctness:</strong> For each digit position d (from 0 to digitCount-1), extract the d-th 8-bit digit using bitwise operations:
/// digit = (key >> (d × 8)) &amp; 0xFF. This ensures each byte of the integer is processed independently.</description></item>
/// <item><description><strong>Stable Distribution (Counting Sort per Digit):</strong> Within each digit pass, elements are distributed into 256 buckets (0-255) based on the current digit value.
/// The distribution must preserve the relative order of elements with the same digit value (stable). This is achieved by processing elements in forward order and appending to buckets.</description></item>
/// <item><description><strong>LSD Processing Order:</strong> Digits must be processed from least significant (d=0) to most significant (d=digitCount-1).
/// This bottom-up approach ensures that after processing digit d, all digits 0 through d are correctly sorted, with stability maintained by previous passes.</description></item>
/// <item><description><strong>Digit Count Determination with Early Termination:</strong> The number of passes (digitCount) is determined by the actual range of values, not the full bit width.
/// digitCount = ⌈requiredBits / 8⌉ where requiredBits is calculated from (max XOR min) to find differing bits.
/// This optimization skips unnecessary high-order digit passes when the value range is small. When all elements are equal (range == 0), sorting is skipped entirely.</description></item>
/// <item><description><strong>Bucket Collection Order:</strong> After distributing elements for a digit, buckets must be collected in ascending order (bucket 0, 1, 2, ..., 255).
/// Due to sign-bit flipping, negative values naturally sort before positive values.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution (Radix Sort, LSD variant)</description></item>
/// <item><description>Stable      : Yes (maintains relative order of elements with equal keys)</description></item>
/// <item><description>In-place    : No (O(n) auxiliary space for temporary buffer)</description></item>
/// <item><description>Best case   : Θ(n) - When all elements are identical (early termination on range == 0)</description></item>
/// <item><description>Average case: Θ(d × n) - Linear in input size, where d depends on actual value range</description></item>
/// <item><description>Worst case  : Θ(d × n) - Same complexity regardless of input order, d = ⌈keyBits/8⌉ for full range</description></item>
/// <item><description>Comparisons : 0 (Non-comparison sort, uses bitwise operations only)</description></item>
/// <item><description>Digit Passes: d = ⌈requiredBits/8⌉ (early termination based on actual value range, not full bit width)</description></item>
/// <item><description>Reads       : n (initial min/max scan) + d × n (one read per distribute pass) + optional final copy</description></item>
/// <item><description>Writes      : d × n (one write per distribute pass to temp) + optional final copy</description></item>
/// <item><description>Memory      : O(n) for temporary buffer</description></item>
/// </list>
/// <para><strong>Radix-256 Advantages:</strong></para>
/// <list type="bullet">
/// <item><description>Fewer passes than radix-10: 4 passes for 32-bit vs 10 passes for decimal</description></item>
/// <item><description>Efficient bit operations: Shift and mask are faster than division/modulo</description></item>
/// <item><description>Cache-friendly bucket size: 256 buckets fit well in L1/L2 cache</description></item>
/// <item><description>Sign-bit flip handles signed integers without separate negative/positive processing</description></item>
/// <item><description>Stable sort: Maintains relative order of equal elements</description></item>
/// </list>
/// <para><strong>Supported Key Mappings (via <see cref="IRadixKeySelector{T}"/>):</strong></para>
/// <list type="bullet">
/// <item><description><strong>Integers:</strong> byte, sbyte, short, ushort, int, uint, long, ulong, nint, nuint (up to 64-bit); Int128/UInt128/BigInteger are rejected (64-bit key ceiling, see below)</description></item>
/// <item><description><strong>Floating point:</strong> Half, float, double via IEEE 754 total-order key transform (all NaN values sort first, matching <see cref="IComparable{T}"/> semantics)</description></item>
/// <item><description><strong>Key selector:</strong> arbitrary element types via an extracted <c>int</c> key; equal keys retain input order, making stability observable</description></item>
/// </list>
/// <para><strong>Why 128-bit Types Are Not Supported:</strong></para>
/// <list type="bullet">
/// <item><description><strong>Key Storage Limitation:</strong> Keys are stored as <c>ulong</c> (64-bit).
/// Supporting 128-bit would require <c>UInt128</c> keys, significantly increasing memory usage and complexity.</description></item>
/// <item><description><strong>Performance Trade-offs:</strong> 128-bit operations are significantly slower than 64-bit on most architectures,
/// negating the performance benefits of radix sort.</description></item>
/// <item><description><strong>Practical Rarity:</strong> Sorting 128-bit integers is uncommon in typical applications.
/// For such cases, comparison-based sorts (e.g., QuickSort, MergeSort) remain practical alternatives.</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Radix_sort#Least_significant_digit</para>
/// </remarks>
public static class RadixLSD256Sort
{
    private const int RadixBits = 8;        // 8 bits per digit
    private const int RadixSize = 256;      // 2^8 = 256 buckets

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Temporary buffer for digit redistribution

    /// <summary>
    /// Sorts the elements in the specified span.
    /// Uses NullContext for zero-overhead fast path.
    /// </summary>
    /// <typeparam name="T"> The type of elements to sort. Must be a binary integer type with defined min/max values.</typeparam>
    /// <param name="span"> The span of elements to sort.</param>
    public static void Sort<T>(Span<T> span) where T : IBinaryInteger<T>, IMinMaxValue<T>
        => Sort(span, NullContext.Default);

    /// <summary>
    /// Sorts the elements in the specified span.
    /// </summary>
    /// <typeparam name="T"> The type of elements to sort. Must be a binary integer type with defined min/max values.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="span"> The span of elements to sort.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation.</param>
    /// <exception cref="NotSupportedException">
    /// Thrown when <typeparamref name="T"/> is a 128-bit type (<see cref="Int128"/> or <see cref="UInt128"/>).
    /// This implementation only supports integer types up to 64-bit due to key storage and performance constraints.
    /// See class-level remarks for detailed explanation of this limitation.
    /// </exception>
    public static void Sort<T, TContext>(Span<T> span, TContext context)
        where T : IBinaryInteger<T>, IMinMaxValue<T>
        where TContext : ISortContext
        => SortCore(span, default(BinaryIntegerRadixKey<T>), new ComparableComparer<T>(), context);

    /// <summary>
    /// Sorts the elements in the specified span by an integer key extracted with <paramref name="keySelector"/>.
    /// Elements with equal keys retain their relative input order (stable).
    /// Uses NullContext for zero-overhead fast path.
    /// </summary>
    /// <typeparam name="T">The type of elements to sort.</typeparam>
    /// <param name="span">The span of elements to sort.</param>
    /// <param name="keySelector">Extracts the integer sort key from an element. Must be pure and consistent per element.</param>
    public static void Sort<T>(Span<T> span, Func<T, int> keySelector)
    {
        ArgumentNullException.ThrowIfNull(keySelector);
        var selector = new FuncRadixKeySelector<T>(keySelector);
        SortCore(span, selector, new RadixKeyComparer<T, FuncRadixKeySelector<T>>(selector), NullContext.Default);
    }

    /// <summary>
    /// Sorts the elements in the specified span by an integer key extracted with <paramref name="keySelector"/>.
    /// Elements with equal keys retain their relative input order (stable).
    /// </summary>
    /// <typeparam name="T">The type of elements to sort.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="span">The span of elements to sort.</param>
    /// <param name="keySelector">Extracts the integer sort key from an element. Must be pure and consistent per element.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation.</param>
    public static void Sort<T, TContext>(Span<T> span, Func<T, int> keySelector, TContext context)
        where TContext : ISortContext
    {
        ArgumentNullException.ThrowIfNull(keySelector);
        var selector = new FuncRadixKeySelector<T>(keySelector);
        SortCore(span, selector, new RadixKeyComparer<T, FuncRadixKeySelector<T>>(selector), context);
    }

    /// <summary>
    /// Sorts <see cref="Half"/> values via the IEEE 754 total-order key transform.
    /// All NaN values sort first, matching <see cref="IComparable{T}"/> semantics.
    /// </summary>
    public static void Sort(Span<Half> span)
        => SortCore(span, default(HalfRadixKey), new ComparableComparer<Half>(), NullContext.Default);

    /// <inheritdoc cref="Sort(Span{Half})"/>
    public static void Sort<TContext>(Span<Half> span, TContext context) where TContext : ISortContext
        => SortCore(span, default(HalfRadixKey), new ComparableComparer<Half>(), context);

    /// <summary>
    /// Sorts <see cref="float"/> values via the IEEE 754 total-order key transform.
    /// All NaN values sort first, matching <see cref="IComparable{T}"/> semantics.
    /// </summary>
    public static void Sort(Span<float> span)
        => SortCore(span, default(SingleRadixKey), new ComparableComparer<float>(), NullContext.Default);

    /// <inheritdoc cref="Sort(Span{float})"/>
    public static void Sort<TContext>(Span<float> span, TContext context) where TContext : ISortContext
        => SortCore(span, default(SingleRadixKey), new ComparableComparer<float>(), context);

    /// <summary>
    /// Sorts <see cref="double"/> values via the IEEE 754 total-order key transform.
    /// All NaN values sort first, matching <see cref="IComparable{T}"/> semantics.
    /// </summary>
    public static void Sort(Span<double> span)
        => SortCore(span, default(DoubleRadixKey), new ComparableComparer<double>(), NullContext.Default);

    /// <inheritdoc cref="Sort(Span{double})"/>
    public static void Sort<TContext>(Span<double> span, TContext context) where TContext : ISortContext
        => SortCore(span, default(DoubleRadixKey), new ComparableComparer<double>(), context);

    private static void SortCore<T, TRadixKey, TComparer, TContext>(Span<T> span, TRadixKey radixKey, TComparer comparer, TContext context)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;

        // Validate the key width up front (BinaryIntegerRadixKey throws NotSupportedException for >64-bit types)
        _ = TRadixKey.KeyBits;

        // Rent buffers from ArrayPool
        var tempArray = ArrayPool<T>.Shared.Rent(span.Length);
        var bucketOffsetsArray = ArrayPool<int>.Shared.Rent(RadixSize + 1);

        try
        {
            var tempBuffer = tempArray.AsSpan(0, span.Length);
            var bucketOffsets = bucketOffsetsArray.AsSpan(0, RadixSize + 1);
            var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
            var temp = new SortSpan<T, TComparer, TContext>(tempBuffer, context, comparer, BUFFER_TEMP);

            // Find min and max to determine actual required passes
            // This optimization skips unnecessary high-order digit passes
            var minKey = ulong.MaxValue;
            var maxKey = ulong.MinValue;

            for (var i = 0; i < s.Length; i++)
            {
                var value = s.Read(i);
                var key = radixKey.GetKey(value);
                if (key < minKey) minKey = key;
                if (key > maxKey) maxKey = key;
            }

            // Calculate required number of passes based on the range
            // XOR to find differing bits, then count bits needed
            var range = maxKey ^ minKey;

            // Early return if all elements are equal (range == 0)
            if (range == 0) return;

            var requiredBits = 64 - System.Numerics.BitOperations.LeadingZeroCount(range);
            var digitCount = (requiredBits + RadixBits - 1) / RadixBits;

            // Start LSD radix sort from the least significant digit
            LSDSort(s, temp, radixKey, digitCount, bucketOffsets);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(tempArray, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
            ArrayPool<int>.Shared.Return(bucketOffsetsArray);
        }
    }

    private static void LSDSort<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> temp, TRadixKey radixKey, int digitCount, Span<int> bucketOffsets)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var src = s;
        var dst = temp;

        // Perform LSD radix sort with ping-pong buffers
        for (int d = 0; d < digitCount; d++)
        {
            src.Context.OnPhase(SortPhase.RadixPass, d, digitCount);
            var shift = d * RadixBits;

            // Clear bucket offsets
            // bucketOffsets[0..RadixSize] stores bucket boundaries:
            // - Initially: bucketOffsets[digit+1] = count of elements with 'digit'
            // - After prefix sum: bucketOffsets[digit] = start index for 'digit' bucket
            // - During distribution: bucketOffsets[digit]++ tracks next write position
            bucketOffsets.Clear();

            // Count occurrences of each digit (store count in digit+1 position)
            for (var i = 0; i < src.Length; i++)
            {
                var value = src.Read(i);
                var key = radixKey.GetKey(value);
                var digit = (int)((key >> shift) & 0xFF);
                bucketOffsets[digit + 1]++;
            }

            // Calculate cumulative offsets (prefix sum)
            // After this: bucketOffsets[digit] = start index for bucket 'digit'
            for (var i = 1; i <= RadixSize; i++)
            {
                bucketOffsets[i] += bucketOffsets[i - 1];
            }

            // Distribute elements from src to dst based on current digit
            for (var i = 0; i < src.Length; i++)
            {
                var value = src.Read(i);
                var key = radixKey.GetKey(value);
                var digit = (int)((key >> shift) & 0xFF);
                var destIndex = bucketOffsets[digit]++;
                dst.Write(destIndex, value);
            }

            // Swap src/dst for next pass (ping-pong)
            var tempSortSpan = src;
            src = dst;
            dst = tempSortSpan;
        }

        // After digitCount swaps, if digitCount is odd, final data is in src (which points to temp buffer after odd swaps)
        // Pass 0: s→temp, swap (src=temp), Pass 1: temp→s, swap (src=s), ...
        if ((digitCount & 1) == 1)
        {
            src.CopyTo(0, s, 0, s.Length);
        }
    }
}
