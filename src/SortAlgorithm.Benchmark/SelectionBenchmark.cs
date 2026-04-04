namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class SelectionBenchmark
{
    [Params(256, 1024)]
    public int Size { get; set; }

    [Params(DataPattern.Random, DataPattern.SingleElementMoved, DataPattern.Sorted, DataPattern.Reversed, DataPattern.PipeOrgan)]
    public DataPattern Pattern { get; set; }

    private int[] _template = default!;
    private int[] _selectionArray = default!;
    private int[] _doubleSelectionArray = default!;
    private int[] _cycleArray = default!;
    private int[] _pancakeArray = default!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _template = BenchmarkData.GenerateIntArray(Size, Pattern);
        _selectionArray = new int[Size];
        _doubleSelectionArray = new int[Size];
        _cycleArray = new int[Size];
        _pancakeArray = new int[Size];
    }

    [IterationSetup]
    public void Setup()
    {
        _template.CopyTo(_selectionArray, 0);
        _template.CopyTo(_doubleSelectionArray, 0);
        _template.CopyTo(_cycleArray, 0);
        _template.CopyTo(_pancakeArray, 0);
    }

    [Benchmark(Baseline = true)]
    public void SelectionSort()
    {
        SortAlgorithm.Algorithms.SelectionSort.Sort(_selectionArray.AsSpan());
    }

    [Benchmark]
    public void DoubleSelectionSort()
    {
        SortAlgorithm.Algorithms.DoubleSelectionSort.Sort(_cycleArray.AsSpan());
    }

    [Benchmark]
    public void CycleSort()
    {
        SortAlgorithm.Algorithms.CycleSort.Sort(_cycleArray.AsSpan());
    }

    [Benchmark]
    public void PancakeSort()
    {
        SortAlgorithm.Algorithms.PancakeSort.Sort(_pancakeArray.AsSpan());
    }
}
