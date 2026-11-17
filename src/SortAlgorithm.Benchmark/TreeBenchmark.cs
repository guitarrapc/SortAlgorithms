namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class TreeBenchmark
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

    [Benchmark]
    public void BalancedBinaryTreeSort()
    {
        SortAlgorithm.Algorithms.BalancedBinaryTreeSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void BTreeSort()
    {
        SortAlgorithm.Algorithms.BTreeSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void BPlusTreeSort()
    {
        SortAlgorithm.Algorithms.BPlusTreeSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void CartesianTreeSort()
    {
        SortAlgorithm.Algorithms.CartesianTreeSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark(Baseline = true)]
    public void BinaryTreeSort()
    {
        SortAlgorithm.Algorithms.BinaryTreeSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void SplaySort()
    {
        SortAlgorithm.Algorithms.SplaySort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void TreapSort()
    {
        SortAlgorithm.Algorithms.TreapSort.Sort(_buffers.Next().AsSpan());
    }
}
