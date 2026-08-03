using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// SymMerge Sortは、SymMergeアルゴリズムを用いたイテレーティブな（ボトムアップ）安定インプレースソートです。
/// RotateMergeSortIterativeと同じボトムアップ構造ですが、マージステップにRotateではなくSymMergeを使用します。
/// SymMergeは各マージで対称二分探索により最適な分割点を1回見つけ、1回のローテーションと2つの再帰呼び出しでマージします。
/// 分割点を全体の中点から取るため、RotateMergeのように長い側の中央から取る場合と違って部分問題が常に等分され、探索範囲が毎回半分になります。
/// <br/>
/// SymMerge Sort is an iterative (bottom-up) stable in-place sort using the SymMerge algorithm.
/// It shares the same bottom-up structure as RotateMergeSortIterative, but replaces the rotation-based
/// merge with SymMerge: a single symmetric binary search finds the optimal split point, then one rotation
/// and two recursive calls complete the merge.
/// The split point comes from the midpoint of the whole range rather than from the median of the longer
/// side, so every sub-problem is halved and the search range halves with it.
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
/// <item><description><strong>Rotation Algorithm (shift fast paths, block swap when balanced, GCD-cycle otherwise):</strong>
/// Left-rotates s[lo..hi) by (m-lo) positions: [left_part | right_block] → [right_block | left_part].
/// Fast path leftLen≤4 or rightLen≤4: save the small side to local variables, shift, and write back.
/// Equal-length sides: a single block swap exchanges the two halves.
/// Otherwise: the GCD-cycle (juggling) rotation the SymMerge paper's assignment bound is stated for —
/// gcd(leftLen, rightLen) independent cycles, each advanced by assignment.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Hybrid (Merge + Insertion), Iterative</description></item>
/// <item><description>Stable      : Yes (≥ comparison in binary search preserves relative order)</description></item>
/// <item><description>In-place    : Yes (no external buffer; the symmetric recursion needs an O(log n) stack)</description></item>
/// <item><description>Best case   : O(n log n) – SymMerge is not adaptive: every pass still binary-searches each pair, even on input where no rotation would move an element</description></item>
/// <item><description>Average case: O(n log² n) moves, O(n log n) comparisons</description></item>
/// <item><description>Worst case  : O(n log² n) moves, O(n log n) comparisons</description></item>
/// <item><description>Comparisons : O(n log n) - each SymMerge locates its split by binary search, so a merge level costs O(n) comparisons</description></item>
/// <item><description>Swaps       : O(n log² n) - bounded by the rotations, but only the equal-length ones exchange blocks; every other rotation moves elements by assignment and is counted under Index Writes instead</description></item>
/// <item><description>Index Reads : O(n log² n) - dominated by the rotations rather than by the binary searches</description></item>
/// <item><description>Index Writes: O(n log² n) - each rotation writes every element it moves; this is what makes the move bound exceed the comparison bound</description></item>
/// <item><description>Space       : O(log n) – Recursion stack depth within each SymMerge call</description></item>
/// </list>
/// <para><strong>Implementation note – already-sorted skip:</strong></para>
/// <para>The Best case row above is SymMerge's own: neither Kim and Kutzner's algorithm nor Go's
/// sort.stable is adaptive, and both pay their binary searches on every pass regardless of input.
/// This implementation adds a check Go does not have — a merge whose left run ends at or below the
/// start of its right run is already merged, so it is skipped — at the top of each phase-2 pair and
/// again at each SymMerge sub-problem. The check is identity-preserving: it never changes the output
/// or the order of any operation, it only recognises a merge that has nothing to do. Its effect is
/// that sorted input costs one comparison per pair instead of a binary search, which drops the
/// measured cost on sorted and nearly-sorted input to O(n); it does not make SymMerge adaptive.</para>
/// <para><strong>Implementation note – rotation cost:</strong></para>
/// <para>Kim and Kutzner state the O((M+N)·log M) assignment bound for a rotation costing M+N+gcd(M+N)
/// assignments, i.e. the GCD-cycle (juggling) rotation. Go's sort.symMerge substitutes a block-swap
/// (Gries–Mills) rotation, which costs len - gcd swaps and therefore 2(len - gcd) reads and writes —
/// roughly twice the traffic on unbalanced sides. Go does this because sort.Interface exposes only Swap;
/// SortSpan exposes Read and Write, so this implementation uses the GCD-cycle rotation the bound assumes
/// and keeps the block swap only for equal-length sides, where the two cost the same len writes and the
/// block swap runs as two sequential streams.</para>
/// <para><strong>SymMerge vs RotateMerge:</strong></para>
/// <list type="bullet">
/// <item><description>SymMerge performs exactly one O(log n) binary search per recursive call and one rotation,
/// achieving O(n) comparisons per merge via balanced recursion (T(n) = 2T(n/2) + O(log n) = O(n))</description></item>
/// <item><description>RotateMerge splits at the median of the longer side instead, so a sub-problem is only
/// guaranteed to shrink to 3/4 rather than 1/2; it reaches the same O(n log n) total comparisons
/// (Dudziński–Dydek) but with a larger constant, and its sub-problems are unbalanced enough that the
/// resulting rotations move more elements</description></item>
/// <item><description>Both are O(n log n) comparisons and O(n log² n) moves; the separation between them is a
/// constant factor, not an asymptotic one</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Pok-Son Kim and Arne Kutzner, "Stable minimum storage merging by symmetric comparisons" (2004) https://link.springer.com/chapter/10.1007/978-3-540-30140-0_50</para>
/// <para>Go standard library: sort.symMerge (src/sort/sort.go) https://github.com/golang/go/blob/go1.25.8/src/sort/zsortinterface.go#L378-L479 </para>
/// </remarks>
public static class SymMergeSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by SymMergeSortTests, which derives from StableSortTestsBase.</remarks>
    public static bool IsStable => true;

    // Threshold for using insertion sort for initial block seeding (Phase 1).
    // Matches Go's sort.stable blockSize (20).
    private const int InsertionSortThreshold = 20;

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
        // n is a span length, so it can be as large as int.MaxValue. Every index expression below is
        // therefore written so that it cannot overflow: none of them forms a sum that exceeds n.
        // The natural spellings do overflow — `width * 2` once width passes 2^30 (reached from 20 at
        // n > 20*2^26 = 1,342,177,280), and `left + width` once the two together pass int.MaxValue.
        // Because the project compiles unchecked, such a sum wraps negative instead of throwing, and a
        // negative index reaches SortSpan's Unsafe.Add unbounded. Comparing against a difference
        // (n - i, n - mid, n - width) keeps both operands inside the span and needs no guard.

        // Phase 1: sort every block of size InsertionSortThreshold with insertion sort.
        s.Context.OnPhase(SortPhase.MergeInitSort, InsertionSortThreshold);
        for (var i = 0; i < n; )
        {
            var blockEnd = n - i > InsertionSortThreshold ? i + InsertionSortThreshold : n;
            InsertionSort.SortCore(s, i, blockEnd);
            i = blockEnd;
        }

        // Phase 2: bottom-up merge passes using SymMerge.
        // Each pass merges adjacent pairs of runs of length `width`, then doubles width.
        var passNum = 0;
        var width = InsertionSortThreshold;
        while (width < n)
        {
            passNum++;
            s.Context.OnPhase(SortPhase.MergePass, width, passNum);
            // left < n - width is `left + width < n`: it guarantees a non-empty right run exists.
            for (var left = 0; left < n - width; )
            {
                // mid: exclusive end of left run / start of right run (half-open convention).
                var mid = left + width;
                // right: exclusive end of right run — clamped to n for the final, short pair.
                var right = n - mid > width ? mid + width : n;

                // Already-sorted skip: left run's max ≤ right run's min → no merge needed.
                if (!s.IsLessOrEqualAt(mid - 1, mid))
                    SymMerge(s, left, mid, right);

                // right is min(left + 2*width, n), so this is the `left += width * 2` step: when the
                // pair was clamped, right == n ends the pass exactly as the unclamped sum would.
                left = right;
            }

            // Stop before doubling would pass n. Spelled as width > n - width rather than
            // width * 2 > n so the pass sequence terminates without ever forming the overflowing product.
            if (width > n - width) break;
            width *= 2;
        }
    }

    /// <summary>
    /// Merges two sorted subarrays s[a..m) and s[m..b) in-place stably using the SymMerge algorithm.
    /// Performs a symmetric binary search to find the optimal split index, then one rotation,
    /// and recursively merges the two resulting subproblems.
    /// Based on the algorithm by Pok-Son Kim and Arne Kutzner (2004).
    /// <para>
    /// The second of the two sub-problems is in tail position, so it is taken as a loop iteration
    /// instead of a call; only the left sub-problem [a..start, mid) consumes a stack frame.
    /// </para>
    /// </summary>
    /// <param name="s">The SortSpan to operate on</param>
    /// <param name="a">Inclusive start of the left sorted run (half-open: left run is s[a..m))</param>
    /// <param name="m">Exclusive end of left run / inclusive start of right run (s[m..b))</param>
    /// <param name="b">Exclusive end of the right sorted run</param>
    private static void SymMerge<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int a, int m, int b)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (true)
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
                    if (s.IsValueGreaterThan(tmp, c))
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
                    if (s.IsElementLessOrEqual(c, tmp))
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

            // Merge the two remaining subproblems on each half. The left one recurses;
            // the right one is in tail position, so it continues the loop instead.
            if (a < lo && lo < mid)
                SymMerge(s, a, lo, mid);
            if (mid < end && end < b)
            {
                a = mid;
                m = end;
                continue;
            }
            return;
        }
    }

    /// <summary>
    /// Left-rotates s[lo..hi) by (m - lo) positions: [s[lo..m) | s[m..hi)] → [s[m..hi) | s[lo..m)].
    /// Fast paths for small sides (≤ RotateSmallThreshold): save the small side to local variables,
    /// shift the large side, and write the saved elements back.
    /// Equal-length sides: one block swap of the two halves.
    /// General case: the GCD-cycle (juggling) rotation — gcd(leftLen, rightLen) independent cycles,
    /// each advanced by assignment.
    /// Every path moves each of the len elements exactly once; the equal-length branch reports that
    /// movement as swaps and the other three as reads and writes.
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

        // Equal-length sides: one block swap exchanges the two halves. That is leftLen swaps,
        // so len reads and len writes — the same traffic as the GCD-cycle below, but read and
        // written as two sequential streams. This is the only shape where a swap-based rotation
        // does not pay for the extra writes, so it is the only shape that keeps one.
        if (leftLen == rightLen)
        {
            SwapRange(s, lo, m, leftLen);
            return;
        }

        // General case: GCD-cycle (juggling) rotation.
        // Left-rotating len elements by leftLen decomposes the permutation into
        // gcd(leftLen, rightLen) independent cycles; walking each cycle by assignment reads and
        // writes every element exactly once, for len reads and len writes.
        //
        // The alternatives both cost more writes here:
        //   - block swap (Gries-Mills), what Go's sort.rotate uses: len - gcd swaps,
        //     i.e. 2(len - gcd) reads and writes — about twice the traffic when the sides are
        //     unbalanced, and equal only when leftLen == rightLen (handled above).
        //     Go takes it because sort.Interface exposes only Swap; SortSpan exposes Read/Write.
        //   - 3-reversal, Reverse(A), Reverse(B), Reverse(AB): 3·len/2 swaps, i.e. 3·len writes.
        //
        // Kim and Kutzner state the O((M+N)·log M) assignment bound of SymMerge for a rotation
        // costing M+N+gcd(M+N) assignments, which is this one.
        var len = hi - lo;
        var cycles = Gcd(leftLen, rightLen);
        for (var cycle = 0; cycle < cycles; cycle++)
        {
            // Each cycle carries one element in `tmp` while the rest shift back by leftLen.
            var start = lo + cycle;
            var tmp = s.Read(start);
            var current = start;
            while (true)
            {
                var next = current + leftLen;
                if (next >= hi) next -= len;
                if (next == start) break;
                s.Write(current, s.Read(next));
                current = next;
            }
            s.Write(current, tmp);
        }
    }

    /// <summary>
    /// Greatest common divisor by Euclid's algorithm. Used to count the independent cycles of the
    /// GCD-cycle rotation.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Gcd(int a, int b)
    {
        while (b != 0)
        {
            var t = b;
            b = a % b;
            a = t;
        }
        return a;
    }

    /// <summary>
    /// Swaps n consecutive elements starting at index a with n consecutive elements starting at index b.
    /// Used by the equal-length branch of <see cref="Rotate"/>.
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
