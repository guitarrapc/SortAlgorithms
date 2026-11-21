using SortAlgorithm.Contexts;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 先に入力を4分割して並び具合を調べ、並んだ区間の多い4分の1は quadsort に任せ、それ以外を fulcrum 分割で in-place に並べる、不安定なクイックソートと
/// マージソートの混成ソートです（van den Hoven, 2022）。fulcrum 分割は範囲の先頭と末尾の32要素ずつを512要素の補助領域へ退避して空きを作り、
/// 空きの少ない側から16要素ずつ読んで、各要素を両端の次の空きに書き込みます。偏った分割や短い区間は quadsort に、同じピボットが続いたときは逆向きの分割に回します。
/// <br/>
/// An unstable hybrid of quicksort and mergesort (van den Hoven, 2022). An up-front analysis of the input's four quarters finds ordered,
/// reversed and streaky stretches and hands streaky quarters to quadsort; everything else is partitioned in place by the fulcrum partition.
/// It sets the first and last 32 elements of the range aside in 512 elements of swap memory, which frees room at both ends, then reads 16
/// elements at a time from the end with less free room and writes each to the next free slot at both ends, the end it belongs to keeping it.
/// Unbalanced or short partitions go to quadsort, and a pivot equal to the previous one leads to a reverse partition.
/// </summary>
/// <remarks>
/// <para><strong>Algorithm:</strong></para>
/// <list type="number">
/// <item><description><strong>Small input:</strong> up to 256 elements are sorted by quadsort with swap memory of their own size.</description></item>
/// <item><description><strong>Analysis:</strong> each quarter's adjacent pairs are counted as descending or not, 32 pairs at a time. A block of 32
/// that is entirely one or the other is a streak. When none of the four quarters shows a streak, the next 96 pairs of each are skipped and
/// assumed half descending. Input with no descending pair and ordered quarter boundaries is already sorted. Quarters that descend strictly
/// throughout are reversed, together with a neighbour when the boundary between them descends too.</description></item>
/// <item><description><strong>Dispatch:</strong> a quarter with more streaks than n / 256 is sorted by quadsort (skipped when it has no descending
/// pair). Adjacent non-streaky quarters are partitioned together as one range. The sorted quarters are then merged in place with rotations,
/// skipping the merges whose boundaries are already in order.</description></item>
/// <item><description><strong>Pivot:</strong> the median of three medians of three at fixed sixteenths for up to 2,048 elements. Above that, an
/// evenly spaced sample of 32 · 2^k elements (the smallest such that the sample's cube is at least the range, at most the swap memory) is
/// exchanged to the front of the range, its halves are sorted with quadsort, and the median of the two is found by binary search. A sample
/// whose first half is constant and whose second half does not exceed it marks low-cardinality data, and the range is sorted with quadsort
/// instead.</description></item>
/// <item><description><strong>Fulcrum partition:</strong> the pivot's slot takes the last element and the pivot is held aside. The first and last
/// 32 elements of the rest are copied into swap memory; then, 16 at a time, elements are read from the front when at most 48 slots are free
/// at the front and from the back when at least 16 are. Each element is written to the next slot at the front and the next slot at the back,
/// and the front cursor advances when it is not greater than the pivot; the slot the element did not keep is overwritten later. The remainder
/// and the 64 elements in swap memory follow, and the pivot goes to the boundary.</description></item>
/// <item><description><strong>Recursion:</strong> the side above the pivot is sorted by quadsort when it is at most 96 elements or the side below
/// is at most 1/32 of it, and partitioned otherwise, with the pivot as the upper bound it carries. The side below continues in the loop, unless
/// it is at most 96 elements (quadsort) or the side above is at most 1/32 of it (reverse partition).</description></item>
/// <item><description><strong>Reverse partition:</strong> when the new pivot is not less than the bound the range carries, every element is at
/// most the pivot, so the elements less than it are separated from those equal to it, which are then in their final place.</description></item>
/// </list>
/// <para><strong>Implementation notes:</strong></para>
/// <list type="bullet">
/// <item><description>Configuration: this is a port of crumsort 1.2.1.3 in the configuration where the comparison is inlined (<c>#define cmp</c>),
/// which is what a struct <typeparamref name="TComparer"/> becomes after JIT specialization: the streak threshold is n / 256, and the cache
/// limit that forces quadsort for quarters above 262,144 elements does not apply. Swap memory is the default 512 elements (<c>CRUM_AUX</c>).</description></item>
/// <item><description>Sample offset: the reference starts the cube-root sample at an offset taken from the address of a stack variable, which
/// differs between builds and runs. This port starts it at the middle of the last stride, so the samples, and with them the operation counts,
/// are reproducible.</description></item>
/// <item><description>Swap memory: 512 elements, or n for up to 256 elements, rented from <see cref="ArrayPool{T}"/> where the reference uses a
/// stack array, because a stack array cannot hold an arbitrary <typeparamref name="T"/>.</description></item>
/// <item><description>Buffers: 0 is the input, 1 is the swap memory, which also holds the exchanged pivot sample's temporary, and 2 is
/// quadsort's 32-element stack array. Quadsort runs with crumsort's swap memory, as the reference shares it.</description></item>
/// <item><description>Comparer contract: the partitions write every element they read to the range and to nothing else, whatever the comparer
/// answers. Quadsort may repeat or lose elements under a comparer that is not a consistent total order, and it can throw
/// <see cref="InvalidOperationException"/>; nothing outside the buffers is read or written.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Hybrid (Partition + Merge)</description></item>
/// <item><description>Stable      : No (the fulcrum partition writes elements read from both ends of the range into one side)</description></item>
/// <item><description>In-place    : Yes (512 elements of swap memory, a constant, and O(log n) recursion)</description></item>
/// <item><description>Best case   : Θ(n) - ordered input is recognised by the analysis with about n comparisons, and strictly descending input is reversed</description></item>
/// <item><description>Average case: Θ(n log n) - the sampled pivot splits each range near its median, and each partition reads every element of its range once and writes it twice</description></item>
/// <item><description>Worst case  : O(n log n) comparisons, O(n log² n) moves - a partition continues only on a split no worse than 1:32, otherwise quadsort or a reverse partition takes over; quadsort and the quarter merges merge runs longer than the 512 elements of swap memory with rotations</description></item>
/// <item><description>Comparisons : Θ(n) best, Θ(n log n) average and worst - one per element per partition, plus pivot selection of O(n^(1/3) log n) per range and quadsort's merges for the ranges handed to it</description></item>
/// <item><description>Swaps       : O(n log² n) - reversals of descending quarters and stretches, quadsort's small exchanges, and the exchanges rotations fall back to when both sides exceed swap memory; partitions move elements by writes</description></item>
/// <item><description>Index Reads : Θ(n) best, Θ(n log n) average, O(n log² n) worst - the analysis reads the pairs it counts, a partition reads every element of its range once, and quadsort, rotations and merges read what they compare or move</description></item>
/// <item><description>Index Writes: 0 best, Θ(n log n) average, O(n log² n) worst - a partition writes every element twice, at both ends, plus the 64 it sets aside and the pivot's moves; reversals, quadsort, rotations and the sample exchange add their own</description></item>
/// <item><description>Space       : O(log n) - 512 elements of swap memory and quadsort's 32-element stack array, both constants, plus the recursion</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>GitHub: https://github.com/scandum/crumsort (version 1.2.1.3, public domain), with quadsort 1.2.1.3</para>
/// </remarks>
public static class CrumSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by CrumSortTests, which derives from SortTestsBase.</remarks>
    public static bool IsStable => false;

    // Buffer identifiers for visualization
    const int BUFFER_MAIN = 0;       // Main input array
    const int BUFFER_SWAP = 1;       // Swap memory: the partition's 64 set-aside elements, quadsort's swap, the sample exchange's temporary

    // Ranges up to this size are sorted by quadsort with swap memory of their own size.
    const int QUADSORT_MAX = 256;

    // Elements of swap memory (the reference's CRUM_AUX).
    const int CRUM_AUX = 512;

    // Partitions this short, on either side, go to quadsort (the reference's CRUM_OUT).
    const int CRUM_OUT = 96;

    // Ranges up to this size take the median of nine. Larger ones sample about the cube root.
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
        var swapSize = n <= QUADSORT_MAX ? n : CRUM_AUX;
        var rented = ArrayPool<T>.Shared.Rent(swapSize);
        try
        {
            var w = new SortSpan<T, TComparer, TContext>(rented.AsSpan(0, swapSize), context, comparer, BUFFER_SWAP);
            if (n <= QUADSORT_MAX) Quadsort(s, first, w, swapSize, n);
            else Analyze(s, first, w, swapSize, n);
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
        a.Context.OnPhase(SortPhase.CrumSortQuadsort, a.Offset + p, a.Offset + p + n - 1);
        QuadSort.SortSwap(a, p, w, 0, swapSize, n);
    }

    /// <summary>
    /// <c>crum_analyze</c>: decides per quarter of a[pa..pa + n) between quadsort and partitioning, after detecting fully ordered input and
    /// reversing strictly descending quarters, then merges the quarters in place.
    /// </summary>
    static void Analyze<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w, int swapSize, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        a.Context.OnPhase(SortPhase.CrumSortAnalyze, a.Offset + pa, a.Offset + pa + n - 1);

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

        // A quarter longer than the first has one pair more, counted last.
        if (quad1 < quad2) { bbalance += Bit(a.IsGreaterAt(ptb, ptb + 1)); ptb++; }
        if (quad1 < quad3) { cbalance += Bit(a.IsGreaterAt(ptc, ptc + 1)); ptc++; }
        if (quad1 < quad4) { dbalance += Bit(a.IsGreaterAt(ptd, ptd + 1)); ptd++; }

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
                Partition(a, pa, w, swapSize, n, -1);
                return;
            case 1:
                if (abalance != 0) Quadsort(a, pa, w, swapSize, quad1);
                Partition(a, pta + 1, w, swapSize, quad2 + half2, -1);
                break;
            case 2:
                Partition(a, pa, w, swapSize, quad1, -1);
                if (bbalance != 0) Quadsort(a, pta + 1, w, swapSize, quad2);
                Partition(a, ptb + 1, w, swapSize, half2, -1);
                break;
            case 3:
                if (abalance != 0) Quadsort(a, pa, w, swapSize, quad1);
                if (bbalance != 0) Quadsort(a, pta + 1, w, swapSize, quad2);
                Partition(a, ptb + 1, w, swapSize, half2, -1);
                break;
            case 4:
                Partition(a, pa, w, swapSize, half1, -1);
                if (cbalance != 0) Quadsort(a, ptb + 1, w, swapSize, quad3);
                Partition(a, ptc + 1, w, swapSize, quad4, -1);
                break;
            case 8:
                Partition(a, pa, w, swapSize, half1 + quad3, -1);
                if (dbalance != 0) Quadsort(a, ptc + 1, w, swapSize, quad4);
                break;
            case 9:
                if (abalance != 0) Quadsort(a, pa, w, swapSize, quad1);
                Partition(a, pta + 1, w, swapSize, quad2 + quad3, -1);
                if (dbalance != 0) Quadsort(a, ptc + 1, w, swapSize, quad4);
                break;
            case 12:
                Partition(a, pa, w, swapSize, half1, -1);
                if (cbalance != 0) Quadsort(a, ptb + 1, w, swapSize, quad3);
                if (dbalance != 0) Quadsort(a, ptc + 1, w, swapSize, quad4);
                break;
            default: // 5, 6, 7, 10, 11, 13, 14, 15
                if (aStreaky) { if (abalance != 0) Quadsort(a, pa, w, swapSize, quad1); }
                else Partition(a, pa, w, swapSize, quad1, -1);
                if (bStreaky) { if (bbalance != 0) Quadsort(a, pta + 1, w, swapSize, quad2); }
                else Partition(a, pta + 1, w, swapSize, quad2, -1);
                if (cStreaky) { if (cbalance != 0) Quadsort(a, ptb + 1, w, swapSize, quad3); }
                else Partition(a, ptb + 1, w, swapSize, quad3, -1);
                if (dStreaky) { if (dbalance != 0) Quadsort(a, ptc + 1, w, swapSize, quad4); }
                else Partition(a, ptc + 1, w, swapSize, quad4, -1);
                break;
        }

        a.Context.OnPhase(SortPhase.CrumSortMerge, a.Offset + pa, a.Offset + pa + half1 - 1, a.Offset + pa + n - 1);

        if (!a.IsGreaterAt(pta, pta + 1))
        {
            if (!a.IsGreaterAt(ptc, ptc + 1))
            {
                if (!a.IsGreaterAt(ptb, ptb + 1)) return;
            }
            else
            {
                QuadSort.RotateMergeBlock(a, pa + half1, w, 0, swapSize, quad3, quad4);
            }
        }
        else
        {
            QuadSort.RotateMergeBlock(a, pa, w, 0, swapSize, quad1, quad2);

            if (a.IsGreaterAt(ptc, ptc + 1))
            {
                QuadSort.RotateMergeBlock(a, pa + half1, w, 0, swapSize, quad3, quad4);
            }
        }
        QuadSort.RotateMergeBlock(a, pa, w, 0, swapSize, half1, half2);
    }

    // ------------------------------------------------------------------
    // Pivot selection
    // ------------------------------------------------------------------

    /// <summary>
    /// <c>crum_binary_median</c>: the index of the median of two sorted runs of <paramref name="len"/> elements at <paramref name="pta"/>
    /// and <paramref name="ptb"/>, found by halving both at once.
    /// </summary>
    static int BinaryMedian<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pta, int ptb, int len)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while ((len /= 2) != 0)
        {
            if (!a.IsGreaterAt(pta + len, ptb + len)) pta += len; else ptb += len;
        }
        return a.IsGreaterAt(pta, ptb) ? pta : ptb;
    }

    /// <summary>
    /// <c>crum_median_of_cbrt</c>: exchanges an evenly spaced sample of about the cube root of a[pa..pa + n) with the front of the range,
    /// sorts its two halves there with quadsort, and returns the index of their combined median. <paramref name="generic"/> reports a sample
    /// whose first half is constant and whose second half does not exceed it.
    /// </summary>
    static int MedianOfCbrt<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w, int swapSize, int n, out bool generic)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var cbrt = 32;
        while (n > (long)cbrt * cbrt * cbrt && cbrt < swapSize) cbrt *= 2;

        var div = n / cbrt;

        a.Context.OnPhase(SortPhase.CrumSortPivotSample, a.Offset + pa, a.Offset + pa + n - 1, cbrt);

        // The reference subtracts an offset taken from a stack address; the middle of the stride keeps the sample reproducible.
        var pta = pa + n - 1 - div / 2;
        var piv = pa + cbrt;

        for (var cnt = cbrt; cnt != 0; cnt--)
        {
            w.Write(0, a.Read(--piv));
            a.Write(piv, a.Read(pta));
            a.Write(pta, w.Read(0));

            pta -= div;
        }

        cbrt /= 2;

        QuadSort.SortSwap(a, piv, w, 0, swapSize, cbrt);
        QuadSort.SortSwap(a, piv + cbrt, w, 0, swapSize, cbrt);

        // The quadsort calls are over, so their detail phase must not cover the median search.
        a.Context.OnPhase(SortPhase.CrumSortPivotSample, a.Offset + pa, a.Offset + pa + n - 1, cbrt * 2);

        // The reference combines the two comparisons with a bitwise &, so both are made.
        var secondNotAbove = !a.IsGreaterAt(piv + cbrt * 2 - 1, piv);
        var firstConstant = !a.IsGreaterAt(piv + cbrt - 1, piv);
        generic = secondNotAbove & firstConstant;

        return BinaryMedian(a, piv, piv + cbrt, cbrt);
    }

    /// <summary>
    /// <c>crum_median_of_three</c>: the offset, among <paramref name="v0"/>, <paramref name="v1"/> and <paramref name="v2"/> from
    /// <paramref name="pa"/>, of the median of the three elements.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int MedianOfThree<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, int v0, int v1, int v2)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var x = a.IsGreaterAt(pa + v0, pa + v1);
        var y = a.IsGreaterAt(pa + v0, pa + v2);
        var z = a.IsGreaterAt(pa + v1, pa + v2);

        return (Bit(x == y) + Bit(y ^ z)) switch
        {
            0 => v0,
            1 => v1,
            _ => v2,
        };
    }

    /// <summary>
    /// <c>crum_median_of_nine</c>: the index of the median of the medians of three groups of three elements at fixed sixteenths of the range.
    /// </summary>
    static int MedianOfNine<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var div = n / 16;

        var x = MedianOfThree(a, pa, div * 2, div * 1, div * 4);
        var y = MedianOfThree(a, pa, div * 8, div * 6, div * 10);
        var z = MedianOfThree(a, pa, div * 14, div * 12, div * 15);

        return pa + MedianOfThree(a, pa, x, y, z);
    }

    /// <summary>
    /// Returns the index of the pivot of a[pa..pa + n). <paramref name="generic"/> reports that the cube-root sample marks low-cardinality data.
    /// The work on elements stays in methods that are not inlined into the recursive <see cref="Partition"/>, so that its frames stay small.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    static int ChoosePivot<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w, int swapSize, int n, out bool generic)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (n <= MEDIAN_OF_NINE_MAX)
        {
            a.Context.OnPhase(SortPhase.CrumSortPivotSample, a.Offset + pa, a.Offset + pa + n - 1, 9);
            generic = false;
            return MedianOfNine(a, pa, n);
        }
        return MedianOfCbrt(a, pa, w, swapSize, n, out generic);
    }

    // ------------------------------------------------------------------
    // Partitioning
    // ------------------------------------------------------------------

    /// <summary>
    /// <c>fulcrum_default_partition</c>: rearranges a[pa..pa + n) so that the elements not greater than <paramref name="piv"/> come first,
    /// and returns their count.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    static int DefaultPartition<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w, T piv, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        Debug.Assert(n >= 64);

        a.CopyTo(pa, w, 0, 32);
        a.CopyTo(pa + n - 32, w, 32, 32);

        var ptr = pa + n - 1;
        var pta = pa + 32;
        var tpa = pa + n - 33;
        var m = 0;

        // The count of 16-element blocks to read, both ends together, before the remainder; the 64 set aside are not among them.
        var cnt = n / 16 - 4;

        while (true)
        {
            if (pta - pa - m <= 48)
            {
                if (cnt-- == 0) break;

                for (var i = 16; i != 0; i--) DefaultStep(a, a, pta++, piv, pa, ref ptr, ref m);
            }
            if (pta - pa - m >= 16)
            {
                if (cnt-- == 0) break;

                for (var i = 16; i != 0; i--) DefaultStep(a, a, tpa--, piv, pa, ref ptr, ref m);
            }
        }

        if (pta - pa - m <= 48)
        {
            for (cnt = n % 16; cnt != 0; cnt--) DefaultStep(a, a, pta++, piv, pa, ref ptr, ref m);
        }
        else
        {
            for (cnt = n % 16; cnt != 0; cnt--) DefaultStep(a, a, tpa--, piv, pa, ref ptr, ref m);
        }

        for (var k = 0; k < 64; k++) DefaultStep(a, w, k, piv, pa, ref ptr, ref m);

        return m;
    }

    /// <summary>
    /// One element of the default partition: written to the next slot at both ends, the front keeping it when it is not greater than the pivot.
    /// The reference assigns the back slot first.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void DefaultStep<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, SortSpan<T, TComparer, TContext> src, int i, T piv, int ptl, ref int ptr, ref int m)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var notGreater = !src.IsElementGreaterThan(i, piv, out var v);
        a.Write(ptr + m, v);
        a.Write(ptl + m, v);
        m += Bit(notGreater);
        ptr--;
    }

    /// <summary>
    /// <c>fulcrum_reverse_partition</c>: rearranges a[pa..pa + n) so that the elements less than <paramref name="piv"/> come first, and returns
    /// their count. It is the default partition with the comparison turned around.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    static int ReversePartition<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w, T piv, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        Debug.Assert(n >= 64);

        a.CopyTo(pa, w, 0, 32);
        a.CopyTo(pa + n - 32, w, 32, 32);

        var ptr = pa + n - 1;
        var pta = pa + 32;
        var tpa = pa + n - 33;
        var m = 0;
        var cnt = n / 16 - 4;

        while (true)
        {
            if (pta - pa - m <= 48)
            {
                if (cnt-- == 0) break;

                for (var i = 16; i != 0; i--) ReverseStep(a, a, pta++, piv, pa, ref ptr, ref m);
            }
            if (pta - pa - m >= 16)
            {
                if (cnt-- == 0) break;

                for (var i = 16; i != 0; i--) ReverseStep(a, a, tpa--, piv, pa, ref ptr, ref m);
            }
        }

        if (pta - pa - m <= 48)
        {
            for (cnt = n % 16; cnt != 0; cnt--) ReverseStep(a, a, pta++, piv, pa, ref ptr, ref m);
        }
        else
        {
            for (cnt = n % 16; cnt != 0; cnt--) ReverseStep(a, a, tpa--, piv, pa, ref ptr, ref m);
        }

        for (var k = 0; k < 64; k++) ReverseStep(a, w, k, piv, pa, ref ptr, ref m);

        return m;
    }

    /// <summary>
    /// One element of the reverse partition: written to the next slot at both ends, the front keeping it when the pivot is greater.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void ReverseStep<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, SortSpan<T, TComparer, TContext> src, int i, T piv, int ptl, ref int ptr, ref int m)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var less = src.IsValueGreaterThan(piv, i, out var v);
        a.Write(ptr + m, v);
        a.Write(ptl + m, v);
        m += Bit(less);
        ptr--;
    }

    /// <summary>
    /// <c>fulcrum_partition</c>: sorts a[pa..pa + n). <paramref name="max"/> is the index of an element known to be not less than every
    /// element of the range, the pivot of an enclosing partition, or -1 when there is none.
    /// </summary>
    static void Partition<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w, int swapSize, int n, int max)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (true)
        {
            var ptp = ChoosePivot(a, pa, w, swapSize, n, out var generic);
            if (generic) break;

            var piv = a.Read(ptp);

            int aSize, sSize;

            // Every element here is at most the bound, so a pivot not less than it leaves only elements equal to the pivot above it.
            if (max >= 0 && !a.IsElementGreaterThan(max, piv))
            {
                a.Context.OnPhase(SortPhase.CrumSortReversePartition, a.Offset + pa, a.Offset + pa + n - 1);
                aSize = ReversePartition(a, pa, w, piv, n);
                sSize = n - aSize;
                n = aSize;

                if (sSize <= aSize / 32 || aSize <= CRUM_OUT) break;

                max = -1;
                continue;
            }

            // The last element fills the pivot's slot, and the pivot, held aside, later fills the boundary.
            n--;
            a.Write(ptp, a.Read(pa + n));

            a.Context.OnPhase(SortPhase.CrumSortPartition, a.Offset + pa, a.Offset + pa + n - 1);
            aSize = DefaultPartition(a, pa, w, piv, n);
            sSize = n - aSize;

            ptp = pa + aSize;
            a.Write(pa + n, a.Read(ptp));
            a.Write(ptp, piv);

            if (aSize <= sSize / 32 || sSize <= CRUM_OUT)
            {
                Quadsort(a, ptp + 1, w, swapSize, sSize);
            }
            else
            {
                Partition(a, ptp + 1, w, swapSize, sSize, max);
            }
            n = aSize;

            if (sSize <= aSize / 32 || aSize <= CRUM_OUT)
            {
                if (aSize <= CRUM_OUT) break;

                a.Context.OnPhase(SortPhase.CrumSortReversePartition, a.Offset + pa, a.Offset + pa + n - 1);
                aSize = ReversePartition(a, pa, w, piv, n);
                sSize = n - aSize;
                n = aSize;

                if (sSize <= aSize / 32 || aSize <= CRUM_OUT) break;

                max = -1;
                continue;
            }
            max = ptp;
        }
        Quadsort(a, pa, w, swapSize, n);
    }
}
