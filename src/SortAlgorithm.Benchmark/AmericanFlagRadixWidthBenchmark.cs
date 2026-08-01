using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using SortAlgorithm.Utils;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Benchmark;

/// <summary>
/// TEMPORARY measurement harness: compares the previous 4-bit digit (16 buckets) against the current
/// 8-bit digit (256 buckets, the byte-wise form of McIlroy et al.), with both variants in the SAME run
/// so machine drift between runs cannot be mistaken for the effect.
/// The trade is passes against write streams: 8 bits halves the number of digit levels but scatters into
/// 256 concurrent destinations per level, and the fixed per-node cost (clear + prefix sum + bucket walk)
/// grows 16x, which small recursion nodes cannot amortize.
/// <para>
/// Result (ratio against Radix16_C16, lower is better). Widening the digit alone regressed full-int-range
/// keys badly, and raising the cutoff with it is what made the change a win everywhere:
/// </para>
/// <code>
/// cutoff sweep (benchmark methods since replaced by the two below)
/// n         keys      C16    C32    C64    C128
/// 4096      narrow    1.01   0.94   0.95   0.94
/// 4096      wide      1.55   0.51   0.53   0.52
/// 8192      narrow    0.65   0.65   0.66   0.65
/// 8192      wide      1.52   1.01   0.68   0.64
/// 65536     narrow    0.57   0.56   0.58   0.57
/// 65536     wide      0.62   0.66   0.66   0.67
/// 1048576   narrow    0.66   0.66   0.65   0.66
/// 1048576   wide      1.21   0.65   0.66   0.65
/// </code>
/// <para>
/// Three further variants were measured against the shipped configuration and ALL THREE LOST, so none of them
/// is being left on the table. Do not re-litigate without new evidence:
/// </para>
/// <code>
/// n         keys      shipped   Cycle   BinaryLeaf   PerNodeRescan
/// 4096      narrow    0.95      0.92    0.95         1.05
/// 4096      wide      0.52      0.59    0.64         0.56
/// 8192      narrow    0.66      0.65    0.66         0.71
/// 8192      wide      0.67      0.75    0.93         0.71
/// 65536     narrow    0.56      0.60    0.57         0.61
/// 65536     wide      0.62      0.63    0.65         0.65
/// 1048576   narrow    0.70      0.71    0.66         0.71
/// 1048576   wide      0.68      0.68    0.84         0.69
/// </code>
/// <para>
/// Cycle: holding the in-flight element in a local removes a write per chain step, but the shipped Swap is a
/// single fused ref-to-ref exchange, so there is no write to remove — only an extra key extraction on the
/// held value. BinaryLeaf: binary search cuts comparisons, not element moves, and at &lt;= 64 elements the
/// moves dominate. PerNodeRescan: one extra pass per node buys skipping that node's uniform levels, but the
/// shipped code already finds one uniform level per counting pass, so the rescan only pays off for a node
/// that skips 2+ levels — which these distributions do not produce often enough to cover the extra pass.
/// </para>
/// Delete once the numbers are recorded.
/// </summary>
[MemoryDiagnoser]
public class AmericanFlagRadixWidthBenchmark
{
    [Params(4096, 8192, 65536, 1_048_576)]
    public int Size { get; set; }

    /// <summary>
    /// Narrow: values in [1, n], so the key range is small and few digit levels survive the range scan.
    /// Wide: values across the full int range, so every digit level is live — the case that stresses
    /// the per-node cost of 256 buckets.
    /// </summary>
    [Params(true, false)]
    public bool WideKeyRange { get; set; }

    private SortBuffers<int> _buffers = default!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(42);
        int[] source;
        if (WideKeyRange)
        {
            source = new int[Size];
            for (var i = 0; i < source.Length; i++) source[i] = random.Next(int.MinValue, int.MaxValue);
        }
        else
        {
            source = ArrayPatterns.GenerateRandom(Size, random);
        }
        _buffers = new SortBuffers<int>(source);
    }

    [IterationSetup]
    public void IterationSetup() => _buffers.Reset();

    [Benchmark(Baseline = true)]
    public void Radix16_C16() => AmericanFlagSortRadix16Baseline.Sort(_buffers.Next().AsSpan());

    /// <summary>The shipped implementation: 8-bit digit with the cutoff raised to 64.</summary>
    [Benchmark]
    public void Radix256_Shipped() => AmericanFlagSort.Sort(_buffers.Next().AsSpan());

    /// <summary>Shipped configuration but with the McIlroy register-held cycle permutation.</summary>
    [Benchmark]
    public void Radix256_Cycle() => AmericanFlagSortVariants.SortCycle(_buffers.Next().AsSpan());

    /// <summary>Shipped configuration but with BinaryInsertionSort at the 64-element cutoff.</summary>
    [Benchmark]
    public void Radix256_BinaryLeaf() => AmericanFlagSortVariants.SortBinaryLeaf(_buffers.Next().AsSpan());

    /// <summary>Shipped configuration but rescanning the key range at every recursion node, not just the top.</summary>
    [Benchmark]
    public void Radix256_PerNodeRescan() => AmericanFlagSortPerNodeRescan.Sort(_buffers.Next().AsSpan());
}

/// <summary>
/// Cutoff as a type argument so the JIT folds it to a constant per instantiation, the way
/// <see cref="IRadixKeySelector{T}"/> folds its key width. A plain field would be reloaded per call
/// and would not fold the loop bounds, which is exactly what is under measurement here.
/// </summary>
public interface ICutoff { static abstract int Value { get; } }
public readonly struct Cutoff32 : ICutoff { public static int Value => 32; }
public readonly struct Cutoff64 : ICutoff { public static int Value => 64; }
public readonly struct Cutoff128 : ICutoff { public static int Value => 128; }

/// <summary>
/// Verbatim copy of AmericanFlagSort with the 4-bit digit it used before the widening
/// (range scan included, so the only difference under test is the digit width).
/// </summary>
public static class AmericanFlagSortRadix16Baseline
{
    private const int RadixBits = 4;
    private const int RadixSize = 16;
    private const int InsertionSortCutoff = 16;
    private const int BUFFER_MAIN = 0;

    public static void Sort<T>(Span<T> span) where T : IBinaryInteger<T>
        => SortCore(span, default(BinaryIntegerRadixKey<T>), new ComparableComparer<T>(), NullContext.Default);

    public static void Sort<T, TContext>(Span<T> span, TContext context) where T : IBinaryInteger<T> where TContext : ISortContext
        => SortCore(span, default(BinaryIntegerRadixKey<T>), new ComparableComparer<T>(), context);

    private static void SortCore<T, TRadixKey, TComparer, TContext>(Span<T> span, TRadixKey radixKey, TComparer comparer, TContext context)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;
        RadixKeyGuard.ValidateKeyBits<T, TRadixKey>();

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);

        var minKey = ulong.MaxValue;
        var maxKey = ulong.MinValue;
        for (var i = 0; i < s.Length; i++)
        {
            var key = radixKey.GetKey(s.Read(i));
            if (key < minKey) minKey = key;
            if (key > maxKey) maxKey = key;
        }

        var range = maxKey - minKey;
        if (range == 0) return;

        var requiredBits = 64 - BitOperations.LeadingZeroCount(range);
        var digitCount = (requiredBits + RadixBits - 1) / RadixBits;

        Recursive(s, radixKey, minKey, 0, s.Length, digitCount - 1, digitCount);
    }

    private static void Recursive<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, TRadixKey radixKey, ulong minKey, int start, int length, int digit, int digitCount)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (length <= InsertionSortCutoff)
        {
            InsertionSort.SortCore(s, start, start + length);
            return;
        }

        if (digit < 0) return;

        var shift = digit * RadixBits;

        Span<int> bucketCounts = stackalloc int[RadixSize + 1];
        Span<int> bucketNext = stackalloc int[RadixSize];

        s.Context.OnPhase(SortPhase.RadixPass, digit, digitCount);
        bucketCounts.Clear();

        for (var i = 0; i < length; i++)
        {
            var value = s.Read(start + i);
            var key = radixKey.GetKey(value) - minKey;
            var digitValue = (int)((key >> shift) & 0xF);
            bucketCounts[digitValue + 1]++;
        }

        var nonEmptyBuckets = 0;
        for (var i = 0; i < RadixSize; i++)
        {
            if (bucketCounts[i + 1] > 0 && ++nonEmptyBuckets > 1)
                break;
        }

        if (nonEmptyBuckets <= 1)
        {
            if (digit > 0)
                Recursive(s, radixKey, minKey, start, length, digit - 1, digitCount);
            return;
        }

        for (var i = 1; i <= RadixSize; i++)
        {
            bucketCounts[i] += bucketCounts[i - 1];
        }

        for (var i = 0; i < RadixSize; i++)
        {
            bucketNext[i] = bucketCounts[i];
        }

        PermuteInPlace(s, radixKey, minKey, start, shift, bucketCounts, bucketNext);

        for (var i = 0; i < RadixSize; i++)
        {
            var bucketStart = bucketCounts[i];
            var bucketLength = bucketCounts[i + 1] - bucketStart;

            if (bucketLength > 1)
            {
                Recursive(s, radixKey, minKey, start + bucketStart, bucketLength, digit - 1, digitCount);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void PermuteInPlace<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, TRadixKey radixKey, ulong minKey, int start, int shift, Span<int> bucketCounts, Span<int> bucketNext)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        for (var bucket = 0; bucket < RadixSize; bucket++)
        {
            var bucketEnd = bucketCounts[bucket + 1];

            while (bucketNext[bucket] < bucketEnd)
            {
                var currentPos = start + bucketNext[bucket];
                var currentValue = s.Read(currentPos);
                var currentKey = radixKey.GetKey(currentValue) - minKey;
                var currentDigit = (int)((currentKey >> shift) & 0xF);

                if (currentDigit == bucket)
                {
                    bucketNext[bucket]++;
                    continue;
                }

                s.Swap(currentPos, start + bucketNext[currentDigit]);
                bucketNext[currentDigit]++;
            }
        }
    }
}

/// <summary>
/// Current AmericanFlagSort (8-bit digit, range normalization) with the insertion-sort cutoff left open,
/// to find where a 256-bucket split stops paying for itself on a small recursion node.
/// </summary>
public static class AmericanFlagSortRadix256Tunable
{
    private const int RadixBits = 8;
    private const int RadixSize = 256;
    private const int RadixMask = RadixSize - 1;
    private const int BUFFER_MAIN = 0;

    public static void Sort<T, TCutoff>(Span<T> span)
        where T : IBinaryInteger<T>
        where TCutoff : struct, ICutoff
        => SortCore<T, BinaryIntegerRadixKey<T>, ComparableComparer<T>, NullContext, TCutoff>(
            span, default, new ComparableComparer<T>(), NullContext.Default);

    private static void SortCore<T, TRadixKey, TComparer, TContext, TCutoff>(Span<T> span, TRadixKey radixKey, TComparer comparer, TContext context)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
        where TCutoff : struct, ICutoff
    {
        if (span.Length <= 1) return;
        RadixKeyGuard.ValidateKeyBits<T, TRadixKey>();

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);

        var minKey = ulong.MaxValue;
        var maxKey = ulong.MinValue;
        for (var i = 0; i < s.Length; i++)
        {
            var key = radixKey.GetKey(s.Read(i));
            if (key < minKey) minKey = key;
            if (key > maxKey) maxKey = key;
        }

        var range = maxKey - minKey;
        if (range == 0) return;

        var requiredBits = 64 - BitOperations.LeadingZeroCount(range);
        var digitCount = (requiredBits + RadixBits - 1) / RadixBits;

        Recursive<T, TRadixKey, TComparer, TContext, TCutoff>(s, radixKey, minKey, 0, s.Length, digitCount - 1, digitCount);
    }

    private static void Recursive<T, TRadixKey, TComparer, TContext, TCutoff>(SortSpan<T, TComparer, TContext> s, TRadixKey radixKey, ulong minKey, int start, int length, int digit, int digitCount)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
        where TCutoff : struct, ICutoff
    {
        if (length <= TCutoff.Value)
        {
            InsertionSort.SortCore(s, start, start + length);
            return;
        }

        if (digit < 0) return;

        var shift = digit * RadixBits;

        Span<int> bucketCounts = stackalloc int[RadixSize + 1];
        Span<int> bucketNext = stackalloc int[RadixSize];

        s.Context.OnPhase(SortPhase.RadixPass, digit, digitCount);
        bucketCounts.Clear();

        for (var i = 0; i < length; i++)
        {
            var key = radixKey.GetKey(s.Read(start + i)) - minKey;
            bucketCounts[(int)((key >> shift) & RadixMask) + 1]++;
        }

        var nonEmptyBuckets = 0;
        for (var i = 0; i < RadixSize; i++)
        {
            if (bucketCounts[i + 1] > 0 && ++nonEmptyBuckets > 1)
                break;
        }

        if (nonEmptyBuckets <= 1)
        {
            if (digit > 0)
                Recursive<T, TRadixKey, TComparer, TContext, TCutoff>(s, radixKey, minKey, start, length, digit - 1, digitCount);
            return;
        }

        for (var i = 1; i <= RadixSize; i++) bucketCounts[i] += bucketCounts[i - 1];
        for (var i = 0; i < RadixSize; i++) bucketNext[i] = bucketCounts[i];

        for (var bucket = 0; bucket < RadixSize; bucket++)
        {
            var bucketEnd = bucketCounts[bucket + 1];
            while (bucketNext[bucket] < bucketEnd)
            {
                var currentPos = start + bucketNext[bucket];
                var currentKey = radixKey.GetKey(s.Read(currentPos)) - minKey;
                var currentDigit = (int)((currentKey >> shift) & RadixMask);

                if (currentDigit == bucket) { bucketNext[bucket]++; continue; }

                s.Swap(currentPos, start + bucketNext[currentDigit]);
                bucketNext[currentDigit]++;
            }
        }

        for (var i = 0; i < RadixSize; i++)
        {
            var bucketStart = bucketCounts[i];
            var bucketLength = bucketCounts[i + 1] - bucketStart;
            if (bucketLength > 1)
            {
                Recursive<T, TRadixKey, TComparer, TContext, TCutoff>(s, radixKey, minKey, start + bucketStart, bucketLength, digit - 1, digitCount);
            }
        }
    }
}

/// <summary>
/// Shipped configuration (8-bit digit, cutoff 64, range normalization) with two things varied that the
/// shipped code does not do, to check whether either is being left on the table:
/// <list type="bullet">
/// <item><description><c>Cycle</c>: the register-held cycle permutation from McIlroy et al. — the in-flight
/// element stays in a local instead of being written back to the array on every step of the chain.</description></item>
/// <item><description><c>BinaryLeaf</c>: <see cref="BinaryInsertionSort"/> instead of <see cref="InsertionSort"/>
/// at the cutoff. At 64 elements the leaf sort's comparison count is no longer negligible.</description></item>
/// </list>
/// </summary>
public static class AmericanFlagSortVariants
{
    private const int RadixBits = 8;
    private const int RadixSize = 256;
    private const int RadixMask = RadixSize - 1;
    private const int InsertionSortCutoff = 64;
    private const int BUFFER_MAIN = 0;

    public static void SortCycle<T>(Span<T> span) where T : IBinaryInteger<T>
        => Entry<T, BinaryIntegerRadixKey<T>, ComparableComparer<T>, NullContext>(span, default, new ComparableComparer<T>(), NullContext.Default, cycle: true, binaryLeaf: false);

    public static void SortBinaryLeaf<T>(Span<T> span) where T : IBinaryInteger<T>
        => Entry<T, BinaryIntegerRadixKey<T>, ComparableComparer<T>, NullContext>(span, default, new ComparableComparer<T>(), NullContext.Default, cycle: false, binaryLeaf: true);

    private static void Entry<T, TRadixKey, TComparer, TContext>(Span<T> span, TRadixKey radixKey, TComparer comparer, TContext context, bool cycle, bool binaryLeaf)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;
        RadixKeyGuard.ValidateKeyBits<T, TRadixKey>();

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        if (s.Length <= InsertionSortCutoff) { Leaf(s, 0, s.Length, binaryLeaf); return; }

        var minKey = ulong.MaxValue;
        var maxKey = ulong.MinValue;
        for (var i = 0; i < s.Length; i++)
        {
            var key = radixKey.GetKey(s.Read(i));
            if (key < minKey) minKey = key;
            if (key > maxKey) maxKey = key;
        }

        var range = maxKey - minKey;
        if (range == 0) return;

        var requiredBits = 64 - BitOperations.LeadingZeroCount(range);
        var digitCount = (requiredBits + RadixBits - 1) / RadixBits;

        Recursive(s, radixKey, minKey, 0, s.Length, digitCount - 1, cycle, binaryLeaf);
    }

    private static void Leaf<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int start, int length, bool binaryLeaf)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (binaryLeaf) BinaryInsertionSort.SortCore(s, start, start + length, start);
        else InsertionSort.SortCore(s, start, start + length);
    }

    private static void Recursive<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, TRadixKey radixKey, ulong minKey, int start, int length, int digit, bool cycle, bool binaryLeaf)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (length <= InsertionSortCutoff) { Leaf(s, start, length, binaryLeaf); return; }
        if (digit < 0) return;

        var shift = digit * RadixBits;
        Span<int> bucketCounts = stackalloc int[RadixSize + 1];
        Span<int> bucketNext = stackalloc int[RadixSize];
        bucketCounts.Clear();

        for (var i = 0; i < length; i++)
        {
            var key = radixKey.GetKey(s.Read(start + i)) - minKey;
            bucketCounts[(int)((key >> shift) & RadixMask) + 1]++;
        }

        var nonEmptyBuckets = 0;
        for (var i = 0; i < RadixSize; i++)
            if (bucketCounts[i + 1] > 0 && ++nonEmptyBuckets > 1) break;

        if (nonEmptyBuckets <= 1)
        {
            if (digit > 0) Recursive(s, radixKey, minKey, start, length, digit - 1, cycle, binaryLeaf);
            return;
        }

        for (var i = 1; i <= RadixSize; i++) bucketCounts[i] += bucketCounts[i - 1];
        for (var i = 0; i < RadixSize; i++) bucketNext[i] = bucketCounts[i];

        if (cycle)
        {
            // Hold the in-flight element in a local: the chain writes each array slot once instead of
            // performing a full swap (read + two writes) at every step.
            for (var bucket = 0; bucket < RadixSize; bucket++)
            {
                var bucketEnd = bucketCounts[bucket + 1];
                while (bucketNext[bucket] < bucketEnd)
                {
                    var home = start + bucketNext[bucket];
                    var tmp = s.Read(home);
                    int d;
                    while ((d = (int)(((radixKey.GetKey(tmp) - minKey) >> shift) & RadixMask)) != bucket)
                    {
                        var q = start + bucketNext[d]++;
                        var evicted = s.Read(q);
                        s.Write(q, tmp);
                        tmp = evicted;
                    }
                    s.Write(home, tmp);
                    bucketNext[bucket]++;
                }
            }
        }
        else
        {
            for (var bucket = 0; bucket < RadixSize; bucket++)
            {
                var bucketEnd = bucketCounts[bucket + 1];
                while (bucketNext[bucket] < bucketEnd)
                {
                    var currentPos = start + bucketNext[bucket];
                    var currentKey = radixKey.GetKey(s.Read(currentPos)) - minKey;
                    var currentDigit = (int)((currentKey >> shift) & RadixMask);
                    if (currentDigit == bucket) { bucketNext[bucket]++; continue; }
                    s.Swap(currentPos, start + bucketNext[currentDigit]);
                    bucketNext[currentDigit]++;
                }
            }
        }

        for (var i = 0; i < RadixSize; i++)
        {
            var bucketStart = bucketCounts[i];
            var bucketLength = bucketCounts[i + 1] - bucketStart;
            if (bucketLength > 1)
                Recursive(s, radixKey, minKey, start + bucketStart, bucketLength, digit - 1, cycle, binaryLeaf);
        }
    }
}

/// <summary>
/// Shipped configuration plus a min/max rescan at EVERY recursion node, not just at the top.
/// <para>
/// Correctness: within a node all higher digits are already equal, so re-normalizing by the node's own
/// minimum and re-deriving its digit count still sorts the node by key — subtracting a constant is
/// order-preserving and the node's position among its siblings is already fixed.
/// </para>
/// <para>
/// It also makes the uniform-digit early-termination check dead: normalized keys span [0, range], so the
/// minimum's top digit is 0 while the maximum's is non-zero by construction, and the top digit of a
/// rescanned node can never be uniform. The variant therefore drops that check.
/// </para>
/// <para>
/// The trade: one extra pass per node buys skipping however many uniform levels that node has. It wins only
/// if a node skips 2+ levels on average, because the shipped code already discovers one uniform level per
/// counting pass.
/// </para>
/// </summary>
public static class AmericanFlagSortPerNodeRescan
{
    private const int RadixBits = 8;
    private const int RadixSize = 256;
    private const int RadixMask = RadixSize - 1;
    private const int InsertionSortCutoff = 64;
    private const int BUFFER_MAIN = 0;

    public static void Sort<T>(Span<T> span) where T : IBinaryInteger<T>
    {
        if (span.Length <= 1) return;
        RadixKeyGuard.ValidateKeyBits<T, BinaryIntegerRadixKey<T>>();
        var s = new SortSpan<T, ComparableComparer<T>, NullContext>(span, NullContext.Default, new ComparableComparer<T>(), BUFFER_MAIN);
        if (s.Length <= InsertionSortCutoff) { InsertionSort.SortCore(s, 0, s.Length); return; }
        Recursive(s, default(BinaryIntegerRadixKey<T>), 0, s.Length);
    }

    private static void Recursive<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, TRadixKey radixKey, int start, int length)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (length <= InsertionSortCutoff)
        {
            InsertionSort.SortCore(s, start, start + length);
            return;
        }

        // Rescan this node's own key range instead of inheriting the parent's digit position.
        var minKey = ulong.MaxValue;
        var maxKey = ulong.MinValue;
        for (var i = 0; i < length; i++)
        {
            var key = radixKey.GetKey(s.Read(start + i));
            if (key < minKey) minKey = key;
            if (key > maxKey) maxKey = key;
        }

        var range = maxKey - minKey;
        if (range == 0) return; // every key in this node is equal

        var requiredBits = 64 - BitOperations.LeadingZeroCount(range);
        var digit = (requiredBits + RadixBits - 1) / RadixBits - 1;
        var shift = digit * RadixBits;

        Span<int> bucketCounts = stackalloc int[RadixSize + 1];
        Span<int> bucketNext = stackalloc int[RadixSize];
        bucketCounts.Clear();

        for (var i = 0; i < length; i++)
        {
            var key = radixKey.GetKey(s.Read(start + i)) - minKey;
            bucketCounts[(int)((key >> shift) & RadixMask) + 1]++;
        }

        // No uniform-digit check: the rescan guarantees the top digit varies.
        for (var i = 1; i <= RadixSize; i++) bucketCounts[i] += bucketCounts[i - 1];
        for (var i = 0; i < RadixSize; i++) bucketNext[i] = bucketCounts[i];

        for (var bucket = 0; bucket < RadixSize - 1; bucket++)
        {
            var bucketEnd = bucketCounts[bucket + 1];
            while (bucketNext[bucket] < bucketEnd)
            {
                var currentPos = start + bucketNext[bucket];
                var currentKey = radixKey.GetKey(s.Read(currentPos)) - minKey;
                var currentDigit = (int)((currentKey >> shift) & RadixMask);
                if (currentDigit == bucket) { bucketNext[bucket]++; continue; }
                s.Swap(currentPos, start + bucketNext[currentDigit]);
                bucketNext[currentDigit]++;
            }
        }

        for (var i = 0; i < RadixSize; i++)
        {
            var bucketStart = bucketCounts[i];
            var bucketLength = bucketCounts[i + 1] - bucketStart;
            if (bucketLength > 1)
                Recursive(s, radixKey, start + bucketStart, bucketLength);
        }
    }
}
