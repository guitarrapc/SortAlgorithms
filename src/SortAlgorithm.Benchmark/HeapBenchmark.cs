namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class HeapBenchmark
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
    public void PoplarHeapSort()
    {
        SortAlgorithm.Algorithms.PoplarHeapSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void TournamentSort()
    {
        SortAlgorithm.Algorithms.TournamentSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void BinomialHeapSort()
    {
        SortAlgorithm.Algorithms.BinomialHeapSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void PairingHeapSort()
    {
        SortAlgorithm.Algorithms.PairingHeapSort.Sort(_buffers.Next().AsSpan());
    }
}
