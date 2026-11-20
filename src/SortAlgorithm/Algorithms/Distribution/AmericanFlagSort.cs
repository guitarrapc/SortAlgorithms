using SortAlgorithm.Contexts;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// American Flag Sort - In-place MSD Radix Sortの実装。
/// 値をビット列として扱い、8ビットずつ（256種類）の桁に分けて要素を分類し、in-placeで並び替えます。
/// 最上位桁（Most Significant Digit）から最下位桁へ向かって処理することで、再帰的にソートを実現します。
/// <see cref="RadixMSD4Sort"/>と異なり補助バッファを一切確保せず、配列内の置換サイクルで要素を移すことでin-placeソートを実現します。
/// <br/>
/// American Flag Sort - An in-place MSD Radix Sort implementation.
/// Treats values as bit sequences, dividing them into 8-bit digits (256 buckets) and classifying elements in-place.
/// Processing from the Most Significant Digit to the least significant ensures a recursive sort.
/// Unlike <see cref="RadixMSD4Sort"/>, this implementation allocates no auxiliary buffer at all and moves elements along permutation cycles within the array.
/// </summary>
/// <remarks>
/// <para>Processes the most significant differing digit first, using radix R=256.
/// A key-range scan normalizes keys by their minimum and determines d, the number of relevant digits.
/// Uniform digits do not permute elements; singleton buckets and exhausted digits terminate recursion.
/// Every nontrivial bucket is processed by radix distribution, without a comparison-sort cutoff.</para>
/// <para>The permutation walks the buckets in order. An element already in the bucket being filled stays where it is; any other
/// element is carried around a cycle: it is written to the next free slot of its own bucket, the element it evicts is carried on,
/// and the cycle closes when an evicted element belongs to the slot it started from. One temporary holds the carried element, as in
/// McIlroy, Bostic and McIlroy's program, so a misplaced element costs one read and one write rather than an exchange.
/// The walk stops before the last occupied bucket: once the buckets below it are full, every element left in its range belongs to it.</para>
/// <para>The SortBy key defines the complete order. Natural floating keys tie signed zeros and map every NaN
/// to the first key. Custom key selectors define their own equivalence classes.</para>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution (MSD radix)</description></item>
/// <item><description>Stable      : No - cycle permutations can reorder equal keys</description></item>
/// <item><description>In-place    : Yes - elements are permuted in the input</description></item>
/// <item><description>Best case   : Θ(n) - equal keys need only a range scan</description></item>
/// <item><description>Average case: O(n + d × n) - radix levels over the normalized key range</description></item>
/// <item><description>Worst case  : O(n + d × n) - bounded by the digit count</description></item>
/// <item><description>Comparisons : 0 - keys and digits determine the order without element comparisons</description></item>
/// <item><description>Swaps       : 0 - a cycle moves elements by assignment through one temporary, never by exchange</description></item>
/// <item><description>Index Reads : O(n + d × n) - range scan and digit levels</description></item>
/// <item><description>Index Writes: O(d × n) - every write puts an element in its final slot for the level, so a level writes each slot at most once; elements already in their bucket and uniform digits are not written</description></item>
/// <item><description>Space       : O(d × R) - radix counters and recursion stack</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/American_flag_sort</para>
/// <para>Peter M. McIlroy, Keith Bostic and M. Douglas McIlroy, "Engineering Radix Sort", Computing Systems 6(1), 1993, pp. 5–27.</para>
/// </remarks>
public static class AmericanFlagSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>See the stability note in this type's summary.</remarks>
    public static bool IsStable => false;

    private const int RadixBits = 8;        // 8 bits per digit
    private const int RadixSize = 256;      // 2^8 = 256 buckets
    private const int RadixMask = RadixSize - 1;

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array

    /// <summary>
    /// Sorts the elements in the specified span in ascending order.
    /// Uses NullContext for zero-overhead fast path.
    /// </summary>
    /// <typeparam name="T"> The type of elements to sort. Must be a binary integer type (up to 64-bit).</typeparam>
    /// <param name="span"> The span of elements to sort.</param>
    public static void Sort<T>(Span<T> span) where T : IBinaryInteger<T>
        => Sort(span, NullContext.Default);

    /// <summary>
    /// Sorts the elements in the specified span using the specified context.
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
    /// NOTE: the in-place cyclic permutation is unstable — elements with equal keys may be reordered.
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
    /// NOTE: the in-place cyclic permutation is unstable — elements with equal keys may be reordered.
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
    /// NOTE: this algorithm is unstable - elements with equal keys may be reordered.
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

        // Validate the key width up front (BinaryIntegerRadixKey throws NotSupportedException for >64-bit types).
        RadixKeyGuard.ValidateKeyBits<T, TRadixKey>();

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);


        // Announce the range scan: without it a consumer sees n reads with no phase attached, and the label
        // from whatever ran before stays on screen. KeyRangeScan rather than DistributionCount: this measures
        // the keys, it does not tally per-value occurrences.
        s.Context.OnPhase(SortPhase.KeyRangeScan);

        // One scan of the keys decides how many digit levels can hold a difference.
        // Without it the digit count comes from the key width alone (4 levels for a 32-bit key), and every
        // level whose digit happens to be uniform still costs a full counting pass before the uniform-digit
        // check in the recursion can fire: keys drawn from 0..999 need only 2 levels, so the other 2 were
        // spent proving a digit was uniform before any element moved.
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

        // Start American Flag Sort from the most significant digit that can differ
        AmericanFlagSortRecursive(s, radixKey, minKey, 0, s.Length, digitCount - 1, digitCount);
    }

    /// <param name="minKey">
    /// Smallest key in the whole input. Every digit is taken from (key - minKey) so that the digit count
    /// derives from the width of the key range; see the normalization note in <see cref="SortCore"/>.
    /// </param>
    /// <param name="digitCount">
    /// Total number of digit positions the normalized keys need. Carried down the recursion to report
    /// <see cref="SortPhase.RadixPass"/>, whose contract is param1 = current digit, param2 = total digits;
    /// a consumer cannot derive the total from <paramref name="digit"/> alone.
    /// </param>
    // No AggressiveInlining here: the JIT will not inline a recursive call, so the attribute could only ever
    // affect the single entry call from SortCore, and it would pull a 2 KB stackalloc into that frame.
    // The sibling MSD sorts do not carry it either.
    private static void AmericanFlagSortRecursive<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, TRadixKey radixKey, ulong minKey, int start, int length, int digit, int digitCount)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (length <= 1) return;

        // Base case: if we've processed all digits, we're done
        if (digit < 0)
        {
            return;
        }

        var shift = digit * RadixBits;

        // Bucket arrays live on the stack: 2052 bytes per level (257 + 256 ints). Recursion depth is capped
        // by the digit count (at most 4 levels for a 32-bit key, 8 for a 64-bit one), so the whole sort holds
        // at most ~16 KB of them.
        Span<int> bucketCounts = stackalloc int[RadixSize + 1];  // Stores both start and end for each bucket
        Span<int> bucketNext = stackalloc int[RadixSize];        // Current write position for each bucket

        // Phase 1: Count occurrences of each digit value
        s.Context.OnPhase(SortPhase.RadixPass, digit, digitCount);
        // Store count for digit d in bucketCounts[d+1] (off-by-one trick for prefix sum)
        bucketCounts.Clear();

        for (var i = 0; i < length; i++)
        {
            var value = s.Read(start + i);
            var key = radixKey.GetKey(value) - minKey;
            var digitValue = (int)((key >> shift) & RadixMask);  // Extract 8-bit digit
            bucketCounts[digitValue + 1]++;
        }

        // Find the last occupied bucket while bucketCounts[i+1] still holds the raw count for bucket i.
        // length > 1, so some bucket is occupied and the scan stops.
        var lastBucket = RadixMask;
        while (bucketCounts[lastBucket + 1] == 0)
            lastBucket--;

        // If that bucket holds every element the digit is uniform:
        // skip permutation and recursively process the next digit
        if (bucketCounts[lastBucket + 1] == length)
        {
            if (digit > 0)
                AmericanFlagSortRecursive(s, radixKey, minKey, start, length, digit - 1, digitCount);

            // If digit == 0, there are no lower digits left to process, so we're done.
            return;
        }

        // Phase 2: Calculate bucket offsets (prefix sum)
        // After prefix sum: bucketCounts[d] = start of bucket d, bucketCounts[d+1] = end of bucket d
        // This gives us both boundaries for each bucket from a single array
        for (var i = 1; i <= RadixSize; i++)
        {
            bucketCounts[i] += bucketCounts[i - 1];
        }

        // Phase 2.5: Initialize next write positions
        // bucketNext[i] tracks the current write position for bucket i
        // Copy bucket start positions from bucketCounts[i] (after prefix sum, bucketCounts[i] = start of bucket i)
        for (var i = 0; i < RadixSize; i++)
        {
            bucketNext[i] = bucketCounts[i];
        }

        // The offsets are final here, so where every bucket will lie is already decided even though no
        // element has moved yet. Report it before the permutation: a consumer that has to work the boundaries
        // out for itself has to reimplement the key mapping, the normalization and the digit width, and a
        // wrong reconstruction still looks like a plausible partition. The whole report sits behind the
        // NullContext test so the optimized path keeps none of it.
        if (typeof(TContext) != typeof(NullContext))
        {
            ReportBuckets(s, bucketCounts, start);
        }

        // Phase 3: In-place permutation
        // Rearrange elements into their correct buckets using cyclic permutation
        PermuteInPlace(s, radixKey, minKey, start, shift, lastBucket, bucketCounts, bucketNext);

        // Phase 4: Recursively sort each bucket for the next digit
        for (var i = 0; i < RadixSize; i++)
        {
            // bucketCounts provides direct access to boundaries
            var bucketStart = bucketCounts[i];
            var bucketEnd = bucketCounts[i + 1];  // No conditional needed!
            var bucketLength = bucketEnd - bucketStart;

            if (bucketLength > 1)
            {
                AmericanFlagSortRecursive(s, radixKey, minKey, start + bucketStart, bucketLength, digit - 1, digitCount);
            }
        }
    }

    /// <summary>
    /// Reports the span each non-empty bucket occupies, one <see cref="SortPhase.DistributionBucket"/> per bucket.
    /// Empty buckets are skipped, so this costs min(radix, length) reports rather than a fixed 256.
    /// </summary>
    /// <param name="boundaries">Prefix-summed offsets: boundaries[d] is the start of bucket d relative to
    /// <paramref name="start"/>, boundaries[d + 1] its end.</param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ReportBuckets<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, ReadOnlySpan<int> boundaries, int start)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        for (var d = 0; d < RadixSize; d++)
        {
            var length = boundaries[d + 1] - boundaries[d];
            if (length > 0)
            {
                s.Context.OnPhase(SortPhase.DistributionBucket, start + boundaries[d], length, d);
            }
        }
    }

    /// <summary>
    /// Permutes elements in-place into their correct buckets along permutation cycles, carrying one element at a time.
    /// </summary>
    /// <remarks>
    /// Array roles:
    /// - <paramref name="bucketCounts"/>: Immutable boundary array where bucketCounts[i] = start of bucket i, bucketCounts[i+1] = end of bucket i
    /// - <paramref name="bucketNext"/>: Mutable current write position for each bucket (incremented as elements are placed)
    /// This separation ensures correct boundary detection and avoids array role confusion.
    /// <paramref name="minKey"/> must be the same value the counting pass used, or an element would be routed
    /// to a bucket the counts never reserved space for.
    /// </remarks>
    /// <param name="lastBucket">The highest bucket with a nonzero count. Buckets above it are empty.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void PermuteInPlace<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, TRadixKey radixKey, ulong minKey, int start, int shift, int lastBucket, Span<int> bucketCounts, Span<int> bucketNext)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // In-place permutation using bucket positions.
        // The last occupied bucket is not visited: once the buckets below it are filled, every element left
        // in its range belongs to it, so walking it would only re-read elements that are already home.
        for (var bucket = 0; bucket < lastBucket; bucket++)
        {
            // Get the range for this bucket directly from bucketCounts
            // bucketCounts[bucket] = start, bucketCounts[bucket + 1] = end
            var bucketEnd = bucketCounts[bucket + 1];

            while (bucketNext[bucket] < bucketEnd)
            {
                var leaderPos = start + bucketNext[bucket];
                var value = s.Read(leaderPos);
                var digit = (int)(((radixKey.GetKey(value) - minKey) >> shift) & RadixMask);

                // Already in the bucket being filled: it stays.
                if (digit == bucket)
                {
                    bucketNext[bucket]++;
                    continue;
                }

                // Carry the element to the next free slot of its bucket and pick up the one it evicts, until an
                // evicted element belongs to the leader's bucket. The slots and the final order are those an exchange
                // with the leader slot would give; holding the element instead saves the write back to the leader per step.
                do
                {
#if DEBUG
                    Debug.Assert(bucketNext[digit] >= bucketCounts[digit]);
                    Debug.Assert(bucketNext[digit] < bucketCounts[digit + 1]);
#endif
                    var targetPos = start + bucketNext[digit]++;
                    var evicted = s.Read(targetPos);
                    s.Write(targetPos, value);
                    value = evicted;
                    digit = (int)(((radixKey.GetKey(value) - minKey) >> shift) & RadixMask);
                } while (digit != bucket);

                s.Write(leaderPos, value);
                bucketNext[bucket]++;
            }
        }
    }

}
