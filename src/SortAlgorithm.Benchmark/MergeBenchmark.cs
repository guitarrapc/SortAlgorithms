namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class MergeBenchmark
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

    [Benchmark(Baseline = true)]
    public void MergeSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.MergeSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void PingpongMergeSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.PingpongMergeSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void BottomupMergeSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.BottomupMergeSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void StdStableSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.StdStableSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void RotateMergeSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.RotateMergeSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void RotateMergeSortRecursive()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.RotateMergeSortRecursive.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void SymMergeSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.SymMergeSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void BlockMergeSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.BlockMergeSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void NaturalMergeSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.NaturalMergeSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void TimSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.TimSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void PowerSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.PowerSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void ShiftSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.ShiftSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void SpinSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.SpinSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void SpinSortVariant()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.SpinSortVariant.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void Glidesort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.Glidesort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void FlatStableSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.FlatStableSort.Sort(_work.AsSpan());
    }
}
