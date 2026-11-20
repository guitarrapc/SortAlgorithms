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

/// <summary>
/// The SIMD selection sorts against their scalar versions at larger sizes. Both pairs stay Θ(n²), so the scalar baselines grow
/// to about 13 ms (SelectionSort) and 6 ms (DoubleSelectionSort) per run at 8192 random keys; the rest of the family stays in
/// <see cref="SelectionBenchmark"/> at its smaller sizes.
/// </summary>
[MemoryDiagnoser]
[RankColumn]
public class SelectionSimdBenchmark
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
    public void SelectionSort()
    {
        SortAlgorithm.Algorithms.SelectionSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void SelectionSortSimd()
    {
        SortAlgorithm.Algorithms.SelectionSortSimd.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void DoubleSelectionSort()
    {
        SortAlgorithm.Algorithms.DoubleSelectionSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void DoubleSelectionSortSimd()
    {
        SortAlgorithm.Algorithms.DoubleSelectionSortSimd.Sort(_buffers.Next().AsSpan());
    }
}
