using SortAlgorithm.Contexts;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// American Flag Sort - In-place MSD Radix Sortの実装。
/// 値をビット列として扱い、4ビットずつ（16種類）の桁に分けて要素を分類し、in-placeで並び替えます。
/// 最上位桁（Most Significant Digit）から最下位桁へ向かって処理することで、再帰的にソートを実現します。
/// RadixMSDSortと異なり、補助バッファの使用を最小限に抑え、配列内で要素をスワップすることでin-placeソートを実現します。
/// <br/>
/// American Flag Sort - An in-place MSD Radix Sort implementation.
/// Treats values as bit sequences, dividing them into 4-bit digits (16 buckets) and classifying elements in-place.
/// Processing from the Most Significant Digit to the least significant ensures a recursive sort.
/// Unlike RadixMSDSort, this implementation minimizes auxiliary buffer usage and achieves in-place sorting by swapping elements within the array.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct American Flag Sort (Base-4):</strong></para>
/// <list type="number">
/// <item><description><strong>Order-Preserving Key Mapping:</strong> Elements are mapped to fixed-width unsigned keys through
/// <see cref="IRadixKeySelector{T}"/>. Signed integers flip the sign bit (e.g. 32-bit: key = (uint)value ^ 0x8000_0000),
/// floating-point values use the IEEE 754 total-order bit transform, and key-selector overloads extract an int key from arbitrary elements.
/// This ensures negative values are ordered correctly before positive values without separate processing.</description></item>
/// <item><description><strong>Digit Extraction Correctness:</strong> For each digit position d (from digitCount-1 down to 0), extract the d-th 4-bit digit using bitwise operations:
/// digit = (key >> (d × 4)) &amp; 0xF. This ensures each 4-bit segment of the integer is processed independently.</description></item>
/// <item><description><strong>In-Place Permutation:</strong> Elements are rearranged in-place using a two-pass approach:
/// 1. Count phase: Count occurrences of each digit value
/// 2. Permutation phase: Place each element in its correct bucket position using bucket offsets</description></item>
/// <item><description><strong>MSD Processing Order:</strong> Digits must be processed from most significant (d=digitCount-1) to least significant (d=0).
/// This top-down approach partitions the array into buckets recursively, processing each bucket independently for subsequent digits.</description></item>
/// <item><description><strong>Recursive Bucket Processing:</strong> After permuting elements based on the current digit, each bucket must be recursively sorted for the remaining digits.
/// Base cases: buckets with 0 or 1 elements are already sorted; buckets where all remaining digits are the same are also sorted.</description></item>
/// <item><description><strong>Cutoff to Insertion Sort:</strong> For small buckets (typically &lt; 16 elements), switching to insertion sort can improve performance due to lower overhead.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution (Radix Sort, MSD variant, American Flag Sort)</description></item>
/// <item><description>Stable      : No (in-place permutation does not preserve relative order)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space, excluding recursion stack)</description></item>
/// <item><description>Best case   : Θ(n) - When all elements fall into one bucket early</description></item>
/// <item><description>Average case: Θ(d × n) - d = ⌈bitSize/4⌉ is constant for fixed-width integers</description></item>
/// <item><description>Worst case  : Θ(d × n) - Same complexity regardless of input order</description></item>
/// <item><description>Comparisons : 0 (Non-comparison sort, uses bitwise operations only)</description></item>
/// <item><description>Digit Passes: up to d = ⌈bitSize/4⌉ (2 for byte, 4 for short, 8 for int, 16 for long), but can terminate early</description></item>
/// <item><description>Memory      : O(1) auxiliary space (excluding recursion stack which is O(log n) expected, O(n) worst case)</description></item>
/// </list>
/// <para><strong>Algorithm Overview:</strong></para>
/// <para>The algorithm consists of four phases per digit level:</para>
/// <list type="number">
/// <item><description><strong>Count Phase:</strong> Count occurrences of each digit value (0-15)</description></item>
/// <item><description><strong>Offset Calculation:</strong> Compute bucket offsets (cumulative sum)</description></item>
/// <item><description><strong>Permutation Phase:</strong> Rearrange elements into their buckets in-place using cyclic permutations</description></item>
/// <item><description><strong>Recursive Phase:</strong> Recursively sort each non-empty bucket for the next digit</description></item>
/// </list>
/// <para><strong>Supported Key Mappings (via <see cref="IRadixKeySelector{T}"/>):</strong></para>
/// <list type="bullet">
/// <item><description><strong>Integers:</strong> byte, sbyte, short, ushort, int, uint, long, ulong, nint, nuint (up to 64-bit); Int128/UInt128/BigInteger are rejected (64-bit key ceiling, see below)</description></item>
/// <item><description><strong>Floating point:</strong> Half, float, double via IEEE 754 total-order key transform (all NaN values sort first, matching <see cref="IComparable{T}"/> semantics)</description></item>
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
    private const int RadixBits = 4;        // 4 bits per digit
    private const int RadixSize = 16;       // 2^4 = 16 buckets
    private const int InsertionSortCutoff = 16; // Switch to insertion sort for small buckets

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

        // Validate the key width up front (BinaryIntegerRadixKey throws NotSupportedException for >64-bit types)
        // and derive the digit count from it (4 bits per digit).
        var digitCount = (TRadixKey.KeyBits + RadixBits - 1) / RadixBits;
        var s = new SortSpan<T, TComparer, TContext>(span, context, comparer, BUFFER_MAIN);

        // Start American Flag Sort from the most significant digit
        AmericanFlagSortRecursive(s, radixKey, 0, s.Length, digitCount - 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AmericanFlagSortRecursive<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, TRadixKey radixKey, int start, int length, int digit)
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

        // Allocate bucket arrays on stack (RadixSize=16, so only small stack usage)
        Span<int> bucketCounts = stackalloc int[RadixSize + 1];  // Stores both start and end for each bucket
        Span<int> bucketNext = stackalloc int[RadixSize];        // Current write position for each bucket

        // Phase 1: Count occurrences of each digit value
        s.Context.OnPhase(SortPhase.RadixPass, digit, digit);
        // Store count for digit d in bucketCounts[d+1] (off-by-one trick for prefix sum)
        bucketCounts.Clear();

        for (var i = 0; i < length; i++)
        {
            var value = s.Read(start + i);
            var key = radixKey.GetKey(value);
            var digitValue = (int)((key >> shift) & 0xF);  // Extract 4-bit digit
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
                AmericanFlagSortRecursive(s, radixKey, start, length, digit - 1);

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
        PermuteInPlace(s, radixKey, start, shift, bucketCounts, bucketNext);

        // Phase 4: Recursively sort each bucket for the next digit
        for (var i = 0; i < RadixSize; i++)
        {
            // bucketCounts provides direct access to boundaries
            var bucketStart = bucketCounts[i];
            var bucketEnd = bucketCounts[i + 1];  // No conditional needed!
            var bucketLength = bucketEnd - bucketStart;

            if (bucketLength > 1)
            {
                AmericanFlagSortRecursive(s, radixKey, start + bucketStart, bucketLength, digit - 1);
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
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void PermuteInPlace<T, TRadixKey, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, TRadixKey radixKey, int start, int shift, Span<int> bucketCounts, Span<int> bucketNext)
        where TRadixKey : struct, IRadixKeySelector<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // In-place permutation using bucket positions
        // Process each bucket sequentially
        for (var bucket = 0; bucket < RadixSize; bucket++)
        {
            // Get the range for this bucket directly from bucketCounts
            // bucketCounts[bucket] = start, bucketCounts[bucket + 1] = end
            var bucketEnd = bucketCounts[bucket + 1];

            // Move elements to their correct positions within and across buckets
            while (bucketNext[bucket] < bucketEnd)
            {
                var currentPos = start + bucketNext[bucket];
                var currentValue = s.Read(currentPos);
                var currentKey = radixKey.GetKey(currentValue);
                var currentDigit = (int)((currentKey >> shift) & 0xF);

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
