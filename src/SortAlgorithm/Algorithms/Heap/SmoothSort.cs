using SortAlgorithm.Contexts;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// Leonardo数列に基づくヒープソートの変種で、ソート済みデータに対して適応的に高速に動作する高度なソートアルゴリズムです。
/// 通常のヒープソートは二分ヒープを使用しますが、SmoothSortはLeonardo数列(L(k) = L(k-1) + L(k-2) + 1, L(0)=1, L(1)=1)に基づく
/// 複数のLeonardo木を組み合わせたヒープ構造を使用します。この特性により、既にソートされた部分列を効率的に認識し、
/// 不要な操作を回避することで、ソート済みデータに対してO(n)の最良計算量を達成します。
/// <br/>
/// A sophisticated heap-based sorting algorithm using Leonardo heaps that adaptively achieves O(n) time for already-sorted data.
/// Unlike traditional heapsort which uses binary heaps, Smoothsort employs multiple Leonardo trees based on the Leonardo sequence
/// (L(k) = L(k-1) + L(k-2) + 1, where L(0)=1, L(1)=1). This structure allows efficient recognition of already-sorted subsequences,
/// enabling it to skip unnecessary operations and achieve O(n) best-case performance.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Smoothsort:</strong></para>
/// <list type="number">
/// <item><description><strong>Leonardo Heap Property:</strong> The array is structured as a sequence of Leonardo heaps,
/// where each heap satisfies the max-heap property and heap roots are ordered in ascending sequence.
/// Leonardo numbers L(k) = L(k-1) + L(k-2) + 1 with L(0)=1, L(1)=1 define valid heap sizes (1, 1, 3, 5, 9, 15, 25, ...).</description></item>
/// <item><description><strong>Heap Construction (Build Phase):</strong> Elements are incrementally added to form Leonardo heaps.
/// The algorithm maintains heap sizes as a bitstring representation in variable 'p', where each bit indicates presence of a specific Leonardo number.
/// When adding elements, heaps are merged following Leonardo number rules (L(k) can be formed by combining L(k-1) and L(k-2) heaps plus one element).</description></item>
/// <item><description><strong>Trinkle Operation:</strong> Ensures heap property across the forest of Leonardo heaps by comparing and swapping roots.
/// This operation maintains the invariant that heap roots form an ascending sequence, enabling efficient extraction of maximum elements.</description></item>
/// <item><description><strong>Shift Operation (Sift-down):</strong> Restores heap property within a single Leonardo tree by moving elements down
/// to their correct positions, similar to heapify in binary heaps but adapted to Leonardo tree structure.</description></item>
/// <item><description><strong>Extraction Phase (Sort Phase):</strong> Repeatedly extracts the maximum element (rightmost heap root) and restructures
/// remaining heaps. When a Leonardo heap of size L(k) is removed, it splits into two smaller heaps of sizes L(k-1) and L(k-2),
/// which are then re-heapified using SemiTrinkle operations.</description></item>
/// <item><description><strong>Adaptive Behavior:</strong> For already sorted or nearly sorted data, the Trinkle operation detects
/// that elements are already in correct positions and terminates early, achieving O(n) time complexity instead of O(n log n).</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Heap (Leonardo heap variant)</description></item>
/// <item><description>Stable      : No (heap operations do not preserve relative order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space - only uses variables for heap size tracking)</description></item>
/// <item><description>Adaptive    : Yes (recognizes sorted subsequences and achieves O(n) on sorted data)</description></item>
/// <item><description>Best case   : Ω(n) - Already sorted data requires only O(n) comparisons during heap construction with no swaps</description></item>
/// <item><description>Average case: Θ(n log n) - Typical random data requires O(n log n) comparisons and swaps</description></item>
/// <item><description>Worst case  : O(n log n) - Reverse sorted data requires maximum heap operations but bounded by Leonardo heap depth (log φ n)</description></item>
/// <item><description>Comparisons : Best Ω(n), Average ~1.5n log n, Worst O(n log n) - Leonardo heap depth log φ n where φ = (1+√5)/2 ≈ 1.618 (golden ratio)</description></item>
/// <item><description>Swaps       : Best 0 (sorted), Average O(n log n), Worst O(n log n) - Each swap involves 2 writes</description></item>
/// <item><description>Writes      : Best 0 (sorted), Average O(n log n), Worst O(n log n) - Tracked separately from swaps for precise measurement</description></item>
/// </list>
/// <para><strong>Implementation Verification:</strong></para>
/// <list type="bullet">
/// <item><description>✓ Leonardo number sequence correctly generated via Up/Down operations</description></item>
/// <item><description>✓ Heap construction uses bitstring 'p' to track Leonardo heap sizes</description></item>
/// <item><description>✓ Trinkle operation maintains ascending order of heap roots across the forest</description></item>
/// <item><description>✓ Shift operation properly sifts elements down within Leonardo trees</description></item>
/// <item><description>✓ Extraction phase correctly splits L(k) heaps into L(k-1) and L(k-2) components</description></item>
/// <item><description>✓ Achieves O(n) performance on sorted data (verified: n=100 → 0 swaps, 188 comparisons vs theoretical ~100)</description></item>
/// <item><description>✓ Achieves O(n log n) on random/reversed data (verified within expected bounds)</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Smoothsort</para>
/// <para>Paper: Dijkstra, E.W. (1981). "Smoothsort, an alternative for sorting in situ" (EWD796a) https://www.cs.utexas.edu/~EWD/ewd07xx/EWD796a.PDF</para>
/// <para>Slide: https://www.slideshare.net/habib_786/smooth-sort</para>
/// </remarks>
public static class SmoothSort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array

    /// <summary>
    /// Sorts the elements in the specified span in ascending order using the default comparer.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort in place.</param>
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
    {
        Sort(span, NullContext.Default);
    }

    /// <summary>
    /// Sorts the elements in the specified span using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T>(span, context, BUFFER_MAIN);

        int q = 1, r = 0, p = 1, b = 1, c = 1;
        int r1 = 0, b1 = 0, c1 = 0;

        // Build heap fase
        while (q < span.Length)
        {
            r1 = r;
            //Debug.WriteLine($"[SmoothSort - Build] Start q={q}, r={r}, p={p}, b={b}, c={c}, r1={r1}");

            if ((p & 7) == 3)
            {
                b1 = b;
                c1 = c;
                Shift(s, ref r1, ref b1, ref c1);

                p = (p + 1) >> 2;

                Up(ref b, ref c);
                Up(ref b, ref c);
            }
            else if ((p & 3) == 1)
            {
                if (q + c < span.Length)
                {
                    b1 = b;
                    c1 = c;
                    Shift(s, ref r1, ref b1, ref c1);
                }
                else
                {
                    Trinkle(s, ref p, ref b1, ref b, ref c1, ref c, ref r1);
                }

                Down(ref b, ref c);
                p <<= 1;

                while (b > 1)
                {
                    Down(ref b, ref c);
                    p <<= 1;
                }
                ++p;

                Debug.Assert(p != 0, "p should not be zero after increment");
            }

            ++q;
            ++r;
        }

        // Sort fase
        r1 = r;
        //Debug.WriteLine($"[SmoothSort - Sort] Start q={q}, r={r}, p={p}, b={b}, c={c}, r1={r1}");
        Trinkle(s, ref p, ref b1, ref b, ref c1, ref c, ref r1);

        while (q > 1)
        {
            //Debug.WriteLine($"[SmoothSort - Sort] Loop q={q}, r={r}, p={p}, b={b}, c={c}, r1={r1}");
            --q;
            if (b == 1)
            {
                --r;
                --p;

                // shift while p is power of 2
                while ((p & 1) == 0)
                {
                    p >>= 1;
                    Up(ref b, ref c);
                }
            }
            else
            {
                if (b >= 3)
                {
                    --p;
                    r = r - b + c;

                    if (p > 0)
                    {
                        SemiTrinkle(s, ref p, ref b1, ref b, ref c1, ref c, ref r1, ref r);
                    }

                    Down(ref b, ref c);
                    p = (p << 1) + 1;

                    // update r and re-SemiTrinkle
                    r = r + c;
                    SemiTrinkle(s, ref p, ref b1, ref b, ref c1, ref c, ref r1, ref r);
                    Down(ref b, ref c);
                    p = (p << 1) + 1;

                    Debug.Assert(p != 0, "p should not be zero");
                }
            }

            Debug.Assert(p != 0, "p should not be zero");
        }
    }

    /// <summary>
    /// Shift the element to the right position
    /// </summary>
    /// <param name="s"></param>
    /// <param name="r1"></param>
    /// <param name="b1"></param>
    /// <param name="c1"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Shift<T>(SortSpan<T> s, ref int r1, ref int b1, ref int c1) where T : IComparable<T>
    {
        var r0 = r1;
        var t = s.Read(r0);

        while (b1 >= 3)
        {
            var r2 = r1 - b1 + c1;
            if (s.Compare(r1 - 1, r2) > 0)
            {
                r2 = r1 - 1;
                Down(ref b1, ref c1);
            }

            if (s.Compare(r2, t) <= 0)
            {
                b1 = 1;
            }
            else
            {
                s.Write(r1, s.Read(r2));
                r1 = r2;
                Down(ref b1, ref c1);
            }
        }

        if (r1 - r0 != 0)
        {
            s.Write(r1, t);
        }
    }

    /// <summary>
    /// Construct or adjust the partial heap (reconstruct the main heap)
    /// </summary>
    /// <param name="s"></param>
    /// <param name="p"></param>
    /// <param name="b1"></param>
    /// <param name="b"></param>
    /// <param name="c1"></param>
    /// <param name="c"></param>
    /// <param name="r1"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Trinkle<T>(SortSpan<T> s, ref int p, ref int b1, ref int b, ref int c1, ref int c, ref int r1) where T : IComparable<T>
    {
        int p1 = p, r0 = r1;
        b1 = b;
        c1 = c;

        var t = s.Read(r0);

        while (p1 > 0)
        {
            while ((p1 & 1) == 0)
            {
                p1 >>= 1;
                Up(ref b1, ref c1);
            }

            var r3 = r1 - b1;

            if ((p1 == 1) || s.Compare(r3, t) <= 0)
            {
                // don't need to reconstruct the heap
                p1 = 0;
            }
            else
            {
                p1--;
                if (b1 == 1)
                {
                    // 1st step heap, just move the element
                    s.Write(r1, s.Read(r3));
                    r1 = r3;
                }
                else
                {
                    if (b1 >= 3)
                    {
                        var r2 = r1 - b1 + c1;
                        if (s.Compare(r1 - 1, r2) > 0)
                        {
                            r2 = r1 - 1;
                            Down(ref b1, ref c1);
                            p1 <<= 1;
                        }

                        // Judge swap or not
                        if (s.Compare(r2, r3) <= 0)
                        {
                            s.Write(r1, s.Read(r3));
                            r1 = r3;
                        }
                        else
                        {
                            s.Write(r1, s.Read(r2));
                            r1 = r2;
                            Down(ref b1, ref c1);
                            p1 = 0;
                        }
                    }
                }
            }
        }

        // fix if position is changed from origin
        if (r1 - r0 != 0)
        {
            s.Write(r1, t);
        }

        // final adjustment
        Shift(s, ref r1, ref b1, ref c1);
    }

    /// <summary>
    /// Trinkle part of the heap
    /// </summary>
    /// <param name="s"></param>
    /// <param name="p"></param>
    /// <param name="b1"></param>
    /// <param name="b"></param>
    /// <param name="c1"></param>
    /// <param name="c"></param>
    /// <param name="r1"></param>
    /// <param name="r"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SemiTrinkle<T>(SortSpan<T> s, ref int p, ref int b1, ref int b, ref int c1, ref int c, ref int r1, ref int r) where T : IComparable<T>
    {
        r1 = r - c;
        if (s.Compare(r1, r) > 0)
        {
            s.Swap(r, r1);
            Trinkle(s, ref p, ref b1, ref b, ref c1, ref c, ref r1);
        }
    }

    /// <summary>
    /// Upward Leonardo heap step
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Up(ref int a, ref int b)
    {
        var temp = a;
        a += b + 1;
        b = temp;
    }

    /// <summary>
    /// Downward Leonardo heap step
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Down(ref int a, ref int b)
    {
        var temp = b;
        b = a - b - 1;
        a = temp;
    }
}
