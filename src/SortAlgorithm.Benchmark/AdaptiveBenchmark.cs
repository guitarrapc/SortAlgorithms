namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class AdaptiveBenchmark
{
    [Params(256, 1024, 8192)]
    public int Size { get; set; }

    [Params(DataPattern.Random, DataPattern.SingleElementMoved, DataPattern.Sorted, DataPattern.Reversed, DataPattern.PipeOrgan)]
    public DataPattern Pattern { get; set; }

    private SortBuffers<int> _buffers = default!;

    // Restore cost stays out of the timed region: [IterationSetup] refreshes a pool of
    // pre-copied buffers and each invocation sorts a fresh one (see SortBuffers<T>).
    // Program.cs pins the job's InvocationCount to the pool size.
    [GlobalSetup]
    public void Setup()
    {
        _buffers = new SortBuffers<int>(BenchmarkData.GenerateIntArray(Size, Pattern));
    }

    [IterationSetup]
    public void IterationSetup() => _buffers.Reset();

    [Benchmark(Baseline = true)]
    public void DropMergeSort()
    {
        SortAlgorithm.Algorithms.DropMergeSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void PatienceSort()
    {
        SortAlgorithm.Algorithms.PatienceSort.Sort(_buffers.Next().AsSpan());
    }
}

[MemoryDiagnoser]
[RankColumn]
public class AdaptiveSlowBenchmark
{
    [Params(256, 1024)]
    public int Size { get; set; }

    [Params(DataPattern.Random, DataPattern.SingleElementMoved, DataPattern.Sorted, DataPattern.Reversed, DataPattern.PipeOrgan)]
    public DataPattern Pattern { get; set; }

    private SortBuffers<int> _buffers = default!;

    // Restore cost stays out of the timed region: [IterationSetup] refreshes a pool of
    // pre-copied buffers and each invocation sorts a fresh one (see SortBuffers<T>).
    // Program.cs pins the job's InvocationCount to the pool size.
    [GlobalSetup]
    public void Setup()
    {
        _buffers = new SortBuffers<int>(BenchmarkData.GenerateIntArray(Size, Pattern));
    }

    [IterationSetup]
    public void IterationSetup() => _buffers.Reset();

    [Benchmark(Baseline = true)]
    public void StrandSort()
    {
        SortAlgorithm.Algorithms.StrandSort.Sort(_buffers.Next().AsSpan());
    }
}
