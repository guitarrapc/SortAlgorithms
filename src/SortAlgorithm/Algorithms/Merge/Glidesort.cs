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
            var (prevRunStart, prevRunEnd, prevRunType, prevRunMid) = CreateLogicalRun(s, cursor, last);
            cursor = prevRunEnd;

            while (cursor < last)
            {
                var nextRunStartIdx = prevRunStart - first;
                var nextRunEndIdx = prevRunEnd - first;

                // Create next logical run
                var (nextStart, nextEnd, nextType, nextMid) = CreateLogicalRun(s, cursor, last);
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
        finally
        {
            ArrayPool<T>.Shared.Return(scratchBuffer, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
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
        SortSpan<T, TComparer, TContext> s, int start, int last)
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
        return (start, start + skip, 0, 0); // Unsorted
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
    /// Applies merge reduction (shrinking already-sorted prefix/suffix) before merging.
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

        // Shrink: skip elements at start of left that are already in place
        // and elements at end of right that are already in place
        var left = start;
        var right = mid;
        var rightEnd = end;

        // If left's last element <= right's first element, already sorted
        if (s.Compare(mid - 1, mid) <= 0) return;

        // Skip left prefix already in place: find first left element > right's first
        while (left < mid && s.Compare(left, mid) <= 0)
        {
            left++;
        }

        // Skip right suffix already in place: find last right element < left's last
        while (rightEnd > mid && s.Compare(mid - 1, rightEnd - 1) <= 0)
        {
            rightEnd--;
        }

        if (left >= mid || rightEnd <= mid) return;

        var len1 = mid - left;
        var len2 = rightEnd - mid;

        if (len1 <= len2)
        {
            MergeLow(s, t, left, len1, mid, len2);
        }
        else
        {
            MergeHigh(s, t, left, len1, mid, len2);
        }
    }

    /// <summary>
    /// Merges when left run is smaller: copies left to scratch and merges forward.
    /// </summary>
    private static void MergeLow<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        int base1, int len1, int base2, int len2)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Copy left run to scratch
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

        if (c1 < e1)
        {
            t.CopyTo(c1, s, o, e1 - c1);
        }
    }

    /// <summary>
    /// Merges when right run is smaller: copies right to scratch and merges backward.
    /// </summary>
    private static void MergeHigh<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        int base1, int len1, int base2, int len2)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Copy right run to scratch
        s.CopyTo(base2, t, 0, len2);

        var c1 = base1 + len1 - 1;
        var c2 = len2 - 1;
        var o = base2 + len2 - 1;

        while (c1 >= base1 && c2 >= 0)
        {
            var val1 = s.Read(c1);
            var val2 = t.Read(c2);

            if (s.Compare(val1, val2) <= 0) // <= means take val2 for stability
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

        if (c2 >= 0)
        {
            t.CopyTo(0, s, o - c2, c2 + 1);
        }
    }

    /// <summary>
    /// Merges three adjacent sorted runs: [start..mid1), [mid1..mid2), [mid2..end).
    /// Merges the smaller pair first for efficiency.
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
        var len2 = end - mid2;

        if (len0 < len2)
        {
            // Merge first two, then merge result with third
            PhysicalMerge(s, t, scratch, start, mid1, mid2, comparer, context);
            PhysicalMerge(s, t, scratch, start, mid2, end, comparer, context);
        }
        else
        {
            // Merge last two, then merge first with result
            PhysicalMerge(s, t, scratch, mid1, mid2, end, comparer, context);
            PhysicalMerge(s, t, scratch, start, mid1, end, comparer, context);
        }
    }

    /// <summary>
    /// Merges four adjacent sorted runs: [start..mid1), [mid1..mid2), [mid2..mid3), [mid3..end).
    /// Merges pairs first, then merges the results.
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
        // Merge left pair and right pair, then merge results
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

        StableQuicksortRec(s, t, start, end, recursionLimit, STRATEGY_RIGHT, default!, comparer, context);
    }

    /// <summary>
    /// Recursive stable bidirectional 2-way quicksort with PartitionStrategy.
    /// <para>
    /// Partitions elements into (less, geq) groups using bidirectional scanning:
    /// forward scan writes less → s[front], geq → t[front]; backward scan writes
    /// geq → s[back], less → t[back]. Both groups are in stable order and reassembly
    /// requires only two disjoint copies from t to s.
    /// </para>
    /// <para>
    /// PartitionStrategy handles duplicates without explicit 3-way partitioning:
    /// STRATEGY_RIGHT partitions as (&lt; pivot) vs (≥ pivot).
    /// When lessTotal == 0, re-partitions with STRATEGY_LEFT_WITH_PIVOT as (≤ pivot) vs (&gt; pivot).
    /// Since all elements are ≥ pivot, the ≤ group = elements equal to pivot, which are already sorted.
    /// STRATEGY_LEFT_IF_EQUAL selects a new pivot and checks equality with the previous one.
    /// </para>
    /// </summary>
    private static void StableQuicksortRec<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        int start, int end, int recursionLimit,
        byte strategy, T prevPivot,
        TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (true)
        {
            var n = end - start;
            if (n < SMALL_SORT)
            {
                BlockInsertionSort(s, t, start, end);
                return;
            }

            if (recursionLimit == 0)
            {
                // Fallback: guaranteed O(n log n) stable merge sort.
                StableMergeSortFallback(s, t, start, end);
                return;
            }

            recursionLimit--;

            // Select pivot and determine partition direction based on strategy.
            T pivot;
            bool partitionLeft;

            if (strategy == STRATEGY_LEFT_WITH_PIVOT)
            {
                // Reuse the previous pivot; partition as (<= pivot) vs (> pivot).
                pivot = prevPivot;
                partitionLeft = true;
            }
            else
            {
                var pivotIdx = SelectPivotIndex(s, start, end);
                pivot = s.Read(pivotIdx);

                if (strategy == STRATEGY_LEFT_IF_EQUAL)
                {
                    // If prevPivot >= newPivot (i.e., they are equal or new is smaller),
                    // partition left to strip out equal elements.
                    partitionLeft = s.Compare(prevPivot, pivot) >= 0;
                }
                else
                {
                    partitionLeft = false;
                }
            }

            // Bidirectional 2-way stable partition.
            //
            // Forward scan from s[fwd++]:
            //   "less" side (< or <=) → s[destFront++]  (safe: destFront ≤ fwd)
            //   "geq"  side (>= or >) → t[scratchFront++]
            //
            // Backward scan from s[bwd--]:
            //   "geq"  side (>= or >) → s[destBack--]   (safe: destBack ≥ bwd)
            //   "less" side (< or <=) → t[scratchBack--]
            //
            // After partition:
            //   s[start..destFront)   = less_fwd  (stable order)
            //   s[destBack+1..end)    = geq_bwd   (stable order)
            //   t[0..scratchFront)    = geq_fwd   (stable order)
            //   t[scratchBack+1..n-1] = less_bwd  (stable order)
            //
            // Reassembly: less_fwd is already in place, geq_bwd is already in place.
            // Copy less_bwd and geq_fwd from t into the gap in s.
            var fwd = start;
            var bwd = end - 1;
            var destFront = start;
            var scratchFront = 0;
            var destBack = end - 1;
            var scratchBack = n - 1;

            while (fwd <= bwd)
            {
                // Forward scan
                var fwdVal = s.Read(fwd);
                var fwdCmp = s.Compare(fwdVal, pivot);
                bool fwdIsLess = partitionLeft ? fwdCmp <= 0 : fwdCmp < 0;

                if (fwdIsLess)
                {
                    if (destFront != fwd) s.Write(destFront, fwdVal);
                    destFront++;
                }
                else
                {
                    t.Write(scratchFront, fwdVal);
                    scratchFront++;
                }
                fwd++;

                if (fwd > bwd) break;

                // Backward scan
                var bwdVal = s.Read(bwd);
                var bwdCmp = s.Compare(bwdVal, pivot);
                bool bwdIsLess = partitionLeft ? bwdCmp <= 0 : bwdCmp < 0;

                if (!bwdIsLess)
                {
                    if (destBack != bwd) s.Write(destBack, bwdVal);
                    destBack--;
                }
                else
                {
                    t.Write(scratchBack, bwdVal);
                    scratchBack--;
                }
                bwd--;
            }

            // Count elements in each group.
            var lessFwdCount = destFront - start;
            var lessBwdCount = n - 1 - scratchBack;
            var geqFwdCount = scratchFront;
            var geqBwdCount = end - 1 - destBack;
            var lessTotal = lessFwdCount + lessBwdCount;
            var geqTotal = geqFwdCount + geqBwdCount;

            // Reassemble: s = [less_fwd | less_bwd | geq_fwd | geq_bwd]
            // less_fwd at s[start..start+lessFwdCount) — already in place.
            // geq_bwd at s[end-geqBwdCount..end) — already in place.
            // Copy less_bwd from t to fill the gap.
            if (lessBwdCount > 0)
            {
                t.CopyTo(scratchBack + 1, s, start + lessFwdCount, lessBwdCount);
            }
            // Copy geq_fwd from t to fill the gap.
            if (geqFwdCount > 0)
            {
                t.CopyTo(0, s, start + lessTotal, geqFwdCount);
            }

            // PartitionStrategy: handle the all-elements-geq case (duplicate handling).
            // When lessTotal == 0 and we partitioned right, all elements are >= pivot.
            // Re-partition with LeftWithPivot to separate == pivot (which don't need sorting)
            // from > pivot. This is the key mechanism for O(n log k) on many duplicates.
            if (lessTotal == 0 && !partitionLeft)
            {
                strategy = STRATEGY_LEFT_WITH_PIVOT;
                prevPivot = pivot;
                continue; // tail-call via loop
            }

            // When partitionLeft is true, the "less" side contains elements <= pivot.
            // This only happens after a LeftWithPivot or LeftIfEqual match, where all
            // elements were >= prevPivot. So (<= pivot) ∩ (>= prevPivot) with matching
            // pivots means these are all equal to pivot — no need to sort.
            // Only recurse on the "geq" (> pivot) side.
            if (partitionLeft)
            {
                strategy = STRATEGY_RIGHT;
                prevPivot = default!;
                start = start + lessTotal;
                continue; // tail-call via loop for the > side
            }

            // Normal (right) partition: recurse on both sides.
            // Use STRATEGY_LEFT_IF_EQUAL for the geq side to detect duplicate pivots.
            if (lessTotal <= geqTotal)
            {
                if (lessTotal > 1)
                {
                    StableQuicksortRec(s, t, start, start + lessTotal,
                        recursionLimit, STRATEGY_RIGHT, default!, comparer, context);
                }
                strategy = STRATEGY_LEFT_IF_EQUAL;
                prevPivot = pivot;
                start = start + lessTotal;
                // Continue loop for the >= side
            }
            else
            {
                if (geqTotal > 1)
                {
                    StableQuicksortRec(s, t, start + lessTotal, end,
                        recursionLimit, STRATEGY_LEFT_IF_EQUAL, pivot, comparer, context);
                }
                strategy = STRATEGY_RIGHT;
                prevPivot = default!;
                end = start + lessTotal;
                // Continue loop for the < side
            }
        }
    }

    /// <summary>
    /// Fallback stable merge sort for when quicksort recursion budget is exhausted.
    /// Guarantees O(n log n) worst-case via simple top-down merge sort using scratch buffer.
    /// </summary>
    private static void StableMergeSortFallback<T, TComparer, TContext>(
        SortSpan<T, TComparer, TContext> s,
        SortSpan<T, TComparer, TContext> t,
        int start, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = end - start;
        if (n < SMALL_SORT)
        {
            BlockInsertionSort(s, t, start, end);
            return;
        }

        var mid = start + n / 2;
        StableMergeSortFallback(s, t, start, mid);
        StableMergeSortFallback(s, t, mid, end);

        // Already sorted across the boundary — no merge needed
        if (s.Compare(mid - 1, mid) <= 0) return;

        var len1 = mid - start;
        var len2 = end - mid;
        if (len1 <= len2)
        {
            MergeLow(s, t, start, len1, mid, len2);
        }
        else
        {
            MergeHigh(s, t, start, len1, mid, len2);
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
