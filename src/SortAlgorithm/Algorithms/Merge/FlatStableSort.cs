using SortAlgorithm.Contexts;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// Boost.Sort の flat_stable_sort を参照とした安定ソートです。配列をブロック単位に分割し、ブロックインデックスの操作と循環バッファを使ってマージします。
/// <br/>
/// A stable sort based on Boost.Sort's flat_stable_sort, processing elements in fixed-size blocks
/// and merging them via block-index manipulation and a circular scratch buffer.
/// </summary>
/// <remarks>
/// <para><strong>Structural Fidelity to Boost flat_stable_sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Block Size Selection (GetBlockSize):</strong> The block size is a power-of-two
/// chosen based on the element type size, mirroring Boost's <c>block_size_fss</c> template:
/// strings use 2^6 = 64, and other types use 2^sz[BitsSize] where <c>sz[] = {10,10,10,9,8,7,6,6}</c>.</description></item>
/// <item><description><strong>merge_block-style Outer Model:</strong> Keeps an explicit block index array,
/// merges logical block groups through that index, and applies a final permutation via <c>RearrangeWithIndex</c>, mirroring
/// Boost's <c>merge_block</c> base class structure.</description></item>
/// <item><description><strong>Divide — Recursive Block-Group Split:</strong> Mirrors Boost's <c>divide(itx_first, itx_last)</c>.
/// When <c>nblock &lt; 5</c>, delegates to <c>SortSmall</c>.
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
/// <item><description>Worst case  : O(n log n) (balanced binary split at block granularity)</description></item>
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
        var tempLength = blockSize << 1; // Always 2*BLOCK_SIZE (power of two), matching Boost's circular_buffer<Value_t, Power2+1>
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

    /// <summary>
    /// Sorts the elements in <paramref name="data"/> in-place, using <paramref name="aux"/> as scratch space.
    /// Result is left in <paramref name="data"/>. Mirrors Boost's <c>range_sort_data</c>.
    /// </summary>
    /// <remarks>
    /// Delegates to <see cref="RangeSortBuffer"/> for each half (writing sorted output into
    /// <paramref name="aux"/>), then <see cref="MergeFromBufferToData"/> combines them back into
    /// <paramref name="data"/>.
    /// </remarks>
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

        // flat_stable_sort always splits halves with mid = (len+1)/2, favoring the left half when len is odd.
        var mid = (len + 1) / 2;
        var dataLeft = data.Slice(0, mid, data.BufferId);
        var dataRight = data.Slice(mid, len - mid, data.BufferId);
        var auxLeft = aux.Slice(0, mid, aux.BufferId);
        var auxRight = aux.Slice(mid, len - mid, aux.BufferId);

        RangeSortBuffer(dataLeft, auxLeft);
        RangeSortBuffer(dataRight, auxRight);
        MergeFromBufferToData(aux, mid, data);
    }

    /// <summary>
    /// Sorts the elements in <paramref name="data"/> and writes the sorted result into <paramref name="aux"/>.
    /// Mirrors Boost's <c>range_sort_buffer</c>.
    /// </summary>
    /// <remarks>
    /// The sorted output is placed in <paramref name="aux"/>; <paramref name="data"/> may be modified
    /// as intermediate scratch. Handles len 0/1/2 as special cases without recursion. For larger
    /// ranges, delegates to <see cref="RangeSortData"/> for each half (result in <paramref name="data"/>),
    /// then <see cref="MergeFromDataToBuffer"/> combines them into <paramref name="aux"/>.
    /// </remarks>
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

        var mid = (len + 1) / 2;
        var dataLeft = data.Slice(0, mid, data.BufferId);
        var dataRight = data.Slice(mid, len - mid, data.BufferId);
        var auxLeft = aux.Slice(0, mid, aux.BufferId);
        var auxRight = aux.Slice(mid, len - mid, aux.BufferId);

        RangeSortData(dataLeft, auxLeft);
        RangeSortData(dataRight, auxRight);
        MergeFromDataToBuffer(data, mid, aux);
    }

    /// <summary>
    /// Stable merge of <paramref name="data"/>[0..<paramref name="mid"/>) and
    /// <paramref name="data"/>[<paramref name="mid"/>..len) into <paramref name="aux"/>.
    /// </summary>
    /// <remarks>
    /// <c>IsLessOrEqual</c> favours the left element on equal keys, preserving stable order.
    /// </remarks>
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

    /// <summary>
    /// Stable merge of <paramref name="aux"/>[0..<paramref name="mid"/>) and
    /// <paramref name="aux"/>[<paramref name="mid"/>..len) into <paramref name="data"/>.
    /// </summary>
    /// <remarks>
    /// <c>IsLessOrEqual</c> favours the left element on equal keys, preserving stable order.
    /// </remarks>
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

    /// <summary>
    /// Stably inserts the sorted range [<paramref name="mid"/>..<paramref name="last"/>) into the sorted
    /// prefix [<paramref name="first"/>..<paramref name="mid"/>), using <paramref name="aux"/> as scratch.
    /// Mirrors Boost's <c>insert_sorted</c>.
    /// </summary>
    /// <remarks>
    /// Processes tail elements right-to-left; each insertion point is found with
    /// <see cref="UpperBound"/> (inserts <em>after</em> equal elements, preserving stability).
    /// </remarks>
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

    /// <summary>
    /// Stably inserts the sorted range [<paramref name="first"/>..<paramref name="mid"/>) into the sorted
    /// suffix [<paramref name="mid"/>..<paramref name="last"/>), using <paramref name="aux"/> as scratch.
    /// Mirrors Boost's <c>insert_sorted_backward</c>.
    /// </summary>
    /// <remarks>
    /// Processes head elements left-to-right; each insertion point is found with
    /// <see cref="LowerBound"/> (inserts <em>before</em> equal elements, preserving stability).
    /// </remarks>
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

    /// <summary>
    /// Stable merge where the left half is in <paramref name="buf"/>[0..<paramref name="leftLen"/>)
    /// and the right half is already in <paramref name="main"/> at
    /// [<paramref name="mainStart"/>+<paramref name="leftLen"/>..+<paramref name="rightLen"/>).
    /// Writes the merged result into <paramref name="main"/> starting at <paramref name="mainStart"/>.
    /// </summary>
    /// <remarks>
    /// Used by <see cref="SortSmall"/> for 3–4 block groups after sorting the left half into
    /// <c>_temp</c>. <c>IsLessOrEqual</c> favours the left (<paramref name="buf"/>) element on equal
    /// keys. Remaining right elements are already in place; only a remaining-left flush is needed.
    /// </remarks>
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

    /// <summary>
    /// Reverse stable merge where the right half is in <paramref name="buf"/>[0..<paramref name="rightLen"/>)
    /// and the left half is in <paramref name="main"/>[<paramref name="mainStart"/>..+<paramref name="leftLen"/>).
    /// Fills <paramref name="main"/> from the high end downward.
    /// </summary>
    /// <remarks>
    /// Takes from the left when <c>IsGreaterThan(left, right)</c>, otherwise from
    /// <paramref name="buf"/>. Remaining right elements in <paramref name="buf"/> are flushed into
    /// the front of the merged region; remaining left elements are already in place.
    /// </remarks>
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

    /// <summary>Reverses elements in <paramref name="s"/>[<paramref name="lo"/>..<paramref name="hi"/>] in-place.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Reverse<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int lo, int hi)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (lo < hi)
            s.Swap(lo++, hi--);
    }

    /// <summary>Returns ⌈<paramref name="value"/> / <paramref name="divisor"/>⌉ using integer arithmetic.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CeilDiv(int value, int divisor)
        => (value + divisor - 1) / divisor;

    /// <summary>
    /// Binary search returning the first index in [<paramref name="first"/>..<paramref name="last"/>)
    /// where <c>s[i] &gt; <paramref name="value"/></c> (strict upper bound).
    /// </summary>
    /// <remarks>
    /// Uses <c>IsLessOrEqual(s[mid], value)</c>: advances past equal elements,
    /// so the insertion point is <em>after</em> any equal values.
    /// </remarks>
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

    /// <summary>
    /// Binary search returning the first index in [<paramref name="first"/>..<paramref name="last"/>)
    /// where <c>s[i] ≥ <paramref name="value"/></c> (strict lower bound).
    /// </summary>
    /// <remarks>
    /// Uses <c>IsLessThan(s[mid], value)</c>: does not advance past equal elements,
    /// so the insertion point is <em>before</em> any equal values.
    /// </remarks>
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

    /// <summary>
    /// Returns the block size for type <typeparamref name="T"/>, mirroring Boost's <c>block_size_fss</c>.
    /// </summary>
    /// <remarks>
    /// Block size is a power of two selected by element size in bytes via a lookup table:
    /// <c>powers = [10, 10, 10, 9, 8, 7, 6, 6]</c> (index = log₂(sizeof−1), clamped to 0–7).
    /// <c>string</c> is always treated as 64 (1&lt;&lt;6) regardless of its reference size.
    /// Example: <c>int</c> (4 bytes) → bitSize = log₂(3) = 1 → powers[1] = 10 → blockSize = 1024.
    /// </remarks>
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

    /// <summary>
    /// Holds all state for a block-level stable merge pass over <see cref="_data"/>.
    /// Mirrors Boost's <c>merge_block</c> class.
    /// </summary>
    /// <remarks>
    /// Uses a <em>deferred-permutation</em> model: the recursive <see cref="Divide"/>/<see cref="MergeRangePos"/>
    /// pipeline maintains a logical permutation in <see cref="_index"/> without physically moving blocks.
    /// <see cref="RearrangeWithIndex"/> materialises the permutation in a single cycle-following pass at the end.
    /// <para>
    /// <see cref="_temp"/> is the shared circular-buffer scratch of capacity 2×<see cref="_blockSize"/>.
    /// <see cref="_scratch"/> (length 2×<see cref="_nblock"/>+1) is scratch for <see cref="MergeRangePos"/>.
    /// </para>
    /// </remarks>
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

        /// <summary>
        /// Executes the block-level stable sort: recursively divides and merges block-index ranges,
        /// then materialises the permutation with <see cref="RearrangeWithIndex"/>.
        /// </summary>
        public void Sort()
        {
            if (_data.Length <= 1)
                return;

            Divide(0, _nblock);
            RearrangeWithIndex();
        }

        /// <summary>
        /// Recursively splits the block index range [<paramref name="indexFirst"/>..<paramref name="indexLast"/>)
        /// in half, sorts each half, then merges with <see cref="MergeRangePos"/>.
        /// Mirrors Boost's <c>divide</c>.
        /// </summary>
        /// <remarks>
        /// nblock &lt; 5 → <see cref="SortSmall"/> (no recursion).
        /// nblock &gt; 7 → attempts <see cref="IsSortedForward"/> / <see cref="IsSortedBackward"/> fast paths first.
        /// Otherwise splits at <c>(nblock+1)/2</c>, recurses on each half, then merges.
        /// </remarks>
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

        /// <summary>
        /// Sorts a small contiguous group of blocks (nblock &lt; 5) without further recursion.
        /// Mirrors Boost's <c>sort_small</c>.
        /// </summary>
        /// <remarks>
        /// nblock ≤ 0 → no-op. data.Length ≤ SORT_MIN_INTERNAL → <see cref="InsertionSort.SortCore"/> directly.
        /// nblock &lt; 3 → <see cref="RangeSortData"/> (result stays in <c>_data</c>).
        /// nblock 3–4 → sort right half with <see cref="RangeSortData"/>, sort left half with
        /// <see cref="RangeSortBuffer"/> (result into <c>_temp</c>), merge with <see cref="MergeHalf"/>.
        /// </remarks>
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

        /// <summary>
        /// Fast path: detects that the leading portion of the block range is already ascending,
        /// sorts any unsorted tail, merges, then returns <c>true</c> to skip recursive splitting.
        /// Returns <c>false</c> when no useful sorted prefix exists.
        /// Mirrors Boost's forward fast path inside <c>divide</c>.
        /// </summary>
        /// <remarks>
        /// Uses <see cref="NumberStableSortedForward"/> with <c>minProcess = max(blockSize, len/8)</c>.
        /// When <c>nsorted2 ≤ 2×blockSize</c> the unsorted tail is handled via <see cref="SortSubRange"/> +
        /// <see cref="InsertSorted"/>; otherwise the unsorted blocks recurse through <see cref="Divide"/>
        /// before merging.
        /// </remarks>
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
                SortSubRange(mid, rng.End);
                InsertSorted(_data, rng.Start, mid, rng.End, _temp);
                return true;
            }

            var nsorted1Adjust = nsorted1 & ~(_blockSize - 1);
            var nblock1 = nsorted1Adjust / _blockSize;
            Divide(indexFirst + nblock1, indexLast);
            MergeRangePos(indexFirst, indexFirst + nblock1, indexLast);
            return true;
        }

        /// <summary>
        /// Fast path: detects that the trailing portion of the block range is already ascending,
        /// sorts any unsorted prefix, merges, then returns <c>true</c> to skip recursive splitting.
        /// Returns <c>false</c> when no useful sorted suffix exists.
        /// Mirrors Boost's backward fast path inside <c>divide</c>.
        /// </summary>
        /// <remarks>
        /// Uses <see cref="NumberStableSortedBackward"/> with <c>minProcess = max(blockSize, len/8)</c>.
        /// When <c>nsorted1 ≤ 2×blockSize</c> the unsorted prefix is handled via <see cref="SortSubRange"/> +
        /// <see cref="InsertSortedBackward"/>; otherwise the unsorted blocks recurse through <see cref="Divide"/>
        /// before merging.
        /// </remarks>
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
                SortSubRange(rng.Start, mid);
                InsertSortedBackward(_data, rng.Start, mid, rng.End, _temp);
                return true;
            }

            var nblock1 = CeilDiv(nsorted1, _blockSize);
            Divide(indexFirst, indexFirst + nblock1);
            MergeRangePos(indexFirst, indexFirst + nblock1, indexLast);
            return true;
        }

        /// <summary>
        /// Sorts the physical range [first, last) via a nested MergeBlockState, mirroring Boost's recursive
        /// <c>flat_stable_sort(sub-range, ptr_circ)</c> call used in the partial-sorted fast path.
        /// The sub-state reuses the shared <c>_temp</c> scratch buffer.
        /// </summary>
        private void SortSubRange(int first, int last)
        {
            var len = last - first;
            if (len <= 1)
                return;

            var data = _data.Slice(first, len, BUFFER_MAIN);
            var nblock = CeilDiv(len, _blockSize);

            // Use ArrayPool (matching Boost's std::vector<size_t> index heap allocation in merge_block)
            // rather than stackalloc, so this is safe regardless of nblock size.
            var indexBuffer = ArrayPool<int>.Shared.Rent(nblock);
            var scratchBuffer = ArrayPool<int>.Shared.Rent((nblock << 1) + 1);
            try
            {
                var subIndex = indexBuffer.AsSpan(0, nblock);
                var subScratch = scratchBuffer.AsSpan(0, (nblock << 1) + 1);
                for (var i = 0; i < nblock; i++)
                    subIndex[i] = i;

                var sub = new MergeBlockState<T, TComparer, TContext>(data, _temp, subIndex, subScratch, _blockSize);
                sub.Sort();
            }
            finally
            {
                ArrayPool<int>.Shared.Return(scratchBuffer);
                ArrayPool<int>.Shared.Return(indexBuffer);
            }
        }

        /// <summary>
        /// Stably merges two sorted block-index ranges
        /// [<paramref name="indexFirst"/>..<paramref name="indexMiddle"/>) and
        /// [<paramref name="indexMiddle"/>..<paramref name="indexLast"/>) by updating <see cref="_index"/>
        /// with the merged order. Mirrors Boost's <c>merge_range_pos</c>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Block positions are only written into <see cref="_index"/>; physical data movement is deferred
        /// to <see cref="RearrangeWithIndex"/>. A <see cref="CircularSortBuffer{T,TComparer,TContext}"/> backed
        /// by <see cref="_temp"/> buffers elements during the merge via <see cref="MergeCircular"/>.
        /// </para>
        /// <para>
        /// Tail-block handling: when the last right block is the tail block (<c>_ntail ≠ 0</c>), its
        /// elements are pushed into the circular buffer and the tail index is appended to the left group;
        /// <see cref="MoveRangePosBackward"/> then shifts the combined physical region to its correct layout.
        /// </para>
        /// </remarks>
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

                if (circ.Size == 0)
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
                circ.PopMoveFront(_data, tailRange.Start, circ.Size);
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

            circ.PopMoveFront(_data, remainingRange.Start, circ.Size);
            while (leftPos < leftCount)
            {
                _index[outIndex++] = leftScratch[leftPos++];
            }
        }

        /// <summary>
        /// Merges elements from range A (<paramref name="firstA"/>..<paramref name="endA"/>) and range B
        /// (<paramref name="firstB"/>..<paramref name="endB"/>) into the circular buffer <paramref name="circ"/>,
        /// stopping as soon as one range is exhausted.
        /// Returns <c>true</c> if range A was exhausted first (or simultaneously), <c>false</c> if range B was.
        /// </summary>
        /// <remarks>
        /// <para><strong>Stability contract — equal elements always favour A:</strong><br/>
        /// The element-wise loop uses <c>IsLessThan(B, A)</c> (strict less-than) as the branch condition:
        /// <list type="bullet">
        ///   <item><c>B &lt; A</c> → take B</item>
        ///   <item><c>B ≥ A</c> (including equal) → take A</item>
        /// </list>
        /// Equal elements therefore come from A before B, preserving the original relative order.
        /// </para>
        /// <para><strong>Fast-path conditions and why their strictness matters for stability:</strong></para>
        /// <list type="number">
        ///   <item><description>
        ///     <c>!IsLessThan(B.first, A.last)</c> — i.e. <c>A.last ≤ B.first</c> (non-strict).<br/>
        ///     All of A sorts before or equal to all of B, so the entire A range can be pushed to the
        ///     circular buffer without inspecting B.  Using non-strict here is correct: when
        ///     <c>A.last == B.first</c> (equal boundary), A still comes entirely before B, which matches
        ///     the element-wise "equal → take A" rule.  A strict condition (<c>&lt;</c>) would fail to
        ///     detect this all-A-first case when the boundary values are equal and would fall through to
        ///     the slow loop unnecessarily — or worse, could take B before A.
        ///   </description></item>
        ///   <item><description>
        ///     <c>IsLessThan(B.last, A.first)</c> — i.e. <c>B.last &lt; A.first</c> (strict).<br/>
        ///     All of B is strictly less than A.first, so the entire B range can be pushed first.
        ///     Using strict here is also correct: when <c>B.last == A.first</c> we cannot safely take all
        ///     of B, because the equal element at <c>B.last</c> must come after the equal element at
        ///     <c>A.first</c> (stability requires A first).  A non-strict condition (<c>≤</c>) would
        ///     incorrectly push the equal B element ahead of A, breaking stability.
        ///   </description></item>
        /// </list>
        /// </remarks>
        private bool MergeCircular(ref int firstA, int endA, ref int firstB, int endB, ref CircularSortBuffer<T, TComparer, TContext> circ)
        {
            // Fast path A: the entire A block is ≤ the start of B (A.last ≤ B.first).
            // Non-strict: equal boundary is still "A before B" — consistent with the element loop.
            if (!_data.IsLessThan(_data.Read(firstB), _data.Read(endA - 1)))
            {
                circ.PushMoveBack(_data, firstA, endA - firstA);
                firstA = endA;
                return true;
            }

            // Fast path B: the entire B block is strictly less than A.first (B.last < A.first).
            // Strict: if B.last == A.first we must NOT take all of B first — equal means A goes first.
            if (_data.IsLessThan(_data.Read(endB - 1), _data.Read(firstA)))
            {
                circ.PushMoveBack(_data, firstB, endB - firstB);
                firstB = endB;
                return false;
            }

            // Element-wise merge: IsLessThan(B, A) is strict, so equal → take A (stability).
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
                    // B ≥ A (including equal) — take A to preserve stable order.
                    circ.PushBack(valueA);
                    firstA++;
                }
            }

            return firstA == endA;
        }

        /// <summary>
        /// Shifts elements within a contiguous span of logical blocks rightward (higher address) by
        /// <paramref name="npos"/> positions to accommodate the shorter tail block at the end.
        /// Mirrors Boost's <c>move_range_pos_backward</c>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Called from <see cref="MergeRangePos"/> only when the tail block (<c>_nblock−1</c>, with
        /// <c>_ntail</c> elements) appears as the last entry in the right-hand index.  At that point the
        /// tail is appended to the left-hand index and the combined physical region
        /// <c>blocks[first..last)</c> must be shifted right by <paramref name="npos"/> = <c>_ntail</c>
        /// positions so that each block slot ends up holding exactly the data that belongs there after
        /// the merge.
        /// </para>
        /// <para><strong>Algorithm (processes blocks right-to-left):</strong></para>
        /// <list type="number">
        ///   <item><description>
        ///     <strong>Last block (the tail, <c>blocks[last−1]</c>):</strong> if it holds more than
        ///     <paramref name="npos"/> elements, slide the first <c>range.Length − npos</c> elements
        ///     forward by <paramref name="npos"/> positions within the block
        ///     (<c>src = range.Start</c>, <c>dst = range.Start + npos</c>).
        ///     When <c>range.Length == npos</c> (i.e. the tail is exactly <c>_ntail</c> elements) this
        ///     step is skipped — the slot is already ready to receive incoming data from the left.
        ///   </description></item>
        ///   <item><description>
        ///     <strong>Each preceding full block (right-to-left):</strong>
        ///     <list type="bullet">
        ///       <item>Copy the block's last <paramref name="npos"/> elements into the first
        ///             <paramref name="npos"/> slots of the next (right) block.
        ///             Source and destination are adjacent — no overlap.</item>
        ///       <item>Shift the block's remaining <c>range.Length − npos</c> elements forward
        ///             by <paramref name="npos"/> positions within the same block
        ///             (<c>dst = src + npos</c>, so dst &gt; src — overlapping forward shift).</item>
        ///     </list>
        ///   </description></item>
        /// </list>
        /// <para><strong>Overlap safety:</strong>
        /// The two copies that shift elements forward within a single block have <c>dst &gt; src</c> with
        /// a potentially overlapping region.  <c>SortSpan.CopyTo</c> delegates to
        /// <c>Span&lt;T&gt;.CopyTo</c> → <c>Buffer.Memmove</c>, which selects a right-to-left traversal
        /// automatically when destination is ahead of source — exactly the behaviour of Boost's
        /// explicit <c>util::move_backward</c> calls.  Forward (left-to-right) copy would corrupt data
        /// when <c>_blockSize &gt; 2 × npos</c>.
        /// </para>
        /// <para><strong>Precondition:</strong> <paramref name="npos"/> ≤ <c>_blockSize</c>;
        /// <c>blocks[last−1]</c> must have at least <paramref name="npos"/> elements.</para>
        /// </remarks>
        private void MoveRangePosBackward(Span<int> blocks, int first, int last, int npos)
        {
            // Step 1 — last block (tail): free the first npos slots by pushing existing
            // elements forward; skip when Length == npos (tail is full of incoming data).
            var range1 = GetRange(blocks[last - 1]);
            if (range1.Length > npos)
            {
                var moveCount = range1.Length - npos;
                // dst > src, potentially overlapping — Buffer.Memmove handles right-to-left traversal.
                _data.CopyTo(range1.Start, _data, range1.Start + npos, moveCount);
            }

            // Step 2 — remaining blocks right-to-left: spill last npos elems to the next block,
            // then shift the rest of the current block forward to fill the vacated npos slots.
            for (var it = last - 1; first < it;)
            {
                it--;
                var range2 = range1;
                range1 = GetRange(blocks[it]);
                var mid1 = range1.End - npos;
                // Spill: move the last npos elements of range1 into the first npos slots of range2.
                // Adjacent blocks — no overlap.
                _data.CopyTo(mid1, _data, range2.Start, npos);
                // Shift: slide range1's remaining elements forward by npos positions.
                // dst > src, potentially overlapping — Buffer.Memmove handles right-to-left traversal.
                _data.CopyTo(range1.Start, _data, range1.Start + npos, range1.Length - npos);
            }
        }

        /// <summary>
        /// Materialises the deferred block permutation in <see cref="_index"/> by following each cycle
        /// and physically moving blocks into their correct positions.
        /// Mirrors Boost's <c>rearrange_with_index</c>.
        /// </summary>
        /// <remarks>
        /// For each out-of-place position, saves the initial block in <see cref="_temp"/> and follows
        /// the permutation cycle — copying each block one step forward — until the cycle closes back
        /// at the starting position. Each visited entry in <see cref="_index"/> is reset to itself as
        /// it is placed. <see cref="_temp"/> must hold at least one full block (guaranteed by the
        /// 2×blockSize allocation in <see cref="SortCore"/>).
        /// </remarks>
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

        /// <summary>Returns <c>true</c> when <paramref name="position"/> is the last block and the tail is shorter than a full block.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsTail(int position)
            => position == (_nblock - 1) && _ntail != 0;

        /// <summary>Returns the physical element range [start, start+length) for the block at <paramref name="position"/>.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ElementRange GetRange(int position)
        {
            var start = position * _blockSize;
            var end = position == (_nblock - 1) ? _data.Length : start + _blockSize;
            return new ElementRange(start, end - start);
        }

        /// <summary>Returns the physical element range covering <paramref name="count"/> consecutive blocks starting at <paramref name="position"/>.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ElementRange GetGroupRange(int position, int count)
        {
            var start = position * _blockSize;
            var endPosition = position + count;
            var end = endPosition == _nblock ? _data.Length : endPosition * _blockSize;
            return new ElementRange(start, end - start);
        }
    }

    /// <summary>
    /// Circular buffer backed by a <see cref="SortSpan{T,TComparer,TContext}"/>.
    /// Mirrors the API and state model of Boost's <c>circular_buffer&lt;Value_t, Power2&gt;</c>:
    /// capacity is always a power of two, so all index wrap-around uses a bitmask
    /// (<c>_mask = _capacity - 1</c>) instead of modulo division.
    /// </summary>
    private ref struct CircularSortBuffer<T, TComparer, TContext>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // --- Storage (mirrors Boost's ptr / NMAX / MASK) ---
        private readonly SortSpan<T, TComparer, TContext> _buffer;
        private readonly int _capacity;  // NMAX — must be a power of two
        private readonly int _mask;      // MASK = NMAX - 1  (enables & instead of %)
        // --- Mutable state (mirrors Boost's nelem / first_pos) ---
        private int _nelem;
        private int _first;

        public CircularSortBuffer(SortSpan<T, TComparer, TContext> buffer)
        {
            _buffer = buffer;
            _capacity = buffer.Length;
            _mask = _capacity - 1;   // valid only when _capacity is a power of two
            _nelem = 0;
            _first = 0;
        }

        // --- State properties (mirrors Boost's size() / capacity() / empty() / full() / free_size()) ---
        public int Size => _nelem;
        public int Capacity => _capacity;
        public int FreeSize => _capacity - _nelem;
        public bool Empty => _nelem == 0;
        public bool Full => _nelem == _capacity;

        /// <summary>Clears the buffer without touching the backing storage (mirrors Boost's <c>clear()</c>).</summary>
        public void Clear() { _nelem = 0; _first = 0; }

        // --- Element access (mirrors Boost's front() / back() / operator[]) ---
        /// <summary>Returns the first element (mirrors Boost's <c>front()</c>).</summary>
        public T Front() => _buffer.Read(_first);

        /// <summary>Returns the last element (mirrors Boost's <c>back()</c>).</summary>
        public T Back() => _buffer.Read((_first + _nelem - 1) & _mask);

        /// <summary>Returns the element at logical position <paramref name="pos"/> (mirrors Boost's <c>operator[]</c>).</summary>
        public T this[int pos] => _buffer.Read((_first + pos) & _mask);

        // --- Single-element mutations (mirrors Boost's push_back / push_front / pop_front / pop_back) ---
        /// <summary>Appends a value at the back (mirrors Boost's <c>push_back(val)</c>).</summary>
        public void PushBack(T value)
        {
            _buffer.Write((_first + _nelem) & _mask, value);
            _nelem++;
        }

        /// <summary>Prepends a value at the front (mirrors Boost's <c>push_front(val)</c>).</summary>
        public void PushFront(T value)
        {
            _first = (_first + _mask) & _mask;   // (_first - 1 + NMAX) & MASK
            _buffer.Write(_first, value);
            _nelem++;
        }

        /// <summary>Removes the front element (mirrors Boost's <c>pop_front()</c>).</summary>
        public void PopFront()
        {
            _nelem--;
            _first = (_first + 1) & _mask;
        }

        /// <summary>Removes the back element (mirrors Boost's <c>pop_back()</c>).</summary>
        public void PopBack() => _nelem--;

        // --- Bulk move operations (mirrors Boost's push_move_back / push_move_front / pop_move_front / pop_move_back) ---
        /// <summary>
        /// Moves <paramref name="length"/> elements from <paramref name="source"/> into the back
        /// (mirrors Boost's <c>push_move_back(iter, num)</c>).
        /// </summary>
        public void PushMoveBack(SortSpan<T, TComparer, TContext> source, int start, int length)
        {
            var pos = _first + _nelem;
            _nelem += length;
            for (var i = 0; i < length; i++)
                _buffer.Write(pos++ & _mask, source.Read(start + i));
        }

        /// <summary>
        /// Moves <paramref name="length"/> elements from <paramref name="source"/> into the front
        /// (mirrors Boost's <c>push_copy_front</c> semantics — decrements <c>first_pos</c> before writing).
        /// </summary>
        public void PushMoveFront(SortSpan<T, TComparer, TContext> source, int start, int length)
        {
            _first = (_first + _capacity - length) & _mask;
            _nelem += length;
            var pos = _first;
            for (var i = 0; i < length; i++)
                _buffer.Write(pos++ & _mask, source.Read(start + i));
        }

        /// <summary>
        /// Moves <paramref name="length"/> elements from the front into <paramref name="destination"/>
        /// (mirrors Boost's <c>pop_move_front(iter, num)</c>).
        /// </summary>
        public void PopMoveFront(SortSpan<T, TComparer, TContext> destination, int start, int length)
        {
            _nelem -= length;
            var pos = _first;
            _first = (_first + length) & _mask;
            for (var i = 0; i < length; i++)
                destination.Write(start + i, _buffer.Read(pos++ & _mask));
        }

        /// <summary>
        /// Moves <paramref name="length"/> elements from the back into <paramref name="destination"/>
        /// (mirrors Boost's <c>pop_move_back(iter, num)</c>).
        /// </summary>
        public void PopMoveBack(SortSpan<T, TComparer, TContext> destination, int start, int length)
        {
            _nelem -= length;
            var pos = (_first + _nelem) & _mask;
            for (var i = 0; i < length; i++)
                destination.Write(start + i, _buffer.Read(pos++ & _mask));
        }
    }

    /// <summary>A half-open element range [<see cref="Start"/>, <see cref="Start"/>+<see cref="Length"/>) within the data span.</summary>
    private readonly record struct ElementRange(int Start, int Length)
    {
        public int End => Start + Length;
    }

    /// <summary>
    /// Scans forward from <paramref name="first"/> to find the longest already-sorted prefix
    /// (ascending or descending). Reverses a descending prefix in-place.
    /// Returns its length if ≥ <paramref name="minProcess"/>, otherwise 0.
    /// </summary>
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

    /// <summary>
    /// Scans backward from <paramref name="last"/> to find the longest already-sorted suffix
    /// (ascending or descending). Reverses a descending suffix in-place.
    /// Returns its length if ≥ <paramref name="minProcess"/>, otherwise 0.
    /// </summary>
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
