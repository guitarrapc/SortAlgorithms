namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class DistributionBenchmark
{
    [Params(256, 1024, 8192)]
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
    public void CountingSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.CountingSort.Sort(_work.AsSpan(), x => x);
    }

    [Benchmark(Baseline = true)]
    public void CountingSortInteger()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.CountingSortInteger.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void PigeonSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.PigeonholeSort.Sort(_work.AsSpan(), x => x);
    }

    [Benchmark]
    public void PigeonSortInteger()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.PigeonholeSortInteger.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void BucketSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.BucketSort.Sort(_work.AsSpan(), x => x);
    }

    [Benchmark]
    public void BucketSortInteger()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.BucketSortInteger.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void FlashSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.FlashSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void RadixLSD4Sort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.RadixLSD4Sort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void RadixLSD256Sort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.RadixLSD256Sort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void RadixLSD10Sort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.RadixLSD10Sort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void RadixMSD4Sort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.RadixMSD4Sort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void RadixMSD10Sort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.RadixMSD10Sort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void AmericanFlagSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.AmericanFlagSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void SpreadSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.SpreadSort.Sort(_work.AsSpan());
    }
}
