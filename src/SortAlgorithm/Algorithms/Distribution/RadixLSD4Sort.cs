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
/// <item><description><strong>Order-Preserving Key Mapping:</strong> Elements are mapped to fixed-width unsigned keys through
/// <see cref="IRadixKeySelector{T}"/>. Signed integers flip the sign bit (e.g. 32-bit: key = (uint)value ^ 0x8000_0000),
/// floating-point values use the IEEE 754 bit transform (-0 and +0 tie), and key-selector overloads extract an int key from arbitrary elements.
/// This ensures ordering correctness without separate sign handling and avoids the MinValue overflow issue with Abs().</description></item>
/// <item><description><strong>Digit Extraction Correctness:</strong> For each digit position d (from 0 to digitCount-1), extract the d-th 2-bit digit using bitwise operations:
/// digit = (key >> (d × 2)) &amp; 0b11. This ensures each 2-bit segment of the key is processed independently.</description></item>
/// <item><description><strong>Stable Distribution (Counting Sort per Digit):</strong> Within each digit pass, elements are distributed into 4 buckets (0-3) based on the current digit value.
/// The distribution must preserve the relative order of elements with the same digit value (stable). This is achieved by processing elements in forward order and appending to buckets.</description></item>
/// <item><description><strong>LSD Processing Order:</strong> Digits must be processed from least significant (d=0) to most significant (d=digitCount-1).
/// This bottom-up approach ensures that after processing digit d, all digits 0 through d are correctly sorted, with stability maintained by previous passes.</description></item>
/// <item><description><strong>Digit Count Determination with Early Termination:</strong> The number of passes (digitCount) is determined by the actual range of key values, not the full key width.
/// Keys are shifted down by the minimum key, so digitCount = ⌈requiredBits / 2⌉ where requiredBits is the bit width of (max − min).
/// This optimization skips unnecessary high-order digit passes when the key range is small, and — unlike a (max XOR min) bit width —
/// it stays effective when the range straddles a high bit boundary, as sign-flipped keys of signed values around zero do.</description></item>
/// <item><description><strong>Identity Passes Are Skippable:</strong> If every element shares the same value at digit d, the stable
/// distribution for that digit writes them out in the order they already are — the pass is the identity and can be skipped
/// without affecting the result. This is orthogonal to the range-derived digit count, which only trims uniform digits above
/// the most significant one; a digit anywhere below it can be uniform too (every value a multiple of four, say). Skipping
/// changes which buffer holds the data, so the final copy is decided by the number of passes actually executed.</description></item>
/// <item><description><strong>Bucket Collection Order:</strong> After distributing elements for a digit, buckets must be collected in ascending order (bucket 0, 1, 2, 3).
/// Due to the order-preserving key mapping, negative values naturally sort before positive values.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution (Radix Sort, LSD variant)</description></item>
/// <item><description>Stable      : Yes (maintains relative order of elements with equal keys)</description></item>
/// <item><description>In-place    : No (O(n) auxiliary space for temporary buffer)</description></item>
/// <item><description>Best case   : Θ(n) - When all elements are identical (early termination on range == 0)</description></item>
/// <item><description>Average case: Θ(d × n) - Linear in input size, where d depends on actual key range</description></item>
/// <item><description>Worst case  : Θ(d × n) - Same complexity regardless of input order, d = ⌈keyBits/2⌉ for full range</description></item>
/// <item><description>Comparisons : 0 (Non-comparison sort, uses bitwise operations only)</description></item>
/// <item><description>Digit Passes: d = ⌈requiredBits/2⌉ examined (early termination based on actual key range, not full key width); e ≤ d executed, the rest being identity passes</description></item>
/// <item><description>Reads       : n (initial min/max scan) + d × n (every digit is counted) + e × n (one read per executed distribute pass) + optional final copy</description></item>
/// <item><description>Writes      : e × n (one write per executed distribute pass to temp) + optional final copy</description></item>
/// <item><description>Memory      : O(n) for temporary buffer</description></item>
/// </list>
/// <para><strong>Why Keys Are Recomputed Rather Than Memoized:</strong></para>
/// <para>At radix 4 a 32-bit key takes up to 16 passes, more than any other radix here, so memoizing each
/// element's normalized key in a <c>ulong[]</c> looks like the obvious trade: compute it once, then read it
/// back 16 times. That version existed and was measured against this one, and it is 17-26% slower across
/// n = 1024/8192/65536 for both narrow and full-range <c>int</c> keys — including the 16-pass case, where the
/// memoization has the most to amortize (same-run comparison, 20 iterations). A memoized key has to be
/// permuted along with its element, so every distribute pass gains a second scattered write, of 8 bytes,
/// into a second array; recomputing costs two ALU ops on a value the pass has already read. The same effect
/// decided the corresponding question in <see cref="RadixLSD10Sort"/>, where carrying a running quotient
/// cost about 2.2x at n = 8192.</para>
/// <para><strong>Radix-4 Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>More passes than radix-256: 16 passes for 32-bit vs 4 passes for radix-256</description></item>
/// <item><description>Smaller bucket size: 4 buckets fit entirely in CPU registers/L1 cache</description></item>
/// <item><description>Simple bit operations: Only 2-bit shift and mask (&amp; 0b11)</description></item>
/// <item><description>Good for educational purposes: Shows radix sort with minimal buckets</description></item>
/// <item><description>Trade-off: More passes vs simpler bucket management</description></item>
/// </list>
/// <para><strong>Supported Key Mappings (via <see cref="IRadixKeySelector{T}"/>):</strong></para>
/// <list type="bullet">
/// <item><description><strong>Integers:</strong> byte, sbyte, short, ushort, int, uint, long, ulong, nint, nuint (up to 64-bit); Int128/UInt128/BigInteger are rejected (64-bit key ceiling, see below)</description></item>
/// <item><description><strong>Floating point:</strong> Half, float, double via IEEE 754 bit transform (all NaN values sort first, matching <see cref="IComparable{T}"/> semantics; <c>-0.0</c> and <c>+0.0</c> are a tie and keep their input order, this sort being stable)</description></item>
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
public static class RadixLSD4Sort
{
    private const int RadixBits = 2;        // 2 bits per digit
    private const int RadixSize = 4;        // 2^2 = 4 buckets

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Temporary buffer for digit redistribution

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
    /// Sorts <see cref="Half"/> values via the IEEE 754 bit transform.
    /// All NaN values sort first, matching <see cref="IComparable{T}"/> semantics.
    /// </summary>
    public static void Sort(Span<Half> span)
        => SortCore(span, default(HalfRadixKey), new ComparableComparer<Half>(), NullContext.Default);

    /// <inheritdoc cref="Sort(Span{Half})"/>
    public static void Sort<TContext>(Span<Half> span, TContext context) where TContext : ISortContext
        => SortCore(span, default(HalfRadixKey), new ComparableComparer<Half>(), context);

    /// <summary>
    /// Sorts <see cref="float"/> values via the IEEE 754 bit transform.
    /// All NaN values sort first, matching <see cref="IComparable{T}"/> semantics.
    /// </summary>
    public static void Sort(Span<float> span)
        => SortCore(span, default(SingleRadixKey), new ComparableComparer<float>(), NullContext.Default);

    /// <inheritdoc cref="Sort(Span{float})"/>
    public static void Sort<TContext>(Span<float> span, TContext context) where TContext : ISortContext
        => SortCore(span, default(SingleRadixKey), new ComparableComparer<float>(), context);

    /// <summary>
    /// Sorts <see cref="double"/> values via the IEEE 754 bit transform.
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

            // Use stackalloc for small fixed-size bucket offsets (5 ints = 20 bytes)
            Span<int> bucketOffsets = stackalloc int[RadixSize + 1];
            var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
            var temp = new SortSpan<T, TComparer, TContext>(tempBuffer, context, comparer, BUFFER_TEMP);

            // Announce the range scan: without it a consumer sees n reads with no phase attached, and the
            // label from whatever ran before stays on screen through the whole scan. KeyRangeScan rather than
            // DistributionCount: this measures the keys, it does not tally per-value occurrences.
            s.Context.OnPhase(SortPhase.KeyRangeScan);

            // Find min and max to determine actual required passes
            var minKey = ulong.MaxValue;
            var maxKey = ulong.MinValue;

            for (var i = 0; i < s.Length; i++)
            {
                var value = s.Read(i);
                var key = radixKey.GetKey(value);
                if (key < minKey) minKey = key;
                if (key > maxKey) maxKey = key;
            }

            // Calculate required number of passes from the width of the key range.
            // Digit extraction then works on (key - minKey), so every key fits in that width:
            // normalized keys span [0, maxKey - minKey], and subtracting a constant preserves order.
            var range = maxKey - minKey;

            // Early exit: if all elements are the same (range == 0), no sorting needed
            if (range == 0) return;

            // Normalizing is what makes the pass count depend on the span of the keys rather than on
            // where they sit: keys straddling a high bit boundary (e.g. signed values around zero, whose
            // sign-flipped keys straddle 0x8000_0000) share no leading bits, so max ^ min would report the
            // full key width and force every pass. bitlength(max - min) <= bitlength(max ^ min) always.
            var requiredBits = 64 - BitOperations.LeadingZeroCount(range);
            var digitCount = (requiredBits + RadixBits - 1) / RadixBits;

            // Start LSD radix sort from the least significant digit
            LSDSort(s, temp, radixKey, digitCount, minKey, bucketOffsets);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(tempArray, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    private static void LSDSort<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> temp, TRadixKey radixKey, int digitCount, ulong minKey, Span<int> bucketOffsets)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var src = s;
        var dst = temp;

        // Counted separately from d: a pass that turns out to be the identity is not run, and it is the
        // number actually run that decides which buffer the result ends up in.
        var executedPasses = 0;

        // Perform LSD radix sort with ping-pong buffers
        for (int d = 0; d < digitCount; d++)
        {
            src.Context.OnPhase(SortPhase.RadixPass, d, digitCount);
            var shift = d * RadixBits;

            // Clear bucket offsets
            bucketOffsets.Clear();

            // Count occurrences of each digit
            for (var i = 0; i < src.Length; i++)
            {
                var key = radixKey.GetKey(src.Read(i)) - minKey;
                var digit = (int)((key >> shift) & 0b11);
                bucketOffsets[digit + 1]++;
            }

            // If one bucket holds every element, this digit sorts nothing: a stable distribution over a
            // single bucket writes the elements out in the order they already are, so the pass is the
            // identity and the whole distribution can be skipped. Deriving digitCount from the key range
            // cannot catch this — that only trims uniform digits above the most significant one, while a
            // digit anywhere below it can be uniform too (every value a multiple of four, say).
            if (IsSingleBucket(bucketOffsets, src.Length)) continue;

            // Calculate cumulative offsets (prefix sum)
            for (var i = 1; i <= RadixSize; i++)
            {
                bucketOffsets[i] += bucketOffsets[i - 1];
            }

            // Distribute elements from src to dst based on current digit
            for (var i = 0; i < src.Length; i++)
            {
                var value = src.Read(i);
                var key = radixKey.GetKey(value) - minKey;
                var digit = (int)((key >> shift) & 0b11);
                var destIndex = bucketOffsets[digit]++;
                dst.Write(destIndex, value);
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
}
