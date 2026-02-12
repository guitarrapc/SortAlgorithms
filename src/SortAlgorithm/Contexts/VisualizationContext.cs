namespace SortAlgorithm.Contexts;

/// <summary>
/// Provides a context for visualizing sorting operations by exposing callbacks for compare, swap, and index access
/// events.
/// </summary>
/// <remarks>
/// Use this class to observe or record the behavior of sorting algorithms by supplying callback actions
/// for key operations. This is useful for building visualizations or collecting statistics during sorting. The class is
/// sealed and intended for use as a utility within sorting visualizations or analysis tools.
/// </remarks>
public sealed class VisualizationContext : ISortContext
{
    private readonly Action<int, int, int, int, int>? _onCompare;
    private readonly Action<int, int, int>? _onSwap;
    private readonly Action<int, int>? _onIndexRead;
    private readonly Action<int, int, object?>? _onIndexWrite;
    private readonly Action<int, int, int, int, int, object?[]?>? _onRangeCopy;

    public VisualizationContext(
        Action<int, int, int, int, int>? onCompare = null,
        Action<int, int, int>? onSwap = null,
        Action<int, int>? onIndexRead = null,
        Action<int, int, object?>? onIndexWrite = null,
        Action<int, int, int, int, int, object?[]?>? onRangeCopy = null)
    {
        _onCompare = onCompare;
        _onSwap = onSwap;
        _onIndexRead = onIndexRead;
        _onIndexWrite = onIndexWrite;
        _onRangeCopy = onRangeCopy;
    }

    public void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ) => _onCompare?.Invoke(i, j, result, bufferIdI, bufferIdJ);
    public void OnSwap(int i, int j, int bufferId) => _onSwap?.Invoke(i, j, bufferId);
    public void OnIndexRead(int index, int bufferId) => _onIndexRead?.Invoke(index, bufferId);
    public void OnIndexWrite(int index, int bufferId, object? value = null) => _onIndexWrite?.Invoke(index, bufferId, value);
    public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, object?[]? values = null)
        => _onRangeCopy?.Invoke(sourceIndex, destinationIndex, length, sourceBufferId, destinationBufferId, values);
}
