namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class MergeBenchmark
{
    [Params(256, 1024, 8192)]
    public int Size { get; set; }

    [Params(DataPattern.Random, DataPattern.SingleElementMoved, DataPattern.Sorted, DataPattern.Reversed, DataPattern.PipeOrgan)]
    public DataPattern Pattern { get; set; }

    private int[] _mergeArray = default!;
    private int[] _pingpongmergeArray = default!;
    private int[] _bottomupmergeArray = default!;
    private int[] _stdstableArray = default!;
    private int[] _rotatemergeArray = default!;
    private int[] _rotatemergeRecursiveArray = default!;
    private int[] _symmergeArray = default!;
    private int[] _blockmergeArray = default!;
    private int[] _naturalmergeArray = default!;
    private int[] _timArray = default!;
    private int[] _powerArray = default!;
    private int[] _shiftArray = default!;
    private int[] _spinvariantArray = default!;
    private int[] _spinArray = default!;
    private int[] _glidesortArray = default!;

    [IterationSetup]
    public void Setup()
    {
        _mergeArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _pingpongmergeArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _bottomupmergeArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _stdstableArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _rotatemergeArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _rotatemergeRecursiveArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _symmergeArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _blockmergeArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _naturalmergeArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _timArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _powerArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _shiftArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _spinvariantArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _spinArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _glidesortArray = BenchmarkData.GenerateIntArray(Size, Pattern);
    }

    [Benchmark(Baseline = true)]
    public void MergeSort()
    {
        SortAlgorithm.Algorithms.MergeSort.Sort(_mergeArray.AsSpan());
    }

    [Benchmark]
    public void PingpongMergeSort()
    {
        SortAlgorithm.Algorithms.PingpongMergeSort.Sort(_rotatemergeArray.AsSpan());
    }

    [Benchmark]
    public void BottomupMergeSort()
    {
        SortAlgorithm.Algorithms.BottomupMergeSort.Sort(_bottomupmergeArray.AsSpan());
    }

    [Benchmark]
    public void StdStableSort()
    {
        SortAlgorithm.Algorithms.StdStableSort.Sort(_stdstableArray.AsSpan());
    }

    [Benchmark]
    public void RotateMergeSort()
    {
        SortAlgorithm.Algorithms.RotateMergeSort.Sort(_rotatemergeArray.AsSpan());
    }

    [Benchmark]
    public void RotateMergeSortRecursive()
    {
        SortAlgorithm.Algorithms.RotateMergeSortRecursive.Sort(_rotatemergeRecursiveArray.AsSpan());
    }

    [Benchmark]
    public void SymMergeSort()
    {
        SortAlgorithm.Algorithms.SymMergeSort.Sort(_symmergeArray.AsSpan());
    }

    [Benchmark]
    public void BlockMergeSort()
    {
        SortAlgorithm.Algorithms.BlockMergeSort.Sort(_blockmergeArray.AsSpan());
    }

    [Benchmark]
    public void NaturalMergeSort()
    {
        SortAlgorithm.Algorithms.NaturalMergeSort.Sort(_naturalmergeArray.AsSpan());
    }

    [Benchmark]
    public void TimSort()
    {
        SortAlgorithm.Algorithms.TimSort.Sort(_timArray.AsSpan());
    }

    [Benchmark]
    public void PowerSort()
    {
        SortAlgorithm.Algorithms.PowerSort.Sort(_powerArray.AsSpan());
    }

    [Benchmark]
    public void ShiftSort()
    {
        SortAlgorithm.Algorithms.ShiftSort.Sort(_shiftArray.AsSpan());
    }

    [Benchmark]
    public void SpinSort()
    {
        SortAlgorithm.Algorithms.SpinSort.Sort(_spinArray.AsSpan());
    }

    [Benchmark]
    public void SpinSortVariant()
    {
        SortAlgorithm.Algorithms.SpinSortVariant.Sort(_spinvariantArray.AsSpan());
    }

    [Benchmark]
    public void Glidesort()
    {
        SortAlgorithm.Algorithms.Glidesort.Sort(_glidesortArray.AsSpan());
    }
}
