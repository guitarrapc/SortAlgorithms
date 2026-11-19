using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// ポプラ（poplar）と呼ばれる完全二分ヒープの森を配列上に作り、最大値の取り出しを繰り返すヒープソートの変種。
/// ポプラは「2 つの部分ポプラ（同じ大きさ）の後ろに根を置いた」連続区間で、大きさは常に 2^k - 1 です。
/// 森は大きいポプラから順に並び、各ポプラは取れる限り大きく取るので、要素数だけで森の形が決まります。
/// そのため位置を保存せず、O(1) の補助領域で全操作を行えます。
/// <br/>
/// A heapsort variant that lays a forest of perfect binary heaps, called poplars, over the array and repeatedly extracts the maximum.
/// A poplar is a contiguous section holding two equal-sized subpoplars followed by their root, so its size is always 2^k - 1.
/// The forest stores the bigger poplars first and makes each one as big as it can be, so the element count alone determines its shape: no poplar positions are stored, and every operation runs in O(1) auxiliary space.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Poplar Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Poplar:</strong> A perfect max-heap mapped onto a contiguous section as [left subpoplar][right subpoplar][root].
/// For a poplar of size s starting at f, the root is at f + s - 1, the right subpoplar's root at f + s - 2, and the left subpoplar's root at f + s/2 - 1. A single element is a poplar.</description>
/// </item>
/// <item><description><strong>Poplar Heap:</strong> The poplars are ordered from biggest to smallest and each is the largest 2^k - 1 that fits the elements remaining, so only the two rightmost poplars can share a size and there are at most ⌊log2(n + 1)⌋ + 1 of them.
/// A sorted sequence already satisfies this.</description></item>
/// <item><description><strong>Sift:</strong> A semipoplar is a poplar whose root may be smaller than a subpoplar root. Swapping the root with the bigger subpoplar root and repeating in that subpoplar turns it into a poplar in O(log n).</description></item>
/// <item><description><strong>Build Phase:</strong> Elements join one at a time as single-element poplars. Whenever the two poplars preceding the next element have the same size, that element becomes their root and is sifted.
/// The number of such merges after the k-th element is the exponent of the highest power of two dividing k (the binary carry sequence, OEIS A007814).</description></item>
/// <item><description><strong>Extraction Phase:</strong> The maximum is the biggest poplar root. It is found by walking the O(log n) roots, swapped with the root of the last poplar, and the poplar it came from is sifted.
/// The last element is then final and the heap shrinks by one; the poplar it was the root of splits into its two subpoplars without any work.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Heap (Forest of poplars)</description></item>
/// <item><description>Stable      : No (sift and root exchanges move equal keys past each other)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space)</description></item>
/// <item><description>Best case   : Ω(n log n) - building a sorted input is linear, but every extraction still walks the O(log n) poplar roots</description></item>
/// <item><description>Average case: Θ(n log n)</description></item>
/// <item><description>Worst case  : O(n log n)</description></item>
/// <item><description>Comparisons : O(n log n) - two per sift level, plus one per poplar root walked during each extraction</description></item>
/// <item><description>Swaps       : 0 - the sift and the root exchange move elements through a hole</description></item>
/// <item><description>Index Reads : O(n log n) - two per comparison of two roots, one per comparison with the held element, one per moved element</description></item>
/// <item><description>Index Writes: Best 0 (sorted), Average/Worst O(n log n) - one per sift level that moves, plus one per sift that moved and two per root exchange</description></item>
/// <item><description>Space       : O(1) - the forest occupies the input span and its shape is recomputed from the element count</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>The build phase uses Morwenn's iterative construction driven by the binary carry sequence, with single-element
/// base poplars. The reference implementation starts from 15-element poplars built by insertion sort; that is a hybrid stage
/// outside the algorithm's definition and is not used here.</description></item>
/// <item><description>The reference's build loop stops merging only when the merged poplar would exceed the whole collection
/// (<c>poplar_size &gt; size</c>). That bound does not check that the new root exists: with single-element base poplars it
/// sifts a root past the end from n = 13, and with the reference's 15-element base from n = 125. This implementation
/// stops when no element is left to serve as the root.</description></item>
/// <item><description>Poplar sizes come from <see cref="BitOperations.Log2(uint)"/>, so the first poplar of a heap is found in O(1)
/// and walking all roots in one extraction is O(log n).</description></item>
/// <item><description>The sift holds the sinking element aside and moves the bigger subpoplar roots up into the hole it leaves,
/// writing the element once where it stops; the root exchange of an extraction does the same. The comparisons and their order
/// are those of the swapping sift.</description></item>
/// </list>
/// <para><strong>Difference from SmoothSort:</strong></para>
/// <list type="bullet">
/// <item><description>Trees are perfect binary heaps of size 2^k - 1 instead of Leonardo trees, so their positions follow from n
/// and need no bit vector of tree sizes.</description></item>
/// <item><description>The roots are not kept in sorted order. Each extraction scans them instead, which is why the best case is
/// Ω(n log n) rather than smoothsort's Θ(n), although a sorted input still moves no element.</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Bron, C.; Hesselink, W. H. (1991). "Smoothsort revisited". Information Processing Letters 39 (5): 269-276.</para>
/// <para>Morwenn. poplar-heap: https://github.com/Morwenn/poplar-heap</para>
/// </remarks>
public static class PoplarHeapSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by PoplarHeapSortTests, which derives from SortTestsBase.</remarks>
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
        => Sort(span, 0, span.Length, new ComparableComparer<T>(), NullContext.Default);

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
        => Sort(span, 0, span.Length, new ComparableComparer<T>(), context);

    /// <summary>
    /// Sorts the elements in the specified span using the provided comparer and sort context.
    /// </summary>
    public static void Sort<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        => Sort(span, 0, span.Length, comparer, context);

    /// <summary>
    /// Sorts the subrange [first..last) using the default comparer and the provided sort context.
    /// </summary>
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

        if (last - first <= 1) return;

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);

        s.Context.OnPhase(SortPhase.HeapBuild, s.Offset + first, s.Offset + last - 1);
        MakeHeap(s, first, last);

        SortHeap(s, first, last);
    }

    /// <summary>
    /// Turns [first..last) into a poplar heap.
    /// Each outer iteration adds one element as a single-element poplar, then merges as many times as the
    /// binary carry sequence says: after the k-th such poplar, the two poplars before the next element have
    /// the same size TrailingZeroCount(k) times in a row, and each time that element becomes their root.
    /// </summary>
    private static void MakeHeap<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var poplarLevel = 1;  // number of single-element poplars added so far
        var it = first;       // start of the poplar that was just completed
        var next = first + 1; // next element to join the heap

        while (true)
        {
            var poplarSize = 1u;
            for (var merges = BitOperations.TrailingZeroCount(poplarLevel); merges > 0; merges--)
            {
                // No element left to become the root: every element already belongs to a poplar.
                if (next == last) break;

                // The poplar before `it` has the same size, so [it - poplarSize .. next] is a semipoplar rooted at `next`.
                it -= (int)poplarSize;
                poplarSize = 2 * poplarSize + 1;
                Debug.Assert(it >= first, $"MakeHeap: semipoplar start {it} precedes first {first}");
                Sift(s, it, poplarSize);
                next++;
            }

            // At most one element remains, and a single element is a poplar on its own.
            if (last - next <= 1) return;

            it = next;
            next++;
            poplarLevel++;
        }
    }

    /// <summary>
    /// Repeatedly moves the maximum of the poplar heap [first..last) to its end and shrinks the heap by one.
    /// </summary>
    private static void SortHeap<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var size = (uint)(last - first);
        var totalExtractions = (int)size - 1;
        var step = 1;

        var poplarSize = FirstPoplarSize(size);
        do
        {
            s.Context.OnPhase(SortPhase.HeapExtract, step++, totalExtractions);
            PopHeap(s, first, last, size, poplarSize);
            last--;
            size--;
            poplarSize = FirstPoplarSize(size);
        } while (size > 1);
    }

    /// <summary>
    /// Moves the maximum of the poplar heap [first..last) to last - 1.
    /// The maximum is the biggest poplar root; it is exchanged with the root of the last poplar,
    /// and the poplar that received the smaller root is sifted.
    /// </summary>
    /// <param name="size">Number of elements in [first..last).</param>
    /// <param name="poplarSize">Size of the first poplar, FirstPoplarSize(size).</param>
    private static void PopHeap<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, int last, uint size, uint poplarSize)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var lastRoot = last - 1;
        var bigger = lastRoot;
        var biggerSize = poplarSize;

        // Walk the poplar roots from the biggest poplar to the last one
        var it = first;
        while (true)
        {
            var root = it + (int)poplarSize - 1;
            if (root == lastRoot) break;
            if (s.IsLessAt(bigger, root))
            {
                bigger = root;
                biggerSize = poplarSize;
            }
            it = root + 1;
            size -= poplarSize;
            poplarSize = FirstPoplarSize(size);
        }

        s.Context.OnRole(s.Offset + bigger, s.BufferId, RoleType.CurrentMax);

        if (bigger != lastRoot)
        {
            // Exchange the two roots and sift the poplar that received the smaller one. That element is held
            // aside while it sinks, so the exchange costs one write at each end instead of a swap.
            var value = s.Read(lastRoot);
            s.Write(lastRoot, s.Read(bigger));
            s.Write(SiftHole(s, bigger, biggerSize, value), value);
        }

        s.Context.OnRole(s.Offset + bigger, s.BufferId, RoleType.None);
    }

    /// <summary>
    /// Turns the semipoplar [first..first+size) into a poplar.
    /// While a subpoplar root is bigger than the root, swap the root with the bigger subpoplar root
    /// and continue in that subpoplar, whose size is size / 2.
    /// </summary>
    /// <remarks>
    /// The root's element is held aside while bigger subpoplar roots move up into the hole it leaves, and it is
    /// written once where it stops, or not at all when it does not move. The comparisons are those of the swaps.
    /// </remarks>
    private static void Sift<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int first, uint size)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (size < 2) return;

        var root = first + (int)size - 1;
        var value = s.Read(root);
        var hole = SiftHole(s, root, size, value);
        if (hole != root) s.Write(hole, value);
    }

    /// <summary>
    /// Sifts <paramref name="value"/> into the poplar of <paramref name="size"/> elements whose root position
    /// <paramref name="root"/> is a hole, and returns the position where <paramref name="value"/> belongs.
    /// The caller writes it there.
    /// </summary>
    /// <remarks>
    /// <paramref name="value"/> stands in for the root, so each level compares it with the right subpoplar root,
    /// then the larger of the two with the left subpoplar root, as the swapping sift does.
    /// </remarks>
    private static int SiftHole<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int root, uint size, T value)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var hole = root;
        while (size >= 2)
        {
            // The subpoplars of the poplar rooted at the hole span [hole - size + 1 .. hole - 1]
            var rightRoot = hole - 1;
            var leftRoot = hole - (int)(size - size / 2);

            int maxRoot;
            if (s.IsValueLessThan(value, rightRoot))
                maxRoot = s.IsLessAt(rightRoot, leftRoot) ? leftRoot : rightRoot;
            else if (s.IsValueLessThan(value, leftRoot))
                maxRoot = leftRoot;
            else
                return hole;

            s.Write(hole, s.Read(maxRoot));
            hole = maxRoot;
            size /= 2;
        }
        return hole;
    }

    /// <summary>
    /// Size of the first (biggest) poplar of a poplar heap holding <paramref name="size"/> elements:
    /// the largest 2^k - 1 that does not exceed it. Returns 0 for 0.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint FirstPoplarSize(uint size) => (1u << BitOperations.Log2(size + 1)) - 1;
}
