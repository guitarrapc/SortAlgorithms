namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class ExchangeBenchmark
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
    public void BubbleSort()
    {
        SortAlgorithm.Algorithms.BubbleSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void CocktailShakerSort()
    {
        SortAlgorithm.Algorithms.CocktailShakerSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void OddEvenSort()
    {
        SortAlgorithm.Algorithms.OddEvenSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void CombSort()
    {
        SortAlgorithm.Algorithms.CombSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void CircleSort()
    {
        SortAlgorithm.Algorithms.CircleSort.Sort(_buffers.Next().AsSpan());
    }
}
