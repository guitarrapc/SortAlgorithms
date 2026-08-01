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
/// <item><description><strong>Digit Count From The Key Range:</strong> The number of digit levels is determined by the actual range of key values,
/// not the full key width. Keys are shifted down by the minimum key, so digitCount = ⌈requiredBits / 2⌉ where requiredBits is the bit width
/// of (max − min). Subtracting a constant is order-preserving, so this changes how many levels the recursion starts from and nothing else.
/// Unlike a (max XOR min) bit width it stays effective when the range straddles a high bit boundary, as sign-flipped keys of signed values
/// around zero do.</description></item>
/// <item><description><strong>MSD Processing Order:</strong> Digits must be processed from most significant (d=digitCount-1) to least significant (d=0).
/// This top-down approach partitions the array into buckets recursively, processing each bucket independently for subsequent digits.</description></item>
/// <item><description><strong>Recursive Bucket Processing:</strong> After distributing elements based on the current digit, each bucket must be recursively sorted for the remaining digits.
/// Base cases: buckets with 0 or 1 elements are already sorted; buckets where all remaining digits are the same are also sorted.</description></item>
/// <item><description><strong>Identity Levels Are Skippable:</strong> If every element in a range shares the same value at the
/// current digit, the stable distribution for that digit writes them out in the order they already are — the distribution and the
/// copy back are both the identity and can be skipped, and the range descends to the next digit untouched. Deriving the digit count
/// from the key width cannot catch this: a uniform digit can sit anywhere, at the top of the key (values small relative to the type)
/// or inside any bucket of the recursion (a shared prefix among the elements that reached it).</description></item>
/// <item><description><strong>Cutoff to Insertion Sort:</strong> For small buckets (typically &lt; 16 elements), switching to insertion sort can improve performance due to lower overhead.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution (Radix Sort, MSD variant)</description></item>
/// <item><description>Stable      : Yes (maintains relative order of elements with equal keys)</description></item>
/// <item><description>In-place    : No (O(n) auxiliary space for temporary buffer)</description></item>
/// <item><description>Best case   : Θ(n) - When all keys are equal (early termination on range == 0, after the single key scan)</description></item>
/// <item><description>Average case: Θ(n + d × n) - One O(n) key range scan + d levels, d = ⌈bitWidth(max − min)/2⌉</description></item>
/// <item><description>Worst case  : Θ(n + d × n) - Same complexity regardless of input order, d = ⌈keyBits/2⌉ for a full-width range</description></item>
/// <item><description>Comparisons : 0 (Non-comparison sort, uses bitwise operations only)</description></item>
/// <item><description>Digit Passes: d = ⌈bitWidth(max − min)/2⌉ examined, at most ⌈keyBits/2⌉ (4 for byte, 8 for short, 16 for int, 32 for long);
/// a level whose digit is uniform is counted but not distributed</description></item>
/// <item><description>Memory      : O(n) for temporary buffer</description></item>
/// </list>
/// <para><strong>MSD vs LSD:</strong></para>
/// <list type="bullet">
/// <item><description>MSD processes high-order digits first, enabling early termination when buckets are fully sorted</description></item>
/// <item><description>MSD skips a uniform digit per bucket, where LSD can only skip one that is uniform across the whole input</description></item>
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

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);

        // Below the cutoff the recursion would insertion-sort the whole range without extracting a single
        // digit, so the range scan below would be pure overhead — n extra reads and n key extractions on an
        // input that never reaches a digit pass. Take the fallback directly, before renting a buffer no
        // digit pass would use.
        if (s.Length <= InsertionSortCutoff)
        {
            InsertionSort.SortCore(s, 0, s.Length);
            return;
        }

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

        // Every key is equal, so the input is already sorted whatever order it is in. This is the only path
        // that is linear in n: the digit passes below are always Θ(digitCount × n).
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
            var bucketStart = bucketStarts[i];
            var bucketEnd = (i == RadixSize - 1) ? length : bucketStarts[i + 1];
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
}
