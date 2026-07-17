namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class DistributionBenchmark
{
    [Params(256, 1024, 8192)]
    public int Size { get; set; }

    [Params(DataPattern.Random, DataPattern.SingleElementMoved, DataPattern.Sorted, DataPattern.Reversed, DataPattern.PipeOrgan)]
    public DataPattern Pattern { get; set; }

    private SortBuffers<int> _buffers = default!;

    // Restore cost stays out of the timed region: [IterationSetup] refreshes a pool of
    // pre-copied buffers and each invocation sorts a fresh one (see SortBuffers<T>).
    // Program.cs pins the job's InvocationCount to the pool size.
    [GlobalSetup]
    public void Setup()
    {
        _buffers = new SortBuffers<int>(BenchmarkData.GenerateIntArray(Size, Pattern));
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
    public void RadixLSD256Sort()
    {
        SortAlgorithm.Algorithms.RadixLSD256Sort.Sort(_buffers.Next().AsSpan());
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
    public void SpreadSort()
    {
        SortAlgorithm.Algorithms.SpreadSort.Sort(_buffers.Next().AsSpan());
    }
}
