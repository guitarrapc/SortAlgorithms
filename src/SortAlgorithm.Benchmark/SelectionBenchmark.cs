namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class SelectionBenchmark
{
    [Params(256, 1024)]
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
    public void SelectionSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.SelectionSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void DoubleSelectionSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.DoubleSelectionSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void CycleSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.CycleSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void PancakeSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.PancakeSort.Sort(_work.AsSpan());
    }
}
