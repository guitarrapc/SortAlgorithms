namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class NetworkBenchmark
{
    [Params(256, 1024, 4096)]
    public int Size { get; set; }

    [Params(DataPattern.Random, DataPattern.SingleElementMoved, DataPattern.Sorted, DataPattern.Reversed, DataPattern.PipeOrgan)]
    public DataPattern Pattern { get; set; }

    private int[] _template = default!;
    private int[] _batcheroddevenmergeArray = default!;
    private int[] _bionicArray = default!;
    private int[] _bionicRecursiveArray = default!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _template = BenchmarkData.GenerateIntArray(Size, Pattern);
        _batcheroddevenmergeArray = new int[Size];
        _bionicArray = new int[Size];
        _bionicRecursiveArray = new int[Size];
    }

    [IterationSetup]
    public void Setup()
    {
        _template.CopyTo(_batcheroddevenmergeArray, 0);
        _template.CopyTo(_bionicArray, 0);
        _template.CopyTo(_bionicRecursiveArray, 0);
    }

    [Benchmark(Baseline = true)]
    public void BitonicSort()
    {
        SortAlgorithm.Algorithms.BitonicSort.Sort(_bionicArray.AsSpan());
    }

    [Benchmark]
    public void BitonicRecursiveSort()
    {
        SortAlgorithm.Algorithms.BitonicSortNonOptimized.Sort(_bionicRecursiveArray.AsSpan());
    }

    [Benchmark]
    public void BatcherOddEvenMergeSort()
    {
        SortAlgorithm.Algorithms.BatcherOddEvenMergeSort.Sort(_bionicRecursiveArray.AsSpan());
    }
}
