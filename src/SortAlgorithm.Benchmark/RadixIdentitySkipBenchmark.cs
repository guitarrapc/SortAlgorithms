using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Benchmark;

/// <summary>
/// TEMPORARY measurement harness for the identity-digit skip. Each shipped sort is paired with a copy of
/// itself that never checks, in the SAME run, over two data shapes: one where no digit is uniform (the
/// check can only cost) and one where the low digits are (the check can only pay).
/// Delete once the numbers are recorded.
/// </summary>
[MemoryDiagnoser]
public class RadixIdentitySkipBenchmark
{
    [Params(1024, 8192)]
    public int Size { get; set; }

    /// <summary>1 = plain [0, n), no uniform digit anywhere. 65536 = every value a multiple of 2^16.</summary>
    [Params(1, 65_536)]
    public int Stride { get; set; }

    private SortBuffers<int> _buffers = default!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(42);
        var source = Enumerable.Range(0, Size).Select(x => x * Stride).OrderBy(_ => random.Next()).ToArray();
        _buffers = new SortBuffers<int>(source);
    }

    [IterationSetup]
    public void IterationSetup() => _buffers.Reset();

    [Benchmark(Baseline = true)]
    public void Lsd4_NoSkip() => RadixLSD4SortNoSkip.Sort(_buffers.Next().AsSpan());

    [Benchmark]
    public void Lsd4_Skip() => RadixLSD4Sort.Sort(_buffers.Next().AsSpan());

    [Benchmark]
    public void Lsd256_NoSkip() => RadixLSD256SortNoSkip.Sort(_buffers.Next().AsSpan());

    [Benchmark]
    public void Lsd256_Skip() => RadixLSD256Sort.Sort(_buffers.Next().AsSpan());

    [Benchmark]
    public void Lsd10_NoSkip() => RadixLSD10SortNoSkip.Sort(_buffers.Next().AsSpan());

    [Benchmark]
    public void Lsd10_Skip() => RadixLSD10Sort.Sort(_buffers.Next().AsSpan());
}

/// <summary>RadixLSD4Sort without the identity-digit check.</summary>
public static class RadixLSD4SortNoSkip
{
    private const int RadixBits = 2;
    private const int RadixSize = 4;

    public static void Sort<T>(Span<T> span) where T : IBinaryInteger<T>
        => SortCore(span, default(BinaryIntegerRadixKey<T>), new ComparableComparer<T>(), NullContext.Default);

    private static void SortCore<T, TRadixKey, TComparer, TContext>(Span<T> span, TRadixKey radixKey, TComparer comparer, TContext context)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;
        RadixKeyGuard.ValidateKeyBits<T, TRadixKey>();

        var tempArray = ArrayPool<T>.Shared.Rent(span.Length);
        var keysArray = ArrayPool<ulong>.Shared.Rent(span.Length);
        var keysBufferArray = ArrayPool<ulong>.Shared.Rent(span.Length);

        try
        {
            var tempBuffer = tempArray.AsSpan(0, span.Length);
            var keys = keysArray.AsSpan(0, span.Length);
            var keysBuffer = keysBufferArray.AsSpan(0, span.Length);

            Span<int> bucketOffsets = stackalloc int[RadixSize + 1];
            var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, 0);
            var temp = new SortSpan<T, TComparer, TContext>(tempBuffer, context, comparer, 1);

            var minKey = ulong.MaxValue;
            var maxKey = ulong.MinValue;

            for (var i = 0; i < s.Length; i++)
            {
                var key = radixKey.GetKey(s.Read(i));
                keys[i] = key;
                if (key < minKey) minKey = key;
                if (key > maxKey) maxKey = key;
            }

            var range = maxKey - minKey;
            if (range == 0) return;

            if (minKey != 0)
            {
                for (var i = 0; i < keys.Length; i++) keys[i] -= minKey;
            }

            var requiredBits = 64 - BitOperations.LeadingZeroCount(range);
            var digitCount = (requiredBits + RadixBits - 1) / RadixBits;

            LSDSort(s, temp, keys, keysBuffer, digitCount, bucketOffsets);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(tempArray, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
            ArrayPool<ulong>.Shared.Return(keysArray);
            ArrayPool<ulong>.Shared.Return(keysBufferArray);
        }
    }

    private static void LSDSort<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> temp, Span<ulong> keys, Span<ulong> keysBuffer, int digitCount, Span<int> bucketOffsets)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var src = s;
        var dst = temp;
        var srcKeys = keys;
        var dstKeys = keysBuffer;

        for (int d = 0; d < digitCount; d++)
        {
            src.Context.OnPhase(SortPhase.RadixPass, d, digitCount);
            var shift = d * RadixBits;

            bucketOffsets.Clear();

            for (var i = 0; i < src.Length; i++)
            {
                bucketOffsets[(int)((srcKeys[i] >> shift) & 0b11) + 1]++;
            }

            for (var i = 1; i <= RadixSize; i++) bucketOffsets[i] += bucketOffsets[i - 1];

            for (var i = 0; i < src.Length; i++)
            {
                var value = src.Read(i);
                var key = srcKeys[i];
                var destIndex = bucketOffsets[(int)((key >> shift) & 0b11)]++;
                dst.Write(destIndex, value);
                dstKeys[destIndex] = key;
            }

            var swapSpan = src; src = dst; dst = swapSpan;
            var swapKeys = srcKeys; srcKeys = dstKeys; dstKeys = swapKeys;
        }

        if ((digitCount & 1) == 1) src.CopyTo(0, s, 0, s.Length);
    }
}

/// <summary>RadixLSD256Sort without the identity-digit check.</summary>
public static class RadixLSD256SortNoSkip
{
    private const int RadixBits = 8;
    private const int RadixSize = 256;

    public static void Sort<T>(Span<T> span) where T : IBinaryInteger<T>
        => SortCore(span, default(BinaryIntegerRadixKey<T>), new ComparableComparer<T>(), NullContext.Default);

    private static void SortCore<T, TRadixKey, TComparer, TContext>(Span<T> span, TRadixKey radixKey, TComparer comparer, TContext context)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;
        RadixKeyGuard.ValidateKeyBits<T, TRadixKey>();

        var tempArray = ArrayPool<T>.Shared.Rent(span.Length);
        var bucketOffsetsArray = ArrayPool<int>.Shared.Rent(RadixSize + 1);

        try
        {
            var tempBuffer = tempArray.AsSpan(0, span.Length);
            var bucketOffsets = bucketOffsetsArray.AsSpan(0, RadixSize + 1);
            var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, 0);
            var temp = new SortSpan<T, TComparer, TContext>(tempBuffer, context, comparer, 1);

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

            LSDSort(s, temp, radixKey, digitCount, minKey, bucketOffsets);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(tempArray, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
            ArrayPool<int>.Shared.Return(bucketOffsetsArray);
        }
    }

    private static void LSDSort<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> temp, TRadixKey radixKey, int digitCount, ulong minKey, Span<int> bucketOffsets)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var src = s;
        var dst = temp;

        for (int d = 0; d < digitCount; d++)
        {
            src.Context.OnPhase(SortPhase.RadixPass, d, digitCount);
            var shift = d * RadixBits;

            bucketOffsets.Clear();

            for (var i = 0; i < src.Length; i++)
            {
                var key = radixKey.GetKey(src.Read(i)) - minKey;
                bucketOffsets[(int)((key >> shift) & 0xFF) + 1]++;
            }

            for (var i = 1; i <= RadixSize; i++) bucketOffsets[i] += bucketOffsets[i - 1];

            for (var i = 0; i < src.Length; i++)
            {
                var value = src.Read(i);
                var key = radixKey.GetKey(value) - minKey;
                dst.Write(bucketOffsets[(int)((key >> shift) & 0xFF)]++, value);
            }

            var swapSpan = src; src = dst; dst = swapSpan;
        }

        if ((digitCount & 1) == 1) src.CopyTo(0, s, 0, s.Length);
    }
}

/// <summary>RadixLSD10Sort without the identity-digit check.</summary>
public static class RadixLSD10SortNoSkip
{
    private const int RadixBase = 10;
    private const int HistogramStride = RadixBase + 1;
    private const int MaxDigits = 20;

    private static ReadOnlySpan<ulong> Pow10 =>
    [
        1UL, 10UL, 100UL, 1_000UL, 10_000UL, 100_000UL, 1_000_000UL, 10_000_000UL,
        100_000_000UL, 1_000_000_000UL, 10_000_000_000UL, 100_000_000_000UL,
        1_000_000_000_000UL, 10_000_000_000_000UL, 100_000_000_000_000UL,
        1_000_000_000_000_000UL, 10_000_000_000_000_000UL, 100_000_000_000_000_000UL,
        1_000_000_000_000_000_000UL, 10_000_000_000_000_000_000UL
    ];

    public static void Sort<T>(Span<T> span) where T : IBinaryInteger<T>
        => SortCore(span, default(BinaryIntegerRadixKey<T>), new ComparableComparer<T>(), NullContext.Default);

    private static void SortCore<T, TRadixKey, TComparer, TContext>(Span<T> span, TRadixKey radixKey, TComparer comparer, TContext context)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;
        RadixKeyGuard.ValidateKeyBits<T, TRadixKey>();

        var tempArray = ArrayPool<T>.Shared.Rent(span.Length);

        try
        {
            var tempBuffer = tempArray.AsSpan(0, span.Length);

            Span<int> allHistograms = stackalloc int[MaxDigits * HistogramStride];
            var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, 0);
            var temp = new SortSpan<T, TComparer, TContext>(tempBuffer, context, comparer, 1);

            var minKey = ulong.MaxValue;
            var maxKey = ulong.MinValue;

            for (var i = 0; i < s.Length; i++)
            {
                var key = radixKey.GetKey(s.Read(i));
                if (key < minKey) minKey = key;
                if (key > maxKey) maxKey = key;
            }

            if (minKey == maxKey) return;

            var pow10 = Pow10;
            var range = maxKey - minKey;
            var digitCount = GetDigitCountFromUlong(range, pow10);

            var histograms = allHistograms[..(digitCount * HistogramStride)];
            histograms.Clear();

            for (var i = 0; i < s.Length; i++)
            {
                var quotient = radixKey.GetKey(s.Read(i)) - minKey;
                var block = 0;
                for (var d = 0; d < digitCount; d++)
                {
                    histograms[block + (int)(quotient % RadixBase) + 1]++;
                    quotient /= RadixBase;
                    block += HistogramStride;
                }
            }

            LSDSort(s, temp, radixKey, digitCount, minKey, histograms, pow10);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(tempArray, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void LSDSort<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> temp, TRadixKey radixKey, int digitCount, ulong minKey, Span<int> histograms, ReadOnlySpan<ulong> pow10)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var src = s;
        var dst = temp;

        for (int d = 0; d < digitCount; d++)
        {
            src.Context.OnPhase(SortPhase.RadixPass, d, digitCount);
            var divisor = pow10[d];

            var bucketOffsets = histograms.Slice(d * HistogramStride, HistogramStride);
            for (var i = 1; i <= RadixBase; i++) bucketOffsets[i] += bucketOffsets[i - 1];

            for (var i = 0; i < src.Length; i++)
            {
                var value = src.Read(i);
                var normalizedKey = radixKey.GetKey(value) - minKey;
                dst.Write(bucketOffsets[(int)((normalizedKey / divisor) % RadixBase)]++, value);
            }

            var swapSpan = src; src = dst; dst = swapSpan;
        }

        if ((digitCount & 1) == 1) src.CopyTo(0, s, 0, s.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetDigitCountFromUlong(ulong value, ReadOnlySpan<ulong> pow10)
    {
        if (value == 0) return 1;
        for (int d = 1; d < pow10.Length; d++)
            if (value < pow10[d]) return d;
        return 20;
    }
}
