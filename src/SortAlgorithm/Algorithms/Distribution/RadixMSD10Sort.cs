using SortAlgorithm.Contexts;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 10進数基数のMSD（Most Significant Digit）基数ソート。
/// 値を10進数の桁として扱い、最上位桁から最下位桁まで再帰的にバケットソートを行います。
/// 人間が理解しやすい10進数ベースのアルゴリズムで、デバッグや教育目的に適しています。
/// <br/>
/// Decimal-based MSD (Most Significant Digit) radix sort.
/// Treats values as decimal digits and performs bucket sorting recursively from the most significant digit to the least significant digit.
/// This decimal-based algorithm is easy for humans to understand and is suitable for debugging and educational purposes.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct MSD Radix Sort (Decimal Base):</strong></para>
/// <list type="number">
/// <item><description><strong>Order-Preserving Key Mapping:</strong> Elements are mapped to fixed-width unsigned keys through
/// <see cref="IRadixKeySelector{T}"/>. Signed integers flip the sign bit (e.g. 32-bit: key = (uint)value ^ 0x8000_0000),
/// floating-point values use the IEEE 754 bit transform (-0 and +0 tie), and key-selector overloads extract an int key from arbitrary elements.
/// This ensures ordering correctness without separate sign handling and avoids the MinValue overflow issue with Abs().</description></item>
/// <item><description><strong>Dynamic Starting Digit (MSD Optimization):</strong> Before sorting, performs a single O(n) pass to find the minimum and maximum
/// key values and derives the digit count from (max − min), the keys then being normalized by the minimum. This eliminates empty high-order
/// digit passes, which is critical for MSD performance when the values span far less than the type's capacity. Deriving it from the maximum alone
/// is not enough: the digit count then follows where the keys sit rather than how far apart they are, and an order-preserving key mapping can place
/// them anywhere (sign-flipped non-negative ints all start above 2,147,483,648 — ten decimal digits whatever the values were).</description></item>
/// <item><description><strong>Digit Extraction Consistency:</strong> For a given position from most significant digit, extract the digit using (key / divisor) % 10
/// where divisor = 10^(digitCount - 1 - d) for digit position d.</description></item>
/// <item><description><strong>MSD Processing Order:</strong> Digits must be processed from most significant (d=digitCount-1) to least significant (d=0).
/// This top-down approach partitions the array into buckets recursively, processing each bucket independently for subsequent digits.</description></item>
/// <item><description><strong>Recursive Bucket Processing:</strong> After distributing elements based on the current digit, each bucket must be recursively sorted for the remaining digits.
/// Base cases: buckets with 0 or 1 elements are already sorted; buckets where all remaining digits are the same are also sorted.</description></item>
/// <item><description><strong>Identity Levels Are Skippable:</strong> If every element in a range shares the same value at the
/// current digit, the stable distribution for that digit writes them out in the order they already are — the distribution and the
/// copy back are both the identity and can be skipped, and the range descends to the next digit untouched. This is orthogonal to the
/// data-derived digit count above, which only trims uniform digits above the most significant one; a digit anywhere below it can be
/// uniform too, in any bucket of the recursion (a shared prefix among the elements that reached it).</description></item>
/// <item><description><strong>Cutoff to Insertion Sort:</strong> Below a bucket size the recursion stops splitting and insertion-sorts the range instead,
/// because emptying a small bucket by distribution costs more levels than sorting it outright. The definition leaves the threshold free; the value
/// this implementation uses was measured, and is recorded on the <c>InsertionSortCutoff</c> constant.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution (Radix Sort, MSD variant)</description></item>
/// <item><description>Stable      : Yes (maintains relative order of elements with equal keys)</description></item>
/// <item><description>In-place    : No (O(n) auxiliary space for temporary buffer)</description></item>
/// <item><description>Best case   : Θ(n) - When all keys are equal (early termination on range == 0, after the single key scan)</description></item>
/// <item><description>Average case: Θ(n + d × n) - One O(n) key range scan + d levels, d = number of decimal digits of (max − min), not of the type maximum</description></item>
/// <item><description>Worst case  : Θ(n + d × n) - Same complexity regardless of input order</description></item>
/// <item><description>Comparisons : data-dependent, not zero. The digit passes themselves use arithmetic operations only, but every range that
/// reaches the cutoff is finished by <see cref="InsertionSort"/>, which compares. Measured with StatisticsContext at n = 100,000 <c>int</c>:
/// 667,030 comparisons for uniform random input, 90,000 for already-sorted 0..n, and 0 for keys drawn from 0..999 and for all-equal input
/// (there every leaf either ends at a single element or is settled by the range scan, so the fallback never runs)</description></item>
/// <item><description>Digit Passes: 1 initial key range scan + d = ⌈log₁₀(max − min)⌉ levels examined; a level whose digit is uniform is counted but not distributed</description></item>
/// <item><description>Memory      : O(n) for temporary buffer</description></item>
/// </list>
/// <para><strong>MSD vs LSD (Decimal):</strong></para>
/// <list type="bullet">
/// <item><description>MSD processes high-order digits first, enabling early termination when buckets are fully sorted</description></item>
/// <item><description>MSD dynamically computes starting digit from data, avoiding unnecessary passes for small values in large types</description></item>
/// <item><description>MSD is cache-friendlier for partially sorted data as it localizes accesses within buckets</description></item>
/// <item><description>MSD requires recursive processing of buckets, adding overhead compared to LSD's iterative approach</description></item>
/// <item><description>Both MSD and LSD can be implemented as stable sorts (this implementation maintains stability)</description></item>
/// </list>
/// <para><strong>Note:</strong> Uses decimal arithmetic (division and modulo), which may be slower than binary-based radix sorts (e.g., RadixMSD4Sort with bit shifts).
/// However, it is more intuitive for understanding and debugging.</para>
/// <para><strong>Supported Key Mappings (via <see cref="IRadixKeySelector{T}"/>):</strong></para>
/// <list type="bullet">
/// <item><description><strong>Integers:</strong> byte, sbyte, short, ushort, int, uint, long, ulong, nint, nuint (up to 64-bit); Int128/UInt128/BigInteger are rejected (64-bit key ceiling, see below)</description></item>
/// <item><description><strong>Floating point:</strong> Half, float, double via IEEE 754 bit transform (all NaN values sort first, matching <see cref="IComparable{T}"/> semantics; <c>-0.0</c> and <c>+0.0</c> are a tie and keep their input order, this sort being stable)</description></item>
/// <item><description><strong>Key selector:</strong> arbitrary element types via an extracted <c>int</c> key; equal keys retain input order, making stability observable</description></item>
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
/// <para><strong>Why the floating-point overloads compare by key:</strong></para>
/// <para>Every range that reaches the insertion-sort cutoff is ordered by <c>TComparer</c>, not by the digit
/// it never extracts, so the two orders have to agree or the answer depends on where the cutoff happens to
/// fall. For integers and for the key-selector overloads they agree by construction. For floating point they
/// did not: the element overloads passed <see cref="ComparableComparer{T}"/>, which
/// <see cref="SortSpan{T, TComparer, TContext}"/> specializes to the raw IEEE 754 operators on the
/// <see cref="NullContext"/> path, and under those NaN is unordered — an insertion sort cannot move it, while
/// the digit passes place it first from its key of 0. A range holding NaN alongside ordered values therefore
/// came out wrong whenever it fell to the cutoff, and only on that path: the same input sorted with an
/// observation context attached, or in a Debug build, went through <c>CompareTo</c> and came out right.
/// These overloads now pass <see cref="RadixKeyComparer{T, TRadixKey}"/>, which orders by exactly the key the
/// digit passes use, so the fallback and the distribution cannot disagree for any selector.</para>/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Radix_sort#Most_significant_digit</para>
/// </remarks>
public static class RadixMSD10Sort
{
    private const int RadixBase = 10;       // Decimal base
    // Switch to insertion sort for small buckets. The textbook constant is 15-16 (Sedgewick's MSD string
    // sort, American flag sort), but those are radix-256 string sorts and nothing in the MSD definition
    // fixes the number, so it was measured here rather than inherited. Same sweep and protocol as
    // <see cref="RadixMSD4Sort"/> — ratios for 48 against 16, minimum of 41-71 repetitions with
    // DOTNET_TieredCompilation=0, best of three interleaved A/B cycles per case:
    //   n=4096     full 1.00   narrow 0.94   dup32 1.00
    //   n=65536    full 0.96   narrow 0.92   dup32 0.96
    //   n=1048576  not resolvable on this machine
    // Up to n=65536 the win is small but consistent, and 16 measured worst or joint-worst everywhere.
    // At n=1048576 it cannot be called: this sort's wall clock swings about ±40% run to run on an
    // identical binary (one configuration spanned 13-18 ms, another 33-46 ms), and two independent
    // sample sets disagreed on the sign. 48 is taken there to match RadixMSD4Sort rather than because
    // the measurement chose it. Do not read any 1M-scale difference under ~40% here as real.
    private const int InsertionSortCutoff = 48;

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Temporary buffer for digit redistribution

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
    /// This implementation only supports integer types up to 64-bit due to key storage and performance constraints.
    /// See class-level remarks for detailed explanation of this limitation.
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
        => SortCore(span, default(HalfRadixKey), new RadixKeyComparer<Half, HalfRadixKey>(default), NullContext.Default);

    /// <inheritdoc cref="Sort(Span{Half})"/>
    public static void Sort<TContext>(Span<Half> span, TContext context) where TContext : ISortContext
        => SortCore(span, default(HalfRadixKey), new RadixKeyComparer<Half, HalfRadixKey>(default), context);

    /// <summary>
    /// Sorts <see cref="float"/> values via the IEEE 754 bit transform.
    /// All NaN values sort first, matching <see cref="IComparable{T}"/> semantics.
    /// </summary>
    public static void Sort(Span<float> span)
        => SortCore(span, default(SingleRadixKey), new RadixKeyComparer<float, SingleRadixKey>(default), NullContext.Default);

    /// <inheritdoc cref="Sort(Span{float})"/>
    public static void Sort<TContext>(Span<float> span, TContext context) where TContext : ISortContext
        => SortCore(span, default(SingleRadixKey), new RadixKeyComparer<float, SingleRadixKey>(default), context);

    /// <summary>
    /// Sorts <see cref="double"/> values via the IEEE 754 bit transform.
    /// All NaN values sort first, matching <see cref="IComparable{T}"/> semantics.
    /// </summary>
    public static void Sort(Span<double> span)
        => SortCore(span, default(DoubleRadixKey), new RadixKeyComparer<double, DoubleRadixKey>(default), NullContext.Default);

    /// <inheritdoc cref="Sort(Span{double})"/>
    public static void Sort<TContext>(Span<double> span, TContext context) where TContext : ISortContext
        => SortCore(span, default(DoubleRadixKey), new RadixKeyComparer<double, DoubleRadixKey>(default), context);

    private static void SortCore<T, TRadixKey, TComparer, TContext>(Span<T> span, TRadixKey radixKey, TComparer comparer, TContext context)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;
        RadixKeyGuard.ValidateKeyBits<T, TRadixKey>();

        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);

        // Below the cutoff the recursion would insertion-sort the whole range without extracting a single
        // digit, so the range scan below would be pure overhead — n extra reads and n key extractions on an
        // input that never reaches a digit pass. Take the fallback directly, before renting a buffer no
        // digit pass would use.
        if (s.Length <= InsertionSortCutoff)
        {
            InsertionSort.SortCore(s, 0, s.Length);
            return;
        }

        // Announce the range scan: without it a consumer sees n reads with no phase attached, and the
        // label from whatever ran before stays on screen through the whole scan. KeyRangeScan rather than
        // DistributionCount: this measures the keys, it does not tally per-value occurrences.
        s.Context.OnPhase(SortPhase.KeyRangeScan);

        // One scan of the keys decides how many digit levels can hold a difference. Scanning for the maximum
        // alone is not enough: the digit count then follows where the keys sit rather than how far apart they
        // are, and the sign-flipped key of a small non-negative int sits just above 2,147,483,648 — ten
        // decimal digits, of which the top six are the same for every element in the input.
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
        // is >= minKey).
        var range = maxKey - minKey;

        // Every key is equal, so the input is already sorted whatever order it is in. This is the only path
        // that is linear in n: the digit passes below are always Θ(digitCount × n).
        if (range == 0) return;

        // Pre-computed powers of 10 for O(1) divisor lookup
        // Pow10[d] = 10^d for d in [0..19], supporting up to 20 decimal digits (ulong max)
        // This eliminates O(digit) loop in divisor calculation for each recursive call
        ReadOnlySpan<ulong> pow10 = [
            1UL,                      // 10^0
            10UL,                     // 10^1
            100UL,                    // 10^2
            1_000UL,                  // 10^3
            10_000UL,                 // 10^4
            100_000UL,                // 10^5
            1_000_000UL,              // 10^6
            10_000_000UL,             // 10^7
            100_000_000UL,            // 10^8
            1_000_000_000UL,          // 10^9
            10_000_000_000UL,         // 10^10
            100_000_000_000UL,        // 10^11
            1_000_000_000_000UL,      // 10^12
            10_000_000_000_000UL,     // 10^13
            100_000_000_000_000UL,    // 10^14
            1_000_000_000_000_000UL,  // 10^15
            10_000_000_000_000_000UL, // 10^16
            100_000_000_000_000_000UL,// 10^17
            1_000_000_000_000_000_000UL,  // 10^18
            10_000_000_000_000_000_000UL  // 10^19 (max for 20-digit ulong: 18,446,744,073,709,551,615)
        ];

        var digitCount = GetDigitCountFromUlong(range, pow10);

        // Rent buffer from ArrayPool (only temp buffer needed now)
        var tempArray = ArrayPool<T>.Shared.Rent(span.Length);

        try
        {
            var temp = new SortSpan<T, TComparer, TContext>(tempArray.AsSpan(0, span.Length), context, comparer, BUFFER_TEMP);

            // Start MSD radix sort from the most significant digit that can differ
            MSDSort(s, temp, radixKey, minKey, 0, s.Length, digitCount - 1, digitCount, pow10);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(tempArray, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    /// <param name="minKey">
    /// Smallest key in the whole input. Every digit is taken from (key - minKey) so that the digit count
    /// derives from the width of the key range; see the normalization note in <see cref="SortCore"/>.
    /// </param>
    /// <param name="digitCount">
    /// Total number of digit positions the normalized keys need. Carried down the recursion only to report
    /// <see cref="SortPhase.RadixPass"/>, whose contract is param1 = current digit, param2 = total digits;
    /// a consumer cannot derive the total from <paramref name="digit"/> alone.
    /// </param>
    private static void MSDSort<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> temp, TRadixKey radixKey, ulong minKey, int start, int length, int digit, int digitCount, ReadOnlySpan<ulong> pow10)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Base case: every digit has been consumed, so every normalized key in this range is equal and any
        // order it is in is sorted. Checked before the cutoff: below it this range would be handed to
        // insertion sort, which can only confirm at a cost of length-1 comparisons what reaching digit < 0
        // already proves, and would report a sort phase for work that changes nothing.
        if (digit < 0)
        {
            return;
        }

        // Base case: if length is small, use insertion sort
        if (length <= InsertionSortCutoff)
        {
            InsertionSort.SortCore(s, start, start + length);
            return;
        }

        s.Context.OnPhase(SortPhase.RadixPass, digit, digitCount);
        var divisor = pow10[digit];

        Span<int> counts = stackalloc int[RadixBase];
        counts.Clear(); // Required: [module: SkipLocalsInit] skips zero-initialization
        Span<int> offsets = stackalloc int[RadixBase];

        // Phase 1: Count occurrences of each digit value
        for (var i = 0; i < length; i++)
        {
            var key = radixKey.GetKey(s.Read(start + i)) - minKey;
            var digitValue = (int)((key / divisor) % 10);
            counts[digitValue]++;
        }

        // If one bucket holds every element, this digit partitions nothing: a stable distribution over a
        // single bucket writes the elements out in the order they already are, so the distribution and the
        // copy back are both the identity and can be skipped. The range moves on to the next digit
        // untouched, which is what lets a shared prefix cost reads only. This is orthogonal to the
        // range-derived digit count, which only trims uniform digits above the most significant one;
        // a digit anywhere below it can be uniform too, in any bucket of the recursion.
        if (IsSingleBucket(counts, length))
        {
            if (digit > 0)
            {
                MSDSort(s, temp, radixKey, minKey, start, length, digit - 1, digitCount, pow10);
            }

            // digit == 0: no lower digits left, and every key in the range is equal.
            return;
        }

        // Phase 2: Calculate bucket offsets (prefix sum)
        offsets[0] = 0;
        for (var i = 1; i < RadixBase; i++)
        {
            offsets[i] = offsets[i - 1] + counts[i - 1];
        }

        // The offsets are final here, so where every bucket will lie is already decided even though no element
        // has moved yet. Report it before the distribution: a consumer that works the boundaries out for itself
        // has to reimplement the key mapping, the normalization and the digit width, and a wrong reconstruction
        // still looks like a plausible partition. The whole report sits behind the NullContext test so the
        // optimized path keeps none of it.
        if (typeof(TContext) != typeof(NullContext))
        {
            ReportBuckets(s.Context, offsets, counts, start);
        }

        // Phase 3: Distribute elements into temp buffer (forward scan keeps stability)
        Span<int> writePos = stackalloc int[RadixBase];
        offsets.CopyTo(writePos);

        for (var i = 0; i < length; i++)
        {
            var value = s.Read(start + i);
            var key = radixKey.GetKey(value) - minKey;
            var digitValue = (int)((key / divisor) % 10);
            var destIndex = writePos[digitValue]++;
            temp.Write(start + destIndex, value);
        }

        // Copy back from temp to source
        temp.CopyTo(start, s, start, length);

        // Phase 4: Recursively sort each bucket for the next digit
        for (var i = 0; i < RadixBase; i++)
        {
            if (counts[i] > 1)
            {
                MSDSort(s, temp, radixKey, minKey, start + offsets[i], counts[i], digit - 1, digitCount, pow10);
            }
        }
    }

    /// <summary>
    /// True when a single bucket holds all <paramref name="length"/> elements, i.e. every element in the
    /// range shares this digit. Counts are indexed by digit value directly, before the prefix sum that
    /// turns them into start offsets.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSingleBucket(ReadOnlySpan<int> counts, int length)
    {
        // The first bucket holding anything settles it: if it does not hold everything, some other
        // bucket holds the rest. So the scan stops at the first non-empty entry either way.
        foreach (var count in counts)
        {
            if (count != 0) return count == length;
        }
        return false;
    }

    /// <summary>
    /// Get the number of decimal digits needed to represent a ulong value
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetDigitCountFromUlong(ulong value, ReadOnlySpan<ulong> pow10)
    {
        if (value == 0) return 1;

        // value < 10^1 -> 1 digit, value < 10^2 -> 2 digits, ..., value < 10^d -> d digits
        // Pow10 is 10^0...10^19
        for (int d = 1; d < pow10.Length; d++)
            if (value < pow10[d]) return d;

        return 20; // max for ulong (10^20 > 2^64)
    }
    /// <summary>
    /// Reports the span each non-empty bucket occupies, one <see cref="SortPhase.DistributionBucket"/> per bucket.
    /// Empty buckets are skipped, so this costs min(radix, length) reports rather than a fixed RadixBase.
    /// </summary>
    /// <param name="offsets">Start of each bucket relative to <paramref name="start"/>.</param>
    /// <param name="counts">Element count of each bucket, indexed the same way.</param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ReportBuckets<TContext>(TContext context, ReadOnlySpan<int> offsets, ReadOnlySpan<int> counts, int start)
        where TContext : ISortContext
    {
        for (var d = 0; d < RadixBase; d++)
        {
            if (counts[d] > 0)
            {
                context.OnPhase(SortPhase.DistributionBucket, start + offsets[d], counts[d], d);
            }
        }
    }

}
