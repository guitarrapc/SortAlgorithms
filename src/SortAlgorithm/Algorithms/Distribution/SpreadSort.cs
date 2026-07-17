using SortAlgorithm.Contexts;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// Boost C++ SpreadSort の integer_sort をベースにした整数ソート実装。
/// Boost.Sort のチューニング定数・アルゴリズム構造を参考に、C# / SortSpan 向けに適応しています。
/// <br/>
/// An integer sorting implementation based on the Boost C++ SpreadSort integer_sort algorithm.
/// Adopts Boost's tuning constants, range-based bucket calculation, in-place 3-way swap distribution,
/// per-bucket dynamic fallback via <c>get_min_count</c>, and <c>is_sorted_or_find_extremes</c> early detection,
/// adapted for C# generics and the SortSpan abstraction.
/// </summary>
/// <remarks>
/// <para><strong>Design Decisions Based on Boost:</strong></para>
/// <list type="bullet">
/// <item><description><strong>min_sort_size = 1000:</strong> Arrays smaller than 1000 elements fall back to PDQSort immediately (Boost: <c>min_sort_size</c>).</description></item>
/// <item><description><strong>Range-based bucket index:</strong> <c>bucket = (key >> log_divisor) - div_min</c> produces value-proportional bucket counts (Boost: <c>spreadsort_rec</c>).</description></item>
/// <item><description><strong>In-place 3-way swap:</strong> In-place distribution based on Boost's 3-way swap loop, requiring O(1) auxiliary space (Boost: <c>inner_swap_loop</c>).</description></item>
/// <item><description><strong>get_min_count per-bucket fallback:</strong> Computes a dynamic threshold from remaining bit range to decide pdqsort fallback per bucket (Boost: <c>get_min_count</c>).</description></item>
/// <item><description><strong>is_sorted_or_find_extremes:</strong> Combines sorted-detection and min/max search in a single pass (Boost: <c>is_sorted_or_find_extremes</c>).</description></item>
/// <item><description><strong>get_log_divisor:</strong> Adaptive radix width calculation with <c>max_finishing_splits</c> one-pass completion optimization (Boost: <c>get_log_divisor</c>).</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution (Hybrid: Distribution + Comparison via PDQSort + Insertion)</description></item>
/// <item><description>Stable      : No (elements are redistributed across buckets via in-place swaps)</description></item>
/// <item><description>In-place    : Partially (distribution is in-place, but this implementation uses an auxiliary bin cache).</description></item>
/// <item><description>Best case   : O(n) - When data is already sorted (early detection)</description></item>
/// <item><description>Average case: O(n √(log n)) - Hybrid distribution and comparison</description></item>
/// <item><description>Worst case  : O(n * (K/S + S)) where K = log₂(range), S = max_splits</description></item>
/// <item><description>Memory      : O(n) auxiliary metadata in this implementation (bin_sizes on stack, bin_cache via ArrayPool)</description></item>
/// </list>
/// <para><strong>Boost Constants (from constants.hpp):</strong></para>
/// <list type="bullet">
/// <item><description>max_splits = 11 — Maximum radix bits per level (cache-tuned)</description></item>
/// <item><description>max_finishing_splits = 12 — Relaxed limit for single-pass completion</description></item>
/// <item><description>int_log_mean_bin_size = 2 — Target ~4 elements per bin</description></item>
/// <item><description>int_log_min_split_count = 9 — Minimum split count for spreading</description></item>
/// <item><description>int_log_finishing_count = 31 — Threshold for single-pass completion</description></item>
/// <item><description>min_sort_size = 1000 — Minimum size to use spreadsort</description></item>
/// </list>
/// <para><strong>Supported Key Mappings (via <see cref="IRadixKeySelector{T}"/>):</strong></para>
/// <list type="bullet">
/// <item><description><strong>Integers:</strong> byte, sbyte, short, ushort, int, uint, long, ulong (fixed-width up to 64-bit);
/// nint/nuint are rejected (platform-dependent bit width makes distribution behavior inconsistent across environments); Int128/UInt128/BigInteger are rejected (64-bit key ceiling)</description></item>
/// <item><description><strong>Floating point:</strong> Half, float, double via IEEE 754 total-order key transform (all NaN values sort first, matching <see cref="IComparable{T}"/> semantics)</description></item>
/// <item><description><strong>Key selector:</strong> arbitrary element types via an extracted <c>int</c> key; NOTE: SpreadSort is unstable, so elements with equal keys may be reordered</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Boost.Sort SpreadSort: https://www.boost.org/doc/libs/release/libs/sort/doc/html/sort/sort_hpp/spreadsort.html</para>
/// <para>Paper: "Spreadsort: A Cache-Friendly Sorting Algorithm" by Steven Ross (2002) https://github.com/boostorg/sort/blob/develop/doc/papers/original_spreadsort06_2002.pdf</para>
/// </remarks>
public static class SpreadSort
{
    // Boost constants from constants.hpp
    const int MaxSplits = 11;                         // max_splits: max log₂(bucketCount) per level
    const int MaxFinishingSplits = MaxSplits + 1;     // max_finishing_splits: relaxed limit for one-pass completion
    const int LogMeanBinSize = 2;                     // int_log_mean_bin_size: target ~4 elements per bin
    const int LogMinSplitCount = 9;                   // int_log_min_split_count: minimum split count for spreading
    const int LogFinishingCount = 31;                 // int_log_finishing_count: threshold for one-pass completion
    const int MinSortSize = 1000;                     // min_sort_size: minimum size to use spreadsort
    const int InsertionSortCutoff = 16;               // Switch to insertion sort for tiny ranges within bins

    // Buffer identifiers for visualization
    const int BUFFER_MAIN = 0;

    /// <summary>
    /// Sorts the elements in the specified span in ascending order.
    /// Uses NullContext for zero-overhead fast path.
    /// </summary>
    /// <typeparam name="T">The type of elements to sort. Must be a binary integer type (up to 64-bit).</typeparam>
    /// <param name="span">The span of elements to sort.</param>
    public static void Sort<T>(Span<T> span) where T : IBinaryInteger<T>
        => Sort(span, NullContext.Default);

    /// <summary>
    /// Sorts the elements in the specified span using the specified context.
    /// </summary>
    /// <typeparam name="T">The type of elements to sort. Must be a binary integer type (up to 64-bit).</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="span">The span of elements to sort.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation.</param>
    public static void Sort<T, TContext>(Span<T> span, TContext context)
        where T : IBinaryInteger<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;

        ThrowIfUnsupportedType<T>();

        SortCore(span, default(BinaryIntegerRadixKey<T>), new ComparableComparer<T>(), context);
    }

    /// <summary>
    /// Sorts the elements in the specified span by an integer key extracted with <paramref name="keySelector"/>.
    /// NOTE: SpreadSort is unstable — elements with equal keys may be reordered.
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
    /// NOTE: SpreadSort is unstable — elements with equal keys may be reordered.
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

    static void SortCore<T, TRadixKey, TComparer, TContext>(Span<T> span, TRadixKey radixKey, TComparer comparer, TContext context)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;

        RadixKeyGuard.ValidateKeyBits<T, TRadixKey>();
        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);

        // Boost: Don't sort if it's too small to optimize (min_sort_size = 1000)
        if (s.Length < MinSortSize)
        {
            PDQSort.SortCore(s, 0, s.Length);
            return;
        }

        SpreadCore(s, radixKey);
    }

    static void SpreadCore<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, TRadixKey radixKey)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Boost: bin_sizes array sized to 1 << max_finishing_splits (4096)
        Span<int> binSizes = stackalloc int[1 << MaxFinishingSplits];

        // Boost: bin_cache is a std::vector<RandomAccessIter> shared across recursive levels.
        // Each level writes its bin boundaries into binCache[cacheOffset..cacheEnd).
        // Siblings never coexist on the stack — the parent loops sequentially, so each
        // child reuses the region starting at the parent's cacheEnd. Only the current
        // ancestor chain's regions are live at any time.
        //
        // Worst-case depth is bounded: each bin has >= 2 elements (singletons are skipped),
        // and the deepest chain of bins partitions n elements, so the sum of binCounts
        // along any root-to-leaf path is <= n. Pre-allocating s.Length is therefore sufficient.
        var rentedCache = ArrayPool<int>.Shared.Rent(s.Length);
        try
        {
            var binCache = rentedCache.AsSpan(0, s.Length);
            SpreadSortRec(s, radixKey, 0, s.Length, binCache, 0, binSizes);
        }
        finally
        {
            ArrayPool<int>.Shared.Return(rentedCache);
        }
    }

    /// <summary>
    /// Recursive SpreadSort implementation, inspired by Boost's spreadsort_rec.
    /// </summary>
    static void SpreadSortRec<T, TRadixKey, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        TRadixKey radixKey,
        int first, int last,
        Span<int> binCache, int cacheOffset,
        Span<int> binSizes)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var count = last - first;

        // Boost: is_sorted_or_find_extremes — combined sorted check + min/max finding
        if (!IsSortedOrFindExtremes(s, first, last, out var minIdx, out var maxIdx))
            return; // Already sorted

        var minKey = radixKey.GetKey(s.Read(minIdx));
        var maxKey = radixKey.GetKey(s.Read(maxIdx));

        // Compute log₂ of the value range (Boost: rough_log_2_size(max - min))
        var range = maxKey - minKey;
        var logRange = RoughLog2Size(range);

        // Boost: get_log_divisor — adaptive radix width calculation
        var logDivisor = GetLogDivisor(count, logRange);

        // Boost: bucket boundaries via range-based division
        var divMin = (long)(minKey >> logDivisor);
        var divMax = (long)(maxKey >> logDivisor);
        var binCount = (int)(divMax - divMin) + 1;

        // Boost: size_bins — clear bin_sizes and ensure bin_cache has space
        var cacheEnd = cacheOffset + binCount;

        var currentBinSizes = binSizes[..binCount];
        currentBinSizes.Clear();
        var bins = binCache.Slice(cacheOffset, binCount);

        // Phase 1: Count elements per bin (Boost: ~10% of runtime)
        s.Context.OnPhase(SortPhase.DistributionCount);
        for (var i = first; i < last; i++)
        {
            var key = radixKey.GetKey(s.Read(i));
            var bin = (int)((long)(key >> logDivisor) - divMin);
            currentBinSizes[bin]++;
        }

        // Phase 2: Compute bin positions (prefix sum using absolute indices)
        s.Context.OnPhase(SortPhase.DistributionAccumulate);
        bins[0] = first;
        for (var u = 0; u < binCount - 1; u++)
            bins[u + 1] = bins[u] + currentBinSizes[u];

        // Phase 3: In-place 3-way swap (Boost: dominates runtime)
        // Each bin position pointer advances as elements are swapped into place.
        s.Context.OnPhase(SortPhase.DistributionWrite);
        var nextBinStart = first;
        for (var u = 0; u < binCount - 1; u++)
        {
            var localBinPos = bins[u];
            nextBinStart += currentBinSizes[u];
            for (var current = localBinPos; current < nextBinStart; current++)
            {
                var targetBin = (int)((long)(radixKey.GetKey(s.Read(current)) >> logDivisor) - divMin);
                while (targetBin != u)
                {
                    // 3-way swap: reduces copies per item (Boost: ~1% faster than 2-way)
                    var b = bins[targetBin]++;
                    var bBin = (int)((long)(radixKey.GetKey(s.Read(b)) >> logDivisor) - divMin);

                    T tmp;
                    if (bBin != u)
                    {
                        var c = bins[bBin]++;
                        tmp = s.Read(c);
                        s.Write(c, s.Read(b));
                    }
                    else
                    {
                        tmp = s.Read(b);
                    }
                    s.Write(b, s.Read(current));
                    s.Write(current, tmp);

                    targetBin = (int)((long)(radixKey.GetKey(s.Read(current)) >> logDivisor) - divMin);
                }
            }
            bins[u] = nextBinStart;
        }
        bins[binCount - 1] = last;

        // Boost: If we've bucket-sorted (log_divisor == 0), the array is fully sorted
        if (logDivisor == 0)
            return;

        // Boost: get_min_count — dynamic threshold for per-bucket pdqsort fallback
        var maxCount = GetMinCount(logDivisor);

        // Phase 4: Recurse on each bin
        var lastPos = first;
        for (var u = cacheOffset; u < cacheEnd; u++)
        {
            var binEnd = binCache[u];
            var binLength = binEnd - lastPos;
            lastPos = binEnd;

            if (binLength < 2)
                continue;

            // Boost: use pdqsort if its worst-case is better for this bin
            if (binLength < maxCount)
            {
                if (binLength <= InsertionSortCutoff)
                    InsertionSort.SortCore(s, binEnd - binLength, binEnd);
                else
                    PDQSort.SortCore(s, binEnd - binLength, binEnd);
            }
            else
            {
                SpreadSortRec(s, radixKey, binEnd - binLength, binEnd, binCache, cacheEnd, binSizes);
            }
        }
    }

    /// <summary>
    /// Boost: is_sorted_or_find_extremes — combined sorted check and min/max finding.
    /// Returns true if NOT sorted (i.e., needs sorting). Returns false if already sorted.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsSortedOrFindExtremes<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        int first, int last,
        out int minIdx, out int maxIdx)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        minIdx = first;
        maxIdx = first;

        var current = first;
        // Walk sorted prefix: advance while next element >= current
        while (s.IsGreaterOrEqualAt(current + 1, current))
        {
            if (++current == last - 1)
                return false; // Entire range is sorted
        }

        // The maximum so far is the last element of the sorted prefix
        maxIdx = current;

        // Continue to find true min and max
        while (++current < last)
        {
            if (s.IsGreaterAt(current, maxIdx))
                maxIdx = current;
            else if (s.IsLessAt(current, minIdx))
                minIdx = current;
        }

        return true; // Not sorted, needs sorting
    }

    /// <summary>
    /// Boost: rough_log_2_size — Returns the number of bits required to represent the non-zero range.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int RoughLog2Size(ulong input)
    {
        if (input == 0) return 0;
        return 64 - BitOperations.LeadingZeroCount(input);
    }

    /// <summary>
    /// Boost: get_log_divisor — compute the right-shift amount (bits to discard).
    /// Radix width = logRange - logDivisor.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int GetLogDivisor(int count, int logRange)
    {
        int logDivisor;

        // Boost: If we can finish in one iteration without exceeding
        // max_finishing_splits or n bins, do so (log_divisor = 0 means use all bits)
        logDivisor = logRange - RoughLog2Size((ulong)count);
        if (logDivisor <= 0 && logRange <= MaxFinishingSplits)
        {
            logDivisor = 0;
        }
        else
        {
            // Otherwise divide the data into an optimized number of pieces
            if (logDivisor < 0) logDivisor = 0;
            logDivisor += LogMeanBinSize;

            // Cannot exceed max_splits or cache misses slow down bin lookups
            if ((logRange - logDivisor) > MaxSplits)
                logDivisor = logRange - MaxSplits;
        }

        return logDivisor;
    }

    /// <summary>
    /// Boost: get_min_count — compute the minimum element count for spreading to be worthwhile.
    /// Below this threshold, comparison sort (pdqsort) is used instead.
    /// This is the core optimization of the SpreadSort algorithm.
    /// </summary>
    static int GetMinCount(int logRange)
    {
        const int minSize = LogMeanBinSize + LogMinSplitCount; // 2 + 9 = 11

        // NOTE: Boost's get_min_count is a template parameterized on log_finishing_count,
        // and includes a guard "if (log_finishing_count < min_size)" as a compile-time
        // dead-code elimination hint for the float_sort variant (float_log_finishing_count=4 < 10).
        // For integer_sort the constants are fixed: LogFinishingCount(31) >= minSize(11),
        // so that block is always unreachable and is omitted here.

        var baseIterations = MaxSplits - LogMinSplitCount; // 11 - 9 = 2
        // sum of n to n + x = ((x + 1) * (n + (n + x)))/2 + log_mean_bin_size
        var baseRange = ((baseIterations + 1) * (MaxSplits + LogMinSplitCount)) / 2
                        + LogMeanBinSize; // ((2+1)*(11+9))/2 + 2 = 32

        if (logRange < baseRange)
        {
            var result = LogMinSplitCount; // 9
            for (var offset = minSize; offset < logRange; offset += ++result)
            {
                // intentionally empty; result is incremented in the loop
            }
            // Preventing overflow: Boost uses size_t (unsigned 64-bit) so 1 << 63 is valid,
            // but C# int is signed 32-bit where 1 << 31 = int.MinValue. Saturate at >= 31.
            var shift = result + LogMeanBinSize;
            if (shift >= 31)
                return int.MaxValue;
            return 1 << shift;
        }

        // Quick division for larger ranges
        var remainder = logRange - baseRange;
        var bitLength = ((MaxSplits - 1 + remainder) / MaxSplits)
                        + baseIterations + minSize;

        // Preventing overflow: Boost uses size_t (unsigned 64-bit) so 1 << 63 is valid,
        // but C# int is signed 32-bit where 1 << 31 = int.MinValue. Saturate at >= 31.
        if (bitLength >= 31)
            return int.MaxValue;

        return 1 << bitLength;
    }

    /// <summary>
    /// Validates that type T is a supported fixed-width integer type.
    /// Throws <see cref="NotSupportedException"/> for unsupported types.
    /// </summary>
    static void ThrowIfUnsupportedType<T>() where T : IBinaryInteger<T>
    {
        if (typeof(T) == typeof(byte) || typeof(T) == typeof(sbyte) ||
            typeof(T) == typeof(short) || typeof(T) == typeof(ushort) ||
            typeof(T) == typeof(int) || typeof(T) == typeof(uint) ||
            typeof(T) == typeof(long) || typeof(T) == typeof(ulong))
            return;

        if (typeof(T) == typeof(nint) || typeof(T) == typeof(nuint))
            throw new NotSupportedException($"Type {typeof(T).Name} is not supported. Native-sized integers have platform-dependent bit width, which makes distribution sort behavior inconsistent across 32-bit and 64-bit environments.");
        if (typeof(T) == typeof(Int128) || typeof(T) == typeof(UInt128))
            throw new NotSupportedException($"Type {typeof(T).Name} with 128-bit size is not supported. Maximum supported bit size is 64.");

        throw new NotSupportedException($"Type {typeof(T).Name} is not supported.");
    }
}
