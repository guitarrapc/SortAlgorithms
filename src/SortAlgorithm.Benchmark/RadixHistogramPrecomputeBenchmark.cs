using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Benchmark;

/// <summary>
/// TEMPORARY measurement harness: compares counting each digit inside its own pass against building
/// every digit's histogram in one preprocessing pass, with both variants in the SAME run so machine
/// drift between runs cannot be mistaken for the effect.
/// Delete once the numbers are recorded.
/// </summary>
[MemoryDiagnoser]
public class RadixHistogramPrecomputeBenchmark
{
    [Params(1024, 8192)]
    public int Size { get; set; }

    /// <summary>
    /// Byte-width of the key range, which is what decides how many digit passes RadixLSD256Sort runs.
    /// The point of precomputing histograms is to trade a fixed extra preprocessing pass for one
    /// saved read pass per digit, so the trade only pays from some pass count upward — this is the axis
    /// that has to vary. (RadixLSD10Sort sees 3, 5, 8 and 10 decimal digits for these four widths.)
    /// </summary>
    [Params(1, 2, 3, 4)]
    public int RadixDigits { get; set; }

    private SortBuffers<int> _buffers = default!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(42);
        var source = new int[Size];
        for (var i = 0; i < source.Length; i++)
        {
            source[i] = RadixDigits switch
            {
                1 => random.Next(0, 1 << 8),
                2 => random.Next(0, 1 << 16),
                3 => random.Next(0, 1 << 24),
                _ => random.Next(int.MinValue, int.MaxValue),
            };
        }
        _buffers = new SortBuffers<int>(source);
    }

    [IterationSetup]
    public void IterationSetup() => _buffers.Reset();

    /// <summary>RadixLSD256Sort as shipped: each pass counts its own digit.</summary>
    [Benchmark(Baseline = true)]
    public void Lsd256_CountPerPass() => RadixLSD256Sort.Sort(_buffers.Next().AsSpan());

    /// <summary>The rejected variant, kept here so the comparison can be re-run.</summary>
    [Benchmark]
    public void Lsd256_Histogram() => RadixLSD256SortHistogramVariant.Sort(_buffers.Next().AsSpan());

    /// <summary>RadixLSD10Sort before histogram precomputation.</summary>
    [Benchmark]
    public void Lsd10_CountPerPass() => RadixLSD10SortCountPerPassBaseline.Sort(_buffers.Next().AsSpan());

    /// <summary>RadixLSD10Sort as shipped.</summary>
    [Benchmark]
    public void Lsd10_Histogram() => RadixLSD10Sort.Sort(_buffers.Next().AsSpan());
}

/// <summary>
/// RadixLSD256Sort with every digit's histogram built in one preprocessing pass (2n + d×n reads).
/// Measured 16-38% slower than the shipped per-pass counting across n = 1024/8192 and d = 1..4, so it was
/// not adopted; kept here as the subject of that comparison.
/// </summary>
public static class RadixLSD256SortHistogramVariant
{
    private const int RadixBits = 8;
    private const int RadixSize = 256;
    private const int HistogramStride = RadixSize + 1;

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
        int[]? histogramsArray = null;

        try
        {
            var tempBuffer = tempArray.AsSpan(0, span.Length);
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

            histogramsArray = ArrayPool<int>.Shared.Rent(digitCount * HistogramStride);
            var histograms = histogramsArray.AsSpan(0, digitCount * HistogramStride);
            histograms.Clear();

            for (var i = 0; i < s.Length; i++)
            {
                var remaining = radixKey.GetKey(s.Read(i)) - minKey;
                var block = 0;
                for (var d = 0; d < digitCount; d++)
                {
                    histograms[block + (int)(remaining & 0xFF) + 1]++;
                    remaining >>= RadixBits;
                    block += HistogramStride;
                }
            }

            LSDSort(s, temp, radixKey, digitCount, minKey, histograms);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(tempArray, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
            if (histogramsArray is not null) ArrayPool<int>.Shared.Return(histogramsArray);
        }
    }

    private static void LSDSort<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> temp, TRadixKey radixKey, int digitCount, ulong minKey, Span<int> histograms)
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

            var bucketOffsets = histograms.Slice(d * HistogramStride, HistogramStride);

            for (var i = 1; i <= RadixSize; i++)
            {
                bucketOffsets[i] += bucketOffsets[i - 1];
            }

            for (var i = 0; i < src.Length; i++)
            {
                var value = src.Read(i);
                var key = radixKey.GetKey(value) - minKey;
                var digit = (int)((key >> shift) & 0xFF);
                dst.Write(bucketOffsets[digit]++, value);
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

/// <summary>Verbatim copy of RadixLSD10Sort before histogram precomputation (counts inside each pass).</summary>
public static class RadixLSD10SortCountPerPassBaseline
{
    private const int RadixBase = 10;

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

            Span<int> bucketCounts = stackalloc int[RadixBase];
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

            LSDSort(s, temp, radixKey, digitCount, minKey, bucketCounts, pow10);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(tempArray, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void LSDSort<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> temp, TRadixKey radixKey, int digitCount, ulong minKey, Span<int> bucketCounts, ReadOnlySpan<ulong> pow10)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        Span<int> bucketStarts = stackalloc int[RadixBase];

        var src = s;
        var dst = temp;

        for (int d = 0; d < digitCount; d++)
        {
            src.Context.OnPhase(SortPhase.RadixPass, d, digitCount);
            var divisor = pow10[d];

            bucketCounts.Clear();

            for (var i = 0; i < src.Length; i++)
            {
                var normalizedKey = radixKey.GetKey(src.Read(i)) - minKey;
                var digit = (int)((normalizedKey / divisor) % 10);
                bucketCounts[digit]++;
            }

            bucketStarts[0] = 0;
            for (var i = 1; i < RadixBase; i++)
            {
                bucketStarts[i] = bucketStarts[i - 1] + bucketCounts[i - 1];
            }

            for (var i = 0; i < src.Length; i++)
            {
                var value = src.Read(i);
                var normalizedKey = radixKey.GetKey(value) - minKey;
                var digit = (int)((normalizedKey / divisor) % 10);
                dst.Write(bucketStarts[digit]++, value);
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetDigitCountFromUlong(ulong value, ReadOnlySpan<ulong> pow10)
    {
        if (value == 0) return 1;
        for (int d = 1; d < pow10.Length; d++)
            if (value < pow10[d]) return d;
        return 20;
    }
}
