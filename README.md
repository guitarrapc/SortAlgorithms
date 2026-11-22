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
Interactive results: https://guitarrapc.github.io/SortAlgorithms/ (rankings per size, scaling exponents, run-to-run stability, history).

<picture>
  <source media="(prefers-color-scheme: dark)" srcset=".github/images/benchmark/pareto-dark.svg">
  <img alt="Mean time against run-to-run variation (CV) for every algorithm measured at n = 8192, Random input" src=".github/images/benchmark/pareto-light.svg">
</picture>

#### Fastest per category and input pattern

Measured at each category's largest size. "≈ 2nd" means the 99.9% confidence intervals of the two fastest overlap. 🟢 🟡 🔴 mark a CV below 2%, below 5%, and 5% or more. DotnetSort is a shared reference and is not ranked.

| Category | n | Random | 1-Moved | Sorted | Reversed | PipeOrgan | ManyDup |
|---|--:|---|---|---|---|---|---|
| Exchange | 1024 | **CombSort**<br>×3.08 vs 2nd 🟢 | **CocktailShakerSort**<br>×10.21 vs 2nd 🔴 | **CocktailShakerSort**<br>×1.26 vs 2nd 🟢 | **CombSort**<br>×1.18 vs 2nd 🟢 | **CombSort**<br>×6.43 vs 2nd 🟢 | **CombSort**<br>×3.71 vs 2nd 🟢 |
| Selection | 1024 | **DoubleSelectionSortSimd**<br>×1.19 vs 2nd 🟢 | **DoubleSelectionSortSimd**<br>×1.59 vs 2nd 🟢 | **DoubleSelectionSortSimd**<br>×1.75 vs 2nd 🟢 | **SelectionSortSimd**<br>≈ 2nd 🟢 | **DoubleSelectionSortSimd**<br>×1.81 vs 2nd 🟢 | **BingoSort**<br>×2.56 vs 2nd 🟢 |
| Insertion | 8192 | **ShellSortKnuth1973**<br>×1.06 vs 2nd 🟢 | **InsertionSort**<br>×1.17 vs 2nd 🟢 | **InsertionSort**<br>×1.24 vs 2nd 🟡 | **ShellSortKnuth1973**<br>×1.15 vs 2nd 🟢 | **ShellSortKnuth1973**<br>×1.16 vs 2nd 🟢 | **ShellSortSedgewick1986**<br>×1.03 vs 2nd 🟢 |
| Merge | 8192 | **SimdMergeSort**<br>×1.31 vs 2nd 🟢 | **TimSort**<br>×1.34 vs 2nd 🟡 | **Glidesort**<br>≈ 2nd 🟡 | **TimSort**<br>≈ 2nd 🟡 | **Driftsort**<br>×1.14 vs 2nd 🟢 | **Driftsort**<br>×1.16 vs 2nd 🟢 |
| MergeSlow | 1024 | **StrandSort**<br>only one 🟢 | **StrandSort**<br>only one 🔴 | **StrandSort**<br>only one 🟡 | **StrandSort**<br>only one 🟢 | **StrandSort**<br>only one 🟢 | **StrandSort**<br>only one 🟢 |
| Heap | 8192 | **HeapSort**<br>×1.01 vs 2nd 🟢 | **SmoothSort**<br>×2.95 vs 2nd 🟢 | **SmoothSort**<br>×3.37 vs 2nd 🟢 | **PairingHeapSort**<br>×3.12 vs 2nd 🟢 | **PairingHeapSort**<br>×1.08 vs 2nd 🟢 | **HeapSort**<br>≈ 2nd 🟢 |
| Partition | 8192 | **SimdQuickSort**<br>×1.28 vs 2nd 🟢 | **FluxSort**<br>×1.34 vs 2nd 🟢 | **Ips4oSort**<br>×1.17 vs 2nd 🟡 | **Ips4oSort**<br>≈ 2nd 🟡 | **FluxSort**<br>×1.26 vs 2nd 🟢 | **PDQSortBranchless**<br>×1.16 vs 2nd 🟢 |
| Distribution | 8192 | **PigeonSortInteger**<br>×1.39 vs 2nd 🟢 | **PigeonSortInteger**<br>×1.36 vs 2nd 🟡 | **SpreadSort**<br>×4.91 vs 2nd 🟡 | **PigeonSortInteger**<br>×1.42 vs 2nd 🟢 | **PigeonSortInteger**<br>×1.26 vs 2nd 🟢 | **PigeonSortInteger**<br>×1.28 vs 2nd 🟢 |
| Network | 8192 | **BitonicSortAnyLengthSimd**<br>×1.58 vs 2nd 🟢 | **BitonicSortAnyLengthSimd**<br>×1.28 vs 2nd 🟢 | **BitonicSortAnyLengthSimd**<br>×1.34 vs 2nd 🟢 | **BitonicSortAnyLengthSimd**<br>×1.32 vs 2nd 🟢 | **BitonicSortAnyLengthSimd**<br>×1.33 vs 2nd 🟢 | **BitonicSortAnyLengthSimd**<br>×1.58 vs 2nd 🟢 |
| Tree | 8192 | **BinaryTreeSort**<br>×1.27 vs 2nd 🟢 | **CartesianTreeSort**<br>×2.13 vs 2nd 🟢 | **CartesianTreeSort**<br>×1.92 vs 2nd 🟢 | **CartesianTreeSort**<br>×1.95 vs 2nd 🟢 | **CartesianTreeSort**<br>×1.99 vs 2nd 🟢 | **BalancedBinaryTreeSort**<br>×1.06 vs 2nd 🟢 |
| Scaling | 1048576 | **PigeonSortInteger**<br>×1.78 vs 2nd 🟢 | **TimSort**<br>≈ 2nd 🟢 | **Ips4oSort**<br>≈ 2nd 🟢 | **Driftsort**<br>≈ 2nd 🟢 | **Driftsort**<br>×1.18 vs 2nd 🟢 | **PigeonSortInteger**<br>×1.35 vs 2nd 🟢 |

<details>
<summary>Benchmark results (2026-11-22 20:28 UTC)</summary>

Workflow run: https://github.com/guitarrapc/SortAlgorithms/actions/runs/45053462105

### DistributionBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.86GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.22 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method              | Size | Pattern            | Mean           | Error        | StdDev      | Ratio  | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------------- |----- |------------------- |---------------:|-------------:|------------:|-------:|--------:|-----:|----------:|------------:|
| **CountingSort**        | **256**  | **Random**             |     **1,681.9 ns** |     **28.36 ns** |    **12.59 ns** |   **1.44** |    **0.01** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | Random             |     1,164.5 ns |     16.89 ns |     8.83 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 256  | Random             |     1,493.0 ns |     23.12 ns |    12.09 ns |   1.28 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | Random             |       911.4 ns |     27.72 ns |    12.31 ns |   0.78 |    0.01 |    1 |         - |          NA |
| BucketSort          | 256  | Random             |     1,838.1 ns |     38.16 ns |    16.94 ns |   1.58 |    0.02 |    2 |         - |          NA |
| BucketSortInteger   | 256  | Random             |     1,537.7 ns |     22.99 ns |    12.02 ns |   1.32 |    0.01 |    2 |         - |          NA |
| FlashSort           | 256  | Random             |     5,433.6 ns |    271.77 ns |   142.14 ns |   4.67 |    0.12 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | Random             |     3,049.5 ns |    362.24 ns |   189.46 ns |   2.62 |    0.15 |    3 |         - |          NA |
| RotateLSDRadixSort  | 256  | Random             |    68,929.3 ns |    664.36 ns |   294.98 ns |  59.20 |    0.49 |    7 |         - |          NA |
| RadixLSD256Sort     | 256  | Random             |     1,332.7 ns |     25.08 ns |     8.94 ns |   1.14 |    0.01 |    2 |         - |          NA |
| Radsort             | 256  | Random             |    10,153.7 ns |    547.64 ns |   195.29 ns |   8.72 |    0.17 |    6 |         - |          NA |
| RadixLSD10Sort      | 256  | Random             |     4,238.4 ns |    309.48 ns |   161.87 ns |   3.64 |    0.13 |    4 |         - |          NA |
| RadixMSD4Sort       | 256  | Random             |     6,940.7 ns |    311.59 ns |   162.97 ns |   5.96 |    0.14 |    5 |         - |          NA |
| RadixMSD10Sort      | 256  | Random             |     6,029.5 ns |     66.53 ns |    34.80 ns |   5.18 |    0.05 |    5 |         - |          NA |
| AmericanFlagSort    | 256  | Random             |     3,227.6 ns |    243.32 ns |   127.26 ns |   2.77 |    0.10 |    3 |         - |          NA |
| SkaSort             | 256  | Random             |     4,382.6 ns |    302.82 ns |   158.38 ns |   3.76 |    0.13 |    4 |         - |          NA |
| SpreadSort          | 256  | Random             |     7,925.2 ns |     97.40 ns |    50.94 ns |   6.81 |    0.06 |    5 |         - |          NA |
| DotnetSort          | 256  | Random             |     7,559.7 ns |    463.75 ns |   242.55 ns |   6.49 |    0.20 |    5 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **256**  | **SingleElementMoved** |     **1,573.3 ns** |    **101.56 ns** |    **36.22 ns** |   **1.67** |    **0.04** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | SingleElementMoved |       944.8 ns |      9.65 ns |     4.29 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 256  | SingleElementMoved |     1,522.1 ns |      8.72 ns |     3.11 ns |   1.61 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | SingleElementMoved |       736.8 ns |     45.01 ns |    23.54 ns |   0.78 |    0.02 |    1 |         - |          NA |
| BucketSort          | 256  | SingleElementMoved |     1,763.4 ns |     12.13 ns |     5.39 ns |   1.87 |    0.01 |    2 |         - |          NA |
| BucketSortInteger   | 256  | SingleElementMoved |     1,551.2 ns |     10.26 ns |     4.56 ns |   1.64 |    0.01 |    2 |         - |          NA |
| FlashSort           | 256  | SingleElementMoved |     5,263.7 ns |    259.45 ns |   115.20 ns |   5.57 |    0.12 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | SingleElementMoved |     2,899.6 ns |     74.06 ns |    32.88 ns |   3.07 |    0.04 |    4 |         - |          NA |
| RotateLSDRadixSort  | 256  | SingleElementMoved |    44,947.8 ns |    550.86 ns |   244.59 ns |  47.57 |    0.31 |    7 |         - |          NA |
| RadixLSD256Sort     | 256  | SingleElementMoved |     1,165.5 ns |     82.61 ns |    36.68 ns |   1.23 |    0.04 |    2 |         - |          NA |
| Radsort             | 256  | SingleElementMoved |     9,482.5 ns |    314.93 ns |   164.71 ns |  10.04 |    0.17 |    6 |         - |          NA |
| RadixLSD10Sort      | 256  | SingleElementMoved |     4,447.8 ns |    230.88 ns |   120.76 ns |   4.71 |    0.12 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | SingleElementMoved |     7,317.6 ns |    527.04 ns |   275.65 ns |   7.75 |    0.28 |    5 |         - |          NA |
| RadixMSD10Sort      | 256  | SingleElementMoved |     6,069.8 ns |     19.49 ns |    10.20 ns |   6.42 |    0.03 |    5 |         - |          NA |
| AmericanFlagSort    | 256  | SingleElementMoved |     2,132.9 ns |     41.01 ns |    18.21 ns |   2.26 |    0.02 |    3 |         - |          NA |
| SkaSort             | 256  | SingleElementMoved |     3,146.3 ns |     31.60 ns |    11.27 ns |   3.33 |    0.02 |    4 |         - |          NA |
| SpreadSort          | 256  | SingleElementMoved |     1,055.3 ns |    147.61 ns |    65.54 ns |   1.12 |    0.07 |    2 |         - |          NA |
| DotnetSort          | 256  | SingleElementMoved |     1,355.0 ns |    170.91 ns |    89.39 ns |   1.43 |    0.09 |    2 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **256**  | **Sorted**             |     **1,580.5 ns** |     **59.64 ns** |    **26.48 ns** |   **1.65** |    **0.03** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | Sorted             |       957.7 ns |     11.57 ns |     4.13 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 256  | Sorted             |     1,412.8 ns |     11.22 ns |     4.98 ns |   1.48 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | Sorted             |       693.2 ns |      6.13 ns |     3.21 ns |   0.72 |    0.00 |    2 |         - |          NA |
| BucketSort          | 256  | Sorted             |     1,673.6 ns |     14.77 ns |     7.72 ns |   1.75 |    0.01 |    3 |         - |          NA |
| BucketSortInteger   | 256  | Sorted             |     1,351.8 ns |     17.95 ns |     7.97 ns |   1.41 |    0.01 |    3 |         - |          NA |
| FlashSort           | 256  | Sorted             |     4,572.2 ns |     28.49 ns |    12.65 ns |   4.77 |    0.02 |    6 |         - |          NA |
| RadixLSD4Sort       | 256  | Sorted             |     2,861.7 ns |    272.37 ns |   120.93 ns |   2.99 |    0.12 |    5 |         - |          NA |
| RotateLSDRadixSort  | 256  | Sorted             |    44,307.7 ns |    236.11 ns |   123.49 ns |  46.27 |    0.22 |    9 |         - |          NA |
| RadixLSD256Sort     | 256  | Sorted             |     2,067.6 ns |     72.94 ns |    32.39 ns |   2.16 |    0.03 |    4 |         - |          NA |
| Radsort             | 256  | Sorted             |     9,318.0 ns |    323.45 ns |   169.17 ns |   9.73 |    0.17 |    8 |         - |          NA |
| RadixLSD10Sort      | 256  | Sorted             |     3,895.7 ns |     18.68 ns |     6.66 ns |   4.07 |    0.02 |    6 |         - |          NA |
| RadixMSD4Sort       | 256  | Sorted             |     7,207.9 ns |    348.93 ns |   182.50 ns |   7.53 |    0.18 |    7 |         - |          NA |
| RadixMSD10Sort      | 256  | Sorted             |     6,066.5 ns |    184.65 ns |    81.98 ns |   6.33 |    0.08 |    7 |         - |          NA |
| AmericanFlagSort    | 256  | Sorted             |     1,700.7 ns |     21.19 ns |     7.55 ns |   1.78 |    0.01 |    3 |         - |          NA |
| SkaSort             | 256  | Sorted             |     2,872.8 ns |     50.22 ns |    17.91 ns |   3.00 |    0.02 |    5 |         - |          NA |
| SpreadSort          | 256  | Sorted             |       423.8 ns |     28.24 ns |    12.54 ns |   0.44 |    0.01 |    1 |         - |          NA |
| DotnetSort          | 256  | Sorted             |       811.4 ns |     11.22 ns |     4.00 ns |   0.85 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **256**  | **Reversed**           |     **1,599.0 ns** |     **18.11 ns** |     **8.04 ns** |   **1.80** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | Reversed           |       889.7 ns |     11.17 ns |     5.84 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 256  | Reversed           |     1,389.1 ns |     21.22 ns |     9.42 ns |   1.56 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | Reversed           |       653.1 ns |     11.81 ns |     5.24 ns |   0.73 |    0.01 |    2 |         - |          NA |
| BucketSort          | 256  | Reversed           |     1,750.0 ns |     61.46 ns |    27.29 ns |   1.97 |    0.03 |    3 |         - |          NA |
| BucketSortInteger   | 256  | Reversed           |     1,462.7 ns |     16.35 ns |     7.26 ns |   1.64 |    0.01 |    3 |         - |          NA |
| FlashSort           | 256  | Reversed           |     4,667.5 ns |     94.97 ns |    49.67 ns |   5.25 |    0.06 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | Reversed           |     2,905.9 ns |     30.28 ns |    15.84 ns |   3.27 |    0.03 |    4 |         - |          NA |
| RotateLSDRadixSort  | 256  | Reversed           |    45,023.0 ns |    280.05 ns |   146.47 ns |  50.60 |    0.35 |    8 |         - |          NA |
| RadixLSD256Sort     | 256  | Reversed           |     1,065.9 ns |      8.70 ns |     3.86 ns |   1.20 |    0.01 |    3 |         - |          NA |
| Radsort             | 256  | Reversed           |     9,595.3 ns |    348.30 ns |   182.17 ns |  10.78 |    0.20 |    7 |         - |          NA |
| RadixLSD10Sort      | 256  | Reversed           |     4,035.5 ns |     32.80 ns |    11.70 ns |   4.54 |    0.03 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | Reversed           |     7,262.6 ns |    211.28 ns |   110.50 ns |   8.16 |    0.13 |    6 |         - |          NA |
| RadixMSD10Sort      | 256  | Reversed           |     6,304.9 ns |    315.26 ns |   164.89 ns |   7.09 |    0.18 |    6 |         - |          NA |
| AmericanFlagSort    | 256  | Reversed           |     1,703.8 ns |     13.43 ns |     4.79 ns |   1.92 |    0.01 |    3 |         - |          NA |
| SkaSort             | 256  | Reversed           |     2,861.0 ns |     47.95 ns |    17.10 ns |   3.22 |    0.03 |    4 |         - |          NA |
| SpreadSort          | 256  | Reversed           |       479.5 ns |      6.73 ns |     2.99 ns |   0.54 |    0.00 |    1 |         - |          NA |
| DotnetSort          | 256  | Reversed           |     1,313.6 ns |    202.45 ns |    89.89 ns |   1.48 |    0.09 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **256**  | **PipeOrgan**          |     **1,545.7 ns** |     **13.70 ns** |     **6.08 ns** |   **1.54** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | PipeOrgan          |     1,004.3 ns |      8.90 ns |     4.65 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 256  | PipeOrgan          |     1,494.6 ns |     15.08 ns |     7.89 ns |   1.49 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | PipeOrgan          |       826.0 ns |     15.73 ns |     6.98 ns |   0.82 |    0.01 |    1 |         - |          NA |
| BucketSort          | 256  | PipeOrgan          |     1,736.2 ns |     39.43 ns |    14.06 ns |   1.73 |    0.01 |    3 |         - |          NA |
| BucketSortInteger   | 256  | PipeOrgan          |     1,640.9 ns |     24.27 ns |     8.65 ns |   1.63 |    0.01 |    3 |         - |          NA |
| FlashSort           | 256  | PipeOrgan          |     4,783.4 ns |     47.53 ns |    21.10 ns |   4.76 |    0.03 |    6 |         - |          NA |
| RadixLSD4Sort       | 256  | PipeOrgan          |     2,862.7 ns |    325.60 ns |   170.30 ns |   2.85 |    0.16 |    4 |         - |          NA |
| RotateLSDRadixSort  | 256  | PipeOrgan          |    44,741.8 ns |    267.97 ns |   140.15 ns |  44.55 |    0.23 |   10 |         - |          NA |
| RadixLSD256Sort     | 256  | PipeOrgan          |     1,266.9 ns |     34.14 ns |    15.16 ns |   1.26 |    0.02 |    3 |         - |          NA |
| Radsort             | 256  | PipeOrgan          |     9,418.0 ns |    362.48 ns |   189.58 ns |   9.38 |    0.18 |    9 |         - |          NA |
| RadixLSD10Sort      | 256  | PipeOrgan          |     3,961.8 ns |     48.27 ns |    17.22 ns |   3.94 |    0.02 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | PipeOrgan          |     7,442.8 ns |    345.84 ns |   180.88 ns |   7.41 |    0.17 |    8 |         - |          NA |
| RadixMSD10Sort      | 256  | PipeOrgan          |     6,113.4 ns |     31.22 ns |    13.86 ns |   6.09 |    0.03 |    7 |         - |          NA |
| AmericanFlagSort    | 256  | PipeOrgan          |     2,499.4 ns |     14.07 ns |     6.25 ns |   2.49 |    0.01 |    4 |         - |          NA |
| SkaSort             | 256  | PipeOrgan          |     3,743.5 ns |     42.25 ns |    15.07 ns |   3.73 |    0.02 |    5 |         - |          NA |
| SpreadSort          | 256  | PipeOrgan          |     1,569.6 ns |     46.67 ns |    20.72 ns |   1.56 |    0.02 |    3 |         - |          NA |
| DotnetSort          | 256  | PipeOrgan          |     2,514.5 ns |    323.19 ns |   169.04 ns |   2.50 |    0.16 |    4 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **256**  | **ManyDuplicates**     |     **1,562.8 ns** |     **61.95 ns** |    **22.09 ns** |   **1.58** |    **0.03** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | ManyDuplicates     |       988.2 ns |     29.74 ns |    13.20 ns |   1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 256  | ManyDuplicates     |     1,796.9 ns |    613.23 ns |   320.73 ns |   1.82 |    0.31 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | ManyDuplicates     |       957.0 ns |    189.59 ns |    99.16 ns |   0.97 |    0.10 |    1 |         - |          NA |
| BucketSort          | 256  | ManyDuplicates     |     2,855.5 ns |    101.07 ns |    44.88 ns |   2.89 |    0.06 |    4 |         - |          NA |
| BucketSortInteger   | 256  | ManyDuplicates     |     1,731.4 ns |    128.52 ns |    57.06 ns |   1.75 |    0.06 |    2 |         - |          NA |
| FlashSort           | 256  | ManyDuplicates     |     5,205.9 ns |     63.01 ns |    27.98 ns |   5.27 |    0.07 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | ManyDuplicates     |     2,292.2 ns |     29.37 ns |    15.36 ns |   2.32 |    0.03 |    3 |         - |          NA |
| RotateLSDRadixSort  | 256  | ManyDuplicates     |    60,136.7 ns |    105.43 ns |    46.81 ns |  60.86 |    0.76 |    8 |         - |          NA |
| RadixLSD256Sort     | 256  | ManyDuplicates     |     1,334.6 ns |     27.29 ns |    12.12 ns |   1.35 |    0.02 |    2 |         - |          NA |
| Radsort             | 256  | ManyDuplicates     |    11,455.6 ns |    201.96 ns |    89.67 ns |  11.59 |    0.17 |    7 |         - |          NA |
| RadixLSD10Sort      | 256  | ManyDuplicates     |     2,871.7 ns |     16.85 ns |     7.48 ns |   2.91 |    0.04 |    4 |         - |          NA |
| RadixMSD4Sort       | 256  | ManyDuplicates     |     3,377.6 ns |     33.21 ns |    14.74 ns |   3.42 |    0.04 |    4 |         - |          NA |
| RadixMSD10Sort      | 256  | ManyDuplicates     |     3,743.5 ns |     53.84 ns |    19.20 ns |   3.79 |    0.05 |    4 |         - |          NA |
| AmericanFlagSort    | 256  | ManyDuplicates     |     3,389.7 ns |    378.54 ns |   197.98 ns |   3.43 |    0.19 |    4 |         - |          NA |
| SkaSort             | 256  | ManyDuplicates     |     3,985.9 ns |    185.51 ns |    82.37 ns |   4.03 |    0.09 |    4 |         - |          NA |
| SpreadSort          | 256  | ManyDuplicates     |     7,085.4 ns |    220.81 ns |    98.04 ns |   7.17 |    0.13 |    6 |         - |          NA |
| DotnetSort          | 256  | ManyDuplicates     |     7,147.9 ns |    296.73 ns |   155.19 ns |   7.23 |    0.17 |    6 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **1024** | **Random**             |     **5,989.2 ns** |     **52.17 ns** |    **18.60 ns** |   **1.40** |    **0.01** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Random             |     4,279.5 ns |     54.67 ns |    19.50 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | Random             |     5,063.7 ns |     34.94 ns |    12.46 ns |   1.18 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | Random             |     3,257.8 ns |     25.19 ns |    11.18 ns |   0.76 |    0.00 |    1 |         - |          NA |
| BucketSort          | 1024 | Random             |     7,202.5 ns |    300.99 ns |   157.42 ns |   1.68 |    0.04 |    3 |         - |          NA |
| BucketSortInteger   | 1024 | Random             |     5,515.7 ns |     32.24 ns |    11.50 ns |   1.29 |    0.01 |    2 |         - |          NA |
| FlashSort           | 1024 | Random             |    22,023.9 ns |    292.16 ns |   152.80 ns |   5.15 |    0.04 |    5 |         - |          NA |
| RadixLSD4Sort       | 1024 | Random             |    13,727.4 ns |    213.81 ns |   111.83 ns |   3.21 |    0.03 |    4 |         - |          NA |
| RotateLSDRadixSort  | 1024 | Random             |   364,288.5 ns |  1,480.72 ns |   657.45 ns |  85.13 |    0.39 |    8 |         - |          NA |
| RadixLSD256Sort     | 1024 | Random             |     7,579.8 ns |    375.05 ns |   196.16 ns |   1.77 |    0.04 |    3 |         - |          NA |
| Radsort             | 1024 | Random             |    38,730.8 ns |    358.40 ns |   187.45 ns |   9.05 |    0.06 |    7 |         - |          NA |
| RadixLSD10Sort      | 1024 | Random             |    20,886.9 ns |    234.98 ns |   122.90 ns |   4.88 |    0.03 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | Random             |    31,288.9 ns |    226.40 ns |   118.41 ns |   7.31 |    0.04 |    6 |         - |          NA |
| RadixMSD10Sort      | 1024 | Random             |    29,373.3 ns |    296.25 ns |   154.94 ns |   6.86 |    0.04 |    6 |         - |          NA |
| AmericanFlagSort    | 1024 | Random             |    22,124.6 ns |    263.20 ns |   137.66 ns |   5.17 |    0.04 |    5 |         - |          NA |
| SkaSort             | 1024 | Random             |    16,044.9 ns |    236.75 ns |   105.12 ns |   3.75 |    0.03 |    4 |         - |          NA |
| SpreadSort          | 1024 | Random             |     8,931.9 ns |    441.15 ns |   230.73 ns |   2.09 |    0.05 |    3 |         - |          NA |
| DotnetSort          | 1024 | Random             |    38,711.3 ns |    479.99 ns |   251.04 ns |   9.05 |    0.07 |    7 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **1024** | **SingleElementMoved** |     **5,910.9 ns** |    **314.47 ns** |   **164.47 ns** |   **1.71** |    **0.05** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | SingleElementMoved |     3,448.1 ns |     54.31 ns |    24.12 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | SingleElementMoved |     5,380.3 ns |    282.93 ns |   147.98 ns |   1.56 |    0.04 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | SingleElementMoved |     2,506.3 ns |     12.02 ns |     6.29 ns |   0.73 |    0.01 |    1 |         - |          NA |
| BucketSort          | 1024 | SingleElementMoved |     6,940.4 ns |    460.18 ns |   240.68 ns |   2.01 |    0.07 |    3 |         - |          NA |
| BucketSortInteger   | 1024 | SingleElementMoved |     5,860.3 ns |    268.03 ns |   140.19 ns |   1.70 |    0.04 |    3 |         - |          NA |
| FlashSort           | 1024 | SingleElementMoved |    19,109.4 ns |    234.55 ns |   104.14 ns |   5.54 |    0.05 |    5 |         - |          NA |
| RadixLSD4Sort       | 1024 | SingleElementMoved |    20,752.3 ns |    218.57 ns |   114.31 ns |   6.02 |    0.05 |    5 |         - |          NA |
| RotateLSDRadixSort  | 1024 | SingleElementMoved |   228,242.2 ns |  1,369.40 ns |   608.02 ns |  66.20 |    0.46 |    6 |         - |          NA |
| RadixLSD256Sort     | 1024 | SingleElementMoved |     6,367.9 ns |    508.08 ns |   265.74 ns |   1.85 |    0.07 |    3 |         - |          NA |
| Radsort             | 1024 | SingleElementMoved |    24,731.4 ns |    290.33 ns |   151.85 ns |   7.17 |    0.06 |    5 |         - |          NA |
| RadixLSD10Sort      | 1024 | SingleElementMoved |    21,260.7 ns |    360.01 ns |   188.29 ns |   6.17 |    0.07 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | SingleElementMoved |    31,509.3 ns |    125.49 ns |    65.63 ns |   9.14 |    0.06 |    5 |         - |          NA |
| RadixMSD10Sort      | 1024 | SingleElementMoved |    29,004.1 ns |    191.51 ns |    85.03 ns |   8.41 |    0.06 |    5 |         - |          NA |
| AmericanFlagSort    | 1024 | SingleElementMoved |    11,313.5 ns |    221.66 ns |    98.42 ns |   3.28 |    0.03 |    4 |         - |          NA |
| SkaSort             | 1024 | SingleElementMoved |    10,621.7 ns |    354.52 ns |   185.42 ns |   3.08 |    0.05 |    4 |         - |          NA |
| SpreadSort          | 1024 | SingleElementMoved |     5,924.3 ns |    375.37 ns |   196.33 ns |   1.72 |    0.05 |    3 |         - |          NA |
| DotnetSort          | 1024 | SingleElementMoved |     6,898.7 ns |    458.27 ns |   239.68 ns |   2.00 |    0.07 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **1024** | **Sorted**             |     **6,560.8 ns** |    **358.21 ns** |   **187.35 ns** |   **1.78** |    **0.05** |    **5** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Sorted             |     3,689.5 ns |     60.13 ns |    21.44 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 1024 | Sorted             |     5,195.7 ns |    542.97 ns |   283.99 ns |   1.41 |    0.07 |    4 |         - |          NA |
| PigeonSortInteger   | 1024 | Sorted             |     2,814.8 ns |     35.63 ns |    12.71 ns |   0.76 |    0.01 |    2 |         - |          NA |
| BucketSort          | 1024 | Sorted             |     7,371.6 ns |    493.23 ns |   257.97 ns |   2.00 |    0.07 |    5 |         - |          NA |
| BucketSortInteger   | 1024 | Sorted             |     5,022.1 ns |    317.81 ns |   166.22 ns |   1.36 |    0.04 |    4 |         - |          NA |
| FlashSort           | 1024 | Sorted             |    17,625.2 ns |     57.26 ns |    25.42 ns |   4.78 |    0.03 |    7 |         - |          NA |
| RadixLSD4Sort       | 1024 | Sorted             |    17,627.2 ns |    468.85 ns |   245.22 ns |   4.78 |    0.07 |    7 |         - |          NA |
| RotateLSDRadixSort  | 1024 | Sorted             |   225,870.0 ns |    888.25 ns |   394.39 ns |  61.22 |    0.34 |    8 |         - |          NA |
| RadixLSD256Sort     | 1024 | Sorted             |     6,192.6 ns |    203.64 ns |    90.42 ns |   1.68 |    0.02 |    5 |         - |          NA |
| Radsort             | 1024 | Sorted             |    24,641.2 ns |    512.85 ns |   268.23 ns |   6.68 |    0.08 |    7 |         - |          NA |
| RadixLSD10Sort      | 1024 | Sorted             |    20,527.7 ns |    259.92 ns |   135.94 ns |   5.56 |    0.05 |    7 |         - |          NA |
| RadixMSD4Sort       | 1024 | Sorted             |    30,165.6 ns |    254.96 ns |   113.21 ns |   8.18 |    0.05 |    7 |         - |          NA |
| RadixMSD10Sort      | 1024 | Sorted             |    28,689.9 ns |    428.56 ns |   224.15 ns |   7.78 |    0.07 |    7 |         - |          NA |
| AmericanFlagSort    | 1024 | Sorted             |     9,973.1 ns |    414.32 ns |   183.96 ns |   2.70 |    0.05 |    6 |         - |          NA |
| SkaSort             | 1024 | Sorted             |     9,049.8 ns |    343.04 ns |   179.42 ns |   2.45 |    0.05 |    6 |         - |          NA |
| SpreadSort          | 1024 | Sorted             |       665.3 ns |     15.78 ns |     7.00 ns |   0.18 |    0.00 |    1 |         - |          NA |
| DotnetSort          | 1024 | Sorted             |     4,154.3 ns |    293.25 ns |   153.38 ns |   1.13 |    0.04 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **1024** | **Reversed**           |     **6,183.4 ns** |    **286.10 ns** |   **149.64 ns** |   **1.76** |    **0.09** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Reversed           |     3,515.3 ns |    344.34 ns |   180.09 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | Reversed           |     5,062.2 ns |    348.53 ns |   182.29 ns |   1.44 |    0.08 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | Reversed           |     2,458.7 ns |      9.10 ns |     4.76 ns |   0.70 |    0.03 |    1 |         - |          NA |
| BucketSort          | 1024 | Reversed           |     6,396.6 ns |    363.86 ns |   190.31 ns |   1.82 |    0.10 |    3 |         - |          NA |
| BucketSortInteger   | 1024 | Reversed           |     5,348.1 ns |     44.43 ns |    15.84 ns |   1.52 |    0.07 |    3 |         - |          NA |
| FlashSort           | 1024 | Reversed           |    17,027.7 ns |    145.10 ns |    64.43 ns |   4.85 |    0.23 |    5 |         - |          NA |
| RadixLSD4Sort       | 1024 | Reversed           |    14,857.8 ns |    550.17 ns |   287.75 ns |   4.24 |    0.21 |    5 |         - |          NA |
| RotateLSDRadixSort  | 1024 | Reversed           |   230,373.5 ns |  1,359.53 ns |   603.64 ns |  65.68 |    3.10 |    8 |         - |          NA |
| RadixLSD256Sort     | 1024 | Reversed           |     6,212.5 ns |    431.28 ns |   225.57 ns |   1.77 |    0.10 |    3 |         - |          NA |
| Radsort             | 1024 | Reversed           |    23,988.5 ns |    151.18 ns |    79.07 ns |   6.84 |    0.32 |    6 |         - |          NA |
| RadixLSD10Sort      | 1024 | Reversed           |    20,890.2 ns |    241.80 ns |   107.36 ns |   5.96 |    0.28 |    6 |         - |          NA |
| RadixMSD4Sort       | 1024 | Reversed           |    31,360.9 ns |    406.00 ns |   212.35 ns |   8.94 |    0.42 |    7 |         - |          NA |
| RadixMSD10Sort      | 1024 | Reversed           |    29,034.7 ns |    351.74 ns |   183.97 ns |   8.28 |    0.39 |    7 |         - |          NA |
| AmericanFlagSort    | 1024 | Reversed           |    10,580.7 ns |    320.02 ns |   167.38 ns |   3.02 |    0.15 |    4 |         - |          NA |
| SkaSort             | 1024 | Reversed           |    10,071.4 ns |    391.58 ns |   204.80 ns |   2.87 |    0.15 |    4 |         - |          NA |
| SpreadSort          | 1024 | Reversed           |     4,757.1 ns |     24.24 ns |     8.65 ns |   1.36 |    0.06 |    3 |         - |          NA |
| DotnetSort          | 1024 | Reversed           |     6,858.6 ns |    267.29 ns |   118.68 ns |   1.96 |    0.10 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **1024** | **PipeOrgan**          |     **5,941.0 ns** |    **360.47 ns** |   **188.53 ns** |   **1.50** |    **0.05** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | PipeOrgan          |     3,954.1 ns |     48.63 ns |    17.34 ns |   1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 1024 | PipeOrgan          |     4,993.4 ns |     34.45 ns |    12.28 ns |   1.26 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | PipeOrgan          |     3,340.1 ns |    369.52 ns |   193.27 ns |   0.84 |    0.05 |    1 |         - |          NA |
| BucketSort          | 1024 | PipeOrgan          |     7,656.1 ns |    319.60 ns |   167.16 ns |   1.94 |    0.04 |    2 |         - |          NA |
| BucketSortInteger   | 1024 | PipeOrgan          |     5,635.8 ns |     53.01 ns |    18.90 ns |   1.43 |    0.01 |    2 |         - |          NA |
| FlashSort           | 1024 | PipeOrgan          |    19,111.7 ns |     77.50 ns |    34.41 ns |   4.83 |    0.02 |    3 |         - |          NA |
| RadixLSD4Sort       | 1024 | PipeOrgan          |    17,069.5 ns |    257.48 ns |   134.67 ns |   4.32 |    0.04 |    3 |         - |          NA |
| RotateLSDRadixSort  | 1024 | PipeOrgan          |   244,001.5 ns |  2,096.61 ns | 1,096.57 ns |  61.71 |    0.36 |    4 |         - |          NA |
| RadixLSD256Sort     | 1024 | PipeOrgan          |     6,723.7 ns |    260.82 ns |   136.41 ns |   1.70 |    0.03 |    2 |         - |          NA |
| Radsort             | 1024 | PipeOrgan          |    24,422.7 ns |    425.78 ns |   222.69 ns |   6.18 |    0.06 |    3 |         - |          NA |
| RadixLSD10Sort      | 1024 | PipeOrgan          |    20,501.7 ns |    287.44 ns |   127.63 ns |   5.18 |    0.04 |    3 |         - |          NA |
| RadixMSD4Sort       | 1024 | PipeOrgan          |    31,417.6 ns |    108.36 ns |    48.11 ns |   7.95 |    0.03 |    3 |         - |          NA |
| RadixMSD10Sort      | 1024 | PipeOrgan          |    29,324.7 ns |    227.09 ns |   118.77 ns |   7.42 |    0.04 |    3 |         - |          NA |
| AmericanFlagSort    | 1024 | PipeOrgan          |    15,318.9 ns |    137.79 ns |    72.07 ns |   3.87 |    0.02 |    3 |         - |          NA |
| SkaSort             | 1024 | PipeOrgan          |    14,141.7 ns |    152.34 ns |    67.64 ns |   3.58 |    0.02 |    3 |         - |          NA |
| SpreadSort          | 1024 | PipeOrgan          |     6,405.6 ns |    458.75 ns |   239.93 ns |   1.62 |    0.06 |    2 |         - |          NA |
| DotnetSort          | 1024 | PipeOrgan          |    17,021.9 ns |  1,058.21 ns |   469.85 ns |   4.30 |    0.11 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **1024** | **ManyDuplicates**     |     **6,330.4 ns** |    **255.40 ns** |   **133.58 ns** |   **1.69** |    **0.08** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | ManyDuplicates     |     3,748.6 ns |    351.03 ns |   183.60 ns |   1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 1024 | ManyDuplicates     |     6,324.4 ns |    300.51 ns |   157.17 ns |   1.69 |    0.09 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | ManyDuplicates     |     3,155.8 ns |    226.29 ns |   118.36 ns |   0.84 |    0.05 |    1 |         - |          NA |
| BucketSort          | 1024 | ManyDuplicates     |    10,345.0 ns |    317.31 ns |   165.96 ns |   2.77 |    0.13 |    2 |         - |          NA |
| BucketSortInteger   | 1024 | ManyDuplicates     |     5,460.8 ns |    293.85 ns |   153.69 ns |   1.46 |    0.08 |    2 |         - |          NA |
| FlashSort           | 1024 | ManyDuplicates     |    19,453.4 ns |    269.02 ns |   140.70 ns |   5.20 |    0.23 |    3 |         - |          NA |
| RadixLSD4Sort       | 1024 | ManyDuplicates     |     8,885.8 ns |    348.27 ns |   182.15 ns |   2.38 |    0.12 |    2 |         - |          NA |
| RotateLSDRadixSort  | 1024 | ManyDuplicates     |   247,284.5 ns |  2,022.44 ns | 1,057.77 ns |  66.10 |    2.95 |    5 |         - |          NA |
| RadixLSD256Sort     | 1024 | ManyDuplicates     |     4,186.1 ns |     25.71 ns |    11.42 ns |   1.12 |    0.05 |    1 |         - |          NA |
| Radsort             | 1024 | ManyDuplicates     |    25,173.4 ns |    289.12 ns |   151.22 ns |   6.73 |    0.30 |    4 |         - |          NA |
| RadixLSD10Sort      | 1024 | ManyDuplicates     |    11,333.2 ns |    295.23 ns |   154.41 ns |   3.03 |    0.14 |    2 |         - |          NA |
| RadixMSD4Sort       | 1024 | ManyDuplicates     |    10,074.5 ns |    281.80 ns |   147.38 ns |   2.69 |    0.13 |    2 |         - |          NA |
| RadixMSD10Sort      | 1024 | ManyDuplicates     |    12,457.6 ns |    300.40 ns |   133.38 ns |   3.33 |    0.15 |    2 |         - |          NA |
| AmericanFlagSort    | 1024 | ManyDuplicates     |     9,879.3 ns |    263.59 ns |   137.87 ns |   2.64 |    0.12 |    2 |         - |          NA |
| SkaSort             | 1024 | ManyDuplicates     |     8,214.0 ns |    336.15 ns |   175.81 ns |   2.20 |    0.11 |    2 |         - |          NA |
| SpreadSort          | 1024 | ManyDuplicates     |     7,019.4 ns |    317.97 ns |   166.30 ns |   1.88 |    0.09 |    2 |         - |          NA |
| DotnetSort          | 1024 | ManyDuplicates     |    30,156.5 ns |    394.08 ns |   206.11 ns |   8.06 |    0.36 |    4 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **4096** | **Random**             |    **24,403.8 ns** |    **223.35 ns** |   **116.81 ns** |   **1.42** |    **0.01** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Random             |    17,134.5 ns |    194.13 ns |   101.53 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | Random             |    19,992.2 ns |    639.23 ns |   334.33 ns |   1.17 |    0.02 |    2 |         - |          NA |
| PigeonSortInteger   | 4096 | Random             |    12,918.0 ns |    330.33 ns |   146.67 ns |   0.75 |    0.01 |    1 |         - |          NA |
| BucketSort          | 4096 | Random             |    27,968.5 ns |    490.50 ns |   217.79 ns |   1.63 |    0.01 |    2 |         - |          NA |
| BucketSortInteger   | 4096 | Random             |    22,916.7 ns |    278.14 ns |   145.47 ns |   1.34 |    0.01 |    2 |         - |          NA |
| FlashSort           | 4096 | Random             |    87,644.8 ns |  1,227.57 ns |   545.05 ns |   5.12 |    0.04 |    5 |         - |          NA |
| RadixLSD4Sort       | 4096 | Random             |    62,448.3 ns |    159.21 ns |    83.27 ns |   3.64 |    0.02 |    4 |         - |          NA |
| RotateLSDRadixSort  | 4096 | Random             | 1,819,621.3 ns |  4,602.55 ns | 2,043.56 ns | 106.20 |    0.60 |    8 |         - |          NA |
| RadixLSD256Sort     | 4096 | Random             |    25,617.8 ns |    858.36 ns |   381.12 ns |   1.50 |    0.02 |    2 |         - |          NA |
| Radsort             | 4096 | Random             |    88,346.4 ns |    657.71 ns |   292.03 ns |   5.16 |    0.03 |    5 |         - |          NA |
| RadixLSD10Sort      | 4096 | Random             |    82,701.4 ns |    549.53 ns |   244.00 ns |   4.83 |    0.03 |    5 |         - |          NA |
| RadixMSD4Sort       | 4096 | Random             |   127,798.2 ns |    822.72 ns |   365.29 ns |   7.46 |    0.05 |    6 |         - |          NA |
| RadixMSD10Sort      | 4096 | Random             |   114,537.3 ns |    651.38 ns |   289.21 ns |   6.68 |    0.04 |    6 |         - |          NA |
| AmericanFlagSort    | 4096 | Random             |    80,821.3 ns |    274.08 ns |   121.69 ns |   4.72 |    0.03 |    5 |         - |          NA |
| SkaSort             | 4096 | Random             |    60,298.8 ns |    456.93 ns |   202.88 ns |   3.52 |    0.02 |    4 |         - |          NA |
| SpreadSort          | 4096 | Random             |    36,496.9 ns |    236.26 ns |   123.57 ns |   2.13 |    0.01 |    3 |         - |          NA |
| DotnetSort          | 4096 | Random             |   186,993.9 ns |  1,083.38 ns |   566.63 ns |  10.91 |    0.07 |    7 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **4096** | **SingleElementMoved** |    **24,305.4 ns** |    **278.88 ns** |   **123.82 ns** |   **1.67** |    **0.07** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 4096 | SingleElementMoved |    14,569.6 ns |  1,169.60 ns |   611.72 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | SingleElementMoved |    21,090.7 ns |    243.36 ns |   127.28 ns |   1.45 |    0.06 |    3 |         - |          NA |
| PigeonSortInteger   | 4096 | SingleElementMoved |    10,138.0 ns |    560.40 ns |   248.82 ns |   0.70 |    0.03 |    1 |         - |          NA |
| BucketSort          | 4096 | SingleElementMoved |    26,907.8 ns |    352.79 ns |   156.64 ns |   1.85 |    0.07 |    3 |         - |          NA |
| BucketSortInteger   | 4096 | SingleElementMoved |    23,385.0 ns |    715.69 ns |   317.77 ns |   1.61 |    0.07 |    3 |         - |          NA |
| FlashSort           | 4096 | SingleElementMoved |    75,525.1 ns |    217.08 ns |    96.39 ns |   5.19 |    0.21 |    5 |         - |          NA |
| RadixLSD4Sort       | 4096 | SingleElementMoved |    85,853.4 ns |  1,662.47 ns |   869.51 ns |   5.90 |    0.24 |    5 |         - |          NA |
| RotateLSDRadixSort  | 4096 | SingleElementMoved | 1,106,738.8 ns |  3,672.57 ns | 1,920.82 ns |  76.08 |    3.01 |    7 |         - |          NA |
| RadixLSD256Sort     | 4096 | SingleElementMoved |    22,565.2 ns |  1,013.14 ns |   529.89 ns |   1.55 |    0.07 |    3 |         - |          NA |
| Radsort             | 4096 | SingleElementMoved |    56,131.8 ns |    331.64 ns |   147.25 ns |   3.86 |    0.15 |    4 |         - |          NA |
| RadixLSD10Sort      | 4096 | SingleElementMoved |    84,189.9 ns |    396.23 ns |   175.93 ns |   5.79 |    0.23 |    5 |         - |          NA |
| RadixMSD4Sort       | 4096 | SingleElementMoved |   130,439.0 ns |    588.95 ns |   261.50 ns |   8.97 |    0.36 |    6 |         - |          NA |
| RadixMSD10Sort      | 4096 | SingleElementMoved |   114,655.4 ns |    763.22 ns |   338.88 ns |   7.88 |    0.31 |    6 |         - |          NA |
| AmericanFlagSort    | 4096 | SingleElementMoved |    44,155.2 ns |    656.05 ns |   343.13 ns |   3.04 |    0.12 |    3 |         - |          NA |
| SkaSort             | 4096 | SingleElementMoved |    37,441.6 ns |    215.20 ns |    95.55 ns |   2.57 |    0.10 |    3 |         - |          NA |
| SpreadSort          | 4096 | SingleElementMoved |    23,191.3 ns |    395.70 ns |   206.96 ns |   1.59 |    0.06 |    3 |         - |          NA |
| DotnetSort          | 4096 | SingleElementMoved |    32,140.1 ns |  1,404.25 ns |   734.45 ns |   2.21 |    0.10 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **4096** | **Sorted**             |    **22,840.4 ns** |  **1,052.14 ns** |   **550.29 ns** |   **1.49** |    **0.03** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Sorted             |    15,304.5 ns |    123.89 ns |    64.80 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 4096 | Sorted             |    19,509.7 ns |    186.02 ns |    82.59 ns |   1.27 |    0.01 |    4 |         - |          NA |
| PigeonSortInteger   | 4096 | Sorted             |    10,955.0 ns |     69.23 ns |    30.74 ns |   0.72 |    0.00 |    2 |         - |          NA |
| BucketSort          | 4096 | Sorted             |    25,443.3 ns |    248.71 ns |   130.08 ns |   1.66 |    0.01 |    4 |         - |          NA |
| BucketSortInteger   | 4096 | Sorted             |    19,930.3 ns |    185.30 ns |    82.27 ns |   1.30 |    0.01 |    4 |         - |          NA |
| FlashSort           | 4096 | Sorted             |    71,207.9 ns |    451.30 ns |   200.38 ns |   4.65 |    0.02 |    7 |         - |          NA |
| RadixLSD4Sort       | 4096 | Sorted             |    92,988.2 ns |  1,197.28 ns |   626.20 ns |   6.08 |    0.05 |    7 |         - |          NA |
| RotateLSDRadixSort  | 4096 | Sorted             | 1,102,622.9 ns |  4,947.52 ns | 2,196.73 ns |  72.05 |    0.32 |    9 |         - |          NA |
| RadixLSD256Sort     | 4096 | Sorted             |    22,897.9 ns |    309.45 ns |   137.40 ns |   1.50 |    0.01 |    4 |         - |          NA |
| Radsort             | 4096 | Sorted             |    52,818.0 ns |    723.66 ns |   378.49 ns |   3.45 |    0.03 |    6 |         - |          NA |
| RadixLSD10Sort      | 4096 | Sorted             |    80,859.0 ns |    744.09 ns |   330.38 ns |   5.28 |    0.03 |    7 |         - |          NA |
| RadixMSD4Sort       | 4096 | Sorted             |   128,934.3 ns |    422.12 ns |   187.42 ns |   8.42 |    0.04 |    8 |         - |          NA |
| RadixMSD10Sort      | 4096 | Sorted             |   112,755.4 ns |    693.56 ns |   247.33 ns |   7.37 |    0.03 |    8 |         - |          NA |
| AmericanFlagSort    | 4096 | Sorted             |    37,861.4 ns |    775.77 ns |   405.74 ns |   2.47 |    0.03 |    5 |         - |          NA |
| SkaSort             | 4096 | Sorted             |    33,362.1 ns |    873.11 ns |   456.65 ns |   2.18 |    0.03 |    5 |         - |          NA |
| SpreadSort          | 4096 | Sorted             |     2,303.2 ns |     84.28 ns |    37.42 ns |   0.15 |    0.00 |    1 |         - |          NA |
| DotnetSort          | 4096 | Sorted             |    19,654.9 ns |    814.42 ns |   361.61 ns |   1.28 |    0.02 |    4 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **4096** | **Reversed**           |    **23,743.1 ns** |    **894.69 ns** |   **397.25 ns** |   **1.58** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Reversed           |    15,061.7 ns |     80.67 ns |    42.19 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | Reversed           |    19,633.4 ns |    229.86 ns |   102.06 ns |   1.30 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 4096 | Reversed           |     9,623.0 ns |     51.46 ns |    26.92 ns |   0.64 |    0.00 |    1 |         - |          NA |
| BucketSort          | 4096 | Reversed           |    26,131.8 ns |    949.43 ns |   496.57 ns |   1.73 |    0.03 |    3 |         - |          NA |
| BucketSortInteger   | 4096 | Reversed           |    22,116.9 ns |    579.08 ns |   257.11 ns |   1.47 |    0.02 |    3 |         - |          NA |
| FlashSort           | 4096 | Reversed           |    68,718.1 ns |  1,264.03 ns |   561.24 ns |   4.56 |    0.04 |    6 |         - |          NA |
| RadixLSD4Sort       | 4096 | Reversed           |    83,386.2 ns |  1,720.49 ns |   899.85 ns |   5.54 |    0.06 |    7 |         - |          NA |
| RotateLSDRadixSort  | 4096 | Reversed           | 1,120,833.1 ns |  2,056.45 ns | 1,075.56 ns |  74.42 |    0.21 |    9 |         - |          NA |
| RadixLSD256Sort     | 4096 | Reversed           |    21,717.8 ns |    464.56 ns |   242.97 ns |   1.44 |    0.02 |    3 |         - |          NA |
| Radsort             | 4096 | Reversed           |    53,282.8 ns |    248.92 ns |   110.52 ns |   3.54 |    0.01 |    5 |         - |          NA |
| RadixLSD10Sort      | 4096 | Reversed           |    82,416.4 ns |    280.82 ns |   124.69 ns |   5.47 |    0.02 |    7 |         - |          NA |
| RadixMSD4Sort       | 4096 | Reversed           |   133,495.2 ns |    480.44 ns |   213.32 ns |   8.86 |    0.03 |    8 |         - |          NA |
| RadixMSD10Sort      | 4096 | Reversed           |   114,679.4 ns |  1,268.39 ns |   563.17 ns |   7.61 |    0.04 |    8 |         - |          NA |
| AmericanFlagSort    | 4096 | Reversed           |    38,656.2 ns |    608.75 ns |   318.39 ns |   2.57 |    0.02 |    4 |         - |          NA |
| SkaSort             | 4096 | Reversed           |    37,832.9 ns |    458.04 ns |   239.57 ns |   2.51 |    0.02 |    4 |         - |          NA |
| SpreadSort          | 4096 | Reversed           |    19,304.9 ns |    252.77 ns |   132.20 ns |   1.28 |    0.01 |    3 |         - |          NA |
| DotnetSort          | 4096 | Reversed           |    35,264.1 ns |  2,798.94 ns | 1,463.90 ns |   2.34 |    0.09 |    4 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **4096** | **PipeOrgan**          |    **23,115.1 ns** |    **294.79 ns** |   **154.18 ns** |   **1.27** |    **0.01** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 4096 | PipeOrgan          |    18,262.0 ns |    311.01 ns |   162.67 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | PipeOrgan          |    19,760.9 ns |    294.48 ns |   154.02 ns |   1.08 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 4096 | PipeOrgan          |    12,744.6 ns |    394.12 ns |   206.13 ns |   0.70 |    0.01 |    1 |         - |          NA |
| BucketSort          | 4096 | PipeOrgan          |    26,516.2 ns |    682.58 ns |   357.00 ns |   1.45 |    0.02 |    2 |         - |          NA |
| BucketSortInteger   | 4096 | PipeOrgan          |    21,954.9 ns |    577.72 ns |   302.16 ns |   1.20 |    0.02 |    2 |         - |          NA |
| FlashSort           | 4096 | PipeOrgan          |    74,632.3 ns |    484.79 ns |   215.25 ns |   4.09 |    0.04 |    3 |         - |          NA |
| RadixLSD4Sort       | 4096 | PipeOrgan          |    78,148.9 ns |    514.01 ns |   228.22 ns |   4.28 |    0.04 |    3 |         - |          NA |
| RotateLSDRadixSort  | 4096 | PipeOrgan          | 1,108,969.7 ns |  1,162.88 ns |   516.33 ns |  60.73 |    0.51 |    5 |         - |          NA |
| RadixLSD256Sort     | 4096 | PipeOrgan          |    25,089.6 ns |    139.46 ns |    61.92 ns |   1.37 |    0.01 |    2 |         - |          NA |
| Radsort             | 4096 | PipeOrgan          |    53,240.3 ns |  1,023.16 ns |   535.13 ns |   2.92 |    0.04 |    3 |         - |          NA |
| RadixLSD10Sort      | 4096 | PipeOrgan          |    81,953.6 ns |    366.90 ns |   191.90 ns |   4.49 |    0.04 |    3 |         - |          NA |
| RadixMSD4Sort       | 4096 | PipeOrgan          |   135,265.1 ns |    513.45 ns |   227.97 ns |   7.41 |    0.06 |    4 |         - |          NA |
| RadixMSD10Sort      | 4096 | PipeOrgan          |   115,759.0 ns |    795.75 ns |   283.77 ns |   6.34 |    0.06 |    4 |         - |          NA |
| AmericanFlagSort    | 4096 | PipeOrgan          |    63,563.3 ns |    333.95 ns |   174.66 ns |   3.48 |    0.03 |    3 |         - |          NA |
| SkaSort             | 4096 | PipeOrgan          |    54,659.3 ns |    248.33 ns |   110.26 ns |   2.99 |    0.03 |    3 |         - |          NA |
| SpreadSort          | 4096 | PipeOrgan          |    27,043.8 ns |    219.43 ns |    97.43 ns |   1.48 |    0.01 |    2 |         - |          NA |
| DotnetSort          | 4096 | PipeOrgan          |    93,505.8 ns |  5,616.82 ns | 2,937.71 ns |   5.12 |    0.16 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **4096** | **ManyDuplicates**     |    **22,014.1 ns** |    **312.65 ns** |   **138.82 ns** |   **1.54** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 4096 | ManyDuplicates     |    14,302.7 ns |    167.92 ns |    87.82 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | ManyDuplicates     |    25,580.3 ns |    217.15 ns |   113.57 ns |   1.79 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 4096 | ManyDuplicates     |    11,339.4 ns |    321.05 ns |   167.92 ns |   0.79 |    0.01 |    1 |         - |          NA |
| BucketSort          | 4096 | ManyDuplicates     |    41,739.5 ns |    552.02 ns |   245.10 ns |   2.92 |    0.02 |    4 |         - |          NA |
| BucketSortInteger   | 4096 | ManyDuplicates     |    20,913.9 ns |    369.60 ns |   193.31 ns |   1.46 |    0.02 |    3 |         - |          NA |
| FlashSort           | 4096 | ManyDuplicates     |    76,571.8 ns |    788.30 ns |   350.01 ns |   5.35 |    0.04 |    5 |         - |          NA |
| RadixLSD4Sort       | 4096 | ManyDuplicates     |    35,594.1 ns |    464.03 ns |   242.70 ns |   2.49 |    0.02 |    4 |         - |          NA |
| RotateLSDRadixSort  | 4096 | ManyDuplicates     | 1,001,466.2 ns |  4,395.98 ns | 1,951.84 ns |  70.02 |    0.42 |    7 |         - |          NA |
| RadixLSD256Sort     | 4096 | ManyDuplicates     |    16,275.5 ns |    178.39 ns |    93.30 ns |   1.14 |    0.01 |    2 |         - |          NA |
| Radsort             | 4096 | ManyDuplicates     |    52,923.9 ns |    418.79 ns |   219.03 ns |   3.70 |    0.03 |    4 |         - |          NA |
| RadixLSD10Sort      | 4096 | ManyDuplicates     |    44,651.4 ns |    311.73 ns |   138.41 ns |   3.12 |    0.02 |    4 |         - |          NA |
| RadixMSD4Sort       | 4096 | ManyDuplicates     |    36,199.4 ns |    254.36 ns |   133.04 ns |   2.53 |    0.02 |    4 |         - |          NA |
| RadixMSD10Sort      | 4096 | ManyDuplicates     |    47,908.0 ns |    275.48 ns |   122.31 ns |   3.35 |    0.02 |    4 |         - |          NA |
| AmericanFlagSort    | 4096 | ManyDuplicates     |    35,655.1 ns |    416.58 ns |   184.96 ns |   2.49 |    0.02 |    4 |         - |          NA |
| SkaSort             | 4096 | ManyDuplicates     |    23,402.5 ns |    229.85 ns |   102.05 ns |   1.64 |    0.01 |    3 |         - |          NA |
| SpreadSort          | 4096 | ManyDuplicates     |    27,334.2 ns |    308.04 ns |   161.11 ns |   1.91 |    0.02 |    3 |         - |          NA |
| DotnetSort          | 4096 | ManyDuplicates     |   121,657.3 ns |    778.48 ns |   345.65 ns |   8.51 |    0.05 |    6 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **8192** | **Random**             |    **50,811.2 ns** |    **221.53 ns** |    **98.36 ns** |   **1.42** |    **0.00** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Random             |    35,658.6 ns |    229.49 ns |   101.90 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | Random             |    39,592.4 ns |    388.56 ns |   172.52 ns |   1.11 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | Random             |    25,673.4 ns |    368.64 ns |   192.81 ns |   0.72 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | Random             |    57,696.9 ns |    973.19 ns |   347.05 ns |   1.62 |    0.01 |    2 |         - |          NA |
| BucketSortInteger   | 8192 | Random             |    47,074.6 ns |    196.84 ns |    87.40 ns |   1.32 |    0.00 |    2 |         - |          NA |
| FlashSort           | 8192 | Random             |   185,845.1 ns |    459.83 ns |   163.98 ns |   5.21 |    0.01 |    3 |         - |          NA |
| RadixLSD4Sort       | 8192 | Random             |   146,502.0 ns |    324.89 ns |   115.86 ns |   4.11 |    0.01 |    3 |         - |          NA |
| RotateLSDRadixSort  | 8192 | Random             | 4,408,094.1 ns | 14,724.01 ns | 5,250.72 ns | 123.62 |    0.36 |    7 |         - |          NA |
| RadixLSD256Sort     | 8192 | Random             |    50,644.4 ns |    387.04 ns |   202.43 ns |   1.42 |    0.01 |    2 |         - |          NA |
| Radsort             | 8192 | Random             |   122,716.0 ns |  1,039.31 ns |   461.46 ns |   3.44 |    0.02 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | Random             |   165,247.6 ns |  1,265.12 ns |   661.68 ns |   4.63 |    0.02 |    3 |         - |          NA |
| RadixMSD4Sort       | 8192 | Random             |   275,543.7 ns |    875.99 ns |   458.16 ns |   7.73 |    0.02 |    5 |         - |          NA |
| RadixMSD10Sort      | 8192 | Random             |   228,177.1 ns |    385.13 ns |   171.00 ns |   6.40 |    0.02 |    4 |         - |          NA |
| AmericanFlagSort    | 8192 | Random             |   160,102.5 ns |    614.27 ns |   272.74 ns |   4.49 |    0.01 |    3 |         - |          NA |
| SkaSort             | 8192 | Random             |   119,526.3 ns |    331.11 ns |   147.01 ns |   3.35 |    0.01 |    3 |         - |          NA |
| SpreadSort          | 8192 | Random             |   127,189.2 ns |    447.15 ns |   233.87 ns |   3.57 |    0.01 |    3 |         - |          NA |
| DotnetSort          | 8192 | Random             |   406,828.9 ns |  2,525.07 ns | 1,320.66 ns |  11.41 |    0.05 |    6 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **8192** | **SingleElementMoved** |    **46,018.5 ns** |    **815.08 ns** |   **361.90 ns** |   **1.53** |    **0.04** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | SingleElementMoved |    30,064.1 ns |  1,483.09 ns |   775.68 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | SingleElementMoved |    41,641.4 ns |    602.07 ns |   267.32 ns |   1.39 |    0.04 |    3 |         - |          NA |
| PigeonSortInteger   | 8192 | SingleElementMoved |    22,146.0 ns |  1,267.29 ns |   662.82 ns |   0.74 |    0.03 |    1 |         - |          NA |
| BucketSort          | 8192 | SingleElementMoved |    53,194.1 ns |    331.31 ns |   147.10 ns |   1.77 |    0.05 |    3 |         - |          NA |
| BucketSortInteger   | 8192 | SingleElementMoved |    45,310.3 ns |  1,250.67 ns |   654.13 ns |   1.51 |    0.04 |    3 |         - |          NA |
| FlashSort           | 8192 | SingleElementMoved |   150,010.5 ns |    629.51 ns |   279.51 ns |   4.99 |    0.13 |    5 |         - |          NA |
| RadixLSD4Sort       | 8192 | SingleElementMoved |   192,505.3 ns |  2,992.98 ns | 1,565.39 ns |   6.41 |    0.17 |    5 |         - |          NA |
| RotateLSDRadixSort  | 8192 | SingleElementMoved | 2,807,819.8 ns |  5,090.27 ns | 2,662.31 ns |  93.45 |    2.40 |    7 |         - |          NA |
| RadixLSD256Sort     | 8192 | SingleElementMoved |    47,017.5 ns |  1,364.72 ns |   713.78 ns |   1.56 |    0.05 |    3 |         - |          NA |
| Radsort             | 8192 | SingleElementMoved |    76,705.9 ns |    528.42 ns |   234.62 ns |   2.55 |    0.07 |    4 |         - |          NA |
| RadixLSD10Sort      | 8192 | SingleElementMoved |   168,530.4 ns |  1,185.77 ns |   526.49 ns |   5.61 |    0.14 |    5 |         - |          NA |
| RadixMSD4Sort       | 8192 | SingleElementMoved |   277,717.5 ns |    905.16 ns |   401.90 ns |   9.24 |    0.24 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | SingleElementMoved |   230,322.5 ns |  6,134.35 ns | 2,723.69 ns |   7.67 |    0.21 |    5 |         - |          NA |
| AmericanFlagSort    | 8192 | SingleElementMoved |    87,531.2 ns |  1,291.09 ns |   675.26 ns |   2.91 |    0.08 |    4 |         - |          NA |
| SkaSort             | 8192 | SingleElementMoved |    73,273.1 ns |    442.45 ns |   196.45 ns |   2.44 |    0.06 |    4 |         - |          NA |
| SpreadSort          | 8192 | SingleElementMoved |    50,822.6 ns |    248.63 ns |   110.39 ns |   1.69 |    0.04 |    3 |         - |          NA |
| DotnetSort          | 8192 | SingleElementMoved |    68,804.4 ns |  2,536.76 ns | 1,126.34 ns |   2.29 |    0.07 |    4 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **8192** | **Sorted**             |    **44,863.8 ns** |    **379.90 ns** |   **135.47 ns** |   **1.49** |    **0.02** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Sorted             |    30,181.7 ns |    709.51 ns |   315.03 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 8192 | Sorted             |    41,842.6 ns |    770.20 ns |   274.66 ns |   1.39 |    0.02 |    4 |         - |          NA |
| PigeonSortInteger   | 8192 | Sorted             |    22,336.3 ns |    557.53 ns |   291.60 ns |   0.74 |    0.01 |    2 |         - |          NA |
| BucketSort          | 8192 | Sorted             |    54,118.1 ns |    908.45 ns |   475.14 ns |   1.79 |    0.02 |    4 |         - |          NA |
| BucketSortInteger   | 8192 | Sorted             |    39,707.3 ns |    576.49 ns |   255.96 ns |   1.32 |    0.02 |    4 |         - |          NA |
| FlashSort           | 8192 | Sorted             |   140,937.2 ns |  1,260.31 ns |   559.59 ns |   4.67 |    0.05 |    6 |         - |          NA |
| RadixLSD4Sort       | 8192 | Sorted             |   212,667.9 ns |  4,887.55 ns | 2,556.28 ns |   7.05 |    0.11 |    7 |         - |          NA |
| RotateLSDRadixSort  | 8192 | Sorted             | 2,801,454.7 ns | 12,396.33 ns | 5,504.05 ns |  92.83 |    0.92 |    9 |         - |          NA |
| RadixLSD256Sort     | 8192 | Sorted             |    49,223.0 ns |    328.64 ns |   171.89 ns |   1.63 |    0.02 |    4 |         - |          NA |
| Radsort             | 8192 | Sorted             |    70,979.0 ns |    428.79 ns |   190.39 ns |   2.35 |    0.02 |    5 |         - |          NA |
| RadixLSD10Sort      | 8192 | Sorted             |   161,820.1 ns |    679.63 ns |   301.76 ns |   5.36 |    0.05 |    6 |         - |          NA |
| RadixMSD4Sort       | 8192 | Sorted             |   278,136.6 ns |    493.51 ns |   219.12 ns |   9.22 |    0.09 |    8 |         - |          NA |
| RadixMSD10Sort      | 8192 | Sorted             |   228,051.3 ns |    982.69 ns |   436.32 ns |   7.56 |    0.07 |    7 |         - |          NA |
| AmericanFlagSort    | 8192 | Sorted             |    74,922.3 ns |    738.53 ns |   386.27 ns |   2.48 |    0.03 |    5 |         - |          NA |
| SkaSort             | 8192 | Sorted             |    65,334.3 ns |    478.90 ns |   212.63 ns |   2.16 |    0.02 |    5 |         - |          NA |
| SpreadSort          | 8192 | Sorted             |     4,552.7 ns |    250.55 ns |   111.25 ns |   0.15 |    0.00 |    1 |         - |          NA |
| DotnetSort          | 8192 | Sorted             |    45,057.0 ns |  5,368.60 ns | 2,807.88 ns |   1.49 |    0.09 |    4 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **8192** | **Reversed**           |    **46,995.1 ns** |  **1,210.32 ns** |   **633.02 ns** |   **1.71** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Reversed           |    27,554.1 ns |    266.78 ns |   139.53 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | Reversed           |    38,932.5 ns |    333.14 ns |   147.92 ns |   1.41 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 8192 | Reversed           |    19,410.2 ns |    274.65 ns |   121.95 ns |   0.70 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | Reversed           |    51,169.6 ns |    457.82 ns |   163.26 ns |   1.86 |    0.01 |    3 |         - |          NA |
| BucketSortInteger   | 8192 | Reversed           |    43,464.1 ns |    369.41 ns |   193.21 ns |   1.58 |    0.01 |    3 |         - |          NA |
| FlashSort           | 8192 | Reversed           |   135,796.8 ns |  2,113.48 ns |   753.69 ns |   4.93 |    0.03 |    5 |         - |          NA |
| RadixLSD4Sort       | 8192 | Reversed           |   213,079.4 ns |  2,048.91 ns |   909.73 ns |   7.73 |    0.05 |    7 |         - |          NA |
| RotateLSDRadixSort  | 8192 | Reversed           | 2,843,147.3 ns |  5,379.15 ns | 1,918.26 ns | 103.19 |    0.50 |    9 |         - |          NA |
| RadixLSD256Sort     | 8192 | Reversed           |    45,176.3 ns |    240.85 ns |   125.97 ns |   1.64 |    0.01 |    3 |         - |          NA |
| Radsort             | 8192 | Reversed           |    73,268.4 ns |  1,072.02 ns |   475.98 ns |   2.66 |    0.02 |    4 |         - |          NA |
| RadixLSD10Sort      | 8192 | Reversed           |   165,191.3 ns |    329.77 ns |   146.42 ns |   6.00 |    0.03 |    6 |         - |          NA |
| RadixMSD4Sort       | 8192 | Reversed           |   284,385.2 ns |    855.54 ns |   447.46 ns |  10.32 |    0.05 |    8 |         - |          NA |
| RadixMSD10Sort      | 8192 | Reversed           |   229,795.0 ns |  1,213.64 ns |   538.86 ns |   8.34 |    0.04 |    7 |         - |          NA |
| AmericanFlagSort    | 8192 | Reversed           |    77,046.6 ns |  1,168.35 ns |   611.07 ns |   2.80 |    0.02 |    4 |         - |          NA |
| SkaSort             | 8192 | Reversed           |    74,778.0 ns |    512.92 ns |   227.74 ns |   2.71 |    0.02 |    4 |         - |          NA |
| SpreadSort          | 8192 | Reversed           |    72,181.7 ns |    664.68 ns |   295.12 ns |   2.62 |    0.02 |    4 |         - |          NA |
| DotnetSort          | 8192 | Reversed           |    75,265.1 ns |  4,896.15 ns | 2,173.92 ns |   2.73 |    0.07 |    4 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **8192** | **PipeOrgan**          |    **46,610.5 ns** |    **260.31 ns** |   **136.15 ns** |   **1.46** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | PipeOrgan          |    32,019.8 ns |    290.09 ns |   128.80 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | PipeOrgan          |    39,191.6 ns |    310.07 ns |   137.67 ns |   1.22 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 8192 | PipeOrgan          |    25,342.6 ns |    291.30 ns |   152.36 ns |   0.79 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | PipeOrgan          |    52,884.4 ns |  1,150.75 ns |   510.94 ns |   1.65 |    0.02 |    3 |         - |          NA |
| BucketSortInteger   | 8192 | PipeOrgan          |    44,649.4 ns |    277.82 ns |   145.30 ns |   1.39 |    0.01 |    3 |         - |          NA |
| FlashSort           | 8192 | PipeOrgan          |   161,438.5 ns |  2,040.35 ns | 1,067.14 ns |   5.04 |    0.04 |    7 |         - |          NA |
| RadixLSD4Sort       | 8192 | PipeOrgan          |   171,506.0 ns |  1,204.62 ns |   630.04 ns |   5.36 |    0.03 |    7 |         - |          NA |
| RotateLSDRadixSort  | 8192 | PipeOrgan          | 2,816,204.0 ns |  3,715.97 ns | 1,649.91 ns |  87.95 |    0.33 |   11 |         - |          NA |
| RadixLSD256Sort     | 8192 | PipeOrgan          |    52,245.2 ns |    226.58 ns |   100.60 ns |   1.63 |    0.01 |    3 |         - |          NA |
| Radsort             | 8192 | PipeOrgan          |    73,260.3 ns |  1,269.60 ns |   664.03 ns |   2.29 |    0.02 |    4 |         - |          NA |
| RadixLSD10Sort      | 8192 | PipeOrgan          |   164,190.2 ns |  1,498.68 ns |   665.42 ns |   5.13 |    0.03 |    7 |         - |          NA |
| RadixMSD4Sort       | 8192 | PipeOrgan          |   299,302.2 ns |  2,049.44 ns | 1,071.90 ns |   9.35 |    0.05 |    9 |         - |          NA |
| RadixMSD10Sort      | 8192 | PipeOrgan          |   230,224.5 ns |  1,134.90 ns |   503.90 ns |   7.19 |    0.03 |    8 |         - |          NA |
| AmericanFlagSort    | 8192 | PipeOrgan          |   128,718.0 ns |    194.43 ns |    86.33 ns |   4.02 |    0.02 |    6 |         - |          NA |
| SkaSort             | 8192 | PipeOrgan          |   111,687.1 ns |    802.14 ns |   356.16 ns |   3.49 |    0.02 |    6 |         - |          NA |
| SpreadSort          | 8192 | PipeOrgan          |    88,292.7 ns |    467.63 ns |   207.63 ns |   2.76 |    0.01 |    5 |         - |          NA |
| DotnetSort          | 8192 | PipeOrgan          |   365,721.3 ns |  8,434.49 ns | 4,411.40 ns |  11.42 |    0.14 |   10 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **8192** | **ManyDuplicates**     |    **43,989.1 ns** |    **328.59 ns** |   **171.86 ns** |   **1.54** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | ManyDuplicates     |    28,542.6 ns |    259.52 ns |   135.74 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | ManyDuplicates     |    70,079.2 ns |    759.81 ns |   337.36 ns |   2.46 |    0.02 |    5 |         - |          NA |
| PigeonSortInteger   | 8192 | ManyDuplicates     |    22,306.1 ns |    342.76 ns |   152.19 ns |   0.78 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | ManyDuplicates     |    80,398.8 ns |    473.30 ns |   210.15 ns |   2.82 |    0.01 |    5 |         - |          NA |
| BucketSortInteger   | 8192 | ManyDuplicates     |    41,147.2 ns |    261.73 ns |   136.89 ns |   1.44 |    0.01 |    3 |         - |          NA |
| FlashSort           | 8192 | ManyDuplicates     |   152,719.8 ns |    648.81 ns |   288.07 ns |   5.35 |    0.03 |    6 |         - |          NA |
| RadixLSD4Sort       | 8192 | ManyDuplicates     |    70,498.1 ns |    932.13 ns |   413.87 ns |   2.47 |    0.02 |    5 |         - |          NA |
| RotateLSDRadixSort  | 8192 | ManyDuplicates     | 2,111,734.6 ns |  7,469.57 ns | 3,316.54 ns |  73.99 |    0.35 |    8 |         - |          NA |
| RadixLSD256Sort     | 8192 | ManyDuplicates     |    32,245.2 ns |    236.48 ns |   123.68 ns |   1.13 |    0.01 |    2 |         - |          NA |
| Radsort             | 8192 | ManyDuplicates     |    65,497.3 ns |    643.32 ns |   285.64 ns |   2.29 |    0.01 |    5 |         - |          NA |
| RadixLSD10Sort      | 8192 | ManyDuplicates     |    88,935.4 ns |  1,217.77 ns |   540.70 ns |   3.12 |    0.02 |    5 |         - |          NA |
| RadixMSD4Sort       | 8192 | ManyDuplicates     |    71,756.4 ns |    331.36 ns |   147.12 ns |   2.51 |    0.01 |    5 |         - |          NA |
| RadixMSD10Sort      | 8192 | ManyDuplicates     |    93,197.0 ns |    321.72 ns |   142.85 ns |   3.27 |    0.02 |    5 |         - |          NA |
| AmericanFlagSort    | 8192 | ManyDuplicates     |    70,636.2 ns |    743.74 ns |   330.22 ns |   2.47 |    0.02 |    5 |         - |          NA |
| SkaSort             | 8192 | ManyDuplicates     |    43,433.7 ns |    218.71 ns |    97.11 ns |   1.52 |    0.01 |    3 |         - |          NA |
| SpreadSort          | 8192 | ManyDuplicates     |    54,340.5 ns |    510.71 ns |   267.11 ns |   1.90 |    0.01 |    4 |         - |          NA |
| DotnetSort          | 8192 | ManyDuplicates     |   246,805.5 ns |  3,054.81 ns | 1,597.73 ns |   8.65 |    0.07 |    7 |         - |          NA |

### ExchangeBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.86GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.22 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method             | Size | Pattern            | Mean         | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------- |----- |------------------- |-------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **BubbleSort**         | **256**  | **Random**             |  **53,872.6 ns** |    **250.26 ns** |    **111.12 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Random             |  39,903.2 ns |    613.26 ns |    320.75 ns |  0.74 |    0.01 |    4 |         - |          NA |
| OddEvenSort        | 256  | Random             |  57,668.2 ns |    522.32 ns |    273.19 ns |  1.07 |    0.01 |    5 |         - |          NA |
| CombSort           | 256  | Random             |  10,082.7 ns |    351.57 ns |    183.88 ns |  0.19 |    0.00 |    2 |         - |          NA |
| CircleSort         | 256  | Random             |  29,518.1 ns |    445.18 ns |    197.66 ns |  0.55 |    0.00 |    3 |         - |          NA |
| DotnetSort         | 256  | Random             |   7,571.2 ns |    467.61 ns |    244.57 ns |  0.14 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BubbleSort**         | **256**  | **SingleElementMoved** |   **3,960.9 ns** |     **21.65 ns** |      **7.72 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | SingleElementMoved |     381.5 ns |     14.18 ns |      6.29 ns |  0.10 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 256  | SingleElementMoved |   8,973.9 ns |    273.23 ns |    142.90 ns |  2.27 |    0.03 |    4 |         - |          NA |
| CombSort           | 256  | SingleElementMoved |   3,454.0 ns |    152.42 ns |     67.67 ns |  0.87 |    0.02 |    3 |         - |          NA |
| CircleSort         | 256  | SingleElementMoved |  13,618.6 ns |    133.42 ns |     69.78 ns |  3.44 |    0.02 |    5 |         - |          NA |
| DotnetSort         | 256  | SingleElementMoved |   1,412.2 ns |    153.99 ns |     80.54 ns |  0.36 |    0.02 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BubbleSort**         | **256**  | **Sorted**             |     **280.1 ns** |      **4.89 ns** |      **2.17 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Sorted             |     201.4 ns |      3.60 ns |      1.60 ns |  0.72 |    0.01 |    1 |         - |          NA |
| OddEvenSort        | 256  | Sorted             |     260.7 ns |     13.11 ns |      5.82 ns |  0.93 |    0.02 |    2 |         - |          NA |
| CombSort           | 256  | Sorted             |   2,949.7 ns |    338.57 ns |    177.08 ns | 10.53 |    0.60 |    5 |         - |          NA |
| CircleSort         | 256  | Sorted             |   2,181.2 ns |     17.05 ns |      6.08 ns |  7.79 |    0.06 |    4 |         - |          NA |
| DotnetSort         | 256  | Sorted             |     809.9 ns |     12.45 ns |      4.44 ns |  2.89 |    0.03 |    3 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BubbleSort**         | **256**  | **Reversed**           |  **29,475.6 ns** |    **116.23 ns** |     **51.61 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Reversed           |  27,703.5 ns |    190.98 ns |     84.80 ns |  0.94 |    0.00 |    4 |         - |          NA |
| OddEvenSort        | 256  | Reversed           |  25,329.5 ns |    180.81 ns |     80.28 ns |  0.86 |    0.00 |    4 |         - |          NA |
| CombSort           | 256  | Reversed           |   3,354.8 ns |    553.20 ns |    289.33 ns |  0.11 |    0.01 |    2 |         - |          NA |
| CircleSort         | 256  | Reversed           |   4,340.7 ns |     45.07 ns |     20.01 ns |  0.15 |    0.00 |    3 |         - |          NA |
| DotnetSort         | 256  | Reversed           |   1,426.4 ns |    360.30 ns |    188.45 ns |  0.05 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BubbleSort**         | **256**  | **PipeOrgan**          |  **34,678.7 ns** |    **191.49 ns** |     **85.02 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| CocktailShakerSort | 256  | PipeOrgan          |  17,708.5 ns |    183.81 ns |     96.13 ns |  0.51 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 256  | PipeOrgan          |  28,271.1 ns |    523.00 ns |    273.54 ns |  0.82 |    0.01 |    4 |         - |          NA |
| CombSort           | 256  | PipeOrgan          |   3,065.3 ns |     53.16 ns |     23.60 ns |  0.09 |    0.00 |    2 |         - |          NA |
| CircleSort         | 256  | PipeOrgan          |  19,456.5 ns |    245.31 ns |    128.30 ns |  0.56 |    0.00 |    3 |         - |          NA |
| DotnetSort         | 256  | PipeOrgan          |   2,440.7 ns |    492.17 ns |    257.42 ns |  0.07 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BubbleSort**         | **256**  | **ManyDuplicates**     |  **53,079.2 ns** |    **225.91 ns** |    **118.16 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 256  | ManyDuplicates     |  36,979.9 ns |    342.65 ns |    179.21 ns |  0.70 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 256  | ManyDuplicates     |  54,034.8 ns |  1,512.40 ns |    671.52 ns |  1.02 |    0.01 |    4 |         - |          NA |
| CombSort           | 256  | ManyDuplicates     |   7,544.1 ns |    313.38 ns |    163.90 ns |  0.14 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | ManyDuplicates     |  25,872.3 ns |    315.38 ns |    164.95 ns |  0.49 |    0.00 |    2 |         - |          NA |
| DotnetSort         | 256  | ManyDuplicates     |   7,128.2 ns |    311.83 ns |    163.10 ns |  0.13 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BubbleSort**         | **1024** | **Random**             | **683,560.3 ns** |  **1,484.45 ns** |    **659.10 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Random             | 416,921.8 ns |  2,483.87 ns |  1,102.85 ns |  0.61 |    0.00 |    4 |         - |          NA |
| OddEvenSort        | 1024 | Random             | 646,893.0 ns |  1,007.53 ns |    526.96 ns |  0.95 |    0.00 |    5 |         - |          NA |
| CombSort           | 1024 | Random             |  53,441.5 ns |    280.21 ns |    146.55 ns |  0.08 |    0.00 |    2 |         - |          NA |
| CircleSort         | 1024 | Random             | 164,661.7 ns |  1,132.99 ns |    592.57 ns |  0.24 |    0.00 |    3 |         - |          NA |
| DotnetSort         | 1024 | Random             |  38,585.8 ns |    717.25 ns |    375.14 ns |  0.06 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BubbleSort**         | **1024** | **SingleElementMoved** |  **57,786.1 ns** |    **301.79 ns** |    **134.00 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | SingleElementMoved |   1,605.8 ns |    341.16 ns |    178.43 ns |  0.03 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 1024 | SingleElementMoved | 126,981.1 ns |    964.57 ns |    428.28 ns |  2.20 |    0.01 |    6 |         - |          NA |
| CombSort           | 1024 | SingleElementMoved |  16,398.3 ns |    220.64 ns |    115.40 ns |  0.28 |    0.00 |    3 |         - |          NA |
| CircleSort         | 1024 | SingleElementMoved |  76,869.1 ns |    267.58 ns |    118.81 ns |  1.33 |    0.00 |    5 |         - |          NA |
| DotnetSort         | 1024 | SingleElementMoved |   6,901.3 ns |    460.29 ns |    240.74 ns |  0.12 |    0.00 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BubbleSort**         | **1024** | **Sorted**             |   **1,086.6 ns** |      **9.31 ns** |      **4.13 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Sorted             |     764.4 ns |     11.52 ns |      6.02 ns |  0.70 |    0.01 |    1 |         - |          NA |
| OddEvenSort        | 1024 | Sorted             |     963.8 ns |     11.28 ns |      5.01 ns |  0.89 |    0.01 |    2 |         - |          NA |
| CombSort           | 1024 | Sorted             |  14,583.1 ns |    122.71 ns |     54.48 ns | 13.42 |    0.07 |    5 |         - |          NA |
| CircleSort         | 1024 | Sorted             |   9,602.0 ns |    355.89 ns |    186.14 ns |  8.84 |    0.16 |    4 |         - |          NA |
| DotnetSort         | 1024 | Sorted             |   4,032.8 ns |     53.54 ns |     19.09 ns |  3.71 |    0.02 |    3 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BubbleSort**         | **1024** | **Reversed**           | **438,975.3 ns** |  **2,682.58 ns** |  **1,403.04 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Reversed           | 440,122.9 ns | 11,271.55 ns |  4,019.54 ns |  1.00 |    0.01 |    3 |         - |          NA |
| OddEvenSort        | 1024 | Reversed           | 407,405.1 ns |  3,299.86 ns |  1,465.16 ns |  0.93 |    0.00 |    3 |         - |          NA |
| CombSort           | 1024 | Reversed           |  16,560.7 ns |    340.06 ns |    150.99 ns |  0.04 |    0.00 |    2 |         - |          NA |
| CircleSort         | 1024 | Reversed           |  19,482.4 ns |    320.98 ns |    142.52 ns |  0.04 |    0.00 |    2 |         - |          NA |
| DotnetSort         | 1024 | Reversed           |   7,617.3 ns |  1,129.26 ns |    590.63 ns |  0.02 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BubbleSort**         | **1024** | **PipeOrgan**          | **510,386.6 ns** |  **4,533.21 ns** |  **2,370.96 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | PipeOrgan          | 284,608.6 ns |  1,856.17 ns |    824.15 ns |  0.56 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | PipeOrgan          | 458,381.6 ns | 38,668.42 ns | 17,169.02 ns |  0.90 |    0.03 |    4 |         - |          NA |
| CombSort           | 1024 | PipeOrgan          |  16,274.8 ns |    152.30 ns |     79.66 ns |  0.03 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | PipeOrgan          | 104,685.8 ns |  1,052.88 ns |    467.48 ns |  0.21 |    0.00 |    2 |         - |          NA |
| DotnetSort         | 1024 | PipeOrgan          |  16,478.6 ns |  1,204.16 ns |    629.80 ns |  0.03 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BubbleSort**         | **1024** | **ManyDuplicates**     | **680,952.7 ns** |  **5,021.67 ns** |  **2,626.43 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | ManyDuplicates     | 420,554.8 ns |  1,943.89 ns |  1,016.69 ns |  0.62 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | ManyDuplicates     | 614,019.7 ns |  1,111.35 ns |    493.45 ns |  0.90 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | ManyDuplicates     |  34,420.4 ns |    422.94 ns |    221.20 ns |  0.05 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | ManyDuplicates     | 127,645.7 ns |    339.80 ns |    121.18 ns |  0.19 |    0.00 |    2 |         - |          NA |
| DotnetSort         | 1024 | ManyDuplicates     |  29,984.0 ns |    296.33 ns |    131.57 ns |  0.04 |    0.00 |    1 |         - |          NA |

### HeapBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.86GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.22 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method           | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **HeapSort**         | **256**  | **Random**             |     **8,756.5 ns** |    **407.61 ns** |    **180.98 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | Random             |     8,901.8 ns |    283.92 ns |    126.06 ns |  1.02 |    0.02 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | Random             |    13,834.8 ns |    263.15 ns |    137.63 ns |  1.58 |    0.03 |    2 |         - |          NA |
| BottomupHeapSort | 256  | Random             |     9,312.3 ns |    338.06 ns |    176.81 ns |  1.06 |    0.03 |    1 |         - |          NA |
| WeakHeapSort     | 256  | Random             |    16,294.8 ns |    197.94 ns |     87.88 ns |  1.86 |    0.04 |    2 |         - |          NA |
| SmoothSort       | 256  | Random             |    17,279.7 ns |     90.82 ns |     40.33 ns |  1.97 |    0.04 |    2 |         - |          NA |
| PoplarHeapSort   | 256  | Random             |    17,630.6 ns |    551.08 ns |    288.22 ns |  2.01 |    0.05 |    2 |         - |          NA |
| TournamentSort   | 256  | Random             |    17,274.4 ns |    451.32 ns |    200.39 ns |  1.97 |    0.04 |    2 |         - |          NA |
| BinomialHeapSort | 256  | Random             |    35,940.7 ns |    734.73 ns |    384.28 ns |  4.11 |    0.09 |    3 |         - |          NA |
| PairingHeapSort  | 256  | Random             |    21,040.8 ns |    286.28 ns |    127.11 ns |  2.40 |    0.05 |    2 |         - |          NA |
| DotnetSort       | 256  | Random             |     7,645.2 ns |    478.73 ns |    250.38 ns |  0.87 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **SingleElementMoved** |     **7,541.5 ns** |    **351.80 ns** |    **184.00 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 256  | SingleElementMoved |     7,140.2 ns |    278.97 ns |    145.91 ns |  0.95 |    0.03 |    3 |         - |          NA |
| TernaryHeapSort  | 256  | SingleElementMoved |     5,598.8 ns |    232.31 ns |    103.15 ns |  0.74 |    0.02 |    3 |         - |          NA |
| BottomupHeapSort | 256  | SingleElementMoved |     7,448.8 ns |    352.44 ns |    184.33 ns |  0.99 |    0.03 |    3 |         - |          NA |
| WeakHeapSort     | 256  | SingleElementMoved |     7,002.3 ns |    483.42 ns |    252.84 ns |  0.93 |    0.04 |    3 |         - |          NA |
| SmoothSort       | 256  | SingleElementMoved |     1,854.2 ns |    322.00 ns |    168.41 ns |  0.25 |    0.02 |    1 |         - |          NA |
| PoplarHeapSort   | 256  | SingleElementMoved |     3,858.9 ns |     52.35 ns |     18.67 ns |  0.51 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 256  | SingleElementMoved |     5,865.1 ns |    565.73 ns |    295.89 ns |  0.78 |    0.04 |    3 |         - |          NA |
| BinomialHeapSort | 256  | SingleElementMoved |     7,883.5 ns |    376.34 ns |    196.83 ns |  1.05 |    0.03 |    3 |         - |          NA |
| PairingHeapSort  | 256  | SingleElementMoved |     5,779.4 ns |    425.03 ns |    222.30 ns |  0.77 |    0.03 |    3 |         - |          NA |
| DotnetSort       | 256  | SingleElementMoved |     1,948.7 ns |    180.89 ns |     94.61 ns |  0.26 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **Sorted**             |     **7,239.7 ns** |    **233.81 ns** |    **122.29 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| MinHeapSort      | 256  | Sorted             |     6,729.9 ns |    176.62 ns |     92.37 ns |  0.93 |    0.02 |    5 |         - |          NA |
| TernaryHeapSort  | 256  | Sorted             |     4,189.3 ns |    202.21 ns |    105.76 ns |  0.58 |    0.02 |    4 |         - |          NA |
| BottomupHeapSort | 256  | Sorted             |     6,953.6 ns |     56.05 ns |     19.99 ns |  0.96 |    0.02 |    5 |         - |          NA |
| WeakHeapSort     | 256  | Sorted             |     6,419.7 ns |    295.33 ns |    154.46 ns |  0.89 |    0.02 |    5 |         - |          NA |
| SmoothSort       | 256  | Sorted             |     1,268.6 ns |      6.94 ns |      3.08 ns |  0.18 |    0.00 |    2 |         - |          NA |
| PoplarHeapSort   | 256  | Sorted             |     3,343.9 ns |     10.18 ns |      4.52 ns |  0.46 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 256  | Sorted             |     5,360.8 ns |    210.93 ns |    110.32 ns |  0.74 |    0.02 |    5 |         - |          NA |
| BinomialHeapSort | 256  | Sorted             |     6,757.1 ns |    195.76 ns |    102.39 ns |  0.93 |    0.02 |    5 |         - |          NA |
| PairingHeapSort  | 256  | Sorted             |     5,552.3 ns |    283.36 ns |    148.20 ns |  0.77 |    0.02 |    5 |         - |          NA |
| DotnetSort       | 256  | Sorted             |       923.0 ns |    141.03 ns |     73.76 ns |  0.13 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **Reversed**           |     **6,663.9 ns** |    **296.91 ns** |    **155.29 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 256  | Reversed           |     7,301.6 ns |    245.80 ns |    128.56 ns |  1.10 |    0.03 |    4 |         - |          NA |
| TernaryHeapSort  | 256  | Reversed           |     3,773.9 ns |    118.04 ns |     61.74 ns |  0.57 |    0.02 |    3 |         - |          NA |
| BottomupHeapSort | 256  | Reversed           |     7,359.6 ns |    271.51 ns |    142.00 ns |  1.10 |    0.03 |    4 |         - |          NA |
| WeakHeapSort     | 256  | Reversed           |     7,358.0 ns |    313.66 ns |    164.05 ns |  1.10 |    0.03 |    4 |         - |          NA |
| SmoothSort       | 256  | Reversed           |     4,869.8 ns |    333.07 ns |    147.88 ns |  0.73 |    0.03 |    4 |         - |          NA |
| PoplarHeapSort   | 256  | Reversed           |     5,903.2 ns |    299.29 ns |    156.53 ns |  0.89 |    0.03 |    4 |         - |          NA |
| TournamentSort   | 256  | Reversed           |     5,532.9 ns |    316.24 ns |    165.40 ns |  0.83 |    0.03 |    4 |         - |          NA |
| BinomialHeapSort | 256  | Reversed           |     6,686.3 ns |    216.25 ns |    113.10 ns |  1.00 |    0.03 |    4 |         - |          NA |
| PairingHeapSort  | 256  | Reversed           |     2,576.5 ns |     29.40 ns |     10.48 ns |  0.39 |    0.01 |    2 |         - |          NA |
| DotnetSort       | 256  | Reversed           |     1,522.0 ns |    202.58 ns |     89.95 ns |  0.23 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **PipeOrgan**          |     **6,883.7 ns** |    **259.11 ns** |    **135.52 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 256  | PipeOrgan          |     6,564.0 ns |     40.16 ns |     14.32 ns |  0.95 |    0.02 |    3 |         - |          NA |
| TernaryHeapSort  | 256  | PipeOrgan          |     4,149.1 ns |    414.03 ns |    216.55 ns |  0.60 |    0.03 |    2 |         - |          NA |
| BottomupHeapSort | 256  | PipeOrgan          |     7,149.4 ns |    241.45 ns |    126.28 ns |  1.04 |    0.03 |    3 |         - |          NA |
| WeakHeapSort     | 256  | PipeOrgan          |     6,898.0 ns |    360.16 ns |    188.37 ns |  1.00 |    0.03 |    3 |         - |          NA |
| SmoothSort       | 256  | PipeOrgan          |     5,067.2 ns |    147.49 ns |     65.49 ns |  0.74 |    0.02 |    3 |         - |          NA |
| PoplarHeapSort   | 256  | PipeOrgan          |     5,883.5 ns |     80.26 ns |     28.62 ns |  0.85 |    0.02 |    3 |         - |          NA |
| TournamentSort   | 256  | PipeOrgan          |     3,963.8 ns |    291.30 ns |    129.34 ns |  0.58 |    0.02 |    2 |         - |          NA |
| BinomialHeapSort | 256  | PipeOrgan          |     7,860.7 ns |    362.23 ns |    189.45 ns |  1.14 |    0.03 |    3 |         - |          NA |
| PairingHeapSort  | 256  | PipeOrgan          |     6,902.3 ns |    232.59 ns |    121.65 ns |  1.00 |    0.02 |    3 |         - |          NA |
| DotnetSort       | 256  | PipeOrgan          |     2,398.8 ns |    340.08 ns |    177.87 ns |  0.35 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **ManyDuplicates**     |     **8,520.3 ns** |    **250.92 ns** |    **111.41 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | ManyDuplicates     |     8,729.6 ns |    409.26 ns |    214.05 ns |  1.02 |    0.03 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | ManyDuplicates     |    12,902.4 ns |    413.67 ns |    216.35 ns |  1.51 |    0.03 |    2 |         - |          NA |
| BottomupHeapSort | 256  | ManyDuplicates     |     9,043.6 ns |    264.48 ns |    138.33 ns |  1.06 |    0.02 |    1 |         - |          NA |
| WeakHeapSort     | 256  | ManyDuplicates     |    13,835.8 ns |    217.95 ns |     96.77 ns |  1.62 |    0.02 |    2 |         - |          NA |
| SmoothSort       | 256  | ManyDuplicates     |    16,130.2 ns |    136.46 ns |     60.59 ns |  1.89 |    0.02 |    2 |         - |          NA |
| PoplarHeapSort   | 256  | ManyDuplicates     |    18,324.4 ns |  3,783.90 ns |  1,979.05 ns |  2.15 |    0.22 |    2 |         - |          NA |
| TournamentSort   | 256  | ManyDuplicates     |    17,164.0 ns |    413.99 ns |    216.53 ns |  2.01 |    0.03 |    2 |         - |          NA |
| BinomialHeapSort | 256  | ManyDuplicates     |    32,279.5 ns |    699.00 ns |    365.59 ns |  3.79 |    0.06 |    3 |         - |          NA |
| PairingHeapSort  | 256  | ManyDuplicates     |    19,867.8 ns |    292.15 ns |    129.72 ns |  2.33 |    0.03 |    2 |         - |          NA |
| DotnetSort       | 256  | ManyDuplicates     |     7,177.5 ns |    320.26 ns |    167.50 ns |  0.84 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Random**             |    **39,861.6 ns** |    **322.38 ns** |    **168.61 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Random             |    39,591.9 ns |    279.50 ns |    146.18 ns |  0.99 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | Random             |    68,039.1 ns |    445.48 ns |    232.99 ns |  1.71 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | Random             |    41,816.7 ns |  1,013.59 ns |    530.13 ns |  1.05 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | Random             |    82,902.5 ns |    961.48 ns |    342.87 ns |  2.08 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 1024 | Random             |    92,232.7 ns |    781.47 ns |    408.73 ns |  2.31 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 1024 | Random             |    85,099.4 ns |    633.42 ns |    281.24 ns |  2.13 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 1024 | Random             |    84,682.9 ns |    463.99 ns |    206.01 ns |  2.12 |    0.01 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | Random             |   203,628.5 ns |  1,019.78 ns |    533.36 ns |  5.11 |    0.02 |    4 |         - |          NA |
| PairingHeapSort  | 1024 | Random             |   104,344.7 ns |  1,861.99 ns |    826.73 ns |  2.62 |    0.02 |    3 |         - |          NA |
| DotnetSort       | 1024 | Random             |    38,664.2 ns |    473.69 ns |    210.32 ns |  0.97 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **SingleElementMoved** |    **33,018.4 ns** |     **75.16 ns** |     **39.31 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 1024 | SingleElementMoved |    32,558.5 ns |    244.41 ns |    127.83 ns |  0.99 |    0.00 |    4 |         - |          NA |
| TernaryHeapSort  | 1024 | SingleElementMoved |    24,112.9 ns |    386.48 ns |    171.60 ns |  0.73 |    0.00 |    3 |         - |          NA |
| BottomupHeapSort | 1024 | SingleElementMoved |    32,240.6 ns |    153.53 ns |     80.30 ns |  0.98 |    0.00 |    4 |         - |          NA |
| WeakHeapSort     | 1024 | SingleElementMoved |    33,302.9 ns |    287.57 ns |    150.41 ns |  1.01 |    0.00 |    4 |         - |          NA |
| SmoothSort       | 1024 | SingleElementMoved |     6,600.6 ns |    392.05 ns |    205.05 ns |  0.20 |    0.01 |    1 |         - |          NA |
| PoplarHeapSort   | 1024 | SingleElementMoved |    16,828.2 ns |    477.44 ns |    211.98 ns |  0.51 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 1024 | SingleElementMoved |    29,890.8 ns |  1,113.18 ns |    582.21 ns |  0.91 |    0.02 |    4 |         - |          NA |
| BinomialHeapSort | 1024 | SingleElementMoved |    33,884.9 ns |    388.89 ns |    203.40 ns |  1.03 |    0.01 |    4 |         - |          NA |
| PairingHeapSort  | 1024 | SingleElementMoved |    22,357.8 ns |    444.34 ns |    197.29 ns |  0.68 |    0.01 |    3 |         - |          NA |
| DotnetSort       | 1024 | SingleElementMoved |     6,895.4 ns |    315.40 ns |    164.96 ns |  0.21 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Sorted**             |    **32,001.3 ns** |    **225.31 ns** |    **100.04 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Sorted             |    30,006.4 ns |    268.23 ns |    140.29 ns |  0.94 |    0.00 |    3 |         - |          NA |
| TernaryHeapSort  | 1024 | Sorted             |    20,215.3 ns |    405.41 ns |    212.04 ns |  0.63 |    0.01 |    3 |         - |          NA |
| BottomupHeapSort | 1024 | Sorted             |    31,567.6 ns |    784.74 ns |    279.85 ns |  0.99 |    0.01 |    3 |         - |          NA |
| WeakHeapSort     | 1024 | Sorted             |    31,795.1 ns |    225.08 ns |    117.72 ns |  0.99 |    0.00 |    3 |         - |          NA |
| SmoothSort       | 1024 | Sorted             |     5,267.0 ns |    372.88 ns |    195.02 ns |  0.16 |    0.01 |    1 |         - |          NA |
| PoplarHeapSort   | 1024 | Sorted             |    14,975.0 ns |    220.25 ns |    115.19 ns |  0.47 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 1024 | Sorted             |    25,895.9 ns |    472.56 ns |    247.16 ns |  0.81 |    0.01 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | Sorted             |    30,852.8 ns |    229.76 ns |    102.02 ns |  0.96 |    0.00 |    3 |         - |          NA |
| PairingHeapSort  | 1024 | Sorted             |    21,929.6 ns |    334.90 ns |    148.70 ns |  0.69 |    0.00 |    3 |         - |          NA |
| DotnetSort       | 1024 | Sorted             |     4,456.6 ns |    814.87 ns |    426.19 ns |  0.14 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Reversed**           |    **29,644.1 ns** |    **116.99 ns** |     **51.95 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Reversed           |    32,322.7 ns |    131.75 ns |     58.50 ns |  1.09 |    0.00 |    4 |         - |          NA |
| TernaryHeapSort  | 1024 | Reversed           |    18,197.4 ns |    387.83 ns |    172.20 ns |  0.61 |    0.01 |    3 |         - |          NA |
| BottomupHeapSort | 1024 | Reversed           |    32,235.5 ns |    179.97 ns |     94.13 ns |  1.09 |    0.00 |    4 |         - |          NA |
| WeakHeapSort     | 1024 | Reversed           |    35,525.1 ns |    226.43 ns |    118.43 ns |  1.20 |    0.00 |    4 |         - |          NA |
| SmoothSort       | 1024 | Reversed           |    23,769.9 ns |    681.52 ns |    302.60 ns |  0.80 |    0.01 |    4 |         - |          NA |
| PoplarHeapSort   | 1024 | Reversed           |    27,417.7 ns |    689.67 ns |    360.71 ns |  0.92 |    0.01 |    4 |         - |          NA |
| TournamentSort   | 1024 | Reversed           |    27,788.2 ns |  1,728.48 ns |    904.03 ns |  0.94 |    0.03 |    4 |         - |          NA |
| BinomialHeapSort | 1024 | Reversed           |    29,442.8 ns |    160.13 ns |     71.10 ns |  0.99 |    0.00 |    4 |         - |          NA |
| PairingHeapSort  | 1024 | Reversed           |    11,259.0 ns |    318.08 ns |    166.36 ns |  0.38 |    0.01 |    2 |         - |          NA |
| DotnetSort       | 1024 | Reversed           |     7,635.8 ns |  1,584.47 ns |    828.71 ns |  0.26 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **PipeOrgan**          |    **31,447.7 ns** |    **218.42 ns** |    **114.24 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | PipeOrgan          |    29,856.0 ns |    260.78 ns |    136.39 ns |  0.95 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | PipeOrgan          |    19,099.4 ns |    831.50 ns |    434.89 ns |  0.61 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | PipeOrgan          |    31,438.9 ns |    244.62 ns |    108.61 ns |  1.00 |    0.00 |    2 |         - |          NA |
| WeakHeapSort     | 1024 | PipeOrgan          |    34,544.5 ns |    350.77 ns |    155.75 ns |  1.10 |    0.01 |    2 |         - |          NA |
| SmoothSort       | 1024 | PipeOrgan          |    26,702.2 ns |    466.96 ns |    207.33 ns |  0.85 |    0.01 |    2 |         - |          NA |
| PoplarHeapSort   | 1024 | PipeOrgan          |    28,218.0 ns |    410.88 ns |    182.43 ns |  0.90 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 1024 | PipeOrgan          |    19,475.4 ns |    591.14 ns |    309.18 ns |  0.62 |    0.01 |    1 |         - |          NA |
| BinomialHeapSort | 1024 | PipeOrgan          |    33,666.6 ns |    414.84 ns |    184.19 ns |  1.07 |    0.01 |    2 |         - |          NA |
| PairingHeapSort  | 1024 | PipeOrgan          |    30,236.9 ns |    142.66 ns |     74.61 ns |  0.96 |    0.00 |    2 |         - |          NA |
| DotnetSort       | 1024 | PipeOrgan          |    16,682.9 ns |    415.31 ns |    184.40 ns |  0.53 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **ManyDuplicates**     |    **37,678.7 ns** |    **244.86 ns** |    **128.06 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | ManyDuplicates     |    38,221.4 ns |    373.13 ns |    195.15 ns |  1.01 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | ManyDuplicates     |    62,341.2 ns |    702.00 ns |    311.69 ns |  1.65 |    0.01 |    3 |         - |          NA |
| BottomupHeapSort | 1024 | ManyDuplicates     |    38,614.1 ns |    230.96 ns |    120.80 ns |  1.02 |    0.00 |    2 |         - |          NA |
| WeakHeapSort     | 1024 | ManyDuplicates     |    63,445.5 ns |    293.09 ns |    153.29 ns |  1.68 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 1024 | ManyDuplicates     |    83,357.9 ns |    655.86 ns |    233.88 ns |  2.21 |    0.01 |    4 |         - |          NA |
| PoplarHeapSort   | 1024 | ManyDuplicates     |    79,930.3 ns |    589.39 ns |    261.69 ns |  2.12 |    0.01 |    4 |         - |          NA |
| TournamentSort   | 1024 | ManyDuplicates     |    83,262.2 ns |  2,039.23 ns |  1,066.56 ns |  2.21 |    0.03 |    4 |         - |          NA |
| BinomialHeapSort | 1024 | ManyDuplicates     |   160,258.7 ns |  2,258.84 ns |  1,181.42 ns |  4.25 |    0.03 |    5 |         - |          NA |
| PairingHeapSort  | 1024 | ManyDuplicates     |    89,571.4 ns |    740.34 ns |    387.21 ns |  2.38 |    0.01 |    4 |         - |          NA |
| DotnetSort       | 1024 | ManyDuplicates     |    30,094.7 ns |    359.45 ns |    188.00 ns |  0.80 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Random**             |   **168,908.6 ns** |    **613.95 ns** |    **272.60 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Random             |   171,733.7 ns |  1,682.15 ns |    879.80 ns |  1.02 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 4096 | Random             |   323,566.6 ns |  1,738.83 ns |    909.44 ns |  1.92 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 4096 | Random             |   184,233.8 ns |    976.14 ns |    510.54 ns |  1.09 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | Random             |   400,157.1 ns |  1,247.41 ns |    652.42 ns |  2.37 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 4096 | Random             |   438,931.7 ns |  1,892.91 ns |    840.46 ns |  2.60 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 4096 | Random             |   399,561.5 ns |  1,408.87 ns |    736.87 ns |  2.37 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 4096 | Random             |   403,260.8 ns |  1,082.73 ns |    386.11 ns |  2.39 |    0.00 |    3 |         - |          NA |
| BinomialHeapSort | 4096 | Random             | 1,060,276.8 ns |  3,474.62 ns |  1,817.30 ns |  6.28 |    0.01 |    4 |         - |          NA |
| PairingHeapSort  | 4096 | Random             |   503,776.9 ns |  2,783.65 ns |  1,235.96 ns |  2.98 |    0.01 |    3 |         - |          NA |
| DotnetSort       | 4096 | Random             |   186,486.0 ns |  1,286.85 ns |    673.05 ns |  1.10 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **SingleElementMoved** |   **132,171.2 ns** |  **1,395.22 ns** |    **619.49 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| MinHeapSort      | 4096 | SingleElementMoved |   135,652.6 ns |    572.14 ns |    299.24 ns |  1.03 |    0.00 |    5 |         - |          NA |
| TernaryHeapSort  | 4096 | SingleElementMoved |   149,319.6 ns |    892.68 ns |    396.36 ns |  1.13 |    0.01 |    5 |         - |          NA |
| BottomupHeapSort | 4096 | SingleElementMoved |   143,019.2 ns |  1,138.97 ns |    505.71 ns |  1.08 |    0.01 |    5 |         - |          NA |
| WeakHeapSort     | 4096 | SingleElementMoved |   159,512.9 ns |    521.52 ns |    231.56 ns |  1.21 |    0.01 |    5 |         - |          NA |
| SmoothSort       | 4096 | SingleElementMoved |    25,980.1 ns |     54.38 ns |     19.39 ns |  0.20 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 4096 | SingleElementMoved |    72,572.3 ns |    381.37 ns |    169.33 ns |  0.55 |    0.00 |    3 |         - |          NA |
| TournamentSort   | 4096 | SingleElementMoved |   181,131.5 ns | 14,052.72 ns |  6,239.50 ns |  1.37 |    0.04 |    5 |         - |          NA |
| BinomialHeapSort | 4096 | SingleElementMoved |   147,717.7 ns |    801.00 ns |    285.65 ns |  1.12 |    0.01 |    5 |         - |          NA |
| PairingHeapSort  | 4096 | SingleElementMoved |    88,851.6 ns |    551.57 ns |    244.90 ns |  0.67 |    0.00 |    4 |         - |          NA |
| DotnetSort       | 4096 | SingleElementMoved |    31,756.2 ns |    450.10 ns |    199.85 ns |  0.24 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Sorted**             |   **130,255.1 ns** |    **571.96 ns** |    **253.95 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Sorted             |   126,803.7 ns |  1,392.51 ns |    618.28 ns |  0.97 |    0.00 |    4 |         - |          NA |
| TernaryHeapSort  | 4096 | Sorted             |    97,725.6 ns |  1,375.73 ns |    610.83 ns |  0.75 |    0.00 |    3 |         - |          NA |
| BottomupHeapSort | 4096 | Sorted             |   140,308.8 ns |    411.56 ns |    215.25 ns |  1.08 |    0.00 |    4 |         - |          NA |
| WeakHeapSort     | 4096 | Sorted             |   156,413.5 ns |  2,665.95 ns |  1,394.34 ns |  1.20 |    0.01 |    4 |         - |          NA |
| SmoothSort       | 4096 | Sorted             |    20,911.8 ns |    214.16 ns |    112.01 ns |  0.16 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 4096 | Sorted             |    66,001.3 ns |    392.06 ns |    205.06 ns |  0.51 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 4096 | Sorted             |   131,777.8 ns |  4,987.27 ns |  2,608.44 ns |  1.01 |    0.02 |    4 |         - |          NA |
| BinomialHeapSort | 4096 | Sorted             |   133,697.7 ns |  1,416.68 ns |    740.95 ns |  1.03 |    0.01 |    4 |         - |          NA |
| PairingHeapSort  | 4096 | Sorted             |    88,941.1 ns |    631.37 ns |    280.33 ns |  0.68 |    0.00 |    3 |         - |          NA |
| DotnetSort       | 4096 | Sorted             |    20,671.2 ns |  3,414.55 ns |  1,785.88 ns |  0.16 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Reversed**           |   **125,999.6 ns** |    **870.63 ns** |    **455.36 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Reversed           |   130,530.7 ns |    396.24 ns |    175.93 ns |  1.04 |    0.00 |    3 |         - |          NA |
| TernaryHeapSort  | 4096 | Reversed           |    90,274.3 ns |  1,636.35 ns |    583.54 ns |  0.72 |    0.00 |    2 |         - |          NA |
| BottomupHeapSort | 4096 | Reversed           |   144,248.2 ns |  1,304.41 ns |    682.23 ns |  1.14 |    0.01 |    3 |         - |          NA |
| WeakHeapSort     | 4096 | Reversed           |   169,549.0 ns |  2,263.95 ns |  1,005.21 ns |  1.35 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 4096 | Reversed           |   132,642.2 ns |  2,847.78 ns |  1,489.44 ns |  1.05 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 4096 | Reversed           |   123,680.7 ns |  2,478.09 ns |  1,100.29 ns |  0.98 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 4096 | Reversed           |   170,445.6 ns | 10,025.67 ns |  4,451.46 ns |  1.35 |    0.03 |    3 |         - |          NA |
| BinomialHeapSort | 4096 | Reversed           |   129,906.1 ns |    779.08 ns |    345.91 ns |  1.03 |    0.00 |    3 |         - |          NA |
| PairingHeapSort  | 4096 | Reversed           |    41,998.3 ns |    346.04 ns |    153.64 ns |  0.33 |    0.00 |    1 |         - |          NA |
| DotnetSort       | 4096 | Reversed           |    35,888.9 ns |  2,658.36 ns |  1,390.37 ns |  0.28 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **PipeOrgan**          |   **131,629.6 ns** |  **1,069.25 ns** |    **559.24 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 4096 | PipeOrgan          |   129,116.4 ns |    880.48 ns |    390.94 ns |  0.98 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 4096 | PipeOrgan          |    94,272.9 ns |  4,996.53 ns |  2,613.28 ns |  0.72 |    0.02 |    1 |         - |          NA |
| BottomupHeapSort | 4096 | PipeOrgan          |   142,881.0 ns |    633.81 ns |    281.42 ns |  1.09 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | PipeOrgan          |   169,774.0 ns |    676.12 ns |    300.20 ns |  1.29 |    0.01 |    1 |         - |          NA |
| SmoothSort       | 4096 | PipeOrgan          |   277,782.0 ns |  2,227.06 ns |  1,164.80 ns |  2.11 |    0.01 |    2 |         - |          NA |
| PoplarHeapSort   | 4096 | PipeOrgan          |   139,587.0 ns |  3,565.06 ns |  1,582.91 ns |  1.06 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 4096 | PipeOrgan          |   104,204.5 ns |  3,468.06 ns |  1,539.84 ns |  0.79 |    0.01 |    1 |         - |          NA |
| BinomialHeapSort | 4096 | PipeOrgan          |   146,290.8 ns |    478.37 ns |    212.40 ns |  1.11 |    0.00 |    1 |         - |          NA |
| PairingHeapSort  | 4096 | PipeOrgan          |   119,288.7 ns |  2,265.47 ns |  1,005.88 ns |  0.91 |    0.01 |    1 |         - |          NA |
| DotnetSort       | 4096 | PipeOrgan          |    93,675.4 ns |  4,501.09 ns |  2,354.15 ns |  0.71 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **ManyDuplicates**     |   **157,899.7 ns** |    **851.63 ns** |    **378.13 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 4096 | ManyDuplicates     |   157,286.8 ns |    601.81 ns |    267.21 ns |  1.00 |    0.00 |    2 |         - |          NA |
| TernaryHeapSort  | 4096 | ManyDuplicates     |   294,963.4 ns |    753.48 ns |    268.70 ns |  1.87 |    0.00 |    3 |         - |          NA |
| BottomupHeapSort | 4096 | ManyDuplicates     |   164,319.2 ns |  1,097.59 ns |    487.34 ns |  1.04 |    0.00 |    2 |         - |          NA |
| WeakHeapSort     | 4096 | ManyDuplicates     |   285,444.8 ns |    719.97 ns |    319.67 ns |  1.81 |    0.00 |    3 |         - |          NA |
| SmoothSort       | 4096 | ManyDuplicates     |   391,068.9 ns |    873.30 ns |    387.75 ns |  2.48 |    0.01 |    4 |         - |          NA |
| PoplarHeapSort   | 4096 | ManyDuplicates     |   374,929.8 ns |  1,266.65 ns |    562.40 ns |  2.37 |    0.01 |    4 |         - |          NA |
| TournamentSort   | 4096 | ManyDuplicates     |   401,083.5 ns |  1,426.20 ns |    745.93 ns |  2.54 |    0.01 |    4 |         - |          NA |
| BinomialHeapSort | 4096 | ManyDuplicates     |   711,184.8 ns |  1,924.13 ns |    854.33 ns |  4.50 |    0.01 |    5 |         - |          NA |
| PairingHeapSort  | 4096 | ManyDuplicates     |   393,950.5 ns |  1,481.91 ns |    528.46 ns |  2.49 |    0.01 |    4 |         - |          NA |
| DotnetSort       | 4096 | ManyDuplicates     |   122,465.3 ns |  1,021.40 ns |    534.21 ns |  0.78 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Random**             |   **347,975.6 ns** |  **1,188.30 ns** |    **621.50 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Random             |   352,452.0 ns |  1,183.60 ns |    619.04 ns |  1.01 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | Random             |   697,535.9 ns |  5,652.16 ns |  2,509.59 ns |  2.00 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | Random             |   396,636.3 ns |  1,321.98 ns |    691.42 ns |  1.14 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | Random             |   868,786.5 ns |  3,903.98 ns |  2,041.85 ns |  2.50 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 8192 | Random             |   971,419.5 ns |  3,814.96 ns |  1,693.86 ns |  2.79 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 8192 | Random             |   854,011.5 ns |  1,396.52 ns |    498.01 ns |  2.45 |    0.00 |    3 |         - |          NA |
| TournamentSort   | 8192 | Random             |   891,124.3 ns |  1,423.64 ns |    632.10 ns |  2.56 |    0.00 |    3 |         - |          NA |
| BinomialHeapSort | 8192 | Random             | 2,330,615.0 ns |  5,708.32 ns |  2,534.53 ns |  6.70 |    0.01 |    4 |         - |          NA |
| PairingHeapSort  | 8192 | Random             | 1,124,325.2 ns | 10,863.90 ns |  4,823.64 ns |  3.23 |    0.01 |    3 |         - |          NA |
| DotnetSort       | 8192 | Random             |   407,123.4 ns |  2,363.40 ns |  1,236.10 ns |  1.17 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **SingleElementMoved** |   **278,855.2 ns** |  **1,060.96 ns** |    **554.90 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 8192 | SingleElementMoved |   277,041.9 ns |  2,743.08 ns |    978.21 ns |  0.99 |    0.00 |    4 |         - |          NA |
| TernaryHeapSort  | 8192 | SingleElementMoved |   430,339.0 ns |  2,204.10 ns |    978.63 ns |  1.54 |    0.00 |    5 |         - |          NA |
| BottomupHeapSort | 8192 | SingleElementMoved |   313,511.1 ns |  1,037.23 ns |    460.54 ns |  1.12 |    0.00 |    4 |         - |          NA |
| WeakHeapSort     | 8192 | SingleElementMoved |   349,633.9 ns |  2,017.72 ns |  1,055.31 ns |  1.25 |    0.00 |    4 |         - |          NA |
| SmoothSort       | 8192 | SingleElementMoved |    51,642.4 ns |    700.12 ns |    366.17 ns |  0.19 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 8192 | SingleElementMoved |   152,262.4 ns |  1,224.61 ns |    543.74 ns |  0.55 |    0.00 |    3 |         - |          NA |
| TournamentSort   | 8192 | SingleElementMoved |   505,768.7 ns |  3,148.54 ns |  1,122.80 ns |  1.81 |    0.01 |    5 |         - |          NA |
| BinomialHeapSort | 8192 | SingleElementMoved |   307,142.4 ns |  2,927.75 ns |  1,299.94 ns |  1.10 |    0.00 |    4 |         - |          NA |
| PairingHeapSort  | 8192 | SingleElementMoved |   181,149.3 ns |    780.14 ns |    346.39 ns |  0.65 |    0.00 |    3 |         - |          NA |
| DotnetSort       | 8192 | SingleElementMoved |    68,533.0 ns |  2,246.31 ns |    997.37 ns |  0.25 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Sorted**             |   **277,290.5 ns** |  **1,885.28 ns** |    **986.04 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Sorted             |   257,760.6 ns |  1,188.18 ns |    527.56 ns |  0.93 |    0.00 |    4 |         - |          NA |
| TernaryHeapSort  | 8192 | Sorted             |   431,827.6 ns |  3,010.24 ns |  1,574.41 ns |  1.56 |    0.01 |    5 |         - |          NA |
| BottomupHeapSort | 8192 | Sorted             |   309,900.1 ns |  1,984.87 ns |  1,038.13 ns |  1.12 |    0.01 |    4 |         - |          NA |
| WeakHeapSort     | 8192 | Sorted             |   341,629.8 ns |    942.08 ns |    418.29 ns |  1.23 |    0.00 |    4 |         - |          NA |
| SmoothSort       | 8192 | Sorted             |    41,571.1 ns |    463.91 ns |    242.63 ns |  0.15 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 8192 | Sorted             |   140,217.0 ns |    539.90 ns |    239.72 ns |  0.51 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 8192 | Sorted             |   500,167.8 ns | 22,825.78 ns | 11,938.32 ns |  1.80 |    0.04 |    5 |         - |          NA |
| BinomialHeapSort | 8192 | Sorted             |   278,478.8 ns |    719.57 ns |    319.49 ns |  1.00 |    0.00 |    4 |         - |          NA |
| PairingHeapSort  | 8192 | Sorted             |   182,006.1 ns |  1,068.30 ns |    474.33 ns |  0.66 |    0.00 |    3 |         - |          NA |
| DotnetSort       | 8192 | Sorted             |    43,837.3 ns |  2,701.99 ns |  1,413.19 ns |  0.16 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Reversed**           |   **256,103.2 ns** |    **909.65 ns** |    **475.77 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Reversed           |   278,819.5 ns |    675.22 ns |    353.15 ns |  1.09 |    0.00 |    2 |         - |          NA |
| TernaryHeapSort  | 8192 | Reversed           |   404,241.1 ns |  2,124.70 ns |  1,111.26 ns |  1.58 |    0.00 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | Reversed           |   315,708.9 ns |    982.48 ns |    436.23 ns |  1.23 |    0.00 |    2 |         - |          NA |
| WeakHeapSort     | 8192 | Reversed           |   367,436.7 ns |  1,168.50 ns |    611.15 ns |  1.43 |    0.00 |    2 |         - |          NA |
| SmoothSort       | 8192 | Reversed           |   565,692.7 ns |  2,043.92 ns |  1,069.01 ns |  2.21 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 8192 | Reversed           |   403,441.6 ns |  5,319.12 ns |  2,361.72 ns |  1.58 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 8192 | Reversed           |   592,947.9 ns |  2,195.44 ns |    974.79 ns |  2.32 |    0.01 |    3 |         - |          NA |
| BinomialHeapSort | 8192 | Reversed           |   274,772.2 ns |  1,597.46 ns |    709.28 ns |  1.07 |    0.00 |    2 |         - |          NA |
| PairingHeapSort  | 8192 | Reversed           |    82,138.4 ns |  1,392.77 ns |    618.40 ns |  0.32 |    0.00 |    1 |         - |          NA |
| DotnetSort       | 8192 | Reversed           |    77,829.1 ns |  6,328.65 ns |  2,809.96 ns |  0.30 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **PipeOrgan**          |   **272,160.7 ns** |  **3,672.41 ns** |  **1,630.57 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | PipeOrgan          |   263,048.9 ns |    842.07 ns |    373.88 ns |  0.97 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | PipeOrgan          |   481,814.1 ns |  3,329.34 ns |  1,478.25 ns |  1.77 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | PipeOrgan          |   315,525.9 ns |  1,952.68 ns |    867.00 ns |  1.16 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | PipeOrgan          |   372,123.4 ns |  1,146.13 ns |    599.45 ns |  1.37 |    0.01 |    1 |         - |          NA |
| SmoothSort       | 8192 | PipeOrgan          |   706,360.7 ns |    666.00 ns |    295.71 ns |  2.60 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 8192 | PipeOrgan          |   583,812.6 ns |    546.33 ns |    194.83 ns |  2.15 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 8192 | PipeOrgan          |   510,043.7 ns |  1,244.40 ns |    552.52 ns |  1.87 |    0.01 |    2 |         - |          NA |
| BinomialHeapSort | 8192 | PipeOrgan          |   305,583.1 ns |  1,555.76 ns |    690.77 ns |  1.12 |    0.01 |    1 |         - |          NA |
| PairingHeapSort  | 8192 | PipeOrgan          |   244,543.3 ns |  3,640.74 ns |  1,904.18 ns |  0.90 |    0.01 |    1 |         - |          NA |
| DotnetSort       | 8192 | PipeOrgan          |   363,105.9 ns |  5,820.04 ns |  3,044.00 ns |  1.33 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **ManyDuplicates**     |   **317,187.3 ns** |  **1,247.75 ns** |    **652.60 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 8192 | ManyDuplicates     |   319,383.8 ns |  1,264.88 ns |    661.56 ns |  1.01 |    0.00 |    2 |         - |          NA |
| TernaryHeapSort  | 8192 | ManyDuplicates     |   639,969.2 ns |  4,085.67 ns |  2,136.89 ns |  2.02 |    0.01 |    3 |         - |          NA |
| BottomupHeapSort | 8192 | ManyDuplicates     |   353,291.8 ns |  2,042.07 ns |    906.69 ns |  1.11 |    0.00 |    2 |         - |          NA |
| WeakHeapSort     | 8192 | ManyDuplicates     |   599,903.2 ns |  2,400.09 ns |  1,065.66 ns |  1.89 |    0.00 |    3 |         - |          NA |
| SmoothSort       | 8192 | ManyDuplicates     |   817,434.7 ns |  4,595.26 ns |  2,040.32 ns |  2.58 |    0.01 |    4 |         - |          NA |
| PoplarHeapSort   | 8192 | ManyDuplicates     |   794,196.8 ns |  1,754.57 ns |    779.04 ns |  2.50 |    0.01 |    4 |         - |          NA |
| TournamentSort   | 8192 | ManyDuplicates     |   850,078.6 ns |  1,531.16 ns |    546.03 ns |  2.68 |    0.01 |    4 |         - |          NA |
| BinomialHeapSort | 8192 | ManyDuplicates     | 1,479,038.0 ns |  8,410.37 ns |  4,398.79 ns |  4.66 |    0.02 |    5 |         - |          NA |
| PairingHeapSort  | 8192 | ManyDuplicates     |   828,359.4 ns |  6,893.17 ns |  3,060.61 ns |  2.61 |    0.01 |    4 |         - |          NA |
| DotnetSort       | 8192 | ManyDuplicates     |   246,991.6 ns |  5,242.56 ns |  2,327.73 ns |  0.78 |    0.01 |    1 |         - |          NA |

### InsertionBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.86GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.22 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                 | Size | Pattern            | Mean            | Error        | StdDev       | Ratio    | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------- |----- |------------------- |----------------:|-------------:|-------------:|---------:|--------:|-----:|----------:|------------:|
| **InsertionSort**          | **256**  | **Random**             |      **9,370.8 ns** |    **439.75 ns** |    **195.25 ns** |     **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Random             |      8,582.3 ns |    828.92 ns |    433.54 ns |     0.92 |    0.05 |    1 |         - |          NA |
| BinaryInsertSort       | 256  | Random             |     14,908.9 ns |    295.14 ns |    154.36 ns |     1.59 |    0.03 |    1 |         - |          NA |
| GnomeSort              | 256  | Random             |     14,079.3 ns |    146.12 ns |     76.42 ns |     1.50 |    0.03 |    1 |         - |          NA |
| LibrarySort            | 256  | Random             |     29,576.4 ns |    254.37 ns |    133.04 ns |     3.16 |    0.06 |    2 |         - |          NA |
| MergeInsertionSort     | 256  | Random             |     25,901.3 ns |    441.57 ns |    196.06 ns |     2.77 |    0.06 |    2 |         - |          NA |
| ShellSortKnuth1973     | 256  | Random             |     11,091.0 ns |    399.99 ns |    209.20 ns |     1.18 |    0.03 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Random             |     11,001.8 ns |    273.40 ns |    142.99 ns |     1.17 |    0.03 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | Random             |     11,879.4 ns |    287.42 ns |    150.33 ns |     1.27 |    0.03 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | Random             |     11,805.3 ns |    250.85 ns |    131.20 ns |     1.26 |    0.03 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Random             |     11,987.5 ns |    164.26 ns |     85.91 ns |     1.28 |    0.03 |    1 |         - |          NA |
| DotnetSort             | 256  | Random             |      7,688.9 ns |    438.91 ns |    229.56 ns |     0.82 |    0.03 |    1 |         - |          NA |
|      |                    |                 |              |              |          |         |      |           |             |
| **InsertionSort**          | **256**  | **SingleElementMoved** |        **283.1 ns** |     **10.67 ns** |      **5.58 ns** |     **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 256  | SingleElementMoved |        334.3 ns |      4.61 ns |      2.04 ns |     1.18 |    0.02 |    1 |         - |          NA |
| BinaryInsertSort       | 256  | SingleElementMoved |      1,366.7 ns |    326.88 ns |    170.97 ns |     4.83 |    0.58 |    2 |         - |          NA |
| GnomeSort              | 256  | SingleElementMoved |        338.0 ns |     19.34 ns |      8.59 ns |     1.19 |    0.04 |    1 |         - |          NA |
| LibrarySort            | 256  | SingleElementMoved |     27,856.9 ns |    222.87 ns |    116.57 ns |    98.43 |    1.84 |    4 |         - |          NA |
| MergeInsertionSort     | 256  | SingleElementMoved |     13,156.6 ns |    196.56 ns |    102.80 ns |    46.49 |    0.92 |    3 |         - |          NA |
| ShellSortKnuth1973     | 256  | SingleElementMoved |      1,030.2 ns |     24.97 ns |     11.09 ns |     3.64 |    0.08 |    2 |         - |          NA |
| ShellSortSedgewick1986 | 256  | SingleElementMoved |      1,063.7 ns |     33.00 ns |     14.65 ns |     3.76 |    0.08 |    2 |         - |          NA |
| ShellSortTokuda1992    | 256  | SingleElementMoved |      1,258.5 ns |     22.96 ns |     10.19 ns |     4.45 |    0.09 |    2 |         - |          NA |
| ShellSortCiura2001     | 256  | SingleElementMoved |      1,149.2 ns |     16.43 ns |      8.59 ns |     4.06 |    0.08 |    2 |         - |          NA |
| ShellSortLee2021       | 256  | SingleElementMoved |      1,265.9 ns |     32.25 ns |     16.87 ns |     4.47 |    0.10 |    2 |         - |          NA |
| DotnetSort             | 256  | SingleElementMoved |      1,437.7 ns |    141.13 ns |     62.66 ns |     5.08 |    0.23 |    2 |         - |          NA |
|      |                    |                 |              |              |          |         |      |           |             |
| **InsertionSort**          | **256**  | **Sorted**             |        **214.1 ns** |     **15.75 ns** |      **8.24 ns** |     **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Sorted             |        276.0 ns |     14.03 ns |      5.00 ns |     1.29 |    0.06 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | Sorted             |      1,124.3 ns |      8.24 ns |      3.66 ns |     5.26 |    0.21 |    4 |         - |          NA |
| GnomeSort              | 256  | Sorted             |        279.9 ns |      1.59 ns |      0.71 ns |     1.31 |    0.05 |    2 |         - |          NA |
| LibrarySort            | 256  | Sorted             |     26,389.3 ns |    211.69 ns |     93.99 ns |   123.46 |    4.87 |    6 |         - |          NA |
| MergeInsertionSort     | 256  | Sorted             |     12,440.6 ns |    356.27 ns |    186.34 ns |    58.20 |    2.43 |    5 |         - |          NA |
| ShellSortKnuth1973     | 256  | Sorted             |      1,216.7 ns |     15.40 ns |      6.84 ns |     5.69 |    0.23 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Sorted             |      1,233.8 ns |     10.60 ns |      4.71 ns |     5.77 |    0.23 |    4 |         - |          NA |
| ShellSortTokuda1992    | 256  | Sorted             |      1,499.1 ns |     18.31 ns |      8.13 ns |     7.01 |    0.28 |    4 |         - |          NA |
| ShellSortCiura2001     | 256  | Sorted             |      1,325.6 ns |     38.88 ns |     20.33 ns |     6.20 |    0.26 |    4 |         - |          NA |
| ShellSortLee2021       | 256  | Sorted             |      1,506.9 ns |      4.80 ns |      2.51 ns |     7.05 |    0.28 |    4 |         - |          NA |
| DotnetSort             | 256  | Sorted             |        811.5 ns |     14.13 ns |      6.27 ns |     3.80 |    0.15 |    3 |         - |          NA |
|      |                    |                 |              |              |          |         |      |           |             |
| **InsertionSort**          | **256**  | **Reversed**           |     **15,350.7 ns** |    **248.82 ns** |    **130.14 ns** |     **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Reversed           |     12,332.7 ns |     89.62 ns |     39.79 ns |     0.80 |    0.01 |    3 |         - |          NA |
| BinaryInsertSort       | 256  | Reversed           |      5,761.2 ns |     29.27 ns |     13.00 ns |     0.38 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 256  | Reversed           |     26,964.2 ns |    754.82 ns |    394.79 ns |     1.76 |    0.03 |    5 |         - |          NA |
| LibrarySort            | 256  | Reversed           |     22,676.7 ns |    250.84 ns |    131.19 ns |     1.48 |    0.01 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | Reversed           |     12,483.4 ns |    314.63 ns |    164.56 ns |     0.81 |    0.01 |    3 |         - |          NA |
| ShellSortKnuth1973     | 256  | Reversed           |      1,497.3 ns |     19.94 ns |      7.11 ns |     0.10 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Reversed           |      1,419.4 ns |     38.14 ns |     13.60 ns |     0.09 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | Reversed           |      1,443.2 ns |     25.00 ns |     11.10 ns |     0.09 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | Reversed           |      1,454.6 ns |     42.72 ns |     18.97 ns |     0.09 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Reversed           |      1,495.1 ns |     33.73 ns |     14.98 ns |     0.10 |    0.00 |    1 |         - |          NA |
| DotnetSort             | 256  | Reversed           |      1,388.7 ns |    272.54 ns |    142.54 ns |     0.09 |    0.01 |    1 |         - |          NA |
|      |                    |                 |              |              |          |         |      |           |             |
| **InsertionSort**          | **256**  | **PipeOrgan**          |      **8,121.6 ns** |    **435.12 ns** |    **193.20 ns** |     **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PairInsertionSort      | 256  | PipeOrgan          |      5,403.9 ns |  1,733.64 ns |    906.73 ns |     0.67 |    0.11 |    4 |         - |          NA |
| BinaryInsertSort       | 256  | PipeOrgan          |      3,810.4 ns |    412.08 ns |    215.53 ns |     0.47 |    0.03 |    3 |         - |          NA |
| GnomeSort              | 256  | PipeOrgan          |     12,706.4 ns |    394.15 ns |    206.15 ns |     1.57 |    0.04 |    6 |         - |          NA |
| LibrarySort            | 256  | PipeOrgan          |     19,793.7 ns |    639.49 ns |    283.94 ns |     2.44 |    0.06 |    7 |         - |          NA |
| MergeInsertionSort     | 256  | PipeOrgan          |     12,683.6 ns |    297.37 ns |    155.53 ns |     1.56 |    0.04 |    6 |         - |          NA |
| ShellSortKnuth1973     | 256  | PipeOrgan          |      1,539.5 ns |     39.18 ns |     17.40 ns |     0.19 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | PipeOrgan          |      1,565.5 ns |     31.45 ns |     16.45 ns |     0.19 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | PipeOrgan          |      1,851.5 ns |     81.34 ns |     29.01 ns |     0.23 |    0.01 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | PipeOrgan          |      1,611.6 ns |     24.34 ns |     10.81 ns |     0.20 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | PipeOrgan          |      1,734.1 ns |     20.59 ns |      9.14 ns |     0.21 |    0.00 |    1 |         - |          NA |
| DotnetSort             | 256  | PipeOrgan          |      2,700.7 ns |    309.95 ns |    162.11 ns |     0.33 |    0.02 |    2 |         - |          NA |
|      |                    |                 |              |              |          |         |      |           |             |
| **InsertionSort**          | **256**  | **ManyDuplicates**     |      **9,431.5 ns** |    **392.45 ns** |    **174.25 ns** |     **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 256  | ManyDuplicates     |      8,212.3 ns |    214.38 ns |     76.45 ns |     0.87 |    0.02 |    1 |         - |          NA |
| BinaryInsertSort       | 256  | ManyDuplicates     |     17,096.2 ns |  3,104.23 ns |  1,623.57 ns |     1.81 |    0.17 |    3 |         - |          NA |
| GnomeSort              | 256  | ManyDuplicates     |     13,845.3 ns |    329.19 ns |    172.17 ns |     1.47 |    0.03 |    2 |         - |          NA |
| LibrarySort            | 256  | ManyDuplicates     |     30,786.2 ns |    389.13 ns |    172.78 ns |     3.27 |    0.06 |    4 |         - |          NA |
| MergeInsertionSort     | 256  | ManyDuplicates     |     25,874.9 ns |    382.59 ns |    200.10 ns |     2.74 |    0.05 |    4 |         - |          NA |
| ShellSortKnuth1973     | 256  | ManyDuplicates     |      9,921.7 ns |    450.49 ns |    235.62 ns |     1.05 |    0.03 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | ManyDuplicates     |     10,076.1 ns |    277.09 ns |    144.93 ns |     1.07 |    0.02 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | ManyDuplicates     |     10,178.5 ns |    235.99 ns |    123.43 ns |     1.08 |    0.02 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | ManyDuplicates     |     10,210.9 ns |    250.42 ns |    130.98 ns |     1.08 |    0.02 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | ManyDuplicates     |      9,894.5 ns |    401.54 ns |    210.01 ns |     1.05 |    0.03 |    1 |         - |          NA |
| DotnetSort             | 256  | ManyDuplicates     |      7,235.6 ns |    524.34 ns |    274.24 ns |     0.77 |    0.03 |    1 |         - |          NA |
|      |                    |                 |              |              |          |         |      |           |             |
| **InsertionSort**          | **1024** | **Random**             |    **122,145.2 ns** |  **1,757.75 ns** |    **780.45 ns** |     **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Random             |     97,301.8 ns |  2,723.10 ns |  1,424.23 ns |     0.80 |    0.01 |    4 |         - |          NA |
| BinaryInsertSort       | 1024 | Random             |     79,407.6 ns |    385.53 ns |    171.18 ns |     0.65 |    0.00 |    3 |         - |          NA |
| GnomeSort              | 1024 | Random             |    194,803.2 ns |  1,827.48 ns |    811.41 ns |     1.59 |    0.01 |    6 |         - |          NA |
| LibrarySort            | 1024 | Random             |    137,898.3 ns |    844.18 ns |    374.82 ns |     1.13 |    0.01 |    5 |         - |          NA |
| MergeInsertionSort     | 1024 | Random             |    135,160.5 ns |    965.76 ns |    344.40 ns |     1.11 |    0.01 |    5 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Random             |     59,372.2 ns |    163.72 ns |     72.69 ns |     0.49 |    0.00 |    2 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Random             |     62,248.6 ns |    503.95 ns |    263.58 ns |     0.51 |    0.00 |    2 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Random             |     65,082.2 ns |    491.81 ns |    257.23 ns |     0.53 |    0.00 |    2 |         - |          NA |
| ShellSortCiura2001     | 1024 | Random             |     64,085.6 ns |    314.46 ns |    164.47 ns |     0.52 |    0.00 |    2 |         - |          NA |
| ShellSortLee2021       | 1024 | Random             |     64,128.1 ns |    154.21 ns |     80.65 ns |     0.53 |    0.00 |    2 |         - |          NA |
| DotnetSort             | 1024 | Random             |     38,512.3 ns |    680.93 ns |    356.14 ns |     0.32 |    0.00 |    1 |         - |          NA |
|      |                    |                 |              |              |          |         |      |           |             |
| **InsertionSort**          | **1024** | **SingleElementMoved** |        **939.7 ns** |     **39.62 ns** |     **20.72 ns** |     **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | SingleElementMoved |      1,076.1 ns |     16.09 ns |      7.14 ns |     1.15 |    0.02 |    1 |         - |          NA |
| BinaryInsertSort       | 1024 | SingleElementMoved |      6,548.0 ns |  1,307.55 ns |    683.87 ns |     6.97 |    0.70 |    2 |         - |          NA |
| GnomeSort              | 1024 | SingleElementMoved |      1,219.7 ns |    253.79 ns |    132.74 ns |     1.30 |    0.14 |    1 |         - |          NA |
| LibrarySort            | 1024 | SingleElementMoved |    259,852.8 ns |  2,971.05 ns |  1,319.16 ns |   276.66 |    5.85 |    4 |         - |          NA |
| MergeInsertionSort     | 1024 | SingleElementMoved |     59,286.2 ns |    359.91 ns |    188.24 ns |    63.12 |    1.31 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | SingleElementMoved |      4,469.7 ns |     43.00 ns |     15.33 ns |     4.76 |    0.10 |    2 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | SingleElementMoved |      5,018.2 ns |    263.02 ns |    116.78 ns |     5.34 |    0.16 |    2 |         - |          NA |
| ShellSortTokuda1992    | 1024 | SingleElementMoved |      5,340.7 ns |     99.55 ns |     35.50 ns |     5.69 |    0.12 |    2 |         - |          NA |
| ShellSortCiura2001     | 1024 | SingleElementMoved |      5,297.7 ns |     76.59 ns |     27.31 ns |     5.64 |    0.12 |    2 |         - |          NA |
| ShellSortLee2021       | 1024 | SingleElementMoved |      5,506.5 ns |    354.21 ns |    185.26 ns |     5.86 |    0.22 |    2 |         - |          NA |
| DotnetSort             | 1024 | SingleElementMoved |      6,838.8 ns |    483.44 ns |    214.65 ns |     7.28 |    0.26 |    2 |         - |          NA |
|      |                    |                 |              |              |          |         |      |           |             |
| **InsertionSort**          | **1024** | **Sorted**             |        **895.8 ns** |    **464.39 ns** |    **242.89 ns** |     **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Sorted             |      1,273.8 ns |    749.67 ns |    332.86 ns |     1.50 |    0.48 |    3 |         - |          NA |
| BinaryInsertSort       | 1024 | Sorted             |      5,427.2 ns |    292.91 ns |    130.06 ns |     6.38 |    1.32 |    5 |         - |          NA |
| GnomeSort              | 1024 | Sorted             |      1,090.9 ns |      7.56 ns |      3.95 ns |     1.28 |    0.26 |    2 |         - |          NA |
| LibrarySort            | 1024 | Sorted             |    215,873.6 ns |    864.15 ns |    383.69 ns |   253.94 |   52.21 |    7 |         - |          NA |
| MergeInsertionSort     | 1024 | Sorted             |     59,484.2 ns |    549.68 ns |    244.06 ns |    69.97 |   14.39 |    6 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Sorted             |      5,946.6 ns |     32.47 ns |     14.42 ns |     7.00 |    1.44 |    5 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Sorted             |      6,941.1 ns |    334.56 ns |    174.98 ns |     8.16 |    1.69 |    5 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Sorted             |      7,312.9 ns |    366.72 ns |    191.80 ns |     8.60 |    1.78 |    5 |         - |          NA |
| ShellSortCiura2001     | 1024 | Sorted             |      7,201.3 ns |    403.29 ns |    179.07 ns |     8.47 |    1.75 |    5 |         - |          NA |
| ShellSortLee2021       | 1024 | Sorted             |      7,324.2 ns |    243.97 ns |    108.32 ns |     8.62 |    1.78 |    5 |         - |          NA |
| DotnetSort             | 1024 | Sorted             |      4,166.8 ns |    424.02 ns |    221.77 ns |     4.90 |    1.04 |    4 |         - |          NA |
|      |                    |                 |              |              |          |         |      |           |             |
| **InsertionSort**          | **1024** | **Reversed**           |    **229,541.0 ns** |  **1,331.16 ns** |    **591.04 ns** |     **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Reversed           |    188,246.9 ns |  1,323.73 ns |    587.75 ns |     0.82 |    0.00 |    4 |         - |          NA |
| BinaryInsertSort       | 1024 | Reversed           |     41,379.6 ns |    365.33 ns |    191.08 ns |     0.18 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | Reversed           |    397,464.1 ns |  4,858.93 ns |  2,541.31 ns |     1.73 |    0.01 |    5 |         - |          NA |
| LibrarySort            | 1024 | Reversed           |    196,554.5 ns |  1,954.90 ns |    867.99 ns |     0.86 |    0.00 |    4 |         - |          NA |
| MergeInsertionSort     | 1024 | Reversed           |     60,031.5 ns |    432.91 ns |    226.42 ns |     0.26 |    0.00 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Reversed           |      7,075.7 ns |    260.20 ns |    136.09 ns |     0.03 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Reversed           |      7,344.8 ns |    390.85 ns |    204.42 ns |     0.03 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Reversed           |      7,849.6 ns |    362.34 ns |    189.51 ns |     0.03 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Reversed           |      7,583.8 ns |    369.13 ns |    193.06 ns |     0.03 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Reversed           |      7,694.2 ns |    610.71 ns |    217.78 ns |     0.03 |    0.00 |    1 |         - |          NA |
| DotnetSort             | 1024 | Reversed           |      8,256.4 ns |    848.57 ns |    443.82 ns |     0.04 |    0.00 |    1 |         - |          NA |
|      |                    |                 |              |              |          |         |      |           |             |
| **InsertionSort**          | **1024** | **PipeOrgan**          |    **118,957.3 ns** |  **7,156.51 ns** |  **3,742.99 ns** |     **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | PipeOrgan          |     65,618.2 ns |  1,514.56 ns |    672.48 ns |     0.55 |    0.02 |    4 |         - |          NA |
| BinaryInsertSort       | 1024 | PipeOrgan          |     24,560.8 ns |    711.00 ns |    371.87 ns |     0.21 |    0.01 |    3 |         - |          NA |
| GnomeSort              | 1024 | PipeOrgan          |    191,898.2 ns |  4,383.13 ns |  2,292.46 ns |     1.61 |    0.05 |    6 |         - |          NA |
| LibrarySort            | 1024 | PipeOrgan          |    111,594.1 ns |  1,861.96 ns |    973.84 ns |     0.94 |    0.03 |    5 |         - |          NA |
| MergeInsertionSort     | 1024 | PipeOrgan          |     59,412.8 ns |    253.65 ns |    112.62 ns |     0.50 |    0.01 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | PipeOrgan          |      8,029.5 ns |    427.36 ns |    223.52 ns |     0.07 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | PipeOrgan          |      8,526.6 ns |    421.14 ns |    220.26 ns |     0.07 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | PipeOrgan          |      9,359.9 ns |    384.65 ns |    201.18 ns |     0.08 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | PipeOrgan          |      9,146.2 ns |    371.08 ns |    194.08 ns |     0.08 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | PipeOrgan          |      9,523.8 ns |    435.18 ns |    227.61 ns |     0.08 |    0.00 |    1 |         - |          NA |
| DotnetSort             | 1024 | PipeOrgan          |     16,576.8 ns |    534.08 ns |    279.33 ns |     0.14 |    0.00 |    2 |         - |          NA |
|      |                    |                 |              |              |          |         |      |           |             |
| **InsertionSort**          | **1024** | **ManyDuplicates**     |    **118,241.1 ns** |    **371.47 ns** |    **132.47 ns** |     **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | ManyDuplicates     |     96,912.0 ns |  3,977.32 ns |  2,080.21 ns |     0.82 |    0.02 |    4 |         - |          NA |
| BinaryInsertSort       | 1024 | ManyDuplicates     |     77,148.8 ns |    366.71 ns |    162.82 ns |     0.65 |    0.00 |    3 |         - |          NA |
| GnomeSort              | 1024 | ManyDuplicates     |    189,564.9 ns |    396.83 ns |    176.19 ns |     1.60 |    0.00 |    6 |         - |          NA |
| LibrarySort            | 1024 | ManyDuplicates     |    145,285.2 ns |    577.27 ns |    256.31 ns |     1.23 |    0.00 |    5 |         - |          NA |
| MergeInsertionSort     | 1024 | ManyDuplicates     |    134,941.7 ns |  1,152.66 ns |    511.79 ns |     1.14 |    0.00 |    5 |         - |          NA |
| ShellSortKnuth1973     | 1024 | ManyDuplicates     |     47,219.3 ns |    341.05 ns |    151.43 ns |     0.40 |    0.00 |    2 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | ManyDuplicates     |     46,751.6 ns |    172.00 ns |     89.96 ns |     0.40 |    0.00 |    2 |         - |          NA |
| ShellSortTokuda1992    | 1024 | ManyDuplicates     |     47,701.1 ns |    384.09 ns |    200.89 ns |     0.40 |    0.00 |    2 |         - |          NA |
| ShellSortCiura2001     | 1024 | ManyDuplicates     |     46,654.4 ns |    277.17 ns |    144.97 ns |     0.39 |    0.00 |    2 |         - |          NA |
| ShellSortLee2021       | 1024 | ManyDuplicates     |     46,358.6 ns |    580.70 ns |    257.84 ns |     0.39 |    0.00 |    2 |         - |          NA |
| DotnetSort             | 1024 | ManyDuplicates     |     30,044.0 ns |    221.16 ns |     98.20 ns |     0.25 |    0.00 |    1 |         - |          NA |
|      |                    |                 |              |              |          |         |      |           |             |
| **InsertionSort**          | **4096** | **Random**             |  **1,827,704.6 ns** |  **6,882.31 ns** |  **2,454.30 ns** |     **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PairInsertionSort      | 4096 | Random             |  1,382,273.4 ns | 13,154.58 ns |  6,880.10 ns |     0.76 |    0.00 |    6 |         - |          NA |
| BinaryInsertSort       | 4096 | Random             |    500,218.6 ns |  1,561.38 ns |    816.63 ns |     0.27 |    0.00 |    3 |         - |          NA |
| GnomeSort              | 4096 | Random             |  2,996,689.9 ns |  8,592.54 ns |  3,064.18 ns |     1.64 |    0.00 |    8 |         - |          NA |
| LibrarySort            | 4096 | Random             |    622,336.8 ns |  1,348.37 ns |    598.68 ns |     0.34 |    0.00 |    4 |         - |          NA |
| MergeInsertionSort     | 4096 | Random             |    958,123.3 ns |  8,364.33 ns |  3,713.81 ns |     0.52 |    0.00 |    5 |         - |          NA |
| ShellSortKnuth1973     | 4096 | Random             |    299,628.3 ns |  1,755.71 ns |    779.55 ns |     0.16 |    0.00 |    2 |         - |          NA |
| ShellSortSedgewick1986 | 4096 | Random             |    318,110.4 ns |  1,797.24 ns |    939.99 ns |     0.17 |    0.00 |    2 |         - |          NA |
| ShellSortTokuda1992    | 4096 | Random             |    331,497.6 ns |  2,379.19 ns |  1,244.36 ns |     0.18 |    0.00 |    2 |         - |          NA |
| ShellSortCiura2001     | 4096 | Random             |    325,916.9 ns |  2,990.84 ns |  1,327.95 ns |     0.18 |    0.00 |    2 |         - |          NA |
| ShellSortLee2021       | 4096 | Random             |    325,664.2 ns |  1,092.09 ns |    571.18 ns |     0.18 |    0.00 |    2 |         - |          NA |
| DotnetSort             | 4096 | Random             |    186,686.3 ns |    869.78 ns |    454.91 ns |     0.10 |    0.00 |    1 |         - |          NA |
|      |                    |                 |              |              |          |         |      |           |             |
| **InsertionSort**          | **4096** | **SingleElementMoved** |      **3,549.7 ns** |     **31.28 ns** |     **13.89 ns** |     **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 4096 | SingleElementMoved |      4,833.9 ns |    365.52 ns |    191.17 ns |     1.36 |    0.05 |    2 |         - |          NA |
| BinaryInsertSort       | 4096 | SingleElementMoved |     29,623.8 ns |  2,283.11 ns |  1,194.11 ns |     8.35 |    0.32 |    3 |         - |          NA |
| GnomeSort              | 4096 | SingleElementMoved |      4,407.3 ns |     66.64 ns |     23.77 ns |     1.24 |    0.01 |    2 |         - |          NA |
| LibrarySort            | 4096 | SingleElementMoved |  3,039,621.4 ns | 10,397.70 ns |  3,707.92 ns |   856.31 |    3.27 |    5 |         - |          NA |
| MergeInsertionSort     | 4096 | SingleElementMoved |    466,721.2 ns |  2,074.12 ns |    920.92 ns |   131.48 |    0.54 |    4 |         - |          NA |
| ShellSortKnuth1973     | 4096 | SingleElementMoved |     20,904.1 ns |    177.51 ns |     92.84 ns |     5.89 |    0.03 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 4096 | SingleElementMoved |     23,495.9 ns |    329.87 ns |    172.53 ns |     6.62 |    0.05 |    3 |         - |          NA |
| ShellSortTokuda1992    | 4096 | SingleElementMoved |     26,130.7 ns |     78.89 ns |     41.26 ns |     7.36 |    0.03 |    3 |         - |          NA |
| ShellSortCiura2001     | 4096 | SingleElementMoved |     25,610.0 ns |    165.06 ns |     73.29 ns |     7.21 |    0.03 |    3 |         - |          NA |
| ShellSortLee2021       | 4096 | SingleElementMoved |     26,495.4 ns |    239.86 ns |    106.50 ns |     7.46 |    0.04 |    3 |         - |          NA |
| DotnetSort             | 4096 | SingleElementMoved |     31,574.1 ns |    232.46 ns |    103.21 ns |     8.89 |    0.04 |    3 |         - |          NA |
|      |                    |                 |              |              |          |         |      |           |             |
| **InsertionSort**          | **4096** | **Sorted**             |      **2,881.0 ns** |     **18.86 ns** |      **8.37 ns** |     **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 4096 | Sorted             |      3,610.1 ns |     15.12 ns |      5.39 ns |     1.25 |    0.00 |    2 |         - |          NA |
| BinaryInsertSort       | 4096 | Sorted             |     27,340.6 ns |  1,023.29 ns |    454.35 ns |     9.49 |    0.15 |    4 |         - |          NA |
| GnomeSort              | 4096 | Sorted             |      4,411.4 ns |    284.10 ns |    148.59 ns |     1.53 |    0.05 |    2 |         - |          NA |
| LibrarySort            | 4096 | Sorted             |  2,427,236.8 ns |  7,527.19 ns |  2,684.27 ns |   842.50 |    2.45 |    6 |         - |          NA |
| MergeInsertionSort     | 4096 | Sorted             |    463,976.7 ns |  1,190.26 ns |    528.48 ns |   161.05 |    0.47 |    5 |         - |          NA |
| ShellSortKnuth1973     | 4096 | Sorted             |     28,974.0 ns |    214.02 ns |    111.94 ns |    10.06 |    0.05 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 4096 | Sorted             |     33,082.3 ns |    374.80 ns |    166.42 ns |    11.48 |    0.06 |    4 |         - |          NA |
| ShellSortTokuda1992    | 4096 | Sorted             |     37,139.6 ns |    345.08 ns |    180.48 ns |    12.89 |    0.07 |    4 |         - |          NA |
| ShellSortCiura2001     | 4096 | Sorted             |     36,310.9 ns |    499.10 ns |    261.04 ns |    12.60 |    0.09 |    4 |         - |          NA |
| ShellSortLee2021       | 4096 | Sorted             |     37,100.4 ns |    331.51 ns |    173.39 ns |    12.88 |    0.07 |    4 |         - |          NA |
| DotnetSort             | 4096 | Sorted             |     19,440.8 ns |    409.28 ns |    214.06 ns |     6.75 |    0.07 |    3 |         - |          NA |
|      |                    |                 |              |              |          |         |      |           |             |
| **InsertionSort**          | **4096** | **Reversed**           |  **3,594,333.1 ns** | **18,156.16 ns** |  **6,474.66 ns** |    **1.000** |    **0.00** |    **5** |         **-** |          **NA** |
| PairInsertionSort      | 4096 | Reversed           |  2,968,969.2 ns |  8,047.46 ns |  3,573.12 ns |    0.826 |    0.00 |    4 |         - |          NA |
| BinaryInsertSort       | 4096 | Reversed           |    446,903.4 ns |  1,444.53 ns |    641.38 ns |    0.124 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 4096 | Reversed           |  6,045,870.9 ns | 18,812.97 ns |  8,353.07 ns |    1.682 |    0.00 |    6 |         - |          NA |
| LibrarySort            | 4096 | Reversed           |  2,321,138.7 ns | 10,563.31 ns |  3,766.98 ns |    0.646 |    0.00 |    3 |         - |          NA |
| MergeInsertionSort     | 4096 | Reversed           |    468,461.1 ns |  3,727.48 ns |  1,655.02 ns |    0.130 |    0.00 |    2 |         - |          NA |
| ShellSortKnuth1973     | 4096 | Reversed           |     33,187.2 ns |    312.61 ns |    163.50 ns |    0.009 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 4096 | Reversed           |     35,898.7 ns |    522.16 ns |    273.10 ns |    0.010 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 4096 | Reversed           |     37,586.5 ns |    456.06 ns |    238.53 ns |    0.010 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 4096 | Reversed           |     36,265.1 ns |    729.12 ns |    323.73 ns |    0.010 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 4096 | Reversed           |     37,908.5 ns |    222.30 ns |     98.70 ns |    0.011 |    0.00 |    1 |         - |          NA |
| DotnetSort             | 4096 | Reversed           |     34,443.2 ns |  1,637.81 ns |    727.20 ns |    0.010 |    0.00 |    1 |         - |          NA |
|      |                    |                 |              |              |          |         |      |           |             |
| **InsertionSort**          | **4096** | **PipeOrgan**          |  **1,806,516.4 ns** |  **7,618.49 ns** |  **3,984.62 ns** |     **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PairInsertionSort      | 4096 | PipeOrgan          |  1,001,149.0 ns |  2,814.91 ns |  1,249.84 ns |     0.55 |    0.00 |    5 |         - |          NA |
| BinaryInsertSort       | 4096 | PipeOrgan          |    246,035.4 ns |  2,191.11 ns |    972.87 ns |     0.14 |    0.00 |    3 |         - |          NA |
| GnomeSort              | 4096 | PipeOrgan          |  2,979,290.3 ns |  6,294.49 ns |  3,292.14 ns |     1.65 |    0.00 |    7 |         - |          NA |
| LibrarySort            | 4096 | PipeOrgan          |    890,628.9 ns | 23,862.79 ns | 10,595.23 ns |     0.49 |    0.01 |    5 |         - |          NA |
| MergeInsertionSort     | 4096 | PipeOrgan          |    472,982.2 ns |  2,917.93 ns |  1,295.58 ns |     0.26 |    0.00 |    4 |         - |          NA |
| ShellSortKnuth1973     | 4096 | PipeOrgan          |     38,336.1 ns |    398.02 ns |    208.17 ns |     0.02 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 4096 | PipeOrgan          |     41,827.0 ns |    432.08 ns |    191.84 ns |     0.02 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 4096 | PipeOrgan          |     46,258.3 ns |    263.80 ns |    117.13 ns |     0.03 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 4096 | PipeOrgan          |     44,256.7 ns |     98.37 ns |     35.08 ns |     0.02 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 4096 | PipeOrgan          |     46,683.5 ns |    393.43 ns |    174.69 ns |     0.03 |    0.00 |    1 |         - |          NA |
| DotnetSort             | 4096 | PipeOrgan          |     93,098.7 ns |  2,610.77 ns |  1,159.20 ns |     0.05 |    0.00 |    2 |         - |          NA |
|      |                    |                 |              |              |          |         |      |           |             |
| **InsertionSort**          | **4096** | **ManyDuplicates**     |  **1,780,678.3 ns** | **12,633.51 ns** |  **5,609.36 ns** |     **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PairInsertionSort      | 4096 | ManyDuplicates     |  1,369,387.2 ns | 18,031.69 ns |  8,006.18 ns |     0.77 |    0.00 |    6 |         - |          NA |
| BinaryInsertSort       | 4096 | ManyDuplicates     |    459,558.5 ns |  2,116.79 ns |  1,107.12 ns |     0.26 |    0.00 |    3 |         - |          NA |
| GnomeSort              | 4096 | ManyDuplicates     |  2,926,989.0 ns |  8,940.72 ns |  3,188.35 ns |     1.64 |    0.01 |    8 |         - |          NA |
| LibrarySort            | 4096 | ManyDuplicates     |    656,832.6 ns |  2,252.71 ns |  1,000.22 ns |     0.37 |    0.00 |    4 |         - |          NA |
| MergeInsertionSort     | 4096 | ManyDuplicates     |    934,428.1 ns |  5,778.81 ns |  3,022.43 ns |     0.52 |    0.00 |    5 |         - |          NA |
| ShellSortKnuth1973     | 4096 | ManyDuplicates     |    208,370.9 ns |  1,358.12 ns |    603.02 ns |     0.12 |    0.00 |    2 |         - |          NA |
| ShellSortSedgewick1986 | 4096 | ManyDuplicates     |    198,188.5 ns |    840.56 ns |    373.21 ns |     0.11 |    0.00 |    2 |         - |          NA |
| ShellSortTokuda1992    | 4096 | ManyDuplicates     |    201,180.9 ns |  1,247.06 ns |    652.24 ns |     0.11 |    0.00 |    2 |         - |          NA |
| ShellSortCiura2001     | 4096 | ManyDuplicates     |    198,914.1 ns |    583.32 ns |    259.00 ns |     0.11 |    0.00 |    2 |         - |          NA |
| ShellSortLee2021       | 4096 | ManyDuplicates     |    199,026.0 ns |    990.08 ns |    517.83 ns |     0.11 |    0.00 |    2 |         - |          NA |
| DotnetSort             | 4096 | ManyDuplicates     |    122,104.7 ns |    871.04 ns |    386.75 ns |     0.07 |    0.00 |    1 |         - |          NA |
|      |                    |                 |              |              |          |         |      |           |             |
| **InsertionSort**          | **8192** | **Random**             |  **7,243,086.0 ns** | **98,656.71 ns** | **43,804.19 ns** |     **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PairInsertionSort      | 8192 | Random             |  5,408,252.6 ns | 23,489.11 ns | 12,285.26 ns |     0.75 |    0.00 |    5 |         - |          NA |
| BinaryInsertSort       | 8192 | Random             |  1,404,668.0 ns |  4,138.01 ns |  1,837.30 ns |     0.19 |    0.00 |    3 |         - |          NA |
| GnomeSort              | 8192 | Random             | 11,922,039.0 ns | 66,576.46 ns | 34,820.77 ns |     1.65 |    0.01 |    7 |         - |          NA |
| LibrarySort            | 8192 | Random             |  1,348,127.2 ns |  2,629.83 ns |  1,167.66 ns |     0.19 |    0.00 |    3 |         - |          NA |
| MergeInsertionSort     | 8192 | Random             |  2,990,680.4 ns |  3,414.00 ns |  1,515.84 ns |     0.41 |    0.00 |    4 |         - |          NA |
| ShellSortKnuth1973     | 8192 | Random             |    668,287.4 ns |  4,541.52 ns |  2,016.46 ns |     0.09 |    0.00 |    2 |         - |          NA |
| ShellSortSedgewick1986 | 8192 | Random             |    709,556.3 ns |  1,026.91 ns |    455.96 ns |     0.10 |    0.00 |    2 |         - |          NA |
| ShellSortTokuda1992    | 8192 | Random             |    727,907.8 ns |  3,613.50 ns |  1,889.93 ns |     0.10 |    0.00 |    2 |         - |          NA |
| ShellSortCiura2001     | 8192 | Random             |    716,560.3 ns |  1,851.79 ns |    822.21 ns |     0.10 |    0.00 |    2 |         - |          NA |
| ShellSortLee2021       | 8192 | Random             |    719,330.0 ns |  3,798.04 ns |  1,986.45 ns |     0.10 |    0.00 |    2 |         - |          NA |
| DotnetSort             | 8192 | Random             |    405,858.0 ns |  3,798.69 ns |  1,986.79 ns |     0.06 |    0.00 |    1 |         - |          NA |
|      |                    |                 |              |              |          |         |      |           |             |
| **InsertionSort**          | **8192** | **SingleElementMoved** |      **7,204.6 ns** |    **299.94 ns** |    **133.17 ns** |     **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 8192 | SingleElementMoved |      8,439.6 ns |    487.04 ns |    254.73 ns |     1.17 |    0.04 |    1 |         - |          NA |
| BinaryInsertSort       | 8192 | SingleElementMoved |     64,607.2 ns |  4,506.27 ns |  2,356.87 ns |     8.97 |    0.35 |    3 |         - |          NA |
| GnomeSort              | 8192 | SingleElementMoved |      8,895.1 ns |    397.01 ns |    207.65 ns |     1.24 |    0.03 |    1 |         - |          NA |
| LibrarySort            | 8192 | SingleElementMoved | 11,599,863.0 ns | 57,882.69 ns | 30,273.77 ns | 1,610.54 |   28.07 |    5 |         - |          NA |
| MergeInsertionSort     | 8192 | SingleElementMoved |  1,609,790.4 ns |  5,218.71 ns |  2,317.14 ns |   223.51 |    3.87 |    4 |         - |          NA |
| ShellSortKnuth1973     | 8192 | SingleElementMoved |     46,076.1 ns |    345.81 ns |    153.54 ns |     6.40 |    0.11 |    2 |         - |          NA |
| ShellSortSedgewick1986 | 8192 | SingleElementMoved |     55,726.6 ns |    705.94 ns |    313.44 ns |     7.74 |    0.14 |    3 |         - |          NA |
| ShellSortTokuda1992    | 8192 | SingleElementMoved |     58,005.1 ns |    441.43 ns |    196.00 ns |     8.05 |    0.14 |    3 |         - |          NA |
| ShellSortCiura2001     | 8192 | SingleElementMoved |     56,176.0 ns |    302.53 ns |    134.32 ns |     7.80 |    0.14 |    3 |         - |          NA |
| ShellSortLee2021       | 8192 | SingleElementMoved |     57,807.8 ns |    467.01 ns |    244.26 ns |     8.03 |    0.14 |    3 |         - |          NA |
| DotnetSort             | 8192 | SingleElementMoved |     69,298.4 ns |  3,161.35 ns |  1,653.45 ns |     9.62 |    0.27 |    3 |         - |          NA |
|      |                    |                 |              |              |          |         |      |           |             |
| **InsertionSort**          | **8192** | **Sorted**             |      **5,908.9 ns** |    **334.57 ns** |    **174.99 ns** |     **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 8192 | Sorted             |      7,303.7 ns |    287.42 ns |    150.33 ns |     1.24 |    0.04 |    2 |         - |          NA |
| BinaryInsertSort       | 8192 | Sorted             |     60,238.3 ns |  2,713.79 ns |  1,419.37 ns |    10.20 |    0.36 |    4 |         - |          NA |
| GnomeSort              | 8192 | Sorted             |      8,786.8 ns |    327.25 ns |    171.16 ns |     1.49 |    0.05 |    2 |         - |          NA |
| LibrarySort            | 8192 | Sorted             |  8,899,871.4 ns | 30,849.76 ns | 16,135.02 ns | 1,507.31 |   41.55 |    7 |         - |          NA |
| MergeInsertionSort     | 8192 | Sorted             |  1,609,794.7 ns |  1,200.55 ns |    428.13 ns |   272.64 |    7.52 |    6 |         - |          NA |
| ShellSortKnuth1973     | 8192 | Sorted             |     64,930.1 ns |    577.22 ns |    205.84 ns |    11.00 |    0.31 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 8192 | Sorted             |     78,606.9 ns |    302.54 ns |    158.23 ns |    13.31 |    0.37 |    5 |         - |          NA |
| ShellSortTokuda1992    | 8192 | Sorted             |     81,984.3 ns |    221.33 ns |     98.27 ns |    13.89 |    0.38 |    5 |         - |          NA |
| ShellSortCiura2001     | 8192 | Sorted             |     79,635.8 ns |    224.67 ns |     80.12 ns |    13.49 |    0.37 |    5 |         - |          NA |
| ShellSortLee2021       | 8192 | Sorted             |     82,219.1 ns |    956.42 ns |    424.66 ns |    13.92 |    0.39 |    5 |         - |          NA |
| DotnetSort             | 8192 | Sorted             |     44,394.2 ns |  4,681.96 ns |  2,448.75 ns |     7.52 |    0.44 |    3 |         - |          NA |
|      |                    |                 |              |              |          |         |      |           |             |
| **InsertionSort**          | **8192** | **Reversed**           | **14,314,259.3 ns** | **17,620.50 ns** |  **7,823.61 ns** |    **1.000** |    **0.00** |    **5** |         **-** |          **NA** |
| PairInsertionSort      | 8192 | Reversed           | 11,873,843.2 ns | 29,600.26 ns | 13,142.70 ns |    0.830 |    0.00 |    4 |         - |          NA |
| BinaryInsertSort       | 8192 | Reversed           |  1,676,959.2 ns |  5,029.87 ns |  2,233.29 ns |    0.117 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 8192 | Reversed           | 23,927,303.3 ns | 79,494.34 ns | 41,577.08 ns |    1.672 |    0.00 |    6 |         - |          NA |
| LibrarySort            | 8192 | Reversed           |  8,694,119.2 ns | 46,369.88 ns | 24,252.35 ns |    0.607 |    0.00 |    3 |         - |          NA |
| MergeInsertionSort     | 8192 | Reversed           |  1,631,160.4 ns | 13,831.81 ns |  6,141.41 ns |    0.114 |    0.00 |    2 |         - |          NA |
| ShellSortKnuth1973     | 8192 | Reversed           |     68,062.4 ns |    810.88 ns |    360.03 ns |    0.005 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 8192 | Reversed           |     81,222.2 ns |    502.34 ns |    179.14 ns |    0.006 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 8192 | Reversed           |     85,163.7 ns |    425.06 ns |    188.73 ns |    0.006 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 8192 | Reversed           |     78,274.3 ns |    515.62 ns |    228.94 ns |    0.005 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 8192 | Reversed           |     86,537.1 ns |    911.51 ns |    404.72 ns |    0.006 |    0.00 |    1 |         - |          NA |
| DotnetSort             | 8192 | Reversed           |     76,781.8 ns |  5,961.91 ns |  3,118.19 ns |    0.005 |    0.00 |    1 |         - |          NA |
|      |                    |                 |              |              |          |         |      |           |             |
| **InsertionSort**          | **8192** | **PipeOrgan**          |  **7,183,947.9 ns** | **15,464.63 ns** |  **6,866.39 ns** |     **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PairInsertionSort      | 8192 | PipeOrgan          |  3,987,062.4 ns | 37,100.98 ns | 19,404.53 ns |     0.55 |    0.00 |    6 |         - |          NA |
| BinaryInsertSort       | 8192 | PipeOrgan          |    932,687.4 ns |  2,210.51 ns |    981.48 ns |     0.13 |    0.00 |    3 |         - |          NA |
| GnomeSort              | 8192 | PipeOrgan          | 11,872,097.3 ns | 14,361.76 ns |  6,376.71 ns |     1.65 |    0.00 |    8 |         - |          NA |
| LibrarySort            | 8192 | PipeOrgan          |  2,910,707.8 ns |  5,355.47 ns |  1,909.81 ns |     0.41 |    0.00 |    5 |         - |          NA |
| MergeInsertionSort     | 8192 | PipeOrgan          |  1,619,398.6 ns |  3,157.52 ns |  1,401.96 ns |     0.23 |    0.00 |    4 |         - |          NA |
| ShellSortKnuth1973     | 8192 | PipeOrgan          |     82,623.0 ns |    410.92 ns |    182.45 ns |     0.01 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 8192 | PipeOrgan          |     95,856.7 ns |    680.31 ns |    355.81 ns |     0.01 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 8192 | PipeOrgan          |     99,448.4 ns |    408.63 ns |    213.72 ns |     0.01 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 8192 | PipeOrgan          |     98,518.8 ns |  1,094.75 ns |    572.57 ns |     0.01 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 8192 | PipeOrgan          |    100,337.9 ns |  1,785.30 ns |    792.68 ns |     0.01 |    0.00 |    1 |         - |          NA |
| DotnetSort             | 8192 | PipeOrgan          |    363,033.7 ns |  6,099.46 ns |  3,190.13 ns |     0.05 |    0.00 |    2 |         - |          NA |
|      |                    |                 |              |              |          |         |      |           |             |
| **InsertionSort**          | **8192** | **ManyDuplicates**     |  **7,029,206.0 ns** | **19,322.72 ns** |  **8,579.41 ns** |     **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PairInsertionSort      | 8192 | ManyDuplicates     |  5,307,913.5 ns | 25,325.72 ns | 11,244.78 ns |     0.76 |    0.00 |    5 |         - |          NA |
| BinaryInsertSort       | 8192 | ManyDuplicates     |  1,276,413.4 ns |  2,386.44 ns |  1,248.16 ns |     0.18 |    0.00 |    3 |         - |          NA |
| GnomeSort              | 8192 | ManyDuplicates     | 11,582,733.0 ns | 17,322.59 ns |  7,691.34 ns |     1.65 |    0.00 |    7 |         - |          NA |
| LibrarySort            | 8192 | ManyDuplicates     |  1,393,980.5 ns |  5,454.57 ns |  2,852.84 ns |     0.20 |    0.00 |    3 |         - |          NA |
| MergeInsertionSort     | 8192 | ManyDuplicates     |  2,919,546.9 ns |  4,218.18 ns |  1,872.90 ns |     0.42 |    0.00 |    4 |         - |          NA |
| ShellSortKnuth1973     | 8192 | ManyDuplicates     |    436,128.5 ns |  2,408.92 ns |  1,259.91 ns |     0.06 |    0.00 |    2 |         - |          NA |
| ShellSortSedgewick1986 | 8192 | ManyDuplicates     |    394,284.1 ns |  4,651.76 ns |  2,065.41 ns |     0.06 |    0.00 |    2 |         - |          NA |
| ShellSortTokuda1992    | 8192 | ManyDuplicates     |    410,803.1 ns |  2,502.16 ns |  1,308.68 ns |     0.06 |    0.00 |    2 |         - |          NA |
| ShellSortCiura2001     | 8192 | ManyDuplicates     |    410,043.8 ns |    738.93 ns |    328.09 ns |     0.06 |    0.00 |    2 |         - |          NA |
| ShellSortLee2021       | 8192 | ManyDuplicates     |    407,396.0 ns |  2,222.40 ns |    986.76 ns |     0.06 |    0.00 |    2 |         - |          NA |
| DotnetSort             | 8192 | ManyDuplicates     |    245,767.9 ns |  1,419.92 ns |    742.65 ns |     0.03 |    0.00 |    1 |         - |          NA |

### MergeBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.86GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.22 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                   | Size | Pattern            | Mean           | Error        | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------------- |----- |------------------- |---------------:|-------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **MergeSort**                | **256**  | **Random**             |    **16,506.2 ns** |    **385.29 ns** |   **201.51 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Random             |    15,353.7 ns |    116.36 ns |    51.67 ns |  0.93 |    0.01 |    3 |         - |          NA |
| BottomupMergeSort        | 256  | Random             |    11,937.9 ns |    447.33 ns |   233.96 ns |  0.72 |    0.02 |    3 |         - |          NA |
| StdStableSort            | 256  | Random             |     7,865.3 ns |    367.11 ns |   192.01 ns |  0.48 |    0.01 |    2 |         - |          NA |
| RotateMergeSort          | 256  | Random             |    24,204.7 ns |    652.52 ns |   341.28 ns |  1.47 |    0.03 |    4 |         - |          NA |
| RotateMergeSortRecursive | 256  | Random             |    29,099.5 ns |    587.52 ns |   307.29 ns |  1.76 |    0.03 |    5 |         - |          NA |
| SymMergeSort             | 256  | Random             |    16,093.6 ns |    727.28 ns |   380.38 ns |  0.98 |    0.02 |    3 |         - |          NA |
| BlockMergeSort           | 256  | Random             |    12,718.9 ns |    208.59 ns |   109.10 ns |  0.77 |    0.01 |    3 |         - |          NA |
| NaturalMergeSort         | 256  | Random             |    11,838.6 ns |    308.08 ns |   161.13 ns |  0.72 |    0.01 |    3 |         - |          NA |
| PatienceSort             | 256  | Random             |    16,935.2 ns |    168.74 ns |    74.92 ns |  1.03 |    0.01 |    3 |         - |          NA |
| DropMergeSort            | 256  | Random             |    12,300.3 ns |    292.36 ns |   152.91 ns |  0.75 |    0.01 |    3 |         - |          NA |
| TimSort                  | 256  | Random             |    13,195.9 ns |    436.95 ns |   228.53 ns |  0.80 |    0.02 |    3 |         - |          NA |
| PowerSort                | 256  | Random             |     8,508.1 ns |    325.78 ns |   170.39 ns |  0.52 |    0.01 |    2 |         - |          NA |
| ShiftSort                | 256  | Random             |    13,122.6 ns |    288.75 ns |   151.02 ns |  0.80 |    0.01 |    3 |         - |          NA |
| SpinSort                 | 256  | Random             |     7,625.9 ns |    452.19 ns |   236.50 ns |  0.46 |    0.01 |    2 |         - |          NA |
| Glidesort                | 256  | Random             |     8,101.8 ns |    312.85 ns |   163.63 ns |  0.49 |    0.01 |    2 |         - |          NA |
| Driftsort                | 256  | Random             |     6,917.5 ns |    523.09 ns |   273.59 ns |  0.42 |    0.02 |    2 |         - |          NA |
| FlatStableSort           | 256  | Random             |     8,606.2 ns |    334.72 ns |   175.07 ns |  0.52 |    0.01 |    2 |         - |          NA |
| QuadSort                 | 256  | Random             |     4,473.9 ns |    471.25 ns |   246.47 ns |  0.27 |    0.01 |    1 |         - |          NA |
| SimdMergeSort            | 256  | Random             |     4,717.4 ns |    155.43 ns |    81.29 ns |  0.29 |    0.01 |    1 |         - |          NA |
| DotnetSort               | 256  | Random             |     7,608.9 ns |    299.24 ns |   156.51 ns |  0.46 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **256**  | **SingleElementMoved** |     **4,512.0 ns** |     **25.10 ns** |    **11.14 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | SingleElementMoved |     5,614.0 ns |    327.05 ns |   171.05 ns |  1.24 |    0.04 |    7 |         - |          NA |
| BottomupMergeSort        | 256  | SingleElementMoved |     1,755.6 ns |    100.70 ns |    44.71 ns |  0.39 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 256  | SingleElementMoved |       716.7 ns |     27.01 ns |    14.13 ns |  0.16 |    0.00 |    2 |         - |          NA |
| RotateMergeSort          | 256  | SingleElementMoved |       597.4 ns |     65.41 ns |    29.04 ns |  0.13 |    0.01 |    2 |         - |          NA |
| RotateMergeSortRecursive | 256  | SingleElementMoved |       930.2 ns |     63.78 ns |    33.36 ns |  0.21 |    0.01 |    2 |         - |          NA |
| SymMergeSort             | 256  | SingleElementMoved |       407.2 ns |     33.79 ns |    15.00 ns |  0.09 |    0.00 |    1 |         - |          NA |
| BlockMergeSort           | 256  | SingleElementMoved |     2,987.3 ns |    323.02 ns |   168.95 ns |  0.66 |    0.04 |    5 |         - |          NA |
| NaturalMergeSort         | 256  | SingleElementMoved |       446.2 ns |     24.09 ns |    10.70 ns |  0.10 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 256  | SingleElementMoved |     9,372.1 ns |    728.93 ns |   381.25 ns |  2.08 |    0.08 |    8 |         - |          NA |
| DropMergeSort            | 256  | SingleElementMoved |       752.9 ns |     11.40 ns |     5.06 ns |  0.17 |    0.00 |    2 |         - |          NA |
| TimSort                  | 256  | SingleElementMoved |       369.6 ns |     34.31 ns |    15.23 ns |  0.08 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | SingleElementMoved |       451.5 ns |     55.58 ns |    29.07 ns |  0.10 |    0.01 |    1 |         - |          NA |
| ShiftSort                | 256  | SingleElementMoved |       587.7 ns |      8.62 ns |     3.83 ns |  0.13 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 256  | SingleElementMoved |       861.3 ns |     28.11 ns |    12.48 ns |  0.19 |    0.00 |    2 |         - |          NA |
| Glidesort                | 256  | SingleElementMoved |       862.4 ns |     54.56 ns |    24.23 ns |  0.19 |    0.01 |    2 |         - |          NA |
| Driftsort                | 256  | SingleElementMoved |       783.8 ns |     32.70 ns |    14.52 ns |  0.17 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 256  | SingleElementMoved |     1,421.6 ns |     37.15 ns |    16.49 ns |  0.32 |    0.00 |    3 |         - |          NA |
| QuadSort                 | 256  | SingleElementMoved |       679.6 ns |     32.55 ns |    14.45 ns |  0.15 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 256  | SingleElementMoved |     3,162.5 ns |    356.84 ns |   186.63 ns |  0.70 |    0.04 |    5 |         - |          NA |
| DotnetSort               | 256  | SingleElementMoved |     1,376.8 ns |     96.74 ns |    42.95 ns |  0.31 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **256**  | **Sorted**             |     **4,273.0 ns** |     **27.29 ns** |    **12.12 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Sorted             |     5,374.7 ns |    346.87 ns |   181.42 ns |  1.26 |    0.04 |    9 |         - |          NA |
| BottomupMergeSort        | 256  | Sorted             |     1,497.8 ns |     75.72 ns |    33.62 ns |  0.35 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 256  | Sorted             |       584.4 ns |     12.88 ns |     5.72 ns |  0.14 |    0.00 |    3 |         - |          NA |
| RotateMergeSort          | 256  | Sorted             |       319.5 ns |     26.21 ns |    13.71 ns |  0.07 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 256  | Sorted             |       491.0 ns |      3.27 ns |     1.45 ns |  0.11 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 256  | Sorted             |       314.2 ns |     75.50 ns |    33.52 ns |  0.07 |    0.01 |    2 |         - |          NA |
| BlockMergeSort           | 256  | Sorted             |     2,571.8 ns |     30.41 ns |    13.50 ns |  0.60 |    0.00 |    6 |         - |          NA |
| NaturalMergeSort         | 256  | Sorted             |       240.0 ns |     17.28 ns |     7.67 ns |  0.06 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 256  | Sorted             |     8,037.6 ns |    342.75 ns |   179.27 ns |  1.88 |    0.04 |   10 |         - |          NA |
| DropMergeSort            | 256  | Sorted             |       611.9 ns |     16.18 ns |     7.18 ns |  0.14 |    0.00 |    3 |         - |          NA |
| TimSort                  | 256  | Sorted             |       205.0 ns |      1.89 ns |     0.67 ns |  0.05 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | Sorted             |       175.8 ns |      9.47 ns |     4.20 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Sorted             |       233.2 ns |      5.19 ns |     2.31 ns |  0.05 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 256  | Sorted             |       148.8 ns |     10.34 ns |     5.41 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 256  | Sorted             |       205.2 ns |     11.96 ns |     6.26 ns |  0.05 |    0.00 |    1 |         - |          NA |
| Driftsort                | 256  | Sorted             |       222.5 ns |      4.39 ns |     1.95 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | Sorted             |     1,275.9 ns |     12.18 ns |     6.37 ns |  0.30 |    0.00 |    5 |         - |          NA |
| QuadSort                 | 256  | Sorted             |       319.4 ns |      9.19 ns |     4.81 ns |  0.07 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 256  | Sorted             |     3,096.4 ns |     30.84 ns |    16.13 ns |  0.72 |    0.00 |    7 |         - |          NA |
| DotnetSort               | 256  | Sorted             |       836.6 ns |     35.16 ns |    15.61 ns |  0.20 |    0.00 |    4 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **256**  | **Reversed**           |     **9,107.7 ns** |    **288.16 ns** |   **150.71 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Reversed           |     8,084.4 ns |    291.12 ns |   152.26 ns |  0.89 |    0.02 |    6 |         - |          NA |
| BottomupMergeSort        | 256  | Reversed           |     5,051.3 ns |    296.85 ns |   155.26 ns |  0.55 |    0.02 |    5 |         - |          NA |
| StdStableSort            | 256  | Reversed           |     4,202.8 ns |    269.40 ns |   140.90 ns |  0.46 |    0.02 |    5 |         - |          NA |
| RotateMergeSort          | 256  | Reversed           |     6,002.4 ns |    446.74 ns |   233.65 ns |  0.66 |    0.03 |    5 |         - |          NA |
| RotateMergeSortRecursive | 256  | Reversed           |     7,870.8 ns |    269.95 ns |   141.19 ns |  0.86 |    0.02 |    6 |         - |          NA |
| SymMergeSort             | 256  | Reversed           |     2,530.4 ns |     20.91 ns |     9.28 ns |  0.28 |    0.00 |    4 |         - |          NA |
| BlockMergeSort           | 256  | Reversed           |     3,141.4 ns |    131.04 ns |    58.18 ns |  0.35 |    0.01 |    4 |         - |          NA |
| NaturalMergeSort         | 256  | Reversed           |       294.6 ns |      7.31 ns |     3.82 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 256  | Reversed           |     1,741.7 ns |     14.70 ns |     6.53 ns |  0.19 |    0.00 |    3 |         - |          NA |
| DropMergeSort            | 256  | Reversed           |     1,562.1 ns |     16.99 ns |     7.54 ns |  0.17 |    0.00 |    3 |         - |          NA |
| TimSort                  | 256  | Reversed           |       257.8 ns |     11.43 ns |     5.08 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | Reversed           |       263.0 ns |     35.85 ns |    12.79 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Reversed           |     4,194.6 ns |    225.62 ns |   118.01 ns |  0.46 |    0.01 |    5 |         - |          NA |
| SpinSort                 | 256  | Reversed           |       289.9 ns |     12.75 ns |     6.67 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 256  | Reversed           |       293.7 ns |     24.22 ns |    10.75 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 256  | Reversed           |       320.7 ns |     15.65 ns |     8.19 ns |  0.04 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | Reversed           |     2,220.8 ns |     17.69 ns |     6.31 ns |  0.24 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 256  | Reversed           |       280.4 ns |      3.71 ns |     1.65 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SimdMergeSort            | 256  | Reversed           |     3,126.8 ns |    319.18 ns |   166.94 ns |  0.34 |    0.02 |    4 |         - |          NA |
| DotnetSort               | 256  | Reversed           |     1,331.5 ns |    173.25 ns |    76.93 ns |  0.15 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **256**  | **PipeOrgan**          |     **6,811.2 ns** |    **351.92 ns** |   **184.06 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | PipeOrgan          |     6,877.0 ns |    263.27 ns |   137.70 ns |  1.01 |    0.03 |    6 |         - |          NA |
| BottomupMergeSort        | 256  | PipeOrgan          |     3,205.9 ns |     34.95 ns |    15.52 ns |  0.47 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 256  | PipeOrgan          |     3,455.9 ns |     29.29 ns |    13.01 ns |  0.51 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 256  | PipeOrgan          |     5,930.4 ns |    577.18 ns |   301.88 ns |  0.87 |    0.05 |    6 |         - |          NA |
| RotateMergeSortRecursive | 256  | PipeOrgan          |     7,307.8 ns |    487.22 ns |   254.83 ns |  1.07 |    0.04 |    6 |         - |          NA |
| SymMergeSort             | 256  | PipeOrgan          |     3,375.2 ns |     52.50 ns |    23.31 ns |  0.50 |    0.01 |    5 |         - |          NA |
| BlockMergeSort           | 256  | PipeOrgan          |     3,319.2 ns |    660.18 ns |   345.29 ns |  0.49 |    0.05 |    5 |         - |          NA |
| NaturalMergeSort         | 256  | PipeOrgan          |       496.9 ns |      7.45 ns |     3.90 ns |  0.07 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 256  | PipeOrgan          |     5,827.5 ns |    282.70 ns |   147.86 ns |  0.86 |    0.03 |    6 |         - |          NA |
| DropMergeSort            | 256  | PipeOrgan          |     6,190.4 ns |    354.08 ns |   157.21 ns |  0.91 |    0.03 |    6 |         - |          NA |
| TimSort                  | 256  | PipeOrgan          |       759.8 ns |     32.58 ns |    11.62 ns |  0.11 |    0.00 |    2 |         - |          NA |
| PowerSort                | 256  | PipeOrgan          |       498.7 ns |     13.14 ns |     5.83 ns |  0.07 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | PipeOrgan          |     2,701.3 ns |     45.86 ns |    20.36 ns |  0.40 |    0.01 |    5 |         - |          NA |
| SpinSort                 | 256  | PipeOrgan          |     1,839.1 ns |     61.11 ns |    27.13 ns |  0.27 |    0.01 |    4 |         - |          NA |
| Glidesort                | 256  | PipeOrgan          |     1,276.8 ns |     27.08 ns |     9.66 ns |  0.19 |    0.00 |    3 |         - |          NA |
| Driftsort                | 256  | PipeOrgan          |       461.9 ns |     33.11 ns |    17.32 ns |  0.07 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | PipeOrgan          |     1,896.3 ns |     10.71 ns |     4.76 ns |  0.28 |    0.01 |    4 |         - |          NA |
| QuadSort                 | 256  | PipeOrgan          |       857.3 ns |      2.96 ns |     1.06 ns |  0.13 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 256  | PipeOrgan          |     3,011.1 ns |     51.05 ns |    22.67 ns |  0.44 |    0.01 |    5 |         - |          NA |
| DotnetSort               | 256  | PipeOrgan          |     2,281.0 ns |     42.58 ns |    15.18 ns |  0.34 |    0.01 |    5 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **256**  | **ManyDuplicates**     |    **15,850.2 ns** |    **292.37 ns** |   **152.91 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | ManyDuplicates     |    15,208.8 ns |    348.49 ns |   182.27 ns |  0.96 |    0.01 |    3 |         - |          NA |
| BottomupMergeSort        | 256  | ManyDuplicates     |    11,472.6 ns |    343.83 ns |   179.83 ns |  0.72 |    0.01 |    3 |         - |          NA |
| StdStableSort            | 256  | ManyDuplicates     |     7,416.0 ns |    275.90 ns |   144.30 ns |  0.47 |    0.01 |    2 |         - |          NA |
| RotateMergeSort          | 256  | ManyDuplicates     |    22,977.2 ns |    215.57 ns |    76.87 ns |  1.45 |    0.01 |    4 |         - |          NA |
| RotateMergeSortRecursive | 256  | ManyDuplicates     |    28,187.4 ns |    461.07 ns |   204.72 ns |  1.78 |    0.02 |    5 |         - |          NA |
| SymMergeSort             | 256  | ManyDuplicates     |    15,443.7 ns |    165.70 ns |    73.57 ns |  0.97 |    0.01 |    3 |         - |          NA |
| BlockMergeSort           | 256  | ManyDuplicates     |    12,073.9 ns |    274.43 ns |   121.85 ns |  0.76 |    0.01 |    3 |         - |          NA |
| NaturalMergeSort         | 256  | ManyDuplicates     |    11,434.7 ns |    356.49 ns |   186.45 ns |  0.72 |    0.01 |    3 |         - |          NA |
| PatienceSort             | 256  | ManyDuplicates     |    14,219.5 ns |    233.63 ns |   103.73 ns |  0.90 |    0.01 |    3 |         - |          NA |
| DropMergeSort            | 256  | ManyDuplicates     |    10,551.9 ns |    462.97 ns |   205.56 ns |  0.67 |    0.01 |    3 |         - |          NA |
| TimSort                  | 256  | ManyDuplicates     |    12,178.2 ns |    367.30 ns |   192.10 ns |  0.77 |    0.01 |    3 |         - |          NA |
| PowerSort                | 256  | ManyDuplicates     |     8,028.6 ns |    408.93 ns |   213.88 ns |  0.51 |    0.01 |    2 |         - |          NA |
| ShiftSort                | 256  | ManyDuplicates     |    12,537.9 ns |    382.97 ns |   200.30 ns |  0.79 |    0.01 |    3 |         - |          NA |
| SpinSort                 | 256  | ManyDuplicates     |     7,190.6 ns |    340.62 ns |   178.15 ns |  0.45 |    0.01 |    2 |         - |          NA |
| Glidesort                | 256  | ManyDuplicates     |     7,504.7 ns |    286.64 ns |   127.27 ns |  0.47 |    0.01 |    2 |         - |          NA |
| Driftsort                | 256  | ManyDuplicates     |     6,753.9 ns |    377.16 ns |   197.26 ns |  0.43 |    0.01 |    2 |         - |          NA |
| FlatStableSort           | 256  | ManyDuplicates     |     8,056.6 ns |    302.35 ns |   134.24 ns |  0.51 |    0.01 |    2 |         - |          NA |
| QuadSort                 | 256  | ManyDuplicates     |     4,572.3 ns |    785.92 ns |   411.05 ns |  0.29 |    0.02 |    1 |         - |          NA |
| SimdMergeSort            | 256  | ManyDuplicates     |     3,750.5 ns |    283.48 ns |   125.87 ns |  0.24 |    0.01 |    1 |         - |          NA |
| DotnetSort               | 256  | ManyDuplicates     |     7,207.1 ns |    460.98 ns |   241.10 ns |  0.45 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **1024** | **Random**             |    **76,239.7 ns** |    **954.19 ns** |   **499.06 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Random             |    71,030.0 ns |    281.88 ns |   125.15 ns |  0.93 |    0.01 |    4 |         - |          NA |
| BottomupMergeSort        | 1024 | Random             |    56,930.5 ns |    643.25 ns |   285.61 ns |  0.75 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 1024 | Random             |    41,419.4 ns |    368.87 ns |   163.78 ns |  0.54 |    0.00 |    3 |         - |          NA |
| RotateMergeSort          | 1024 | Random             |   138,997.1 ns |  1,099.59 ns |   488.23 ns |  1.82 |    0.01 |    5 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Random             |   163,482.5 ns |  1,218.42 ns |   540.98 ns |  2.14 |    0.01 |    5 |         - |          NA |
| SymMergeSort             | 1024 | Random             |    94,030.4 ns |  1,626.97 ns |   850.94 ns |  1.23 |    0.01 |    4 |         - |          NA |
| BlockMergeSort           | 1024 | Random             |    63,976.6 ns |    477.06 ns |   249.51 ns |  0.84 |    0.01 |    4 |         - |          NA |
| NaturalMergeSort         | 1024 | Random             |    57,417.2 ns |    318.86 ns |   166.77 ns |  0.75 |    0.01 |    4 |         - |          NA |
| PatienceSort             | 1024 | Random             |    83,177.6 ns |    837.08 ns |   371.67 ns |  1.09 |    0.01 |    4 |         - |          NA |
| DropMergeSort            | 1024 | Random             |    56,611.8 ns |    729.79 ns |   324.03 ns |  0.74 |    0.01 |    4 |         - |          NA |
| TimSort                  | 1024 | Random             |    64,290.2 ns |    204.04 ns |    90.59 ns |  0.84 |    0.01 |    4 |         - |          NA |
| PowerSort                | 1024 | Random             |    45,469.0 ns |    946.31 ns |   420.17 ns |  0.60 |    0.01 |    3 |         - |          NA |
| ShiftSort                | 1024 | Random             |    65,141.5 ns |     70.37 ns |    31.24 ns |  0.85 |    0.01 |    4 |         - |          NA |
| SpinSort                 | 1024 | Random             |    39,833.3 ns |    560.10 ns |   199.74 ns |  0.52 |    0.00 |    3 |         - |          NA |
| Glidesort                | 1024 | Random             |    36,161.2 ns |    245.06 ns |   108.81 ns |  0.47 |    0.00 |    3 |         - |          NA |
| Driftsort                | 1024 | Random             |    31,836.9 ns |    249.78 ns |   130.64 ns |  0.42 |    0.00 |    3 |         - |          NA |
| FlatStableSort           | 1024 | Random             |    44,755.1 ns |    423.22 ns |   187.91 ns |  0.59 |    0.00 |    3 |         - |          NA |
| QuadSort                 | 1024 | Random             |    23,099.1 ns |    218.53 ns |   114.30 ns |  0.30 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 1024 | Random             |    16,943.1 ns |    197.49 ns |   103.29 ns |  0.22 |    0.00 |    1 |         - |          NA |
| DotnetSort               | 1024 | Random             |    38,346.3 ns |    322.07 ns |   143.00 ns |  0.50 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **1024** | **SingleElementMoved** |    **17,807.6 ns** |    **131.65 ns** |    **58.45 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | SingleElementMoved |    21,756.2 ns |    446.89 ns |   233.73 ns |  1.22 |    0.01 |    8 |         - |          NA |
| BottomupMergeSort        | 1024 | SingleElementMoved |     6,628.7 ns |    350.18 ns |   183.15 ns |  0.37 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 1024 | SingleElementMoved |     3,378.9 ns |     64.59 ns |    23.03 ns |  0.19 |    0.00 |    4 |         - |          NA |
| RotateMergeSort          | 1024 | SingleElementMoved |     1,701.9 ns |    361.93 ns |   189.30 ns |  0.10 |    0.01 |    2 |         - |          NA |
| RotateMergeSortRecursive | 1024 | SingleElementMoved |     2,787.0 ns |     98.13 ns |    43.57 ns |  0.16 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 1024 | SingleElementMoved |     1,217.8 ns |     64.78 ns |    28.76 ns |  0.07 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 1024 | SingleElementMoved |    12,411.9 ns |    341.41 ns |   178.56 ns |  0.70 |    0.01 |    6 |         - |          NA |
| NaturalMergeSort         | 1024 | SingleElementMoved |     1,366.7 ns |     19.30 ns |    10.09 ns |  0.08 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 1024 | SingleElementMoved |    49,708.4 ns |  2,488.64 ns | 1,301.60 ns |  2.79 |    0.07 |    9 |         - |          NA |
| DropMergeSort            | 1024 | SingleElementMoved |     2,760.7 ns |     15.90 ns |     8.32 ns |  0.16 |    0.00 |    3 |         - |          NA |
| TimSort                  | 1024 | SingleElementMoved |       942.5 ns |     26.28 ns |    11.67 ns |  0.05 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | SingleElementMoved |     1,375.7 ns |     21.52 ns |     9.55 ns |  0.08 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 1024 | SingleElementMoved |     1,908.4 ns |     15.14 ns |     6.72 ns |  0.11 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 1024 | SingleElementMoved |     3,966.6 ns |    764.21 ns |   399.70 ns |  0.22 |    0.02 |    4 |         - |          NA |
| Glidesort                | 1024 | SingleElementMoved |     2,385.9 ns |     27.75 ns |     9.90 ns |  0.13 |    0.00 |    3 |         - |          NA |
| Driftsort                | 1024 | SingleElementMoved |     1,459.9 ns |     42.21 ns |    18.74 ns |  0.08 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 1024 | SingleElementMoved |     5,841.3 ns |    269.56 ns |   119.69 ns |  0.33 |    0.01 |    5 |         - |          NA |
| QuadSort                 | 1024 | SingleElementMoved |     1,579.0 ns |     53.91 ns |    23.93 ns |  0.09 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 1024 | SingleElementMoved |    14,639.3 ns |    155.48 ns |    69.03 ns |  0.82 |    0.00 |    6 |         - |          NA |
| DotnetSort               | 1024 | SingleElementMoved |     7,084.8 ns |    853.18 ns |   446.23 ns |  0.40 |    0.02 |    5 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **1024** | **Sorted**             |    **17,374.6 ns** |    **171.62 ns** |    **89.76 ns** |  **1.00** |    **0.00** |    **9** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Sorted             |    20,970.1 ns |    245.75 ns |   128.53 ns |  1.21 |    0.01 |   10 |         - |          NA |
| BottomupMergeSort        | 1024 | Sorted             |     5,944.3 ns |    354.14 ns |   185.22 ns |  0.34 |    0.01 |    7 |         - |          NA |
| StdStableSort            | 1024 | Sorted             |     3,292.7 ns |    435.71 ns |   227.89 ns |  0.19 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 1024 | Sorted             |     1,203.3 ns |      9.95 ns |     4.42 ns |  0.07 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Sorted             |     2,119.0 ns |    239.68 ns |   125.35 ns |  0.12 |    0.01 |    4 |         - |          NA |
| SymMergeSort             | 1024 | Sorted             |     1,191.7 ns |      6.43 ns |     2.86 ns |  0.07 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | Sorted             |    11,011.2 ns |    297.05 ns |   155.36 ns |  0.63 |    0.01 |    8 |         - |          NA |
| NaturalMergeSort         | 1024 | Sorted             |       681.7 ns |     11.18 ns |     4.96 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 1024 | Sorted             |    40,877.6 ns |    917.08 ns |   407.19 ns |  2.35 |    0.02 |   11 |         - |          NA |
| DropMergeSort            | 1024 | Sorted             |     2,205.4 ns |      6.95 ns |     3.64 ns |  0.13 |    0.00 |    4 |         - |          NA |
| TimSort                  | 1024 | Sorted             |       629.9 ns |     17.09 ns |     7.59 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Sorted             |       629.3 ns |     18.32 ns |     9.58 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Sorted             |       950.4 ns |    327.54 ns |   171.31 ns |  0.05 |    0.01 |    2 |         - |          NA |
| SpinSort                 | 1024 | Sorted             |       557.3 ns |     12.50 ns |     5.55 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Sorted             |       554.1 ns |      9.93 ns |     5.19 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | Sorted             |       660.1 ns |     17.72 ns |     7.87 ns |  0.04 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Sorted             |     5,334.1 ns |    404.03 ns |   211.31 ns |  0.31 |    0.01 |    7 |         - |          NA |
| QuadSort                 | 1024 | Sorted             |       909.5 ns |     26.97 ns |     9.62 ns |  0.05 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 1024 | Sorted             |    15,740.9 ns |    173.75 ns |    90.87 ns |  0.91 |    0.01 |    9 |         - |          NA |
| DotnetSort               | 1024 | Sorted             |     4,066.6 ns |    113.41 ns |    50.35 ns |  0.23 |    0.00 |    6 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **1024** | **Reversed**           |    **36,836.3 ns** |    **200.03 ns** |   **104.62 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Reversed           |    32,981.7 ns |     65.96 ns |    29.29 ns |  0.90 |    0.00 |    4 |         - |          NA |
| BottomupMergeSort        | 1024 | Reversed           |    19,862.0 ns |    311.57 ns |   162.96 ns |  0.54 |    0.00 |    3 |         - |          NA |
| StdStableSort            | 1024 | Reversed           |    17,638.2 ns |    156.96 ns |    82.09 ns |  0.48 |    0.00 |    3 |         - |          NA |
| RotateMergeSort          | 1024 | Reversed           |    27,529.2 ns |    273.99 ns |   121.65 ns |  0.75 |    0.00 |    4 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Reversed           |    36,141.0 ns |    298.22 ns |   132.41 ns |  0.98 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 1024 | Reversed           |    10,991.6 ns |    363.58 ns |   190.16 ns |  0.30 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 1024 | Reversed           |    15,059.0 ns |     76.48 ns |    33.96 ns |  0.41 |    0.00 |    3 |         - |          NA |
| NaturalMergeSort         | 1024 | Reversed           |       995.5 ns |     15.19 ns |     7.95 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 1024 | Reversed           |     6,184.5 ns |    339.67 ns |   177.66 ns |  0.17 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 1024 | Reversed           |     7,050.1 ns |    369.26 ns |   193.13 ns |  0.19 |    0.00 |    2 |         - |          NA |
| TimSort                  | 1024 | Reversed           |       947.2 ns |      7.94 ns |     3.53 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Reversed           |       928.7 ns |     11.66 ns |     5.18 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Reversed           |    18,801.7 ns |    207.46 ns |   108.50 ns |  0.51 |    0.00 |    3 |         - |          NA |
| SpinSort                 | 1024 | Reversed           |     1,012.9 ns |     15.79 ns |     8.26 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Reversed           |     1,028.2 ns |     63.14 ns |    33.03 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | Reversed           |     1,000.5 ns |     12.67 ns |     5.63 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Reversed           |     9,273.2 ns |    603.87 ns |   315.83 ns |  0.25 |    0.01 |    2 |         - |          NA |
| QuadSort                 | 1024 | Reversed           |     1,033.6 ns |     22.35 ns |     9.92 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SimdMergeSort            | 1024 | Reversed           |    14,276.2 ns |    265.94 ns |   139.09 ns |  0.39 |    0.00 |    3 |         - |          NA |
| DotnetSort               | 1024 | Reversed           |     8,113.3 ns |    923.89 ns |   483.21 ns |  0.22 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **1024** | **PipeOrgan**          |    **27,733.2 ns** |    **262.16 ns** |   **137.11 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | PipeOrgan          |    27,562.2 ns |    613.84 ns |   272.55 ns |  0.99 |    0.01 |    7 |         - |          NA |
| BottomupMergeSort        | 1024 | PipeOrgan          |    13,446.2 ns |    316.93 ns |   165.76 ns |  0.48 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 1024 | PipeOrgan          |    15,019.6 ns |    225.62 ns |   118.00 ns |  0.54 |    0.00 |    6 |         - |          NA |
| RotateMergeSort          | 1024 | PipeOrgan          |    25,637.2 ns |    549.06 ns |   243.79 ns |  0.92 |    0.01 |    7 |         - |          NA |
| RotateMergeSortRecursive | 1024 | PipeOrgan          |    32,326.2 ns |    376.06 ns |   196.69 ns |  1.17 |    0.01 |    7 |         - |          NA |
| SymMergeSort             | 1024 | PipeOrgan          |    15,200.3 ns |    556.78 ns |   247.21 ns |  0.55 |    0.01 |    6 |         - |          NA |
| BlockMergeSort           | 1024 | PipeOrgan          |    15,421.0 ns |    289.96 ns |   151.66 ns |  0.56 |    0.01 |    6 |         - |          NA |
| NaturalMergeSort         | 1024 | PipeOrgan          |     1,801.8 ns |     45.82 ns |    23.96 ns |  0.06 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 1024 | PipeOrgan          |    29,268.3 ns |    663.28 ns |   346.91 ns |  1.06 |    0.01 |    7 |         - |          NA |
| DropMergeSort            | 1024 | PipeOrgan          |    26,479.0 ns |    569.21 ns |   297.71 ns |  0.95 |    0.01 |    7 |         - |          NA |
| TimSort                  | 1024 | PipeOrgan          |     2,700.5 ns |     34.63 ns |    15.38 ns |  0.10 |    0.00 |    2 |         - |          NA |
| PowerSort                | 1024 | PipeOrgan          |     1,749.4 ns |     27.64 ns |    14.46 ns |  0.06 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | PipeOrgan          |    12,059.7 ns |    297.90 ns |   155.81 ns |  0.43 |    0.01 |    6 |         - |          NA |
| SpinSort                 | 1024 | PipeOrgan          |     8,317.6 ns |    379.12 ns |   198.29 ns |  0.30 |    0.01 |    5 |         - |          NA |
| Glidesort                | 1024 | PipeOrgan          |     4,777.5 ns |     57.94 ns |    20.66 ns |  0.17 |    0.00 |    4 |         - |          NA |
| Driftsort                | 1024 | PipeOrgan          |     1,549.8 ns |     15.03 ns |     7.86 ns |  0.06 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | PipeOrgan          |     8,438.8 ns |    436.40 ns |   228.24 ns |  0.30 |    0.01 |    5 |         - |          NA |
| QuadSort                 | 1024 | PipeOrgan          |     3,781.1 ns |    985.42 ns |   515.40 ns |  0.14 |    0.02 |    3 |         - |          NA |
| SimdMergeSort            | 1024 | PipeOrgan          |    14,640.5 ns |    349.39 ns |   155.13 ns |  0.53 |    0.01 |    6 |         - |          NA |
| DotnetSort               | 1024 | PipeOrgan          |    16,878.7 ns |    399.31 ns |   208.85 ns |  0.61 |    0.01 |    6 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **1024** | **ManyDuplicates**     |    **67,638.4 ns** |    **333.51 ns** |   **174.43 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | ManyDuplicates     |    63,702.9 ns |    141.71 ns |    50.54 ns |  0.94 |    0.00 |    3 |         - |          NA |
| BottomupMergeSort        | 1024 | ManyDuplicates     |    49,125.0 ns |    218.85 ns |    97.17 ns |  0.73 |    0.00 |    3 |         - |          NA |
| StdStableSort            | 1024 | ManyDuplicates     |    33,636.4 ns |    285.45 ns |   149.30 ns |  0.50 |    0.00 |    2 |         - |          NA |
| RotateMergeSort          | 1024 | ManyDuplicates     |   122,755.8 ns |  1,662.43 ns |   869.48 ns |  1.81 |    0.01 |    4 |         - |          NA |
| RotateMergeSortRecursive | 1024 | ManyDuplicates     |   144,152.7 ns |    926.23 ns |   411.25 ns |  2.13 |    0.01 |    4 |         - |          NA |
| SymMergeSort             | 1024 | ManyDuplicates     |    80,515.7 ns |    301.61 ns |   107.56 ns |  1.19 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | ManyDuplicates     |    60,218.8 ns |    455.40 ns |   202.20 ns |  0.89 |    0.00 |    3 |         - |          NA |
| NaturalMergeSort         | 1024 | ManyDuplicates     |    49,657.9 ns |    177.24 ns |    78.70 ns |  0.73 |    0.00 |    3 |         - |          NA |
| PatienceSort             | 1024 | ManyDuplicates     |    52,711.1 ns |    172.32 ns |    90.13 ns |  0.78 |    0.00 |    3 |         - |          NA |
| DropMergeSort            | 1024 | ManyDuplicates     |    40,134.7 ns |    258.35 ns |   135.12 ns |  0.59 |    0.00 |    2 |         - |          NA |
| TimSort                  | 1024 | ManyDuplicates     |    54,936.4 ns |    379.40 ns |   135.30 ns |  0.81 |    0.00 |    3 |         - |          NA |
| PowerSort                | 1024 | ManyDuplicates     |    36,509.3 ns |    672.19 ns |   351.57 ns |  0.54 |    0.01 |    2 |         - |          NA |
| ShiftSort                | 1024 | ManyDuplicates     |    55,893.1 ns |    386.65 ns |   202.23 ns |  0.83 |    0.00 |    3 |         - |          NA |
| SpinSort                 | 1024 | ManyDuplicates     |    32,588.7 ns |    417.26 ns |   185.26 ns |  0.48 |    0.00 |    2 |         - |          NA |
| Glidesort                | 1024 | ManyDuplicates     |    22,957.4 ns |    314.94 ns |   112.31 ns |  0.34 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | ManyDuplicates     |    19,768.7 ns |    165.59 ns |    73.52 ns |  0.29 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | ManyDuplicates     |    36,283.4 ns |    352.87 ns |   184.56 ns |  0.54 |    0.00 |    2 |         - |          NA |
| QuadSort                 | 1024 | ManyDuplicates     |    22,959.7 ns |    383.77 ns |   200.72 ns |  0.34 |    0.00 |    1 |         - |          NA |
| SimdMergeSort            | 1024 | ManyDuplicates     |    21,855.8 ns |    226.13 ns |   118.27 ns |  0.32 |    0.00 |    1 |         - |          NA |
| DotnetSort               | 1024 | ManyDuplicates     |    29,965.9 ns |    454.73 ns |   201.90 ns |  0.44 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **4096** | **Random**             |   **344,710.1 ns** |  **1,036.91 ns** |   **460.40 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Random             |   319,420.4 ns |    981.14 ns |   435.63 ns |  0.93 |    0.00 |    3 |         - |          NA |
| BottomupMergeSort        | 4096 | Random             |   261,894.0 ns |    949.57 ns |   421.62 ns |  0.76 |    0.00 |    3 |         - |          NA |
| StdStableSort            | 4096 | Random             |   203,935.6 ns |  1,433.26 ns |   636.37 ns |  0.59 |    0.00 |    3 |         - |          NA |
| RotateMergeSort          | 4096 | Random             |   738,260.4 ns |  3,658.50 ns | 1,913.47 ns |  2.14 |    0.01 |    5 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Random             |   853,662.8 ns |  2,046.09 ns |   908.48 ns |  2.48 |    0.00 |    5 |         - |          NA |
| SymMergeSort             | 4096 | Random             |   499,217.0 ns |  4,722.51 ns | 2,096.82 ns |  1.45 |    0.01 |    4 |         - |          NA |
| BlockMergeSort           | 4096 | Random             |   313,536.6 ns |    655.01 ns |   290.83 ns |  0.91 |    0.00 |    3 |         - |          NA |
| NaturalMergeSort         | 4096 | Random             |   267,300.4 ns |  1,055.30 ns |   468.56 ns |  0.78 |    0.00 |    3 |         - |          NA |
| PatienceSort             | 4096 | Random             |   392,200.1 ns |  2,050.93 ns | 1,072.68 ns |  1.14 |    0.00 |    3 |         - |          NA |
| DropMergeSort            | 4096 | Random             |   256,076.7 ns |  2,773.22 ns | 1,450.45 ns |  0.74 |    0.00 |    3 |         - |          NA |
| TimSort                  | 4096 | Random             |   302,292.3 ns |    990.19 ns |   517.89 ns |  0.88 |    0.00 |    3 |         - |          NA |
| PowerSort                | 4096 | Random             |   226,315.2 ns |  1,514.90 ns |   792.32 ns |  0.66 |    0.00 |    3 |         - |          NA |
| ShiftSort                | 4096 | Random             |   311,750.9 ns |    621.69 ns |   276.03 ns |  0.90 |    0.00 |    3 |         - |          NA |
| SpinSort                 | 4096 | Random             |   201,258.5 ns |  3,648.77 ns | 1,620.08 ns |  0.58 |    0.00 |    3 |         - |          NA |
| Glidesort                | 4096 | Random             |   156,763.9 ns |    828.55 ns |   367.88 ns |  0.45 |    0.00 |    3 |         - |          NA |
| Driftsort                | 4096 | Random             |   138,207.0 ns |    702.02 ns |   311.70 ns |  0.40 |    0.00 |    3 |         - |          NA |
| FlatStableSort           | 4096 | Random             |   211,559.2 ns |  1,434.82 ns |   637.07 ns |  0.61 |    0.00 |    3 |         - |          NA |
| QuadSort                 | 4096 | Random             |   106,675.2 ns |    312.97 ns |   138.96 ns |  0.31 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 4096 | Random             |    73,714.9 ns |  1,167.28 ns |   518.28 ns |  0.21 |    0.00 |    1 |         - |          NA |
| DotnetSort               | 4096 | Random             |   186,564.5 ns |  1,184.43 ns |   619.48 ns |  0.54 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **4096** | **SingleElementMoved** |    **70,908.4 ns** |    **111.89 ns** |    **49.68 ns** |  **1.00** |    **0.00** |   **10** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | SingleElementMoved |    85,993.0 ns |    337.84 ns |   176.70 ns |  1.21 |    0.00 |   11 |         - |          NA |
| BottomupMergeSort        | 4096 | SingleElementMoved |    25,863.1 ns |    184.60 ns |    96.55 ns |  0.36 |    0.00 |    7 |         - |          NA |
| StdStableSort            | 4096 | SingleElementMoved |    16,578.0 ns |    197.31 ns |    70.36 ns |  0.23 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 4096 | SingleElementMoved |     5,179.5 ns |    496.12 ns |   259.48 ns |  0.07 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 4096 | SingleElementMoved |     9,616.5 ns |    293.91 ns |   153.72 ns |  0.14 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 4096 | SingleElementMoved |     4,513.1 ns |    658.85 ns |   344.59 ns |  0.06 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 4096 | SingleElementMoved |    49,616.3 ns |    345.94 ns |   180.94 ns |  0.70 |    0.00 |    9 |         - |          NA |
| NaturalMergeSort         | 4096 | SingleElementMoved |     4,916.1 ns |     51.19 ns |    18.26 ns |  0.07 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 4096 | SingleElementMoved |   337,978.2 ns | 12,896.24 ns | 6,744.98 ns |  4.77 |    0.09 |   12 |         - |          NA |
| DropMergeSort            | 4096 | SingleElementMoved |    10,853.9 ns |    261.92 ns |   136.99 ns |  0.15 |    0.00 |    4 |         - |          NA |
| TimSort                  | 4096 | SingleElementMoved |     3,228.5 ns |    246.62 ns |   109.50 ns |  0.05 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | SingleElementMoved |     5,127.7 ns |    439.42 ns |   229.83 ns |  0.07 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 4096 | SingleElementMoved |     7,300.9 ns |    462.53 ns |   241.91 ns |  0.10 |    0.00 |    3 |         - |          NA |
| SpinSort                 | 4096 | SingleElementMoved |    10,899.9 ns |    348.86 ns |   182.46 ns |  0.15 |    0.00 |    4 |         - |          NA |
| Glidesort                | 4096 | SingleElementMoved |     8,883.8 ns |    348.90 ns |   154.91 ns |  0.13 |    0.00 |    4 |         - |          NA |
| Driftsort                | 4096 | SingleElementMoved |     4,950.3 ns |     72.14 ns |    25.73 ns |  0.07 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 4096 | SingleElementMoved |    21,255.7 ns |     50.66 ns |    18.07 ns |  0.30 |    0.00 |    6 |         - |          NA |
| QuadSort                 | 4096 | SingleElementMoved |     5,066.0 ns |    177.47 ns |    92.82 ns |  0.07 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 4096 | SingleElementMoved |    67,433.9 ns |  1,067.10 ns |   558.11 ns |  0.95 |    0.01 |   10 |         - |          NA |
| DotnetSort               | 4096 | SingleElementMoved |    31,446.0 ns |    307.21 ns |   136.40 ns |  0.44 |    0.00 |    8 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **4096** | **Sorted**             |    **68,433.5 ns** |    **568.97 ns** |   **252.63 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Sorted             |    84,124.1 ns |    462.50 ns |   241.90 ns |  1.23 |    0.01 |    9 |         - |          NA |
| BottomupMergeSort        | 4096 | Sorted             |    23,903.0 ns |    393.56 ns |   140.35 ns |  0.35 |    0.00 |    6 |         - |          NA |
| StdStableSort            | 4096 | Sorted             |    16,570.0 ns |     97.99 ns |    43.51 ns |  0.24 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 4096 | Sorted             |     4,848.7 ns |    235.72 ns |   123.28 ns |  0.07 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Sorted             |     7,994.9 ns |    283.47 ns |   148.26 ns |  0.12 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 4096 | Sorted             |     4,641.2 ns |     45.30 ns |    16.15 ns |  0.07 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 4096 | Sorted             |    43,392.3 ns |    180.76 ns |    80.26 ns |  0.63 |    0.00 |    7 |         - |          NA |
| NaturalMergeSort         | 4096 | Sorted             |     2,390.0 ns |     21.69 ns |     9.63 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 4096 | Sorted             |   223,140.1 ns | 18,937.80 ns | 9,904.84 ns |  3.26 |    0.14 |   10 |         - |          NA |
| DropMergeSort            | 4096 | Sorted             |     8,843.0 ns |    403.37 ns |   210.97 ns |  0.13 |    0.00 |    4 |         - |          NA |
| TimSort                  | 4096 | Sorted             |     2,356.3 ns |     22.36 ns |     9.93 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | Sorted             |     2,319.2 ns |     11.56 ns |     6.05 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | Sorted             |     3,205.0 ns |    550.22 ns |   287.78 ns |  0.05 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 4096 | Sorted             |     2,000.4 ns |     19.65 ns |     8.72 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 4096 | Sorted             |     1,982.2 ns |     42.26 ns |    18.76 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | Sorted             |     2,582.0 ns |    408.85 ns |   213.84 ns |  0.04 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Sorted             |    18,819.1 ns |    346.47 ns |   153.84 ns |  0.28 |    0.00 |    5 |         - |          NA |
| QuadSort                 | 4096 | Sorted             |     3,208.5 ns |     40.62 ns |    18.04 ns |  0.05 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 4096 | Sorted             |    67,727.6 ns |    610.56 ns |   319.34 ns |  0.99 |    0.01 |    8 |         - |          NA |
| DotnetSort               | 4096 | Sorted             |    19,689.2 ns |  1,566.64 ns |   695.60 ns |  0.29 |    0.01 |    5 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **4096** | **Reversed**           |   **150,893.2 ns** |    **911.33 ns** |   **404.64 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Reversed           |   134,191.1 ns |    387.30 ns |   171.97 ns |  0.89 |    0.00 |    5 |         - |          NA |
| BottomupMergeSort        | 4096 | Reversed           |    81,699.6 ns |    293.16 ns |   130.16 ns |  0.54 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 4096 | Reversed           |    74,969.7 ns |    530.87 ns |   235.71 ns |  0.50 |    0.00 |    4 |         - |          NA |
| RotateMergeSort          | 4096 | Reversed           |   124,716.2 ns |    935.50 ns |   415.37 ns |  0.83 |    0.00 |    5 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Reversed           |   159,622.3 ns |    850.18 ns |   377.48 ns |  1.06 |    0.00 |    5 |         - |          NA |
| SymMergeSort             | 4096 | Reversed           |    46,771.9 ns |    150.06 ns |    66.63 ns |  0.31 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 4096 | Reversed           |    67,825.9 ns |    852.58 ns |   445.92 ns |  0.45 |    0.00 |    4 |         - |          NA |
| NaturalMergeSort         | 4096 | Reversed           |     3,751.6 ns |    334.81 ns |   175.11 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 4096 | Reversed           |    23,725.1 ns |    248.75 ns |   110.45 ns |  0.16 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 4096 | Reversed           |    30,709.1 ns |    518.76 ns |   271.32 ns |  0.20 |    0.00 |    3 |         - |          NA |
| TimSort                  | 4096 | Reversed           |     3,598.0 ns |     48.32 ns |    21.45 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | Reversed           |     3,573.4 ns |     69.84 ns |    24.91 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | Reversed           |    84,455.3 ns |    966.88 ns |   505.70 ns |  0.56 |    0.00 |    4 |         - |          NA |
| SpinSort                 | 4096 | Reversed           |     3,854.2 ns |    167.74 ns |    74.48 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 4096 | Reversed           |     3,674.8 ns |    198.01 ns |    87.92 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | Reversed           |     3,684.1 ns |     45.43 ns |    16.20 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Reversed           |    40,477.6 ns |    745.19 ns |   389.75 ns |  0.27 |    0.00 |    3 |         - |          NA |
| QuadSort                 | 4096 | Reversed           |     3,887.6 ns |     44.42 ns |    19.72 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SimdMergeSort            | 4096 | Reversed           |    66,377.7 ns |    765.94 ns |   340.08 ns |  0.44 |    0.00 |    4 |         - |          NA |
| DotnetSort               | 4096 | Reversed           |    35,623.3 ns |  4,360.86 ns | 1,936.25 ns |  0.24 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **4096** | **PipeOrgan**          |   **113,673.7 ns** |  **1,501.06 ns** |   **785.08 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | PipeOrgan          |   110,935.1 ns |    190.32 ns |    84.50 ns |  0.98 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 4096 | PipeOrgan          |    54,930.4 ns |    316.06 ns |   140.33 ns |  0.48 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 4096 | PipeOrgan          |    64,618.9 ns |  1,105.66 ns |   578.28 ns |  0.57 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 4096 | PipeOrgan          |   115,253.4 ns |  3,019.04 ns | 1,579.02 ns |  1.01 |    0.01 |    6 |         - |          NA |
| RotateMergeSortRecursive | 4096 | PipeOrgan          |   139,683.2 ns |    397.45 ns |   176.47 ns |  1.23 |    0.01 |    7 |         - |          NA |
| SymMergeSort             | 4096 | PipeOrgan          |    62,501.5 ns |  1,133.29 ns |   592.73 ns |  0.55 |    0.01 |    5 |         - |          NA |
| BlockMergeSort           | 4096 | PipeOrgan          |    63,303.5 ns |    271.33 ns |   141.91 ns |  0.56 |    0.00 |    5 |         - |          NA |
| NaturalMergeSort         | 4096 | PipeOrgan          |     6,946.9 ns |    361.79 ns |   189.22 ns |  0.06 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 4096 | PipeOrgan          |   171,728.5 ns | 12,035.93 ns | 6,295.02 ns |  1.51 |    0.05 |    8 |         - |          NA |
| DropMergeSort            | 4096 | PipeOrgan          |   110,631.8 ns |  1,926.73 ns |   855.48 ns |  0.97 |    0.01 |    6 |         - |          NA |
| TimSort                  | 4096 | PipeOrgan          |    10,415.3 ns |    474.19 ns |   248.01 ns |  0.09 |    0.00 |    2 |         - |          NA |
| PowerSort                | 4096 | PipeOrgan          |     6,734.0 ns |    246.93 ns |   109.64 ns |  0.06 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | PipeOrgan          |    52,586.2 ns |    221.72 ns |    98.45 ns |  0.46 |    0.00 |    5 |         - |          NA |
| SpinSort                 | 4096 | PipeOrgan          |     9,195.5 ns |    315.68 ns |   140.16 ns |  0.08 |    0.00 |    2 |         - |          NA |
| Glidesort                | 4096 | PipeOrgan          |    18,993.9 ns |    228.89 ns |   101.63 ns |  0.17 |    0.00 |    3 |         - |          NA |
| Driftsort                | 4096 | PipeOrgan          |     5,974.0 ns |    366.73 ns |   191.81 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | PipeOrgan          |    36,561.8 ns |    290.17 ns |   128.84 ns |  0.32 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 4096 | PipeOrgan          |    11,977.4 ns |    346.01 ns |   180.97 ns |  0.11 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 4096 | PipeOrgan          |    69,292.0 ns |  1,302.82 ns |   681.40 ns |  0.61 |    0.01 |    5 |         - |          NA |
| DotnetSort               | 4096 | PipeOrgan          |    92,869.7 ns |  3,041.23 ns | 1,590.62 ns |  0.82 |    0.01 |    6 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **4096** | **ManyDuplicates**     |   **278,776.2 ns** |  **1,620.45 ns** |   **719.49 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | ManyDuplicates     |   262,340.4 ns |  1,338.42 ns |   594.27 ns |  0.94 |    0.00 |    4 |         - |          NA |
| BottomupMergeSort        | 4096 | ManyDuplicates     |   202,987.9 ns |  1,300.61 ns |   577.48 ns |  0.73 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 4096 | ManyDuplicates     |   143,757.1 ns |    807.53 ns |   358.55 ns |  0.52 |    0.00 |    4 |         - |          NA |
| RotateMergeSort          | 4096 | ManyDuplicates     |   550,286.3 ns |  3,108.75 ns | 1,625.94 ns |  1.97 |    0.01 |    6 |         - |          NA |
| RotateMergeSortRecursive | 4096 | ManyDuplicates     |   648,537.6 ns |  1,935.55 ns |   859.39 ns |  2.33 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 4096 | ManyDuplicates     |   370,109.6 ns |  1,656.24 ns |   866.25 ns |  1.33 |    0.00 |    5 |         - |          NA |
| BlockMergeSort           | 4096 | ManyDuplicates     |   275,754.6 ns |  1,405.91 ns |   624.23 ns |  0.99 |    0.00 |    4 |         - |          NA |
| NaturalMergeSort         | 4096 | ManyDuplicates     |   202,948.7 ns |    805.63 ns |   421.36 ns |  0.73 |    0.00 |    4 |         - |          NA |
| PatienceSort             | 4096 | ManyDuplicates     |   180,276.6 ns |    853.43 ns |   378.93 ns |  0.65 |    0.00 |    4 |         - |          NA |
| DropMergeSort            | 4096 | ManyDuplicates     |   152,608.7 ns |  1,032.32 ns |   458.36 ns |  0.55 |    0.00 |    4 |         - |          NA |
| TimSort                  | 4096 | ManyDuplicates     |   231,359.0 ns |  1,630.54 ns |   852.80 ns |  0.83 |    0.00 |    4 |         - |          NA |
| PowerSort                | 4096 | ManyDuplicates     |   154,404.5 ns |  1,442.30 ns |   640.39 ns |  0.55 |    0.00 |    4 |         - |          NA |
| ShiftSort                | 4096 | ManyDuplicates     |   237,088.8 ns |  1,432.68 ns |   749.32 ns |  0.85 |    0.00 |    4 |         - |          NA |
| SpinSort                 | 4096 | ManyDuplicates     |   140,461.8 ns |  1,327.66 ns |   589.49 ns |  0.50 |    0.00 |    4 |         - |          NA |
| Glidesort                | 4096 | ManyDuplicates     |    56,142.7 ns |  1,248.86 ns |   653.18 ns |  0.20 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | ManyDuplicates     |    47,852.3 ns |    120.04 ns |    53.30 ns |  0.17 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | ManyDuplicates     |   147,564.9 ns |  1,383.49 ns |   614.28 ns |  0.53 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 4096 | ManyDuplicates     |    98,483.3 ns |    659.63 ns |   235.23 ns |  0.35 |    0.00 |    3 |         - |          NA |
| SimdMergeSort            | 4096 | ManyDuplicates     |    74,842.1 ns |    163.75 ns |    72.71 ns |  0.27 |    0.00 |    2 |         - |          NA |
| DotnetSort               | 4096 | ManyDuplicates     |   122,104.6 ns |  1,352.19 ns |   707.22 ns |  0.44 |    0.00 |    4 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **8192** | **Random**             |   **730,289.2 ns** |  **2,076.57 ns** |   **922.01 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Random             |   674,665.5 ns |    882.45 ns |   391.82 ns |  0.92 |    0.00 |    4 |         - |          NA |
| BottomupMergeSort        | 8192 | Random             |   562,188.5 ns |  2,993.24 ns | 1,565.52 ns |  0.77 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 8192 | Random             |   496,870.6 ns |  2,282.18 ns |   813.85 ns |  0.68 |    0.00 |    4 |         - |          NA |
| RotateMergeSort          | 8192 | Random             | 1,662,896.6 ns |  7,841.86 ns | 4,101.45 ns |  2.28 |    0.01 |    6 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Random             | 1,910,333.1 ns |  5,851.57 ns | 2,598.13 ns |  2.62 |    0.00 |    6 |         - |          NA |
| SymMergeSort             | 8192 | Random             | 1,121,754.2 ns |  2,446.38 ns | 1,086.21 ns |  1.54 |    0.00 |    5 |         - |          NA |
| BlockMergeSort           | 8192 | Random             |   679,300.7 ns |  1,376.44 ns |   719.90 ns |  0.93 |    0.00 |    4 |         - |          NA |
| NaturalMergeSort         | 8192 | Random             |   572,315.9 ns |    950.40 ns |   421.98 ns |  0.78 |    0.00 |    4 |         - |          NA |
| PatienceSort             | 8192 | Random             |   849,368.9 ns |  3,623.53 ns | 1,895.17 ns |  1.16 |    0.00 |    4 |         - |          NA |
| DropMergeSort            | 8192 | Random             |   544,055.2 ns |  3,064.83 ns | 1,360.80 ns |  0.74 |    0.00 |    4 |         - |          NA |
| TimSort                  | 8192 | Random             |   663,294.1 ns |  2,384.34 ns | 1,058.66 ns |  0.91 |    0.00 |    4 |         - |          NA |
| PowerSort                | 8192 | Random             |   495,295.8 ns |  1,314.80 ns |   583.78 ns |  0.68 |    0.00 |    4 |         - |          NA |
| ShiftSort                | 8192 | Random             |   682,867.3 ns |  1,418.08 ns |   741.68 ns |  0.94 |    0.00 |    4 |         - |          NA |
| SpinSort                 | 8192 | Random             |   439,145.4 ns |  1,276.26 ns |   667.51 ns |  0.60 |    0.00 |    4 |         - |          NA |
| Glidesort                | 8192 | Random             |   323,302.1 ns |  2,056.23 ns | 1,075.45 ns |  0.44 |    0.00 |    3 |         - |          NA |
| Driftsort                | 8192 | Random             |   286,798.8 ns |  1,290.01 ns |   572.77 ns |  0.39 |    0.00 |    3 |         - |          NA |
| FlatStableSort           | 8192 | Random             |   475,875.0 ns |  1,410.18 ns |   626.13 ns |  0.65 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 8192 | Random             |   227,801.3 ns |    427.34 ns |   189.74 ns |  0.31 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 8192 | Random             |   173,480.3 ns |  2,238.85 ns | 1,170.96 ns |  0.24 |    0.00 |    1 |         - |          NA |
| DotnetSort               | 8192 | Random             |   404,954.8 ns |  3,993.00 ns | 2,088.42 ns |  0.55 |    0.00 |    4 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **8192** | **SingleElementMoved** |   **141,942.2 ns** |    **670.93 ns** |   **297.90 ns** |  **1.00** |    **0.00** |    **9** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | SingleElementMoved |   173,377.5 ns |    849.79 ns |   377.31 ns |  1.22 |    0.00 |    9 |         - |          NA |
| BottomupMergeSort        | 8192 | SingleElementMoved |    54,682.8 ns |    546.94 ns |   286.06 ns |  0.39 |    0.00 |    6 |         - |          NA |
| StdStableSort            | 8192 | SingleElementMoved |    31,753.5 ns |    443.30 ns |   196.83 ns |  0.22 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 8192 | SingleElementMoved |     9,381.8 ns |    631.32 ns |   280.31 ns |  0.07 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 8192 | SingleElementMoved |    18,665.6 ns |    321.35 ns |   168.07 ns |  0.13 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 8192 | SingleElementMoved |     8,431.7 ns |    382.92 ns |   200.28 ns |  0.06 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 8192 | SingleElementMoved |    97,139.6 ns |    484.72 ns |   253.52 ns |  0.68 |    0.00 |    8 |         - |          NA |
| NaturalMergeSort         | 8192 | SingleElementMoved |    10,017.2 ns |    670.04 ns |   350.44 ns |  0.07 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 8192 | SingleElementMoved |   790,530.4 ns |  4,532.26 ns | 2,370.46 ns |  5.57 |    0.02 |   10 |         - |          NA |
| DropMergeSort            | 8192 | SingleElementMoved |    21,738.4 ns |    248.87 ns |   110.50 ns |  0.15 |    0.00 |    4 |         - |          NA |
| TimSort                  | 8192 | SingleElementMoved |     6,282.0 ns |    364.00 ns |   190.38 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | SingleElementMoved |    10,030.9 ns |    522.78 ns |   232.12 ns |  0.07 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 8192 | SingleElementMoved |    14,270.6 ns |    226.38 ns |   100.51 ns |  0.10 |    0.00 |    3 |         - |          NA |
| SpinSort                 | 8192 | SingleElementMoved |    18,496.7 ns |    546.88 ns |   242.82 ns |  0.13 |    0.00 |    4 |         - |          NA |
| Glidesort                | 8192 | SingleElementMoved |    17,458.6 ns |    131.73 ns |    58.49 ns |  0.12 |    0.00 |    4 |         - |          NA |
| Driftsort                | 8192 | SingleElementMoved |     9,642.7 ns |    486.22 ns |   215.88 ns |  0.07 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 8192 | SingleElementMoved |    30,102.0 ns |    372.20 ns |   194.67 ns |  0.21 |    0.00 |    5 |         - |          NA |
| QuadSort                 | 8192 | SingleElementMoved |     9,823.2 ns |    318.54 ns |   166.60 ns |  0.07 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 8192 | SingleElementMoved |   166,956.1 ns |    786.29 ns |   349.12 ns |  1.18 |    0.00 |    9 |         - |          NA |
| DotnetSort               | 8192 | SingleElementMoved |    70,260.9 ns |  2,614.83 ns | 1,367.60 ns |  0.49 |    0.01 |    7 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **8192** | **Sorted**             |   **137,525.9 ns** |  **1,603.09 ns** |   **711.78 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Sorted             |   168,780.8 ns |    211.31 ns |    93.82 ns |  1.23 |    0.01 |    8 |         - |          NA |
| BottomupMergeSort        | 8192 | Sorted             |    50,513.0 ns |    150.14 ns |    66.66 ns |  0.37 |    0.00 |    6 |         - |          NA |
| StdStableSort            | 8192 | Sorted             |    31,431.6 ns |     73.33 ns |    38.35 ns |  0.23 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 8192 | Sorted             |     9,864.8 ns |    575.10 ns |   205.09 ns |  0.07 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Sorted             |    16,239.6 ns |    699.02 ns |   365.60 ns |  0.12 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 8192 | Sorted             |     9,591.8 ns |    391.06 ns |   204.53 ns |  0.07 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 8192 | Sorted             |    85,835.4 ns |    642.29 ns |   285.18 ns |  0.62 |    0.00 |    7 |         - |          NA |
| NaturalMergeSort         | 8192 | Sorted             |     4,746.5 ns |    377.78 ns |   167.74 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 8192 | Sorted             |   786,654.3 ns |  3,206.75 ns | 1,423.82 ns |  5.72 |    0.03 |    9 |         - |          NA |
| DropMergeSort            | 8192 | Sorted             |    17,555.9 ns |    314.41 ns |   164.44 ns |  0.13 |    0.00 |    4 |         - |          NA |
| TimSort                  | 8192 | Sorted             |     4,603.7 ns |     82.65 ns |    29.47 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Sorted             |     4,583.9 ns |     91.15 ns |    32.50 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Sorted             |     5,995.3 ns |    428.49 ns |   224.11 ns |  0.04 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | Sorted             |     4,048.7 ns |     84.72 ns |    30.21 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Sorted             |     3,985.7 ns |    295.63 ns |   131.26 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Sorted             |     4,739.1 ns |    261.14 ns |   115.95 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Sorted             |     4,195.1 ns |    259.02 ns |   115.01 ns |  0.03 |    0.00 |    1 |         - |          NA |
| QuadSort                 | 8192 | Sorted             |     6,388.8 ns |    258.35 ns |   135.12 ns |  0.05 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 8192 | Sorted             |   144,741.2 ns |    333.98 ns |   148.29 ns |  1.05 |    0.01 |    8 |         - |          NA |
| DotnetSort               | 8192 | Sorted             |    44,711.8 ns |  6,840.90 ns | 3,577.92 ns |  0.33 |    0.02 |    6 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **8192** | **Reversed**           |   **304,474.4 ns** |  **1,216.30 ns** |   **540.05 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Reversed           |   272,098.4 ns |  1,331.81 ns |   591.33 ns |  0.89 |    0.00 |    7 |         - |          NA |
| BottomupMergeSort        | 8192 | Reversed           |   167,402.5 ns |    344.45 ns |   152.94 ns |  0.55 |    0.00 |    6 |         - |          NA |
| StdStableSort            | 8192 | Reversed           |   267,666.2 ns |  2,527.64 ns | 1,322.00 ns |  0.88 |    0.00 |    7 |         - |          NA |
| RotateMergeSort          | 8192 | Reversed           |   263,788.5 ns |    919.59 ns |   408.30 ns |  0.87 |    0.00 |    7 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Reversed           |   334,399.7 ns |  3,752.37 ns | 1,666.08 ns |  1.10 |    0.01 |    7 |         - |          NA |
| SymMergeSort             | 8192 | Reversed           |    95,729.8 ns |    386.38 ns |   171.56 ns |  0.31 |    0.00 |    5 |         - |          NA |
| BlockMergeSort           | 8192 | Reversed           |   141,276.4 ns |  2,084.95 ns | 1,090.47 ns |  0.46 |    0.00 |    6 |         - |          NA |
| NaturalMergeSort         | 8192 | Reversed           |     7,308.9 ns |    371.01 ns |   164.73 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 8192 | Reversed           |    47,350.9 ns |    213.09 ns |    94.61 ns |  0.16 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 8192 | Reversed           |    64,291.6 ns |    214.23 ns |    95.12 ns |  0.21 |    0.00 |    3 |         - |          NA |
| TimSort                  | 8192 | Reversed           |     7,301.9 ns |    372.62 ns |   194.89 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Reversed           |     7,362.8 ns |    462.63 ns |   241.97 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Reversed           |   185,113.2 ns |  1,612.79 ns |   716.09 ns |  0.61 |    0.00 |    6 |         - |          NA |
| SpinSort                 | 8192 | Reversed           |     7,778.6 ns |    343.31 ns |   179.56 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Reversed           |     7,320.2 ns |    364.21 ns |   190.49 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Reversed           |     7,321.5 ns |    400.61 ns |   209.53 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Reversed           |     8,263.1 ns |  1,386.32 ns |   615.53 ns |  0.03 |    0.00 |    1 |         - |          NA |
| QuadSort                 | 8192 | Reversed           |     7,817.1 ns |    306.49 ns |   160.30 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SimdMergeSort            | 8192 | Reversed           |   159,368.1 ns |  1,648.34 ns |   862.12 ns |  0.52 |    0.00 |    6 |         - |          NA |
| DotnetSort               | 8192 | Reversed           |    78,735.9 ns |  6,903.63 ns | 3,610.73 ns |  0.26 |    0.01 |    4 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **8192** | **PipeOrgan**          |   **229,462.0 ns** |  **2,618.57 ns** | **1,369.56 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | PipeOrgan          |   225,383.9 ns |  4,907.99 ns | 2,179.18 ns |  0.98 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 8192 | PipeOrgan          |   112,993.2 ns |  1,126.70 ns |   500.26 ns |  0.49 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 8192 | PipeOrgan          |   222,854.5 ns |  2,619.29 ns |   934.06 ns |  0.97 |    0.01 |    6 |         - |          NA |
| RotateMergeSort          | 8192 | PipeOrgan          |   239,568.3 ns |  1,615.90 ns |   845.15 ns |  1.04 |    0.01 |    6 |         - |          NA |
| RotateMergeSortRecursive | 8192 | PipeOrgan          |   289,099.8 ns |    260.76 ns |    92.99 ns |  1.26 |    0.01 |    7 |         - |          NA |
| SymMergeSort             | 8192 | PipeOrgan          |   127,175.5 ns |  1,730.97 ns |   905.33 ns |  0.55 |    0.00 |    5 |         - |          NA |
| BlockMergeSort           | 8192 | PipeOrgan          |   129,381.3 ns |    307.78 ns |   136.66 ns |  0.56 |    0.00 |    5 |         - |          NA |
| NaturalMergeSort         | 8192 | PipeOrgan          |    13,681.8 ns |    523.37 ns |   273.73 ns |  0.06 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 8192 | PipeOrgan          |   661,644.4 ns |  3,844.63 ns | 2,010.81 ns |  2.88 |    0.02 |    9 |         - |          NA |
| DropMergeSort            | 8192 | PipeOrgan          |   224,736.2 ns |  6,728.93 ns | 2,987.69 ns |  0.98 |    0.01 |    6 |         - |          NA |
| TimSort                  | 8192 | PipeOrgan          |    20,809.5 ns |    607.11 ns |   317.53 ns |  0.09 |    0.00 |    2 |         - |          NA |
| PowerSort                | 8192 | PipeOrgan          |    13,239.0 ns |    279.83 ns |   124.25 ns |  0.06 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | PipeOrgan          |   106,010.5 ns |  1,600.09 ns |   836.88 ns |  0.46 |    0.00 |    5 |         - |          NA |
| SpinSort                 | 8192 | PipeOrgan          |    18,434.3 ns |    429.87 ns |   224.83 ns |  0.08 |    0.00 |    2 |         - |          NA |
| Glidesort                | 8192 | PipeOrgan          |    38,011.7 ns |    523.98 ns |   232.65 ns |  0.17 |    0.00 |    3 |         - |          NA |
| Driftsort                | 8192 | PipeOrgan          |    11,662.5 ns |    407.84 ns |   213.31 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | PipeOrgan          |    66,244.8 ns |    906.13 ns |   402.33 ns |  0.29 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 8192 | PipeOrgan          |    24,195.3 ns |    271.57 ns |   120.58 ns |  0.11 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 8192 | PipeOrgan          |   141,028.2 ns |  1,217.13 ns |   636.58 ns |  0.61 |    0.00 |    5 |         - |          NA |
| DotnetSort               | 8192 | PipeOrgan          |   363,841.4 ns |  5,695.85 ns | 2,979.04 ns |  1.59 |    0.02 |    8 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **8192** | **ManyDuplicates**     |   **565,744.5 ns** |  **5,726.65 ns** | **2,542.67 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | ManyDuplicates     |   538,762.5 ns | 10,827.51 ns | 5,663.00 ns |  0.95 |    0.01 |    4 |         - |          NA |
| BottomupMergeSort        | 8192 | ManyDuplicates     |   411,612.9 ns |  1,684.43 ns |   880.99 ns |  0.73 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 8192 | ManyDuplicates     |   347,478.0 ns |  2,074.31 ns |   921.01 ns |  0.61 |    0.00 |    4 |         - |          NA |
| RotateMergeSort          | 8192 | ManyDuplicates     | 1,143,114.3 ns |  4,510.33 ns | 1,608.43 ns |  2.02 |    0.01 |    6 |         - |          NA |
| RotateMergeSortRecursive | 8192 | ManyDuplicates     | 1,344,181.0 ns |  2,078.10 ns | 1,086.89 ns |  2.38 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 8192 | ManyDuplicates     |   774,088.1 ns |  1,954.31 ns | 1,022.14 ns |  1.37 |    0.01 |    5 |         - |          NA |
| BlockMergeSort           | 8192 | ManyDuplicates     |   565,314.6 ns |  2,357.78 ns | 1,233.16 ns |  1.00 |    0.00 |    4 |         - |          NA |
| NaturalMergeSort         | 8192 | ManyDuplicates     |   407,764.8 ns |    847.10 ns |   376.12 ns |  0.72 |    0.00 |    4 |         - |          NA |
| PatienceSort             | 8192 | ManyDuplicates     |   362,273.1 ns |  2,634.58 ns | 1,169.77 ns |  0.64 |    0.00 |    4 |         - |          NA |
| DropMergeSort            | 8192 | ManyDuplicates     |   302,204.1 ns |  1,687.81 ns |   882.76 ns |  0.53 |    0.00 |    4 |         - |          NA |
| TimSort                  | 8192 | ManyDuplicates     |   467,795.8 ns |  1,882.09 ns |   984.37 ns |  0.83 |    0.00 |    4 |         - |          NA |
| PowerSort                | 8192 | ManyDuplicates     |   318,031.1 ns |  2,516.48 ns | 1,316.17 ns |  0.56 |    0.00 |    4 |         - |          NA |
| ShiftSort                | 8192 | ManyDuplicates     |   495,151.0 ns |  1,994.29 ns | 1,043.05 ns |  0.88 |    0.00 |    4 |         - |          NA |
| SpinSort                 | 8192 | ManyDuplicates     |   287,890.9 ns |  3,090.52 ns | 1,372.21 ns |  0.51 |    0.00 |    4 |         - |          NA |
| Glidesort                | 8192 | ManyDuplicates     |   102,012.9 ns |  1,656.75 ns |   866.51 ns |  0.18 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | ManyDuplicates     |    87,635.3 ns |  1,549.37 ns |   687.93 ns |  0.15 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | ManyDuplicates     |   329,853.0 ns |  1,660.78 ns |   868.62 ns |  0.58 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 8192 | ManyDuplicates     |   201,429.6 ns |    773.06 ns |   343.25 ns |  0.36 |    0.00 |    3 |         - |          NA |
| SimdMergeSort            | 8192 | ManyDuplicates     |   163,388.1 ns |    729.78 ns |   260.25 ns |  0.29 |    0.00 |    2 |         - |          NA |
| DotnetSort               | 8192 | ManyDuplicates     |   245,583.3 ns |  1,574.48 ns |   823.48 ns |  0.43 |    0.00 |    4 |         - |          NA |

### MergeSlowBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.86GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.22 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method     | Size | Pattern            | Mean         | Error       | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------- |----- |------------------- |-------------:|------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **StrandSort** | **256**  | **Random**             |  **12,001.6 ns** |   **271.98 ns** |   **142.25 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DotnetSort | 256  | Random             |   7,598.4 ns |   344.46 ns |   180.16 ns |  0.63 |    0.02 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **256**  | **SingleElementMoved** |     **883.8 ns** |    **27.57 ns** |    **14.42 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DotnetSort | 256  | SingleElementMoved |   1,380.9 ns |   142.76 ns |    74.67 ns |  1.56 |    0.08 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **256**  | **Sorted**             |     **606.8 ns** |    **11.19 ns** |     **4.97 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DotnetSort | 256  | Sorted             |     812.3 ns |    18.72 ns |     8.31 ns |  1.34 |    0.02 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **256**  | **Reversed**           |  **50,508.8 ns** | **2,959.62 ns** | **1,547.94 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DotnetSort | 256  | Reversed           |   1,507.3 ns |   564.40 ns |   295.19 ns |  0.03 |    0.01 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **256**  | **PipeOrgan**          |  **29,689.7 ns** |   **320.44 ns** |   **142.28 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DotnetSort | 256  | PipeOrgan          |   2,484.1 ns |   367.72 ns |   192.32 ns |  0.08 |    0.01 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **256**  | **ManyDuplicates**     |   **9,996.3 ns** |   **407.86 ns** |   **213.32 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DotnetSort | 256  | ManyDuplicates     |   7,141.0 ns |   426.92 ns |   223.28 ns |  0.71 |    0.03 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **1024** | **Random**             |  **74,036.1 ns** |   **333.93 ns** |   **119.08 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DotnetSort | 1024 | Random             |  38,199.0 ns |   331.35 ns |   147.12 ns |  0.52 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **1024** | **SingleElementMoved** |   **2,833.3 ns** |   **303.73 ns** |   **158.86 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DotnetSort | 1024 | SingleElementMoved |   6,868.7 ns |   257.33 ns |   134.59 ns |  2.43 |    0.13 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **1024** | **Sorted**             |   **1,836.1 ns** |    **98.48 ns** |    **51.51 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DotnetSort | 1024 | Sorted             |   4,256.0 ns |   553.04 ns |   289.25 ns |  2.32 |    0.16 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **1024** | **Reversed**           | **805,342.4 ns** | **4,012.36 ns** | **1,430.85 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DotnetSort | 1024 | Reversed           |   8,263.6 ns |   549.18 ns |   243.84 ns |  0.01 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **1024** | **PipeOrgan**          | **481,992.1 ns** | **2,896.67 ns** | **1,286.14 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DotnetSort | 1024 | PipeOrgan          |  16,719.2 ns |   457.67 ns |   239.37 ns |  0.03 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **1024** | **ManyDuplicates**     |  **45,014.9 ns** |   **363.82 ns** |   **190.29 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DotnetSort | 1024 | ManyDuplicates     |  30,028.9 ns |   398.06 ns |   208.19 ns |  0.67 |    0.01 |    1 |         - |          NA |

### NetworkBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.86GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.22 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                            | Size | Pattern            | Mean           | Error       | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ---------------------------------- |----- |------------------- |---------------:|------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **BitonicSort**                       | **256**  | **Random**             |    **16,469.8 ns** |   **133.32 ns** |    **47.54 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| BitonicSortBranchless             | 256  | Random             |     5,015.6 ns |   224.36 ns |   117.35 ns |  0.30 |    0.01 |    2 |         - |          NA |
| BitonicRecursiveSort              | 256  | Random             |    33,815.4 ns |   674.36 ns |   352.70 ns |  2.05 |    0.02 |    6 |         - |          NA |
| BitonicSortAnyLength              | 256  | Random             |    26,393.1 ns |   532.64 ns |   236.50 ns |  1.60 |    0.01 |    5 |         - |          NA |
| BitonicSortAnyLengthSimd          | 256  | Random             |     3,265.7 ns |    15.57 ns |     6.91 ns |  0.20 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 256  | Random             |    13,668.1 ns |   319.68 ns |   167.20 ns |  0.83 |    0.01 |    4 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 256  | Random             |     4,352.7 ns |    58.49 ns |    25.97 ns |  0.26 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 256  | Random             |    14,797.9 ns |   243.05 ns |   107.92 ns |  0.90 |    0.01 |    4 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 256  | Random             |     4,303.6 ns |    25.18 ns |    11.18 ns |  0.26 |    0.00 |    2 |         - |          NA |
| DotnetSort                        | 256  | Random             |     7,585.7 ns |   425.69 ns |   222.64 ns |  0.46 |    0.01 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BitonicSort**                       | **256**  | **SingleElementMoved** |     **4,775.0 ns** |    **94.93 ns** |    **42.15 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 256  | SingleElementMoved |     4,652.0 ns |   100.98 ns |    52.81 ns |  0.97 |    0.01 |    2 |         - |          NA |
| BitonicRecursiveSort              | 256  | SingleElementMoved |    23,492.3 ns |   334.25 ns |   174.82 ns |  4.92 |    0.05 |    4 |         - |          NA |
| BitonicSortAnyLength              | 256  | SingleElementMoved |    15,206.8 ns |   122.21 ns |    54.26 ns |  3.18 |    0.03 |    3 |         - |          NA |
| BitonicSortAnyLengthSimd          | 256  | SingleElementMoved |     3,448.5 ns |   328.32 ns |   171.72 ns |  0.72 |    0.03 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 256  | SingleElementMoved |     5,022.8 ns |   436.97 ns |   228.55 ns |  1.05 |    0.05 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 256  | SingleElementMoved |     4,472.6 ns |   361.33 ns |   188.98 ns |  0.94 |    0.04 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 256  | SingleElementMoved |     3,847.0 ns |   323.91 ns |   169.41 ns |  0.81 |    0.03 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 256  | SingleElementMoved |     4,252.4 ns |    39.10 ns |    13.94 ns |  0.89 |    0.01 |    2 |         - |          NA |
| DotnetSort                        | 256  | SingleElementMoved |     1,352.2 ns |   138.62 ns |    61.55 ns |  0.28 |    0.01 |    1 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BitonicSort**                       | **256**  | **Sorted**             |     **5,028.8 ns** |   **454.71 ns** |   **237.82 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 256  | Sorted             |     4,792.9 ns |   183.88 ns |    81.64 ns |  0.95 |    0.04 |    2 |         - |          NA |
| BitonicRecursiveSort              | 256  | Sorted             |    23,191.8 ns |   133.03 ns |    59.07 ns |  4.62 |    0.20 |    4 |         - |          NA |
| BitonicSortAnyLength              | 256  | Sorted             |    14,812.2 ns |   233.71 ns |   122.24 ns |  2.95 |    0.13 |    3 |         - |          NA |
| BitonicSortAnyLengthSimd          | 256  | Sorted             |     3,424.7 ns |   328.34 ns |   171.73 ns |  0.68 |    0.04 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 256  | Sorted             |     4,746.9 ns |     7.20 ns |     3.20 ns |  0.95 |    0.04 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 256  | Sorted             |     4,331.1 ns |    55.08 ns |    24.46 ns |  0.86 |    0.04 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 256  | Sorted             |     4,005.3 ns |   373.24 ns |   195.21 ns |  0.80 |    0.05 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 256  | Sorted             |     4,298.1 ns |    33.07 ns |    11.79 ns |  0.86 |    0.04 |    2 |         - |          NA |
| DotnetSort                        | 256  | Sorted             |       814.4 ns |    12.04 ns |     6.30 ns |  0.16 |    0.01 |    1 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BitonicSort**                       | **256**  | **Reversed**           |     **4,581.9 ns** |    **63.53 ns** |    **33.23 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 256  | Reversed           |     4,641.3 ns |    28.22 ns |    14.76 ns |  1.01 |    0.01 |    3 |         - |          NA |
| BitonicRecursiveSort              | 256  | Reversed           |    23,145.2 ns |    64.75 ns |    23.09 ns |  5.05 |    0.04 |    5 |         - |          NA |
| BitonicSortAnyLength              | 256  | Reversed           |    15,016.5 ns |   254.25 ns |   132.98 ns |  3.28 |    0.04 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 256  | Reversed           |     3,388.1 ns |   285.38 ns |   149.26 ns |  0.74 |    0.03 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 256  | Reversed           |     4,873.3 ns |   273.84 ns |   143.22 ns |  1.06 |    0.03 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 256  | Reversed           |     4,332.7 ns |    33.31 ns |    17.42 ns |  0.95 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetwork            | 256  | Reversed           |     3,507.3 ns |   290.97 ns |   129.19 ns |  0.77 |    0.03 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 256  | Reversed           |     4,250.6 ns |    29.84 ns |    10.64 ns |  0.93 |    0.01 |    3 |         - |          NA |
| DotnetSort                        | 256  | Reversed           |     1,456.0 ns |   263.58 ns |   137.86 ns |  0.32 |    0.03 |    1 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BitonicSort**                       | **256**  | **PipeOrgan**          |     **5,044.5 ns** |    **50.67 ns** |    **22.50 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 256  | PipeOrgan          |     5,092.1 ns |    76.84 ns |    34.12 ns |  1.01 |    0.01 |    2 |         - |          NA |
| BitonicRecursiveSort              | 256  | PipeOrgan          |    23,128.4 ns |   230.14 ns |   102.18 ns |  4.58 |    0.03 |    4 |         - |          NA |
| BitonicSortAnyLength              | 256  | PipeOrgan          |    14,743.4 ns |   289.87 ns |   151.61 ns |  2.92 |    0.03 |    3 |         - |          NA |
| BitonicSortAnyLengthSimd          | 256  | PipeOrgan          |     3,267.0 ns |     8.36 ns |     2.98 ns |  0.65 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 256  | PipeOrgan          |     4,751.1 ns |   346.98 ns |   181.48 ns |  0.94 |    0.03 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 256  | PipeOrgan          |     4,355.1 ns |     8.23 ns |     3.65 ns |  0.86 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 256  | PipeOrgan          |     3,849.1 ns |   407.23 ns |   212.99 ns |  0.76 |    0.04 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 256  | PipeOrgan          |     4,306.6 ns |    26.18 ns |    11.63 ns |  0.85 |    0.00 |    2 |         - |          NA |
| DotnetSort                        | 256  | PipeOrgan          |     2,478.1 ns |   312.64 ns |   163.52 ns |  0.49 |    0.03 |    1 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BitonicSort**                       | **256**  | **ManyDuplicates**     |    **15,254.2 ns** |   **301.42 ns** |   **133.83 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| BitonicSortBranchless             | 256  | ManyDuplicates     |     4,764.7 ns |    44.81 ns |    19.90 ns |  0.31 |    0.00 |    2 |         - |          NA |
| BitonicRecursiveSort              | 256  | ManyDuplicates     |    32,885.1 ns |   160.01 ns |    71.05 ns |  2.16 |    0.02 |    6 |         - |          NA |
| BitonicSortAnyLength              | 256  | ManyDuplicates     |    25,395.6 ns |   401.60 ns |   178.31 ns |  1.66 |    0.02 |    5 |         - |          NA |
| BitonicSortAnyLengthSimd          | 256  | ManyDuplicates     |     3,292.6 ns |    13.98 ns |     4.98 ns |  0.22 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 256  | ManyDuplicates     |    12,941.1 ns |   161.03 ns |    71.50 ns |  0.85 |    0.01 |    4 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 256  | ManyDuplicates     |     4,347.8 ns |    53.04 ns |    18.92 ns |  0.29 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 256  | ManyDuplicates     |    14,005.5 ns |   627.44 ns |   328.16 ns |  0.92 |    0.02 |    4 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 256  | ManyDuplicates     |     4,267.7 ns |     8.37 ns |     2.99 ns |  0.28 |    0.00 |    2 |         - |          NA |
| DotnetSort                        | 256  | ManyDuplicates     |     7,146.2 ns |   262.41 ns |   137.25 ns |  0.47 |    0.01 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BitonicSort**                       | **1024** | **Random**             |    **86,258.4 ns** |   **895.98 ns** |   **397.82 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| BitonicSortBranchless             | 1024 | Random             |    29,385.4 ns |   433.83 ns |   226.90 ns |  0.34 |    0.00 |    2 |         - |          NA |
| BitonicRecursiveSort              | 1024 | Random             |   176,672.4 ns | 1,236.79 ns |   549.14 ns |  2.05 |    0.01 |    6 |         - |          NA |
| BitonicSortAnyLength              | 1024 | Random             |   140,905.2 ns |   880.70 ns |   460.62 ns |  1.63 |    0.01 |    5 |         - |          NA |
| BitonicSortAnyLengthSimd          | 1024 | Random             |    18,785.0 ns |   193.94 ns |    69.16 ns |  0.22 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 1024 | Random             |    71,512.8 ns |   399.69 ns |   142.53 ns |  0.83 |    0.00 |    4 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 1024 | Random             |    26,845.5 ns | 1,053.81 ns |   551.16 ns |  0.31 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 1024 | Random             |    81,779.9 ns |   541.46 ns |   193.09 ns |  0.95 |    0.00 |    4 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 1024 | Random             |    26,496.3 ns |   436.59 ns |   228.35 ns |  0.31 |    0.00 |    2 |         - |          NA |
| DotnetSort                        | 1024 | Random             |    38,501.7 ns |   541.14 ns |   283.03 ns |  0.45 |    0.00 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BitonicSort**                       | **1024** | **SingleElementMoved** |    **27,963.0 ns** |   **769.13 ns** |   **341.50 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 1024 | SingleElementMoved |    28,713.5 ns |   461.41 ns |   241.33 ns |  1.03 |    0.01 |    2 |         - |          NA |
| BitonicRecursiveSort              | 1024 | SingleElementMoved |   121,337.7 ns | 1,549.39 ns |   810.36 ns |  4.34 |    0.06 |    4 |         - |          NA |
| BitonicSortAnyLength              | 1024 | SingleElementMoved |    79,946.3 ns |   226.11 ns |   100.40 ns |  2.86 |    0.03 |    3 |         - |          NA |
| BitonicSortAnyLengthSimd          | 1024 | SingleElementMoved |    18,966.7 ns |   330.70 ns |   172.96 ns |  0.68 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 1024 | SingleElementMoved |    29,718.4 ns | 1,047.58 ns |   547.90 ns |  1.06 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 1024 | SingleElementMoved |    26,316.4 ns |   364.39 ns |   161.79 ns |  0.94 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 1024 | SingleElementMoved |    22,360.1 ns | 1,375.77 ns |   719.55 ns |  0.80 |    0.03 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 1024 | SingleElementMoved |    26,450.1 ns |   218.50 ns |   114.28 ns |  0.95 |    0.01 |    2 |         - |          NA |
| DotnetSort                        | 1024 | SingleElementMoved |     6,801.8 ns |   240.51 ns |   125.79 ns |  0.24 |    0.01 |    1 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BitonicSort**                       | **1024** | **Sorted**             |    **28,895.6 ns** |   **314.26 ns** |   **164.36 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 1024 | Sorted             |    29,396.7 ns |   170.81 ns |    89.34 ns |  1.02 |    0.01 |    3 |         - |          NA |
| BitonicRecursiveSort              | 1024 | Sorted             |   119,823.5 ns | 1,139.16 ns |   505.79 ns |  4.15 |    0.03 |    5 |         - |          NA |
| BitonicSortAnyLength              | 1024 | Sorted             |    77,948.2 ns |   383.62 ns |   170.33 ns |  2.70 |    0.02 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 1024 | Sorted             |    18,840.5 ns |   313.63 ns |   164.04 ns |  0.65 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 1024 | Sorted             |    29,904.0 ns |   406.01 ns |   212.35 ns |  1.03 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 1024 | Sorted             |    26,484.6 ns |   361.60 ns |   160.55 ns |  0.92 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetwork            | 1024 | Sorted             |    23,162.6 ns |   295.10 ns |   154.34 ns |  0.80 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 1024 | Sorted             |    26,390.7 ns |   337.39 ns |   176.46 ns |  0.91 |    0.01 |    3 |         - |          NA |
| DotnetSort                        | 1024 | Sorted             |     4,056.2 ns |    49.04 ns |    17.49 ns |  0.14 |    0.00 |    1 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BitonicSort**                       | **1024** | **Reversed**           |    **27,905.0 ns** |   **418.53 ns** |   **218.90 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 1024 | Reversed           |    28,690.9 ns |   344.80 ns |   153.10 ns |  1.03 |    0.01 |    3 |         - |          NA |
| BitonicRecursiveSort              | 1024 | Reversed           |   120,600.8 ns |   858.47 ns |   381.17 ns |  4.32 |    0.03 |    5 |         - |          NA |
| BitonicSortAnyLength              | 1024 | Reversed           |    79,759.2 ns |   238.36 ns |   105.83 ns |  2.86 |    0.02 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 1024 | Reversed           |    19,043.0 ns |   327.15 ns |   171.11 ns |  0.68 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 1024 | Reversed           |    28,823.6 ns | 1,586.64 ns |   829.84 ns |  1.03 |    0.03 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 1024 | Reversed           |    26,367.9 ns |   319.48 ns |   141.85 ns |  0.94 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetwork            | 1024 | Reversed           |    21,786.3 ns | 1,170.99 ns |   519.93 ns |  0.78 |    0.02 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 1024 | Reversed           |    26,449.6 ns |   506.77 ns |   225.01 ns |  0.95 |    0.01 |    3 |         - |          NA |
| DotnetSort                        | 1024 | Reversed           |     8,207.6 ns |   579.42 ns |   303.05 ns |  0.29 |    0.01 |    1 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BitonicSort**                       | **1024** | **PipeOrgan**          |    **28,507.5 ns** |   **536.01 ns** |   **280.34 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 1024 | PipeOrgan          |    29,394.2 ns |   171.26 ns |    89.57 ns |  1.03 |    0.01 |    2 |         - |          NA |
| BitonicRecursiveSort              | 1024 | PipeOrgan          |   119,611.1 ns |   723.55 ns |   321.26 ns |  4.20 |    0.04 |    4 |         - |          NA |
| BitonicSortAnyLength              | 1024 | PipeOrgan          |    78,347.0 ns |   498.86 ns |   221.50 ns |  2.75 |    0.03 |    3 |         - |          NA |
| BitonicSortAnyLengthSimd          | 1024 | PipeOrgan          |    18,810.8 ns |   416.25 ns |   184.82 ns |  0.66 |    0.01 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 1024 | PipeOrgan          |    28,907.3 ns |   265.62 ns |   117.94 ns |  1.01 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 1024 | PipeOrgan          |    26,506.5 ns |   251.70 ns |   131.65 ns |  0.93 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 1024 | PipeOrgan          |    21,380.2 ns |   912.00 ns |   476.99 ns |  0.75 |    0.02 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 1024 | PipeOrgan          |    26,336.1 ns |   160.57 ns |    71.29 ns |  0.92 |    0.01 |    2 |         - |          NA |
| DotnetSort                        | 1024 | PipeOrgan          |    16,651.5 ns |   356.81 ns |   127.24 ns |  0.58 |    0.01 |    1 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BitonicSort**                       | **1024** | **ManyDuplicates**     |    **71,943.9 ns** |   **620.84 ns** |   **275.66 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 1024 | ManyDuplicates     |    29,343.0 ns |   268.88 ns |   140.63 ns |  0.41 |    0.00 |    2 |         - |          NA |
| BitonicRecursiveSort              | 1024 | ManyDuplicates     |   165,301.2 ns | 1,274.35 ns |   666.51 ns |  2.30 |    0.01 |    5 |         - |          NA |
| BitonicSortAnyLength              | 1024 | ManyDuplicates     |   127,491.1 ns |   630.88 ns |   280.11 ns |  1.77 |    0.01 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 1024 | ManyDuplicates     |    18,808.3 ns |   165.89 ns |    86.76 ns |  0.26 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 1024 | ManyDuplicates     |    64,074.6 ns |   749.20 ns |   391.84 ns |  0.89 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 1024 | ManyDuplicates     |    26,725.5 ns |   232.31 ns |   121.50 ns |  0.37 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 1024 | ManyDuplicates     |    67,696.9 ns |   834.46 ns |   436.44 ns |  0.94 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 1024 | ManyDuplicates     |    26,486.6 ns |   314.65 ns |   164.57 ns |  0.37 |    0.00 |    2 |         - |          NA |
| DotnetSort                        | 1024 | ManyDuplicates     |    29,863.7 ns |   409.22 ns |   214.03 ns |  0.42 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BitonicSort**                       | **4096** | **Random**             |   **429,531.0 ns** | **1,304.38 ns** |   **579.15 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 4096 | Random             |   163,609.6 ns | 1,001.80 ns |   444.80 ns |  0.38 |    0.00 |    2 |         - |          NA |
| BitonicRecursiveSort              | 4096 | Random             |   883,765.8 ns | 1,063.33 ns |   556.14 ns |  2.06 |    0.00 |    5 |         - |          NA |
| BitonicSortAnyLength              | 4096 | Random             |   710,736.0 ns | 2,031.75 ns |   724.54 ns |  1.65 |    0.00 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 4096 | Random             |    99,484.1 ns | 1,291.36 ns |   573.37 ns |  0.23 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 4096 | Random             |   362,524.0 ns | 1,736.36 ns |   908.15 ns |  0.84 |    0.00 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 4096 | Random             |   150,422.2 ns | 2,080.89 ns | 1,088.34 ns |  0.35 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 4096 | Random             |   428,189.4 ns | 1,494.79 ns |   781.80 ns |  1.00 |    0.00 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 4096 | Random             |   148,769.7 ns |   937.12 ns |   334.19 ns |  0.35 |    0.00 |    2 |         - |          NA |
| DotnetSort                        | 4096 | Random             |   186,760.0 ns | 2,709.35 ns | 1,417.04 ns |  0.43 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BitonicSort**                       | **4096** | **SingleElementMoved** |   **152,392.1 ns** | **1,331.61 ns** |   **591.24 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| BitonicSortBranchless             | 4096 | SingleElementMoved |   160,463.2 ns | 1,399.75 ns |   621.50 ns |  1.05 |    0.01 |    4 |         - |          NA |
| BitonicRecursiveSort              | 4096 | SingleElementMoved |   601,423.3 ns | 2,621.47 ns | 1,371.08 ns |  3.95 |    0.02 |    6 |         - |          NA |
| BitonicSortAnyLength              | 4096 | SingleElementMoved |   407,770.8 ns | 4,242.37 ns | 2,218.84 ns |  2.68 |    0.02 |    5 |         - |          NA |
| BitonicSortAnyLengthSimd          | 4096 | SingleElementMoved |   100,127.5 ns | 1,765.25 ns |   923.26 ns |  0.66 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 4096 | SingleElementMoved |   165,979.0 ns | 3,543.98 ns | 1,853.57 ns |  1.09 |    0.01 |    4 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 4096 | SingleElementMoved |   149,795.4 ns | 2,668.68 ns | 1,395.77 ns |  0.98 |    0.01 |    4 |         - |          NA |
| PairwiseSortingNetwork            | 4096 | SingleElementMoved |   123,446.7 ns | 2,259.39 ns | 1,003.19 ns |  0.81 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 4096 | SingleElementMoved |   148,278.8 ns |   868.38 ns |   385.57 ns |  0.97 |    0.00 |    4 |         - |          NA |
| DotnetSort                        | 4096 | SingleElementMoved |    31,554.8 ns |   381.73 ns |   136.13 ns |  0.21 |    0.00 |    1 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BitonicSort**                       | **4096** | **Sorted**             |   **158,093.2 ns** | **2,464.99 ns** | **1,289.24 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 4096 | Sorted             |   163,102.1 ns |   448.56 ns |   159.96 ns |  1.03 |    0.01 |    3 |         - |          NA |
| BitonicRecursiveSort              | 4096 | Sorted             |   593,094.7 ns | 1,179.29 ns |   523.61 ns |  3.75 |    0.03 |    5 |         - |          NA |
| BitonicSortAnyLength              | 4096 | Sorted             |   395,012.0 ns | 1,241.97 ns |   649.58 ns |  2.50 |    0.02 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 4096 | Sorted             |    99,105.3 ns | 1,442.13 ns |   754.26 ns |  0.63 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 4096 | Sorted             |   167,229.0 ns | 2,189.97 ns | 1,145.40 ns |  1.06 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 4096 | Sorted             |   150,469.6 ns | 1,692.05 ns |   884.98 ns |  0.95 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetwork            | 4096 | Sorted             |   127,472.4 ns | 2,102.60 ns | 1,099.70 ns |  0.81 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 4096 | Sorted             |   149,365.2 ns | 2,120.06 ns |   941.32 ns |  0.94 |    0.01 |    3 |         - |          NA |
| DotnetSort                        | 4096 | Sorted             |    19,523.9 ns |   713.04 ns |   316.59 ns |  0.12 |    0.00 |    1 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BitonicSort**                       | **4096** | **Reversed**           |   **153,750.4 ns** | **1,465.18 ns** |   **650.55 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 4096 | Reversed           |   160,710.6 ns |   782.10 ns |   347.26 ns |  1.05 |    0.00 |    3 |         - |          NA |
| BitonicRecursiveSort              | 4096 | Reversed           |   597,991.5 ns | 1,695.83 ns |   752.96 ns |  3.89 |    0.02 |    5 |         - |          NA |
| BitonicSortAnyLength              | 4096 | Reversed           |   407,108.2 ns | 1,346.93 ns |   598.04 ns |  2.65 |    0.01 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 4096 | Reversed           |    99,407.3 ns |   273.06 ns |   121.24 ns |  0.65 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 4096 | Reversed           |   158,368.5 ns | 3,580.51 ns | 1,872.67 ns |  1.03 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 4096 | Reversed           |   149,303.5 ns | 1,439.41 ns |   639.11 ns |  0.97 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetwork            | 4096 | Reversed           |   125,474.1 ns | 4,143.68 ns | 1,839.82 ns |  0.82 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 4096 | Reversed           |   148,720.0 ns | 1,515.11 ns |   792.43 ns |  0.97 |    0.01 |    3 |         - |          NA |
| DotnetSort                        | 4096 | Reversed           |    37,325.4 ns | 3,951.16 ns | 2,066.53 ns |  0.24 |    0.01 |    1 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BitonicSort**                       | **4096** | **PipeOrgan**          |   **155,989.7 ns** | **1,879.03 ns** |   **982.77 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 4096 | PipeOrgan          |   163,993.2 ns | 2,252.71 ns | 1,000.22 ns |  1.05 |    0.01 |    3 |         - |          NA |
| BitonicRecursiveSort              | 4096 | PipeOrgan          |   593,292.8 ns | 1,391.89 ns |   618.01 ns |  3.80 |    0.02 |    5 |         - |          NA |
| BitonicSortAnyLength              | 4096 | PipeOrgan          |   395,281.0 ns | 2,035.59 ns | 1,064.65 ns |  2.53 |    0.02 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 4096 | PipeOrgan          |    97,992.0 ns |   690.77 ns |   306.71 ns |  0.63 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 4096 | PipeOrgan          |   162,163.0 ns |   797.16 ns |   353.95 ns |  1.04 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 4096 | PipeOrgan          |   149,757.9 ns | 1,631.64 ns |   724.46 ns |  0.96 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetwork            | 4096 | PipeOrgan          |   122,928.2 ns | 5,339.21 ns | 2,792.51 ns |  0.79 |    0.02 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 4096 | PipeOrgan          |   148,727.2 ns |   583.01 ns |   258.86 ns |  0.95 |    0.01 |    3 |         - |          NA |
| DotnetSort                        | 4096 | PipeOrgan          |    93,817.5 ns | 3,300.75 ns | 1,726.36 ns |  0.60 |    0.01 |    1 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BitonicSort**                       | **4096** | **ManyDuplicates**     |   **333,235.0 ns** | **2,251.48 ns** | **1,177.57 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| BitonicSortBranchless             | 4096 | ManyDuplicates     |   163,433.7 ns |   756.05 ns |   335.69 ns |  0.49 |    0.00 |    3 |         - |          NA |
| BitonicRecursiveSort              | 4096 | ManyDuplicates     |   781,050.6 ns | 1,553.90 ns |   812.72 ns |  2.34 |    0.01 |    6 |         - |          NA |
| BitonicSortAnyLength              | 4096 | ManyDuplicates     |   603,877.0 ns | 1,276.28 ns |   667.52 ns |  1.81 |    0.01 |    5 |         - |          NA |
| BitonicSortAnyLengthSimd          | 4096 | ManyDuplicates     |    98,546.6 ns |   145.53 ns |    64.62 ns |  0.30 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 4096 | ManyDuplicates     |   304,439.8 ns | 1,140.22 ns |   596.36 ns |  0.91 |    0.00 |    4 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 4096 | ManyDuplicates     |   150,244.6 ns | 1,883.00 ns |   836.06 ns |  0.45 |    0.00 |    3 |         - |          NA |
| PairwiseSortingNetwork            | 4096 | ManyDuplicates     |   316,518.1 ns | 3,063.31 ns | 1,360.13 ns |  0.95 |    0.00 |    4 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 4096 | ManyDuplicates     |   148,328.2 ns |   449.43 ns |   199.55 ns |  0.45 |    0.00 |    3 |         - |          NA |
| DotnetSort                        | 4096 | ManyDuplicates     |   121,867.2 ns | 1,084.36 ns |   481.46 ns |  0.37 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BitonicSort**                       | **8192** | **Random**             |   **952,942.6 ns** | **1,127.58 ns** |   **500.65 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 8192 | Random             |   379,718.0 ns | 2,023.65 ns | 1,058.41 ns |  0.40 |    0.00 |    2 |         - |          NA |
| BitonicRecursiveSort              | 8192 | Random             | 1,977,335.7 ns | 6,650.91 ns | 2,953.04 ns |  2.07 |    0.00 |    5 |         - |          NA |
| BitonicSortAnyLength              | 8192 | Random             | 1,590,777.8 ns | 4,012.76 ns | 2,098.75 ns |  1.67 |    0.00 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 8192 | Random             |   219,789.5 ns | 2,401.83 ns | 1,256.20 ns |  0.23 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 8192 | Random             |   808,396.7 ns | 3,818.98 ns | 1,695.65 ns |  0.85 |    0.00 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 8192 | Random             |   347,272.9 ns | 2,288.69 ns | 1,016.19 ns |  0.36 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 8192 | Random             |   967,764.1 ns | 5,352.38 ns | 2,376.49 ns |  1.02 |    0.00 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 8192 | Random             |   346,484.9 ns | 2,295.68 ns | 1,200.69 ns |  0.36 |    0.00 |    2 |         - |          NA |
| DotnetSort                        | 8192 | Random             |   407,124.5 ns | 3,055.32 ns | 1,597.99 ns |  0.43 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BitonicSort**                       | **8192** | **SingleElementMoved** |   **349,667.4 ns** | **2,117.49 ns** | **1,107.49 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| BitonicSortBranchless             | 8192 | SingleElementMoved |   370,710.7 ns | 2,270.72 ns | 1,008.21 ns |  1.06 |    0.00 |    4 |         - |          NA |
| BitonicRecursiveSort              | 8192 | SingleElementMoved | 1,354,195.0 ns | 5,526.34 ns | 2,890.38 ns |  3.87 |    0.01 |    6 |         - |          NA |
| BitonicSortAnyLength              | 8192 | SingleElementMoved |   916,372.9 ns | 1,591.05 ns |   706.44 ns |  2.62 |    0.01 |    5 |         - |          NA |
| BitonicSortAnyLengthSimd          | 8192 | SingleElementMoved |   221,322.6 ns | 1,415.37 ns |   740.26 ns |  0.63 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 8192 | SingleElementMoved |   386,844.5 ns | 6,100.96 ns | 2,708.86 ns |  1.11 |    0.01 |    4 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 8192 | SingleElementMoved |   346,712.3 ns | 3,611.30 ns | 1,603.44 ns |  0.99 |    0.01 |    4 |         - |          NA |
| PairwiseSortingNetwork            | 8192 | SingleElementMoved |   283,412.5 ns | 6,014.83 ns | 3,145.87 ns |  0.81 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 8192 | SingleElementMoved |   344,642.1 ns |   691.81 ns |   307.17 ns |  0.99 |    0.00 |    4 |         - |          NA |
| DotnetSort                        | 8192 | SingleElementMoved |    69,133.1 ns | 1,675.13 ns |   876.12 ns |  0.20 |    0.00 |    1 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BitonicSort**                       | **8192** | **Sorted**             |   **363,274.7 ns** | **2,415.88 ns** | **1,263.55 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 8192 | Sorted             |   378,861.7 ns | 1,215.40 ns |   539.64 ns |  1.04 |    0.00 |    3 |         - |          NA |
| BitonicRecursiveSort              | 8192 | Sorted             | 1,336,225.7 ns | 6,533.38 ns | 3,417.08 ns |  3.68 |    0.01 |    5 |         - |          NA |
| BitonicSortAnyLength              | 8192 | Sorted             |   885,979.9 ns | 4,122.23 ns | 1,830.29 ns |  2.44 |    0.01 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 8192 | Sorted             |   218,754.7 ns | 2,076.78 ns |   922.10 ns |  0.60 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 8192 | Sorted             |   388,884.0 ns | 3,658.24 ns | 1,913.33 ns |  1.07 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 8192 | Sorted             |   349,471.4 ns | 3,402.58 ns | 1,779.61 ns |  0.96 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetwork            | 8192 | Sorted             |   294,109.7 ns |   791.79 ns |   351.56 ns |  0.81 |    0.00 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 8192 | Sorted             |   346,103.0 ns | 1,427.59 ns |   633.86 ns |  0.95 |    0.00 |    3 |         - |          NA |
| DotnetSort                        | 8192 | Sorted             |    47,773.4 ns | 8,130.51 ns | 4,252.41 ns |  0.13 |    0.01 |    1 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BitonicSort**                       | **8192** | **Reversed**           |   **355,311.1 ns** | **2,010.99 ns** | **1,051.78 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 8192 | Reversed           |   371,677.8 ns | 2,564.08 ns | 1,341.07 ns |  1.05 |    0.00 |    3 |         - |          NA |
| BitonicRecursiveSort              | 8192 | Reversed           | 1,348,037.1 ns | 2,978.43 ns | 1,322.44 ns |  3.79 |    0.01 |    5 |         - |          NA |
| BitonicSortAnyLength              | 8192 | Reversed           |   915,272.3 ns | 3,073.79 ns | 1,364.78 ns |  2.58 |    0.01 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 8192 | Reversed           |   220,953.6 ns | 1,280.76 ns |   669.86 ns |  0.62 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 8192 | Reversed           |   368,391.2 ns | 6,123.20 ns | 3,202.55 ns |  1.04 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 8192 | Reversed           |   347,890.9 ns | 2,813.08 ns | 1,471.29 ns |  0.98 |    0.00 |    3 |         - |          NA |
| PairwiseSortingNetwork            | 8192 | Reversed           |   290,732.1 ns | 6,340.87 ns | 3,316.40 ns |  0.82 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 8192 | Reversed           |   345,690.1 ns | 2,702.56 ns | 1,199.95 ns |  0.97 |    0.00 |    3 |         - |          NA |
| DotnetSort                        | 8192 | Reversed           |    77,640.6 ns | 4,260.44 ns | 1,891.66 ns |  0.22 |    0.01 |    1 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BitonicSort**                       | **8192** | **PipeOrgan**          |   **358,201.1 ns** | **1,529.11 ns** |   **799.75 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 8192 | PipeOrgan          |   378,169.6 ns | 1,113.28 ns |   494.31 ns |  1.06 |    0.00 |    3 |         - |          NA |
| BitonicRecursiveSort              | 8192 | PipeOrgan          | 1,334,624.0 ns | 2,214.55 ns |   983.28 ns |  3.73 |    0.01 |    5 |         - |          NA |
| BitonicSortAnyLength              | 8192 | PipeOrgan          |   884,708.8 ns | 2,545.20 ns | 1,331.19 ns |  2.47 |    0.01 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 8192 | PipeOrgan          |   216,246.0 ns | 2,294.54 ns | 1,200.09 ns |  0.60 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 8192 | PipeOrgan          |   379,868.5 ns | 3,076.57 ns | 1,609.10 ns |  1.06 |    0.00 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 8192 | PipeOrgan          |   350,297.4 ns | 1,751.95 ns |   777.88 ns |  0.98 |    0.00 |    3 |         - |          NA |
| PairwiseSortingNetwork            | 8192 | PipeOrgan          |   287,752.2 ns | 7,149.01 ns | 3,739.07 ns |  0.80 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 8192 | PipeOrgan          |   346,975.8 ns | 2,093.25 ns | 1,094.81 ns |  0.97 |    0.00 |    3 |         - |          NA |
| DotnetSort                        | 8192 | PipeOrgan          |   362,447.5 ns | 6,808.73 ns | 3,561.10 ns |  1.01 |    0.01 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BitonicSort**                       | **8192** | **ManyDuplicates**     |   **714,061.5 ns** | **2,557.99 ns** | **1,337.88 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 8192 | ManyDuplicates     |   378,211.8 ns |   698.56 ns |   249.11 ns |  0.53 |    0.00 |    2 |         - |          NA |
| BitonicRecursiveSort              | 8192 | ManyDuplicates     | 1,713,381.8 ns |   816.87 ns |   362.70 ns |  2.40 |    0.00 |    5 |         - |          NA |
| BitonicSortAnyLength              | 8192 | ManyDuplicates     | 1,307,565.6 ns | 3,027.24 ns | 1,344.11 ns |  1.83 |    0.00 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 8192 | ManyDuplicates     |   218,656.7 ns | 1,436.05 ns |   637.61 ns |  0.31 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 8192 | ManyDuplicates     |   661,034.2 ns |   942.16 ns |   492.77 ns |  0.93 |    0.00 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 8192 | ManyDuplicates     |   349,619.7 ns | 2,759.85 ns | 1,443.45 ns |  0.49 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 8192 | ManyDuplicates     |   683,040.1 ns | 1,409.57 ns |   502.67 ns |  0.96 |    0.00 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 8192 | ManyDuplicates     |   345,894.6 ns |   776.84 ns |   344.92 ns |  0.48 |    0.00 |    2 |         - |          NA |
| DotnetSort                        | 8192 | ManyDuplicates     |   245,406.7 ns |   702.38 ns |   311.86 ns |  0.34 |    0.00 |    1 |         - |          NA |

### PartitionBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.86GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.22 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                       | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **QuickSort**                    | **256**  | **Random**             |    **11,207.9 ns** |    **448.75 ns** |    **234.71 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 256  | Random             |    12,248.6 ns |    308.44 ns |    161.32 ns |  1.09 |    0.03 |    3 |         - |          NA |
| QuickSortMedian3             | 256  | Random             |    11,789.7 ns |    107.85 ns |     47.89 ns |  1.05 |    0.02 |    3 |         - |          NA |
| QuickSortMedian9             | 256  | Random             |    12,411.0 ns |    263.09 ns |    116.81 ns |  1.11 |    0.02 |    3 |         - |          NA |
| SampleSort                   | 256  | Random             |    14,235.4 ns |    432.28 ns |    226.09 ns |  1.27 |    0.03 |    3 |         - |          NA |
| SimdQuickSort                | 256  | Random             |     2,422.4 ns |    319.12 ns |    141.69 ns |  0.22 |    0.01 |    1 |         - |          NA |
| DualPivotQuickSort           | 256  | Random             |     7,743.2 ns |    453.30 ns |    237.09 ns |  0.69 |    0.02 |    3 |         - |          NA |
| StableQuickSort              | 256  | Random             |    25,330.1 ns |    247.97 ns |    110.10 ns |  2.26 |    0.05 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Random             |    14,742.0 ns |    389.64 ns |    203.79 ns |  1.32 |    0.03 |    3 |         - |          NA |
| DestswapStableQuickSort      | 256  | Random             |    25,039.8 ns |    239.88 ns |    106.51 ns |  2.23 |    0.04 |    4 |         - |          NA |
| IntroSort                    | 256  | Random             |     9,007.3 ns |    437.19 ns |    228.66 ns |  0.80 |    0.02 |    3 |         - |          NA |
| IntroSortDotnet              | 256  | Random             |     8,294.1 ns |    394.50 ns |    175.16 ns |  0.74 |    0.02 |    3 |         - |          NA |
| PDQSort                      | 256  | Random             |     7,657.5 ns |    246.73 ns |    109.55 ns |  0.68 |    0.02 |    3 |         - |          NA |
| PDQSortBranchless            | 256  | Random             |     5,682.8 ns |    403.15 ns |    210.86 ns |  0.51 |    0.02 |    2 |         - |          NA |
| Ipnsort                      | 256  | Random             |     4,833.3 ns |    323.48 ns |    169.19 ns |  0.43 |    0.02 |    2 |         - |          NA |
| StdSort                      | 256  | Random             |     5,811.5 ns |    257.18 ns |    134.51 ns |  0.52 |    0.02 |    2 |         - |          NA |
| BlockQuickSort               | 256  | Random             |     6,083.5 ns |    475.12 ns |    248.49 ns |  0.54 |    0.02 |    2 |         - |          NA |
| Ips4oSort                    | 256  | Random             |    10,186.9 ns |    343.96 ns |    179.90 ns |  0.91 |    0.02 |    3 |         - |          NA |
| FluxSort                     | 256  | Random             |     7,865.2 ns |    326.46 ns |    170.75 ns |  0.70 |    0.02 |    3 |         - |          NA |
| CrumSort                     | 256  | Random             |     4,498.8 ns |    310.28 ns |    137.77 ns |  0.40 |    0.01 |    2 |         - |          NA |
| BlitSort                     | 256  | Random             |     7,971.6 ns |    348.84 ns |    182.45 ns |  0.71 |    0.02 |    3 |         - |          NA |
| DotnetSort                   | 256  | Random             |     7,624.9 ns |    318.36 ns |    166.51 ns |  0.68 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **SingleElementMoved** |     **1,425.3 ns** |    **165.15 ns** |     **73.33 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 256  | SingleElementMoved |     2,838.4 ns |    246.47 ns |    128.91 ns |  2.00 |    0.12 |    2 |         - |          NA |
| QuickSortMedian3             | 256  | SingleElementMoved |     2,137.8 ns |    236.31 ns |    104.92 ns |  1.50 |    0.10 |    2 |         - |          NA |
| QuickSortMedian9             | 256  | SingleElementMoved |     2,307.2 ns |    249.45 ns |    110.76 ns |  1.62 |    0.10 |    2 |         - |          NA |
| SampleSort                   | 256  | SingleElementMoved |     7,452.7 ns |    374.34 ns |    166.21 ns |  5.24 |    0.26 |    4 |         - |          NA |
| SimdQuickSort                | 256  | SingleElementMoved |     1,793.8 ns |     34.34 ns |     15.25 ns |  1.26 |    0.06 |    2 |         - |          NA |
| DualPivotQuickSort           | 256  | SingleElementMoved |     1,043.1 ns |     70.82 ns |     37.04 ns |  0.73 |    0.04 |    2 |         - |          NA |
| StableQuickSort              | 256  | SingleElementMoved |     9,071.5 ns |    297.31 ns |    155.50 ns |  6.38 |    0.31 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 256  | SingleElementMoved |     4,905.4 ns |    291.53 ns |    152.48 ns |  3.45 |    0.19 |    3 |         - |          NA |
| DestswapStableQuickSort      | 256  | SingleElementMoved |    10,387.1 ns |    519.74 ns |    271.83 ns |  7.30 |    0.38 |    5 |         - |          NA |
| IntroSort                    | 256  | SingleElementMoved |     2,758.9 ns |    487.27 ns |    254.85 ns |  1.94 |    0.19 |    2 |         - |          NA |
| IntroSortDotnet              | 256  | SingleElementMoved |     1,428.9 ns |    177.34 ns |     92.75 ns |  1.00 |    0.08 |    2 |         - |          NA |
| PDQSort                      | 256  | SingleElementMoved |     1,125.8 ns |    173.65 ns |     90.82 ns |  0.79 |    0.07 |    2 |         - |          NA |
| PDQSortBranchless            | 256  | SingleElementMoved |     1,274.6 ns |    191.63 ns |    100.22 ns |  0.90 |    0.08 |    2 |         - |          NA |
| Ipnsort                      | 256  | SingleElementMoved |     3,733.9 ns |    100.09 ns |     52.35 ns |  2.63 |    0.12 |    2 |         - |          NA |
| StdSort                      | 256  | SingleElementMoved |     1,174.9 ns |     77.19 ns |     34.27 ns |  0.83 |    0.04 |    2 |         - |          NA |
| BlockQuickSort               | 256  | SingleElementMoved |     2,002.9 ns |    189.32 ns |     84.06 ns |  1.41 |    0.08 |    2 |         - |          NA |
| Ips4oSort                    | 256  | SingleElementMoved |       594.1 ns |     17.77 ns |      6.34 ns |  0.42 |    0.02 |    1 |         - |          NA |
| FluxSort                     | 256  | SingleElementMoved |     1,591.1 ns |     29.70 ns |     10.59 ns |  1.12 |    0.05 |    2 |         - |          NA |
| CrumSort                     | 256  | SingleElementMoved |       678.1 ns |     45.83 ns |     20.35 ns |  0.48 |    0.03 |    1 |         - |          NA |
| BlitSort                     | 256  | SingleElementMoved |     3,107.7 ns |     33.52 ns |     11.95 ns |  2.19 |    0.10 |    2 |         - |          NA |
| DotnetSort                   | 256  | SingleElementMoved |     1,355.3 ns |     91.25 ns |     40.51 ns |  0.95 |    0.05 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Sorted**             |       **898.2 ns** |     **35.25 ns** |     **18.44 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 256  | Sorted             |     1,834.7 ns |     38.55 ns |     20.16 ns |  2.04 |    0.04 |    6 |         - |          NA |
| QuickSortMedian3             | 256  | Sorted             |     1,207.7 ns |     19.75 ns |      8.77 ns |  1.35 |    0.03 |    5 |         - |          NA |
| QuickSortMedian9             | 256  | Sorted             |     1,310.3 ns |     47.04 ns |     16.78 ns |  1.46 |    0.03 |    5 |         - |          NA |
| SampleSort                   | 256  | Sorted             |     4,525.7 ns |    282.82 ns |    125.58 ns |  5.04 |    0.16 |    7 |         - |          NA |
| SimdQuickSort                | 256  | Sorted             |     1,736.8 ns |     14.66 ns |      6.51 ns |  1.93 |    0.04 |    6 |         - |          NA |
| DualPivotQuickSort           | 256  | Sorted             |       870.9 ns |      5.15 ns |      2.29 ns |  0.97 |    0.02 |    4 |         - |          NA |
| StableQuickSort              | 256  | Sorted             |     7,796.5 ns |    408.52 ns |    213.67 ns |  8.68 |    0.28 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Sorted             |     4,356.9 ns |     18.73 ns |      6.68 ns |  4.85 |    0.09 |    7 |         - |          NA |
| DestswapStableQuickSort      | 256  | Sorted             |     9,736.2 ns |    328.14 ns |    171.62 ns | 10.84 |    0.27 |    9 |         - |          NA |
| IntroSort                    | 256  | Sorted             |       808.3 ns |     10.33 ns |      5.40 ns |  0.90 |    0.02 |    4 |         - |          NA |
| IntroSortDotnet              | 256  | Sorted             |     1,001.7 ns |     12.27 ns |      6.42 ns |  1.12 |    0.02 |    4 |         - |          NA |
| PDQSort                      | 256  | Sorted             |       315.0 ns |     14.42 ns |      7.54 ns |  0.35 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 256  | Sorted             |       328.7 ns |     88.10 ns |     39.12 ns |  0.37 |    0.04 |    2 |         - |          NA |
| Ipnsort                      | 256  | Sorted             |       174.4 ns |      6.47 ns |      2.87 ns |  0.19 |    0.00 |    1 |         - |          NA |
| StdSort                      | 256  | Sorted             |       437.1 ns |     20.62 ns |      9.15 ns |  0.49 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 256  | Sorted             |     1,612.8 ns |     63.11 ns |     28.02 ns |  1.80 |    0.05 |    6 |         - |          NA |
| Ips4oSort                    | 256  | Sorted             |       161.1 ns |     10.59 ns |      5.54 ns |  0.18 |    0.01 |    1 |         - |          NA |
| FluxSort                     | 256  | Sorted             |       259.2 ns |     24.92 ns |     11.06 ns |  0.29 |    0.01 |    2 |         - |          NA |
| CrumSort                     | 256  | Sorted             |       342.6 ns |     13.57 ns |      6.03 ns |  0.38 |    0.01 |    2 |         - |          NA |
| BlitSort                     | 256  | Sorted             |       294.4 ns |     19.76 ns |      8.77 ns |  0.33 |    0.01 |    2 |         - |          NA |
| DotnetSort                   | 256  | Sorted             |       806.2 ns |      2.71 ns |      0.97 ns |  0.90 |    0.02 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Reversed**           |     **1,036.6 ns** |     **24.72 ns** |     **12.93 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 256  | Reversed           |     2,096.7 ns |    104.83 ns |     46.55 ns |  2.02 |    0.05 |    6 |         - |          NA |
| QuickSortMedian3             | 256  | Reversed           |     1,306.7 ns |     28.23 ns |     10.07 ns |  1.26 |    0.02 |    5 |         - |          NA |
| QuickSortMedian9             | 256  | Reversed           |     1,469.9 ns |     70.89 ns |     37.07 ns |  1.42 |    0.04 |    5 |         - |          NA |
| SampleSort                   | 256  | Reversed           |     5,033.9 ns |    216.70 ns |    113.34 ns |  4.86 |    0.12 |    7 |         - |          NA |
| SimdQuickSort                | 256  | Reversed           |     1,772.1 ns |     12.75 ns |      6.67 ns |  1.71 |    0.02 |    6 |         - |          NA |
| DualPivotQuickSort           | 256  | Reversed           |       964.1 ns |     10.59 ns |      4.70 ns |  0.93 |    0.01 |    4 |         - |          NA |
| StableQuickSort              | 256  | Reversed           |     9,050.9 ns |    309.01 ns |    161.62 ns |  8.73 |    0.18 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Reversed           |     4,792.1 ns |     94.52 ns |     41.97 ns |  4.62 |    0.07 |    7 |         - |          NA |
| DestswapStableQuickSort      | 256  | Reversed           |     9,075.9 ns |    475.29 ns |    248.58 ns |  8.76 |    0.25 |    8 |         - |          NA |
| IntroSort                    | 256  | Reversed           |       785.2 ns |     16.00 ns |      8.37 ns |  0.76 |    0.01 |    3 |         - |          NA |
| IntroSortDotnet              | 256  | Reversed           |     1,438.3 ns |     20.50 ns |      9.10 ns |  1.39 |    0.02 |    5 |         - |          NA |
| PDQSort                      | 256  | Reversed           |       547.8 ns |      2.86 ns |      1.02 ns |  0.53 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 256  | Reversed           |       766.8 ns |     12.20 ns |      6.38 ns |  0.74 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 256  | Reversed           |       242.7 ns |      2.90 ns |      1.29 ns |  0.23 |    0.00 |    1 |         - |          NA |
| StdSort                      | 256  | Reversed           |       767.8 ns |     18.99 ns |      9.93 ns |  0.74 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 256  | Reversed           |     2,291.2 ns |    370.71 ns |    193.89 ns |  2.21 |    0.18 |    6 |         - |          NA |
| Ips4oSort                    | 256  | Reversed           |       267.5 ns |     17.81 ns |      9.31 ns |  0.26 |    0.01 |    1 |         - |          NA |
| FluxSort                     | 256  | Reversed           |       360.3 ns |     12.55 ns |      6.57 ns |  0.35 |    0.01 |    1 |         - |          NA |
| CrumSort                     | 256  | Reversed           |       318.3 ns |      7.24 ns |      3.78 ns |  0.31 |    0.00 |    1 |         - |          NA |
| BlitSort                     | 256  | Reversed           |       372.2 ns |     13.38 ns |      5.94 ns |  0.36 |    0.01 |    1 |         - |          NA |
| DotnetSort                   | 256  | Reversed           |     1,247.3 ns |     59.54 ns |     21.23 ns |  1.20 |    0.02 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **PipeOrgan**          |     **7,762.2 ns** |    **418.70 ns** |    **185.90 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 256  | PipeOrgan          |     3,771.7 ns |    530.58 ns |    277.50 ns |  0.49 |    0.04 |    4 |         - |          NA |
| QuickSortMedian3             | 256  | PipeOrgan          |     3,001.4 ns |    155.43 ns |     69.01 ns |  0.39 |    0.01 |    3 |         - |          NA |
| QuickSortMedian9             | 256  | PipeOrgan          |     2,662.8 ns |    359.50 ns |    188.03 ns |  0.34 |    0.02 |    3 |         - |          NA |
| SampleSort                   | 256  | PipeOrgan          |     4,899.0 ns |    331.66 ns |    173.46 ns |  0.63 |    0.03 |    4 |         - |          NA |
| SimdQuickSort                | 256  | PipeOrgan          |     5,168.4 ns |     20.94 ns |     10.95 ns |  0.67 |    0.02 |    4 |         - |          NA |
| DualPivotQuickSort           | 256  | PipeOrgan          |     2,022.7 ns |    228.62 ns |    119.57 ns |  0.26 |    0.02 |    2 |         - |          NA |
| StableQuickSort              | 256  | PipeOrgan          |     7,849.4 ns |    642.81 ns |    336.20 ns |  1.01 |    0.05 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 256  | PipeOrgan          |     4,429.1 ns |    277.92 ns |    145.36 ns |  0.57 |    0.02 |    4 |         - |          NA |
| DestswapStableQuickSort      | 256  | PipeOrgan          |    12,612.1 ns |    283.08 ns |    148.06 ns |  1.63 |    0.04 |    6 |         - |          NA |
| IntroSort                    | 256  | PipeOrgan          |     4,281.0 ns |    267.62 ns |    118.83 ns |  0.55 |    0.02 |    4 |         - |          NA |
| IntroSortDotnet              | 256  | PipeOrgan          |     2,152.5 ns |    157.58 ns |     69.96 ns |  0.28 |    0.01 |    2 |         - |          NA |
| PDQSort                      | 256  | PipeOrgan          |     1,741.2 ns |     44.03 ns |     19.55 ns |  0.22 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 256  | PipeOrgan          |     2,787.2 ns |    280.59 ns |    124.58 ns |  0.36 |    0.02 |    3 |         - |          NA |
| Ipnsort                      | 256  | PipeOrgan          |     4,075.0 ns |    328.65 ns |    171.89 ns |  0.53 |    0.02 |    4 |         - |          NA |
| StdSort                      | 256  | PipeOrgan          |     5,043.0 ns |    234.24 ns |    122.51 ns |  0.65 |    0.02 |    4 |         - |          NA |
| BlockQuickSort               | 256  | PipeOrgan          |     4,737.6 ns |    332.75 ns |    174.03 ns |  0.61 |    0.03 |    4 |         - |          NA |
| Ips4oSort                    | 256  | PipeOrgan          |     8,804.5 ns |    339.55 ns |    177.59 ns |  1.13 |    0.03 |    5 |         - |          NA |
| FluxSort                     | 256  | PipeOrgan          |     3,856.9 ns |     48.95 ns |     17.46 ns |  0.50 |    0.01 |    4 |         - |          NA |
| CrumSort                     | 256  | PipeOrgan          |       864.6 ns |     14.90 ns |      6.61 ns |  0.11 |    0.00 |    1 |         - |          NA |
| BlitSort                     | 256  | PipeOrgan          |     4,180.5 ns |    387.05 ns |    202.44 ns |  0.54 |    0.03 |    4 |         - |          NA |
| DotnetSort                   | 256  | PipeOrgan          |     2,603.8 ns |    407.76 ns |    213.27 ns |  0.34 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **ManyDuplicates**     |     **8,938.5 ns** |    **410.96 ns** |    **214.94 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 256  | ManyDuplicates     |     7,774.1 ns |    443.93 ns |    232.18 ns |  0.87 |    0.03 |    3 |         - |          NA |
| QuickSortMedian3             | 256  | ManyDuplicates     |    10,045.4 ns |    236.71 ns |    105.10 ns |  1.12 |    0.03 |    3 |         - |          NA |
| QuickSortMedian9             | 256  | ManyDuplicates     |    10,277.9 ns |    350.02 ns |    183.07 ns |  1.15 |    0.03 |    3 |         - |          NA |
| SampleSort                   | 256  | ManyDuplicates     |    12,687.8 ns |    396.63 ns |    176.10 ns |  1.42 |    0.04 |    4 |         - |          NA |
| SimdQuickSort                | 256  | ManyDuplicates     |     2,376.3 ns |    108.02 ns |     47.96 ns |  0.27 |    0.01 |    1 |         - |          NA |
| DualPivotQuickSort           | 256  | ManyDuplicates     |     7,176.5 ns |    827.87 ns |    432.99 ns |  0.80 |    0.05 |    3 |         - |          NA |
| StableQuickSort              | 256  | ManyDuplicates     |    16,373.6 ns |    129.96 ns |     57.70 ns |  1.83 |    0.04 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 256  | ManyDuplicates     |     8,190.4 ns |    395.64 ns |    206.93 ns |  0.92 |    0.03 |    3 |         - |          NA |
| DestswapStableQuickSort      | 256  | ManyDuplicates     |    14,118.1 ns |    327.51 ns |    145.42 ns |  1.58 |    0.04 |    4 |         - |          NA |
| IntroSort                    | 256  | ManyDuplicates     |     8,047.5 ns |    402.44 ns |    210.49 ns |  0.90 |    0.03 |    3 |         - |          NA |
| IntroSortDotnet              | 256  | ManyDuplicates     |     7,830.7 ns |    369.11 ns |    193.05 ns |  0.88 |    0.03 |    3 |         - |          NA |
| PDQSort                      | 256  | ManyDuplicates     |     7,262.3 ns |    357.14 ns |    186.79 ns |  0.81 |    0.03 |    3 |         - |          NA |
| PDQSortBranchless            | 256  | ManyDuplicates     |     4,888.3 ns |    271.59 ns |    120.59 ns |  0.55 |    0.02 |    2 |         - |          NA |
| Ipnsort                      | 256  | ManyDuplicates     |     4,870.5 ns |    453.25 ns |    237.06 ns |  0.55 |    0.03 |    2 |         - |          NA |
| StdSort                      | 256  | ManyDuplicates     |     5,381.4 ns |    194.09 ns |     86.18 ns |  0.60 |    0.02 |    2 |         - |          NA |
| BlockQuickSort               | 256  | ManyDuplicates     |     5,615.6 ns |    540.75 ns |    240.10 ns |  0.63 |    0.03 |    2 |         - |          NA |
| Ips4oSort                    | 256  | ManyDuplicates     |     9,997.6 ns |    312.87 ns |    163.64 ns |  1.12 |    0.03 |    3 |         - |          NA |
| FluxSort                     | 256  | ManyDuplicates     |     7,784.0 ns |    328.89 ns |    172.02 ns |  0.87 |    0.03 |    3 |         - |          NA |
| CrumSort                     | 256  | ManyDuplicates     |     4,532.3 ns |    361.95 ns |    189.31 ns |  0.51 |    0.02 |    2 |         - |          NA |
| BlitSort                     | 256  | ManyDuplicates     |     7,883.6 ns |    295.62 ns |    154.61 ns |  0.88 |    0.03 |    3 |         - |          NA |
| DotnetSort                   | 256  | ManyDuplicates     |     7,184.8 ns |    309.12 ns |    161.68 ns |  0.80 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Random**             |    **53,457.9 ns** |  **1,316.63 ns** |    **688.62 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Random             |    56,799.7 ns |    735.27 ns |    384.56 ns |  1.06 |    0.01 |    3 |         - |          NA |
| QuickSortMedian3             | 1024 | Random             |    54,258.3 ns |    354.73 ns |    157.50 ns |  1.02 |    0.01 |    3 |         - |          NA |
| QuickSortMedian9             | 1024 | Random             |    57,868.8 ns |    224.93 ns |     99.87 ns |  1.08 |    0.01 |    3 |         - |          NA |
| SampleSort                   | 1024 | Random             |    66,632.6 ns |    652.27 ns |    341.15 ns |  1.25 |    0.02 |    3 |         - |          NA |
| SimdQuickSort                | 1024 | Random             |    14,700.8 ns |    280.45 ns |    146.68 ns |  0.28 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort           | 1024 | Random             |    39,679.7 ns |    354.40 ns |    185.36 ns |  0.74 |    0.01 |    2 |         - |          NA |
| StableQuickSort              | 1024 | Random             |   126,545.8 ns |    989.27 ns |    439.24 ns |  2.37 |    0.03 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Random             |    69,067.0 ns |    563.73 ns |    294.84 ns |  1.29 |    0.02 |    3 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Random             |   113,230.6 ns |    855.40 ns |    379.80 ns |  2.12 |    0.03 |    4 |         - |          NA |
| IntroSort                    | 1024 | Random             |    43,957.2 ns |    392.17 ns |    174.13 ns |  0.82 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 1024 | Random             |    41,964.1 ns |    251.67 ns |    111.74 ns |  0.79 |    0.01 |    2 |         - |          NA |
| PDQSort                      | 1024 | Random             |    39,594.3 ns |    200.28 ns |    104.75 ns |  0.74 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | Random             |    25,468.3 ns |    372.78 ns |    194.97 ns |  0.48 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 1024 | Random             |    24,597.0 ns |    369.30 ns |    193.15 ns |  0.46 |    0.01 |    2 |         - |          NA |
| StdSort                      | 1024 | Random             |    26,440.8 ns |    513.74 ns |    268.70 ns |  0.49 |    0.01 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | Random             |    28,270.9 ns |    349.81 ns |    155.32 ns |  0.53 |    0.01 |    2 |         - |          NA |
| Ips4oSort                    | 1024 | Random             |    25,974.4 ns |    287.02 ns |    127.44 ns |  0.49 |    0.01 |    2 |         - |          NA |
| FluxSort                     | 1024 | Random             |    33,059.0 ns |    247.44 ns |    129.42 ns |  0.62 |    0.01 |    2 |         - |          NA |
| CrumSort                     | 1024 | Random             |    32,874.4 ns |    291.45 ns |    129.40 ns |  0.62 |    0.01 |    2 |         - |          NA |
| BlitSort                     | 1024 | Random             |    34,104.6 ns |    216.53 ns |    113.25 ns |  0.64 |    0.01 |    2 |         - |          NA |
| DotnetSort                   | 1024 | Random             |    38,362.3 ns |    438.93 ns |    194.89 ns |  0.72 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **SingleElementMoved** |     **5,788.7 ns** |    **259.49 ns** |    **135.72 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 1024 | SingleElementMoved |    10,652.2 ns |    520.23 ns |    230.99 ns |  1.84 |    0.06 |    3 |         - |          NA |
| QuickSortMedian3             | 1024 | SingleElementMoved |     8,409.4 ns |    417.66 ns |    218.45 ns |  1.45 |    0.05 |    3 |         - |          NA |
| QuickSortMedian9             | 1024 | SingleElementMoved |     9,442.6 ns |    341.40 ns |    178.56 ns |  1.63 |    0.05 |    3 |         - |          NA |
| SampleSort                   | 1024 | SingleElementMoved |    34,609.3 ns |  1,341.18 ns |    701.47 ns |  5.98 |    0.18 |    6 |         - |          NA |
| SimdQuickSort                | 1024 | SingleElementMoved |    34,092.2 ns |    887.26 ns |    393.95 ns |  5.89 |    0.15 |    6 |         - |          NA |
| DualPivotQuickSort           | 1024 | SingleElementMoved |     4,996.6 ns |    579.08 ns |    302.87 ns |  0.86 |    0.05 |    2 |         - |          NA |
| StableQuickSort              | 1024 | SingleElementMoved |    45,067.2 ns |    444.88 ns |    232.68 ns |  7.79 |    0.18 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | SingleElementMoved |    20,718.4 ns |    304.81 ns |    159.42 ns |  3.58 |    0.08 |    5 |         - |          NA |
| DestswapStableQuickSort      | 1024 | SingleElementMoved |    44,681.4 ns |    730.45 ns |    382.04 ns |  7.72 |    0.18 |    7 |         - |          NA |
| IntroSort                    | 1024 | SingleElementMoved |    14,068.3 ns |    332.07 ns |    147.44 ns |  2.43 |    0.06 |    4 |         - |          NA |
| IntroSortDotnet              | 1024 | SingleElementMoved |     6,210.6 ns |    372.08 ns |    194.61 ns |  1.07 |    0.04 |    2 |         - |          NA |
| PDQSort                      | 1024 | SingleElementMoved |     4,176.9 ns |    328.55 ns |    171.84 ns |  0.72 |    0.03 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | SingleElementMoved |     4,840.3 ns |    341.95 ns |    178.85 ns |  0.84 |    0.03 |    2 |         - |          NA |
| Ipnsort                      | 1024 | SingleElementMoved |    18,895.0 ns |    232.00 ns |    121.34 ns |  3.27 |    0.08 |    5 |         - |          NA |
| StdSort                      | 1024 | SingleElementMoved |     4,166.2 ns |    333.77 ns |    174.57 ns |  0.72 |    0.03 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | SingleElementMoved |     9,930.9 ns |    466.92 ns |    244.21 ns |  1.72 |    0.06 |    3 |         - |          NA |
| Ips4oSort                    | 1024 | SingleElementMoved |    14,429.4 ns |     66.57 ns |     29.56 ns |  2.49 |    0.06 |    4 |         - |          NA |
| FluxSort                     | 1024 | SingleElementMoved |     1,762.4 ns |     44.66 ns |     19.83 ns |  0.30 |    0.01 |    1 |         - |          NA |
| CrumSort                     | 1024 | SingleElementMoved |     1,736.2 ns |    321.31 ns |    168.05 ns |  0.30 |    0.03 |    1 |         - |          NA |
| BlitSort                     | 1024 | SingleElementMoved |     1,707.9 ns |    250.72 ns |    111.32 ns |  0.30 |    0.02 |    1 |         - |          NA |
| DotnetSort                   | 1024 | SingleElementMoved |     6,826.2 ns |    387.13 ns |    202.48 ns |  1.18 |    0.04 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Sorted**             |     **4,349.6 ns** |    **290.85 ns** |    **152.12 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Sorted             |     8,310.6 ns |    702.94 ns |    367.65 ns |  1.91 |    0.10 |    6 |         - |          NA |
| QuickSortMedian3             | 1024 | Sorted             |     5,696.5 ns |    304.98 ns |    159.51 ns |  1.31 |    0.05 |    5 |         - |          NA |
| QuickSortMedian9             | 1024 | Sorted             |     6,054.1 ns |    336.50 ns |    176.00 ns |  1.39 |    0.06 |    5 |         - |          NA |
| SampleSort                   | 1024 | Sorted             |    19,565.8 ns |    934.33 ns |    488.67 ns |  4.50 |    0.18 |    7 |         - |          NA |
| SimdQuickSort                | 1024 | Sorted             |    35,569.1 ns |    315.06 ns |    164.78 ns |  8.19 |    0.27 |    8 |         - |          NA |
| DualPivotQuickSort           | 1024 | Sorted             |     4,742.0 ns |     47.18 ns |     16.82 ns |  1.09 |    0.04 |    5 |         - |          NA |
| StableQuickSort              | 1024 | Sorted             |    39,772.7 ns |    901.72 ns |    400.37 ns |  9.15 |    0.31 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Sorted             |    19,938.6 ns |    109.64 ns |     48.68 ns |  4.59 |    0.15 |    7 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Sorted             |    42,863.6 ns |    492.69 ns |    257.69 ns |  9.87 |    0.32 |    8 |         - |          NA |
| IntroSort                    | 1024 | Sorted             |     3,983.8 ns |     14.00 ns |      6.22 ns |  0.92 |    0.03 |    5 |         - |          NA |
| IntroSortDotnet              | 1024 | Sorted             |     4,682.4 ns |     35.86 ns |     12.79 ns |  1.08 |    0.04 |    5 |         - |          NA |
| PDQSort                      | 1024 | Sorted             |     1,176.4 ns |      5.48 ns |      1.95 ns |  0.27 |    0.01 |    3 |         - |          NA |
| PDQSortBranchless            | 1024 | Sorted             |     1,177.8 ns |     15.11 ns |      6.71 ns |  0.27 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 1024 | Sorted             |       599.5 ns |      7.21 ns |      3.20 ns |  0.14 |    0.00 |    1 |         - |          NA |
| StdSort                      | 1024 | Sorted             |     1,581.6 ns |     34.29 ns |     17.93 ns |  0.36 |    0.01 |    4 |         - |          NA |
| BlockQuickSort               | 1024 | Sorted             |     8,194.3 ns |    313.87 ns |    164.16 ns |  1.89 |    0.07 |    6 |         - |          NA |
| Ips4oSort                    | 1024 | Sorted             |       560.3 ns |     25.69 ns |     11.41 ns |  0.13 |    0.00 |    1 |         - |          NA |
| FluxSort                     | 1024 | Sorted             |       781.0 ns |     21.02 ns |      9.33 ns |  0.18 |    0.01 |    2 |         - |          NA |
| CrumSort                     | 1024 | Sorted             |       828.2 ns |     10.51 ns |      5.49 ns |  0.19 |    0.01 |    2 |         - |          NA |
| BlitSort                     | 1024 | Sorted             |       789.8 ns |      8.10 ns |      2.89 ns |  0.18 |    0.01 |    2 |         - |          NA |
| DotnetSort                   | 1024 | Sorted             |     4,131.2 ns |    263.20 ns |    137.66 ns |  0.95 |    0.04 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Reversed**           |     **4,798.3 ns** |    **355.74 ns** |    **186.06 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Reversed           |     9,224.6 ns |    625.91 ns |    327.36 ns |  1.92 |    0.09 |    7 |         - |          NA |
| QuickSortMedian3             | 1024 | Reversed           |     6,028.4 ns |    366.30 ns |    191.58 ns |  1.26 |    0.06 |    7 |         - |          NA |
| QuickSortMedian9             | 1024 | Reversed           |     6,390.1 ns |    296.34 ns |    154.99 ns |  1.33 |    0.06 |    7 |         - |          NA |
| SampleSort                   | 1024 | Reversed           |    21,890.4 ns |    524.10 ns |    274.11 ns |  4.57 |    0.17 |    8 |         - |          NA |
| SimdQuickSort                | 1024 | Reversed           |    22,093.9 ns |    286.20 ns |    149.69 ns |  4.61 |    0.17 |    8 |         - |          NA |
| DualPivotQuickSort           | 1024 | Reversed           |     4,619.4 ns |     21.90 ns |      7.81 ns |  0.96 |    0.03 |    6 |         - |          NA |
| StableQuickSort              | 1024 | Reversed           |    45,568.7 ns |    530.71 ns |    235.64 ns |  9.51 |    0.34 |    9 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Reversed           |    20,228.7 ns |    203.45 ns |     90.34 ns |  4.22 |    0.15 |    8 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Reversed           |    41,016.4 ns |  1,095.01 ns |    572.71 ns |  8.56 |    0.32 |    9 |         - |          NA |
| IntroSort                    | 1024 | Reversed           |     3,767.0 ns |     11.41 ns |      5.07 ns |  0.79 |    0.03 |    5 |         - |          NA |
| IntroSortDotnet              | 1024 | Reversed           |     7,209.7 ns |    442.33 ns |    231.35 ns |  1.50 |    0.07 |    7 |         - |          NA |
| PDQSort                      | 1024 | Reversed           |     2,139.9 ns |    309.15 ns |    137.26 ns |  0.45 |    0.03 |    3 |         - |          NA |
| PDQSortBranchless            | 1024 | Reversed           |     2,617.8 ns |     63.37 ns |     33.15 ns |  0.55 |    0.02 |    4 |         - |          NA |
| Ipnsort                      | 1024 | Reversed           |       950.2 ns |      7.03 ns |      3.12 ns |  0.20 |    0.01 |    1 |         - |          NA |
| StdSort                      | 1024 | Reversed           |     2,813.7 ns |    300.74 ns |    157.29 ns |  0.59 |    0.04 |    4 |         - |          NA |
| BlockQuickSort               | 1024 | Reversed           |    11,067.2 ns |    348.04 ns |    182.03 ns |  2.31 |    0.09 |    7 |         - |          NA |
| Ips4oSort                    | 1024 | Reversed           |       933.7 ns |     24.75 ns |     10.99 ns |  0.19 |    0.01 |    1 |         - |          NA |
| FluxSort                     | 1024 | Reversed           |     1,262.3 ns |    291.39 ns |    152.40 ns |  0.26 |    0.03 |    2 |         - |          NA |
| CrumSort                     | 1024 | Reversed           |     1,160.8 ns |     14.26 ns |      5.09 ns |  0.24 |    0.01 |    2 |         - |          NA |
| BlitSort                     | 1024 | Reversed           |     1,158.1 ns |     13.80 ns |      4.92 ns |  0.24 |    0.01 |    2 |         - |          NA |
| DotnetSort                   | 1024 | Reversed           |     8,326.1 ns |  1,022.64 ns |    534.86 ns |  1.74 |    0.12 |    7 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **PipeOrgan**          |   **107,808.6 ns** |    **555.28 ns** |    **246.55 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| QuickSort3way                | 1024 | PipeOrgan          |    20,548.6 ns |  2,099.81 ns |  1,098.24 ns |  0.19 |    0.01 |    3 |         - |          NA |
| QuickSortMedian3             | 1024 | PipeOrgan          |    15,639.6 ns |    528.78 ns |    276.56 ns |  0.15 |    0.00 |    2 |         - |          NA |
| QuickSortMedian9             | 1024 | PipeOrgan          |    11,891.5 ns |    419.52 ns |    186.27 ns |  0.11 |    0.00 |    2 |         - |          NA |
| SampleSort                   | 1024 | PipeOrgan          |    21,687.1 ns |    615.44 ns |    273.26 ns |  0.20 |    0.00 |    3 |         - |          NA |
| SimdQuickSort                | 1024 | PipeOrgan          |    37,610.5 ns |    731.40 ns |    324.75 ns |  0.35 |    0.00 |    4 |         - |          NA |
| DualPivotQuickSort           | 1024 | PipeOrgan          |    10,015.3 ns |    541.23 ns |    283.08 ns |  0.09 |    0.00 |    2 |         - |          NA |
| StableQuickSort              | 1024 | PipeOrgan          |    40,119.8 ns |    772.02 ns |    403.78 ns |  0.37 |    0.00 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | PipeOrgan          |    19,945.8 ns |    540.49 ns |    282.68 ns |  0.19 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 1024 | PipeOrgan          |    57,583.5 ns |    462.75 ns |    205.47 ns |  0.53 |    0.00 |    5 |         - |          NA |
| IntroSort                    | 1024 | PipeOrgan          |    26,158.1 ns |    240.04 ns |    125.55 ns |  0.24 |    0.00 |    3 |         - |          NA |
| IntroSortDotnet              | 1024 | PipeOrgan          |    14,329.0 ns |    286.10 ns |    127.03 ns |  0.13 |    0.00 |    2 |         - |          NA |
| PDQSort                      | 1024 | PipeOrgan          |     9,338.3 ns |    292.87 ns |    153.18 ns |  0.09 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | PipeOrgan          |    13,852.6 ns |    392.88 ns |    205.48 ns |  0.13 |    0.00 |    2 |         - |          NA |
| Ipnsort                      | 1024 | PipeOrgan          |    21,541.4 ns |    289.69 ns |    151.51 ns |  0.20 |    0.00 |    3 |         - |          NA |
| StdSort                      | 1024 | PipeOrgan          |    24,049.2 ns |    326.25 ns |    144.86 ns |  0.22 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 1024 | PipeOrgan          |    23,370.4 ns |    703.18 ns |    367.78 ns |  0.22 |    0.00 |    3 |         - |          NA |
| Ips4oSort                    | 1024 | PipeOrgan          |    15,146.1 ns |    155.69 ns |     81.43 ns |  0.14 |    0.00 |    2 |         - |          NA |
| FluxSort                     | 1024 | PipeOrgan          |     2,941.5 ns |     19.93 ns |      7.11 ns |  0.03 |    0.00 |    1 |         - |          NA |
| CrumSort                     | 1024 | PipeOrgan          |     3,016.0 ns |     45.39 ns |     20.16 ns |  0.03 |    0.00 |    1 |         - |          NA |
| BlitSort                     | 1024 | PipeOrgan          |     2,991.7 ns |     23.73 ns |     10.53 ns |  0.03 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 1024 | PipeOrgan          |    16,549.2 ns |    444.94 ns |    197.55 ns |  0.15 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **ManyDuplicates**     |    **34,367.6 ns** |    **444.52 ns** |    **197.37 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 1024 | ManyDuplicates     |    26,160.9 ns |    491.95 ns |    257.30 ns |  0.76 |    0.01 |    4 |         - |          NA |
| QuickSortMedian3             | 1024 | ManyDuplicates     |    39,107.2 ns |    358.98 ns |    187.75 ns |  1.14 |    0.01 |    4 |         - |          NA |
| QuickSortMedian9             | 1024 | ManyDuplicates     |    40,396.4 ns |    192.45 ns |    100.66 ns |  1.18 |    0.01 |    4 |         - |          NA |
| SampleSort                   | 1024 | ManyDuplicates     |    50,783.0 ns |    467.19 ns |    244.35 ns |  1.48 |    0.01 |    4 |         - |          NA |
| SimdQuickSort                | 1024 | ManyDuplicates     |    15,161.4 ns |    350.72 ns |    125.07 ns |  0.44 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort           | 1024 | ManyDuplicates     |    26,572.6 ns |    445.51 ns |    233.01 ns |  0.77 |    0.01 |    4 |         - |          NA |
| StableQuickSort              | 1024 | ManyDuplicates     |    59,523.3 ns |  1,082.86 ns |    566.35 ns |  1.73 |    0.02 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | ManyDuplicates     |    26,756.8 ns |    271.12 ns |    141.80 ns |  0.78 |    0.01 |    4 |         - |          NA |
| DestswapStableQuickSort      | 1024 | ManyDuplicates     |    46,066.0 ns |    255.04 ns |    133.39 ns |  1.34 |    0.01 |    4 |         - |          NA |
| IntroSort                    | 1024 | ManyDuplicates     |    31,977.4 ns |    283.62 ns |    148.34 ns |  0.93 |    0.01 |    4 |         - |          NA |
| IntroSortDotnet              | 1024 | ManyDuplicates     |    32,744.6 ns |    168.72 ns |     74.91 ns |  0.95 |    0.01 |    4 |         - |          NA |
| PDQSort                      | 1024 | ManyDuplicates     |    26,257.6 ns |    396.14 ns |    207.19 ns |  0.76 |    0.01 |    4 |         - |          NA |
| PDQSortBranchless            | 1024 | ManyDuplicates     |    12,307.9 ns |    246.77 ns |    109.57 ns |  0.36 |    0.00 |    1 |         - |          NA |
| Ipnsort                      | 1024 | ManyDuplicates     |    19,656.7 ns |    224.80 ns |     99.81 ns |  0.57 |    0.00 |    3 |         - |          NA |
| StdSort                      | 1024 | ManyDuplicates     |    19,456.8 ns |    308.22 ns |    161.21 ns |  0.57 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 1024 | ManyDuplicates     |    20,940.1 ns |    294.37 ns |    130.70 ns |  0.61 |    0.00 |    3 |         - |          NA |
| Ips4oSort                    | 1024 | ManyDuplicates     |    15,566.3 ns |    158.27 ns |     82.78 ns |  0.45 |    0.00 |    2 |         - |          NA |
| FluxSort                     | 1024 | ManyDuplicates     |    31,979.9 ns |    310.11 ns |    137.69 ns |  0.93 |    0.01 |    4 |         - |          NA |
| CrumSort                     | 1024 | ManyDuplicates     |    31,875.4 ns |    238.75 ns |    106.01 ns |  0.93 |    0.01 |    4 |         - |          NA |
| BlitSort                     | 1024 | ManyDuplicates     |    32,762.7 ns |    300.04 ns |    156.92 ns |  0.95 |    0.01 |    4 |         - |          NA |
| DotnetSort                   | 1024 | ManyDuplicates     |    30,109.8 ns |    158.78 ns |     83.04 ns |  0.88 |    0.01 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Random**             |   **239,753.5 ns** |  **2,547.47 ns** |  **1,332.38 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Random             |   261,992.1 ns |  1,313.92 ns |    687.20 ns |  1.09 |    0.01 |    3 |         - |          NA |
| QuickSortMedian3             | 4096 | Random             |   249,553.7 ns |  1,899.02 ns |    993.23 ns |  1.04 |    0.01 |    3 |         - |          NA |
| QuickSortMedian9             | 4096 | Random             |   265,575.9 ns |  1,073.78 ns |    561.61 ns |  1.11 |    0.01 |    3 |         - |          NA |
| SampleSort                   | 4096 | Random             |   302,414.9 ns |  2,498.63 ns |  1,109.41 ns |  1.26 |    0.01 |    3 |         - |          NA |
| SimdQuickSort                | 4096 | Random             |    79,604.0 ns |    729.24 ns |    323.79 ns |  0.33 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort           | 4096 | Random             |   192,704.3 ns |    640.40 ns |    284.34 ns |  0.80 |    0.00 |    3 |         - |          NA |
| StableQuickSort              | 4096 | Random             |   602,868.8 ns |  1,493.59 ns |    663.16 ns |  2.51 |    0.01 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Random             |   316,738.8 ns |  1,158.92 ns |    514.57 ns |  1.32 |    0.01 |    3 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Random             |   507,905.0 ns |  1,520.29 ns |    795.14 ns |  2.12 |    0.01 |    4 |         - |          NA |
| IntroSort                    | 4096 | Random             |   206,966.3 ns |  1,099.94 ns |    488.38 ns |  0.86 |    0.00 |    3 |         - |          NA |
| IntroSortDotnet              | 4096 | Random             |   201,032.2 ns |    582.26 ns |    258.53 ns |  0.84 |    0.00 |    3 |         - |          NA |
| PDQSort                      | 4096 | Random             |   192,746.3 ns |  2,635.99 ns |  1,378.68 ns |  0.80 |    0.01 |    3 |         - |          NA |
| PDQSortBranchless            | 4096 | Random             |   109,348.4 ns |  2,487.74 ns |  1,104.57 ns |  0.46 |    0.00 |    2 |         - |          NA |
| Ipnsort                      | 4096 | Random             |   116,783.6 ns |    539.72 ns |    192.47 ns |  0.49 |    0.00 |    2 |         - |          NA |
| StdSort                      | 4096 | Random             |   116,122.6 ns |    522.90 ns |    186.47 ns |  0.48 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 4096 | Random             |   124,312.1 ns |  1,639.63 ns |    857.56 ns |  0.52 |    0.00 |    2 |         - |          NA |
| Ips4oSort                    | 4096 | Random             |   108,588.0 ns |  2,051.45 ns |  1,072.95 ns |  0.45 |    0.00 |    2 |         - |          NA |
| FluxSort                     | 4096 | Random             |   141,992.3 ns |  1,488.22 ns |    660.78 ns |  0.59 |    0.00 |    2 |         - |          NA |
| CrumSort                     | 4096 | Random             |   141,158.0 ns |    606.23 ns |    269.17 ns |  0.59 |    0.00 |    2 |         - |          NA |
| BlitSort                     | 4096 | Random             |   151,848.9 ns |  1,190.43 ns |    622.62 ns |  0.63 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 4096 | Random             |   186,656.7 ns |  2,070.67 ns |  1,083.00 ns |  0.78 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **SingleElementMoved** |    **25,405.9 ns** |    **972.83 ns** |    **508.81 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 4096 | SingleElementMoved |    43,435.7 ns |  3,194.66 ns |  1,670.87 ns |  1.71 |    0.07 |    4 |         - |          NA |
| QuickSortMedian3             | 4096 | SingleElementMoved |    34,304.6 ns |    281.78 ns |    147.38 ns |  1.35 |    0.03 |    4 |         - |          NA |
| QuickSortMedian9             | 4096 | SingleElementMoved |    39,336.3 ns |    582.23 ns |    304.52 ns |  1.55 |    0.03 |    4 |         - |          NA |
| SampleSort                   | 4096 | SingleElementMoved |   167,717.6 ns |  1,219.77 ns |    541.58 ns |  6.60 |    0.13 |    7 |         - |          NA |
| SimdQuickSort                | 4096 | SingleElementMoved |   500,918.1 ns |  2,745.55 ns |  1,435.98 ns | 19.72 |    0.38 |    8 |         - |          NA |
| DualPivotQuickSort           | 4096 | SingleElementMoved |    21,776.3 ns |  1,943.59 ns |  1,016.53 ns |  0.86 |    0.04 |    4 |         - |          NA |
| StableQuickSort              | 4096 | SingleElementMoved |   216,932.7 ns |    920.33 ns |    408.63 ns |  8.54 |    0.16 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | SingleElementMoved |    92,084.4 ns |    170.25 ns |     60.71 ns |  3.63 |    0.07 |    6 |         - |          NA |
| DestswapStableQuickSort      | 4096 | SingleElementMoved |   192,354.6 ns |  1,599.64 ns |    710.25 ns |  7.57 |    0.15 |    7 |         - |          NA |
| IntroSort                    | 4096 | SingleElementMoved |    65,590.9 ns |    345.97 ns |    153.61 ns |  2.58 |    0.05 |    5 |         - |          NA |
| IntroSortDotnet              | 4096 | SingleElementMoved |    27,195.8 ns |    281.78 ns |    125.11 ns |  1.07 |    0.02 |    4 |         - |          NA |
| PDQSort                      | 4096 | SingleElementMoved |    15,494.5 ns |    301.40 ns |    157.64 ns |  0.61 |    0.01 |    3 |         - |          NA |
| PDQSortBranchless            | 4096 | SingleElementMoved |    17,829.8 ns |    172.97 ns |     90.47 ns |  0.70 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 4096 | SingleElementMoved |    91,062.9 ns |    529.65 ns |    235.17 ns |  3.59 |    0.07 |    6 |         - |          NA |
| StdSort                      | 4096 | SingleElementMoved |    15,427.2 ns |    198.31 ns |    103.72 ns |  0.61 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 4096 | SingleElementMoved |    45,805.4 ns |    315.88 ns |    165.21 ns |  1.80 |    0.03 |    4 |         - |          NA |
| Ips4oSort                    | 4096 | SingleElementMoved |    64,887.4 ns |    165.91 ns |     73.67 ns |  2.55 |    0.05 |    5 |         - |          NA |
| FluxSort                     | 4096 | SingleElementMoved |     5,680.1 ns |    337.13 ns |    176.33 ns |  0.22 |    0.01 |    1 |         - |          NA |
| CrumSort                     | 4096 | SingleElementMoved |     7,378.0 ns |    332.06 ns |    173.67 ns |  0.29 |    0.01 |    2 |         - |          NA |
| BlitSort                     | 4096 | SingleElementMoved |     7,388.3 ns |    326.46 ns |    170.75 ns |  0.29 |    0.01 |    2 |         - |          NA |
| DotnetSort                   | 4096 | SingleElementMoved |    31,826.5 ns |    807.31 ns |    422.24 ns |  1.25 |    0.03 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Sorted**             |    **20,223.1 ns** |    **349.03 ns** |    **154.97 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Sorted             |    35,984.8 ns |  2,289.29 ns |  1,197.34 ns |  1.78 |    0.06 |    6 |         - |          NA |
| QuickSortMedian3             | 4096 | Sorted             |    25,755.2 ns |    360.35 ns |    160.00 ns |  1.27 |    0.01 |    5 |         - |          NA |
| QuickSortMedian9             | 4096 | Sorted             |    27,034.4 ns |    219.64 ns |    114.88 ns |  1.34 |    0.01 |    5 |         - |          NA |
| SampleSort                   | 4096 | Sorted             |    90,336.9 ns |  3,003.90 ns |  1,571.10 ns |  4.47 |    0.08 |    7 |         - |          NA |
| SimdQuickSort                | 4096 | Sorted             |   546,405.8 ns |  2,978.63 ns |  1,557.88 ns | 27.02 |    0.21 |    9 |         - |          NA |
| DualPivotQuickSort           | 4096 | Sorted             |    22,763.3 ns |    671.69 ns |    298.24 ns |  1.13 |    0.02 |    5 |         - |          NA |
| StableQuickSort              | 4096 | Sorted             |   192,061.8 ns |  1,979.73 ns |  1,035.44 ns |  9.50 |    0.08 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Sorted             |    89,325.2 ns |  1,370.41 ns |    608.47 ns |  4.42 |    0.04 |    7 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Sorted             |   187,542.8 ns |    892.54 ns |    396.29 ns |  9.27 |    0.07 |    8 |         - |          NA |
| IntroSort                    | 4096 | Sorted             |    19,233.5 ns |     62.00 ns |     27.53 ns |  0.95 |    0.01 |    5 |         - |          NA |
| IntroSortDotnet              | 4096 | Sorted             |    22,227.6 ns |     90.50 ns |     40.18 ns |  1.10 |    0.01 |    5 |         - |          NA |
| PDQSort                      | 4096 | Sorted             |     4,561.8 ns |    384.36 ns |    170.66 ns |  0.23 |    0.01 |    3 |         - |          NA |
| PDQSortBranchless            | 4096 | Sorted             |     4,529.2 ns |    268.06 ns |    140.20 ns |  0.22 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 4096 | Sorted             |     2,303.1 ns |     68.83 ns |     30.56 ns |  0.11 |    0.00 |    1 |         - |          NA |
| StdSort                      | 4096 | Sorted             |     6,055.7 ns |    334.23 ns |    174.81 ns |  0.30 |    0.01 |    4 |         - |          NA |
| BlockQuickSort               | 4096 | Sorted             |    39,617.3 ns |    664.97 ns |    295.25 ns |  1.96 |    0.02 |    6 |         - |          NA |
| Ips4oSort                    | 4096 | Sorted             |     1,924.1 ns |     76.45 ns |     33.94 ns |  0.10 |    0.00 |    1 |         - |          NA |
| FluxSort                     | 4096 | Sorted             |     2,869.2 ns |     38.18 ns |     16.95 ns |  0.14 |    0.00 |    2 |         - |          NA |
| CrumSort                     | 4096 | Sorted             |     2,884.4 ns |     19.25 ns |      6.86 ns |  0.14 |    0.00 |    2 |         - |          NA |
| BlitSort                     | 4096 | Sorted             |     2,951.9 ns |    383.65 ns |    170.34 ns |  0.15 |    0.01 |    2 |         - |          NA |
| DotnetSort                   | 4096 | Sorted             |    19,640.4 ns |  1,402.94 ns |    622.92 ns |  0.97 |    0.03 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Reversed**           |    **22,544.0 ns** |    **763.19 ns** |    **399.16 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Reversed           |    40,470.4 ns |  2,364.19 ns |  1,236.52 ns |  1.80 |    0.06 |    5 |         - |          NA |
| QuickSortMedian3             | 4096 | Reversed           |    27,248.2 ns |  1,172.04 ns |    613.00 ns |  1.21 |    0.03 |    4 |         - |          NA |
| QuickSortMedian9             | 4096 | Reversed           |    28,212.4 ns |    450.16 ns |    199.87 ns |  1.25 |    0.02 |    4 |         - |          NA |
| SampleSort                   | 4096 | Reversed           |    99,140.1 ns |  3,064.32 ns |  1,602.70 ns |  4.40 |    0.10 |    7 |         - |          NA |
| SimdQuickSort                | 4096 | Reversed           |   242,058.0 ns |  1,595.42 ns |    708.38 ns | 10.74 |    0.19 |    9 |         - |          NA |
| DualPivotQuickSort           | 4096 | Reversed           |    21,130.1 ns |    705.51 ns |    313.25 ns |  0.94 |    0.02 |    4 |         - |          NA |
| StableQuickSort              | 4096 | Reversed           |   220,048.6 ns |  2,459.72 ns |  1,286.48 ns |  9.76 |    0.17 |    9 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Reversed           |    91,945.9 ns |  1,342.50 ns |    596.08 ns |  4.08 |    0.07 |    7 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Reversed           |   181,635.9 ns |  2,903.54 ns |  1,289.19 ns |  8.06 |    0.15 |    8 |         - |          NA |
| IntroSort                    | 4096 | Reversed           |    18,697.6 ns |    504.46 ns |    263.84 ns |  0.83 |    0.02 |    4 |         - |          NA |
| IntroSortDotnet              | 4096 | Reversed           |    34,476.9 ns |    338.65 ns |    177.12 ns |  1.53 |    0.03 |    5 |         - |          NA |
| PDQSort                      | 4096 | Reversed           |     8,252.0 ns |    354.05 ns |    185.17 ns |  0.37 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 4096 | Reversed           |    10,225.8 ns |    296.31 ns |    154.98 ns |  0.45 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 4096 | Reversed           |     3,710.6 ns |    337.41 ns |    176.47 ns |  0.16 |    0.01 |    1 |         - |          NA |
| StdSort                      | 4096 | Reversed           |    10,550.1 ns |    372.42 ns |    165.36 ns |  0.47 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 4096 | Reversed           |    49,068.9 ns |    384.66 ns |    201.18 ns |  2.18 |    0.04 |    6 |         - |          NA |
| Ips4oSort                    | 4096 | Reversed           |     3,592.4 ns |    427.25 ns |    223.46 ns |  0.16 |    0.01 |    1 |         - |          NA |
| FluxSort                     | 4096 | Reversed           |     4,376.5 ns |    252.10 ns |    111.93 ns |  0.19 |    0.01 |    1 |         - |          NA |
| CrumSort                     | 4096 | Reversed           |     4,307.4 ns |     25.73 ns |     11.42 ns |  0.19 |    0.00 |    1 |         - |          NA |
| BlitSort                     | 4096 | Reversed           |     4,498.6 ns |    486.08 ns |    254.23 ns |  0.20 |    0.01 |    1 |         - |          NA |
| DotnetSort                   | 4096 | Reversed           |    35,646.0 ns |  2,710.56 ns |  1,417.68 ns |  1.58 |    0.07 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **PipeOrgan**          | **1,570,329.9 ns** | **10,888.64 ns** |  **5,694.97 ns** | **1.000** |    **0.00** |    **7** |         **-** |          **NA** |
| QuickSort3way                | 4096 | PipeOrgan          |   108,379.2 ns |  7,792.83 ns |  4,075.80 ns | 0.069 |    0.00 |    3 |         - |          NA |
| QuickSortMedian3             | 4096 | PipeOrgan          |    79,695.5 ns |  3,217.96 ns |  1,683.06 ns | 0.051 |    0.00 |    3 |         - |          NA |
| QuickSortMedian9             | 4096 | PipeOrgan          |    53,039.0 ns |  1,324.69 ns |    692.84 ns | 0.034 |    0.00 |    2 |         - |          NA |
| SampleSort                   | 4096 | PipeOrgan          |   100,644.7 ns |  4,714.20 ns |  2,465.62 ns | 0.064 |    0.00 |    3 |         - |          NA |
| SimdQuickSort                | 4096 | PipeOrgan          |   242,695.9 ns |  1,608.92 ns |    714.37 ns | 0.155 |    0.00 |    6 |         - |          NA |
| DualPivotQuickSort           | 4096 | PipeOrgan          |    46,983.9 ns |    692.66 ns |    362.27 ns | 0.030 |    0.00 |    2 |         - |          NA |
| StableQuickSort              | 4096 | PipeOrgan          |   192,847.5 ns |  3,112.69 ns |  1,628.00 ns | 0.123 |    0.00 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | PipeOrgan          |    89,738.9 ns |  2,459.96 ns |  1,092.24 ns | 0.057 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 4096 | PipeOrgan          |   258,198.1 ns |    894.50 ns |    397.16 ns | 0.164 |    0.00 |    6 |         - |          NA |
| IntroSort                    | 4096 | PipeOrgan          |   135,192.9 ns |    767.92 ns |    340.96 ns | 0.086 |    0.00 |    4 |         - |          NA |
| IntroSortDotnet              | 4096 | PipeOrgan          |    86,050.1 ns |  6,055.33 ns |  2,688.60 ns | 0.055 |    0.00 |    3 |         - |          NA |
| PDQSort                      | 4096 | PipeOrgan          |    43,831.3 ns |    705.13 ns |    368.79 ns | 0.028 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 4096 | PipeOrgan          |    64,122.4 ns |    765.22 ns |    400.22 ns | 0.041 |    0.00 |    3 |         - |          NA |
| Ipnsort                      | 4096 | PipeOrgan          |   107,810.5 ns |    754.90 ns |    394.83 ns | 0.069 |    0.00 |    3 |         - |          NA |
| StdSort                      | 4096 | PipeOrgan          |   132,960.6 ns |    853.00 ns |    378.74 ns | 0.085 |    0.00 |    4 |         - |          NA |
| BlockQuickSort               | 4096 | PipeOrgan          |   105,312.8 ns |  1,283.80 ns |    671.45 ns | 0.067 |    0.00 |    3 |         - |          NA |
| Ips4oSort                    | 4096 | PipeOrgan          |    66,828.4 ns |    642.45 ns |    336.01 ns | 0.043 |    0.00 |    3 |         - |          NA |
| FluxSort                     | 4096 | PipeOrgan          |    11,743.7 ns |    202.70 ns |     90.00 ns | 0.007 |    0.00 |    1 |         - |          NA |
| CrumSort                     | 4096 | PipeOrgan          |    13,724.5 ns |    249.74 ns |    110.89 ns | 0.009 |    0.00 |    1 |         - |          NA |
| BlitSort                     | 4096 | PipeOrgan          |    13,771.0 ns |    291.92 ns |    152.68 ns | 0.009 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 4096 | PipeOrgan          |    91,997.9 ns |  2,456.20 ns |  1,090.57 ns | 0.059 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **ManyDuplicates**     |   **133,754.6 ns** |    **284.35 ns** |    **126.25 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 4096 | ManyDuplicates     |    96,927.7 ns |    726.10 ns |    258.94 ns |  0.72 |    0.00 |    2 |         - |          NA |
| QuickSortMedian3             | 4096 | ManyDuplicates     |   146,958.5 ns |  1,269.10 ns |    663.76 ns |  1.10 |    0.00 |    3 |         - |          NA |
| QuickSortMedian9             | 4096 | ManyDuplicates     |   152,950.7 ns |  1,213.57 ns |    538.83 ns |  1.14 |    0.00 |    3 |         - |          NA |
| SampleSort                   | 4096 | ManyDuplicates     |   200,149.9 ns |  2,003.82 ns |  1,048.04 ns |  1.50 |    0.01 |    3 |         - |          NA |
| SimdQuickSort                | 4096 | ManyDuplicates     |    82,241.4 ns |    994.38 ns |    520.08 ns |  0.61 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort           | 4096 | ManyDuplicates     |    99,562.9 ns |    633.57 ns |    281.31 ns |  0.74 |    0.00 |    2 |         - |          NA |
| StableQuickSort              | 4096 | ManyDuplicates     |   231,555.6 ns |    867.70 ns |    453.83 ns |  1.73 |    0.00 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | ManyDuplicates     |    99,147.3 ns |    372.70 ns |    165.48 ns |  0.74 |    0.00 |    2 |         - |          NA |
| DestswapStableQuickSort      | 4096 | ManyDuplicates     |   170,281.4 ns |    368.85 ns |    163.77 ns |  1.27 |    0.00 |    3 |         - |          NA |
| IntroSort                    | 4096 | ManyDuplicates     |   125,569.0 ns |  1,455.04 ns |    761.02 ns |  0.94 |    0.01 |    3 |         - |          NA |
| IntroSortDotnet              | 4096 | ManyDuplicates     |   133,300.5 ns |    914.17 ns |    478.13 ns |  1.00 |    0.00 |    3 |         - |          NA |
| PDQSort                      | 4096 | ManyDuplicates     |    96,240.9 ns |    860.98 ns |    382.28 ns |  0.72 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 4096 | ManyDuplicates     |    35,042.0 ns |    679.24 ns |    355.26 ns |  0.26 |    0.00 |    1 |         - |          NA |
| Ipnsort                      | 4096 | ManyDuplicates     |    62,774.2 ns |    222.32 ns |     98.71 ns |  0.47 |    0.00 |    2 |         - |          NA |
| StdSort                      | 4096 | ManyDuplicates     |    54,755.8 ns |    351.56 ns |    183.87 ns |  0.41 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 4096 | ManyDuplicates     |    72,039.2 ns |    469.50 ns |    208.46 ns |  0.54 |    0.00 |    2 |         - |          NA |
| Ips4oSort                    | 4096 | ManyDuplicates     |    51,902.7 ns |  1,771.10 ns |    786.38 ns |  0.39 |    0.01 |    2 |         - |          NA |
| FluxSort                     | 4096 | ManyDuplicates     |    44,239.9 ns |    520.16 ns |    230.95 ns |  0.33 |    0.00 |    2 |         - |          NA |
| CrumSort                     | 4096 | ManyDuplicates     |    45,176.8 ns |    227.92 ns |    119.21 ns |  0.34 |    0.00 |    2 |         - |          NA |
| BlitSort                     | 4096 | ManyDuplicates     |    54,930.9 ns |    492.66 ns |    257.67 ns |  0.41 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 4096 | ManyDuplicates     |   122,089.9 ns |    978.19 ns |    434.32 ns |  0.91 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Random**             |   **510,328.2 ns** |  **3,154.58 ns** |  **1,649.91 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Random             |   559,808.5 ns |  6,853.12 ns |  3,584.31 ns |  1.10 |    0.01 |    3 |         - |          NA |
| QuickSortMedian3             | 8192 | Random             |   529,168.0 ns |  2,328.80 ns |  1,218.01 ns |  1.04 |    0.00 |    3 |         - |          NA |
| QuickSortMedian9             | 8192 | Random             |   563,157.6 ns |  3,077.04 ns |  1,609.35 ns |  1.10 |    0.00 |    3 |         - |          NA |
| SampleSort                   | 8192 | Random             |   642,732.8 ns |  2,511.03 ns |  1,114.91 ns |  1.26 |    0.00 |    3 |         - |          NA |
| SimdQuickSort                | 8192 | Random             |   176,185.6 ns |    787.13 ns |    411.68 ns |  0.35 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort           | 8192 | Random             |   419,265.2 ns |  2,247.18 ns |  1,175.32 ns |  0.82 |    0.00 |    3 |         - |          NA |
| StableQuickSort              | 8192 | Random             | 1,302,780.8 ns |  4,489.93 ns |  1,993.56 ns |  2.55 |    0.01 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Random             |   670,081.9 ns |  1,935.65 ns |    859.44 ns |  1.31 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Random             | 1,073,940.2 ns |  6,235.59 ns |  2,768.64 ns |  2.10 |    0.01 |    4 |         - |          NA |
| IntroSort                    | 8192 | Random             |   445,349.0 ns |  1,893.27 ns |    840.62 ns |  0.87 |    0.00 |    3 |         - |          NA |
| IntroSortDotnet              | 8192 | Random             |   436,111.5 ns |  1,408.50 ns |    625.38 ns |  0.85 |    0.00 |    3 |         - |          NA |
| PDQSort                      | 8192 | Random             |   415,598.7 ns |  1,038.36 ns |    543.08 ns |  0.81 |    0.00 |    3 |         - |          NA |
| PDQSortBranchless            | 8192 | Random             |   225,495.1 ns |    366.61 ns |    162.78 ns |  0.44 |    0.00 |    2 |         - |          NA |
| Ipnsort                      | 8192 | Random             |   251,667.5 ns |  1,308.20 ns |    580.85 ns |  0.49 |    0.00 |    2 |         - |          NA |
| StdSort                      | 8192 | Random             |   242,933.1 ns |    544.47 ns |    241.75 ns |  0.48 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | Random             |   259,124.5 ns |  1,299.29 ns |    679.55 ns |  0.51 |    0.00 |    2 |         - |          NA |
| Ips4oSort                    | 8192 | Random             |   240,786.1 ns |  2,856.51 ns |  1,494.01 ns |  0.47 |    0.00 |    2 |         - |          NA |
| FluxSort                     | 8192 | Random             |   290,459.1 ns |    497.38 ns |    260.14 ns |  0.57 |    0.00 |    2 |         - |          NA |
| CrumSort                     | 8192 | Random             |   293,026.5 ns |  1,117.04 ns |    584.24 ns |  0.57 |    0.00 |    2 |         - |          NA |
| BlitSort                     | 8192 | Random             |   323,899.4 ns |  1,572.45 ns |    822.42 ns |  0.63 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 8192 | Random             |   406,018.6 ns |  4,286.79 ns |  2,242.07 ns |  0.80 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **SingleElementMoved** |    **52,684.7 ns** |  **1,232.23 ns** |    **644.48 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 8192 | SingleElementMoved |    91,338.9 ns |  6,459.49 ns |  3,378.44 ns |  1.73 |    0.06 |    4 |         - |          NA |
| QuickSortMedian3             | 8192 | SingleElementMoved |    70,922.1 ns |  3,250.67 ns |  1,443.32 ns |  1.35 |    0.03 |    4 |         - |          NA |
| QuickSortMedian9             | 8192 | SingleElementMoved |    81,275.5 ns |    696.55 ns |    309.27 ns |  1.54 |    0.02 |    4 |         - |          NA |
| SampleSort                   | 8192 | SingleElementMoved |   414,920.7 ns |  2,974.60 ns |  1,320.74 ns |  7.88 |    0.09 |    7 |         - |          NA |
| SimdQuickSort                | 8192 | SingleElementMoved | 1,933,670.0 ns |  6,149.93 ns |  3,216.53 ns | 36.71 |    0.43 |    8 |         - |          NA |
| DualPivotQuickSort           | 8192 | SingleElementMoved |    50,126.4 ns |  3,378.19 ns |  1,766.86 ns |  0.95 |    0.03 |    4 |         - |          NA |
| StableQuickSort              | 8192 | SingleElementMoved |   474,233.8 ns |  7,914.20 ns |  4,139.28 ns |  9.00 |    0.13 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | SingleElementMoved |   194,786.7 ns |    942.72 ns |    418.57 ns |  3.70 |    0.04 |    6 |         - |          NA |
| DestswapStableQuickSort      | 8192 | SingleElementMoved |   401,593.7 ns |  3,907.96 ns |  2,043.94 ns |  7.62 |    0.10 |    7 |         - |          NA |
| IntroSort                    | 8192 | SingleElementMoved |   137,371.8 ns |  1,170.92 ns |    612.41 ns |  2.61 |    0.03 |    5 |         - |          NA |
| IntroSortDotnet              | 8192 | SingleElementMoved |    56,992.8 ns |    714.48 ns |    317.23 ns |  1.08 |    0.01 |    4 |         - |          NA |
| PDQSort                      | 8192 | SingleElementMoved |    30,803.3 ns |    406.38 ns |    180.44 ns |  0.58 |    0.01 |    3 |         - |          NA |
| PDQSortBranchless            | 8192 | SingleElementMoved |    35,235.3 ns |    571.81 ns |    203.91 ns |  0.67 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 8192 | SingleElementMoved |   198,917.6 ns |  1,939.63 ns |  1,014.46 ns |  3.78 |    0.05 |    6 |         - |          NA |
| StdSort                      | 8192 | SingleElementMoved |    30,008.1 ns |    265.35 ns |    138.78 ns |  0.57 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 8192 | SingleElementMoved |    94,941.7 ns |    242.36 ns |    107.61 ns |  1.80 |    0.02 |    4 |         - |          NA |
| Ips4oSort                    | 8192 | SingleElementMoved |   127,461.7 ns |    644.49 ns |    286.16 ns |  2.42 |    0.03 |    5 |         - |          NA |
| FluxSort                     | 8192 | SingleElementMoved |    10,787.7 ns |    172.49 ns |     61.51 ns |  0.20 |    0.00 |    1 |         - |          NA |
| CrumSort                     | 8192 | SingleElementMoved |    14,561.2 ns |    206.88 ns |    108.20 ns |  0.28 |    0.00 |    2 |         - |          NA |
| BlitSort                     | 8192 | SingleElementMoved |    14,443.3 ns |     61.82 ns |     27.45 ns |  0.27 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 8192 | SingleElementMoved |    68,030.2 ns |    461.13 ns |    204.74 ns |  1.29 |    0.02 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Sorted**             |    **43,263.1 ns** |    **415.03 ns** |    **184.28 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Sorted             |    75,745.7 ns |  3,284.61 ns |  1,717.92 ns |  1.75 |    0.04 |    6 |         - |          NA |
| QuickSortMedian3             | 8192 | Sorted             |    54,272.2 ns |    338.51 ns |    177.05 ns |  1.25 |    0.01 |    5 |         - |          NA |
| QuickSortMedian9             | 8192 | Sorted             |    56,849.0 ns |    634.73 ns |    281.82 ns |  1.31 |    0.01 |    5 |         - |          NA |
| SampleSort                   | 8192 | Sorted             |   262,697.7 ns | 21,792.65 ns | 11,397.98 ns |  6.07 |    0.25 |    8 |         - |          NA |
| SimdQuickSort                | 8192 | Sorted             | 2,142,962.4 ns |  5,173.02 ns |  2,705.59 ns | 49.53 |    0.21 |   10 |         - |          NA |
| DualPivotQuickSort           | 8192 | Sorted             |    52,874.6 ns |    799.83 ns |    355.13 ns |  1.22 |    0.01 |    5 |         - |          NA |
| StableQuickSort              | 8192 | Sorted             |   417,386.0 ns |  3,301.18 ns |  1,726.58 ns |  9.65 |    0.05 |    9 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Sorted             |   190,379.5 ns |    766.68 ns |    340.41 ns |  4.40 |    0.02 |    7 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Sorted             |   391,933.0 ns |  1,556.81 ns |    814.24 ns |  9.06 |    0.04 |    9 |         - |          NA |
| IntroSort                    | 8192 | Sorted             |    41,553.4 ns |    368.83 ns |    192.90 ns |  0.96 |    0.01 |    5 |         - |          NA |
| IntroSortDotnet              | 8192 | Sorted             |    47,323.3 ns |    352.98 ns |    184.62 ns |  1.09 |    0.01 |    5 |         - |          NA |
| PDQSort                      | 8192 | Sorted             |     8,937.9 ns |    500.48 ns |    222.21 ns |  0.21 |    0.00 |    3 |         - |          NA |
| PDQSortBranchless            | 8192 | Sorted             |     8,875.9 ns |    272.31 ns |    142.42 ns |  0.21 |    0.00 |    3 |         - |          NA |
| Ipnsort                      | 8192 | Sorted             |     4,655.9 ns |    287.06 ns |    150.14 ns |  0.11 |    0.00 |    1 |         - |          NA |
| StdSort                      | 8192 | Sorted             |    12,029.5 ns |    353.47 ns |    184.87 ns |  0.28 |    0.00 |    4 |         - |          NA |
| BlockQuickSort               | 8192 | Sorted             |    86,029.1 ns |  1,182.79 ns |    525.16 ns |  1.99 |    0.01 |    6 |         - |          NA |
| Ips4oSort                    | 8192 | Sorted             |     3,976.5 ns |    333.95 ns |    174.66 ns |  0.09 |    0.00 |    1 |         - |          NA |
| FluxSort                     | 8192 | Sorted             |     5,773.0 ns |    495.65 ns |    259.24 ns |  0.13 |    0.01 |    2 |         - |          NA |
| CrumSort                     | 8192 | Sorted             |     5,684.9 ns |    227.67 ns |    119.07 ns |  0.13 |    0.00 |    2 |         - |          NA |
| BlitSort                     | 8192 | Sorted             |     5,682.4 ns |    245.30 ns |    128.30 ns |  0.13 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 8192 | Sorted             |    43,677.3 ns |  3,699.78 ns |  1,935.06 ns |  1.01 |    0.04 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Reversed**           |    **47,601.5 ns** |  **1,473.02 ns** |    **770.42 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Reversed           |    84,649.7 ns |  3,447.27 ns |  1,530.61 ns |  1.78 |    0.04 |    5 |         - |          NA |
| QuickSortMedian3             | 8192 | Reversed           |    56,422.3 ns |    439.42 ns |    195.11 ns |  1.19 |    0.02 |    4 |         - |          NA |
| QuickSortMedian9             | 8192 | Reversed           |    59,937.4 ns |    419.59 ns |    186.30 ns |  1.26 |    0.02 |    4 |         - |          NA |
| SampleSort                   | 8192 | Reversed           |   363,411.8 ns |  9,172.54 ns |  4,797.42 ns |  7.64 |    0.15 |    8 |         - |          NA |
| SimdQuickSort                | 8192 | Reversed           |   839,595.8 ns |  2,653.65 ns |  1,178.24 ns | 17.64 |    0.27 |   10 |         - |          NA |
| DualPivotQuickSort           | 8192 | Reversed           |    50,784.9 ns |  1,870.23 ns |    978.16 ns |  1.07 |    0.03 |    4 |         - |          NA |
| StableQuickSort              | 8192 | Reversed           |   475,752.2 ns |  2,892.75 ns |  1,284.40 ns | 10.00 |    0.15 |    9 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Reversed           |   240,784.4 ns |  4,860.16 ns |  2,157.94 ns |  5.06 |    0.09 |    7 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Reversed           |   381,521.3 ns |  1,092.06 ns |    484.88 ns |  8.02 |    0.12 |    8 |         - |          NA |
| IntroSort                    | 8192 | Reversed           |    40,362.7 ns |  1,144.66 ns |    598.68 ns |  0.85 |    0.02 |    4 |         - |          NA |
| IntroSortDotnet              | 8192 | Reversed           |    75,057.2 ns |    339.79 ns |    150.87 ns |  1.58 |    0.02 |    5 |         - |          NA |
| PDQSort                      | 8192 | Reversed           |    16,332.3 ns |    436.21 ns |    193.68 ns |  0.34 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | Reversed           |    19,910.9 ns |    377.88 ns |    167.78 ns |  0.42 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 8192 | Reversed           |     7,184.5 ns |    221.86 ns |    116.04 ns |  0.15 |    0.00 |    1 |         - |          NA |
| StdSort                      | 8192 | Reversed           |    20,571.2 ns |    466.72 ns |    244.10 ns |  0.43 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 8192 | Reversed           |   105,661.7 ns |    641.05 ns |    284.63 ns |  2.22 |    0.03 |    6 |         - |          NA |
| Ips4oSort                    | 8192 | Reversed           |     7,022.5 ns |    492.33 ns |    218.60 ns |  0.15 |    0.00 |    1 |         - |          NA |
| FluxSort                     | 8192 | Reversed           |     8,633.3 ns |    313.05 ns |    139.00 ns |  0.18 |    0.00 |    1 |         - |          NA |
| CrumSort                     | 8192 | Reversed           |     8,631.7 ns |    242.26 ns |    126.71 ns |  0.18 |    0.00 |    1 |         - |          NA |
| BlitSort                     | 8192 | Reversed           |     8,719.9 ns |    442.54 ns |    231.46 ns |  0.18 |    0.01 |    1 |         - |          NA |
| DotnetSort                   | 8192 | Reversed           |    79,249.5 ns |  4,319.04 ns |  2,258.94 ns |  1.67 |    0.05 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **PipeOrgan**          | **6,135,446.5 ns** | **52,276.57 ns** | **27,341.66 ns** | **1.000** |    **0.00** |    **7** |         **-** |          **NA** |
| QuickSort3way                | 8192 | PipeOrgan          |   253,390.1 ns | 12,454.49 ns |  6,513.94 ns | 0.041 |    0.00 |    5 |         - |          NA |
| QuickSortMedian3             | 8192 | PipeOrgan          |   191,043.1 ns |  4,984.77 ns |  2,607.13 ns | 0.031 |    0.00 |    5 |         - |          NA |
| QuickSortMedian9             | 8192 | PipeOrgan          |   112,362.8 ns |  3,626.03 ns |  1,896.48 ns | 0.018 |    0.00 |    3 |         - |          NA |
| SampleSort                   | 8192 | PipeOrgan          |   388,916.0 ns |  5,097.90 ns |  2,263.50 ns | 0.063 |    0.00 |    5 |         - |          NA |
| SimdQuickSort                | 8192 | PipeOrgan          |   560,411.7 ns |  3,181.84 ns |  1,134.68 ns | 0.091 |    0.00 |    6 |         - |          NA |
| DualPivotQuickSort           | 8192 | PipeOrgan          |   100,681.7 ns |  1,793.55 ns |    796.35 ns | 0.016 |    0.00 |    3 |         - |          NA |
| StableQuickSort              | 8192 | PipeOrgan          |   417,713.6 ns |  2,965.36 ns |  1,550.94 ns | 0.068 |    0.00 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | PipeOrgan          |   192,728.9 ns |  9,231.09 ns |  4,828.04 ns | 0.031 |    0.00 |    5 |         - |          NA |
| DestswapStableQuickSort      | 8192 | PipeOrgan          |   544,662.3 ns |  2,242.03 ns |  1,172.62 ns | 0.089 |    0.00 |    6 |         - |          NA |
| IntroSort                    | 8192 | PipeOrgan          |   296,001.8 ns |  1,250.72 ns |    446.02 ns | 0.048 |    0.00 |    5 |         - |          NA |
| IntroSortDotnet              | 8192 | PipeOrgan          |   351,117.0 ns | 11,120.32 ns |  5,816.14 ns | 0.057 |    0.00 |    5 |         - |          NA |
| PDQSort                      | 8192 | PipeOrgan          |    94,181.5 ns |  1,559.16 ns |    815.47 ns | 0.015 |    0.00 |    3 |         - |          NA |
| PDQSortBranchless            | 8192 | PipeOrgan          |   138,356.8 ns |  2,223.10 ns |  1,162.72 ns | 0.023 |    0.00 |    4 |         - |          NA |
| Ipnsort                      | 8192 | PipeOrgan          |   239,637.0 ns |    852.01 ns |    378.30 ns | 0.039 |    0.00 |    5 |         - |          NA |
| StdSort                      | 8192 | PipeOrgan          |   322,810.0 ns |  2,829.72 ns |  1,480.00 ns | 0.053 |    0.00 |    5 |         - |          NA |
| BlockQuickSort               | 8192 | PipeOrgan          |   228,750.3 ns |  1,842.27 ns |    963.54 ns | 0.037 |    0.00 |    5 |         - |          NA |
| Ips4oSort                    | 8192 | PipeOrgan          |   158,999.1 ns |  1,443.69 ns |    755.08 ns | 0.026 |    0.00 |    4 |         - |          NA |
| FluxSort                     | 8192 | PipeOrgan          |    23,285.0 ns |    353.82 ns |    185.05 ns | 0.004 |    0.00 |    1 |         - |          NA |
| CrumSort                     | 8192 | PipeOrgan          |    29,459.6 ns |    319.43 ns |    167.07 ns | 0.005 |    0.00 |    2 |         - |          NA |
| BlitSort                     | 8192 | PipeOrgan          |    29,448.9 ns |     75.21 ns |     33.40 ns | 0.005 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 8192 | PipeOrgan          |   362,448.9 ns |  7,189.62 ns |  3,192.24 ns | 0.059 |    0.00 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **ManyDuplicates**     |   **265,701.3 ns** |  **1,427.25 ns** |    **633.71 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 8192 | ManyDuplicates     |   190,367.9 ns |    127.19 ns |     56.47 ns |  0.72 |    0.00 |    3 |         - |          NA |
| QuickSortMedian3             | 8192 | ManyDuplicates     |   289,993.8 ns |  1,232.87 ns |    644.81 ns |  1.09 |    0.00 |    4 |         - |          NA |
| QuickSortMedian9             | 8192 | ManyDuplicates     |   300,232.5 ns |  1,781.96 ns |    791.20 ns |  1.13 |    0.00 |    4 |         - |          NA |
| SampleSort                   | 8192 | ManyDuplicates     |   401,737.6 ns |  2,861.50 ns |  1,496.62 ns |  1.51 |    0.01 |    5 |         - |          NA |
| SimdQuickSort                | 8192 | ManyDuplicates     | 1,073,094.4 ns |  1,906.80 ns |    997.29 ns |  4.04 |    0.01 |    6 |         - |          NA |
| DualPivotQuickSort           | 8192 | ManyDuplicates     |   195,829.7 ns |  1,556.16 ns |    813.90 ns |  0.74 |    0.00 |    3 |         - |          NA |
| StableQuickSort              | 8192 | ManyDuplicates     |   445,400.6 ns |  5,281.20 ns |  2,344.88 ns |  1.68 |    0.01 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | ManyDuplicates     |   193,473.1 ns |  1,177.34 ns |    615.77 ns |  0.73 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 8192 | ManyDuplicates     |   330,163.3 ns |  1,031.59 ns |    539.54 ns |  1.24 |    0.00 |    4 |         - |          NA |
| IntroSort                    | 8192 | ManyDuplicates     |   248,367.7 ns |  1,865.40 ns |    828.25 ns |  0.93 |    0.00 |    4 |         - |          NA |
| IntroSortDotnet              | 8192 | ManyDuplicates     |   267,905.7 ns |    360.14 ns |    159.90 ns |  1.01 |    0.00 |    4 |         - |          NA |
| PDQSort                      | 8192 | ManyDuplicates     |   188,439.1 ns |  1,308.58 ns |    684.41 ns |  0.71 |    0.00 |    3 |         - |          NA |
| PDQSortBranchless            | 8192 | ManyDuplicates     |    63,014.2 ns |    357.64 ns |    158.79 ns |  0.24 |    0.00 |    1 |         - |          NA |
| Ipnsort                      | 8192 | ManyDuplicates     |   120,561.2 ns |    496.13 ns |    176.92 ns |  0.45 |    0.00 |    2 |         - |          NA |
| StdSort                      | 8192 | ManyDuplicates     |   100,047.5 ns |    463.38 ns |    205.74 ns |  0.38 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | ManyDuplicates     |   141,242.1 ns |    792.75 ns |    351.99 ns |  0.53 |    0.00 |    2 |         - |          NA |
| Ips4oSort                    | 8192 | ManyDuplicates     |    78,042.4 ns |    607.73 ns |    269.83 ns |  0.29 |    0.00 |    1 |         - |          NA |
| FluxSort                     | 8192 | ManyDuplicates     |    73,924.1 ns |    397.40 ns |    176.45 ns |  0.28 |    0.00 |    1 |         - |          NA |
| CrumSort                     | 8192 | ManyDuplicates     |    73,091.7 ns |    479.29 ns |    212.81 ns |  0.28 |    0.00 |    1 |         - |          NA |
| BlitSort                     | 8192 | ManyDuplicates     |   103,264.2 ns |    611.69 ns |    271.60 ns |  0.39 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 8192 | ManyDuplicates     |   245,412.0 ns |  1,455.47 ns |    761.24 ns |  0.92 |    0.00 |    4 |         - |          NA |

### ScalingBenchmark

The table is not listed here because of its length. The results are on the benchmark site (category Scaling) and in the artifacts of the workflow run.

### SelectionBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.86GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.22 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                  | Size | Pattern            | Mean         | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------------ |----- |------------------- |-------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **SelectionSort**           | **256**  | **Random**             |  **33,379.8 ns** |    **315.60 ns** |    **165.06 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 256  | Random             |  11,779.6 ns |    421.94 ns |    187.34 ns |  0.35 |    0.01 |    2 |         - |          NA |
| DoubleSelectionSort     | 256  | Random             |  23,292.7 ns |    273.51 ns |     97.54 ns |  0.70 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 256  | Random             |  12,043.5 ns |    111.34 ns |     58.23 ns |  0.36 |    0.00 |    2 |         - |          NA |
| CycleSort               | 256  | Random             |  63,195.8 ns |    328.26 ns |    171.69 ns |  1.89 |    0.01 |    5 |         - |          NA |
| PancakeSort             | 256  | Random             |  42,145.5 ns |    364.25 ns |    190.51 ns |  1.26 |    0.01 |    4 |         - |          NA |
| BingoSort               | 256  | Random             |  37,509.7 ns |    367.52 ns |    192.22 ns |  1.12 |    0.01 |    4 |         - |          NA |
| DotnetSort              | 256  | Random             |   7,579.0 ns |    299.37 ns |    156.58 ns |  0.23 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**           | **256**  | **SingleElementMoved** |  **25,057.4 ns** |    **403.06 ns** |    **178.96 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| SelectionSortSimd       | 256  | SingleElementMoved |   6,194.0 ns |    246.28 ns |    109.35 ns |  0.25 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSort     | 256  | SingleElementMoved |  12,416.3 ns |    282.98 ns |    100.91 ns |  0.50 |    0.00 |    4 |         - |          NA |
| DoubleSelectionSortSimd | 256  | SingleElementMoved |   4,511.0 ns |    357.12 ns |    158.56 ns |  0.18 |    0.01 |    2 |         - |          NA |
| CycleSort               | 256  | SingleElementMoved |  35,028.3 ns |    234.58 ns |    122.69 ns |  1.40 |    0.01 |    7 |         - |          NA |
| PancakeSort             | 256  | SingleElementMoved |  20,509.5 ns |    367.00 ns |    162.95 ns |  0.82 |    0.01 |    5 |         - |          NA |
| BingoSort               | 256  | SingleElementMoved |  25,443.9 ns |    195.19 ns |    102.09 ns |  1.02 |    0.01 |    6 |         - |          NA |
| DotnetSort              | 256  | SingleElementMoved |   1,429.1 ns |     92.56 ns |     33.01 ns |  0.06 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**           | **256**  | **Sorted**             |  **24,566.1 ns** |    **252.65 ns** |    **112.18 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| SelectionSortSimd       | 256  | Sorted             |   4,400.9 ns |    101.07 ns |     44.88 ns |  0.18 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSort     | 256  | Sorted             |  10,441.0 ns |    253.31 ns |     90.33 ns |  0.43 |    0.00 |    4 |         - |          NA |
| DoubleSelectionSortSimd | 256  | Sorted             |   3,348.6 ns |    106.98 ns |     47.50 ns |  0.14 |    0.00 |    2 |         - |          NA |
| CycleSort               | 256  | Sorted             |  22,338.3 ns |    133.66 ns |     59.35 ns |  0.91 |    0.00 |    6 |         - |          NA |
| PancakeSort             | 256  | Sorted             |  16,862.8 ns |     57.97 ns |     20.67 ns |  0.69 |    0.00 |    5 |         - |          NA |
| BingoSort               | 256  | Sorted             |  24,705.1 ns |    191.53 ns |    100.17 ns |  1.01 |    0.01 |    6 |         - |          NA |
| DotnetSort              | 256  | Sorted             |     831.4 ns |     11.31 ns |      5.91 ns |  0.03 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**           | **256**  | **Reversed**           |  **21,942.1 ns** |    **779.30 ns** |    **346.01 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| SelectionSortSimd       | 256  | Reversed           |   7,269.7 ns |    313.25 ns |    163.84 ns |  0.33 |    0.01 |    3 |         - |          NA |
| DoubleSelectionSort     | 256  | Reversed           |  10,466.2 ns |    270.10 ns |    141.27 ns |  0.48 |    0.01 |    4 |         - |          NA |
| DoubleSelectionSortSimd | 256  | Reversed           |   5,625.5 ns |    471.53 ns |    246.62 ns |  0.26 |    0.01 |    2 |         - |          NA |
| CycleSort               | 256  | Reversed           |  41,102.1 ns |    331.68 ns |    173.48 ns |  1.87 |    0.03 |    8 |         - |          NA |
| PancakeSort             | 256  | Reversed           |  17,012.1 ns |    257.81 ns |    134.84 ns |  0.78 |    0.01 |    5 |         - |          NA |
| BingoSort               | 256  | Reversed           |  28,593.9 ns |  5,258.53 ns |  2,334.82 ns |  1.30 |    0.10 |    7 |         - |          NA |
| DotnetSort              | 256  | Reversed           |   1,498.1 ns |     42.51 ns |     22.23 ns |  0.07 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**           | **256**  | **PipeOrgan**          |  **23,774.4 ns** |    **685.20 ns** |    **358.37 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| SelectionSortSimd       | 256  | PipeOrgan          |   8,894.0 ns |    245.00 ns |    128.14 ns |  0.37 |    0.01 |    3 |         - |          NA |
| DoubleSelectionSort     | 256  | PipeOrgan          |  11,084.3 ns |    830.22 ns |    368.62 ns |  0.47 |    0.02 |    4 |         - |          NA |
| DoubleSelectionSortSimd | 256  | PipeOrgan          |   4,768.4 ns |    124.61 ns |     55.33 ns |  0.20 |    0.00 |    2 |         - |          NA |
| CycleSort               | 256  | PipeOrgan          |  55,307.0 ns |    208.20 ns |    108.89 ns |  2.33 |    0.03 |    6 |         - |          NA |
| PancakeSort             | 256  | PipeOrgan          |  26,622.5 ns |    230.86 ns |    120.75 ns |  1.12 |    0.02 |    5 |         - |          NA |
| BingoSort               | 256  | PipeOrgan          |  25,658.7 ns |  1,215.69 ns |    539.77 ns |  1.08 |    0.03 |    5 |         - |          NA |
| DotnetSort              | 256  | PipeOrgan          |   2,467.5 ns |    330.95 ns |    173.09 ns |  0.10 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**           | **256**  | **ManyDuplicates**     |  **31,214.3 ns** |    **141.59 ns** |     **74.05 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 256  | ManyDuplicates     |  11,641.8 ns |    588.08 ns |    307.57 ns |  0.37 |    0.01 |    2 |         - |          NA |
| DoubleSelectionSort     | 256  | ManyDuplicates     |  24,589.9 ns |    316.84 ns |    165.71 ns |  0.79 |    0.01 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 256  | ManyDuplicates     |  11,712.1 ns |    366.40 ns |    191.63 ns |  0.38 |    0.01 |    2 |         - |          NA |
| CycleSort               | 256  | ManyDuplicates     |  64,732.9 ns |    194.56 ns |     86.39 ns |  2.07 |    0.01 |    6 |         - |          NA |
| PancakeSort             | 256  | ManyDuplicates     |  39,434.9 ns |    195.56 ns |     86.83 ns |  1.26 |    0.00 |    5 |         - |          NA |
| BingoSort               | 256  | ManyDuplicates     |   7,785.5 ns |    361.85 ns |    189.25 ns |  0.25 |    0.01 |    1 |         - |          NA |
| DotnetSort              | 256  | ManyDuplicates     |   7,112.9 ns |    457.94 ns |    239.51 ns |  0.23 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**           | **1024** | **Random**             | **422,020.1 ns** |  **3,358.83 ns** |  **1,491.34 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 1024 | Random             |  81,591.7 ns |  2,473.93 ns |  1,098.44 ns |  0.19 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 1024 | Random             | 247,629.1 ns |    713.82 ns |    316.94 ns |  0.59 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 1024 | Random             |  68,356.1 ns |    735.51 ns |    384.69 ns |  0.16 |    0.00 |    2 |         - |          NA |
| CycleSort               | 1024 | Random             | 995,041.8 ns |  2,299.00 ns |  1,020.77 ns |  2.36 |    0.01 |    5 |         - |          NA |
| PancakeSort             | 1024 | Random             | 489,595.3 ns |  2,090.60 ns |    928.24 ns |  1.16 |    0.00 |    4 |         - |          NA |
| BingoSort               | 1024 | Random             | 438,429.9 ns |  1,002.61 ns |    445.16 ns |  1.04 |    0.00 |    4 |         - |          NA |
| DotnetSort              | 1024 | Random             |  38,644.7 ns |    426.29 ns |    222.96 ns |  0.09 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**           | **1024** | **SingleElementMoved** | **378,417.4 ns** |  **2,714.23 ns** |  **1,419.59 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| SelectionSortSimd       | 1024 | SingleElementMoved |  47,194.7 ns |    855.34 ns |    379.78 ns |  0.12 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSort     | 1024 | SingleElementMoved | 193,053.5 ns |  1,157.31 ns |    605.30 ns |  0.51 |    0.00 |    4 |         - |          NA |
| DoubleSelectionSortSimd | 1024 | SingleElementMoved |  29,629.2 ns |    612.72 ns |    320.46 ns |  0.08 |    0.00 |    2 |         - |          NA |
| CycleSort               | 1024 | SingleElementMoved | 542,498.6 ns |  1,904.99 ns |    996.35 ns |  1.43 |    0.01 |    6 |         - |          NA |
| PancakeSort             | 1024 | SingleElementMoved | 315,818.0 ns | 15,540.99 ns |  6,900.29 ns |  0.83 |    0.02 |    5 |         - |          NA |
| BingoSort               | 1024 | SingleElementMoved | 379,339.2 ns |  1,506.57 ns |    668.93 ns |  1.00 |    0.00 |    5 |         - |          NA |
| DotnetSort              | 1024 | SingleElementMoved |   6,799.2 ns |    399.11 ns |    208.74 ns |  0.02 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**           | **1024** | **Sorted**             | **376,450.2 ns** |  **1,123.56 ns** |    **400.67 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| SelectionSortSimd       | 1024 | Sorted             |  37,539.7 ns |    501.23 ns |    222.55 ns |  0.10 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSort     | 1024 | Sorted             | 160,468.7 ns |  1,095.45 ns |    390.65 ns |  0.43 |    0.00 |    4 |         - |          NA |
| DoubleSelectionSortSimd | 1024 | Sorted             |  21,435.7 ns |    530.04 ns |    277.22 ns |  0.06 |    0.00 |    2 |         - |          NA |
| CycleSort               | 1024 | Sorted             | 339,036.1 ns |    830.67 ns |    434.45 ns |  0.90 |    0.00 |    6 |         - |          NA |
| PancakeSort             | 1024 | Sorted             | 254,021.3 ns |  1,149.18 ns |    510.24 ns |  0.67 |    0.00 |    5 |         - |          NA |
| BingoSort               | 1024 | Sorted             | 376,840.1 ns |  1,816.23 ns |    647.68 ns |  1.00 |    0.00 |    6 |         - |          NA |
| DotnetSort              | 1024 | Sorted             |   4,088.9 ns |    254.81 ns |    113.14 ns |  0.01 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**           | **1024** | **Reversed**           | **344,293.0 ns** | **18,227.74 ns** |  **9,533.46 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| SelectionSortSimd       | 1024 | Reversed           |  54,478.7 ns |    815.29 ns |    361.99 ns |  0.16 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 1024 | Reversed           | 160,899.2 ns |  1,041.87 ns |    462.60 ns |  0.47 |    0.01 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 1024 | Reversed           |  54,606.4 ns |    611.48 ns |    319.82 ns |  0.16 |    0.00 |    2 |         - |          NA |
| CycleSort               | 1024 | Reversed           | 628,721.0 ns |  2,989.36 ns |  1,327.30 ns |  1.83 |    0.05 |    7 |         - |          NA |
| PancakeSort             | 1024 | Reversed           | 275,184.7 ns | 21,887.71 ns | 11,447.69 ns |  0.80 |    0.04 |    4 |         - |          NA |
| BingoSort               | 1024 | Reversed           | 433,887.4 ns | 27,945.56 ns | 12,408.00 ns |  1.26 |    0.05 |    6 |         - |          NA |
| DotnetSort              | 1024 | Reversed           |   8,015.4 ns |    847.14 ns |    443.07 ns |  0.02 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**           | **1024** | **PipeOrgan**          | **390,387.2 ns** |  **5,433.01 ns** |  **2,412.29 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| SelectionSortSimd       | 1024 | PipeOrgan          |  69,508.6 ns |  2,836.51 ns |  1,483.55 ns |  0.18 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSort     | 1024 | PipeOrgan          | 195,270.7 ns |  3,432.94 ns |  1,795.50 ns |  0.50 |    0.01 |    4 |         - |          NA |
| DoubleSelectionSortSimd | 1024 | PipeOrgan          |  38,464.0 ns |    640.39 ns |    334.94 ns |  0.10 |    0.00 |    2 |         - |          NA |
| CycleSort               | 1024 | PipeOrgan          | 876,096.7 ns |  3,052.12 ns |  1,596.32 ns |  2.24 |    0.01 |    6 |         - |          NA |
| PancakeSort             | 1024 | PipeOrgan          | 405,450.8 ns |  3,645.04 ns |  1,618.42 ns |  1.04 |    0.01 |    5 |         - |          NA |
| BingoSort               | 1024 | PipeOrgan          | 425,741.7 ns |  3,010.94 ns |  1,336.88 ns |  1.09 |    0.01 |    5 |         - |          NA |
| DotnetSort              | 1024 | PipeOrgan          |  16,682.6 ns |    185.70 ns |     66.22 ns |  0.04 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**           | **1024** | **ManyDuplicates**     | **403,939.1 ns** |  **1,826.25 ns** |    **810.87 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 1024 | ManyDuplicates     |  79,670.5 ns |    854.76 ns |    379.52 ns |  0.20 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 1024 | ManyDuplicates     | 268,394.6 ns |    739.65 ns |    328.41 ns |  0.66 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 1024 | ManyDuplicates     |  67,060.1 ns |    582.09 ns |    258.45 ns |  0.17 |    0.00 |    2 |         - |          NA |
| CycleSort               | 1024 | ManyDuplicates     | 987,453.7 ns |  1,858.17 ns |    825.04 ns |  2.44 |    0.00 |    5 |         - |          NA |
| PancakeSort             | 1024 | ManyDuplicates     | 461,037.9 ns |  2,974.46 ns |  1,555.70 ns |  1.14 |    0.00 |    4 |         - |          NA |
| BingoSort               | 1024 | ManyDuplicates     |  26,156.7 ns |    236.09 ns |    123.48 ns |  0.06 |    0.00 |    1 |         - |          NA |
| DotnetSort              | 1024 | ManyDuplicates     |  29,877.7 ns |    216.93 ns |     96.32 ns |  0.07 |    0.00 |    1 |         - |          NA |

### TreeBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.86GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.22 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                 | Size | Pattern            | Mean            | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------- |----- |------------------- |----------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **BalancedBinaryTreeSort** | **256**  | **Random**             |     **21,738.1 ns** |    **303.41 ns** |    **158.69 ns** |  **1.43** |    **0.01** |    **2** |         **-** |          **NA** |
| BTreeSort              | 256  | Random             |     22,577.4 ns |  5,177.28 ns |  2,707.82 ns |  1.48 |    0.17 |    2 |         - |          NA |
| BPlusTreeSort          | 256  | Random             |     19,805.1 ns |    232.75 ns |    121.73 ns |  1.30 |    0.01 |    2 |         - |          NA |
| CartesianTreeSort      | 256  | Random             |     17,100.5 ns |    241.67 ns |    126.40 ns |  1.12 |    0.01 |    2 |         - |          NA |
| BinaryTreeSort         | 256  | Random             |     15,253.9 ns |     76.27 ns |     39.89 ns |  1.00 |    0.00 |    2 |         - |          NA |
| SplaySort              | 256  | Random             |     39,270.8 ns |    372.64 ns |    194.90 ns |  2.57 |    0.01 |    3 |         - |          NA |
| TreapSort              | 256  | Random             |     23,024.5 ns |    404.33 ns |    211.47 ns |  1.51 |    0.01 |    2 |         - |          NA |
| DotnetSort             | 256  | Random             |      7,606.7 ns |    507.03 ns |    265.18 ns |  0.50 |    0.02 |    1 |         - |          NA |
|      |                    |                 |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **SingleElementMoved** |      **8,325.1 ns** |    **339.44 ns** |    **177.53 ns** |  **0.12** |    **0.00** |    **5** |         **-** |          **NA** |
| BTreeSort              | 256  | SingleElementMoved |      5,965.9 ns |     81.20 ns |     36.05 ns |  0.09 |    0.00 |    4 |         - |          NA |
| BPlusTreeSort          | 256  | SingleElementMoved |      5,555.2 ns |    303.23 ns |    158.60 ns |  0.08 |    0.00 |    4 |         - |          NA |
| CartesianTreeSort      | 256  | SingleElementMoved |      2,053.0 ns |      9.73 ns |      4.32 ns |  0.03 |    0.00 |    2 |         - |          NA |
| BinaryTreeSort         | 256  | SingleElementMoved |     69,212.7 ns |    484.97 ns |    253.65 ns |  1.00 |    0.00 |    6 |         - |          NA |
| SplaySort              | 256  | SingleElementMoved |      4,309.5 ns |     23.53 ns |     10.45 ns |  0.06 |    0.00 |    3 |         - |          NA |
| TreapSort              | 256  | SingleElementMoved |      5,987.9 ns |    393.35 ns |    174.65 ns |  0.09 |    0.00 |    4 |         - |          NA |
| DotnetSort             | 256  | SingleElementMoved |      1,406.0 ns |    113.50 ns |     59.36 ns |  0.02 |    0.00 |    1 |         - |          NA |
|      |                    |                 |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Sorted**             |      **7,885.1 ns** |    **309.41 ns** |    **161.83 ns** |  **0.11** |    **0.00** |    **5** |         **-** |          **NA** |
| BTreeSort              | 256  | Sorted             |      5,813.6 ns |    341.37 ns |    178.55 ns |  0.08 |    0.00 |    4 |         - |          NA |
| BPlusTreeSort          | 256  | Sorted             |      5,105.3 ns |     11.17 ns |      5.84 ns |  0.07 |    0.00 |    4 |         - |          NA |
| CartesianTreeSort      | 256  | Sorted             |      2,066.2 ns |      6.25 ns |      2.78 ns |  0.03 |    0.00 |    2 |         - |          NA |
| BinaryTreeSort         | 256  | Sorted             |     75,090.9 ns |    958.17 ns |    501.14 ns |  1.00 |    0.00 |    6 |         - |          NA |
| SplaySort              | 256  | Sorted             |      3,913.5 ns |    243.14 ns |    127.17 ns |  0.05 |    0.00 |    3 |         - |          NA |
| TreapSort              | 256  | Sorted             |      5,014.2 ns |     36.48 ns |     13.01 ns |  0.07 |    0.00 |    4 |         - |          NA |
| DotnetSort             | 256  | Sorted             |        818.5 ns |     50.30 ns |     22.33 ns |  0.01 |    0.00 |    1 |         - |          NA |
|      |                    |                 |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Reversed**           |      **7,665.0 ns** |    **340.87 ns** |    **178.28 ns** |  **0.10** |    **0.00** |    **5** |         **-** |          **NA** |
| BTreeSort              | 256  | Reversed           |      9,469.6 ns |    576.91 ns |    205.73 ns |  0.13 |    0.00 |    5 |         - |          NA |
| BPlusTreeSort          | 256  | Reversed           |      8,613.4 ns |    386.99 ns |    202.40 ns |  0.11 |    0.00 |    5 |         - |          NA |
| CartesianTreeSort      | 256  | Reversed           |      1,926.7 ns |     11.47 ns |      5.09 ns |  0.03 |    0.00 |    2 |         - |          NA |
| BinaryTreeSort         | 256  | Reversed           |     75,500.0 ns |    750.40 ns |    392.47 ns |  1.00 |    0.00 |    6 |         - |          NA |
| SplaySort              | 256  | Reversed           |      3,719.3 ns |     27.36 ns |      9.76 ns |  0.05 |    0.00 |    3 |         - |          NA |
| TreapSort              | 256  | Reversed           |      4,869.7 ns |    113.61 ns |     50.44 ns |  0.06 |    0.00 |    4 |         - |          NA |
| DotnetSort             | 256  | Reversed           |      1,432.6 ns |    185.48 ns |     82.35 ns |  0.02 |    0.00 |    1 |         - |          NA |
|      |                    |                 |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **PipeOrgan**          |      **6,833.0 ns** |    **352.91 ns** |    **184.58 ns** |  **0.18** |    **0.00** |    **3** |         **-** |          **NA** |
| BTreeSort              | 256  | PipeOrgan          |      6,815.8 ns |    267.31 ns |    118.69 ns |  0.18 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 256  | PipeOrgan          |      6,176.4 ns |    139.48 ns |     61.93 ns |  0.16 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 256  | PipeOrgan          |      2,208.8 ns |     24.77 ns |      8.83 ns |  0.06 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | PipeOrgan          |     38,191.2 ns |    537.16 ns |    238.50 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 256  | PipeOrgan          |      4,373.9 ns |     16.96 ns |      8.87 ns |  0.11 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | PipeOrgan          |      7,062.3 ns |    419.88 ns |    219.61 ns |  0.18 |    0.01 |    3 |         - |          NA |
| DotnetSort             | 256  | PipeOrgan          |      2,500.7 ns |    371.75 ns |    194.43 ns |  0.07 |    0.00 |    1 |         - |          NA |
|      |                    |                 |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **ManyDuplicates**     |     **20,482.2 ns** |    **586.29 ns** |    **260.32 ns** |  **1.51** |    **0.02** |    **3** |         **-** |          **NA** |
| BTreeSort              | 256  | ManyDuplicates     |     19,679.6 ns |    342.42 ns |    179.09 ns |  1.45 |    0.02 |    3 |         - |          NA |
| BPlusTreeSort          | 256  | ManyDuplicates     |     18,962.4 ns |    291.52 ns |    129.44 ns |  1.40 |    0.01 |    3 |         - |          NA |
| CartesianTreeSort      | 256  | ManyDuplicates     |     19,772.4 ns |    194.26 ns |    101.60 ns |  1.46 |    0.01 |    3 |         - |          NA |
| BinaryTreeSort         | 256  | ManyDuplicates     |     13,564.9 ns |    212.06 ns |    110.91 ns |  1.00 |    0.00 |    2 |         - |          NA |
| SplaySort              | 256  | ManyDuplicates     |     36,922.7 ns |    615.44 ns |    321.89 ns |  2.72 |    0.03 |    4 |         - |          NA |
| TreapSort              | 256  | ManyDuplicates     |     22,823.1 ns |    271.95 ns |    120.75 ns |  1.68 |    0.02 |    3 |         - |          NA |
| DotnetSort             | 256  | ManyDuplicates     |      7,237.2 ns |    397.65 ns |    207.98 ns |  0.53 |    0.02 |    1 |         - |          NA |
|      |                    |                 |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Random**             |     **99,887.7 ns** |    **740.19 ns** |    **387.13 ns** |  **1.41** |    **0.01** |    **3** |         **-** |          **NA** |
| BTreeSort              | 1024 | Random             |     96,937.6 ns |    779.26 ns |    346.00 ns |  1.37 |    0.01 |    3 |         - |          NA |
| BPlusTreeSort          | 1024 | Random             |     96,735.1 ns |    804.68 ns |    357.28 ns |  1.36 |    0.01 |    3 |         - |          NA |
| CartesianTreeSort      | 1024 | Random             |     85,800.2 ns |    637.43 ns |    283.02 ns |  1.21 |    0.01 |    3 |         - |          NA |
| BinaryTreeSort         | 1024 | Random             |     70,973.7 ns |    523.19 ns |    232.30 ns |  1.00 |    0.00 |    2 |         - |          NA |
| SplaySort              | 1024 | Random             |    199,462.7 ns |  1,316.34 ns |    584.46 ns |  2.81 |    0.01 |    4 |         - |          NA |
| TreapSort              | 1024 | Random             |    104,804.0 ns |    854.85 ns |    379.56 ns |  1.48 |    0.01 |    3 |         - |          NA |
| DotnetSort             | 1024 | Random             |     38,460.2 ns |    774.99 ns |    405.33 ns |  0.54 |    0.01 |    1 |         - |          NA |
|      |                    |                 |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **SingleElementMoved** |     **36,542.3 ns** |    **462.69 ns** |    **241.99 ns** | **0.034** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 1024 | SingleElementMoved |     27,450.7 ns |    438.15 ns |    229.16 ns | 0.025 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 1024 | SingleElementMoved |     25,415.2 ns |    327.64 ns |    171.36 ns | 0.023 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 1024 | SingleElementMoved |      7,946.0 ns |    265.03 ns |    138.61 ns | 0.007 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | SingleElementMoved |  1,085,056.5 ns |    497.53 ns |    220.91 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | SingleElementMoved |     17,070.4 ns |    153.72 ns |     80.40 ns | 0.016 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | SingleElementMoved |     26,247.8 ns |    594.97 ns |    311.18 ns | 0.024 |    0.00 |    3 |         - |          NA |
| DotnetSort             | 1024 | SingleElementMoved |      6,756.6 ns |    431.91 ns |    191.77 ns | 0.006 |    0.00 |    1 |         - |          NA |
|      |                    |                 |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Sorted**             |     **35,309.5 ns** |    **342.16 ns** |    **178.96 ns** | **0.029** |    **0.00** |    **5** |         **-** |          **NA** |
| BTreeSort              | 1024 | Sorted             |     26,472.5 ns |    493.76 ns |    258.24 ns | 0.022 |    0.00 |    4 |         - |          NA |
| BPlusTreeSort          | 1024 | Sorted             |     24,166.6 ns |    304.86 ns |    135.36 ns | 0.020 |    0.00 |    4 |         - |          NA |
| CartesianTreeSort      | 1024 | Sorted             |      8,154.5 ns |    405.51 ns |    212.09 ns | 0.007 |    0.00 |    2 |         - |          NA |
| BinaryTreeSort         | 1024 | Sorted             |  1,204,526.7 ns |  1,799.48 ns |    941.16 ns | 1.000 |    0.00 |    6 |         - |          NA |
| SplaySort              | 1024 | Sorted             |     15,419.7 ns |     94.76 ns |     42.07 ns | 0.013 |    0.00 |    3 |         - |          NA |
| TreapSort              | 1024 | Sorted             |     22,631.0 ns |    555.76 ns |    290.68 ns | 0.019 |    0.00 |    4 |         - |          NA |
| DotnetSort             | 1024 | Sorted             |      4,195.2 ns |    685.56 ns |    304.39 ns | 0.003 |    0.00 |    1 |         - |          NA |
|      |                    |                 |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Reversed**           |     **35,024.1 ns** |    **624.34 ns** |    **326.54 ns** | **0.029** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 1024 | Reversed           |     40,861.5 ns |    382.88 ns |    200.25 ns | 0.034 |    0.00 |    4 |         - |          NA |
| BPlusTreeSort          | 1024 | Reversed           |     37,670.2 ns |    815.02 ns |    361.88 ns | 0.032 |    0.00 |    4 |         - |          NA |
| CartesianTreeSort      | 1024 | Reversed           |      7,632.3 ns |    283.01 ns |    148.02 ns | 0.006 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | Reversed           |  1,195,461.2 ns |  5,923.48 ns |  3,098.10 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | Reversed           |     14,916.8 ns |    101.65 ns |     53.17 ns | 0.012 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | Reversed           |     22,150.4 ns |    719.38 ns |    376.25 ns | 0.019 |    0.00 |    3 |         - |          NA |
| DotnetSort             | 1024 | Reversed           |      7,826.8 ns |  1,136.09 ns |    594.20 ns | 0.007 |    0.00 |    1 |         - |          NA |
|      |                    |                 |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **PipeOrgan**          |     **31,005.7 ns** |    **744.40 ns** |    **330.52 ns** |  **0.05** |    **0.00** |    **3** |         **-** |          **NA** |
| BTreeSort              | 1024 | PipeOrgan          |     30,850.0 ns |    814.81 ns |    361.78 ns |  0.05 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 1024 | PipeOrgan          |     27,784.1 ns |    566.61 ns |    296.35 ns |  0.05 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 1024 | PipeOrgan          |      8,744.8 ns |    376.89 ns |    197.12 ns |  0.01 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | PipeOrgan          |    603,956.2 ns |    497.40 ns |    220.85 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | PipeOrgan          |     17,490.3 ns |    184.04 ns |     96.26 ns |  0.03 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | PipeOrgan          |     32,142.3 ns |  1,058.49 ns |    553.61 ns |  0.05 |    0.00 |    3 |         - |          NA |
| DotnetSort             | 1024 | PipeOrgan          |     17,284.6 ns |  1,543.19 ns |    807.12 ns |  0.03 |    0.00 |    2 |         - |          NA |
|      |                    |                 |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **ManyDuplicates**     |     **84,713.0 ns** |    **820.80 ns** |    **364.44 ns** |  **1.13** |    **0.00** |    **2** |         **-** |          **NA** |
| BTreeSort              | 1024 | ManyDuplicates     |     88,409.8 ns |    587.60 ns |    260.90 ns |  1.18 |    0.00 |    2 |         - |          NA |
| BPlusTreeSort          | 1024 | ManyDuplicates     |     84,432.1 ns |    743.15 ns |    329.96 ns |  1.12 |    0.00 |    2 |         - |          NA |
| CartesianTreeSort      | 1024 | ManyDuplicates     |     96,818.5 ns |    463.62 ns |    205.85 ns |  1.29 |    0.00 |    2 |         - |          NA |
| BinaryTreeSort         | 1024 | ManyDuplicates     |     75,104.8 ns |    272.46 ns |     97.16 ns |  1.00 |    0.00 |    2 |         - |          NA |
| SplaySort              | 1024 | ManyDuplicates     |    158,294.1 ns |  2,261.90 ns |  1,183.02 ns |  2.11 |    0.02 |    3 |         - |          NA |
| TreapSort              | 1024 | ManyDuplicates     |    101,051.3 ns |    552.04 ns |    245.11 ns |  1.35 |    0.00 |    2 |         - |          NA |
| DotnetSort             | 1024 | ManyDuplicates     |     30,121.9 ns |    409.88 ns |    214.38 ns |  0.40 |    0.00 |    1 |         - |          NA |
|      |                    |                 |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **4096** | **Random**             |    **464,087.1 ns** |  **3,593.56 ns** |  **1,879.50 ns** |  **1.38** |    **0.01** |    **3** |         **-** |          **NA** |
| BTreeSort              | 4096 | Random             |    460,868.3 ns |  2,059.47 ns |    914.42 ns |  1.37 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 4096 | Random             |    448,283.3 ns |  1,842.61 ns |    963.72 ns |  1.33 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 4096 | Random             |    432,300.6 ns |  1,305.34 ns |    682.72 ns |  1.29 |    0.00 |    3 |         - |          NA |
| BinaryTreeSort         | 4096 | Random             |    336,307.3 ns |  2,067.92 ns |    918.17 ns |  1.00 |    0.00 |    2 |         - |          NA |
| SplaySort              | 4096 | Random             |    999,182.3 ns |  2,575.94 ns |  1,143.73 ns |  2.97 |    0.01 |    4 |         - |          NA |
| TreapSort              | 4096 | Random             |    499,326.0 ns |  1,789.97 ns |    794.76 ns |  1.48 |    0.00 |    3 |         - |          NA |
| DotnetSort             | 4096 | Random             |    186,509.3 ns |  1,997.68 ns |    886.98 ns |  0.55 |    0.00 |    1 |         - |          NA |
|      |                    |                 |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **4096** | **SingleElementMoved** |    **163,593.3 ns** |  **1,090.50 ns** |    **484.19 ns** | **0.009** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 4096 | SingleElementMoved |    131,491.9 ns |    786.89 ns |    349.38 ns | 0.008 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 4096 | SingleElementMoved |    122,447.0 ns |  1,682.90 ns |    880.19 ns | 0.007 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 4096 | SingleElementMoved |     31,126.9 ns |    127.68 ns |     66.78 ns | 0.002 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 4096 | SingleElementMoved | 17,395,901.4 ns |  6,046.66 ns |  3,162.52 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 4096 | SingleElementMoved |     66,837.8 ns |    306.90 ns |    160.52 ns | 0.004 |    0.00 |    2 |         - |          NA |
| TreapSort              | 4096 | SingleElementMoved |    114,108.6 ns |  2,608.78 ns |  1,364.44 ns | 0.007 |    0.00 |    3 |         - |          NA |
| DotnetSort             | 4096 | SingleElementMoved |     31,706.0 ns |    699.53 ns |    310.60 ns | 0.002 |    0.00 |    1 |         - |          NA |
|      |                    |                 |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **4096** | **Sorted**             |    **157,490.9 ns** |    **862.06 ns** |    **382.76 ns** | **0.008** |    **0.00** |    **6** |         **-** |          **NA** |
| BTreeSort              | 4096 | Sorted             |    129,421.8 ns |  3,776.14 ns |  1,975.00 ns | 0.007 |    0.00 |    5 |         - |          NA |
| BPlusTreeSort          | 4096 | Sorted             |    118,739.2 ns |  1,437.39 ns |    638.21 ns | 0.006 |    0.00 |    5 |         - |          NA |
| CartesianTreeSort      | 4096 | Sorted             |     32,166.6 ns |    362.05 ns |    160.75 ns | 0.002 |    0.00 |    2 |         - |          NA |
| BinaryTreeSort         | 4096 | Sorted             | 19,033,250.1 ns |  4,534.68 ns |  1,617.11 ns | 1.000 |    0.00 |    7 |         - |          NA |
| SplaySort              | 4096 | Sorted             |     61,384.8 ns |    299.61 ns |    156.70 ns | 0.003 |    0.00 |    3 |         - |          NA |
| TreapSort              | 4096 | Sorted             |     96,058.8 ns |  2,363.41 ns |  1,236.11 ns | 0.005 |    0.00 |    4 |         - |          NA |
| DotnetSort             | 4096 | Sorted             |     20,273.1 ns |  1,448.67 ns |    643.22 ns | 0.001 |    0.00 |    1 |         - |          NA |
|      |                    |                 |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **4096** | **Reversed**           |    **157,116.4 ns** |  **1,389.94 ns** |    **726.97 ns** | **0.008** |    **0.00** |    **5** |         **-** |          **NA** |
| BTreeSort              | 4096 | Reversed           |    177,858.2 ns |  1,708.47 ns |    758.57 ns | 0.009 |    0.00 |    5 |         - |          NA |
| BPlusTreeSort          | 4096 | Reversed           |    163,987.0 ns |  2,297.23 ns |  1,019.99 ns | 0.009 |    0.00 |    5 |         - |          NA |
| CartesianTreeSort      | 4096 | Reversed           |     30,275.0 ns |    122.84 ns |     64.25 ns | 0.002 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 4096 | Reversed           | 19,141,767.6 ns |  7,151.29 ns |  3,175.22 ns | 1.000 |    0.00 |    6 |         - |          NA |
| SplaySort              | 4096 | Reversed           |     58,889.5 ns |    163.71 ns |     85.62 ns | 0.003 |    0.00 |    3 |         - |          NA |
| TreapSort              | 4096 | Reversed           |     94,599.5 ns |  1,554.24 ns |    812.90 ns | 0.005 |    0.00 |    4 |         - |          NA |
| DotnetSort             | 4096 | Reversed           |     37,997.8 ns |  9,394.81 ns |  4,913.67 ns | 0.002 |    0.00 |    2 |         - |          NA |
|      |                    |                 |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **4096** | **PipeOrgan**          |    **148,922.2 ns** |  **7,800.25 ns** |  **4,079.68 ns** | **0.016** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 4096 | PipeOrgan          |    145,938.2 ns |  3,061.19 ns |  1,601.06 ns | 0.015 |    0.00 |    4 |         - |          NA |
| BPlusTreeSort          | 4096 | PipeOrgan          |    136,846.1 ns |  4,532.42 ns |  2,370.55 ns | 0.014 |    0.00 |    4 |         - |          NA |
| CartesianTreeSort      | 4096 | PipeOrgan          |     34,654.3 ns |    476.71 ns |    249.33 ns | 0.004 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 4096 | PipeOrgan          |  9,555,765.4 ns | 53,612.31 ns | 28,040.27 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 4096 | PipeOrgan          |     68,777.4 ns |    740.42 ns |    328.75 ns | 0.007 |    0.00 |    2 |         - |          NA |
| TreapSort              | 4096 | PipeOrgan          |    149,313.5 ns |  3,026.19 ns |  1,079.17 ns | 0.016 |    0.00 |    4 |         - |          NA |
| DotnetSort             | 4096 | PipeOrgan          |     92,504.2 ns |  3,259.06 ns |  1,704.55 ns | 0.010 |    0.00 |    3 |         - |          NA |
|      |                    |                 |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **4096** | **ManyDuplicates**     |    **340,798.0 ns** |  **3,637.87 ns** |  **1,902.67 ns** |  **0.46** |    **0.00** |    **2** |         **-** |          **NA** |
| BTreeSort              | 4096 | ManyDuplicates     |    373,619.4 ns |  1,086.64 ns |    482.47 ns |  0.50 |    0.00 |    2 |         - |          NA |
| BPlusTreeSort          | 4096 | ManyDuplicates     |    358,096.2 ns |  1,145.04 ns |    508.40 ns |  0.48 |    0.00 |    2 |         - |          NA |
| CartesianTreeSort      | 4096 | ManyDuplicates     |    458,399.2 ns |    658.49 ns |    292.37 ns |  0.61 |    0.00 |    2 |         - |          NA |
| BinaryTreeSort         | 4096 | ManyDuplicates     |    748,012.8 ns |  1,379.89 ns |    612.68 ns |  1.00 |    0.00 |    3 |         - |          NA |
| SplaySort              | 4096 | ManyDuplicates     |    649,664.0 ns |  3,126.00 ns |  1,634.96 ns |  0.87 |    0.00 |    3 |         - |          NA |
| TreapSort              | 4096 | ManyDuplicates     |    438,559.1 ns |  1,682.61 ns |    880.04 ns |  0.59 |    0.00 |    2 |         - |          NA |
| DotnetSort             | 4096 | ManyDuplicates     |    122,142.9 ns |  1,166.44 ns |    517.91 ns |  0.16 |    0.00 |    1 |         - |          NA |
|      |                    |                 |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **8192** | **Random**             |  **1,016,030.0 ns** |  **4,950.29 ns** |  **2,589.10 ns** |  **1.34** |    **0.01** |    **3** |         **-** |          **NA** |
| BTreeSort              | 8192 | Random             |    992,454.2 ns |  4,384.98 ns |  2,293.43 ns |  1.31 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 8192 | Random             |    965,663.0 ns |  1,873.64 ns |    831.91 ns |  1.27 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 8192 | Random             |    963,677.1 ns |  2,731.86 ns |  1,428.81 ns |  1.27 |    0.00 |    3 |         - |          NA |
| BinaryTreeSort         | 8192 | Random             |    758,474.2 ns |  4,831.91 ns |  2,527.18 ns |  1.00 |    0.00 |    2 |         - |          NA |
| SplaySort              | 8192 | Random             |  2,232,510.7 ns | 10,691.30 ns |  4,747.00 ns |  2.94 |    0.01 |    4 |         - |          NA |
| TreapSort              | 8192 | Random             |  1,123,472.2 ns |  3,799.90 ns |  1,687.18 ns |  1.48 |    0.01 |    3 |         - |          NA |
| DotnetSort             | 8192 | Random             |    405,718.0 ns |  5,055.70 ns |  2,644.23 ns |  0.53 |    0.00 |    1 |         - |          NA |
|      |                    |                 |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **8192** | **SingleElementMoved** |    **341,386.6 ns** |  **2,889.26 ns** |  **1,282.85 ns** | **0.005** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 8192 | SingleElementMoved |    278,501.6 ns |  2,123.99 ns |  1,110.89 ns | 0.004 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 8192 | SingleElementMoved |    261,924.3 ns |  1,818.17 ns |    950.94 ns | 0.004 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 8192 | SingleElementMoved |     62,333.9 ns |    444.83 ns |    197.51 ns | 0.001 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 8192 | SingleElementMoved | 68,259,318.5 ns | 26,041.82 ns | 13,620.37 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 8192 | SingleElementMoved |    133,078.4 ns |    535.56 ns |    190.98 ns | 0.002 |    0.00 |    2 |         - |          NA |
| TreapSort              | 8192 | SingleElementMoved |    255,659.6 ns | 20,843.89 ns | 10,901.76 ns | 0.004 |    0.00 |    3 |         - |          NA |
| DotnetSort             | 8192 | SingleElementMoved |     69,325.4 ns |  2,573.52 ns |  1,346.00 ns | 0.001 |    0.00 |    1 |         - |          NA |
|      |                    |                 |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **8192** | **Sorted**             |    **333,746.2 ns** |  **1,114.00 ns** |    **494.62 ns** | **0.004** |    **0.00** |    **6** |         **-** |          **NA** |
| BTreeSort              | 8192 | Sorted             |    270,276.2 ns |  3,472.74 ns |  1,541.92 ns | 0.004 |    0.00 |    5 |         - |          NA |
| BPlusTreeSort          | 8192 | Sorted             |    252,261.8 ns |    796.71 ns |    353.75 ns | 0.003 |    0.00 |    5 |         - |          NA |
| CartesianTreeSort      | 8192 | Sorted             |     64,076.9 ns |    356.57 ns |    158.32 ns | 0.001 |    0.00 |    2 |         - |          NA |
| BinaryTreeSort         | 8192 | Sorted             | 75,683,180.0 ns |  8,399.67 ns |  3,729.51 ns | 1.000 |    0.00 |    7 |         - |          NA |
| SplaySort              | 8192 | Sorted             |    123,206.4 ns |    648.97 ns |    339.42 ns | 0.002 |    0.00 |    3 |         - |          NA |
| TreapSort              | 8192 | Sorted             |    203,950.9 ns |  9,988.59 ns |  5,224.23 ns | 0.003 |    0.00 |    4 |         - |          NA |
| DotnetSort             | 8192 | Sorted             |     46,680.4 ns |  4,134.43 ns |  2,162.39 ns | 0.001 |    0.00 |    1 |         - |          NA |
|      |                    |                 |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **8192** | **Reversed**           |    **333,851.5 ns** |  **2,064.71 ns** |  **1,079.88 ns** | **0.004** |    **0.00** |    **5** |         **-** |          **NA** |
| BTreeSort              | 8192 | Reversed           |    364,931.5 ns |  1,584.97 ns |    703.74 ns | 0.005 |    0.00 |    5 |         - |          NA |
| BPlusTreeSort          | 8192 | Reversed           |    337,281.0 ns |  1,594.93 ns |    834.18 ns | 0.004 |    0.00 |    5 |         - |          NA |
| CartesianTreeSort      | 8192 | Reversed           |     60,173.2 ns |    150.95 ns |     67.02 ns | 0.001 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 8192 | Reversed           | 76,422,791.4 ns | 17,389.54 ns |  7,721.06 ns | 1.000 |    0.00 |    6 |         - |          NA |
| SplaySort              | 8192 | Reversed           |    117,373.0 ns |    732.57 ns |    325.27 ns | 0.002 |    0.00 |    3 |         - |          NA |
| TreapSort              | 8192 | Reversed           |    193,598.5 ns |  4,346.51 ns |  2,273.31 ns | 0.003 |    0.00 |    4 |         - |          NA |
| DotnetSort             | 8192 | Reversed           |     75,616.3 ns |  3,516.27 ns |  1,839.08 ns | 0.001 |    0.00 |    2 |         - |          NA |
|      |                    |                 |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **8192** | **PipeOrgan**          |    **390,104.7 ns** | **25,062.94 ns** | **13,108.40 ns** | **0.010** |    **0.00** |    **3** |         **-** |          **NA** |
| BTreeSort              | 8192 | PipeOrgan          |    336,103.8 ns | 17,786.55 ns |  9,302.71 ns | 0.009 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 8192 | PipeOrgan          |    323,272.3 ns |  4,241.38 ns |  1,883.20 ns | 0.008 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 8192 | PipeOrgan          |     69,013.8 ns |  1,030.79 ns |    457.68 ns | 0.002 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 8192 | PipeOrgan          | 38,124,692.6 ns | 16,862.43 ns |  8,819.38 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 8192 | PipeOrgan          |    137,308.6 ns |  1,148.95 ns |    510.14 ns | 0.004 |    0.00 |    2 |         - |          NA |
| TreapSort              | 8192 | PipeOrgan          |    527,508.7 ns | 10,355.13 ns |  5,415.93 ns | 0.014 |    0.00 |    4 |         - |          NA |
| DotnetSort             | 8192 | PipeOrgan          |    363,160.9 ns |  3,591.58 ns |  1,878.47 ns | 0.010 |    0.00 |    3 |         - |          NA |
|      |                    |                 |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **8192** | **ManyDuplicates**     |    **681,607.3 ns** |  **7,595.34 ns** |  **3,972.51 ns** |  **0.19** |    **0.00** |    **2** |         **-** |          **NA** |
| BTreeSort              | 8192 | ManyDuplicates     |    750,168.9 ns |  2,384.13 ns |  1,246.95 ns |  0.21 |    0.00 |    2 |         - |          NA |
| BPlusTreeSort          | 8192 | ManyDuplicates     |    720,018.7 ns |  4,710.86 ns |  2,463.87 ns |  0.20 |    0.00 |    2 |         - |          NA |
| CartesianTreeSort      | 8192 | ManyDuplicates     |    997,176.0 ns |  1,564.80 ns |    694.78 ns |  0.27 |    0.00 |    3 |         - |          NA |
| BinaryTreeSort         | 8192 | ManyDuplicates     |  3,644,571.6 ns |  7,493.23 ns |  3,919.10 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 8192 | ManyDuplicates     |  1,306,830.9 ns |  4,165.23 ns |  2,178.49 ns |  0.36 |    0.00 |    4 |         - |          NA |
| TreapSort              | 8192 | ManyDuplicates     |    907,734.1 ns |  4,127.65 ns |  2,158.84 ns |  0.25 |    0.00 |    3 |         - |          NA |
| DotnetSort             | 8192 | ManyDuplicates     |    245,654.4 ns |  1,545.55 ns |    808.35 ns |  0.07 |    0.00 |    1 |         - |          NA |

</details>

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
