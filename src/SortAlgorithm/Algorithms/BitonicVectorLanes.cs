using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// The vector operations <see cref="BitonicSortAnyLengthSimd"/> uses, implemented once per vector width with the cross-platform vector API.
/// A mask is a vector whose selected lanes have every bit set; the methods ending in Bits return one bit per lane, lane 0 in bit 0.
/// </summary>
internal interface IBitonicLanes<TVec, T>
{
    static abstract bool IsSupported { get; }
    static abstract int Count { get; }
    static abstract TVec Load(ref T source);
    static abstract void Store(ref T destination, TVec v);
    /// <summary><paramref name="value"/> in every lane.</summary>
    static abstract TVec Broadcast(T value);
    static abstract TVec Min(TVec a, TVec b);
    static abstract TVec Max(TVec a, TVec b);
    static abstract ulong LessBits(TVec a, TVec b);
    static abstract ulong GreaterBits(TVec a, TVec b);
    /// <summary>Moves lane l ^ <paramref name="partner"/> into lane l.</summary>
    static abstract TVec Partner(TVec v, int partner);
    /// <summary>Takes <paramref name="a"/> in the lanes <paramref name="mask"/> selects and <paramref name="b"/> in the others.</summary>
    static abstract TVec Select(TVec mask, TVec a, TVec b);
    static abstract TVec Xor(TVec a, TVec b);
    static abstract TVec Not(TVec mask);
    static abstract TVec None { get; }
    /// <summary>The lanes whose <paramref name="bit"/> is set.</summary>
    static abstract TVec Upper(int bit);
    /// <summary>
    /// The lanes whose block of 2^<paramref name="log2Block"/> lanes is merged descending when the whole vector is sorted ascending with
    /// Lang's recursion, in which every first half is sorted opposite to its range: the root block is ascending, and each block is descending
    /// when the path to it from the root takes the first half an odd number of times.
    /// </summary>
    static abstract TVec DescendingBlocks(int log2Block);
    /// <summary>One bit for each lane whose <paramref name="bit"/> is clear.</summary>
    static abstract ulong LanesWithout(int bit);
}

internal readonly struct BitonicLanes512<T> : IBitonicLanes<Vector512<T>, T>
{
    public static bool IsSupported => Vector512.IsHardwareAccelerated;
    public static int Count => Vector512<T>.Count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector512<T> Load(ref T source) => Vector512.LoadUnsafe(ref source);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector512<T> Broadcast(T value) => Vector512.Create(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Store(ref T destination, Vector512<T> v) => v.StoreUnsafe(ref destination);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector512<T> Min(Vector512<T> a, Vector512<T> b) => Vector512.Min(a, b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector512<T> Max(Vector512<T> a, Vector512<T> b) => Vector512.Max(a, b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ulong LessBits(Vector512<T> a, Vector512<T> b) => Vector512.LessThan(a, b).ExtractMostSignificantBits();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ulong GreaterBits(Vector512<T> a, Vector512<T> b) => Vector512.GreaterThan(a, b).ExtractMostSignificantBits();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector512<T> Select(Vector512<T> mask, Vector512<T> a, Vector512<T> b) => Vector512.ConditionalSelect(mask, a, b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector512<T> Xor(Vector512<T> a, Vector512<T> b) => a ^ b;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector512<T> Not(Vector512<T> mask) => ~mask;
    public static Vector512<T> None => Vector512<T>.Zero;

    // A 64-bit lane l is the 32-bit lanes 2l and 2l + 1, so lane l ^ x is 32-bit lane j ^ 2x.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector512<T> Partner(Vector512<T> v, int partner)
        => Vector512.Shuffle(v.AsInt32(), Vector512<int>.Indices ^ Vector512.Create(partner * (Unsafe.SizeOf<T>() / 4))).As<int, T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector512<T> Upper(int bit)
        => Unsafe.SizeOf<T>() == 4
            ? (~Vector512.Equals(Vector512<int>.Indices & Vector512.Create(bit), Vector512<int>.Zero)).As<int, T>()
            : (~Vector512.Equals(Vector512<long>.Indices & Vector512.Create((long)bit), Vector512<long>.Zero)).As<long, T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector512<T> DescendingBlocks(int log2Block)
    {
        // The block index t has k = log2(W) - log2Block bits; the path takes the first half once per 0 bit, so the parity of the
        // number of 0 bits is (k & 1) ^ parity(t). t has at most 4 bits.
        var k = BitOperations.Log2((uint)Count) - log2Block;
        if (Unsafe.SizeOf<T>() == 4)
        {
            var t = Vector512<int>.Indices >>> log2Block;
            var parity = (t ^ (t >>> 1) ^ (t >>> 2) ^ (t >>> 3)) & Vector512<int>.One;
            return Vector512.Equals(parity ^ Vector512.Create(k & 1), Vector512<int>.One).As<int, T>();
        }
        else
        {
            var t = Vector512<long>.Indices >>> log2Block;
            var parity = (t ^ (t >>> 1) ^ (t >>> 2) ^ (t >>> 3)) & Vector512<long>.One;
            return Vector512.Equals(parity ^ Vector512.Create((long)(k & 1)), Vector512<long>.One).As<long, T>();
        }
    }

    public static ulong LanesWithout(int bit) => BitonicLanes.LanesWithout(Count, bit);
}

internal readonly struct BitonicLanes256<T> : IBitonicLanes<Vector256<T>, T>
{
    public static bool IsSupported => Vector256.IsHardwareAccelerated;
    public static int Count => Vector256<T>.Count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector256<T> Load(ref T source) => Vector256.LoadUnsafe(ref source);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector256<T> Broadcast(T value) => Vector256.Create(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Store(ref T destination, Vector256<T> v) => v.StoreUnsafe(ref destination);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector256<T> Min(Vector256<T> a, Vector256<T> b) => Vector256.Min(a, b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector256<T> Max(Vector256<T> a, Vector256<T> b) => Vector256.Max(a, b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ulong LessBits(Vector256<T> a, Vector256<T> b) => Vector256.LessThan(a, b).ExtractMostSignificantBits();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ulong GreaterBits(Vector256<T> a, Vector256<T> b) => Vector256.GreaterThan(a, b).ExtractMostSignificantBits();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector256<T> Select(Vector256<T> mask, Vector256<T> a, Vector256<T> b) => Vector256.ConditionalSelect(mask, a, b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector256<T> Xor(Vector256<T> a, Vector256<T> b) => a ^ b;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector256<T> Not(Vector256<T> mask) => ~mask;
    public static Vector256<T> None => Vector256<T>.Zero;

    // A 64-bit lane l is the 32-bit lanes 2l and 2l + 1, so lane l ^ x is 32-bit lane j ^ 2x.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<T> Partner(Vector256<T> v, int partner)
        => Vector256.Shuffle(v.AsInt32(), Vector256<int>.Indices ^ Vector256.Create(partner * (Unsafe.SizeOf<T>() / 4))).As<int, T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<T> Upper(int bit)
        => Unsafe.SizeOf<T>() == 4
            ? (~Vector256.Equals(Vector256<int>.Indices & Vector256.Create(bit), Vector256<int>.Zero)).As<int, T>()
            : (~Vector256.Equals(Vector256<long>.Indices & Vector256.Create((long)bit), Vector256<long>.Zero)).As<long, T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<T> DescendingBlocks(int log2Block)
    {
        var k = BitOperations.Log2((uint)Count) - log2Block;
        if (Unsafe.SizeOf<T>() == 4)
        {
            var t = Vector256<int>.Indices >>> log2Block;
            var parity = (t ^ (t >>> 1) ^ (t >>> 2) ^ (t >>> 3)) & Vector256<int>.One;
            return Vector256.Equals(parity ^ Vector256.Create(k & 1), Vector256<int>.One).As<int, T>();
        }
        else
        {
            var t = Vector256<long>.Indices >>> log2Block;
            var parity = (t ^ (t >>> 1) ^ (t >>> 2) ^ (t >>> 3)) & Vector256<long>.One;
            return Vector256.Equals(parity ^ Vector256.Create((long)(k & 1)), Vector256<long>.One).As<long, T>();
        }
    }

    public static ulong LanesWithout(int bit) => BitonicLanes.LanesWithout(Count, bit);
}

internal readonly struct BitonicLanes128<T> : IBitonicLanes<Vector128<T>, T>
{
    public static bool IsSupported => Vector128.IsHardwareAccelerated;
    public static int Count => Vector128<T>.Count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector128<T> Load(ref T source) => Vector128.LoadUnsafe(ref source);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector128<T> Broadcast(T value) => Vector128.Create(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Store(ref T destination, Vector128<T> v) => v.StoreUnsafe(ref destination);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector128<T> Min(Vector128<T> a, Vector128<T> b) => Vector128.Min(a, b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector128<T> Max(Vector128<T> a, Vector128<T> b) => Vector128.Max(a, b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ulong LessBits(Vector128<T> a, Vector128<T> b) => Vector128.LessThan(a, b).ExtractMostSignificantBits();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ulong GreaterBits(Vector128<T> a, Vector128<T> b) => Vector128.GreaterThan(a, b).ExtractMostSignificantBits();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector128<T> Select(Vector128<T> mask, Vector128<T> a, Vector128<T> b) => Vector128.ConditionalSelect(mask, a, b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector128<T> Xor(Vector128<T> a, Vector128<T> b) => a ^ b;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector128<T> Not(Vector128<T> mask) => ~mask;
    public static Vector128<T> None => Vector128<T>.Zero;

    // A 64-bit lane l is the 32-bit lanes 2l and 2l + 1, so lane l ^ x is 32-bit lane j ^ 2x.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<T> Partner(Vector128<T> v, int partner)
        => Vector128.Shuffle(v.AsInt32(), Vector128<int>.Indices ^ Vector128.Create(partner * (Unsafe.SizeOf<T>() / 4))).As<int, T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<T> Upper(int bit)
        => Unsafe.SizeOf<T>() == 4
            ? (~Vector128.Equals(Vector128<int>.Indices & Vector128.Create(bit), Vector128<int>.Zero)).As<int, T>()
            : (~Vector128.Equals(Vector128<long>.Indices & Vector128.Create((long)bit), Vector128<long>.Zero)).As<long, T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<T> DescendingBlocks(int log2Block)
    {
        var k = BitOperations.Log2((uint)Count) - log2Block;
        if (Unsafe.SizeOf<T>() == 4)
        {
            var t = Vector128<int>.Indices >>> log2Block;
            var parity = (t ^ (t >>> 1) ^ (t >>> 2) ^ (t >>> 3)) & Vector128<int>.One;
            return Vector128.Equals(parity ^ Vector128.Create(k & 1), Vector128<int>.One).As<int, T>();
        }
        else
        {
            var t = Vector128<long>.Indices >>> log2Block;
            var parity = (t ^ (t >>> 1) ^ (t >>> 2) ^ (t >>> 3)) & Vector128<long>.One;
            return Vector128.Equals(parity ^ Vector128.Create((long)(k & 1)), Vector128<long>.One).As<long, T>();
        }
    }

    public static ulong LanesWithout(int bit) => BitonicLanes.LanesWithout(Count, bit);
}

internal static class BitonicLanes
{
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
