using SortAlgorithm.Contexts;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

public static class DestswapStableQuickSort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Temporary merge buffer

    // Use BottomUp merge sort for small partitions or when recursion budget is exhausted.
    // BottomUp fallback guarantees O(n log n) worst case for adversarial inputs.
    private const int SMALL_SORT = 16;

    // Recursively select a pseudo-median when n is at or above this threshold.
    // Below the threshold a plain median-of-3 at the biased sample positions is used.
    private const int PSEUDO_MEDIAN_REC_THRESHOLD = 64;

    // PartitionStrategy constants — control the comparison predicate for the 2-way partition.
    private const byte STRATEGY_RIGHT = 0;           // Normal: less = val < pivot, geq = val >= pivot
    private const byte STRATEGY_LEFT_WITH_PIVOT = 1;  // Duplicate handling: less = val <= prevPivot, geq = val > prevPivot
    private const byte STRATEGY_LEFT_IF_EQUAL = 2;    // Adaptive: select new pivot; if newPivot <= prevPivot, use left strategy

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
    /// Sorts the subrange [first..last) using the provided comparer and context.
    /// This is the full-control version with explicit TContext type parameter.
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
        if (n <= 1) return;

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        var scratchBuffer = ArrayPool<T>.Shared.Rent(n);
        try
        {
            SortCore(s, first, last - 1, scratchBuffer.AsSpan(0, n));
        }
        finally
        {
            ArrayPool<T>.Shared.Return(scratchBuffer, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    /// <summary>
    /// Entry point for DestSwap stable quicksort.
    /// Splits input in half and delegates to <see cref="SortInto"/> which writes output directly
    /// to <c>s[left..right]</c> using <c>scratch</c> as the complementary buffer — no copy-back per recursion level.
    /// Recursion limit is 2·log₂(n); exhausted budget falls back to BottomUp merge sort.
    /// </summary>
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int right, Span<T> scratch)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = right - left + 1;
        var t = new SortSpan<T, TComparer, TContext>(scratch, s.Context, s.Comparer, BUFFER_TEMP);
        var logn = 32 - BitOperations.LeadingZeroCount((uint)n);
        var half = n / 2;
        SortInto(s, t,
            leftInMain: true, leftOff: left, leftLen: half,
            rightInMain: true, rightOff: left + half, rightLen: n - half,
            destStart: left, scrStart: 0,
            recursionLimit: 2 * logn, strategy: STRATEGY_RIGHT, prevPivot: default!);
    }

    /// <summary>
    /// Recursive DestSwap stable 2-way quicksort.
    /// Input is a logical concatenation of two half-spans that may reside in either <c>s</c> (main)
    /// or <c>t</c> (scratch). Output is always written to <c>s[destStart..destStart+n)</c> with
    /// <c>t[scrStart..scrStart+n)</c> as the complementary scratch region.
    /// <para>
    /// Partition writes four groups directly without copy-back:
    /// <list type="bullet">
    /// <item><description>less_fwd  — s[destStart .. destStart+lessFwdCount)</description></item>
    /// <item><description>geq_bwd   — s[destStart+n-geqBwdCount .. destStart+n)</description></item>
    /// <item><description>geq_fwd   — t[scrStart .. scrStart+geqFwdCount)</description></item>
    /// <item><description>less_bwd  — t[scrStart+n-lessBwdCount .. scrStart+n)</description></item>
    /// </list>
    /// Recursive calls swap dest/scratch roles so each level writes directly into its destination
    /// region, avoids full copy-back at each level.
    /// </para>
    /// <para>
    /// <see cref="STRATEGY_RIGHT"/> (normal): less = val &lt; pivot.<br/>
    /// <see cref="STRATEGY_LEFT_WITH_PIVOT"/> (duplicate handling): triggered when all elements ≥ pivot;
    /// re-partitions the same data with less = val ≤ prevPivot to separate equal elements.<br/>
    /// <see cref="STRATEGY_LEFT_IF_EQUAL"/> (adaptive): used for the geq-side recursion; selects a
    /// new pivot and activates left strategy only when newPivot ≤ prevPivot (equal-heavy region).
    /// </para>
    /// When recursion budget reaches zero or n &lt; <see cref="SMALL_SORT"/>, assembles the split
    /// input into dest and applies <see cref="BottomupMergeSort.SortCore"/> as the O(n log n) fallback.
    /// </summary>
    private static void SortInto<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        bool leftInMain, int leftOff, int leftLen,
        bool rightInMain, int rightOff, int rightLen,
        int destStart, int scrStart,
        int recursionLimit, byte strategy, T prevPivot)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (true)
        {
            var n = leftLen + rightLen;

            // --- Base case: small partition or recursion budget exhausted ---
            if (n < SMALL_SORT || recursionLimit == 0)
            {
                // Assemble split input into s[destStart..destStart+n)
                if (leftLen > 0)
                {
                    if (leftInMain) { if (leftOff != destStart) s.CopyTo(leftOff, s, destStart, leftLen); }
                    else t.CopyTo(leftOff, s, destStart, leftLen);
                }
                if (rightLen > 0)
                {
                    var rightDest = destStart + leftLen;
                    if (rightInMain) { if (rightOff != rightDest) s.CopyTo(rightOff, s, rightDest, rightLen); }
                    else t.CopyTo(rightOff, s, rightDest, rightLen);
                }
                if (n > 1)
                {
                    // BottomUp fallback — contract satisfied by construction:
                    //   sLocal : s[destStart..+n)  — input already assembled above; SortCore postcondition
                    //            guarantees sorted result always lands back in sLocal.
                    //   tLocal : t[scrStart..+n)   — scrStart + n ≤ t.Length holds throughout recursion
                    //            because each split assigns (scrStart, geqTotal) to one child and
                    //            (scrStart+geqTotal, lessTotal) to the other; their n' values always
                    //            sum to n, keeping the upper bound at the original t.Length.
                    //   buffer IDs : BUFFER_MAIN=0 / BUFFER_TEMP=1 match BottomupMergeSort's constants;
                    //                relative offsets are 0-based after Slice, so no offset drift.
                    var sLocal = s.Slice(destStart, n, BUFFER_MAIN);
                    var tLocal = t.Slice(scrStart, n, BUFFER_TEMP);
                    BottomupMergeSort.SortCore(sLocal, tLocal);
                }
                return;
            }

            recursionLimit--;

            // --- Pivot selection ---
            T pivot;
            bool partitionLeft;
            if (strategy == STRATEGY_LEFT_WITH_PIVOT)
            {
                pivot = prevPivot;
                partitionLeft = true;
            }
            else
            {
                pivot = SelectPivot(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, rightLen);
                // STRATEGY_LEFT_IF_EQUAL: use left (<=) strategy when new pivot <= previous pivot,
                // which indicates the geq side contains a cluster of equal elements.
                partitionLeft = strategy == STRATEGY_LEFT_IF_EQUAL && s.IsGreaterOrEqual(prevPivot, pivot);
            }

            // --- Bidirectional 2-way stable partition ---
            // Forward scan (left half) : less → s[destFront++],  geq  → t[scrFront++]
            // Backward scan (right half): geq  → s[destBack--],  less → t[scrBack--]
            //
            // Overlap safety:
            //   Forward: destFront ≤ fwdI because we advance destFront only when writing (≤ 1 per read).
            //   Backward with rightInMain=true: bwdI and destBack start at the same index and both
            //     decrement each geq step — always a read-then-write to the same position.
            var destFront = destStart;
            var destBack = destStart + n - 1;
            var scrFront = scrStart;
            var scrBack = scrStart + n - 1;

            var fwdI = leftOff;
            var bwdI = rightOff + rightLen - 1;
            var minCount = Math.Min(leftLen, rightLen);

            // Interleaved forward+backward: load both values first to expose ILP to the JIT.
            for (var k = 0; k < minCount; k++)
            {
                var valFwd = leftInMain ? s.Read(fwdI) : t.Read(fwdI);
                var valBwd = rightInMain ? s.Read(bwdI) : t.Read(bwdI);
                bool fwdGoLess = partitionLeft ? s.IsLessOrEqual(valFwd, pivot) : s.IsLessThan(valFwd, pivot);
                bool bwdGoLess = partitionLeft ? s.IsLessOrEqual(valBwd, pivot) : s.IsLessThan(valBwd, pivot);
                if (fwdGoLess) s.Write(destFront++, valFwd); else t.Write(scrFront++, valFwd);
                if (bwdGoLess) t.Write(scrBack--, valBwd); else s.Write(destBack--, valBwd);
                fwdI++;
                bwdI--;
            }

            // Forward tail (leftLen > rightLen)
            for (; fwdI < leftOff + leftLen; fwdI++)
            {
                var val = leftInMain ? s.Read(fwdI) : t.Read(fwdI);
                bool goLess = partitionLeft ? s.IsLessOrEqual(val, pivot) : s.IsLessThan(val, pivot);
                if (goLess) s.Write(destFront++, val); else t.Write(scrFront++, val);
            }

            // Backward tail (rightLen > leftLen)
            for (; bwdI >= rightOff; bwdI--)
            {
                var val = rightInMain ? s.Read(bwdI) : t.Read(bwdI);
                bool goLess = partitionLeft ? s.IsLessOrEqual(val, pivot) : s.IsLessThan(val, pivot);
                if (goLess) t.Write(scrBack--, val); else s.Write(destBack--, val);
            }

            // Count elements in each group (no copy-back required):
            //   less_fwd : s[destStart              .. destStart+lessFwdCount)
            //   geq_bwd  : s[destStart+n-geqBwdCount .. destStart+n)
            //   geq_fwd  : t[scrStart               .. scrStart+geqFwdCount)
            //   less_bwd : t[scrStart+n-lessBwdCount .. scrStart+n)
            var lessFwdCount = destFront - destStart;
            var geqFwdCount = scrFront - scrStart;
            var geqBwdCount = destStart + n - 1 - destBack;
            var lessBwdCount = scrStart + n - 1 - scrBack;
            var lessTotal = lessFwdCount + lessBwdCount;
            var geqTotal = geqFwdCount + geqBwdCount;

            // --- All elements went to geq (none less) → switch to left strategy with same pivot ---
            // Guarantees progress: with left strategy at least the pivot element goes to less side.
            if (lessTotal == 0 && !partitionLeft)
            {
                strategy = STRATEGY_LEFT_WITH_PIVOT;
                prevPivot = pivot;
                leftInMain = false; leftOff = scrStart; leftLen = geqFwdCount;
                rightInMain = true; rightOff = destStart + n - geqBwdCount; rightLen = geqBwdCount;
                continue;
            }

            // --- PartitionLeft: less side (≤ pivot) is already in dest; recurse only on geq side ---
            if (partitionLeft)
            {
                // Assemble less_bwd from scratch tail into dest (less_fwd already at dest front)
                if (lessBwdCount > 0)
                    t.CopyTo(scrStart + n - lessBwdCount, s, destStart + lessFwdCount, lessBwdCount);

                strategy = STRATEGY_RIGHT;
                prevPivot = default!;
                leftInMain = false; leftOff = scrStart; leftLen = geqFwdCount;
                rightInMain = true; rightOff = destStart + n - geqBwdCount; rightLen = geqBwdCount;
                destStart += lessTotal;
                continue;
            }

            // --- Normal partition: recurse on smaller side, tail-call on larger ---
            // less side : left=less_fwd(s), right=less_bwd(t),
            //             dest=s[destStart..+lessTotal],  scratch=t[scrStart+geqTotal..+lessTotal]
            // geq side  : left=geq_fwd(t),  right=geq_bwd(s),
            //             dest=s[destStart+lessTotal..+geqTotal], scratch=t[scrStart..+geqTotal]
            if (lessTotal <= geqTotal)
            {
                if (lessTotal > 0)
                {
                    SortInto(s, t,
                        true, destStart, lessFwdCount,
                        false, scrStart + n - lessBwdCount, lessBwdCount,
                        destStart, scrStart + geqTotal,
                        recursionLimit, STRATEGY_RIGHT, default!);
                }
                // Tail-call on larger (geq) side
                strategy = STRATEGY_LEFT_IF_EQUAL;
                prevPivot = pivot;
                leftInMain = false; leftOff = scrStart; leftLen = geqFwdCount;
                rightInMain = true; rightOff = destStart + n - geqBwdCount; rightLen = geqBwdCount;
                destStart += lessTotal;
                continue;
            }
            else
            {
                if (geqTotal > 0)
                {
                    SortInto(s, t,
                        false, scrStart, geqFwdCount,
                        true, destStart + n - geqBwdCount, geqBwdCount,
                        destStart + lessTotal, scrStart,
                        recursionLimit, STRATEGY_LEFT_IF_EQUAL, pivot);
                }
                // Tail-call on larger (less) side
                strategy = STRATEGY_RIGHT;
                prevPivot = default!;
                leftInMain = true; leftOff = destStart; leftLen = lessFwdCount;
                rightInMain = false; rightOff = scrStart + n - lessBwdCount; rightLen = lessBwdCount;
                scrStart += geqTotal;
                continue;
            }
        }
    }

    /// <summary>
    /// Reads the element at logical index <paramref name="idx"/> from the split input
    /// (left and right half-spans concatenated logically).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T ReadSplitInput<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> t,
        bool leftInMain, int leftOff, int leftLen,
        bool rightInMain, int rightOff,
        int idx)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (idx < leftLen)
            return leftInMain ? s.Read(leftOff + idx) : t.Read(leftOff + idx);
        return rightInMain ? s.Read(rightOff + idx - leftLen) : t.Read(rightOff + idx - leftLen);
    }

    /// <summary>
    /// Selects a pivot value using Glidesort inspired pseudo-median heuristic.
    /// Samples three positions biased away from the extremes:
    /// <c>a = 0</c>, <c>b = n/2 - n/8</c>, <c>c = n - n/8</c>.
    /// <list type="bullet">
    /// <item><description>
    /// n &lt; <see cref="PSEUDO_MEDIAN_REC_THRESHOLD"/>: median-of-3 at those positions
    /// (3 reads, 2-3 comparisons).
    /// </description></item>
    /// <item><description>
    /// n ≥ threshold: each position is refined to the median of three nearby samples spaced
    /// n/64 apart, then the median of the three refined positions is returned
    /// (9 reads, 8-12 comparisons).
    /// </description></item>
    /// </list>
    /// Compared to first/middle/last, the biased positions avoid the extremes that cause
    /// degenerate pivots on pipeorgan, valley, and partially-sorted patterns.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T SelectPivot<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> t,
        bool leftInMain, int leftOff, int leftLen,
        bool rightInMain, int rightOff, int rightLen)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = leftLen + rightLen;
        var eighth = n / 8;
        var a = 0;
        var b = n / 2 - eighth;  // slightly left of centre
        var c = n - 1 - eighth;  // n/8 before the last element

        if (n < PSEUDO_MEDIAN_REC_THRESHOLD)
            return Median3Value(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, a, b, c);

        return Median3RecValue(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, a, b, c, eighth);
    }

    /// <summary>
    /// Refines three sample positions using local median-of-3, then returns the median value
    /// of the three refined positions.
    /// Each position <c>x</c> is replaced by the median index of
    /// <c>{ x, x + n8*4, x + n8*7 }</c> where <c>n8 = eighth / 8 = n / 64</c>.
    /// Bounds: the highest sampled index is <c>(n - n/8) + 7*(n/64) = 63n/64 &lt; n</c>.
    /// Called only when <c>n ≥ <see cref="PSEUDO_MEDIAN_REC_THRESHOLD"/></c>, so n8 ≥ 1.
    /// </summary>
    private static T Median3RecValue<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> t,
        bool leftInMain, int leftOff, int leftLen,
        bool rightInMain, int rightOff,
        int a, int b, int c, int eighth)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n8 = eighth / 8;  // = n / 64; ≥ 1 because n ≥ PSEUDO_MEDIAN_REC_THRESHOLD
        a = Median3Idx(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, a, a + n8 * 4, a + n8 * 7);
        b = Median3Idx(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, b, b + n8 * 4, b + n8 * 7);
        c = Median3Idx(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, c, c + n8 * 4, c + n8 * 7);
        return Median3Value(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, a, b, c);
    }

    /// <summary>
    /// Returns the logical index of the median among the values at the three given logical indices.
    /// Uses 2-3 value comparisons via <see cref="SortSpan{T,TComparer,TContext}.IsLessThan(T,T)"/>.
    /// The XOR-based selection matches Glidesort's reference implementation.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Median3Idx<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> t,
        bool leftInMain, int leftOff, int leftLen,
        bool rightInMain, int rightOff,
        int a, int b, int c)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var va = ReadSplitInput(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, a);
        var vb = ReadSplitInput(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, b);
        var vc = ReadSplitInput(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, c);
        var x = s.IsLessThan(va, vb);  // va < vb
        var y = s.IsLessThan(va, vc);  // va < vc
        if (x == y)
        {
            // va is min (x=y=true) or va is max (x=y=false): median is between vb and vc
            var z = s.IsLessThan(vb, vc);
            return (z ^ x) ? c : b;
        }
        return a;  // va is median
    }

    /// <summary>
    /// Returns the value of the median among the values at the three given logical indices.
    /// Uses 2-3 value comparisons. The XOR-based selection matches Glidesort's reference.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T Median3Value<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> t,
        bool leftInMain, int leftOff, int leftLen,
        bool rightInMain, int rightOff,
        int a, int b, int c)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var va = ReadSplitInput(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, a);
        var vb = ReadSplitInput(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, b);
        var vc = ReadSplitInput(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, c);
        var x = s.IsLessThan(va, vb);  // va < vb
        var y = s.IsLessThan(va, vc);  // va < vc
        if (x == y)
        {
            // va is min (x=y=true) or va is max (x=y=false): median is between vb and vc
            var z = s.IsLessThan(vb, vc);
            return (z ^ x) ? vc : vb;
        }
        return va;  // va is median
    }
}
