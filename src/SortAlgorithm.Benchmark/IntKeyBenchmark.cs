using SortAlgorithm.Utils;

namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class IntKeyBenchmark
{
    [Params(256, 1024, 8192)]
    public int Size { get; set; }

    [Params(DataPattern.Random, DataPattern.SingleElementMoved, DataPattern.Sorted, DataPattern.Reversed, DataPattern.PipeOrgan)]
    public DataPattern Pattern { get; set; }

    private IntKey[] _pristine = default!;
    private IntKey[] _work = default!;

    // GlobalSetup + per-invocation copy instead of IterationSetup: IterationSetup forces
    // InvocationCount=1, losing precision for µs-scale workloads. The copy cost is
    // identical for every benchmark method, so relative comparisons are unaffected.
    [GlobalSetup]
    public void Setup()
    {
        _pristine = BenchmarkData.GenerateIntKeyArray(Size, Pattern);
        _work = new IntKey[Size];
    }

    [Benchmark(Baseline = true)]
    public void QuickSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.QuickSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void QuickSort3way()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.QuickSort3way.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void QuickSortMedian3()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.QuickSortMedian3.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void QuickSortMedian9()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.QuickSortMedian9.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void DualPivotQuickSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.DualPivotQuickSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void StableQuickSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.StableQuickSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void IntroSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.IntroSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void IntroSortDotnet()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.IntroSortDotnet.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void PDQSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.PDQSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void StdSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.StdSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void BlockQuickSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.BlockQuickSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void DotnetSort()
    {
        Array.Copy(_pristine, _work, Size);
        _work.AsSpan().Sort();
    }
}
