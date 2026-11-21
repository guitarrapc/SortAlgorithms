using SortAlgorithm.Contexts;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 先に入力を4分割して並び具合を調べ、並んだ区間の多い4分の1は quadsort に任せ、それ以外を安定な分割で並べる、in-place で安定なクイックソートとマージソートの
/// 混成ソートです（van den Hoven, 2022）。補助領域は512要素だけで、それに収まる区間は大きい要素を補助領域へ退避して戻し、収まらない区間は半分ずつ分割してから
/// 中央の2つの部分を回転で入れ替えます。quadsort も同じ補助領域を使い、長い連を回転でマージします。
/// <br/>
/// An in-place stable hybrid of quicksort and mergesort (van den Hoven, 2022). An up-front analysis of the input's four quarters finds ordered,
/// reversed and streaky stretches and hands streaky quarters to quadsort; everything else is partitioned stably. Only 512 elements of swap
/// memory are used: a piece that fits moves its greater elements there and back, and a longer range partitions its two halves and then
/// exchanges the middle two parts with a rotation. Quadsort shares the swap memory and merges runs longer than it with rotations.
/// </summary>
/// <remarks>
/// <para><strong>Algorithm:</strong></para>
/// <list type="number">
/// <item><description><strong>Small input:</strong> up to 132 elements are sorted by quadsort.</description></item>
/// <item><description><strong>Analysis:</strong> each quarter's adjacent pairs are counted as descending or not, 32 pairs at a time. A block of 32
/// that is entirely one or the other is a streak. When none of the four quarters shows a streak, the next 96 pairs of each are skipped and
/// assumed half descending. Input with no descending pair and ordered quarter boundaries is already sorted. Quarters that descend strictly
/// throughout are reversed, together with a neighbour when the boundary between them descends too.</description></item>
/// <item><description><strong>Dispatch:</strong> a quarter with more streaks than n / 256 is sorted by quadsort (skipped when it has no descending
/// pair). Adjacent non-streaky quarters are partitioned together as one range. The sorted quarters are then merged in place with rotations,
/// skipping the merges whose boundaries are already in order.</description></item>
/// <item><description><strong>Pivot:</strong> the pseudomedian of nine evenly spaced elements, copied into swap memory, for up to 2,048 elements.
/// Above that, an evenly spaced sample of 32 · 2^k elements (the smallest such that the sample's cube is at least the range, at most the swap
/// memory) is copied into swap memory and trimmed four at a time to its middle half, the two halves of what remains are sorted with quadsort,
/// and the median of the two is found by binary search. A sample whose last element does not exceed its first marks low-cardinality data,
/// and the range is sorted with quadsort instead.</description></item>
/// <item><description><strong>Stable partition:</strong> a piece of at most 512 elements is read in order; each element is written to the next
/// slot at the front of the piece and the next slot of swap memory, and the cursor of the side it belongs to advances. The greater elements
/// are then copied back after the others. A longer range partitions its two halves the same way and rotates the greater part of the first
/// half past the not-greater part of the second, so both sides keep their input order.</description></item>
/// <item><description><strong>Recursion:</strong> the greater side is sorted by quadsort when it is at most 96 elements or the other side is at
/// most 1/16 of it, and partitioned otherwise. The not-greater side continues in the loop with the pivot as the bound it carries, unless it is
/// at most 96 elements or the greater side is at most 1/16 of it, which quadsort handles.</description></item>
/// <item><description><strong>Reverse partition:</strong> when the new pivot is not less than the bound the range carries, or nothing came out
/// greater than the pivot, the elements less than the pivot are separated stably from those equal to it, which are then in their final place.</description></item>
/// </list>
/// <para><strong>Implementation notes:</strong></para>
/// <list type="bullet">
/// <item><description>Configuration: this is a port of blitsort 1.2.1.2 in the configuration where the comparison is inlined (<c>#define cmp</c>),
/// which is what a struct <typeparamref name="TComparer"/> becomes after JIT specialization: the streak threshold is n / 256, and the cache
/// limit that forces quadsort for quarters above 262,144 elements does not apply. Swap memory is the default 512 elements (<c>BLIT_AUX</c>).</description></item>
/// <item><description>Partition step: the gcc form of the reference, which writes every element to both destinations and advances the two
/// cursors arithmetically, so the loop has no data-dependent branch. Each element therefore costs two writes, and the one on the other side is
/// overwritten by the next element or left unused.</description></item>
/// <item><description>Swap memory: 512 elements rented from <see cref="ArrayPool{T}"/> where the reference uses a stack array, because a stack
/// array cannot hold an arbitrary <typeparamref name="T"/>. Input of up to 132 elements goes to quadsort, which rents its own.</description></item>
/// <item><description>Buffers: 0 is the input, 1 is the swap memory, which also holds the pivot samples, and 2 is quadsort's 32-element stack
/// array. Quadsort runs with blitsort's swap memory, as the reference shares it.</description></item>
/// <item><description>Comparer contract: the partitions write every element they read to exactly one place, whatever the comparer answers.
/// Quadsort may repeat or lose elements under a comparer that is not a consistent total order, and it can throw
/// <see cref="InvalidOperationException"/>; nothing outside the buffers is read or written.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Hybrid (Partition + Merge)</description></item>
/// <item><description>Stable      : Yes (both sides of every partition keep their input order, equal elements stay on the not-greater side, the rotations move whole parts, and quadsort is stable)</description></item>
/// <item><description>In-place    : Yes (512 elements of swap memory, a constant, and O(log n) recursion)</description></item>
/// <item><description>Best case   : Θ(n) - ordered input is recognised by the analysis with about n comparisons, and strictly descending input is reversed</description></item>
/// <item><description>Average case: Θ(n log n) comparisons, O(n log² n) moves - a partition of a range longer than the swap memory rotates the middle at every halving, about n log(n / 512) moves</description></item>
/// <item><description>Worst case  : O(n log n) comparisons, O(n log² n) moves - a partition continues only on a split no worse than 1:16, otherwise quadsort or a reverse partition takes over</description></item>
/// <item><description>Comparisons : Θ(n) best, Θ(n log n) average and worst - one per element per partition, plus pivot selection of O(n^(1/3) log n) per range and quadsort's merges for the ranges handed to it</description></item>
/// <item><description>Swaps       : O(n log² n) - reversals of descending quarters and stretches, quadsort's small exchanges, and the exchanges rotations fall back to when both sides exceed swap memory; partitions move elements by writes</description></item>
/// <item><description>Index Reads : Θ(n) best, O(n log² n) average and worst - the analysis reads the pairs it counts, a partition reads every element of its range and the greater ones again from swap memory, and rotations read what they move</description></item>
/// <item><description>Index Writes: 0 best, O(n log² n) average and worst - a partition writes every element twice and its greater elements once more on the way back, and the rotations of long ranges, quadsort and reversals add their own</description></item>
/// <item><description>Space       : O(log n) - 512 elements of swap memory and quadsort's 32-element stack array, both constants, plus the recursion</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>GitHub: https://github.com/scandum/blitsort (version 1.2.1.2, public domain), with quadsort 1.2.1.3</para>
/// </remarks>
public static class BlitSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by BlitSortTests, which derives from StableSortTestsBase.</remarks>
    public static bool IsStable => true;

    // Buffer identifiers for visualization
    const int BUFFER_MAIN = 0;       // Main input array
    const int BUFFER_SWAP = 1;       // Swap memory: the greater side of a partition piece, the pivot samples, quadsort's swap

    // Ranges up to this size are sorted by quadsort without analysis.
    const int QUADSORT_MAX = 132;

    // Elements of swap memory (the reference's BLIT_AUX).
    const int BLIT_AUX = 512;

    // Partitions this short, on either side, go to quadsort (the reference's BLIT_OUT).
    const int BLIT_OUT = 96;

    // Ranges up to this size take the pseudomedian of nine. Larger ones sample about the cube root.
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

        if (n <= QUADSORT_MAX)
        {
            context.OnPhase(SortPhase.BlitSortQuadsort, first, last - 1);
            QuadSort.SortCore(s, first, n);
            return;
        }

        var rented = ArrayPool<T>.Shared.Rent(BLIT_AUX);
        try
        {
            var w = new SortSpan<T, TComparer, TContext>(rented.AsSpan(0, BLIT_AUX), context, comparer, BUFFER_SWAP);
            Analyze(s, first, w, BLIT_AUX, n);
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
        a.Context.OnPhase(SortPhase.BlitSortQuadsort, a.Offset + p, a.Offset + p + n - 1);
        QuadSort.SortSwap(a, p, w, 0, swapSize, n);
    }

    /// <summary>
    /// <c>blit_analyze</c>: decides per quarter of a[pa..pa + n) between quadsort and partitioning, after detecting fully ordered input and
    /// reversing strictly descending quarters, then merges the quarters in place.
    /// </summary>
    static void Analyze<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w, int swapSize, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        a.Context.OnPhase(SortPhase.BlitSortAnalyze, a.Offset + pa, a.Offset + pa + n - 1);

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

        // Switch a quarter to quadsort when it is more than half ordered.
        cnt = n / 256;
        var aStreaky = astreaks > cnt;
        var bStreaky = bstreaks > cnt;
        var cStreaky = cstreaks > cnt;
        var dStreaky = dstreaks > cnt;

        switch (Bit(aStreaky) + Bit(bStreaky) * 2 + Bit(cStreaky) * 4 + Bit(dStreaky) * 8)
        {
            case 0:
                Partition(a, pa, w, swapSize, n);
                return;
            case 1:
                if (abalance != 0) Quadsort(a, pa, w, swapSize, quad1);
                Partition(a, pta + 1, w, swapSize, quad2 + half2);
                break;
            case 2:
                Partition(a, pa, w, swapSize, quad1);
                if (bbalance != 0) Quadsort(a, pta + 1, w, swapSize, quad2);
                Partition(a, ptb + 1, w, swapSize, half2);
                break;
            case 3:
                if (abalance != 0) Quadsort(a, pa, w, swapSize, quad1);
                if (bbalance != 0) Quadsort(a, pta + 1, w, swapSize, quad2);
                Partition(a, ptb + 1, w, swapSize, half2);
                break;
            case 4:
                Partition(a, pa, w, swapSize, half1);
                if (cbalance != 0) Quadsort(a, ptb + 1, w, swapSize, quad3);
                Partition(a, ptc + 1, w, swapSize, quad4);
                break;
            case 8:
                Partition(a, pa, w, swapSize, half1 + quad3);
                if (dbalance != 0) Quadsort(a, ptc + 1, w, swapSize, quad4);
                break;
            case 9:
                if (abalance != 0) Quadsort(a, pa, w, swapSize, quad1);
                Partition(a, pta + 1, w, swapSize, quad2 + quad3);
                if (dbalance != 0) Quadsort(a, ptc + 1, w, swapSize, quad4);
                break;
            case 12:
                Partition(a, pa, w, swapSize, half1);
                if (cbalance != 0) Quadsort(a, ptb + 1, w, swapSize, quad3);
                if (dbalance != 0) Quadsort(a, ptc + 1, w, swapSize, quad4);
                break;
            default: // 5, 6, 7, 10, 11, 13, 14, 15
                if (aStreaky) { if (abalance != 0) Quadsort(a, pa, w, swapSize, quad1); }
                else Partition(a, pa, w, swapSize, quad1);
                if (bStreaky) { if (bbalance != 0) Quadsort(a, pta + 1, w, swapSize, quad2); }
                else Partition(a, pta + 1, w, swapSize, quad2);
                if (cStreaky) { if (cbalance != 0) Quadsort(a, ptb + 1, w, swapSize, quad3); }
                else Partition(a, ptb + 1, w, swapSize, quad3);
                if (dStreaky) { if (dbalance != 0) Quadsort(a, ptc + 1, w, swapSize, quad4); }
                else Partition(a, ptc + 1, w, swapSize, quad4);
                break;
        }

        a.Context.OnPhase(SortPhase.BlitSortMerge, a.Offset + pa, a.Offset + pa + half1 - 1, a.Offset + pa + n - 1);

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
    /// <c>blit_binary_median</c>: the median of two sorted runs of <paramref name="len"/> elements at <paramref name="pta"/> and
    /// <paramref name="ptb"/> of swap memory, found by halving both at once.
    /// </summary>
    static T BinaryMedian<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> w, int pta, int ptb, int len)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while ((len /= 2) != 0)
        {
            if (!w.IsGreaterAt(pta + len, ptb + len)) pta += len; else ptb += len;
        }
        return w.IsGreaterAt(pta, ptb, out var av, out var bv) ? av : bv;
    }

    /// <summary>
    /// <c>blit_trim_four</c>: leaves the middle two of the four elements at w[p..p + 4) in w[p + 1] and w[p + 2], with the moves of the
    /// reference.
    /// </summary>
    static void TrimFour<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> w, int p)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var x = Bit(w.IsGreaterAt(p, p + 1));
        var t = w.Read(p + 1 - x);
        w.Write(p, w.Read(p + x));
        w.Write(p + 1, t);

        x = Bit(w.IsGreaterAt(p + 2, p + 3));
        t = w.Read(p + 3 - x);
        w.Write(p + 2, w.Read(p + 2 + x));
        w.Write(p + 3, t);

        // The larger of the two minima and the smaller of the two maxima.
        x = Bit(!w.IsGreaterAt(p, p + 2)) * 2;
        w.Write(p + 2, w.Read(p + x));

        x = Bit(w.IsGreaterAt(p + 1, p + 3)) * 2;
        w.Write(p + 1, w.Read(p + 1 + x));
    }

    /// <summary>
    /// <c>blit_median_of_nine</c>: the pseudomedian of nine evenly spaced elements of a[pa..pa + n), copied into swap memory.
    /// </summary>
    static T MedianOfNine<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var z = n / 9;

        for (var x = 0; x < 9; x++)
        {
            w.Write(x, a.Read(pa + x * z));
        }

        TrimFour(w, 0);
        TrimFour(w, 4);

        w.Write(0, w.Read(5));
        w.Write(3, w.Read(8));

        TrimFour(w, 0);

        w.Write(0, w.Read(6));

        var x0 = w.IsGreaterAt(0, 1);
        var y0 = w.IsGreaterAt(0, 2);
        var z0 = w.IsGreaterAt(1, 2);

        return w.Read(Bit(x0 == y0) + Bit(y0 ^ z0));
    }

    /// <summary>
    /// <c>blit_median_of_cbrt</c>: copies an evenly spaced sample of about the cube root of a[pa..pa + n) into swap memory, trims it four at a
    /// time to its middle half, sorts the two halves of what remains with quadsort, and returns their combined median.
    /// <paramref name="generic"/> reports a sample whose last element does not exceed its first.
    /// </summary>
    static T MedianOfCbrt<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w, int swapSize, int n, out bool generic)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var cbrt = 32;
        while (n > (long)cbrt * cbrt * cbrt && cbrt < swapSize) cbrt *= 2;

        var div = n / cbrt;

        a.Context.OnPhase(SortPhase.BlitSortPivotSample, a.Offset + pa, a.Offset + pa + n - 1, cbrt);

        var pta = pa;
        for (var cnt = 0; cnt < cbrt; cnt++)
        {
            w.Write(cnt, a.Read(pta));
            pta += div;
        }

        var ptx = 0;
        var pty = cbrt / 2;

        for (var cnt = cbrt / 8; cnt != 0; cnt--)
        {
            TrimFour(w, ptx);
            TrimFour(w, pty);

            w.Write(ptx, w.Read(pty + 1));
            w.Write(ptx + 3, w.Read(pty + 2));

            ptx += 4;
            pty += 4;
        }
        cbrt /= 4;

        QuadSort.SortSwap(w, 0, w, cbrt * 2, cbrt, cbrt);
        QuadSort.SortSwap(w, cbrt, w, cbrt * 2, cbrt, cbrt);

        // The quadsort calls are over, so their detail phase must not cover the median search.
        a.Context.OnPhase(SortPhase.BlitSortPivotSample, a.Offset + pa, a.Offset + pa + n - 1, cbrt * 2);

        generic = !w.IsGreaterAt(cbrt * 2 - 1, 0);

        return BinaryMedian(w, 0, cbrt, cbrt);
    }

    /// <summary>
    /// Returns the pivot of a[pa..pa + n). <paramref name="generic"/> reports that the cube-root sample marks low-cardinality data. The work
    /// on elements stays in methods that are not inlined into the recursive <see cref="Partition"/>, so that its frames stay small.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    static T ChoosePivot<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w, int swapSize, int n, out bool generic)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (n <= MEDIAN_OF_NINE_MAX)
        {
            a.Context.OnPhase(SortPhase.BlitSortPivotSample, a.Offset + pa, a.Offset + pa + n - 1, 9);
            generic = false;
            return MedianOfNine(a, pa, w, n);
        }
        return MedianOfCbrt(a, pa, w, swapSize, n, out generic);
    }

    // ------------------------------------------------------------------
    // Partitioning
    // ------------------------------------------------------------------

    /// <summary>
    /// <c>blit_default_partition</c>: stably rearranges a[pa..pa + n) so that the elements not greater than <paramref name="piv"/> come first,
    /// and returns their count. A range longer than the swap memory partitions its halves and rotates the middle.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    static int DefaultPartition<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w, T piv, int swapSize, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (n > swapSize)
        {
            var h = n / 2;
            var l = DefaultPartition(a, pa, w, piv, swapSize, h);
            var r = DefaultPartition(a, pa + h, w, piv, swapSize, n - h);

            QuadSort.TrinityRotation(a, pa + l, w, 0, swapSize, h - l + r, h - l);

            return l + r;
        }

        var m = 0;
        var i = 0;

        for (var cnt = n / 4; cnt != 0; cnt--)
        {
            DefaultStep(a, pa, w, piv, ref i, ref m);
            DefaultStep(a, pa, w, piv, ref i, ref m);
            DefaultStep(a, pa, w, piv, ref i, ref m);
            DefaultStep(a, pa, w, piv, ref i, ref m);
        }

        for (var cnt = n % 4; cnt != 0; cnt--)
        {
            DefaultStep(a, pa, w, piv, ref i, ref m);
        }

        w.CopyTo(0, a, pa + m, n - m);

        return m;
    }

    /// <summary>
    /// One element of the default partition: written to both destinations, and the cursor of the side it belongs to advances.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void DefaultStep<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w, T piv, ref int i, ref int m)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var notGreater = !a.IsElementGreaterThan(pa + i, piv, out var v);
        a.Write(pa + m, v);
        w.Write(i - m, v);
        m += Bit(notGreater);
        i++;
    }

    /// <summary>
    /// <c>blit_reverse_partition</c>: stably rearranges a[pa..pa + n) so that the elements less than <paramref name="piv"/> come first, and
    /// returns their count. It is the default partition with the comparison turned around.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    static int ReversePartition<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w, T piv, int swapSize, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (n > swapSize)
        {
            var h = n / 2;
            var l = ReversePartition(a, pa, w, piv, swapSize, h);
            var r = ReversePartition(a, pa + h, w, piv, swapSize, n - h);

            QuadSort.TrinityRotation(a, pa + l, w, 0, swapSize, h - l + r, h - l);

            return l + r;
        }

        var m = 0;
        var i = 0;

        for (var cnt = n / 4; cnt != 0; cnt--)
        {
            ReverseStep(a, pa, w, piv, ref i, ref m);
            ReverseStep(a, pa, w, piv, ref i, ref m);
            ReverseStep(a, pa, w, piv, ref i, ref m);
            ReverseStep(a, pa, w, piv, ref i, ref m);
        }

        for (var cnt = n % 4; cnt != 0; cnt--)
        {
            ReverseStep(a, pa, w, piv, ref i, ref m);
        }

        w.CopyTo(0, a, pa + m, n - m);

        return m;
    }

    /// <summary>
    /// One element of the reverse partition: written to both destinations, and the cursor of the side it belongs to advances.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void ReverseStep<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w, T piv, ref int i, ref int m)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var less = a.IsValueGreaterThan(piv, pa + i, out var v);
        a.Write(pa + m, v);
        w.Write(i - m, v);
        m += Bit(less);
        i++;
    }

    /// <summary>
    /// Announces and runs the reverse partition of a[pa..pa + n) and returns the count of elements less than the pivot.
    /// </summary>
    static int Reverse<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w, T piv, int swapSize, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        a.Context.OnPhase(SortPhase.BlitSortReversePartition, a.Offset + pa, a.Offset + pa + n - 1);
        return ReversePartition(a, pa, w, piv, swapSize, n);
    }

    /// <summary>
    /// <c>blit_partition</c>: sorts a[pa..pa + n) stably.
    /// </summary>
    static void Partition<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int pa, SortSpan<T, TComparer, TContext> w, int swapSize, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // A nonzero aSize means max holds the pivot of the round before, which bounds every element of the range.
        var aSize = 0;
        T max = default!;

        while (true)
        {
            var piv = ChoosePivot(a, pa, w, swapSize, n, out var generic);
            if (generic)
            {
                Quadsort(a, pa, w, swapSize, n);
                return;
            }

            int sSize;

            if (aSize != 0 && !a.IsGreaterThan(max, piv))
            {
                aSize = Reverse(a, pa, w, piv, swapSize, n);
                sSize = n - aSize;

                if (sSize <= aSize / 16 || aSize <= BLIT_OUT)
                {
                    Quadsort(a, pa, w, swapSize, aSize);
                    return;
                }
                n = aSize;
                aSize = 0;
                continue;
            }

            a.Context.OnPhase(SortPhase.BlitSortPartition, a.Offset + pa, a.Offset + pa + n - 1);
            aSize = DefaultPartition(a, pa, w, piv, swapSize, n);
            sSize = n - aSize;

            if (aSize <= sSize / 16 || sSize <= BLIT_OUT)
            {
                if (sSize == 0)
                {
                    aSize = Reverse(a, pa, w, piv, swapSize, aSize);
                    sSize = n - aSize;

                    if (sSize <= aSize / 16 || aSize <= BLIT_OUT)
                    {
                        Quadsort(a, pa, w, swapSize, aSize);
                        return;
                    }
                    n = aSize;
                    aSize = 0;
                    continue;
                }
                Quadsort(a, pa + aSize, w, swapSize, sSize);
            }
            else
            {
                Partition(a, pa + aSize, w, swapSize, sSize);
            }

            if (sSize <= aSize / 16 || aSize <= BLIT_OUT)
            {
                Quadsort(a, pa, w, swapSize, aSize);
                return;
            }
            n = aSize;
            max = piv;
        }
    }
}
