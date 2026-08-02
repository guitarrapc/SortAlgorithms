using SortAlgorithm.Contexts;
using System.Buffers;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// Powersort マージ木と安定パーティションクイックソートを組み合わせたハイブリッド安定ソートです。
/// Rust 標準ライブラリの安定ソート (driftsort) の C# 移植です。
/// <br/>
/// A hybrid stable sort combining a powersort merge tree with a stable-partition quicksort.
/// C# port of driftsort, the stable sort of the Rust standard library (slice::sort since Rust 1.81).
/// </summary>
/// <remarks>
/// <para><strong>Algorithm:</strong></para>
/// <list type="number">
/// <item><description><strong>Insertion Fast Path:</strong> Inputs of at most 20 elements are sorted with a
/// guarded insertion sort (already-sorted inputs perform no writes).</description></item>
/// <item><description><strong>Run Detection:</strong> Scans for ascending or strictly descending pre-sorted runs.
/// A run is accepted when its length reaches ~sqrt(n) (capped at 64); strictly descending runs are reversed in place.
/// Segments without a qualifying run become Unsorted logical runs of the same threshold length.</description></item>
/// <item><description><strong>Powersort Merge Tree:</strong> Fixed-point desired-depth comparison
/// ("Nearly-Optimal Mergesorts", Munro &amp; Wild) decides the merge order with a bounded run stack.</description></item>
/// <item><description><strong>Lazy Logical Merges:</strong> Adjacent Unsorted runs are concatenated while they fit
/// in scratch; a physical merge (quicksorting unsorted sides first) happens only when a sorted run participates
/// or scratch would overflow. Fully random input therefore degenerates into one large stable quicksort.</description></item>
/// <item><description><strong>Stable Quicksort:</strong> Out-of-place stable two-way partition into scratch with
/// pseudo-median pivot selection. A pivot equal to the left-ancestor pivot triggers an equal-partition pass that
/// removes all pivot-equal elements from recursion, giving O(n log k) behavior for k distinct values.
/// After 2·log2(n) imbalanced partitions it falls back to the merge path in eager mode for O(n log n) worst case.</description></item>
/// <item><description><strong>Small Sort:</strong> Sub-slices of at most 32 elements use stable 4/8-element
/// conditional-selection networks extended by guarded insertion, finished with a bidirectional (parity) merge.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Hybrid (Merge + QuickSort + Insertion)</description></item>
/// <item><description>Stable      : Yes (preserves relative order of equal elements)</description></item>
/// <item><description>In-place    : No (auxiliary buffer of about n elements; n/2 for very large inputs)</description></item>
/// <item><description>Best case   : O(n) - Fully ascending or descending input sorts with exactly n-1 comparisons</description></item>
/// <item><description>Average case: O(n log k) where k is the number of distinct values - Equal-partition trick exploits duplicates</description></item>
/// <item><description>Worst case  : O(n log n) - Guaranteed by the powersort merge tree and the bounded quicksort recursion</description></item>
/// <item><description>Space       : O(n) auxiliary (max(ceil(n/2), min(n, 8MB/sizeof(T))) elements, at least 48)</description></item>
/// </list>
/// <para><strong>Differences from the Reference (Rust) Implementation:</strong></para>
/// <list type="bullet">
/// <item><description><strong>Scratch allocation:</strong> uses <see cref="System.Buffers.ArrayPool{T}"/> instead of
/// the reference's 4KiB stack buffer + heap fallback (<c>stackalloc</c> is not available for generic T in C#).</description></item>
/// <item><description><strong>Small-sort scratch:</strong> the insertion temporary lives in a local instead of the
/// scratch buffer, so the minimum scratch is 48 elements instead of the reference's 49.</description></item>
/// <item><description><strong>Eager runs:</strong> the eager path calls the small sort directly; the reference routes
/// through <c>stable_quicksort</c> purely for binary-size reasons (behavior is identical as the length is at most 32).</description></item>
/// <item><description><strong>Interior mutability:</strong> element operations copy values (SortSpan semantics), so the
/// reference's Freeze-type distinction and pivot re-copy are unnecessary.</description></item>
/// <item><description><strong>Partition kernels (beyond the reference):</strong> under NullContext the stable partition
/// dispatches to a branchless two-cursor kernel (elements up to 16 bytes) or an AVX-512 vpcompressd kernel
/// (int with the default comparer). Observing contexts always use the reference-shaped loop so operation
/// counts stay accurate. Micro-benchmarked on Zen 4 (.NET 10): the partition kernel is 4x (branchless)
/// to 14-17x (AVX-512) faster than the reference loop on unpredictable data.</description></item>
/// <item><description><strong>Branch-free bidirectional merge:</strong> under NullContext the small-sort merge
/// matches the reference's branchless merge_up/merge_down via bool-arithmetic cursor advance and mask-select of
/// the taken element (RyuJIT's if-conversion is heuristic in this loop shape, so ternaries and standalone
/// Math.Min/Max compile to branches — verified by disassembly). Measured on Zen 4: ~3x faster on randomly
/// interleaved runs and data-independent in time; 1.1-1.4x slower when one run entirely precedes the other (the
/// reference makes the same trade). The tie rules of the branchy loop are kept exactly (forward consumes the
/// left run on ties, backward the right run), so stability is unaffected. Observing contexts keep the
/// reference-shaped conditional loop.</description></item>
/// </list>
/// <para><strong>References:</strong></para>
/// <para>GitHub: https://github.com/Voultapher/driftsort</para>
/// <para>Rust stdlib: core::slice::sort::stable (Rust 1.81+)</para>
/// <para>Powersort: "Nearly-Optimal Mergesorts" — J. Ian Munro and Sebastian Wild (2018)</para>
/// </remarks>
public static class Driftsort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by DriftsortTests, which derives from StableSortTestsBase.</remarks>
    public static bool IsStable => true;

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Scratch buffer (partition destination / merge buffer / small-sort staging)

    // Inputs of at most this many elements always use the insertion sort fast path.
    private const int MAX_LEN_ALWAYS_INSERTION_SORT = 20;

    // Sub-slices of at most this many elements are sorted with the small sort.
    private const int SMALL_SORT_THRESHOLD = 32;

    // The small sort stages up to SMALL_SORT_THRESHOLD elements in scratch plus
    // 16 elements of sub-scratch for the two Sort8 stages. (The reference needs
    // one more element because its insertion temporary also lives in scratch.)
    private const int MIN_SMALL_SORT_SCRATCH_LEN = SMALL_SORT_THRESHOLD + 16;

    // Scratch scales as max(ceil(n/2), min(n, MAX_FULL_ALLOC_BYTES / sizeof(T))):
    // full n for small inputs, n/2 for large inputs, without a sudden dropoff.
    private const int MAX_FULL_ALLOC_BYTES = 8_000_000; // 8MB

    // Minimum run threshold of the sqrt regime; name matches the reference implementation
    // for cross-referencing. For len > MIN_SQRT_RUN_LEN^2 the run-detection threshold is
    // sqrt(len), which is then always >= this value (hence "MIN"). For smaller inputs the
    // same constant serves as the UPPER cap on the threshold (Math.Min below), so pattern
    // detection of fully or nearly sorted inputs keeps working.
    private const int MIN_SQRT_RUN_LEN = 64;

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

        var n = last - first;
        if (n <= 1) return;

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);

        // Guarded insertion sort handles the common tiny inputs without touching scratch.
        if (n <= MAX_LEN_ALWAYS_INSERTION_SORT)
        {
            InsertionSortShiftLeft(s, first, last);
            return;
        }

        SortMain(s, first, last, comparer, context);
    }

    /// <summary>
    /// Allocates scratch and runs the driftsort main loop.
    /// Scratch is max(ceil(n/2), min(n, 8MB/sizeof(T)), MIN_SMALL_SORT_SCRATCH_LEN) elements:
    /// full n allows the whole input to be quicksorted (better for random and low-cardinality data),
    /// while very large inputs scale down to n/2 and rely on physical merges instead.
    /// </summary>
    private static void SortMain<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, int first, int last, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = last - first;
        var maxFullAlloc = MAX_FULL_ALLOC_BYTES / Unsafe.SizeOf<T>();
        var allocLen = Math.Max(Math.Max(n - n / 2, Math.Min(n, maxFullAlloc)), MIN_SMALL_SORT_SCRATCH_LEN);

        var scratchBuffer = ArrayPool<T>.Shared.Rent(allocLen);
        try
        {
            var t = new SortSpan<T, TComparer, TContext>(scratchBuffer.AsSpan(0, allocLen), context, comparer, BUFFER_TEMP);

            // For small inputs quicksort is not yet beneficial; one or two small-sorts
            // plus a single merge outperform it, so use eager mode.
            var eagerSort = n <= SMALL_SORT_THRESHOLD * 2;
            DriftLoop(s, t, first, last, eagerSort, comparer, context);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(scratchBuffer, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    /// <summary>
    /// The driftsort main loop: creates logical runs left to right and merges them in the
    /// order dictated by the powersort desired-depth heuristic. When <paramref name="eagerSort"/>
    /// is true only small-sorts and physical merges are performed, which makes this loop the
    /// O(n log n) fallback for the quicksort recursion limit.
    /// Requires scratch of at least max(len/2, MIN_SMALL_SORT_SCRATCH_LEN) elements.
    /// </summary>
    private static void DriftLoop<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        int first, int last, bool eagerSort,
        TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var len = last - first;
        if (len < 2) return;

        context.OnPhase(SortPhase.MergeRunDetect);

        var scaleFactor = MergeTreeScaleFactor(len);

        // It's important to have a relatively high entry barrier for pre-sorted runs:
        // a single accepted run forces several merges and shrinks the maximum quicksort
        // size a lot, so sqrt(len) is used as the threshold. For small inputs sqrt would
        // break pattern detection of fully or nearly sorted inputs, hence the cap.
        var minGoodRunLen = len <= MIN_SQRT_RUN_LEN * MIN_SQRT_RUN_LEN
            ? Math.Min(len - len / 2, MIN_SQRT_RUN_LEN)
            : SqrtApprox(len);

        // Run stack for the powersort heuristic. desiredDepths[i] is the desired depth of
        // the merge node between runs[i] and the run after it. Desired depths are strictly
        // ascending on the stack and each is < 64 (LeadingZeroCount of a non-zero u64), so
        // at most 64 distinct values plus the initial dummy run fit in 66 entries.
        Span<int> runLens = stackalloc int[66];
        Span<bool> runSorted = stackalloc bool[66];
        Span<byte> desiredDepths = stackalloc byte[66];
        var stackLen = 0;

        var scanIdx = 0;         // Relative to first.
        var prevRunLen = 0;      // Initial dummy run.
        var prevRunSorted = true;
        while (true)
        {
            // Compute the next run and the desired depth of the merge node between prevRun
            // and nextRun. On the last iteration a dummy run with root-level desired depth
            // fully collapses the merge tree.
            int nextRunLen;
            bool nextRunSorted;
            byte desiredDepth;
            if (scanIdx < len)
            {
                (nextRunLen, nextRunSorted) = CreateRun(s, t, first + scanIdx, last, minGoodRunLen, eagerSort, comparer, context);
                desiredDepth = MergeTreeDepth(scanIdx - prevRunLen, scanIdx, scanIdx + nextRunLen, scaleFactor);
            }
            else
            {
                if (stackLen > 1)
                {
                    context.OnPhase(SortPhase.MergeRunCollapse, stackLen);
                }
                nextRunLen = 0;
                nextRunSorted = true;
                desiredDepth = 0;
            }

            // Merge all earlier runs that desire to be deeper in the merge tree than the
            // merge node between prevRun and nextRun.
            while (stackLen > 1 && desiredDepths[stackLen - 1] >= desiredDepth)
            {
                var leftLen = runLens[stackLen - 1];
                var leftSorted = runSorted[stackLen - 1];
                var mergedLen = leftLen + prevRunLen;
                var mergeStart = first + scanIdx - mergedLen;
                prevRunSorted = LogicalMerge(s, t, mergeStart, mergeStart + leftLen, first + scanIdx, leftSorted, prevRunSorted, comparer, context);
                prevRunLen = mergedLen;
                stackLen--;
            }

            runLens[stackLen] = prevRunLen;
            runSorted[stackLen] = prevRunSorted;
            desiredDepths[stackLen] = desiredDepth;
            stackLen++;

            // Break before overriding the last run with the dummy run.
            if (scanIdx >= len) break;

            scanIdx += nextRunLen;
            prevRunLen = nextRunLen;
            prevRunSorted = nextRunSorted;
        }

        // If the fully collapsed run is still unsorted every logical merge was a
        // concatenation, so it covers the whole input and fits in scratch.
        if (!prevRunSorted)
        {
            StableQuicksortRoot(s, t, first, last, comparer, context);
        }
    }

    // Powersort Merge Tree

    /// <summary>
    /// Computes the scale factor for the powersort merge tree.
    /// Maps [0, n) to [0, 2^62) so desired depths become leading-zero counts.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong MergeTreeScaleFactor(int n)
    {
        return ((1UL << 62) + (ulong)n - 1) / (ulong)n;
    }

    /// <summary>
    /// Computes the desired depth in the merge tree for the split point between
    /// adjacent runs [left..mid) and [mid..right), using the powersort heuristic.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte MergeTreeDepth(int left, int mid, int right, ulong scaleFactor)
    {
        var x = (ulong)left + (ulong)mid;
        var y = (ulong)mid + (ulong)right;
        return (byte)BitOperations.LeadingZeroCount((scaleFactor * x) ^ (scaleFactor * y));
    }

    /// <summary>
    /// Approximates sqrt(n) as 2^(log2(n)/2) refined by one Newton iteration.
    /// </summary>
    private static int SqrtApprox(int n)
    {
        var ilog = BitOperations.Log2((uint)(n | 1));
        var shift = (1 + ilog) / 2;
        return ((1 << shift) + (n >> shift)) / 2;
    }

    // Logical Runs

    /// <summary>
    /// Creates a new logical run starting at <paramref name="start"/>. A pre-existing run that
    /// clears <paramref name="minGoodRunLen"/> is returned as sorted (descending runs are reversed).
    /// Otherwise, eager mode small-sorts a block of at most SMALL_SORT_THRESHOLD elements and
    /// returns it sorted, while lazy mode returns an unsorted run of <paramref name="minGoodRunLen"/>.
    /// </summary>
    private static (int length, bool sorted) CreateRun<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        int start, int last, int minGoodRunLen, bool eagerSort,
        TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var len = last - start;
        if (len >= minGoodRunLen)
        {
            var (runLen, wasReversed) = FindExistingRun(s, start, last);
            if (runLen >= minGoodRunLen)
            {
                if (wasReversed)
                {
                    Reverse(s, start, start + runLen - 1);
                }
                return (runLen, true);
            }
        }

        if (eagerSort)
        {
            // The reference routes through stable_quicksort for binary-size reasons; the
            // length is at most SMALL_SORT_THRESHOLD, so it always small-sorts immediately.
            var eagerRunLen = Math.Min(SMALL_SORT_THRESHOLD, len);
            SortSmall(s, t, start, start + eagerRunLen, comparer, context);
            return (eagerRunLen, true);
        }

        return (Math.Min(minGoodRunLen, len), false);
    }

    /// <summary>
    /// Finds a run of sorted elements starting at <paramref name="start"/>.
    /// Returns the run length and whether the run is strictly descending.
    /// Only strictly descending runs may be reversed without breaking stability.
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

    /// <summary>
    /// Lazy logical merge of adjacent runs [start..mid) and [mid..end), as in glidesort.
    /// Two unsorted runs that still fit in scratch are concatenated (deferring all work);
    /// otherwise unsorted sides are quicksorted and the runs are physically merged.
    /// Returns whether the resulting run is physically sorted.
    /// </summary>
    private static bool LogicalMerge<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        int start, int mid, int end, bool leftSorted, bool rightSorted,
        TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // A combined run larger than scratch could no longer be quicksorted, so it *must*
        // be physically merged here even if both sides are unsorted.
        var canFitInScratch = end - start <= t.Length;
        if (!canFitInScratch || leftSorted || rightSorted)
        {
            if (!leftSorted)
            {
                StableQuicksortRoot(s, t, start, mid, comparer, context);
            }
            if (!rightSorted)
            {
                StableQuicksortRoot(s, t, mid, end, comparer, context);
            }
            MergeRuns(s, t, start, mid, end, comparer, context);
            return true;
        }

        return false;
    }

    // Physical Merge

    /// <summary>
    /// Merges the adjacent sorted runs [start..mid) and [mid..end) using scratch, copying the
    /// shorter run out and merging towards it: forwards when the left run is shorter, backwards
    /// when the right run is shorter. Needs scratch for min(leftLen, rightLen) elements, which is
    /// always at most len/2 and therefore fits the allocation guarantee.
    /// </summary>
    private static void MergeRuns<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        int start, int mid, int end,
        TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var leftLen = mid - start;
        var rightLen = end - mid;
        Debug.Assert(Math.Min(leftLen, rightLen) <= t.Length, "Scratch must fit the shorter run.");
        if (leftLen <= 0 || rightLen <= 0 || t.Length < Math.Min(leftLen, rightLen)) return;

        context.OnPhase(SortPhase.DriftsortPhysicalMerge, start, mid - 1, end - 1);
        context.OnRole(start, BUFFER_MAIN, RoleType.LeftPointer);
        context.OnRole(mid, BUFFER_MAIN, RoleType.RightPointer);

        if (leftLen <= rightLen)
        {
            // Copy the left run to scratch and merge forwards into the gap.
            s.CopyTo(start, t, 0, leftLen);
            var i = 0;
            var j = mid;
            var k = start;
            var leftValue = t.Read(i);
            var rightValue = s.Read(j);
            while (true)
            {
                // On ties consume the left run first to keep the merge stable.
                if (s.IsLessThan(rightValue, leftValue))
                {
                    s.Write(k++, rightValue);
                    j++;
                    if (j == end) break;
                    rightValue = s.Read(j);
                }
                else
                {
                    s.Write(k++, leftValue);
                    i++;
                    if (i == leftLen) break;
                    leftValue = t.Read(i);
                }
            }
            if (i < leftLen)
            {
                t.CopyTo(i, s, k, leftLen - i);
            }
        }
        else
        {
            // Copy the right run to scratch and merge backwards into the gap.
            s.CopyTo(mid, t, 0, rightLen);
            var i = mid - 1;
            var j = rightLen - 1;
            var k = end - 1;
            var leftValue = s.Read(i);
            var rightValue = t.Read(j);
            while (true)
            {
                // On ties consume the right run first (it is placed behind the left run).
                if (s.IsLessThan(rightValue, leftValue))
                {
                    s.Write(k--, leftValue);
                    i--;
                    if (i < start) break;
                    leftValue = s.Read(i);
                }
                else
                {
                    s.Write(k--, rightValue);
                    j--;
                    if (j < 0) break;
                    rightValue = t.Read(j);
                }
            }
            if (j >= 0)
            {
                t.CopyTo(0, s, start, j + 1);
            }
        }

        context.OnRole(start, BUFFER_MAIN, RoleType.None);
        context.OnRole(mid, BUFFER_MAIN, RoleType.None);
    }

    // Stable Quicksort

    /// <summary>
    /// Quicksorts [start..end) with the recursion limit initialized to 2·floor(log2(len)),
    /// which bounds the number of imbalanced partitions before falling back to the
    /// eager merge path.
    /// </summary>
    private static void StableQuicksortRoot<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        int start, int end,
        TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var limit = 2 * BitOperations.Log2((uint)((end - start) | 1));
        StableQuicksort(s, t, start, end, limit, false, default!, comparer, context);
    }

    /// <summary>
    /// Recursive stable quicksort over [start..end). Requires scratch for end - start elements.
    /// <para>
    /// A chosen pivot equal to the left-ancestor pivot means every element in this slice is
    /// &gt;= the ancestor and the slice is duplicate-heavy: an equal-partition pass
    /// (&lt;= pivot to the left, pivot-equal elements final) then skips the whole equal block,
    /// a strategy borrowed from pdqsort that gives O(n log k) for k distinct values.
    /// </para>
    /// </summary>
    private static void StableQuicksort<T, TComparer, TContext>(
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

            if (len <= SMALL_SORT_THRESHOLD)
            {
                SortSmall(s, t, start, end, comparer, context);
                return;
            }

            if (limit == 0)
            {
                // Too many bad pivots: switch to the O(n log n) fallback, which is
                // the drift loop in eager mode (small-sorts and physical merges only).
                DriftLoop(s, t, start, end, eagerSort: true, comparer, context);
                return;
            }
            limit--;

            context.OnPhase(SortPhase.DriftsortQuicksort, start, end - 1);

            var pivotPos = ChoosePivot(s, start, end);
            var pivot = s.Read(pivotPos);
            context.OnRole(pivotPos, BUFFER_MAIN, RoleType.Pivot);

            var performEqualPartition = false;
            if (hasAncestorPivot)
            {
                // ancestorPivot <= all elements here, so !(ancestorPivot < pivot) means pivot
                // is equal to the ancestor: partition out the equal elements instead.
                performEqualPartition = !s.IsLessThan(ancestorPivot, pivot);
            }

            var leftLen = 0;
            if (!performEqualPartition)
            {
                leftLen = StablePartition(s, t, start, end, pivotPos, pivotGoesLeft: false, equalGoesLeft: false, comparer, context);
                // A zero-size left partition means the pivot is the minimum, which also
                // signals a duplicate-heavy slice worth an equal-partition pass.
                performEqualPartition = leftLen == 0;
            }

            if (performEqualPartition)
            {
                var midEq = StablePartition(s, t, start, end, pivotPos, pivotGoesLeft: true, equalGoesLeft: true, comparer, context);
                context.OnRole(pivotPos, BUFFER_MAIN, RoleType.None);
                start += midEq;
                hasAncestorPivot = false;
                continue;
            }
            context.OnRole(pivotPos, BUFFER_MAIN, RoleType.None);

            // Process the left side in the next loop iteration, the right side recursively.
            StableQuicksort(s, t, start + leftLen, end, limit, true, pivot, comparer, context);
            end = start + leftLen;
        }
    }

    /// <summary>
    /// Stably partitions [start..end) around the pivot at <paramref name="pivotPos"/> and returns
    /// the size of the left partition. Elements going left are written to the front of scratch in
    /// scan order; elements going right are written to the back of scratch in reverse scan order,
    /// then both groups are copied back (the right group reversed again), preserving relative order.
    /// The pivot element itself is placed without a self-comparison.
    /// <para>With <paramref name="equalGoesLeft"/> false, elements &lt; pivot go left;
    /// with it true, elements &lt;= pivot go left (used by the equal-partition pass).</para>
    /// <para><strong>Kernel tiers</strong> (micro-benchmarked on Zen 4, .NET 10, per-invocation
    /// distinct permutations so the branch predictor cannot learn the data): the fast kernels are
    /// gated on NullContext because the branchless kernel physically writes each element to two
    /// candidate slots and the SIMD kernel bypasses per-element operations, either of which would
    /// corrupt observed operation counts. Observing contexts always take the reference loop below.</para>
    /// <list type="number">
    /// <item><description>AVX-512 vpcompressd kernel: int + ComparableComparer + NullContext
    /// (baseline ratio 0.06-0.20 on 1024-16384 partitions).</description></item>
    /// <item><description>Branchless two-cursor kernel: NullContext and sizeof(T) &lt;= 16
    /// (ratio 0.24-0.65). RyuJIT compiles the ternary store index of the reference loop to a
    /// data-dependent branch, so the reference loop mispredicts ~50% on random data.</description></item>
    /// <item><description>Reference loop: observing contexts and large elements.</description></item>
    /// </list>
    /// </summary>
    private static int StablePartition<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        int start, int end, int pivotPos, bool pivotGoesLeft, bool equalGoesLeft,
        TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var len = end - start;
        Debug.Assert(len <= t.Length, "Scratch must fit the whole partition.");

        // All typeof/sizeof conditions below are JIT constants; dead tiers are eliminated
        // per instantiation.
        if (typeof(TContext) == typeof(NullContext))
        {
            if (typeof(T) == typeof(int) && comparer is IComparableComparer && Avx512F.IsSupported)
            {
                return StablePartitionAvx512Int(s, t, start, end, pivotPos, pivotGoesLeft, equalGoesLeft);
            }
            if (Unsafe.SizeOf<T>() <= 16)
            {
                return StablePartitionBranchless(s, t, start, end, pivotPos, pivotGoesLeft, equalGoesLeft);
            }
        }

        var pivot = s.Read(pivotPos);
        var numLeft = 0;
        var scratchRev = len; // Decremented before every placement; right side fills scratch from the back.

        // Elements before the pivot, the pivot itself, then the rest — split into two loops
        // (as in the reference) so the pivot check is not paid per element.
        for (var i = start; i < pivotPos; i++)
        {
            var value = s.Read(i);
            var towardsLeft = equalGoesLeft ? s.IsLessOrEqual(value, pivot) : s.IsLessThan(value, pivot);
            scratchRev--;
            t.Write(towardsLeft ? numLeft : scratchRev + numLeft, value);
            if (towardsLeft) numLeft++;
        }

        // The pivot is never compared against itself.
        scratchRev--;
        t.Write(pivotGoesLeft ? numLeft : scratchRev + numLeft, pivot);
        if (pivotGoesLeft) numLeft++;

        for (var i = pivotPos + 1; i < end; i++)
        {
            var value = s.Read(i);
            var towardsLeft = equalGoesLeft ? s.IsLessOrEqual(value, pivot) : s.IsLessThan(value, pivot);
            scratchRev--;
            t.Write(towardsLeft ? numLeft : scratchRev + numLeft, value);
            if (towardsLeft) numLeft++;
        }

        // Copy the left group back in order, then the right group in reverse
        // (restoring its scan order).
        t.CopyTo(0, s, start, numLeft);
        var numRight = len - numLeft;
        for (var i = 0; i < numRight; i++)
        {
            s.Write(start + numLeft + i, t.Read(len - 1 - i));
        }

        return numLeft;
    }

    /// <summary>
    /// Branchless stable partition for NullContext and elements up to 16 bytes.
    /// Two independent cursors write each element unconditionally to BOTH candidate slots
    /// (left slot first, so the right store wins when the cursors converge on one slot;
    /// when they converge both stores carry the same value). Every slot that temporarily
    /// holds a stale copy is overwritten later by its owning element because stale copies
    /// only ever land in the still-open gap between the cursors. Cursor updates use
    /// bool-to-int arithmetic; the 4x unroll amortizes loop control (RyuJIT does not
    /// unroll this loop itself). Same scratch layout as the reference loop.
    /// </summary>
    private static int StablePartitionBranchless<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        int start, int end, int pivotPos, bool pivotGoesLeft, bool equalGoesLeft)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var len = end - start;
        var pivot = s.Read(pivotPos);
        var leftIdx = 0;
        var rightIdx = len - 1;

        var i = start;
        for (; i + 4 <= pivotPos; i += 4)
        {
            PartitionOneBranchless(s, t, i, pivot, equalGoesLeft, ref leftIdx, ref rightIdx);
            PartitionOneBranchless(s, t, i + 1, pivot, equalGoesLeft, ref leftIdx, ref rightIdx);
            PartitionOneBranchless(s, t, i + 2, pivot, equalGoesLeft, ref leftIdx, ref rightIdx);
            PartitionOneBranchless(s, t, i + 3, pivot, equalGoesLeft, ref leftIdx, ref rightIdx);
        }
        for (; i < pivotPos; i++)
        {
            PartitionOneBranchless(s, t, i, pivot, equalGoesLeft, ref leftIdx, ref rightIdx);
        }

        // The pivot is never compared against itself.
        if (pivotGoesLeft)
        {
            t.Write(leftIdx, pivot);
            leftIdx++;
        }
        else
        {
            t.Write(rightIdx, pivot);
            rightIdx--;
        }

        i = pivotPos + 1;
        for (; i + 4 <= end; i += 4)
        {
            PartitionOneBranchless(s, t, i, pivot, equalGoesLeft, ref leftIdx, ref rightIdx);
            PartitionOneBranchless(s, t, i + 1, pivot, equalGoesLeft, ref leftIdx, ref rightIdx);
            PartitionOneBranchless(s, t, i + 2, pivot, equalGoesLeft, ref leftIdx, ref rightIdx);
            PartitionOneBranchless(s, t, i + 3, pivot, equalGoesLeft, ref leftIdx, ref rightIdx);
        }
        for (; i < end; i++)
        {
            PartitionOneBranchless(s, t, i, pivot, equalGoesLeft, ref leftIdx, ref rightIdx);
        }

        var numLeft = leftIdx;
        t.CopyTo(0, s, start, numLeft);
        var numRight = len - numLeft;
        for (var j = 0; j < numRight; j++)
        {
            s.Write(start + numLeft + j, t.Read(len - 1 - j));
        }

        return numLeft;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void PartitionOneBranchless<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        int i, T pivot, bool equalGoesLeft, ref int leftIdx, ref int rightIdx)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var value = s.Read(i);
        var towardsLeft = equalGoesLeft ? s.IsLessOrEqual(value, pivot) : s.IsLessThan(value, pivot);
        var b = (int)Unsafe.As<bool, byte>(ref towardsLeft);
        t.Write(leftIdx, value);
        t.Write(rightIdx, value);
        leftIdx += b;
        rightIdx -= b ^ 1;
    }

    /// <summary>
    /// AVX-512 stable partition for int + ComparableComparer + NullContext. vpcompressd packs
    /// the lanes selected by a comparison mask in lane order (order-preserving, so stability
    /// holds): left lanes are compressed to the bottom and stored at the left cursor; right
    /// lanes are compressed, whole-vector reversed, and stored so their real lanes end at the
    /// right cursor. Both are compress-to-REGISTER plus a plain full-width store — on Zen 4 the
    /// memory form (vpcompressd to memory) is microcoded and 4x slower. Each full-width store
    /// writes its unused lanes as garbage into the open gap between the cursors, so the vector
    /// loop requires gap >= 31 (the two stores' real regions then cannot overlap each other's
    /// garbage); the tail and narrow-gap remainder run the scalar branchless step. The right
    /// group's copy-back also runs vectorized (load 16 / reverse permute / store forward).
    /// </summary>
    private static unsafe int StablePartitionAvx512Int<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        int start, int end, int pivotPos, bool pivotGoesLeft, bool equalGoesLeft)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        Debug.Assert(typeof(T) == typeof(int));
        // Reinterpret the raw spans as int; safe because typeof(T) == typeof(int) is guarded
        // by the caller and this tier only runs under NullContext (no observer to bypass).
        var vRaw = s.RawSpan;
        var tRaw = t.RawSpan;
        var v = MemoryMarshal.CreateSpan(ref Unsafe.As<T, int>(ref MemoryMarshal.GetReference(vRaw)), vRaw.Length);
        var scratch = MemoryMarshal.CreateSpan(ref Unsafe.As<T, int>(ref MemoryMarshal.GetReference(tRaw)), tRaw.Length);

        var len = end - start;
        var pivot = v[pivotPos];
        var leftIdx = 0;
        var rightIdx = len - 1;

        fixed (int* vp = v, tp = scratch)
        {
            var pivotVec = Vector512.Create(pivot);
            var reverseIdx = Vector512.Create(15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0);

            var i = start;
            for (; i + 16 <= pivotPos && rightIdx - leftIdx >= 31; i += 16)
            {
                PartitionVector16(vp, tp, i, pivotVec, reverseIdx, equalGoesLeft, ref leftIdx, ref rightIdx);
            }
            for (; i < pivotPos; i++)
            {
                PartitionOneInt(vp, tp, i, pivot, equalGoesLeft, ref leftIdx, ref rightIdx);
            }

            if (pivotGoesLeft)
            {
                tp[leftIdx] = pivot;
                leftIdx++;
            }
            else
            {
                tp[rightIdx] = pivot;
                rightIdx--;
            }

            i = pivotPos + 1;
            for (; i + 16 <= end && rightIdx - leftIdx >= 31; i += 16)
            {
                PartitionVector16(vp, tp, i, pivotVec, reverseIdx, equalGoesLeft, ref leftIdx, ref rightIdx);
            }
            for (; i < end; i++)
            {
                PartitionOneInt(vp, tp, i, pivot, equalGoesLeft, ref leftIdx, ref rightIdx);
            }

            var numLeft = leftIdx;
            scratch[..numLeft].CopyTo(v[start..]);
            var numRight = len - numLeft;

            // Reverse copy scratch[len-1-j] -> v[start+numLeft+j], 16 lanes at a time.
            var j = 0;
            for (; j + 16 <= numRight; j += 16)
            {
                var block = Vector512.Load(tp + (len - 16 - j));
                var reversed = Avx512F.PermuteVar16x32(block, reverseIdx);
                Vector512.Store(reversed, vp + start + numLeft + j);
            }
            for (; j < numRight; j++)
            {
                vp[start + numLeft + j] = tp[len - 1 - j];
            }

            return numLeft;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe void PartitionVector16(int* vp, int* tp, int i, Vector512<int> pivotVec, Vector512<int> reverseIdx, bool equalGoesLeft, ref int leftIdx, ref int rightIdx)
    {
        var vec = Vector512.Load(vp + i);
        var mask = equalGoesLeft ? Vector512.LessThanOrEqual(vec, pivotVec) : Vector512.LessThan(vec, pivotVec);
        var k = BitOperations.PopCount(mask.ExtractMostSignificantBits());

        // Left: real lanes packed at the bottom land at leftIdx.. in scan order.
        var leftPacked = Avx512F.Compress(Vector512<int>.Zero, mask, vec);
        Vector512.Store(leftPacked, tp + leftIdx);

        // Right: pack right lanes in scan order, reverse all lanes, store so the real lanes
        // end exactly at rightIdx (matching the back-to-front scratch layout).
        var rightPacked = Avx512F.Compress(Vector512<int>.Zero, ~mask, vec);
        var rightReversed = Avx512F.PermuteVar16x32(rightPacked, reverseIdx);
        Vector512.Store(rightReversed, tp + rightIdx - 15);

        leftIdx += k;
        rightIdx -= 16 - k;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe void PartitionOneInt(int* vp, int* tp, int i, int pivot, bool equalGoesLeft, ref int leftIdx, ref int rightIdx)
    {
        var value = vp[i];
        var towardsLeft = equalGoesLeft ? value <= pivot : value < pivot;
        var b = (int)Unsafe.As<bool, byte>(ref towardsLeft);
        tp[leftIdx] = value;
        tp[rightIdx] = value;
        leftIdx += b;
        rightIdx -= b ^ 1;
    }

    // Pivot Selection

    /// <summary>
    /// Selects a pivot index by sampling an adaptive number of points (median of three sections,
    /// recursively for large slices), approximating the quality of a median of sqrt(n) elements.
    /// Algorithm taken from glidesort by Orson Peters.
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

    // Small Sort

    /// <summary>
    /// Sorts [start..end) of at most SMALL_SORT_THRESHOLD elements: both halves are seeded with a
    /// stable 8- or 4-element network (or a single element) staged into scratch, extended element
    /// by element with guarded insertion, and joined back into place with a bidirectional merge.
    /// Uses scratch [0..len) for staging plus [len..len+16) as Sort8 sub-scratch.
    /// </summary>
    private static void SortSmall<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        int start, int end,
        TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var len = end - start;
        if (len < 2) return;
        Debug.Assert(len + 16 <= t.Length, "Small sort needs len + 16 scratch elements.");

        context.OnPhase(SortPhase.DriftsortSmallSort, start, end - 1);

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
    /// <para>Under NullContext the branch-free twin below runs instead; the loop here keeps
    /// the reference-shaped conditional steps so observing contexts report accurate reads.</para>
    /// </summary>
    private static void BidirectionalMerge<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> src, int srcStart, int len,
        SortSpan<T, TComparer, TContext> dst, int dstStart)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        Debug.Assert(len >= 2, "BidirectionalMerge requires len >= 2.");

        // The typeof check is a JIT constant; the untaken tier is eliminated per instantiation.
        if (typeof(TContext) == typeof(NullContext))
        {
            BidirectionalMergeBranchless(src, srcStart, len, dst, dstStart);
            return;
        }

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

    /// <summary>
    /// Branch-free twin of <see cref="BidirectionalMerge"/> matching the reference's
    /// merge_up/merge_down: cursors advance by bool arithmetic and the taken element is picked
    /// with a mask select from the already-loaded pair, so the loop has no data-dependent branch.
    /// The tie rules match the branchy loop exactly (forward consumes the left run on ties,
    /// backward the right run) and the select returns exactly one of the two loaded values,
    /// so stability is preserved.
    /// Measured on Zen 4 (.NET 10, int): ~3x faster than the branchy loop on randomly
    /// interleaved runs (and data-independent in time), 1.1-1.4x slower when one run entirely
    /// precedes the other — the same trade the reference makes; merge inputs here are
    /// small-sort halves of quicksort leaves and eager runs, which are dominated by random
    /// interleavings.
    /// </summary>
    private static void BidirectionalMergeBranchless<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> src, int srcStart, int len,
        SortSpan<T, TComparer, TContext> dst, int dstStart)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
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
            var lv = src.Read(left);
            var rv = src.Read(right);
            var takeLeft = !src.IsLessThan(rv, lv);
            var bl = (int)Unsafe.As<bool, byte>(ref takeLeft);
            dst.Write(d, MergeSelect(src, takeLeft, lv, rv));
            left += bl;
            right += bl ^ 1;
            d++;

            // Backward step: on ties take the right run first.
            var lvRev = src.Read(leftRev);
            var rvRev = src.Read(rightRev);
            var takeRight = !src.IsLessThan(rvRev, lvRev);
            var br = (int)Unsafe.As<bool, byte>(ref takeRight);
            dst.Write(dRev, MergeSelect(src, takeRight, rvRev, lvRev));
            rightRev -= br;
            leftRev -= br ^ 1;
            dRev--;
        }

        // Odd length: one element is left unconsumed in the input.
        if ((len & 1) != 0)
        {
            var leftNonempty = left <= leftRev;
            dst.Write(d, src.Read(leftNonempty ? left : right));
        }
    }

    /// <summary>
    /// Selects <paramref name="a"/> when <paramref name="takeA"/> is true, else <paramref name="b"/>.
    /// For primitive types with the default comparer the selection is an XOR mask on the value's
    /// bit pattern (pure bit select, so float/double reinterpret safely regardless of value):
    /// RyuJIT's if-conversion of ternaries and standalone Math.Min/Max is heuristic and falls
    /// back to a data-dependent branch in this loop shape (verified by disassembly on Zen 4),
    /// while setcc + neg/and/xor is branch-free by construction. Other element types use the
    /// ternary fallback.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T MergeSelect<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, bool takeA, T a, T b)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Like the SortSpan primitive specializations: for value type TComparer the 'is' check
        // and every typeof check are JIT constants, so exactly one select form survives.
        if (s.Comparer is IComparableComparer)
        {
            if (Unsafe.SizeOf<T>() == sizeof(int)
                && (typeof(T) == typeof(int) || typeof(T) == typeof(uint) || typeof(T) == typeof(float)))
            {
                var mask = -(int)Unsafe.As<bool, byte>(ref takeA);
                var av = Unsafe.As<T, int>(ref a);
                var bv = Unsafe.As<T, int>(ref b);
                var sel = bv ^ ((av ^ bv) & mask);
                return Unsafe.As<int, T>(ref sel);
            }
            if (Unsafe.SizeOf<T>() == sizeof(long)
                && (typeof(T) == typeof(long) || typeof(T) == typeof(ulong) || typeof(T) == typeof(double)))
            {
                var mask = -(long)Unsafe.As<bool, byte>(ref takeA);
                var av = Unsafe.As<T, long>(ref a);
                var bv = Unsafe.As<T, long>(ref b);
                var sel = bv ^ ((av ^ bv) & mask);
                return Unsafe.As<long, T>(ref sel);
            }
            if (Unsafe.SizeOf<T>() == sizeof(short)
                && (typeof(T) == typeof(short) || typeof(T) == typeof(ushort) || typeof(T) == typeof(Half)))
            {
                var mask = -(int)Unsafe.As<bool, byte>(ref takeA);
                var sel = (short)(Unsafe.As<T, short>(ref b) ^ ((Unsafe.As<T, short>(ref a) ^ Unsafe.As<T, short>(ref b)) & mask));
                return Unsafe.As<short, T>(ref sel);
            }
            if (typeof(T) == typeof(byte) || typeof(T) == typeof(sbyte))
            {
                var mask = -(int)Unsafe.As<bool, byte>(ref takeA);
                var sel = (byte)(Unsafe.As<T, byte>(ref b) ^ ((Unsafe.As<T, byte>(ref a) ^ Unsafe.As<T, byte>(ref b)) & mask));
                return Unsafe.As<byte, T>(ref sel);
            }
        }

        return takeA ? a : b;
    }

    // Insertion Sort

    /// <summary>
    /// Guarded insertion sort of [first..last) assuming the first element forms a sorted prefix.
    /// Already-sorted input performs n-1 comparisons and no writes.
    /// </summary>
    private static void InsertionSortShiftLeft<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        for (var tail = first + 1; tail < last; tail++)
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
        } while (j > begin && s.IsValueLessThan(value, j - 1));
        s.Write(j, value);
    }
}
