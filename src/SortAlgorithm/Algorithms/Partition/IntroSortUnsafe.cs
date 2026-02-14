using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// IntroSort implementation using Unsafe operations for maximum performance.
/// This is a direct port of dotnet runtime's GenericArraySortHelper&lt;T&gt; using Unsafe and MemoryMarshal.
/// <br/>
/// A hybrid sorting algorithm that combines QuickSort, HeapSort, and InsertionSort.
/// It primarily uses QuickSort, but switches to InsertionSort for small arrays and HeapSort when recursion depth becomes too deep,
/// avoiding QuickSort's worst-case O(n²) and guaranteeing O(n log n) in all cases.
/// </summary>
/// <remarks>
/// <para><strong>Key Optimizations:</strong></para>
/// <list type="bullet">
/// <item><description><strong>Unsafe operations:</strong> Uses Unsafe.Add and MemoryMarshal.GetReference to eliminate bounds checks</description></item>
/// <item><description><strong>Type-specialized comparisons:</strong> LessThan/GreaterThan methods compile to single CPU instructions for primitives</description></item>
/// <item><description><strong>No abstraction overhead:</strong> No SortSpan, no StatisticsContext - direct span manipulation only</description></item>
/// <item><description><strong>Simple median-of-3:</strong> Uses dotnet runtime's exact pivot selection strategy</description></item>
/// <item><description><strong>1-based heap indexing:</strong> Simpler parent/child calculations for HeapSort</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Hybrid (Partition (base) + Heap + Insertion)</description></item>
/// <item><description>Stable      : No (QuickSort and HeapSort are unstable; element order is not preserved for equal values)</description></item>
/// <item><description>In-place    : Yes (O(log n) auxiliary space for recursion stack, no additional arrays allocated)</description></item>
/// <item><description>Best case   : Θ(n log n) - Occurs when QuickSort consistently creates balanced partitions and InsertionSort handles small subarrays efficiently</description></item>
/// <item><description>Average case: Θ(n log n) - Expected ~1.386n log₂ n comparisons from QuickSort</description></item>
/// <item><description>Worst case  : O(n log n) - Guaranteed by HeapSort fallback when recursion depth exceeds 2⌊log₂(n)⌋</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>dotnet runtime: https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Collections/Generic/ArraySortHelper.cs</para>
/// </remarks>
public static class IntroSortUnsafe
{
    // Follow dotnet runtime threshold
    private const int IntrosortSizeThreshold = 16;

    /// <summary>
    /// Sorts the elements in the specified span in ascending order.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="keys">The span of elements to sort in place.</param>
    public static void Sort<T>(Span<T> keys) where T : IComparable<T>
    {
        if (keys.Length > 1)
        {
            IntroSort(keys, 2 * (BitOperations.Log2((uint)keys.Length) + 1));
        }
    }

    /// <summary>
    /// Internal IntroSort implementation. This is marked with NoInlining to prevent
    /// the JIT from inlining recursive calls into itself, which would hurt performance.
    /// </summary>
    /// <remarks>
    /// From dotnet runtime comments:
    /// "IntroSort is recursive; block it from being inlined into itself as this is currently not profitable."
    /// </remarks>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void IntroSort<T>(Span<T> keys, int depthLimit) where T : IComparable<T>
    {
        int partitionSize = keys.Length;
        while (partitionSize > 1)
        {
            if (partitionSize <= IntrosortSizeThreshold)
            {
                if (partitionSize == 2)
                {
                    SwapIfGreater(ref keys[0], ref keys[1]);
                    return;
                }

                if (partitionSize == 3)
                {
                    ref T hiRef = ref keys[2];
                    ref T him1Ref = ref keys[1];
                    ref T loRef = ref keys[0];

                    SwapIfGreater(ref loRef, ref him1Ref);
                    SwapIfGreater(ref loRef, ref hiRef);
                    SwapIfGreater(ref him1Ref, ref hiRef);
                    return;
                }

                InsertionSort(keys.Slice(0, partitionSize));
                return;
            }

            if (depthLimit == 0)
            {
                HeapSort(keys.Slice(0, partitionSize));
                return;
            }
            depthLimit--;

            int p = PickPivotAndPartition(keys.Slice(0, partitionSize));

            // Note we've already partitioned around the pivot and do not have to move the pivot again.
            IntroSort(keys[(p + 1)..partitionSize], depthLimit);
            partitionSize = p;
        }
    }

    /// <summary>
    /// Picks a pivot using median-of-3 and partitions the range.
    /// This is a direct port of dotnet runtime's PickPivotAndPartition.
    /// </summary>
    private static unsafe int PickPivotAndPartition<T>(Span<T> keys) where T : IComparable<T>
    {
        // Use median-of-three to select a pivot. Grab a reference to the 0th, Length-1th, and Length/2th elements, and sort them.
        ref T zeroRef = ref MemoryMarshal.GetReference(keys);
        ref T lastRef = ref Unsafe.Add(ref zeroRef, keys.Length - 1);
        ref T middleRef = ref Unsafe.Add(ref zeroRef, (keys.Length - 1) >> 1);
        SwapIfGreater(ref zeroRef, ref middleRef);
        SwapIfGreater(ref zeroRef, ref lastRef);
        SwapIfGreater(ref middleRef, ref lastRef);

        // Select the middle value as the pivot, and move it to be just before the last element.
        ref T nextToLastRef = ref Unsafe.Add(ref zeroRef, keys.Length - 2);
        T pivot = middleRef;
        Swap(ref middleRef, ref nextToLastRef);

        // Walk the left and right pointers, swapping elements as necessary, until they cross.
        ref T leftRef = ref zeroRef, rightRef = ref nextToLastRef;
        while (Unsafe.IsAddressLessThan(ref leftRef, ref rightRef))
        {
            if (pivot == null)
            {
                while (Unsafe.IsAddressLessThan(ref leftRef, ref nextToLastRef) && (leftRef = ref Unsafe.Add(ref leftRef, 1)) == null) ;
                while (Unsafe.IsAddressGreaterThan(ref rightRef, ref zeroRef) && (rightRef = ref Unsafe.Add(ref rightRef, -1)) != null) ;
            }
            else
            {
                while (Unsafe.IsAddressLessThan(ref leftRef, ref nextToLastRef) && GreaterThan(ref pivot, ref leftRef = ref Unsafe.Add(ref leftRef, 1))) ;
                while (Unsafe.IsAddressGreaterThan(ref rightRef, ref zeroRef) && LessThan(ref pivot, ref rightRef = ref Unsafe.Add(ref rightRef, -1))) ;
            }

            if (Unsafe.IsAddressGreaterThanOrEqualTo(ref leftRef, ref rightRef))
            {
                break;
            }

            Swap(ref leftRef, ref rightRef);
        }

        // Put the pivot in the correct location.
        if (!Unsafe.AreSame(ref leftRef, ref nextToLastRef))
        {
            Swap(ref leftRef, ref nextToLastRef);
        }

        return (int)((nint)Unsafe.ByteOffset(ref zeroRef, ref leftRef) / Unsafe.SizeOf<T>());
    }

    /// <summary>
    /// InsertionSort implementation using Unsafe operations.
    /// </summary>
    private static void InsertionSort<T>(Span<T> keys) where T : IComparable<T>
    {
        for (int i = 0; i < keys.Length - 1; i++)
        {
            T t = Unsafe.Add(ref MemoryMarshal.GetReference(keys), i + 1);

            int j = i;
            while (j >= 0 && (t == null || LessThan(ref t, ref Unsafe.Add(ref MemoryMarshal.GetReference(keys), j))))
            {
                Unsafe.Add(ref MemoryMarshal.GetReference(keys), j + 1) = Unsafe.Add(ref MemoryMarshal.GetReference(keys), j);
                j--;
            }

            Unsafe.Add(ref MemoryMarshal.GetReference(keys), j + 1) = t!;
        }
    }

    /// <summary>
    /// HeapSort implementation using 1-based heap indexing.
    /// </summary>
    private static void HeapSort<T>(Span<T> keys) where T : IComparable<T>
    {
        int n = keys.Length;
        for (int i = n >> 1; i >= 1; i--)
        {
            DownHeap(keys, i, n);
        }

        for (int i = n; i > 1; i--)
        {
            Swap(ref keys[0], ref keys[i - 1]);
            DownHeap(keys, 1, i - 1);
        }
    }

    /// <summary>
    /// Restores the heap property using sift-down operation with 1-based indexing.
    /// </summary>
    private static void DownHeap<T>(Span<T> keys, int i, int n) where T : IComparable<T>
    {
        T d = keys[i - 1];
        while (i <= n >> 1)
        {
            int child = 2 * i;
            if (child < n && (keys[child - 1] == null || LessThan(ref keys[child - 1], ref keys[child])))
            {
                child++;
            }

            if (keys[child - 1] == null || !LessThan(ref d, ref keys[child - 1]))
                break;

            keys[i - 1] = keys[child - 1];
            i = child;
        }

        keys[i - 1] = d;
    }

    /// <summary>
    /// Swaps the values in the two references if the first is greater than the second.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SwapIfGreater<T>(ref T i, ref T j) where T : IComparable<T>
    {
        if (i != null && GreaterThan(ref i, ref j))
        {
            Swap(ref i, ref j);
        }
    }

    /// <summary>
    /// Swaps the values in the two references, regardless of whether the two references are the same.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Swap<T>(ref T i, ref T j)
    {
        T t = i;
        i = j;
        j = t;
    }

    /// <summary>
    /// Type-specialized LessThan comparison. Compiles to a single CPU instruction for primitives.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool LessThan<T>(ref T left, ref T right) where T : IComparable<T>
    {
        if (typeof(T) == typeof(byte)) return (byte)(object)left < (byte)(object)right;
        if (typeof(T) == typeof(sbyte)) return (sbyte)(object)left < (sbyte)(object)right;
        if (typeof(T) == typeof(ushort)) return (ushort)(object)left < (ushort)(object)right;
        if (typeof(T) == typeof(short)) return (short)(object)left < (short)(object)right;
        if (typeof(T) == typeof(uint)) return (uint)(object)left < (uint)(object)right;
        if (typeof(T) == typeof(int)) return (int)(object)left < (int)(object)right;
        if (typeof(T) == typeof(ulong)) return (ulong)(object)left < (ulong)(object)right;
        if (typeof(T) == typeof(long)) return (long)(object)left < (long)(object)right;
        if (typeof(T) == typeof(nuint)) return (nuint)(object)left < (nuint)(object)right;
        if (typeof(T) == typeof(nint)) return (nint)(object)left < (nint)(object)right;
        if (typeof(T) == typeof(float)) return (float)(object)left < (float)(object)right;
        if (typeof(T) == typeof(double)) return (double)(object)left < (double)(object)right;
        if (typeof(T) == typeof(Half)) return (Half)(object)left < (Half)(object)right;
        return left.CompareTo(right) < 0;
    }

    /// <summary>
    /// Type-specialized GreaterThan comparison. Compiles to a single CPU instruction for primitives.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool GreaterThan<T>(ref T left, ref T right) where T : IComparable<T>
    {
        if (typeof(T) == typeof(byte)) return (byte)(object)left > (byte)(object)right;
        if (typeof(T) == typeof(sbyte)) return (sbyte)(object)left > (sbyte)(object)right;
        if (typeof(T) == typeof(ushort)) return (ushort)(object)left > (ushort)(object)right;
        if (typeof(T) == typeof(short)) return (short)(object)left > (short)(object)right;
        if (typeof(T) == typeof(uint)) return (uint)(object)left > (uint)(object)right;
        if (typeof(T) == typeof(int)) return (int)(object)left > (int)(object)right;
        if (typeof(T) == typeof(ulong)) return (ulong)(object)left > (ulong)(object)right;
        if (typeof(T) == typeof(long)) return (long)(object)left > (long)(object)right;
        if (typeof(T) == typeof(nuint)) return (nuint)(object)left > (nuint)(object)right;
        if (typeof(T) == typeof(nint)) return (nint)(object)left > (nint)(object)right;
        if (typeof(T) == typeof(float)) return (float)(object)left > (float)(object)right;
        if (typeof(T) == typeof(double)) return (double)(object)left > (double)(object)right;
        if (typeof(T) == typeof(Half)) return (Half)(object)left > (Half)(object)right;
        return left.CompareTo(right) > 0;
    }
}
