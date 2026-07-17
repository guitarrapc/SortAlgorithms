using SortAlgorithm.Contexts;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 2^2 (4) 基数のMSD基数ソート。
/// 値をビット列として扱い、2ビットずつ（4種類）の桁に分けてバケットソートを行います。
/// 最上位桁（Most Significant Digit）から最下位桁へ向かって処理することで、再帰的にソートを実現します。
/// 符号付き整数は符号ビット反転により、負数も含めて正しくソートされます。
/// <br/>
/// MSD Radix Sort with radix 2^2 (4).
/// Treats values as bit sequences, dividing them into 2-bit digits (4 buckets) and performing bucket sort for each digit.
/// Processing from the Most Significant Digit to the least significant ensures a recursive sort.
/// Signed integers are handled via sign-bit flipping to maintain correct ordering including negative values.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct MSD Radix Sort (Base-4):</strong></para>
/// <list type="number">
/// <item><description><strong>Order-Preserving Key Mapping:</strong> Elements are mapped to fixed-width unsigned keys through
/// <see cref="IRadixKeySelector{T}"/>. Signed integers flip the sign bit (e.g. 32-bit: key = (uint)value ^ 0x8000_0000),
/// floating-point values use the IEEE 754 total-order bit transform, and key-selector overloads extract an int key from arbitrary elements.
/// This ensures ordering correctness without separate sign handling and avoids the MinValue overflow issue with Abs().</description></item>
/// <item><description><strong>Digit Extraction Correctness:</strong> For each digit position d (from digitCount-1 down to 0), extract the d-th 2-bit digit using bitwise operations:
/// digit = (key >> (d × 2)) &amp; 0b11. This ensures each 2-bit segment of the integer is processed independently.</description></item>
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
/// <item><description>Average case: Θ(d × n) - d = ⌈bitSize/2⌉ is constant for fixed-width integers</description></item>
/// <item><description>Worst case  : Θ(d × n) - Same complexity regardless of input order</description></item>
/// <item><description>Comparisons : 0 (Non-comparison sort, uses bitwise operations only)</description></item>
/// <item><description>Digit Passes: up to d = ⌈bitSize/2⌉ (4 for byte, 8 for short, 16 for int, 32 for long), but can terminate early</description></item>
/// <item><description>Memory      : O(n) for temporary buffer</description></item>
/// </list>
/// <para><strong>MSD vs LSD:</strong></para>
/// <list type="bullet">
/// <item><description>MSD processes high-order digits first, enabling early termination when buckets are fully sorted</description></item>
/// <item><description>MSD is cache-friendlier for partially sorted data as it localizes accesses within buckets</description></item>
/// <item><description>MSD requires recursive processing of buckets, adding overhead compared to LSD's iterative approach</description></item>
/// <item><description>Both MSD and LSD can be implemented as stable sorts (this implementation maintains stability)</description></item>
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
/// <para>Wiki: https://en.wikipedia.org/wiki/Radix_sort#Most_significant_digit</para>
/// </remarks>
public static class RadixMSD4Sort
{
    private const int RadixBits = 2;        // 2 bits per digit
    private const int RadixSize = 4;        // 2^2 = 4 buckets
    private const int InsertionSortCutoff = 16; // Switch to insertion sort for small buckets

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

        // Rent temporary buffer from ArrayPool for element redistribution
        var tempArray = ArrayPool<T>.Shared.Rent(span.Length);

        try
        {
            var tempBuffer = tempArray.AsSpan(0, span.Length);

            // The insertion-sort cutoff compares by the radix key, matching the
            // digit passes and preserving stability for equal keys.
            var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
            var temp = new SortSpan<T, TComparer, TContext>(tempBuffer, context, comparer, BUFFER_TEMP);

            // Calculate digit count from the key width (2 bits per digit)
            // MSD doesn't need to scan for min/max - empty buckets are naturally skipped
            var digitCount = (TRadixKey.KeyBits + RadixBits - 1) / RadixBits;

            // Start MSD radix sort from the most significant digit
            MSDSort(s, temp, radixKey, 0, s.Length, digitCount - 1);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(tempArray, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    private static void MSDSort<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> temp, TRadixKey radixKey, int start, int length, int digit)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Base case: if length is small, use insertion sort (key-based comparer keeps it stable)
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

        s.Context.OnPhase(SortPhase.RadixPass, digit, digit);
        var shift = digit * RadixBits;

        // Allocate bucket counts on stack (RadixSize+1 = 5 elements = 20 bytes)
        // Each recursive level gets its own bucketCounts, avoiding reuse corruption
        Span<int> bucketCounts = stackalloc int[RadixSize + 1];
        bucketCounts.Clear(); // Required: [module: SkipLocalsInit] skips zero-initialization

        // Count occurrences of each digit in the current range
        for (var i = 0; i < length; i++)
        {
            var value = s.Read(start + i);
            var key = radixKey.GetKey(value);
            var digitValue = (int)((key >> shift) & 0b11);  // Extract 2-bit digit
            bucketCounts[digitValue + 1]++;
        }

        // Calculate prefix sum and save bucket start positions in one pass
        // RadixSize=4 is small enough for stackalloc (16 bytes)
        Span<int> bucketStarts = stackalloc int[RadixSize];
        bucketStarts[0] = 0; // First bucket always starts at offset 0
        for (var i = 1; i <= RadixSize; i++)
        {
            bucketCounts[i] += bucketCounts[i - 1];
            if (i < RadixSize)
            {
                bucketStarts[i] = bucketCounts[i];
            }
        }

        // Distribute elements into temp buffer based on current digit
        // Make a copy of bucketCounts for the scatter phase since we modify it
        Span<int> bucketOffsets = stackalloc int[RadixSize + 1];
        bucketCounts.CopyTo(bucketOffsets);

        for (var i = 0; i < length; i++)
        {
            var value = s.Read(start + i);
            var key = radixKey.GetKey(value);
            var digitValue = (int)((key >> shift) & 0b11);  // Extract 2-bit digit
            var destIndex = bucketOffsets[digitValue]++;
            temp.Write(start + destIndex, value);
        }

        // Copy back from temp to source
        temp.CopyTo(start, s, start, length);

        // Recursively sort each bucket for the next digit
        for (var i = 0; i < RadixSize; i++)
        {
            var bucketStart = bucketStarts[i];
            var bucketEnd = (i == RadixSize - 1) ? length : bucketStarts[i + 1];
            var bucketLength = bucketEnd - bucketStart;

            if (bucketLength > 1)
            {
                MSDSort(s, temp, radixKey, start + bucketStart, bucketLength, digit - 1);
            }
        }
    }
}
