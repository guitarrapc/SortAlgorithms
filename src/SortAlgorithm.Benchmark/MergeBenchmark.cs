namespace SortAlgorithm.Benchmark;

[MemoryDiagnoser]
[RankColumn]
public class MergeBenchmark
{
    [Params(256, 1024, 8192)]
    public int Size { get; set; }

    [Params(DataPattern.Random, DataPattern.SingleElementMoved, DataPattern.Sorted, DataPattern.Reversed, DataPattern.PipeOrgan)]
    public DataPattern Pattern { get; set; }

    private int[] _template = default!;
    private int[] _mergeArray = default!;
    private int[] _pingpongmergeArray = default!;
    private int[] _bottomupmergeArray = default!;
    private int[] _stdstableArray = default!;
    private int[] _rotatemergeArray = default!;
    private int[] _rotatemergeRecursiveArray = default!;
    private int[] _symmergeArray = default!;
    private int[] _naturalmergeArray = default!;
    private int[] _timArray = default!;
    private int[] _powerArray = default!;
    private int[] _shiftArray = default!;
    private int[] _spinvariantArray = default!;
    private int[] _spinArray = default!;
    private int[] _glidesortArray = default!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _template = BenchmarkData.GenerateIntArray(Size, Pattern);
        _mergeArray = new int[Size];
        _pingpongmergeArray = new int[Size];
        _bottomupmergeArray = new int[Size];
        _stdstableArray = new int[Size];
        _rotatemergeArray = new int[Size];
        _rotatemergeRecursiveArray = new int[Size];
        _symmergeArray = new int[Size];
        _naturalmergeArray = new int[Size];
        _timArray = new int[Size];
        _powerArray = new int[Size];
        _shiftArray = new int[Size];
        _spinvariantArray = new int[Size];
        _spinArray = new int[Size];
        _glidesortArray = new int[Size];
    }

    [IterationSetup]
    public void Setup()
    {
        _template.CopyTo(_mergeArray, 0);
        _template.CopyTo(_pingpongmergeArray, 0);
        _template.CopyTo(_bottomupmergeArray, 0);
        _template.CopyTo(_stdstableArray, 0);
        _template.CopyTo(_rotatemergeArray, 0);
        _template.CopyTo(_rotatemergeRecursiveArray, 0);
        _template.CopyTo(_symmergeArray, 0);
        _template.CopyTo(_naturalmergeArray, 0);
        _template.CopyTo(_timArray, 0);
        _template.CopyTo(_powerArray, 0);
        _template.CopyTo(_shiftArray, 0);
        _template.CopyTo(_spinvariantArray, 0);
        _template.CopyTo(_spinArray, 0);
        _template.CopyTo(_glidesortArray, 0);
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
