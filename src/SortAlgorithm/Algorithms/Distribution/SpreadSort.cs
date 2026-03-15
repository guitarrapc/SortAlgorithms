using SortAlgorithm.Contexts;
using System.Buffers;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// SpreadSort - ハイブリッド分配ソートアルゴリズムの実装。
/// 値の範囲に基づいてバケットに分配し、バケット内を再帰的にソートします。
/// 分配が効果的でない場合は比較ベースのソートにフォールバックすることで、最悪ケースでもO(n log²n)を保証します。
/// <br/>
/// SpreadSort - A hybrid distribution sorting algorithm implementation.
/// Distributes elements into buckets based on their value range, then recursively sorts each bucket.
/// Falls back to comparison-based sorting when distribution is ineffective, guaranteeing O(n log²n) worst case.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct SpreadSort:</strong></para>
/// <list type="number">
/// <item><description><strong>Sign-Bit Flipping for Signed Integers:</strong> For signed types, the sign bit is flipped to convert signed values to unsigned keys,
/// ensuring correct ordering of negative values before positive values without separate processing.</description></item>
/// <item><description><strong>Bucket Count Based on Range:</strong> The number of buckets is determined by the upper bits of the value range (max - min).
/// The bucket count is capped by the number of elements to avoid excessive empty buckets.</description></item>
/// <item><description><strong>Value-Based Distribution:</strong> Each element is mapped to a bucket using:
/// bucket = ((key - minKey) >> shift), where shift is calculated from the range and bucket count.
/// This distributes elements proportionally across the available buckets.</description></item>
/// <item><description><strong>Iterative Processing:</strong> Each non-empty bucket is sorted iteratively using an explicit stack of work items.
/// Base cases: buckets with 0 or 1 elements are already sorted; small buckets fall back to insertion sort.</description></item>
/// <item><description><strong>Fallback to Comparison Sort:</strong> When all elements map to the same bucket (no progress via distribution),
/// the algorithm falls back to insertion sort to avoid infinite loops and ensure O(n log²n) worst case.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution (Hybrid: Distribution + Comparison)</description></item>
/// <item><description>Stable      : No (elements are redistributed across buckets)</description></item>
/// <item><description>In-place    : No (requires O(n) auxiliary space for bucket storage)</description></item>
/// <item><description>Best case   : O(n) - When distribution is perfectly uniform</description></item>
/// <item><description>Average case: O(n √(log n)) - Hybrid distribution and comparison</description></item>
/// <item><description>Worst case  : O(n log²n) - When distribution provides no benefit</description></item>
/// <item><description>Memory      : O(n) auxiliary space for bucket arrays and count arrays</description></item>
/// </list>
/// <para><strong>Algorithm Overview:</strong></para>
/// <para>The algorithm consists of these phases per iteration:</para>
/// <list type="number">
/// <item><description><strong>Find Range:</strong> Determine min and max unsigned keys in the current range</description></item>
/// <item><description><strong>Calculate Buckets:</strong> Compute bucket count from the bit width of the range, capped by element count</description></item>
/// <item><description><strong>Count Phase:</strong> Count occurrences of elements in each bucket</description></item>
/// <item><description><strong>Offset Phase:</strong> Compute prefix sums to determine bucket boundaries</description></item>
/// <item><description><strong>Distribute Phase:</strong> Place elements into their correct bucket positions using a temporary buffer</description></item>
/// <item><description><strong>Iterate Phase:</strong> Push each non-trivial bucket onto an explicit work stack and repeat</description></item>
/// </list>
/// <para><strong>Supported Types:</strong></para>
/// <list type="bullet">
/// <item><description><strong>Supported:</strong> byte, sbyte, short, ushort, int, uint, long, ulong (fixed-width up to 64-bit)</description></item>
/// <item><description><strong>Not Supported:</strong> nint, nuint (platform-dependent bit width), Int128, UInt128, BigInteger</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Spreadsort</para>
/// <para>Boost.Sort SpreadSort: https://www.boost.org/doc/libs/release/libs/sort/doc/html/sort/sort_hpp/spreadsort.html</para>
/// <para>Paper: "Spreadsort: A Cache-Friendly Sorting Algorithm" by Steven Ross (2002)</para>
/// </remarks>
public static class SpreadSort
{
    private const int InsertionSortCutoff = 16; // Switch to insertion sort for small ranges
    private const int MaxBucketLogBits = 11;    // Max log2(bucketCount) to avoid excessive memory
    private const int MinBucketLogBits = 1;     // Minimum meaningful bucket split
    private const int StackAllocThreshold = 128; // Use stackalloc for work stack when element count is at or below this

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Temporary buffer

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

        if (s.Length <= InsertionSortCutoff)
        {
            InsertionSort.SortCore(s, 0, s.Length);
            return;
        }

        SortCore(s);
    }

    private static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s)
        where T : IBinaryInteger<T>, IMinMaxValue<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // counts and writePos are bounded by MaxBucketLogBits (2048 ints = 8KB each) - always stackalloc
        Span<int> counts = stackalloc int[1 << MaxBucketLogBits];
        Span<int> writePos = stackalloc int[1 << MaxBucketLogBits];

        // temp buffer requires ArrayPool (can't stackalloc generic T without unmanaged constraint)
        var rentedTemp = ArrayPool<T>.Shared.Rent(s.Length);

        // Work stack size bound (n ints):
        //   Invariant: the sum of lengths across all work items on the stack is always <= n.
        //   - Pop removes L from the sum; Push adds back at most L (buckets partition the range).
        //   - Each pushed item has length >= 2 (we skip length <= 1).
        //   - Therefore max simultaneous items = floor(n/2), needing floor(n/2)*2 = n ints.
        int[]? rentedStack = null;
        Span<int> stack = s.Length <= StackAllocThreshold
            ? stackalloc int[s.Length]                     // Small: stackalloc work stack (128 ints = 512B)
            : (rentedStack = ArrayPool<int>.Shared.Rent(s.Length)).AsSpan(0, s.Length);
        try
        {
            var temp = new SortSpan<T, TComparer, TContext>(rentedTemp.AsSpan(0, s.Length), s.Context, s.Comparer, BUFFER_TEMP);
            SpreadSortIterative(s, temp, counts, writePos, stack);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(rentedTemp);
            if (rentedStack != null)
                ArrayPool<int>.Shared.Return(rentedStack);
        }
    }

    private static void SpreadSortIterative<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> temp, Span<int> counts, Span<int> writePos, Span<int> stack)
        where T : IBinaryInteger<T>, IMinMaxValue<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Initialize with the full range (no initial push needed)
        var start = 0;
        var length = s.Length;
        var stackTop = 0;

        while (true)
        {
            // Drain: handle trivial/small ranges by consuming or popping from stack.
            // This is the single exit point - all code paths that finish a subproblem
            // set length <= InsertionSortCutoff (or 0) and continue here.
            while (length <= InsertionSortCutoff)
            {
                if (length > 1)
                    InsertionSort.SortCore(s, start, start + length);
                if (stackTop == 0) return;
                length = stack[--stackTop];
                start = stack[--stackTop];
            }

            // Phase 1: Find min and max keys
            var minKey = GetUnsignedKey(s.Read(start));
            var maxKey = minKey;

            for (var i = 1; i < length; i++)
            {
                var key = GetUnsignedKey(s.Read(start + i));
                if (key < minKey) minKey = key;
                if (key > maxKey) maxKey = key;
            }

            // All elements have the same key - already sorted
            if (minKey == maxKey) { length = 0; continue; }

            // Phase 2: Calculate bucket count
            var range = maxKey - minKey;
            var rangeBits = 64 - BitOperations.LeadingZeroCount(range);

            var logBuckets = rangeBits / 2;
            if (logBuckets < MinBucketLogBits) logBuckets = MinBucketLogBits;
            if (logBuckets > MaxBucketLogBits) logBuckets = MaxBucketLogBits;

            // Cap bucket count so that bucketCount = 1 << logBuckets does not exceed length.
            // floor(log2(n)) guarantees 1 << floor(log2(n)) <= n.
            // length is int, so use int.LeadingZeroCount directly without type narrowing.
            var maxLogByLength = 31 - int.LeadingZeroCount(length);
            if (logBuckets > maxLogByLength) logBuckets = maxLogByLength;

            var bucketCount = 1 << logBuckets;
            var shift = (int)(rangeBits - logBuckets);
            if (shift < 0) shift = 0;

            var bucketCounts = counts[..bucketCount];
            bucketCounts.Clear();

            // Phase 3: Count elements per bucket
            // Track non-empty bucket count inline: increment only on 0→1 transition
            // to avoid a separate counting pass over bucketCounts.
            var nonEmptyBuckets = 0;
            s.Context.OnPhase(SortPhase.DistributionCount);
            for (var i = 0; i < length; i++)
            {
                var key = GetUnsignedKey(s.Read(start + i));
                var bucket = (int)((key - minKey) >> shift);
                if (bucket >= bucketCount) bucket = bucketCount - 1;
                if (bucketCounts[bucket]++ == 0) nonEmptyBuckets++;
            }

            // All elements fell into one bucket (no progress)
            // Fall back to insertion sort to avoid infinite loop
            if (nonEmptyBuckets <= 1)
            {
                InsertionSort.SortCore(s, start, start + length);
                length = 0; continue;
            }

            // Phase 4: Compute prefix sums (bucket offsets)
            s.Context.OnPhase(SortPhase.DistributionAccumulate);
            var prefixSum = 0;
            for (var i = 0; i < bucketCount; i++)
            {
                var count = bucketCounts[i];
                bucketCounts[i] = prefixSum;
                prefixSum += count;
            }

            // Phase 5: Distribute elements into temp buffer
            var bucketWritePos = writePos[..bucketCount];
            bucketCounts.CopyTo(bucketWritePos);

            s.Context.OnPhase(SortPhase.DistributionWrite);
            for (var i = 0; i < length; i++)
            {
                var value = s.Read(start + i);
                var key = GetUnsignedKey(value);
                var bucket = (int)((key - minKey) >> shift);
                if (bucket >= bucketCount) bucket = bucketCount - 1;

                // temp is reused as a scratch buffer at offset 0 for every subproblem
                temp.Write(bucketWritePos[bucket], value);
                bucketWritePos[bucket]++;
            }

            // Phase 6: Copy back from temp to main array
            temp.CopyTo(0, s, start, length);

            // Phase 7: Largest-first push optimization
            // Find the largest bucket (will be processed inline, not pushed).
            // Push all other non-trivial buckets onto the work stack.
            // This is analogous to QuickSort's "recurse on smaller, loop on larger"
            // and keeps peak stack usage proportional to the non-largest portions.
            var largestIdx = 0;
            var largestLen = 0;
            for (var i = 0; i < bucketCount; i++)
            {
                var bucketStart = bucketCounts[i];
                var bucketEnd = (i + 1 < bucketCount) ? bucketCounts[i + 1] : length;
                var bucketLength = bucketEnd - bucketStart;
                if (bucketLength > largestLen)
                {
                    largestLen = bucketLength;
                    largestIdx = i;
                }
            }

            // Push all non-trivial buckets except the largest
            for (var i = 0; i < bucketCount; i++)
            {
                if (i == largestIdx) continue;
                var bucketStart = bucketCounts[i];
                var bucketEnd = (i + 1 < bucketCount) ? bucketCounts[i + 1] : length;
                var bucketLength = bucketEnd - bucketStart;

                if (bucketLength > 1)
                {
                    Debug.Assert(stackTop + 2 <= stack.Length, $"Stack overflow: stackTop={stackTop}, stack.Length={stack.Length}, bucketCount={bucketCount}");
                    stack[stackTop++] = start + bucketStart;
                    stack[stackTop++] = bucketLength;
                }
            }

            // Process the largest bucket inline (tail-call optimization).
            // If largestLen <= InsertionSortCutoff, the drain loop at the top handles it.
            start += bucketCounts[largestIdx];
            length = largestLen;
        }
    }

    /// <summary>
    /// Validates that type T is a supported fixed-width integer type.
    /// Throws <see cref="NotSupportedException"/> for unsupported types.
    /// </summary>
    /// <remarks>
    /// Called once at the Sort entry point, not on the hot path.
    /// </remarks>
    private static void ThrowIfUnsupportedType<T>() where T : IBinaryInteger<T>
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
    /// <remarks>
    /// Uses a flat <c>typeof(T)</c> chain instead of runtime bitSize branching.
    /// The JIT eliminates all non-matching branches for each value type specialization,
    /// producing branchless code equivalent to a dedicated <c>SortCoreInt32</c> / <c>SortCoreInt64</c> etc.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong GetUnsignedKey<T>(T value) where T : IBinaryInteger<T>
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

        // Unreachable when ThrowIfUnsupportedType is called at the entry point
        throw new NotSupportedException($"Type {typeof(T).Name} is not supported.");
    }
}
