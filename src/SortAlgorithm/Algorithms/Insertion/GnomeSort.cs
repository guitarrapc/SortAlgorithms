using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// ノームソートアルゴリズム、位置を記憶する最適化版。
/// 引き返し始めた位置を記憶し、鉢を置いた後はその位置へ直接戻ることで、並べ終えた区間を歩き直す比較を省きます。この最適化により、比較回数は挿入ソートと同じになります。
/// ノームソートは「庭師のソート (Stupid sort)」とも呼ばれ、庭師が鉢植えを一つずつ並べ替えるように動作します。
/// <br/>
/// Gnome sort algorithm - optimized version with position memory.
/// The gnome remembers where it turned back and, after placing a pot, returns there directly instead of re-walking the pairs it has just ordered. With this optimization it makes the same comparisons as insertion sort.
/// Gnome sort, also known as "Stupid sort," works like a garden gnome arranging flower pots one by one.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Gnome Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Adjacent-only Comparison:</strong> The algorithm must only compare adjacent elements (i-1 and i).
/// This ensures the "step-by-step" nature of the gnome's movement and maintains stability.</description></item>
/// <item><description><strong>Sequential Swapping (Not Shifting):</strong> Elements must be moved via sequential swaps of adjacent pairs, not via shift operations.
/// This is the essential characteristic that distinguishes Gnome Sort from Insertion Sort. Using shift operations would transform it into Insertion Sort.</description></item>
/// <item><description><strong>Backward Movement on Disorder:</strong> When an out-of-order pair is found (arr[i-1] &gt; arr[i]), the algorithm must swap them and move backward (i--).
/// It continues moving backward until either reaching the beginning (i == 0) or finding a correctly ordered pair.</description></item>
/// <item><description><strong>Position Memory:</strong> The gnome remembers the position it turned back from. Once the pot it carries is in
/// place, it returns to that position instead of stepping back up over the pairs it has just put in order. The forward steps of the plain
/// walk, one comparison each, disappear, and the comparisons fall to the level of insertion sort. The walk is then the same as swap-based
/// insertion sort (<see cref="InsertionSortNonOptimized"/>).</description></item>
/// <item><description><strong>Stability Preservation:</strong> Uses strict inequality (&gt;) rather than (≥) when comparing elements.
/// This ensures that equal elements maintain their original relative order, making it a stable sort.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Insertion</description></item>
/// <item><description>Stable      : Yes (strict inequality preserves relative order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space)</description></item>
/// <item><description>Best case   : O(n) - Already sorted, only n-1 comparisons needed</description></item>
/// <item><description>Average case: O(n²) - Approximately n²/4 comparisons and swaps</description></item>
/// <item><description>Worst case  : O(n²) - Reverse sorted, exactly n(n-1)/2 comparisons and n(n-1)/2 swaps</description></item>
/// <item><description>Comparisons : Best: n-1 | Average: ~n²/4 | Worst: n(n-1)/2 (one per swap, plus one per pot that stops above the front)</description></item>
/// <item><description>Swaps       : Best: 0 | Average: ~n²/4 | Worst: n(n-1)/2</description></item>
/// <item><description>Index Reads : Best: 2(n-1) | Average/Worst: O(n²) (each comparison reads both neighbours, and each swap reads its two operands)</description></item>
/// <item><description>Index Writes: Best: 0 | Average: ~n²/2 | Worst: n(n-1) (2 writes per swap)</description></item>
/// <item><description>Space       : O(1) - the gnome's position and the position it remembers</description></item>
/// </list>
/// <para><strong>Comparer contract:</strong></para>
/// <para>Each walk back ends at the front at the latest, so the sort finishes whatever the comparer answers. The range keeps its elements,
/// and nothing outside it is read or written.</para>
/// <para><strong>History:</strong></para>
/// <para>The method is older than its names. Michael Singer's "PDP-11 Assembler Language Programming and Machine Organization"
/// (1980, §2.3) describes it exactly: compare each number with its successor, and if they are out of order, "interchange the two
/// numbers, and move one step to the left". Hamid Sarbazi-Azad published it as "Stupid Sort" in 2000, and Dick Grune named it
/// gnome sort.</para>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Gnome_sort</para>
/// <para>Singer, M. (1980). PDP-11 Assembler Language Programming and Machine Organization. Wiley. https://archive.org/details/pdp11assemblerlanguageprogrammingandmachineorganization/page/n60/mode/1up</para>
/// </remarks>
public static class GnomeSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by GnomeSortTests, which derives from StableSortTestsBase.</remarks>
    public static bool IsStable => true;

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
    /// Sorts the elements in the specified span using a custom comparer and sort context.
    /// This is the full-control version with explicit TContext type parameter.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of the comparer. Must implement <see cref="IComparer{T}"/>.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="comparer">The comparer used to determine the order of elements.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation.</param>
    public static void Sort<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);

        // `bound` is the remembered position. The gnome carries the pot at `bound` down to its place, then returns to `bound + 1`
        // instead of stepping back up over the pairs it has just put in order.
        for (var bound = 1; bound < s.Length; bound++)
        {
            context.OnPhase(SortPhase.GnomePass, bound, s.Length - 1);
            for (var i = bound; i > 0 && s.IsGreaterAt(i - 1, i); i--)
            {
                s.Swap(i - 1, i);
            }
        }
    }
}

/// <summary>
/// ノームソートアルゴリズム - 単純なwhileループ実装。
/// 最も基本的なノームソートの実装で、位置記憶などの最適化は行いません。実装は非常にシンプルですが、パフォーマンスは劣ります。
/// ノームソートは「庭師のソート (Stupid sort)」とも呼ばれ、庭師が鉢植えを一つずつ並べ替えるように動作します。
/// <br/>
/// Gnome sort algorithm - simple while loop implementation.
/// The most basic gnome sort implementation without optimizations like position memory. Very simple implementation but with inferior performance.
/// Gnome sort, also known as "Stupid sort," works like a garden gnome arranging flower pots one by one.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Gnome Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Adjacent-only Comparison:</strong> The algorithm must only compare adjacent elements (i-1 and i).
/// This ensures the "step-by-step" nature of the gnome's movement and maintains stability.</description></item>
/// <item><description><strong>Sequential Swapping (Not Shifting):</strong> Elements must be moved via sequential swaps of adjacent pairs, not via shift operations.
/// This is the essential characteristic that distinguishes Gnome Sort from Insertion Sort. Using shift operations would transform it into Insertion Sort.</description></item>
/// <item><description><strong>Backward Movement on Disorder:</strong> When an out-of-order pair is found (arr[i-1] &gt; arr[i]), the algorithm must swap them and move backward (i--).
/// It continues moving backward until either reaching the beginning (i == 0) or finding a correctly ordered pair.</description></item>
/// <item><description><strong>No Position Memory (Non-optimized):</strong> This basic version does not remember the previous position.
/// After backtracking to the beginning, it restarts from position 1, leading to redundant comparisons and reduced efficiency.</description></item>
/// <item><description><strong>Stability Preservation:</strong> Uses strict inequality (&gt;) rather than (≥) when comparing elements.
/// This ensures that equal elements maintain their original relative order, making it a stable sort.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Insertion</description></item>
/// <item><description>Stable      : Yes (strict inequality preserves relative order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space)</description></item>
/// <item><description>Best case   : Ω(n) - Already sorted, only n-1 comparisons needed</description></item>
/// <item><description>Average case: Θ(n²) - Approximately n²/4 comparisons and swaps</description></item>
/// <item><description>Worst case  : O(n²) - Reverse sorted, exactly n(n-1)/2 swaps and 2×n(n-1)/2 = n(n-1) comparisons</description></item>
/// <item><description>Comparisons : Best: n-1 | Average: ~n²/2 | Worst: n(n-1) (each swap requires 2 compares: one before swap, one in while condition)</description></item>
/// <item><description>Swaps       : Best: 0 | Average: ~n²/4 | Worst: n(n-1)/2</description></item>
/// <item><description>Index Reads : Best: 2(n-1) | Average/Worst: O(n²) (each comparison reads both neighbours, and each swap reads its two operands)</description></item>
/// <item><description>Index Writes: Best: 0 | Average: ~n²/2 | Worst: n(n-1) (2 writes per swap)</description></item>
/// <item><description>Space       : O(1) - the gnome walks the array with a single position variable</description></item>
/// </list>
/// <para><strong>Comparer contract:</strong></para>
/// <para>Each swap exchanges an adjacent inverted pair, so under a consistent comparer the sort makes at most n(n - 1) / 2 swaps. Without
/// position memory nothing else ends the walk, so a comparer that drives the sort past that makes it throw
/// <see cref="InvalidOperationException"/> instead of walking forever. The natural order of a primitive type is always consistent, and
/// the count is compiled out for it. Either way the range keeps its elements, and nothing outside it is read or written.</para>
/// </remarks>
public static class GnomeSortNonOptimized
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by GnomeSortNonOptimizedTests, which derives from StableSortTestsBase.</remarks>
    public static bool IsStable => true;

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

    public static void Sort<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);

        // As in GnomeSort, a consistent comparer allows at most n(n - 1) / 2 swaps, and the count folds away for the natural order of a
        // primitive type.
        var checkProgress = !s.UsesNaturalPrimitiveOrder;
        var swapBudget = (long)s.Length * (s.Length - 1) / 2;
        var i = 0;
        while (i < s.Length)
        {
            context.OnPhase(SortPhase.GnomePass, i, s.Length - 1);
            if (i == 0 || s.IsLessOrEqualAt(i - 1, i))
            {
                i++;
            }
            else
            {
                if (checkProgress && --swapBudget < 0) ThrowHelper.ThrowInconsistentComparer();
                s.Swap(i, i - 1);
                --i;
            }
        }
    }
}
