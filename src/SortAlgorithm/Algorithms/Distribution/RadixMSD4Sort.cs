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
/// <para>Processes the most significant differing digit first, using radix R=4.
/// A key-range scan normalizes keys by their minimum and determines d, the number of relevant digits.
/// Uniform digits do not permute elements; singleton buckets and exhausted digits terminate recursion.
/// Every nontrivial bucket is processed by radix distribution, without a comparison-sort cutoff.</para>
/// <para>The SortBy key defines the complete order. Natural floating keys tie signed zeros and map every NaN
/// to the first key. Custom key selectors define their own equivalence classes.</para>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution (MSD radix)</description></item>
/// <item><description>Stable      : Yes - stable bucket distribution</description></item>
/// <item><description>In-place    : No - element scratch storage</description></item>
/// <item><description>Best case   : Θ(n) - equal keys need only a range scan</description></item>
/// <item><description>Average case: O(n + d × n) - radix levels over the normalized key range</description></item>
/// <item><description>Worst case  : O(n + d × n) - bounded by the digit count</description></item>
/// <item><description>Comparisons : 0 - keys and digits determine the order without element comparisons</description></item>
/// <item><description>Swaps       : 0 - distribution and copies move elements</description></item>
/// <item><description>Index Reads : O(n + d × n) - range scan and digit levels</description></item>
/// <item><description>Index Writes: O(d × n) - distribution; uniform digits do not move elements</description></item>
/// <item><description>Space       : O(n + d × R) - element scratch and radix counters</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Radix_sort#Most_significant_digit</para>
/// </remarks>
public static class RadixMSD4Sort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>See the stability note in this type's summary.</remarks>
    public static bool IsStable => true;

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
        => SortCore(span, default(HalfRadixKey), new RadixKeyComparer<Half, HalfRadixKey>(default), NullContext.Default);

    /// <inheritdoc cref="Sort(Span{Half})"/>
    public static void Sort<TContext>(Span<Half> span, TContext context) where TContext : ISortContext
        => SortCore(span, default(HalfRadixKey), new RadixKeyComparer<Half, HalfRadixKey>(default), context);

    /// <summary>
    /// Sorts <see cref="float"/> values via the IEEE 754 bit transform.
    /// All NaN values sort first, matching <see cref="IComparable{T}"/> semantics.
    /// </summary>
    public static void Sort(Span<float> span)
        => SortCore(span, default(SingleRadixKey), new RadixKeyComparer<float, SingleRadixKey>(default), NullContext.Default);

    /// <inheritdoc cref="Sort(Span{float})"/>
    public static void Sort<TContext>(Span<float> span, TContext context) where TContext : ISortContext
        => SortCore(span, default(SingleRadixKey), new RadixKeyComparer<float, SingleRadixKey>(default), context);

    /// <summary>
    /// Sorts <see cref="double"/> values via the IEEE 754 bit transform.
    /// All NaN values sort first, matching <see cref="IComparable{T}"/> semantics.
    /// </summary>
    public static void Sort(Span<double> span)
        => SortCore(span, default(DoubleRadixKey), new RadixKeyComparer<double, DoubleRadixKey>(default), NullContext.Default);

    /// <inheritdoc cref="Sort(Span{double})"/>
    public static void Sort<TContext>(Span<double> span, TContext context) where TContext : ISortContext
        => SortCore(span, default(DoubleRadixKey), new RadixKeyComparer<double, DoubleRadixKey>(default), context);

    private static void SortCore<T, TRadixKey, TComparer, TContext>(Span<T> span, TRadixKey radixKey, TComparer comparer, TContext context)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;
        RadixKeyGuard.ValidateKeyBits<T, TRadixKey>();

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);


        // Announce the range scan: without it a consumer sees n reads with no phase attached, and the
        // label from whatever ran before stays on screen through the whole scan. KeyRangeScan rather than
        // DistributionCount: this measures the keys, it does not tally per-value occurrences.
        s.Context.OnPhase(SortPhase.KeyRangeScan);

        // One scan of the keys decides how many digit levels can hold a difference.
        // Without it the digit count comes from the key width alone (16 levels for a 32-bit key), and every
        // level above the range still costs a full counting pass before the uniform-digit check in the
        // recursion can fire. That is not a corner case: the sign-flipped key of a small non-negative int
        // sits just above 0x8000_0000, so keys drawn from 0..999 leave the top 11 of the 16 levels uniform.
        var minKey = ulong.MaxValue;
        var maxKey = ulong.MinValue;
        for (var i = 0; i < s.Length; i++)
        {
            var key = radixKey.GetKey(s.Read(i));
            if (key < minKey) minKey = key;
            if (key > maxKey) maxKey = key;
        }

        // Digits are extracted from (key - minKey), so the level count follows the width of the range rather
        // than where the keys sit. Subtracting a constant is order-preserving and cannot underflow (every key
        // is >= minKey), and unlike a (max XOR min) width it stays effective when the range straddles a high
        // bit boundary — which is exactly what sign-flipped keys of signed values around zero do.
        var range = maxKey - minKey;

        // Equal mapped keys are already in the requested order.
        if (range == 0) return;

        var requiredBits = 64 - BitOperations.LeadingZeroCount(range);
        var digitCount = (requiredBits + RadixBits - 1) / RadixBits;

        // Rent temporary buffer from ArrayPool for element redistribution
        var tempArray = ArrayPool<T>.Shared.Rent(span.Length);

        try
        {
            var temp = new SortSpan<T, TComparer, TContext>(tempArray.AsSpan(0, span.Length), context, comparer, BUFFER_TEMP);

            // Start MSD radix sort from the most significant digit that can differ
            MSDSort(s, temp, radixKey, minKey, 0, s.Length, digitCount - 1, digitCount);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(tempArray, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    /// <param name="minKey">
    /// Smallest key in the whole input. Every digit is taken from (key - minKey) so that the digit count
    /// derives from the width of the key range; see the normalization note in <see cref="SortCore"/>.
    /// </param>
    /// <param name="digitCount">
    /// Total number of digit positions the normalized keys need. Carried down the recursion only to report
    /// <see cref="SortPhase.RadixPass"/>, whose contract is param1 = current digit, param2 = total digits;
    /// a consumer cannot derive the total from <paramref name="digit"/> alone.
    /// </param>
    private static void MSDSort<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> temp, TRadixKey radixKey, ulong minKey, int start, int length, int digit, int digitCount)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // No lower digit remains once the normalized keys are exhausted.
        if (digit < 0)
        {
            return;
        }

        if (length <= 1) return;

        s.Context.OnPhase(SortPhase.RadixPass, digit, digitCount);
        var shift = digit * RadixBits;

        // Allocate bucket counts on stack (RadixSize+1 = 5 elements = 20 bytes)
        // Each recursive level gets its own bucketCounts, avoiding reuse corruption
        Span<int> bucketCounts = stackalloc int[RadixSize + 1];
        bucketCounts.Clear(); // Required: [module: SkipLocalsInit] skips zero-initialization

        // Count occurrences of each digit in the current range
        for (var i = 0; i < length; i++)
        {
            var value = s.Read(start + i);
            var key = radixKey.GetKey(value) - minKey;
            var digitValue = (int)((key >> shift) & 0b11);  // Extract 2-bit digit
            bucketCounts[digitValue + 1]++;
        }

        // If one bucket holds every element, this digit partitions nothing: a stable distribution over a
        // single bucket writes the elements out in the order they already are, so the distribution and the
        // copy back are both the identity and can be skipped. The range moves on to the next digit
        // untouched, which is what lets a shared prefix cost reads only. Unlike the LSD sorts there is no
        // buffer parity to keep: every executed level copies back, so the data is always in s.
        if (IsSingleBucket(bucketCounts, length))
        {
            if (digit > 0)
            {
                MSDSort(s, temp, radixKey, minKey, start, length, digit - 1, digitCount);
            }

            // digit == 0: no lower digits left, and every key in the range is equal.
            return;
        }

        // Prefix sum. After it bucketCounts[i] is the start of bucket i and bucketCounts[i + 1] its end,
        // so the one array carries both boundaries the recursion below needs — no separate starts array,
        // and no conditional to keep one in step with it.
        for (var i = 1; i <= RadixSize; i++)
        {
            bucketCounts[i] += bucketCounts[i - 1];
        }

        // The offsets are final here, so where every bucket will lie is already decided even though no element
        // has moved yet. Report it before the distribution: a consumer that works the boundaries out for itself
        // has to reimplement the key mapping, the normalization and the digit width, and a wrong reconstruction
        // still looks like a plausible partition. The whole report sits behind the NullContext test so the
        // optimized path keeps none of it.
        if (typeof(TContext) != typeof(NullContext))
        {
            ReportBuckets(s.Context, bucketCounts, start);
        }

        // Distribute elements into temp buffer based on current digit.
        // The scatter advances its write positions, so it works on a copy and leaves the boundaries intact.
        Span<int> bucketOffsets = stackalloc int[RadixSize];
        bucketCounts[..RadixSize].CopyTo(bucketOffsets);

        for (var i = 0; i < length; i++)
        {
            var value = s.Read(start + i);
            var key = radixKey.GetKey(value) - minKey;
            var digitValue = (int)((key >> shift) & 0b11);  // Extract 2-bit digit
            var destIndex = bucketOffsets[digitValue]++;
            temp.Write(start + destIndex, value);
        }

        // Copy back from temp to source
        temp.CopyTo(start, s, start, length);

        // Recursively sort each bucket for the next digit
        for (var i = 0; i < RadixSize; i++)
        {
            var bucketStart = bucketCounts[i];
            var bucketEnd = bucketCounts[i + 1];
            var bucketLength = bucketEnd - bucketStart;

            if (bucketLength > 1)
            {
                MSDSort(s, temp, radixKey, minKey, start + bucketStart, bucketLength, digit - 1, digitCount);
            }
        }
    }

    /// <summary>
    /// True when a single bucket holds all <paramref name="length"/> elements, i.e. every element in the
    /// range shares this digit. Counts are still in their pre-prefix-sum layout, at
    /// <c>bucketCounts[digit + 1]</c>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSingleBucket(ReadOnlySpan<int> bucketCounts, int length)
    {
        // The first bucket holding anything settles it: if it does not hold everything, some other
        // bucket holds the rest. So the scan stops at the first non-empty entry either way.
        foreach (var count in bucketCounts[1..])
        {
            if (count != 0) return count == length;
        }
        return false;
    }
    /// <summary>
    /// Reports the span each non-empty bucket occupies, one <see cref="SortPhase.DistributionBucket"/> per bucket.
    /// Empty buckets are skipped, so this costs min(radix, length) reports rather than a fixed RadixSize.
    /// </summary>
    /// <param name="boundaries">Prefix-summed offsets: boundaries[d] is the start of bucket d relative to
    /// <paramref name="start"/>, boundaries[d + 1] its end.</param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ReportBuckets<TContext>(TContext context, ReadOnlySpan<int> boundaries, int start)
        where TContext : ISortContext
    {
        for (var d = 0; d < RadixSize; d++)
        {
            var length = boundaries[d + 1] - boundaries[d];
            if (length > 0)
            {
                context.OnPhase(SortPhase.DistributionBucket, start + boundaries[d], length, d);
            }
        }
    }

}
