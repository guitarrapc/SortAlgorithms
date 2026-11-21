using SortAlgorithm.Contexts;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 任意長のバイトニックソート（Lang, 1998）を SIMD 命令で実行する版です。<see cref="BitonicSortAnyLength"/> と同じ比較器網を同じ段の順に実行しますが、
/// 各比較器は最小値と最大値を常に両方の位置へ書き込み、W レーン分の比較器を1命令でまとめて行います。距離が W 以上の比較器の並びは2本のベクトルの
/// min/max で、ちょうど W 要素の整列とマージはレジスタ内のレーン並べ替えで、それ以外は1つずつ実行します。
/// <br/>
/// Bitonic sort for any input length (Lang, 1998) on SIMD instructions. It runs the network of <see cref="BitonicSortAnyLength"/>, stage by
/// stage in the same order, but every comparator writes its minimum and its maximum to its two positions, and W comparators run as one
/// instruction where the network allows it: a run of comparators at a distance of at least W as the minimum and maximum of two vectors, a
/// sort or merge of exactly W elements with lane permutations in a register, and every other comparator one at a time.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct SIMD Bitonic Sort of Any Length:</strong></para>
/// <list type="number">
/// <item><description><strong>Same Network:</strong> The recursion, the comparators and their directions are those of
/// <see cref="BitonicSortAnyLength"/>: the first ⌊n/2⌋ elements sorted opposite to the range and the rest in its direction, then a merge that
/// compares i with i + m for i &lt; n - m (m the largest power of two below n) and merges [0, m) and [m, n). Correctness and the comparison
/// count therefore carry over.</description></item>
/// <item><description><strong>Comparator as a Module:</strong> Each comparator delivers the minimum to one position and the maximum to the other,
/// whether or not they were in order, as in <see cref="BitonicSortBranchless"/>. Both forms leave the same values in the same positions.</description></item>
/// <item><description><strong>Comparator Runs:</strong> The comparators (i, i + m) of one merge have one direction and do not share a position
/// when m ≥ W, so W consecutive ones are the lane-wise minimum and maximum of the vectors at i and i + m.</description></item>
/// <item><description><strong>Blocks of W Elements:</strong> A sort or merge of exactly W elements is loaded into one vector. Its comparators of
/// one stage pair lane l with lane l ^ d, so a stage is a lane permutation, a minimum, a maximum and a blend that puts the larger value in the
/// upper lane of an ascending block and in the lower lane of a descending one. In a sort, a block's direction follows from Lang's recursion:
/// it is descending when the path to it takes the first half an odd number of times, counted from the direction of the whole vector.
/// Comparators of one stage touch distinct lanes, and comparators in different blocks are independent, so running a stage across the whole
/// vector at once leaves what the recursion leaves.</description></item>
/// <item><description><strong>Comparison Consistency:</strong> The vector minima and maxima order the elements as their natural order does,
/// which holds for the primitive 32- and 64-bit integer types the vector path runs on. Every other type, and every other comparer, runs the
/// network one comparator at a time.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Network Sort / Exchange</description></item>
/// <item><description>Stable      : No (comparators exchange non-adjacent elements)</description></item>
/// <item><description>In-place    : Yes (the comparators act on the input; O(log n) recursion stack)</description></item>
/// <item><description>Best case   : Θ(n log² n) - the network runs the same comparators on every input</description></item>
/// <item><description>Average case: Θ(n log² n) - the same comparators as the best case</description></item>
/// <item><description>Worst case  : Θ(n log² n) - the same comparators as the best case</description></item>
/// <item><description>Comparisons : Θ(n log² n) - one per comparator, exactly as many as <see cref="BitonicSortAnyLength"/></description></item>
/// <item><description>Swaps       : 0 - every comparator writes both positions instead of exchanging them</description></item>
/// <item><description>Index Reads : Θ(n log² n) - two per comparator outside a block of W elements; a block reads its W elements once for all its stages</description></item>
/// <item><description>Index Writes: Θ(n log² n) - two per comparator outside a block of W elements; a block writes its W elements once after its stages</description></item>
/// <item><description>Space       : O(log n) - the recursion stack</description></item>
/// <item><description>Depth       : ⌈log₂ n⌉ (⌈log₂ n⌉ + 1) / 2 stages, as <see cref="BitonicSortAnyLength"/></description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <para>The vector path runs on <c>int</c>, <c>uint</c>, <c>long</c>, <c>ulong</c>, <c>nint</c> and <c>nuint</c> in their natural order, with
/// the widest of 512-, 256- and 128-bit vectors that the runtime accelerates, using only the cross-platform vector API. Everything else runs the
/// comparators one at a time through the comparator of <see cref="BitonicSortBranchless"/>.</para>
/// <para>An observed sort runs the same code. A comparator run reports, per vector pair, W reads at each position, W comparisons in index
/// order, and W writes at each position, the same as W comparators run one at a time. A block reports W reads, then W / 2 comparisons per stage
/// between values held in the register, reported as -1, then W writes. The counts of reads and writes therefore depend on W, which depends on
/// the machine; the comparisons do not.</para>
/// <para><strong>Reference:</strong></para>
/// <para>H. W. Lang, "Bitonic sorting network for n not a power of 2", 1998: https://hwlang.de/algorithmen/sortieren/bitonic/oddn.htm</para>
/// </remarks>
public static class BitonicSortAnyLengthSimd
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by BitonicSortAnyLengthSimdTests, which derives from SortTestsBase.</remarks>
    public static bool IsStable => false;

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array

    /// <summary>
    /// Sorts the elements in the specified span in ascending order using the default comparer.
    /// Uses NullContext for zero-overhead fast path.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort in place.</param>
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
        => Sort(span, new ComparableComparer<T>(), NullContext.Default);

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
        => Sort(span, new ComparableComparer<T>(), context);

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
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        SortCore(s, 0, span.Length);
    }

    /// <summary>
    /// Sorts the <paramref name="count"/> elements from <paramref name="low"/> ascending.
    /// </summary>
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int low, int count)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (count <= 1) return;

        if (IsVectorizable<T, TComparer>())
        {
            if (BitonicLanes512<T>.IsSupported)
            {
                SortVectors<T, TComparer, TContext, BitonicLanes512<T>, Vector512<T>>(s, low, count, ascending: true);
                return;
            }
            if (BitonicLanes256<T>.IsSupported)
            {
                SortVectors<T, TComparer, TContext, BitonicLanes256<T>, Vector256<T>>(s, low, count, ascending: true);
                return;
            }
            if (BitonicLanes128<T>.IsSupported)
            {
                SortVectors<T, TComparer, TContext, BitonicLanes128<T>, Vector128<T>>(s, low, count, ascending: true);
                return;
            }
        }

        SortModules(s, low, count, ascending: true);
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
    /// 1 when the comparators run one at a time.
    /// </summary>
    internal static int LaneCount<T, TComparer>()
        => !IsVectorizable<T, TComparer>() ? 1
        : BitonicLanes512<T>.IsSupported ? Vector512<T>.Count
        : BitonicLanes256<T>.IsSupported ? Vector256<T>.Count
        : BitonicLanes128<T>.IsSupported ? Vector128<T>.Count
        : 1;

    // ---------------------------------------------------------------------------------------------------------------------------------
    // Vector path
    // ---------------------------------------------------------------------------------------------------------------------------------

    static void SortVectors<T, TComparer, TContext, TLanes, TVec>(SortSpan<T, TComparer, TContext> s, int low, int count, bool ascending)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        where TLanes : struct, IBitonicLanes<TVec, T>
    {
        if (count <= 1) return;
        if (count == TLanes.Count)
        {
            SortBlock<T, TComparer, TContext, TLanes, TVec>(s, low, ascending);
            return;
        }

        var half = count / 2;
        if (half > 1) SortVectors<T, TComparer, TContext, TLanes, TVec>(s, low, half, !ascending);
        SortVectors<T, TComparer, TContext, TLanes, TVec>(s, low + half, count - half, ascending);
        MergeVectors<T, TComparer, TContext, TLanes, TVec>(s, low, count, ascending);
    }

    static void MergeVectors<T, TComparer, TContext, TLanes, TVec>(SortSpan<T, TComparer, TContext> s, int low, int count, bool ascending)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        where TLanes : struct, IBitonicLanes<TVec, T>
    {
        if (count <= 1) return;
        if (count == TLanes.Count)
        {
            MergeBlock<T, TComparer, TContext, TLanes, TVec>(s, low, ascending);
            return;
        }

        var m = BitonicSortAnyLength.GreatestPowerOfTwoLessThan(count);
        var end = low + count - m;
        var i = low;
        if (m >= TLanes.Count)
        {
            for (; i + TLanes.Count <= end; i += TLanes.Count)
            {
                CompareVectors<T, TComparer, TContext, TLanes, TVec>(s, i, i + m, ascending);
            }
        }
        for (; i < end; i++)
        {
            Module(s, i, i + m, ascending);
        }

        if (m > 1) MergeVectors<T, TComparer, TContext, TLanes, TVec>(s, low, m, ascending);
        if (count - m > 1) MergeVectors<T, TComparer, TContext, TLanes, TVec>(s, low + m, count - m, ascending);
    }

    /// <summary>
    /// The W comparators (lower + k, upper + k), as the minimum and maximum of the two vectors.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void CompareVectors<T, TComparer, TContext, TLanes, TVec>(SortSpan<T, TComparer, TContext> s, int lower, int upper, bool ascending)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        where TLanes : struct, IBitonicLanes<TVec, T>
    {
        var w = TLanes.Count;
        var a = TLanes.Load(ref s.ReadBlock(lower, w));
        var b = TLanes.Load(ref s.ReadBlock(upper, w));
        if (typeof(TContext) != typeof(NullContext)) s.ReportLaneCompares(lower, upper, w, TLanes.LessBits(a, b), TLanes.GreaterBits(a, b));
        var min = TLanes.Min(a, b);
        var max = TLanes.Max(a, b);
        TLanes.Store(ref s.BlockRef(lower), ascending ? min : max);
        s.ReportWrittenBlock(lower, w);
        TLanes.Store(ref s.BlockRef(upper), ascending ? max : min);
        s.ReportWrittenBlock(upper, w);
    }

    /// <summary>
    /// Lang's sort of the W elements from <paramref name="low"/>, as one vector: for block sizes 2, 4, …, W, each block is merged in the
    /// direction the recursion gives it.
    /// </summary>
    static void SortBlock<T, TComparer, TContext, TLanes, TVec>(SortSpan<T, TComparer, TContext> s, int low, bool ascending)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        where TLanes : struct, IBitonicLanes<TVec, T>
    {
        var v = TLanes.Load(ref s.ReadBlock(low, TLanes.Count));
        v = Level<T, TComparer, TContext, TLanes, TVec>(s, v, 1, ascending);
        if (TLanes.Count >= 4) v = Level<T, TComparer, TContext, TLanes, TVec>(s, v, 2, ascending);
        if (TLanes.Count >= 8) v = Level<T, TComparer, TContext, TLanes, TVec>(s, v, 3, ascending);
        if (TLanes.Count >= 16) v = Level<T, TComparer, TContext, TLanes, TVec>(s, v, 4, ascending);
        TLanes.Store(ref s.BlockRef(low), v);
        s.ReportWrittenBlock(low, TLanes.Count);
    }

    /// <summary>
    /// The merge of the W elements from <paramref name="low"/> in one direction, as one vector: stages at distances W / 2, …, 1.
    /// </summary>
    static void MergeBlock<T, TComparer, TContext, TLanes, TVec>(SortSpan<T, TComparer, TContext> s, int low, bool ascending)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        where TLanes : struct, IBitonicLanes<TVec, T>
    {
        var descending = ascending ? TLanes.None : TLanes.Not(TLanes.None);
        var v = TLanes.Load(ref s.ReadBlock(low, TLanes.Count));
        if (TLanes.Count >= 16) v = Step<T, TComparer, TContext, TLanes, TVec>(s, v, 8, descending);
        if (TLanes.Count >= 8) v = Step<T, TComparer, TContext, TLanes, TVec>(s, v, 4, descending);
        if (TLanes.Count >= 4) v = Step<T, TComparer, TContext, TLanes, TVec>(s, v, 2, descending);
        v = Step<T, TComparer, TContext, TLanes, TVec>(s, v, 1, descending);
        TLanes.Store(ref s.BlockRef(low), v);
        s.ReportWrittenBlock(low, TLanes.Count);
    }

    /// <summary>
    /// Merges every block of 2^<paramref name="log2Block"/> lanes in the direction Lang's recursion gives it within a vector sorted in the
    /// given direction.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static TVec Level<T, TComparer, TContext, TLanes, TVec>(SortSpan<T, TComparer, TContext> s, TVec v, int log2Block, bool ascending)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        where TLanes : struct, IBitonicLanes<TVec, T>
    {
        var descending = TLanes.DescendingBlocks(log2Block);
        if (!ascending) descending = TLanes.Not(descending);
        if (log2Block >= 4) v = Step<T, TComparer, TContext, TLanes, TVec>(s, v, 8, descending);
        if (log2Block >= 3) v = Step<T, TComparer, TContext, TLanes, TVec>(s, v, 4, descending);
        if (log2Block >= 2) v = Step<T, TComparer, TContext, TLanes, TVec>(s, v, 2, descending);
        return Step<T, TComparer, TContext, TLanes, TVec>(s, v, 1, descending);
    }

    /// <summary>
    /// Compares each lane l with lane l ^ <paramref name="distance"/>. The larger value goes to the lane whose bit is set in an ascending
    /// block and to the other lane in a block that <paramref name="descending"/> selects.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static TVec Step<T, TComparer, TContext, TLanes, TVec>(SortSpan<T, TComparer, TContext> s, TVec v, int distance, TVec descending)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        where TLanes : struct, IBitonicLanes<TVec, T>
    {
        var p = TLanes.Partner(v, distance);
        if (typeof(TContext) != typeof(NullContext)) s.ReportSelectedHeldLaneCompares(TLanes.LanesWithout(distance), TLanes.LessBits(v, p), TLanes.GreaterBits(v, p));
        return TLanes.Select(TLanes.Xor(TLanes.Upper(distance), descending), TLanes.Max(v, p), TLanes.Min(v, p));
    }

    // ---------------------------------------------------------------------------------------------------------------------------------
    // One comparator at a time
    // ---------------------------------------------------------------------------------------------------------------------------------

    static void SortModules<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int low, int count, bool ascending)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (count <= 1) return;

        var half = count / 2;
        if (half > 1) SortModules(s, low, half, !ascending);
        SortModules(s, low + half, count - half, ascending);
        MergeModules(s, low, count, ascending);
    }

    static void MergeModules<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int low, int count, bool ascending)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (count <= 1) return;

        var m = BitonicSortAnyLength.GreatestPowerOfTwoLessThan(count);
        for (var i = low; i < low + count - m; i++)
        {
            Module(s, i, i + m, ascending);
        }

        if (m > 1) MergeModules(s, low, m, ascending);
        if (count - m > 1) MergeModules(s, low + m, count - m, ascending);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Module<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int lower, int upper, bool ascending)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (ascending) BranchlessComparator.Ascending(s, lower, upper);
        else BranchlessComparator.Descending(s, lower, upper);
    }
}
