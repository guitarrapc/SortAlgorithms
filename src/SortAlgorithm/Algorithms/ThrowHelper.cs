using SortAlgorithm.Contexts;
using System.Diagnostics.CodeAnalysis;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// Reports a comparer that is not a consistent total order. The helpers are not marked <c>NoInlining</c>. With it, the JIT kept their
/// calls inside the loops that check cursors instead of moving them to cold code.
/// </summary>
internal static class ThrowHelper
{
    const string InconsistentComparerMessage = "Comparison method violates its general contract.";

    /// <summary>Reports a comparer whose answers would move a cursor or a write out of its range.</summary>
    [DoesNotReturn]
    public static void ThrowInconsistentComparer()
        => throw new InvalidOperationException(InconsistentComparerMessage);

    /// <summary>
    /// Reports a comparer that moved an unguarded insertion past its sentinel. The shifts have left a gap at <paramref name="gap"/>, and
    /// filling it with the element being inserted keeps the range a permutation of its input.
    /// </summary>
    [DoesNotReturn]
    public static void RestoreAndThrowInconsistentComparer<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> s, int gap, T value)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        s.Write(gap, value);
        throw new InvalidOperationException(InconsistentComparerMessage);
    }
}
