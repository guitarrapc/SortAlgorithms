namespace SortAlgorithm.Benchmark;

/// <summary>
/// The algorithms of the other classes again, at sizes where the input no longer fits in the inner caches.
/// An algorithm that is quadratic on one of the patterns is left out, because one sort at these sizes would take
/// seconds to minutes: the exchange and insertion sorts, the unbalanced tree sort, and QuickSort and SimdQuickSort
/// on their adversarial patterns.
/// </summary>
/// <remarks>
/// Each iteration runs <see cref="SortBuffers.LargeInvocationsPerIteration"/> invocations instead of 64, since one sort of a
/// large input already takes milliseconds. The sizes start above those of the other classes, so the site never ranks
/// the same algorithm twice at one size. A new algorithm that is not quadratic on any pattern gets a method here too.
/// </remarks>
[MemoryDiagnoser]
[RankColumn]
[InvocationCount(SortBuffers.LargeInvocationsPerIteration)]
public class ScalingBenchmark
{
    [Params(16384, 65536, 262144, 1048576)]
    public int Size { get; set; }

    [Params(DataPattern.Random, DataPattern.SingleElementMoved, DataPattern.Sorted, DataPattern.Reversed, DataPattern.PipeOrgan, DataPattern.ManyDuplicates)]
    public DataPattern Pattern { get; set; }

    private SortBuffers<int> _buffers = default!;

    // Each invocation sorts its own pre-copied buffer, so the copy is not timed (see SortBuffers<T>).
    [GlobalSetup]
    public void Setup()
    {
        _buffers = new SortBuffers<int>(seed => BenchmarkData.GenerateIntArray(Size, Pattern, seed), SortBuffers.LargeInvocationsPerIteration);
    }

    [IterationSetup]
    public void IterationSetup() => _buffers.Reset();

    [Benchmark(Baseline = true)]
    public void DotnetSort()
    {
        _buffers.Next().AsSpan().Sort();
    }

    // Exchange

    [Benchmark]
    public void CombSort()
    {
        SortAlgorithm.Algorithms.CombSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void CircleSort()
    {
        SortAlgorithm.Algorithms.CircleSort.Sort(_buffers.Next().AsSpan());
    }

    // Insertion

    [Benchmark]
    public void ShellSortKnuth1973()
    {
        SortAlgorithm.Algorithms.ShellSortKnuth1973.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void ShellSortSedgewick1986()
    {
        SortAlgorithm.Algorithms.ShellSortSedgewick1986.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void ShellSortTokuda1992()
    {
        SortAlgorithm.Algorithms.ShellSortTokuda1992.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void ShellSortCiura2001()
    {
        SortAlgorithm.Algorithms.ShellSortCiura2001.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void ShellSortLee2021()
    {
        SortAlgorithm.Algorithms.ShellSortLee2021.Sort(_buffers.Next().AsSpan());
    }

    // Merge

    [Benchmark]
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
    public void PatienceSort()
    {
        SortAlgorithm.Algorithms.PatienceSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void DropMergeSort()
    {
        SortAlgorithm.Algorithms.DropMergeSort.Sort(_buffers.Next().AsSpan());
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
    public void Glidesort()
    {
        SortAlgorithm.Algorithms.Glidesort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void Driftsort()
    {
        SortAlgorithm.Algorithms.Driftsort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void FlatStableSort()
    {
        SortAlgorithm.Algorithms.FlatStableSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void QuadSort()
    {
        SortAlgorithm.Algorithms.QuadSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void SimdMergeSort()
    {
        SortAlgorithm.Algorithms.SimdMergeSort.Sort(_buffers.Next().AsSpan());
    }

    // Heap

    [Benchmark]
    public void HeapSort()
    {
        SortAlgorithm.Algorithms.HeapSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void MinHeapSort()
    {
        SortAlgorithm.Algorithms.MinHeapSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void TernaryHeapSort()
    {
        SortAlgorithm.Algorithms.TernaryHeapSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void BottomupHeapSort()
    {
        SortAlgorithm.Algorithms.BottomupHeapSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void WeakHeapSort()
    {
        SortAlgorithm.Algorithms.WeakHeapSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void SmoothSort()
    {
        SortAlgorithm.Algorithms.SmoothSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void PoplarHeapSort()
    {
        SortAlgorithm.Algorithms.PoplarHeapSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void TournamentSort()
    {
        SortAlgorithm.Algorithms.TournamentSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void BinomialHeapSort()
    {
        SortAlgorithm.Algorithms.BinomialHeapSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void PairingHeapSort()
    {
        SortAlgorithm.Algorithms.PairingHeapSort.Sort(_buffers.Next().AsSpan());
    }

    // Partition

    [Benchmark]
    public void QuickSort3way()
    {
        SortAlgorithm.Algorithms.QuickSort3way.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void QuickSortMedian3()
    {
        SortAlgorithm.Algorithms.QuickSortMedian3.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void QuickSortMedian9()
    {
        SortAlgorithm.Algorithms.QuickSortMedian9.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void SampleSort()
    {
        SortAlgorithm.Algorithms.SampleSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void DualPivotQuickSort()
    {
        SortAlgorithm.Algorithms.DualPivotQuickSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void StableQuickSort()
    {
        SortAlgorithm.Algorithms.StableQuickSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void BidirectionalStableQuickSort()
    {
        SortAlgorithm.Algorithms.BidirectionalStableQuickSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void DestswapStableQuickSort()
    {
        SortAlgorithm.Algorithms.DestswapStableQuickSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void IntroSort()
    {
        SortAlgorithm.Algorithms.IntroSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void IntroSortDotnet()
    {
        SortAlgorithm.Algorithms.IntroSortDotnet.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void PDQSort()
    {
        SortAlgorithm.Algorithms.PDQSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void PDQSortBranchless()
    {
        SortAlgorithm.Algorithms.PDQSortBranchless.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void Ipnsort()
    {
        SortAlgorithm.Algorithms.Ipnsort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void StdSort()
    {
        SortAlgorithm.Algorithms.StdSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void BlockQuickSort()
    {
        SortAlgorithm.Algorithms.BlockQuickSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void Ips4oSort()
    {
        SortAlgorithm.Algorithms.Ips4oSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void FluxSort()
    {
        SortAlgorithm.Algorithms.FluxSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void CrumSort()
    {
        SortAlgorithm.Algorithms.CrumSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void BlitSort()
    {
        SortAlgorithm.Algorithms.BlitSort.Sort(_buffers.Next().AsSpan());
    }

    // Distribution

    [Benchmark]
    public void CountingSort()
    {
        SortAlgorithm.Algorithms.CountingSort.SortBy(_buffers.Next().AsSpan(), x => x);
    }

    [Benchmark]
    public void CountingSortInteger()
    {
        SortAlgorithm.Algorithms.CountingSortInteger.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void PigeonSort()
    {
        SortAlgorithm.Algorithms.PigeonholeSort.SortBy(_buffers.Next().AsSpan(), x => x);
    }

    [Benchmark]
    public void PigeonSortInteger()
    {
        SortAlgorithm.Algorithms.PigeonholeSortInteger.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void BucketSort()
    {
        SortAlgorithm.Algorithms.BucketSort.SortBy(_buffers.Next().AsSpan(), x => x);
    }

    [Benchmark]
    public void BucketSortInteger()
    {
        SortAlgorithm.Algorithms.BucketSortInteger.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void FlashSort()
    {
        SortAlgorithm.Algorithms.FlashSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void RadixLSD4Sort()
    {
        SortAlgorithm.Algorithms.RadixLSD4Sort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void RotateLSDRadixSort()
    {
        SortAlgorithm.Algorithms.RotateLSDRadixSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void RadixLSD256Sort()
    {
        SortAlgorithm.Algorithms.RadixLSD256Sort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void Radsort()
    {
        SortAlgorithm.Algorithms.Radsort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void RadixLSD10Sort()
    {
        SortAlgorithm.Algorithms.RadixLSD10Sort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void RadixMSD4Sort()
    {
        SortAlgorithm.Algorithms.RadixMSD4Sort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void RadixMSD10Sort()
    {
        SortAlgorithm.Algorithms.RadixMSD10Sort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void AmericanFlagSort()
    {
        SortAlgorithm.Algorithms.AmericanFlagSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void SkaSort()
    {
        SortAlgorithm.Algorithms.SkaSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void SpreadSort()
    {
        SortAlgorithm.Algorithms.SpreadSort.Sort(_buffers.Next().AsSpan());
    }

    // Network

    [Benchmark]
    public void BitonicSort()
    {
        SortAlgorithm.Algorithms.BitonicSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void BitonicSortBranchless()
    {
        SortAlgorithm.Algorithms.BitonicSortBranchless.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void BitonicRecursiveSort()
    {
        SortAlgorithm.Algorithms.BitonicSortNonOptimized.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void BitonicSortAnyLength()
    {
        SortAlgorithm.Algorithms.BitonicSortAnyLength.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void BitonicSortAnyLengthSimd()
    {
        SortAlgorithm.Algorithms.BitonicSortAnyLengthSimd.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void BatcherOddEvenMergeSort()
    {
        SortAlgorithm.Algorithms.BatcherOddEvenMergeSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void BatcherOddEvenMergeSortBranchless()
    {
        SortAlgorithm.Algorithms.BatcherOddEvenMergeSortBranchless.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void PairwiseSortingNetwork()
    {
        SortAlgorithm.Algorithms.PairwiseSortingNetwork.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void PairwiseSortingNetworkBranchless()
    {
        SortAlgorithm.Algorithms.PairwiseSortingNetworkBranchless.Sort(_buffers.Next().AsSpan());
    }

    // Tree

    [Benchmark]
    public void BalancedBinaryTreeSort()
    {
        SortAlgorithm.Algorithms.BalancedBinaryTreeSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void BTreeSort()
    {
        SortAlgorithm.Algorithms.BTreeSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void BPlusTreeSort()
    {
        SortAlgorithm.Algorithms.BPlusTreeSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void CartesianTreeSort()
    {
        SortAlgorithm.Algorithms.CartesianTreeSort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void SplaySort()
    {
        SortAlgorithm.Algorithms.SplaySort.Sort(_buffers.Next().AsSpan());
    }

    [Benchmark]
    public void TreapSort()
    {
        SortAlgorithm.Algorithms.TreapSort.Sort(_buffers.Next().AsSpan());
    }
}
