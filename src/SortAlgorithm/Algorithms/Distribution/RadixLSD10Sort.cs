using SortAlgorithm.Contexts;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 10進数基数のLSD（Least Significant Digit）基数ソート。
/// 値を10進数の桁として扱い、最下位桁から最上位桁まで順に安定なバケットソートを繰り返します。
/// 人間が理解しやすい10進数ベースのアルゴリズムで、デバッグや教育目的に適しています。
/// <br/>
/// Decimal-based LSD (Least Significant Digit) radix sort.
/// Treats values as decimal digits and performs stable bucket sorting repeatedly from the least significant digit to the most significant digit.
/// This decimal-based algorithm is easy for humans to understand and is suitable for debugging and educational purposes.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct LSD Radix Sort (Decimal Base):</strong></para>
/// <list type="number">
/// <item><description><strong>Stable Sorting per Digit:</strong> Each pass must be stable (preserve relative order of equal keys).
/// This implementation uses counting sort to maintain insertion order, ensuring stability.</description></item>
/// <item><description><strong>Digit Extraction Consistency:</strong> For a given position, the digit must be extracted consistently across all values.
/// This uses (value / divisor) % 10 where divisor = 10^d (d = 0, 1, 2, ...).</description></item>
/// <item><description><strong>LSD Processing Order:</strong> Process digits from least significant (ones place) to most significant (highest decimal digit).
/// This ensures that lower-order digits are already sorted when processing higher-order digits.</description></item>
/// <item><description><strong>Complete Pass Coverage:</strong> Must perform d passes where d = ⌈log₁₀(max)⌉ + 1 (number of decimal digits in the maximum value).
/// Incomplete passes result in partially sorted arrays.</description></item>
/// <item><description><strong>Order-Preserving Key Mapping:</strong> Elements are mapped to fixed-width unsigned keys through
/// <see cref="IRadixKeySelector{T}"/>. Signed integers flip the sign bit (e.g. 32-bit: key = (uint)value ^ 0x8000_0000),
/// floating-point values use the IEEE 754 total-order bit transform, and key-selector overloads extract an int key from arbitrary elements.
/// This ensures ordering correctness without separate sign handling and avoids the MinValue overflow issue with Abs().</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution (Non-comparison based)</description></item>
/// <item><description>Stable      : Yes (insertion order preserved in buckets)</description></item>
/// <item><description>In-place    : No (O(n + 10) auxiliary space for buckets)</description></item>
/// <item><description>Best case   : Θ(d × n) - d = number of decimal digits (d = ⌈log₁₀(max)⌉ + 1)</description></item>
/// <item><description>Average case: Θ(d × n) - Linear in input size, independent of value distribution</description></item>
/// <item><description>Worst case  : Θ(d × n) - Performance depends on digit count, not comparisons</description></item>
/// <item><description>Comparisons : 0 (Non-comparison sort; uses only arithmetic operations)</description></item>
/// <item><description>Swaps       : 0 (Elements moved via bucket redistribution, not swaps)</description></item>
/// <item><description>Writes      : d × n (Each element written once per digit pass)</description></item>
/// <item><description>Reads       : d × n (Each element read once per digit pass)</description></item>
/// </list>
/// <para><strong>Note:</strong> Uses decimal arithmetic (division and modulo), which may be slower than binary-based radix sorts (e.g., RadixLSD4Sort with bit shifts).
/// However, it is more intuitive for understanding and debugging.</para>
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
public static class RadixLSD10Sort
{
    private const int RadixBase = 10;       // Decimal base

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;           // Main input array
    private const int BUFFER_TEMP = 1;           // Temporary buffer for digit redistribution

    /// <summary>
    /// Sorts the elements in the specified span.
    /// Uses NullContext for zero-overhead fast path.
    /// </summary>
    /// <typeparam name="T"> The type of elements to sort. Must be a binary integer type (up to 64-bit).</typeparam>
    /// <param name="span"> The span of elements to sort.</param>
    public static void Sort<T>(Span<T> span) where T : IBinaryInteger<T>
        => Sort(span, NullContext.Default);

    /// <summary>
    /// Sorts the elements in the specified span.
    /// </summary>
    /// <typeparam name="T"> The type of elements to sort. Must be a binary integer type (up to 64-bit).</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="span"> The span of elements to sort.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation.</param>
    /// <exception cref="NotSupportedException">
    /// Thrown when <typeparamref name="T"/> is a 128-bit type (<see cref="Int128"/> or <see cref="UInt128"/>).
    /// This implementation only supports integer types up to 64-bit due to key storage and performance constraints.
    /// See class-level remarks for detailed explanation of this limitation.
    /// </exception>
    public static void Sort<T, TContext>(Span<T> span, TContext context)
        where T : IBinaryInteger<T>
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
    public static void SortBy<T>(Span<T> span, Func<T, int> keySelector)
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
    public static void SortBy<T, TContext>(Span<T> span, Func<T, int> keySelector, TContext context)
        where TContext : ISortContext
    {
        ArgumentNullException.ThrowIfNull(keySelector);
        var selector = new FuncRadixKeySelector<T>(keySelector);
        SortCore(span, selector, new RadixKeyComparer<T, FuncRadixKeySelector<T>>(selector), context);
    }

    /// <summary>
    /// Sorts the elements in the specified span by keys produced with a custom
    /// <see cref="IRadixKeySelector{T}"/> implementation (full-control overload, up to 64-bit keys).
    /// Implement the selector as a readonly struct for JIT devirtualization and inlining.
    /// Elements with equal keys retain their relative input order (stable).
    /// Uses NullContext for zero-overhead fast path.
    /// </summary>
    /// <typeparam name="T">The type of elements to sort.</typeparam>
    /// <typeparam name="TRadixKey">The radix key selector type. Must be a struct implementing <see cref="IRadixKeySelector{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort.</param>
    /// <param name="radixKey">Maps an element to its order-preserving unsigned key.</param>
    public static void SortBy<T, TRadixKey>(Span<T> span, TRadixKey radixKey)
        where TRadixKey : struct, IRadixKeySelector<T>
        => SortCore(span, radixKey, new RadixKeyComparer<T, TRadixKey>(radixKey), NullContext.Default);

    /// <inheritdoc cref="SortBy{T, TRadixKey}(Span{T}, TRadixKey)"/>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation.</param>
    public static void SortBy<T, TRadixKey, TContext>(Span<T> span, TRadixKey radixKey, TContext context)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TContext : ISortContext
        => SortCore(span, radixKey, new RadixKeyComparer<T, TRadixKey>(radixKey), context);

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
        RadixKeyGuard.ValidateKeyBits<T, TRadixKey>();

        // Rent buffers from ArrayPool
        var tempArray = ArrayPool<T>.Shared.Rent(span.Length);

        try
        {
            var tempBuffer = tempArray.AsSpan(0, span.Length);

            // Use stackalloc for small fixed-size bucket counts (10 ints = 40 bytes)
            Span<int> bucketCounts = stackalloc int[RadixBase];
            var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
            var temp = new SortSpan<T, TComparer, TContext>(tempBuffer, context, comparer, BUFFER_TEMP);

            // Find min and max unsigned keys to determine required digit count
            var minKey = ulong.MaxValue;
            var maxKey = ulong.MinValue;

            for (var i = 0; i < s.Length; i++)
            {
                var value = s.Read(i);
                var key = radixKey.GetKey(value);
                if (key < minKey) minKey = key;
                if (key > maxKey) maxKey = key;
            }

            // Early exit: if all elements are the same (range == 0), no sorting needed
            if (minKey == maxKey) return;

            // Pre-computed powers of 10 for O(1) divisor lookup
            // Pow10[d] = 10^d for d in [0..19], supporting up to 20 decimal digits (ulong max)
            // This eliminates O(digit) loop in divisor calculation for each recursive call
            ReadOnlySpan<ulong> pow10 = [
                1UL,                      // 10^0
                10UL,                     // 10^1
                100UL,                    // 10^2
                1_000UL,                  // 10^3
                10_000UL,                 // 10^4
                100_000UL,                // 10^5
                1_000_000UL,              // 10^6
                10_000_000UL,             // 10^7
                100_000_000UL,            // 10^8
                1_000_000_000UL,          // 10^9
                10_000_000_000UL,         // 10^10
                100_000_000_000UL,        // 10^11
                1_000_000_000_000UL,      // 10^12
                10_000_000_000_000UL,     // 10^13
                100_000_000_000_000UL,    // 10^14
                1_000_000_000_000_000UL,  // 10^15
                10_000_000_000_000_000UL, // 10^16
                100_000_000_000_000_000UL,// 10^17
                1_000_000_000_000_000_000UL,  // 10^18
                10_000_000_000_000_000_000UL  // 10^19 (max for 20-digit ulong: 18,446,744,073,709,551,615)
            ];

            // Calculate required number of decimal digits based on the range
            // For a narrow range (e.g., 9,000,000,000 to 9,000,000,100), we only need digits to represent the range (100 → 3 digits)
            // instead of maxKey (9,000,000,100 → 10 digits), dramatically reducing passes
            var range = maxKey - minKey;
            var digitCount = GetDigitCountFromUlong(range, pow10);

            // Start LSD radix sort from the least significant digit
            LSDSort(s, temp, radixKey, digitCount, minKey, bucketCounts, pow10);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(tempArray, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void LSDSort<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> source, SortSpan<T, TComparer, TContext> temp, TRadixKey radixKey, int digitCount, ulong minKey, Span<int> bucketCounts, ReadOnlySpan<ulong> pow10)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        Span<int> bucketStarts = stackalloc int[RadixBase];

        // Perform LSD radix sort on unsigned keys
        for (int d = 0; d < digitCount; d++)
        {
            source.Context.OnPhase(SortPhase.RadixPass, d, digitCount);
            var divisor = pow10[d];

            // Clear bucket counts
            bucketCounts.Clear();

            // Count occurrences of each decimal digit
            // Use (key - minKey) to normalize the range, extracting only the necessary digits
            for (var i = 0; i < source.Length; i++)
            {
                var value = source.Read(i);
                var key = radixKey.GetKey(value);
                var normalizedKey = key - minKey;
                var digit = (int)((normalizedKey / divisor) % 10);
                bucketCounts[digit]++;
            }

            // Calculate cumulative bucket positions
            bucketStarts[0] = 0;
            for (var i = 1; i < RadixBase; i++)
            {
                bucketStarts[i] = bucketStarts[i - 1] + bucketCounts[i - 1];
            }

            // Distribute elements into temp buffer based on current digit
            for (var i = 0; i < source.Length; i++)
            {
                var value = source.Read(i);
                var key = radixKey.GetKey(value);
                var normalizedKey = key - minKey;
                var digit = (int)((normalizedKey / divisor) % 10);
                var pos = bucketStarts[digit]++;
                temp.Write(pos, value);
            }

            // Copy back from temp buffer
            temp.CopyTo(0, source, 0, source.Length);
        }
    }

    /// <summary>
    /// Get the number of decimal digits needed to represent a ulong value
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetDigitCountFromUlong(ulong value, ReadOnlySpan<ulong> pow10)
    {
        if (value == 0) return 1;

        // value < 10^1 -> 1 digit, value < 10^2 -> 2 digits, ..., value < 10^d -> d digits
        // Pow10 is 10^0...10^19
        for (int d = 1; d < pow10.Length; d++)
            if (value < pow10[d]) return d;

        return 20; // max for ulong (10^20 > 2^64)
    }
}
