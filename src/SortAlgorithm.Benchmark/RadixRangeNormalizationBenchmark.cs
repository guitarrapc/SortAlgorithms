using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using SortAlgorithm.Utils;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Benchmark;

/// <summary>
/// TEMPORARY measurement harness: compares the previous (max XOR min) key-range sizing against the
/// current (max - min) normalization, with both variants in the SAME run so machine drift between
/// runs cannot be mistaken for the effect.
/// Delete once the numbers are recorded.
/// </summary>
[MemoryDiagnoser]
public class RadixRangeNormalizationBenchmark
{
    [Params(1024, 8192)]
    public int Size { get; set; }

    /// <summary>true: values straddle zero ([-n/2, n/2)). false: values are non-negative ([1, n]).</summary>
    [Params(true, false)]
    public bool StraddlesZero { get; set; }

    private SortBuffers<int> _buffers = default!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(42);
        var source = StraddlesZero
            ? ArrayPatterns.GenerateNegativePositiveRandom(Size, random)
            : ArrayPatterns.GenerateRandom(Size, random);
        _buffers = new SortBuffers<int>(source);
    }

    [IterationSetup]
    public void IterationSetup() => _buffers.Reset();

    [Benchmark(Baseline = true)]
    public void Lsd4_Xor() => RadixLSD4SortXorBaseline.Sort(_buffers.Next().AsSpan());

    [Benchmark]
    public void Lsd4_Normalized() => RadixLSD4Sort.Sort(_buffers.Next().AsSpan());

    [Benchmark]
    public void Lsd256_Xor() => RadixLSD256SortXorBaseline.Sort(_buffers.Next().AsSpan());

    [Benchmark]
    public void Lsd256_Normalized() => RadixLSD256Sort.Sort(_buffers.Next().AsSpan());

    [Benchmark]
    public void Lsd10_CopyBack() => RadixLSD10SortCopyBackBaseline.Sort(_buffers.Next().AsSpan());

    [Benchmark]
    public void Lsd10_PingPong() => RadixLSD10Sort.Sort(_buffers.Next().AsSpan());
}

/// <summary>Verbatim copy of RadixLSD10Sort before ping-pong (copy back to source each pass).</summary>
public static class RadixLSD10SortCopyBackBaseline
{
    private const int RadixBase = 10;

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

        var tempArray = ArrayPool<T>.Shared.Rent(span.Length);

        try
        {
            var tempBuffer = tempArray.AsSpan(0, span.Length);

            Span<int> bucketCounts = stackalloc int[RadixBase];
            var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, 0);
            var temp = new SortSpan<T, TComparer, TContext>(tempBuffer, context, comparer, 1);

            var minKey = ulong.MaxValue;
            var maxKey = ulong.MinValue;

            for (var i = 0; i < s.Length; i++)
            {
                var value = s.Read(i);
                var key = radixKey.GetKey(value);
                if (key < minKey) minKey = key;
                if (key > maxKey) maxKey = key;
            }

            if (minKey == maxKey) return;

            ReadOnlySpan<ulong> pow10 = [
                1UL, 10UL, 100UL, 1_000UL, 10_000UL, 100_000UL, 1_000_000UL, 10_000_000UL,
                100_000_000UL, 1_000_000_000UL, 10_000_000_000UL, 100_000_000_000UL,
                1_000_000_000_000UL, 10_000_000_000_000UL, 100_000_000_000_000UL,
                1_000_000_000_000_000UL, 10_000_000_000_000_000UL, 100_000_000_000_000_000UL,
                1_000_000_000_000_000_000UL, 10_000_000_000_000_000_000UL
            ];

            var range = maxKey - minKey;
            var digitCount = GetDigitCountFromUlong(range, pow10);

            LSDSort(s, temp, radixKey, digitCount, minKey, bucketCounts, pow10);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(tempArray, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void LSDSort<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> source, SortSpan<T, TComparer, TContext> temp, TRadixKey radixKey, int digitCount, ulong minKey, Span<int> bucketCounts, ReadOnlySpan<ulong> pow10)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        Span<int> bucketStarts = stackalloc int[RadixBase];

        for (int d = 0; d < digitCount; d++)
        {
            source.Context.OnPhase(SortPhase.RadixPass, d, digitCount);
            var divisor = pow10[d];

            bucketCounts.Clear();

            for (var i = 0; i < source.Length; i++)
            {
                var value = source.Read(i);
                var key = radixKey.GetKey(value);
                var normalizedKey = key - minKey;
                var digit = (int)((normalizedKey / divisor) % 10);
                bucketCounts[digit]++;
            }

            bucketStarts[0] = 0;
            for (var i = 1; i < RadixBase; i++)
            {
                bucketStarts[i] = bucketStarts[i - 1] + bucketCounts[i - 1];
            }

            for (var i = 0; i < source.Length; i++)
            {
                var value = source.Read(i);
                var key = radixKey.GetKey(value);
                var normalizedKey = key - minKey;
                var digit = (int)((normalizedKey / divisor) % 10);
                var pos = bucketStarts[digit]++;
                temp.Write(pos, value);
            }

            temp.CopyTo(0, source, 0, source.Length);
        }
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

/// <summary>Verbatim copy of RadixLSD4Sort before range normalization (int fast path only).</summary>
public static class RadixLSD4SortXorBaseline
{
    private const int RadixBits = 2;
    private const int RadixSize = 4;

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
                var value = s.Read(i);
                var key = radixKey.GetKey(value);
                keys[i] = key;
                if (key < minKey) minKey = key;
                if (key > maxKey) maxKey = key;
            }

            var range = maxKey ^ minKey;
            if (range == 0) return;

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
                var digit = (int)((srcKeys[i] >> shift) & 0b11);
                bucketOffsets[digit + 1]++;
            }

            for (var i = 1; i <= RadixSize; i++)
            {
                bucketOffsets[i] += bucketOffsets[i - 1];
            }

            for (var i = 0; i < src.Length; i++)
            {
                var value = src.Read(i);
                var key = srcKeys[i];
                var digit = (int)((key >> shift) & 0b11);
                var destIndex = bucketOffsets[digit]++;
                dst.Write(destIndex, value);
                dstKeys[destIndex] = key;
            }

            var tempSortSpan = src;
            src = dst;
            dst = tempSortSpan;

            var tempKeys = srcKeys;
            srcKeys = dstKeys;
            dstKeys = tempKeys;
        }

        if ((digitCount & 1) == 1)
        {
            src.CopyTo(0, s, 0, s.Length);
        }
    }
}

/// <summary>Verbatim copy of RadixLSD256Sort before range normalization (int fast path only).</summary>
public static class RadixLSD256SortXorBaseline
{
    private const int RadixBits = 8;
    private const int RadixSize = 256;

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
                var value = s.Read(i);
                var key = radixKey.GetKey(value);
                if (key < minKey) minKey = key;
                if (key > maxKey) maxKey = key;
            }

            var range = maxKey ^ minKey;
            if (range == 0) return;

            var requiredBits = 64 - BitOperations.LeadingZeroCount(range);
            var digitCount = (requiredBits + RadixBits - 1) / RadixBits;

            LSDSort(s, temp, radixKey, digitCount, bucketOffsets);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(tempArray, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
            ArrayPool<int>.Shared.Return(bucketOffsetsArray);
        }
    }

    private static void LSDSort<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> temp, TRadixKey radixKey, int digitCount, Span<int> bucketOffsets)
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
                var value = src.Read(i);
                var key = radixKey.GetKey(value);
                var digit = (int)((key >> shift) & 0xFF);
                bucketOffsets[digit + 1]++;
            }

            for (var i = 1; i <= RadixSize; i++)
            {
                bucketOffsets[i] += bucketOffsets[i - 1];
            }

            for (var i = 0; i < src.Length; i++)
            {
                var value = src.Read(i);
                var key = radixKey.GetKey(value);
                var digit = (int)((key >> shift) & 0xFF);
                var destIndex = bucketOffsets[digit]++;
                dst.Write(destIndex, value);
            }

            var tempSortSpan = src;
            src = dst;
            dst = tempSortSpan;
        }

        if ((digitCount & 1) == 1)
        {
            src.CopyTo(0, s, 0, s.Length);
        }
    }
}
