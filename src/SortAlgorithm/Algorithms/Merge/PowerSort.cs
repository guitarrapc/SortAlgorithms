using SortAlgorithm.Contexts;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

// reference: https://drops.dagstuhl.de/storage/00lipics/lipics-vol229-icalp2022/LIPIcs.ICALP.2022.68/LIPIcs.ICALP.2022.68.pdf
// reference: https://arxiv.org/abs/1805.04154
/// <summary>
/// PowerSort is an improved adaptive merge sort algorithm that optimizes the merge order
/// based on the "power" of runs, resulting in better performance than TimSort.
/// It maintains O(n log n) worst-case time complexity while achieving O(n) on nearly sorted data.
/// <br/>
/// PowerSortは、ランの「パワー」に基づいてマージ順序を最適化する改良型適応マージソートアルゴリズムです。
/// TimSortよりも優れたパフォーマンスを発揮し、最悪ケースでもO(n log n)、ほぼソート済みデータではO(n)を実現します。
/// </summary>
/// <remarks>
/// <para><strong>Key Innovations over TimSort:</strong></para>
/// <list type="number">
/// <item><description><strong>Power-based Merge Strategy:</strong> Instead of TimSort's invariant-based approach,
/// PowerSort assigns each run a "power" value based on its position and length relative to the total array size.
/// Runs are merged when their power values indicate it's optimal, leading to a more balanced merge tree.
/// The power is calculated as: power = floor(log2(n / runLength)).</description></item>
/// <item><description><strong>Simpler Merge Logic:</strong> PowerSort uses a simpler merge condition (power[i-1] ≤ power[i])
/// compared to TimSort's two invariants. This simplicity leads to fewer edge cases and more predictable behavior.</description></item>
/// <item><description><strong>Provably Optimal Merge Costs:</strong> PowerSort has been proven to achieve optimal merge costs
/// up to lower-order terms, matching the theoretical lower bound for comparison-based sorting.</description></item>
/// </list>
/// <para><strong>Theoretical Conditions for Correct PowerSort:</strong></para>
/// <list type="number">
/// <item><description><strong>Run Detection:</strong> Same as TimSort - identify maximal monotonic sequences (runs).
/// Ascending runs use ≤ comparison for stability, descending runs use &gt; and are reversed.</description></item>
/// <item><description><strong>Run Extension:</strong> Short runs (&lt; MIN_MERGE) are extended using Binary Insertion Sort
/// to ensure efficient merging, maintaining minimum run length of 32 elements.</description></item>
/// <item><description><strong>Power Calculation:</strong> For each run spanning [beginA..endB) in an array of size n,
/// compute power = floor(log2(n / (endB - beginA))). This determines merge priority.</description></item>
/// <item><description><strong>Merge Condition:</strong> Merge adjacent runs when power[i-1] ≤ power[i].
/// This ensures merges happen at the right time to minimize total merge cost.</description></item>
/// <item><description><strong>Stable Merging:</strong> Uses the same galloping merge as TimSort, preserving stability
/// with ≤ comparison when choosing from the left run during merge operations.</description></item>
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
/// <para><strong>Advantages over TimSort:</strong></para>
/// <list type="bullet">
/// <item><description>Simpler algorithm: Single merge condition vs. TimSort's dual invariants</description></item>
/// <item><description>Provably optimal: Achieves theoretical lower bound for merge costs</description></item>
/// <item><description>Better worst-case: More balanced merge tree in pathological cases</description></item>
/// <item><description>Easier to analyze: Power values provide clear merge priority</description></item>
/// <item><description>Same adaptive behavior: Still exploits runs in partially sorted data</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>MIN_MERGE = 32: Minimum run length, same as TimSort</description></item>
/// <item><description>MIN_GALLOP = 7: Threshold for galloping mode during merges</description></item>
/// <item><description>Power calculation: Uses bit manipulation for efficient log2 computation</description></item>
/// <item><description>Merge operations: Reuses TimSort's galloping merge for efficiency</description></item>
/// <item><description>ArrayPool: Uses ArrayPool&lt;T&gt;.Shared to minimize GC pressure</description></item>
/// </list>
/// <para><strong>When to Use PowerSort:</strong></para>
/// <list type="bullet">
/// <item><description>General-purpose stable sorting with guaranteed O(n log n) performance</description></item>
/// <item><description>Data with unknown distribution (may be partially sorted)</description></item>
/// <item><description>When stability is required (e.g., multi-key sorting)</description></item>
/// <item><description>Replacing TimSort for better worst-case guarantees</description></item>
/// <item><description>Large datasets where merge cost optimization matters</description></item>
/// </list>
/// <para><strong>References:</strong></para>
/// <para>Paper: "Nearly-Optimal Mergesort: Fast, Practical Sorting Methods That Optimally Adapt to Existing Runs"
/// by J. Ian Munro and Sebastian Wild (2018)</para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Powersort#cite_note-acmtechnews-2</para>
/// <para>ICALP 2022: https://drops.dagstuhl.de/storage/00lipics/lipics-vol229-icalp2022/LIPIcs.ICALP.2022.68/LIPIcs.ICALP.2022.68.pdf</para>
/// <para>arXiv: Nearly-Optimal Mergesorts: Fast, Practical Sorting Methods That Optimally Adapt to Existing Runs https://arxiv.org/abs/1805.04154</para>
/// </remarks>
public static class PowerSort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Temporary merge buffer

    // PowerSort constants
    private const int MIN_MERGE = 32;        // Minimum sized sequence to merge
    private const int MIN_GALLOP = 7;        // Threshold for entering galloping mode

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

        // For very small arrays, use binary insertion sort directly
        if (n < MIN_MERGE)
        {
            BinaryInsertionSort.Sort(span, first, last, comparer, context);
            return;
        }

        SortCore(span, first, last, comparer, context);
    }

    /// <summary>
    /// Core PowerSort implementation.
    /// Based on the algorithm from the PowerSort paper (Algorithm in Section 5.3).
    /// </summary>
    private static void SortCore<T, TComparer, TContext>(Span<T> span, int first, int last, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = last - first;
        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);

        // PowerSort uses a stack with node-power values to determine merge order
        Span<int> runBase = stackalloc int[85]; // 85 is enough for 2^64 elements
        Span<int> runLen = stackalloc int[85];
        Span<int> power = stackalloc int[85];   // power[i] is the power of node BETWEEN run[i-1] and run[i]
        var stackSize = 0;

        // Find the leftmost run (s1, e1)
        var s1 = first;
        var e1 = FindAndPrepareRun(s, s1, last);

        // Process runs using PowerSort algorithm
        while (e1 < last)
        {
            // Find next run (s2, e2)
            var s2 = e1;
            var e2 = FindAndPrepareRun(s, s2, last);

            // Calculate node power between current run [s1..e1) and next run [s2..e2)
            // Convert to relative positions (0-based from 'first')
            var p = ComputeNodePower(s1 - first, e1 - first, s2 - first, e2 - first, n);

            // Merge while P.top() > p (previous merge deeper in tree than current)
            while (stackSize > 0 && power[stackSize - 1] > p)
            {
                // Pop from stack and merge
                stackSize--;
                var prevBase = runBase[stackSize];
                var prevLen = runLen[stackSize];
                var currentLen = e1 - s1;

                // Merge prevRun [prevBase..prevBase+prevLen) with current run [s1..e1)
                // These two runs are always adjacent (prevBase + prevLen == s1)
                MergeRuns(span, prevBase, prevLen, s1, currentLen, comparer, context);

                // Update s1 to represent the merged result
                s1 = prevBase;
                // e1 stays the same (end of the merged region)
            }

            // Push current run onto stack with its power
            runBase[stackSize] = s1;
            runLen[stackSize] = e1 - s1;
            power[stackSize] = p;
            stackSize++;

            // Move to next run
            s1 = s2;
            e1 = e2;
        }

        // Now [s1..e1) is the rightmost run
        // Merge all remaining runs from the stack with the rightmost run
        while (stackSize > 0)
        {
            stackSize--;
            var prevBase = runBase[stackSize];
            var prevLen = runLen[stackSize];

            var currentLen = e1 - s1;

            // The previous run should be immediately before the current run [s1..e1)
            if (prevBase + prevLen != s1)
            {
                throw new InvalidOperationException($"Runs are not adjacent in final collapse: prev ends at {prevBase + prevLen}, current starts at {s1}");
            }

            MergeRuns(span, prevBase, prevLen, s1, currentLen, comparer, context);

            s1 = prevBase;
            // e1 stays the same
        }
    }

    /// <summary>
    /// Finds and prepares a run starting at position 'start'.
    /// Returns the end position (exclusive) of the prepared run.
    /// A run is either ascending or strictly descending (which is then reversed).
    /// Short runs are extended to MIN_MERGE using binary insertion sort.
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
        if (s.Compare(start, runEnd) > 0)
        {
            // Strictly descending run
            while (runEnd < last && s.Compare(runEnd - 1, runEnd) > 0)
            {
                runEnd++;
            }
            // Reverse the descending run to make it ascending
            Reverse(s, start, runEnd - 1);
        }
        else
        {
            // Ascending run (allowing equals for stability)
            while (runEnd < last && s.Compare(runEnd - 1, runEnd) <= 0)
            {
                runEnd++;
            }
        }

        var runLength = runEnd - start;

        // If run is too small, extend it to MIN_MERGE using binary insertion sort
        if (runLength < MIN_MERGE)
        {
            var force = Math.Min(MIN_MERGE, last - start);
            BinaryInsertionSort.SortCore(s, start, start + force, start + runLength);
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
    /// Computes the node power for PowerSort merge strategy using integer-based fixed-point arithmetic.
    /// This is the key innovation of PowerSort over TimSort - determining optimal merge timing based on
    /// the tree depth where run midpoints diverge.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Based on the PowerSort paper algorithm:
    /// <code>
    /// NodePower(s1, e1, s2, e2, n):
    ///   n1 = e1 - s1 + 1; n2 = e2 - s2 + 1; ℓ = 0
    ///   a = (s1 + n1/2 - 1) / n
    ///   b = (s2 + n2/2 - 1) / n
    ///   while floor(a * 2^ℓ) == floor(b * 2^ℓ) do ℓ = ℓ + 1
    ///   return ℓ
    /// </code>
    /// </para>
    /// The paper uses 1-based indexing with inclusive ranges [s1..e1].
    /// Our implementation uses 0-based indexing with exclusive end [s1..e1).
    /// Adjustment: n1 = e1 - s1 (not +1), formula becomes a = (s1 + n1/2) / n (not -1 for 0-based).
    /// </remarks>
    private static int ComputeNodePower(int s1, int e1, int s2, int e2, int n)
    {
        // Compute run lengths
        uint n1 = (uint)(e1 - s1);
        uint n2 = (uint)(e2 - s2);

        // Represent midpoints in half-unit precision to avoid 0.5 floating-point
        // Original: m = s + len/2.0  →  Transformed: 2*m = 2*s + len
        // Numerator for 2*midpoint (range is approximately < 3n)
        ulong M1 = (ulong)(2u * (uint)s1) + n1;
        ulong M2 = (ulong)(2u * (uint)s2) + n2;

        // Normalization denominator: 2n (to match 2*midpoint scale)
        ulong D = (ulong)(2u * (uint)n);

        // Compute 64-bit fixed-point normalized values: x = floor((M/D) * 2^64)
        // Use UInt128 to prevent overflow during (M << 64) operation (requires .NET 7+)
        ulong x1 = (ulong)(((UInt128)M1 << 64) / D);
        ulong x2 = (ulong)(((UInt128)M2 << 64) / D);

        // XOR reveals first differing bit position
        ulong diff = x1 ^ x2;
        if (diff == 0)
        {
            // Identical at 64-bit resolution → same point, return max practical power
            return 64;
        }

        // Count leading zero bits (common prefix length)
        int commonPrefix = BitOperations.LeadingZeroCount(diff); // Returns 0..64

        // floor(x*2^ℓ) matches while ℓ <= commonPrefix
        // First divergence occurs at ℓ = commonPrefix + 1
        int power = commonPrefix + 1;

        // Cap at 64 for practical use (run stack size ~64 is sufficient)
        if (power > 64) power = 64;
        return power;
    }

    /// <summary>
    /// Merges two adjacent runs with galloping mode optimization.
    /// </summary>
    private static void MergeRuns<T, TComparer, TContext>(Span<T> span, int base1, int len1, int base2, int len2, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        var ms = new MergeState();

        // Optimize: Find where first element of run2 goes in run1
        // Elements before this point are already in their final positions
        var k = GallopRight(s, s.Read(base2), base1, len1, 0);
        base1 += k;
        len1 -= k;
        if (len1 == 0) return;

        // Optimize: Find where last element of run1 goes in run2
        // Elements after this point are already in their final positions
        len2 = GallopLeft(s, s.Read(base1 + len1 - 1), base2, len2, len2 - 1);
        if (len2 == 0) return;

        // Merge remaining runs using galloping
        if (len1 <= len2)
        {
            MergeLow(span, base1, len1, base2, len2, ref ms, comparer, context);
        }
        else
        {
            MergeHigh(span, base1, len1, base2, len2, ref ms, comparer, context);
        }
    }

    /// <summary>
    /// Locate the position at which to insert key in a sorted range.
    /// Returns the index k such that all elements in [base..base+k) are less than key,
    /// and all elements in [base+k..base+len) are greater than or equal to key.
    /// Uses galloping (exponential search followed by binary search).
    /// </summary>
    private static int GallopLeft<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, T key, int baseIdx, int len, int hint)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var lastOfs = 0;
        var ofs = 1;
        var p = baseIdx + hint;

        if (s.Compare(key, p) > 0)
        {
            // Gallop right until s[base + hint + lastOfs] < key <= s[base + hint + ofs]
            var maxOfs = len - hint;
            while (ofs < maxOfs && s.Compare(key, p + ofs) > 0)
            {
                lastOfs = ofs;
                ofs = (ofs << 1) + 1;
                if (ofs <= 0) // Overflow
                {
                    ofs = maxOfs;
                }
            }
            if (ofs > maxOfs)
            {
                ofs = maxOfs;
            }

            lastOfs += hint;
            ofs += hint;
        }
        else
        {
            // Gallop left until s[base + hint - ofs] < key <= s[base + hint - lastOfs]
            var maxOfs = hint + 1;
            while (ofs < maxOfs && s.Compare(key, p - ofs) <= 0)
            {
                lastOfs = ofs;
                ofs = (ofs << 1) + 1;
                if (ofs <= 0) // Overflow
                {
                    ofs = maxOfs;
                }
            }
            if (ofs > maxOfs)
            {
                ofs = maxOfs;
            }

            var tmp = lastOfs;
            lastOfs = hint - ofs;
            ofs = hint - tmp;
        }

        // Binary search in [base + lastOfs, base + ofs)
        lastOfs++;
        while (lastOfs < ofs)
        {
            var m = lastOfs + ((ofs - lastOfs) >> 1);
            if (s.Compare(key, baseIdx + m) > 0)
            {
                lastOfs = m + 1;
            }
            else
            {
                ofs = m;
            }
        }
        return ofs;
    }

    /// <summary>
    /// Locate the position at which to insert key in a sorted range.
    /// Returns the index k such that all elements in [base..base+k) are less than or equal to key,
    /// and all elements in [base+k..base+len) are greater than key.
    /// Uses galloping (exponential search followed by binary search).
    /// </summary>
    private static int GallopRight<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, T key, int baseIdx, int len, int hint)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var lastOfs = 0;
        var ofs = 1;
        var p = baseIdx + hint;

        if (s.Compare(key, p) < 0)
        {
            // Gallop left until s[base + hint - ofs] <= key < s[base + hint - lastOfs]
            var maxOfs = hint + 1;
            while (ofs < maxOfs && s.Compare(key, p - ofs) < 0)
            {
                lastOfs = ofs;
                ofs = (ofs << 1) + 1;
                if (ofs <= 0) // Overflow
                {
                    ofs = maxOfs;
                }
            }
            if (ofs > maxOfs)
            {
                ofs = maxOfs;
            }

            var tmp = lastOfs;
            lastOfs = hint - ofs;
            ofs = hint - tmp;
        }
        else
        {
            // Gallop right until s[base + hint + lastOfs] <= key < s[base + hint + ofs]
            var maxOfs = len - hint;
            while (ofs < maxOfs && s.Compare(key, p + ofs) >= 0)
            {
                lastOfs = ofs;
                ofs = (ofs << 1) + 1;
                if (ofs <= 0) // Overflow
                {
                    ofs = maxOfs;
                }
            }
            if (ofs > maxOfs)
            {
                ofs = maxOfs;
            }

            lastOfs += hint;
            ofs += hint;
        }

        // Binary search in [base + lastOfs, base + ofs)
        lastOfs++;
        while (lastOfs < ofs)
        {
            var m = lastOfs + ((ofs - lastOfs) >> 1);
            if (s.Compare(key, baseIdx + m) >= 0)
            {
                lastOfs = m + 1;
            }
            else
            {
                ofs = m;
            }
        }
        return ofs;
    }

    /// <summary>
    /// Merges two adjacent runs where the first run is smaller or equal.
    /// Uses galloping mode when one run consistently wins.
    /// </summary>
    private static void MergeLow<T, TComparer, TContext>(Span<T> span, int base1, int len1, int base2, int len2, ref MergeState ms, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);

        // Rent temp array from ArrayPool
        var tmp = ArrayPool<T>.Shared.Rent(len1);
        try
        {
            var t = new SortSpan<T, TComparer, TContext>(tmp.AsSpan(0, len1), context, comparer, BUFFER_TEMP);
            s.CopyTo(base1, t, 0, len1);

            var cursor1 = 0;          // Index in temp (first run)
            var cursor2 = base2;      // Index in span (second run)
            var dest = base1;         // Destination index

            // Move first element of second run
            s.Write(dest++, s.Read(cursor2++));
            len2--;

            if (len2 == 0)
            {
                t.CopyTo(0, s, dest, len1);
                return;
            }
            if (len1 == 1)
            {
                s.CopyTo(cursor2, s, dest, len2);
                s.Write(dest + len2, t.Read(cursor1));
                return;
            }

            var minGallop = ms.MinGallop;

            while (true)
            {
                var count1 = 0;  // # of times run1 won in a row
                var count2 = 0;  // # of times run2 won in a row

                // One-pair-at-a-time mode
                do
                {
                    var val1 = t.Read(cursor1);
                    var val2 = s.Read(cursor2);

                    if (s.Compare(val1, val2) <= 0)
                    {
                        s.Write(dest++, val1);
                        cursor1++;
                        count1++;
                        count2 = 0;
                        len1--;
                        if (len1 == 0)
                        {
                            goto exitMerge;
                        }
                    }
                    else
                    {
                        s.Write(dest++, val2);
                        cursor2++;
                        count2++;
                        count1 = 0;
                        len2--;
                        if (len2 == 0)
                        {
                            goto exitMerge;
                        }
                    }
                } while ((count1 | count2) < minGallop);

                // Galloping mode: one run is winning consistently
                do
                {
                    count1 = GallopRight(t, s.Read(cursor2), cursor1, len1, 0);
                    if (count1 != 0)
                    {
                        t.CopyTo(cursor1, s, dest, count1);
                        dest += count1;
                        cursor1 += count1;
                        len1 -= count1;
                        if (len1 == 0)
                        {
                            goto exitMerge;
                        }
                    }
                    s.Write(dest++, s.Read(cursor2++));
                    len2--;
                    if (len2 == 0)
                    {
                        goto exitMerge;
                    }

                    count2 = GallopLeft(s, t.Read(cursor1), cursor2, len2, 0);
                    if (count2 != 0)
                    {
                        s.CopyTo(cursor2, s, dest, count2);
                        dest += count2;
                        cursor2 += count2;
                        len2 -= count2;
                        if (len2 == 0)
                        {
                            goto exitMerge;
                        }
                    }
                    s.Write(dest++, t.Read(cursor1++));
                    len1--;
                    if (len1 == 1)
                    {
                        goto exitMerge;
                    }

                    minGallop--;
                } while (count1 >= MIN_GALLOP || count2 >= MIN_GALLOP);

                if (minGallop < 0)
                {
                    minGallop = 0;
                }
                minGallop += 2;  // Penalize for leaving galloping mode
            }

            exitMerge:
            ms.MinGallop = minGallop < 1 ? 1 : minGallop;

            if (len1 == 1)
            {
                s.CopyTo(cursor2, s, dest, len2);
                s.Write(dest + len2, t.Read(cursor1));
            }
            else if (len1 > 0)
            {
                t.CopyTo(cursor1, s, dest, len1);
            }
        }
        finally
        {
            // Return the rented array to the pool
            ArrayPool<T>.Shared.Return(tmp, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    /// <summary>
    /// Merges two adjacent runs where the second run is smaller.
    /// Uses galloping mode when one run consistently wins.
    /// </summary>
    private static void MergeHigh<T, TComparer, TContext>(Span<T> span, int base1, int len1, int base2, int len2, ref MergeState ms, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);

        // Rent temp array from ArrayPool
        var tmp = ArrayPool<T>.Shared.Rent(len2);
        try
        {
            var t = new SortSpan<T, TComparer, TContext>(tmp.AsSpan(0, len2), context, comparer, BUFFER_TEMP);
            s.CopyTo(base2, t, 0, len2);

            var cursor1 = base1 + len1 - 1;  // Index in span (first run, from end)
            var cursor2 = len2 - 1;          // Index in temp (second run, from end)
            var dest = base2 + len2 - 1;     // Destination index (from end)

            // Move last element of first run
            s.Write(dest--, s.Read(cursor1--));
            len1--;

            if (len1 == 0)
            {
                t.CopyTo(0, s, dest - (len2 - 1), len2);
                return;
            }
            if (len2 == 1)
            {
                dest -= len1;
                cursor1 -= len1;
                s.CopyTo(cursor1 + 1, s, dest + 1, len1);
                s.Write(dest, t.Read(0));
                return;
            }

            var minGallop = ms.MinGallop;

            while (true)
            {
                var count1 = 0;  // # of times run1 won in a row
                var count2 = 0;  // # of times run2 won in a row

                // One-pair-at-a-time mode
                do
                {
                    var val1 = s.Read(cursor1);
                    var val2 = t.Read(cursor2);

                    if (s.Compare(val2, val1) >= 0)
                    {
                        s.Write(dest--, val2);
                        cursor2--;
                        count2++;
                        count1 = 0;
                        len2--;
                        if (len2 == 0)
                        {
                            goto exitMerge;
                        }
                    }
                    else
                    {
                        s.Write(dest--, val1);
                        cursor1--;
                        count1++;
                        count2 = 0;
                        len1--;
                        if (len1 == 0)
                        {
                            goto exitMerge;
                        }
                    }
                } while ((count1 | count2) < minGallop);

                // Galloping mode: one run is winning consistently
                do
                {
                    count1 = len1 - GallopRight(s, t.Read(cursor2), base1, len1, len1 - 1);
                    if (count1 != 0)
                    {
                        dest -= count1;
                        cursor1 -= count1;
                        len1 -= count1;
                        s.CopyTo(cursor1 + 1, s, dest + 1, count1);
                        if (len1 == 0)
                        {
                            goto exitMerge;
                        }
                    }
                    s.Write(dest--, t.Read(cursor2--));
                    len2--;
                    if (len2 == 1)
                    {
                        goto exitMerge;
                    }

                    count2 = len2 - GallopLeft(t, s.Read(cursor1), 0, len2, len2 - 1);
                    if (count2 != 0)
                    {
                        dest -= count2;
                        cursor2 -= count2;
                        len2 -= count2;
                        t.CopyTo(cursor2 + 1, s, dest + 1, count2);
                        if (len2 == 0)
                        {
                            goto exitMerge;
                        }
                    }
                    s.Write(dest--, s.Read(cursor1--));
                    len1--;
                    if (len1 == 0)
                    {
                        goto exitMerge;
                    }

                    minGallop--;
                } while (count1 >= MIN_GALLOP || count2 >= MIN_GALLOP);

                if (minGallop < 0)
                {
                    minGallop = 0;
                }
                minGallop += 2;  // Penalize for leaving galloping mode
            }

            exitMerge:
            ms.MinGallop = minGallop < 1 ? 1 : minGallop;

            if (len2 == 1)
            {
                dest -= len1;
                cursor1 -= len1;
                s.CopyTo(cursor1 + 1, s, dest + 1, len1);
                s.Write(dest, t.Read(cursor2));
            }
            else if (len2 > 0)
            {
                t.CopyTo(0, s, dest - (len2 - 1), len2);
            }
        }
        finally
        {
            // Return the rented array to the pool
            ArrayPool<T>.Shared.Return(tmp, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    /// <summary>
    /// Merge state structure to track galloping threshold dynamically.
    /// </summary>
    private ref struct MergeState
    {
        public int MinGallop;

        public MergeState()
        {
            MinGallop = MIN_GALLOP;
        }
    }
}
