namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class AdaptiveBenchmark
{
    [Params(256, 1024, 8192)]
    public int Size { get; set; }

    [Params(DataPattern.Random, DataPattern.SingleElementMoved, DataPattern.Sorted, DataPattern.Reversed, DataPattern.PipeOrgan)]
    public DataPattern Pattern { get; set; }

    private int[] _template = default!;
    private int[] _dropMergeArray = default!;
    private int[] _patienceArray = default!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _template = BenchmarkData.GenerateIntArray(Size, Pattern);
        _dropMergeArray = new int[Size];
        _patienceArray = new int[Size];
    }

    [IterationSetup]
    public void Setup()
    {
        _template.CopyTo(_dropMergeArray, 0);
        _template.CopyTo(_patienceArray, 0);
    }

    [Benchmark(Baseline = true)]
    public void DropMergeSort()
    {
        SortAlgorithm.Algorithms.DropMergeSort.Sort(_dropMergeArray.AsSpan());
    }

    [Benchmark]
    public void PatienceSort()
    {
        SortAlgorithm.Algorithms.PatienceSort.Sort(_patienceArray.AsSpan());
    }
}

[MemoryDiagnoser]
[RankColumn]
public class AdaptiveSlowBenchmark
{
    [Params(256, 1024)]
    public int Size { get; set; }

    [Params(DataPattern.Random, DataPattern.SingleElementMoved, DataPattern.Sorted, DataPattern.Reversed, DataPattern.PipeOrgan)]
    public DataPattern Pattern { get; set; }

    private int[] _template = default!;
    private int[] _strandArray = default!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _template = BenchmarkData.GenerateIntArray(Size, Pattern);
        _strandArray = new int[Size];
    }

    [IterationSetup]
    public void Setup()
    {
        _template.CopyTo(_strandArray, 0);
    }

    [Benchmark(Baseline = true)]
    public void StrandSort()
    {
        SortAlgorithm.Algorithms.StrandSort.Sort(_strandArray.AsSpan());
    }
}
