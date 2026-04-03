using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 最適化したイテレーティブな（ボトムアップ）Rotate Merge Sortです。
/// 再帰を使わず2フェーズで配列を処理します：フェーズ1は各ブロック（≤InsertionSortThreshold要素）をInsertionSortでソートし、フェーズ2はランの幅を毎パス倍増させながら隣接するランのペアをインプレースローテーションでマージします。
/// マージは分割統治型インプレースマージ（大きい側の中央を取り、反対側をbinary searchし、rotateして左右の部分問題を生成）を明示スタック（stackalloc）で駆動します。
/// 安定ソートであり、外部バッファを使用せずに典型的にはO(n log² n)の性能を達成します。
/// <br/>
/// Iterative (bottom-up) Rotate Merge Sort.
/// Eliminates recursion by processing the array in two phases: Phase 1 sorts each block of ≤InsertionSortThreshold elements with insertion sort, Phase 2 merges adjacent run pairs using in-place rotation while doubling the run width each pass.
/// The merge uses divide-and-conquer in-place merging driven by an explicit stack (stackalloc): pick the median of the larger side, binary search in the opposite side, rotate, then push both sub-problems onto the worklist.
/// This algorithm is stable and typically achieves O(n log² n) performance without requiring an external buffer.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Iterative Rotate Merge Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Phase 1 – Insertion Sort Seeding:</strong> Every contiguous block of size
/// InsertionSortThreshold is sorted independently with insertion sort.
/// The last block may be shorter; its size is clamped to the remaining element count.</description></item>
/// <item><description><strong>Phase 2 – Bottom-Up Merge Passes:</strong> Starting from
/// <c>width = InsertionSortThreshold</c>, each pass merges adjacent run pairs
/// [left..left+width-1] and [left+width..left+2*width-1], then doubles <c>width</c>.
/// The outer loop runs ⌈log₂(n/InsertionSortThreshold)⌉ times.</description></item>
/// <item><description><strong>End-of-Array Clamping:</strong> The right boundary of the last pair is
/// clamped: <c>right = Math.Min(left + 2*width - 1, n - 1)</c>.
/// The loop condition <c>left &lt; n - width</c> guarantees a non-empty right run exists before merging.</description></item>
/// <item><description><strong>Already-Sorted Skip:</strong> Before each merge, if
/// <c>s[mid] ≤ s[mid+1]</c> the two runs are already in order and the merge is skipped,
/// reducing work on nearly-sorted inputs.</description></item>
/// <item><description><strong>Completely-Disjoint Skip:</strong> Before each merge, if
/// <c>s[left] &gt; s[right]</c> every element of the left run exceeds every element of the right run.
/// The entire pair is resolved with a single rotation, bypassing all divide-and-conquer work.
/// Reverse-order inputs produce run pairs exactly in this form after Phase 1, turning O(n log² n) merge work into O(n log n) rotations.</description></item>
/// <item><description><strong>Divide-and-Conquer In-Place Merge (Explicit Stack):</strong> Each merge picks the median of
/// the larger side, binary searches (lower_bound or upper_bound) for its position in the opposite side,
/// rotates the overlapping region, then pushes the two resulting sub-problems onto an explicit worklist (stackalloc).
/// This replaces recursion with an iterative loop, eliminating O(log n) call-stack frames per merge.</description></item>
/// <item><description><strong>Rotation Algorithm (Left-Rotate by k, 3-Reversal with fast paths):</strong>
/// Left-rotates A[left..right] by k positions: [left_k_elems | rest] → [rest | left_k_elems].
/// Fast path k==1: move leftmost element to right end.
/// Fast path k==n-1: move rightmost element to left end (left rotate n-1 = right rotate 1).
/// General case uses 3-reversal: Reverse(A[left..left+k-1]), Reverse(A[left+k..right]), Reverse(A[left..right]).</description></item>
/// <item><description><strong>Stability Preservation:</strong> Lower bound is used when the left side is larger
/// and upper bound when the right side is larger, ensuring equal elements from the left run appear before
/// those from the right run.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Hybrid (Merge + Insertion), Iterative</description></item>
/// <item><description>Stable      : Yes (lower/upper bound preserves relative order)</description></item>
/// <item><description>Bufferless  : Yes (no external buffer; merge uses O(log n) explicit stack via stackalloc)</description></item>
/// <item><description>Best case   : O(n) – Sorted data: insertion sort is O(n), all phase-2 merges are skipped</description></item>
/// <item><description>Average case: O(n log² n) – Binary search (log n) + rotation (n) per merge × log n passes</description></item>
/// <item><description>Worst case  : O(n log² n)</description></item>
/// <item><description>Space       : O(log n) – Explicit merge stack (stackalloc, no heap allocation or call-stack recursion)</description></item>
/// </list>
/// <para><strong>Fully Iterative Design:</strong></para>
/// <list type="bullet">
/// <item><description>The outer sort loop is iterative (bottom-up), eliminating O(log n) sort recursion depth</description></item>
/// <item><description>The merge is also iterative: an explicit worklist (stackalloc) replaces recursion, eliminating O(log n) merge call-stack frames</description></item>
/// <item><description>No recursion at all — StackOverflow is impossible regardless of input size or pattern</description></item>
/// <item><description>Merge order differs from recursive variant: bottom-up processes fixed-width blocks rather than balanced halves,
/// but total work and asymptotic complexity are identical</description></item>
/// </list>
/// </remarks>
public static class RotateMergeSort
{
    // Threshold for using insertion sort instead of rotation-based merge
    private const int InsertionSortThreshold = 16;

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
    /// Phase 2 merges adjacent run pairs with doubling widths until fully sorted.
    /// </summary>
    /// <param name="s">The SortSpan wrapping the span to sort</param>
    /// <param name="n">Total number of elements (span.Length)</param>
    private static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Phase 1: sort every block of size InsertionSortThreshold with insertion sort.
        // InsertionSort.SortCore uses exclusive end [first, last), so pass Math.Min(i + threshold, n).
        s.Context.OnPhase(SortPhase.MergeInitSort, InsertionSortThreshold);
        for (var i = 0; i < n; i += InsertionSortThreshold)
            InsertionSort.SortCore(s, i, Math.Min(i + InsertionSortThreshold, n));

        // Phase 2: bottom-up merge passes.
        // Each pass merges adjacent pairs of runs of length `width`, then doubles width.
        var passNum = 0;
        for (var width = InsertionSortThreshold; width < n; width *= 2)
        {
            passNum++;
            s.Context.OnPhase(SortPhase.MergePass, width, passNum);
            // left + width < n guarantees a non-empty right run ([mid+1..right]) exists.
            for (var left = 0; left < n - width; left += width * 2)
            {
                // mid: inclusive end of left run — always exactly `width` elements from `left`.
                var mid = left + width - 1;
                // right: inclusive end of right run — clamped to last valid index for the final pair.
                var right = Math.Min(left + width * 2 - 1, n - 1);

                // Already-sorted skip: left run's max ≤ right run's min → no merge needed.
                if (s.IsLessOrEqualAt(mid, mid + 1))
                    continue;

                // Completely disjoint in reverse order: every left element > every right element → rotate entire run pair.
                // Reverse-array inputs produce run pairs exactly in this form after Phase 1, so this skips all recursive merge work.
                if (!s.IsLessOrEqualAt(left, right))
                {
                    Rotate(s, left, right, mid - left + 1);
                    continue;
                }

                MergeInPlace(s, left, mid, right);
            }
        }
    }

    /// <summary>
    /// Merges two sorted subarrays [left..mid] and [mid+1..right] in-place using divide-and-conquer rotation.
    /// Picks the median of the larger side, binary searches for its position in the opposite side,
    /// rotates the overlapping region, then iteratively processes both resulting sub-problems via an explicit stack.
    /// Uses stackalloc for the worklist. A fixed-size work stack sized for typical practical inputs, add overflow protection or pooled fallback if strict robustness is required.
    /// </summary>
    private static void MergeInPlace<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int mid, int right)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Explicit stack for iterative divide-and-conquer merge.
        // Each decomposition step halves the smaller side, so max depth is O(log n).
        // 64 entries handle any practical Span<T> length addressable by int indexing.
        Span<int> stackL = stackalloc int[64];
        Span<int> stackM = stackalloc int[64];
        Span<int> stackR = stackalloc int[64];
        var top = 0;
        stackL[top] = left;
        stackM[top] = mid;
        stackR[top] = right;
        top++;

        while (top > 0)
        {
            top--;
            var l = stackL[top];
            var m = stackM[top];
            var r = stackR[top];

            var len1 = m - l + 1;
            var len2 = r - m;

            // Empty side — nothing to merge
            if (len1 <= 0 || len2 <= 0) continue;

            // Already-sorted skip: left run's max ≤ right run's min → merge is a no-op.
            // This catches trivial sub-problems produced by rotation (one side already in place).
            if (s.IsLessOrEqualAt(m, m + 1)) continue;

            // Completely disjoint in reverse order: every left element > every right element → rotate sub-problem into place.
            if (!s.IsLessOrEqualAt(l, r))
            {
                Rotate(s, l, r, len1);
                continue;
            }

            // Two single elements: the only possible action is a swap
            if (len1 == 1 && len2 == 1)
            {
                s.Swap(l, r);
                continue;
            }

            int mid1, mid2;

            if (len1 > len2)
            {
                // Left side is larger: pick its median, lower_bound in right side
                mid1 = l + len1 / 2;
                mid2 = LowerBound(s, m + 1, r, mid1);
            }
            else
            {
                // Right side is larger (or equal): pick its median, upper_bound in left side
                mid2 = m + 1 + len2 / 2;
                mid1 = UpperBound(s, l, m, mid2);
            }

            // Rotate [mid1..m] ++ [m+1..mid2-1] → [m+1..mid2-1] ++ [mid1..m]
            var rotateLen = m - mid1 + 1;
            if (rotateLen > 0 && mid2 > m + 1)
                Rotate(s, mid1, mid2 - 1, rotateLen);

            // New boundary after rotation
            var newMid = mid1 + (mid2 - m - 1);

            // Push both sub-problems: right first (processed second), then left (processed first)
            stackL[top] = newMid;
            stackM[top] = newMid + (m - mid1);
            stackR[top] = r;
            top++;

            stackL[top] = l;
            stackM[top] = mid1 - 1;
            stackR[top] = newMid - 1;
            top++;
        }
    }

    /// <summary>
    /// Finds the first position in [left..right] where s[pos] >= s[keyIndex] (lower bound).
    /// Returns right + 1 if all elements are less than s[keyIndex].
    /// Uses &lt;= comparison so equal elements from the left run stay before the right run (stability).
    /// </summary>
    private static int LowerBound<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int right, int keyIndex)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var lo = left;
        var hi = right + 1;

        while (lo < hi)
        {
            var m = lo + (hi - lo) / 2;
            if (s.IsLessAt(m, keyIndex))
                lo = m + 1;
            else
                hi = m;
        }

        return lo;
    }

    /// <summary>
    /// Finds the first position in [left..right] where s[pos] > s[keyIndex] (upper bound).
    /// Returns right + 1 if all elements are less than or equal to s[keyIndex].
    /// Uses strict &gt; so equal elements from the left run stay before the right run (stability).
    /// </summary>
    private static int UpperBound<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int right, int keyIndex)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var lo = left;
        var hi = right + 1;

        while (lo < hi)
        {
            var m = lo + (hi - lo) / 2;
            if (s.IsLessOrEqualAt(m, keyIndex))
                lo = m + 1;
            else
                hi = m;
        }

        return lo;
    }

    /// <summary>
    /// Left-rotates A[left..right] by k positions: [left_k_elems | rest] → [rest | left_k_elems].
    /// Fast path k==1 (left rotate 1): move leftmost element to right end.
    /// Fast path k==n-1 (left rotate n-1 = right rotate 1): move rightmost element to left end.
    /// General case uses 3-reversal: Reverse[left..left+k-1], Reverse[left+k..right], Reverse[left..right].
    /// All paths are linear scans, enabling hardware prefetching without GCD or modulo overhead.
    /// </summary>
    /// <param name="k">The number of positions to rotate left</param>
    private static void Rotate<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int right, int k)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (k == 0 || left >= right) return;

        var n = right - left + 1;
        k = k % n;
        if (k == 0) return;

        // Fast path: k==1 (left rotate 1) - move leftmost element to right end, shift rest left (sequential read/write, no swap)
        if (k == 1)
        {
            var tmp = s.Read(left);
            for (var i = left; i < right; i++)
                s.Write(i, s.Read(i + 1));
            s.Write(right, tmp);
            return;
        }

        // Fast path: k==n-1 (left rotate n-1 = right rotate 1) - move rightmost element to left end, shift rest right (sequential read/write, no swap)
        if (k == n - 1)
        {
            var tmp = s.Read(right);
            for (var i = right; i > left; i--)
                s.Write(i, s.Read(i - 1));
            s.Write(left, tmp);
            return;
        }

        // General case: left rotate by k via 3-reversal (linear scans, cache-friendly, no GCD overhead)
        // [A|B] → Reverse(A), Reverse(B), Reverse(AB) → [B|A]
        Reverse(s, left, left + k - 1);
        Reverse(s, left + k, right);
        Reverse(s, left, right);
    }

    /// <summary>
    /// Reverses a subarray in-place using swaps.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Reverse<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int right)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (left < right)
        {
            s.Swap(left, right);
            left++;
            right--;
        }
    }
}

/// <summary>
/// 最適化した再帰呼び出しなRotate Merge Sortです。
/// 配列を再帰的に半分に分割し、それぞれをソートした後、分割統治型の再帰的インプレースマージ（大きい側の中央を取り、反対側をbinary searchし、rotateして左右を再帰）でマージします。
/// 安定ソートであり、外部バッファを使用せずに典型的にはO(n log² n)の性能を達成します。
/// 小さい配列（≤16要素）ではInsertionSortを使用する実用的な最適化を含みます。
/// <br/>
/// Optimized recursive Rotate Merge Sort.
/// Recursively divides the array in half, sorts each part, then merges sorted subarrays in-place using
/// divide-and-conquer rotation merge: pick the median of the larger side, binary search in the opposite side,
/// rotate, then recursively merge both halves.
/// This algorithm is stable and typically achieves O(n log² n) performance without requiring an external buffer.
/// Includes practical optimization: insertion sort for small subarrays (≤16 elements).
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Rotate Merge Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Hybrid Optimization:</strong> For subarrays with ≤16 elements, insertion sort is used instead of rotation-based merge.
/// This is a practical optimization similar to TimSort and IntroSort, reducing overhead for small sizes.</description></item>
/// <item><description><strong>Divide Step (Binary Partitioning):</strong> The array must be divided into two roughly equal halves at each recursion level.
/// The midpoint is calculated as mid = (left + right) / 2, ensuring balanced subdivision.
/// This guarantees a recursion depth of ⌈log₂(n)⌉.</description></item>
/// <item><description><strong>Base Case (Termination Condition):</strong> Recursion must terminate when a subarray has size ≤ 1.
/// An array of size 0 or 1 is trivially sorted and requires no further processing.</description></item>
/// <item><description><strong>Conquer Step (Recursive Sorting):</strong> Each half must be sorted independently via recursive calls.
/// The left subarray [left..mid] and right subarray [mid+1..right] are sorted before merging.</description></item>
/// <item><description><strong>Divide-and-Conquer In-Place Merge:</strong> Each merge picks the median of
/// the larger side, binary searches (lower_bound or upper_bound) for its position in the opposite side,
/// rotates the overlapping region, then recursively merges the two resulting sub-problems.
/// Picking the larger side guarantees progress (the median index is always ≥ 1).</description></item>
/// <item><description><strong>Already-Sorted Skip:</strong> Before each merge, if
/// <c>s[mid] ≤ s[mid+1]</c> the two runs are already in order and the merge is skipped.</description></item>
/// <item><description><strong>Completely-Disjoint Skip:</strong> Before each merge, if
/// <c>s[left] &gt; s[right]</c> every element of the left run exceeds every element of the right run.
/// The entire pair is resolved with a single rotation, bypassing all recursive merge work.
/// Reverse-order inputs produce run pairs exactly in this form, turning O(n log² n) merge work into O(n log n) rotations.</description></item>
/// <item><description><strong>Rotation Algorithm (Left-Rotate by k, 3-Reversal with fast paths):</strong>
/// Fast path k==1: move leftmost element to right end (sequential reads/writes, no swap).
/// Fast path k==n-1: move rightmost element to left end (left rotate n-1 = right rotate 1, sequential reads/writes, no swap).
/// General case uses 3-reversal: Reverse(A[left..left+k-1]), Reverse(A[left+k..right]), Reverse(A[left..right]).
/// All three phases are linear scans, enabling hardware prefetching and eliminating GCD/modulo overhead.</description></item>
/// <item><description><strong>Stability Preservation:</strong> Lower bound is used when the left side is larger
/// and upper bound when the right side is larger, ensuring equal elements from the left run appear before
/// those from the right run.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Hybrid (Merge + Insertion)</description></item>
/// <item><description>Stable      : Yes (lower/upper bound preserves relative order)</description></item>
/// <item><description>Bufferless  : Yes (no external buffer, uses rotation for in-place merging)</description></item>
/// <item><description>Best case   : O(n) - Sorted data with insertion sort optimization for small partitions</description></item>
/// <item><description>Average case: O(n log² n) - Binary search (log n) + rotation (n) per merge level (log n levels)</description></item>
/// <item><description>Worst case  : O(n log² n) - Rotation adds O(n) factor to each merge operation</description></item>
/// <item><description>Comparisons : Best O(n), Average/Worst O(n log² n)</description></item>
/// <item><description>Writes      : Best O(n), Average/Worst O(n log² n) - k==1/k==n-1 fast paths use sequential writes; 3-reversal uses cache-friendly swaps</description></item>
/// <item><description>Swaps       : 0 for k==1/k==n-1 fast paths; O(n/2) per rotation in general case (3-reversal)</description></item>
/// <item><description>Space       : O(log n) - Recursion stack for sort + merge, no auxiliary buffer needed</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Merge_sort#Variants</para>
/// <para>Rotation-based in-place merge: Practical In-Place Merging (Geffert et al.)</para>
/// </remarks>
public static class RotateMergeSortRecursive
{
    // Threshold for using insertion sort instead of rotation-based merge
    // Small subarrays benefit from insertion sort's lower overhead
    private const int InsertionSortThreshold = 16;

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array (in-place operations only)

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

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        SortCore(s, 0, span.Length - 1);
    }

    /// <summary>
    /// Core recursive merge sort implementation.
    /// </summary>
    /// <param name="s">The SortSpan wrapping the span to sort</param>
    /// <param name="left">The inclusive start index of the range to sort</param>
    /// <param name="right">The inclusive end index of the range to sort</param>
    private static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int right)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (right <= left) return; // Base case: array of size 0 or 1 is sorted

        var length = right - left + 1;

        // Optimization: Use insertion sort for small subarrays
        // Rotation overhead is too high for small sizes, and insertion sort has better cache locality
        if (length <= InsertionSortThreshold)
        {
            // Reuse existing InsertionSort.SortCore
            // Note: SortCore uses exclusive end index [first, last), so we pass right + 1
            InsertionSort.SortCore(s, left, right + 1);
            return;
        }

        var mid = left + (right - left) / 2;

        // Conquer: Recursively sort left and right halves
        SortCore(s, left, mid);
        SortCore(s, mid + 1, right);

        // Optimization: Skip merge if already sorted (left[last] <= right[first])
        if (s.IsLessOrEqualAt(mid, mid + 1))
        {
            return; // Already sorted, no merge needed
        }

        // Completely disjoint in reverse order: every left element > every right element → rotate entire run pair.
        // Reverse-array inputs produce run pairs exactly in this form, so this skips all recursive merge work.
        if (!s.IsLessOrEqualAt(left, right))
        {
            Rotate(s, left, right, mid - left + 1);
            return;
        }

        // Merge: Combine two sorted halves in-place using rotation
        MergeInPlace(s, left, mid, right);
    }

    /// <summary>
    /// Merges two sorted subarrays [left..mid] and [mid+1..right] in-place using divide-and-conquer rotation.
    /// Picks the median of the smaller side, binary searches for its position in the opposite side,
    /// rotates the overlapping region, then recursively merges both resulting sub-problems.
    /// </summary>
    private static void MergeInPlace<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int mid, int right)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var len1 = mid - left + 1;
        var len2 = right - mid;

        if (len1 == 0 || len2 == 0) return;

        // Already-sorted skip: left run's max ≤ right run's min → merge is a no-op.
        if (s.IsLessOrEqualAt(mid, mid + 1)) return;

        // Completely disjoint in reverse order: every left element > every right element → rotate sub-problem into place.
        if (!s.IsLessOrEqualAt(left, right))
        {
            Rotate(s, left, right, len1);
            return;
        }

        if (len1 == 1 && len2 == 1)
        {
            s.Swap(left, right);
            return;
        }

        int mid1, mid2;

        if (len1 > len2)
        {
            // Left side is larger: pick its median, lower_bound in right side
            mid1 = left + len1 / 2;
            mid2 = LowerBound(s, mid + 1, right, mid1);
        }
        else
        {
            // Right side is larger (or equal): pick its median, upper_bound in left side
            mid2 = mid + 1 + len2 / 2;
            mid1 = UpperBound(s, left, mid, mid2);
        }

        var rotateLen = mid - mid1 + 1;
        if (rotateLen > 0 && mid2 > mid + 1)
            Rotate(s, mid1, mid2 - 1, rotateLen);

        var newMid = mid1 + (mid2 - mid - 1);

        MergeInPlace(s, left, mid1 - 1, newMid - 1);
        MergeInPlace(s, newMid, newMid + (mid - mid1), right);
    }

    /// <summary>
    /// Finds the first position in [left..right] where s[pos] >= s[keyIndex] (lower bound).
    /// </summary>
    private static int LowerBound<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int right, int keyIndex)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var lo = left;
        var hi = right + 1;

        while (lo < hi)
        {
            var m = lo + (hi - lo) / 2;
            if (s.IsLessAt(m, keyIndex))
                lo = m + 1;
            else
                hi = m;
        }

        return lo;
    }

    /// <summary>
    /// Finds the first position in [left..right] where s[pos] > s[keyIndex] (upper bound).
    /// </summary>
    private static int UpperBound<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int right, int keyIndex)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var lo = left;
        var hi = right + 1;

        while (lo < hi)
        {
            var m = lo + (hi - lo) / 2;
            if (s.IsLessOrEqualAt(m, keyIndex))
                lo = m + 1;
            else
                hi = m;
        }

        return lo;
    }

    /// <summary>
    /// Rotates a subarray left by k positions.
    /// Fast paths for k==1 and k==n-1 shift a single element sequentially (no swaps, linear access).
    /// General case uses 3-reversal: Reverse[left..left+k-1], Reverse[left+k..right], Reverse[left..right].
    /// All paths are linear scans, enabling hardware prefetching without GCD or modulo overhead.
    /// </summary>
    /// <param name="s">The SortSpan wrapping the array</param>
    /// <param name="left">The start index of the subarray to rotate</param>
    /// <param name="right">The end index of the subarray to rotate</param>
    /// <param name="k">The number of positions to rotate left</param>
    private static void Rotate<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int right, int k)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (k == 0 || left >= right) return;

        var n = right - left + 1;
        k = k % n;
        if (k == 0) return;

        // Fast path: k==1 (left rotate 1) - move leftmost element to right end, shift rest left (sequential read/write, no swap)
        if (k == 1)
        {
            var tmp = s.Read(left);
            for (var i = left; i < right; i++)
                s.Write(i, s.Read(i + 1));
            s.Write(right, tmp);
            return;
        }

        // Fast path: k==n-1 (left rotate n-1 = right rotate 1) - move rightmost element to left end, shift rest right (sequential read/write, no swap)
        if (k == n - 1)
        {
            var tmp = s.Read(right);
            for (var i = right; i > left; i--)
                s.Write(i, s.Read(i - 1));
            s.Write(left, tmp);
            return;
        }

        // General case: left rotate by k via 3-reversal (linear scans, cache-friendly, no GCD overhead)
        // [A|B] → Reverse(A), Reverse(B), Reverse(AB) → [B|A]
        Reverse(s, left, left + k - 1);
        Reverse(s, left + k, right);
        Reverse(s, left, right);
    }

    /// <summary>
    /// Reverses a subarray in-place using swaps.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Reverse<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int right)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (left < right)
        {
            s.Swap(left, right);
            left++;
            right--;
        }
    }
}


/// <summary>
/// 最適化していないRotate Merge Sortです。
/// 配列を再帰的に半分に分割し、それぞれをソートした後、分割統治型の再帰的インプレースマージ（大きい側の中央を取り、反対側をbinary searchし、rotateして左右を再帰）でマージします。
/// 安定ソートであり、外部バッファを使用せずに典型的にはO(n log² n)の性能を達成します。
/// 回転にはGCD-cycle（Juggling）アルゴリズムを使用し、swapではなくwriteのみで回転を行います。
/// <br/>
/// Non-Optimized Rotate Merge Sort.
/// Recursively divides the array in half, sorts each part, then merges sorted subarrays in-place using
/// divide-and-conquer rotation merge: pick the median of the larger side, binary search in the opposite side,
/// rotate, then recursively merge both halves.
/// Uses GCD-cycle (Juggling) rotation which performs writes only (no swaps).
/// This algorithm is stable and typically achieves O(n log² n) performance without requiring an external buffer.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Rotate Merge Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Hybrid Optimization:</strong> For subarrays with ≤16 elements, insertion sort is used instead of rotation-based merge.
/// This is a practical optimization similar to TimSort and IntroSort, reducing overhead for small sizes.</description></item>
/// <item><description><strong>Divide Step (Binary Partitioning):</strong> The array must be divided into two roughly equal halves at each recursion level.
/// The midpoint is calculated as mid = (left + right) / 2, ensuring balanced subdivision.
/// This guarantees a recursion depth of ⌈log₂(n)⌉.</description></item>
/// <item><description><strong>Base Case (Termination Condition):</strong> Recursion must terminate when a subarray has size ≤ 1.
/// An array of size 0 or 1 is trivially sorted and requires no further processing.</description></item>
/// <item><description><strong>Conquer Step (Recursive Sorting):</strong> Each half must be sorted independently via recursive calls.
/// The left subarray [left..mid] and right subarray [mid+1..right] are sorted before merging.</description></item>
/// <item><description><strong>Divide-and-Conquer In-Place Merge:</strong> Each merge picks the median of
/// the larger side, binary searches (lower_bound or upper_bound) for its position in the opposite side,
/// rotates the overlapping region, then recursively merges the two resulting sub-problems.
/// Picking the larger side guarantees progress (the median index is always ≥ 1).</description></item>
/// <item><description><strong>Rotation Algorithm (Left-Rotate by k, GCD-Cycle / Juggling):</strong> Left-rotates A[left..right] by k positions: [left_k_elems | rest] → [rest | left_k_elems] using GCD-based cycle detection.
/// To left-rotate array A of length n by k positions: Find GCD(n, k) independent cycles, and for each cycle, move elements using assignments only.
/// This achieves O(n) time rotation with O(1) space using only writes (no swaps needed).</description></item>
/// <item><description><strong>Stability Preservation:</strong> Lower bound is used when the left side is larger
/// and upper bound when the right side is larger, ensuring equal elements from the left run appear before
/// those from the right run.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Hybrid (Merge + Insertion)</description></item>
/// <item><description>Stable      : Yes (lower/upper bound preserves relative order)</description></item>
/// <item><description>Bufferless  : Yes (no external buffer, uses rotation for in-place merging)</description></item>
/// <item><description>Best case   : O(n) - Sorted data with insertion sort optimization for small partitions</description></item>
/// <item><description>Average case: O(n log² n) - Binary search (log n) + rotation (n) per merge level (log n levels)</description></item>
/// <item><description>Worst case  : O(n log² n) - Rotation adds O(n) factor to each merge operation</description></item>
/// <item><description>Comparisons : Best O(n), Average/Worst O(n log² n)</description></item>
/// <item><description>Writes      : Best O(n), Average/Worst O(n² log n) - GCD-cycle rotation uses assignments only (no swaps)</description></item>
/// <item><description>Swaps       : 0 - GCD-cycle rotation uses only write operations, no swaps needed</description></item>
/// <item><description>Space       : O(log n) - Recursion stack for sort + merge, no auxiliary buffer needed</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Merge_sort#Variants</para>
/// <para>Rotation-based in-place merge: Practical In-Place Merging (Geffert et al.)</para>
/// </remarks>
public static class RotateMergeSortNonOptimized
{
    // Threshold for using insertion sort instead of rotation-based merge
    // Small subarrays benefit from insertion sort's lower overhead
    private const int InsertionSortThreshold = 16;

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array (in-place operations only)

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

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        SortCore(s, 0, span.Length - 1);
    }

    /// <summary>
    /// Core recursive merge sort implementation.
    /// </summary>
    /// <param name="s">The SortSpan wrapping the span to sort</param>
    /// <param name="left">The inclusive start index of the range to sort</param>
    /// <param name="right">The inclusive end index of the range to sort</param>
    private static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int right)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (right <= left) return; // Base case: array of size 0 or 1 is sorted

        var length = right - left + 1;

        // Optimization: Use insertion sort for small subarrays
        // Rotation overhead is too high for small sizes, and insertion sort has better cache locality
        if (length <= InsertionSortThreshold)
        {
            // Reuse existing InsertionSort.SortCore
            // Note: SortCore uses exclusive end index [first, last), so we pass right + 1
            InsertionSort.SortCore(s, left, right + 1);
            return;
        }

        var mid = left + (right - left) / 2;

        // Conquer: Recursively sort left and right halves
        SortCore(s, left, mid);
        SortCore(s, mid + 1, right);

        // Optimization: Skip merge if already sorted (left[last] <= right[first])
        if (s.IsLessOrEqualAt(mid, mid + 1))
        {
            return; // Already sorted, no merge needed
        }

        // Merge: Combine two sorted halves in-place using rotation
        MergeInPlace(s, left, mid, right);
    }

    /// <summary>
    /// Merges two sorted subarrays [left..mid] and [mid+1..right] in-place using divide-and-conquer rotation.
    /// Picks the median of the larger side, binary searches for its position in the opposite side,
    /// rotates the overlapping region, then recursively merges both resulting sub-problems.
    /// </summary>
    private static void MergeInPlace<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int mid, int right)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var len1 = mid - left + 1;
        var len2 = right - mid;

        if (len1 == 0 || len2 == 0) return;

        // Already-sorted skip: left run's max ≤ right run's min → merge is a no-op.
        if (s.IsLessOrEqualAt(mid, mid + 1)) return;

        if (len1 == 1 && len2 == 1)
        {
            s.Swap(left, right);
            return;
        }

        int mid1, mid2;

        if (len1 > len2)
        {
            // Left side is larger: pick its median, lower_bound in right side
            mid1 = left + len1 / 2;
            mid2 = LowerBound(s, mid + 1, right, mid1);
        }
        else
        {
            // Right side is larger (or equal): pick its median, upper_bound in left side
            mid2 = mid + 1 + len2 / 2;
            mid1 = UpperBound(s, left, mid, mid2);
        }

        var rotateLen = mid - mid1 + 1;
        if (rotateLen > 0 && mid2 > mid + 1)
            Rotate(s, mid1, mid2 - 1, rotateLen);

        var newMid = mid1 + (mid2 - mid - 1);

        MergeInPlace(s, left, mid1 - 1, newMid - 1);
        MergeInPlace(s, newMid, newMid + (mid - mid1), right);
    }

    /// <summary>
    /// Finds the first position in [left..right] where s[pos] >= s[keyIndex] (lower bound).
    /// </summary>
    private static int LowerBound<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int right, int keyIndex)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var lo = left;
        var hi = right + 1;

        while (lo < hi)
        {
            var m = lo + (hi - lo) / 2;
            if (s.IsLessAt(m, keyIndex))
                lo = m + 1;
            else
                hi = m;
        }

        return lo;
    }

    /// <summary>
    /// Finds the first position in [left..right] where s[pos] > s[keyIndex] (upper bound).
    /// </summary>
    private static int UpperBound<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int right, int keyIndex)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var lo = left;
        var hi = right + 1;

        while (lo < hi)
        {
            var m = lo + (hi - lo) / 2;
            if (s.IsLessOrEqualAt(m, keyIndex))
                lo = m + 1;
            else
                hi = m;
        }

        return lo;
    }

    /// <summary>
    /// Rotates a subarray by k positions to the left using the GCD-cycle (Juggling) algorithm.
    /// This algorithm divides the rotation into GCD(n, k) independent cycles and moves elements
    /// within each cycle using assignments only (no swaps needed).
    /// </summary>
    /// <param name="s">The SortSpan wrapping the array</param>
    /// <param name="left">The start index of the subarray to rotate</param>
    /// <param name="right">The end index of the subarray to rotate</param>
    /// <param name="k">The number of positions to rotate left</param>
    private static void Rotate<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int left, int right, int k)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (k == 0 || left >= right) return;

        var n = right - left + 1;
        k = k % n;
        if (k == 0) return;

        // Left rotation via GCD-cycle (Juggling algorithm): [left_k_elems | rest] → [rest | left_k_elems]
        // Divide rotation into gcd(n, k) independent cycles
        var cycles = GCD(n, k);

        for (var cycle = 0; cycle < cycles; cycle++)
        {
            // Save the first element of this cycle
            var startIdx = left + cycle;
            var temp = s.Read(startIdx);
            var currentIdx = startIdx;

            // Move elements in this cycle
            while (true)
            {
                var nextIdx = currentIdx + k;
                if (nextIdx > right)
                    nextIdx = left + (nextIdx - right - 1);

                // If we've completed the cycle, break
                if (nextIdx == startIdx)
                    break;

                // Move element from nextIdx to currentIdx
                s.Write(currentIdx, s.Read(nextIdx));
                currentIdx = nextIdx;
            }

            // Place the saved element in its final position
            s.Write(currentIdx, temp);
        }
    }

    /// <summary>
    /// Calculates the greatest common divisor (GCD) of two numbers using Euclid's algorithm.
    /// </summary>
    /// <param name="a">First number</param>
    /// <param name="b">Second number</param>
    /// <returns>GCD of a and b</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GCD(int a, int b)
    {
        while (b != 0)
        {
            var temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }
}
