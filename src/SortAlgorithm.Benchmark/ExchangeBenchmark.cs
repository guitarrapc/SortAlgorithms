namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class ExchangeBenchmark
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
    public void BubbleSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.BubbleSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void CocktailShakerSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.CocktailShakerSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void OddEvenSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.OddEvenSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void CombSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.CombSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void CircleSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.CircleSort.Sort(_work.AsSpan());
    }
}
