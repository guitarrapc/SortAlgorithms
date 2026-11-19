using System.Buffers;
using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 入力配列中の自然に発生する昇順（非減少）および厳密降順の連続部分列（ラン）を検出し、降順ランを反転して昇順に変換した後、
/// 隣接ランを反復的にマージしてソートする適応型マージソートです。
/// データが部分的にソートされている場合に効率的であり、すでにソート済みまたは完全逆順のデータに対しては最良ケースO(n)で動作します。
/// 安定ソートです。
/// <br/>
/// An adaptive merge sort that detects naturally occurring ascending (non-decreasing) and strictly descending runs in the input,
/// reverses descending runs in-place to make them ascending, then iteratively merges adjacent pairs of runs until the entire array is sorted.
/// Efficient on partially sorted data, achieving O(n) best case on already-sorted or fully-reversed input.
/// Stable sort.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Natural Merge Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Run Detection:</strong> Scan the array once to identify maximal runs.
/// Ascending runs are non-decreasing subsequences where s[i] &lt;= s[i+1].
/// Strictly descending runs (s[i] &gt; s[i+1]) are detected and reversed in-place.
/// Using strict descent for detection preserves stability when reversing.</description></item>
/// <item><description><strong>Pairwise Merging:</strong> In each pass, merge adjacent pairs of runs, as bottom-up merge sort does
/// with runs of one element. If an odd number of runs exists, the last run is carried forward unmerged.</description></item>
/// <item><description><strong>Termination:</strong> Repeat passes until only a single run remains, covering the entire array.</description></item>
/// <item><description><strong>Stability Preservation:</strong> During merge, when elements from both runs are equal,
/// the element from the left run is placed first, preserving relative order.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Merge</description></item>
/// <item><description>Stable      : Yes (equal elements maintain relative order: the left run's element is placed first)</description></item>
/// <item><description>In-place    : No (requires O(n) auxiliary space for merging)</description></item>
/// <item><description>Best case   : O(n) - Already sorted or fully reversed data is a single run; only one detection scan needed</description></item>
/// <item><description>Average case: O(n log n) - Random data has ~n/2 runs, requiring ~log₂(n) merge passes</description></item>
/// <item><description>Worst case  : O(n log n) - Alternating up/down data produces many small runs, requiring full merge passes</description></item>
/// <item><description>Comparisons : Θ(n) best, O(n log r) for r natural runs, O(n log n) worst - run detection costs n-1 comparisons, and a merge pass costs at most n, counting the in-order check before each merge</description></item>
/// <item><description>Swaps       : O(n) - only the reversal that turns a detected descending run ascending; merging moves elements by copying</description></item>
/// <item><description>Index Reads : O(n log r) - a merge reads each element of its two runs a bounded number of times; Θ(n) when the input is a single run</description></item>
/// <item><description>Index Writes: O(n log r) - a merge writes its shorter run into the buffer and every element it moves into place; O(n) when the input is a single run</description></item>
/// <item><description>Space       : O(n) - the buffer holds the shorter run of a merge, at most n/2 elements, and the run boundaries take at most ⌈n/2⌉ + 1 ints (both rented from ArrayPool)</description></item>
/// </list>
/// <para><strong>Implementation note – recorded run boundaries:</strong></para>
/// <para>Tape-era natural merging finds the runs again on every pass, at n - 1 comparisons a pass. This implementation scans
/// once and keeps the boundaries, so each pass merges the runs the previous pass produced. A merge can leave two runs whose
/// boundary is already in order; one comparison before each merge finds that and skips the merge.</para>
/// <para><strong>Implementation note – shorter-run buffering:</strong></para>
/// <para>Each merge copies only the shorter of its two runs into the buffer, as Rust's slice merge and libstdc++'s
/// __merge_adaptive do. A buffered left run is merged front to back and a buffered right run back to front, and whatever is
/// left of the run that stayed in place is already in its final position.</para>
/// <para><strong>Advantages over Standard Merge Sort:</strong></para>
/// <list type="bullet">
/// <item><description>Adaptive - O(n) on already-sorted or fully-reversed data (standard merge sort is always O(n log n))</description></item>
/// <item><description>Non-recursive - Iterative bottom-up approach avoids stack overflow on large inputs</description></item>
/// <item><description>Fewer merges - Exploits pre-existing order, skipping unnecessary work</description></item>
/// </list>
/// <para><strong>Comparer contract:</strong></para>
/// <para>Every merge pass halves the number of recorded runs whatever the comparer answers, so the sort ends after ⌈log₂ r⌉ passes.
/// The range keeps its elements, and nothing outside it is read or written.</para>
/// <para><strong>History:</strong></para>
/// <para>Natural merging was in production use before Knuth's TAOCP (1973), which is often cited for it. Marilyn M. Jensen's
/// IBM "A Survey of Sorting Techniques" (1963) describes "the von Neumann, or natural, merge", which "takes full advantage of
/// all ordering existing" in the input, and notes that Sort 90 for the IBM 7070 uses it. 1963 is a lower bound, not a proven
/// origin.</para>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Merge_sort#Natural_merge_sort</para>
/// <para>Jensen, M. M. (1963). "A Survey of Sorting Techniques". IBM Z77-3013. https://cassini.mirrorservice.org/sites/www.bitsavers.org/pdf/ibm/technical_information_exchange/Z77-3013_Jensen_-_A_Survey_of_Sorting_Techniques_19630718.pdf</para>
/// </remarks>
public static class NaturalMergeSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by NaturalMergeSortTests, which derives from StableSortTestsBase.</remarks>
    public static bool IsStable => true;

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_MERGE = 1;      // Merge buffer (auxiliary space)

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

        // A merge buffers the shorter of its two runs, which never exceeds half the span.
        var bufferLength = span.Length / 2;
        var buffer = ArrayPool<T>.Shared.Rent(bufferLength);
        // Every run but the last holds at least two elements, so there are at most ceil(n/2) runs, plus the sentinel.
        var runsBuffer = ArrayPool<int>.Shared.Rent((span.Length + 1) / 2 + 1);
        try
        {
            var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
            var b = new SortSpan<T, TComparer, TContext>(buffer.AsSpan(0, bufferLength), context, comparer, BUFFER_MERGE);
            SortCore(s, b, runsBuffer.AsSpan());
        }
        finally
        {
            ArrayPool<int>.Shared.Return(runsBuffer);
            ArrayPool<T>.Shared.Return(buffer, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    /// <summary>
    /// Core iterative natural merge sort implementation.
    /// Detects the natural runs once, then merges adjacent pairs of runs pass by pass, using the recorded boundaries,
    /// until a single run remains.
    /// </summary>
    /// <param name="s">The SortSpan wrapping the span to sort</param>
    /// <param name="b">The SortSpan wrapping the auxiliary buffer for merging</param>
    /// <param name="runs">Buffer to store run boundary indices. runs[i] is the start index of run i; runs[runCount] = length (sentinel).</param>
    private static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> b, Span<int> runs)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var length = s.Length;

        s.Context.OnPhase(SortPhase.MergeRunDetect);
        var runCount = DetectNaturalRuns(s, runs, length);

        // Each pass merges run 2j with run 2j+1 and keeps the start of run 2j as the boundary of the merged run.
        // An odd run at the end is carried forward unmerged. The count halves every pass whatever the comparer answers.
        while (runCount > 1)
        {
            var merged = 0;
            for (var i = 0; i < runCount; i += 2)
            {
                var left = runs[i];
                // merged <= i, so this never overwrites a boundary the pass has not read yet.
                runs[merged++] = left;
                if (i + 1 == runCount) break;

                var mid = runs[i + 1];   // start of the right run
                var right = runs[i + 2]; // exclusive end of the right run

                // Every pair the pass joins is announced, including one the check below finds already in order,
                // so a consumer that tracks runs sees each join.
                s.Context.OnPhase(SortPhase.MergeSortMerge, left, mid - 1, right - 1);
                s.Context.OnRole(left, BUFFER_MAIN, RoleType.LeftPointer);
                s.Context.OnRole(mid, BUFFER_MAIN, RoleType.RightPointer);
                // Two runs whose boundary is in order are already one run. A reversed descending run can leave such a pair, and so can a merge.
                if (!s.IsLessOrEqualAt(mid - 1, mid))
                {
                    Merge(s, b, left, mid, right);
                }
                s.Context.OnRole(left, BUFFER_MAIN, RoleType.None);
                s.Context.OnRole(mid, BUFFER_MAIN, RoleType.None);
            }
            runs[merged] = length;
            runCount = merged;
        }
    }

    /// <summary>
    /// Scans the array and records the start index of each natural run.
    /// Detects both ascending (non-decreasing) and strictly descending runs.
    /// Descending runs are reversed in-place to become ascending, preserving stability.
    /// </summary>
    /// <param name="s">The SortSpan wrapping the array to scan</param>
    /// <param name="runs">Output buffer. runs[i] = start of run i; runs[runCount] = length (sentinel).</param>
    /// <param name="length">The number of elements in the array</param>
    /// <returns>The number of natural runs detected.</returns>
    private static int DetectNaturalRuns<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, Span<int> runs, int length)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var runCount = 0;
        var i = 0;

        while (i < length)
        {
            runs[runCount] = i;
            var runEnd = i + 1;

            if (runEnd < length)
            {
                // The first pair decides the direction and already belongs to the run, so each scan continues after it.
                // Each branch steps past it on its own: incrementing inside the comparison's argument added a register copy
                // to the scan loop (verified by disassembly) and measured slower on sorted input.
                if (s.IsGreaterAt(i, runEnd))
                {
                    // Strictly descending run
                    runEnd++;
                    while (runEnd < length && s.IsGreaterAt(runEnd - 1, runEnd))
                    {
                        runEnd++;
                    }
                    // Reverse to ascending.
                    // Because the run is strictly descending, it contains no equal adjacent elements, so reversing does not violate stability.
                    ReverseRun(s, i, runEnd - 1);
                }
                else
                {
                    // Ascending run (non-decreasing, allowing equals for stability)
                    runEnd++;
                    while (runEnd < length && s.IsLessOrEqualAt(runEnd - 1, runEnd))
                    {
                        runEnd++;
                    }
                }
            }

            runCount++;
            i = runEnd;
        }

        // Close with sentinel
        runs[runCount] = length;

        return runCount;
    }

    /// <summary>
    /// Reverses the elements between indices <paramref name="lo"/> and <paramref name="hi"/> (inclusive) in-place.
    /// Used to convert a strictly descending run into ascending order.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ReverseRun<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int lo, int hi)
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
    /// Merges the sorted runs [left, mid) and [mid, right), both non-empty, buffering only the shorter one.
    /// A buffered left run is merged front to back and a buffered right run back to front, so the run that stays
    /// in place is always read before its slots are overwritten.
    /// </summary>
    /// <param name="s">The SortSpan wrapping the main array</param>
    /// <param name="b">The SortSpan wrapping the auxiliary buffer</param>
    /// <param name="left">The inclusive start index of the left run</param>
    /// <param name="mid">The inclusive start index of the right run</param>
    /// <param name="right">The exclusive end index of the right run</param>
    private static void Merge<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> b, int left, int mid, int right)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var leftLength = mid - left;
        var rightLength = right - mid;

        if (leftLength <= rightLength)
        {
            s.CopyTo(left, b, 0, leftLength);

            var l = 0;      // next element of the buffered left run
            var r = mid;    // next element of the right run, still in place
            var k = left;   // next output slot
            var leftValue = b.Read(l);
            var rightValue = s.Read(r);
            while (true)
            {
                // Stability: take the left element when equal
                if (s.IsLessOrEqual(leftValue, rightValue))
                {
                    s.Write(k++, leftValue);
                    // The rest of the right run is already in place
                    if (++l == leftLength) return;
                    leftValue = b.Read(l);
                }
                else
                {
                    s.Write(k++, rightValue);
                    if (++r == right) break;
                    rightValue = s.Read(r);
                }
            }
            b.CopyTo(l, s, k, leftLength - l);
        }
        else
        {
            s.CopyTo(mid, b, 0, rightLength);

            var l = mid - 1;          // last element of the left run, still in place
            var r = rightLength - 1;  // last element of the buffered right run
            var k = right - 1;        // next output slot, filled from the back
            var leftValue = s.Read(l);
            var rightValue = b.Read(r);
            while (true)
            {
                // Stability: place the right element at the back when equal
                if (s.IsLessOrEqual(leftValue, rightValue))
                {
                    s.Write(k--, rightValue);
                    // The rest of the left run is already in place
                    if (--r < 0) return;
                    rightValue = b.Read(r);
                }
                else
                {
                    s.Write(k--, leftValue);
                    if (--l < left) break;
                    leftValue = s.Read(l);
                }
            }
            b.CopyTo(0, s, left, r + 1);
        }
    }
}
