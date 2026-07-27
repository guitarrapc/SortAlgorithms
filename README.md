# SortAlgorithms

This repository shows implementation for the Major Sort Algorithm.
Aim not to use LINQ or similar ease to use, but memory unefficient technique.

## Table of Contents

- [Benchmark](#benchmark)
  - [AdaptiveBenchmark](#adaptivebenchmark)
  - [AdaptiveSlowBenchmark](#adaptiveslowbenchmark)
  - [DistributionBenchmark](#distributionbenchmark)
  - [ExchangeBenchmark](#exchangebenchmark)
  - [HeapBenchmark](#heapbenchmark)
  - [InsertionBenchmark](#insertionbenchmark)
  - [IntKeyBenchmark](#intkeybenchmark)
  - [MergeBenchmark](#mergebenchmark)
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
  - [Adaptive](#adaptive)
  - [Distribution](#distribution)
  - [Network](#network)
  - [Tree](#tree)
  - [Joke](#joke)

## Benchmark

<!-- BENCHMARK_START -->
<details>
<summary>Benchmark results (2026-07-27 16:04 UTC)</summary>

Workflow run: https://github.com/guitarrapc/SortAlgorithms/actions/runs/30279363441

### AdaptiveBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method        | Size | Pattern            | Mean           | Error        | StdDev       | Median         | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------- |----- |------------------- |---------------:|-------------:|-------------:|---------------:|------:|--------:|-----:|----------:|------------:|
| **DropMergeSort** | **256**  | **Random**             |     **5,276.9 ns** |    **766.12 ns** |    **340.16 ns** |     **5,346.9 ns** |  **1.00** |    **0.09** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | Random             |     5,566.9 ns |    887.21 ns |    393.93 ns |     5,677.9 ns |  1.06 |    0.10 |    1 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **DropMergeSort** | **256**  | **SingleElementMoved** |       **886.1 ns** |    **112.93 ns** |     **40.27 ns** |       **899.5 ns** |  **1.00** |    **0.06** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | SingleElementMoved |     7,353.2 ns |    289.19 ns |    151.25 ns |     7,287.3 ns |  8.31 |    0.39 |    2 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **DropMergeSort** | **256**  | **Sorted**             |       **806.0 ns** |     **26.84 ns** |     **14.04 ns** |       **804.1 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | Sorted             |     7,601.9 ns |    125.74 ns |     55.83 ns |     7,595.0 ns |  9.43 |    0.17 |    2 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **DropMergeSort** | **256**  | **Reversed**           |     **7,843.2 ns** |    **272.78 ns** |    **142.67 ns** |     **7,849.9 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| PatienceSort  | 256  | Reversed           |     1,856.2 ns |    362.35 ns |    189.51 ns |     1,852.2 ns |  0.24 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **DropMergeSort** | **256**  | **PipeOrgan**          |     **7,544.8 ns** |    **424.74 ns** |    **222.15 ns** |     **7,406.6 ns** |  **1.00** |    **0.04** |    **2** |         **-** |          **NA** |
| PatienceSort  | 256  | PipeOrgan          |     5,452.5 ns |    527.73 ns |    276.01 ns |     5,425.4 ns |  0.72 |    0.04 |    1 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **DropMergeSort** | **1024** | **Random**             |    **24,768.7 ns** |    **471.36 ns** |    **209.29 ns** |    **24,678.4 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | Random             |    22,434.8 ns |    702.30 ns |    367.32 ns |    22,270.7 ns |  0.91 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **DropMergeSort** | **1024** | **SingleElementMoved** |     **3,976.5 ns** |  **2,846.83 ns** |  **1,488.95 ns** |     **4,948.3 ns** |  **1.18** |    **0.68** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | SingleElementMoved |    39,333.6 ns |  1,339.95 ns |    477.84 ns |    39,236.0 ns | 11.63 |    5.02 |    2 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **DropMergeSort** | **1024** | **Sorted**             |     **2,095.5 ns** |    **404.98 ns** |    **211.81 ns** |     **1,954.3 ns** |  **1.01** |    **0.13** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | Sorted             |    42,378.9 ns |  6,116.68 ns |  2,715.84 ns |    40,781.8 ns | 20.39 |    2.17 |    2 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **DropMergeSort** | **1024** | **Reversed**           |    **52,809.1 ns** |    **155.69 ns** |     **81.43 ns** |    **52,807.2 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PatienceSort  | 1024 | Reversed           |     6,447.1 ns |    142.36 ns |     74.45 ns |     6,434.8 ns |  0.12 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **DropMergeSort** | **1024** | **PipeOrgan**          |    **40,015.8 ns** |  **1,101.09 ns** |    **575.89 ns** |    **39,831.5 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| PatienceSort  | 1024 | PipeOrgan          |    26,659.7 ns |  1,339.79 ns |    700.73 ns |    26,807.7 ns |  0.67 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **DropMergeSort** | **8192** | **Random**             |   **537,525.0 ns** |    **932.97 ns** |    **414.24 ns** |   **537,525.0 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | Random             |   718,362.7 ns |  2,151.30 ns |    955.19 ns |   718,090.5 ns |  1.34 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **DropMergeSort** | **8192** | **SingleElementMoved** |    **17,230.2 ns** |    **192.37 ns** |     **68.60 ns** |    **17,237.1 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | SingleElementMoved |   741,699.0 ns | 17,073.97 ns |  8,930.02 ns |   744,969.4 ns | 43.05 |    0.51 |    2 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **DropMergeSort** | **8192** | **Sorted**             |    **15,955.5 ns** |    **543.39 ns** |    **241.27 ns** |    **15,943.5 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | Sorted             |   742,306.0 ns | 12,629.96 ns |  6,605.71 ns |   746,121.3 ns | 46.53 |    0.77 |    2 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **DropMergeSort** | **8192** | **Reversed**           | **1,122,498.5 ns** |  **2,586.12 ns** |  **1,352.59 ns** | **1,122,810.8 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PatienceSort  | 8192 | Reversed           |    46,153.8 ns |  1,089.70 ns |    569.93 ns |    46,181.9 ns |  0.04 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **DropMergeSort** | **8192** | **PipeOrgan**          |   **520,285.7 ns** | **32,462.50 ns** | **16,978.52 ns** |   **509,798.5 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | PipeOrgan          |   574,102.3 ns |  1,860.96 ns |    663.64 ns |   574,169.0 ns |  1.10 |    0.03 |    1 |         - |          NA |

### AdaptiveSlowBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method     | Size | Pattern            | Mean         | Error       | StdDev    | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------- |----- |------------------- |-------------:|------------:|----------:|------:|--------:|-----:|----------:|------------:|
| **StrandSort** | **256**  | **Random**             |   **6,906.6 ns** |   **814.36 ns** | **425.93 ns** |  **1.00** |    **0.08** |    **1** |         **-** |          **NA** |
|      |                    |              |             |           |       |         |      |           |             |
| **StrandSort** | **256**  | **SingleElementMoved** |     **818.9 ns** |    **13.38 ns** |   **7.00 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |             |           |       |         |      |           |             |
| **StrandSort** | **256**  | **Sorted**             |     **543.8 ns** |     **7.46 ns** |   **3.31 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |             |           |       |         |      |           |             |
| **StrandSort** | **256**  | **Reversed**           |  **52,505.3 ns** |   **527.25 ns** | **234.10 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |             |           |       |         |      |           |             |
| **StrandSort** | **256**  | **PipeOrgan**          |  **27,533.4 ns** |   **215.73 ns** | **112.83 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |             |           |       |         |      |           |             |
| **StrandSort** | **1024** | **Random**             |  **55,961.0 ns** |   **156.55 ns** |  **55.83 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |           |       |         |      |           |             |
| **StrandSort** | **1024** | **SingleElementMoved** |   **2,624.3 ns** |    **21.03 ns** |   **9.34 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |           |       |         |      |           |             |
| **StrandSort** | **1024** | **Sorted**             |   **1,582.9 ns** |     **5.31 ns** |   **2.78 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |           |       |         |      |           |             |
| **StrandSort** | **1024** | **Reversed**           | **771,498.5 ns** | **1,376.91 ns** | **611.36 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |           |       |         |      |           |             |
| **StrandSort** | **1024** | **PipeOrgan**          | **395,334.6 ns** | **1,520.13 ns** | **795.06 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |

### DistributionBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method              | Size | Pattern            | Mean           | Error         | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------------- |----- |------------------- |---------------:|--------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **CountingSort**        | **256**  | **Random**             |     **1,905.1 ns** |     **166.61 ns** |     **87.14 ns** |  **1.85** |    **0.08** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | Random             |     1,030.4 ns |       5.79 ns |      3.03 ns |  1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 256  | Random             |     1,521.2 ns |       9.38 ns |      4.16 ns |  1.48 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | Random             |       994.5 ns |      15.76 ns |      5.62 ns |  0.97 |    0.01 |    1 |         - |          NA |
| BucketSort          | 256  | Random             |     8,422.6 ns |     569.74 ns |    252.97 ns |  8.17 |    0.23 |    6 |         - |          NA |
| BucketSortInteger   | 256  | Random             |     2,908.6 ns |      80.70 ns |     28.78 ns |  2.82 |    0.03 |    3 |         - |          NA |
| FlashSort           | 256  | Random             |     4,323.7 ns |      30.14 ns |     10.75 ns |  4.20 |    0.02 |    4 |         - |          NA |
| RadixLSD4Sort       | 256  | Random             |     5,788.6 ns |     223.60 ns |    116.95 ns |  5.62 |    0.11 |    5 |         - |          NA |
| RadixLSD256Sort     | 256  | Random             |     2,424.5 ns |     255.64 ns |    113.51 ns |  2.35 |    0.10 |    3 |         - |          NA |
| RadixLSD10Sort      | 256  | Random             |     4,152.8 ns |     299.64 ns |    133.04 ns |  4.03 |    0.12 |    4 |         - |          NA |
| RadixMSD4Sort       | 256  | Random             |     9,433.3 ns |     315.55 ns |    165.04 ns |  9.15 |    0.15 |    6 |         - |          NA |
| RadixMSD10Sort      | 256  | Random             |    13,770.2 ns |     438.36 ns |    229.27 ns | 13.36 |    0.21 |    7 |         - |          NA |
| AmericanFlagSort    | 256  | Random             |     3,982.7 ns |      20.42 ns |      9.07 ns |  3.87 |    0.01 |    4 |         - |          NA |
| SpreadSort          | 256  | Random             |     1,708.1 ns |      31.36 ns |     13.92 ns |  1.66 |    0.01 |    2 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **CountingSort**        | **256**  | **SingleElementMoved** |     **1,597.6 ns** |      **18.65 ns** |      **6.65 ns** |  **1.66** |    **0.01** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | SingleElementMoved |       963.4 ns |       3.08 ns |      1.61 ns |  1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 256  | SingleElementMoved |     1,498.0 ns |      11.24 ns |      4.99 ns |  1.55 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | SingleElementMoved |       976.8 ns |       7.60 ns |      3.38 ns |  1.01 |    0.00 |    1 |         - |          NA |
| BucketSort          | 256  | SingleElementMoved |     3,508.0 ns |     387.13 ns |    171.89 ns |  3.64 |    0.17 |    4 |         - |          NA |
| BucketSortInteger   | 256  | SingleElementMoved |     2,104.9 ns |     421.88 ns |    220.65 ns |  2.18 |    0.22 |    3 |         - |          NA |
| FlashSort           | 256  | SingleElementMoved |     5,084.8 ns |     359.17 ns |    187.85 ns |  5.28 |    0.18 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | SingleElementMoved |     5,684.5 ns |      44.37 ns |     15.82 ns |  5.90 |    0.02 |    5 |         - |          NA |
| RadixLSD256Sort     | 256  | SingleElementMoved |     1,975.0 ns |      45.60 ns |     23.85 ns |  2.05 |    0.02 |    3 |         - |          NA |
| RadixLSD10Sort      | 256  | SingleElementMoved |     4,008.8 ns |     308.60 ns |    137.02 ns |  4.16 |    0.13 |    4 |         - |          NA |
| RadixMSD4Sort       | 256  | SingleElementMoved |     8,383.9 ns |      40.51 ns |     14.45 ns |  8.70 |    0.02 |    6 |         - |          NA |
| RadixMSD10Sort      | 256  | SingleElementMoved |    13,224.3 ns |     285.98 ns |    149.57 ns | 13.73 |    0.15 |    7 |         - |          NA |
| AmericanFlagSort    | 256  | SingleElementMoved |     3,054.7 ns |      50.05 ns |     17.85 ns |  3.17 |    0.02 |    4 |         - |          NA |
| SpreadSort          | 256  | SingleElementMoved |     1,110.3 ns |      18.93 ns |      8.41 ns |  1.15 |    0.01 |    1 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **CountingSort**        | **256**  | **Sorted**             |     **1,543.6 ns** |      **13.00 ns** |      **4.64 ns** |  **1.69** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | Sorted             |       912.2 ns |       3.68 ns |      1.63 ns |  1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 256  | Sorted             |     1,488.4 ns |       8.71 ns |      3.87 ns |  1.63 |    0.00 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | Sorted             |       958.6 ns |       5.27 ns |      2.34 ns |  1.05 |    0.00 |    2 |         - |          NA |
| BucketSort          | 256  | Sorted             |     3,136.0 ns |      18.17 ns |      6.48 ns |  3.44 |    0.01 |    5 |         - |          NA |
| BucketSortInteger   | 256  | Sorted             |     1,878.6 ns |       8.12 ns |      4.25 ns |  2.06 |    0.01 |    4 |         - |          NA |
| FlashSort           | 256  | Sorted             |     4,959.2 ns |     322.38 ns |    168.61 ns |  5.44 |    0.17 |    7 |         - |          NA |
| RadixLSD4Sort       | 256  | Sorted             |     6,071.2 ns |      75.41 ns |     39.44 ns |  6.66 |    0.04 |    8 |         - |          NA |
| RadixLSD256Sort     | 256  | Sorted             |     1,911.2 ns |      58.93 ns |     21.02 ns |  2.10 |    0.02 |    4 |         - |          NA |
| RadixLSD10Sort      | 256  | Sorted             |     3,888.1 ns |      30.83 ns |     13.69 ns |  4.26 |    0.02 |    6 |         - |          NA |
| RadixMSD4Sort       | 256  | Sorted             |     8,721.9 ns |     512.02 ns |    227.34 ns |  9.56 |    0.23 |    9 |         - |          NA |
| RadixMSD10Sort      | 256  | Sorted             |    13,114.0 ns |     352.56 ns |    184.40 ns | 14.38 |    0.19 |   10 |         - |          NA |
| AmericanFlagSort    | 256  | Sorted             |     2,914.8 ns |      46.75 ns |     16.67 ns |  3.20 |    0.02 |    5 |         - |          NA |
| SpreadSort          | 256  | Sorted             |       378.7 ns |       4.30 ns |      1.53 ns |  0.42 |    0.00 |    1 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **CountingSort**        | **256**  | **Reversed**           |     **1,562.2 ns** |      **12.05 ns** |      **5.35 ns** |  **1.69** |    **0.01** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | Reversed           |       925.4 ns |       9.36 ns |      4.16 ns |  1.00 |    0.01 |    1 |         - |          NA |
| PigeonSort          | 256  | Reversed           |     1,418.8 ns |      17.52 ns |      6.25 ns |  1.53 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | Reversed           |       897.7 ns |       9.07 ns |      4.03 ns |  0.97 |    0.01 |    1 |         - |          NA |
| BucketSort          | 256  | Reversed           |    12,069.3 ns |   1,964.12 ns |  1,027.27 ns | 13.04 |    1.05 |    7 |         - |          NA |
| BucketSortInteger   | 256  | Reversed           |     3,073.3 ns |      21.07 ns |      9.36 ns |  3.32 |    0.02 |    4 |         - |          NA |
| FlashSort           | 256  | Reversed           |     4,397.9 ns |     403.64 ns |    211.11 ns |  4.75 |    0.22 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | Reversed           |     5,693.7 ns |      24.11 ns |      8.60 ns |  6.15 |    0.03 |    5 |         - |          NA |
| RadixLSD256Sort     | 256  | Reversed           |     1,937.0 ns |      72.86 ns |     25.98 ns |  2.09 |    0.03 |    3 |         - |          NA |
| RadixLSD10Sort      | 256  | Reversed           |     4,138.4 ns |     341.60 ns |    178.66 ns |  4.47 |    0.18 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | Reversed           |     9,671.7 ns |     923.42 ns |    410.00 ns | 10.45 |    0.42 |    6 |         - |          NA |
| RadixMSD10Sort      | 256  | Reversed           |    13,846.2 ns |     275.16 ns |    143.91 ns | 14.96 |    0.16 |    7 |         - |          NA |
| AmericanFlagSort    | 256  | Reversed           |     4,985.2 ns |     267.68 ns |    140.00 ns |  5.39 |    0.14 |    5 |         - |          NA |
| SpreadSort          | 256  | Reversed           |       876.6 ns |     647.68 ns |    338.75 ns |  0.95 |    0.35 |    1 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **CountingSort**        | **256**  | **PipeOrgan**          |     **1,541.9 ns** |     **119.84 ns** |     **53.21 ns** |  **1.79** |    **0.06** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | PipeOrgan          |       862.7 ns |       5.93 ns |      3.10 ns |  1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 256  | PipeOrgan          |     1,427.7 ns |       5.24 ns |      2.33 ns |  1.65 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | PipeOrgan          |       939.3 ns |      26.43 ns |     11.73 ns |  1.09 |    0.01 |    1 |         - |          NA |
| BucketSort          | 256  | PipeOrgan          |     7,070.6 ns |     502.78 ns |    262.96 ns |  8.20 |    0.29 |    6 |         - |          NA |
| BucketSortInteger   | 256  | PipeOrgan          |     2,517.9 ns |      18.24 ns |      6.50 ns |  2.92 |    0.01 |    4 |         - |          NA |
| FlashSort           | 256  | PipeOrgan          |     4,630.9 ns |     299.98 ns |    156.89 ns |  5.37 |    0.17 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | PipeOrgan          |     5,836.2 ns |      12.77 ns |      5.67 ns |  6.77 |    0.02 |    6 |         - |          NA |
| RadixLSD256Sort     | 256  | PipeOrgan          |     2,097.1 ns |     158.11 ns |     70.20 ns |  2.43 |    0.08 |    3 |         - |          NA |
| RadixLSD10Sort      | 256  | PipeOrgan          |     3,982.3 ns |     288.58 ns |    150.93 ns |  4.62 |    0.17 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | PipeOrgan          |     9,594.1 ns |     394.57 ns |    206.37 ns | 11.12 |    0.23 |    7 |         - |          NA |
| RadixMSD10Sort      | 256  | PipeOrgan          |    13,836.0 ns |     154.39 ns |     80.75 ns | 16.04 |    0.10 |    8 |         - |          NA |
| AmericanFlagSort    | 256  | PipeOrgan          |     4,493.3 ns |     435.50 ns |    227.77 ns |  5.21 |    0.25 |    5 |         - |          NA |
| SpreadSort          | 256  | PipeOrgan          |     1,702.6 ns |      96.97 ns |     50.71 ns |  1.97 |    0.06 |    2 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **Random**             |     **6,513.1 ns** |     **410.60 ns** |    **214.75 ns** |  **1.64** |    **0.11** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Random             |     3,987.3 ns |     516.70 ns |    270.24 ns |  1.00 |    0.09 |    1 |         - |          NA |
| PigeonSort          | 1024 | Random             |     5,599.6 ns |     434.58 ns |    227.29 ns |  1.41 |    0.10 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | Random             |     3,442.8 ns |      13.62 ns |      4.86 ns |  0.87 |    0.05 |    1 |         - |          NA |
| BucketSort          | 1024 | Random             |    51,935.9 ns |   6,199.37 ns |  3,242.39 ns | 13.08 |    1.12 |    7 |         - |          NA |
| BucketSortInteger   | 1024 | Random             |    15,047.0 ns |     143.91 ns |     75.27 ns |  3.79 |    0.24 |    4 |         - |          NA |
| FlashSort           | 1024 | Random             |    17,688.3 ns |     284.63 ns |    126.38 ns |  4.45 |    0.28 |    4 |         - |          NA |
| RadixLSD4Sort       | 1024 | Random             |    24,753.1 ns |     400.58 ns |    209.51 ns |  6.23 |    0.39 |    5 |         - |          NA |
| RadixLSD256Sort     | 1024 | Random             |     9,650.3 ns |     312.79 ns |    163.60 ns |  2.43 |    0.16 |    3 |         - |          NA |
| RadixLSD10Sort      | 1024 | Random             |    20,577.5 ns |     173.74 ns |     90.87 ns |  5.18 |    0.32 |    4 |         - |          NA |
| RadixMSD4Sort       | 1024 | Random             |    38,421.0 ns |     340.59 ns |    151.22 ns |  9.67 |    0.60 |    6 |         - |          NA |
| RadixMSD10Sort      | 1024 | Random             |    51,107.8 ns |     299.51 ns |    106.81 ns | 12.87 |    0.80 |    7 |         - |          NA |
| AmericanFlagSort    | 1024 | Random             |    19,066.2 ns |     467.47 ns |    244.49 ns |  4.80 |    0.30 |    4 |         - |          NA |
| SpreadSort          | 1024 | Random             |     8,586.2 ns |     370.66 ns |    193.86 ns |  2.16 |    0.14 |    3 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **SingleElementMoved** |     **6,419.6 ns** |     **457.46 ns** |    **203.11 ns** |  **1.69** |    **0.10** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | SingleElementMoved |     3,806.7 ns |     410.11 ns |    214.50 ns |  1.00 |    0.07 |    1 |         - |          NA |
| PigeonSort          | 1024 | SingleElementMoved |     5,484.0 ns |     293.64 ns |    153.58 ns |  1.44 |    0.08 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | SingleElementMoved |     3,581.0 ns |     420.25 ns |    219.80 ns |  0.94 |    0.07 |    1 |         - |          NA |
| BucketSort          | 1024 | SingleElementMoved |    12,840.2 ns |     351.00 ns |    155.85 ns |  3.38 |    0.18 |    3 |         - |          NA |
| BucketSortInteger   | 1024 | SingleElementMoved |     7,822.3 ns |     418.92 ns |    219.10 ns |  2.06 |    0.12 |    2 |         - |          NA |
| FlashSort           | 1024 | SingleElementMoved |    19,585.8 ns |     607.53 ns |    317.75 ns |  5.16 |    0.28 |    4 |         - |          NA |
| RadixLSD4Sort       | 1024 | SingleElementMoved |    24,207.8 ns |     191.75 ns |    100.29 ns |  6.38 |    0.33 |    4 |         - |          NA |
| RadixLSD256Sort     | 1024 | SingleElementMoved |     9,368.1 ns |     296.29 ns |    154.97 ns |  2.47 |    0.13 |    2 |         - |          NA |
| RadixLSD10Sort      | 1024 | SingleElementMoved |    20,403.8 ns |     410.33 ns |    214.61 ns |  5.37 |    0.28 |    4 |         - |          NA |
| RadixMSD4Sort       | 1024 | SingleElementMoved |    31,884.9 ns |      67.53 ns |     35.32 ns |  8.40 |    0.43 |    5 |         - |          NA |
| RadixMSD10Sort      | 1024 | SingleElementMoved |    47,855.2 ns |     310.09 ns |    162.18 ns | 12.61 |    0.65 |    6 |         - |          NA |
| AmericanFlagSort    | 1024 | SingleElementMoved |    11,645.8 ns |     250.13 ns |    130.82 ns |  3.07 |    0.16 |    3 |         - |          NA |
| SpreadSort          | 1024 | SingleElementMoved |     7,373.8 ns |     191.02 ns |     99.91 ns |  1.94 |    0.10 |    2 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **Sorted**             |    **10,736.6 ns** |   **6,643.58 ns** |  **3,474.72 ns** |  **3.11** |    **0.95** |    **5** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Sorted             |     3,449.4 ns |      86.23 ns |     30.75 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PigeonSort          | 1024 | Sorted             |     5,386.7 ns |     503.18 ns |    263.18 ns |  1.56 |    0.07 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | Sorted             |     3,412.6 ns |       6.76 ns |      3.00 ns |  0.99 |    0.01 |    2 |         - |          NA |
| BucketSort          | 1024 | Sorted             |    12,559.4 ns |     331.83 ns |    173.55 ns |  3.64 |    0.06 |    5 |         - |          NA |
| BucketSortInteger   | 1024 | Sorted             |     7,542.6 ns |     248.87 ns |    130.16 ns |  2.19 |    0.04 |    4 |         - |          NA |
| FlashSort           | 1024 | Sorted             |    18,801.3 ns |     190.62 ns |     84.64 ns |  5.45 |    0.05 |    6 |         - |          NA |
| RadixLSD4Sort       | 1024 | Sorted             |    25,315.0 ns |     377.54 ns |    197.46 ns |  7.34 |    0.08 |    7 |         - |          NA |
| RadixLSD256Sort     | 1024 | Sorted             |     9,223.2 ns |     363.99 ns |    190.37 ns |  2.67 |    0.06 |    5 |         - |          NA |
| RadixLSD10Sort      | 1024 | Sorted             |    20,228.7 ns |     416.99 ns |    218.09 ns |  5.86 |    0.08 |    6 |         - |          NA |
| RadixMSD4Sort       | 1024 | Sorted             |    31,723.3 ns |      87.07 ns |     45.54 ns |  9.20 |    0.08 |    8 |         - |          NA |
| RadixMSD10Sort      | 1024 | Sorted             |    47,564.1 ns |      68.91 ns |     30.60 ns | 13.79 |    0.11 |    9 |         - |          NA |
| AmericanFlagSort    | 1024 | Sorted             |    11,220.5 ns |     842.14 ns |    440.45 ns |  3.25 |    0.12 |    5 |         - |          NA |
| SpreadSort          | 1024 | Sorted             |       693.3 ns |       8.58 ns |      3.81 ns |  0.20 |    0.00 |    1 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **Reversed**           |     **5,779.0 ns** |     **270.08 ns** |    **141.26 ns** |  **1.68** |    **0.04** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Reversed           |     3,442.3 ns |      78.05 ns |     27.83 ns |  1.00 |    0.01 |    1 |         - |          NA |
| PigeonSort          | 1024 | Reversed           |     5,052.4 ns |      21.33 ns |      7.61 ns |  1.47 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | Reversed           |     3,128.3 ns |      11.76 ns |      4.19 ns |  0.91 |    0.01 |    1 |         - |          NA |
| BucketSort          | 1024 | Reversed           |    80,517.1 ns |     319.78 ns |    141.98 ns | 23.39 |    0.18 |    7 |         - |          NA |
| BucketSortInteger   | 1024 | Reversed           |    17,166.6 ns |     649.87 ns |    288.55 ns |  4.99 |    0.09 |    4 |         - |          NA |
| FlashSort           | 1024 | Reversed           |    16,641.2 ns |     354.31 ns |    185.31 ns |  4.83 |    0.06 |    4 |         - |          NA |
| RadixLSD4Sort       | 1024 | Reversed           |    25,191.8 ns |     484.79 ns |    253.55 ns |  7.32 |    0.09 |    4 |         - |          NA |
| RadixLSD256Sort     | 1024 | Reversed           |     9,240.6 ns |     324.07 ns |    169.49 ns |  2.68 |    0.05 |    3 |         - |          NA |
| RadixLSD10Sort      | 1024 | Reversed           |    20,302.8 ns |     455.85 ns |    238.42 ns |  5.90 |    0.08 |    4 |         - |          NA |
| RadixMSD4Sort       | 1024 | Reversed           |    36,485.6 ns |     193.39 ns |    101.15 ns | 10.60 |    0.08 |    5 |         - |          NA |
| RadixMSD10Sort      | 1024 | Reversed           |    49,150.7 ns |     260.03 ns |    115.45 ns | 14.28 |    0.11 |    6 |         - |          NA |
| AmericanFlagSort    | 1024 | Reversed           |    21,825.5 ns |     667.51 ns |    349.12 ns |  6.34 |    0.11 |    4 |         - |          NA |
| SpreadSort          | 1024 | Reversed           |     5,592.8 ns |     346.55 ns |    181.25 ns |  1.62 |    0.05 |    2 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **PipeOrgan**          |     **5,635.1 ns** |     **315.79 ns** |    **165.16 ns** |  **1.76** |    **0.05** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | PipeOrgan          |     3,210.3 ns |       6.62 ns |      2.36 ns |  1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 1024 | PipeOrgan          |     5,195.7 ns |     407.86 ns |    213.32 ns |  1.62 |    0.06 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | PipeOrgan          |     3,452.7 ns |     467.10 ns |    244.30 ns |  1.08 |    0.07 |    1 |         - |          NA |
| BucketSort          | 1024 | PipeOrgan          |    50,297.4 ns |     663.68 ns |    294.68 ns | 15.67 |    0.09 |    9 |         - |          NA |
| BucketSortInteger   | 1024 | PipeOrgan          |    12,105.9 ns |     274.16 ns |    143.39 ns |  3.77 |    0.04 |    5 |         - |          NA |
| FlashSort           | 1024 | PipeOrgan          |    17,774.9 ns |     264.05 ns |     94.16 ns |  5.54 |    0.03 |    6 |         - |          NA |
| RadixLSD4Sort       | 1024 | PipeOrgan          |    25,883.7 ns |     306.43 ns |    160.27 ns |  8.06 |    0.05 |    7 |         - |          NA |
| RadixLSD256Sort     | 1024 | PipeOrgan          |     9,570.3 ns |     384.75 ns |    201.23 ns |  2.98 |    0.06 |    4 |         - |          NA |
| RadixLSD10Sort      | 1024 | PipeOrgan          |    20,420.5 ns |     310.28 ns |    162.28 ns |  6.36 |    0.05 |    6 |         - |          NA |
| RadixMSD4Sort       | 1024 | PipeOrgan          |    36,704.2 ns |     673.93 ns |    352.48 ns | 11.43 |    0.10 |    8 |         - |          NA |
| RadixMSD10Sort      | 1024 | PipeOrgan          |    49,841.0 ns |     221.74 ns |     98.45 ns | 15.53 |    0.03 |    9 |         - |          NA |
| AmericanFlagSort    | 1024 | PipeOrgan          |    19,076.0 ns |     512.75 ns |    227.67 ns |  5.94 |    0.07 |    6 |         - |          NA |
| SpreadSort          | 1024 | PipeOrgan          |     7,211.4 ns |      50.16 ns |     22.27 ns |  2.25 |    0.01 |    3 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **Random**             |    **53,566.8 ns** |     **997.28 ns** |    **521.59 ns** |  **1.54** |    **0.02** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Random             |    34,691.8 ns |     648.12 ns |    338.98 ns |  1.00 |    0.01 |    1 |         - |          NA |
| PigeonSort          | 8192 | Random             |    47,423.7 ns |     675.97 ns |    300.14 ns |  1.37 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | Random             |    29,883.9 ns |     583.03 ns |    207.92 ns |  0.86 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | Random             |   987,715.6 ns | 123,034.32 ns | 64,349.33 ns | 28.47 |    1.77 |    7 |         - |          NA |
| BucketSortInteger   | 8192 | Random             |   246,863.1 ns |   3,291.02 ns |  1,173.61 ns |  7.12 |    0.07 |    5 |         - |          NA |
| FlashSort           | 8192 | Random             |   154,553.5 ns |   1,067.58 ns |    558.37 ns |  4.46 |    0.04 |    4 |         - |          NA |
| RadixLSD4Sort       | 8192 | Random             |   232,675.0 ns |   1,382.41 ns |    613.80 ns |  6.71 |    0.06 |    5 |         - |          NA |
| RadixLSD256Sort     | 8192 | Random             |    69,796.2 ns |   1,035.36 ns |    541.51 ns |  2.01 |    0.02 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | Random             |   172,014.5 ns |     512.03 ns |    267.80 ns |  4.96 |    0.05 |    4 |         - |          NA |
| RadixMSD4Sort       | 8192 | Random             |   380,600.5 ns |   1,181.43 ns |    617.91 ns | 10.97 |    0.10 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | Random             |   421,285.2 ns |  10,935.81 ns |  4,855.57 ns | 12.14 |    0.17 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | Random             |   236,150.5 ns |   1,819.44 ns |    951.60 ns |  6.81 |    0.07 |    5 |         - |          NA |
| SpreadSort          | 8192 | Random             |    83,060.4 ns |     892.25 ns |    466.66 ns |  2.39 |    0.03 |    3 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **SingleElementMoved** |    **47,139.9 ns** |     **971.55 ns** |    **431.38 ns** |  **1.60** |    **0.03** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | SingleElementMoved |    29,452.5 ns |   1,256.56 ns |    557.92 ns |  1.00 |    0.02 |    1 |         - |          NA |
| PigeonSort          | 8192 | SingleElementMoved |    43,348.4 ns |     325.55 ns |    170.27 ns |  1.47 |    0.03 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | SingleElementMoved |    27,925.2 ns |     528.48 ns |    188.46 ns |  0.95 |    0.02 |    1 |         - |          NA |
| BucketSort          | 8192 | SingleElementMoved |    91,505.3 ns |     222.48 ns |     98.78 ns |  3.11 |    0.05 |    3 |         - |          NA |
| BucketSortInteger   | 8192 | SingleElementMoved |    48,951.6 ns |     454.25 ns |    237.58 ns |  1.66 |    0.03 |    2 |         - |          NA |
| FlashSort           | 8192 | SingleElementMoved |   158,205.4 ns |   1,264.11 ns |    561.27 ns |  5.37 |    0.10 |    4 |         - |          NA |
| RadixLSD4Sort       | 8192 | SingleElementMoved |   233,668.7 ns |     497.42 ns |    220.86 ns |  7.94 |    0.14 |    5 |         - |          NA |
| RadixLSD256Sort     | 8192 | SingleElementMoved |    40,732.6 ns |     576.43 ns |    255.94 ns |  1.38 |    0.03 |    2 |         - |          NA |
| RadixLSD10Sort      | 8192 | SingleElementMoved |   165,941.9 ns |   7,070.65 ns |  3,698.09 ns |  5.64 |    0.15 |    4 |         - |          NA |
| RadixMSD4Sort       | 8192 | SingleElementMoved |   255,081.5 ns |   1,211.17 ns |    537.77 ns |  8.66 |    0.15 |    5 |         - |          NA |
| RadixMSD10Sort      | 8192 | SingleElementMoved |   377,015.0 ns |     671.69 ns |    298.23 ns | 12.80 |    0.23 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | SingleElementMoved |    97,170.9 ns |     821.37 ns |    429.59 ns |  3.30 |    0.06 |    3 |         - |          NA |
| SpreadSort          | 8192 | SingleElementMoved |    49,929.8 ns |   1,416.43 ns |    628.90 ns |  1.70 |    0.04 |    2 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **Sorted**             |    **46,731.8 ns** |   **1,899.67 ns** |    **843.46 ns** |  **1.66** |    **0.04** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Sorted             |    28,215.7 ns |   1,032.89 ns |    540.22 ns |  1.00 |    0.03 |    2 |         - |          NA |
| PigeonSort          | 8192 | Sorted             |    41,449.0 ns |     831.23 ns |    434.75 ns |  1.47 |    0.03 |    3 |         - |          NA |
| PigeonSortInteger   | 8192 | Sorted             |    27,676.1 ns |   1,113.23 ns |    494.28 ns |  0.98 |    0.02 |    2 |         - |          NA |
| BucketSort          | 8192 | Sorted             |    89,743.2 ns |     411.18 ns |    215.05 ns |  3.18 |    0.06 |    4 |         - |          NA |
| BucketSortInteger   | 8192 | Sorted             |    47,603.1 ns |     731.66 ns |    324.86 ns |  1.69 |    0.03 |    3 |         - |          NA |
| FlashSort           | 8192 | Sorted             |   152,823.4 ns |   1,231.42 ns |    644.06 ns |  5.42 |    0.10 |    5 |         - |          NA |
| RadixLSD4Sort       | 8192 | Sorted             |   234,983.7 ns |   1,178.86 ns |    616.57 ns |  8.33 |    0.15 |    6 |         - |          NA |
| RadixLSD256Sort     | 8192 | Sorted             |    39,594.2 ns |   1,542.26 ns |    684.77 ns |  1.40 |    0.03 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | Sorted             |   166,164.2 ns |   6,472.40 ns |  3,385.19 ns |  5.89 |    0.16 |    5 |         - |          NA |
| RadixMSD4Sort       | 8192 | Sorted             |   249,962.2 ns |     886.32 ns |    463.56 ns |  8.86 |    0.16 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | Sorted             |   377,135.9 ns |     945.26 ns |    494.39 ns | 13.37 |    0.24 |    7 |         - |          NA |
| AmericanFlagSort    | 8192 | Sorted             |    93,835.3 ns |     723.53 ns |    378.42 ns |  3.33 |    0.06 |    4 |         - |          NA |
| SpreadSort          | 8192 | Sorted             |     5,308.2 ns |     535.82 ns |    237.91 ns |  0.19 |    0.01 |    1 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **Reversed**           |    **47,015.7 ns** |   **1,374.14 ns** |    **718.70 ns** |  **1.65** |    **0.05** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Reversed           |    28,586.8 ns |   1,411.91 ns |    738.46 ns |  1.00 |    0.03 |    1 |         - |          NA |
| PigeonSort          | 8192 | Reversed           |    41,805.2 ns |     430.09 ns |    190.96 ns |  1.46 |    0.04 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | Reversed           |    26,053.4 ns |   1,346.69 ns |    704.35 ns |  0.91 |    0.03 |    1 |         - |          NA |
| BucketSort          | 8192 | Reversed           | 1,722,379.0 ns |  13,343.70 ns |  4,758.49 ns | 60.29 |    1.47 |    9 |         - |          NA |
| BucketSortInteger   | 8192 | Reversed           |   309,859.0 ns |   3,227.10 ns |  1,687.84 ns | 10.85 |    0.27 |    7 |         - |          NA |
| FlashSort           | 8192 | Reversed           |   132,875.9 ns |   1,080.77 ns |    565.27 ns |  4.65 |    0.11 |    4 |         - |          NA |
| RadixLSD4Sort       | 8192 | Reversed           |   240,391.8 ns |   1,463.03 ns |    765.19 ns |  8.41 |    0.21 |    6 |         - |          NA |
| RadixLSD256Sort     | 8192 | Reversed           |    40,845.3 ns |     525.61 ns |    233.37 ns |  1.43 |    0.04 |    2 |         - |          NA |
| RadixLSD10Sort      | 8192 | Reversed           |   164,177.4 ns |   3,218.72 ns |  1,429.13 ns |  5.75 |    0.15 |    5 |         - |          NA |
| RadixMSD4Sort       | 8192 | Reversed           |   289,391.4 ns |   2,337.69 ns |  1,222.66 ns | 10.13 |    0.25 |    7 |         - |          NA |
| RadixMSD10Sort      | 8192 | Reversed           |   388,846.3 ns |   1,507.51 ns |    788.46 ns | 13.61 |    0.33 |    8 |         - |          NA |
| AmericanFlagSort    | 8192 | Reversed           |   220,809.0 ns |   1,220.08 ns |    541.72 ns |  7.73 |    0.19 |    6 |         - |          NA |
| SpreadSort          | 8192 | Reversed           |    61,254.3 ns |     939.09 ns |    416.96 ns |  2.14 |    0.05 |    3 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **PipeOrgan**          |    **44,738.1 ns** |     **627.42 ns** |    **278.58 ns** |  **1.66** |    **0.02** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | PipeOrgan          |    26,938.6 ns |     743.05 ns |    329.92 ns |  1.00 |    0.02 |    1 |         - |          NA |
| PigeonSort          | 8192 | PipeOrgan          |    41,393.8 ns |   1,061.02 ns |    554.93 ns |  1.54 |    0.03 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | PipeOrgan          |    27,428.6 ns |   1,098.21 ns |    487.61 ns |  1.02 |    0.02 |    1 |         - |          NA |
| BucketSort          | 8192 | PipeOrgan          |   907,791.5 ns |   8,394.06 ns |  4,390.26 ns | 33.70 |    0.42 |    8 |         - |          NA |
| BucketSortInteger   | 8192 | PipeOrgan          |   190,867.2 ns |   1,518.71 ns |    794.32 ns |  7.09 |    0.09 |    5 |         - |          NA |
| FlashSort           | 8192 | PipeOrgan          |   139,670.9 ns |     727.15 ns |    322.86 ns |  5.19 |    0.06 |    4 |         - |          NA |
| RadixLSD4Sort       | 8192 | PipeOrgan          |   240,052.3 ns |   8,631.42 ns |  4,514.40 ns |  8.91 |    0.19 |    6 |         - |          NA |
| RadixLSD256Sort     | 8192 | PipeOrgan          |    72,871.7 ns |   1,126.66 ns |    589.26 ns |  2.71 |    0.04 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | PipeOrgan          |   164,547.8 ns |   9,940.48 ns |  5,199.06 ns |  6.11 |    0.20 |    5 |         - |          NA |
| RadixMSD4Sort       | 8192 | PipeOrgan          |   286,734.9 ns |   7,583.88 ns |  3,367.29 ns | 10.65 |    0.17 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | PipeOrgan          |   394,739.7 ns |   1,659.78 ns |    868.10 ns | 14.66 |    0.17 |    7 |         - |          NA |
| AmericanFlagSort    | 8192 | PipeOrgan          |   171,946.8 ns |   1,509.88 ns |    789.70 ns |  6.38 |    0.08 |    5 |         - |          NA |
| SpreadSort          | 8192 | PipeOrgan          |    81,201.5 ns |     662.49 ns |    346.49 ns |  3.01 |    0.04 |    3 |         - |          NA |

### ExchangeBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method             | Size | Pattern            | Mean         | Error       | StdDev      | Ratio  | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------- |----- |------------------- |-------------:|------------:|------------:|-------:|--------:|-----:|----------:|------------:|
| **BubbleSort**         | **256**  | **Random**             |  **32,774.9 ns** |   **505.41 ns** |   **264.34 ns** |   **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Random             |  16,225.3 ns |   123.78 ns |    54.96 ns |   0.50 |    0.00 |    2 |         - |          NA |
| OddEvenSort        | 256  | Random             |  26,070.7 ns |   368.88 ns |   163.78 ns |   0.80 |    0.01 |    3 |         - |          NA |
| CombSort           | 256  | Random             |   3,745.2 ns |   571.47 ns |   298.89 ns |   0.11 |    0.01 |    1 |         - |          NA |
| CircleSort         | 256  | Random             |  17,893.7 ns |   182.31 ns |    80.94 ns |   0.55 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **SingleElementMoved** |     **404.8 ns** |     **1.23 ns** |     **0.54 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 256  | SingleElementMoved |     310.4 ns |    10.09 ns |     4.48 ns |   0.77 |    0.01 |    1 |         - |          NA |
| OddEvenSort        | 256  | SingleElementMoved |  15,621.5 ns |    71.46 ns |    31.73 ns |  38.59 |    0.09 |    4 |         - |          NA |
| CombSort           | 256  | SingleElementMoved |   2,805.1 ns |     9.27 ns |     4.12 ns |   6.93 |    0.01 |    3 |         - |          NA |
| CircleSort         | 256  | SingleElementMoved |  14,924.7 ns |   320.61 ns |   167.69 ns |  36.87 |    0.39 |    4 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **Sorted**             |     **244.4 ns** |     **0.94 ns** |     **0.42 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Sorted             |     166.8 ns |     0.96 ns |     0.50 ns |   0.68 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 256  | Sorted             |     212.5 ns |     1.01 ns |     0.45 ns |   0.87 |    0.00 |    2 |         - |          NA |
| CombSort           | 256  | Sorted             |   2,479.8 ns |     3.13 ns |     1.12 ns |  10.15 |    0.02 |    3 |         - |          NA |
| CircleSort         | 256  | Sorted             |   2,161.1 ns |   299.89 ns |   133.15 ns |   8.84 |    0.51 |    3 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **Reversed**           |  **27,615.7 ns** |   **261.18 ns** |   **136.60 ns** |   **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Reversed           |  25,223.2 ns |   337.33 ns |   176.43 ns |   0.91 |    0.01 |    3 |         - |          NA |
| OddEvenSort        | 256  | Reversed           |  24,327.4 ns |   210.75 ns |   110.22 ns |   0.88 |    0.01 |    3 |         - |          NA |
| CombSort           | 256  | Reversed           |   3,140.7 ns |    32.10 ns |    11.45 ns |   0.11 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | Reversed           |   4,374.1 ns |   374.17 ns |   195.70 ns |   0.16 |    0.01 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **PipeOrgan**          |  **26,288.2 ns** |   **514.85 ns** |   **228.60 ns** |   **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | PipeOrgan          |  16,966.2 ns |   252.98 ns |   112.32 ns |   0.65 |    0.01 |    2 |         - |          NA |
| OddEvenSort        | 256  | PipeOrgan          |  27,902.7 ns | 2,115.86 ns |   939.45 ns |   1.06 |    0.03 |    3 |         - |          NA |
| CombSort           | 256  | PipeOrgan          |   3,020.5 ns |    19.86 ns |     8.82 ns |   0.11 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | PipeOrgan          |  19,244.5 ns |   191.58 ns |   100.20 ns |   0.73 |    0.01 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Random**             | **603,664.1 ns** | **4,604.54 ns** | **2,408.26 ns** |   **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Random             | 312,310.7 ns | 1,273.51 ns |   666.07 ns |   0.52 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | Random             | 529,932.6 ns | 1,867.17 ns |   829.03 ns |   0.88 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | Random             |  31,851.7 ns |   539.93 ns |   239.73 ns |   0.05 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | Random             |  97,609.6 ns | 1,607.55 ns |   713.76 ns |   0.16 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **SingleElementMoved** |   **1,552.8 ns** |    **85.58 ns** |    **30.52 ns** |   **1.00** |    **0.03** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | SingleElementMoved |   1,157.5 ns |    45.86 ns |    20.36 ns |   0.75 |    0.02 |    1 |         - |          NA |
| OddEvenSort        | 1024 | SingleElementMoved | 223,154.3 ns | 1,504.35 ns |   667.94 ns | 143.76 |    2.61 |    5 |         - |          NA |
| CombSort           | 1024 | SingleElementMoved |  14,900.5 ns |   187.01 ns |    97.81 ns |   9.60 |    0.18 |    3 |         - |          NA |
| CircleSort         | 1024 | SingleElementMoved |  84,030.9 ns |   760.67 ns |   397.84 ns |  54.13 |    1.00 |    4 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Sorted**             |     **957.2 ns** |     **0.74 ns** |     **0.39 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Sorted             |     848.0 ns |   373.01 ns |   195.09 ns |   0.89 |    0.19 |    1 |         - |          NA |
| OddEvenSort        | 1024 | Sorted             |     806.4 ns |     4.58 ns |     2.40 ns |   0.84 |    0.00 |    1 |         - |          NA |
| CombSort           | 1024 | Sorted             |  12,917.8 ns |   221.27 ns |   115.73 ns |  13.50 |    0.11 |    3 |         - |          NA |
| CircleSort         | 1024 | Sorted             |   9,230.3 ns |   359.54 ns |   188.05 ns |   9.64 |    0.19 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Reversed**           | **414,430.7 ns** | **1,517.33 ns** |   **793.59 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Reversed           | 386,136.1 ns |   633.63 ns |   281.34 ns |   0.93 |    0.00 |    2 |         - |          NA |
| OddEvenSort        | 1024 | Reversed           | 368,151.0 ns | 1,286.60 ns |   672.92 ns |   0.89 |    0.00 |    2 |         - |          NA |
| CombSort           | 1024 | Reversed           |  16,695.5 ns |   331.38 ns |   147.14 ns |   0.04 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | Reversed           |  18,561.8 ns |   310.76 ns |   137.98 ns |   0.04 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **PipeOrgan**          | **383,524.4 ns** | **1,038.60 ns** |   **461.15 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | PipeOrgan          | 253,352.1 ns |   602.88 ns |   315.32 ns |   0.66 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | PipeOrgan          | 359,771.0 ns | 1,068.82 ns |   474.56 ns |   0.94 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | PipeOrgan          |  16,864.0 ns |   396.56 ns |   207.41 ns |   0.04 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | PipeOrgan          | 103,235.6 ns |   957.60 ns |   500.84 ns |   0.27 |    0.00 |    2 |         - |          NA |

### HeapBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method           | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **HeapSort**         | **256**  | **Random**             |     **3,967.4 ns** |    **172.35 ns** |     **76.52 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | Random             |     3,508.5 ns |    250.40 ns |    130.97 ns |  0.88 |    0.04 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | Random             |     4,284.7 ns |    794.96 ns |    352.97 ns |  1.08 |    0.09 |    1 |         - |          NA |
| BottomupHeapSort | 256  | Random             |     4,093.8 ns |    136.02 ns |     60.39 ns |  1.03 |    0.02 |    1 |         - |          NA |
| WeakHeapSort     | 256  | Random             |     9,149.8 ns |    397.77 ns |    208.04 ns |  2.31 |    0.06 |    4 |         - |          NA |
| SmoothSort       | 256  | Random             |     5,201.1 ns |    220.58 ns |     97.94 ns |  1.31 |    0.03 |    2 |         - |          NA |
| TournamentSort   | 256  | Random             |     7,602.1 ns |    598.81 ns |    313.19 ns |  1.92 |    0.08 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **SingleElementMoved** |     **3,872.4 ns** |     **49.77 ns** |     **22.10 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 256  | SingleElementMoved |     3,383.9 ns |     78.66 ns |     28.05 ns |  0.87 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 256  | SingleElementMoved |     4,112.8 ns |    112.65 ns |     50.02 ns |  1.06 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 256  | SingleElementMoved |     4,067.3 ns |     96.62 ns |     34.46 ns |  1.05 |    0.01 |    2 |         - |          NA |
| WeakHeapSort     | 256  | SingleElementMoved |     7,618.0 ns |     72.15 ns |     25.73 ns |  1.97 |    0.01 |    4 |         - |          NA |
| SmoothSort       | 256  | SingleElementMoved |     1,733.4 ns |     26.76 ns |      9.54 ns |  0.45 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 256  | SingleElementMoved |     5,305.2 ns |    461.73 ns |    241.49 ns |  1.37 |    0.06 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **Sorted**             |     **3,870.5 ns** |    **242.83 ns** |    **127.01 ns** |  **1.00** |    **0.04** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 256  | Sorted             |     3,466.5 ns |    164.33 ns |     72.96 ns |  0.90 |    0.03 |    2 |         - |          NA |
| TernaryHeapSort  | 256  | Sorted             |     4,320.7 ns |     84.53 ns |     30.15 ns |  1.12 |    0.03 |    2 |         - |          NA |
| BottomupHeapSort | 256  | Sorted             |     4,249.4 ns |    377.76 ns |    197.57 ns |  1.10 |    0.06 |    2 |         - |          NA |
| WeakHeapSort     | 256  | Sorted             |     8,084.5 ns |    347.95 ns |    181.98 ns |  2.09 |    0.08 |    3 |         - |          NA |
| SmoothSort       | 256  | Sorted             |     1,251.9 ns |     41.64 ns |     18.49 ns |  0.32 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 256  | Sorted             |     3,284.9 ns |    262.66 ns |    116.62 ns |  0.85 |    0.04 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **Reversed**           |     **3,977.0 ns** |    **346.95 ns** |    **181.46 ns** |  **1.00** |    **0.06** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | Reversed           |     3,303.0 ns |     63.34 ns |     33.13 ns |  0.83 |    0.04 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | Reversed           |     4,204.0 ns |    388.13 ns |    203.00 ns |  1.06 |    0.07 |    1 |         - |          NA |
| BottomupHeapSort | 256  | Reversed           |     4,580.8 ns |    456.28 ns |    238.64 ns |  1.15 |    0.07 |    1 |         - |          NA |
| WeakHeapSort     | 256  | Reversed           |     8,791.7 ns |    315.91 ns |    165.23 ns |  2.21 |    0.10 |    2 |         - |          NA |
| SmoothSort       | 256  | Reversed           |     4,720.4 ns |    214.19 ns |    112.03 ns |  1.19 |    0.06 |    1 |         - |          NA |
| TournamentSort   | 256  | Reversed           |     4,984.8 ns |    475.65 ns |    248.77 ns |  1.26 |    0.08 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **PipeOrgan**          |     **3,013.5 ns** |     **82.95 ns** |     **36.83 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | PipeOrgan          |     3,114.0 ns |    120.34 ns |     53.43 ns |  1.03 |    0.02 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | PipeOrgan          |     3,994.9 ns |    171.72 ns |     76.24 ns |  1.33 |    0.03 |    2 |         - |          NA |
| BottomupHeapSort | 256  | PipeOrgan          |     4,137.9 ns |    294.09 ns |    153.82 ns |  1.37 |    0.05 |    2 |         - |          NA |
| WeakHeapSort     | 256  | PipeOrgan          |     8,443.3 ns |    256.46 ns |    113.87 ns |  2.80 |    0.05 |    4 |         - |          NA |
| SmoothSort       | 256  | PipeOrgan          |     5,540.7 ns |    288.44 ns |    150.86 ns |  1.84 |    0.05 |    3 |         - |          NA |
| TournamentSort   | 256  | PipeOrgan          |     7,266.0 ns |    332.41 ns |    173.85 ns |  2.41 |    0.06 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Random**             |    **19,493.3 ns** |    **449.22 ns** |    **234.95 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Random             |    17,736.7 ns |    302.49 ns |    134.31 ns |  0.91 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | Random             |    20,429.4 ns |    452.34 ns |    200.84 ns |  1.05 |    0.02 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | Random             |    19,173.6 ns |    341.45 ns |    151.60 ns |  0.98 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | Random             |    48,085.3 ns |    366.97 ns |    162.94 ns |  2.47 |    0.03 |    3 |         - |          NA |
| SmoothSort       | 1024 | Random             |    26,964.4 ns |    637.90 ns |    333.64 ns |  1.38 |    0.02 |    2 |         - |          NA |
| TournamentSort   | 1024 | Random             |    66,191.1 ns | 15,332.42 ns |  8,019.15 ns |  3.40 |    0.39 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **SingleElementMoved** |    **21,865.6 ns** |    **658.49 ns** |    **292.38 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | SingleElementMoved |    16,932.7 ns |    298.20 ns |    132.40 ns |  0.77 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | SingleElementMoved |    21,253.5 ns |  1,098.67 ns |    574.62 ns |  0.97 |    0.03 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | SingleElementMoved |    19,329.4 ns |    404.30 ns |    211.46 ns |  0.88 |    0.01 |    2 |         - |          NA |
| WeakHeapSort     | 1024 | SingleElementMoved |    40,600.7 ns |    451.70 ns |    236.25 ns |  1.86 |    0.03 |    3 |         - |          NA |
| SmoothSort       | 1024 | SingleElementMoved |     7,722.2 ns |    282.88 ns |    147.95 ns |  0.35 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 1024 | SingleElementMoved |    24,431.0 ns |    253.47 ns |    112.54 ns |  1.12 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Sorted**             |    **22,615.6 ns** |    **708.12 ns** |    **314.41 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Sorted             |    16,939.7 ns |    323.41 ns |    143.60 ns |  0.75 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | Sorted             |    22,926.7 ns |  1,314.39 ns |    583.60 ns |  1.01 |    0.03 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | Sorted             |    19,308.3 ns |    491.54 ns |    257.08 ns |  0.85 |    0.02 |    2 |         - |          NA |
| WeakHeapSort     | 1024 | Sorted             |    41,118.2 ns |    386.16 ns |    171.46 ns |  1.82 |    0.02 |    3 |         - |          NA |
| SmoothSort       | 1024 | Sorted             |     5,003.8 ns |     88.86 ns |     31.69 ns |  0.22 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 1024 | Sorted             |    15,363.8 ns |    423.92 ns |    188.22 ns |  0.68 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Reversed**           |    **19,453.5 ns** |    **548.25 ns** |    **286.74 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Reversed           |    18,817.6 ns |  1,115.68 ns |    495.37 ns |  0.97 |    0.03 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | Reversed           |    19,401.9 ns |    340.24 ns |    151.07 ns |  1.00 |    0.02 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | Reversed           |    19,841.9 ns |    352.22 ns |    156.39 ns |  1.02 |    0.02 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | Reversed           |    44,674.9 ns |    284.10 ns |    148.59 ns |  2.30 |    0.03 |    2 |         - |          NA |
| SmoothSort       | 1024 | Reversed           |    23,019.0 ns |    316.26 ns |    165.41 ns |  1.18 |    0.02 |    1 |         - |          NA |
| TournamentSort   | 1024 | Reversed           |    24,822.8 ns |    691.23 ns |    306.91 ns |  1.28 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **PipeOrgan**          |    **15,878.7 ns** |    **243.07 ns** |    **107.92 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | PipeOrgan          |    16,281.7 ns |    284.54 ns |    126.34 ns |  1.03 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | PipeOrgan          |    18,239.2 ns |    247.94 ns |    110.09 ns |  1.15 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | PipeOrgan          |    19,483.9 ns |    247.97 ns |    110.10 ns |  1.23 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | PipeOrgan          |    43,834.2 ns |    380.16 ns |    168.79 ns |  2.76 |    0.02 |    3 |         - |          NA |
| SmoothSort       | 1024 | PipeOrgan          |    28,039.7 ns |    985.28 ns |    515.32 ns |  1.77 |    0.03 |    2 |         - |          NA |
| TournamentSort   | 1024 | PipeOrgan          |    43,134.3 ns |  4,177.78 ns |  2,185.06 ns |  2.72 |    0.13 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Random**             |   **498,213.4 ns** |  **1,537.24 ns** |    **682.54 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Random             |   485,383.2 ns |  1,401.96 ns |    499.95 ns |  0.97 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | Random             |   583,915.8 ns |  1,752.20 ns |    916.43 ns |  1.17 |    0.00 |    1 |         - |          NA |
| BottomupHeapSort | 8192 | Random             |   580,235.8 ns |  2,035.04 ns |    903.57 ns |  1.16 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | Random             |   886,173.1 ns |  1,877.26 ns |    833.52 ns |  1.78 |    0.00 |    2 |         - |          NA |
| SmoothSort       | 8192 | Random             |   818,298.3 ns |  4,211.24 ns |  2,202.56 ns |  1.64 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 8192 | Random             | 1,349,709.4 ns |  8,100.41 ns |  3,596.63 ns |  2.71 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **SingleElementMoved** |   **369,330.1 ns** |    **960.45 ns** |    **502.33 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 8192 | SingleElementMoved |   379,621.4 ns |  1,467.53 ns |    767.55 ns |  1.03 |    0.00 |    2 |         - |          NA |
| TernaryHeapSort  | 8192 | SingleElementMoved |   413,322.8 ns |  1,000.05 ns |    523.05 ns |  1.12 |    0.00 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | SingleElementMoved |   445,874.2 ns |  1,224.88 ns |    640.64 ns |  1.21 |    0.00 |    2 |         - |          NA |
| WeakHeapSort     | 8192 | SingleElementMoved |   432,895.8 ns |  1,174.00 ns |    614.03 ns |  1.17 |    0.00 |    2 |         - |          NA |
| SmoothSort       | 8192 | SingleElementMoved |    59,319.4 ns |    808.36 ns |    358.92 ns |  0.16 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 8192 | SingleElementMoved |   773,441.6 ns |  4,015.61 ns |  2,100.24 ns |  2.09 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Sorted**             |   **370,136.7 ns** |  **1,302.53 ns** |    **681.25 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Sorted             |   379,673.7 ns |    678.96 ns |    355.11 ns |  1.03 |    0.00 |    2 |         - |          NA |
| TernaryHeapSort  | 8192 | Sorted             |   414,888.4 ns |    558.72 ns |    248.08 ns |  1.12 |    0.00 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | Sorted             |   447,983.8 ns |  1,334.22 ns |    697.82 ns |  1.21 |    0.00 |    2 |         - |          NA |
| WeakHeapSort     | 8192 | Sorted             |   442,273.9 ns |    922.24 ns |    482.35 ns |  1.19 |    0.00 |    2 |         - |          NA |
| SmoothSort       | 8192 | Sorted             |    41,943.6 ns |  1,066.83 ns |    557.97 ns |  0.11 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 8192 | Sorted             |   511,689.2 ns | 12,464.73 ns |  6,519.29 ns |  1.38 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Reversed**           |   **396,274.6 ns** |  **1,290.86 ns** |    **573.15 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Reversed           |   353,061.8 ns |  1,380.70 ns |    613.04 ns |  0.89 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | Reversed           |   432,601.9 ns | 13,188.97 ns |  6,898.09 ns |  1.09 |    0.02 |    1 |         - |          NA |
| BottomupHeapSort | 8192 | Reversed           |   480,600.8 ns |  5,871.03 ns |  2,606.78 ns |  1.21 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | Reversed           |   472,355.8 ns |  2,003.92 ns |    889.75 ns |  1.19 |    0.00 |    1 |         - |          NA |
| SmoothSort       | 8192 | Reversed           |   519,087.4 ns | 26,144.74 ns | 13,674.21 ns |  1.31 |    0.03 |    1 |         - |          NA |
| TournamentSort   | 8192 | Reversed           |   728,215.1 ns |  2,911.03 ns |  1,292.52 ns |  1.84 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **PipeOrgan**          |   **367,915.2 ns** |  **1,314.01 ns** |    **687.25 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | PipeOrgan          |   380,696.2 ns |  1,237.08 ns |    647.02 ns |  1.03 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | PipeOrgan          |   455,131.7 ns |  2,788.42 ns |  1,458.40 ns |  1.24 |    0.00 |    1 |         - |          NA |
| BottomupHeapSort | 8192 | PipeOrgan          |   454,114.9 ns |    793.01 ns |    414.76 ns |  1.23 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | PipeOrgan          |   502,811.4 ns |  2,278.37 ns |  1,011.61 ns |  1.37 |    0.00 |    1 |         - |          NA |
| SmoothSort       | 8192 | PipeOrgan          |   655,418.0 ns |  2,306.86 ns |  1,206.53 ns |  1.78 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 8192 | PipeOrgan          | 1,075,623.1 ns | 14,033.00 ns |  7,339.53 ns |  2.92 |    0.02 |    3 |         - |          NA |

### InsertionBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                 | Size | Pattern            | Mean         | Error       | StdDev      | Median       | Ratio  | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------- |----- |------------------- |-------------:|------------:|------------:|-------------:|-------:|--------:|-----:|----------:|------------:|
| **InsertionSort**          | **256**  | **Random**             |   **8,676.1 ns** |   **396.41 ns** |   **207.33 ns** |   **8,687.2 ns** |   **1.00** |    **0.03** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Random             |  11,047.6 ns | 6,756.46 ns | 3,533.76 ns |   8,687.7 ns |   1.27 |    0.39 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | Random             |   9,439.8 ns |   366.94 ns |   191.92 ns |   9,468.4 ns |   1.09 |    0.03 |    2 |         - |          NA |
| GnomeSort              | 256  | Random             |  28,049.9 ns |   311.61 ns |   162.98 ns |  28,049.9 ns |   3.23 |    0.08 |    4 |         - |          NA |
| LibrarySort            | 256  | Random             |  16,227.2 ns |   243.98 ns |   127.61 ns |  16,213.3 ns |   1.87 |    0.04 |    3 |         - |          NA |
| MergeInsertionSort     | 256  | Random             |  34,525.5 ns | 1,123.44 ns |   587.58 ns |  34,766.2 ns |   3.98 |    0.11 |    5 |         - |          NA |
| ShellSortKnuth1973     | 256  | Random             |   3,024.4 ns |   260.98 ns |   115.88 ns |   2,980.7 ns |   0.35 |    0.01 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Random             |   3,175.4 ns |   618.22 ns |   323.34 ns |   2,994.9 ns |   0.37 |    0.04 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | Random             |   3,167.3 ns |    33.83 ns |    12.06 ns |   3,165.2 ns |   0.37 |    0.01 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | Random             |   3,184.1 ns |   204.02 ns |    90.59 ns |   3,188.8 ns |   0.37 |    0.01 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Random             |   3,111.5 ns |   264.95 ns |   117.64 ns |   3,059.3 ns |   0.36 |    0.02 |    1 |         - |          NA |
|      |                    |              |             |             |              |        |         |      |           |             |
| **InsertionSort**          | **256**  | **SingleElementMoved** |     **427.9 ns** |     **3.49 ns** |     **1.83 ns** |     **428.1 ns** |   **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 256  | SingleElementMoved |     289.5 ns |     4.73 ns |     1.69 ns |     289.0 ns |   0.68 |    0.00 |    1 |         - |          NA |
| BinaryInsertSort       | 256  | SingleElementMoved |     974.3 ns |     5.43 ns |     2.84 ns |     973.9 ns |   2.28 |    0.01 |    3 |         - |          NA |
| GnomeSort              | 256  | SingleElementMoved |     465.9 ns |     4.91 ns |     2.18 ns |     465.1 ns |   1.09 |    0.01 |    2 |         - |          NA |
| LibrarySort            | 256  | SingleElementMoved |   8,468.2 ns |   369.85 ns |   193.44 ns |   8,535.3 ns |  19.79 |    0.43 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | SingleElementMoved |  25,376.7 ns |   754.78 ns |   394.77 ns |  25,427.5 ns |  59.31 |    0.90 |    6 |         - |          NA |
| ShellSortKnuth1973     | 256  | SingleElementMoved |   1,288.7 ns |    11.41 ns |     5.07 ns |   1,286.1 ns |   3.01 |    0.02 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 256  | SingleElementMoved |   1,287.3 ns |    12.90 ns |     6.75 ns |   1,288.5 ns |   3.01 |    0.02 |    4 |         - |          NA |
| ShellSortTokuda1992    | 256  | SingleElementMoved |   1,590.3 ns |     8.15 ns |     4.26 ns |   1,590.0 ns |   3.72 |    0.02 |    4 |         - |          NA |
| ShellSortCiura2001     | 256  | SingleElementMoved |   1,386.7 ns |    12.08 ns |     5.36 ns |   1,386.2 ns |   3.24 |    0.02 |    4 |         - |          NA |
| ShellSortLee2021       | 256  | SingleElementMoved |   1,802.1 ns |   444.81 ns |   197.50 ns |   1,837.9 ns |   4.21 |    0.43 |    4 |         - |          NA |
|      |                    |              |             |             |              |        |         |      |           |             |
| **InsertionSort**          | **256**  | **Sorted**             |     **323.9 ns** |     **1.71 ns** |     **0.61 ns** |     **324.0 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Sorted             |     211.7 ns |     1.73 ns |     0.91 ns |     211.4 ns |   0.65 |    0.00 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | Sorted             |     174.0 ns |     0.91 ns |     0.41 ns |     173.9 ns |   0.54 |    0.00 |    1 |         - |          NA |
| GnomeSort              | 256  | Sorted             |     245.0 ns |     2.13 ns |     0.94 ns |     244.8 ns |   0.76 |    0.00 |    2 |         - |          NA |
| LibrarySort            | 256  | Sorted             |   7,080.8 ns |    54.57 ns |    28.54 ns |   7,086.3 ns |  21.86 |    0.09 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | Sorted             |  24,980.5 ns |   367.68 ns |   192.31 ns |  25,046.0 ns |  77.12 |    0.58 |    6 |         - |          NA |
| ShellSortKnuth1973     | 256  | Sorted             |   1,067.0 ns |    10.02 ns |     4.45 ns |   1,065.3 ns |   3.29 |    0.01 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Sorted             |   1,073.1 ns |    16.52 ns |     5.89 ns |   1,071.0 ns |   3.31 |    0.02 |    4 |         - |          NA |
| ShellSortTokuda1992    | 256  | Sorted             |   1,468.5 ns |   430.00 ns |   224.90 ns |   1,315.5 ns |   4.53 |    0.66 |    4 |         - |          NA |
| ShellSortCiura2001     | 256  | Sorted             |   1,153.9 ns |    64.94 ns |    23.16 ns |   1,144.7 ns |   3.56 |    0.07 |    4 |         - |          NA |
| ShellSortLee2021       | 256  | Sorted             |   1,308.7 ns |     4.24 ns |     1.88 ns |   1,307.9 ns |   4.04 |    0.01 |    4 |         - |          NA |
|      |                    |              |             |             |              |        |         |      |           |             |
| **InsertionSort**          | **256**  | **Reversed**           |  **16,572.3 ns** |   **241.87 ns** |   **126.50 ns** |  **16,561.4 ns** |   **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Reversed           |  18,870.9 ns |   440.39 ns |   230.33 ns |  18,835.4 ns |   1.14 |    0.02 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | Reversed           |  16,960.5 ns |   355.38 ns |   185.87 ns |  16,982.9 ns |   1.02 |    0.01 |    2 |         - |          NA |
| GnomeSort              | 256  | Reversed           |  57,992.1 ns |   288.11 ns |   150.69 ns |  58,018.0 ns |   3.50 |    0.03 |    5 |         - |          NA |
| LibrarySort            | 256  | Reversed           |  38,701.1 ns |   347.10 ns |   181.54 ns |  38,712.4 ns |   2.34 |    0.02 |    4 |         - |          NA |
| MergeInsertionSort     | 256  | Reversed           |  25,060.8 ns |   778.17 ns |   345.51 ns |  24,952.8 ns |   1.51 |    0.02 |    3 |         - |          NA |
| ShellSortKnuth1973     | 256  | Reversed           |   1,777.7 ns |     8.32 ns |     2.97 ns |   1,779.1 ns |   0.11 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Reversed           |   1,913.0 ns |   521.30 ns |   231.46 ns |   1,783.1 ns |   0.12 |    0.01 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | Reversed           |   1,898.8 ns |    21.59 ns |     7.70 ns |   1,896.4 ns |   0.11 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | Reversed           |   1,792.1 ns |     8.20 ns |     4.29 ns |   1,791.3 ns |   0.11 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Reversed           |   1,877.8 ns |     9.09 ns |     3.24 ns |   1,878.0 ns |   0.11 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |              |        |         |      |           |             |
| **InsertionSort**          | **256**  | **PipeOrgan**          |   **9,407.9 ns** | **2,044.26 ns** | **1,069.19 ns** |   **9,043.4 ns** |   **1.01** |    **0.15** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 256  | PipeOrgan          |   9,655.6 ns |   445.94 ns |   233.23 ns |   9,663.0 ns |   1.04 |    0.11 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | PipeOrgan          |  10,221.3 ns |   382.87 ns |   200.25 ns |  10,176.9 ns |   1.10 |    0.11 |    2 |         - |          NA |
| GnomeSort              | 256  | PipeOrgan          |  23,621.2 ns |   188.09 ns |    98.38 ns |  23,627.7 ns |   2.54 |    0.25 |    3 |         - |          NA |
| LibrarySort            | 256  | PipeOrgan          |  24,187.4 ns |   358.99 ns |   187.76 ns |  24,136.4 ns |   2.60 |    0.26 |    3 |         - |          NA |
| MergeInsertionSort     | 256  | PipeOrgan          |  22,790.3 ns |    98.97 ns |    43.94 ns |  22,794.2 ns |   2.45 |    0.24 |    3 |         - |          NA |
| ShellSortKnuth1973     | 256  | PipeOrgan          |   1,554.9 ns |    22.04 ns |     7.86 ns |   1,556.7 ns |   0.17 |    0.02 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | PipeOrgan          |   1,667.2 ns |   396.80 ns |   207.53 ns |   1,545.9 ns |   0.18 |    0.03 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | PipeOrgan          |   1,755.6 ns |    15.01 ns |     7.85 ns |   1,754.7 ns |   0.19 |    0.02 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | PipeOrgan          |   1,661.2 ns |     9.96 ns |     4.42 ns |   1,662.1 ns |   0.18 |    0.02 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | PipeOrgan          |   1,777.2 ns |    14.95 ns |     6.64 ns |   1,775.9 ns |   0.19 |    0.02 |    1 |         - |          NA |
|      |                    |              |             |             |              |        |         |      |           |             |
| **InsertionSort**          | **1024** | **Random**             | **127,743.8 ns** | **1,421.41 ns** |   **631.11 ns** | **127,768.5 ns** |   **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Random             | 139,741.9 ns | 4,579.28 ns | 2,395.05 ns | 140,441.1 ns |   1.09 |    0.02 |    3 |         - |          NA |
| BinaryInsertSort       | 1024 | Random             | 146,898.9 ns |   755.38 ns |   335.39 ns | 146,841.7 ns |   1.15 |    0.01 |    3 |         - |          NA |
| GnomeSort              | 1024 | Random             | 424,446.8 ns |   859.36 ns |   449.46 ns | 424,448.6 ns |   3.32 |    0.02 |    4 |         - |          NA |
| LibrarySort            | 1024 | Random             |  82,560.0 ns | 1,480.60 ns |   774.38 ns |  82,395.1 ns |   0.65 |    0.01 |    2 |         - |          NA |
| MergeInsertionSort     | 1024 | Random             | 418,777.0 ns | 2,365.55 ns | 1,237.23 ns | 418,721.2 ns |   3.28 |    0.02 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Random             |  16,566.2 ns |   447.29 ns |   233.94 ns |  16,517.1 ns |   0.13 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Random             |  16,627.3 ns |   381.24 ns |   169.27 ns |  16,574.8 ns |   0.13 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Random             |  16,990.0 ns |   114.07 ns |    50.65 ns |  16,970.0 ns |   0.13 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Random             |  16,787.7 ns |   230.81 ns |   102.48 ns |  16,789.4 ns |   0.13 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Random             |  17,116.4 ns |   221.07 ns |    98.16 ns |  17,131.6 ns |   0.13 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |              |        |         |      |           |             |
| **InsertionSort**          | **1024** | **SingleElementMoved** |   **1,628.6 ns** |     **6.55 ns** |     **2.91 ns** |   **1,629.1 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | SingleElementMoved |   1,078.9 ns |    12.84 ns |     5.70 ns |   1,075.8 ns |   0.66 |    0.00 |    1 |         - |          NA |
| BinaryInsertSort       | 1024 | SingleElementMoved |   4,705.1 ns |    15.94 ns |     5.68 ns |   4,706.0 ns |   2.89 |    0.01 |    3 |         - |          NA |
| GnomeSort              | 1024 | SingleElementMoved |   1,806.5 ns |     1.88 ns |     0.84 ns |   1,806.5 ns |   1.11 |    0.00 |    2 |         - |          NA |
| LibrarySort            | 1024 | SingleElementMoved |  36,853.9 ns |   110.02 ns |    48.85 ns |  36,840.6 ns |  22.63 |    0.05 |    5 |         - |          NA |
| MergeInsertionSort     | 1024 | SingleElementMoved | 288,676.5 ns |   819.62 ns |   428.68 ns | 288,736.0 ns | 177.25 |    0.39 |    6 |         - |          NA |
| ShellSortKnuth1973     | 1024 | SingleElementMoved |   6,481.1 ns |   208.70 ns |    92.66 ns |   6,517.1 ns |   3.98 |    0.05 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | SingleElementMoved |   6,710.3 ns |   207.20 ns |    92.00 ns |   6,667.0 ns |   4.12 |    0.05 |    4 |         - |          NA |
| ShellSortTokuda1992    | 1024 | SingleElementMoved |   7,310.1 ns |    41.98 ns |    14.97 ns |   7,313.2 ns |   4.49 |    0.01 |    4 |         - |          NA |
| ShellSortCiura2001     | 1024 | SingleElementMoved |   7,329.3 ns |   290.24 ns |   151.80 ns |   7,386.7 ns |   4.50 |    0.09 |    4 |         - |          NA |
| ShellSortLee2021       | 1024 | SingleElementMoved |   7,251.4 ns |    23.20 ns |    12.13 ns |   7,245.1 ns |   4.45 |    0.01 |    4 |         - |          NA |
|      |                    |              |             |             |              |        |         |      |           |             |
| **InsertionSort**          | **1024** | **Sorted**             |   **1,270.6 ns** |     **2.17 ns** |     **1.13 ns** |   **1,270.4 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Sorted             |     804.2 ns |     1.22 ns |     0.64 ns |     804.2 ns |   0.63 |    0.00 |    2 |         - |          NA |
| BinaryInsertSort       | 1024 | Sorted             |     650.0 ns |     2.62 ns |     1.37 ns |     650.3 ns |   0.51 |    0.00 |    1 |         - |          NA |
| GnomeSort              | 1024 | Sorted             |     958.2 ns |     0.81 ns |     0.29 ns |     958.1 ns |   0.75 |    0.00 |    2 |         - |          NA |
| LibrarySort            | 1024 | Sorted             |  35,625.4 ns | 9,053.69 ns | 4,735.26 ns |  35,908.7 ns |  28.04 |    3.51 |    5 |         - |          NA |
| MergeInsertionSort     | 1024 | Sorted             | 287,630.6 ns |   458.00 ns |   163.33 ns | 287,676.3 ns | 226.37 |    0.22 |    6 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Sorted             |   5,315.5 ns |   238.22 ns |   124.59 ns |   5,242.5 ns |   4.18 |    0.09 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Sorted             |   5,873.3 ns |     5.49 ns |     1.96 ns |   5,872.5 ns |   4.62 |    0.00 |    4 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Sorted             |   6,514.3 ns |   595.17 ns |   311.28 ns |   6,310.2 ns |   5.13 |    0.23 |    4 |         - |          NA |
| ShellSortCiura2001     | 1024 | Sorted             |   6,547.3 ns |   400.18 ns |   209.30 ns |   6,491.3 ns |   5.15 |    0.16 |    4 |         - |          NA |
| ShellSortLee2021       | 1024 | Sorted             |   6,311.7 ns |     1.67 ns |     0.87 ns |   6,311.7 ns |   4.97 |    0.00 |    4 |         - |          NA |
|      |                    |              |             |             |              |        |         |      |           |             |
| **InsertionSort**          | **1024** | **Reversed**           | **250,391.3 ns** | **1,214.84 ns** |   **635.39 ns** | **250,322.6 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Reversed           | 280,637.9 ns |   972.19 ns |   431.66 ns | 280,560.3 ns |   1.12 |    0.00 |    2 |         - |          NA |
| BinaryInsertSort       | 1024 | Reversed           | 232,343.8 ns | 2,770.79 ns | 1,449.18 ns | 231,726.2 ns |   0.93 |    0.01 |    2 |         - |          NA |
| GnomeSort              | 1024 | Reversed           | 845,979.1 ns | 3,211.23 ns | 1,425.81 ns | 845,591.1 ns |   3.38 |    0.01 |    4 |         - |          NA |
| LibrarySort            | 1024 | Reversed           | 418,048.2 ns | 1,321.95 ns |   586.96 ns | 418,025.7 ns |   1.67 |    0.00 |    3 |         - |          NA |
| MergeInsertionSort     | 1024 | Reversed           | 288,070.7 ns | 1,790.92 ns |   936.69 ns | 288,070.4 ns |   1.15 |    0.00 |    2 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Reversed           |   8,540.3 ns |   594.34 ns |   263.89 ns |   8,387.6 ns |   0.03 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Reversed           |   8,888.4 ns |   293.47 ns |   153.49 ns |   8,886.4 ns |   0.04 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Reversed           |  10,241.5 ns |   868.81 ns |   454.40 ns |  10,206.7 ns |   0.04 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Reversed           |   9,451.3 ns |   375.67 ns |   196.48 ns |   9,527.3 ns |   0.04 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Reversed           |  10,046.5 ns |   491.15 ns |   256.88 ns |  10,159.6 ns |   0.04 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |              |        |         |      |           |             |
| **InsertionSort**          | **1024** | **PipeOrgan**          | **133,233.8 ns** | **3,912.90 ns** | **2,046.52 ns** | **132,022.0 ns** |   **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | PipeOrgan          | 145,022.7 ns | 3,577.39 ns | 1,871.04 ns | 145,245.3 ns |   1.09 |    0.02 |    2 |         - |          NA |
| BinaryInsertSort       | 1024 | PipeOrgan          | 130,221.0 ns |   191.86 ns |    68.42 ns | 130,250.8 ns |   0.98 |    0.01 |    2 |         - |          NA |
| GnomeSort              | 1024 | PipeOrgan          | 340,609.3 ns | 1,359.49 ns |   603.62 ns | 340,313.4 ns |   2.56 |    0.04 |    4 |         - |          NA |
| LibrarySort            | 1024 | PipeOrgan          | 272,035.4 ns |   897.76 ns |   469.55 ns | 272,034.2 ns |   2.04 |    0.03 |    3 |         - |          NA |
| MergeInsertionSort     | 1024 | PipeOrgan          | 249,680.4 ns | 5,610.54 ns | 2,934.42 ns | 248,540.3 ns |   1.87 |    0.03 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | PipeOrgan          |   8,155.8 ns |   442.64 ns |   231.51 ns |   8,023.4 ns |   0.06 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | PipeOrgan          |   8,629.7 ns |   566.53 ns |   296.31 ns |   8,534.6 ns |   0.06 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | PipeOrgan          |   9,397.4 ns |   664.70 ns |   347.65 ns |   9,404.7 ns |   0.07 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | PipeOrgan          |   9,311.4 ns |   487.22 ns |   254.83 ns |   9,394.3 ns |   0.07 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | PipeOrgan          |   9,370.9 ns |   650.59 ns |   340.27 ns |   9,318.1 ns |   0.07 |    0.00 |    1 |         - |          NA |

### IntKeyBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method             | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **QuickSort**          | **256**  | **Random**             |     **2,987.9 ns** |     **53.39 ns** |     **23.71 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 256  | Random             |     3,152.0 ns |    347.65 ns |    154.36 ns |  1.05 |    0.05 |    2 |         - |          NA |
| QuickSortMedian3   | 256  | Random             |     4,511.7 ns |    471.38 ns |    246.54 ns |  1.51 |    0.08 |    2 |         - |          NA |
| QuickSortMedian9   | 256  | Random             |     4,051.8 ns |    573.91 ns |    300.17 ns |  1.36 |    0.10 |    2 |         - |          NA |
| DualPivotQuickSort | 256  | Random             |     2,906.0 ns |    360.70 ns |    188.65 ns |  0.97 |    0.06 |    2 |         - |          NA |
| StableQuickSort    | 256  | Random             |    11,386.6 ns |    475.55 ns |    211.15 ns |  3.81 |    0.07 |    3 |         - |          NA |
| IntroSort          | 256  | Random             |     2,190.3 ns |     36.70 ns |     16.29 ns |  0.73 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | Random             |     1,891.4 ns |     55.09 ns |     24.46 ns |  0.63 |    0.01 |    1 |         - |          NA |
| PDQSort            | 256  | Random             |     1,901.1 ns |     92.75 ns |     33.08 ns |  0.64 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | Random             |     3,556.8 ns |    420.54 ns |    219.95 ns |  1.19 |    0.07 |    2 |         - |          NA |
| Ipnsort            | 256  | Random             |     5,039.6 ns |     75.50 ns |     26.93 ns |  1.69 |    0.01 |    2 |         - |          NA |
| StdSort            | 256  | Random             |     3,641.1 ns |    448.96 ns |    234.82 ns |  1.22 |    0.07 |    2 |         - |          NA |
| BlockQuickSort     | 256  | Random             |     2,847.3 ns |     47.77 ns |     21.21 ns |  0.95 |    0.01 |    2 |         - |          NA |
| DotnetSort         | 256  | Random             |     2,195.7 ns |    272.19 ns |    142.36 ns |  0.73 |    0.05 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **256**  | **SingleElementMoved** |     **1,580.2 ns** |     **45.31 ns** |     **20.12 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| QuickSort3way      | 256  | SingleElementMoved |     5,278.2 ns |    360.67 ns |    188.64 ns |  3.34 |    0.12 |    5 |         - |          NA |
| QuickSortMedian3   | 256  | SingleElementMoved |     5,289.5 ns |    513.63 ns |    268.64 ns |  3.35 |    0.17 |    5 |         - |          NA |
| QuickSortMedian9   | 256  | SingleElementMoved |     4,314.1 ns |    348.95 ns |    182.51 ns |  2.73 |    0.11 |    5 |         - |          NA |
| DualPivotQuickSort | 256  | SingleElementMoved |     4,351.7 ns |    439.93 ns |    195.33 ns |  2.75 |    0.12 |    5 |         - |          NA |
| StableQuickSort    | 256  | SingleElementMoved |     9,114.3 ns |    554.22 ns |    289.87 ns |  5.77 |    0.19 |    6 |         - |          NA |
| IntroSort          | 256  | SingleElementMoved |       911.3 ns |     10.19 ns |      4.52 ns |  0.58 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | SingleElementMoved |     1,123.1 ns |     11.12 ns |      5.82 ns |  0.71 |    0.01 |    2 |         - |          NA |
| PDQSort            | 256  | SingleElementMoved |     1,328.3 ns |    186.10 ns |     97.33 ns |  0.84 |    0.06 |    3 |         - |          NA |
| PDQSortBranchless  | 256  | SingleElementMoved |     1,451.3 ns |     46.74 ns |     20.75 ns |  0.92 |    0.02 |    3 |         - |          NA |
| Ipnsort            | 256  | SingleElementMoved |     4,757.0 ns |     34.31 ns |     12.24 ns |  3.01 |    0.04 |    5 |         - |          NA |
| StdSort            | 256  | SingleElementMoved |     2,742.1 ns |     41.32 ns |     14.74 ns |  1.74 |    0.02 |    4 |         - |          NA |
| BlockQuickSort     | 256  | SingleElementMoved |     1,496.0 ns |     35.14 ns |     18.38 ns |  0.95 |    0.02 |    3 |         - |          NA |
| DotnetSort         | 256  | SingleElementMoved |     1,132.7 ns |     19.76 ns |     10.33 ns |  0.72 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **256**  | **Sorted**             |     **1,116.8 ns** |     **24.72 ns** |     **12.93 ns** |  **1.00** |    **0.02** |    **4** |         **-** |          **NA** |
| QuickSort3way      | 256  | Sorted             |     6,350.7 ns |    173.62 ns |     77.09 ns |  5.69 |    0.09 |    6 |         - |          NA |
| QuickSortMedian3   | 256  | Sorted             |     6,635.1 ns |    278.92 ns |    145.88 ns |  5.94 |    0.14 |    6 |         - |          NA |
| QuickSortMedian9   | 256  | Sorted             |     4,578.9 ns |     67.82 ns |     24.18 ns |  4.10 |    0.05 |    5 |         - |          NA |
| DualPivotQuickSort | 256  | Sorted             |     4,853.1 ns |    525.37 ns |    274.78 ns |  4.35 |    0.24 |    5 |         - |          NA |
| StableQuickSort    | 256  | Sorted             |     8,638.5 ns |    312.44 ns |    163.41 ns |  7.74 |    0.16 |    7 |         - |          NA |
| IntroSort          | 256  | Sorted             |       306.7 ns |      1.58 ns |      0.83 ns |  0.27 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet    | 256  | Sorted             |     1,042.9 ns |     15.28 ns |      7.99 ns |  0.93 |    0.01 |    4 |         - |          NA |
| PDQSort            | 256  | Sorted             |       300.8 ns |      3.86 ns |      1.71 ns |  0.27 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless  | 256  | Sorted             |       300.4 ns |      3.12 ns |      1.63 ns |  0.27 |    0.00 |    2 |         - |          NA |
| Ipnsort            | 256  | Sorted             |       161.9 ns |     46.00 ns |     20.42 ns |  0.14 |    0.02 |    1 |         - |          NA |
| StdSort            | 256  | Sorted             |       710.3 ns |      3.41 ns |      1.78 ns |  0.64 |    0.01 |    3 |         - |          NA |
| BlockQuickSort     | 256  | Sorted             |     1,273.9 ns |     11.93 ns |      5.30 ns |  1.14 |    0.01 |    4 |         - |          NA |
| DotnetSort         | 256  | Sorted             |       919.1 ns |      7.29 ns |      3.81 ns |  0.82 |    0.01 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **256**  | **Reversed**           |       **958.1 ns** |     **26.36 ns** |     **11.70 ns** |  **1.00** |    **0.02** |    **4** |         **-** |          **NA** |
| QuickSort3way      | 256  | Reversed           |     5,508.8 ns |    452.26 ns |    236.54 ns |  5.75 |    0.24 |    6 |         - |          NA |
| QuickSortMedian3   | 256  | Reversed           |     7,103.9 ns |     26.88 ns |     11.93 ns |  7.42 |    0.09 |    7 |         - |          NA |
| QuickSortMedian9   | 256  | Reversed           |     5,099.5 ns |    448.07 ns |    234.35 ns |  5.32 |    0.24 |    6 |         - |          NA |
| DualPivotQuickSort | 256  | Reversed           |     4,706.4 ns |    485.06 ns |    253.69 ns |  4.91 |    0.26 |    6 |         - |          NA |
| StableQuickSort    | 256  | Reversed           |     9,163.1 ns |    423.44 ns |    221.46 ns |  9.56 |    0.24 |    8 |         - |          NA |
| IntroSort          | 256  | Reversed           |       636.6 ns |      4.88 ns |      2.17 ns |  0.66 |    0.01 |    3 |         - |          NA |
| IntroSortDotnet    | 256  | Reversed           |     1,612.9 ns |     49.86 ns |     26.08 ns |  1.68 |    0.03 |    5 |         - |          NA |
| PDQSort            | 256  | Reversed           |       529.5 ns |      4.34 ns |      1.93 ns |  0.55 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless  | 256  | Reversed           |       925.7 ns |      6.45 ns |      2.87 ns |  0.97 |    0.01 |    4 |         - |          NA |
| Ipnsort            | 256  | Reversed           |       216.6 ns |      2.29 ns |      1.02 ns |  0.23 |    0.00 |    1 |         - |          NA |
| StdSort            | 256  | Reversed           |       964.2 ns |    187.05 ns |     83.05 ns |  1.01 |    0.08 |    4 |         - |          NA |
| BlockQuickSort     | 256  | Reversed           |     1,601.3 ns |     17.99 ns |      9.41 ns |  1.67 |    0.02 |    5 |         - |          NA |
| DotnetSort         | 256  | Reversed           |     1,407.4 ns |     38.67 ns |     17.17 ns |  1.47 |    0.02 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **256**  | **PipeOrgan**          |     **7,916.9 ns** |    **477.11 ns** |    **249.54 ns** |  **1.00** |    **0.04** |    **3** |         **-** |          **NA** |
| QuickSort3way      | 256  | PipeOrgan          |     5,575.8 ns |    380.88 ns |    199.21 ns |  0.70 |    0.03 |    2 |         - |          NA |
| QuickSortMedian3   | 256  | PipeOrgan          |     6,429.2 ns |     57.82 ns |     25.67 ns |  0.81 |    0.02 |    2 |         - |          NA |
| QuickSortMedian9   | 256  | PipeOrgan          |     4,149.9 ns |    203.25 ns |    106.30 ns |  0.52 |    0.02 |    2 |         - |          NA |
| DualPivotQuickSort | 256  | PipeOrgan          |     2,337.5 ns |    438.01 ns |    229.09 ns |  0.30 |    0.03 |    1 |         - |          NA |
| StableQuickSort    | 256  | PipeOrgan          |     9,158.3 ns |    348.39 ns |    182.22 ns |  1.16 |    0.04 |    3 |         - |          NA |
| IntroSort          | 256  | PipeOrgan          |     1,978.0 ns |     45.14 ns |     20.04 ns |  0.25 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | PipeOrgan          |     2,508.1 ns |     94.80 ns |     33.81 ns |  0.32 |    0.01 |    1 |         - |          NA |
| PDQSort            | 256  | PipeOrgan          |     1,757.5 ns |     34.94 ns |     15.51 ns |  0.22 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | PipeOrgan          |     3,182.8 ns |     44.16 ns |     15.75 ns |  0.40 |    0.01 |    1 |         - |          NA |
| Ipnsort            | 256  | PipeOrgan          |     5,287.6 ns |    295.92 ns |    154.77 ns |  0.67 |    0.03 |    2 |         - |          NA |
| StdSort            | 256  | PipeOrgan          |     3,869.9 ns |     87.08 ns |     31.05 ns |  0.49 |    0.01 |    2 |         - |          NA |
| BlockQuickSort     | 256  | PipeOrgan          |     4,392.8 ns |     84.99 ns |     30.31 ns |  0.56 |    0.02 |    2 |         - |          NA |
| DotnetSort         | 256  | PipeOrgan          |     2,849.6 ns |    614.36 ns |    321.32 ns |  0.36 |    0.04 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **1024** | **Random**             |    **15,455.5 ns** |    **484.76 ns** |    **253.54 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 1024 | Random             |    18,733.4 ns |  1,096.94 ns |    573.72 ns |  1.21 |    0.04 |    2 |         - |          NA |
| QuickSortMedian3   | 1024 | Random             |    26,360.4 ns |  4,552.90 ns |  2,381.25 ns |  1.71 |    0.15 |    3 |         - |          NA |
| QuickSortMedian9   | 1024 | Random             |    23,012.8 ns |  4,706.52 ns |  2,461.60 ns |  1.49 |    0.15 |    3 |         - |          NA |
| DualPivotQuickSort | 1024 | Random             |    12,586.3 ns |    399.24 ns |    208.81 ns |  0.81 |    0.02 |    1 |         - |          NA |
| StableQuickSort    | 1024 | Random             |    84,656.8 ns |    735.82 ns |    384.85 ns |  5.48 |    0.09 |    4 |         - |          NA |
| IntroSort          | 1024 | Random             |    12,069.7 ns |    497.97 ns |    260.45 ns |  0.78 |    0.02 |    1 |         - |          NA |
| IntroSortDotnet    | 1024 | Random             |     9,972.3 ns |    326.93 ns |    170.99 ns |  0.65 |    0.01 |    1 |         - |          NA |
| PDQSort            | 1024 | Random             |     9,816.2 ns |    617.49 ns |    322.96 ns |  0.64 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless  | 1024 | Random             |    17,202.7 ns |    406.34 ns |    180.42 ns |  1.11 |    0.02 |    2 |         - |          NA |
| Ipnsort            | 1024 | Random             |    23,614.9 ns |    805.86 ns |    421.48 ns |  1.53 |    0.03 |    3 |         - |          NA |
| StdSort            | 1024 | Random             |    15,308.0 ns |    195.30 ns |    102.15 ns |  0.99 |    0.02 |    2 |         - |          NA |
| BlockQuickSort     | 1024 | Random             |    16,287.8 ns |    334.57 ns |    148.55 ns |  1.05 |    0.02 |    2 |         - |          NA |
| DotnetSort         | 1024 | Random             |    11,434.0 ns |    503.50 ns |    263.34 ns |  0.74 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **1024** | **SingleElementMoved** |     **7,563.8 ns** |    **597.70 ns** |    **312.61 ns** |  **1.00** |    **0.05** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 1024 | SingleElementMoved |    35,177.6 ns |    447.76 ns |    234.19 ns |  4.66 |    0.18 |    6 |         - |          NA |
| QuickSortMedian3   | 1024 | SingleElementMoved |    31,401.4 ns |    232.92 ns |    103.42 ns |  4.16 |    0.16 |    6 |         - |          NA |
| QuickSortMedian9   | 1024 | SingleElementMoved |    22,145.2 ns |  1,798.71 ns |    940.76 ns |  2.93 |    0.16 |    5 |         - |          NA |
| DualPivotQuickSort | 1024 | SingleElementMoved |    23,086.4 ns |    275.42 ns |    144.05 ns |  3.06 |    0.12 |    5 |         - |          NA |
| StableQuickSort    | 1024 | SingleElementMoved |    42,330.8 ns |    181.10 ns |     80.41 ns |  5.60 |    0.22 |    7 |         - |          NA |
| IntroSort          | 1024 | SingleElementMoved |     4,454.9 ns |    310.16 ns |    162.22 ns |  0.59 |    0.03 |    1 |         - |          NA |
| IntroSortDotnet    | 1024 | SingleElementMoved |     6,611.6 ns |    252.42 ns |    132.02 ns |  0.88 |    0.04 |    2 |         - |          NA |
| PDQSort            | 1024 | SingleElementMoved |     5,221.7 ns |    299.32 ns |    156.55 ns |  0.69 |    0.03 |    1 |         - |          NA |
| PDQSortBranchless  | 1024 | SingleElementMoved |     6,582.0 ns |    541.27 ns |    283.10 ns |  0.87 |    0.05 |    2 |         - |          NA |
| Ipnsort            | 1024 | SingleElementMoved |    22,871.3 ns |     75.65 ns |     33.59 ns |  3.03 |    0.12 |    5 |         - |          NA |
| StdSort            | 1024 | SingleElementMoved |    11,769.8 ns |    470.26 ns |    245.96 ns |  1.56 |    0.07 |    4 |         - |          NA |
| BlockQuickSort     | 1024 | SingleElementMoved |     9,143.8 ns |    600.81 ns |    266.77 ns |  1.21 |    0.06 |    3 |         - |          NA |
| DotnetSort         | 1024 | SingleElementMoved |     6,352.9 ns |     59.12 ns |     30.92 ns |  0.84 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **1024** | **Sorted**             |     **5,742.7 ns** |    **514.25 ns** |    **228.33 ns** |  **1.00** |    **0.05** |    **4** |         **-** |          **NA** |
| QuickSort3way      | 1024 | Sorted             |    47,022.3 ns |  1,197.89 ns |    531.87 ns |  8.20 |    0.32 |    7 |         - |          NA |
| QuickSortMedian3   | 1024 | Sorted             |    43,284.4 ns |    248.42 ns |    110.30 ns |  7.55 |    0.29 |    7 |         - |          NA |
| QuickSortMedian9   | 1024 | Sorted             |    22,430.9 ns |    633.11 ns |    331.13 ns |  3.91 |    0.16 |    6 |         - |          NA |
| DualPivotQuickSort | 1024 | Sorted             |    24,494.6 ns |    219.02 ns |     97.25 ns |  4.27 |    0.16 |    6 |         - |          NA |
| StableQuickSort    | 1024 | Sorted             |    42,245.7 ns |    189.95 ns |     99.35 ns |  7.37 |    0.28 |    7 |         - |          NA |
| IntroSort          | 1024 | Sorted             |     1,118.1 ns |      6.64 ns |      2.95 ns |  0.19 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet    | 1024 | Sorted             |     5,044.5 ns |    268.33 ns |    140.34 ns |  0.88 |    0.04 |    4 |         - |          NA |
| PDQSort            | 1024 | Sorted             |     1,017.9 ns |      3.75 ns |      1.66 ns |  0.18 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless  | 1024 | Sorted             |     1,026.3 ns |     31.17 ns |     16.30 ns |  0.18 |    0.01 |    2 |         - |          NA |
| Ipnsort            | 1024 | Sorted             |       512.6 ns |      3.80 ns |      1.69 ns |  0.09 |    0.00 |    1 |         - |          NA |
| StdSort            | 1024 | Sorted             |     2,612.1 ns |      4.02 ns |      1.78 ns |  0.46 |    0.02 |    3 |         - |          NA |
| BlockQuickSort     | 1024 | Sorted             |     7,626.0 ns |    164.90 ns |     86.24 ns |  1.33 |    0.05 |    5 |         - |          NA |
| DotnetSort         | 1024 | Sorted             |     4,615.8 ns |    253.86 ns |    112.71 ns |  0.80 |    0.04 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **1024** | **Reversed**           |     **4,613.3 ns** |    **242.14 ns** |    **107.51 ns** |  **1.00** |    **0.03** |    **3** |         **-** |          **NA** |
| QuickSort3way      | 1024 | Reversed           |    38,780.7 ns |    751.73 ns |    333.77 ns |  8.41 |    0.19 |    6 |         - |          NA |
| QuickSortMedian3   | 1024 | Reversed           |    51,808.3 ns |    198.02 ns |     87.92 ns | 11.24 |    0.24 |    6 |         - |          NA |
| QuickSortMedian9   | 1024 | Reversed           |    22,831.5 ns |    102.60 ns |     36.59 ns |  4.95 |    0.11 |    5 |         - |          NA |
| DualPivotQuickSort | 1024 | Reversed           |    24,331.4 ns |    195.49 ns |    102.24 ns |  5.28 |    0.12 |    5 |         - |          NA |
| StableQuickSort    | 1024 | Reversed           |    45,606.8 ns |    147.05 ns |     65.29 ns |  9.89 |    0.21 |    6 |         - |          NA |
| IntroSort          | 1024 | Reversed           |     4,001.2 ns |    548.40 ns |    243.49 ns |  0.87 |    0.05 |    3 |         - |          NA |
| IntroSortDotnet    | 1024 | Reversed           |     8,081.2 ns |     83.46 ns |     29.76 ns |  1.75 |    0.04 |    4 |         - |          NA |
| PDQSort            | 1024 | Reversed           |     1,919.0 ns |     18.92 ns |      8.40 ns |  0.42 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless  | 1024 | Reversed           |     3,329.8 ns |     63.48 ns |     22.64 ns |  0.72 |    0.02 |    3 |         - |          NA |
| Ipnsort            | 1024 | Reversed           |       767.8 ns |      0.92 ns |      0.33 ns |  0.17 |    0.00 |    1 |         - |          NA |
| StdSort            | 1024 | Reversed           |     3,712.8 ns |    874.52 ns |    457.39 ns |  0.81 |    0.10 |    3 |         - |          NA |
| BlockQuickSort     | 1024 | Reversed           |     7,729.0 ns |     66.30 ns |     23.64 ns |  1.68 |    0.04 |    4 |         - |          NA |
| DotnetSort         | 1024 | Reversed           |     7,473.3 ns |    443.24 ns |    231.82 ns |  1.62 |    0.06 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **1024** | **PipeOrgan**          |    **97,410.1 ns** |    **443.20 ns** |    **231.80 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way      | 1024 | PipeOrgan          |    35,296.1 ns |    403.62 ns |    211.10 ns |  0.36 |    0.00 |    4 |         - |          NA |
| QuickSortMedian3   | 1024 | PipeOrgan          |    38,318.3 ns |  1,216.31 ns |    636.15 ns |  0.39 |    0.01 |    4 |         - |          NA |
| QuickSortMedian9   | 1024 | PipeOrgan          |    21,758.3 ns |    522.92 ns |    232.18 ns |  0.22 |    0.00 |    3 |         - |          NA |
| DualPivotQuickSort | 1024 | PipeOrgan          |    12,713.3 ns |  1,358.18 ns |    710.35 ns |  0.13 |    0.01 |    2 |         - |          NA |
| StableQuickSort    | 1024 | PipeOrgan          |    45,294.0 ns |    192.56 ns |     85.50 ns |  0.46 |    0.00 |    4 |         - |          NA |
| IntroSort          | 1024 | PipeOrgan          |    15,320.5 ns |    530.40 ns |    277.41 ns |  0.16 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet    | 1024 | PipeOrgan          |    14,912.4 ns |    327.17 ns |    171.12 ns |  0.15 |    0.00 |    2 |         - |          NA |
| PDQSort            | 1024 | PipeOrgan          |     9,604.6 ns |    516.85 ns |    270.32 ns |  0.10 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 1024 | PipeOrgan          |    18,410.6 ns |    515.70 ns |    269.72 ns |  0.19 |    0.00 |    3 |         - |          NA |
| Ipnsort            | 1024 | PipeOrgan          |    25,402.3 ns |    394.83 ns |    206.50 ns |  0.26 |    0.00 |    3 |         - |          NA |
| StdSort            | 1024 | PipeOrgan          |    20,924.2 ns |    684.31 ns |    357.91 ns |  0.21 |    0.00 |    3 |         - |          NA |
| BlockQuickSort     | 1024 | PipeOrgan          |    24,692.5 ns |    477.58 ns |    249.78 ns |  0.25 |    0.00 |    3 |         - |          NA |
| DotnetSort         | 1024 | PipeOrgan          |    15,297.2 ns |  1,820.04 ns |    951.91 ns |  0.16 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **8192** | **Random**             |   **427,616.1 ns** |  **2,618.94 ns** |  **1,369.76 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| QuickSort3way      | 8192 | Random             |   425,946.7 ns |  3,319.57 ns |  1,473.91 ns |  1.00 |    0.00 |    1 |         - |          NA |
| QuickSortMedian3   | 8192 | Random             |   532,733.8 ns |  3,683.99 ns |  1,635.71 ns |  1.25 |    0.01 |    1 |         - |          NA |
| QuickSortMedian9   | 8192 | Random             |   506,063.8 ns |  8,726.77 ns |  4,564.27 ns |  1.18 |    0.01 |    1 |         - |          NA |
| DualPivotQuickSort | 8192 | Random             |   365,194.3 ns |  1,518.37 ns |    794.14 ns |  0.85 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 8192 | Random             | 1,159,329.6 ns |  1,680.34 ns |    746.08 ns |  2.71 |    0.01 |    2 |         - |          NA |
| IntroSort          | 8192 | Random             |   384,084.8 ns |  2,243.56 ns |    996.15 ns |  0.90 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 8192 | Random             |   350,909.2 ns |    985.07 ns |    515.21 ns |  0.82 |    0.00 |    1 |         - |          NA |
| PDQSort            | 8192 | Random             |   361,326.9 ns |  2,113.88 ns |  1,105.60 ns |  0.84 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | Random             |   465,298.4 ns |  2,751.34 ns |  1,221.61 ns |  1.09 |    0.00 |    1 |         - |          NA |
| Ipnsort            | 8192 | Random             |   480,871.0 ns |  2,652.64 ns |  1,387.38 ns |  1.12 |    0.00 |    1 |         - |          NA |
| StdSort            | 8192 | Random             |   403,366.2 ns |  1,085.46 ns |    567.72 ns |  0.94 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 8192 | Random             |   437,588.5 ns |    731.81 ns |    324.93 ns |  1.02 |    0.00 |    1 |         - |          NA |
| DotnetSort         | 8192 | Random             |   344,271.2 ns |    873.68 ns |    456.95 ns |  0.81 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **8192** | **SingleElementMoved** |    **75,393.2 ns** |  **2,075.99 ns** |  **1,085.78 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 8192 | SingleElementMoved |   749,344.6 ns |  2,457.66 ns |  1,091.22 ns |  9.94 |    0.14 |    8 |         - |          NA |
| QuickSortMedian3   | 8192 | SingleElementMoved |   571,450.5 ns |  4,448.07 ns |  1,974.97 ns |  7.58 |    0.11 |    7 |         - |          NA |
| QuickSortMedian9   | 8192 | SingleElementMoved |   212,070.2 ns |  4,001.66 ns |  2,092.95 ns |  2.81 |    0.05 |    5 |         - |          NA |
| DualPivotQuickSort | 8192 | SingleElementMoved |   156,241.0 ns |  1,100.73 ns |    575.70 ns |  2.07 |    0.03 |    4 |         - |          NA |
| StableQuickSort    | 8192 | SingleElementMoved |   433,947.2 ns |  3,778.46 ns |  1,976.21 ns |  5.76 |    0.08 |    6 |         - |          NA |
| IntroSort          | 8192 | SingleElementMoved |    41,797.1 ns |  1,590.45 ns |    831.84 ns |  0.55 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet    | 8192 | SingleElementMoved |    63,933.3 ns |    608.09 ns |    318.05 ns |  0.85 |    0.01 |    2 |         - |          NA |
| PDQSort            | 8192 | SingleElementMoved |    44,517.6 ns |    959.89 ns |    502.04 ns |  0.59 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | SingleElementMoved |    53,462.6 ns |    954.84 ns |    423.95 ns |  0.71 |    0.01 |    2 |         - |          NA |
| Ipnsort            | 8192 | SingleElementMoved |   226,806.4 ns |  1,105.11 ns |    490.68 ns |  3.01 |    0.04 |    5 |         - |          NA |
| StdSort            | 8192 | SingleElementMoved |    94,437.3 ns |  1,626.00 ns |    850.43 ns |  1.25 |    0.02 |    3 |         - |          NA |
| BlockQuickSort     | 8192 | SingleElementMoved |    93,539.7 ns |    525.36 ns |    233.26 ns |  1.24 |    0.02 |    3 |         - |          NA |
| DotnetSort         | 8192 | SingleElementMoved |    77,741.6 ns |  3,870.82 ns |  2,024.52 ns |  1.03 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **8192** | **Sorted**             |    **61,549.2 ns** |  **4,920.67 ns** |  **2,573.61 ns** |  **1.00** |    **0.06** |    **5** |         **-** |          **NA** |
| QuickSort3way      | 8192 | Sorted             | 1,023,290.7 ns |  7,515.29 ns |  3,930.64 ns | 16.65 |    0.67 |    9 |         - |          NA |
| QuickSortMedian3   | 8192 | Sorted             |   890,280.5 ns |  6,617.04 ns |  3,460.84 ns | 14.49 |    0.59 |    9 |         - |          NA |
| QuickSortMedian9   | 8192 | Sorted             |   207,445.6 ns |  3,054.49 ns |  1,597.56 ns |  3.38 |    0.14 |    7 |         - |          NA |
| DualPivotQuickSort | 8192 | Sorted             |   175,024.7 ns |  1,046.81 ns |    464.79 ns |  2.85 |    0.12 |    7 |         - |          NA |
| StableQuickSort    | 8192 | Sorted             |   431,216.4 ns |    711.78 ns |    316.04 ns |  7.02 |    0.28 |    8 |         - |          NA |
| IntroSort          | 8192 | Sorted             |     8,717.1 ns |    397.21 ns |    207.75 ns |  0.14 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet    | 8192 | Sorted             |    48,201.1 ns |    621.95 ns |    325.29 ns |  0.78 |    0.03 |    4 |         - |          NA |
| PDQSort            | 8192 | Sorted             |     8,161.4 ns |    361.61 ns |    160.56 ns |  0.13 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless  | 8192 | Sorted             |     8,043.8 ns |    483.55 ns |    214.70 ns |  0.13 |    0.01 |    2 |         - |          NA |
| Ipnsort            | 8192 | Sorted             |     3,922.7 ns |    106.77 ns |     47.40 ns |  0.06 |    0.00 |    1 |         - |          NA |
| StdSort            | 8192 | Sorted             |    20,720.6 ns |    330.98 ns |    118.03 ns |  0.34 |    0.01 |    3 |         - |          NA |
| BlockQuickSort     | 8192 | Sorted             |    80,602.1 ns |    575.87 ns |    301.19 ns |  1.31 |    0.05 |    6 |         - |          NA |
| DotnetSort         | 8192 | Sorted             |    49,067.0 ns |  1,988.48 ns |  1,040.01 ns |  0.80 |    0.04 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **8192** | **Reversed**           |    **49,737.5 ns** |  **4,907.76 ns** |  **2,566.85 ns** |  **1.00** |    **0.07** |    **5** |         **-** |          **NA** |
| QuickSort3way      | 8192 | Reversed           |   838,750.3 ns |  5,820.36 ns |  3,044.16 ns | 16.90 |    0.84 |    9 |         - |          NA |
| QuickSortMedian3   | 8192 | Reversed           | 1,121,959.1 ns |  2,421.34 ns |  1,266.41 ns | 22.61 |    1.12 |   10 |         - |          NA |
| QuickSortMedian9   | 8192 | Reversed           |   214,117.5 ns |  5,718.45 ns |  2,990.86 ns |  4.32 |    0.22 |    7 |         - |          NA |
| DualPivotQuickSort | 8192 | Reversed           |   179,045.0 ns |  1,345.72 ns |    703.84 ns |  3.61 |    0.18 |    7 |         - |          NA |
| StableQuickSort    | 8192 | Reversed           |   471,402.1 ns |  5,832.04 ns |  3,050.27 ns |  9.50 |    0.48 |    8 |         - |          NA |
| IntroSort          | 8192 | Reversed           |    34,853.9 ns |  1,025.13 ns |    455.16 ns |  0.70 |    0.04 |    4 |         - |          NA |
| IntroSortDotnet    | 8192 | Reversed           |    79,977.8 ns |    842.06 ns |    440.42 ns |  1.61 |    0.08 |    6 |         - |          NA |
| PDQSort            | 8192 | Reversed           |    14,665.3 ns |    536.65 ns |    238.28 ns |  0.30 |    0.02 |    2 |         - |          NA |
| PDQSortBranchless  | 8192 | Reversed           |    25,905.9 ns |    914.80 ns |    478.46 ns |  0.52 |    0.03 |    3 |         - |          NA |
| Ipnsort            | 8192 | Reversed           |     6,295.0 ns |    425.13 ns |    222.35 ns |  0.13 |    0.01 |    1 |         - |          NA |
| StdSort            | 8192 | Reversed           |    27,095.0 ns |  1,036.74 ns |    542.24 ns |  0.55 |    0.03 |    3 |         - |          NA |
| BlockQuickSort     | 8192 | Reversed           |    78,371.5 ns |    949.63 ns |    421.64 ns |  1.58 |    0.08 |    6 |         - |          NA |
| DotnetSort         | 8192 | Reversed           |    82,184.4 ns |  4,738.04 ns |  2,478.09 ns |  1.66 |    0.09 |    6 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **8192** | **PipeOrgan**          | **5,363,575.0 ns** |  **7,239.28 ns** |  **3,214.29 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way      | 8192 | PipeOrgan          |   509,620.2 ns |  1,868.32 ns |    977.17 ns |  0.10 |    0.00 |    2 |         - |          NA |
| QuickSortMedian3   | 8192 | PipeOrgan          |   509,555.6 ns | 37,233.05 ns | 19,473.61 ns |  0.10 |    0.00 |    2 |         - |          NA |
| QuickSortMedian9   | 8192 | PipeOrgan          |   276,275.3 ns |  3,023.83 ns |  1,581.52 ns |  0.05 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort | 8192 | PipeOrgan          |   147,810.4 ns |  1,461.68 ns |    764.49 ns |  0.03 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 8192 | PipeOrgan          |   467,331.5 ns |  2,264.72 ns |  1,184.49 ns |  0.09 |    0.00 |    2 |         - |          NA |
| IntroSort          | 8192 | PipeOrgan          |   329,872.5 ns |  1,610.27 ns |    842.20 ns |  0.06 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet    | 8192 | PipeOrgan          |   372,292.7 ns |  1,534.21 ns |    802.42 ns |  0.07 |    0.00 |    2 |         - |          NA |
| PDQSort            | 8192 | PipeOrgan          |   145,682.1 ns |  3,928.42 ns |  2,054.64 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | PipeOrgan          |   277,683.2 ns |  1,962.46 ns |  1,026.41 ns |  0.05 |    0.00 |    2 |         - |          NA |
| Ipnsort            | 8192 | PipeOrgan          |   257,558.7 ns |  1,572.87 ns |    822.64 ns |  0.05 |    0.00 |    2 |         - |          NA |
| StdSort            | 8192 | PipeOrgan          |   434,042.0 ns |  1,438.48 ns |    752.35 ns |  0.08 |    0.00 |    2 |         - |          NA |
| BlockQuickSort     | 8192 | PipeOrgan          |   268,620.9 ns |  1,256.65 ns |    657.25 ns |  0.05 |    0.00 |    2 |         - |          NA |
| DotnetSort         | 8192 | PipeOrgan          |   359,114.2 ns |  6,526.49 ns |  3,413.48 ns |  0.07 |    0.00 |    2 |         - |          NA |

### MergeBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                   | Size | Pattern            | Mean           | Error       | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------------- |----- |------------------- |---------------:|------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **MergeSort**                | **256**  | **Random**             |     **8,230.5 ns** |   **269.67 ns** |   **141.04 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Random             |     7,872.8 ns |    72.88 ns |    25.99 ns |  0.96 |    0.02 |    3 |         - |          NA |
| BottomupMergeSort        | 256  | Random             |     4,839.6 ns |   380.91 ns |   199.22 ns |  0.59 |    0.02 |    2 |         - |          NA |
| StdStableSort            | 256  | Random             |     3,025.9 ns |    52.97 ns |    23.52 ns |  0.37 |    0.01 |    1 |         - |          NA |
| RotateMergeSort          | 256  | Random             |     9,557.2 ns |   685.91 ns |   358.75 ns |  1.16 |    0.05 |    3 |         - |          NA |
| RotateMergeSortRecursive | 256  | Random             |    11,317.2 ns |   522.66 ns |   273.36 ns |  1.38 |    0.04 |    3 |         - |          NA |
| SymMergeSort             | 256  | Random             |     6,792.8 ns |    58.72 ns |    26.07 ns |  0.83 |    0.01 |    3 |         - |          NA |
| BlockMergeSort           | 256  | Random             |     6,479.5 ns |   219.77 ns |   114.94 ns |  0.79 |    0.02 |    3 |         - |          NA |
| NaturalMergeSort         | 256  | Random             |     5,462.9 ns |   537.96 ns |   281.36 ns |  0.66 |    0.03 |    2 |         - |          NA |
| TimSort                  | 256  | Random             |     3,967.0 ns |   467.31 ns |   244.41 ns |  0.48 |    0.03 |    2 |         - |          NA |
| PowerSort                | 256  | Random             |     3,873.8 ns | 1,330.85 ns |   696.06 ns |  0.47 |    0.08 |    2 |         - |          NA |
| ShiftSort                | 256  | Random             |     4,060.6 ns |   276.43 ns |   122.74 ns |  0.49 |    0.02 |    2 |         - |          NA |
| SpinSort                 | 256  | Random             |     2,077.3 ns |    13.20 ns |     4.71 ns |  0.25 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 256  | Random             |     2,440.1 ns |    56.64 ns |    25.15 ns |  0.30 |    0.01 |    1 |         - |          NA |
| Glidesort                | 256  | Random             |     4,730.7 ns |   732.89 ns |   383.32 ns |  0.57 |    0.04 |    2 |         - |          NA |
| Driftsort                | 256  | Random             |     4,402.3 ns |   452.69 ns |   236.77 ns |  0.54 |    0.03 |    2 |         - |          NA |
| FlatStableSort           | 256  | Random             |     2,643.2 ns |    34.41 ns |    12.27 ns |  0.32 |    0.01 |    1 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **MergeSort**                | **256**  | **SingleElementMoved** |     **4,290.4 ns** |    **17.08 ns** |     **6.09 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | SingleElementMoved |     5,430.5 ns |   394.51 ns |   206.34 ns |  1.27 |    0.05 |    7 |         - |          NA |
| BottomupMergeSort        | 256  | SingleElementMoved |     2,355.2 ns |    85.10 ns |    30.35 ns |  0.55 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 256  | SingleElementMoved |     1,950.6 ns |   260.80 ns |   136.40 ns |  0.45 |    0.03 |    4 |         - |          NA |
| RotateMergeSort          | 256  | SingleElementMoved |       584.5 ns |     2.48 ns |     1.10 ns |  0.14 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 256  | SingleElementMoved |       783.0 ns |   113.02 ns |    50.18 ns |  0.18 |    0.01 |    3 |         - |          NA |
| SymMergeSort             | 256  | SingleElementMoved |       514.0 ns |     3.31 ns |     1.47 ns |  0.12 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 256  | SingleElementMoved |     4,547.2 ns |   544.41 ns |   284.74 ns |  1.06 |    0.06 |    6 |         - |          NA |
| NaturalMergeSort         | 256  | SingleElementMoved |       598.4 ns |     4.64 ns |     1.65 ns |  0.14 |    0.00 |    2 |         - |          NA |
| TimSort                  | 256  | SingleElementMoved |       549.0 ns |   208.85 ns |   109.23 ns |  0.13 |    0.02 |    2 |         - |          NA |
| PowerSort                | 256  | SingleElementMoved |       404.2 ns |     1.06 ns |     0.47 ns |  0.09 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | SingleElementMoved |       399.7 ns |    20.93 ns |     9.29 ns |  0.09 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 256  | SingleElementMoved |       909.2 ns |    12.82 ns |     5.69 ns |  0.21 |    0.00 |    3 |         - |          NA |
| SpinSortVariant          | 256  | SingleElementMoved |     1,092.6 ns |   357.84 ns |   187.16 ns |  0.25 |    0.04 |    3 |         - |          NA |
| Glidesort                | 256  | SingleElementMoved |     1,193.3 ns |    14.31 ns |     6.35 ns |  0.28 |    0.00 |    3 |         - |          NA |
| Driftsort                | 256  | SingleElementMoved |     1,151.2 ns |    23.10 ns |    10.26 ns |  0.27 |    0.00 |    3 |         - |          NA |
| FlatStableSort           | 256  | SingleElementMoved |     1,257.6 ns |     9.10 ns |     4.76 ns |  0.29 |    0.00 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **MergeSort**                | **256**  | **Sorted**             |     **3,854.0 ns** |     **1.33 ns** |     **0.59 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Sorted             |     4,950.3 ns |   294.86 ns |   154.22 ns |  1.28 |    0.04 |    8 |         - |          NA |
| BottomupMergeSort        | 256  | Sorted             |     1,961.6 ns |     9.17 ns |     3.27 ns |  0.51 |    0.00 |    6 |         - |          NA |
| StdStableSort            | 256  | Sorted             |     1,699.2 ns |     6.76 ns |     2.41 ns |  0.44 |    0.00 |    6 |         - |          NA |
| RotateMergeSort          | 256  | Sorted             |       346.4 ns |     1.96 ns |     0.87 ns |  0.09 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 256  | Sorted             |       485.2 ns |    58.28 ns |    25.88 ns |  0.13 |    0.01 |    4 |         - |          NA |
| SymMergeSort             | 256  | Sorted             |       343.3 ns |     2.17 ns |     0.96 ns |  0.09 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 256  | Sorted             |     3,644.8 ns |   337.19 ns |   149.71 ns |  0.95 |    0.04 |    7 |         - |          NA |
| NaturalMergeSort         | 256  | Sorted             |       210.1 ns |     6.00 ns |     2.66 ns |  0.05 |    0.00 |    2 |         - |          NA |
| TimSort                  | 256  | Sorted             |       190.4 ns |     4.78 ns |     2.12 ns |  0.05 |    0.00 |    2 |         - |          NA |
| PowerSort                | 256  | Sorted             |       150.9 ns |     1.75 ns |     0.91 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Sorted             |       214.2 ns |     1.39 ns |     0.49 ns |  0.06 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 256  | Sorted             |       132.5 ns |     1.31 ns |     0.58 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 256  | Sorted             |       183.7 ns |     0.76 ns |     0.34 ns |  0.05 |    0.00 |    2 |         - |          NA |
| Glidesort                | 256  | Sorted             |       327.5 ns |    11.05 ns |     4.91 ns |  0.08 |    0.00 |    3 |         - |          NA |
| Driftsort                | 256  | Sorted             |       206.0 ns |     2.99 ns |     1.56 ns |  0.05 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 256  | Sorted             |     1,115.0 ns |     4.16 ns |     1.85 ns |  0.29 |    0.00 |    5 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **MergeSort**                | **256**  | **Reversed**           |     **8,938.9 ns** |   **544.76 ns** |   **284.92 ns** |  **1.00** |    **0.04** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Reversed           |     7,840.5 ns |   316.75 ns |   165.67 ns |  0.88 |    0.03 |    7 |         - |          NA |
| BottomupMergeSort        | 256  | Reversed           |     5,073.6 ns |    32.90 ns |    11.73 ns |  0.57 |    0.02 |    6 |         - |          NA |
| StdStableSort            | 256  | Reversed           |     2,300.0 ns |    52.07 ns |    18.57 ns |  0.26 |    0.01 |    4 |         - |          NA |
| RotateMergeSort          | 256  | Reversed           |     1,784.1 ns |    24.61 ns |    12.87 ns |  0.20 |    0.01 |    3 |         - |          NA |
| RotateMergeSortRecursive | 256  | Reversed           |     1,861.2 ns |     4.28 ns |     1.53 ns |  0.21 |    0.01 |    3 |         - |          NA |
| SymMergeSort             | 256  | Reversed           |     2,955.5 ns | 1,847.02 ns |   966.03 ns |  0.33 |    0.10 |    5 |         - |          NA |
| BlockMergeSort           | 256  | Reversed           |     9,366.1 ns |   431.96 ns |   225.93 ns |  1.05 |    0.04 |    7 |         - |          NA |
| NaturalMergeSort         | 256  | Reversed           |       329.2 ns |     1.92 ns |     0.85 ns |  0.04 |    0.00 |    1 |         - |          NA |
| TimSort                  | 256  | Reversed           |       262.3 ns |     2.86 ns |     1.02 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | Reversed           |       227.9 ns |     0.67 ns |     0.30 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Reversed           |       251.7 ns |     2.47 ns |     0.88 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 256  | Reversed           |       258.4 ns |     1.38 ns |     0.72 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 256  | Reversed           |       473.6 ns |   348.76 ns |   182.41 ns |  0.05 |    0.02 |    2 |         - |          NA |
| Glidesort                | 256  | Reversed           |       278.5 ns |     7.01 ns |     3.11 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 256  | Reversed           |       442.1 ns |   158.76 ns |    70.49 ns |  0.05 |    0.01 |    2 |         - |          NA |
| FlatStableSort           | 256  | Reversed           |     3,017.8 ns |   319.52 ns |   167.11 ns |  0.34 |    0.02 |    5 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **MergeSort**                | **256**  | **PipeOrgan**          |     **6,529.6 ns** |   **377.76 ns** |   **197.58 ns** |  **1.00** |    **0.04** |    **9** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | PipeOrgan          |     6,720.3 ns |    91.88 ns |    48.05 ns |  1.03 |    0.03 |    9 |         - |          NA |
| BottomupMergeSort        | 256  | PipeOrgan          |     4,104.0 ns | 1,402.11 ns |   733.33 ns |  0.63 |    0.11 |    7 |         - |          NA |
| StdStableSort            | 256  | PipeOrgan          |     2,098.7 ns |    15.26 ns |     6.77 ns |  0.32 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 256  | PipeOrgan          |     4,133.6 ns |    40.32 ns |    14.38 ns |  0.63 |    0.02 |    7 |         - |          NA |
| RotateMergeSortRecursive | 256  | PipeOrgan          |     5,138.2 ns |   482.66 ns |   252.44 ns |  0.79 |    0.04 |    8 |         - |          NA |
| SymMergeSort             | 256  | PipeOrgan          |     2,542.1 ns |    83.14 ns |    36.92 ns |  0.39 |    0.01 |    6 |         - |          NA |
| BlockMergeSort           | 256  | PipeOrgan          |     6,634.0 ns |    53.75 ns |    28.11 ns |  1.02 |    0.03 |    9 |         - |          NA |
| NaturalMergeSort         | 256  | PipeOrgan          |       688.8 ns |     9.99 ns |     5.23 ns |  0.11 |    0.00 |    2 |         - |          NA |
| TimSort                  | 256  | PipeOrgan          |       837.3 ns |     2.68 ns |     0.96 ns |  0.13 |    0.00 |    3 |         - |          NA |
| PowerSort                | 256  | PipeOrgan          |       499.8 ns |     5.31 ns |     2.36 ns |  0.08 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | PipeOrgan          |       547.6 ns |    29.32 ns |    10.45 ns |  0.08 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 256  | PipeOrgan          |     1,928.2 ns |   426.50 ns |   223.07 ns |  0.30 |    0.03 |    5 |         - |          NA |
| SpinSortVariant          | 256  | PipeOrgan          |     1,879.5 ns |    10.50 ns |     5.49 ns |  0.29 |    0.01 |    5 |         - |          NA |
| Glidesort                | 256  | PipeOrgan          |     1,214.9 ns |     9.76 ns |     4.33 ns |  0.19 |    0.01 |    4 |         - |          NA |
| Driftsort                | 256  | PipeOrgan          |       438.1 ns |     3.83 ns |     1.70 ns |  0.07 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | PipeOrgan          |     2,088.7 ns |    11.69 ns |     6.12 ns |  0.32 |    0.01 |    5 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **MergeSort**                | **1024** | **Random**             |    **38,033.2 ns** | **2,434.02 ns** | **1,273.04 ns** |  **1.00** |    **0.04** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Random             |    35,790.4 ns |   816.44 ns |   427.01 ns |  0.94 |    0.03 |    3 |         - |          NA |
| BottomupMergeSort        | 1024 | Random             |    22,261.3 ns |   318.38 ns |   141.36 ns |  0.59 |    0.02 |    2 |         - |          NA |
| StdStableSort            | 1024 | Random             |    15,640.1 ns |   349.23 ns |   155.06 ns |  0.41 |    0.01 |    1 |         - |          NA |
| RotateMergeSort          | 1024 | Random             |    69,251.2 ns | 6,219.30 ns | 3,252.82 ns |  1.82 |    0.10 |    4 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Random             |    66,724.8 ns |   636.57 ns |   282.64 ns |  1.76 |    0.06 |    4 |         - |          NA |
| SymMergeSort             | 1024 | Random             |    41,191.8 ns |   510.27 ns |   226.56 ns |  1.08 |    0.03 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | Random             |    32,637.1 ns |   571.09 ns |   298.69 ns |  0.86 |    0.03 |    3 |         - |          NA |
| NaturalMergeSort         | 1024 | Random             |    27,920.5 ns | 2,561.69 ns | 1,339.81 ns |  0.73 |    0.04 |    3 |         - |          NA |
| TimSort                  | 1024 | Random             |    20,000.3 ns | 1,379.85 ns |   612.66 ns |  0.53 |    0.02 |    2 |         - |          NA |
| PowerSort                | 1024 | Random             |    13,646.5 ns |   643.49 ns |   336.56 ns |  0.36 |    0.01 |    1 |         - |          NA |
| ShiftSort                | 1024 | Random             |    19,979.7 ns |   849.25 ns |   444.18 ns |  0.53 |    0.02 |    2 |         - |          NA |
| SpinSort                 | 1024 | Random             |    13,666.7 ns | 1,051.17 ns |   549.78 ns |  0.36 |    0.02 |    1 |         - |          NA |
| SpinSortVariant          | 1024 | Random             |    14,723.5 ns |   440.89 ns |   230.59 ns |  0.39 |    0.01 |    1 |         - |          NA |
| Glidesort                | 1024 | Random             |    19,919.5 ns |   293.82 ns |   130.46 ns |  0.52 |    0.02 |    2 |         - |          NA |
| Driftsort                | 1024 | Random             |    20,818.3 ns |   458.19 ns |   239.64 ns |  0.55 |    0.02 |    2 |         - |          NA |
| FlatStableSort           | 1024 | Random             |    14,819.4 ns |   308.21 ns |   136.85 ns |  0.39 |    0.01 |    1 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **MergeSort**                | **1024** | **SingleElementMoved** |    **17,171.5 ns** |   **184.40 ns** |    **81.87 ns** |  **1.00** |    **0.01** |    **9** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | SingleElementMoved |    21,143.6 ns |   218.96 ns |   114.52 ns |  1.23 |    0.01 |    9 |         - |          NA |
| BottomupMergeSort        | 1024 | SingleElementMoved |     7,246.7 ns |    14.74 ns |     6.54 ns |  0.42 |    0.00 |    8 |         - |          NA |
| StdStableSort            | 1024 | SingleElementMoved |     8,412.0 ns |   417.02 ns |   218.11 ns |  0.49 |    0.01 |    8 |         - |          NA |
| RotateMergeSort          | 1024 | SingleElementMoved |     1,943.8 ns |     7.06 ns |     3.14 ns |  0.11 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 1024 | SingleElementMoved |     2,462.3 ns |    90.87 ns |    32.41 ns |  0.14 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 1024 | SingleElementMoved |     1,702.8 ns |     3.91 ns |     2.04 ns |  0.10 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | SingleElementMoved |    19,871.9 ns |   345.94 ns |   180.93 ns |  1.16 |    0.01 |    9 |         - |          NA |
| NaturalMergeSort         | 1024 | SingleElementMoved |     2,023.5 ns |     4.35 ns |     1.55 ns |  0.12 |    0.00 |    3 |         - |          NA |
| TimSort                  | 1024 | SingleElementMoved |       820.4 ns |     2.99 ns |     1.33 ns |  0.05 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | SingleElementMoved |     1,624.9 ns |   440.40 ns |   230.34 ns |  0.09 |    0.01 |    3 |         - |          NA |
| ShiftSort                | 1024 | SingleElementMoved |     1,587.4 ns |   364.95 ns |   190.87 ns |  0.09 |    0.01 |    3 |         - |          NA |
| SpinSort                 | 1024 | SingleElementMoved |     4,364.7 ns |   459.45 ns |   240.30 ns |  0.25 |    0.01 |    6 |         - |          NA |
| SpinSortVariant          | 1024 | SingleElementMoved |     3,542.6 ns |   377.78 ns |   167.74 ns |  0.21 |    0.01 |    5 |         - |          NA |
| Glidesort                | 1024 | SingleElementMoved |     2,598.8 ns |     5.09 ns |     2.26 ns |  0.15 |    0.00 |    4 |         - |          NA |
| Driftsort                | 1024 | SingleElementMoved |     1,233.8 ns |     6.90 ns |     3.06 ns |  0.07 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 1024 | SingleElementMoved |     5,388.3 ns |    47.66 ns |    21.16 ns |  0.31 |    0.00 |    7 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **MergeSort**                | **1024** | **Sorted**             |    **15,696.8 ns** |   **150.22 ns** |    **78.57 ns** |  **1.00** |    **0.01** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Sorted             |    19,748.5 ns |   226.04 ns |   118.22 ns |  1.26 |    0.01 |    7 |         - |          NA |
| BottomupMergeSort        | 1024 | Sorted             |     5,778.7 ns |     9.65 ns |     3.44 ns |  0.37 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 1024 | Sorted             |     7,697.4 ns |   473.51 ns |   210.24 ns |  0.49 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 1024 | Sorted             |     1,343.2 ns |     6.87 ns |     2.45 ns |  0.09 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Sorted             |     1,858.0 ns |     6.59 ns |     2.93 ns |  0.12 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 1024 | Sorted             |     1,503.0 ns |   471.31 ns |   246.50 ns |  0.10 |    0.01 |    2 |         - |          NA |
| BlockMergeSort           | 1024 | Sorted             |    13,983.5 ns |   402.06 ns |   210.29 ns |  0.89 |    0.01 |    6 |         - |          NA |
| NaturalMergeSort         | 1024 | Sorted             |       588.7 ns |     6.13 ns |     3.21 ns |  0.04 |    0.00 |    1 |         - |          NA |
| TimSort                  | 1024 | Sorted             |       563.0 ns |     5.87 ns |     2.61 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Sorted             |       524.3 ns |     5.45 ns |     2.42 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Sorted             |       713.9 ns |     5.33 ns |     2.36 ns |  0.05 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 1024 | Sorted             |       459.0 ns |     2.88 ns |     1.28 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 1024 | Sorted             |       656.1 ns |     1.69 ns |     0.88 ns |  0.04 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Sorted             |       496.6 ns |    27.00 ns |     9.63 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | Sorted             |       584.9 ns |     7.04 ns |     3.68 ns |  0.04 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Sorted             |     5,045.3 ns |   377.23 ns |   197.30 ns |  0.32 |    0.01 |    4 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **MergeSort**                | **1024** | **Reversed**           |    **36,157.7 ns** |   **714.22 ns** |   **373.55 ns** |  **1.00** |    **0.01** |    **5** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Reversed           |    34,208.7 ns | 2,614.60 ns | 1,160.90 ns |  0.95 |    0.03 |    5 |         - |          NA |
| BottomupMergeSort        | 1024 | Reversed           |    20,402.7 ns |   405.53 ns |   212.10 ns |  0.56 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 1024 | Reversed           |    10,421.2 ns |   493.16 ns |   218.97 ns |  0.29 |    0.01 |    2 |         - |          NA |
| RotateMergeSort          | 1024 | Reversed           |     8,879.2 ns |   401.50 ns |   178.27 ns |  0.25 |    0.01 |    2 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Reversed           |     9,116.8 ns |   495.91 ns |   259.37 ns |  0.25 |    0.01 |    2 |         - |          NA |
| SymMergeSort             | 1024 | Reversed           |     8,570.9 ns |   292.41 ns |   152.94 ns |  0.24 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 1024 | Reversed           |    39,995.0 ns |   476.25 ns |   249.09 ns |  1.11 |    0.01 |    5 |         - |          NA |
| NaturalMergeSort         | 1024 | Reversed           |     1,044.6 ns |     5.18 ns |     2.30 ns |  0.03 |    0.00 |    1 |         - |          NA |
| TimSort                  | 1024 | Reversed           |       848.2 ns |     6.10 ns |     2.71 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Reversed           |       812.5 ns |     5.85 ns |     2.60 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Reversed           |       827.4 ns |     1.07 ns |     0.47 ns |  0.02 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 1024 | Reversed           |       957.4 ns |     4.99 ns |     1.78 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 1024 | Reversed           |     1,059.2 ns |     8.64 ns |     3.84 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Reversed           |       855.3 ns |     3.85 ns |     1.37 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | Reversed           |       857.1 ns |     2.36 ns |     1.05 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Reversed           |    12,502.6 ns |   408.55 ns |   213.68 ns |  0.35 |    0.01 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **MergeSort**                | **1024** | **PipeOrgan**          |    **26,500.6 ns** |   **620.12 ns** |   **324.33 ns** |  **1.00** |    **0.02** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | PipeOrgan          |    26,918.3 ns |   623.77 ns |   326.24 ns |  1.02 |    0.02 |    7 |         - |          NA |
| BottomupMergeSort        | 1024 | PipeOrgan          |    13,732.0 ns |   475.23 ns |   248.56 ns |  0.52 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 1024 | PipeOrgan          |     9,420.9 ns |   441.04 ns |   230.67 ns |  0.36 |    0.01 |    4 |         - |          NA |
| RotateMergeSort          | 1024 | PipeOrgan          |    18,020.5 ns |   221.86 ns |    98.51 ns |  0.68 |    0.01 |    6 |         - |          NA |
| RotateMergeSortRecursive | 1024 | PipeOrgan          |    21,428.9 ns |   541.37 ns |   283.15 ns |  0.81 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 1024 | PipeOrgan          |    11,716.9 ns | 1,002.91 ns |   445.30 ns |  0.44 |    0.02 |    5 |         - |          NA |
| BlockMergeSort           | 1024 | PipeOrgan          |    32,195.9 ns |   268.45 ns |   140.41 ns |  1.22 |    0.01 |    7 |         - |          NA |
| NaturalMergeSort         | 1024 | PipeOrgan          |     2,581.1 ns |   264.51 ns |    94.33 ns |  0.10 |    0.00 |    2 |         - |          NA |
| TimSort                  | 1024 | PipeOrgan          |     2,561.4 ns |    16.13 ns |     7.16 ns |  0.10 |    0.00 |    2 |         - |          NA |
| PowerSort                | 1024 | PipeOrgan          |     1,809.3 ns |   189.40 ns |    84.10 ns |  0.07 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | PipeOrgan          |     2,046.5 ns |   293.76 ns |   153.64 ns |  0.08 |    0.01 |    1 |         - |          NA |
| SpinSort                 | 1024 | PipeOrgan          |     8,024.2 ns |   311.01 ns |   162.66 ns |  0.30 |    0.01 |    4 |         - |          NA |
| SpinSortVariant          | 1024 | PipeOrgan          |     7,774.1 ns |   307.70 ns |   160.94 ns |  0.29 |    0.01 |    4 |         - |          NA |
| Glidesort                | 1024 | PipeOrgan          |     4,379.2 ns |    52.90 ns |    23.49 ns |  0.17 |    0.00 |    3 |         - |          NA |
| Driftsort                | 1024 | PipeOrgan          |     1,618.3 ns |   234.92 ns |   122.87 ns |  0.06 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | PipeOrgan          |     9,087.1 ns |   540.97 ns |   282.94 ns |  0.34 |    0.01 |    4 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **MergeSort**                | **8192** | **Random**             |   **679,569.7 ns** | **1,624.51 ns** |   **721.29 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Random             |   613,538.0 ns | 1,473.73 ns |   654.35 ns |  0.90 |    0.00 |    2 |         - |          NA |
| BottomupMergeSort        | 8192 | Random             |   500,148.7 ns | 2,680.09 ns | 1,401.74 ns |  0.74 |    0.00 |    2 |         - |          NA |
| StdStableSort            | 8192 | Random             |   472,038.0 ns |   970.60 ns |   430.95 ns |  0.69 |    0.00 |    2 |         - |          NA |
| RotateMergeSort          | 8192 | Random             | 1,357,176.8 ns | 6,131.48 ns | 3,206.88 ns |  2.00 |    0.00 |    4 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Random             | 1,466,748.0 ns | 3,666.75 ns | 1,628.06 ns |  2.16 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 8192 | Random             | 1,014,477.6 ns | 2,659.44 ns | 1,180.81 ns |  1.49 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 8192 | Random             |   690,911.6 ns | 2,646.39 ns | 1,175.01 ns |  1.02 |    0.00 |    2 |         - |          NA |
| NaturalMergeSort         | 8192 | Random             |   618,353.2 ns | 3,867.85 ns | 1,717.35 ns |  0.91 |    0.00 |    2 |         - |          NA |
| TimSort                  | 8192 | Random             |   566,504.2 ns | 2,087.54 ns | 1,091.82 ns |  0.83 |    0.00 |    2 |         - |          NA |
| PowerSort                | 8192 | Random             |   425,356.0 ns | 1,633.45 ns |   725.26 ns |  0.63 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 8192 | Random             |   560,737.3 ns | 1,931.62 ns | 1,010.28 ns |  0.83 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | Random             |   370,337.4 ns | 2,528.60 ns | 1,322.50 ns |  0.54 |    0.00 |    2 |         - |          NA |
| SpinSortVariant          | 8192 | Random             |   368,218.8 ns |   837.84 ns |   372.01 ns |  0.54 |    0.00 |    2 |         - |          NA |
| Glidesort                | 8192 | Random             |   205,951.6 ns | 1,980.72 ns | 1,035.96 ns |  0.30 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Random             |   217,387.6 ns | 1,299.21 ns |   576.86 ns |  0.32 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Random             |   396,791.2 ns | 1,830.84 ns |   957.56 ns |  0.58 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **MergeSort**                | **8192** | **SingleElementMoved** |   **136,044.3 ns** |   **951.10 ns** |   **497.44 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | SingleElementMoved |   167,322.3 ns | 2,010.47 ns | 1,051.51 ns |  1.23 |    0.01 |    7 |         - |          NA |
| BottomupMergeSort        | 8192 | SingleElementMoved |    56,239.7 ns |   864.78 ns |   308.39 ns |  0.41 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 8192 | SingleElementMoved |   109,607.4 ns |   915.57 ns |   406.52 ns |  0.81 |    0.00 |    6 |         - |          NA |
| RotateMergeSort          | 8192 | SingleElementMoved |    14,269.7 ns |   633.41 ns |   331.28 ns |  0.10 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 8192 | SingleElementMoved |    18,236.0 ns | 1,615.39 ns |   844.88 ns |  0.13 |    0.01 |    3 |         - |          NA |
| SymMergeSort             | 8192 | SingleElementMoved |    12,946.9 ns |   382.25 ns |   169.72 ns |  0.10 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 8192 | SingleElementMoved |   146,835.2 ns |   978.91 ns |   511.99 ns |  1.08 |    0.01 |    7 |         - |          NA |
| NaturalMergeSort         | 8192 | SingleElementMoved |    15,602.0 ns |   116.94 ns |    51.92 ns |  0.11 |    0.00 |    3 |         - |          NA |
| TimSort                  | 8192 | SingleElementMoved |     5,920.4 ns |   449.70 ns |   235.20 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | SingleElementMoved |    10,672.0 ns |   121.33 ns |    63.46 ns |  0.08 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 8192 | SingleElementMoved |    10,364.7 ns |   369.99 ns |   164.28 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | SingleElementMoved |    22,543.2 ns |   435.16 ns |   193.21 ns |  0.17 |    0.00 |    3 |         - |          NA |
| SpinSortVariant          | 8192 | SingleElementMoved |    19,957.6 ns |   826.14 ns |   366.81 ns |  0.15 |    0.00 |    3 |         - |          NA |
| Glidesort                | 8192 | SingleElementMoved |    20,153.6 ns |   220.08 ns |    78.48 ns |  0.15 |    0.00 |    3 |         - |          NA |
| Driftsort                | 8192 | SingleElementMoved |     9,449.0 ns |   905.23 ns |   401.93 ns |  0.07 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 8192 | SingleElementMoved |    46,763.8 ns | 1,447.56 ns |   642.73 ns |  0.34 |    0.00 |    4 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **MergeSort**                | **8192** | **Sorted**             |   **124,532.8 ns** |   **803.89 ns** |   **356.93 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Sorted             |   158,609.6 ns | 1,030.76 ns |   539.11 ns |  1.27 |    0.01 |    7 |         - |          NA |
| BottomupMergeSort        | 8192 | Sorted             |    44,672.6 ns |   814.27 ns |   361.54 ns |  0.36 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 8192 | Sorted             |   106,062.0 ns |   859.01 ns |   449.28 ns |  0.85 |    0.00 |    6 |         - |          NA |
| RotateMergeSort          | 8192 | Sorted             |    10,868.0 ns |   426.65 ns |   189.43 ns |  0.09 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Sorted             |    15,988.8 ns | 1,586.33 ns |   829.68 ns |  0.13 |    0.01 |    4 |         - |          NA |
| SymMergeSort             | 8192 | Sorted             |    10,564.4 ns |   330.99 ns |   173.12 ns |  0.08 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 8192 | Sorted             |   117,960.3 ns | 1,035.85 ns |   541.77 ns |  0.95 |    0.00 |    6 |         - |          NA |
| NaturalMergeSort         | 8192 | Sorted             |     4,379.3 ns |   666.29 ns |   348.48 ns |  0.04 |    0.00 |    1 |         - |          NA |
| TimSort                  | 8192 | Sorted             |     4,081.4 ns |     9.01 ns |     3.21 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Sorted             |     4,014.0 ns |    10.88 ns |     3.88 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Sorted             |     5,348.0 ns |   294.49 ns |   154.03 ns |  0.04 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | Sorted             |     3,671.5 ns |   496.10 ns |   220.27 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 8192 | Sorted             |     5,386.6 ns |   560.96 ns |   293.39 ns |  0.04 |    0.00 |    2 |         - |          NA |
| Glidesort                | 8192 | Sorted             |     3,470.3 ns |    78.63 ns |    34.91 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Sorted             |     4,124.1 ns |    23.29 ns |    10.34 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Sorted             |     3,774.1 ns |   417.98 ns |   185.59 ns |  0.03 |    0.00 |    1 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **MergeSort**                | **8192** | **Reversed**           |   **303,386.7 ns** | **2,139.84 ns** | **1,119.18 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Reversed           |   273,727.2 ns | 2,567.65 ns | 1,140.05 ns |  0.90 |    0.00 |    5 |         - |          NA |
| BottomupMergeSort        | 8192 | Reversed           |   170,793.6 ns | 1,111.13 ns |   581.14 ns |  0.56 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 8192 | Reversed           |   128,997.8 ns | 2,318.93 ns | 1,212.85 ns |  0.43 |    0.00 |    3 |         - |          NA |
| RotateMergeSort          | 8192 | Reversed           |    83,464.4 ns |   849.14 ns |   444.12 ns |  0.28 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Reversed           |    88,131.7 ns |   760.07 ns |   397.53 ns |  0.29 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 8192 | Reversed           |    74,738.0 ns |   932.09 ns |   487.50 ns |  0.25 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 8192 | Reversed           |   340,943.7 ns |   760.24 ns |   397.62 ns |  1.12 |    0.00 |    5 |         - |          NA |
| NaturalMergeSort         | 8192 | Reversed           |     8,084.6 ns |   633.93 ns |   331.56 ns |  0.03 |    0.00 |    1 |         - |          NA |
| TimSort                  | 8192 | Reversed           |     6,450.4 ns |   460.36 ns |   204.40 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Reversed           |     6,711.8 ns |   441.62 ns |   230.98 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Reversed           |     6,262.2 ns |   612.04 ns |   271.75 ns |  0.02 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 8192 | Reversed           |     7,811.8 ns |   405.19 ns |   211.92 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 8192 | Reversed           |     8,401.7 ns |   518.67 ns |   230.29 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Reversed           |     6,382.4 ns |   484.64 ns |   215.18 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Reversed           |     6,683.5 ns |   419.50 ns |   219.41 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Reversed           |     7,329.4 ns | 1,009.95 ns |   528.22 ns |  0.02 |    0.00 |    1 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **MergeSort**                | **8192** | **PipeOrgan**          |   **219,743.6 ns** | **2,517.59 ns** | **1,117.83 ns** |  **1.00** |    **0.01** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | PipeOrgan          |   223,983.4 ns | 1,418.18 ns |   741.73 ns |  1.02 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 8192 | PipeOrgan          |   112,937.7 ns | 1,828.98 ns |   956.59 ns |  0.51 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 8192 | PipeOrgan          |   122,150.0 ns | 2,900.12 ns | 1,516.82 ns |  0.56 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 8192 | PipeOrgan          |   159,248.4 ns |   339.17 ns |   150.59 ns |  0.72 |    0.00 |    6 |         - |          NA |
| RotateMergeSortRecursive | 8192 | PipeOrgan          |   186,407.1 ns | 2,048.33 ns | 1,071.31 ns |  0.85 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 8192 | PipeOrgan          |    98,899.8 ns |   857.40 ns |   380.69 ns |  0.45 |    0.00 |    5 |         - |          NA |
| BlockMergeSort           | 8192 | PipeOrgan          |   248,552.8 ns | 1,147.02 ns |   599.92 ns |  1.13 |    0.01 |    6 |         - |          NA |
| NaturalMergeSort         | 8192 | PipeOrgan          |    18,857.8 ns |   709.64 ns |   253.06 ns |  0.09 |    0.00 |    2 |         - |          NA |
| TimSort                  | 8192 | PipeOrgan          |    18,784.0 ns |   344.21 ns |   152.83 ns |  0.09 |    0.00 |    2 |         - |          NA |
| PowerSort                | 8192 | PipeOrgan          |    12,402.7 ns |   187.51 ns |    66.87 ns |  0.06 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | PipeOrgan          |    15,533.7 ns |   474.09 ns |   169.06 ns |  0.07 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | PipeOrgan          |    17,691.4 ns | 1,048.49 ns |   465.53 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSortVariant          | 8192 | PipeOrgan          |    19,724.8 ns | 1,126.80 ns |   589.34 ns |  0.09 |    0.00 |    2 |         - |          NA |
| Glidesort                | 8192 | PipeOrgan          |    34,644.3 ns |   707.24 ns |   314.02 ns |  0.16 |    0.00 |    3 |         - |          NA |
| Driftsort                | 8192 | PipeOrgan          |    10,719.4 ns |    82.50 ns |    36.63 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | PipeOrgan          |    79,504.1 ns | 1,484.02 ns |   776.17 ns |  0.36 |    0.00 |    4 |         - |          NA |

### NetworkBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                  | Size | Pattern            | Mean         | Error       | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------------ |----- |------------------- |-------------:|------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **BitonicSort**             | **256**  | **Random**             |  **11,267.9 ns** |   **312.15 ns** |   **163.26 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Random             |  22,545.9 ns |    89.74 ns |    39.84 ns |  2.00 |    0.03 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Random             |  16,616.1 ns |   139.44 ns |    72.93 ns |  1.47 |    0.02 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **SingleElementMoved** |  **10,149.5 ns** |   **478.67 ns** |   **212.53 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | SingleElementMoved |  23,245.2 ns |   405.72 ns |   212.20 ns |  2.29 |    0.05 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | SingleElementMoved |  16,684.7 ns |    96.15 ns |    50.29 ns |  1.64 |    0.03 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **Sorted**             |  **10,255.8 ns** |   **523.89 ns** |   **274.00 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Sorted             |  22,932.6 ns |   119.57 ns |    62.54 ns |  2.24 |    0.06 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Sorted             |  16,760.9 ns |   103.75 ns |    46.07 ns |  1.64 |    0.04 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **Reversed**           |  **10,024.1 ns** |   **366.18 ns** |   **191.52 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Reversed           |  22,721.6 ns |    70.44 ns |    36.84 ns |  2.27 |    0.04 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Reversed           |  16,749.9 ns |   158.03 ns |    56.36 ns |  1.67 |    0.03 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **PipeOrgan**          |   **9,175.7 ns** |   **534.59 ns** |   **237.36 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | PipeOrgan          |  22,611.6 ns |   208.33 ns |   108.96 ns |  2.47 |    0.06 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | PipeOrgan          |  16,770.9 ns |   142.80 ns |    74.69 ns |  1.83 |    0.04 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Random**             |  **94,861.6 ns** | **1,421.34 ns** |   **743.39 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Random             | 123,948.0 ns | 1,062.70 ns |   471.84 ns |  1.31 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Random             | 102,474.1 ns |   494.17 ns |   258.46 ns |  1.08 |    0.01 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **SingleElementMoved** |  **58,963.1 ns** | **1,404.72 ns** |   **734.69 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | SingleElementMoved | 118,555.4 ns |   506.41 ns |   264.86 ns |  2.01 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | SingleElementMoved | 102,623.3 ns |   216.32 ns |    96.05 ns |  1.74 |    0.02 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Sorted**             |  **58,393.9 ns** | **1,156.99 ns** |   **605.13 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Sorted             | 118,642.1 ns |   700.79 ns |   366.53 ns |  2.03 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Sorted             | 102,769.8 ns |   432.00 ns |   225.95 ns |  1.76 |    0.02 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Reversed**           |  **57,518.7 ns** | **1,141.20 ns** |   **596.87 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Reversed           | 118,598.5 ns |   892.51 ns |   466.80 ns |  2.06 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Reversed           | 102,531.8 ns |   243.07 ns |   127.13 ns |  1.78 |    0.02 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **PipeOrgan**          |  **55,367.2 ns** | **4,235.68 ns** | **2,215.34 ns** |  **1.00** |    **0.05** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | PipeOrgan          | 116,270.2 ns |   162.79 ns |    85.14 ns |  2.10 |    0.08 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | PipeOrgan          | 102,604.6 ns |   253.03 ns |   132.34 ns |  1.86 |    0.07 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Random**             | **544,091.9 ns** | **3,170.41 ns** | **1,658.19 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Random             | 818,420.1 ns | 1,779.30 ns |   790.02 ns |  1.50 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Random             | 638,926.4 ns | 4,118.31 ns | 1,828.56 ns |  1.17 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **SingleElementMoved** | **322,336.1 ns** | **3,522.47 ns** | **1,564.00 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | SingleElementMoved | 592,354.4 ns | 3,583.68 ns | 1,591.18 ns |  1.84 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | SingleElementMoved | 585,239.5 ns |   602.91 ns |   315.33 ns |  1.82 |    0.01 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Sorted**             | **321,618.1 ns** | **3,926.16 ns** | **2,053.46 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Sorted             | 591,611.7 ns | 1,152.38 ns |   511.66 ns |  1.84 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Sorted             | 585,361.8 ns |   618.82 ns |   323.66 ns |  1.82 |    0.01 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Reversed**           | **316,396.7 ns** | **5,078.27 ns** | **2,656.03 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Reversed           | 591,185.0 ns | 3,564.14 ns | 1,864.11 ns |  1.87 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Reversed           | 585,050.6 ns |   701.76 ns |   311.58 ns |  1.85 |    0.01 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **PipeOrgan**          | **298,273.6 ns** | **9,294.62 ns** | **4,126.87 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | PipeOrgan          | 578,939.6 ns | 1,791.36 ns |   795.37 ns |  1.94 |    0.03 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | PipeOrgan          | 585,114.2 ns |   207.61 ns |   108.58 ns |  1.96 |    0.03 |    2 |         - |          NA |

### PartitionBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                       | Size | Pattern            | Mean           | Error         | StdDev       | Median         | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------------- |----- |------------------- |---------------:|--------------:|-------------:|---------------:|------:|--------:|-----:|----------:|------------:|
| **QuickSort**                    | **256**  | **Random**             |     **2,745.0 ns** |     **398.69 ns** |    **208.52 ns** |     **2,632.8 ns** |  **1.00** |    **0.10** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 256  | Random             |     3,110.7 ns |      45.69 ns |     16.29 ns |     3,115.8 ns |  1.14 |    0.08 |    3 |         - |          NA |
| QuickSortMedian3             | 256  | Random             |     4,492.9 ns |     367.92 ns |    192.43 ns |     4,427.8 ns |  1.64 |    0.13 |    3 |         - |          NA |
| QuickSortMedian9             | 256  | Random             |     3,726.2 ns |      98.89 ns |     43.91 ns |     3,707.5 ns |  1.36 |    0.09 |    3 |         - |          NA |
| DualPivotQuickSort           | 256  | Random             |     2,250.5 ns |      73.08 ns |     32.45 ns |     2,231.7 ns |  0.82 |    0.06 |    2 |         - |          NA |
| StableQuickSort              | 256  | Random             |    11,429.0 ns |     495.57 ns |    220.04 ns |    11,458.3 ns |  4.18 |    0.30 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Random             |     7,448.5 ns |     274.61 ns |    121.93 ns |     7,386.5 ns |  2.73 |    0.19 |    4 |         - |          NA |
| DestswapStableQuickSort      | 256  | Random             |     8,711.7 ns |     277.98 ns |    145.39 ns |     8,738.9 ns |  3.19 |    0.22 |    4 |         - |          NA |
| IntroSort                    | 256  | Random             |     2,466.3 ns |   1,243.97 ns |    650.62 ns |     2,261.9 ns |  0.90 |    0.23 |    2 |         - |          NA |
| IntroSortDotnet              | 256  | Random             |     1,625.7 ns |      56.10 ns |     24.91 ns |     1,616.0 ns |  0.60 |    0.04 |    1 |         - |          NA |
| PDQSort                      | 256  | Random             |     1,680.7 ns |      51.37 ns |     18.32 ns |     1,680.3 ns |  0.62 |    0.04 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | Random             |     2,902.3 ns |      49.40 ns |     21.93 ns |     2,890.9 ns |  1.06 |    0.07 |    3 |         - |          NA |
| Ipnsort                      | 256  | Random             |     3,386.0 ns |      22.51 ns |      9.99 ns |     3,383.0 ns |  1.24 |    0.08 |    3 |         - |          NA |
| StdSort                      | 256  | Random             |     1,875.5 ns |     488.19 ns |    255.33 ns |     1,722.2 ns |  0.69 |    0.10 |    1 |         - |          NA |
| BlockQuickSort               | 256  | Random             |     2,341.9 ns |      79.40 ns |     41.53 ns |     2,322.0 ns |  0.86 |    0.06 |    2 |         - |          NA |
| DotnetSort                   | 256  | Random             |     1,842.0 ns |      58.77 ns |     20.96 ns |     1,840.4 ns |  0.67 |    0.05 |    1 |         - |          NA |
|      |                    |                |               |              |                |       |         |      |           |             |
| **QuickSort**                    | **256**  | **SingleElementMoved** |     **1,225.1 ns** |      **28.07 ns** |     **12.46 ns** |     **1,228.0 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 256  | SingleElementMoved |     5,436.0 ns |     848.03 ns |    443.54 ns |     5,143.8 ns |  4.44 |    0.34 |    4 |         - |          NA |
| QuickSortMedian3             | 256  | SingleElementMoved |     5,225.9 ns |     491.21 ns |    218.10 ns |     5,088.5 ns |  4.27 |    0.17 |    4 |         - |          NA |
| QuickSortMedian9             | 256  | SingleElementMoved |     4,208.6 ns |      76.54 ns |     27.30 ns |     4,210.3 ns |  3.44 |    0.04 |    3 |         - |          NA |
| DualPivotQuickSort           | 256  | SingleElementMoved |     3,858.0 ns |     350.89 ns |    155.80 ns |     3,924.3 ns |  3.15 |    0.12 |    3 |         - |          NA |
| StableQuickSort              | 256  | SingleElementMoved |     8,856.1 ns |     256.11 ns |    133.95 ns |     8,920.6 ns |  7.23 |    0.12 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 256  | SingleElementMoved |     5,420.3 ns |      35.95 ns |     15.96 ns |     5,427.0 ns |  4.42 |    0.04 |    4 |         - |          NA |
| DestswapStableQuickSort      | 256  | SingleElementMoved |    10,499.0 ns |     593.09 ns |    310.20 ns |    10,528.6 ns |  8.57 |    0.25 |    5 |         - |          NA |
| IntroSort                    | 256  | SingleElementMoved |       870.8 ns |      33.89 ns |     15.05 ns |       875.1 ns |  0.71 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | SingleElementMoved |     1,173.9 ns |     239.20 ns |    125.11 ns |     1,101.4 ns |  0.96 |    0.10 |    2 |         - |          NA |
| PDQSort                      | 256  | SingleElementMoved |     1,094.9 ns |      23.10 ns |     12.08 ns |     1,093.2 ns |  0.89 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 256  | SingleElementMoved |     1,458.7 ns |       8.85 ns |      3.93 ns |     1,459.2 ns |  1.19 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 256  | SingleElementMoved |     3,299.5 ns |      26.83 ns |     11.91 ns |     3,300.5 ns |  2.69 |    0.03 |    3 |         - |          NA |
| StdSort                      | 256  | SingleElementMoved |     1,535.8 ns |     103.76 ns |     46.07 ns |     1,530.5 ns |  1.25 |    0.04 |    2 |         - |          NA |
| BlockQuickSort               | 256  | SingleElementMoved |     1,542.0 ns |     312.43 ns |    163.41 ns |     1,431.4 ns |  1.26 |    0.13 |    2 |         - |          NA |
| DotnetSort                   | 256  | SingleElementMoved |     1,142.9 ns |     277.05 ns |    144.90 ns |     1,203.6 ns |  0.93 |    0.11 |    2 |         - |          NA |
|      |                    |                |               |              |                |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Sorted**             |       **892.6 ns** |      **62.08 ns** |     **32.47 ns** |       **873.4 ns** |  **1.00** |    **0.05** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 256  | Sorted             |     6,796.4 ns |      58.58 ns |     26.01 ns |     6,802.9 ns |  7.62 |    0.25 |    8 |         - |          NA |
| QuickSortMedian3             | 256  | Sorted             |     6,276.5 ns |      66.89 ns |     23.85 ns |     6,267.4 ns |  7.04 |    0.24 |    8 |         - |          NA |
| QuickSortMedian9             | 256  | Sorted             |     4,615.7 ns |      97.20 ns |     34.66 ns |     4,634.2 ns |  5.18 |    0.18 |    7 |         - |          NA |
| DualPivotQuickSort           | 256  | Sorted             |     4,047.9 ns |      37.93 ns |     13.53 ns |     4,049.6 ns |  4.54 |    0.15 |    7 |         - |          NA |
| StableQuickSort              | 256  | Sorted             |     8,808.1 ns |     295.45 ns |    154.52 ns |     8,905.7 ns |  9.88 |    0.37 |    9 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Sorted             |     5,077.3 ns |      49.06 ns |     21.78 ns |     5,073.8 ns |  5.69 |    0.19 |    7 |         - |          NA |
| DestswapStableQuickSort      | 256  | Sorted             |    10,090.7 ns |     337.25 ns |    176.39 ns |    10,199.5 ns | 11.32 |    0.42 |    9 |         - |          NA |
| IntroSort                    | 256  | Sorted             |       298.6 ns |       2.72 ns |      0.97 ns |       298.7 ns |  0.33 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 256  | Sorted             |     1,028.9 ns |      16.21 ns |      7.20 ns |     1,030.5 ns |  1.15 |    0.04 |    5 |         - |          NA |
| PDQSort                      | 256  | Sorted             |       373.0 ns |       2.85 ns |      1.49 ns |       372.1 ns |  0.42 |    0.01 |    3 |         - |          NA |
| PDQSortBranchless            | 256  | Sorted             |       391.7 ns |      42.98 ns |     22.48 ns |       376.7 ns |  0.44 |    0.03 |    3 |         - |          NA |
| Ipnsort                      | 256  | Sorted             |       146.2 ns |       2.53 ns |      1.32 ns |       145.6 ns |  0.16 |    0.01 |    1 |         - |          NA |
| StdSort                      | 256  | Sorted             |       537.2 ns |     139.37 ns |     61.88 ns |       539.5 ns |  0.60 |    0.07 |    4 |         - |          NA |
| BlockQuickSort               | 256  | Sorted             |     1,392.5 ns |     311.31 ns |    162.82 ns |     1,417.5 ns |  1.56 |    0.18 |    6 |         - |          NA |
| DotnetSort                   | 256  | Sorted             |       835.3 ns |       8.71 ns |      3.87 ns |       835.5 ns |  0.94 |    0.03 |    5 |         - |          NA |
|      |                    |                |               |              |                |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Reversed**           |       **976.8 ns** |      **12.38 ns** |      **5.49 ns** |       **976.6 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 256  | Reversed           |     5,469.9 ns |     486.09 ns |    254.23 ns |     5,404.8 ns |  5.60 |    0.25 |    6 |         - |          NA |
| QuickSortMedian3             | 256  | Reversed           |     7,130.4 ns |      31.14 ns |     16.29 ns |     7,135.8 ns |  7.30 |    0.04 |    7 |         - |          NA |
| QuickSortMedian9             | 256  | Reversed           |     4,969.4 ns |     483.60 ns |    252.93 ns |     4,813.7 ns |  5.09 |    0.25 |    6 |         - |          NA |
| DualPivotQuickSort           | 256  | Reversed           |     3,893.6 ns |     314.16 ns |    164.31 ns |     3,794.9 ns |  3.99 |    0.16 |    5 |         - |          NA |
| StableQuickSort              | 256  | Reversed           |     8,691.3 ns |     397.36 ns |    207.83 ns |     8,736.3 ns |  8.90 |    0.21 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Reversed           |     5,218.2 ns |      29.30 ns |     10.45 ns |     5,215.3 ns |  5.34 |    0.03 |    6 |         - |          NA |
| DestswapStableQuickSort      | 256  | Reversed           |    10,207.7 ns |     438.89 ns |    229.55 ns |    10,241.9 ns | 10.45 |    0.23 |    8 |         - |          NA |
| IntroSort                    | 256  | Reversed           |       581.6 ns |      56.78 ns |     29.70 ns |       584.0 ns |  0.60 |    0.03 |    2 |         - |          NA |
| IntroSortDotnet              | 256  | Reversed           |     1,669.7 ns |     359.81 ns |    188.19 ns |     1,745.0 ns |  1.71 |    0.18 |    4 |         - |          NA |
| PDQSort                      | 256  | Reversed           |       547.3 ns |       3.71 ns |      1.65 ns |       547.1 ns |  0.56 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 256  | Reversed           |     1,026.4 ns |     341.57 ns |    178.65 ns |       908.8 ns |  1.05 |    0.17 |    3 |         - |          NA |
| Ipnsort                      | 256  | Reversed           |       222.7 ns |       0.99 ns |      0.44 ns |       222.6 ns |  0.23 |    0.00 |    1 |         - |          NA |
| StdSort                      | 256  | Reversed           |       651.6 ns |       6.35 ns |      3.32 ns |       652.1 ns |  0.67 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 256  | Reversed           |     1,445.0 ns |      11.69 ns |      4.17 ns |     1,444.2 ns |  1.48 |    0.01 |    4 |         - |          NA |
| DotnetSort                   | 256  | Reversed           |     1,464.3 ns |     144.19 ns |     51.42 ns |     1,482.7 ns |  1.50 |    0.05 |    4 |         - |          NA |
|      |                    |                |               |              |                |       |         |      |           |             |
| **QuickSort**                    | **256**  | **PipeOrgan**          |     **7,726.2 ns** |     **281.49 ns** |    **147.23 ns** |     **7,761.8 ns** |  **1.00** |    **0.03** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 256  | PipeOrgan          |     4,792.0 ns |     111.52 ns |     39.77 ns |     4,796.5 ns |  0.62 |    0.01 |    3 |         - |          NA |
| QuickSortMedian3             | 256  | PipeOrgan          |     6,761.9 ns |     172.55 ns |     76.61 ns |     6,726.5 ns |  0.88 |    0.02 |    4 |         - |          NA |
| QuickSortMedian9             | 256  | PipeOrgan          |     4,275.6 ns |     548.23 ns |    286.73 ns |     4,129.1 ns |  0.55 |    0.04 |    3 |         - |          NA |
| DualPivotQuickSort           | 256  | PipeOrgan          |     1,799.7 ns |     371.11 ns |    164.78 ns |     1,724.9 ns |  0.23 |    0.02 |    1 |         - |          NA |
| StableQuickSort              | 256  | PipeOrgan          |     9,308.8 ns |     398.33 ns |    208.33 ns |     9,278.6 ns |  1.21 |    0.03 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 256  | PipeOrgan          |     5,224.2 ns |     462.89 ns |    242.10 ns |     5,168.8 ns |  0.68 |    0.03 |    3 |         - |          NA |
| DestswapStableQuickSort      | 256  | PipeOrgan          |    10,818.6 ns |     515.43 ns |    269.58 ns |    10,959.5 ns |  1.40 |    0.04 |    5 |         - |          NA |
| IntroSort                    | 256  | PipeOrgan          |     1,670.2 ns |      75.31 ns |     26.86 ns |     1,669.7 ns |  0.22 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | PipeOrgan          |     2,199.5 ns |      53.55 ns |     19.10 ns |     2,198.2 ns |  0.28 |    0.01 |    2 |         - |          NA |
| PDQSort                      | 256  | PipeOrgan          |     1,689.5 ns |      39.65 ns |     17.61 ns |     1,678.7 ns |  0.22 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | PipeOrgan          |     2,965.0 ns |      64.13 ns |     22.87 ns |     2,960.4 ns |  0.38 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 256  | PipeOrgan          |     3,688.1 ns |     319.21 ns |    166.95 ns |     3,577.5 ns |  0.48 |    0.02 |    3 |         - |          NA |
| StdSort                      | 256  | PipeOrgan          |     2,336.4 ns |     468.69 ns |    245.13 ns |     2,224.5 ns |  0.30 |    0.03 |    2 |         - |          NA |
| BlockQuickSort               | 256  | PipeOrgan          |     4,416.0 ns |     355.59 ns |    185.98 ns |     4,408.3 ns |  0.57 |    0.02 |    3 |         - |          NA |
| DotnetSort                   | 256  | PipeOrgan          |     2,547.4 ns |      51.77 ns |     22.98 ns |     2,542.4 ns |  0.33 |    0.01 |    2 |         - |          NA |
|      |                    |                |               |              |                |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Random**             |    **13,336.2 ns** |     **269.11 ns** |    **119.49 ns** |    **13,386.9 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Random             |    19,186.7 ns |   1,042.50 ns |    462.88 ns |    19,221.6 ns |  1.44 |    0.03 |    2 |         - |          NA |
| QuickSortMedian3             | 1024 | Random             |    23,488.1 ns |     830.69 ns |    434.47 ns |    23,437.6 ns |  1.76 |    0.03 |    2 |         - |          NA |
| QuickSortMedian9             | 1024 | Random             |    22,036.0 ns |   6,099.42 ns |  3,190.11 ns |    20,463.7 ns |  1.65 |    0.23 |    2 |         - |          NA |
| DualPivotQuickSort           | 1024 | Random             |    10,367.5 ns |     571.79 ns |    253.88 ns |    10,393.4 ns |  0.78 |    0.02 |    1 |         - |          NA |
| StableQuickSort              | 1024 | Random             |    84,121.2 ns |     528.93 ns |    234.85 ns |    84,148.6 ns |  6.31 |    0.06 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Random             |    37,784.5 ns |     384.45 ns |    170.70 ns |    37,753.2 ns |  2.83 |    0.03 |    3 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Random             |    39,771.3 ns |     426.21 ns |    189.24 ns |    39,853.0 ns |  2.98 |    0.03 |    3 |         - |          NA |
| IntroSort                    | 1024 | Random             |    11,355.4 ns |     725.57 ns |    379.49 ns |    11,308.5 ns |  0.85 |    0.03 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | Random             |     9,058.1 ns |     499.84 ns |    261.43 ns |     9,079.8 ns |  0.68 |    0.02 |    1 |         - |          NA |
| PDQSort                      | 1024 | Random             |     9,089.7 ns |     609.76 ns |    318.92 ns |     9,102.5 ns |  0.68 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | Random             |    14,033.0 ns |     246.16 ns |    128.75 ns |    14,079.5 ns |  1.05 |    0.01 |    1 |         - |          NA |
| Ipnsort                      | 1024 | Random             |    17,920.1 ns |     237.25 ns |    105.34 ns |    17,969.0 ns |  1.34 |    0.01 |    2 |         - |          NA |
| StdSort                      | 1024 | Random             |     8,995.3 ns |     376.31 ns |    196.82 ns |     8,976.5 ns |  0.67 |    0.02 |    1 |         - |          NA |
| BlockQuickSort               | 1024 | Random             |    12,880.0 ns |     426.19 ns |    189.23 ns |    12,866.8 ns |  0.97 |    0.02 |    1 |         - |          NA |
| DotnetSort                   | 1024 | Random             |    10,348.2 ns |     224.09 ns |     99.50 ns |    10,338.2 ns |  0.78 |    0.01 |    1 |         - |          NA |
|      |                    |                |               |              |                |       |         |      |           |             |
| **QuickSort**                    | **1024** | **SingleElementMoved** |     **5,716.5 ns** |      **94.35 ns** |     **41.89 ns** |     **5,715.9 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 1024 | SingleElementMoved |    39,682.1 ns |     782.52 ns |    409.27 ns |    39,763.6 ns |  6.94 |    0.08 |    6 |         - |          NA |
| QuickSortMedian3             | 1024 | SingleElementMoved |    31,611.2 ns |     494.04 ns |    219.36 ns |    31,575.1 ns |  5.53 |    0.05 |    5 |         - |          NA |
| QuickSortMedian9             | 1024 | SingleElementMoved |    31,152.9 ns |  22,736.01 ns | 11,891.37 ns |    23,740.2 ns |  5.45 |    1.96 |    4 |         - |          NA |
| DualPivotQuickSort           | 1024 | SingleElementMoved |    21,921.0 ns |   1,077.77 ns |    563.70 ns |    21,792.1 ns |  3.83 |    0.10 |    3 |         - |          NA |
| StableQuickSort              | 1024 | SingleElementMoved |    42,597.5 ns |     245.95 ns |    128.64 ns |    42,593.1 ns |  7.45 |    0.06 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | SingleElementMoved |    27,371.8 ns |   1,778.87 ns |    930.38 ns |    27,096.9 ns |  4.79 |    0.16 |    4 |         - |          NA |
| DestswapStableQuickSort      | 1024 | SingleElementMoved |    42,628.9 ns |     252.33 ns |    131.97 ns |    42,641.8 ns |  7.46 |    0.06 |    6 |         - |          NA |
| IntroSort                    | 1024 | SingleElementMoved |     4,423.4 ns |     546.75 ns |    285.96 ns |     4,411.6 ns |  0.77 |    0.05 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | SingleElementMoved |     6,395.5 ns |      43.16 ns |     19.16 ns |     6,394.8 ns |  1.12 |    0.01 |    1 |         - |          NA |
| PDQSort                      | 1024 | SingleElementMoved |     4,879.5 ns |      98.40 ns |     35.09 ns |     4,886.3 ns |  0.85 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | SingleElementMoved |     6,433.2 ns |     578.64 ns |    302.64 ns |     6,339.5 ns |  1.13 |    0.05 |    1 |         - |          NA |
| Ipnsort                      | 1024 | SingleElementMoved |    16,418.0 ns |      42.30 ns |     18.78 ns |    16,418.0 ns |  2.87 |    0.02 |    2 |         - |          NA |
| StdSort                      | 1024 | SingleElementMoved |     7,081.6 ns |     256.90 ns |    134.36 ns |     7,075.2 ns |  1.24 |    0.02 |    1 |         - |          NA |
| BlockQuickSort               | 1024 | SingleElementMoved |     7,671.3 ns |     221.93 ns |     98.54 ns |     7,640.1 ns |  1.34 |    0.02 |    1 |         - |          NA |
| DotnetSort                   | 1024 | SingleElementMoved |     5,808.1 ns |   1,145.53 ns |    599.14 ns |     5,660.7 ns |  1.02 |    0.10 |    1 |         - |          NA |
|      |                    |                |               |              |                |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Sorted**             |     **4,088.7 ns** |      **58.03 ns** |     **20.69 ns** |     **4,084.9 ns** |  **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Sorted             |    52,698.2 ns |     665.80 ns |    295.62 ns |    52,583.1 ns | 12.89 |    0.09 |    8 |         - |          NA |
| QuickSortMedian3             | 1024 | Sorted             |    43,151.4 ns |     168.14 ns |     87.94 ns |    43,150.8 ns | 10.55 |    0.05 |    7 |         - |          NA |
| QuickSortMedian9             | 1024 | Sorted             |    22,780.5 ns |     203.37 ns |     90.30 ns |    22,794.0 ns |  5.57 |    0.03 |    6 |         - |          NA |
| DualPivotQuickSort           | 1024 | Sorted             |    21,669.4 ns |     301.41 ns |    133.83 ns |    21,626.6 ns |  5.30 |    0.04 |    6 |         - |          NA |
| StableQuickSort              | 1024 | Sorted             |    42,412.2 ns |     382.95 ns |    170.03 ns |    42,373.4 ns | 10.37 |    0.06 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Sorted             |    25,753.7 ns |   1,392.28 ns |    728.19 ns |    25,636.1 ns |  6.30 |    0.17 |    6 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Sorted             |    43,088.8 ns |     398.59 ns |    176.98 ns |    43,065.4 ns | 10.54 |    0.06 |    7 |         - |          NA |
| IntroSort                    | 1024 | Sorted             |     1,109.3 ns |       9.98 ns |      4.43 ns |     1,107.8 ns |  0.27 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet              | 1024 | Sorted             |     4,917.4 ns |     549.52 ns |    243.99 ns |     4,810.1 ns |  1.20 |    0.06 |    4 |         - |          NA |
| PDQSort                      | 1024 | Sorted             |     1,326.1 ns |       2.48 ns |      1.30 ns |     1,326.3 ns |  0.32 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | Sorted             |     1,331.1 ns |      15.78 ns |      7.01 ns |     1,331.5 ns |  0.33 |    0.00 |    2 |         - |          NA |
| Ipnsort                      | 1024 | Sorted             |       515.8 ns |       2.60 ns |      1.36 ns |       516.3 ns |  0.13 |    0.00 |    1 |         - |          NA |
| StdSort                      | 1024 | Sorted             |     1,804.5 ns |       7.71 ns |      2.75 ns |     1,803.5 ns |  0.44 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 1024 | Sorted             |     6,265.8 ns |      45.86 ns |     20.36 ns |     6,261.5 ns |  1.53 |    0.01 |    5 |         - |          NA |
| DotnetSort                   | 1024 | Sorted             |     4,037.2 ns |      92.91 ns |     41.25 ns |     4,030.6 ns |  0.99 |    0.01 |    4 |         - |          NA |
|      |                    |                |               |              |                |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Reversed**           |     **4,774.8 ns** |     **393.86 ns** |    **205.99 ns** |     **4,692.9 ns** |  **1.00** |    **0.06** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Reversed           |    38,240.4 ns |     178.72 ns |     93.47 ns |    38,239.5 ns |  8.02 |    0.31 |    7 |         - |          NA |
| QuickSortMedian3             | 1024 | Reversed           |    52,001.2 ns |     242.86 ns |    107.83 ns |    52,010.4 ns | 10.91 |    0.43 |    7 |         - |          NA |
| QuickSortMedian9             | 1024 | Reversed           |    22,443.5 ns |     686.39 ns |    304.76 ns |    22,263.1 ns |  4.71 |    0.19 |    6 |         - |          NA |
| DualPivotQuickSort           | 1024 | Reversed           |    20,066.6 ns |     551.71 ns |    288.56 ns |    19,967.1 ns |  4.21 |    0.17 |    6 |         - |          NA |
| StableQuickSort              | 1024 | Reversed           |    42,358.2 ns |     440.43 ns |    195.55 ns |    42,333.4 ns |  8.89 |    0.35 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Reversed           |    25,364.3 ns |     431.51 ns |    191.60 ns |    25,354.5 ns |  5.32 |    0.21 |    6 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Reversed           |    43,593.0 ns |     424.13 ns |    221.83 ns |    43,607.1 ns |  9.14 |    0.36 |    7 |         - |          NA |
| IntroSort                    | 1024 | Reversed           |     2,942.4 ns |      36.74 ns |     16.31 ns |     2,939.0 ns |  0.62 |    0.02 |    3 |         - |          NA |
| IntroSortDotnet              | 1024 | Reversed           |     7,435.6 ns |     437.77 ns |    228.96 ns |     7,311.4 ns |  1.56 |    0.08 |    5 |         - |          NA |
| PDQSort                      | 1024 | Reversed           |     1,904.9 ns |      11.61 ns |      4.14 ns |     1,903.1 ns |  0.40 |    0.02 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | Reversed           |     3,341.4 ns |     296.24 ns |    131.53 ns |     3,363.2 ns |  0.70 |    0.04 |    3 |         - |          NA |
| Ipnsort                      | 1024 | Reversed           |       802.4 ns |       8.38 ns |      3.72 ns |       800.4 ns |  0.17 |    0.01 |    1 |         - |          NA |
| StdSort                      | 1024 | Reversed           |     2,607.9 ns |     297.65 ns |    155.68 ns |     2,504.4 ns |  0.55 |    0.04 |    3 |         - |          NA |
| BlockQuickSort               | 1024 | Reversed           |     7,712.6 ns |     194.66 ns |     86.43 ns |     7,673.6 ns |  1.62 |    0.07 |    5 |         - |          NA |
| DotnetSort                   | 1024 | Reversed           |     8,089.5 ns |     891.41 ns |    466.23 ns |     7,965.5 ns |  1.70 |    0.11 |    5 |         - |          NA |
|      |                    |                |               |              |                |       |         |      |           |             |
| **QuickSort**                    | **1024** | **PipeOrgan**          |    **97,715.3 ns** |     **392.02 ns** |    **174.06 ns** |    **97,740.9 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| QuickSort3way                | 1024 | PipeOrgan          |    31,569.1 ns |     456.32 ns |    238.67 ns |    31,564.3 ns |  0.32 |    0.00 |    4 |         - |          NA |
| QuickSortMedian3             | 1024 | PipeOrgan          |    38,215.0 ns |   1,075.37 ns |    562.44 ns |    38,261.9 ns |  0.39 |    0.01 |    5 |         - |          NA |
| QuickSortMedian9             | 1024 | PipeOrgan          |    21,915.5 ns |     131.18 ns |     58.24 ns |    21,907.5 ns |  0.22 |    0.00 |    3 |         - |          NA |
| DualPivotQuickSort           | 1024 | PipeOrgan          |     9,286.5 ns |     436.76 ns |    228.43 ns |     9,301.6 ns |  0.10 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 1024 | PipeOrgan          |    45,824.0 ns |     416.64 ns |    184.99 ns |    45,809.7 ns |  0.47 |    0.00 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | PipeOrgan          |    24,109.5 ns |     607.69 ns |    317.83 ns |    23,996.7 ns |  0.25 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 1024 | PipeOrgan          |    49,510.4 ns |   1,242.68 ns |    649.95 ns |    49,448.0 ns |  0.51 |    0.01 |    5 |         - |          NA |
| IntroSort                    | 1024 | PipeOrgan          |    11,756.2 ns |   2,254.93 ns |  1,179.37 ns |    11,257.7 ns |  0.12 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 1024 | PipeOrgan          |    13,800.9 ns |     563.34 ns |    250.13 ns |    13,827.8 ns |  0.14 |    0.00 |    2 |         - |          NA |
| PDQSort                      | 1024 | PipeOrgan          |     8,841.2 ns |     515.55 ns |    269.64 ns |     8,859.0 ns |  0.09 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | PipeOrgan          |    16,157.5 ns |     339.24 ns |    150.62 ns |    16,167.2 ns |  0.17 |    0.00 |    2 |         - |          NA |
| Ipnsort                      | 1024 | PipeOrgan          |    19,554.3 ns |     198.46 ns |     88.12 ns |    19,565.5 ns |  0.20 |    0.00 |    3 |         - |          NA |
| StdSort                      | 1024 | PipeOrgan          |    13,383.9 ns |     525.22 ns |    233.20 ns |    13,466.2 ns |  0.14 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | PipeOrgan          |    23,045.8 ns |     272.86 ns |    121.15 ns |    23,075.5 ns |  0.24 |    0.00 |    3 |         - |          NA |
| DotnetSort                   | 1024 | PipeOrgan          |    16,086.6 ns |   1,013.67 ns |    530.17 ns |    16,192.1 ns |  0.16 |    0.01 |    2 |         - |          NA |
|      |                    |                |               |              |                |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Random**             |   **420,843.6 ns** |   **3,926.73 ns** |  **2,053.76 ns** |   **421,108.4 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Random             |   440,346.0 ns |   1,351.51 ns |    706.87 ns |   440,563.7 ns |  1.05 |    0.01 |    2 |         - |          NA |
| QuickSortMedian3             | 8192 | Random             |   533,828.5 ns |   2,108.63 ns |  1,102.85 ns |   534,114.8 ns |  1.27 |    0.01 |    2 |         - |          NA |
| QuickSortMedian9             | 8192 | Random             |   516,437.1 ns |   1,751.69 ns |    916.17 ns |   516,466.0 ns |  1.23 |    0.01 |    2 |         - |          NA |
| DualPivotQuickSort           | 8192 | Random             |   349,150.9 ns |   1,655.62 ns |    865.92 ns |   349,291.1 ns |  0.83 |    0.00 |    2 |         - |          NA |
| StableQuickSort              | 8192 | Random             | 1,153,320.9 ns |   2,171.36 ns |  1,135.66 ns | 1,152,984.4 ns |  2.74 |    0.01 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Random             |   965,826.5 ns |   1,898.00 ns |    992.69 ns |   965,672.5 ns |  2.30 |    0.01 |    3 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Random             |   841,936.5 ns |   1,342.48 ns |    702.14 ns |   842,009.5 ns |  2.00 |    0.01 |    3 |         - |          NA |
| IntroSort                    | 8192 | Random             |   366,124.9 ns |   2,254.13 ns |  1,178.95 ns |   365,949.2 ns |  0.87 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet              | 8192 | Random             |   355,535.3 ns |     961.71 ns |    502.99 ns |   355,311.9 ns |  0.84 |    0.00 |    2 |         - |          NA |
| PDQSort                      | 8192 | Random             |   344,141.8 ns |   1,062.49 ns |    555.70 ns |   344,163.1 ns |  0.82 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | Random             |   194,847.1 ns |   2,269.36 ns |    809.27 ns |   194,628.5 ns |  0.46 |    0.00 |    1 |         - |          NA |
| Ipnsort                      | 8192 | Random             |   199,219.1 ns |   1,539.99 ns |    805.45 ns |   199,436.0 ns |  0.47 |    0.00 |    1 |         - |          NA |
| StdSort                      | 8192 | Random             |   371,195.6 ns | 108,043.27 ns | 47,971.88 ns |   352,060.3 ns |  0.88 |    0.11 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | Random             |   421,527.1 ns |   1,217.32 ns |    636.68 ns |   421,497.2 ns |  1.00 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 8192 | Random             |   338,339.2 ns |   8,666.36 ns |  4,532.67 ns |   340,188.4 ns |  0.80 |    0.01 |    2 |         - |          NA |
|      |                    |                |               |              |                |       |         |      |           |             |
| **QuickSort**                    | **8192** | **SingleElementMoved** |    **54,768.4 ns** |   **1,808.71 ns** |    **945.99 ns** |    **54,665.1 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 8192 | SingleElementMoved |   855,822.4 ns |   4,439.75 ns |  2,322.08 ns |   856,019.5 ns | 15.63 |    0.26 |   10 |         - |          NA |
| QuickSortMedian3             | 8192 | SingleElementMoved |   571,714.0 ns |   4,693.25 ns |  2,083.83 ns |   571,363.5 ns | 10.44 |    0.17 |    9 |         - |          NA |
| QuickSortMedian9             | 8192 | SingleElementMoved |   213,042.3 ns |   3,839.86 ns |  2,008.32 ns |   212,676.2 ns |  3.89 |    0.07 |    6 |         - |          NA |
| DualPivotQuickSort           | 8192 | SingleElementMoved |   140,662.9 ns |   2,083.76 ns |  1,089.84 ns |   140,964.4 ns |  2.57 |    0.05 |    4 |         - |          NA |
| StableQuickSort              | 8192 | SingleElementMoved |   434,762.4 ns |   1,366.90 ns |    714.91 ns |   434,517.4 ns |  7.94 |    0.13 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | SingleElementMoved |   259,973.5 ns |   1,131.22 ns |    591.65 ns |   260,036.5 ns |  4.75 |    0.08 |    7 |         - |          NA |
| DestswapStableQuickSort      | 8192 | SingleElementMoved |   379,051.7 ns |   1,268.38 ns |    563.17 ns |   379,016.0 ns |  6.92 |    0.11 |    8 |         - |          NA |
| IntroSort                    | 8192 | SingleElementMoved |    41,317.8 ns |   3,485.74 ns |  1,823.11 ns |    40,946.1 ns |  0.75 |    0.03 |    1 |         - |          NA |
| IntroSortDotnet              | 8192 | SingleElementMoved |    61,121.3 ns |     660.81 ns |    345.62 ns |    61,058.5 ns |  1.12 |    0.02 |    2 |         - |          NA |
| PDQSort                      | 8192 | SingleElementMoved |    41,754.8 ns |     372.00 ns |    194.56 ns |    41,788.2 ns |  0.76 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 8192 | SingleElementMoved |    54,505.2 ns |     818.32 ns |    428.00 ns |    54,474.6 ns |  1.00 |    0.02 |    2 |         - |          NA |
| Ipnsort                      | 8192 | SingleElementMoved |   174,993.4 ns |   1,436.50 ns |    751.32 ns |   174,782.6 ns |  3.20 |    0.05 |    5 |         - |          NA |
| StdSort                      | 8192 | SingleElementMoved |    61,806.9 ns |     899.67 ns |    470.54 ns |    61,714.5 ns |  1.13 |    0.02 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | SingleElementMoved |    80,587.2 ns |   1,982.84 ns |  1,037.06 ns |    80,806.9 ns |  1.47 |    0.03 |    3 |         - |          NA |
| DotnetSort                   | 8192 | SingleElementMoved |    60,284.2 ns |   7,578.48 ns |  3,963.69 ns |    61,087.0 ns |  1.10 |    0.07 |    2 |         - |          NA |
|      |                    |                |               |              |                |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Sorted**             |    **41,810.2 ns** |   **1,457.80 ns** |    **762.46 ns** |    **41,794.1 ns** |  **1.00** |    **0.02** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Sorted             | 1,171,088.0 ns |   3,079.40 ns |  1,367.27 ns | 1,170,819.4 ns | 28.02 |    0.48 |   10 |         - |          NA |
| QuickSortMedian3             | 8192 | Sorted             |   887,435.9 ns |   4,705.02 ns |  2,089.06 ns |   887,392.2 ns | 21.23 |    0.37 |    9 |         - |          NA |
| QuickSortMedian9             | 8192 | Sorted             |   212,180.7 ns |   5,586.67 ns |  2,921.94 ns |   213,192.3 ns |  5.08 |    0.11 |    7 |         - |          NA |
| DualPivotQuickSort           | 8192 | Sorted             |   152,474.1 ns |   2,290.66 ns |  1,198.06 ns |   152,940.6 ns |  3.65 |    0.07 |    6 |         - |          NA |
| StableQuickSort              | 8192 | Sorted             |   433,785.5 ns |   3,346.65 ns |  1,485.93 ns |   433,430.1 ns | 10.38 |    0.18 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Sorted             |   254,551.0 ns |  11,661.73 ns |  6,099.31 ns |   252,390.9 ns |  6.09 |    0.17 |    7 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Sorted             |   380,861.7 ns |   1,889.67 ns |    988.33 ns |   380,748.5 ns |  9.11 |    0.16 |    8 |         - |          NA |
| IntroSort                    | 8192 | Sorted             |     8,926.6 ns |   1,261.49 ns |    560.11 ns |     9,028.8 ns |  0.21 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 8192 | Sorted             |    48,053.5 ns |   1,047.36 ns |    547.79 ns |    47,928.4 ns |  1.15 |    0.02 |    4 |         - |          NA |
| PDQSort                      | 8192 | Sorted             |    10,391.2 ns |     293.66 ns |    153.59 ns |    10,478.6 ns |  0.25 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | Sorted             |    10,602.0 ns |     349.43 ns |    182.76 ns |    10,686.9 ns |  0.25 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 8192 | Sorted             |     4,400.8 ns |     714.46 ns |    317.23 ns |     4,367.3 ns |  0.11 |    0.01 |    1 |         - |          NA |
| StdSort                      | 8192 | Sorted             |    15,199.5 ns |   1,165.93 ns |    609.81 ns |    14,913.5 ns |  0.36 |    0.02 |    3 |         - |          NA |
| BlockQuickSort               | 8192 | Sorted             |    68,069.5 ns |     989.90 ns |    517.74 ns |    68,199.6 ns |  1.63 |    0.03 |    5 |         - |          NA |
| DotnetSort                   | 8192 | Sorted             |    43,358.9 ns |   2,760.75 ns |  1,225.79 ns |    42,993.8 ns |  1.04 |    0.03 |    4 |         - |          NA |
|      |                    |                |               |              |                |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Reversed**           |    **45,626.7 ns** |     **896.53 ns** |    **398.06 ns** |    **45,503.0 ns** |  **1.00** |    **0.01** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Reversed           |   835,665.5 ns |   3,103.85 ns |  1,623.37 ns |   835,581.2 ns | 18.32 |    0.15 |   12 |         - |          NA |
| QuickSortMedian3             | 8192 | Reversed           | 1,127,066.4 ns |  14,036.14 ns |  5,005.42 ns | 1,126,164.8 ns | 24.70 |    0.22 |   13 |         - |          NA |
| QuickSortMedian9             | 8192 | Reversed           |   208,543.2 ns |   5,653.43 ns |  2,956.85 ns |   209,360.6 ns |  4.57 |    0.07 |    9 |         - |          NA |
| DualPivotQuickSort           | 8192 | Reversed           |   143,192.8 ns |   2,198.90 ns |  1,150.07 ns |   142,749.1 ns |  3.14 |    0.03 |    8 |         - |          NA |
| StableQuickSort              | 8192 | Reversed           |   432,608.6 ns |   3,169.70 ns |  1,657.82 ns |   432,141.4 ns |  9.48 |    0.08 |   11 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Reversed           |   252,196.0 ns |   3,883.16 ns |  2,030.97 ns |   252,382.1 ns |  5.53 |    0.06 |   10 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Reversed           |   386,838.7 ns |   3,607.83 ns |  1,601.90 ns |   386,469.7 ns |  8.48 |    0.08 |   11 |         - |          NA |
| IntroSort                    | 8192 | Reversed           |    24,407.1 ns |   1,530.77 ns |    800.62 ns |    24,318.9 ns |  0.53 |    0.02 |    4 |         - |          NA |
| IntroSortDotnet              | 8192 | Reversed           |    76,503.9 ns |     989.93 ns |    439.54 ns |    76,474.1 ns |  1.68 |    0.02 |    6 |         - |          NA |
| PDQSort                      | 8192 | Reversed           |    14,446.7 ns |     259.33 ns |    115.15 ns |    14,438.8 ns |  0.32 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | Reversed           |    23,130.5 ns |     314.81 ns |    164.65 ns |    23,124.9 ns |  0.51 |    0.01 |    4 |         - |          NA |
| Ipnsort                      | 8192 | Reversed           |     6,417.3 ns |     313.83 ns |    164.14 ns |     6,406.9 ns |  0.14 |    0.00 |    1 |         - |          NA |
| StdSort                      | 8192 | Reversed           |    18,784.3 ns |     919.71 ns |    408.36 ns |    18,681.7 ns |  0.41 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 8192 | Reversed           |    75,398.6 ns |     631.21 ns |    280.26 ns |    75,465.5 ns |  1.65 |    0.01 |    6 |         - |          NA |
| DotnetSort                   | 8192 | Reversed           |    97,232.9 ns |  11,907.31 ns |  6,227.75 ns |    99,960.3 ns |  2.13 |    0.13 |    7 |         - |          NA |
|      |                    |                |               |              |                |       |         |      |           |             |
| **QuickSort**                    | **8192** | **PipeOrgan**          | **5,383,913.6 ns** |   **7,196.64 ns** |  **3,763.98 ns** | **5,383,466.3 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 8192 | PipeOrgan          |   452,075.7 ns |   1,872.50 ns |    979.35 ns |   451,887.9 ns |  0.08 |    0.00 |    3 |         - |          NA |
| QuickSortMedian3             | 8192 | PipeOrgan          |   496,300.4 ns |   5,744.11 ns |  3,004.28 ns |   495,914.7 ns |  0.09 |    0.00 |    3 |         - |          NA |
| QuickSortMedian9             | 8192 | PipeOrgan          |   279,724.2 ns |   2,804.54 ns |  1,466.83 ns |   280,188.2 ns |  0.05 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort           | 8192 | PipeOrgan          |   122,886.7 ns |   1,959.40 ns |  1,024.80 ns |   123,032.9 ns |  0.02 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 8192 | PipeOrgan          |   472,391.5 ns |   3,742.38 ns |  1,957.34 ns |   471,967.2 ns |  0.09 |    0.00 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | PipeOrgan          |   238,155.7 ns |   4,496.05 ns |  1,996.27 ns |   238,431.2 ns |  0.04 |    0.00 |    2 |         - |          NA |
| DestswapStableQuickSort      | 8192 | PipeOrgan          |   458,449.9 ns |   1,349.19 ns |    705.65 ns |   458,259.9 ns |  0.09 |    0.00 |    3 |         - |          NA |
| IntroSort                    | 8192 | PipeOrgan          |   263,834.2 ns |   6,024.58 ns |  3,150.97 ns |   263,777.7 ns |  0.05 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet              | 8192 | PipeOrgan          |   360,011.2 ns |     831.31 ns |    369.11 ns |   359,932.5 ns |  0.07 |    0.00 |    2 |         - |          NA |
| PDQSort                      | 8192 | PipeOrgan          |   115,515.5 ns |   1,628.22 ns |    722.94 ns |   115,531.8 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 8192 | PipeOrgan          |   200,553.1 ns |   2,074.54 ns |  1,085.03 ns |   200,495.7 ns |  0.04 |    0.00 |    2 |         - |          NA |
| Ipnsort                      | 8192 | PipeOrgan          |   218,076.7 ns |     769.86 ns |    341.82 ns |   218,021.6 ns |  0.04 |    0.00 |    2 |         - |          NA |
| StdSort                      | 8192 | PipeOrgan          |   302,125.9 ns |  16,478.00 ns |  8,618.31 ns |   304,992.2 ns |  0.06 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | PipeOrgan          |   255,636.0 ns |   1,033.45 ns |    458.86 ns |   255,640.6 ns |  0.05 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 8192 | PipeOrgan          |   346,643.4 ns |   2,141.56 ns |    950.87 ns |   347,102.8 ns |  0.06 |    0.00 |    2 |         - |          NA |

### SelectionBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method              | Size | Pattern            | Mean           | Error       | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------------- |----- |------------------- |---------------:|------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **SelectionSort**       | **256**  | **Random**             |    **26,169.8 ns** |    **195.3 ns** |   **102.15 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Random             |    24,838.2 ns |    295.8 ns |   154.73 ns |  0.95 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | Random             |    91,509.7 ns |  6,482.7 ns | 2,878.35 ns |  3.50 |    0.10 |    3 |         - |          NA |
| PancakeSort         | 256  | Random             |    42,997.0 ns |    369.0 ns |   192.98 ns |  1.64 |    0.01 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **SelectionSort**       | **256**  | **SingleElementMoved** |    **22,309.3 ns** |    **444.0 ns** |   **232.21 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | SingleElementMoved |    20,864.5 ns |    188.8 ns |    83.82 ns |  0.94 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | SingleElementMoved |    50,358.0 ns |    923.8 ns |   410.19 ns |  2.26 |    0.03 |    2 |         - |          NA |
| PancakeSort         | 256  | SingleElementMoved |    19,805.0 ns |    230.6 ns |   120.62 ns |  0.89 |    0.01 |    1 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **SelectionSort**       | **256**  | **Sorted**             |    **22,143.3 ns** |    **379.4 ns** |   **168.45 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Sorted             |    16,224.4 ns |    142.3 ns |    63.20 ns |  0.73 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | Sorted             |    32,160.3 ns |    263.4 ns |   137.77 ns |  1.45 |    0.01 |    3 |         - |          NA |
| PancakeSort         | 256  | Sorted             |    15,048.3 ns |    212.5 ns |    94.37 ns |  0.68 |    0.01 |    1 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **SelectionSort**       | **256**  | **Reversed**           |    **20,893.2 ns** |  **2,592.2 ns** | **1,355.75 ns** |  **1.00** |    **0.09** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Reversed           |    16,288.1 ns |    102.2 ns |    45.39 ns |  0.78 |    0.05 |    1 |         - |          NA |
| CycleSort           | 256  | Reversed           |    44,242.0 ns |    330.9 ns |   173.04 ns |  2.13 |    0.13 |    3 |         - |          NA |
| PancakeSort         | 256  | Reversed           |    19,765.5 ns |  4,590.3 ns | 2,400.83 ns |  0.95 |    0.12 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **SelectionSort**       | **256**  | **PipeOrgan**          |    **24,905.2 ns** |    **540.2 ns** |   **282.52 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | PipeOrgan          |    18,137.3 ns |  1,068.4 ns |   558.82 ns |  0.73 |    0.02 |    1 |         - |          NA |
| CycleSort           | 256  | PipeOrgan          |    60,090.6 ns |  2,364.3 ns | 1,236.59 ns |  2.41 |    0.05 |    4 |         - |          NA |
| PancakeSort         | 256  | PipeOrgan          |    33,214.6 ns |    224.9 ns |   117.62 ns |  1.33 |    0.01 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Random**             |   **368,187.9 ns** |  **1,188.7 ns** |   **621.73 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Random             |   358,194.2 ns |  1,493.4 ns |   663.06 ns |  0.97 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | Random             | 1,654,184.5 ns |  5,732.1 ns | 2,545.08 ns |  4.49 |    0.01 |    3 |         - |          NA |
| PancakeSort         | 1024 | Random             |   622,027.5 ns |  2,605.0 ns | 1,156.62 ns |  1.69 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **SelectionSort**       | **1024** | **SingleElementMoved** |   **332,896.3 ns** |    **928.6 ns** |   **485.70 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | SingleElementMoved |   312,375.9 ns |  1,225.1 ns |   640.77 ns |  0.94 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | SingleElementMoved |   768,409.5 ns |  5,129.3 ns | 2,277.45 ns |  2.31 |    0.01 |    2 |         - |          NA |
| PancakeSort         | 1024 | SingleElementMoved |   294,851.9 ns |  4,044.2 ns | 2,115.20 ns |  0.89 |    0.01 |    1 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Sorted**             |   **332,322.9 ns** |  **1,170.9 ns** |   **519.88 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Sorted             |   248,260.7 ns |    807.0 ns |   287.79 ns |  0.75 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | Sorted             |   494,882.4 ns |  1,509.6 ns |   670.28 ns |  1.49 |    0.00 |    3 |         - |          NA |
| PancakeSort         | 1024 | Sorted             |   223,697.5 ns |    226.5 ns |   100.57 ns |  0.67 |    0.00 |    1 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Reversed**           |   **315,738.2 ns** | **15,984.2 ns** | **8,360.03 ns** |  **1.00** |    **0.04** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Reversed           |   249,235.1 ns |  1,022.7 ns |   534.90 ns |  0.79 |    0.02 |    1 |         - |          NA |
| CycleSort           | 1024 | Reversed           |   666,462.7 ns |  2,991.9 ns | 1,564.83 ns |  2.11 |    0.05 |    3 |         - |          NA |
| PancakeSort         | 1024 | Reversed           |   315,297.9 ns | 10,898.4 ns | 5,700.10 ns |  1.00 |    0.03 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **SelectionSort**       | **1024** | **PipeOrgan**          |   **350,805.6 ns** |  **3,969.2 ns** | **2,075.98 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | PipeOrgan          |   256,022.4 ns |  4,924.2 ns | 2,575.46 ns |  0.73 |    0.01 |    1 |         - |          NA |
| CycleSort           | 1024 | PipeOrgan          |   905,475.1 ns | 11,627.9 ns | 6,081.59 ns |  2.58 |    0.02 |    4 |         - |          NA |
| PancakeSort         | 1024 | PipeOrgan          |   501,597.0 ns |  1,136.0 ns |   504.38 ns |  1.43 |    0.01 |    3 |         - |          NA |

### TreeBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                 | Size | Pattern            | Mean           | Error       | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------- |----- |------------------- |---------------:|------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **BalancedBinaryTreeSort** | **256**  | **Random**             |    **13,092.5 ns** |   **493.34 ns** |   **258.02 ns** |  **3.82** |    **0.11** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 256  | Random             |     3,431.8 ns |   199.17 ns |    88.43 ns |  1.00 |    0.03 |    1 |         - |          NA |
| SplaySort              | 256  | Random             |    22,491.1 ns |   604.46 ns |   316.14 ns |  6.56 |    0.17 |    4 |         - |          NA |
| TreapSort              | 256  | Random             |     9,081.0 ns |   645.40 ns |   337.56 ns |  2.65 |    0.11 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **SingleElementMoved** |    **15,169.3 ns** |   **784.97 ns** |   **410.55 ns** |  **0.30** |    **0.01** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 256  | SingleElementMoved |    50,641.7 ns |   149.69 ns |    66.46 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 256  | SingleElementMoved |     4,298.7 ns |   423.82 ns |   221.67 ns |  0.08 |    0.00 |    1 |         - |          NA |
| TreapSort              | 256  | SingleElementMoved |     5,903.0 ns |   267.40 ns |   139.86 ns |  0.12 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Sorted**             |    **13,356.4 ns** |   **981.38 ns** |   **513.28 ns** |  **0.18** |    **0.01** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 256  | Sorted             |    75,889.6 ns |   192.91 ns |    68.79 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 256  | Sorted             |     3,687.1 ns |     4.06 ns |     1.80 ns |  0.05 |    0.00 |    1 |         - |          NA |
| TreapSort              | 256  | Sorted             |     5,004.8 ns |   107.03 ns |    38.17 ns |  0.07 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Reversed**           |    **12,096.0 ns** |   **271.26 ns** |   **141.87 ns** |  **0.16** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 256  | Reversed           |    73,598.6 ns |   348.65 ns |   182.35 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 256  | Reversed           |     3,587.8 ns |   226.94 ns |   100.76 ns |  0.05 |    0.00 |    1 |         - |          NA |
| TreapSort              | 256  | Reversed           |     5,632.6 ns |   386.77 ns |   202.29 ns |  0.08 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **PipeOrgan**          |    **12,677.5 ns** |   **747.68 ns** |   **391.05 ns** |  **0.33** |    **0.01** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 256  | PipeOrgan          |    38,281.3 ns |    78.55 ns |    34.88 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 256  | PipeOrgan          |     4,439.1 ns |   377.55 ns |   197.47 ns |  0.12 |    0.00 |    1 |         - |          NA |
| TreapSort              | 256  | PipeOrgan          |     6,878.9 ns |   145.03 ns |    51.72 ns |  0.18 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Random**             |   **123,982.0 ns** | **4,528.02 ns** | **2,368.24 ns** |  **6.11** |    **0.13** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 1024 | Random             |    20,308.0 ns |   444.57 ns |   232.52 ns |  1.00 |    0.02 |    1 |         - |          NA |
| SplaySort              | 1024 | Random             |   162,925.6 ns | 2,750.26 ns | 1,221.13 ns |  8.02 |    0.10 |    4 |         - |          NA |
| TreapSort              | 1024 | Random             |    40,437.1 ns | 2,274.04 ns | 1,189.37 ns |  1.99 |    0.06 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **SingleElementMoved** |   **106,213.4 ns** | **1,662.87 ns** |   **869.71 ns** |  **0.14** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 1024 | SingleElementMoved |   780,559.9 ns |   674.93 ns |   299.67 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | SingleElementMoved |    16,239.0 ns |    63.16 ns |    28.04 ns |  0.02 |    0.00 |    1 |         - |          NA |
| TreapSort              | 1024 | SingleElementMoved |    29,404.5 ns |   603.08 ns |   315.42 ns |  0.04 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Sorted**             |    **99,155.0 ns** |   **963.51 ns** |   **427.80 ns** |  **0.08** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 1024 | Sorted             | 1,190,879.7 ns | 1,235.84 ns |   646.37 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | Sorted             |    14,826.5 ns |   197.69 ns |    87.78 ns |  0.01 |    0.00 |    1 |         - |          NA |
| TreapSort              | 1024 | Sorted             |    24,223.2 ns |   533.16 ns |   278.85 ns |  0.02 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Reversed**           |    **59,476.9 ns** | **1,018.16 ns** |   **363.09 ns** |  **0.05** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 1024 | Reversed           | 1,152,043.7 ns | 1,237.21 ns |   647.09 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | Reversed           |    14,352.4 ns |   298.95 ns |   156.36 ns |  0.01 |    0.00 |    1 |         - |          NA |
| TreapSort              | 1024 | Reversed           |    23,237.1 ns |   463.88 ns |   205.97 ns |  0.02 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **PipeOrgan**          |    **91,004.5 ns** | **1,666.04 ns** |   **871.37 ns** |  **0.16** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 1024 | PipeOrgan          |   583,054.0 ns |   701.46 ns |   366.88 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | PipeOrgan          |    17,124.1 ns |   245.11 ns |   128.20 ns |  0.03 |    0.00 |    1 |         - |          NA |
| TreapSort              | 1024 | PipeOrgan          |    34,234.8 ns | 1,527.46 ns |   798.89 ns |  0.06 |    0.00 |    2 |         - |          NA |

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
- [Cycle Sort](./src/SortAlgorithm/Algorithms/Selection/CycleSort.cs)
- [Double Selection Sort](./src/SortAlgorithm/Algorithms/Selection/DoubleSelectionSort.cs)
- [Pancake Sort](./src/SortAlgorithm/Algorithms/Selection/PancakeSort.cs)
- [Selection Sort](./src/SortAlgorithm/Algorithms/Selection/SelectionSort.cs)

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
- [Flat Stable Sort](./src/SortAlgorithm/Algorithms/Merge/FlatStableSort.cs)
- [Glidesort](./src/SortAlgorithm/Algorithms/Merge/Glidesort.cs)
- [Merge Sort](./src/SortAlgorithm/Algorithms/Merge/MergeSort.cs)
- [Natural Merge Sort](./src/SortAlgorithm/Algorithms/Merge/NaturalMergeSort.cs)
- [Pingpong Merge Sort](./src/SortAlgorithm/Algorithms/Merge/PingpongMergeSort.cs)
- [Power Sort](./src/SortAlgorithm/Algorithms/Merge/PowerSort.cs)
- [Rotate Merge Sort](./src/SortAlgorithm/Algorithms/Merge/RotateMergeSort.cs)
  - Iterative
  - Recursive
- [Shift Sort](./src/SortAlgorithm/Algorithms/Merge/ShiftSort.cs)
- [Spin Sort](./src/SortAlgorithm/Algorithms/Merge/SpinSort.cs)
- [Spin Sort (Boost)](./src/SortAlgorithm/Algorithms/Merge/SpinSortVariant.cs)
- [std::stable_sort (LLVM)](./src/SortAlgorithm/Algorithms/Merge/StdStableSort.cs)
- [SymMerge Sort](./src/SortAlgorithm/Algorithms/Merge/SymMergeSort.cs)
- [Tim Sort](./src/SortAlgorithm/Algorithms/Merge/TimSort.cs)

### Heap
- [Bottom-Up Heap Sort](./src/SortAlgorithm/Algorithms/Heap/BottomupHeapSort.cs)
- [Heap Sort](./src/SortAlgorithm/Algorithms/Heap/HeapSort.cs)
- [Min-Heap Sort](./src/SortAlgorithm/Algorithms/Heap/MinHeapSort.cs)
- [Smooth Sort](./src/SortAlgorithm/Algorithms/Heap/SmoothSort.cs)
- [Ternary Heap Sort](./src/SortAlgorithm/Algorithms/Heap/TernaryHeapSort.cs)
- [Tournament Sort](./src/SortAlgorithm/Algorithms/Heap/TournamentSort.cs)
- [Weak Heap Sort](./src/SortAlgorithm/Algorithms/Heap/WeakHeapSort.cs)

### Partition
- [Quick Sort (Bidirectional Stable)](./src/SortAlgorithm/Algorithms/Partition/BidirectionalStableQuickSort.cs)
- [Block Quick Sort](./src/SortAlgorithm/Algorithms/Partition/BlockQuickSort.cs)
- [Quick Sort (Destswap Stable)](./src/SortAlgorithm/Algorithms/Partition/DestswapStableQuickSort.cs)
- [Quick Sort (Dual Pivot)](./src/SortAlgorithm/Algorithms/Partition/DualPivotQuickSort.cs)
- [Intro Sort](./src/SortAlgorithm/Algorithms/Partition/IntroSort.cs)
- [Intro Sort (Dotnet)](./src/SortAlgorithm/Algorithms/Partition/IntroSortDotnet.cs)
- [Ipnsort](./src/SortAlgorithm/Algorithms/Partition/Ipnsort.cs)
- [Pattern-Defeating Quick Sort](./src/SortAlgorithm/Algorithms/Partition/PDQSort.cs)
- [PDQ Sort Branchless](./src/SortAlgorithm/Algorithms/Partition/PDQSortBranchless.cs)
- [Quick Sort](./src/SortAlgorithm/Algorithms/Partition/QuickSort.cs)
- [Quick Sort (3-Way)](./src/SortAlgorithm/Algorithms/Partition/QuickSort3way.cs)
- [Quick Sort (Median of 3)](./src/SortAlgorithm/Algorithms/Partition/QuickSortMedian3.cs)
- [Quick Sort (Median of 9)](./src/SortAlgorithm/Algorithms/Partition/QuickSortMedian9.cs)
- [Quick Sort (Stable)](./src/SortAlgorithm/Algorithms/Partition/StableQuickSort.cs)
- [std::sort (LLVM)](./src/SortAlgorithm/Algorithms/Partition/StdSort.cs)

### Adaptive
- [Drop-Merge Sort](./src/SortAlgorithm/Algorithms/Adaptive/DropMergeSort.cs)
- [Patience Sort](./src/SortAlgorithm/Algorithms/Adaptive/PatienceSort.cs)
- [Strand Sort](./src/SortAlgorithm/Algorithms/Adaptive/StrandSort.cs)

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
- [Spread Sort](./src/SortAlgorithm/Algorithms/Distribution/SpreadSort.cs)

### Network
- [Batcher Odd-Even Merge Sort](./src/SortAlgorithm/Algorithms/Network/BatcherOddEvenMergeSort.cs)
- [Bitonic Sort](./src/SortAlgorithm/Algorithms/Network/BitonicSort.cs)
  - Iterative
  - Recursive

### Tree
- [Binary Tree Sort (AVL)](./src/SortAlgorithm/Algorithms/Tree/BalancedBinaryTreeSort.cs)
- [Binary Tree Sort (BST)](./src/SortAlgorithm/Algorithms/Tree/BinaryTreeSort.cs)
- [Splay Sort](./src/SortAlgorithm/Algorithms/Tree/SplaySort.cs)
- [Treap Sort](./src/SortAlgorithm/Algorithms/Tree/TreapSort.cs)

### Joke
- [Bogo Sort](./src/SortAlgorithm/Algorithms/Joke/BogoSort.cs)
- [Slow Sort](./src/SortAlgorithm/Algorithms/Joke/SlowSort.cs)
- [Stooge Sort](./src/SortAlgorithm/Algorithms/Joke/StoogeSort.cs)
<!-- ALGORITHMS_END -->
