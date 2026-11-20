using SortAlgorithm.Contexts;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// Edelkamp と Weiß の BlockQuicksort（中央値√n とピボット重複チェック付きの版）です。
/// 比較の結果を分岐ではなくオフセットバッファへの書き込みと加算に変え、ブロックごとに移動することで分岐予測ミスをなくします。
/// <br/>
/// Edelkamp and Weiß's BlockQuicksort, in the variant with a median-of-√n pivot and a duplicate check
/// (blocked_double_pivot_check_mosqrt). The partition turns each comparison into a store and an increment of an offset buffer
/// instead of a branch, and moves the misplaced elements a block at a time, so that branch mispredictions do not depend on the data.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct BlockQuicksort:</strong></para>
/// <list type="number">
/// <item><description><strong>Main loop:</strong> Ranges wait on an explicit stack. A range of more than IS_THRESH = 20 elements whose
/// depth is below 2·ilogb(n) + 3 is partitioned; the larger side is pushed and the smaller one is partitioned next. A range at the depth
/// limit is sorted by heapsort, and a range of at most 20 elements by insertion sort.</description></item>
/// <item><description><strong>Pivot:</strong> the median of √n samples for more than 20,000 elements, of five medians of five for more
/// than 800, of three medians of three for more than 100, and of three otherwise. The selection leaves a sample not greater than the
/// pivot just before the range it partitions and one not less just after it, so the samples are not partitioned again: [first + 1,
/// last - 1) for the median networks, and [first + k/2, last - k/2) for the √n sample, whose upper half is moved to the end of the
/// range.</description></item>
/// <item><description><strong>Block partition:</strong> The pivot is taken out, leaving a hole. Blocks of 128 elements are scanned from
/// both ends; every offset is stored, and the count advances only for an element on the wrong side. The misplaced elements of a left and
/// a right block are exchanged as a cyclic permutation through the hole, two moves per pair. The last partial blocks are classified
/// once more, the elements still misplaced are moved to the boundary, and the pivot fills the hole there.</description></item>
/// <item><description><strong>Duplicate check:</strong> when the pivot equals one of the two samples next to the partitioned range, or a
/// range of 50 to 256 elements leaves nearly all of its last block on one side, a Lomuto scan from the pivot gathers the elements equal
/// to it. The scan goes on while at least a quarter of the elements it has seen were equal, counting the pivot, and the gathered elements
/// leave the recursion.</description></item>
/// <item><description><strong>Small ranges:</strong> the insertion sort compares each element with the first one, moves the whole prefix
/// when it is smaller, and otherwise inserts it without a bound, because the first element stops the scan.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Hybrid (Partition (base) + Heap + Insertion)</description></item>
/// <item><description>Stable      : No (partitioning moves elements across equal keys)</description></item>
/// <item><description>In-place    : Yes (an explicit stack of O(log n) ranges and two 128-entry offset buffers)</description></item>
/// <item><description>Best case   : Θ(n log n) - the median of a growing sample splits near the middle; Θ(n) when the duplicate check gathers whole ranges of equal keys</description></item>
/// <item><description>Average case: Θ(n log n) - n log₂ n + o(n log n) comparisons with the median-of-√n pivot</description></item>
/// <item><description>Worst case  : O(n log n) - the depth limit hands a range that keeps splitting badly to heapsort</description></item>
/// <item><description>Comparisons : Θ(n log n) average, O(n log n) worst - each element of a partitioned range is compared with the pivot once, plus the samples, the duplicate scans and the insertion sorts</description></item>
/// <item><description>Swaps       : O(n) average, O(n log n) worst - the median networks, the sample placement and introselect swap, and the duplicate check exchanges equal keys; the partition itself moves elements through a hole</description></item>
/// <item><description>Index Reads : Θ(n log n) - every partition level reads each element of its range once to classify it, again when it is moved, and the offset buffers are read back for the moves</description></item>
/// <item><description>Index Writes: Θ(n log n) - each misplaced element is written once per level through the hole, each classified element writes its offset into a buffer, and the insertion sorts and heapsort add their own writes</description></item>
/// <item><description>Space       : O(log n) - the explicit stack, plus 2 × 128 offsets</description></item>
/// </list>
/// <para><strong>Implementation notes:</strong></para>
/// <list type="bullet">
/// <item><description>The reference's <c>std::nth_element</c> (for the √n sample) and <c>std::partial_sort</c> (the heapsort) are
/// defined by the C++ library. The authors built with GCC, whose libstdc++ selects by introselect and sorts the heap with
/// <c>__adjust_heap</c>; this port uses the same introselect and <see cref="BottomupHeapSort"/>, whose sift is that one.</description></item>
/// <item><description>Under an observing context an offset is stored only when the count advances, and a median network exchanges two
/// elements only when they are out of order, so a consumer sees the moves the algorithm logically makes. Without an observer the offsets
/// are stored unconditionally, as in the reference.</description></item>
/// <item><description>The reference unrolls the classification loops four times; the scans here are plain loops.</description></item>
/// <item><description>A natural-order floating-point range first moves its NaNs to the front, without comparisons, as the other
/// partition sorts here do.</description></item>
/// </list>
/// <para><strong>Comparer contract:</strong></para>
/// <para>The block scans are bounded by their counts. The insertion sort's unguarded scan stops at the first element, and the partition
/// inside introselect at its pivot, which only a consistent comparer guarantees, so those cursors are also checked against their range.
/// The natural order of a primitive type is always consistent, and the checks are compiled out for it. A comparer that is not a
/// consistent total order can make the sort throw <see cref="InvalidOperationException"/>. Either way the range keeps its elements, in an
/// unspecified order, and nothing outside the range is read or written.</para>
/// <para><strong>Reference:</strong></para>
/// <para>Paper: Stefan Edelkamp, Armin Weiß, "BlockQuicksort: How Branch Mispredictions don't affect Quicksort" (2016). https://arxiv.org/abs/1604.06697</para>
/// <para>Code: blocked_double_pivot_check_mosqrt (quicksort.h, partition.h, median.h, insertionsort.h), as vendored in https://github.com/Voultapher/sort-research-rs</para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Quicksort</para>
/// </remarks>
public static class BlockQuickSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by BlockQuickSortTests, which derives from SortTestsBase.</remarks>
    public static bool IsStable => false;

    // The reference's constants: block size, the insertion-sort threshold IS_THRESH, and the size above which the pivot is the median of √n.
    const int BLOCKSIZE = 128;
    const int IS_THRESH = 20;
    const int SqrtSampleThreshold = 20000;

    // Buffer identifiers for visualization
    const int BUFFER_MAIN = 0;       // Main input array
    const int BUFFER_INDEX_L = 1;    // Left offset buffer of the block partition
    const int BUFFER_INDEX_R = 2;    // Right offset buffer of the block partition

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
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use for element comparisons.</typeparam>
    /// <typeparam name="TContext">The type of the sort context.</typeparam>
    /// <param name="span">The span containing elements to sort.</param>
    /// <param name="first">The inclusive start index of the range to sort.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    /// <param name="comparer">The comparer to use for element comparisons.</param>
    /// <param name="context">The sort context that tracks statistics and provides sorting operations.</param>
    public static void Sort<T, TComparer, TContext>(Span<T> span, int first, int last, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        ArgumentOutOfRangeException.ThrowIfNegative(first);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(last, span.Length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(first, last);

        if (last - first <= 1) return;

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);

        // A natural-order NaN prefix needs no further partitioning.
        var nanEnd = FloatingPointUtils.MoveNaNsToFront(s, first, last);

        if (nanEnd >= last)
        {
            // All values are NaN, already "sorted"
            return;
        }

        SortCore(s, nanEnd, last - 1);
    }

    /// <summary>
    /// Sorts the subrange [left..right] (inclusive).
    /// This overload accepts a SortSpan directly for use by other algorithms that already have a SortSpan instance.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use for element comparisons.</typeparam>
    /// <typeparam name="TContext">The type of the sort context.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="left">The inclusive start index of the range to sort.</param>
    /// <param name="right">The inclusive end index of the range to sort.</param>
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int right)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (right <= left) return;
        QsortDoublePivotCheck(s, left, right + 1);
    }

    /// <summary>
    /// The reference's main loop (<c>qsort_double_pivot_check</c>): ranges wait on an explicit stack with the depth at which they were
    /// made. A range is partitioned while its depth is below 2·ilogb(n) + 3 and it holds more than <see cref="IS_THRESH"/> elements; the
    /// larger side is pushed and the smaller one is partitioned next, so every pushed range is at most half of the one before it.
    /// </summary>
    private static void QsortDoublePivotCheck<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int begin, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var depthLimit = 2 * BitOperations.Log2((uint)(end - begin)) + 3;

        // Each pushed range is the larger side, so the range being partitioned at least halves per entry: 64 entries cover any length.
        Span<int> stack = stackalloc int[128];
        Span<int> depthStack = stackalloc int[64];
        stack[0] = begin;
        stack[1] = end;
        depthStack[0] = 0;
        var top = 1;
        var depth = 0;

        do
        {
            if (depth < depthLimit && end - begin > IS_THRESH)
            {
                var pivot = Partition(s, begin, end, out var pivotLength);
                if (pivot - begin > end - pivot)
                {
                    stack[2 * top] = begin;
                    stack[2 * top + 1] = pivot;
                    begin = pivot + pivotLength;
                }
                else
                {
                    stack[2 * top] = pivot + pivotLength;
                    stack[2 * top + 1] = end;
                    end = pivot;
                }
                depth++;
                depthStack[top] = depth;
                top++;
            }
            else
            {
                if (end - begin > IS_THRESH)
                {
                    s.Context.OnPhase(SortPhase.HybridToHeapSort, begin, end - 1);
                    BottomupHeapSort.SortCore(s, begin, end);
                }
                else if (end - begin > 1)
                {
                    s.Context.OnPhase(SortPhase.HybridToInsertionSort, begin, end - 1, IS_THRESH);
                    InsertionSort(s, begin, end);
                }

                top--;
                begin = stack[2 * top];
                end = stack[2 * top + 1];
                depth = depthStack[top];
            }
        } while (top != 0);
    }

    /// <summary>
    /// The reference's <c>Hoare_block_partition_mosqrt::partition</c> with duplicate check: selects the pivot by the size of
    /// [<paramref name="begin"/>, <paramref name="end"/>) and block-partitions the range without the samples that bound it. Returns the
    /// pivot position; <paramref name="pivotLength"/> elements from it on equal the pivot and leave the recursion.
    /// </summary>
    private static int Partition<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int begin, int end, out int pivotLength)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = end - begin;
        int mid;
        if (n > SqrtSampleThreshold)
        {
            var k = (int)Math.Sqrt(n);
            k += 1 - (k % 2); // make it an odd number
            mid = MedianOfK(s, begin, end, k);
            s.Context.OnPhase(SortPhase.QuickSortPartition, begin, end - 1, mid);
            return BlockPartition(s, begin + k / 2, end - k / 2, mid, out pivotLength);
        }

        if (n > 800) mid = MedianOf5MediansOf5(s, begin, end);
        else if (n > 100) mid = MedianOf3MediansOf3(s, begin, end);
        else mid = MedianOf3(s, begin, begin + n / 2, end - 1);
        s.Context.OnPhase(SortPhase.QuickSortPartition, begin, end - 1, mid);
        return BlockPartition(s, begin + 1, end - 1, mid, out pivotLength);
    }

    /// <summary>
    /// The reference's <c>hoare_block_partition_unroll_loop</c> with the check for elements equal to the pivot. Partitions
    /// [<paramref name="begin"/>, <paramref name="end"/>) around the element at <paramref name="pivotPos"/>; it needs
    /// s[begin - 1] ≤ pivot ≤ s[end], which the pivot selection leaves in place.
    /// </summary>
    private static int BlockPartition<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int begin, int end, int pivotPos, out int pivotLength)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var last = end - 1;
        var tempBegin = begin;
        Span<int> indexL = stackalloc int[BLOCKSIZE];
        Span<int> indexR = stackalloc int[BLOCKSIZE];
        var bl = new SortSpan<int, Comparer<int>, TContext>(indexL, s.Context, Comparer<int>.Default, BUFFER_INDEX_L);
        var br = new SortSpan<int, Comparer<int>, TContext>(indexR, s.Context, Comparer<int>.Default, BUFFER_INDEX_R);

        // The pivot equals one of the samples next to the range, so the sample held repeated keys.
        var doublePivotCheck = s.IsGreaterOrEqualAt(pivotPos, end) || s.IsGreaterOrEqualAt(begin - 1, pivotPos);
        pivotLength = 1;

        s.Context.OnRole(pivotPos, BUFFER_MAIN, RoleType.Pivot);
        var pivot = s.Read(pivotPos);
        if (pivotPos != last) s.Write(pivotPos, s.Read(last));
        var hole = last;
        last--;

        var numLeft = 0;
        var numRight = 0;
        var startLeft = 0;
        var startRight = 0;
        int num;

        var smallArray = last - begin + 1 <= 2 * BLOCKSIZE && last - begin > 48;

        // Main loop: classify a full block on each side whose offsets are used up, then move the misplaced pairs.
        // The predicates are written as greater-or-equal rather than as the reference's negated less-than: the negated form compiled to two
        // setcc instructions per element and measured slower. They call the comparer with the same operands.
        while (last - begin + 1 > 2 * BLOCKSIZE)
        {
            if (numLeft == 0)
            {
                startLeft = 0;
                for (var j = 0; j < BLOCKSIZE; j++)
                {
                    numLeft = AppendOffset(bl, numLeft, j, s.IsElementGreaterOrEqual(begin + j, pivot));
                }
            }
            if (numRight == 0)
            {
                startRight = 0;
                for (var j = 0; j < BLOCKSIZE; j++)
                {
                    numRight = AppendOffset(br, numRight, j, s.IsValueGreaterOrEqual(pivot, last - j));
                }
            }

            num = Math.Min(numLeft, numRight);
            if (num != 0) hole = Rearrange(s, bl, br, begin, last, startLeft, startRight, num, hole);
            numLeft -= num;
            numRight -= num;
            startLeft += num;
            startRight += num;
            if (numLeft == 0) begin += BLOCKSIZE;
            if (numRight == 0) last -= BLOCKSIZE;
        }

        if (numLeft == 0) startLeft = 0;
        if (numRight == 0) startRight = 0;

        // Final iteration: classify what is left between the cursors.
        int shiftL, shiftR;
        if (numRight == 0 && numLeft == 0)
        {
            shiftL = (last - begin + 1) / 2;
            shiftR = last - begin + 1 - shiftL;
            startLeft = 0;
            startRight = 0;
            var j = 0;
            for (; j < shiftL; j++)
            {
                numLeft = AppendOffset(bl, numLeft, j, s.IsElementGreaterOrEqual(begin + j, pivot));
                numRight = AppendOffset(br, numRight, j, s.IsValueGreaterOrEqual(pivot, last - j));
            }
            if (shiftL < shiftR)
            {
                numRight = AppendOffset(br, numRight, j, s.IsValueGreaterOrEqual(pivot, last - j));
            }
        }
        else if (numRight != 0)
        {
            shiftL = last - begin - BLOCKSIZE + 1;
            shiftR = BLOCKSIZE;
            startLeft = 0;
            for (var j = 0; j < shiftL; j++)
            {
                numLeft = AppendOffset(bl, numLeft, j, s.IsElementGreaterOrEqual(begin + j, pivot));
            }
        }
        else
        {
            shiftL = BLOCKSIZE;
            shiftR = last - begin - BLOCKSIZE + 1;
            startRight = 0;
            for (var j = 0; j < shiftR; j++)
            {
                numRight = AppendOffset(br, numRight, j, s.IsValueGreaterOrEqual(pivot, last - j));
            }
        }

        num = Math.Min(numLeft, numRight);
        if (num != 0) hole = Rearrange(s, bl, br, begin, last, startLeft, startRight, num, hole);
        numLeft -= num;
        numRight -= num;
        startLeft += num;
        startRight += num;
        if (numLeft == 0) begin += shiftL;
        if (numRight == 0) last -= shiftR;

        // Move the remaining misplaced elements of one side to the boundary, and the pivot into the hole there.
        if (numLeft != 0)
        {
            var lowerI = startLeft + numLeft - 1;
            var upper = last - begin;
            while (lowerI >= startLeft && bl.Read(lowerI) == upper)
            {
                upper--;
                lowerI--;
            }
            if (lowerI >= startLeft)
            {
                var temp = s.Read(begin + upper);
                while (lowerI >= startLeft)
                {
                    var misplaced = begin + bl.Read(lowerI);
                    s.Write(begin + upper, s.Read(misplaced));
                    upper--;
                    s.Write(misplaced, s.Read(begin + upper));
                    lowerI--;
                }
                s.Write(begin + upper, temp);
            }
            var pivotHole = begin + upper + 1;
            if (hole != pivotHole) s.Write(hole, s.Read(pivotHole));

            // Gather the elements equal to the pivot above it while at least a quarter of those seen are equal (Lomuto, Elmasry,
            // Katajainen and Stenmark).
            if (doublePivotCheck || (smallArray && numLeft >= (15 * shiftL) / 16))
            {
                var beginLomuto = pivotHole;
                var q = beginLomuto + 1;
                var countSwaps = 1u;
                var countSteps = 0u;
                while (q < end && (countSwaps << 2) > countSteps)
                {
                    if (s.IsValueGreaterOrEqual(pivot, q))
                    {
                        beginLomuto++;
                        countSwaps++;
                        if (beginLomuto != q) s.Swap(beginLomuto, q);
                    }
                    q++;
                    countSteps++;
                }
                pivotLength = beginLomuto + 1 - pivotHole;
            }

            s.Write(pivotHole, pivot);
            s.Context.OnRole(pivotPos, BUFFER_MAIN, RoleType.None);
            return pivotHole;
        }

        if (numRight != 0)
        {
            var lowerI = startRight + numRight - 1;
            var upper = last - begin;
            while (lowerI >= startRight && br.Read(lowerI) == upper)
            {
                upper--;
                lowerI--;
            }
            if (hole != last - upper) s.Write(hole, s.Read(last - upper));
            while (lowerI >= startRight)
            {
                var misplaced = last - br.Read(lowerI);
                s.Write(last - upper, s.Read(misplaced));
                upper--;
                s.Write(misplaced, s.Read(last - upper));
                lowerI--;
            }
            var pivotHole = last - upper;

            // Gather the elements equal to the pivot below it, as on the left side.
            if (doublePivotCheck || (smallArray && numRight >= (15 * shiftR) / 16))
            {
                var beginLomuto = pivotHole;
                var q = beginLomuto - 1;
                var countSwaps = 1u;
                var countSteps = 0u;
                while (q > tempBegin && (countSwaps << 2) > countSteps)
                {
                    if (s.IsElementGreaterOrEqual(q, pivot))
                    {
                        beginLomuto--;
                        countSwaps++;
                        if (beginLomuto != q) s.Swap(beginLomuto, q);
                    }
                    q--;
                    countSteps++;
                }
                pivotLength = pivotHole + 1 - beginLomuto;
                s.Write(pivotHole, pivot);
                s.Context.OnRole(pivotPos, BUFFER_MAIN, RoleType.None);
                return beginLomuto;
            }

            s.Write(pivotHole, pivot);
            s.Context.OnRole(pivotPos, BUFFER_MAIN, RoleType.None);
            return pivotHole;
        }

        // No element is left on the wrong side: the cursors met, and the pivot goes where they met.
        if (hole != begin) s.Write(hole, s.Read(begin));
        s.Write(begin, pivot);
        s.Context.OnRole(pivotPos, BUFFER_MAIN, RoleType.None);
        return begin;
    }

    /// <summary>
    /// Exchanges <paramref name="num"/> misplaced elements of the left block with as many of the right block as one cyclic permutation
    /// through the hole: two moves per pair instead of a swap's three. Returns the new hole.
    /// </summary>
    private static int Rearrange<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s,
        SortSpan<int, Comparer<int>, TContext> bl, SortSpan<int, Comparer<int>, TContext> br,
        int begin, int last, int startLeft, int startRight, int num, int hole)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var left = begin + bl.Read(startLeft);
        var right = last - br.Read(startRight);
        s.Write(hole, s.Read(left));
        s.Write(left, s.Read(right));
        for (var j = 1; j < num; j++)
        {
            left = begin + bl.Read(startLeft + j);
            s.Write(right, s.Read(left));
            right = last - br.Read(startRight + j);
            s.Write(left, s.Read(right));
        }
        return right;
    }

    /// <summary>
    /// Stores <paramref name="offset"/> at <paramref name="count"/> and advances the count when <paramref name="matches"/> is true, the
    /// reference's <c>indexL[num_left] = j; num_left += cond;</c>. Without an observer the store is unconditional, so the comparison
    /// never becomes a branch; a rejected offset is overwritten by the next one, and the count never exceeds the scan position. Under an
    /// observing context the store stays conditional, so a consumer is not told about writes the algorithm does not need.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int AppendOffset<TContext>(SortSpan<int, Comparer<int>, TContext> buffer, int count, int offset, bool matches)
        where TContext : ISortContext
    {
        if (typeof(TContext) == typeof(NullContext))
        {
            buffer.Write(count, offset);
            return count + (matches ? 1 : 0);
        }

        if (matches)
        {
            buffer.Write(count, offset);
            return count + 1;
        }
        return count;
    }

    /// <summary>
    /// The reference's insertion sort (a copy of GCC 4.7.2's <c>std::__insertion_sort</c>): an element smaller than the first one moves
    /// the whole prefix up; any other is inserted without a bound, because the first element stops the scan.
    /// </summary>
    private static void InsertionSort<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (first == last) return;

        // Only a consistent comparer stops the unguarded scan at the first element, and element access is unchecked, so the cursor is also
        // checked. The natural order of a primitive type is consistent, so the check folds away for it.
        var checkCursors = !s.UsesNaturalPrimitiveOrder;
        for (var i = first + 1; i < last; i++)
        {
            s.Context.OnPhase(SortPhase.InsertionPass, s.Offset + i, s.Offset + first, s.Offset + last - 1);
            s.Context.OnRole(s.Offset + i, s.BufferId, RoleType.RightPointer);
            if (s.IsLessAt(i, first, out var value, out _))
            {
                // std::move_backward of the prefix, one element at a time: the prefix is at most IS_THRESH long.
                for (var k = i; k > first; k--)
                {
                    s.Write(k, s.Read(k - 1));
                }
                s.Write(first, value);
            }
            else
            {
                var hole = i;
                var next = i - 1;
                while (s.IsValueLessThan(value, next, out var shifted))
                {
                    s.Write(hole, shifted);
                    hole = next;
                    next--;
                    if (checkCursors && next < first) ThrowHelper.RestoreAndThrowInconsistentComparer(s, hole, value);
                }
                if (hole != i) s.Write(hole, value);
            }
            s.Context.OnRole(s.Offset + i, s.BufferId, RoleType.None);
        }
    }

    /// <summary>The reference's <c>sort_pair</c>: orders two elements with one comparison.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SortPair<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int i1, int i2)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (s.IsLessAt(i2, i1)) s.Swap(i1, i2);
    }

    /// <summary>The reference's <c>median_of_3</c>: sorts the three elements in place and returns the middle position.</summary>
    private static int MedianOf3<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int i1, int i2, int i3)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        SortPair(s, i1, i2);
        SortPair(s, i2, i3);
        SortPair(s, i1, i2);
        return i2;
    }

    /// <summary>The reference's <c>median_of_5</c> network: sorts enough of the five elements to put their median at i3.</summary>
    private static int MedianOf5<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int i1, int i2, int i3, int i4, int i5)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        SortPair(s, i1, i2);
        SortPair(s, i4, i5);
        SortPair(s, i1, i4);
        SortPair(s, i2, i5);
        SortPair(s, i3, i4);
        SortPair(s, i2, i3);
        SortPair(s, i3, i4);
        return i3;
    }

    /// <summary>
    /// The reference's <c>median_of_3_medians_of_3</c>: medians of three groups at the start, the middle and the end, the first and last
    /// of them moved to the ends of the range, and the median of the three.
    /// </summary>
    private static int MedianOf3MediansOf3<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int begin, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = end - begin;
        var first = MedianOf3(s, begin, begin + 1, begin + 2);
        var middle = MedianOf3(s, begin + n / 2, begin + n / 2 + 1, begin + n / 2 + 2);
        var last = MedianOf3(s, end - 3, end - 2, end - 1);
        s.Swap(begin, first);
        s.Swap(last, end - 1);
        return MedianOf3(s, begin, middle, end - 1);
    }

    /// <summary>
    /// The reference's <c>median_of_5_medians_of_5</c>: medians of five groups of five spread over the range, the first and last of them
    /// moved to the ends of the range, and the median of the five.
    /// </summary>
    private static int MedianOf5MediansOf5<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int begin, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = end - begin;
        var left = begin + n / 4 - 1;
        var mid = begin + n / 2 - 2;
        var right = begin + (3 * n) / 4 - 3;
        var last = end - 5;
        var first = MedianOf5(s, begin, begin + 1, begin + 2, begin + 3, begin + 4);
        left = MedianOf5(s, left, left + 1, left + 2, left + 3, left + 4);
        mid = MedianOf5(s, mid, mid + 1, mid + 2, mid + 3, mid + 4);
        right = MedianOf5(s, right, right + 1, right + 2, right + 3, right + 4);
        last = MedianOf5(s, last, last + 1, last + 2, last + 3, last + 4);
        s.Swap(begin, first);
        s.Swap(last, end - 1);
        return MedianOf5(s, begin, left, mid, right, end - 1);
    }

    /// <summary>
    /// The reference's <c>median_of_k</c>: gathers <paramref name="k"/> samples spread over the range at its front, selects their median,
    /// and moves the samples above it to the end of the range. Returns the median's position, k/2 after the start.
    /// </summary>
    private static int MedianOfK<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int begin, int end, int k)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = end - begin;
        if (n < k + 3) return MedianOf3(s, begin, begin + n / 2, end - 1);

        var step = n / (k + 3);
        var searchLeft = begin + step;
        var searchRight = end - step;
        var place = begin;
        for (var j = 0; j < k / 2; j++)
        {
            s.Swap(place, searchLeft);
            place++;
            s.Swap(place, searchRight);
            place++;
            searchLeft += step;
            searchRight -= step;
        }
        s.Swap(place, begin + n / 2);
        place++;

        var middle = begin + (place - begin) / 2;
        NthElement(s, begin, middle, place);
        for (int from = middle + 1, to = end - k / 2; from < place; from++, to++)
        {
            s.Swap(from, to);
        }
        return middle;
    }

    /// <summary>
    /// <c>std::nth_element</c> as libstdc++ implements it (<c>__introselect</c>): median-of-3 to the front, an unguarded Hoare partition,
    /// and the side holding <paramref name="nth"/> is kept, until at most three elements remain for insertion sort. Past
    /// 2⌊log₂ n⌋ partitions libstdc++ selects with a heap; a heapsort of the range leaves the same element at nth.
    /// </summary>
    private static void NthElement<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int nth, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (first == last || nth == last) return;

        var depthLimit = 2 * BitOperations.Log2((uint)(last - first));
        while (last - first > 3)
        {
            if (depthLimit == 0)
            {
                BottomupHeapSort.SortCore(s, first, last);
                return;
            }
            depthLimit--;

            var cut = UnguardedPartitionPivot(s, first, last);
            if (cut <= nth) first = cut;
            else last = cut;
        }
        InsertionSort(s, first, last);
    }

    /// <summary>
    /// libstdc++'s <c>__unguarded_partition_pivot</c>: moves the median of s[first + 1], the middle and the last element to first, then
    /// partitions [first + 1, last) around it with unguarded scans and returns the cut.
    /// </summary>
    private static int UnguardedPartitionPivot<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var a = first + 1;
        var b = first + (last - first) / 2;
        var c = last - 1;
        if (s.IsLessAt(a, b))
        {
            if (s.IsLessAt(b, c)) s.Swap(first, b);
            else if (s.IsLessAt(a, c)) s.Swap(first, c);
            else s.Swap(first, a);
        }
        else if (s.IsLessAt(a, c)) s.Swap(first, a);
        else if (s.IsLessAt(b, c)) s.Swap(first, c);
        else s.Swap(first, b);

        // The pivot stays at first, so it is held. The median-of-3 samples stop the scans, which only a consistent comparer guarantees, so
        // the cursors are also checked against the range; the checks fold away under the natural order of a primitive type.
        var checkCursors = !s.UsesNaturalPrimitiveOrder;
        var pivot = s.Read(first);
        var lo = first + 1;
        var hi = last;
        while (true)
        {
            while (s.IsElementLessThan(lo, pivot))
            {
                lo++;
                if (checkCursors && lo == last) ThrowHelper.ThrowInconsistentComparer();
            }
            hi--;
            while (s.IsValueLessThan(pivot, hi))
            {
                hi--;
                if (checkCursors && hi < first) ThrowHelper.ThrowInconsistentComparer();
            }
            if (!(lo < hi)) return lo;
            s.Swap(lo, hi);
            lo++;
        }
    }
}
