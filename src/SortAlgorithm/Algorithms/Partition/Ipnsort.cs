using SortAlgorithm.Contexts;
using System.Buffers;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 実行時パターン検出、branchless Lomuto パーティション、ソーティングネットワーク small sort を組み合わせた
/// 不安定ハイブリッドクイックソートです。Rust 標準ライブラリの不安定ソート (ipnsort) の C# 移植です。
/// <br/>
/// An unstable hybrid quicksort combining run detection, a branchless Lomuto partition, and
/// sorting-network small sorts. C# port of ipnsort (Instruction-Parallel-Network sort) by
/// Lukas Bergdoll, the unstable sort of the Rust standard library (slice::sort_unstable since Rust 1.81).
/// </summary>
/// <remarks>
/// <para><strong>Algorithm:</strong></para>
/// <list type="number">
/// <item><description><strong>Insertion Fast Path:</strong> Inputs of at most 20 elements are sorted with a
/// guarded insertion sort (already-sorted inputs perform no writes).</description></item>
/// <item><description><strong>Run Detection:</strong> A single pre-sorted run covering the whole input is
/// detected in n-1 comparisons; a strictly descending run is reversed in place. Partial runs are not
/// exploited further (users wanting mergesort behavior should use a stable sort such as Driftsort).</description></item>
/// <item><description><strong>Quicksort:</strong> Recursion with pseudo-median pivot selection (glidesort
/// sampling, ~O(n^0.528) sampled elements). A pivot equal to the left-ancestor pivot is the partition
/// minimum; an equal-partition pass (&lt;= pivot to the left) then removes the whole pivot-equal block from
/// recursion, giving O(n log k) behavior for k distinct values.</description></item>
/// <item><description><strong>Partition Kernels:</strong> elements up to 96 bytes use a branchless Lomuto
/// partition paired with a cyclic permutation (2 moves per element, no data-dependent branch on the
/// classification); larger elements use a branchy Hoare partition with cyclic swaps (fewer moves,
/// assuming comparison cost dominates).</description></item>
/// <item><description><strong>Small Sort:</strong> Partitions of at most 32 elements use, by element size:
/// 9/13-element optimal sorting networks extended by guarded insertion and joined with a bidirectional
/// merge (up to 8 bytes); stable 4/8-element conditional-selection networks with bidirectional merges
/// (up to 85 bytes, shared shape with Driftsort's small sort); or plain insertion sort with a 16-element
/// threshold (larger elements, no scratch).</description></item>
/// <item><description><strong>Heapsort Fallback:</strong> After 2·floor(log2(n)) partitions the recursion
/// switches to heapsort, guaranteeing O(n log n) worst case.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Hybrid (Partition (base) + Heap + Insertion + Network)</description></item>
/// <item><description>Stable      : No</description></item>
/// <item><description>In-place    : Yes for large elements; small-sort scratch of at most 48 pooled elements otherwise (the reference uses a stack array, unavailable for generic T in C#)</description></item>
/// <item><description>Best case   : O(n) - Fully ascending or descending input sorts with exactly n-1 comparisons</description></item>
/// <item><description>Average case: O(n log k) where k is the number of distinct values - Equal-partition trick exploits duplicates</description></item>
/// <item><description>Worst case  : O(n log n) - Guaranteed by the bounded partition count plus heapsort fallback</description></item>
/// <item><description>Space       : O(log n) stack; at most 48 pooled scratch elements for the small sort</description></item>
/// </list>
/// <para><strong>Differences from the Reference (Rust) Implementation:</strong></para>
/// <list type="bullet">
/// <item><description><strong>Small-sort scratch:</strong> uses <see cref="System.Buffers.ArrayPool{T}"/> instead of
/// the reference's stack array (<c>stackalloc</c> is not available for generic T in C#).</description></item>
/// <item><description><strong>Copy semantics:</strong> element operations copy values (SortSpan semantics), so the
/// reference's Freeze/Copy type distinctions collapse to element-size checks.</description></item>
/// <item><description><strong>Cyclic permutation:</strong> expressed with SortSpan reads/writes and an explicit final
/// gap iteration instead of raw pointers and drop guards; the move sequence is identical.</description></item>
/// <item><description><strong>NaN handling:</strong> a NaN pre-pass moves NaN values to the front for floating-point
/// element types (same approach as the PDQSort family here), since the optimized primitive comparisons treat
/// NaN as unordered.</description></item>
/// <item><description><strong>Registerized network kernel (beyond the reference):</strong> under NullContext the
/// 9/13-element networks load the whole region into locals and run branchless min/max exchanges in registers
/// (the reference uses cmov exchanges on memory). Micro-benchmarked on Zen 4 (.NET 10, int, per-segment distinct
/// permutations): ~5-7.5x faster than the branchy network on random data, 1.4-1.6x on descending, matching on
/// ascending — a win or tie in every scenario. Observing contexts always use the reference-shaped conditional
/// exchange so compare/swap counts stay accurate.</description></item>
/// </list>
/// <para><strong>References:</strong></para>
/// <para>GitHub: https://github.com/Voultapher/sort-research-rs (ipnsort)</para>
/// <para>Rust stdlib: core::slice::sort::unstable (Rust 1.81+)</para>
/// <para>Writeup: https://github.com/Voultapher/sort-research-rs/blob/main/writeup/ipnsort_introduction/text.md</para>
/// </remarks>
public static class Ipnsort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Small-sort scratch (staging / merge buffer)

    // Inputs of at most this many elements always use the insertion sort fast path.
    private const int MAX_LEN_ALWAYS_INSERTION_SORT = 20;

    // Small-sort thresholds by kind; names match the reference implementation.
    private const int SMALL_SORT_FALLBACK_THRESHOLD = 16;
    private const int SMALL_SORT_GENERAL_THRESHOLD = 32;
    private const int SMALL_SORT_NETWORK_THRESHOLD = 32;

    // The general small sort stages up to SMALL_SORT_GENERAL_THRESHOLD elements in scratch plus
    // 16 elements of sub-scratch for the two Sort8 stages; the network small sort needs at most
    // SMALL_SORT_NETWORK_THRESHOLD elements. 48 covers both.
    private const int MAX_SMALL_SORT_SCRATCH_LEN = SMALL_SORT_GENERAL_THRESHOLD + 16;

    // Element-size budget for a small-sort scratch buffer; mirrors the reference's stack-array
    // limit that decides between the general small sort and the scratch-free insertion fallback.
    private const int MAX_SCRATCH_ARRAY_BYTES = 4096;

    // Elements at most this large use the branchless Lomuto partition; larger elements use the
    // branchy Hoare partition where fewer moves matter more than branch-free classification.
    private const int MAX_BRANCHLESS_PARTITION_SIZE = 96;

    // Recursively select a pseudomedian if the slice is at least this long.
    private const int PSEUDO_MEDIAN_REC_THRESHOLD = 64;

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
    /// Sorts the subrange [first..last) using the default comparer and the provided sort context.
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
    /// This is the full-control version with explicit TComparer and TContext type parameters.
    /// </summary>
    public static void Sort<T, TComparer, TContext>(Span<T> span, int first, int last, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        ArgumentOutOfRangeException.ThrowIfNegative(first);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(last, span.Length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(first, last);

        if (last - first <= 1) return;

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);

        // For floating-point types, move NaN values to the front. The optimized primitive
        // comparisons treat NaN as unordered, which would otherwise break the sort.
        var nanEnd = FloatingPointUtils.MoveNaNsToFront(s, first, last);
        if (last - nanEnd <= 1) return;
        first = nanEnd;

        var n = last - first;

        // Guarded insertion sort handles the common tiny inputs without touching scratch.
        if (n <= MAX_LEN_ALWAYS_INSERTION_SORT)
        {
            context.OnPhase(SortPhase.HybridToInsertionSort, first, last - 1, MAX_LEN_ALWAYS_INSERTION_SORT);
            InsertionSortShiftLeft(s, first, last, 1);
            return;
        }

        IpnsortMain(s, first, last, comparer, context);
    }

    /// <summary>
    /// The ipnsort main path: detects a single run covering the whole input (ascending kept as-is,
    /// strictly descending reversed), otherwise runs the bounded quicksort. The small-sort scratch
    /// (at most 48 elements) is rented once here; element types too large for a scratch buffer use
    /// the scratch-free insertion small sort instead.
    /// </summary>
    private static void IpnsortMain<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, int first, int last, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = last - first;

        context.OnPhase(SortPhase.MergeRunDetect);
        var (runLen, wasReversed) = FindExistingRun(s, first, last);
        if (runLen == n)
        {
            if (wasReversed)
            {
                context.OnPhase(SortPhase.Reverse, first, last - 1);
                Reverse(s, first, last - 1);
            }

            // In-place merging of a long partial run would be possible here, but as in the
            // reference that use-case belongs to the stable sort (Driftsort).
            return;
        }

        // Limit the number of imbalanced partitions to 2 * floor(log2(len)).
        // The binary OR by one eliminates the zero check in the logarithm.
        var limit = 2 * BitOperations.Log2((uint)(n | 1));

        if (SmallSortNeedsScratch<T>())
        {
            var scratchBuffer = ArrayPool<T>.Shared.Rent(MAX_SMALL_SORT_SCRATCH_LEN);
            try
            {
                var t = new SortSpan<T, TComparer, TContext>(scratchBuffer.AsSpan(0, MAX_SMALL_SORT_SCRATCH_LEN), context, comparer, BUFFER_TEMP);
                Quicksort(s, t, first, last, limit, false, default!, comparer, context);
            }
            finally
            {
                ArrayPool<T>.Shared.Return(scratchBuffer, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
            }
        }
        else
        {
            // Insertion-only small sort: no scratch is ever touched.
            var t = new SortSpan<T, TComparer, TContext>(Span<T>.Empty, context, comparer, BUFFER_TEMP);
            Quicksort(s, t, first, last, limit, false, default!, comparer, context);
        }
    }

    /// <summary>
    /// Small-sort kind selection, mirroring the reference's const dispatch. All conditions are
    /// JIT constants per instantiation. Network: element fits in a machine word (cheap in-place
    /// swap heuristic). General: a 48-element scratch stays within the stack-array budget the
    /// reference uses. Fallback: plain insertion sort, no scratch.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool UseNetworkSmallSort<T>() => Unsafe.SizeOf<T>() <= sizeof(ulong);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool SmallSortNeedsScratch<T>() => Unsafe.SizeOf<T>() * MAX_SMALL_SORT_SCRATCH_LEN <= MAX_SCRATCH_ARRAY_BYTES;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int SmallSortThreshold<T>() => SmallSortNeedsScratch<T>() ? SMALL_SORT_NETWORK_THRESHOLD : SMALL_SORT_FALLBACK_THRESHOLD;

    /// <summary>
    /// Finds a run of sorted elements starting at <paramref name="start"/>.
    /// Returns the run length and whether the run is strictly descending.
    /// Only strictly descending runs may be reversed without breaking the comparer contract.
    /// </summary>
    private static (int length, bool descending) FindExistingRun<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, int start, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var len = last - start;
        if (len < 2) return (len, false);

        var i = start + 2;
        var strictlyDescending = s.IsLessAt(start + 1, start);
        if (strictlyDescending)
        {
            while (i < last && s.IsLessAt(i, i - 1))
            {
                i++;
            }
        }
        else
        {
            while (i < last && !s.IsLessAt(i, i - 1))
            {
                i++;
            }
        }
        return (i - start, strictlyDescending);
    }

    /// <summary>
    /// Reverses the subrange [lo..hi] in place.
    /// </summary>
    private static void Reverse<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int lo, int hi)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (lo < hi)
        {
            s.Swap(lo, hi);
            lo++;
            hi--;
        }
    }

    // Quicksort

    /// <summary>
    /// Recursive quicksort over [start..end) with tail-call elimination on the right side.
    /// <para>
    /// A chosen pivot not greater than the left-ancestor pivot must be equal to it (the ancestor is
    /// a lower bound for this slice), which signals a duplicate-heavy slice: an equal-partition pass
    /// (&lt;= pivot to the left) then skips the whole pivot-equal block, giving O(n log k) behavior
    /// for k distinct values — the strategy pdqsort introduced.
    /// </para>
    /// <para><paramref name="limit"/> is the number of partitions allowed before switching to
    /// heapsort for the O(n log n) worst-case guarantee.</para>
    /// </summary>
    private static void Quicksort<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        int start, int end, int limit, bool hasAncestorPivot, T ancestorPivot,
        TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (true)
        {
            var len = end - start;

            if (len <= SmallSortThreshold<T>())
            {
                SmallSort(s, t, start, end, comparer, context);
                return;
            }

            // Too many bad pivot choices: fall back to heapsort to guarantee O(n log n).
            if (limit == 0)
            {
                context.OnPhase(SortPhase.HybridToHeapSort, start, end - 1);
                HeapSortFallback(s, start, end);
                return;
            }
            limit--;

            var pivotPos = ChoosePivot(s, start, end);
            context.OnPhase(SortPhase.QuickSortPartition, start, end - 1, pivotPos);
            context.OnRole(pivotPos, BUFFER_MAIN, RoleType.Pivot);

            // If the chosen pivot is equal to the ancestor pivot it is the smallest element in the
            // slice: partition into (equal to pivot | greater than pivot) and skip the equal block.
            // This case is usually hit when the slice contains many duplicate elements.
            if (hasAncestorPivot && !s.IsLessThan(ancestorPivot, s.Read(pivotPos)))
            {
                var numLtEq = Partition(s, start, end, pivotPos, equalGoesLeft: true);
                context.OnRole(pivotPos, BUFFER_MAIN, RoleType.None);

                // The left side holds only pivot-equal elements and numLtEq includes the pivot
                // slot, so sorting continues after it.
                start += numLtEq + 1;
                hasAncestorPivot = false;
                continue;
            }

            var numLt = Partition(s, start, end, pivotPos, equalGoesLeft: false);
            context.OnRole(pivotPos, BUFFER_MAIN, RoleType.None);

            // The pivot now sits at its final position start + numLt.
            var pivot = s.Read(start + numLt);

            // Recurse into the left side; continue with the right side in this loop. The reference
            // uses a fixed recursion limit as testing showed no benefit in recursing into the
            // shorter side.
            Quicksort(s, t, start, start + numLt, limit, hasAncestorPivot, ancestorPivot, comparer, context);
            start = start + numLt + 1;
            hasAncestorPivot = true;
            ancestorPivot = pivot;
        }
    }

    // Partition

    /// <summary>
    /// Partitions [start..end) around the pivot at <paramref name="pivotPos"/>: elements comparing
    /// less than the pivot (or less-or-equal with <paramref name="equalGoesLeft"/>) end up on the
    /// left, the pivot lands between the sides at start + numLt, and the count of left-side
    /// elements is returned.
    /// <para>Elements up to 96 bytes use the branchless Lomuto kernel (2 moves per element, no
    /// data-dependent branch on the classification result); larger elements use the branchy Hoare
    /// kernel, which moves only out-of-place elements. The size check is a JIT constant.</para>
    /// </summary>
    private static int Partition<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, int start, int end, int pivotPos, bool equalGoesLeft)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var len = end - start;
        if (len == 0) return 0;

        // Place the pivot at the beginning of the slice; partition the rest against a copy.
        s.Swap(start, pivotPos);
        var pivot = s.Read(start);

        var numLt = Unsafe.SizeOf<T>() <= MAX_BRANCHLESS_PARTITION_SIZE
            ? PartitionLomutoBranchlessCyclic(s, start + 1, end, pivot, equalGoesLeft)
            : PartitionHoareBranchyCyclic(s, start + 1, end, pivot, equalGoesLeft);

        // Place the pivot between the two partitions. When numLt > 0 this moves the last
        // left-side element to the front, which is fine for an unstable sort.
        s.Swap(start, start + numLt);

        return numLt;
    }

    /// <summary>
    /// Branchless Lomuto partition over [lo..end) paired with a cyclic permutation, by Lukas
    /// Bergdoll and Orson Peters. Instead of swapping one pair at a time, each element is rotated
    /// through a single temporary gap (2 moves per element); the classification result only feeds
    /// an index increment, so no data-dependent branch is required for placement. The first
    /// element is lifted into the gap temporary up front and processed as the final iteration.
    /// <para>The loop is manually unrolled 2x for elements up to 16 bytes as in the reference
    /// (UNROLL_LEN = 2): RyuJIT does not unroll it by itself, and the unroll measured 4-7% faster
    /// on 1024+ element partitions (Zen 4, .NET 10). The whole loop compiles branch-free — see
    /// the note in <see cref="PartitionLoopBody"/>.</para>
    /// </summary>
    private static int PartitionLomutoBranchlessCyclic<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, int lo, int end, T pivot, bool equalGoesLeft)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (end - lo == 0) return 0;

        // The gap temporary holds a duplicate of s[lo] until the final iteration writes it back.
        var gapValue = s.Read(lo);
        var gapPos = lo;
        var numLt = 0;

        var right = lo + 1;
        if (Unsafe.SizeOf<T>() <= 16)
        {
            for (; right + 2 <= end; right += 2)
            {
                PartitionLoopBody(s, right, pivot, equalGoesLeft, ref gapPos, ref numLt, lo);
                PartitionLoopBody(s, right + 1, pivot, equalGoesLeft, ref gapPos, ref numLt, lo);
            }
        }
        for (; right < end; right++)
        {
            PartitionLoopBody(s, right, pivot, equalGoesLeft, ref gapPos, ref numLt, lo);
        }

        // Final iteration processes the saved gap value as the last logical element.
        {
            var rightIsLt = equalGoesLeft ? s.IsLessOrEqual(gapValue, pivot) : s.IsLessThan(gapValue, pivot);
            var left = lo + numLt;

            s.Write(gapPos, s.Read(left));
            s.Write(left, gapValue);

            numLt += rightIsLt ? 1 : 0;
        }

        return numLt;
    }

    /// <summary>
    /// One cyclic-permutation partition step: rotate left slot -> gap, right element -> left slot,
    /// right becomes the new gap, and advance the less-than count by the classification result.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void PartitionLoopBody<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, int right, T pivot, bool equalGoesLeft,
        ref int gapPos, ref int numLt, int lo)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var value = s.Read(right);
        var rightIsLt = equalGoesLeft ? s.IsLessOrEqual(value, pivot) : s.IsLessThan(value, pivot);
        var left = lo + numLt;

        s.Write(gapPos, s.Read(left));
        s.Write(left, value);
        gapPos = right;

        // RyuJIT (.NET 10) compiles this add-of-condition ternary to setcc + add — no
        // data-dependent branch (verified by disassembly on Zen 4; the store side is
        // unconditional, so the whole loop body is branch-free like the reference's
        // `num_lt += right_is_lt as usize`). Do not "fix" this into bit tricks: the
        // branchless-increment rewrite measured identical code and identical time.
        numLt += rightIsLt ? 1 : 0;
    }

    /// <summary>
    /// Branchy Hoare partition over [lo..end) with cyclic swaps, optimized for large elements
    /// where moves are expensive: only out-of-place pairs are touched, and each pair costs two
    /// copies through a single gap temporary instead of a three-copy swap.
    /// </summary>
    private static int PartitionHoareBranchyCyclic<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, int lo, int end, T pivot, bool equalGoesLeft)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (end - lo == 0) return 0;

        var left = lo;
        var right = end;

        var gapActive = false;
        var gapValue = default(T)!;
        var gapPos = 0;

        while (true)
        {
            // Find the first element on the left that belongs to the right side.
            while (left < right)
            {
                var value = s.Read(left);
                var isLt = equalGoesLeft ? s.IsLessOrEqual(value, pivot) : s.IsLessThan(value, pivot);
                if (!isLt) break;
                left++;
            }

            // Find the last element on the right that belongs to the left side.
            while (true)
            {
                right--;
                if (left >= right) break;
                var value = s.Read(right);
                var isLt = equalGoesLeft ? s.IsLessOrEqual(value, pivot) : s.IsLessThan(value, pivot);
                if (isLt) break;
            }

            if (left >= right) break;

            // Exchange the out-of-order pair via the cyclic gap.
            if (!gapActive)
            {
                gapValue = s.Read(left);
                gapActive = true;
            }
            else
            {
                s.Write(gapPos, s.Read(left));
            }
            gapPos = right;
            s.Write(left, s.Read(right));
            left++;
        }

        // Close the cycle: the first lifted left element fills the last opened right slot.
        if (gapActive)
        {
            s.Write(gapPos, gapValue);
        }

        return left - lo;
    }

    // Pivot Selection

    /// <summary>
    /// Selects a pivot index by sampling an adaptive number of points (median of three sections,
    /// recursively for large slices), approximating the quality of a median of sqrt(n) elements.
    /// Algorithm taken from glidesort by Orson Peters. Requires end - start >= 8.
    /// </summary>
    private static int ChoosePivot<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, int start, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var len = end - start;
        var lenDiv8 = len / 8;

        var a = start;                 // [0, floor(n/8))
        var b = start + lenDiv8 * 4;   // [4*floor(n/8), 5*floor(n/8))
        var c = start + lenDiv8 * 7;   // [7*floor(n/8), 8*floor(n/8))

        return len < PSEUDO_MEDIAN_REC_THRESHOLD
            ? Median3(s, a, b, c)
            : Median3Rec(s, a, b, c, lenDiv8);
    }

    /// <summary>
    /// Calculates an approximate median of three sections of length <paramref name="n"/>,
    /// recursing while sections stay large. Sampling scales as O(n^(log8(3))) ~= O(n^0.528).
    /// </summary>
    private static int Median3Rec<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, int a, int b, int c, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (n * 8 >= PSEUDO_MEDIAN_REC_THRESHOLD)
        {
            var n8 = n / 8;
            a = Median3Rec(s, a, a + n8 * 4, a + n8 * 7, n8);
            b = Median3Rec(s, b, b + n8 * 4, b + n8 * 7, n8);
            c = Median3Rec(s, c, c + n8 * 4, c + n8 * 7, n8);
        }
        return Median3(s, a, b, c);
    }

    /// <summary>
    /// Returns the index holding the median of the elements at indices a, b, c
    /// using at most three comparisons.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Median3<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, int a, int b, int c)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var x = s.IsLessAt(a, b);
        var y = s.IsLessAt(a, c);
        if (x == y)
        {
            // If x=y=false then b, c <= a: return max(b, c).
            // If x=y=true then a < b, c: return min(b, c).
            // Toggling the outcome of b < c with XOR x yields both behaviors.
            var z = s.IsLessAt(b, c);
            return z ^ x ? c : b;
        }
        // Either c <= a < b or b <= a < c, so a is the median.
        return a;
    }

    // Heapsort Fallback

    /// <summary>
    /// Heapsort over [start..end), the O(n log n) fallback of the quicksort recursion.
    /// Build and extract share one loop as in the reference: iterations with i >= len sift the
    /// build nodes len/2-1..0, the remaining iterations swap the maximum out and re-sift the root.
    /// </summary>
    private static void HeapSortFallback<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, int start, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var len = end - start;

        for (var i = len + len / 2 - 1; i >= 0; i--)
        {
            int siftIdx;
            if (i >= len)
            {
                siftIdx = i - len;
            }
            else
            {
                s.Swap(start, start + i);
                siftIdx = 0;
            }

            SiftDown(s, start, Math.Min(i, len), siftIdx);
        }
    }

    /// <summary>
    /// Restores the max-heap invariant (parent >= child) for the heap of size <paramref name="n"/>
    /// rooted at <paramref name="start"/>, sifting down from <paramref name="node"/>.
    /// </summary>
    private static void SiftDown<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, int start, int n, int node)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (true)
        {
            var child = 2 * node + 1;
            if (child >= n) break;

            // Choose the greater child; the bounds branch is highly predictable while the
            // comparison feeds an index increment.
            if (child + 1 < n && s.IsLessAt(start + child, start + child + 1))
            {
                child++;
            }

            // Stop if the invariant holds at node.
            if (!s.IsLessAt(start + node, start + child)) break;

            s.Swap(start + node, start + child);
            node = child;
        }
    }

    // Small Sort

    /// <summary>
    /// Sorts [start..end) of at most SmallSortThreshold elements, dispatching on element size:
    /// sorting networks for word-sized elements, the stable 4/8 selection networks for elements
    /// that fit the scratch budget, and plain insertion sort otherwise. Size checks are JIT
    /// constants per instantiation.
    /// </summary>
    private static void SmallSort<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        int start, int end,
        TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (end - start < 2) return;

        if (UseNetworkSmallSort<T>())
        {
            SmallSortNetwork(s, t, start, end);
        }
        else if (SmallSortNeedsScratch<T>())
        {
            SmallSortGeneral(s, t, start, end);
        }
        else
        {
            InsertionSortShiftLeft(s, start, end, 1);
        }
    }

    /// <summary>
    /// Small sort tuned for word-sized (integer-like) elements. Inputs shorter than 18 elements
    /// sort a single region with a 9- or 13-element optimal network extended by guarded insertion;
    /// longer inputs sort both halves that way and join them with a bidirectional merge staged
    /// through scratch. Requires scratch for end - start elements.
    /// </summary>
    private static void SmallSortNetwork<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        int start, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var len = end - start;
        Debug.Assert(len <= SMALL_SORT_NETWORK_THRESHOLD, "Small sort called above its threshold.");
        Debug.Assert(len <= t.Length, "Scratch must fit the whole small sort.");

        var lenDiv2 = len / 2;
        var noMerge = len < 18;

        var regionStart = start;
        var regionLen = noMerge ? len : lenDiv2;

        while (true)
        {
            int presortedLen;
            if (regionLen >= 13)
            {
                Sort13Optimal(s, regionStart);
                presortedLen = 13;
            }
            else if (regionLen >= 9)
            {
                Sort9Optimal(s, regionStart);
                presortedLen = 9;
            }
            else
            {
                presortedLen = 1;
            }

            InsertionSortShiftLeft(s, regionStart, regionStart + regionLen, presortedLen);

            if (noMerge) return;

            if (regionStart != start) break;

            regionStart = start + lenDiv2;
            regionLen = len - lenDiv2;
        }

        // Both halves are sorted; merge them through scratch and copy back.
        BidirectionalMerge(s, start, len, t, 0);
        t.CopyTo(0, s, start, len);
    }

    /// <summary>
    /// General small sort for element types that fit the scratch budget: both halves are seeded
    /// with a stable 8- or 4-element network (or a single element) staged into scratch, extended
    /// element by element with guarded insertion, and joined back into place with a bidirectional
    /// merge. Uses scratch [0..len) for staging plus [len..len+16) as Sort8 sub-scratch.
    /// Shares its shape with Driftsort's small sort (the reference implementations share it too).
    /// </summary>
    private static void SmallSortGeneral<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        int start, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var len = end - start;
        Debug.Assert(len <= SMALL_SORT_GENERAL_THRESHOLD, "Small sort called above its threshold.");
        Debug.Assert(len + 16 <= t.Length, "General small sort needs len + 16 scratch elements.");

        var lenDiv2 = len / 2;

        int presortedLen;
        if (Unsafe.SizeOf<T>() <= 16 && len >= 16)
        {
            Sort8Stable(s, start, t, 0, len);
            Sort8Stable(s, start + lenDiv2, t, lenDiv2, len + 8);
            presortedLen = 8;
        }
        else if (len >= 8)
        {
            Sort4Stable(s, start, t, 0);
            Sort4Stable(s, start + lenDiv2, t, lenDiv2);
            presortedLen = 4;
        }
        else
        {
            t.Write(0, s.Read(start));
            t.Write(lenDiv2, s.Read(start + lenDiv2));
            presortedLen = 1;
        }

        for (var half = 0; half < 2; half++)
        {
            var offset = half == 0 ? 0 : lenDiv2;
            var desiredLen = half == 0 ? lenDiv2 : len - lenDiv2;
            for (var i = presortedLen; i < desiredLen; i++)
            {
                t.Write(offset + i, s.Read(start + offset + i));
                InsertTail(t, offset, offset + i);
            }
        }

        // Both halves are now sorted in scratch; merge them back into the input.
        BidirectionalMerge(t, 0, len, s, start);
    }

    /// <summary>
    /// Compares the elements at base + a and base + b and swaps them when the latter is smaller.
    /// One comparison per call; the element order of equal elements is preserved (no swap).
    /// This is the observing-context primitive: the conditional swap reports the logical
    /// compare/swap operations accurately. NullContext instead runs the registerized kernels
    /// (<see cref="Sort9Registerized"/> / <see cref="Sort13Registerized"/>).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SwapIfLess<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, int b, int aPos, int bPos)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (s.IsLessAt(b + bPos, b + aPos))
        {
            s.Swap(b + aPos, b + bPos);
        }
    }

    /// <summary>
    /// Optimal 9-element sorting network (25 compare-exchanges), see
    /// https://bertdobbelaere.github.io/sorting_networks.html. Requires 9 elements at <paramref name="b"/>.
    /// <para>Under NullContext the network runs registerized (see <see cref="Sort9Registerized"/>);
    /// observing contexts use the reference-shaped conditional exchanges below so compare/swap
    /// operation counts stay meaningful.</para>
    /// </summary>
    private static void Sort9Optimal<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, int b)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // The typeof check is a JIT constant; the untaken tier is eliminated per instantiation.
        if (typeof(TContext) == typeof(NullContext))
        {
            Sort9Registerized(s, b);
            return;
        }

        SwapIfLess(s, b, 0, 3);
        SwapIfLess(s, b, 1, 7);
        SwapIfLess(s, b, 2, 5);
        SwapIfLess(s, b, 4, 8);
        SwapIfLess(s, b, 0, 7);
        SwapIfLess(s, b, 2, 4);
        SwapIfLess(s, b, 3, 8);
        SwapIfLess(s, b, 5, 6);
        SwapIfLess(s, b, 0, 2);
        SwapIfLess(s, b, 1, 3);
        SwapIfLess(s, b, 4, 5);
        SwapIfLess(s, b, 7, 8);
        SwapIfLess(s, b, 1, 4);
        SwapIfLess(s, b, 3, 6);
        SwapIfLess(s, b, 5, 7);
        SwapIfLess(s, b, 0, 1);
        SwapIfLess(s, b, 2, 4);
        SwapIfLess(s, b, 3, 5);
        SwapIfLess(s, b, 6, 8);
        SwapIfLess(s, b, 2, 3);
        SwapIfLess(s, b, 4, 5);
        SwapIfLess(s, b, 6, 7);
        SwapIfLess(s, b, 1, 2);
        SwapIfLess(s, b, 3, 4);
        SwapIfLess(s, b, 5, 6);
    }

    /// <summary>
    /// Optimal 13-element sorting network (45 compare-exchanges), see
    /// https://bertdobbelaere.github.io/sorting_networks.html. Requires 13 elements at <paramref name="b"/>.
    /// <para>Under NullContext the network runs registerized (see <see cref="Sort13Registerized"/>);
    /// observing contexts use the reference-shaped conditional exchanges below so compare/swap
    /// operation counts stay meaningful.</para>
    /// </summary>
    private static void Sort13Optimal<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, int b)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // The typeof check is a JIT constant; the untaken tier is eliminated per instantiation.
        if (typeof(TContext) == typeof(NullContext))
        {
            Sort13Registerized(s, b);
            return;
        }

        SwapIfLess(s, b, 0, 12);
        SwapIfLess(s, b, 1, 10);
        SwapIfLess(s, b, 2, 9);
        SwapIfLess(s, b, 3, 7);
        SwapIfLess(s, b, 5, 11);
        SwapIfLess(s, b, 6, 8);
        SwapIfLess(s, b, 1, 6);
        SwapIfLess(s, b, 2, 3);
        SwapIfLess(s, b, 4, 11);
        SwapIfLess(s, b, 7, 9);
        SwapIfLess(s, b, 8, 10);
        SwapIfLess(s, b, 0, 4);
        SwapIfLess(s, b, 1, 2);
        SwapIfLess(s, b, 3, 6);
        SwapIfLess(s, b, 7, 8);
        SwapIfLess(s, b, 9, 10);
        SwapIfLess(s, b, 11, 12);
        SwapIfLess(s, b, 4, 6);
        SwapIfLess(s, b, 5, 9);
        SwapIfLess(s, b, 8, 11);
        SwapIfLess(s, b, 10, 12);
        SwapIfLess(s, b, 0, 5);
        SwapIfLess(s, b, 3, 8);
        SwapIfLess(s, b, 4, 7);
        SwapIfLess(s, b, 6, 11);
        SwapIfLess(s, b, 9, 10);
        SwapIfLess(s, b, 0, 1);
        SwapIfLess(s, b, 2, 5);
        SwapIfLess(s, b, 6, 9);
        SwapIfLess(s, b, 7, 8);
        SwapIfLess(s, b, 10, 11);
        SwapIfLess(s, b, 1, 3);
        SwapIfLess(s, b, 2, 4);
        SwapIfLess(s, b, 5, 6);
        SwapIfLess(s, b, 9, 10);
        SwapIfLess(s, b, 1, 2);
        SwapIfLess(s, b, 3, 4);
        SwapIfLess(s, b, 5, 7);
        SwapIfLess(s, b, 6, 8);
        SwapIfLess(s, b, 2, 3);
        SwapIfLess(s, b, 4, 5);
        SwapIfLess(s, b, 6, 7);
        SwapIfLess(s, b, 8, 9);
        SwapIfLess(s, b, 3, 4);
        SwapIfLess(s, b, 5, 6);
    }

    // Registerized network kernels (NullContext only)
    //
    // The branchy conditional exchange costs a ~50%-mispredicted branch per compare-exchange on
    // random data, and even a branchless memory-operand exchange (the reference's cmov-selected
    // swap_if_less) leaves 2 loads + 2 stores per exchange with dependent exchanges chained
    // through store-to-load forwarding. Loading the whole region into locals once, running the
    // network as register-register min/max exchanges, and storing once removes both. Measured on
    // Zen 4 (.NET 10, int, per-segment distinct permutations): ~5-7.5x faster than the branchy
    // network on random data, 1.4-1.6x faster on descending data, and matching it on already
    // sorted data (the branchy best case) - a win or tie in every scenario, so no dispatch on
    // data pattern or size is needed. Gated on NullContext because the register form has no
    // per-element operations to report; observing contexts keep the SwapIfLess network.

    /// <summary>
    /// Registerized twin of <see cref="Sort9Optimal"/>: same optimal 25-exchange listing,
    /// executed on locals with branchless min/max exchanges.
    /// </summary>
    private static void Sort9Registerized<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, int b)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var e0 = s.Read(b);
        var e1 = s.Read(b + 1);
        var e2 = s.Read(b + 2);
        var e3 = s.Read(b + 3);
        var e4 = s.Read(b + 4);
        var e5 = s.Read(b + 5);
        var e6 = s.Read(b + 6);
        var e7 = s.Read(b + 7);
        var e8 = s.Read(b + 8);

        MinMaxLocal(s, ref e0, ref e3); MinMaxLocal(s, ref e1, ref e7); MinMaxLocal(s, ref e2, ref e5); MinMaxLocal(s, ref e4, ref e8);
        MinMaxLocal(s, ref e0, ref e7); MinMaxLocal(s, ref e2, ref e4); MinMaxLocal(s, ref e3, ref e8); MinMaxLocal(s, ref e5, ref e6);
        MinMaxLocal(s, ref e0, ref e2); MinMaxLocal(s, ref e1, ref e3); MinMaxLocal(s, ref e4, ref e5); MinMaxLocal(s, ref e7, ref e8);
        MinMaxLocal(s, ref e1, ref e4); MinMaxLocal(s, ref e3, ref e6); MinMaxLocal(s, ref e5, ref e7);
        MinMaxLocal(s, ref e0, ref e1); MinMaxLocal(s, ref e2, ref e4); MinMaxLocal(s, ref e3, ref e5); MinMaxLocal(s, ref e6, ref e8);
        MinMaxLocal(s, ref e2, ref e3); MinMaxLocal(s, ref e4, ref e5); MinMaxLocal(s, ref e6, ref e7);
        MinMaxLocal(s, ref e1, ref e2); MinMaxLocal(s, ref e3, ref e4); MinMaxLocal(s, ref e5, ref e6);

        s.Write(b, e0);
        s.Write(b + 1, e1);
        s.Write(b + 2, e2);
        s.Write(b + 3, e3);
        s.Write(b + 4, e4);
        s.Write(b + 5, e5);
        s.Write(b + 6, e6);
        s.Write(b + 7, e7);
        s.Write(b + 8, e8);
    }

    /// <summary>
    /// Registerized twin of <see cref="Sort13Optimal"/>: same optimal 45-exchange listing,
    /// executed on locals with branchless min/max exchanges.
    /// </summary>
    private static void Sort13Registerized<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, int b)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var e0 = s.Read(b);
        var e1 = s.Read(b + 1);
        var e2 = s.Read(b + 2);
        var e3 = s.Read(b + 3);
        var e4 = s.Read(b + 4);
        var e5 = s.Read(b + 5);
        var e6 = s.Read(b + 6);
        var e7 = s.Read(b + 7);
        var e8 = s.Read(b + 8);
        var e9 = s.Read(b + 9);
        var e10 = s.Read(b + 10);
        var e11 = s.Read(b + 11);
        var e12 = s.Read(b + 12);

        MinMaxLocal(s, ref e0, ref e12); MinMaxLocal(s, ref e1, ref e10); MinMaxLocal(s, ref e2, ref e9); MinMaxLocal(s, ref e3, ref e7);
        MinMaxLocal(s, ref e5, ref e11); MinMaxLocal(s, ref e6, ref e8);
        MinMaxLocal(s, ref e1, ref e6); MinMaxLocal(s, ref e2, ref e3); MinMaxLocal(s, ref e4, ref e11); MinMaxLocal(s, ref e7, ref e9);
        MinMaxLocal(s, ref e8, ref e10);
        MinMaxLocal(s, ref e0, ref e4); MinMaxLocal(s, ref e1, ref e2); MinMaxLocal(s, ref e3, ref e6); MinMaxLocal(s, ref e7, ref e8);
        MinMaxLocal(s, ref e9, ref e10); MinMaxLocal(s, ref e11, ref e12);
        MinMaxLocal(s, ref e4, ref e6); MinMaxLocal(s, ref e5, ref e9); MinMaxLocal(s, ref e8, ref e11); MinMaxLocal(s, ref e10, ref e12);
        MinMaxLocal(s, ref e0, ref e5); MinMaxLocal(s, ref e3, ref e8); MinMaxLocal(s, ref e4, ref e7); MinMaxLocal(s, ref e6, ref e11);
        MinMaxLocal(s, ref e9, ref e10);
        MinMaxLocal(s, ref e0, ref e1); MinMaxLocal(s, ref e2, ref e5); MinMaxLocal(s, ref e6, ref e9); MinMaxLocal(s, ref e7, ref e8);
        MinMaxLocal(s, ref e10, ref e11);
        MinMaxLocal(s, ref e1, ref e3); MinMaxLocal(s, ref e2, ref e4); MinMaxLocal(s, ref e5, ref e6); MinMaxLocal(s, ref e9, ref e10);
        MinMaxLocal(s, ref e1, ref e2); MinMaxLocal(s, ref e3, ref e4); MinMaxLocal(s, ref e5, ref e7); MinMaxLocal(s, ref e6, ref e8);
        MinMaxLocal(s, ref e2, ref e3); MinMaxLocal(s, ref e4, ref e5); MinMaxLocal(s, ref e6, ref e7); MinMaxLocal(s, ref e8, ref e9);
        MinMaxLocal(s, ref e3, ref e4); MinMaxLocal(s, ref e5, ref e6);

        s.Write(b, e0);
        s.Write(b + 1, e1);
        s.Write(b + 2, e2);
        s.Write(b + 3, e3);
        s.Write(b + 4, e4);
        s.Write(b + 5, e5);
        s.Write(b + 6, e6);
        s.Write(b + 7, e7);
        s.Write(b + 8, e8);
        s.Write(b + 9, e9);
        s.Write(b + 10, e10);
        s.Write(b + 11, e11);
        s.Write(b + 12, e12);
    }

    /// <summary>
    /// Branchless local exchange: leaves min(a, b) in <paramref name="a"/> and max in
    /// <paramref name="b"/>, preserving the order of equal elements (no exchange).
    /// For primitive types with the default comparer this uses Math.Min/Math.Max, which RyuJIT
    /// compiles to one cmp whose flags feed both cmovs — measured 1.4-1.5x faster than two
    /// value-select ternaries, which materialize the bool and re-test it. Other types fall back
    /// to value-select ternaries via the comparer (branch-free where RyuJIT emits cmov, and the
    /// locals still avoid the per-exchange memory round-trips either way).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void MinMaxLocal<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, ref T a, ref T b)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Like the SortSpan primitive specializations: for value type TComparer the 'is' check
        // and every typeof check are JIT constants, so exactly one exchange form survives.
        if (s.Comparer is IComparableComparer)
        {
            if (typeof(T) == typeof(byte)) { MinMax(ref Unsafe.As<T, byte>(ref a), ref Unsafe.As<T, byte>(ref b)); return; }
            if (typeof(T) == typeof(sbyte)) { MinMax(ref Unsafe.As<T, sbyte>(ref a), ref Unsafe.As<T, sbyte>(ref b)); return; }
            if (typeof(T) == typeof(ushort)) { MinMax(ref Unsafe.As<T, ushort>(ref a), ref Unsafe.As<T, ushort>(ref b)); return; }
            if (typeof(T) == typeof(short)) { MinMax(ref Unsafe.As<T, short>(ref a), ref Unsafe.As<T, short>(ref b)); return; }
            if (typeof(T) == typeof(uint)) { MinMax(ref Unsafe.As<T, uint>(ref a), ref Unsafe.As<T, uint>(ref b)); return; }
            if (typeof(T) == typeof(int)) { MinMax(ref Unsafe.As<T, int>(ref a), ref Unsafe.As<T, int>(ref b)); return; }
            if (typeof(T) == typeof(ulong)) { MinMax(ref Unsafe.As<T, ulong>(ref a), ref Unsafe.As<T, ulong>(ref b)); return; }
            if (typeof(T) == typeof(long)) { MinMax(ref Unsafe.As<T, long>(ref a), ref Unsafe.As<T, long>(ref b)); return; }
            // float/double: the NaN pre-pass in Sort guarantees no NaN reaches the network, and
            // Math.Min/Max order -0.0 before +0.0 (consistent with the comparer's total order).
            if (typeof(T) == typeof(float)) { MinMax(ref Unsafe.As<T, float>(ref a), ref Unsafe.As<T, float>(ref b)); return; }
            if (typeof(T) == typeof(double)) { MinMax(ref Unsafe.As<T, double>(ref a), ref Unsafe.As<T, double>(ref b)); return; }
        }

        var lt = s.IsLessThan(b, a);
        var lo = lt ? b : a;
        var hi = lt ? a : b;
        a = lo;
        b = hi;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void MinMax(ref byte a, ref byte b) { var x = a; var y = b; a = Math.Min(x, y); b = Math.Max(x, y); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void MinMax(ref sbyte a, ref sbyte b) { var x = a; var y = b; a = Math.Min(x, y); b = Math.Max(x, y); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void MinMax(ref ushort a, ref ushort b) { var x = a; var y = b; a = Math.Min(x, y); b = Math.Max(x, y); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void MinMax(ref short a, ref short b) { var x = a; var y = b; a = Math.Min(x, y); b = Math.Max(x, y); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void MinMax(ref uint a, ref uint b) { var x = a; var y = b; a = Math.Min(x, y); b = Math.Max(x, y); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void MinMax(ref int a, ref int b) { var x = a; var y = b; a = Math.Min(x, y); b = Math.Max(x, y); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void MinMax(ref ulong a, ref ulong b) { var x = a; var y = b; a = Math.Min(x, y); b = Math.Max(x, y); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void MinMax(ref long a, ref long b) { var x = a; var y = b; a = Math.Min(x, y); b = Math.Max(x, y); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void MinMax(ref float a, ref float b) { var x = a; var y = b; a = Math.Min(x, y); b = Math.Max(x, y); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void MinMax(ref double a, ref double b) { var x = a; var y = b; a = Math.Min(x, y); b = Math.Max(x, y); }

    /// <summary>
    /// Stably sorts the four elements at src[b..b+4) into dst[d..d+4) using five comparisons
    /// and conditional index selection (each element is copied exactly once).
    /// </summary>
    private static void Sort4Stable<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> src, int b,
        SortSpan<T, TComparer, TContext> dst, int d)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Stably create two pairs a0 <= a1 and a2 <= a3.
        var c1 = src.IsLessAt(b + 1, b);
        var c2 = src.IsLessAt(b + 3, b + 2);
        var a0 = b + (c1 ? 1 : 0);
        var a1 = b + (c1 ? 0 : 1);
        var a2 = b + 2 + (c2 ? 1 : 0);
        var a3 = b + 2 + (c2 ? 0 : 1);

        // Compare (a0, a2) and (a1, a3) to identify the min and max. Two unknown elements
        // remain, but stability requires knowing which is leftmost:
        // c3, c4 | min max unknownLeft unknownRight
        //  0,  0 | a0  a3    a1         a2
        //  0,  1 | a0  a1    a2         a3
        //  1,  0 | a2  a3    a0         a1
        //  1,  1 | a2  a1    a0         a3
        var c3 = src.IsLessAt(a2, a0);
        var c4 = src.IsLessAt(a3, a1);
        var min = c3 ? a2 : a0;
        var max = c4 ? a1 : a3;
        var unknownLeft = c3 ? a0 : c4 ? a2 : a1;
        var unknownRight = c4 ? a3 : c3 ? a1 : a2;

        // Sort the two unknown elements.
        var c5 = src.IsLessAt(unknownRight, unknownLeft);
        var lo = c5 ? unknownRight : unknownLeft;
        var hi = c5 ? unknownLeft : unknownRight;

        dst.Write(d, src.Read(min));
        dst.Write(d + 1, src.Read(lo));
        dst.Write(d + 2, src.Read(hi));
        dst.Write(d + 3, src.Read(max));
    }

    /// <summary>
    /// Stably sorts the eight elements at src[b..b+8) into dst[d..d+8): two Sort4 stages into
    /// the sub-scratch at <paramref name="subScratch"/> (8 elements), then a bidirectional merge.
    /// </summary>
    private static void Sort8Stable<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> src, int b,
        SortSpan<T, TComparer, TContext> dst, int d, int subScratch)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        Sort4Stable(src, b, dst, subScratch);
        Sort4Stable(src, b + 4, dst, subScratch + 4);
        BidirectionalMerge(dst, subScratch, 8, dst, d);
    }

    /// <summary>
    /// Merges src[srcStart..srcStart+len/2) and src[srcStart+len/2..srcStart+len) into
    /// dst[dstStart..dstStart+len), consuming the input from both ends at once
    /// (idea from quadsort's parity merge, adapted as in the reference). The source and
    /// destination regions must not overlap and len must be at least 2.
    /// <para>All read indices stay in bounds for any comparison outcome; an inconsistent
    /// comparer yields an unspecified permutation of copies but never faults.</para>
    /// </summary>
    private static void BidirectionalMerge<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> src, int srcStart, int len,
        SortSpan<T, TComparer, TContext> dst, int dstStart)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        Debug.Assert(len >= 2, "BidirectionalMerge requires len >= 2.");
        var lenDiv2 = len / 2;

        var left = srcStart;
        var right = srcStart + lenDiv2;
        var d = dstStart;

        var leftRev = srcStart + lenDiv2 - 1;
        var rightRev = srcStart + len - 1;
        var dRev = dstStart + len - 1;

        for (var iter = 0; iter < lenDiv2; iter++)
        {
            // Forward step: on ties take the left run first.
            var takeLeft = !src.IsLessAt(right, left);
            dst.Write(d, src.Read(takeLeft ? left : right));
            if (takeLeft) left++; else right++;
            d++;

            // Backward step: on ties take the right run first.
            var takeRight = !src.IsLessAt(rightRev, leftRev);
            dst.Write(dRev, src.Read(takeRight ? rightRev : leftRev));
            if (takeRight) rightRev--; else leftRev--;
            dRev--;
        }

        // Odd length: one element is left unconsumed in the input.
        if ((len & 1) != 0)
        {
            var leftNonempty = left <= leftRev;
            dst.Write(d, src.Read(leftNonempty ? left : right));
        }
    }

    // Insertion Sort

    /// <summary>
    /// Guarded insertion sort of [first..last) assuming the first <paramref name="offset"/>
    /// elements form a sorted prefix. Already-sorted input performs no writes.
    /// </summary>
    private static void InsertionSortShiftLeft<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, int first, int last, int offset)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        for (var tail = first + offset; tail < last; tail++)
        {
            InsertTail(s, first, tail);
        }
    }

    /// <summary>
    /// Inserts the element at <paramref name="tail"/> into the sorted range [begin..tail),
    /// shifting greater elements one slot right. Does nothing when it is already in place.
    /// </summary>
    private static void InsertTail<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, int begin, int tail)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (!s.IsLessAt(tail, tail - 1)) return;

        var value = s.Read(tail);
        var j = tail;
        do
        {
            s.Write(j, s.Read(j - 1));
            j--;
        } while (j > begin && s.IsLessThan(value, s.Read(j - 1)));
        s.Write(j, value);
    }
}
