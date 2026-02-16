using System.Runtime.CompilerServices;

namespace SortAlgorithm.Contexts;

/// <summary>
/// No-op implementation of ISortContext.
/// </summary>
/// <remarks>
/// Struct is used to achieve zero cost abstraction for cases where no context is needed.
/// All methods are implemented as no-ops, so there is no overhead for using this context. (Dead Code Elimination in <see cref="SortAlgorithm.Algorithms.SortSpan{T, TComparer, TContext}"/>)
/// </remarks>
public readonly struct NullContext : ISortContext
{
    public static readonly NullContext Default = default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnSwap(int i, int j, int bufferId) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnIndexRead(int index, int bufferId) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnIndexWrite(int index, int bufferId, object? value = null) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, object?[]? values = null) { }
}
