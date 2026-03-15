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
/// <para><strong>Supported Types:</strong></para>
/// <list type="bullet">
/// <item><description><strong>Supported:</strong> byte, sbyte, short, ushort, int, uint, long, ulong (fixed-width up to 64-bit)</description></item>
/// <item><description><strong>Not Supported:</strong> nint, nuint (platform-dependent bit width), Int128, UInt128, BigInteger</description></item>
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
    /// <typeparam name="T">The type of elements to sort. Must be a binary integer type with defined min/max values.</typeparam>
    /// <param name="span">The span of elements to sort.</param>
    public static void Sort<T>(Span<T> span) where T : IBinaryInteger<T>, IMinMaxValue<T>
        => Sort(span, NullContext.Default);

    /// <summary>
    /// Sorts the elements in the specified span using the specified context.
    /// </summary>
    /// <typeparam name="T">The type of elements to sort. Must be a binary integer type with defined min/max values.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="span">The span of elements to sort.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation.</param>
    public static void Sort<T, TContext>(Span<T> span, TContext context)
        where T : IBinaryInteger<T>, IMinMaxValue<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;

        ThrowIfUnsupportedType<T>();

        var s = new SortSpan<T, ComparableComparer<T>, TContext>(span, context, new ComparableComparer<T>(), BUFFER_MAIN);

        // Boost: Don't sort if it's too small to optimize (min_sort_size = 1000)
        if (s.Length < MinSortSize)
        {
            PDQSort.SortCore(s, 0, s.Length);
            return;
        }

        SortCore(s);
    }

    static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s)
        where T : IBinaryInteger<T>, IMinMaxValue<T>
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
            SpreadSortRec(s, 0, s.Length, binCache, 0, binSizes);
        }
        finally
        {
            ArrayPool<int>.Shared.Return(rentedCache);
        }
    }

    /// <summary>
    /// Recursive SpreadSort implementation, inspired by Boost's spreadsort_rec.
    /// </summary>
    static void SpreadSortRec<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        int first, int last,
        Span<int> binCache, int cacheOffset,
        Span<int> binSizes)
        where T : IBinaryInteger<T>, IMinMaxValue<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var count = last - first;

        // Boost: is_sorted_or_find_extremes — combined sorted check + min/max finding
        if (!IsSortedOrFindExtremes(s, first, last, out var minIdx, out var maxIdx))
            return; // Already sorted

        var minKey = GetUnsignedKey(s.Read(minIdx));
        var maxKey = GetUnsignedKey(s.Read(maxIdx));

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
            var key = GetUnsignedKey(s.Read(i));
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
                var targetBin = (int)((long)(GetUnsignedKey(s.Read(current)) >> logDivisor) - divMin);
                while (targetBin != u)
                {
                    // 3-way swap: reduces copies per item (Boost: ~1% faster than 2-way)
                    var b = bins[targetBin]++;
                    var bBin = (int)((long)(GetUnsignedKey(s.Read(b)) >> logDivisor) - divMin);

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

                    targetBin = (int)((long)(GetUnsignedKey(s.Read(current)) >> logDivisor) - divMin);
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
                SpreadSortRec(s, binEnd - binLength, binEnd, binCache, cacheEnd, binSizes);
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
        where T : IBinaryInteger<T>, IMinMaxValue<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        minIdx = first;
        maxIdx = first;

        var current = first;
        // Walk sorted prefix: advance while next element >= current
        while (s.Compare(current + 1, current) >= 0)
        {
            if (++current == last - 1)
                return false; // Entire range is sorted
        }

        // The maximum so far is the last element of the sorted prefix
        maxIdx = current;

        // Continue to find true min and max
        while (++current < last)
        {
            if (s.Compare(current, maxIdx) > 0)
                maxIdx = current;
            else if (s.Compare(current, minIdx) < 0)
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

        // Boost: if we can complete in one iteration, do so
        if (LogFinishingCount < minSize)
        {
            if (logRange <= minSize && logRange <= MaxSplits)
            {
                if (logRange <= LogFinishingCount)
                    return 1 << LogFinishingCount;
                return 1 << logRange;
            }
        }

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

    /// <summary>
    /// Converts a value to an unsigned key for distribution sorting.
    /// For signed types, flips the sign bit to ensure correct ordering.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static ulong GetUnsignedKey<T>(T value) where T : IBinaryInteger<T>
    {
        // 8-bit
        if (typeof(T) == typeof(byte))
            return byte.CreateTruncating(value);
        if (typeof(T) == typeof(sbyte))
            return (ulong)((byte)sbyte.CreateTruncating(value) ^ 0x80);

        // 16-bit
        if (typeof(T) == typeof(ushort))
            return ushort.CreateTruncating(value);
        if (typeof(T) == typeof(short))
            return (ulong)((ushort)short.CreateTruncating(value) ^ 0x8000);

        // 32-bit
        if (typeof(T) == typeof(uint))
            return uint.CreateTruncating(value);
        if (typeof(T) == typeof(int))
            return (uint)int.CreateTruncating(value) ^ 0x8000_0000;

        // 64-bit
        if (typeof(T) == typeof(ulong))
            return ulong.CreateTruncating(value);
        if (typeof(T) == typeof(long))
            return (ulong)long.CreateTruncating(value) ^ 0x8000_0000_0000_0000;

        throw new NotSupportedException($"Type {typeof(T).Name} is not supported.");
    }
}
