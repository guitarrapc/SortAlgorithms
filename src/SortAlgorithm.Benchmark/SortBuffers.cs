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

    /// <summary>
    /// Seed of the input behind the pool's first buffer; buffer <c>i</c> uses <c>FirstSeed + i</c>.
    /// The first buffer therefore holds the input every buffer used to share.
    /// </summary>
    public const int FirstSeed = 42;
}

/// <summary>
/// A pool of pre-copied work buffers that keeps the restore cost (Array.Copy) out of the
/// measured region. <see cref="Reset"/> is called from [IterationSetup] to refresh all
/// buffers from their sources; each benchmark invocation then takes a fresh,
/// unsorted buffer via <see cref="Next"/> with zero copy inside the timed code.
/// </summary>
/// <remarks>
/// Every buffer has its own source, generated from its own seed. When all buffers held the same
/// input, the branch predictor learned that input across the invocations of an iteration, and
/// branchy sorts measured much faster on "random" data of a few thousand elements than
/// on data they had not seen. Patterns that do not depend on the seed (sorted, reversed, pipe organ)
/// still produce the same input in every buffer; they are as predictable as their definition.
/// </remarks>
/// <typeparam name="T">Element type of the arrays being sorted.</typeparam>
public sealed class SortBuffers<T>
{
    private readonly T[][] _sources;
    private readonly T[][] _buffers;
    private int _next;

    /// <param name="generate">Produces the input for a seed; called once per buffer with
    /// <see cref="SortBuffers.FirstSeed"/> + the buffer's index. Every input must have the same length.</param>
    public SortBuffers(Func<int, T[]> generate)
    {
        _sources = new T[SortBuffers.InvocationsPerIteration][];
        _buffers = new T[SortBuffers.InvocationsPerIteration][];
        for (var i = 0; i < _buffers.Length; i++)
        {
            _sources[i] = generate(SortBuffers.FirstSeed + i);
            if (_sources[i].Length != _sources[0].Length)
                throw new ArgumentException("Every generated input must have the same length.", nameof(generate));
            _buffers[i] = new T[_sources[i].Length];
        }
    }

    /// <summary>The inputs behind the buffers, in invocation order. Not to be modified.</summary>
    public IReadOnlyList<T[]> Sources => _sources;

    /// <summary>Refreshes every buffer from its own source. Call from [IterationSetup].</summary>
    public void Reset()
    {
        for (var i = 0; i < _buffers.Length; i++)
        {
            Array.Copy(_sources[i], _buffers[i], _sources[i].Length);
        }
        _next = 0;
    }

    /// <summary>Returns the next fresh (unsorted) buffer for this iteration.</summary>
    public T[] Next() => _buffers[_next++];
}
