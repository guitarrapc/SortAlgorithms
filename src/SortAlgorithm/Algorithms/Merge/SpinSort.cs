using SortAlgorithm.Contexts;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// SpinSortBoostは、Boost.Sortライブラリのspin_sort実装に構造的に忠実な安定ソートアルゴリズムです。
/// レベル駆動の再帰（RangeSort）とレベルパリティに基づくバッファ選択を用いたトップダウンマージソートで、
/// ceil(n/2) の補助メモリのみを使用します。
/// <br/>
/// SpinSortBoost is a stable sort algorithm structurally faithful to Boost.Sort's spin_sort.
/// It uses level-driven recursion (RangeSort) with level-parity-based buffer selection,
/// top-down merge sort with only ceil(n/2) auxiliary memory, and early exit for sorted/reversed data.
/// </summary>
/// <remarks>
/// <para><strong>Structural Fidelity to Boost SpinSort:</strong></para>
/// <list type="number">
/// <item><description><strong>Level-Driven Recursion (RangeSort):</strong> Carries a recursion level counter.
/// At odd levels, data resides in the first parameter and results go to the second.
/// At even levels, data resides in the second parameter and results stay in the second.
/// This eliminates the need for explicit pre-copies at each recursion level.</description></item>
/// <item><description><strong>NBits64 Level Computation:</strong> The recursion depth is computed as
/// <c>nbits64(ceil(n / Sort_min) - 1)</c>, matching Boost's formula. The constructor subtracts 1
/// because it performs one split level itself.</description></item>
/// <item><description><strong>Parity-Based Constructor Split:</strong> When nlevel is odd, the right (larger) half
/// is copied to the buffer and sorted first. When nlevel is even, the left (larger) half is copied.
/// This ensures <c>RangeSort</c> finds data in the correct parameter at the top level.</description></item>
/// <item><description><strong>CheckStableSort with Level Parity:</strong> For ranges &gt; 1024, checks the
/// buffer that holds data based on current level parity. At odd levels, data is in range1 and an
/// extra copy (move_forward) is needed after sorting. At even levels, data is in range2 and no copy
/// is needed.</description></item>
/// <item><description><strong>SortRangeSort:</strong> A standalone entry point that computes its own nlevel
/// and dispatches to RangeSort. Used by CheckStableSort to sort unsorted tails.</description></item>
/// <item><description><strong>MIN_CHECK Merge Probe:</strong> Boost's <c>util::merge</c> and
/// <c>util::merge_half</c> spend one comparison on merges of 1024+ elements to detect runs that are
/// already in order (or fully inverted) and replace the merge with bulk copies.</description></item>
/// <item><description><strong>Binary-Search Tail Insertion:</strong> <c>insert_partial_sort</c> locates each
/// tail element with <c>upper_bound</c> and shifts the prefix in blocks, so inserting a short unsorted
/// tail costs O(tailLen · log prefixLen) comparisons instead of one per shifted element.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Hybrid (Merge + Insertion)</description></item>
/// <item><description>Stable      : Yes (≤ comparison in all merge operations preserves relative order)</description></item>
/// <item><description>In-place    : No (requires O(ceil(n/2)) temporary space for the half buffer)</description></item>
/// <item><description>Best case   : O(n) - Already sorted, reverse sorted, or nearly-sorted data</description></item>
/// <item><description>Average case: O(n log n) - Balanced recursive merge</description></item>
/// <item><description>Worst case  : O(n log n) - Guaranteed by balanced binary split</description></item>
/// <item><description>Comparisons : Θ(n) best, O(n log n) average and worst - matching the running time Boost states for spinsort</description></item>
/// <item><description>Swaps       : O(n) - only the reversal that turns a detected descending run ascending; merging moves elements by copying</description></item>
/// <item><description>Index Reads : O(n log n) - every merge level reads each element of the runs it joins; Θ(n) when the input is one run</description></item>
/// <item><description>Index Writes: O(n log n) - every merge level writes each element it moves, into the half buffer or back; Θ(n) when the input is one run</description></item>
/// <item><description>Space       : O(ceil(n/2))</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Boost.Sort SpinSort: https://github.com/boostorg/sort/blob/develop/include/boost/sort/spinsort/spinsort.hpp</para>
/// <para>Author: Francisco José Tapia (2016), Boost Software License 1.0</para>
/// </remarks>
public static class SpinSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by SpinSortTests, which derives from StableSortTestsBase.</remarks>
    public static bool IsStable => true;

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;    // Main input array
    private const int BUFFER_TEMP = 1;    // Temporary half buffer

    // Boost's outer Sort_min = 36. Arrays with n <= Sort_min * 2 (72) use insertion sort directly.
    private const int SORT_MIN = 36;

    // Boost's inner sort_min = 32, used in RangeSort base case and CheckStableSort.
    private const int SORT_MIN_INTERNAL = 32;

    // Boost util::merge / util::merge_half MIN_CHECK: minimum combined run length for which the
    // "already ordered" probe is attempted before falling back to the element-wise merge.
    private const int MIN_CHECK = 1024;

    /// <summary>
    /// Sorts the elements in the specified span in ascending order using the default comparer.
    /// </summary>
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
        => Sort(span, 0, span.Length, new ComparableComparer<T>(), NullContext.Default);

    /// <summary>
    /// Sorts the elements in the specified span using the provided sort context.
    /// </summary>
    public static void Sort<T, TContext>(Span<T> span, TContext context)
        where T : IComparable<T>
        where TContext : ISortContext
        => Sort(span, 0, span.Length, new ComparableComparer<T>(), context);

    /// <summary>
    /// Sorts the elements in the specified span using the provided comparer and sort context.
    /// </summary>
    public static void Sort<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        => Sort(span, 0, span.Length, comparer, context);

    /// <summary>
    /// Sorts the subrange [first..last) using the provided sort context.
    /// </summary>
    public static void Sort<T, TContext>(Span<T> span, int first, int last, TContext context)
        where T : IComparable<T>
        where TContext : ISortContext
        => Sort(span, first, last, new ComparableComparer<T>(), context);

    /// <summary>
    /// Sorts the subrange [first..last) using the provided comparer and sort context.
    /// </summary>
    public static void Sort<T, TComparer, TContext>(Span<T> span, int first, int last, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        ArgumentOutOfRangeException.ThrowIfNegative(first);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(last, span.Length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(first, last);

        var n = last - first;
        if (n <= 1) return;

        // Boost: if (nelem <= (Sort_min << 1)) insert_sort
        if (n <= SORT_MIN * 2)
        {
            BinaryInsertionSort.Sort(span, first, last, comparer, context);
            return;
        }

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        SortCore(s, first, last);
    }

    /// <summary>
    /// Core algorithm, equivalent to Boost's spinsort constructor.
    /// Performs pre-sort detection, computes nlevel, splits based on nlevel parity,
    /// calls RangeSort for each half, and merge_half's the results.
    /// </summary>
    private static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = last - first;

        // Boost: check if sorted (ascending scan)
        if (IsAscending(s, first, last)) return;

        // Boost: check if reverse sorted (descending scan) + reverse
        if (IsDescending(s, first, last))
        {
            Reverse(s, first, last - 1);
            return;
        }

        var nelem1 = (n + 1) / 2; // ceil(n/2) — larger half
        var nelem2 = n - nelem1;   // floor(n/2) — smaller half

        // 葉は InsertionSort で埋められる。これを通知しないと、観測側からは
        // 挿入ソートしか見えず、その上で動いているマージ構造が伝わらない。
        s.Context.OnPhase(SortPhase.MergeInitSort, SORT_MIN_INTERNAL);

        var buf = ArrayPool<T>.Shared.Rent(nelem1);
        try
        {
            var bufS = new SortSpan<T, TComparer, TContext>(buf.AsSpan(0, nelem1), s.Context, s.Comparer, BUFFER_TEMP);

            // Boost: nlevel = nbits64(((nelem + Sort_min - 1) / Sort_min) - 1) - 1
            var nlevel = (int)(NBits64((uint)(((n + SORT_MIN - 1) / SORT_MIN) - 1)) - 1);

            if ((nlevel & 1) == 1)
            {
                // Odd nlevel: RangeSort(range1, range2, level) puts data from range1 → result in range2.
                //
                // Boost layout:
                //   range_1 = [first, first + nelem_2)   — size = nelem2 (smaller, left)
                //   range_2 = [first + nelem_2, last)    — size = nelem1 (larger, right)
                //   Copy range_2 → buf
                //
                // Step 1: RangeSort(buf, range_2, nlevel) — data in buf, result in range_2
                //         After: s[first+nelem2..last) is sorted
                // Step 2: rng_bx = buf[0..nelem2) — reuse buf as scratch for left half
                //         RangeSort(range_1, rng_bx, nlevel) — data in range_1, result in rng_bx
                //         After: buf[0..nelem2) has sorted left half
                // Step 3: MergeHalf(buf[0..nelem2), s[first+nelem2..last)) → s[first..last)

                // Copy right half to buf
                s.CopyTo(first + nelem2, bufS, 0, nelem1);

                // Sort right half: data in buf, result in s[first+nelem2..last)
                var mainRight = s.Slice(first + nelem2, nelem1, BUFFER_MAIN);
                RangeSort(bufS, 0, mainRight, 0, nelem1, nlevel);

                // Sort left half: data in s[first..first+nelem2), result in buf[0..nelem2)
                var mainLeft = s.Slice(first, nelem2, BUFFER_MAIN);
                var bufLeft = bufS.Slice(0, nelem2, BUFFER_TEMP);
                RangeSort(mainLeft, 0, bufLeft, 0, nelem2, nlevel);

                // MergeHalf: sorted left (buf, nelem2) + sorted right (s, nelem1) → s[first..last)
                s.Context.OnPhase(SortPhase.MergeSortMerge, first, first + nelem2 - 1, last - 1);
                MergeHalf(bufLeft, s, first, nelem2, nelem1);
            }
            else
            {
                // Even nlevel: RangeSort(range1, range2, level) — data in range2, result in range2.
                //
                // Boost layout:
                //   range_1 = [first, first + nelem_1)   — size = nelem1 (larger, left)
                //   range_2 = [first + nelem_1, last)    — size = nelem2 (smaller, right)
                //   Copy range_1 → buf
                //
                // Step 1: RangeSort(range_1, buf, nlevel) — data in buf, result in buf
                //         After: buf[0..nelem1) has sorted left half
                // Step 2: Resize range_1 to nelem2 for scratch
                //         RangeSort(range_1_resized, range_2, nlevel) — data in range_2, result in range_2
                //         After: s[first+nelem1..last) is sorted
                // Step 3: MergeHalf(buf[0..nelem1), s[first+nelem1..last)) → s[first..last)

                // Copy left half to buf
                s.CopyTo(first, bufS, 0, nelem1);

                // Sort left half: data in buf, result in buf
                var mainLeft = s.Slice(first, nelem1, BUFFER_MAIN);
                RangeSort(mainLeft, 0, bufS, 0, nelem1, nlevel);

                // Sort right half: data in s[first+nelem1..last), result in s[first+nelem1..last)
                // Reuse s[first..first+nelem2) as scratch (range_1 resized to nelem2)
                var mainScratch = s.Slice(first, nelem2, BUFFER_MAIN);
                var mainRight = s.Slice(first + nelem1, nelem2, BUFFER_MAIN);
                RangeSort(mainScratch, 0, mainRight, 0, nelem2, nlevel);

                // MergeHalf: sorted left (buf, nelem1) + sorted right (s, nelem2) → s[first..last)
                s.Context.OnPhase(SortPhase.MergeSortMerge, first, first + nelem1 - 1, last - 1);
                MergeHalf(bufS, s, first, nelem1, nelem2);
            }
        }
        finally
        {
            ArrayPool<T>.Shared.Return(buf, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    /// <summary>
    /// Boost's range_sort: level-driven recursive merge sort.
    /// <br/>
    /// Contract:
    /// <list type="bullet">
    /// <item><description>Odd level  — data is in range1 (first param), sorted result goes to range2 (second param).</description></item>
    /// <item><description>Even level — data is in range2 (second param), sorted result stays in range2.</description></item>
    /// </list>
    /// Both ranges must have the same size. Level must be ≥ 1.
    /// </summary>
    private static void RangeSort<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> range1, int start1,
        SortSpan<T, TComparer, TContext> range2, int start2,
        int len, int level)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Boost: check_stable_sort based on level parity
        if (len > 1024)
        {
            if ((level & 1) == 0)
            {
                // Even level: data is in range2
                if (CheckStableSort(range2, start2, start2 + len, range1, start1))
                    return;
            }
            else
            {
                // Odd level: data is in range1
                if (CheckStableSort(range1, start1, start1 + len, range2, start2))
                {
                    // Result needs to be in range2 → copy
                    range1.CopyTo(start1, range2, start2, len);
                    return;
                }
            }
        }

        // Split range1 into two halves
        var half = (len + 1) / 2;
        var right = len - half;

        if (level < 2)
        {
            // Base case: insertion sort range1's halves in-place, then merge into range2
            InsertionSort.SortCore(range1, start1, start1 + half);
            InsertionSort.SortCore(range1, start1 + half, start1 + len);
        }
        else
        {
            // Recursive: swap range1/range2 roles for sub-calls
            // range_sort(range2_half, range1_half, level-1)
            // At level-1: if odd → data in range2_half, result in range1_half
            //             if even → data in range1_half, result in range1_half
            // Either way, result ends up in range1_half. ✓
            RangeSort(range2, start2, range1, start1, half, level - 1);
            RangeSort(range2, start2 + half, range1, start1 + half, right, level - 1);
        }

        // Merge range1's two sorted halves into range2
        // 出力先 (range2) の絶対インデックスで通知する。range2 はレベルのパリティによって
        // メイン配列と一時バッファーを往復するため、Offset を足して実際の位置を伝える。
        var mergeStart = range2.Offset + start2;
        range2.Context.OnPhase(SortPhase.MergeSortMerge, mergeStart, mergeStart + half - 1, mergeStart + len - 1);
        MergeFromSrcToDst(range1, start1, half, start1 + half, right, range2, start2);
    }

    /// <summary>
    /// Boost's sort_range_sort: computes nlevel for a sub-range and dispatches to RangeSort.
    /// Used by CheckStableSort to sort unsorted tails. Ensures the sorted result ends up in rng_data.
    /// </summary>
    private static void SortRangeSort<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> data, int dataStart, int dataLen,
        SortSpan<T, TComparer, TContext> aux, int auxStart)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Boost: sort_min = 32
        if (dataLen <= SORT_MIN_INTERNAL)
        {
            InsertionSort.SortCore(data, dataStart, dataStart + dataLen);
            return;
        }

        // Boost: nlevel = nbits64(((rng_data.size() + sort_min - 1) / sort_min) - 1)
        // Note: no -1 at the end (unlike the constructor which subtracts 1)
        var nlevel = NBits64((uint)(((dataLen + SORT_MIN_INTERNAL - 1) / SORT_MIN_INTERNAL) - 1));

        if ((nlevel & 1) == 0)
        {
            // Even nlevel: data in range2 (second param = data), result stays in data ✓
            RangeSort(aux, auxStart, data, dataStart, dataLen, (int)nlevel);
        }
        else
        {
            // Odd nlevel: data in range1 (first param = data), result in range2 (aux)
            RangeSort(data, dataStart, aux, auxStart, dataLen, (int)nlevel);
            // Copy result back to data
            aux.CopyTo(auxStart, data, dataStart, dataLen);
        }
    }

    /// <summary>
    /// Boost's check_stable_sort: detects nearly-sorted data and handles it via partial insertion.
    /// Returns true if the data was fully sorted, nearly sorted (small unsorted tail after a sorted
    /// prefix), or nearly reversed (small unsorted tail after a descending prefix).
    /// </summary>
    private static bool CheckStableSort<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> data, int first, int last,
        SortSpan<T, TComparer, TContext> aux, int auxStart)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var ndata = last - first;

        // Boost: if (ndata < 32) { insert_sort; return true; }
        if (ndata < SORT_MIN_INTERNAL)
        {
            InsertionSort.SortCore(data, first, last);
            return true;
        }

        // Boost: min_insert_partial_sort = max(32, ndata >> 3)
        var minInsertPartial = Math.Max(SORT_MIN_INTERNAL, ndata >> 3);

        // Check ascending: scan for sorted prefix
        var sortedEnd = first + 1;
        while (sortedEnd < last && data.IsLessOrEqualAt(sortedEnd - 1, sortedEnd))
            sortedEnd++;

        if (sortedEnd == last) return true; // fully sorted

        // Small unsorted tail after sorted prefix
        if (last - sortedEnd < minInsertPartial)
        {
            SortTailAndInsert(data, first, sortedEnd, last, aux, auxStart);
            return true;
        }

        // Boost: check descending only if ascending failed at the very first pair
        if (sortedEnd != first + 1) return false;

        // Check strictly descending
        sortedEnd = first + 1;
        while (sortedEnd < last && data.IsLessAt(sortedEnd, sortedEnd - 1))
            sortedEnd++;

        if (last - sortedEnd >= minInsertPartial) return false;

        // Reverse the descending prefix
        Reverse(data, first, sortedEnd - 1);

        // Sort remaining tail and insert
        if (sortedEnd < last)
        {
            SortTailAndInsert(data, first, sortedEnd, last, aux, auxStart);
        }
        return true;
    }

    /// <summary>
    /// Sorts the unsorted tail data[mid..last) in-place, then merges it into the sorted prefix
    /// data[first..mid) via InsertPartialSort. Uses SortRangeSort for the tail sort to maintain
    /// structural fidelity with Boost (which calls sort_range_sort for the tail).
    /// </summary>
    private static void SortTailAndInsert<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> data, int first, int mid, int last,
        SortSpan<T, TComparer, TContext> aux, int auxStart)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var tailLen = last - mid;

        // Boost: sort_range_sort(range(it2, rng_data.last), rng_aux, comp)
        SortRangeSort(data, mid, tailLen, aux, auxStart);

        // Merge sorted prefix + sorted tail
        InsertPartialSort(data, first, mid, last, aux, auxStart);
    }

    /// <summary>
    /// Merges a small sorted tail data[mid..last) into a larger sorted prefix data[first..mid).
    /// Uses aux as temporary storage for the tail during the insertion.
    /// </summary>
    /// <remarks>
    /// Mirrors Boost's <c>insert_partial_sort</c>: each tail element's destination is located with
    /// <see cref="UpperBound"/> and the prefix elements between two consecutive destinations are
    /// shifted as a block, so the comparison count is O(tailLen · log prefixLen) instead of being
    /// proportional to the shift distance. Boost stores the destinations in a <c>std::vector</c> and
    /// then walks it backwards; processing the tail from its largest element instead makes the same
    /// traversal possible without the position array, because the destinations are monotonic.
    /// </remarks>
    private static void InsertPartialSort<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> data, int first, int mid, int last,
        SortSpan<T, TComparer, TContext> aux, int auxStart)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var tailLen = last - mid;
        if (tailLen == 0 || mid == first) return;

        // Copy the sorted tail out; data[mid..last) becomes free space for the shifted prefix.
        data.CopyTo(mid, aux, auxStart, tailLen);

        var moveLast = mid;
        for (var remaining = tailLen; remaining > 0; remaining--)
        {
            var value = aux.Read(auxStart + remaining - 1);
            var moveFirst = UpperBound(data, first, moveLast, value);

            if (moveFirst != moveLast)
            {
                // Shift the prefix block forward by the number of tail elements still to place.
                // Source and destination overlap; SortSpan.CopyTo delegates to Span<T>.CopyTo,
                // which uses Buffer.Memmove and therefore handles the overlap.
                data.CopyTo(moveFirst, data, moveFirst + remaining, moveLast - moveFirst);
            }

            data.Write(moveFirst + remaining - 1, value);
            moveLast = moveFirst;
        }
    }

    /// <summary>
    /// Binary search returning the first index in [<paramref name="first"/>..<paramref name="last"/>)
    /// where <c>s[i] &gt; <paramref name="value"/></c> (strict upper bound).
    /// </summary>
    /// <remarks>
    /// Uses <c>IsLessOrEqual(s[mid], value)</c>: advances past equal elements, so the insertion point
    /// is <em>after</em> any equal values, matching Boost's <c>std::upper_bound</c> and preserving stability.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int UpperBound<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last, T value)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (first < last)
        {
            var mid = first + ((last - first) >> 1);
            if (s.IsElementLessOrEqual(mid, value))
            {
                first = mid + 1;
            }
            else
            {
                last = mid;
            }
        }

        return first;
    }

    /// <summary>
    /// Merges two adjacent sorted ranges from src into dst (stable: takes from left on equal).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void MergeFromSrcToDst<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> src, int left1, int len1, int left2, int len2,
        SortSpan<T, TComparer, TContext> dst, int dstStart)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var li = left1;
        var ri = left2;
        var di = dstStart;
        var leftEnd = left1 + len1;
        var rightEnd = left2 + len2;

        // Boost util::merge's MIN_CHECK short-circuit: on large merges, one comparison can prove the
        // two runs are already in order (or fully inverted), turning the merge into bulk copies.
        // Keeping this inline matters: extracting it into a non-inlined helper cost ~9% on random input.
        if (len1 + len2 >= MIN_CHECK && len1 != 0 && len2 != 0)
        {
            // src[left2] >= src[leftEnd - 1]: the runs concatenate as-is.
            if (src.IsGreaterOrEqualAt(left2, leftEnd - 1))
            {
                src.CopyTo(left1, dst, di, len1);
                src.CopyTo(left2, dst, di + len1, len2);
                return;
            }

            // src[rightEnd - 1] < src[left1]: the whole right run precedes the whole left run.
            if (src.IsLessAt(rightEnd - 1, left1))
            {
                src.CopyTo(left2, dst, di, len2);
                src.CopyTo(left1, dst, di + len2, len1);
                return;
            }
        }

        while (li < leftEnd && ri < rightEnd)
        {
            var leftVal = src.Read(li);
            var rightVal = src.Read(ri);
            if (src.IsLessOrEqual(leftVal, rightVal))
            {
                dst.Write(di++, leftVal);
                li++;
            }
            else
            {
                dst.Write(di++, rightVal);
                ri++;
            }
        }

        if (li < leftEnd)
            src.CopyTo(li, dst, di, leftEnd - li);
        else if (ri < rightEnd)
            src.CopyTo(ri, dst, di, rightEnd - ri);
    }

    /// <summary>
    /// Merges buf[0..leftLen) (sorted, in auxiliary buffer) and main[mainStart+leftLen..mainStart+leftLen+rightLen)
    /// (sorted, in-place) into main[mainStart..mainStart+leftLen+rightLen).
    /// </summary>
    private static void MergeHalf<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> buf,
        SortSpan<T, TComparer, TContext> main,
        int mainStart, int leftLen, int rightLen)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var li = 0;
        var ri = mainStart + leftLen;
        var di = mainStart;
        var leftEnd = leftLen;
        var rightEnd = mainStart + leftLen + rightLen;

        // Boost util::merge_half's MIN_CHECK short-circuit. Same idea as MergeFromSrcToDst, except the
        // right run already occupies its final tail position, so the ordered case only copies the buffer.
        if (leftLen + rightLen >= MIN_CHECK && leftLen != 0 && rightLen != 0)
        {
            // main[ri] >= buf[leftEnd - 1]: the runs concatenate as-is; the right run stays put.
            if (main.IsGreaterOrEqualAcross(ri, buf, leftEnd - 1))
            {
                buf.CopyTo(0, main, di, leftLen);
                return;
            }

            // main[rightEnd - 1] < buf[0]: the whole right run precedes the buffer.
            // The move overlaps when rightLen > leftLen; Span<T>.CopyTo handles that.
            if (main.IsLessAcross(rightEnd - 1, buf, 0))
            {
                main.CopyTo(ri, main, di, rightLen);
                buf.CopyTo(0, main, di + rightLen, leftLen);
                return;
            }
        }

        while (li < leftEnd && ri < rightEnd)
        {
            var leftVal = buf.Read(li);
            var rightVal = main.Read(ri);
            if (buf.IsLessOrEqual(leftVal, rightVal))
            {
                main.Write(di++, leftVal);
                li++;
            }
            else
            {
                main.Write(di++, rightVal);
                ri++;
            }
        }

        // Copy remaining left elements (right remainder is already in place)
        if (li < leftEnd)
            buf.CopyTo(li, main, di, leftEnd - li);
    }

    /// <summary>
    /// Boost's ascending check: returns true if s[first..last) is non-decreasing.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAscending<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        for (var i = first + 1; i < last; i++)
        {
            if (s.IsGreaterAt(i - 1, i)) return false;
        }
        return true;
    }

    /// <summary>
    /// Boost's descending check: returns true if s[first..last) is strictly decreasing.
    /// Equal elements return false to preserve stability.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsDescending<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        for (var i = first + 1; i < last; i++)
        {
            if (s.IsGreaterOrEqualAt(i, i - 1)) return false;
        }
        return true;
    }

    /// <summary>Reverses the elements in s[lo..hi] (inclusive) in-place.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Reverse<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int lo, int hi)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (lo < hi)
            s.Swap(lo++, hi--);
    }

    /// <summary>
    /// Boost's nbits64: returns the number of bits needed to represent num.
    /// Equivalent to floor(log2(num)) + 1 for num > 0, and 0 for num == 0.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint NBits64(uint num)
    {
        if (num == 0) return 0;
        return (uint)(BitOperations.Log2(num) + 1);
    }
}
