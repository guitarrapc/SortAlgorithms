using SortAlgorithm.Contexts;
using System.Buffers;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// Radsort。補助領域 O(√n) で動く、2^8 (256) 基数の安定な LSD 基数ソートです。
/// 入力配列を固定長ブロックの列とみなし、読み終えた入力ブロックをそのまま出力ブロックとして再利用することで、
/// 通常の LSD 基数ソートが必要とする n 要素の第 2 配列を、2σ 個のスクラッチブロックとブロック置換の管理表に置き換えます。
/// 各桁は、要素をバケットごとの出力ブロック列へ分配する「ソートフェーズ」と、ブロック置換 π を書き換えてブロックをバケット順に並べ直す
/// 「修正フェーズ」（要素は動かさない）からなり、全桁の後の「最終化」がブロックを入力配列へ詰めて戻します。
/// 符号付き整数は符号ビット反転により、負数も含めて正しくソートされます。
/// <br/>
/// Radsort: a stable LSD radix sort with radix 2^8 (256) that needs only O(√n) auxiliary space.
/// It treats the input as a sequence of fixed-size blocks and reuses each input block for output as soon as it has been consumed,
/// replacing the n-element second array of a conventional LSD radix sort with 2σ scratch blocks and a block permutation.
/// Each digit is a sort phase, which distributes the elements into per-bucket chains of output blocks, and a fixup phase, which
/// rewrites the block permutation π so the blocks appear in bucket order without moving any element. After the last digit,
/// a finalization moves the blocks back into the input array, closing the gaps that partly filled blocks leave.
/// Signed integers are handled via sign-bit flipping to maintain correct ordering including negative values.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Radsort (Base-256):</strong></para>
/// <list type="number">
/// <item><description><strong>Order-Preserving Key Mapping:</strong> Elements are mapped to fixed-width unsigned keys through
/// <see cref="IRadixKeySelector{T}"/>. Signed integers flip the sign bit, floating-point values use the IEEE 754 bit transform
/// (-0 and +0 tie), and key-selector overloads extract an int key from arbitrary elements. Keys are then normalized by the minimum
/// key, which preserves their order.</description></item>
/// <item><description><strong>Digit Extraction:</strong> For digit position d, digit = ((key − minKey) &gt;&gt; (d × 8)) &amp; 0xFF.
/// A digit is an integer derived from the key; testing it is never a call to the comparer.</description></item>
/// <item><description><strong>LSD Processing Order with a Stable Pass:</strong> Digits are processed from d = 0 upward, and each pass
/// sorts by its digit stably, so after pass d the sequence is ordered by digits d..0.</description></item>
/// <item><description><strong>Blocks and the Block Permutation:</strong> The input A of length n is viewed as ⌊n/b⌋ blocks of b
/// elements, preceded by 2σ scratch blocks T, for n_π = ⌊n/b⌋ + 2σ physical blocks. The permutation π maps a logical block index to
/// the physical block that holds it. The n mod b elements after A's last full block are copied into a scratch block at setup, where
/// they form a partial block. Between passes this invariant holds: logical blocks [0, σ) and [f, n_π) are unallocated; logical
/// blocks [σ, f) are allocated, and the used elements of those blocks, concatenated in logical order, are the input permuted by the
/// passes run so far; the block at f − 1 is partial; and the table P lists every partial block (logical index, used length) in
/// ascending logical order.</description></item>
/// <item><description><strong>Sort Phase:</strong> Every bucket first draws one output block from the head start of σ unallocated
/// blocks. The allocated blocks are then read in logical order, and each element is appended to its bucket's current block. When that
/// block fills, the bucket draws the block at the next logical index i_out, and U[i_out] records the bucket. A drawn block has always
/// been read to its end already: there are at most σ − 1 partly filled output blocks besides the one being replaced, and full output
/// blocks never outnumber the input blocks consumed, so the head start keeps i_out ≤ i_in. (The paper's Lemma 1 states i_out &lt; i_in;
/// equality occurs when a bucket fills on the last element of a full input block, which has been read by then. The reference
/// implementation asserts ≤.)</description></item>
/// <item><description><strong>Fixup Phase:</strong> A prefix sum over the per-bucket block counts gives each bucket's first logical
/// index, offset by a new head start of σ. Scanning U in logical order and placing each block at its bucket's next index builds π′,
/// with each bucket's blocks in the order they were drawn. Each bucket's current block becomes its partial block in P. No element
/// moves. At most ⌊n/b⌋ output blocks are full and σ are partial, so at least n_π − σ − ⌊n/b⌋ = σ blocks remain unallocated
/// to form the next head start.</description></item>
/// <item><description><strong>Stability:</strong> Within a pass, a bucket's elements are appended in input order to blocks drawn at
/// increasing logical indices, and the fixup keeps a bucket's blocks in the order they were drawn. Equal digits therefore leave a pass
/// in the order they entered it.</description></item>
/// <item><description><strong>Finalization (push–pull):</strong> For each physical block of A in turn: if an allocated block that
/// belongs later in the output occupies it, that block is pushed to a known unallocated block; then the next logical block is pulled
/// in, its used elements written right after the previous block's, so the gaps of partial blocks close. Every pull frees the block it
/// came from, which supplies the next push target. Once A's ⌊n/b⌋ blocks are filled, every logical block left lives in T and is
/// copied straight to its place.</description></item>
/// <item><description><strong>Block Size:</strong> Scratch costs O(b) and the block bookkeeping O(n/b), so b ∈ Θ(√n) bounds the
/// auxiliary space by O(√n). The analysis takes σ as a constant.</description></item>
/// <item><description><strong>Digit Count from the Key Range:</strong> The number of passes is d = ⌈requiredBits / 8⌉, where
/// requiredBits is the bit width of (max − min).</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution (Radix Sort, LSD variant; each digit is distributed into chains of fixed-size blocks that reuse consumed input blocks)</description></item>
/// <item><description>Stable      : Yes (input blocks are read in logical order, a bucket's blocks are drawn at increasing logical indices, and the fixup keeps that order)</description></item>
/// <item><description>In-place    : No (O(√n) auxiliary space: 2σ scratch blocks of b elements plus per-block bookkeeping, with b ∈ Θ(√n))</description></item>
/// <item><description>Best case   : Θ(n) - all keys equal: the range scan finds range 0 and no pass runs. With a non-zero range, Θ(d × n): every pass moves every element whatever the order</description></item>
/// <item><description>Average case: Θ(d × n) - each pass reads and writes every element once; the fixup touches O(n/b) block entries and moves no element</description></item>
/// <item><description>Worst case  : Θ(d × n) - the same work regardless of input order</description></item>
/// <item><description>Comparisons : 0 (Non-comparison sort: digits are integers derived from the keys, and testing one never calls the comparer)</description></item>
/// <item><description>Swaps       : 0 - elements move by being written into output blocks and by block copies in the finalization, never by exchange</description></item>
/// <item><description>Index Reads : Θ(d × n) - n (range scan) + n mod b (tail moved to scratch) + d × n (each pass reads every element from its input block) + at most 2n (finalization: every element pulled once, at most ⌊n/b⌋ whole blocks pushed once)</description></item>
/// <item><description>Index Writes: Θ(d × n) - n mod b (tail) + d × n (each pass writes every element into an output block) + at most 2n (finalization); every read except the range scan is paired with a write</description></item>
/// <item><description>Space       : O(b + n/b) = O(√n) for b ∈ Θ(√n) - 2σ × b scratch elements, π, π′ and U with ⌊n/b⌋ + 2σ entries each, and O(σ) per-bucket state</description></item>
/// <item><description>Digit Passes: d = ⌈requiredBits / 8⌉, derived from the key range rather than the key width</description></item>
/// </list>
/// <para><strong>Where This Sits Among the LSD Radix Sorts:</strong></para>
/// <para><see cref="RadixLSD256Sort"/> runs the same passes on the same radix, but distributes each digit into an n-element second
/// array after counting it, which also lets it skip a pass whose digit every element shares. Radsort has no counting pass — output
/// blocks are drawn on demand — so it cannot know a pass is the identity before running it, and every pass runs. Its advantage is
/// memory and cache locality: an output block is a block that was just read, so it is likely still in cache when written. The paper
/// measures Radsort overtaking a conventional out-of-place LSD radix sort only from about twice the L2 cache size (around 2 MiB of
/// data); below that the bookkeeping costs more than the locality saves.</para>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description><strong>Block size:</strong> b is the power of two minimising 2σ × b × sizeof(T) + 9 × (⌊n/b⌋ + 2σ) bytes —
/// the scratch blocks plus π and π′ (int) and U (byte) — the criterion of the authors' generic implementation, without its b ≥ 64
/// floor, which would make the scratch 32n for n = 1024. For int elements this gives b = 1 below n ≈ 450, b = 2 at n = 1024,
/// b = 8 at n = 8192 and b = 64 at n = 2^20. The authors' benchmarked code and the paper's experiments use a fixed b = 512 instead,
/// which turns the overhead into a fixed 2σ × 512 elements and gives up the O(√n) bound.</description></item>
/// <item><description><strong>Bucket cursors are one int:</strong> a position relative to the start of A, where scratch block j sits
/// at (j − 2σ) × b, a negative position. Because b is a power of two and every block starts at a multiple of b, a bucket's block is
/// full exactly when its next position is a multiple of b, so no end pointer is stored or loaded — the effect of the paper's §4.3
/// end-of-block check, without its alignment arithmetic. The fixup recognises a bucket's partial block by comparing physical block
/// numbers (position &gt;&gt; log₂ b) rather than end pointers.</description></item>
/// <item><description><strong>A pull that would copy a block onto itself is skipped:</strong> when the right block already sits in
/// place and no gap precedes it, the pull moves nothing.</description></item>
/// <item><description><strong>Scratch is cleared only when observed:</strong> the paper's optional dummy fill of T. A push copies a
/// whole block, including the unused tail of a partial block, and clearing T first means an observer sees no stale pool contents in
/// those copies. Unused elements never reach the output, so the unobserved path skips the clear.</description></item>
/// <item><description><strong>Not ported:</strong> the parallel operation of §4.4 — it gives each thread its own head start and
/// partial blocks, and the observation contract is a single ordered stream of operations — and skipping the finalization (§4.1),
/// which leaves the data in permuted representation instead of sorting the span.</description></item>
/// <item><description><strong>Paper erratum:</strong> Alg. 7 sets j_out ← i_in − σ; the physical block it fills is i_in + σ, as the
/// reference implementation's compact() does.</description></item>
/// </list>
/// <para><strong>Supported Key Mappings (via <see cref="IRadixKeySelector{T}"/>):</strong></para>
/// <list type="bullet">
/// <item><description><strong>Integers:</strong> byte, sbyte, short, ushort, int, uint, long, ulong, nint, nuint (up to 64-bit); Int128/UInt128/BigInteger are rejected (64-bit key ceiling, see <see cref="RadixLSD256Sort"/>)</description></item>
/// <item><description><strong>Floating point:</strong> Half, float, double via IEEE 754 bit transform (all NaN values sort first, matching <see cref="IComparable{T}"/> semantics; <c>-0.0</c> and <c>+0.0</c> are a tie and keep their input order, this sort being stable)</description></item>
/// <item><description><strong>Key selector:</strong> arbitrary element types via an extracted <c>int</c> key; equal keys retain input order, making stability observable</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Robert Clausecker and Florian Schintke, "Parallel O(√n) Overhead LSD Radix Sort", arXiv:2607.05302 (2026): https://arxiv.org/abs/2607.05302</para>
/// <para>Reference implementation (BSD-2-Clause): https://github.com/clausecker/radsort (radixsort_permuted.c)</para>
/// </remarks>
public static class Radsort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>See the stability note in this type's summary.</remarks>
    public static bool IsStable => true;

    private const int RadixBits = 8;                    // 8 bits per digit
    private const int RadixSize = 256;                  // σ: 2^8 buckets
    private const int ScratchBlocks = 2 * RadixSize;    // 2σ: head start + the partial blocks of the previous pass
    private const ulong DigitMask = RadixSize - 1;

    // Bytes of bookkeeping per block in the block-size choice: π and π′ (int each) and U (byte).
    private const int BookkeepingBytesPerBlock = 2 * sizeof(int) + sizeof(byte);

    // Keeps the scratch length (2σ << shift) far inside int range. The size policy never comes near it.
    internal const int MaxBlockShift = 20;

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array A; its blocks double as output blocks
    private const int BUFFER_SCRATCH = 1;    // Scratch blocks T: 2σ blocks of b elements

    /// <summary>
    /// Sorts the elements in the specified span.
    /// Uses NullContext for zero-overhead fast path.
    /// </summary>
    /// <typeparam name="T"> The type of elements to sort. Must be a binary integer type (up to 64-bit).</typeparam>
    /// <param name="span"> The span of elements to sort.</param>
    public static void Sort<T>(Span<T> span) where T : IBinaryInteger<T>
        => Sort(span, NullContext.Default);

    /// <summary>
    /// Sorts the elements in the specified span.
    /// </summary>
    /// <typeparam name="T"> The type of elements to sort. Must be a binary integer type (up to 64-bit).</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="span"> The span of elements to sort.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation.</param>
    /// <exception cref="NotSupportedException">
    /// Thrown when <typeparamref name="T"/> is a 128-bit type (<see cref="Int128"/> or <see cref="UInt128"/>).
    /// This implementation only supports integer types up to 64-bit.
    /// </exception>
    public static void Sort<T, TContext>(Span<T> span, TContext context)
        where T : IBinaryInteger<T>
        where TContext : ISortContext
        => SortCore(span, default(BinaryIntegerRadixKey<T>), new ComparableComparer<T>(), context);

    /// <summary>
    /// Sorts the elements in the specified span by an integer key extracted with <paramref name="keySelector"/>.
    /// Elements with equal keys retain their relative input order (stable).
    /// Uses NullContext for zero-overhead fast path.
    /// </summary>
    /// <typeparam name="T">The type of elements to sort.</typeparam>
    /// <param name="span">The span of elements to sort.</param>
    /// <param name="keySelector">Extracts the integer sort key from an element. Must be pure and consistent per element.</param>
    public static void SortBy<T>(Span<T> span, Func<T, int> keySelector)
    {
        ArgumentNullException.ThrowIfNull(keySelector);
        var selector = new FuncRadixKeySelector<T>(keySelector);
        SortCore(span, selector, new RadixKeyComparer<T, FuncRadixKeySelector<T>>(selector), NullContext.Default);
    }

    /// <summary>
    /// Sorts the elements in the specified span by an integer key extracted with <paramref name="keySelector"/>.
    /// Elements with equal keys retain their relative input order (stable).
    /// </summary>
    /// <typeparam name="T">The type of elements to sort.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="span">The span of elements to sort.</param>
    /// <param name="keySelector">Extracts the integer sort key from an element. Must be pure and consistent per element.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation.</param>
    public static void SortBy<T, TContext>(Span<T> span, Func<T, int> keySelector, TContext context)
        where TContext : ISortContext
    {
        ArgumentNullException.ThrowIfNull(keySelector);
        var selector = new FuncRadixKeySelector<T>(keySelector);
        SortCore(span, selector, new RadixKeyComparer<T, FuncRadixKeySelector<T>>(selector), context);
    }

    /// <summary>
    /// Sorts the elements in the specified span by keys produced with a custom
    /// <see cref="IRadixKeySelector{T}"/> implementation (full-control overload, up to 64-bit keys).
    /// Implement the selector as a readonly struct for JIT devirtualization and inlining.
    /// Elements with equal keys retain their relative input order (stable).
    /// Uses NullContext for zero-overhead fast path.
    /// </summary>
    /// <typeparam name="T">The type of elements to sort.</typeparam>
    /// <typeparam name="TRadixKey">The radix key selector type. Must be a struct implementing <see cref="IRadixKeySelector{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort.</param>
    /// <param name="radixKey">Maps an element to its order-preserving unsigned key.</param>
    public static void SortBy<T, TRadixKey>(Span<T> span, TRadixKey radixKey)
        where TRadixKey : struct, IRadixKeySelector<T>
        => SortCore(span, radixKey, new RadixKeyComparer<T, TRadixKey>(radixKey), NullContext.Default);

    /// <inheritdoc cref="SortBy{T, TRadixKey}(Span{T}, TRadixKey)"/>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation.</param>
    public static void SortBy<T, TRadixKey, TContext>(Span<T> span, TRadixKey radixKey, TContext context)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TContext : ISortContext
        => SortCore(span, radixKey, new RadixKeyComparer<T, TRadixKey>(radixKey), context);

    /// <summary>
    /// Sorts <see cref="Half"/> values via the IEEE 754 bit transform.
    /// All NaN values sort first, matching <see cref="IComparable{T}"/> semantics.
    /// </summary>
    public static void Sort(Span<Half> span)
        => SortCore(span, default(HalfRadixKey), new ComparableComparer<Half>(), NullContext.Default);

    /// <inheritdoc cref="Sort(Span{Half})"/>
    public static void Sort<TContext>(Span<Half> span, TContext context) where TContext : ISortContext
        => SortCore(span, default(HalfRadixKey), new ComparableComparer<Half>(), context);

    /// <summary>
    /// Sorts <see cref="float"/> values via the IEEE 754 bit transform.
    /// All NaN values sort first, matching <see cref="IComparable{T}"/> semantics.
    /// </summary>
    public static void Sort(Span<float> span)
        => SortCore(span, default(SingleRadixKey), new ComparableComparer<float>(), NullContext.Default);

    /// <inheritdoc cref="Sort(Span{float})"/>
    public static void Sort<TContext>(Span<float> span, TContext context) where TContext : ISortContext
        => SortCore(span, default(SingleRadixKey), new ComparableComparer<float>(), context);

    /// <summary>
    /// Sorts <see cref="double"/> values via the IEEE 754 bit transform.
    /// All NaN values sort first, matching <see cref="IComparable{T}"/> semantics.
    /// </summary>
    public static void Sort(Span<double> span)
        => SortCore(span, default(DoubleRadixKey), new ComparableComparer<double>(), NullContext.Default);

    /// <inheritdoc cref="Sort(Span{double})"/>
    public static void Sort<TContext>(Span<double> span, TContext context) where TContext : ISortContext
        => SortCore(span, default(DoubleRadixKey), new ComparableComparer<double>(), context);

    /// <summary>
    /// log₂ of the block size for <paramref name="n"/> elements of <paramref name="elementBytes"/> bytes: the power of two b that
    /// minimises 2σ × b × elementBytes (the scratch blocks) + 9 × (⌊n/b⌋ + 2σ) (π, π′ and U), with ties going to the larger b.
    /// The first term grows with b and the second shrinks, so the minimum lies near b = √(9n / (2σ × elementBytes)): b ∈ Θ(√n).
    /// </summary>
    internal static int BlockShift(int n, int elementBytes)
    {
        var shift = 0;
        var footprint = Footprint(n, elementBytes, 0);
        while (shift < MaxBlockShift)
        {
            var next = Footprint(n, elementBytes, shift + 1);
            if (next > footprint) break;
            footprint = next;
            shift++;
        }
        return shift;

        static long Footprint(int n, int elementBytes, int shift)
            => ((long)ScratchBlocks * elementBytes << shift)
             + ((long)(n >> shift) + ScratchBlocks) * BookkeepingBytesPerBlock;
    }

    /// <summary>
    /// Sorts with an explicit block size of 2^<paramref name="blockShift"/> instead of the size policy. The algorithm is correct for
    /// any block size, and the policy picks one from n alone, so this is how tests reach shapes the policy never produces for a
    /// given n — every element in scratch (n &lt; b), or blocks far larger than the buckets they hold.
    /// </summary>
    internal static void SortWithBlockShift<T, TRadixKey, TContext>(Span<T> span, TRadixKey radixKey, int blockShift, TContext context)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TContext : ISortContext
    {
        ArgumentOutOfRangeException.ThrowIfNegative(blockShift);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(blockShift, MaxBlockShift);
        SortCore(span, radixKey, new RadixKeyComparer<T, TRadixKey>(radixKey), context, blockShift);
    }

    private static void SortCore<T, TRadixKey, TComparer, TContext>(Span<T> span, TRadixKey radixKey, TComparer comparer, TContext context)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
        => SortCore(span, radixKey, comparer, context, BlockShift(span.Length, Unsafe.SizeOf<T>()));

    private static void SortCore<T, TRadixKey, TComparer, TContext>(Span<T> span, TRadixKey radixKey, TComparer comparer, TContext context, int blockShift)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;
        RadixKeyGuard.ValidateKeyBits<T, TRadixKey>();

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);

        // Announce the range scan: it reads every element and moves none, and without the announcement a
        // consumer would see those reads under whatever phase was current before the sort started.
        s.Context.OnPhase(SortPhase.KeyRangeScan);

        var minKey = ulong.MaxValue;
        var maxKey = ulong.MinValue;
        for (var i = 0; i < s.Length; i++)
        {
            var key = radixKey.GetKey(s.Read(i));
            if (key < minKey) minKey = key;
            if (key > maxKey) maxKey = key;
        }

        // Every key equal: there is no digit to sort by.
        var range = maxKey - minKey;
        if (range == 0) return;

        // Digits are taken from (key - minKey), whose values span [0, range], so the pass count follows the
        // width of the range rather than the position of the keys: bitlength(max - min) <= bitlength(max ^ min).
        var requiredBits = 64 - BitOperations.LeadingZeroCount(range);
        var digitCount = (requiredBits + RadixBits - 1) / RadixBits;

        var n = span.Length;
        var blockCount = (n >> blockShift) + ScratchBlocks;
        var scratchLength = ScratchBlocks << blockShift;

        var scratchArray = ArrayPool<T>.Shared.Rent(scratchLength);
        var permArray = ArrayPool<int>.Shared.Rent(blockCount);
        var perm2Array = ArrayPool<int>.Shared.Rent(blockCount);
        var usageArray = ArrayPool<byte>.Shared.Rent(blockCount);

        // Per-bucket state: the partial-block table P lives across passes, the rest is rebuilt by each pass.
        Span<int> bucketState = stackalloc int[5 * RadixSize];
        var partialIndex = bucketState.Slice(0 * RadixSize, RadixSize);
        var partialLength = bucketState.Slice(1 * RadixSize, RadixSize);
        var cursors = bucketState.Slice(2 * RadixSize, RadixSize);
        var counts = bucketState.Slice(3 * RadixSize, RadixSize);
        var starts = bucketState.Slice(4 * RadixSize, RadixSize);

        try
        {
            var scratchSpan = scratchArray.AsSpan(0, scratchLength);

            // The paper's optional dummy fill of T. Pushes copy whole blocks, unused tails included, so an observer
            // would otherwise see whatever the pool left in them; unobserved, those elements never reach the output.
            if (typeof(TContext) != typeof(NullContext))
            {
                scratchSpan.Clear();
            }

            var scratch = new SortSpan<T, TComparer, TContext>(scratchSpan, context, comparer, BUFFER_SCRATCH);
            var sorter = new Sorter<T, TComparer, TContext>(
                s, scratch,
                permArray.AsSpan(0, blockCount), perm2Array.AsSpan(0, blockCount), usageArray.AsSpan(0, blockCount),
                partialIndex, partialLength, blockShift);

            // Setup moves the tail into scratch, so it is announced as part of the first pass rather than of the scan,
            // which moves nothing.
            s.Context.OnPhase(SortPhase.RadixPass, 0, digitCount);
            sorter.Setup();

            for (var d = 0; d < digitCount; d++)
            {
                if (d > 0) s.Context.OnPhase(SortPhase.RadixPass, d, digitCount);
                var digit = new DigitSelector<T, TRadixKey>(radixKey, minKey, d * RadixBits);
                var outputBlocks = sorter.Distribute(digit, cursors, counts);
                sorter.Fixup(outputBlocks, cursors, counts, starts);
            }

            // The finalization collects the bucket-ordered blocks back into the span.
            s.Context.OnPhase(SortPhase.DistributionWrite);
            sorter.Compact();
        }
        finally
        {
            ArrayPool<T>.Shared.Return(scratchArray, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
            ArrayPool<int>.Shared.Return(permArray);
            ArrayPool<int>.Shared.Return(perm2Array);
            ArrayPool<byte>.Shared.Return(usageArray);
        }
    }

    /// <summary>
    /// The state of one Radsort run: the paper's A, T, π, U, P and f, and the procedures that act on them.
    /// </summary>
    /// <remarks>
    /// Logical block indices are written i, physical ones j. Physical blocks [0, 2σ) are the scratch blocks; physical block j ≥ 2σ
    /// is A[(j − 2σ) × b, (j − 2σ + 1) × b). A position is an element index relative to the start of A, so scratch positions are
    /// negative: physical block j starts at position (j − 2σ) × b whichever buffer holds it.
    /// </remarks>
    private ref struct Sorter<T, TComparer, TContext>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        readonly SortSpan<T, TComparer, TContext> main;      // A
        readonly SortSpan<T, TComparer, TContext> scratch;   // T
        Span<int> perm;                                      // π: logical block → physical block
        Span<int> perm2;                                     // π′ in the fixup, π⁻¹ in the finalization
        readonly Span<byte> usage;                           // U: bucket of each output block, by logical index
        readonly Span<int> partialIndex;                     // P: logical index of each partial block, ascending
        readonly Span<int> partialLength;                    // P: used elements of each partial block
        readonly int n;
        readonly int shift;                                  // log₂ b
        readonly int blockCount;                             // n_π = ⌊n/b⌋ + 2σ
        int fill;                                            // f: one past the last allocated logical block

        public Sorter(
            SortSpan<T, TComparer, TContext> main,
            SortSpan<T, TComparer, TContext> scratch,
            Span<int> perm, Span<int> perm2, Span<byte> usage,
            Span<int> partialIndex, Span<int> partialLength,
            int shift)
        {
            this.main = main;
            this.scratch = scratch;
            this.perm = perm;
            this.perm2 = perm2;
            this.usage = usage;
            this.partialIndex = partialIndex;
            this.partialLength = partialLength;
            this.shift = shift;
            n = main.Length;
            blockCount = perm.Length;
            fill = 0;
        }

        readonly int BlockSize => 1 << shift;

        /// <summary>Position of the first element of physical block <paramref name="block"/>.</summary>
        readonly int StartOf(int block) => (block - ScratchBlocks) << shift;

        /// <summary>
        /// Alg. 3: logical [0, σ) is the head start (scratch blocks 0..σ − 1), logical [σ, f − 1) are A's full blocks in order, and
        /// logical [f − 1, n_π) are scratch blocks σ..2σ − 1, the first of which receives the n mod b elements after A's last full block.
        /// </summary>
        public void Setup()
        {
            var fullBlocks = n >> shift;
            fill = RadixSize + fullBlocks + 1;

            var i = 0;
            for (; i < RadixSize; i++) perm[i] = i;
            for (; i < fill - 1; i++) perm[i] = i + RadixSize;
            for (; i < blockCount; i++) perm[i] = i - (fill - 1) + RadixSize;

            var tail = n - (fullBlocks << shift);
            if (tail > 0)
            {
                main.CopyTo(fullBlocks << shift, scratch, RadixSize << shift, tail);
            }

            // The tail is the only partial block. The other entries never match a logical index below n_π.
            partialIndex[0] = fill - 1;
            partialLength[0] = tail;
            partialIndex[1..].Fill(blockCount);
            partialLength[1..].Clear();
        }

        /// <summary>
        /// Alg. 5 (sort phase): distributes the allocated blocks, read in logical order, into per-bucket chains of output blocks drawn
        /// from logical index 0 upward. Leaves each bucket's next position in <paramref name="cursors"/> and its block count in
        /// <paramref name="counts"/>, records each output block's bucket in U, and returns the number of output blocks drawn.
        /// </summary>
        public readonly int Distribute<TRadixKey>(DigitSelector<T, TRadixKey> digit, Span<int> cursors, Span<int> counts)
            where TRadixKey : struct, IRadixKeySelector<T>
        {
            var main = this.main;
            var scratch = this.scratch;
            var perm = this.perm;
            var usage = this.usage;
            var partialIndex = this.partialIndex;
            var partialLength = this.partialLength;
            var shift = this.shift;
            var blockSize = 1 << shift;
            var blockMask = blockSize - 1;
            var scratchLength = scratch.Length;
            var fill = this.fill;

            // One initial output block per bucket, from the head start.
            for (var c = 0; c < RadixSize; c++)
            {
                cursors[c] = (perm[c] - ScratchBlocks) << shift;
                usage[c] = (byte)c;
                counts[c] = 1;
            }

            var iOut = RadixSize;
            var iPartial = 0;
            for (var iIn = RadixSize; iIn < fill; iIn++)
            {
                var block = perm[iIn];
                var length = blockSize;
                if (partialIndex[iPartial] == iIn)
                {
                    length = partialLength[iPartial];
                    iPartial++;
                }

                var source = block < ScratchBlocks ? scratch : main;
                var sourceStart = block < ScratchBlocks ? block << shift : (block - ScratchBlocks) << shift;

                for (var k = 0; k < length; k++)
                {
                    var value = source.Read(sourceStart + k);
                    var c = digit.Of(value);

                    var position = cursors[c];
                    if (position < 0)
                        scratch.Write(position + scratchLength, value);
                    else
                        main.Write(position, value);
                    position++;

                    // Every block starts at a multiple of b, so reaching one means the block just filled.
                    if ((position & blockMask) == 0)
                    {
                        // Lemma 1: the drawn block has been read to its end (equality only after the last element of block iIn).
                        Debug.Assert(iOut <= iIn);
                        position = (perm[iOut] - ScratchBlocks) << shift;
                        usage[iOut] = (byte)c;
                        counts[c]++;
                        iOut++;
                    }
                    cursors[c] = position;
                }
            }

            return iOut;
        }

        /// <summary>
        /// Alg. 6 (fixup phase): rebuilds π so the <paramref name="outputBlocks"/> output blocks follow a new head start in bucket order,
        /// with each bucket's blocks in the order they were drawn, records each bucket's current block as its partial block, and moves
        /// no element.
        /// </summary>
        public void Fixup(int outputBlocks, ReadOnlySpan<int> cursors, ReadOnlySpan<int> counts, Span<int> starts)
        {
            var blockMask = BlockSize - 1;

            starts[0] = RadixSize;
            for (var c = 1; c < RadixSize; c++)
            {
                starts[c] = starts[c - 1] + counts[c - 1];
            }

            for (var i = 0; i < outputBlocks; i++)
            {
                var c = usage[i];
                var k = starts[c]++;
                var block = perm[i];
                perm2[k] = block;

                // The bucket's current block is the one its cursor points into; it becomes the bucket's partial block.
                var cursor = cursors[c];
                if (block == (cursor >> shift) + ScratchBlocks)
                {
                    partialIndex[c] = k;
                    partialLength[c] = cursor & blockMask;
                }
            }

            // Lemma 2: at least σ unallocated blocks follow the output blocks; the first σ become the next head start.
            Debug.Assert(outputBlocks + RadixSize <= blockCount);
            perm.Slice(outputBlocks, RadixSize).CopyTo(perm2);
            perm[(outputBlocks + RadixSize)..].CopyTo(perm2[(outputBlocks + RadixSize)..]);

            fill = outputBlocks + RadixSize;
            var previous = perm;
            perm = perm2;
            perm2 = previous;
        }

        /// <summary>
        /// Alg. 7 (finalize; compact() in the reference implementation): moves the used elements of logical blocks σ..f − 1, in
        /// order, to the front of A. Physical blocks of A are filled in order; an allocated block that belongs later is pushed to a
        /// free block first, and every pull from elsewhere frees the block it came from. Logical blocks left after A's blocks are
        /// filled all live in T and are copied straight to their place.
        /// </summary>
        public readonly void Compact()
        {
            var perm = this.perm;
            var inverse = perm2;
            for (var i = 0; i < blockCount; i++)
            {
                inverse[perm[i]] = i;
            }

            var blockSize = BlockSize;
            var start = 0;              // where the next block's elements go in A
            var iPartial = 0;
            var jFree = perm[0];        // an unallocated block, guaranteed by the invariant
            var iFree = 0;

            var jOut = ScratchBlocks;
            for (; jOut < blockCount; jOut++)
            {
                var iIn = jOut - RadixSize;
                var iOut = inverse[jOut];
                var jIn = perm[iIn];

                // An allocated block that belongs later sits where the next one goes: push it to the free block.
                if (iOut > iIn && iOut < fill)
                {
                    Debug.Assert(iFree < RadixSize || iFree >= fill);
                    CopyBlock(jOut, jFree, blockSize);
                    perm[iFree] = jOut;
                    perm[iOut] = jFree;
                    inverse[jFree] = iOut;
                    inverse[jOut] = iFree;
                    iFree = iOut;
                    iOut = inverse[jOut];
                }

                // Pull logical block iIn to the front, then record it as living at jOut.
                var length = NextLength(iIn, ref iPartial);
                Pull(jIn, length, start);
                perm[iIn] = jOut;
                perm[iOut] = jIn;
                inverse[jIn] = iOut;
                inverse[jOut] = iIn;
                start += length;

                // Pulled from elsewhere: the block it came from is now free.
                if (iIn != iOut)
                {
                    jFree = jIn;
                    iFree = iOut;
                }
            }

            for (var iIn = jOut - RadixSize; iIn < fill; iIn++)
            {
                var jIn = perm[iIn];
                Debug.Assert(jIn < ScratchBlocks);
                var length = NextLength(iIn, ref iPartial);
                Pull(jIn, length, start);
                start += length;
            }

            Debug.Assert(start == n);
        }

        /// <summary>Alg. 2: the used length of logical block <paramref name="i"/>, advancing past it in P if it is partial.</summary>
        readonly int NextLength(int i, ref int iPartial)
        {
            if (partialIndex[iPartial] == i) return partialLength[iPartial++];
            return BlockSize;
        }

        /// <summary>Copies the first <paramref name="length"/> elements of physical block <paramref name="from"/> into physical block <paramref name="to"/>.</summary>
        readonly void CopyBlock(int from, int to, int length)
        {
            var destination = to < ScratchBlocks ? scratch : main;
            var destinationStart = to < ScratchBlocks ? to << shift : StartOf(to);
            if (from < ScratchBlocks)
                scratch.CopyTo(from << shift, destination, destinationStart, length);
            else
                main.CopyTo(StartOf(from), destination, destinationStart, length);
        }

        /// <summary>
        /// Copies the first <paramref name="length"/> elements of physical block <paramref name="from"/> to A[<paramref name="start"/>..].
        /// The ranges may overlap when the block is in A; the copy has memmove semantics. A block already in place with no gap before
        /// it would be copied onto itself, so that pull is skipped.
        /// </summary>
        readonly void Pull(int from, int length, int start)
        {
            if (length == 0) return;
            if (from < ScratchBlocks)
            {
                scratch.CopyTo(from << shift, main, start, length);
            }
            else
            {
                var source = StartOf(from);
                if (source != start) main.CopyTo(source, main, start, length);
            }
        }
    }

    /// <summary>
    /// Extracts one radix-256 digit of an element's normalized key.
    /// </summary>
    private readonly struct DigitSelector<T, TRadixKey>(TRadixKey radixKey, ulong minKey, int shift)
        where TRadixKey : struct, IRadixKeySelector<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Of(T value) => (int)(((radixKey.GetKey(value) - minKey) >> shift) & DigitMask);
    }
}
