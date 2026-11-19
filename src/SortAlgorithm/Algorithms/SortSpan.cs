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
    /// Gets the absolute offset of this span in the original buffer.
    /// Used by algorithms to compute absolute indices for OnPhase/OnRole calls.
    /// </summary>
    public int Offset => _offset;

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
    /// Whether comparisons use the natural order of one of the primitive types that this span compares directly. That order is a
    /// consistent total order. The JIT folds the value to a constant for each instantiation.
    /// </summary>
    public bool UsesNaturalPrimitiveOrder
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => typeof(TComparer).IsValueType && default(TComparer) is IComparableComparer &&
            (typeof(T) == typeof(byte) || typeof(T) == typeof(sbyte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(short) ||
             typeof(T) == typeof(uint) || typeof(T) == typeof(int) || typeof(T) == typeof(ulong) || typeof(T) == typeof(long) ||
             typeof(T) == typeof(nuint) || typeof(T) == typeof(nint) || typeof(T) == typeof(float) || typeof(T) == typeof(double) ||
             typeof(T) == typeof(Half));
    }

    /// <summary>
    /// Exposes the underlying span without any operation tracking.
    /// Only for NullContext-gated fast paths (e.g. SIMD kernels) where no observer exists.
    /// The getter enforces the contract: any access with an observing <typeparamref name="TContext"/>
    /// throws, so element operations can never be silently lost. The typeof check is a JIT
    /// constant per instantiation — for NullContext the guard folds away entirely.
    /// </summary>
    internal Span<T> RawSpan
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (typeof(TContext) != typeof(NullContext))
            {
                ThrowRawSpanRequiresNullContext();
            }
            return _span;
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowRawSpanRequiresNullContext()
        => throw new InvalidOperationException(
            $"{nameof(RawSpan)} bypasses operation tracking and may only be used when TContext is {nameof(NullContext)}; " +
            "an observing context would silently lose element operations.");

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
        // (nint)(uint) zero-extends the index (non-negative by contract).
        // Unsafe.Add(ref, int) sign-extends, emitting a movsxd per access; the unsigned cast
        // lets the JIT reuse the implicitly zero-extended 32-bit register in the address mode.
        return Unsafe.Add(ref _ref, (nint)(uint)i);
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
        Unsafe.Add(ref _ref, (nint)(uint)i) = value;
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
            return _comparer.Compare(Unsafe.Add(ref _ref, (nint)(uint)i), Unsafe.Add(ref _ref, (nint)(uint)j));
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
            return _comparer.Compare(Unsafe.Add(ref _ref, (nint)(uint)i), value);
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
            return _comparer.Compare(value, Unsafe.Add(ref _ref, (nint)(uint)i));
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
    /// uses a direct comparison; floating-point predicates retain CompareTo ordering for NaN.
    /// For non-primitive types or instrumented contexts, delegates to the comparer.
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
            if (typeof(T) == typeof(float)) return Unsafe.As<T, float>(ref a) < Unsafe.As<T, float>(ref b) || float.IsNaN(Unsafe.As<T, float>(ref a)) && !float.IsNaN(Unsafe.As<T, float>(ref b));
            if (typeof(T) == typeof(double)) return Unsafe.As<T, double>(ref a) < Unsafe.As<T, double>(ref b) || double.IsNaN(Unsafe.As<T, double>(ref a)) && !double.IsNaN(Unsafe.As<T, double>(ref b));
            if (typeof(T) == typeof(Half)) return Unsafe.As<T, Half>(ref a) < Unsafe.As<T, Half>(ref b) || Half.IsNaN(Unsafe.As<T, Half>(ref a)) && !Half.IsNaN(Unsafe.As<T, Half>(ref b));
        }
        return _comparer.Compare(a, b) < 0;
#endif
    }

    /// <summary>
    /// Returns true if <paramref name="a"/> is less than or equal to <paramref name="b"/>.
    /// For <see cref="ComparableComparer{T}"/> with primitive <typeparamref name="T"/> and <see cref="NullContext"/>,
    /// uses a direct comparison; floating-point predicates retain CompareTo ordering for NaN.
    /// For non-primitive types or instrumented contexts, delegates to the comparer.
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
            if (typeof(T) == typeof(float)) return Unsafe.As<T, float>(ref a) <= Unsafe.As<T, float>(ref b) || float.IsNaN(Unsafe.As<T, float>(ref a));
            if (typeof(T) == typeof(double)) return Unsafe.As<T, double>(ref a) <= Unsafe.As<T, double>(ref b) || double.IsNaN(Unsafe.As<T, double>(ref a));
            if (typeof(T) == typeof(Half)) return Unsafe.As<T, Half>(ref a) <= Unsafe.As<T, Half>(ref b) || Half.IsNaN(Unsafe.As<T, Half>(ref a));
        }
        return _comparer.Compare(a, b) <= 0;
#endif
    }

    /// <summary>
    /// Returns true if <paramref name="a"/> is strictly greater than <paramref name="b"/>.
    /// Equivalent to <c>Compare(a, b) &gt; 0</c> and logically identical to <c>!IsLessOrEqual(a, b)</c>,
    /// but provided for readability at call sites that naturally express "greater than" semantics.
    /// For <see cref="ComparableComparer{T}"/> with primitive <typeparamref name="T"/> and <see cref="NullContext"/>,
    /// uses a direct comparison; floating-point predicates retain CompareTo ordering for NaN.
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
            if (typeof(T) == typeof(float)) return Unsafe.As<T, float>(ref a) > Unsafe.As<T, float>(ref b) || float.IsNaN(Unsafe.As<T, float>(ref b)) && !float.IsNaN(Unsafe.As<T, float>(ref a));
            if (typeof(T) == typeof(double)) return Unsafe.As<T, double>(ref a) > Unsafe.As<T, double>(ref b) || double.IsNaN(Unsafe.As<T, double>(ref b)) && !double.IsNaN(Unsafe.As<T, double>(ref a));
            if (typeof(T) == typeof(Half)) return Unsafe.As<T, Half>(ref a) > Unsafe.As<T, Half>(ref b) || Half.IsNaN(Unsafe.As<T, Half>(ref b)) && !Half.IsNaN(Unsafe.As<T, Half>(ref a));
        }
        return _comparer.Compare(a, b) > 0;
#endif
    }

    /// <summary>
    /// Returns true if <paramref name="a"/> is greater than or equal to <paramref name="b"/>.
    /// Equivalent to <c>Compare(a, b) &gt;= 0</c> and logically identical to <c>!IsLessThan(a, b)</c>,
    /// but provided for readability at call sites that naturally express "greater or equal" semantics.
    /// For <see cref="ComparableComparer{T}"/> with primitive <typeparamref name="T"/> and <see cref="NullContext"/>,
    /// uses a direct comparison; floating-point predicates retain CompareTo ordering for NaN.
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
            if (typeof(T) == typeof(float)) return Unsafe.As<T, float>(ref a) >= Unsafe.As<T, float>(ref b) || float.IsNaN(Unsafe.As<T, float>(ref b));
            if (typeof(T) == typeof(double)) return Unsafe.As<T, double>(ref a) >= Unsafe.As<T, double>(ref b) || double.IsNaN(Unsafe.As<T, double>(ref b));
            if (typeof(T) == typeof(Half)) return Unsafe.As<T, Half>(ref a) >= Unsafe.As<T, Half>(ref b) || Half.IsNaN(Unsafe.As<T, Half>(ref b));
        }
        return _comparer.Compare(a, b) >= 0;
#endif
    }

    // ------------------------------------------------------------------
    // Located comparisons.
    //
    // Every overload below answers the same question — how do two operands order — and differs only in
    // where each operand lives. Which reads happen, and which locations the comparison can name, follow
    // from the operand shape alone, so each shape announces itself once, in one Report* helper, and the
    // public overloads are the four orderings over it. Repeating the index and buffer arithmetic per
    // operator is what lets one of them drift, which is the failure this whole surface exists to avoid.
    //
    // The NullContext path never reaches a Report* helper: it reads directly and delegates to the
    // value-based overloads, which hold the primitive specialization. That keeps the specialization in
    // one place per operator rather than one per overload.
    //
    // Naming:
    //   Is<op>At        both operands are indices of this span
    //   IsElement<op>   the left operand is an index of this span, the right is a value
    //   IsValue<op>     the left operand is a value, the right is an index of this span
    //   Is<op>Across    both operands are indices, of this span and another
    // An `out` parameter means the span performs the read and hands the value back, for callers that
    // need it afterwards; the reported location is then the span's own and cannot be wrong.
    // ------------------------------------------------------------------

    /// <summary>Reads without announcing anything. Only for the NullContext path.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private T ReadUnobserved(int i)
    {
#if DEBUG
        return _span[i]; // TEMP: bounds check for debugging OOB
#else
        return Unsafe.Add(ref _ref, (nint)(uint)i);
#endif
    }

    // NetWasm 0.13 benefits from inlining the reporting helpers. Keep standard .NET
    // heuristics: forcing these helpers inline regressed the measured SortVivo API.
    /// <summary>Announces the reads and the comparison for two indices of this span.</summary>
#if NETWASM
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    private int ReportCompareAt(int i, int j, out T ai, out T aj)
    {
        ai = _span[i];
        aj = _span[j];
        var result = _comparer.Compare(ai, aj);
        _context.OnIndexRead(_offset + i, _bufferId);
        _context.OnIndexRead(_offset + j, _bufferId);
        _context.OnCompare(_offset + i, _offset + j, result, _bufferId, _bufferId);
        return result;
    }

    /// <summary>
    /// Announces the read and the comparison for an element of this span against a value that is not
    /// in any buffer, which is reported as -1.
    /// </summary>
#if NETWASM
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    private int ReportCompareElement(int i, T value, out T element)
    {
        element = _span[i];
        var result = _comparer.Compare(element, value);
        _context.OnIndexRead(_offset + i, _bufferId);
        _context.OnCompare(_offset + i, -1, result, _bufferId, -1);
        return result;
    }

    /// <summary>
    /// As <see cref="ReportCompareElement"/> with the operands the other way round, so a consumer sees
    /// the same sign the algorithm did.
    /// </summary>
#if NETWASM
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    private int ReportCompareValue(T value, int j, out T element)
    {
        element = _span[j];
        var result = _comparer.Compare(value, element);
        _context.OnIndexRead(_offset + j, _bufferId);
        _context.OnCompare(-1, _offset + j, result, -1, _bufferId);
        return result;
    }

    /// <summary>
    /// Announces the reads and the comparison for one index of this span and one of another. Both
    /// operands keep their own buffer identity, which is what an index alone cannot express.
    /// </summary>
#if NETWASM
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    private int ReportCompareAcross(int i, SortSpan<T, TComparer, TContext> other, int j, out T ai, out T bj)
    {
        ai = _span[i];
        bj = other._span[j];
        var result = _comparer.Compare(ai, bj);
        _context.OnIndexRead(_offset + i, _bufferId);
        _context.OnIndexRead(other._offset + j, other._bufferId);
        _context.OnCompare(_offset + i, other._offset + j, result, _bufferId, other._bufferId);
        return result;
    }

    /// <summary>
    /// Returns true if the element at index <paramref name="i"/> is strictly less than the element at index <paramref name="j"/>.
    /// Named "At" to disambiguate from the value-based <see cref="IsLessThan"/> overload when <typeparamref name="T"/> is <c>int</c>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsLessAt(int i, int j)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareAt(i, j, out _, out _) < 0;
        return IsLessThan(ReadUnobserved(i), ReadUnobserved(j));
    }

    /// <summary>
    /// Returns true if the element at index <paramref name="i"/> is less than or equal to the element at index <paramref name="j"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsLessOrEqualAt(int i, int j)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareAt(i, j, out _, out _) <= 0;
        return IsLessOrEqual(ReadUnobserved(i), ReadUnobserved(j));
    }

    /// <summary>
    /// Returns true if the element at index <paramref name="i"/> is strictly greater than the element at index <paramref name="j"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsGreaterAt(int i, int j)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareAt(i, j, out _, out _) > 0;
        return IsGreaterThan(ReadUnobserved(i), ReadUnobserved(j));
    }

    /// <summary>
    /// Returns true if the element at index <paramref name="i"/> is greater than or equal to the element at index <paramref name="j"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsGreaterOrEqualAt(int i, int j)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareAt(i, j, out _, out _) >= 0;
        return IsGreaterOrEqual(ReadUnobserved(i), ReadUnobserved(j));
    }

    /// <summary>
    /// Returns true if the element at index <paramref name="i"/> is strictly less than the element at
    /// index <paramref name="j"/>, handing back both elements.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsLessAt(int i, int j, out T ai, out T aj)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareAt(i, j, out ai, out aj) < 0;
        ai = ReadUnobserved(i);
        aj = ReadUnobserved(j);
        return IsLessThan(ai, aj);
    }

    /// <summary>
    /// Returns true if the element at index <paramref name="i"/> is strictly greater than the element at
    /// index <paramref name="j"/>, handing back both elements.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsGreaterAt(int i, int j, out T ai, out T aj)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareAt(i, j, out ai, out aj) > 0;
        ai = ReadUnobserved(i);
        aj = ReadUnobserved(j);
        return IsGreaterThan(ai, aj);
    }

    /// <summary>
    /// Returns true if the element at index <paramref name="i"/> is strictly less than <paramref name="value"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsElementLessThan(int i, T value)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareElement(i, value, out _) < 0;
        return IsLessThan(ReadUnobserved(i), value);
    }

    /// <summary>
    /// Returns true if the element at index <paramref name="i"/> is less than or equal to <paramref name="value"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsElementLessOrEqual(int i, T value)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareElement(i, value, out _) <= 0;
        return IsLessOrEqual(ReadUnobserved(i), value);
    }

    /// <summary>
    /// Returns true if the element at index <paramref name="i"/> is strictly greater than <paramref name="value"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsElementGreaterThan(int i, T value)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareElement(i, value, out _) > 0;
        return IsGreaterThan(ReadUnobserved(i), value);
    }

    /// <summary>
    /// Returns true if the element at index <paramref name="i"/> is greater than or equal to <paramref name="value"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsElementGreaterOrEqual(int i, T value)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareElement(i, value, out _) >= 0;
        return IsGreaterOrEqual(ReadUnobserved(i), value);
    }

    /// <summary>
    /// Returns true if the element at index <paramref name="i"/> is strictly less than <paramref name="value"/>,
    /// handing back the element that was read.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsElementLessThan(int i, T value, out T element)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareElement(i, value, out element) < 0;
        element = ReadUnobserved(i);
        return IsLessThan(element, value);
    }

    /// <summary>
    /// Returns true if the element at index <paramref name="i"/> is strictly greater than <paramref name="value"/>,
    /// handing back the element that was read.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsElementGreaterThan(int i, T value, out T element)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareElement(i, value, out element) > 0;
        element = ReadUnobserved(i);
        return IsGreaterThan(element, value);
    }

    /// <summary>
    /// Returns true if <paramref name="value"/> is strictly less than the element at index <paramref name="j"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValueLessThan(T value, int j)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareValue(value, j, out _) < 0;
        return IsLessThan(value, ReadUnobserved(j));
    }

    /// <summary>
    /// Returns true if <paramref name="value"/> is less than or equal to the element at index <paramref name="j"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValueLessOrEqual(T value, int j)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareValue(value, j, out _) <= 0;
        return IsLessOrEqual(value, ReadUnobserved(j));
    }

    /// <summary>
    /// Returns true if <paramref name="value"/> is strictly greater than the element at index <paramref name="j"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValueGreaterThan(T value, int j)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareValue(value, j, out _) > 0;
        return IsGreaterThan(value, ReadUnobserved(j));
    }

    /// <summary>
    /// Returns true if <paramref name="value"/> is greater than or equal to the element at index <paramref name="j"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValueGreaterOrEqual(T value, int j)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareValue(value, j, out _) >= 0;
        return IsGreaterOrEqual(value, ReadUnobserved(j));
    }

    /// <summary>
    /// Returns true if the element at index <paramref name="i"/> of this span is strictly less than the
    /// element at index <paramref name="j"/> of <paramref name="other"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsLessAcross(int i, SortSpan<T, TComparer, TContext> other, int j)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareAcross(i, other, j, out _, out _) < 0;
        return IsLessThan(ReadUnobserved(i), other.ReadUnobserved(j));
    }

    /// <summary>
    /// Returns true if the element at index <paramref name="i"/> of this span is less than or equal to the
    /// element at index <paramref name="j"/> of <paramref name="other"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsLessOrEqualAcross(int i, SortSpan<T, TComparer, TContext> other, int j)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareAcross(i, other, j, out _, out _) <= 0;
        return IsLessOrEqual(ReadUnobserved(i), other.ReadUnobserved(j));
    }

    /// <summary>
    /// Returns true if the element at index <paramref name="i"/> of this span is strictly greater than the
    /// element at index <paramref name="j"/> of <paramref name="other"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsGreaterAcross(int i, SortSpan<T, TComparer, TContext> other, int j)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareAcross(i, other, j, out _, out _) > 0;
        return IsGreaterThan(ReadUnobserved(i), other.ReadUnobserved(j));
    }

    /// <summary>
    /// Returns true if the element at index <paramref name="i"/> of this span is greater than or equal to
    /// the element at index <paramref name="j"/> of <paramref name="other"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsGreaterOrEqualAcross(int i, SortSpan<T, TComparer, TContext> other, int j)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareAcross(i, other, j, out _, out _) >= 0;
        return IsGreaterOrEqual(ReadUnobserved(i), other.ReadUnobserved(j));
    }

    /// <summary>
    /// Returns true if the element at index <paramref name="i"/> of this span is less than or equal to the
    /// element at index <paramref name="j"/> of <paramref name="other"/>, handing back both elements.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsLessOrEqualAcross(int i, SortSpan<T, TComparer, TContext> other, int j, out T ai, out T bj)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareAcross(i, other, j, out ai, out bj) <= 0;
        ai = ReadUnobserved(i);
        bj = other.ReadUnobserved(j);
        return IsLessOrEqual(ai, bj);
    }

    /// <summary>
    /// Returns true if the element at index <paramref name="i"/> of this span is greater than or equal to
    /// the element at index <paramref name="j"/> of <paramref name="other"/>, handing back both elements.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsGreaterOrEqualAcross(int i, SortSpan<T, TComparer, TContext> other, int j, out T ai, out T bj)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareAcross(i, other, j, out ai, out bj) >= 0;
        ai = ReadUnobserved(i);
        bj = other.ReadUnobserved(j);
        return IsGreaterOrEqual(ai, bj);
    }

    /// <summary>
    /// Returns true if the element at index <paramref name="i"/> of this span is strictly less than the
    /// element at index <paramref name="j"/> of <paramref name="other"/>, handing back both elements.
    /// A merge loop that writes back whichever operand won needs the value it just compared; taking it from
    /// here keeps the write off a second <see cref="Read"/>, which would announce the same element twice.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsLessAcross(int i, SortSpan<T, TComparer, TContext> other, int j, out T ai, out T bj)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareAcross(i, other, j, out ai, out bj) < 0;
        ai = ReadUnobserved(i);
        bj = other.ReadUnobserved(j);
        return IsLessThan(ai, bj);
    }

    /// <summary>
    /// Returns true if the element at index <paramref name="i"/> of this span is strictly greater than the
    /// element at index <paramref name="j"/> of <paramref name="other"/>, handing back both elements.
    /// A merge loop that writes back whichever operand won needs the value it just compared; taking it from
    /// here keeps the write off a second <see cref="Read"/>, which would announce the same element twice.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsGreaterAcross(int i, SortSpan<T, TComparer, TContext> other, int j, out T ai, out T bj)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareAcross(i, other, j, out ai, out bj) > 0;
        ai = ReadUnobserved(i);
        bj = other.ReadUnobserved(j);
        return IsGreaterThan(ai, bj);
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
        ref T si = ref Unsafe.Add(ref _ref, (nint)(uint)i);
        ref T sj = ref Unsafe.Add(ref _ref, (nint)(uint)j);
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
            // 値はソース範囲のスパンをそのまま渡す。object?[] を組むと 1 コピーにつき
            // 配列 1 本 + 要素数ぶんのボックス化が発生していた。
            _context.OnRangeCopy<T>(_offset + sourceIndex, destination._offset + destinationIndex, length, _bufferId, destination.BufferId, _span.Slice(sourceIndex, length));
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
            _context.OnRangeCopy<T>(_offset + sourceIndex, destinationIndex, length, _bufferId, -1, _span.Slice(sourceIndex, length));
        }
        _span.Slice(sourceIndex, length).CopyTo(destination.Slice(destinationIndex, length));
    }
    /// <summary>Classifies a cached value for the unobserved natural floating-point comparison path.</summary>
    /// <remarks>After clearing the sign bit, NaN encodings exceed positive infinity.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsNaturalFloatingNaN(T value)
    {
        if (typeof(TContext) == typeof(NullContext) && _comparer is IComparableComparer)
        {
            if (typeof(T) == typeof(float)) return (Unsafe.As<T, uint>(ref value) & 0x7fff_ffffU) > 0x7f80_0000U;
            if (typeof(T) == typeof(double)) return (Unsafe.As<T, ulong>(ref value) & 0x7fff_ffff_ffff_ffffUL) > 0x7ff0_0000_0000_0000UL;
            if (typeof(T) == typeof(Half)) return (Unsafe.As<T, ushort>(ref value) & 0x7fff) > 0x7c00;
        }
        return false;
    }

    /// <summary>
    /// Compares with a cached value whose NaN state was prepared by IsNaturalFloatingNaN.
    /// Other comparers and observing contexts retain their ordinary comparison and callbacks.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsElementGreaterThanWithNaNState(int i, T value, bool valueIsNaN, out T element)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareElement(i, value, out element) > 0;
        element = ReadUnobserved(i);
        if (_comparer is IComparableComparer)
        {
            if (typeof(T) == typeof(float)) return valueIsNaN
                ? !float.IsNaN(Unsafe.As<T, float>(ref element))
                : Unsafe.As<T, float>(ref element) > Unsafe.As<T, float>(ref value);
            if (typeof(T) == typeof(double)) return valueIsNaN
                ? !double.IsNaN(Unsafe.As<T, double>(ref element))
                : Unsafe.As<T, double>(ref element) > Unsafe.As<T, double>(ref value);
            if (typeof(T) == typeof(Half)) return valueIsNaN
                ? !Half.IsNaN(Unsafe.As<T, Half>(ref element))
                : Unsafe.As<T, Half>(ref element) > Unsafe.As<T, Half>(ref value);
        }
        return IsGreaterThan(element, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsElementLessOrEqual(int i, T value, out T element)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareElement(i, value, out element) <= 0;
        element = ReadUnobserved(i);
        return IsLessOrEqual(element, value);
    }

    /// <summary>Uses a cached NaN classification while preserving other comparison paths.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsElementLessThanWithNaNState(int i, T value, bool valueIsNaN)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareElement(i, value, out _) < 0;
        var element = ReadUnobserved(i);
        if (_comparer is IComparableComparer)
        {
            if (typeof(T) == typeof(float)) return valueIsNaN ? false : !(Unsafe.As<T, float>(ref element) >= Unsafe.As<T, float>(ref value));
            if (typeof(T) == typeof(double)) return valueIsNaN ? false : !(Unsafe.As<T, double>(ref element) >= Unsafe.As<T, double>(ref value));
            if (typeof(T) == typeof(Half)) return valueIsNaN ? false : !(Unsafe.As<T, Half>(ref element) >= Unsafe.As<T, Half>(ref value));
        }
        return IsLessThan(element, value);
    }

    /// <summary>Uses a cached NaN classification while preserving other comparison paths.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsElementLessOrEqualWithNaNState(int i, T value, bool valueIsNaN)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareElement(i, value, out _) <= 0;
        var element = ReadUnobserved(i);
        if (_comparer is IComparableComparer)
        {
            if (typeof(T) == typeof(float)) return valueIsNaN ? float.IsNaN(Unsafe.As<T, float>(ref element)) : !(Unsafe.As<T, float>(ref element) > Unsafe.As<T, float>(ref value));
            if (typeof(T) == typeof(double)) return valueIsNaN ? double.IsNaN(Unsafe.As<T, double>(ref element)) : !(Unsafe.As<T, double>(ref element) > Unsafe.As<T, double>(ref value));
            if (typeof(T) == typeof(Half)) return valueIsNaN ? Half.IsNaN(Unsafe.As<T, Half>(ref element)) : !(Unsafe.As<T, Half>(ref element) > Unsafe.As<T, Half>(ref value));
        }
        return IsLessOrEqual(element, value);
    }

    /// <summary>Uses a cached NaN classification while preserving other comparison paths.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValueLessThanWithNaNState(T value, int i, bool valueIsNaN)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareValue(value, i, out _) < 0;
        var element = ReadUnobserved(i);
        if (_comparer is IComparableComparer)
        {
            if (typeof(T) == typeof(float)) return valueIsNaN ? !float.IsNaN(Unsafe.As<T, float>(ref element)) : Unsafe.As<T, float>(ref value) < Unsafe.As<T, float>(ref element);
            if (typeof(T) == typeof(double)) return valueIsNaN ? !double.IsNaN(Unsafe.As<T, double>(ref element)) : Unsafe.As<T, double>(ref value) < Unsafe.As<T, double>(ref element);
            if (typeof(T) == typeof(Half)) return valueIsNaN ? !Half.IsNaN(Unsafe.As<T, Half>(ref element)) : Unsafe.As<T, Half>(ref value) < Unsafe.As<T, Half>(ref element);
        }
        return IsLessThan(value, element);
    }

    /// <summary>Uses a cached NaN classification while preserving other comparison paths.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValueLessOrEqualWithNaNState(T value, int i, bool valueIsNaN)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareValue(value, i, out _) <= 0;
        var element = ReadUnobserved(i);
        if (_comparer is IComparableComparer)
        {
            if (typeof(T) == typeof(float)) return valueIsNaN ? true : Unsafe.As<T, float>(ref value) <= Unsafe.As<T, float>(ref element);
            if (typeof(T) == typeof(double)) return valueIsNaN ? true : Unsafe.As<T, double>(ref value) <= Unsafe.As<T, double>(ref element);
            if (typeof(T) == typeof(Half)) return valueIsNaN ? true : Unsafe.As<T, Half>(ref value) <= Unsafe.As<T, Half>(ref element);
        }
        return IsLessOrEqual(value, element);
    }

    /// <summary>Uses a cached NaN classification while preserving other comparison paths.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValueGreaterThanWithNaNState(T value, int i, bool valueIsNaN)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareValue(value, i, out _) > 0;
        var element = ReadUnobserved(i);
        if (_comparer is IComparableComparer)
        {
            if (typeof(T) == typeof(float)) return valueIsNaN ? false : !(Unsafe.As<T, float>(ref value) <= Unsafe.As<T, float>(ref element));
            if (typeof(T) == typeof(double)) return valueIsNaN ? false : !(Unsafe.As<T, double>(ref value) <= Unsafe.As<T, double>(ref element));
            if (typeof(T) == typeof(Half)) return valueIsNaN ? false : !(Unsafe.As<T, Half>(ref value) <= Unsafe.As<T, Half>(ref element));
        }
        return IsGreaterThan(value, element);
    }

    /// <summary>Uses a cached NaN classification while preserving other comparison paths.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValueGreaterOrEqualWithNaNState(T value, int i, bool valueIsNaN)
    {
        if (typeof(TContext) != typeof(NullContext)) return ReportCompareValue(value, i, out _) >= 0;
        var element = ReadUnobserved(i);
        if (_comparer is IComparableComparer)
        {
            if (typeof(T) == typeof(float)) return valueIsNaN ? float.IsNaN(Unsafe.As<T, float>(ref element)) : !(Unsafe.As<T, float>(ref value) < Unsafe.As<T, float>(ref element));
            if (typeof(T) == typeof(double)) return valueIsNaN ? double.IsNaN(Unsafe.As<T, double>(ref element)) : !(Unsafe.As<T, double>(ref value) < Unsafe.As<T, double>(ref element));
            if (typeof(T) == typeof(Half)) return valueIsNaN ? Half.IsNaN(Unsafe.As<T, Half>(ref element)) : !(Unsafe.As<T, Half>(ref value) < Unsafe.As<T, Half>(ref element));
        }
        return IsGreaterOrEqual(value, element);
    }

    /// <summary>Runs an invariant-key loop with a natural-order specialization when available.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WithPreparedKey<TLoop>(T key, ref TLoop loop)
        where TLoop : struct, IPreparedKeyLoop<T, TComparer, TContext>
    {
        if (typeof(TContext) != typeof(NullContext) || _comparer is not IComparableComparer ||
            !KeyPreparation<T>.Enabled(TLoop.Kind))
            loop.Run<ComparerKeyOrder>(this, key);
        else if (IsPreparedKeyNaN(key, TLoop.Kind))
            loop.Run<NaNKeyOrder>(this, key);
        else
            loop.Run<NonNaNKeyOrder>(this, key);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsElementLessThan<TOrder>(int i, T key) where TOrder : struct, IKeyOrder
        => typeof(TOrder) == typeof(ComparerKeyOrder) ? IsElementLessThan(i, key)
            : IsElementLessThanWithNaNState(i, key, typeof(TOrder) == typeof(NaNKeyOrder));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValueLessThan<TOrder>(T key, int i) where TOrder : struct, IKeyOrder
        => typeof(TOrder) == typeof(ComparerKeyOrder) ? IsValueLessThan(key, i)
            : IsValueLessThanWithNaNState(key, i, typeof(TOrder) == typeof(NaNKeyOrder));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsElementLessOrEqual<TOrder>(int i, T key) where TOrder : struct, IKeyOrder
        => typeof(TOrder) == typeof(ComparerKeyOrder) ? IsElementLessOrEqual(i, key)
            : IsElementLessOrEqualWithNaNState(i, key, typeof(TOrder) == typeof(NaNKeyOrder));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsPreparedKeyNaN(T key, PreparedKeyKind kind)
    {
        if (kind == PreparedKeyKind.BinarySearch && typeof(T) == typeof(float))
            return float.IsNaN(Unsafe.As<T, float>(ref key));

        // An ordered self-comparison is false only for NaN.
        if (typeof(T) == typeof(float))
        {
            var value = Unsafe.As<T, float>(ref key);
            var copy = value;
            return !(value >= copy);
        }
        if (typeof(T) == typeof(double))
        {
            var value = Unsafe.As<T, double>(ref key);
            var copy = value;
            return !(value >= copy);
        }
        if (typeof(T) == typeof(Half)) return Half.IsNaN(Unsafe.As<T, Half>(ref key));
        return false;
    }
}
