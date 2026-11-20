using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// The comparator of the <c>*Branchless</c> sorting networks: it reads both positions, compares them once, and writes the
/// minimum to one position and the maximum to the other, whether or not they were already in order.
/// </summary>
/// <remarks>
/// Every context sees two reads, one comparison and two writes per comparator. When nothing observes the sort and the key is a
/// primitive type in its natural order, the minimum and maximum are selected in a 128-bit vector register, so no branch depends
/// on the data. Otherwise the comparison's result selects them, which the JIT may compile to a branch.
/// </remarks>
internal static class BranchlessComparator
{
    /// <summary>
    /// Writes the smaller of s[<paramref name="lower"/>] and s[<paramref name="upper"/>] to <paramref name="lower"/> and the larger to
    /// <paramref name="upper"/>, always writing both.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Ascending<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int lower, int upper)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (typeof(TContext) == typeof(NullContext) && UseVectorSelect<T, TComparer>(s.Comparer))
        {
            WriteMinMax(s, lower, upper, minAt: lower, maxAt: upper);
            return;
        }

        var greater = s.IsGreaterAt(lower, upper, out var a, out var b);
        s.Write(lower, greater ? b : a);
        s.Write(upper, greater ? a : b);
    }

    /// <summary>
    /// Writes the larger of s[<paramref name="lower"/>] and s[<paramref name="upper"/>] to <paramref name="lower"/> and the smaller to
    /// <paramref name="upper"/>, always writing both.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Descending<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int lower, int upper)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (typeof(TContext) == typeof(NullContext) && UseVectorSelect<T, TComparer>(s.Comparer))
        {
            WriteMinMax(s, lower, upper, minAt: upper, maxAt: lower);
            return;
        }

        var less = s.IsLessAt(lower, upper, out var a, out var b);
        s.Write(lower, less ? b : a);
        s.Write(upper, less ? a : b);
    }

    /// <summary>
    /// True when <typeparamref name="T"/> is a primitive type that <see cref="Vector128{T}"/> supports, compared in its natural
    /// order, and 128-bit vectors are hardware accelerated. Both operands are JIT-time constants for a struct comparer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool UseVectorSelect<T, TComparer>(TComparer comparer)
        where TComparer : IComparer<T>
        => Vector128.IsHardwareAccelerated && comparer is IComparableComparer
           && (typeof(T) == typeof(byte) || typeof(T) == typeof(sbyte)
            || typeof(T) == typeof(ushort) || typeof(T) == typeof(short)
            || typeof(T) == typeof(uint) || typeof(T) == typeof(int)
            || typeof(T) == typeof(ulong) || typeof(T) == typeof(long)
            || typeof(T) == typeof(nuint) || typeof(T) == typeof(nint)
            || typeof(T) == typeof(float) || typeof(T) == typeof(double));

    /// <summary>
    /// Reads s[<paramref name="lower"/>] and s[<paramref name="upper"/>], selects their minimum and maximum in a vector register in the
    /// order <see cref="IComparable{T}.CompareTo"/> defines, and writes them to <paramref name="minAt"/> and <paramref name="maxAt"/>.
    /// Only for the types <see cref="UseVectorSelect{T, TComparer}"/> accepts.
    /// </summary>
    /// <remarks>
    /// The results are written straight from the vector register. Returned through <c>out</c> parameters, they went through a
    /// general-purpose register on the way to memory, and the sorts measured slower.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteMinMax<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int lower, int upper, int minAt, int maxAt)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var a = s.Read(lower);
        var b = s.Read(upper);

        // CompareTo puts NaN below every number, which the vector minimum and maximum of floating-point values do not.
        // a > b in that order is: a is not NaN, and a <= b does not hold (false when b is NaN).
        if (typeof(T) == typeof(double))
        {
            var va = Vector128.CreateScalarUnsafe(Unsafe.BitCast<T, double>(a));
            var vb = Vector128.CreateScalarUnsafe(Unsafe.BitCast<T, double>(b));
            var greater = Vector128.AndNot(Vector128.Equals(va, va), Vector128.LessThanOrEqual(va, vb));
            s.Write(minAt, Unsafe.BitCast<double, T>(Vector128.ConditionalSelect(greater, vb, va).ToScalar()));
            s.Write(maxAt, Unsafe.BitCast<double, T>(Vector128.ConditionalSelect(greater, va, vb).ToScalar()));
            return;
        }
        if (typeof(T) == typeof(float))
        {
            var va = Vector128.CreateScalarUnsafe(Unsafe.BitCast<T, float>(a));
            var vb = Vector128.CreateScalarUnsafe(Unsafe.BitCast<T, float>(b));
            var greater = Vector128.AndNot(Vector128.Equals(va, va), Vector128.LessThanOrEqual(va, vb));
            s.Write(minAt, Unsafe.BitCast<float, T>(Vector128.ConditionalSelect(greater, vb, va).ToScalar()));
            s.Write(maxAt, Unsafe.BitCast<float, T>(Vector128.ConditionalSelect(greater, va, vb).ToScalar()));
            return;
        }

        var x = Vector128.CreateScalarUnsafe(a);
        var y = Vector128.CreateScalarUnsafe(b);
        s.Write(minAt, Vector128.Min(x, y).ToScalar());
        s.Write(maxAt, Vector128.Max(x, y).ToScalar());
    }
}
