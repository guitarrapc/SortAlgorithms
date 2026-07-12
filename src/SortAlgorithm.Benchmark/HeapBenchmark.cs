namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class HeapBenchmark
{
    [Params(256, 1024, 8192)]
    public int Size { get; set; }

    [Params(DataPattern.Random, DataPattern.SingleElementMoved, DataPattern.Sorted, DataPattern.Reversed, DataPattern.PipeOrgan)]
    public DataPattern Pattern { get; set; }

    private int[] _pristine = default!;
    private int[] _work = default!;

    // GlobalSetup + per-invocation copy instead of IterationSetup: IterationSetup forces
    // InvocationCount=1, losing precision for µs-scale workloads. The copy cost is
    // identical for every benchmark method, so relative comparisons are unaffected.
    [GlobalSetup]
    public void Setup()
    {
        _pristine = BenchmarkData.GenerateIntArray(Size, Pattern);
        _work = new int[Size];
    }

    [Benchmark(Baseline = true)]
    public void HeapSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.HeapSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void MinHeapSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.MinHeapSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void TernaryHeapSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.TernaryHeapSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void BottomupHeapSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.BottomupHeapSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void WeakHeapSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.WeakHeapSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void SmoothSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.SmoothSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void TournamentSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.TournamentSort.Sort(_work.AsSpan());
    }
}
