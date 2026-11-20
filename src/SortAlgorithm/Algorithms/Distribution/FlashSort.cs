using SortAlgorithm.Contexts;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 整数キーを使用したフラッシュソートのジェネリック版。値が均等に分布している場合に最適に動作します。
/// 値域を m 個のクラスに分割し、各要素をクラスに振り分けた後、置換サイクルでインプレースに並べ替えます。
/// 各クラス内を挿入ソートで仕上げることでソートが完了します。
/// <br/>
/// Flash sort (generic, integer key), a distribution-based sort that performs optimally when values are uniformly distributed.
/// Divides the value range into m classes, distributes elements into classes, then rearranges them in-place using permutation cycles.
/// A final insertion sort pass within each class completes the sort.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Flash Sort (Generic, Range-based):</strong></para>
/// <list type="number">
/// <item><description><strong>Key Extraction:</strong> Each element must have a deterministic unsigned 64-bit key derived from its binary representation.
/// Signed integers are converted to unsigned by bit-casting; the key is stable.</description></item>
/// <item><description><strong>Range Partitioning:</strong> The key range [minKey, maxKey] is divided linearly into m classes.
/// Class index K = ⌊c1 × (key − minKey)⌋ with c1 = (m − 1) / (maxKey − minKey), so the minimum falls in class 0 and the maximum in class m − 1.</description></item>
/// <item><description><strong>Class Count:</strong> This implementation uses m = max(2, ⌊0.43 × n⌋) classes, the optimum m/n = √(3/16) ≈ 0.43
/// that Neubert derives from his cost model for uniformly distributed keys.</description></item>
/// <item><description><strong>Permutation Phase:</strong> Elements are placed into their class regions by in-place permutation cycles.
/// The maximum element is first exchanged to index 0, which starts the first cycle. A cycle holds one element in a temporary,
/// writes it to the next free slot of its class and picks up the element it evicts, so each element is moved once.</description></item>
/// <item><description><strong>Per-Class Sorting:</strong> Each class region is sorted using Insertion Sort. Every element is already in its
/// class, so this finishes the sort; it does not restore the input order of equal keys, which the permutation has already lost.</description></item>
/// <item><description><strong>Uniform Distribution Assumption:</strong> Optimal performance (O(n)) requires uniform key distribution.
/// Worst case O(n²) occurs when all elements fall into a single class.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution</description></item>
/// <item><description>Stable      : No (permutation phase does not preserve relative order)</description></item>
/// <item><description>In-place    : Yes (O(m) auxiliary space for class boundaries only)</description></item>
/// <item><description>Best case   : Ω(n) - Uniform distribution, one pass over the array</description></item>
/// <item><description>Average case: Θ(n) - Assumes uniform distribution, permutation phase is O(n)</description></item>
/// <item><description>Worst case  : O(n²) - All elements in one class, degenerates to Insertion Sort</description></item>
/// <item><description>Comparisons : O(n) expected - a class holds n/m elements in expectation, and Insertion Sort over a class of constant expected size costs O(1); O(n²) worst, when one class holds everything</description></item>
/// <item><description>Swaps       : O(1) - a single exchange brings the maximum to the front; the permutation cycle moves elements by assignment, not by swapping</description></item>
/// <item><description>Index Reads : Θ(n) on average - the range scan, the classification and the permutation each touch every element once; O(n²) worst, when the final Insertion Sort degenerates</description></item>
/// <item><description>Index Writes: Θ(n) on average - the permutation places each element once; O(n²) worst for the same reason</description></item>
/// <item><description>Space       : O(m) - Two integer arrays of size m for counts and boundaries</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description><strong>Floating-point class constant:</strong> c1 is a double, as in Neubert's code. Converting to double, multiplying by a
/// positive constant and truncating are each monotonic, so a larger key never lands in a smaller class, and c1 × (maxKey − minKey) stays
/// below m for every m a span can reach. Counting and permuting call the same function, which is all the cycles need. Exact 128-bit
/// arithmetic would add a division per classification, and every element is classified at least twice.</description></item>
/// <item><description><strong>Final pass per class:</strong> Neubert runs one straight insertion sort over the whole array. Sorting each class
/// on its own moves the same elements and skips the comparison at each class boundary, at the cost of keeping the boundaries.</description></item>
/// </list>
/// <para><strong>Note:</strong> クラス数は 0.43n (最小 2) に自動調整されます。キーの分布が偏るとパフォーマンスが低下します。128ビット整数型には対応していません。</para>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Flashsort</para>
/// <para>Karl-Dietrich Neubert, "The Flashsort1 Algorithm", Dr. Dobb's Journal, February 1998.</para>
/// </remarks>
public static class FlashSort
{
    /// <summary>
    /// Whether this algorithm preserves the relative order of elements that compare equal.
    /// </summary>
    /// <remarks>See the stability note in this type's summary.</remarks>
    public static bool IsStable => false;


    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;           // Main input array

    /// <summary>
    /// Sorts the elements in the specified span using FlashSort.
    /// Uses NullContext for zero-overhead fast path.
    /// </summary>
    /// <typeparam name="T">The type of elements to sort. Must be a supported binary integer type up to 64 bits.</typeparam>
    /// <param name="span">The span of elements to sort.</param>
    /// <exception cref="NotSupportedException">
    /// Thrown when <typeparamref name="T"/> is a 128-bit type (<see cref="Int128"/> or <see cref="UInt128"/>).
    /// </exception>
    public static void Sort<T>(Span<T> span) where T : IBinaryInteger<T>
        => Sort(span, NullContext.Default);

    /// <summary>
    /// Sorts the elements in the specified span using FlashSort with context tracking.
    /// </summary>
    /// <typeparam name="T">The type of elements to sort. Must be a supported binary integer type up to 64 bits.</typeparam>
    /// <typeparam name="TContext">The type of context for tracking operations.</typeparam>
    /// <param name="span">The span of elements to sort.</param>
    /// <param name="context">The sort context for tracking operations.</param>
    /// <exception cref="NotSupportedException">
    /// Thrown when <typeparamref name="T"/> is a 128-bit type (<see cref="Int128"/> or <see cref="UInt128"/>).
    /// This implementation only supports integer types up to 64-bit due to key storage constraints.
    /// </exception>
    public static void Sort<T, TContext>(Span<T> span, TContext context)
        where T : IBinaryInteger<T>
        where TContext : ISortContext
    {
        if (span.Length <= 1) return;

        var n = span.Length;
        var bitSize = GetBitSize<T>();



        var s = new SortSpan<T, NullComparer<T>, TContext>(span, context, default, BUFFER_MAIN);

        // Pass 1: find min/max unsigned keys and the index of the maximum element
        context.OnPhase(SortPhase.DistributionCount);

        var minKey = ulong.MaxValue;
        var maxKey = ulong.MinValue;
        var maxIdx = 0;

        for (var i = 0; i < n; i++)
        {
            var key = GetUnsignedKey(s.Read(i), bitSize);
            if (key < minKey) minKey = key;
            if (key > maxKey) { maxKey = key; maxIdx = i; }
        }

        // All elements equal: already sorted
        if (minKey == maxKey) return;

        // m = number of classes, at Neubert's optimum of 0.43·n
        var m = Math.Max(2, (int)(0.43 * n));
        var c1 = (m - 1) / (double)(maxKey - minKey);

        var countArr = ArrayPool<int>.Shared.Rent(m);
        var boundaryArr = ArrayPool<int>.Shared.Rent(m);

        try
        {
            var count = countArr.AsSpan(0, m);
            var boundary = boundaryArr.AsSpan(0, m);
            count.Clear();

            // Pass 2: count elements per class
            for (var i = 0; i < n; i++)
            {
                var key = GetUnsignedKey(s.Read(i), bitSize);
                count[ClassOf(key, minKey, c1)]++;
            }

            // Pass 3: prefix sum → count[k] becomes exclusive upper bound of class k
            context.OnPhase(SortPhase.DistributionAccumulate);
            for (var k = 1; k < m; k++)
                count[k] += count[k - 1];

            // Save boundaries before permutation modifies count[]
            count.CopyTo(boundary);

            context.OnPhase(SortPhase.DistributionWrite);

            // Move the maximum element to index 0: the first cycle starts there, in the last class
            s.Swap(maxIdx, 0);

            // Pass 4: permutation phase — place every element in its correct class region.
            // count[k] is the exclusive end of class k's unfilled slots; a cycle fills them from the top down.
            var move = 0;
            var j = 0;
            var kClass = ClassOf(maxKey, minKey, c1);

            while (move < n - 1)
            {
                // Find the next cycle leader. Slot j is filled when j >= count[kClass], and kClass already holds the class
                // of the element at j, so only a step reads. An unplaced element leaves an unfilled slot at or after j.
                while (j >= count[kClass])
                {
                    j++;
                    kClass = ClassOf(GetUnsignedKey(s.Read(j), bitSize), minKey, c1);
                }

                // Pick up the element at j and cycle it into its correct class
                var flash = s.Read(j);

                // The cycle closes when the next free slot of flash's class is j itself
                while (j != count[kClass])
                {
                    kClass = ClassOf(GetUnsignedKey(flash, bitSize), minKey, c1);
                    var dest = --count[kClass];
                    var hold = s.Read(dest);
                    s.Write(dest, flash);
                    flash = hold;
                    move++;
                }
            }

            // Pass 5: insertion sort within each class region [boundary[k-1], boundary[k])
            // Uses absolute indices and a real comparer so statistics and visualization are accurate
            var insertionSpan = new SortSpan<T, ComparableComparer<T>, TContext>(span, context, new ComparableComparer<T>(), BUFFER_MAIN);
            var lo = 0;
            for (var k = 0; k < m; k++)
            {
                var hi = boundary[k];
                if (hi - lo > 1)
                    InsertionSort.SortCore(insertionSpan, lo, hi);
                lo = hi;
            }
        }
        finally
        {
            ArrayPool<int>.Shared.Return(countArr, clearArray: false);
            ArrayPool<int>.Shared.Return(boundaryArr, clearArray: false);
        }
    }

    /// <summary>
    /// Maps an unsigned key to its class, ⌊c1 × (key − minKey)⌋ with c1 = (m − 1) / (maxKey − minKey).
    /// The result lies in [0, m − 1] and never decreases as the key grows (see the class remarks).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ClassOf(ulong key, ulong minKey, double c1)
        => (int)(c1 * (key - minKey));

    /// <summary>
    /// Get the bit size of the type T.
    /// </summary>
    /// <exception cref="NotSupportedException">Thrown for 128-bit types or unsupported types.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetBitSize<T>() where T : IBinaryInteger<T>
    {
        if (typeof(T) == typeof(sbyte) || typeof(T) == typeof(byte))
            return 8;
        else if (typeof(T) == typeof(short) || typeof(T) == typeof(ushort))
            return 16;
        else if (typeof(T) == typeof(int) || typeof(T) == typeof(uint))
            return 32;
        else if (typeof(T) == typeof(long) || typeof(T) == typeof(ulong))
            return 64;
        else if (typeof(T) == typeof(nint) || typeof(T) == typeof(nuint))
            return IntPtr.Size * 8; // 32-bit or 64-bit depending on platform
        else if (typeof(T) == typeof(Int128) || typeof(T) == typeof(UInt128))
            throw new NotSupportedException($"Type {typeof(T).Name} with 128-bit size is not supported. Maximum supported bit size is 64.");
        else
            throw new NotSupportedException($"Type {typeof(T).Name} is not supported.");
    }

    /// <summary>
    /// Convert a signed or unsigned value to an unsigned key for classification.
    /// For signed types, flips the sign bit so that the natural unsigned ordering matches
    /// the signed ordering (negatives sort before positives with no overflow risk).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong GetUnsignedKey<T>(T value, int bitSize) where T : IBinaryInteger<T>
    {
        if (bitSize <= 8)
        {
            var byteValue = byte.CreateTruncating(value);
            if (typeof(T) == typeof(sbyte))
                return (byte)(byteValue ^ 0x80);
            return byteValue;
        }
        else if (bitSize <= 16)
        {
            var ushortValue = ushort.CreateTruncating(value);
            if (typeof(T) == typeof(short))
                return (ushort)(ushortValue ^ 0x8000);
            return ushortValue;
        }
        else if (bitSize <= 32)
        {
            var uintValue = uint.CreateTruncating(value);
            if (typeof(T) == typeof(int) || (typeof(T) == typeof(nint) && IntPtr.Size == 4))
                return uintValue ^ 0x8000_0000;
            return uintValue;
        }
        else
        {
            var ulongValue = ulong.CreateTruncating(value);
            if (typeof(T) == typeof(long) || (typeof(T) == typeof(nint) && IntPtr.Size == 8))
                return ulongValue ^ 0x8000_0000_0000_0000;
            return ulongValue;
        }
    }
}
