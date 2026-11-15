using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

internal enum PreparedKeyKind
{
    Partition,
    BinarySearch,
}

internal interface IKeyOrder { }

internal readonly struct ComparerKeyOrder : IKeyOrder { }
internal readonly struct NaNKeyOrder : IKeyOrder { }
internal readonly struct NonNaNKeyOrder : IKeyOrder { }

// A struct loop avoids allocating a delegate for the selected comparison policy.
internal interface IPreparedKeyLoop<T, TComparer, TContext>
    where TComparer : IComparer<T>
    where TContext : ISortContext
{
    static abstract PreparedKeyKind Kind { get; }

    void Run<TOrder>(SortSpan<T, TComparer, TContext> s, T key)
        where TOrder : struct, IKeyOrder;
}

internal static class KeyPreparation<T>
{
    // Inline the selector so the caller can discard unused comparison paths.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Enabled(PreparedKeyKind kind) => kind switch
    {
        PreparedKeyKind.BinarySearch => typeof(T) == typeof(float),
        _ => typeof(T) == typeof(float) || typeof(T) == typeof(double) || typeof(T) == typeof(Half),
    };
}
