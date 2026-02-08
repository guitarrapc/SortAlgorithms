namespace SandboxBenchmark;

[MemoryDiagnoser]
[RankColumn]
public class TreeBenchmark
{
    [Params(256, 1024)]
    public int Size { get; set; }

    [Params(DataPattern.Random, DataPattern.Sorted, DataPattern.Reversed, DataPattern.AntiQuicksort)]
    public DataPattern Pattern { get; set; }

    private int[] _balancedbinarytreeArray = default!;
    private int[] _binarytreeArray = default!;

    [IterationSetup]
    public void Setup()
    {
        _balancedbinarytreeArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _binarytreeArray = BenchmarkData.GenerateIntArray(Size, Pattern);
    }

    [Benchmark]
    public void BalancedBinaryTreeSort()
    {
        SortAlgorithm.Algorithms.BalancedBinaryTreeSort.Sort(_balancedbinarytreeArray.AsSpan());
    }

    [Benchmark(Baseline = true)]
    public void BinaryTreeSort()
    {
        SortAlgorithm.Algorithms.BinaryTreeSort.Sort(_binarytreeArray.AsSpan());
    }
}
