using SortAlgorithm.Contexts;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// キー選択関数を使用したバケットソートのジェネリック版。値が均等に分布している場合に最適に動作します。
/// 値域を複数のバケットに分割し、各要素をキーに基づいてバケットに配置します。
/// 各バケット内をソートした後、バケットを順番に連結すればソートが完了します。
/// <br/>
/// Bucket sort with key projection (int key selector), a generic bucket sort algorithm that performs optimally when values are uniformly distributed.
/// Divides the value range into multiple buckets and distributes elements based on their keys.
/// After sorting each bucket, concatenating them in order completes the sort.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Bucket Sort (Generic, Range-based):</strong></para>
/// <list type="number">
/// <item><description><strong>Key Extraction:</strong> Each element must have a deterministic integer key obtained via the key selector function.
/// The key must be stable (same element always produces the same key).</description></item>
/// <item><description><strong>Range Partitioning:</strong> The key range [min, max] is divided into k equal-sized buckets.
/// Each bucket i handles keys in the range [min + i×bucketSize, min + (i+1)×bucketSize).</description></item>
/// <item><description><strong>Distribution Function:</strong> Elements are distributed to buckets using: bucketIndex = ⌊(key - min) / bucketSize⌋.
/// This ensures all elements with similar keys go to the same bucket.</description></item>
/// <item><description><strong>Bucket Count:</strong> This implementation uses k = min(n, range) buckets. The Θ(n + k) average
/// case requires k to grow with n, so that a bucket holds O(1) elements in expectation; a bucket count fixed below that
/// leaves n/k elements per bucket and the per-bucket sorting, not the distribution, decides the running time.
/// More buckets than distinct keys cannot help, which is why the count is also bounded by the range.</description></item>
/// <item><description><strong>Per-Bucket Sorting:</strong> Each bucket is sorted using Insertion Sort (stable, O(m²) for m elements).
/// This ensures stability if the inner sort is stable.</description></item>
/// <item><description><strong>Concatenation Order:</strong> Buckets are concatenated in ascending order (bucket 0, 1, 2, ...).
/// Since bucket i contains only keys less than bucket i+1, concatenation produces a sorted sequence.</description></item>
/// <item><description><strong>Uniform Distribution Assumption:</strong> Optimal performance (O(n)) requires uniform key distribution.
/// Worst case O(n²) occurs when all elements fall into a single bucket.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution</description></item>
/// <item><description>Stable      : Yes (Insertion Sort preserves relative order)</description></item>
/// <item><description>In-place    : No (O(n + k) auxiliary space for buckets)</description></item>
/// <item><description>Best case   : Ω(n + k) - Uniform distribution, each bucket has ~n/k elements</description></item>
/// <item><description>Average case: Θ(n + k) - Assumes uniform distribution, total sort cost n×(n/k)²/k + n + k ≈ n</description></item>
/// <item><description>Worst case  : O(n²) - All elements in one bucket, degenerates to Insertion Sort</description></item>
/// <item><description>Range limit : None - auxiliary space depends on n, not on the key range</description></item>
/// <item><description>Comparisons : O(n log(n/k)) on average - Each bucket sorted independently</description></item>
/// <item><description>Space       : O(n + k) - k bucket lists plus n elements total</description></item>
/// <item><description>Note        : Bucket count is min(n, range); there is no cap on the key range, unlike CountingSort and PigeonholeSort. Skewed key distribution degrades performance.</description></item>
/// </list>
/// <para><strong>Comparison with Related Algorithms:</strong></para>
/// <list type="bullet">
/// <item><description>vs CountingSort / PigeonholeSort: Those size their auxiliary array by the key <em>range</em> and therefore
/// refuse inputs whose range is large relative to n. Bucket sort sizes its buckets by n instead, so it carries no range
/// limit and is the distribution sort for sparse keys — the case where the other two throw. The price is that a bucket
/// holds a span of keys rather than one, so a comparison sort has to run inside it: bucket sort is not comparison-free
/// and its cost depends on how the keys are distributed, with an O(n²) worst case the other two do not have.</description></item>
/// <item><description>vs RadixSort: Both handle a wide key range, but radix sort makes one pass per digit and needs a
/// fixed-width key mapping, while bucket sort distributes once. Radix sort's cost is independent of the key
/// distribution; bucket sort's is not.</description></item>
/// <item><description>Ordering control: <see cref="BucketSort.Sort{T, TComparer, TContext}(Span{T}, Func{T, int}, TComparer, TContext)"/>
/// is the only distribution-sort entry point in this library where a caller-supplied comparer defines the final order and
/// the key is merely a bucketing hint. Counting, pigeonhole and radix sort all order strictly by the extracted key.</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Bucket_sort</para>
/// </remarks>
public static class BucketSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>See the stability note in this type's summary.</remarks>
    public static bool IsStable => true;

    private const int StackAllocThreshold = 1024; // Use stackalloc for the bucket array when the count is smaller than this

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Temporary buffer for sorted elements

    /// <summary>
    /// Sorts the elements in the specified span by an integer key extracted with <paramref name="keySelector"/>.
    /// The key alone defines the order; elements with equal keys retain their relative input order (stable).
    /// Uses NullContext for zero-overhead fast path.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <param name="span">The span of elements to sort in place.</param>
    /// <param name="keySelector">Extracts the integer sort key from an element. Must be pure and consistent per element.</param>
    public static void SortBy<T>(Span<T> span, Func<T, int> keySelector)
    {
        ArgumentNullException.ThrowIfNull(keySelector);
        var selector = new FuncKeySelector<T>(keySelector);
        SortCore(span, selector, new KeySelectorComparer<T, FuncKeySelector<T>>(selector), NullContext.Default);
    }

    /// <summary>
    /// Sorts the elements in the specified span by an integer key extracted with <paramref name="keySelector"/>.
    /// The key alone defines the order; elements with equal keys retain their relative input order (stable).
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="keySelector">Extracts the integer sort key from an element. Must be pure and consistent per element.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void SortBy<T, TContext>(Span<T> span, Func<T, int> keySelector, TContext context)
        where TContext : ISortContext
    {
        ArgumentNullException.ThrowIfNull(keySelector);
        var selector = new FuncKeySelector<T>(keySelector);
        SortCore(span, selector, new KeySelectorComparer<T, FuncKeySelector<T>>(selector), context);
    }

    /// <summary>
    /// Sorts the elements in the specified span using the provided comparer and sort context.
    /// The comparer defines the final order; <paramref name="keySelector"/> is only a bucket-distribution
    /// accelerator and MUST be order-consistent with the comparer
    /// (comparer.Compare(x, y) &lt;= 0 implies keySelector(x) &lt;= keySelector(y)), otherwise the result is not sorted.
    /// This is the full-control version with explicit TContext type parameter.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use for element comparisons.</typeparam>
    /// <typeparam name="TContext">The type of sort context.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="keySelector">Extracts the bucket-distribution key. Must be order-consistent with <paramref name="comparer"/>.</param>
    /// <param name="comparer">The comparer to use for element comparisons.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation.</param>
    public static void Sort<T, TComparer, TContext>(Span<T> span, Func<T, int> keySelector, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        ArgumentNullException.ThrowIfNull(keySelector);
        SortCore(span, new FuncKeySelector<T>(keySelector), comparer, context);
    }

    private static void SortCore<T, TKeySelector, TComparer, TContext>(Span<T> span, TKeySelector keySelector, TComparer comparer, TContext context)
        where TKeySelector : struct, IKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);

        // Rent arrays from ArrayPool for temporary storage
        var keysArray = ArrayPool<int>.Shared.Rent(span.Length);
        var tempArray = ArrayPool<T>.Shared.Rent(span.Length);
        try
        {
            // Create SortSpan for temp buffer to track operations
            var tempSpan = new SortSpan<T, TComparer, TContext>(tempArray.AsSpan(0, span.Length), context, comparer, BUFFER_TEMP);
            var keys = keysArray.AsSpan(0, span.Length);

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

            long range = (long)max - (long)min + 1;

            // One bucket per element, so a bucket holds O(1) elements in expectation and the
            // per-bucket insertion sort stays O(n) overall. A count that does not scale with n
            // leaves n/k elements per bucket and makes the insertion sorts dominate: with the
            // sqrt(n) heuristic this measured Theta(n^1.5) even on uniformly distributed input.
            var bucketCount = s.Length;

            // More buckets than distinct keys cannot help; one bucket per key is the limit.
            if (range < bucketCount)
            {
                bucketCount = (int)range;
            }

            // Calculate bucket size (range divided by bucket count)
            var bucketSize = Math.Max(1, (range + bucketCount - 1) / bucketCount);

            int[]? rentedBounds = null;
            Span<int> bucketBounds = bucketCount <= StackAllocThreshold
                ? stackalloc int[bucketCount]
                : (rentedBounds = ArrayPool<int>.Shared.Rent(bucketCount)).AsSpan(0, bucketCount);
            bucketBounds.Clear(); // Required: neither branch yields zeroed memory - [module: SkipLocalsInit] skips it for stackalloc, and a pooled array carries its previous contents
            try
            {
                BucketDistribute(s, tempSpan, keys, bucketBounds, bucketSize, min);
            }
            finally
            {
                if (rentedBounds is not null)
                    ArrayPool<int>.Shared.Return(rentedBounds);
            }
        }
        finally
        {
            ArrayPool<int>.Shared.Return(keysArray);
            ArrayPool<T>.Shared.Return(tempArray, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    /// <summary>
    /// Distributes elements into buckets inside the temp buffer, sorts each bucket, and copies back.
    /// </summary>
    /// <param name="bucketBounds">
    /// A single k-sized array serving three roles in sequence: per-bucket counts, then each bucket's
    /// start offset, then - because scattering advances each entry once per element - each bucket's
    /// exclusive end. The end of bucket i-1 is the start of bucket i, so no second array is needed to
    /// recover the bucket ranges after distribution.
    /// </param>
    private static void BucketDistribute<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> temp, Span<int> keys, Span<int> bucketBounds, long bucketSize, int min)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var bucketCount = bucketBounds.Length;

        // First pass: convert keys to bucket indices and count
        // Reuse keys array to store bucket indices (eliminates division in second pass)
        s.Context.OnPhase(SortPhase.DistributionCount);
        for (var i = 0; i < s.Length; i++)
        {
            var key = keys[i];
            var bucketIndex = (int)(((long)key - min) / bucketSize);

            // Handle edge case where key == max
            if (bucketIndex >= bucketCount)
            {
                bucketIndex = bucketCount - 1;
            }

            keys[i] = bucketIndex; // Overwrite with bucket index
            bucketBounds[bucketIndex]++;
        }

        // Turn the counts into each bucket's starting position in the temp array
        s.Context.OnPhase(SortPhase.DistributionAccumulate);
        var offset = 0;
        for (var i = 0; i < bucketCount; i++)
        {
            var count = bucketBounds[i];
            bucketBounds[i] = offset;
            offset += count;
        }

        // Second pass: distribute elements using cached bucket indices
        s.Context.OnPhase(SortPhase.DistributionWrite);
        for (var i = 0; i < s.Length; i++)
        {
            var bucketIndex = keys[i]; // Reuse bucket index (no division)
            var pos = bucketBounds[bucketIndex]++;
            temp.Write(pos, s.Read(i));
        }

        // Sort each bucket in place inside the temp buffer. Every entry was advanced once per element
        // it received, so bucketBounds[i] is now the exclusive end of bucket i and the previous end is
        // its start.
        //
        // A bucket is sorted as a range of the temp buffer, not as a buffer of its own. A private
        // identifier would claim the elements moved elsewhere and then have them reappear in the CopyTo
        // below, which reports BUFFER_TEMP; the range is already unambiguous without one. Passing the
        // range instead of a slice reports the same indices and avoids building a SortSpan per bucket.
        var start = 0;
        for (var i = 0; i < bucketCount; i++)
        {
            var end = bucketBounds[i];
            if (end - start > 1)
            {
                InsertionSort.SortCore(temp, start, end);
            }
            start = end;
        }

        // Write sorted data back to original span using CopyTo for better performance
        temp.CopyTo(0, s, 0, s.Length);
    }

}

/// <summary>
/// 整数専用のバケットソート。
/// 値域を複数のバケットに分割し、各要素を値に応じてバケットに配置後、各バケット内をソートしてから結合します。
/// 値の分布が均等な場合に高速に動作します。
/// <br/>
/// Integer-specific bucket sort.
/// Divides the value range into multiple buckets, distributes elements by value, sorts each bucket, then concatenates.
/// Performs optimally when values are uniformly distributed.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Bucket Sort (Range-based):</strong></para>
/// <list type="number">
/// <item><description><strong>Range Partitioning:</strong> The value range [min, max] is divided into k equal-sized buckets.
/// Each bucket i handles values in the range [min + i×bucketSize, min + (i+1)×bucketSize).</description></item>
/// <item><description><strong>Distribution Function:</strong> Elements are distributed to buckets using: bucketIndex = ⌊(value - min) / bucketSize⌋.
/// This ensures all elements with similar values go to the same bucket.</description></item>
/// <item><description><strong>Bucket Count:</strong> This implementation uses k = min(n, range) buckets. The Θ(n + k) average
/// case requires k to grow with n, so that a bucket holds O(1) elements in expectation; a bucket count fixed below that
/// leaves n/k elements per bucket and the per-bucket sorting, not the distribution, decides the running time.
/// More buckets than distinct keys cannot help, which is why the count is also bounded by the range.</description></item>
/// <item><description><strong>Per-Bucket Sorting:</strong> Each bucket is sorted using Insertion Sort (stable, O(m²) for m elements).
/// This ensures stability if the inner sort is stable.</description></item>
/// <item><description><strong>Concatenation Order:</strong> Buckets are concatenated in ascending order (bucket 0, 1, 2, ...).
/// Since bucket i contains only values less than bucket i+1, concatenation produces a sorted sequence.</description></item>
/// <item><description><strong>Uniform Distribution Assumption:</strong> Optimal performance (O(n)) requires uniform distribution.
/// Worst case O(n²) occurs when all elements fall into a single bucket.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution</description></item>
/// <item><description>Stable      : Yes (Insertion Sort preserves relative order)</description></item>
/// <item><description>In-place    : No (O(n + k) auxiliary space for buckets)</description></item>
/// <item><description>Best case   : Ω(n + k) - Uniform distribution, each bucket has ~n/k elements</description></item>
/// <item><description>Average case: Θ(n + k) - Assumes uniform distribution, total sort cost n×(n/k)²/k + n + k ≈ n</description></item>
/// <item><description>Worst case  : O(n²) - All elements in one bucket, degenerates to Insertion Sort</description></item>
/// <item><description>Range limit : None - auxiliary space depends on n, not on the key range</description></item>
/// <item><description>Comparisons : O(n log(n/k)) on average - Each bucket sorted independently</description></item>
/// <item><description>Space       : O(n + k) - k bucket lists plus n elements total</description></item>
/// <item><description>Note        : バケット数は min(n, 値域)。CountingSort / PigeonholeSort と違い値域の上限制約はありません。</description></item>
/// </list>
/// </remarks>
public static class BucketSortInteger
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>See the stability note in this type's summary.</remarks>
    public static bool IsStable => true;

    private const int StackAllocThreshold = 1024; // Use stackalloc for the bucket array when the count is smaller than this

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Temporary buffer

    /// <summary>
    /// Sorts the elements in the specified span in ascending order using the default comparer.
    /// Uses NullContext for zero-overhead fast path.
    /// </summary>
    /// <typeparam name="T"> The type of elements to sort. Must be a binary integer type with defined min/max values.</typeparam>
    /// <param name="span"> The span of elements to sort.</param>
    public static void Sort<T>(Span<T> span) where T : IBinaryInteger<T>, IMinMaxValue<T>
        => Sort(span, new NumberComparer<T>(), NullContext.Default);

    /// <summary>
    /// Sorts the elements in the specified span using the provided sort context.
    /// </summary>
    /// <typeparam name="T"> The type of elements to sort. Must be a binary integer type with defined min/max values.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="span"> The span of elements to sort.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation.</param>
    public static void Sort<T, TContext>(Span<T> span, TContext context)
        where T : IBinaryInteger<T>, IMinMaxValue<T>
        where TContext : ISortContext
        => Sort(span, new NumberComparer<T>(), context);

    /// <summary>
    /// Sorts the elements in the specified span using the provided comparer and sort context.
    /// This is the full-control version with explicit TContext type parameter.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use for element comparisons.</typeparam>
    /// <typeparam name="TContext">The type of sort context.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="comparer">The comparer to use for element comparisons.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation.</param>
    public static void Sort<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context)
        where T : IBinaryInteger<T>, IMinMaxValue<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;

        EnsureSupportedType<T>();

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);

        // Rent arrays from ArrayPool for temporary storage
        var indicesArray = ArrayPool<int>.Shared.Rent(span.Length);
        var tempArray = ArrayPool<T>.Shared.Rent(span.Length);
        try
        {
            // Create SortSpan for temp buffer to track operations
            var tempSpan = new SortSpan<T, TComparer, TContext>(tempArray.AsSpan(0, span.Length), context, comparer, BUFFER_TEMP);
            var indices = indicesArray.AsSpan(0, span.Length);

            SortCore(s, tempSpan, indices, context);
        }
        finally
        {
            ArrayPool<int>.Shared.Return(indicesArray);
            ArrayPool<T>.Shared.Return(tempArray, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    private static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> tempSpan, Span<int> bucketIndices, TContext context)
        where T : IBinaryInteger<T>, IMinMaxValue<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Find min and max
        var minValue = s.Read(0);
        var maxValue = s.Read(0);

        for (var i = 1; i < s.Length; i++)
        {
            var value = s.Read(i);
            if (s.IsLessThan(value, minValue)) minValue = value;
            if (s.IsGreaterThan(value, maxValue)) maxValue = value;
        }

        // If all elements are the same, no need to sort
        if (s.Compare(minValue, maxValue) == 0) return;

        // Convert to long for range calculation
        var min = ConvertToLong(minValue);
        var max = ConvertToLong(maxValue);

        // Compute range in ulong to avoid signed overflow.
        // (ulong)(max - min) reinterprets the unchecked signed subtraction as an unsigned distance,
        // which is correct even when max - min overflows long
        // (e.g. min = long.MinValue, max = long.MaxValue → true distance = 2^64 - 1).
        // The +1 wraps to 0 only when the true range is exactly 2^64 (full long space); cap to ulong.MaxValue.
        ulong range = (ulong)(max - min) + 1;
        if (range == 0) range = ulong.MaxValue;

        // One bucket per element, so a bucket holds O(1) elements in expectation and the per-bucket
        // insertion sort stays O(n) overall. See <see cref="BucketSort"/> for the measurement that
        // rules out a bucket count fixed at sqrt(n).
        var bucketCount = s.Length;

        // More buckets than distinct values cannot help; one bucket per value is the limit.
        if (range < (ulong)bucketCount)
        {
            bucketCount = (int)range;
        }

        // Ceiling division without overflow: (range + bucketCount - 1) / bucketCount can overflow ulong
        // for large ranges, so use: a / b + (a % b != 0 ? 1 : 0)
        ulong bucketSize = Math.Max(1UL, range / (ulong)bucketCount + (range % (ulong)bucketCount != 0 ? 1UL : 0UL));

        int[]? rentedBounds = null;
        Span<int> bucketBounds = bucketCount <= StackAllocThreshold
            ? stackalloc int[bucketCount]
            : (rentedBounds = ArrayPool<int>.Shared.Rent(bucketCount)).AsSpan(0, bucketCount);
        bucketBounds.Clear(); // Required: neither branch yields zeroed memory - [module: SkipLocalsInit] skips it for stackalloc, and a pooled array carries its previous contents
        try
        {
            BucketDistribute(s, tempSpan, bucketIndices, bucketBounds, bucketSize, min);
        }
        finally
        {
            if (rentedBounds is not null)
                ArrayPool<int>.Shared.Return(rentedBounds);
        }
    }

    /// <inheritdoc cref="BucketSort.BucketDistribute"/>
    private static void BucketDistribute<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> source, SortSpan<T, TComparer, TContext> tempSpan, Span<int> bucketIndices, Span<int> bucketBounds, ulong bucketSize, long min)
        where T : IBinaryInteger<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var bucketCount = bucketBounds.Length;

        // First pass: calculate bucket indices and count
        // Cache bucket indices to avoid division in second pass
        source.Context.OnPhase(SortPhase.DistributionCount);
        for (var i = 0; i < source.Length; i++)
        {
            var value = source.Read(i);
            var valueLong = ConvertToLong(value);
            // (ulong)(valueLong - min): reinterprets the unchecked signed subtraction as an unsigned
            // distance. Correct even when valueLong - min overflows long (e.g. full ulong range).
            var bucketIndex = (int)((ulong)(valueLong - min) / bucketSize);

            // Handle edge case where value == max
            if (bucketIndex >= bucketCount)
            {
                bucketIndex = bucketCount - 1;
            }

            bucketIndices[i] = bucketIndex; // Cache bucket index
            bucketBounds[bucketIndex]++;
        }

        // Turn the counts into each bucket's starting position in the temp array
        source.Context.OnPhase(SortPhase.DistributionAccumulate);
        var offset = 0;
        for (var i = 0; i < bucketCount; i++)
        {
            var count = bucketBounds[i];
            bucketBounds[i] = offset;
            offset += count;
        }

        // Second pass: distribute elements using cached bucket indices
        source.Context.OnPhase(SortPhase.DistributionWrite);
        for (var i = 0; i < source.Length; i++)
        {
            var bucketIndex = bucketIndices[i]; // Reuse cached index (no division)
            var pos = bucketBounds[bucketIndex]++;
            tempSpan.Write(pos, source.Read(i));
        }

        // Sort each bucket in place inside the temp buffer. Every entry was advanced once per element
        // it received, so bucketBounds[i] is now the exclusive end of bucket i and the previous end is
        // its start.
        //
        // A bucket is a range of the temp buffer, not a buffer of its own, so it is sorted as a range:
        // the indices reported are the same either way, and the final CopyTo below reports BUFFER_TEMP,
        // so a consumer never sees elements appear in a buffer they were not written to.
        var start = 0;
        for (var i = 0; i < bucketCount; i++)
        {
            var end = bucketBounds[i];
            if (end - start > 1)
            {
                InsertionSort.SortCore(tempSpan, start, end);
            }
            start = end;
        }

        // Write sorted data back to original span using CopyTo for better performance
        tempSpan.CopyTo(0, source, 0, source.Length);
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

    /// <summary>
    /// Converts an integer value to <see cref="long"/> while preserving sort order.
    /// For <see cref="ulong"/> and 64-bit <see cref="nuint"/>, which cannot be safely
    /// represented as <see cref="long"/> via a plain cast, the sign bit is flipped via XOR.
    /// This maps [0, 2^64-1] → [long.MinValue, long.MaxValue] monotonically, so
    /// <c>a &lt; b</c> as unsigned iff <c>ConvertToLong(a) &lt; ConvertToLong(b)</c> as signed.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long ConvertToLong<T>(T value) where T : IBinaryInteger<T>
    {
        // ulong (and 64-bit nuint) values above long.MaxValue become negative under a plain cast,
        // corrupting min/max detection and bucket index arithmetic.
        // XOR-ing the sign bit remaps the unsigned range to the signed range in order-preserving fashion:
        //   ulong 0            → long.MinValue  (smallest)
        //   ulong.MaxValue     → long.MaxValue   (largest)
        if (typeof(T) == typeof(ulong) || (typeof(T) == typeof(nuint) && IntPtr.Size == 8))
            return (long)(ulong.CreateTruncating(value) ^ (1UL << 63));
        return long.CreateChecked(value);
    }
}
