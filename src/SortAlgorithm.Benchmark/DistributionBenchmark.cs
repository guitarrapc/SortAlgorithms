namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class DistributionBenchmark
{
    [Params(256, 1024, 8192)]
    public int Size { get; set; }

    [Params(DataPattern.Random, DataPattern.SingleElementMoved, DataPattern.Sorted, DataPattern.Reversed, DataPattern.PipeOrgan)]
    public DataPattern Pattern { get; set; }

    private int[] _template = default!;
    private int[] _countingArray = default!;
    private int[] _countingIntegerArray = default!;
    private int[] _pigeonholeArray = default!;
    private int[] _pigeonholeIntegerArray = default!;
    private int[] _bucketArray = default!;
    private int[] _bucketIntegerArray = default!;
    private int[] _flashArray = default!;
    private int[] _radixLSD4Sort = default!;
    private int[] _radixLSD256Sort = default!;
    private int[] _radixLSD10Sort = default!;
    private int[] _radixMSD4Sort = default!;
    private int[] _radixMSD10Sort = default!;
    private int[] _americanflagArray = default!;
    private int[] _spreadArray = default!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _template = BenchmarkData.GenerateIntArray(Size, Pattern);
        _countingArray = new int[Size];
        _countingIntegerArray = new int[Size];
        _pigeonholeArray = new int[Size];
        _pigeonholeIntegerArray = new int[Size];
        _bucketArray = new int[Size];
        _bucketIntegerArray = new int[Size];
        _flashArray = new int[Size];
        _radixLSD4Sort = new int[Size];
        _radixLSD256Sort = new int[Size];
        _radixLSD10Sort = new int[Size];
        _radixMSD4Sort = new int[Size];
        _radixMSD10Sort = new int[Size];
        _americanflagArray = new int[Size];
        _spreadArray = new int[Size];
    }

    [IterationSetup]
    public void Setup()
    {
        _template.CopyTo(_countingArray, 0);
        _template.CopyTo(_countingIntegerArray, 0);
        _template.CopyTo(_pigeonholeArray, 0);
        _template.CopyTo(_pigeonholeIntegerArray, 0);
        _template.CopyTo(_bucketArray, 0);
        _template.CopyTo(_bucketIntegerArray, 0);
        _template.CopyTo(_flashArray, 0);
        _template.CopyTo(_radixLSD4Sort, 0);
        _template.CopyTo(_radixLSD256Sort, 0);
        _template.CopyTo(_radixLSD10Sort, 0);
        _template.CopyTo(_radixMSD4Sort, 0);
        _template.CopyTo(_radixMSD10Sort, 0);
        _template.CopyTo(_americanflagArray, 0);
        _template.CopyTo(_spreadArray, 0);
    }

    [Benchmark]
    public void CountingSort()
    {
        SortAlgorithm.Algorithms.CountingSort.Sort(_countingArray.AsSpan(), x => x);
    }

    [Benchmark(Baseline = true)]
    public void CountingSortInteger()
    {
        SortAlgorithm.Algorithms.CountingSortInteger.Sort(_countingIntegerArray.AsSpan());
    }

    [Benchmark]
    public void PigeonSort()
    {
        SortAlgorithm.Algorithms.PigeonholeSortInteger.Sort(_pigeonholeArray.AsSpan());
    }

    [Benchmark]
    public void PigeonSortInteger()
    {
        SortAlgorithm.Algorithms.PigeonholeSortInteger.Sort(_pigeonholeIntegerArray.AsSpan());
    }

    [Benchmark]
    public void BucketSort()
    {
        SortAlgorithm.Algorithms.BucketSort.Sort(_bucketArray.AsSpan(), x => x);
    }

    [Benchmark]
    public void BucketSortInteger()
    {
        SortAlgorithm.Algorithms.BucketSortInteger.Sort(_bucketIntegerArray.AsSpan());
    }

    [Benchmark]
    public void FlashSort()
    {
        SortAlgorithm.Algorithms.FlashSort.Sort(_flashArray.AsSpan());
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

    [Benchmark]
    public void AmericanFlagSort()
    {
        SortAlgorithm.Algorithms.AmericanFlagSort.Sort(_americanflagArray.AsSpan());
    }

    [Benchmark]
    public void SpreadSort()
    {
        SortAlgorithm.Algorithms.SpreadSort.Sort(_spreadArray.AsSpan());
    }
}
