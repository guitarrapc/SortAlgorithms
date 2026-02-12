namespace SortAlgorithm.Contexts;

/// <summary>
/// Tracks the number of comparisons, swaps, and index accesses performed during sorting operations for a collection of elements.
/// </summary>
/// <remarks>
/// Use this context to gather statistics when implementing or analyzing sorting algorithms. The
/// collected counts can be used to evaluate algorithm efficiency or compare different sorting strategies. This class is
/// thread-safe for incrementing statistics.
/// </remarks>
public sealed class StatisticsContext : ISortContext
{
    public ulong CompareCount => _compareCount;
    private ulong _compareCount;

    public ulong SwapCount => _swapCount;
    private ulong _swapCount;

    public ulong IndexReadCount => _indexReadCount;
    private ulong _indexReadCount;

    public ulong IndexWriteCount => _indexWriteCount;
    private ulong _indexWriteCount;

    public void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ)
    {
        // Always count comparisons (even with negative buffer IDs)
        // Negative buffer IDs in comparisons typically indicate index-less value comparisons,
        // which are still logically part of the sorting algorithm
        Interlocked.Increment(ref _compareCount);
    }

    public void OnSwap(int i, int j, int bufferId)
    {
        // Exclude swaps with negative buffer IDs (used for tree nodes or other non-array structures)
        if (bufferId < 0)
            return;

        Interlocked.Increment(ref _swapCount);
        // Swap操作は内部的にRead×2 + Write×2を含む
        // temp = array[i] (Read), value = array[j] (Read), array[i] = value (Write), array[j] = temp (Write)
        Interlocked.Add(ref _indexReadCount, 2);
        Interlocked.Add(ref _indexWriteCount, 2);
    }

    public void OnIndexRead(int index, int bufferId)
    {
        // Exclude reads from negative buffer IDs (used for tree nodes or other non-array structures)
        if (bufferId < 0)
            return;

        Interlocked.Increment(ref _indexReadCount);
    }

    public void OnIndexWrite(int index, int bufferId, object? value = null)
    {
        // Exclude writes to negative buffer IDs (used for tree nodes or other non-array structures)
        if (bufferId < 0)
            return;

        Interlocked.Increment(ref _indexWriteCount);
    }

    public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, object?[]? values = null)
    {
        // Range copy is counted as: length reads from source + length writes to destination
        // Exclude operations with negative buffer IDs (used for tree nodes or other non-array structures)
        if (sourceBufferId >= 0)
            Interlocked.Add(ref _indexReadCount, (ulong)length);

        if (destinationBufferId >= 0)
            Interlocked.Add(ref _indexWriteCount, (ulong)length);
    }

    /// <summary>
    /// Resets all operation counters to zero.
    /// </summary>
    /// <remarks>
    /// Call this method to clear the current counts for comparisons, swaps, and index accesses,
    /// typically before starting a new measurement or operation.
    /// </remarks>
    public void Reset()
    {
        _compareCount = 0;
        _swapCount = 0;
        _indexReadCount = 0;
        _indexWriteCount = 0;
    }
}
