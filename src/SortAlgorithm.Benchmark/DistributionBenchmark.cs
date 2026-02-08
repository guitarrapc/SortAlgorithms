namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class DistributionBenchmark
{
    [Params(256, 1024)]
    public int Size { get; set; }

    [Params(DataPattern.Random, DataPattern.Sorted, DataPattern.Reversed, DataPattern.AntiQuicksort)]
    public DataPattern Pattern { get; set; }

    private int[] _americanflagArray = default!;
    private int[] _bucketArray = default!;
    private int[] _countingArray = default!;
    private int[] _pigeonholeArray = default!;
    private int[] _radixLSD4Sort = default!;
    private int[] _radixLSD256Sort = default!;
    private int[] _radixLSD10Sort = default!;
    private int[] _radixMSD4Sort = default!;
    private int[] _radixMSD10Sort = default!;

    [IterationSetup]
    public void Setup()
    {
        _americanflagArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _bucketArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _countingArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _pigeonholeArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _radixLSD4Sort = BenchmarkData.GenerateIntArray(Size, Pattern);
        _radixLSD256Sort = BenchmarkData.GenerateIntArray(Size, Pattern);
        _radixLSD10Sort = BenchmarkData.GenerateIntArray(Size, Pattern);
        _radixMSD4Sort = BenchmarkData.GenerateIntArray(Size, Pattern);
        _radixMSD10Sort = BenchmarkData.GenerateIntArray(Size, Pattern);
    }

    [Benchmark]
    public void AmericanFlagSort()
    {
        SortAlgorithm.Algorithms.AmericanFlagSort.Sort(_americanflagArray.AsSpan());
    }

    [Benchmark]
    public void BucketSort()
    {
        SortAlgorithm.Algorithms.BucketSortInteger.Sort(_bucketArray.AsSpan());
    }

    [Benchmark(Baseline = true)]
    public void CountingSort()
    {
        SortAlgorithm.Algorithms.CountingSortInteger.Sort(_countingArray.AsSpan());
    }

    [Benchmark]
    public void PigeonSort()
    {
        SortAlgorithm.Algorithms.PigeonholeSortInteger.Sort(_countingArray.AsSpan());
    }

    [Benchmark]
    public void RadixLSD4Sort()
    {
        SortAlgorithm.Algorithms.RadixLSD4Sort.Sort(_radixLSD4Sort.AsSpan());
    }

    [Benchmark]
    public void RadixLSD256Sort()
    {
        SortAlgorithm.Algorithms.RadixLSD256Sort.Sort(_radixLSD256Sort.AsSpan());
    }

    [Benchmark]
    public void RadixLSD10Sort()
    {
        SortAlgorithm.Algorithms.RadixLSD10Sort.Sort(_radixLSD10Sort.AsSpan());
    }

    [Benchmark]
    public void RadixMSD4Sort()
    {
        SortAlgorithm.Algorithms.RadixMSD4Sort.Sort(_radixMSD4Sort.AsSpan());
    }

    [Benchmark]
    public void RadixMSD10Sort()
    {
        SortAlgorithm.Algorithms.RadixMSD10Sort.Sort(_radixMSD10Sort.AsSpan());
    }
}
