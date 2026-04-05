namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class PartitionBenchmark
{
    [Params(256, 1024, 8192)]
    public int Size { get; set; }

    [Params(DataPattern.Random, DataPattern.SingleElementMoved, DataPattern.Sorted, DataPattern.Reversed, DataPattern.PipeOrgan)]
    public DataPattern Pattern { get; set; }

    private int[] _quickArray = default!;
    private int[] _quick3wayArray = default!;
    private int[] _quickmedian3Array = default!;
    private int[] _quickmedian9Array = default!;
    private int[] _dualpivotquickArray = default!;
    private int[] _stablequickArray = default!;
    private int[] _bidirectionalstablequickArray = default!;
    private int[] _destswapstableqQuickArray = default!;
    private int[] _introArray = default!;
    private int[] _introdotnetArray = default!;
    private int[] _pdqArray = default!;
    private int[] _stdArray = default!;
    private int[] _blockquickArray = default!;
    private int[] _dotnetArray = default!;

    [IterationSetup]
    public void Setup()
    {
        _quickArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _quick3wayArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _quickmedian3Array = BenchmarkData.GenerateIntArray(Size, Pattern);
        _quickmedian9Array = BenchmarkData.GenerateIntArray(Size, Pattern);
        _dualpivotquickArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _stablequickArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _bidirectionalstablequickArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _destswapstableqQuickArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _introArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _introdotnetArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _pdqArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _stdArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _blockquickArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _dotnetArray = BenchmarkData.GenerateIntArray(Size, Pattern);
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
    public void BidirectionalStableQuickSort()
    {
        SortAlgorithm.Algorithms.BidirectionalStableQuickSort.Sort(_bidirectionalstablequickArray.AsSpan());
    }

    [Benchmark]
    public void DestswapStableQuickSort()
    {
        SortAlgorithm.Algorithms.DestswapStableQuickSort.Sort(_destswapstableqQuickArray.AsSpan());
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
