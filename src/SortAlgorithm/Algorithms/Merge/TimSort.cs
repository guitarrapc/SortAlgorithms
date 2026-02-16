using SortAlgorithm.Contexts;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 配列を自然なランに分割し、それらを適応的にマージする高度な安定ソートアルゴリズムです。
/// 部分的にソートされたデータに対して優れた性能を発揮し、最悪でもO(n log n)を保証します。
/// <br/>
/// An adaptive, stable sorting algorithm that identifies natural runs in the data and merges them intelligently.
/// Excels on partially sorted data while guaranteeing O(n log n) worst-case performance.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct TimSort:</strong></para>
/// <list type="number">
/// <item><description><strong>Run Detection:</strong> The algorithm must identify maximal monotonic sequences (runs).
/// An ascending run is a sequence where each element is ≥ the previous (a[i] ≤ a[i+1]).
/// A strictly descending run is a sequence where each element is &gt; the previous (a[i] &gt; a[i+1]).
/// This implementation correctly distinguishes between ascending (using ≤ for stability) and strictly descending (using &gt;) runs.</description></item>
/// <item><description><strong>Descending Run Reversal:</strong> Strictly descending runs must be reversed in-place to convert them to ascending runs.
/// The reversal is done by swapping elements from both ends moving towards the center: swap(a[lo], a[hi]), lo++, hi--.
/// This maintains stability because the original relative order of equal elements is preserved within the run.</description></item>
/// <item><description><strong>MinRun Calculation:</strong> The minimum run length (minRun) must be computed to balance run merging efficiency.
/// For array size n, minRun is calculated by taking the top 6 bits of n and adding 1 if any of the remaining bits are set.
/// This ensures: 32 ≤ minRun ≤ 64 for large n, and n/minRun is close to or slightly less than a power of 2.
/// Formula: while n ≥ 64: r |= (n &amp; 1), n >>= 1; return n + r.
/// This guarantees balanced merge tree depth and O(n log n) worst-case performance.</description></item>
/// <item><description><strong>Run Extension:</strong> If a natural run is shorter than minRun, it must be extended to minRun length using Binary Insertion Sort.
/// The already-sorted portion of the run is used as a starting point, and remaining elements are inserted using binary search.
/// This reduces comparisons to O(k log k) for extending a run of length k, while maintaining stability.</description></item>
/// <item><description><strong>Run Stack Invariants:</strong> The stack of pending runs must maintain two invariants at all times (except during final collapse):
/// (A) runLen[i-1] &gt; runLen[i] + runLen[i+1] - Ensures roughly balanced merges
/// (B) runLen[i] &gt; runLen[i+1] - Ensures decreasing run lengths
/// When an invariant is violated, runs must be merged immediately. If both runLen[i-1] and runLen[i+1] exist,
/// merge the smaller of the two with runLen[i] to minimize work. These invariants guarantee O(log n) stack depth.</description></item>
/// <item><description><strong>Stable Merging:</strong> When merging two adjacent runs, the algorithm must preserve the relative order of equal elements.
/// This is achieved by using ≤ comparison when choosing from the left run: if left[i] ≤ right[j], take left[i].
/// The smaller run is copied to a temporary buffer, and elements are merged back into the original array.
/// This ensures that equal elements from the left run appear before equal elements from the right run.</description></item>
/// <item><description><strong>Stack Collapse:</strong> After all runs are identified, the remaining runs on the stack must be merged.
/// The final collapse merges runs in a specific order to maintain efficiency: always merge the smaller of runLen[i-1] and runLen[i+1] with runLen[i].
/// This continues until only one run remains, which is the fully sorted array.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Hybrid (Merge + Insertion)</description></item>
/// <item><description>Stable      : Yes (≤ comparison in ascending run detection and merging preserves relative order)</description></item>
/// <item><description>In-place    : No (requires O(n/2) temporary space worst-case for merging)</description></item>
/// <item><description>Best case   : O(n) - Already sorted or reverse sorted data (single run detected, n-1 comparisons)</description></item>
/// <item><description>Average case: O(n log n) - Balanced run detection and merge tree with adaptive behavior</description></item>
/// <item><description>Worst case  : O(n log n) - Guaranteed by minRun calculation (ensures balanced merges) and stack invariants (ensures O(log n) depth)</description></item>
/// <item><description>Comparisons : Best O(n), Average/Worst O(n log n) - Exploits existing order in data</description></item>
/// <item><description>Writes      : Best O(1), Average/Worst O(n log n) - Minimal writes for sorted data due to run detection</description></item>
/// <item><description>Space       : O(n/2) worst-case for temporary merge buffer (smaller run is copied)</description></item>
/// </list>
/// <para><strong>Adaptive Behavior:</strong></para>
/// <list type="bullet">
/// <item><description>Sorted data: O(n) - Detected as single ascending run, no merges needed</description></item>
/// <item><description>Reverse sorted: O(n) - Detected as single descending run, reversed in O(n) with n/2 swaps</description></item>
/// <item><description>Partially sorted: O(n) to O(n log n) - Exploits existing runs, fewer merges needed</description></item>
/// <item><description>Random data: O(n log n) - Falls back to efficient merge sort with run-based optimization</description></item>
/// </list>
/// <para><strong>Why TimSort is Superior for Real-World Data:</strong></para>
/// <list type="bullet">
/// <item><description>Exploits existing order: Natural runs are identified and preserved, reducing work on partially sorted data</description></item>
/// <item><description>Stable sorting: Critical for multi-key sorts (e.g., sort by age, then by name) and maintaining data integrity</description></item>
/// <item><description>Predictable performance: O(n log n) guarantee prevents worst-case scenarios unlike QuickSort</description></item>
/// <item><description>Memory efficient: Temporary buffer size is at most n/2 (smaller run is copied)</description></item>
/// <item><description>Balanced merges: MinRun calculation and stack invariants ensure logarithmic merge tree depth</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>MIN_MERGE = 32: Arrays smaller than 32 elements use Binary Insertion Sort directly (O(n²) but fast for small n)</description></item>
/// <item><description>MIN_GALLOP = 7: Threshold for entering galloping mode during merges</description></item>
/// <item><description>Stack size = 85: Sufficient for 2^64 elements (worst-case stack depth is ⌈log_φ(n)⌉ where φ ≈ 1.618 is golden ratio)</description></item>
/// <item><description>Galloping mode: Adaptive exponential search + binary search when one run consistently wins, dynamically adjusted threshold</description></item>
/// <item><description>Range reduction: Before merging, skip elements already in final positions using galloping</description></item>
/// <item><description>ArrayPool: Uses ArrayPool&lt;T&gt;.Shared to rent temporary buffers, reducing GC pressure and allocation overhead</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Timsort</para>
/// <para>Original description: https://github.com/python/cpython/blob/v3.4.10/Objects/listsort.txt</para>
/// <para>Arxiv: Tight Universal Bounds for Partially Presorted Pareto Front and Convex Hull https://arxiv.org/abs/2512.06559</para>
/// <para>Arxiv: On the Worst-Case Complexity of TimSort https://arxiv.org/abs/1805.08612</para>
/// <para>YouTube: https://www.youtube.com/watch?v=exbuZQpWkQ0 (Efficient Algorithms COMP526 (Fall 2023))</para>
/// </remarks>
public static class TimSort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Temporary merge buffer

    // TimSort constants
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
    /// Core TimSort implementation.
    /// </summary>
    private static void SortCore<T, TComparer, TContext>(Span<T> span, int first, int last, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var n = last - first;
        var minRun = ComputeMinRun(n);
        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);

        // Stack to track runs (start position and length)
        Span<int> runBase = stackalloc int[85]; // 85 is enough for 2^64 elements
        Span<int> runLen = stackalloc int[85];
        var stackSize = 0;

        var i = first;
        while (i < last)
        {
            // Find next run (either ascending or strictly descending)
            var runEnd = i + 1;
            if (runEnd < last)
            {
                // Check if descending
                if (s.Compare(i, runEnd) > 0)
                {
                    // Strictly descending run
                    while (runEnd < last && s.Compare(runEnd - 1, runEnd) > 0)
                    {
                        runEnd++;
                    }
                    // Reverse the descending run to make it ascending
                    Reverse(s, i, runEnd - 1);
                }
                else
                {
                    // Ascending run (allowing equals for stability)
                    while (runEnd < last && s.Compare(runEnd - 1, runEnd) <= 0)
                    {
                        runEnd++;
                    }
                }
            }

            var runLength = runEnd - i;

            // If run is too small, extend it to minRun using binary insertion sort
            if (runLength < minRun)
            {
                var force = Math.Min(minRun, last - i);
                BinaryInsertionSort.SortCore(s, i, i + force, i + runLength);
                runEnd = i + force;
                runLength = force;
            }

            // Push run onto stack
            runBase[stackSize] = i;
            runLen[stackSize] = runLength;
            stackSize++;

            // Merge runs to maintain invariants
            MergeCollapse(span, runBase, runLen, ref stackSize, comparer, context);

            i = runEnd;
        }

        // Force merge all remaining runs
        MergeForceCollapse(span, runBase, runLen, ref stackSize, comparer, context);
    }

    /// <summary>
    /// Computes the minimum run length for the given array size.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]

    private static int ComputeMinRun(int n)
    {
        var r = 0;
        while (n >= MIN_MERGE)
        {
            r |= n & 1;
            n >>= 1;
        }
        return n + r;
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
    /// Maintains the run stack invariants by merging runs when necessary.
    /// </summary>
    private static void MergeCollapse<T, TComparer, TContext>(Span<T> span, Span<int> runBase, Span<int> runLen, ref int stackSize, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (stackSize > 1)
        {
            var n = stackSize - 2;

            // Check invariants:
            // 1. runLen[i-1] > runLen[i] + runLen[i+1]
            // 2. runLen[i] > runLen[i+1]
            if (n > 0 && runLen[n - 1] <= runLen[n] + runLen[n + 1])
            {
                // Merge the smaller of the two runs with X
                if (runLen[n - 1] < runLen[n + 1])
                {
                    n--;
                }
                MergeAt(span, runBase, runLen, ref stackSize, n, comparer, context);
            }
            else if (runLen[n] <= runLen[n + 1])
            {
                MergeAt(span, runBase, runLen, ref stackSize, n, comparer, context);
            }
            else
            {
                break;
            }
        }
    }

    /// <summary>
    /// Merges all runs on the stack until only one remains.
    /// </summary>
    private static void MergeForceCollapse<T, TComparer, TContext>(Span<T> span, Span<int> runBase, Span<int> runLen, ref int stackSize, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (stackSize > 1)
        {
            var n = stackSize - 2;
            if (n > 0 && runLen[n - 1] < runLen[n + 1])
            {
                n--;
            }
            MergeAt(span, runBase, runLen, ref stackSize, n, comparer, context);
        }
    }

    /// <summary>
    /// Merges the run at stack position i with the run at position i+1.
    /// </summary>
    private static void MergeAt<T, TComparer, TContext>(Span<T> span, Span<int> runBase, Span<int> runLen, ref int stackSize, int i, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var base1 = runBase[i];
        var len1 = runLen[i];
        var base2 = runBase[i + 1];
        var len2 = runLen[i + 1];

        // Merge runs
        MergeRuns(span, base1, len1, base2, len2, comparer, context);

        // Update stack
        runLen[i] = len1 + len2;
        if (i == stackSize - 3)
        {
            runBase[i + 1] = runBase[i + 2];
            runLen[i + 1] = runLen[i + 2];
        }
        stackSize--;
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

            if (len2 == 0)
            {
                // Run2 is exhausted, copy remaining run1 from temp
                t.CopyTo(cursor1, s, dest, len1);
            }
            // else: len1 == 0, run2 is already in correct position
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
                    if (len2 == 0)
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

            if (len1 == 0)
            {
                // Run1 is exhausted, copy remaining run2 from temp
                t.CopyTo(0, s, dest - (len2 - 1), len2);
            }
            // else: len2 == 0, run1 is already in correct position
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
