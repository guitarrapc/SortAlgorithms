using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// Defines an order-preserving mapping from an element to a fixed-width unsigned radix key.
/// This is the core abstraction of the radix-sort family: digit extraction, bucket math, and
/// range calculations all operate on the returned <see cref="ulong"/> key, never on the element itself.
/// Implement as a <see langword="readonly"/> <see langword="struct"/> to enable JIT devirtualization and inlining.
/// </summary>
/// <typeparam name="T">The type of elements from which a key is extracted.</typeparam>
/// <remarks>
/// <para><strong>Contract:</strong></para>
/// <list type="bullet">
/// <item><description><strong>Order definition:</strong> the sort orders elements by the returned key in ascending unsigned order.
/// Elements with equal keys are ties; stable radix implementations keep their input order for ties.</description></item>
/// <item><description><strong>Monotonicity:</strong> a selector that mirrors an existing element order (e.g. the built-in
/// integer and IEEE 754 selectors) must be strictly monotonic with respect to it:
/// if x orders strictly before y, then GetKey(x) &lt; GetKey(y), and equal elements map to equal keys.
/// The second half is the binding one for floating point: -0 and +0 are equal elements, so the built-in
/// IEEE 754 selectors give them the same key rather than the distinct keys the <c>totalOrder</c> predicate
/// would assign. Separating them would leave a stable radix sort reordering elements its declared order
/// calls equal.</description></item>
/// <item><description><strong>Fixed width:</strong> only the low <see cref="KeyBits"/> bits of the key may be non-zero.
/// <see cref="KeyBits"/> must not exceed 64; wider keys are outside this abstraction by design
/// (see the 128-bit rationale in the radix sort class docs).</description></item>
/// <item><description><strong>Purity:</strong> GetKey must return the same key for the same element for the duration of a sort.</description></item>
/// </list>
/// </remarks>
public interface IRadixKeySelector<T>
{
    /// <summary>The number of significant low bits in keys produced by <see cref="GetKey"/> (1..64).</summary>
    static abstract int KeyBits { get; }

    /// <summary>Maps <paramref name="value"/> to its order-preserving unsigned key.</summary>
    ulong GetKey(T value);
}

/// <summary>
/// Validates a selector's declared key width at sort entry.
/// For the built-in selectors the check constant-folds away; for user selectors it rejects
/// widths the ulong key storage cannot represent before any digit math runs on them.
/// </summary>
internal static class RadixKeyGuard
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ValidateKeyBits<T, TRadixKey>() where TRadixKey : struct, IRadixKeySelector<T>
    {
        var keyBits = TRadixKey.KeyBits;
        if (keyBits is < 1 or > 64)
            throw new NotSupportedException($"{typeof(TRadixKey).Name}.KeyBits ({keyBits}) must be between 1 and 64: radix keys are stored as ulong.");
    }
}

/// <summary>
/// Compares elements by their radix key.
/// Used by radix sorts for small-range fallbacks (insertion sort cutoff, comparison-sort fallback)
/// so the fallback orders by exactly the same key as the digit passes — consistency holds by
/// construction for every selector, including floating-point key transforms.
/// </summary>
internal readonly struct RadixKeyComparer<T, TRadixKey>(TRadixKey radixKey) : IComparer<T>
    where TRadixKey : struct, IRadixKeySelector<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Compare(T? x, T? y) => radixKey.GetKey(x!).CompareTo(radixKey.GetKey(y!));
}

/// <summary>
/// Radix key selector for standard binary integer types (byte, sbyte, short, ushort, int, uint,
/// long, ulong, nint, nuint). Signed types are mapped by flipping the sign bit so negative values
/// order before positive values; unsigned types map to themselves.
/// </summary>
/// <remarks>
/// <para>The <c>typeof(T)</c> branches are constant-folded by the JIT per instantiation, so
/// <see cref="GetKey"/> compiles down to a single cast/xor for each concrete type.</para>
/// <para>Sign-bit flipping technique:</para>
/// <list type="bullet">
/// <item><description>int.MinValue → 0x0000_0000 (sorts first), -1 → 0x7FFF_FFFF, 0 → 0x8000_0000, int.MaxValue → 0xFFFF_FFFF (sorts last)</description></item>
/// <item><description>No Abs() needed, avoids MinValue overflow; single unified pass for all values</description></item>
/// </list>
/// <para><see cref="KeyBits"/> throws <see cref="NotSupportedException"/> for 128-bit and other
/// non-standard integer types: the abstraction stores keys in <see cref="ulong"/>, so 64 bits is the ceiling.</para>
/// </remarks>
public readonly struct BinaryIntegerRadixKey<T> : IRadixKeySelector<T> where T : IBinaryInteger<T>
{
    public static int KeyBits
    {
        get
        {
            if (typeof(T) == typeof(byte) || typeof(T) == typeof(sbyte))
                return 8;
            else if (typeof(T) == typeof(short) || typeof(T) == typeof(ushort))
                return 16;
            else if (typeof(T) == typeof(int) || typeof(T) == typeof(uint))
                return 32;
            else if (typeof(T) == typeof(long) || typeof(T) == typeof(ulong))
                return 64;
            else if (typeof(T) == typeof(nint) || typeof(T) == typeof(nuint))
                return IntPtr.Size * 8;
            else if (typeof(T) == typeof(Int128) || typeof(T) == typeof(UInt128))
                throw new NotSupportedException($"Type {typeof(T).Name} with 128-bit size is not supported. Maximum supported bit size is 64.");
            else
                throw new NotSupportedException($"Type {typeof(T).Name} is not supported.");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong GetKey(T value)
    {
        if (typeof(T) == typeof(byte))
        {
            return byte.CreateTruncating(value);
        }
        else if (typeof(T) == typeof(sbyte))
        {
            return (ulong)((byte)sbyte.CreateTruncating(value) ^ 0x80);
        }
        else if (typeof(T) == typeof(short))
        {
            return (ulong)((ushort)short.CreateTruncating(value) ^ 0x8000);
        }
        else if (typeof(T) == typeof(ushort))
        {
            return ushort.CreateTruncating(value);
        }
        else if (typeof(T) == typeof(int))
        {
            return (uint)int.CreateTruncating(value) ^ 0x8000_0000;
        }
        else if (typeof(T) == typeof(uint))
        {
            return uint.CreateTruncating(value);
        }
        else if (typeof(T) == typeof(long))
        {
            return (ulong)long.CreateTruncating(value) ^ 0x8000_0000_0000_0000;
        }
        else if (typeof(T) == typeof(ulong))
        {
            return ulong.CreateTruncating(value);
        }
        else if (typeof(T) == typeof(nint))
        {
            // nint is signed: flip the platform-width sign bit
            return IntPtr.Size == 8
                ? (ulong)nint.CreateTruncating(value) ^ 0x8000_0000_0000_0000
                : (uint)nint.CreateTruncating(value) ^ 0x8000_0000;
        }
        else if (typeof(T) == typeof(nuint))
        {
            return nuint.CreateTruncating(value);
        }
        else
        {
            throw new NotSupportedException($"Type {typeof(T).Name} is not supported.");
        }
    }
}

/// <summary>
/// Adapts a <see cref="Func{T, TResult}"/> returning an <see cref="int"/> key to the radix key contract.
/// The int key is sign-bit flipped into a 32-bit unsigned key, so negative keys order before positive keys
/// and elements with equal keys are grouped (stable radix implementations keep their input order).
/// </summary>
/// <remarks>
/// Note: the underlying delegate call is not devirtualized by the JIT.
/// For maximum performance, implement <see cref="IRadixKeySelector{T}"/> directly as a <see langword="readonly"/> <see langword="struct"/>.
/// </remarks>
internal readonly struct FuncRadixKeySelector<T>(Func<T, int> func) : IRadixKeySelector<T>
{
    public static int KeyBits => 32;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong GetKey(T value) => (uint)func(value) ^ 0x8000_0000;
}

/// <summary>
/// Order-preserving radix key for <see cref="Half"/>, matching the <see cref="IComparable{T}"/> order
/// (all NaN values first, then -∞ .. +∞, with -0 and +0 tied).
/// </summary>
/// <remarks>
/// <para>IEEE 754 bit transform: if the sign bit is set, flip all bits; otherwise flip only the sign bit.
/// NaN (any payload or sign) is normalized to key 0 so it sorts first like <c>Half.CompareTo</c>;
/// no non-NaN value maps to 0 (the smallest non-NaN key is ~0xFC00 for -∞).</para>
/// <para>-0 is normalized to +0 before the transform. The raw transform separates them — it is the IEEE 754
/// <c>totalOrder</c> predicate, under which -0 sorts strictly first — but -0 and +0 are equal under IEEE 754
/// comparison <em>and</em> under <see cref="IComparable{T}"/>, so <see cref="IRadixKeySelector{T}"/> requires
/// them to map to the same key: a selector that separated them would make a stable radix sort reorder elements
/// its own declared order calls equal, and would make the result depend on whether a range fell below an
/// implementation's insertion-sort cutoff. A caller who wants <c>totalOrder</c> can pass a custom selector to
/// the <c>SortBy</c> overloads.</para>
/// </remarks>
public readonly struct HalfRadixKey : IRadixKeySelector<Half>
{
    public static int KeyBits => 16;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong GetKey(Half value)
    {
        if (Half.IsNaN(value)) return 0UL;
        var bits = BitConverter.HalfToUInt16Bits(value);
        // See DoubleRadixKey: this gives -0 the bits of +0, so the two tie.
        if ((ushort)(bits << 1) == 0) bits = 0;
        return (bits & 0x8000) != 0 ? (ushort)~bits : (ushort)(bits ^ 0x8000);
    }
}

/// <summary>
/// Order-preserving radix key for <see cref="float"/>, matching the <see cref="IComparable{T}"/> order
/// (all NaN values first, then -∞ .. +∞, with -0 and +0 tied).
/// See <see cref="HalfRadixKey"/> remarks for the transform and for why -0 and +0 tie.
/// </summary>
public readonly struct SingleRadixKey : IRadixKeySelector<float>
{
    public static int KeyBits => 32;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong GetKey(float value)
    {
        if (float.IsNaN(value)) return 0UL;
        var bits = BitConverter.SingleToUInt32Bits(value);
        // See DoubleRadixKey: this gives -0 the bits of +0, so the two tie.
        if (bits << 1 == 0) bits = 0;
        return (bits & 0x8000_0000) != 0 ? ~bits : bits ^ 0x8000_0000;
    }
}

/// <summary>
/// Order-preserving radix key for <see cref="double"/>, matching the <see cref="IComparable{T}"/> order
/// (all NaN values first, then -∞ .. +∞, with -0 and +0 tied).
/// See <see cref="HalfRadixKey"/> remarks for the transform and for why -0 and +0 tie.
/// </summary>
public readonly struct DoubleRadixKey : IRadixKeySelector<double>
{
    public static int KeyBits => 64;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong GetKey(double value)
    {
        if (double.IsNaN(value)) return 0UL;
        var bits = BitConverter.DoubleToUInt64Bits(value);
        // Give -0 the bits of +0, so the two tie. Shifting the sign bit out leaves zero for exactly the
        // two zeros and for nothing else, so the test needs no 64-bit sign constant. That matters: the
        // three forms measured 1.3%/1.4%, 5.2%/4.8% and 11.8%/12.2% over an unnormalized key on
        // RadixLSD256Sort<double> at n = 65536/1048576 (minimum of 71/41 repetitions,
        // DOTNET_TieredCompilation=0, an int sort in the same run as drift anchor):
        //   this shift test                        1.3% / 1.4%   (inside the anchor's own drift)
        //   comparing bits to the sign constant    5.2% / 4.8%
        //   (value + 0.0) before taking the bits  11.8% / 12.2%
        if (bits << 1 == 0) bits = 0;
        return (bits & 0x8000_0000_0000_0000) != 0 ? ~bits : bits ^ 0x8000_0000_0000_0000;
    }
}
