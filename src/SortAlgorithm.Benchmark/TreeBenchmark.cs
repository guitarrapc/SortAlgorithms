namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class TreeBenchmark
{
    [Params(256, 1024)]
    public int Size { get; set; }

    [Params(DataPattern.Random, DataPattern.SingleElementMoved, DataPattern.Sorted, DataPattern.Reversed, DataPattern.PipeOrgan)]
    public DataPattern Pattern { get; set; }

    private int[] _template = default!;
    private int[] _balancedbinarytreeArray = default!;
    private int[] _binarytreeArray = default!;
    private int[] _splayArray = default!;
    private int[] _treapArray = default!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _template = BenchmarkData.GenerateIntArray(Size, Pattern);
        _balancedbinarytreeArray = new int[Size];
        _binarytreeArray = new int[Size];
        _splayArray = new int[Size];
        _treapArray = new int[Size];
    }

    [IterationSetup]
    public void Setup()
    {
        _template.CopyTo(_balancedbinarytreeArray, 0);
        _template.CopyTo(_binarytreeArray, 0);
        _template.CopyTo(_splayArray, 0);
        _template.CopyTo(_treapArray, 0);
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

    [Benchmark]
    public void SplaySort()
    {
        SortAlgorithm.Algorithms.SplaySort.Sort(_splayArray.AsSpan());
    }

    [Benchmark]
    public void TreapSort()
    {
        SortAlgorithm.Algorithms.TreapSort.Sort(_treapArray.AsSpan());
    }
}
