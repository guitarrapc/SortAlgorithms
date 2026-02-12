namespace SortAlgorithm.Contexts;

/// <summary>
/// No-op implementation of ISortContext.
/// </summary>
public sealed class NullContext : ISortContext
{
    public static readonly NullContext Default = new();

    private NullContext()
    {
    }

    public void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ)
    {
    }

    public void OnSwap(int i, int j, int bufferId)
    {
    }

    public void OnIndexRead(int index, int bufferId)
    {
    }

    public void OnIndexWrite(int index, int bufferId, object? value = null)
    {
    }

    public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, object?[]? values = null)
    {
    }
}
