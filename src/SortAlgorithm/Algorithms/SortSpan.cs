using System.Runtime.CompilerServices;

using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// A wrapper around Span&lt;T&gt; that tracks sorting operations through ISortContext.
/// Supports buffer identification for visualization purposes.
/// Uses a generic TComparer for comparison operations, enabling JIT devirtualization when a struct comparer is used.
/// Uses a generic TContext to enable zero-overhead optimization when NullContext is used (JIT eliminates all tracking code).
/// </summary>
/// <typeparam name="T">The type of elements in the span</typeparam>
/// <typeparam name="TComparer">The type of comparer to use for element comparisons</typeparam>
/// <typeparam name="TContext">The type of context for tracking operations (use NullContext for zero-overhead fast path)</typeparam>
internal readonly ref struct SortSpan<T, TComparer, TContext> 
    where TComparer : IComparer<T>
    where TContext : ISortContext
{
    private readonly Span<T> _span;
    private readonly TContext _context;
    private readonly TComparer _comparer;
    private readonly int _bufferId;

    /// <summary>
    /// Initializes a new instance of SortSpan with the specified span, context, and comparer.
    /// </summary>
    /// <param name="span">The span to wrap</param>
    /// <param name="context">The context for tracking operations</param>
    /// <param name="comparer">The comparer to use for element comparisons</param>
    /// <param name="bufferId">Buffer identifier (0 = main array, 1+ = auxiliary buffers). Default is 0.</param>
    public SortSpan(Span<T> span, TContext context, TComparer comparer, int bufferId)
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
    /// Gets the comparer used by this SortSpan.
    /// </summary>
    public TContext Context => _context;

    /// <summary>
    /// Retrieves the element at the specified zero-based index. (Equivalent to span[i].)
    /// </summary>
    /// <param name="i">The zero-based index of the element to retrieve. Must be within the bounds of the collection.</param>
    /// <returns>The element of type T at the specified index.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Read(int i)
    {
        // JIT optimizes this away when TContext is NullContext (Dead Code Elimination)
        if (typeof(TContext) != typeof(NullContext))
        {
            _context.OnIndexRead(i, _bufferId);
        }
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
        // JIT optimizes this away when TContext is NullContext (Dead Code Elimination)
        if (typeof(TContext) != typeof(NullContext))
        {
            _context.OnIndexWrite(i, _bufferId, value);
        }
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
        // JIT optimizes this entire path when TContext is NullContext
        if (typeof(TContext) != typeof(NullContext))
        {
            var a = Read(i);
            var b = Read(j);
            var result = _comparer.Compare(a, b);
            _context.OnCompare(i, j, result, _bufferId, _bufferId);
            return result;
        }
        else
        {
            // Fast path: direct array access without tracking
            return _comparer.Compare(_span[i], _span[j]);
        }
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
        // JIT optimizes this entire path when TContext is NullContext
        if (typeof(TContext) != typeof(NullContext))
        {
            var a = Read(i);
            var result = _comparer.Compare(a, value);
            _context.OnCompare(i, -1, result, _bufferId, -1);
            return result;
        }
        else
        {
            // Fast path: direct array access without tracking
            return _comparer.Compare(_span[i], value);
        }
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
        // JIT optimizes this entire path when TContext is NullContext
        if (typeof(TContext) != typeof(NullContext))
        {
            var b = Read(i);
            var result = _comparer.Compare(value, b);
            _context.OnCompare(-1, i, result, -1, _bufferId);
            return result;
        }
        else
        {
            // Fast path: direct array access without tracking
            return _comparer.Compare(value, _span[i]);
        }
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
        // JIT optimizes this entire path when TContext is NullContext
        if (typeof(TContext) != typeof(NullContext))
        {
            var result = _comparer.Compare(a, b);
            _context.OnCompare(-1, -1, result, -1, -1);
            return result;
        }
        else
        {
            // Fast path: direct comparison without tracking
            return _comparer.Compare(a, b);
        }
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
        // JIT optimizes this away when TContext is NullContext (Dead Code Elimination)
        if (typeof(TContext) != typeof(NullContext))
        {
            _context.OnSwap(i, j, _bufferId);
        }
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
    public void CopyTo(int sourceIndex, SortSpan<T, TComparer, TContext> destination, int destinationIndex, int length)
    {
        // JIT optimizes this away when TContext is NullContext (Dead Code Elimination)
        if (typeof(TContext) != typeof(NullContext))
        {
            var values = new object?[length];
            for (int i = 0; i < length; i++)
            {
                values[i] = _span[sourceIndex + i];
            }
            _context.OnRangeCopy(sourceIndex, destinationIndex, length, _bufferId, destination.BufferId, values);
        }
        _span.Slice(sourceIndex, length).CopyTo(destination._span.Slice(destinationIndex, length));
    }

    /// <summary>
    /// Returns a new SortSpan that wraps a slice of this span with a different buffer identifier.
    /// </summary>
    /// <param name="start">The starting index of the slice.</param>
    /// <param name="length">The number of elements in the slice.</param>
    /// <param name="bufferId">The buffer identifier for the new SortSpan.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SortSpan<T, TComparer, TContext> Slice(int start, int length, int bufferId)
        => new SortSpan<T, TComparer, TContext>(_span.Slice(start, length), _context, _comparer, bufferId);

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
        // JIT optimizes this away when TContext is NullContext (Dead Code Elimination)
        if (typeof(TContext) != typeof(NullContext))
        {
            var values = new object?[length];
            for (int i = 0; i < length; i++)
            {
                values[i] = _span[sourceIndex + i];
            }
            _context.OnRangeCopy(sourceIndex, destinationIndex, length, _bufferId, -1, values);
        }
        _span.Slice(sourceIndex, length).CopyTo(destination.Slice(destinationIndex, length));
    }
}
