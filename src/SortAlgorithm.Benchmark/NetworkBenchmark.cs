namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class NetworkBenchmark
{
    [Params(256, 1024, 4096)]
    public int Size { get; set; }

    [Params(DataPattern.Random, DataPattern.Sorted, DataPattern.Reversed, DataPattern.AntiQuicksort)]
    public DataPattern Pattern { get; set; }

    private int[] _bionicArray = default!;
    private int[] _bionicRecursiveArray = default!;

    [IterationSetup]
    public void Setup()
    {
        _bionicArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _bionicRecursiveArray = BenchmarkData.GenerateIntArray(Size, Pattern);
    }

    [Benchmark(Baseline = true)]
    public void BitonicSort()
    {
        SortAlgorithm.Algorithms.BitonicSort.Sort(_bionicArray.AsSpan());
    }

    [Benchmark(Baseline = true)]
    public void BitonicRecursiveSort()
    {
        SortAlgorithm.Algorithms.BitonicSortNonOptimized.Sort(_bionicRecursiveArray.AsSpan());
    }
}
