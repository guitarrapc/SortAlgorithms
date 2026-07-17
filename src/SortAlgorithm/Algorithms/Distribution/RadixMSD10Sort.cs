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
/// floating-point values use the IEEE 754 total-order bit transform, and key-selector overloads extract an int key from arbitrary elements.
/// This ensures ordering correctness without separate sign handling and avoids the MinValue overflow issue with Abs().</description></item>
/// <item><description><strong>Dynamic Starting Digit (MSD Optimization):</strong> Before sorting, performs a single O(n) pass to find the maximum key value
/// and computes the actual required digit count. This eliminates empty high-order digit passes, which is critical for MSD performance
/// when values are small relative to the type's capacity (e.g., values ≤ 999 in a 64-bit type need only 3 digits, not 20).</description></item>
/// <item><description><strong>Digit Extraction Consistency:</strong> For a given position from most significant digit, extract the digit using (key / divisor) % 10
/// where divisor = 10^(digitCount - 1 - d) for digit position d.</description></item>
/// <item><description><strong>MSD Processing Order:</strong> Digits must be processed from most significant (d=digitCount-1) to least significant (d=0).
/// This top-down approach partitions the array into buckets recursively, processing each bucket independently for subsequent digits.</description></item>
/// <item><description><strong>Recursive Bucket Processing:</strong> After distributing elements based on the current digit, each bucket must be recursively sorted for the remaining digits.
/// Base cases: buckets with 0 or 1 elements are already sorted; buckets where all remaining digits are the same are also sorted.</description></item>
/// <item><description><strong>Cutoff to Insertion Sort:</strong> For small buckets (typically &lt; 16 elements), switching to insertion sort can improve performance due to lower overhead.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution (Radix Sort, MSD variant)</description></item>
/// <item><description>Stable      : Yes (maintains relative order of elements with equal keys)</description></item>
/// <item><description>In-place    : No (O(n) auxiliary space for temporary buffer)</description></item>
/// <item><description>Best case   : Θ(n) - When all elements fall into one bucket early, or when the initial digit scan shows all values are equal</description></item>
/// <item><description>Average case: Θ(n + d × n) - One O(n) pass for max digit computation + d passes where d = ⌈log₁₀(actualMax)⌉ (actual data digits, not type maximum)</description></item>
/// <item><description>Worst case  : Θ(n + d × n) - Same complexity regardless of input order</description></item>
/// <item><description>Comparisons : 0 (Non-comparison sort, uses arithmetic operations only)</description></item>
/// <item><description>Digit Passes: 1 initial pass for max computation + up to d = ⌈log₁₀(actualMax)⌉, but can terminate early per bucket</description></item>
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
/// <item><description><strong>Floating point:</strong> Half, float, double via IEEE 754 total-order key transform (all NaN values sort first, matching <see cref="IComparable{T}"/> semantics)</description></item>
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
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Radix_sort#Most_significant_digit</para>
/// </remarks>
public static class RadixMSD10Sort
{
    private const int RadixBase = 10;       // Decimal base
    private const int InsertionSortCutoff = 16; // Switch to insertion sort for small buckets

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
    /// Sorts <see cref="Half"/> values via the IEEE 754 total-order key transform.
    /// All NaN values sort first, matching <see cref="IComparable{T}"/> semantics.
    /// </summary>
    public static void Sort(Span<Half> span)
        => SortCore(span, default(HalfRadixKey), new ComparableComparer<Half>(), NullContext.Default);

    /// <inheritdoc cref="Sort(Span{Half})"/>
    public static void Sort<TContext>(Span<Half> span, TContext context) where TContext : ISortContext
        => SortCore(span, default(HalfRadixKey), new ComparableComparer<Half>(), context);

    /// <summary>
    /// Sorts <see cref="float"/> values via the IEEE 754 total-order key transform.
    /// All NaN values sort first, matching <see cref="IComparable{T}"/> semantics.
    /// </summary>
    public static void Sort(Span<float> span)
        => SortCore(span, default(SingleRadixKey), new ComparableComparer<float>(), NullContext.Default);

    /// <inheritdoc cref="Sort(Span{float})"/>
    public static void Sort<TContext>(Span<float> span, TContext context) where TContext : ISortContext
        => SortCore(span, default(SingleRadixKey), new ComparableComparer<float>(), context);

    /// <summary>
    /// Sorts <see cref="double"/> values via the IEEE 754 total-order key transform.
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

        // Rent buffer from ArrayPool (only temp buffer needed now)
        var tempArray = ArrayPool<T>.Shared.Rent(span.Length);

        try
        {
            var tempBuffer = tempArray.AsSpan(0, span.Length);

            // The insertion-sort cutoff compares by the radix key, matching the
            // digit passes and preserving stability for equal keys.
            var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);
            var temp = new SortSpan<T, TComparer, TContext>(tempBuffer, context, comparer, BUFFER_TEMP);

            // Compute actual maximum digit count from the data (MSD optimization)
            // This is the key optimization: instead of using the key's maximum possible digits,
            // we scan the data once to find the actual maximum key and its digit count.
            // This eliminates empty high-order digit passes, which is crucial for MSD performance.
            var digitCount = ComputeMaxDigit(s, radixKey, 0, s.Length);

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

            // Start MSD radix sort from the most significant digit
            MSDSort(s, temp, radixKey, 0, s.Length, digitCount - 1, pow10);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(tempArray, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
    }

    private static void MSDSort<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, SortSpan<T, TComparer, TContext> temp, TRadixKey radixKey, int start, int length, int digit, ReadOnlySpan<ulong> pow10)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Base case: if length is small, use insertion sort (key-based comparer keeps it stable)
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

        s.Context.OnPhase(SortPhase.RadixPass, digit, digit);
        var divisor = pow10[digit];

        Span<int> counts = stackalloc int[RadixBase];
        counts.Clear(); // Required: [module: SkipLocalsInit] skips zero-initialization
        Span<int> offsets = stackalloc int[RadixBase];

        // Phase 1: Count occurrences of each digit value
        for (var i = 0; i < length; i++)
        {
            var key = radixKey.GetKey(s.Read(start + i));
            var digitValue = (int)((key / divisor) % 10);
            counts[digitValue]++;
        }

        // Phase 2: Calculate bucket offsets (prefix sum)
        offsets[0] = 0;
        for (var i = 1; i < RadixBase; i++)
        {
            offsets[i] = offsets[i - 1] + counts[i - 1];
        }

        // Phase 3: Distribute elements into temp buffer (forward scan keeps stability)
        Span<int> writePos = stackalloc int[RadixBase];
        offsets.CopyTo(writePos);

        for (var i = 0; i < length; i++)
        {
            var value = s.Read(start + i);
            var key = radixKey.GetKey(value);
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
                MSDSort(s, temp, radixKey, start + offsets[i], counts[i], digit - 1, pow10);
            }
        }
    }

    /// <summary>
    /// Compute the actual maximum digit count needed for the given data.
    /// This performs a single pass through the data to find the maximum key value,
    /// then calculates the number of digits required.
    /// This optimization is crucial for MSD radix sort to avoid processing empty high-order digits.
    /// </summary>
    /// <remarks>
    /// MSD Optimization:
    /// - Without this: Always starts from maximum possible digits (e.g., 20 for 64-bit), causing empty recursion
    /// - With this: Starts from actual required digits (e.g., 3 for values &lt;= 999), avoiding unnecessary passes
    ///
    /// Performance Impact:
    /// - One O(n) pass upfront to scan maxKey
    /// - Eliminates O(d × n) work for d unnecessary high-order digits
    /// - Critical for data with small values in large integer types (e.g., byte values in long arrays)
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ComputeMaxDigit<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, TRadixKey radixKey, int start, int length)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        if (length == 0) return 0;

        // Find maximum key in the data
        var maxKey = 0UL;
        for (var i = 0; i < length; i++)
        {
            var key = radixKey.GetKey(s.Read(start + i));
            if (key > maxKey)
            {
                maxKey = key;
            }
        }

        // Calculate digit count from maxKey
        // log₁₀(maxKey) + 1, but using iterative division to avoid floating point
        if (maxKey == 0) return 1; // Special case: all keys map to zero

        var digitCount = 0;
        var temp = maxKey;
        while (temp > 0)
        {
            temp /= 10;
            digitCount++;
        }

        return digitCount;
    }
}
