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
/// <item><description><strong>merge_block-style Outer Model:</strong> The implementation now keeps an explicit block index array,
/// merges logical block groups through that index, and applies a final permutation via <c>RearrangeWithIndex</c>, mirroring
/// Boost's <c>merge_block</c> base class structure.</description></item>
/// <item><description><strong>Divide — Recursive Block-Group Split:</strong> Mirrors Boost's <c>divide(itx_first, itx_last)</c>.
/// When <c>nblock &lt; 5</c>, falls through to <c>SortSmall</c>.
/// When <c>nblock &gt; 7</c>, calls <c>IsSortedForward</c> / <c>IsSortedBackward</c> for partial pre-sort detection.
/// Otherwise splits at <c>(nblock+1)&gt;&gt;1</c>, recurses on both halves, and merges via
/// <c>MergeRangePos</c> over the block index (corresponding to Boost's <c>merge_range_pos</c>).</description></item>
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
/// <item><description><strong>MergeRangePos / RearrangeWithIndex:</strong> Larger block groups are merged by manipulating block
/// indices and a circular scratch buffer, then the final logical block permutation is materialized in-place at the end,
/// mirroring Boost's <c>merge_range_pos</c>, <c>move_range_pos_backward</c>, and <c>rearrange_with_index</c>.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Merge (Block-based)</description></item>
/// <item><description>Stable      : Yes (≤ comparison in all merge operations preserves relative order)</description></item>
/// <item><description>In-place    : No (requires O(n / BLOCK_SIZE) block-index storage plus O(BLOCK_SIZE) circular scratch)</description></item>
/// <item><description>Best case   : O(n) to O(n log n), depending on whether the partial-sorted fast paths trigger</description></item>
/// <item><description>Average case: O(n log n)</description></item>
/// <item><description>Worst case  : O(n log n) — Guaranteed by balanced binary split at block granularity</description></item>
/// <item><description>Space       : O(n / BLOCK_SIZE) indices + O(BLOCK_SIZE) temporary elements + O(log n) recursion stack</description></item>
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
        if (n <= 1)
            return;

        var blockSize = GetBlockSize<T>();
        var nblock = CeilDiv(n, blockSize);
        var tempLength = Math.Min(n, blockSize << 1);
        var tempBuffer = ArrayPool<T>.Shared.Rent(tempLength);
        var indexBuffer = ArrayPool<int>.Shared.Rent(nblock);
        var scratchBuffer = ArrayPool<int>.Shared.Rent((nblock << 1) + 1);
        try
        {
            var data = s.Slice(first, n, BUFFER_MAIN);
            var temp = new SortSpan<T, TComparer, TContext>(tempBuffer.AsSpan(0, tempLength), s.Context, s.Comparer, BUFFER_TEMP);
            var index = indexBuffer.AsSpan(0, nblock);
            var scratch = scratchBuffer.AsSpan(0, (nblock << 1) + 1);

            for (var i = 0; i < nblock; i++)
                index[i] = i;

            var mergeBlock = new MergeBlockState<T, TComparer, TContext>(data, temp, index, scratch, blockSize);
            mergeBlock.Sort();
        }
        finally
        {
            ArrayPool<int>.Shared.Return(scratchBuffer);
            ArrayPool<int>.Shared.Return(indexBuffer);
            ArrayPool<T>.Shared.Return(tempBuffer, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
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

    private readonly ref struct MergeBlockState<T, TComparer, TContext>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        private readonly SortSpan<T, TComparer, TContext> _data;
        private readonly SortSpan<T, TComparer, TContext> _temp;
        private readonly Span<int> _index;
        private readonly Span<int> _scratch;
        private readonly int _blockSize;
        private readonly int _nblock;
        private readonly int _ntail;

        public MergeBlockState(
            SortSpan<T, TComparer, TContext> data,
            SortSpan<T, TComparer, TContext> temp,
            Span<int> index,
            Span<int> scratch,
            int blockSize)
        {
            _data = data;
            _temp = temp;
            _index = index;
            _scratch = scratch;
            _blockSize = blockSize;
            _nblock = index.Length;
            _ntail = data.Length % blockSize;
        }

        public void Sort()
        {
            if (_data.Length <= 1)
                return;

            Divide(0, _nblock);
            RearrangeWithIndex();
        }

        private void Divide(int indexFirst, int indexLast)
        {
            var nblock = indexLast - indexFirst;
            if (nblock < 5)
            {
                SortSmall(indexFirst, indexLast);
                return;
            }

            if (nblock > 7)
            {
                if (IsSortedForward(indexFirst, indexLast))
                    return;

                if (IsSortedBackward(indexFirst, indexLast))
                    return;
            }

            var nblock1 = (nblock + 1) >> 1;
            var middle = indexFirst + nblock1;
            Divide(indexFirst, middle);
            Divide(middle, indexLast);
            MergeRangePos(indexFirst, middle, indexLast);
        }

        private void SortSmall(int indexFirst, int indexLast)
        {
            var nblock = indexLast - indexFirst;
            if (nblock <= 0)
                return;

            var dataRange = GetGroupRange(_index[indexFirst], nblock);
            var data = _data.Slice(dataRange.Start, dataRange.Length, BUFFER_MAIN);

            if (data.Length <= SORT_MIN_INTERNAL)
            {
                InsertionSort.SortCore(data, 0, data.Length);
                return;
            }

            if (nblock < 3)
            {
                RangeSortData(data, _temp.Slice(0, data.Length, BUFFER_TEMP));
                return;
            }

            var nblock1 = (nblock + 1) >> 1;
            var left = GetGroupRange(_index[indexFirst], nblock1);
            var rightStart = left.End;
            var right = new ElementRange(rightStart, dataRange.End - rightStart);

            RangeSortData(_data.Slice(right.Start, right.Length, BUFFER_MAIN), _temp.Slice(0, right.Length, BUFFER_TEMP));
            var leftData = _data.Slice(left.Start, left.Length, BUFFER_MAIN);
            var leftAux = _temp.Slice(0, left.Length, BUFFER_TEMP);
            RangeSortBuffer(leftData, leftAux);
            MergeHalf(leftAux, _data, left.Start, left.Length, right.Length);
        }

        private bool IsSortedForward(int indexFirst, int indexLast)
        {
            var nblock = indexLast - indexFirst;
            var rng = GetGroupRange(_index[indexFirst], nblock);
            var minProcess = Math.Max(_blockSize, rng.Length >> 3);
            var nsorted1 = NumberStableSortedForward(_data, rng.Start, rng.End, minProcess);
            if (nsorted1 == rng.Length)
                return true;

            if (nsorted1 == 0)
                return false;

            var nsorted2 = rng.Length - nsorted1;
            var mid = rng.Start + nsorted1;
            if (nsorted2 <= (_blockSize << 1))
            {
                SortPhysicalRange(mid, rng.End);
                InsertSorted(_data, rng.Start, mid, rng.End, _temp);
                return true;
            }

            var nsorted1Adjust = nsorted1 & ~(_blockSize - 1);
            var nblock1 = nsorted1Adjust / _blockSize;
            Divide(indexFirst + nblock1, indexLast);
            MergeRangePos(indexFirst, indexFirst + nblock1, indexLast);
            return true;
        }

        private bool IsSortedBackward(int indexFirst, int indexLast)
        {
            var nblock = indexLast - indexFirst;
            var rng = GetGroupRange(_index[indexFirst], nblock);
            var minProcess = Math.Max(_blockSize, rng.Length >> 3);
            var nsorted2 = NumberStableSortedBackward(_data, rng.Start, rng.End, minProcess);
            if (nsorted2 == rng.Length)
                return true;

            if (nsorted2 == 0)
                return false;

            var mid = rng.End - nsorted2;
            var nsorted1 = rng.Length - nsorted2;
            if (nsorted1 <= (_blockSize << 1))
            {
                SortPhysicalRange(rng.Start, mid);
                InsertSortedBackward(_data, rng.Start, mid, rng.End, _temp);
                return true;
            }

            var nblock1 = CeilDiv(nsorted1, _blockSize);
            Divide(indexFirst, indexFirst + nblock1);
            MergeRangePos(indexFirst, indexFirst + nblock1, indexLast);
            return true;
        }

        private void SortPhysicalRange(int first, int last)
        {
            var len = last - first;
            if (len <= 1)
                return;

            var data = _data.Slice(first, len, BUFFER_MAIN);
            if (len <= SORT_MIN_INTERNAL)
            {
                InsertionSort.SortCore(data, 0, len);
                return;
            }

            RangeSortData(data, _temp.Slice(0, len, BUFFER_TEMP));
        }

        private void MergeRangePos(int indexFirst, int indexMiddle, int indexLast)
        {
            var leftCount = indexMiddle - indexFirst;
            var rightCount = indexLast - indexMiddle;
            if (leftCount == 0 || rightCount == 0)
                return;

            var leftScratch = _scratch[..(leftCount + 1)];
            var rightScratch = _scratch.Slice(leftCount + 1, rightCount);
            _index.Slice(indexFirst, leftCount).CopyTo(leftScratch);
            _index.Slice(indexMiddle, rightCount).CopyTo(rightScratch);

            var outIndex = indexFirst;
            var leftPos = 0;
            var rightPos = 0;
            var hasLeft = false;
            var hasRight = false;
            var rangeA = default(ElementRange);
            var rangeB = default(ElementRange);
            var itA = 0;
            var itB = 0;
            var circ = new CircularSortBuffer<T, TComparer, TContext>(_temp);

            while (leftPos < leftCount && rightPos < rightCount)
            {
                if (!hasLeft)
                {
                    rangeA = GetRange(leftScratch[leftPos]);
                    itA = rangeA.Start;
                    hasLeft = true;
                }

                if (!hasRight)
                {
                    rangeB = GetRange(rightScratch[rightPos]);
                    itB = rangeB.Start;
                    hasRight = true;
                }

                if (circ.Count == 0)
                {
                    if (!_data.IsLessThan(_data.Read(rangeB.Start), _data.Read(rangeA.End - 1)))
                    {
                        _index[outIndex++] = leftScratch[leftPos++];
                        hasLeft = false;
                        continue;
                    }

                    if (_data.IsLessThan(_data.Read(rangeB.End - 1), _data.Read(rangeA.Start)))
                    {
                        if (!IsTail(rightScratch[rightPos]))
                        {
                            _index[outIndex++] = rightScratch[rightPos];
                        }
                        else
                        {
                            circ.PushMoveBack(_data, rangeB.Start, rangeB.Length);
                        }

                        rightPos++;
                        hasRight = false;
                        continue;
                    }
                }

                var finishedLeft = MergeCircular(ref itA, rangeA.End, ref itB, rangeB.End, ref circ);
                if (finishedLeft)
                {
                    circ.PopMoveFront(_data, rangeA.Start, rangeA.Length);
                    _index[outIndex++] = leftScratch[leftPos++];
                    hasLeft = false;
                }
                else
                {
                    if (!IsTail(rightScratch[rightPos]))
                    {
                        circ.PopMoveFront(_data, rangeB.Start, rangeB.Length);
                        _index[outIndex++] = rightScratch[rightPos];
                    }

                    rightPos++;
                    hasRight = false;
                }
            }

            if (leftPos == leftCount)
            {
                var tailRange = GetRange(rightScratch[rightPos]);
                circ.PopMoveFront(_data, tailRange.Start, circ.Count);
                while (rightPos < rightCount)
                {
                    _index[outIndex++] = rightScratch[rightPos++];
                }

                return;
            }

            var remainingRange = GetRange(leftScratch[leftPos]);
            if (_ntail != 0 && rightScratch[rightCount - 1] == (_nblock - 1))
            {
                leftScratch[leftCount] = rightScratch[rightCount - 1];
                var numA = itA - remainingRange.Start;
                circ.PopMoveBack(_data, remainingRange.Start, numA);
                MoveRangePosBackward(leftScratch, leftPos, leftCount + 1, _ntail);
                leftCount++;
            }

            circ.PopMoveFront(_data, remainingRange.Start, circ.Count);
            while (leftPos < leftCount)
            {
                _index[outIndex++] = leftScratch[leftPos++];
            }
        }

        private bool MergeCircular(ref int firstA, int endA, ref int firstB, int endB, ref CircularSortBuffer<T, TComparer, TContext> circ)
        {
            if (!_data.IsLessThan(_data.Read(firstB), _data.Read(endA - 1)))
            {
                circ.PushMoveBack(_data, firstA, endA - firstA);
                firstA = endA;
                return true;
            }

            if (_data.IsLessThan(_data.Read(endB - 1), _data.Read(firstA)))
            {
                circ.PushMoveBack(_data, firstB, endB - firstB);
                firstB = endB;
                return false;
            }

            while (firstA < endA && firstB < endB)
            {
                var valueA = _data.Read(firstA);
                var valueB = _data.Read(firstB);
                if (_data.IsLessThan(valueB, valueA))
                {
                    circ.PushBack(valueB);
                    firstB++;
                }
                else
                {
                    circ.PushBack(valueA);
                    firstA++;
                }
            }

            return firstA == endA;
        }

        private void MoveRangePosBackward(Span<int> blocks, int first, int last, int npos)
        {
            var range1 = GetRange(blocks[last - 1]);
            if (range1.Length > npos)
            {
                var moveCount = range1.Length - npos;
                _data.CopyTo(range1.Start, _data, range1.Start + npos, moveCount);
            }

            for (var it = last - 1; first < it;)
            {
                it--;
                var range2 = range1;
                range1 = GetRange(blocks[it]);
                var mid1 = range1.End - npos;
                _data.CopyTo(mid1, _data, range2.Start, npos);
                _data.CopyTo(range1.Start, _data, range1.Start + npos, range1.Length - npos);
            }
        }

        private void RearrangeWithIndex()
        {
            var pos = 0;
            while (pos < _index.Length)
            {
                while (pos < _index.Length && _index[pos] == pos)
                    pos++;

                if (pos == _index.Length)
                    return;

                var initialRange = GetRange(pos);
                _data.CopyTo(initialRange.Start, _temp, 0, initialRange.Length);
                var auxLength = initialRange.Length;

                var dest = pos;
                var src = _index[pos];
                while (src != pos)
                {
                    var srcRange = GetRange(src);
                    var destRange = GetRange(dest);
                    _data.CopyTo(srcRange.Start, _data, destRange.Start, srcRange.Length);
                    _index[dest] = dest;
                    dest = src;
                    src = _index[src];
                }

                var finalRange = GetRange(dest);
                _temp.CopyTo(0, _data, finalRange.Start, auxLength);
                _index[dest] = dest;
                pos++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsTail(int position)
            => position == (_nblock - 1) && _ntail != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ElementRange GetRange(int position)
        {
            var start = position * _blockSize;
            var end = position == (_nblock - 1) ? _data.Length : start + _blockSize;
            return new ElementRange(start, end - start);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ElementRange GetGroupRange(int position, int count)
        {
            var start = position * _blockSize;
            var endPosition = position + count;
            var end = endPosition == _nblock ? _data.Length : endPosition * _blockSize;
            return new ElementRange(start, end - start);
        }
    }

    private ref struct CircularSortBuffer<T, TComparer, TContext>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        private readonly SortSpan<T, TComparer, TContext> _buffer;
        private readonly int _capacity;
        private int _count;
        private int _first;

        public CircularSortBuffer(SortSpan<T, TComparer, TContext> buffer)
        {
            _buffer = buffer;
            _capacity = buffer.Length;
            _count = 0;
            _first = 0;
        }

        public int Count => _count;

        public void PushBack(T value)
        {
            _buffer.Write((_first + _count) % _capacity, value);
            _count++;
        }

        public void PushMoveBack(SortSpan<T, TComparer, TContext> source, int start, int length)
        {
            for (var i = 0; i < length; i++)
                PushBack(source.Read(start + i));
        }

        public void PopMoveFront(SortSpan<T, TComparer, TContext> destination, int start, int length)
        {
            for (var i = 0; i < length; i++)
            {
                destination.Write(start + i, _buffer.Read(_first));
                _first = (_first + 1) % _capacity;
            }

            _count -= length;
        }

        public void PopMoveBack(SortSpan<T, TComparer, TContext> destination, int start, int length)
        {
            _count -= length;
            var pos = (_first + _count) % _capacity;
            for (var i = 0; i < length; i++)
                destination.Write(start + i, _buffer.Read((pos + i) % _capacity));
        }
    }

    private readonly record struct ElementRange(int Start, int Length)
    {
        public int End => Start + Length;
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
}
