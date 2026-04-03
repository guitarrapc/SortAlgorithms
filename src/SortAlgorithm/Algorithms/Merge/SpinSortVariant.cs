using SortAlgorithm.Contexts;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// SpinSortは、Boost.SortライブラリのSpinSort実装を参考にした安定ソートアルゴリズムです。
/// 昇順・降順チェックによる早期終了、ソート済みプレフィックス＋小さな未ソートテールの検出（CheckStableSort）、
/// ceil(n/2) の補助メモリのみを使用する半バッファ分割戦略、
/// そして挿入ソートをベースケースとするトップダウン型ピンポンマージにより、
/// ほぼソート済みデータに対して特に高いパフォーマンスを発揮します。
/// <br/>
/// SpinSort is a stable sort algorithm inspired by the Boost.Sort library's SpinSort.
/// It achieves high performance — especially on nearly-sorted data — through:
/// early exit via ascending/descending run detection, nearly-sorted detection with partial
/// insertion (CheckStableSort), a half-buffer strategy using only ceil(n/2) auxiliary memory,
/// and top-down ping-pong merge with insertion sort as the base case.
/// <br/>
/// <strong>Note:</strong> This implementation follows Boost SpinSort's algorithm and optimizations
/// but is not a line-for-line port. Key structural differences are documented in the remarks below.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct SpinSort:</strong></para>
/// <list type="number">
/// <item><description><strong>Small Array Optimization:</strong> Arrays with n ≤ SORT_MIN * 2 (72 elements) are sorted
/// directly with BinaryInsertionSort, avoiding the overhead of buffer allocation and recursive merging.</description></item>
/// <item><description><strong>Pre-sort Detection:</strong> Before allocating any buffer, the algorithm performs O(n) scans
/// to check if the data is already ascending (return immediately) or fully descending (reverse and return).
/// This provides O(n) best-case performance for already-sorted or reverse-sorted data.</description></item>
/// <item><description><strong>Half-Buffer Split Strategy:</strong> The array is split into a left half of ceil(n/2) elements
/// and a right half of floor(n/2) elements. A buffer of size ceil(n/2) is allocated. The left half is sorted into the buffer,
/// the right half is sorted in-place using the left portion of the main array as scratch space (since left data is now safe in
/// the buffer), and finally the two sorted halves are merged via a merge_half operation.</description></item>
/// <item><description><strong>Ping-Pong Merge Sort (SortIntoDestination):</strong> Recursively sorts src into dst by splitting
/// into halves, recursively sorting each half (swapping src/dst roles at each level), and merging. Uses SORT_MIN_INTERNAL (32)
/// as the insertion sort threshold. At each level, both src and dst must hold valid copies of the data (ping-pong precondition).</description></item>
/// <item><description><strong>Nearly-Sorted Detection (CheckStableSort):</strong> For ranges larger than 1024 elements,
/// scans for a sorted (or strictly descending) prefix with a small unsorted tail (less than max(32, n/8) elements).
/// When detected, the tail is sorted in-place and merged into the prefix via right-to-left partial insertion (InsertPartialSort),
/// achieving O(n) performance on nearly-sorted data without full recursive decomposition.</description></item>
/// <item><description><strong>Merge-Skip Optimization:</strong> After sorting both halves, if max(left) ≤ min(right),
/// the merge is skipped entirely. This applies in both SortCore (Phase 3) and SortIntoDestination (recursive merge),
/// providing O(n) copy-only performance when sorted halves are already in order.</description></item>
/// <item><description><strong>Stable Merging:</strong> All merge operations use ≤ comparison (take from left when equal),
/// preserving the relative order of equal elements throughout the sort.</description></item>
/// <item><description><strong>Half Merge (MergeHalf):</strong> When the left sorted portion is already in the buffer,
/// merging reads from the buffer (left) and the in-place right portion simultaneously, writing to the main array from the start.
/// When the left is exhausted, the right remainder is already in place — no copy needed.
/// This is safe because the write index di = mainStart + li + ri is always ≤ mainStart + nptr + ri (the right read index).</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Hybrid (Merge + Insertion)</description></item>
/// <item><description>Stable      : Yes (≤ comparison in all merge operations preserves relative order)</description></item>
/// <item><description>In-place    : No (requires O(ceil(n/2)) temporary space for the half buffer)</description></item>
/// <item><description>Best case   : O(n) - Already sorted, reverse sorted, or nearly-sorted data (pre-sort + CheckStableSort detection)</description></item>
/// <item><description>Average case: O(n log n) - Balanced recursive ping-pong merge</description></item>
/// <item><description>Worst case  : O(n log n) - Guaranteed by balanced binary split</description></item>
/// <item><description>Comparisons : Best O(n), Average/Worst O(n log n)</description></item>
/// <item><description>Writes      : Best O(1), Average/Worst O(n log n)</description></item>
/// <item><description>Space       : O(ceil(n/2)) — half of PingpongMergeSort's O(n) requirement</description></item>
/// </list>
/// <para><strong>Comparison with Related Algorithms:</strong></para>
/// <list type="bullet">
/// <item><description>vs TimSort/PowerSort: Less adaptive to natural runs but simpler merge logic and lower memory</description></item>
/// <item><description>vs PingpongMergeSort: Uses half the auxiliary memory (ceil(n/2) vs n)</description></item>
/// <item><description>vs BottomupMergeSort: Top-down recursion with insertion sort base case instead of size-1 bottom-up</description></item>
/// </list>
/// <para><strong>Differences from Boost SpinSort:</strong></para>
/// <list type="bullet">
/// <item><description>Boost uses a level-driven <c>range_sort</c> that carries a recursion level counter and selects
/// src/dst by parity. This implementation uses <c>SortIntoDestination(src, dst)</c> which always checks/sorts dst,
/// relying on the stronger ping-pong precondition (dst = copy of src) to eliminate level tracking.</description></item>
/// <item><description>Boost's <c>check_stable_sort</c> checks src or dst based on the current level's parity and may need
/// a <c>move_forward</c> copy. This implementation always checks dst (which already holds valid data), avoiding the copy.</description></item>
/// <item><description>Boost uses separate <c>Sort_min = 36</c> (outer cutoff) and <c>sort_min = 32</c> (inner recursive threshold).
/// This implementation mirrors that with <c>SORT_MIN = 36</c> and <c>SORT_MIN_INTERNAL = 32</c>.</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Boost.Sort SpinSort: https://github.com/boostorg/sort/blob/develop/include/boost/sort/spinsort/spinsort.hpp</para>
/// <para>Author: Francisco José Tapia (2016), Boost Software License 1.0</para>
/// </remarks>
public static class SpinSortVariant
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;    // Main input array
    private const int BUFFER_TEMP = 1;    // Temporary half buffer

    // Threshold: arrays with n <= SORT_MIN * 2 use BinaryInsertionSort directly.
    // Matches Boost's outer Sort_min = 36 (Sort_min << 1 = 72 is the cutoff).
    private const int SORT_MIN = 36;

    // Boost's internal sort_min = 32, used in recursive sort (range_sort) and check_stable_sort.
    // Distinct from outer SORT_MIN = 36 which controls when BinaryInsertionSort is used at the top level.
    private const int SORT_MIN_INTERNAL = 32;

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
    /// <param name="context">The sort context for tracking statistics and visualization. Cannot be null.</param>
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
    /// <param name="context">The sort context for tracking statistics and visualization. Cannot be null.</param>
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
    /// <param name="context">The sort context for tracking statistics and visualization. Cannot be null.</param>
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
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="span">The span containing elements to sort.</param>
    /// <param name="first">The inclusive start index of the range to sort.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    /// <param name="comparer">The comparer used to compare elements.</param>
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

        // Small arrays: use binary insertion sort directly (matches Boost Sort_min * 2 = 72)
        if (n <= SORT_MIN * 2)
        {
            BinaryInsertionSort.Sort(span, first, last, comparer, context);
            return;
        }

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        SortCore(s, first, last);
    }

    /// <summary>
    /// Core SpinSort implementation.
    /// Performs the three-phase algorithm: pre-sort detection, half-buffer sort, and half merge.
    /// </summary>
    private static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = last - first;

        // Single-pass pre-sort detection: avoids the two-scan overhead of separate ascending/descending checks.
        // Returns 1 (ascending → return), -1 (strictly descending → reverse), 0 (unsorted → sort).
        switch (CheckPreSorted(s, first, last))
        {
            case 1: return;
            case -1: Reverse(s, first, last - 1); return;
        }

        // nptr  = ceil(n/2): left half size — fits exactly in the half buffer
        // nptr2 = floor(n/2): right half size
        var nptr = (n + 1) / 2;
        var nptr2 = n - nptr;

        var buf = ArrayPool<T>.Shared.Rent(nptr);
        try
        {
            var bufS = new SortSpan<T, TComparer, TContext>(buf.AsSpan(0, nptr), s.Context, s.Comparer, BUFFER_TEMP);

            // Phase 1: Sort left half s[first..first+nptr) into bufS[0..nptr).
            // Slice to 0-based views for clean ping-pong indexing.
            // Copy mainLeft → bufS first to satisfy the ping-pong precondition (both must hold valid data).
            var mainLeft = s.Slice(first, nptr, BUFFER_MAIN);
            mainLeft.CopyTo(0, bufS, 0, nptr);
            SortIntoDestination(mainLeft, 0, bufS, 0, nptr);
            // After: bufS[0..nptr) is sorted. mainLeft may be modified (used as scratch during recursion).

            // Phase 2: Sort right half s[first+nptr..last) in-place.
            // mainLeft data is now safe in bufS, so s[first..first+nptr2) is free to use as scratch.
            // Copy mainRight → mainScratch to satisfy the ping-pong precondition.
            // mainScratch uses the first nptr2 positions of the original left half.
            // This is always valid because nptr2 = floor(n/2) <= ceil(n/2) = nptr.
            var mainRight = s.Slice(first + nptr, nptr2, BUFFER_MAIN);
            var mainScratch = s.Slice(first, nptr2, BUFFER_MAIN);
            mainRight.CopyTo(0, mainScratch, 0, nptr2);
            SortIntoDestination(mainScratch, 0, mainRight, 0, nptr2);
            // After: mainRight (= s[first+nptr..last)) is sorted. mainScratch may be modified.

            // Phase 3: Merge bufS[0..nptr) (sorted left) + s[first+nptr..last) (sorted right) → s[first..last).
            // Near-sorted fast path: if max(left) <= min(right), the two halves are already in order.
            // Just copy the buffer into the left side of main — right is already in place.
            var rightMin = s.Read(first + nptr);
            if (bufS.Compare(nptr - 1, rightMin) <= 0)
            {
                bufS.CopyTo(0, s, first, nptr);
            }
            else
            {
                MergeHalf(bufS, s, first, nptr, nptr2);
            }
        }
        finally
        {
            ArrayPool<T>.Shared.Return(buf, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    /// <summary>
    /// Sorts src[srcStart..srcStart+len) and writes the sorted result to dst[dstStart..dstStart+len)
    /// using top-down ping-pong merge sort with insertion sort as the base case.
    /// <br/>
    /// PRECONDITION: dst[dstStart..dstStart+len) must contain a valid copy of src[srcStart..srcStart+len).
    /// This is established by SortCore's explicit copy before the first call, and maintained inductively:
    /// each sub-call receives (dst_parent, src_parent) as its (new_src, new_dst); since src_parent = dst_parent
    /// by the parent's precondition, the sub-call's src = dst holds as well.
    /// <br/>
    /// At each recursion level, src and dst swap roles so that sorted sub-results accumulate in src,
    /// ready for the final merge into dst.
    /// </summary>
    private static void SortIntoDestination<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> src, int srcStart,
        SortSpan<T, TComparer, TContext> dst, int dstStart,
        int len)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Base case: dst already holds valid data (ping-pong invariant) — sort dst directly.
        // Constraint: uses Boost's internal sort_min = 32 (not outer Sort_min = 36).
        if (len <= SORT_MIN_INTERNAL)
        {
            InsertionSort.SortCore(dst, dstStart, dstStart + len);
            return;
        }

        // Boost's check_stable_sort: detect nearly-sorted data and handle via partial insertion.
        // Only for large ranges (> 1024) where the overhead of scanning is worthwhile.
        if (len > 1024 && CheckStableSort(dst, dstStart, dstStart + len, src, srcStart))
            return;

        var half = (len + 1) / 2;
        var right = len - half;

        // Recursive ping-pong: swap src/dst roles so each half ends up sorted in src.
        // After these calls, src[srcStart..srcStart+half) and src[srcStart+half..srcStart+len) are sorted.
        SortIntoDestination(dst, dstStart, src, srcStart, half);
        SortIntoDestination(dst, dstStart + half, src, srcStart + half, right);

        // Near-sorted fast path: if max(left) <= min(right) the two sorted halves are already
        // in order relative to each other — skip the merge and just copy src to dst.
        if (src.IsLessOrEqualAt(srcStart + half - 1, srcStart + half))
        {
            src.CopyTo(srcStart, dst, dstStart, len);
            return;
        }

        // Merge the two sorted src halves into dst
        MergeFromSrcToDst(src, srcStart, half, srcStart + half, right, dst, dstStart);
    }

    /// <summary>
    /// Merges two adjacent sorted ranges from src into dst (stable: takes from left on equal).
    /// src[left1..left1+len1) and src[left2..left2+len2) (both sorted) → dst[dstStart..dstStart+len1+len2).
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

        while (li < leftEnd && ri < rightEnd)
        {
            var leftVal = src.Read(li);
            var rightVal = src.Read(ri);
            // Stability: take from left when equal (≤ comparison)
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
    /// <br/>
    /// When the left is exhausted, the right remainder is already in the correct position — no copy needed.
    /// Safety invariant: write index di = mainStart + li + ri ≤ mainStart + leftLen + ri (right read index),
    /// so writes never overtake right reads.
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

        while (li < leftEnd && ri < rightEnd)
        {
            var leftVal = buf.Read(li);
            var rightVal = main.Read(ri);
            // Stability: take from left when equal (≤ comparison)
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
    /// Single-pass pre-sort check. Scans s[first..last) once, tracking both ascending and strictly
    /// descending order simultaneously. Exits as soon as neither can hold.
    /// <list type="bullet">
    /// <item><description>Returns  1 : non-decreasing (ascending) — no action needed.</description></item>
    /// <item><description>Returns -1 : strictly decreasing — reverse to obtain ascending order.
    /// Equal elements disable the strictly-descending path (c == 0 sets desc = false) to preserve stability.</description></item>
    /// <item><description>Returns  0 : neither — full sort required.</description></item>
    /// </list>
    /// </summary>
    private static int CheckPreSorted<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var asc = true;
        var desc = true;
        for (var i = first + 1; i < last; i++)
        {
            if (s.IsGreaterAt(i - 1, i)) asc = false;   // prev > curr: not non-decreasing
            else desc = false;         // prev <= curr: not strictly decreasing (handles equal too)
            if (!asc && !desc) return 0;
        }
        return asc ? 1 : -1;
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
    /// Boost's check_stable_sort: detects nearly-sorted data and handles it via partial insertion.
    /// Returns true if the data was fully sorted, nearly sorted (small unsorted tail after a sorted
    /// prefix), or nearly reversed (small unsorted tail after a descending prefix). In all true cases,
    /// data[first..last) is sorted in-place. Returns false if full recursive sort is needed.
    /// <br/>
    /// Threshold: unsorted tail must be less than max(32, n/8) elements.
    /// Equal elements break the descending path to preserve stability.
    /// </summary>
    private static bool CheckStableSort<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> data, int first, int last,
        SortSpan<T, TComparer, TContext> aux, int auxStart)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var ndata = last - first;
        if (ndata < SORT_MIN_INTERNAL)
        {
            InsertionSort.SortCore(data, first, last);
            return true;
        }

        // Boost: max unsorted tail length before falling back to full sort
        var minInsertPartial = Math.Max(SORT_MIN_INTERNAL, ndata >> 3);

        // Check ascending: scan for sorted prefix
        var sortedEnd = first + 1;
        while (sortedEnd < last && data.IsLessOrEqualAt(sortedEnd - 1, sortedEnd))
            sortedEnd++;

        if (sortedEnd == last) return true; // fully sorted

        // Small unsorted tail → sort tail + partial insert
        if (last - sortedEnd < minInsertPartial)
        {
            SortTailAndInsert(data, first, sortedEnd, last, aux, auxStart);
            return true;
        }

        // Check descending only if ascending failed at the very first pair
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
    /// data[first..mid) via InsertPartialSort. Uses aux as scratch space for the tail sort.
    /// </summary>
    private static void SortTailAndInsert<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> data, int first, int mid, int last,
        SortSpan<T, TComparer, TContext> aux, int auxStart)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var tailLen = last - mid;

        // Sort the tail in-place
        if (tailLen <= SORT_MIN_INTERNAL)
        {
            InsertionSort.SortCore(data, mid, last);
        }
        else
        {
            // Use aux as scratch for ping-pong sort: copy tail → aux, then sort aux → data
            data.CopyTo(mid, aux, auxStart, tailLen);
            SortIntoDestination(aux, auxStart, data, mid, tailLen);
        }

        // Merge sorted prefix + sorted tail
        InsertPartialSort(data, first, mid, last, aux, auxStart);
    }

    /// <summary>
    /// Boost's insert_partial_sort equivalent: merges a small sorted tail data[mid..last) into
    /// a larger sorted prefix data[first..mid) using right-to-left merge.
    /// <br/>
    /// Safety: write index di = mid-1 + (tail elements consumed) never overtakes prefix read index ai
    /// until all tail elements are placed, because the gap between di and ai equals the remaining tail count.
    /// <br/>
    /// Stability: when prefix[ai] == tail[bi], takes from tail (placed at higher index);
    /// prefix element is later placed at a lower index, preserving original order.
    /// </summary>
    private static void InsertPartialSort<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> data, int first, int mid, int last,
        SortSpan<T, TComparer, TContext> aux, int auxStart)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var tailLen = last - mid;
        if (tailLen == 0 || mid == first) return;

        // Copy sorted tail to aux for the merge
        data.CopyTo(mid, aux, auxStart, tailLen);

        // Right-to-left merge: prefix (data[first..mid)) + tail (aux[auxStart..+tailLen))
        var ai = mid - 1;
        var bi = tailLen - 1;
        var di = last - 1;

        while (bi >= 0)
        {
            if (ai >= first)
            {
                var aVal = data.Read(ai);
                var bVal = aux.Read(auxStart + bi);
                if (data.IsGreaterThan(aVal, bVal))
                {
                    data.Write(di--, aVal);
                    ai--;
                }
                else
                {
                    data.Write(di--, bVal);
                    bi--;
                }
            }
            else
            {
                // Prefix exhausted: copy remaining tail elements from aux
                aux.CopyTo(auxStart, data, first, bi + 1);
                break;
            }
        }
    }
}
