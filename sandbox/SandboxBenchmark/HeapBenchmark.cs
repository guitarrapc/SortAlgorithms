namespace SandboxBenchmark;

[MemoryDiagnoser]
[RankColumn]
public class HeapBenchmark
{
    [Params(256, 1024, 8192)]
    public int Size { get; set; }

    [Params(DataPattern.Random, DataPattern.Sorted, DataPattern.Reversed, DataPattern.AntiQuicksort)]
    public DataPattern Pattern { get; set; }

    private int[] _bottomupHeapArray = default!;
    private int[] _heapArray = default!;
    private int[] _smoothArray = default!;
    private int[] _ternaryHeapArray = default!;
    private int[] _weakHeapArray = default!;

    [IterationSetup]
    public void Setup()
    {
        _bottomupHeapArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _heapArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _smoothArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _ternaryHeapArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _weakHeapArray = BenchmarkData.GenerateIntArray(Size, Pattern);
    }

    [Benchmark]
    public void BottomupHeapSort()
    {
        SortAlgorithm.Algorithms.BottomupHeapSort.Sort(_bottomupHeapArray.AsSpan());
    }

    [Benchmark(Baseline = true)]
    public void HeapSort()
    {
        SortAlgorithm.Algorithms.HeapSort.Sort(_heapArray.AsSpan());
    }

    [Benchmark]
    public void SmoothSort()
    {
        SortAlgorithm.Algorithms.SmoothSort.Sort(_smoothArray.AsSpan());
    }

    [Benchmark]
    public void TernaryHeapSort()
    {
        SortAlgorithm.Algorithms.TernaryHeapSort.Sort(_ternaryHeapArray.AsSpan());
    }

    [Benchmark]
    public void WeakHeapSort()
    {
        SortAlgorithm.Algorithms.WeakHeapSort.Sort(_weakHeapArray.AsSpan());
    }
}
