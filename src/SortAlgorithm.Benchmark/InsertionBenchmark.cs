namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class InsertionBenchmark
{
    [Params(256, 1024)]
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
    public void InsertionSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.InsertionSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void PairInsertionSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.PairInsertionSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void BinaryInsertSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.BinaryInsertionSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void GnomeSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.GnomeSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void LibrarySort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.LibrarySort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void MergeInsertionSort()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.MergeInsertionSort.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void ShellSortKnuth1973()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.ShellSortKnuth1973.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void ShellSortSedgewick1986()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.ShellSortSedgewick1986.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void ShellSortTokuda1992()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.ShellSortTokuda1992.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void ShellSortCiura2001()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.ShellSortCiura2001.Sort(_work.AsSpan());
    }

    [Benchmark]
    public void ShellSortLee2021()
    {
        Array.Copy(_pristine, _work, Size);
        SortAlgorithm.Algorithms.ShellSortLee2021.Sort(_work.AsSpan());
    }
}
