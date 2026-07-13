namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class MergeBenchmark
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

    [Benchmark(Baseline = true)]
    public void MergeSort()
    {
        SortAlgorithm.Algorithms.MergeSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void PingpongMergeSort()
    {
        SortAlgorithm.Algorithms.PingpongMergeSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void BottomupMergeSort()
    {
        SortAlgorithm.Algorithms.BottomupMergeSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void StdStableSort()
    {
        SortAlgorithm.Algorithms.StdStableSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void RotateMergeSort()
    {
        SortAlgorithm.Algorithms.RotateMergeSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void RotateMergeSortRecursive()
    {
        SortAlgorithm.Algorithms.RotateMergeSortRecursive.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void SymMergeSort()
    {
        SortAlgorithm.Algorithms.SymMergeSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void BlockMergeSort()
    {
        SortAlgorithm.Algorithms.BlockMergeSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void NaturalMergeSort()
    {
        SortAlgorithm.Algorithms.NaturalMergeSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void TimSort()
    {
        SortAlgorithm.Algorithms.TimSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void PowerSort()
    {
        SortAlgorithm.Algorithms.PowerSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void ShiftSort()
    {
        SortAlgorithm.Algorithms.ShiftSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void SpinSort()
    {
        SortAlgorithm.Algorithms.SpinSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void SpinSortVariant()
    {
        SortAlgorithm.Algorithms.SpinSortVariant.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void Glidesort()
    {
        SortAlgorithm.Algorithms.Glidesort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void FlatStableSort()
    {
        SortAlgorithm.Algorithms.FlatStableSort.Sort(_buffers.Next().AsSpan());
    }
}
