using SortAlgorithm.Contexts;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// ska_sort（Skarupke, 2016）。キーの最上位バイトから 256 基数で分配する in-place な MSD 基数ソートで、American flag sort の置換を
/// ラウンド方式に置き換えたものです。各ラウンドは埋まっていないバケットの空きスロットを順に見て、要素を自分のバケットの次の空きスロットと
/// 無条件に交換し、戻ってきた要素は次のラウンドに回します。1,024 要素以上の範囲をこのラウンドで、128 要素以上の範囲を American flag sort で、
/// それより小さい範囲を std::sort で並べます。
/// <br/>
/// ska_sort (Skarupke, 2016). An in-place MSD radix sort with radix 256 that starts at the most significant byte of the key and
/// replaces American flag sort's permutation with rounds: a round walks the unfilled slots of every unfinished bucket and exchanges
/// each element with the next free slot of its own bucket without asking whether it is already there; the element that comes back
/// waits for the next round. Ranges of 1,024 or more elements are distributed in rounds, ranges of 128 or more by American flag sort,
/// and smaller ranges are sorted by std::sort.
/// </summary>
/// <remarks>
/// <para><strong>Algorithm:</strong></para>
/// <list type="number">
/// <item><description><strong>Entry:</strong> an input of fewer than 128 elements is sorted by std::sort. A larger one is distributed by
/// the most significant byte of its key. The key range is not measured first, so a byte that every key shares still costs a counting
/// pass before the next byte is reached.</description></item>
/// <item><description><strong>Counting:</strong> each level counts the elements of its range per byte value and lays the non-empty
/// buckets out in byte order.</description></item>
/// <item><description><strong>Rounds (1,024 elements or more):</strong> a round walks the unfilled slots of each listed bucket. The
/// element in a slot is exchanged with the next free slot of its own bucket, which is the slot itself when the element is already home,
/// so the loop has no branch on the data; the element that arrives is not looked at until the next round. A bucket that is full
/// afterwards leaves the list, and the rounds stop when at most one bucket is listed. Every element is looked at once per level and
/// ends in its bucket.</description></item>
/// <item><description><strong>American flag sort (128 to 1,023 elements):</strong> the buckets are filled in byte order. An element of
/// the bucket being filled stays; any other is exchanged with the next free slot of its bucket and the slot is read again. The last
/// occupied bucket is not walked.</description></item>
/// <item><description><strong>Recursion:</strong> nothing follows the last byte. Before it, a bucket of 128 or more elements is
/// distributed by the next byte and a smaller one of two or more elements is sorted by std::sort. After rounds the buckets are taken
/// from the end of the order the rounds left their list in; after American flag sort they are taken in byte order.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Hybrid (Distribution (MSD radix) + Partition (std::sort below 128 elements))</description></item>
/// <item><description>Stable      : No - exchanges into buckets and std::sort can reorder equal keys</description></item>
/// <item><description>In-place    : Yes - elements are exchanged within the input</description></item>
/// <item><description>Best case   : Θ(n) - the first counting pass reads every element, and the recursion stops at the first level whose buckets all hold fewer than 128 elements</description></item>
/// <item><description>Average case: O(d × n) - at most d levels, each counting and placing every element of its ranges once; d is the number of key bytes</description></item>
/// <item><description>Worst case  : O(d × n) - every level keeps a bucket of 128 or more elements until the bytes run out; std::sort adds O(n) on ranges of fewer than 128 elements</description></item>
/// <item><description>Comparisons : O(n) - only std::sort compares, on disjoint ranges of fewer than 128 elements; none when no such range has two elements</description></item>
/// <item><description>Swaps       : O(d × n) - a round exchanges every element it looks at, itself included when it is home; American flag sort exchanges each misplaced element; std::sort adds O(n)</description></item>
/// <item><description>Index Reads : O(d × n) - each level reads its range once to count and again to place, plus std::sort's O(n)</description></item>
/// <item><description>Index Writes: O(d × n) - each exchange writes two positions; std::sort's insertion shifts add O(n)</description></item>
/// <item><description>Space       : O(d × R) - per-level bucket offsets and lists on the stack, R = 256, plus std::sort's recursion</description></item>
/// </list>
/// <para><strong>Implementation notes:</strong></para>
/// <list type="bullet">
/// <item><description>The thresholds 128 and 1,024 are those of <c>ska_sort</c> (<c>inplace_radix_sort&lt;128, 1024&gt;</c>).</description></item>
/// <item><description>The reference walks a bucket four slots per iteration. Unrolling it here measured no faster, so the walk is a
/// plain loop; the order of the exchanges is the same.</description></item>
/// <item><description>std::sort is implementation-defined; this class uses <see cref="StdSort"/>, the port of libc++'s. It compares with
/// the order the keys define: the natural order for built-in integers, the key for SortBy and for floating-point values.</description></item>
/// <item><description>Keys come from <see cref="IRadixKeySelector{T}"/>, so d = ⌈KeyBits / 8⌉. The built-in floating-point keys tie -0
/// with +0 and put every NaN first, which ska_sort's own float key does not do; the SortBy contract of this library decides.</description></item>
/// <item><description>Phases: <see cref="SortPhase.RadixPass"/> opens each level, <see cref="SortPhase.DistributionBucket"/> reports the
/// buckets before anything moves, <see cref="SortPhase.SkaSortRound"/> and <see cref="SortPhase.SkaSortAmericanFlag"/> mark the
/// permutation, and <see cref="SortPhase.SkaSortStdSort"/> marks each range handed to std::sort.</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Malte Skarupke, "I Wrote a Faster Sorting Algorithm", 2016. https://probablydance.com/2016/12/27/i-wrote-a-faster-sorting-algorithm/</para>
/// <para>Code: https://github.com/skarupke/ska_sort (Boost Software License 1.0)</para>
/// </remarks>
public static class SkaSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>See the stability note in this type's summary.</remarks>
    public static bool IsStable => false;

    const int RadixBits = 8;
    const int RadixSize = 256;
    const int RadixMask = RadixSize - 1;

    // Ranges below StdSortThreshold go to std::sort, ranges below AmericanFlagSortThreshold to American flag sort.
    const int StdSortThreshold = 128;
    const int AmericanFlagSortThreshold = 1024;

    // Buffer identifiers for visualization
    const int BUFFER_MAIN = 0;       // Main input array

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
    /// </exception>
    public static void Sort<T, TContext>(Span<T> span, TContext context)
        where T : IBinaryInteger<T>
        where TContext : ISortContext
        => SortCore(span, default(BinaryIntegerRadixKey<T>), new ComparableComparer<T>(), context);

    /// <summary>
    /// Sorts the elements in the specified span by an integer key extracted with <paramref name="keySelector"/>.
    /// NOTE: this algorithm is unstable — elements with equal keys may be reordered.
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
    /// NOTE: this algorithm is unstable — elements with equal keys may be reordered.
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

    /// <param name="comparer">The order std::sort uses; it must agree with the order of <paramref name="radixKey"/>.</param>
    static void SortCore<T, TRadixKey, TComparer, TContext>(Span<T> span, TRadixKey radixKey, TComparer comparer, TContext context)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;

        // Validate the key width up front (BinaryIntegerRadixKey throws NotSupportedException for >64-bit types).
        RadixKeyGuard.ValidateKeyBits<T, TRadixKey>();

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        var digitCount = (TRadixKey.KeyBits + RadixBits - 1) / RadixBits;

        if (SortedByStdSortBelowThreshold(s, 0, s.Length)) return;
        SortLevel(s, radixKey, 0, s.Length, digitCount - 1, digitCount);
    }

    /// <summary>
    /// Sorts a range of at most one element trivially and a range below <see cref="StdSortThreshold"/> with std::sort.
    /// Returns false, without touching the range, when it is large enough to be distributed.
    /// </summary>
    static bool SortedByStdSortBelowThreshold<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int start, int length)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (length <= 1) return true;
        if (length >= StdSortThreshold) return false;

        s.Context.OnPhase(SortPhase.SkaSortStdSort, start, start + length - 1, StdSortThreshold);
        StdSort.SortCore(s, start, start + length);
        return true;
    }

    /// <param name="digit">Byte of the key this level distributes by, counted from the least significant byte.</param>
    /// <param name="digitCount">Number of bytes of the key, reported as <see cref="SortPhase.RadixPass"/>'s param2.</param>
    static void SortLevel<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, TRadixKey radixKey, int start, int length, int digit, int digitCount)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (length < AmericanFlagSortThreshold)
            AmericanFlagSortLevel(s, radixKey, start, length, digit, digitCount);
        else
            RoundSortLevel(s, radixKey, start, length, digit, digitCount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int Digit<T, TRadixKey>(TRadixKey radixKey, T value, int shift)
        where TRadixKey : struct, IRadixKeySelector<T>
        => (int)((radixKey.GetKey(value) >> shift) & RadixMask);

    /// <summary>
    /// Distributes a range of <see cref="StdSortThreshold"/> to <see cref="AmericanFlagSortThreshold"/> - 1 elements by American flag sort,
    /// then sorts each bucket by the next byte.
    /// </summary>
    static void AmericanFlagSortLevel<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, TRadixKey radixKey, int start, int length, int digit, int digitCount)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var shift = digit * RadixBits;
        s.Context.OnPhase(SortPhase.RadixPass, digit, digitCount);

        // offsets holds the counts first, then the next free slot of each bucket; ends holds the end of each occupied bucket.
        Span<int> offsets = stackalloc int[RadixSize];
        Span<int> ends = stackalloc int[RadixSize];
        Span<byte> buckets = stackalloc byte[RadixSize];
        offsets.Clear(); // [module: SkipLocalsInit] leaves stackalloc memory uninitialized

        for (var i = 0; i < length; i++)
            offsets[Digit(radixKey, s.Read(start + i), shift)]++;

        var total = 0;
        var bucketCount = 0;
        for (var d = 0; d < RadixSize; d++)
        {
            var count = offsets[d];
            if (count == 0) continue;
            offsets[d] = total;
            total += count;
            ends[d] = total;
            buckets[bucketCount++] = (byte)d;
        }

        if (bucketCount > 1)
        {
            if (typeof(TContext) != typeof(NullContext))
                ReportBuckets(s, start, offsets, ends, buckets[..bucketCount]);
            s.Context.OnPhase(SortPhase.SkaSortAmericanFlag, start, start + length - 1);

            var current = 0;
            var currentBucket = (int)buckets[0];
            var lastListed = bucketCount - 1;
            var pos = 0;
            var bucketEnd = ends[currentBucket];
            var lastElement = length - 1;
            while (true)
            {
                var d = Digit(radixKey, s.Read(start + pos), shift);
                if (d == currentBucket)
                {
                    pos++;
                    if (pos == lastElement) break;
                    if (pos == bucketEnd)
                    {
                        // Move on to the next bucket that still has a free slot. The last one is never walked.
                        do
                        {
                            current++;
                            if (current == lastListed) goto Recurse;
                            currentBucket = buckets[current];
                        } while (offsets[currentBucket] == ends[currentBucket]);

                        pos = offsets[currentBucket];
                        bucketEnd = ends[currentBucket];
                    }
                }
                else
                {
                    s.Swap(start + pos, start + offsets[d]++);
                }
            }
        }

    Recurse:
        if (digit == 0) return;

        var bucketStart = 0;
        for (var b = 0; b < bucketCount; b++)
        {
            var bucketStop = ends[buckets[b]];
            var bucketLength = bucketStop - bucketStart;
            if (!SortedByStdSortBelowThreshold(s, start + bucketStart, bucketLength))
                SortLevel(s, radixKey, start + bucketStart, bucketLength, digit - 1, digitCount);
            bucketStart = bucketStop;
        }
    }

    /// <summary>
    /// Distributes a range of at least <see cref="AmericanFlagSortThreshold"/> elements in rounds, then sorts each bucket by the next byte.
    /// </summary>
    static void RoundSortLevel<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, TRadixKey radixKey, int start, int length, int digit, int digitCount)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var shift = digit * RadixBits;
        s.Context.OnPhase(SortPhase.RadixPass, digit, digitCount);

        // offsets holds the counts first, then the next free slot of each bucket; ends[d] is where bucket d ends, empty or not,
        // so ends[d - 1] is where bucket d starts.
        Span<int> offsets = stackalloc int[RadixSize];
        Span<int> ends = stackalloc int[RadixSize];
        Span<byte> buckets = stackalloc byte[RadixSize];
        offsets.Clear(); // [module: SkipLocalsInit] leaves stackalloc memory uninitialized

        for (var i = 0; i < length; i++)
            offsets[Digit(radixKey, s.Read(start + i), shift)]++;

        var total = 0;
        var bucketCount = 0;
        for (var d = 0; d < RadixSize; d++)
        {
            var count = offsets[d];
            if (count != 0)
            {
                offsets[d] = total;
                total += count;
                buckets[bucketCount++] = (byte)d;
            }
            ends[d] = total;
        }

        if (bucketCount > 1 && typeof(TContext) != typeof(NullContext))
            ReportBuckets(s, start, offsets, ends, buckets[..bucketCount]);

        var listed = bucketCount;
        while (listed > 1)
        {
            s.Context.OnPhase(SortPhase.SkaSortRound, start, start + length - 1, listed);
            listed = Round(s, radixKey, start, shift, offsets, ends, buckets[..listed]);
        }

        if (digit == 0) return;

        // The rounds have reordered the list; the buckets are taken from its end.
        for (var b = bucketCount; b > 0; b--)
        {
            var bucket = buckets[b - 1];
            var bucketStart = bucket == 0 ? 0 : ends[bucket - 1];
            var bucketLength = ends[bucket] - bucketStart;
            if (!SortedByStdSortBelowThreshold(s, start + bucketStart, bucketLength))
                SortLevel(s, radixKey, start + bucketStart, bucketLength, digit - 1, digitCount);
        }
    }

    /// <summary>
    /// Walks every listed bucket once and moves the buckets that still have free slots afterwards to the front of the list, keeping
    /// their order. Returns how many remain listed.
    /// </summary>
    static int Round<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, TRadixKey radixKey, int start, int shift, Span<int> offsets, ReadOnlySpan<int> ends, Span<byte> listed)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var kept = 0;
        while (true)
        {
            if (kept == listed.Length) return kept;
            if (!WalkBucket(s, radixKey, start, shift, offsets, ends, listed[kept])) break;
            kept++;
        }

        for (var i = kept + 1; i < listed.Length; i++)
        {
            if (!WalkBucket(s, radixKey, start, shift, offsets, ends, listed[i])) continue;
            (listed[kept], listed[i]) = (listed[i], listed[kept]);
            kept++;
        }
        return kept;
    }

    /// <summary>
    /// Exchanges the element in each unfilled slot of <paramref name="bucket"/> with the next free slot of its own bucket.
    /// Returns whether the bucket still has free slots.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool WalkBucket<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, TRadixKey radixKey, int start, int shift, Span<int> offsets, ReadOnlySpan<int> ends, int bucket)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var first = offsets[bucket];
        var end = ends[bucket];
        if (first == end) return false;

        // The slot count is fixed before the walk; offsets[bucket] moves on while it runs when an element of this bucket is placed.
        var pos = start + first;
        var stop = start + end;
        for (; pos < stop; pos++)
            PlaceSlot(s, radixKey, start, shift, offsets, pos);

        return offsets[bucket] != end;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void PlaceSlot<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, TRadixKey radixKey, int start, int shift, Span<int> offsets, int pos)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var d = Digit(radixKey, s.Read(pos), shift);
        s.Swap(pos, start + offsets[d]++);
    }

    /// <summary>
    /// Reports the span each non-empty bucket occupies, one <see cref="SortPhase.DistributionBucket"/> per bucket, before any element moves.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    static void ReportBuckets<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int start, ReadOnlySpan<int> offsets, ReadOnlySpan<int> ends, ReadOnlySpan<byte> buckets)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        foreach (var d in buckets)
            s.Context.OnPhase(SortPhase.DistributionBucket, start + offsets[d], ends[d] - offsets[d], d);
    }
}
