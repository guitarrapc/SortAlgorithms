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

    // Each invocation sorts its own pre-copied buffer, so the copy is not timed (see SortBuffers<T>).
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
    public void BitonicSortBranchless()
    {
        SortAlgorithm.Algorithms.BitonicSortBranchless.Sort(_buffers.Next().AsSpan());
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
    public void BatcherOddEvenMergeSortBranchless()
    {
        SortAlgorithm.Algorithms.BatcherOddEvenMergeSortBranchless.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void PairwiseSortingNetwork()
    {
        SortAlgorithm.Algorithms.PairwiseSortingNetwork.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void PairwiseSortingNetworkBranchless()
    {
        SortAlgorithm.Algorithms.PairwiseSortingNetworkBranchless.Sort(_buffers.Next().AsSpan());
    }
}
