using System.Buffers;
using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// Block Sort (WikiSort) は、安定なインプレースO(1)補助メモリのマージソートです。
/// ボトムアップマージソートの構造を基に、ソーティングネットワーク（4〜8要素）、小さいレベル向けのキャッシュベースマージ、大きいレベル向けの内部バッファを用いたブロックマージの3段階で処理します。
/// 内部バッファが確保できない場合はインプレースローテーションマージにフォールバックします。
/// <br/>
/// Block Sort (WikiSort) is a stable, in-place merge sort using O(1) auxiliary memory.
/// Built on a bottom-up merge sort structure, it processes data in three stages:
/// sorting networks for groups of 4–8 elements, cache-based merging for small merge levels, and block-based merging with internal buffers for large levels.
/// When internal buffers cannot be extracted, it falls back to in-place rotation-based merging.
/// </summary>
/// <remarks>
/// <para><strong>Algorithm Overview (ported from Zig std.sort.block / WikiSort):</strong></para>
/// <list type="number">
/// <item><description><strong>Phase 1 – Sorting Network Seeding:</strong> Groups of 4–8 elements are sorted
/// using optimal sorting networks with stability tracking via an order array.
/// Each group boundary is determined by the Iterator, which evenly distributes elements
/// across power-of-two sized blocks.</description></item>
/// <item><description><strong>Phase 2 – Cache-Based Merging (small levels):</strong> When adjacent A and B
/// subarrays fit within the fixed-size cache (512 elements), merging uses an external buffer.
/// A four-way merge optimization merges two pairs simultaneously when four subarrays fit.</description></item>
/// <item><description><strong>Phase 3 – Block Merging (large levels):</strong> When subarrays exceed the cache:
/// (a) Extract up to two internal buffers of √A unique values from the data.
/// (b) Tag A blocks with buffer1 values and roll them through B blocks.
/// (c) Merge each dropped A block with trailing B values using cache, internal buffer, or in-place merge.
/// (d) Re-sort buffer2 with insertion sort and redistribute both buffers back.</description></item>
/// <item><description><strong>Stability Preservation:</strong> Sorting networks use order-tracking to break ties
/// by original position. Binary searches use lower-bound / upper-bound semantics to keep equal elements
/// from the left run before those from the right run.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Merge (Block / WikiSort), Bottom-Up</description></item>
/// <item><description>Stable      : Yes</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space — fixed 512-element cache on stack)</description></item>
/// <item><description>Best case   : O(n) — Already sorted data: all merge-level skips fire</description></item>
/// <item><description>Average case: O(n log n)</description></item>
/// <item><description>Worst case  : O(n log n)</description></item>
/// <item><description>Space       : O(1) — 512-element stack cache, no heap allocation</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Block_sort</para>
/// <para>Paper: Ratio Based Stable In-Place Merging by Pok-Son Kim & Arne Kutzner https://link.springer.com/chapter/10.1007/978-3-540-79228-4_22</para>
/// <para>WikiSort: https://github.com/BonzaiThePenguin/WikiSort</para>
/// <para>Zig std.sort.block: https://github.com/ziglang/zig/blob/0.15.2/lib/std/sort/block.zig</para>
/// </remarks>
public static class BlockMergeSort
{
    // Buffer identifiers for visualization
    const int BUFFER_MAIN = 0;
    const int BUFFER_CACHE = 1;

    // Fixed-size cache on the stack (matches Zig reference)
    const int CacheSize = 512;

    /// <summary>
    /// Sorts the elements in the specified span in ascending order using the default comparer.
    /// Uses NullContext for zero-overhead fast path.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort in place.</param>
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
        => Sort(span, new ComparableComparer<T>(), NullContext.Default);

    /// <summary>
    /// Sorts the elements in the specified span using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T, TContext>(Span<T> span, TContext context)
        where T : IComparable<T>
        where TContext : ISortContext
        => Sort(span, new ComparableComparer<T>(), context);

    /// <summary>
    /// Sorts the elements in the specified span using the provided comparer and sort context.
    /// This is the full-control version with explicit TContext type parameter.
    /// </summary>
    public static void Sort<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;

        var cacheArray = ArrayPool<T>.Shared.Rent(CacheSize);
        try
        {
            var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
            var cache = new SortSpan<T, TComparer, TContext>(cacheArray.AsSpan(0, CacheSize), context, comparer, BUFFER_CACHE);
            SortCore(s, cache);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(cacheArray, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> cache)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = s.Length;

        if (n < 4)
        {
            if (n == 3)
            {
                // Hard-coded insertion sort for 3 elements
                if (s.Compare(1, 0) < 0) s.Swap(0, 1);
                if (s.Compare(2, 1) < 0)
                {
                    s.Swap(1, 2);
                    if (s.Compare(1, 0) < 0) s.Swap(0, 1);
                }
            }
            else if (n == 2)
            {
                if (s.Compare(1, 0) < 0) s.Swap(0, 1);
            }
            return;
        }

        // Phase 1: Sort groups of 4-8 elements using sorting networks
        var iterator = new BlockIterator(n, 4);
        while (!iterator.Finished())
        {
            Span<byte> order = [0, 1, 2, 3, 4, 5, 6, 7];
            var range = iterator.NextRange();
            var rangeLen = range.End - range.Start;

            switch (rangeLen)
            {
                case 8:
                    NetworkSwap(s, order, range.Start, 0, 1);
                    NetworkSwap(s, order, range.Start, 2, 3);
                    NetworkSwap(s, order, range.Start, 4, 5);
                    NetworkSwap(s, order, range.Start, 6, 7);
                    NetworkSwap(s, order, range.Start, 0, 2);
                    NetworkSwap(s, order, range.Start, 1, 3);
                    NetworkSwap(s, order, range.Start, 4, 6);
                    NetworkSwap(s, order, range.Start, 5, 7);
                    NetworkSwap(s, order, range.Start, 1, 2);
                    NetworkSwap(s, order, range.Start, 5, 6);
                    NetworkSwap(s, order, range.Start, 0, 4);
                    NetworkSwap(s, order, range.Start, 3, 7);
                    NetworkSwap(s, order, range.Start, 1, 5);
                    NetworkSwap(s, order, range.Start, 2, 6);
                    NetworkSwap(s, order, range.Start, 1, 4);
                    NetworkSwap(s, order, range.Start, 3, 6);
                    NetworkSwap(s, order, range.Start, 2, 4);
                    NetworkSwap(s, order, range.Start, 3, 5);
                    NetworkSwap(s, order, range.Start, 3, 4);
                    break;
                case 7:
                    NetworkSwap(s, order, range.Start, 1, 2);
                    NetworkSwap(s, order, range.Start, 3, 4);
                    NetworkSwap(s, order, range.Start, 5, 6);
                    NetworkSwap(s, order, range.Start, 0, 2);
                    NetworkSwap(s, order, range.Start, 3, 5);
                    NetworkSwap(s, order, range.Start, 4, 6);
                    NetworkSwap(s, order, range.Start, 0, 1);
                    NetworkSwap(s, order, range.Start, 4, 5);
                    NetworkSwap(s, order, range.Start, 2, 6);
                    NetworkSwap(s, order, range.Start, 0, 4);
                    NetworkSwap(s, order, range.Start, 1, 5);
                    NetworkSwap(s, order, range.Start, 0, 3);
                    NetworkSwap(s, order, range.Start, 2, 5);
                    NetworkSwap(s, order, range.Start, 1, 3);
                    NetworkSwap(s, order, range.Start, 2, 4);
                    NetworkSwap(s, order, range.Start, 2, 3);
                    break;
                case 6:
                    NetworkSwap(s, order, range.Start, 1, 2);
                    NetworkSwap(s, order, range.Start, 4, 5);
                    NetworkSwap(s, order, range.Start, 0, 2);
                    NetworkSwap(s, order, range.Start, 3, 5);
                    NetworkSwap(s, order, range.Start, 0, 1);
                    NetworkSwap(s, order, range.Start, 3, 4);
                    NetworkSwap(s, order, range.Start, 2, 5);
                    NetworkSwap(s, order, range.Start, 0, 3);
                    NetworkSwap(s, order, range.Start, 1, 4);
                    NetworkSwap(s, order, range.Start, 2, 4);
                    NetworkSwap(s, order, range.Start, 1, 3);
                    NetworkSwap(s, order, range.Start, 2, 3);
                    break;
                case 5:
                    NetworkSwap(s, order, range.Start, 0, 1);
                    NetworkSwap(s, order, range.Start, 3, 4);
                    NetworkSwap(s, order, range.Start, 2, 4);
                    NetworkSwap(s, order, range.Start, 2, 3);
                    NetworkSwap(s, order, range.Start, 1, 4);
                    NetworkSwap(s, order, range.Start, 0, 3);
                    NetworkSwap(s, order, range.Start, 0, 2);
                    NetworkSwap(s, order, range.Start, 1, 3);
                    NetworkSwap(s, order, range.Start, 1, 2);
                    break;
                case 4:
                    NetworkSwap(s, order, range.Start, 0, 1);
                    NetworkSwap(s, order, range.Start, 2, 3);
                    NetworkSwap(s, order, range.Start, 0, 2);
                    NetworkSwap(s, order, range.Start, 1, 3);
                    NetworkSwap(s, order, range.Start, 1, 2);
                    break;
            }
        }

        if (n < 8) return;

        // Phase 2 & 3: Bottom-up merge
        while (true)
        {
            if (iterator.Length() < CacheSize)
            {
                // Small levels: cache-based merging
                if ((iterator.Length() + 1) * 4 <= CacheSize && iterator.Length() * 4 <= n)
                {
                    // Four-way merge optimization: merge two pairs into cache, then merge results back
                    iterator.Begin();
                    while (!iterator.Finished())
                    {
                        var A1 = iterator.NextRange();
                        var B1 = iterator.NextRange();
                        var A2 = iterator.NextRange();
                        var B2 = iterator.NextRange();
                        var A1Len = A1.End - A1.Start;
                        var B1Len = B1.End - B1.Start;
                        var A2Len = A2.End - A2.Start;
                        var B2Len = B2.End - B2.Start;

                        // Merge A1 and B1 into cache
                        if (s.Compare(B1.End - 1, A1.Start) < 0)
                        {
                            // Reverse order: copy B1 then A1 into cache
                            s.CopyTo(A1.Start, cache, B1Len, A1Len);
                            s.CopyTo(B1.Start, cache, 0, B1Len);
                        }
                        else if (s.Compare(B1.Start, A1.End - 1) < 0)
                        {
                            MergeIntoCache(s, A1, B1, cache, 0);
                        }
                        else
                        {
                            // Already in order; check if everything is in order
                            if (s.Compare(B2.Start, A2.End - 1) >= 0 && s.Compare(A2.Start, B1.End - 1) >= 0) continue;
                            s.CopyTo(A1.Start, cache, 0, A1Len);
                            s.CopyTo(B1.Start, cache, A1Len, B1Len);
                        }
                        var mergedA1Len = A1Len + B1Len;

                        // Merge A2 and B2 into cache (after A1+B1)
                        if (s.Compare(B2.End - 1, A2.Start) < 0)
                        {
                            s.CopyTo(A2.Start, cache, mergedA1Len + B2Len, A2Len);
                            s.CopyTo(B2.Start, cache, mergedA1Len, B2Len);
                        }
                        else if (s.Compare(B2.Start, A2.End - 1) < 0)
                        {
                            MergeIntoCache(s, A2, B2, cache, mergedA1Len);
                        }
                        else
                        {
                            s.CopyTo(A2.Start, cache, mergedA1Len, A2Len);
                            s.CopyTo(B2.Start, cache, mergedA1Len + A2Len, B2Len);
                        }
                        var mergedA2Len = A2Len + B2Len;

                        // Merge the two merged results from cache back into items
                        var A3 = new BlockRange(0, mergedA1Len);
                        var B3 = new BlockRange(mergedA1Len, mergedA1Len + mergedA2Len);

                        if (cache.Compare(B3.End - 1, A3.Start) < 0)
                        {
                            cache.CopyTo(A3.Start, s, A1.Start + mergedA2Len, A3.Length());
                            cache.CopyTo(B3.Start, s, A1.Start, B3.Length());
                        }
                        else if (cache.Compare(B3.Start, A3.End - 1) < 0)
                        {
                            MergeFromCache(cache, A3, B3, s, A1.Start);
                        }
                        else
                        {
                            cache.CopyTo(A3.Start, s, A1.Start, A3.Length());
                            cache.CopyTo(B3.Start, s, A1.Start + mergedA1Len, B3.Length());
                        }
                    }

                    // Merged two levels at once
                    _ = iterator.NextLevel();
                }
                else
                {
                    // Single-level cache-based merge
                    iterator.Begin();
                    while (!iterator.Finished())
                    {
                        var A = iterator.NextRange();
                        var B = iterator.NextRange();
                        var ALen = A.End - A.Start;

                        if (s.Compare(B.End - 1, A.Start) < 0)
                        {
                            Rotate(s, A.Start, B.End, ALen);
                        }
                        else if (s.Compare(B.Start, A.End - 1) < 0)
                        {
                            // Copy A into cache, then merge externally
                            s.CopyTo(A.Start, cache, 0, ALen);
                            MergeExternal(s, A, B, cache);
                        }
                    }
                }
            }
            else
            {
                // Large levels: in-place block merging
                BlockMergeLevel(s, ref iterator, cache);
            }

            if (!iterator.NextLevel()) break;
        }
    }

    // Sorting network swap (stable via order tracking)

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void NetworkSwap<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, Span<byte> order, int offset, int x, int y)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var ix = offset + x;
        var iy = offset + y;
        var cmp = s.Compare(iy, ix);
        if (cmp < 0 || (order[x] > order[y] && cmp == 0))
        {
            s.Swap(ix, iy);
            (order[x], order[y]) = (order[y], order[x]);
        }
    }

    // Iterator: distributes n elements into evenly-sized ranges

    ref struct BlockIterator
    {
        int _size;
        int _powerOfTwo;
        int _numerator;
        int _decimal;
        int _denominator;
        int _decimalStep;
        int _numeratorStep;

        public BlockIterator(int size, int minLevel)
        {
            _powerOfTwo = FloorPowerOfTwo(size);
            _denominator = _powerOfTwo / minLevel;
            _size = size;
            _numerator = 0;
            _decimal = 0;
            _decimalStep = size / _denominator;
            _numeratorStep = size % _denominator;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Begin()
        {
            _numerator = 0;
            _decimal = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlockRange NextRange()
        {
            var start = _decimal;
            _decimal += _decimalStep;
            _numerator += _numeratorStep;
            if (_numerator >= _denominator)
            {
                _numerator -= _denominator;
                _decimal += 1;
            }
            return new BlockRange(start, _decimal);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Finished() => _decimal >= _size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool NextLevel()
        {
            _decimalStep += _decimalStep;
            _numeratorStep += _numeratorStep;
            if (_numeratorStep >= _denominator)
            {
                _numeratorStep -= _denominator;
                _decimalStep += 1;
            }
            return _decimalStep < _size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Length() => _decimalStep;

        static int FloorPowerOfTwo(int n)
        {
            n |= n >> 1;
            n |= n >> 2;
            n |= n >> 4;
            n |= n >> 8;
            n |= n >> 16;
            return n - (n >> 1);
        }
    }

    // Range and Pull structs

    readonly record struct BlockRange(int Start, int End)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Length() => End - Start;
    }

    struct Pull
    {
        public int From;
        public int To;
        public int Count;
        public BlockRange Range;
    }

    // Block merge for large levels (in-place)

    static void BlockMergeLevel<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, ref BlockIterator iterator, SortSpan<T, TComparer, TContext> cache)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var blockSize = IntSqrt(iterator.Length());
        var bufferSize = iterator.Length() / blockSize + 1;

        var A = new BlockRange(0, 0);
        var B = new BlockRange(0, 0);
        int index, last, count, find, start, pullIndex;

        Span<Pull> pull = stackalloc Pull[2];

        var buffer1 = new BlockRange(0, 0);
        var buffer2 = new BlockRange(0, 0);

        // Find two internal buffers of size 'bufferSize' each
        find = bufferSize + bufferSize;
        var findSeparately = false;

        if (blockSize <= CacheSize)
        {
            find = bufferSize;
        }
        else if (find > iterator.Length())
        {
            find = bufferSize;
            findSeparately = true;
        }

        pullIndex = 0;
        iterator.Begin();
        while (!iterator.Finished())
        {
            A = iterator.NextRange();
            B = iterator.NextRange();

            // Check A for unique values
            last = A.Start;
            count = 1;
            while (count < find)
            {
                index = FindLastForward(s, s.Read(last), new BlockRange(last + 1, A.End), find - count);
                if (index == A.End) break;
                last = index;
                count++;
            }
            index = last;

            if (count >= bufferSize)
            {
                pull[pullIndex] = new Pull { Range = new BlockRange(A.Start, B.End), Count = count, From = index, To = A.Start };
                pullIndex = 1;

                if (count == bufferSize + bufferSize)
                {
                    buffer1 = new BlockRange(A.Start, A.Start + bufferSize);
                    buffer2 = new BlockRange(A.Start + bufferSize, A.Start + count);
                    break;
                }
                else if (find == bufferSize + bufferSize)
                {
                    buffer1 = new BlockRange(A.Start, A.Start + count);
                    find = bufferSize;
                }
                else if (blockSize <= CacheSize)
                {
                    buffer1 = new BlockRange(A.Start, A.Start + count);
                    break;
                }
                else if (findSeparately)
                {
                    buffer1 = new BlockRange(A.Start, A.Start + count);
                    findSeparately = false;
                }
                else
                {
                    buffer2 = new BlockRange(A.Start, A.Start + count);
                    break;
                }
            }
            else if (pullIndex == 0 && count > buffer1.Length())
            {
                buffer1 = new BlockRange(A.Start, A.Start + count);
                pull[pullIndex] = new Pull { Range = new BlockRange(A.Start, B.End), Count = count, From = index, To = A.Start };
            }

            // Check B for unique values
            last = B.End - 1;
            count = 1;
            while (count < find)
            {
                index = FindFirstBackward(s, s.Read(last), new BlockRange(B.Start, last), find - count);
                if (index == B.Start) break;
                last = index - 1;
                count++;
            }
            index = last;

            if (count >= bufferSize)
            {
                pull[pullIndex] = new Pull { Range = new BlockRange(A.Start, B.End), Count = count, From = index, To = B.End };
                pullIndex = 1;

                if (count == bufferSize + bufferSize)
                {
                    buffer1 = new BlockRange(B.End - count, B.End - bufferSize);
                    buffer2 = new BlockRange(B.End - bufferSize, B.End);
                    break;
                }
                else if (find == bufferSize + bufferSize)
                {
                    buffer1 = new BlockRange(B.End - count, B.End);
                    find = bufferSize;
                }
                else if (blockSize <= CacheSize)
                {
                    buffer1 = new BlockRange(B.End - count, B.End);
                    break;
                }
                else if (findSeparately)
                {
                    buffer1 = new BlockRange(B.End - count, B.End);
                    findSeparately = false;
                }
                else
                {
                    if (pull[0].Range.Start == A.Start) pull[0] = pull[0] with { Range = new BlockRange(pull[0].Range.Start, pull[0].Range.End - pull[1].Count) };
                    buffer2 = new BlockRange(B.End - count, B.End);
                    break;
                }
            }
            else if (pullIndex == 0 && count > buffer1.Length())
            {
                buffer1 = new BlockRange(B.End - count, B.End);
                pull[pullIndex] = new Pull { Range = new BlockRange(A.Start, B.End), Count = count, From = index, To = B.End };
            }
        }

        // Pull out the two ranges for internal buffers
        for (pullIndex = 0; pullIndex < 2; pullIndex++)
        {
            var length = pull[pullIndex].Count;
            if (pull[pullIndex].To < pull[pullIndex].From)
            {
                // Pulling left
                index = pull[pullIndex].From;
                count = 1;
                while (count < length)
                {
                    index = FindFirstBackward(s, s.Read(index - 1), new BlockRange(pull[pullIndex].To, pull[pullIndex].From - (count - 1)), length - count);
                    var rStart = index + 1;
                    var rEnd = pull[pullIndex].From + 1;
                    Rotate(s, rStart, rEnd, rEnd - rStart - count);
                    pull[pullIndex] = pull[pullIndex] with { From = index + count };
                    count++;
                }
            }
            else if (pull[pullIndex].To > pull[pullIndex].From)
            {
                // Pulling right
                index = pull[pullIndex].From + 1;
                count = 1;
                while (count < length)
                {
                    index = FindLastForward(s, s.Read(index), new BlockRange(index, pull[pullIndex].To), length - count);
                    var rStart = pull[pullIndex].From;
                    var rEnd = index - 1;
                    Rotate(s, rStart, rEnd, count);
                    pull[pullIndex] = pull[pullIndex] with { From = index - 1 - count };
                    count++;
                }
            }
        }

        // Adjust block_size and buffer_size
        bufferSize = buffer1.Length();
        if (bufferSize > 0)
            blockSize = iterator.Length() / bufferSize + 1;

        // Now merge each A+B combination
        iterator.Begin();
        while (!iterator.Finished())
        {
            A = iterator.NextRange();
            B = iterator.NextRange();

            // Remove parts used by internal buffers
            start = A.Start;
            if (start == pull[0].Range.Start)
            {
                if (pull[0].From > pull[0].To)
                {
                    A = new BlockRange(A.Start + pull[0].Count, A.End);
                    if (A.Length() == 0) continue;
                }
                else if (pull[0].From < pull[0].To)
                {
                    B = new BlockRange(B.Start, B.End - pull[0].Count);
                    if (B.Length() == 0) continue;
                }
            }
            if (start == pull[1].Range.Start)
            {
                if (pull[1].From > pull[1].To)
                {
                    A = new BlockRange(A.Start + pull[1].Count, A.End);
                    if (A.Length() == 0) continue;
                }
                else if (pull[1].From < pull[1].To)
                {
                    B = new BlockRange(B.Start, B.End - pull[1].Count);
                    if (B.Length() == 0) continue;
                }
            }

            if (s.Compare(B.End - 1, A.Start) < 0)
            {
                // Reverse order: rotate
                Rotate(s, A.Start, B.End, A.Length());
            }
            else if (s.Compare(A.End, A.End - 1) < 0)
            {
                // Need to merge
                var blockA = new BlockRange(A.Start, A.End);
                var firstA = new BlockRange(A.Start, A.Start + blockA.Length() % blockSize);

                // Swap first value of each A block with buffer1
                var indexA = buffer1.Start;
                index = firstA.End;
                while (index < blockA.End)
                {
                    s.Swap(indexA, index);
                    indexA++;
                    index += blockSize;
                }

                // Roll A blocks through B blocks
                var lastA = firstA;
                var lastB = new BlockRange(0, 0);
                var blockB = new BlockRange(B.Start, B.Start + Math.Min(blockSize, B.Length()));
                blockA = new BlockRange(blockA.Start + firstA.Length(), blockA.End);
                indexA = buffer1.Start;

                // Copy firstA into cache or buffer2
                if (lastA.Length() <= CacheSize)
                {
                    s.CopyTo(lastA.Start, cache, 0, lastA.Length());
                }
                else if (buffer2.Length() > 0)
                {
                    BlockSwap(s, lastA.Start, buffer2.Start, lastA.Length());
                }

                if (blockA.Length() > 0)
                {
                    while (true)
                    {
                        if ((lastB.Length() > 0 && s.Compare(lastB.End - 1, s.Read(indexA)) >= 0) || blockB.Length() == 0)
                        {
                            // Drop minimum A block
                            var BSplit = BinaryFirst(s, s.Read(indexA), lastB);
                            var BRemaining = lastB.End - BSplit;

                            // Find minimum A block
                            var minA = blockA.Start;
                            var findA = minA + blockSize;
                            while (findA < blockA.End)
                            {
                                if (s.Compare(findA, minA) < 0)
                                    minA = findA;
                                findA += blockSize;
                            }
                            BlockSwap(s, blockA.Start, minA, blockSize);

                            // Restore tagged value
                            s.Swap(blockA.Start, indexA);
                            indexA++;

                            // Merge previous A block with B values
                            if (lastA.Length() <= CacheSize)
                            {
                                MergeExternal(s, lastA, new BlockRange(lastA.End, BSplit), cache);
                            }
                            else if (buffer2.Length() > 0)
                            {
                                MergeInternal(s, lastA, new BlockRange(lastA.End, BSplit), buffer2);
                            }
                            else
                            {
                                MergeInPlace(s, lastA, new BlockRange(lastA.End, BSplit));
                            }

                            if (buffer2.Length() > 0 || blockSize <= CacheSize)
                            {
                                if (blockSize <= CacheSize)
                                {
                                    s.CopyTo(blockA.Start, cache, 0, blockSize);
                                }
                                else
                                {
                                    BlockSwap(s, blockA.Start, buffer2.Start, blockSize);
                                }
                                BlockSwap(s, BSplit, blockA.Start + blockSize - BRemaining, BRemaining);
                            }
                            else
                            {
                                Rotate(s, BSplit, blockA.Start + blockSize, blockA.Start - BSplit);
                            }

                            lastA = new BlockRange(blockA.Start - BRemaining, blockA.Start - BRemaining + blockSize);
                            lastB = new BlockRange(lastA.End, lastA.End + BRemaining);

                            blockA = new BlockRange(blockA.Start + blockSize, blockA.End);
                            if (blockA.Length() == 0) break;
                        }
                        else if (blockB.Length() < blockSize)
                        {
                            // Move unevenly sized last B block before remaining A blocks
                            Rotate(s, blockA.Start, blockB.End, blockB.Start - blockA.Start);

                            lastB = new BlockRange(blockA.Start, blockA.Start + blockB.Length());
                            blockA = new BlockRange(blockA.Start + blockB.Length(), blockA.End + blockB.Length());
                            blockB = new BlockRange(blockB.Start, blockB.Start); // empty
                        }
                        else
                        {
                            // Roll leftmost A block to end by swapping with next B block
                            BlockSwap(s, blockA.Start, blockB.Start, blockSize);
                            lastB = new BlockRange(blockA.Start, blockA.Start + blockSize);

                            blockA = new BlockRange(blockA.Start + blockSize, blockA.End + blockSize);
                            blockB = new BlockRange(blockB.Start + blockSize,
                                blockB.End > B.End - blockSize ? B.End : blockB.End + blockSize);
                        }
                    }
                }

                // Merge last A block with remaining B values
                if (lastA.Length() <= CacheSize)
                {
                    MergeExternal(s, lastA, new BlockRange(lastA.End, B.End), cache);
                }
                else if (buffer2.Length() > 0)
                {
                    MergeInternal(s, lastA, new BlockRange(lastA.End, B.End), buffer2);
                }
                else
                {
                    MergeInPlace(s, lastA, new BlockRange(lastA.End, B.End));
                }
            }
        }

        // Insertion sort buffer2, then redistribute buffers back
        if (buffer2.Length() > 0)
        {
            InsertionSort.SortCore(s, buffer2.Start, buffer2.End);
        }

        for (pullIndex = 0; pullIndex < 2; pullIndex++)
        {
            var unique = pull[pullIndex].Count * 2;
            if (pull[pullIndex].From > pull[pullIndex].To)
            {
                // Redistribute left
                var buffer = new BlockRange(pull[pullIndex].Range.Start, pull[pullIndex].Range.Start + pull[pullIndex].Count);
                while (buffer.Length() > 0)
                {
                    index = FindFirstForward(s, s.Read(buffer.Start), new BlockRange(buffer.End, pull[pullIndex].Range.End), unique);
                    var amount = index - buffer.End;
                    Rotate(s, buffer.Start, index, buffer.Length());
                    buffer = new BlockRange(buffer.Start + amount + 1, buffer.End + amount);
                    unique -= 2;
                }
            }
            else if (pull[pullIndex].From < pull[pullIndex].To)
            {
                // Redistribute right
                var buffer = new BlockRange(pull[pullIndex].Range.End - pull[pullIndex].Count, pull[pullIndex].Range.End);
                while (buffer.Length() > 0)
                {
                    index = FindLastBackward(s, s.Read(buffer.End - 1), new BlockRange(pull[pullIndex].Range.Start, buffer.Start), unique);
                    var amount = buffer.Start - index;
                    Rotate(s, index, buffer.End, amount);
                    buffer = new BlockRange(buffer.Start - amount, buffer.End - amount - 1);
                    unique -= 2;
                }
            }
        }
    }

    // Binary search helpers

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int BinaryFirst<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, T value, BlockRange range)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var curr = range.Start;
        var size = range.Length();
        if (range.Start >= range.End) return range.End;
        while (size > 0)
        {
            var offset = size % 2;
            size /= 2;
            if (s.Compare(curr + size, value) < 0)
            {
                curr += size + offset;
            }
        }
        return curr;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int BinaryLast<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, T value, BlockRange range)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var curr = range.Start;
        var size = range.Length();
        if (range.Start >= range.End) return range.End;
        while (size > 0)
        {
            var offset = size % 2;
            size /= 2;
            if (s.Compare(value, curr + size) >= 0)
            {
                curr += size + offset;
            }
        }
        return curr;
    }

    // Find helpers (linear + binary search combo)

    static int FindFirstForward<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, T value, BlockRange range, int unique)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (range.Length() == 0) return range.Start;
        var skip = Math.Max(range.Length() / unique, 1);
        var index = range.Start + skip;
        while (s.Compare(index - 1, value) < 0)
        {
            if (index >= range.End - skip)
                return BinaryFirst(s, value, new BlockRange(index, range.End));
            index += skip;
        }
        return BinaryFirst(s, value, new BlockRange(index - skip, index));
    }

    static int FindFirstBackward<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, T value, BlockRange range, int unique)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (range.Length() == 0) return range.Start;
        var skip = Math.Max(range.Length() / unique, 1);
        var index = range.End - skip;
        while (index > range.Start && s.Compare(index - 1, value) >= 0)
        {
            if (index < range.Start + skip)
                return BinaryFirst(s, value, new BlockRange(range.Start, index));
            index -= skip;
        }
        return BinaryFirst(s, value, new BlockRange(index, index + skip));
    }

    static int FindLastForward<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, T value, BlockRange range, int unique)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (range.Length() == 0) return range.Start;
        var skip = Math.Max(range.Length() / unique, 1);
        var index = range.Start + skip;
        while (s.Compare(value, index - 1) >= 0)
        {
            if (index >= range.End - skip)
                return BinaryLast(s, value, new BlockRange(index, range.End));
            index += skip;
        }
        return BinaryLast(s, value, new BlockRange(index - skip, index));
    }

    static int FindLastBackward<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, T value, BlockRange range, int unique)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (range.Length() == 0) return range.Start;
        var skip = Math.Max(range.Length() / unique, 1);
        var index = range.End - skip;
        while (index > range.Start && s.Compare(value, index - 1) < 0)
        {
            if (index < range.Start + skip)
                return BinaryLast(s, value, new BlockRange(range.Start, index));
            index -= skip;
        }
        return BinaryLast(s, value, new BlockRange(index, index + skip));
    }

    // Block swap

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void BlockSwap<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int start1, int start2, int blockSize)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        for (var i = 0; i < blockSize; i++)
        {
            s.Swap(start1 + i, start2 + i);
        }
    }

    // Rotation: left-rotate items[start..end) by 'amount' positions

    static void Rotate<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int start, int end, int amount)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = end - start;
        if (n == 0 || amount == 0 || amount == n) return;
        Reverse(s, start, start + amount);
        Reverse(s, start + amount, end);
        Reverse(s, start, end);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Reverse<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int lo, int hi)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        hi--;
        while (lo < hi)
        {
            s.Swap(lo, hi);
            lo++;
            hi--;
        }
    }

    // Merge operations

    /// <summary>
    /// In-place merge without a buffer. Uses binary search + rotate.
    /// Only called when no internal buffers could be extracted.
    /// </summary>
    static void MergeInPlace<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, BlockRange A, BlockRange B)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (A.Length() == 0 || B.Length() == 0) return;

        var aStart = A.Start;
        var aEnd = A.End;
        var bStart = B.Start;
        var bEnd = B.End;

        while (true)
        {
            var mid = BinaryFirst(s, s.Read(aStart), new BlockRange(bStart, bEnd));
            var amount = mid - aEnd;
            Rotate(s, aStart, mid, aEnd - aStart);
            if (bEnd == mid) break;

            bStart = mid;
            aStart += amount;
            aEnd = bStart;
            aStart = BinaryLast(s, s.Read(aStart), new BlockRange(aStart, aEnd));
            if (aStart == aEnd) break;
        }
    }

    /// <summary>
    /// Merge using an internal buffer. Swaps A into buffer, merges from buffer + B into A's position.
    /// </summary>
    static void MergeInternal<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, BlockRange A, BlockRange B, BlockRange buffer)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var aCount = 0;
        var bCount = 0;
        var insert = 0;
        var aLen = A.Length();
        var bLen = B.Length();

        if (bLen > 0 && aLen > 0)
        {
            while (true)
            {
                if (s.Compare(B.Start + bCount, buffer.Start + aCount) >= 0)
                {
                    s.Swap(A.Start + insert, buffer.Start + aCount);
                    aCount++;
                    insert++;
                    if (aCount >= aLen) break;
                }
                else
                {
                    s.Swap(A.Start + insert, B.Start + bCount);
                    bCount++;
                    insert++;
                    if (bCount >= bLen) break;
                }
            }
        }

        BlockSwap(s, buffer.Start + aCount, A.Start + insert, aLen - aCount);
    }

    /// <summary>
    /// Merge using external cache. A is already copied into cache[0..A.Length()].
    /// </summary>
    static void MergeExternal<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, BlockRange A, BlockRange B, SortSpan<T, TComparer, TContext> cache)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var aIndex = 0;
        var bIndex = B.Start;
        var insertIndex = A.Start;
        var aLast = A.Length();
        var bLast = B.End;

        if (B.Length() > 0 && A.Length() > 0)
        {
            while (true)
            {
                if (s.Compare(cache.Read(aIndex), s.Read(bIndex)) <= 0)
                {
                    s.Write(insertIndex++, cache.Read(aIndex++));
                    if (aIndex == aLast) break;
                }
                else
                {
                    s.Write(insertIndex++, s.Read(bIndex++));
                    if (bIndex == bLast) break;
                }
            }
        }

        // Copy remainder of A from cache
        cache.CopyTo(aIndex, s, insertIndex, aLast - aIndex);
    }

    /// <summary>
    /// Merge A and B from items into cache starting at the given cacheOffset.
    /// </summary>
    static void MergeIntoCache<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, BlockRange A, BlockRange B, SortSpan<T, TComparer, TContext> cache, int cacheOffset)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var aIndex = A.Start;
        var bIndex = B.Start;
        var insertIndex = cacheOffset;

        while (true)
        {
            if (s.Compare(aIndex, bIndex) <= 0)
            {
                cache.Write(insertIndex++, s.Read(aIndex++));
                if (aIndex == A.End)
                {
                    s.CopyTo(bIndex, cache, insertIndex, B.End - bIndex);
                    break;
                }
            }
            else
            {
                cache.Write(insertIndex++, s.Read(bIndex++));
                if (bIndex == B.End)
                {
                    s.CopyTo(aIndex, cache, insertIndex, A.End - aIndex);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Merge A3 and B3 from cache back into items at the given destination.
    /// </summary>
    static void MergeFromCache<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> cache, BlockRange A, BlockRange B, SortSpan<T, TComparer, TContext> s, int destStart)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var aIndex = A.Start;
        var bIndex = B.Start;
        var insertIndex = destStart;

        while (true)
        {
            if (cache.Compare(aIndex, bIndex) <= 0)
            {
                s.Write(insertIndex++, cache.Read(aIndex++));
                if (aIndex == A.End)
                {
                    cache.CopyTo(bIndex, s, insertIndex, B.End - bIndex);
                    break;
                }
            }
            else
            {
                s.Write(insertIndex++, cache.Read(bIndex++));
                if (bIndex == B.End)
                {
                    cache.CopyTo(aIndex, s, insertIndex, A.End - aIndex);
                    break;
                }
            }
        }
    }


    // Integer square root

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int IntSqrt(int n)
    {
        var r = (int)Math.Sqrt(n);
        while (r * r > n) r--;
        while ((r + 1) * (r + 1) <= n) r++;
        return r;
    }
}
