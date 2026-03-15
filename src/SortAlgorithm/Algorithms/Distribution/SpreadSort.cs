using SortAlgorithm.Contexts;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// SpreadSort - MSD Radix/ビット抽出ベースのハイブリッド分配ソートアルゴリズムの実装。
/// キーの上位ビットからビット抽出によりバケットに分配し、バケット内を再帰的にソートします。
/// ビット差分と部分問題サイズに基づく3段階フォールバック (InsertionSort / PDQSort / Spread分配) で、最悪ケースでもO(n log²n)を保証します。
/// <br/>
/// SpreadSort - An MSD radix/bit-extraction based hybrid distribution sorting algorithm.
/// Extracts upper bits from unsigned keys to distribute elements into buckets, then recursively sorts each bucket.
/// Uses a three-tier fallback (InsertionSort / PDQSort / Spread partition) based on bit difference and subproblem size,
/// guaranteeing O(n log²n) worst case.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct SpreadSort:</strong></para>
/// <list type="number">
/// <item><description><strong>Sign-Bit Flipping for Signed Integers:</strong> For signed types, the sign bit is flipped to convert signed values to unsigned keys,
/// ensuring correct ordering of negative values before positive values without separate processing.</description></item>
/// <item><description><strong>Adaptive Radix Width:</strong> The radix width (number of bits extracted per level) is determined
/// by a principled SpreadSort rule: radixBits = diffBits / maxSplits, where maxSplits = log₂(n) − LogMeanBinSize.
/// This distributes the differing bit range evenly across radix levels, targeting ~2^LogMeanBinSize elements per bin.
/// The bucket count is capped by the number of elements to avoid excessive empty buckets.</description></item>
/// <item><description><strong>Bit-Extraction Distribution:</strong> Each element is mapped to a bucket using:
/// bucket = ((key >> shift) &amp; mask), where shift is the lowest bit position of the target group and mask = (1 &lt;&lt; radixBits) - 1.
/// This extracts a specific bit group from each key, providing MSD radix-style partitioning.</description></item>
/// <item><description><strong>Iterative Processing:</strong> Each non-empty bucket is sorted iteratively using an explicit stack of work items.
/// Base cases: buckets with 0 or 1 elements are already sorted; small buckets fall back to insertion sort.</description></item>
/// <item><description><strong>Three-Tier Fallback:</strong> The algorithm uses three tiers based on subproblem size and bit difference:
/// (1) Tiny subproblems (≤ InsertionSortCutoff) fall back to InsertionSort.
/// (2) Subproblems too small for effective spreading (maxSplits &lt; 1) fall back to PDQSort.
/// (3) All other subproblems use spread partition (bit-extraction distribution), with radixBits capped to diffBits.
/// This decision based on XOR diff and subproblem size is the core SpreadSort principle that distinguishes it from plain MSD radix sort.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution (Hybrid: Distribution + Comparison via PDQSort + Insertion)</description></item>
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
/// <item><description><strong>Find Diff:</strong> Determine min and max unsigned keys; compute XOR diff to identify differing bit positions</description></item>
/// <item><description><strong>Calculate Shift:</strong> Adaptively compute radix width from subproblem size; derive shift from highest differing bit (adaptive level-skip)</description></item>
/// <item><description><strong>Count Phase:</strong> Count elements per bucket using bit extraction: bucket = (key >> shift) &amp; mask</description></item>
/// <item><description><strong>Offset Phase:</strong> Compute prefix sums to determine bucket boundaries</description></item>
/// <item><description><strong>Distribute Phase:</strong> Place elements into their correct bucket positions using a temporary buffer</description></item>
/// <item><description><strong>Iterate Phase:</strong> Advance to next lower bit group (nextShift = shift − radixBits); push each non-trivial bucket onto work stack</description></item>
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
    private const int LogMeanBinSize = 2;       // Target ~4 elements per bin on average (Boost SpreadSort: LOG_MEAN_BIN_SIZE)
    private const int MaxBucketLogBits = 11;    // Max log2(bucketCount) to avoid excessive memory (Boost SpreadSort: MAX_SPLITS)
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
        //   No shift is stored: each subproblem recomputes its own shift from XOR diff,
        //   which naturally provides adaptive level-skipping for shared upper bits.
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

            // Phase 1: Find min and max keys, compute XOR diff
            var minKey = GetUnsignedKey(s.Read(start));
            var maxKey = minKey;

            for (var i = 1; i < length; i++)
            {
                var key = GetUnsignedKey(s.Read(start + i));
                if (key < minKey) minKey = key;
                if (key > maxKey) maxKey = key;
            }

            // All elements have the same key - already sorted
            if (minKey == maxKey)
            {
                length = 0;
                continue;
            }

            // Phase 2: Determine bit range using XOR diff and compute shift
            // XOR reveals exactly which bit positions differ between min and max keys.
            // This is a key SpreadSort principle: partition by the highest differing bits,
            // rather than dividing a numeric range into equal-width intervals.
            var diff = minKey ^ maxKey;
            int highestBit = 63 - BitOperations.LeadingZeroCount(diff);
            var diffBits = highestBit + 1; // number of differing bits

            // SpreadSort decision: should we spread or comparison-sort?
            // maxSplits = how many radix levels this subproblem can sustain
            // (logLength bins per level, each bin targets ~2^LogMeanBinSize elements).
            // When maxSplits < 1, the subproblem is too small to benefit from spreading
            // and we fall back to comparison sort (PDQSort).
            var logLength = 31 - int.LeadingZeroCount(length);
            var maxSplits = logLength - LogMeanBinSize;

            if (maxSplits < 1)
            {
                // Comparison sort fallback: subproblem is too small for effective spreading.
                PDQSort.SortCore(s, start, start + length);
                length = 0;
                continue;
            }

            // Principled radixBits: distribute differing bits evenly across radix levels.
            // This ensures each level extracts a proportional share of the key space,
            // keeping average bin size around 2^LogMeanBinSize.
            var radixBits = diffBits / maxSplits;
            if (radixBits < MinBucketLogBits) radixBits = MinBucketLogBits;
            if (radixBits > MaxBucketLogBits) radixBits = MaxBucketLogBits;

            // Cap so bucketCount = 1 << radixBits does not exceed element count.
            // floor(log2(n)) guarantees 1 << floor(log2(n)) <= n.
            if (radixBits > logLength) radixBits = logLength;

            // Cap so we don't try to extract more bits than actually differ.
            // e.g., diffBits=2 with radixBits=3 would overextract; cap to diffBits
            // so the split stays within the meaningful bit range.
            if (radixBits > diffBits) radixBits = diffBits;

            // Compute effective shift: extract radixBits from the highest differing bit downward.
            // This provides adaptive level-skipping: when a bucket's elements share common upper bits,
            // the XOR diff reveals the actual differing range, and we jump directly to those bits
            // instead of stepping through empty radix levels.
            var shift = highestBit + 1 - radixBits;
            if (shift < 0) shift = 0;

            var bucketCount = 1 << radixBits;
            var mask = (ulong)(bucketCount - 1);

            var bucketCounts = counts[..bucketCount];
            bucketCounts.Clear();

            // Phase 3: Count elements per bucket using bit extraction
            // bucket = (key >> shift) & mask extracts the target bit group directly,
            // unlike range-based partitioning which uses (key - minKey) >> shift.
            // Track non-empty bucket count inline: increment only on 0→1 transition
            // to avoid a separate counting pass over bucketCounts.
            var nonEmptyBuckets = 0;
            s.Context.OnPhase(SortPhase.DistributionCount);
            for (var i = 0; i < length; i++)
            {
                var key = GetUnsignedKey(s.Read(start + i));
                var bucket = (int)((key >> shift) & mask);
                if (bucketCounts[bucket]++ == 0) nonEmptyBuckets++;
            }

            // All elements fell into one bucket (no progress).
            // Fall back to comparison sort to avoid infinite loop.
            // With XOR-based shift this should not occur because min and max always differ
            // at bit highestBit, guaranteeing at least 2 non-empty buckets. Kept as a safety net.
            if (nonEmptyBuckets <= 1)
            {
                PDQSort.SortCore(s, start, start + length);
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

            // Phase 5: Distribute elements into temp buffer using bit extraction
            var bucketWritePos = writePos[..bucketCount];
            bucketCounts.CopyTo(bucketWritePos);

            s.Context.OnPhase(SortPhase.DistributionWrite);
            const int tempStart = 0;
            for (var i = 0; i < length; i++)
            {
                var value = s.Read(start + i);
                var key = GetUnsignedKey(value);
                var bucket = (int)((key >> shift) & mask);

                // temp is reused as a scratch buffer at offset 0 for every subproblem
                temp.Write(tempStart + bucketWritePos[bucket], value);
                bucketWritePos[bucket]++;
            }

            // Phase 6: Copy back from temp to main array
            temp.CopyTo(tempStart, s, start, length);

            // Phase 7: Largest-first push optimization
            // Each bucket's subproblem will recompute its own shift from XOR diff,
            // which naturally provides adaptive level-skipping for shared upper bits.
            // Select the largest bucket as the next inline subproblem.
            // Trivial sizes are handled by the drain loop above, similar in spirit
            // to QuickSort's "recurse on smaller, iterate on larger" optimization.
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
                    if (stackTop + 2 > stack.Length)
                        throw new InvalidOperationException("Internal work stack overflow.");
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
    /// The JIT eliminates all non-matching branches for each value type specialization.
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
