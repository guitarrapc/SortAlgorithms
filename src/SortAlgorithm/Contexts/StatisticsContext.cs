namespace SortAlgorithm.Contexts;

/// <summary>
/// Tracks the number of comparisons, swaps, and index accesses performed during sorting operations for a collection of elements.
/// </summary>
/// <remarks>
/// Use this context to gather statistics when implementing or analyzing sorting algorithms. The
/// collected counts can be used to evaluate algorithm efficiency or compare different sorting strategies.
/// Counters use plain (non-atomic) increments: every sort in this library is single-threaded, and
/// Interlocked operations cost 10-20x a plain increment (lock-prefixed RMW + full memory barrier),
/// which dominated instrumented-run time. Do not share one instance across concurrently running sorts;
/// use one instance per sort (or per thread) instead.
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
        _compareCount++;
    }

    public void OnSwap(int i, int j, int bufferId)
    {
        // Exclude swaps with negative buffer IDs (reserved for non-array structures excluded from statistics)
        if (bufferId < 0)
            return;

        _swapCount++;
        // Swap操作は内部的にRead×2 + Write×2を含む
        // temp = array[i] (Read), value = array[j] (Read), array[i] = value (Write), array[j] = temp (Write)
        _indexReadCount += 2;
        _indexWriteCount += 2;
    }

    public void OnIndexRead(int index, int bufferId)
    {
        // Exclude reads from negative buffer IDs (reserved for non-array structures excluded from statistics)
        if (bufferId < 0)
            return;

        _indexReadCount++;
    }

    public void OnIndexWrite(int index, int bufferId, object? value = null)
    {
        // Exclude writes to negative buffer IDs (reserved for non-array structures excluded from statistics)
        if (bufferId < 0)
            return;

        _indexWriteCount++;
    }

    public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, object?[]? values = null)
    {
        // Range copy is counted as: length reads from source + length writes to destination
        // Exclude operations with negative buffer IDs (reserved for non-array structures excluded from statistics)
        if (sourceBufferId >= 0)
            _indexReadCount += (ulong)length;

        if (destinationBufferId >= 0)
            _indexWriteCount += (ulong)length;
    }

    public void OnPhase(SortPhase phase, int param1 = 0, int param2 = 0, int param3 = 0) { }

    public void OnRole(int index, int bufferId, RoleType role) { }

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
