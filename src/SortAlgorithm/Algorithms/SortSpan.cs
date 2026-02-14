using System.Runtime.CompilerServices;

using SortAlgorithm.Contexts;

using Unsafe = System.Runtime.CompilerServices.Unsafe;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// A wrapper around Span&lt;T&gt; that tracks sorting operations through ISortContext.
/// Supports buffer identification for visualization purposes.
/// Uses a generic TComparer for comparison operations, enabling JIT devirtualization when a struct comparer is used.
/// </summary>
/// <typeparam name="T">The type of elements in the span</typeparam>
/// <typeparam name="TComparer">The type of comparer to use for element comparisons</typeparam>
internal readonly ref struct SortSpan<T, TComparer> where TComparer : IComparer<T>
{
    private readonly Span<T> _span;
    private readonly ISortContext _context;
    private readonly TComparer _comparer;
    private readonly int _bufferId;

    /// <summary>
    /// Initializes a new instance of SortSpan with the specified span, context, and comparer.
    /// </summary>
    /// <param name="span">The span to wrap</param>
    /// <param name="context">The context for tracking operations</param>
    /// <param name="comparer">The comparer to use for element comparisons</param>
    /// <param name="bufferId">Buffer identifier (0 = main array, 1+ = auxiliary buffers). Default is 0.</param>
    public SortSpan(Span<T> span, ISortContext context, TComparer comparer, int bufferId)
    {
        _span = span;
        _context = context;
        _comparer = comparer;
        _bufferId = bufferId;
    }

    public int Length => _span.Length;

    /// <summary>
    /// Gets the buffer identifier for this span.
    /// </summary>
    public int BufferId => _bufferId;

    /// <summary>
    /// Gets the comparer used by this SortSpan.
    /// </summary>
    public TComparer Comparer => _comparer;

    /// <summary>
    /// Type-specialized comparison optimized for primitive types.
    /// When TComp is ComparableComparer&lt;TValue&gt; and TValue is a primitive type,
    /// this compiles to a single CPU instruction. Otherwise falls back to the comparer.
    /// </summary>
    /// <remarks>
    /// This optimization applies to both DEBUG and RELEASE builds, ensuring consistent
    /// behavior while providing maximum performance for benchmarks.
    /// JIT will eliminate all typeof() checks at compile time when types are known.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CompareOptimized<TValue, TComp>(ref TValue left, ref TValue right, TComp comparer)
        where TComp : IComparer<TValue>
    {
        // Only optimize when using ComparableComparer<T>
        // Use typeof() for compile-time type checking (JIT can eliminate this branch)
        if (typeof(TComp).Name == "ComparableComparer`1")
        {
            // Type-specialized comparisons for primitive types
            // These compile to single CPU instructions (e.g., cmp, jl, setl)
            if (typeof(TValue) == typeof(byte))
            {
                var l = Unsafe.As<TValue, byte>(ref left);
                var r = Unsafe.As<TValue, byte>(ref right);
                return l.CompareTo(r);
            }
            if (typeof(TValue) == typeof(sbyte))
            {
                var l = Unsafe.As<TValue, sbyte>(ref left);
                var r = Unsafe.As<TValue, sbyte>(ref right);
                return l.CompareTo(r);
            }
            if (typeof(TValue) == typeof(ushort))
            {
                var l = Unsafe.As<TValue, ushort>(ref left);
                var r = Unsafe.As<TValue, ushort>(ref right);
                return l.CompareTo(r);
            }
            if (typeof(TValue) == typeof(short))
            {
                var l = Unsafe.As<TValue, short>(ref left);
                var r = Unsafe.As<TValue, short>(ref right);
                return l.CompareTo(r);
            }
            if (typeof(TValue) == typeof(uint))
            {
                var l = Unsafe.As<TValue, uint>(ref left);
                var r = Unsafe.As<TValue, uint>(ref right);
                return l.CompareTo(r);
            }
            if (typeof(TValue) == typeof(int))
            {
                var l = Unsafe.As<TValue, int>(ref left);
                var r = Unsafe.As<TValue, int>(ref right);
                return l.CompareTo(r);
            }
            if (typeof(TValue) == typeof(ulong))
            {
                var l = Unsafe.As<TValue, ulong>(ref left);
                var r = Unsafe.As<TValue, ulong>(ref right);
                return l.CompareTo(r);
            }
            if (typeof(TValue) == typeof(long))
            {
                var l = Unsafe.As<TValue, long>(ref left);
                var r = Unsafe.As<TValue, long>(ref right);
                return l.CompareTo(r);
            }
            if (typeof(TValue) == typeof(nuint))
            {
                var l = Unsafe.As<TValue, nuint>(ref left);
                var r = Unsafe.As<TValue, nuint>(ref right);
                return l.CompareTo(r);
            }
            if (typeof(TValue) == typeof(nint))
            {
                var l = Unsafe.As<TValue, nint>(ref left);
                var r = Unsafe.As<TValue, nint>(ref right);
                return l.CompareTo(r);
            }
            if (typeof(TValue) == typeof(float))
            {
                var l = Unsafe.As<TValue, float>(ref left);
                var r = Unsafe.As<TValue, float>(ref right);
                return l.CompareTo(r);
            }
            if (typeof(TValue) == typeof(double))
            {
                var l = Unsafe.As<TValue, double>(ref left);
                var r = Unsafe.As<TValue, double>(ref right);
                return l.CompareTo(r);
            }
            if (typeof(TValue) == typeof(Half))
            {
                var l = Unsafe.As<TValue, Half>(ref left);
                var r = Unsafe.As<TValue, Half>(ref right);
                return l.CompareTo(r);
            }
        }

        // Fallback to comparer for non-primitive types or custom comparers
        return comparer.Compare(left, right);
    }

    /// <summary>
    /// Retrieves the element at the specified zero-based index. (Equivalent to span[i].)
    /// </summary>
    /// <param name="i">The zero-based index of the element to retrieve. Must be within the bounds of the collection.</param>
    /// <returns>The element of type T at the specified index.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Read(int i)
    {
#if DEBUG
        _context.OnIndexRead(i, _bufferId);
#endif
        return _span[i];
    }

    /// <summary>
    /// Sets the element at the specified index to the given value. (Equivalent to span[i] = value.)
    /// </summary>
    /// <param name="i">The zero-based index of the element to set.</param>
    /// <param name="value">The value to assign to the element at the specified index.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(int i, T value)
    {
#if DEBUG
        _context.OnIndexWrite(i, _bufferId, value);
#endif
        _span[i] = value;
    }

    /// <summary>
    /// Compares the elements at the specified indices and returns an integer that indicates their relative order. (Equivalent to comparer.Compare(span[i], span[j]).)
    /// </summary>
    /// <param name="i">The index of the first element to compare.</param>
    /// <param name="j">The index of the second element to compare.</param>
    /// <returns>A signed integer that indicates the relative order of the elements: less than zero if the element at index i is
    /// less than the element at index j; zero if they are equal; greater than zero if the element at index i is greater
    /// than the element at index j.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Compare(int i, int j)
    {
#if DEBUG
        var a = Read(i);
        var b = Read(j);
        var result = CompareOptimized(ref a, ref b, _comparer);
        _context.OnCompare(i, j, result, _bufferId, _bufferId);
        return result;
#else
        return CompareOptimized(ref _span[i], ref _span[j], _comparer);
#endif
    }

    /// <summary>
    /// Compares the element at the specified index with a given value. (Equivalent to comparer.Compare(span[i], value).)
    /// </summary>
    /// <param name="i">The index of the element to compare.</param>
    /// <param name="value">The value to compare against.</param>
    /// <returns>A signed integer that indicates the relative order: less than zero if the element at index i is
    /// less than value; zero if they are equal; greater than zero if the element at index i is greater than value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Compare(int i, T value)
    {
#if DEBUG
        var a = Read(i);
        var result = CompareOptimized(ref a, ref value, _comparer);
        _context.OnCompare(i, -1, result, _bufferId, -1);
        return result;
#else
        return CompareOptimized(ref _span[i], ref value, _comparer);
#endif
    }

    /// <summary>
    /// Compares a given value with the element at the specified index. (Equivalent to comparer.Compare(value, span[i]).)
    /// </summary>
    /// <param name="value">The value to compare.</param>
    /// <param name="i">The index of the element to compare against.</param>
    /// <returns>A signed integer that indicates the relative order: less than zero if value is
    /// less than the element at index i; zero if they are equal; greater than zero if value is greater than the element at index i.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Compare(T value, int i)
    {
#if DEBUG
        var b = Read(i);
        var result = CompareOptimized(ref value, ref b, _comparer);
        _context.OnCompare(-1, i, result, -1, _bufferId);
        return result;
#else
        return CompareOptimized(ref value, ref _span[i], _comparer);
#endif
    }

    /// <summary>
    /// Compares two values directly (not from the span). (Equivalent to comparer.Compare(a, b).)
    /// </summary>
    /// <param name="a">The first value to compare.</param>
    /// <param name="b">The second value to compare.</param>
    /// <returns>A signed integer that indicates the relative order: less than zero if a is
    /// less than b; zero if they are equal; greater than zero if a is greater than b.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Compare(T a, T b)
    {
#if DEBUG
        var result = CompareOptimized(ref a, ref b, _comparer);
        _context.OnCompare(-1, -1, result, -1, -1);
        return result;
#else
        return CompareOptimized(ref a, ref b, _comparer);
#endif
    }

    /// <summary>
    /// Exchanges the values at the specified indices within the collection. (Equivalent to swapping span[i] and span[j].)
    /// </summary>
    /// <remarks>This method notifies the underlying context of the swap operation before updating the values.
    /// Both indices must refer to valid elements within the collection.</remarks>
    /// <param name="i">The index of the first element to swap.</param>
    /// <param name="j">The index of the second element to swap.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Swap(int i, int j)
    {
#if DEBUG
        _context.OnSwap(i, j, _bufferId);
#endif
        (_span[i], _span[j]) = (_span[j], _span[i]);
    }

    /// <summary>
    /// Copies a range of elements from this span to another SortSpan. (Equivalent to source.Slice(sourceIndex, length).CopyTo(destination.Slice(destinationIndex)).)
    /// </summary>
    /// <param name="sourceIndex">The starting index in the source span.</param>
    /// <param name="destination">The destination SortSpan to copy to.</param>
    /// <param name="destinationIndex">The starting index in the destination span.</param>
    /// <param name="length">The number of elements to copy.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(int sourceIndex, SortSpan<T, TComparer> destination, int destinationIndex, int length)
    {
#if DEBUG
        var values = new object?[length];
        for (int i = 0; i < length; i++)
        {
            values[i] = _span[sourceIndex + i];
        }
        _context.OnRangeCopy(sourceIndex, destinationIndex, length, _bufferId, destination.BufferId, values);
#endif
        _span.Slice(sourceIndex, length).CopyTo(destination._span.Slice(destinationIndex, length));
    }

    /// <summary>
    /// Copies a range of elements from this span to a regular Span. (Equivalent to source.Slice(sourceIndex, length).CopyTo(destination.Slice(destinationIndex)).)
    /// </summary>
    /// <param name="sourceIndex">The starting index in the source span.</param>
    /// <param name="destination">The destination Span to copy to.</param>
    /// <param name="destinationIndex">The starting index in the destination span.</param>
    /// <param name="length">The number of elements to copy.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(int sourceIndex, Span<T> destination, int destinationIndex, int length)
    {
#if DEBUG
        var values = new object?[length];
        for (int i = 0; i < length; i++)
        {
            values[i] = _span[sourceIndex + i];
        }
        _context.OnRangeCopy(sourceIndex, destinationIndex, length, _bufferId, -1, values);
#endif
        _span.Slice(sourceIndex, length).CopyTo(destination.Slice(destinationIndex, length));
    }
}
