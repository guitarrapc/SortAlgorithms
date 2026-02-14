using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using SortAlgorithm.Contexts;

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
    /// Gets a reference to the element at the specified index without bounds checking.
    /// </summary>
    /// <param name="i">The zero-based index of the element to access.</param>
    /// <returns>A reference to the element at the specified index.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref T UnguardedAccess(int i)
    {
        return ref Unsafe.Add(ref MemoryMarshal.GetReference(_span), i);
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
        // Optimize span access by avoiding bounds checking
        // _span[i];
        return UnguardedAccess(i);
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
        // Optimize span access by avoiding bounds checking
        // _span[i] = value;
        UnguardedAccess(i) = value;
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
        var result = _comparer.Compare(Read(i), Read(j));
        _context.OnCompare(i, j, result, _bufferId, _bufferId);
        return result;
#else
        return _comparer.Compare(Read(i), Read(j));
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
        var result = _comparer.Compare(a, value);
        _context.OnCompare(i, -1, result, _bufferId, -1);
        return result;
#else
        return _comparer.Compare(Read(i), value);
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
        var result = _comparer.Compare(value, b);
        _context.OnCompare(-1, i, result, -1, _bufferId);
        return result;
#else
        return _comparer.Compare(value, Read(i));
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
        var result = _comparer.Compare(a, b);
        _context.OnCompare(-1, -1, result, -1, -1);
        return result;
#else
        return _comparer.Compare(a, b);
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
        var a = UnguardedAccess(i);
        var b = UnguardedAccess(j);
        (a, b) = (b, a);
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
