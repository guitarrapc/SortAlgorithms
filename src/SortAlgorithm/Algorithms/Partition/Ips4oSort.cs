using SortAlgorithm.Contexts;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// In-place Super Scalar Samplesort（IS⁴o。論文は逐次版を I1S⁴o と呼びます）。無作為標本から最大 255 個の分割点を選び、分岐のない決定木で
/// 各要素のバケットを求めて k 個のバケットへ分けることを、バケットが小さくなるまで繰り返すサンプルソートです（Axtmann, Witt, Ferizovic, Sanders, 2017）。
/// 要素はバケットごとのバッファブロックに集めてから入力の先頭へ書き戻し、ブロック単位の置換で各バケットの領域へ運ぶので、入力と同じ大きさの補助配列を使いません。
/// 標本に同じ値の分割点が重なるときは等値バケットを設け、分割点と等しい要素はそこで確定させます。
/// <br/>
/// In-place Super Scalar Samplesort (IS⁴o; the paper calls the sequential algorithm I1S⁴o). A samplesort that picks up to 255 splitters
/// from a random sample, finds each element's bucket with a branchless decision tree, and splits the range into k buckets, repeating on
/// every bucket until the buckets are small (Axtmann, Witt, Ferizovic and Sanders, 2017). Elements are collected in one buffer block per
/// bucket and written back to the front of the range, then a block permutation moves whole blocks to their buckets, so no array the size
/// of the input is needed. When the sample repeats a splitter, equality buckets take the elements equal to it, which are then final.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct IS⁴o:</strong></para>
/// <list type="number">
/// <item><description><strong>Sample and Splitters:</strong> A partitioning step on n elements with k = 2^ℓ buckets moves a random sample of
/// αk − 1 elements to the front of the range and sorts it by this same procedure. Every α-th sample element becomes a splitter
/// s₁ ≤ … ≤ s_{k−1}. Repeated splitters are dropped, and the remaining ones are padded with copies of the last to the next power of two.</description></item>
/// <item><description><strong>Branchless Decision Tree:</strong> The splitters form an implicit complete binary search tree t with root
/// t₁ and children t₂ⱼ and t₂ⱼ₊₁. An element e descends ℓ levels with j := 2j + [tⱼ &lt; e] and lands in bucket j − k, so bucket i holds
/// the elements with s_i &lt; e ≤ s_{i+1}. The comparison only feeds an addition, so no branch depends on it.</description></item>
/// <item><description><strong>Equality Buckets:</strong> When several splitters repeat, one more step j := 2j + [e ≥ s_j] splits every
/// bucket into the elements below its upper splitter and those equal to it. The equal ones are in their final place and are not
/// partitioned again.</description></item>
/// <item><description><strong>Local Classification:</strong> Each bucket has a buffer block of b elements. A classified element joins its
/// bucket's buffer, and a full buffer is written back over elements that have already been classified, starting at the front of the
/// range. Counting the elements gives each bucket's final range.</description></item>
/// <item><description><strong>Block Permutation:</strong> Bucket ranges are rounded to block boundaries, and each bucket keeps a write
/// pointer and a read pointer. A block read from a bucket's unprocessed blocks is classified by its first element and swapped into the
/// next write position of its bucket, through two swap blocks, until a block lands in a position that has already been read. A block
/// that would end past the range goes to an overflow block.</description></item>
/// <item><description><strong>Cleanup:</strong> The partial blocks at both ends of each bucket are filled from the part of a block that
/// crossed into the next bucket, the overflow block and the bucket's buffer block. Afterwards every element is inside its bucket.</description></item>
/// <item><description><strong>Recursion and Base Case:</strong> Buckets of more than 2n₀ elements are partitioned again. Buckets of at
/// most 2n₀ elements, and every bucket of a step on at most k·n₀ elements, are sorted by insertion sort during the cleanup. An input of at
/// most 16·n₀ elements is sorted by insertion sort at once, and an input that is sorted, or in non-increasing order, is detected and
/// returned or reversed before anything else.</description></item>
/// </list>
/// <para><strong>Implementation notes:</strong></para>
/// <list type="bullet">
/// <item><description>Reference: the sequential path of ips4o at 08a5b92 (BSD-2-Clause), with its default parameters: n₀ = 16, at most
/// k = 256 buckets (fewer for steps on at most 1,048,576 elements, so the base cases stay near n₀), α = max(1, ⌊0.2 ⌊log₂ n⌋⌋), equality
/// buckets when at least 5 splitter candidates repeat, b = 2 KiB divided by the element size and rounded down to a power of two, and 7
/// elements classified together, level by level.</description></item>
/// <item><description>Random source: the reference's 64-bit linear congruential generator (Knuth's MMIX constants), but started from a
/// <c>seed</c> parameter instead of <c>std::random_device</c>, and mapped to a range by the high 64 bits of its product with the range
/// instead of <c>std::uniform_int_distribution</c>, whose mapping differs between standard libraries. A given seed always draws the same
/// samples.</description></item>
/// <item><description>Explicit stack: ranges waiting to be partitioned are kept on a pooled stack that grows on demand, in the order the
/// reference recurses. The reference recurses on the call stack, which a .NET process cannot recover from overflowing. Sorting a sample is
/// still a nested call; samples have at most 1,535 elements, so the nesting is shallow.</description></item>
/// <item><description>Departure: an input of fewer than two elements returns at once. The reference's sorted check compares a single
/// element with itself.</description></item>
/// <item><description>Inconsistent comparer: the block permutation trusts that a block's first element classifies as it did before. A
/// comparer that answers differently can move a block to another bucket and overwrite one that has not been read. Before the cleanup, the
/// bucket pointers are checked against the counts of the classification, and a mismatch throws <see cref="InvalidOperationException"/>;
/// the range is then no longer a permutation of its input. A step above k·n₀ elements that leaves one bucket as large as its range also
/// throws, because a consistent comparer never does and the step would repeat forever. The natural order of a primitive type is
/// consistent, so the checks fold away for it.</description></item>
/// <item><description>Adversarial input: the draws depend only on the seed and the range lengths, never on the values, so McIlroy's killer
/// adversary can be built against a fixed seed once the initial check is made to fail. Built against the default seed, it makes every sample
/// hold the smallest elements of its range, and the comparison count grows like n^1.5 up to 2^20 elements, where k still grows with n. The
/// same input sorted with another seed is no worse than random input (<c>sandbox/DotnetFiles/VerifyIps4oSort.cs</c>).</description></item>
/// <item><description>Buffers: 1 holds the decision tree, 2 the sorted splitters, 3 one buffer block per bucket (up to 2k blocks with
/// equality buckets), 4 the two swap blocks and 5 the overflow block. All are rented once per call and reused by every partitioning step.
/// Bucket boundaries, pointers and buffer fill levels are metadata and are not reported.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Partitioning (Samplesort)</description></item>
/// <item><description>Stable      : No (the sample is drawn by swaps, and the buffer blocks and the block permutation reorder elements of a bucket)</description></item>
/// <item><description>In-place    : Yes (the buffer blocks, swap blocks and overflow block take O(k·b) elements regardless of n; the stack of waiting ranges grows with the recursion)</description></item>
/// <item><description>Best case   : Θ(n) - sorted input, and input in non-increasing order, end after the initial check of n comparisons</description></item>
/// <item><description>Average case: Θ(n log n) - each partitioning step costs log₂ k comparisons per element and divides the range by about k, and the base cases cost O(n₀) per element</description></item>
/// <item><description>Worst case  : O(n² / log n) - a sample can hold the smallest elements of its range and leave all others in the last bucket, so a step removes only its Θ(k log n) sample elements; with k at most 256 that makes Θ(n / log n) steps of Θ(n) work each</description></item>
/// <item><description>Comparisons : n log₂(n / n₀) + O(n₀ n) average, O(n² / log n) worst - log₂ k down the tree per element and step (one more with equality buckets), plus the sample, the first element of each block in the permutation and the base cases; n for sorted or non-increasing input</description></item>
/// <item><description>Swaps       : O(n) - only the sample draws, at most half of each partitioned range, and the ⌊n/2⌋ exchanges that reverse non-increasing input; blocks move by copies, not swaps</description></item>
/// <item><description>Index Reads : Θ(n log n) average, O(n² / log n) worst - each step reads every element once to classify it, log₂ k splitters per element, and the blocks copied by the permutation and the cleanup, plus the base cases; 2n for sorted input</description></item>
/// <item><description>Index Writes: Θ(n log n) average, O(n² / log n) worst - each step writes every element to a buffer block and back, copies misplaced blocks through the swap blocks, and fills the bucket margins, plus the base-case shifts; 0 for sorted input</description></item>
/// <item><description>Space       : O(k·b) - buffer blocks, swap blocks, overflow block and the splitters, plus the stack of waiting ranges</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Paper: M. Axtmann, S. Witt, D. Ferizovic and P. Sanders, "In-Place Parallel Super Scalar Samplesort (IPS⁴o)", ESA 2017, LIPIcs 87, 9:1–9:14. https://arxiv.org/abs/1705.02257</para>
/// <para>Paper: M. Axtmann, S. Witt, D. Ferizovic and P. Sanders, "Engineering In-place (Shared-memory) Sorting Algorithms", ACM Transactions on Parallel Computing 9(1), 2022. https://arxiv.org/abs/2009.13569</para>
/// <para>Paper: P. Sanders and S. Winkel, "Super Scalar Sample Sort", ESA 2004, LNCS 3221, 784–796.</para>
/// <para>Code: https://github.com/ips4o/ips4o</para>
/// </remarks>
public static class Ips4oSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>Verified by Ips4oSortTests, which derives from SortTestsBase.</remarks>
    public static bool IsStable => false;

    // Buffer identifiers for visualization
    const int BUFFER_MAIN = 0;       // Main input array
    const int BUFFER_TREE = 1;       // Splitters in decision-tree order, positions 1 .. leaves - 1
    const int BUFFER_SPLITTERS = 2;  // Sorted splitters, the last one repeated, for the equality step
    const int BUFFER_BLOCKS = 3;     // One buffer block per bucket during classification
    const int BUFFER_SWAP = 4;       // The two swap blocks of the block permutation
    const int BUFFER_OVERFLOW = 5;   // The block that would end past the range

    // The reference's default configuration (ips4o/config.hpp).
    const int BASE_CASE_SIZE = 16;                                         // n₀
    const int BASE_CASE_MULTIPLIER = 16;                                   // whole inputs up to 16·n₀ go to insertion sort
    const int BLOCK_SIZE_IN_BYTES = 2 << 10;
    const int EQUAL_BUCKETS_THRESHOLD = 5;                                 // repeated splitter candidates that enable equality buckets
    const int LOG_BUCKETS = 8;                                             // at most 2^8 buckets, equality buckets excluded
    const int OVERSAMPLING_FACTOR_PERCENT = 20;                            // α = 0.2 log₂ n
    const int SINGLE_LEVEL_THRESHOLD = BASE_CASE_SIZE << LOG_BUCKETS;      // 4,096: one more level reaches the base case
    const int TWO_LEVEL_THRESHOLD = SINGLE_LEVEL_THRESHOLD << LOG_BUCKETS; // 1,048,576: two more levels reach the base case
    const int MAX_BUCKETS = 2 << LOG_BUCKETS;                              // equality buckets included

    const ulong DEFAULT_SEED = 0x9E3779B97F4A7C15UL;    // Golden ratio derived; deterministic seed for reproducible samples
    const ulong LCG_MULTIPLIER = 6364136223846793005UL; // Knuth's MMIX constants, as in the reference
    const ulong LCG_INCREMENT = 1442695040888963407UL;

    const int INITIAL_STACK = 4 * MAX_BUCKETS;          // two ints per waiting range

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
    /// <param name="seed">Start of the generator that draws the samples. The same seed always draws the same samples.</param>
    public static void Sort<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context, ulong seed = DEFAULT_SEED)
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
    /// Sorts the subrange [first..last) using the provided comparer and sort context.
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
    /// <param name="seed">Start of the generator that draws the samples. The same seed always draws the same samples.</param>
    public static void Sort<T, TComparer, TContext>(Span<T> span, int first, int last, TComparer comparer, TContext context, ulong seed = DEFAULT_SEED)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        ArgumentOutOfRangeException.ThrowIfNegative(first);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(last, span.Length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(first, last);

        var n = last - first;
        if (n < 2) return;

        var a = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
        if (SortSimpleCases(a, first, last)) return;
        if (n <= BASE_CASE_MULTIPLIER * BASE_CASE_SIZE)
        {
            BaseCase(a, first, last);
            return;
        }

        var blockSize = BlockSize<T>();
        // Steps on more than 4,096 elements may use all 256 leaves; smaller steps never use more leaves than the first one.
        var leaves = 1 << (n > SINGLE_LEVEL_THRESHOLD ? LOG_BUCKETS : LogBuckets(n));
        var buckets = 2 * leaves;

        var tree = ArrayPool<T>.Shared.Rent(leaves);
        var splitters = ArrayPool<T>.Shared.Rent(leaves);
        var blocks = ArrayPool<T>.Shared.Rent(buckets * blockSize);
        var swap = ArrayPool<T>.Shared.Rent(2 * blockSize);
        var overflow = ArrayPool<T>.Shared.Rent(blockSize);
        var meta = ArrayPool<long>.Shared.Rent(5 * buckets + 1);
        var st = new Sorter<T, TComparer, TContext>
        {
            A = a,
            Tree = new(tree.AsSpan(0, leaves), context, comparer, BUFFER_TREE),
            Splitters = new(splitters.AsSpan(0, leaves), context, comparer, BUFFER_SPLITTERS),
            Blocks = new(blocks.AsSpan(0, buckets * blockSize), context, comparer, BUFFER_BLOCKS),
            Swap = new(swap.AsSpan(0, 2 * blockSize), context, comparer, BUFFER_SWAP),
            Overflow = new(overflow.AsSpan(0, blockSize), context, comparer, BUFFER_OVERFLOW),
            BucketStart = meta.AsSpan(0, buckets + 1),
            BucketSize = meta.AsSpan(buckets + 1, buckets),
            WritePointer = meta.AsSpan(2 * buckets + 1, buckets),
            ReadPointer = meta.AsSpan(3 * buckets + 1, buckets),
            Fill = meta.AsSpan(4 * buckets + 1, buckets),
            Stack = ArrayPool<int>.Shared.Rent(INITIAL_STACK),
            Random = seed,
            BlockSize = blockSize,
            CheckComparer = !a.UsesNaturalPrimitiveOrder,
        };
        st.Fill.Clear();
        try
        {
            Sequential(ref st, first, last);
        }
        finally
        {
            var clear = RuntimeHelpers.IsReferenceOrContainsReferences<T>();
            ArrayPool<T>.Shared.Return(tree, clear);
            ArrayPool<T>.Shared.Return(splitters, clear);
            ArrayPool<T>.Shared.Return(blocks, clear);
            ArrayPool<T>.Shared.Return(swap, clear);
            ArrayPool<T>.Shared.Return(overflow, clear);
            ArrayPool<long>.Shared.Return(meta);
            ArrayPool<int>.Shared.Return(st.Stack);
        }
    }

    /// <summary>
    /// The state of one call: the buffers, the classifier of the current partitioning step, the bucket metadata and the generator.
    /// </summary>
    ref struct Sorter<T, TComparer, TContext>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        public SortSpan<T, TComparer, TContext> A;
        public SortSpan<T, TComparer, TContext> Tree;
        public SortSpan<T, TComparer, TContext> Splitters;
        public SortSpan<T, TComparer, TContext> Blocks;
        public SortSpan<T, TComparer, TContext> Swap;
        public SortSpan<T, TComparer, TContext> Overflow;
        // Offsets from Begin. Rounding a bucket end up to a block boundary can pass int.MaxValue, so they are long.
        public Span<long> BucketStart;
        public Span<long> BucketSize;
        public Span<long> WritePointer;
        public Span<long> ReadPointer;
        public Span<long> Fill;
        public int[] Stack;
        public int StackTop;
        public ulong Random;
        public int BlockSize;
        public bool CheckComparer;
        // The current partitioning step.
        public int Begin;
        public int End;
        public int LogLeaves;
        public int Leaves;
        public int NumBuckets;
        public bool EqualBuckets;
        public bool HasOverflow;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int Bit(bool value) => Unsafe.As<bool, byte>(ref value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static long AlignToNextBlock(long p, int blockSize) => (p + blockSize - 1) & ~(long)(blockSize - 1);

    /// <summary>
    /// b: 2 KiB of elements, rounded down to a power of two, and at least one.
    /// </summary>
    static int BlockSize<T>()
    {
        var size = Unsafe.SizeOf<T>();
        return 1 << BitOperations.Log2((uint)(BLOCK_SIZE_IN_BYTES < size ? 1 : BLOCK_SIZE_IN_BYTES / size));
    }

    /// <summary>
    /// ℓ for a step on n elements: fewer buckets when one or two more levels reach the base case, so the base cases stay near n₀.
    /// </summary>
    static int LogBuckets(int n)
    {
        if (n <= SINGLE_LEVEL_THRESHOLD) return Math.Max(1, BitOperations.Log2((uint)(n / BASE_CASE_SIZE)));
        if (n <= TWO_LEVEL_THRESHOLD) return Math.Max(1, (BitOperations.Log2((uint)(n / BASE_CASE_SIZE)) + 1) / 2);
        return LOG_BUCKETS;
    }

    /// <summary>
    /// α for a step on n elements: 0.2 ⌊log₂ n⌋, at least 1.
    /// </summary>
    static double OversamplingFactor(int n)
    {
        var f = OVERSAMPLING_FACTOR_PERCENT / 100.0 * BitOperations.Log2((uint)n);
        return f < 1.0 ? 1.0 : f;
    }

    /// <summary>
    /// A draw from [0, range): the generator's next state, scaled by the high 64 bits of its product with the range.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static long NextBelow(ref ulong state, ulong range)
    {
        state = unchecked(state * LCG_MULTIPLIER + LCG_INCREMENT);
        return (long)Math.BigMul(state, range, out _);
    }

    /// <summary>
    /// <c>sortSimpleCases</c>: true when a[begin..end) is sorted, or was in non-increasing order and has been reversed.
    /// </summary>
    static bool SortSimpleCases<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int begin, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        a.Context.OnPhase(SortPhase.Ips4oSimpleCases, a.Offset + begin, a.Offset + end - 1);

        if (a.IsGreaterOrEqualAt(end - 1, begin))
        {
            for (var i = begin + 1; i < end; i++)
            {
                if (a.IsLessAt(i, i - 1)) return false;
            }
            return true;
        }

        for (var i = begin; i + 1 < end; i++)
        {
            if (a.IsLessAt(i, i + 1)) return false;
        }
        for (int i = begin, j = end - 1; i < j; i++, j--)
        {
            a.Swap(i, j);
        }
        return true;
    }

    /// <summary>
    /// Announces and runs the base case on a[begin..end).
    /// </summary>
    static void BaseCase<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int begin, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (end - begin < 2) return;
        a.Context.OnPhase(SortPhase.Ips4oBaseCase, a.Offset + begin, a.Offset + end - 1);
        InsertionSort(a, begin, end);
    }

    /// <summary>
    /// <c>insertionSort</c>: an element smaller than the first moves the whole sorted prefix; any other stops at the first element
    /// that is not greater, which the first comparison guarantees.
    /// </summary>
    static void InsertionSort<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> a, int begin, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Only a consistent comparer stops the inner loop at begin, and element access is unchecked. The natural order of a primitive
        // type is consistent, so the check folds away for it.
        var checkCursor = !a.UsesNaturalPrimitiveOrder;
        for (var it = begin + 1; it < end; it++)
        {
            var value = a.Read(it);
            if (a.IsValueLessThan(value, begin))
            {
                for (var k = it; k > begin; k--)
                {
                    a.Write(k, a.Read(k - 1));
                }
                a.Write(begin, value);
            }
            else
            {
                var cur = it;
                var next = it - 1;
                while (true)
                {
                    if (checkCursor && next < begin) ThrowHelper.RestoreAndThrowInconsistentComparer(a, cur, value);
                    if (!a.IsValueLessThan(value, next, out var moved)) break;
                    a.Write(cur, moved);
                    cur = next;
                    next--;
                }
                a.Write(cur, value);
            }
        }
    }

    /// <summary>
    /// <c>sequential</c> and <c>sequential_rec</c>: sorts a[begin..end) by partitioning steps until every bucket has been sorted by
    /// the base case. Ranges wait on the explicit stack above its current top and are taken in the order the reference recurses.
    /// </summary>
    static void Sequential<T, TComparer, TContext>(ref Sorter<T, TComparer, TContext> st, int begin, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (end - begin <= 2 * BASE_CASE_SIZE)
        {
            BaseCase(st.A, begin, end);
            return;
        }

        var bottom = st.StackTop;
        Push(ref st, begin, end);
        while (st.StackTop > bottom)
        {
            st.StackTop -= 2;
            var b = st.Stack[st.StackTop];
            var e = st.Stack[st.StackTop + 1];

            Partition(ref st, b, e);

            // A step on at most k·n₀ elements sorted its buckets during the cleanup.
            if (e - b <= SINGLE_LEVEL_THRESHOLD) continue;

            // The reference recurses on the ordinary buckets from the first, then on the last bucket when equality buckets are in use
            // (it holds the elements greater than every splitter). Pushing in reverse takes them in that order.
            var numBuckets = st.NumBuckets;
            var stride = st.EqualBuckets ? 2 : 1;
            if (st.EqualBuckets) PushBucket(ref st, b, e, numBuckets - 1);
            for (var i = numBuckets - stride; i >= 0; i -= stride)
            {
                PushBucket(ref st, b, e, i);
            }
        }
    }

    static void PushBucket<T, TComparer, TContext>(ref Sorter<T, TComparer, TContext> st, int begin, int end, int bucket)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var start = st.BucketStart[bucket];
        var stop = st.BucketStart[bucket + 1];
        if (stop - start <= 2 * BASE_CASE_SIZE) return;
        // A step above k·n₀ elements has at least two distinct splitters, or equality buckets, so a consistent comparer leaves every
        // bucket smaller than its range. One that does not would partition the same range again forever.
        if (st.CheckComparer && stop - start == end - begin) ThrowHelper.ThrowInconsistentComparer();
        Push(ref st, begin + (int)start, begin + (int)stop);
    }

    static void Push<T, TComparer, TContext>(ref Sorter<T, TComparer, TContext> st, int begin, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (st.StackTop + 2 > st.Stack.Length)
        {
            var larger = ArrayPool<int>.Shared.Rent(st.Stack.Length * 2);
            Array.Copy(st.Stack, larger, st.StackTop);
            ArrayPool<int>.Shared.Return(st.Stack);
            st.Stack = larger;
        }
        st.Stack[st.StackTop] = begin;
        st.Stack[st.StackTop + 1] = end;
        st.StackTop += 2;
    }

    /// <summary>
    /// <c>partition</c>: one partitioning step on a[begin..end). Afterwards BucketStart holds the bucket boundaries as offsets from begin,
    /// NumBuckets their count and EqualBuckets whether every second bucket is an equality bucket.
    /// </summary>
    static void Partition<T, TComparer, TContext>(ref Sorter<T, TComparer, TContext> st, int begin, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Sorting the sample runs partitioning steps of its own, so the step's range is set after the classifier has been built.
        BuildClassifier(ref st, begin, end);
        st.Begin = begin;
        st.End = end;
        st.HasOverflow = false;

        var firstEmptyBlock = Classify(ref st);
        var overflowBucket = ComputeOverflowBucket(ref st);
        PermuteBlocks(ref st, firstEmptyBlock);
        if (st.CheckComparer) CheckBucketPointers(ref st);
        Cleanup(ref st, overflowBucket);
    }

    /// <summary>
    /// <c>buildClassifier</c>: draws and sorts the sample, picks the splitters and builds the decision tree.
    /// </summary>
    static void BuildClassifier<T, TComparer, TContext>(ref Sorter<T, TComparer, TContext> st, int begin, int end)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var a = st.A;
        var n = end - begin;
        var logBuckets = LogBuckets(n);
        var numBuckets = 1 << logBuckets;
        var step = Math.Max(1, (int)OversamplingFactor(n));
        var numSamples = Math.Min(step * numBuckets - 1, n / 2);

        a.Context.OnPhase(SortPhase.Ips4oSampling, a.Offset + begin, a.Offset + end - 1, numSamples);
        SelectSample(ref st, begin, end, numSamples);
        Sequential(ref st, begin, begin + numSamples);

        // Every step-th sample element is a splitter candidate; equal neighbours in the sorted sample are dropped.
        var splitters = st.Splitters;
        var splitter = begin + step - 1;
        var value = a.Read(splitter);
        splitters.Write(0, value);
        var last = 0;
        for (var i = 2; i < numBuckets; i++)
        {
            splitter += step;
            value = a.Read(splitter);
            if (splitters.IsElementLessThan(last, value)) splitters.Write(++last, value);
        }

        var distinct = last + 1;
        var equalBuckets = numBuckets - 1 - distinct >= EQUAL_BUCKETS_THRESHOLD;

        // Pad to the next power of two with the last candidate, then repeat the last splitter once more for the equality step of the
        // last leaf.
        logBuckets = BitOperations.Log2((uint)distinct) + 1;
        numBuckets = 1 << logBuckets;
        for (var i = distinct + 1; i < numBuckets; i++)
        {
            splitters.Write(++last, value);
        }
        splitters.Write(numBuckets - 1, splitters.Read(numBuckets - 2));
        BuildTree(st.Tree, splitters, 0, numBuckets - 1, 1, numBuckets);

        st.LogLeaves = logBuckets;
        st.Leaves = numBuckets;
        st.EqualBuckets = equalBuckets;
        st.NumBuckets = equalBuckets ? 2 * numBuckets : numBuckets;
    }

    /// <summary>
    /// <c>selectSample</c>: a partial Fisher–Yates shuffle that swaps numSamples random elements to the front of a[begin..end).
    /// </summary>
    static void SelectSample<T, TComparer, TContext>(ref Sorter<T, TComparer, TContext> st, int begin, int end, int numSamples)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var a = st.A;
        var remaining = (ulong)(end - begin);
        for (var p = begin; numSamples > 0; numSamples--, p++)
        {
            var i = (int)NextBelow(ref st.Random, remaining--);
            a.Swap(p, p + i);
        }
    }

    /// <summary>
    /// <c>Classifier::build</c>: places sorted[left..right) as an implicit binary search tree, the median at position pos.
    /// </summary>
    static void BuildTree<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> tree, SortSpan<T, TComparer, TContext> sorted, int left, int right, int pos, int leaves)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var mid = left + (right - left) / 2;
        tree.Write(pos, sorted.Read(mid));
        if (2 * pos < leaves)
        {
            BuildTree(tree, sorted, left, mid, 2 * pos, leaves);
            BuildTree(tree, sorted, mid, right, 2 * pos + 1, leaves);
        }
    }

    /// <summary>
    /// <c>Classifier::classify</c> for one element: ℓ steps down the tree, then the equality step when equality buckets are in use.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int ClassifyOne<T, TComparer, TContext>(ref Sorter<T, TComparer, TContext> st, T value)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var tree = st.Tree;
        var b = 1;
        for (var l = st.LogLeaves; l > 0; l--)
        {
            b = 2 * b + Bit(tree.IsElementLessThan(b, value));
        }
        if (st.EqualBuckets) return 2 * b + Bit(st.Splitters.IsValueGreaterOrEqual(value, b - st.Leaves)) - 2 * st.Leaves;
        return b - st.Leaves;
    }

    /// <summary>
    /// <c>sequentialClassification</c>: classifies the range into the buffer blocks, then sets the bucket boundaries and, for each
    /// bucket, the write pointer at its first block boundary and the read pointer at its last full block written back. Returns the
    /// offset of the first position no full block was written back to.
    /// </summary>
    static long Classify<T, TComparer, TContext>(ref Sorter<T, TComparer, TContext> st)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var a = st.A;
        a.Context.OnPhase(SortPhase.Ips4oClassification, a.Offset + st.Begin, a.Offset + st.End - 1, st.NumBuckets);

        var numBuckets = st.NumBuckets;
        var blockSize = st.BlockSize;
        st.BucketSize[..numBuckets].Clear();
        var firstEmptyBlock = ClassifyLocally(ref st);

        long sum = 0;
        st.BucketStart[0] = 0;
        for (var i = 0; i < numBuckets; i++)
        {
            sum += st.BucketSize[i];
            st.BucketStart[i + 1] = sum;
        }

        for (var bucket = 0; bucket < numBuckets; bucket++)
        {
            var start = AlignToNextBlock(st.BucketStart[bucket], blockSize);
            var stop = AlignToNextBlock(st.BucketStart[bucket + 1], blockSize);
            st.WritePointer[bucket] = start;
            st.ReadPointer[bucket] = (start >= firstEmptyBlock ? start : (stop <= firstEmptyBlock ? stop : firstEmptyBlock)) - blockSize;
        }
        return firstEmptyBlock;
    }

    /// <summary>
    /// <c>classifyLocally</c>: classifies a[Begin..End), seven elements at a time level by level, and returns the offset of the first
    /// position no full buffer block was written back to.
    /// </summary>
    static long ClassifyLocally<T, TComparer, TContext>(ref Sorter<T, TComparer, TContext> st)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var a = st.A;
        var tree = st.Tree;
        var splitters = st.Splitters;
        var logLeaves = st.LogLeaves;
        var leaves = st.Leaves;
        var equalBuckets = st.EqualBuckets;
        var firstBucket = equalBuckets ? 2 * leaves : leaves;
        long write = 0;

        var i = st.Begin;
        var end = st.End;
        for (; i <= end - 7; i += 7)
        {
            var v0 = a.Read(i);
            var v1 = a.Read(i + 1);
            var v2 = a.Read(i + 2);
            var v3 = a.Read(i + 3);
            var v4 = a.Read(i + 4);
            var v5 = a.Read(i + 5);
            var v6 = a.Read(i + 6);
            int b0 = 1, b1 = 1, b2 = 1, b3 = 1, b4 = 1, b5 = 1, b6 = 1;
            for (var l = logLeaves; l > 0; l--)
            {
                b0 = 2 * b0 + Bit(tree.IsElementLessThan(b0, v0));
                b1 = 2 * b1 + Bit(tree.IsElementLessThan(b1, v1));
                b2 = 2 * b2 + Bit(tree.IsElementLessThan(b2, v2));
                b3 = 2 * b3 + Bit(tree.IsElementLessThan(b3, v3));
                b4 = 2 * b4 + Bit(tree.IsElementLessThan(b4, v4));
                b5 = 2 * b5 + Bit(tree.IsElementLessThan(b5, v5));
                b6 = 2 * b6 + Bit(tree.IsElementLessThan(b6, v6));
            }
            if (equalBuckets)
            {
                b0 = 2 * b0 + Bit(splitters.IsValueGreaterOrEqual(v0, b0 - leaves));
                b1 = 2 * b1 + Bit(splitters.IsValueGreaterOrEqual(v1, b1 - leaves));
                b2 = 2 * b2 + Bit(splitters.IsValueGreaterOrEqual(v2, b2 - leaves));
                b3 = 2 * b3 + Bit(splitters.IsValueGreaterOrEqual(v3, b3 - leaves));
                b4 = 2 * b4 + Bit(splitters.IsValueGreaterOrEqual(v4, b4 - leaves));
                b5 = 2 * b5 + Bit(splitters.IsValueGreaterOrEqual(v5, b5 - leaves));
                b6 = 2 * b6 + Bit(splitters.IsValueGreaterOrEqual(v6, b6 - leaves));
            }
            ToBuffer(ref st, b0 - firstBucket, v0, ref write);
            ToBuffer(ref st, b1 - firstBucket, v1, ref write);
            ToBuffer(ref st, b2 - firstBucket, v2, ref write);
            ToBuffer(ref st, b3 - firstBucket, v3, ref write);
            ToBuffer(ref st, b4 - firstBucket, v4, ref write);
            ToBuffer(ref st, b5 - firstBucket, v5, ref write);
            ToBuffer(ref st, b6 - firstBucket, v6, ref write);
        }
        for (; i < end; i++)
        {
            var v = a.Read(i);
            ToBuffer(ref st, ClassifyOne(ref st, v), v, ref write);
        }

        for (var bucket = 0; bucket < st.NumBuckets; bucket++)
        {
            st.BucketSize[bucket] += st.Fill[bucket];
        }
        return write;
    }

    /// <summary>
    /// Adds an element to its bucket's buffer block, first writing a full buffer back to the range at the write offset.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void ToBuffer<T, TComparer, TContext>(ref Sorter<T, TComparer, TContext> st, int bucket, T value, ref long write)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var blockSize = st.BlockSize;
        ref var fill = ref st.Fill[bucket];
        if (fill == blockSize)
        {
            st.Blocks.CopyTo(bucket * blockSize, st.A, st.Begin + (int)write, blockSize);
            write += blockSize;
            st.BucketSize[bucket] += blockSize;
            fill = 0;
        }
        st.Blocks.Write(bucket * blockSize + (int)fill, value);
        fill++;
    }

    /// <summary>
    /// <c>computeOverflowBucket</c>: the last bucket with more than one block of elements, the only one whose last block can end past
    /// the range.
    /// </summary>
    static int ComputeOverflowBucket<T, TComparer, TContext>(ref Sorter<T, TComparer, TContext> st)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var bucket = st.NumBuckets - 1;
        while (bucket >= 0 && st.BucketStart[bucket + 1] - st.BucketStart[bucket] <= st.BlockSize)
        {
            bucket--;
        }
        return bucket;
    }

    /// <summary>
    /// <c>permuteBlocks</c>: visits the buckets in turn, and from each takes unprocessed blocks until none is left, following every
    /// chain of swaps to its end. When no full block was written back there is nothing to move, and the phase is not announced.
    /// </summary>
    static void PermuteBlocks<T, TComparer, TContext>(ref Sorter<T, TComparer, TContext> st, long firstEmptyBlock)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var a = st.A;
        var blockSize = st.BlockSize;
        if (firstEmptyBlock > 0) a.Context.OnPhase(SortPhase.Ips4oBlockPermutation, a.Offset + st.Begin, a.Offset + st.End - 1, blockSize);

        var numBuckets = st.NumBuckets;
        var maxOffset = AlignToNextBlock(st.End - st.Begin + 1, blockSize) - blockSize;
        var readBucket = 0;
        for (var count = numBuckets; count > 0; count--)
        {
            int dest;
            while ((dest = ClassifyAndReadBlock(ref st, readBucket)) != -1)
            {
                var current = 0;
                while ((dest = SwapBlock(ref st, maxOffset, dest, current)) != -1)
                {
                    current ^= 1;
                }
            }
            readBucket = (readBucket + 1) % numBuckets;
        }
    }

    /// <summary>
    /// <c>classifyAndReadBlock</c>: moves the next unprocessed block of a bucket into swap block 0 and returns the bucket of its first
    /// element, or -1 when the bucket has none left.
    /// </summary>
    static int ClassifyAndReadBlock<T, TComparer, TContext>(ref Sorter<T, TComparer, TContext> st, int readBucket)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var blockSize = st.BlockSize;
        var write = st.WritePointer[readBucket];
        var read = st.ReadPointer[readBucket];
        st.ReadPointer[readBucket] = read - blockSize;
        if (read < write) return -1;

        st.A.CopyTo(st.Begin + (int)read, st.Swap, 0, blockSize);
        return ClassifyOne(ref st, st.Swap.Read(0));
    }

    /// <summary>
    /// <c>swapBlock</c>: advances the destination bucket's write pointer past blocks that are already home, then either places the block
    /// held in swap block <paramref name="current"/> in a position that has been read (or in the overflow block) and returns -1, or swaps
    /// it with a block that belongs elsewhere and returns that block's bucket.
    /// </summary>
    static int SwapBlock<T, TComparer, TContext>(ref Sorter<T, TComparer, TContext> st, long maxOffset, int dest, int current)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var a = st.A;
        var blockSize = st.BlockSize;
        long write;
        int newDest;
        do
        {
            write = st.WritePointer[dest];
            var read = st.ReadPointer[dest];
            st.WritePointer[dest] = write + blockSize;
            if (write > read)
            {
                if (write >= maxOffset)
                {
                    st.Swap.CopyTo(current * blockSize, st.Overflow, 0, blockSize);
                    st.HasOverflow = true;
                    return -1;
                }
                st.Swap.CopyTo(current * blockSize, a, st.Begin + (int)write, blockSize);
                return -1;
            }
            newDest = ClassifyOne(ref st, a.Read(st.Begin + (int)write));
        }
        while (newDest == dest);

        a.CopyTo(st.Begin + (int)write, st.Swap, (current ^ 1) * blockSize, blockSize);
        st.Swap.CopyTo(current * blockSize, a, st.Begin + (int)write, blockSize);
        return newDest;
    }

    /// <summary>
    /// Each bucket's write pointer ends after the full blocks its classification wrote back. A comparer that classified a block's first
    /// element differently in the permutation leaves some pointer elsewhere, and the cleanup would then move elements outside the range.
    /// </summary>
    static void CheckBucketPointers<T, TComparer, TContext>(ref Sorter<T, TComparer, TContext> st)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        for (var bucket = 0; bucket < st.NumBuckets; bucket++)
        {
            var expected = AlignToNextBlock(st.BucketStart[bucket], st.BlockSize) + st.BucketSize[bucket] - st.Fill[bucket];
            if (st.WritePointer[bucket] != expected) ThrowHelper.ThrowInconsistentComparer();
        }
    }

    /// <summary>
    /// <c>writeMargins</c>: fills each bucket's partial blocks, then sorts the bucket by the base case when it is small or when this step
    /// is the last level.
    /// </summary>
    static void Cleanup<T, TComparer, TContext>(ref Sorter<T, TComparer, TContext> st, int overflowBucket)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var a = st.A;
        var begin = st.Begin;
        var blockSize = st.BlockSize;
        var isLastLevel = st.End - begin <= SINGLE_LEVEL_THRESHOLD;

        for (var i = 0; i < st.NumBuckets; i++)
        {
            var bstart = st.BucketStart[i];
            var bend = st.BucketStart[i + 1];
            var bwrite = st.WritePointer[i];
            var count = st.Fill[i];
            var fromOverflow = i == overflowBucket && st.HasOverflow;
            var crossed = !fromOverflow && bwrite > bend && bend - bstart > blockSize;

            // A bucket whose blocks are all home and whose buffer is empty moves nothing.
            if (fromOverflow || crossed || count > 0)
                a.Context.OnPhase(SortPhase.Ips4oCleanup, a.Offset + begin + (int)bstart, a.Offset + begin + (int)bend - 1, i);

            var dst = bstart;
            var remaining = AlignToNextBlock(bstart, blockSize) - bstart;

            if (fromOverflow)
            {
                // The overflow block fills the bucket's first partial block, and its tail goes to the last block position.
                var tailSize = blockSize - remaining;
                Copy(st.Overflow, 0, a, begin + dst, remaining);
                dst = bwrite - blockSize;
                Copy(st.Overflow, remaining, a, begin + dst, tailSize);
                dst += tailSize;
                remaining = long.MaxValue;
            }
            else if (crossed)
            {
                // The bucket's last block crossed into the next bucket; its head fills the bucket's first partial block.
                var headSize = bwrite - bend;
                Copy(a, begin + bend, a, begin + dst, headSize);
                dst += headSize;
                remaining -= headSize;
            }

            if (count <= remaining)
            {
                Copy(st.Blocks, i * blockSize, a, begin + dst, count);
            }
            else
            {
                Copy(st.Blocks, i * blockSize, a, begin + dst, remaining);
                Copy(st.Blocks, i * blockSize + remaining, a, begin + bwrite, count - remaining);
            }
            st.Fill[i] = 0;

            if (isLastLevel || bend - bstart <= 2 * BASE_CASE_SIZE) BaseCase(a, begin + (int)bstart, begin + (int)bend);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Copy<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> source, long sourceIndex, SortSpan<T, TComparer, TContext> destination, long destinationIndex, long length)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (length > 0) source.CopyTo((int)sourceIndex, destination, (int)destinationIndex, (int)length);
    }
}
