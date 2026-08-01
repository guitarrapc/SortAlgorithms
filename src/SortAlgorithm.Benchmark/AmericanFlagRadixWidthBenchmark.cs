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
/// n         keys      C16    C32    C64    C128   shipped (C64)
/// 4096      narrow    1.01   0.94   0.95   0.94   0.90
/// 4096      wide      1.55   0.51   0.53   0.52   0.50
/// 8192      narrow    0.65   0.65   0.66   0.65   0.68
/// 8192      wide      1.52   1.01   0.68   0.64   0.63
/// 65536     narrow    0.57   0.56   0.58   0.57   0.57
/// 65536     wide      0.62   0.66   0.66   0.67   0.63
/// 1048576   narrow    0.66   0.66   0.65   0.66   0.61
/// 1048576   wide      1.21   0.65   0.66   0.65   0.62
/// </code>
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

    [Benchmark]
    public void Radix256_C32() => AmericanFlagSortRadix256Tunable.Sort<int, Cutoff32>(_buffers.Next().AsSpan());

    [Benchmark]
    public void Radix256_C64() => AmericanFlagSortRadix256Tunable.Sort<int, Cutoff64>(_buffers.Next().AsSpan());

    [Benchmark]
    public void Radix256_C128() => AmericanFlagSortRadix256Tunable.Sort<int, Cutoff128>(_buffers.Next().AsSpan());
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
