namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class StringBenchmark
{
    [Params(256, 1024, 8192)]
    public int Size { get; set; }

    [Params(DataPattern.Random, DataPattern.SingleElementMoved, DataPattern.Sorted, DataPattern.Reversed, DataPattern.PipeOrgan)]
    public DataPattern Pattern { get; set; }

    private string[] _template = default!;
    private string[] _quickArray = default!;
    private string[] _quick3wayArray = default!;
    private string[] _quickmedian3Array = default!;
    private string[] _quickmedian9Array = default!;
    private string[] _dualpivotquickArray = default!;
    private string[] _stablequickArray = default!;
    private string[] _introArray = default!;
    private string[] _introdotnetArray = default!;
    private string[] _pdqArray = default!;
    private string[] _stdArray = default!;
    private string[] _blockquickArray = default!;
    private string[] _dotnetArray = default!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _template = BenchmarkData.GenerateStringArray(Size, Pattern);
        _quickArray = new string[Size];
        _quick3wayArray = new string[Size];
        _quickmedian3Array = new string[Size];
        _quickmedian9Array = new string[Size];
        _dualpivotquickArray = new string[Size];
        _stablequickArray = new string[Size];
        _introArray = new string[Size];
        _introdotnetArray = new string[Size];
        _pdqArray = new string[Size];
        _stdArray = new string[Size];
        _blockquickArray = new string[Size];
        _dotnetArray = new string[Size];
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
