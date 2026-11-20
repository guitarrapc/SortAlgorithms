using SortAlgorithm.Contexts;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 無作為に取り出した 2^k − 1 個のサンプルを先に整列し、その要素を事前に選ばれたピボットとして
/// 中央値、四分位点、八分位点…の順に分割へ使う QuickSort の一般化です（Frazer &amp; McKellar, 1970）。
/// 1 回の分割ごとにピボットを選び直す代わりに、すべてのピボットを 1 つのサンプルから決めることで分割が釣り合い、
/// 比較回数の期待値は n が大きくなるにつれて情報理論的下限 log₂ n! に近づきます。
/// サンプル自身の整列と、サンプルを使い切った部分配列には同じ手順を再帰的に適用し（後者は CPython の samplesort の方式）、他のソートへは切り替えません。
/// <br/>
/// A generalisation of QuickSort that draws a random sample of 2^k − 1 elements, sorts it first, and then uses its
/// elements as preselected pivots — the median first, then the quartiles, then the octiles, and so on (Frazer &amp; McKellar, 1970).
/// Taking every pivot from one sample instead of choosing a new one for each partition keeps the partitions balanced,
/// so the expected number of comparisons approaches the information-theoretic bound log₂ n! as n grows.
/// The sample itself, and the subarrays left once the sample is exhausted, are sorted by the same procedure (the latter as
/// CPython's samplesort did); the algorithm never switches to another sort.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Sample Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Random Sample:</strong> A range that carries no presorted sample draws 2^k − 1 of its elements uniformly
/// at random and moves them to the front of the range. The sample must be random: the balance argument, and with it the
/// comparison bound, holds only in expectation over the draw.</description></item>
/// <item><description><strong>Sample Sorted First:</strong> The sample is sorted before any of its elements is used as a pivot.
/// It is sorted by this same procedure, as a range of its own.</description></item>
/// <item><description><strong>Pivots in Sample Order:</strong> A range whose first 2t + 1 elements are a sorted sample
/// x₁ ≤ … ≤ x₂ₜ₊₁ is partitioned around the sample median xₜ₊₁. The lower half x₁…xₜ goes on as the sorted sample of the
/// part below the pivot and the upper half xₜ₊₂…x₂ₜ₊₁ as the sorted sample of the part above it, so the quartiles partition the
/// two halves next, and after k levels the 2^k − 1 sample elements have split the range into 2^k subarrays.</description></item>
/// <item><description><strong>Sample Order Preserved:</strong> Partitioning must not disturb the order of the sample halves, otherwise
/// the next level would pick a pivot that is not a quantile of the sample. The elements that are not in the sample may be
/// permuted freely.</description></item>
/// <item><description><strong>Recursion on Exhaustion:</strong> When a range's sample is exhausted (one sample element remains and has
/// been used as its pivot), the two subarrays carry no sample and start again at condition 1. A range of length 0 or 1, or one
/// that consists only of its sorted sample, is sorted.</description></item>
/// </list>
/// <para><strong>Implementation notes:</strong></para>
/// <list type="bullet">
/// <item><description>Sample size: 2^k − 1 where 2^k is the power of two closest to n / ln n, with n the length of the range drawing the
/// sample; the theory leaves the sample size free as long as it grows with n. The rule is evaluated through a table of 25 cutoffs
/// (<c>SampleCutoffs</c>) instead of a logarithm per range, and the table rounds at the arithmetic midpoint between two powers
/// (46, 114, 269, …). CPython's samplesort chose the same power but rounded at the geometric midpoint (43, 106, 250, …) and never drew
/// fewer than 15 pivots, handing smaller ranges to binary insertion sort. This class has no such cutoff: for every n ≥ 2 the closest
/// power is at least 2, so each range draws at least one pivot and the small ranges at the bottom of the recursion are ordinary
/// randomised quicksort partitions.</description></item>
/// <item><description>Recursion on exhausted ranges: a range whose sample is used up draws a new sample, as CPython's samplesort did.
/// Wikipedia describes Frazer and McKellar as sampling once and quicksorting the resulting subarrays; the paper itself was not
/// available to check.</description></item>
/// <item><description>Random source: a xorshift32 stream with a <c>seed</c> parameter, so a given seed always draws the same samples and
/// the operation counts are reproducible. The draw is a partial Fisher–Yates shuffle of the first 2^k − 1 positions.</description></item>
/// <item><description>Sample placement: a range keeps its sorted sample in its first positions. To partition, the elements after the sample
/// are split around the pivot by a Hoare-style bidirectional scan, then the block [pivot, upper half of the sample] is shifted
/// right past the elements that went below the pivot. The shift is one swap per element of the block, the displaced elements land in front
/// of it in some order, and the block keeps its order. Afterwards both subranges again have their sorted sample in their first positions.</description></item>
/// <item><description>Explicit stack: ranges waiting to be sorted are kept on a 64-entry stack-allocated stack instead of the call stack.
/// The current range is always the smaller partition or a sample, and a sample is at most 3/5 of its range (n = 5 draws 3), so the depth
/// is at most log_{5/3}(2³¹) + 1 &lt; 44 regardless of the input.</description></item>
/// <item><description>Buffer: none. The sample lives inside the range it partitions, so every operation is on buffer 0.</description></item>
/// <item><description>Adversarial input: the draws depend only on the seed and the range lengths, never on the values, so McIlroy's
/// killer adversary can be built against a fixed seed. Built against the default seed it drives the comparison count along
/// n log³ n rather than n²; the same input sorted with another seed is no worse than random input
/// (<c>sandbox/DotnetFiles/VerifySampleSort.cs</c>).</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Partitioning (Divide and Conquer)</description></item>
/// <item><description>Stable      : No (partitioning exchanges elements across the range, and the sample draw moves elements to the front)</description></item>
/// <item><description>In-place    : Yes (the sample is kept inside the range being sorted; O(log n) explicit stack)</description></item>
/// <item><description>Best case   : Θ(n log n) - every element is compared with the pivots on its path, Θ(log n) of them even when the sample splits evenly</description></item>
/// <item><description>Average case: Θ(n log n) - expected comparisons n log₂ n (1 + o(1)), approaching log₂ n! as the sample grows with n</description></item>
/// <item><description>Worst case  : O(n log³ n) - every draw takes the extremes of its range, so all non-sample elements land in one subarray; even then each round fixes its whole sample of about n / ln n elements, so W(n) ≤ W(sample) + O(n log n) + W(n − sample). A sample that does not grow with n would leave QuickSort's O(n²)</description></item>
/// <item><description>Comparisons : n log₂ n (1 + o(1)) average, O(n log³ n) worst - each element meets one pivot per sample level, and the pivots come from sample quantiles rather than from single elements</description></item>
/// <item><description>Swaps       : Θ(n log n) average, O(n log³ n) worst - partition exchanges, plus the sample draw and the shift of each pivot block past the elements below it</description></item>
/// <item><description>Index Reads : Θ(n log n) average, O(n log³ n) worst - one per comparison (the pivot is held in a local), two per swap, and one pivot read per partition</description></item>
/// <item><description>Index Writes: Θ(n log n) average, O(n log³ n) worst - every write comes from a swap, so the writes are twice the swap count</description></item>
/// <item><description>Space       : O(log n) - explicit stack only; the sample occupies positions of the range itself</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Paper: W. D. Frazer and A. C. McKellar, "Samplesort: A Sampling Approach to Minimal Storage Tree Sorting", Journal of the ACM 17(3), 496–507 (1970). https://doi.org/10.1145/321592.321600</para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Samplesort</para>
/// </remarks>
public static class SampleSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by SampleSortTests, which derives from SortTestsBase.</remarks>
    public static bool IsStable => false;

    // Buffer identifiers for visualization
    const int BUFFER_MAIN = 0;       // Main input array (the sample lives inside it)

    const uint XORSHIFT_SEED = 0x9E3779B9u; // Golden ratio derived; deterministic seed for reproducible sample draws

    // Ranges waiting to be sorted. The proven bound is below 44 (see the class remarks).
    const int STACK_CAPACITY = 64;

    // SampleCutoffs[i] is the smallest range length whose sample has 2^(i + 2) − 1 elements: the first n ≥ 3 with
    // n / ln n > 1.5 · 2^(i + 1), the midpoint at which 2^(i + 2) becomes the power of two closest to n / ln n.
    // Shorter ranges draw a single pivot (2^1 − 1). The last entry already lies below int.MaxValue, so the largest sample is 2^26 − 1.
    static ReadOnlySpan<int> SampleCutoffs =>
    [
        5, 17, 46, 114, 269, 617, 1390, 3086, 6775, 14744, 31854, 68403, 146133,
        310812, 658527, 1390530, 2927421, 6146515, 12874489, 26908482, 56129782,
        116873901, 242955772, 504289414, 1045264292,
    ];

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
    /// <typeparam name="TComparer">The type of the comparer.</typeparam>
    /// <typeparam name="TContext">The type of the sort context.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="comparer">The comparer to use for element comparisons.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    /// <param name="seed">Seed of the xorshift32 stream that draws the samples. The same seed always draws the same samples; 0 selects the default seed.</param>
    public static void Sort<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context, uint seed = XORSHIFT_SEED)
        where TComparer : IComparer<T>
        where TContext : ISortContext
        => Sort(span, 0, span.Length, comparer, context, seed);

    /// <summary>
    /// Sorts the subrange [first..last) using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <typeparam name="TContext">The type of the sort context.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="first">The inclusive start index of the range to sort.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T, TContext>(Span<T> span, int first, int last, TContext context)
        where T : IComparable<T>
        where TContext : ISortContext
        => Sort(span, first, last, new ComparableComparer<T>(), context);

    /// <summary>
    /// Sorts the subrange [first..last) using the provided comparer and context.
    /// This is the full-control version with explicit TComparer and TContext type parameters.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <typeparam name="TComparer">The type of comparer to use for element comparisons.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="span">The span containing elements to sort.</param>
    /// <param name="first">The inclusive start index of the range to sort.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    /// <param name="comparer">The comparer to use for element comparisons.</param>
    /// <param name="context">The sort context for tracking statistics and observations.</param>
    /// <param name="seed">Seed of the xorshift32 stream that draws the samples. The same seed always draws the same samples; 0 selects the default seed.</param>
    public static void Sort<T, TComparer, TContext>(Span<T> span, int first, int last, TComparer comparer, TContext context, uint seed = XORSHIFT_SEED)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        ArgumentOutOfRangeException.ThrowIfNegative(first);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(last, span.Length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(first, last);

        if (last - first <= 1) return;

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        SortCore(s, first, last, seed == 0 ? XORSHIFT_SEED : seed);
    }

    /// <summary>
    /// Sorts [lo..hi) with the Frazer–McKellar procedure, driven by an explicit stack of ranges.
    /// Each range is (lo, hi, sorted): its first <c>sorted</c> elements are a presorted sample of 2^j − 1 elements,
    /// and 0 means the range has no sample yet.
    /// </summary>
    static void SortCore<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int lo, int hi, uint rng)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        Span<int> stackLo = stackalloc int[STACK_CAPACITY];
        Span<int> stackHi = stackalloc int[STACK_CAPACITY];
        Span<int> stackSorted = stackalloc int[STACK_CAPACITY];
        var top = -1;
        var sorted = 0;

        while (true)
        {
            var length = hi - lo;

            // A range of length 0 or 1, or one that is nothing but its sorted sample, is done.
            if (length <= 1 || sorted >= length)
            {
                if (top < 0) break;
                lo = stackLo[top];
                hi = stackHi[top];
                sorted = stackSorted[top--];
                continue;
            }

            if (sorted == 0)
            {
                var sampleCount = SampleCount(length);
                DrawSample(s, lo, hi, sampleCount, ref rng);

                if (sampleCount > 1)
                {
                    // Sort the sample as a range of its own first, then come back to partition [lo..hi) around it.
                    Debug.Assert(top + 1 < STACK_CAPACITY);
                    stackLo[++top] = lo;
                    stackHi[top] = hi;
                    stackSorted[top] = sampleCount;
                    hi = lo + sampleCount;
                    continue;
                }

                // A one-element sample is already sorted.
                sorted = 1;
            }

            var pivotFinal = PartitionAroundSampleMedian(s, lo, hi, sorted);
            var half = sorted >> 1;

            // [lo..pivotFinal) and [pivotFinal + 1..hi) each start with a sorted sample of `half` elements.
            // Continue with the smaller part and defer the larger one, which bounds the stack depth.
            var leftLength = pivotFinal - lo;
            var rightLength = hi - pivotFinal - 1;
            if (leftLength < rightLength)
            {
                if (rightLength > half)
                {
                    Debug.Assert(top + 1 < STACK_CAPACITY);
                    stackLo[++top] = pivotFinal + 1;
                    stackHi[top] = hi;
                    stackSorted[top] = half;
                }
                hi = pivotFinal;
            }
            else
            {
                if (leftLength > half)
                {
                    Debug.Assert(top + 1 < STACK_CAPACITY);
                    stackLo[++top] = lo;
                    stackHi[top] = pivotFinal;
                    stackSorted[top] = half;
                }
                lo = pivotFinal + 1;
            }
            sorted = half;
        }
    }

    /// <summary>
    /// Returns 2^k − 1 where 2^k is the power of two closest to length / ln(length), looked up in <see cref="SampleCutoffs"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int SampleCount(int length)
    {
        var cutoffs = SampleCutoffs;
        var k = 1;
        while (k <= cutoffs.Length && length >= cutoffs[k - 1]) k++;
        return (1 << k) - 1;
    }

    /// <summary>
    /// Moves a uniformly random sample of <paramref name="sampleCount"/> elements of [lo..hi) to [lo..lo + sampleCount)
    /// with a partial Fisher–Yates shuffle.
    /// </summary>
    static void DrawSample<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int lo, int hi, int sampleCount, ref uint rng)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var length = hi - lo;
        var announced = false;
        for (var i = 0; i < sampleCount; i++)
        {
            // Multiply-shift maps the 32-bit draw onto [0, length - i).
            var j = i + (int)(((ulong)NextRandom(ref rng) * (uint)(length - i)) >> 32);
            if (j == i) continue;

            // Announced at the first moving swap: a draw that happens to pick [lo..lo + sampleCount) moves nothing,
            // and a phase with no operation under it would never be shown.
            if (!announced)
            {
                s.Context.OnPhase(SortPhase.SampleSortSample, lo, hi - 1, sampleCount);
                announced = true;
            }
            s.Swap(lo + i, lo + j);
        }
    }

    /// <summary>
    /// Partitions [lo..hi), whose first <paramref name="sorted"/> elements are a sorted sample, around the sample median.
    /// Returns the final index of the pivot. On return [lo..pivot) starts with the lower half of the sample and holds
    /// no element greater than the pivot; (pivot..hi) starts with the upper half and holds no element less than the pivot.
    /// </summary>
    static int PartitionAroundSampleMedian<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int lo, int hi, int sorted)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var pivotIndex = lo + (sorted >> 1);
        var restStart = lo + sorted;

        s.Context.OnPhase(SortPhase.QuickSortPartition, lo, hi - 1, pivotIndex);
        s.Context.OnRole(pivotIndex, BUFFER_MAIN, RoleType.Pivot);
        var pivot = s.Read(pivotIndex);

        // Split the elements after the sample: [restStart..lessEnd) ≤ pivot, [lessEnd..hi) ≥ pivot.
        var partition = new PartitionLoop<T, TComparer, TContext> { I = restStart, J = hi - 1 };
        s.WithPreparedKey(pivot, ref partition);
        var lessCount = partition.I - restStart;

        s.Context.OnRole(pivotIndex, BUFFER_MAIN, RoleType.None);

        // Shift the block [pivot, upper half of the sample] = [pivotIndex..restStart) right by lessCount.
        // Going from the block's last element down, each swap puts one block element at its final position and moves
        // an element that went below the pivot into the vacated slot, so the block keeps its order.
        if (lessCount > 0)
        {
            for (var i = restStart - 1; i >= pivotIndex; i--)
            {
                s.Swap(i, i + lessCount);
            }
        }

        return pivotIndex + lessCount;
    }

    /// <summary>
    /// Generates the next value of a xorshift32 stream.
    /// Deterministic: the same seed always produces the same sequence, so the samples, and with them the
    /// operation counts, are reproducible.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static uint NextRandom(ref uint state)
    {
        state ^= state << 13;
        state ^= state >> 17;
        state ^= state << 5;
        return state;
    }

    /// <summary>
    /// Hoare-style bidirectional partition of [I..J] around a pivot that is not inside the range,
    /// so both scans carry their own bound instead of relying on the pivot as a sentinel.
    /// On return, I is the first index of the part that holds no element less than the pivot.
    /// </summary>
    struct PartitionLoop<T, TComparer, TContext> : IPreparedKeyLoop<T, TComparer, TContext>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        public static PreparedKeyKind Kind => PreparedKeyKind.Partition;
        public int I;
        public int J;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Run<TOrder>(SortSpan<T, TComparer, TContext> s, T pivot)
            where TOrder : struct, IKeyOrder
        {
            var i = I;
            var j = J;
            while (true)
            {
                while (i <= j && s.IsElementLessThan<TOrder>(i, pivot)) i++;

                // When the scans meet, s[i] is already known not to be less than the pivot, so it can stay on the right
                // without comparing it again.
                while (i < j && s.IsValueLessThan<TOrder>(pivot, j)) j--;

                if (i >= j) break;

                s.Swap(i, j);
                i++;
                j--;
            }
            I = i;
        }
    }
}
