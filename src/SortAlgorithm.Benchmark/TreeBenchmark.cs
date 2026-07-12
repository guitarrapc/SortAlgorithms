namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class TreeBenchmark
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

    [Benchmark]
    public void BalancedBinaryTreeSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.BalancedBinaryTreeSort.Sort(_work.AsSpan());
    }

    [Benchmark(Baseline = true)]
    public void BinaryTreeSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.BinaryTreeSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void SplaySort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.SplaySort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void TreapSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.TreapSort.Sort(_work.AsSpan());
    }
}
