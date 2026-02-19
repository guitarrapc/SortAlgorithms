using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// Defines a zero-allocation key selector for distribution sort algorithms.
/// Implement as a <see langword="readonly"/> <see langword="struct"/> to enable JIT devirtualization and inlining.
/// </summary>
/// <typeparam name="T">The type of elements from which a key is extracted.</typeparam>
/// <remarks>
/// <para>
/// This interface follows the same zero-allocation devirtualization pattern as <see cref="System.Collections.Generic.IComparer{T}"/>
/// used throughout this library. When <typeparamref name="T"/> is a struct, the JIT compiler can:
/// <list type="bullet">
/// <item><description>Devirtualize the <see cref="GetKey"/> call completely (no virtual dispatch)</description></item>
/// <item><description>Inline the <see cref="GetKey"/> body into the calling sort loop</description></item>
/// </list>
/// </para>
/// <para>
/// Contrast with <see cref="System.Func{T, TResult}"/>: a delegate always incurs indirect-call overhead
/// and may allocate on the heap when it captures variables.
/// </para>
/// <example>
/// <code>
/// readonly struct PersonAgeSelector : IKeySelector&lt;Person&gt;
/// {
///     public int GetKey(Person value) =&gt; value.Age;
/// }
///
/// BucketSort.Sort(span, new PersonAgeSelector());
/// </code>
/// </example>
/// </remarks>
public interface IKeySelector<T>
{
    /// <summary>
    /// Extracts an integer key from the specified element.
    /// The key is used to determine the element's bucket in distribution sort algorithms.
    /// </summary>
    /// <param name="value">The element from which to extract the key.</param>
    /// <returns>An integer key that represents the sort position of <paramref name="value"/>.</returns>
    int GetKey(T value);
}

/// <summary>
/// Adapts a <see cref="Func{T, TResult}"/> delegate to the <see cref="IKeySelector{T}"/> interface.
/// Used internally by convenience overloads to delegate to the high-performance generic path.
/// </summary>
/// <remarks>
/// Note: the underlying delegate call is not devirtualized by the JIT.
/// For maximum performance, implement <see cref="IKeySelector{T}"/> directly as a <see langword="readonly"/> <see langword="struct"/>.
/// </remarks>
internal readonly struct FuncKeySelector<T>(Func<T, int> func) : IKeySelector<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetKey(T value) => func(value);
}
