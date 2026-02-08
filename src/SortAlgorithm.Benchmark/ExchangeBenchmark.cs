namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class ExchangeBenchmark
{
    [Params(256, 1024)]
    public int Size { get; set; }

    [Params(DataPattern.Random, DataPattern.Sorted, DataPattern.Reversed, DataPattern.AntiQuicksort)]
    public DataPattern Pattern { get; set; }

    private int[] _bubbleArray = default!;
    private int[] _cocktailShakerArray = default!;
    private int[] _combArray = default!;
    private int[] _oddEventArray = default!;

    [IterationSetup]
    public void Setup()
    {
        _bubbleArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _cocktailShakerArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _combArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _oddEventArray = BenchmarkData.GenerateIntArray(Size, Pattern);
    }

    [Benchmark(Baseline = true)]
    public void BubbleSort()
    {
        SortAlgorithm.Algorithms.BubbleSort.Sort(_bubbleArray.AsSpan());
    }

    [Benchmark]
    public void CocktailShakerSort()
    {
        SortAlgorithm.Algorithms.CocktailShakerSort.Sort(_cocktailShakerArray.AsSpan());
    }

    [Benchmark]
    public void CombSort()
    {
        SortAlgorithm.Algorithms.CombSort.Sort(_combArray.AsSpan());
    }

    [Benchmark]
    public void OddEventSort()
    {
        SortAlgorithm.Algorithms.OddEvenSort.Sort(_oddEventArray.AsSpan());
    }
}
