namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class PartitionBenchmark
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
    public void QuickSort()
    {
        SortAlgorithm.Algorithms.QuickSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void QuickSort3way()
    {
        SortAlgorithm.Algorithms.QuickSort3way.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void QuickSortMedian3()
    {
        SortAlgorithm.Algorithms.QuickSortMedian3.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void QuickSortMedian9()
    {
        SortAlgorithm.Algorithms.QuickSortMedian9.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void SampleSort()
    {
        SortAlgorithm.Algorithms.SampleSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void SimdQuickSort()
    {
        SortAlgorithm.Algorithms.SimdQuickSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void DualPivotQuickSort()
    {
        SortAlgorithm.Algorithms.DualPivotQuickSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void StableQuickSort()
    {
        SortAlgorithm.Algorithms.StableQuickSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void BidirectionalStableQuickSort()
    {
        SortAlgorithm.Algorithms.BidirectionalStableQuickSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void DestswapStableQuickSort()
    {
        SortAlgorithm.Algorithms.DestswapStableQuickSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void IntroSort()
    {
        SortAlgorithm.Algorithms.IntroSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void IntroSortDotnet()
    {
        SortAlgorithm.Algorithms.IntroSortDotnet.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void PDQSort()
    {
        SortAlgorithm.Algorithms.PDQSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void PDQSortBranchless()
    {
        SortAlgorithm.Algorithms.PDQSortBranchless.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void Ipnsort()
    {
        SortAlgorithm.Algorithms.Ipnsort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void StdSort()
    {
        SortAlgorithm.Algorithms.StdSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void BlockQuickSort()
    {
        SortAlgorithm.Algorithms.BlockQuickSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void Ips4oSort()
    {
        SortAlgorithm.Algorithms.Ips4oSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void FluxSort()
    {
        SortAlgorithm.Algorithms.FluxSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void CrumSort()
    {
        SortAlgorithm.Algorithms.CrumSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void BlitSort()
    {
        SortAlgorithm.Algorithms.BlitSort.Sort(_buffers.Next().AsSpan());
    }

    // Same call in every benchmark class: a shared reference for comparing results across classes.
    [Benchmark]
    public void DotnetSort()
    {
        _buffers.Next().AsSpan().Sort();
    }
}
