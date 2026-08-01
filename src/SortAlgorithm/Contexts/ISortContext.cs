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
    /// Handles a write operation whose written value is not observable
    /// (for example a tree pointer or height field, where only the fact of the write matters).
    /// </summary>
    /// <param name="index">The zero-based index at which the write operation occurs</param>
    /// <param name="bufferId">Buffer identifier (0 = main array, 1+ = auxiliary buffers)</param>
    void OnIndexWrite(int index, int bufferId);

    /// <summary>
    /// Handles a write operation at the specified index, specifying which buffer, carrying the written value.
    /// </summary>
    /// <remarks>
    /// The value is passed as <typeparamref name="T"/> rather than <see cref="object"/> on purpose:
    /// an <c>object</c> parameter boxes every write, which for an observing sort is one allocation per
    /// element written (measured: 12-19 MiB per pass for 32768 elements). Implementations that only
    /// need the fact of the write should ignore <paramref name="value"/>; implementations that need it
    /// for a known element type can test <c>typeof(T)</c> and reinterpret without boxing.
    /// </remarks>
    /// <param name="index">The zero-based index at which the write operation occurs</param>
    /// <param name="bufferId">Buffer identifier (0 = main array, 1+ = auxiliary buffers)</param>
    /// <param name="value">The value being written</param>
    void OnIndexWrite<T>(int index, int bufferId, T value);

    /// <summary>
    /// Handles a range copy operation between buffers whose copied values are not available.
    /// </summary>
    /// <param name="sourceIndex">Starting index in the source buffer</param>
    /// <param name="destinationIndex">Starting index in the destination buffer</param>
    /// <param name="length">Number of elements copied</param>
    /// <param name="sourceBufferId">Source buffer identifier (0 = main array, 1+ = auxiliary buffers, -1 = external)</param>
    /// <param name="destinationBufferId">Destination buffer identifier (0 = main array, 1+ = auxiliary buffers, -1 = external)</param>
    void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId);

    /// <summary>
    /// Handles a range copy operation between buffers, carrying the copied values.
    /// </summary>
    /// <remarks>
    /// The values are passed as a <see cref="ReadOnlySpan{T}"/> over the source range rather than a
    /// materialized <c>object?[]</c>: the array form allocated one array plus one box per element on
    /// every range copy. The span is only valid for the duration of the call; implementations that need
    /// to retain the values must copy them out.
    /// </remarks>
    /// <param name="sourceIndex">Starting index in the source buffer</param>
    /// <param name="destinationIndex">Starting index in the destination buffer</param>
    /// <param name="length">Number of elements copied</param>
    /// <param name="sourceBufferId">Source buffer identifier (0 = main array, 1+ = auxiliary buffers, -1 = external)</param>
    /// <param name="destinationBufferId">Destination buffer identifier (0 = main array, 1+ = auxiliary buffers, -1 = external)</param>
    /// <param name="values">The actual values being copied (used for visualization accuracy)</param>
    void OnRangeCopy<T>(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, ReadOnlySpan<T> values);

    /// <summary>
    /// Announces the current algorithm phase using structured data.
    /// The tutorial layer assembles the display string from <paramref name="phase"/> and its parameters.
    /// Implementations that do not support tutorial visualization may implement this as a no-op.
    /// </summary>
    /// <param name="phase">Phase kind. Determines how parameters are interpreted.</param>
    /// <param name="param1">First phase parameter (meaning depends on <paramref name="phase"/>).</param>
    /// <param name="param2">Second phase parameter (meaning depends on <paramref name="phase"/>).</param>
    /// <param name="param3">Third phase parameter (meaning depends on <paramref name="phase"/>).</param>
    void OnPhase(SortPhase phase, int param1 = 0, int param2 = 0, int param3 = 0);

    /// <summary>
    /// Assigns a semantic role to a specific array element (e.g., Pivot, CurrentMin).
    /// The role persists across steps until explicitly cleared with <see cref="RoleType.None"/>.
    /// Implementations that do not support tutorial visualization may implement this as a no-op.
    /// </summary>
    /// <param name="index">The zero-based index of the element</param>
    /// <param name="bufferId">Buffer identifier (0 = main array, 1+ = auxiliary buffers)</param>
    /// <param name="role">The role to assign; use <see cref="RoleType.None"/> to clear</param>
    void OnRole(int index, int bufferId, RoleType role);
}

