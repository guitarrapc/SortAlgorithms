namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class SelectionBenchmark
{
    [Params(256, 1024)]
    public int Size { get; set; }

    [Params(DataPattern.Random, DataPattern.Sorted, DataPattern.Reversed, DataPattern.AntiQuicksort)]
    public DataPattern Pattern { get; set; }

    private int[] _cycleArray = default!;
    private int[] _doubleSelectionArray = default!;
    private int[] _pancakeArray = default!;
    private int[] _selectionArray = default!;

    [IterationSetup]
    public void Setup()
    {
        _cycleArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _doubleSelectionArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _pancakeArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _selectionArray = BenchmarkData.GenerateIntArray(Size, Pattern);
    }

    [Benchmark]
    public void CycleSort()
    {
        SortAlgorithm.Algorithms.CycleSort.Sort(_cycleArray.AsSpan());
    }

    [Benchmark]
    public void DoubleSelectionSort()
    {
        SortAlgorithm.Algorithms.DoubleSelectionSort.Sort(_cycleArray.AsSpan());
    }

    [Benchmark]
    public void PancakeSort()
    {
        SortAlgorithm.Algorithms.PancakeSort.Sort(_pancakeArray.AsSpan());
    }

    [Benchmark(Baseline = true)]
    public void SelectionSort()
    {
        SortAlgorithm.Algorithms.SelectionSort.Sort(_selectionArray.AsSpan());
    }
}
