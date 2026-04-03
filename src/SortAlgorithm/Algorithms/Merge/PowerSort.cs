using SortAlgorithm.Contexts;
using System.Buffers;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// PowerSortは、ランの「パワー」に基づいてマージ順序を最適化する改良型適応マージソートアルゴリズムです。
/// TimSortの不変条件ベースのマージ戦略を、node powerによる単一条件に置き換え、
/// 証明可能な最適マージコストと、最悪ケースO(n log n)、ほぼソート済みデータではO(n)を実現します。
/// <br/>
/// PowerSort is an improved adaptive merge sort that replaces TimSort's dual invariants
/// with a single power-based merge condition, achieving provably optimal merge costs.
/// It maintains O(n log n) worst-case time complexity while achieving O(n) on nearly sorted data.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct PowerSort:</strong></para>
/// <list type="number">
/// <item><description><strong>Run Detection:</strong> Identify maximal monotonic sequences (runs).
/// Ascending runs use ≤ comparison for stability, strictly descending runs use &gt; and are reversed.</description></item>
/// <item><description><strong>Run Extension:</strong> Short runs (&lt; MIN_RUN) are extended using Insertion Sort
/// to ensure efficient merging (matching the reference PowerSort default of 24).</description></item>
/// <item><description><strong>NodePower Calculation:</strong> For adjacent runs [s1..e1) and [s2..e2) in an array of size n,
/// the node power is the first bit position where the normalized midpoints diverge in binary.
/// Computed via 64-bit fixed-point arithmetic and LeadingZeroCount (matching the reference <c>node_power_clz</c>).</description></item>
/// <item><description><strong>Merge Condition:</strong> Merge runs when the previous boundary power exceeds the new boundary power (bp[i-1] &gt; p).
/// This single condition replaces TimSort's two invariants, maintaining non-decreasing boundary powers on the stack
/// and ensuring nearly-optimal total merge cost.</description></item>
/// <item><description><strong>Stable Merging:</strong> Simple linear merge (copy smaller run to buffer, merge back).
/// Stability is preserved with ≤ comparison when choosing from the left run.
/// No galloping — PowerSort optimizes merge ORDER via node power, not individual merge operations.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Hybrid (Merge + Insertion)</description></item>
/// <item><description>Stable      : Yes (preserves relative order of equal elements)</description></item>
/// <item><description>In-place    : No (requires O(n/2) temporary space for merging)</description></item>
/// <item><description>Best case   : O(n) - Already sorted or reverse sorted data (single run)</description></item>
/// <item><description>Average case: O(n log n) - Optimal merge tree construction</description></item>
/// <item><description>Worst case  : O(n log n) - Guaranteed by power-based merge strategy</description></item>
/// <item><description>Comparisons : Best O(n), Average/Worst O(n log n) - Exploits existing order</description></item>
/// <item><description>Writes      : Best O(1), Average/Worst O(n log n) - Minimal for sorted data</description></item>
/// <item><description>Space       : O(n/2) worst-case for temporary merge buffer</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>MIN_RUN = 24: Fixed minimum run length matching the reference PowerSort default.</description></item>
/// <item><description>NodePower: Uses 64-bit fixed-point scaling <c>(twoM &lt;&lt; 30) / n</c> and <c>BitOperations.LeadingZeroCount</c>.</description></item>
/// <item><description>Merge: Copy-smaller strategy (MergeLow/MergeHigh) — copies the smaller run to a buffer rented from ArrayPool.</description></item>
/// </list>
/// <para><strong>References:</strong></para>
/// <para>Paper: "Nearly-Optimal Mergesort: Fast, Practical Sorting Methods That Optimally Adapt to Existing Runs"
/// by J. Ian Munro and Sebastian Wild (2018)</para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Powersort</para>
/// <para>ICALP 2022: https://drops.dagstuhl.de/storage/00lipics/lipics-vol229-icalp2022/LIPIcs.ICALP.2022.68/LIPIcs.ICALP.2022.68.pdf</para>
/// <para>arXiv: https://arxiv.org/abs/1805.04154</para>
/// </remarks>
public static class PowerSort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Temporary merge buffer

    // PowerSort constants
    private const int MIN_RUN = 24;          // Fixed minimum run length (matches reference PowerSort default)

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

        // For very small arrays, use insertion sort directly
        if (n < MIN_RUN)
        {
            InsertionSort.Sort(span, first, last, comparer, context);
            return;
        }

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        SortCore(s, first, last, comparer, context);
    }

    /// <summary>
    /// Core PowerSort implementation.
    /// Based on the algorithm from the PowerSort paper (Algorithm in Section 5.3).
    /// </summary>
    /// <remarks>
    /// <para><strong>Algorithm Structure with Explicit Boundary Stack:</strong></para>
    /// <list type="bullet">
    /// <item><description>runCount = number of runs on stack</description></item>
    /// <item><description>bpCount = number of boundary powers (always runCount - 1)</description></item>
    /// <item><description>Invariant: bp[i] = boundary power between runs[i] and runs[i+1]</description></item>
    /// <item><description>Merge condition: bp[bpCount-1] > p where p = boundary(top, nextRun)</description></item>
    /// <item><description>After merge: runCount--, bpCount-- (boundary stack shrinks together with run stack)</description></item>
    /// </list>
    /// <para>This explicit management ensures data structure invariants are always maintained,
    /// preventing future bugs from stale boundary values.</para>
    /// </remarks>
    private static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = last - first;

        // PowerSort uses two parallel stacks:
        // 1. Run stack (runBase, runLen) - stores the runs themselves
        // 2. Boundary power stack (bp) - stores powers of boundaries between adjacent runs
        // Invariant: if runCount = k, then bpCount = k - 1
        Span<int> runBase = stackalloc int[85]; // Conservative fixed upper bound for run/boundary stack depth.
        Span<int> runLen = stackalloc int[85];
        Span<int> bp = stackalloc int[85];      // bp[i] = power of boundary between runs[i] and runs[i+1]
        var runCount = 0;   // Number of runs on stack
        var bpCount = 0;    // Number of boundary powers (always runCount - 1)

        // Find and push the first run onto the stack
        var runStart = first;
        var runEnd = FindAndPrepareRun(s, runStart, last);
        runBase[0] = runStart;
        runLen[0] = runEnd - runStart;
        runCount = 1;

        // Early exit: if the entire array is a single run, we're done
        // This avoids unnecessary buffer allocation for already sorted or reversed arrays
        if (runEnd >= last)
        {
            return; // Already sorted (either ascending or reversed and then reversed back)
        }

        // Reusable temporary buffer for merging
        // Start with minRun size (reasonable initial capacity)
        // MergeLow needs len1, MergeHigh needs len2, so we track the smaller run size
        var tmpBufferSize = Math.Min(MIN_RUN * 2, n / 2);
        T[] tmpBuffer = ArrayPool<T>.Shared.Rent(tmpBufferSize);

        try
        {
            // No boundary yet (need at least 2 runs for a boundary)
            context.OnPhase(SortPhase.MergeRunDetect);

            // Process remaining runs
            runStart = runEnd;
            while (runStart < last)
            {
                // Find next run (but don't push yet)
                var nextStart = runStart;
                var nextEnd = FindAndPrepareRun(s, nextStart, last);

                // Compute the power of the boundary between current stack top and next run
                var topBase = runBase[runCount - 1];
                var topLen = runLen[runCount - 1];
                var p = ComputeNodePower(topBase - first, topBase + topLen - first, nextStart - first, nextEnd - first, n);

                // Merge while the previous boundary has higher power than current boundary.
                // Per the PowerSort paper (Algorithm alg:powersort): p is computed once before
                // the loop and stays constant throughout. Each merge extends the current run
                // leftward (e1 is unchanged), so the boundary at e1..s2 and its power p remain
                // the same regardless of how many merges occur.
                // bp[bpCount-1] is the boundary between runs[runCount-2] and runs[runCount-1]
                // p is the boundary between runs[runCount-1] and nextRun
                // If bp[bpCount-1] > p, merge runs[runCount-2] and runs[runCount-1]
                while (bpCount > 0 && bp[bpCount - 1] > p)
                {
                    // Verify invariant: bpCount should always equal runCount - 1
                    Debug.Assert(bpCount == runCount - 1, $"Invariant violated before merge: bpCount={bpCount}, runCount={runCount}, expected bpCount={runCount - 1}");

                    // Merge the top two runs: runs[runCount-2] and runs[runCount-1]
                    var base1 = runBase[runCount - 2];
                    var len1 = runLen[runCount - 2];
                    var base2 = runBase[runCount - 1];
                    var len2 = runLen[runCount - 1];

                    // Verify structural consistency: runs must be adjacent
                    Debug.Assert(base1 + len1 == base2, $"Runs are not adjacent: run[{runCount - 2}] ends at {base1 + len1}, run[{runCount - 1}] starts at {base2}");

                    MergeRuns(s, base1, len1, base2, len2, ref tmpBuffer, ref tmpBufferSize, comparer, context);

                    // Replace the two runs with the merged result
                    runBase[runCount - 2] = base1;
                    runLen[runCount - 2] = len1 + len2;
                    runCount--;  // One less run on stack
                    bpCount--;   // One less boundary (the boundary between the merged runs is gone)

                    // Verify invariant after merge
                    Debug.Assert(bpCount == runCount - 1, $"Invariant violated after merge: bpCount={bpCount}, runCount={runCount}, expected bpCount={runCount - 1}");
                    // p is NOT recalculated here (matches the paper's algorithm)
                }

                // Push the next run onto the stack
                runBase[runCount] = nextStart;
                runLen[runCount] = nextEnd - nextStart;
                runCount++;

                // Push the boundary power between the new top and the previous top
                bp[bpCount] = p;
                bpCount++;

                // Verify invariant after push: bpCount should always equal runCount - 1
                Debug.Assert(bpCount == runCount - 1, $"Invariant violated after push: bpCount={bpCount}, runCount={runCount}, expected bpCount={runCount - 1}");

                runStart = nextEnd;
            }

            // Force merge all remaining runs from right to left
            context.OnPhase(SortPhase.MergeRunCollapse, runCount);
            while (runCount > 1)
            {
                // Verify invariant before final merge
                Debug.Assert(bpCount == runCount - 1, $"Invariant violated in final collapse before merge: bpCount={bpCount}, runCount={runCount}, expected bpCount={runCount - 1}");

                // Always merge the last two runs
                var base1 = runBase[runCount - 2];
                var len1 = runLen[runCount - 2];
                var base2 = runBase[runCount - 1];
                var len2 = runLen[runCount - 1];

                // Verify structural consistency
                Debug.Assert(base1 + len1 == base2, $"Runs are not adjacent in final collapse: run[{runCount - 2}] ends at {base1 + len1}, run[{runCount - 1}] starts at {base2}");

                MergeRuns(s, base1, len1, base2, len2, ref tmpBuffer, ref tmpBufferSize, comparer, context);

                // Replace the two runs with the merged result
                runLen[runCount - 2] = len1 + len2;
                runCount--;
                bpCount--;  // One less boundary

                // Verify invariant after final merge
                Debug.Assert(bpCount == runCount - 1, $"Invariant violated in final collapse after merge: bpCount={bpCount}, runCount={runCount}, expected bpCount={runCount - 1}");
            }

            // Final check: should have exactly 1 run and 0 boundaries
            Debug.Assert(runCount == 1 && bpCount == 0, $"Final state check failed: runCount={runCount}, bpCount={bpCount}, expected runCount=1 and bpCount=0");
        }
        finally
        {
            // Return the rented array to the pool
            ArrayPool<T>.Shared.Return(tmpBuffer, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    /// <summary>
    /// Finds and prepares a run starting at position 'start'.
    /// Returns the end position (exclusive) of the prepared run.
    /// A run is either ascending or strictly descending (which is then reversed).
    /// Short runs are extended to MIN_RUN using insertion sort.
    /// </summary>
    private static int FindAndPrepareRun<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int start, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var runEnd = start + 1;

        if (runEnd >= last)
        {
            return last;  // Single element run
        }

        // Check if descending
        if (s.IsGreaterAt(start, runEnd))
        {
            // Strictly descending run
            while (runEnd < last && s.IsGreaterAt(runEnd - 1, runEnd))
            {
                runEnd++;
            }
            // Reverse the descending run to make it ascending
            Reverse(s, start, runEnd - 1);
        }
        else
        {
            // Ascending run (allowing equals for stability)
            while (runEnd < last && s.IsLessOrEqualAt(runEnd - 1, runEnd))
            {
                runEnd++;
            }
        }

        var runLength = runEnd - start;

        // If run is too small, extend it to MIN_RUN using binary insertion sort
        if (runLength < MIN_RUN)
        {
            var force = Math.Min(MIN_RUN, last - start);
            InsertionSort.SortCore(s, start, start + force, start + runLength);
            runEnd = start + force;
        }

        return runEnd;
    }

    /// <summary>
    /// Reverses the elements in the range [lo..hi].
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
    /// Computes the node power for PowerSort merge strategy.
    /// Matches the reference PowerSort's <c>node_power_clz</c> implementation using 64-bit arithmetic only.
    /// </summary>
    /// <remarks>
    /// <para>
    /// For two adjacent runs [s1..e1) and [s2..e2) in an array of size n,
    /// the node power is the first bit position where the normalized midpoints diverge in binary.
    /// This implementation merges boundaries with larger power values first.
    /// </para>
    /// <para>
    /// Algorithm: Compute 2× midpoints (twoM1 = s1+e1, twoM2 = s2+e2), scale to 32-bit
    /// fixed-point via <c>(twoM &lt;&lt; 30) / n</c>, then XOR + CLZ.
    /// Since n ≤ int.MaxValue and twoM &lt; 2n ≤ 2^32, the shift <c>twoM &lt;&lt; 30</c>
    /// fits in a 64-bit ulong (&lt; 2^62), avoiding UInt128 arithmetic entirely.
    /// </para>
    /// </remarks>
    private static int ComputeNodePower(int s1, int e1, int s2, int e2, int n)
    {
        // 2× midpoints: for run [s, e), midpoint = (s+e)/2, so 2*midpoint = s + e
        // Note: s1+e1 = s1+s2 since e1 = s2 (adjacent runs). Matches reference's l2 = beginA + beginB.
        ulong twoM1 = (ulong)(uint)s1 + (uint)e1;  // < 2n
        ulong twoM2 = (ulong)(uint)s2 + (uint)e2;  // < 2n

        // Scale to 32-bit fixed-point: (twoM << 30) / n
        // twoM < 2n ≤ 2^32, so twoM << 30 < 2^62 → fits in ulong (no UInt128 needed)
        // Result < 2^31 → fits in uint
        uint a = (uint)((twoM1 << 30) / (uint)n);
        uint b = (uint)((twoM2 << 30) / (uint)n);

        // XOR reveals first differing bit; CLZ gives the node power directly.
        // This works because the (twoM << 30) / n scaling maps [0,1) midpoints
        // to [0, 2^31) in uint32, where the always-zero MSB accounts for the +1
        // offset between "common prefix length" and "power" (= nCommonBits + 1).
        return BitOperations.LeadingZeroCount(a ^ b);
    }

    /// <summary>
    /// Merges two adjacent runs using simple linear merge.
    /// Copies the smaller run to the temporary buffer and merges back in-place.
    /// The temporary buffer is reused across multiple merges for efficiency.
    /// </summary>
    /// <remarks>
    /// This follows the reference PowerSort implementation's merge strategy (COPY_SMALLER):
    /// copy the smaller of the two runs to a temporary buffer and merge linearly.
    /// Unlike TimSort's galloping merge, PowerSort relies on optimal merge ORDER
    /// (via node power) rather than optimizing individual merge operations.
    /// </remarks>
    private static void MergeRuns<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int base1, int len1, int base2, int len2,
        ref T[] tmpBuffer, ref int tmpBufferSize, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (len1 <= len2)
        {
            MergeLow(s, base1, len1, base2, len2, ref tmpBuffer, ref tmpBufferSize, comparer, context);
        }
        else
        {
            MergeHigh(s, base1, len1, base2, len2, ref tmpBuffer, ref tmpBufferSize, comparer, context);
        }
    }

    /// <summary>
    /// Merges two adjacent runs where the first run is smaller or equal.
    /// Copies the first run to the temporary buffer and merges forward.
    /// Uses simple linear merge matching the reference PowerSort implementation.
    /// </summary>
    private static void MergeLow<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int base1, int len1, int base2, int len2,
        ref T[] tmpBuffer, ref int tmpBufferSize, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Ensure tmpBuffer is large enough for len1
        if (tmpBufferSize < len1)
        {
            ArrayPool<T>.Shared.Return(tmpBuffer, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
            tmpBufferSize = len1;
            tmpBuffer = ArrayPool<T>.Shared.Rent(tmpBufferSize);
        }

        // Copy first run to temp buffer
        var t = new SortSpan<T, TComparer, TContext>(tmpBuffer.AsSpan(0, len1), context, comparer, BUFFER_TEMP);
        s.CopyTo(base1, t, 0, len1);

        var c1 = 0;                // Cursor in temp (first run)
        var e1 = len1;             // End of first run in temp
        var c2 = base2;            // Cursor in span (second run)
        var e2 = base2 + len2;     // End of second run in span
        var o = base1;             // Output cursor in span

        // Simple linear merge forward
        while (c1 < e1 && c2 < e2)
        {
            var val1 = t.Read(c1);
            var val2 = s.Read(c2);

            if (s.IsLessOrEqual(val1, val2)) // <= for stability (prefer left run on equal)
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

        // Copy remaining elements from temp (run1)
        // Elements remaining in run2 are already in place
        if (c1 < e1)
        {
            t.CopyTo(c1, s, o, e1 - c1);
        }
    }

    /// <summary>
    /// Merges two adjacent runs where the second run is smaller.
    /// Copies the second run to the temporary buffer and merges backward.
    /// Uses simple linear merge matching the reference PowerSort implementation.
    /// </summary>
    private static void MergeHigh<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int base1, int len1, int base2, int len2,
        ref T[] tmpBuffer, ref int tmpBufferSize, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Ensure tmpBuffer is large enough for len2
        if (tmpBufferSize < len2)
        {
            ArrayPool<T>.Shared.Return(tmpBuffer, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
            tmpBufferSize = len2;
            tmpBuffer = ArrayPool<T>.Shared.Rent(tmpBufferSize);
        }

        // Copy second run to temp buffer
        var t = new SortSpan<T, TComparer, TContext>(tmpBuffer.AsSpan(0, len2), context, comparer, BUFFER_TEMP);
        s.CopyTo(base2, t, 0, len2);

        var c1 = base1 + len1 - 1;  // Cursor in span (first run, from end)
        var c2 = len2 - 1;          // Cursor in temp (second run, from end)
        var o = base2 + len2 - 1;   // Output cursor (from end)

        // Simple linear merge backward
        while (c1 >= base1 && c2 >= 0)
        {
            var val1 = s.Read(c1);
            var val2 = t.Read(c2);

            if (s.IsLessOrEqual(val1, val2)) // <= means val2 >= val1, take val2 for stability
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

        // Copy remaining elements from temp (run2)
        // Elements remaining in run1 are already in place
        if (c2 >= 0)
        {
            t.CopyTo(0, s, o - c2, c2 + 1);
        }
    }
}
