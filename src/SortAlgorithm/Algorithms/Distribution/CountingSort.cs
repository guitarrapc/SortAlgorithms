using SortAlgorithm.Contexts;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 値の分布状況を数え上げることを利用してインデックスを導きソートします。
/// 各要素からキーを抽出し、その出現回数をカウントして累積和を計算し、正しい位置に配置する安定なソートアルゴリズムです。
/// キーの範囲が狭い場合に非常に高速ですが、範囲が広いとメモリを大量に消費します。
/// <br/>
/// Sorts elements by counting the distribution of extracted keys.
/// A stable sorting algorithm that extracts keys, counts occurrences, and uses cumulative sums to place elements.
/// Very fast when the key range is narrow, but consumes significant memory for wide ranges.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Counting Sort (Generic, Key-based):</strong></para>
/// <list type="number">
/// <item><description><strong>Key Extraction:</strong> Each element must have a deterministic integer key obtained via the key selector function.
/// The key must be stable (same element always produces the same key).</description></item>
/// <item><description><strong>Range Determination:</strong> The algorithm finds min and max keys to determine the range [min, max].
/// A count array of size (max - min + 1) is allocated to track occurrences.</description></item>
/// <item><description><strong>Index Normalization:</strong> Keys are normalized by subtracting min (<c>key - min</c>), mapping keys to array indices [0, range-1].
/// This is safe even when min == int.MinValue, because the validated range guarantees the difference fits in an int.</description></item>
/// <item><description><strong>Counting Phase:</strong> For each element, its key is extracted and <c>countArray[key - min]</c> is incremented.
/// This records how many times each key appears.</description></item>
/// <item><description><strong>Cumulative Sum:</strong> The count array is transformed into cumulative counts.
/// countArray[i] becomes the number of elements with keys ≤ i, indicating the final position.</description></item>
/// <item><description><strong>Placement Phase:</strong> Elements are placed in reverse order (for stability).
/// For each element with key k, it is placed at position <c>countArray[k - min] - 1</c>, then the count is decremented.</description></item>
/// <item><description><strong>Stability:</strong> Processing elements in reverse order ensures that elements with equal keys maintain their original relative order.</description></item>
/// <item><description><strong>Range Limitation:</strong> The key range must be reasonable (≤ <c>MaxCountArraySize</c>, 10,000,000).
/// Excessive ranges cause memory allocation failures.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution</description></item>
/// <item><description>Stable      : Yes (reverse-order placement preserves relative order)</description></item>
/// <item><description>In-place    : No (O(n + k) where k = range of keys)</description></item>
/// <item><description>Best case   : O(n + k) - Every input does the same work; no pattern is favourable</description></item>
/// <item><description>Average case: O(n + k) - Linear in input size plus key range</description></item>
/// <item><description>Worst case  : O(n + k) - Independent of how the keys are distributed</description></item>
/// <item><description>Comparisons : 0 - No comparison operations between keys (distribution sort)</description></item>
/// <item><description>Swaps       : 0 - Elements are placed at a computed index, never exchanged</description></item>
/// <item><description>Index Reads : 3n - n to extract the keys, n to place the elements, n for the write-back.
/// Counting runs over the extracted keys, so it does not touch the elements a third time</description></item>
/// <item><description>Index Writes: 2n - n to place each element in the output, n for the write-back. Producing a
/// separate output is inherent to counting sort; returning it to the caller's span is what the in-place API adds,
/// and it is issued as one range copy that an observer expands into n reads and n writes</description></item>
/// <item><description>Space       : O(n + k) - an n-element output plus a k-sized counter array</description></item>
/// </list>
/// <para><strong>Note:</strong> A large key range leads to excessive memory usage. The maximum range is <c>MaxCountArraySize</c> (10,000,000).</para>
/// <para><strong>Comparison with Related Algorithms:</strong></para>
/// <list type="bullet">
/// <item><description>vs <see cref="PigeonholeSort"/>: Both are O(n + k) and both must store the elements, because a
/// key selector leaves an element carrying payload its key does not determine. What differs is the price of not having
/// a prefix sum. Pigeonhole sort computes no position, so it has to remember which elements landed in which hole — a
/// per-element chain on top of the hole array — and collection walks those chains. Counting sort computes the position
/// instead, so a k-sized counter array is the whole auxiliary structure and collection is a linear scan. The two move
/// the elements the same number of times — both are 3n reads / 2n writes — so what counting sort buys here is not
/// fewer operations but a cheaper shape: one fewer n-sized auxiliary array, and a collection pass that runs
/// sequentially instead of chasing a chain pointer per element.</description></item>
/// <item><description>vs <see cref="CountingSortInteger"/>: The integer overload runs the same mechanism and differs
/// only in where the key comes from. Counting sort's auxiliary structure does not change with the element type. That is
/// not true of <see cref="PigeonholeSort"/> and <see cref="PigeonholeSortInteger"/>, whose structures differ because a
/// hole index can reconstruct an integer but not an arbitrary element. The one asymmetry is the read count: extracting
/// a key materializes it, so counting can run over the keys and the elements are read 3n times here against 4n when the
/// element is its own key.</description></item>
/// <item><description>vs <see cref="BucketSort"/>: A counting sort bucket holds exactly one key, so no comparison sort
/// runs inside it. The running time does not depend on how the keys are distributed and there is no O(n²) worst case.
/// The price is that k is the whole key range, so a sparse key range is refused outright rather than sorted more
/// slowly — the case bucket sort exists to cover.</description></item>
/// <item><description>vs Radix sort: Radix sort holds k down to the digit radix and pays one distribution pass per
/// digit, so a wide key range costs passes rather than memory, and it needs a fixed-width digit mapping of the key.
/// Counting sort distributes once against the key itself, which is cheaper while the range stays narrow and impossible
/// once it does not.</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Counting_sort</para>
/// </remarks>
public static class CountingSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>See the stability note in this type's summary.</remarks>
    public static bool IsStable => true;

    private const int MaxCountArraySize = 10_000_000; // Maximum allowed count array size
    private const int StackAllocThreshold = 1024; // Use stackalloc for count arrays smaller than this

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Temporary buffer for sorted elements

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
        try
        {
            // Create SortSpan for temp buffer to track operations
            var tempSpan = new SortSpan<T, NullComparer<T>, TContext>(tempArray.AsSpan(0, span.Length), context, default, BUFFER_TEMP);
            var keys = keysArray.AsSpan(0, span.Length);

            // Find min/max and cache keys in single pass
            var min = int.MaxValue;
            var max = int.MinValue;

            for (var i = 0; i < span.Length; i++)
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
            // Only the absolute cap is enforced here. CountingSortInteger additionally rejects range > MaxRangeFactor*n,
            // because there the values are the data: a range far wider than n means counting sort is the wrong tool for
            // this input and the caller should be told so. A key selector inverts that. The key is a projection the
            // caller chose, so its density is a property of that projection rather than of the data, and a sparse key
            // space may be exactly what the caller intended. Refusing it would reject a legitimate use on a guess about
            // intent, so the generic overload bounds only what it must to allocate safely.
            long range = (long)max - (long)min + 1;
            if (range > MaxCountArraySize)
                throw new ArgumentException($"Key range ({range}) exceeds maximum count array size ({MaxCountArraySize}). Consider using another comparison-based sort.");

            var size = (int)range;

            // Use stackalloc for small count arrays, ArrayPool for larger ones
            int[]? rentedCountArray = null;
            Span<int> countArray = size <= StackAllocThreshold
                ? stackalloc int[size]
                : (rentedCountArray = ArrayPool<int>.Shared.Rent(size)).AsSpan(0, size);
            countArray.Clear(); // Required: neither branch yields zeroed memory - [module: SkipLocalsInit] skips it for stackalloc, and a pooled array carries its previous contents
            try
            {
                CountSort(s, keys, tempSpan, countArray, min);
            }
            finally
            {
                if (rentedCountArray is not null)
                {
                    ArrayPool<int>.Shared.Return(rentedCountArray);
                }
            }
        }
        finally
        {
            ArrayPool<int>.Shared.Return(keysArray);
            ArrayPool<T>.Shared.Return(tempArray, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    /// <summary>
    /// Core counting sort implementation.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CountSort<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, Span<int> keys, SortSpan<T, TComparer, TContext> tempSpan, Span<int> countArray, int min)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Count occurrences of each key
        s.Context.OnPhase(SortPhase.DistributionCount);
        for (var i = 0; i < s.Length; i++)
        {
            countArray[keys[i] - min]++;
        }

        // Calculate cumulative counts (for stable sort)
        s.Context.OnPhase(SortPhase.DistributionAccumulate);
        for (var i = 1; i < countArray.Length; i++)
        {
            countArray[i] += countArray[i - 1];
        }

        // Build result array in reverse order to maintain stability
        s.Context.OnPhase(SortPhase.DistributionWrite);
        for (var i = s.Length - 1; i >= 0; i--)
        {
            var key = keys[i];
            var index = key - min;
            var pos = countArray[index] - 1;
            tempSpan.Write(pos, s.Read(i));
            countArray[index]--;
        }

        // Write sorted data back to original span using CopyTo for efficiency
        tempSpan.CopyTo(0, s, 0, s.Length);
    }
}

/// <summary>
/// 整数値を直接カウンティングソートでソートします。
/// 各値の出現回数をカウントし、累積和を計算して正しい位置に配置する安定なソートアルゴリズムです。
/// 値の範囲が狭い場合に非常に高速ですが、範囲が広いとメモリを大量に消費します。
/// <br/>
/// Directly sorts integer values using counting sort.
/// A stable sorting algorithm that counts occurrences and uses cumulative sums to place elements.
/// Very fast when the value range is narrow, but consumes significant memory for wide ranges.
/// </summary>
/// <remarks>
/// <para><strong>Supported Types:</strong></para>
/// <list type="bullet">
/// <item><description><strong>Supported:</strong> byte, sbyte, short, ushort, int, uint, long, ulong, nint, nuint (up to 64-bit)</description></item>
/// <item><description><strong>Not Supported:</strong> Int128, UInt128, BigInteger (>64-bit types)</description></item>
/// </list>
/// <para><strong>Why Int128/UInt128 are not supported:</strong></para>
/// <para>The value range for 128-bit types can reach 2^128, making the count array impractically large.
/// If you need to sort Int128/UInt128, consider using a comparison-based sort.</para>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution</description></item>
/// <item><description>Stable      : Yes</description></item>
/// <item><description>In-place    : No (O(n + k) where k = range of values)</description></item>
/// <item><description>Best case   : O(n + k) - Every input does the same work; no pattern is favourable</description></item>
/// <item><description>Average case: O(n + k) - Linear in input size plus value range</description></item>
/// <item><description>Worst case  : O(n + k) - Independent of how the values are distributed</description></item>
/// <item><description>Comparisons : 2n+1 (n×2 for min/max scan, +1 for early-exit equality check) - the ordering
/// step is comparison-free, but discovering the range is not, and the scan is real work the sort performs</description></item>
/// <item><description>Swaps       : 0 - Elements are placed at a computed index, never exchanged</description></item>
/// <item><description>Index Reads : 4n - n to discover the range, n to count, n to place, n for the write-back. An
/// element is its own key, so there is nothing to extract once and reuse; every pass reads the elements themselves</description></item>
/// <item><description>Index Writes: 2n - n to place each element in the output, n for the write-back. Producing a
/// separate output is inherent to counting sort; returning it to the caller's span is what the in-place API adds,
/// and it is issued as one range copy that an observer expands into n reads and n writes</description></item>
/// <item><description>Space       : O(n + k) - an n-element output plus a k-sized counter array</description></item>
/// </list>
/// <para><strong>Note:</strong> 値の範囲が大きいとメモリ使用量が膨大になります。最大範囲は <c>MaxCountArraySize</c> (10,000,000)、かつ range/n ≤ <c>MaxRangeFactor</c> (32) の制約があります。</para>
/// <para><strong>Comparison with Related Algorithms:</strong></para>
/// <list type="bullet">
/// <item><description>vs <see cref="PigeonholeSortInteger"/>: Turning counts into cumulative offsets and placing each
/// element at a computed index is counting sort's defining step. Removing it for plain integers is possible, but what
/// remains is pigeonhole sort rather than a faster counting sort, so this overload keeps the prefix sum, the second
/// read pass and the O(n) output buffer that pigeonhole sort's integer overload does without. Note also that the two
/// counting sort overloads share one mechanism while pigeonhole sort's two do not: a hole index reconstructs an integer
/// but not an arbitrary element. Where this library draws that boundary is recorded in
/// <c>.github/docs/specs/sorting_api.md</c>.</description></item>
/// <item><description>vs <see cref="BucketSortInteger"/>: One counter per distinct value means no comparison sort
/// runs inside a bucket. The running time does not depend on how the values are distributed and there is no O(n²)
/// worst case. The price is that k is the whole value range, which is why the range limits above exist at all; bucket
/// sort sizes its buckets by n instead and therefore accepts the sparse inputs this overload rejects.</description></item>
/// <item><description>vs Radix sort: Radix sort holds k down to the digit radix and pays one distribution pass per
/// digit, so a wide value range costs passes rather than memory. Counting sort distributes once against the value
/// itself, which is cheaper while the range stays narrow and impossible once it does not.</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Counting_sort</para>
/// </remarks>
public static class CountingSortInteger
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>See the stability note in this type's summary.</remarks>
    public static bool IsStable => true;

    private const int MaxCountArraySize = 10_000_000; // Maximum allowed count array size
    private const int MaxRangeFactor = 32;            // Maximum allowed range/n ratio; range > MaxRangeFactor*n means O(range) dominates O(n)
    private const int StackAllocThreshold = 1024;     // Use stackalloc for count arrays smaller than this

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Temporary buffer for sorted elements

    /// <summary>
    /// Sorts the elements in the specified span in ascending order.
    /// Uses NullContext for zero-overhead fast path.
    /// </summary>
    /// <typeparam name="T"> The type of elements to sort. Must be a binary integer type with defined min/max values.</typeparam>
    /// <param name="span"> The span of elements to sort.</param>
    public static void Sort<T>(Span<T> span) where T : IBinaryInteger<T>, IMinMaxValue<T>
        => Sort(span, NullContext.Default);

    /// <summary>
    /// Sorts the elements in the specified span using the specified context.
    /// </summary>
    /// <typeparam name="T"> The type of elements to sort. Must be a binary integer type with defined min/max values.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="span"> The span of elements to sort.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation.</param>
    public static void Sort<T, TContext>(Span<T> span, TContext context)
        where T : IBinaryInteger<T>, IMinMaxValue<T>, IComparisonOperators<T, T, bool>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;

        EnsureSupportedType<T>();

        var comparer = new NumberComparer<T>();
        var s = new SortSpan<T, NumberComparer<T>, TContext>(span, context, comparer, BUFFER_MAIN);

        var tempArray = ArrayPool<T>.Shared.Rent(span.Length);
        try
        {
            var tempSpan = new SortSpan<T, NumberComparer<T>, TContext>(tempArray.AsSpan(0, span.Length), context, comparer, BUFFER_TEMP);
            // Find min and max to determine range.
            // The scan runs through the observable accessors: these comparisons are work the sort really
            // performs, and a comparer call made directly would leave no trace of it in the stream. That the
            // ordering step of counting sort is comparison-free does not make the range scan free.
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
            if (range == 0 || range > (ulong)MaxCountArraySize)
                throw new ArgumentException($"Value range ({range}) exceeds maximum count array size ({MaxCountArraySize}). Consider another comparison-based sort.");
            if (range > (ulong)s.Length * MaxRangeFactor)
                throw new ArgumentException($"Value range ({range}) is too large relative to array size ({s.Length}): range/n={range}/{(ulong)s.Length} exceeds limit of {MaxRangeFactor}. Consider another comparison-based sort.");

            var size = (int)range;

            // Use stackalloc for small count arrays, ArrayPool for larger ones
            int[]? rentedCountArray = null;
            Span<int> countArray = size <= StackAllocThreshold
                ? stackalloc int[size]
                : (rentedCountArray = ArrayPool<int>.Shared.Rent(size)).AsSpan(0, size);
            countArray.Clear(); // Required: neither branch yields zeroed memory - [module: SkipLocalsInit] skips it for stackalloc, and a pooled array carries its previous contents
            try
            {
                CountSort(s, tempSpan, countArray, umin);
            }
            finally
            {
                if (rentedCountArray is not null)
                {
                    ArrayPool<int>.Shared.Return(rentedCountArray);
                }
            }
        }
        finally
        {
            ArrayPool<T>.Shared.Return(tempArray, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CountSort<T, TContext>(SortSpan<T, NumberComparer<T>, TContext> s, SortSpan<T, NumberComparer<T>, TContext> tempSpan, Span<int> countArray, ulong umin)
        where T : IBinaryInteger<T>, IComparisonOperators<T, T, bool>
        where TContext : ISortContext
    {
        // Count occurrences
        s.Context.OnPhase(SortPhase.DistributionCount);
        for (var i = 0; i < s.Length; i++)
        {
            var value = s.Read(i);
            var index = (int)(ulong.CreateTruncating(value) - umin);
            countArray[index]++;
        }

        // Calculate cumulative counts (for stable sort)
        s.Context.OnPhase(SortPhase.DistributionAccumulate);
        for (var i = 1; i < countArray.Length; i++)
        {
            countArray[i] += countArray[i - 1];
        }

        // Build result array in reverse order to maintain stability
        s.Context.OnPhase(SortPhase.DistributionWrite);
        for (var i = s.Length - 1; i >= 0; i--)
        {
            var value = s.Read(i);
            var index = (int)(ulong.CreateTruncating(value) - umin);
            var pos = countArray[index] - 1;
            tempSpan.Write(pos, value);
            countArray[index]--;
        }

        // Write sorted data back to original span using CopyTo for efficiency
        tempSpan.CopyTo(0, s, 0, s.Length);
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
