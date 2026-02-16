using SortAlgorithm.Contexts;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 弱ヒープ（Weak Heap）という特殊なヒープ構造を使用するソートアルゴリズムです。
/// 通常のヒープソートより比較回数が少なく、理論的には約 n log n - 0.9n 回の比較で済みます。
/// <br/>
/// Uses a special heap structure called a weak heap, which requires fewer comparisons than standard heap sort—
/// theoretically about n log n - 0.9n comparisons.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct WeakHeapSort:</strong></para>
/// <list type="number">
/// <item><description><strong>Weak Heap Property:</strong> A weak heap is a relaxed variant of a binary heap where:
/// - For each node i, all elements in its "left" subtree are ≤ a[i] (weak heap property)
/// - The "right" subtree has no ordering constraint relative to a[i]
/// - A reverse bit array r[i] determines which physical child is considered "left": if r[i]=false, left child is 2i; if r[i]=true, children roles are swapped
/// - This dynamic left/right assignment allows the structure to adapt and minimize comparisons</description></item>
/// <item><description><strong>Distinguished Ancestor Computation:</strong> For node j &gt; 0, the distinguished ancestor dAncestor(j) is:
/// - The ancestor whose "right" spine contains j
/// - Computed by: parent = j &gt;&gt; 1; while ((j &amp; 1) == r[parent]) { j = parent; parent = j &gt;&gt; 1; } return parent
/// - This ascends the tree while j's parity (odd/even) matches its parent's reverse bit</description></item>
/// <item><description><strong>Merge Operation:</strong> Merge(i, j) is the fundamental primitive:
/// - Compares a[i] and a[j] (one comparison only)
/// - If a[j] > a[i], swaps them and flips r[j]
/// - This single comparison maintains the weak heap property while adapting structure via reverse bits
/// - Contrast with standard heap: requires two comparisons (left vs. right child)</description></item>
/// <item><description><strong>Build Phase (Weakheapify):</strong> Constructs a max weak heap bottom-up:
/// - For j = n-1 down to 1: Merge(dAncestor(j), j)
/// - Each node is merged with its distinguished ancestor
/// - Runs in O(n) time with exactly n-1 comparisons
/// - After this phase, a[0] holds the maximum element (max weak heap root)</description></item>
/// <item><description><strong>Extract Phase:</strong> Repeatedly extracts the maximum element:
/// - For m = n-1 down to 2:
///   1. Swap(a[0], a[m]) — move current max to its final sorted position
///   2. j = 1; while (2j + r[j] &lt; m) j = 2j + r[j] — descend the distinguished path to a leaf
///   3. while (j &gt; 0) { Merge(0, j); j &gt;&gt;= 1 } — ascend back to root, merging root with path nodes
/// - Final step: Swap(a[0], a[1]) to sort the last two elements
/// - This process restores the weak heap property for the reduced heap [0..m-1] after each extraction</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Heap / Selection (optimized variant)</description></item>
/// <item><description>Stable      : No (element order changes arbitrarily during heap operations)</description></item>
/// <item><description>In-place    : Nearly (requires O(n) bits for reverse array ≈ n/64 ulongs = n/8 bytes overhead)</description></item>
/// <item><description>Best case   : Ω(n log n) - Heap construction and extraction always required regardless of input</description></item>
/// <item><description>Average case: Θ(n log n) - Build weak heap O(n) + extract phase O(n log n)</description></item>
/// <item><description>Worst case  : O(n log n) - Guaranteed upper bound for all inputs</description></item>
/// <item><description>Comparisons : ~n log n - 0.9n - Significantly fewer than standard heap sort's ~2n log n</description></item>
/// <item><description>Swaps       : ~n log n - Similar swap count to standard heap sort</description></item>
/// <item><description>Cache       : Poor locality - Non-sequential access pattern similar to heap sort, frequent cache misses</description></item>
/// </list>
/// <para><strong>Why Fewer Comparisons?:</strong></para>
/// <para>
/// Weak heaps achieve fewer comparisons through two key mechanisms:
/// 1. Only one comparison per merge operation (vs. two in standard heaps for left/right child selection)
/// 2. Reverse bits encode previous comparison outcomes, eliminating redundant comparisons during sift operations
/// The trade-off: additional space for reverse bits and slightly more complex bookkeeping
/// </para>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>Uses bit-packed ulong array for reverse bits: stackalloc for small arrays (≤1024 elements → 16 ulongs), ArrayPool for larger arrays</description></item>
/// <item><description>Bit packing reduces memory usage by 8x compared to bool[] (1 bit vs 8 bits per element)</description></item>
/// <item><description>Distinguished ancestor follows the right spine upward based on parity vs. parent's reverse bit</description></item>
/// <item><description>Merge is the only comparison operation; all other logic composes merges</description></item>
/// <item><description>Space overhead: n bits = n/8 bytes (e.g., 1000 elements → 125 bytes)</description></item>
/// <item><description>Final swap(0,1) is unconditional in classic implementations; here it's conditional to avoid redundant work</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Heapsort</para>
/// </remarks>
public static class WeakHeapSort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int STACKALLOC_THRESHOLD = 1024; // Use stackalloc for arrays <= 1024 elements (in bits: 128 bytes)

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

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        SortCore(s, first, n);
    }

    /// <summary>
    /// Core weak heap sort on range [offset..offset+n).
    /// Produces ascending order by building a max weak heap and extracting max elements to the end.
    /// </summary>
    private static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int offset, int n)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (n <= 1) return;

        // Calculate space needed for bit-packed reverse bits
        // Each ulong stores 64 bits, so we need (n + 63) / 64 ulongs
        var ulongCount = (n + 63) / 64;
        ulong[]? rentedArray = null;

        try
        {
            // Allocate reverse bits: r[i] indicates whether node i's children are swapped
            // r[0] is unused (root has no parent) but allocated for uniform indexing
            // Use bit packing: each ulong stores 64 bits, reducing memory by 8x vs bool[]
            Span<ulong> r = ulongCount <= STACKALLOC_THRESHOLD / 64
                ? stackalloc ulong[ulongCount]
                : (rentedArray = ArrayPool<ulong>.Shared.Rent(ulongCount)).AsSpan(0, ulongCount);
            r.Clear(); // Initialize all reverse bits to false

            // Phase 1: Build max weak heap (bottom-up merges)
            // After this, offset+0 contains the maximum element
            for (var j = n - 1; j > 0; j--)
            {
                var i = DistinguishedAncestor(j, r);
                Merge(s, offset, r, i, j);
            }

            // Phase 2: Extract max elements from n-1 down to 2
            // Each iteration moves the current maximum to its final position
            for (var m = n - 1; m >= 2; m--)
            {
                // Move current max (at offset+0) to position offset+m
                s.Swap(offset, offset + m);

                // Restore weak heap property for reduced heap [0..m-1]
                // Descend the distinguished path from node 1 to a leaf
                var x = 1;
                int y;
                while ((y = 2 * x + (GetBit(r, x) ? 1 : 0)) < m)
                {
                    x = y;
                }

                // Ascend from leaf to root, merging root with each node on the path
                while (x > 0)
                {
                    Merge(s, offset, r, 0, x);
                    x >>= 1;
                }
            }

            // Final step: Sort the last two elements (at positions offset+0 and offset+1)
            // Classic weak heap sort does an unconditional swap here; we use conditional to avoid redundant work
            if (n > 1 && s.Compare(offset + 1, offset) < 0)
            {
                s.Swap(offset, offset + 1);
            }
        }
        finally
        {
            if (rentedArray != null)
            {
                ArrayPool<ulong>.Shared.Return(rentedArray);
            }
        }
    }

    /// <summary>
    /// Merge two nodes in the weak heap, maintaining the max weak heap property.
    /// If a[j] > a[i], swaps them and flips the reverse bit r[j].
    /// This is the fundamental comparison operation of weak heap sort.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Merge<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int offset, Span<ulong> r, int i, int j)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (s.Compare(offset + j, offset + i) > 0)
        {
            s.Swap(offset + i, offset + j);
            FlipBit(r, j);
        }
    }

    /// <summary>
    /// Computes the distinguished ancestor of node j (where j > 0).
    /// The distinguished ancestor is the ancestor whose "right" spine contains j.
    /// Algorithm: Ascend while j's parity matches its parent's reverse bit.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int DistinguishedAncestor(int j, Span<ulong> r)
    {
        // Climb the tree while (j & 1) == r[parent]
        // parent = j >> 1
        // Important: We check r[parent], not r[j]
        while (j > 0)
        {
            var parent = j >> 1;
            var parentBit = GetBit(r, parent) ? 1 : 0;

            // If j's parity differs from parent's reverse bit, parent is the distinguished ancestor
            if ((j & 1) != parentBit)
                return parent;

            j = parent;
        }
        return 0; // Reached root (should not happen for j > 0, but safe fallback)
    }

    /// <summary>
    /// Gets a bit value from the bit-packed array at the specified index.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool GetBit(Span<ulong> bits, int index)
    {
        var ulongIndex = index / 64;
        var bitIndex = index % 64;
        return ((bits[ulongIndex] >> bitIndex) & 1) == 1;
    }

    /// <summary>
    /// Flips a bit in the bit-packed array at the specified index.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void FlipBit(Span<ulong> bits, int index)
    {
        var ulongIndex = index / 64;
        var bitIndex = index % 64;
        bits[ulongIndex] ^= 1UL << bitIndex;
    }
}
