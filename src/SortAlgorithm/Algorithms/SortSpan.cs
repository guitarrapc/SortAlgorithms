using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
    private readonly ref T _ref;
    private readonly TContext _context;
    private readonly TComparer _comparer;
    private readonly int _bufferId;
    private readonly int _offset;

    /// <summary>
    /// Initializes a new instance of SortSpan with the specified span, context, and comparer.
    /// </summary>
    /// <param name="span">The span to wrap</param>
    /// <param name="context">The context for tracking operations</param>
    /// <param name="comparer">The comparer to use for element comparisons</param>
    /// <param name="bufferId">Buffer identifier (0 = main array, 1+ = auxiliary buffers). Default is 0.</param>
    /// <param name="offset">Absolute offset in the original buffer for context-reported indices. Default is 0.</param>
    public SortSpan(Span<T> span, TContext context, TComparer comparer, int bufferId, int offset = 0)
    {
        _span = span;
        _ref = ref MemoryMarshal.GetReference(span);
        _context = context;
        _comparer = comparer;
        _bufferId = bufferId;
        _offset = offset;
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
            _context.OnIndexRead(_offset + i, _bufferId);
        }
#if DEBUG
        return _span[i]; // TEMP: bounds check for debugging OOB
#else
        return Unsafe.Add(ref _ref, i);
#endif
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
            _context.OnIndexWrite(_offset + i, _bufferId, value);
        }
#if DEBUG
        _span[i] = value; // TEMP: bounds check for debugging OOB
#else
        Unsafe.Add(ref _ref, i) = value;
#endif
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
            _context.OnCompare(_offset + i, _offset + j, result, _bufferId, _bufferId);
            return result;
        }
        else
        {
#if DEBUG
            // TEMP: bounds check for debugging OOB
            return _comparer.Compare(_span[i], _span[j]);
#else
            // Fast path: bounds-check-free access without tracking
            return _comparer.Compare(Unsafe.Add(ref _ref, i), Unsafe.Add(ref _ref, j));
#endif
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
            _context.OnCompare(_offset + i, -1, result, _bufferId, -1);
            return result;
        }
        else
        {
#if DEBUG
            // TEMP: bounds check for debugging OOB
            return _comparer.Compare(_span[i], value);
#else
            // Fast path: bounds-check-free access without tracking
            return _comparer.Compare(Unsafe.Add(ref _ref, i), value);
#endif
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
            _context.OnCompare(-1, _offset + i, result, -1, _bufferId);
            return result;
        }
        else
        {
#if DEBUG
            // TEMP: bounds check for debugging OOB
            return _comparer.Compare(value, _span[i]);
#else
            // Fast path: bounds-check-free access without tracking
            return _comparer.Compare(value, Unsafe.Add(ref _ref, i));
#endif
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
    /// Returns true if <paramref name="a"/> is strictly less than <paramref name="b"/>.
    /// For <see cref="ComparableComparer{T}"/> with primitive <typeparamref name="T"/> and <see cref="NullContext"/>,
    /// uses a direct primitive comparison, avoiding IComparer dispatch.
    /// For non-primitive types or instrumented contexts, delegates to the comparer.
    /// Note: for <c>float</c>, <c>double</c>, and <c>Half</c>, the specialized path uses IEEE 754 <c>&lt;</c>,
    /// which treats NaN as unordered (always returns false), unlike <see cref="IComparable{T}.CompareTo"/>.
    /// Callers that may encounter NaN should handle it separately (e.g., with a NaN pre-pass).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsLessThan(T a, T b)
    {
        if (typeof(TContext) != typeof(NullContext))
        {
            var result = _comparer.Compare(a, b);
            _context.OnCompare(-1, -1, result, -1, -1);
            return result < 0;
        }
#if DEBUG
        return _comparer.Compare(a, b) < 0;
#else
        // For value type TComparer the JIT constant-folds this 'is' check at specialization time:
        // true when TComparer is ComparableComparer<T>, false otherwise — no runtime overhead.
        // The guard is required so that custom comparers (e.g. reverse order) are never bypassed.
        if (_comparer is IComparableComparer)
        {
            if (typeof(T) == typeof(byte)) return Unsafe.As<T, byte>(ref a) < Unsafe.As<T, byte>(ref b);
            if (typeof(T) == typeof(sbyte)) return Unsafe.As<T, sbyte>(ref a) < Unsafe.As<T, sbyte>(ref b);
            if (typeof(T) == typeof(ushort)) return Unsafe.As<T, ushort>(ref a) < Unsafe.As<T, ushort>(ref b);
            if (typeof(T) == typeof(short)) return Unsafe.As<T, short>(ref a) < Unsafe.As<T, short>(ref b);
            if (typeof(T) == typeof(uint)) return Unsafe.As<T, uint>(ref a) < Unsafe.As<T, uint>(ref b);
            if (typeof(T) == typeof(int)) return Unsafe.As<T, int>(ref a) < Unsafe.As<T, int>(ref b);
            if (typeof(T) == typeof(ulong)) return Unsafe.As<T, ulong>(ref a) < Unsafe.As<T, ulong>(ref b);
            if (typeof(T) == typeof(long)) return Unsafe.As<T, long>(ref a) < Unsafe.As<T, long>(ref b);
            if (typeof(T) == typeof(nuint)) return Unsafe.As<T, nuint>(ref a) < Unsafe.As<T, nuint>(ref b);
            if (typeof(T) == typeof(nint)) return Unsafe.As<T, nint>(ref a) < Unsafe.As<T, nint>(ref b);
            if (typeof(T) == typeof(float)) return Unsafe.As<T, float>(ref a) < Unsafe.As<T, float>(ref b);
            if (typeof(T) == typeof(double)) return Unsafe.As<T, double>(ref a) < Unsafe.As<T, double>(ref b);
            if (typeof(T) == typeof(Half)) return Unsafe.As<T, Half>(ref a) < Unsafe.As<T, Half>(ref b);
        }
        return _comparer.Compare(a, b) < 0;
#endif
    }

    /// <summary>
    /// Returns true if <paramref name="a"/> is less than or equal to <paramref name="b"/>.
    /// For <see cref="ComparableComparer{T}"/> with primitive <typeparamref name="T"/> and <see cref="NullContext"/>,
    /// uses a direct primitive comparison, avoiding IComparer dispatch.
    /// For non-primitive types or instrumented contexts, delegates to the comparer.
    /// Note: for <c>float</c>, <c>double</c>, and <c>Half</c>, the specialized path uses IEEE 754 <c>&lt;=</c>,
    /// which treats NaN as unordered (always returns false), unlike <see cref="IComparable{T}.CompareTo"/>.
    /// Callers that may encounter NaN should handle it separately (e.g., with a NaN pre-pass).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsLessOrEqual(T a, T b)
    {
        if (typeof(TContext) != typeof(NullContext))
        {
            var result = _comparer.Compare(a, b);
            _context.OnCompare(-1, -1, result, -1, -1);
            return result <= 0;
        }
#if DEBUG
        return _comparer.Compare(a, b) <= 0;
#else
        // For value type TComparer the JIT constant-folds this 'is' check at specialization time:
        // true when TComparer is ComparableComparer<T>, false otherwise — no runtime overhead.
        // The guard is required so that custom comparers (e.g. reverse order) are never bypassed.
        if (_comparer is IComparableComparer)
        {
            if (typeof(T) == typeof(byte)) return Unsafe.As<T, byte>(ref a) <= Unsafe.As<T, byte>(ref b);
            if (typeof(T) == typeof(sbyte)) return Unsafe.As<T, sbyte>(ref a) <= Unsafe.As<T, sbyte>(ref b);
            if (typeof(T) == typeof(ushort)) return Unsafe.As<T, ushort>(ref a) <= Unsafe.As<T, ushort>(ref b);
            if (typeof(T) == typeof(short)) return Unsafe.As<T, short>(ref a) <= Unsafe.As<T, short>(ref b);
            if (typeof(T) == typeof(uint)) return Unsafe.As<T, uint>(ref a) <= Unsafe.As<T, uint>(ref b);
            if (typeof(T) == typeof(int)) return Unsafe.As<T, int>(ref a) <= Unsafe.As<T, int>(ref b);
            if (typeof(T) == typeof(ulong)) return Unsafe.As<T, ulong>(ref a) <= Unsafe.As<T, ulong>(ref b);
            if (typeof(T) == typeof(long)) return Unsafe.As<T, long>(ref a) <= Unsafe.As<T, long>(ref b);
            if (typeof(T) == typeof(nuint)) return Unsafe.As<T, nuint>(ref a) <= Unsafe.As<T, nuint>(ref b);
            if (typeof(T) == typeof(nint)) return Unsafe.As<T, nint>(ref a) <= Unsafe.As<T, nint>(ref b);
            if (typeof(T) == typeof(float)) return Unsafe.As<T, float>(ref a) <= Unsafe.As<T, float>(ref b);
            if (typeof(T) == typeof(double)) return Unsafe.As<T, double>(ref a) <= Unsafe.As<T, double>(ref b);
            if (typeof(T) == typeof(Half)) return Unsafe.As<T, Half>(ref a) <= Unsafe.As<T, Half>(ref b);
        }
        return _comparer.Compare(a, b) <= 0;
#endif
    }

    /// <summary>
    /// Returns true if the element at index <paramref name="i"/> is strictly less than the element at index <paramref name="j"/>.
    /// Equivalent to <c>Compare(i, j) &lt; 0</c> but avoids the <c>CompareTo</c> → <c>int</c> → <c>&lt; 0</c> chain
    /// for primitive types by returning <c>bool</c> directly via the same specialization used by <see cref="IsLessThan"/>.
    /// Named "At" to disambiguate from the value-based <see cref="IsLessThan"/> overload when <typeparamref name="T"/> is <c>int</c>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsLessAt(int i, int j)
    {
        if (typeof(TContext) != typeof(NullContext))
        {
            var a = _span[i];
            var b = _span[j];
            var result = _comparer.Compare(a, b);
            _context.OnIndexRead(_offset + i, _bufferId);
            _context.OnIndexRead(_offset + j, _bufferId);
            _context.OnCompare(_offset + i, _offset + j, result, _bufferId, _bufferId);
            return result < 0;
        }
#if DEBUG
        return _comparer.Compare(_span[i], _span[j]) < 0; // TEMP: bounds check for debugging OOB
#else
        if (_comparer is IComparableComparer)
        {
            ref T ai = ref Unsafe.Add(ref _ref, i);
            ref T aj = ref Unsafe.Add(ref _ref, j);
            if (typeof(T) == typeof(byte)) return Unsafe.As<T, byte>(ref ai) < Unsafe.As<T, byte>(ref aj);
            if (typeof(T) == typeof(sbyte)) return Unsafe.As<T, sbyte>(ref ai) < Unsafe.As<T, sbyte>(ref aj);
            if (typeof(T) == typeof(ushort)) return Unsafe.As<T, ushort>(ref ai) < Unsafe.As<T, ushort>(ref aj);
            if (typeof(T) == typeof(short)) return Unsafe.As<T, short>(ref ai) < Unsafe.As<T, short>(ref aj);
            if (typeof(T) == typeof(uint)) return Unsafe.As<T, uint>(ref ai) < Unsafe.As<T, uint>(ref aj);
            if (typeof(T) == typeof(int)) return Unsafe.As<T, int>(ref ai) < Unsafe.As<T, int>(ref aj);
            if (typeof(T) == typeof(ulong)) return Unsafe.As<T, ulong>(ref ai) < Unsafe.As<T, ulong>(ref aj);
            if (typeof(T) == typeof(long)) return Unsafe.As<T, long>(ref ai) < Unsafe.As<T, long>(ref aj);
            if (typeof(T) == typeof(nuint)) return Unsafe.As<T, nuint>(ref ai) < Unsafe.As<T, nuint>(ref aj);
            if (typeof(T) == typeof(nint)) return Unsafe.As<T, nint>(ref ai) < Unsafe.As<T, nint>(ref aj);
            if (typeof(T) == typeof(float)) return Unsafe.As<T, float>(ref ai) < Unsafe.As<T, float>(ref aj);
            if (typeof(T) == typeof(double)) return Unsafe.As<T, double>(ref ai) < Unsafe.As<T, double>(ref aj);
            if (typeof(T) == typeof(Half)) return Unsafe.As<T, Half>(ref ai) < Unsafe.As<T, Half>(ref aj);
        }
        return _comparer.Compare(Unsafe.Add(ref _ref, i), Unsafe.Add(ref _ref, j)) < 0;
#endif
    }

    /// <summary>
    /// Returns true if the element at index <paramref name="i"/> is less than or equal to the element at index <paramref name="j"/>.
    /// Equivalent to <c>Compare(i, j) &lt;= 0</c> but avoids the <c>CompareTo</c> → <c>int</c> → <c>&lt;= 0</c> chain
    /// for primitive types by returning <c>bool</c> directly via the same specialization used by <see cref="IsLessOrEqual"/>.
    /// Named "At" to disambiguate from the value-based <see cref="IsLessOrEqual"/> overload when <typeparamref name="T"/> is <c>int</c>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsLessOrEqualAt(int i, int j)
    {
        if (typeof(TContext) != typeof(NullContext))
        {
            var a = _span[i];
            var b = _span[j];
            var result = _comparer.Compare(a, b);
            _context.OnIndexRead(_offset + i, _bufferId);
            _context.OnIndexRead(_offset + j, _bufferId);
            _context.OnCompare(_offset + i, _offset + j, result, _bufferId, _bufferId);
            return result <= 0;
        }
#if DEBUG
        return _comparer.Compare(_span[i], _span[j]) <= 0; // TEMP: bounds check for debugging OOB
#else
        if (_comparer is IComparableComparer)
        {
            ref T ai = ref Unsafe.Add(ref _ref, i);
            ref T aj = ref Unsafe.Add(ref _ref, j);
            if (typeof(T) == typeof(byte)) return Unsafe.As<T, byte>(ref ai) <= Unsafe.As<T, byte>(ref aj);
            if (typeof(T) == typeof(sbyte)) return Unsafe.As<T, sbyte>(ref ai) <= Unsafe.As<T, sbyte>(ref aj);
            if (typeof(T) == typeof(ushort)) return Unsafe.As<T, ushort>(ref ai) <= Unsafe.As<T, ushort>(ref aj);
            if (typeof(T) == typeof(short)) return Unsafe.As<T, short>(ref ai) <= Unsafe.As<T, short>(ref aj);
            if (typeof(T) == typeof(uint)) return Unsafe.As<T, uint>(ref ai) <= Unsafe.As<T, uint>(ref aj);
            if (typeof(T) == typeof(int)) return Unsafe.As<T, int>(ref ai) <= Unsafe.As<T, int>(ref aj);
            if (typeof(T) == typeof(ulong)) return Unsafe.As<T, ulong>(ref ai) <= Unsafe.As<T, ulong>(ref aj);
            if (typeof(T) == typeof(long)) return Unsafe.As<T, long>(ref ai) <= Unsafe.As<T, long>(ref aj);
            if (typeof(T) == typeof(nuint)) return Unsafe.As<T, nuint>(ref ai) <= Unsafe.As<T, nuint>(ref aj);
            if (typeof(T) == typeof(nint)) return Unsafe.As<T, nint>(ref ai) <= Unsafe.As<T, nint>(ref aj);
            if (typeof(T) == typeof(float)) return Unsafe.As<T, float>(ref ai) <= Unsafe.As<T, float>(ref aj);
            if (typeof(T) == typeof(double)) return Unsafe.As<T, double>(ref ai) <= Unsafe.As<T, double>(ref aj);
            if (typeof(T) == typeof(Half)) return Unsafe.As<T, Half>(ref ai) <= Unsafe.As<T, Half>(ref aj);
        }
        return _comparer.Compare(Unsafe.Add(ref _ref, i), Unsafe.Add(ref _ref, j)) <= 0;
#endif
    }

    /// <summary>
    /// Returns true if <paramref name="a"/> is strictly greater than <paramref name="b"/>.
    /// Equivalent to <c>Compare(a, b) &gt; 0</c> and logically identical to <c>!IsLessOrEqual(a, b)</c>,
    /// but provided for readability at call sites that naturally express "greater than" semantics.
    /// For <see cref="ComparableComparer{T}"/> with primitive <typeparamref name="T"/> and <see cref="NullContext"/>,
    /// uses a direct primitive comparison, avoiding IComparer dispatch.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsGreaterThan(T a, T b)
    {
        if (typeof(TContext) != typeof(NullContext))
        {
            var result = _comparer.Compare(a, b);
            _context.OnCompare(-1, -1, result, -1, -1);
            return result > 0;
        }
#if DEBUG
        return _comparer.Compare(a, b) > 0; // TEMP: bounds check for debugging OOB
#else
        if (_comparer is IComparableComparer)
        {
            if (typeof(T) == typeof(byte)) return Unsafe.As<T, byte>(ref a) > Unsafe.As<T, byte>(ref b);
            if (typeof(T) == typeof(sbyte)) return Unsafe.As<T, sbyte>(ref a) > Unsafe.As<T, sbyte>(ref b);
            if (typeof(T) == typeof(ushort)) return Unsafe.As<T, ushort>(ref a) > Unsafe.As<T, ushort>(ref b);
            if (typeof(T) == typeof(short)) return Unsafe.As<T, short>(ref a) > Unsafe.As<T, short>(ref b);
            if (typeof(T) == typeof(uint)) return Unsafe.As<T, uint>(ref a) > Unsafe.As<T, uint>(ref b);
            if (typeof(T) == typeof(int)) return Unsafe.As<T, int>(ref a) > Unsafe.As<T, int>(ref b);
            if (typeof(T) == typeof(ulong)) return Unsafe.As<T, ulong>(ref a) > Unsafe.As<T, ulong>(ref b);
            if (typeof(T) == typeof(long)) return Unsafe.As<T, long>(ref a) > Unsafe.As<T, long>(ref b);
            if (typeof(T) == typeof(nuint)) return Unsafe.As<T, nuint>(ref a) > Unsafe.As<T, nuint>(ref b);
            if (typeof(T) == typeof(nint)) return Unsafe.As<T, nint>(ref a) > Unsafe.As<T, nint>(ref b);
            if (typeof(T) == typeof(float)) return Unsafe.As<T, float>(ref a) > Unsafe.As<T, float>(ref b);
            if (typeof(T) == typeof(double)) return Unsafe.As<T, double>(ref a) > Unsafe.As<T, double>(ref b);
            if (typeof(T) == typeof(Half)) return Unsafe.As<T, Half>(ref a) > Unsafe.As<T, Half>(ref b);
        }
        return _comparer.Compare(a, b) > 0;
#endif
    }

    /// <summary>
    /// Returns true if <paramref name="a"/> is greater than or equal to <paramref name="b"/>.
    /// Equivalent to <c>Compare(a, b) &gt;= 0</c> and logically identical to <c>!IsLessThan(a, b)</c>,
    /// but provided for readability at call sites that naturally express "greater or equal" semantics.
    /// For <see cref="ComparableComparer{T}"/> with primitive <typeparamref name="T"/> and <see cref="NullContext"/>,
    /// uses a direct primitive comparison, avoiding IComparer dispatch.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsGreaterOrEqual(T a, T b)
    {
        if (typeof(TContext) != typeof(NullContext))
        {
            var result = _comparer.Compare(a, b);
            _context.OnCompare(-1, -1, result, -1, -1);
            return result >= 0;
        }
#if DEBUG
        return _comparer.Compare(a, b) >= 0; // TEMP: bounds check for debugging OOB
#else
        if (_comparer is IComparableComparer)
        {
            if (typeof(T) == typeof(byte)) return Unsafe.As<T, byte>(ref a) >= Unsafe.As<T, byte>(ref b);
            if (typeof(T) == typeof(sbyte)) return Unsafe.As<T, sbyte>(ref a) >= Unsafe.As<T, sbyte>(ref b);
            if (typeof(T) == typeof(ushort)) return Unsafe.As<T, ushort>(ref a) >= Unsafe.As<T, ushort>(ref b);
            if (typeof(T) == typeof(short)) return Unsafe.As<T, short>(ref a) >= Unsafe.As<T, short>(ref b);
            if (typeof(T) == typeof(uint)) return Unsafe.As<T, uint>(ref a) >= Unsafe.As<T, uint>(ref b);
            if (typeof(T) == typeof(int)) return Unsafe.As<T, int>(ref a) >= Unsafe.As<T, int>(ref b);
            if (typeof(T) == typeof(ulong)) return Unsafe.As<T, ulong>(ref a) >= Unsafe.As<T, ulong>(ref b);
            if (typeof(T) == typeof(long)) return Unsafe.As<T, long>(ref a) >= Unsafe.As<T, long>(ref b);
            if (typeof(T) == typeof(nuint)) return Unsafe.As<T, nuint>(ref a) >= Unsafe.As<T, nuint>(ref b);
            if (typeof(T) == typeof(nint)) return Unsafe.As<T, nint>(ref a) >= Unsafe.As<T, nint>(ref b);
            if (typeof(T) == typeof(float)) return Unsafe.As<T, float>(ref a) >= Unsafe.As<T, float>(ref b);
            if (typeof(T) == typeof(double)) return Unsafe.As<T, double>(ref a) >= Unsafe.As<T, double>(ref b);
            if (typeof(T) == typeof(Half)) return Unsafe.As<T, Half>(ref a) >= Unsafe.As<T, Half>(ref b);
        }
        return _comparer.Compare(a, b) >= 0;
#endif
    }

    /// <summary>
    /// Returns true if the element at index <paramref name="i"/> is strictly greater than the element at index <paramref name="j"/>.
    /// Equivalent to <c>Compare(i, j) &gt; 0</c> and logically identical to <c>!IsLessOrEqualAt(i, j)</c>,
    /// but provided for readability at call sites that naturally express "greater than" semantics.
    /// Named "At" to disambiguate from the value-based <see cref="IsGreaterThan"/> overload when <typeparamref name="T"/> is <c>int</c>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsGreaterAt(int i, int j)
    {
        if (typeof(TContext) != typeof(NullContext))
        {
            var a = _span[i];
            var b = _span[j];
            var result = _comparer.Compare(a, b);
            _context.OnIndexRead(_offset + i, _bufferId);
            _context.OnIndexRead(_offset + j, _bufferId);
            _context.OnCompare(_offset + i, _offset + j, result, _bufferId, _bufferId);
            return result > 0;
        }
#if DEBUG
        return _comparer.Compare(_span[i], _span[j]) > 0; // TEMP: bounds check for debugging OOB
#else
        if (_comparer is IComparableComparer)
        {
            ref T ai = ref Unsafe.Add(ref _ref, i);
            ref T aj = ref Unsafe.Add(ref _ref, j);
            if (typeof(T) == typeof(byte)) return Unsafe.As<T, byte>(ref ai) > Unsafe.As<T, byte>(ref aj);
            if (typeof(T) == typeof(sbyte)) return Unsafe.As<T, sbyte>(ref ai) > Unsafe.As<T, sbyte>(ref aj);
            if (typeof(T) == typeof(ushort)) return Unsafe.As<T, ushort>(ref ai) > Unsafe.As<T, ushort>(ref aj);
            if (typeof(T) == typeof(short)) return Unsafe.As<T, short>(ref ai) > Unsafe.As<T, short>(ref aj);
            if (typeof(T) == typeof(uint)) return Unsafe.As<T, uint>(ref ai) > Unsafe.As<T, uint>(ref aj);
            if (typeof(T) == typeof(int)) return Unsafe.As<T, int>(ref ai) > Unsafe.As<T, int>(ref aj);
            if (typeof(T) == typeof(ulong)) return Unsafe.As<T, ulong>(ref ai) > Unsafe.As<T, ulong>(ref aj);
            if (typeof(T) == typeof(long)) return Unsafe.As<T, long>(ref ai) > Unsafe.As<T, long>(ref aj);
            if (typeof(T) == typeof(nuint)) return Unsafe.As<T, nuint>(ref ai) > Unsafe.As<T, nuint>(ref aj);
            if (typeof(T) == typeof(nint)) return Unsafe.As<T, nint>(ref ai) > Unsafe.As<T, nint>(ref aj);
            if (typeof(T) == typeof(float)) return Unsafe.As<T, float>(ref ai) > Unsafe.As<T, float>(ref aj);
            if (typeof(T) == typeof(double)) return Unsafe.As<T, double>(ref ai) > Unsafe.As<T, double>(ref aj);
            if (typeof(T) == typeof(Half)) return Unsafe.As<T, Half>(ref ai) > Unsafe.As<T, Half>(ref aj);
        }
        return _comparer.Compare(Unsafe.Add(ref _ref, i), Unsafe.Add(ref _ref, j)) > 0;
#endif
    }

    /// <summary>
    /// Returns true if the element at index <paramref name="i"/> is greater than or equal to the element at index <paramref name="j"/>.
    /// Equivalent to <c>Compare(i, j) &gt;= 0</c> and logically identical to <c>!IsLessAt(i, j)</c>,
    /// but provided for readability at call sites that naturally express "greater or equal" semantics.
    /// Named "At" to disambiguate from the value-based <see cref="IsGreaterOrEqual"/> overload when <typeparamref name="T"/> is <c>int</c>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsGreaterOrEqualAt(int i, int j)
    {
        if (typeof(TContext) != typeof(NullContext))
        {
            var a = _span[i];
            var b = _span[j];
            var result = _comparer.Compare(a, b);
            _context.OnIndexRead(_offset + i, _bufferId);
            _context.OnIndexRead(_offset + j, _bufferId);
            _context.OnCompare(_offset + i, _offset + j, result, _bufferId, _bufferId);
            return result >= 0;
        }
#if DEBUG
        return _comparer.Compare(_span[i], _span[j]) >= 0; // TEMP: bounds check for debugging OOB
#else
        if (_comparer is IComparableComparer)
        {
            ref T ai = ref Unsafe.Add(ref _ref, i);
            ref T aj = ref Unsafe.Add(ref _ref, j);
            if (typeof(T) == typeof(byte)) return Unsafe.As<T, byte>(ref ai) >= Unsafe.As<T, byte>(ref aj);
            if (typeof(T) == typeof(sbyte)) return Unsafe.As<T, sbyte>(ref ai) >= Unsafe.As<T, sbyte>(ref aj);
            if (typeof(T) == typeof(ushort)) return Unsafe.As<T, ushort>(ref ai) >= Unsafe.As<T, ushort>(ref aj);
            if (typeof(T) == typeof(short)) return Unsafe.As<T, short>(ref ai) >= Unsafe.As<T, short>(ref aj);
            if (typeof(T) == typeof(uint)) return Unsafe.As<T, uint>(ref ai) >= Unsafe.As<T, uint>(ref aj);
            if (typeof(T) == typeof(int)) return Unsafe.As<T, int>(ref ai) >= Unsafe.As<T, int>(ref aj);
            if (typeof(T) == typeof(ulong)) return Unsafe.As<T, ulong>(ref ai) >= Unsafe.As<T, ulong>(ref aj);
            if (typeof(T) == typeof(long)) return Unsafe.As<T, long>(ref ai) >= Unsafe.As<T, long>(ref aj);
            if (typeof(T) == typeof(nuint)) return Unsafe.As<T, nuint>(ref ai) >= Unsafe.As<T, nuint>(ref aj);
            if (typeof(T) == typeof(nint)) return Unsafe.As<T, nint>(ref ai) >= Unsafe.As<T, nint>(ref aj);
            if (typeof(T) == typeof(float)) return Unsafe.As<T, float>(ref ai) >= Unsafe.As<T, float>(ref aj);
            if (typeof(T) == typeof(double)) return Unsafe.As<T, double>(ref ai) >= Unsafe.As<T, double>(ref aj);
            if (typeof(T) == typeof(Half)) return Unsafe.As<T, Half>(ref ai) >= Unsafe.As<T, Half>(ref aj);
        }
        return _comparer.Compare(Unsafe.Add(ref _ref, i), Unsafe.Add(ref _ref, j)) >= 0;
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
        // JIT optimizes this away when TContext is NullContext (Dead Code Elimination)
        if (typeof(TContext) != typeof(NullContext))
        {
            _context.OnSwap(_offset + i, _offset + j, _bufferId);
        }
#if DEBUG
        (_span[i], _span[j]) = (_span[j], _span[i]);
#else
        ref T si = ref Unsafe.Add(ref _ref, i);
        ref T sj = ref Unsafe.Add(ref _ref, j);
        (si, sj) = (sj, si);
#endif
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
            _context.OnRangeCopy(_offset + sourceIndex, destination._offset + destinationIndex, length, _bufferId, destination.BufferId, values);
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
        => new SortSpan<T, TComparer, TContext>(_span.Slice(start, length), _context, _comparer, bufferId, _offset + start);

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
            _context.OnRangeCopy(_offset + sourceIndex, destinationIndex, length, _bufferId, -1, values);
        }
        _span.Slice(sourceIndex, length).CopyTo(destination.Slice(destinationIndex, length));
    }
}
