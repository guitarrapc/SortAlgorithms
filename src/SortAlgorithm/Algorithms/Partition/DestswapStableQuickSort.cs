using SortAlgorithm.Contexts;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// DestSwap 方式の安定ソート版クイックソートです。
/// 分割時に less を出力先バッファへ、geq をスクラッチバッファへ直接書き込み、再帰ごとに出力先とスクラッチの役割を交換することで、各レベルでのコピーバックを不要にした安定 2-way パーティションを行います。
/// 左半分の前方スキャンと右半分の後方スキャンを交互に実行し、Glidesort の擬似メディアンでピボットを選択します。他のアルゴリズムへの切り替えは行わず、最悪計算量は O(n²) です。
/// <br/>
/// A stable variant of quicksort using the DestSwap strategy.
/// The stable 2-way partition writes less elements directly into the destination buffer and geq elements into the scratch buffer, and recursive calls swap the destination/scratch roles so that no copy-back is needed at each level.
/// It interleaves a forward scan over the left half with a backward scan over the right half, and selects the pivot with glidesort's pseudo-median.
/// It does not switch to another algorithm, so its worst case is O(n²) like other quicksorts.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct DestSwap Stable QuickSort:</strong></para>
/// <list type="number">
/// <item><description><strong>Pseudo-Median Pivot Selection (glidesort):</strong> Three regions of n/8 elements start at a = 0,
/// b = n/2 - n/8 and c = n - n/8, each moved into the other half of the split input when its own half is too short.
/// For n below the pseudo-median threshold (64) the pivot is the median of the three elements at a, b and c (2-3 comparisons).
/// From the threshold on, each region is first reduced to the median of three of its own subregions an eighth of its size, recursively
/// while a region holds at least 8 elements, so about n^0.528 elements are sampled. Below 8 elements c is the last element.</description></item>
/// <item><description><strong>DestSwap Stable 2-way Partitioning (no copy-back):</strong> The logical input is a concatenation of two half-spans
/// that may reside in either the main span or the scratch span. A single interleaved pass scans the left half forward and the right half backward,
/// writing four groups directly to their final regions:
/// <list type="bullet">
/// <item><description>less_fwd — front of the destination region, written in scan order</description></item>
/// <item><description>geq_bwd — back of the destination region, written in reverse scan order</description></item>
/// <item><description>geq_fwd — front of the scratch region, written in scan order</description></item>
/// <item><description>less_bwd — back of the scratch region, written in reverse scan order</description></item>
/// </list>
/// Recursive calls swap the destination/scratch roles, so each level writes directly into its final destination
/// and the per-level copy-back required by conventional stable quicksorts is eliminated.</description></item>
/// <item><description><strong>Partition Invariant:</strong> Upon completion of the partitioning phase with the normal (right) strategy:
/// <list type="bullet">
/// <item><description>All elements on the less side are strictly less than pivot</description></item>
/// <item><description>All elements on the geq side are greater than or equal to pivot</description></item>
/// <item><description>Relative order of elements within each side is preserved from the original array</description></item>
/// </list>
/// This invariant guarantees stability; elements equal to the pivot remain on the geq side and are separated by the strategy switch below.</description></item>
/// <item><description><strong>Duplicate Handling via Partition Strategies:</strong> Three comparison strategies control the partition predicate:
/// <list type="bullet">
/// <item><description>Right (normal): less = val &lt; pivot</description></item>
/// <item><description>Left with pivot: less = val ≤ prevPivot — activated when a partition produces no less elements,
/// re-partitioning the same data so that at least the pivot-equal elements move to the less side, guaranteeing progress</description></item>
/// <item><description>Left if equal (adaptive): used for the geq-side recursion; a new pivot is selected and the left strategy
/// activates only when newPivot ≤ prevPivot, indicating an equal-heavy region. The less side of such a range keeps the same ancestor
/// pivot, because every element of the range is at least that pivot.</description></item>
/// </list>
/// This makes arrays with many duplicate elements terminate without degrading to quadratic behavior.</description></item>
/// <item><description><strong>Recursive Subdivision with Tail Recursion Optimization:</strong> Between the less and geq sides,
/// the smaller one is sorted recursively and the larger one is sorted via iteration (tail recursion elimination),
/// keeping the recursion stack depth O(log n) even in adversarial cases.</description></item>
/// <item><description><strong>Termination:</strong> Every partition puts the pivot on the side that is not empty in its strategy, so the
/// side processed next is strictly smaller than the range, down to ranges of one element. No other algorithm takes over, so a run of bad
/// pivots costs O(n²) as in any quicksort.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Partitioning (Divide and Conquer)</description></item>
/// <item><description>Stable      : Yes (stable 2-way partition and order-preserving buffer roles keep equal elements in original relative order)</description></item>
/// <item><description>In-place    : No (O(n) pooled scratch buffer, O(log n) recursion stack)</description></item>
/// <item><description>Best case   : Θ(n log n) - Occurs when pivot consistently divides array into balanced partitions; Θ(n) when all elements are equal</description></item>
/// <item><description>Average case: Θ(n log n) - the pseudo-median splits near the median</description></item>
/// <item><description>Worst case  : O(n²) - pivots that keep splitting off a constant number of elements</description></item>
/// <item><description>Comparisons : Θ(n log n) average, O(n²) worst - the 2-way partition costs 1 comparison per element per level, plus the pseudo-median's samples and, on a range checked for equal keys, one comparison of its pivot with the previous one</description></item>
/// <item><description>Swaps       : 0 - the destination-swap partition moves elements by assignment between buffer roles, never by exchange</description></item>
/// <item><description>Index Reads : Θ(n log n) average, O(n²) worst - every partition level reads each element of the range it splits</description></item>
/// <item><description>Index Writes: Θ(n log n) average, O(n²) worst - each element is written once per partition level, with no copy-back pass; the left-strategy step copies the less elements left in scratch to their destination</description></item>
/// <item><description>Space       : O(n) pooled scratch buffer + O(log n) recursion stack</description></item>
/// <item><description>Copies      : ~n log₂ n (average) - Each element is written once per partition level; no per-level copy-back</description></item>
/// </list>
/// <para><strong>Comparer contract:</strong></para>
/// <para>A consistent comparer puts the pivot itself on the less side of a left partition and on the geq side of a right one, which is what
/// makes every step smaller than its range. When a comparer that is not a consistent total order breaks that, the range is gathered into
/// its destination and left unsorted instead of being partitioned forever; other ranges may still be waiting in scratch, so the sort does
/// not stop. Every element is written to exactly one place per partition, so the range keeps its elements, in an unspecified order.</para>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Quicksort</para>
/// <para>Glidesort (partition, pseudo-median and duplicate strategies): https://github.com/orlp/glidesort</para>
/// </remarks>
public static class DestswapStableQuickSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by DestswapStableQuickSortTests, which derives from StableSortTestsBase.</remarks>
    public static bool IsStable => true;

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Temporary merge buffer

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
    /// </summary>
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int right, Span<T> scratch)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = right - left + 1;
        var t = new SortSpan<T, TComparer, TContext>(scratch, s.Context, s.Comparer, BUFFER_TEMP);
        var half = n / 2;
        SortInto(s, t,
            leftInMain: true, leftOff: left, leftLen: half,
            rightInMain: true, rightOff: left + half, rightLen: n - half,
            destStart: left, scrStart: 0,
            strategy: STRATEGY_RIGHT, prevPivot: default!);
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
    /// A range of one element is copied into its destination and needs nothing else.
    /// </summary>
    private static void SortInto<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        bool leftInMain, int leftOff, int leftLen,
        bool rightInMain, int rightOff, int rightLen,
        int destStart, int scrStart,
        byte strategy, T prevPivot)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (true)
        {
            var n = leftLen + rightLen;

            // --- Base case: a range of at most one element is sorted once it is in its destination ---
            if (n <= 1)
            {
                if (leftLen == 1)
                {
                    if (!leftInMain) t.CopyTo(leftOff, s, destStart, 1);
                    else if (leftOff != destStart) s.CopyTo(leftOff, s, destStart, 1);
                }
                else if (rightLen == 1)
                {
                    if (!rightInMain) t.CopyTo(rightOff, s, destStart, 1);
                    else if (rightOff != destStart) s.CopyTo(rightOff, s, destStart, 1);
                }
                return;
            }

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

            s.Context.OnPhase(SortPhase.QuickSortPartition, destStart, destStart + n - 1, -1);

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

            // The pivot is one of the elements, so a consistent comparer puts it on the geq side of a right partition and on the less side of
            // a left one. A comparer that does not would leave the side that is processed next as large as the range, forever. Other ranges
            // may still be waiting in scratch, so the sort cannot stop here; this range is gathered into its destination and left as it is.
            if (partitionLeft ? lessTotal == 0 : geqTotal == 0)
            {
                if (lessBwdCount > 0) t.CopyTo(scrStart + n - lessBwdCount, s, destStart + lessFwdCount, lessBwdCount);
                if (geqFwdCount > 0) t.CopyTo(scrStart, s, destStart + lessTotal, geqFwdCount);
                return;
            }

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
            // The geq side checks its next pivot against this one. The less side keeps the ancestor pivot this range was checked
            // against: every element here is at least that pivot, so a next pivot not above it still marks a run of equal elements.
            var lessStrategy = strategy == STRATEGY_LEFT_IF_EQUAL ? STRATEGY_LEFT_IF_EQUAL : STRATEGY_RIGHT;
            var lessPrevPivot = strategy == STRATEGY_LEFT_IF_EQUAL ? prevPivot : default!;
            if (lessTotal <= geqTotal)
            {
                if (lessTotal > 0)
                {
                    SortInto(s, t,
                        true, destStart, lessFwdCount,
                        false, scrStart + n - lessBwdCount, lessBwdCount,
                        destStart, scrStart + geqTotal,
                        lessStrategy, lessPrevPivot);
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
                        STRATEGY_LEFT_IF_EQUAL, pivot);
                }
                // Tail-call on larger (less) side
                strategy = lessStrategy;
                prevPivot = lessPrevPivot;
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
    /// Selects the pivot value with glidesort's pseudo-median. a, b and c start three regions of n/8 elements, each placed inside one of the
    /// two halves of the split input: a at the start, b at n/2 - n/8 and c at n - n/8, moved into the other half when theirs is too short.
    /// Below <see cref="PSEUDO_MEDIAN_REC_THRESHOLD"/> the pivot is the median of the three elements at a, b and c; from it on each region
    /// is reduced recursively by <see cref="Median3RecIdx"/>. Below 8 elements the regions are empty, and c is the last element.
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
        var a = leftLen >= eighth ? 0 : leftLen;
        var b = leftLen >= n / 2 ? n / 2 - eighth : n - n / 2;
        var c = rightLen >= eighth ? n - Math.Max(eighth, 1) : leftLen - eighth;

        if (n < PSEUDO_MEDIAN_REC_THRESHOLD)
            return Median3Value(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, a, b, c);

        var m = Median3RecIdx(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, a, b, c, eighth);
        return ReadSplitInput(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, m);
    }

    /// <summary>
    /// The median of three regions of <paramref name="regionLen"/> elements starting at a, b and c. A region of at least 8 elements is
    /// first reduced to the median of three of its own subregions an eighth of its size, so the sample grows as about n^0.528.
    /// </summary>
    private static int Median3RecIdx<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> t,
        bool leftInMain, int leftOff, int leftLen,
        bool rightInMain, int rightOff,
        int a, int b, int c, int regionLen)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (regionLen * 8 >= PSEUDO_MEDIAN_REC_THRESHOLD)
        {
            var n8 = regionLen / 8;
            a = Median3RecIdx(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, a, a + n8 * 4, a + n8 * 7, n8);
            b = Median3RecIdx(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, b, b + n8 * 4, b + n8 * 7, n8);
            c = Median3RecIdx(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, c, c + n8 * 4, c + n8 * 7, n8);
        }
        return Median3Idx(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, a, b, c);
    }

    /// <summary>
    /// Returns the logical index of the median among the values at the three given logical indices.
    /// Uses 2-3 value comparisons via <see cref="SortSpan{T,TComparer,TContext}.IsLessThan(T,T)"/>.
    ///
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
    /// Uses 2-3 value comparisons.
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
