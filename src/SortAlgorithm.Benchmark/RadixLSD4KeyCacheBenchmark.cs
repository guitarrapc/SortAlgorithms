using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Benchmark;

/// <summary>
/// TEMPORARY measurement harness: memoizing each element's normalized key in a ulong[] and permuting that
/// array alongside the elements, so a pass never recomputes a key, versus holding no keys and recomputing
/// GetKey(value) - min in both the count and the distribute loop. Same algorithm either way; the only
/// difference is memoize vs recompute.
///
/// The two are paired in the SAME run so machine drift cannot be mistaken for the effect.
/// Delete once the numbers are recorded.
/// </summary>
[MemoryDiagnoser]
public class RadixLSD4KeyCacheBenchmark
{
    [Params(1024, 8192, 65_536)]
    public int Size { get; set; }

    /// <summary>
    /// false: values are [0, n), so the key range is narrow and the pass count is small (5-8 passes).
    /// true: values span the whole int range, the worst case at radix 4 (16 passes), where memoizing a
    /// key has the most recomputation to amortize and the most permuting to pay for.
    /// </summary>
    [Params(false, true)]
    public bool FullRange { get; set; }

    private SortBuffers<int> _buffers = default!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(42);
        var source = FullRange
            ? Enumerable.Range(0, Size).Select(_ => random.Next(int.MinValue, int.MaxValue)).ToArray()
            : Enumerable.Range(0, Size).OrderBy(_ => random.Next()).ToArray();
        _buffers = new SortBuffers<int>(source);
    }

    [IterationSetup]
    public void IterationSetup() => _buffers.Reset();

    /// <summary>RadixLSD4Sort as shipped: no key array, each pass recomputes GetKey(value) - min.</summary>
    [Benchmark(Baseline = true)]
    public void Lsd4_Recompute() => RadixLSD4Sort.Sort(_buffers.Next().AsSpan());

    /// <summary>The rejected variant: keys memoized in a ulong[] and permuted with the elements.</summary>
    [Benchmark]
    public void Lsd4_KeyCache() => RadixLSD4SortKeyCacheVariant.Sort(_buffers.Next().AsSpan());
}

/// <summary>
/// RadixLSD4Sort memoizing every element's normalized key in a pair of ping-pong ulong[] buffers, so the
/// count loop reads keys instead of elements and no pass recomputes one. Measured 17-26% slower than the
/// shipped recomputing version across n = 1024/8192/65536 for both narrow and full-range int keys — the
/// memoized key has to be permuted with its element, which costs a second scattered 8-byte write per
/// element per pass. Everything else matches the shipped implementation, so the comparison isolates the
/// memoize-vs-recompute decision. Kept as the subject of that comparison.
/// </summary>
public static class RadixLSD4SortKeyCacheVariant
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
        var executedPasses = 0;

        for (int d = 0; d < digitCount; d++)
        {
            src.Context.OnPhase(SortPhase.RadixPass, d, digitCount);
            var shift = d * RadixBits;

            bucketOffsets.Clear();

            for (var i = 0; i < src.Length; i++)
            {
                bucketOffsets[(int)((srcKeys[i] >> shift) & 0b11) + 1]++;
            }

            if (IsSingleBucket(bucketOffsets, src.Length)) continue;

            for (var i = 1; i <= RadixSize; i++) bucketOffsets[i] += bucketOffsets[i - 1];

            for (var i = 0; i < src.Length; i++)
            {
                var value = src.Read(i);
                var key = srcKeys[i];
                var destIndex = bucketOffsets[(int)((key >> shift) & 0b11)]++;
                dst.Write(destIndex, value);
                dstKeys[destIndex] = key;
            }

            var swapSpan = src;
            src = dst;
            dst = swapSpan;

            var swapKeys = srcKeys;
            srcKeys = dstKeys;
            dstKeys = swapKeys;
            executedPasses++;
        }

        if ((executedPasses & 1) == 1) src.CopyTo(0, s, 0, s.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSingleBucket(ReadOnlySpan<int> bucketOffsets, int length)
    {
        foreach (var count in bucketOffsets[1..])
        {
            if (count != 0) return count == length;
        }
        return false;
    }
}
