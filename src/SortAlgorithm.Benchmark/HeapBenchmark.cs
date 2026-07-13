namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class HeapBenchmark
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
    public void HeapSort()
    {
        SortAlgorithm.Algorithms.HeapSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void MinHeapSort()
    {
        SortAlgorithm.Algorithms.MinHeapSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void TernaryHeapSort()
    {
        SortAlgorithm.Algorithms.TernaryHeapSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void BottomupHeapSort()
    {
        SortAlgorithm.Algorithms.BottomupHeapSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void WeakHeapSort()
    {
        SortAlgorithm.Algorithms.WeakHeapSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void SmoothSort()
    {
        SortAlgorithm.Algorithms.SmoothSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void TournamentSort()
    {
        SortAlgorithm.Algorithms.TournamentSort.Sort(_buffers.Next().AsSpan());
    }
}
