using SortAlgorithm.Contexts;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
/// <item><description>Family      : Distribution (Hybrid: Distribution + Comparison via PDQSort)</description></item>
/// <item><description>Stable      : No (elements are redistributed across buckets via in-place swaps)</description></item>
/// <item><description>In-place    : Partially (distribution is in-place, but this implementation uses an auxiliary bin cache).</description></item>
/// <item><description>Best case   : O(n) - When data is already sorted (early detection)</description></item>
/// <item><description>Average case: O(n √(log n)) - Hybrid distribution and comparison</description></item>
/// <item><description>Worst case  : O(n * (K/S + S)) where K = log₂(range), S = max_splits</description></item>
/// <item><description>Memory      : O(1) auxiliary metadata — both bin_sizes and bin_cache are bounded by the
/// key width and the tuning constants, independent of n (bin_sizes on stack, bin_cache via ArrayPool)</description></item>
/// </list>
/// <para><strong>Why SpreadSort over the other distribution sorts:</strong></para>
/// <para>All of the distribution sorts in this namespace are linear-ish in n; they differ in what they
/// assume about the key distribution and in what they allocate. SpreadSort's distinguishing properties,
/// each traceable to the implementation:</para>
/// <list type="bullet">
/// <item><description><strong>Auxiliary memory does not scale with n.</strong> <c>bin_sizes</c> is a fixed
/// <c>stackalloc</c> of 2^<see cref="MaxFinishingSplits"/>, and the pooled <c>bin_cache</c> is sized by
/// <see cref="BinCacheCapacity"/> from the key width alone. <see cref="RadixLSD256Sort"/> and
/// <see cref="BucketSort"/> need O(n) auxiliary space, and <see cref="FlashSort"/>'s O(m) is O(n) in
/// practice because it picks m = ⌊0.43n⌋. Only <see cref="AmericanFlagSort"/> matches this.</description></item>
/// <item><description><strong>The radix width is re-derived at every level, not fixed.</strong>
/// <see cref="GetLogDivisor"/> computes the split count from the range and element count of the
/// <em>current</em> sub-range. LSD radix also adapts, but once and globally (pass count from
/// <c>max XOR min</c> in 8-bit steps), and American Flag's 4-bit digit width is fixed regardless of
/// range. So a clustered distribution with a distant outlier costs the radix sorts the passes the full
/// span implies, while here each recursion narrows to what its own bucket actually contains.</description></item>
/// <item><description><strong>Each bucket independently decides to stop distributing.</strong>
/// <see cref="GetMinCount"/> derives a per-bucket threshold from the remaining bit range and hands
/// small or narrow buckets to <see cref="PDQSort"/>. The radix sorts commit to the remaining digit
/// passes for every bucket; FlashSort and BucketSort commit to Insertion Sort per class, which is why
/// FlashSort degrades to O(n²) when a class is oversubscribed. The comparison fallback bounds the
/// non-uniform case here instead.</description></item>
/// <item><description><strong>Sorted input is detected, not just equal input.</strong>
/// <see cref="IsSortedOrFindExtremes"/> returns in a single pass that would have been spent finding
/// the extremes anyway. The radix sorts only short-circuit on <c>range == 0</c>.</description></item>
/// <item><description><strong>Below <c>min_sort_size</c> it is PDQSort.</strong> Small inputs pay a
/// comparison sort rather than a distribution pass whose setup exceeds the work, so the type is a
/// reasonable default when the input size is not known in advance.</description></item>
/// </list>
/// <para><strong>When to prefer another distribution sort:</strong> SpreadSort is unstable — when equal
/// keys must retain their input order, use <see cref="RadixLSD256Sort"/> or <see cref="BucketSort"/>.
/// It also requires a fixed-width key of at most 64 bits (see Supported Key Mappings below), so wider
/// integer types are only served by the sorts that accept them. And when the key range is small and
/// known, <see cref="CountingSort"/> or <see cref="PigeonholeSort"/> do the same work in one pass
/// without recursion.</para>
/// <para><strong>Boost Constants (from constants.hpp):</strong></para>
/// <list type="bullet">
/// <item><description>max_splits = 11 — Maximum radix bits per level (cache-tuned)</description></item>
/// <item><description>max_finishing_splits = 12 — Relaxed limit for single-pass completion</description></item>
/// <item><description>int_log_mean_bin_size = 2 — Target ~4 elements per bin</description></item>
/// <item><description>int_log_min_split_count = 9 — Minimum split count for spreading</description></item>
/// <item><description>int_log_finishing_count = 31 — Above min_size, so single-pass completion is disabled</description></item>
/// <item><description>float_log_min_split_count = 8, float_log_finishing_count = 4 — the floating-point
/// instantiation, where single-pass completion IS enabled (see <see cref="GetMinCount"/>)</description></item>
/// <item><description>min_sort_size = 1000 — Minimum size to use spreadsort</description></item>
/// </list>
/// <para><strong>Tuning constants follow Boost's two instantiations:</strong></para>
/// <para>Boost templates <c>get_min_count</c> on these constants and instantiates it once for
/// <c>integer_sort</c> and once for <c>float_sort</c>; this implementation does the same. The
/// floating-point overloads are the <c>float_*</c> instantiation even though they otherwise run the
/// <c>integer_sort</c> algorithm on transformed keys — <c>float_sort</c>'s remaining machinery
/// (splitting positives from negatives, iterating negative bins in reverse) exists only because
/// Boost casts float bits to a <em>signed</em> integer, where negative floats come out descending.
/// The order-preserving key transform used here removes that need entirely.</para>
/// <para>Measured: the constants only diverge where <c>log_divisor</c> is small enough to reach the
/// one-pass-completion branch, which depends on key width. At 32/64 bits it is never reached, so
/// float and double are unaffected; at 16 bits <see cref="Half"/> is 1.1x-2.5x faster with the
/// <c>float_*</c> constants. The same branch measured neutral on the integer path at the same key
/// width (<c>short</c>), so integer_sort keeps Boost's setting.</para>
/// <para><strong>Supported Key Mappings (via <see cref="IRadixKeySelector{T}"/>):</strong></para>
/// <list type="bullet">
/// <item><description><strong>Integers:</strong> byte, sbyte, short, ushort, int, uint, long, ulong (fixed-width up to 64-bit);
/// nint/nuint are rejected (platform-dependent bit width makes distribution behavior inconsistent across environments); Int128/UInt128/BigInteger are rejected (64-bit key ceiling)</description></item>
/// <item><description><strong>Floating point:</strong> Half, float, double via IEEE 754 bit transform
/// (all NaN values sort first, matching <see cref="IComparable{T}"/> semantics; <c>-0.0</c> and <c>+0.0</c>
/// are a tie and their relative order is unspecified, as in <c>Array.Sort</c>)</description></item>
/// <item><description><strong>Key selector:</strong> arbitrary element types via an extracted <c>int</c> key; NOTE: SpreadSort is unstable, so elements with equal keys may be reordered</description></item>
/// </list>
/// <para><strong>Why the extremes are found by key, not by comparison:</strong></para>
/// <para>Boost's <c>is_sorted_or_find_extremes</c> locates the min and max with <c>operator&lt;</c>,
/// which is safe there because <c>operator&lt;</c> and <c>operator&gt;&gt;</c> are assumed to agree.
/// Here the bin index is <c>(key &gt;&gt; log_divisor) - div_min</c>, so it only lands inside
/// <c>[0, binCount)</c> when <c>div_min</c>/<c>div_max</c> are the extremes <em>by key</em>. NaN breaks
/// the assumption: <see cref="SortSpan{T, TComparer, TContext}"/> specializes
/// <see cref="ComparableComparer{T}"/> to raw IEEE 754 operators, under which NaN is unordered and so is
/// never selected as the minimum even though its key is 0, and the comparison-derived minimum then fails
/// to bound the keys.</para>
/// <para><c>-0.0</c> used to be a second reason — it is equal to <c>+0.0</c> under both comparison paths
/// while the raw IEEE 754 <c>totalOrder</c> transform gave it a strictly smaller key. That divergence was
/// removed where it started, in <see cref="DoubleRadixKey"/> and its siblings, which now map <c>-0.0</c>
/// and <c>+0.0</c> to the same key. So on this path the comparer and the key now agree on everything
/// except NaN.</para>
/// <para>Only the extremes need this treatment. The already-sorted check keeps using the comparer, which
/// is both faster and still sound: after the NaN pre-pass the comparer order and the key order coincide.</para>
/// <para>That leaves the per-bin PDQSort fallback, which does use <c>TComparer</c>. NaN would still
/// be unordered there (its key is 0, so it shares the lowest bin with the most negative values), so
/// the floating-point overloads partition NaN to the front first — the same pre-pass PDQSort and
/// IntroSort use — and spread only the ordered tail.</para>
/// <para><c>-0.0</c> and <c>+0.0</c> are a genuine tie: equal under IEEE 754, under
/// <see cref="IComparable{T}"/>, and now by key as well, so they always share a bin. SpreadSort is
/// unstable, so their relative order is unspecified, exactly as in <c>Array.Sort</c>.</para>
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
    const int FloatLogMinSplitCount = 8;              // float_log_min_split_count
    const int FloatLogFinishingCount = 4;             // float_log_finishing_count: enables one-pass completion
    const int MinSortSize = 1000;                     // min_sort_size: minimum size to use spreadsort

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
    /// Sorts <see cref="Half"/> values via the IEEE 754 bit transform.
    /// All NaN values sort first, matching <see cref="IComparable{T}"/> semantics.
    /// </summary>
    public static void Sort(Span<Half> span)
        => SortFloatCore(span, default(HalfRadixKey), NullContext.Default);

    /// <inheritdoc cref="Sort(Span{Half})"/>
    public static void Sort<TContext>(Span<Half> span, TContext context) where TContext : ISortContext
        => SortFloatCore(span, default(HalfRadixKey), context);

    /// <summary>
    /// Sorts <see cref="float"/> values via the IEEE 754 bit transform.
    /// All NaN values sort first, matching <see cref="IComparable{T}"/> semantics.
    /// </summary>
    public static void Sort(Span<float> span)
        => SortFloatCore(span, default(SingleRadixKey), NullContext.Default);

    /// <inheritdoc cref="Sort(Span{float})"/>
    public static void Sort<TContext>(Span<float> span, TContext context) where TContext : ISortContext
        => SortFloatCore(span, default(SingleRadixKey), context);

    /// <summary>
    /// Sorts <see cref="double"/> values via the IEEE 754 bit transform.
    /// All NaN values sort first, matching <see cref="IComparable{T}"/> semantics.
    /// </summary>
    public static void Sort(Span<double> span)
        => SortFloatCore(span, default(DoubleRadixKey), NullContext.Default);

    /// <inheritdoc cref="Sort(Span{double})"/>
    public static void Sort<TContext>(Span<double> span, TContext context) where TContext : ISortContext
        => SortFloatCore(span, default(DoubleRadixKey), context);

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

        SpreadCore(s, radixKey, 0, s.Length, LogMinSplitCount, LogFinishingCount);
    }

    /// <summary>
    /// Entry point for the floating-point overloads. Identical to <see cref="SortCore"/> apart from
    /// NaN handling: NaN maps to key 0 and therefore shares the lowest bin with the most negative
    /// values, where the comparisons <c>TComparer</c> performs leave it unordered.
    /// </summary>
    /// <remarks>
    /// The spread path needs no NaN pre-pass here — <see cref="SpreadSortRec"/> detects NaN from the
    /// extremes it already computes — so NaN-free and already-sorted input pays nothing for this.
    /// The small-input path does need one, because <see cref="PDQSort.SortCore"/> is the internal
    /// entry and, unlike PDQSort's public entry, does not partition NaN itself.
    /// </remarks>
    static void SortFloatCore<T, TRadixKey, TContext>(Span<T> span, TRadixKey radixKey, TContext context)
        where T : IComparable<T>
        where TRadixKey : struct, IRadixKeySelector<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;

        RadixKeyGuard.ValidateKeyBits<T, TRadixKey>();
        var s = new SortSpan<T, ComparableComparer<T>, TContext>(span, context, new ComparableComparer<T>(), BUFFER_MAIN);

        if (span.Length < MinSortSize)
        {
            var nanEnd = FloatingPointUtils.MoveNaNsToFront(s, 0, span.Length);
            PDQSort.SortCore(s, nanEnd, span.Length);
            return;
        }

        // Boost instantiates spreadsort_rec with the float_* tuning constants for float_sort.
        SpreadCore(s, radixKey, 0, span.Length, FloatLogMinSplitCount, FloatLogFinishingCount);
    }

    /// <summary>
    /// Capacity the bin cache must have for a key of <paramref name="keyBits"/> bits.
    /// </summary>
    /// <remarks>
    /// <para>Boost's <c>bin_cache</c> is a <c>std::vector</c> that <c>size_bins</c> grows on demand
    /// (<c>bin_cache.resize(cache_end)</c>), so it never needs a closed-form bound. A single pooled
    /// rental does, and it is NOT n: <c>binCount</c> is derived from the key range, not from the
    /// element count, so a level with few elements spread over a wide range still claims thousands
    /// of slots. The bound below comes from the recursion structure instead.</para>
    /// <para>Per level, <c>bits = logRange - logDivisor</c> and <c>binCount = 2^bits</c>:</para>
    /// <list type="bullet">
    /// <item><description>A level that recurses has <c>logDivisor != 0</c>, so <c>get_log_divisor</c>'s
    /// max_splits clamp caps <c>bits</c> at <see cref="MaxSplits"/> — at most 2^11 slots.</description></item>
    /// <item><description>A level with <c>logDivisor == 0</c> takes up to 2^<see cref="MaxFinishingSplits"/>
    /// slots but returns before recursing, so it can only ever be the last level of a chain.</description></item>
    /// <item><description>Each level consumes at least 8 bits of range: <c>bits</c> is
    /// <c>roughLog2(count) - 2</c> (clamped to <see cref="MaxSplits"/>), and <c>count</c> is at least
    /// <see cref="MinSortSize"/> at the root and at least <c>get_min_count</c>'s floor of 2^11 in any
    /// recursive call. A child's range is bounded by its parent's <c>logDivisor</c>, so the chain is
    /// at most <c>keyBits / 8</c> levels deep.</description></item>
    /// </list>
    /// <para>The <c>+ 1</c> is slack for the root, which may consume as few as 8 bits while the bound
    /// divides by 8 exactly. <see cref="SpreadSortRec"/> also re-checks the capacity and falls back to
    /// PDQSort rather than indexing out of range, so a miscalculation here degrades to a slower sort
    /// instead of a crash.</para>
    /// </remarks>
    static int BinCacheCapacity(int keyBits)
        => ((keyBits / 8) + 1) * (1 << MaxSplits) + (1 << MaxFinishingSplits);

    [SkipLocalsInit]
    static void SpreadCore<T, TRadixKey, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, TRadixKey radixKey, int first, int last,
        int logMinSplitCount, int logFinishingCount)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Boost: bin_sizes array sized to 1 << max_finishing_splits (4096).
        // SkipLocalsInit: only binSizes[..binCount] is ever touched, and size_bins' equivalent
        // (currentBinSizes.Clear()) zeroes exactly that prefix before the counting pass.
        Span<int> binSizes = stackalloc int[1 << MaxFinishingSplits];

        // Boost: bin_cache is a std::vector<RandomAccessIter> shared across recursive levels.
        // Each level writes its bin boundaries into binCache[cacheOffset..cacheEnd).
        // Siblings never coexist on the stack — the parent loops sequentially, so each
        // child reuses the region starting at the parent's cacheEnd. Only the current
        // ancestor chain's regions are live at any time.
        var capacity = BinCacheCapacity(TRadixKey.KeyBits);
        var rentedCache = ArrayPool<int>.Shared.Rent(capacity);
        try
        {
            var binCache = rentedCache.AsSpan(0, capacity);
            SpreadSortRec(s, radixKey, first, last, binCache, 0, binSizes, logMinSplitCount, logFinishingCount);
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
        Span<int> binSizes,
        int logMinSplitCount, int logFinishingCount)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var count = last - first;

        // Boost: is_sorted_or_find_extremes — combined sorted check + min/max finding
        if (!IsSortedOrFindExtremes(s, radixKey, first, last, out var minKey, out var maxKey))
            return; // Already sorted. A comparer-sorted verdict also rules out NaN: NaN is unordered
                    // under the comparisons SortSpan performs, so any NaN breaks the ascending walk.

        // NaN maps to key 0 and no non-NaN floating-point value does (the smallest non-NaN key is -∞'s;
        // -0.0 shares +0.0's key, which is mid-range),
        // so the extremes pass has already detected NaN — no dedicated scan needed. NaN cannot be
        // ordered by TComparer, so partition it to the front and re-derive the extremes over what is
        // left. Folded away for non-floating-point T, where key 0 is an ordinary value and
        // MoveNaNsToFront compiles to `return first`.
        if (minKey == 0)
        {
            var nanEnd = FloatingPointUtils.MoveNaNsToFront(s, first, last);
            if (nanEnd != first)
            {
                first = nanEnd;
                count = last - first;
                if (count <= 1) return; // all NaN, and NaN values are mutually equal

                // Removing the NaN values can take the remainder below min_sort_size; apply the
                // same policy the entry point does rather than spreading a tiny range.
                if (count < MinSortSize)
                {
                    PDQSort.SortCore(s, first, last);
                    return;
                }

                if (!IsSortedOrFindExtremes(s, radixKey, first, last, out minKey, out maxKey))
                    return;
            }
        }

        // Compute log₂ of the value range (Boost: rough_log_2_size(max - min))
        var range = maxKey - minKey;
        var logRange = RoughLog2Size(range);

        // Boost: get_log_divisor — adaptive radix width calculation
        var logDivisor = GetLogDivisor(count, logRange);

        // Boost: bucket boundaries via range-based division
        var divMin = (long)(minKey >> logDivisor);
        var divMax = (long)(maxKey >> logDivisor);
        var binCount = (int)(divMax - divMin) + 1;

        // Boost: size_bins — clear bin_sizes and ensure bin_cache has space.
        // Boost resizes bin_cache here; the pooled rental cannot grow, so on the (expected
        // unreachable) capacity miss fall back to a comparison sort instead of indexing past
        // the end. See BinCacheCapacity for why this cannot fire.
        var cacheEnd = cacheOffset + binCount;
        if (cacheEnd > binCache.Length)
        {
            PDQSort.SortCore(s, first, last);
            return;
        }

        var currentBinSizes = binSizes[..binCount];
        currentBinSizes.Clear();
        var bins = binCache.Slice(cacheOffset, binCount);

        // Phase 1: Count elements per bin (Boost: ~10% of runtime)
        s.Context.OnPhase(SortPhase.DistributionCount);
        for (var i = first; i < last; i++)
        {
            var key = radixKey.GetKey(s.Read(i));
            var bin = (int)((long)(key >> logDivisor) - divMin);
            BinAt(currentBinSizes, bin)++;
        }

        // Phase 2: Compute bin positions (prefix sum using absolute indices)
        s.Context.OnPhase(SortPhase.DistributionAccumulate);
        bins[0] = first;
        for (var u = 0; u < binCount - 1; u++)
            BinAt(bins, u + 1) = BinAt(bins, u) + BinAt(currentBinSizes, u);

        // Phase 3: In-place 3-way swap (Boost: dominates runtime, mostly in the swap and
        // bin lookups — hence the bounds-check-free BinAt accessor).
        // Each bin position pointer advances as elements are swapped into place.
        s.Context.OnPhase(SortPhase.DistributionWrite);
        var nextBinStart = first;
        for (var u = 0; u < binCount - 1; u++)
        {
            var localBinPos = BinAt(bins, u);
            nextBinStart += BinAt(currentBinSizes, u);
            for (var current = localBinPos; current < nextBinStart; current++)
            {
                var targetBin = (int)((long)(radixKey.GetKey(s.Read(current)) >> logDivisor) - divMin);
                while (targetBin != u)
                {
                    // 3-way swap: reduces copies per item (Boost: ~1% faster than 2-way)
                    var b = BinAt(bins, targetBin)++;
                    var bBin = (int)((long)(radixKey.GetKey(s.Read(b)) >> logDivisor) - divMin);

                    T tmp;
                    if (bBin != u)
                    {
                        var c = BinAt(bins, bBin)++;
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
            BinAt(bins, u) = nextBinStart;
        }
        bins[binCount - 1] = last;

        // Boost: If we've bucket-sorted (log_divisor == 0), the array is fully sorted
        if (logDivisor == 0)
            return;

        // Boost: get_min_count — dynamic threshold for per-bucket pdqsort fallback
        var maxCount = GetMinCount(logDivisor, logMinSplitCount, logFinishingCount);

        // Phase 4: Recurse on each bin
        var lastPos = first;
        for (var u = cacheOffset; u < cacheEnd; u++)
        {
            var binEnd = binCache[u];
            var binLength = binEnd - lastPos;
            lastPos = binEnd;

            if (binLength < 2)
                continue;

            // Boost: use pdqsort if its worst-case is better for this bin.
            // No separate insertion-sort cutoff here: PDQSort already switches to insertion sort
            // below its own threshold and reports the HybridToInsertionSort phase while doing so.
            if (binLength < maxCount)
            {
                PDQSort.SortCore(s, binEnd - binLength, binEnd);
            }
            else
            {
                SpreadSortRec(s, radixKey, binEnd - binLength, binEnd, binCache, cacheEnd, binSizes,
                              logMinSplitCount, logFinishingCount);
            }
        }
    }

    /// <summary>
    /// Bounds-check-free access to a bin slot. Bin indices are derived from radix keys that the
    /// comparer-based min/max search has already bounded (see the comparer/key agreement invariant),
    /// so the index is always within the span. DEBUG keeps the bounds check so a violated invariant
    /// surfaces as an exception in tests rather than as silent corruption; this mirrors
    /// <see cref="SortSpan{T, TComparer, TContext}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static ref int BinAt(Span<int> bins, int index)
    {
#if DEBUG
        return ref bins[index];
#else
        return ref Unsafe.Add(ref MemoryMarshal.GetReference(bins), (nint)(uint)index);
#endif
    }

    /// <summary>
    /// Boost: is_sorted_or_find_extremes — combined sorted check and min/max finding.
    /// Returns true if NOT sorted (i.e., needs sorting). Returns false if already sorted.
    /// </summary>
    /// <remarks>
    /// <para>The sorted check stays comparison-based, exactly as in Boost. That keeps
    /// <see cref="SortSpan{T, TComparer, TContext}"/>'s primitive specialization and, more
    /// importantly, keeps the loop free of a loop-carried dependency: both operands are loaded
    /// independently each iteration, so it pipelines. It is safe to decide "already sorted" this way
    /// because after the NaN pre-pass the comparer order and the key order coincide: NaN was the last
    /// place they differed, once the key selectors stopped separating <c>-0.0</c> from <c>+0.0</c>.</para>
    /// <para>The extremes are a different matter — they must be the extremes <em>by key</em> (see the
    /// class remarks), and the comparison walk cannot supply them, so an unsorted range is scanned
    /// once more for min/max by key. This costs a pass only on input that is actually going to be
    /// distributed; already-sorted input still returns after the comparison walk alone.</para>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsSortedOrFindExtremes<T, TRadixKey, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        TRadixKey radixKey,
        int first, int last,
        out ulong minKey, out ulong maxKey)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Walk sorted prefix: advance while next element >= current
        var current = first;
        while (s.IsGreaterOrEqualAt(current + 1, current))
        {
            if (++current == last - 1)
            {
                minKey = 0;
                maxKey = 0;
                return false; // Entire range is sorted
            }
        }

        // Not sorted. Find the true extremes by key.
        minKey = ulong.MaxValue;
        maxKey = ulong.MinValue;
        var minIdx = first;
        var maxIdx = first;
        for (var i = first; i < last; i++)
        {
            var key = radixKey.GetKey(s.Read(i));
            ReportKeyCompare(s, i, minIdx, key, minKey);
            if (key < minKey)
            {
                minKey = key;
                minIdx = i;
            }
            ReportKeyCompare(s, i, maxIdx, key, maxKey);
            if (key > maxKey)
            {
                maxKey = key;
                maxIdx = i;
            }
        }

        return true; // Not sorted, needs sorting
    }

    /// <summary>
    /// Reports a key comparison to the context so observers still see the work this pass does.
    /// Element reads are already reported by <see cref="SortSpan{T, TComparer, TContext}.Read"/>.
    /// The whole body — including the index bookkeeping feeding it — folds away under NullContext.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void ReportKeyCompare<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, int i, int j, ulong keyI, ulong keyJ)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (typeof(TContext) != typeof(NullContext))
        {
            s.Context.OnCompare(s.Offset + i, s.Offset + j, keyI.CompareTo(keyJ), s.BufferId, s.BufferId);
        }
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
    /// <remarks>
    /// Boost templates this on <c>log_min_split_count</c> and <c>log_finishing_count</c> and
    /// instantiates it twice, with <c>int_*</c> constants for <c>integer_sort</c> and <c>float_*</c>
    /// constants for <c>float_sort</c>. Both are passed here as arguments; the function runs once per
    /// recursion level, not per element, so there is nothing to gain from folding them.
    /// </remarks>
    static int GetMinCount(int logRange, int logMinSplitCount, int logFinishingCount)
    {
        var minSize = LogMeanBinSize + logMinSplitCount; // integer: 2 + 9 = 11, float: 2 + 8 = 10

        // Boost: if we can complete in one iteration, do so. Reaching this means the bin still has
        // more elements than the number of distinct values its remaining range can hold, so one more
        // distribution pass bucket-sorts it outright. Boost disables the whole block for
        // integer_sort by setting int_log_finishing_count (31) above min_size (11) — there pdqsort
        // is the better finisher — and enables it for float_sort via float_log_finishing_count (4).
        if (logFinishingCount < minSize)
        {
            if (logRange <= minSize && logRange <= MaxSplits)
            {
                // Return no smaller than a certain minimum limit
                if (logRange <= logFinishingCount)
                    return 1 << logFinishingCount;
                return 1 << logRange;
            }
        }

        var baseIterations = MaxSplits - logMinSplitCount; // integer: 11 - 9 = 2, float: 11 - 8 = 3
        // sum of n to n + x = ((x + 1) * (n + (n + x)))/2 + log_mean_bin_size
        var baseRange = ((baseIterations + 1) * (MaxSplits + logMinSplitCount)) / 2
                        + LogMeanBinSize; // integer: 32, float: 40

        if (logRange < baseRange)
        {
            var result = logMinSplitCount;
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
