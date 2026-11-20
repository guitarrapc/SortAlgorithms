using SortAlgorithm.Contexts;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// The vector operations the SIMD selection sorts use, implemented once per vector width so that each scan is written once.
/// Each mask method returns one bit per lane, lane 0 in bit 0.
/// </summary>
internal interface IVectorLanes<TVec, T>
{
    static abstract int Count { get; }
    static abstract TVec Load(ref T source);
    static abstract TVec Broadcast(T value);
    static abstract TVec Min(TVec a, TVec b);
    static abstract TVec Max(TVec a, TVec b);
    static abstract ulong Less(TVec a, TVec b);
    static abstract ulong Greater(TVec a, TVec b);
    static abstract ulong Equal(TVec a, TVec b);
}

internal readonly struct Vector128Lanes<T> : IVectorLanes<Vector128<T>, T>
{
    public static int Count => Vector128<T>.Count;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector128<T> Load(ref T source) => Vector128.LoadUnsafe(ref source);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector128<T> Broadcast(T value) => Vector128.Create(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector128<T> Min(Vector128<T> a, Vector128<T> b) => Vector128.Min(a, b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector128<T> Max(Vector128<T> a, Vector128<T> b) => Vector128.Max(a, b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ulong Less(Vector128<T> a, Vector128<T> b) => Vector128.LessThan(a, b).ExtractMostSignificantBits();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ulong Greater(Vector128<T> a, Vector128<T> b) => Vector128.GreaterThan(a, b).ExtractMostSignificantBits();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ulong Equal(Vector128<T> a, Vector128<T> b) => Vector128.Equals(a, b).ExtractMostSignificantBits();
}

internal readonly struct Vector256Lanes<T> : IVectorLanes<Vector256<T>, T>
{
    public static int Count => Vector256<T>.Count;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector256<T> Load(ref T source) => Vector256.LoadUnsafe(ref source);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector256<T> Broadcast(T value) => Vector256.Create(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector256<T> Min(Vector256<T> a, Vector256<T> b) => Vector256.Min(a, b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector256<T> Max(Vector256<T> a, Vector256<T> b) => Vector256.Max(a, b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ulong Less(Vector256<T> a, Vector256<T> b) => Vector256.LessThan(a, b).ExtractMostSignificantBits();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ulong Greater(Vector256<T> a, Vector256<T> b) => Vector256.GreaterThan(a, b).ExtractMostSignificantBits();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ulong Equal(Vector256<T> a, Vector256<T> b) => Vector256.Equals(a, b).ExtractMostSignificantBits();
}

internal readonly struct Vector512Lanes<T> : IVectorLanes<Vector512<T>, T>
{
    public static int Count => Vector512<T>.Count;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector512<T> Load(ref T source) => Vector512.LoadUnsafe(ref source);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector512<T> Broadcast(T value) => Vector512.Create(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector512<T> Min(Vector512<T> a, Vector512<T> b) => Vector512.Min(a, b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector512<T> Max(Vector512<T> a, Vector512<T> b) => Vector512.Max(a, b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ulong Less(Vector512<T> a, Vector512<T> b) => Vector512.LessThan(a, b).ExtractMostSignificantBits();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ulong Greater(Vector512<T> a, Vector512<T> b) => Vector512.GreaterThan(a, b).ExtractMostSignificantBits();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ulong Equal(Vector512<T> a, Vector512<T> b) => Vector512.Equals(a, b).ExtractMostSignificantBits();
}

/// <summary>
/// Minimum and maximum searches for the SIMD selection sorts. Every element operation goes through <see cref="SortSpan{T, TComparer, TContext}"/>,
/// so an observed run executes the same scan as an unobserved one and reports each lane of each vector instruction.
/// </summary>
internal static class SelectionVectorScan
{
    /// <summary>
    /// Whether the scans can run on <typeparamref name="T"/>: a primitive integer type ordered by its natural order, whose vector comparisons
    /// agree with that order. Floating-point types are excluded because <c>CompareTo</c> orders NaN first and treats -0 and +0 as equal.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsVectorizable<T, TComparer>()
        => typeof(TComparer).IsValueType && default(TComparer) is IComparableComparer &&
            (typeof(T) == typeof(byte) || typeof(T) == typeof(sbyte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(short) ||
             typeof(T) == typeof(uint) || typeof(T) == typeof(int) || typeof(T) == typeof(ulong) || typeof(T) == typeof(long) ||
             typeof(T) == typeof(nuint) || typeof(T) == typeof(nint));

    /// <summary>
    /// The lanes per vector that the SIMD selection sorts use for <typeparamref name="T"/> on this machine, or 0 when no vector width is
    /// accelerated. It follows the same order of preference as their dispatch: 512, then 256, then 128 bits.
    /// </summary>
    internal static int LaneCount<T>()
        => Vector512.IsHardwareAccelerated ? Vector512<T>.Count
        : Vector256.IsHardwareAccelerated ? Vector256<T>.Count
        : Vector128.IsHardwareAccelerated ? Vector128<T>.Count
        : 0;

    /// <summary>
    /// Returns the index of the first minimum of [<paramref name="start"/>, <paramref name="end"/>), which holds at least one vector.
    /// One pass folds whole vectors into a vector of lane minima, which is then reduced to one value; the elements after the last whole
    /// vector are compared with it one by one. A second pass finds the first element equal to that value.
    /// </summary>
    public static int IndexOfFirstMinimum<T, TComparer, TContext, TLanes, TVec>(SortSpan<T, TComparer, TContext> s, int start, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        where TLanes : struct, IVectorLanes<TVec, T>
    {
        var w = TLanes.Count;
        Debug.Assert(end - start >= w);

        var lanes = TLanes.Load(ref s.ReadBlock(start, w));
        var i = start + w;
        for (; i <= end - w; i += w)
        {
            var v = TLanes.Load(ref s.ReadBlock(i, w));
            if (typeof(TContext) != typeof(NullContext)) s.ReportLaneCompares(i, w, TLanes.Less(v, lanes), TLanes.Greater(v, lanes));
            lanes = TLanes.Min(lanes, v);
        }

        var min = LaneMinimum<T, TComparer, TContext, TLanes, TVec>(s, lanes);
        for (; i < end; i++)
        {
            if (s.IsElementLessThan(i, min, out var element)) min = element;
        }

        return IndexOfFirst<T, TComparer, TContext, TLanes, TVec>(s, start, end, min);
    }

    /// <summary>
    /// Returns the minimum and maximum of [<paramref name="start"/>, <paramref name="end"/>), which holds at least two vectors.
    /// Elements are paired lane by lane across two adjacent vectors: each pair is compared once, then the smaller side is compared
    /// with the lane minima and the larger with the lane maxima, three comparisons per two elements. The elements after the last
    /// pair of whole vectors are paired with their neighbours in the same way, and an element left over is compared with both.
    /// </summary>
    public static (T Min, T Max) MinMax<T, TComparer, TContext, TLanes, TVec>(SortSpan<T, TComparer, TContext> s, int start, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        where TLanes : struct, IVectorLanes<TVec, T>
    {
        var w = TLanes.Count;
        Debug.Assert(end - start >= 2 * w);

        var a = TLanes.Load(ref s.ReadBlock(start, w));
        var b = TLanes.Load(ref s.ReadBlock(start + w, w));
        if (typeof(TContext) != typeof(NullContext)) s.ReportLaneCompares(start, start + w, w, TLanes.Less(a, b), TLanes.Greater(a, b));
        var minLanes = TLanes.Min(a, b);
        var maxLanes = TLanes.Max(a, b);

        var i = start + 2 * w;
        for (; i <= end - 2 * w; i += 2 * w)
        {
            a = TLanes.Load(ref s.ReadBlock(i, w));
            b = TLanes.Load(ref s.ReadBlock(i + w, w));
            if (typeof(TContext) != typeof(NullContext)) s.ReportLaneCompares(i, i + w, w, TLanes.Less(a, b), TLanes.Greater(a, b));
            var lo = TLanes.Min(a, b);
            var hi = TLanes.Max(a, b);
            if (typeof(TContext) != typeof(NullContext)) s.ReportHeldLaneCompares(w, TLanes.Less(lo, minLanes), TLanes.Greater(lo, minLanes));
            if (typeof(TContext) != typeof(NullContext)) s.ReportHeldLaneCompares(w, TLanes.Less(hi, maxLanes), TLanes.Greater(hi, maxLanes));
            minLanes = TLanes.Min(minLanes, lo);
            maxLanes = TLanes.Max(maxLanes, hi);
        }

        var min = LaneMinimum<T, TComparer, TContext, TLanes, TVec>(s, minLanes);
        var max = LaneMaximum<T, TComparer, TContext, TLanes, TVec>(s, maxLanes);
        for (; i + 1 < end; i += 2)
        {
            if (s.IsLessAt(i + 1, i, out var second, out var first))
            {
                if (s.IsLessThan(second, min)) min = second;
                if (s.IsLessThan(max, first)) max = first;
            }
            else
            {
                if (s.IsLessThan(first, min)) min = first;
                if (s.IsLessThan(max, second)) max = second;
            }
        }
        if (i < end)
        {
            var last = s.Read(i);
            if (s.IsLessThan(last, min)) min = last;
            if (s.IsLessThan(max, last)) max = last;
        }

        return (min, max);
    }

    /// <summary>
    /// Returns the index of the first element of [<paramref name="start"/>, <paramref name="end"/>) equal to <paramref name="value"/>,
    /// which the range must contain. Whole vectors are tested from the front, and the elements after the last of them one by one.
    /// </summary>
    public static int IndexOfFirst<T, TComparer, TContext, TLanes, TVec>(SortSpan<T, TComparer, TContext> s, int start, int end, T value)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        where TLanes : struct, IVectorLanes<TVec, T>
    {
        var w = TLanes.Count;
        var target = TLanes.Broadcast(value);
        var j = start;
        for (; j <= end - w; j += w)
        {
            var v = TLanes.Load(ref s.ReadBlock(j, w));
            if (typeof(TContext) != typeof(NullContext)) s.ReportLaneCompares(j, w, TLanes.Less(v, target), TLanes.Greater(v, target));
            var equal = TLanes.Equal(v, target);
            if (equal != 0) return j + BitOperations.TrailingZeroCount(equal);
        }
        for (; j < end; j++)
        {
            if (s.Compare(j, value) == 0) return j;
        }
        throw new UnreachableException("The searched value comes from the range.");
    }

    /// <summary>
    /// Returns the index of the last element of [<paramref name="start"/>, <paramref name="end"/>) equal to <paramref name="value"/>,
    /// which the range must contain. Whole vectors are tested from the back, and the elements before the first of them one by one.
    /// </summary>
    public static int IndexOfLast<T, TComparer, TContext, TLanes, TVec>(SortSpan<T, TComparer, TContext> s, int start, int end, T value)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        where TLanes : struct, IVectorLanes<TVec, T>
    {
        var w = TLanes.Count;
        var target = TLanes.Broadcast(value);
        var j = end - w;
        for (; j >= start; j -= w)
        {
            var v = TLanes.Load(ref s.ReadBlock(j, w));
            if (typeof(TContext) != typeof(NullContext)) s.ReportLaneCompares(j, w, TLanes.Less(v, target), TLanes.Greater(v, target));
            var equal = TLanes.Equal(v, target);
            if (equal != 0) return j + 63 - BitOperations.LeadingZeroCount(equal);
        }
        for (var k = j + w - 1; k >= start; k--)
        {
            if (s.Compare(k, value) == 0) return k;
        }
        throw new UnreachableException("The searched value comes from the range.");
    }

    /// <summary>Reduces the lanes to their minimum.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T LaneMinimum<T, TComparer, TContext, TLanes, TVec>(SortSpan<T, TComparer, TContext> s, TVec lanes)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        where TLanes : struct, IVectorLanes<TVec, T>
        => FoldLanes<T, TComparer, TContext>(s, FoldTo128<T, TComparer, TContext, TVec>(s, lanes, max: false), max: false);

    /// <summary>Reduces the lanes to their maximum.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T LaneMaximum<T, TComparer, TContext, TLanes, TVec>(SortSpan<T, TComparer, TContext> s, TVec lanes)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        where TLanes : struct, IVectorLanes<TVec, T>
        => FoldLanes<T, TComparer, TContext>(s, FoldTo128<T, TComparer, TContext, TVec>(s, lanes, max: true), max: true);

    /// <summary>
    /// Halves the lanes down to 128 bits, each step comparing the lower half with the upper half in one instruction and keeping the
    /// smaller (or larger) of each pair. A reduction of W lanes takes W - 1 comparisons of held values in total, these steps and
    /// <see cref="FoldLanes"/> together.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector128<T> FoldTo128<T, TComparer, TContext, TVec>(SortSpan<T, TComparer, TContext> s, TVec lanes, bool max)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        Vector256<T> v256;
        if (typeof(TVec) == typeof(Vector512<T>))
        {
            var v512 = Unsafe.As<TVec, Vector512<T>>(ref lanes);
            var (lo, hi) = (v512.GetLower(), v512.GetUpper());
            if (typeof(TContext) != typeof(NullContext))
                s.ReportHeldLaneCompares(Vector256<T>.Count, Vector256.LessThan(lo, hi).ExtractMostSignificantBits(), Vector256.GreaterThan(lo, hi).ExtractMostSignificantBits());
            v256 = max ? Vector256.Max(lo, hi) : Vector256.Min(lo, hi);
        }
        else if (typeof(TVec) == typeof(Vector256<T>))
        {
            v256 = Unsafe.As<TVec, Vector256<T>>(ref lanes);
        }
        else
        {
            return Unsafe.As<TVec, Vector128<T>>(ref lanes);
        }

        var (low, high) = (v256.GetLower(), v256.GetUpper());
        if (typeof(TContext) != typeof(NullContext))
            s.ReportHeldLaneCompares(Vector128<T>.Count, Vector128.LessThan(low, high).ExtractMostSignificantBits(), Vector128.GreaterThan(low, high).ExtractMostSignificantBits());
        return max ? Vector128.Max(low, high) : Vector128.Min(low, high);
    }

    /// <summary>Reduces the lanes of a 128-bit vector one lane at a time, a comparison of two held values per lane after the first.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T FoldLanes<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, Vector128<T> lanes, bool max)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        ref var lane = ref Unsafe.As<Vector128<T>, T>(ref lanes);
        var result = lane;
        for (var k = 1; k < Vector128<T>.Count; k++)
        {
            var x = Unsafe.Add(ref lane, k);
            if (max ? s.IsLessThan(result, x) : s.IsLessThan(x, result)) result = x;
        }
        return result;
    }
}
