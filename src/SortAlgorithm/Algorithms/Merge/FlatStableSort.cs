using SortAlgorithm.Contexts;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// Boost.Sortライブラリのflat_stable_sort実装に構造的に忠実な安定ソートアルゴリズムです。
/// 配列を固定サイズのブロック群として扱い、Boost の divide / sort_small / partial sorted fast path を C# の SortSpan 上に写像した安定ソートです。
/// <br/>
/// A stable sort algorithm structurally faithful to Boost.Sort's flat_stable_sort.
/// Maps the array onto fixed-size block groups and mirrors Boost's divide / sort_small / partial sorted fast paths on top of SortSpan.
/// </summary>
/// <remarks>
/// <para><strong>Structural Fidelity to Boost flat_stable_sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Block Size Selection (GetBlockSize):</strong> The block size is a power-of-two
/// chosen based on the element type size, mirroring Boost's <c>block_size_fss</c> template:
/// strings use 2^6 = 64, and other types use 2^sz[BitsSize] where <c>sz[] = {10,10,10,9,8,7,6,6}</c>.
/// This matches the <c>flat::flat_stable_sort&lt;Iter_t, Compare, Power2&gt;</c> template parameter selection.</description></item>
/// <item><description><strong>Divide — Recursive Block-Group Split:</strong> Mirrors Boost's <c>divide(itx_first, itx_last)</c>.
/// When <c>nblock &lt; 5</c>, falls through to <c>SortSmall</c>.
/// When <c>nblock &gt; 7</c>, calls <c>IsSortedForward</c> / <c>IsSortedBackward</c> for partial pre-sort detection.
/// Otherwise splits at <c>(nblock+1)&gt;&gt;1</c>, recurses on both halves, and merges via
/// <c>MergeAdjacentWithLeftBuffer</c> (corresponding to Boost's <c>merge_range_pos</c>).</description></item>
/// <item><description><strong>IsSortedForward / IsSortedBackward (Partial Pre-Sort Optimization):</strong>
/// Counts already-sorted elements from the front or back via <c>NumberStableSortedForward/Backward</c>
/// (Boost's <c>number_stable_sorted_forward/backward</c> from <c>sort_basic.hpp</c>).
/// When a large sorted prefix or suffix is found, only the unsorted region is recursively sorted then merged,
/// avoiding redundant work on already-ordered data.</description></item>
/// <item><description><strong>SortSmall — Base Case for nblock &lt; 5:</strong> Mirrors Boost's <c>sort_small</c>.
/// For <c>len ≤ SORT_MIN_INTERNAL (32)</c> uses <c>InsertionSort</c> (equivalent to Boost's <c>insert_sort</c>
/// with the same threshold of 32). For 1-2 blocks it calls <c>RangeSortData</c>; for 3-4 blocks it sorts the
/// right group via <c>RangeSortData</c>, the left group via <c>RangeSortBuffer</c>, then merges with <c>MergeHalf</c>,
/// matching Boost's asymmetric small-block path.</description></item>
/// <item><description><strong>RangeSortData / RangeSortBuffer:</strong> Alternating recursive half-buffer merge sorts,
/// corresponding to Boost's <c>range_sort_data</c> / <c>range_sort_buffer</c> pair from <c>sort_basic.hpp</c>.</description></item>
/// <item><description><strong>InsertSorted / InsertSortedBackward:</strong> The partial-sorted fast paths use stable
/// upper-bound / lower-bound insertion against a copied small side, mirroring Boost's <c>insert_sorted</c> and
/// <c>insert_sorted_backward</c> helpers instead of falling back to a generic adjacent merge.</description></item>
/// <item><description><strong>MergeAdjacentWithLeftBuffer — Half-Buffer Merge:</strong> Copies the shorter half
/// to the auxiliary buffer and merges forward or backward depending on which side is smaller. Corresponds to
/// Boost's <c>merge_half</c> / <c>merge_half_backward</c> from <c>range.hpp</c>.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Merge (Block-based)</description></item>
/// <item><description>Stable      : Yes (≤ comparison in all merge operations preserves relative order)</description></item>
/// <item><description>In-place    : No (requires O(n) auxiliary space in the worst case)</description></item>
/// <item><description>Best case   : O(n) to O(n log n), depending on whether the partial-sorted fast paths trigger</description></item>
/// <item><description>Average case: O(n log n)</description></item>
/// <item><description>Worst case  : O(n log n) — Guaranteed by balanced binary split at block granularity</description></item>
/// <item><description>Space       : O(n) temporary buffer + O(log n) recursion stack</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Boost.Sort flat_stable_sort: https://github.com/boostorg/sort/blob/develop/include/boost/sort/flat_stable_sort/flat_stable_sort.hpp</para>
/// <para>Author: Francisco José Tapia (2017), Boost Software License 1.0 https://github.com/boostorg/sort/blob/develop/doc/papers/flat_stable_sort_eng.pdf</para>
/// </remarks>
public static class FlatStableSort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;    // Main input array
    private const int BUFFER_TEMP = 1;    // Temporary half buffer

    // Boost's inner sort_min = 32, used in RangeSort base case and CheckStableSort.
    private const int SORT_MIN_INTERNAL = 32;

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

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        SortCore(s, first, last);
    }

    /// <summary>
    /// Core algorithm, equivalent to Boost's flat_stable_sort constructor.
    /// Allocates the reusable auxiliary storage needed by Boost's sort_small and half-buffer merges,
    /// then dispatches to Divide.
    /// </summary>
    private static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = last - first;
        var blockSize = GetBlockSize<T>();
        var auxLength = Math.Max((n + 1) / 2, Math.Min(n, blockSize << 1));
        var mergeBuffer = ArrayPool<T>.Shared.Rent(auxLength);
        try
        {
            var aux = new SortSpan<T, TComparer, TContext>(mergeBuffer.AsSpan(0, auxLength), s.Context, s.Comparer, BUFFER_TEMP);
            Divide(s, first, last, blockSize, aux);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(mergeBuffer, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    private static void Divide<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last, int blockSize, SortSpan<T, TComparer, TContext> aux)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var len = last - first;
        if (len <= 1)
            return;

        var nblock = CeilDiv(len, blockSize);
        if (nblock < 5)
        {
            SortSmall(s, first, last, blockSize, aux);
            return;
        }

        if (nblock > 7)
        {
            if (IsSortedForward(s, first, last, blockSize, aux))
                return;

            if (IsSortedBackward(s, first, last, blockSize, aux))
                return;
        }

        var nblock1 = (nblock + 1) >> 1;
        var mid = first + Math.Min(len, nblock1 * blockSize);

        Divide(s, first, mid, blockSize, aux);
        Divide(s, mid, last, blockSize, aux);
        MergeAdjacentWithLeftBuffer(s, first, mid, last, aux);
    }

    private static void SortSmall<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last, int blockSize, SortSpan<T, TComparer, TContext> aux)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var len = last - first;
        if (len <= 1)
            return;

        var data = s.Slice(first, len, BUFFER_MAIN);

        if (len <= SORT_MIN_INTERNAL)
        {
            InsertionSort.SortCore(data, 0, len);
            return;
        }

        var nblock = CeilDiv(len, blockSize);
        if (nblock < 3)
        {
            RangeSortData(data, aux.Slice(0, len, BUFFER_TEMP));
            return;
        }

        var nblock1 = (nblock + 1) >> 1;
        var splitLen = Math.Min(len, nblock1 * blockSize);
        var left = data.Slice(0, splitLen, BUFFER_MAIN);
        var right = data.Slice(splitLen, len - splitLen, BUFFER_MAIN);

        RangeSortData(right, aux.Slice(0, right.Length, BUFFER_TEMP));
        var leftAux = aux.Slice(0, left.Length, BUFFER_TEMP);
        RangeSortBuffer(left, leftAux);
        MergeHalf(leftAux, s, first, left.Length, right.Length);
    }

    private static bool IsSortedForward<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last, int blockSize, SortSpan<T, TComparer, TContext> aux)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var len = last - first;
        var minProcess = Math.Max(blockSize, len >> 3);
        var nsorted1 = NumberStableSortedForward(s, first, last, minProcess);
        if (nsorted1 == len)
            return true;

        if (nsorted1 == 0)
            return false;

        var nsorted2 = len - nsorted1;
        var mid = first + nsorted1;

        if (nsorted2 <= (blockSize << 1))
        {
            Divide(s, mid, last, blockSize, aux);
            InsertSorted(s, first, mid, last, aux);
            return true;
        }

        var nsorted1Adjust = nsorted1 & ~(blockSize - 1);
        Divide(s, first + nsorted1Adjust, last, blockSize, aux);
        MergeAdjacentWithLeftBuffer(s, first, first + nsorted1Adjust, last, aux);
        return true;
    }

    private static bool IsSortedBackward<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last, int blockSize, SortSpan<T, TComparer, TContext> aux)
        SortSpan<T, TComparer, TContext> s,
        int first,
        int last,
        int blockSize,
        SortSpan<T, TComparer, TContext> aux)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var len = last - first;
        var minProcess = Math.Max(blockSize, len >> 3);
        var nsorted2 = NumberStableSortedBackward(s, first, last, minProcess);
        if (nsorted2 == len)
            return true;

        if (nsorted2 == 0)
            return false;

        var nsorted1 = len - nsorted2;
        var mid = last - nsorted2;

        if (nsorted1 <= (blockSize << 1))
        {
            Divide(s, first, mid, blockSize, aux);
            InsertSortedBackward(s, first, mid, last, aux);
            return true;
        }

        var nblock1 = CeilDiv(nsorted1, blockSize);
        var nsorted1Adjust = Math.Min(len, nblock1 * blockSize);
        Divide(s, first, first + nsorted1Adjust, blockSize, aux);
        MergeAdjacentWithLeftBuffer(s, first, first + nsorted1Adjust, last, aux);
        return true;
    }

    private static int NumberStableSortedForward<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last, int minProcess)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (last - first < 2)
            return 0;

        var sortedEnd = first + 1;
        while (sortedEnd < last && s.IsLessOrEqualAt(sortedEnd - 1, sortedEnd))
            sortedEnd++;

        var nsorted = sortedEnd - first;
        if (nsorted != 1)
            return nsorted >= minProcess ? nsorted : 0;

        sortedEnd = first + 1;
        while (sortedEnd < last && s.IsLessAt(sortedEnd, sortedEnd - 1))
            sortedEnd++;

        nsorted = sortedEnd - first;
        if (nsorted < minProcess)
            return 0;

        Reverse(s, first, sortedEnd - 1);
        return nsorted;
    }

    private static int NumberStableSortedBackward<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last, int minProcess)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (last - first < 2)
            return 0;

        var it = last - 1;
        while (it != first && s.IsLessOrEqualAt(it - 1, it))
            it--;

        var nsorted = last - it;
        if (nsorted != 1)
            return nsorted >= minProcess ? nsorted : 0;

        it = last - 1;
        while (it != first && s.IsLessAt(it, it - 1))
            it--;

        nsorted = last - it;
        if (nsorted < minProcess)
            return 0;

        Reverse(s, it, last - 1);
        return nsorted;
    }

    private static void RangeSortData<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> data, SortSpan<T, TComparer, TContext> aux)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var len = data.Length;
        switch (len)
        {
            case <= 1:
                return;
            case <= SORT_MIN_INTERNAL:
                InsertionSort.SortCore(data, 0, len);
                return;
        }

        var mid = len / 2;
        var dataLeft = data.Slice(0, mid, data.BufferId);
        var dataRight = data.Slice(mid, len - mid, data.BufferId);
        var auxLeft = aux.Slice(0, mid, aux.BufferId);
        var auxRight = aux.Slice(mid, len - mid, aux.BufferId);

        RangeSortBuffer(dataLeft, auxLeft);
        RangeSortBuffer(dataRight, auxRight);
        MergeFromBufferToData(aux, mid, data);
    }

    private static void RangeSortBuffer<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> data, SortSpan<T, TComparer, TContext> aux)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var len = data.Length;
        switch (len)
        {
            case 0:
                return;
            case 1:
                aux.Write(0, data.Read(0));
                return;
            case 2:
                var v0 = data.Read(0);
                var v1 = data.Read(1);
                if (data.IsLessThan(v1, v0))
                {
                    aux.Write(0, v1);
                    aux.Write(1, v0);
                }
                else
                {
                    aux.Write(0, v0);
                    aux.Write(1, v1);
                }
                return;
        }

        if (len <= SORT_MIN_INTERNAL)
        {
            InsertionSort.SortCore(data, 0, len);
            data.CopyTo(0, aux, 0, len);
            return;
        }

        var mid = len / 2;
        var dataLeft = data.Slice(0, mid, data.BufferId);
        var dataRight = data.Slice(mid, len - mid, data.BufferId);
        var auxLeft = aux.Slice(0, mid, aux.BufferId);
        var auxRight = aux.Slice(mid, len - mid, aux.BufferId);

        RangeSortData(dataLeft, auxLeft);
        RangeSortData(dataRight, auxRight);
        MergeFromDataToBuffer(data, mid, aux);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void MergeFromDataToBuffer<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> data, int mid, SortSpan<T, TComparer, TContext> aux)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var len = data.Length;
        var left = 0;
        var right = mid;
        var dst = 0;

        while (left < mid && right < len)
        {
            var leftValue = data.Read(left);
            var rightValue = data.Read(right);
            if (data.IsLessOrEqual(leftValue, rightValue))
            {
                aux.Write(dst++, leftValue);
                left++;
            }
            else
            {
                aux.Write(dst++, rightValue);
                right++;
            }
        }

        if (left < mid)
            data.CopyTo(left, aux, dst, mid - left);
        else if (right < len)
            data.CopyTo(right, aux, dst, len - right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void MergeFromBufferToData<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> aux, int mid, SortSpan<T, TComparer, TContext> data)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var len = data.Length;
        var left = 0;
        var right = mid;
        var dst = 0;

        while (left < mid && right < len)
        {
            var leftValue = aux.Read(left);
            var rightValue = aux.Read(right);
            if (aux.IsLessOrEqual(leftValue, rightValue))
            {
                data.Write(dst++, leftValue);
                left++;
            }
            else
            {
                data.Write(dst++, rightValue);
                right++;
            }
        }

        if (left < mid)
            aux.CopyTo(left, data, dst, mid - left);
        else if (right < len)
            aux.CopyTo(right, data, dst, len - right);
    }

    private static void InsertSorted<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> data, int first, int mid, int last, SortSpan<T, TComparer, TContext> aux)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var tailLen = last - mid;
        if (tailLen == 0 || mid == first)
            return;

        data.CopyTo(mid, aux, 0, tailLen);

        var moveFirst = mid;
        var moveLast = mid;
        for (var remaining = tailLen; remaining > 0; remaining--)
        {
            moveLast = moveFirst;
            var value = aux.Read(remaining - 1);
            moveFirst = UpperBound(data, first, moveLast, value);

            if (moveFirst != moveLast)
            {
                var length = moveLast - moveFirst;
                data.CopyTo(moveFirst, data, moveFirst + remaining, length);
            }

            data.Write(moveFirst + remaining - 1, value);
        }
    }

    private static void InsertSortedBackward<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> data, int first, int mid, int last, SortSpan<T, TComparer, TContext> aux)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var headLen = mid - first;
        if (headLen == 0 || mid == last)
            return;

        data.CopyTo(first, aux, 0, headLen);

        var moveFirst = mid;
        var moveLast = mid;
        for (var inserted = 0; inserted < headLen; inserted++)
        {
            moveFirst = moveLast;
            var value = aux.Read(inserted);
            moveLast = LowerBound(data, moveFirst, last, value);

            if (moveFirst != moveLast)
            {
                var destStart = moveFirst - (headLen - inserted);
                data.CopyTo(moveFirst, data, destStart, moveLast - moveFirst);
            }

            data.Write(moveLast - (headLen - inserted), value);
        }
    }

    private static void MergeAdjacentWithLeftBuffer<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> data, int first, int mid, int last, SortSpan<T, TComparer, TContext> aux)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var leftLen = mid - first;
        var rightLen = last - mid;
        if (leftLen == 0 || rightLen == 0)
            return;

        if (data.IsLessOrEqualAt(mid - 1, mid))
            return;

        if (leftLen <= rightLen)
        {
            data.CopyTo(first, aux, 0, leftLen);
            MergeHalf(aux.Slice(0, leftLen, BUFFER_TEMP), data, first, leftLen, rightLen);
            return;
        }

        data.CopyTo(mid, aux, 0, rightLen);
        MergeHalfBackward(data, first, leftLen, aux.Slice(0, rightLen, BUFFER_TEMP), rightLen);
    }

    private static void MergeHalf<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> buf, SortSpan<T, TComparer, TContext> main, int mainStart, int leftLen, int rightLen)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var left = 0;
        var right = mainStart + leftLen;
        var dst = mainStart;
        var leftEnd = leftLen;
        var rightEnd = mainStart + leftLen + rightLen;

        while (left < leftEnd && right < rightEnd)
        {
            var leftValue = buf.Read(left);
            var rightValue = main.Read(right);
            if (buf.IsLessOrEqual(leftValue, rightValue))
            {
                main.Write(dst++, leftValue);
                left++;
            }
            else
            {
                main.Write(dst++, rightValue);
                right++;
            }
        }

        if (left < leftEnd)
            buf.CopyTo(left, main, dst, leftEnd - left);
    }

    private static void MergeHalfBackward<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> main, int mainStart, int leftLen, SortSpan<T, TComparer, TContext> buf, int rightLen)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var left = mainStart + leftLen - 1;
        var right = rightLen - 1;
        var dst = mainStart + leftLen + rightLen - 1;

        while (left >= mainStart && right >= 0)
        {
            var leftValue = main.Read(left);
            var rightValue = buf.Read(right);
            if (main.IsGreaterThan(leftValue, rightValue))
            {
                main.Write(dst--, leftValue);
                left--;
            }
            else
            {
                main.Write(dst--, rightValue);
                right--;
            }
        }

        if (right >= 0)
            buf.CopyTo(0, main, mainStart, right + 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Reverse<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int lo, int hi)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (lo < hi)
            s.Swap(lo++, hi--);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CeilDiv(int value, int divisor)
        => (value + divisor - 1) / divisor;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int UpperBound<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last, T value)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (first < last)
        {
            var mid = first + ((last - first) >> 1);
            if (s.IsLessOrEqual(s.Read(mid), value))
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int LowerBound<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last, T value)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (first < last)
        {
            var mid = first + ((last - first) >> 1);
            if (s.IsLessThan(s.Read(mid), value))
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetBlockSize<T>()
    {
        if (typeof(T) == typeof(string))
            return 1 << 6;

        var size = Unsafe.SizeOf<T>();
        var bitSize = size switch
        {
            <= 1 => 0,
            > 128 => 7,
            _ => (int)BitOperations.Log2((uint)(size - 1)),
        };

        ReadOnlySpan<byte> powers = [10, 10, 10, 9, 8, 7, 6, 6];
        return 1 << powers[Math.Min(bitSize, powers.Length - 1)];

    }
}
