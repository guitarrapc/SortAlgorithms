# SortAlgorithms

This repository shows implementation for the Major Sort Algorithm.
Aim not to use LINQ or similar ease to use, but memory unefficient technique.

## Table of Contents

- [Benchmark](#benchmark)
  - [DistributionBenchmark](#distributionbenchmark)
  - [ExchangeBenchmark](#exchangebenchmark)
  - [HeapBenchmark](#heapbenchmark)
  - [InsertionBenchmark](#insertionbenchmark)
  - [IntKeyBenchmark](#intkeybenchmark)
  - [MergeBenchmark](#mergebenchmark)
  - [MergeSlowBenchmark](#mergeslowbenchmark)
  - [NetworkBenchmark](#networkbenchmark)
  - [PartitionBenchmark](#partitionbenchmark)
  - [SelectionBenchmark](#selectionbenchmark)
  - [StringBenchmark](#stringbenchmark)
  - [TreeBenchmark](#treebenchmark)
- [Implemented Sort Algorithm](#implemented-sort-algorithm)
  - [Exchange](#exchange)
  - [Selection](#selection)
  - [Insertion](#insertion)
  - [Merge](#merge)
  - [Heap](#heap)
  - [Partition](#partition)
  - [Distribution](#distribution)
  - [Network](#network)
  - [Tree](#tree)
  - [Joke](#joke)

## Benchmark

<!-- BENCHMARK_START -->
Interactive results: https://guitarrapc.github.io/SortAlgorithms/ (rankings per size, chosen algorithms compared across sizes, scaling exponents, run-to-run stability, history).

<picture>
  <source media="(prefers-color-scheme: dark)" srcset=".github/images/benchmark/pareto-dark.svg">
  <img alt="Mean time against run-to-run variation (CV) for every algorithm measured at n = 8192, Random input" src=".github/images/benchmark/pareto-light.svg">
</picture>

#### Fastest per category and input pattern

Measured at each category's largest size. "≈ 2nd" means the 99.9% confidence intervals of the two fastest overlap. 🟢 🟡 🔴 mark a CV below 2%, below 5%, and 5% or more. DotnetSort is a shared reference and is not ranked.

| Category | n | Random | 1-Moved | Sorted | Reversed | PipeOrgan | ManyDup |
|---|--:|---|---|---|---|---|---|
| Exchange | 1024 | **CombSort**<br>×2.91 vs 2nd 🟢 | **CocktailShakerSort**<br>×11.58 vs 2nd 🟢 | **CocktailShakerSort**<br>×1.29 vs 2nd 🟢 | **CombSort**<br>×1.15 vs 2nd 🟢 | **CombSort**<br>×6.04 vs 2nd 🟢 | **CombSort**<br>×3.53 vs 2nd 🟢 |
| Selection | 1024 | **SelectionSortSimd**<br>≈ 2nd 🟢 | **DoubleSelectionSortSimd**<br>×1.50 vs 2nd 🟢 | **DoubleSelectionSortSimd**<br>×1.52 vs 2nd 🟢 | **DoubleSelectionSortSimd**<br>×1.02 vs 2nd 🟢 | **DoubleSelectionSortSimd**<br>×1.62 vs 2nd 🟢 | **BingoSort**<br>×2.82 vs 2nd 🟢 |
| Insertion | 8192 | **ShellSortKnuth1973**<br>×1.08 vs 2nd 🟢 | **InsertionSort**<br>×1.14 vs 2nd 🟢 | **InsertionSort**<br>×1.24 vs 2nd 🟡 | **ShellSortKnuth1973**<br>×1.15 vs 2nd 🟢 | **ShellSortKnuth1973**<br>×1.13 vs 2nd 🟢 | **ShellSortSedgewick1986**<br>×1.02 vs 2nd 🟢 |
| Merge | 8192 | **SimdMergeSort**<br>×3.15 vs 2nd 🟢 | **TimSort**<br>×1.51 vs 2nd 🟡 | **Glidesort**<br>≈ 2nd 🟡 | **NaturalMergeSort**<br>≈ 2nd 🟢 | **Driftsort**<br>×1.18 vs 2nd 🟢 | **Driftsort**<br>×1.43 vs 2nd 🟢 |
| MergeSlow | 1024 | **StrandSort**<br>only one 🟢 | **StrandSort**<br>only one 🔴 | **StrandSort**<br>only one 🟢 | **StrandSort**<br>only one 🟢 | **StrandSort**<br>only one 🔴 | **StrandSort**<br>only one 🟢 |
| Heap | 8192 | **HeapSort**<br>×1.02 vs 2nd 🟢 | **SmoothSort**<br>×3.15 vs 2nd 🟢 | **SmoothSort**<br>×3.61 vs 2nd 🟢 | **PairingHeapSort**<br>×3.13 vs 2nd 🟢 | **PairingHeapSort**<br>×1.09 vs 2nd 🟢 | **HeapSort**<br>×1.02 vs 2nd 🟢 |
| Partition | 8192 | **SimdQuickSort**<br>×4.09 vs 2nd 🟢 | **FluxSort**<br>×1.31 vs 2nd 🟢 | **Ips4oSort**<br>×1.21 vs 2nd 🟡 | **Ips4oSort**<br>≈ 2nd 🟡 | **FluxSort**<br>×1.26 vs 2nd 🟢 | **SimdQuickSort**<br>×1.14 vs 2nd 🟢 |
| Distribution | 8192 | **PigeonSortInteger**<br>×1.41 vs 2nd 🟢 | **PigeonSortInteger**<br>×1.21 vs 2nd 🟢 | **SpreadSort**<br>×4.62 vs 2nd 🔴 | **PigeonSortInteger**<br>×1.39 vs 2nd 🟢 | **PigeonSortInteger**<br>×1.35 vs 2nd 🟢 | **PigeonSortInteger**<br>×1.30 vs 2nd 🟢 |
| Network | 8192 | **BitonicSortAnyLengthSimd**<br>×3.00 vs 2nd 🟢 | **BitonicSortAnyLengthSimd**<br>×2.56 vs 2nd 🟢 | **BitonicSortAnyLengthSimd**<br>×2.55 vs 2nd 🟢 | **BitonicSortAnyLengthSimd**<br>×2.56 vs 2nd 🟢 | **BitonicSortAnyLengthSimd**<br>×2.81 vs 2nd 🟢 | **BitonicSortAnyLengthSimd**<br>×3.09 vs 2nd 🟢 |
| Tree | 8192 | **BinaryTreeSort**<br>×1.23 vs 2nd 🟢 | **CartesianTreeSort**<br>×2.14 vs 2nd 🟢 | **CartesianTreeSort**<br>×2.01 vs 2nd 🟢 | **CartesianTreeSort**<br>×1.95 vs 2nd 🟢 | **CartesianTreeSort**<br>×2.04 vs 2nd 🟢 | **BalancedBinaryTreeSort**<br>×1.07 vs 2nd 🟢 |
| Scaling | 1048576 | **PigeonSortInteger**<br>×1.84 vs 2nd 🟢 | **FlatStableSort**<br>≈ 2nd 🟢 | **Ips4oSort**<br>≈ 2nd 🟢 | **Driftsort**<br>≈ 2nd 🟢 | **Driftsort**<br>×1.18 vs 2nd 🟢 | **Driftsort**<br>×1.07 vs 2nd 🟡 |

Measured 2026-11-23 10:32 UTC on AMD EPYC 9V74 3.69GHz in [workflow run](https://github.com/guitarrapc/SortAlgorithms/actions/runs/45146440301). The full BenchmarkDotNet tables are in that run's artifacts.

<!-- BENCHMARK_END -->


## Implemented Sort Algorithm

<!-- ALGORITHMS_START -->
### Exchange
- [Bubble Sort](./src/SortAlgorithm/Algorithms/Exchange/BubbleSort.cs)
- [Circle Sort](./src/SortAlgorithm/Algorithms/Exchange/CircleSort.cs)
- [Cocktail Shaker Sort](./src/SortAlgorithm/Algorithms/Exchange/CocktailShakerSort.cs)
- [Comb Sort](./src/SortAlgorithm/Algorithms/Exchange/CombSort.cs)
- [Odd-Even Sort](./src/SortAlgorithm/Algorithms/Exchange/OddEvenSort.cs)

### Selection
- [Bingo Sort](./src/SortAlgorithm/Algorithms/Selection/BingoSort.cs)
- [Cycle Sort](./src/SortAlgorithm/Algorithms/Selection/CycleSort.cs)
- [Double Selection Sort](./src/SortAlgorithm/Algorithms/Selection/DoubleSelectionSort.cs)
- [Double Selection Sort (SIMD)](./src/SortAlgorithm/Algorithms/Selection/DoubleSelectionSortSimd.cs)
- [Pancake Sort](./src/SortAlgorithm/Algorithms/Selection/PancakeSort.cs)
- [Selection Sort](./src/SortAlgorithm/Algorithms/Selection/SelectionSort.cs)
- [Selection Sort (SIMD)](./src/SortAlgorithm/Algorithms/Selection/SelectionSortSimd.cs)

### Insertion
- [Binary Insertion Sort](./src/SortAlgorithm/Algorithms/Insertion/BinaryInsertionSort.cs)
- [Gnome Sort](./src/SortAlgorithm/Algorithms/Insertion/GnomeSort.cs)
- [Insertion Sort](./src/SortAlgorithm/Algorithms/Insertion/InsertionSort.cs)
- [Library Sort](./src/SortAlgorithm/Algorithms/Insertion/LibrarySort.cs)
- [Merge Insertion Sort](./src/SortAlgorithm/Algorithms/Insertion/MergeInsertionSort.cs)
- [Pair Insertion Sort](./src/SortAlgorithm/Algorithms/Insertion/PairInsertionSort.cs)
- [Shell Sort](./src/SortAlgorithm/Algorithms/Insertion/ShellSort.cs)
  - Knuth1973
  - Sedgewick1986
  - Tokuda1992
  - Ciura2001
  - Lee2021

### Merge
- [Block Merge Sort](./src/SortAlgorithm/Algorithms/Merge/BlockMergeSort.cs)
- [Bottom-Up Merge Sort](./src/SortAlgorithm/Algorithms/Merge/BottomupMergeSort.cs)
- [Driftsort](./src/SortAlgorithm/Algorithms/Merge/Driftsort.cs)
- [Drop-Merge Sort](./src/SortAlgorithm/Algorithms/Merge/DropMergeSort.cs)
- [Flat Stable Sort](./src/SortAlgorithm/Algorithms/Merge/FlatStableSort.cs)
- [Glidesort](./src/SortAlgorithm/Algorithms/Merge/Glidesort.cs)
- [Merge Sort](./src/SortAlgorithm/Algorithms/Merge/MergeSort.cs)
- [Natural Merge Sort](./src/SortAlgorithm/Algorithms/Merge/NaturalMergeSort.cs)
- [Patience Sort](./src/SortAlgorithm/Algorithms/Merge/PatienceSort.cs)
- [Pingpong Merge Sort](./src/SortAlgorithm/Algorithms/Merge/PingpongMergeSort.cs)
- [Power Sort](./src/SortAlgorithm/Algorithms/Merge/PowerSort.cs)
- [Quad Sort](./src/SortAlgorithm/Algorithms/Merge/QuadSort.cs)
- [Rotate Merge Sort](./src/SortAlgorithm/Algorithms/Merge/RotateMergeSort.cs)
  - Iterative
  - Recursive
- [Shift Sort](./src/SortAlgorithm/Algorithms/Merge/ShiftSort.cs)
- [Merge Sort (SIMD, Chhugani)](./src/SortAlgorithm/Algorithms/Merge/SimdMergeSort.cs)
- [Spin Sort](./src/SortAlgorithm/Algorithms/Merge/SpinSort.cs)
- [std::stable_sort (LLVM)](./src/SortAlgorithm/Algorithms/Merge/StdStableSort.cs)
- [Strand Sort](./src/SortAlgorithm/Algorithms/Merge/StrandSort.cs)
- [SymMerge Sort](./src/SortAlgorithm/Algorithms/Merge/SymMergeSort.cs)
- [Tim Sort](./src/SortAlgorithm/Algorithms/Merge/TimSort.cs)

### Heap
- [Binomial Heap Sort](./src/SortAlgorithm/Algorithms/Heap/BinomialHeapSort.cs)
- [Bottom-Up Heap Sort](./src/SortAlgorithm/Algorithms/Heap/BottomupHeapSort.cs)
- [Heap Sort](./src/SortAlgorithm/Algorithms/Heap/HeapSort.cs)
- [Min-Heap Sort](./src/SortAlgorithm/Algorithms/Heap/MinHeapSort.cs)
- [Pairing Heap Sort](./src/SortAlgorithm/Algorithms/Heap/PairingHeapSort.cs)
- [Poplar Heap Sort](./src/SortAlgorithm/Algorithms/Heap/PoplarHeapSort.cs)
- [Smooth Sort](./src/SortAlgorithm/Algorithms/Heap/SmoothSort.cs)
- [Ternary Heap Sort](./src/SortAlgorithm/Algorithms/Heap/TernaryHeapSort.cs)
- [Tournament Sort](./src/SortAlgorithm/Algorithms/Heap/TournamentSort.cs)
- [Weak Heap Sort](./src/SortAlgorithm/Algorithms/Heap/WeakHeapSort.cs)

### Partition
- [Quick Sort (Bidirectional Stable)](./src/SortAlgorithm/Algorithms/Partition/BidirectionalStableQuickSort.cs)
- [Blit Sort](./src/SortAlgorithm/Algorithms/Partition/BlitSort.cs)
- [Block Quick Sort](./src/SortAlgorithm/Algorithms/Partition/BlockQuickSort.cs)
- [Crum Sort](./src/SortAlgorithm/Algorithms/Partition/CrumSort.cs)
- [Quick Sort (Destswap Stable)](./src/SortAlgorithm/Algorithms/Partition/DestswapStableQuickSort.cs)
- [Quick Sort (Dual Pivot)](./src/SortAlgorithm/Algorithms/Partition/DualPivotQuickSort.cs)
- [Flux Sort](./src/SortAlgorithm/Algorithms/Partition/FluxSort.cs)
- [Intro Sort](./src/SortAlgorithm/Algorithms/Partition/IntroSort.cs)
- [Intro Sort (Dotnet)](./src/SortAlgorithm/Algorithms/Partition/IntroSortDotnet.cs)
- [Ipnsort](./src/SortAlgorithm/Algorithms/Partition/Ipnsort.cs)
- [In-place Super Scalar Samplesort (IS⁴o)](./src/SortAlgorithm/Algorithms/Partition/Ips4oSort.cs)
- [Pattern-Defeating Quick Sort](./src/SortAlgorithm/Algorithms/Partition/PDQSort.cs)
- [PDQ Sort Branchless](./src/SortAlgorithm/Algorithms/Partition/PDQSortBranchless.cs)
- [Quick Sort](./src/SortAlgorithm/Algorithms/Partition/QuickSort.cs)
- [Quick Sort (3-Way)](./src/SortAlgorithm/Algorithms/Partition/QuickSort3way.cs)
- [Quick Sort (Median of 3)](./src/SortAlgorithm/Algorithms/Partition/QuickSortMedian3.cs)
- [Quick Sort (Median of 9)](./src/SortAlgorithm/Algorithms/Partition/QuickSortMedian9.cs)
- [Sample Sort](./src/SortAlgorithm/Algorithms/Partition/SampleSort.cs)
- [Quick Sort (SIMD, Bramas)](./src/SortAlgorithm/Algorithms/Partition/SimdQuickSort.cs)
- [Quick Sort (Stable)](./src/SortAlgorithm/Algorithms/Partition/StableQuickSort.cs)
- [std::sort (LLVM)](./src/SortAlgorithm/Algorithms/Partition/StdSort.cs)

### Distribution
- [American Flag Sort](./src/SortAlgorithm/Algorithms/Distribution/AmericanFlagSort.cs)
- [Bucket Sort](./src/SortAlgorithm/Algorithms/Distribution/BucketSort.cs)
- [Counting Sort](./src/SortAlgorithm/Algorithms/Distribution/CountingSort.cs)
- [Flash Sort](./src/SortAlgorithm/Algorithms/Distribution/FlashSort.cs)
- [Pigeonhole Sort](./src/SortAlgorithm/Algorithms/Distribution/PigeonholeSort.cs)
- [Radix LSD Sort (Base 10)](./src/SortAlgorithm/Algorithms/Distribution/RadixLSD10Sort.cs)
- [Radix LSD Sort (Base 256)](./src/SortAlgorithm/Algorithms/Distribution/RadixLSD256Sort.cs)
- [Radix LSD Sort (Base 4)](./src/SortAlgorithm/Algorithms/Distribution/RadixLSD4Sort.cs)
- [Radix MSD Sort (Base 10)](./src/SortAlgorithm/Algorithms/Distribution/RadixMSD10Sort.cs)
- [Radix MSD Sort (Base 4)](./src/SortAlgorithm/Algorithms/Distribution/RadixMSD4Sort.cs)
- [Radsort](./src/SortAlgorithm/Algorithms/Distribution/Radsort.cs)
- [Rotate LSD Radix Sort (Base 4)](./src/SortAlgorithm/Algorithms/Distribution/RotateLSDRadixSort.cs)
- [Ska Sort](./src/SortAlgorithm/Algorithms/Distribution/SkaSort.cs)
- [Spread Sort](./src/SortAlgorithm/Algorithms/Distribution/SpreadSort.cs)

### Network
- [Batcher Odd-Even Merge Sort](./src/SortAlgorithm/Algorithms/Network/BatcherOddEvenMergeSort.cs)
- [Batcher Odd-Even Merge Sort Branchless](./src/SortAlgorithm/Algorithms/Network/BatcherOddEvenMergeSortBranchless.cs)
- [Bitonic Sort](./src/SortAlgorithm/Algorithms/Network/BitonicSort.cs)
  - Iterative
  - Recursive
- [Bitonic Sort (Any Length)](./src/SortAlgorithm/Algorithms/Network/BitonicSortAnyLength.cs)
- [Bitonic Sort (Any Length, SIMD)](./src/SortAlgorithm/Algorithms/Network/BitonicSortAnyLengthSimd.cs)
- [Bitonic Sort Branchless](./src/SortAlgorithm/Algorithms/Network/BitonicSortBranchless.cs)
- [Pairwise Sorting Network](./src/SortAlgorithm/Algorithms/Network/PairwiseSortingNetwork.cs)
- [Pairwise Sorting Network Branchless](./src/SortAlgorithm/Algorithms/Network/PairwiseSortingNetworkBranchless.cs)

### Tree
- [B Plus Tree Sort](./src/SortAlgorithm/Algorithms/Tree/BPlusTreeSort.cs)
- [B Tree Sort](./src/SortAlgorithm/Algorithms/Tree/BTreeSort.cs)
- [Binary Tree Sort (AVL)](./src/SortAlgorithm/Algorithms/Tree/BalancedBinaryTreeSort.cs)
- [Binary Tree Sort (BST)](./src/SortAlgorithm/Algorithms/Tree/BinaryTreeSort.cs)
- [Cartesian Tree Sort](./src/SortAlgorithm/Algorithms/Tree/CartesianTreeSort.cs)
- [Splay Sort](./src/SortAlgorithm/Algorithms/Tree/SplaySort.cs)
- [Treap Sort](./src/SortAlgorithm/Algorithms/Tree/TreapSort.cs)

### Joke
- [Bogo Sort](./src/SortAlgorithm/Algorithms/Joke/BogoSort.cs)
- [Bozo Sort](./src/SortAlgorithm/Algorithms/Joke/BozoSort.cs)
- [Fun Sort](./src/SortAlgorithm/Algorithms/Joke/FunSort.cs)
- [Slow Sort](./src/SortAlgorithm/Algorithms/Joke/SlowSort.cs)
- [Stooge Sort](./src/SortAlgorithm/Algorithms/Joke/StoogeSort.cs)
<!-- ALGORITHMS_END -->
