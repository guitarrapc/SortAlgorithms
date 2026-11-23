using SortAlgorithm.Contexts;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// Chhugani らの SIMD マージソート（2008）です。K 本のベクトルに K² 要素を読み込み、レーンごとの odd-even 整列網と転置で長さ K の整列列を作り、
/// 2 つの整列列をバイトニックマージ網で 1 単位ずつ併合することを、キャッシュに収まるブロックの中で、続いてブロックをまたいで繰り返します。
/// 併合のたびに分岐するのは次の単位をどちらの列から読むかだけで、それ以外の比較はすべてベクトルの最小・最大で行います。
/// <br/>
/// Chhugani et al.'s SIMD merge sort (2008). K² elements are loaded into K vectors, every lane is sorted by an odd-even sorting network
/// across the vectors, and a transpose turns each vector into a sorted sequence of K elements. Pairs of sorted sequences are then merged one
/// unit at a time by a bitonic merge network, first inside blocks that fit in the cache and then across the blocks. The only branch of a
/// merge chooses which sequence the next unit is read from; every other comparison is a vector minimum and maximum.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct SIMD Merge Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>In-Register Sort:</strong> K² elements are loaded into K vectors of K lanes. An odd-even sorting network of K
/// inputs is applied to the vectors, each comparator taking the minimum and the maximum of two whole vectors, so every lane ends sorted
/// across the vectors. A transpose then makes each vector one lane, a sorted sequence of K elements.</description></item>
/// <item><description><strong>Bitonic Merge Network:</strong> Two sorted sequences of U elements each are merged by reversing the second,
/// which makes the pair bitonic, and comparing element i with element i + U, then each half at distances U/2, …, 1. The lower half of the
/// result is the U smallest elements, sorted, and the upper half the U largest.</description></item>
/// <item><description><strong>Merging Two Sorted Lists:</strong> The first unit of each list is merged by the network; the lower half is
/// output and the upper half is kept. The next unit comes from the list whose next element is smaller, and is merged with the kept half,
/// until both lists are exhausted and the kept half is output. Every output unit is then no greater than every element not yet output, so
/// the output is sorted. Lists of L elements each take 2L/U − 1 networks.</description></item>
/// <item><description><strong>Blocks:</strong> The input is divided into blocks of M elements, each sorted on its own: the in-register sort
/// makes sequences of K elements, and pairs of sequences are merged until the block is one sequence. Four pairs are merged at once with
/// K-wide units while there are four; the last two pairs of a block are merged at once with 2K-wide units and the last pair with a 4K-wide
/// unit. Pairs of sorted blocks are then merged the same way until one sequence remains.</description></item>
/// <item><description><strong>Comparison Consistency:</strong> The vector minima and maxima order the elements as their natural order does,
/// which holds for the primitive 32- and 64-bit integer types the vector path runs on. Every other type, and every other comparer, runs the
/// same algorithm with one-lane vectors.</description></item>
/// </list>
/// <para><strong>Implementation notes:</strong></para>
/// <list type="bullet">
/// <item><description>Vector width: K is the lane count of the widest of 512-, 256- and 128-bit vectors that the runtime accelerates, written with
/// the cross-platform vector API: 16, 8 or 4 for 32-bit keys, and 8, 4 or 2 for 64-bit keys. The paper uses 4-wide SSE and states that the
/// algorithm holds for any K.</description></item>
/// <item><description>Block size: M = C / 2E with C = 1 MiB, rounded down to a power of two: 131,072 32-bit or 65,536 64-bit keys. The paper
/// takes C from the L2 cache of the machine; a fixed C makes the observed operations the same on every machine with the same K.</description></item>
/// <item><description>Unit width: a merge level uses K-wide units, four pairs at a time, when it has at least four pairs, 2K-wide units two at a
/// time when it has two or three, and one 4K-wide unit when it has one. This is the paper's rule for a full block (four networks in flight)
/// and the rule of its analysis for the levels across blocks. A unit is never wider than the sequences it merges. Merges run at once advance
/// one network each in turn.</description></item>
/// <item><description>Lengths: the paper takes N and M as powers of two. Here every position from n on holds, virtually, the largest key, so
/// sequences keep their lengths; a vector that reaches past n is loaded with those lanes filled with the largest key and stored without them.
/// A pair whose second sequence starts at or past the end of its block is copied unchanged. The chunk that holds element n − 1 is transposed
/// once before its network, so that each of its vectors sorts K consecutive positions and the filled lanes end up past n.</description></item>
/// <item><description>Omitted: the parallel steps (4.3) and the multiway merging of the paper's Section 4.4, which merges all blocks in one
/// pass when the merges across blocks are bound by memory bandwidth. The network's shuffles are single-vector lane permutations and blends
/// rather than the paper's two-register shuffles; they perform the same comparators.</description></item>
/// <item><description>Buffers: 0 is the input and 1 an auxiliary array of n elements; each merge level reads one and writes the other, and each
/// block starts so that the last level ends in the input.</description></item>
/// <item><description>Observation: a vector load is reported as one read per lane that holds an element, a vector store as one write per
/// lane stored, both in index order. A network comparator between two vectors is K comparisons, and a comparator inside a vector K/2, of
/// values held in registers, reported as -1; lanes that hold the largest key past n are compared and reported too. The choice of the next unit
/// is a comparison of the two list heads in memory. The one-lane path compares only elements; a position past n counts as greater without a
/// comparison.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Merge (Bottom-up, Sorting Networks)</description></item>
/// <item><description>Stable      : No (the networks exchange lanes far apart and keep no order between equal keys)</description></item>
/// <item><description>In-place    : No (an auxiliary array of n elements)</description></item>
/// <item><description>Best case   : Θ(n log n) - the networks, the passes and the units do not depend on the keys</description></item>
/// <item><description>Average case: Θ(n log n) - log₂(n/K) merge levels each pass every element through one network</description></item>
/// <item><description>Worst case  : Θ(n log n) - the same work as every other input</description></item>
/// <item><description>Comparisons : Θ(n log n log K) - each merge level costs log₂ 2U comparators per element (U = K, 2K or 4K) and one head comparison per unit; the in-register sort costs Θ(log² K) per element</description></item>
/// <item><description>Swaps       : 0 - the networks write minima and maxima into registers and nothing is exchanged in memory</description></item>
/// <item><description>Index Reads : Θ(n log(n/K)) - each level loads every element once and reads two list heads per unit; the in-register sort loads every element once</description></item>
/// <item><description>Index Writes: Θ(n log(n/K)) - each level stores every element once, merged or copied; the in-register sort stores every element once</description></item>
/// <item><description>Space       : O(n) - the auxiliary array; the vectors of a chunk and of four units are held in registers or on the stack</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Paper: J. Chhugani, A. D. Nguyen, V. W. Lee, W. Macy, M. Hagog, Y.-K. Chen, A. Baransi, S. Kumar and P. Dubey, "Efficient
/// Implementation of Sorting on Multi-Core SIMD CPU Architecture", PVLDB 1(2), 1313–1324, 2008. https://www.vldb.org/pvldb/vol1/1454171.pdf</para>
/// </remarks>
public static class SimdMergeSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by SimdMergeSortTests, which derives from SortTestsBase.</remarks>
    public static bool IsStable => false;

    // Buffer identifiers for visualization
    const int BUFFER_MAIN = 0;       // Main input array
    const int BUFFER_AUX = 1;        // Auxiliary array of n elements; merge levels alternate between the two

    // M = C / 2E with C = 1 MiB.
    const int BLOCK_BYTES = 512 << 10;

    // Four networks in flight: four merges with K-wide units, two with 2K-wide units, or one with a 4K-wide unit.
    const int NETWORKS_IN_FLIGHT = 4;

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
    /// <typeparam name="TComparer">The type of the comparer.</typeparam>
    /// <typeparam name="TContext">The type of the sort context.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="comparer">The comparer to use for element comparisons.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        => Sort(span, 0, span.Length, comparer, context);

    /// <summary>
    /// Sorts the subrange [first..last) using the provided sort context.
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
    /// This is the full-control version with explicit TComparer and TContext type parameters.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use for element comparisons.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="span">The span containing elements to sort.</param>
    /// <param name="first">The inclusive start index of the range to sort.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    /// <param name="comparer">The comparer to use for element comparisons.</param>
    /// <param name="context">The sort context for tracking statistics and observations.</param>
    public static void Sort<T, TComparer, TContext>(Span<T> span, int first, int last, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        ArgumentOutOfRangeException.ThrowIfNegative(first);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(last, span.Length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(first, last);

        var n = last - first;
        if (n < 2) return;

        var rented = ArrayPool<T>.Shared.Rent(n);
        try
        {
            var a = new SortSpan<T, TComparer, TContext>(span.Slice(first, n), context, comparer, BUFFER_MAIN, first);
            var z = new SortSpan<T, TComparer, TContext>(rented.AsSpan(0, n), context, comparer, BUFFER_AUX);

            if (SimdQuickSort.IsVectorizable<T, TComparer>())
            {
                if (BitonicLanes512<T>.IsSupported)
                {
                    Run<T, TComparer, TContext, VectorKernel<T, TComparer, TContext, BitonicLanes512<T>, Vector512<T>>>(a, z, n);
                    return;
                }
                if (BitonicLanes256<T>.IsSupported)
                {
                    Run<T, TComparer, TContext, VectorKernel<T, TComparer, TContext, BitonicLanes256<T>, Vector256<T>>>(a, z, n);
                    return;
                }
                if (BitonicLanes128<T>.IsSupported)
                {
                    Run<T, TComparer, TContext, VectorKernel<T, TComparer, TContext, BitonicLanes128<T>, Vector128<T>>>(a, z, n);
                    return;
                }
            }

            Run<T, TComparer, TContext, ScalarKernel<T, TComparer, TContext>>(a, z, n);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(rented, RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    /// <summary>
    /// The lanes per vector this algorithm uses for <typeparamref name="T"/> and <typeparamref name="TComparer"/> on this machine,
    /// 1 on the one-lane path.
    /// </summary>
    internal static int LaneCount<T, TComparer>()
        => !SimdQuickSort.IsVectorizable<T, TComparer>() ? 1
        : BitonicLanes512<T>.IsSupported ? Vector512<T>.Count
        : BitonicLanes256<T>.IsSupported ? Vector256<T>.Count
        : BitonicLanes128<T>.IsSupported ? Vector128<T>.Count
        : 1;

    /// <summary>M: the block length in elements.</summary>
    internal static int BlockLength<T>() => 1 << BitOperations.Log2((uint)Math.Max(1, BLOCK_BYTES / Unsafe.SizeOf<T>()));

    /// <summary>
    /// The comparators of Batcher's odd-even merge sort of <paramref name="k"/> inputs (a power of two), as consecutive pairs.
    /// It has K − 1 + K log₂K (log₂K − 1) / 4 comparators.
    /// </summary>
    internal static int[] OddEvenSortingNetwork(int k)
    {
        var pairs = new List<int>();
        for (var p = 1; p < k; p <<= 1)
        {
            for (var d = p; d >= 1; d >>= 1)
            {
                for (var j = d % p; j <= k - 1 - d; j += 2 * d)
                {
                    for (var i = 0; i <= Math.Min(d - 1, k - j - d - 1); i++)
                    {
                        if ((i + j) / (p * 2) == (i + j + d) / (p * 2))
                        {
                            pairs.Add(i + j);
                            pairs.Add(i + j + d);
                        }
                    }
                }
            }
        }
        return [.. pairs];
    }

    /// <summary>
    /// The steps one path of the algorithm needs: the lane count, the in-register sort of a block's chunks, and the merging of a group of
    /// pairs at once.
    /// </summary>
    interface IKernel<T, TComparer, TContext>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        static abstract int Lanes { get; }

        /// <summary>Sorts every chunk of K² elements of src[start..end) into sequences of K elements in dst.</summary>
        static abstract void SortChunks(SortSpan<T, TComparer, TContext> src, SortSpan<T, TComparer, TContext> dst, int start, int end, int n);

        /// <summary>
        /// Merges, at once, each pair src[x..x + run) and src[x + run..min(x + 2·run, hi)) for x in <paramref name="starts"/> into dst[x..),
        /// with units of <paramref name="width"/> vectors.
        /// </summary>
        static abstract void MergeGroup(SortSpan<T, TComparer, TContext> src, SortSpan<T, TComparer, TContext> dst, ReadOnlySpan<int> starts, int run, int width, int hi, int n);
    }

    /// <summary>
    /// Sorts the blocks one after another, then merges across them, so that the last level ends in <paramref name="a"/>.
    /// </summary>
    static void Run<T, TComparer, TContext, TKernel>(SortSpan<T, TComparer, TContext> a, SortSpan<T, TComparer, TContext> z, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        where TKernel : struct, IKernel<T, TComparer, TContext>
    {
        var k = TKernel.Lanes;
        var block = BlockLength<T>();

        var levelsAcross = 0;
        for (long run = block; run < n; run *= 2) levelsAcross++;
        // Where the blocks must be when the levels across them start.
        var blocksInMain = levelsAcross % 2 == 0;

        for (long blockStart = 0; blockStart < n; blockStart += block)
        {
            var start = (int)blockStart;
            var end = (int)Math.Min(blockStart + block, n);
            var levels = 0;
            for (long run = k; run < end - start; run *= 2) levels++;

            var inMain = blocksInMain ^ (levels % 2 == 1);
            a.Context.OnPhase(SortPhase.SimdMergeSortInRegisterSort, a.Offset + start, a.Offset + end - 1, k);
            TKernel.SortChunks(a, inMain ? a : z, start, end, n);

            for (var run = k; run < end - start; run *= 2)
            {
                a.Context.OnPhase(SortPhase.SimdMergeSortBlockMerge, a.Offset + start, a.Offset + end - 1, run);
                MergeLevel<T, TComparer, TContext, TKernel>(inMain ? a : z, inMain ? z : a, start, end, run, n);
                inMain = !inMain;
            }
        }

        var acrossInMain = blocksInMain;
        for (long run = block; run < n; run *= 2)
        {
            a.Context.OnPhase(SortPhase.SimdMergeSortMerge, a.Offset, a.Offset + n - 1, (int)run);
            MergeLevel<T, TComparer, TContext, TKernel>(acrossInMain ? a : z, acrossInMain ? z : a, 0, n, (int)run, n);
            acrossInMain = !acrossInMain;
        }
    }

    /// <summary>
    /// Merges the pairs of sequences of <paramref name="run"/> elements in src[lo..hi) into dst, in groups that keep four networks in
    /// flight. A last sequence without a partner is copied.
    /// </summary>
    static void MergeLevel<T, TComparer, TContext, TKernel>(SortSpan<T, TComparer, TContext> src, SortSpan<T, TComparer, TContext> dst, int lo, int hi, int run, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        where TKernel : struct, IKernel<T, TComparer, TContext>
    {
        var pairs = 0L;
        for (long x = lo; x + run < hi; x += 2L * run) pairs++;
        var width = pairs >= 4 ? 1 : pairs >= 2 ? 2 : 4;
        width = Math.Min(width, run / TKernel.Lanes);
        var group = NETWORKS_IN_FLIGHT / width;

        Span<int> starts = stackalloc int[NETWORKS_IN_FLIGHT];
        var count = 0;
        for (long x = lo; x < hi; x += 2L * run)
        {
            if (x + run >= hi)
            {
                if (count > 0) TKernel.MergeGroup(src, dst, starts[..count], run, width, hi, n);
                count = 0;
                src.CopyTo((int)x, dst, (int)x, hi - (int)x);
                break;
            }

            starts[count++] = (int)x;
            if (count == group)
            {
                TKernel.MergeGroup(src, dst, starts[..count], run, width, hi, n);
                count = 0;
            }
        }
        if (count > 0) TKernel.MergeGroup(src, dst, starts[..count], run, width, hi, n);
    }

    /// <summary>The largest value of a primitive integer type: the key of every position past n.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T LargestKey<T>()
    {
        if (typeof(T) == typeof(int)) { var v = int.MaxValue; return Unsafe.As<int, T>(ref v); }
        if (typeof(T) == typeof(uint)) { var v = uint.MaxValue; return Unsafe.As<uint, T>(ref v); }
        if (typeof(T) == typeof(long)) { var v = long.MaxValue; return Unsafe.As<long, T>(ref v); }
        if (typeof(T) == typeof(ulong)) { var v = ulong.MaxValue; return Unsafe.As<ulong, T>(ref v); }
        if (typeof(T) == typeof(nint)) { var v = nint.MaxValue; return Unsafe.As<nint, T>(ref v); }
        if (typeof(T) == typeof(nuint)) { var v = nuint.MaxValue; return Unsafe.As<nuint, T>(ref v); }
        throw new NotSupportedException();
    }

    [InlineArray(16)]
    struct Vectors16<TVec>
    {
        TVec element;
    }

    [InlineArray(4)]
    struct Vectors4<TVec>
    {
        TVec element;
    }

    /// <summary>64 bytes, the size of the widest vector.</summary>
    [InlineArray(64)]
    struct Scratch
    {
        byte element;
    }

    /// <summary>The state of one merge of a group: the read positions and ends of both lists, and the next output position.</summary>
    struct MergeState
    {
        public int X;
        public int XEnd;
        public int Y;
        public int YEnd;
        public int Out;
        public int Step; // 0: not started, 1: merging, 2: done
    }

    /// <summary>
    /// The vector path: K lanes per vector.
    /// </summary>
    readonly struct VectorKernel<T, TComparer, TContext, TLanes, TVec> : IKernel<T, TComparer, TContext>
        where TComparer : IComparer<T>
        where TContext : ISortContext
        where TLanes : struct, IBitonicLanes<TVec, T>
    {
        public static int Lanes => TLanes.Count;

        public static void SortChunks(SortSpan<T, TComparer, TContext> src, SortSpan<T, TComparer, TContext> dst, int start, int end, int n)
        {
            var k = TLanes.Count;
            var network = Networks.For(k);
            var buffer = new Vectors16<TVec>();
            Span<TVec> r = buffer;
            r = r[..k];

            for (var chunk = start; chunk < end; chunk += k * k)
            {
                var partial = chunk + k * k > n;
                for (var j = 0; j < k; j++) r[j] = Load(src, chunk + j * k, n);

                // The chunk holding position n − 1 sorts K consecutive positions per vector, so the lanes past n end past n.
                if (partial) Transpose(r);
                for (var p = 0; p < network.Length; p += 2) Exchange(src, ref r[network[p]], ref r[network[p + 1]]);
                Transpose(r);

                for (var j = 0; j < k; j++) Store(dst, chunk + j * k, n, r[j]);
            }
        }

        public static void MergeGroup(SortSpan<T, TComparer, TContext> src, SortSpan<T, TComparer, TContext> dst, ReadOnlySpan<int> starts, int run, int width, int hi, int n)
        {
            Span<MergeState> states = stackalloc MergeState[NETWORKS_IN_FLIGHT];
            for (var j = 0; j < starts.Length; j++)
            {
                var x = starts[j];
                states[j] = new MergeState { X = x, XEnd = x + run, Y = x + run, YEnd = (int)Math.Min((long)x + 2L * run, hi), Out = x };
            }

            // A group of 2K-wide units holds two merges, and a group of 4K-wide units one.
            if (width == 1) MergeGroupOneVector(src, dst, states[..starts.Length], n);
            else if (width == 2) MergeGroupTwoVectors(src, dst, states[..starts.Length], n);
            else MergeFourVectors(src, dst, ref states[0], n);
        }

        // The merges below hold the vectors of a network in locals, not in a span: the JIT keeps a span's vectors in memory, and storing a
        // vector and loading it back at each stage of the network made the 512-bit path slower than the 256-bit one.

        /// <summary>
        /// Merges a group whose units are two vectors, each merge advancing one network in turn.
        /// </summary>
        static void MergeGroupTwoVectors(in SortSpan<T, TComparer, TContext> src, in SortSpan<T, TComparer, TContext> dst, Span<MergeState> states, int n)
        {
            var k = TLanes.Count;
            Unsafe.SkipInit(out Vectors4<TVec> carryBuffer);
            Span<TVec> carries = carryBuffer;

            var active = states.Length;
            while (active > 0)
            {
                for (var j = 0; j < states.Length; j++)
                {
                    ref var s = ref states[j];
                    if (s.Step == 2) continue;

                    // c holds the first unit, then the kept upper half; the unit at p is merged with it.
                    TVec c0, c1;
                    int p;
                    if (s.Step == 0)
                    {
                        c0 = Load(src, s.X, n);
                        c1 = Load(src, s.X + k, n);
                        s.X += 2 * k;
                        p = s.Y;
                        s.Y += 2 * k;
                        s.Step = 1;
                    }
                    else
                    {
                        if (s.X < s.XEnd && (s.Y >= s.YEnd || !src.IsLessAt(s.Y, s.X)))
                        {
                            p = s.X;
                            s.X += 2 * k;
                        }
                        else if (s.Y < s.YEnd)
                        {
                            p = s.Y;
                            s.Y += 2 * k;
                        }
                        else
                        {
                            Store(dst, s.Out, n, carries[2 * j]);
                            Store(dst, s.Out + k, n, carries[2 * j + 1]);
                            s.Step = 2;
                            active--;
                            continue;
                        }
                        c0 = carries[2 * j];
                        c1 = carries[2 * j + 1];
                    }

                    var b0 = Load(src, p, n);
                    var b1 = Load(src, p + k, n);
                    Network(src, ref c0, ref c1, b0, b1, out var low0, out var low1);
                    carries[2 * j] = c0;
                    carries[2 * j + 1] = c1;
                    Store(dst, s.Out, n, low0);
                    Store(dst, s.Out + k, n, low1);
                    s.Out += 2 * k;
                }
            }
        }

        /// <summary>
        /// Merges one pair with units of four vectors.
        /// </summary>
        static void MergeFourVectors(in SortSpan<T, TComparer, TContext> src, in SortSpan<T, TComparer, TContext> dst, ref MergeState s, int n)
        {
            var k = TLanes.Count;
            // c holds the first unit, then the kept upper half; the unit at p is merged with it.
            var c0 = Load(src, s.X, n);
            var c1 = Load(src, s.X + k, n);
            var c2 = Load(src, s.X + 2 * k, n);
            var c3 = Load(src, s.X + 3 * k, n);
            s.X += 4 * k;
            var p = s.Y;
            s.Y += 4 * k;

            while (true)
            {
                var b0 = Load(src, p, n);
                var b1 = Load(src, p + k, n);
                var b2 = Load(src, p + 2 * k, n);
                var b3 = Load(src, p + 3 * k, n);
                Network(src, ref c0, ref c1, ref c2, ref c3, b0, b1, b2, b3, out var low0, out var low1, out var low2, out var low3);
                Store(dst, s.Out, n, low0);
                Store(dst, s.Out + k, n, low1);
                Store(dst, s.Out + 2 * k, n, low2);
                Store(dst, s.Out + 3 * k, n, low3);
                s.Out += 4 * k;

                if (s.X < s.XEnd && (s.Y >= s.YEnd || !src.IsLessAt(s.Y, s.X)))
                {
                    p = s.X;
                    s.X += 4 * k;
                }
                else if (s.Y < s.YEnd)
                {
                    p = s.Y;
                    s.Y += 4 * k;
                }
                else break;
            }

            Store(dst, s.Out, n, c0);
            Store(dst, s.Out + k, n, c1);
            Store(dst, s.Out + 2 * k, n, c2);
            Store(dst, s.Out + 3 * k, n, c3);
            s.Step = 2;
        }

        /// <summary>
        /// Merges a group whose units are single vectors, each merge advancing one network in turn.
        /// </summary>
        static void MergeGroupOneVector(in SortSpan<T, TComparer, TContext> src, in SortSpan<T, TComparer, TContext> dst, Span<MergeState> states, int n)
        {
            var k = TLanes.Count;
            Unsafe.SkipInit(out Vectors4<TVec> carryBuffer);
            Span<TVec> carries = carryBuffer;

            var active = states.Length;
            while (active > 0)
            {
                for (var j = 0; j < states.Length; j++)
                {
                    ref var s = ref states[j];
                    if (s.Step == 2) continue;

                    TVec low;
                    if (s.Step == 0)
                    {
                        var first = Load(src, s.X, n);
                        var second = Load(src, s.Y, n);
                        s.X += k;
                        s.Y += k;
                        Network(src, first, second, out low, out var high);
                        carries[j] = high;
                        s.Step = 1;
                    }
                    else
                    {
                        TVec loaded;
                        if (s.X < s.XEnd && (s.Y >= s.YEnd || !src.IsLessAt(s.Y, s.X)))
                        {
                            loaded = Load(src, s.X, n);
                            s.X += k;
                        }
                        else if (s.Y < s.YEnd)
                        {
                            loaded = Load(src, s.Y, n);
                            s.Y += k;
                        }
                        else
                        {
                            Store(dst, s.Out, n, carries[j]);
                            s.Step = 2;
                            active--;
                            continue;
                        }
                        Network(src, carries[j], loaded, out low, out var high);
                        carries[j] = high;
                    }

                    Store(dst, s.Out, n, low);
                    s.Out += k;
                }
            }
        }

        /// <summary>
        /// The bitonic merge network of two sorted vectors: b is reversed, lane i is compared with lane i of the other vector, then each
        /// vector is cleaned at distances K/2, …, 1.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Network(in SortSpan<T, TComparer, TContext> s, TVec a, TVec b, out TVec low, out TVec high)
        {
            var reversed = TLanes.Partner(b, TLanes.Count - 1);
            if (typeof(TContext) != typeof(NullContext)) s.ReportHeldLaneCompares(TLanes.Count, TLanes.LessBits(a, reversed), TLanes.GreaterBits(a, reversed));
            low = TLanes.Min(a, reversed);
            high = TLanes.Max(a, reversed);
            CleanLanes(s, ref low, ref high);
        }

        /// <summary>
        /// The bitonic merge network of two sorted units of two vectors, c and b: b is reversed, element i is compared with element i + U,
        /// then each half is cleaned at distances U/2, …, 1, first between the vectors and then inside them. low receives the lower half and
        /// c the upper.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Network(in SortSpan<T, TComparer, TContext> s, ref TVec c0, ref TVec c1, TVec b0, TVec b1, out TVec low0, out TVec low1)
        {
            var k = TLanes.Count;
            low0 = c0;
            low1 = c1;
            c0 = TLanes.Partner(b1, k - 1);
            c1 = TLanes.Partner(b0, k - 1);
            Exchange(s, ref low0, ref c0);
            Exchange(s, ref low1, ref c1);

            Exchange(s, ref low0, ref low1);
            Exchange(s, ref c0, ref c1);
            CleanLanes(s, ref low0, ref low1, ref c0, ref c1);
        }

        /// <summary>
        /// The bitonic merge network of two sorted units of four vectors, c and b: b is reversed, element i is compared with element i + U,
        /// then each half is cleaned at distances U/2, …, 1, first between the vectors and then inside them. low receives the lower half and
        /// c the upper.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Network(in SortSpan<T, TComparer, TContext> s, ref TVec c0, ref TVec c1, ref TVec c2, ref TVec c3, TVec b0, TVec b1, TVec b2, TVec b3,
            out TVec low0, out TVec low1, out TVec low2, out TVec low3)
        {
            var k = TLanes.Count;
            low0 = c0;
            low1 = c1;
            low2 = c2;
            low3 = c3;
            c0 = TLanes.Partner(b3, k - 1);
            c1 = TLanes.Partner(b2, k - 1);
            c2 = TLanes.Partner(b1, k - 1);
            c3 = TLanes.Partner(b0, k - 1);
            Exchange(s, ref low0, ref c0);
            Exchange(s, ref low1, ref c1);
            Exchange(s, ref low2, ref c2);
            Exchange(s, ref low3, ref c3);

            Exchange(s, ref low0, ref low2);
            Exchange(s, ref low1, ref low3);
            Exchange(s, ref c0, ref c2);
            Exchange(s, ref c1, ref c3);
            Exchange(s, ref low0, ref low1);
            Exchange(s, ref low2, ref low3);
            Exchange(s, ref c0, ref c1);
            Exchange(s, ref c2, ref c3);
            CleanLanes(s, ref low0, ref low1, ref low2, ref low3, ref c0, ref c1, ref c2, ref c3);
        }

        /// <summary>The stages at distances K/2, …, 1 inside four vectors, each stage applied to them in order.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void CleanLanes(in SortSpan<T, TComparer, TContext> s, ref TVec v0, ref TVec v1, ref TVec v2, ref TVec v3)
        {
            if (TLanes.Count >= 16) CleanLanes(s, 8, ref v0, ref v1, ref v2, ref v3);
            if (TLanes.Count >= 8) CleanLanes(s, 4, ref v0, ref v1, ref v2, ref v3);
            if (TLanes.Count >= 4) CleanLanes(s, 2, ref v0, ref v1, ref v2, ref v3);
            CleanLanes(s, 1, ref v0, ref v1, ref v2, ref v3);
        }

        /// <summary>The stages at distances K/2, …, 1 inside eight vectors, each stage applied to them in order.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void CleanLanes(in SortSpan<T, TComparer, TContext> s, ref TVec v0, ref TVec v1, ref TVec v2, ref TVec v3,
            ref TVec v4, ref TVec v5, ref TVec v6, ref TVec v7)
        {
            if (TLanes.Count >= 16)
            {
                CleanLanes(s, 8, ref v0, ref v1, ref v2, ref v3);
                CleanLanes(s, 8, ref v4, ref v5, ref v6, ref v7);
            }
            if (TLanes.Count >= 8)
            {
                CleanLanes(s, 4, ref v0, ref v1, ref v2, ref v3);
                CleanLanes(s, 4, ref v4, ref v5, ref v6, ref v7);
            }
            if (TLanes.Count >= 4)
            {
                CleanLanes(s, 2, ref v0, ref v1, ref v2, ref v3);
                CleanLanes(s, 2, ref v4, ref v5, ref v6, ref v7);
            }
            CleanLanes(s, 1, ref v0, ref v1, ref v2, ref v3);
            CleanLanes(s, 1, ref v4, ref v5, ref v6, ref v7);
        }

        /// <summary>One stage at lane distance <paramref name="d"/> inside four vectors, in order.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void CleanLanes(in SortSpan<T, TComparer, TContext> s, int d, ref TVec v0, ref TVec v1, ref TVec v2, ref TVec v3)
        {
            CleanLanes(s, ref v0, d);
            CleanLanes(s, ref v1, d);
            CleanLanes(s, ref v2, d);
            CleanLanes(s, ref v3, d);
        }

        /// <summary>
        /// The stages at distances K/2, …, 1 inside the vectors, each applied to <paramref name="low"/> and then to <paramref name="high"/>.
        /// The distances are constants, so each stage's permutation and mask are too.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void CleanLanes(in SortSpan<T, TComparer, TContext> s, ref TVec low, ref TVec high)
        {
            if (TLanes.Count >= 16)
            {
                CleanLanes(s, ref low, 8);
                CleanLanes(s, ref high, 8);
            }
            if (TLanes.Count >= 8)
            {
                CleanLanes(s, ref low, 4);
                CleanLanes(s, ref high, 4);
            }
            if (TLanes.Count >= 4)
            {
                CleanLanes(s, ref low, 2);
                CleanLanes(s, ref high, 2);
            }
            CleanLanes(s, ref low, 1);
            CleanLanes(s, ref high, 1);
        }

        /// <summary>One stage inside a vector at lane distance <paramref name="d"/>: the smaller of each pair to the lower lane.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void CleanLanes(in SortSpan<T, TComparer, TContext> s, ref TVec v, int d)
        {
            var partner = TLanes.Partner(v, d);
            if (typeof(TContext) != typeof(NullContext)) s.ReportSelectedHeldLaneCompares(TLanes.LanesWithout(d), TLanes.LessBits(v, partner), TLanes.GreaterBits(v, partner));
            v = TLanes.Select(TLanes.Upper(d), TLanes.Max(v, partner), TLanes.Min(v, partner));
        }

        /// <summary>A comparator between two vectors: the lane minima to <paramref name="lo"/> and the maxima to <paramref name="hi"/>.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Exchange(in SortSpan<T, TComparer, TContext> s, ref TVec lo, ref TVec hi)
        {
            if (typeof(TContext) != typeof(NullContext)) s.ReportHeldLaneCompares(TLanes.Count, TLanes.LessBits(lo, hi), TLanes.GreaterBits(lo, hi));
            var min = TLanes.Min(lo, hi);
            hi = TLanes.Max(lo, hi);
            lo = min;
        }

        /// <summary>
        /// Transposes K vectors of K lanes in place: for each bit d, vectors i and i + d (i without bit d) exchange the lanes with bit d set in
        /// the first and clear in the second.
        /// </summary>
        static void Transpose(Span<TVec> r)
        {
            for (var d = 1; d < r.Length; d <<= 1)
            {
                var upper = TLanes.Upper(d);
                for (var i = 0; i < r.Length; i++)
                {
                    if ((i & d) != 0) continue;
                    var a = r[i];
                    var b = r[i + d];
                    r[i] = TLanes.Select(upper, TLanes.Partner(b, d), a);
                    r[i + d] = TLanes.Select(upper, b, TLanes.Partner(a, d));
                }
            }
        }

        /// <summary>The vector at <paramref name="p"/>, its lanes from <paramref name="n"/> on holding the largest key.</summary>
        // The partial vector is filled in memory by a call that returns nothing: a call returning the vector would make the JIT keep every
        // vector this method returns in memory, as the call's return buffer.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static TVec Load(in SortSpan<T, TComparer, TContext> s, int p, int n)
        {
            var k = TLanes.Count;
            if (n - p >= k) return TLanes.Load(ref s.ReadBlock(p, k));
            Unsafe.SkipInit(out Scratch scratch);
            ref var lanes = ref Unsafe.As<Scratch, T>(ref scratch);
            FillPartial(s, p, n - p, ref lanes);
            return TLanes.Load(ref lanes);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void FillPartial(in SortSpan<T, TComparer, TContext> s, int p, int count, ref T lanes)
        {
            TLanes.Store(ref lanes, TLanes.Broadcast(LargestKey<T>()));
            if (count <= 0) return;
            ref var from = ref s.ReadBlock(p, count);
            for (var i = 0; i < count; i++) Unsafe.Add(ref lanes, i) = Unsafe.Add(ref from, i);
        }

        /// <summary>Stores the lanes of <paramref name="v"/> that fall before <paramref name="n"/>.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Store(in SortSpan<T, TComparer, TContext> s, int p, int n, TVec v)
        {
            var k = TLanes.Count;
            if (n - p >= k)
            {
                TLanes.Store(ref s.BlockRef(p), v);
                s.ReportWrittenBlock(p, k);
                return;
            }
            if (n - p > 0) StorePartial(s, p, n - p, v);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void StorePartial(in SortSpan<T, TComparer, TContext> s, int p, int count, TVec v)
        {
            Unsafe.SkipInit(out Scratch buffer);
            ref var scratch = ref Unsafe.As<Scratch, T>(ref buffer);
            TLanes.Store(ref scratch, v);
            ref var to = ref s.BlockRef(p);
            for (var i = 0; i < count; i++) Unsafe.Add(ref to, i) = Unsafe.Add(ref scratch, i);
            s.ReportWrittenBlock(p, count);
        }
    }

    /// <summary>
    /// The one-lane path: K = 1, every comparison through the comparer. A unit holds up to four elements; a position past n holds no
    /// element and counts as greater than every element.
    /// </summary>
    readonly struct ScalarKernel<T, TComparer, TContext> : IKernel<T, TComparer, TContext>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        public static int Lanes => 1;

        // With one lane a chunk is one element and its network is empty: the element is loaded and stored.
        public static void SortChunks(SortSpan<T, TComparer, TContext> src, SortSpan<T, TComparer, TContext> dst, int start, int end, int n)
        {
            for (var i = start; i < end; i++) dst.Write(i, src.Read(i));
        }

        public static void MergeGroup(SortSpan<T, TComparer, TContext> src, SortSpan<T, TComparer, TContext> dst, ReadOnlySpan<int> starts, int run, int width, int hi, int n)
        {
            Span<MergeState> states = stackalloc MergeState[NETWORKS_IN_FLIGHT];
            Span<int> carryCount = stackalloc int[NETWORKS_IN_FLIGHT];
            for (var j = 0; j < starts.Length; j++)
            {
                var x = starts[j];
                states[j] = new MergeState { X = x, XEnd = x + run, Y = x + run, YEnd = (int)Math.Min((long)x + 2L * run, hi), Out = x };
            }

            var carryBuffer = new Elements<T>();
            var unitBuffer = new Elements<T>();
            var mergedBuffer = new Elements<T>();
            Span<T> carries = carryBuffer;
            var unit = ((Span<T>)unitBuffer)[..width];
            var merged = ((Span<T>)mergedBuffer)[..(2 * width)];

            var active = starts.Length;
            while (active > 0)
            {
                for (var j = 0; j < starts.Length; j++)
                {
                    ref var s = ref states[j];
                    if (s.Step == 2) continue;
                    var carry = carries.Slice(j * width, width);

                    if (s.Step == 0)
                    {
                        var firstCount = LoadUnit(src, s.X, n, carry);
                        var secondCount = LoadUnit(src, s.Y, n, unit);
                        s.X += width;
                        s.Y += width;
                        carryCount[j] = Merge(src, dst, ref s, n, carry, firstCount, unit, secondCount, merged);
                        s.Step = 1;
                        continue;
                    }

                    bool fromX;
                    if (s.X < s.XEnd) fromX = s.Y >= s.YEnd || !src.IsLessAt(s.Y, s.X);
                    else if (s.Y < s.YEnd) fromX = false;
                    else
                    {
                        for (var i = 0; i < carryCount[j]; i++) dst.Write(s.Out + i, carry[i]);
                        s.Step = 2;
                        active--;
                        continue;
                    }

                    int count;
                    if (fromX)
                    {
                        count = LoadUnit(src, s.X, n, unit);
                        s.X += width;
                    }
                    else
                    {
                        count = LoadUnit(src, s.Y, n, unit);
                        s.Y += width;
                    }
                    carryCount[j] = Merge(src, dst, ref s, n, carry, carryCount[j], unit, count, merged);
                }
            }
        }

        /// <summary>
        /// Runs the network on <paramref name="a"/> (the first <paramref name="aCount"/> are elements) and <paramref name="b"/>, stores the
        /// elements of the lower half, keeps the upper half in <paramref name="a"/>, and returns how many elements it holds.
        /// </summary>
        static int Merge(SortSpan<T, TComparer, TContext> src, SortSpan<T, TComparer, TContext> dst, ref MergeState state, int n,
            Span<T> a, int aCount, Span<T> b, int bCount, Span<T> merged)
        {
            var width = a.Length;
            // merged = a followed by b reversed; positions holding no element are flagged, and they are always a's tail and b's head.
            Span<bool> present = stackalloc bool[8];
            for (var i = 0; i < width; i++)
            {
                merged[i] = a[i];
                present[i] = i < aCount;
                merged[width + i] = b[width - 1 - i];
                present[width + i] = width - 1 - i < bCount;
            }

            for (var i = 0; i < width; i++) Exchange(src, merged, present, i, i + width);
            for (var d = width / 2; d >= 1; d >>= 1)
            {
                CleanHalf(src, merged, present, 0, width, d);
                CleanHalf(src, merged, present, width, width, d);
            }

            var total = aCount + bCount;
            var lowCount = Math.Min(width, total);
            for (var i = 0; i < lowCount; i++) dst.Write(state.Out + i, merged[i]);
            state.Out += width;
            for (var i = 0; i < width; i++) a[i] = merged[width + i];
            return total - lowCount;
        }

        static void CleanHalf(SortSpan<T, TComparer, TContext> s, Span<T> v, Span<bool> present, int offset, int length, int d)
        {
            for (var i = 0; i < length; i++)
            {
                if ((i & d) == 0) Exchange(s, v, present, offset + i, offset + i + d);
            }
        }

        /// <summary>A comparator: the smaller to <paramref name="i"/>. A position without an element is greater and is never compared.</summary>
        static void Exchange(SortSpan<T, TComparer, TContext> s, Span<T> v, Span<bool> present, int i, int j)
        {
            if (!present[j]) return;
            if (!present[i] || s.IsGreaterThan(v[i], v[j]))
            {
                (v[i], v[j]) = (v[j], v[i]);
                (present[i], present[j]) = (present[j], present[i]);
            }
        }

        /// <summary>Reads the elements of the unit at <paramref name="p"/> that fall before <paramref name="n"/> and returns their count.</summary>
        static int LoadUnit(SortSpan<T, TComparer, TContext> s, int p, int n, Span<T> into)
        {
            var count = Math.Min(into.Length, n - p);
            for (var i = 0; i < count; i++) into[i] = s.Read(p + i);
            return count;
        }
    }

    [InlineArray(8)]
    struct Elements<T>
    {
        T element;
    }

    /// <summary>The odd-even sorting networks of 2, 4, 8 and 16 inputs.</summary>
    static class Networks
    {
        static readonly int[][] ByLog = [[], OddEvenSortingNetwork(2), OddEvenSortingNetwork(4), OddEvenSortingNetwork(8), OddEvenSortingNetwork(16)];

        public static int[] For(int k) => ByLog[BitOperations.Log2((uint)k)];
    }
}
