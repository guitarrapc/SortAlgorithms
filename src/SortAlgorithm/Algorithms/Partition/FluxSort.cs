using SortAlgorithm.Contexts;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 先に入力を 4 分割して並び具合を調べ、整列済み・逆順の区間を見つけた部分は quadsort、それ以外はピボット以下を元配列の先頭へ、
/// ピボットより大きい要素を補助領域へ走査順のまま書き分ける安定な分割で並べる、安定なクイックソートとマージソートの混成ソートです（van den Hoven, 2021）。
/// 直前と同じピボットを選んだときはピボットと等しい要素をまとめて取り除き、偏った分割や短い区間は quadsort に任せます。
/// <br/>
/// A stable hybrid of quicksort and mergesort (van den Hoven, 2021). An up-front analysis of the input's four quarters finds ordered,
/// reversed and streaky stretches and hands those to quadsort; everything else is partitioned stably: one pass copies the elements not
/// greater than the pivot to the front of the main array and the greater ones to swap memory, both in scan order. Choosing the same
/// pivot twice in a row separates the elements equal to it in one more pass, and unbalanced or short partitions go to quadsort.
/// </summary>
/// <remarks>
/// <para><strong>Algorithm:</strong></para>
/// <list type="number">
/// <item><description><strong>Small input:</strong> up to 132 elements are sorted by quadsort.</description></item>
/// <item><description><strong>Analysis:</strong> each quarter's adjacent pairs are counted as descending or not, 32 pairs at a time;
/// a block of 32 that is entirely one or the other is a streak. When none of the four quarters shows a streak, the next 96 pairs of
/// each are skipped and assumed half descending. Input with no descending pair and ordered quarter boundaries is already sorted.
/// Quarters that descend strictly throughout are reversed, together with a neighbour when the boundary between them descends too.</description></item>
/// <item><description><strong>Dispatch:</strong> a quarter with more streaks than n / 256 is sorted by quadsort (skipped when it has no
/// descending pair); adjacent non-streaky quarters are partitioned together as one range. The sorted quarters are then cross merged
/// into swap memory pairwise and back as one, skipping the merges whose boundaries are already in order.</description></item>
/// <item><description><strong>Pivot:</strong> the pseudomedian of nine for up to 2,048 elements. Above that, an evenly spaced sample of
/// 32 · 2^k elements (the smallest such that the sample's cube is at least the range) is copied out, its halves are sorted with quadsort,
/// and the median of the two is found by binary search. A sample whose first half is constant and whose second half does not exceed
/// it marks low-cardinality data, and the range is sorted with quadsort instead.</description></item>
/// <item><description><strong>Partition:</strong> elements not greater than the pivot go to the front of the main array, greater ones to
/// swap memory, in scan order. A leading run of elements not greater than the pivot covering more than a quarter of the range means
/// the data is partly ordered, and both sides go to quadsort. The pivot is pushed on a stack kept at the end of swap memory.</description></item>
/// <item><description><strong>Recursion:</strong> the greater side is copied back and quadsorted when it is at most 96 elements or the
/// smaller side is at most 1/32 of it; otherwise it is partitioned from swap memory into place. The not-greater side continues in the
/// same loop unless it is at most 96 elements or the greater side is at most 1/32 of it, in which case it is quadsorted or reverse
/// partitioned.</description></item>
/// <item><description><strong>Reverse partition:</strong> when the not-greater side picks a pivot equal to the previous one (or the greater
/// side came out empty), the elements less than the pivot are separated from those equal to it, which are then in their final place.</description></item>
/// </list>
/// <para><strong>Implementation notes:</strong></para>
/// <list type="bullet">
/// <item><description>Configuration: this is a port of fluxsort 1.2.1.3 in the configuration where the comparison is inlined
/// (<c>#define cmp</c>), which is what a struct <typeparamref name="TComparer"/> becomes after JIT specialization: the streak threshold is
/// n / 256, and the cache limit that forces quadsort for quarters above 262,144 elements does not apply.</description></item>
/// <item><description>Partition step: the gcc form of the reference, which writes every element to both destinations and advances the
/// two cursors arithmetically, so the loop has no data-dependent branch. Each element therefore costs two writes, one of which a later
/// element overwrites.</description></item>
/// <item><description>Sample offset: the reference starts the sample at an offset taken from the address of a stack variable, which differs
/// between builds and runs. This port starts at the middle of the first stride, so the samples, and with them the operation counts, are
/// reproducible.</description></item>
/// <item><description>Pseudomedian of nine: the reference copies the nine candidates into a stack array; this port holds them in locals,
/// so the comparisons between candidates are reported without a buffer location.</description></item>
/// <item><description>Buffers: 0 is the input, 1 is the swap memory of n elements whose end holds the pivot stack, and 2 is quadsort's
/// 32-element stack array. Quadsort runs with fluxsort's swap memory, as the reference shares it.</description></item>
/// <item><description>Comparer contract: as in quadsort, the merges and the pivot stack rely on a consistent total order.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Hybrid (Partition + Merge)</description></item>
/// <item><description>Stable      : Yes (both partition destinations are filled in scan order, equal elements stay on the not-greater side, and quadsort is stable)</description></item>
/// <item><description>In-place    : No (n elements of swap memory shared with quadsort, holding the pivot stack at its end)</description></item>
/// <item><description>Best case   : Θ(n) - ordered input is recognised by the analysis with about n comparisons, strictly descending input is reversed, and low-cardinality ranges collapse by reverse partitions</description></item>
/// <item><description>Average case: Θ(n log n) - the sampled pivot splits each range near its median</description></item>
/// <item><description>Worst case  : O(n log n) - a partition continues only on a split no worse than 1:32; otherwise the larger side goes to quadsort, or, when the greater side is the small one, a reverse partition removes the pivot's duplicates (at least 1/17 of the range) before partitioning continues, or hands the rest to quadsort</description></item>
/// <item><description>Comparisons : Θ(n) best, Θ(n log n) average and worst - one per element per partition, plus pivot selection of O(n^(1/3) log n) per range and quadsort's merges for the ranges handed to it</description></item>
/// <item><description>Swaps       : O(n) - only reversals of descending quarters and stretches, and quadsort's small exchanges; partitions and merges move elements by writes</description></item>
/// <item><description>Index Reads : Θ(n) best, Θ(n log n) average and worst - every partition reads each element of its range once, plus the pivot it compares against</description></item>
/// <item><description>Index Writes: O(n) best, Θ(n log n) average and worst - every partition writes each element twice and copies the greater side back</description></item>
/// <item><description>Space       : O(n) - swap memory of n elements, plus quadsort's 32-element stack array and O(log n) recursion</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>GitHub: https://github.com/scandum/fluxsort (version 1.2.1.3)</para>
/// </remarks>
public static class FluxSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by FluxSortTests, which derives from StableSortTestsBase.</remarks>
    public static bool IsStable => true;

    // Buffer identifiers for visualization
    const int BUFFER_MAIN = 0;       // Main input array
    const int BUFFER_SWAP = 1;       // Swap memory: partition destination for greater elements, quadsort's swap, pivot stack at its end

    // Ranges this short are sorted by quadsort without analysis.
    const int ANALYZE_MIN = 133;

    // Partitions this short, on either side, go to quadsort (the reference's FLUX_OUT).
    const int FLUX_OUT = 96;

    // Ranges up to this size take the pseudomedian of nine; larger ones sample about the cube root.
    const int MEDIAN_OF_NINE_MAX = 2048;

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

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);

        if (n < ANALYZE_MIN)
        {
            context.OnPhase(SortPhase.FluxSortQuadsort, first, last - 1);
            QuadSort.SortCore(s, first, n);
            return;
        }

        var rented = ArrayPool<T>.Shared.Rent(n);
        try
        {
            var w = new SortSpan<T, TComparer, TContext>(rented.AsSpan(0, n), context, comparer, BUFFER_SWAP);
            Analyze(s, first, w, n, n);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(rented, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int Bit(bool value) => Unsafe.As<bool, byte>(ref value);

    /// <summary>
    /// Hands a[p..p + n) to quadsort with the shared swap memory, announcing the delegation.
    /// </summary>
    static void Quadsort<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int p, SortSpan<T, TComparer, TContext> w, int swapSize, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (n < 2) return;
        a.Context.OnPhase(SortPhase.FluxSortQuadsort, a.Offset + p, a.Offset + p + n - 1);
        QuadSort.SortSwap(a, p, w, 0, swapSize, n);
    }

    /// <summary>
    /// <c>flux_analyze</c>: decides per quarter of a[pa..pa + n) between quadsort and partitioning, after detecting fully ordered input and
    /// reversing strictly descending quarters, then merges the quarters.
    /// </summary>
    static void Analyze<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w, int swapSize, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        a.Context.OnPhase(SortPhase.FluxSortAnalyze, a.Offset + pa, a.Offset + pa + n - 1);

        var half1 = n / 2;
        var quad1 = half1 / 2;
        var quad2 = half1 - quad1;
        var half2 = n - half1;
        var quad3 = half2 / 2;
        var quad4 = half2 - quad3;

        var pta = pa;
        var ptb = pa + quad1;
        var ptc = pa + half1;
        var ptd = pa + half1 + quad3;

        int astreaks = 0, bstreaks = 0, cstreaks = 0, dstreaks = 0;
        int abalance = 0, bbalance = 0, cbalance = 0, dbalance = 0;

        // A quarter longer than the first starts one pair later, so all four end their scan on their last element together.
        if (quad1 < quad2) { bbalance += Bit(a.IsGreaterAt(ptb, ptb + 1)); ptb++; }
        if (quad1 < quad3) { cbalance += Bit(a.IsGreaterAt(ptc, ptc + 1)); ptc++; }
        if (quad1 < quad4) { dbalance += Bit(a.IsGreaterAt(ptd, ptd + 1)); ptd++; }

        int cnt;
        for (cnt = n; cnt > 132; cnt -= 128)
        {
            int asum = 0, bsum = 0, csum = 0, dsum = 0;

            for (var loop = 32; loop != 0; loop--)
            {
                asum += Bit(a.IsGreaterAt(pta, pta + 1)); pta++;
                bsum += Bit(a.IsGreaterAt(ptb, ptb + 1)); ptb++;
                csum += Bit(a.IsGreaterAt(ptc, ptc + 1)); ptc++;
                dsum += Bit(a.IsGreaterAt(ptd, ptd + 1)); ptd++;
            }

            // A block of 32 pairs that all ascend or all descend is a streak.
            abalance += asum; asum = Bit(asum == 0 || asum == 32); astreaks += asum;
            bbalance += bsum; bsum = Bit(bsum == 0 || bsum == 32); bstreaks += bsum;
            cbalance += csum; csum = Bit(csum == 0 || csum == 32); cstreaks += csum;
            dbalance += dsum; dsum = Bit(dsum == 0 || dsum == 32); dstreaks += dsum;

            // No quarter streaked: skip the next 96 pairs of each, counting them as half descending.
            if (cnt > 516 && asum + bsum + csum + dsum == 0)
            {
                abalance += 48; pta += 96;
                bbalance += 48; ptb += 96;
                cbalance += 48; ptc += 96;
                dbalance += 48; ptd += 96;
                cnt -= 384;
            }
        }

        for (; cnt > 7; cnt -= 4)
        {
            abalance += Bit(a.IsGreaterAt(pta, pta + 1)); pta++;
            bbalance += Bit(a.IsGreaterAt(ptb, ptb + 1)); ptb++;
            cbalance += Bit(a.IsGreaterAt(ptc, ptc + 1)); ptc++;
            dbalance += Bit(a.IsGreaterAt(ptd, ptd + 1)); ptd++;
        }

        // pta, ptb, ptc and ptd now sit on the last element of their quarter.
        cnt = abalance + bbalance + cbalance + dbalance;

        if (cnt == 0)
        {
            if (!a.IsGreaterAt(pta, pta + 1) && !a.IsGreaterAt(ptb, ptb + 1) && !a.IsGreaterAt(ptc, ptc + 1))
            {
                return;
            }
        }

        // A quarter whose every pair descended.
        var aReversed = quad1 - abalance == 1;
        var bReversed = quad2 - bbalance == 1;
        var cReversed = quad3 - cbalance == 1;
        var dReversed = quad4 - dbalance == 1;

        if (aReversed | bReversed | cReversed | dReversed)
        {
            // The reference multiplies the flags by the boundary comparison, so all three comparisons are made whatever the flags say.
            var boundary1 = a.IsGreaterAt(pta, pta + 1);
            var boundary2 = a.IsGreaterAt(ptb, ptb + 1);
            var boundary3 = a.IsGreaterAt(ptc, ptc + 1);
            var span1 = aReversed && bReversed && boundary1;
            var span2 = bReversed && cReversed && boundary2;
            var span3 = cReversed && dReversed && boundary3;

            switch (Bit(span1) | Bit(span2) * 2 | Bit(span3) * 4)
            {
                case 0: break;
                case 1: QuadSort.QuadReversal(a, pa, ptb); abalance = bbalance = 0; break;
                case 2: QuadSort.QuadReversal(a, pta + 1, ptc); bbalance = cbalance = 0; break;
                case 3: QuadSort.QuadReversal(a, pa, ptc); abalance = bbalance = cbalance = 0; break;
                case 4: QuadSort.QuadReversal(a, ptb + 1, ptd); cbalance = dbalance = 0; break;
                case 5:
                    QuadSort.QuadReversal(a, pa, ptb);
                    QuadSort.QuadReversal(a, ptb + 1, ptd);
                    abalance = bbalance = cbalance = dbalance = 0;
                    break;
                case 6: QuadSort.QuadReversal(a, pta + 1, ptd); bbalance = cbalance = dbalance = 0; break;
                case 7: QuadSort.QuadReversal(a, pa, ptd); return;
            }

            if (aReversed && abalance != 0) { QuadSort.QuadReversal(a, pa, pta); abalance = 0; }
            if (bReversed && bbalance != 0) { QuadSort.QuadReversal(a, pta + 1, ptb); bbalance = 0; }
            if (cReversed && cbalance != 0) { QuadSort.QuadReversal(a, ptb + 1, ptc); cbalance = 0; }
            if (dReversed && dbalance != 0) { QuadSort.QuadReversal(a, ptc + 1, ptd); dbalance = 0; }
        }

        // Switch a quarter to quadsort when it is at least half ordered.
        cnt = n / 256;
        var aStreaky = astreaks > cnt;
        var bStreaky = bstreaks > cnt;
        var cStreaky = cstreaks > cnt;
        var dStreaky = dstreaks > cnt;

        switch (Bit(aStreaky) + Bit(bStreaky) * 2 + Bit(cStreaky) * 4 + Bit(dStreaky) * 8)
        {
            case 0:
                Partition(a, pa, w, false, n, n);
                return;
            case 1:
                if (abalance != 0) Quadsort(a, pa, w, swapSize, quad1);
                Partition(a, pta + 1, w, false, quad2 + half2, quad2 + half2);
                break;
            case 2:
                Partition(a, pa, w, false, quad1, quad1);
                if (bbalance != 0) Quadsort(a, pta + 1, w, swapSize, quad2);
                Partition(a, ptb + 1, w, false, half2, half2);
                break;
            case 3:
                if (abalance != 0) Quadsort(a, pa, w, swapSize, quad1);
                if (bbalance != 0) Quadsort(a, pta + 1, w, swapSize, quad2);
                Partition(a, ptb + 1, w, false, half2, half2);
                break;
            case 4:
                Partition(a, pa, w, false, half1, half1);
                if (cbalance != 0) Quadsort(a, ptb + 1, w, swapSize, quad3);
                Partition(a, ptc + 1, w, false, quad4, quad4);
                break;
            case 8:
                Partition(a, pa, w, false, half1 + quad3, half1 + quad3);
                if (dbalance != 0) Quadsort(a, ptc + 1, w, swapSize, quad4);
                break;
            case 9:
                if (abalance != 0) Quadsort(a, pa, w, swapSize, quad1);
                Partition(a, pta + 1, w, false, quad2 + quad3, quad2 + quad3);
                if (dbalance != 0) Quadsort(a, ptc + 1, w, swapSize, quad4);
                break;
            case 12:
                Partition(a, pa, w, false, half1, half1);
                if (cbalance != 0) Quadsort(a, ptb + 1, w, swapSize, quad3);
                if (dbalance != 0) Quadsort(a, ptc + 1, w, swapSize, quad4);
                break;
            default: // 5, 6, 7, 10, 11, 13, 14, 15
                if (aStreaky) { if (abalance != 0) Quadsort(a, pa, w, swapSize, quad1); }
                else Partition(a, pa, w, false, quad1, quad1);
                if (bStreaky) { if (bbalance != 0) Quadsort(a, pta + 1, w, swapSize, quad2); }
                else Partition(a, pta + 1, w, false, quad2, quad2);
                if (cStreaky) { if (cbalance != 0) Quadsort(a, ptb + 1, w, swapSize, quad3); }
                else Partition(a, ptb + 1, w, false, quad3, quad3);
                if (dStreaky) { if (dbalance != 0) Quadsort(a, ptc + 1, w, swapSize, quad4); }
                else Partition(a, ptc + 1, w, false, quad4, quad4);
                break;
        }

        a.Context.OnPhase(SortPhase.FluxSortMerge, a.Offset + pa, a.Offset + pa + half1 - 1, a.Offset + pa + n - 1);

        if (!a.IsGreaterAt(pta, pta + 1))
        {
            if (!a.IsGreaterAt(ptc, ptc + 1))
            {
                if (!a.IsGreaterAt(ptb, ptb + 1)) return;
                a.CopyTo(pa, w, 0, n);
            }
            else
            {
                QuadSort.CrossMerge(w, half1, a, pa + half1, quad3, quad4);
                a.CopyTo(pa, w, 0, half1);
            }
        }
        else
        {
            if (!a.IsGreaterAt(ptc, ptc + 1))
            {
                a.CopyTo(pa + half1, w, half1, half2);
                QuadSort.CrossMerge(w, 0, a, pa, quad1, quad2);
            }
            else
            {
                QuadSort.CrossMerge(w, half1, a, ptb + 1, quad3, quad4);
                QuadSort.CrossMerge(w, 0, a, pa, quad1, quad2);
            }
        }
        QuadSort.CrossMerge(a, pa, w, 0, half1, half2);
    }

    // ------------------------------------------------------------------
    // Pivot selection
    // ------------------------------------------------------------------

    /// <summary>
    /// <c>binary_median</c>: the median of two sorted runs of <paramref name="len"/> elements at <paramref name="pta"/> and
    /// <paramref name="ptb"/> of the same buffer, found by halving both at once.
    /// </summary>
    static T BinaryMedian<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int pta, int ptb, int len)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while ((len /= 2) != 0)
        {
            if (!s.IsGreaterAt(pta + len, ptb + len)) pta += len; else ptb += len;
        }
        return s.IsGreaterAt(pta, ptb, out var av, out var bv) ? av : bv;
    }

    /// <summary>
    /// <c>trim_four</c>: leaves the middle two of four values in <paramref name="v1"/> and <paramref name="v2"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void TrimFour<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, ref T v0, ref T v1, ref T v2, ref T v3)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var x = s.IsGreaterThan(v0, v1);
        var t = x ? v0 : v1;
        v0 = x ? v1 : v0;
        v1 = t;

        x = s.IsGreaterThan(v2, v3);
        t = x ? v2 : v3;
        v2 = x ? v3 : v2;
        v3 = t;

        // The larger of the two minima and the smaller of the two maxima.
        v2 = s.IsLessOrEqual(v0, v2) ? v2 : v0;
        v1 = s.IsGreaterThan(v1, v3) ? v3 : v1;
    }

    /// <summary>
    /// <c>median_of_nine</c>: the pseudomedian of nine evenly spaced elements of src[ps..ps + n), with 15 comparisons.
    /// </summary>
    static T MedianOfNine<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> src, int ps, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var z = n / 9;

        var s0 = src.Read(ps);
        var s1 = src.Read(ps + z);
        var s2 = src.Read(ps + z * 2);
        var s3 = src.Read(ps + z * 3);
        var s4 = src.Read(ps + z * 4);
        var s5 = src.Read(ps + z * 5);
        var s6 = src.Read(ps + z * 6);
        var s7 = src.Read(ps + z * 7);
        var s8 = src.Read(ps + z * 8);

        TrimFour(src, ref s0, ref s1, ref s2, ref s3);
        TrimFour(src, ref s4, ref s5, ref s6, ref s7);

        s0 = s5;
        s3 = s8;

        TrimFour(src, ref s0, ref s1, ref s2, ref s3);

        s0 = s6;

        var x = src.IsGreaterThan(s0, s1);
        var y = src.IsGreaterThan(s0, s2);
        var zz = src.IsGreaterThan(s1, s2);

        return (Bit(x == y) + Bit(y ^ zz)) switch
        {
            0 => s0,
            1 => s1,
            _ => s2,
        };
    }

    /// <summary>
    /// <c>median_of_cbrt</c>: copies an evenly spaced sample of about the cube root of the range into the buffer the range is not in,
    /// sorts its two halves with quadsort, and returns their combined median. <paramref name="generic"/> reports a sample whose first half
    /// is constant and whose second half does not exceed it.
    /// </summary>
    static T MedianOfCbrt<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w, bool fromSwap, int n, out bool generic)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var cbrt = 32;
        while (n > (long)cbrt * cbrt * cbrt) cbrt *= 2;

        var div = n / cbrt;

        var src = fromSwap ? w : a;
        var ps = fromSwap ? 0 : pa;
        var pts = fromSwap ? a : w;
        var pd = fromSwap ? pa : 0;

        a.Context.OnPhase(SortPhase.FluxSortPivotSample, a.Offset + pa, a.Offset + pa + n - 1, cbrt);

        var pti = ps + div / 2;
        for (var cnt = 0; cnt < cbrt; cnt++)
        {
            pts.Write(pd + cnt, src.Read(pti));
            pti += div;
        }

        cbrt /= 2;

        QuadSort.SortSwap(pts, pd, pts, pd + cbrt * 2, cbrt, cbrt);
        QuadSort.SortSwap(pts, pd + cbrt, pts, pd + cbrt * 2, cbrt, cbrt);

        // The reference combines the two comparisons with a bitwise &, so both are made.
        var secondNotAbove = !pts.IsGreaterAt(pd + cbrt * 2 - 1, pd);
        var firstConstant = !pts.IsGreaterAt(pd + cbrt - 1, pd);
        generic = secondNotAbove & firstConstant;

        return BinaryMedian(pts, pd, pd + cbrt, cbrt);
    }

    // ------------------------------------------------------------------
    // Partitioning
    // ------------------------------------------------------------------

    /// <summary>
    /// <c>flux_reverse_partition</c>: splits a[pa..pa + n) into the elements less than the pivot at w[piv] and those not less, which
    /// all equal it here, and sorts the first part.
    /// </summary>
    static void ReversePartition<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w, int piv, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        a.Context.OnPhase(SortPhase.FluxSortReversePartition, a.Offset + pa, a.Offset + pa + n - 1);
        a.Context.OnRole(w.Offset + piv, BUFFER_SWAP, RoleType.Pivot);

        var m = 0;
        var i = 0;

        for (var cnt = n / 8; cnt != 0; cnt--)
        {
            ReverseStep(a, pa, w, piv, ref i, ref m);
            ReverseStep(a, pa, w, piv, ref i, ref m);
            ReverseStep(a, pa, w, piv, ref i, ref m);
            ReverseStep(a, pa, w, piv, ref i, ref m);
            ReverseStep(a, pa, w, piv, ref i, ref m);
            ReverseStep(a, pa, w, piv, ref i, ref m);
            ReverseStep(a, pa, w, piv, ref i, ref m);
            ReverseStep(a, pa, w, piv, ref i, ref m);
        }

        for (var cnt = n % 8; cnt != 0; cnt--)
        {
            ReverseStep(a, pa, w, piv, ref i, ref m);
        }

        a.Context.OnRole(w.Offset + piv, BUFFER_SWAP, RoleType.None);

        var aSize = m;
        var sSize = n - aSize;

        w.CopyTo(0, a, pa + aSize, sSize);

        if (sSize <= aSize / 16 || aSize <= FLUX_OUT)
        {
            Quadsort(a, pa, w, aSize, aSize);
            return;
        }
        Partition(a, pa, w, false, piv, aSize);
    }

    /// <summary>
    /// One element of the reverse partition: written to both destinations, and the cursor of the side it belongs to advances.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void ReverseStep<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w, int piv, ref int i, ref int m)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var less = w.IsGreaterAcross(piv, a, pa + i, out _, out var v);
        a.Write(pa + m, v);
        w.Write(i - m, v);
        m += Bit(less);
        i++;
    }

    /// <summary>
    /// <c>flux_default_partition</c>: splits the range read from <paramref name="src"/>[<paramref name="ps"/>..) into the elements not
    /// greater than the pivot at w[piv], written to a[pa..), and the greater ones, written to w[0..). Returns the count of the first part,
    /// or 0 after sorting both parts with quadsort when a leading run of elements not greater than the pivot covered more than a quarter.
    /// </summary>
    static int DefaultPartition<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w,
        SortSpan<T, TComparer, TContext> src, int ps, int piv, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var run = 0;
        var m = 0;
        var i = 0;

        for (var blk = 8; blk <= n; blk += 8)
        {
            PartitionStep(a, pa, w, src, ps, piv, ref i, ref m);
            PartitionStep(a, pa, w, src, ps, piv, ref i, ref m);
            PartitionStep(a, pa, w, src, ps, piv, ref i, ref m);
            PartitionStep(a, pa, w, src, ps, piv, ref i, ref m);
            PartitionStep(a, pa, w, src, ps, piv, ref i, ref m);
            PartitionStep(a, pa, w, src, ps, piv, ref i, ref m);
            PartitionStep(a, pa, w, src, ps, piv, ref i, ref m);
            PartitionStep(a, pa, w, src, ps, piv, ref i, ref m);

            if (m == blk) run = blk;
        }

        for (var cnt = n % 8; cnt != 0; cnt--)
        {
            PartitionStep(a, pa, w, src, ps, piv, ref i, ref m);
        }

        if (run <= n / 4) return m;

        if (m == n) return m;

        var greater = n - m;

        w.CopyTo(0, a, pa + m, greater);

        Quadsort(a, pa + m, w, greater, greater);
        Quadsort(a, pa, w, m, m);

        return 0;
    }

    /// <summary>
    /// One element of the default partition: written to both destinations, and the cursor of the side it belongs to advances.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void PartitionStep<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w,
        SortSpan<T, TComparer, TContext> src, int ps, int piv, ref int i, ref int m)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var notGreater = !src.IsGreaterAcross(ps + i, w, piv, out var v, out _);
        a.Write(pa + m, v);
        w.Write(i - m, v);
        m += Bit(notGreater);
        i++;
    }

    /// <summary>
    /// <c>flux_partition</c>: sorts the <paramref name="n"/> elements read from w[0..) when <paramref name="fromSwap"/> is set, or from
    /// a[pa..) otherwise, into a[pa..pa + n). Pivots are pushed below <paramref name="piv"/> on the stack at the end of swap memory.
    /// </summary>
    static void Partition<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w, bool fromSwap, int piv, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var aSize = 0;

        while (true)
        {
            --piv;

            if (n <= MEDIAN_OF_NINE_MAX)
            {
                w.Write(piv, fromSwap ? MedianOfNine(w, 0, n) : MedianOfNine(a, pa, n));
            }
            else
            {
                w.Write(piv, MedianOfCbrt(a, pa, w, fromSwap, n, out var generic));

                if (generic)
                {
                    if (fromSwap)
                    {
                        w.CopyTo(0, a, pa, n);
                    }
                    Quadsort(a, pa, w, n, n);
                    return;
                }
            }

            // The same pivot as the previous round: everything here is not greater than it, so take out the equal elements.
            if (aSize != 0 && !w.IsGreaterAt(piv + 1, piv))
            {
                ReversePartition(a, pa, w, piv, n);
                return;
            }

            a.Context.OnPhase(SortPhase.FluxSortPartition, a.Offset + pa, a.Offset + pa + n - 1, fromSwap ? BUFFER_SWAP : BUFFER_MAIN);
            a.Context.OnRole(w.Offset + piv, BUFFER_SWAP, RoleType.Pivot);
            aSize = fromSwap ? DefaultPartition(a, pa, w, w, 0, piv, n) : DefaultPartition(a, pa, w, a, pa, piv, n);
            a.Context.OnRole(w.Offset + piv, BUFFER_SWAP, RoleType.None);
            var sSize = n - aSize;

            if (aSize <= sSize / 32 || sSize <= FLUX_OUT)
            {
                if (aSize == 0) return;

                if (sSize == 0)
                {
                    ReversePartition(a, pa, w, piv, aSize);
                    return;
                }
                w.CopyTo(0, a, pa + aSize, sSize);
                Quadsort(a, pa + aSize, w, sSize, sSize);
            }
            else
            {
                Partition(a, pa + aSize, w, true, piv, sSize);
            }

            if (sSize <= aSize / 32 || aSize <= FLUX_OUT)
            {
                if (aSize <= FLUX_OUT)
                {
                    Quadsort(a, pa, w, aSize, aSize);
                }
                else
                {
                    ReversePartition(a, pa, w, piv, aSize);
                }
                return;
            }
            n = aSize;
            fromSwap = false;
        }
    }
}
