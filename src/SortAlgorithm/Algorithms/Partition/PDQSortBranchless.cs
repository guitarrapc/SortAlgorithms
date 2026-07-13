using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// PDQSort の branchless パーティション版です。分岐予測ミスに支配される比較駆動のパーティションを、
/// オフセットバッファへの分岐レス分類(BlockQuicksort 由来)に置き換えることで、ランダムデータや
/// 重複の多いデータで大幅な高速化を実現します。SIMD は使用しない純スカラー実装です。
/// <br/>
/// PDQSort variant with branchless partitioning. Replaces the comparison-driven partition loop
/// (dominated by branch mispredictions on random data) with branch-free classification into
/// offset buffers, derived from BlockQuicksort. Pure scalar code - no SIMD.
/// </summary>
/// <remarks>
/// <para><strong>Difference from <see cref="PDQSort"/>:</strong></para>
/// <para>
/// The only structural difference is PartitionRight: instead of the bidirectional swap loop
/// (one data-dependent branch per element, ~50% mispredicted on random input), elements are
/// classified into two 64-entry offset buffers using flag arithmetic (cmp + setcc + add,
/// no branch on data), then misplaced pairs are exchanged with a cyclic permutation.
/// This is the "block partition" of orlp/pdqsort (pdqsort.h, partition_right_branchless),
/// derived from "BlockQuicksort: How Branch Mispredictions don't affect Quicksort"
/// by Stefan Edelkamp and Armin Weiss (https://arxiv.org/abs/1604.06697).
/// </para>
/// <para><strong>Implementation notes specific to .NET (measured via BenchmarkDotNet + DisassemblyDiagnoser):</strong></para>
/// <list type="bullet">
/// <item><description>Offset buffers are stackalloc'd and accessed via <see cref="MemoryMarshal.GetReference{T}(Span{T})"/> +
/// <see cref="Unsafe.Add{T}(ref T, int)"/>: the store index is a runtime value the JIT cannot prove in-range,
/// so Span indexing would emit a bounds check per classified element (measured 5-15% loss).</description></item>
/// <item><description><c>Unsafe.As&lt;bool, byte&gt;</c> converts the comparison result to an addend, which RyuJIT
/// compiles to setcc (no data-dependent branch). A ternary (b ? 1 : 0) is not guaranteed branch-free.</description></item>
/// <item><description>The classification loops are manually unrolled 8x like orlp's implementation;
/// RyuJIT does not unroll them automatically (measured -13% on many-duplicates inputs).</description></item>
/// </list>
/// <para><strong>Measured trade-offs vs <see cref="PDQSort"/> (int keys, Ryzen 9 7950X3D, .NET 10):</strong></para>
/// <list type="bullet">
/// <item><description>Random n=8192: 0.62x (38% faster), n=65536: 0.59x. Measured against fresh arrays per
/// invocation; re-sorting one identical array lets the branch predictor memorize small inputs and
/// understates the branchless advantage.</description></item>
/// <item><description>16 distinct values n=65536: 0.40x (2.5x faster)</description></item>
/// <item><description>Sorted / nearly-sorted: unchanged (alreadyPartitioned path skips the block machinery)</description></item>
/// <item><description>Reversed / pipe-organ: ~1.5x slower than branchy PDQSort (classification is pure overhead
/// when branches are perfectly predictable) - still several times faster than IntroSort on these
/// patterns thanks to pattern detection. This is the same trade-off orlp/pdqsort accepts.</description></item>
/// <item><description>n=256 random: ~1.2x slower (block machinery fixed costs); crossover is around n≈512-1024.</description></item>
/// </list>
/// <para>All other components (ninther pivot selection, equal-block skip, partition_left for duplicates,
/// partial insertion sort pattern detection, pattern-defeating shuffles, heapsort fallback, tail recursion
/// elimination) are identical to <see cref="PDQSort"/> - see its documentation for the theory.</para>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Hybrid (Partition (base) + Heap + Insertion)</description></item>
/// <item><description>Stable      : No</description></item>
/// <item><description>In-place    : Yes (O(log n) stack + 128 bytes of offset buffers per partition call)</description></item>
/// <item><description>Best case   : O(n) - Sorted, reverse sorted, all equal elements</description></item>
/// <item><description>Average case: Θ(n log n)</description></item>
/// <item><description>Worst case  : O(n log n) - HeapSort fallback</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Paper: https://arxiv.org/abs/2106.05123 (pdqsort), https://arxiv.org/abs/1604.06697 (BlockQuicksort)</para>
/// <para>YouTube: https://www.youtube.com/watch?v=jz-PBiWwNjc</para>
/// <para>Other implementation: https://github.com/orlp/pdqsort (partition_right_branchless)</para>
/// </remarks>
public static class PDQSortBranchless
{
    // Constants (identical to PDQSort)
    private const int InsertionSortThreshold = 24;
    private const int NintherThreshold = 128;
    private const int PartialInsertionSortLimit = 8;

    // Block partition constants: 64 offsets per side fit both offset buffers in two cache
    // lines and keep every offset within a byte (orlp uses the same block size for the
    // non-cache-line-aligned configuration).
    private const int BlockSize = 64;

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array

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
    /// <typeparam name="TComparer">The type of the comparer.</typeparam>
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

        // For floating-point types, move NaN values to the front
        // This improves performance and enhances PDQSort's pattern detection
        int nanEnd = FloatingPointUtils.MoveNaNsToFront(s, first, last);

        if (nanEnd >= last)
        {
            // All values are NaN, already "sorted"
            return;
        }

        // Sort the non-NaN portion
        var badAllowed = Log2(last - nanEnd);
        PDQSortLoop(s, nanEnd, last, badAllowed, true);
    }

    /// <summary>
    /// Internal entry point for other algorithms that already hold a <see cref="SortSpan{T,TComparer,TContext}"/>.
    /// Sorts the subrange [first..last) using branchless PDQSort.
    /// </summary>
    internal static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (last - first <= 1) return;
        var badAllowed = Log2(last - first);
        PDQSortLoop(s, first, last, badAllowed, true);
    }

    /// <summary>
    /// Main PDQSort loop with tail recursion elimination. Identical to <see cref="PDQSort"/>'s loop
    /// except that PartitionRight is the branchless block partition.
    /// </summary>
    private static void PDQSortLoop<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int begin, int end, int badAllowed, bool leftmost)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (true)
        {
            var size = end - begin;

            // Use insertion sort for small arrays
            if (size < InsertionSortThreshold)
            {
                s.Context.OnRole(begin, BUFFER_MAIN, RoleType.None);
                s.Context.OnPhase(SortPhase.HybridToInsertionSort, begin, end - 1, InsertionSortThreshold);

                if (leftmost)
                {
                    InsertionSort.SortCore(s, begin, end);
                }
                else
                {
                    InsertionSort.UnguardedSortCore(s, begin, end);
                }
                return;
            }

            // Choose pivot as median of 3 or pseudomedian of 9 (ninther)
            var s2 = size / 2;
            if (size > NintherThreshold)
            {
                Sort3(s, begin, begin + s2, end - 1);
                Sort3(s, begin + 1, begin + (s2 - 1), end - 2);
                Sort3(s, begin + 2, begin + (s2 + 1), end - 3);
                Sort3(s, begin + (s2 - 1), begin + s2, begin + (s2 + 1));
                s.Swap(begin, begin + s2);
            }
            else
            {
                Sort3(s, begin + s2, begin, end - 1);
            }

            // If *(begin - 1) is the end of the right partition of a previous partition operation,
            // there is no element in [begin, end) that is smaller than *(begin - 1).
            // Then if our pivot compares equal to *(begin - 1) we change strategy.
            if (!leftmost && s.IsGreaterOrEqualAt(begin - 1, begin))
            {
                s.Context.OnPhase(SortPhase.QuickSortPartition, begin, end - 1, begin);
                s.Context.OnRole(begin, BUFFER_MAIN, RoleType.Pivot);
                var pivotBegin = begin;
                begin = PartitionLeft(s, begin, end) + 1;
                s.Context.OnRole(pivotBegin, BUFFER_MAIN, RoleType.None);
                continue;
            }

            // Partition and detect equal elements block
            s.Context.OnPhase(SortPhase.QuickSortPartition, begin, end - 1, begin);
            s.Context.OnRole(begin, BUFFER_MAIN, RoleType.Pivot);
            var (equalLeft, equalRight, alreadyPartitioned) = PartitionRightSkipEquals(s, begin, end);
            s.Context.OnRole(begin, BUFFER_MAIN, RoleType.None);

            // Calculate sizes excluding the equal block
            var lSize = equalLeft - begin;        // Elements < pivot
            var rSize = end - equalRight;          // Elements > pivot
            var eqSize = equalRight - equalLeft;         // Elements == pivot (to be excluded from recursion)
            var effective = size - eqSize;  // Effective size excluding equal elements

            // Check for highly unbalanced partition (using effective size)
            var highlyUnbalanced = effective > 0 && (lSize < effective / 8 || rSize < effective / 8);

            // If we got a highly unbalanced partition we shuffle elements to break many patterns
            if (highlyUnbalanced)
            {
                s.Context.OnPhase(SortPhase.PDQPatternShuffle, begin, end - 1, badAllowed);
                // If we had too many bad partitions, switch to heapsort to guarantee O(n log n)
                if (--badAllowed == 0)
                {
                    s.Context.OnPhase(SortPhase.HybridToHeapSort, begin, end - 1);
                    HeapSort.SortCore(s, begin, end);
                    return;
                }

                if (lSize >= InsertionSortThreshold)
                {
                    s.Swap(begin, begin + lSize / 4);
                    s.Swap(equalLeft - 1, equalLeft - lSize / 4);

                    if (lSize > NintherThreshold)
                    {
                        s.Swap(begin + 1, begin + (lSize / 4 + 1));
                        s.Swap(begin + 2, begin + (lSize / 4 + 2));
                        s.Swap(equalLeft - 2, equalLeft - (lSize / 4 + 1));
                        s.Swap(equalLeft - 3, equalLeft - (lSize / 4 + 2));
                    }
                }

                if (rSize >= InsertionSortThreshold)
                {
                    s.Swap(equalRight, equalRight + rSize / 4);
                    s.Swap(end - 1, end - rSize / 4);

                    if (rSize > NintherThreshold)
                    {
                        s.Swap(equalRight + 1, equalRight + (1 + rSize / 4));
                        s.Swap(equalRight + 2, equalRight + (2 + rSize / 4));
                        s.Swap(end - 2, end - (1 + rSize / 4));
                        s.Swap(end - 3, end - (2 + rSize / 4));
                    }
                }
            }
            else
            {
                // If we were decently balanced and we tried to sort an already partitioned
                // sequence try to use insertion sort (excluding equal elements block)
                if (alreadyPartitioned)
                {
                    s.Context.OnPhase(SortPhase.PDQPartialInsertionSort, begin, end - 1);
                    if (PartialInsertionSort(s, begin, equalLeft) &&
                        PartialInsertionSort(s, equalRight, end))
                    {
                        return;
                    }
                }
            }

            // Tail recursion optimization: always recurse on smaller partition, loop on larger
            var leftSize = equalLeft - begin;
            var rightSize = end - equalRight;

            if (leftSize < rightSize)
            {
                // Recurse on smaller left partition (preserves leftmost flag)
                PDQSortLoop(s, begin, equalLeft, badAllowed, leftmost);
                // Tail recursion: continue loop with larger right partition
                begin = equalRight;
                leftmost = false;
            }
            else
            {
                // Recurse on smaller right partition (always non-leftmost)
                PDQSortLoop(s, equalRight, end, badAllowed, false);
                // Tail recursion: continue loop with larger left partition
                end = equalLeft;
                // Preserve leftmost flag for left partition
            }
        }
    }

    /// <summary>
    /// Partitions using the branchless PartitionRight and then detects consecutive elements
    /// equal to the pivot, returning the bounds of the equal block to exclude from recursion.
    /// Same contract as PDQSort.PartitionRightSkipEquals.
    /// </summary>
    private static (int eqL, int eqR, bool alreadyPartitioned) PartitionRightSkipEquals<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int begin, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var (pivotPos, alreadyPartitioned) = PartitionRight(s, begin, end);

        // Read pivot value once to minimize SortSpan access
        var pivot = s.Read(pivotPos);

        // Expand left: find consecutive elements equal to pivot
        var eqL = pivotPos;
        while (eqL > begin && s.Compare(eqL - 1, pivot) == 0)
        {
            eqL--;
        }

        // Expand right: find consecutive elements equal to pivot
        var eqR = pivotPos + 1;
        while (eqR < end && s.Compare(eqR, pivot) == 0)
        {
            eqR++;
        }

        return (eqL, eqR, alreadyPartitioned);
    }

    /// <summary>
    /// Classifies one element from the left side: unconditionally stores its offset, then
    /// advances the buffer cursor only when the element belongs to the right partition.
    /// The bool-to-byte add compiles to cmp + setcc + add: no data-dependent branch.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ClassifyLeft<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, T pivot, ref byte offsetsL, ref int numL, ref int it, ref int i)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        Unsafe.Add(ref offsetsL, numL) = (byte)i;
        var wrongSide = s.IsGreaterOrEqual(s.Read(it), pivot);
        numL += Unsafe.As<bool, byte>(ref wrongSide);
        it++;
        i++;
    }

    /// <summary>
    /// Classifies one element from the right side (walking backwards from <c>last</c>).
    /// Offsets are 1-based distances from <c>last</c>, matching orlp's convention.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ClassifyRight<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, T pivot, ref byte offsetsR, ref int numR, ref int it, ref int i)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        Unsafe.Add(ref offsetsR, numR) = (byte)i;
        it--;
        var wrongSide = s.IsLessThan(s.Read(it), pivot);
        numR += Unsafe.As<bool, byte>(ref wrongSide);
        i++;
    }

    /// <summary>
    /// Branchless block partition: port of orlp's partition_right_branchless (pdqsort.h),
    /// derived from BlockQuicksort (Edelkamp/Weiss). Elements equal to the pivot go to the
    /// right partition - identical contract to PDQSort.PartitionRight.
    /// </summary>
    /// <remarks>
    /// Invariant during the block phase: [begin+1, first) &lt; pivot, [last, end) &gt;= pivot,
    /// [first, last) unclassified. Each round classifies up to one 64-element block per side
    /// into offset buffers, then swaps the misplaced pairs with a cyclic permutation
    /// (one read + one write per element instead of a full swap).
    /// </remarks>
    private static (int pivotPos, bool alreadyPartitioned) PartitionRight<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int begin, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Move pivot into local for speed
        var pivot = s.Read(begin);

        var first = begin;
        var last = end;

        // Find the first element greater than or equal to the pivot (the median of 3 guarantees this exists).
        do { first++; } while (s.IsLessThan(s.Read(first), pivot));

        // Find the first element strictly smaller than the pivot. We have to guard this search if
        // there was no element before *first.
        if (first - 1 == begin)
        {
            do { last--; } while (first < last && s.IsGreaterOrEqual(s.Read(last), pivot));
        }
        else
        {
            do { last--; } while (s.IsGreaterOrEqual(s.Read(last), pivot));
        }

        // If the first pair of elements that should be swapped to partition are the same element,
        // the passed in sequence already was correctly partitioned
        var alreadyPartitioned = first >= last;

        if (!alreadyPartitioned)
        {
            s.Swap(first, last);
            first++;

            Span<byte> offsetsLBuf = stackalloc byte[BlockSize];
            Span<byte> offsetsRBuf = stackalloc byte[BlockSize];
            // Runtime-varying store indices defeat the JIT's bounds-check elimination on Span
            // indexing; go through refs so classification stays check-free (see class remarks).
            ref var offsetsL = ref MemoryMarshal.GetReference(offsetsLBuf);
            ref var offsetsR = ref MemoryMarshal.GetReference(offsetsRBuf);
            var numL = 0;
            var numR = 0;
            var startL = 0;
            var startR = 0;

            while (last - first > 2 * BlockSize)
            {
                // Fill up offset blocks with indices of elements on the wrong side.
                if (numL == 0)
                {
                    startL = 0;
                    var it = first;
                    for (var i = 0; i < BlockSize;)
                    {
                        // 8x unroll, mirroring orlp's pdqsort.h (RyuJIT does not unroll this).
                        ClassifyLeft(s, pivot, ref offsetsL, ref numL, ref it, ref i);
                        ClassifyLeft(s, pivot, ref offsetsL, ref numL, ref it, ref i);
                        ClassifyLeft(s, pivot, ref offsetsL, ref numL, ref it, ref i);
                        ClassifyLeft(s, pivot, ref offsetsL, ref numL, ref it, ref i);
                        ClassifyLeft(s, pivot, ref offsetsL, ref numL, ref it, ref i);
                        ClassifyLeft(s, pivot, ref offsetsL, ref numL, ref it, ref i);
                        ClassifyLeft(s, pivot, ref offsetsL, ref numL, ref it, ref i);
                        ClassifyLeft(s, pivot, ref offsetsL, ref numL, ref it, ref i);
                    }
                }
                if (numR == 0)
                {
                    startR = 0;
                    var it = last;
                    for (var i = 1; i <= BlockSize;)
                    {
                        ClassifyRight(s, pivot, ref offsetsR, ref numR, ref it, ref i);
                        ClassifyRight(s, pivot, ref offsetsR, ref numR, ref it, ref i);
                        ClassifyRight(s, pivot, ref offsetsR, ref numR, ref it, ref i);
                        ClassifyRight(s, pivot, ref offsetsR, ref numR, ref it, ref i);
                        ClassifyRight(s, pivot, ref offsetsR, ref numR, ref it, ref i);
                        ClassifyRight(s, pivot, ref offsetsR, ref numR, ref it, ref i);
                        ClassifyRight(s, pivot, ref offsetsR, ref numR, ref it, ref i);
                        ClassifyRight(s, pivot, ref offsetsR, ref numR, ref it, ref i);
                    }
                }

                var num = Math.Min(numL, numR);
                SwapOffsets(s, first, last, ref Unsafe.Add(ref offsetsL, startL), ref Unsafe.Add(ref offsetsR, startR), num, numL == numR);
                numL -= num;
                numR -= num;
                startL += num;
                startR += num;
                if (numL == 0) first += BlockSize;
                if (numR == 0) last -= BlockSize;
            }

            // Handle the leftover partial blocks (at most 2 * BlockSize elements remain).
            var unknownLeft = (last - first) - ((numR > 0 || numL > 0) ? BlockSize : 0);
            int lSize, rSize;
            if (numR > 0)
            {
                // Right block unfinished: the unknown region belongs to the left block.
                lSize = unknownLeft;
                rSize = BlockSize;
            }
            else if (numL > 0)
            {
                lSize = BlockSize;
                rSize = unknownLeft;
            }
            else
            {
                // No leftover block, split the unknown elements in two blocks.
                lSize = unknownLeft / 2;
                rSize = unknownLeft - lSize;
            }

            if (unknownLeft > 0 && numL == 0)
            {
                startL = 0;
                var it = first;
                for (var i = 0; i < lSize;)
                {
                    ClassifyLeft(s, pivot, ref offsetsL, ref numL, ref it, ref i);
                }
            }
            if (unknownLeft > 0 && numR == 0)
            {
                startR = 0;
                var it = last;
                for (var i = 1; i <= rSize;)
                {
                    ClassifyRight(s, pivot, ref offsetsR, ref numR, ref it, ref i);
                }
            }

            var numFinal = Math.Min(numL, numR);
            SwapOffsets(s, first, last, ref Unsafe.Add(ref offsetsL, startL), ref Unsafe.Add(ref offsetsR, startR), numFinal, numL == numR);
            numL -= numFinal;
            numR -= numFinal;
            startL += numFinal;
            startR += numFinal;
            if (numL == 0) first += lSize;
            if (numR == 0) last -= rSize;

            // Only one side can have leftovers: swap them to the boundary one by one.
            if (numL > 0)
            {
                while (numL-- > 0)
                {
                    last--;
                    s.Swap(first + Unsafe.Add(ref offsetsL, startL + numL), last);
                }
                first = last;
            }
            if (numR > 0)
            {
                while (numR-- > 0)
                {
                    s.Swap(last - Unsafe.Add(ref offsetsR, startR + numR), first);
                    first++;
                }
                last = first;
            }
        }

        // Put the pivot in the right place
        var pivotPos = first - 1;
        s.Write(begin, s.Read(pivotPos));
        s.Write(pivotPos, pivot);

        return (pivotPos, alreadyPartitioned);
    }

    /// <summary>
    /// Exchanges misplaced elements between the two offset blocks. When both blocks drain
    /// together (numL == numR) plain swaps keep pdqsort O(n) on descending inputs; otherwise
    /// a cyclic permutation saves one write per element (orlp's swap_offsets).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SwapOffsets<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last, ref byte offsetsL, ref byte offsetsR, int num, bool useSwaps)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (useSwaps)
        {
            for (var i = 0; i < num; i++)
            {
                s.Swap(first + Unsafe.Add(ref offsetsL, i), last - Unsafe.Add(ref offsetsR, i));
            }
        }
        else if (num > 0)
        {
            var l = first + offsetsL;
            var r = last - offsetsR;
            var tmp = s.Read(l);
            s.Write(l, s.Read(r));
            for (var i = 1; i < num; i++)
            {
                l = first + Unsafe.Add(ref offsetsL, i);
                s.Write(r, s.Read(l));
                r = last - Unsafe.Add(ref offsetsR, i);
                s.Write(l, s.Read(r));
            }
            s.Write(r, tmp);
        }
    }

    /// <summary>
    /// Partitions [begin, end) around pivot *begin. Elements equal to the pivot are put to the left.
    /// Used when many equal elements are detected. Identical to PDQSort.PartitionLeft
    /// (rare path, so no block partitioning is applied here - same as orlp).
    /// </summary>
    private static int PartitionLeft<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int begin, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var pivot = s.Read(begin);
        var first = begin;
        var last = end;

        do { last--; } while (s.IsLessThan(pivot, s.Read(last)));

        if (last + 1 == end)
        {
            do { first++; } while (first < last && s.IsGreaterOrEqual(pivot, s.Read(first)));
        }
        else
        {
            do { first++; } while (s.IsGreaterOrEqual(pivot, s.Read(first)));
        }

        while (first < last)
        {
            s.Swap(first, last);
            do { last--; } while (s.IsLessThan(pivot, s.Read(last)));
            do { first++; } while (s.IsGreaterOrEqual(pivot, s.Read(first)));
        }

        var pivotPos = last;
        s.Write(begin, s.Read(pivotPos));
        s.Write(pivotPos, pivot);

        return pivotPos;
    }

    /// <summary>
    /// Attempts to use insertion sort on [begin, end). Will return false if more than
    /// PartialInsertionSortLimit elements were moved, and abort sorting. Otherwise it will
    /// successfully sort and return true. Identical to PDQSort.PartialInsertionSort.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool PartialInsertionSort<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int begin, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (begin == end) return true;

        var limit = 0;
        for (var cur = begin + 1; cur < end; cur++)
        {
            var sift = cur;
            var siftValue = s.Read(cur);

            // Compare first so we can avoid 2 moves for an element already positioned correctly.
            if (s.IsLessAt(sift, sift - 1))
            {
                do
                {
                    s.Write(sift, s.Read(sift - 1));
                    sift--;
                }
                while (sift != begin && s.IsLessThan(siftValue, s.Read(sift - 1)));

                s.Write(sift, siftValue);
                limit += cur - sift;
            }

            if (limit > PartialInsertionSortLimit)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Sorts 3 elements at positions a, b, c.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Sort3<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int a, int b, int c)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (s.IsLessAt(b, a)) s.Swap(a, b);
        if (s.IsLessAt(c, b)) s.Swap(b, c);
        if (s.IsLessAt(b, a)) s.Swap(a, b);
    }

    /// <summary>
    /// Returns floor(log2(n)), assumes n > 0.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Log2(int n)
    {
        return BitOperations.Log2((uint)n);
    }
}
