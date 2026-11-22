namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class InsertionBenchmark
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
    public void InsertionSort()
    {
        SortAlgorithm.Algorithms.InsertionSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void PairInsertionSort()
    {
        SortAlgorithm.Algorithms.PairInsertionSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void BinaryInsertSort()
    {
        SortAlgorithm.Algorithms.BinaryInsertionSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void GnomeSort()
    {
        SortAlgorithm.Algorithms.GnomeSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void LibrarySort()
    {
        SortAlgorithm.Algorithms.LibrarySort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void MergeInsertionSort()
    {
        SortAlgorithm.Algorithms.MergeInsertionSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void ShellSortKnuth1973()
    {
        SortAlgorithm.Algorithms.ShellSortKnuth1973.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void ShellSortSedgewick1986()
    {
        SortAlgorithm.Algorithms.ShellSortSedgewick1986.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void ShellSortTokuda1992()
    {
        SortAlgorithm.Algorithms.ShellSortTokuda1992.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void ShellSortCiura2001()
    {
        SortAlgorithm.Algorithms.ShellSortCiura2001.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void ShellSortLee2021()
    {
        SortAlgorithm.Algorithms.ShellSortLee2021.Sort(_buffers.Next().AsSpan());
    }

    // Same call in every benchmark class: a shared reference for comparing results across classes.
    [Benchmark]
    public void DotnetSort()
    {
        _buffers.Next().AsSpan().Sort();
    }
}
