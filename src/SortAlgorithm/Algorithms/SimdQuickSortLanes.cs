using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// The vector operations <see cref="SimdQuickSort"/> uses, implemented once per vector width. A mask is a vector whose selected lanes have
/// every bit set; the methods ending in Bits return one bit per lane, lane 0 in bit 0.
/// </summary>
internal interface ISimdQuickSortLanes<TVec, T>
{
    /// <summary>Whether this width can run on this machine.</summary>
    static abstract bool IsSupported { get; }
    static abstract int Count { get; }
    static abstract TVec Load(ref T source);
    static abstract void Store(ref T destination, TVec v);
    static abstract TVec Broadcast(T value);
    /// <summary>The largest value of <typeparamref name="T"/> in every lane.</summary>
    static abstract TVec Padding { get; }
    static abstract TVec Min(TVec a, TVec b);
    static abstract TVec Max(TVec a, TVec b);
    static abstract TVec LessOrEqual(TVec a, TVec b);
    static abstract TVec Greater(TVec a, TVec b);
    static abstract TVec And(TVec a, TVec b);
    /// <summary>A mask of the first <paramref name="n"/> lanes.</summary>
    static abstract TVec FirstLanes(int n);
    static abstract int PopCount(TVec mask);
    static abstract ulong LessBits(TVec a, TVec b);
    static abstract ulong GreaterBits(TVec a, TVec b);
    /// <summary>
    /// Stores the <paramref name="count"/> lanes of <paramref name="v"/> selected by <paramref name="mask"/>, packed in lane order, and nothing
    /// else.
    /// </summary>
    static abstract unsafe void CompressStore(byte* destination, TVec mask, TVec v, int count);
    /// <summary>Moves lane l ^ <paramref name="partner"/> into lane l.</summary>
    static abstract TVec Partner(TVec v, int partner);
    static abstract TVec Reverse(TVec v);
    /// <summary>Takes <paramref name="hi"/> in the lanes whose <paramref name="bit"/> is set and <paramref name="lo"/> in the others.</summary>
    static abstract TVec Blend(TVec lo, TVec hi, int bit);
    /// <summary>One bit for each lane whose <paramref name="bit"/> is clear.</summary>
    static abstract ulong LanesWithout(int bit);
}

/// <summary>
/// 512-bit vectors, stored with the AVX-512F compress and a masked store.
/// </summary>
internal readonly struct SimdQuickSortLanes512<T> : ISimdQuickSortLanes<Vector512<T>, T>
{
    public static bool IsSupported => Avx512F.IsSupported && Vector512.IsHardwareAccelerated;
    public static int Count => Vector512<T>.Count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector512<T> Load(ref T source) => Vector512.LoadUnsafe(ref source);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Store(ref T destination, Vector512<T> v) => v.StoreUnsafe(ref destination);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector512<T> Broadcast(T value) => Vector512.Create(value);

    public static Vector512<T> Padding
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => !SimdQuickSortLanes.IsSigned<T>() ? Vector512<T>.AllBitsSet
            : Unsafe.SizeOf<T>() == 4 ? Vector512.ShiftRightLogical(Vector512<uint>.AllBitsSet, 1).As<uint, T>()
            : Vector512.ShiftRightLogical(Vector512<ulong>.AllBitsSet, 1).As<ulong, T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector512<T> Min(Vector512<T> a, Vector512<T> b) => Vector512.Min(a, b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector512<T> Max(Vector512<T> a, Vector512<T> b) => Vector512.Max(a, b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector512<T> LessOrEqual(Vector512<T> a, Vector512<T> b) => Vector512.LessThanOrEqual(a, b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector512<T> And(Vector512<T> a, Vector512<T> b) => a & b;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector512<T> Greater(Vector512<T> a, Vector512<T> b) => Vector512.GreaterThan(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector512<T> FirstLanes(int n)
        => Unsafe.SizeOf<T>() == 4
            ? Vector512.LessThan(Vector512<int>.Indices, Vector512.Create(n)).As<int, T>()
            : Vector512.LessThan(Vector512<long>.Indices, Vector512.Create((long)n)).As<long, T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int PopCount(Vector512<T> mask) => BitOperations.PopCount(mask.ExtractMostSignificantBits());
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ulong LessBits(Vector512<T> a, Vector512<T> b) => Vector512.LessThan(a, b).ExtractMostSignificantBits();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ulong GreaterBits(Vector512<T> a, Vector512<T> b) => Vector512.GreaterThan(a, b).ExtractMostSignificantBits();

    // Compressing in a register and storing the first count lanes writes the same lanes as a compress to memory, and measured faster.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void CompressStore(byte* destination, Vector512<T> mask, Vector512<T> v, int count)
    {
        if (Unsafe.SizeOf<T>() == 4) Avx512F.MaskStore((int*)destination, FirstLanes(count).AsInt32(), Avx512F.Compress(Vector512<int>.Zero, mask.AsInt32(), v.AsInt32()));
        else Avx512F.MaskStore((long*)destination, FirstLanes(count).AsInt64(), Avx512F.Compress(Vector512<long>.Zero, mask.AsInt64(), v.AsInt64()));
    }

    // A 64-bit lane l is the 32-bit lanes 2l and 2l + 1, so lane l ^ x is 32-bit lane j ^ 2x, and lane l has bit b set when 32-bit lane j has 2b set.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector512<T> Partner(Vector512<T> v, int partner)
        => Vector512.Shuffle(v.AsInt32(), Vector512<int>.Indices ^ Vector512.Create(partner * (Unsafe.SizeOf<T>() / 4))).As<int, T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector512<T> Reverse(Vector512<T> v) => Partner(v, Count - 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector512<T> Blend(Vector512<T> lo, Vector512<T> hi, int bit)
    {
        var low = Vector512.Equals(Vector512<int>.Indices & Vector512.Create(bit * (Unsafe.SizeOf<T>() / 4)), Vector512<int>.Zero);
        return Vector512.ConditionalSelect(low.As<int, T>(), lo, hi);
    }

    public static ulong LanesWithout(int bit) => SimdQuickSortLanes.LanesWithout(Count, bit);
}

/// <summary>
/// 256-bit vectors, stored with the AVX-512VL compress and a masked store where they exist, and otherwise with a lane permutation from
/// <see cref="SimdQuickSortLanes"/> followed by an AVX2 masked store of the selected lanes.
/// </summary>
internal readonly struct SimdQuickSortLanes256<T> : ISimdQuickSortLanes<Vector256<T>, T>
{
    public static bool IsSupported => Avx2.IsSupported && Vector256.IsHardwareAccelerated;
    public static int Count => Vector256<T>.Count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector256<T> Load(ref T source) => Vector256.LoadUnsafe(ref source);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Store(ref T destination, Vector256<T> v) => v.StoreUnsafe(ref destination);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector256<T> Broadcast(T value) => Vector256.Create(value);

    public static Vector256<T> Padding
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => !SimdQuickSortLanes.IsSigned<T>() ? Vector256<T>.AllBitsSet
            : Unsafe.SizeOf<T>() == 4 ? Vector256.ShiftRightLogical(Vector256<uint>.AllBitsSet, 1).As<uint, T>()
            : Vector256.ShiftRightLogical(Vector256<ulong>.AllBitsSet, 1).As<ulong, T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector256<T> Min(Vector256<T> a, Vector256<T> b) => Vector256.Min(a, b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector256<T> Max(Vector256<T> a, Vector256<T> b) => Vector256.Max(a, b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector256<T> LessOrEqual(Vector256<T> a, Vector256<T> b) => Vector256.LessThanOrEqual(a, b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector256<T> And(Vector256<T> a, Vector256<T> b) => a & b;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector256<T> Greater(Vector256<T> a, Vector256<T> b) => Vector256.GreaterThan(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<T> FirstLanes(int n)
        => Unsafe.SizeOf<T>() == 4
            ? Vector256.LessThan(Vector256<int>.Indices, Vector256.Create(n)).As<int, T>()
            : Vector256.LessThan(Vector256<long>.Indices, Vector256.Create((long)n)).As<long, T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int PopCount(Vector256<T> mask) => BitOperations.PopCount(mask.ExtractMostSignificantBits());
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ulong LessBits(Vector256<T> a, Vector256<T> b) => Vector256.LessThan(a, b).ExtractMostSignificantBits();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ulong GreaterBits(Vector256<T> a, Vector256<T> b) => Vector256.GreaterThan(a, b).ExtractMostSignificantBits();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void CompressStore(byte* destination, Vector256<T> mask, Vector256<T> v, int count)
    {
        if (Avx512F.VL.IsSupported)
        {
            if (Unsafe.SizeOf<T>() == 4) Avx512F.VL.MaskStore((int*)destination, FirstLanes(count).AsInt32(), Avx512F.VL.Compress(Vector256<int>.Zero, mask.AsInt32(), v.AsInt32()));
            else Avx512F.VL.MaskStore((long*)destination, FirstLanes(count).AsInt64(), Avx512F.VL.Compress(Vector256<long>.Zero, mask.AsInt64(), v.AsInt64()));
            return;
        }

        // The table entry lists the selected lanes as 32-bit lane indices; a 64-bit lane contributes both of its halves.
        var bits = (int)mask.ExtractMostSignificantBits();
        var table = Unsafe.SizeOf<T>() == 4 ? SimdQuickSortLanes.Pack8x32 : SimdQuickSortLanes.Pack4x64;
        var entry = Vector128.CreateScalarUnsafe(Unsafe.ReadUnaligned<ulong>(ref table[bits * 8])).AsByte();
        var packed = Avx2.PermuteVar8x32(v.AsInt32(), Avx2.ConvertToVector256Int32(entry));
        var ints = count * (Unsafe.SizeOf<T>() / 4);
        Avx2.MaskStore((int*)destination, Vector256.LessThan(Vector256<int>.Indices, Vector256.Create(ints)), packed);
    }

    // A 64-bit lane l is the 32-bit lanes 2l and 2l + 1, so lane l ^ x is 32-bit lane j ^ 2x, and lane l has bit b set when 32-bit lane j has 2b set.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<T> Partner(Vector256<T> v, int partner)
        => Vector256.Shuffle(v.AsInt32(), Vector256<int>.Indices ^ Vector256.Create(partner * (Unsafe.SizeOf<T>() / 4))).As<int, T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector256<T> Reverse(Vector256<T> v) => Partner(v, Count - 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<T> Blend(Vector256<T> lo, Vector256<T> hi, int bit)
    {
        var low = Vector256.Equals(Vector256<int>.Indices & Vector256.Create(bit * (Unsafe.SizeOf<T>() / 4)), Vector256<int>.Zero);
        return Vector256.ConditionalSelect(low.As<int, T>(), lo, hi);
    }

    public static ulong LanesWithout(int bit) => SimdQuickSortLanes.LanesWithout(Count, bit);
}

/// <summary>
/// Helpers and tables shared by the <see cref="SimdQuickSort"/> lane types.
/// </summary>
internal static class SimdQuickSortLanes
{
    /// <summary>
    /// Entry m (8 bytes) lists, in ascending order, the lanes of an eight-lane 32-bit vector whose bit is set in m; the rest of the entry is 0.
    /// </summary>
    internal static readonly byte[] Pack8x32 = BuildPack(8, 1);

    /// <summary>
    /// Entry m (8 bytes) lists, in ascending order, the 32-bit halves of the lanes of a four-lane 64-bit vector whose bit is set in m.
    /// </summary>
    internal static readonly byte[] Pack4x64 = BuildPack(4, 2);

    static byte[] BuildPack(int lanes, int halves)
    {
        var table = new byte[(1 << lanes) * 8];
        for (var m = 0; m < 1 << lanes; m++)
        {
            var k = 0;
            for (var l = 0; l < lanes; l++)
            {
                if ((m >> l & 1) == 0) continue;
                for (var h = 0; h < halves; h++)
                {
                    table[m * 8 + k++] = (byte)(l * halves + h);
                }
            }
        }
        return table;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsSigned<T>()
        => typeof(T) == typeof(int) || typeof(T) == typeof(long) || typeof(T) == typeof(nint);

    internal static ulong LanesWithout(int count, int bit)
    {
        ulong lanes = 0;
        for (var l = 0; l < count; l++)
        {
            if ((l & bit) == 0) lanes |= 1UL << l;
        }
        return lanes;
    }
}
