namespace SortAlgorithm.Benchmark;

/// <summary>Constants shared by the jobs in Program.cs and every <see cref="SortBuffers{T}"/> pool.</summary>
public static class SortBuffers
{
    /// <summary>
    /// Invocations per measured iteration, and so the number of buffers in each pool.
    /// Every job must use WithInvocationCount(InvocationsPerIteration) and WithUnrollFactor(1).
    /// If a job invokes more often, <see cref="SortBuffers{T}.Next"/> throws instead of sorting sorted data.
    /// </summary>
    public const int InvocationsPerIteration = 64;

    /// <summary>
    /// Invocations per iteration for <see cref="ScalingBenchmark"/>, which overrides the job with [InvocationCount].
    /// One sort of a large input already takes milliseconds, and a pool of 64 inputs of 2^20 ints with their copies would hold 512 MB.
    /// </summary>
    public const int LargeInvocationsPerIteration = 4;

    /// <summary>Seed for buffer 0. Buffer <c>i</c> uses <c>FirstSeed + i</c>.</summary>
    public const int FirstSeed = 42;
}

/// <summary>
/// Pre-copied input buffers that keep the copy out of the timed code.
/// Call <see cref="Reset"/> from [IterationSetup], then take one buffer per invocation with <see cref="Next"/>.
/// </summary>
/// <remarks>
/// Each buffer gets its own seed, so the branch predictor cannot learn the input across invocations.
/// Seed-independent patterns (sorted, reversed, pipe organ) are still identical in every buffer.
/// On pipe organ this makes branchy sorts look faster. Shifting the peak far enough to avoid that
/// also weakens pipe organ as a median-of-three worst case, so the pattern stays as defined.
/// </remarks>
/// <typeparam name="T">Element type of the arrays being sorted.</typeparam>
public sealed class SortBuffers<T>
{
    private readonly T[][] _sources;
    private readonly T[][] _buffers;
    private int _next;

    /// <param name="generate">Returns the input for a seed. Called once per buffer with
    /// <see cref="SortBuffers.FirstSeed"/> + its index. All inputs must have the same length.</param>
    /// <param name="count">Number of buffers; must equal the invocation count of the job that uses the pool.</param>
    public SortBuffers(Func<int, T[]> generate, int count = SortBuffers.InvocationsPerIteration)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(count, 1);
        _sources = new T[count][];
        _buffers = new T[count][];
        for (var i = 0; i < _buffers.Length; i++)
        {
            _sources[i] = generate(SortBuffers.FirstSeed + i);
            if (_sources[i].Length != _sources[0].Length)
                throw new ArgumentException("Every generated input must have the same length.", nameof(generate));
            _buffers[i] = new T[_sources[i].Length];
        }
    }

    /// <summary>The source inputs, in invocation order. Do not modify them.</summary>
    public IReadOnlyList<T[]> Sources => _sources;

    /// <summary>Copies each source into its buffer. Call from [IterationSetup].</summary>
    public void Reset()
    {
        for (var i = 0; i < _buffers.Length; i++)
        {
            Array.Copy(_sources[i], _buffers[i], _sources[i].Length);
        }
        _next = 0;
    }

    /// <summary>Returns the next unsorted buffer.</summary>
    public T[] Next() => _buffers[_next++];
}
