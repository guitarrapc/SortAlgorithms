namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class NetworkBenchmark
{
    [Params(256, 1024, 4096, 8192)]
    public int Size { get; set; }

    [Params(DataPattern.Random, DataPattern.SingleElementMoved, DataPattern.Sorted, DataPattern.Reversed, DataPattern.PipeOrgan, DataPattern.ManyDuplicates)]
    public DataPattern Pattern { get; set; }

    private SortBuffers<int> _buffers = default!;

    // Restore cost stays out of the timed region: [IterationSetup] refreshes a pool of
    // pre-copied buffers and each invocation sorts a fresh one, each generated from its own
    // seed, so a seeded pattern never repeats within an iteration (see SortBuffers<T>).
    // Program.cs pins the job's InvocationCount to the pool size.
    [GlobalSetup]
    public void Setup()
    {
        _buffers = new SortBuffers<int>(seed => BenchmarkData.GenerateIntArray(Size, Pattern, seed));
    }

    [IterationSetup]
    public void IterationSetup() => _buffers.Reset();

    [Benchmark(Baseline = true)]
    public void BitonicSort()
    {
        SortAlgorithm.Algorithms.BitonicSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void BitonicRecursiveSort()
    {
        SortAlgorithm.Algorithms.BitonicSortNonOptimized.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void BatcherOddEvenMergeSort()
    {
        SortAlgorithm.Algorithms.BatcherOddEvenMergeSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void PairwiseSortingNetwork()
    {
        SortAlgorithm.Algorithms.PairwiseSortingNetwork.Sort(_buffers.Next().AsSpan());
    }
}
