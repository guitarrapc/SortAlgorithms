using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// SymMerge Sortは、SymMergeアルゴリズムを用いたイテレーティブな（ボトムアップ）安定インプレースソートです。
/// RotateMergeSortIterativeと同じボトムアップ構造ですが、マージステップにRotateではなくSymMergeを使用します。
/// SymMergeは各マージで対称二分探索により最適な分割点を1回見つけ、1回のローテーションと2つの再帰呼び出しでマージします。
/// これによりRotateMergeに比べて比較回数がO(n log² n)からO(n log n)に削減されます。
/// <br/>
/// SymMerge Sort is an iterative (bottom-up) stable in-place sort using the SymMerge algorithm.
/// It shares the same bottom-up structure as RotateMergeSortIterative, but replaces the rotation-based
/// merge with SymMerge: a single symmetric binary search finds the optimal split point, then one rotation
/// and two recursive calls complete the merge.
/// This reduces the comparison count from O(n log² n) to O(n log n) compared to RotateMerge.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct SymMerge Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Phase 1 – Insertion Sort Seeding:</strong> Every contiguous block of size
/// InsertionSortThreshold is sorted independently with insertion sort.
/// The last block may be shorter; its size is clamped to the remaining element count.</description></item>
/// <item><description><strong>Phase 2 – Bottom-Up Merge Passes:</strong> Starting from
/// <c>width = InsertionSortThreshold</c>, each pass merges adjacent run pairs
/// [left..left+width-1] and [left+width..left+2*width-1], then doubles <c>width</c>.</description></item>
/// <item><description><strong>Already-Sorted Skip:</strong> Before each merge, if
/// <c>s[mid-1] ≤ s[mid]</c> the two runs are already in order and the merge is skipped.</description></item>
/// <item><description><strong>SymMerge Algorithm:</strong> Given sorted runs s[a..m) and s[m..b), computes
/// the midpoint <c>mid = (a+b)/2</c> and pivot sum <c>n = mid+m</c>, then binary-searches for split index
/// <c>start</c>. One rotation of s[start..end) (where end = n-start) brings elements into place,
/// followed by two recursive SymMerge calls on the resulting subproblems [a..start, mid) and [mid, end, b).</description></item>
/// <item><description><strong>Stability Preservation:</strong> The binary search uses ≥ comparison
/// (<c>s[p-c] ≥ s[c]</c> → advance lo), ensuring equal elements from the left run appear before those
/// from the right run in the merged result.</description></item>
/// <item><description><strong>Single-Element Base Cases:</strong> When one run has exactly 1 element,
/// a binary search finds its insertion position in the other run and a single shift completes the merge,
/// avoiding the full SymMerge binary search + rotation + recursion overhead.</description></item>
/// <item><description><strong>Rotation Algorithm (shift-based fast paths + GCD block-swap fallback):</strong>
/// Left-rotates s[lo..hi) by (m-lo) positions: [left_part | right_block] → [right_block | left_part].
/// Fast path leftLen≤4 or rightLen≤4: save the small side to local variables, shift, and write back.
/// General case uses GCD-based block-swap: repeatedly swaps adjacent blocks of the smaller side
/// until both sides are equal length, then performs one final block swap.
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Hybrid (Merge + Insertion), Iterative</description></item>
/// <item><description>Stable      : Yes (≥ comparison in binary search preserves relative order)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space)</description></item>
/// <item><description>Best case   : O(n) – Sorted data: insertion sort is O(n), all phase-2 merges are skipped</description></item>
/// <item><description>Average case: O(n log² n) moves, O(n log n) comparisons</description></item>
/// <item><description>Worst case  : O(n log² n) moves, O(n log n) comparisons</description></item>
/// <item><description>Space       : O(log n) – Recursion stack depth within each SymMerge call</description></item>
/// </list>
/// <para><strong>SymMerge vs RotateMerge:</strong></para>
/// <list type="bullet">
/// <item><description>RotateMerge scans left-to-right and may perform many small rotations, costing
/// O(n log n) comparisons per merge (due to binary search per gallop step)</description></item>
/// <item><description>SymMerge performs exactly one O(log n) binary search per recursive call and one rotation,
/// achieving O(n) comparisons per merge via balanced recursion (T(n) = 2T(n/2) + O(log n) = O(n))</description></item>
/// <item><description>Total comparisons: O(n log n) for SymMergeSort vs O(n log² n) for RotateMergeSortIterative</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Pok-Son Kim and Arne Kutzner, "Stable minimum storage merging by symmetric comparisons" (2004) https://link.springer.com/chapter/10.1007/978-3-540-30140-0_50</para>
/// <para>Go standard library: sort.symMerge (src/sort/sort.go) https://github.com/golang/go/blob/go1.25.8/src/sort/zsortinterface.go#L378-L479 </para>
/// </remarks>
public static class SymMergeSort
{
    // Threshold for using insertion sort for initial block seeding (Phase 1).
    // Matches Go's sort.stable blockSize (20).
    private const int InsertionSortThreshold = 20;

    // Threshold for falling back to insertion sort inside SymMerge recursion.
    // Lower than InsertionSortThreshold because SymMerge sub-problems are already
    // two sorted runs, so a merge-aware fallback is more efficient at smaller sizes.
    private const int SymMergeThreshold = 8;

    // Maximum small-side length for the shift-based Rotate fast path.
    // When the smaller side of the rotation is <= this value, the elements are
    // saved to local variables and a single shift replaces the 3-reversal.
    private const int RotateSmallThreshold = 4;

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;

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
    /// <param name="span">The span of elements to sort in place.</param>
    /// <param name="context">The sort context for tracking operations. Cannot be null.</param>
    public static void Sort<T, TContext>(Span<T> span, TContext context)
        where T : IComparable<T>
        where TContext : ISortContext
        => Sort(span, new ComparableComparer<T>(), context);

    /// <summary>
    /// Sorts the elements in the specified span using the provided comparer and sort context.
    /// This is the full-control version with explicit TComparer and TContext type parameters.
    /// </summary>
    public static void Sort<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        SortCore(s, span.Length);
    }

    /// <summary>
    /// Bottom-up iterative sort core: Phase 1 seeds sorted runs with insertion sort,
    /// Phase 2 merges adjacent run pairs with doubling widths using SymMerge.
    /// </summary>
    /// <param name="s">The SortSpan wrapping the span to sort</param>
    /// <param name="n">Total number of elements (span.Length)</param>
    private static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Phase 1: sort every block of size InsertionSortThreshold with insertion sort.
        s.Context.OnPhase(SortPhase.MergeInitSort, InsertionSortThreshold);
        for (var i = 0; i < n; i += InsertionSortThreshold)
            InsertionSort.SortCore(s, i, Math.Min(i + InsertionSortThreshold, n));

        // Phase 2: bottom-up merge passes using SymMerge.
        // Each pass merges adjacent pairs of runs of length `width`, then doubles width.
        var passNum = 0;
        for (var width = InsertionSortThreshold; width < n; width *= 2)
        {
            passNum++;
            s.Context.OnPhase(SortPhase.MergePass, width, passNum);
            // left + width < n guarantees a non-empty right run exists.
            for (var left = 0; left + width < n; left += width * 2)
            {
                // mid: exclusive end of left run / start of right run (half-open convention).
                var mid = left + width;
                // right: exclusive end of right run — clamped to last valid index for the final pair.
                var right = Math.Min(left + width * 2, n);

                // Already-sorted skip: left run's max ≤ right run's min → no merge needed.
                if (s.IsLessOrEqualAt(mid - 1, mid))
                    continue;

                SymMerge(s, left, mid, right);
            }
        }
    }

    /// <summary>
    /// Merges two sorted subarrays s[a..m) and s[m..b) in-place stably using the SymMerge algorithm.
    /// Performs a symmetric binary search to find the optimal split index, then one rotation,
    /// and recursively merges the two resulting subproblems.
    /// Based on the algorithm by Pok-Son Kim and Arne Kutzner (2004).
    /// </summary>
    /// <param name="s">The SortSpan to operate on</param>
    /// <param name="a">Inclusive start of the left sorted run (half-open: left run is s[a..m))</param>
    /// <param name="m">Exclusive end of left run / inclusive start of right run (s[m..b))</param>
    /// <param name="b">Exclusive end of the right sorted run</param>
    private static void SymMerge<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int a, int m, int b)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Base cases: empty halves
        if (a >= m || m >= b) return;

        // Already-sorted skip (Bottleneck 2): left run's max ≤ right run's min.
        // This fires frequently during recursive sub-problems where the two halves
        // ended up already in order after the rotation of the parent call.
        if (s.IsLessOrEqualAt(m - 1, m)) return;

        // Single-element base cases: when one side has exactly 1 element,
        // binary-search for its insertion position in the other run and shift-insert.
        // This avoids the full SymMerge machinery (binary search + rotation + 2 recursive calls)
        // and reduces comparisons to O(log n) + O(n) moves.
        if (m - a == 1)
        {
            // Left run is a single element: binary search in right run for insertion position.
            // Use lower_bound in the right run: find the first element >= tmp.
            // Inserting tmp before that position preserves stability because the left-run element
            // must remain before equal elements from the right run.
            var tmp = s.Read(a);
            var ilo = m;
            var ihi = b;
            while (ilo < ihi)
            {
                var c = (int)((uint)(ilo + ihi) >> 1);
                if (s.Compare(tmp, c) > 0)
                    ilo = c + 1;
                else
                    ihi = c;
            }
            // Shift s[m..ilo) one position to the left, then place tmp at ilo-1.
            for (var i = a; i < ilo - 1; i++)
                s.Write(i, s.Read(i + 1));
            s.Write(ilo - 1, tmp);
            return;
        }
        if (b - m == 1)
        {
            // Right run is a single element: binary search in left run for insertion position.
            // Use upper_bound in the left run: find the first element > tmp.
            // Inserting tmp there preserves stability because equal elements from the left run
            // must remain before the right-run element.
            var tmp = s.Read(m);
            var ilo = a;
            var ihi = m;
            while (ilo < ihi)
            {
                var c = (int)((uint)(ilo + ihi) >> 1);
                if (s.Compare(c, tmp) <= 0)
                    ilo = c + 1;
                else
                    ihi = c;
            }
            // Shift s[ilo..m) one position to the right, then place tmp at ilo.
            for (var i = m; i > ilo; i--)
                s.Write(i, s.Read(i - 1));
            s.Write(ilo, tmp);
            return;
        }

        // For small ranges, use merge-aware insertion that exploits the two-run structure.
        // Unlike plain InsertionSort (which treats the range as unsorted), this inserts elements
        // from the shorter side into the merged sequence using binary search, yielding
        // comparisons: roughly O(min(L,R) * log(L+R)), moves: up to O(min(L,R) * (L+R))
        if (b - a <= SymMergeThreshold)
        {
            MergeAwareInsertion(s, a, m, b);
            return;
        }

        // mid: midpoint of the whole range [a..b); pivot sum n = mid + m
        var mid = (int)((uint)(a + b) >> 1);
        var n = mid + m;

        // Binary search bounds: search for split index 'start' such that
        // elements s[a..start) go to the first half and s[start..m) go to the second half.
        // The symmetric mirror of 'start' in the right run is 'end = n - start'.
        int lo, hi;
        if (m > mid)
        {
            // Right run is longer: search in the left portion of the right run
            lo = n - b;
            hi = mid;
        }
        else
        {
            // Left run is longer (or equal): search in the right portion of the left run
            lo = a;
            hi = m;
        }

        // p = n - 1: the index such that indices (c) and (p - c) are mirror positions.
        var p = n - 1;

        // Find the smallest 'lo' such that s[p - lo] < s[lo].
        // When s[p-c] >= s[c], s[c] belongs in the first half → advance lo.
        // The >= condition (not >) ensures stability: equal left-run elements stay before right-run elements.
        while (lo < hi)
        {
            var c = (int)((uint)(lo + hi) >> 1);
            if (s.IsGreaterOrEqualAt(p - c, c))
                lo = c + 1;
            else
                hi = c;
        }

        var end = n - lo;

        // Rotate s[lo..end) to bring s[m..end) before s[lo..m):
        // [s[a..lo) | s[lo..m) | s[m..end) | s[end..b)]
        //            ^^^^^^^^^   ^^^^^^^^^^
        //            left part   right part  → after rotate: [s[m..end) | s[lo..m)]
        if (lo < m && m < end)
            Rotate(s, lo, m, end);

        // Recursively merge the two remaining subproblems on each half
        if (a < lo && lo < mid)
            SymMerge(s, a, lo, mid);
        if (mid < end && end < b)
            SymMerge(s, mid, end, b);
    }

    /// <summary>
    /// Left-rotates s[lo..hi) by (m - lo) positions: [s[lo..m) | s[m..hi)] → [s[m..hi) | s[lo..m)].
    /// Fast paths for small sides (≤ RotateSmallThreshold): save the small side to local variables,
    /// shift the large side, and write the saved elements back. This replaces swap-based rotation
    /// with 1×Read + 1×Write per element, cutting write traffic roughly in half for small rotations.
    /// General case uses GCD-based block-swap: repeatedly swaps adjacent blocks of the smaller side
    /// until both sides are equal, then performs one final block swap. This achieves exactly (hi - lo)
    /// swaps with good cache locality from contiguous swapRange operations.
    /// </summary>
    private static void Rotate<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int lo, int m, int hi)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var leftLen = m - lo;
        var rightLen = hi - m;

        // Fast path: small left side — save left, shift right leftward, write back saved
        if (leftLen <= RotateSmallThreshold)
        {
            // Save leftLen elements (1-4) to local variables
            var t0 = s.Read(lo);
            var t1 = leftLen > 1 ? s.Read(lo + 1) : default!;
            var t2 = leftLen > 2 ? s.Read(lo + 2) : default!;
            var t3 = leftLen > 3 ? s.Read(lo + 3) : default!;
            // Shift right portion to the left
            for (var i = lo; i < hi - leftLen; i++)
                s.Write(i, s.Read(i + leftLen));
            // Write saved elements at the end
            var dst = hi - leftLen;
            s.Write(dst, t0);
            if (leftLen > 1) s.Write(dst + 1, t1);
            if (leftLen > 2) s.Write(dst + 2, t2);
            if (leftLen > 3) s.Write(dst + 3, t3);
            return;
        }

        // Fast path: small right side — save right, shift left rightward, write back saved
        if (rightLen <= RotateSmallThreshold)
        {
            // Save rightLen elements (1-4) to local variables
            var t0 = s.Read(m);
            var t1 = rightLen > 1 ? s.Read(m + 1) : default!;
            var t2 = rightLen > 2 ? s.Read(m + 2) : default!;
            var t3 = rightLen > 3 ? s.Read(m + 3) : default!;
            // Shift left portion to the right
            for (var i = hi - 1; i >= lo + rightLen; i--)
                s.Write(i, s.Read(i - rightLen));
            // Write saved elements at the beginning
            s.Write(lo, t0);
            if (rightLen > 1) s.Write(lo + 1, t1);
            if (rightLen > 2) s.Write(lo + 2, t2);
            if (rightLen > 3) s.Write(lo + 3, t3);
            return;
        }

        // Generally simple symmerge implementation uses a 3-reversal rotate: reverse the left part, reverse the right part, then reverse the whole.
        // 3-reversal [A|B] → Reverse(A), Reverse(B), Reverse(AB) → [B|A]
        // However, this can be slower than necessary for small sides due to the multiple passes and non-sequential access patterns.
        // So we use the fast paths above for small sides, and fall back to the block-swap rotation method for larger sides, which achieves the rotation with exactly (hi - lo) swaps and good cache locality.
        //
        // Reverse(s, lo, m - 1);
        // Reverse(s, m, hi - 1);
        // Reverse(s, lo, hi - 1);


        // General case uses block-swap rotation.
        // repeatedly swaps adjacent blocks of the smaller side until both sides are equal,
        // then performs one final block swap.
        var i2 = leftLen;
        var j2 = rightLen;
        while (i2 != j2)
        {
            if (i2 > j2)
            {
                SwapRange(s, m - i2, m, j2);
                i2 -= j2;
            }
            else
            {
                SwapRange(s, m - i2, m + j2 - i2, i2);
                j2 -= i2;
            }
        }
        SwapRange(s, m - i2, m, i2);
    }

    /// <summary>
    /// Merge-aware insertion: merges two sorted runs s[a..m) and s[m..b) by binary-inserting
    /// elements from the shorter side into the merged sequence. This exploits the known two-run
    /// structure that the SymMerge fallback receives, unlike plain InsertionSort which treats
    /// the range as unsorted.
    /// <para>
    /// <strong>Left ≤ Right case:</strong> Inserts left elements right-to-left. After each insertion
    /// the sorted merged region [i+1, b) extends one position to the left. Binary search uses
    /// lower_bound (strict &lt;) so equal right-run elements stay after the left element (stability).
    /// </para>
    /// <para>
    /// <strong>Right &lt; Left case:</strong> Inserts right elements left-to-right. After each insertion
    /// the sorted merged region [a, i+1) extends one position to the right. Binary search uses
    /// upper_bound (≤) so equal left-run elements stay before the right element (stability).
    /// </para>
    /// </summary>
    private static void MergeAwareInsertion<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int a, int m, int b)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var leftLen = m - a;
        var rightLen = b - m;

        if (leftLen <= rightLen)
        {
            // Insert left elements (smaller side) into the merged sequence, right to left.
            // After each insertion, the sorted merged region [i+1, b) grows leftward.
            for (var i = m - 1; i >= a; i--)
            {
                var tmp = s.Read(i);

                // lower_bound in the merged region [i+1, b):
                // left element should go before equal right elements (stability).
                var ilo = i + 1;
                var ihi = b;
                while (ilo < ihi)
                {
                    var c = (int)((uint)(ilo + ihi) >> 1);
                    if (s.Compare(c, tmp) < 0)
                        ilo = c + 1;
                    else
                        ihi = c;
                }

                // Shift s[i+1..ilo) one position to the left, then place tmp at ilo-1.
                for (var j = i; j < ilo - 1; j++)
                    s.Write(j, s.Read(j + 1));
                s.Write(ilo - 1, tmp);
            }
        }
        else
        {
            // Insert right elements (smaller side) into the merged sequence, left to right.
            // After each insertion, the sorted merged region [a, i+1) grows rightward.
            for (var i = m; i < b; i++)
            {
                var tmp = s.Read(i);

                // upper_bound in the merged region [a, i):
                // right element should go after equal left elements (stability).
                var ilo = a;
                var ihi = i;
                while (ilo < ihi)
                {
                    var c = (int)((uint)(ilo + ihi) >> 1);
                    if (s.Compare(c, tmp) <= 0)
                        ilo = c + 1;
                    else
                        ihi = c;
                }

                // Shift s[ilo..i) one position to the right, then place tmp at ilo.
                for (var j = i; j > ilo; j--)
                    s.Write(j, s.Read(j - 1));
                s.Write(ilo, tmp);
            }
        }
    }

    /// <summary>
    /// Swaps n consecutive elements starting at index a with n consecutive elements starting at index b.
    /// Used by the GCD-based block-swap rotation algorithm.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SwapRange<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int a, int b, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        for (var i = 0; i < n; i++)
            s.Swap(a + i, b + i);
    }
}
