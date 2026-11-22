namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class DistributionBenchmark
{
    [Params(256, 1024, 4096, 8192)]
    public int Size { get; set; }

    [Params(DataPattern.Random, DataPattern.SingleElementMoved, DataPattern.Sorted, DataPattern.Reversed, DataPattern.PipeOrgan, DataPattern.ManyDuplicates)]
    public DataPattern Pattern { get; set; }

    private SortBuffers<int> _buffers = default!;

    // Each invocation sorts its own pre-copied buffer, so the copy is not timed (see SortBuffers<T>).
    [GlobalSetup]
    public void Setup()
    {
        _buffers = new SortBuffers<int>(seed => BenchmarkData.GenerateIntArray(Size, Pattern, seed));
    }

    [IterationSetup]
    public void IterationSetup() => _buffers.Reset();

    [Benchmark]
    public void CountingSort()
    {
        SortAlgorithm.Algorithms.CountingSort.SortBy(_buffers.Next().AsSpan(), x => x);
    }

    [Benchmark(Baseline = true)]
    public void CountingSortInteger()
    {
        SortAlgorithm.Algorithms.CountingSortInteger.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void PigeonSort()
    {
        SortAlgorithm.Algorithms.PigeonholeSort.SortBy(_buffers.Next().AsSpan(), x => x);
    }

    [Benchmark]
    public void PigeonSortInteger()
    {
        SortAlgorithm.Algorithms.PigeonholeSortInteger.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void BucketSort()
    {
        SortAlgorithm.Algorithms.BucketSort.SortBy(_buffers.Next().AsSpan(), x => x);
    }

    [Benchmark]
    public void BucketSortInteger()
    {
        SortAlgorithm.Algorithms.BucketSortInteger.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void FlashSort()
    {
        SortAlgorithm.Algorithms.FlashSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void RadixLSD4Sort()
    {
        SortAlgorithm.Algorithms.RadixLSD4Sort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void RotateLSDRadixSort()
    {
        SortAlgorithm.Algorithms.RotateLSDRadixSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void RadixLSD256Sort()
    {
        SortAlgorithm.Algorithms.RadixLSD256Sort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void Radsort()
    {
        SortAlgorithm.Algorithms.Radsort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void RadixLSD10Sort()
    {
        SortAlgorithm.Algorithms.RadixLSD10Sort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void RadixMSD4Sort()
    {
        SortAlgorithm.Algorithms.RadixMSD4Sort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void RadixMSD10Sort()
    {
        SortAlgorithm.Algorithms.RadixMSD10Sort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void AmericanFlagSort()
    {
        SortAlgorithm.Algorithms.AmericanFlagSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void SkaSort()
    {
        SortAlgorithm.Algorithms.SkaSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void SpreadSort()
    {
        SortAlgorithm.Algorithms.SpreadSort.Sort(_buffers.Next().AsSpan());
    }

    // Same call in every benchmark class: a shared reference for comparing results across classes.
    [Benchmark]
    public void DotnetSort()
    {
        _buffers.Next().AsSpan().Sort();
    }
}
