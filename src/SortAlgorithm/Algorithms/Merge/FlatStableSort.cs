using SortAlgorithm.Contexts;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// Boost.Sort の flat_stable_sort を参考にした、ブロック境界で分割する安定ソートです。
/// 再帰はブロック群単位で進め、小規模グループは専用の安定ソートへ落とし込みます。
/// <br/>
/// A stable sort inspired by Boost.Sort's flat_stable_sort.
/// Recursion progresses on block groups, and small groups fall back to a local stable sort.
/// </summary>
public static class FlatStableSort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;    // Main input array
    private const int BUFFER_TEMP = 1;    // Temporary half buffer

    // Boost's outer Sort_min = 36. Arrays with n <= Sort_min * 2 (72) use insertion sort directly.
    private const int SORT_MIN = 36;

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
    /// Core algorithm, equivalent to Boost's flast_stable_sort constructor.
    /// Performs pre-sort detection, computes nlevel, splits based on nlevel parity,
    /// calls RangeSort for each half, and merge_half's the results.
    /// </summary>
    private static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (IsAscending(s, first, last))
            return;

        if (IsDescending(s, first, last))
        {
            Reverse(s, first, last - 1);
            return;
        }

        var n = last - first;
        var blockSize = GetBlockSize<T>();
        var mergeBuffer = ArrayPool<T>.Shared.Rent((n + 1) / 2);
        try
        {
            var aux = new SortSpan<T, TComparer, TContext>(mergeBuffer.AsSpan(0, (n + 1) / 2), s.Context, s.Comparer, BUFFER_TEMP);
            Divide(s, first, last, blockSize, aux);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(mergeBuffer, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    private static void Divide<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        int first,
        int last,
        int blockSize,
        SortSpan<T, TComparer, TContext> aux)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var len = last - first;
        if (len <= 1)
            return;

        var nblock = CeilDiv(len, blockSize);
        if (nblock < 5)
        {
            SortSmall(s, first, last);
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

    private static void SortSmall<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var len = last - first;
        if (len <= 1)
            return;

        if (len <= SORT_MIN_INTERNAL)
        {
            InsertionSort.SortCore(s, first, last);
            return;
        }

        StableSortRange(s, first, last);
    }

    private static bool IsSortedForward<T, TComparer, TContext>(
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
        var nsorted1 = NumberStableSortedForward(s, first, last, minProcess);
        if (nsorted1 == len)
            return true;

        if (nsorted1 == 0)
            return false;

        var nsorted2 = len - nsorted1;
        var mid = first + nsorted1;

        if (nsorted2 <= (blockSize << 1))
        {
            StableSortRange(s, mid, last);
            InsertPartialSort(s, first, mid, last, aux, 0);
            return true;
        }

        var nsorted1Adjust = nsorted1 & ~(blockSize - 1);
        Divide(s, first + nsorted1Adjust, last, blockSize, aux);
        MergeAdjacentWithLeftBuffer(s, first, first + nsorted1Adjust, last, aux);
        return true;
    }

    private static bool IsSortedBackward<T, TComparer, TContext>(
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
            StableSortRange(s, first, mid);
            MergeAdjacentWithLeftBuffer(s, first, mid, last, aux);
            return true;
        }

        var nblock1 = CeilDiv(nsorted1, blockSize);
        var nsorted1Adjust = Math.Min(len, nblock1 * blockSize);
        Divide(s, first, first + nsorted1Adjust, blockSize, aux);
        MergeAdjacentWithLeftBuffer(s, first, first + nsorted1Adjust, last, aux);
        return true;
    }

    private static int NumberStableSortedForward<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        int first,
        int last,
        int minProcess)
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

    private static int NumberStableSortedBackward<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        int first,
        int last,
        int minProcess)
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

    private static void StableSortRange<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var len = last - first;
        if (len <= 1)
            return;

        if (len <= SORT_MIN_INTERNAL)
        {
            InsertionSort.SortCore(s, first, last);
            return;
        }

        var temp = ArrayPool<T>.Shared.Rent(len);
        try
        {
            var data = s.Slice(first, len, BUFFER_MAIN);
            var aux = new SortSpan<T, TComparer, TContext>(temp.AsSpan(0, len), s.Context, s.Comparer, BUFFER_TEMP);
            StableSortData(data, aux);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(temp, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    private static void StableSortData<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> data,
        SortSpan<T, TComparer, TContext> aux)
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

        StableSortToBuffer(dataLeft, auxLeft);
        StableSortToBuffer(dataRight, auxRight);
        MergeFromBufferToData(aux, mid, data);
    }

    private static void StableSortToBuffer<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> data,
        SortSpan<T, TComparer, TContext> aux)
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

        StableSortData(dataLeft, auxLeft);
        StableSortData(dataRight, auxRight);
        MergeFromDataToBuffer(data, mid, aux);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void MergeFromDataToBuffer<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> data,
        int mid,
        SortSpan<T, TComparer, TContext> aux)
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
    private static void MergeFromBufferToData<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> aux,
        int mid,
        SortSpan<T, TComparer, TContext> data)
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

    private static void InsertPartialSort<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> data,
        int first,
        int mid,
        int last,
        SortSpan<T, TComparer, TContext> aux,
        int auxStart)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var tailLen = last - mid;
        if (tailLen == 0 || mid == first)
            return;

        data.CopyTo(mid, aux, auxStart, tailLen);

        var left = mid - 1;
        var right = tailLen - 1;
        var dst = last - 1;

        while (right >= 0)
        {
            if (left >= first)
            {
                var leftValue = data.Read(left);
                var rightValue = aux.Read(auxStart + right);
                if (data.IsGreaterThan(leftValue, rightValue))
                {
                    data.Write(dst--, leftValue);
                    left--;
                }
                else
                {
                    data.Write(dst--, rightValue);
                    right--;
                }
            }
            else
            {
                aux.CopyTo(auxStart, data, first, right + 1);
                break;
            }
        }
    }

    private static void MergeAdjacentWithLeftBuffer<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> data,
        int first,
        int mid,
        int last,
        SortSpan<T, TComparer, TContext> aux)
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

    private static void MergeHalf<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> buf,
        SortSpan<T, TComparer, TContext> main,
        int mainStart,
        int leftLen,
        int rightLen)
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

    private static void MergeHalfBackward<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> main,
        int mainStart,
        int leftLen,
        SortSpan<T, TComparer, TContext> buf,
        int rightLen)
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
    private static bool IsAscending<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        for (var i = first + 1; i < last; i++)
        {
            if (s.IsGreaterAt(i - 1, i))
                return false;
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsDescending<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        for (var i = first + 1; i < last; i++)
        {
            if (s.IsGreaterOrEqualAt(i, i - 1))
                return false;
        }

        return true;
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
