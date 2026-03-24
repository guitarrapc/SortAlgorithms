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

    private IntKey[] _quickArray = default!;
    private IntKey[] _quick3wayArray = default!;
    private IntKey[] _quickmedian3Array = default!;
    private IntKey[] _quickmedian9Array = default!;
    private IntKey[] _dualpivotquickArray = default!;
    private IntKey[] _stablequickArray = default!;
    private IntKey[] _introArray = default!;
    private IntKey[] _introdotnetArray = default!;
    private IntKey[] _pdqArray = default!;
    private IntKey[] _stdArray = default!;
    private IntKey[] _blockquickArray = default!;
    private IntKey[] _dotnetArray = default!;

    [IterationSetup]
    public void Setup()
    {
        _quickArray = BenchmarkData.GenerateIntKeyArray(Size, Pattern);
        _quick3wayArray = BenchmarkData.GenerateIntKeyArray(Size, Pattern);
        _quickmedian3Array = BenchmarkData.GenerateIntKeyArray(Size, Pattern);
        _quickmedian9Array = BenchmarkData.GenerateIntKeyArray(Size, Pattern);
        _dualpivotquickArray = BenchmarkData.GenerateIntKeyArray(Size, Pattern);
        _stablequickArray = BenchmarkData.GenerateIntKeyArray(Size, Pattern);
        _introArray = BenchmarkData.GenerateIntKeyArray(Size, Pattern);
        _introdotnetArray = BenchmarkData.GenerateIntKeyArray(Size, Pattern);
        _pdqArray = BenchmarkData.GenerateIntKeyArray(Size, Pattern);
        _stdArray = BenchmarkData.GenerateIntKeyArray(Size, Pattern);
        _blockquickArray = BenchmarkData.GenerateIntKeyArray(Size, Pattern);
        _dotnetArray = BenchmarkData.GenerateIntKeyArray(Size, Pattern);
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
