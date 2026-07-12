namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class NetworkBenchmark
{
    [Params(256, 1024, 4096)]
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
    public void BitonicSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.BitonicSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void BitonicRecursiveSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.BitonicSortNonOptimized.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void BatcherOddEvenMergeSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.BatcherOddEvenMergeSort.Sort(_work.AsSpan());
    }
}
