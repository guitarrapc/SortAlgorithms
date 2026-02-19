using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// A high-performance struct comparer that uses IComparable&lt;T&gt;.CompareTo for comparison.
/// This struct is used internally by convenience overloads to achieve maximum performance
/// through JIT devirtualization and inlining of the constrained CompareTo call.
/// </summary>
/// <typeparam name="T">The type of elements to compare. Must implement IComparable&lt;T&gt;.</typeparam>
/// <remarks>
/// <para>
/// When TComparer is a struct type (like this ComparableComparer&lt;T&gt;), the JIT compiler can:
/// - Devirtualize the Compare method completely (no virtual dispatch)
/// - Inline the Compare method into the calling code
/// - Further inline the constrained CompareTo call into a direct comparison instruction for primitives (int, float, etc.)
/// </para>
/// <para>
/// This results in performance equivalent to or better than using IComparable&lt;T&gt; directly,
/// while maintaining compatibility with the generic TComparer pattern used throughout the codebase.
/// </para>
/// <para>
/// Performance comparison (for int array sorting):
/// - Using Comparer&lt;int&gt;.Default (class): ~3-5ns overhead per comparison due to virtual dispatch
/// - Using ComparableComparer&lt;int&gt; (struct): ~0ns overhead, same as direct CompareTo
/// </para>
/// </remarks>
internal readonly struct ComparableComparer<T> : IComparer<T> where T : IComparable<T>
{
    /// <summary>
    /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
    /// </summary>
    /// <param name="x">The first object to compare.</param>
    /// <param name="y">The second object to compare.</param>
    /// <returns>
    /// A signed integer that indicates the relative values of x and y:
    /// - Less than zero: x is less than y
    /// - Zero: x equals y
    /// - Greater than zero: x is greater than y
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Compare(T? x, T? y)
    {
        if (x == null)
        {
            return y == null ? 0 : -1;
        }
        return x.CompareTo(y!);
    }
}

/// <summary>
/// A no-op comparer for distribution sort algorithms that never compare elements.
/// Calling <see cref="Compare"/> is a programming error and throws <see cref="NotSupportedException"/>.
/// </summary>
/// <typeparam name="T">The element type. No constraint required.</typeparam>
internal readonly struct NullComparer<T> : IComparer<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Compare(T? x, T? y) =>
        throw new NotSupportedException("NullComparer should never be called by distribution sort algorithms.");
}
