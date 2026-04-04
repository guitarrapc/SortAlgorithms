namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class PartitionBenchmark
{
    [Params(256, 1024, 8192)]
    public int Size { get; set; }

    [Params(DataPattern.Random, DataPattern.SingleElementMoved, DataPattern.Sorted, DataPattern.Reversed, DataPattern.PipeOrgan)]
    public DataPattern Pattern { get; set; }

    private int[] _template = default!;
    private int[] _quickArray = default!;
    private int[] _quick3wayArray = default!;
    private int[] _quickmedian3Array = default!;
    private int[] _quickmedian9Array = default!;
    private int[] _dualpivotquickArray = default!;
    private int[] _stablequickArray = default!;
    private int[] _introArray = default!;
    private int[] _introdotnetArray = default!;
    private int[] _pdqArray = default!;
    private int[] _stdArray = default!;
    private int[] _blockquickArray = default!;
    private int[] _dotnetArray = default!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _template = BenchmarkData.GenerateIntArray(Size, Pattern);
        _quickArray = new int[Size];
        _quick3wayArray = new int[Size];
        _quickmedian3Array = new int[Size];
        _quickmedian9Array = new int[Size];
        _dualpivotquickArray = new int[Size];
        _stablequickArray = new int[Size];
        _introArray = new int[Size];
        _introdotnetArray = new int[Size];
        _pdqArray = new int[Size];
        _stdArray = new int[Size];
        _blockquickArray = new int[Size];
        _dotnetArray = new int[Size];
    }

    [IterationSetup]
    public void Setup()
    {
        _template.CopyTo(_quickArray, 0);
        _template.CopyTo(_quick3wayArray, 0);
        _template.CopyTo(_quickmedian3Array, 0);
        _template.CopyTo(_quickmedian9Array, 0);
        _template.CopyTo(_dualpivotquickArray, 0);
        _template.CopyTo(_stablequickArray, 0);
        _template.CopyTo(_introArray, 0);
        _template.CopyTo(_introdotnetArray, 0);
        _template.CopyTo(_pdqArray, 0);
        _template.CopyTo(_stdArray, 0);
        _template.CopyTo(_blockquickArray, 0);
        _template.CopyTo(_dotnetArray, 0);
    }

    [Benchmark(Baseline = true)]
    public void QuickSort()
    {
        SortAlgorithm.Algorithms.QuickSort.Sort(_quickArray.AsSpan());
    }

    [Benchmark]
    public void QuickSort3way()
    {
        SortAlgorithm.Algorithms.QuickSort3way.Sort(_quick3wayArray.AsSpan());
    }

    [Benchmark]
    public void QuickSortMedian3()
    {
        SortAlgorithm.Algorithms.QuickSortMedian3.Sort(_quickmedian3Array.AsSpan());
    }

    [Benchmark]
    public void QuickSortMedian9()
    {
        SortAlgorithm.Algorithms.QuickSortMedian9.Sort(_quickmedian9Array.AsSpan());
    }

    [Benchmark]
    public void DualPivotQuickSort()
    {
        SortAlgorithm.Algorithms.DualPivotQuickSort.Sort(_dualpivotquickArray.AsSpan());
    }

    [Benchmark]
    public void StableQuickSort()
    {
        SortAlgorithm.Algorithms.StableQuickSort.Sort(_stablequickArray.AsSpan());
    }

    [Benchmark]
    public void IntroSort()
    {
        SortAlgorithm.Algorithms.IntroSort.Sort(_introArray.AsSpan());
    }

    [Benchmark]
    public void IntroSortDotnet()
    {
        SortAlgorithm.Algorithms.IntroSortDotnet.Sort(_introdotnetArray.AsSpan());
    }

    [Benchmark]
    public void PDQSort()
    {
        SortAlgorithm.Algorithms.PDQSort.Sort(_pdqArray.AsSpan());
    }

    [Benchmark]
    public void StdSort()
    {
        SortAlgorithm.Algorithms.StdSort.Sort(_stdArray.AsSpan());
    }

    [Benchmark]
    public void BlockQuickSort()
    {
        SortAlgorithm.Algorithms.BlockQuickSort.Sort(_blockquickArray.AsSpan());
    }

    [Benchmark]
    public void DotnetSort()
    {
        _dotnetArray.AsSpan().Sort();
    }
}
