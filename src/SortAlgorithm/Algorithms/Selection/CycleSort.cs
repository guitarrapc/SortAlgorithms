using SortAlgorithm.Contexts;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 各要素を最終位置に直接配置するサイクル検出アルゴリズムです。書き込み回数を理論的に最小化（最大n回）し、メモリ書き込みコストが高い環境で有用です。
/// 要素の正しい位置を計算し、そこにある要素と交換しながらサイクルを辿ります。
/// <br/>
/// A cycle-detection algorithm that places each element directly in its final position. Minimizes writes to the theoretical minimum (at most n writes), useful in environments with expensive memory write operations.
/// Calculates the correct position of each element and follows cycles by swapping displaced elements.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Cycle Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Cycle Detection:</strong> The algorithm must identify permutation cycles in the array.
/// Each element belongs to exactly one cycle. By placing an element in its correct position and following the displacement chain,
/// the algorithm completes each cycle without revisiting positions.</description></item>
/// <item><description><strong>Position Calculation:</strong> An element's final position is the cycle's start plus the number of elements after
/// the start that are smaller than it. Every element before the start is already final and no larger, so it needs no comparison.
/// This costs one comparison per element after the start each time an element is placed.</description></item>
/// <item><description><strong>Minimal Writes:</strong> An element already in its final position is never written; any other element is written once,
/// directly to its final position. Nothing is written merely to make room, which is the minimum number of writes for an in-place sort.</description></item>
/// <item><description><strong>Duplicate Handling:</strong> When the computed position already holds an equal element, place the element after the
/// equal ones instead. Exchanging it with an equal element would make the cycle repeat forever.</description></item>
/// <item><description><strong>Cycle Completion:</strong> Continue rotating elements within a cycle until returning to the starting position.
/// Once a cycle is completed, move to the next unprocessed position to start a new cycle.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Selection (though often classified separately due to unique cycle-based approach)</description></item>
/// <item><description>Stable      : No (relative order of equal elements is not preserved)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space, only constant extra variables)</description></item>
/// <item><description>Best case   : Θ(n²) - sorted input still counts once from every start position: n(n-1)/2 comparisons and no writes</description></item>
/// <item><description>Average case: Θ(n²) - every placed element costs another count over the range after its cycle's start</description></item>
/// <item><description>Worst case  : Θ(n²) - about 3n²/2 comparisons when one cycle runs through every element; at most n writes</description></item>
/// <item><description>Comparisons : Θ(n²) - n(n-1)/2 for the counts from each start, one more count for each element placed after a cycle's first, and for each placement one per equal element skipped plus the one that ends the skip</description></item>
/// <item><description>Swaps       : 0 - each element is carried in a held variable and dropped into its computed position; nothing is exchanged</description></item>
/// <item><description>Index Reads : Θ(n²) - each count reads every element after the cycle's start and each start reads its item; each placement reads the cells its duplicate skip compares and the cell it overwrites</description></item>
/// <item><description>Index Writes: O(n) - one per element not already in its final position, the minimum for an in-place sort</description></item>
/// <item><description>Space       : O(1) - only the held element and a few counters</description></item>
/// </list>
/// <para><strong>Use Cases:</strong></para>
/// <list type="bullet">
/// <item><description>Flash memory or EEPROM with limited write cycles</description></item>
/// <item><description>Systems where memory writes are significantly more expensive than comparisons</description></item>
/// <item><description>Educational purposes to demonstrate cycle detection in permutations</description></item>
/// </list>
/// <para><strong>Comparer contract:</strong></para>
/// <para>The duplicate skip stops before the end of the range, and the rotations take at most n steps in total, only with a consistent
/// comparer. Both are checked. A comparer that is not a consistent total order can make the sort throw
/// <see cref="InvalidOperationException"/>. Either way the range keeps its elements, and nothing outside it is read or written.</para>
/// <para><strong>History:</strong></para>
/// <para>Haddon (1990) named the method and its idea of placing each element by following cycles; his paper's title calls it
/// "A Linear Sorting Method". The form implemented here finds each element's position by counting the smaller elements, which costs
/// Θ(n²) comparisons. It is a later general form for arbitrary keys, not the linear-time method of the title.</para>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Cycle_sort</para>
/// <para>Paper: Haddon, B. K. (1990). "Cycle-Sort: A Linear Sorting Method". The Computer Journal 33 (4): 365-367. https://doi.org/10.1093/comjnl/33.4.365</para>
/// </remarks>
public static class CycleSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by CycleSortTests, which derives from SortTestsBase.</remarks>
    public static bool IsStable => false;

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array

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

        // A consistent comparer writes each element to its final cell at most once over the whole sort, so the rotations take at most n
        // steps in total. More steps mean the comparer is not consistent, and a rotation might never return to its start. The natural order
        // of a primitive type is consistent, so this check and the two below fold away for it.
        var steps = span.Length;

        for (var cycleStart = 0; cycleStart < span.Length - 1; cycleStart++)
        {
            context.OnPhase(SortPhase.CycleSortCycle, cycleStart, span.Length - 1);
            context.OnRole(cycleStart, BUFFER_MAIN, RoleType.LeftPointer);

            var item = s.Read(cycleStart);
            var pos = FindPosition(ref s, item, cycleStart);

            // If the item is already in the correct position, skip
            if (pos == cycleStart)
            {
                context.OnRole(cycleStart, BUFFER_MAIN, RoleType.None);
                continue;
            }

            // Skip duplicates
            // With a consistent comparer the skip stops before the end, because the positions from pos onwards outnumber the copies of the
            // item. Element access is unchecked, so a skip that reaches the end throws. Nothing has been written yet.
            pos = SkipDuplicates(ref s, item, pos);
            if (!s.UsesNaturalPrimitiveOrder && pos == s.Length) ThrowHelper.ThrowInconsistentComparer();

            // Put the item at its correct position
            var temp = s.Read(pos);
            s.Write(pos, item);
            item = temp;

            // Rotate the rest of the cycle
            while (pos != cycleStart)
            {
                if (!s.UsesNaturalPrimitiveOrder && --steps < 0) ThrowHelper.RestoreAndThrowInconsistentComparer(s, cycleStart, item);

                pos = FindPosition(ref s, item, cycleStart);
                pos = SkipDuplicates(ref s, item, pos);

                // The cell at cycleStart still holds a copy of the first item, so the held item goes there before throwing.
                if (!s.UsesNaturalPrimitiveOrder && pos == s.Length) ThrowHelper.RestoreAndThrowInconsistentComparer(s, cycleStart, item);

                temp = s.Read(pos);
                s.Write(pos, item);
                item = temp;
            }

            context.OnRole(cycleStart, BUFFER_MAIN, RoleType.None);
        }
    }

    /// <summary>
    /// Returns <paramref name="start"/> plus the number of elements after it that are smaller than <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// The count adds each comparison result instead of branching on it, a branch that random input mispredicts about half the time.
    /// The item is invariant during the count, so a floating-point item has its NaN state resolved once and each comparison is a single test.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int FindPosition<T, TComparer, TContext>(ref SortSpan<T, TComparer, TContext> s, T value, int start)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var count = new CountSmaller<T, TComparer, TContext> { Position = start };
        s.WithPreparedKey(value, ref count);
        return count.Position;
    }

    struct CountSmaller<T, TComparer, TContext> : IPreparedKeyLoop<T, TComparer, TContext>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // A linear scan against an invariant key, like a partition.
        public static PreparedKeyKind Kind => PreparedKeyKind.Partition;
        public int Position;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Run<TOrder>(SortSpan<T, TComparer, TContext> s, T item)
            where TOrder : struct, IKeyOrder
        {
            var start = Position;
            var pos = start;
            for (var i = start + 1; i < s.Length; i++)
            {
                pos += s.IsElementLessThan<TOrder>(i, item) ? 1 : 0;
            }
            Position = pos;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int SkipDuplicates<T, TComparer, TContext>(ref SortSpan<T, TComparer, TContext> s, T value, int pos)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (pos < s.Length && s.Compare(value, pos) == 0)
        {
            pos++;
        }
        return pos;
    }
}
