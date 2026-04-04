namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class HeapBenchmark
{
    [Params(256, 1024, 8192)]
    public int Size { get; set; }

    [Params(DataPattern.Random, DataPattern.SingleElementMoved, DataPattern.Sorted, DataPattern.Reversed, DataPattern.PipeOrgan)]
    public DataPattern Pattern { get; set; }

    private int[] _template = default!;
    private int[] _heapArray = default!;
    private int[] _minheapArray = default!;
    private int[] _ternaryHeapArray = default!;
    private int[] _bottomupHeapArray = default!;
    private int[] _weakHeapArray = default!;
    private int[] _smoothArray = default!;
    private int[] _tournamentArray = default!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _template = BenchmarkData.GenerateIntArray(Size, Pattern);
        _heapArray = new int[Size];
        _minheapArray = new int[Size];
        _ternaryHeapArray = new int[Size];
        _bottomupHeapArray = new int[Size];
        _weakHeapArray = new int[Size];
        _smoothArray = new int[Size];
        _tournamentArray = new int[Size];
    }

    [IterationSetup]
    public void Setup()
    {
        _template.CopyTo(_heapArray, 0);
        _template.CopyTo(_minheapArray, 0);
        _template.CopyTo(_ternaryHeapArray, 0);
        _template.CopyTo(_bottomupHeapArray, 0);
        _template.CopyTo(_weakHeapArray, 0);
        _template.CopyTo(_smoothArray, 0);
        _template.CopyTo(_tournamentArray, 0);
    }

    [Benchmark(Baseline = true)]
    public void HeapSort()
    {
        SortAlgorithm.Algorithms.HeapSort.Sort(_heapArray.AsSpan());
    }

    [Benchmark]
    public void MinHeapSort()
    {
        SortAlgorithm.Algorithms.MinHeapSort.Sort(_minheapArray.AsSpan());
    }

    [Benchmark]
    public void TernaryHeapSort()
    {
        SortAlgorithm.Algorithms.TernaryHeapSort.Sort(_ternaryHeapArray.AsSpan());
    }

    [Benchmark]
    public void BottomupHeapSort()
    {
        SortAlgorithm.Algorithms.BottomupHeapSort.Sort(_bottomupHeapArray.AsSpan());
    }

    [Benchmark]
    public void WeakHeapSort()
    {
        SortAlgorithm.Algorithms.WeakHeapSort.Sort(_weakHeapArray.AsSpan());
    }

    [Benchmark]
    public void SmoothSort()
    {
        SortAlgorithm.Algorithms.SmoothSort.Sort(_smoothArray.AsSpan());
    }

    [Benchmark]
    public void TournamentSort()
    {
        SortAlgorithm.Algorithms.TournamentSort.Sort(_tournamentArray.AsSpan());
    }
}
