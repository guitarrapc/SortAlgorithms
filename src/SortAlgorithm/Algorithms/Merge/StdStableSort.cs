using System.Buffers;
using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// LLVM libcxx の std::stable_sort に相当する再帰的トップダウンマージソートです。
/// O(n) の補助領域を確保し、ソート結果とバッファを交互に使用するピンポン型マージで安定性を保ちます。
/// <br/>
/// A recursive top-down merge sort equivalent to LLVM libcxx's std::stable_sort.
/// Allocates O(n) auxiliary space and preserves stability via ping-pong merging between the main span and the buffer.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct std::stable_sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Full-Size Buffer Allocation:</strong> A temporary buffer of exactly n elements is allocated upfront.
/// Because the buffer size always equals the subarray length at every recursion level, the algorithm always uses
/// the buffer-assisted path (no in-place merge fallback is needed).</description></item>
/// <item><description><strong>Ping-Pong Between Two Roles:</strong> The algorithm alternates between two recursive routines.
/// StableSort(s, b) sorts s in-place using b as scratch, while StableSortMove(s, b) sorts s and writes the
/// sorted result into b. The two routines call each other so that each level's output naturally lands in the
/// correct buffer without an extra copy.</description></item>
/// <item><description><strong>StableSort(s, b) — Output to s:</strong> Divides s into two halves.
/// StableSortMove is called for each half (writing sorted halves into the corresponding regions of b).
/// The two sorted b-halves are then merged back into s via MergeIntoS.</description></item>
/// <item><description><strong>StableSortMove(s, b) — Output to b:</strong> Divides s into two halves.
/// StableSort is called for each half (sorting each s-half in-place using the b-half as scratch).
/// The two sorted s-halves are then merged into b via MergeIntoB.</description></item>
/// <item><description><strong>Insertion Sort Base Case (len ≤ 8):</strong> Inside StableSortMove, when the subarray is
/// small enough, InsertionSortMove writes the sorted result directly into b without further recursion.
/// This mirrors the threshold used in LLVM's __stable_sort_move.</description></item>
/// <item><description><strong>Stability Preservation:</strong> All merge steps take from the left half when elements are
/// equal (IsLessOrEqual rather than IsLessThan), preserving the relative order of equal elements.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Merge</description></item>
/// <item><description>Stable      : Yes (equal elements maintain relative order)</description></item>
/// <item><description>In-place    : No (requires O(n) auxiliary space)</description></item>
/// <item><description>Best case   : O(n log n)</description></item>
/// <item><description>Average case: O(n log n)</description></item>
/// <item><description>Worst case  : O(n log n)</description></item>
/// <item><description>Space       : O(n) auxiliary buffer + O(log n) stack for recursion</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>LLVM libcxx: https://github.com/llvm/llvm-project/blob/llvmorg-22.1.2/libcxx/include/__algorithm/stable_sort.h</para>
/// </remarks>
public static class StdStableSort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_MERGE = 1;      // Merge buffer (auxiliary space)

    // Threshold for insertion sort inside StableSortMove. LLVM's __stable_sort_move threshold.
    private const int InsertionSortThreshold = 8;

    // Threshold for using insertion sort in StableSort to avoid O(n) buffer allocation and copying overhead for small arrays. LLVM threshold is 128 for trivially-copy-assignable types, but we use a smaller fixed threshold here since C# cannot detect that at compile time.
    private const int InPlaceThreshold = 16;

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

        // fall-back for small arrays to avoid O(n) buffer allocation and copying overhead.
        if (span.Length <= InPlaceThreshold)
        {
            InsertionSort.Sort(span, comparer, context);
            return;
        }

        // Rent buffer from ArrayPool for O(n) auxiliary space
        var buffer = ArrayPool<T>.Shared.Rent(span.Length);
        try
        {
            var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
            var b = new SortSpan<T, TComparer, TContext>(buffer.AsSpan(0, span.Length), context, comparer, BUFFER_MERGE);
            StableSort(s, b);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(buffer, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    /// <summary>
    /// Corresponds to __stable_sort with len &lt;= buff_size path (always taken here).
    /// Sorts s in-place using b as auxiliary scratch space.
    /// <list type="number">
    /// <item><description>StableSortMove(sLeft, bLeft) → bLeft sorted</description></item>
    /// <item><description>StableSortMove(sRight, bRight) → bRight sorted</description></item>
    /// <item><description>MergeIntoS(b, l2, s) → s sorted</description></item>
    /// </list>
    /// </summary>
    private static void StableSort<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> b)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var len = s.Length;
        switch (len)
        {
            case <= 1:
                return;
            case 2:
                if (s.IsLessAt(1, 0))
                    s.Swap(0, 1);
                return;
        }

        var l2 = len / 2;
        var sLeft = s.Slice(0, l2, BUFFER_MAIN);
        var bLeft = b.Slice(0, l2, BUFFER_MERGE);
        var sRight = s.Slice(l2, len - l2, BUFFER_MAIN);
        var bRight = b.Slice(l2, len - l2, BUFFER_MERGE);

        StableSortMove(sLeft, bLeft);         // bLeft sorted
        StableSortMove(sRight, bRight);       // bRight sorted
        s.Context.OnPhase(SortPhase.StdStableSortSort, s.Offset, s.Offset + l2, s.Offset + len - 1);
        s.Context.OnRole(s.Offset, BUFFER_MERGE, RoleType.LeftPointer);
        s.Context.OnRole(s.Offset + l2, BUFFER_MERGE, RoleType.RightPointer);
        MergeIntoS(b, l2, s);           // merge b[0..l2) + b[l2..len) → s
        s.Context.OnRole(s.Offset, BUFFER_MERGE, RoleType.None);
        s.Context.OnRole(s.Offset + l2, BUFFER_MERGE, RoleType.None);
    }

    /// <summary>
    /// Corresponds to __stable_sort_move.
    /// Sorts s and writes the sorted result into b (ping-pong counterpart of StableSort).
    /// <list type="number">
    /// <item><description>len &lt;= InsertionSortThreshold → InsertionSortMove(s, b)</description></item>
    /// <item><description>Otherwise: StableSort(sLeft, bLeft) + StableSort(sRight, bRight) to sort s-halves in-place,
    ///   then MergeIntoB(s, l2, b) to write sorted result into b.</description></item>
    /// </list>
    /// </summary>
    private static void StableSortMove<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> b)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var len = s.Length;
        switch (len)
        {
            case 0:
                return;
            case 1:
                b.Write(0, s.Read(0));
                return;
            case 2:
                var v0 = s.Read(0);
                var v1 = s.Read(1);
                if (s.IsLessThan(v1, v0))
                {
                    b.Write(0, v1);
                    b.Write(1, v0);
                }
                else
                {
                    b.Write(0, v0);
                    b.Write(1, v1);
                }
                return;
        }

        if (len <= InsertionSortThreshold)
        {
            InsertionSortMove(s, b);
            return;
        }

        var l2 = len / 2;
        var sLeft = s.Slice(0, l2, BUFFER_MAIN);
        var bLeft = b.Slice(0, l2, BUFFER_MERGE);
        var sRight = s.Slice(l2, len - l2, BUFFER_MAIN);
        var bRight = b.Slice(l2, len - l2, BUFFER_MERGE);

        StableSort(sLeft, bLeft);         // sLeft sorted in-place
        StableSort(sRight, bRight);       // sRight sorted in-place
        s.Context.OnPhase(SortPhase.StdStableSortMove, s.Offset, s.Offset + l2, s.Offset + len - 1);
        s.Context.OnRole(s.Offset, BUFFER_MAIN, RoleType.LeftPointer);
        s.Context.OnRole(s.Offset + l2, BUFFER_MAIN, RoleType.RightPointer);
        MergeIntoB(s, l2, b);       // merge s[0..l2) + s[l2..len) → b
        s.Context.OnRole(s.Offset, BUFFER_MAIN, RoleType.None);
        s.Context.OnRole(s.Offset + l2, BUFFER_MAIN, RoleType.None);
    }

    /// <summary>
    /// Corresponds to __insertion_sort_move.
    /// Performs insertion sort on s and writes the sorted result into b.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void InsertionSortMove<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> b)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var len = s.Length;
        b.Write(0, s.Read(0));

        for (var i = 1; i < len; i++)
        {
            var tmp = s.Read(i);
            var j = i - 1;
            if (s.IsLessThan(tmp, b.Read(j)))
            {
                b.Write(i, b.Read(j));
                j--;
                while (j >= 0 && s.IsLessThan(tmp, b.Read(j)))
                {
                    b.Write(j + 1, b.Read(j));
                    j--;
                }
                b.Write(j + 1, tmp);
            }
            else
            {
                b.Write(i, tmp);
            }
        }
    }

    /// <summary>
    /// Corresponds to __merge_move_construct.
    /// Merges sorted s[0..l2) and s[l2..len) into b[0..len).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void MergeIntoB<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int l2, SortSpan<T, TComparer, TContext> b)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var len = s.Length;
        var l = 0;
        var r = l2;
        var k = 0;

        while (l < l2 && r < len)
        {
            var lv = s.Read(l);
            var rv = s.Read(r);
            if (s.IsLessOrEqual(lv, rv))
            {
                b.Write(k, lv);
                l++;
            }
            else
            {
                b.Write(k, rv);
                r++;
            }
            k++;
        }

        if (l < l2)
            s.CopyTo(l, b, k, l2 - l);
        else if (r < len)
            s.CopyTo(r, b, k, len - r);
    }

    /// <summary>
    /// Corresponds to __merge_move_assign.
    /// Merges sorted b[0..l2) and b[l2..len) into s[0..len).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void MergeIntoS<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> b, int l2, SortSpan<T, TComparer, TContext> s)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var len = s.Length;
        var l = 0;
        var r = l2;
        var k = 0;

        while (l < l2 && r < len)
        {
            var lv = b.Read(l);
            var rv = b.Read(r);
            if (b.IsLessOrEqual(lv, rv))
            {
                s.Write(k, lv);
                l++;
            }
            else
            {
                s.Write(k, rv);
                r++;
            }
            k++;
        }

        if (l < l2)
            b.CopyTo(l, s, k, l2 - l);
        else if (r < len)
            b.CopyTo(r, s, k, len - r);
    }
}
