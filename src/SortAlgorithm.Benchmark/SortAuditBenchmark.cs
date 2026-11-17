using SortAlgorithm.Contexts;

namespace SortAlgorithm.Benchmark;

public abstract class SortAuditBase
{
    protected SortBuffers<int> buffers = default!;
    public abstract int Size { get; set; }

    [Params(DataPattern.Random, DataPattern.Sorted, DataPattern.Reversed, DataPattern.ManyDuplicates, DataPattern.PipeOrgan, DataPattern.SingleElementMoved)]
    public DataPattern Pattern { get; set; }

    protected void Setup(string name)
    {
        buffers = new SortBuffers<int>(seed => BenchmarkData.GenerateIntArray(Size, Pattern, seed));

        // Every input an invocation will sort passes the correctness gate, not just the first.
        var sort = SortAuditAlgorithms.All.Single(a => a.Name == name).Sort;
        foreach (var input in buffers.Sources)
        {
            var expected = input.Order().ToArray();
            var actual = (int[])input.Clone();
            sort(actual);
            if (!actual.AsSpan().SequenceEqual(expected))
                throw new InvalidOperationException($"{name}: {Size}/{Pattern} failed correctness gate.");
        }
    }

    [IterationSetup]
    public void Reset() => buffers.Reset();
}
public class SortAuditBenchmark : SortAuditBase
{
    [Params(256, 1000)]
    public override int Size { get; set; }

    [GlobalSetup(Target = nameof(AmericanFlagSort))]
    public void SetupAmericanFlagSort() => Setup(nameof(AmericanFlagSort));

    [Benchmark]
    public int AmericanFlagSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.AmericanFlagSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(BalancedBinaryTreeSort))]
    public void SetupBalancedBinaryTreeSort() => Setup(nameof(BalancedBinaryTreeSort));

    [Benchmark]
    public int BalancedBinaryTreeSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.BalancedBinaryTreeSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(BalancedBinaryTreeSortNonOptimized))]
    public void SetupBalancedBinaryTreeSortNonOptimized() => Setup(nameof(BalancedBinaryTreeSortNonOptimized));

    [Benchmark]
    public int BalancedBinaryTreeSortNonOptimized()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.BalancedBinaryTreeSortNonOptimized.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(BatcherOddEvenMergeSort))]
    public void SetupBatcherOddEvenMergeSort() => Setup(nameof(BatcherOddEvenMergeSort));

    [Benchmark]
    public int BatcherOddEvenMergeSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.BatcherOddEvenMergeSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(BidirectionalStableQuickSort))]
    public void SetupBidirectionalStableQuickSort() => Setup(nameof(BidirectionalStableQuickSort));

    [Benchmark]
    public int BidirectionalStableQuickSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.BidirectionalStableQuickSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(BinaryInsertionSort))]
    public void SetupBinaryInsertionSort() => Setup(nameof(BinaryInsertionSort));

    [Benchmark]
    public int BinaryInsertionSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.BinaryInsertionSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(BinaryTreeSort))]
    public void SetupBinaryTreeSort() => Setup(nameof(BinaryTreeSort));

    [Benchmark]
    public int BinaryTreeSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.BinaryTreeSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(BingoSort))]
    public void SetupBingoSort() => Setup(nameof(BingoSort));

    [Benchmark]
    public int BingoSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.BingoSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(BinomialHeapSort))]
    public void SetupBinomialHeapSort() => Setup(nameof(BinomialHeapSort));

    [Benchmark]
    public int BinomialHeapSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.BinomialHeapSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(BlockMergeSort))]
    public void SetupBlockMergeSort() => Setup(nameof(BlockMergeSort));

    [Benchmark]
    public int BlockMergeSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.BlockMergeSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(BlockQuickSort))]
    public void SetupBlockQuickSort() => Setup(nameof(BlockQuickSort));

    [Benchmark]
    public int BlockQuickSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.BlockQuickSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(BottomupHeapSort))]
    public void SetupBottomupHeapSort() => Setup(nameof(BottomupHeapSort));

    [Benchmark]
    public int BottomupHeapSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.BottomupHeapSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(BottomupMergeSort))]
    public void SetupBottomupMergeSort() => Setup(nameof(BottomupMergeSort));

    [Benchmark]
    public int BottomupMergeSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.BottomupMergeSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(BPlusTreeSort))]
    public void SetupBPlusTreeSort() => Setup(nameof(BPlusTreeSort));

    [Benchmark]
    public int BPlusTreeSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.BPlusTreeSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(BTreeSort))]
    public void SetupBTreeSort() => Setup(nameof(BTreeSort));

    [Benchmark]
    public int BTreeSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.BTreeSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(BubbleSort))]
    public void SetupBubbleSort() => Setup(nameof(BubbleSort));

    [Benchmark]
    public int BubbleSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.BubbleSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(BubbleSortNonOptimized))]
    public void SetupBubbleSortNonOptimized() => Setup(nameof(BubbleSortNonOptimized));

    [Benchmark]
    public int BubbleSortNonOptimized()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.BubbleSortNonOptimized.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(BucketSort))]
    public void SetupBucketSort() => Setup(nameof(BucketSort));

    [Benchmark]
    public int BucketSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.BucketSort.SortBy(values.AsSpan(), static x => x, NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(BucketSortInteger))]
    public void SetupBucketSortInteger() => Setup(nameof(BucketSortInteger));

    [Benchmark]
    public int BucketSortInteger()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.BucketSortInteger.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(CartesianTreeSort))]
    public void SetupCartesianTreeSort() => Setup(nameof(CartesianTreeSort));

    [Benchmark]
    public int CartesianTreeSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.CartesianTreeSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(CircleSort))]
    public void SetupCircleSort() => Setup(nameof(CircleSort));

    [Benchmark]
    public int CircleSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.CircleSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(CocktailShakerSort))]
    public void SetupCocktailShakerSort() => Setup(nameof(CocktailShakerSort));

    [Benchmark]
    public int CocktailShakerSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.CocktailShakerSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(CocktailShakerSortNonOptimized))]
    public void SetupCocktailShakerSortNonOptimized() => Setup(nameof(CocktailShakerSortNonOptimized));

    [Benchmark]
    public int CocktailShakerSortNonOptimized()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.CocktailShakerSortNonOptimized.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(CombSort))]
    public void SetupCombSort() => Setup(nameof(CombSort));

    [Benchmark]
    public int CombSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.CombSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(CountingSort))]
    public void SetupCountingSort() => Setup(nameof(CountingSort));

    [Benchmark]
    public int CountingSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.CountingSort.SortBy(values.AsSpan(), static x => x, NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(CountingSortInteger))]
    public void SetupCountingSortInteger() => Setup(nameof(CountingSortInteger));

    [Benchmark]
    public int CountingSortInteger()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.CountingSortInteger.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(CycleSort))]
    public void SetupCycleSort() => Setup(nameof(CycleSort));

    [Benchmark]
    public int CycleSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.CycleSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(DestswapStableQuickSort))]
    public void SetupDestswapStableQuickSort() => Setup(nameof(DestswapStableQuickSort));

    [Benchmark]
    public int DestswapStableQuickSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.DestswapStableQuickSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(DoubleSelectionSort))]
    public void SetupDoubleSelectionSort() => Setup(nameof(DoubleSelectionSort));

    [Benchmark]
    public int DoubleSelectionSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.DoubleSelectionSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(Driftsort))]
    public void SetupDriftsort() => Setup(nameof(Driftsort));

    [Benchmark]
    public int Driftsort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.Driftsort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(DropMergeSort))]
    public void SetupDropMergeSort() => Setup(nameof(DropMergeSort));

    [Benchmark]
    public int DropMergeSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.DropMergeSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(DualPivotQuickSort))]
    public void SetupDualPivotQuickSort() => Setup(nameof(DualPivotQuickSort));

    [Benchmark]
    public int DualPivotQuickSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.DualPivotQuickSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(FlashSort))]
    public void SetupFlashSort() => Setup(nameof(FlashSort));

    [Benchmark]
    public int FlashSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.FlashSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(FlatStableSort))]
    public void SetupFlatStableSort() => Setup(nameof(FlatStableSort));

    [Benchmark]
    public int FlatStableSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.FlatStableSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(FluxSort))]
    public void SetupFluxSort() => Setup(nameof(FluxSort));

    [Benchmark]
    public int FluxSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.FluxSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(FunSort))]
    public void SetupFunSort() => Setup(nameof(FunSort));

    [Benchmark]
    public int FunSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.FunSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(Glidesort))]
    public void SetupGlidesort() => Setup(nameof(Glidesort));

    [Benchmark]
    public int Glidesort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.Glidesort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(GnomeSort))]
    public void SetupGnomeSort() => Setup(nameof(GnomeSort));

    [Benchmark]
    public int GnomeSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.GnomeSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(GnomeSortNonOptimized))]
    public void SetupGnomeSortNonOptimized() => Setup(nameof(GnomeSortNonOptimized));

    [Benchmark]
    public int GnomeSortNonOptimized()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.GnomeSortNonOptimized.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(HeapSort))]
    public void SetupHeapSort() => Setup(nameof(HeapSort));

    [Benchmark]
    public int HeapSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.HeapSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(HeapSortNonOptimized))]
    public void SetupHeapSortNonOptimized() => Setup(nameof(HeapSortNonOptimized));

    [Benchmark]
    public int HeapSortNonOptimized()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.HeapSortNonOptimized.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(InsertionSort))]
    public void SetupInsertionSort() => Setup(nameof(InsertionSort));

    [Benchmark]
    public int InsertionSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.InsertionSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(InsertionSortNonOptimized))]
    public void SetupInsertionSortNonOptimized() => Setup(nameof(InsertionSortNonOptimized));

    [Benchmark]
    public int InsertionSortNonOptimized()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.InsertionSortNonOptimized.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(IntroSort))]
    public void SetupIntroSort() => Setup(nameof(IntroSort));

    [Benchmark]
    public int IntroSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.IntroSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(IntroSortDotnet))]
    public void SetupIntroSortDotnet() => Setup(nameof(IntroSortDotnet));

    [Benchmark]
    public int IntroSortDotnet()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.IntroSortDotnet.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(Ipnsort))]
    public void SetupIpnsort() => Setup(nameof(Ipnsort));

    [Benchmark]
    public int Ipnsort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.Ipnsort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(LibrarySort))]
    public void SetupLibrarySort() => Setup(nameof(LibrarySort));

    [Benchmark]
    public int LibrarySort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.LibrarySort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(MergeInsertionSort))]
    public void SetupMergeInsertionSort() => Setup(nameof(MergeInsertionSort));

    [Benchmark]
    public int MergeInsertionSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.MergeInsertionSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(MergeSort))]
    public void SetupMergeSort() => Setup(nameof(MergeSort));

    [Benchmark]
    public int MergeSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.MergeSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(MinHeapSort))]
    public void SetupMinHeapSort() => Setup(nameof(MinHeapSort));

    [Benchmark]
    public int MinHeapSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.MinHeapSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(NaturalMergeSort))]
    public void SetupNaturalMergeSort() => Setup(nameof(NaturalMergeSort));

    [Benchmark]
    public int NaturalMergeSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.NaturalMergeSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(OddEvenSort))]
    public void SetupOddEvenSort() => Setup(nameof(OddEvenSort));

    [Benchmark]
    public int OddEvenSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.OddEvenSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(PairingHeapSort))]
    public void SetupPairingHeapSort() => Setup(nameof(PairingHeapSort));

    [Benchmark]
    public int PairingHeapSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.PairingHeapSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(PairInsertionSort))]
    public void SetupPairInsertionSort() => Setup(nameof(PairInsertionSort));

    [Benchmark]
    public int PairInsertionSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.PairInsertionSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(PairwiseSortingNetwork))]
    public void SetupPairwiseSortingNetwork() => Setup(nameof(PairwiseSortingNetwork));

    [Benchmark]
    public int PairwiseSortingNetwork()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.PairwiseSortingNetwork.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(PancakeSort))]
    public void SetupPancakeSort() => Setup(nameof(PancakeSort));

    [Benchmark]
    public int PancakeSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.PancakeSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(PatienceSort))]
    public void SetupPatienceSort() => Setup(nameof(PatienceSort));

    [Benchmark]
    public int PatienceSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.PatienceSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(PDQSort))]
    public void SetupPDQSort() => Setup(nameof(PDQSort));

    [Benchmark]
    public int PDQSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.PDQSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(PDQSortBranchless))]
    public void SetupPDQSortBranchless() => Setup(nameof(PDQSortBranchless));

    [Benchmark]
    public int PDQSortBranchless()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.PDQSortBranchless.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(PigeonholeSort))]
    public void SetupPigeonholeSort() => Setup(nameof(PigeonholeSort));

    [Benchmark]
    public int PigeonholeSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.PigeonholeSort.SortBy(values.AsSpan(), static x => x, NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(PigeonholeSortInteger))]
    public void SetupPigeonholeSortInteger() => Setup(nameof(PigeonholeSortInteger));

    [Benchmark]
    public int PigeonholeSortInteger()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.PigeonholeSortInteger.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(PingpongMergeSort))]
    public void SetupPingpongMergeSort() => Setup(nameof(PingpongMergeSort));

    [Benchmark]
    public int PingpongMergeSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.PingpongMergeSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(PoplarHeapSort))]
    public void SetupPoplarHeapSort() => Setup(nameof(PoplarHeapSort));

    [Benchmark]
    public int PoplarHeapSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.PoplarHeapSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(PowerSort))]
    public void SetupPowerSort() => Setup(nameof(PowerSort));

    [Benchmark]
    public int PowerSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.PowerSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(QuadSort))]
    public void SetupQuadSort() => Setup(nameof(QuadSort));

    [Benchmark]
    public int QuadSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.QuadSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(QuickSort))]
    public void SetupQuickSort() => Setup(nameof(QuickSort));

    [Benchmark]
    public int QuickSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.QuickSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(QuickSort3way))]
    public void SetupQuickSort3way() => Setup(nameof(QuickSort3way));

    [Benchmark]
    public int QuickSort3way()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.QuickSort3way.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(QuickSortMedian3))]
    public void SetupQuickSortMedian3() => Setup(nameof(QuickSortMedian3));

    [Benchmark]
    public int QuickSortMedian3()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.QuickSortMedian3.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(QuickSortMedian9))]
    public void SetupQuickSortMedian9() => Setup(nameof(QuickSortMedian9));

    [Benchmark]
    public int QuickSortMedian9()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.QuickSortMedian9.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(RadixLSD10Sort))]
    public void SetupRadixLSD10Sort() => Setup(nameof(RadixLSD10Sort));

    [Benchmark]
    public int RadixLSD10Sort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.RadixLSD10Sort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(RadixLSD256Sort))]
    public void SetupRadixLSD256Sort() => Setup(nameof(RadixLSD256Sort));

    [Benchmark]
    public int RadixLSD256Sort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.RadixLSD256Sort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(RadixLSD4Sort))]
    public void SetupRadixLSD4Sort() => Setup(nameof(RadixLSD4Sort));

    [Benchmark]
    public int RadixLSD4Sort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.RadixLSD4Sort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(RadixMSD10Sort))]
    public void SetupRadixMSD10Sort() => Setup(nameof(RadixMSD10Sort));

    [Benchmark]
    public int RadixMSD10Sort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.RadixMSD10Sort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(RadixMSD4Sort))]
    public void SetupRadixMSD4Sort() => Setup(nameof(RadixMSD4Sort));

    [Benchmark]
    public int RadixMSD4Sort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.RadixMSD4Sort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(Radsort))]
    public void SetupRadsort() => Setup(nameof(Radsort));

    [Benchmark]
    public int Radsort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.Radsort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(RotateLSDRadixSort))]
    public void SetupRotateLSDRadixSort() => Setup(nameof(RotateLSDRadixSort));

    [Benchmark]
    public int RotateLSDRadixSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.RotateLSDRadixSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(RotateMergeSort))]
    public void SetupRotateMergeSort() => Setup(nameof(RotateMergeSort));

    [Benchmark]
    public int RotateMergeSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.RotateMergeSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(RotateMergeSortNonOptimized))]
    public void SetupRotateMergeSortNonOptimized() => Setup(nameof(RotateMergeSortNonOptimized));

    [Benchmark]
    public int RotateMergeSortNonOptimized()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.RotateMergeSortNonOptimized.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(RotateMergeSortRecursive))]
    public void SetupRotateMergeSortRecursive() => Setup(nameof(RotateMergeSortRecursive));

    [Benchmark]
    public int RotateMergeSortRecursive()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.RotateMergeSortRecursive.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(SampleSort))]
    public void SetupSampleSort() => Setup(nameof(SampleSort));

    [Benchmark]
    public int SampleSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.SampleSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(SelectionSort))]
    public void SetupSelectionSort() => Setup(nameof(SelectionSort));

    [Benchmark]
    public int SelectionSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.SelectionSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(ShellSortCiura2001))]
    public void SetupShellSortCiura2001() => Setup(nameof(ShellSortCiura2001));

    [Benchmark]
    public int ShellSortCiura2001()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.ShellSortCiura2001.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(ShellSortKnuth1973))]
    public void SetupShellSortKnuth1973() => Setup(nameof(ShellSortKnuth1973));

    [Benchmark]
    public int ShellSortKnuth1973()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.ShellSortKnuth1973.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(ShellSortLee2021))]
    public void SetupShellSortLee2021() => Setup(nameof(ShellSortLee2021));

    [Benchmark]
    public int ShellSortLee2021()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.ShellSortLee2021.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(ShellSortSedgewick1986))]
    public void SetupShellSortSedgewick1986() => Setup(nameof(ShellSortSedgewick1986));

    [Benchmark]
    public int ShellSortSedgewick1986()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.ShellSortSedgewick1986.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(ShellSortTokuda1992))]
    public void SetupShellSortTokuda1992() => Setup(nameof(ShellSortTokuda1992));

    [Benchmark]
    public int ShellSortTokuda1992()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.ShellSortTokuda1992.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(ShiftSort))]
    public void SetupShiftSort() => Setup(nameof(ShiftSort));

    [Benchmark]
    public int ShiftSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.ShiftSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(SmoothSort))]
    public void SetupSmoothSort() => Setup(nameof(SmoothSort));

    [Benchmark]
    public int SmoothSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.SmoothSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(SpinSort))]
    public void SetupSpinSort() => Setup(nameof(SpinSort));

    [Benchmark]
    public int SpinSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.SpinSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(SplaySort))]
    public void SetupSplaySort() => Setup(nameof(SplaySort));

    [Benchmark]
    public int SplaySort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.SplaySort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(SpreadSort))]
    public void SetupSpreadSort() => Setup(nameof(SpreadSort));

    [Benchmark]
    public int SpreadSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.SpreadSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(StableQuickSort))]
    public void SetupStableQuickSort() => Setup(nameof(StableQuickSort));

    [Benchmark]
    public int StableQuickSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.StableQuickSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(StdSort))]
    public void SetupStdSort() => Setup(nameof(StdSort));

    [Benchmark]
    public int StdSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.StdSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(StdStableSort))]
    public void SetupStdStableSort() => Setup(nameof(StdStableSort));

    [Benchmark]
    public int StdStableSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.StdStableSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(StrandSort))]
    public void SetupStrandSort() => Setup(nameof(StrandSort));

    [Benchmark]
    public int StrandSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.StrandSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(SymMergeSort))]
    public void SetupSymMergeSort() => Setup(nameof(SymMergeSort));

    [Benchmark]
    public int SymMergeSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.SymMergeSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(TernaryHeapSort))]
    public void SetupTernaryHeapSort() => Setup(nameof(TernaryHeapSort));

    [Benchmark]
    public int TernaryHeapSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.TernaryHeapSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(TimSort))]
    public void SetupTimSort() => Setup(nameof(TimSort));

    [Benchmark]
    public int TimSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.TimSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(TournamentSort))]
    public void SetupTournamentSort() => Setup(nameof(TournamentSort));

    [Benchmark]
    public int TournamentSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.TournamentSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(TreapSort))]
    public void SetupTreapSort() => Setup(nameof(TreapSort));

    [Benchmark]
    public int TreapSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.TreapSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(WeakHeapSort))]
    public void SetupWeakHeapSort() => Setup(nameof(WeakHeapSort));

    [Benchmark]
    public int WeakHeapSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.WeakHeapSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }
}

public class SortAuditNetworkBenchmark : SortAuditBase
{
    [Params(256, 1024)]
    public override int Size { get; set; }

    [GlobalSetup(Target = nameof(BitonicSort))]
    public void SetupBitonicSort() => Setup(nameof(BitonicSort));

    [Benchmark]
    public int BitonicSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.BitonicSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(BitonicSortNonOptimized))]
    public void SetupBitonicSortNonOptimized() => Setup(nameof(BitonicSortNonOptimized));

    [Benchmark]
    public int BitonicSortNonOptimized()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.BitonicSortNonOptimized.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }
}

public class SortAuditJokeBenchmark : SortAuditBase
{
    [Params(5, 7)]
    public override int Size { get; set; }

    [GlobalSetup(Target = nameof(BogoSort))]
    public void SetupBogoSort() => Setup(nameof(BogoSort));

    [Benchmark]
    public int BogoSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.BogoSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(BozoSort))]
    public void SetupBozoSort() => Setup(nameof(BozoSort));

    [Benchmark]
    public int BozoSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.BozoSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(SlowSort))]
    public void SetupSlowSort() => Setup(nameof(SlowSort));

    [Benchmark]
    public int SlowSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.SlowSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }

    [GlobalSetup(Target = nameof(StoogeSort))]
    public void SetupStoogeSort() => Setup(nameof(StoogeSort));

    [Benchmark]
    public int StoogeSort()
    {
        var values = buffers.Next();
        SortAlgorithm.Algorithms.StoogeSort.Sort(values.AsSpan(), NullContext.Default);
        return values[values.Length / 2];
    }
}

