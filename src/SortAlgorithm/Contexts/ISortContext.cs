namespace SortAlgorithm.Contexts;

/// <summary>
/// Defines the contract for tracking sorting algorithm operations.
/// Implementations can collect statistics, visualize operations, or perform other observations.
/// </summary>
public interface ISortContext
{
    /// <summary>
    /// Handles the result of comparing two elements, specifying which buffers they belong to.
    /// </summary>
    /// <param name="i">Index of the compare from</param>
    /// <param name="j">Index of the compare to</param>
    /// <param name="result">The result of the comparison</param>
    /// <param name="bufferIdI">Buffer identifier for element at index i (0 = main array, 1+ = auxiliary buffers)</param>
    /// <param name="bufferIdJ">Buffer identifier for element at index j (0 = main array, 1+ = auxiliary buffers)</param>
    void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ);

    /// <summary>
    /// Handles the swapping of two elements, specifying which buffer they belong to.
    /// </summary>
    /// <param name="i">Index of the swap from</param>
    /// <param name="j">Index of the swap to</param>
    /// <param name="bufferId">Buffer identifier (0 = main array, 1+ = auxiliary buffers)</param>
    void OnSwap(int i, int j, int bufferId);

    /// <summary>
    /// Handles the event when an item at the specified index is read, specifying which buffer.
    /// </summary>
    /// <param name="index">The zero-based index of the item that was read</param>
    /// <param name="bufferId">Buffer identifier (0 = main array, 1+ = auxiliary buffers)</param>
    void OnIndexRead(int index, int bufferId);

    /// <summary>
    /// Handles a write operation at the specified index, specifying which buffer.
    /// </summary>
    /// <param name="index">The zero-based index at which the write operation occurs</param>
    /// <param name="bufferId">Buffer identifier (0 = main array, 1+ = auxiliary buffers)</param>
    /// <param name="value">The value being written (optional, used for visualization)</param>
    void OnIndexWrite(int index, int bufferId, object? value = null);

    /// <summary>
    /// Handles a range copy operation between buffers.
    /// </summary>
    /// <param name="sourceIndex">Starting index in the source buffer</param>
    /// <param name="destinationIndex">Starting index in the destination buffer</param>
    /// <param name="length">Number of elements copied</param>
    /// <param name="sourceBufferId">Source buffer identifier (0 = main array, 1+ = auxiliary buffers, -1 = external)</param>
    /// <param name="destinationBufferId">Destination buffer identifier (0 = main array, 1+ = auxiliary buffers, -1 = external)</param>
    /// <param name="values">The actual values being copied (used for visualization accuracy). May be null if values are not available.</param>
    void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, object?[]? values = null);
}

