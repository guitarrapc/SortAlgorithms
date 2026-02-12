namespace SortAlgorithm.Contexts;

/// <summary>
/// Provides a composite implementation of the ISortContext interface that delegates sorting events to multiple
/// underlying sort contexts.
/// </summary>
/// <remarks>
/// Use CompositeSortContext to combine multiple ISortContext instances, allowing each to receive
/// notifications for sorting operations such as comparisons, swaps, and index accesses. This is useful when you want to
/// apply multiple behaviors or observers to the same sorting process, such as logging, statistics collection, or
/// visualization. All provided contexts will receive each event in the order they were supplied to the
/// constructor.
/// </remarks>
public sealed class CompositeContext : ISortContext
{
    private readonly ISortContext[] _contexts;

    public CompositeContext(params ISortContext[] contexts)
    {
        _contexts = contexts;
    }

    public void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ)
    {
        foreach (var context in _contexts)
        {
            context.OnCompare(i, j, result, bufferIdI, bufferIdJ);
        }
    }
    public void OnSwap(int i, int j, int bufferId)
    {
        foreach (var context in _contexts)
        {
            context.OnSwap(i, j, bufferId);
        }
    }
    public void OnIndexRead(int index, int bufferId)
    {
        foreach (var context in _contexts)
        {
            context.OnIndexRead(index, bufferId);
        }
    }

    public void OnIndexWrite(int index, int bufferId, object? value = null)
    {
        foreach (var context in _contexts)
        {
            context.OnIndexWrite(index, bufferId, value);
        }
    }

    public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, object?[]? values = null)
    {
        foreach (var context in _contexts)
        {
            context.OnRangeCopy(sourceIndex, destinationIndex, length, sourceBufferId, destinationBufferId, values);
        }
    }
}
