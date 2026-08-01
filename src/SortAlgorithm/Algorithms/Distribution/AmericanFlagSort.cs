using SortAlgorithm.Contexts;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// American Flag Sort - In-place MSD Radix Sortの実装。
/// 値をビット列として扱い、4ビットずつ（16種類）の桁に分けて要素を分類し、in-placeで並び替えます。
/// 最上位桁（Most Significant Digit）から最下位桁へ向かって処理することで、再帰的にソートを実現します。
/// <see cref="RadixMSD4Sort"/>と異なり補助バッファを一切確保せず、配列内で要素をスワップすることでin-placeソートを実現します。
/// <br/>
/// American Flag Sort - An in-place MSD Radix Sort implementation.
/// Treats values as bit sequences, dividing them into 8-bit digits (256 buckets) and classifying elements in-place.
/// Processing from the Most Significant Digit to the least significant ensures a recursive sort.
/// Unlike <see cref="RadixMSD4Sort"/>, this implementation allocates no auxiliary buffer at all and achieves in-place sorting by swapping elements within the array.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct American Flag Sort (Base-256):</strong></para>
/// <list type="number">
/// <item><description><strong>Order-Preserving Key Mapping:</strong> Elements are mapped to fixed-width unsigned keys through
/// <see cref="IRadixKeySelector{T}"/>. Signed integers flip the sign bit (e.g. 32-bit: key = (uint)value ^ 0x8000_0000),
/// floating-point values use the IEEE 754 bit transform (-0 and +0 tie), and key-selector overloads extract an int key from arbitrary elements.
/// This ensures negative values are ordered correctly before positive values without separate processing.</description></item>
/// <item><description><strong>Digit Extraction Correctness:</strong> For each digit position d (from digitCount-1 down to 0), extract the d-th 8-bit digit using bitwise operations:
/// digit = (key >> (d × 8)) &amp; 0xFF. This ensures each byte of the integer is processed independently.
/// The byte-wise digit is the form McIlroy, Bostic and McIlroy describe; it halves the level count against a 4-bit digit,
/// at the cost of a 16x larger fixed cost per recursion node (see <c>InsertionSortCutoff</c>).</description></item>
/// <item><description><strong>Digit Count Determination with Range Normalization:</strong> The number of digit levels is determined by the actual
/// range of the keys, not by the key width. One scan finds min and max; digits are then extracted from (key − min), so
/// digitCount = ⌈requiredBits / 8⌉ where requiredBits is the bit width of (max − min). Subtracting a constant is order-preserving
/// and cannot underflow, and — unlike a (max XOR min) width — it stays effective when the range straddles a high bit boundary,
/// as sign-flipped keys of signed values around zero do. When all keys are equal (range == 0), sorting is skipped entirely.
/// Without this, uniform high digits are still discovered one full counting pass at a time.</description></item>
/// <item><description><strong>In-Place Permutation:</strong> Elements are rearranged in-place using a two-pass approach:
/// 1. Count phase: Count occurrences of each digit value
/// 2. Permutation phase: Place each element in its correct bucket position using bucket offsets</description></item>
/// <item><description><strong>MSD Processing Order:</strong> Digits must be processed from most significant (d=digitCount-1) to least significant (d=0).
/// This top-down approach partitions the array into buckets recursively, processing each bucket independently for subsequent digits.</description></item>
/// <item><description><strong>Recursive Bucket Processing:</strong> After permuting elements based on the current digit, each bucket must be recursively sorted for the remaining digits.
/// Base cases: buckets with 0 or 1 elements are already sorted; buckets where all remaining digits are the same are also sorted.
/// The digit position is inherited from the parent rather than re-derived per bucket: rescanning each node's own key range
/// would let it skip several uniform levels at once, but it costs a pass per node while the inherited scheme already
/// discovers one uniform level per counting pass, and it measured slower at every size (see <c>AmericanFlagRadixWidthBenchmark</c>).</description></item>
/// <item><description><strong>Cutoff to Insertion Sort:</strong> For small buckets, switching to insertion sort can improve performance
/// due to lower overhead. The threshold has to scale with the radix rather than sit at a "small array" constant: at 256 buckets a split
/// costs ~1000 fixed operations before a single element moves, so this implementation cuts off at &lt;= 64 elements
/// (the measurements behind that number are on the <c>InsertionSortCutoff</c> constant).</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution (Radix Sort, MSD variant, American Flag Sort)</description></item>
/// <item><description>Stable      : No (in-place permutation does not preserve relative order)</description></item>
/// <item><description>In-place    : Yes (bucket counters are stack-allocated, O(radix) per recursion level; nothing is allocated on the heap)</description></item>
/// <item><description>Best case   : Θ(n) - all keys equal, caught by the range scan and returned before any digit pass
/// (measured: 1.00n reads and zero writes for n = 100,000 equal <c>int</c> values)</description></item>
/// <item><description>Average case: Θ(d × n) - d = ⌈requiredBits/8⌉, from the width of the key range rather than the key width</description></item>
/// <item><description>Worst case  : Θ(d × n) - Same complexity regardless of input order</description></item>
/// <item><description>Comparisons : data-dependent, not zero. The digit passes themselves use bitwise operations only, but every bucket that
/// reaches the cutoff is finished by <see cref="InsertionSort"/>, which compares. Measured with StatisticsContext at n = 100,000 <c>int</c>:
/// 66,544 comparisons for uniform random input, and 0 both for already-sorted 0..n and for keys drawn from 0..999
/// (in those two every leaf either ends at a single element or stays above the cutoff, so the fallback never runs)</description></item>
/// <item><description>Digit Passes: d = ⌈requiredBits/8⌉ from the key range, capped by the key width (1 for byte, 2 for short, 4 for int,
/// 8 for long); levels below that can still terminate early when a bucket's digit turns out to be uniform</description></item>
/// <item><description>Reads       : n (range scan) + one per element per digit level visited + the permutation and cutoff reads.
/// Inputs at or below the cutoff skip the range scan entirely and go straight to <see cref="InsertionSort"/>,
/// so a small array pays no radix overhead at all</description></item>
/// <item><description>Memory      : O(1) auxiliary space, 2052 bytes of stack per recursion level (257 + 256 counters).
/// Recursion depth is bounded by the digit count (at most 4 for 32-bit keys, 8 for 64-bit,
/// and less when the key range is narrow), because each level consumes exactly one digit — it does not depend on n, on the input order,
/// or on how the buckets split</description></item>
/// </list>
/// <para><strong>Algorithm Overview:</strong></para>
/// <para>Inputs at or below the cutoff go straight to <see cref="InsertionSort"/>. Otherwise: one range scan
/// over the keys, then four phases per digit level:</para>
/// <list type="number">
/// <item><description><strong>Count Phase:</strong> Count occurrences of each digit value (0-255)</description></item>
/// <item><description><strong>Offset Calculation:</strong> Compute bucket offsets (cumulative sum)</description></item>
/// <item><description><strong>Permutation Phase:</strong> Rearrange elements into their buckets in-place. This walks the permutation's
/// cycles, but with a plain pairwise swap per step rather than holding the in-flight element in a register as McIlroy et al. describe:
/// the register-held form was measured and lost at every size (see <c>AmericanFlagRadixWidthBenchmark</c>), because
/// the swap primitive is already a single fused ref-to-ref exchange with no write to remove.
/// The final bucket is skipped: once every other bucket is full, only its own elements can remain</description></item>
/// <item><description><strong>Recursive Phase:</strong> Recursively sort each non-empty bucket for the next digit</description></item>
/// </list>
/// <para><strong>What This Buys Over The Other Distribution Sorts:</strong></para>
/// <para>All three sorts below order by the same <see cref="IRadixKeySelector{T}"/> keys and differ only in how elements are moved.
/// The operation counts come from <c>sandbox/DotnetFiles/AmericanFlagPassAudit.cs</c> (StatisticsContext, n = 100,000 uniform
/// random <c>int</c>); they are deterministic and machine-independent. For wall-clock, see the benchmark tables in README.md —
/// the numbers there are produced on the benchmark CI machine, so do not compare them against a local run.</para>
/// <list type="bullet">
/// <item><description><strong>Against <see cref="RadixMSD4Sort"/> (same MSD partitioning, buffered):</strong> strictly less element traffic.
/// Both partition top-down from the most significant digit and both skip uniform digit levels, but the buffered variant scatters every
/// element of a level into a rented temp buffer and copies the whole level back, while the in-place permutation only touches elements that
/// are not already in their bucket, and the 8-bit digit needs half the levels of RadixMSD4Sort's 2-bit one.
/// Measured: 1.01M reads / 462K writes / 67K comparisons here against 2.40M reads / 1.62M writes / 223K comparisons for
/// RadixMSD4Sort. It also rents nothing from ArrayPool.
/// The price is stability — RadixMSD4Sort keeps equal keys in input order, this does not.</description></item>
/// <item><description><strong>Against <see cref="RadixLSD256Sort"/> (LSD, buffered):</strong> the advantage is memory, not throughput.
/// LSD rents an n-element buffer and writes all n elements once per digit pass; this implementation permutes the caller's span in place and
/// allocates nothing. Both now use an 8-bit digit, and LSD remains the faster of the two: its scatter is a straight sequential write into a
/// separate buffer, while the in-place permutation follows a swap chain whose next destination is data-dependent. Measured at n = 100,000
/// uniform random <c>int</c>: 600K reads / 400K writes for RadixLSD256Sort against 1.01M / 462K here.
/// Reach for this algorithm when an O(n) auxiliary buffer is unavailable or unwanted, not when raw speed is the goal.</description></item>
/// <item><description><strong>Low digits are never examined once a bucket is small enough.</strong> An LSD sort must run every pass it has
/// committed to, over the whole array, before the result is ordered. An MSD sort stops descending as soon as a bucket reaches the cutoff, so
/// for keys that separate on their high digits the low digits are never read at all — the deeper the key (64-bit integers,
/// <see cref="double"/>), the more that matters.</description></item>
/// <item><description><strong>Recursion is bounded by the key width, not by the data.</strong> Each level consumes exactly one digit, so depth
/// is at most 4 for 32-bit keys and 8 for 64-bit keys for any input. This is what separates it from an in-place comparison-based partitioner
/// such as QuickSort, where an adversarial input can drive depth toward O(n).</description></item>
/// <item><description><strong>Non-goal — stability.</strong> The cyclic in-place permutation reorders equal keys by construction; it cannot be
/// made stable without the auxiliary buffer whose absence is the entire point. Use <see cref="RadixLSD256Sort"/> or
/// <see cref="RadixMSD4Sort"/> when ties must keep their input order.</description></item>
/// </list>
/// <para><strong>Supported Key Mappings (via <see cref="IRadixKeySelector{T}"/>):</strong></para>
/// <list type="bullet">
/// <item><description><strong>Integers:</strong> byte, sbyte, short, ushort, int, uint, long, ulong, nint, nuint (up to 64-bit); Int128/UInt128/BigInteger are rejected (64-bit key ceiling, see below)</description></item>
/// <item><description><strong>Floating point:</strong> Half, float, double via IEEE 754 bit transform (all NaN values sort first, matching <see cref="IComparable{T}"/> semantics; <c>-0.0</c> and <c>+0.0</c> are a tie and their relative order is unspecified, as in <c>Array.Sort</c>)</description></item>
/// <item><description><strong>Key selector:</strong> arbitrary element types via an extracted <c>int</c> key; NOTE: unlike the stable radix variants, the in-place permutation may reorder elements with equal keys</description></item>
/// </list>
/// <para><strong>Why 128-bit Types Are Not Supported:</strong></para>
/// <list type="bullet">
/// <item><description><strong>Key Storage Limitation:</strong> Keys are stored as <c>ulong</c> (64-bit).
/// Supporting 128-bit would require <c>UInt128</c> keys, significantly increasing memory usage and complexity.</description></item>
/// <item><description><strong>Performance Trade-offs:</strong> 128-bit operations are significantly slower than 64-bit on most architectures,
/// negating the performance benefits of radix sort.</description></item>
/// <item><description><strong>Practical Rarity:</strong> Sorting 128-bit integers is uncommon in typical applications.
/// For such cases, comparison-based sorts (e.g., QuickSort, MergeSort) remain practical alternatives.</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/American_flag_sort</para>
/// <para>Paper: "Engineering Radix Sort" by McIlroy, Bostic, and McIlroy (1993)</para>
/// </remarks>
public static class AmericanFlagSort
{
    private const int RadixBits = 8;        // 8 bits per digit
    private const int RadixSize = 256;      // 2^8 = 256 buckets
    private const int RadixMask = RadixSize - 1;

    // Switch to insertion sort for small buckets. This has to scale with the radix, not sit at a
    // "small array" constant: a 256-way split costs ~1000 fixed operations (clear 257 counters, prefix
    // sum, bucket-boundary walk) before a single element moves, so a node has to be big enough to earn
    // it back. Leaving it at 16 while widening the digit from 4 to 8 bits made full-int-range keys
    // 1.5x SLOWER at n=4096 and n=8192, where one 256-way split leaves buckets sitting just above 16
    // and every one of them pays that fixed cost again to sort ~20 elements. Measured ratios against
    // the 4-bit digit at n=4096/8192/65536/1048576 with full-range keys:
    //   cutoff 16 -> 1.55 / 1.52 / 0.62 / 1.21   (worse than 4-bit at three of four sizes)
    //   cutoff 32 -> 0.51 / 1.01 / 0.66 / 0.65
    //   cutoff 64 -> 0.53 / 0.68 / 0.66 / 0.66
    // 64 is RadixSize/4 and is the smallest value that wins at every measured size; 128 measured the
    // same within noise but doubles the quadratic term insertion sort pays on its largest input.
    private const int InsertionSortCutoff = 64;

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array

    /// <summary>
    /// Sorts the elements in the specified span in ascending order.
    /// Uses NullContext for zero-overhead fast path.
    /// </summary>
    /// <typeparam name="T"> The type of elements to sort. Must be a binary integer type (up to 64-bit).</typeparam>
    /// <param name="span"> The span of elements to sort.</param>
    public static void Sort<T>(Span<T> span) where T : IBinaryInteger<T>
        => Sort(span, NullContext.Default);

    /// <summary>
    /// Sorts the elements in the specified span using the specified context.
    /// </summary>
    /// <typeparam name="T"> The type of elements to sort. Must be a binary integer type (up to 64-bit).</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="span"> The span of elements to sort.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation.</param>
    /// <exception cref="NotSupportedException">
    /// Thrown when <typeparamref name="T"/> is a 128-bit type (<see cref="Int128"/> or <see cref="UInt128"/>).
    /// This implementation only supports integer types up to 64-bit due to key storage and performance constraints.
    /// See class-level remarks for detailed explanation of this limitation.
    /// </exception>
    public static void Sort<T, TContext>(Span<T> span, TContext context)
        where T : IBinaryInteger<T>
        where TContext : ISortContext
        => SortCore(span, default(BinaryIntegerRadixKey<T>), new ComparableComparer<T>(), context);

    /// <summary>
    /// Sorts the elements in the specified span by an integer key extracted with <paramref name="keySelector"/>.
    /// NOTE: the in-place cyclic permutation is unstable — elements with equal keys may be reordered.
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
    /// NOTE: the in-place cyclic permutation is unstable — elements with equal keys may be reordered.
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
    /// NOTE: this algorithm is unstable - elements with equal keys may be reordered.
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

        // Validate the key width up front (BinaryIntegerRadixKey throws NotSupportedException for >64-bit types).
        RadixKeyGuard.ValidateKeyBits<T, TRadixKey>();

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);

        // Below the cutoff the recursion would insertion-sort the whole range without extracting a single
        // digit, so the range scan below would be pure overhead — n extra reads and n key extractions on an
        // input that never reaches a digit pass. Take the fallback directly.
        if (s.Length <= InsertionSortCutoff)
        {
            InsertionSort.SortCore(s, 0, s.Length);
            return;
        }

        // Announce the range scan: without it a consumer sees n reads with no phase attached, and the label
        // from whatever ran before stays on screen. KeyRangeScan rather than DistributionCount: this measures
        // the keys, it does not tally per-value occurrences.
        s.Context.OnPhase(SortPhase.KeyRangeScan);

        // One scan of the keys decides how many digit levels can hold a difference.
        // Without it the digit count comes from the key width alone (4 levels for a 32-bit key), and every
        // level whose digit happens to be uniform still costs a full counting pass before the uniform-digit
        // check in the recursion can fire: keys drawn from 0..999 need only 2 levels, so the other 2 were
        // spent proving a digit was uniform before any element moved.
        var minKey = ulong.MaxValue;
        var maxKey = ulong.MinValue;
        for (var i = 0; i < s.Length; i++)
        {
            var key = radixKey.GetKey(s.Read(i));
            if (key < minKey) minKey = key;
            if (key > maxKey) maxKey = key;
        }

        // Digits are extracted from (key - minKey), so the level count follows the width of the range rather
        // than where the keys sit. Subtracting a constant is order-preserving and cannot underflow (every key
        // is >= minKey), and unlike a (max XOR min) width it stays effective when the range straddles a high
        // bit boundary — which is exactly what sign-flipped keys of signed values around zero do.
        var range = maxKey - minKey;

        // Every key is equal, so any permutation is sorted. This is the only path that is linear in n:
        // the digit passes below are always Θ(digitCount × n).
        if (range == 0) return;

        var requiredBits = 64 - BitOperations.LeadingZeroCount(range);
        var digitCount = (requiredBits + RadixBits - 1) / RadixBits;

        // Start American Flag Sort from the most significant digit that can differ
        AmericanFlagSortRecursive(s, radixKey, minKey, 0, s.Length, digitCount - 1, digitCount);
    }

    /// <param name="minKey">
    /// Smallest key in the whole input. Every digit is taken from (key - minKey) so that the digit count
    /// derives from the width of the key range; see the normalization note in <see cref="SortCore"/>.
    /// </param>
    /// <param name="digitCount">
    /// Total number of digit positions the normalized keys need. Carried down the recursion to report
    /// <see cref="SortPhase.RadixPass"/>, whose contract is param1 = current digit, param2 = total digits;
    /// a consumer cannot derive the total from <paramref name="digit"/> alone.
    /// </param>
    // No AggressiveInlining here: the JIT will not inline a recursive call, so the attribute could only ever
    // affect the single entry call from SortCore, and it would pull a 2 KB stackalloc into that frame.
    // The sibling MSD sorts do not carry it either.
    private static void AmericanFlagSortRecursive<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, TRadixKey radixKey, ulong minKey, int start, int length, int digit, int digitCount)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Base case: if length is small, use insertion sort
        if (length <= InsertionSortCutoff)
        {
            InsertionSort.SortCore(s, start, start + length);
            return;
        }

        // Base case: if we've processed all digits, we're done
        if (digit < 0)
        {
            return;
        }

        var shift = digit * RadixBits;

        // Bucket arrays live on the stack: 2052 bytes per level (257 + 256 ints). Recursion depth is capped
        // by the digit count (at most 4 levels for a 32-bit key, 8 for a 64-bit one), so the whole sort holds
        // at most ~16 KB of them.
        Span<int> bucketCounts = stackalloc int[RadixSize + 1];  // Stores both start and end for each bucket
        Span<int> bucketNext = stackalloc int[RadixSize];        // Current write position for each bucket

        // Phase 1: Count occurrences of each digit value
        s.Context.OnPhase(SortPhase.RadixPass, digit, digitCount);
        // Store count for digit d in bucketCounts[d+1] (off-by-one trick for prefix sum)
        bucketCounts.Clear();

        for (var i = 0; i < length; i++)
        {
            var value = s.Read(start + i);
            var key = radixKey.GetKey(value) - minKey;
            var digitValue = (int)((key >> shift) & RadixMask);  // Extract 8-bit digit
            bucketCounts[digitValue + 1]++;
        }

        // Early termination optimization: Check for uniform digit values
        // Count non-empty buckets BEFORE prefix sum transformation
        // At this point, bucketCounts[i+1] holds the raw count for bucket i (off-by-one indexing)
        var nonEmptyBuckets = 0;
        for (var i = 0; i < RadixSize; i++)
        {
            if (bucketCounts[i + 1] > 0 && ++nonEmptyBuckets > 1)
                break;
        }

        // If all elements have the same digit value (0 or 1 non-empty buckets),
        // skip permutation and recursively process the next digit
        if (nonEmptyBuckets <= 1)
        {
            if (digit > 0)
                AmericanFlagSortRecursive(s, radixKey, minKey, start, length, digit - 1, digitCount);

            // If digit == 0, there are no lower digits left to process, so we're done.
            return;
        }

        // Phase 2: Calculate bucket offsets (prefix sum)
        // After prefix sum: bucketCounts[d] = start of bucket d, bucketCounts[d+1] = end of bucket d
        // This gives us both boundaries for each bucket from a single array
        for (var i = 1; i <= RadixSize; i++)
        {
            bucketCounts[i] += bucketCounts[i - 1];
        }

        // Phase 2.5: Initialize next write positions
        // bucketNext[i] tracks the current write position for bucket i
        // Copy bucket start positions from bucketCounts[i] (after prefix sum, bucketCounts[i] = start of bucket i)
        for (var i = 0; i < RadixSize; i++)
        {
            bucketNext[i] = bucketCounts[i];
        }

        // Phase 3: In-place permutation
        // Rearrange elements into their correct buckets using cyclic permutation
        PermuteInPlace(s, radixKey, minKey, start, shift, bucketCounts, bucketNext);

        // Phase 4: Recursively sort each bucket for the next digit
        for (var i = 0; i < RadixSize; i++)
        {
            // bucketCounts provides direct access to boundaries
            var bucketStart = bucketCounts[i];
            var bucketEnd = bucketCounts[i + 1];  // No conditional needed!
            var bucketLength = bucketEnd - bucketStart;

            if (bucketLength > 1)
            {
                AmericanFlagSortRecursive(s, radixKey, minKey, start + bucketStart, bucketLength, digit - 1, digitCount);
            }
        }
    }

    /// <summary>
    /// Permutes elements in-place into their correct buckets.
    /// Uses a technique similar to cyclic permutation to avoid using auxiliary buffer.
    /// </summary>
    /// <remarks>
    /// Array roles:
    /// - <paramref name="bucketCounts"/>: Immutable boundary array where bucketCounts[i] = start of bucket i, bucketCounts[i+1] = end of bucket i
    /// - <paramref name="bucketNext"/>: Mutable current write position for each bucket (incremented as elements are placed)
    /// This separation ensures correct boundary detection and avoids array role confusion.
    /// <paramref name="minKey"/> must be the same value the counting pass used, or an element would be routed
    /// to a bucket the counts never reserved space for.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void PermuteInPlace<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, TRadixKey radixKey, ulong minKey, int start, int shift, Span<int> bucketCounts, Span<int> bucketNext)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // In-place permutation using bucket positions.
        // The last bucket is deliberately not visited: once buckets 0..RadixSize-2 have each been filled to
        // their own count, every element with one of those digits sits in its own bucket, so whatever is left
        // in the final bucket's range can only be elements of that digit. Walking it would re-read and
        // re-classify count[RadixSize-1] elements to conclude they are already home.
        for (var bucket = 0; bucket < RadixSize - 1; bucket++)
        {
            // Get the range for this bucket directly from bucketCounts
            // bucketCounts[bucket] = start, bucketCounts[bucket + 1] = end
            var bucketEnd = bucketCounts[bucket + 1];

            // Move elements to their correct positions within and across buckets
            while (bucketNext[bucket] < bucketEnd)
            {
                var currentPos = start + bucketNext[bucket];
                var currentValue = s.Read(currentPos);
                var currentKey = radixKey.GetKey(currentValue) - minKey;
                var currentDigit = (int)((currentKey >> shift) & RadixMask);

                // If element is already in correct bucket, advance
                if (currentDigit == bucket)
                {
                    bucketNext[bucket]++;
                    continue;
                }

                // Swap current element to its correct bucket
                var targetPos = start + bucketNext[currentDigit];

#if DEBUG
                // targetPos must stay within currentDigit bucket range
                // bucketCounts[currentDigit] = start, bucketCounts[currentDigit + 1] = end
                Debug.Assert(bucketNext[currentDigit] >= bucketCounts[currentDigit]);
                Debug.Assert(bucketNext[currentDigit] < bucketCounts[currentDigit + 1]);
#endif

                s.Swap(currentPos, targetPos);
                bucketNext[currentDigit]++;
            }
        }
    }

}
