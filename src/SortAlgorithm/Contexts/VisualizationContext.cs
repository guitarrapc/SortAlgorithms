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
    private readonly Action<SortPhase, int, int, int>? _onPhase;
    private readonly Action<int, int, RoleType>? _onRole;

    public VisualizationContext(
        Action<int, int, int, int, int>? onCompare = null,
        Action<int, int, int>? onSwap = null,
        Action<int, int>? onIndexRead = null,
        Action<int, int, object?>? onIndexWrite = null,
        Action<int, int, int, int, int, object?[]?>? onRangeCopy = null,
        Action<SortPhase, int, int, int>? onPhase = null,
        Action<int, int, RoleType>? onRole = null)
    {
        _onCompare = onCompare;
        _onSwap = onSwap;
        _onIndexRead = onIndexRead;
        _onIndexWrite = onIndexWrite;
        _onRangeCopy = onRangeCopy;
        _onPhase = onPhase;
        _onRole = onRole;
    }

    public void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ) => _onCompare?.Invoke(i, j, result, bufferIdI, bufferIdJ);
    public void OnSwap(int i, int j, int bufferId) => _onSwap?.Invoke(i, j, bufferId);
    public void OnIndexRead(int index, int bufferId) => _onIndexRead?.Invoke(index, bufferId);
    public void OnIndexWrite(int index, int bufferId) => _onIndexWrite?.Invoke(index, bufferId, null);

    // The callbacks are typed with object?/object?[]? so that a caller can observe any element type
    // without knowing it. That costs a box per write and an array per range copy, but only when a
    // callback is actually attached — the null check happens before the arguments are materialized.
    // Consumers that record every operation of a large sort should implement ISortContext directly
    // and keep the value as T (see SortVivo's recording context).
    public void OnIndexWrite<T>(int index, int bufferId, T value) => _onIndexWrite?.Invoke(index, bufferId, value);

    public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId)
        => _onRangeCopy?.Invoke(sourceIndex, destinationIndex, length, sourceBufferId, destinationBufferId, null);

    public void OnRangeCopy<T>(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, ReadOnlySpan<T> values)
    {
        if (_onRangeCopy is null) return;

        var boxed = new object?[values.Length];
        for (var i = 0; i < values.Length; i++) boxed[i] = values[i];
        _onRangeCopy(sourceIndex, destinationIndex, length, sourceBufferId, destinationBufferId, boxed);
    }
    public void OnPhase(SortPhase phase, int param1 = 0, int param2 = 0, int param3 = 0) => _onPhase?.Invoke(phase, param1, param2, param3);
    public void OnRole(int index, int bufferId, RoleType role) => _onRole?.Invoke(index, bufferId, role);
}
