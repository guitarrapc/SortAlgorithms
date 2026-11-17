namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class SelectionBenchmark
{
    [Params(256, 1024)]
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
    public void SelectionSort()
    {
        SortAlgorithm.Algorithms.SelectionSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void DoubleSelectionSort()
    {
        SortAlgorithm.Algorithms.DoubleSelectionSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void CycleSort()
    {
        SortAlgorithm.Algorithms.CycleSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void PancakeSort()
    {
        SortAlgorithm.Algorithms.PancakeSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void BingoSort()
    {
        SortAlgorithm.Algorithms.BingoSort.Sort(_buffers.Next().AsSpan());
    }
}
