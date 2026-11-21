using SortAlgorithm.Contexts;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 8 要素ごとの比較結果から整列済み・逆順・未整列を見分けるクアッドスワップで 32 要素のブロックを作り、
/// 4 ブロックずつ両端から同時に進めるパリティマージ／クロスマージで統合する、ボトムアップの適応型安定マージソートです（van den Hoven, 2020）。
/// 整列済みまたは逆順の入力は n 回程度の比較で終わり、短い区間は分岐のないパリティマージで並べます。
/// <br/>
/// A bottom-up, adaptive, stable merge sort (van den Hoven, 2020). A quad swap analyzer classifies every group of eight elements
/// as in order, in reverse order, or neither, turning the input into sorted blocks of 32. Quad merges then
/// join four blocks at a time with branchless parity and cross merges that work from both ends of the output at once.
/// Ordered and reverse-ordered input finish after about n comparisons, and short ranges are sorted by branchless parity merges.
/// </summary>
/// <remarks>
/// <para><strong>Algorithm:</strong></para>
/// <list type="number">
/// <item><description><strong>Small input:</strong> fewer than 32 elements are sorted by <c>tail_swap</c>, which splits the range into four quarters,
/// sorts each recursively, and joins them with two parity merges into a stack array and one back. Two to seven elements use the
/// fixed parity swap networks of <c>tiny_sort</c>.</description></item>
/// <item><description><strong>Quad swap:</strong> each group of eight is compared as four pairs. All four in order, with the pairs also in order across
/// their boundaries, continues an ordered stretch. All four strictly descending, again across the boundaries, continues a descending
/// stretch that is reversed in place once it ends. Anything else gets four branchless pair swaps and two levels of parity merges.
/// Groups of four eight-element blocks are then parity merged into blocks of 32. A whole input that is one descending stretch is
/// reversed and the sort ends here.</description></item>
/// <item><description><strong>Quad merge:</strong> four adjacent blocks are merged into swap memory pairwise and back as one, doubling the block size
/// twice per level. A pair whose boundary is already in order is copied instead of merged, and four blocks already in order are left
/// alone. Each merge is a cross merge: it copies eight elements at a time from whichever end of a run cannot interleave with the other
/// run, and otherwise merges branchlessly from the head and the tail of the output at once.</description></item>
/// <item><description><strong>Tail merge:</strong> the runs the quad merges leave are merged pairwise. When both runs fit in swap
/// memory and the right run has at least 64 elements, they are cross merged there and copied back. Otherwise the right run is copied
/// to swap memory and the two are merged backwards.</description></item>
/// <item><description><strong>Rotate merge:</strong> when the runs outgrow swap memory, the remaining levels are merged in place by splitting
/// the left run, binary searching its upper half's first element in the right run, rotating the pieces with a trinity rotation,
/// and recursing until the pieces fit in swap memory.</description></item>
/// </list>
/// <para><strong>Implementation notes:</strong></para>
/// <list type="bullet">
/// <item><description>Configuration: this is a port of quadsort 1.2.1.3 in the configuration where the comparison is inlined
/// (<c>#define cmp</c>), which is what a struct <typeparamref name="TComparer"/> becomes after JIT specialization. That makes the
/// gcc-only <c>QUAD_CACHE</c> switch to branchy merges unreachable, so it is not ported.</description></item>
/// <item><description>Branchless steps: the reference has a gcc and a clang form of its branchless merge step. This port uses the gcc
/// form, which writes both candidates (the one not taken is overwritten or left unused) and advances the cursors
/// arithmetically: the clang form selects the value to write, and RyuJIT compiles that selection inside a loop to a data-dependent
/// branch (verified by disassembly). Where the reference writes the merge step out as a plain ternary, at the ends of a merge,
/// one position is written. The branchless pair swaps write both positions whatever the comparison decided, as the reference does,
/// choosing the destination index rather than the value.</description></item>
/// <item><description>Fidelity: with a consistent comparer and identical inputs, this port makes the comparisons the reference makes, in the same order.
/// ScandumReferenceTraceTests holds it to that.</description></item>
/// <item><description>Swap memory: n elements rented from <see cref="ArrayPool{T}"/>, or, above 4,194,304 elements, 4,194,304 · 4^k
/// elements for the smallest k that exceeds n / 8, as the reference allocates. The reference falls back to a 512-element stack array
/// when allocation fails. .NET reports that failure with an exception instead, so the fallback is not ported.</description></item>
/// <item><description>Buffers: 0 is the input, 1 is the swap memory, and 2 is the 32-element stack array that the quad swap and the
/// small-input <c>tail_swap</c> merge through, which the reference keeps on the stack as well.</description></item>
/// <item><description>Comparer contract: the reference trusts earlier comparisons to keep its merge cursors inside the runs. Element access
/// is unchecked here, so this port checks the cursors wherever only that trust protects them. A comparer that is not a consistent total
/// order can make the sort throw <see cref="InvalidOperationException"/>, and it can leave the elements in an unspecified state, with some
/// repeated and others lost. It cannot make the sort read or write outside its buffers.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Merge</description></item>
/// <item><description>Stable      : Yes (every merge takes the left element on ties, and only strictly descending stretches are reversed)</description></item>
/// <item><description>In-place    : No (n elements of swap memory, plus the 32-element stack array)</description></item>
/// <item><description>Best case   : Θ(n) - ordered or strictly descending input is recognised by the quad swap with about n comparisons; descending input is then reversed</description></item>
/// <item><description>Average case: Θ(n log n) - each quad merge level halves the number of runs twice</description></item>
/// <item><description>Worst case  : O(n log n) - the swap memory holds more than n / 8 elements, so the rotate merge levels recurse a bounded number of times before their pieces fit</description></item>
/// <item><description>Comparisons : Θ(n) best, Θ(n log n) average and worst - the quad swap spends 7 comparisons per eight ordered elements; a merge level makes about one comparison per element it merges, none for elements it copies in blocks of eight, plus the guard comparisons that look for such blocks</description></item>
/// <item><description>Swaps       : O(n) - exchanges reverse descending stretches, settle the middle pair of a four-element tiny sort, and finish trinity rotations; merges and the branchless pair sorts move elements by writes</description></item>
/// <item><description>Index Reads : Θ(n) best, Θ(n log n) average and worst - the quad swap reads every element, and merges and rotations read what they compare or move</description></item>
/// <item><description>Index Writes: O(n) best, Θ(n log n) average and worst - merges and rotations move elements, reversals exchange them, and a branchless step writes both of its candidates</description></item>
/// <item><description>Space       : O(n) - swap memory of n elements (more than n / 8 above 4,194,304 elements) plus a 32-element stack array</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>GitHub: https://github.com/scandum/quadsort (version 1.2.1.3)</para>
/// </remarks>
public static class QuadSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by QuadSortTests, which derives from StableSortTestsBase.</remarks>
    public static bool IsStable => true;

    // Buffer identifiers for visualization
    const int BUFFER_MAIN = 0;       // Main input array
    const int BUFFER_SWAP = 1;       // Swap memory (n elements, or less above SWAP_CAP)
    internal const int BUFFER_STACK = 2; // The 32-element stack array of quad_swap and the small-input tail_swap

    // Elements of the stack array the reference declares in quad_swap (VAR swap[32]).
    const int STACK_SIZE = 32;

    // quadsort sorts fewer than this many elements with tail_swap in its stack array.
    const int QUAD_SWAP_MIN = 32;

    // quadsort_swap sorts up to this many elements with tail_swap in the caller's swap memory.
    const int SWAP_TAIL_SWAP_MAX = 96;

    // Above this many elements the swap memory is capped (reference: 4194304 · 4^k with swap_size · 8 > nmemb).
    const int SWAP_CAP = 4194304;

    // trinity_rotation never uses more swap memory than this.
    const int ROTATION_SWAP_CAP = 65536;

    /// <summary>The reference's stack array <c>VAR swap[32]</c>.</summary>
    [InlineArray(STACK_SIZE)]
    internal struct StackSwap<TElement>
    {
        TElement element0;
    }

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

        if (last - first < 2) return;

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        SortCore(s, first, last - first);
    }

    /// <summary>
    /// Sorts the span with <paramref name="swapSize"/> elements of swap memory. With swap memory shorter than the runs, the final
    /// levels are rotate merges. A span of up to 96 elements is sorted by <c>tail_swap</c>, which needs as much swap memory as there
    /// are elements, so that much is provided whatever <paramref name="swapSize"/> says. Below 32 elements of swap memory the
    /// 32-element blocks could never be merged, so a smaller <paramref name="swapSize"/> is rejected.
    /// </summary>
    internal static void SortWithSwapSize<T, TComparer, TContext>(Span<T> span, int swapSize, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(swapSize, STACK_SIZE);
        if (span.Length < 2) return;

        if (span.Length <= SWAP_TAIL_SWAP_MAX && swapSize < span.Length)
        {
            swapSize = span.Length;
        }

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        var rented = ArrayPool<T>.Shared.Rent(swapSize);
        try
        {
            var w = new SortSpan<T, TComparer, TContext>(rented.AsSpan(0, swapSize), context, comparer, BUFFER_SWAP);
            SortSwap(s, 0, w, 0, swapSize, span.Length);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(rented, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    /// <summary>
    /// The reference's <c>quadsort</c>: sorts <paramref name="n"/> elements of <paramref name="a"/> starting at <paramref name="pa"/>,
    /// allocating the swap memory itself once the quad swap has found the input is not already sorted.
    /// </summary>
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (n < 2) return;

        if (n < QUAD_SWAP_MIN)
        {
            // The reference declares a stack array of n elements (VAR swap[nmemb]). Every n below 32 fits the 32-element array.
            var stackStorage = new StackSwap<T>();
            Span<T> stackSpan = stackStorage;
            var st = new SortSpan<T, TComparer, TContext>(stackSpan, a.Context, a.Comparer, BUFFER_STACK);
            a.Context.OnPhase(SortPhase.QuadSortTailSwap, a.Offset + pa, a.Offset + pa + n - 1);
            TailSwap(a, pa, st, 0, n);
            return;
        }

        if (QuadSwap(a, pa, n)) return;

        var swapSize = SwapSize(n);
        var rented = ArrayPool<T>.Shared.Rent(swapSize);
        try
        {
            var w = new SortSpan<T, TComparer, TContext>(rented.AsSpan(0, swapSize), a.Context, a.Comparer, BUFFER_SWAP);
            var block = QuadMerge(a, pa, w, 0, swapSize, n, 32);
            RotateMerge(a, pa, w, 0, swapSize, n, block);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(rented, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    /// <summary>
    /// Elements of swap memory quadsort allocates for <paramref name="n"/> elements: n itself up to 4,194,304, and above that
    /// 4,194,304 · 4^k for the smallest k whose result exceeds n / 8.
    /// </summary>
    internal static int SwapSize(int n)
    {
        if (n <= SWAP_CAP) return n;

        var swapSize = SWAP_CAP;
        while ((long)swapSize * 8 <= n) swapSize *= 4;
        return swapSize;
    }

    /// <summary>
    /// The reference's <c>quadsort_swap</c>: sorts <paramref name="n"/> elements of <paramref name="a"/> starting at <paramref name="pa"/>
    /// with the caller's swap memory <paramref name="w"/>[<paramref name="pw"/>..<paramref name="pw"/> + <paramref name="swapSize"/>).
    /// Up to 96 elements are sorted by <c>tail_swap</c>, which needs <paramref name="n"/> elements of swap memory.
    /// <paramref name="a"/> and <paramref name="w"/> may be the same buffer as long as the two ranges do not overlap.
    /// It is not inlined because FluxSort calls it from its recursive partition, whose frames must not hold element copies.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void SortSwap<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w, int pw, int swapSize, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (n <= SWAP_TAIL_SWAP_MAX)
        {
            if (n < 2) return;
            a.Context.OnPhase(SortPhase.QuadSortTailSwap, a.Offset + pa, a.Offset + pa + n - 1);
            TailSwap(a, pa, w, pw, n);
        }
        else if (!QuadSwap(a, pa, n))
        {
            var block = QuadMerge(a, pa, w, pw, swapSize, n, 32);
            RotateMerge(a, pa, w, pw, swapSize, n, block);
        }
    }

    // ------------------------------------------------------------------
    // Branchless steps
    // ------------------------------------------------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int Bit(bool value) => Unsafe.As<bool, byte>(ref value);

    /// <summary>
    /// <c>branchless_swap</c>: orders <paramref name="a"/>[i] and <paramref name="a"/>[i + 1]. Both positions are written whatever the
    /// comparison decided. Returns 1 when the pair was out of order.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int BranchlessSwap<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int i)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var gt = a.IsGreaterAt(i, i + 1, out var l, out var r);
        var x = Bit(gt);
        WritePair(a, i, x, l, r);
        return x;
    }

    /// <summary>
    /// Writes back a pair whose comparison was made earlier, exchanged when <paramref name="swap"/> is 1. Both positions are written
    /// whatever the flag. The flag picks the destination index rather than the value, because RyuJIT compiles a value selection
    /// inside a loop to a branch.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void WritePair<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int i, int swap, T l, T r)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        a.Write(i + swap, l);
        a.Write(i + (swap ^ 1), r);
    }

    /// <summary>
    /// <c>head_branchless_merge</c> in the reference's gcc form: writes src[l] to dst[d] and then src[r] to dst[d + x], where x is 1 when
    /// the left element is taken. The taken element ends up at dst[d]. When it is the left one, src[r] lands one position ahead, which a
    /// later step overwrites. Writing both instead of selecting one keeps the step free of data-dependent branches.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void HeadBranchless<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> dst, ref int d, SortSpan<T, TComparer, TContext> src, ref int l, ref int r)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var x = Bit(src.IsGreaterAt(l, r, out var lv, out var rv)) ^ 1;
        dst.Write(d, lv);
        dst.Write(d + x, rv);
        l += x;
        r += x ^ 1;
        d++;
    }

    /// <summary>
    /// <c>tail_branchless_merge</c> in the reference's gcc form: writes src[l] to dst[d] and then src[r] to dst[d - 1 + y], where y is 1
    /// when the right element is taken. The taken element ends up at dst[d]. When it is the left one, src[r] lands one position below,
    /// which a later step overwrites.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void TailBranchless<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> dst, ref int d, SortSpan<T, TComparer, TContext> src, ref int l, ref int r)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var y = Bit(src.IsGreaterAt(l, r, out var lv, out var rv)) ^ 1;
        dst.Write(d, lv);
        l -= y ^ 1;
        d--;
        dst.Write(d + y, rv);
        r -= y;
    }

    /// <summary><see cref="HeadBranchless"/> with the two runs in different buffers.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void HeadBranchlessAcross<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> dst, ref int d,
        SortSpan<T, TComparer, TContext> left, ref int l, SortSpan<T, TComparer, TContext> right, ref int r)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var x = Bit(left.IsGreaterAcross(l, right, r, out var lv, out var rv)) ^ 1;
        dst.Write(d, lv);
        dst.Write(d + x, rv);
        l += x;
        r += x ^ 1;
        d++;
    }

    /// <summary><see cref="TailBranchless"/> with the two runs in different buffers.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void TailBranchlessAcross<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> dst, ref int d,
        SortSpan<T, TComparer, TContext> left, ref int l, SortSpan<T, TComparer, TContext> right, ref int r)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var y = Bit(left.IsGreaterAcross(l, right, r, out var lv, out var rv)) ^ 1;
        dst.Write(d, lv);
        l -= y ^ 1;
        d--;
        dst.Write(d + y, rv);
        r -= y;
    }

    /// <summary>
    /// The reference's plain merge step, where it writes the ternary out instead of using the branchless macro:
    /// dst[d++] = src[l] &lt;= src[r] ? src[l++] : src[r++]. Writes one position, so it is safe at the end of an output range.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void HeadStep<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> dst, ref int d, SortSpan<T, TComparer, TContext> src, ref int l, ref int r)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var gt = src.IsGreaterAt(l, r, out var lv, out var rv);
        dst.Write(d++, gt ? rv : lv);
        var x = Bit(gt);
        r += x;
        l += x ^ 1;
    }

    /// <summary><see cref="HeadStep"/> with the two runs in different buffers.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void HeadStepAcross<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> dst, ref int d,
        SortSpan<T, TComparer, TContext> left, ref int l, SortSpan<T, TComparer, TContext> right, ref int r)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var gt = left.IsGreaterAcross(l, right, r, out var lv, out var rv);
        dst.Write(d++, gt ? rv : lv);
        var x = Bit(gt);
        r += x;
        l += x ^ 1;
    }

    /// <summary>
    /// The reference's plain backward merge step: dst[d--] = left[l] &gt; right[r] ? left[l--] : right[r--].
    /// Writes one position, so it is safe at the start of an output range.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void TailStepAcross<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> dst, ref int d,
        SortSpan<T, TComparer, TContext> left, ref int l, SortSpan<T, TComparer, TContext> right, ref int r)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var gt = left.IsGreaterAcross(l, right, r, out var lv, out var rv);
        dst.Write(d--, gt ? lv : rv);
        var x = Bit(gt);
        l -= x;
        r -= x ^ 1;
    }

    /// <summary>Reports a comparer whose answers would move a merge cursor out of its range.</summary>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    static void ThrowInconsistentComparer()
        => throw new InvalidOperationException("Comparison method violates its general contract.");

    /// <summary>The last step of a head merge: dst[d] = src[l] &lt;= src[r] ? src[l] : src[r].</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void HeadLast<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> dst, int d, SortSpan<T, TComparer, TContext> src, int l, int r)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var gt = src.IsGreaterAt(l, r, out var lv, out var rv);
        dst.Write(d, gt ? rv : lv);
    }

    /// <summary>The last step of a tail merge: dst[d] = src[l] &gt; src[r] ? src[l] : src[r].</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void TailLast<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> dst, int d, SortSpan<T, TComparer, TContext> src, int l, int r)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var gt = src.IsGreaterAt(l, r, out var lv, out var rv);
        dst.Write(d, gt ? lv : rv);
    }

    // ------------------------------------------------------------------
    // Sorting 0 to 31 elements
    // ------------------------------------------------------------------

    /// <summary><c>parity_swap_four</c>.</summary>
    static void ParitySwapFour<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int p)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        BranchlessSwap(a, p);
        BranchlessSwap(a, p + 2);

        if (a.IsGreaterAt(p + 1, p + 2))
        {
            a.Swap(p + 1, p + 2);

            BranchlessSwap(a, p);
            BranchlessSwap(a, p + 2);
            BranchlessSwap(a, p + 1);
        }
    }

    /// <summary><c>parity_swap_five</c>.</summary>
    static void ParitySwapFive<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int p)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        BranchlessSwap(a, p);
        BranchlessSwap(a, p + 2);
        var x = BranchlessSwap(a, p + 1);
        var y = BranchlessSwap(a, p + 3);

        if (x + y != 0)
        {
            BranchlessSwap(a, p);
            BranchlessSwap(a, p + 2);
            BranchlessSwap(a, p + 1);
            BranchlessSwap(a, p + 3);
            BranchlessSwap(a, p);
            BranchlessSwap(a, p + 2);
        }
    }

    /// <summary><c>parity_swap_six</c>: needs 6 elements of swap memory.</summary>
    static void ParitySwapSix<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int p, SortSpan<T, TComparer, TContext> w, int pw)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        BranchlessSwap(a, p);
        BranchlessSwap(a, p + 1);
        BranchlessSwap(a, p + 4);
        BranchlessSwap(a, p + 3);

        if (!a.IsGreaterAt(p + 2, p + 3))
        {
            BranchlessSwap(a, p);
            BranchlessSwap(a, p + 4);
            return;
        }

        var gt = a.IsGreaterAt(p, p + 1, out var l, out var r);
        WritePair(w, pw, Bit(gt), l, r);
        w.Write(pw + 2, a.Read(p + 2));

        gt = a.IsGreaterAt(p + 4, p + 5, out l, out r);
        WritePair(w, pw + 4, Bit(gt), l, r);
        w.Write(pw + 3, a.Read(p + 3));

        var d = p;
        var ptl = pw;
        var ptr = pw + 3;
        HeadBranchless(a, ref d, w, ref ptl, ref ptr);
        HeadBranchless(a, ref d, w, ref ptl, ref ptr);
        HeadBranchless(a, ref d, w, ref ptl, ref ptr);

        d = p + 5;
        ptl = pw + 2;
        ptr = pw + 5;
        TailBranchless(a, ref d, w, ref ptl, ref ptr);
        TailBranchless(a, ref d, w, ref ptl, ref ptr);
        TailLast(a, d, w, ptl, ptr);
    }

    /// <summary><c>parity_swap_seven</c>: needs 7 elements of swap memory.</summary>
    static void ParitySwapSeven<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int p, SortSpan<T, TComparer, TContext> w, int pw)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        BranchlessSwap(a, p);
        BranchlessSwap(a, p + 2);
        BranchlessSwap(a, p + 4);
        var y = BranchlessSwap(a, p + 1);
        y += BranchlessSwap(a, p + 3);
        y += BranchlessSwap(a, p + 5);

        if (y == 0) return;

        BranchlessSwap(a, p + 4);

        var gt = a.IsGreaterAt(p, p + 1, out var l, out var r);
        WritePair(w, pw, Bit(gt), l, r);
        w.Write(pw + 2, a.Read(p + 2));

        gt = a.IsGreaterAt(p + 3, p + 4, out l, out r);
        WritePair(w, pw + 3, Bit(gt), l, r);

        gt = a.IsGreaterAt(p + 5, p + 6, out l, out r);
        WritePair(w, pw + 5, Bit(gt), l, r);

        var d = p;
        var ptl = pw;
        var ptr = pw + 3;
        HeadBranchless(a, ref d, w, ref ptl, ref ptr);
        HeadBranchless(a, ref d, w, ref ptl, ref ptr);
        HeadBranchless(a, ref d, w, ref ptl, ref ptr);

        d = p + 6;
        ptl = pw + 2;
        ptr = pw + 6;
        TailBranchless(a, ref d, w, ref ptl, ref ptr);
        TailBranchless(a, ref d, w, ref ptl, ref ptr);
        TailBranchless(a, ref d, w, ref ptl, ref ptr);

        // Three tail steps can use up all three left elements only if the comparer contradicted itself.
        if (ptl < pw) ThrowInconsistentComparer();
        TailLast(a, d, w, ptl, ptr);
    }

    /// <summary><c>tiny_sort</c>: sorts 0 to 7 elements.</summary>
    static void TinySort<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int p, SortSpan<T, TComparer, TContext> w, int pw, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        switch (n)
        {
            case 2:
                BranchlessSwap(a, p);
                return;
            case 3:
                BranchlessSwap(a, p);
                BranchlessSwap(a, p + 1);
                BranchlessSwap(a, p);
                return;
            case 4:
                ParitySwapFour(a, p);
                return;
            case 5:
                ParitySwapFive(a, p);
                return;
            case 6:
                ParitySwapSix(a, p, w, pw);
                return;
            case 7:
                ParitySwapSeven(a, p, w, pw);
                return;
        }
    }

    /// <summary>
    /// <c>parity_merge</c>: merges src[f..f + left) and src[f + left..f + left + right) into dst[d..), working from the head and the
    /// tail of the output at once. <paramref name="left"/> must equal <paramref name="right"/> or be one smaller.
    /// </summary>
    static void ParityMerge<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> dst, int d, SortSpan<T, TComparer, TContext> src, int f, int left, int right)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var ptl = f;
        var ptr = f + left;
        var ptd = d;
        var tpl = ptr - 1;
        var tpr = tpl + right;
        var tpd = d + left + right - 1;

        if (left < right)
        {
            HeadStep(dst, ref ptd, src, ref ptl, ref ptr);
        }
        HeadStep(dst, ref ptd, src, ref ptl, ref ptr);

        while (--left != 0)
        {
            HeadBranchless(dst, ref ptd, src, ref ptl, ref ptr);
            TailBranchless(dst, ref tpd, src, ref tpl, ref tpr);
        }
        TailLast(dst, tpd, src, tpl, tpr);
    }

    /// <summary>
    /// <c>tail_swap</c>: sorts <paramref name="n"/> elements by sorting four quarters recursively and parity merging them through
    /// <paramref name="n"/> elements of swap memory. Below eight elements it is <c>tiny_sort</c>.
    /// </summary>
    static void TailSwap<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w, int pw, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (n < 8)
        {
            TinySort(a, pa, w, pw, n);
            return;
        }

        var half1 = n / 2;
        var quad1 = half1 / 2;
        var quad2 = half1 - quad1;
        var half2 = n - half1;
        var quad3 = half2 / 2;
        var quad4 = half2 - quad3;

        var pta = pa;
        TailSwap(a, pta, w, pw, quad1); pta += quad1;
        TailSwap(a, pta, w, pw, quad2); pta += quad2;
        TailSwap(a, pta, w, pw, quad3); pta += quad3;
        TailSwap(a, pta, w, pw, quad4);

        if (!a.IsGreaterAt(pa + quad1 - 1, pa + quad1) && !a.IsGreaterAt(pa + half1 - 1, pa + half1) && !a.IsGreaterAt(pta - 1, pta))
        {
            return;
        }
        ParityMerge(w, pw, a, pa, quad1, quad2);
        ParityMerge(w, pw + half1, a, pa + half1, quad3, quad4);
        ParityMerge(a, pa, w, pw, half1, half2);
    }

    // ------------------------------------------------------------------
    // Sorted blocks of 32
    // ------------------------------------------------------------------

    /// <summary>
    /// <c>quad_reversal</c>: reverses <paramref name="a"/>[<paramref name="pta"/>..<paramref name="ptz"/>] (both inclusive) by exchanges,
    /// working inwards from both ends and outwards from the middle at once. Needs at least three elements.
    /// </summary>
    internal static void QuadReversal<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pta, int ptz)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        Debug.Assert(ptz - pta >= 2);

        var loop = (ptz - pta) / 2;
        var ptb = pta + loop;
        var pty = ptz - loop;

        if (loop % 2 == 0)
        {
            a.Swap(ptb, pty);
            ptb--;
            pty++;
            loop--;
        }

        loop /= 2;

        do
        {
            a.Swap(pta, ptz);
            pta++;
            ptz--;
            a.Swap(ptb, pty);
            ptb--;
            pty++;
        }
        while (loop-- != 0);
    }

    /// <summary><c>parity_merge_two</c>: merges the sorted pairs src[ps..ps + 2) and src[ps + 2..ps + 4) into dst[pd..pd + 4).</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void ParityMergeTwo<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> src, int ps, SortSpan<T, TComparer, TContext> dst, int pd)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var ptl = ps;
        var ptr = ps + 2;
        var pts = pd;
        HeadBranchless(dst, ref pts, src, ref ptl, ref ptr);
        HeadLast(dst, pts, src, ptl, ptr);

        ptl = ps + 1;
        ptr = ps + 3;
        pts = pd + 3;
        TailBranchless(dst, ref pts, src, ref ptl, ref ptr);
        TailLast(dst, pts, src, ptl, ptr);
    }

    /// <summary><c>parity_merge_four</c>: merges the sorted runs src[ps..ps + 4) and src[ps + 4..ps + 8) into dst[pd..pd + 8).</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void ParityMergeFour<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> src, int ps, SortSpan<T, TComparer, TContext> dst, int pd)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var ptl = ps;
        var ptr = ps + 4;
        var pts = pd;
        HeadBranchless(dst, ref pts, src, ref ptl, ref ptr);
        HeadBranchless(dst, ref pts, src, ref ptl, ref ptr);
        HeadBranchless(dst, ref pts, src, ref ptl, ref ptr);
        HeadLast(dst, pts, src, ptl, ptr);

        ptl = ps + 3;
        ptr = ps + 7;
        pts = pd + 7;
        TailBranchless(dst, ref pts, src, ref ptl, ref ptr);
        TailBranchless(dst, ref pts, src, ref ptl, ref ptr);
        TailBranchless(dst, ref pts, src, ref ptl, ref ptr);
        TailLast(dst, pts, src, ptl, ptr);
    }

    /// <summary><c>quad_swap_merge</c>: sorts eight elements whose four pairs are already in order.</summary>
    static void QuadSwapMerge<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int p, SortSpan<T, TComparer, TContext> w, int pw)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        ParityMergeTwo(a, p, w, pw);
        ParityMergeTwo(a, p + 4, w, pw + 4);
        ParityMergeFour(w, pw, a, p);
    }

    /// <summary>
    /// <c>quad_swap</c>: turns <paramref name="a"/>[<paramref name="pa"/>..<paramref name="pa"/> + <paramref name="n"/>) into sorted blocks
    /// of 32 (the last block shorter), reversing descending stretches in place. Returns true when the whole range was one descending
    /// stretch, which leaves it sorted.
    /// </summary>
    /// <remarks>
    /// The reference jumps between the ordered, reversed and unordered states with <c>goto</c>, including into the middle of a
    /// <c>switch</c>. The labels below are the same states, lifted to the method's top level where C# allows jumps to them.
    /// </remarks>
    static bool QuadSwap<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        a.Context.OnPhase(SortPhase.QuadSortQuadSwap, a.Offset + pa, a.Offset + pa + n - 1);

        var stackStorage = new StackSwap<T>();
        Span<T> stackSpan = stackStorage;
        var st = new SortSpan<T, TComparer, TContext>(stackSpan, a.Context, a.Comparer, BUFFER_STACK);

        var pta = pa;
        var pts = pa;
        var count = n / 8;
        bool v1, v2, v3, v4;
        T a1, b1, a2, b2, a3, b3, a4, b4;

    LoopHead:
        if (count-- == 0) goto LoopEnd;

        v1 = a.IsGreaterAt(pta, pta + 1, out a1, out b1);
        v2 = a.IsGreaterAt(pta + 2, pta + 3, out a2, out b2);
        v3 = a.IsGreaterAt(pta + 4, pta + 5, out a3, out b3);
        v4 = a.IsGreaterAt(pta + 6, pta + 7, out a4, out b4);

        switch (Bit(v1) + Bit(v2) * 2 + Bit(v3) * 4 + Bit(v4) * 8)
        {
            case 0:
                if (!a.IsGreaterAt(pta + 1, pta + 2) && !a.IsGreaterAt(pta + 3, pta + 4) && !a.IsGreaterAt(pta + 5, pta + 6))
                {
                    goto Ordered;
                }
                QuadSwapMerge(a, pta, st, 0);
                pta += 8;
                goto LoopHead;

            case 15:
                if (a.IsGreaterAt(pta + 1, pta + 2) && a.IsGreaterAt(pta + 3, pta + 4) && a.IsGreaterAt(pta + 5, pta + 6))
                {
                    pts = pta;
                    goto Reversed;
                }
                goto NotOrdered;

            default:
                goto NotOrdered;
        }

    NotOrdered:
        WritePair(a, pta, Bit(v1), a1, b1);
        WritePair(a, pta + 2, Bit(v2), a2, b2);
        WritePair(a, pta + 4, Bit(v3), a3, b3);
        WritePair(a, pta + 6, Bit(v4), a4, b4);
        QuadSwapMerge(a, pta, st, 0);
        pta += 8;
        goto LoopHead;

    Ordered:
        pta += 8;

        if (count-- != 0)
        {
            // All four pairs are compared before any is acted on (the reference combines them with a bitwise |).
            v1 = a.IsGreaterAt(pta, pta + 1, out a1, out b1);
            v2 = a.IsGreaterAt(pta + 2, pta + 3, out a2, out b2);
            v3 = a.IsGreaterAt(pta + 4, pta + 5, out a3, out b3);
            v4 = a.IsGreaterAt(pta + 6, pta + 7, out a4, out b4);

            if (v1 | v2 | v3 | v4)
            {
                if (v1 & v2 & v3 & v4 && a.IsGreaterAt(pta + 1, pta + 2) && a.IsGreaterAt(pta + 3, pta + 4) && a.IsGreaterAt(pta + 5, pta + 6))
                {
                    pts = pta;
                    goto Reversed;
                }
                goto NotOrdered;
            }
            if (!a.IsGreaterAt(pta + 1, pta + 2) && !a.IsGreaterAt(pta + 3, pta + 4) && !a.IsGreaterAt(pta + 5, pta + 6))
            {
                goto Ordered;
            }
            QuadSwapMerge(a, pta, st, 0);
            pta += 8;
            goto LoopHead;
        }
        goto LoopEnd;

    Reversed:
        pta += 8;

        if (count-- != 0)
        {
            // Here the flags mean "pair in order" (cmp <= 0).
            v1 = !a.IsGreaterAt(pta, pta + 1, out a1, out b1);
            v2 = !a.IsGreaterAt(pta + 2, pta + 3, out a2, out b2);
            v3 = !a.IsGreaterAt(pta + 4, pta + 5, out a3, out b3);
            v4 = !a.IsGreaterAt(pta + 6, pta + 7, out a4, out b4);

            if (!(v1 | v2 | v3 | v4))
            {
                if (a.IsGreaterAt(pta - 1, pta) && a.IsGreaterAt(pta + 1, pta + 2) && a.IsGreaterAt(pta + 3, pta + 4) && a.IsGreaterAt(pta + 5, pta + 6))
                {
                    goto Reversed;
                }
            }
            QuadReversal(a, pts, pta - 1);

            if (v1 & v2 & v3 & v4 && !a.IsGreaterAt(pta + 1, pta + 2) && !a.IsGreaterAt(pta + 3, pta + 4) && !a.IsGreaterAt(pta + 5, pta + 6))
            {
                goto Ordered;
            }
            if (!(v1 | v2 | v3 | v4) && a.IsGreaterAt(pta + 1, pta + 2) && a.IsGreaterAt(pta + 3, pta + 4) && a.IsGreaterAt(pta + 5, pta + 6))
            {
                pts = pta;
                goto Reversed;
            }

            WritePair(a, pta, Bit(v1) ^ 1, a1, b1);
            WritePair(a, pta + 2, Bit(v2) ^ 1, a2, b2);
            WritePair(a, pta + 4, Bit(v3) ^ 1, a3, b3);
            WritePair(a, pta + 6, Bit(v4) ^ 1, a4, b4);

            if (a.IsGreaterAt(pta + 1, pta + 2) || a.IsGreaterAt(pta + 3, pta + 4) || a.IsGreaterAt(pta + 5, pta + 6))
            {
                QuadSwapMerge(a, pta, st, 0);
            }
            pta += 8;
            goto LoopHead;
        }

        {
            // The descending stretch reached the last full group. It takes in the remainder only if all of the remainder keeps
            // descending (the reference's fall-through switch on nmemb % 8, checked from the remainder's end backwards).
            var rem = n % 8;
            for (var k = rem - 1; k >= 0; k--)
            {
                if (!a.IsGreaterAt(pta + k - 1, pta + k)) goto PartialReversal;
            }
            QuadReversal(a, pts, pta + rem - 1);

            if (pts == pa) return true;
            goto ReverseEnd;
        }

    PartialReversal:
        QuadReversal(a, pts, pta - 1);

    LoopEnd:
        TailSwap(a, pta, st, 0, n % 8);

    ReverseEnd:
        pta = pa;

        for (count = n / 32; count-- != 0; pta += 32)
        {
            if (!a.IsGreaterAt(pta + 7, pta + 8) && !a.IsGreaterAt(pta + 15, pta + 16) && !a.IsGreaterAt(pta + 23, pta + 24))
            {
                continue;
            }
            ParityMerge(st, 0, a, pta, 8, 8);
            ParityMerge(st, 16, a, pta + 16, 8, 8);
            ParityMerge(a, pta, st, 0, 16, 16);
        }

        if (n % 32 > 8)
        {
            TailMerge(a, pta, st, 0, STACK_SIZE, n % 32, 8);
        }
        return false;
    }

    // ------------------------------------------------------------------
    // Quad merge support
    // ------------------------------------------------------------------

    /// <summary>
    /// <c>cross_merge</c>: merges src[ps..ps + left) and src[ps + left..ps + left + right) into dst[pd..). Eight elements at a time are
    /// copied from whichever end of a run lies entirely before (or after) the other run's next element. The rest is merged branchlessly
    /// from the head and the tail at once. Two runs of nearly equal length that interleave at both ends go to <see cref="ParityMerge"/>.
    /// </summary>
    internal static void CrossMerge<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> dst, int pd, SortSpan<T, TComparer, TContext> src, int ps, int left, int right)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var ptl = ps;
        var ptr = ps + left;
        var tpl = ptr - 1;
        var tpr = tpl + right;

        if (left + 1 >= right && right >= left && left >= 32)
        {
            if (src.IsGreaterAt(ptl + 15, ptr) && !src.IsGreaterAt(ptl, ptr + 15) && src.IsGreaterAt(tpl, tpr - 15) && !src.IsGreaterAt(tpl - 15, tpr))
            {
                ParityMerge(dst, pd, src, ps, left, right);
                return;
            }
        }

        var ptd = pd;
        var tpd = pd + left + right - 1;
        var end = ps + left + right;

        while (true)
        {
            if (tpl - ptl > 8)
            {
                // Only a consistent comparer keeps the right run from being overdrawn. An overdrawn run would send the block copies
                // below outside the output, and a cursor at the end of the range would make the comparisons read past it.
                if (ptr > tpr + 1 || ptr >= end) ThrowInconsistentComparer();

            Ptl8Ptr:
                if (!src.IsGreaterAt(ptl + 7, ptr))
                {
                    src.CopyTo(ptl, dst, ptd, 8);
                    ptd += 8;
                    ptl += 8;

                    if (tpl - ptl > 8) goto Ptl8Ptr;
                    continue;
                }

            Tpl8Tpr:
                if (src.IsGreaterAt(tpl - 7, tpr))
                {
                    tpd -= 7;
                    tpl -= 7;
                    src.CopyTo(tpl, dst, tpd, 8);
                    tpd--;
                    tpl--;

                    if (tpl - ptl > 8) goto Tpl8Tpr;
                    continue;
                }
            }

            if (tpr - ptr > 8)
            {
                // The same goes for the left run.
                if (ptl > tpl + 1 || tpl < ps) ThrowInconsistentComparer();

            PtlPtr8:
                if (src.IsGreaterAt(ptl, ptr + 7))
                {
                    src.CopyTo(ptr, dst, ptd, 8);
                    ptd += 8;
                    ptr += 8;

                    if (tpr - ptr > 8) goto PtlPtr8;
                    continue;
                }

            TplTpr8:
                if (!src.IsGreaterAt(tpl, tpr - 7))
                {
                    tpd -= 7;
                    tpr -= 7;
                    src.CopyTo(tpr, dst, tpd, 8);
                    tpd--;
                    tpr--;

                    if (tpr - ptr > 8) goto TplTpr8;
                    continue;
                }
            }

            if (tpd - ptd < 16) break;

            // A cursor moves at most eight places in a batch, so only a batch near the ends of the range has to check its reads.
            // The output still to fill keeps ptl and tpr inside the range, which leaves ptr and tpl to check.
            if (ptr <= end - 8 && tpl >= ps + 7)
            {
                for (var loop = 8; loop != 0; loop--)
                {
                    HeadBranchless(dst, ref ptd, src, ref ptl, ref ptr);
                    TailBranchless(dst, ref tpd, src, ref tpl, ref tpr);
                }
            }
            else
            {
                for (var loop = 8; loop != 0; loop--)
                {
                    if (ptr >= end || tpl < ps) ThrowInconsistentComparer();
                    HeadBranchless(dst, ref ptd, src, ref ptl, ref ptr);
                    TailBranchless(dst, ref tpd, src, ref tpl, ref tpr);
                }
            }
        }

        while (ptl <= tpl && ptr <= tpr)
        {
            HeadStep(dst, ref ptd, src, ref ptl, ref ptr);
        }
        if (ptl <= tpl)
        {
            src.CopyTo(ptl, dst, ptd, tpl - ptl + 1);
            ptd += tpl - ptl + 1;
        }
        if (ptr <= tpr)
        {
            src.CopyTo(ptr, dst, ptd, tpr - ptr + 1);
        }
    }

    /// <summary>
    /// <c>quad_merge_block</c>: merges four sorted blocks of <paramref name="block"/> elements at <paramref name="pa"/> into one,
    /// pairwise into swap memory and then back. A pair whose boundary is in order is copied instead of merged.
    /// </summary>
    static void QuadMergeBlock<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w, int pw, int block)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var blockX2 = block * 2;
        var pt1 = pa + block;
        var pt2 = pt1 + block;
        var pt3 = pt2 + block;

        var firstInOrder = !a.IsGreaterAt(pt1 - 1, pt1);
        var secondInOrder = !a.IsGreaterAt(pt3 - 1, pt3);

        switch (Bit(firstInOrder) | Bit(secondInOrder) * 2)
        {
            case 0:
                CrossMerge(w, pw, a, pa, block, block);
                CrossMerge(w, pw + blockX2, a, pt2, block, block);
                break;
            case 1:
                a.CopyTo(pa, w, pw, blockX2);
                CrossMerge(w, pw + blockX2, a, pt2, block, block);
                break;
            case 2:
                CrossMerge(w, pw, a, pa, block, block);
                a.CopyTo(pt2, w, pw + blockX2, blockX2);
                break;
            case 3:
                if (!a.IsGreaterAt(pt2 - 1, pt2)) return;
                a.CopyTo(pa, w, pw, blockX2 * 2);
                break;
        }
        CrossMerge(a, pa, w, pw, blockX2, blockX2);
    }

    /// <summary>
    /// <c>quad_merge</c>: merges sorted blocks of <paramref name="block"/> elements four at a time for as long as four blocks fit in
    /// both the range and swap memory, tail merging whatever is left at each level. Returns the block size the rotate merge starts from.
    /// </summary>
    static int QuadMerge<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w, int pw, int swapSize, int n, int block)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var pte = pa + n;
        long blk = (long)block * 4;

        while (blk <= n && blk <= swapSize)
        {
            var quarter = (int)(blk / 4);
            a.Context.OnPhase(SortPhase.QuadSortQuadMerge, a.Offset + pa, a.Offset + pa + n - 1, quarter);

            var pta = pa;
            do
            {
                QuadMergeBlock(a, pta, w, pw, quarter);
                pta += (int)blk;
            }
            while (pte - pta >= blk);

            TailMerge(a, pta, w, pw, swapSize, pte - pta, quarter);

            blk *= 4;
        }

        TailMerge(a, pa, w, pw, swapSize, n, (int)(blk / 4));

        return (int)(blk / 2);
    }

    /// <summary>
    /// <c>partial_forward_merge</c>: merges a[pa..pa + block) with a[pa + block..pa + n) by copying the left run to swap memory and
    /// merging forwards. Two elements at a time are taken from a run while its next two come before the other run's next element.
    /// </summary>
    static void PartialForwardMerge<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w, int pw, int n, int block)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (n == block) return;

        var ptr = pa + block;
        var tpr = pa + n - 1;

        if (!a.IsGreaterAt(ptr - 1, ptr)) return;

        a.CopyTo(pa, w, pw, block);

        var ptl = pw;
        var tpl = pw + block - 1;
        var pd = pa;

        while (ptl < tpl - 1 && ptr < tpr - 1)
        {
        Ptr2:
            if (w.IsGreaterAcross(ptl, a, ptr + 1))
            {
                a.Write(pd++, a.Read(ptr++));
                a.Write(pd++, a.Read(ptr++));

                if (ptr < tpr - 1) goto Ptr2;
                break;
            }
            if (!w.IsGreaterAcross(ptl + 1, a, ptr))
            {
                a.Write(pd++, w.Read(ptl++));
                a.Write(pd++, w.Read(ptl++));

                if (ptl < tpl - 1) goto Ptl2;
                break;
            }

            goto CrossSwap;

        Ptl2:
            if (!w.IsGreaterAcross(ptl + 1, a, ptr))
            {
                a.Write(pd++, w.Read(ptl++));
                a.Write(pd++, w.Read(ptl++));

                if (ptl < tpl - 1) goto Ptl2;
                break;
            }

            if (w.IsGreaterAcross(ptl, a, ptr + 1))
            {
                a.Write(pd++, a.Read(ptr++));
                a.Write(pd++, a.Read(ptr++));

                if (ptr < tpr - 1) goto Ptr2;
                break;
            }

        CrossSwap:
            {
                // The two next elements go out as a pair in either order, then one more merge step.
                var gt = w.IsGreaterAcross(ptl, a, ptr, out var lv, out var rv);
                var x = Bit(gt) ^ 1;
                a.Write(pd + x, rv);
                ptr++;
                a.Write(pd + (x ^ 1), lv);
                ptl++;
                pd += 2;
                HeadBranchlessAcross(a, ref pd, w, ref ptl, a, ref ptr);
            }
        }

        while (ptl <= tpl && ptr <= tpr)
        {
            HeadStepAcross(a, ref pd, w, ref ptl, a, ref ptr);
        }

        if (ptl <= tpl)
        {
            w.CopyTo(ptl, a, pd, tpl - ptl + 1);
        }
    }

    /// <summary>
    /// <c>partial_backward_merge</c>: merges a[pa..pa + block) with a[pa + block..pa + n) by copying the right run to swap memory and
    /// merging backwards. When the whole range fits in swap memory and the right run is long, it cross merges into swap memory and
    /// copies back instead. Sixteen, then two elements at a time are taken from a run while they come after the other run's next element.
    /// </summary>
    static void PartialBackwardMerge<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w, int pw, int swapSize, int n, int block)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (n == block) return;

        var tpl = pa + block - 1;
        var tpa = pa + n - 1;

        if (!a.IsGreaterAt(tpl, tpl + 1)) return;

        var right = n - block;

        if (n <= swapSize && right >= 64)
        {
            CrossMerge(w, pw, a, pa, block, right);
            w.CopyTo(pw, a, pa, n);
            return;
        }

        a.CopyTo(pa + block, w, pw, right);

        var tpr = pw + right - 1;

        while (tpl > pa + 16 && tpr > pw + 16)
        {
        TplTpr16:
            if (!a.IsGreaterAcross(tpl, w, tpr - 15))
            {
                w.CopyTo(tpr - 15, a, tpa - 15, 16);
                tpa -= 16;
                tpr -= 16;

                if (tpr > pw + 16) goto TplTpr16;
                break;
            }

        Tpl16Tpr:
            if (a.IsGreaterAcross(tpl - 15, w, tpr))
            {
                a.CopyTo(tpl - 15, a, tpa - 15, 16);
                tpa -= 16;
                tpl -= 16;

                if (tpl > pa + 16) goto Tpl16Tpr;
                break;
            }

            for (var loop = 8; loop != 0; loop--)
            {
                if (!a.IsGreaterAcross(tpl, w, tpr - 1))
                {
                    a.Write(tpa--, w.Read(tpr--));
                    a.Write(tpa--, w.Read(tpr--));
                }
                else if (a.IsGreaterAcross(tpl - 1, w, tpr))
                {
                    a.Write(tpa--, a.Read(tpl--));
                    a.Write(tpa--, a.Read(tpl--));
                }
                else
                {
                    BackwardCrossSwap(a, ref tpa, ref tpl, w, ref tpr);
                }
            }
        }

        while (tpr > pw + 1 && tpl > pa + 1)
        {
        Tpr2:
            if (!a.IsGreaterAcross(tpl, w, tpr - 1))
            {
                a.Write(tpa--, w.Read(tpr--));
                a.Write(tpa--, w.Read(tpr--));

                if (tpr > pw + 1) goto Tpr2;
                break;
            }

            if (a.IsGreaterAcross(tpl - 1, w, tpr))
            {
                a.Write(tpa--, a.Read(tpl--));
                a.Write(tpa--, a.Read(tpl--));

                if (tpl > pa + 1) goto Tpl2;
                break;
            }
            goto CrossSwap;

        Tpl2:
            if (a.IsGreaterAcross(tpl - 1, w, tpr))
            {
                a.Write(tpa--, a.Read(tpl--));
                a.Write(tpa--, a.Read(tpl--));

                if (tpl > pa + 1) goto Tpl2;
                break;
            }

            if (!a.IsGreaterAcross(tpl, w, tpr - 1))
            {
                a.Write(tpa--, w.Read(tpr--));
                a.Write(tpa--, w.Read(tpr--));

                if (tpr > pw + 1) goto Tpr2;
                break;
            }

        CrossSwap:
            BackwardCrossSwap(a, ref tpa, ref tpl, w, ref tpr);
        }

        while (tpr >= pw && tpl >= pa)
        {
            TailStepAcross(a, ref tpa, a, ref tpl, w, ref tpr);
        }

        if (tpr >= pw)
        {
            w.CopyTo(pw, a, tpa - (tpr - pw), tpr - pw + 1);
        }
    }

    /// <summary>
    /// The backward merge's cross swap: the last elements of both runs go out as a pair in either order, then one more merge step.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void BackwardCrossSwap<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, ref int tpa, ref int tpl, SortSpan<T, TComparer, TContext> w, ref int tpr)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var gt = a.IsGreaterAcross(tpl, w, tpr, out var lv, out var rv);
        var x = Bit(gt) ^ 1;
        tpa--;
        a.Write(tpa + x, rv);
        tpr--;
        a.Write(tpa + (x ^ 1), lv);
        tpl--;
        tpa--;
        TailBranchlessAcross(a, ref tpa, a, ref tpl, w, ref tpr);
    }

    /// <summary>
    /// <c>tail_merge</c>: merges runs of <paramref name="block"/> elements pairwise, doubling the run length, until one run covers
    /// <paramref name="n"/> elements or the runs outgrow swap memory.
    /// </summary>
    static void TailMerge<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w, int pw, int swapSize, int n, int block)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var pte = pa + n;
        long blk = block;

        while (blk < n && blk <= swapSize)
        {
            var b = (int)blk;
            a.Context.OnPhase(SortPhase.QuadSortTailMerge, a.Offset + pa, a.Offset + pa + n - 1, b);

            for (var pta = pa; pte - pta > b; pta += b * 2)
            {
                if (pte - pta - b > b)
                {
                    PartialBackwardMerge(a, pta, w, pw, swapSize, b * 2, b);
                    continue;
                }
                PartialBackwardMerge(a, pta, w, pw, swapSize, pte - pta, b);
                break;
            }
            blk *= 2;
        }
    }

    // ------------------------------------------------------------------
    // In-place rotate merge support
    // ------------------------------------------------------------------

    /// <summary>
    /// <c>trinity_rotation</c>: exchanges a[pa..pa + left) with a[pa + left..pa + n). The shorter side goes through swap memory
    /// when it fits. Otherwise the difference between the sides does (a bridge rotation), and failing that the rotation is done
    /// in place by four-way, three-way and two-way cycles from both ends.
    /// </summary>
    internal static void TrinityRotation<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w, int pw, int swapSize, int n, int left)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var right = n - left;

        if (swapSize > ROTATION_SWAP_CAP)
        {
            swapSize = ROTATION_SWAP_CAP;
        }

        if (left < right)
        {
            if (left <= swapSize)
            {
                a.CopyTo(pa, w, pw, left);
                a.CopyTo(pa + left, a, pa, right);
                w.CopyTo(pw, a, pa + right, left);
            }
            else
            {
                var pta = pa;
                var ptb = pa + left;
                var bridge = right - left;

                if (bridge <= swapSize && bridge > 3)
                {
                    var ptc = pa + right;
                    var ptd = ptc + left;

                    a.CopyTo(ptb, w, pw, bridge);

                    while (left-- != 0)
                    {
                        --ptc;
                        --ptd;
                        a.Write(ptc, a.Read(ptd));
                        --ptb;
                        a.Write(ptd, a.Read(ptb));
                    }
                    w.CopyTo(pw, a, pta, bridge);
                }
                else
                {
                    var ptc = ptb;
                    var ptd = ptc + right;

                    bridge = left / 2;

                    while (bridge-- != 0)
                    {
                        FourWayCycle(a, ref pta, ref ptb, ref ptc, ref ptd);
                    }

                    bridge = (ptd - ptc) / 2;

                    while (bridge-- != 0)
                    {
                        var temp = a.Read(ptc);
                        a.Write(ptc++, a.Read(--ptd));
                        a.Write(ptd, a.Read(pta));
                        a.Write(pta++, temp);
                    }

                    bridge = (ptd - pta) / 2;

                    while (bridge-- != 0)
                    {
                        a.Swap(pta++, --ptd);
                    }
                }
            }
        }
        else if (right < left)
        {
            if (right <= swapSize)
            {
                a.CopyTo(pa + left, w, pw, right);
                a.CopyTo(pa, a, pa + right, left);
                w.CopyTo(pw, a, pa, right);
            }
            else
            {
                var pta = pa;
                var ptb = pa + left;
                var bridge = left - right;

                if (bridge <= swapSize && bridge > 3)
                {
                    var ptc = pa + right;
                    var ptd = ptc + left;

                    a.CopyTo(ptc, w, pw, bridge);

                    while (right-- != 0)
                    {
                        a.Write(ptc++, a.Read(pta));
                        a.Write(pta++, a.Read(ptb++));
                    }
                    w.CopyTo(pw, a, ptd - bridge, bridge);
                }
                else
                {
                    var ptc = ptb;
                    var ptd = ptc + right;

                    bridge = right / 2;

                    while (bridge-- != 0)
                    {
                        FourWayCycle(a, ref pta, ref ptb, ref ptc, ref ptd);
                    }

                    bridge = (ptb - pta) / 2;

                    while (bridge-- != 0)
                    {
                        var temp = a.Read(--ptb);
                        a.Write(ptb, a.Read(pta));
                        a.Write(pta++, a.Read(--ptd));
                        a.Write(ptd, temp);
                    }

                    bridge = (ptd - pta) / 2;

                    while (bridge-- != 0)
                    {
                        a.Swap(pta++, --ptd);
                    }
                }
            }
        }
        else
        {
            var pta = pa;
            var ptb = pa + left;

            while (left-- != 0)
            {
                a.Swap(pta++, ptb++);
            }
        }
    }

    /// <summary>One step of the in-place rotation: four elements, one under each cursor, move one place around a cycle.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void FourWayCycle<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, ref int pta, ref int ptb, ref int ptc, ref int ptd)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var temp = a.Read(--ptb);
        a.Write(ptb, a.Read(pta));
        a.Write(pta++, a.Read(ptc));
        a.Write(ptc++, a.Read(--ptd));
        a.Write(ptd, temp);
    }

    /// <summary>
    /// <c>monobound_binary_first</c>: the number of elements of a[parr..parr + top) that are strictly less than a[pvalue], found by a
    /// binary search whose iteration count depends only on <paramref name="top"/>.
    /// </summary>
    static int MonoboundBinaryFirst<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int parr, int pvalue, int top)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var end = parr + top;

        while (top > 1)
        {
            var mid = top / 2;

            if (!a.IsGreaterAt(pvalue, end - mid))
            {
                end -= mid;
            }
            top -= mid;
        }

        if (!a.IsGreaterAt(pvalue, end - 1))
        {
            end--;
        }
        return end - parr;
    }

    /// <summary>
    /// <c>rotate_merge_block</c>: merges a[pa..pa + lblock) with the following <paramref name="right"/> elements in place. The left run is
    /// split in half, the right run is split where the left run's upper half begins, the middle two pieces are exchanged by a rotation,
    /// and both resulting pairs of runs are merged the same way until they fit in swap memory.
    /// </summary>
    internal static void RotateMergeBlock<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w, int pw, int swapSize, int lblock, int right)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (!a.IsGreaterAt(pa + lblock - 1, pa + lblock)) return;

        var rblock = lblock / 2;
        lblock -= rblock;

        var left = MonoboundBinaryFirst(a, pa + lblock + rblock, pa + lblock, right);

        right -= left;

        // [ lblock ] [ rblock ] [ left ] [ right ]

        if (left != 0)
        {
            if ((long)lblock + left <= swapSize)
            {
                a.CopyTo(pa, w, pw, lblock);
                a.CopyTo(pa + lblock + rblock, w, pw + lblock, left);
                a.CopyTo(pa + lblock, a, pa + lblock + left, rblock);

                CrossMerge(a, pa, w, pw, lblock, left);
            }
            else
            {
                TrinityRotation(a, pa + lblock, w, pw, swapSize, rblock + left, rblock);

                var unbalanced = ((long)left * 2 < lblock) | ((long)lblock * 2 < left);

                if (unbalanced && left <= swapSize)
                {
                    PartialBackwardMerge(a, pa, w, pw, swapSize, lblock + left, lblock);
                }
                else if (unbalanced && lblock <= swapSize)
                {
                    PartialForwardMerge(a, pa, w, pw, lblock + left, lblock);
                }
                else
                {
                    RotateMergeBlock(a, pa, w, pw, swapSize, lblock, left);
                }
            }
        }

        if (right != 0)
        {
            var unbalanced = ((long)right * 2 < rblock) | ((long)rblock * 2 < right);

            if ((unbalanced && right <= swapSize) || (long)right + rblock <= swapSize)
            {
                PartialBackwardMerge(a, pa + lblock + left, w, pw, swapSize, rblock + right, rblock);
            }
            else if (unbalanced && rblock <= swapSize)
            {
                PartialForwardMerge(a, pa + lblock + left, w, pw, rblock + right, rblock);
            }
            else
            {
                RotateMergeBlock(a, pa + lblock + left, w, pw, swapSize, rblock, right);
            }
        }
    }

    /// <summary>
    /// <c>rotate_merge</c>: merges runs of <paramref name="block"/> elements pairwise in place, doubling the run length until one run
    /// covers the range. When a single merge of a right run that fits in swap memory is all that is left, it is a backward merge.
    /// </summary>
    static void RotateMerge<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w, int pw, int swapSize, int n, int block)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var pte = pa + n;

        // The reference computes nmemb - block in size_t, so a block longer than the range never takes this branch.
        if (n <= (long)block * 2 && n >= block && n - block <= swapSize)
        {
            // This is one backward merge, the work of a tail merge. With full swap memory it only finds the runs already merged.
            if (n > block) a.Context.OnPhase(SortPhase.QuadSortTailMerge, a.Offset + pa, a.Offset + pa + n - 1, block);
            PartialBackwardMerge(a, pa, w, pw, swapSize, n, block);
            return;
        }

        long blk = block;

        while (blk < n)
        {
            var b = (int)blk;
            a.Context.OnPhase(SortPhase.QuadSortRotateMerge, a.Offset + pa, a.Offset + pa + n - 1, b);

            for (var pta = pa; pte - pta > b; pta += b * 2)
            {
                if (pte - pta - b > b)
                {
                    RotateMergeBlock(a, pta, w, pw, swapSize, b, b);
                    continue;
                }
                RotateMergeBlock(a, pta, w, pw, swapSize, b, pte - pta - b);
                break;
            }
            blk *= 2;
        }
    }
}
