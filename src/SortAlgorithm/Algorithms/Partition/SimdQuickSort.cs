using SortAlgorithm.Contexts;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// Bramas (2017) のベクトル化クイックソートです。3点の中央値をピボットとし、範囲の両端のベクトルを先にレジスタへ読み込んで書き込み先の空きを作り、
/// 以後は空きの少ない側から1ベクトルずつ読み込んで、ピボット以下のレーンを左の書き込み位置へ、より大きいレーンを右の書き込み位置へ compress-store で書き分ける
/// in-place 分割を行います。16ベクトル以下の範囲は、最後のベクトルを最大値で埋めて、レジスタ内の bitonic network で整列します。
/// <br/>
/// Bramas's vectorized quicksort (2017). The pivot is the median of three elements. Partitioning is in place: the first and last vectors of
/// the range are loaded into registers to free room at both ends, and every following step loads one vector from the side with less free
/// room, compares it with the pivot, and compress-stores the lanes not greater than the pivot at the left write position and the others at
/// the right one. A range of at most sixteen vectors is sorted by a bitonic network in registers, its last vector padded with the largest key.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct SIMD QuickSort:</strong></para>
/// <list type="number">
/// <item><description><strong>Median-of-3 Pivot:</strong> The pivot is the median of the elements at left, mid = left + (right - left) / 2 and
/// right. It is swapped to right, the range [left, right - 1] is partitioned around its value, and it is then swapped to the boundary, where it
/// is in its final place.</description></item>
/// <item><description><strong>Free Room at Both Ends:</strong> With W lanes per vector, the first and last vectors of a range of at least 2W
/// elements are loaded into registers first, which leaves W free slots before the first unread element and W after the last. Each step loads
/// W unread elements from the side with less free room, so when it stores, each side has at least W free slots and neither store can overwrite
/// an unread element.</description></item>
/// <item><description><strong>Compress-Store:</strong> A loaded vector is compared with the broadcast pivot. Its lanes not greater than the pivot
/// are stored packed, in lane order, at the left write position, which then advances by their count; the other lanes are stored packed ending at
/// the right write position, which moves back by theirs. Elements equal to the pivot go left.</description></item>
/// <item><description><strong>Remainder and Preloaded Vectors:</strong> When fewer than W elements are left unread, they are loaded as one vector
/// whose lanes past them are masked out, and then the two preloaded vectors are stored the same way. Every element of the range is written
/// exactly once, and the write positions meet at the boundary.</description></item>
/// <item><description><strong>Recursion:</strong> The ranges [left, boundary - 1] and [boundary + 1, right] are sorted the same way. A range of
/// at most 16W elements is sorted by the network instead; a range of one element is sorted.</description></item>
/// <item><description><strong>Bitonic Network:</strong> A range is loaded into k = ⌈m / W⌉ vectors, the lanes after its last element filled with
/// the largest value of the key type. One vector is sorted by the bitonic sort of its W lanes: for block sizes b = 2, 4, …, W, each lane is
/// compared with its mirror in the block (lane l with l ^ (b - 1)) and then with the lanes at distances b / 4, …, 1, the larger value always going
/// to the higher lane. Several vectors are sorted by sorting the first H (the largest power of two below k) and the other k - H, comparing vector
/// H - 1 - j with vector H + j reversed, and then cleaning each part: vectors at distances P / 2, …, 1 (P the part's size rounded up to a power of
/// two), then lanes at distances W / 2, …, 1 inside each vector. A comparator whose second vector would lie past k is skipped, because that vector
/// would hold only padding. The padding is never less than an element, so it sorts to the end and is not stored back.</description></item>
/// <item><description><strong>Comparison Consistency:</strong> The vector comparisons, minima and maxima order the elements as their natural order
/// does, which holds for the primitive 32- and 64-bit integer types the vector path runs on. Every other type, and every other comparer, runs the
/// same algorithm with one-lane vectors: one element per step, sixteen-element networks, and no padding.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Partitioning (Divide and Conquer)</description></item>
/// <item><description>Stable      : No (each step sends equal keys to the left in the order the two ends are read, and the network exchanges non-adjacent lanes)</description></item>
/// <item><description>In-place    : Yes (O(log n) recursion stack; registers hold the two preloaded vectors and the vectors of one network)</description></item>
/// <item><description>Best case   : Θ(n log n) - every median of three splits its range evenly</description></item>
/// <item><description>Average case: Θ(n log n) - each partition reads, compares and writes every element of its range once</description></item>
/// <item><description>Worst case  : O(n²) - keys equal to the pivot all go left, so a range of equal keys loses one element per partition; storing the preloaded vectors last rotates a sorted range, whose next median of three then samples its smallest keys, so sorted and reversed input degrade the same way</description></item>
/// <item><description>Comparisons : Θ(n log n) average, O(n²) worst - one per element of a partitioned range and two or three for its median; a network over k vectors of W lanes adds its comparators, padding lanes included</description></item>
/// <item><description>Swaps       : O(n) - two per partition: the pivot to the end of the range and from there to the boundary; nothing else swaps</description></item>
/// <item><description>Index Reads : Θ(n log n) average, O(n²) worst - every element of a partitioned range once, plus the samples, the pivot, and up to W - 1 lanes past the remainder; each network reads its range once</description></item>
/// <item><description>Index Writes: Θ(n log n) average, O(n²) worst - every element of a partitioned range once, plus the two swaps; each network writes its range once</description></item>
/// <item><description>Space       : O(log n) - the smaller side is sorted by recursion and the larger by the loop</description></item>
/// </list>
/// <para><strong>Use Cases:</strong></para>
/// <list type="bullet">
/// <item><description>Large arrays of unordered 32- or 64-bit integer keys on CPUs with AVX-512 or AVX2; presorted and duplicate-heavy keys degrade quadratically</description></item>
/// <item><description>Comparing vectorized partitioning and in-register sorting networks with scalar quicksorts</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <para>The vector path runs on <c>int</c>, <c>uint</c>, <c>long</c>, <c>ulong</c>, <c>nint</c> and <c>nuint</c> in their natural order. It uses
/// 512-bit vectors when AVX-512F is supported and the runtime accelerates them, and otherwise 256-bit vectors when AVX2 is supported. A
/// compress-store packs the selected lanes in a register, with the AVX-512 compress or, on AVX2 alone, a lane permutation from a table, and then
/// writes exactly those lanes with a masked store. The reference compresses straight to memory, which writes the same lanes but measured slower.
/// W is 16 for <c>int</c> with 512-bit vectors, so ranges of at most 256 elements go to the network, as in the reference
/// (<c>SortLimite = 16 * 64 / sizeof(T)</c>). Anywhere else the one-lane path runs.</para>
/// <para>The reference's pivot selection returns the right element for two of the six orders of the three samples (right ≤ mid ≤ left and
/// right ≤ left ≤ mid), which is then the smallest of them; this class takes their median, as the paper states. The reference recurses into the
/// right range and then the left; here the smaller range is sorted by recursion first and the larger one by the loop. When the loop leaves no
/// unread element, the remainder vector, which would store nothing, is not loaded.</para>
/// <para>An observed sort runs the same code. A vector load is reported as W reads and its comparison with the pivot as W comparisons, in index
/// order; a compress-store as one write per stored lane. The preloaded vectors and the network operate on values held in registers, so their
/// comparisons are reported as -1: W / 2 per step inside a vector and W per step between two vectors. The network's vectors are kept in a stack
/// array between steps, as the reference keeps them in registers, and that array is not reported as a buffer. The counts depend on W, which
/// depends on the machine.</para>
/// <para><strong>Reference:</strong></para>
/// <para>Paper: Berenger Bramas, "A Novel Hybrid Quicksort Algorithm Vectorized using AVX-512 on Intel Skylake", International Journal of
/// Advanced Computer Science and Applications 8(10), 2017. https://arxiv.org/abs/1704.08579</para>
/// <para>Code (MIT): https://github.com/berenger-eu/avx-512-sort (sort512.hpp)</para>
/// </remarks>
public static class SimdQuickSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by SimdQuickSortTests, which derives from SortTestsBase.</remarks>
    public static bool IsStable => false;

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array

    // A range of at most this many vectors is sorted by the network: the reference's SortLimite is sixteen 512-bit vectors.
    private const int SMALL_SORT_VECTORS = 16;

    /// <summary>
    /// Sorts the elements in the specified span in ascending order using the default comparer.
    /// Uses NullContext for zero-overhead fast path.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort in place.</param>
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
        => Sort(span, 0, span.Length, new ComparableComparer<T>(), NullContext.Default);

    /// <summary>
    /// Sorts the elements in the specified span using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <typeparam name="TContext">The type of the sort context.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T, TContext>(Span<T> span, TContext context)
        where T : IComparable<T>
        where TContext : ISortContext
        => Sort(span, 0, span.Length, new ComparableComparer<T>(), context);

    /// <summary>
    /// Sorts the elements in the specified span using the provided comparer and sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of the comparer</typeparam>
    /// <typeparam name="TContext">The type of the sort context.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="comparer">The comparer to use for element comparisons.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        => Sort(span, 0, span.Length, comparer, context);

    /// <summary>
    /// Sorts the subrange [first..last) using the provided comparer and sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <typeparam name="TContext">The type of the sort context.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="first">The inclusive start index of the range to sort.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T, TContext>(Span<T> span, int first, int last, TContext context)
        where T : IComparable<T>
        where TContext : ISortContext
        => Sort(span, first, last, new ComparableComparer<T>(), context);

    /// <summary>
    /// Sorts the subrange [first..last) using the provided comparer and sort context.
    /// This is the full-control version with explicit TContext type parameter.
    /// </summary>
    public static void Sort<T, TComparer, TContext>(Span<T> span, int first, int last, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        ArgumentOutOfRangeException.ThrowIfNegative(first);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(last, span.Length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(first, last);

        if (last - first <= 1) return;

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        SortCore(s, first, last);
    }

    /// <summary>
    /// Sorts the subrange [first..last) using the provided sort context.
    /// This overload accepts a SortSpan directly for use by other algorithms that already have a SortSpan instance.
    /// </summary>
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (last - first <= 1) return;

        if (IsVectorizable<T, TComparer>())
        {
            if (SimdQuickSortLanes512<T>.IsSupported)
            {
                SortRange<T, TComparer, TContext, SimdQuickSortLanes512<T>, Vector512<T>>(s, first, last - 1);
                return;
            }
            if (SimdQuickSortLanes256<T>.IsSupported)
            {
                SortRange<T, TComparer, TContext, SimdQuickSortLanes256<T>, Vector256<T>>(s, first, last - 1);
                return;
            }
        }

        SortRangeScalar(s, first, last - 1);
    }

    /// <summary>
    /// Whether the vector path can run on <typeparamref name="T"/>: a primitive 32- or 64-bit integer type ordered by its natural order.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsVectorizable<T, TComparer>()
        => typeof(TComparer).IsValueType && default(TComparer) is IComparableComparer &&
            (typeof(T) == typeof(int) || typeof(T) == typeof(uint) || typeof(T) == typeof(long) || typeof(T) == typeof(ulong) ||
             typeof(T) == typeof(nint) || typeof(T) == typeof(nuint));

    /// <summary>
    /// The lanes per vector this algorithm uses for <typeparamref name="T"/> and <typeparamref name="TComparer"/> on this machine,
    /// 1 on the one-lane path.
    /// </summary>
    internal static int LaneCount<T, TComparer>()
        => !IsVectorizable<T, TComparer>() ? 1
        : SimdQuickSortLanes512<T>.IsSupported ? Vector512<T>.Count
        : SimdQuickSortLanes256<T>.IsSupported ? Vector256<T>.Count
        : 1;

    // ---------------------------------------------------------------------------------------------------------------------------------
    // Vector path
    // ---------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Sorts [<paramref name="left"/>, <paramref name="right"/>] (both inclusive).
    /// </summary>
    static void SortRange<T, TComparer, TContext, TLanes, TVec>(SortSpan<T, TComparer, TContext> s, int left, int right)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        where TLanes : struct, ISimdQuickSortLanes<TVec, T>
    {
        var limit = SMALL_SORT_VECTORS * TLanes.Count;
        while (right - left >= limit)
        {
            var part = PivotPartition<T, TComparer, TContext, TLanes, TVec>(s, left, right);
            if (part - left < right - part)
            {
                if (left < part - 1) SortRange<T, TComparer, TContext, TLanes, TVec>(s, left, part - 1);
                left = part + 1;
            }
            else
            {
                if (part + 1 < right) SortRange<T, TComparer, TContext, TLanes, TVec>(s, part + 1, right);
                right = part - 1;
            }
        }

        if (left < right) SmallSort<T, TComparer, TContext, TLanes, TVec>(s, left, right - left + 1);
    }

    /// <summary>
    /// Moves the median of three to <paramref name="right"/>, partitions the rest of the range around it, and puts it at the boundary,
    /// whose index is returned.
    /// </summary>
    static int PivotPartition<T, TComparer, TContext, TLanes, TVec>(SortSpan<T, TComparer, TContext> s, int left, int right)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        where TLanes : struct, ISimdQuickSortLanes<TVec, T>
    {
        var pivotIndex = MedianOfThree(s, left, left + (right - left) / 2, right);
        s.Swap(pivotIndex, right);
        s.Context.OnPhase(SortPhase.QuickSortPartition, s.Offset + left, s.Offset + right, s.Offset + right);
        s.Context.OnRole(s.Offset + right, s.BufferId, RoleType.Pivot);

        var part = Partition<T, TComparer, TContext, TLanes, TVec>(s, left, right - 1, s.Read(right));

        s.Context.OnRole(s.Offset + right, s.BufferId, RoleType.None);
        s.Swap(part, right);
        return part;
    }

    /// <summary>
    /// Partitions [<paramref name="left"/>, <paramref name="right"/>], which holds at least two vectors, so that the elements not greater
    /// than <paramref name="pivot"/> come first, and returns the index of the first greater element.
    /// </summary>
    internal static unsafe int Partition<T, TComparer, TContext, TLanes, TVec>(SortSpan<T, TComparer, TContext> s, int left, int right, T pivot)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        where TLanes : struct, ISimdQuickSortLanes<TVec, T>
    {
        var w = TLanes.Count;
        Debug.Assert(right - left + 1 >= 2 * w);

        var pivotVector = TLanes.Broadcast(pivot);
        fixed (byte* bytes = &Unsafe.As<T, byte>(ref s.BlockRef(0)))
        {
            var leftVector = TLanes.Load(ref s.ReadBlock(left, w));
            var leftWrite = left;
            left += w;

            var rightWrite = right + 1;
            right -= w - 1;
            var rightVector = TLanes.Load(ref s.ReadBlock(right, w));

            while (left + w <= right)
            {
                int at;
                if (left - leftWrite <= rightWrite - right)
                {
                    at = left;
                    left += w;
                }
                else
                {
                    right -= w;
                    at = right;
                }

                var v = TLanes.Load(ref s.ReadBlock(at, w));
                if (typeof(TContext) != typeof(NullContext)) s.ReportLaneCompares(at, w, TLanes.LessBits(v, pivotVector), TLanes.GreaterBits(v, pivotVector));
                var low = TLanes.LessOrEqual(v, pivotVector);
                var lowCount = TLanes.PopCount(low);
                StoreSides<T, TComparer, TContext, TLanes, TVec>(s, bytes, ref leftWrite, ref rightWrite, v, low, lowCount, TLanes.Greater(v, pivotVector), w - lowCount);
            }

            var remaining = right - left;
            if (remaining > 0)
            {
                var v = TLanes.Load(ref s.ReadBlock(left, w));
                if (typeof(TContext) != typeof(NullContext)) s.ReportLaneCompares(left, w, TLanes.LessBits(v, pivotVector), TLanes.GreaterBits(v, pivotVector));
                var unread = TLanes.FirstLanes(remaining);
                var low = TLanes.And(TLanes.LessOrEqual(v, pivotVector), unread);
                var high = TLanes.And(TLanes.Greater(v, pivotVector), unread);
                StoreSides<T, TComparer, TContext, TLanes, TVec>(s, bytes, ref leftWrite, ref rightWrite, v, low, TLanes.PopCount(low), high, TLanes.PopCount(high));
            }

            StoreHeld<T, TComparer, TContext, TLanes, TVec>(s, bytes, ref leftWrite, ref rightWrite, leftVector, pivotVector);
            StoreHeld<T, TComparer, TContext, TLanes, TVec>(s, bytes, ref leftWrite, ref rightWrite, rightVector, pivotVector);
            Debug.Assert(leftWrite == rightWrite);
            return leftWrite;
        }
    }

    /// <summary>
    /// Compares a preloaded vector, whose lanes are held in registers, with the pivot and stores it on both sides.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static unsafe void StoreHeld<T, TComparer, TContext, TLanes, TVec>(SortSpan<T, TComparer, TContext> s, byte* bytes, ref int leftWrite, ref int rightWrite, TVec v, TVec pivotVector)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        where TLanes : struct, ISimdQuickSortLanes<TVec, T>
    {
        if (typeof(TContext) != typeof(NullContext)) s.ReportHeldLaneCompares(TLanes.Count, TLanes.LessBits(v, pivotVector), TLanes.GreaterBits(v, pivotVector));
        var low = TLanes.LessOrEqual(v, pivotVector);
        var lowCount = TLanes.PopCount(low);
        StoreSides<T, TComparer, TContext, TLanes, TVec>(s, bytes, ref leftWrite, ref rightWrite, v, low, lowCount, TLanes.Greater(v, pivotVector), TLanes.Count - lowCount);
    }

    /// <summary>
    /// Compress-stores the lanes of <paramref name="v"/> selected by <paramref name="low"/> at the left write position and those selected by
    /// <paramref name="high"/> ending at the right write position, and moves both positions past them.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static unsafe void StoreSides<T, TComparer, TContext, TLanes, TVec>(SortSpan<T, TComparer, TContext> s, byte* bytes, ref int leftWrite, ref int rightWrite,
        TVec v, TVec low, int lowCount, TVec high, int highCount)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        where TLanes : struct, ISimdQuickSortLanes<TVec, T>
    {
        TLanes.CompressStore(bytes + (nint)leftWrite * Unsafe.SizeOf<T>(), low, v, lowCount);
        s.ReportWrittenBlock(leftWrite, lowCount);
        leftWrite += lowCount;

        rightWrite -= highCount;
        TLanes.CompressStore(bytes + (nint)rightWrite * Unsafe.SizeOf<T>(), high, v, highCount);
        s.ReportWrittenBlock(rightWrite, highCount);
    }

    /// <summary>
    /// Sorts [<paramref name="start"/>, <paramref name="start"/> + <paramref name="length"/>), at most sixteen vectors, with the network.
    /// </summary>
    static void SmallSort<T, TComparer, TContext, TLanes, TVec>(SortSpan<T, TComparer, TContext> s, int start, int length)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        where TLanes : struct, ISimdQuickSortLanes<TVec, T>
    {
        var w = TLanes.Count;
        Debug.Assert(length >= 2 && length <= SMALL_SORT_VECTORS * w);
        s.Context.OnPhase(SortPhase.SimdQuickSortBitonicSort, s.Offset + start, s.Offset + start + length - 1, w);

        var full = length / w;
        var tail = length - full * w;
        VectorFile<TVec> file = default;
        ref var v = ref Unsafe.As<VectorFile<TVec>, TVec>(ref file);

        for (var i = 0; i < full; i++)
        {
            Unsafe.Add(ref v, i) = TLanes.Load(ref s.ReadBlock(start + i * w, w));
        }
        if (tail > 0)
        {
            var padded = TLanes.Padding;
            MemoryMarshal.CreateReadOnlySpan(ref s.ReadBlock(start + full * w, tail), tail)
                .CopyTo(MemoryMarshal.CreateSpan(ref Unsafe.As<TVec, T>(ref padded), tail));
            Unsafe.Add(ref v, full) = padded;
        }

        Network<T, TComparer, TContext, TLanes, TVec>(s, ref v, full + (tail > 0 ? 1 : 0));

        for (var i = 0; i < full; i++)
        {
            TLanes.Store(ref s.BlockRef(start + i * w), Unsafe.Add(ref v, i));
            s.ReportWrittenBlock(start + i * w, w);
        }
        if (tail > 0)
        {
            MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<TVec, T>(ref Unsafe.Add(ref v, full)), tail)
                .CopyTo(MemoryMarshal.CreateSpan(ref s.BlockRef(start + full * w), tail));
            s.ReportWrittenBlock(start + full * w, tail);
        }
    }

    /// <summary>
    /// Sorts the <paramref name="count"/> vectors from <paramref name="v"/> as one sequence: the first H (the largest power of two below
    /// <paramref name="count"/>) and the rest are sorted, compared across the middle with the second part reversed, and then cleaned.
    /// </summary>
    static void Network<T, TComparer, TContext, TLanes, TVec>(SortSpan<T, TComparer, TContext> s, ref TVec v, int count)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        where TLanes : struct, ISimdQuickSortLanes<TVec, T>
    {
        if (count == 1)
        {
            v = SortVector<T, TComparer, TContext, TLanes, TVec>(s, v);
            return;
        }

        var half = 1 << BitOperations.Log2((uint)(count - 1));
        Network<T, TComparer, TContext, TLanes, TVec>(s, ref v, half);
        Network<T, TComparer, TContext, TLanes, TVec>(s, ref Unsafe.Add(ref v, half), count - half);

        for (var j = 0; j < count - half; j++)
        {
            ref var a = ref Unsafe.Add(ref v, half - 1 - j);
            ref var b = ref Unsafe.Add(ref v, half + j);
            var reversed = TLanes.Reverse(b);
            if (typeof(TContext) != typeof(NullContext)) s.ReportHeldLaneCompares(TLanes.Count, TLanes.LessBits(a, reversed), TLanes.GreaterBits(a, reversed));
            b = TLanes.Max(a, reversed);
            a = TLanes.Min(a, reversed);
        }

        Clean<T, TComparer, TContext, TLanes, TVec>(s, ref v, half);
        Clean<T, TComparer, TContext, TLanes, TVec>(s, ref Unsafe.Add(ref v, half), count - half);
    }

    /// <summary>
    /// Cleans <paramref name="count"/> vectors whose sequence is bitonic: vectors at distances P / 2, …, 1 (P the count rounded up to a power
    /// of two, skipping vectors past the count), then the lanes inside each vector.
    /// </summary>
    static void Clean<T, TComparer, TContext, TLanes, TVec>(SortSpan<T, TComparer, TContext> s, ref TVec v, int count)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        where TLanes : struct, ISimdQuickSortLanes<TVec, T>
    {
        for (var d = (int)BitOperations.RoundUpToPowerOf2((uint)count) / 2; d >= 1; d /= 2)
        {
            for (var i = 0; i + d < count; i++)
            {
                if ((i & d) != 0) continue;
                ref var a = ref Unsafe.Add(ref v, i);
                ref var b = ref Unsafe.Add(ref v, i + d);
                if (typeof(TContext) != typeof(NullContext)) s.ReportHeldLaneCompares(TLanes.Count, TLanes.LessBits(a, b), TLanes.GreaterBits(a, b));
                var min = TLanes.Min(a, b);
                b = TLanes.Max(a, b);
                a = min;
            }
        }

        for (var i = 0; i < count; i++)
        {
            ref var a = ref Unsafe.Add(ref v, i);
            a = CleanVector<T, TComparer, TContext, TLanes, TVec>(s, a);
        }
    }

    /// <summary>
    /// The bitonic sort of the lanes of one vector: for block sizes 2, 4, …, W, a step with the mirror lane in the block and then steps at
    /// distances b / 4, …, 1.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static TVec SortVector<T, TComparer, TContext, TLanes, TVec>(SortSpan<T, TComparer, TContext> s, TVec v)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        where TLanes : struct, ISimdQuickSortLanes<TVec, T>
    {
        v = Step<T, TComparer, TContext, TLanes, TVec>(s, v, 1, 1);
        if (TLanes.Count >= 4)
        {
            v = Step<T, TComparer, TContext, TLanes, TVec>(s, v, 3, 2);
            v = Step<T, TComparer, TContext, TLanes, TVec>(s, v, 1, 1);
        }
        if (TLanes.Count >= 8)
        {
            v = Step<T, TComparer, TContext, TLanes, TVec>(s, v, 7, 4);
            v = Step<T, TComparer, TContext, TLanes, TVec>(s, v, 2, 2);
            v = Step<T, TComparer, TContext, TLanes, TVec>(s, v, 1, 1);
        }
        if (TLanes.Count >= 16)
        {
            v = Step<T, TComparer, TContext, TLanes, TVec>(s, v, 15, 8);
            v = Step<T, TComparer, TContext, TLanes, TVec>(s, v, 4, 4);
            v = Step<T, TComparer, TContext, TLanes, TVec>(s, v, 2, 2);
            v = Step<T, TComparer, TContext, TLanes, TVec>(s, v, 1, 1);
        }
        return v;
    }

    /// <summary>
    /// Cleans the lanes of one vector whose lanes form a bitonic sequence: steps at distances W / 2, …, 1.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static TVec CleanVector<T, TComparer, TContext, TLanes, TVec>(SortSpan<T, TComparer, TContext> s, TVec v)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        where TLanes : struct, ISimdQuickSortLanes<TVec, T>
    {
        if (TLanes.Count >= 16) v = Step<T, TComparer, TContext, TLanes, TVec>(s, v, 8, 8);
        if (TLanes.Count >= 8) v = Step<T, TComparer, TContext, TLanes, TVec>(s, v, 4, 4);
        if (TLanes.Count >= 4) v = Step<T, TComparer, TContext, TLanes, TVec>(s, v, 2, 2);
        return Step<T, TComparer, TContext, TLanes, TVec>(s, v, 1, 1);
    }

    /// <summary>
    /// Compares each lane l with lane l ^ <paramref name="partner"/> and leaves the larger value in the lane whose <paramref name="bit"/> is set.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static TVec Step<T, TComparer, TContext, TLanes, TVec>(SortSpan<T, TComparer, TContext> s, TVec v, int partner, int bit)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        where TLanes : struct, ISimdQuickSortLanes<TVec, T>
    {
        var p = TLanes.Partner(v, partner);
        if (typeof(TContext) != typeof(NullContext)) s.ReportSelectedHeldLaneCompares(TLanes.LanesWithout(bit), TLanes.LessBits(v, p), TLanes.GreaterBits(v, p));
        return TLanes.Blend(TLanes.Min(v, p), TLanes.Max(v, p), bit);
    }

    // ---------------------------------------------------------------------------------------------------------------------------------
    // One-lane path
    // ---------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Sorts [<paramref name="left"/>, <paramref name="right"/>] (both inclusive) with one-lane vectors.
    /// </summary>
    static void SortRangeScalar<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int right)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (right - left >= SMALL_SORT_VECTORS)
        {
            var pivotIndex = MedianOfThree(s, left, left + (right - left) / 2, right);
            s.Swap(pivotIndex, right);
            s.Context.OnPhase(SortPhase.QuickSortPartition, s.Offset + left, s.Offset + right, s.Offset + right);
            s.Context.OnRole(s.Offset + right, s.BufferId, RoleType.Pivot);

            var part = PartitionScalar(s, left, right - 1, s.Read(right));

            s.Context.OnRole(s.Offset + right, s.BufferId, RoleType.None);
            s.Swap(part, right);

            if (part - left < right - part)
            {
                if (left < part - 1) SortRangeScalar(s, left, part - 1);
                left = part + 1;
            }
            else
            {
                if (part + 1 < right) SortRangeScalar(s, part + 1, right);
                right = part - 1;
            }
        }

        if (left < right) SmallSortScalar(s, left, right - left + 1);
    }

    /// <summary>
    /// The partition with one-lane vectors: the first and last elements are held, then each step reads the next element from the side with
    /// less free room and writes it at the left or the right write position.
    /// </summary>
    static int PartitionScalar<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int right, T pivot)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        Debug.Assert(right - left + 1 >= 2);

        var leftValue = s.Read(left);
        var leftWrite = left;
        left++;
        var rightWrite = right + 1;
        var rightValue = s.Read(right);

        while (left < right)
        {
            int at;
            if (left - leftWrite <= rightWrite - right)
            {
                at = left;
                left++;
            }
            else
            {
                right--;
                at = right;
            }

            if (s.IsElementLessOrEqual(at, pivot, out var value)) s.Write(leftWrite++, value);
            else s.Write(--rightWrite, value);
        }

        if (s.IsLessOrEqual(leftValue, pivot)) s.Write(leftWrite++, leftValue);
        else s.Write(--rightWrite, leftValue);
        if (s.IsLessOrEqual(rightValue, pivot)) s.Write(leftWrite++, rightValue);
        else s.Write(--rightWrite, rightValue);

        Debug.Assert(leftWrite == rightWrite);
        return leftWrite;
    }

    /// <summary>
    /// Sorts at most sixteen elements with the network of one-lane vectors, which keeps only the comparators between vectors.
    /// </summary>
    static void SmallSortScalar<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int start, int length)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        Debug.Assert(length >= 2 && length <= SMALL_SORT_VECTORS);
        s.Context.OnPhase(SortPhase.SimdQuickSortBitonicSort, s.Offset + start, s.Offset + start + length - 1, 1);

        ElementFile<T> file = default;
        ref var v = ref Unsafe.As<ElementFile<T>, T>(ref file);
        for (var i = 0; i < length; i++)
        {
            Unsafe.Add(ref v, i) = s.Read(start + i);
        }

        NetworkScalar(s, ref v, length);

        for (var i = 0; i < length; i++)
        {
            s.Write(start + i, Unsafe.Add(ref v, i));
        }
    }

    static void NetworkScalar<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, ref T v, int count)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (count == 1) return;

        var half = 1 << BitOperations.Log2((uint)(count - 1));
        NetworkScalar(s, ref v, half);
        NetworkScalar(s, ref Unsafe.Add(ref v, half), count - half);

        for (var j = 0; j < count - half; j++)
        {
            Exchange(s, ref Unsafe.Add(ref v, half - 1 - j), ref Unsafe.Add(ref v, half + j));
        }

        CleanScalar(s, ref v, half);
        CleanScalar(s, ref Unsafe.Add(ref v, half), count - half);
    }

    static void CleanScalar<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, ref T v, int count)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        for (var d = (int)BitOperations.RoundUpToPowerOf2((uint)count) / 2; d >= 1; d /= 2)
        {
            for (var i = 0; i + d < count; i++)
            {
                if ((i & d) == 0) Exchange(s, ref Unsafe.Add(ref v, i), ref Unsafe.Add(ref v, i + d));
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Exchange<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, ref T a, ref T b)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (s.IsLessThan(b, a)) (a, b) = (b, a);
    }

    // ---------------------------------------------------------------------------------------------------------------------------------
    // Shared
    // ---------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Returns the index of the median of the elements at <paramref name="a"/>, <paramref name="b"/> and <paramref name="c"/>.
    /// </summary>
    static int MedianOfThree<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int a, int b, int c)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (s.IsLessOrEqualAt(a, b))
        {
            if (s.IsLessOrEqualAt(b, c)) return b;
            return s.IsLessOrEqualAt(a, c) ? c : a;
        }
        if (s.IsLessOrEqualAt(a, c)) return a;
        return s.IsLessOrEqualAt(b, c) ? c : b;
    }

    /// <summary>The vectors of one network, held in registers by the reference.</summary>
    [InlineArray(SMALL_SORT_VECTORS)]
    struct VectorFile<TVec>
    {
        TVec _element;
    }

    /// <summary>The elements of one network on the one-lane path.</summary>
    [InlineArray(SMALL_SORT_VECTORS)]
    struct ElementFile<T>
    {
        T _element;
    }
}
