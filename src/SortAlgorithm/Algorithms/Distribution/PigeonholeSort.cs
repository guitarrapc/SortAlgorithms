using SortAlgorithm.Contexts;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// ピジョンホールソート（鳩の巣ソート）は、分布ソートの一種で、各値を対応する「穴」（バケット）に配置してソートします。
/// キーの範囲が狭い場合に非常に高速ですが、範囲が広いとメモリを大量に消費します。
/// <br/>
/// Pigeonhole sort is a distribution sort that places each value into its corresponding "hole" (bucket) for sorting.
/// Very fast when the key range is narrow, but consumes significant memory for wide ranges.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Pigeonhole Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Key Extraction:</strong> Each element must have a deterministic integer key obtained via the key selector function.
/// The key must be stable (same element always produces the same key).</description></item>
/// <item><description><strong>Range Determination:</strong> The algorithm finds min and max keys to determine the range [min, max].
/// A hole array of size (max - min + 1) is allocated. Each index corresponds to one unique key value.</description></item>
/// <item><description><strong>Base Key Normalization:</strong> Keys are normalized by subtracting baseKey (= min), mapping keys to array indices [0, range-1].
/// This allows handling negative keys correctly: holes[key - baseKey] maps key to its hole.</description></item>
/// <item><description><strong>Distribution Phase:</strong> Each element is placed into its corresponding hole using a linked list.
/// Elements are scanned from the last index down to the first, and index i is prepended to hole[k - baseKey].
/// Each hole stores only a head index into the temp array; a next[] array chains the remaining elements within each hole.
/// Prepending during a backward scan leaves every chain in ascending index order, which is what a forward scan
/// appending to a tail would produce, so no per-hole tail index is needed.</description></item>
/// <item><description><strong>Collection Phase:</strong> Iterate through holes array in ascending order.
/// For each hole i, traverse its linked list (head → next → … → -1) and write elements back to the source array.
/// This reconstructs the sorted sequence in O(n + k) time.</description></item>
/// <item><description><strong>Correctness Guarantee:</strong> Since holes are traversed in index order (0 to range-1),
/// and each index i corresponds to key (i + baseKey), elements are written back in ascending key order.
/// The algorithm correctly sorts as long as the key selector function produces consistent integer keys.</description></item>
/// <item><description><strong>Stability:</strong> This implementation IS stable because each hole's chain holds its
/// elements in ascending input index order and is collected head-first.</description></item>
/// <item><description><strong>Range Limitation:</strong> The key range must be reasonable (≤ <c>MaxHoleArraySize</c>, 10,000,000).
/// Excessive ranges cause memory allocation failures or out-of-memory errors.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution</description></item>
/// <item><description>Stable      : Yes (preserves relative order of elements with equal keys)</description></item>
/// <item><description>In-place    : No (O(n + k) auxiliary space where k = range of keys)</description></item>
/// <item><description>Best case   : O(n + k) - All cases have the same complexity</description></item>
/// <item><description>Average case: O(n + k) - Linear in input size plus key range</description></item>
/// <item><description>Worst case  : O(n + k) - Even with all elements having different keys</description></item>
/// <item><description>Comparisons : 0 - No comparison operations between keys (distribution sort)</description></item>
/// <item><description>IndexReads  : 3n - n reads for key extraction, n reads for copying to temp, n reads for writing back</description></item>
/// <item><description>IndexWrites : 2n - n writes to temp, n writes back to original array</description></item>
/// <item><description>Memory      : O(n + k) - Temporary arrays for elements, keys, next[] links, and hole head indices</description></item>
/// <item><description>Note        : A large key range leads to excessive memory usage. The maximum range is <c>MaxHoleArraySize</c> (10,000,000).</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Pigeonhole_sort</para>
/// </remarks>
public static class PigeonholeSort
{
    private const int MaxHoleArraySize = 10_000_000; // Maximum allowed hole array size
    private const int StackAllocThreshold = 1024; // Use stackalloc for the holeHead array when range is smaller than this

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Temporary buffer for elements

    /// <summary>
    /// Sorts the elements in the specified span in ascending order using the key selector.
    /// Uses NullContext for zero-overhead fast path.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <param name="span">The span of elements to sort in place.</param>
    public static void SortBy<T>(Span<T> span, Func<T, int> keySelector)
    {
        ArgumentNullException.ThrowIfNull(keySelector);
        SortCore(span, new FuncKeySelector<T>(keySelector), NullContext.Default);
    }

    /// <summary>
    /// Sorts the elements in the specified span using the key selector and sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void SortBy<T, TContext>(Span<T> span, Func<T, int> keySelector, TContext context)
        where TContext : ISortContext
    {
        ArgumentNullException.ThrowIfNull(keySelector);
        SortCore(span, new FuncKeySelector<T>(keySelector), context);
    }

    private static void SortCore<T, TKeySelector, TContext>(Span<T> span, TKeySelector keySelector, TContext context)
        where TKeySelector : struct, IKeySelector<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T, NullComparer<T>, TContext>(span, context, default, BUFFER_MAIN);

        // Rent arrays from ArrayPool for temporary storage
        var keysArray = ArrayPool<int>.Shared.Rent(span.Length);
        var tempArray = ArrayPool<T>.Shared.Rent(span.Length);
        var nextArray = ArrayPool<int>.Shared.Rent(span.Length);
        try
        {
            // Create SortSpan for temp buffer to track operations
            var tempSpan = new SortSpan<T, NullComparer<T>, TContext>(tempArray.AsSpan(0, span.Length), context, default, BUFFER_TEMP);
            var keys = keysArray.AsSpan(0, span.Length);
            var next = nextArray.AsSpan(0, span.Length);

            // Find min/max and cache keys in single pass
            var min = int.MaxValue;
            var max = int.MinValue;

            for (var i = 0; i < s.Length; i++)
            {
                var key = keySelector.GetKey(s.Read(i));
                keys[i] = key;
                if (key < min) min = key;
                if (key > max) max = key;
            }

            // If all keys are the same, no need to sort
            if (min == max) return;

            // Validate range. Computed in long so the full int key space cannot overflow the subtraction;
            // the cap below is far under int.MaxValue, so it is the only bound that has to be tested.
            //
            // Only the absolute cap is enforced here. PigeonholeSortInteger additionally rejects range > MaxRangeFactor*n,
            // because there the values are the data: a range far wider than n means pigeonhole sort is the wrong tool for
            // this input and the caller should be told so. A key selector inverts that. The key is a projection the
            // caller chose, so its density is a property of that projection rather than of the data, and a sparse key
            // space may be exactly what the caller intended. Refusing it would reject a legitimate use on a guess about
            // intent, so the generic overload bounds only what it must to allocate safely.
            long range = (long)max - (long)min + 1;
            if (range > MaxHoleArraySize)
                throw new ArgumentException($"Key range ({range}) exceeds maximum hole array size ({MaxHoleArraySize}). Consider using another comparison-based sort.");

            var baseKey = (long)min; // long avoids -int.MinValue overflow; subtraction is safe after range validation
            var size = (int)range;

            // Each hole is a linked list of element indices: holeHead[h] = first element index (-1 = empty).
            // next[] chains the rest; the backward distribution scan keeps each chain in ascending index order.
            int[]? rentedHoleHeadArray = null;
            Span<int> holeHead = size <= StackAllocThreshold
                ? stackalloc int[size]
                : (rentedHoleHeadArray = ArrayPool<int>.Shared.Rent(size)).AsSpan(0, size);
            holeHead.Fill(-1); // Required: an empty hole must read as -1, and [module: SkipLocalsInit] leaves stackalloc uninitialized
            try
            {
                DistributeAndCollect(s, tempSpan, keys, holeHead, next, baseKey);
            }
            finally
            {
                if (rentedHoleHeadArray is not null)
                    ArrayPool<int>.Shared.Return(rentedHoleHeadArray);
            }
        }
        finally
        {
            ArrayPool<int>.Shared.Return(keysArray);
            ArrayPool<T>.Shared.Return(tempArray, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
            ArrayPool<int>.Shared.Return(nextArray);
        }
    }

    /// <summary>
    /// Distributes elements into linked-list holes (phase 1), then collects them back in hole index order (phase 2).
    /// Achieves O(n + k) complexity; no prefix-sum phase.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DistributeAndCollect<T, TContext>(SortSpan<T, NullComparer<T>, TContext> source, SortSpan<T, NullComparer<T>, TContext> temp, Span<int> keys, Span<int> holeHead, Span<int> next, long baseKey)
        where TContext : ISortContext
    {
        // Phase 1: Copy elements to temp and prepend each to its hole's linked list (O(n)).
        // Scanning backwards and prepending leaves every chain in ascending index order, which is
        // what a forward scan appending to a tail would produce - so stability holds without a
        // per-hole tail index, and the loop drops one load, one branch and one indirect store.
        source.Context.OnPhase(SortPhase.DistributionCount);
        for (var i = source.Length - 1; i >= 0; i--)
        {
            temp.Write(i, source.Read(i));
            var h = (int)(keys[i] - baseKey);
            next[i] = holeHead[h];
            holeHead[h] = i;
        }

        // Phase 2: Collect elements from holes in ascending key order (O(n + k))
        source.Context.OnPhase(SortPhase.DistributionWrite);
        var pos = 0;
        for (var h = 0; h < holeHead.Length; h++)
        {
            var j = holeHead[h];
            while (j != -1)
            {
                source.Write(pos++, temp.Read(j));
                j = next[j];
            }
        }
    }
}

/// <summary>
/// 整数値を直接ピジョンホールソートでソートします。
/// 各値を対応する「穴」（バケット）に配置してソートする、安定なソートアルゴリズムです。
/// 値の範囲が狭い場合に非常に高速ですが、範囲が広いとメモリを大量に消費します。
/// <br/>
/// Directly sorts integer values using pigeonhole sort.
/// A stable sorting algorithm that places each value into its corresponding "hole" (bucket).
/// Very fast when the value range is narrow, but consumes significant memory for wide ranges.
/// </summary>
/// <remarks>
/// <para><strong>Supported Types:</strong></para>
/// <list type="bullet">
/// <item><description><strong>Supported:</strong> byte, sbyte, short, ushort, int, uint, long, ulong, nint, nuint (up to 64-bit)</description></item>
/// <item><description><strong>Not Supported:</strong> Int128, UInt128, BigInteger (>64-bit types)</description></item>
/// </list>
/// <para><strong>Why Int128/UInt128 are not supported:</strong></para>
/// <para>The value range for 128-bit types can reach 2^128, making the hole array impractically large.
/// If you need to sort Int128/UInt128, consider using a comparison-based sort.</para>
/// <para><strong>What a hole stores:</strong></para>
/// <para>A hole records how many elements it received, not the elements themselves. The hole index already
/// determines the value it holds (<c>value == h + min</c>), so two integers sharing a hole are equal and
/// therefore indistinguishable: storing them would record nothing the index does not already say. This is
/// what the integer specialization buys, and it removes the auxiliary element buffer entirely — the sort
/// touches only the input array and one k-sized hole array.</para>
/// <para>
/// <see cref="PigeonholeSort"/> cannot do this. There an element carries payload beyond its key, so the
/// holes must hold the elements and the sort needs O(n) auxiliary space. Both are pigeonhole sort; they
/// differ only in how much of an element the hole index can reconstruct.
/// </para>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution</description></item>
/// <item><description>Stable      : Yes, vacuously — equal integers are indistinguishable, so no relative order is observable</description></item>
/// <item><description>In-place    : No (O(k) where k = range of values; independent of n)</description></item>
/// <item><description>Best case   : O(n + k) - All cases have the same complexity</description></item>
/// <item><description>Average case: O(n + k) - Linear in input size plus value range</description></item>
/// <item><description>Worst case  : O(n + k) - Even with all elements having different values</description></item>
/// <item><description>Comparisons : 2n+1 (n×2 for min/max scan, +1 for early-exit equality check)</description></item>
/// <item><description>Swaps       : 0</description></item>
/// <item><description>IndexReads  : 2n - n for the min/max scan, n for the distribution pass</description></item>
/// <item><description>IndexWrites : n - each element is written once, directly into its final position</description></item>
/// <item><description>Memory      : O(k) - one hole array; no per-element auxiliary storage</description></item>
/// <item><description>Note        : 値の範囲が大きいとメモリ使用量が膨大になります。最大範囲は <c>MaxHoleArraySize</c> (10,000,000)、かつ range/n ≤ <c>MaxRangeFactor</c> (32) の制約があります。</description></item>
/// </list>
/// <para><strong>Comparison with Related Algorithms:</strong></para>
/// <list type="bullet">
/// <item><description>vs <see cref="CountingSortInteger"/>: Counting sort turns counts into cumulative offsets and then
/// places each element at a computed index, which requires reading the input a second time and an O(n) output
/// buffer. Pigeonhole sort never computes a position: it walks the holes in order and emits. That removes the
/// prefix-sum pass, one full read of the input and the O(n) buffer.</description></item>
/// <item><description>vs <see cref="BucketSortInteger"/>: A pigeonhole is one bucket per distinct value, so no
/// comparison sort runs inside a bucket. Performance does not depend on how the values are distributed and there
/// is no O(n²) worst case; the cost is that k is the whole value range rather than a chosen bucket count.</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Pigeonhole_sort</para>
/// </remarks>
public static class PigeonholeSortInteger
{
    private const int MaxHoleArraySize = 10_000_000; // Maximum allowed hole array size
    private const int MaxRangeFactor = 32;           // Maximum allowed range/n ratio; range > MaxRangeFactor*n means O(range) dominates O(n)
    private const int StackAllocThreshold = 1024; // Use stackalloc for the hole array when range is smaller than this

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array (the only buffer: holes hold occupancy, not elements)

    /// <summary>
    /// Sorts the elements in the specified span in ascending order.
    /// Uses NullContext for zero-overhead fast path.
    /// </summary>
    /// <typeparam name="T"> The type of elements to sort. Must be a binary integer type with defined min/max values.</typeparam>
    /// <param name="span"> The span of elements to sort.</param>
    public static void Sort<T>(Span<T> span) where T : IBinaryInteger<T>, IMinMaxValue<T>
        => Sort(span, NullContext.Default);

    /// <summary>
    /// Sorts integer values in the specified span with sort context.
    /// Always sorts in ascending numeric order (<see cref="IComparable{T}"/> natural order).
    /// Arbitrary sort order via a comparer is not supported.
    /// </summary>
    /// <typeparam name="T"> The type of elements to sort. Must be a binary integer type with defined min/max values.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="span"> The span of elements to sort.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation.</param>
    public static void Sort<T, TContext>(Span<T> span, TContext context)
        where T : IBinaryInteger<T>, IMinMaxValue<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;

        EnsureSupportedType<T>();

        var comparer = new NumberComparer<T>();
        var s = new SortSpan<T, NumberComparer<T>, TContext>(span, context, comparer, BUFFER_MAIN);

        SortCore(s);
    }

    private static void SortCore<T, TContext>(SortSpan<T, NumberComparer<T>, TContext> s)
        where T : IBinaryInteger<T>, IMinMaxValue<T>, IComparisonOperators<T, T, bool>
        where TContext : ISortContext
    {
        // Find min and max to determine range
        var minValue = T.MaxValue;
        var maxValue = T.MinValue;

        for (var i = 0; i < s.Length; i++)
        {
            var value = s.Read(i);
            if (s.IsLessThan(value, minValue)) minValue = value;
            if (s.IsGreaterThan(value, maxValue)) maxValue = value;
        }

        // If all elements are the same, no need to sort
        if (s.Compare(minValue, maxValue) == 0) return;

        // Use ulong arithmetic for range calculation to correctly handle all supported types
        // including ulong and nuint. ulong.CreateTruncating preserves 2's complement bit patterns
        // for signed types, so wrapping ulong subtraction gives the correct element count for both
        // signed and unsigned types.
        var umin = ulong.CreateTruncating(minValue);
        var umax = ulong.CreateTruncating(maxValue);

        // range == 0 means overflow (actual range is 2^64), which implies an enormous value range
        ulong range = umax - umin + 1;
        if (range == 0 || range > (ulong)MaxHoleArraySize)
            throw new ArgumentException($"Value range ({range}) exceeds maximum hole array size ({MaxHoleArraySize}). Consider using another comparison-based sort.");
        // A hole array far larger than the input makes the collection scan, not the elements, the cost of the sort.
        if (range > (ulong)s.Length * MaxRangeFactor)
            throw new ArgumentException($"Value range ({range}) is too large relative to array size ({s.Length}): range/n={range}/{(ulong)s.Length} exceeds limit of {MaxRangeFactor}. Consider another comparison-based sort.");

        var size = (int)range;

        // A hole holds how many elements it received. For a plain integer the hole index already
        // determines the value it holds (value == h + min), so storing the elements themselves would
        // record nothing the index does not: two integers in the same hole are equal and therefore
        // indistinguishable. That is what removes the n-sized auxiliary buffers entirely.
        int[]? rentedHoles = null;
        Span<int> holes = size <= StackAllocThreshold
            ? stackalloc int[size]
            : (rentedHoles = ArrayPool<int>.Shared.Rent(size)).AsSpan(0, size);
        holes.Clear(); // Required: neither branch yields zeroed memory - [module: SkipLocalsInit] skips it for stackalloc, and a pooled array carries its previous contents
        try
        {
            DistributeAndCollect(s, holes, umin);
        }
        finally
        {
            if (rentedHoles is not null)
                ArrayPool<int>.Shared.Return(rentedHoles);
        }
    }

    /// <summary>
    /// Drops every element into its hole (phase 1), then empties the holes back into the array in
    /// ascending hole order (phase 2). O(n + k); no prefix sum and no auxiliary element buffer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DistributeAndCollect<T, TContext>(SortSpan<T, NumberComparer<T>, TContext> s, Span<int> holes, ulong umin)
        where T : IBinaryInteger<T>, IComparisonOperators<T, T, bool>
        where TContext : ISortContext
    {
        // Phase 1: drop each element into the hole its value selects (O(n)).
        s.Context.OnPhase(SortPhase.DistributionCount);
        for (var i = 0; i < s.Length; i++)
        {
            holes[(int)(ulong.CreateTruncating(s.Read(i)) - umin)]++;
        }

        // Phase 2: empty the holes back into the array in ascending order (O(n + k)).
        // Hole h holds elements whose value is h + min, recovered by reversing the phase 1 mapping:
        // umin + h wraps in ulong exactly as the value's own arithmetic does, and CreateTruncating
        // narrows the 2's complement bit pattern back to T for signed and unsigned types alike.
        s.Context.OnPhase(SortPhase.DistributionWrite);
        var pos = 0;
        for (var h = 0; h < holes.Length; h++)
        {
            var count = holes[h];
            if (count == 0) continue;

            var value = T.CreateTruncating(umin + (ulong)h);
            do
            {
                s.Write(pos++, value);
            } while (--count > 0);
        }
    }

    /// <summary>
    /// Throws <see cref="NotSupportedException"/> if <typeparamref name="T"/> is not supported.
    /// Supported types: sbyte, byte, short, ushort, int, uint, long, ulong, nint, nuint.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void EnsureSupportedType<T>() where T : IBinaryInteger<T>
    {
        if (typeof(T) == typeof(byte) || typeof(T) == typeof(sbyte) ||
            typeof(T) == typeof(short) || typeof(T) == typeof(ushort) ||
            typeof(T) == typeof(int) || typeof(T) == typeof(uint) ||
            typeof(T) == typeof(long) || typeof(T) == typeof(ulong) ||
            typeof(T) == typeof(nint) || typeof(T) == typeof(nuint))
            return;
        if (typeof(T) == typeof(Int128) || typeof(T) == typeof(UInt128))
            throw new NotSupportedException($"Type {typeof(T).Name} with 128-bit size is not supported. Maximum supported bit size is 64.");
        throw new NotSupportedException($"Type {typeof(T).Name} is not supported.");
    }

}
