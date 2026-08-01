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
/// <item><description><strong>Complete Pass Coverage:</strong> Must perform d passes where d is the number of decimal digits of (max − min),
/// the keys having been normalized by the minimum key. Incomplete passes result in partially sorted arrays.</description></item>
/// <item><description><strong>Identity Passes Are Skippable:</strong> If every element shares the same value at digit d, the stable
/// distribution for that digit writes them out in the order they already are — the pass is the identity and can be skipped
/// without affecting the result, so "complete coverage" above means every digit is examined, not that every digit is distributed.
/// This is orthogonal to the range-derived digit count, which only trims uniform digits above the most significant one; a digit
/// anywhere below it can be uniform too (every value a multiple of 1000, say). Skipping changes which buffer holds the data, so
/// the final copy is decided by the number of passes actually executed.</description></item>
/// <item><description><strong>Order-Preserving Key Mapping:</strong> Elements are mapped to fixed-width unsigned keys through
/// <see cref="IRadixKeySelector{T}"/>. Signed integers flip the sign bit (e.g. 32-bit: key = (uint)value ^ 0x8000_0000),
/// floating-point values use the IEEE 754 total-order bit transform, and key-selector overloads extract an int key from arbitrary elements.
/// This ensures ordering correctness without separate sign handling and avoids the MinValue overflow issue with Abs().</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution (Non-comparison based)</description></item>
/// <item><description>Stable      : Yes (insertion order preserved in buckets)</description></item>
/// <item><description>In-place    : No (O(n) auxiliary space for the temporary buffer, plus d × 11 stack counters)</description></item>
/// <item><description>Best case   : Θ(n) - When all elements are identical (early termination on range == 0)</description></item>
/// <item><description>Average case: Θ(d × n) - Linear in input size, independent of value distribution</description></item>
/// <item><description>Worst case  : Θ(d × n) - Performance depends on digit count, not comparisons</description></item>
/// <item><description>Comparisons : 0 (Non-comparison sort; uses only arithmetic operations)</description></item>
/// <item><description>Swaps       : 0 (Elements moved via bucket redistribution, not swaps)</description></item>
/// <item><description>Writes      : e × n (one write per executed distribute pass) + optional final copy, where e ≤ d is the number of non-identity digits</description></item>
/// <item><description>Reads       : 2 × n preprocessing (min/max scan, then one pass building every digit's histogram) + e × n (one read per executed distribute pass) + optional final copy</description></item>
/// <item><description>Memory      : O(n) for temporary buffer + d × 11 stack counters</description></item>
/// </list>
/// <para><strong>Note:</strong> Uses decimal arithmetic (division and modulo), which may be slower than binary-based radix sorts (e.g., RadixLSD4Sort with bit shifts).
/// However, it is more intuitive for understanding and debugging.</para>
/// <para><strong>Why the Distribute Pass Still Divides by a Run-Time Divisor:</strong></para>
/// <para>Extracting the d-th digit as (key / 10^d) % 10 divides by a divisor only known at run time, which is the
/// one arithmetic operation on this path the hardware is bad at. It can be removed: carry each element's running
/// quotient beside it, and its digit becomes a remainder, with the next pass continuing from quotient / 10 —
/// every division then by a literal 10, which the JIT lowers to a multiply and a shift. That was implemented and
/// measured against this version, and it is about 2.2× slower at n = 8192 (3 to 10 decimal digits), though only
/// ~10% slower at n = 1024 where every buffer still fits in L1. The quotient has to be permuted alongside its
/// element, so the distribute loop gains a second scattered write, of 8 bytes, into a second array — twice the
/// randomly-addressed write streams. The division is real but it is not what this sort is bound by.</para>
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

    // Per-digit slot count in the histogram block. The extra leading slot lets a pass turn its counts
    // into start offsets with an in-place prefix sum (count for 'digit' lives at [digit + 1]).
    private const int HistogramStride = RadixBase + 1;

    // Decimal digits of ulong.MaxValue, the widest key range a pass count can be derived from.
    private const int MaxDigits = 20;

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

            // One histogram block per digit, all of them stack-resident (20 × 11 ints = 880 bytes).
            Span<int> allHistograms = stackalloc int[MaxDigits * HistogramStride];
            var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
            var temp = new SortSpan<T, TComparer, TContext>(tempBuffer, context, comparer, BUFFER_TEMP);

            // Announce the range scan: without it a consumer sees n reads with no phase attached, and the
            // label from whatever ran before stays on screen through the whole scan. KeyRangeScan rather than
            // DistributionCount: this measures the keys, it does not tally per-value occurrences.
            s.Context.OnPhase(SortPhase.KeyRangeScan);

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

            // Every digit's histogram is built in this one pass over the elements, so a digit pass
            // reads each element exactly once (to distribute it) instead of twice. Counting per pass
            // would re-read the whole span d times to learn something the key already carries: the key
            // holds all d digits at once, and peeling them off with a running /= 10 divides by a
            // constant, which costs no hardware division at all.
            //
            // The distribute pass below still divides by pow10[d], whose divisor is only known at run
            // time. Removing that too — by carrying each element's running quotient through the passes
            // so its digit is just a remainder — was implemented and measured: it makes this sort about
            // 2.2x slower at n = 8192 (and ~10% at n = 1024, where everything fits in L1). The quotient
            // has to be permuted alongside its element, so the distribute loop gains a second scattered
            // write, of 8 bytes, into a second array; that costs far more than the division it removes.
            //
            // Layout: digit d owns histograms[d * Stride .. (d + 1) * Stride), counted at [digit + 1]
            // so the prefix sum in each pass can run in place (see LSDSort).
            var histograms = allHistograms[..(digitCount * HistogramStride)];
            histograms.Clear();

            for (var i = 0; i < s.Length; i++)
            {
                var quotient = radixKey.GetKey(s.Read(i)) - minKey;
                var block = 0;
                for (var d = 0; d < digitCount; d++)
                {
                    histograms[block + (int)(quotient % RadixBase) + 1]++;
                    quotient /= RadixBase;
                    block += HistogramStride;
                }
            }

            // Start LSD radix sort from the least significant digit
            LSDSort(s, temp, radixKey, digitCount, minKey, histograms, pow10);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(tempArray, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void LSDSort<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> temp, TRadixKey radixKey, int digitCount, ulong minKey, Span<int> histograms, ReadOnlySpan<ulong> pow10)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Each pass reads from src and writes the distributed result to dst. Rather than copying dst
        // back over src to set up the next pass, the two spans trade roles: a pass never has to see
        // the buffer its predecessor started from, so the copy only buys naming, not correctness.
        var src = s;
        var dst = temp;

        // Counted separately from d: a pass that turns out to be the identity is not run, and it is the
        // number actually run that decides which buffer the result ends up in.
        var executedPasses = 0;

        // Perform LSD radix sort on unsigned keys
        for (int d = 0; d < digitCount; d++)
        {
            src.Context.OnPhase(SortPhase.RadixPass, d, digitCount);
            var divisor = pow10[d];

            // This digit's histogram, already counted by the caller's single pass:
            // - On entry: bucketOffsets[digit+1] = count of elements with 'digit'
            // - After prefix sum: bucketOffsets[digit] = start index for 'digit' bucket
            // - During distribution: bucketOffsets[digit]++ tracks next write position
            var bucketOffsets = histograms.Slice(d * HistogramStride, HistogramStride);

            // If one bucket holds every element, this digit sorts nothing: a stable distribution over a
            // single bucket writes the elements out in the order they already are, so the pass is the
            // identity and can be skipped. Deriving digitCount from the key range cannot catch this —
            // that only trims uniform digits above the most significant one, while a digit anywhere
            // below it can be uniform too (every value a multiple of 1000, say). Here the answer is
            // free: the counts were all built up front, before any pass ran.
            if (IsSingleBucket(bucketOffsets, src.Length)) continue;

            // Calculate cumulative bucket positions (prefix sum)
            for (var i = 1; i <= RadixBase; i++)
            {
                bucketOffsets[i] += bucketOffsets[i - 1];
            }

            // Distribute elements into the destination buffer based on current digit
            // Use (key - minKey) to normalize the range, extracting only the necessary digits
            for (var i = 0; i < src.Length; i++)
            {
                var value = src.Read(i);
                var normalizedKey = radixKey.GetKey(value) - minKey;
                var digit = (int)((normalizedKey / divisor) % RadixBase);
                var pos = bucketOffsets[digit]++;
                dst.Write(pos, value);
            }

            // Swap src/dst for next pass (ping-pong)
            var tempSortSpan = src;
            src = dst;
            dst = tempSortSpan;
            executedPasses++;
        }

        // Each executed pass moves the data to the other buffer, so an odd number of them leaves it in
        // temp. Skipped passes move nothing, which is exactly why the parity is taken from the passes
        // that ran rather than from digitCount.
        // Pass 0: s→temp, swap (src=temp), Pass 1: temp→s, swap (src=s), ...
        if ((executedPasses & 1) == 1)
        {
            src.CopyTo(0, s, 0, s.Length);
        }
    }

    /// <summary>
    /// True when a single bucket holds all <paramref name="length"/> elements, i.e. every element shares
    /// this digit. Counts are still in their pre-prefix-sum layout, at <c>bucketOffsets[digit + 1]</c>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSingleBucket(ReadOnlySpan<int> bucketOffsets, int length)
    {
        // The first bucket holding anything settles it: if it does not hold everything, some other
        // bucket holds the rest. So the scan stops at the first non-empty entry either way.
        foreach (var count in bucketOffsets[1..])
        {
            if (count != 0) return count == length;
        }
        return false;
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
