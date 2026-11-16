using SortAlgorithm.Contexts;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 2^2 (4) 基数の Rotate LSD 基数ソート。補助バッファを使わない、インプレースで安定な LSD 基数ソートです。
/// 最下位桁から最上位桁へ 1 桁ずつパスを進め、各パスでは「現在の桁の値だけをキーとする安定なマージソート」を行います。
/// マージは要素を退避せず、桁の値域を二分し、両ランの境界を二分探索で求め、境界をまたぐ 2 ブロックを回転（rotation）で入れ替えることで行います。
/// 符号付き整数は符号ビット反転により、負数も含めて正しくソートされます。
/// <br/>
/// Rotate LSD Radix Sort with radix 2^2 (4): an in-place, stable LSD radix sort that needs no auxiliary buffer.
/// Digits are processed from the least significant to the most significant, and each pass is a stable merge sort keyed on the current digit alone.
/// The merge moves no element into scratch storage: it halves the digit range, binary searches both runs for where the halves meet,
/// and rotates the two blocks that straddle the split into place.
/// Signed integers are handled via sign-bit flipping to maintain correct ordering including negative values.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Rotate LSD Radix Sort (Base-4):</strong></para>
/// <list type="number">
/// <item><description><strong>Order-Preserving Key Mapping:</strong> Elements are mapped to fixed-width unsigned keys through
/// <see cref="IRadixKeySelector{T}"/>. Signed integers flip the sign bit, floating-point values use the IEEE 754 bit transform
/// (-0 and +0 tie), and key-selector overloads extract an int key from arbitrary elements. Keys are then normalized by the minimum
/// key, which preserves their order.</description></item>
/// <item><description><strong>Digit Extraction:</strong> For digit position d, digit = ((key − minKey) &gt;&gt; (d × 2)) &amp; 0b11.
/// A digit is an integer derived from the key; testing it is never a call to the comparer.</description></item>
/// <item><description><strong>LSD Processing Order with a Stable Pass:</strong> Digits are processed from d = 0 upward, and each pass
/// sorts by its digit stably. After pass d the sequence is ordered by digits d..0, because a stable pass keeps the order the earlier
/// passes established among elements that share digit d. Stability of the pass is therefore what makes the sort correct, not an
/// extra property of it.</description></item>
/// <item><description><strong>The Pass Is a Merge Sort on One Digit:</strong> The range is halved recursively down to single elements,
/// and every pair of adjacent runs, each already stably ordered by the digit, is merged. A merge sort is stable whenever its merge is.</description></item>
/// <item><description><strong>Merge by Bisecting the Digit Range:</strong> Merge(a, m, b, [da, db)) receives two runs [a, m) and [m, b)
/// that are ordered by the digit and whose digits all lie in [da, db). With dm = ⌊(da + db) / 2⌋, a lower-bound binary search finds the
/// first element of each run whose digit is ≥ dm: m1 in the left run, m2 in the right. That cuts the range into four blocks
/// [low-left | high-left | low-right | high-right]; rotating the middle two gives [low-left | low-right | high-left | high-right].
/// The first half holds every digit in [da, dm) and the second every digit in [dm, db), each half again two ordered runs, so both are
/// merged recursively on the halved digit range. A radix of 4 makes the bisection exactly two levels deep.</description></item>
/// <item><description><strong>Stability of the Merge:</strong> In every digit group the block taken from the left run precedes the block
/// taken from the right run, and a rotation keeps the order inside each block it moves. Equal digits therefore leave a merge in the
/// order they entered it.</description></item>
/// <item><description><strong>Termination:</strong> A merge ends when its range holds fewer than two elements, or when its digit range
/// holds a single value — then every element in the range has the same digit, and the stable order of equal digits is the input order,
/// which is already in place.</description></item>
/// <item><description><strong>Rotation by Block Swap (Gries–Mills):</strong> [A | B] becomes [B | A] by repeatedly swapping the shorter
/// side with the adjacent end of the longer side, which settles that many elements for good each time. A rotation of l + r elements
/// costs l + r − gcd(l, r) swaps. Every element movement in this algorithm is one of these swaps.</description></item>
/// <item><description><strong>Digit Count from the Key Range:</strong> The number of passes is d = ⌈requiredBits / 2⌉, where
/// requiredBits is the bit width of (max − min). Normalizing by the minimum keeps this small when the keys straddle a high bit boundary,
/// as the sign-flipped keys of signed values around zero do.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution (Radix Sort, LSD variant; each digit is sorted by an in-place rotation merge sort)</description></item>
/// <item><description>Stable      : Yes (every merge puts the left run's block before the right run's block in each digit group, and rotations keep the order inside a block)</description></item>
/// <item><description>In-place    : Yes (no auxiliary element buffer; only the recursion stack)</description></item>
/// <item><description>Best case   : Θ(n) - all keys equal: the range scan finds range 0 and no pass runs. With a non-zero range, Θ(d × n) when every pass finds its digit already in order: each binary search lands on the run boundary and no rotation moves anything</description></item>
/// <item><description>Average case: Θ(d × n log n) - each pass is a merge sort whose merges rotate a constant fraction of their elements</description></item>
/// <item><description>Worst case  : O(d × n log n) - a merge of L elements rotates at most L elements on each level of the digit bisection, and radix 4 bisects twice</description></item>
/// <item><description>Comparisons : 0 (Non-comparison sort: digits are integers derived from the keys, and testing one never calls the comparer)</description></item>
/// <item><description>Swaps       : Best 0, Average Θ(d × n log n), Worst O(d × n log n) - every element movement is a block-swap rotation, l + r − gcd(l, r) swaps for a rotation of l + r elements</description></item>
/// <item><description>Index Reads : Best Θ(n), Average Θ(d × n log n), Worst O(d × n log n) - n (range scan) + 2 per swap + the binary-search probes; the probes cost O(log L) per merge of L elements, which sums to Θ(n) per pass, so the rotations dominate</description></item>
/// <item><description>Index Writes: Best 0, Average Θ(d × n log n), Worst O(d × n log n) - 2 per swap; nothing is written except by a swap</description></item>
/// <item><description>Space       : O(log n) - recursion stack of the per-digit merge sort (depth ⌈log₂ n⌉) plus the digit bisection (depth 2); no auxiliary element buffer</description></item>
/// <item><description>Digit Passes: d = ⌈requiredBits / 2⌉, derived from the key range rather than the key width</description></item>
/// <item><description>Rotations   : O(n) per pass - at most one per node of the radix-4 digit bisection (three per merge), and a merge sort of n elements performs n − 1 merges</description></item>
/// </list>
/// <para><strong>Where This Sits Among the LSD Radix Sorts:</strong></para>
/// <para><see cref="RadixLSD4Sort"/> distributes each digit into a second array of n elements and costs Θ(n) per pass. Rotate LSD
/// performs the same passes in the same order, but sorts each digit inside the input with a rotation merge sort, trading that buffer
/// for a log n factor per pass. Both are stable and both derive the pass count from the key range. RadixLSD4Sort also skips a pass whose
/// digit every element shares; Rotate LSD runs it, but such a pass moves nothing — every split puts both runs wholly on one side, so
/// every rotation has an empty side — and costs only its Θ(n) probes, so a separate identity check would save little.</para>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description><strong>Empty-run merge:</strong> a merge one of whose runs is empty returns at once. The reference implementation
/// recurses into it, binary searches the empty run (no probe) and the other run, and rotates nothing at every level; skipping it removes
/// those probes and changes no element movement.</description></item>
/// <item><description><strong>Keys are recomputed, not memoized:</strong> a probe reads the element and derives its digit. Memoizing keys
/// would need an n-element key array permuted alongside the elements, which is exactly the buffer this algorithm exists to avoid.</description></item>
/// <item><description><strong>Midpoint:</strong> the merge sort splits at a + (b − a) / 2, which equals the reference's (a + b) / 2 for every
/// range that does not overflow.</description></item>
/// </list>
/// <para><strong>Supported Key Mappings (via <see cref="IRadixKeySelector{T}"/>):</strong></para>
/// <list type="bullet">
/// <item><description><strong>Integers:</strong> byte, sbyte, short, ushort, int, uint, long, ulong, nint, nuint (up to 64-bit); Int128/UInt128/BigInteger are rejected (64-bit key ceiling, see <see cref="RadixLSD4Sort"/>)</description></item>
/// <item><description><strong>Floating point:</strong> Half, float, double via IEEE 754 bit transform (all NaN values sort first, matching <see cref="IComparable{T}"/> semantics; <c>-0.0</c> and <c>+0.0</c> are a tie and keep their input order, this sort being stable)</description></item>
/// <item><description><strong>Key selector:</strong> arbitrary element types via an extracted <c>int</c> key; equal keys retain input order, making stability observable</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>aphitorite, "Rotate LSD Radix Sort", ArrayV (MIT License, 2020–2021): https://github.com/Gaming32/ArrayV/blob/main/src/main/java/io/github/arrayv/sorts/distribute/RotateLSDRadixSort.java</para>
/// <para>Block-swap rotation: David Gries and Harlan Mills, "Swapping Sections", Technical Report TR 81-452, Cornell University (1981)</para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Radix_sort#Least_significant_digit</para>
/// </remarks>
public static class RotateLSDRadixSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>See the stability note in this type's summary.</remarks>
    public static bool IsStable => true;

    private const int RadixBits = 2;        // 2 bits per digit
    private const int RadixSize = 4;        // 2^2 = 4 digit values
    private const ulong DigitMask = RadixSize - 1;

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array (in-place operations only)

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

    private static void SortCore<T, TRadixKey, TComparer, TContext>(Span<T> span, TRadixKey radixKey, TComparer comparer, TContext context)
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

        for (var d = 0; d < digitCount; d++)
        {
            s.Context.OnPhase(SortPhase.RadixPass, d, digitCount);
            var digit = new DigitSelector<T, TRadixKey>(radixKey, minKey, d * RadixBits);
            MergeSort(s, digit, 0, s.Length);
        }
    }

    /// <summary>
    /// Stable merge sort of [a, b) keyed on the current digit alone.
    /// </summary>
    private static void MergeSort<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, DigitSelector<T, TRadixKey> digit, int a, int b)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (b - a < 2) return;

        var m = a + (b - a) / 2;
        MergeSort(s, digit, a, m);
        MergeSort(s, digit, m, b);
        Merge(s, digit, a, m, b, 0, RadixSize);
    }

    /// <summary>
    /// Merges the digit-ordered runs [a, m) and [m, b), all of whose digits lie in [da, db), by bisecting the digit range:
    /// [low-left | high-left | low-right | high-right] → rotate the middle two → [low-left | low-right | high-left | high-right],
    /// then merges each half on its half of the digit range.
    /// </summary>
    private static void Merge<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, DigitSelector<T, TRadixKey> digit, int a, int m, int b, int da, int db)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Fewer than two elements, or a single digit value: the range is already in stable digit order.
        if (b - a < 2 || db - da < 2) return;

        // One run empty: the other run is the whole range and is already ordered (see the implementation note).
        if (m == a || m == b) return;

        var dm = (da + db) / 2;
        var m1 = LowerBoundDigit(s, digit, a, m, dm);
        var m2 = LowerBoundDigit(s, digit, m, b, dm);

        Rotate(s, m1, m, m2);
        m = m1 + (m2 - m);

        Merge(s, digit, m, m2, b, dm, db);
        Merge(s, digit, a, m1, m, da, dm);
    }

    /// <summary>
    /// Returns the first index in [lo, hi) whose digit is ≥ <paramref name="target"/>, or <paramref name="hi"/> if none is.
    /// [lo, hi) must be ordered by the digit.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int LowerBoundDigit<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, DigitSelector<T, TRadixKey> digit, int lo, int hi, int target)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        while (lo < hi)
        {
            var mid = lo + (hi - lo) / 2;
            if (digit.Of(s.Read(mid)) >= target)
                hi = mid;
            else
                lo = mid + 1;
        }
        return lo;
    }

    /// <summary>
    /// Rotates [a, b) so that [m, b) comes before [a, m), by block swaps (Gries–Mills): the shorter side is swapped with the
    /// adjacent end of the longer side, which puts that many elements in their final place, and the rest is rotated the same way.
    /// Costs (b − a) − gcd(m − a, b − m) swaps.
    /// </summary>
    private static void Rotate<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int a, int m, int b)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var l = m - a;
        var r = b - m;

        while (l > 0 && r > 0)
        {
            if (r < l)
            {
                // [A1 A2 | B] with |A2| = |B|: swap A2 and B → [A1 B | A2], A2 is final; rotate [A1 | B].
                SwapBlocks(s, m - r, m, r);
                b -= r;
                m -= r;
                l -= r;
            }
            else
            {
                // [A | B1 B2] with |B1| = |A|: swap A and B1 → [B1 | A B2], B1 is final; rotate [A | B2].
                SwapBlocks(s, a, m, l);
                a += l;
                m += l;
                r -= l;
            }
        }
    }

    /// <summary>
    /// Swaps the <paramref name="length"/> elements starting at <paramref name="x"/> with those starting at <paramref name="y"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SwapBlocks<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int x, int y, int length)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        for (var i = 0; i < length; i++)
            s.Swap(x + i, y + i);
    }

    /// <summary>
    /// Extracts one radix-4 digit of an element's normalized key.
    /// </summary>
    private readonly struct DigitSelector<T, TRadixKey>(TRadixKey radixKey, ulong minKey, int shift)
        where TRadixKey : struct, IRadixKeySelector<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Of(T value) => (int)(((radixKey.GetKey(value) - minKey) >> shift) & DigitMask);
    }
}
