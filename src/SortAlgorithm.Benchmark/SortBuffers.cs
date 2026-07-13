namespace SortAlgorithm.Benchmark;

/// <summary>
/// Holds the invocation batch size shared between the benchmark job configuration
/// (Program.cs) and the buffer pools used by every benchmark class.
/// </summary>
public static class SortBuffers
{
    /// <summary>
    /// Number of invocations BenchmarkDotNet runs per measured iteration, and therefore
    /// the number of pre-copied buffers each pool must hold. Every job in Program.cs
    /// must set WithInvocationCount(InvocationsPerIteration) and WithUnrollFactor(1);
    /// running these benchmarks with a job that invokes more often than this throws
    /// IndexOutOfRangeException from <see cref="SortBuffers{T}.Next"/> (loud failure
    /// instead of silently sorting already-sorted data).
    /// </summary>
    public const int InvocationsPerIteration = 64;
}

/// <summary>
/// A pool of pre-copied work buffers that keeps the restore cost (Array.Copy) out of the
/// measured region. <see cref="Reset"/> is called from [IterationSetup] to refresh all
/// buffers from the pristine source; each benchmark invocation then takes a fresh,
/// unsorted buffer via <see cref="Next"/> with zero copy inside the timed code.
/// </summary>
/// <typeparam name="T">Element type of the arrays being sorted.</typeparam>
public sealed class SortBuffers<T>
{
    private readonly T[] _pristine;
    private readonly T[][] _buffers;
    private int _next;

    public SortBuffers(T[] pristine)
    {
        _pristine = pristine;
        _buffers = new T[SortBuffers.InvocationsPerIteration][];
        for (var i = 0; i < _buffers.Length; i++)
        {
            _buffers[i] = new T[pristine.Length];
        }
    }

    /// <summary>Refreshes every buffer from the pristine source. Call from [IterationSetup].</summary>
    public void Reset()
    {
        foreach (var buffer in _buffers)
        {
            Array.Copy(_pristine, buffer, _pristine.Length);
        }
        _next = 0;
    }

    /// <summary>Returns the next fresh (unsorted) buffer for this iteration.</summary>
    public T[] Next() => _buffers[_next++];
}
