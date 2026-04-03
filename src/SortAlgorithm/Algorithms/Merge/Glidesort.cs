using SortAlgorithm.Contexts;
using System.Buffers;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// Glidesortは、TimSort系マージソートのプリソートデータに対する最良性能と、
/// pattern-defeating quicksortの重複データに対する最良性能を組み合わせた安定ソートアルゴリズムです。
/// 比較ベースのソートで任意の比較演算子をサポートし、パターン付きデータに優れる一方、ランダムデータにも非常に高速です。
/// <br/>
/// Glidesort is a novel stable sorting algorithm that combines the best-case behavior of
/// Timsort-style merge sorts for pre-sorted data with the best-case behavior of
/// pattern-defeating quicksort for data with many duplicates.
/// It is a comparison-based sort supporting arbitrary comparison operators, and while exceptional on data with patterns it is also very fast for random data.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Glidesort:</strong></para>
/// <list type="number">
/// <item><description><strong>Run Detection:</strong> The algorithm scans the input for natural runs (ascending or strictly descending).
/// A run is considered significant if its length ≥ SMALL_SORT and run² ≥ n/2 (i.e., the run is proportionally large).
/// Strictly descending runs are reversed in-place for stability. Short or insignificant sequences are treated as unsorted blocks.</description></item>
/// <item><description><strong>Logical Runs:</strong> The algorithm uses three types of logical runs:
/// Sorted (a physically sorted run), Unsorted (a block of SMALL_SORT elements not yet sorted),
/// and DoubleSorted (two adjacent sorted runs that can be merged later).
/// This deferred merge strategy reduces unnecessary data movement.</description></item>
/// <item><description><strong>Powersort Merge Tree:</strong> Uses the Powersort heuristic to determine optimal merge order.
/// For adjacent runs, the merge depth is computed using fixed-point arithmetic and leading-zero count,
/// ensuring a nearly-optimal merge tree with O(log n) stack depth.</description></item>
/// <item><description><strong>Logical Merge:</strong> Merges are performed logically before physically:
/// two unsorted runs are concatenated if they fit in scratch; unsorted runs are quicksorted before merging;
/// two sorted runs become DoubleSorted; triple and quad merges combine DoubleSorted runs efficiently.</description></item>
/// <item><description><strong>Stable Quicksort:</strong> Unsorted blocks are sorted using a stable bidirectional quicksort
/// that partitions into scratch space, maintaining stability. Uses median-of-3 pivot selection
/// with recursive pseudo-median for large arrays.</description></item>
/// <item><description><strong>Physical Merge:</strong> Uses merge reduction (shrinking already-sorted prefixes/suffixes)
/// and split-merge techniques to reduce data movement. The smaller run is copied to scratch and merged back.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Hybrid (Merge + QuickSort + Insertion)</description></item>
/// <item><description>Stable      : Yes (preserves relative order of equal elements)</description></item>
/// <item><description>In-place    : No (requires O(n) auxiliary space by default, degrades gracefully with less)</description></item>
/// <item><description>Best case   : O(n) - Already sorted or reverse sorted data (single run detected)</description></item>
/// <item><description>Average case: O(n log k) where k is the number of distinct values - Exploits duplicates via quicksort partitioning</description></item>
/// <item><description>Worst case  : O(n log n) - Guaranteed by powersort merge tree and bounded quicksort recursion</description></item>
/// <item><description>Space       : O(n) default auxiliary buffer (scales down for very large arrays)</description></item>
/// </list>
/// <para><strong>Key Innovations:</strong></para>
/// <list type="bullet">
/// <item><description>Logical runs: defers physical sorting of unsorted blocks, reducing work when they get merged with larger sorted runs</description></item>
/// <item><description>Powersort merge tree: provably near-optimal merge order for any run distribution</description></item>
/// <item><description>Triple/quad merges: combines multiple merge operations to reduce data movement</description></item>
/// <item><description>Duplicate handling: stable quicksort partitioning naturally handles many equal elements in O(n log k)</description></item>
/// </list>
/// <para><strong>References:</strong></para>
/// <para>GitHub: https://github.com/orlp/glidesort</para>
/// <para>FOSDEM 2023 Talk: https://fosdem.org/2023/schedule/event/rust_glidesort/</para>
/// <para>Powersort Paper: "Nearly-Optimal Mergesorts" by J. Ian Munro and Sebastian Wild (2018)</para>
/// </remarks>
public static class Glidesort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Temporary merge buffer

    // For this many or fewer elements we use the block insertion sort (branchless small sort).
    private const int SMALL_SORT = 48;

    // Recursively select a pseudomedian if above this threshold.
    private const int PSEUDO_MEDIAN_REC_THRESHOLD = 64;

    // If the total size of a merge operation is above this threshold, glidesort will
    // attempt to split it into (instruction-level) parallel merges when applicable.
    private const int MERGE_SPLIT_THRESHOLD = 32;

    // Scratch buffer scaling thresholds (in bytes), matching the reference implementation.
    // When sorting N elements we allocate a buffer of at most size N, N/2 or N/8.
    private const int FULL_ALLOC_MAX_BYTES = 1024 * 1024;        // 1 MB
    private const int HALF_ALLOC_MAX_BYTES = 1024 * 1024 * 1024;  // 1 GB

    // Partition strategy constants for the stable bidirectional quicksort.
    // Matches the reference PartitionStrategy enum.
    private const byte STRATEGY_RIGHT = 0;           // Select new pivot, partition as (< pivot) vs (>= pivot)
    private const byte STRATEGY_LEFT_WITH_PIVOT = 1;  // Use previous pivot, partition as (<= pivot) vs (> pivot)
    private const byte STRATEGY_LEFT_IF_EQUAL = 2;    // Select new pivot; if equal to previous, partition left; else right

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

        // Fast path for small arrays: use insertion sort directly
        if (n < SMALL_SORT)
        {
            InsertionSort.SortCore(s, first, last);
            return;
        }

        SortCore(s, first, last, comparer, context);
    }

    /// <summary>
    /// Core Glidesort implementation using powersort merge tree with logical runs.
    /// </summary>
    private static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = last - first;

        // Allocate scratch buffer
        var scratchSize = ComputeScratchSize<T>(n);
        var scratchBuffer = ArrayPool<T>.Shared.Rent(scratchSize);

        try
        {
            var scratch = scratchBuffer.AsSpan(0, scratchSize);
            var t = new SortSpan<T, TComparer, TContext>(scratch, context, comparer, BUFFER_TEMP);

            GlidesortCore(s, t, scratch, first, last, eagerSmallsort: false, comparer, context);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(scratchBuffer, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    /// <summary>
    /// Shared powersort merge-tree loop used by both the top-level sort and the quicksort fallback.
    /// <para>
    /// When <paramref name="eagerSmallsort"/> is false (normal path), unsorted blocks are deferred
    /// as Unsorted logical runs and quicksorted only when they need to be merged.
    /// </para>
    /// <para>
    /// When <paramref name="eagerSmallsort"/> is true (quicksort recursion-limit fallback), unsorted
    /// blocks are immediately sorted with BlockInsertionSort and become Sorted runs. This matches
    /// the reference <c>glidesort(eager_smallsort=true)</c>: no quicksort calls are made, so the
    /// fallback is guaranteed O(n log n) via the powersort merge tree alone.
    /// </para>
    /// </summary>
    private static void GlidesortCore<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        Span<T> scratch,
        int first, int last, bool eagerSmallsort,
        TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = last - first;

        context.OnPhase(SortPhase.MergeRunDetect);

        // Powersort merge tree scale factor
        var scaleFactor = MergeTreeScaleFactor(n);

        // Merge stack: stores left children and their desired depths.
        // 64 entries is sufficient because powersort desired depths are strictly ascending
        // on the stack and each depth is < 64 (since it comes from LeadingZeroCount of a u64).
        // This guarantees the stack never exceeds 64 entries for any input size.
        Span<int> stackRunStart = stackalloc int[64];
        Span<int> stackRunEnd = stackalloc int[64];
        Span<byte> stackRunType = stackalloc byte[64]; // 0=Unsorted, 1=Sorted, 2=DoubleSorted
        Span<int> stackRunMid = stackalloc int[64];    // Mid point for DoubleSorted
        Span<byte> stackDesiredDepth = stackalloc byte[64];
        var stackLen = 0;

        // Create the first logical run
        var cursor = first;
        var (prevRunStart, prevRunEnd, prevRunType, prevRunMid) = CreateLogicalRun(s, t, cursor, last, eagerSmallsort);
        cursor = prevRunEnd;

        while (cursor < last)
        {
            var nextRunStartIdx = prevRunStart - first;
            var nextRunEndIdx = prevRunEnd - first;

            // Create next logical run
            var (nextStart, nextEnd, nextType, nextMid) = CreateLogicalRun(s, t, cursor, last, eagerSmallsort);
            cursor = nextEnd;

            var nextNextEndIdx = nextEnd - first;

            var desiredDepth = MergeTreeDepth(nextRunStartIdx, nextRunEndIdx, nextNextEndIdx, scaleFactor);

            // Create the left child and eagerly merge all nodes with deeper desired merge depth
            var leftStart = prevRunStart;
            var leftEnd = prevRunEnd;
            var leftType = prevRunType;
            var leftMid = prevRunMid;

            while (stackLen > 0 && stackDesiredDepth[stackLen - 1] >= desiredDepth)
            {
                // Pop from stack
                stackLen--;
                var ancestorStart = stackRunStart[stackLen];
                var ancestorEnd = stackRunEnd[stackLen];
                var ancestorType = stackRunType[stackLen];
                var ancestorMid = stackRunMid[stackLen];

                // Logical merge: ancestor (left) + leftChild (right)
                (leftStart, leftEnd, leftType, leftMid) = LogicalMerge(
                    s, t, scratch,
                    ancestorStart, ancestorEnd, ancestorType, ancestorMid,
                    leftStart, leftEnd, leftType, leftMid,
                    comparer, context);
            }

            // Push left child onto stack
            Debug.Assert(stackLen < 64, $"Merge stack overflow: stackLen={stackLen}. This should never happen with powersort merge tree.");
            stackRunStart[stackLen] = leftStart;
            stackRunEnd[stackLen] = leftEnd;
            stackRunType[stackLen] = leftType;
            stackRunMid[stackLen] = leftMid;
            stackDesiredDepth[stackLen] = desiredDepth;
            stackLen++;

            prevRunStart = nextStart;
            prevRunEnd = nextEnd;
            prevRunType = nextType;
            prevRunMid = nextMid;
        }

        // Collapse the stack down to a single logical run
        context.OnPhase(SortPhase.MergeRunCollapse, stackLen + 1);
        var resultStart = prevRunStart;
        var resultEnd = prevRunEnd;
        var resultType = prevRunType;
        var resultMid = prevRunMid;

        while (stackLen > 0)
        {
            stackLen--;
            var ancestorStart = stackRunStart[stackLen];
            var ancestorEnd = stackRunEnd[stackLen];
            var ancestorType = stackRunType[stackLen];
            var ancestorMid = stackRunMid[stackLen];

            (resultStart, resultEnd, resultType, resultMid) = LogicalMerge(
                s, t, scratch,
                ancestorStart, ancestorEnd, ancestorType, ancestorMid,
                resultStart, resultEnd, resultType, resultMid,
                comparer, context);
        }

        // Physically sort the final result
        PhysicalSort(s, t, scratch, resultStart, resultEnd, resultType, resultMid, comparer, context);
    }

    // Powersort Merge Tree

    /// <summary>
    /// Computes the scale factor for the powersort merge tree.
    /// Maps [0, n) to [0, 2^62) for efficient depth computation.
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

    // Logical Run Creation

    /// <summary>
    /// Creates a logical run starting at position <paramref name="start"/>.
    /// Returns (runStart, runEnd, runType, runMid).
    /// runType: 0=Unsorted, 1=Sorted, 2=DoubleSorted.
    /// </summary>
    /// <remarks>
    /// Matches the reference: a run is significant if it is at least SMALL_SORT long
    /// AND run_length² ≥ remaining / 2, where remaining = last - start.
    /// The reference uses el.len() (the remaining slice length at each call site), not the
    /// total input length. This means runs near the END of the array are recognised using
    /// the remaining suffix as the baseline — a run of 32 elements in the last 64 is
    /// proportionally significant (32²=1024 ≥ 32), even though it would be missed if
    /// the full n were used as the denominator.
    /// </remarks>
    private static (int start, int end, byte type, int mid) CreateLogicalRun<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        int start, int last, bool eagerSmallsort)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var remaining = last - start;

        // Check for a significant natural run
        if (remaining >= SMALL_SORT)
        {
            var (runLength, descending) = RunLengthAtStart(s, start, last);

            // A run is significant if it's at least SMALL_SORT and run² >= remaining/2.
            // Uses remaining (not total n) to match the reference: near the tail of a large
            // array, shorter runs can still be proportionally large in the remaining data.
            if (runLength >= SMALL_SORT && (long)runLength * runLength >= (long)remaining / 2)
            {
                if (descending)
                {
                    Reverse(s, start, start + runLength - 1);
                }
                return (start, start + runLength, 1, 0); // Sorted
            }
        }

        // Otherwise create a small unsorted run, capped at SMALL_SORT.
        // This cap ensures the run can always be sorted later regardless of scratch size.
        var skip = Math.Min(SMALL_SORT, remaining);
        var runEnd = start + skip;

        // When eagerSmallsort is true (used by the quicksort fallback), sort unsorted
        // blocks immediately so they become Sorted runs. This matches the reference
        // glidesort(eager_smallsort=true) behavior: no Unsorted runs are created, so the
        // powersort merge tree only performs merges (guaranteed O(n log n)).
        if (eagerSmallsort)
        {
            BlockInsertionSort(s, t, start, runEnd);
            return (start, runEnd, 1, 0); // Sorted
        }

        return (start, runEnd, 0, 0); // Unsorted
    }

    /// <summary>
    /// Returns the length of the sorted run at the start and whether it is strictly descending.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (int length, bool descending) RunLengthAtStart<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, int start, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (last - start < 2) return (last - start, false);

        var descending = s.Compare(start + 1, start) < 0;
        if (descending)
        {
            var i = start + 2;
            while (i < last && s.Compare(i, i - 1) < 0)
            {
                i++;
            }
            return (i - start, true);
        }
        else
        {
            var i = start + 2;
            while (i < last && s.Compare(i, i - 1) >= 0)
            {
                i++;
            }
            return (i - start, false);
        }
    }

    // Logical Merge

    /// <summary>
    /// Performs a logical merge of left and right runs.
    /// This defers physical sorting when possible, combining runs into DoubleSorted.
    /// </summary>
    private static (int start, int end, byte type, int mid) LogicalMerge<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        Span<T> scratch,
        int leftStart, int leftEnd, byte leftType, int leftMid,
        int rightStart, int rightEnd, byte rightType, int rightMid,
        TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var leftLen = leftEnd - leftStart;
        var rightLen = rightEnd - rightStart;

        // Unsorted + Unsorted: concatenate if the combined block fits in scratch.
        // This matches the reference (l.len() + r.len() <= scratch.len()).
        // Unsorted blocks start at SMALL_SORT and can grow through concatenation,
        // but the powersort merge tree limits how many adjacent unsorted blocks exist.
        // When scratch is smaller than n (for very large arrays), this condition
        // naturally prevents unbounded growth.
        if (leftType == 0 && rightType == 0 && leftLen + rightLen <= scratch.Length)
        {
            return (leftStart, rightEnd, 0, 0);
        }

        // Unsorted on left: quicksort it first
        if (leftType == 0)
        {
            StableQuicksort(s, t, scratch, leftStart, leftEnd, comparer, context);
            leftType = 1;
        }

        // Unsorted on right: quicksort it first
        if (rightType == 0)
        {
            StableQuicksort(s, t, scratch, rightStart, rightEnd, comparer, context);
            rightType = 1;
        }

        // Sorted + Sorted: defer as DoubleSorted
        if (leftType == 1 && rightType == 1)
        {
            return (leftStart, rightEnd, 2, leftEnd);
        }

        // DoubleSorted(left) + Sorted(right): triple merge
        if (leftType == 2 && rightType == 1)
        {
            PhysicalTripleMerge(s, t, scratch, leftStart, leftMid, leftEnd, rightEnd, comparer, context);
            return (leftStart, rightEnd, 1, 0);
        }

        // Sorted(left) + DoubleSorted(right): triple merge
        if (leftType == 1 && rightType == 2)
        {
            PhysicalTripleMerge(s, t, scratch, leftStart, leftEnd, rightMid, rightEnd, comparer, context);
            return (leftStart, rightEnd, 1, 0);
        }

        // DoubleSorted + DoubleSorted: quad merge
        if (leftType == 2 && rightType == 2)
        {
            PhysicalQuadMerge(s, t, scratch, leftStart, leftMid, leftEnd, rightMid, rightEnd, comparer, context);
            return (leftStart, rightEnd, 1, 0);
        }

        // Fallback: physically sort both sides and merge
        PhysicalSort(s, t, scratch, leftStart, leftEnd, leftType, leftMid, comparer, context);
        PhysicalSort(s, t, scratch, rightStart, rightEnd, rightType, rightMid, comparer, context);
        PhysicalMerge(s, t, scratch, leftStart, leftEnd, rightEnd, comparer, context);
        return (leftStart, rightEnd, 1, 0);
    }

    // Physical Sort

    /// <summary>
    /// Ensures a logical run is physically sorted.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void PhysicalSort<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        Span<T> scratch,
        int start, int end, byte type, int mid,
        TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        switch (type)
        {
            case 1: // Sorted - already done
                break;
            case 0: // Unsorted - quicksort it
                StableQuicksort(s, t, scratch, start, end, comparer, context);
                break;
            case 2: // DoubleSorted - merge the two halves
                PhysicalMerge(s, t, scratch, start, mid, end, comparer, context);
                break;
        }
    }

    // Physical Merge Operations

    /// <summary>
    /// Merges two adjacent sorted runs [start..mid) and [mid..end) using scratch space.
    /// Uses an iterative shrink-split loop matching the reference glidesort <c>physical_merge</c>:
    /// <para>1. <b>Shrink</b>: skip already-in-place prefix of left and suffix of right.</para>
    /// <para>2. <b>Split</b>: find split points (lsplit, rsplit) via <see cref="MergeSplitPoints{T,TComparer,TContext}"/>
    ///    such that <c>left1.len == right0.len</c>, partitioning each side into two sub-runs.</para>
    /// <para>3. <b>Gap trick</b>: copy left1 to scratch, then execute two independent merges that
    ///    write into the vacated gap regions — one backward (left0 + right0 → left's space)
    ///    and one forward (left1-from-scratch + right1 → right's space).</para>
    /// <para>4. When scratch is too small for the gap trick, swap left1 ↔ right0, recursively
    ///    merge the right half, and loop for the left half.</para>
    /// </summary>
    private static void PhysicalMerge<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        Span<T> scratch,
        int start, int mid, int end,
        TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (mid <= start || mid >= end) return;

        var curLeft = start;
        var curMid = mid;
        var curRightEnd = end;

        while (ShrinkStableMerge(s, ref curLeft, curMid, ref curRightEnd))
        {
            // Find split points for two independent sub-merges.
            // Guarantees: left1.len == right0.len (= rsplit).
            var (lsplit, rsplit) = MergeSplitPoints(s, curLeft, curMid, curMid, curRightEnd);
            var left1Len = curMid - curLeft - lsplit; // == rsplit
            var left1Start = curLeft + lsplit;

            if (left1Len <= scratch.Length)
            {
                // Gap trick: copy left1 to scratch, then two in-place merges.
                s.CopyTo(left1Start, t, 0, left1Len);

                // Merge left0 + right0 backward into [curLeft..curMid).
                // The gap (left1's former space) is to the right of left0.
                MergeRightGap(s, curLeft, lsplit, curMid, rsplit);

                // Merge left1 (from scratch) + right1 forward into [curMid..curRightEnd).
                // The gap (right0's former space) is to the left of right1.
                var right0End = curMid + rsplit;
                MergeLeftGap(s, t, left1Len, right0End, curRightEnd - right0End, curMid);

                return;
            }
            else
            {
                // Scratch too small: swap left1 ↔ right0 (equal length),
                // recursively merge right half, then loop for left half.
                for (var i = 0; i < left1Len; i++)
                    s.Swap(left1Start + i, curMid + i);

                // Right half: [curMid..curRightEnd) now contains [left1_old | right1].
                PhysicalMerge(s, t, scratch, curMid, curMid + left1Len, curRightEnd, comparer, context);

                // Left half: [curLeft..curMid) now contains [left0 | right0_old].
                curRightEnd = curMid;
                curMid = left1Start;
            }
        }
    }

    /// <summary>
    /// Shrinks a stable merge of [left..mid) and [mid..rightEnd) by skipping
    /// already-in-place prefix of left and suffix of right. Updates <paramref name="left"/>
    /// and <paramref name="rightEnd"/> to the reduced boundaries. Returns <c>false</c> if
    /// the merge is unnecessary (already sorted or empty).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool ShrinkStableMerge<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        ref int left, int mid, ref int rightEnd)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (left >= mid || mid >= rightEnd) return false;

        // Already completely sorted?
        if (s.Compare(mid - 1, mid) <= 0) return false;

        // Skip left prefix already in place: find first left element > right's first.
        var newLeft = left;
        while (newLeft < mid && s.Compare(newLeft, mid) <= 0)
            newLeft++;

        // Skip right suffix already in place: find last right element < left's last.
        var newRightEnd = rightEnd;
        while (newRightEnd > mid && s.Compare(mid - 1, newRightEnd - 1) <= 0)
            newRightEnd--;

        if (newLeft >= mid || newRightEnd <= mid) return false;

        left = newLeft;
        rightEnd = newRightEnd;
        return true;
    }

    /// <summary>
    /// Given sorted left and right of equal size <paramref name="n"/>, finds the smallest
    /// <c>i</c> such that for all <c>l</c> in <c>left[i..]</c> and <c>r</c> in
    /// <c>right[..n-i]</c> we have <c>l &gt; r</c>.
    /// Uses binary search (O(log n) comparisons).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CrossoverPoint<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        int leftStart, int rightStart, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var lo = 0;
        var maybe = n;
        while (maybe > 0)
        {
            var step = maybe / 2;
            var i = lo + step;
            // Is right[n-1-i] < left[i]? If so, i is a valid crossover point.
            if (s.Compare(rightStart + n - 1 - i, leftStart + i) < 0)
            {
                maybe = step;
            }
            else
            {
                lo += step + 1;
                maybe -= step + 1;
            }
        }
        return lo;
    }

    /// <summary>
    /// Computes split points <c>(lsplit, rsplit)</c> for two sorted runs
    /// <c>left=[leftStart..leftEnd)</c> and <c>right=[rightStart..rightEnd)</c> such that
    /// merging <c>(left[..lsplit], right[..rsplit])</c> followed by
    /// <c>(left[lsplit..], right[rsplit..])</c> equals merging the full runs.
    /// Guarantees <c>left[lsplit..].len == right[..rsplit].len</c>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (int lsplit, int rsplit) MergeSplitPoints<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        int leftStart, int leftEnd, int rightStart, int rightEnd)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var leftLen = leftEnd - leftStart;
        var rightLen = rightEnd - rightStart;
        var minLen = Math.Min(leftLen, rightLen);
        var leftSkip = leftLen - minLen;
        var i = CrossoverPoint(s, leftStart + leftSkip, rightStart, minLen);
        return (leftSkip + i, minLen - i);
    }

    /// <summary>
    /// Merges left0 = <c>s[l0..l0+l0Len)</c> and right0 = <c>s[r0..r0+r0Len)</c>
    /// <b>backward</b> into <c>s[l0..l0+l0Len+r0Len)</c>.
    /// The gap between <c>l0+l0Len</c> and <c>r0</c> must equal <c>r0Len</c>.
    /// Used after the gap trick: left1 has been moved to scratch, freeing space
    /// adjacent to left0 for in-place backward merge output.
    /// </summary>
    private static void MergeRightGap<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        int l0, int l0Len, int r0, int r0Len)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var c1 = l0 + l0Len - 1;
        var c2 = r0 + r0Len - 1;
        var o = l0 + l0Len + r0Len - 1;

        while (c1 >= l0 && c2 >= r0)
        {
            var val1 = s.Read(c1);
            var val2 = s.Read(c2);

            if (s.Compare(val1, val2) <= 0) // <= takes val2 for stability
            {
                s.Write(o--, val2);
                c2--;
            }
            else
            {
                s.Write(o--, val1);
                c1--;
            }
        }

        // If right0 has remaining elements, copy them to the front of the output.
        // (Remaining left0 elements are already in their correct positions.)
        while (c2 >= r0)
        {
            s.Write(o--, s.Read(c2--));
        }
    }

    /// <summary>
    /// Merges left1 = <c>t[0..l1Len)</c> (from scratch) and right1 = <c>s[r1..r1+r1Len)</c>
    /// <b>forward</b> into <c>s[outStart..outStart+l1Len+r1Len)</c>.
    /// The gap between <c>outStart</c> and <c>r1</c> must equal <c>l1Len</c>.
    /// Used after the gap trick: right0's former space provides the gap for
    /// in-place forward merge output.
    /// </summary>
    private static void MergeLeftGap<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        int l1Len, int r1, int r1Len, int outStart)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var c1 = 0;
        var e1 = l1Len;
        var c2 = r1;
        var e2 = r1 + r1Len;
        var o = outStart;

        while (c1 < e1 && c2 < e2)
        {
            var val1 = t.Read(c1);
            var val2 = s.Read(c2);

            if (s.Compare(val1, val2) <= 0) // <= for stability
            {
                s.Write(o++, val1);
                c1++;
            }
            else
            {
                s.Write(o++, val2);
                c2++;
            }
        }

        // If left1 has remaining elements in scratch, copy them to output.
        // (Remaining right1 elements are already in their correct positions.)
        if (c1 < e1)
        {
            t.CopyTo(c1, s, o, e1 - c1);
        }
    }

    /// <summary>
    /// Forward-merges two adjacent sorted runs <c>s[leftStart..mid)</c> and <c>s[mid..rightEnd)</c>
    /// into scratch at <c>t[tOffset..tOffset+totalLen)</c>.
    /// Caller must ensure <c>tOffset + totalLen ≤ t.Length</c>.
    /// </summary>
    private static void MergeIntoScratchAt<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        int leftStart, int mid, int rightEnd, int tOffset)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var c1 = leftStart;
        var e1 = mid;
        var c2 = mid;
        var e2 = rightEnd;
        var o = tOffset;

        while (c1 < e1 && c2 < e2)
        {
            var v1 = s.Read(c1);
            var v2 = s.Read(c2);

            if (s.Compare(v1, v2) <= 0) // <= for stability
            {
                t.Write(o++, v1);
                c1++;
            }
            else
            {
                t.Write(o++, v2);
                c2++;
            }
        }

        while (c1 < e1) t.Write(o++, s.Read(c1++));
        while (c2 < e2) t.Write(o++, s.Read(c2++));
    }

    /// <summary>
    /// Backward-merges <c>s[leftStart..leftStart+leftLen)</c> with <c>t[0..tLen)</c>
    /// into <c>s[leftStart..outEnd)</c>. The gap between the left run and <c>outEnd</c>
    /// provides the output space.
    /// Used for triple/quad merge when the right pair was merged into scratch.
    /// </summary>
    private static void MergeRightGapFromScratch<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        int leftStart, int leftLen, int tLen, int outEnd)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var c1 = leftStart + leftLen - 1;
        var c2 = tLen - 1;
        var o = outEnd - 1;

        while (c1 >= leftStart && c2 >= 0)
        {
            var v1 = s.Read(c1);
            var v2 = t.Read(c2);

            if (s.Compare(v1, v2) <= 0) // <= takes v2 for stability
            {
                s.Write(o--, v2);
                c2--;
            }
            else
            {
                s.Write(o--, v1);
                c1--;
            }
        }

        while (c2 >= 0)
        {
            s.Write(o--, t.Read(c2--));
        }
    }

    /// <summary>
    /// Forward-merges two sorted halves <c>t[0..leftLen)</c> and <c>t[leftLen..totalLen)</c>
    /// from scratch back into <c>s[outStart..outStart+totalLen)</c>.
    /// Used for quad merge full-scratch path.
    /// </summary>
    private static void MergeFromScratch<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        int leftLen, int totalLen, int outStart)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var c1 = 0;
        var e1 = leftLen;
        var c2 = leftLen;
        var e2 = totalLen;
        var o = outStart;

        while (c1 < e1 && c2 < e2)
        {
            var v1 = t.Read(c1);
            var v2 = t.Read(c2);

            if (s.Compare(v1, v2) <= 0) // <= for stability
            {
                s.Write(o++, v1);
                c1++;
            }
            else
            {
                s.Write(o++, v2);
                c2++;
            }
        }

        while (c1 < e1) s.Write(o++, t.Read(c1++));
        while (c2 < e2) s.Write(o++, t.Read(c2++));
    }

    /// <summary>
    /// Merges three adjacent sorted runs: [start..mid1), [mid1..mid2), [mid2..end).
    /// Uses the gap trick when possible: merge the smaller pair into scratch, then
    /// merge the result with the remaining run using the vacated gap as output space.
    /// This saves one full copy compared to two sequential PhysicalMerge calls.
    /// </summary>
    private static void PhysicalTripleMerge<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        Span<T> scratch,
        int start, int mid1, int mid2, int end,
        TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var len0 = mid1 - start;
        var len1 = mid2 - mid1;
        var len2 = end - mid2;

        if (len0 < len2)
        {
            // Smaller pair is r0+r1. Try to merge into scratch.
            if (len0 + len1 <= scratch.Length)
            {
                // Gap trick: merge r0+r1 → t[0..len0+len1), gap is s[start..mid2).
                MergeIntoScratchAt(s, t, start, mid1, mid2, 0);
                // Forward-merge t[0..len0+len1) with s[mid2..end) into s[start..end).
                MergeLeftGap(s, t, len0 + len1, mid2, len2, start);
            }
            else
            {
                PhysicalMerge(s, t, scratch, start, mid1, mid2, comparer, context);
                PhysicalMerge(s, t, scratch, start, mid2, end, comparer, context);
            }
        }
        else
        {
            // Smaller pair is r1+r2. Try to merge into scratch.
            if (len1 + len2 <= scratch.Length)
            {
                // Gap trick: merge r1+r2 → t[0..len1+len2), gap is s[mid1..end).
                MergeIntoScratchAt(s, t, mid1, mid2, end, 0);
                // Backward-merge s[start..mid1) with t[0..len1+len2) into s[start..end).
                MergeRightGapFromScratch(s, t, start, len0, len1 + len2, end);
            }
            else
            {
                PhysicalMerge(s, t, scratch, mid1, mid2, end, comparer, context);
                PhysicalMerge(s, t, scratch, start, mid1, end, comparer, context);
            }
        }
    }

    /// <summary>
    /// Merges four adjacent sorted runs: [start..mid1), [mid1..mid2), [mid2..mid3), [mid3..end).
    /// Uses the gap trick when possible:
    /// <para><b>Full scratch</b>: merge both pairs into scratch, then merge the two halves back.
    /// Every element moves exactly twice (3 merge passes instead of ~6).</para>
    /// <para><b>Partial scratch</b>: merge one pair in-place first (freeing t), then merge
    /// the other into scratch, and finish with a gap merge (4 merge passes).</para>
    /// <para><b>Fallback</b>: three sequential PhysicalMerge calls (6 merge passes).</para>
    /// </summary>
    private static void PhysicalQuadMerge<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        Span<T> scratch,
        int start, int mid1, int mid2, int mid3, int end,
        TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var leftLen = mid2 - start;
        var rightLen = end - mid2;
        var totalLen = leftLen + rightLen;

        // Full scratch: merge both pairs into scratch, then merge back.
        if (totalLen <= scratch.Length)
        {
            MergeIntoScratchAt(s, t, start, mid1, mid2, 0);
            MergeIntoScratchAt(s, t, mid2, mid3, end, leftLen);
            MergeFromScratch(s, t, leftLen, totalLen, start);
            return;
        }

        // Partial: merge one pair in-place first (freeing t), then merge
        // the other into scratch, and finish with a gap merge.
        if (leftLen <= scratch.Length)
        {
            // Merge right pair in-place first (uses t temporarily, then releases it).
            PhysicalMerge(s, t, scratch, mid2, mid3, end, comparer, context);
            // Merge left pair into scratch.
            MergeIntoScratchAt(s, t, start, mid1, mid2, 0);
            // Final: forward-merge t[0..leftLen) + s[mid2..end) → s[start..end).
            MergeLeftGap(s, t, leftLen, mid2, rightLen, start);
            return;
        }

        if (rightLen <= scratch.Length)
        {
            // Merge left pair in-place first (uses t temporarily, then releases it).
            PhysicalMerge(s, t, scratch, start, mid1, mid2, comparer, context);
            // Merge right pair into scratch.
            MergeIntoScratchAt(s, t, mid2, mid3, end, 0);
            // Final: backward-merge s[start..mid2) + t[0..rightLen) → s[start..end).
            MergeRightGapFromScratch(s, t, start, leftLen, rightLen, end);
            return;
        }

        // Fallback: sequential merges.
        PhysicalMerge(s, t, scratch, start, mid1, mid2, comparer, context);
        PhysicalMerge(s, t, scratch, mid2, mid3, end, comparer, context);
        PhysicalMerge(s, t, scratch, start, mid2, end, comparer, context);
    }

    // Stable Quicksort

    /// <summary>
    /// Sorts the range [start..end) using a stable bidirectional 2-way quicksort with scratch space.
    /// Uses PartitionStrategy to handle duplicates: when all elements are ≥ pivot, re-partitions with
    /// reversed comparison to separate equal elements. This achieves O(n log k) for k distinct values.
    /// Falls back to a guaranteed O(n log n) stable merge sort when the recursion budget is exhausted.
    /// </summary>
    private static void StableQuicksort<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        Span<T> scratch,
        int start, int end,
        TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = end - start;
        if (n < 2) return;

        if (n < SMALL_SORT)
        {
            BlockInsertionSort(s, t, start, end);
            return;
        }

        var logn = 64 - BitOperations.LeadingZeroCount((ulong)n);
        var recursionLimit = 2 * logn;

        // Split input into left (forward scan) and right (backward scan) halves.
        // Both initially reside in s (the main array). The dest/scratch swap
        // recursion will avoid copying partition results back at each level;
        // copies are deferred to base cases only.
        var half = n / 2;
        StableQuicksortInto(s, t, scratch,
            true, start, half,
            true, start + half, n - half,
            start, 0,
            recursionLimit, STRATEGY_RIGHT, default!,
            comparer, context);
    }

    /// <summary>
    /// Reads a value at logical index <paramref name="idx"/> from split input
    /// (left + right concatenated). Left is in <c>s</c> when <paramref name="leftInMain"/>
    /// is true, else in <c>t</c>; same for right.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T ReadSplitInput<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        bool leftInMain, int leftOff, int leftLen,
        bool rightInMain, int rightOff,
        int idx)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (idx < leftLen)
        {
            return leftInMain ? s.Read(leftOff + idx) : t.Read(leftOff + idx);
        }
        else
        {
            return rightInMain ? s.Read(rightOff + idx - leftLen) : t.Read(rightOff + idx - leftLen);
        }
    }

    /// <summary>
    /// Selects a pivot value from split input using the same pseudo-median-of-3 heuristic
    /// as <see cref="SelectPivotIndex{T,TComparer,TContext}"/>, but reads from the logical
    /// concatenation of left and right which may reside in different buffers.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T SelectPivotFromInput<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        bool leftInMain, int leftOff, int leftLen,
        bool rightInMain, int rightOff, int rightLen)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = leftLen + rightLen;
        var eighth = n / 8;

        var a = 0;
        var b = n / 2 - eighth;
        var c = n - eighth;

        if (n < PSEUDO_MEDIAN_REC_THRESHOLD)
        {
            return Median3ValueFromInput(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, a, b, c);
        }
        else
        {
            return Median3RecValueFromInput(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, a, b, c, eighth);
        }
    }

    /// <summary>
    /// Recursively selects a pivot value from split input by sampling from three regions.
    /// </summary>
    private static T Median3RecValueFromInput<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        bool leftInMain, int leftOff, int leftLen,
        bool rightInMain, int rightOff,
        int a, int b, int c, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n8 = n / 8;
        if (n * 8 >= PSEUDO_MEDIAN_REC_THRESHOLD)
        {
            a = Median3IdxFromInput(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, a, a + n8 * 4, a + n8 * 7);
            b = Median3IdxFromInput(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, b, b + n8 * 4, b + n8 * 7);
            c = Median3IdxFromInput(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, c, c + n8 * 4, c + n8 * 7);
        }
        return Median3ValueFromInput(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, a, b, c);
    }

    /// <summary>
    /// Returns the logical index of the median of three logical indices from split input.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Median3IdxFromInput<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        bool leftInMain, int leftOff, int leftLen,
        bool rightInMain, int rightOff,
        int a, int b, int c)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var va = ReadSplitInput(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, a);
        var vb = ReadSplitInput(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, b);
        var vc = ReadSplitInput(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, c);

        var x = s.Compare(va, vb) < 0;
        var y = s.Compare(va, vc) < 0;
        if (x == y)
        {
            var z = s.Compare(vb, vc) < 0;
            return (z ^ x) ? c : b;
        }
        return a;
    }

    /// <summary>
    /// Returns the median value of three logical indices from split input.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T Median3ValueFromInput<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        bool leftInMain, int leftOff, int leftLen,
        bool rightInMain, int rightOff,
        int a, int b, int c)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var va = ReadSplitInput(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, a);
        var vb = ReadSplitInput(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, b);
        var vc = ReadSplitInput(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, c);

        var x = s.Compare(va, vb) < 0;
        var y = s.Compare(va, vc) < 0;
        if (x == y)
        {
            var z = s.Compare(vb, vc) < 0;
            return (z ^ x) ? vc : vb;
        }
        return va;
    }

    /// <summary>
    /// Recursive stable bidirectional 2-way quicksort with dest/scratch swap.
    /// <para>
    /// Matches the reference <c>stable_bidir_quicksort_into</c>: input data is specified
    /// as two "halves" (left for forward scan, right for backward scan) that may reside in
    /// either <c>s</c> (main) or <c>t</c> (scratch). Output is always written to
    /// <c>s[destStart..destStart+n)</c> with <c>t[scrStart..scrStart+n)</c> as scratch.
    /// </para>
    /// <para>
    /// After partitioning, the four result groups (less_fwd in dest, less_bwd in scratch,
    /// geq_fwd in scratch, geq_bwd in dest) are passed directly to recursive calls
    /// <b>without copying back</b>. Only at the base case (small sort / recursion-limit
    /// fallback) are elements assembled into dest. This reduces total data movement from
    /// O(n log n) to O(n) copies over the full recursion tree.
    /// </para>
    /// </summary>
    private static void StableQuicksortInto<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        Span<T> scratch,
        bool leftInMain, int leftOff, int leftLen,
        bool rightInMain, int rightOff, int rightLen,
        int destStart, int scrStart,
        int recursionLimit,
        byte strategy, T prevPivot,
        TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (true)
        {
            var n = leftLen + rightLen;

            // --- Base case ---
            // Assemble split input into dest, then sort in-place.
            if (n < SMALL_SORT || recursionLimit == 0)
            {
                // Copy left → s[destStart..destStart+leftLen)
                if (leftLen > 0)
                {
                    if (leftInMain) { if (leftOff != destStart) s.CopyTo(leftOff, s, destStart, leftLen); }
                    else { t.CopyTo(leftOff, s, destStart, leftLen); }
                }
                // Copy right → s[destStart+leftLen..destStart+n)
                if (rightLen > 0)
                {
                    var rightDest = destStart + leftLen;
                    if (rightInMain) { if (rightOff != rightDest) s.CopyTo(rightOff, s, rightDest, rightLen); }
                    else { t.CopyTo(rightOff, s, rightDest, rightLen); }
                }

                if (n < SMALL_SORT)
                {
                    // Slice t to the current scratch region so that BlockInsertionSort
                    // (which indexes from 0) does not corrupt sibling recursion data.
                    var tLocal = t.Slice(scrStart, n, BUFFER_TEMP);
                    BlockInsertionSort(s, tLocal, destStart, destStart + n);
                }
                else
                {
                    var tLocal = t.Slice(scrStart, n, BUFFER_TEMP);
                    GlidesortCore(s, tLocal, scratch.Slice(scrStart, n), destStart, destStart + n, eagerSmallsort: true, comparer, context);
                }
                return;
            }

            recursionLimit--;

            // --- Pivot selection from split input ---
            T pivot;
            bool partitionLeft;

            if (strategy == STRATEGY_LEFT_WITH_PIVOT)
            {
                pivot = prevPivot;
                partitionLeft = true;
            }
            else
            {
                pivot = SelectPivotFromInput(s, t, leftInMain, leftOff, leftLen, rightInMain, rightOff, rightLen);

                if (strategy == STRATEGY_LEFT_IF_EQUAL)
                {
                    partitionLeft = s.Compare(prevPivot, pivot) >= 0;
                }
                else
                {
                    partitionLeft = false;
                }
            }

            // --- Bidirectional 2-way stable partition ---
            //
            // Forward scan through left: less → s[destFront++], geq → t[scrFront++]
            // Backward scan through right (reverse): geq → s[destBack--], less → t[scrBack--]
            //
            // Overlap safety:
            //   Forward:  destFront ≤ fwdCur (when left is in s, leftOff == destStart)
            //             scrFront  ≤ fwdCur (when left is in t, leftOff == scrStart)
            //   Backward: bwdCur    ≤ destBack (when right is in s)
            //             bwdCur    ≤ scrBack  (when right is in t)
            //   Cross: forward writes in s end at destStart+lessFwdCount ≤ destStart+leftLen,
            //          backward reads from s start at rightOff ≥ destStart+leftLen (when contiguous).
            var destFront = destStart;
            var destBack = destStart + n - 1;
            var scrFront = scrStart;
            var scrBack = scrStart + n - 1;

            // Forward scan through left
            for (var i = leftOff; i < leftOff + leftLen; i++)
            {
                var val = leftInMain ? s.Read(i) : t.Read(i);
                var cmp = s.Compare(val, pivot);
                bool isLess = partitionLeft ? cmp <= 0 : cmp < 0;

                if (isLess)
                {
                    if (!leftInMain || destFront != i) s.Write(destFront, val);
                    destFront++;
                }
                else
                {
                    if (leftInMain || scrFront != i) t.Write(scrFront, val);
                    scrFront++;
                }
            }

            // Backward scan through right (reverse order)
            for (var i = rightOff + rightLen - 1; i >= rightOff; i--)
            {
                var val = rightInMain ? s.Read(i) : t.Read(i);
                var cmp = s.Compare(val, pivot);
                bool isLess = partitionLeft ? cmp <= 0 : cmp < 0;

                if (!isLess)
                {
                    if (!rightInMain || destBack != i) s.Write(destBack, val);
                    destBack--;
                }
                else
                {
                    if (rightInMain || scrBack != i) t.Write(scrBack, val);
                    scrBack--;
                }
            }

            // Count elements in each group.
            var lessFwdCount = destFront - destStart;
            var lessBwdCount = scrStart + n - 1 - scrBack;
            var geqFwdCount = scrFront - scrStart;
            var geqBwdCount = destStart + n - 1 - destBack;
            var lessTotal = lessFwdCount + lessBwdCount;
            var geqTotal = geqFwdCount + geqBwdCount;

            // Four groups — NO copy-back needed:
            //   less_fwd: s[destStart .. destStart+lessFwdCount)
            //   less_bwd: t[scrStart+n-lessBwdCount .. scrStart+n)
            //   geq_fwd:  t[scrStart .. scrStart+geqFwdCount)
            //   geq_bwd:  s[destStart+n-geqBwdCount .. destStart+n)

            // --- PartitionStrategy: all-elements-geq → re-partition with LeftWithPivot ---
            if (lessTotal == 0 && !partitionLeft)
            {
                strategy = STRATEGY_LEFT_WITH_PIVOT;
                prevPivot = pivot;
                // All data is in geq → new input for tail-call:
                leftInMain = false;
                leftOff = scrStart;
                leftLen = geqFwdCount;
                rightInMain = true;
                rightOff = destStart + n - geqBwdCount;
                rightLen = geqBwdCount;
                // destStart, scrStart, n unchanged
                continue;
            }

            // --- PartitionLeft: less side is all-equal → skip, recurse only on geq ---
            if (partitionLeft)
            {
                // less_bwd elements are in scratch — assemble them into dest next to less_fwd.
                if (lessBwdCount > 0)
                {
                    t.CopyTo(scrStart + n - lessBwdCount, s, destStart + lessFwdCount, lessBwdCount);
                }

                strategy = STRATEGY_RIGHT;
                prevPivot = default!;
                leftInMain = false;
                leftOff = scrStart;
                leftLen = geqFwdCount;
                rightInMain = true;
                rightOff = destStart + n - geqBwdCount;
                rightLen = geqBwdCount;
                destStart = destStart + lessTotal;
                // scrStart unchanged; n = geqTotal (implicit via leftLen + rightLen)
                continue;
            }

            // --- Normal partition: recurse on both sides ---
            //
            // Less recursion:
            //   left  = less_fwd in s[destStart .. +lessFwdCount)
            //   right = less_bwd in t[scrStart+n-lessBwdCount .. +lessBwdCount)
            //   dest  = s[destStart .. +lessTotal)
            //   scratch = t[scrStart+geqTotal .. +lessTotal)
            //
            // Geq recursion:
            //   left  = geq_fwd in t[scrStart .. +geqFwdCount)
            //   right = geq_bwd in s[destStart+n-geqBwdCount .. +geqBwdCount)
            //   dest  = s[destStart+lessTotal .. +geqTotal)
            //   scratch = t[scrStart .. +geqTotal)

            if (lessTotal <= geqTotal)
            {
                // Recurse on smaller (less) side
                if (lessTotal > 0)
                {
                    StableQuicksortInto(s, t, scratch,
                        true, destStart, lessFwdCount,
                        false, scrStart + n - lessBwdCount, lessBwdCount,
                        destStart, scrStart + geqTotal,
                        recursionLimit, STRATEGY_RIGHT, default!,
                        comparer, context);
                }

                // Tail-call on larger (geq) side
                strategy = STRATEGY_LEFT_IF_EQUAL;
                prevPivot = pivot;
                leftInMain = false;
                leftOff = scrStart;
                leftLen = geqFwdCount;
                rightInMain = true;
                rightOff = destStart + n - geqBwdCount;
                rightLen = geqBwdCount;
                destStart = destStart + lessTotal;
                // scrStart unchanged; geq scratch = t[scrStart..+geqTotal)
                continue;
            }
            else
            {
                // Recurse on smaller (geq) side
                if (geqTotal > 0)
                {
                    StableQuicksortInto(s, t, scratch,
                        false, scrStart, geqFwdCount,
                        true, destStart + n - geqBwdCount, geqBwdCount,
                        destStart + lessTotal, scrStart,
                        recursionLimit, STRATEGY_LEFT_IF_EQUAL, pivot,
                        comparer, context);
                }

                // Tail-call on larger (less) side
                strategy = STRATEGY_RIGHT;
                prevPivot = default!;
                leftInMain = true;
                leftOff = destStart;
                leftLen = lessFwdCount;
                rightInMain = false;
                rightOff = scrStart + n - lessBwdCount;
                rightLen = lessBwdCount;
                // destStart unchanged; less scratch = t[scrStart+geqTotal..+lessTotal)
                scrStart = scrStart + geqTotal;
                continue;
            }
        }
    }

    // Branchless Small Sort (Block Insertion Sort)

    /// <summary>
    /// Sorts the range [start..end) using the Glidesort block insertion sort.
    /// First sorts the first min(n, 32) elements using branchless sorting networks
    /// (Sort4 → Sort8 → Sort16 → Sort32), then inserts remaining elements into the
    /// sorted prefix in blocks of up to 32 via a backward merge from scratch space.
    /// </summary>
    /// <remarks>
    /// Matches the reference <c>block_insertion_sort</c> from the Rust glidesort codebase.
    /// For SMALL_SORT=48, this is at most two passes: one Sort32 and one 16-element block insert.
    /// The scratch span <paramref name="t"/> is used for the Sort8/Sort16/Sort32 merge steps
    /// and for staging each block before insertion. All accesses are sequential and non-overlapping.
    /// </remarks>
    private static void BlockInsertionSort<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        int start, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = end - start;
        if (n <= 1) return;

        // Sort the first min(n, 32) elements using branchless sorting networks.
        var numSorted = SmallSortPartial(s, t, start, end);

        // Insert remaining elements in blocks of up to 32 at a time.
        while (start + numSorted < end)
        {
            var blockStart = start + numSorted;
            var blockLen = Math.Min(end - blockStart, 32);

            // Copy block to scratch space and sort it there (in-place via insertion sort;
            // blockLen ≤ 32 so no extra scratch is needed for the sort itself).
            s.CopyTo(blockStart, t, 0, blockLen);
            InsertionSort.SortCore(t, 0, blockLen);

            // Backward-merge sorted scratch into the sorted prefix.
            BlockInsert(s, t, start, numSorted, blockLen);
            numSorted += blockLen;
        }
    }

    /// <summary>
    /// Sorts the first min(end-start, 32) elements of [start..end) in place using
    /// branchless sorting networks. Returns the count sorted: 32, 16, 8, 4, or n (for n&lt;4).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int SmallSortPartial<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        int start, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = end - start;
        if (n >= 32) { Sort32(s, t, start); return 32; }
        if (n >= 16) { Sort16(s, t, start); return 16; }
        if (n >= 8)  { Sort8(s, t, start);  return 8; }
        if (n >= 4)  { Sort4(s, start);     return 4; }
        InsertionSort.SortCore(s, start, end);
        return n;
    }

    /// <summary>
    /// Backward-merges the sorted scratch block t[0..blockLen) into the sorted prefix
    /// s[sStart..sStart+sLen), writing the merged result into s[sStart..sStart+sLen+blockLen).
    /// Works right-to-left so elements s[sStart+sLen..+blockLen) (the gap that holds the
    /// original unsorted block) are filled with the correctly ordered merge output.
    /// </summary>
    /// <remarks>
    /// Corresponds to <c>BlockInserter::insert</c> in the Rust reference.
    /// Stability: equal elements from the left prefix (s) are output before those from the
    /// right scratch block (t) because we use <see cref="SortSpan{T,TComparer,TContext}.Compare(T,T)"/> &gt; 0
    /// (strict greater-than) as the condition to drain from s.
    /// </remarks>
    private static void BlockInsert<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        int sStart, int sLen, int blockLen)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var o  = sStart + sLen + blockLen - 1; // output pointer (right to left)
        var si = sStart + sLen - 1;            // right end of sorted prefix
        var ti = blockLen - 1;                 // right end of scratch block

        while (si >= sStart && ti >= 0)
        {
            var sv = s.Read(si);
            var tv = t.Read(ti);
            // > for stability: drain sv only when it is strictly greater,
            // so equal elements from the left prefix always come first.
            if (s.Compare(sv, tv) > 0)
            {
                s.Write(o--, sv);
                si--;
            }
            else
            {
                s.Write(o--, tv);
                ti--;
            }
        }

        // Drain any remaining scratch elements.
        // Prefix elements that remain are already in their correct positions.
        while (ti >= 0)
        {
            s.Write(o--, t.Read(ti--));
        }
    }

    /// <summary>
    /// Simple forward merge: copies left run to scratch, then merges forward.
    /// Used by sorting networks (Sort8/Sort16/Sort32) for merging fixed-size halves.
    /// </summary>
    private static void MergeLow<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        int base1, int len1, int base2, int len2)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        s.CopyTo(base1, t, 0, len1);

        var c1 = 0;
        var e1 = len1;
        var c2 = base2;
        var e2 = base2 + len2;
        var o = base1;

        while (c1 < e1 && c2 < e2)
        {
            var val1 = t.Read(c1);
            var val2 = s.Read(c2);

            if (s.Compare(val1, val2) <= 0)
            {
                s.Write(o++, val1);
                c1++;
            }
            else
            {
                s.Write(o++, val2);
                c2++;
            }
        }

        if (c1 < e1)
        {
            t.CopyTo(c1, s, o, e1 - c1);
        }
    }

    /// <summary>
    /// Sorts 4 consecutive elements at [i..i+4) using a stable 5-comparison sorting network.
    /// Network: (0,2)→(1,3)→(0,1)→(2,3)→(1,2). Always performs exactly 5 comparisons.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Sort4<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int i)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (s.Compare(i,     i + 2) > 0) s.Swap(i,     i + 2);
        if (s.Compare(i + 1, i + 3) > 0) s.Swap(i + 1, i + 3);
        if (s.Compare(i,     i + 1) > 0) s.Swap(i,     i + 1);
        if (s.Compare(i + 2, i + 3) > 0) s.Swap(i + 2, i + 3);
        if (s.Compare(i + 1, i + 2) > 0) s.Swap(i + 1, i + 2);
    }

    /// <summary>
    /// Sorts 8 consecutive elements at [i..i+8) by sorting two groups of 4 then merging.
    /// </summary>
    private static void Sort8<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> t, int i)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        Sort4(s, i);
        Sort4(s, i + 4);
        if (s.Compare(i + 3, i + 4) <= 0) return; // Already in order — skip merge.
        MergeLow(s, t, i, 4, i + 4, 4);
    }

    /// <summary>
    /// Sorts 16 consecutive elements at [i..i+16) by sorting two groups of 8 then merging.
    /// </summary>
    private static void Sort16<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> t, int i)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        Sort8(s, t, i);
        Sort8(s, t, i + 8);
        if (s.Compare(i + 7, i + 8) <= 0) return; // Already in order — skip merge.
        MergeLow(s, t, i, 8, i + 8, 8);
    }

    /// <summary>
    /// Sorts 32 consecutive elements at [i..i+32) by sorting two groups of 16 then merging.
    /// Requires t to have capacity ≥ 16 (used by the final MergeLow call).
    /// </summary>
    private static void Sort32<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> t, int i)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        Sort16(s, t, i);
        Sort16(s, t, i + 16);
        if (s.Compare(i + 15, i + 16) <= 0) return; // Already in order — skip merge.
        MergeLow(s, t, i, 16, i + 16, 16);
    }

    // Pivot Selection

    /// <summary>
    /// Selects a pivot index using pseudo-median-of-3 with recursive refinement for large arrays.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int SelectPivotIndex<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, int start, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = end - start;
        var eighth = n / 8;

        var a = start;
        var b = start + n / 2 - eighth;
        var c = end - eighth;

        if (n < PSEUDO_MEDIAN_REC_THRESHOLD)
        {
            return Median3Index(s, a, b, c);
        }
        else
        {
            return Median3RecIndex(s, a, b, c, eighth);
        }
    }

    /// <summary>
    /// Recursively computes an approximate median by sampling from three regions.
    /// </summary>
    private static int Median3RecIndex<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, int a, int b, int c, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n8 = n / 8;
        if (n * 8 >= PSEUDO_MEDIAN_REC_THRESHOLD)
        {
            a = Median3RecIndex(s, a, a + n8 * 4, a + n8 * 7, n8);
            b = Median3RecIndex(s, b, b + n8 * 4, b + n8 * 7, n8);
            c = Median3RecIndex(s, c, c + n8 * 4, c + n8 * 7, n8);
        }
        return Median3Index(s, a, b, c);
    }

    /// <summary>
    /// Returns the index of the median of three elements.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Median3Index<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s, int a, int b, int c)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var x = s.Compare(a, b) < 0;
        var y = s.Compare(a, c) < 0;
        if (x == y)
        {
            // If x=y=false then b,c <= a, return max(b,c)
            // If x=y=true then a < b,c, return min(b,c)
            var z = s.Compare(b, c) < 0;
            return (z ^ x) ? c : b;
        }
        else
        {
            // Either c <= a < b or b <= a < c, so a is the median
            return a;
        }
    }

    // Utility

    /// <summary>
    /// Reverses elements in the range [lo..hi] (inclusive).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    /// Computes the scratch buffer size for a given input size.
    /// Matches the reference scaling strategy: full allocation (n) up to 1 MB total bytes,
    /// half allocation (n/2) up to 1 GB, then n/8 for very large data.
    /// Always allocates at least SMALL_SORT elements so that small-sort operations succeed.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ComputeScratchSize<T>(int n)
    {
        var tlen = Unsafe.SizeOf<T>();
        tlen = Math.Max(tlen, 1);

        var fullAllowed = Math.Min(n, FULL_ALLOC_MAX_BYTES / tlen);
        var halfAllowed = Math.Min(n / 2, HALF_ALLOC_MAX_BYTES / tlen);
        var eighthAllowed = n / 8;

        var size = Math.Max(fullAllowed, Math.Max(halfAllowed, eighthAllowed));
        return Math.Max(size, SMALL_SORT);
    }
}
