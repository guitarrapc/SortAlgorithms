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
| Exchange | 1024 | **CombSort**<br>×3.00 vs 2nd 🟢 | **CocktailShakerSort**<br>×11.69 vs 2nd 🟢 | **CocktailShakerSort**<br>×1.24 vs 2nd 🟡 | **CombSort**<br>×1.15 vs 2nd 🟢 | **CombSort**<br>×6.44 vs 2nd 🟢 | **CombSort**<br>×3.48 vs 2nd 🟢 |
| Selection | 1024 | **SelectionSortSimd**<br>≈ 2nd 🟢 | **DoubleSelectionSortSimd**<br>×1.48 vs 2nd 🟢 | **DoubleSelectionSortSimd**<br>×1.53 vs 2nd 🟢 | **DoubleSelectionSortSimd**<br>×1.02 vs 2nd 🟢 | **DoubleSelectionSortSimd**<br>×1.63 vs 2nd 🟢 | **BingoSort**<br>×2.78 vs 2nd 🟢 |
| Insertion | 8192 | **ShellSortKnuth1973**<br>×1.07 vs 2nd 🟢 | **InsertionSort**<br>×1.17 vs 2nd 🟢 | **InsertionSort**<br>×1.27 vs 2nd 🟢 | **ShellSortKnuth1973**<br>×1.15 vs 2nd 🟢 | **ShellSortKnuth1973**<br>×1.13 vs 2nd 🟢 | **ShellSortSedgewick1986**<br>×1.04 vs 2nd 🟢 |
| Merge | 8192 | **SimdMergeSort**<br>×1.08 vs 2nd 🟢 | **TimSort**<br>×1.55 vs 2nd 🟡 | **Glidesort**<br>≈ 2nd 🟢 | **NaturalMergeSort**<br>≈ 2nd 🟢 | **Driftsort**<br>×1.19 vs 2nd 🟢 | **Driftsort**<br>×1.46 vs 2nd 🟢 |
| MergeSlow | 1024 | **StrandSort**<br>only one 🟢 | **StrandSort**<br>only one 🟢 | **StrandSort**<br>only one 🟢 | **StrandSort**<br>only one 🟢 | **StrandSort**<br>only one 🟢 | **StrandSort**<br>only one 🟢 |
| Heap | 8192 | **HeapSort**<br>×1.01 vs 2nd 🟢 | **SmoothSort**<br>×3.13 vs 2nd 🟢 | **SmoothSort**<br>×3.64 vs 2nd 🟢 | **PairingHeapSort**<br>×3.13 vs 2nd 🟢 | **PairingHeapSort**<br>×1.08 vs 2nd 🟢 | **HeapSort**<br>×1.02 vs 2nd 🟢 |
| Partition | 8192 | **SimdQuickSort**<br>×4.14 vs 2nd 🟢 | **FluxSort**<br>×1.30 vs 2nd 🟡 | **Ips4oSort**<br>×1.21 vs 2nd 🟢 | **Ips4oSort**<br>≈ 2nd 🟡 | **FluxSort**<br>×1.27 vs 2nd 🟢 | **PDQSortBranchless**<br>×1.02 vs 2nd 🟢 |
| Distribution | 8192 | **PigeonSortInteger**<br>×1.39 vs 2nd 🟢 | **PigeonSortInteger**<br>×1.24 vs 2nd 🟢 | **SpreadSort**<br>×4.93 vs 2nd 🟢 | **PigeonSortInteger**<br>×1.37 vs 2nd 🟢 | **PigeonSortInteger**<br>×1.34 vs 2nd 🟢 | **PigeonSortInteger**<br>×1.28 vs 2nd 🟢 |
| Network | 8192 | **BitonicSortAnyLengthSimd**<br>×3.04 vs 2nd 🟢 | **BitonicSortAnyLengthSimd**<br>×2.58 vs 2nd 🟢 | **BitonicSortAnyLengthSimd**<br>×2.62 vs 2nd 🟢 | **BitonicSortAnyLengthSimd**<br>×2.64 vs 2nd 🟢 | **BitonicSortAnyLengthSimd**<br>×2.79 vs 2nd 🟢 | **BitonicSortAnyLengthSimd**<br>×3.07 vs 2nd 🟢 |
| Tree | 8192 | **BinaryTreeSort**<br>×1.22 vs 2nd 🟢 | **CartesianTreeSort**<br>×2.14 vs 2nd 🟢 | **CartesianTreeSort**<br>×2.00 vs 2nd 🟢 | **CartesianTreeSort**<br>×1.95 vs 2nd 🟢 | **CartesianTreeSort**<br>×2.04 vs 2nd 🟢 | **BalancedBinaryTreeSort**<br>×1.07 vs 2nd 🟢 |

<details>
<summary>Benchmark results (2026-11-22 04:43 UTC)</summary>

Workflow run: https://github.com/guitarrapc/SortAlgorithms/actions/runs/44984332332

### DistributionBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.49 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method              | Size | Pattern            | Mean           | Error        | StdDev      | Ratio  | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------------- |----- |------------------- |---------------:|-------------:|------------:|-------:|--------:|-----:|----------:|------------:|
| **CountingSort**        | **256**  | **Random**             |     **1,302.1 ns** |     **46.13 ns** |    **20.48 ns** |   **1.46** |    **0.05** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | Random             |       895.2 ns |     62.64 ns |    27.81 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 256  | Random             |     1,137.6 ns |     25.41 ns |    11.28 ns |   1.27 |    0.04 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | Random             |       845.3 ns |    413.68 ns |   216.36 ns |   0.94 |    0.23 |    1 |         - |          NA |
| BucketSort          | 256  | Random             |     1,444.7 ns |     36.65 ns |    16.27 ns |   1.62 |    0.05 |    2 |         - |          NA |
| BucketSortInteger   | 256  | Random             |     1,214.2 ns |     48.41 ns |    21.50 ns |   1.36 |    0.04 |    2 |         - |          NA |
| FlashSort           | 256  | Random             |     4,030.8 ns |    170.68 ns |    89.27 ns |   4.51 |    0.16 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | Random             |     2,159.5 ns |     35.24 ns |    15.65 ns |   2.41 |    0.07 |    3 |         - |          NA |
| RotateLSDRadixSort  | 256  | Random             |    57,106.4 ns |    731.04 ns |   382.35 ns |  63.84 |    1.81 |    7 |         - |          NA |
| RadixLSD256Sort     | 256  | Random             |     1,051.8 ns |     18.82 ns |     8.36 ns |   1.18 |    0.03 |    2 |         - |          NA |
| Radsort             | 256  | Random             |     7,732.3 ns |    169.48 ns |    75.25 ns |   8.64 |    0.25 |    6 |         - |          NA |
| RadixLSD10Sort      | 256  | Random             |     3,186.0 ns |     41.56 ns |    18.45 ns |   3.56 |    0.10 |    4 |         - |          NA |
| RadixMSD4Sort       | 256  | Random             |     5,154.6 ns |     31.94 ns |    11.39 ns |   5.76 |    0.16 |    5 |         - |          NA |
| RadixMSD10Sort      | 256  | Random             |     4,618.5 ns |     48.37 ns |    17.25 ns |   5.16 |    0.14 |    5 |         - |          NA |
| AmericanFlagSort    | 256  | Random             |     2,412.4 ns |     49.62 ns |    17.69 ns |   2.70 |    0.08 |    3 |         - |          NA |
| SkaSort             | 256  | Random             |     3,365.7 ns |    294.75 ns |   154.16 ns |   3.76 |    0.19 |    4 |         - |          NA |
| SpreadSort          | 256  | Random             |     6,092.7 ns |    330.27 ns |   172.74 ns |   6.81 |    0.26 |    5 |         - |          NA |
| DotnetSort          | 256  | Random             |     5,807.6 ns |    153.97 ns |    80.53 ns |   6.49 |    0.20 |    5 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **256**  | **SingleElementMoved** |     **1,253.1 ns** |     **15.52 ns** |     **8.12 ns** |   **1.60** |    **0.01** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | SingleElementMoved |       782.4 ns |      6.72 ns |     2.40 ns |   1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 256  | SingleElementMoved |     1,129.7 ns |     17.58 ns |     7.80 ns |   1.44 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | SingleElementMoved |       653.3 ns |     12.95 ns |     4.62 ns |   0.83 |    0.01 |    1 |         - |          NA |
| BucketSort          | 256  | SingleElementMoved |     1,388.9 ns |      6.42 ns |     2.85 ns |   1.78 |    0.01 |    2 |         - |          NA |
| BucketSortInteger   | 256  | SingleElementMoved |     1,119.4 ns |     19.58 ns |     6.98 ns |   1.43 |    0.01 |    2 |         - |          NA |
| FlashSort           | 256  | SingleElementMoved |     3,198.9 ns |     77.27 ns |    27.56 ns |   4.09 |    0.03 |    4 |         - |          NA |
| RadixLSD4Sort       | 256  | SingleElementMoved |     2,304.9 ns |    213.22 ns |    94.67 ns |   2.95 |    0.11 |    3 |         - |          NA |
| RotateLSDRadixSort  | 256  | SingleElementMoved |    34,912.7 ns |    473.72 ns |   247.76 ns |  44.62 |    0.32 |    7 |         - |          NA |
| RadixLSD256Sort     | 256  | SingleElementMoved |       990.2 ns |     35.63 ns |    15.82 ns |   1.27 |    0.02 |    2 |         - |          NA |
| Radsort             | 256  | SingleElementMoved |     7,480.0 ns |    290.53 ns |   151.95 ns |   9.56 |    0.19 |    6 |         - |          NA |
| RadixLSD10Sort      | 256  | SingleElementMoved |     3,095.3 ns |     12.25 ns |     5.44 ns |   3.96 |    0.01 |    4 |         - |          NA |
| RadixMSD4Sort       | 256  | SingleElementMoved |     5,678.0 ns |    257.76 ns |   134.81 ns |   7.26 |    0.16 |    5 |         - |          NA |
| RadixMSD10Sort      | 256  | SingleElementMoved |     4,879.9 ns |    295.85 ns |   154.73 ns |   6.24 |    0.19 |    5 |         - |          NA |
| AmericanFlagSort    | 256  | SingleElementMoved |     1,647.6 ns |     10.95 ns |     4.86 ns |   2.11 |    0.01 |    2 |         - |          NA |
| SkaSort             | 256  | SingleElementMoved |     2,411.2 ns |      6.96 ns |     2.48 ns |   3.08 |    0.01 |    3 |         - |          NA |
| SpreadSort          | 256  | SingleElementMoved |       796.2 ns |     74.55 ns |    33.10 ns |   1.02 |    0.04 |    1 |         - |          NA |
| DotnetSort          | 256  | SingleElementMoved |     1,107.7 ns |    156.50 ns |    81.85 ns |   1.42 |    0.10 |    2 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **256**  | **Sorted**             |     **1,336.1 ns** |    **313.95 ns** |   **164.20 ns** |   **1.80** |    **0.21** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | Sorted             |       742.0 ns |     12.19 ns |     5.41 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 256  | Sorted             |     1,118.0 ns |     23.05 ns |    10.24 ns |   1.51 |    0.02 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | Sorted             |       539.4 ns |      6.94 ns |     3.08 ns |   0.73 |    0.01 |    2 |         - |          NA |
| BucketSort          | 256  | Sorted             |     1,317.3 ns |     12.27 ns |     6.42 ns |   1.78 |    0.01 |    3 |         - |          NA |
| BucketSortInteger   | 256  | Sorted             |     1,032.3 ns |     30.45 ns |    13.52 ns |   1.39 |    0.02 |    3 |         - |          NA |
| FlashSort           | 256  | Sorted             |     2,843.2 ns |    203.32 ns |    90.28 ns |   3.83 |    0.12 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | Sorted             |     2,210.1 ns |     78.04 ns |    34.65 ns |   2.98 |    0.05 |    4 |         - |          NA |
| RotateLSDRadixSort  | 256  | Sorted             |    34,123.0 ns |    178.56 ns |    93.39 ns |  45.99 |    0.33 |    9 |         - |          NA |
| RadixLSD256Sort     | 256  | Sorted             |       902.4 ns |     15.13 ns |     6.72 ns |   1.22 |    0.01 |    3 |         - |          NA |
| Radsort             | 256  | Sorted             |     7,218.5 ns |     25.32 ns |    11.24 ns |   9.73 |    0.07 |    8 |         - |          NA |
| RadixLSD10Sort      | 256  | Sorted             |     3,058.4 ns |      6.48 ns |     2.31 ns |   4.12 |    0.03 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | Sorted             |     5,608.9 ns |    257.41 ns |   134.63 ns |   7.56 |    0.18 |    7 |         - |          NA |
| RadixMSD10Sort      | 256  | Sorted             |     4,585.7 ns |     55.24 ns |    19.70 ns |   6.18 |    0.05 |    6 |         - |          NA |
| AmericanFlagSort    | 256  | Sorted             |     1,253.9 ns |     16.54 ns |     8.65 ns |   1.69 |    0.02 |    3 |         - |          NA |
| SkaSort             | 256  | Sorted             |     2,021.0 ns |     10.36 ns |     3.69 ns |   2.72 |    0.02 |    4 |         - |          NA |
| SpreadSort          | 256  | Sorted             |       251.8 ns |      3.64 ns |     1.62 ns |   0.34 |    0.00 |    1 |         - |          NA |
| DotnetSort          | 256  | Sorted             |       629.4 ns |     13.32 ns |     5.92 ns |   0.85 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **256**  | **Reversed**           |     **1,179.4 ns** |     **14.55 ns** |     **7.61 ns** |   **1.58** |    **0.01** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 256  | Reversed           |       748.3 ns |     11.94 ns |     5.30 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 256  | Reversed           |     1,163.0 ns |      3.92 ns |     1.74 ns |   1.55 |    0.01 |    4 |         - |          NA |
| PigeonSortInteger   | 256  | Reversed           |       557.6 ns |      7.72 ns |     3.43 ns |   0.75 |    0.01 |    2 |         - |          NA |
| BucketSort          | 256  | Reversed           |     1,380.6 ns |      7.78 ns |     3.46 ns |   1.85 |    0.01 |    4 |         - |          NA |
| BucketSortInteger   | 256  | Reversed           |     1,055.4 ns |     26.91 ns |    11.95 ns |   1.41 |    0.02 |    4 |         - |          NA |
| FlashSort           | 256  | Reversed           |     2,680.2 ns |     66.45 ns |    23.70 ns |   3.58 |    0.04 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | Reversed           |     2,235.2 ns |     97.38 ns |    43.24 ns |   2.99 |    0.06 |    5 |         - |          NA |
| RotateLSDRadixSort  | 256  | Reversed           |    34,842.1 ns |    287.12 ns |   127.48 ns |  46.56 |    0.35 |    9 |         - |          NA |
| RadixLSD256Sort     | 256  | Reversed           |       918.3 ns |      8.91 ns |     3.96 ns |   1.23 |    0.01 |    4 |         - |          NA |
| Radsort             | 256  | Reversed           |     7,872.3 ns |    217.32 ns |   113.66 ns |  10.52 |    0.16 |    8 |         - |          NA |
| RadixLSD10Sort      | 256  | Reversed           |     3,168.5 ns |    229.33 ns |   119.94 ns |   4.23 |    0.15 |    6 |         - |          NA |
| RadixMSD4Sort       | 256  | Reversed           |     5,444.2 ns |     45.01 ns |    16.05 ns |   7.28 |    0.05 |    7 |         - |          NA |
| RadixMSD10Sort      | 256  | Reversed           |     4,769.3 ns |    289.20 ns |   151.26 ns |   6.37 |    0.20 |    7 |         - |          NA |
| AmericanFlagSort    | 256  | Reversed           |     1,376.9 ns |     19.65 ns |     8.73 ns |   1.84 |    0.02 |    4 |         - |          NA |
| SkaSort             | 256  | Reversed           |     3,103.9 ns |  1,205.91 ns |   630.71 ns |   4.15 |    0.80 |    6 |         - |          NA |
| SpreadSort          | 256  | Reversed           |       374.4 ns |     10.05 ns |     4.46 ns |   0.50 |    0.01 |    1 |         - |          NA |
| DotnetSort          | 256  | Reversed           |     1,111.3 ns |    176.03 ns |    78.16 ns |   1.49 |    0.10 |    4 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **256**  | **PipeOrgan**          |     **1,202.4 ns** |     **11.48 ns** |     **4.09 ns** |   **1.49** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | PipeOrgan          |       807.4 ns |     13.35 ns |     5.93 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 256  | PipeOrgan          |     1,097.2 ns |      6.96 ns |     2.48 ns |   1.36 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | PipeOrgan          |       630.4 ns |      6.75 ns |     3.00 ns |   0.78 |    0.01 |    1 |         - |          NA |
| BucketSort          | 256  | PipeOrgan          |     1,346.3 ns |     23.73 ns |    10.54 ns |   1.67 |    0.02 |    3 |         - |          NA |
| BucketSortInteger   | 256  | PipeOrgan          |     1,159.3 ns |     67.41 ns |    35.26 ns |   1.44 |    0.04 |    3 |         - |          NA |
| FlashSort           | 256  | PipeOrgan          |     3,425.9 ns |    136.80 ns |    71.55 ns |   4.24 |    0.09 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | PipeOrgan          |     2,172.1 ns |     18.17 ns |     8.07 ns |   2.69 |    0.02 |    4 |         - |          NA |
| RotateLSDRadixSort  | 256  | PipeOrgan          |    34,592.4 ns |    196.10 ns |   102.57 ns |  42.85 |    0.32 |    8 |         - |          NA |
| RadixLSD256Sort     | 256  | PipeOrgan          |       974.3 ns |     37.99 ns |    16.87 ns |   1.21 |    0.02 |    3 |         - |          NA |
| Radsort             | 256  | PipeOrgan          |     7,801.5 ns |    281.72 ns |   147.35 ns |   9.66 |    0.18 |    7 |         - |          NA |
| RadixLSD10Sort      | 256  | PipeOrgan          |     3,136.0 ns |     10.97 ns |     4.87 ns |   3.88 |    0.03 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | PipeOrgan          |     5,545.6 ns |     64.74 ns |    23.09 ns |   6.87 |    0.05 |    6 |         - |          NA |
| RadixMSD10Sort      | 256  | PipeOrgan          |     4,862.2 ns |    235.29 ns |   123.06 ns |   6.02 |    0.15 |    6 |         - |          NA |
| AmericanFlagSort    | 256  | PipeOrgan          |     1,881.1 ns |     37.30 ns |    13.30 ns |   2.33 |    0.02 |    4 |         - |          NA |
| SkaSort             | 256  | PipeOrgan          |     2,692.1 ns |     16.78 ns |     7.45 ns |   3.33 |    0.02 |    5 |         - |          NA |
| SpreadSort          | 256  | PipeOrgan          |     1,386.1 ns |     58.24 ns |    20.77 ns |   1.72 |    0.03 |    3 |         - |          NA |
| DotnetSort          | 256  | PipeOrgan          |     1,944.2 ns |    456.38 ns |   238.70 ns |   2.41 |    0.28 |    4 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **256**  | **ManyDuplicates**     |     **1,209.1 ns** |     **24.51 ns** |    **12.82 ns** |   **1.57** |    **0.03** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | ManyDuplicates     |       771.6 ns |     25.33 ns |    11.25 ns |   1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 256  | ManyDuplicates     |     1,323.9 ns |    276.26 ns |   144.49 ns |   1.72 |    0.18 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | ManyDuplicates     |       725.3 ns |    156.62 ns |    81.91 ns |   0.94 |    0.10 |    1 |         - |          NA |
| BucketSort          | 256  | ManyDuplicates     |     2,159.9 ns |    126.26 ns |    56.06 ns |   2.80 |    0.08 |    3 |         - |          NA |
| BucketSortInteger   | 256  | ManyDuplicates     |     1,479.7 ns |      5.70 ns |     2.03 ns |   1.92 |    0.03 |    2 |         - |          NA |
| FlashSort           | 256  | ManyDuplicates     |     3,996.5 ns |    101.91 ns |    45.25 ns |   5.18 |    0.09 |    4 |         - |          NA |
| RadixLSD4Sort       | 256  | ManyDuplicates     |     1,910.9 ns |    233.66 ns |   122.21 ns |   2.48 |    0.15 |    3 |         - |          NA |
| RotateLSDRadixSort  | 256  | ManyDuplicates     |    46,503.5 ns |    716.74 ns |   318.24 ns |  60.28 |    0.90 |    7 |         - |          NA |
| RadixLSD256Sort     | 256  | ManyDuplicates     |     1,045.4 ns |     15.84 ns |     7.03 ns |   1.36 |    0.02 |    2 |         - |          NA |
| Radsort             | 256  | ManyDuplicates     |     8,991.8 ns |    269.45 ns |   140.93 ns |  11.66 |    0.23 |    6 |         - |          NA |
| RadixLSD10Sort      | 256  | ManyDuplicates     |     2,206.1 ns |     18.15 ns |     8.06 ns |   2.86 |    0.04 |    3 |         - |          NA |
| RadixMSD4Sort       | 256  | ManyDuplicates     |     2,727.8 ns |    315.15 ns |   139.93 ns |   3.54 |    0.18 |    3 |         - |          NA |
| RadixMSD10Sort      | 256  | ManyDuplicates     |     2,972.5 ns |    133.34 ns |    59.20 ns |   3.85 |    0.09 |    3 |         - |          NA |
| AmericanFlagSort    | 256  | ManyDuplicates     |     2,439.5 ns |    133.92 ns |    59.46 ns |   3.16 |    0.08 |    3 |         - |          NA |
| SkaSort             | 256  | ManyDuplicates     |     2,832.5 ns |    168.39 ns |    74.77 ns |   3.67 |    0.10 |    3 |         - |          NA |
| SpreadSort          | 256  | ManyDuplicates     |     5,653.7 ns |    346.28 ns |   181.11 ns |   7.33 |    0.24 |    5 |         - |          NA |
| DotnetSort          | 256  | ManyDuplicates     |     5,517.3 ns |    232.43 ns |   121.57 ns |   7.15 |    0.18 |    5 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **1024** | **Random**             |     **4,809.2 ns** |    **134.74 ns** |    **59.82 ns** |   **1.47** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Random             |     3,269.4 ns |     37.95 ns |    13.53 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | Random             |     3,981.0 ns |     18.34 ns |     9.59 ns |   1.22 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | Random             |     2,556.1 ns |     34.88 ns |    15.48 ns |   0.78 |    0.01 |    1 |         - |          NA |
| BucketSort          | 1024 | Random             |     5,428.9 ns |     31.89 ns |    14.16 ns |   1.66 |    0.01 |    3 |         - |          NA |
| BucketSortInteger   | 1024 | Random             |     4,231.2 ns |     26.27 ns |    11.66 ns |   1.29 |    0.01 |    3 |         - |          NA |
| FlashSort           | 1024 | Random             |    15,331.6 ns |    181.51 ns |    80.59 ns |   4.69 |    0.03 |    6 |         - |          NA |
| RadixLSD4Sort       | 1024 | Random             |    10,772.8 ns |    281.57 ns |   147.27 ns |   3.30 |    0.04 |    5 |         - |          NA |
| RotateLSDRadixSort  | 1024 | Random             |   281,951.8 ns |  2,360.97 ns | 1,048.29 ns |  86.24 |    0.45 |    9 |         - |          NA |
| RadixLSD256Sort     | 1024 | Random             |     5,267.6 ns |     37.84 ns |    16.80 ns |   1.61 |    0.01 |    3 |         - |          NA |
| Radsort             | 1024 | Random             |    29,822.5 ns |    240.74 ns |   125.91 ns |   9.12 |    0.05 |    8 |         - |          NA |
| RadixLSD10Sort      | 1024 | Random             |    16,249.1 ns |    274.28 ns |   143.46 ns |   4.97 |    0.05 |    6 |         - |          NA |
| RadixMSD4Sort       | 1024 | Random             |    28,951.3 ns |    256.39 ns |   134.10 ns |   8.86 |    0.05 |    8 |         - |          NA |
| RadixMSD10Sort      | 1024 | Random             |    22,434.4 ns |    262.03 ns |   137.05 ns |   6.86 |    0.05 |    7 |         - |          NA |
| AmericanFlagSort    | 1024 | Random             |    17,045.0 ns |    362.07 ns |   189.37 ns |   5.21 |    0.06 |    6 |         - |          NA |
| SkaSort             | 1024 | Random             |    12,311.8 ns |    184.70 ns |    82.01 ns |   3.77 |    0.03 |    5 |         - |          NA |
| SpreadSort          | 1024 | Random             |     6,832.2 ns |    180.86 ns |    94.59 ns |   2.09 |    0.03 |    4 |         - |          NA |
| DotnetSort          | 1024 | Random             |    29,879.6 ns |    524.21 ns |   232.75 ns |   9.14 |    0.08 |    8 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **1024** | **SingleElementMoved** |     **5,054.2 ns** |    **199.43 ns** |   **104.31 ns** |   **1.72** |    **0.04** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | SingleElementMoved |     2,944.8 ns |     54.18 ns |    24.06 ns |   1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 1024 | SingleElementMoved |     3,862.6 ns |     11.29 ns |     5.01 ns |   1.31 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | SingleElementMoved |     2,629.3 ns |    321.75 ns |   168.28 ns |   0.89 |    0.05 |    1 |         - |          NA |
| BucketSort          | 1024 | SingleElementMoved |     5,385.7 ns |    152.12 ns |    67.54 ns |   1.83 |    0.03 |    2 |         - |          NA |
| BucketSortInteger   | 1024 | SingleElementMoved |     4,108.9 ns |     37.85 ns |    16.81 ns |   1.40 |    0.01 |    2 |         - |          NA |
| FlashSort           | 1024 | SingleElementMoved |    11,964.9 ns |    447.54 ns |   198.71 ns |   4.06 |    0.07 |    4 |         - |          NA |
| RadixLSD4Sort       | 1024 | SingleElementMoved |    12,689.9 ns |    308.72 ns |   161.47 ns |   4.31 |    0.06 |    4 |         - |          NA |
| RotateLSDRadixSort  | 1024 | SingleElementMoved |   176,369.5 ns |  1,087.40 ns |   482.81 ns |  59.90 |    0.48 |    7 |         - |          NA |
| RadixLSD256Sort     | 1024 | SingleElementMoved |     5,199.7 ns |     64.89 ns |    28.81 ns |   1.77 |    0.02 |    2 |         - |          NA |
| Radsort             | 1024 | SingleElementMoved |    19,384.5 ns |    410.10 ns |   146.25 ns |   6.58 |    0.07 |    6 |         - |          NA |
| RadixLSD10Sort      | 1024 | SingleElementMoved |    16,025.6 ns |    137.24 ns |    71.78 ns |   5.44 |    0.05 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | SingleElementMoved |    23,789.0 ns |    229.57 ns |   101.93 ns |   8.08 |    0.07 |    6 |         - |          NA |
| RadixMSD10Sort      | 1024 | SingleElementMoved |    22,310.6 ns |    392.48 ns |   205.27 ns |   7.58 |    0.09 |    6 |         - |          NA |
| AmericanFlagSort    | 1024 | SingleElementMoved |     9,009.2 ns |    316.45 ns |   165.51 ns |   3.06 |    0.06 |    3 |         - |          NA |
| SkaSort             | 1024 | SingleElementMoved |     8,311.2 ns |    364.24 ns |   190.50 ns |   2.82 |    0.06 |    3 |         - |          NA |
| SpreadSort          | 1024 | SingleElementMoved |     4,494.4 ns |     60.72 ns |    26.96 ns |   1.53 |    0.01 |    2 |         - |          NA |
| DotnetSort          | 1024 | SingleElementMoved |     5,348.5 ns |    284.11 ns |   148.60 ns |   1.82 |    0.05 |    2 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **1024** | **Sorted**             |     **4,867.5 ns** |    **380.28 ns** |   **198.89 ns** |   **1.67** |    **0.08** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Sorted             |     2,918.5 ns |    195.41 ns |    86.76 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 1024 | Sorted             |     3,842.0 ns |      8.15 ns |     2.91 ns |   1.32 |    0.04 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | Sorted             |     2,168.6 ns |      9.11 ns |     4.04 ns |   0.74 |    0.02 |    2 |         - |          NA |
| BucketSort          | 1024 | Sorted             |     4,852.4 ns |     43.15 ns |    19.16 ns |   1.66 |    0.04 |    4 |         - |          NA |
| BucketSortInteger   | 1024 | Sorted             |     3,833.7 ns |     86.91 ns |    38.59 ns |   1.31 |    0.04 |    3 |         - |          NA |
| FlashSort           | 1024 | Sorted             |    11,175.5 ns |    418.42 ns |   218.84 ns |   3.83 |    0.12 |    6 |         - |          NA |
| RadixLSD4Sort       | 1024 | Sorted             |    12,370.7 ns |    323.12 ns |   169.00 ns |   4.24 |    0.13 |    6 |         - |          NA |
| RotateLSDRadixSort  | 1024 | Sorted             |   174,457.9 ns |    553.18 ns |   197.27 ns |  59.82 |    1.60 |    8 |         - |          NA |
| RadixLSD256Sort     | 1024 | Sorted             |     4,902.3 ns |     96.01 ns |    42.63 ns |   1.68 |    0.05 |    4 |         - |          NA |
| Radsort             | 1024 | Sorted             |    18,961.9 ns |    309.78 ns |   162.02 ns |   6.50 |    0.18 |    7 |         - |          NA |
| RadixLSD10Sort      | 1024 | Sorted             |    15,889.5 ns |    115.05 ns |    60.18 ns |   5.45 |    0.15 |    7 |         - |          NA |
| RadixMSD4Sort       | 1024 | Sorted             |    23,806.3 ns |    188.72 ns |    98.70 ns |   8.16 |    0.22 |    7 |         - |          NA |
| RadixMSD10Sort      | 1024 | Sorted             |    22,565.5 ns |    200.06 ns |   104.64 ns |   7.74 |    0.21 |    7 |         - |          NA |
| AmericanFlagSort    | 1024 | Sorted             |     7,461.6 ns |    262.90 ns |   137.50 ns |   2.56 |    0.08 |    5 |         - |          NA |
| SkaSort             | 1024 | Sorted             |     7,001.4 ns |    186.83 ns |    97.72 ns |   2.40 |    0.07 |    5 |         - |          NA |
| SpreadSort          | 1024 | Sorted             |       556.3 ns |     64.94 ns |    33.96 ns |   0.19 |    0.01 |    1 |         - |          NA |
| DotnetSort          | 1024 | Sorted             |     3,233.3 ns |    210.62 ns |    93.51 ns |   1.11 |    0.04 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **1024** | **Reversed**           |     **4,529.8 ns** |    **309.89 ns** |   **162.08 ns** |   **1.50** |    **0.09** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Reversed           |     3,024.2 ns |    291.98 ns |   152.71 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | Reversed           |     4,115.5 ns |     24.59 ns |     8.77 ns |   1.36 |    0.06 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | Reversed           |     2,178.9 ns |     45.12 ns |    20.03 ns |   0.72 |    0.03 |    1 |         - |          NA |
| BucketSort          | 1024 | Reversed           |     5,129.5 ns |     23.77 ns |    10.55 ns |   1.70 |    0.08 |    3 |         - |          NA |
| BucketSortInteger   | 1024 | Reversed           |     3,906.9 ns |     35.10 ns |    15.59 ns |   1.29 |    0.06 |    3 |         - |          NA |
| FlashSort           | 1024 | Reversed           |    10,765.9 ns |    402.40 ns |   210.46 ns |   3.57 |    0.18 |    5 |         - |          NA |
| RadixLSD4Sort       | 1024 | Reversed           |    16,342.3 ns |    279.82 ns |   146.35 ns |   5.42 |    0.25 |    6 |         - |          NA |
| RotateLSDRadixSort  | 1024 | Reversed           |   177,966.4 ns |    807.62 ns |   358.59 ns |  58.97 |    2.72 |    8 |         - |          NA |
| RadixLSD256Sort     | 1024 | Reversed           |     5,084.4 ns |    128.66 ns |    67.29 ns |   1.68 |    0.08 |    3 |         - |          NA |
| Radsort             | 1024 | Reversed           |    20,968.2 ns |     63.84 ns |    22.77 ns |   6.95 |    0.32 |    7 |         - |          NA |
| RadixLSD10Sort      | 1024 | Reversed           |    15,798.9 ns |    128.74 ns |    67.34 ns |   5.24 |    0.24 |    6 |         - |          NA |
| RadixMSD4Sort       | 1024 | Reversed           |    24,005.1 ns |    210.32 ns |   110.00 ns |   7.95 |    0.37 |    7 |         - |          NA |
| RadixMSD10Sort      | 1024 | Reversed           |    22,376.8 ns |    208.70 ns |   109.15 ns |   7.42 |    0.34 |    7 |         - |          NA |
| AmericanFlagSort    | 1024 | Reversed           |     8,364.7 ns |    227.77 ns |   119.13 ns |   2.77 |    0.13 |    4 |         - |          NA |
| SkaSort             | 1024 | Reversed           |     7,957.7 ns |    336.95 ns |   176.23 ns |   2.64 |    0.13 |    4 |         - |          NA |
| SpreadSort          | 1024 | Reversed           |     3,899.1 ns |    274.91 ns |   143.78 ns |   1.29 |    0.07 |    3 |         - |          NA |
| DotnetSort          | 1024 | Reversed           |     5,984.2 ns |  1,240.18 ns |   648.64 ns |   1.98 |    0.22 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **1024** | **PipeOrgan**          |     **4,465.1 ns** |     **36.70 ns** |    **13.09 ns** |   **1.40** |    **0.06** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | PipeOrgan          |     3,195.8 ns |    294.59 ns |   154.07 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | PipeOrgan          |     3,885.5 ns |     10.31 ns |     4.58 ns |   1.22 |    0.05 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | PipeOrgan          |     2,462.4 ns |     44.13 ns |    15.74 ns |   0.77 |    0.03 |    1 |         - |          NA |
| BucketSort          | 1024 | PipeOrgan          |     4,992.1 ns |     17.04 ns |     7.56 ns |   1.57 |    0.07 |    3 |         - |          NA |
| BucketSortInteger   | 1024 | PipeOrgan          |     4,306.6 ns |     42.82 ns |    22.40 ns |   1.35 |    0.06 |    3 |         - |          NA |
| FlashSort           | 1024 | PipeOrgan          |    13,372.1 ns |    301.00 ns |   157.43 ns |   4.19 |    0.19 |    4 |         - |          NA |
| RadixLSD4Sort       | 1024 | PipeOrgan          |    10,693.3 ns |    253.21 ns |   132.43 ns |   3.35 |    0.15 |    4 |         - |          NA |
| RotateLSDRadixSort  | 1024 | PipeOrgan          |   176,232.8 ns |  1,365.56 ns |   606.32 ns |  55.25 |    2.45 |    6 |         - |          NA |
| RadixLSD256Sort     | 1024 | PipeOrgan          |     5,635.4 ns |    311.14 ns |   162.73 ns |   1.77 |    0.09 |    3 |         - |          NA |
| Radsort             | 1024 | PipeOrgan          |    19,189.6 ns |    275.08 ns |   143.87 ns |   6.02 |    0.27 |    4 |         - |          NA |
| RadixLSD10Sort      | 1024 | PipeOrgan          |    16,042.9 ns |    223.26 ns |   116.77 ns |   5.03 |    0.23 |    4 |         - |          NA |
| RadixMSD4Sort       | 1024 | PipeOrgan          |    24,112.1 ns |    418.59 ns |   218.93 ns |   7.56 |    0.34 |    5 |         - |          NA |
| RadixMSD10Sort      | 1024 | PipeOrgan          |    24,956.0 ns |    286.58 ns |   149.89 ns |   7.82 |    0.35 |    5 |         - |          NA |
| AmericanFlagSort    | 1024 | PipeOrgan          |    11,665.9 ns |    238.98 ns |   106.11 ns |   3.66 |    0.16 |    4 |         - |          NA |
| SkaSort             | 1024 | PipeOrgan          |    11,103.0 ns |    465.51 ns |   243.47 ns |   3.48 |    0.17 |    4 |         - |          NA |
| SpreadSort          | 1024 | PipeOrgan          |     4,814.6 ns |    114.46 ns |    50.82 ns |   1.51 |    0.07 |    3 |         - |          NA |
| DotnetSort          | 1024 | PipeOrgan          |    13,020.9 ns |    267.79 ns |   118.90 ns |   4.08 |    0.18 |    4 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **1024** | **ManyDuplicates**     |     **4,496.6 ns** |    **145.85 ns** |    **64.76 ns** |   **1.58** |    **0.04** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | ManyDuplicates     |     2,839.8 ns |    162.99 ns |    72.37 ns |   1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 1024 | ManyDuplicates     |     4,803.5 ns |     49.05 ns |    25.65 ns |   1.69 |    0.04 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | ManyDuplicates     |     2,499.8 ns |    384.85 ns |   201.28 ns |   0.88 |    0.07 |    1 |         - |          NA |
| BucketSort          | 1024 | ManyDuplicates     |     8,441.8 ns |    311.79 ns |   163.07 ns |   2.97 |    0.09 |    2 |         - |          NA |
| BucketSortInteger   | 1024 | ManyDuplicates     |     4,172.8 ns |     57.81 ns |    30.24 ns |   1.47 |    0.04 |    2 |         - |          NA |
| FlashSort           | 1024 | ManyDuplicates     |    13,691.1 ns |    219.15 ns |   114.62 ns |   4.82 |    0.12 |    3 |         - |          NA |
| RadixLSD4Sort       | 1024 | ManyDuplicates     |     6,898.8 ns |    276.58 ns |   144.66 ns |   2.43 |    0.07 |    2 |         - |          NA |
| RotateLSDRadixSort  | 1024 | ManyDuplicates     |   191,132.5 ns |  1,535.15 ns |   802.91 ns |  67.34 |    1.57 |    5 |         - |          NA |
| RadixLSD256Sort     | 1024 | ManyDuplicates     |     3,302.5 ns |     16.39 ns |     7.28 ns |   1.16 |    0.03 |    1 |         - |          NA |
| Radsort             | 1024 | ManyDuplicates     |    19,955.6 ns |    226.49 ns |   118.46 ns |   7.03 |    0.17 |    4 |         - |          NA |
| RadixLSD10Sort      | 1024 | ManyDuplicates     |     8,936.6 ns |    230.93 ns |   120.78 ns |   3.15 |    0.08 |    2 |         - |          NA |
| RadixMSD4Sort       | 1024 | ManyDuplicates     |     7,704.6 ns |    380.76 ns |   199.15 ns |   2.71 |    0.09 |    2 |         - |          NA |
| RadixMSD10Sort      | 1024 | ManyDuplicates     |     9,759.9 ns |    251.84 ns |   131.72 ns |   3.44 |    0.09 |    2 |         - |          NA |
| AmericanFlagSort    | 1024 | ManyDuplicates     |     7,414.5 ns |    333.67 ns |   174.52 ns |   2.61 |    0.08 |    2 |         - |          NA |
| SkaSort             | 1024 | ManyDuplicates     |     6,431.9 ns |    440.72 ns |   195.68 ns |   2.27 |    0.08 |    2 |         - |          NA |
| SpreadSort          | 1024 | ManyDuplicates     |     5,439.9 ns |    181.56 ns |    94.96 ns |   1.92 |    0.05 |    2 |         - |          NA |
| DotnetSort          | 1024 | ManyDuplicates     |    23,250.2 ns |    360.40 ns |   188.50 ns |   8.19 |    0.20 |    4 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **4096** | **Random**             |    **19,030.1 ns** |    **466.81 ns** |   **244.15 ns** |   **1.43** |    **0.03** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Random             |    13,328.2 ns |    382.32 ns |   199.96 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | Random             |    15,591.8 ns |    229.97 ns |   120.28 ns |   1.17 |    0.02 |    2 |         - |          NA |
| PigeonSortInteger   | 4096 | Random             |     9,909.8 ns |    248.24 ns |   129.83 ns |   0.74 |    0.01 |    1 |         - |          NA |
| BucketSort          | 4096 | Random             |    21,427.2 ns |    209.01 ns |   109.32 ns |   1.61 |    0.02 |    2 |         - |          NA |
| BucketSortInteger   | 4096 | Random             |    17,346.4 ns |    126.25 ns |    45.02 ns |   1.30 |    0.02 |    2 |         - |          NA |
| FlashSort           | 4096 | Random             |    64,745.4 ns | 10,727.72 ns | 5,610.81 ns |   4.86 |    0.40 |    5 |         - |          NA |
| RadixLSD4Sort       | 4096 | Random             |    48,674.8 ns |    324.80 ns |   144.21 ns |   3.65 |    0.05 |    4 |         - |          NA |
| RotateLSDRadixSort  | 4096 | Random             | 1,402,296.0 ns |  2,665.49 ns |   950.54 ns | 105.23 |    1.49 |    8 |         - |          NA |
| RadixLSD256Sort     | 4096 | Random             |    19,997.1 ns |    443.33 ns |   158.10 ns |   1.50 |    0.02 |    2 |         - |          NA |
| Radsort             | 4096 | Random             |    68,468.3 ns |    495.38 ns |   219.95 ns |   5.14 |    0.07 |    5 |         - |          NA |
| RadixLSD10Sort      | 4096 | Random             |    64,080.5 ns |    284.57 ns |   126.35 ns |   4.81 |    0.07 |    5 |         - |          NA |
| RadixMSD4Sort       | 4096 | Random             |    98,369.1 ns |    600.80 ns |   266.76 ns |   7.38 |    0.11 |    6 |         - |          NA |
| RadixMSD10Sort      | 4096 | Random             |    88,918.6 ns |    564.07 ns |   250.45 ns |   6.67 |    0.10 |    6 |         - |          NA |
| AmericanFlagSort    | 4096 | Random             |    61,661.6 ns |    693.44 ns |   307.89 ns |   4.63 |    0.07 |    5 |         - |          NA |
| SkaSort             | 4096 | Random             |    46,658.1 ns |    911.41 ns |   404.67 ns |   3.50 |    0.06 |    4 |         - |          NA |
| SpreadSort          | 4096 | Random             |    28,353.8 ns |    279.49 ns |   124.10 ns |   2.13 |    0.03 |    3 |         - |          NA |
| DotnetSort          | 4096 | Random             |   145,114.7 ns |  1,981.24 ns |   879.69 ns |  10.89 |    0.17 |    7 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **4096** | **SingleElementMoved** |    **19,561.8 ns** |    **491.08 ns** |   **256.85 ns** |   **1.63** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 4096 | SingleElementMoved |    12,030.5 ns |    207.34 ns |   108.44 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | SingleElementMoved |    15,268.9 ns |    278.72 ns |   123.75 ns |   1.27 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 4096 | SingleElementMoved |     9,860.5 ns |    342.03 ns |   178.89 ns |   0.82 |    0.02 |    1 |         - |          NA |
| BucketSort          | 4096 | SingleElementMoved |    20,851.9 ns |    143.80 ns |    63.85 ns |   1.73 |    0.02 |    3 |         - |          NA |
| BucketSortInteger   | 4096 | SingleElementMoved |    19,007.3 ns |    829.98 ns |   434.10 ns |   1.58 |    0.04 |    3 |         - |          NA |
| FlashSort           | 4096 | SingleElementMoved |    47,559.7 ns |    874.11 ns |   388.11 ns |   3.95 |    0.05 |    4 |         - |          NA |
| RadixLSD4Sort       | 4096 | SingleElementMoved |    71,595.1 ns |    711.61 ns |   372.18 ns |   5.95 |    0.06 |    5 |         - |          NA |
| RotateLSDRadixSort  | 4096 | SingleElementMoved |   857,222.1 ns |  1,767.58 ns |   784.82 ns |  71.26 |    0.61 |    7 |         - |          NA |
| RadixLSD256Sort     | 4096 | SingleElementMoved |    19,315.2 ns |    518.66 ns |   230.29 ns |   1.61 |    0.02 |    3 |         - |          NA |
| Radsort             | 4096 | SingleElementMoved |    44,698.7 ns |    258.16 ns |   135.02 ns |   3.72 |    0.03 |    4 |         - |          NA |
| RadixLSD10Sort      | 4096 | SingleElementMoved |    63,499.3 ns |    364.55 ns |   190.67 ns |   5.28 |    0.05 |    5 |         - |          NA |
| RadixMSD4Sort       | 4096 | SingleElementMoved |   100,748.6 ns |    640.10 ns |   284.21 ns |   8.38 |    0.08 |    6 |         - |          NA |
| RadixMSD10Sort      | 4096 | SingleElementMoved |    88,452.5 ns |    837.14 ns |   437.84 ns |   7.35 |    0.07 |    6 |         - |          NA |
| AmericanFlagSort    | 4096 | SingleElementMoved |    34,180.4 ns |    748.17 ns |   391.31 ns |   2.84 |    0.04 |    3 |         - |          NA |
| SkaSort             | 4096 | SingleElementMoved |    28,970.2 ns |    186.84 ns |    82.96 ns |   2.41 |    0.02 |    3 |         - |          NA |
| SpreadSort          | 4096 | SingleElementMoved |    18,047.3 ns |    285.77 ns |   149.46 ns |   1.50 |    0.02 |    3 |         - |          NA |
| DotnetSort          | 4096 | SingleElementMoved |    24,461.8 ns |    209.26 ns |    92.91 ns |   2.03 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **4096** | **Sorted**             |    **18,060.4 ns** |    **568.28 ns** |   **297.22 ns** |   **1.54** |    **0.03** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Sorted             |    11,760.9 ns |    236.58 ns |   123.74 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 4096 | Sorted             |    15,370.3 ns |    490.08 ns |   217.60 ns |   1.31 |    0.02 |    4 |         - |          NA |
| PigeonSortInteger   | 4096 | Sorted             |     8,687.7 ns |    287.26 ns |   150.24 ns |   0.74 |    0.01 |    2 |         - |          NA |
| BucketSort          | 4096 | Sorted             |    21,756.0 ns |    245.86 ns |   128.59 ns |   1.85 |    0.02 |    4 |         - |          NA |
| BucketSortInteger   | 4096 | Sorted             |    15,649.2 ns |    432.39 ns |   191.98 ns |   1.33 |    0.02 |    4 |         - |          NA |
| FlashSort           | 4096 | Sorted             |    44,548.6 ns |     93.10 ns |    48.69 ns |   3.79 |    0.04 |    5 |         - |          NA |
| RadixLSD4Sort       | 4096 | Sorted             |    68,087.4 ns |  1,307.98 ns |   684.10 ns |   5.79 |    0.08 |    6 |         - |          NA |
| RotateLSDRadixSort  | 4096 | Sorted             |   852,130.3 ns |  2,696.34 ns | 1,410.24 ns |  72.46 |    0.73 |    8 |         - |          NA |
| RadixLSD256Sort     | 4096 | Sorted             |    18,007.6 ns |    269.70 ns |   141.06 ns |   1.53 |    0.02 |    4 |         - |          NA |
| Radsort             | 4096 | Sorted             |    42,044.3 ns |    511.21 ns |   182.30 ns |   3.58 |    0.04 |    5 |         - |          NA |
| RadixLSD10Sort      | 4096 | Sorted             |    62,975.4 ns |    294.15 ns |   153.85 ns |   5.36 |    0.05 |    6 |         - |          NA |
| RadixMSD4Sort       | 4096 | Sorted             |    99,751.6 ns |    345.71 ns |   153.50 ns |   8.48 |    0.09 |    7 |         - |          NA |
| RadixMSD10Sort      | 4096 | Sorted             |    87,667.2 ns |    328.63 ns |   145.91 ns |   7.45 |    0.08 |    7 |         - |          NA |
| AmericanFlagSort    | 4096 | Sorted             |    28,155.3 ns |    150.23 ns |    66.70 ns |   2.39 |    0.02 |    4 |         - |          NA |
| SkaSort             | 4096 | Sorted             |    25,744.2 ns |    488.24 ns |   255.36 ns |   2.19 |    0.03 |    4 |         - |          NA |
| SpreadSort          | 4096 | Sorted             |     1,946.9 ns |    239.61 ns |   125.32 ns |   0.17 |    0.01 |    1 |         - |          NA |
| DotnetSort          | 4096 | Sorted             |    15,411.1 ns |    381.84 ns |   199.71 ns |   1.31 |    0.02 |    4 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **4096** | **Reversed**           |    **17,523.7 ns** |    **214.46 ns** |   **112.17 ns** |   **1.47** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Reversed           |    11,888.1 ns |    306.98 ns |   160.56 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | Reversed           |    16,322.5 ns |    134.77 ns |    59.84 ns |   1.37 |    0.02 |    3 |         - |          NA |
| PigeonSortInteger   | 4096 | Reversed           |     8,756.3 ns |    228.75 ns |   119.64 ns |   0.74 |    0.01 |    1 |         - |          NA |
| BucketSort          | 4096 | Reversed           |    21,312.6 ns |    245.63 ns |   128.47 ns |   1.79 |    0.03 |    3 |         - |          NA |
| BucketSortInteger   | 4096 | Reversed           |    15,806.0 ns |    100.50 ns |    44.62 ns |   1.33 |    0.02 |    3 |         - |          NA |
| FlashSort           | 4096 | Reversed           |    42,793.3 ns |    321.93 ns |   168.37 ns |   3.60 |    0.05 |    5 |         - |          NA |
| RadixLSD4Sort       | 4096 | Reversed           |    71,079.4 ns |    906.08 ns |   473.90 ns |   5.98 |    0.09 |    6 |         - |          NA |
| RotateLSDRadixSort  | 4096 | Reversed           |   868,674.2 ns |  2,259.02 ns | 1,003.02 ns |  73.08 |    0.94 |    8 |         - |          NA |
| RadixLSD256Sort     | 4096 | Reversed           |    18,176.6 ns |    142.57 ns |    63.30 ns |   1.53 |    0.02 |    3 |         - |          NA |
| Radsort             | 4096 | Reversed           |    41,472.5 ns |    325.77 ns |   170.38 ns |   3.49 |    0.05 |    5 |         - |          NA |
| RadixLSD10Sort      | 4096 | Reversed           |    62,937.3 ns |    461.16 ns |   204.76 ns |   5.29 |    0.07 |    6 |         - |          NA |
| RadixMSD4Sort       | 4096 | Reversed           |   101,894.3 ns |    817.00 ns |   362.75 ns |   8.57 |    0.11 |    7 |         - |          NA |
| RadixMSD10Sort      | 4096 | Reversed           |    87,439.4 ns |    344.21 ns |   152.83 ns |   7.36 |    0.09 |    7 |         - |          NA |
| AmericanFlagSort    | 4096 | Reversed           |    31,036.8 ns |    285.98 ns |   149.57 ns |   2.61 |    0.04 |    4 |         - |          NA |
| SkaSort             | 4096 | Reversed           |    28,972.6 ns |    196.19 ns |    87.11 ns |   2.44 |    0.03 |    4 |         - |          NA |
| SpreadSort          | 4096 | Reversed           |    14,856.0 ns |    162.66 ns |    85.07 ns |   1.25 |    0.02 |    3 |         - |          NA |
| DotnetSort          | 4096 | Reversed           |    26,406.4 ns |    555.61 ns |   290.59 ns |   2.22 |    0.04 |    4 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **4096** | **PipeOrgan**          |    **18,087.3 ns** |    **105.83 ns** |    **55.35 ns** |   **1.45** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 4096 | PipeOrgan          |    12,507.8 ns |     97.13 ns |    43.13 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | PipeOrgan          |    15,528.1 ns |    595.25 ns |   264.30 ns |   1.24 |    0.02 |    3 |         - |          NA |
| PigeonSortInteger   | 4096 | PipeOrgan          |     9,869.7 ns |    298.56 ns |   156.15 ns |   0.79 |    0.01 |    1 |         - |          NA |
| BucketSort          | 4096 | PipeOrgan          |    20,241.2 ns |    159.41 ns |    70.78 ns |   1.62 |    0.01 |    3 |         - |          NA |
| BucketSortInteger   | 4096 | PipeOrgan          |    19,126.2 ns |    197.77 ns |    87.81 ns |   1.53 |    0.01 |    3 |         - |          NA |
| FlashSort           | 4096 | PipeOrgan          |    51,507.0 ns |    291.33 ns |   129.35 ns |   4.12 |    0.02 |    4 |         - |          NA |
| RadixLSD4Sort       | 4096 | PipeOrgan          |    58,322.5 ns |    497.60 ns |   260.25 ns |   4.66 |    0.02 |    4 |         - |          NA |
| RotateLSDRadixSort  | 4096 | PipeOrgan          |   856,821.5 ns |  1,814.66 ns |   949.10 ns |  68.50 |    0.23 |    6 |         - |          NA |
| RadixLSD256Sort     | 4096 | PipeOrgan          |    19,717.3 ns |    400.33 ns |   209.38 ns |   1.58 |    0.02 |    3 |         - |          NA |
| Radsort             | 4096 | PipeOrgan          |    42,682.7 ns |    633.46 ns |   281.26 ns |   3.41 |    0.02 |    4 |         - |          NA |
| RadixLSD10Sort      | 4096 | PipeOrgan          |    63,851.2 ns |    447.69 ns |   159.65 ns |   5.10 |    0.02 |    4 |         - |          NA |
| RadixMSD4Sort       | 4096 | PipeOrgan          |   104,853.1 ns |  1,133.68 ns |   592.93 ns |   8.38 |    0.05 |    5 |         - |          NA |
| RadixMSD10Sort      | 4096 | PipeOrgan          |    89,190.1 ns |    510.62 ns |   267.06 ns |   7.13 |    0.03 |    5 |         - |          NA |
| AmericanFlagSort    | 4096 | PipeOrgan          |    48,046.0 ns |    454.73 ns |   237.83 ns |   3.84 |    0.02 |    4 |         - |          NA |
| SkaSort             | 4096 | PipeOrgan          |    42,515.9 ns |    314.12 ns |   139.47 ns |   3.40 |    0.02 |    4 |         - |          NA |
| SpreadSort          | 4096 | PipeOrgan          |    21,247.2 ns |    322.32 ns |   168.58 ns |   1.70 |    0.01 |    3 |         - |          NA |
| DotnetSort          | 4096 | PipeOrgan          |    72,328.9 ns |  4,994.33 ns | 2,612.13 ns |   5.78 |    0.20 |    4 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **4096** | **ManyDuplicates**     |    **17,305.6 ns** |    **185.81 ns** |    **82.50 ns** |   **1.55** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 4096 | ManyDuplicates     |    11,147.7 ns |    285.11 ns |   149.12 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | ManyDuplicates     |    19,776.4 ns |    176.65 ns |    78.43 ns |   1.77 |    0.02 |    3 |         - |          NA |
| PigeonSortInteger   | 4096 | ManyDuplicates     |     8,745.7 ns |    231.37 ns |   121.01 ns |   0.78 |    0.01 |    1 |         - |          NA |
| BucketSort          | 4096 | ManyDuplicates     |    30,498.4 ns |    495.45 ns |   176.68 ns |   2.74 |    0.04 |    4 |         - |          NA |
| BucketSortInteger   | 4096 | ManyDuplicates     |    15,925.3 ns |    249.33 ns |   110.70 ns |   1.43 |    0.02 |    3 |         - |          NA |
| FlashSort           | 4096 | ManyDuplicates     |    53,410.1 ns |    532.83 ns |   236.58 ns |   4.79 |    0.06 |    5 |         - |          NA |
| RadixLSD4Sort       | 4096 | ManyDuplicates     |    27,359.9 ns |    184.20 ns |    81.79 ns |   2.45 |    0.03 |    4 |         - |          NA |
| RotateLSDRadixSort  | 4096 | ManyDuplicates     |   772,650.5 ns |  3,183.06 ns | 1,664.80 ns |  69.32 |    0.89 |    7 |         - |          NA |
| RadixLSD256Sort     | 4096 | ManyDuplicates     |    12,479.8 ns |    196.90 ns |    70.22 ns |   1.12 |    0.02 |    2 |         - |          NA |
| Radsort             | 4096 | ManyDuplicates     |    42,487.9 ns |    501.66 ns |   262.38 ns |   3.81 |    0.05 |    4 |         - |          NA |
| RadixLSD10Sort      | 4096 | ManyDuplicates     |    34,635.8 ns |    149.16 ns |    66.23 ns |   3.11 |    0.04 |    4 |         - |          NA |
| RadixMSD4Sort       | 4096 | ManyDuplicates     |    28,519.2 ns |    359.30 ns |   187.92 ns |   2.56 |    0.04 |    4 |         - |          NA |
| RadixMSD10Sort      | 4096 | ManyDuplicates     |    37,039.4 ns |    270.56 ns |   141.51 ns |   3.32 |    0.04 |    4 |         - |          NA |
| AmericanFlagSort    | 4096 | ManyDuplicates     |    26,509.7 ns |    179.93 ns |    94.11 ns |   2.38 |    0.03 |    4 |         - |          NA |
| SkaSort             | 4096 | ManyDuplicates     |    18,475.2 ns |  1,049.80 ns |   466.12 ns |   1.66 |    0.04 |    3 |         - |          NA |
| SpreadSort          | 4096 | ManyDuplicates     |    21,072.9 ns |    374.97 ns |   196.12 ns |   1.89 |    0.03 |    3 |         - |          NA |
| DotnetSort          | 4096 | ManyDuplicates     |    95,023.4 ns |  1,132.17 ns |   592.15 ns |   8.53 |    0.12 |    6 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **8192** | **Random**             |    **39,938.0 ns** |    **695.46 ns** |   **363.74 ns** |   **1.44** |    **0.02** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Random             |    27,826.3 ns |    514.92 ns |   269.31 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | Random             |    30,840.4 ns |    443.93 ns |   232.19 ns |   1.11 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | Random             |    19,991.1 ns |    508.36 ns |   265.88 ns |   0.72 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | Random             |    44,371.6 ns |    382.26 ns |   169.72 ns |   1.59 |    0.02 |    2 |         - |          NA |
| BucketSortInteger   | 8192 | Random             |    36,450.1 ns |    889.25 ns |   465.09 ns |   1.31 |    0.02 |    2 |         - |          NA |
| FlashSort           | 8192 | Random             |   133,738.5 ns |    583.77 ns |   305.32 ns |   4.81 |    0.04 |    3 |         - |          NA |
| RadixLSD4Sort       | 8192 | Random             |   113,235.9 ns |    276.72 ns |   122.86 ns |   4.07 |    0.04 |    3 |         - |          NA |
| RotateLSDRadixSort  | 8192 | Random             | 3,389,209.9 ns |  2,304.82 ns |   821.92 ns | 121.81 |    1.11 |    6 |         - |          NA |
| RadixLSD256Sort     | 8192 | Random             |    39,250.7 ns |    148.79 ns |    77.82 ns |   1.41 |    0.01 |    2 |         - |          NA |
| Radsort             | 8192 | Random             |    94,726.6 ns |    647.99 ns |   287.71 ns |   3.40 |    0.03 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | Random             |   128,136.8 ns |    289.90 ns |   128.72 ns |   4.61 |    0.04 |    3 |         - |          NA |
| RadixMSD4Sort       | 8192 | Random             |   212,942.2 ns |  1,489.16 ns |   661.20 ns |   7.65 |    0.07 |    4 |         - |          NA |
| RadixMSD10Sort      | 8192 | Random             |   180,821.3 ns |    870.09 ns |   455.07 ns |   6.50 |    0.06 |    4 |         - |          NA |
| AmericanFlagSort    | 8192 | Random             |   121,472.0 ns |    331.97 ns |   147.40 ns |   4.37 |    0.04 |    3 |         - |          NA |
| SkaSort             | 8192 | Random             |    91,690.7 ns |    755.03 ns |   335.24 ns |   3.30 |    0.03 |    3 |         - |          NA |
| SpreadSort          | 8192 | Random             |    99,037.9 ns |    514.76 ns |   228.56 ns |   3.56 |    0.03 |    3 |         - |          NA |
| DotnetSort          | 8192 | Random             |   314,937.1 ns |  1,559.88 ns |   692.60 ns |  11.32 |    0.11 |    5 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **8192** | **SingleElementMoved** |    **38,292.8 ns** |    **508.82 ns** |   **225.92 ns** |   **1.59** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | SingleElementMoved |    24,017.8 ns |    458.48 ns |   239.80 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | SingleElementMoved |    30,329.5 ns |    381.44 ns |   199.50 ns |   1.26 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 8192 | SingleElementMoved |    19,425.3 ns |    194.32 ns |   101.63 ns |   0.81 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | SingleElementMoved |    43,867.1 ns |    500.77 ns |   222.34 ns |   1.83 |    0.02 |    3 |         - |          NA |
| BucketSortInteger   | 8192 | SingleElementMoved |    33,244.4 ns |    417.92 ns |   218.58 ns |   1.38 |    0.02 |    3 |         - |          NA |
| FlashSort           | 8192 | SingleElementMoved |    94,200.4 ns |  1,668.50 ns |   872.66 ns |   3.92 |    0.05 |    5 |         - |          NA |
| RadixLSD4Sort       | 8192 | SingleElementMoved |   159,667.5 ns |  1,820.59 ns |   952.20 ns |   6.65 |    0.07 |    7 |         - |          NA |
| RotateLSDRadixSort  | 8192 | SingleElementMoved | 2,175,131.4 ns |  6,511.03 ns | 2,890.94 ns |  90.57 |    0.86 |    9 |         - |          NA |
| RadixLSD256Sort     | 8192 | SingleElementMoved |    40,107.7 ns |    397.53 ns |   176.50 ns |   1.67 |    0.02 |    3 |         - |          NA |
| Radsort             | 8192 | SingleElementMoved |    61,376.9 ns |    205.41 ns |   107.43 ns |   2.56 |    0.02 |    4 |         - |          NA |
| RadixLSD10Sort      | 8192 | SingleElementMoved |   127,648.6 ns |    820.76 ns |   292.69 ns |   5.32 |    0.05 |    6 |         - |          NA |
| RadixMSD4Sort       | 8192 | SingleElementMoved |   217,136.7 ns |  1,410.18 ns |   626.13 ns |   9.04 |    0.09 |    8 |         - |          NA |
| RadixMSD10Sort      | 8192 | SingleElementMoved |   180,123.5 ns |  1,806.49 ns |   802.09 ns |   7.50 |    0.08 |    7 |         - |          NA |
| AmericanFlagSort    | 8192 | SingleElementMoved |    67,894.4 ns |  1,003.93 ns |   445.75 ns |   2.83 |    0.03 |    4 |         - |          NA |
| SkaSort             | 8192 | SingleElementMoved |    56,621.7 ns |    543.35 ns |   284.18 ns |   2.36 |    0.02 |    4 |         - |          NA |
| SpreadSort          | 8192 | SingleElementMoved |    41,254.5 ns |  2,986.99 ns | 1,562.25 ns |   1.72 |    0.06 |    3 |         - |          NA |
| DotnetSort          | 8192 | SingleElementMoved |    53,210.7 ns |    496.12 ns |   176.92 ns |   2.22 |    0.02 |    4 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **8192** | **Sorted**             |    **39,760.1 ns** |  **1,794.96 ns** |   **938.80 ns** |   **1.69** |    **0.04** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Sorted             |    23,501.6 ns |    294.02 ns |   153.78 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 8192 | Sorted             |    30,218.2 ns |    503.29 ns |   263.23 ns |   1.29 |    0.01 |    4 |         - |          NA |
| PigeonSortInteger   | 8192 | Sorted             |    17,332.3 ns |    253.72 ns |   132.70 ns |   0.74 |    0.01 |    2 |         - |          NA |
| BucketSort          | 8192 | Sorted             |    44,703.1 ns |    336.39 ns |   175.94 ns |   1.90 |    0.01 |    4 |         - |          NA |
| BucketSortInteger   | 8192 | Sorted             |    33,540.0 ns |    168.41 ns |    88.08 ns |   1.43 |    0.01 |    4 |         - |          NA |
| FlashSort           | 8192 | Sorted             |    90,007.3 ns |    982.65 ns |   436.30 ns |   3.83 |    0.03 |    5 |         - |          NA |
| RadixLSD4Sort       | 8192 | Sorted             |   156,227.3 ns |  2,184.52 ns | 1,142.55 ns |   6.65 |    0.06 |    7 |         - |          NA |
| RotateLSDRadixSort  | 8192 | Sorted             | 2,165,434.2 ns |  3,430.43 ns | 1,794.18 ns |  92.14 |    0.57 |    9 |         - |          NA |
| RadixLSD256Sort     | 8192 | Sorted             |    37,294.1 ns |    347.63 ns |   181.82 ns |   1.59 |    0.01 |    4 |         - |          NA |
| Radsort             | 8192 | Sorted             |    57,364.0 ns |    320.25 ns |   167.50 ns |   2.44 |    0.02 |    4 |         - |          NA |
| RadixLSD10Sort      | 8192 | Sorted             |   126,091.0 ns |    302.19 ns |   107.76 ns |   5.37 |    0.03 |    6 |         - |          NA |
| RadixMSD4Sort       | 8192 | Sorted             |   213,162.1 ns |  1,006.45 ns |   446.87 ns |   9.07 |    0.06 |    8 |         - |          NA |
| RadixMSD10Sort      | 8192 | Sorted             |   175,201.6 ns |    788.30 ns |   350.01 ns |   7.46 |    0.05 |    7 |         - |          NA |
| AmericanFlagSort    | 8192 | Sorted             |    55,688.3 ns |    934.10 ns |   333.11 ns |   2.37 |    0.02 |    4 |         - |          NA |
| SkaSort             | 8192 | Sorted             |    50,770.6 ns |    552.13 ns |   245.15 ns |   2.16 |    0.02 |    4 |         - |          NA |
| SpreadSort          | 8192 | Sorted             |     3,515.4 ns |     40.25 ns |    17.87 ns |   0.15 |    0.00 |    1 |         - |          NA |
| DotnetSort          | 8192 | Sorted             |    33,411.2 ns |  3,449.90 ns | 1,531.78 ns |   1.42 |    0.06 |    4 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **8192** | **Reversed**           |    **35,350.0 ns** |    **302.15 ns** |   **158.03 ns** |   **1.48** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Reversed           |    23,838.2 ns |    487.53 ns |   254.99 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | Reversed           |    32,518.7 ns |    175.25 ns |    91.66 ns |   1.36 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 8192 | Reversed           |    17,344.5 ns |    186.29 ns |    82.71 ns |   0.73 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | Reversed           |    42,131.4 ns |    308.94 ns |   137.17 ns |   1.77 |    0.02 |    3 |         - |          NA |
| BucketSortInteger   | 8192 | Reversed           |    31,551.7 ns |    163.88 ns |    72.77 ns |   1.32 |    0.01 |    3 |         - |          NA |
| FlashSort           | 8192 | Reversed           |    83,537.6 ns |    633.12 ns |   281.11 ns |   3.50 |    0.04 |    5 |         - |          NA |
| RadixLSD4Sort       | 8192 | Reversed           |   159,006.0 ns |    844.85 ns |   375.12 ns |   6.67 |    0.07 |    7 |         - |          NA |
| RotateLSDRadixSort  | 8192 | Reversed           | 2,205,210.3 ns |  6,731.63 ns | 2,988.88 ns |  92.52 |    0.93 |    9 |         - |          NA |
| RadixLSD256Sort     | 8192 | Reversed           |    37,169.9 ns |    266.47 ns |   139.37 ns |   1.56 |    0.02 |    3 |         - |          NA |
| Radsort             | 8192 | Reversed           |    56,740.8 ns |    430.23 ns |   191.02 ns |   2.38 |    0.02 |    4 |         - |          NA |
| RadixLSD10Sort      | 8192 | Reversed           |   126,043.2 ns |    471.64 ns |   209.41 ns |   5.29 |    0.05 |    6 |         - |          NA |
| RadixMSD4Sort       | 8192 | Reversed           |   216,647.7 ns |    949.92 ns |   421.77 ns |   9.09 |    0.09 |    8 |         - |          NA |
| RadixMSD10Sort      | 8192 | Reversed           |   175,477.8 ns |    695.50 ns |   308.80 ns |   7.36 |    0.07 |    7 |         - |          NA |
| AmericanFlagSort    | 8192 | Reversed           |    61,386.7 ns |    593.57 ns |   310.45 ns |   2.58 |    0.03 |    4 |         - |          NA |
| SkaSort             | 8192 | Reversed           |    57,621.2 ns |    353.14 ns |   184.70 ns |   2.42 |    0.03 |    4 |         - |          NA |
| SpreadSort          | 8192 | Reversed           |    55,481.0 ns |    907.13 ns |   402.77 ns |   2.33 |    0.03 |    4 |         - |          NA |
| DotnetSort          | 8192 | Reversed           |    59,809.8 ns |  3,332.33 ns | 1,742.87 ns |   2.51 |    0.07 |    4 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **8192** | **PipeOrgan**          |    **36,753.7 ns** |    **217.34 ns** |   **113.68 ns** |   **1.39** |    **0.01** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | PipeOrgan          |    26,430.7 ns |    407.08 ns |   212.91 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | PipeOrgan          |    30,395.3 ns |    290.83 ns |   129.13 ns |   1.15 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | PipeOrgan          |    19,727.2 ns |    228.60 ns |   119.56 ns |   0.75 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | PipeOrgan          |    40,497.8 ns |    213.57 ns |    94.82 ns |   1.53 |    0.01 |    2 |         - |          NA |
| BucketSortInteger   | 8192 | PipeOrgan          |    35,599.9 ns |    521.58 ns |   272.80 ns |   1.35 |    0.01 |    2 |         - |          NA |
| FlashSort           | 8192 | PipeOrgan          |   114,107.4 ns |  1,383.92 ns |   723.82 ns |   4.32 |    0.04 |    4 |         - |          NA |
| RadixLSD4Sort       | 8192 | PipeOrgan          |   134,990.8 ns |    548.35 ns |   243.47 ns |   5.11 |    0.04 |    4 |         - |          NA |
| RotateLSDRadixSort  | 8192 | PipeOrgan          | 2,175,042.0 ns |  4,582.33 ns | 2,034.58 ns |  82.30 |    0.63 |    8 |         - |          NA |
| RadixLSD256Sort     | 8192 | PipeOrgan          |    40,673.0 ns |    601.64 ns |   314.67 ns |   1.54 |    0.02 |    2 |         - |          NA |
| Radsort             | 8192 | PipeOrgan          |    59,082.1 ns |    248.72 ns |   130.09 ns |   2.24 |    0.02 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | PipeOrgan          |   127,265.6 ns |    952.87 ns |   339.80 ns |   4.82 |    0.04 |    4 |         - |          NA |
| RadixMSD4Sort       | 8192 | PipeOrgan          |   221,356.7 ns |    917.44 ns |   407.35 ns |   8.38 |    0.07 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | PipeOrgan          |   179,806.9 ns |    530.85 ns |   235.70 ns |   6.80 |    0.05 |    5 |         - |          NA |
| AmericanFlagSort    | 8192 | PipeOrgan          |    98,797.0 ns |    471.22 ns |   209.23 ns |   3.74 |    0.03 |    4 |         - |          NA |
| SkaSort             | 8192 | PipeOrgan          |    86,833.4 ns |  1,450.07 ns |   758.42 ns |   3.29 |    0.04 |    4 |         - |          NA |
| SpreadSort          | 8192 | PipeOrgan          |    67,015.1 ns |    446.41 ns |   233.48 ns |   2.54 |    0.02 |    3 |         - |          NA |
| DotnetSort          | 8192 | PipeOrgan          |   283,860.8 ns |  6,061.53 ns | 3,170.30 ns |  10.74 |    0.14 |    7 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **8192** | **ManyDuplicates**     |    **33,744.1 ns** |    **165.93 ns** |    **73.67 ns** |   **1.52** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | ManyDuplicates     |    22,144.0 ns |    441.18 ns |   195.89 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | ManyDuplicates     |    54,448.4 ns |    503.08 ns |   223.37 ns |   2.46 |    0.02 |    5 |         - |          NA |
| PigeonSortInteger   | 8192 | ManyDuplicates     |    17,336.3 ns |    300.76 ns |   133.54 ns |   0.78 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | ManyDuplicates     |    60,658.3 ns |  1,020.28 ns |   533.62 ns |   2.74 |    0.03 |    5 |         - |          NA |
| BucketSortInteger   | 8192 | ManyDuplicates     |    32,135.9 ns |    424.97 ns |   222.27 ns |   1.45 |    0.02 |    3 |         - |          NA |
| FlashSort           | 8192 | ManyDuplicates     |   108,281.6 ns |  3,951.29 ns | 2,066.60 ns |   4.89 |    0.10 |    6 |         - |          NA |
| RadixLSD4Sort       | 8192 | ManyDuplicates     |    54,244.9 ns |    248.87 ns |   130.16 ns |   2.45 |    0.02 |    5 |         - |          NA |
| RotateLSDRadixSort  | 8192 | ManyDuplicates     | 1,629,095.2 ns |  6,474.59 ns | 2,874.76 ns |  73.57 |    0.62 |    8 |         - |          NA |
| RadixLSD256Sort     | 8192 | ManyDuplicates     |    25,028.8 ns |    114.10 ns |    40.69 ns |   1.13 |    0.01 |    2 |         - |          NA |
| Radsort             | 8192 | ManyDuplicates     |    53,592.4 ns |    215.72 ns |    95.78 ns |   2.42 |    0.02 |    5 |         - |          NA |
| RadixLSD10Sort      | 8192 | ManyDuplicates     |    69,400.2 ns |  1,283.39 ns |   671.24 ns |   3.13 |    0.04 |    5 |         - |          NA |
| RadixMSD4Sort       | 8192 | ManyDuplicates     |    56,365.7 ns |    448.89 ns |   234.78 ns |   2.55 |    0.02 |    5 |         - |          NA |
| RadixMSD10Sort      | 8192 | ManyDuplicates     |    72,384.4 ns |    318.47 ns |   141.40 ns |   3.27 |    0.03 |    5 |         - |          NA |
| AmericanFlagSort    | 8192 | ManyDuplicates     |    52,591.1 ns |    237.03 ns |   123.97 ns |   2.38 |    0.02 |    5 |         - |          NA |
| SkaSort             | 8192 | ManyDuplicates     |    33,856.2 ns |    333.75 ns |   174.56 ns |   1.53 |    0.01 |    3 |         - |          NA |
| SpreadSort          | 8192 | ManyDuplicates     |    42,121.9 ns |    314.66 ns |   139.71 ns |   1.90 |    0.02 |    4 |         - |          NA |
| DotnetSort          | 8192 | ManyDuplicates     |   190,235.3 ns |    462.43 ns |   205.32 ns |   8.59 |    0.07 |    7 |         - |          NA |

### ExchangeBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.49 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method             | Size | Pattern            | Mean         | Error       | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------- |----- |------------------- |-------------:|------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **BubbleSort**         | **256**  | **Random**             |  **41,875.1 ns** |   **313.95 ns** |   **164.20 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Random             |  27,288.0 ns |   214.66 ns |   112.27 ns |  0.65 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 256  | Random             |  42,002.6 ns |   287.20 ns |   150.21 ns |  1.00 |    0.01 |    4 |         - |          NA |
| CombSort           | 256  | Random             |   7,871.7 ns |    67.16 ns |    29.82 ns |  0.19 |    0.00 |    2 |         - |          NA |
| CircleSort         | 256  | Random             |  23,031.0 ns |   124.72 ns |    55.38 ns |  0.55 |    0.00 |    3 |         - |          NA |
| DotnetSort         | 256  | Random             |   5,917.1 ns |   412.66 ns |   215.83 ns |  0.14 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **256**  | **SingleElementMoved** |   **4,476.0 ns** |    **18.52 ns** |     **6.60 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 256  | SingleElementMoved |     304.0 ns |    12.64 ns |     6.61 ns |  0.07 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 256  | SingleElementMoved |   8,598.0 ns |   347.01 ns |   181.49 ns |  1.92 |    0.04 |    5 |         - |          NA |
| CombSort           | 256  | SingleElementMoved |   2,672.4 ns |    37.00 ns |    13.19 ns |  0.60 |    0.00 |    3 |         - |          NA |
| CircleSort         | 256  | SingleElementMoved |  10,248.0 ns |   237.17 ns |   124.04 ns |  2.29 |    0.03 |    5 |         - |          NA |
| DotnetSort         | 256  | SingleElementMoved |   1,070.0 ns |   131.73 ns |    68.90 ns |  0.24 |    0.01 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **256**  | **Sorted**             |     **220.0 ns** |    **12.62 ns** |     **5.60 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Sorted             |     179.8 ns |     4.80 ns |     1.71 ns |  0.82 |    0.02 |    1 |         - |          NA |
| OddEvenSort        | 256  | Sorted             |     208.0 ns |    23.75 ns |    10.54 ns |  0.95 |    0.05 |    1 |         - |          NA |
| CombSort           | 256  | Sorted             |   2,279.5 ns |    48.91 ns |    17.44 ns | 10.37 |    0.25 |    4 |         - |          NA |
| CircleSort         | 256  | Sorted             |   1,737.5 ns |   237.04 ns |   105.25 ns |  7.90 |    0.48 |    3 |         - |          NA |
| DotnetSort         | 256  | Sorted             |     660.3 ns |    30.07 ns |    13.35 ns |  3.00 |    0.09 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **256**  | **Reversed**           |  **22,927.8 ns** |   **230.86 ns** |   **120.75 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Reversed           |  21,660.8 ns |   167.46 ns |    74.35 ns |  0.94 |    0.01 |    4 |         - |          NA |
| OddEvenSort        | 256  | Reversed           |  19,808.7 ns |   657.82 ns |   344.05 ns |  0.86 |    0.01 |    4 |         - |          NA |
| CombSort           | 256  | Reversed           |   2,481.9 ns |   280.84 ns |   146.89 ns |  0.11 |    0.01 |    2 |         - |          NA |
| CircleSort         | 256  | Reversed           |   3,417.5 ns |    76.59 ns |    34.01 ns |  0.15 |    0.00 |    3 |         - |          NA |
| DotnetSort         | 256  | Reversed           |   1,112.5 ns |   182.07 ns |    95.23 ns |  0.05 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **256**  | **PipeOrgan**          |  **26,867.0 ns** |   **503.70 ns** |   **263.44 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| CocktailShakerSort | 256  | PipeOrgan          |  14,044.1 ns |   281.37 ns |   147.16 ns |  0.52 |    0.01 |    3 |         - |          NA |
| OddEvenSort        | 256  | PipeOrgan          |  21,852.5 ns |   157.00 ns |    82.11 ns |  0.81 |    0.01 |    4 |         - |          NA |
| CombSort           | 256  | PipeOrgan          |   2,433.4 ns |    48.43 ns |    21.50 ns |  0.09 |    0.00 |    2 |         - |          NA |
| CircleSort         | 256  | PipeOrgan          |  15,426.9 ns |   101.18 ns |    52.92 ns |  0.57 |    0.01 |    3 |         - |          NA |
| DotnetSort         | 256  | PipeOrgan          |   1,817.2 ns |   196.65 ns |    87.31 ns |  0.07 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **256**  | **ManyDuplicates**     |  **41,085.7 ns** |   **258.42 ns** |   **135.16 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 256  | ManyDuplicates     |  26,803.5 ns |   543.82 ns |   284.43 ns |  0.65 |    0.01 |    3 |         - |          NA |
| OddEvenSort        | 256  | ManyDuplicates     |  43,904.4 ns |   449.97 ns |   199.79 ns |  1.07 |    0.01 |    4 |         - |          NA |
| CombSort           | 256  | ManyDuplicates     |   6,190.8 ns |   405.25 ns |   211.95 ns |  0.15 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | ManyDuplicates     |  19,964.7 ns |   141.04 ns |    73.77 ns |  0.49 |    0.00 |    2 |         - |          NA |
| DotnetSort         | 256  | ManyDuplicates     |   5,659.8 ns |   376.58 ns |   196.96 ns |  0.14 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **1024** | **Random**             | **531,175.7 ns** | **2,861.42 ns** | **1,270.49 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Random             | 296,992.0 ns |   816.53 ns |   362.54 ns |  0.56 |    0.00 |    4 |         - |          NA |
| OddEvenSort        | 1024 | Random             | 479,929.0 ns | 1,098.56 ns |   487.77 ns |  0.90 |    0.00 |    5 |         - |          NA |
| CombSort           | 1024 | Random             |  42,468.7 ns |   289.80 ns |   151.57 ns |  0.08 |    0.00 |    2 |         - |          NA |
| CircleSort         | 1024 | Random             | 127,349.5 ns |   441.60 ns |   196.07 ns |  0.24 |    0.00 |    3 |         - |          NA |
| DotnetSort         | 1024 | Random             |  29,948.6 ns |   493.17 ns |   257.94 ns |  0.06 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **1024** | **SingleElementMoved** |  **66,627.3 ns** |   **122.29 ns** |    **54.30 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | SingleElementMoved |   1,088.7 ns |     6.30 ns |     2.25 ns |  0.02 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 1024 | SingleElementMoved | 122,190.7 ns |   817.43 ns |   362.94 ns |  1.83 |    0.01 |    5 |         - |          NA |
| CombSort           | 1024 | SingleElementMoved |  12,724.7 ns |   209.32 ns |   109.48 ns |  0.19 |    0.00 |    3 |         - |          NA |
| CircleSort         | 1024 | SingleElementMoved |  57,737.8 ns |   195.43 ns |    86.77 ns |  0.87 |    0.00 |    4 |         - |          NA |
| DotnetSort         | 1024 | SingleElementMoved |   5,359.5 ns |   339.88 ns |   177.76 ns |  0.08 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **1024** | **Sorted**             |     **885.4 ns** |    **55.09 ns** |    **19.65 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Sorted             |     624.3 ns |    41.17 ns |    21.53 ns |  0.71 |    0.03 |    1 |         - |          NA |
| OddEvenSort        | 1024 | Sorted             |     775.2 ns |    35.65 ns |    15.83 ns |  0.88 |    0.02 |    2 |         - |          NA |
| CombSort           | 1024 | Sorted             |  11,668.2 ns |   261.59 ns |   136.82 ns | 13.18 |    0.30 |    5 |         - |          NA |
| CircleSort         | 1024 | Sorted             |   7,295.8 ns |   299.15 ns |   132.82 ns |  8.24 |    0.22 |    4 |         - |          NA |
| DotnetSort         | 1024 | Sorted             |   3,146.7 ns |    39.28 ns |    17.44 ns |  3.56 |    0.07 |    3 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **1024** | **Reversed**           | **340,463.4 ns** | **2,019.41 ns** |   **896.63 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Reversed           | 338,494.6 ns |   685.58 ns |   304.40 ns |  0.99 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | Reversed           | 316,982.4 ns | 1,414.98 ns |   628.26 ns |  0.93 |    0.00 |    3 |         - |          NA |
| CombSort           | 1024 | Reversed           |  12,827.9 ns |   277.68 ns |   145.23 ns |  0.04 |    0.00 |    2 |         - |          NA |
| CircleSort         | 1024 | Reversed           |  14,764.7 ns |   233.14 ns |   121.93 ns |  0.04 |    0.00 |    2 |         - |          NA |
| DotnetSort         | 1024 | Reversed           |   5,982.5 ns | 1,174.28 ns |   614.17 ns |  0.02 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **1024** | **PipeOrgan**          | **394,238.3 ns** | **1,310.47 ns** |   **581.86 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | PipeOrgan          | 220,652.6 ns | 1,176.66 ns |   615.41 ns |  0.56 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | PipeOrgan          | 349,043.2 ns | 1,470.64 ns |   652.97 ns |  0.89 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | PipeOrgan          |  12,717.7 ns |   292.82 ns |   130.01 ns |  0.03 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | PipeOrgan          |  81,851.0 ns | 1,293.33 ns |   676.44 ns |  0.21 |    0.00 |    2 |         - |          NA |
| DotnetSort         | 1024 | PipeOrgan          |  12,953.4 ns |   227.70 ns |    81.20 ns |  0.03 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **1024** | **ManyDuplicates**     | **529,007.8 ns** | **3,242.02 ns** | **1,439.48 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | ManyDuplicates     | 291,145.5 ns | 2,346.37 ns | 1,041.80 ns |  0.55 |    0.00 |    4 |         - |          NA |
| OddEvenSort        | 1024 | ManyDuplicates     | 496,308.7 ns | 4,888.21 ns | 1,743.18 ns |  0.94 |    0.00 |    5 |         - |          NA |
| CombSort           | 1024 | ManyDuplicates     |  28,228.6 ns |   238.94 ns |   124.97 ns |  0.05 |    0.00 |    2 |         - |          NA |
| CircleSort         | 1024 | ManyDuplicates     |  98,362.8 ns |   291.49 ns |   129.42 ns |  0.19 |    0.00 |    3 |         - |          NA |
| DotnetSort         | 1024 | ManyDuplicates     |  23,109.5 ns |   222.31 ns |    79.28 ns |  0.04 |    0.00 |    1 |         - |          NA |

### HeapBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.49 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method           | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **HeapSort**         | **256**  | **Random**             |     **6,934.5 ns** |     **95.09 ns** |     **42.22 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | Random             |     6,898.4 ns |    175.45 ns |     91.76 ns |  0.99 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | Random             |    10,624.3 ns |    281.70 ns |    147.34 ns |  1.53 |    0.02 |    2 |         - |          NA |
| BottomupHeapSort | 256  | Random             |     7,046.6 ns |    131.62 ns |     58.44 ns |  1.02 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 256  | Random             |    12,495.9 ns |    269.74 ns |    119.77 ns |  1.80 |    0.02 |    2 |         - |          NA |
| SmoothSort       | 256  | Random             |    13,618.3 ns |    323.22 ns |    169.05 ns |  1.96 |    0.03 |    2 |         - |          NA |
| PoplarHeapSort   | 256  | Random             |    13,472.9 ns |    165.67 ns |     73.56 ns |  1.94 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 256  | Random             |    13,250.0 ns |    257.81 ns |    114.47 ns |  1.91 |    0.02 |    2 |         - |          NA |
| BinomialHeapSort | 256  | Random             |    27,562.9 ns |    357.17 ns |    186.80 ns |  3.97 |    0.03 |    3 |         - |          NA |
| PairingHeapSort  | 256  | Random             |    16,141.7 ns |     31.68 ns |     14.07 ns |  2.33 |    0.01 |    2 |         - |          NA |
| DotnetSort       | 256  | Random             |     5,960.1 ns |    218.16 ns |    114.10 ns |  0.86 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **SingleElementMoved** |     **5,875.4 ns** |    **185.23 ns** |     **96.88 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| MinHeapSort      | 256  | SingleElementMoved |     5,504.1 ns |    245.08 ns |    128.18 ns |  0.94 |    0.03 |    5 |         - |          NA |
| TernaryHeapSort  | 256  | SingleElementMoved |     3,802.6 ns |    276.83 ns |    122.92 ns |  0.65 |    0.02 |    4 |         - |          NA |
| BottomupHeapSort | 256  | SingleElementMoved |     5,728.4 ns |    224.65 ns |    117.50 ns |  0.98 |    0.02 |    5 |         - |          NA |
| WeakHeapSort     | 256  | SingleElementMoved |     5,341.4 ns |     59.13 ns |     21.09 ns |  0.91 |    0.01 |    5 |         - |          NA |
| SmoothSort       | 256  | SingleElementMoved |     1,315.5 ns |    111.31 ns |     49.42 ns |  0.22 |    0.01 |    2 |         - |          NA |
| PoplarHeapSort   | 256  | SingleElementMoved |     3,010.6 ns |    137.82 ns |     61.19 ns |  0.51 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 256  | SingleElementMoved |     4,460.3 ns |    380.73 ns |    199.13 ns |  0.76 |    0.03 |    4 |         - |          NA |
| BinomialHeapSort | 256  | SingleElementMoved |     6,106.4 ns |    166.59 ns |     87.13 ns |  1.04 |    0.02 |    5 |         - |          NA |
| PairingHeapSort  | 256  | SingleElementMoved |     4,411.8 ns |    257.93 ns |    134.90 ns |  0.75 |    0.02 |    4 |         - |          NA |
| DotnetSort       | 256  | SingleElementMoved |     1,053.9 ns |    132.11 ns |     69.09 ns |  0.18 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **Sorted**             |     **5,614.7 ns** |    **247.57 ns** |    **129.49 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| MinHeapSort      | 256  | Sorted             |     5,231.7 ns |    190.46 ns |     99.62 ns |  0.93 |    0.03 |    5 |         - |          NA |
| TernaryHeapSort  | 256  | Sorted             |     3,251.0 ns |    194.57 ns |     86.39 ns |  0.58 |    0.02 |    4 |         - |          NA |
| BottomupHeapSort | 256  | Sorted             |     5,447.3 ns |    202.57 ns |     89.94 ns |  0.97 |    0.03 |    5 |         - |          NA |
| WeakHeapSort     | 256  | Sorted             |     5,043.5 ns |    280.59 ns |    146.76 ns |  0.90 |    0.03 |    5 |         - |          NA |
| SmoothSort       | 256  | Sorted             |       912.0 ns |      8.37 ns |      3.72 ns |  0.16 |    0.00 |    2 |         - |          NA |
| PoplarHeapSort   | 256  | Sorted             |     2,639.4 ns |    151.74 ns |     67.38 ns |  0.47 |    0.02 |    3 |         - |          NA |
| TournamentSort   | 256  | Sorted             |     4,284.1 ns |    407.03 ns |    212.88 ns |  0.76 |    0.04 |    5 |         - |          NA |
| BinomialHeapSort | 256  | Sorted             |     5,266.1 ns |    313.61 ns |    164.03 ns |  0.94 |    0.03 |    5 |         - |          NA |
| PairingHeapSort  | 256  | Sorted             |     4,134.4 ns |     55.19 ns |     19.68 ns |  0.74 |    0.02 |    5 |         - |          NA |
| DotnetSort       | 256  | Sorted             |       656.1 ns |     14.68 ns |      6.52 ns |  0.12 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **Reversed**           |     **5,100.7 ns** |     **51.54 ns** |     **18.38 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 256  | Reversed           |     5,739.3 ns |    248.23 ns |    129.83 ns |  1.13 |    0.02 |    4 |         - |          NA |
| TernaryHeapSort  | 256  | Reversed           |     2,790.2 ns |    285.39 ns |    149.26 ns |  0.55 |    0.03 |    3 |         - |          NA |
| BottomupHeapSort | 256  | Reversed           |     5,632.1 ns |    194.98 ns |     86.57 ns |  1.10 |    0.02 |    4 |         - |          NA |
| WeakHeapSort     | 256  | Reversed           |     5,678.9 ns |    212.84 ns |    111.32 ns |  1.11 |    0.02 |    4 |         - |          NA |
| SmoothSort       | 256  | Reversed           |     3,873.6 ns |    198.30 ns |    103.72 ns |  0.76 |    0.02 |    4 |         - |          NA |
| PoplarHeapSort   | 256  | Reversed           |     4,672.4 ns |    324.12 ns |    169.52 ns |  0.92 |    0.03 |    4 |         - |          NA |
| TournamentSort   | 256  | Reversed           |     4,420.5 ns |    432.30 ns |    226.10 ns |  0.87 |    0.04 |    4 |         - |          NA |
| BinomialHeapSort | 256  | Reversed           |     5,278.5 ns |    404.32 ns |    211.47 ns |  1.03 |    0.04 |    4 |         - |          NA |
| PairingHeapSort  | 256  | Reversed           |     2,004.8 ns |      9.07 ns |      4.03 ns |  0.39 |    0.00 |    2 |         - |          NA |
| DotnetSort       | 256  | Reversed           |     1,115.5 ns |    169.62 ns |     88.72 ns |  0.22 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **PipeOrgan**          |     **5,439.0 ns** |    **223.87 ns** |    **117.09 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 256  | PipeOrgan          |     5,186.5 ns |    231.96 ns |    121.32 ns |  0.95 |    0.03 |    3 |         - |          NA |
| TernaryHeapSort  | 256  | PipeOrgan          |     3,034.6 ns |    161.98 ns |     71.92 ns |  0.56 |    0.02 |    2 |         - |          NA |
| BottomupHeapSort | 256  | PipeOrgan          |     5,492.2 ns |    172.86 ns |     76.75 ns |  1.01 |    0.02 |    3 |         - |          NA |
| WeakHeapSort     | 256  | PipeOrgan          |     5,271.3 ns |    134.46 ns |     70.32 ns |  0.97 |    0.02 |    3 |         - |          NA |
| SmoothSort       | 256  | PipeOrgan          |     3,962.3 ns |    160.64 ns |     84.02 ns |  0.73 |    0.02 |    3 |         - |          NA |
| PoplarHeapSort   | 256  | PipeOrgan          |     4,613.8 ns |    205.43 ns |     91.21 ns |  0.85 |    0.02 |    3 |         - |          NA |
| TournamentSort   | 256  | PipeOrgan          |     3,136.8 ns |    308.34 ns |    161.27 ns |  0.58 |    0.03 |    2 |         - |          NA |
| BinomialHeapSort | 256  | PipeOrgan          |     6,001.4 ns |    278.33 ns |    145.57 ns |  1.10 |    0.03 |    3 |         - |          NA |
| PairingHeapSort  | 256  | PipeOrgan          |     5,284.0 ns |     43.45 ns |     15.50 ns |  0.97 |    0.02 |    3 |         - |          NA |
| DotnetSort       | 256  | PipeOrgan          |     1,964.1 ns |    271.04 ns |    141.76 ns |  0.36 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **ManyDuplicates**     |     **6,765.6 ns** |    **367.33 ns** |    **192.12 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 256  | ManyDuplicates     |     6,884.8 ns |    146.14 ns |     76.43 ns |  1.02 |    0.03 |    2 |         - |          NA |
| TernaryHeapSort  | 256  | ManyDuplicates     |    10,119.4 ns |    409.59 ns |    214.22 ns |  1.50 |    0.05 |    3 |         - |          NA |
| BottomupHeapSort | 256  | ManyDuplicates     |     7,030.6 ns |    228.47 ns |    119.49 ns |  1.04 |    0.03 |    2 |         - |          NA |
| WeakHeapSort     | 256  | ManyDuplicates     |    10,789.6 ns |    197.47 ns |    103.28 ns |  1.60 |    0.05 |    3 |         - |          NA |
| SmoothSort       | 256  | ManyDuplicates     |    12,628.4 ns |    314.43 ns |    164.45 ns |  1.87 |    0.06 |    3 |         - |          NA |
| PoplarHeapSort   | 256  | ManyDuplicates     |    12,926.6 ns |    241.65 ns |    107.29 ns |  1.91 |    0.05 |    3 |         - |          NA |
| TournamentSort   | 256  | ManyDuplicates     |    13,306.1 ns |    300.23 ns |    157.03 ns |  1.97 |    0.06 |    3 |         - |          NA |
| BinomialHeapSort | 256  | ManyDuplicates     |    24,615.3 ns |    337.03 ns |    149.64 ns |  3.64 |    0.10 |    4 |         - |          NA |
| PairingHeapSort  | 256  | ManyDuplicates     |    15,333.7 ns |    309.24 ns |    161.74 ns |  2.27 |    0.07 |    3 |         - |          NA |
| DotnetSort       | 256  | ManyDuplicates     |     5,437.6 ns |    111.74 ns |     39.85 ns |  0.80 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Random**             |    **30,543.0 ns** |    **166.99 ns** |     **59.55 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Random             |    30,865.4 ns |    233.25 ns |    122.00 ns |  1.01 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | Random             |    52,671.1 ns |    322.84 ns |    115.13 ns |  1.72 |    0.00 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | Random             |    32,398.3 ns |    213.74 ns |     94.90 ns |  1.06 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | Random             |    64,505.6 ns |    847.71 ns |    302.30 ns |  2.11 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 1024 | Random             |    70,923.3 ns |    271.72 ns |    120.64 ns |  2.32 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 1024 | Random             |    65,941.1 ns |    193.78 ns |    101.35 ns |  2.16 |    0.00 |    3 |         - |          NA |
| TournamentSort   | 1024 | Random             |    65,527.7 ns |    421.00 ns |    186.93 ns |  2.15 |    0.01 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | Random             |   156,392.5 ns |    668.54 ns |    296.84 ns |  5.12 |    0.01 |    4 |         - |          NA |
| PairingHeapSort  | 1024 | Random             |    80,850.2 ns |    272.28 ns |    120.89 ns |  2.65 |    0.01 |    3 |         - |          NA |
| DotnetSort       | 1024 | Random             |    29,819.2 ns |    479.02 ns |    212.69 ns |  0.98 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **SingleElementMoved** |    **25,717.2 ns** |    **546.27 ns** |    **285.71 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 1024 | SingleElementMoved |    24,848.7 ns |    206.42 ns |     73.61 ns |  0.97 |    0.01 |    3 |         - |          NA |
| TernaryHeapSort  | 1024 | SingleElementMoved |    19,261.8 ns |    395.81 ns |    175.74 ns |  0.75 |    0.01 |    3 |         - |          NA |
| BottomupHeapSort | 1024 | SingleElementMoved |    25,027.1 ns |    317.87 ns |    141.13 ns |  0.97 |    0.01 |    3 |         - |          NA |
| WeakHeapSort     | 1024 | SingleElementMoved |    25,731.3 ns |    343.09 ns |    179.44 ns |  1.00 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 1024 | SingleElementMoved |     4,806.4 ns |    222.50 ns |     98.79 ns |  0.19 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 1024 | SingleElementMoved |    12,996.9 ns |    258.83 ns |    135.37 ns |  0.51 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 1024 | SingleElementMoved |    21,563.0 ns |    622.57 ns |    276.43 ns |  0.84 |    0.01 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | SingleElementMoved |    26,245.5 ns |    337.64 ns |    176.59 ns |  1.02 |    0.01 |    3 |         - |          NA |
| PairingHeapSort  | 1024 | SingleElementMoved |    17,305.4 ns |    186.60 ns |     66.54 ns |  0.67 |    0.01 |    3 |         - |          NA |
| DotnetSort       | 1024 | SingleElementMoved |     5,266.1 ns |    315.94 ns |    140.28 ns |  0.20 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Sorted**             |    **24,900.3 ns** |    **225.76 ns** |    **118.08 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Sorted             |    23,275.0 ns |    206.39 ns |    107.95 ns |  0.93 |    0.01 |    3 |         - |          NA |
| TernaryHeapSort  | 1024 | Sorted             |    15,304.4 ns |    589.77 ns |    308.46 ns |  0.61 |    0.01 |    3 |         - |          NA |
| BottomupHeapSort | 1024 | Sorted             |    24,381.9 ns |    231.57 ns |    121.12 ns |  0.98 |    0.01 |    3 |         - |          NA |
| WeakHeapSort     | 1024 | Sorted             |    24,807.9 ns |    282.06 ns |    147.52 ns |  1.00 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 1024 | Sorted             |     3,702.3 ns |    100.28 ns |     44.53 ns |  0.15 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 1024 | Sorted             |    11,577.0 ns |    294.67 ns |    154.12 ns |  0.46 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 1024 | Sorted             |    20,373.9 ns |    183.87 ns |     65.57 ns |  0.82 |    0.00 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | Sorted             |    22,884.3 ns |    339.41 ns |    177.52 ns |  0.92 |    0.01 |    3 |         - |          NA |
| PairingHeapSort  | 1024 | Sorted             |    17,555.2 ns |    530.25 ns |    277.33 ns |  0.71 |    0.01 |    3 |         - |          NA |
| DotnetSort       | 1024 | Sorted             |     3,205.4 ns |    160.35 ns |     71.20 ns |  0.13 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Reversed**           |    **22,904.0 ns** |    **280.04 ns** |    **146.46 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Reversed           |    25,056.2 ns |    199.91 ns |    104.56 ns |  1.09 |    0.01 |    4 |         - |          NA |
| TernaryHeapSort  | 1024 | Reversed           |    13,045.5 ns |    229.31 ns |    101.81 ns |  0.57 |    0.01 |    3 |         - |          NA |
| BottomupHeapSort | 1024 | Reversed           |    25,166.6 ns |    259.65 ns |    135.80 ns |  1.10 |    0.01 |    4 |         - |          NA |
| WeakHeapSort     | 1024 | Reversed           |    27,790.0 ns |    389.78 ns |    203.86 ns |  1.21 |    0.01 |    4 |         - |          NA |
| SmoothSort       | 1024 | Reversed           |    18,696.2 ns |    418.43 ns |    185.78 ns |  0.82 |    0.01 |    4 |         - |          NA |
| PoplarHeapSort   | 1024 | Reversed           |    21,225.7 ns |    658.89 ns |    344.61 ns |  0.93 |    0.02 |    4 |         - |          NA |
| TournamentSort   | 1024 | Reversed           |    21,043.3 ns |    999.48 ns |    522.75 ns |  0.92 |    0.02 |    4 |         - |          NA |
| BinomialHeapSort | 1024 | Reversed           |    22,720.7 ns |    246.29 ns |    128.81 ns |  0.99 |    0.01 |    4 |         - |          NA |
| PairingHeapSort  | 1024 | Reversed           |     8,010.4 ns |    283.61 ns |    148.34 ns |  0.35 |    0.01 |    2 |         - |          NA |
| DotnetSort       | 1024 | Reversed           |     5,270.2 ns |    113.56 ns |     50.42 ns |  0.23 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **PipeOrgan**          |    **24,422.0 ns** |    **179.15 ns** |     **93.70 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | PipeOrgan          |    23,263.5 ns |    177.14 ns |     92.65 ns |  0.95 |    0.00 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | PipeOrgan          |    14,613.1 ns |    642.60 ns |    336.09 ns |  0.60 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | PipeOrgan          |    24,473.6 ns |    184.47 ns |     96.48 ns |  1.00 |    0.01 |    2 |         - |          NA |
| WeakHeapSort     | 1024 | PipeOrgan          |    26,971.7 ns |    257.38 ns |    134.62 ns |  1.10 |    0.01 |    2 |         - |          NA |
| SmoothSort       | 1024 | PipeOrgan          |    20,220.8 ns |    426.61 ns |    189.42 ns |  0.83 |    0.01 |    2 |         - |          NA |
| PoplarHeapSort   | 1024 | PipeOrgan          |    21,727.0 ns |    355.26 ns |    157.74 ns |  0.89 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 1024 | PipeOrgan          |    15,141.6 ns |    689.80 ns |    306.28 ns |  0.62 |    0.01 |    1 |         - |          NA |
| BinomialHeapSort | 1024 | PipeOrgan          |    25,793.5 ns |    280.21 ns |    146.56 ns |  1.06 |    0.01 |    2 |         - |          NA |
| PairingHeapSort  | 1024 | PipeOrgan          |    22,481.2 ns |    241.62 ns |    126.37 ns |  0.92 |    0.01 |    2 |         - |          NA |
| DotnetSort       | 1024 | PipeOrgan          |    12,990.4 ns |    511.92 ns |    227.29 ns |  0.53 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **ManyDuplicates**     |    **29,238.7 ns** |    **199.37 ns** |     **88.52 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | ManyDuplicates     |    29,120.2 ns |    437.24 ns |    228.69 ns |  1.00 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | ManyDuplicates     |    48,147.5 ns |    823.19 ns |    430.54 ns |  1.65 |    0.01 |    3 |         - |          NA |
| BottomupHeapSort | 1024 | ManyDuplicates     |    29,969.3 ns |    187.94 ns |     98.30 ns |  1.02 |    0.00 |    2 |         - |          NA |
| WeakHeapSort     | 1024 | ManyDuplicates     |    51,507.7 ns |  5,496.81 ns |  2,874.94 ns |  1.76 |    0.09 |    3 |         - |          NA |
| SmoothSort       | 1024 | ManyDuplicates     |    64,103.5 ns |    161.19 ns |     71.57 ns |  2.19 |    0.01 |    4 |         - |          NA |
| PoplarHeapSort   | 1024 | ManyDuplicates     |    61,996.7 ns |    140.25 ns |     62.27 ns |  2.12 |    0.01 |    4 |         - |          NA |
| TournamentSort   | 1024 | ManyDuplicates     |    65,313.0 ns |    788.85 ns |    350.26 ns |  2.23 |    0.01 |    4 |         - |          NA |
| BinomialHeapSort | 1024 | ManyDuplicates     |   121,985.6 ns |    685.55 ns |    304.39 ns |  4.17 |    0.02 |    5 |         - |          NA |
| PairingHeapSort  | 1024 | ManyDuplicates     |    69,702.9 ns |    649.41 ns |    288.34 ns |  2.38 |    0.01 |    4 |         - |          NA |
| DotnetSort       | 1024 | ManyDuplicates     |    23,299.8 ns |    328.86 ns |    172.00 ns |  0.80 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Random**             |   **130,550.1 ns** |    **600.98 ns** |    **314.33 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Random             |   132,977.8 ns |  1,350.68 ns |    706.43 ns |  1.02 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 4096 | Random             |   250,701.5 ns |  1,287.61 ns |    571.70 ns |  1.92 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 4096 | Random             |   143,789.4 ns |    558.41 ns |    247.94 ns |  1.10 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | Random             |   309,960.6 ns |  1,209.50 ns |    537.03 ns |  2.37 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 4096 | Random             |   342,572.3 ns |  2,482.79 ns |  1,298.55 ns |  2.62 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 4096 | Random             |   309,626.9 ns |    740.74 ns |    387.42 ns |  2.37 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 4096 | Random             |   312,849.9 ns |    613.61 ns |    272.45 ns |  2.40 |    0.01 |    3 |         - |          NA |
| BinomialHeapSort | 4096 | Random             |   809,653.6 ns |  3,715.83 ns |  1,649.85 ns |  6.20 |    0.02 |    4 |         - |          NA |
| PairingHeapSort  | 4096 | Random             |   391,877.0 ns |  2,011.00 ns |    892.90 ns |  3.00 |    0.01 |    3 |         - |          NA |
| DotnetSort       | 4096 | Random             |   144,402.8 ns |  2,028.96 ns |  1,061.19 ns |  1.11 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **SingleElementMoved** |   **102,590.6 ns** |    **577.61 ns** |    **302.10 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| MinHeapSort      | 4096 | SingleElementMoved |   105,433.2 ns |    537.70 ns |    238.74 ns |  1.03 |    0.00 |    5 |         - |          NA |
| TernaryHeapSort  | 4096 | SingleElementMoved |   121,133.6 ns |  4,563.70 ns |  2,026.31 ns |  1.18 |    0.02 |    5 |         - |          NA |
| BottomupHeapSort | 4096 | SingleElementMoved |   111,045.7 ns |    388.45 ns |    172.48 ns |  1.08 |    0.00 |    5 |         - |          NA |
| WeakHeapSort     | 4096 | SingleElementMoved |   123,986.9 ns |    575.35 ns |    255.46 ns |  1.21 |    0.00 |    5 |         - |          NA |
| SmoothSort       | 4096 | SingleElementMoved |    18,872.2 ns |    498.37 ns |    260.66 ns |  0.18 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 4096 | SingleElementMoved |    56,108.6 ns |    331.78 ns |    147.31 ns |  0.55 |    0.00 |    3 |         - |          NA |
| TournamentSort   | 4096 | SingleElementMoved |   128,050.4 ns | 23,084.77 ns | 12,073.78 ns |  1.25 |    0.11 |    5 |         - |          NA |
| BinomialHeapSort | 4096 | SingleElementMoved |   114,442.5 ns |    345.66 ns |    153.47 ns |  1.12 |    0.00 |    5 |         - |          NA |
| PairingHeapSort  | 4096 | SingleElementMoved |    68,941.5 ns |    234.54 ns |    104.14 ns |  0.67 |    0.00 |    4 |         - |          NA |
| DotnetSort       | 4096 | SingleElementMoved |    24,658.3 ns |    443.72 ns |    197.01 ns |  0.24 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Sorted**             |   **100,115.3 ns** |  **1,692.41 ns** |    **885.16 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Sorted             |    98,101.8 ns |    893.62 ns |    467.38 ns |  0.98 |    0.01 |    4 |         - |          NA |
| TernaryHeapSort  | 4096 | Sorted             |    76,564.3 ns |  2,170.05 ns |    963.52 ns |  0.76 |    0.01 |    3 |         - |          NA |
| BottomupHeapSort | 4096 | Sorted             |   108,897.1 ns |    225.71 ns |    100.22 ns |  1.09 |    0.01 |    4 |         - |          NA |
| WeakHeapSort     | 4096 | Sorted             |   121,250.0 ns |  1,007.14 ns |    526.75 ns |  1.21 |    0.01 |    4 |         - |          NA |
| SmoothSort       | 4096 | Sorted             |    15,046.2 ns |    108.73 ns |     48.28 ns |  0.15 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 4096 | Sorted             |    51,315.4 ns |    232.05 ns |    121.37 ns |  0.51 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 4096 | Sorted             |   105,967.8 ns |  8,421.39 ns |  4,404.55 ns |  1.06 |    0.04 |    4 |         - |          NA |
| BinomialHeapSort | 4096 | Sorted             |   102,266.0 ns |    684.98 ns |    304.14 ns |  1.02 |    0.01 |    4 |         - |          NA |
| PairingHeapSort  | 4096 | Sorted             |    69,908.4 ns |    413.42 ns |    183.56 ns |  0.70 |    0.01 |    3 |         - |          NA |
| DotnetSort       | 4096 | Sorted             |    15,124.0 ns |    276.44 ns |     98.58 ns |  0.15 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Reversed**           |    **97,650.0 ns** |    **415.09 ns** |    **217.10 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Reversed           |   102,536.8 ns |  5,291.19 ns |  2,767.39 ns |  1.05 |    0.03 |    3 |         - |          NA |
| TernaryHeapSort  | 4096 | Reversed           |    65,032.5 ns |  1,748.25 ns |    914.37 ns |  0.67 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 4096 | Reversed           |   111,597.1 ns |    742.14 ns |    329.52 ns |  1.14 |    0.00 |    3 |         - |          NA |
| WeakHeapSort     | 4096 | Reversed           |   131,116.5 ns |    739.46 ns |    328.32 ns |  1.34 |    0.00 |    3 |         - |          NA |
| SmoothSort       | 4096 | Reversed           |   103,448.4 ns |  3,858.21 ns |  1,713.07 ns |  1.06 |    0.02 |    3 |         - |          NA |
| PoplarHeapSort   | 4096 | Reversed           |    96,451.8 ns |  2,118.27 ns |  1,107.90 ns |  0.99 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 4096 | Reversed           |   181,252.2 ns | 30,803.34 ns | 16,110.74 ns |  1.86 |    0.16 |    4 |         - |          NA |
| BinomialHeapSort | 4096 | Reversed           |   100,454.0 ns |    517.18 ns |    184.43 ns |  1.03 |    0.00 |    3 |         - |          NA |
| PairingHeapSort  | 4096 | Reversed           |    31,825.2 ns |    556.95 ns |    291.30 ns |  0.33 |    0.00 |    1 |         - |          NA |
| DotnetSort       | 4096 | Reversed           |    28,483.2 ns |  2,487.67 ns |  1,301.10 ns |  0.29 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **PipeOrgan**          |   **101,596.9 ns** |    **604.26 ns** |    **215.48 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 4096 | PipeOrgan          |   100,011.7 ns |    445.70 ns |    197.89 ns |  0.98 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 4096 | PipeOrgan          |    72,453.9 ns |  1,683.44 ns |    880.47 ns |  0.71 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 4096 | PipeOrgan          |   109,995.8 ns |    421.16 ns |    187.00 ns |  1.08 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | PipeOrgan          |   131,891.9 ns |    727.22 ns |    322.89 ns |  1.30 |    0.00 |    1 |         - |          NA |
| SmoothSort       | 4096 | PipeOrgan          |   208,779.5 ns |  4,800.97 ns |  2,131.66 ns |  2.05 |    0.02 |    2 |         - |          NA |
| PoplarHeapSort   | 4096 | PipeOrgan          |   107,119.2 ns |  2,138.52 ns |    949.52 ns |  1.05 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 4096 | PipeOrgan          |    79,178.4 ns |  5,004.28 ns |  2,617.34 ns |  0.78 |    0.02 |    1 |         - |          NA |
| BinomialHeapSort | 4096 | PipeOrgan          |   112,877.7 ns |    893.77 ns |    396.84 ns |  1.11 |    0.00 |    1 |         - |          NA |
| PairingHeapSort  | 4096 | PipeOrgan          |    93,439.4 ns |    413.50 ns |    183.60 ns |  0.92 |    0.00 |    1 |         - |          NA |
| DotnetSort       | 4096 | PipeOrgan          |    70,958.5 ns |  2,404.84 ns |  1,067.76 ns |  0.70 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **ManyDuplicates**     |   **120,634.7 ns** |    **547.24 ns** |    **242.98 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 4096 | ManyDuplicates     |   121,736.9 ns |    467.54 ns |    207.59 ns |  1.01 |    0.00 |    2 |         - |          NA |
| TernaryHeapSort  | 4096 | ManyDuplicates     |   228,665.7 ns |    626.79 ns |    278.30 ns |  1.90 |    0.00 |    3 |         - |          NA |
| BottomupHeapSort | 4096 | ManyDuplicates     |   127,469.7 ns |    422.68 ns |    187.67 ns |  1.06 |    0.00 |    2 |         - |          NA |
| WeakHeapSort     | 4096 | ManyDuplicates     |   222,193.8 ns |    635.68 ns |    282.25 ns |  1.84 |    0.00 |    3 |         - |          NA |
| SmoothSort       | 4096 | ManyDuplicates     |   296,937.8 ns |  1,158.82 ns |    514.52 ns |  2.46 |    0.01 |    4 |         - |          NA |
| PoplarHeapSort   | 4096 | ManyDuplicates     |   290,950.9 ns |    484.41 ns |    253.36 ns |  2.41 |    0.00 |    4 |         - |          NA |
| TournamentSort   | 4096 | ManyDuplicates     |   310,822.6 ns |    978.28 ns |    511.66 ns |  2.58 |    0.01 |    4 |         - |          NA |
| BinomialHeapSort | 4096 | ManyDuplicates     |   546,055.5 ns |  2,722.79 ns |  1,424.07 ns |  4.53 |    0.01 |    5 |         - |          NA |
| PairingHeapSort  | 4096 | ManyDuplicates     |   303,857.8 ns |    897.79 ns |    398.62 ns |  2.52 |    0.01 |    4 |         - |          NA |
| DotnetSort       | 4096 | ManyDuplicates     |    94,272.1 ns |    560.29 ns |    199.80 ns |  0.78 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Random**             |   **268,387.5 ns** |    **844.54 ns** |    **374.98 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Random             |   272,228.3 ns |  1,832.70 ns |    958.54 ns |  1.01 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | Random             |   542,139.6 ns |  1,937.00 ns |    860.04 ns |  2.02 |    0.00 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | Random             |   308,995.3 ns |    853.36 ns |    446.32 ns |  1.15 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | Random             |   673,809.3 ns |  1,021.46 ns |    534.24 ns |  2.51 |    0.00 |    3 |         - |          NA |
| SmoothSort       | 8192 | Random             |   743,263.8 ns |  3,414.80 ns |  1,786.00 ns |  2.77 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 8192 | Random             |   663,095.6 ns |    953.79 ns |    423.49 ns |  2.47 |    0.00 |    3 |         - |          NA |
| TournamentSort   | 8192 | Random             |   691,464.1 ns |  1,104.69 ns |    490.49 ns |  2.58 |    0.00 |    3 |         - |          NA |
| BinomialHeapSort | 8192 | Random             | 1,782,314.3 ns |  8,303.98 ns |  4,343.14 ns |  6.64 |    0.02 |    4 |         - |          NA |
| PairingHeapSort  | 8192 | Random             |   871,657.7 ns |  6,376.76 ns |  3,335.17 ns |  3.25 |    0.01 |    3 |         - |          NA |
| DotnetSort       | 8192 | Random             |   315,398.0 ns |  1,501.59 ns |    785.36 ns |  1.18 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **SingleElementMoved** |   **216,444.3 ns** |  **1,647.48 ns** |    **861.67 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 8192 | SingleElementMoved |   216,161.8 ns |  1,429.65 ns |    634.77 ns |  1.00 |    0.00 |    4 |         - |          NA |
| TernaryHeapSort  | 8192 | SingleElementMoved |   347,759.5 ns |  8,735.87 ns |  4,569.03 ns |  1.61 |    0.02 |    5 |         - |          NA |
| BottomupHeapSort | 8192 | SingleElementMoved |   243,340.0 ns |  1,524.62 ns |    676.94 ns |  1.12 |    0.01 |    4 |         - |          NA |
| WeakHeapSort     | 8192 | SingleElementMoved |   270,623.8 ns |  3,022.29 ns |  1,341.92 ns |  1.25 |    0.01 |    4 |         - |          NA |
| SmoothSort       | 8192 | SingleElementMoved |    37,713.3 ns |    417.44 ns |    185.35 ns |  0.17 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 8192 | SingleElementMoved |   117,917.0 ns |    365.54 ns |    162.30 ns |  0.54 |    0.00 |    3 |         - |          NA |
| TournamentSort   | 8192 | SingleElementMoved |   382,423.4 ns |  1,616.39 ns |    717.69 ns |  1.77 |    0.01 |    5 |         - |          NA |
| BinomialHeapSort | 8192 | SingleElementMoved |   238,199.4 ns |  2,012.67 ns |    893.64 ns |  1.10 |    0.01 |    4 |         - |          NA |
| PairingHeapSort  | 8192 | SingleElementMoved |   140,368.6 ns |    273.15 ns |     97.41 ns |  0.65 |    0.00 |    3 |         - |          NA |
| DotnetSort       | 8192 | SingleElementMoved |    52,887.5 ns |    851.66 ns |    445.43 ns |  0.24 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Sorted**             |   **214,075.9 ns** |    **976.02 ns** |    **433.36 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Sorted             |   199,772.4 ns |    566.01 ns |    251.31 ns |  0.93 |    0.00 |    4 |         - |          NA |
| TernaryHeapSort  | 8192 | Sorted             |   333,752.5 ns |  2,412.85 ns |  1,071.32 ns |  1.56 |    0.01 |    5 |         - |          NA |
| BottomupHeapSort | 8192 | Sorted             |   239,410.0 ns |    937.68 ns |    416.33 ns |  1.12 |    0.00 |    4 |         - |          NA |
| WeakHeapSort     | 8192 | Sorted             |   266,025.2 ns |  1,064.00 ns |    556.49 ns |  1.24 |    0.00 |    4 |         - |          NA |
| SmoothSort       | 8192 | Sorted             |    29,916.6 ns |    302.24 ns |    158.08 ns |  0.14 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 8192 | Sorted             |   108,759.6 ns |  1,048.60 ns |    548.44 ns |  0.51 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 8192 | Sorted             |   386,848.4 ns | 25,635.28 ns | 13,407.75 ns |  1.81 |    0.06 |    5 |         - |          NA |
| BinomialHeapSort | 8192 | Sorted             |   213,778.4 ns |  1,628.47 ns |    723.05 ns |  1.00 |    0.00 |    4 |         - |          NA |
| PairingHeapSort  | 8192 | Sorted             |   140,717.1 ns |  1,043.24 ns |    545.63 ns |  0.66 |    0.00 |    3 |         - |          NA |
| DotnetSort       | 8192 | Sorted             |    34,235.4 ns |  2,190.80 ns |    972.73 ns |  0.16 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Reversed**           |   **198,711.9 ns** |    **770.36 ns** |    **342.04 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Reversed           |   218,563.1 ns |    695.38 ns |    363.69 ns |  1.10 |    0.00 |    2 |         - |          NA |
| TernaryHeapSort  | 8192 | Reversed           |   328,803.1 ns |  1,505.33 ns |    668.38 ns |  1.65 |    0.00 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | Reversed           |   244,620.6 ns |    894.59 ns |    397.20 ns |  1.23 |    0.00 |    2 |         - |          NA |
| WeakHeapSort     | 8192 | Reversed           |   285,397.4 ns |  1,624.93 ns |    849.87 ns |  1.44 |    0.00 |    2 |         - |          NA |
| SmoothSort       | 8192 | Reversed           |   439,161.8 ns |  1,139.33 ns |    595.89 ns |  2.21 |    0.00 |    3 |         - |          NA |
| PoplarHeapSort   | 8192 | Reversed           |   322,080.3 ns |  3,132.38 ns |  1,390.80 ns |  1.62 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 8192 | Reversed           |   505,629.2 ns |  4,069.37 ns |  2,128.36 ns |  2.54 |    0.01 |    3 |         - |          NA |
| BinomialHeapSort | 8192 | Reversed           |   212,390.6 ns |  1,333.50 ns |    592.08 ns |  1.07 |    0.00 |    2 |         - |          NA |
| PairingHeapSort  | 8192 | Reversed           |    63,489.1 ns |    380.74 ns |    199.14 ns |  0.32 |    0.00 |    1 |         - |          NA |
| DotnetSort       | 8192 | Reversed           |    59,874.9 ns |  3,095.14 ns |  1,618.82 ns |  0.30 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **PipeOrgan**          |   **213,309.9 ns** |  **3,293.06 ns** |  **1,462.14 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | PipeOrgan          |   205,666.8 ns |  4,339.41 ns |  2,269.60 ns |  0.96 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | PipeOrgan          |   374,505.9 ns |  1,090.23 ns |    388.79 ns |  1.76 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | PipeOrgan          |   242,930.4 ns |  2,029.14 ns |    900.95 ns |  1.14 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | PipeOrgan          |   290,265.5 ns |  2,676.57 ns |  1,188.41 ns |  1.36 |    0.01 |    1 |         - |          NA |
| SmoothSort       | 8192 | PipeOrgan          |   550,705.1 ns |  2,155.91 ns |  1,127.58 ns |  2.58 |    0.02 |    3 |         - |          NA |
| PoplarHeapSort   | 8192 | PipeOrgan          |   453,443.7 ns |  1,462.37 ns |    764.85 ns |  2.13 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 8192 | PipeOrgan          |   414,244.5 ns | 24,411.44 ns | 12,767.65 ns |  1.94 |    0.06 |    2 |         - |          NA |
| BinomialHeapSort | 8192 | PipeOrgan          |   235,998.5 ns |  2,297.56 ns |  1,201.67 ns |  1.11 |    0.01 |    1 |         - |          NA |
| PairingHeapSort  | 8192 | PipeOrgan          |   190,749.6 ns |    969.52 ns |    430.47 ns |  0.89 |    0.01 |    1 |         - |          NA |
| DotnetSort       | 8192 | PipeOrgan          |   281,005.0 ns |  5,680.72 ns |  2,522.27 ns |  1.32 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **ManyDuplicates**     |   **243,600.9 ns** |    **982.68 ns** |    **436.31 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 8192 | ManyDuplicates     |   248,191.1 ns |    912.00 ns |    476.99 ns |  1.02 |    0.00 |    2 |         - |          NA |
| TernaryHeapSort  | 8192 | ManyDuplicates     |   497,245.1 ns |  4,515.04 ns |  2,361.46 ns |  2.04 |    0.01 |    3 |         - |          NA |
| BottomupHeapSort | 8192 | ManyDuplicates     |   274,425.9 ns |  1,432.06 ns |    635.84 ns |  1.13 |    0.00 |    2 |         - |          NA |
| WeakHeapSort     | 8192 | ManyDuplicates     |   466,386.8 ns |  2,018.14 ns |    896.07 ns |  1.91 |    0.00 |    3 |         - |          NA |
| SmoothSort       | 8192 | ManyDuplicates     |   624,146.4 ns |  1,419.84 ns |    742.60 ns |  2.56 |    0.01 |    4 |         - |          NA |
| PoplarHeapSort   | 8192 | ManyDuplicates     |   615,608.2 ns |    624.98 ns |    277.50 ns |  2.53 |    0.00 |    4 |         - |          NA |
| TournamentSort   | 8192 | ManyDuplicates     |   660,313.6 ns |  6,238.41 ns |  2,769.89 ns |  2.71 |    0.01 |    4 |         - |          NA |
| BinomialHeapSort | 8192 | ManyDuplicates     | 1,136,178.5 ns |  6,450.78 ns |  2,864.18 ns |  4.66 |    0.01 |    5 |         - |          NA |
| PairingHeapSort  | 8192 | ManyDuplicates     |   641,459.5 ns |  5,294.35 ns |  2,350.72 ns |  2.63 |    0.01 |    4 |         - |          NA |
| DotnetSort       | 8192 | ManyDuplicates     |   190,198.4 ns |    575.73 ns |    255.63 ns |  0.78 |    0.00 |    1 |         - |          NA |

### InsertionBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.49 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                 | Size | Pattern            | Mean            | Error         | StdDev        | Ratio    | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------- |----- |------------------- |----------------:|--------------:|--------------:|---------:|--------:|-----:|----------:|------------:|
| **InsertionSort**          | **256**  | **Random**             |      **7,284.7 ns** |     **318.17 ns** |     **113.46 ns** |     **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Random             |      7,274.7 ns |     129.95 ns |      57.70 ns |     1.00 |    0.02 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | Random             |     11,516.5 ns |     263.05 ns |     137.58 ns |     1.58 |    0.03 |    3 |         - |          NA |
| GnomeSort              | 256  | Random             |     10,837.2 ns |     309.02 ns |     161.62 ns |     1.49 |    0.03 |    3 |         - |          NA |
| LibrarySort            | 256  | Random             |     22,648.5 ns |     377.16 ns |     197.26 ns |     3.11 |    0.05 |    4 |         - |          NA |
| MergeInsertionSort     | 256  | Random             |     20,281.5 ns |     187.54 ns |      83.27 ns |     2.78 |    0.04 |    4 |         - |          NA |
| ShellSortKnuth1973     | 256  | Random             |      8,180.0 ns |     213.14 ns |     111.47 ns |     1.12 |    0.02 |    2 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Random             |      8,178.5 ns |     179.22 ns |      79.58 ns |     1.12 |    0.02 |    2 |         - |          NA |
| ShellSortTokuda1992    | 256  | Random             |      8,618.6 ns |     303.80 ns |     158.89 ns |     1.18 |    0.03 |    2 |         - |          NA |
| ShellSortCiura2001     | 256  | Random             |      8,420.9 ns |     257.29 ns |     134.57 ns |     1.16 |    0.02 |    2 |         - |          NA |
| ShellSortLee2021       | 256  | Random             |      8,839.8 ns |     285.94 ns |     149.55 ns |     1.21 |    0.03 |    2 |         - |          NA |
| DotnetSort             | 256  | Random             |      5,889.1 ns |     333.81 ns |     174.59 ns |     0.81 |    0.03 |    1 |         - |          NA |
|      |                    |                 |               |               |          |         |      |           |             |
| **InsertionSort**          | **256**  | **SingleElementMoved** |        **228.9 ns** |      **17.40 ns** |       **7.73 ns** |     **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 256  | SingleElementMoved |        260.8 ns |      17.52 ns |       7.78 ns |     1.14 |    0.05 |    1 |         - |          NA |
| BinaryInsertSort       | 256  | SingleElementMoved |      1,002.9 ns |      38.86 ns |      13.86 ns |     4.39 |    0.15 |    3 |         - |          NA |
| GnomeSort              | 256  | SingleElementMoved |        319.4 ns |       5.71 ns |       2.98 ns |     1.40 |    0.04 |    2 |         - |          NA |
| LibrarySort            | 256  | SingleElementMoved |     21,671.9 ns |     142.14 ns |      74.34 ns |    94.75 |    2.92 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | SingleElementMoved |     10,047.8 ns |     252.79 ns |     112.24 ns |    43.93 |    1.43 |    4 |         - |          NA |
| ShellSortKnuth1973     | 256  | SingleElementMoved |        817.1 ns |      33.76 ns |      14.99 ns |     3.57 |    0.13 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 256  | SingleElementMoved |        821.1 ns |      11.54 ns |       5.12 ns |     3.59 |    0.11 |    3 |         - |          NA |
| ShellSortTokuda1992    | 256  | SingleElementMoved |      1,010.1 ns |      67.48 ns |      29.96 ns |     4.42 |    0.18 |    3 |         - |          NA |
| ShellSortCiura2001     | 256  | SingleElementMoved |        898.8 ns |     142.50 ns |      63.27 ns |     3.93 |    0.29 |    3 |         - |          NA |
| ShellSortLee2021       | 256  | SingleElementMoved |        995.4 ns |      53.52 ns |      19.09 ns |     4.35 |    0.15 |    3 |         - |          NA |
| DotnetSort             | 256  | SingleElementMoved |      1,067.0 ns |     108.76 ns |      48.29 ns |     4.67 |    0.24 |    3 |         - |          NA |
|      |                    |                 |               |               |          |         |      |           |             |
| **InsertionSort**          | **256**  | **Sorted**             |        **151.1 ns** |       **7.17 ns** |       **3.75 ns** |     **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Sorted             |        183.0 ns |       0.91 ns |       0.32 ns |     1.21 |    0.03 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | Sorted             |        909.2 ns |      19.56 ns |       8.69 ns |     6.02 |    0.15 |    4 |         - |          NA |
| GnomeSort              | 256  | Sorted             |        233.2 ns |      33.64 ns |      14.94 ns |     1.54 |    0.10 |    3 |         - |          NA |
| LibrarySort            | 256  | Sorted             |     21,894.0 ns |     209.85 ns |      93.17 ns |   144.95 |    3.37 |    6 |         - |          NA |
| MergeInsertionSort     | 256  | Sorted             |     10,096.3 ns |     239.56 ns |     125.30 ns |    66.84 |    1.72 |    5 |         - |          NA |
| ShellSortKnuth1973     | 256  | Sorted             |        635.1 ns |       4.00 ns |       1.77 ns |     4.20 |    0.10 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Sorted             |        653.0 ns |      31.39 ns |      13.94 ns |     4.32 |    0.13 |    4 |         - |          NA |
| ShellSortTokuda1992    | 256  | Sorted             |        785.9 ns |      15.14 ns |       5.40 ns |     5.20 |    0.12 |    4 |         - |          NA |
| ShellSortCiura2001     | 256  | Sorted             |        712.9 ns |      22.08 ns |      11.55 ns |     4.72 |    0.13 |    4 |         - |          NA |
| ShellSortLee2021       | 256  | Sorted             |        817.7 ns |      20.92 ns |       9.29 ns |     5.41 |    0.14 |    4 |         - |          NA |
| DotnetSort             | 256  | Sorted             |        655.1 ns |      21.76 ns |      11.38 ns |     4.34 |    0.12 |    4 |         - |          NA |
|      |                    |                 |               |               |          |         |      |           |             |
| **InsertionSort**          | **256**  | **Reversed**           |     **11,848.9 ns** |     **212.36 ns** |      **94.29 ns** |     **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Reversed           |      9,408.2 ns |     267.98 ns |     140.16 ns |     0.79 |    0.01 |    3 |         - |          NA |
| BinaryInsertSort       | 256  | Reversed           |      4,671.5 ns |      21.69 ns |       7.73 ns |     0.39 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 256  | Reversed           |     21,165.1 ns |     507.81 ns |     225.47 ns |     1.79 |    0.02 |    4 |         - |          NA |
| LibrarySort            | 256  | Reversed           |     17,632.1 ns |     234.52 ns |     104.13 ns |     1.49 |    0.01 |    4 |         - |          NA |
| MergeInsertionSort     | 256  | Reversed           |      9,886.4 ns |     218.42 ns |     114.24 ns |     0.83 |    0.01 |    3 |         - |          NA |
| ShellSortKnuth1973     | 256  | Reversed           |      1,165.0 ns |      14.39 ns |       6.39 ns |     0.10 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Reversed           |      1,093.8 ns |      25.24 ns |      13.20 ns |     0.09 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | Reversed           |      1,127.2 ns |      37.17 ns |      16.50 ns |     0.10 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | Reversed           |      1,164.9 ns |     140.66 ns |      62.45 ns |     0.10 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Reversed           |      1,151.6 ns |      44.73 ns |      19.86 ns |     0.10 |    0.00 |    1 |         - |          NA |
| DotnetSort             | 256  | Reversed           |      1,103.7 ns |     173.51 ns |      90.75 ns |     0.09 |    0.01 |    1 |         - |          NA |
|      |                    |                 |               |               |          |         |      |           |             |
| **InsertionSort**          | **256**  | **PipeOrgan**          |      **6,197.7 ns** |     **238.92 ns** |     **124.96 ns** |     **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PairInsertionSort      | 256  | PipeOrgan          |      4,944.6 ns |     272.49 ns |     142.52 ns |     0.80 |    0.03 |    4 |         - |          NA |
| BinaryInsertSort       | 256  | PipeOrgan          |      3,010.7 ns |     197.21 ns |      87.56 ns |     0.49 |    0.02 |    3 |         - |          NA |
| GnomeSort              | 256  | PipeOrgan          |      9,891.0 ns |     293.05 ns |     153.27 ns |     1.60 |    0.04 |    6 |         - |          NA |
| LibrarySort            | 256  | PipeOrgan          |     15,057.1 ns |     150.00 ns |      66.60 ns |     2.43 |    0.05 |    7 |         - |          NA |
| MergeInsertionSort     | 256  | PipeOrgan          |     10,068.3 ns |     336.03 ns |     175.75 ns |     1.63 |    0.04 |    6 |         - |          NA |
| ShellSortKnuth1973     | 256  | PipeOrgan          |      1,017.9 ns |      18.03 ns |       8.01 ns |     0.16 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | PipeOrgan          |      1,058.8 ns |      77.31 ns |      34.33 ns |     0.17 |    0.01 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | PipeOrgan          |      1,099.8 ns |      48.18 ns |      21.39 ns |     0.18 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | PipeOrgan          |      1,058.1 ns |      36.24 ns |      16.09 ns |     0.17 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | PipeOrgan          |      1,147.9 ns |      64.19 ns |      28.50 ns |     0.19 |    0.01 |    1 |         - |          NA |
| DotnetSort             | 256  | PipeOrgan          |      1,892.8 ns |     296.32 ns |     154.98 ns |     0.31 |    0.02 |    2 |         - |          NA |
|      |                    |                 |               |               |          |         |      |           |             |
| **InsertionSort**          | **256**  | **ManyDuplicates**     |      **7,347.8 ns** |     **192.16 ns** |      **85.32 ns** |     **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 256  | ManyDuplicates     |      7,368.6 ns |     342.32 ns |     151.99 ns |     1.00 |    0.02 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | ManyDuplicates     |     11,056.9 ns |     256.04 ns |     113.69 ns |     1.50 |    0.02 |    3 |         - |          NA |
| GnomeSort              | 256  | ManyDuplicates     |     10,610.9 ns |     219.77 ns |     114.94 ns |     1.44 |    0.02 |    3 |         - |          NA |
| LibrarySort            | 256  | ManyDuplicates     |     23,884.7 ns |     374.38 ns |     195.81 ns |     3.25 |    0.04 |    4 |         - |          NA |
| MergeInsertionSort     | 256  | ManyDuplicates     |     20,428.3 ns |     385.31 ns |     201.52 ns |     2.78 |    0.04 |    4 |         - |          NA |
| ShellSortKnuth1973     | 256  | ManyDuplicates     |      7,367.5 ns |     360.28 ns |     188.43 ns |     1.00 |    0.03 |    2 |         - |          NA |
| ShellSortSedgewick1986 | 256  | ManyDuplicates     |      7,608.3 ns |     211.34 ns |      93.84 ns |     1.04 |    0.02 |    2 |         - |          NA |
| ShellSortTokuda1992    | 256  | ManyDuplicates     |      7,471.8 ns |     307.25 ns |     160.70 ns |     1.02 |    0.02 |    2 |         - |          NA |
| ShellSortCiura2001     | 256  | ManyDuplicates     |      7,282.5 ns |     226.41 ns |     100.53 ns |     0.99 |    0.02 |    2 |         - |          NA |
| ShellSortLee2021       | 256  | ManyDuplicates     |      7,311.2 ns |     159.34 ns |      83.34 ns |     1.00 |    0.02 |    2 |         - |          NA |
| DotnetSort             | 256  | ManyDuplicates     |      5,505.5 ns |     209.09 ns |     109.36 ns |     0.75 |    0.02 |    1 |         - |          NA |
|      |                    |                 |               |               |          |         |      |           |             |
| **InsertionSort**          | **1024** | **Random**             |     **94,681.5 ns** |   **2,709.41 ns** |   **1,203.00 ns** |     **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Random             |     89,935.2 ns |     490.79 ns |     217.91 ns |     0.95 |    0.01 |    4 |         - |          NA |
| BinaryInsertSort       | 1024 | Random             |     62,871.6 ns |      74.89 ns |      26.70 ns |     0.66 |    0.01 |    3 |         - |          NA |
| GnomeSort              | 1024 | Random             |    150,778.2 ns |     555.90 ns |     246.82 ns |     1.59 |    0.02 |    5 |         - |          NA |
| LibrarySort            | 1024 | Random             |    106,820.9 ns |     307.10 ns |     136.35 ns |     1.13 |    0.01 |    4 |         - |          NA |
| MergeInsertionSort     | 1024 | Random             |    106,691.0 ns |   2,035.75 ns |     903.89 ns |     1.13 |    0.02 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Random             |     43,897.7 ns |     251.00 ns |     131.28 ns |     0.46 |    0.01 |    2 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Random             |     46,165.5 ns |     183.48 ns |      95.97 ns |     0.49 |    0.01 |    2 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Random             |     47,905.8 ns |     379.35 ns |     198.41 ns |     0.51 |    0.01 |    2 |         - |          NA |
| ShellSortCiura2001     | 1024 | Random             |     47,154.5 ns |     281.14 ns |     147.04 ns |     0.50 |    0.01 |    2 |         - |          NA |
| ShellSortLee2021       | 1024 | Random             |     47,753.9 ns |     226.14 ns |     118.28 ns |     0.50 |    0.01 |    2 |         - |          NA |
| DotnetSort             | 1024 | Random             |     29,986.3 ns |     478.78 ns |     250.41 ns |     0.32 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |          |         |      |           |             |
| **InsertionSort**          | **1024** | **SingleElementMoved** |        **753.9 ns** |      **82.38 ns** |      **36.58 ns** |     **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | SingleElementMoved |        893.7 ns |     154.73 ns |      68.70 ns |     1.19 |    0.10 |    1 |         - |          NA |
| BinaryInsertSort       | 1024 | SingleElementMoved |      4,677.1 ns |     345.34 ns |     180.62 ns |     6.22 |    0.35 |    3 |         - |          NA |
| GnomeSort              | 1024 | SingleElementMoved |      1,132.4 ns |       9.32 ns |       4.88 ns |     1.51 |    0.07 |    2 |         - |          NA |
| LibrarySort            | 1024 | SingleElementMoved |    192,652.4 ns |     846.01 ns |     375.63 ns |   256.05 |   11.18 |    5 |         - |          NA |
| MergeInsertionSort     | 1024 | SingleElementMoved |     48,956.1 ns |     481.75 ns |     251.97 ns |    65.07 |    2.85 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | SingleElementMoved |      3,747.5 ns |     291.42 ns |     152.42 ns |     4.98 |    0.29 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | SingleElementMoved |      3,844.2 ns |      14.18 ns |       5.06 ns |     5.11 |    0.22 |    3 |         - |          NA |
| ShellSortTokuda1992    | 1024 | SingleElementMoved |      4,332.3 ns |     229.35 ns |     119.95 ns |     5.76 |    0.29 |    3 |         - |          NA |
| ShellSortCiura2001     | 1024 | SingleElementMoved |      4,135.9 ns |      29.58 ns |      10.55 ns |     5.50 |    0.24 |    3 |         - |          NA |
| ShellSortLee2021       | 1024 | SingleElementMoved |      4,209.3 ns |     161.26 ns |      71.60 ns |     5.59 |    0.26 |    3 |         - |          NA |
| DotnetSort             | 1024 | SingleElementMoved |      5,179.2 ns |     206.10 ns |      91.51 ns |     6.88 |    0.32 |    3 |         - |          NA |
|      |                    |                 |               |               |          |         |      |           |             |
| **InsertionSort**          | **1024** | **Sorted**             |        **589.6 ns** |      **15.67 ns** |       **6.96 ns** |     **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Sorted             |        764.2 ns |      13.99 ns |       4.99 ns |     1.30 |    0.02 |    2 |         - |          NA |
| BinaryInsertSort       | 1024 | Sorted             |      4,508.7 ns |     951.56 ns |     497.68 ns |     7.65 |    0.80 |    3 |         - |          NA |
| GnomeSort              | 1024 | Sorted             |        864.2 ns |      57.47 ns |      25.52 ns |     1.47 |    0.04 |    2 |         - |          NA |
| LibrarySort            | 1024 | Sorted             |    192,441.3 ns |     375.02 ns |     133.74 ns |   326.41 |    3.59 |    5 |         - |          NA |
| MergeInsertionSort     | 1024 | Sorted             |     49,039.4 ns |     355.23 ns |     185.79 ns |    83.18 |    0.96 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Sorted             |      3,107.6 ns |      19.19 ns |       8.52 ns |     5.27 |    0.06 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Sorted             |      3,606.1 ns |     292.97 ns |     153.23 ns |     6.12 |    0.25 |    3 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Sorted             |      4,054.4 ns |     215.73 ns |     112.83 ns |     6.88 |    0.20 |    3 |         - |          NA |
| ShellSortCiura2001     | 1024 | Sorted             |      3,761.0 ns |     218.85 ns |     114.46 ns |     6.38 |    0.20 |    3 |         - |          NA |
| ShellSortLee2021       | 1024 | Sorted             |      3,764.7 ns |      38.44 ns |      17.07 ns |     6.39 |    0.08 |    3 |         - |          NA |
| DotnetSort             | 1024 | Sorted             |      3,228.7 ns |     194.00 ns |      86.14 ns |     5.48 |    0.15 |    3 |         - |          NA |
|      |                    |                 |               |               |          |         |      |           |             |
| **InsertionSort**          | **1024** | **Reversed**           |    **178,680.1 ns** |   **1,364.96 ns** |     **606.05 ns** |     **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Reversed           |    145,785.1 ns |     370.48 ns |     132.12 ns |     0.82 |    0.00 |    4 |         - |          NA |
| BinaryInsertSort       | 1024 | Reversed           |     34,470.3 ns |     279.79 ns |     146.33 ns |     0.19 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | Reversed           |    301,298.3 ns |   4,601.80 ns |   2,406.83 ns |     1.69 |    0.01 |    5 |         - |          NA |
| LibrarySort            | 1024 | Reversed           |    151,583.1 ns |   1,210.52 ns |     537.48 ns |     0.85 |    0.00 |    4 |         - |          NA |
| MergeInsertionSort     | 1024 | Reversed           |     48,518.0 ns |     640.14 ns |     334.81 ns |     0.27 |    0.00 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Reversed           |      5,490.0 ns |      35.63 ns |      15.82 ns |     0.03 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Reversed           |      5,758.6 ns |     455.04 ns |     238.00 ns |     0.03 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Reversed           |      6,083.3 ns |     352.13 ns |     184.17 ns |     0.03 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Reversed           |      5,793.1 ns |     262.47 ns |     137.28 ns |     0.03 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Reversed           |      6,007.9 ns |     264.86 ns |     138.53 ns |     0.03 |    0.00 |    1 |         - |          NA |
| DotnetSort             | 1024 | Reversed           |      6,055.4 ns |   1,316.36 ns |     688.48 ns |     0.03 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |          |         |      |           |             |
| **InsertionSort**          | **1024** | **PipeOrgan**          |     **90,009.8 ns** |     **458.25 ns** |     **203.47 ns** |     **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | PipeOrgan          |     73,972.7 ns |     307.88 ns |     161.03 ns |     0.82 |    0.00 |    5 |         - |          NA |
| BinaryInsertSort       | 1024 | PipeOrgan          |     19,889.6 ns |     329.67 ns |     146.38 ns |     0.22 |    0.00 |    3 |         - |          NA |
| GnomeSort              | 1024 | PipeOrgan          |    147,487.3 ns |     977.41 ns |     348.55 ns |     1.64 |    0.00 |    7 |         - |          NA |
| LibrarySort            | 1024 | PipeOrgan          |     92,425.2 ns |   2,804.31 ns |   1,466.71 ns |     1.03 |    0.02 |    6 |         - |          NA |
| MergeInsertionSort     | 1024 | PipeOrgan          |     50,334.5 ns |     420.03 ns |     219.68 ns |     0.56 |    0.00 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | PipeOrgan          |      5,118.9 ns |     362.45 ns |     189.57 ns |     0.06 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | PipeOrgan          |      5,546.3 ns |     344.40 ns |     180.13 ns |     0.06 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | PipeOrgan          |      5,599.5 ns |     116.93 ns |      61.15 ns |     0.06 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | PipeOrgan          |      5,698.9 ns |     251.54 ns |     131.56 ns |     0.06 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | PipeOrgan          |      5,729.7 ns |     233.54 ns |     103.70 ns |     0.06 |    0.00 |    1 |         - |          NA |
| DotnetSort             | 1024 | PipeOrgan          |     12,797.1 ns |     485.44 ns |     215.54 ns |     0.14 |    0.00 |    2 |         - |          NA |
|      |                    |                 |               |               |          |         |      |           |             |
| **InsertionSort**          | **1024** | **ManyDuplicates**     |     **94,478.0 ns** |   **6,246.32 ns** |   **3,266.95 ns** |     **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | ManyDuplicates     |     89,724.1 ns |   4,066.46 ns |   2,126.84 ns |     0.95 |    0.04 |    4 |         - |          NA |
| BinaryInsertSort       | 1024 | ManyDuplicates     |     60,056.9 ns |     699.84 ns |     310.74 ns |     0.64 |    0.02 |    3 |         - |          NA |
| GnomeSort              | 1024 | ManyDuplicates     |    147,545.8 ns |     507.21 ns |     225.20 ns |     1.56 |    0.05 |    5 |         - |          NA |
| LibrarySort            | 1024 | ManyDuplicates     |    112,495.6 ns |     449.37 ns |     235.03 ns |     1.19 |    0.04 |    4 |         - |          NA |
| MergeInsertionSort     | 1024 | ManyDuplicates     |    104,614.0 ns |   1,053.93 ns |     467.95 ns |     1.11 |    0.04 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | ManyDuplicates     |     33,783.3 ns |     298.44 ns |     156.09 ns |     0.36 |    0.01 |    2 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | ManyDuplicates     |     35,113.2 ns |     461.02 ns |     241.12 ns |     0.37 |    0.01 |    2 |         - |          NA |
| ShellSortTokuda1992    | 1024 | ManyDuplicates     |     34,990.0 ns |     130.29 ns |      57.85 ns |     0.37 |    0.01 |    2 |         - |          NA |
| ShellSortCiura2001     | 1024 | ManyDuplicates     |     34,114.6 ns |     747.62 ns |     391.02 ns |     0.36 |    0.01 |    2 |         - |          NA |
| ShellSortLee2021       | 1024 | ManyDuplicates     |     33,449.7 ns |     213.80 ns |      94.93 ns |     0.35 |    0.01 |    2 |         - |          NA |
| DotnetSort             | 1024 | ManyDuplicates     |     23,304.5 ns |     423.83 ns |     221.67 ns |     0.25 |    0.01 |    1 |         - |          NA |
|      |                    |                 |               |               |          |         |      |           |             |
| **InsertionSort**          | **4096** | **Random**             |  **1,415,119.1 ns** |   **5,095.81 ns** |   **2,665.21 ns** |     **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PairInsertionSort      | 4096 | Random             |  1,347,902.9 ns |  22,042.43 ns |   9,786.98 ns |     0.95 |    0.01 |    6 |         - |          NA |
| BinaryInsertSort       | 4096 | Random             |    399,610.7 ns |   1,860.78 ns |     973.23 ns |     0.28 |    0.00 |    3 |         - |          NA |
| GnomeSort              | 4096 | Random             |  2,328,072.3 ns |  13,121.58 ns |   6,862.84 ns |     1.65 |    0.01 |    7 |         - |          NA |
| LibrarySort            | 4096 | Random             |    479,360.5 ns |   1,344.97 ns |     597.17 ns |     0.34 |    0.00 |    4 |         - |          NA |
| MergeInsertionSort     | 4096 | Random             |    748,883.2 ns |   1,462.49 ns |     649.35 ns |     0.53 |    0.00 |    5 |         - |          NA |
| ShellSortKnuth1973     | 4096 | Random             |    217,120.6 ns |     652.18 ns |     289.57 ns |     0.15 |    0.00 |    2 |         - |          NA |
| ShellSortSedgewick1986 | 4096 | Random             |    231,441.8 ns |     612.18 ns |     271.81 ns |     0.16 |    0.00 |    2 |         - |          NA |
| ShellSortTokuda1992    | 4096 | Random             |    241,512.6 ns |   1,271.35 ns |     564.49 ns |     0.17 |    0.00 |    2 |         - |          NA |
| ShellSortCiura2001     | 4096 | Random             |    238,128.4 ns |     375.55 ns |     166.74 ns |     0.17 |    0.00 |    2 |         - |          NA |
| ShellSortLee2021       | 4096 | Random             |    243,628.8 ns |   5,693.97 ns |   2,528.16 ns |     0.17 |    0.00 |    2 |         - |          NA |
| DotnetSort             | 4096 | Random             |    144,908.7 ns |     958.04 ns |     501.08 ns |     0.10 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |          |         |      |           |             |
| **InsertionSort**          | **4096** | **SingleElementMoved** |      **2,757.2 ns** |      **28.75 ns** |      **10.25 ns** |     **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 4096 | SingleElementMoved |      3,679.5 ns |     480.10 ns |     213.17 ns |     1.33 |    0.07 |    2 |         - |          NA |
| BinaryInsertSort       | 4096 | SingleElementMoved |     21,812.7 ns |     463.30 ns |     205.71 ns |     7.91 |    0.07 |    3 |         - |          NA |
| GnomeSort              | 4096 | SingleElementMoved |      4,479.5 ns |     337.26 ns |     176.39 ns |     1.62 |    0.06 |    2 |         - |          NA |
| LibrarySort            | 4096 | SingleElementMoved |  2,358,659.0 ns |   8,028.49 ns |   4,199.05 ns |   855.45 |    3.26 |    5 |         - |          NA |
| MergeInsertionSort     | 4096 | SingleElementMoved |    373,552.5 ns |   1,928.91 ns |     856.45 ns |   135.48 |    0.55 |    4 |         - |          NA |
| ShellSortKnuth1973     | 4096 | SingleElementMoved |     16,148.5 ns |     229.99 ns |     102.12 ns |     5.86 |    0.04 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 4096 | SingleElementMoved |     18,268.4 ns |     191.15 ns |      84.87 ns |     6.63 |    0.04 |    3 |         - |          NA |
| ShellSortTokuda1992    | 4096 | SingleElementMoved |     20,688.8 ns |     360.10 ns |     159.89 ns |     7.50 |    0.06 |    3 |         - |          NA |
| ShellSortCiura2001     | 4096 | SingleElementMoved |     20,118.7 ns |     438.17 ns |     229.17 ns |     7.30 |    0.08 |    3 |         - |          NA |
| ShellSortLee2021       | 4096 | SingleElementMoved |     20,551.5 ns |     193.70 ns |     101.31 ns |     7.45 |    0.04 |    3 |         - |          NA |
| DotnetSort             | 4096 | SingleElementMoved |     24,390.2 ns |     252.09 ns |      89.90 ns |     8.85 |    0.04 |    3 |         - |          NA |
|      |                    |                 |               |               |          |         |      |           |             |
| **InsertionSort**          | **4096** | **Sorted**             |      **2,314.7 ns** |      **76.23 ns** |      **33.85 ns** |     **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 4096 | Sorted             |      2,872.5 ns |     149.18 ns |      78.02 ns |     1.24 |    0.04 |    2 |         - |          NA |
| BinaryInsertSort       | 4096 | Sorted             |     21,570.0 ns |     938.04 ns |     416.50 ns |     9.32 |    0.21 |    3 |         - |          NA |
| GnomeSort              | 4096 | Sorted             |      3,347.6 ns |      23.77 ns |       8.47 ns |     1.45 |    0.02 |    2 |         - |          NA |
| LibrarySort            | 4096 | Sorted             |  2,356,057.5 ns |   6,304.56 ns |   2,799.26 ns | 1,018.07 |   13.77 |    5 |         - |          NA |
| MergeInsertionSort     | 4096 | Sorted             |    370,371.8 ns |   1,972.50 ns |     875.80 ns |   160.04 |    2.19 |    4 |         - |          NA |
| ShellSortKnuth1973     | 4096 | Sorted             |     14,986.8 ns |     188.01 ns |      98.34 ns |     6.48 |    0.10 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 4096 | Sorted             |     17,145.3 ns |     220.41 ns |      97.86 ns |     7.41 |    0.11 |    3 |         - |          NA |
| ShellSortTokuda1992    | 4096 | Sorted             |     19,235.6 ns |     149.87 ns |      78.38 ns |     8.31 |    0.12 |    3 |         - |          NA |
| ShellSortCiura2001     | 4096 | Sorted             |     18,783.0 ns |     262.95 ns |     137.53 ns |     8.12 |    0.12 |    3 |         - |          NA |
| ShellSortLee2021       | 4096 | Sorted             |     19,192.1 ns |     248.54 ns |     129.99 ns |     8.29 |    0.12 |    3 |         - |          NA |
| DotnetSort             | 4096 | Sorted             |     15,017.4 ns |     373.75 ns |     165.95 ns |     6.49 |    0.11 |    3 |         - |          NA |
|      |                    |                 |               |               |          |         |      |           |             |
| **InsertionSort**          | **4096** | **Reversed**           |  **2,785,094.5 ns** |   **5,836.24 ns** |   **2,081.26 ns** |    **1.000** |    **0.00** |    **5** |         **-** |          **NA** |
| PairInsertionSort      | 4096 | Reversed           |  2,302,368.0 ns |   5,047.25 ns |   1,799.90 ns |    0.827 |    0.00 |    4 |         - |          NA |
| BinaryInsertSort       | 4096 | Reversed           |    365,612.9 ns |   1,130.78 ns |     502.07 ns |    0.131 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 4096 | Reversed           |  4,674,975.2 ns |  94,303.18 ns |  49,322.38 ns |    1.679 |    0.02 |    6 |         - |          NA |
| LibrarySort            | 4096 | Reversed           |  1,796,728.6 ns |   2,258.81 ns |   1,002.92 ns |    0.645 |    0.00 |    3 |         - |          NA |
| MergeInsertionSort     | 4096 | Reversed           |    377,346.5 ns |   1,989.94 ns |   1,040.78 ns |    0.135 |    0.00 |    2 |         - |          NA |
| ShellSortKnuth1973     | 4096 | Reversed           |     25,726.7 ns |     357.30 ns |     186.88 ns |    0.009 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 4096 | Reversed           |     28,042.8 ns |     388.67 ns |     203.28 ns |    0.010 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 4096 | Reversed           |     29,117.0 ns |     293.49 ns |     153.50 ns |    0.010 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 4096 | Reversed           |     28,155.5 ns |     394.86 ns |     206.52 ns |    0.010 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 4096 | Reversed           |     29,654.6 ns |     207.29 ns |     108.42 ns |    0.011 |    0.00 |    1 |         - |          NA |
| DotnetSort             | 4096 | Reversed           |     26,640.2 ns |   1,357.99 ns |     710.26 ns |    0.010 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |          |         |      |           |             |
| **InsertionSort**          | **4096** | **PipeOrgan**          |  **1,396,934.5 ns** |   **1,623.03 ns** |     **848.87 ns** |     **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PairInsertionSort      | 4096 | PipeOrgan          |  1,155,123.4 ns |   4,373.77 ns |   1,941.98 ns |     0.83 |    0.00 |    6 |         - |          NA |
| BinaryInsertSort       | 4096 | PipeOrgan          |    201,240.9 ns |   2,406.14 ns |   1,068.34 ns |     0.14 |    0.00 |    3 |         - |          NA |
| GnomeSort              | 4096 | PipeOrgan          |  2,307,135.9 ns |   3,707.91 ns |   1,646.34 ns |     1.65 |    0.00 |    8 |         - |          NA |
| LibrarySort            | 4096 | PipeOrgan          |    797,898.6 ns |  10,012.52 ns |   5,236.74 ns |     0.57 |    0.00 |    5 |         - |          NA |
| MergeInsertionSort     | 4096 | PipeOrgan          |    377,463.2 ns |   2,035.08 ns |     903.59 ns |     0.27 |    0.00 |    4 |         - |          NA |
| ShellSortKnuth1973     | 4096 | PipeOrgan          |     24,847.4 ns |     363.58 ns |     190.16 ns |     0.02 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 4096 | PipeOrgan          |     26,569.9 ns |     450.36 ns |     235.55 ns |     0.02 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 4096 | PipeOrgan          |     28,670.6 ns |     338.37 ns |     176.97 ns |     0.02 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 4096 | PipeOrgan          |     28,303.6 ns |     472.56 ns |     247.16 ns |     0.02 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 4096 | PipeOrgan          |     28,932.6 ns |     435.58 ns |     227.82 ns |     0.02 |    0.00 |    1 |         - |          NA |
| DotnetSort             | 4096 | PipeOrgan          |     71,894.6 ns |   3,103.03 ns |   1,622.95 ns |     0.05 |    0.00 |    2 |         - |          NA |
|      |                    |                 |               |               |          |         |      |           |             |
| **InsertionSort**          | **4096** | **ManyDuplicates**     |  **1,386,594.8 ns** |  **20,640.43 ns** |   **9,164.48 ns** |     **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PairInsertionSort      | 4096 | ManyDuplicates     |  1,292,661.1 ns |  18,548.93 ns |   8,235.84 ns |     0.93 |    0.01 |    6 |         - |          NA |
| BinaryInsertSort       | 4096 | ManyDuplicates     |    364,394.5 ns |   2,536.30 ns |   1,326.53 ns |     0.26 |    0.00 |    3 |         - |          NA |
| GnomeSort              | 4096 | ManyDuplicates     |  2,264,788.9 ns |   5,157.47 ns |   2,697.46 ns |     1.63 |    0.01 |    7 |         - |          NA |
| LibrarySort            | 4096 | ManyDuplicates     |    505,212.3 ns |   1,940.61 ns |   1,014.98 ns |     0.36 |    0.00 |    4 |         - |          NA |
| MergeInsertionSort     | 4096 | ManyDuplicates     |    732,975.3 ns |   1,147.56 ns |     509.52 ns |     0.53 |    0.00 |    5 |         - |          NA |
| ShellSortKnuth1973     | 4096 | ManyDuplicates     |    152,619.5 ns |     735.77 ns |     384.82 ns |     0.11 |    0.00 |    2 |         - |          NA |
| ShellSortSedgewick1986 | 4096 | ManyDuplicates     |    144,547.0 ns |     441.47 ns |     196.02 ns |     0.10 |    0.00 |    2 |         - |          NA |
| ShellSortTokuda1992    | 4096 | ManyDuplicates     |    146,465.0 ns |     746.73 ns |     390.55 ns |     0.11 |    0.00 |    2 |         - |          NA |
| ShellSortCiura2001     | 4096 | ManyDuplicates     |    144,639.3 ns |   2,469.06 ns |   1,096.28 ns |     0.10 |    0.00 |    2 |         - |          NA |
| ShellSortLee2021       | 4096 | ManyDuplicates     |    142,684.0 ns |     518.17 ns |     271.01 ns |     0.10 |    0.00 |    2 |         - |          NA |
| DotnetSort             | 4096 | ManyDuplicates     |     94,406.7 ns |     442.34 ns |     196.40 ns |     0.07 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |          |         |      |           |             |
| **InsertionSort**          | **8192** | **Random**             |  **5,592,390.3 ns** |  **20,657.29 ns** |  **10,804.16 ns** |     **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PairInsertionSort      | 8192 | Random             |  5,268,744.8 ns |  98,314.14 ns |  51,420.19 ns |     0.94 |    0.01 |    5 |         - |          NA |
| BinaryInsertSort       | 8192 | Random             |  1,140,399.2 ns |   1,551.68 ns |     811.56 ns |     0.20 |    0.00 |    3 |         - |          NA |
| GnomeSort              | 8192 | Random             |  9,222,730.5 ns |  21,743.70 ns |   9,654.34 ns |     1.65 |    0.00 |    6 |         - |          NA |
| LibrarySort            | 8192 | Random             |  1,038,061.8 ns |   3,854.79 ns |   2,016.13 ns |     0.19 |    0.00 |    3 |         - |          NA |
| MergeInsertionSort     | 8192 | Random             |  2,360,205.8 ns |   5,560.63 ns |   2,468.95 ns |     0.42 |    0.00 |    4 |         - |          NA |
| ShellSortKnuth1973     | 8192 | Random             |    481,896.1 ns |     643.14 ns |     336.37 ns |     0.09 |    0.00 |    2 |         - |          NA |
| ShellSortSedgewick1986 | 8192 | Random             |    515,604.8 ns |   2,019.58 ns |     896.70 ns |     0.09 |    0.00 |    2 |         - |          NA |
| ShellSortTokuda1992    | 8192 | Random             |    531,107.4 ns |   1,224.86 ns |     640.62 ns |     0.09 |    0.00 |    2 |         - |          NA |
| ShellSortCiura2001     | 8192 | Random             |    523,844.2 ns |   1,406.34 ns |     624.43 ns |     0.09 |    0.00 |    2 |         - |          NA |
| ShellSortLee2021       | 8192 | Random             |    532,304.0 ns |   8,721.38 ns |   4,561.45 ns |     0.10 |    0.00 |    2 |         - |          NA |
| DotnetSort             | 8192 | Random             |    314,198.9 ns |   2,508.41 ns |   1,311.95 ns |     0.06 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |          |         |      |           |             |
| **InsertionSort**          | **8192** | **SingleElementMoved** |      **5,549.2 ns** |     **181.95 ns** |      **80.79 ns** |     **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 8192 | SingleElementMoved |      6,509.0 ns |     282.66 ns |     147.84 ns |     1.17 |    0.03 |    1 |         - |          NA |
| BinaryInsertSort       | 8192 | SingleElementMoved |     50,042.6 ns |   1,832.77 ns |     958.57 ns |     9.02 |    0.20 |    4 |         - |          NA |
| GnomeSort              | 8192 | SingleElementMoved |      8,991.1 ns |     367.36 ns |     192.14 ns |     1.62 |    0.04 |    2 |         - |          NA |
| LibrarySort            | 8192 | SingleElementMoved |  8,951,027.2 ns |   7,320.12 ns |   3,250.18 ns | 1,613.32 |   21.72 |    6 |         - |          NA |
| MergeInsertionSort     | 8192 | SingleElementMoved |  1,283,883.8 ns |   3,920.14 ns |   1,740.57 ns |   231.40 |    3.13 |    5 |         - |          NA |
| ShellSortKnuth1973     | 8192 | SingleElementMoved |     35,662.4 ns |     453.22 ns |     237.04 ns |     6.43 |    0.10 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 8192 | SingleElementMoved |     43,016.8 ns |     210.14 ns |     109.91 ns |     7.75 |    0.11 |    4 |         - |          NA |
| ShellSortTokuda1992    | 8192 | SingleElementMoved |     45,012.9 ns |     191.96 ns |      85.23 ns |     8.11 |    0.11 |    4 |         - |          NA |
| ShellSortCiura2001     | 8192 | SingleElementMoved |     43,775.3 ns |     301.96 ns |     134.07 ns |     7.89 |    0.11 |    4 |         - |          NA |
| ShellSortLee2021       | 8192 | SingleElementMoved |     44,841.3 ns |     223.01 ns |     116.64 ns |     8.08 |    0.11 |    4 |         - |          NA |
| DotnetSort             | 8192 | SingleElementMoved |     53,485.8 ns |   1,224.21 ns |     543.56 ns |     9.64 |    0.16 |    4 |         - |          NA |
|      |                    |                 |               |               |          |         |      |           |             |
| **InsertionSort**          | **8192** | **Sorted**             |      **4,524.2 ns** |     **180.89 ns** |      **80.32 ns** |     **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 8192 | Sorted             |      5,724.5 ns |     235.68 ns |     123.26 ns |     1.27 |    0.03 |    2 |         - |          NA |
| BinaryInsertSort       | 8192 | Sorted             |     46,061.9 ns |     285.13 ns |     101.68 ns |    10.18 |    0.17 |    4 |         - |          NA |
| GnomeSort              | 8192 | Sorted             |      6,804.3 ns |     289.97 ns |     151.66 ns |     1.50 |    0.04 |    2 |         - |          NA |
| LibrarySort            | 8192 | Sorted             |  8,885,122.2 ns |  28,539.22 ns |  14,926.56 ns | 1,964.44 |   32.27 |    6 |         - |          NA |
| MergeInsertionSort     | 8192 | Sorted             |  1,283,633.2 ns |   2,431.05 ns |   1,079.40 ns |   283.80 |    4.65 |    5 |         - |          NA |
| ShellSortKnuth1973     | 8192 | Sorted             |     33,579.8 ns |     380.42 ns |     168.91 ns |     7.42 |    0.13 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 8192 | Sorted             |     41,045.6 ns |     147.44 ns |      52.58 ns |     9.07 |    0.15 |    4 |         - |          NA |
| ShellSortTokuda1992    | 8192 | Sorted             |     42,780.7 ns |     598.53 ns |     265.75 ns |     9.46 |    0.16 |    4 |         - |          NA |
| ShellSortCiura2001     | 8192 | Sorted             |     41,452.5 ns |     482.69 ns |     252.45 ns |     9.16 |    0.16 |    4 |         - |          NA |
| ShellSortLee2021       | 8192 | Sorted             |     42,650.4 ns |     435.20 ns |     227.62 ns |     9.43 |    0.16 |    4 |         - |          NA |
| DotnetSort             | 8192 | Sorted             |     34,069.4 ns |   3,242.57 ns |   1,695.92 ns |     7.53 |    0.37 |    3 |         - |          NA |
|      |                    |                 |               |               |          |         |      |           |             |
| **InsertionSort**          | **8192** | **Reversed**           | **11,182,644.6 ns** | **285,179.44 ns** | **126,621.44 ns** |    **1.000** |    **0.00** |    **5** |         **-** |          **NA** |
| PairInsertionSort      | 8192 | Reversed           |  9,226,288.4 ns |  90,400.71 ns |  47,281.32 ns |    0.825 |    0.01 |    4 |         - |          NA |
| BinaryInsertSort       | 8192 | Reversed           |  1,368,462.7 ns |   5,401.28 ns |   2,824.98 ns |    0.122 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 8192 | Reversed           | 18,496,367.0 ns |  78,791.21 ns |  34,983.79 ns |    1.654 |    0.02 |    6 |         - |          NA |
| LibrarySort            | 8192 | Reversed           |  6,723,175.9 ns |  13,601.73 ns |   6,039.25 ns |    0.601 |    0.01 |    3 |         - |          NA |
| MergeInsertionSort     | 8192 | Reversed           |  1,284,753.9 ns |   3,922.79 ns |   1,741.74 ns |    0.115 |    0.00 |    2 |         - |          NA |
| ShellSortKnuth1973     | 8192 | Reversed           |     52,983.9 ns |   1,103.45 ns |     577.13 ns |    0.005 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 8192 | Reversed           |     63,541.8 ns |     575.23 ns |     300.86 ns |    0.006 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 8192 | Reversed           |     65,878.6 ns |     442.63 ns |     196.53 ns |    0.006 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 8192 | Reversed           |     60,865.0 ns |     485.08 ns |     215.38 ns |    0.005 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 8192 | Reversed           |     67,472.0 ns |   1,041.36 ns |     462.37 ns |    0.006 |    0.00 |    1 |         - |          NA |
| DotnetSort             | 8192 | Reversed           |     59,998.2 ns |   4,079.40 ns |   2,133.60 ns |    0.005 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |          |         |      |           |             |
| **InsertionSort**          | **8192** | **PipeOrgan**          |  **5,566,619.5 ns** |  **14,091.02 ns** |   **7,369.87 ns** |    **1.000** |    **0.00** |    **7** |         **-** |          **NA** |
| PairInsertionSort      | 8192 | PipeOrgan          |  4,600,348.3 ns |   6,684.68 ns |   3,496.22 ns |    0.826 |    0.00 |    6 |         - |          NA |
| BinaryInsertSort       | 8192 | PipeOrgan          |    769,040.3 ns |   3,290.98 ns |   1,461.22 ns |    0.138 |    0.00 |    3 |         - |          NA |
| GnomeSort              | 8192 | PipeOrgan          |  9,208,443.6 ns |   9,567.90 ns |   4,248.21 ns |    1.654 |    0.00 |    8 |         - |          NA |
| LibrarySort            | 8192 | PipeOrgan          |  2,942,208.0 ns |  16,089.58 ns |   8,415.16 ns |    0.529 |    0.00 |    5 |         - |          NA |
| MergeInsertionSort     | 8192 | PipeOrgan          |  1,286,610.2 ns |   2,515.07 ns |   1,116.71 ns |    0.231 |    0.00 |    4 |         - |          NA |
| ShellSortKnuth1973     | 8192 | PipeOrgan          |     53,673.9 ns |     294.21 ns |     130.63 ns |    0.010 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 8192 | PipeOrgan          |     60,573.0 ns |     383.96 ns |     200.82 ns |    0.011 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 8192 | PipeOrgan          |     61,177.2 ns |     774.80 ns |     344.02 ns |    0.011 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 8192 | PipeOrgan          |     62,775.1 ns |     533.14 ns |     278.84 ns |    0.011 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 8192 | PipeOrgan          |     62,569.0 ns |     860.17 ns |     449.89 ns |    0.011 |    0.00 |    1 |         - |          NA |
| DotnetSort             | 8192 | PipeOrgan          |    280,341.3 ns |   6,765.79 ns |   3,538.64 ns |    0.050 |    0.00 |    2 |         - |          NA |
|      |                    |                 |               |               |          |         |      |           |             |
| **InsertionSort**          | **8192** | **ManyDuplicates**     |  **5,447,574.0 ns** |  **12,719.30 ns** |   **4,535.83 ns** |     **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PairInsertionSort      | 8192 | ManyDuplicates     |  5,089,487.0 ns |  55,969.15 ns |  24,850.65 ns |     0.93 |    0.00 |    5 |         - |          NA |
| BinaryInsertSort       | 8192 | ManyDuplicates     |  1,021,229.3 ns |   1,466.60 ns |     523.00 ns |     0.19 |    0.00 |    3 |         - |          NA |
| GnomeSort              | 8192 | ManyDuplicates     |  8,997,794.5 ns |  13,889.18 ns |   6,166.88 ns |     1.65 |    0.00 |    6 |         - |          NA |
| LibrarySort            | 8192 | ManyDuplicates     |  1,081,401.7 ns |   3,524.52 ns |   1,564.91 ns |     0.20 |    0.00 |    3 |         - |          NA |
| MergeInsertionSort     | 8192 | ManyDuplicates     |  2,306,034.6 ns |   4,137.39 ns |   1,837.02 ns |     0.42 |    0.00 |    4 |         - |          NA |
| ShellSortKnuth1973     | 8192 | ManyDuplicates     |    318,907.9 ns |   5,758.90 ns |   3,012.01 ns |     0.06 |    0.00 |    2 |         - |          NA |
| ShellSortSedgewick1986 | 8192 | ManyDuplicates     |    280,340.6 ns |   1,205.75 ns |     630.63 ns |     0.05 |    0.00 |    2 |         - |          NA |
| ShellSortTokuda1992    | 8192 | ManyDuplicates     |    296,667.1 ns |     822.15 ns |     365.04 ns |     0.05 |    0.00 |    2 |         - |          NA |
| ShellSortCiura2001     | 8192 | ManyDuplicates     |    297,607.8 ns |   1,128.52 ns |     590.24 ns |     0.05 |    0.00 |    2 |         - |          NA |
| ShellSortLee2021       | 8192 | ManyDuplicates     |    290,671.4 ns |     976.00 ns |     510.47 ns |     0.05 |    0.00 |    2 |         - |          NA |
| DotnetSort             | 8192 | ManyDuplicates     |    190,106.9 ns |     187.94 ns |      83.45 ns |     0.03 |    0.00 |    1 |         - |          NA |

### MergeBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.49 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                   | Size | Pattern            | Mean           | Error        | StdDev       | Median         | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------------- |----- |------------------- |---------------:|-------------:|-------------:|---------------:|------:|--------:|-----:|----------:|------------:|
| **MergeSort**                | **256**  | **Random**             |    **12,692.8 ns** |    **129.97 ns** |     **67.98 ns** |    **12,695.1 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Random             |    11,974.8 ns |    297.41 ns |    132.05 ns |    11,973.3 ns |  0.94 |    0.01 |    4 |         - |          NA |
| BottomupMergeSort        | 256  | Random             |     9,041.1 ns |    344.69 ns |    153.04 ns |     9,013.5 ns |  0.71 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 256  | Random             |     5,329.7 ns |    235.79 ns |    104.69 ns |     5,381.2 ns |  0.42 |    0.01 |    3 |         - |          NA |
| RotateMergeSort          | 256  | Random             |    18,755.7 ns |    499.86 ns |    261.43 ns |    18,775.0 ns |  1.48 |    0.02 |    5 |         - |          NA |
| RotateMergeSortRecursive | 256  | Random             |    22,648.7 ns |    414.43 ns |    184.01 ns |    22,727.6 ns |  1.78 |    0.02 |    6 |         - |          NA |
| SymMergeSort             | 256  | Random             |    12,438.6 ns |    104.46 ns |     37.25 ns |    12,441.5 ns |  0.98 |    0.01 |    4 |         - |          NA |
| BlockMergeSort           | 256  | Random             |     9,660.1 ns |    228.83 ns |    101.60 ns |     9,702.6 ns |  0.76 |    0.01 |    4 |         - |          NA |
| NaturalMergeSort         | 256  | Random             |     9,194.0 ns |    195.73 ns |     86.90 ns |     9,215.8 ns |  0.72 |    0.01 |    4 |         - |          NA |
| PatienceSort             | 256  | Random             |    13,063.4 ns |    323.34 ns |    143.56 ns |    13,077.7 ns |  1.03 |    0.01 |    4 |         - |          NA |
| DropMergeSort            | 256  | Random             |     9,347.2 ns |    164.15 ns |     72.88 ns |     9,356.3 ns |  0.74 |    0.01 |    4 |         - |          NA |
| TimSort                  | 256  | Random             |    10,203.2 ns |    267.91 ns |    140.12 ns |    10,202.0 ns |  0.80 |    0.01 |    4 |         - |          NA |
| PowerSort                | 256  | Random             |     6,512.9 ns |    307.64 ns |    160.90 ns |     6,432.7 ns |  0.51 |    0.01 |    3 |         - |          NA |
| ShiftSort                | 256  | Random             |    10,079.2 ns |    294.63 ns |    130.82 ns |    10,162.7 ns |  0.79 |    0.01 |    4 |         - |          NA |
| SpinSort                 | 256  | Random             |     5,959.1 ns |    206.88 ns |    108.20 ns |     5,988.4 ns |  0.47 |    0.01 |    3 |         - |          NA |
| Glidesort                | 256  | Random             |     5,706.2 ns |    109.24 ns |     48.50 ns |     5,688.9 ns |  0.45 |    0.00 |    3 |         - |          NA |
| Driftsort                | 256  | Random             |     4,841.8 ns |    368.03 ns |    192.49 ns |     4,757.5 ns |  0.38 |    0.01 |    3 |         - |          NA |
| FlatStableSort           | 256  | Random             |     6,561.6 ns |    144.06 ns |     75.34 ns |     6,535.6 ns |  0.52 |    0.01 |    3 |         - |          NA |
| QuadSort                 | 256  | Random             |     3,567.4 ns |    252.15 ns |    131.88 ns |     3,523.2 ns |  0.28 |    0.01 |    2 |         - |          NA |
| SimdMergeSort            | 256  | Random             |     1,935.2 ns |     37.71 ns |     16.74 ns |     1,932.8 ns |  0.15 |    0.00 |    1 |         - |          NA |
| DotnetSort               | 256  | Random             |     5,831.5 ns |    254.16 ns |    132.93 ns |     5,781.4 ns |  0.46 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **256**  | **SingleElementMoved** |     **3,506.3 ns** |     **33.24 ns** |     **11.85 ns** |     **3,502.2 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | SingleElementMoved |     4,227.6 ns |    191.82 ns |     85.17 ns |     4,197.8 ns |  1.21 |    0.02 |    6 |         - |          NA |
| BottomupMergeSort        | 256  | SingleElementMoved |     1,450.7 ns |    220.90 ns |    115.54 ns |     1,386.7 ns |  0.41 |    0.03 |    4 |         - |          NA |
| StdStableSort            | 256  | SingleElementMoved |       549.7 ns |     16.37 ns |      7.27 ns |       548.5 ns |  0.16 |    0.00 |    2 |         - |          NA |
| RotateMergeSort          | 256  | SingleElementMoved |       528.1 ns |     44.03 ns |     19.55 ns |       525.0 ns |  0.15 |    0.01 |    2 |         - |          NA |
| RotateMergeSortRecursive | 256  | SingleElementMoved |       738.0 ns |     66.30 ns |     29.44 ns |       725.9 ns |  0.21 |    0.01 |    2 |         - |          NA |
| SymMergeSort             | 256  | SingleElementMoved |       400.5 ns |     21.55 ns |      9.57 ns |       400.0 ns |  0.11 |    0.00 |    1 |         - |          NA |
| BlockMergeSort           | 256  | SingleElementMoved |     2,246.2 ns |     88.82 ns |     39.44 ns |     2,226.2 ns |  0.64 |    0.01 |    5 |         - |          NA |
| NaturalMergeSort         | 256  | SingleElementMoved |       344.3 ns |      7.50 ns |      2.67 ns |       344.3 ns |  0.10 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 256  | SingleElementMoved |     7,275.3 ns |    634.33 ns |    331.77 ns |     7,208.9 ns |  2.07 |    0.09 |    7 |         - |          NA |
| DropMergeSort            | 256  | SingleElementMoved |       521.3 ns |     21.60 ns |     11.30 ns |       517.5 ns |  0.15 |    0.00 |    2 |         - |          NA |
| TimSort                  | 256  | SingleElementMoved |       294.2 ns |     28.10 ns |     12.48 ns |       293.7 ns |  0.08 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | SingleElementMoved |       341.6 ns |     26.69 ns |      9.52 ns |       339.2 ns |  0.10 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | SingleElementMoved |       387.2 ns |     13.56 ns |      7.09 ns |       384.7 ns |  0.11 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 256  | SingleElementMoved |       671.5 ns |     19.53 ns |      8.67 ns |       669.9 ns |  0.19 |    0.00 |    2 |         - |          NA |
| Glidesort                | 256  | SingleElementMoved |       655.7 ns |     18.52 ns |      8.22 ns |       655.0 ns |  0.19 |    0.00 |    2 |         - |          NA |
| Driftsort                | 256  | SingleElementMoved |       572.4 ns |     39.64 ns |     20.73 ns |       562.5 ns |  0.16 |    0.01 |    2 |         - |          NA |
| FlatStableSort           | 256  | SingleElementMoved |     1,086.1 ns |     46.24 ns |     20.53 ns |     1,079.9 ns |  0.31 |    0.01 |    3 |         - |          NA |
| QuadSort                 | 256  | SingleElementMoved |       526.3 ns |     29.31 ns |     13.01 ns |       522.2 ns |  0.15 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 256  | SingleElementMoved |     2,062.2 ns |    259.47 ns |    135.71 ns |     1,972.8 ns |  0.59 |    0.04 |    5 |         - |          NA |
| DotnetSort               | 256  | SingleElementMoved |     1,101.8 ns |    144.70 ns |     64.25 ns |     1,072.7 ns |  0.31 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **256**  | **Sorted**             |     **3,506.5 ns** |    **459.53 ns** |    **240.34 ns** |     **3,451.7 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Sorted             |     4,037.4 ns |     24.87 ns |     11.04 ns |     4,033.3 ns |  1.16 |    0.07 |    7 |         - |          NA |
| BottomupMergeSort        | 256  | Sorted             |     1,171.1 ns |      7.08 ns |      3.14 ns |     1,171.0 ns |  0.34 |    0.02 |    5 |         - |          NA |
| StdStableSort            | 256  | Sorted             |       468.1 ns |      6.61 ns |      2.36 ns |       466.9 ns |  0.13 |    0.01 |    3 |         - |          NA |
| RotateMergeSort          | 256  | Sorted             |       236.4 ns |      2.57 ns |      0.92 ns |       236.5 ns |  0.07 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 256  | Sorted             |       410.6 ns |      2.30 ns |      1.02 ns |       410.9 ns |  0.12 |    0.01 |    3 |         - |          NA |
| SymMergeSort             | 256  | Sorted             |       229.6 ns |      0.76 ns |      0.27 ns |       229.6 ns |  0.07 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 256  | Sorted             |     1,968.3 ns |      7.29 ns |      3.24 ns |     1,967.9 ns |  0.56 |    0.04 |    6 |         - |          NA |
| NaturalMergeSort         | 256  | Sorted             |       196.8 ns |      8.44 ns |      4.41 ns |       197.0 ns |  0.06 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 256  | Sorted             |     6,133.6 ns |    246.41 ns |    109.41 ns |     6,105.6 ns |  1.76 |    0.11 |    8 |         - |          NA |
| DropMergeSort            | 256  | Sorted             |       481.2 ns |     11.31 ns |      5.02 ns |       479.3 ns |  0.14 |    0.01 |    3 |         - |          NA |
| TimSort                  | 256  | Sorted             |       142.5 ns |      3.25 ns |      1.44 ns |       142.9 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | Sorted             |       135.5 ns |      5.15 ns |      2.29 ns |       134.6 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Sorted             |       203.2 ns |      2.80 ns |      1.24 ns |       203.6 ns |  0.06 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 256  | Sorted             |       141.2 ns |      4.72 ns |      2.09 ns |       141.1 ns |  0.04 |    0.00 |    1 |         - |          NA |
| Glidesort                | 256  | Sorted             |       178.5 ns |     13.28 ns |      6.95 ns |       181.7 ns |  0.05 |    0.00 |    2 |         - |          NA |
| Driftsort                | 256  | Sorted             |       231.7 ns |     96.67 ns |     50.56 ns |       247.6 ns |  0.07 |    0.01 |    2 |         - |          NA |
| FlatStableSort           | 256  | Sorted             |       993.7 ns |     10.83 ns |      4.81 ns |       992.2 ns |  0.28 |    0.02 |    5 |         - |          NA |
| QuadSort                 | 256  | Sorted             |       265.4 ns |     10.67 ns |      3.81 ns |       267.0 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 256  | Sorted             |     1,925.4 ns |     25.42 ns |     13.30 ns |     1,921.8 ns |  0.55 |    0.04 |    6 |         - |          NA |
| DotnetSort               | 256  | Sorted             |       632.0 ns |     11.21 ns |      5.86 ns |       630.7 ns |  0.18 |    0.01 |    4 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **256**  | **Reversed**           |     **7,075.7 ns** |    **137.37 ns** |     **61.00 ns** |     **7,096.2 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Reversed           |     6,159.7 ns |     13.40 ns |      4.78 ns |     6,159.5 ns |  0.87 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 256  | Reversed           |     3,726.5 ns |     13.21 ns |      4.71 ns |     3,728.2 ns |  0.53 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 256  | Reversed           |     3,181.6 ns |     34.12 ns |     15.15 ns |     3,186.3 ns |  0.45 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 256  | Reversed           |     4,334.2 ns |    128.23 ns |     56.93 ns |     4,308.8 ns |  0.61 |    0.01 |    5 |         - |          NA |
| RotateMergeSortRecursive | 256  | Reversed           |     6,213.0 ns |    287.86 ns |    150.55 ns |     6,293.9 ns |  0.88 |    0.02 |    6 |         - |          NA |
| SymMergeSort             | 256  | Reversed           |     1,765.8 ns |     29.88 ns |     10.65 ns |     1,765.9 ns |  0.25 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 256  | Reversed           |     2,446.5 ns |     13.81 ns |      4.92 ns |     2,446.0 ns |  0.35 |    0.00 |    4 |         - |          NA |
| NaturalMergeSort         | 256  | Reversed           |       227.2 ns |      7.10 ns |      3.15 ns |       226.4 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 256  | Reversed           |     1,257.0 ns |      6.63 ns |      2.94 ns |     1,256.2 ns |  0.18 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 256  | Reversed           |     1,211.1 ns |     24.26 ns |     10.77 ns |     1,205.7 ns |  0.17 |    0.00 |    2 |         - |          NA |
| TimSort                  | 256  | Reversed           |       197.4 ns |      3.55 ns |      1.86 ns |       196.9 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | Reversed           |       209.5 ns |     14.98 ns |      7.84 ns |       209.9 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Reversed           |     3,221.7 ns |     41.43 ns |     18.40 ns |     3,219.4 ns |  0.46 |    0.00 |    5 |         - |          NA |
| SpinSort                 | 256  | Reversed           |       228.8 ns |     12.04 ns |      6.30 ns |       225.5 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 256  | Reversed           |       223.2 ns |      3.72 ns |      1.65 ns |       223.5 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 256  | Reversed           |       231.2 ns |      4.77 ns |      2.12 ns |       230.8 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | Reversed           |     1,734.2 ns |      9.96 ns |      3.55 ns |     1,733.0 ns |  0.25 |    0.00 |    3 |         - |          NA |
| QuadSort                 | 256  | Reversed           |       239.2 ns |     15.71 ns |      8.22 ns |       242.2 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SimdMergeSort            | 256  | Reversed           |     2,589.2 ns |    213.23 ns |    111.53 ns |     2,530.9 ns |  0.37 |    0.02 |    4 |         - |          NA |
| DotnetSort               | 256  | Reversed           |     1,113.1 ns |    132.32 ns |     69.20 ns |     1,151.5 ns |  0.16 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **256**  | **PipeOrgan**          |     **9,650.3 ns** |  **6,912.21 ns** |  **3,615.22 ns** |     **7,831.9 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | PipeOrgan          |     5,211.6 ns |     59.76 ns |     26.53 ns |     5,201.1 ns |  0.60 |    0.18 |    5 |         - |          NA |
| BottomupMergeSort        | 256  | PipeOrgan          |     2,501.8 ns |     20.17 ns |      8.95 ns |     2,496.8 ns |  0.29 |    0.09 |    4 |         - |          NA |
| StdStableSort            | 256  | PipeOrgan          |     1,934.8 ns |     11.44 ns |      5.08 ns |     1,934.4 ns |  0.22 |    0.07 |    4 |         - |          NA |
| RotateMergeSort          | 256  | PipeOrgan          |     4,337.2 ns |    344.80 ns |    180.33 ns |     4,225.7 ns |  0.50 |    0.15 |    5 |         - |          NA |
| RotateMergeSortRecursive | 256  | PipeOrgan          |     5,635.2 ns |    336.41 ns |    175.95 ns |     5,513.3 ns |  0.65 |    0.20 |    5 |         - |          NA |
| SymMergeSort             | 256  | PipeOrgan          |     2,710.6 ns |    275.21 ns |    143.94 ns |     2,621.3 ns |  0.31 |    0.09 |    4 |         - |          NA |
| BlockMergeSort           | 256  | PipeOrgan          |     2,406.8 ns |     13.58 ns |      6.03 ns |     2,405.0 ns |  0.28 |    0.08 |    4 |         - |          NA |
| NaturalMergeSort         | 256  | PipeOrgan          |       384.5 ns |      6.77 ns |      3.54 ns |       384.0 ns |  0.04 |    0.01 |    1 |         - |          NA |
| PatienceSort             | 256  | PipeOrgan          |     4,599.0 ns |    255.11 ns |    133.43 ns |     4,547.9 ns |  0.53 |    0.16 |    5 |         - |          NA |
| DropMergeSort            | 256  | PipeOrgan          |     5,089.4 ns |    322.73 ns |    168.80 ns |     5,064.5 ns |  0.59 |    0.18 |    5 |         - |          NA |
| TimSort                  | 256  | PipeOrgan          |       617.2 ns |     34.53 ns |     18.06 ns |       614.9 ns |  0.07 |    0.02 |    2 |         - |          NA |
| PowerSort                | 256  | PipeOrgan          |       385.3 ns |      5.76 ns |      2.56 ns |       384.8 ns |  0.04 |    0.01 |    1 |         - |          NA |
| ShiftSort                | 256  | PipeOrgan          |     2,090.2 ns |     16.01 ns |      5.71 ns |     2,089.4 ns |  0.24 |    0.07 |    4 |         - |          NA |
| SpinSort                 | 256  | PipeOrgan          |     1,611.7 ns |     31.54 ns |     16.50 ns |     1,610.1 ns |  0.19 |    0.06 |    4 |         - |          NA |
| Glidesort                | 256  | PipeOrgan          |       973.6 ns |      8.74 ns |      3.88 ns |       971.3 ns |  0.11 |    0.03 |    3 |         - |          NA |
| Driftsort                | 256  | PipeOrgan          |       352.4 ns |      3.74 ns |      1.33 ns |       352.5 ns |  0.04 |    0.01 |    1 |         - |          NA |
| FlatStableSort           | 256  | PipeOrgan          |     1,446.4 ns |      7.59 ns |      2.71 ns |     1,446.7 ns |  0.17 |    0.05 |    4 |         - |          NA |
| QuadSort                 | 256  | PipeOrgan          |       671.3 ns |     13.12 ns |      5.82 ns |       670.4 ns |  0.08 |    0.02 |    2 |         - |          NA |
| SimdMergeSort            | 256  | PipeOrgan          |     1,921.8 ns |     36.41 ns |     12.98 ns |     1,916.9 ns |  0.22 |    0.07 |    4 |         - |          NA |
| DotnetSort               | 256  | PipeOrgan          |     1,922.8 ns |    380.49 ns |    199.00 ns |     1,836.1 ns |  0.22 |    0.07 |    4 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **256**  | **ManyDuplicates**     |    **12,238.2 ns** |    **294.15 ns** |    **153.85 ns** |    **12,216.4 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | ManyDuplicates     |    11,623.4 ns |    286.10 ns |    127.03 ns |    11,688.2 ns |  0.95 |    0.01 |    4 |         - |          NA |
| BottomupMergeSort        | 256  | ManyDuplicates     |     8,794.4 ns |    125.42 ns |     65.60 ns |     8,803.4 ns |  0.72 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 256  | ManyDuplicates     |     5,029.0 ns |    466.18 ns |    243.82 ns |     4,954.5 ns |  0.41 |    0.02 |    3 |         - |          NA |
| RotateMergeSort          | 256  | ManyDuplicates     |    18,179.9 ns |    445.61 ns |    197.85 ns |    18,250.5 ns |  1.49 |    0.02 |    5 |         - |          NA |
| RotateMergeSortRecursive | 256  | ManyDuplicates     |    22,093.6 ns |    655.46 ns |    342.82 ns |    22,137.2 ns |  1.81 |    0.03 |    6 |         - |          NA |
| SymMergeSort             | 256  | ManyDuplicates     |    12,018.2 ns |    389.70 ns |    203.82 ns |    11,945.3 ns |  0.98 |    0.02 |    4 |         - |          NA |
| BlockMergeSort           | 256  | ManyDuplicates     |     9,408.6 ns |    235.44 ns |    123.14 ns |     9,390.3 ns |  0.77 |    0.01 |    4 |         - |          NA |
| NaturalMergeSort         | 256  | ManyDuplicates     |     8,847.9 ns |    371.87 ns |    165.11 ns |     8,769.7 ns |  0.72 |    0.02 |    4 |         - |          NA |
| PatienceSort             | 256  | ManyDuplicates     |    11,061.0 ns |    466.03 ns |    243.74 ns |    11,017.7 ns |  0.90 |    0.02 |    4 |         - |          NA |
| DropMergeSort            | 256  | ManyDuplicates     |     8,132.0 ns |    329.62 ns |    172.40 ns |     8,129.6 ns |  0.66 |    0.02 |    4 |         - |          NA |
| TimSort                  | 256  | ManyDuplicates     |     9,620.9 ns |    204.09 ns |    106.74 ns |     9,645.4 ns |  0.79 |    0.01 |    4 |         - |          NA |
| PowerSort                | 256  | ManyDuplicates     |     6,256.3 ns |    343.22 ns |    179.51 ns |     6,256.8 ns |  0.51 |    0.02 |    3 |         - |          NA |
| ShiftSort                | 256  | ManyDuplicates     |     9,732.8 ns |    227.71 ns |    101.10 ns |     9,767.8 ns |  0.80 |    0.01 |    4 |         - |          NA |
| SpinSort                 | 256  | ManyDuplicates     |     5,459.5 ns |    339.91 ns |    177.78 ns |     5,400.4 ns |  0.45 |    0.01 |    3 |         - |          NA |
| Glidesort                | 256  | ManyDuplicates     |     5,459.2 ns |    266.61 ns |     95.08 ns |     5,494.9 ns |  0.45 |    0.01 |    3 |         - |          NA |
| Driftsort                | 256  | ManyDuplicates     |     4,788.0 ns |    333.16 ns |    174.25 ns |     4,810.9 ns |  0.39 |    0.01 |    3 |         - |          NA |
| FlatStableSort           | 256  | ManyDuplicates     |     6,329.7 ns |    151.05 ns |     79.00 ns |     6,312.2 ns |  0.52 |    0.01 |    3 |         - |          NA |
| QuadSort                 | 256  | ManyDuplicates     |     3,571.4 ns |    316.77 ns |    165.68 ns |     3,587.2 ns |  0.29 |    0.01 |    2 |         - |          NA |
| SimdMergeSort            | 256  | ManyDuplicates     |     2,514.2 ns |     55.19 ns |     19.68 ns |     2,522.6 ns |  0.21 |    0.00 |    1 |         - |          NA |
| DotnetSort               | 256  | ManyDuplicates     |     5,594.8 ns |    454.49 ns |    237.71 ns |     5,543.9 ns |  0.46 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **1024** | **Random**             |    **58,632.4 ns** |    **310.98 ns** |    **162.65 ns** |    **58,657.6 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Random             |    56,128.7 ns |    275.42 ns |    144.05 ns |    56,160.6 ns |  0.96 |    0.00 |    4 |         - |          NA |
| BottomupMergeSort        | 1024 | Random             |    42,748.2 ns |    145.77 ns |     64.72 ns |    42,769.4 ns |  0.73 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 1024 | Random             |    29,652.9 ns |  1,281.88 ns |    670.45 ns |    29,597.7 ns |  0.51 |    0.01 |    3 |         - |          NA |
| RotateMergeSort          | 1024 | Random             |   108,378.2 ns |    870.51 ns |    386.51 ns |   108,324.2 ns |  1.85 |    0.01 |    5 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Random             |   127,304.9 ns |    816.86 ns |    362.69 ns |   127,461.5 ns |  2.17 |    0.01 |    5 |         - |          NA |
| SymMergeSort             | 1024 | Random             |    72,499.8 ns |    584.57 ns |    259.55 ns |    72,454.9 ns |  1.24 |    0.01 |    4 |         - |          NA |
| BlockMergeSort           | 1024 | Random             |    49,495.3 ns |    368.49 ns |    163.61 ns |    49,533.2 ns |  0.84 |    0.00 |    4 |         - |          NA |
| NaturalMergeSort         | 1024 | Random             |    44,697.7 ns |    497.52 ns |    260.21 ns |    44,612.6 ns |  0.76 |    0.00 |    4 |         - |          NA |
| PatienceSort             | 1024 | Random             |    64,393.1 ns |    393.20 ns |    174.58 ns |    64,367.2 ns |  1.10 |    0.00 |    4 |         - |          NA |
| DropMergeSort            | 1024 | Random             |    43,760.9 ns |    407.75 ns |    213.26 ns |    43,701.5 ns |  0.75 |    0.00 |    4 |         - |          NA |
| TimSort                  | 1024 | Random             |    50,100.2 ns |    386.80 ns |    202.31 ns |    50,010.1 ns |  0.85 |    0.00 |    4 |         - |          NA |
| PowerSort                | 1024 | Random             |    35,115.7 ns |    563.46 ns |    250.18 ns |    35,119.8 ns |  0.60 |    0.00 |    3 |         - |          NA |
| ShiftSort                | 1024 | Random             |    50,305.3 ns |    125.94 ns |     65.87 ns |    50,311.5 ns |  0.86 |    0.00 |    4 |         - |          NA |
| SpinSort                 | 1024 | Random             |    31,511.7 ns |    239.08 ns |    125.04 ns |    31,454.1 ns |  0.54 |    0.00 |    3 |         - |          NA |
| Glidesort                | 1024 | Random             |    24,459.2 ns |    537.70 ns |    238.74 ns |    24,396.0 ns |  0.42 |    0.00 |    2 |         - |          NA |
| Driftsort                | 1024 | Random             |    21,001.0 ns |    304.91 ns |    159.48 ns |    21,065.8 ns |  0.36 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 1024 | Random             |    34,280.0 ns |    183.13 ns |     81.31 ns |    34,273.5 ns |  0.58 |    0.00 |    3 |         - |          NA |
| QuadSort                 | 1024 | Random             |    17,946.9 ns |    259.25 ns |    115.11 ns |    17,916.9 ns |  0.31 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 1024 | Random             |    12,959.6 ns |    319.89 ns |    167.31 ns |    12,966.0 ns |  0.22 |    0.00 |    1 |         - |          NA |
| DotnetSort               | 1024 | Random             |    29,995.7 ns |    113.42 ns |     50.36 ns |    30,004.6 ns |  0.51 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **1024** | **SingleElementMoved** |    **13,913.9 ns** |    **130.91 ns** |     **68.47 ns** |    **13,896.8 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | SingleElementMoved |    16,852.7 ns |    197.75 ns |    103.43 ns |    16,856.5 ns |  1.21 |    0.01 |    8 |         - |          NA |
| BottomupMergeSort        | 1024 | SingleElementMoved |     5,161.2 ns |    273.73 ns |    143.16 ns |     5,144.4 ns |  0.37 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 1024 | SingleElementMoved |     2,685.3 ns |     31.42 ns |     13.95 ns |     2,685.5 ns |  0.19 |    0.00 |    4 |         - |          NA |
| RotateMergeSort          | 1024 | SingleElementMoved |     1,500.0 ns |    113.37 ns |     50.33 ns |     1,492.0 ns |  0.11 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 1024 | SingleElementMoved |     2,170.3 ns |     22.12 ns |      7.89 ns |     2,170.0 ns |  0.16 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 1024 | SingleElementMoved |     1,239.8 ns |     51.26 ns |     22.76 ns |     1,238.5 ns |  0.09 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 1024 | SingleElementMoved |     9,905.1 ns |    247.54 ns |    129.47 ns |     9,922.1 ns |  0.71 |    0.01 |    6 |         - |          NA |
| NaturalMergeSort         | 1024 | SingleElementMoved |     1,053.3 ns |     11.45 ns |      5.08 ns |     1,051.4 ns |  0.08 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 1024 | SingleElementMoved |    38,167.3 ns |    416.77 ns |    185.05 ns |    38,161.9 ns |  2.74 |    0.02 |    9 |         - |          NA |
| DropMergeSort            | 1024 | SingleElementMoved |     1,954.1 ns |    139.90 ns |     62.11 ns |     1,931.4 ns |  0.14 |    0.00 |    3 |         - |          NA |
| TimSort                  | 1024 | SingleElementMoved |       754.0 ns |     20.64 ns |     10.80 ns |       749.6 ns |  0.05 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | SingleElementMoved |     1,060.4 ns |      6.94 ns |      3.08 ns |     1,061.4 ns |  0.08 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 1024 | SingleElementMoved |     1,226.7 ns |     16.32 ns |      5.82 ns |     1,226.6 ns |  0.09 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 1024 | SingleElementMoved |     2,920.4 ns |    186.50 ns |     97.54 ns |     2,875.4 ns |  0.21 |    0.01 |    4 |         - |          NA |
| Glidesort                | 1024 | SingleElementMoved |     1,862.1 ns |     32.17 ns |     14.28 ns |     1,860.7 ns |  0.13 |    0.00 |    3 |         - |          NA |
| Driftsort                | 1024 | SingleElementMoved |     1,090.9 ns |      8.71 ns |      4.56 ns |     1,090.6 ns |  0.08 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 1024 | SingleElementMoved |     4,448.8 ns |     27.70 ns |     12.30 ns |     4,444.0 ns |  0.32 |    0.00 |    5 |         - |          NA |
| QuadSort                 | 1024 | SingleElementMoved |     1,199.0 ns |     11.85 ns |      4.23 ns |     1,198.1 ns |  0.09 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 1024 | SingleElementMoved |    12,396.7 ns |    288.67 ns |    150.98 ns |    12,426.0 ns |  0.89 |    0.01 |    7 |         - |          NA |
| DotnetSort               | 1024 | SingleElementMoved |     5,102.1 ns |    107.06 ns |     47.53 ns |     5,091.6 ns |  0.37 |    0.00 |    5 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **1024** | **Sorted**             |    **13,395.0 ns** |    **258.27 ns** |    **114.67 ns** |    **13,431.4 ns** |  **1.00** |    **0.00** |    **9** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Sorted             |    16,346.8 ns |     32.94 ns |     17.23 ns |    16,348.7 ns |  1.22 |    0.01 |   10 |         - |          NA |
| BottomupMergeSort        | 1024 | Sorted             |     4,503.1 ns |     23.93 ns |     10.63 ns |     4,498.8 ns |  0.34 |    0.00 |    7 |         - |          NA |
| StdStableSort            | 1024 | Sorted             |     2,528.5 ns |    288.16 ns |    150.72 ns |     2,424.8 ns |  0.19 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 1024 | Sorted             |     1,013.0 ns |    220.85 ns |    115.51 ns |       937.3 ns |  0.08 |    0.01 |    3 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Sorted             |     1,561.2 ns |      9.00 ns |      3.21 ns |     1,561.9 ns |  0.12 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 1024 | Sorted             |       912.8 ns |      8.55 ns |      3.05 ns |       912.8 ns |  0.07 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | Sorted             |     8,481.9 ns |    276.03 ns |    144.37 ns |     8,470.7 ns |  0.63 |    0.01 |    8 |         - |          NA |
| NaturalMergeSort         | 1024 | Sorted             |       525.3 ns |     29.93 ns |     13.29 ns |       518.6 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 1024 | Sorted             |    30,840.8 ns |    188.01 ns |     67.05 ns |    30,839.4 ns |  2.30 |    0.02 |   11 |         - |          NA |
| DropMergeSort            | 1024 | Sorted             |     1,747.8 ns |    171.07 ns |     75.96 ns |     1,711.1 ns |  0.13 |    0.01 |    4 |         - |          NA |
| TimSort                  | 1024 | Sorted             |       509.4 ns |      7.58 ns |      3.96 ns |       510.5 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Sorted             |       510.9 ns |     27.34 ns |     12.14 ns |       514.5 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Sorted             |       653.4 ns |     34.27 ns |     15.21 ns |       650.8 ns |  0.05 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 1024 | Sorted             |       451.9 ns |     22.35 ns |     11.69 ns |       452.5 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Sorted             |       441.3 ns |     12.98 ns |      5.76 ns |       444.6 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | Sorted             |       518.9 ns |     10.74 ns |      5.62 ns |       518.3 ns |  0.04 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Sorted             |     4,275.1 ns |    333.90 ns |    148.26 ns |     4,342.8 ns |  0.32 |    0.01 |    7 |         - |          NA |
| QuadSort                 | 1024 | Sorted             |       675.2 ns |     21.66 ns |      9.62 ns |       670.2 ns |  0.05 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 1024 | Sorted             |    12,355.2 ns |    293.11 ns |    153.30 ns |    12,360.7 ns |  0.92 |    0.01 |    9 |         - |          NA |
| DotnetSort               | 1024 | Sorted             |     3,139.7 ns |     41.52 ns |     18.44 ns |     3,135.4 ns |  0.23 |    0.00 |    6 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **1024** | **Reversed**           |    **29,061.6 ns** |    **275.72 ns** |    **122.42 ns** |    **29,039.1 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Reversed           |    25,487.4 ns |    201.97 ns |     72.02 ns |    25,513.0 ns |  0.88 |    0.00 |    6 |         - |          NA |
| BottomupMergeSort        | 1024 | Reversed           |    15,434.8 ns |     81.30 ns |     42.52 ns |    15,442.0 ns |  0.53 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 1024 | Reversed           |    13,720.1 ns |    287.83 ns |    127.80 ns |    13,772.7 ns |  0.47 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 1024 | Reversed           |    20,418.8 ns |    228.91 ns |    101.64 ns |    20,422.2 ns |  0.70 |    0.00 |    5 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Reversed           |    27,968.1 ns |    268.48 ns |    119.21 ns |    28,007.4 ns |  0.96 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 1024 | Reversed           |     7,641.0 ns |    246.12 ns |    128.72 ns |     7,665.5 ns |  0.26 |    0.00 |    4 |         - |          NA |
| BlockMergeSort           | 1024 | Reversed           |    11,674.9 ns |    207.94 ns |    108.76 ns |    11,682.8 ns |  0.40 |    0.00 |    5 |         - |          NA |
| NaturalMergeSort         | 1024 | Reversed           |       799.9 ns |     16.38 ns |      7.27 ns |       802.4 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 1024 | Reversed           |     4,286.0 ns |     19.16 ns |      8.51 ns |     4,281.5 ns |  0.15 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 1024 | Reversed           |     5,399.5 ns |     97.79 ns |     43.42 ns |     5,402.3 ns |  0.19 |    0.00 |    3 |         - |          NA |
| TimSort                  | 1024 | Reversed           |       743.1 ns |      9.08 ns |      4.75 ns |       742.8 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Reversed           |       753.2 ns |     17.60 ns |      7.81 ns |       754.5 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Reversed           |    14,514.0 ns |     36.22 ns |     16.08 ns |    14,523.3 ns |  0.50 |    0.00 |    5 |         - |          NA |
| SpinSort                 | 1024 | Reversed           |       793.3 ns |     14.98 ns |      7.84 ns |       792.2 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Reversed           |       773.6 ns |     28.14 ns |     10.03 ns |       772.0 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | Reversed           |       779.5 ns |     15.60 ns |      6.93 ns |       778.3 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Reversed           |     7,074.0 ns |    313.80 ns |    164.12 ns |     7,061.8 ns |  0.24 |    0.01 |    4 |         - |          NA |
| QuadSort                 | 1024 | Reversed           |       782.3 ns |     13.85 ns |      6.15 ns |       782.2 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SimdMergeSort            | 1024 | Reversed           |    17,464.0 ns |    145.80 ns |     76.25 ns |    17,467.9 ns |  0.60 |    0.00 |    5 |         - |          NA |
| DotnetSort               | 1024 | Reversed           |     5,427.1 ns |    985.61 ns |    437.62 ns |     5,251.3 ns |  0.19 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **1024** | **PipeOrgan**          |    **21,637.1 ns** |    **362.19 ns** |    **189.43 ns** |    **21,591.8 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | PipeOrgan          |    21,452.2 ns |    207.17 ns |    108.35 ns |    21,453.1 ns |  0.99 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 1024 | PipeOrgan          |    10,420.5 ns |    322.03 ns |    142.98 ns |    10,496.3 ns |  0.48 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 1024 | PipeOrgan          |     8,518.8 ns |    341.42 ns |    178.57 ns |     8,567.1 ns |  0.39 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 1024 | PipeOrgan          |    19,905.5 ns |    587.36 ns |    307.20 ns |    19,801.6 ns |  0.92 |    0.02 |    6 |         - |          NA |
| RotateMergeSortRecursive | 1024 | PipeOrgan          |    25,104.0 ns |    128.67 ns |     67.29 ns |    25,087.8 ns |  1.16 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 1024 | PipeOrgan          |    11,662.9 ns |    239.55 ns |    106.36 ns |    11,658.8 ns |  0.54 |    0.01 |    5 |         - |          NA |
| BlockMergeSort           | 1024 | PipeOrgan          |    12,093.5 ns |    156.43 ns |     81.82 ns |    12,129.1 ns |  0.56 |    0.01 |    5 |         - |          NA |
| NaturalMergeSort         | 1024 | PipeOrgan          |     1,383.3 ns |     56.26 ns |     24.98 ns |     1,374.8 ns |  0.06 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 1024 | PipeOrgan          |    21,988.0 ns |    140.19 ns |     49.99 ns |    21,992.5 ns |  1.02 |    0.01 |    6 |         - |          NA |
| DropMergeSort            | 1024 | PipeOrgan          |    21,769.5 ns |    543.28 ns |    284.15 ns |    21,712.0 ns |  1.01 |    0.01 |    6 |         - |          NA |
| TimSort                  | 1024 | PipeOrgan          |     2,168.4 ns |     58.11 ns |     25.80 ns |     2,160.1 ns |  0.10 |    0.00 |    2 |         - |          NA |
| PowerSort                | 1024 | PipeOrgan          |     1,383.1 ns |     31.51 ns |     16.48 ns |     1,380.4 ns |  0.06 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | PipeOrgan          |     9,418.0 ns |    298.82 ns |    156.29 ns |     9,500.7 ns |  0.44 |    0.01 |    5 |         - |          NA |
| SpinSort                 | 1024 | PipeOrgan          |     7,062.5 ns |    290.96 ns |    152.18 ns |     7,003.8 ns |  0.33 |    0.01 |    4 |         - |          NA |
| Glidesort                | 1024 | PipeOrgan          |     3,750.5 ns |     69.65 ns |     30.93 ns |     3,737.5 ns |  0.17 |    0.00 |    3 |         - |          NA |
| Driftsort                | 1024 | PipeOrgan          |     1,251.0 ns |    135.57 ns |     60.20 ns |     1,226.0 ns |  0.06 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | PipeOrgan          |     6,359.4 ns |    301.62 ns |    133.92 ns |     6,287.9 ns |  0.29 |    0.01 |    4 |         - |          NA |
| QuadSort                 | 1024 | PipeOrgan          |     2,479.4 ns |    296.57 ns |    155.11 ns |     2,369.9 ns |  0.11 |    0.01 |    2 |         - |          NA |
| SimdMergeSort            | 1024 | PipeOrgan          |    12,588.5 ns |    258.77 ns |    135.34 ns |    12,590.4 ns |  0.58 |    0.01 |    5 |         - |          NA |
| DotnetSort               | 1024 | PipeOrgan          |    12,983.7 ns |    309.75 ns |    137.53 ns |    13,041.9 ns |  0.60 |    0.01 |    5 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **1024** | **ManyDuplicates**     |    **52,259.0 ns** |    **403.98 ns** |    **211.29 ns** |    **52,193.2 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | ManyDuplicates     |    49,300.0 ns |    343.60 ns |    179.71 ns |    49,269.7 ns |  0.94 |    0.00 |    2 |         - |          NA |
| BottomupMergeSort        | 1024 | ManyDuplicates     |    37,092.2 ns |    199.45 ns |    104.32 ns |    37,111.2 ns |  0.71 |    0.00 |    2 |         - |          NA |
| StdStableSort            | 1024 | ManyDuplicates     |    23,703.8 ns |  1,461.62 ns |    764.45 ns |    23,807.1 ns |  0.45 |    0.01 |    2 |         - |          NA |
| RotateMergeSort          | 1024 | ManyDuplicates     |    95,636.9 ns |  1,390.58 ns |    617.43 ns |    95,598.2 ns |  1.83 |    0.01 |    3 |         - |          NA |
| RotateMergeSortRecursive | 1024 | ManyDuplicates     |   111,930.4 ns |    886.76 ns |    463.79 ns |   111,729.4 ns |  2.14 |    0.01 |    3 |         - |          NA |
| SymMergeSort             | 1024 | ManyDuplicates     |    62,735.7 ns |    803.84 ns |    356.91 ns |    62,699.5 ns |  1.20 |    0.01 |    2 |         - |          NA |
| BlockMergeSort           | 1024 | ManyDuplicates     |    46,514.5 ns |    117.02 ns |     61.21 ns |    46,517.6 ns |  0.89 |    0.00 |    2 |         - |          NA |
| NaturalMergeSort         | 1024 | ManyDuplicates     |    38,614.9 ns |    187.32 ns |     83.17 ns |    38,605.9 ns |  0.74 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 1024 | ManyDuplicates     |    41,095.0 ns |    817.13 ns |    362.81 ns |    40,991.4 ns |  0.79 |    0.01 |    2 |         - |          NA |
| DropMergeSort            | 1024 | ManyDuplicates     |    30,986.1 ns |    246.56 ns |    109.47 ns |    30,937.4 ns |  0.59 |    0.00 |    2 |         - |          NA |
| TimSort                  | 1024 | ManyDuplicates     |    43,758.3 ns |    253.45 ns |    132.56 ns |    43,758.3 ns |  0.84 |    0.00 |    2 |         - |          NA |
| PowerSort                | 1024 | ManyDuplicates     |    28,359.2 ns |    399.62 ns |    209.01 ns |    28,386.7 ns |  0.54 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 1024 | ManyDuplicates     |    43,307.2 ns |    457.34 ns |    239.20 ns |    43,178.8 ns |  0.83 |    0.01 |    2 |         - |          NA |
| SpinSort                 | 1024 | ManyDuplicates     |    25,427.4 ns |    685.93 ns |    358.75 ns |    25,389.7 ns |  0.49 |    0.01 |    2 |         - |          NA |
| Glidesort                | 1024 | ManyDuplicates     |    14,001.7 ns |    276.06 ns |    122.57 ns |    13,986.1 ns |  0.27 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | ManyDuplicates     |    11,744.5 ns |    500.52 ns |    261.78 ns |    11,732.7 ns |  0.22 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | ManyDuplicates     |    28,719.0 ns |    373.87 ns |    195.54 ns |    28,730.1 ns |  0.55 |    0.00 |    2 |         - |          NA |
| QuadSort                 | 1024 | ManyDuplicates     |    17,836.1 ns |    301.59 ns |    157.74 ns |    17,789.7 ns |  0.34 |    0.00 |    1 |         - |          NA |
| SimdMergeSort            | 1024 | ManyDuplicates     |    15,765.2 ns |    120.44 ns |     53.47 ns |    15,734.2 ns |  0.30 |    0.00 |    1 |         - |          NA |
| DotnetSort               | 1024 | ManyDuplicates     |    23,318.7 ns |    378.61 ns |    198.02 ns |    23,331.3 ns |  0.45 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **4096** | **Random**             |   **266,211.4 ns** |  **1,641.18 ns** |    **728.69 ns** |   **265,876.1 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Random             |   253,212.6 ns |    832.20 ns |    435.26 ns |   253,256.6 ns |  0.95 |    0.00 |    3 |         - |          NA |
| BottomupMergeSort        | 4096 | Random             |   224,182.9 ns | 49,256.83 ns | 25,762.27 ns |   219,868.3 ns |  0.84 |    0.09 |    3 |         - |          NA |
| StdStableSort            | 4096 | Random             |   148,034.5 ns |  2,225.99 ns |    988.35 ns |   148,043.8 ns |  0.56 |    0.00 |    3 |         - |          NA |
| RotateMergeSort          | 4096 | Random             |   573,564.3 ns |  2,560.11 ns |  1,338.99 ns |   573,613.7 ns |  2.15 |    0.01 |    5 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Random             |   663,594.5 ns |  2,017.63 ns |  1,055.26 ns |   663,233.0 ns |  2.49 |    0.01 |    5 |         - |          NA |
| SymMergeSort             | 4096 | Random             |   385,719.5 ns |  1,630.70 ns |    724.04 ns |   385,754.2 ns |  1.45 |    0.00 |    4 |         - |          NA |
| BlockMergeSort           | 4096 | Random             |   243,168.5 ns |    715.87 ns |    317.85 ns |   243,154.6 ns |  0.91 |    0.00 |    3 |         - |          NA |
| NaturalMergeSort         | 4096 | Random             |   208,059.0 ns |    793.73 ns |    352.42 ns |   208,039.8 ns |  0.78 |    0.00 |    3 |         - |          NA |
| PatienceSort             | 4096 | Random             |   303,010.8 ns |    944.85 ns |    336.94 ns |   303,040.3 ns |  1.14 |    0.00 |    3 |         - |          NA |
| DropMergeSort            | 4096 | Random             |   199,640.2 ns |  4,442.03 ns |  1,972.29 ns |   198,822.9 ns |  0.75 |    0.01 |    3 |         - |          NA |
| TimSort                  | 4096 | Random             |   234,806.4 ns |    930.69 ns |    413.23 ns |   234,787.1 ns |  0.88 |    0.00 |    3 |         - |          NA |
| PowerSort                | 4096 | Random             |   174,532.5 ns |    712.72 ns |    316.45 ns |   174,523.1 ns |  0.66 |    0.00 |    3 |         - |          NA |
| ShiftSort                | 4096 | Random             |   241,668.8 ns |    643.95 ns |    285.92 ns |   241,729.8 ns |  0.91 |    0.00 |    3 |         - |          NA |
| SpinSort                 | 4096 | Random             |   156,375.4 ns |  1,435.67 ns |    750.89 ns |   156,112.3 ns |  0.59 |    0.00 |    3 |         - |          NA |
| Glidesort                | 4096 | Random             |    99,802.6 ns |    393.72 ns |    174.81 ns |    99,781.8 ns |  0.37 |    0.00 |    2 |         - |          NA |
| Driftsort                | 4096 | Random             |    85,872.4 ns |  1,035.53 ns |    541.60 ns |    85,604.0 ns |  0.32 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 4096 | Random             |   163,212.2 ns |  1,340.86 ns |    595.35 ns |   163,147.6 ns |  0.61 |    0.00 |    3 |         - |          NA |
| QuadSort                 | 4096 | Random             |    82,626.8 ns |    351.16 ns |    155.92 ns |    82,698.1 ns |  0.31 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 4096 | Random             |    68,026.2 ns |    374.57 ns |    166.31 ns |    67,992.2 ns |  0.26 |    0.00 |    1 |         - |          NA |
| DotnetSort               | 4096 | Random             |   144,643.7 ns |  1,123.64 ns |    498.90 ns |   144,892.9 ns |  0.54 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **4096** | **SingleElementMoved** |    **54,948.5 ns** |    **292.07 ns** |    **152.76 ns** |    **54,951.3 ns** |  **1.00** |    **0.00** |    **9** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | SingleElementMoved |    67,100.5 ns |  1,076.31 ns |    477.89 ns |    66,799.8 ns |  1.22 |    0.01 |   10 |         - |          NA |
| BottomupMergeSort        | 4096 | SingleElementMoved |    20,190.4 ns |    184.21 ns |     81.79 ns |    20,171.6 ns |  0.37 |    0.00 |    6 |         - |          NA |
| StdStableSort            | 4096 | SingleElementMoved |    12,906.5 ns |    230.51 ns |    120.56 ns |    12,964.4 ns |  0.23 |    0.00 |    4 |         - |          NA |
| RotateMergeSort          | 4096 | SingleElementMoved |     4,996.9 ns |    197.60 ns |     87.74 ns |     4,996.4 ns |  0.09 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 4096 | SingleElementMoved |     7,457.3 ns |    347.89 ns |    154.46 ns |     7,357.7 ns |  0.14 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 4096 | SingleElementMoved |     4,426.7 ns |    217.95 ns |    113.99 ns |     4,376.6 ns |  0.08 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 4096 | SingleElementMoved |    38,663.8 ns |    234.65 ns |    104.18 ns |    38,660.6 ns |  0.70 |    0.00 |    8 |         - |          NA |
| NaturalMergeSort         | 4096 | SingleElementMoved |     3,798.3 ns |     49.51 ns |     17.66 ns |     3,794.0 ns |  0.07 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 4096 | SingleElementMoved |   267,082.1 ns | 14,114.14 ns |  7,381.97 ns |   267,529.5 ns |  4.86 |    0.13 |   11 |         - |          NA |
| DropMergeSort            | 4096 | SingleElementMoved |     7,442.7 ns |    327.41 ns |    171.24 ns |     7,462.2 ns |  0.14 |    0.00 |    3 |         - |          NA |
| TimSort                  | 4096 | SingleElementMoved |     2,460.9 ns |     38.32 ns |     17.01 ns |     2,452.5 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | SingleElementMoved |     3,857.5 ns |     31.76 ns |     11.32 ns |     3,861.0 ns |  0.07 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 4096 | SingleElementMoved |     4,363.3 ns |     39.13 ns |     13.95 ns |     4,360.4 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 4096 | SingleElementMoved |     8,525.6 ns |    405.77 ns |    212.22 ns |     8,585.2 ns |  0.16 |    0.00 |    3 |         - |          NA |
| Glidesort                | 4096 | SingleElementMoved |     6,877.1 ns |    215.82 ns |    112.88 ns |     6,841.3 ns |  0.13 |    0.00 |    3 |         - |          NA |
| Driftsort                | 4096 | SingleElementMoved |     3,709.5 ns |     16.22 ns |      7.20 ns |     3,709.2 ns |  0.07 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 4096 | SingleElementMoved |    16,573.3 ns |    117.22 ns |     52.05 ns |    16,576.8 ns |  0.30 |    0.00 |    5 |         - |          NA |
| QuadSort                 | 4096 | SingleElementMoved |     3,886.5 ns |    142.72 ns |     63.37 ns |     3,861.0 ns |  0.07 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 4096 | SingleElementMoved |    67,168.0 ns |    218.40 ns |     96.97 ns |    67,184.6 ns |  1.22 |    0.00 |   10 |         - |          NA |
| DotnetSort               | 4096 | SingleElementMoved |    24,532.9 ns |    282.26 ns |    147.63 ns |    24,558.1 ns |  0.45 |    0.00 |    7 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **4096** | **Sorted**             |    **53,118.1 ns** |    **285.81 ns** |    **126.90 ns** |    **53,118.4 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Sorted             |    65,232.0 ns |    640.47 ns |    334.98 ns |    65,063.3 ns |  1.23 |    0.01 |    9 |         - |          NA |
| BottomupMergeSort        | 4096 | Sorted             |    18,570.6 ns |    109.37 ns |     57.20 ns |    18,576.3 ns |  0.35 |    0.00 |    6 |         - |          NA |
| StdStableSort            | 4096 | Sorted             |    12,206.1 ns |    229.05 ns |    119.80 ns |    12,239.3 ns |  0.23 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 4096 | Sorted             |     3,697.0 ns |      5.83 ns |      2.08 ns |     3,696.8 ns |  0.07 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Sorted             |     6,234.0 ns |    215.50 ns |    112.71 ns |     6,225.8 ns |  0.12 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 4096 | Sorted             |     3,605.9 ns |     24.16 ns |      8.61 ns |     3,603.3 ns |  0.07 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 4096 | Sorted             |    33,630.5 ns |    174.79 ns |     91.42 ns |    33,659.3 ns |  0.63 |    0.00 |    7 |         - |          NA |
| NaturalMergeSort         | 4096 | Sorted             |     1,856.5 ns |     13.08 ns |      4.67 ns |     1,856.0 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 4096 | Sorted             |   176,376.0 ns | 22,037.59 ns | 11,526.08 ns |   171,314.6 ns |  3.32 |    0.20 |   10 |         - |          NA |
| DropMergeSort            | 4096 | Sorted             |     6,838.0 ns |    261.94 ns |    137.00 ns |     6,744.1 ns |  0.13 |    0.00 |    4 |         - |          NA |
| TimSort                  | 4096 | Sorted             |     1,823.7 ns |     35.60 ns |     12.70 ns |     1,822.2 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | Sorted             |     1,780.5 ns |     10.22 ns |      3.64 ns |     1,781.2 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | Sorted             |     2,317.3 ns |      9.17 ns |      4.07 ns |     2,316.9 ns |  0.04 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 4096 | Sorted             |     1,569.7 ns |     37.43 ns |     19.58 ns |     1,567.5 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 4096 | Sorted             |     1,660.9 ns |    232.64 ns |    121.68 ns |     1,611.3 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | Sorted             |     1,831.1 ns |     24.31 ns |     10.80 ns |     1,831.0 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Sorted             |    14,495.7 ns |    204.46 ns |    106.94 ns |    14,482.2 ns |  0.27 |    0.00 |    5 |         - |          NA |
| QuadSort                 | 4096 | Sorted             |     2,395.3 ns |     36.24 ns |     16.09 ns |     2,388.1 ns |  0.05 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 4096 | Sorted             |    69,207.5 ns |    261.37 ns |    136.70 ns |    69,220.3 ns |  1.30 |    0.00 |    9 |         - |          NA |
| DotnetSort               | 4096 | Sorted             |    15,030.8 ns |    371.21 ns |    132.38 ns |    15,009.2 ns |  0.28 |    0.00 |    5 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **4096** | **Reversed**           |   **119,535.3 ns** |    **755.59 ns** |    **335.49 ns** |   **119,488.4 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Reversed           |   104,757.2 ns |    713.65 ns |    254.50 ns |   104,864.6 ns |  0.88 |    0.00 |    5 |         - |          NA |
| BottomupMergeSort        | 4096 | Reversed           |    63,714.8 ns |    371.19 ns |    164.81 ns |    63,647.0 ns |  0.53 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 4096 | Reversed           |    57,970.3 ns |    228.59 ns |    101.49 ns |    57,989.0 ns |  0.48 |    0.00 |    4 |         - |          NA |
| RotateMergeSort          | 4096 | Reversed           |    94,118.1 ns |    625.38 ns |    277.67 ns |    94,158.0 ns |  0.79 |    0.00 |    5 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Reversed           |   124,175.5 ns |    770.66 ns |    342.18 ns |   124,013.8 ns |  1.04 |    0.00 |    5 |         - |          NA |
| SymMergeSort             | 4096 | Reversed           |    32,448.4 ns |    238.67 ns |    105.97 ns |    32,447.4 ns |  0.27 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 4096 | Reversed           |    52,274.5 ns |    381.37 ns |    169.33 ns |    52,211.1 ns |  0.44 |    0.00 |    4 |         - |          NA |
| NaturalMergeSort         | 4096 | Reversed           |     2,998.0 ns |    262.12 ns |    137.09 ns |     2,938.3 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 4096 | Reversed           |    16,687.5 ns |    206.82 ns |     91.83 ns |    16,669.4 ns |  0.14 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 4096 | Reversed           |    24,281.0 ns |    367.39 ns |    192.15 ns |    24,256.5 ns |  0.20 |    0.00 |    3 |         - |          NA |
| TimSort                  | 4096 | Reversed           |     2,818.9 ns |     41.66 ns |     18.50 ns |     2,811.3 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | Reversed           |     2,881.4 ns |    209.28 ns |    109.46 ns |     2,807.6 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | Reversed           |    65,283.5 ns |    216.71 ns |     96.22 ns |    65,253.3 ns |  0.55 |    0.00 |    4 |         - |          NA |
| SpinSort                 | 4096 | Reversed           |     2,956.9 ns |     25.98 ns |      9.26 ns |     2,956.5 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Glidesort                | 4096 | Reversed           |     2,876.9 ns |     49.41 ns |     21.94 ns |     2,878.6 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | Reversed           |     3,028.3 ns |    413.91 ns |    216.48 ns |     2,940.4 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Reversed           |    31,622.6 ns |    586.35 ns |    260.34 ns |    31,581.8 ns |  0.26 |    0.00 |    3 |         - |          NA |
| QuadSort                 | 4096 | Reversed           |     3,063.3 ns |    117.28 ns |     61.34 ns |     3,064.7 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SimdMergeSort            | 4096 | Reversed           |    68,395.7 ns |    201.90 ns |    105.60 ns |    68,424.6 ns |  0.57 |    0.00 |    4 |         - |          NA |
| DotnetSort               | 4096 | Reversed           |    26,709.6 ns |  1,447.60 ns |    757.12 ns |    26,601.7 ns |  0.22 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **4096** | **PipeOrgan**          |    **87,641.3 ns** |    **861.23 ns** |    **307.12 ns** |    **87,570.1 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | PipeOrgan          |    86,499.5 ns |  1,123.51 ns |    498.84 ns |    86,347.7 ns |  0.99 |    0.01 |    8 |         - |          NA |
| BottomupMergeSort        | 4096 | PipeOrgan          |    42,590.3 ns |    675.09 ns |    299.74 ns |    42,494.1 ns |  0.49 |    0.00 |    6 |         - |          NA |
| StdStableSort            | 4096 | PipeOrgan          |    36,837.0 ns |  1,356.04 ns |    602.09 ns |    36,490.2 ns |  0.42 |    0.01 |    6 |         - |          NA |
| RotateMergeSort          | 4096 | PipeOrgan          |    88,290.9 ns |    709.00 ns |    314.80 ns |    88,394.3 ns |  1.01 |    0.00 |    8 |         - |          NA |
| RotateMergeSortRecursive | 4096 | PipeOrgan          |   108,346.4 ns |    368.79 ns |    163.74 ns |   108,378.5 ns |  1.24 |    0.00 |    9 |         - |          NA |
| SymMergeSort             | 4096 | PipeOrgan          |    48,620.0 ns |    749.84 ns |    392.18 ns |    48,720.2 ns |  0.55 |    0.00 |    6 |         - |          NA |
| BlockMergeSort           | 4096 | PipeOrgan          |    48,800.9 ns |    179.32 ns |     79.62 ns |    48,801.7 ns |  0.56 |    0.00 |    6 |         - |          NA |
| NaturalMergeSort         | 4096 | PipeOrgan          |     5,490.6 ns |    346.87 ns |    181.42 ns |     5,529.9 ns |  0.06 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 4096 | PipeOrgan          |   135,936.7 ns | 16,133.72 ns |  8,438.25 ns |   133,269.6 ns |  1.55 |    0.09 |   10 |         - |          NA |
| DropMergeSort            | 4096 | PipeOrgan          |    89,662.7 ns |    522.92 ns |    273.50 ns |    89,652.4 ns |  1.02 |    0.00 |    8 |         - |          NA |
| TimSort                  | 4096 | PipeOrgan          |     8,303.0 ns |    259.86 ns |    135.91 ns |     8,321.4 ns |  0.09 |    0.00 |    3 |         - |          NA |
| PowerSort                | 4096 | PipeOrgan          |     5,192.6 ns |     63.26 ns |     22.56 ns |     5,184.5 ns |  0.06 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | PipeOrgan          |    40,726.3 ns |    843.07 ns |    374.33 ns |    40,601.6 ns |  0.46 |    0.00 |    6 |         - |          NA |
| SpinSort                 | 4096 | PipeOrgan          |     6,645.9 ns |    444.41 ns |    232.43 ns |     6,536.1 ns |  0.08 |    0.00 |    2 |         - |          NA |
| Glidesort                | 4096 | PipeOrgan          |    14,715.2 ns |    276.55 ns |    144.64 ns |    14,698.9 ns |  0.17 |    0.00 |    4 |         - |          NA |
| Driftsort                | 4096 | PipeOrgan          |     6,388.2 ns |  4,922.28 ns |  2,574.45 ns |     4,860.5 ns |  0.07 |    0.03 |    1 |         - |          NA |
| FlatStableSort           | 4096 | PipeOrgan          |    28,452.9 ns |    330.50 ns |    172.86 ns |    28,438.3 ns |  0.32 |    0.00 |    5 |         - |          NA |
| QuadSort                 | 4096 | PipeOrgan          |     9,255.8 ns |    327.36 ns |    171.21 ns |     9,206.1 ns |  0.11 |    0.00 |    3 |         - |          NA |
| SimdMergeSort            | 4096 | PipeOrgan          |    68,872.0 ns |    492.03 ns |    257.34 ns |    68,804.6 ns |  0.79 |    0.00 |    7 |         - |          NA |
| DotnetSort               | 4096 | PipeOrgan          |    71,715.0 ns |  1,294.20 ns |    574.63 ns |    71,675.9 ns |  0.82 |    0.01 |    7 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **4096** | **ManyDuplicates**     |   **215,833.3 ns** |    **798.35 ns** |    **354.47 ns** |   **215,977.7 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | ManyDuplicates     |   202,879.0 ns |    609.08 ns |    270.43 ns |   202,887.3 ns |  0.94 |    0.00 |    4 |         - |          NA |
| BottomupMergeSort        | 4096 | ManyDuplicates     |   153,068.2 ns |  1,312.96 ns |    582.96 ns |   152,891.5 ns |  0.71 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 4096 | ManyDuplicates     |   101,093.8 ns |  3,087.00 ns |  1,614.56 ns |   101,071.2 ns |  0.47 |    0.01 |    4 |         - |          NA |
| RotateMergeSort          | 4096 | ManyDuplicates     |   428,087.0 ns |  2,424.94 ns |  1,268.29 ns |   427,860.1 ns |  1.98 |    0.01 |    6 |         - |          NA |
| RotateMergeSortRecursive | 4096 | ManyDuplicates     |   504,442.6 ns |  2,794.14 ns |  1,240.62 ns |   504,310.2 ns |  2.34 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 4096 | ManyDuplicates     |   287,805.7 ns |  1,415.10 ns |    740.13 ns |   287,630.3 ns |  1.33 |    0.00 |    5 |         - |          NA |
| BlockMergeSort           | 4096 | ManyDuplicates     |   213,172.6 ns |    514.31 ns |    183.41 ns |   213,215.5 ns |  0.99 |    0.00 |    4 |         - |          NA |
| NaturalMergeSort         | 4096 | ManyDuplicates     |   158,569.2 ns |  1,057.09 ns |    469.36 ns |   158,504.5 ns |  0.73 |    0.00 |    4 |         - |          NA |
| PatienceSort             | 4096 | ManyDuplicates     |   139,931.2 ns |    757.53 ns |    396.20 ns |   140,010.7 ns |  0.65 |    0.00 |    4 |         - |          NA |
| DropMergeSort            | 4096 | ManyDuplicates     |   118,052.2 ns |    727.10 ns |    322.84 ns |   118,018.8 ns |  0.55 |    0.00 |    4 |         - |          NA |
| TimSort                  | 4096 | ManyDuplicates     |   183,451.0 ns |    386.35 ns |    171.54 ns |   183,501.9 ns |  0.85 |    0.00 |    4 |         - |          NA |
| PowerSort                | 4096 | ManyDuplicates     |   120,052.4 ns |    848.96 ns |    376.94 ns |   119,987.2 ns |  0.56 |    0.00 |    4 |         - |          NA |
| ShiftSort                | 4096 | ManyDuplicates     |   184,933.9 ns |  2,688.40 ns |  1,193.67 ns |   184,209.0 ns |  0.86 |    0.01 |    4 |         - |          NA |
| SpinSort                 | 4096 | ManyDuplicates     |   109,268.5 ns |  1,252.95 ns |    655.32 ns |   109,142.5 ns |  0.51 |    0.00 |    4 |         - |          NA |
| Glidesort                | 4096 | ManyDuplicates     |    22,346.8 ns |    400.50 ns |    209.47 ns |    22,271.7 ns |  0.10 |    0.00 |    2 |         - |          NA |
| Driftsort                | 4096 | ManyDuplicates     |    17,028.0 ns |    209.57 ns |     93.05 ns |    16,997.6 ns |  0.08 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | ManyDuplicates     |   116,221.0 ns |    952.34 ns |    422.84 ns |   116,169.4 ns |  0.54 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 4096 | ManyDuplicates     |    76,753.3 ns |    192.63 ns |     85.53 ns |    76,766.1 ns |  0.36 |    0.00 |    3 |         - |          NA |
| SimdMergeSort            | 4096 | ManyDuplicates     |    67,726.6 ns |    243.19 ns |    107.98 ns |    67,742.2 ns |  0.31 |    0.00 |    3 |         - |          NA |
| DotnetSort               | 4096 | ManyDuplicates     |    94,579.2 ns |    351.27 ns |    155.97 ns |    94,539.8 ns |  0.44 |    0.00 |    4 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **8192** | **Random**             |   **563,755.2 ns** |  **2,116.43 ns** |    **939.71 ns** |   **563,509.8 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Random             |   534,164.5 ns |  2,719.78 ns |  1,207.60 ns |   534,009.4 ns |  0.95 |    0.00 |    2 |         - |          NA |
| BottomupMergeSort        | 8192 | Random             |   422,554.9 ns |    987.59 ns |    438.50 ns |   422,547.3 ns |  0.75 |    0.00 |    2 |         - |          NA |
| StdStableSort            | 8192 | Random             |   341,910.3 ns |  6,001.97 ns |  3,139.15 ns |   342,126.4 ns |  0.61 |    0.01 |    2 |         - |          NA |
| RotateMergeSort          | 8192 | Random             | 1,288,268.2 ns |  4,870.30 ns |  2,547.26 ns | 1,287,334.0 ns |  2.29 |    0.01 |    4 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Random             | 1,487,589.1 ns |  4,011.34 ns |  1,781.06 ns | 1,487,560.0 ns |  2.64 |    0.01 |    4 |         - |          NA |
| SymMergeSort             | 8192 | Random             |   869,341.1 ns |  3,511.61 ns |  1,559.18 ns |   869,750.5 ns |  1.54 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 8192 | Random             |   525,532.3 ns |  2,445.13 ns |  1,085.65 ns |   525,329.8 ns |  0.93 |    0.00 |    2 |         - |          NA |
| NaturalMergeSort         | 8192 | Random             |   446,050.8 ns |    802.63 ns |    356.37 ns |   445,958.0 ns |  0.79 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 8192 | Random             |   656,188.9 ns |  2,523.37 ns |  1,319.77 ns |   655,943.1 ns |  1.16 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 8192 | Random             |   421,019.1 ns |  2,135.36 ns |  1,116.84 ns |   421,248.3 ns |  0.75 |    0.00 |    2 |         - |          NA |
| TimSort                  | 8192 | Random             |   514,585.1 ns |  1,955.02 ns |  1,022.51 ns |   514,534.1 ns |  0.91 |    0.00 |    2 |         - |          NA |
| PowerSort                | 8192 | Random             |   383,812.0 ns |  1,006.27 ns |    526.30 ns |   383,822.2 ns |  0.68 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 8192 | Random             |   529,749.7 ns |  1,771.52 ns |    926.54 ns |   529,769.6 ns |  0.94 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | Random             |   343,648.9 ns |  1,761.16 ns |    781.97 ns |   343,631.7 ns |  0.61 |    0.00 |    2 |         - |          NA |
| Glidesort                | 8192 | Random             |   204,833.7 ns |  1,887.67 ns |    987.29 ns |   205,032.5 ns |  0.36 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Random             |   173,676.9 ns |    634.17 ns |    281.57 ns |   173,775.6 ns |  0.31 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Random             |   369,112.3 ns |    888.97 ns |    394.71 ns |   369,088.8 ns |  0.65 |    0.00 |    2 |         - |          NA |
| QuadSort                 | 8192 | Random             |   176,936.4 ns |    351.25 ns |    155.96 ns |   176,948.9 ns |  0.31 |    0.00 |    1 |         - |          NA |
| SimdMergeSort            | 8192 | Random             |   161,230.7 ns |  1,438.61 ns |    638.75 ns |   160,989.6 ns |  0.29 |    0.00 |    1 |         - |          NA |
| DotnetSort               | 8192 | Random             |   315,469.4 ns |  2,141.54 ns |    950.86 ns |   315,703.8 ns |  0.56 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **8192** | **SingleElementMoved** |   **110,414.4 ns** |  **1,883.53 ns** |    **836.30 ns** |   **110,077.6 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | SingleElementMoved |   135,149.9 ns |    498.63 ns |    221.40 ns |   135,101.8 ns |  1.22 |    0.01 |    9 |         - |          NA |
| BottomupMergeSort        | 8192 | SingleElementMoved |    42,594.5 ns |    330.87 ns |    173.05 ns |    42,520.6 ns |  0.39 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 8192 | SingleElementMoved |    24,828.3 ns |  1,592.50 ns |    707.08 ns |    24,934.8 ns |  0.22 |    0.01 |    4 |         - |          NA |
| RotateMergeSort          | 8192 | SingleElementMoved |     9,793.9 ns |    658.42 ns |    344.37 ns |     9,700.4 ns |  0.09 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 8192 | SingleElementMoved |    14,390.2 ns |    262.13 ns |    137.10 ns |    14,420.8 ns |  0.13 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 8192 | SingleElementMoved |     8,407.6 ns |     22.01 ns |      9.77 ns |     8,407.6 ns |  0.08 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 8192 | SingleElementMoved |    76,308.8 ns |    598.60 ns |    265.78 ns |    76,307.1 ns |  0.69 |    0.01 |    7 |         - |          NA |
| NaturalMergeSort         | 8192 | SingleElementMoved |     7,557.1 ns |    169.96 ns |     88.89 ns |     7,520.9 ns |  0.07 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 8192 | SingleElementMoved |   622,890.5 ns |  3,045.92 ns |  1,593.07 ns |   622,399.2 ns |  5.64 |    0.04 |   10 |         - |          NA |
| DropMergeSort            | 8192 | SingleElementMoved |    14,627.6 ns |    297.62 ns |    132.15 ns |    14,613.1 ns |  0.13 |    0.00 |    3 |         - |          NA |
| TimSort                  | 8192 | SingleElementMoved |     4,880.0 ns |    326.99 ns |    171.02 ns |     4,774.1 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | SingleElementMoved |     7,817.7 ns |    311.02 ns |    162.67 ns |     7,795.8 ns |  0.07 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 8192 | SingleElementMoved |     9,101.0 ns |    622.53 ns |    325.60 ns |     9,076.9 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | SingleElementMoved |    14,291.3 ns |    353.53 ns |    184.90 ns |    14,288.0 ns |  0.13 |    0.00 |    3 |         - |          NA |
| Glidesort                | 8192 | SingleElementMoved |    13,518.6 ns |    404.49 ns |    179.59 ns |    13,513.5 ns |  0.12 |    0.00 |    3 |         - |          NA |
| Driftsort                | 8192 | SingleElementMoved |     7,935.1 ns |    449.16 ns |    234.92 ns |     7,902.9 ns |  0.07 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 8192 | SingleElementMoved |    23,312.4 ns |    369.35 ns |    164.00 ns |    23,279.7 ns |  0.21 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 8192 | SingleElementMoved |     7,734.4 ns |    311.75 ns |    163.05 ns |     7,680.6 ns |  0.07 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 8192 | SingleElementMoved |   156,914.3 ns |  1,032.82 ns |    458.58 ns |   156,830.2 ns |  1.42 |    0.01 |    9 |         - |          NA |
| DotnetSort               | 8192 | SingleElementMoved |    52,989.1 ns |    537.61 ns |    281.18 ns |    53,018.5 ns |  0.48 |    0.00 |    6 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **8192** | **Sorted**             |   **106,197.9 ns** |    **317.69 ns** |    **141.06 ns** |   **106,203.4 ns** |  **1.00** |    **0.00** |    **9** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Sorted             |   131,884.7 ns |    291.21 ns |    129.30 ns |   131,845.5 ns |  1.24 |    0.00 |   10 |         - |          NA |
| BottomupMergeSort        | 8192 | Sorted             |    39,560.9 ns |    205.70 ns |    107.58 ns |    39,593.2 ns |  0.37 |    0.00 |    7 |         - |          NA |
| StdStableSort            | 8192 | Sorted             |    23,069.9 ns |    559.03 ns |    292.38 ns |    22,938.1 ns |  0.22 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 8192 | Sorted             |     7,434.8 ns |     20.02 ns |      7.14 ns |     7,432.6 ns |  0.07 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Sorted             |    12,395.5 ns |    149.02 ns |     77.94 ns |    12,421.8 ns |  0.12 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 8192 | Sorted             |     7,177.3 ns |     16.09 ns |      7.14 ns |     7,177.5 ns |  0.07 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 8192 | Sorted             |    66,606.0 ns |    699.00 ns |    310.36 ns |    66,499.5 ns |  0.63 |    0.00 |    8 |         - |          NA |
| NaturalMergeSort         | 8192 | Sorted             |     3,618.4 ns |    102.92 ns |     36.70 ns |     3,608.6 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 8192 | Sorted             |   611,119.0 ns |  3,339.26 ns |  1,746.50 ns |   610,831.5 ns |  5.75 |    0.02 |   12 |         - |          NA |
| DropMergeSort            | 8192 | Sorted             |    13,708.7 ns |    346.04 ns |    180.98 ns |    13,677.8 ns |  0.13 |    0.00 |    4 |         - |          NA |
| TimSort                  | 8192 | Sorted             |     3,562.6 ns |     10.56 ns |      3.77 ns |     3,561.1 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Sorted             |     3,553.4 ns |     55.01 ns |     19.62 ns |     3,545.0 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Sorted             |     4,620.8 ns |    247.26 ns |    129.32 ns |     4,537.7 ns |  0.04 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | Sorted             |     3,149.5 ns |    227.07 ns |    100.82 ns |     3,095.3 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Sorted             |     3,072.3 ns |    109.23 ns |     48.50 ns |     3,051.2 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Sorted             |     3,626.6 ns |     28.76 ns |     10.26 ns |     3,626.2 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Sorted             |     3,264.3 ns |    282.91 ns |    125.61 ns |     3,209.9 ns |  0.03 |    0.00 |    1 |         - |          NA |
| QuadSort                 | 8192 | Sorted             |     4,689.8 ns |    156.83 ns |     69.63 ns |     4,653.1 ns |  0.04 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 8192 | Sorted             |   195,841.2 ns |  1,096.08 ns |    486.66 ns |   195,723.6 ns |  1.84 |    0.00 |   11 |         - |          NA |
| DotnetSort               | 8192 | Sorted             |    33,549.7 ns |  3,132.53 ns |  1,638.37 ns |    32,805.3 ns |  0.32 |    0.01 |    6 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **8192** | **Reversed**           |   **241,723.6 ns** |    **437.95 ns** |    **194.45 ns** |   **241,654.1 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Reversed           |   211,459.4 ns |  1,660.00 ns |    868.21 ns |   211,367.2 ns |  0.87 |    0.00 |    6 |         - |          NA |
| BottomupMergeSort        | 8192 | Reversed           |   130,879.3 ns |    279.76 ns |    124.22 ns |   130,900.8 ns |  0.54 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 8192 | Reversed           |   206,675.4 ns |  1,640.93 ns |    728.58 ns |   206,851.3 ns |  0.86 |    0.00 |    6 |         - |          NA |
| RotateMergeSort          | 8192 | Reversed           |   198,506.9 ns |  1,031.13 ns |    457.83 ns |   198,480.7 ns |  0.82 |    0.00 |    6 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Reversed           |   260,429.2 ns |  3,878.39 ns |  2,028.47 ns |   259,366.0 ns |  1.08 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 8192 | Reversed           |    66,594.0 ns |    251.97 ns |    111.88 ns |    66,644.1 ns |  0.28 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 8192 | Reversed           |   109,033.2 ns |  1,004.88 ns |    446.17 ns |   108,949.5 ns |  0.45 |    0.00 |    4 |         - |          NA |
| NaturalMergeSort         | 8192 | Reversed           |     5,546.4 ns |     30.44 ns |     10.86 ns |     5,546.2 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 8192 | Reversed           |    33,221.5 ns |    767.15 ns |    273.57 ns |    33,119.1 ns |  0.14 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 8192 | Reversed           |    50,633.9 ns |    479.43 ns |    250.75 ns |    50,678.8 ns |  0.21 |    0.00 |    3 |         - |          NA |
| TimSort                  | 8192 | Reversed           |     5,653.3 ns |    321.99 ns |    168.41 ns |     5,584.5 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Reversed           |     5,638.6 ns |    261.90 ns |    136.98 ns |     5,555.7 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Reversed           |   144,159.9 ns |  1,505.72 ns |    668.55 ns |   143,978.1 ns |  0.60 |    0.00 |    5 |         - |          NA |
| SpinSort                 | 8192 | Reversed           |     6,140.5 ns |    202.66 ns |    106.00 ns |     6,166.0 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Reversed           |     5,603.5 ns |    152.57 ns |     67.74 ns |     5,576.8 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Reversed           |     5,635.3 ns |    182.56 ns |     95.48 ns |     5,574.6 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Reversed           |     6,143.9 ns |    413.29 ns |    216.16 ns |     5,998.9 ns |  0.03 |    0.00 |    1 |         - |          NA |
| QuadSort                 | 8192 | Reversed           |     6,068.4 ns |    252.27 ns |    131.94 ns |     6,051.0 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SimdMergeSort            | 8192 | Reversed           |   194,313.4 ns |    503.82 ns |    223.70 ns |   194,336.6 ns |  0.80 |    0.00 |    6 |         - |          NA |
| DotnetSort               | 8192 | Reversed           |    59,820.0 ns |  4,705.05 ns |  2,460.83 ns |    59,271.8 ns |  0.25 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **8192** | **PipeOrgan**          |   **177,431.1 ns** |    **457.25 ns** |    **203.02 ns** |   **177,355.5 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | PipeOrgan          |   175,312.1 ns |  3,414.54 ns |  1,516.08 ns |   174,529.6 ns |  0.99 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 8192 | PipeOrgan          |    87,105.6 ns |    891.64 ns |    395.89 ns |    86,866.8 ns |  0.49 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 8192 | PipeOrgan          |   118,717.5 ns |  1,331.90 ns |    696.61 ns |   118,756.1 ns |  0.67 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 8192 | PipeOrgan          |   185,343.5 ns |    859.45 ns |    381.60 ns |   185,470.1 ns |  1.04 |    0.00 |    6 |         - |          NA |
| RotateMergeSortRecursive | 8192 | PipeOrgan          |   225,535.7 ns |  3,392.18 ns |  1,506.15 ns |   224,941.9 ns |  1.27 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 8192 | PipeOrgan          |    99,280.1 ns |  1,763.88 ns |    922.55 ns |    99,355.7 ns |  0.56 |    0.00 |    5 |         - |          NA |
| BlockMergeSort           | 8192 | PipeOrgan          |   100,302.5 ns |    520.38 ns |    231.05 ns |   100,299.1 ns |  0.57 |    0.00 |    5 |         - |          NA |
| NaturalMergeSort         | 8192 | PipeOrgan          |    10,585.6 ns |    402.23 ns |    210.37 ns |    10,571.1 ns |  0.06 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 8192 | PipeOrgan          |   492,389.7 ns |  4,636.99 ns |  2,425.24 ns |   492,493.0 ns |  2.78 |    0.01 |    8 |         - |          NA |
| DropMergeSort            | 8192 | PipeOrgan          |   182,190.6 ns |  2,024.51 ns |  1,058.86 ns |   181,815.4 ns |  1.03 |    0.01 |    6 |         - |          NA |
| TimSort                  | 8192 | PipeOrgan          |    16,534.8 ns |    402.80 ns |    210.67 ns |    16,468.4 ns |  0.09 |    0.00 |    2 |         - |          NA |
| PowerSort                | 8192 | PipeOrgan          |    10,453.8 ns |    388.50 ns |    172.50 ns |    10,524.4 ns |  0.06 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | PipeOrgan          |    81,881.8 ns |    275.65 ns |    144.17 ns |    81,908.7 ns |  0.46 |    0.00 |    5 |         - |          NA |
| SpinSort                 | 8192 | PipeOrgan          |    14,158.3 ns |    334.27 ns |    148.42 ns |    14,232.7 ns |  0.08 |    0.00 |    2 |         - |          NA |
| Glidesort                | 8192 | PipeOrgan          |    29,451.0 ns |    255.14 ns |    113.28 ns |    29,492.9 ns |  0.17 |    0.00 |    3 |         - |          NA |
| Driftsort                | 8192 | PipeOrgan          |     8,818.2 ns |    311.64 ns |    138.37 ns |     8,757.9 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | PipeOrgan          |    51,595.8 ns |    708.29 ns |    370.45 ns |    51,463.2 ns |  0.29 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 8192 | PipeOrgan          |    18,786.7 ns |    477.93 ns |    249.97 ns |    18,670.3 ns |  0.11 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 8192 | PipeOrgan          |   199,764.0 ns |  2,055.48 ns |  1,075.06 ns |   199,251.3 ns |  1.13 |    0.01 |    6 |         - |          NA |
| DotnetSort               | 8192 | PipeOrgan          |   281,133.8 ns |  7,494.09 ns |  3,919.55 ns |   279,922.2 ns |  1.58 |    0.02 |    7 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **8192** | **ManyDuplicates**     |   **438,469.5 ns** |  **4,370.76 ns** |  **2,285.99 ns** |   **438,042.4 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | ManyDuplicates     |   411,098.4 ns |    989.29 ns |    439.25 ns |   411,034.3 ns |  0.94 |    0.00 |    4 |         - |          NA |
| BottomupMergeSort        | 8192 | ManyDuplicates     |   309,300.3 ns |  1,723.98 ns |    901.68 ns |   309,061.3 ns |  0.71 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 8192 | ManyDuplicates     |   220,788.2 ns |  4,305.16 ns |  2,251.68 ns |   220,855.4 ns |  0.50 |    0.01 |    4 |         - |          NA |
| RotateMergeSort          | 8192 | ManyDuplicates     |   884,669.1 ns |  1,716.91 ns |    762.32 ns |   884,728.7 ns |  2.02 |    0.01 |    6 |         - |          NA |
| RotateMergeSortRecursive | 8192 | ManyDuplicates     | 1,045,445.0 ns |  3,394.46 ns |  1,775.37 ns | 1,045,656.3 ns |  2.38 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 8192 | ManyDuplicates     |   601,036.0 ns |  1,670.35 ns |    873.63 ns |   601,229.0 ns |  1.37 |    0.01 |    5 |         - |          NA |
| BlockMergeSort           | 8192 | ManyDuplicates     |   439,909.3 ns |  3,080.55 ns |  1,611.19 ns |   439,579.4 ns |  1.00 |    0.01 |    4 |         - |          NA |
| NaturalMergeSort         | 8192 | ManyDuplicates     |   319,460.1 ns |    630.51 ns |    279.95 ns |   319,488.0 ns |  0.73 |    0.00 |    4 |         - |          NA |
| PatienceSort             | 8192 | ManyDuplicates     |   280,227.6 ns |    905.76 ns |    473.73 ns |   280,071.2 ns |  0.64 |    0.00 |    4 |         - |          NA |
| DropMergeSort            | 8192 | ManyDuplicates     |   233,295.4 ns |    873.64 ns |    387.90 ns |   233,259.4 ns |  0.53 |    0.00 |    4 |         - |          NA |
| TimSort                  | 8192 | ManyDuplicates     |   371,025.8 ns |    984.43 ns |    514.88 ns |   370,904.3 ns |  0.85 |    0.00 |    4 |         - |          NA |
| PowerSort                | 8192 | ManyDuplicates     |   247,426.8 ns |  1,337.78 ns |    699.68 ns |   247,149.8 ns |  0.56 |    0.00 |    4 |         - |          NA |
| ShiftSort                | 8192 | ManyDuplicates     |   384,314.8 ns |  1,591.03 ns |    832.14 ns |   384,376.5 ns |  0.88 |    0.00 |    4 |         - |          NA |
| SpinSort                 | 8192 | ManyDuplicates     |   223,714.5 ns |  1,736.35 ns |    770.95 ns |   223,873.8 ns |  0.51 |    0.00 |    4 |         - |          NA |
| Glidesort                | 8192 | ManyDuplicates     |    35,547.9 ns |  1,431.49 ns |    748.70 ns |    35,398.8 ns |  0.08 |    0.00 |    2 |         - |          NA |
| Driftsort                | 8192 | ManyDuplicates     |    24,305.3 ns |    254.07 ns |    112.81 ns |    24,299.5 ns |  0.06 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | ManyDuplicates     |   246,951.5 ns |    784.44 ns |    279.74 ns |   247,005.0 ns |  0.56 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 8192 | ManyDuplicates     |   156,148.7 ns |    615.21 ns |    273.16 ns |   156,020.1 ns |  0.36 |    0.00 |    3 |         - |          NA |
| SimdMergeSort            | 8192 | ManyDuplicates     |   194,793.0 ns |    606.68 ns |    269.37 ns |   194,879.1 ns |  0.44 |    0.00 |    4 |         - |          NA |
| DotnetSort               | 8192 | ManyDuplicates     |   190,628.9 ns |  1,039.17 ns |    543.51 ns |   190,498.0 ns |  0.43 |    0.00 |    4 |         - |          NA |

### MergeSlowBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.49 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method     | Size | Pattern            | Mean         | Error       | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------- |----- |------------------- |-------------:|------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **StrandSort** | **256**  | **Random**             |   **9,104.2 ns** |   **256.46 ns** |   **113.87 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DotnetSort | 256  | Random             |   5,731.9 ns |    37.03 ns |    13.20 ns |  0.63 |    0.01 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **256**  | **SingleElementMoved** |     **715.5 ns** |    **36.14 ns** |    **16.05 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DotnetSort | 256  | SingleElementMoved |   1,083.3 ns |    45.14 ns |    20.04 ns |  1.51 |    0.04 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **256**  | **Sorted**             |     **472.5 ns** |    **12.53 ns** |     **5.56 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DotnetSort | 256  | Sorted             |     666.2 ns |    49.57 ns |    25.93 ns |  1.41 |    0.05 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **256**  | **Reversed**           |  **38,911.3 ns** | **1,238.84 ns** |   **647.94 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DotnetSort | 256  | Reversed           |   1,144.1 ns |   164.41 ns |    73.00 ns |  0.03 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **256**  | **PipeOrgan**          |  **22,865.9 ns** |   **577.82 ns** |   **302.21 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DotnetSort | 256  | PipeOrgan          |   1,827.2 ns |   185.70 ns |    82.45 ns |  0.08 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **256**  | **ManyDuplicates**     |   **8,001.7 ns** |   **128.10 ns** |    **67.00 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DotnetSort | 256  | ManyDuplicates     |   5,575.1 ns |   263.46 ns |   137.79 ns |  0.70 |    0.02 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **1024** | **Random**             |  **57,671.0 ns** |   **844.61 ns** |   **375.01 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DotnetSort | 1024 | Random             |  29,741.6 ns |   371.91 ns |   194.51 ns |  0.52 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **1024** | **SingleElementMoved** |   **2,178.4 ns** |    **27.06 ns** |    **12.01 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DotnetSort | 1024 | SingleElementMoved |   5,122.2 ns |   107.82 ns |    47.87 ns |  2.35 |    0.02 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **1024** | **Sorted**             |   **1,413.8 ns** |    **13.68 ns** |     **4.88 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DotnetSort | 1024 | Sorted             |   3,211.5 ns |   219.54 ns |   114.83 ns |  2.27 |    0.08 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **1024** | **Reversed**           | **627,755.3 ns** | **4,795.81 ns** | **2,508.30 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DotnetSort | 1024 | Reversed           |   6,352.4 ns |   514.59 ns |   269.14 ns |  0.01 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **1024** | **PipeOrgan**          | **372,380.8 ns** | **2,851.42 ns** | **1,266.05 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DotnetSort | 1024 | PipeOrgan          |  12,729.4 ns |   249.89 ns |   110.95 ns |  0.03 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **1024** | **ManyDuplicates**     |  **35,484.5 ns** |   **155.93 ns** |    **81.56 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DotnetSort | 1024 | ManyDuplicates     |  23,402.1 ns |   468.46 ns |   208.00 ns |  0.66 |    0.01 |    1 |         - |          NA |

### NetworkBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.49 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                            | Size | Pattern            | Mean           | Error        | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ---------------------------------- |----- |------------------- |---------------:|-------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **BitonicSort**                       | **256**  | **Random**             |    **13,132.9 ns** |    **296.58 ns** |   **131.68 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| BitonicSortBranchless             | 256  | Random             |     3,706.6 ns |     54.01 ns |    23.98 ns |  0.28 |    0.00 |    2 |         - |          NA |
| BitonicRecursiveSort              | 256  | Random             |    26,163.2 ns |    279.33 ns |   124.02 ns |  1.99 |    0.02 |    6 |         - |          NA |
| BitonicSortAnyLength              | 256  | Random             |    20,475.4 ns |    592.62 ns |   263.13 ns |  1.56 |    0.02 |    5 |         - |          NA |
| BitonicSortAnyLengthSimd          | 256  | Random             |     1,089.2 ns |     41.82 ns |    18.57 ns |  0.08 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 256  | Random             |    10,515.3 ns |    301.32 ns |   133.79 ns |  0.80 |    0.01 |    4 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 256  | Random             |     3,455.9 ns |    206.78 ns |    91.81 ns |  0.26 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 256  | Random             |    12,118.6 ns |    240.97 ns |   126.03 ns |  0.92 |    0.01 |    4 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 256  | Random             |     3,320.3 ns |     32.36 ns |    14.37 ns |  0.25 |    0.00 |    2 |         - |          NA |
| DotnetSort                        | 256  | Random             |     5,918.1 ns |    349.21 ns |   182.64 ns |  0.45 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**                       | **256**  | **SingleElementMoved** |     **3,886.3 ns** |    **402.56 ns** |   **178.74 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 256  | SingleElementMoved |     3,715.9 ns |     49.17 ns |    21.83 ns |  0.96 |    0.04 |    2 |         - |          NA |
| BitonicRecursiveSort              | 256  | SingleElementMoved |    18,007.7 ns |     37.89 ns |    13.51 ns |  4.64 |    0.20 |    4 |         - |          NA |
| BitonicSortAnyLength              | 256  | SingleElementMoved |    11,618.2 ns |    292.68 ns |   153.08 ns |  2.99 |    0.13 |    3 |         - |          NA |
| BitonicSortAnyLengthSimd          | 256  | SingleElementMoved |     1,141.7 ns |      7.27 ns |     3.23 ns |  0.29 |    0.01 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 256  | SingleElementMoved |     3,811.3 ns |    225.09 ns |   117.73 ns |  0.98 |    0.05 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 256  | SingleElementMoved |     3,481.4 ns |    330.99 ns |   173.11 ns |  0.90 |    0.06 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 256  | SingleElementMoved |     2,954.0 ns |    361.56 ns |   189.10 ns |  0.76 |    0.06 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 256  | SingleElementMoved |     3,338.3 ns |    145.63 ns |    64.66 ns |  0.86 |    0.04 |    2 |         - |          NA |
| DotnetSort                        | 256  | SingleElementMoved |     1,082.2 ns |     60.94 ns |    27.06 ns |  0.28 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**                       | **256**  | **Sorted**             |     **3,456.1 ns** |     **71.77 ns** |    **31.87 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 256  | Sorted             |     3,732.4 ns |     40.86 ns |    14.57 ns |  1.08 |    0.01 |    3 |         - |          NA |
| BitonicRecursiveSort              | 256  | Sorted             |    18,145.1 ns |    214.17 ns |   112.02 ns |  5.25 |    0.05 |    5 |         - |          NA |
| BitonicSortAnyLength              | 256  | Sorted             |    11,526.9 ns |    184.96 ns |    96.74 ns |  3.34 |    0.04 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 256  | Sorted             |     1,134.1 ns |     46.41 ns |    20.61 ns |  0.33 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 256  | Sorted             |     3,713.4 ns |     45.05 ns |    16.07 ns |  1.07 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 256  | Sorted             |     3,402.5 ns |     35.32 ns |    15.68 ns |  0.98 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetwork            | 256  | Sorted             |     2,881.6 ns |    342.51 ns |   152.08 ns |  0.83 |    0.04 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 256  | Sorted             |     3,338.3 ns |     17.47 ns |     6.23 ns |  0.97 |    0.01 |    3 |         - |          NA |
| DotnetSort                        | 256  | Sorted             |       652.1 ns |      8.35 ns |     3.71 ns |  0.19 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**                       | **256**  | **Reversed**           |     **3,581.1 ns** |    **167.36 ns** |    **74.31 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 256  | Reversed           |     3,725.0 ns |     41.40 ns |    14.76 ns |  1.04 |    0.02 |    3 |         - |          NA |
| BitonicRecursiveSort              | 256  | Reversed           |    18,013.4 ns |    134.26 ns |    70.22 ns |  5.03 |    0.10 |    5 |         - |          NA |
| BitonicSortAnyLength              | 256  | Reversed           |    11,663.1 ns |    247.93 ns |   129.67 ns |  3.26 |    0.07 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 256  | Reversed           |     1,145.1 ns |     11.73 ns |     5.21 ns |  0.32 |    0.01 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 256  | Reversed           |     3,594.3 ns |    457.14 ns |   239.09 ns |  1.00 |    0.07 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 256  | Reversed           |     3,418.4 ns |    143.94 ns |    75.28 ns |  0.95 |    0.03 |    3 |         - |          NA |
| PairwiseSortingNetwork            | 256  | Reversed           |     2,682.6 ns |      9.88 ns |     3.52 ns |  0.75 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 256  | Reversed           |     3,330.5 ns |     26.58 ns |     9.48 ns |  0.93 |    0.02 |    3 |         - |          NA |
| DotnetSort                        | 256  | Reversed           |     1,163.0 ns |     26.54 ns |    11.78 ns |  0.32 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**                       | **256**  | **PipeOrgan**          |     **3,746.5 ns** |    **286.12 ns** |   **127.04 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 256  | PipeOrgan          |     3,846.5 ns |    281.50 ns |   147.23 ns |  1.03 |    0.05 |    3 |         - |          NA |
| BitonicRecursiveSort              | 256  | PipeOrgan          |    17,988.8 ns |     56.89 ns |    25.26 ns |  4.81 |    0.14 |    5 |         - |          NA |
| BitonicSortAnyLength              | 256  | PipeOrgan          |    11,568.5 ns |    264.54 ns |   117.46 ns |  3.09 |    0.10 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 256  | PipeOrgan          |     1,052.5 ns |      7.56 ns |     2.70 ns |  0.28 |    0.01 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 256  | PipeOrgan          |     3,443.0 ns |    305.52 ns |   135.65 ns |  0.92 |    0.04 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 256  | PipeOrgan          |     3,401.7 ns |    193.18 ns |    85.77 ns |  0.91 |    0.03 |    3 |         - |          NA |
| PairwiseSortingNetwork            | 256  | PipeOrgan          |     2,826.1 ns |    343.73 ns |   152.62 ns |  0.76 |    0.04 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 256  | PipeOrgan          |     3,425.4 ns |    354.71 ns |   185.52 ns |  0.92 |    0.05 |    3 |         - |          NA |
| DotnetSort                        | 256  | PipeOrgan          |     1,939.4 ns |    374.31 ns |   195.77 ns |  0.52 |    0.05 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**                       | **256**  | **ManyDuplicates**     |    **11,502.4 ns** |    **283.92 ns** |   **126.06 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| BitonicSortBranchless             | 256  | ManyDuplicates     |     3,720.8 ns |     36.30 ns |    12.95 ns |  0.32 |    0.00 |    2 |         - |          NA |
| BitonicRecursiveSort              | 256  | ManyDuplicates     |    25,449.2 ns |    311.69 ns |   138.39 ns |  2.21 |    0.03 |    6 |         - |          NA |
| BitonicSortAnyLength              | 256  | ManyDuplicates     |    20,022.6 ns |    420.57 ns |   219.97 ns |  1.74 |    0.03 |    5 |         - |          NA |
| BitonicSortAnyLengthSimd          | 256  | ManyDuplicates     |     1,146.2 ns |     24.96 ns |     8.90 ns |  0.10 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 256  | ManyDuplicates     |     9,973.8 ns |    435.49 ns |   227.77 ns |  0.87 |    0.02 |    4 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 256  | ManyDuplicates     |     3,407.8 ns |     58.96 ns |    26.18 ns |  0.30 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 256  | ManyDuplicates     |    11,337.3 ns |    252.07 ns |   111.92 ns |  0.99 |    0.01 |    4 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 256  | ManyDuplicates     |     3,330.5 ns |     37.85 ns |    13.50 ns |  0.29 |    0.00 |    2 |         - |          NA |
| DotnetSort                        | 256  | ManyDuplicates     |     5,490.4 ns |    244.28 ns |   127.76 ns |  0.48 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**                       | **1024** | **Random**             |    **66,448.4 ns** |    **563.40 ns** |   **200.91 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| BitonicSortBranchless             | 1024 | Random             |    22,762.8 ns |    234.42 ns |   122.61 ns |  0.34 |    0.00 |    2 |         - |          NA |
| BitonicRecursiveSort              | 1024 | Random             |   138,244.4 ns |    666.90 ns |   296.11 ns |  2.08 |    0.01 |    7 |         - |          NA |
| BitonicSortAnyLength              | 1024 | Random             |   111,220.8 ns |  1,564.88 ns |   694.82 ns |  1.67 |    0.01 |    6 |         - |          NA |
| BitonicSortAnyLengthSimd          | 1024 | Random             |     6,945.5 ns |    181.13 ns |    94.74 ns |  0.10 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 1024 | Random             |    54,643.2 ns |    750.25 ns |   392.39 ns |  0.82 |    0.01 |    4 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 1024 | Random             |    20,827.0 ns |    534.03 ns |   237.11 ns |  0.31 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 1024 | Random             |    66,623.5 ns |    375.41 ns |   166.68 ns |  1.00 |    0.00 |    5 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 1024 | Random             |    20,549.3 ns |    296.08 ns |   154.86 ns |  0.31 |    0.00 |    2 |         - |          NA |
| DotnetSort                        | 1024 | Random             |    29,959.3 ns |    361.60 ns |   189.13 ns |  0.45 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**                       | **1024** | **SingleElementMoved** |    **21,602.5 ns** |    **855.12 ns** |   **447.25 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| BitonicSortBranchless             | 1024 | SingleElementMoved |    23,013.3 ns |    941.38 ns |   492.36 ns |  1.07 |    0.03 |    4 |         - |          NA |
| BitonicRecursiveSort              | 1024 | SingleElementMoved |    93,795.5 ns |  1,088.29 ns |   483.21 ns |  4.34 |    0.09 |    6 |         - |          NA |
| BitonicSortAnyLength              | 1024 | SingleElementMoved |    61,808.1 ns |    158.35 ns |    70.31 ns |  2.86 |    0.06 |    5 |         - |          NA |
| BitonicSortAnyLengthSimd          | 1024 | SingleElementMoved |     6,787.8 ns |    314.25 ns |   164.36 ns |  0.31 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 1024 | SingleElementMoved |    23,016.4 ns |  1,264.55 ns |   561.47 ns |  1.07 |    0.03 |    4 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 1024 | SingleElementMoved |    20,462.9 ns |    226.45 ns |   100.55 ns |  0.95 |    0.02 |    4 |         - |          NA |
| PairwiseSortingNetwork            | 1024 | SingleElementMoved |    17,184.7 ns |  1,545.42 ns |   808.28 ns |  0.80 |    0.04 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 1024 | SingleElementMoved |    20,692.0 ns |    440.04 ns |   230.15 ns |  0.96 |    0.02 |    4 |         - |          NA |
| DotnetSort                        | 1024 | SingleElementMoved |     5,410.3 ns |    299.39 ns |   156.59 ns |  0.25 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**                       | **1024** | **Sorted**             |    **20,992.4 ns** |    **532.80 ns** |   **278.66 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 1024 | Sorted             |    22,742.8 ns |    170.85 ns |    89.36 ns |  1.08 |    0.01 |    3 |         - |          NA |
| BitonicRecursiveSort              | 1024 | Sorted             |    94,165.5 ns |    643.11 ns |   336.36 ns |  4.49 |    0.06 |    5 |         - |          NA |
| BitonicSortAnyLength              | 1024 | Sorted             |    61,570.6 ns |    174.86 ns |    91.46 ns |  2.93 |    0.04 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 1024 | Sorted             |     6,859.3 ns |    308.12 ns |   161.15 ns |  0.33 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 1024 | Sorted             |    23,150.7 ns |    394.01 ns |   206.08 ns |  1.10 |    0.02 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 1024 | Sorted             |    20,651.9 ns |    207.38 ns |   108.46 ns |  0.98 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetwork            | 1024 | Sorted             |    17,572.7 ns |    858.72 ns |   449.13 ns |  0.84 |    0.02 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 1024 | Sorted             |    20,571.2 ns |    356.28 ns |   127.05 ns |  0.98 |    0.01 |    3 |         - |          NA |
| DotnetSort                        | 1024 | Sorted             |     3,125.8 ns |     46.26 ns |    16.50 ns |  0.15 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**                       | **1024** | **Reversed**           |    **22,456.1 ns** |  **1,904.63 ns** |   **996.16 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 1024 | Reversed           |    22,684.9 ns |    385.86 ns |   201.81 ns |  1.01 |    0.04 |    2 |         - |          NA |
| BitonicRecursiveSort              | 1024 | Reversed           |    93,025.1 ns |    187.90 ns |    83.43 ns |  4.15 |    0.17 |    4 |         - |          NA |
| BitonicSortAnyLength              | 1024 | Reversed           |    62,155.2 ns |    266.41 ns |   139.34 ns |  2.77 |    0.12 |    3 |         - |          NA |
| BitonicSortAnyLengthSimd          | 1024 | Reversed           |     6,741.7 ns |    207.53 ns |   108.54 ns |  0.30 |    0.01 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 1024 | Reversed           |    21,879.7 ns |    876.18 ns |   458.26 ns |  0.98 |    0.05 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 1024 | Reversed           |    20,655.3 ns |    147.03 ns |    76.90 ns |  0.92 |    0.04 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 1024 | Reversed           |    17,868.0 ns |    890.14 ns |   465.56 ns |  0.80 |    0.04 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 1024 | Reversed           |    20,501.4 ns |    503.16 ns |   263.16 ns |  0.91 |    0.04 |    2 |         - |          NA |
| DotnetSort                        | 1024 | Reversed           |     6,593.6 ns |    394.65 ns |   175.23 ns |  0.29 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**                       | **1024** | **PipeOrgan**          |    **21,382.9 ns** |    **579.15 ns** |   **302.91 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| BitonicSortBranchless             | 1024 | PipeOrgan          |    22,674.4 ns |    341.21 ns |   178.46 ns |  1.06 |    0.02 |    4 |         - |          NA |
| BitonicRecursiveSort              | 1024 | PipeOrgan          |    93,441.0 ns |    691.67 ns |   307.11 ns |  4.37 |    0.06 |    6 |         - |          NA |
| BitonicSortAnyLength              | 1024 | PipeOrgan          |    61,603.0 ns |    179.88 ns |    94.08 ns |  2.88 |    0.04 |    5 |         - |          NA |
| BitonicSortAnyLengthSimd          | 1024 | PipeOrgan          |     6,394.5 ns |    294.69 ns |   154.13 ns |  0.30 |    0.01 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 1024 | PipeOrgan          |    22,279.2 ns |    678.96 ns |   355.11 ns |  1.04 |    0.02 |    4 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 1024 | PipeOrgan          |    20,550.8 ns |    187.11 ns |    97.86 ns |  0.96 |    0.01 |    4 |         - |          NA |
| PairwiseSortingNetwork            | 1024 | PipeOrgan          |    16,707.8 ns |    810.75 ns |   359.98 ns |  0.78 |    0.02 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 1024 | PipeOrgan          |    20,444.9 ns |    143.57 ns |    75.09 ns |  0.96 |    0.01 |    4 |         - |          NA |
| DotnetSort                        | 1024 | PipeOrgan          |    12,848.5 ns |    517.22 ns |   229.65 ns |  0.60 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**                       | **1024** | **ManyDuplicates**     |    **55,081.3 ns** |    **545.56 ns** |   **285.34 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 1024 | ManyDuplicates     |    22,667.3 ns |    218.28 ns |   114.16 ns |  0.41 |    0.00 |    2 |         - |          NA |
| BitonicRecursiveSort              | 1024 | ManyDuplicates     |   128,559.7 ns |    990.63 ns |   439.84 ns |  2.33 |    0.01 |    5 |         - |          NA |
| BitonicSortAnyLength              | 1024 | ManyDuplicates     |   100,689.3 ns |  2,330.05 ns | 1,034.56 ns |  1.83 |    0.02 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 1024 | ManyDuplicates     |     6,956.6 ns |    283.79 ns |   148.43 ns |  0.13 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 1024 | ManyDuplicates     |    48,861.9 ns |    750.94 ns |   392.76 ns |  0.89 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 1024 | ManyDuplicates     |    20,734.5 ns |    325.30 ns |   170.14 ns |  0.38 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 1024 | ManyDuplicates     |    54,368.4 ns |    517.05 ns |   229.57 ns |  0.99 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 1024 | ManyDuplicates     |    20,324.8 ns |    160.43 ns |    83.91 ns |  0.37 |    0.00 |    2 |         - |          NA |
| DotnetSort                        | 1024 | ManyDuplicates     |    23,312.7 ns |    330.15 ns |   172.68 ns |  0.42 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**                       | **4096** | **Random**             |   **337,885.1 ns** | **15,782.20 ns** | **8,254.40 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| BitonicSortBranchless             | 4096 | Random             |   126,535.2 ns |    519.31 ns |   230.58 ns |  0.37 |    0.01 |    2 |         - |          NA |
| BitonicRecursiveSort              | 4096 | Random             |   692,558.0 ns |  1,261.13 ns |   659.59 ns |  2.05 |    0.05 |    6 |         - |          NA |
| BitonicSortAnyLength              | 4096 | Random             |   558,721.3 ns |  4,541.03 ns | 2,016.24 ns |  1.65 |    0.04 |    5 |         - |          NA |
| BitonicSortAnyLengthSimd          | 4096 | Random             |    37,991.6 ns |    225.26 ns |   117.82 ns |  0.11 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 4096 | Random             |   276,201.8 ns |  1,621.77 ns |   720.08 ns |  0.82 |    0.02 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 4096 | Random             |   116,341.3 ns |  1,779.22 ns |   930.57 ns |  0.34 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 4096 | Random             |   349,656.6 ns |  1,509.00 ns |   670.00 ns |  1.04 |    0.02 |    4 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 4096 | Random             |   115,119.6 ns |  1,650.09 ns |   732.65 ns |  0.34 |    0.01 |    2 |         - |          NA |
| DotnetSort                        | 4096 | Random             |   144,876.5 ns |  1,977.13 ns | 1,034.08 ns |  0.43 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**                       | **4096** | **SingleElementMoved** |   **117,261.8 ns** |  **1,148.35 ns** |   **509.87 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| BitonicSortBranchless             | 4096 | SingleElementMoved |   126,385.7 ns |    316.31 ns |   140.44 ns |  1.08 |    0.00 |    4 |         - |          NA |
| BitonicRecursiveSort              | 4096 | SingleElementMoved |   465,444.4 ns |  1,326.59 ns |   589.01 ns |  3.97 |    0.02 |    6 |         - |          NA |
| BitonicSortAnyLength              | 4096 | SingleElementMoved |   314,823.8 ns |    145.23 ns |    51.79 ns |  2.68 |    0.01 |    5 |         - |          NA |
| BitonicSortAnyLengthSimd          | 4096 | SingleElementMoved |    37,301.9 ns |    237.23 ns |   105.33 ns |  0.32 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 4096 | SingleElementMoved |   128,699.9 ns |  2,739.25 ns | 1,216.24 ns |  1.10 |    0.01 |    4 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 4096 | SingleElementMoved |   116,168.2 ns |  1,609.79 ns |   841.95 ns |  0.99 |    0.01 |    4 |         - |          NA |
| PairwiseSortingNetwork            | 4096 | SingleElementMoved |    95,703.8 ns |  3,313.45 ns | 1,471.19 ns |  0.82 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 4096 | SingleElementMoved |   114,889.0 ns |    690.26 ns |   306.48 ns |  0.98 |    0.00 |    4 |         - |          NA |
| DotnetSort                        | 4096 | SingleElementMoved |    24,472.8 ns |    298.44 ns |   156.09 ns |  0.21 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**                       | **4096** | **Sorted**             |   **116,831.9 ns** |    **996.39 ns** |   **442.40 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| BitonicSortBranchless             | 4096 | Sorted             |   126,719.5 ns |  1,074.90 ns |   477.26 ns |  1.08 |    0.01 |    4 |         - |          NA |
| BitonicRecursiveSort              | 4096 | Sorted             |   466,659.9 ns |  2,410.09 ns | 1,070.10 ns |  3.99 |    0.02 |    6 |         - |          NA |
| BitonicSortAnyLength              | 4096 | Sorted             |   315,088.7 ns |  2,146.83 ns | 1,122.83 ns |  2.70 |    0.01 |    5 |         - |          NA |
| BitonicSortAnyLengthSimd          | 4096 | Sorted             |    36,926.4 ns |    254.68 ns |   113.08 ns |  0.32 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 4096 | Sorted             |   130,133.1 ns |  2,646.18 ns | 1,384.01 ns |  1.11 |    0.01 |    4 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 4096 | Sorted             |   116,205.7 ns |  1,675.67 ns |   744.01 ns |  0.99 |    0.01 |    4 |         - |          NA |
| PairwiseSortingNetwork            | 4096 | Sorted             |    95,130.4 ns |  2,920.84 ns | 1,527.66 ns |  0.81 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 4096 | Sorted             |   115,131.4 ns |    545.57 ns |   242.24 ns |  0.99 |    0.00 |    4 |         - |          NA |
| DotnetSort                        | 4096 | Sorted             |    15,176.1 ns |    561.34 ns |   293.59 ns |  0.13 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**                       | **4096** | **Reversed**           |   **120,097.0 ns** |  **1,908.55 ns** |   **998.21 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 4096 | Reversed           |   126,337.7 ns |    773.28 ns |   343.34 ns |  1.05 |    0.01 |    3 |         - |          NA |
| BitonicRecursiveSort              | 4096 | Reversed           |   464,087.3 ns |  2,719.58 ns | 1,207.51 ns |  3.86 |    0.03 |    5 |         - |          NA |
| BitonicSortAnyLength              | 4096 | Reversed           |   315,867.3 ns |  1,231.24 ns |   546.68 ns |  2.63 |    0.02 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 4096 | Reversed           |    37,185.6 ns |    142.21 ns |    74.38 ns |  0.31 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 4096 | Reversed           |   125,300.0 ns |  2,169.22 ns |   963.15 ns |  1.04 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 4096 | Reversed           |   116,326.8 ns |    989.29 ns |   439.25 ns |  0.97 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetwork            | 4096 | Reversed           |    96,181.1 ns |  5,747.37 ns | 2,551.87 ns |  0.80 |    0.02 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 4096 | Reversed           |   115,009.6 ns |    548.44 ns |   243.51 ns |  0.96 |    0.01 |    3 |         - |          NA |
| DotnetSort                        | 4096 | Reversed           |    27,495.8 ns |  3,430.09 ns | 1,794.01 ns |  0.23 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**                       | **4096** | **PipeOrgan**          |   **118,313.9 ns** |  **1,059.58 ns** |   **554.18 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 4096 | PipeOrgan          |   126,985.8 ns |    555.17 ns |   246.50 ns |  1.07 |    0.01 |    3 |         - |          NA |
| BitonicRecursiveSort              | 4096 | PipeOrgan          |   466,517.6 ns |  5,320.45 ns | 2,782.70 ns |  3.94 |    0.03 |    5 |         - |          NA |
| BitonicSortAnyLength              | 4096 | PipeOrgan          |   314,891.4 ns |  1,232.96 ns |   439.69 ns |  2.66 |    0.01 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 4096 | PipeOrgan          |    34,389.0 ns |    170.02 ns |    88.92 ns |  0.29 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 4096 | PipeOrgan          |   125,673.7 ns |  3,362.04 ns | 1,492.76 ns |  1.06 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 4096 | PipeOrgan          |   116,630.6 ns |  1,329.14 ns |   590.15 ns |  0.99 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetwork            | 4096 | PipeOrgan          |    97,519.3 ns |  6,483.82 ns | 3,391.16 ns |  0.82 |    0.03 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 4096 | PipeOrgan          |   114,834.3 ns |    259.49 ns |   115.21 ns |  0.97 |    0.00 |    3 |         - |          NA |
| DotnetSort                        | 4096 | PipeOrgan          |    72,384.6 ns |  3,799.03 ns | 1,986.96 ns |  0.61 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**                       | **4096** | **ManyDuplicates**     |   **253,850.2 ns** |    **591.05 ns** |   **262.43 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| BitonicSortBranchless             | 4096 | ManyDuplicates     |   126,741.2 ns |  1,017.68 ns |   362.92 ns |  0.50 |    0.00 |    3 |         - |          NA |
| BitonicRecursiveSort              | 4096 | ManyDuplicates     |   613,923.4 ns |  2,069.29 ns | 1,082.28 ns |  2.42 |    0.00 |    6 |         - |          NA |
| BitonicSortAnyLength              | 4096 | ManyDuplicates     |   476,512.8 ns |  1,471.00 ns |   653.13 ns |  1.88 |    0.00 |    5 |         - |          NA |
| BitonicSortAnyLengthSimd          | 4096 | ManyDuplicates     |    36,961.2 ns |    318.05 ns |   166.35 ns |  0.15 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 4096 | ManyDuplicates     |   231,451.6 ns |  1,924.39 ns |   854.44 ns |  0.91 |    0.00 |    4 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 4096 | ManyDuplicates     |   116,066.0 ns |  1,122.08 ns |   400.15 ns |  0.46 |    0.00 |    3 |         - |          NA |
| PairwiseSortingNetwork            | 4096 | ManyDuplicates     |   251,295.2 ns |  2,157.06 ns | 1,128.18 ns |  0.99 |    0.00 |    4 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 4096 | ManyDuplicates     |   115,205.7 ns |    624.59 ns |   277.32 ns |  0.45 |    0.00 |    3 |         - |          NA |
| DotnetSort                        | 4096 | ManyDuplicates     |    94,466.0 ns |    465.71 ns |   243.58 ns |  0.37 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**                       | **8192** | **Random**             |   **732,822.4 ns** |  **1,348.17 ns** |   **598.59 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 8192 | Random             |   293,639.9 ns |    893.20 ns |   396.59 ns |  0.40 |    0.00 |    2 |         - |          NA |
| BitonicRecursiveSort              | 8192 | Random             | 1,552,330.3 ns |  3,140.61 ns | 1,394.45 ns |  2.12 |    0.00 |    5 |         - |          NA |
| BitonicSortAnyLength              | 8192 | Random             | 1,255,178.2 ns |  7,357.20 ns | 3,847.96 ns |  1.71 |    0.01 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 8192 | Random             |    88,449.7 ns |    691.82 ns |   307.17 ns |  0.12 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 8192 | Random             |   615,382.2 ns |  1,414.74 ns |   739.94 ns |  0.84 |    0.00 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 8192 | Random             |   270,179.7 ns |  2,062.28 ns | 1,078.61 ns |  0.37 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 8192 | Random             |   791,842.4 ns |  1,636.48 ns |   855.91 ns |  1.08 |    0.00 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 8192 | Random             |   268,657.5 ns |  2,377.65 ns | 1,243.56 ns |  0.37 |    0.00 |    2 |         - |          NA |
| DotnetSort                        | 8192 | Random             |   315,340.5 ns |  1,706.91 ns |   892.75 ns |  0.43 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**                       | **8192** | **SingleElementMoved** |   **270,101.7 ns** |  **2,635.38 ns** | **1,170.12 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| BitonicSortBranchless             | 8192 | SingleElementMoved |   293,521.2 ns |  1,417.68 ns |   741.47 ns |  1.09 |    0.01 |    4 |         - |          NA |
| BitonicRecursiveSort              | 8192 | SingleElementMoved | 1,049,269.5 ns |  2,599.09 ns | 1,154.01 ns |  3.88 |    0.02 |    6 |         - |          NA |
| BitonicSortAnyLength              | 8192 | SingleElementMoved |   709,720.9 ns |  2,451.32 ns | 1,282.09 ns |  2.63 |    0.01 |    5 |         - |          NA |
| BitonicSortAnyLengthSimd          | 8192 | SingleElementMoved |    85,733.5 ns |    618.99 ns |   274.84 ns |  0.32 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 8192 | SingleElementMoved |   296,719.0 ns |  3,446.33 ns | 1,530.19 ns |  1.10 |    0.01 |    4 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 8192 | SingleElementMoved |   271,908.0 ns |  3,207.00 ns | 1,677.32 ns |  1.01 |    0.01 |    4 |         - |          NA |
| PairwiseSortingNetwork            | 8192 | SingleElementMoved |   221,123.6 ns |  9,631.60 ns | 4,276.49 ns |  0.82 |    0.02 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 8192 | SingleElementMoved |   268,116.0 ns |    579.23 ns |   257.18 ns |  0.99 |    0.00 |    4 |         - |          NA |
| DotnetSort                        | 8192 | SingleElementMoved |    54,268.2 ns |  3,352.69 ns | 1,753.52 ns |  0.20 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**                       | **8192** | **Sorted**             |   **270,337.4 ns** |  **3,075.08 ns** | **1,608.33 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 8192 | Sorted             |   293,379.1 ns |    918.82 ns |   327.66 ns |  1.09 |    0.01 |    3 |         - |          NA |
| BitonicRecursiveSort              | 8192 | Sorted             | 1,048,498.6 ns |  2,008.71 ns |   891.88 ns |  3.88 |    0.02 |    5 |         - |          NA |
| BitonicSortAnyLength              | 8192 | Sorted             |   708,952.8 ns |  1,832.19 ns |   958.27 ns |  2.62 |    0.02 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 8192 | Sorted             |    85,494.7 ns |    706.35 ns |   251.89 ns |  0.32 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 8192 | Sorted             |   300,976.6 ns |  1,926.28 ns |   855.28 ns |  1.11 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 8192 | Sorted             |   272,083.9 ns |  8,283.75 ns | 3,678.04 ns |  1.01 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetwork            | 8192 | Sorted             |   223,595.8 ns |  3,808.30 ns | 1,991.81 ns |  0.83 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 8192 | Sorted             |   268,067.3 ns |  1,681.84 ns |   879.63 ns |  0.99 |    0.01 |    3 |         - |          NA |
| DotnetSort                        | 8192 | Sorted             |    32,436.3 ns |    602.16 ns |   267.36 ns |  0.12 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**                       | **8192** | **Reversed**           |   **276,772.5 ns** |  **2,631.79 ns** | **1,376.48 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 8192 | Reversed           |   294,345.0 ns |  2,431.76 ns | 1,271.86 ns |  1.06 |    0.01 |    3 |         - |          NA |
| BitonicRecursiveSort              | 8192 | Reversed           | 1,044,807.2 ns |  3,529.37 ns | 1,567.06 ns |  3.78 |    0.02 |    5 |         - |          NA |
| BitonicSortAnyLength              | 8192 | Reversed           |   709,590.8 ns |  2,075.51 ns |   921.54 ns |  2.56 |    0.01 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 8192 | Reversed           |    86,154.0 ns |    629.22 ns |   279.38 ns |  0.31 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 8192 | Reversed           |   289,556.7 ns |  3,119.82 ns | 1,631.73 ns |  1.05 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 8192 | Reversed           |   270,347.5 ns |  2,667.92 ns | 1,184.57 ns |  0.98 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetwork            | 8192 | Reversed           |   227,769.2 ns |  4,790.79 ns | 2,505.68 ns |  0.82 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 8192 | Reversed           |   267,450.2 ns |    782.86 ns |   347.60 ns |  0.97 |    0.00 |    3 |         - |          NA |
| DotnetSort                        | 8192 | Reversed           |    59,717.4 ns |  4,034.07 ns | 2,109.90 ns |  0.22 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**                       | **8192** | **PipeOrgan**          |   **273,370.7 ns** |  **1,962.48 ns** | **1,026.42 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 8192 | PipeOrgan          |   294,004.2 ns |  1,573.90 ns |   823.18 ns |  1.08 |    0.00 |    3 |         - |          NA |
| BitonicRecursiveSort              | 8192 | PipeOrgan          | 1,047,200.6 ns |  3,290.91 ns | 1,721.21 ns |  3.83 |    0.01 |    5 |         - |          NA |
| BitonicSortAnyLength              | 8192 | PipeOrgan          |   708,767.7 ns |    957.49 ns |   500.79 ns |  2.59 |    0.01 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 8192 | PipeOrgan          |    79,118.6 ns |    428.87 ns |   152.94 ns |  0.29 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 8192 | PipeOrgan          |   288,732.3 ns |  4,989.10 ns | 2,609.40 ns |  1.06 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 8192 | PipeOrgan          |   269,626.9 ns |  1,958.71 ns |   869.68 ns |  0.99 |    0.00 |    3 |         - |          NA |
| PairwiseSortingNetwork            | 8192 | PipeOrgan          |   220,396.4 ns |  1,840.64 ns |   817.25 ns |  0.81 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 8192 | PipeOrgan          |   267,382.4 ns |    767.10 ns |   340.60 ns |  0.98 |    0.00 |    3 |         - |          NA |
| DotnetSort                        | 8192 | PipeOrgan          |   278,538.0 ns |  3,062.53 ns | 1,601.76 ns |  1.02 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**                       | **8192** | **ManyDuplicates**     |   **542,667.2 ns** |  **1,388.33 ns** |   **495.09 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| BitonicSortBranchless             | 8192 | ManyDuplicates     |   293,665.7 ns |    609.95 ns |   270.82 ns |  0.54 |    0.00 |    3 |         - |          NA |
| BitonicRecursiveSort              | 8192 | ManyDuplicates     | 1,348,613.5 ns |  2,313.24 ns | 1,209.87 ns |  2.49 |    0.00 |    6 |         - |          NA |
| BitonicSortAnyLength              | 8192 | ManyDuplicates     | 1,039,898.2 ns |  4,812.88 ns | 2,517.23 ns |  1.92 |    0.00 |    5 |         - |          NA |
| BitonicSortAnyLengthSimd          | 8192 | ManyDuplicates     |    87,359.3 ns |    188.36 ns |    83.63 ns |  0.16 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 8192 | ManyDuplicates     |   502,722.6 ns |  3,347.31 ns | 1,486.23 ns |  0.93 |    0.00 |    4 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 8192 | ManyDuplicates     |   269,755.7 ns |  2,536.82 ns | 1,326.81 ns |  0.50 |    0.00 |    3 |         - |          NA |
| PairwiseSortingNetwork            | 8192 | ManyDuplicates     |   538,482.7 ns |  6,258.95 ns | 2,779.01 ns |  0.99 |    0.00 |    4 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 8192 | ManyDuplicates     |   267,808.5 ns |  2,565.60 ns | 1,139.14 ns |  0.49 |    0.00 |    3 |         - |          NA |
| DotnetSort                        | 8192 | ManyDuplicates     |   190,186.9 ns |    955.36 ns |   424.19 ns |  0.35 |    0.00 |    2 |         - |          NA |

### PartitionBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.49 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                       | Size | Pattern            | Mean           | Error        | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------------- |----- |------------------- |---------------:|-------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **QuickSort**                    | **256**  | **Random**             |     **8,660.5 ns** |    **163.19 ns** |    **85.35 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 256  | Random             |     9,373.9 ns |    274.34 ns |   143.48 ns |  1.08 |    0.02 |    4 |         - |          NA |
| QuickSortMedian3             | 256  | Random             |     9,102.3 ns |    321.69 ns |   168.25 ns |  1.05 |    0.02 |    4 |         - |          NA |
| QuickSortMedian9             | 256  | Random             |     9,683.3 ns |    373.21 ns |   195.19 ns |  1.12 |    0.02 |    4 |         - |          NA |
| SampleSort                   | 256  | Random             |    10,955.7 ns |    250.36 ns |   130.94 ns |  1.27 |    0.02 |    4 |         - |          NA |
| SimdQuickSort                | 256  | Random             |       919.4 ns |     24.16 ns |    10.73 ns |  0.11 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort           | 256  | Random             |     5,879.4 ns |    215.71 ns |   112.82 ns |  0.68 |    0.01 |    3 |         - |          NA |
| StableQuickSort              | 256  | Random             |    19,856.7 ns |    453.18 ns |   201.21 ns |  2.29 |    0.03 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Random             |    11,610.2 ns |    202.92 ns |   106.13 ns |  1.34 |    0.02 |    4 |         - |          NA |
| DestswapStableQuickSort      | 256  | Random             |    19,380.5 ns |    357.73 ns |   187.10 ns |  2.24 |    0.03 |    5 |         - |          NA |
| IntroSort                    | 256  | Random             |     7,146.4 ns |    100.54 ns |    44.64 ns |  0.83 |    0.01 |    3 |         - |          NA |
| IntroSortDotnet              | 256  | Random             |     6,353.3 ns |    143.22 ns |    74.91 ns |  0.73 |    0.01 |    3 |         - |          NA |
| PDQSort                      | 256  | Random             |     6,021.8 ns |    313.97 ns |   164.21 ns |  0.70 |    0.02 |    3 |         - |          NA |
| PDQSortBranchless            | 256  | Random             |     4,354.6 ns |    411.99 ns |   215.48 ns |  0.50 |    0.02 |    2 |         - |          NA |
| Ipnsort                      | 256  | Random             |     3,817.4 ns |    360.74 ns |   188.67 ns |  0.44 |    0.02 |    2 |         - |          NA |
| StdSort                      | 256  | Random             |     4,440.8 ns |    200.51 ns |   104.87 ns |  0.51 |    0.01 |    2 |         - |          NA |
| BlockQuickSort               | 256  | Random             |     4,690.8 ns |    202.31 ns |   105.81 ns |  0.54 |    0.01 |    2 |         - |          NA |
| Ips4oSort                    | 256  | Random             |    10,722.2 ns |    204.27 ns |   106.84 ns |  1.24 |    0.02 |    4 |         - |          NA |
| FluxSort                     | 256  | Random             |     6,087.0 ns |    369.61 ns |   193.32 ns |  0.70 |    0.02 |    3 |         - |          NA |
| CrumSort                     | 256  | Random             |     3,638.8 ns |    546.24 ns |   285.69 ns |  0.42 |    0.03 |    2 |         - |          NA |
| BlitSort                     | 256  | Random             |     6,632.8 ns |  1,478.68 ns |   773.38 ns |  0.77 |    0.08 |    3 |         - |          NA |
| DotnetSort                   | 256  | Random             |     5,966.9 ns |    327.23 ns |   171.15 ns |  0.69 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **256**  | **SingleElementMoved** |     **1,128.4 ns** |     **91.41 ns** |    **40.59 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 256  | SingleElementMoved |     2,185.2 ns |    366.68 ns |   191.78 ns |  1.94 |    0.17 |    4 |         - |          NA |
| QuickSortMedian3             | 256  | SingleElementMoved |     1,652.1 ns |    327.03 ns |   145.20 ns |  1.47 |    0.13 |    4 |         - |          NA |
| QuickSortMedian9             | 256  | SingleElementMoved |     1,774.8 ns |    131.57 ns |    68.81 ns |  1.57 |    0.08 |    4 |         - |          NA |
| SampleSort                   | 256  | SingleElementMoved |     5,682.0 ns |    369.36 ns |   193.18 ns |  5.04 |    0.23 |    6 |         - |          NA |
| SimdQuickSort                | 256  | SingleElementMoved |       656.5 ns |      5.81 ns |     2.07 ns |  0.58 |    0.02 |    2 |         - |          NA |
| DualPivotQuickSort           | 256  | SingleElementMoved |       827.0 ns |     50.02 ns |    26.16 ns |  0.73 |    0.03 |    3 |         - |          NA |
| StableQuickSort              | 256  | SingleElementMoved |     6,186.0 ns |    205.52 ns |   107.49 ns |  5.49 |    0.20 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 256  | SingleElementMoved |     3,719.6 ns |    294.40 ns |   153.98 ns |  3.30 |    0.17 |    5 |         - |          NA |
| DestswapStableQuickSort      | 256  | SingleElementMoved |     7,686.9 ns |    189.51 ns |    99.12 ns |  6.82 |    0.24 |    7 |         - |          NA |
| IntroSort                    | 256  | SingleElementMoved |     2,070.0 ns |     44.81 ns |    15.98 ns |  1.84 |    0.06 |    4 |         - |          NA |
| IntroSortDotnet              | 256  | SingleElementMoved |     1,090.6 ns |     95.71 ns |    42.49 ns |  0.97 |    0.05 |    3 |         - |          NA |
| PDQSort                      | 256  | SingleElementMoved |       857.8 ns |     59.67 ns |    26.49 ns |  0.76 |    0.03 |    3 |         - |          NA |
| PDQSortBranchless            | 256  | SingleElementMoved |     1,036.5 ns |     93.48 ns |    41.51 ns |  0.92 |    0.05 |    3 |         - |          NA |
| Ipnsort                      | 256  | SingleElementMoved |     2,902.7 ns |     83.14 ns |    29.65 ns |  2.58 |    0.09 |    4 |         - |          NA |
| StdSort                      | 256  | SingleElementMoved |       992.6 ns |    104.65 ns |    54.73 ns |  0.88 |    0.05 |    3 |         - |          NA |
| BlockQuickSort               | 256  | SingleElementMoved |     1,577.9 ns |    131.30 ns |    58.30 ns |  1.40 |    0.07 |    4 |         - |          NA |
| Ips4oSort                    | 256  | SingleElementMoved |       454.5 ns |     11.24 ns |     5.88 ns |  0.40 |    0.01 |    1 |         - |          NA |
| FluxSort                     | 256  | SingleElementMoved |     1,249.6 ns |     98.79 ns |    51.67 ns |  1.11 |    0.06 |    3 |         - |          NA |
| CrumSort                     | 256  | SingleElementMoved |       529.6 ns |     76.81 ns |    34.10 ns |  0.47 |    0.03 |    1 |         - |          NA |
| BlitSort                     | 256  | SingleElementMoved |     2,426.5 ns |     47.90 ns |    25.05 ns |  2.15 |    0.07 |    4 |         - |          NA |
| DotnetSort                   | 256  | SingleElementMoved |     1,089.5 ns |     85.13 ns |    44.53 ns |  0.97 |    0.05 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Sorted**             |       **694.5 ns** |     **19.84 ns** |    **10.38 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 256  | Sorted             |     1,411.5 ns |     13.11 ns |     4.68 ns |  2.03 |    0.03 |    4 |         - |          NA |
| QuickSortMedian3             | 256  | Sorted             |       944.7 ns |     13.92 ns |     6.18 ns |  1.36 |    0.02 |    3 |         - |          NA |
| QuickSortMedian9             | 256  | Sorted             |     1,013.2 ns |     27.21 ns |    14.23 ns |  1.46 |    0.03 |    3 |         - |          NA |
| SampleSort                   | 256  | Sorted             |     3,649.6 ns |    347.02 ns |   181.50 ns |  5.26 |    0.26 |    5 |         - |          NA |
| SimdQuickSort                | 256  | Sorted             |       925.2 ns |      9.91 ns |     4.40 ns |  1.33 |    0.02 |    3 |         - |          NA |
| DualPivotQuickSort           | 256  | Sorted             |       581.4 ns |      9.29 ns |     4.86 ns |  0.84 |    0.01 |    3 |         - |          NA |
| StableQuickSort              | 256  | Sorted             |     6,014.5 ns |    301.50 ns |   157.69 ns |  8.66 |    0.25 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Sorted             |     3,442.7 ns |     24.99 ns |     8.91 ns |  4.96 |    0.07 |    5 |         - |          NA |
| DestswapStableQuickSort      | 256  | Sorted             |     7,366.4 ns |    378.26 ns |   167.95 ns | 10.61 |    0.27 |    7 |         - |          NA |
| IntroSort                    | 256  | Sorted             |       629.0 ns |     37.00 ns |    13.19 ns |  0.91 |    0.02 |    3 |         - |          NA |
| IntroSortDotnet              | 256  | Sorted             |       775.6 ns |      3.83 ns |     1.70 ns |  1.12 |    0.02 |    3 |         - |          NA |
| PDQSort                      | 256  | Sorted             |       328.5 ns |     14.08 ns |     6.25 ns |  0.47 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 256  | Sorted             |       314.1 ns |     20.63 ns |    10.79 ns |  0.45 |    0.02 |    2 |         - |          NA |
| Ipnsort                      | 256  | Sorted             |       148.9 ns |     16.92 ns |     6.04 ns |  0.21 |    0.01 |    1 |         - |          NA |
| StdSort                      | 256  | Sorted             |       352.9 ns |     30.00 ns |    15.69 ns |  0.51 |    0.02 |    2 |         - |          NA |
| BlockQuickSort               | 256  | Sorted             |     1,246.9 ns |     14.89 ns |     5.31 ns |  1.80 |    0.03 |    4 |         - |          NA |
| Ips4oSort                    | 256  | Sorted             |       146.3 ns |      5.19 ns |     2.31 ns |  0.21 |    0.00 |    1 |         - |          NA |
| FluxSort                     | 256  | Sorted             |       203.4 ns |     23.71 ns |    10.53 ns |  0.29 |    0.01 |    2 |         - |          NA |
| CrumSort                     | 256  | Sorted             |       271.7 ns |      8.48 ns |     4.44 ns |  0.39 |    0.01 |    2 |         - |          NA |
| BlitSort                     | 256  | Sorted             |       235.2 ns |     17.10 ns |     8.94 ns |  0.34 |    0.01 |    2 |         - |          NA |
| DotnetSort                   | 256  | Sorted             |       628.9 ns |      5.16 ns |     2.29 ns |  0.91 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Reversed**           |       **824.3 ns** |     **33.83 ns** |    **17.69 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 256  | Reversed           |     1,603.8 ns |     20.18 ns |    10.56 ns |  1.95 |    0.04 |    6 |         - |          NA |
| QuickSortMedian3             | 256  | Reversed           |       996.9 ns |     17.16 ns |     8.97 ns |  1.21 |    0.03 |    5 |         - |          NA |
| QuickSortMedian9             | 256  | Reversed           |     1,113.4 ns |     49.87 ns |    22.14 ns |  1.35 |    0.04 |    5 |         - |          NA |
| SampleSort                   | 256  | Reversed           |     3,965.3 ns |    387.95 ns |   202.91 ns |  4.81 |    0.25 |    7 |         - |          NA |
| SimdQuickSort                | 256  | Reversed           |       659.3 ns |      9.26 ns |     4.84 ns |  0.80 |    0.02 |    4 |         - |          NA |
| DualPivotQuickSort           | 256  | Reversed           |       751.7 ns |     12.21 ns |     5.42 ns |  0.91 |    0.02 |    4 |         - |          NA |
| StableQuickSort              | 256  | Reversed           |     6,167.9 ns |    241.53 ns |   126.33 ns |  7.49 |    0.21 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Reversed           |     3,447.9 ns |    148.37 ns |    65.88 ns |  4.18 |    0.11 |    7 |         - |          NA |
| DestswapStableQuickSort      | 256  | Reversed           |     6,829.5 ns |     48.91 ns |    21.72 ns |  8.29 |    0.17 |    8 |         - |          NA |
| IntroSort                    | 256  | Reversed           |       681.1 ns |     15.86 ns |     7.04 ns |  0.83 |    0.02 |    4 |         - |          NA |
| IntroSortDotnet              | 256  | Reversed           |     1,097.0 ns |     15.77 ns |     8.25 ns |  1.33 |    0.03 |    5 |         - |          NA |
| PDQSort                      | 256  | Reversed           |       427.3 ns |      8.34 ns |     3.70 ns |  0.52 |    0.01 |    3 |         - |          NA |
| PDQSortBranchless            | 256  | Reversed           |       573.0 ns |     11.25 ns |     4.99 ns |  0.70 |    0.02 |    4 |         - |          NA |
| Ipnsort                      | 256  | Reversed           |       188.2 ns |      3.12 ns |     1.63 ns |  0.23 |    0.00 |    1 |         - |          NA |
| StdSort                      | 256  | Reversed           |       661.5 ns |     10.59 ns |     4.70 ns |  0.80 |    0.02 |    4 |         - |          NA |
| BlockQuickSort               | 256  | Reversed           |     1,668.8 ns |     24.62 ns |     8.78 ns |  2.03 |    0.04 |    6 |         - |          NA |
| Ips4oSort                    | 256  | Reversed           |       218.4 ns |    145.63 ns |    64.66 ns |  0.26 |    0.07 |    1 |         - |          NA |
| FluxSort                     | 256  | Reversed           |       280.0 ns |      8.95 ns |     4.68 ns |  0.34 |    0.01 |    2 |         - |          NA |
| CrumSort                     | 256  | Reversed           |       243.9 ns |      4.71 ns |     2.09 ns |  0.30 |    0.01 |    2 |         - |          NA |
| BlitSort                     | 256  | Reversed           |       294.9 ns |     10.66 ns |     4.73 ns |  0.36 |    0.01 |    2 |         - |          NA |
| DotnetSort                   | 256  | Reversed           |     1,117.7 ns |    134.09 ns |    70.13 ns |  1.36 |    0.08 |    5 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **256**  | **PipeOrgan**          |     **5,884.1 ns** |    **452.08 ns** |   **236.45 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 256  | PipeOrgan          |     2,911.6 ns |    322.87 ns |   168.87 ns |  0.50 |    0.03 |    3 |         - |          NA |
| QuickSortMedian3             | 256  | PipeOrgan          |     2,451.3 ns |    133.55 ns |    59.30 ns |  0.42 |    0.02 |    3 |         - |          NA |
| QuickSortMedian9             | 256  | PipeOrgan          |     1,997.6 ns |     96.14 ns |    50.28 ns |  0.34 |    0.01 |    3 |         - |          NA |
| SampleSort                   | 256  | PipeOrgan          |     3,788.1 ns |    249.22 ns |   110.65 ns |  0.64 |    0.03 |    3 |         - |          NA |
| SimdQuickSort                | 256  | PipeOrgan          |       669.4 ns |      8.72 ns |     3.87 ns |  0.11 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort           | 256  | PipeOrgan          |     1,610.6 ns |    188.28 ns |    83.60 ns |  0.27 |    0.02 |    3 |         - |          NA |
| StableQuickSort              | 256  | PipeOrgan          |     5,888.7 ns |     52.76 ns |    23.42 ns |  1.00 |    0.04 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 256  | PipeOrgan          |     3,448.5 ns |     16.68 ns |     7.41 ns |  0.59 |    0.02 |    3 |         - |          NA |
| DestswapStableQuickSort      | 256  | PipeOrgan          |     8,954.3 ns |    267.87 ns |   140.10 ns |  1.52 |    0.06 |    5 |         - |          NA |
| IntroSort                    | 256  | PipeOrgan          |     3,115.5 ns |     36.18 ns |    12.90 ns |  0.53 |    0.02 |    3 |         - |          NA |
| IntroSortDotnet              | 256  | PipeOrgan          |     1,673.7 ns |     35.49 ns |    15.76 ns |  0.28 |    0.01 |    3 |         - |          NA |
| PDQSort                      | 256  | PipeOrgan          |     1,233.1 ns |     43.88 ns |    19.48 ns |  0.21 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 256  | PipeOrgan          |     2,040.9 ns |     46.06 ns |    24.09 ns |  0.35 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 256  | PipeOrgan          |     3,181.6 ns |    281.80 ns |   147.39 ns |  0.54 |    0.03 |    3 |         - |          NA |
| StdSort                      | 256  | PipeOrgan          |     3,762.0 ns |    215.37 ns |   112.64 ns |  0.64 |    0.03 |    3 |         - |          NA |
| BlockQuickSort               | 256  | PipeOrgan          |     3,728.4 ns |    298.79 ns |   132.67 ns |  0.63 |    0.03 |    3 |         - |          NA |
| Ips4oSort                    | 256  | PipeOrgan          |     9,862.9 ns |    316.67 ns |   140.60 ns |  1.68 |    0.07 |    5 |         - |          NA |
| FluxSort                     | 256  | PipeOrgan          |     3,021.6 ns |    110.10 ns |    48.88 ns |  0.51 |    0.02 |    3 |         - |          NA |
| CrumSort                     | 256  | PipeOrgan          |       693.6 ns |     13.81 ns |     6.13 ns |  0.12 |    0.00 |    1 |         - |          NA |
| BlitSort                     | 256  | PipeOrgan          |     3,133.8 ns |     24.36 ns |     8.69 ns |  0.53 |    0.02 |    3 |         - |          NA |
| DotnetSort                   | 256  | PipeOrgan          |     1,914.6 ns |    228.78 ns |   119.66 ns |  0.33 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **256**  | **ManyDuplicates**     |     **6,662.9 ns** |    **225.41 ns** |   **100.08 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 256  | ManyDuplicates     |     5,873.2 ns |    118.43 ns |    52.58 ns |  0.88 |    0.01 |    2 |         - |          NA |
| QuickSortMedian3             | 256  | ManyDuplicates     |     7,768.6 ns |    385.25 ns |   201.49 ns |  1.17 |    0.03 |    2 |         - |          NA |
| QuickSortMedian9             | 256  | ManyDuplicates     |     8,329.9 ns |    300.50 ns |   157.17 ns |  1.25 |    0.03 |    2 |         - |          NA |
| SampleSort                   | 256  | ManyDuplicates     |     9,737.2 ns |    292.41 ns |   152.94 ns |  1.46 |    0.03 |    2 |         - |          NA |
| SimdQuickSort                | 256  | ManyDuplicates     |       656.5 ns |      5.01 ns |     2.22 ns |  0.10 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort           | 256  | ManyDuplicates     |     5,190.1 ns |    222.01 ns |   116.11 ns |  0.78 |    0.02 |    2 |         - |          NA |
| StableQuickSort              | 256  | ManyDuplicates     |    12,717.6 ns |    440.70 ns |   230.49 ns |  1.91 |    0.04 |    2 |         - |          NA |
| BidirectionalStableQuickSort | 256  | ManyDuplicates     |     6,216.9 ns |     77.44 ns |    34.38 ns |  0.93 |    0.01 |    2 |         - |          NA |
| DestswapStableQuickSort      | 256  | ManyDuplicates     |    10,943.4 ns |    321.23 ns |   168.01 ns |  1.64 |    0.03 |    2 |         - |          NA |
| IntroSort                    | 256  | ManyDuplicates     |     6,383.0 ns |    240.78 ns |   106.91 ns |  0.96 |    0.02 |    2 |         - |          NA |
| IntroSortDotnet              | 256  | ManyDuplicates     |     5,947.3 ns |     43.99 ns |    15.69 ns |  0.89 |    0.01 |    2 |         - |          NA |
| PDQSort                      | 256  | ManyDuplicates     |     5,641.5 ns |    476.87 ns |   249.41 ns |  0.85 |    0.04 |    2 |         - |          NA |
| PDQSortBranchless            | 256  | ManyDuplicates     |     3,832.0 ns |    292.27 ns |   152.86 ns |  0.58 |    0.02 |    2 |         - |          NA |
| Ipnsort                      | 256  | ManyDuplicates     |     3,786.1 ns |    389.49 ns |   203.71 ns |  0.57 |    0.03 |    2 |         - |          NA |
| StdSort                      | 256  | ManyDuplicates     |     4,130.8 ns |    312.32 ns |   163.35 ns |  0.62 |    0.02 |    2 |         - |          NA |
| BlockQuickSort               | 256  | ManyDuplicates     |     4,395.8 ns |    347.92 ns |   181.97 ns |  0.66 |    0.03 |    2 |         - |          NA |
| Ips4oSort                    | 256  | ManyDuplicates     |    10,580.6 ns |    280.20 ns |   146.55 ns |  1.59 |    0.03 |    2 |         - |          NA |
| FluxSort                     | 256  | ManyDuplicates     |     5,974.6 ns |    148.68 ns |    77.76 ns |  0.90 |    0.02 |    2 |         - |          NA |
| CrumSort                     | 256  | ManyDuplicates     |     3,561.1 ns |    305.21 ns |   159.63 ns |  0.53 |    0.02 |    2 |         - |          NA |
| BlitSort                     | 256  | ManyDuplicates     |     6,189.9 ns |    179.98 ns |    94.14 ns |  0.93 |    0.02 |    2 |         - |          NA |
| DotnetSort                   | 256  | ManyDuplicates     |     5,603.0 ns |    261.25 ns |   136.64 ns |  0.84 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Random**             |    **40,806.2 ns** |  **1,016.80 ns** |   **531.81 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Random             |    44,198.4 ns |    424.61 ns |   222.08 ns |  1.08 |    0.01 |    2 |         - |          NA |
| QuickSortMedian3             | 1024 | Random             |    42,641.2 ns |    417.12 ns |   185.20 ns |  1.05 |    0.01 |    2 |         - |          NA |
| QuickSortMedian9             | 1024 | Random             |    45,190.6 ns |    851.45 ns |   445.33 ns |  1.11 |    0.02 |    2 |         - |          NA |
| SampleSort                   | 1024 | Random             |    51,690.6 ns |    385.54 ns |   201.64 ns |  1.27 |    0.02 |    2 |         - |          NA |
| SimdQuickSort                | 1024 | Random             |     3,895.1 ns |    452.74 ns |   236.79 ns |  0.10 |    0.01 |    1 |         - |          NA |
| DualPivotQuickSort           | 1024 | Random             |    29,994.7 ns |    561.36 ns |   293.60 ns |  0.74 |    0.01 |    2 |         - |          NA |
| StableQuickSort              | 1024 | Random             |    98,420.3 ns |    430.55 ns |   153.54 ns |  2.41 |    0.03 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Random             |    53,612.7 ns |    553.01 ns |   245.54 ns |  1.31 |    0.02 |    2 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Random             |    86,948.5 ns |    300.18 ns |   133.28 ns |  2.13 |    0.03 |    3 |         - |          NA |
| IntroSort                    | 1024 | Random             |    34,101.9 ns |    213.43 ns |    94.76 ns |  0.84 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 1024 | Random             |    32,433.9 ns |    235.33 ns |   123.08 ns |  0.79 |    0.01 |    2 |         - |          NA |
| PDQSort                      | 1024 | Random             |    30,771.9 ns |    235.66 ns |   123.25 ns |  0.75 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | Random             |    19,415.4 ns |     60.79 ns |    21.68 ns |  0.48 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 1024 | Random             |    19,142.6 ns |    241.36 ns |   126.24 ns |  0.47 |    0.01 |    2 |         - |          NA |
| StdSort                      | 1024 | Random             |    20,425.3 ns |    149.61 ns |    78.25 ns |  0.50 |    0.01 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | Random             |    21,811.0 ns |    313.28 ns |   139.10 ns |  0.53 |    0.01 |    2 |         - |          NA |
| Ips4oSort                    | 1024 | Random             |    18,898.6 ns |    193.28 ns |   101.09 ns |  0.46 |    0.01 |    2 |         - |          NA |
| FluxSort                     | 1024 | Random             |    25,619.7 ns |    241.93 ns |   107.42 ns |  0.63 |    0.01 |    2 |         - |          NA |
| CrumSort                     | 1024 | Random             |    25,641.6 ns |    364.16 ns |   190.46 ns |  0.63 |    0.01 |    2 |         - |          NA |
| BlitSort                     | 1024 | Random             |    26,497.4 ns |    306.43 ns |   160.27 ns |  0.65 |    0.01 |    2 |         - |          NA |
| DotnetSort                   | 1024 | Random             |    29,841.9 ns |    237.91 ns |   124.43 ns |  0.73 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **SingleElementMoved** |     **4,370.3 ns** |    **100.68 ns** |    **52.66 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 1024 | SingleElementMoved |     8,234.6 ns |    578.06 ns |   256.66 ns |  1.88 |    0.06 |    3 |         - |          NA |
| QuickSortMedian3             | 1024 | SingleElementMoved |     6,545.1 ns |    325.37 ns |   170.17 ns |  1.50 |    0.04 |    3 |         - |          NA |
| QuickSortMedian9             | 1024 | SingleElementMoved |     7,394.2 ns |    239.16 ns |   125.09 ns |  1.69 |    0.03 |    3 |         - |          NA |
| SampleSort                   | 1024 | SingleElementMoved |    25,963.1 ns |    952.72 ns |   498.29 ns |  5.94 |    0.13 |    7 |         - |          NA |
| SimdQuickSort                | 1024 | SingleElementMoved |     4,523.1 ns |     25.97 ns |    11.53 ns |  1.04 |    0.01 |    2 |         - |          NA |
| DualPivotQuickSort           | 1024 | SingleElementMoved |     3,899.6 ns |    279.43 ns |   146.15 ns |  0.89 |    0.03 |    2 |         - |          NA |
| StableQuickSort              | 1024 | SingleElementMoved |    31,567.7 ns |  1,403.04 ns |   622.96 ns |  7.22 |    0.16 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | SingleElementMoved |    16,073.0 ns |    205.87 ns |   107.67 ns |  3.68 |    0.05 |    5 |         - |          NA |
| DestswapStableQuickSort      | 1024 | SingleElementMoved |    32,690.0 ns |    746.83 ns |   390.61 ns |  7.48 |    0.12 |    8 |         - |          NA |
| IntroSort                    | 1024 | SingleElementMoved |    11,164.2 ns |    267.30 ns |   139.80 ns |  2.55 |    0.04 |    4 |         - |          NA |
| IntroSortDotnet              | 1024 | SingleElementMoved |     4,766.4 ns |     99.94 ns |    44.37 ns |  1.09 |    0.02 |    2 |         - |          NA |
| PDQSort                      | 1024 | SingleElementMoved |     3,098.5 ns |    163.44 ns |    72.57 ns |  0.71 |    0.02 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | SingleElementMoved |     3,941.6 ns |    234.75 ns |   104.23 ns |  0.90 |    0.02 |    2 |         - |          NA |
| Ipnsort                      | 1024 | SingleElementMoved |    18,890.4 ns |  4,180.28 ns | 2,186.37 ns |  4.32 |    0.47 |    6 |         - |          NA |
| StdSort                      | 1024 | SingleElementMoved |     3,570.5 ns |    361.10 ns |   188.86 ns |  0.82 |    0.04 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | SingleElementMoved |     7,355.4 ns |    292.12 ns |   152.78 ns |  1.68 |    0.04 |    3 |         - |          NA |
| Ips4oSort                    | 1024 | SingleElementMoved |    11,093.3 ns |    235.86 ns |   123.36 ns |  2.54 |    0.04 |    4 |         - |          NA |
| FluxSort                     | 1024 | SingleElementMoved |     1,384.3 ns |     71.91 ns |    37.61 ns |  0.32 |    0.01 |    1 |         - |          NA |
| CrumSort                     | 1024 | SingleElementMoved |     1,324.1 ns |     65.36 ns |    34.19 ns |  0.30 |    0.01 |    1 |         - |          NA |
| BlitSort                     | 1024 | SingleElementMoved |     1,314.7 ns |     34.66 ns |    12.36 ns |  0.30 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 1024 | SingleElementMoved |     5,221.4 ns |     81.73 ns |    36.29 ns |  1.19 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Sorted**             |     **3,303.2 ns** |     **13.82 ns** |     **6.14 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Sorted             |     6,112.9 ns |     35.58 ns |    15.80 ns |  1.85 |    0.01 |    5 |         - |          NA |
| QuickSortMedian3             | 1024 | Sorted             |     4,551.5 ns |    173.12 ns |    90.55 ns |  1.38 |    0.03 |    4 |         - |          NA |
| QuickSortMedian9             | 1024 | Sorted             |     4,598.1 ns |     37.84 ns |    19.79 ns |  1.39 |    0.01 |    4 |         - |          NA |
| SampleSort                   | 1024 | Sorted             |    15,554.5 ns |    656.91 ns |   343.57 ns |  4.71 |    0.10 |    6 |         - |          NA |
| SimdQuickSort                | 1024 | Sorted             |     4,284.1 ns |     30.85 ns |    13.70 ns |  1.30 |    0.00 |    4 |         - |          NA |
| DualPivotQuickSort           | 1024 | Sorted             |     3,174.2 ns |     17.14 ns |     8.96 ns |  0.96 |    0.00 |    4 |         - |          NA |
| StableQuickSort              | 1024 | Sorted             |    30,507.4 ns |    400.10 ns |   177.65 ns |  9.24 |    0.05 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Sorted             |    15,799.8 ns |     86.16 ns |    38.26 ns |  4.78 |    0.01 |    6 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Sorted             |    32,896.2 ns |    380.79 ns |   199.16 ns |  9.96 |    0.06 |    7 |         - |          NA |
| IntroSort                    | 1024 | Sorted             |     3,137.6 ns |     15.81 ns |     8.27 ns |  0.95 |    0.00 |    4 |         - |          NA |
| IntroSortDotnet              | 1024 | Sorted             |     3,635.5 ns |     25.88 ns |     9.23 ns |  1.10 |    0.00 |    4 |         - |          NA |
| PDQSort                      | 1024 | Sorted             |     1,183.1 ns |     29.24 ns |    12.98 ns |  0.36 |    0.00 |    3 |         - |          NA |
| PDQSortBranchless            | 1024 | Sorted             |     1,206.9 ns |     39.18 ns |    17.39 ns |  0.37 |    0.00 |    3 |         - |          NA |
| Ipnsort                      | 1024 | Sorted             |       481.8 ns |     17.68 ns |     7.85 ns |  0.15 |    0.00 |    1 |         - |          NA |
| StdSort                      | 1024 | Sorted             |     1,258.0 ns |     23.50 ns |    10.43 ns |  0.38 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 1024 | Sorted             |     6,239.7 ns |     73.01 ns |    32.42 ns |  1.89 |    0.01 |    5 |         - |          NA |
| Ips4oSort                    | 1024 | Sorted             |       402.7 ns |      6.57 ns |     3.44 ns |  0.12 |    0.00 |    1 |         - |          NA |
| FluxSort                     | 1024 | Sorted             |       637.1 ns |     15.57 ns |     6.91 ns |  0.19 |    0.00 |    2 |         - |          NA |
| CrumSort                     | 1024 | Sorted             |       636.4 ns |     35.52 ns |    15.77 ns |  0.19 |    0.00 |    2 |         - |          NA |
| BlitSort                     | 1024 | Sorted             |       642.1 ns |     22.64 ns |    10.05 ns |  0.19 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 1024 | Sorted             |     3,255.2 ns |    314.79 ns |   164.64 ns |  0.99 |    0.05 |    4 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Reversed**           |     **3,741.9 ns** |    **122.45 ns** |    **64.04 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Reversed           |     7,079.8 ns |    607.56 ns |   317.77 ns |  1.89 |    0.09 |    5 |         - |          NA |
| QuickSortMedian3             | 1024 | Reversed           |     4,710.0 ns |    352.96 ns |   156.72 ns |  1.26 |    0.04 |    5 |         - |          NA |
| QuickSortMedian9             | 1024 | Reversed           |     4,883.0 ns |     55.85 ns |    24.80 ns |  1.31 |    0.02 |    5 |         - |          NA |
| SampleSort                   | 1024 | Reversed           |    16,899.6 ns |    613.97 ns |   321.12 ns |  4.52 |    0.11 |    7 |         - |          NA |
| SimdQuickSort                | 1024 | Reversed           |     3,242.3 ns |     34.30 ns |    12.23 ns |  0.87 |    0.01 |    4 |         - |          NA |
| DualPivotQuickSort           | 1024 | Reversed           |     3,653.7 ns |    227.54 ns |   101.03 ns |  0.98 |    0.03 |    4 |         - |          NA |
| StableQuickSort              | 1024 | Reversed           |    31,393.8 ns |    810.47 ns |   423.89 ns |  8.39 |    0.17 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Reversed           |    15,765.7 ns |    194.38 ns |   101.66 ns |  4.21 |    0.07 |    7 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Reversed           |    30,722.4 ns |    704.96 ns |   368.71 ns |  8.21 |    0.16 |    8 |         - |          NA |
| IntroSort                    | 1024 | Reversed           |     3,211.5 ns |     13.66 ns |     6.07 ns |  0.86 |    0.01 |    4 |         - |          NA |
| IntroSortDotnet              | 1024 | Reversed           |     5,527.7 ns |    232.51 ns |   103.24 ns |  1.48 |    0.04 |    5 |         - |          NA |
| PDQSort                      | 1024 | Reversed           |     1,612.6 ns |     14.37 ns |     6.38 ns |  0.43 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | Reversed           |     2,026.4 ns |      9.48 ns |     4.21 ns |  0.54 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 1024 | Reversed           |       761.3 ns |     48.62 ns |    21.59 ns |  0.20 |    0.01 |    1 |         - |          NA |
| StdSort                      | 1024 | Reversed           |     2,380.7 ns |     19.07 ns |     8.47 ns |  0.64 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 1024 | Reversed           |     8,563.3 ns |    404.68 ns |   179.68 ns |  2.29 |    0.06 |    6 |         - |          NA |
| Ips4oSort                    | 1024 | Reversed           |       725.6 ns |      9.65 ns |     5.05 ns |  0.19 |    0.00 |    1 |         - |          NA |
| FluxSort                     | 1024 | Reversed           |       884.8 ns |      7.08 ns |     2.53 ns |  0.24 |    0.00 |    1 |         - |          NA |
| CrumSort                     | 1024 | Reversed           |       910.9 ns |      9.91 ns |     5.18 ns |  0.24 |    0.00 |    1 |         - |          NA |
| BlitSort                     | 1024 | Reversed           |       905.0 ns |     15.29 ns |     6.79 ns |  0.24 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 1024 | Reversed           |     6,299.3 ns |    942.48 ns |   418.47 ns |  1.68 |    0.11 |    5 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **PipeOrgan**          |    **83,637.4 ns** |    **472.38 ns** |   **209.74 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 1024 | PipeOrgan          |    15,281.4 ns |    739.52 ns |   328.35 ns |  0.18 |    0.00 |    2 |         - |          NA |
| QuickSortMedian3             | 1024 | PipeOrgan          |    12,880.8 ns |    627.80 ns |   328.35 ns |  0.15 |    0.00 |    2 |         - |          NA |
| QuickSortMedian9             | 1024 | PipeOrgan          |     9,169.6 ns |    238.79 ns |   106.02 ns |  0.11 |    0.00 |    2 |         - |          NA |
| SampleSort                   | 1024 | PipeOrgan          |    16,892.0 ns |    764.57 ns |   399.88 ns |  0.20 |    0.00 |    2 |         - |          NA |
| SimdQuickSort                | 1024 | PipeOrgan          |     5,923.4 ns |    296.94 ns |   155.30 ns |  0.07 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort           | 1024 | PipeOrgan          |     7,658.7 ns |    171.42 ns |    89.66 ns |  0.09 |    0.00 |    2 |         - |          NA |
| StableQuickSort              | 1024 | PipeOrgan          |    30,790.4 ns |    596.20 ns |   311.82 ns |  0.37 |    0.00 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | PipeOrgan          |    15,885.3 ns |    302.71 ns |   134.41 ns |  0.19 |    0.00 |    2 |         - |          NA |
| DestswapStableQuickSort      | 1024 | PipeOrgan          |    43,180.7 ns |    435.94 ns |   193.56 ns |  0.52 |    0.00 |    4 |         - |          NA |
| IntroSort                    | 1024 | PipeOrgan          |    20,325.8 ns |    244.16 ns |   108.41 ns |  0.24 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet              | 1024 | PipeOrgan          |    11,092.9 ns |    225.65 ns |   100.19 ns |  0.13 |    0.00 |    2 |         - |          NA |
| PDQSort                      | 1024 | PipeOrgan          |     6,582.6 ns |    268.40 ns |   140.38 ns |  0.08 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | PipeOrgan          |    10,416.5 ns |    357.04 ns |   186.74 ns |  0.12 |    0.00 |    2 |         - |          NA |
| Ipnsort                      | 1024 | PipeOrgan          |    16,660.9 ns |    179.34 ns |    93.80 ns |  0.20 |    0.00 |    2 |         - |          NA |
| StdSort                      | 1024 | PipeOrgan          |    18,188.0 ns |     76.27 ns |    33.87 ns |  0.22 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | PipeOrgan          |    17,916.1 ns |    204.35 ns |    90.73 ns |  0.21 |    0.00 |    2 |         - |          NA |
| Ips4oSort                    | 1024 | PipeOrgan          |    13,643.7 ns |    198.05 ns |   103.58 ns |  0.16 |    0.00 |    2 |         - |          NA |
| FluxSort                     | 1024 | PipeOrgan          |     2,274.6 ns |     10.66 ns |     4.73 ns |  0.03 |    0.00 |    1 |         - |          NA |
| CrumSort                     | 1024 | PipeOrgan          |     2,327.6 ns |     17.32 ns |     7.69 ns |  0.03 |    0.00 |    1 |         - |          NA |
| BlitSort                     | 1024 | PipeOrgan          |     2,305.8 ns |     28.21 ns |    12.53 ns |  0.03 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 1024 | PipeOrgan          |    12,842.0 ns |    470.22 ns |   245.93 ns |  0.15 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **ManyDuplicates**     |    **26,491.1 ns** |    **154.18 ns** |    **80.64 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 1024 | ManyDuplicates     |    20,241.0 ns |    373.85 ns |   165.99 ns |  0.76 |    0.01 |    5 |         - |          NA |
| QuickSortMedian3             | 1024 | ManyDuplicates     |    29,947.4 ns |    304.11 ns |   159.05 ns |  1.13 |    0.01 |    5 |         - |          NA |
| QuickSortMedian9             | 1024 | ManyDuplicates     |    31,405.2 ns |    374.40 ns |   195.82 ns |  1.19 |    0.01 |    5 |         - |          NA |
| SampleSort                   | 1024 | ManyDuplicates     |    39,343.1 ns |    279.14 ns |   145.99 ns |  1.49 |    0.01 |    5 |         - |          NA |
| SimdQuickSort                | 1024 | ManyDuplicates     |     4,779.7 ns |    437.80 ns |   228.98 ns |  0.18 |    0.01 |    1 |         - |          NA |
| DualPivotQuickSort           | 1024 | ManyDuplicates     |    19,592.3 ns |    675.49 ns |   299.92 ns |  0.74 |    0.01 |    5 |         - |          NA |
| StableQuickSort              | 1024 | ManyDuplicates     |    45,960.8 ns |  1,403.04 ns |   733.82 ns |  1.73 |    0.03 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | ManyDuplicates     |    20,947.6 ns |    293.07 ns |   130.12 ns |  0.79 |    0.01 |    5 |         - |          NA |
| DestswapStableQuickSort      | 1024 | ManyDuplicates     |    35,425.0 ns |    339.61 ns |   150.79 ns |  1.34 |    0.01 |    5 |         - |          NA |
| IntroSort                    | 1024 | ManyDuplicates     |    24,888.4 ns |    229.11 ns |   119.83 ns |  0.94 |    0.01 |    5 |         - |          NA |
| IntroSortDotnet              | 1024 | ManyDuplicates     |    25,416.6 ns |    262.01 ns |   137.04 ns |  0.96 |    0.01 |    5 |         - |          NA |
| PDQSort                      | 1024 | ManyDuplicates     |    20,459.5 ns |    131.35 ns |    68.70 ns |  0.77 |    0.00 |    5 |         - |          NA |
| PDQSortBranchless            | 1024 | ManyDuplicates     |     9,622.6 ns |    308.91 ns |   161.57 ns |  0.36 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 1024 | ManyDuplicates     |    15,286.9 ns |    138.18 ns |    72.27 ns |  0.58 |    0.00 |    4 |         - |          NA |
| StdSort                      | 1024 | ManyDuplicates     |    15,067.2 ns |    294.30 ns |   153.92 ns |  0.57 |    0.01 |    4 |         - |          NA |
| BlockQuickSort               | 1024 | ManyDuplicates     |    16,188.9 ns |    189.81 ns |    99.27 ns |  0.61 |    0.00 |    4 |         - |          NA |
| Ips4oSort                    | 1024 | ManyDuplicates     |    12,329.9 ns |    264.44 ns |   138.31 ns |  0.47 |    0.01 |    3 |         - |          NA |
| FluxSort                     | 1024 | ManyDuplicates     |    24,723.7 ns |    288.44 ns |   150.86 ns |  0.93 |    0.01 |    5 |         - |          NA |
| CrumSort                     | 1024 | ManyDuplicates     |    24,893.9 ns |    267.74 ns |   140.03 ns |  0.94 |    0.01 |    5 |         - |          NA |
| BlitSort                     | 1024 | ManyDuplicates     |    25,816.3 ns |    280.86 ns |   146.89 ns |  0.97 |    0.01 |    5 |         - |          NA |
| DotnetSort                   | 1024 | ManyDuplicates     |    23,260.3 ns |    388.23 ns |   203.05 ns |  0.88 |    0.01 |    5 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Random**             |   **185,004.5 ns** |  **1,245.82 ns** |   **651.59 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Random             |   202,322.5 ns |    725.84 ns |   322.28 ns |  1.09 |    0.00 |    3 |         - |          NA |
| QuickSortMedian3             | 4096 | Random             |   194,335.4 ns |  1,740.40 ns |   910.26 ns |  1.05 |    0.01 |    3 |         - |          NA |
| QuickSortMedian9             | 4096 | Random             |   205,791.4 ns |  1,501.61 ns |   666.72 ns |  1.11 |    0.00 |    3 |         - |          NA |
| SampleSort                   | 4096 | Random             |   234,211.2 ns |    910.21 ns |   476.06 ns |  1.27 |    0.00 |    3 |         - |          NA |
| SimdQuickSort                | 4096 | Random             |    19,532.7 ns |    159.46 ns |    70.80 ns |  0.11 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort           | 4096 | Random             |   143,809.0 ns |    565.05 ns |   250.89 ns |  0.78 |    0.00 |    3 |         - |          NA |
| StableQuickSort              | 4096 | Random             |   468,827.2 ns |  1,847.80 ns |   966.44 ns |  2.53 |    0.01 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Random             |   242,416.3 ns |  2,772.42 ns | 1,450.03 ns |  1.31 |    0.01 |    3 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Random             |   389,733.3 ns |  1,612.68 ns |   716.04 ns |  2.11 |    0.01 |    4 |         - |          NA |
| IntroSort                    | 4096 | Random             |   160,361.2 ns |    193.53 ns |    69.01 ns |  0.87 |    0.00 |    3 |         - |          NA |
| IntroSortDotnet              | 4096 | Random             |   155,652.4 ns |  1,435.15 ns |   750.61 ns |  0.84 |    0.00 |    3 |         - |          NA |
| PDQSort                      | 4096 | Random             |   148,963.6 ns |  1,105.36 ns |   578.12 ns |  0.81 |    0.00 |    3 |         - |          NA |
| PDQSortBranchless            | 4096 | Random             |    83,658.2 ns |    998.86 ns |   522.42 ns |  0.45 |    0.00 |    2 |         - |          NA |
| Ipnsort                      | 4096 | Random             |    90,489.0 ns |    576.85 ns |   301.70 ns |  0.49 |    0.00 |    2 |         - |          NA |
| StdSort                      | 4096 | Random             |    88,786.2 ns |    509.36 ns |   266.40 ns |  0.48 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 4096 | Random             |    95,985.8 ns |    410.30 ns |   182.18 ns |  0.52 |    0.00 |    2 |         - |          NA |
| Ips4oSort                    | 4096 | Random             |    79,647.7 ns |    248.63 ns |    88.67 ns |  0.43 |    0.00 |    2 |         - |          NA |
| FluxSort                     | 4096 | Random             |   109,807.3 ns |    543.28 ns |   284.15 ns |  0.59 |    0.00 |    2 |         - |          NA |
| CrumSort                     | 4096 | Random             |   109,350.5 ns |    157.44 ns |    82.34 ns |  0.59 |    0.00 |    2 |         - |          NA |
| BlitSort                     | 4096 | Random             |   117,796.2 ns |    388.07 ns |   172.30 ns |  0.64 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 4096 | Random             |   145,499.5 ns |  1,960.25 ns | 1,025.25 ns |  0.79 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **4096** | **SingleElementMoved** |    **19,288.1 ns** |    **501.59 ns** |   **262.34 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 4096 | SingleElementMoved |    33,577.6 ns |  2,757.35 ns | 1,442.15 ns |  1.74 |    0.07 |    4 |         - |          NA |
| QuickSortMedian3             | 4096 | SingleElementMoved |    26,490.5 ns |    403.81 ns |   211.20 ns |  1.37 |    0.02 |    4 |         - |          NA |
| QuickSortMedian9             | 4096 | SingleElementMoved |    30,411.8 ns |    271.34 ns |   120.48 ns |  1.58 |    0.02 |    4 |         - |          NA |
| SampleSort                   | 4096 | SingleElementMoved |   131,173.2 ns |    988.54 ns |   438.92 ns |  6.80 |    0.09 |    7 |         - |          NA |
| SimdQuickSort                | 4096 | SingleElementMoved |    43,429.7 ns |    445.64 ns |   233.08 ns |  2.25 |    0.03 |    5 |         - |          NA |
| DualPivotQuickSort           | 4096 | SingleElementMoved |    16,906.8 ns |  1,176.41 ns |   615.28 ns |  0.88 |    0.03 |    4 |         - |          NA |
| StableQuickSort              | 4096 | SingleElementMoved |   149,620.4 ns |  2,838.93 ns | 1,484.82 ns |  7.76 |    0.12 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | SingleElementMoved |    71,549.9 ns |    260.76 ns |   115.78 ns |  3.71 |    0.05 |    6 |         - |          NA |
| DestswapStableQuickSort      | 4096 | SingleElementMoved |   141,300.5 ns |    509.82 ns |   226.36 ns |  7.33 |    0.09 |    7 |         - |          NA |
| IntroSort                    | 4096 | SingleElementMoved |    51,679.5 ns |    233.14 ns |   121.94 ns |  2.68 |    0.03 |    5 |         - |          NA |
| IntroSortDotnet              | 4096 | SingleElementMoved |    20,985.2 ns |    279.12 ns |   145.99 ns |  1.09 |    0.02 |    4 |         - |          NA |
| PDQSort                      | 4096 | SingleElementMoved |    12,060.3 ns |    287.35 ns |   150.29 ns |  0.63 |    0.01 |    3 |         - |          NA |
| PDQSortBranchless            | 4096 | SingleElementMoved |    13,814.9 ns |    207.62 ns |   108.59 ns |  0.72 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 4096 | SingleElementMoved |    70,468.2 ns |    704.74 ns |   312.91 ns |  3.65 |    0.05 |    6 |         - |          NA |
| StdSort                      | 4096 | SingleElementMoved |    12,919.0 ns |    169.78 ns |    88.80 ns |  0.67 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 4096 | SingleElementMoved |    34,084.7 ns |    358.64 ns |   159.24 ns |  1.77 |    0.02 |    4 |         - |          NA |
| Ips4oSort                    | 4096 | SingleElementMoved |    50,504.3 ns |    332.11 ns |   147.46 ns |  2.62 |    0.03 |    5 |         - |          NA |
| FluxSort                     | 4096 | SingleElementMoved |     4,347.8 ns |    173.84 ns |    90.92 ns |  0.23 |    0.01 |    1 |         - |          NA |
| CrumSort                     | 4096 | SingleElementMoved |     5,693.3 ns |    235.62 ns |   123.24 ns |  0.30 |    0.01 |    2 |         - |          NA |
| BlitSort                     | 4096 | SingleElementMoved |     5,698.4 ns |    221.37 ns |   115.78 ns |  0.30 |    0.01 |    2 |         - |          NA |
| DotnetSort                   | 4096 | SingleElementMoved |    24,964.7 ns |  1,772.65 ns |   927.13 ns |  1.29 |    0.05 |    4 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Sorted**             |    **15,643.2 ns** |    **260.17 ns** |   **136.07 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Sorted             |    27,732.1 ns |  1,611.13 ns |   842.65 ns |  1.77 |    0.05 |    5 |         - |          NA |
| QuickSortMedian3             | 4096 | Sorted             |    19,778.5 ns |    217.36 ns |   113.68 ns |  1.26 |    0.01 |    4 |         - |          NA |
| QuickSortMedian9             | 4096 | Sorted             |    21,003.0 ns |    358.86 ns |   187.69 ns |  1.34 |    0.02 |    4 |         - |          NA |
| SampleSort                   | 4096 | Sorted             |    69,681.5 ns |  2,101.75 ns | 1,099.25 ns |  4.45 |    0.08 |    7 |         - |          NA |
| SimdQuickSort                | 4096 | Sorted             |    45,089.9 ns |    242.11 ns |   126.63 ns |  2.88 |    0.02 |    6 |         - |          NA |
| DualPivotQuickSort           | 4096 | Sorted             |    15,264.8 ns |  1,700.95 ns |   889.63 ns |  0.98 |    0.05 |    4 |         - |          NA |
| StableQuickSort              | 4096 | Sorted             |   148,333.3 ns |  3,240.47 ns | 1,694.83 ns |  9.48 |    0.13 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Sorted             |    70,455.4 ns |    147.34 ns |    52.54 ns |  4.50 |    0.04 |    7 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Sorted             |   140,282.3 ns |  1,398.66 ns |   621.01 ns |  8.97 |    0.08 |    8 |         - |          NA |
| IntroSort                    | 4096 | Sorted             |    14,990.6 ns |    123.35 ns |    64.51 ns |  0.96 |    0.01 |    4 |         - |          NA |
| IntroSortDotnet              | 4096 | Sorted             |    17,114.1 ns |    174.48 ns |    77.47 ns |  1.09 |    0.01 |    4 |         - |          NA |
| PDQSort                      | 4096 | Sorted             |     4,640.5 ns |    257.00 ns |   134.41 ns |  0.30 |    0.01 |    3 |         - |          NA |
| PDQSortBranchless            | 4096 | Sorted             |     4,538.7 ns |     22.53 ns |     8.03 ns |  0.29 |    0.00 |    3 |         - |          NA |
| Ipnsort                      | 4096 | Sorted             |     1,793.4 ns |     52.46 ns |    23.29 ns |  0.11 |    0.00 |    1 |         - |          NA |
| StdSort                      | 4096 | Sorted             |     4,726.5 ns |    287.60 ns |   150.42 ns |  0.30 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 4096 | Sorted             |    30,401.2 ns |    244.96 ns |   128.12 ns |  1.94 |    0.02 |    5 |         - |          NA |
| Ips4oSort                    | 4096 | Sorted             |     1,513.1 ns |     45.14 ns |    16.10 ns |  0.10 |    0.00 |    1 |         - |          NA |
| FluxSort                     | 4096 | Sorted             |     2,222.7 ns |      9.55 ns |     3.41 ns |  0.14 |    0.00 |    2 |         - |          NA |
| CrumSort                     | 4096 | Sorted             |     2,246.7 ns |     31.74 ns |    11.32 ns |  0.14 |    0.00 |    2 |         - |          NA |
| BlitSort                     | 4096 | Sorted             |     2,265.6 ns |    143.48 ns |    63.70 ns |  0.14 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 4096 | Sorted             |    15,853.9 ns |  2,305.05 ns | 1,205.59 ns |  1.01 |    0.07 |    4 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Reversed**           |    **17,186.9 ns** |    **456.06 ns** |   **202.49 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Reversed           |    30,741.2 ns |  1,582.86 ns |   827.87 ns |  1.79 |    0.05 |    6 |         - |          NA |
| QuickSortMedian3             | 4096 | Reversed           |    20,906.5 ns |    316.26 ns |   140.42 ns |  1.22 |    0.02 |    5 |         - |          NA |
| QuickSortMedian9             | 4096 | Reversed           |    22,097.7 ns |    234.75 ns |   122.78 ns |  1.29 |    0.02 |    5 |         - |          NA |
| SampleSort                   | 4096 | Reversed           |    74,371.7 ns |  1,869.97 ns |   978.03 ns |  4.33 |    0.07 |    8 |         - |          NA |
| SimdQuickSort                | 4096 | Reversed           |    27,637.4 ns |    335.98 ns |   175.72 ns |  1.61 |    0.02 |    6 |         - |          NA |
| DualPivotQuickSort           | 4096 | Reversed           |    16,761.4 ns |  1,099.30 ns |   574.95 ns |  0.98 |    0.03 |    4 |         - |          NA |
| StableQuickSort              | 4096 | Reversed           |   151,638.6 ns |  2,406.70 ns | 1,258.75 ns |  8.82 |    0.12 |    9 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Reversed           |    70,665.4 ns |    336.82 ns |   176.16 ns |  4.11 |    0.05 |    8 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Reversed           |   136,866.7 ns |  1,387.02 ns |   725.44 ns |  7.96 |    0.10 |    9 |         - |          NA |
| IntroSort                    | 4096 | Reversed           |    15,535.9 ns |    116.70 ns |    51.82 ns |  0.90 |    0.01 |    4 |         - |          NA |
| IntroSortDotnet              | 4096 | Reversed           |    26,869.0 ns |    340.93 ns |   178.31 ns |  1.56 |    0.02 |    6 |         - |          NA |
| PDQSort                      | 4096 | Reversed           |     6,399.3 ns |    271.68 ns |   142.09 ns |  0.37 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 4096 | Reversed           |     7,870.9 ns |    329.01 ns |   172.08 ns |  0.46 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 4096 | Reversed           |     2,790.6 ns |     50.30 ns |    17.94 ns |  0.16 |    0.00 |    1 |         - |          NA |
| StdSort                      | 4096 | Reversed           |     9,199.4 ns |    316.78 ns |   165.68 ns |  0.54 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 4096 | Reversed           |    37,490.1 ns |    387.47 ns |   202.65 ns |  2.18 |    0.03 |    7 |         - |          NA |
| Ips4oSort                    | 4096 | Reversed           |     2,672.2 ns |     54.55 ns |    24.22 ns |  0.16 |    0.00 |    1 |         - |          NA |
| FluxSort                     | 4096 | Reversed           |     3,341.2 ns |     29.95 ns |    13.30 ns |  0.19 |    0.00 |    1 |         - |          NA |
| CrumSort                     | 4096 | Reversed           |     3,370.5 ns |     15.95 ns |     5.69 ns |  0.20 |    0.00 |    1 |         - |          NA |
| BlitSort                     | 4096 | Reversed           |     3,347.2 ns |     24.41 ns |     8.70 ns |  0.19 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 4096 | Reversed           |    27,079.2 ns |  2,314.81 ns | 1,027.79 ns |  1.58 |    0.06 |    6 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **4096** | **PipeOrgan**          | **1,216,233.4 ns** |  **5,046.13 ns** | **2,639.22 ns** | **1.000** |    **0.00** |    **6** |         **-** |          **NA** |
| QuickSort3way                | 4096 | PipeOrgan          |    79,837.4 ns |  2,311.11 ns | 1,208.76 ns | 0.066 |    0.00 |    3 |         - |          NA |
| QuickSortMedian3             | 4096 | PipeOrgan          |    63,828.4 ns |  1,812.93 ns |   948.20 ns | 0.052 |    0.00 |    3 |         - |          NA |
| QuickSortMedian9             | 4096 | PipeOrgan          |    41,630.3 ns |  1,937.17 ns | 1,013.18 ns | 0.034 |    0.00 |    2 |         - |          NA |
| SampleSort                   | 4096 | PipeOrgan          |    76,306.5 ns |  1,699.84 ns |   889.05 ns | 0.063 |    0.00 |    3 |         - |          NA |
| SimdQuickSort                | 4096 | PipeOrgan          |    32,638.5 ns |    433.98 ns |   226.98 ns | 0.027 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort           | 4096 | PipeOrgan          |    35,648.8 ns |    913.39 ns |   477.72 ns | 0.029 |    0.00 |    2 |         - |          NA |
| StableQuickSort              | 4096 | PipeOrgan          |   149,957.8 ns |  2,390.61 ns | 1,250.33 ns | 0.123 |    0.00 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | PipeOrgan          |    71,274.8 ns |    776.78 ns |   406.27 ns | 0.059 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 4096 | PipeOrgan          |   179,083.8 ns |    724.40 ns |   321.64 ns | 0.147 |    0.00 |    5 |         - |          NA |
| IntroSort                    | 4096 | PipeOrgan          |   104,906.2 ns |    702.68 ns |   311.99 ns | 0.086 |    0.00 |    4 |         - |          NA |
| IntroSortDotnet              | 4096 | PipeOrgan          |    64,897.9 ns |  1,208.26 ns |   631.95 ns | 0.053 |    0.00 |    3 |         - |          NA |
| PDQSort                      | 4096 | PipeOrgan          |    31,502.9 ns |    687.11 ns |   359.37 ns | 0.026 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 4096 | PipeOrgan          |    47,055.1 ns |    330.56 ns |   117.88 ns | 0.039 |    0.00 |    2 |         - |          NA |
| Ipnsort                      | 4096 | PipeOrgan          |    83,335.8 ns |    568.23 ns |   252.30 ns | 0.069 |    0.00 |    3 |         - |          NA |
| StdSort                      | 4096 | PipeOrgan          |   102,100.2 ns |    648.36 ns |   339.11 ns | 0.084 |    0.00 |    4 |         - |          NA |
| BlockQuickSort               | 4096 | PipeOrgan          |    80,828.3 ns |    626.45 ns |   278.15 ns | 0.066 |    0.00 |    3 |         - |          NA |
| Ips4oSort                    | 4096 | PipeOrgan          |    59,871.6 ns |    259.06 ns |   115.03 ns | 0.049 |    0.00 |    3 |         - |          NA |
| FluxSort                     | 4096 | PipeOrgan          |     8,987.6 ns |    208.58 ns |   109.09 ns | 0.007 |    0.00 |    1 |         - |          NA |
| CrumSort                     | 4096 | PipeOrgan          |    10,752.7 ns |    351.83 ns |   184.02 ns | 0.009 |    0.00 |    1 |         - |          NA |
| BlitSort                     | 4096 | PipeOrgan          |    10,592.1 ns |    237.80 ns |   124.37 ns | 0.009 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 4096 | PipeOrgan          |    71,364.5 ns |  2,119.92 ns | 1,108.76 ns | 0.059 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **4096** | **ManyDuplicates**     |   **103,318.9 ns** |    **757.86 ns** |   **396.37 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 4096 | ManyDuplicates     |    74,956.8 ns |    546.25 ns |   285.70 ns |  0.73 |    0.00 |    3 |         - |          NA |
| QuickSortMedian3             | 4096 | ManyDuplicates     |   113,343.6 ns |    447.17 ns |   233.88 ns |  1.10 |    0.00 |    4 |         - |          NA |
| QuickSortMedian9             | 4096 | ManyDuplicates     |   118,965.9 ns |    719.78 ns |   376.46 ns |  1.15 |    0.01 |    4 |         - |          NA |
| SampleSort                   | 4096 | ManyDuplicates     |   156,013.5 ns |  1,620.05 ns |   847.32 ns |  1.51 |    0.01 |    5 |         - |          NA |
| SimdQuickSort                | 4096 | ManyDuplicates     |    24,327.0 ns |    268.87 ns |   140.62 ns |  0.24 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort           | 4096 | ManyDuplicates     |    73,078.6 ns |    840.20 ns |   439.44 ns |  0.71 |    0.00 |    3 |         - |          NA |
| StableQuickSort              | 4096 | ManyDuplicates     |   178,205.8 ns |  3,036.82 ns | 1,588.31 ns |  1.72 |    0.02 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | ManyDuplicates     |    78,356.6 ns |  2,373.52 ns | 1,053.86 ns |  0.76 |    0.01 |    3 |         - |          NA |
| DestswapStableQuickSort      | 4096 | ManyDuplicates     |   129,909.1 ns |    414.03 ns |   183.83 ns |  1.26 |    0.00 |    4 |         - |          NA |
| IntroSort                    | 4096 | ManyDuplicates     |    96,957.9 ns |    540.33 ns |   239.91 ns |  0.94 |    0.00 |    4 |         - |          NA |
| IntroSortDotnet              | 4096 | ManyDuplicates     |   103,463.5 ns |    567.36 ns |   296.74 ns |  1.00 |    0.00 |    4 |         - |          NA |
| PDQSort                      | 4096 | ManyDuplicates     |    73,893.0 ns |    567.64 ns |   296.88 ns |  0.72 |    0.00 |    3 |         - |          NA |
| PDQSortBranchless            | 4096 | ManyDuplicates     |    26,815.0 ns |     93.45 ns |    41.49 ns |  0.26 |    0.00 |    1 |         - |          NA |
| Ipnsort                      | 4096 | ManyDuplicates     |    48,788.7 ns |    198.69 ns |   103.92 ns |  0.47 |    0.00 |    2 |         - |          NA |
| StdSort                      | 4096 | ManyDuplicates     |    42,502.0 ns |    316.22 ns |   140.41 ns |  0.41 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 4096 | ManyDuplicates     |    55,681.6 ns |    926.18 ns |   330.28 ns |  0.54 |    0.00 |    2 |         - |          NA |
| Ips4oSort                    | 4096 | ManyDuplicates     |    41,279.8 ns |  1,371.99 ns |   717.58 ns |  0.40 |    0.01 |    2 |         - |          NA |
| FluxSort                     | 4096 | ManyDuplicates     |    34,175.6 ns |    254.87 ns |   113.16 ns |  0.33 |    0.00 |    2 |         - |          NA |
| CrumSort                     | 4096 | ManyDuplicates     |    34,511.5 ns |    696.94 ns |   309.45 ns |  0.33 |    0.00 |    2 |         - |          NA |
| BlitSort                     | 4096 | ManyDuplicates     |    42,284.8 ns |    263.77 ns |   137.96 ns |  0.41 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 4096 | ManyDuplicates     |    94,678.5 ns |    752.94 ns |   393.80 ns |  0.92 |    0.00 |    4 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Random**             |   **393,385.1 ns** |  **1,966.13 ns** |   **872.97 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Random             |   431,248.9 ns |  1,222.71 ns |   542.89 ns |  1.10 |    0.00 |    3 |         - |          NA |
| QuickSortMedian3             | 8192 | Random             |   412,476.7 ns |  2,182.77 ns | 1,141.63 ns |  1.05 |    0.00 |    3 |         - |          NA |
| QuickSortMedian9             | 8192 | Random             |   438,561.8 ns |  2,262.19 ns | 1,183.17 ns |  1.11 |    0.00 |    3 |         - |          NA |
| SampleSort                   | 8192 | Random             |   497,805.2 ns |  2,598.57 ns | 1,153.78 ns |  1.27 |    0.00 |    3 |         - |          NA |
| SimdQuickSort                | 8192 | Random             |    41,937.8 ns |    296.39 ns |   131.60 ns |  0.11 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort           | 8192 | Random             |   311,963.4 ns |    768.65 ns |   274.11 ns |  0.79 |    0.00 |    3 |         - |          NA |
| StableQuickSort              | 8192 | Random             | 1,010,841.3 ns |  1,672.59 ns |   742.64 ns |  2.57 |    0.01 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Random             |   519,109.3 ns |  1,706.28 ns |   757.60 ns |  1.32 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Random             |   822,474.8 ns |  2,441.24 ns | 1,083.92 ns |  2.09 |    0.01 |    4 |         - |          NA |
| IntroSort                    | 8192 | Random             |   346,260.1 ns |    524.18 ns |   232.74 ns |  0.88 |    0.00 |    3 |         - |          NA |
| IntroSortDotnet              | 8192 | Random             |   336,646.5 ns |    673.73 ns |   299.14 ns |  0.86 |    0.00 |    3 |         - |          NA |
| PDQSort                      | 8192 | Random             |   323,140.3 ns |    477.26 ns |   211.90 ns |  0.82 |    0.00 |    3 |         - |          NA |
| PDQSortBranchless            | 8192 | Random             |   173,527.7 ns |    465.46 ns |   206.67 ns |  0.44 |    0.00 |    2 |         - |          NA |
| Ipnsort                      | 8192 | Random             |   196,213.2 ns |  1,285.56 ns |   570.80 ns |  0.50 |    0.00 |    2 |         - |          NA |
| StdSort                      | 8192 | Random             |   187,045.4 ns |    360.36 ns |   160.00 ns |  0.48 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | Random             |   199,500.8 ns |    530.03 ns |   235.33 ns |  0.51 |    0.00 |    2 |         - |          NA |
| Ips4oSort                    | 8192 | Random             |   173,598.9 ns |  1,083.17 ns |   480.94 ns |  0.44 |    0.00 |    2 |         - |          NA |
| FluxSort                     | 8192 | Random             |   226,753.4 ns |  1,392.77 ns |   618.40 ns |  0.58 |    0.00 |    2 |         - |          NA |
| CrumSort                     | 8192 | Random             |   227,366.4 ns |  1,372.72 ns |   717.96 ns |  0.58 |    0.00 |    2 |         - |          NA |
| BlitSort                     | 8192 | Random             |   251,331.1 ns |  1,018.67 ns |   452.29 ns |  0.64 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 8192 | Random             |   314,778.8 ns |  2,852.66 ns | 1,492.00 ns |  0.80 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **SingleElementMoved** |    **40,488.5 ns** |    **977.37 ns** |   **433.96 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 8192 | SingleElementMoved |    69,158.7 ns |  3,277.37 ns | 1,714.13 ns |  1.71 |    0.04 |    4 |         - |          NA |
| QuickSortMedian3             | 8192 | SingleElementMoved |    54,153.8 ns |    241.20 ns |   126.15 ns |  1.34 |    0.01 |    4 |         - |          NA |
| QuickSortMedian9             | 8192 | SingleElementMoved |    63,033.2 ns |    895.58 ns |   319.37 ns |  1.56 |    0.02 |    4 |         - |          NA |
| SampleSort                   | 8192 | SingleElementMoved |   318,708.3 ns |  3,013.97 ns | 1,576.36 ns |  7.87 |    0.09 |    7 |         - |          NA |
| SimdQuickSort                | 8192 | SingleElementMoved |   136,801.9 ns |    476.87 ns |   249.41 ns |  3.38 |    0.03 |    6 |         - |          NA |
| DualPivotQuickSort           | 8192 | SingleElementMoved |    38,417.7 ns |  2,748.90 ns | 1,220.53 ns |  0.95 |    0.03 |    4 |         - |          NA |
| StableQuickSort              | 8192 | SingleElementMoved |   324,191.8 ns |  2,084.72 ns | 1,090.35 ns |  8.01 |    0.09 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | SingleElementMoved |   151,682.2 ns |  1,032.48 ns |   540.01 ns |  3.75 |    0.04 |    6 |         - |          NA |
| DestswapStableQuickSort      | 8192 | SingleElementMoved |   294,254.0 ns |  1,901.49 ns |   844.27 ns |  7.27 |    0.08 |    7 |         - |          NA |
| IntroSort                    | 8192 | SingleElementMoved |   108,426.1 ns |    864.29 ns |   308.21 ns |  2.68 |    0.03 |    5 |         - |          NA |
| IntroSortDotnet              | 8192 | SingleElementMoved |    44,190.8 ns |    127.28 ns |    56.51 ns |  1.09 |    0.01 |    4 |         - |          NA |
| PDQSort                      | 8192 | SingleElementMoved |    23,605.0 ns |    297.17 ns |   131.94 ns |  0.58 |    0.01 |    3 |         - |          NA |
| PDQSortBranchless            | 8192 | SingleElementMoved |    27,200.8 ns |    332.10 ns |   173.69 ns |  0.67 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 8192 | SingleElementMoved |   154,013.2 ns |    959.03 ns |   501.59 ns |  3.80 |    0.04 |    6 |         - |          NA |
| StdSort                      | 8192 | SingleElementMoved |    25,293.0 ns |    591.74 ns |   262.74 ns |  0.62 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 8192 | SingleElementMoved |    73,471.8 ns |  1,638.26 ns |   727.40 ns |  1.81 |    0.02 |    4 |         - |          NA |
| Ips4oSort                    | 8192 | SingleElementMoved |    98,616.4 ns |  2,101.90 ns | 1,099.33 ns |  2.44 |    0.04 |    5 |         - |          NA |
| FluxSort                     | 8192 | SingleElementMoved |     8,632.3 ns |    405.01 ns |   211.83 ns |  0.21 |    0.01 |    1 |         - |          NA |
| CrumSort                     | 8192 | SingleElementMoved |    11,247.7 ns |    267.29 ns |   139.80 ns |  0.28 |    0.00 |    2 |         - |          NA |
| BlitSort                     | 8192 | SingleElementMoved |    11,295.0 ns |    309.21 ns |   161.72 ns |  0.28 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 8192 | SingleElementMoved |    52,483.1 ns |  1,204.24 ns |   534.69 ns |  1.30 |    0.02 |    4 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Sorted**             |    **33,777.2 ns** |    **756.41 ns** |   **335.85 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Sorted             |    59,105.9 ns |  3,472.34 ns | 1,541.74 ns |  1.75 |    0.05 |    5 |         - |          NA |
| QuickSortMedian3             | 8192 | Sorted             |    41,675.3 ns |    314.91 ns |   164.70 ns |  1.23 |    0.01 |    4 |         - |          NA |
| QuickSortMedian9             | 8192 | Sorted             |    44,101.0 ns |    259.82 ns |   135.89 ns |  1.31 |    0.01 |    4 |         - |          NA |
| SampleSort                   | 8192 | Sorted             |   199,919.0 ns |  7,880.93 ns | 4,121.88 ns |  5.92 |    0.13 |    7 |         - |          NA |
| SimdQuickSort                | 8192 | Sorted             |   141,623.8 ns |    668.63 ns |   296.88 ns |  4.19 |    0.04 |    6 |         - |          NA |
| DualPivotQuickSort           | 8192 | Sorted             |    37,566.4 ns |  2,694.05 ns | 1,409.04 ns |  1.11 |    0.04 |    4 |         - |          NA |
| StableQuickSort              | 8192 | Sorted             |   324,310.4 ns |  2,635.74 ns | 1,378.54 ns |  9.60 |    0.10 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Sorted             |   150,029.3 ns |  1,252.76 ns |   655.22 ns |  4.44 |    0.04 |    6 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Sorted             |   291,963.0 ns |  1,737.48 ns |   908.74 ns |  8.64 |    0.08 |    8 |         - |          NA |
| IntroSort                    | 8192 | Sorted             |    32,034.6 ns |    354.80 ns |   185.57 ns |  0.95 |    0.01 |    4 |         - |          NA |
| IntroSortDotnet              | 8192 | Sorted             |    36,596.6 ns |    221.87 ns |    98.51 ns |  1.08 |    0.01 |    4 |         - |          NA |
| PDQSort                      | 8192 | Sorted             |     9,198.8 ns |    445.07 ns |   232.78 ns |  0.27 |    0.01 |    3 |         - |          NA |
| PDQSortBranchless            | 8192 | Sorted             |     9,237.3 ns |    106.66 ns |    55.78 ns |  0.27 |    0.00 |    3 |         - |          NA |
| Ipnsort                      | 8192 | Sorted             |     3,634.1 ns |    227.27 ns |   100.91 ns |  0.11 |    0.00 |    1 |         - |          NA |
| StdSort                      | 8192 | Sorted             |     9,183.2 ns |    285.88 ns |   149.52 ns |  0.27 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 8192 | Sorted             |    66,082.9 ns |    221.27 ns |    98.24 ns |  1.96 |    0.02 |    5 |         - |          NA |
| Ips4oSort                    | 8192 | Sorted             |     3,009.0 ns |     23.88 ns |    12.49 ns |  0.09 |    0.00 |    1 |         - |          NA |
| FluxSort                     | 8192 | Sorted             |     4,328.8 ns |     27.64 ns |     9.86 ns |  0.13 |    0.00 |    2 |         - |          NA |
| CrumSort                     | 8192 | Sorted             |     4,373.6 ns |     23.49 ns |    10.43 ns |  0.13 |    0.00 |    2 |         - |          NA |
| BlitSort                     | 8192 | Sorted             |     4,492.9 ns |    422.19 ns |   220.81 ns |  0.13 |    0.01 |    2 |         - |          NA |
| DotnetSort                   | 8192 | Sorted             |    32,274.4 ns |    581.04 ns |   257.98 ns |  0.96 |    0.01 |    4 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Reversed**           |    **36,878.1 ns** |  **1,332.84 ns** |   **697.10 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Reversed           |    65,279.6 ns |  2,979.79 ns | 1,558.49 ns |  1.77 |    0.05 |    5 |         - |          NA |
| QuickSortMedian3             | 8192 | Reversed           |    43,623.6 ns |    214.31 ns |    95.15 ns |  1.18 |    0.02 |    4 |         - |          NA |
| QuickSortMedian9             | 8192 | Reversed           |    46,495.7 ns |    345.75 ns |   153.52 ns |  1.26 |    0.02 |    4 |         - |          NA |
| SampleSort                   | 8192 | Reversed           |   276,541.2 ns |  2,291.97 ns | 1,017.65 ns |  7.50 |    0.14 |    7 |         - |          NA |
| SimdQuickSort                | 8192 | Reversed           |    68,168.1 ns |    107.15 ns |    56.04 ns |  1.85 |    0.03 |    5 |         - |          NA |
| DualPivotQuickSort           | 8192 | Reversed           |    38,726.9 ns |  1,370.92 ns |   717.02 ns |  1.05 |    0.03 |    4 |         - |          NA |
| StableQuickSort              | 8192 | Reversed           |   330,712.4 ns |  4,996.09 ns | 2,218.29 ns |  8.97 |    0.17 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Reversed           |   149,982.8 ns |  1,714.42 ns |   896.67 ns |  4.07 |    0.08 |    6 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Reversed           |   281,823.9 ns |  4,069.17 ns | 1,806.74 ns |  7.64 |    0.14 |    7 |         - |          NA |
| IntroSort                    | 8192 | Reversed           |    33,552.5 ns |    662.82 ns |   346.67 ns |  0.91 |    0.02 |    4 |         - |          NA |
| IntroSortDotnet              | 8192 | Reversed           |    58,145.9 ns |    334.59 ns |   175.00 ns |  1.58 |    0.03 |    5 |         - |          NA |
| PDQSort                      | 8192 | Reversed           |    12,858.7 ns |    646.21 ns |   337.98 ns |  0.35 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | Reversed           |    15,718.3 ns |    323.08 ns |   168.98 ns |  0.43 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 8192 | Reversed           |     5,670.1 ns |    234.15 ns |   122.46 ns |  0.15 |    0.00 |    1 |         - |          NA |
| StdSort                      | 8192 | Reversed           |    18,086.5 ns |    175.50 ns |    91.79 ns |  0.49 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 8192 | Reversed           |    80,402.0 ns |  1,101.36 ns |   489.01 ns |  2.18 |    0.04 |    5 |         - |          NA |
| Ips4oSort                    | 8192 | Reversed           |     5,526.8 ns |    503.32 ns |   263.24 ns |  0.15 |    0.01 |    1 |         - |          NA |
| FluxSort                     | 8192 | Reversed           |     6,738.5 ns |    264.51 ns |   138.34 ns |  0.18 |    0.00 |    1 |         - |          NA |
| CrumSort                     | 8192 | Reversed           |     6,744.2 ns |    313.96 ns |   164.21 ns |  0.18 |    0.01 |    1 |         - |          NA |
| BlitSort                     | 8192 | Reversed           |     6,699.9 ns |    240.42 ns |   125.75 ns |  0.18 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 8192 | Reversed           |    60,012.3 ns |  2,911.70 ns | 1,292.81 ns |  1.63 |    0.04 |    5 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **PipeOrgan**          | **4,741,471.4 ns** | **20,696.52 ns** | **9,189.38 ns** | **1.000** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 8192 | PipeOrgan          |   207,539.5 ns | 12,635.19 ns | 6,608.45 ns | 0.044 |    0.00 |    4 |         - |          NA |
| QuickSortMedian3             | 8192 | PipeOrgan          |   150,067.2 ns |  2,109.18 ns | 1,103.14 ns | 0.032 |    0.00 |    4 |         - |          NA |
| QuickSortMedian9             | 8192 | PipeOrgan          |    90,391.8 ns |  4,434.35 ns | 2,319.25 ns | 0.019 |    0.00 |    3 |         - |          NA |
| SampleSort                   | 8192 | PipeOrgan          |   301,743.2 ns |  2,658.11 ns | 1,180.22 ns | 0.064 |    0.00 |    4 |         - |          NA |
| SimdQuickSort                | 8192 | PipeOrgan          |    64,244.6 ns |    356.63 ns |   186.52 ns | 0.014 |    0.00 |    3 |         - |          NA |
| DualPivotQuickSort           | 8192 | PipeOrgan          |    77,610.0 ns |  2,148.15 ns |   953.79 ns | 0.016 |    0.00 |    3 |         - |          NA |
| StableQuickSort              | 8192 | PipeOrgan          |   323,939.5 ns |  2,259.95 ns | 1,182.00 ns | 0.068 |    0.00 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | PipeOrgan          |   150,502.1 ns |    764.87 ns |   339.61 ns | 0.032 |    0.00 |    4 |         - |          NA |
| DestswapStableQuickSort      | 8192 | PipeOrgan          |   375,790.3 ns |  1,312.39 ns |   582.71 ns | 0.079 |    0.00 |    4 |         - |          NA |
| IntroSort                    | 8192 | PipeOrgan          |   231,635.5 ns |  2,521.27 ns | 1,318.67 ns | 0.049 |    0.00 |    4 |         - |          NA |
| IntroSortDotnet              | 8192 | PipeOrgan          |   272,224.7 ns |  2,461.67 ns | 1,093.00 ns | 0.057 |    0.00 |    4 |         - |          NA |
| PDQSort                      | 8192 | PipeOrgan          |    68,093.4 ns |  2,288.98 ns | 1,197.18 ns | 0.014 |    0.00 |    3 |         - |          NA |
| PDQSortBranchless            | 8192 | PipeOrgan          |   101,655.1 ns |  1,023.02 ns |   535.06 ns | 0.021 |    0.00 |    3 |         - |          NA |
| Ipnsort                      | 8192 | PipeOrgan          |   185,908.2 ns |    411.69 ns |   182.79 ns | 0.039 |    0.00 |    4 |         - |          NA |
| StdSort                      | 8192 | PipeOrgan          |   248,832.4 ns |  1,677.28 ns |   877.25 ns | 0.052 |    0.00 |    4 |         - |          NA |
| BlockQuickSort               | 8192 | PipeOrgan          |   157,821.4 ns |  1,387.40 ns |   725.63 ns | 0.033 |    0.00 |    4 |         - |          NA |
| Ips4oSort                    | 8192 | PipeOrgan          |   142,279.7 ns |    516.11 ns |   229.15 ns | 0.030 |    0.00 |    4 |         - |          NA |
| FluxSort                     | 8192 | PipeOrgan          |    17,966.3 ns |    190.65 ns |    84.65 ns | 0.004 |    0.00 |    1 |         - |          NA |
| CrumSort                     | 8192 | PipeOrgan          |    22,960.8 ns |    458.02 ns |   239.55 ns | 0.005 |    0.00 |    2 |         - |          NA |
| BlitSort                     | 8192 | PipeOrgan          |    22,797.8 ns |    201.47 ns |   105.37 ns | 0.005 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 8192 | PipeOrgan          |   278,296.5 ns |  5,250.08 ns | 2,745.89 ns | 0.059 |    0.00 |    4 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **ManyDuplicates**     |   **205,891.8 ns** |  **1,927.41 ns** | **1,008.07 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 8192 | ManyDuplicates     |   146,580.9 ns |    306.87 ns |   109.43 ns |  0.71 |    0.00 |    3 |         - |          NA |
| QuickSortMedian3             | 8192 | ManyDuplicates     |   222,606.9 ns |    757.36 ns |   336.27 ns |  1.08 |    0.01 |    4 |         - |          NA |
| QuickSortMedian9             | 8192 | ManyDuplicates     |   235,575.6 ns |  2,201.36 ns | 1,151.36 ns |  1.14 |    0.01 |    4 |         - |          NA |
| SampleSort                   | 8192 | ManyDuplicates     |   311,057.4 ns |  1,923.78 ns |   854.17 ns |  1.51 |    0.01 |    5 |         - |          NA |
| SimdQuickSort                | 8192 | ManyDuplicates     |    49,496.8 ns |    136.84 ns |    71.57 ns |  0.24 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort           | 8192 | ManyDuplicates     |   143,876.1 ns |    609.91 ns |   270.80 ns |  0.70 |    0.00 |    3 |         - |          NA |
| StableQuickSort              | 8192 | ManyDuplicates     |   346,435.8 ns |  5,740.68 ns | 3,002.49 ns |  1.68 |    0.02 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | ManyDuplicates     |   152,877.9 ns |    485.00 ns |   215.34 ns |  0.74 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 8192 | ManyDuplicates     |   252,127.3 ns |  1,104.81 ns |   490.54 ns |  1.22 |    0.01 |    4 |         - |          NA |
| IntroSort                    | 8192 | ManyDuplicates     |   193,316.5 ns |    940.60 ns |   491.95 ns |  0.94 |    0.00 |    4 |         - |          NA |
| IntroSortDotnet              | 8192 | ManyDuplicates     |   207,991.5 ns |    926.25 ns |   411.26 ns |  1.01 |    0.01 |    4 |         - |          NA |
| PDQSort                      | 8192 | ManyDuplicates     |   144,150.7 ns |    728.65 ns |   381.10 ns |  0.70 |    0.00 |    3 |         - |          NA |
| PDQSortBranchless            | 8192 | ManyDuplicates     |    48,649.8 ns |    211.20 ns |   110.46 ns |  0.24 |    0.00 |    1 |         - |          NA |
| Ipnsort                      | 8192 | ManyDuplicates     |    93,578.5 ns |    411.06 ns |   182.51 ns |  0.45 |    0.00 |    2 |         - |          NA |
| StdSort                      | 8192 | ManyDuplicates     |    77,569.1 ns |    299.47 ns |   106.79 ns |  0.38 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | ManyDuplicates     |   108,799.0 ns |    171.08 ns |    61.01 ns |  0.53 |    0.00 |    2 |         - |          NA |
| Ips4oSort                    | 8192 | ManyDuplicates     |    62,140.2 ns |    168.52 ns |    74.83 ns |  0.30 |    0.00 |    1 |         - |          NA |
| FluxSort                     | 8192 | ManyDuplicates     |    57,180.1 ns |    219.93 ns |    97.65 ns |  0.28 |    0.00 |    1 |         - |          NA |
| CrumSort                     | 8192 | ManyDuplicates     |    56,548.9 ns |    379.96 ns |   198.73 ns |  0.27 |    0.00 |    1 |         - |          NA |
| BlitSort                     | 8192 | ManyDuplicates     |    79,903.5 ns |    285.03 ns |   126.56 ns |  0.39 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 8192 | ManyDuplicates     |   191,297.2 ns |  2,102.54 ns | 1,099.67 ns |  0.93 |    0.01 |    4 |         - |          NA |

### SelectionBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.49 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                  | Size | Pattern            | Mean         | Error        | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------------ |----- |------------------- |-------------:|-------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **SelectionSort**           | **256**  | **Random**             |  **26,060.6 ns** |    **314.26 ns** |   **164.37 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 256  | Random             |   9,592.4 ns |    334.12 ns |   174.75 ns |  0.37 |    0.01 |    2 |         - |          NA |
| DoubleSelectionSort     | 256  | Random             |  18,443.3 ns |     72.27 ns |    32.09 ns |  0.71 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 256  | Random             |  10,957.6 ns |    216.64 ns |   113.31 ns |  0.42 |    0.00 |    2 |         - |          NA |
| CycleSort               | 256  | Random             |  48,939.9 ns |    274.22 ns |   143.42 ns |  1.88 |    0.01 |    5 |         - |          NA |
| PancakeSort             | 256  | Random             |  32,729.4 ns |    409.54 ns |   214.20 ns |  1.26 |    0.01 |    4 |         - |          NA |
| BingoSort               | 256  | Random             |  29,224.6 ns |    265.67 ns |   138.95 ns |  1.12 |    0.01 |    4 |         - |          NA |
| DotnetSort              | 256  | Random             |   5,870.7 ns |    375.02 ns |   196.14 ns |  0.23 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **SelectionSort**           | **256**  | **SingleElementMoved** |  **19,298.6 ns** |    **161.89 ns** |    **84.67 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| SelectionSortSimd       | 256  | SingleElementMoved |   4,933.1 ns |    330.75 ns |   172.99 ns |  0.26 |    0.01 |    2 |         - |          NA |
| DoubleSelectionSort     | 256  | SingleElementMoved |  10,827.9 ns |    294.15 ns |   153.85 ns |  0.56 |    0.01 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 256  | SingleElementMoved |   4,136.2 ns |    378.03 ns |   197.72 ns |  0.21 |    0.01 |    2 |         - |          NA |
| CycleSort               | 256  | SingleElementMoved |  25,391.0 ns |    125.30 ns |    65.54 ns |  1.32 |    0.01 |    6 |         - |          NA |
| PancakeSort             | 256  | SingleElementMoved |  15,884.3 ns |    229.55 ns |   101.92 ns |  0.82 |    0.01 |    4 |         - |          NA |
| BingoSort               | 256  | SingleElementMoved |  19,572.3 ns |    196.95 ns |    87.45 ns |  1.01 |    0.01 |    5 |         - |          NA |
| DotnetSort              | 256  | SingleElementMoved |   1,132.4 ns |    159.28 ns |    83.30 ns |  0.06 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **SelectionSort**           | **256**  | **Sorted**             |  **19,142.7 ns** |    **256.57 ns** |   **134.19 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| SelectionSortSimd       | 256  | Sorted             |   3,534.5 ns |     80.29 ns |    28.63 ns |  0.18 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSort     | 256  | Sorted             |   8,143.2 ns |    273.23 ns |   142.90 ns |  0.43 |    0.01 |    4 |         - |          NA |
| DoubleSelectionSortSimd | 256  | Sorted             |   2,919.5 ns |     47.16 ns |    16.82 ns |  0.15 |    0.00 |    2 |         - |          NA |
| CycleSort               | 256  | Sorted             |  17,435.2 ns |     78.86 ns |    41.25 ns |  0.91 |    0.01 |    6 |         - |          NA |
| PancakeSort             | 256  | Sorted             |  13,116.3 ns |     44.17 ns |    19.61 ns |  0.69 |    0.00 |    5 |         - |          NA |
| BingoSort               | 256  | Sorted             |  19,124.4 ns |    174.17 ns |    91.09 ns |  1.00 |    0.01 |    6 |         - |          NA |
| DotnetSort              | 256  | Sorted             |     642.3 ns |     41.29 ns |    18.33 ns |  0.03 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **SelectionSort**           | **256**  | **Reversed**           |  **17,760.9 ns** |  **1,389.34 ns** |   **726.65 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| SelectionSortSimd       | 256  | Reversed           |   5,863.2 ns |    298.53 ns |   156.14 ns |  0.33 |    0.02 |    3 |         - |          NA |
| DoubleSelectionSort     | 256  | Reversed           |   8,146.0 ns |    283.15 ns |   148.09 ns |  0.46 |    0.02 |    4 |         - |          NA |
| DoubleSelectionSortSimd | 256  | Reversed           |   4,194.7 ns |     89.48 ns |    39.73 ns |  0.24 |    0.01 |    2 |         - |          NA |
| CycleSort               | 256  | Reversed           |  30,321.0 ns |     88.21 ns |    46.14 ns |  1.71 |    0.07 |    8 |         - |          NA |
| PancakeSort             | 256  | Reversed           |  14,255.7 ns |  1,394.54 ns |   619.19 ns |  0.80 |    0.04 |    5 |         - |          NA |
| BingoSort               | 256  | Reversed           |  22,542.4 ns |  3,015.46 ns | 1,577.15 ns |  1.27 |    0.10 |    7 |         - |          NA |
| DotnetSort              | 256  | Reversed           |   1,154.5 ns |     11.29 ns |     5.91 ns |  0.07 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **SelectionSort**           | **256**  | **PipeOrgan**          |  **18,398.9 ns** |    **565.99 ns** |   **296.02 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| SelectionSortSimd       | 256  | PipeOrgan          |   6,875.0 ns |     72.69 ns |    38.02 ns |  0.37 |    0.01 |    3 |         - |          NA |
| DoubleSelectionSort     | 256  | PipeOrgan          |   9,283.4 ns |    376.52 ns |   196.93 ns |  0.50 |    0.01 |    4 |         - |          NA |
| DoubleSelectionSortSimd | 256  | PipeOrgan          |   4,403.2 ns |    308.42 ns |   161.31 ns |  0.24 |    0.01 |    2 |         - |          NA |
| CycleSort               | 256  | PipeOrgan          |  42,885.8 ns |    235.12 ns |   122.97 ns |  2.33 |    0.04 |    6 |         - |          NA |
| PancakeSort             | 256  | PipeOrgan          |  20,523.3 ns |    195.03 ns |   102.01 ns |  1.12 |    0.02 |    5 |         - |          NA |
| BingoSort               | 256  | PipeOrgan          |  20,681.1 ns |    399.64 ns |   209.02 ns |  1.12 |    0.02 |    5 |         - |          NA |
| DotnetSort              | 256  | PipeOrgan          |   1,923.0 ns |    378.48 ns |   197.95 ns |  0.10 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **SelectionSort**           | **256**  | **ManyDuplicates**     |  **24,420.4 ns** |    **791.59 ns** |   **414.02 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 256  | ManyDuplicates     |   9,248.8 ns |    290.18 ns |   128.84 ns |  0.38 |    0.01 |    2 |         - |          NA |
| DoubleSelectionSort     | 256  | ManyDuplicates     |  19,503.6 ns |    354.06 ns |   185.18 ns |  0.80 |    0.01 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 256  | ManyDuplicates     |  10,739.6 ns |    158.17 ns |    70.23 ns |  0.44 |    0.01 |    2 |         - |          NA |
| CycleSort               | 256  | ManyDuplicates     |  49,949.3 ns |    130.75 ns |    58.05 ns |  2.05 |    0.03 |    6 |         - |          NA |
| PancakeSort             | 256  | ManyDuplicates     |  30,542.9 ns |    125.42 ns |    55.69 ns |  1.25 |    0.02 |    5 |         - |          NA |
| BingoSort               | 256  | ManyDuplicates     |   6,132.6 ns |    323.31 ns |   169.10 ns |  0.25 |    0.01 |    1 |         - |          NA |
| DotnetSort              | 256  | ManyDuplicates     |   5,560.5 ns |    431.46 ns |   225.66 ns |  0.23 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **SelectionSort**           | **1024** | **Random**             | **326,342.0 ns** |    **825.26 ns** |   **366.42 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 1024 | Random             |  58,328.6 ns |    283.78 ns |   148.42 ns |  0.18 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 1024 | Random             | 194,406.6 ns |    479.06 ns |   212.71 ns |  0.60 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 1024 | Random             |  58,565.1 ns |    434.66 ns |   192.99 ns |  0.18 |    0.00 |    2 |         - |          NA |
| CycleSort               | 1024 | Random             | 772,464.5 ns |  1,020.55 ns |   533.77 ns |  2.37 |    0.00 |    5 |         - |          NA |
| PancakeSort             | 1024 | Random             | 376,655.4 ns |  2,889.12 ns | 1,511.06 ns |  1.15 |    0.00 |    4 |         - |          NA |
| BingoSort               | 1024 | Random             | 340,111.5 ns |    711.74 ns |   316.02 ns |  1.04 |    0.00 |    4 |         - |          NA |
| DotnetSort              | 1024 | Random             |  29,852.8 ns |    421.09 ns |   220.24 ns |  0.09 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **SelectionSort**           | **1024** | **SingleElementMoved** | **292,507.7 ns** |    **666.19 ns** |   **295.79 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| SelectionSortSimd       | 1024 | SingleElementMoved |  33,052.9 ns |    682.86 ns |   357.15 ns |  0.11 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSort     | 1024 | SingleElementMoved | 144,972.2 ns | 13,144.05 ns | 6,874.59 ns |  0.50 |    0.02 |    4 |         - |          NA |
| DoubleSelectionSortSimd | 1024 | SingleElementMoved |  22,318.0 ns |    448.39 ns |   234.51 ns |  0.08 |    0.00 |    2 |         - |          NA |
| CycleSort               | 1024 | SingleElementMoved | 390,257.7 ns |  1,250.43 ns |   654.00 ns |  1.33 |    0.00 |    7 |         - |          NA |
| PancakeSort             | 1024 | SingleElementMoved | 239,791.1 ns | 14,321.93 ns | 7,490.65 ns |  0.82 |    0.02 |    5 |         - |          NA |
| BingoSort               | 1024 | SingleElementMoved | 293,220.3 ns |    283.37 ns |   101.05 ns |  1.00 |    0.00 |    6 |         - |          NA |
| DotnetSort              | 1024 | SingleElementMoved |   5,190.5 ns |     69.81 ns |    31.00 ns |  0.02 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **SelectionSort**           | **1024** | **Sorted**             | **291,213.9 ns** |    **678.20 ns** |   **241.85 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| SelectionSortSimd       | 1024 | Sorted             |  25,101.4 ns |    358.40 ns |   127.81 ns |  0.09 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSort     | 1024 | Sorted             | 124,301.0 ns |    640.17 ns |   284.24 ns |  0.43 |    0.00 |    4 |         - |          NA |
| DoubleSelectionSortSimd | 1024 | Sorted             |  16,391.8 ns |    304.13 ns |   135.03 ns |  0.06 |    0.00 |    2 |         - |          NA |
| CycleSort               | 1024 | Sorted             | 263,020.2 ns |  1,465.39 ns |   650.64 ns |  0.90 |    0.00 |    6 |         - |          NA |
| PancakeSort             | 1024 | Sorted             | 196,904.3 ns |    837.54 ns |   298.67 ns |  0.68 |    0.00 |    5 |         - |          NA |
| BingoSort               | 1024 | Sorted             | 292,337.1 ns |  1,308.81 ns |   581.12 ns |  1.00 |    0.00 |    6 |         - |          NA |
| DotnetSort              | 1024 | Sorted             |   3,138.0 ns |     33.81 ns |    12.06 ns |  0.01 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **SelectionSort**           | **1024** | **Reversed**           | **265,386.8 ns** | **21,534.24 ns** | **9,561.33 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| SelectionSortSimd       | 1024 | Reversed           |  39,321.3 ns |    187.90 ns |    83.43 ns |  0.15 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 1024 | Reversed           | 124,663.4 ns |    363.63 ns |   129.67 ns |  0.47 |    0.02 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 1024 | Reversed           |  38,598.8 ns |    452.58 ns |   236.71 ns |  0.15 |    0.00 |    2 |         - |          NA |
| CycleSort               | 1024 | Reversed           | 461,216.0 ns |  1,902.97 ns |   995.29 ns |  1.74 |    0.06 |    7 |         - |          NA |
| PancakeSort             | 1024 | Reversed           | 215,572.7 ns | 17,334.72 ns | 9,066.39 ns |  0.81 |    0.04 |    4 |         - |          NA |
| BingoSort               | 1024 | Reversed           | 341,912.1 ns | 11,777.87 ns | 6,160.05 ns |  1.29 |    0.05 |    6 |         - |          NA |
| DotnetSort              | 1024 | Reversed           |   5,782.4 ns |    966.97 ns |   505.74 ns |  0.02 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **SelectionSort**           | **1024** | **PipeOrgan**          | **302,081.5 ns** |  **2,341.35 ns** | **1,224.57 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| SelectionSortSimd       | 1024 | PipeOrgan          |  49,564.8 ns |    265.98 ns |   118.10 ns |  0.16 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSort     | 1024 | PipeOrgan          | 154,325.2 ns |    736.11 ns |   326.84 ns |  0.51 |    0.00 |    4 |         - |          NA |
| DoubleSelectionSortSimd | 1024 | PipeOrgan          |  30,354.8 ns |    220.17 ns |   115.15 ns |  0.10 |    0.00 |    2 |         - |          NA |
| CycleSort               | 1024 | PipeOrgan          | 678,585.0 ns |  2,876.12 ns | 1,277.02 ns |  2.25 |    0.01 |    6 |         - |          NA |
| PancakeSort             | 1024 | PipeOrgan          | 314,135.3 ns |  1,298.63 ns |   679.21 ns |  1.04 |    0.00 |    5 |         - |          NA |
| BingoSort               | 1024 | PipeOrgan          | 332,607.9 ns |  3,902.99 ns | 2,041.34 ns |  1.10 |    0.01 |    5 |         - |          NA |
| DotnetSort              | 1024 | PipeOrgan          |  12,901.2 ns |    406.03 ns |   180.28 ns |  0.04 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **SelectionSort**           | **1024** | **ManyDuplicates**     | **313,366.3 ns** |  **1,277.30 ns** |   **567.13 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 1024 | ManyDuplicates     |  56,575.4 ns |    238.88 ns |   124.94 ns |  0.18 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 1024 | ManyDuplicates     | 213,792.0 ns |  1,867.57 ns |   829.21 ns |  0.68 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 1024 | ManyDuplicates     |  57,504.3 ns |    329.76 ns |   172.47 ns |  0.18 |    0.00 |    2 |         - |          NA |
| CycleSort               | 1024 | ManyDuplicates     | 765,560.9 ns |    801.72 ns |   419.32 ns |  2.44 |    0.00 |    5 |         - |          NA |
| PancakeSort             | 1024 | ManyDuplicates     | 357,405.9 ns |    826.38 ns |   366.92 ns |  1.14 |    0.00 |    4 |         - |          NA |
| BingoSort               | 1024 | ManyDuplicates     |  20,333.8 ns |    277.48 ns |   145.13 ns |  0.06 |    0.00 |    1 |         - |          NA |
| DotnetSort              | 1024 | ManyDuplicates     |  23,289.6 ns |    395.57 ns |   206.89 ns |  0.07 |    0.00 |    1 |         - |          NA |

### TreeBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.49 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                 | Size | Pattern            | Mean            | Error         | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------- |----- |------------------- |----------------:|--------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **BalancedBinaryTreeSort** | **256**  | **Random**             |     **16,320.8 ns** |     **344.66 ns** |    **153.03 ns** |  **1.38** |    **0.02** |    **2** |         **-** |          **NA** |
| BTreeSort              | 256  | Random             |     15,914.7 ns |     145.22 ns |     75.95 ns |  1.35 |    0.01 |    2 |         - |          NA |
| BPlusTreeSort          | 256  | Random             |     15,523.9 ns |     277.74 ns |    145.26 ns |  1.32 |    0.02 |    2 |         - |          NA |
| CartesianTreeSort      | 256  | Random             |     12,998.7 ns |     278.21 ns |    145.51 ns |  1.10 |    0.02 |    2 |         - |          NA |
| BinaryTreeSort         | 256  | Random             |     11,787.2 ns |     208.59 ns |    109.10 ns |  1.00 |    0.00 |    2 |         - |          NA |
| SplaySort              | 256  | Random             |     30,495.2 ns |     578.91 ns |    302.78 ns |  2.59 |    0.03 |    3 |         - |          NA |
| TreapSort              | 256  | Random             |     17,896.9 ns |     200.82 ns |    105.03 ns |  1.52 |    0.02 |    2 |         - |          NA |
| DotnetSort             | 256  | Random             |      5,913.6 ns |     489.41 ns |    255.97 ns |  0.50 |    0.02 |    1 |         - |          NA |
|      |                    |                 |               |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **SingleElementMoved** |      **6,517.0 ns** |      **78.54 ns** |     **41.08 ns** |  **0.12** |    **0.00** |    **5** |         **-** |          **NA** |
| BTreeSort              | 256  | SingleElementMoved |      4,748.6 ns |     240.37 ns |    125.72 ns |  0.09 |    0.00 |    4 |         - |          NA |
| BPlusTreeSort          | 256  | SingleElementMoved |      4,413.7 ns |     318.31 ns |    166.48 ns |  0.08 |    0.00 |    4 |         - |          NA |
| CartesianTreeSort      | 256  | SingleElementMoved |      1,597.3 ns |       5.77 ns |      2.56 ns |  0.03 |    0.00 |    2 |         - |          NA |
| BinaryTreeSort         | 256  | SingleElementMoved |     53,651.2 ns |      91.23 ns |     32.53 ns |  1.00 |    0.00 |    6 |         - |          NA |
| SplaySort              | 256  | SingleElementMoved |      3,338.2 ns |      10.83 ns |      3.86 ns |  0.06 |    0.00 |    3 |         - |          NA |
| TreapSort              | 256  | SingleElementMoved |      4,665.9 ns |     492.54 ns |    218.69 ns |  0.09 |    0.00 |    4 |         - |          NA |
| DotnetSort             | 256  | SingleElementMoved |      1,147.1 ns |     249.16 ns |    110.63 ns |  0.02 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Sorted**             |      **6,228.9 ns** |     **201.82 ns** |    **105.56 ns** |  **0.11** |    **0.00** |    **5** |         **-** |          **NA** |
| BTreeSort              | 256  | Sorted             |      4,428.0 ns |      58.27 ns |     20.78 ns |  0.08 |    0.00 |    4 |         - |          NA |
| BPlusTreeSort          | 256  | Sorted             |      4,004.6 ns |      36.29 ns |     16.11 ns |  0.07 |    0.00 |    4 |         - |          NA |
| CartesianTreeSort      | 256  | Sorted             |      1,540.5 ns |      13.28 ns |      5.90 ns |  0.03 |    0.00 |    2 |         - |          NA |
| BinaryTreeSort         | 256  | Sorted             |     58,564.0 ns |     249.73 ns |    130.62 ns |  1.00 |    0.00 |    6 |         - |          NA |
| SplaySort              | 256  | Sorted             |      2,976.4 ns |      19.18 ns |      6.84 ns |  0.05 |    0.00 |    3 |         - |          NA |
| TreapSort              | 256  | Sorted             |      3,922.7 ns |     144.35 ns |     75.50 ns |  0.07 |    0.00 |    4 |         - |          NA |
| DotnetSort             | 256  | Sorted             |        655.4 ns |      12.60 ns |      6.59 ns |  0.01 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Reversed**           |      **6,214.3 ns** |     **207.19 ns** |    **108.37 ns** |  **0.10** |    **0.00** |    **5** |         **-** |          **NA** |
| BTreeSort              | 256  | Reversed           |      7,245.3 ns |      62.65 ns |     32.77 ns |  0.12 |    0.00 |    5 |         - |          NA |
| BPlusTreeSort          | 256  | Reversed           |      6,646.4 ns |     243.24 ns |    127.22 ns |  0.11 |    0.00 |    5 |         - |          NA |
| CartesianTreeSort      | 256  | Reversed           |      1,509.2 ns |       7.96 ns |      2.84 ns |  0.02 |    0.00 |    2 |         - |          NA |
| BinaryTreeSort         | 256  | Reversed           |     61,802.8 ns |     269.88 ns |    141.15 ns |  1.00 |    0.00 |    6 |         - |          NA |
| SplaySort              | 256  | Reversed           |      2,975.4 ns |     243.72 ns |    127.47 ns |  0.05 |    0.00 |    3 |         - |          NA |
| TreapSort              | 256  | Reversed           |      3,846.3 ns |     155.44 ns |     81.30 ns |  0.06 |    0.00 |    4 |         - |          NA |
| DotnetSort             | 256  | Reversed           |      1,109.2 ns |     128.56 ns |     67.24 ns |  0.02 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **PipeOrgan**          |      **5,235.5 ns** |     **261.84 ns** |    **136.95 ns** |  **0.18** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 256  | PipeOrgan          |      5,376.9 ns |     367.70 ns |    192.31 ns |  0.18 |    0.01 |    4 |         - |          NA |
| BPlusTreeSort          | 256  | PipeOrgan          |      4,866.1 ns |     343.96 ns |    179.90 ns |  0.17 |    0.01 |    4 |         - |          NA |
| CartesianTreeSort      | 256  | PipeOrgan          |      1,677.2 ns |       7.49 ns |      3.92 ns |  0.06 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | PipeOrgan          |     29,185.7 ns |     292.56 ns |    153.01 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 256  | PipeOrgan          |      3,401.7 ns |      20.66 ns |      7.37 ns |  0.12 |    0.00 |    3 |         - |          NA |
| TreapSort              | 256  | PipeOrgan          |      5,370.6 ns |     334.78 ns |    175.09 ns |  0.18 |    0.01 |    4 |         - |          NA |
| DotnetSort             | 256  | PipeOrgan          |      1,972.2 ns |     339.83 ns |    177.74 ns |  0.07 |    0.01 |    2 |         - |          NA |
|      |                    |                 |               |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **ManyDuplicates**     |     **15,646.4 ns** |      **70.69 ns** |     **31.39 ns** |  **1.49** |    **0.02** |    **3** |         **-** |          **NA** |
| BTreeSort              | 256  | ManyDuplicates     |     15,384.7 ns |     140.19 ns |     62.24 ns |  1.46 |    0.02 |    3 |         - |          NA |
| BPlusTreeSort          | 256  | ManyDuplicates     |     14,756.9 ns |     359.07 ns |    187.80 ns |  1.40 |    0.02 |    3 |         - |          NA |
| CartesianTreeSort      | 256  | ManyDuplicates     |     15,576.8 ns |     168.17 ns |     87.96 ns |  1.48 |    0.02 |    3 |         - |          NA |
| BinaryTreeSort         | 256  | ManyDuplicates     |     10,509.9 ns |     219.94 ns |    115.03 ns |  1.00 |    0.00 |    2 |         - |          NA |
| SplaySort              | 256  | ManyDuplicates     |     28,613.9 ns |     530.70 ns |    277.57 ns |  2.72 |    0.04 |    4 |         - |          NA |
| TreapSort              | 256  | ManyDuplicates     |     17,734.1 ns |     259.28 ns |    115.12 ns |  1.69 |    0.02 |    3 |         - |          NA |
| DotnetSort             | 256  | ManyDuplicates     |      5,586.0 ns |     434.70 ns |    227.36 ns |  0.53 |    0.02 |    1 |         - |          NA |
|      |                    |                 |               |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Random**             |     **74,571.3 ns** |   **1,146.65 ns** |    **599.72 ns** |  **1.36** |    **0.01** |    **2** |         **-** |          **NA** |
| BTreeSort              | 1024 | Random             |     75,327.9 ns |     654.09 ns |    342.10 ns |  1.37 |    0.01 |    2 |         - |          NA |
| BPlusTreeSort          | 1024 | Random             |     73,089.7 ns |     453.87 ns |    201.52 ns |  1.33 |    0.00 |    2 |         - |          NA |
| CartesianTreeSort      | 1024 | Random             |     64,434.9 ns |     619.81 ns |    275.20 ns |  1.17 |    0.00 |    2 |         - |          NA |
| BinaryTreeSort         | 1024 | Random             |     54,880.0 ns |     132.63 ns |     69.37 ns |  1.00 |    0.00 |    2 |         - |          NA |
| SplaySort              | 1024 | Random             |    156,892.6 ns |     848.21 ns |    376.61 ns |  2.86 |    0.01 |    3 |         - |          NA |
| TreapSort              | 1024 | Random             |     81,339.3 ns |     596.09 ns |    264.67 ns |  1.48 |    0.00 |    2 |         - |          NA |
| DotnetSort             | 1024 | Random             |     29,990.3 ns |     349.42 ns |    182.76 ns |  0.55 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **SingleElementMoved** |     **28,287.5 ns** |     **275.48 ns** |    **144.08 ns** | **0.033** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 1024 | SingleElementMoved |     21,313.6 ns |     572.04 ns |    299.19 ns | 0.025 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 1024 | SingleElementMoved |     19,598.6 ns |     181.76 ns |     80.70 ns | 0.023 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 1024 | SingleElementMoved |      6,154.8 ns |     325.14 ns |    170.06 ns | 0.007 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | SingleElementMoved |    852,032.1 ns |   1,199.26 ns |    627.24 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | SingleElementMoved |     13,185.6 ns |     234.14 ns |    103.96 ns | 0.015 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | SingleElementMoved |     20,034.1 ns |     905.49 ns |    473.59 ns | 0.024 |    0.00 |    3 |         - |          NA |
| DotnetSort             | 1024 | SingleElementMoved |      5,330.2 ns |     324.00 ns |    143.86 ns | 0.006 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Sorted**             |     **27,466.8 ns** |     **374.42 ns** |    **195.83 ns** | **0.029** |    **0.00** |    **5** |         **-** |          **NA** |
| BTreeSort              | 1024 | Sorted             |     20,337.4 ns |     122.12 ns |     54.22 ns | 0.022 |    0.00 |    4 |         - |          NA |
| BPlusTreeSort          | 1024 | Sorted             |     18,993.6 ns |      97.84 ns |     51.17 ns | 0.020 |    0.00 |    4 |         - |          NA |
| CartesianTreeSort      | 1024 | Sorted             |      6,063.8 ns |     294.86 ns |    154.22 ns | 0.006 |    0.00 |    2 |         - |          NA |
| BinaryTreeSort         | 1024 | Sorted             |    933,170.8 ns |     981.28 ns |    435.70 ns | 1.000 |    0.00 |    6 |         - |          NA |
| SplaySort              | 1024 | Sorted             |     11,850.7 ns |     198.45 ns |     88.11 ns | 0.013 |    0.00 |    3 |         - |          NA |
| TreapSort              | 1024 | Sorted             |     17,946.9 ns |     500.26 ns |    261.64 ns | 0.019 |    0.00 |    4 |         - |          NA |
| DotnetSort             | 1024 | Sorted             |      3,148.6 ns |      55.17 ns |     19.68 ns | 0.003 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Reversed**           |     **26,675.2 ns** |     **365.13 ns** |    **162.12 ns** | **0.027** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 1024 | Reversed           |     31,343.6 ns |     127.99 ns |     56.83 ns | 0.032 |    0.00 |    4 |         - |          NA |
| BPlusTreeSort          | 1024 | Reversed           |     28,928.2 ns |     259.12 ns |    135.53 ns | 0.029 |    0.00 |    4 |         - |          NA |
| CartesianTreeSort      | 1024 | Reversed           |      5,966.4 ns |     245.88 ns |    128.60 ns | 0.006 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | Reversed           |    992,088.4 ns |   1,284.47 ns |    671.80 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | Reversed           |     11,564.8 ns |     332.03 ns |    147.42 ns | 0.012 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | Reversed           |     17,332.3 ns |     461.12 ns |    241.18 ns | 0.017 |    0.00 |    3 |         - |          NA |
| DotnetSort             | 1024 | Reversed           |      5,937.2 ns |   1,123.52 ns |    587.62 ns | 0.006 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **PipeOrgan**          |     **23,731.3 ns** |     **461.95 ns** |    **241.61 ns** |  **0.05** |    **0.00** |    **3** |         **-** |          **NA** |
| BTreeSort              | 1024 | PipeOrgan          |     23,437.1 ns |     500.71 ns |    261.88 ns |  0.05 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 1024 | PipeOrgan          |     21,917.1 ns |     310.48 ns |    162.39 ns |  0.05 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 1024 | PipeOrgan          |      6,626.3 ns |     270.07 ns |    141.25 ns |  0.01 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | PipeOrgan          |    468,491.0 ns |     454.93 ns |    201.99 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | PipeOrgan          |     13,395.4 ns |     224.55 ns |    117.44 ns |  0.03 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | PipeOrgan          |     24,586.2 ns |     603.90 ns |    315.85 ns |  0.05 |    0.00 |    3 |         - |          NA |
| DotnetSort             | 1024 | PipeOrgan          |     12,895.0 ns |     474.05 ns |    210.48 ns |  0.03 |    0.00 |    2 |         - |          NA |
|      |                    |                 |               |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **ManyDuplicates**     |     **64,886.3 ns** |     **624.79 ns** |    **326.77 ns** |  **1.12** |    **0.01** |    **2** |         **-** |          **NA** |
| BTreeSort              | 1024 | ManyDuplicates     |     68,252.0 ns |     200.40 ns |    104.81 ns |  1.17 |    0.00 |    2 |         - |          NA |
| BPlusTreeSort          | 1024 | ManyDuplicates     |     65,454.4 ns |     238.47 ns |    105.88 ns |  1.13 |    0.00 |    2 |         - |          NA |
| CartesianTreeSort      | 1024 | ManyDuplicates     |     76,560.9 ns |     388.10 ns |    138.40 ns |  1.32 |    0.00 |    2 |         - |          NA |
| BinaryTreeSort         | 1024 | ManyDuplicates     |     58,156.4 ns |     443.02 ns |    196.71 ns |  1.00 |    0.00 |    2 |         - |          NA |
| SplaySort              | 1024 | ManyDuplicates     |    123,647.6 ns |   1,724.40 ns |    901.89 ns |  2.13 |    0.02 |    3 |         - |          NA |
| TreapSort              | 1024 | ManyDuplicates     |     78,405.5 ns |     775.39 ns |    344.28 ns |  1.35 |    0.01 |    2 |         - |          NA |
| DotnetSort             | 1024 | ManyDuplicates     |     23,266.1 ns |     289.10 ns |    151.21 ns |  0.40 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **4096** | **Random**             |    **345,232.2 ns** |   **2,807.41 ns** |  **1,246.51 ns** |  **1.33** |    **0.00** |    **3** |         **-** |          **NA** |
| BTreeSort              | 4096 | Random             |    356,379.4 ns |   1,365.78 ns |    606.42 ns |  1.37 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 4096 | Random             |    347,371.3 ns |   1,543.07 ns |    807.05 ns |  1.34 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 4096 | Random             |    323,132.6 ns |   1,371.05 ns |    717.09 ns |  1.24 |    0.00 |    3 |         - |          NA |
| BinaryTreeSort         | 4096 | Random             |    260,002.9 ns |     847.22 ns |    443.11 ns |  1.00 |    0.00 |    2 |         - |          NA |
| SplaySort              | 4096 | Random             |    776,419.8 ns |   2,395.66 ns |  1,252.98 ns |  2.99 |    0.01 |    4 |         - |          NA |
| TreapSort              | 4096 | Random             |    387,533.1 ns |   1,967.24 ns |  1,028.90 ns |  1.49 |    0.00 |    3 |         - |          NA |
| DotnetSort             | 4096 | Random             |    144,510.9 ns |   1,357.15 ns |    602.58 ns |  0.56 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **4096** | **SingleElementMoved** |    **127,063.0 ns** |     **770.55 ns** |    **342.13 ns** | **0.009** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 4096 | SingleElementMoved |    102,528.8 ns |     429.54 ns |    190.72 ns | 0.008 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 4096 | SingleElementMoved |     94,818.7 ns |     358.45 ns |    159.15 ns | 0.007 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 4096 | SingleElementMoved |     24,294.9 ns |     323.70 ns |    169.30 ns | 0.002 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 4096 | SingleElementMoved | 13,518,903.0 ns |  21,244.36 ns | 11,111.21 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 4096 | SingleElementMoved |     51,888.3 ns |     298.51 ns |    156.13 ns | 0.004 |    0.00 |    2 |         - |          NA |
| TreapSort              | 4096 | SingleElementMoved |     87,683.5 ns |   5,021.66 ns |  2,626.43 ns | 0.006 |    0.00 |    3 |         - |          NA |
| DotnetSort             | 4096 | SingleElementMoved |     24,520.0 ns |     398.72 ns |    208.54 ns | 0.002 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **4096** | **Sorted**             |    **123,264.0 ns** |   **1,755.37 ns** |    **625.98 ns** | **0.008** |    **0.00** |    **6** |         **-** |          **NA** |
| BTreeSort              | 4096 | Sorted             |     98,893.7 ns |     630.84 ns |    280.09 ns | 0.007 |    0.00 |    5 |         - |          NA |
| BPlusTreeSort          | 4096 | Sorted             |     91,479.5 ns |     222.61 ns |     98.84 ns | 0.006 |    0.00 |    5 |         - |          NA |
| CartesianTreeSort      | 4096 | Sorted             |     23,947.7 ns |     414.32 ns |    216.70 ns | 0.002 |    0.00 |    2 |         - |          NA |
| BinaryTreeSort         | 4096 | Sorted             | 14,869,875.3 ns |  23,309.19 ns | 12,191.16 ns | 1.000 |    0.00 |    7 |         - |          NA |
| SplaySort              | 4096 | Sorted             |     47,568.9 ns |     214.86 ns |    112.37 ns | 0.003 |    0.00 |    3 |         - |          NA |
| TreapSort              | 4096 | Sorted             |     74,740.2 ns |   2,472.89 ns |  1,097.98 ns | 0.005 |    0.00 |    4 |         - |          NA |
| DotnetSort             | 4096 | Sorted             |     15,013.5 ns |      86.54 ns |     38.42 ns | 0.001 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **4096** | **Reversed**           |    **120,993.9 ns** |   **1,743.01 ns** |    **911.63 ns** | **0.008** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 4096 | Reversed           |    137,502.7 ns |     885.28 ns |    463.02 ns | 0.009 |    0.00 |    4 |         - |          NA |
| BPlusTreeSort          | 4096 | Reversed           |    127,489.4 ns |     756.30 ns |    335.80 ns | 0.008 |    0.00 |    4 |         - |          NA |
| CartesianTreeSort      | 4096 | Reversed           |     23,490.7 ns |     250.39 ns |    111.18 ns | 0.001 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 4096 | Reversed           | 15,946,875.1 ns |  11,061.87 ns |  4,911.54 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 4096 | Reversed           |     45,657.1 ns |     341.47 ns |    151.61 ns | 0.003 |    0.00 |    2 |         - |          NA |
| TreapSort              | 4096 | Reversed           |     73,855.6 ns |   2,298.46 ns |  1,020.53 ns | 0.005 |    0.00 |    3 |         - |          NA |
| DotnetSort             | 4096 | Reversed           |     28,175.3 ns |   2,420.82 ns |  1,266.14 ns | 0.002 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **4096** | **PipeOrgan**          |    **113,144.5 ns** |   **4,675.63 ns** |  **2,445.45 ns** | **0.015** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 4096 | PipeOrgan          |    112,962.5 ns |   5,497.62 ns |  2,875.36 ns | 0.015 |    0.00 |    4 |         - |          NA |
| BPlusTreeSort          | 4096 | PipeOrgan          |    105,852.4 ns |   1,101.28 ns |    575.99 ns | 0.014 |    0.00 |    4 |         - |          NA |
| CartesianTreeSort      | 4096 | PipeOrgan          |     26,101.4 ns |     384.18 ns |    200.93 ns | 0.004 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 4096 | PipeOrgan          |  7,419,707.2 ns |   7,274.20 ns |  3,229.79 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 4096 | PipeOrgan          |     53,062.4 ns |     132.33 ns |     58.76 ns | 0.007 |    0.00 |    2 |         - |          NA |
| TreapSort              | 4096 | PipeOrgan          |    117,243.0 ns |   4,705.66 ns |  2,461.15 ns | 0.016 |    0.00 |    4 |         - |          NA |
| DotnetSort             | 4096 | PipeOrgan          |     71,943.3 ns |   1,975.55 ns |  1,033.25 ns | 0.010 |    0.00 |    3 |         - |          NA |
|      |                    |                 |               |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **4096** | **ManyDuplicates**     |    **261,788.5 ns** |   **3,343.86 ns** |  **1,748.90 ns** |  **0.45** |    **0.00** |    **2** |         **-** |          **NA** |
| BTreeSort              | 4096 | ManyDuplicates     |    291,405.7 ns |   1,725.94 ns |    766.33 ns |  0.50 |    0.00 |    2 |         - |          NA |
| BPlusTreeSort          | 4096 | ManyDuplicates     |    278,604.1 ns |   1,673.53 ns |    875.29 ns |  0.48 |    0.00 |    2 |         - |          NA |
| CartesianTreeSort      | 4096 | ManyDuplicates     |    365,569.5 ns |     797.40 ns |    284.36 ns |  0.63 |    0.00 |    2 |         - |          NA |
| BinaryTreeSort         | 4096 | ManyDuplicates     |    579,338.6 ns |   1,296.01 ns |    575.44 ns |  1.00 |    0.00 |    3 |         - |          NA |
| SplaySort              | 4096 | ManyDuplicates     |    504,555.9 ns |   1,664.72 ns |    870.68 ns |  0.87 |    0.00 |    3 |         - |          NA |
| TreapSort              | 4096 | ManyDuplicates     |    340,196.3 ns |   3,189.02 ns |  1,415.95 ns |  0.59 |    0.00 |    2 |         - |          NA |
| DotnetSort             | 4096 | ManyDuplicates     |     94,502.6 ns |     975.12 ns |    510.00 ns |  0.16 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **8192** | **Random**             |    **755,244.2 ns** |   **4,778.58 ns** |  **2,121.72 ns** |  **1.29** |    **0.01** |    **3** |         **-** |          **NA** |
| BTreeSort              | 8192 | Random             |    778,980.4 ns |   3,644.05 ns |  1,905.91 ns |  1.33 |    0.01 |    3 |         - |          NA |
| BPlusTreeSort          | 8192 | Random             |    747,725.2 ns |   1,901.44 ns |    844.25 ns |  1.27 |    0.01 |    3 |         - |          NA |
| CartesianTreeSort      | 8192 | Random             |    718,848.2 ns |   3,811.69 ns |  1,692.42 ns |  1.22 |    0.01 |    3 |         - |          NA |
| BinaryTreeSort         | 8192 | Random             |    586,931.9 ns |   6,049.33 ns |  3,163.92 ns |  1.00 |    0.00 |    2 |         - |          NA |
| SplaySort              | 8192 | Random             |  1,734,815.2 ns |   5,029.78 ns |  2,233.26 ns |  2.96 |    0.02 |    4 |         - |          NA |
| TreapSort              | 8192 | Random             |    870,341.7 ns |     984.05 ns |    436.93 ns |  1.48 |    0.01 |    3 |         - |          NA |
| DotnetSort             | 8192 | Random             |    315,378.0 ns |   2,154.37 ns |  1,126.78 ns |  0.54 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **8192** | **SingleElementMoved** |    **265,494.4 ns** |   **2,364.01 ns** |  **1,049.64 ns** | **0.005** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 8192 | SingleElementMoved |    214,250.5 ns |   1,114.90 ns |    495.02 ns | 0.004 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 8192 | SingleElementMoved |    202,025.8 ns |     523.13 ns |    232.27 ns | 0.004 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 8192 | SingleElementMoved |     48,350.3 ns |     343.68 ns |    152.60 ns | 0.001 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 8192 | SingleElementMoved | 53,864,160.1 ns |  47,320.48 ns | 21,010.59 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 8192 | SingleElementMoved |    103,317.7 ns |     623.60 ns |    276.88 ns | 0.002 |    0.00 |    2 |         - |          NA |
| TreapSort              | 8192 | SingleElementMoved |    190,195.6 ns |   5,301.89 ns |  2,772.99 ns | 0.004 |    0.00 |    3 |         - |          NA |
| DotnetSort             | 8192 | SingleElementMoved |     53,422.8 ns |   2,191.49 ns |  1,146.19 ns | 0.001 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **8192** | **Sorted**             |    **258,318.9 ns** |   **1,206.21 ns** |    **535.57 ns** | **0.004** |    **0.00** |    **6** |         **-** |          **NA** |
| BTreeSort              | 8192 | Sorted             |    207,437.7 ns |     561.73 ns |    249.41 ns | 0.004 |    0.00 |    5 |         - |          NA |
| BPlusTreeSort          | 8192 | Sorted             |    195,881.1 ns |     852.85 ns |    378.67 ns | 0.003 |    0.00 |    5 |         - |          NA |
| CartesianTreeSort      | 8192 | Sorted             |     47,517.3 ns |     227.20 ns |    100.88 ns | 0.001 |    0.00 |    2 |         - |          NA |
| BinaryTreeSort         | 8192 | Sorted             | 58,709,738.7 ns |   5,873.03 ns |  3,071.71 ns | 1.000 |    0.00 |    7 |         - |          NA |
| SplaySort              | 8192 | Sorted             |     95,233.3 ns |     891.55 ns |    395.85 ns | 0.002 |    0.00 |    3 |         - |          NA |
| TreapSort              | 8192 | Sorted             |    159,502.5 ns |  16,803.01 ns |  8,788.30 ns | 0.003 |    0.00 |    4 |         - |          NA |
| DotnetSort             | 8192 | Sorted             |     33,539.2 ns |   3,137.32 ns |  1,640.88 ns | 0.001 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **8192** | **Reversed**           |    **258,897.9 ns** |   **2,140.16 ns** |  **1,119.34 ns** | **0.004** |    **0.00** |    **5** |         **-** |          **NA** |
| BTreeSort              | 8192 | Reversed           |    284,498.0 ns |   1,998.81 ns |    887.48 ns | 0.004 |    0.00 |    5 |         - |          NA |
| BPlusTreeSort          | 8192 | Reversed           |    261,066.7 ns |   1,313.26 ns |    583.09 ns | 0.004 |    0.00 |    5 |         - |          NA |
| CartesianTreeSort      | 8192 | Reversed           |     46,676.7 ns |      97.11 ns |     43.12 ns | 0.001 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 8192 | Reversed           | 63,553,675.5 ns |  17,097.32 ns |  7,591.32 ns | 1.000 |    0.00 |    6 |         - |          NA |
| SplaySort              | 8192 | Reversed           |     91,172.6 ns |   1,199.37 ns |    627.29 ns | 0.001 |    0.00 |    3 |         - |          NA |
| TreapSort              | 8192 | Reversed           |    152,453.6 ns |   7,341.90 ns |  3,839.96 ns | 0.002 |    0.00 |    4 |         - |          NA |
| DotnetSort             | 8192 | Reversed           |     59,208.4 ns |   3,880.89 ns |  2,029.78 ns | 0.001 |    0.00 |    2 |         - |          NA |
|      |                    |                 |               |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **8192** | **PipeOrgan**          |    **275,429.9 ns** |  **15,682.65 ns** |  **8,202.33 ns** | **0.009** |    **0.00** |    **3** |         **-** |          **NA** |
| BTreeSort              | 8192 | PipeOrgan          |    261,550.1 ns |   9,441.93 ns |  4,938.31 ns | 0.009 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 8192 | PipeOrgan          |    239,873.9 ns |   5,064.11 ns |  2,248.50 ns | 0.008 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 8192 | PipeOrgan          |     52,253.2 ns |     489.24 ns |    255.88 ns | 0.002 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 8192 | PipeOrgan          | 29,551,951.5 ns | 100,437.78 ns | 44,595.00 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 8192 | PipeOrgan          |    106,634.3 ns |     895.95 ns |    468.60 ns | 0.004 |    0.00 |    2 |         - |          NA |
| TreapSort              | 8192 | PipeOrgan          |    412,788.2 ns |   7,147.58 ns |  3,738.32 ns | 0.014 |    0.00 |    4 |         - |          NA |
| DotnetSort             | 8192 | PipeOrgan          |    278,388.9 ns |   5,056.42 ns |  2,644.61 ns | 0.009 |    0.00 |    3 |         - |          NA |
|      |                    |                 |               |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **8192** | **ManyDuplicates**     |    **522,646.5 ns** |   **2,530.40 ns** |  **1,323.45 ns** |  **0.19** |    **0.00** |    **2** |         **-** |          **NA** |
| BTreeSort              | 8192 | ManyDuplicates     |    579,092.3 ns |   1,523.17 ns |    796.65 ns |  0.21 |    0.00 |    2 |         - |          NA |
| BPlusTreeSort          | 8192 | ManyDuplicates     |    560,745.3 ns |   1,750.11 ns |    915.34 ns |  0.20 |    0.00 |    2 |         - |          NA |
| CartesianTreeSort      | 8192 | ManyDuplicates     |    796,382.0 ns |   5,505.34 ns |  2,444.41 ns |  0.28 |    0.00 |    3 |         - |          NA |
| BinaryTreeSort         | 8192 | ManyDuplicates     |  2,824,319.0 ns |   2,493.66 ns |  1,107.20 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 8192 | ManyDuplicates     |  1,014,336.9 ns |   2,570.10 ns |  1,344.21 ns |  0.36 |    0.00 |    4 |         - |          NA |
| TreapSort              | 8192 | ManyDuplicates     |    705,885.2 ns |   1,732.69 ns |    906.23 ns |  0.25 |    0.00 |    3 |         - |          NA |
| DotnetSort             | 8192 | ManyDuplicates     |    190,348.9 ns |     628.29 ns |    278.96 ns |  0.07 |    0.00 |    1 |         - |          NA |

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
