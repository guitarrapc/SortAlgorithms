namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class ExchangeBenchmark
{
    [Params(256, 1024)]
    public int Size { get; set; }

    [Params(DataPattern.Random, DataPattern.SingleElementMoved, DataPattern.Sorted, DataPattern.Reversed, DataPattern.PipeOrgan)]
    public DataPattern Pattern { get; set; }

    private int[] _template = default!;
    private int[] _bubbleArray = default!;
    private int[] _cocktailShakerArray = default!;
    private int[] _oddEvenArray = default!;
    private int[] _combArray = default!;
    private int[] _circleArray = default!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _template = BenchmarkData.GenerateIntArray(Size, Pattern);
        _bubbleArray = new int[Size];
        _cocktailShakerArray = new int[Size];
        _oddEvenArray = new int[Size];
        _combArray = new int[Size];
        _circleArray = new int[Size];
    }

    [IterationSetup]
    public void Setup()
    {
        _template.CopyTo(_bubbleArray, 0);
        _template.CopyTo(_cocktailShakerArray, 0);
        _template.CopyTo(_oddEvenArray, 0);
        _template.CopyTo(_combArray, 0);
        _template.CopyTo(_circleArray, 0);
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
    public void OddEvenSort()
    {
        SortAlgorithm.Algorithms.OddEvenSort.Sort(_oddEvenArray.AsSpan());
    }

    [Benchmark]
    public void CombSort()
    {
        SortAlgorithm.Algorithms.CombSort.Sort(_combArray.AsSpan());
    }

    [Benchmark]
    public void CircleSort()
    {
        SortAlgorithm.Algorithms.CircleSort.Sort(_circleArray.AsSpan());
    }
}
