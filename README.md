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
<summary>Benchmark results (2026-07-27 15:12 UTC)</summary>

Workflow run: https://github.com/guitarrapc/SortAlgorithms/actions/runs/30269084385

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

| Method        | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **DropMergeSort** | **256**  | **Random**             |     **4,949.4 ns** |    **518.41 ns** |    **271.14 ns** |  **1.00** |    **0.07** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | Random             |     9,954.4 ns |    673.98 ns |    352.50 ns |  2.02 |    0.12 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **256**  | **SingleElementMoved** |       **883.6 ns** |    **308.68 ns** |    **161.45 ns** |  **1.04** |    **0.29** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | SingleElementMoved |     7,675.5 ns |    955.82 ns |    424.39 ns |  9.01 |    1.99 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **256**  | **Sorted**             |       **531.0 ns** |      **1.93 ns** |      **1.01 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | Sorted             |    14,107.6 ns |    264.53 ns |    138.35 ns | 26.57 |    0.25 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **256**  | **Reversed**           |     **7,984.8 ns** |     **70.50 ns** |     **36.87 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| PatienceSort  | 256  | Reversed           |     1,652.1 ns |     11.91 ns |      5.29 ns |  0.21 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **256**  | **PipeOrgan**          |     **7,600.3 ns** |    **260.06 ns** |    **115.47 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| PatienceSort  | 256  | PipeOrgan          |     5,341.9 ns |    390.50 ns |    204.24 ns |  0.70 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **Random**             |    **25,734.7 ns** |  **4,239.89 ns** |  **1,882.54 ns** |  **1.00** |    **0.09** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | Random             |    22,539.8 ns |    862.56 ns |    451.13 ns |  0.88 |    0.06 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **SingleElementMoved** |     **2,519.6 ns** |    **417.06 ns** |    **218.13 ns** |  **1.01** |    **0.12** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | SingleElementMoved |    39,513.0 ns |  1,904.20 ns |    845.48 ns | 15.78 |    1.32 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **Sorted**             |     **2,100.9 ns** |    **409.51 ns** |    **214.18 ns** |  **1.01** |    **0.13** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | Sorted             |    42,684.1 ns |  6,873.15 ns |  3,594.79 ns | 20.49 |    2.47 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **Reversed**           |    **52,613.8 ns** |    **210.76 ns** |     **93.58 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PatienceSort  | 1024 | Reversed           |     6,168.4 ns |    351.60 ns |    183.89 ns |  0.12 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **PipeOrgan**          |    **40,716.6 ns** |  **2,447.82 ns** |  **1,280.26 ns** |  **1.00** |    **0.04** |    **2** |         **-** |          **NA** |
| PatienceSort  | 1024 | PipeOrgan          |    26,338.8 ns |  1,157.32 ns |    605.30 ns |  0.65 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **Random**             |   **538,062.4 ns** |  **2,228.98 ns** |  **1,165.80 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | Random             |   716,716.6 ns |  1,616.28 ns |    845.34 ns |  1.33 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **SingleElementMoved** |    **17,252.2 ns** |    **108.41 ns** |     **48.14 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | SingleElementMoved |   734,821.0 ns | 22,677.92 ns | 10,069.14 ns | 42.59 |    0.56 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **Sorted**             |    **15,971.8 ns** |    **756.16 ns** |    **395.49 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | Sorted             |   739,167.5 ns | 17,482.94 ns |  9,143.92 ns | 46.30 |    1.21 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **Reversed**           | **1,128,770.6 ns** | **17,792.70 ns** |  **7,900.07 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| PatienceSort  | 8192 | Reversed           |    46,158.5 ns |    976.68 ns |    510.82 ns |  0.04 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **PipeOrgan**          |   **517,242.8 ns** | **24,521.83 ns** | **12,825.39 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | PipeOrgan          |   574,030.9 ns |  5,702.55 ns |  2,982.54 ns |  1.11 |    0.03 |    1 |         - |          NA |

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

| Method     | Size | Pattern            | Mean         | Error       | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------- |----- |------------------- |-------------:|------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **StrandSort** | **256**  | **Random**             |   **6,357.6 ns** |   **578.83 ns** |   **257.01 ns** |  **1.00** |    **0.05** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **256**  | **SingleElementMoved** |     **825.5 ns** |    **13.17 ns** |     **4.70 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **256**  | **Sorted**             |     **546.4 ns** |     **9.67 ns** |     **4.30 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **256**  | **Reversed**           |  **52,031.9 ns** |   **164.80 ns** |    **86.19 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **256**  | **PipeOrgan**          |  **27,776.9 ns** |   **330.90 ns** |   **173.07 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **1024** | **Random**             |  **56,126.3 ns** |   **277.14 ns** |   **123.05 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **1024** | **SingleElementMoved** |   **2,643.1 ns** |    **12.99 ns** |     **5.77 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **1024** | **Sorted**             |   **1,740.2 ns** |   **427.75 ns** |   **223.72 ns** |  **1.01** |    **0.17** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **1024** | **Reversed**           | **766,280.8 ns** | **2,385.87 ns** | **1,059.34 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **1024** | **PipeOrgan**          | **395,894.0 ns** | **1,573.91 ns** |   **823.19 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |

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

| Method              | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **CountingSort**        | **256**  | **Random**             |     **1,652.4 ns** |      **5.97 ns** |      **2.65 ns** |  **1.60** |    **0.00** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | Random             |     1,029.7 ns |      4.82 ns |      2.52 ns |  1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 256  | Random             |     1,540.7 ns |      4.56 ns |      2.03 ns |  1.50 |    0.00 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | Random             |     1,253.4 ns |    307.98 ns |    161.08 ns |  1.22 |    0.15 |    2 |         - |          NA |
| BucketSort          | 256  | Random             |     8,656.5 ns |    514.17 ns |    268.92 ns |  8.41 |    0.25 |    6 |         - |          NA |
| BucketSortInteger   | 256  | Random             |     2,917.1 ns |    102.32 ns |     36.49 ns |  2.83 |    0.03 |    3 |         - |          NA |
| FlashSort           | 256  | Random             |     4,541.3 ns |    790.25 ns |    350.88 ns |  4.41 |    0.32 |    4 |         - |          NA |
| RadixLSD4Sort       | 256  | Random             |     5,899.3 ns |    356.00 ns |    186.19 ns |  5.73 |    0.17 |    5 |         - |          NA |
| RadixLSD256Sort     | 256  | Random             |     2,825.9 ns |    790.98 ns |    413.70 ns |  2.74 |    0.38 |    3 |         - |          NA |
| RadixLSD10Sort      | 256  | Random             |     3,956.3 ns |     42.54 ns |     15.17 ns |  3.84 |    0.02 |    4 |         - |          NA |
| RadixMSD4Sort       | 256  | Random             |     9,636.7 ns |    305.71 ns |    159.89 ns |  9.36 |    0.15 |    6 |         - |          NA |
| RadixMSD10Sort      | 256  | Random             |    13,750.4 ns |    358.34 ns |    187.42 ns | 13.35 |    0.17 |    7 |         - |          NA |
| AmericanFlagSort    | 256  | Random             |     4,197.3 ns |    768.23 ns |    401.80 ns |  4.08 |    0.37 |    4 |         - |          NA |
| SpreadSort          | 256  | Random             |     1,702.7 ns |     27.41 ns |     12.17 ns |  1.65 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **256**  | **SingleElementMoved** |     **1,756.2 ns** |    **442.83 ns** |    **231.61 ns** |  **1.79** |    **0.22** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | SingleElementMoved |       978.9 ns |      3.99 ns |      2.09 ns |  1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 256  | SingleElementMoved |     1,495.8 ns |      8.50 ns |      3.77 ns |  1.53 |    0.00 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | SingleElementMoved |       989.8 ns |      6.09 ns |      3.19 ns |  1.01 |    0.00 |    1 |         - |          NA |
| BucketSort          | 256  | SingleElementMoved |     3,587.3 ns |    451.93 ns |    236.37 ns |  3.66 |    0.23 |    4 |         - |          NA |
| BucketSortInteger   | 256  | SingleElementMoved |     2,067.8 ns |    114.88 ns |     51.01 ns |  2.11 |    0.05 |    3 |         - |          NA |
| FlashSort           | 256  | SingleElementMoved |     5,115.5 ns |    355.75 ns |    186.07 ns |  5.23 |    0.18 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | SingleElementMoved |     5,882.5 ns |    435.11 ns |    227.57 ns |  6.01 |    0.22 |    5 |         - |          NA |
| RadixLSD256Sort     | 256  | SingleElementMoved |     1,937.5 ns |     18.54 ns |      8.23 ns |  1.98 |    0.01 |    3 |         - |          NA |
| RadixLSD10Sort      | 256  | SingleElementMoved |     4,021.2 ns |     17.14 ns |      6.11 ns |  4.11 |    0.01 |    4 |         - |          NA |
| RadixMSD4Sort       | 256  | SingleElementMoved |     8,574.1 ns |    388.96 ns |    203.43 ns |  8.76 |    0.20 |    6 |         - |          NA |
| RadixMSD10Sort      | 256  | SingleElementMoved |    13,427.3 ns |    286.99 ns |    150.10 ns | 13.72 |    0.15 |    7 |         - |          NA |
| AmericanFlagSort    | 256  | SingleElementMoved |     3,036.5 ns |     19.94 ns |      8.85 ns |  3.10 |    0.01 |    4 |         - |          NA |
| SpreadSort          | 256  | SingleElementMoved |     1,107.2 ns |     16.56 ns |      8.66 ns |  1.13 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **256**  | **Sorted**             |     **1,559.4 ns** |     **15.70 ns** |      **6.97 ns** |  **1.68** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | Sorted             |       929.6 ns |      5.56 ns |      2.47 ns |  1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 256  | Sorted             |     1,412.6 ns |     11.79 ns |      5.23 ns |  1.52 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | Sorted             |       960.8 ns |      5.93 ns |      2.63 ns |  1.03 |    0.00 |    2 |         - |          NA |
| BucketSort          | 256  | Sorted             |     3,135.7 ns |     15.13 ns |      5.40 ns |  3.37 |    0.01 |    5 |         - |          NA |
| BucketSortInteger   | 256  | Sorted             |     2,026.7 ns |    210.66 ns |     93.54 ns |  2.18 |    0.09 |    4 |         - |          NA |
| FlashSort           | 256  | Sorted             |     4,879.0 ns |    336.87 ns |    176.19 ns |  5.25 |    0.18 |    6 |         - |          NA |
| RadixLSD4Sort       | 256  | Sorted             |     5,670.5 ns |     26.85 ns |     11.92 ns |  6.10 |    0.02 |    6 |         - |          NA |
| RadixLSD256Sort     | 256  | Sorted             |     1,987.5 ns |    358.16 ns |    159.02 ns |  2.14 |    0.16 |    4 |         - |          NA |
| RadixLSD10Sort      | 256  | Sorted             |     5,435.4 ns |  1,511.03 ns |    790.30 ns |  5.85 |    0.80 |    6 |         - |          NA |
| RadixMSD4Sort       | 256  | Sorted             |     8,753.1 ns |    413.20 ns |    183.46 ns |  9.42 |    0.19 |    7 |         - |          NA |
| RadixMSD10Sort      | 256  | Sorted             |    13,357.4 ns |    175.44 ns |     91.76 ns | 14.37 |    0.10 |    8 |         - |          NA |
| AmericanFlagSort    | 256  | Sorted             |     2,918.9 ns |     87.68 ns |     31.27 ns |  3.14 |    0.03 |    5 |         - |          NA |
| SpreadSort          | 256  | Sorted             |       380.2 ns |      2.39 ns |      1.25 ns |  0.41 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **256**  | **Reversed**           |     **1,666.3 ns** |     **16.22 ns** |      **5.78 ns** |  **1.81** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | Reversed           |       919.3 ns |      5.48 ns |      2.87 ns |  1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 256  | Reversed           |     1,415.7 ns |      6.33 ns |      3.31 ns |  1.54 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | Reversed           |       885.6 ns |      3.21 ns |      1.43 ns |  0.96 |    0.00 |    2 |         - |          NA |
| BucketSort          | 256  | Reversed           |    12,259.0 ns |  1,212.42 ns |    634.12 ns | 13.34 |    0.65 |    7 |         - |          NA |
| BucketSortInteger   | 256  | Reversed           |     3,065.8 ns |      8.66 ns |      3.09 ns |  3.33 |    0.01 |    4 |         - |          NA |
| FlashSort           | 256  | Reversed           |     4,701.2 ns |  1,162.94 ns |    516.35 ns |  5.11 |    0.52 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | Reversed           |     5,973.4 ns |    383.49 ns |    200.57 ns |  6.50 |    0.21 |    5 |         - |          NA |
| RadixLSD256Sort     | 256  | Reversed           |     1,927.5 ns |     80.96 ns |     35.95 ns |  2.10 |    0.04 |    3 |         - |          NA |
| RadixLSD10Sort      | 256  | Reversed           |     3,909.6 ns |     49.58 ns |     17.68 ns |  4.25 |    0.02 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | Reversed           |     9,562.6 ns |    485.92 ns |    215.75 ns | 10.40 |    0.22 |    6 |         - |          NA |
| RadixMSD10Sort      | 256  | Reversed           |    13,708.1 ns |    139.21 ns |     72.81 ns | 14.91 |    0.09 |    7 |         - |          NA |
| AmericanFlagSort    | 256  | Reversed           |     5,058.2 ns |    807.71 ns |    358.63 ns |  5.50 |    0.36 |    5 |         - |          NA |
| SpreadSort          | 256  | Reversed           |       562.9 ns |     10.09 ns |      5.28 ns |  0.61 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **256**  | **PipeOrgan**          |     **1,510.5 ns** |     **16.81 ns** |      **6.00 ns** |  **1.75** |    **0.01** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | PipeOrgan          |       863.1 ns |      9.97 ns |      4.43 ns |  1.00 |    0.01 |    1 |         - |          NA |
| PigeonSort          | 256  | PipeOrgan          |     1,438.1 ns |     68.54 ns |     24.44 ns |  1.67 |    0.03 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | PipeOrgan          |       927.5 ns |      1.24 ns |      0.44 ns |  1.07 |    0.01 |    1 |         - |          NA |
| BucketSort          | 256  | PipeOrgan          |     6,956.4 ns |    569.67 ns |    297.95 ns |  8.06 |    0.33 |    6 |         - |          NA |
| BucketSortInteger   | 256  | PipeOrgan          |     2,514.8 ns |     12.77 ns |      4.56 ns |  2.91 |    0.01 |    4 |         - |          NA |
| FlashSort           | 256  | PipeOrgan          |     4,515.4 ns |     81.74 ns |     36.29 ns |  5.23 |    0.05 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | PipeOrgan          |     6,050.0 ns |    293.50 ns |    153.51 ns |  7.01 |    0.17 |    6 |         - |          NA |
| RadixLSD256Sort     | 256  | PipeOrgan          |     2,102.7 ns |    115.69 ns |     60.51 ns |  2.44 |    0.07 |    3 |         - |          NA |
| RadixLSD10Sort      | 256  | PipeOrgan          |     3,795.4 ns |     15.83 ns |      5.64 ns |  4.40 |    0.02 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | PipeOrgan          |     9,433.3 ns |    485.08 ns |    253.71 ns | 10.93 |    0.28 |    7 |         - |          NA |
| RadixMSD10Sort      | 256  | PipeOrgan          |    13,771.6 ns |    321.18 ns |    167.98 ns | 15.96 |    0.20 |    8 |         - |          NA |
| AmericanFlagSort    | 256  | PipeOrgan          |     4,453.6 ns |    397.34 ns |    207.82 ns |  5.16 |    0.23 |    5 |         - |          NA |
| SpreadSort          | 256  | PipeOrgan          |     1,679.2 ns |     42.30 ns |     18.78 ns |  1.95 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **Random**             |     **6,046.5 ns** |      **9.47 ns** |      **4.21 ns** |  **1.59** |    **0.00** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Random             |     3,797.5 ns |     15.41 ns |      6.84 ns |  1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 1024 | Random             |     5,442.0 ns |     12.96 ns |      5.76 ns |  1.43 |    0.00 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | Random             |     3,532.4 ns |    271.29 ns |    141.89 ns |  0.93 |    0.04 |    1 |         - |          NA |
| BucketSort          | 1024 | Random             |    52,598.1 ns |  5,546.47 ns |  2,900.91 ns | 13.85 |    0.72 |    6 |         - |          NA |
| BucketSortInteger   | 1024 | Random             |    14,953.3 ns |    308.87 ns |    161.54 ns |  3.94 |    0.04 |    4 |         - |          NA |
| FlashSort           | 1024 | Random             |    17,741.8 ns |    109.11 ns |     38.91 ns |  4.67 |    0.01 |    4 |         - |          NA |
| RadixLSD4Sort       | 1024 | Random             |    25,101.5 ns |    332.59 ns |    173.95 ns |  6.61 |    0.04 |    4 |         - |          NA |
| RadixLSD256Sort     | 1024 | Random             |     9,503.0 ns |    397.36 ns |    207.83 ns |  2.50 |    0.05 |    3 |         - |          NA |
| RadixLSD10Sort      | 1024 | Random             |    20,960.0 ns |    147.57 ns |     77.18 ns |  5.52 |    0.02 |    4 |         - |          NA |
| RadixMSD4Sort       | 1024 | Random             |    38,657.8 ns |    391.11 ns |    173.66 ns | 10.18 |    0.05 |    5 |         - |          NA |
| RadixMSD10Sort      | 1024 | Random             |    51,014.2 ns |    340.15 ns |    177.91 ns | 13.43 |    0.05 |    6 |         - |          NA |
| AmericanFlagSort    | 1024 | Random             |    18,947.5 ns |    190.38 ns |     99.57 ns |  4.99 |    0.03 |    4 |         - |          NA |
| SpreadSort          | 1024 | Random             |     8,767.6 ns |    486.67 ns |    254.54 ns |  2.31 |    0.06 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **SingleElementMoved** |     **6,388.5 ns** |    **217.08 ns** |    **113.54 ns** |  **1.72** |    **0.09** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | SingleElementMoved |     3,724.4 ns |    399.55 ns |    208.97 ns |  1.00 |    0.07 |    1 |         - |          NA |
| PigeonSort          | 1024 | SingleElementMoved |     5,626.7 ns |    415.32 ns |    217.22 ns |  1.51 |    0.09 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | SingleElementMoved |     3,414.6 ns |     25.59 ns |      9.13 ns |  0.92 |    0.05 |    1 |         - |          NA |
| BucketSort          | 1024 | SingleElementMoved |    12,635.1 ns |    219.39 ns |     97.41 ns |  3.40 |    0.18 |    4 |         - |          NA |
| BucketSortInteger   | 1024 | SingleElementMoved |     7,690.5 ns |    310.63 ns |    137.92 ns |  2.07 |    0.11 |    2 |         - |          NA |
| FlashSort           | 1024 | SingleElementMoved |    19,849.7 ns |    179.86 ns |     94.07 ns |  5.34 |    0.27 |    5 |         - |          NA |
| RadixLSD4Sort       | 1024 | SingleElementMoved |    26,674.7 ns |    286.80 ns |    127.34 ns |  7.18 |    0.37 |    6 |         - |          NA |
| RadixLSD256Sort     | 1024 | SingleElementMoved |     9,358.5 ns |    284.74 ns |    148.93 ns |  2.52 |    0.13 |    3 |         - |          NA |
| RadixLSD10Sort      | 1024 | SingleElementMoved |    20,422.7 ns |     91.23 ns |     32.53 ns |  5.50 |    0.28 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | SingleElementMoved |    32,452.5 ns |    223.78 ns |    117.04 ns |  8.74 |    0.45 |    7 |         - |          NA |
| RadixMSD10Sort      | 1024 | SingleElementMoved |    47,516.2 ns |    246.28 ns |    128.81 ns | 12.79 |    0.65 |    8 |         - |          NA |
| AmericanFlagSort    | 1024 | SingleElementMoved |    11,712.3 ns |    373.72 ns |    195.46 ns |  3.15 |    0.17 |    4 |         - |          NA |
| SpreadSort          | 1024 | SingleElementMoved |     7,541.3 ns |    294.93 ns |    130.95 ns |  2.03 |    0.11 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **Sorted**             |     **5,876.7 ns** |    **507.68 ns** |    **265.52 ns** |  **1.73** |    **0.07** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Sorted             |     3,396.2 ns |      7.97 ns |      2.84 ns |  1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | Sorted             |     5,309.0 ns |    529.29 ns |    276.83 ns |  1.56 |    0.08 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | Sorted             |     3,787.5 ns |    200.05 ns |     88.83 ns |  1.12 |    0.02 |    2 |         - |          NA |
| BucketSort          | 1024 | Sorted             |    12,571.9 ns |    193.15 ns |     85.76 ns |  3.70 |    0.02 |    5 |         - |          NA |
| BucketSortInteger   | 1024 | Sorted             |     7,378.5 ns |    251.43 ns |    131.50 ns |  2.17 |    0.04 |    4 |         - |          NA |
| FlashSort           | 1024 | Sorted             |    18,779.7 ns |    185.05 ns |     82.17 ns |  5.53 |    0.02 |    6 |         - |          NA |
| RadixLSD4Sort       | 1024 | Sorted             |    24,238.6 ns |    628.66 ns |    328.80 ns |  7.14 |    0.09 |    7 |         - |          NA |
| RadixLSD256Sort     | 1024 | Sorted             |     9,812.9 ns |  1,261.41 ns |    659.74 ns |  2.89 |    0.18 |    5 |         - |          NA |
| RadixLSD10Sort      | 1024 | Sorted             |    20,087.6 ns |    253.07 ns |    112.37 ns |  5.91 |    0.03 |    6 |         - |          NA |
| RadixMSD4Sort       | 1024 | Sorted             |    32,045.9 ns |    573.23 ns |    254.52 ns |  9.44 |    0.07 |    8 |         - |          NA |
| RadixMSD10Sort      | 1024 | Sorted             |    47,726.3 ns |    388.78 ns |    203.34 ns | 14.05 |    0.06 |    9 |         - |          NA |
| AmericanFlagSort    | 1024 | Sorted             |    11,156.1 ns |    639.06 ns |    283.75 ns |  3.28 |    0.08 |    5 |         - |          NA |
| SpreadSort          | 1024 | Sorted             |       708.9 ns |     53.68 ns |     23.83 ns |  0.21 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **Reversed**           |     **5,743.0 ns** |      **8.39 ns** |      **2.99 ns** |  **1.67** |    **0.00** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Reversed           |     3,440.4 ns |      7.61 ns |      2.71 ns |  1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 1024 | Reversed           |     5,268.0 ns |    378.91 ns |    198.18 ns |  1.53 |    0.05 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | Reversed           |     3,123.2 ns |      7.80 ns |      3.46 ns |  0.91 |    0.00 |    1 |         - |          NA |
| BucketSort          | 1024 | Reversed           |    80,900.6 ns |    299.53 ns |    156.66 ns | 23.51 |    0.05 |    7 |         - |          NA |
| BucketSortInteger   | 1024 | Reversed           |    17,092.3 ns |    215.70 ns |    112.82 ns |  4.97 |    0.03 |    4 |         - |          NA |
| FlashSort           | 1024 | Reversed           |    16,590.2 ns |    212.38 ns |     94.30 ns |  4.82 |    0.03 |    4 |         - |          NA |
| RadixLSD4Sort       | 1024 | Reversed           |    24,557.5 ns |    555.63 ns |    290.60 ns |  7.14 |    0.08 |    4 |         - |          NA |
| RadixLSD256Sort     | 1024 | Reversed           |     9,272.5 ns |    381.64 ns |    199.60 ns |  2.70 |    0.05 |    3 |         - |          NA |
| RadixLSD10Sort      | 1024 | Reversed           |    20,207.4 ns |    261.85 ns |    136.95 ns |  5.87 |    0.04 |    4 |         - |          NA |
| RadixMSD4Sort       | 1024 | Reversed           |    35,920.0 ns |    359.95 ns |    188.26 ns | 10.44 |    0.05 |    5 |         - |          NA |
| RadixMSD10Sort      | 1024 | Reversed           |    48,973.7 ns |    149.98 ns |     53.48 ns | 14.23 |    0.02 |    6 |         - |          NA |
| AmericanFlagSort    | 1024 | Reversed           |    22,074.8 ns |    683.10 ns |    357.27 ns |  6.42 |    0.10 |    4 |         - |          NA |
| SpreadSort          | 1024 | Reversed           |     5,746.4 ns |    256.41 ns |    134.11 ns |  1.67 |    0.04 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **PipeOrgan**          |     **5,795.9 ns** |    **415.72 ns** |    **217.43 ns** |  **1.71** |    **0.10** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | PipeOrgan          |     3,403.7 ns |    314.45 ns |    164.46 ns |  1.00 |    0.06 |    1 |         - |          NA |
| PigeonSort          | 1024 | PipeOrgan          |     5,113.2 ns |     49.87 ns |     17.78 ns |  1.51 |    0.07 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | PipeOrgan          |     3,444.3 ns |    444.89 ns |    232.68 ns |  1.01 |    0.08 |    1 |         - |          NA |
| BucketSort          | 1024 | PipeOrgan          |    44,978.4 ns |  1,425.65 ns |    633.00 ns | 13.24 |    0.62 |    8 |         - |          NA |
| BucketSortInteger   | 1024 | PipeOrgan          |    12,215.3 ns |    268.77 ns |    140.57 ns |  3.60 |    0.17 |    4 |         - |          NA |
| FlashSort           | 1024 | PipeOrgan          |    17,629.2 ns |    117.38 ns |     52.12 ns |  5.19 |    0.23 |    5 |         - |          NA |
| RadixLSD4Sort       | 1024 | PipeOrgan          |    24,926.2 ns |    263.23 ns |    137.67 ns |  7.34 |    0.33 |    6 |         - |          NA |
| RadixLSD256Sort     | 1024 | PipeOrgan          |     9,690.6 ns |    350.28 ns |    183.20 ns |  2.85 |    0.14 |    3 |         - |          NA |
| RadixLSD10Sort      | 1024 | PipeOrgan          |    20,180.6 ns |    348.95 ns |    182.51 ns |  5.94 |    0.27 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | PipeOrgan          |    35,805.4 ns |    497.49 ns |    260.19 ns | 10.54 |    0.48 |    7 |         - |          NA |
| RadixMSD10Sort      | 1024 | PipeOrgan          |    49,858.9 ns |    164.17 ns |     72.89 ns | 14.68 |    0.66 |    8 |         - |          NA |
| AmericanFlagSort    | 1024 | PipeOrgan          |    18,621.3 ns |    322.76 ns |    168.81 ns |  5.48 |    0.25 |    5 |         - |          NA |
| SpreadSort          | 1024 | PipeOrgan          |     6,816.0 ns |     21.48 ns |      9.54 ns |  2.01 |    0.09 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **Random**             |    **53,878.0 ns** |    **782.30 ns** |    **409.16 ns** |  **1.55** |    **0.02** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Random             |    34,771.4 ns |    775.54 ns |    344.34 ns |  1.00 |    0.01 |    1 |         - |          NA |
| PigeonSort          | 8192 | Random             |    47,528.7 ns |  1,103.66 ns |    577.24 ns |  1.37 |    0.02 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | Random             |    29,695.7 ns |    581.83 ns |    304.31 ns |  0.85 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | Random             |   947,353.5 ns |  2,477.30 ns |  1,295.67 ns | 27.25 |    0.25 |    7 |         - |          NA |
| BucketSortInteger   | 8192 | Random             |   247,698.6 ns |  1,293.76 ns |    676.66 ns |  7.12 |    0.07 |    5 |         - |          NA |
| FlashSort           | 8192 | Random             |   154,455.0 ns |    907.33 ns |    402.86 ns |  4.44 |    0.04 |    4 |         - |          NA |
| RadixLSD4Sort       | 8192 | Random             |   230,718.1 ns |    474.85 ns |    210.84 ns |  6.64 |    0.06 |    5 |         - |          NA |
| RadixLSD256Sort     | 8192 | Random             |    69,740.1 ns |  1,814.60 ns |    949.07 ns |  2.01 |    0.03 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | Random             |   174,058.9 ns |  1,134.93 ns |    593.59 ns |  5.01 |    0.05 |    4 |         - |          NA |
| RadixMSD4Sort       | 8192 | Random             |   381,604.4 ns |  1,752.20 ns |    916.44 ns | 10.98 |    0.10 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | Random             |   420,764.4 ns |  6,580.90 ns |  2,921.96 ns | 12.10 |    0.14 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | Random             |   234,619.9 ns |  1,707.53 ns |    758.15 ns |  6.75 |    0.07 |    5 |         - |          NA |
| SpreadSort          | 8192 | Random             |    82,967.6 ns |    644.75 ns |    286.27 ns |  2.39 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **SingleElementMoved** |    **47,268.9 ns** |    **509.20 ns** |    **226.09 ns** |  **1.63** |    **0.01** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | SingleElementMoved |    28,956.3 ns |    548.38 ns |    243.48 ns |  1.00 |    0.01 |    1 |         - |          NA |
| PigeonSort          | 8192 | SingleElementMoved |    44,692.2 ns |  1,636.28 ns |    855.81 ns |  1.54 |    0.03 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | SingleElementMoved |    28,082.3 ns |  1,885.08 ns |    985.93 ns |  0.97 |    0.03 |    1 |         - |          NA |
| BucketSort          | 8192 | SingleElementMoved |    91,468.2 ns |    590.53 ns |    308.86 ns |  3.16 |    0.03 |    3 |         - |          NA |
| BucketSortInteger   | 8192 | SingleElementMoved |    49,823.2 ns |    631.63 ns |    280.45 ns |  1.72 |    0.02 |    2 |         - |          NA |
| FlashSort           | 8192 | SingleElementMoved |   158,240.3 ns |    876.13 ns |    458.24 ns |  5.47 |    0.05 |    4 |         - |          NA |
| RadixLSD4Sort       | 8192 | SingleElementMoved |   235,005.6 ns |    642.99 ns |    336.30 ns |  8.12 |    0.06 |    5 |         - |          NA |
| RadixLSD256Sort     | 8192 | SingleElementMoved |    40,919.8 ns |    836.99 ns |    371.63 ns |  1.41 |    0.02 |    2 |         - |          NA |
| RadixLSD10Sort      | 8192 | SingleElementMoved |   161,566.0 ns |    430.36 ns |    191.08 ns |  5.58 |    0.04 |    4 |         - |          NA |
| RadixMSD4Sort       | 8192 | SingleElementMoved |   252,619.6 ns |    835.93 ns |    437.21 ns |  8.72 |    0.07 |    5 |         - |          NA |
| RadixMSD10Sort      | 8192 | SingleElementMoved |   379,141.5 ns |  1,536.14 ns |    803.43 ns | 13.09 |    0.11 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | SingleElementMoved |    96,274.3 ns |    575.32 ns |    300.90 ns |  3.33 |    0.03 |    3 |         - |          NA |
| SpreadSort          | 8192 | SingleElementMoved |    50,076.6 ns |  1,020.27 ns |    533.62 ns |  1.73 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **Sorted**             |    **47,051.9 ns** |    **962.45 ns** |    **503.38 ns** |  **1.67** |    **0.03** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Sorted             |    28,240.7 ns |    975.12 ns |    510.01 ns |  1.00 |    0.02 |    2 |         - |          NA |
| PigeonSort          | 8192 | Sorted             |    40,877.7 ns |  1,055.80 ns |    552.20 ns |  1.45 |    0.03 |    3 |         - |          NA |
| PigeonSortInteger   | 8192 | Sorted             |    27,436.5 ns |    566.14 ns |    251.37 ns |  0.97 |    0.02 |    2 |         - |          NA |
| BucketSort          | 8192 | Sorted             |    90,172.9 ns |    591.74 ns |    262.74 ns |  3.19 |    0.06 |    4 |         - |          NA |
| BucketSortInteger   | 8192 | Sorted             |    47,586.2 ns |  1,132.55 ns |    502.86 ns |  1.69 |    0.03 |    3 |         - |          NA |
| FlashSort           | 8192 | Sorted             |   152,871.1 ns |    698.74 ns |    310.24 ns |  5.41 |    0.09 |    5 |         - |          NA |
| RadixLSD4Sort       | 8192 | Sorted             |   233,530.9 ns |  1,440.45 ns |    639.57 ns |  8.27 |    0.14 |    6 |         - |          NA |
| RadixLSD256Sort     | 8192 | Sorted             |    39,016.4 ns |  1,144.47 ns |    508.15 ns |  1.38 |    0.03 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | Sorted             |   166,759.0 ns |  5,142.82 ns |  2,689.79 ns |  5.91 |    0.14 |    5 |         - |          NA |
| RadixMSD4Sort       | 8192 | Sorted             |   249,837.0 ns |  1,660.08 ns |    868.25 ns |  8.85 |    0.15 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | Sorted             |   379,829.6 ns |  1,068.79 ns |    559.00 ns | 13.45 |    0.23 |    7 |         - |          NA |
| AmericanFlagSort    | 8192 | Sorted             |    96,609.5 ns |    419.97 ns |    186.47 ns |  3.42 |    0.06 |    4 |         - |          NA |
| SpreadSort          | 8192 | Sorted             |     5,329.6 ns |    532.31 ns |    236.35 ns |  0.19 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **Reversed**           |    **46,136.7 ns** |  **1,677.85 ns** |    **744.98 ns** |  **1.64** |    **0.03** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Reversed           |    28,218.9 ns |    625.36 ns |    277.66 ns |  1.00 |    0.01 |    1 |         - |          NA |
| PigeonSort          | 8192 | Reversed           |    41,267.0 ns |  2,134.36 ns |    947.67 ns |  1.46 |    0.03 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | Reversed           |    24,997.9 ns |    886.18 ns |    393.47 ns |  0.89 |    0.02 |    1 |         - |          NA |
| BucketSort          | 8192 | Reversed           | 1,718,213.2 ns |  1,134.42 ns |    503.69 ns | 60.89 |    0.56 |    9 |         - |          NA |
| BucketSortInteger   | 8192 | Reversed           |   309,804.0 ns |  1,678.84 ns |    878.07 ns | 10.98 |    0.11 |    7 |         - |          NA |
| FlashSort           | 8192 | Reversed           |   133,288.2 ns |    628.00 ns |    278.84 ns |  4.72 |    0.04 |    4 |         - |          NA |
| RadixLSD4Sort       | 8192 | Reversed           |   236,370.2 ns |    992.89 ns |    440.85 ns |  8.38 |    0.08 |    6 |         - |          NA |
| RadixLSD256Sort     | 8192 | Reversed           |    39,714.6 ns |    813.87 ns |    361.37 ns |  1.41 |    0.02 |    2 |         - |          NA |
| RadixLSD10Sort      | 8192 | Reversed           |   163,893.3 ns |  8,608.04 ns |  4,502.17 ns |  5.81 |    0.16 |    5 |         - |          NA |
| RadixMSD4Sort       | 8192 | Reversed           |   288,331.1 ns |  5,634.03 ns |  2,946.71 ns | 10.22 |    0.14 |    7 |         - |          NA |
| RadixMSD10Sort      | 8192 | Reversed           |   388,501.6 ns |    526.03 ns |    275.12 ns | 13.77 |    0.13 |    8 |         - |          NA |
| AmericanFlagSort    | 8192 | Reversed           |   221,575.6 ns |  3,548.46 ns |  1,855.91 ns |  7.85 |    0.10 |    6 |         - |          NA |
| SpreadSort          | 8192 | Reversed           |    61,000.4 ns |  1,099.91 ns |    488.37 ns |  2.16 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **PipeOrgan**          |    **44,825.2 ns** |  **1,173.68 ns** |    **613.86 ns** |  **1.46** |    **0.03** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | PipeOrgan          |    30,701.4 ns |    969.73 ns |    430.57 ns |  1.00 |    0.02 |    1 |         - |          NA |
| PigeonSort          | 8192 | PipeOrgan          |    42,365.0 ns |    422.13 ns |    187.43 ns |  1.38 |    0.02 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | PipeOrgan          |    27,460.3 ns |  1,419.11 ns |    742.22 ns |  0.89 |    0.03 |    1 |         - |          NA |
| BucketSort          | 8192 | PipeOrgan          |   909,001.5 ns |  7,195.71 ns |  3,763.50 ns | 29.61 |    0.41 |    8 |         - |          NA |
| BucketSortInteger   | 8192 | PipeOrgan          |   191,085.6 ns |    876.51 ns |    389.18 ns |  6.23 |    0.08 |    4 |         - |          NA |
| FlashSort           | 8192 | PipeOrgan          |   138,843.5 ns |    964.19 ns |    504.29 ns |  4.52 |    0.06 |    4 |         - |          NA |
| RadixLSD4Sort       | 8192 | PipeOrgan          |   234,890.0 ns |  1,155.72 ns |    513.15 ns |  7.65 |    0.10 |    5 |         - |          NA |
| RadixLSD256Sort     | 8192 | PipeOrgan          |    73,075.1 ns |  1,367.21 ns |    607.05 ns |  2.38 |    0.04 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | PipeOrgan          |   160,196.2 ns |  8,333.38 ns |  3,700.07 ns |  5.22 |    0.13 |    4 |         - |          NA |
| RadixMSD4Sort       | 8192 | PipeOrgan          |   299,778.8 ns | 51,227.73 ns | 22,745.43 ns |  9.77 |    0.71 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | PipeOrgan          |   393,524.6 ns |  1,083.52 ns |    481.09 ns | 12.82 |    0.17 |    7 |         - |          NA |
| AmericanFlagSort    | 8192 | PipeOrgan          |   171,896.5 ns |  1,821.08 ns |    952.46 ns |  5.60 |    0.08 |    4 |         - |          NA |
| SpreadSort          | 8192 | PipeOrgan          |    81,488.1 ns |  3,062.72 ns |  1,359.86 ns |  2.65 |    0.05 |    3 |         - |          NA |

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
| **BubbleSort**         | **256**  | **Random**             |  **32,433.8 ns** |   **417.15 ns** |   **185.22 ns** |   **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Random             |  16,357.2 ns |   259.24 ns |   135.59 ns |   0.50 |    0.00 |    2 |         - |          NA |
| OddEvenSort        | 256  | Random             |  25,923.4 ns |   497.68 ns |   220.97 ns |   0.80 |    0.01 |    3 |         - |          NA |
| CombSort           | 256  | Random             |   3,721.8 ns |   393.36 ns |   205.73 ns |   0.11 |    0.01 |    1 |         - |          NA |
| CircleSort         | 256  | Random             |  17,634.8 ns |   136.34 ns |    60.53 ns |   0.54 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **SingleElementMoved** |     **406.3 ns** |     **5.81 ns** |     **3.04 ns** |   **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 256  | SingleElementMoved |     308.9 ns |     0.99 ns |     0.52 ns |   0.76 |    0.01 |    1 |         - |          NA |
| OddEvenSort        | 256  | SingleElementMoved |  15,616.2 ns |    81.17 ns |    36.04 ns |  38.44 |    0.28 |    4 |         - |          NA |
| CombSort           | 256  | SingleElementMoved |   2,805.5 ns |     6.45 ns |     2.86 ns |   6.91 |    0.05 |    3 |         - |          NA |
| CircleSort         | 256  | SingleElementMoved |  15,165.9 ns |   157.48 ns |    69.92 ns |  37.33 |    0.31 |    4 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **Sorted**             |     **244.8 ns** |     **1.41 ns** |     **0.63 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Sorted             |     167.5 ns |     1.23 ns |     0.44 ns |   0.68 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 256  | Sorted             |     211.9 ns |     0.80 ns |     0.36 ns |   0.87 |    0.00 |    2 |         - |          NA |
| CombSort           | 256  | Sorted             |   2,480.2 ns |     1.82 ns |     0.81 ns |  10.13 |    0.02 |    3 |         - |          NA |
| CircleSort         | 256  | Sorted             |   2,085.5 ns |     3.45 ns |     1.23 ns |   8.52 |    0.02 |    3 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **Reversed**           |  **27,441.2 ns** |   **392.23 ns** |   **174.15 ns** |   **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Reversed           |  25,090.1 ns |   337.68 ns |   149.93 ns |   0.91 |    0.01 |    3 |         - |          NA |
| OddEvenSort        | 256  | Reversed           |  24,664.7 ns |   364.43 ns |   190.60 ns |   0.90 |    0.01 |    3 |         - |          NA |
| CombSort           | 256  | Reversed           |   3,144.5 ns |    41.53 ns |    14.81 ns |   0.11 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | Reversed           |   4,161.1 ns |    14.40 ns |     5.14 ns |   0.15 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **PipeOrgan**          |  **26,146.2 ns** |   **201.42 ns** |   **105.34 ns** |   **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | PipeOrgan          |  17,058.6 ns |   263.76 ns |   117.11 ns |   0.65 |    0.00 |    2 |         - |          NA |
| OddEvenSort        | 256  | PipeOrgan          |  25,579.7 ns |   179.01 ns |    93.63 ns |   0.98 |    0.01 |    3 |         - |          NA |
| CombSort           | 256  | PipeOrgan          |   3,023.3 ns |    26.81 ns |    11.90 ns |   0.12 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | PipeOrgan          |  19,233.1 ns |   120.44 ns |    53.47 ns |   0.74 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Random**             | **601,114.2 ns** | **3,608.07 ns** | **1,602.00 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Random             | 312,309.2 ns | 1,106.41 ns |   578.67 ns |   0.52 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | Random             | 529,493.2 ns | 2,158.92 ns |   958.57 ns |   0.88 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | Random             |  31,810.7 ns |   498.48 ns |   260.72 ns |   0.05 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | Random             |  97,592.9 ns | 1,501.85 ns |   785.50 ns |   0.16 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **SingleElementMoved** |   **1,533.1 ns** |     **2.93 ns** |     **1.04 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | SingleElementMoved |   1,147.1 ns |     5.41 ns |     1.93 ns |   0.75 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 1024 | SingleElementMoved | 223,065.7 ns | 2,199.97 ns |   976.80 ns | 145.50 |    0.60 |    5 |         - |          NA |
| CombSort           | 1024 | SingleElementMoved |  15,078.0 ns |   379.13 ns |   168.34 ns |   9.84 |    0.10 |    3 |         - |          NA |
| CircleSort         | 1024 | SingleElementMoved |  83,695.8 ns |   397.30 ns |   176.40 ns |  54.59 |    0.11 |    4 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Sorted**             |     **956.0 ns** |     **2.32 ns** |     **1.03 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Sorted             |     642.9 ns |     1.31 ns |     0.58 ns |   0.67 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 1024 | Sorted             |     808.0 ns |    11.93 ns |     5.30 ns |   0.85 |    0.01 |    2 |         - |          NA |
| CombSort           | 1024 | Sorted             |  12,941.2 ns |   249.05 ns |   130.26 ns |  13.54 |    0.13 |    4 |         - |          NA |
| CircleSort         | 1024 | Sorted             |   9,260.8 ns |   335.29 ns |   175.36 ns |   9.69 |    0.17 |    3 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Reversed**           | **414,436.5 ns** |   **999.92 ns** |   **443.97 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Reversed           | 386,031.6 ns |   579.79 ns |   257.43 ns |   0.93 |    0.00 |    2 |         - |          NA |
| OddEvenSort        | 1024 | Reversed           | 367,863.1 ns | 1,355.35 ns |   601.79 ns |   0.89 |    0.00 |    2 |         - |          NA |
| CombSort           | 1024 | Reversed           |  16,724.6 ns |   230.29 ns |   120.45 ns |   0.04 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | Reversed           |  18,577.2 ns |   294.50 ns |   130.76 ns |   0.04 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **PipeOrgan**          | **383,650.0 ns** | **1,298.24 ns** |   **679.00 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | PipeOrgan          | 252,881.9 ns |   455.21 ns |   202.11 ns |   0.66 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | PipeOrgan          | 359,655.1 ns | 1,045.15 ns |   464.05 ns |   0.94 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | PipeOrgan          |  16,775.2 ns |   174.87 ns |    91.46 ns |   0.04 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | PipeOrgan          | 103,162.8 ns |   849.04 ns |   444.07 ns |   0.27 |    0.00 |    2 |         - |          NA |

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
| **HeapSort**         | **256**  | **Random**             |     **3,974.3 ns** |    **376.31 ns** |    **196.82 ns** |  **1.00** |    **0.07** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | Random             |     3,508.4 ns |    326.38 ns |    170.70 ns |  0.88 |    0.06 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | Random             |     4,184.4 ns |    513.15 ns |    268.39 ns |  1.06 |    0.08 |    1 |         - |          NA |
| BottomupHeapSort | 256  | Random             |     4,036.7 ns |    252.08 ns |    111.93 ns |  1.02 |    0.05 |    1 |         - |          NA |
| WeakHeapSort     | 256  | Random             |     9,130.3 ns |    389.57 ns |    203.76 ns |  2.30 |    0.12 |    4 |         - |          NA |
| SmoothSort       | 256  | Random             |     5,211.8 ns |    290.64 ns |    152.01 ns |  1.31 |    0.07 |    2 |         - |          NA |
| TournamentSort   | 256  | Random             |     7,374.5 ns |    403.62 ns |    211.10 ns |  1.86 |    0.10 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **SingleElementMoved** |     **4,116.0 ns** |    **425.04 ns** |    **222.31 ns** |  **1.00** |    **0.07** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 256  | SingleElementMoved |     3,470.0 ns |    229.75 ns |    120.16 ns |  0.85 |    0.05 |    2 |         - |          NA |
| TernaryHeapSort  | 256  | SingleElementMoved |     4,158.7 ns |    237.91 ns |    105.63 ns |  1.01 |    0.06 |    2 |         - |          NA |
| BottomupHeapSort | 256  | SingleElementMoved |     4,080.5 ns |    239.92 ns |    106.53 ns |  0.99 |    0.06 |    2 |         - |          NA |
| WeakHeapSort     | 256  | SingleElementMoved |     7,840.7 ns |    289.95 ns |    151.65 ns |  1.91 |    0.10 |    4 |         - |          NA |
| SmoothSort       | 256  | SingleElementMoved |     1,741.2 ns |     26.94 ns |     11.96 ns |  0.42 |    0.02 |    1 |         - |          NA |
| TournamentSort   | 256  | SingleElementMoved |     5,477.2 ns |    831.39 ns |    434.84 ns |  1.33 |    0.12 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **Sorted**             |     **3,817.6 ns** |    **122.92 ns** |     **43.83 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 256  | Sorted             |     3,424.1 ns |     76.64 ns |     27.33 ns |  0.90 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 256  | Sorted             |     4,096.8 ns |    117.33 ns |     52.10 ns |  1.07 |    0.02 |    2 |         - |          NA |
| BottomupHeapSort | 256  | Sorted             |     4,034.0 ns |    102.69 ns |     45.60 ns |  1.06 |    0.02 |    2 |         - |          NA |
| WeakHeapSort     | 256  | Sorted             |     7,795.4 ns |     69.23 ns |     36.21 ns |  2.04 |    0.02 |    3 |         - |          NA |
| SmoothSort       | 256  | Sorted             |     1,254.9 ns |     40.05 ns |     14.28 ns |  0.33 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 256  | Sorted             |     3,167.3 ns |     51.67 ns |     22.94 ns |  0.83 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **Reversed**           |     **3,833.3 ns** |     **43.00 ns** |     **15.33 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | Reversed           |     3,313.5 ns |     69.86 ns |     24.91 ns |  0.86 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | Reversed           |     4,039.7 ns |     97.94 ns |     43.49 ns |  1.05 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 256  | Reversed           |     4,202.1 ns |     60.31 ns |     21.51 ns |  1.10 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 256  | Reversed           |     8,760.2 ns |    284.35 ns |    148.72 ns |  2.29 |    0.04 |    2 |         - |          NA |
| SmoothSort       | 256  | Reversed           |     4,706.1 ns |    359.54 ns |    159.64 ns |  1.23 |    0.04 |    1 |         - |          NA |
| TournamentSort   | 256  | Reversed           |     4,899.0 ns |    444.93 ns |    197.55 ns |  1.28 |    0.05 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **PipeOrgan**          |     **3,077.5 ns** |    **316.81 ns** |    **140.67 ns** |  **1.00** |    **0.06** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | PipeOrgan          |     3,162.3 ns |    287.79 ns |    150.52 ns |  1.03 |    0.06 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | PipeOrgan          |     3,756.6 ns |    108.94 ns |     48.37 ns |  1.22 |    0.05 |    2 |         - |          NA |
| BottomupHeapSort | 256  | PipeOrgan          |     4,162.4 ns |    364.29 ns |    190.53 ns |  1.35 |    0.08 |    2 |         - |          NA |
| WeakHeapSort     | 256  | PipeOrgan          |     8,355.7 ns |    317.27 ns |    165.94 ns |  2.72 |    0.12 |    4 |         - |          NA |
| SmoothSort       | 256  | PipeOrgan          |     5,535.7 ns |    361.60 ns |    189.12 ns |  1.80 |    0.09 |    3 |         - |          NA |
| TournamentSort   | 256  | PipeOrgan          |     8,241.6 ns |  1,505.76 ns |    787.54 ns |  2.68 |    0.26 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Random**             |    **19,684.4 ns** |    **458.68 ns** |    **239.90 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Random             |    17,872.9 ns |    410.66 ns |    214.78 ns |  0.91 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | Random             |    20,429.3 ns |    300.70 ns |    133.51 ns |  1.04 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | Random             |    19,330.1 ns |    331.49 ns |    173.37 ns |  0.98 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | Random             |    47,971.6 ns |    156.85 ns |     69.64 ns |  2.44 |    0.03 |    3 |         - |          NA |
| SmoothSort       | 1024 | Random             |    27,004.0 ns |    960.21 ns |    502.21 ns |  1.37 |    0.03 |    2 |         - |          NA |
| TournamentSort   | 1024 | Random             |    66,165.5 ns | 23,270.76 ns | 12,171.06 ns |  3.36 |    0.58 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **SingleElementMoved** |    **22,075.0 ns** |  **1,525.55 ns** |    **797.89 ns** |  **1.00** |    **0.05** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | SingleElementMoved |    17,052.3 ns |    224.52 ns |     99.69 ns |  0.77 |    0.03 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | SingleElementMoved |    20,737.5 ns |    648.75 ns |    288.05 ns |  0.94 |    0.03 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | SingleElementMoved |    19,183.2 ns |    333.20 ns |    174.27 ns |  0.87 |    0.03 |    2 |         - |          NA |
| WeakHeapSort     | 1024 | SingleElementMoved |    40,599.1 ns |    398.09 ns |    176.75 ns |  1.84 |    0.06 |    3 |         - |          NA |
| SmoothSort       | 1024 | SingleElementMoved |     7,160.8 ns |     86.70 ns |     30.92 ns |  0.32 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 1024 | SingleElementMoved |    26,611.3 ns |  4,872.26 ns |  2,548.29 ns |  1.21 |    0.12 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Sorted**             |    **21,879.6 ns** |    **912.39 ns** |    **405.11 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Sorted             |    16,863.7 ns |    174.24 ns |     77.36 ns |  0.77 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | Sorted             |    22,788.9 ns |  1,408.32 ns |    625.30 ns |  1.04 |    0.03 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | Sorted             |    19,089.0 ns |    228.25 ns |    101.35 ns |  0.87 |    0.02 |    2 |         - |          NA |
| WeakHeapSort     | 1024 | Sorted             |    41,009.2 ns |    432.87 ns |    192.20 ns |  1.87 |    0.03 |    3 |         - |          NA |
| SmoothSort       | 1024 | Sorted             |     5,149.3 ns |    332.95 ns |    174.14 ns |  0.24 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 1024 | Sorted             |    15,137.0 ns |    326.33 ns |    144.89 ns |  0.69 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Reversed**           |    **19,068.0 ns** |    **421.95 ns** |    **220.69 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Reversed           |    18,372.6 ns |    960.31 ns |    502.26 ns |  0.96 |    0.03 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | Reversed           |    19,453.5 ns |    502.55 ns |    223.13 ns |  1.02 |    0.02 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | Reversed           |    19,733.4 ns |    221.08 ns |     98.16 ns |  1.04 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | Reversed           |    44,546.3 ns |    398.89 ns |    177.11 ns |  2.34 |    0.03 |    2 |         - |          NA |
| SmoothSort       | 1024 | Reversed           |    22,991.2 ns |    378.94 ns |    198.19 ns |  1.21 |    0.02 |    1 |         - |          NA |
| TournamentSort   | 1024 | Reversed           |    24,925.5 ns |    719.13 ns |    376.12 ns |  1.31 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **PipeOrgan**          |    **16,118.8 ns** |    **246.49 ns** |    **109.45 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | PipeOrgan          |    16,245.1 ns |    311.66 ns |    138.38 ns |  1.01 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | PipeOrgan          |    18,266.0 ns |    250.80 ns |    111.36 ns |  1.13 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | PipeOrgan          |    19,476.4 ns |    382.53 ns |    200.07 ns |  1.21 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | PipeOrgan          |    43,803.4 ns |    434.80 ns |    227.41 ns |  2.72 |    0.02 |    3 |         - |          NA |
| SmoothSort       | 1024 | PipeOrgan          |    27,554.6 ns |    409.39 ns |    181.77 ns |  1.71 |    0.02 |    2 |         - |          NA |
| TournamentSort   | 1024 | PipeOrgan          |    42,998.2 ns |  7,880.47 ns |  4,121.64 ns |  2.67 |    0.24 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Random**             |   **498,540.9 ns** |    **826.39 ns** |    **432.22 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Random             |   485,730.1 ns |  1,551.26 ns |    811.34 ns |  0.97 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | Random             |   582,716.3 ns |    728.80 ns |    323.59 ns |  1.17 |    0.00 |    1 |         - |          NA |
| BottomupHeapSort | 8192 | Random             |   579,908.6 ns |    749.77 ns |    332.90 ns |  1.16 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | Random             |   886,183.8 ns |  1,543.51 ns |    807.29 ns |  1.78 |    0.00 |    2 |         - |          NA |
| SmoothSort       | 8192 | Random             |   816,409.8 ns |  2,644.01 ns |  1,382.87 ns |  1.64 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 8192 | Random             | 1,347,142.8 ns |  2,190.45 ns |    972.57 ns |  2.70 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **SingleElementMoved** |   **370,208.0 ns** |  **2,498.55 ns** |  **1,306.79 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 8192 | SingleElementMoved |   378,552.5 ns |    754.58 ns |    335.04 ns |  1.02 |    0.00 |    2 |         - |          NA |
| TernaryHeapSort  | 8192 | SingleElementMoved |   413,145.6 ns |    459.03 ns |    203.81 ns |  1.12 |    0.00 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | SingleElementMoved |   446,442.9 ns |  2,670.42 ns |  1,185.68 ns |  1.21 |    0.01 |    2 |         - |          NA |
| WeakHeapSort     | 8192 | SingleElementMoved |   432,291.2 ns |    961.34 ns |    502.80 ns |  1.17 |    0.00 |    2 |         - |          NA |
| SmoothSort       | 8192 | SingleElementMoved |    58,758.5 ns |  1,280.39 ns |    669.67 ns |  0.16 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 8192 | SingleElementMoved |   772,994.2 ns |  2,307.23 ns |  1,206.73 ns |  2.09 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Sorted**             |   **368,320.2 ns** |  **1,292.62 ns** |    **573.93 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Sorted             |   378,206.3 ns |  1,445.24 ns |    641.69 ns |  1.03 |    0.00 |    2 |         - |          NA |
| TernaryHeapSort  | 8192 | Sorted             |   413,852.6 ns |    950.18 ns |    421.89 ns |  1.12 |    0.00 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | Sorted             |   447,543.7 ns |  1,330.82 ns |    696.05 ns |  1.22 |    0.00 |    2 |         - |          NA |
| WeakHeapSort     | 8192 | Sorted             |   440,456.6 ns |  1,253.64 ns |    655.68 ns |  1.20 |    0.00 |    2 |         - |          NA |
| SmoothSort       | 8192 | Sorted             |    41,354.2 ns |    960.50 ns |    426.47 ns |  0.11 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 8192 | Sorted             |   513,142.9 ns | 11,760.32 ns |  6,150.87 ns |  1.39 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Reversed**           |   **396,343.0 ns** |  **1,450.92 ns** |    **644.22 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Reversed           |   351,596.9 ns |    790.81 ns |    413.61 ns |  0.89 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | Reversed           |   424,845.5 ns |    914.01 ns |    478.04 ns |  1.07 |    0.00 |    1 |         - |          NA |
| BottomupHeapSort | 8192 | Reversed           |   479,673.0 ns |  1,303.88 ns |    681.95 ns |  1.21 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | Reversed           |   471,696.9 ns |  2,526.33 ns |  1,121.70 ns |  1.19 |    0.00 |    1 |         - |          NA |
| SmoothSort       | 8192 | Reversed           |   507,553.6 ns |  3,526.14 ns |  1,844.24 ns |  1.28 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 8192 | Reversed           |   728,678.6 ns |  2,253.87 ns |  1,178.82 ns |  1.84 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **PipeOrgan**          |   **367,014.2 ns** |    **605.72 ns** |    **268.94 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | PipeOrgan          |   380,120.8 ns |  1,589.44 ns |    831.31 ns |  1.04 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | PipeOrgan          |   454,143.0 ns |  1,556.85 ns |    814.26 ns |  1.24 |    0.00 |    1 |         - |          NA |
| BottomupHeapSort | 8192 | PipeOrgan          |   454,022.6 ns |  1,189.29 ns |    622.02 ns |  1.24 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | PipeOrgan          |   503,756.5 ns |  2,233.87 ns |  1,168.36 ns |  1.37 |    0.00 |    1 |         - |          NA |
| SmoothSort       | 8192 | PipeOrgan          |   654,823.0 ns |  1,170.76 ns |    612.33 ns |  1.78 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 8192 | PipeOrgan          | 1,063,882.1 ns |  3,247.95 ns |  1,158.25 ns |  2.90 |    0.00 |    3 |         - |          NA |

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

| Method                 | Size | Pattern            | Mean         | Error       | StdDev      | Ratio  | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------- |----- |------------------- |-------------:|------------:|------------:|-------:|--------:|-----:|----------:|------------:|
| **InsertionSort**          | **256**  | **Random**             |   **8,480.6 ns** |   **450.38 ns** |   **199.97 ns** |   **1.00** |    **0.03** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Random             |   8,919.3 ns |   441.55 ns |   230.94 ns |   1.05 |    0.03 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | Random             |   9,432.0 ns |   490.44 ns |   256.51 ns |   1.11 |    0.04 |    2 |         - |          NA |
| GnomeSort              | 256  | Random             |  27,878.9 ns |   367.88 ns |   163.34 ns |   3.29 |    0.07 |    4 |         - |          NA |
| LibrarySort            | 256  | Random             |  16,406.6 ns |   221.30 ns |   115.74 ns |   1.94 |    0.04 |    3 |         - |          NA |
| MergeInsertionSort     | 256  | Random             |  34,374.3 ns | 1,082.51 ns |   566.17 ns |   4.06 |    0.11 |    5 |         - |          NA |
| ShellSortKnuth1973     | 256  | Random             |   2,977.9 ns |    60.83 ns |    21.69 ns |   0.35 |    0.01 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Random             |   3,026.0 ns |   231.55 ns |   121.10 ns |   0.36 |    0.02 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | Random             |   3,162.1 ns |    56.71 ns |    20.22 ns |   0.37 |    0.01 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | Random             |   2,980.3 ns |    57.43 ns |    20.48 ns |   0.35 |    0.01 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Random             |   3,070.9 ns |    40.33 ns |    17.91 ns |   0.36 |    0.01 |    1 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **256**  | **SingleElementMoved** |     **424.3 ns** |     **4.91 ns** |     **2.18 ns** |   **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 256  | SingleElementMoved |     291.9 ns |    10.28 ns |     4.57 ns |   0.69 |    0.01 |    1 |         - |          NA |
| BinaryInsertSort       | 256  | SingleElementMoved |     973.4 ns |     9.14 ns |     4.06 ns |   2.29 |    0.01 |    3 |         - |          NA |
| GnomeSort              | 256  | SingleElementMoved |     463.8 ns |     2.78 ns |     1.46 ns |   1.09 |    0.01 |    2 |         - |          NA |
| LibrarySort            | 256  | SingleElementMoved |   8,246.0 ns |    15.30 ns |     5.46 ns |  19.43 |    0.09 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | SingleElementMoved |  25,375.6 ns |   424.53 ns |   188.49 ns |  59.80 |    0.50 |    6 |         - |          NA |
| ShellSortKnuth1973     | 256  | SingleElementMoved |   1,285.8 ns |     5.66 ns |     2.96 ns |   3.03 |    0.02 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 256  | SingleElementMoved |   1,290.2 ns |     4.82 ns |     2.52 ns |   3.04 |    0.02 |    4 |         - |          NA |
| ShellSortTokuda1992    | 256  | SingleElementMoved |   1,599.0 ns |    18.62 ns |     6.64 ns |   3.77 |    0.02 |    4 |         - |          NA |
| ShellSortCiura2001     | 256  | SingleElementMoved |   1,397.5 ns |    26.33 ns |    11.69 ns |   3.29 |    0.03 |    4 |         - |          NA |
| ShellSortLee2021       | 256  | SingleElementMoved |   1,566.0 ns |    12.85 ns |     5.71 ns |   3.69 |    0.02 |    4 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **256**  | **Sorted**             |     **323.5 ns** |     **1.80 ns** |     **0.80 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Sorted             |     211.8 ns |     1.08 ns |     0.56 ns |   0.65 |    0.00 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | Sorted             |     173.6 ns |     0.88 ns |     0.46 ns |   0.54 |    0.00 |    1 |         - |          NA |
| GnomeSort              | 256  | Sorted             |     246.3 ns |     1.12 ns |     0.50 ns |   0.76 |    0.00 |    2 |         - |          NA |
| LibrarySort            | 256  | Sorted             |   6,782.0 ns |    19.65 ns |     8.73 ns |  20.96 |    0.05 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | Sorted             |  24,679.5 ns |   394.51 ns |   206.33 ns |  76.29 |    0.63 |    6 |         - |          NA |
| ShellSortKnuth1973     | 256  | Sorted             |   1,066.1 ns |     1.61 ns |     0.71 ns |   3.30 |    0.01 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Sorted             |   1,069.4 ns |     2.60 ns |     0.93 ns |   3.31 |    0.01 |    4 |         - |          NA |
| ShellSortTokuda1992    | 256  | Sorted             |   1,306.0 ns |     1.80 ns |     0.80 ns |   4.04 |    0.01 |    4 |         - |          NA |
| ShellSortCiura2001     | 256  | Sorted             |   1,144.5 ns |     0.52 ns |     0.18 ns |   3.54 |    0.01 |    4 |         - |          NA |
| ShellSortLee2021       | 256  | Sorted             |   1,630.2 ns | 1,533.84 ns |   681.03 ns |   5.04 |    1.97 |    4 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **256**  | **Reversed**           |  **16,505.3 ns** |   **111.11 ns** |    **49.33 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Reversed           |  18,890.9 ns |   466.82 ns |   207.27 ns |   1.14 |    0.01 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | Reversed           |  16,805.9 ns |   292.31 ns |   152.88 ns |   1.02 |    0.01 |    2 |         - |          NA |
| GnomeSort              | 256  | Reversed           |  58,106.2 ns |   403.34 ns |   179.09 ns |   3.52 |    0.01 |    5 |         - |          NA |
| LibrarySort            | 256  | Reversed           |  38,582.4 ns |   594.25 ns |   211.92 ns |   2.34 |    0.01 |    4 |         - |          NA |
| MergeInsertionSort     | 256  | Reversed           |  24,926.2 ns |   476.16 ns |   249.04 ns |   1.51 |    0.01 |    3 |         - |          NA |
| ShellSortKnuth1973     | 256  | Reversed           |   1,781.5 ns |     6.47 ns |     3.39 ns |   0.11 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Reversed           |   1,775.7 ns |    30.03 ns |    13.33 ns |   0.11 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | Reversed           |   1,895.2 ns |     6.29 ns |     3.29 ns |   0.11 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | Reversed           |   1,795.5 ns |    23.05 ns |     8.22 ns |   0.11 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Reversed           |   1,873.7 ns |     8.35 ns |     3.71 ns |   0.11 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **256**  | **PipeOrgan**          |   **8,874.0 ns** | **1,275.59 ns** |   **566.37 ns** |   **1.00** |    **0.08** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 256  | PipeOrgan          |   9,739.4 ns |   516.62 ns |   229.38 ns |   1.10 |    0.06 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | PipeOrgan          |  10,089.3 ns |   447.10 ns |   233.84 ns |   1.14 |    0.07 |    2 |         - |          NA |
| GnomeSort              | 256  | PipeOrgan          |  23,728.7 ns |   325.62 ns |   170.31 ns |   2.68 |    0.15 |    3 |         - |          NA |
| LibrarySort            | 256  | PipeOrgan          |  24,546.3 ns |   280.18 ns |   146.54 ns |   2.77 |    0.15 |    3 |         - |          NA |
| MergeInsertionSort     | 256  | PipeOrgan          |  22,821.7 ns |   156.65 ns |    69.55 ns |   2.58 |    0.14 |    3 |         - |          NA |
| ShellSortKnuth1973     | 256  | PipeOrgan          |   1,546.5 ns |    14.04 ns |     6.23 ns |   0.17 |    0.01 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | PipeOrgan          |   1,770.1 ns |   280.50 ns |   146.71 ns |   0.20 |    0.02 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | PipeOrgan          |   1,744.5 ns |    26.07 ns |     9.30 ns |   0.20 |    0.01 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | PipeOrgan          |   1,877.3 ns |    58.05 ns |    20.70 ns |   0.21 |    0.01 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | PipeOrgan          |   1,777.6 ns |    12.43 ns |     5.52 ns |   0.20 |    0.01 |    1 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **1024** | **Random**             | **127,726.5 ns** |   **587.41 ns** |   **260.81 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Random             | 136,849.5 ns | 3,494.72 ns | 1,827.81 ns |   1.07 |    0.01 |    3 |         - |          NA |
| BinaryInsertSort       | 1024 | Random             | 147,466.2 ns | 1,165.19 ns |   609.42 ns |   1.15 |    0.01 |    3 |         - |          NA |
| GnomeSort              | 1024 | Random             | 424,165.2 ns | 1,498.13 ns |   665.18 ns |   3.32 |    0.01 |    4 |         - |          NA |
| LibrarySort            | 1024 | Random             |  83,898.2 ns | 3,731.68 ns | 1,951.74 ns |   0.66 |    0.01 |    2 |         - |          NA |
| MergeInsertionSort     | 1024 | Random             | 419,058.8 ns | 2,859.55 ns | 1,495.60 ns |   3.28 |    0.01 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Random             |  16,540.0 ns |   266.65 ns |   118.40 ns |   0.13 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Random             |  16,701.2 ns |   364.39 ns |   190.58 ns |   0.13 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Random             |  17,260.2 ns |   154.06 ns |    68.40 ns |   0.14 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Random             |  17,099.8 ns |   329.68 ns |   172.43 ns |   0.13 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Random             |  17,175.7 ns |   419.09 ns |   219.19 ns |   0.13 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **1024** | **SingleElementMoved** |   **1,629.7 ns** |     **6.98 ns** |     **3.10 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | SingleElementMoved |   1,075.9 ns |     2.92 ns |     1.30 ns |   0.66 |    0.00 |    1 |         - |          NA |
| BinaryInsertSort       | 1024 | SingleElementMoved |   4,707.7 ns |    17.60 ns |     6.28 ns |   2.89 |    0.01 |    3 |         - |          NA |
| GnomeSort              | 1024 | SingleElementMoved |   1,807.0 ns |     1.82 ns |     0.81 ns |   1.11 |    0.00 |    2 |         - |          NA |
| LibrarySort            | 1024 | SingleElementMoved |  37,305.4 ns |   549.00 ns |   287.14 ns |  22.89 |    0.17 |    5 |         - |          NA |
| MergeInsertionSort     | 1024 | SingleElementMoved | 289,583.5 ns | 1,228.37 ns |   642.46 ns | 177.69 |    0.49 |    6 |         - |          NA |
| ShellSortKnuth1973     | 1024 | SingleElementMoved |   6,281.6 ns |   411.13 ns |   215.03 ns |   3.85 |    0.12 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | SingleElementMoved |   6,998.1 ns |   338.28 ns |   176.93 ns |   4.29 |    0.10 |    4 |         - |          NA |
| ShellSortTokuda1992    | 1024 | SingleElementMoved |   7,649.9 ns |   162.79 ns |    85.14 ns |   4.69 |    0.05 |    4 |         - |          NA |
| ShellSortCiura2001     | 1024 | SingleElementMoved |   7,380.8 ns |   581.36 ns |   258.13 ns |   4.53 |    0.15 |    4 |         - |          NA |
| ShellSortLee2021       | 1024 | SingleElementMoved |   7,253.2 ns |    27.44 ns |    12.18 ns |   4.45 |    0.01 |    4 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **1024** | **Sorted**             |   **1,274.2 ns** |    **15.10 ns** |     **5.38 ns** |   **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Sorted             |   1,111.1 ns |   318.03 ns |   141.21 ns |   0.87 |    0.10 |    3 |         - |          NA |
| BinaryInsertSort       | 1024 | Sorted             |     647.9 ns |     1.25 ns |     0.55 ns |   0.51 |    0.00 |    1 |         - |          NA |
| GnomeSort              | 1024 | Sorted             |     954.9 ns |     1.70 ns |     0.61 ns |   0.75 |    0.00 |    2 |         - |          NA |
| LibrarySort            | 1024 | Sorted             |  31,059.8 ns |   259.09 ns |   115.04 ns |  24.38 |    0.13 |    5 |         - |          NA |
| MergeInsertionSort     | 1024 | Sorted             | 288,278.3 ns | 1,415.34 ns |   740.25 ns | 226.25 |    1.03 |    6 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Sorted             |   5,354.2 ns |   298.72 ns |   156.24 ns |   4.20 |    0.12 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Sorted             |   6,165.4 ns |   410.87 ns |   214.89 ns |   4.84 |    0.16 |    4 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Sorted             |   6,306.5 ns |     2.67 ns |     1.19 ns |   4.95 |    0.02 |    4 |         - |          NA |
| ShellSortCiura2001     | 1024 | Sorted             |   6,206.7 ns |    12.18 ns |     5.41 ns |   4.87 |    0.02 |    4 |         - |          NA |
| ShellSortLee2021       | 1024 | Sorted             |   6,322.0 ns |     9.81 ns |     4.35 ns |   4.96 |    0.02 |    4 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **1024** | **Reversed**           | **250,153.3 ns** | **1,340.14 ns** |   **700.92 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Reversed           | 280,627.3 ns |   301.69 ns |   133.95 ns |   1.12 |    0.00 |    2 |         - |          NA |
| BinaryInsertSort       | 1024 | Reversed           | 234,748.5 ns | 1,149.84 ns |   510.54 ns |   0.94 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | Reversed           | 845,719.0 ns | 1,997.18 ns | 1,044.57 ns |   3.38 |    0.01 |    4 |         - |          NA |
| LibrarySort            | 1024 | Reversed           | 416,120.4 ns | 1,593.58 ns |   833.47 ns |   1.66 |    0.01 |    3 |         - |          NA |
| MergeInsertionSort     | 1024 | Reversed           | 287,712.3 ns | 1,833.38 ns |   958.89 ns |   1.15 |    0.00 |    2 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Reversed           |   8,952.4 ns |   833.03 ns |   435.69 ns |   0.04 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Reversed           |   8,886.3 ns |   391.27 ns |   204.64 ns |   0.04 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Reversed           |  10,051.1 ns |   603.91 ns |   315.86 ns |   0.04 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Reversed           |   9,454.0 ns |   540.51 ns |   282.70 ns |   0.04 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Reversed           |   9,864.0 ns |   306.07 ns |   160.08 ns |   0.04 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **1024** | **PipeOrgan**          | **129,152.8 ns** | **4,231.14 ns** | **2,212.97 ns** |   **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | PipeOrgan          | 144,583.5 ns | 4,765.41 ns | 2,492.40 ns |   1.12 |    0.03 |    2 |         - |          NA |
| BinaryInsertSort       | 1024 | PipeOrgan          | 130,235.7 ns |   239.09 ns |   106.16 ns |   1.01 |    0.02 |    2 |         - |          NA |
| GnomeSort              | 1024 | PipeOrgan          | 340,576.4 ns |   909.54 ns |   403.84 ns |   2.64 |    0.04 |    4 |         - |          NA |
| LibrarySort            | 1024 | PipeOrgan          | 272,380.6 ns | 1,247.42 ns |   652.43 ns |   2.11 |    0.03 |    3 |         - |          NA |
| MergeInsertionSort     | 1024 | PipeOrgan          | 252,987.0 ns | 9,133.18 ns | 4,776.83 ns |   1.96 |    0.05 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | PipeOrgan          |   8,197.8 ns |   363.68 ns |   190.21 ns |   0.06 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | PipeOrgan          |   8,588.1 ns |   400.86 ns |   209.66 ns |   0.07 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | PipeOrgan          |   9,125.4 ns |   366.52 ns |   191.70 ns |   0.07 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | PipeOrgan          |   9,044.2 ns |   346.70 ns |   181.33 ns |   0.07 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | PipeOrgan          |   9,310.4 ns |   387.94 ns |   202.90 ns |   0.07 |    0.00 |    1 |         - |          NA |

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

| Method             | Size | Pattern            | Mean           | Error        | StdDev       | Median         | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------- |----- |------------------- |---------------:|-------------:|-------------:|---------------:|------:|--------:|-----:|----------:|------------:|
| **QuickSort**          | **256**  | **Random**             |     **3,089.6 ns** |    **296.49 ns** |    **155.07 ns** |     **2,986.1 ns** |  **1.00** |    **0.07** |    **1** |         **-** |          **NA** |
| QuickSort3way      | 256  | Random             |     3,268.3 ns |    347.63 ns |    181.82 ns |     3,183.7 ns |  1.06 |    0.07 |    1 |         - |          NA |
| QuickSortMedian3   | 256  | Random             |     4,659.7 ns |    684.69 ns |    358.11 ns |     4,609.7 ns |  1.51 |    0.13 |    2 |         - |          NA |
| QuickSortMedian9   | 256  | Random             |     3,754.6 ns |     52.84 ns |     18.84 ns |     3,749.6 ns |  1.22 |    0.06 |    1 |         - |          NA |
| DualPivotQuickSort | 256  | Random             |     2,594.2 ns |     65.09 ns |     23.21 ns |     2,586.6 ns |  0.84 |    0.04 |    1 |         - |          NA |
| StableQuickSort    | 256  | Random             |    11,241.7 ns |    419.20 ns |    186.13 ns |    11,261.2 ns |  3.65 |    0.18 |    3 |         - |          NA |
| IntroSort          | 256  | Random             |     2,234.4 ns |    180.09 ns |     79.96 ns |     2,202.9 ns |  0.72 |    0.04 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | Random             |     1,896.3 ns |     85.01 ns |     30.31 ns |     1,891.6 ns |  0.62 |    0.03 |    1 |         - |          NA |
| PDQSort            | 256  | Random             |     1,898.4 ns |     85.95 ns |     38.16 ns |     1,885.0 ns |  0.62 |    0.03 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | Random             |     3,380.7 ns |     94.74 ns |     42.07 ns |     3,375.4 ns |  1.10 |    0.05 |    1 |         - |          NA |
| Ipnsort            | 256  | Random             |     5,210.6 ns |    415.38 ns |    217.25 ns |     5,117.4 ns |  1.69 |    0.10 |    2 |         - |          NA |
| StdSort            | 256  | Random             |     3,225.9 ns |     59.95 ns |     31.35 ns |     3,211.2 ns |  1.05 |    0.05 |    1 |         - |          NA |
| BlockQuickSort     | 256  | Random             |     2,890.5 ns |    249.30 ns |    110.69 ns |     2,851.4 ns |  0.94 |    0.05 |    1 |         - |          NA |
| DotnetSort         | 256  | Random             |     2,192.7 ns |    403.81 ns |    211.20 ns |     2,053.6 ns |  0.71 |    0.07 |    1 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **QuickSort**          | **256**  | **SingleElementMoved** |     **1,567.2 ns** |     **19.45 ns** |      **8.64 ns** |     **1,567.3 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| QuickSort3way      | 256  | SingleElementMoved |     4,862.4 ns |     52.01 ns |     18.55 ns |     4,858.5 ns |  3.10 |    0.02 |    5 |         - |          NA |
| QuickSortMedian3   | 256  | SingleElementMoved |     5,262.3 ns |    533.98 ns |    279.28 ns |     5,073.6 ns |  3.36 |    0.17 |    5 |         - |          NA |
| QuickSortMedian9   | 256  | SingleElementMoved |     4,200.0 ns |    142.78 ns |     63.40 ns |     4,190.3 ns |  2.68 |    0.04 |    5 |         - |          NA |
| DualPivotQuickSort | 256  | SingleElementMoved |     4,139.6 ns |    424.74 ns |    222.15 ns |     3,987.5 ns |  2.64 |    0.13 |    5 |         - |          NA |
| StableQuickSort    | 256  | SingleElementMoved |     8,537.0 ns |     14.14 ns |      5.04 ns |     8,535.7 ns |  5.45 |    0.03 |    6 |         - |          NA |
| IntroSort          | 256  | SingleElementMoved |       922.7 ns |      9.71 ns |      4.31 ns |       924.4 ns |  0.59 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | SingleElementMoved |     1,124.7 ns |     13.80 ns |      7.22 ns |     1,123.0 ns |  0.72 |    0.01 |    2 |         - |          NA |
| PDQSort            | 256  | SingleElementMoved |     1,176.1 ns |     24.21 ns |      8.63 ns |     1,173.5 ns |  0.75 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless  | 256  | SingleElementMoved |     1,463.1 ns |     30.19 ns |     13.40 ns |     1,457.1 ns |  0.93 |    0.01 |    3 |         - |          NA |
| Ipnsort            | 256  | SingleElementMoved |     4,909.8 ns |    323.07 ns |    168.97 ns |     4,916.9 ns |  3.13 |    0.10 |    5 |         - |          NA |
| StdSort            | 256  | SingleElementMoved |     2,744.2 ns |     64.40 ns |     28.59 ns |     2,735.9 ns |  1.75 |    0.02 |    4 |         - |          NA |
| BlockQuickSort     | 256  | SingleElementMoved |     1,520.1 ns |     21.04 ns |      9.34 ns |     1,518.5 ns |  0.97 |    0.01 |    3 |         - |          NA |
| DotnetSort         | 256  | SingleElementMoved |     1,127.3 ns |     28.87 ns |     12.82 ns |     1,128.8 ns |  0.72 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **QuickSort**          | **256**  | **Sorted**             |     **1,232.9 ns** |    **346.99 ns** |    **154.07 ns** |     **1,154.2 ns** |  **1.01** |    **0.16** |    **5** |         **-** |          **NA** |
| QuickSort3way      | 256  | Sorted             |     6,320.7 ns |     70.84 ns |     31.45 ns |     6,319.4 ns |  5.19 |    0.55 |    7 |         - |          NA |
| QuickSortMedian3   | 256  | Sorted             |     6,266.0 ns |     39.64 ns |     20.73 ns |     6,258.4 ns |  5.14 |    0.54 |    7 |         - |          NA |
| QuickSortMedian9   | 256  | Sorted             |     4,829.5 ns |    563.41 ns |    294.68 ns |     4,789.7 ns |  3.97 |    0.48 |    6 |         - |          NA |
| DualPivotQuickSort | 256  | Sorted             |     4,767.9 ns |    450.76 ns |    235.75 ns |     4,626.9 ns |  3.91 |    0.45 |    6 |         - |          NA |
| StableQuickSort    | 256  | Sorted             |     8,715.5 ns |    338.51 ns |    177.05 ns |     8,693.5 ns |  7.16 |    0.77 |    8 |         - |          NA |
| IntroSort          | 256  | Sorted             |       306.4 ns |      2.77 ns |      1.23 ns |       306.8 ns |  0.25 |    0.03 |    2 |         - |          NA |
| IntroSortDotnet    | 256  | Sorted             |     1,038.9 ns |      7.34 ns |      3.84 ns |     1,037.3 ns |  0.85 |    0.09 |    4 |         - |          NA |
| PDQSort            | 256  | Sorted             |       301.1 ns |      4.57 ns |      1.63 ns |       301.1 ns |  0.25 |    0.03 |    2 |         - |          NA |
| PDQSortBranchless  | 256  | Sorted             |       302.4 ns |      4.15 ns |      2.17 ns |       301.5 ns |  0.25 |    0.03 |    2 |         - |          NA |
| Ipnsort            | 256  | Sorted             |       147.5 ns |      1.91 ns |      1.00 ns |       147.3 ns |  0.12 |    0.01 |    1 |         - |          NA |
| StdSort            | 256  | Sorted             |       709.7 ns |      1.87 ns |      0.98 ns |       709.5 ns |  0.58 |    0.06 |    3 |         - |          NA |
| BlockQuickSort     | 256  | Sorted             |     1,274.2 ns |     23.14 ns |     10.27 ns |     1,277.2 ns |  1.05 |    0.11 |    5 |         - |          NA |
| DotnetSort         | 256  | Sorted             |     1,045.1 ns |    272.64 ns |    142.59 ns |     1,025.6 ns |  0.86 |    0.14 |    4 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **QuickSort**          | **256**  | **Reversed**           |       **980.0 ns** |     **31.95 ns** |     **14.19 ns** |       **977.6 ns** |  **1.00** |    **0.02** |    **4** |         **-** |          **NA** |
| QuickSort3way      | 256  | Reversed           |     5,279.4 ns |    293.62 ns |    130.37 ns |     5,191.8 ns |  5.39 |    0.14 |    6 |         - |          NA |
| QuickSortMedian3   | 256  | Reversed           |     7,113.4 ns |     17.03 ns |      6.07 ns |     7,114.8 ns |  7.26 |    0.10 |    7 |         - |          NA |
| QuickSortMedian9   | 256  | Reversed           |     5,139.5 ns |    540.77 ns |    282.83 ns |     4,955.4 ns |  5.25 |    0.28 |    6 |         - |          NA |
| DualPivotQuickSort | 256  | Reversed           |     4,669.1 ns |    406.96 ns |    212.85 ns |     4,532.3 ns |  4.77 |    0.21 |    6 |         - |          NA |
| StableQuickSort    | 256  | Reversed           |     9,121.1 ns |    525.74 ns |    233.43 ns |     8,970.7 ns |  9.31 |    0.26 |    8 |         - |          NA |
| IntroSort          | 256  | Reversed           |       635.8 ns |      1.74 ns |      0.77 ns |       635.7 ns |  0.65 |    0.01 |    3 |         - |          NA |
| IntroSortDotnet    | 256  | Reversed           |     1,582.6 ns |     47.02 ns |     24.59 ns |     1,576.6 ns |  1.62 |    0.03 |    5 |         - |          NA |
| PDQSort            | 256  | Reversed           |       529.7 ns |      4.78 ns |      2.12 ns |       529.4 ns |  0.54 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless  | 256  | Reversed           |       931.6 ns |      9.75 ns |      5.10 ns |       932.9 ns |  0.95 |    0.01 |    4 |         - |          NA |
| Ipnsort            | 256  | Reversed           |       235.6 ns |     45.55 ns |     20.22 ns |       228.9 ns |  0.24 |    0.02 |    1 |         - |          NA |
| StdSort            | 256  | Reversed           |       929.3 ns |     12.76 ns |      4.55 ns |       930.4 ns |  0.95 |    0.01 |    4 |         - |          NA |
| BlockQuickSort     | 256  | Reversed           |     1,605.4 ns |     15.55 ns |      6.91 ns |     1,606.3 ns |  1.64 |    0.02 |    5 |         - |          NA |
| DotnetSort         | 256  | Reversed           |     1,418.5 ns |     54.28 ns |     19.36 ns |     1,408.5 ns |  1.45 |    0.03 |    5 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **QuickSort**          | **256**  | **PipeOrgan**          |     **7,723.6 ns** |     **39.50 ns** |     **17.54 ns** |     **7,720.2 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way      | 256  | PipeOrgan          |     5,749.4 ns |    531.71 ns |    278.10 ns |     5,859.7 ns |  0.74 |    0.03 |    3 |         - |          NA |
| QuickSortMedian3   | 256  | PipeOrgan          |     6,607.3 ns |    518.93 ns |    271.41 ns |     6,424.7 ns |  0.86 |    0.03 |    3 |         - |          NA |
| QuickSortMedian9   | 256  | PipeOrgan          |     4,069.6 ns |    100.66 ns |     35.90 ns |     4,049.6 ns |  0.53 |    0.00 |    3 |         - |          NA |
| DualPivotQuickSort | 256  | PipeOrgan          |     2,276.0 ns |    318.79 ns |    166.74 ns |     2,172.8 ns |  0.29 |    0.02 |    1 |         - |          NA |
| StableQuickSort    | 256  | PipeOrgan          |     9,234.4 ns |    476.37 ns |    249.15 ns |     9,153.6 ns |  1.20 |    0.03 |    4 |         - |          NA |
| IntroSort          | 256  | PipeOrgan          |     2,154.2 ns |    463.55 ns |    242.45 ns |     1,998.3 ns |  0.28 |    0.03 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | PipeOrgan          |     2,512.9 ns |     69.11 ns |     24.65 ns |     2,520.7 ns |  0.33 |    0.00 |    1 |         - |          NA |
| PDQSort            | 256  | PipeOrgan          |     1,775.7 ns |     91.77 ns |     40.75 ns |     1,757.8 ns |  0.23 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | PipeOrgan          |     3,207.3 ns |     34.10 ns |     15.14 ns |     3,206.5 ns |  0.42 |    0.00 |    2 |         - |          NA |
| Ipnsort            | 256  | PipeOrgan          |     5,380.5 ns |    376.25 ns |    196.79 ns |     5,333.4 ns |  0.70 |    0.02 |    3 |         - |          NA |
| StdSort            | 256  | PipeOrgan          |     3,881.1 ns |     66.99 ns |     29.74 ns |     3,868.6 ns |  0.50 |    0.00 |    3 |         - |          NA |
| BlockQuickSort     | 256  | PipeOrgan          |     4,496.3 ns |    285.32 ns |    126.69 ns |     4,468.7 ns |  0.58 |    0.02 |    3 |         - |          NA |
| DotnetSort         | 256  | PipeOrgan          |     2,544.5 ns |    310.91 ns |    138.05 ns |     2,550.9 ns |  0.33 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **QuickSort**          | **1024** | **Random**             |    **15,395.6 ns** |    **320.75 ns** |    **167.76 ns** |    **15,397.2 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 1024 | Random             |    18,388.0 ns |    927.97 ns |    485.34 ns |    18,271.6 ns |  1.19 |    0.03 |    2 |         - |          NA |
| QuickSortMedian3   | 1024 | Random             |    25,027.5 ns |  4,167.00 ns |  2,179.42 ns |    24,292.3 ns |  1.63 |    0.13 |    3 |         - |          NA |
| QuickSortMedian9   | 1024 | Random             |    23,920.1 ns |  4,343.29 ns |  2,271.63 ns |    24,148.2 ns |  1.55 |    0.14 |    3 |         - |          NA |
| DualPivotQuickSort | 1024 | Random             |    12,646.3 ns |    479.74 ns |    250.92 ns |    12,689.2 ns |  0.82 |    0.02 |    1 |         - |          NA |
| StableQuickSort    | 1024 | Random             |    85,663.9 ns |    637.67 ns |    283.13 ns |    85,628.5 ns |  5.56 |    0.06 |    4 |         - |          NA |
| IntroSort          | 1024 | Random             |    12,029.6 ns |    484.71 ns |    253.51 ns |    12,074.2 ns |  0.78 |    0.02 |    1 |         - |          NA |
| IntroSortDotnet    | 1024 | Random             |     9,987.4 ns |    601.20 ns |    314.44 ns |     9,891.0 ns |  0.65 |    0.02 |    1 |         - |          NA |
| PDQSort            | 1024 | Random             |    10,189.8 ns |    891.08 ns |    466.05 ns |     9,971.1 ns |  0.66 |    0.03 |    1 |         - |          NA |
| PDQSortBranchless  | 1024 | Random             |    16,774.8 ns |    207.11 ns |     91.96 ns |    16,775.1 ns |  1.09 |    0.01 |    2 |         - |          NA |
| Ipnsort            | 1024 | Random             |    23,819.9 ns |    568.31 ns |    252.33 ns |    23,746.5 ns |  1.55 |    0.02 |    3 |         - |          NA |
| StdSort            | 1024 | Random             |    15,239.1 ns |    167.36 ns |     74.31 ns |    15,231.5 ns |  0.99 |    0.01 |    2 |         - |          NA |
| BlockQuickSort     | 1024 | Random             |    16,082.4 ns |    151.35 ns |     67.20 ns |    16,077.4 ns |  1.04 |    0.01 |    2 |         - |          NA |
| DotnetSort         | 1024 | Random             |    11,942.0 ns |  1,300.26 ns |    680.06 ns |    11,642.1 ns |  0.78 |    0.04 |    1 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **QuickSort**          | **1024** | **SingleElementMoved** |     **7,533.2 ns** |    **492.42 ns** |    **257.54 ns** |     **7,523.6 ns** |  **1.00** |    **0.05** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 1024 | SingleElementMoved |    35,169.6 ns |  1,081.92 ns |    565.87 ns |    35,029.1 ns |  4.67 |    0.17 |    5 |         - |          NA |
| QuickSortMedian3   | 1024 | SingleElementMoved |    31,699.7 ns |    414.38 ns |    216.73 ns |    31,678.5 ns |  4.21 |    0.14 |    5 |         - |          NA |
| QuickSortMedian9   | 1024 | SingleElementMoved |    21,492.6 ns |    406.48 ns |    180.48 ns |    21,535.8 ns |  2.86 |    0.09 |    4 |         - |          NA |
| DualPivotQuickSort | 1024 | SingleElementMoved |    23,575.0 ns |    256.58 ns |    113.92 ns |    23,609.2 ns |  3.13 |    0.10 |    4 |         - |          NA |
| StableQuickSort    | 1024 | SingleElementMoved |    43,178.5 ns |  2,288.24 ns |  1,015.99 ns |    42,757.8 ns |  5.74 |    0.22 |    6 |         - |          NA |
| IntroSort          | 1024 | SingleElementMoved |     4,555.4 ns |    466.31 ns |    243.89 ns |     4,502.2 ns |  0.61 |    0.04 |    1 |         - |          NA |
| IntroSortDotnet    | 1024 | SingleElementMoved |    10,417.8 ns |  1,042.02 ns |    545.00 ns |    10,230.1 ns |  1.38 |    0.08 |    3 |         - |          NA |
| PDQSort            | 1024 | SingleElementMoved |     5,309.6 ns |    486.68 ns |    254.54 ns |     5,145.8 ns |  0.71 |    0.04 |    1 |         - |          NA |
| PDQSortBranchless  | 1024 | SingleElementMoved |     6,364.1 ns |    307.02 ns |    160.58 ns |     6,272.6 ns |  0.85 |    0.03 |    2 |         - |          NA |
| Ipnsort            | 1024 | SingleElementMoved |    23,036.7 ns |     75.57 ns |     26.95 ns |    23,043.5 ns |  3.06 |    0.10 |    4 |         - |          NA |
| StdSort            | 1024 | SingleElementMoved |    11,860.0 ns |    525.41 ns |    274.80 ns |    11,951.1 ns |  1.58 |    0.06 |    3 |         - |          NA |
| BlockQuickSort     | 1024 | SingleElementMoved |     9,031.4 ns |    516.86 ns |    270.33 ns |     9,071.8 ns |  1.20 |    0.05 |    3 |         - |          NA |
| DotnetSort         | 1024 | SingleElementMoved |     7,479.1 ns |    372.20 ns |    194.67 ns |     7,568.5 ns |  0.99 |    0.04 |    2 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **QuickSort**          | **1024** | **Sorted**             |     **5,631.5 ns** |    **340.99 ns** |    **178.35 ns** |     **5,682.7 ns** |  **1.00** |    **0.04** |    **5** |         **-** |          **NA** |
| QuickSort3way      | 1024 | Sorted             |    47,059.7 ns |    792.43 ns |    414.46 ns |    46,991.2 ns |  8.36 |    0.26 |    8 |         - |          NA |
| QuickSortMedian3   | 1024 | Sorted             |    43,337.9 ns |    227.08 ns |    100.83 ns |    43,328.9 ns |  7.70 |    0.23 |    8 |         - |          NA |
| QuickSortMedian9   | 1024 | Sorted             |    22,455.4 ns |  1,141.48 ns |    597.01 ns |    22,345.9 ns |  3.99 |    0.16 |    7 |         - |          NA |
| DualPivotQuickSort | 1024 | Sorted             |    24,496.2 ns |    269.57 ns |    140.99 ns |    24,488.1 ns |  4.35 |    0.13 |    7 |         - |          NA |
| StableQuickSort    | 1024 | Sorted             |    42,384.9 ns |    245.94 ns |    128.63 ns |    42,403.4 ns |  7.53 |    0.23 |    8 |         - |          NA |
| IntroSort          | 1024 | Sorted             |     1,335.8 ns |     59.20 ns |     26.28 ns |     1,329.3 ns |  0.24 |    0.01 |    3 |         - |          NA |
| IntroSortDotnet    | 1024 | Sorted             |     5,020.9 ns |    515.14 ns |    228.73 ns |     4,895.7 ns |  0.89 |    0.05 |    5 |         - |          NA |
| PDQSort            | 1024 | Sorted             |     1,025.1 ns |     22.67 ns |     10.07 ns |     1,019.9 ns |  0.18 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless  | 1024 | Sorted             |     1,032.2 ns |     31.87 ns |     16.67 ns |     1,023.2 ns |  0.18 |    0.01 |    2 |         - |          NA |
| Ipnsort            | 1024 | Sorted             |       573.7 ns |    135.04 ns |     59.96 ns |       542.6 ns |  0.10 |    0.01 |    1 |         - |          NA |
| StdSort            | 1024 | Sorted             |     2,616.1 ns |     12.90 ns |      5.73 ns |     2,613.3 ns |  0.46 |    0.01 |    4 |         - |          NA |
| BlockQuickSort     | 1024 | Sorted             |     7,224.3 ns |     85.52 ns |     44.73 ns |     7,235.4 ns |  1.28 |    0.04 |    6 |         - |          NA |
| DotnetSort         | 1024 | Sorted             |     5,037.9 ns |    836.22 ns |    437.36 ns |     5,048.3 ns |  0.90 |    0.08 |    5 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **QuickSort**          | **1024** | **Reversed**           |     **4,830.1 ns** |    **624.28 ns** |    **326.51 ns** |     **4,772.8 ns** |  **1.00** |    **0.09** |    **4** |         **-** |          **NA** |
| QuickSort3way      | 1024 | Reversed           |    39,203.3 ns |  1,434.03 ns |    511.39 ns |    39,026.1 ns |  8.15 |    0.52 |    7 |         - |          NA |
| QuickSortMedian3   | 1024 | Reversed           |    52,322.9 ns |  1,219.07 ns |    541.27 ns |    52,224.1 ns | 10.88 |    0.69 |    7 |         - |          NA |
| QuickSortMedian9   | 1024 | Reversed           |    23,581.2 ns |    558.86 ns |    292.30 ns |    23,560.1 ns |  4.90 |    0.31 |    6 |         - |          NA |
| DualPivotQuickSort | 1024 | Reversed           |    24,175.7 ns |    225.42 ns |    100.09 ns |    24,191.6 ns |  5.02 |    0.32 |    6 |         - |          NA |
| StableQuickSort    | 1024 | Reversed           |    45,109.6 ns |    139.31 ns |     72.86 ns |    45,100.6 ns |  9.38 |    0.59 |    7 |         - |          NA |
| IntroSort          | 1024 | Reversed           |     3,861.9 ns |     25.31 ns |      9.03 ns |     3,861.4 ns |  0.80 |    0.05 |    3 |         - |          NA |
| IntroSortDotnet    | 1024 | Reversed           |     7,987.2 ns |    463.48 ns |    242.41 ns |     7,978.8 ns |  1.66 |    0.11 |    5 |         - |          NA |
| PDQSort            | 1024 | Reversed           |     1,913.5 ns |     17.43 ns |      9.12 ns |     1,911.3 ns |  0.40 |    0.02 |    2 |         - |          NA |
| PDQSortBranchless  | 1024 | Reversed           |     3,312.2 ns |     30.10 ns |     13.37 ns |     3,309.0 ns |  0.69 |    0.04 |    3 |         - |          NA |
| Ipnsort            | 1024 | Reversed           |       829.7 ns |    135.46 ns |     60.14 ns |       834.9 ns |  0.17 |    0.02 |    1 |         - |          NA |
| StdSort            | 1024 | Reversed           |     3,364.0 ns |     21.76 ns |      7.76 ns |     3,366.1 ns |  0.70 |    0.04 |    3 |         - |          NA |
| BlockQuickSort     | 1024 | Reversed           |     7,964.2 ns |    180.29 ns |     94.30 ns |     7,993.1 ns |  1.66 |    0.11 |    5 |         - |          NA |
| DotnetSort         | 1024 | Reversed           |     7,183.3 ns |     65.89 ns |     29.26 ns |     7,176.9 ns |  1.49 |    0.09 |    5 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **QuickSort**          | **1024** | **PipeOrgan**          |    **97,363.7 ns** |    **431.73 ns** |    **191.69 ns** |    **97,359.5 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| QuickSort3way      | 1024 | PipeOrgan          |    35,373.8 ns |    352.31 ns |    184.27 ns |    35,366.2 ns |  0.36 |    0.00 |    5 |         - |          NA |
| QuickSortMedian3   | 1024 | PipeOrgan          |    50,847.0 ns | 33,655.97 ns | 17,602.72 ns |    39,074.1 ns |  0.52 |    0.17 |    6 |         - |          NA |
| QuickSortMedian9   | 1024 | PipeOrgan          |    22,298.1 ns |    710.87 ns |    371.80 ns |    22,292.7 ns |  0.23 |    0.00 |    4 |         - |          NA |
| DualPivotQuickSort | 1024 | PipeOrgan          |    11,569.5 ns |    581.32 ns |    258.11 ns |    11,597.8 ns |  0.12 |    0.00 |    2 |         - |          NA |
| StableQuickSort    | 1024 | PipeOrgan          |    45,516.3 ns |    323.11 ns |    168.99 ns |    45,561.5 ns |  0.47 |    0.00 |    6 |         - |          NA |
| IntroSort          | 1024 | PipeOrgan          |    14,980.2 ns |    341.81 ns |    178.77 ns |    14,968.8 ns |  0.15 |    0.00 |    3 |         - |          NA |
| IntroSortDotnet    | 1024 | PipeOrgan          |    15,033.4 ns |    226.74 ns |    100.67 ns |    15,045.8 ns |  0.15 |    0.00 |    3 |         - |          NA |
| PDQSort            | 1024 | PipeOrgan          |     9,392.3 ns |    371.32 ns |    194.21 ns |     9,419.6 ns |  0.10 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 1024 | PipeOrgan          |    18,304.9 ns |    622.86 ns |    276.55 ns |    18,400.2 ns |  0.19 |    0.00 |    4 |         - |          NA |
| Ipnsort            | 1024 | PipeOrgan          |    25,431.6 ns |    617.81 ns |    323.13 ns |    25,364.4 ns |  0.26 |    0.00 |    4 |         - |          NA |
| StdSort            | 1024 | PipeOrgan          |    21,144.9 ns |    643.33 ns |    336.47 ns |    21,162.5 ns |  0.22 |    0.00 |    4 |         - |          NA |
| BlockQuickSort     | 1024 | PipeOrgan          |    24,569.5 ns |    827.62 ns |    367.47 ns |    24,454.8 ns |  0.25 |    0.00 |    4 |         - |          NA |
| DotnetSort         | 1024 | PipeOrgan          |    15,142.2 ns |    856.67 ns |    448.06 ns |    14,928.4 ns |  0.16 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **QuickSort**          | **8192** | **Random**             |   **427,908.9 ns** |  **4,472.87 ns** |  **2,339.40 ns** |   **426,858.8 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| QuickSort3way      | 8192 | Random             |   425,246.1 ns |  3,304.72 ns |  1,728.43 ns |   424,538.1 ns |  0.99 |    0.01 |    1 |         - |          NA |
| QuickSortMedian3   | 8192 | Random             |   533,522.3 ns |  2,218.79 ns |    985.16 ns |   533,550.2 ns |  1.25 |    0.01 |    1 |         - |          NA |
| QuickSortMedian9   | 8192 | Random             |   512,897.6 ns | 13,589.34 ns |  7,107.49 ns |   512,340.9 ns |  1.20 |    0.02 |    1 |         - |          NA |
| DualPivotQuickSort | 8192 | Random             |   364,433.3 ns |  1,494.24 ns |    663.45 ns |   364,355.1 ns |  0.85 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 8192 | Random             | 1,162,212.1 ns |  3,293.33 ns |  1,174.43 ns | 1,161,995.6 ns |  2.72 |    0.01 |    2 |         - |          NA |
| IntroSort          | 8192 | Random             |   383,209.6 ns |  1,778.02 ns |    929.94 ns |   383,079.9 ns |  0.90 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet    | 8192 | Random             |   351,012.3 ns |  1,300.13 ns |    679.99 ns |   351,074.8 ns |  0.82 |    0.00 |    1 |         - |          NA |
| PDQSort            | 8192 | Random             |   361,024.6 ns |  2,128.48 ns |  1,113.24 ns |   360,963.3 ns |  0.84 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | Random             |   466,296.6 ns |  3,070.04 ns |  1,605.69 ns |   466,230.3 ns |  1.09 |    0.01 |    1 |         - |          NA |
| Ipnsort            | 8192 | Random             |   481,434.7 ns |  2,245.42 ns |  1,174.40 ns |   481,180.7 ns |  1.13 |    0.01 |    1 |         - |          NA |
| StdSort            | 8192 | Random             |   403,782.7 ns |  2,268.19 ns |  1,186.31 ns |   403,749.7 ns |  0.94 |    0.01 |    1 |         - |          NA |
| BlockQuickSort     | 8192 | Random             |   437,189.7 ns |  1,381.96 ns |    613.60 ns |   437,241.1 ns |  1.02 |    0.01 |    1 |         - |          NA |
| DotnetSort         | 8192 | Random             |   344,184.3 ns |  2,427.35 ns |  1,077.76 ns |   344,137.9 ns |  0.80 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **QuickSort**          | **8192** | **SingleElementMoved** |    **77,724.7 ns** |  **4,694.92 ns** |  **2,455.53 ns** |    **76,932.0 ns** |  **1.00** |    **0.04** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 8192 | SingleElementMoved |   748,924.0 ns |  3,336.87 ns |  1,745.25 ns |   748,931.1 ns |  9.64 |    0.28 |    8 |         - |          NA |
| QuickSortMedian3   | 8192 | SingleElementMoved |   572,716.0 ns |  4,098.17 ns |  2,143.42 ns |   572,807.0 ns |  7.37 |    0.22 |    7 |         - |          NA |
| QuickSortMedian9   | 8192 | SingleElementMoved |   213,205.9 ns |  5,372.44 ns |  2,809.89 ns |   212,239.1 ns |  2.75 |    0.09 |    5 |         - |          NA |
| DualPivotQuickSort | 8192 | SingleElementMoved |   154,887.3 ns |  1,052.79 ns |    550.63 ns |   154,770.8 ns |  1.99 |    0.06 |    4 |         - |          NA |
| StableQuickSort    | 8192 | SingleElementMoved |   433,717.7 ns |  1,293.65 ns |    676.60 ns |   433,659.5 ns |  5.58 |    0.16 |    6 |         - |          NA |
| IntroSort          | 8192 | SingleElementMoved |    40,347.5 ns |    802.95 ns |    356.52 ns |    40,459.9 ns |  0.52 |    0.02 |    1 |         - |          NA |
| IntroSortDotnet    | 8192 | SingleElementMoved |    63,922.8 ns |  1,002.61 ns |    445.17 ns |    63,846.3 ns |  0.82 |    0.02 |    2 |         - |          NA |
| PDQSort            | 8192 | SingleElementMoved |    43,722.6 ns |    723.67 ns |    378.49 ns |    43,729.2 ns |  0.56 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | SingleElementMoved |    54,121.7 ns |  1,414.94 ns |    740.04 ns |    54,012.8 ns |  0.70 |    0.02 |    2 |         - |          NA |
| Ipnsort            | 8192 | SingleElementMoved |   227,505.6 ns |  1,417.27 ns |    741.26 ns |   227,566.6 ns |  2.93 |    0.09 |    5 |         - |          NA |
| StdSort            | 8192 | SingleElementMoved |    94,487.4 ns |  1,623.61 ns |    849.18 ns |    94,416.8 ns |  1.22 |    0.04 |    3 |         - |          NA |
| BlockQuickSort     | 8192 | SingleElementMoved |    93,095.9 ns |  1,688.04 ns |    882.88 ns |    93,213.8 ns |  1.20 |    0.04 |    3 |         - |          NA |
| DotnetSort         | 8192 | SingleElementMoved |    74,217.7 ns |  5,102.48 ns |  2,668.70 ns |    74,049.0 ns |  0.96 |    0.04 |    2 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **QuickSort**          | **8192** | **Sorted**             |    **62,073.3 ns** |  **6,453.63 ns** |  **3,375.37 ns** |    **61,837.3 ns** |  **1.00** |    **0.07** |    **5** |         **-** |          **NA** |
| QuickSort3way      | 8192 | Sorted             | 1,022,360.9 ns |  4,338.38 ns |  1,926.27 ns | 1,022,872.0 ns | 16.51 |    0.85 |    9 |         - |          NA |
| QuickSortMedian3   | 8192 | Sorted             |   890,754.8 ns |  4,478.17 ns |  1,988.34 ns |   890,744.7 ns | 14.39 |    0.74 |    9 |         - |          NA |
| QuickSortMedian9   | 8192 | Sorted             |   208,493.5 ns |  6,679.03 ns |  2,965.53 ns |   208,033.0 ns |  3.37 |    0.18 |    7 |         - |          NA |
| DualPivotQuickSort | 8192 | Sorted             |   174,833.8 ns |    715.69 ns |    317.77 ns |   174,964.6 ns |  2.82 |    0.14 |    7 |         - |          NA |
| StableQuickSort    | 8192 | Sorted             |   431,847.2 ns |  2,896.41 ns |  1,514.88 ns |   431,718.5 ns |  6.98 |    0.36 |    8 |         - |          NA |
| IntroSort          | 8192 | Sorted             |     8,635.4 ns |    486.89 ns |    254.65 ns |     8,522.9 ns |  0.14 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet    | 8192 | Sorted             |    48,315.9 ns |  1,000.78 ns |    523.43 ns |    48,309.0 ns |  0.78 |    0.04 |    4 |         - |          NA |
| PDQSort            | 8192 | Sorted             |     8,052.8 ns |    286.43 ns |    149.81 ns |     8,043.2 ns |  0.13 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless  | 8192 | Sorted             |     8,156.4 ns |    638.47 ns |    333.93 ns |     8,094.7 ns |  0.13 |    0.01 |    2 |         - |          NA |
| Ipnsort            | 8192 | Sorted             |     4,048.5 ns |    296.94 ns |    155.31 ns |     3,950.8 ns |  0.07 |    0.00 |    1 |         - |          NA |
| StdSort            | 8192 | Sorted             |    20,834.4 ns |    264.10 ns |    117.26 ns |    20,824.0 ns |  0.34 |    0.02 |    3 |         - |          NA |
| BlockQuickSort     | 8192 | Sorted             |    80,914.0 ns |    881.94 ns |    461.27 ns |    80,974.3 ns |  1.31 |    0.07 |    6 |         - |          NA |
| DotnetSort         | 8192 | Sorted             |    50,058.2 ns |  4,726.35 ns |  2,471.97 ns |    49,770.9 ns |  0.81 |    0.06 |    4 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **QuickSort**          | **8192** | **Reversed**           |    **50,022.3 ns** |  **4,589.37 ns** |  **2,037.71 ns** |    **49,379.9 ns** |  **1.00** |    **0.05** |    **5** |         **-** |          **NA** |
| QuickSort3way      | 8192 | Reversed           |   840,956.2 ns |  3,584.55 ns |  1,874.79 ns |   840,663.4 ns | 16.83 |    0.61 |    9 |         - |          NA |
| QuickSortMedian3   | 8192 | Reversed           | 1,121,208.2 ns |  6,923.14 ns |  3,073.92 ns | 1,120,151.0 ns | 22.44 |    0.81 |   10 |         - |          NA |
| QuickSortMedian9   | 8192 | Reversed           |   211,744.1 ns |  1,662.80 ns |    738.29 ns |   211,444.5 ns |  4.24 |    0.15 |    7 |         - |          NA |
| DualPivotQuickSort | 8192 | Reversed           |   180,032.8 ns |  1,387.09 ns |    725.48 ns |   179,806.8 ns |  3.60 |    0.13 |    7 |         - |          NA |
| StableQuickSort    | 8192 | Reversed           |   465,820.0 ns |  2,365.04 ns |  1,050.09 ns |   466,059.2 ns |  9.32 |    0.34 |    8 |         - |          NA |
| IntroSort          | 8192 | Reversed           |    35,183.0 ns |  1,863.23 ns |    827.29 ns |    34,893.9 ns |  0.70 |    0.03 |    4 |         - |          NA |
| IntroSortDotnet    | 8192 | Reversed           |    79,978.4 ns |    302.78 ns |    134.44 ns |    80,036.1 ns |  1.60 |    0.06 |    6 |         - |          NA |
| PDQSort            | 8192 | Reversed           |    14,434.0 ns |    436.99 ns |    155.83 ns |    14,487.1 ns |  0.29 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless  | 8192 | Reversed           |    25,778.5 ns |    865.41 ns |    384.25 ns |    25,804.5 ns |  0.52 |    0.02 |    3 |         - |          NA |
| Ipnsort            | 8192 | Reversed           |     6,146.3 ns |    348.80 ns |    182.43 ns |     6,051.7 ns |  0.12 |    0.01 |    1 |         - |          NA |
| StdSort            | 8192 | Reversed           |    26,751.2 ns |    756.97 ns |    395.91 ns |    26,873.2 ns |  0.54 |    0.02 |    3 |         - |          NA |
| BlockQuickSort     | 8192 | Reversed           |    78,099.0 ns |  1,090.75 ns |    570.49 ns |    77,922.0 ns |  1.56 |    0.06 |    6 |         - |          NA |
| DotnetSort         | 8192 | Reversed           |    80,432.1 ns |  4,071.96 ns |  2,129.71 ns |    80,057.3 ns |  1.61 |    0.07 |    6 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **QuickSort**          | **8192** | **PipeOrgan**          | **5,361,358.4 ns** |  **7,967.19 ns** |  **3,537.48 ns** | **5,361,553.6 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way      | 8192 | PipeOrgan          |   508,523.3 ns |  1,498.71 ns |    665.44 ns |   508,244.0 ns |  0.09 |    0.00 |    2 |         - |          NA |
| QuickSortMedian3   | 8192 | PipeOrgan          |   494,101.4 ns |  5,796.20 ns |  2,066.98 ns |   493,279.1 ns |  0.09 |    0.00 |    2 |         - |          NA |
| QuickSortMedian9   | 8192 | PipeOrgan          |   276,031.4 ns |  3,273.67 ns |  1,712.19 ns |   275,495.0 ns |  0.05 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort | 8192 | PipeOrgan          |   147,030.0 ns |  1,371.64 ns |    717.39 ns |   147,005.3 ns |  0.03 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 8192 | PipeOrgan          |   468,527.7 ns |  2,903.19 ns |  1,518.42 ns |   469,507.2 ns |  0.09 |    0.00 |    2 |         - |          NA |
| IntroSort          | 8192 | PipeOrgan          |   330,256.4 ns |  2,072.22 ns |    920.08 ns |   330,076.5 ns |  0.06 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet    | 8192 | PipeOrgan          |   371,496.0 ns |    888.86 ns |    464.89 ns |   371,468.3 ns |  0.07 |    0.00 |    2 |         - |          NA |
| PDQSort            | 8192 | PipeOrgan          |   143,902.2 ns |  3,033.25 ns |  1,346.78 ns |   143,916.0 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | PipeOrgan          |   275,463.8 ns |  1,461.98 ns |    764.64 ns |   275,384.5 ns |  0.05 |    0.00 |    2 |         - |          NA |
| Ipnsort            | 8192 | PipeOrgan          |   256,402.4 ns |  1,307.23 ns |    683.71 ns |   256,284.6 ns |  0.05 |    0.00 |    2 |         - |          NA |
| StdSort            | 8192 | PipeOrgan          |   435,086.7 ns |  1,839.74 ns |    962.22 ns |   435,171.4 ns |  0.08 |    0.00 |    2 |         - |          NA |
| BlockQuickSort     | 8192 | PipeOrgan          |   267,740.4 ns |  1,759.37 ns |    781.17 ns |   267,524.0 ns |  0.05 |    0.00 |    2 |         - |          NA |
| DotnetSort         | 8192 | PipeOrgan          |   363,539.5 ns | 12,681.96 ns |  6,632.91 ns |   363,014.0 ns |  0.07 |    0.00 |    2 |         - |          NA |

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

| Method                   | Size | Pattern            | Mean           | Error       | StdDev      | Median         | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------------- |----- |------------------- |---------------:|------------:|------------:|---------------:|------:|--------:|-----:|----------:|------------:|
| **MergeSort**                | **256**  | **Random**             |     **8,418.0 ns** |   **213.38 ns** |   **111.60 ns** |     **8,439.9 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Random             |     8,243.4 ns |   532.73 ns |   278.63 ns |     8,189.8 ns |  0.98 |    0.03 |    3 |         - |          NA |
| BottomupMergeSort        | 256  | Random             |     4,809.1 ns |   340.55 ns |   178.11 ns |     4,796.7 ns |  0.57 |    0.02 |    2 |         - |          NA |
| StdStableSort            | 256  | Random             |     3,243.1 ns |   506.83 ns |   265.08 ns |     3,059.7 ns |  0.39 |    0.03 |    1 |         - |          NA |
| RotateMergeSort          | 256  | Random             |     9,372.6 ns |   203.45 ns |    90.33 ns |     9,360.2 ns |  1.11 |    0.02 |    3 |         - |          NA |
| RotateMergeSortRecursive | 256  | Random             |    11,270.2 ns |   499.90 ns |   261.46 ns |    11,242.0 ns |  1.34 |    0.03 |    4 |         - |          NA |
| SymMergeSort             | 256  | Random             |     6,540.6 ns |   131.50 ns |    68.78 ns |     6,511.6 ns |  0.78 |    0.01 |    2 |         - |          NA |
| BlockMergeSort           | 256  | Random             |     6,029.4 ns |   196.60 ns |   102.82 ns |     6,077.4 ns |  0.72 |    0.01 |    2 |         - |          NA |
| NaturalMergeSort         | 256  | Random             |     5,182.1 ns |   324.50 ns |   169.72 ns |     5,125.3 ns |  0.62 |    0.02 |    2 |         - |          NA |
| TimSort                  | 256  | Random             |     3,798.9 ns |    63.14 ns |    22.52 ns |     3,790.4 ns |  0.45 |    0.01 |    2 |         - |          NA |
| PowerSort                | 256  | Random             |     2,332.0 ns |    43.47 ns |    19.30 ns |     2,332.2 ns |  0.28 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Random             |     4,201.8 ns |   503.97 ns |   263.59 ns |     4,031.5 ns |  0.50 |    0.03 |    2 |         - |          NA |
| SpinSort                 | 256  | Random             |     2,092.7 ns |    59.20 ns |    21.11 ns |     2,083.4 ns |  0.25 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 256  | Random             |     2,457.8 ns |    36.58 ns |    16.24 ns |     2,456.8 ns |  0.29 |    0.00 |    1 |         - |          NA |
| Glidesort                | 256  | Random             |     4,208.1 ns |    31.52 ns |    13.99 ns |     4,202.8 ns |  0.50 |    0.01 |    2 |         - |          NA |
| Driftsort                | 256  | Random             |     4,341.7 ns |   405.47 ns |   212.07 ns |     4,194.4 ns |  0.52 |    0.02 |    2 |         - |          NA |
| FlatStableSort           | 256  | Random             |     2,667.1 ns |    45.09 ns |    20.02 ns |     2,661.4 ns |  0.32 |    0.00 |    1 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **MergeSort**                | **256**  | **SingleElementMoved** |     **4,294.7 ns** |    **65.47 ns** |    **23.35 ns** |     **4,288.6 ns** |  **1.00** |    **0.01** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | SingleElementMoved |     5,253.8 ns |    51.35 ns |    22.80 ns |     5,254.4 ns |  1.22 |    0.01 |    9 |         - |          NA |
| BottomupMergeSort        | 256  | SingleElementMoved |     2,336.7 ns |    32.48 ns |    14.42 ns |     2,332.6 ns |  0.54 |    0.00 |    7 |         - |          NA |
| StdStableSort            | 256  | SingleElementMoved |     1,853.7 ns |     8.85 ns |     3.93 ns |     1,855.4 ns |  0.43 |    0.00 |    6 |         - |          NA |
| RotateMergeSort          | 256  | SingleElementMoved |       750.9 ns |   558.25 ns |   247.87 ns |       593.0 ns |  0.17 |    0.05 |    4 |         - |          NA |
| RotateMergeSortRecursive | 256  | SingleElementMoved |       720.5 ns |     7.87 ns |     3.49 ns |       721.3 ns |  0.17 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 256  | SingleElementMoved |       521.0 ns |     6.53 ns |     2.90 ns |       519.9 ns |  0.12 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 256  | SingleElementMoved |     4,236.5 ns |    11.78 ns |     5.23 ns |     4,237.8 ns |  0.99 |    0.01 |    8 |         - |          NA |
| NaturalMergeSort         | 256  | SingleElementMoved |       599.6 ns |     2.81 ns |     1.25 ns |       599.4 ns |  0.14 |    0.00 |    3 |         - |          NA |
| TimSort                  | 256  | SingleElementMoved |       307.4 ns |    13.83 ns |     7.23 ns |       304.9 ns |  0.07 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | SingleElementMoved |       414.9 ns |     5.68 ns |     2.03 ns |       415.8 ns |  0.10 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 256  | SingleElementMoved |       400.7 ns |    14.36 ns |     7.51 ns |       396.8 ns |  0.09 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 256  | SingleElementMoved |       914.2 ns |    17.85 ns |     7.93 ns |       914.8 ns |  0.21 |    0.00 |    5 |         - |          NA |
| SpinSortVariant          | 256  | SingleElementMoved |       960.9 ns |     4.99 ns |     1.78 ns |       961.3 ns |  0.22 |    0.00 |    5 |         - |          NA |
| Glidesort                | 256  | SingleElementMoved |     1,197.6 ns |     9.17 ns |     4.07 ns |     1,196.0 ns |  0.28 |    0.00 |    5 |         - |          NA |
| Driftsort                | 256  | SingleElementMoved |     1,140.3 ns |     5.54 ns |     2.46 ns |     1,140.3 ns |  0.27 |    0.00 |    5 |         - |          NA |
| FlatStableSort           | 256  | SingleElementMoved |     1,258.8 ns |     9.98 ns |     4.43 ns |     1,257.2 ns |  0.29 |    0.00 |    5 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **MergeSort**                | **256**  | **Sorted**             |     **3,853.9 ns** |     **3.40 ns** |     **1.21 ns** |     **3,853.8 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Sorted             |     4,847.9 ns |    10.60 ns |     3.78 ns |     4,849.1 ns |  1.26 |    0.00 |    8 |         - |          NA |
| BottomupMergeSort        | 256  | Sorted             |     1,967.9 ns |     4.30 ns |     1.91 ns |     1,967.7 ns |  0.51 |    0.00 |    6 |         - |          NA |
| StdStableSort            | 256  | Sorted             |     1,707.2 ns |     6.93 ns |     3.62 ns |     1,708.0 ns |  0.44 |    0.00 |    6 |         - |          NA |
| RotateMergeSort          | 256  | Sorted             |       347.4 ns |     2.74 ns |     1.43 ns |       347.1 ns |  0.09 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 256  | Sorted             |       463.2 ns |     1.52 ns |     0.68 ns |       462.9 ns |  0.12 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 256  | Sorted             |       343.4 ns |     2.28 ns |     1.01 ns |       343.1 ns |  0.09 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 256  | Sorted             |     3,243.7 ns |     4.34 ns |     1.93 ns |     3,244.2 ns |  0.84 |    0.00 |    7 |         - |          NA |
| NaturalMergeSort         | 256  | Sorted             |       214.8 ns |     2.02 ns |     1.06 ns |       214.7 ns |  0.06 |    0.00 |    2 |         - |          NA |
| TimSort                  | 256  | Sorted             |       191.0 ns |     2.53 ns |     1.32 ns |       190.8 ns |  0.05 |    0.00 |    2 |         - |          NA |
| PowerSort                | 256  | Sorted             |       148.4 ns |     1.39 ns |     0.62 ns |       148.4 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Sorted             |       320.0 ns |   138.37 ns |    49.35 ns |       340.3 ns |  0.08 |    0.01 |    3 |         - |          NA |
| SpinSort                 | 256  | Sorted             |       134.6 ns |     4.21 ns |     1.87 ns |       133.8 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 256  | Sorted             |       182.2 ns |     1.26 ns |     0.56 ns |       182.0 ns |  0.05 |    0.00 |    2 |         - |          NA |
| Glidesort                | 256  | Sorted             |       192.8 ns |     4.52 ns |     2.00 ns |       193.0 ns |  0.05 |    0.00 |    2 |         - |          NA |
| Driftsort                | 256  | Sorted             |       202.8 ns |     2.49 ns |     1.11 ns |       202.9 ns |  0.05 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 256  | Sorted             |     1,119.5 ns |    10.88 ns |     3.88 ns |     1,118.9 ns |  0.29 |    0.00 |    5 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **MergeSort**                | **256**  | **Reversed**           |     **8,552.9 ns** |   **212.90 ns** |    **94.53 ns** |     **8,511.4 ns** |  **1.00** |    **0.01** |    **5** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Reversed           |     7,690.8 ns |    14.60 ns |     6.48 ns |     7,690.9 ns |  0.90 |    0.01 |    5 |         - |          NA |
| BottomupMergeSort        | 256  | Reversed           |     5,202.0 ns |   270.16 ns |   141.30 ns |     5,106.3 ns |  0.61 |    0.02 |    4 |         - |          NA |
| StdStableSort            | 256  | Reversed           |     2,277.9 ns |    53.73 ns |    19.16 ns |     2,271.1 ns |  0.27 |    0.00 |    2 |         - |          NA |
| RotateMergeSort          | 256  | Reversed           |     1,771.9 ns |     8.51 ns |     3.78 ns |     1,773.1 ns |  0.21 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 256  | Reversed           |     1,863.6 ns |     2.62 ns |     1.16 ns |     1,863.7 ns |  0.22 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 256  | Reversed           |     2,009.7 ns |   227.07 ns |   100.82 ns |     1,963.5 ns |  0.24 |    0.01 |    2 |         - |          NA |
| BlockMergeSort           | 256  | Reversed           |     9,520.4 ns |   476.12 ns |   249.02 ns |     9,599.9 ns |  1.11 |    0.03 |    5 |         - |          NA |
| NaturalMergeSort         | 256  | Reversed           |       334.0 ns |     4.29 ns |     2.24 ns |       334.3 ns |  0.04 |    0.00 |    1 |         - |          NA |
| TimSort                  | 256  | Reversed           |       265.0 ns |     2.78 ns |     1.23 ns |       265.5 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | Reversed           |       230.0 ns |     1.42 ns |     0.74 ns |       229.8 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Reversed           |       251.4 ns |     4.28 ns |     1.53 ns |       251.2 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 256  | Reversed           |       259.1 ns |     1.75 ns |     0.78 ns |       258.8 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 256  | Reversed           |       286.8 ns |     2.21 ns |     0.98 ns |       286.6 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 256  | Reversed           |       279.6 ns |     2.52 ns |     1.32 ns |       279.6 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 256  | Reversed           |       279.3 ns |     3.77 ns |     1.67 ns |       279.0 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | Reversed           |     3,089.0 ns |   492.09 ns |   257.37 ns |     2,910.0 ns |  0.36 |    0.03 |    3 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **MergeSort**                | **256**  | **PipeOrgan**          |     **6,724.4 ns** |   **246.77 ns** |   **129.06 ns** |     **6,770.2 ns** |  **1.00** |    **0.03** |    **9** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | PipeOrgan          |     6,572.0 ns |   493.08 ns |   257.89 ns |     6,409.5 ns |  0.98 |    0.04 |    9 |         - |          NA |
| BottomupMergeSort        | 256  | PipeOrgan          |     3,856.0 ns |   705.45 ns |   368.96 ns |     3,615.8 ns |  0.57 |    0.05 |    7 |         - |          NA |
| StdStableSort            | 256  | PipeOrgan          |     2,061.0 ns |    15.59 ns |     6.92 ns |     2,061.1 ns |  0.31 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 256  | PipeOrgan          |     4,101.8 ns |    51.12 ns |    18.23 ns |     4,095.3 ns |  0.61 |    0.01 |    7 |         - |          NA |
| RotateMergeSortRecursive | 256  | PipeOrgan          |     5,104.5 ns |   583.76 ns |   259.19 ns |     4,914.1 ns |  0.76 |    0.04 |    8 |         - |          NA |
| SymMergeSort             | 256  | PipeOrgan          |     2,563.8 ns |   141.28 ns |    62.73 ns |     2,529.4 ns |  0.38 |    0.01 |    6 |         - |          NA |
| BlockMergeSort           | 256  | PipeOrgan          |     6,326.2 ns |    14.07 ns |     6.25 ns |     6,328.2 ns |  0.94 |    0.02 |    9 |         - |          NA |
| NaturalMergeSort         | 256  | PipeOrgan          |       686.7 ns |     8.84 ns |     3.92 ns |       686.1 ns |  0.10 |    0.00 |    2 |         - |          NA |
| TimSort                  | 256  | PipeOrgan          |       943.2 ns |   266.53 ns |   139.40 ns |       845.5 ns |  0.14 |    0.02 |    3 |         - |          NA |
| PowerSort                | 256  | PipeOrgan          |       502.3 ns |     5.06 ns |     2.25 ns |       501.4 ns |  0.07 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | PipeOrgan          |       540.8 ns |     6.29 ns |     2.79 ns |       540.1 ns |  0.08 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 256  | PipeOrgan          |     1,773.8 ns |    11.29 ns |     5.90 ns |     1,773.7 ns |  0.26 |    0.00 |    5 |         - |          NA |
| SpinSortVariant          | 256  | PipeOrgan          |     2,038.1 ns |   427.56 ns |   223.62 ns |     1,887.1 ns |  0.30 |    0.03 |    5 |         - |          NA |
| Glidesort                | 256  | PipeOrgan          |     1,215.8 ns |     9.51 ns |     4.97 ns |     1,213.5 ns |  0.18 |    0.00 |    4 |         - |          NA |
| Driftsort                | 256  | PipeOrgan          |       438.8 ns |     4.40 ns |     1.95 ns |       438.3 ns |  0.07 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | PipeOrgan          |     2,087.3 ns |     7.92 ns |     3.52 ns |     2,088.6 ns |  0.31 |    0.01 |    5 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **MergeSort**                | **1024** | **Random**             |    **36,649.7 ns** | **1,036.40 ns** |   **460.17 ns** |    **36,693.0 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Random             |    35,385.3 ns |   628.83 ns |   328.89 ns |    35,267.5 ns |  0.97 |    0.01 |    3 |         - |          NA |
| BottomupMergeSort        | 1024 | Random             |    22,667.2 ns |   868.34 ns |   385.55 ns |    22,819.7 ns |  0.62 |    0.01 |    2 |         - |          NA |
| StdStableSort            | 1024 | Random             |    15,563.2 ns |   354.16 ns |   185.23 ns |    15,584.3 ns |  0.42 |    0.01 |    1 |         - |          NA |
| RotateMergeSort          | 1024 | Random             |    68,969.1 ns | 7,832.29 ns | 4,096.44 ns |    67,194.4 ns |  1.88 |    0.11 |    4 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Random             |    67,198.1 ns | 1,456.91 ns |   761.99 ns |    67,032.4 ns |  1.83 |    0.03 |    4 |         - |          NA |
| SymMergeSort             | 1024 | Random             |    41,669.4 ns | 1,183.12 ns |   618.79 ns |    41,756.8 ns |  1.14 |    0.02 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | Random             |    32,588.5 ns |   138.80 ns |    61.63 ns |    32,604.4 ns |  0.89 |    0.01 |    3 |         - |          NA |
| NaturalMergeSort         | 1024 | Random             |    26,126.1 ns | 1,154.21 ns |   512.48 ns |    25,990.6 ns |  0.71 |    0.02 |    2 |         - |          NA |
| TimSort                  | 1024 | Random             |    19,710.9 ns |   502.75 ns |   223.23 ns |    19,662.9 ns |  0.54 |    0.01 |    2 |         - |          NA |
| PowerSort                | 1024 | Random             |    13,403.2 ns |   777.32 ns |   406.56 ns |    13,223.4 ns |  0.37 |    0.01 |    1 |         - |          NA |
| ShiftSort                | 1024 | Random             |    19,745.6 ns |   396.29 ns |   141.32 ns |    19,785.8 ns |  0.54 |    0.01 |    2 |         - |          NA |
| SpinSort                 | 1024 | Random             |    13,310.3 ns |   715.79 ns |   317.82 ns |    13,293.0 ns |  0.36 |    0.01 |    1 |         - |          NA |
| SpinSortVariant          | 1024 | Random             |    14,742.0 ns |   534.01 ns |   279.30 ns |    14,784.0 ns |  0.40 |    0.01 |    1 |         - |          NA |
| Glidesort                | 1024 | Random             |    19,888.8 ns |   394.94 ns |   175.35 ns |    19,882.2 ns |  0.54 |    0.01 |    2 |         - |          NA |
| Driftsort                | 1024 | Random             |    20,737.1 ns |   201.32 ns |   105.29 ns |    20,731.0 ns |  0.57 |    0.01 |    2 |         - |          NA |
| FlatStableSort           | 1024 | Random             |    14,965.5 ns |   439.45 ns |   229.84 ns |    14,940.5 ns |  0.41 |    0.01 |    1 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **MergeSort**                | **1024** | **SingleElementMoved** |    **17,092.6 ns** |   **195.34 ns** |   **102.17 ns** |    **17,078.0 ns** |  **1.00** |    **0.01** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | SingleElementMoved |    21,132.0 ns |   353.07 ns |   156.76 ns |    21,099.2 ns |  1.24 |    0.01 |    8 |         - |          NA |
| BottomupMergeSort        | 1024 | SingleElementMoved |     7,259.4 ns |    19.19 ns |     6.84 ns |     7,259.8 ns |  0.42 |    0.00 |    7 |         - |          NA |
| StdStableSort            | 1024 | SingleElementMoved |     8,176.7 ns |   414.56 ns |   216.82 ns |     8,211.4 ns |  0.48 |    0.01 |    7 |         - |          NA |
| RotateMergeSort          | 1024 | SingleElementMoved |     1,952.4 ns |    39.78 ns |    17.66 ns |     1,944.6 ns |  0.11 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 1024 | SingleElementMoved |     2,450.6 ns |    27.01 ns |     9.63 ns |     2,452.2 ns |  0.14 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 1024 | SingleElementMoved |     1,692.7 ns |     2.95 ns |     1.31 ns |     1,692.9 ns |  0.10 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 1024 | SingleElementMoved |    19,739.9 ns |   450.56 ns |   235.65 ns |    19,697.7 ns |  1.15 |    0.01 |    8 |         - |          NA |
| NaturalMergeSort         | 1024 | SingleElementMoved |     2,028.7 ns |     7.87 ns |     2.81 ns |     2,028.2 ns |  0.12 |    0.00 |    2 |         - |          NA |
| TimSort                  | 1024 | SingleElementMoved |     1,328.8 ns | 1,015.65 ns |   531.21 ns |     1,304.2 ns |  0.08 |    0.03 |    1 |         - |          NA |
| PowerSort                | 1024 | SingleElementMoved |     1,374.7 ns |     5.08 ns |     2.66 ns |     1,373.4 ns |  0.08 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | SingleElementMoved |     1,418.4 ns |   125.23 ns |    55.60 ns |     1,390.6 ns |  0.08 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 1024 | SingleElementMoved |     4,369.2 ns |   380.74 ns |   199.13 ns |     4,340.9 ns |  0.26 |    0.01 |    5 |         - |          NA |
| SpinSortVariant          | 1024 | SingleElementMoved |     3,429.3 ns |    12.34 ns |     4.40 ns |     3,428.9 ns |  0.20 |    0.00 |    4 |         - |          NA |
| Glidesort                | 1024 | SingleElementMoved |     2,597.7 ns |    13.27 ns |     4.73 ns |     2,595.7 ns |  0.15 |    0.00 |    3 |         - |          NA |
| Driftsort                | 1024 | SingleElementMoved |     1,238.6 ns |     7.56 ns |     3.36 ns |     1,238.4 ns |  0.07 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | SingleElementMoved |     5,581.3 ns |   326.67 ns |   170.86 ns |     5,670.6 ns |  0.33 |    0.01 |    6 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **MergeSort**                | **1024** | **Sorted**             |    **15,697.8 ns** |   **168.79 ns** |    **88.28 ns** |    **15,727.2 ns** |  **1.00** |    **0.01** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Sorted             |    19,956.6 ns |   528.51 ns |   276.42 ns |    19,927.3 ns |  1.27 |    0.02 |    7 |         - |          NA |
| BottomupMergeSort        | 1024 | Sorted             |     5,788.1 ns |     8.73 ns |     3.11 ns |     5,788.9 ns |  0.37 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 1024 | Sorted             |     7,730.8 ns |   304.42 ns |   159.22 ns |     7,711.2 ns |  0.49 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 1024 | Sorted             |     1,548.3 ns |   261.58 ns |   136.81 ns |     1,591.3 ns |  0.10 |    0.01 |    3 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Sorted             |     1,859.0 ns |     4.44 ns |     1.58 ns |     1,858.6 ns |  0.12 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 1024 | Sorted             |     1,316.6 ns |     2.40 ns |     1.26 ns |     1,316.2 ns |  0.08 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 1024 | Sorted             |    14,113.7 ns |   353.71 ns |   157.05 ns |    14,144.6 ns |  0.90 |    0.01 |    6 |         - |          NA |
| NaturalMergeSort         | 1024 | Sorted             |       584.8 ns |     2.64 ns |     1.38 ns |       584.7 ns |  0.04 |    0.00 |    1 |         - |          NA |
| TimSort                  | 1024 | Sorted             |       563.4 ns |     1.66 ns |     0.59 ns |       563.5 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Sorted             |       523.3 ns |     0.80 ns |     0.42 ns |       523.2 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Sorted             |       709.0 ns |     5.58 ns |     2.48 ns |       708.4 ns |  0.05 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 1024 | Sorted             |       463.1 ns |     2.52 ns |     1.12 ns |       463.5 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 1024 | Sorted             |       656.6 ns |     0.66 ns |     0.29 ns |       656.7 ns |  0.04 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Sorted             |       495.7 ns |    17.71 ns |     9.26 ns |       491.4 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | Sorted             |       725.3 ns |   279.69 ns |   146.28 ns |       757.9 ns |  0.05 |    0.01 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Sorted             |     5,014.4 ns |   427.56 ns |   223.62 ns |     4,878.0 ns |  0.32 |    0.01 |    4 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **MergeSort**                | **1024** | **Reversed**           |    **36,073.8 ns** |   **687.66 ns** |   **359.66 ns** |    **36,105.9 ns** |  **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Reversed           |    32,521.1 ns |   309.60 ns |   161.93 ns |    32,557.4 ns |  0.90 |    0.01 |    4 |         - |          NA |
| BottomupMergeSort        | 1024 | Reversed           |    20,286.4 ns |   299.81 ns |   156.81 ns |    20,284.5 ns |  0.56 |    0.01 |    3 |         - |          NA |
| StdStableSort            | 1024 | Reversed           |    10,460.0 ns |   632.38 ns |   330.75 ns |    10,542.8 ns |  0.29 |    0.01 |    2 |         - |          NA |
| RotateMergeSort          | 1024 | Reversed           |     8,530.0 ns |   414.51 ns |   216.80 ns |     8,401.8 ns |  0.24 |    0.01 |    2 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Reversed           |     9,051.0 ns |   511.39 ns |   267.46 ns |     8,992.6 ns |  0.25 |    0.01 |    2 |         - |          NA |
| SymMergeSort             | 1024 | Reversed           |     8,560.0 ns |   466.85 ns |   244.17 ns |     8,390.9 ns |  0.24 |    0.01 |    2 |         - |          NA |
| BlockMergeSort           | 1024 | Reversed           |    39,721.9 ns |   256.37 ns |   134.09 ns |    39,752.2 ns |  1.10 |    0.01 |    4 |         - |          NA |
| NaturalMergeSort         | 1024 | Reversed           |     1,058.1 ns |    25.22 ns |    13.19 ns |     1,060.5 ns |  0.03 |    0.00 |    1 |         - |          NA |
| TimSort                  | 1024 | Reversed           |       849.8 ns |    10.25 ns |     5.36 ns |       848.9 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Reversed           |       820.2 ns |    36.94 ns |    13.17 ns |       815.0 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Reversed           |     1,065.2 ns |    15.28 ns |     7.99 ns |     1,064.1 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 1024 | Reversed           |       957.0 ns |     2.61 ns |     1.36 ns |       957.1 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 1024 | Reversed           |     1,057.7 ns |     1.62 ns |     0.85 ns |     1,057.4 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Reversed           |     1,018.3 ns |   143.95 ns |    63.92 ns |     1,044.0 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | Reversed           |       861.4 ns |     8.75 ns |     4.58 ns |       860.7 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Reversed           |    12,463.4 ns |   360.06 ns |   188.32 ns |    12,449.2 ns |  0.35 |    0.01 |    2 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **MergeSort**                | **1024** | **PipeOrgan**          |    **26,539.3 ns** |   **321.48 ns** |   **168.14 ns** |    **26,541.9 ns** |  **1.00** |    **0.01** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | PipeOrgan          |    27,143.7 ns |   359.75 ns |   188.16 ns |    27,187.0 ns |  1.02 |    0.01 |    8 |         - |          NA |
| BottomupMergeSort        | 1024 | PipeOrgan          |    14,189.8 ns |   771.93 ns |   342.74 ns |    14,230.8 ns |  0.53 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 1024 | PipeOrgan          |     9,346.2 ns |   462.05 ns |   241.66 ns |     9,295.3 ns |  0.35 |    0.01 |    4 |         - |          NA |
| RotateMergeSort          | 1024 | PipeOrgan          |    18,117.1 ns |   425.70 ns |   189.01 ns |    18,095.6 ns |  0.68 |    0.01 |    7 |         - |          NA |
| RotateMergeSortRecursive | 1024 | PipeOrgan          |    21,392.7 ns |   286.16 ns |   127.06 ns |    21,426.6 ns |  0.81 |    0.01 |    7 |         - |          NA |
| SymMergeSort             | 1024 | PipeOrgan          |    11,814.2 ns |   973.29 ns |   509.05 ns |    11,757.3 ns |  0.45 |    0.02 |    5 |         - |          NA |
| BlockMergeSort           | 1024 | PipeOrgan          |    32,192.4 ns |   303.41 ns |   158.69 ns |    32,189.1 ns |  1.21 |    0.01 |    8 |         - |          NA |
| NaturalMergeSort         | 1024 | PipeOrgan          |     2,626.8 ns |   513.52 ns |   268.58 ns |     2,609.0 ns |  0.10 |    0.01 |    2 |         - |          NA |
| TimSort                  | 1024 | PipeOrgan          |     2,546.1 ns |    10.79 ns |     4.79 ns |     2,546.5 ns |  0.10 |    0.00 |    2 |         - |          NA |
| PowerSort                | 1024 | PipeOrgan          |     1,622.6 ns |    35.68 ns |    12.73 ns |     1,618.9 ns |  0.06 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | PipeOrgan          |     1,940.0 ns |    17.30 ns |     6.17 ns |     1,937.0 ns |  0.07 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 1024 | PipeOrgan          |     8,017.1 ns |   307.26 ns |   160.70 ns |     8,096.9 ns |  0.30 |    0.01 |    4 |         - |          NA |
| SpinSortVariant          | 1024 | PipeOrgan          |     7,762.2 ns |   316.99 ns |   165.79 ns |     7,763.1 ns |  0.29 |    0.01 |    4 |         - |          NA |
| Glidesort                | 1024 | PipeOrgan          |     4,425.7 ns |   300.65 ns |   133.49 ns |     4,365.0 ns |  0.17 |    0.00 |    3 |         - |          NA |
| Driftsort                | 1024 | PipeOrgan          |     1,363.5 ns |    10.86 ns |     4.82 ns |     1,362.9 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | PipeOrgan          |     9,283.2 ns |   359.08 ns |   187.81 ns |     9,381.8 ns |  0.35 |    0.01 |    4 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **MergeSort**                | **8192** | **Random**             |   **694,889.2 ns** | **2,917.55 ns** | **1,295.41 ns** |   **695,518.7 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Random             |   615,296.7 ns | 4,380.37 ns | 1,944.91 ns |   615,632.6 ns |  0.89 |    0.00 |    2 |         - |          NA |
| BottomupMergeSort        | 8192 | Random             |   495,746.5 ns | 2,304.04 ns | 1,205.06 ns |   495,658.3 ns |  0.71 |    0.00 |    2 |         - |          NA |
| StdStableSort            | 8192 | Random             |   473,270.0 ns | 1,315.45 ns |   584.07 ns |   473,360.8 ns |  0.68 |    0.00 |    2 |         - |          NA |
| RotateMergeSort          | 8192 | Random             | 1,344,559.5 ns | 7,627.44 ns | 3,989.30 ns | 1,344,452.2 ns |  1.93 |    0.01 |    4 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Random             | 1,463,214.3 ns | 3,311.11 ns | 1,180.77 ns | 1,463,094.9 ns |  2.11 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 8192 | Random             | 1,014,802.0 ns | 2,546.49 ns | 1,331.86 ns | 1,015,069.8 ns |  1.46 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 8192 | Random             |   694,296.7 ns | 6,026.06 ns | 2,675.61 ns |   695,241.3 ns |  1.00 |    0.00 |    2 |         - |          NA |
| NaturalMergeSort         | 8192 | Random             |   619,338.6 ns | 4,307.91 ns | 2,253.12 ns |   618,249.2 ns |  0.89 |    0.00 |    2 |         - |          NA |
| TimSort                  | 8192 | Random             |   564,269.2 ns | 1,310.60 ns |   581.92 ns |   564,086.8 ns |  0.81 |    0.00 |    2 |         - |          NA |
| PowerSort                | 8192 | Random             |   425,749.9 ns | 1,632.98 ns |   854.08 ns |   425,961.8 ns |  0.61 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 8192 | Random             |   562,079.7 ns | 1,176.29 ns |   522.28 ns |   562,303.6 ns |  0.81 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | Random             |   370,613.8 ns | 2,458.51 ns | 1,285.85 ns |   370,179.3 ns |  0.53 |    0.00 |    2 |         - |          NA |
| SpinSortVariant          | 8192 | Random             |   370,501.8 ns | 1,566.71 ns |   819.42 ns |   370,546.8 ns |  0.53 |    0.00 |    2 |         - |          NA |
| Glidesort                | 8192 | Random             |   206,270.2 ns | 2,598.38 ns | 1,359.00 ns |   206,174.7 ns |  0.30 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Random             |   218,232.9 ns | 2,382.67 ns | 1,246.19 ns |   217,758.4 ns |  0.31 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Random             |   397,115.2 ns | 2,604.37 ns |   928.74 ns |   396,778.0 ns |  0.57 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **MergeSort**                | **8192** | **SingleElementMoved** |   **135,672.4 ns** | **1,142.82 ns** |   **597.72 ns** |   **135,826.3 ns** |  **1.00** |    **0.01** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | SingleElementMoved |   168,109.8 ns | 1,643.95 ns |   729.92 ns |   168,057.0 ns |  1.24 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 8192 | SingleElementMoved |    55,957.0 ns | 1,296.04 ns |   575.45 ns |    56,133.0 ns |  0.41 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 8192 | SingleElementMoved |   109,618.9 ns |   425.85 ns |   189.08 ns |   109,626.6 ns |  0.81 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 8192 | SingleElementMoved |    14,402.8 ns | 1,046.34 ns |   464.58 ns |    14,395.0 ns |  0.11 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 8192 | SingleElementMoved |    18,124.5 ns | 1,028.04 ns |   537.68 ns |    17,992.6 ns |  0.13 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 8192 | SingleElementMoved |    13,129.9 ns | 1,178.17 ns |   523.11 ns |    12,918.4 ns |  0.10 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 8192 | SingleElementMoved |   147,226.0 ns | 1,107.45 ns |   491.71 ns |   147,432.1 ns |  1.09 |    0.01 |    6 |         - |          NA |
| NaturalMergeSort         | 8192 | SingleElementMoved |    15,591.6 ns |   109.16 ns |    48.47 ns |    15,580.2 ns |  0.11 |    0.00 |    3 |         - |          NA |
| TimSort                  | 8192 | SingleElementMoved |     5,656.0 ns |   346.95 ns |   154.05 ns |     5,549.8 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | SingleElementMoved |    10,431.8 ns |   581.62 ns |   207.41 ns |    10,473.3 ns |  0.08 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 8192 | SingleElementMoved |    10,444.1 ns |   103.14 ns |    45.79 ns |    10,425.4 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | SingleElementMoved |    22,830.1 ns |   783.03 ns |   347.67 ns |    22,941.8 ns |  0.17 |    0.00 |    3 |         - |          NA |
| SpinSortVariant          | 8192 | SingleElementMoved |    19,877.6 ns |   887.31 ns |   464.08 ns |    19,657.8 ns |  0.15 |    0.00 |    3 |         - |          NA |
| Glidesort                | 8192 | SingleElementMoved |    20,091.1 ns |   241.99 ns |   107.45 ns |    20,058.6 ns |  0.15 |    0.00 |    3 |         - |          NA |
| Driftsort                | 8192 | SingleElementMoved |     9,205.4 ns |   400.17 ns |   209.29 ns |     9,064.4 ns |  0.07 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 8192 | SingleElementMoved |    48,534.7 ns | 1,752.07 ns |   777.93 ns |    48,584.6 ns |  0.36 |    0.01 |    4 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **MergeSort**                | **8192** | **Sorted**             |   **124,572.3 ns** |   **435.48 ns** |   **193.36 ns** |   **124,547.6 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Sorted             |   159,237.2 ns |   605.16 ns |   268.69 ns |   159,097.5 ns |  1.28 |    0.00 |    7 |         - |          NA |
| BottomupMergeSort        | 8192 | Sorted             |    46,479.5 ns | 1,344.15 ns |   703.02 ns |    46,275.7 ns |  0.37 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 8192 | Sorted             |   105,989.7 ns |   373.79 ns |   133.30 ns |   105,983.1 ns |  0.85 |    0.00 |    6 |         - |          NA |
| RotateMergeSort          | 8192 | Sorted             |    11,349.7 ns |    60.69 ns |    26.94 ns |    11,343.0 ns |  0.09 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Sorted             |    15,299.2 ns |   278.56 ns |    99.34 ns |    15,343.0 ns |  0.12 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 8192 | Sorted             |    10,694.0 ns |   607.17 ns |   269.59 ns |    10,703.9 ns |  0.09 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 8192 | Sorted             |   111,005.5 ns | 1,105.28 ns |   578.08 ns |   110,923.6 ns |  0.89 |    0.00 |    6 |         - |          NA |
| NaturalMergeSort         | 8192 | Sorted             |     4,107.3 ns |    33.20 ns |    17.36 ns |     4,100.3 ns |  0.03 |    0.00 |    1 |         - |          NA |
| TimSort                  | 8192 | Sorted             |     4,214.2 ns |   395.56 ns |   206.89 ns |     4,069.8 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Sorted             |     4,028.6 ns |    20.75 ns |     9.21 ns |     4,025.2 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Sorted             |     5,332.6 ns |   462.59 ns |   205.39 ns |     5,181.9 ns |  0.04 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | Sorted             |     3,755.0 ns |    42.89 ns |    19.04 ns |     3,754.4 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 8192 | Sorted             |     5,716.1 ns |   680.77 ns |   356.06 ns |     5,744.3 ns |  0.05 |    0.00 |    2 |         - |          NA |
| Glidesort                | 8192 | Sorted             |     3,503.0 ns |   261.57 ns |   136.81 ns |     3,448.0 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Sorted             |     4,235.3 ns |   356.24 ns |   186.32 ns |     4,108.4 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Sorted             |     3,706.4 ns |   135.23 ns |    60.04 ns |     3,691.8 ns |  0.03 |    0.00 |    1 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **MergeSort**                | **8192** | **Reversed**           |   **303,234.8 ns** | **2,212.83 ns** |   **982.51 ns** |   **303,388.7 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Reversed           |   278,528.8 ns | 2,335.39 ns | 1,036.93 ns |   278,385.1 ns |  0.92 |    0.00 |    5 |         - |          NA |
| BottomupMergeSort        | 8192 | Reversed           |   170,379.1 ns | 1,432.54 ns |   636.05 ns |   170,300.0 ns |  0.56 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 8192 | Reversed           |   128,817.4 ns | 3,446.15 ns | 1,530.11 ns |   128,424.5 ns |  0.42 |    0.00 |    3 |         - |          NA |
| RotateMergeSort          | 8192 | Reversed           |    83,661.7 ns |   714.23 ns |   373.56 ns |    83,691.7 ns |  0.28 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Reversed           |    87,987.3 ns | 1,079.95 ns |   479.50 ns |    88,043.0 ns |  0.29 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 8192 | Reversed           |    74,812.0 ns | 1,250.61 ns |   654.09 ns |    74,511.2 ns |  0.25 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 8192 | Reversed           |   340,742.7 ns | 1,183.94 ns |   525.68 ns |   340,682.9 ns |  1.12 |    0.00 |    5 |         - |          NA |
| NaturalMergeSort         | 8192 | Reversed           |     8,045.3 ns |   884.38 ns |   392.67 ns |     8,010.8 ns |  0.03 |    0.00 |    1 |         - |          NA |
| TimSort                  | 8192 | Reversed           |     6,429.8 ns |   490.43 ns |   217.76 ns |     6,354.8 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Reversed           |     6,315.4 ns |   329.56 ns |   146.33 ns |     6,246.7 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Reversed           |     6,277.6 ns |   752.50 ns |   334.12 ns |     6,021.6 ns |  0.02 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 8192 | Reversed           |     7,642.5 ns |   376.15 ns |   196.74 ns |     7,551.9 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 8192 | Reversed           |     8,521.1 ns |   490.20 ns |   174.81 ns |     8,605.1 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Reversed           |     6,479.8 ns |   361.36 ns |   189.00 ns |     6,491.6 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Reversed           |     6,601.5 ns |   414.39 ns |   183.99 ns |     6,505.1 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Reversed           |     7,110.5 ns |   359.05 ns |   159.42 ns |     7,024.4 ns |  0.02 |    0.00 |    1 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **MergeSort**                | **8192** | **PipeOrgan**          |   **218,731.0 ns** | **2,477.23 ns** | **1,295.64 ns** |   **218,693.6 ns** |  **1.00** |    **0.01** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | PipeOrgan          |   223,877.8 ns | 1,612.92 ns |   843.59 ns |   223,975.1 ns |  1.02 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 8192 | PipeOrgan          |   112,843.6 ns | 3,399.46 ns | 1,777.99 ns |   112,544.6 ns |  0.52 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 8192 | PipeOrgan          |   121,740.9 ns | 1,983.64 ns | 1,037.48 ns |   121,802.7 ns |  0.56 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 8192 | PipeOrgan          |   159,031.8 ns |   847.28 ns |   376.20 ns |   158,887.9 ns |  0.73 |    0.00 |    6 |         - |          NA |
| RotateMergeSortRecursive | 8192 | PipeOrgan          |   186,405.3 ns | 1,645.21 ns |   730.48 ns |   186,300.3 ns |  0.85 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 8192 | PipeOrgan          |    98,481.4 ns |   880.68 ns |   391.03 ns |    98,581.6 ns |  0.45 |    0.00 |    5 |         - |          NA |
| BlockMergeSort           | 8192 | PipeOrgan          |   248,632.2 ns |   920.56 ns |   481.47 ns |   248,648.4 ns |  1.14 |    0.01 |    6 |         - |          NA |
| NaturalMergeSort         | 8192 | PipeOrgan          |    18,986.9 ns | 1,471.83 ns |   653.50 ns |    18,574.3 ns |  0.09 |    0.00 |    2 |         - |          NA |
| TimSort                  | 8192 | PipeOrgan          |    19,200.4 ns |   439.50 ns |   195.14 ns |    19,257.4 ns |  0.09 |    0.00 |    2 |         - |          NA |
| PowerSort                | 8192 | PipeOrgan          |    12,252.4 ns |   511.76 ns |   267.66 ns |    12,230.0 ns |  0.06 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | PipeOrgan          |    15,908.0 ns |   461.73 ns |   164.66 ns |    15,922.0 ns |  0.07 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | PipeOrgan          |    18,780.7 ns | 1,788.27 ns |   935.30 ns |    18,656.2 ns |  0.09 |    0.00 |    2 |         - |          NA |
| SpinSortVariant          | 8192 | PipeOrgan          |    19,663.7 ns |   810.49 ns |   359.86 ns |    19,603.6 ns |  0.09 |    0.00 |    2 |         - |          NA |
| Glidesort                | 8192 | PipeOrgan          |    34,730.5 ns |   692.39 ns |   307.43 ns |    34,709.7 ns |  0.16 |    0.00 |    3 |         - |          NA |
| Driftsort                | 8192 | PipeOrgan          |    10,418.8 ns |   506.24 ns |   264.77 ns |    10,391.6 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | PipeOrgan          |    79,642.9 ns | 1,202.81 ns |   629.09 ns |    79,753.6 ns |  0.36 |    0.00 |    4 |         - |          NA |

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
| **BitonicSort**             | **256**  | **Random**             |  **11,405.5 ns** |   **446.59 ns** |   **233.58 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Random             |  22,350.6 ns |    56.07 ns |    24.90 ns |  1.96 |    0.04 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Random             |  16,649.2 ns |   442.02 ns |   196.26 ns |  1.46 |    0.03 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **SingleElementMoved** |  **10,262.1 ns** |   **451.24 ns** |   **236.00 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | SingleElementMoved |  23,011.3 ns |   315.89 ns |   165.22 ns |  2.24 |    0.05 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | SingleElementMoved |  16,741.2 ns |   136.76 ns |    71.53 ns |  1.63 |    0.04 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **Sorted**             |  **10,074.8 ns** |   **287.78 ns** |   **127.78 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Sorted             |  22,942.5 ns |   192.83 ns |   100.85 ns |  2.28 |    0.03 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Sorted             |  16,760.7 ns |   190.82 ns |    84.73 ns |  1.66 |    0.02 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **Reversed**           |   **9,966.5 ns** |   **460.96 ns** |   **204.67 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Reversed           |  22,885.4 ns |   202.81 ns |    90.05 ns |  2.30 |    0.05 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Reversed           |  16,686.9 ns |    59.83 ns |    31.29 ns |  1.67 |    0.03 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **PipeOrgan**          |   **9,567.3 ns** |   **880.20 ns** |   **460.36 ns** |  **1.00** |    **0.06** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | PipeOrgan          |  22,726.4 ns |   133.50 ns |    59.28 ns |  2.38 |    0.11 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | PipeOrgan          |  16,677.2 ns |    46.06 ns |    20.45 ns |  1.75 |    0.08 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Random**             |  **94,465.4 ns** |   **321.88 ns** |   **142.92 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Random             | 123,938.5 ns |   773.82 ns |   343.58 ns |  1.31 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Random             | 102,485.5 ns |   549.91 ns |   244.16 ns |  1.08 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **SingleElementMoved** |  **58,964.1 ns** | **1,015.96 ns** |   **531.36 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | SingleElementMoved | 118,819.5 ns |   452.72 ns |   201.01 ns |  2.02 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | SingleElementMoved | 102,516.9 ns |   218.53 ns |   114.29 ns |  1.74 |    0.01 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Sorted**             |  **57,953.0 ns** | **1,006.82 ns** |   **526.58 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Sorted             | 118,852.8 ns | 1,019.07 ns |   532.99 ns |  2.05 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Sorted             | 102,580.1 ns |   363.48 ns |   190.11 ns |  1.77 |    0.02 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Reversed**           |  **57,392.3 ns** | **1,427.00 ns** |   **746.35 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Reversed           | 118,522.0 ns |   813.43 ns |   361.17 ns |  2.07 |    0.03 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Reversed           | 102,650.2 ns |   220.28 ns |   115.21 ns |  1.79 |    0.02 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **PipeOrgan**          |  **55,059.6 ns** | **4,857.19 ns** | **2,540.40 ns** |  **1.00** |    **0.06** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | PipeOrgan          | 116,617.5 ns |   603.57 ns |   315.68 ns |  2.12 |    0.09 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | PipeOrgan          | 102,759.6 ns |   515.94 ns |   269.85 ns |  1.87 |    0.08 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Random**             | **544,286.2 ns** | **2,826.42 ns** | **1,478.27 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Random             | 818,476.5 ns | 2,468.02 ns | 1,095.82 ns |  1.50 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Random             | 640,614.9 ns | 2,670.36 ns | 1,185.66 ns |  1.18 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **SingleElementMoved** | **321,473.2 ns** | **3,312.15 ns** | **1,732.32 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | SingleElementMoved | 591,357.1 ns | 1,444.97 ns |   641.57 ns |  1.84 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | SingleElementMoved | 585,209.8 ns |   556.55 ns |   291.08 ns |  1.82 |    0.01 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Sorted**             | **319,689.3 ns** | **4,248.76 ns** | **2,222.18 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Sorted             | 591,421.7 ns | 2,033.77 ns |   903.01 ns |  1.85 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Sorted             | 585,385.5 ns | 1,904.35 ns |   845.54 ns |  1.83 |    0.01 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Reversed**           | **314,624.8 ns** | **1,462.96 ns** |   **649.56 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Reversed           | 589,916.2 ns | 1,762.84 ns |   922.00 ns |  1.87 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Reversed           | 585,202.3 ns | 1,104.57 ns |   577.71 ns |  1.86 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **PipeOrgan**          | **308,238.9 ns** | **8,605.06 ns** | **4,500.62 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | PipeOrgan          | 580,037.5 ns | 1,770.92 ns |   926.22 ns |  1.88 |    0.03 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | PipeOrgan          | 585,258.7 ns |   858.10 ns |   448.80 ns |  1.90 |    0.03 |    2 |         - |          NA |

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

| Method                       | Size | Pattern            | Mean           | Error        | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------------- |----- |------------------- |---------------:|-------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **QuickSort**                    | **256**  | **Random**             |     **2,652.5 ns** |    **232.18 ns** |   **121.44 ns** |  **1.00** |    **0.06** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 256  | Random             |     3,279.8 ns |    361.78 ns |   189.22 ns |  1.24 |    0.08 |    1 |         - |          NA |
| QuickSortMedian3             | 256  | Random             |     4,399.2 ns |    191.64 ns |    85.09 ns |  1.66 |    0.08 |    1 |         - |          NA |
| QuickSortMedian9             | 256  | Random             |     3,766.1 ns |    128.68 ns |    45.89 ns |  1.42 |    0.06 |    1 |         - |          NA |
| DualPivotQuickSort           | 256  | Random             |     2,191.1 ns |     23.14 ns |    10.28 ns |  0.83 |    0.03 |    1 |         - |          NA |
| StableQuickSort              | 256  | Random             |    11,494.4 ns |    342.72 ns |   179.25 ns |  4.34 |    0.19 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Random             |     7,861.9 ns |    288.14 ns |   150.70 ns |  2.97 |    0.13 |    2 |         - |          NA |
| DestswapStableQuickSort      | 256  | Random             |     9,015.2 ns |    431.77 ns |   225.82 ns |  3.40 |    0.16 |    2 |         - |          NA |
| IntroSort                    | 256  | Random             |     1,944.8 ns |     45.71 ns |    20.30 ns |  0.73 |    0.03 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | Random             |     1,628.1 ns |     68.99 ns |    24.60 ns |  0.61 |    0.03 |    1 |         - |          NA |
| PDQSort                      | 256  | Random             |     1,872.6 ns |    391.05 ns |   204.52 ns |  0.71 |    0.08 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | Random             |     2,887.5 ns |     75.35 ns |    26.87 ns |  1.09 |    0.05 |    1 |         - |          NA |
| Ipnsort                      | 256  | Random             |     3,398.4 ns |     23.37 ns |     8.33 ns |  1.28 |    0.05 |    1 |         - |          NA |
| StdSort                      | 256  | Random             |     1,842.1 ns |    250.18 ns |   130.85 ns |  0.70 |    0.05 |    1 |         - |          NA |
| BlockQuickSort               | 256  | Random             |     2,356.8 ns |     99.28 ns |    44.08 ns |  0.89 |    0.04 |    1 |         - |          NA |
| DotnetSort                   | 256  | Random             |     1,984.3 ns |    373.34 ns |   195.26 ns |  0.75 |    0.08 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **256**  | **SingleElementMoved** |     **1,229.7 ns** |     **32.92 ns** |    **11.74 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 256  | SingleElementMoved |     5,161.7 ns |    390.68 ns |   173.47 ns |  4.20 |    0.14 |    3 |         - |          NA |
| QuickSortMedian3             | 256  | SingleElementMoved |     5,067.2 ns |     40.95 ns |    14.60 ns |  4.12 |    0.04 |    3 |         - |          NA |
| QuickSortMedian9             | 256  | SingleElementMoved |     4,228.8 ns |    120.06 ns |    53.31 ns |  3.44 |    0.05 |    2 |         - |          NA |
| DualPivotQuickSort           | 256  | SingleElementMoved |     3,797.3 ns |    545.09 ns |   285.09 ns |  3.09 |    0.22 |    2 |         - |          NA |
| StableQuickSort              | 256  | SingleElementMoved |     8,909.1 ns |    469.99 ns |   245.82 ns |  7.25 |    0.20 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 256  | SingleElementMoved |     5,377.2 ns |     23.02 ns |     8.21 ns |  4.37 |    0.04 |    3 |         - |          NA |
| DestswapStableQuickSort      | 256  | SingleElementMoved |    10,422.5 ns |    494.24 ns |   258.50 ns |  8.48 |    0.21 |    4 |         - |          NA |
| IntroSort                    | 256  | SingleElementMoved |       863.9 ns |     21.36 ns |    11.17 ns |  0.70 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | SingleElementMoved |     1,090.7 ns |     11.82 ns |     5.25 ns |  0.89 |    0.01 |    1 |         - |          NA |
| PDQSort                      | 256  | SingleElementMoved |     1,094.1 ns |     20.45 ns |     9.08 ns |  0.89 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | SingleElementMoved |     1,455.7 ns |     14.23 ns |     6.32 ns |  1.18 |    0.01 |    1 |         - |          NA |
| Ipnsort                      | 256  | SingleElementMoved |     3,246.3 ns |     20.11 ns |     8.93 ns |  2.64 |    0.02 |    2 |         - |          NA |
| StdSort                      | 256  | SingleElementMoved |     1,518.6 ns |     39.90 ns |    17.72 ns |  1.23 |    0.02 |    1 |         - |          NA |
| BlockQuickSort               | 256  | SingleElementMoved |     1,408.9 ns |     64.17 ns |    28.49 ns |  1.15 |    0.02 |    1 |         - |          NA |
| DotnetSort                   | 256  | SingleElementMoved |       965.4 ns |     32.15 ns |    11.47 ns |  0.79 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Sorted**             |     **1,242.0 ns** |    **811.14 ns** |   **424.24 ns** |  **1.09** |    **0.48** |    **6** |         **-** |          **NA** |
| QuickSort3way                | 256  | Sorted             |     6,821.3 ns |     64.19 ns |    28.50 ns |  6.01 |    1.68 |    9 |         - |          NA |
| QuickSortMedian3             | 256  | Sorted             |     6,409.0 ns |    332.20 ns |   173.75 ns |  5.65 |    1.58 |    9 |         - |          NA |
| QuickSortMedian9             | 256  | Sorted             |     4,911.3 ns |    304.54 ns |   135.22 ns |  4.33 |    1.21 |    8 |         - |          NA |
| DualPivotQuickSort           | 256  | Sorted             |     4,048.4 ns |     72.16 ns |    25.73 ns |  3.57 |    1.00 |    7 |         - |          NA |
| StableQuickSort              | 256  | Sorted             |     8,708.6 ns |    373.96 ns |   195.59 ns |  7.67 |    2.15 |   10 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Sorted             |     5,213.3 ns |    371.70 ns |   194.41 ns |  4.59 |    1.29 |    8 |         - |          NA |
| DestswapStableQuickSort      | 256  | Sorted             |    10,161.9 ns |    418.56 ns |   218.91 ns |  8.95 |    2.50 |   10 |         - |          NA |
| IntroSort                    | 256  | Sorted             |       299.0 ns |      4.11 ns |     1.82 ns |  0.26 |    0.07 |    2 |         - |          NA |
| IntroSortDotnet              | 256  | Sorted             |     1,040.0 ns |      7.87 ns |     3.50 ns |  0.92 |    0.26 |    6 |         - |          NA |
| PDQSort                      | 256  | Sorted             |       373.1 ns |      3.94 ns |     2.06 ns |  0.33 |    0.09 |    3 |         - |          NA |
| PDQSortBranchless            | 256  | Sorted             |       374.7 ns |      4.58 ns |     2.03 ns |  0.33 |    0.09 |    3 |         - |          NA |
| Ipnsort                      | 256  | Sorted             |       150.1 ns |      1.97 ns |     1.03 ns |  0.13 |    0.04 |    1 |         - |          NA |
| StdSort                      | 256  | Sorted             |       491.2 ns |      2.60 ns |     1.16 ns |  0.43 |    0.12 |    4 |         - |          NA |
| BlockQuickSort               | 256  | Sorted             |     1,201.9 ns |     20.67 ns |     7.37 ns |  1.06 |    0.30 |    6 |         - |          NA |
| DotnetSort                   | 256  | Sorted             |       836.8 ns |      9.78 ns |     4.34 ns |  0.74 |    0.21 |    5 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Reversed**           |       **975.2 ns** |     **34.20 ns** |    **12.20 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 256  | Reversed           |     5,579.4 ns |    686.82 ns |   359.22 ns |  5.72 |    0.35 |    6 |         - |          NA |
| QuickSortMedian3             | 256  | Reversed           |     7,721.4 ns |    377.34 ns |   197.36 ns |  7.92 |    0.21 |    7 |         - |          NA |
| QuickSortMedian9             | 256  | Reversed           |     4,934.1 ns |    327.46 ns |   171.27 ns |  5.06 |    0.18 |    6 |         - |          NA |
| DualPivotQuickSort           | 256  | Reversed           |     3,785.0 ns |     49.52 ns |    17.66 ns |  3.88 |    0.05 |    5 |         - |          NA |
| StableQuickSort              | 256  | Reversed           |     8,859.1 ns |    402.64 ns |   210.59 ns |  9.09 |    0.23 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Reversed           |     5,232.6 ns |     29.19 ns |    10.41 ns |  5.37 |    0.06 |    6 |         - |          NA |
| DestswapStableQuickSort      | 256  | Reversed           |    10,372.9 ns |    354.12 ns |   185.21 ns | 10.64 |    0.22 |    7 |         - |          NA |
| IntroSort                    | 256  | Reversed           |       560.1 ns |     42.88 ns |    19.04 ns |  0.57 |    0.02 |    2 |         - |          NA |
| IntroSortDotnet              | 256  | Reversed           |     1,447.5 ns |     13.60 ns |     7.12 ns |  1.48 |    0.02 |    4 |         - |          NA |
| PDQSort                      | 256  | Reversed           |       551.2 ns |      6.24 ns |     3.26 ns |  0.57 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 256  | Reversed           |       914.8 ns |     21.73 ns |     9.65 ns |  0.94 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 256  | Reversed           |       346.3 ns |    147.47 ns |    77.13 ns |  0.36 |    0.07 |    1 |         - |          NA |
| StdSort                      | 256  | Reversed           |       678.0 ns |     59.38 ns |    31.06 ns |  0.70 |    0.03 |    2 |         - |          NA |
| BlockQuickSort               | 256  | Reversed           |     1,440.7 ns |     19.77 ns |     8.78 ns |  1.48 |    0.02 |    4 |         - |          NA |
| DotnetSort                   | 256  | Reversed           |     1,469.4 ns |     61.20 ns |    27.17 ns |  1.51 |    0.03 |    4 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **256**  | **PipeOrgan**          |     **7,716.6 ns** |    **313.30 ns** |   **139.11 ns** |  **1.00** |    **0.02** |    **7** |         **-** |          **NA** |
| QuickSort3way                | 256  | PipeOrgan          |     5,193.0 ns |    441.43 ns |   230.88 ns |  0.67 |    0.03 |    5 |         - |          NA |
| QuickSortMedian3             | 256  | PipeOrgan          |     6,417.6 ns |     37.38 ns |    16.59 ns |  0.83 |    0.01 |    6 |         - |          NA |
| QuickSortMedian9             | 256  | PipeOrgan          |     4,130.6 ns |    265.75 ns |   117.99 ns |  0.54 |    0.02 |    4 |         - |          NA |
| DualPivotQuickSort           | 256  | PipeOrgan          |     1,984.7 ns |    148.47 ns |    65.92 ns |  0.26 |    0.01 |    2 |         - |          NA |
| StableQuickSort              | 256  | PipeOrgan          |     9,340.2 ns |    394.38 ns |   206.27 ns |  1.21 |    0.03 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 256  | PipeOrgan          |     5,175.7 ns |    372.91 ns |   165.57 ns |  0.67 |    0.02 |    5 |         - |          NA |
| DestswapStableQuickSort      | 256  | PipeOrgan          |    10,632.8 ns |    342.30 ns |   179.03 ns |  1.38 |    0.03 |    8 |         - |          NA |
| IntroSort                    | 256  | PipeOrgan          |     1,662.7 ns |     82.07 ns |    36.44 ns |  0.22 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | PipeOrgan          |     2,284.6 ns |    419.22 ns |   186.14 ns |  0.30 |    0.02 |    2 |         - |          NA |
| PDQSort                      | 256  | PipeOrgan          |     2,675.9 ns |  1,330.32 ns |   695.78 ns |  0.35 |    0.09 |    2 |         - |          NA |
| PDQSortBranchless            | 256  | PipeOrgan          |     2,970.7 ns |     73.45 ns |    26.19 ns |  0.39 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 256  | PipeOrgan          |     3,697.4 ns |    308.58 ns |   161.39 ns |  0.48 |    0.02 |    4 |         - |          NA |
| StdSort                      | 256  | PipeOrgan          |     2,328.4 ns |    447.44 ns |   234.02 ns |  0.30 |    0.03 |    2 |         - |          NA |
| BlockQuickSort               | 256  | PipeOrgan          |     4,366.8 ns |    501.01 ns |   262.04 ns |  0.57 |    0.03 |    4 |         - |          NA |
| DotnetSort                   | 256  | PipeOrgan          |     2,634.8 ns |    256.44 ns |   134.12 ns |  0.34 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Random**             |    **13,435.4 ns** |    **424.03 ns** |   **188.27 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Random             |    18,985.5 ns |    462.18 ns |   205.21 ns |  1.41 |    0.02 |    2 |         - |          NA |
| QuickSortMedian3             | 1024 | Random             |    24,173.2 ns |  3,201.51 ns | 1,421.49 ns |  1.80 |    0.10 |    2 |         - |          NA |
| QuickSortMedian9             | 1024 | Random             |    22,666.6 ns |  4,958.29 ns | 2,593.28 ns |  1.69 |    0.18 |    2 |         - |          NA |
| DualPivotQuickSort           | 1024 | Random             |    10,380.0 ns |    514.93 ns |   269.32 ns |  0.77 |    0.02 |    1 |         - |          NA |
| StableQuickSort              | 1024 | Random             |    83,466.4 ns |    658.62 ns |   292.43 ns |  6.21 |    0.08 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Random             |    37,740.6 ns |    142.52 ns |    50.82 ns |  2.81 |    0.04 |    3 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Random             |    39,777.9 ns |  1,173.94 ns |   613.99 ns |  2.96 |    0.06 |    3 |         - |          NA |
| IntroSort                    | 1024 | Random             |    11,444.7 ns |    469.53 ns |   245.57 ns |  0.85 |    0.02 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | Random             |     9,043.4 ns |    378.97 ns |   198.21 ns |  0.67 |    0.02 |    1 |         - |          NA |
| PDQSort                      | 1024 | Random             |     9,241.7 ns |    429.21 ns |   224.49 ns |  0.69 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | Random             |    13,639.3 ns |    438.52 ns |   229.36 ns |  1.02 |    0.02 |    1 |         - |          NA |
| Ipnsort                      | 1024 | Random             |    17,935.3 ns |    194.95 ns |    69.52 ns |  1.34 |    0.02 |    2 |         - |          NA |
| StdSort                      | 1024 | Random             |     8,952.4 ns |    504.29 ns |   263.75 ns |  0.67 |    0.02 |    1 |         - |          NA |
| BlockQuickSort               | 1024 | Random             |    12,937.6 ns |    530.79 ns |   235.67 ns |  0.96 |    0.02 |    1 |         - |          NA |
| DotnetSort                   | 1024 | Random             |    10,476.8 ns |    514.85 ns |   269.27 ns |  0.78 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **SingleElementMoved** |     **5,639.3 ns** |    **117.04 ns** |    **61.21 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 1024 | SingleElementMoved |    39,526.4 ns |  1,135.52 ns |   593.90 ns |  7.01 |    0.12 |    5 |         - |          NA |
| QuickSortMedian3             | 1024 | SingleElementMoved |    31,466.1 ns |    371.68 ns |   165.03 ns |  5.58 |    0.06 |    4 |         - |          NA |
| QuickSortMedian9             | 1024 | SingleElementMoved |    21,779.9 ns |    455.46 ns |   238.21 ns |  3.86 |    0.06 |    3 |         - |          NA |
| DualPivotQuickSort           | 1024 | SingleElementMoved |    21,261.3 ns |    527.04 ns |   234.01 ns |  3.77 |    0.05 |    3 |         - |          NA |
| StableQuickSort              | 1024 | SingleElementMoved |    42,997.5 ns |    660.56 ns |   345.48 ns |  7.63 |    0.10 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | SingleElementMoved |    26,685.0 ns |    851.12 ns |   445.15 ns |  4.73 |    0.09 |    4 |         - |          NA |
| DestswapStableQuickSort      | 1024 | SingleElementMoved |    43,011.6 ns |    214.14 ns |    95.08 ns |  7.63 |    0.08 |    5 |         - |          NA |
| IntroSort                    | 1024 | SingleElementMoved |     4,139.0 ns |     51.00 ns |    18.19 ns |  0.73 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | SingleElementMoved |     6,233.3 ns |    451.57 ns |   236.18 ns |  1.11 |    0.04 |    1 |         - |          NA |
| PDQSort                      | 1024 | SingleElementMoved |     4,902.5 ns |     34.65 ns |    15.39 ns |  0.87 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | SingleElementMoved |     6,213.2 ns |     72.55 ns |    25.87 ns |  1.10 |    0.01 |    1 |         - |          NA |
| Ipnsort                      | 1024 | SingleElementMoved |    16,557.7 ns |    271.05 ns |   120.35 ns |  2.94 |    0.04 |    2 |         - |          NA |
| StdSort                      | 1024 | SingleElementMoved |     6,764.8 ns |     29.39 ns |    15.37 ns |  1.20 |    0.01 |    1 |         - |          NA |
| BlockQuickSort               | 1024 | SingleElementMoved |     7,743.7 ns |    262.34 ns |   137.21 ns |  1.37 |    0.03 |    1 |         - |          NA |
| DotnetSort                   | 1024 | SingleElementMoved |     5,296.4 ns |    236.01 ns |    84.16 ns |  0.94 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Sorted**             |     **4,306.3 ns** |    **435.15 ns** |   **227.59 ns** |  **1.00** |    **0.07** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Sorted             |    52,554.1 ns |    162.13 ns |    57.82 ns | 12.23 |    0.60 |    8 |         - |          NA |
| QuickSortMedian3             | 1024 | Sorted             |    43,108.6 ns |    122.96 ns |    54.59 ns | 10.03 |    0.49 |    7 |         - |          NA |
| QuickSortMedian9             | 1024 | Sorted             |    22,381.4 ns |  1,057.07 ns |   469.35 ns |  5.21 |    0.27 |    6 |         - |          NA |
| DualPivotQuickSort           | 1024 | Sorted             |    21,576.8 ns |    197.01 ns |   103.04 ns |  5.02 |    0.25 |    6 |         - |          NA |
| StableQuickSort              | 1024 | Sorted             |    42,697.7 ns |    484.64 ns |   253.48 ns |  9.94 |    0.49 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Sorted             |    24,926.3 ns |    883.98 ns |   392.49 ns |  5.80 |    0.30 |    6 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Sorted             |    43,474.0 ns |    659.91 ns |   293.00 ns | 10.12 |    0.50 |    7 |         - |          NA |
| IntroSort                    | 1024 | Sorted             |     1,120.4 ns |     31.48 ns |    11.22 ns |  0.26 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 1024 | Sorted             |     4,966.1 ns |    539.57 ns |   282.21 ns |  1.16 |    0.08 |    4 |         - |          NA |
| PDQSort                      | 1024 | Sorted             |     1,328.1 ns |      7.61 ns |     3.38 ns |  0.31 |    0.02 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | Sorted             |     1,331.2 ns |      7.90 ns |     2.82 ns |  0.31 |    0.02 |    2 |         - |          NA |
| Ipnsort                      | 1024 | Sorted             |       519.2 ns |      2.40 ns |     1.07 ns |  0.12 |    0.01 |    1 |         - |          NA |
| StdSort                      | 1024 | Sorted             |     2,108.5 ns |    314.46 ns |   139.62 ns |  0.49 |    0.04 |    3 |         - |          NA |
| BlockQuickSort               | 1024 | Sorted             |     6,248.7 ns |     19.78 ns |     7.06 ns |  1.45 |    0.07 |    5 |         - |          NA |
| DotnetSort                   | 1024 | Sorted             |     4,015.0 ns |     14.41 ns |     5.14 ns |  0.93 |    0.05 |    4 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Reversed**           |     **4,773.1 ns** |    **440.43 ns** |   **230.36 ns** |  **1.00** |    **0.06** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Reversed           |    38,384.2 ns |    390.62 ns |   204.30 ns |  8.06 |    0.36 |    8 |         - |          NA |
| QuickSortMedian3             | 1024 | Reversed           |    52,320.4 ns |    512.52 ns |   268.06 ns | 10.98 |    0.49 |    9 |         - |          NA |
| QuickSortMedian9             | 1024 | Reversed           |    23,210.6 ns |  1,530.53 ns |   800.50 ns |  4.87 |    0.27 |    7 |         - |          NA |
| DualPivotQuickSort           | 1024 | Reversed           |    20,236.8 ns |    977.52 ns |   511.26 ns |  4.25 |    0.21 |    7 |         - |          NA |
| StableQuickSort              | 1024 | Reversed           |    42,367.4 ns |    437.96 ns |   229.06 ns |  8.89 |    0.39 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Reversed           |    25,216.3 ns |    252.70 ns |   132.17 ns |  5.29 |    0.23 |    7 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Reversed           |    43,368.6 ns |    236.84 ns |   123.87 ns |  9.10 |    0.40 |    8 |         - |          NA |
| IntroSort                    | 1024 | Reversed           |     3,398.3 ns |    701.02 ns |   366.65 ns |  0.71 |    0.08 |    4 |         - |          NA |
| IntroSortDotnet              | 1024 | Reversed           |     7,658.7 ns |    203.18 ns |    90.21 ns |  1.61 |    0.07 |    6 |         - |          NA |
| PDQSort                      | 1024 | Reversed           |     1,904.7 ns |     17.51 ns |     7.78 ns |  0.40 |    0.02 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | Reversed           |     3,202.6 ns |    421.96 ns |   220.69 ns |  0.67 |    0.05 |    4 |         - |          NA |
| Ipnsort                      | 1024 | Reversed           |       802.9 ns |      4.03 ns |     1.79 ns |  0.17 |    0.01 |    1 |         - |          NA |
| StdSort                      | 1024 | Reversed           |     2,611.2 ns |    540.74 ns |   282.82 ns |  0.55 |    0.06 |    3 |         - |          NA |
| BlockQuickSort               | 1024 | Reversed           |     7,783.3 ns |    327.37 ns |   145.36 ns |  1.63 |    0.08 |    6 |         - |          NA |
| DotnetSort                   | 1024 | Reversed           |     7,880.1 ns |    367.23 ns |   192.07 ns |  1.65 |    0.08 |    6 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **PipeOrgan**          |    **97,944.9 ns** |    **359.24 ns** |   **187.89 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| QuickSort3way                | 1024 | PipeOrgan          |    31,157.2 ns |    399.12 ns |   177.21 ns |  0.32 |    0.00 |    4 |         - |          NA |
| QuickSortMedian3             | 1024 | PipeOrgan          |    37,638.3 ns |    207.67 ns |    92.21 ns |  0.38 |    0.00 |    5 |         - |          NA |
| QuickSortMedian9             | 1024 | PipeOrgan          |    22,251.6 ns |    514.61 ns |   269.15 ns |  0.23 |    0.00 |    3 |         - |          NA |
| DualPivotQuickSort           | 1024 | PipeOrgan          |     9,398.5 ns |    765.40 ns |   400.32 ns |  0.10 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 1024 | PipeOrgan          |    45,750.5 ns |  1,172.69 ns |   520.68 ns |  0.47 |    0.01 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | PipeOrgan          |    24,006.7 ns |    714.13 ns |   373.50 ns |  0.25 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 1024 | PipeOrgan          |    49,402.0 ns |    407.37 ns |   213.06 ns |  0.50 |    0.00 |    6 |         - |          NA |
| IntroSort                    | 1024 | PipeOrgan          |    10,939.7 ns |    663.47 ns |   294.58 ns |  0.11 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | PipeOrgan          |    13,882.8 ns |    301.39 ns |   157.63 ns |  0.14 |    0.00 |    2 |         - |          NA |
| PDQSort                      | 1024 | PipeOrgan          |     8,852.9 ns |    564.76 ns |   295.38 ns |  0.09 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | PipeOrgan          |    16,078.6 ns |    365.11 ns |   162.11 ns |  0.16 |    0.00 |    2 |         - |          NA |
| Ipnsort                      | 1024 | PipeOrgan          |    19,532.9 ns |    390.62 ns |   204.30 ns |  0.20 |    0.00 |    3 |         - |          NA |
| StdSort                      | 1024 | PipeOrgan          |    13,390.4 ns |    651.62 ns |   340.81 ns |  0.14 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | PipeOrgan          |    23,359.1 ns |    629.72 ns |   329.36 ns |  0.24 |    0.00 |    3 |         - |          NA |
| DotnetSort                   | 1024 | PipeOrgan          |    15,701.2 ns |    998.13 ns |   522.04 ns |  0.16 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Random**             |   **421,381.9 ns** |  **2,615.99 ns** | **1,368.21 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Random             |   439,679.6 ns |  1,870.52 ns |   830.52 ns |  1.04 |    0.00 |    2 |         - |          NA |
| QuickSortMedian3             | 8192 | Random             |   534,211.4 ns |  2,523.88 ns | 1,320.04 ns |  1.27 |    0.00 |    2 |         - |          NA |
| QuickSortMedian9             | 8192 | Random             |   517,863.9 ns |  2,189.61 ns | 1,145.21 ns |  1.23 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort           | 8192 | Random             |   349,524.1 ns |  1,390.41 ns |   727.21 ns |  0.83 |    0.00 |    2 |         - |          NA |
| StableQuickSort              | 8192 | Random             | 1,149,105.5 ns |  1,731.76 ns |   768.91 ns |  2.73 |    0.01 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Random             |   961,200.1 ns |  3,137.31 ns | 1,392.99 ns |  2.28 |    0.01 |    3 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Random             |   841,841.2 ns |  1,908.72 ns |   998.30 ns |  2.00 |    0.01 |    3 |         - |          NA |
| IntroSort                    | 8192 | Random             |   364,751.0 ns |  2,722.25 ns | 1,423.79 ns |  0.87 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet              | 8192 | Random             |   355,614.3 ns |  1,222.56 ns |   542.82 ns |  0.84 |    0.00 |    2 |         - |          NA |
| PDQSort                      | 8192 | Random             |   344,564.4 ns |  1,215.62 ns |   635.79 ns |  0.82 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | Random             |   194,689.0 ns |  1,205.29 ns |   535.16 ns |  0.46 |    0.00 |    1 |         - |          NA |
| Ipnsort                      | 8192 | Random             |   199,159.1 ns |  1,371.99 ns |   717.58 ns |  0.47 |    0.00 |    1 |         - |          NA |
| StdSort                      | 8192 | Random             |   337,078.1 ns |  1,632.71 ns |   853.94 ns |  0.80 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | Random             |   421,645.1 ns |    869.97 ns |   386.27 ns |  1.00 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 8192 | Random             |   339,234.4 ns |  5,038.15 ns | 2,635.05 ns |  0.81 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **SingleElementMoved** |    **54,150.9 ns** |    **675.83 ns** |   **300.07 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 8192 | SingleElementMoved |   854,688.5 ns |  3,711.14 ns | 1,941.00 ns | 15.78 |    0.09 |   10 |         - |          NA |
| QuickSortMedian3             | 8192 | SingleElementMoved |   571,355.3 ns |  3,109.20 ns | 1,626.17 ns | 10.55 |    0.06 |    9 |         - |          NA |
| QuickSortMedian9             | 8192 | SingleElementMoved |   215,412.4 ns |  3,253.33 ns | 1,701.56 ns |  3.98 |    0.04 |    6 |         - |          NA |
| DualPivotQuickSort           | 8192 | SingleElementMoved |   140,177.3 ns |  2,207.44 ns | 1,154.54 ns |  2.59 |    0.02 |    4 |         - |          NA |
| StableQuickSort              | 8192 | SingleElementMoved |   434,931.7 ns |  1,882.89 ns |   984.79 ns |  8.03 |    0.04 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | SingleElementMoved |   260,121.0 ns |  1,354.89 ns |   708.63 ns |  4.80 |    0.03 |    7 |         - |          NA |
| DestswapStableQuickSort      | 8192 | SingleElementMoved |   382,597.4 ns |  3,428.37 ns | 1,793.10 ns |  7.07 |    0.05 |    8 |         - |          NA |
| IntroSort                    | 8192 | SingleElementMoved |    41,818.7 ns |  3,521.07 ns | 1,841.59 ns |  0.77 |    0.03 |    1 |         - |          NA |
| IntroSortDotnet              | 8192 | SingleElementMoved |    61,208.5 ns |    813.11 ns |   425.27 ns |  1.13 |    0.01 |    2 |         - |          NA |
| PDQSort                      | 8192 | SingleElementMoved |    42,487.8 ns |  1,079.33 ns |   564.51 ns |  0.78 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 8192 | SingleElementMoved |    53,774.0 ns |  1,722.39 ns |   764.75 ns |  0.99 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 8192 | SingleElementMoved |   174,062.4 ns |    621.07 ns |   324.83 ns |  3.21 |    0.02 |    5 |         - |          NA |
| StdSort                      | 8192 | SingleElementMoved |    63,379.4 ns |    876.03 ns |   458.18 ns |  1.17 |    0.01 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | SingleElementMoved |    81,586.0 ns |  4,947.13 ns | 2,196.56 ns |  1.51 |    0.04 |    3 |         - |          NA |
| DotnetSort                   | 8192 | SingleElementMoved |    59,064.7 ns |  8,669.71 ns | 4,534.42 ns |  1.09 |    0.08 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Sorted**             |    **42,302.2 ns** |  **1,385.62 ns** |   **724.71 ns** |  **1.00** |    **0.02** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Sorted             | 1,173,677.1 ns |  5,696.02 ns | 2,979.13 ns | 27.75 |    0.45 |   12 |         - |          NA |
| QuickSortMedian3             | 8192 | Sorted             |   887,539.1 ns |  2,754.88 ns |   982.42 ns | 20.99 |    0.34 |   11 |         - |          NA |
| QuickSortMedian9             | 8192 | Sorted             |   211,827.1 ns |  5,947.93 ns | 3,110.88 ns |  5.01 |    0.11 |    8 |         - |          NA |
| DualPivotQuickSort           | 8192 | Sorted             |   152,332.2 ns |  2,399.10 ns | 1,254.78 ns |  3.60 |    0.06 |    7 |         - |          NA |
| StableQuickSort              | 8192 | Sorted             |   433,734.3 ns |  1,930.35 ns | 1,009.61 ns | 10.26 |    0.17 |   10 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Sorted             |   257,092.3 ns | 10,017.60 ns | 5,239.40 ns |  6.08 |    0.15 |    9 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Sorted             |   382,953.4 ns |  3,254.87 ns | 1,445.18 ns |  9.06 |    0.15 |   10 |         - |          NA |
| IntroSort                    | 8192 | Sorted             |     8,697.0 ns |    600.48 ns |   314.06 ns |  0.21 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 8192 | Sorted             |    47,865.5 ns |    942.62 ns |   418.53 ns |  1.13 |    0.02 |    5 |         - |          NA |
| PDQSort                      | 8192 | Sorted             |    10,424.6 ns |    309.04 ns |   137.22 ns |  0.25 |    0.01 |    3 |         - |          NA |
| PDQSortBranchless            | 8192 | Sorted             |    10,906.9 ns |    282.52 ns |   125.44 ns |  0.26 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 8192 | Sorted             |     4,447.5 ns |    449.86 ns |   235.29 ns |  0.11 |    0.01 |    1 |         - |          NA |
| StdSort                      | 8192 | Sorted             |    14,868.9 ns |  1,002.22 ns |   444.99 ns |  0.35 |    0.01 |    4 |         - |          NA |
| BlockQuickSort               | 8192 | Sorted             |    68,825.8 ns |  1,859.87 ns |   825.79 ns |  1.63 |    0.03 |    6 |         - |          NA |
| DotnetSort                   | 8192 | Sorted             |    45,107.0 ns |  2,589.55 ns | 1,354.38 ns |  1.07 |    0.03 |    5 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Reversed**           |    **45,577.8 ns** |    **852.02 ns** |   **378.30 ns** |  **1.00** |    **0.01** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Reversed           |   834,841.5 ns |  5,029.79 ns | 2,233.26 ns | 18.32 |    0.15 |   11 |         - |          NA |
| QuickSortMedian3             | 8192 | Reversed           | 1,124,727.0 ns |  6,331.02 ns | 3,311.25 ns | 24.68 |    0.20 |   12 |         - |          NA |
| QuickSortMedian9             | 8192 | Reversed           |   205,212.9 ns |  5,077.61 ns | 2,655.69 ns |  4.50 |    0.07 |    8 |         - |          NA |
| DualPivotQuickSort           | 8192 | Reversed           |   143,588.2 ns |  2,719.05 ns | 1,207.27 ns |  3.15 |    0.03 |    7 |         - |          NA |
| StableQuickSort              | 8192 | Reversed           |   432,888.6 ns |  1,717.67 ns |   762.65 ns |  9.50 |    0.08 |   10 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Reversed           |   254,764.0 ns |  3,678.75 ns | 1,924.06 ns |  5.59 |    0.06 |    9 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Reversed           |   387,307.1 ns |  2,827.59 ns | 1,255.47 ns |  8.50 |    0.07 |   10 |         - |          NA |
| IntroSort                    | 8192 | Reversed           |    25,090.2 ns |  1,690.54 ns |   884.19 ns |  0.55 |    0.02 |    4 |         - |          NA |
| IntroSortDotnet              | 8192 | Reversed           |    76,493.0 ns |    462.98 ns |   205.57 ns |  1.68 |    0.01 |    6 |         - |          NA |
| PDQSort                      | 8192 | Reversed           |    14,804.3 ns |  1,288.19 ns |   571.96 ns |  0.32 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | Reversed           |    23,194.3 ns |  1,113.12 ns |   582.18 ns |  0.51 |    0.01 |    4 |         - |          NA |
| Ipnsort                      | 8192 | Reversed           |     6,423.6 ns |    485.12 ns |   215.40 ns |  0.14 |    0.00 |    1 |         - |          NA |
| StdSort                      | 8192 | Reversed           |    18,829.1 ns |    963.73 ns |   427.90 ns |  0.41 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 8192 | Reversed           |    75,952.0 ns |  1,467.89 ns |   767.73 ns |  1.67 |    0.02 |    6 |         - |          NA |
| DotnetSort                   | 8192 | Reversed           |    89,208.4 ns |  2,202.93 ns |   978.12 ns |  1.96 |    0.03 |    6 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **PipeOrgan**          | **5,386,819.4 ns** | **15,399.41 ns** | **6,837.44 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 8192 | PipeOrgan          |   452,941.7 ns |  2,768.68 ns | 1,448.07 ns |  0.08 |    0.00 |    3 |         - |          NA |
| QuickSortMedian3             | 8192 | PipeOrgan          |   495,110.7 ns |  3,233.99 ns | 1,153.27 ns |  0.09 |    0.00 |    3 |         - |          NA |
| QuickSortMedian9             | 8192 | PipeOrgan          |   281,167.4 ns |  2,787.62 ns | 1,457.98 ns |  0.05 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort           | 8192 | PipeOrgan          |   123,675.6 ns |  3,005.02 ns | 1,571.68 ns |  0.02 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 8192 | PipeOrgan          |   471,357.8 ns |  2,831.48 ns | 1,480.92 ns |  0.09 |    0.00 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | PipeOrgan          |   238,149.0 ns |  4,144.95 ns | 2,167.89 ns |  0.04 |    0.00 |    2 |         - |          NA |
| DestswapStableQuickSort      | 8192 | PipeOrgan          |   460,594.5 ns |  2,391.75 ns | 1,250.93 ns |  0.09 |    0.00 |    3 |         - |          NA |
| IntroSort                    | 8192 | PipeOrgan          |   268,943.8 ns |  9,442.03 ns | 4,938.36 ns |  0.05 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet              | 8192 | PipeOrgan          |   360,976.0 ns |  3,960.58 ns | 1,758.52 ns |  0.07 |    0.00 |    2 |         - |          NA |
| PDQSort                      | 8192 | PipeOrgan          |   117,976.8 ns |  2,262.48 ns | 1,183.32 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 8192 | PipeOrgan          |   201,511.9 ns |  1,540.16 ns |   683.84 ns |  0.04 |    0.00 |    2 |         - |          NA |
| Ipnsort                      | 8192 | PipeOrgan          |   217,998.0 ns |  1,561.73 ns |   693.42 ns |  0.04 |    0.00 |    2 |         - |          NA |
| StdSort                      | 8192 | PipeOrgan          |   293,326.5 ns |  4,596.17 ns | 2,403.89 ns |  0.05 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | PipeOrgan          |   255,484.1 ns |  1,546.61 ns |   808.91 ns |  0.05 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 8192 | PipeOrgan          |   348,992.2 ns |  3,286.29 ns | 1,718.79 ns |  0.06 |    0.00 |    2 |         - |          NA |

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

| Method              | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **SelectionSort**       | **256**  | **Random**             |    **26,230.3 ns** |     **176.6 ns** |     **78.39 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Random             |    24,877.0 ns |     422.9 ns |    187.79 ns |  0.95 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | Random             |    90,506.7 ns |   2,621.3 ns |  1,163.86 ns |  3.45 |    0.04 |    3 |         - |          NA |
| PancakeSort         | 256  | Random             |    43,034.6 ns |     581.5 ns |    304.11 ns |  1.64 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **SingleElementMoved** |    **21,996.2 ns** |     **213.2 ns** |     **94.64 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | SingleElementMoved |    21,148.8 ns |     216.3 ns |     77.14 ns |  0.96 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | SingleElementMoved |    50,611.8 ns |   2,009.0 ns |    892.00 ns |  2.30 |    0.04 |    2 |         - |          NA |
| PancakeSort         | 256  | SingleElementMoved |    19,873.5 ns |     314.3 ns |    164.37 ns |  0.90 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **Sorted**             |    **21,949.3 ns** |     **197.1 ns** |     **70.29 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Sorted             |    16,384.9 ns |     560.8 ns |    249.02 ns |  0.75 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | Sorted             |    32,016.6 ns |     184.7 ns |     81.99 ns |  1.46 |    0.01 |    3 |         - |          NA |
| PancakeSort         | 256  | Sorted             |    15,043.8 ns |     300.6 ns |    107.18 ns |  0.69 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **Reversed**           |    **19,502.1 ns** |     **350.9 ns** |    **125.15 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Reversed           |    16,389.7 ns |     209.8 ns |     93.16 ns |  0.84 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | Reversed           |    44,081.0 ns |     237.7 ns |    105.54 ns |  2.26 |    0.01 |    2 |         - |          NA |
| PancakeSort         | 256  | Reversed           |    20,217.4 ns |   4,058.0 ns |  2,122.39 ns |  1.04 |    0.10 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **PipeOrgan**          |    **25,115.8 ns** |     **641.1 ns** |    **335.29 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | PipeOrgan          |    17,778.7 ns |   1,270.6 ns |    664.54 ns |  0.71 |    0.03 |    1 |         - |          NA |
| CycleSort           | 256  | PipeOrgan          |    61,517.8 ns |   2,630.1 ns |  1,375.59 ns |  2.45 |    0.06 |    4 |         - |          NA |
| PancakeSort         | 256  | PipeOrgan          |    33,216.3 ns |     199.5 ns |     88.59 ns |  1.32 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Random**             |   **367,997.2 ns** |   **1,080.1 ns** |    **564.89 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Random             |   357,700.5 ns |     806.7 ns |    421.91 ns |  0.97 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | Random             | 1,656,061.5 ns |  11,135.5 ns |  4,944.25 ns |  4.50 |    0.01 |    3 |         - |          NA |
| PancakeSort         | 1024 | Random             |   621,258.6 ns |   1,268.7 ns |    663.57 ns |  1.69 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **SingleElementMoved** |   **333,656.6 ns** |   **3,166.3 ns** |  **1,405.86 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | SingleElementMoved |   354,042.3 ns | 113,254.4 ns | 59,234.23 ns |  1.06 |    0.17 |    1 |         - |          NA |
| CycleSort           | 1024 | SingleElementMoved |   770,453.8 ns |   6,851.8 ns |  3,583.65 ns |  2.31 |    0.01 |    2 |         - |          NA |
| PancakeSort         | 1024 | SingleElementMoved |   294,741.5 ns |   2,960.7 ns |  1,314.57 ns |  0.88 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Sorted**             |   **332,740.2 ns** |   **1,054.0 ns** |    **467.96 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Sorted             |   249,370.6 ns |   2,781.8 ns |  1,454.94 ns |  0.75 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | Sorted             |   496,063.4 ns |   1,874.2 ns |    980.25 ns |  1.49 |    0.00 |    3 |         - |          NA |
| PancakeSort         | 1024 | Sorted             |   224,430.2 ns |   1,501.4 ns |    785.27 ns |  0.67 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Reversed**           |   **316,558.8 ns** |  **10,824.0 ns** |  **4,805.92 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Reversed           |   249,261.7 ns |   1,138.8 ns |    595.64 ns |  0.79 |    0.01 |    1 |         - |          NA |
| CycleSort           | 1024 | Reversed           |   666,986.6 ns |   2,358.6 ns |  1,233.58 ns |  2.11 |    0.03 |    3 |         - |          NA |
| PancakeSort         | 1024 | Reversed           |   317,752.3 ns |   9,495.1 ns |  4,966.11 ns |  1.00 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **PipeOrgan**          |   **349,921.3 ns** |   **3,692.6 ns** |  **1,931.29 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | PipeOrgan          |   254,694.4 ns |   3,172.5 ns |  1,659.25 ns |  0.73 |    0.01 |    1 |         - |          NA |
| CycleSort           | 1024 | PipeOrgan          |   907,664.6 ns |  12,408.2 ns |  5,509.34 ns |  2.59 |    0.02 |    4 |         - |          NA |
| PancakeSort         | 1024 | PipeOrgan          |   501,192.1 ns |   1,780.8 ns |    790.69 ns |  1.43 |    0.01 |    3 |         - |          NA |

### StringBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method             | Size | Pattern            | Mean               | Error          | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------- |----- |------------------- |-------------------:|---------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **QuickSort**          | **256**  | **Random**             |       **192,654.5 ns** |       **827.9 ns** |     **367.6 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| QuickSort3way      | 256  | Random             |       162,443.2 ns |     1,956.5 ns |   1,023.3 ns |  0.84 |    0.01 |    1 |         - |          NA |
| QuickSortMedian3   | 256  | Random             |       173,130.7 ns |     1,871.4 ns |     978.8 ns |  0.90 |    0.01 |    1 |         - |          NA |
| QuickSortMedian9   | 256  | Random             |       175,631.9 ns |       935.4 ns |     489.2 ns |  0.91 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort | 256  | Random             |       201,063.8 ns |       526.9 ns |     234.0 ns |  1.04 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 256  | Random             |       366,402.8 ns |   171,251.7 ns |  89,568.0 ns |  1.90 |    0.44 |    2 |         - |          NA |
| IntroSort          | 256  | Random             |       179,733.5 ns |     1,011.8 ns |     529.2 ns |  0.93 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | Random             |       164,336.3 ns |     1,427.1 ns |     746.4 ns |  0.85 |    0.00 |    1 |         - |          NA |
| PDQSort            | 256  | Random             |       190,770.4 ns |       577.1 ns |     256.2 ns |  0.99 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | Random             |       188,337.1 ns |       963.1 ns |     503.7 ns |  0.98 |    0.00 |    1 |         - |          NA |
| Ipnsort            | 256  | Random             |       171,925.4 ns |     1,506.9 ns |     788.2 ns |  0.89 |    0.00 |    1 |         - |          NA |
| StdSort            | 256  | Random             |       191,434.0 ns |     1,701.4 ns |     889.9 ns |  0.99 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 256  | Random             |       176,579.6 ns |     1,035.6 ns |     541.6 ns |  0.92 |    0.00 |    1 |         - |          NA |
| DotnetSort         | 256  | Random             |       160,044.0 ns |     1,155.6 ns |     513.1 ns |  0.83 |    0.00 |    1 |         - |          NA |
|      |                    |                    |                |              |       |         |      |           |             |
| **QuickSort**          | **256**  | **SingleElementMoved** |       **135,066.8 ns** |       **331.6 ns** |     **173.4 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| QuickSort3way      | 256  | SingleElementMoved |       202,785.0 ns |     1,814.8 ns |     949.2 ns |  1.50 |    0.01 |    2 |         - |          NA |
| QuickSortMedian3   | 256  | SingleElementMoved |       177,424.0 ns |     1,510.4 ns |     670.6 ns |  1.31 |    0.00 |    2 |         - |          NA |
| QuickSortMedian9   | 256  | SingleElementMoved |       168,440.6 ns |       704.2 ns |     312.7 ns |  1.25 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort | 256  | SingleElementMoved |       321,790.4 ns |     1,692.7 ns |     885.3 ns |  2.38 |    0.01 |    3 |         - |          NA |
| StableQuickSort    | 256  | SingleElementMoved |       242,979.4 ns |       659.0 ns |     292.6 ns |  1.80 |    0.00 |    2 |         - |          NA |
| IntroSort          | 256  | SingleElementMoved |        92,231.6 ns |       980.0 ns |     512.6 ns |  0.68 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | SingleElementMoved |       116,297.8 ns |       664.2 ns |     294.9 ns |  0.86 |    0.00 |    1 |         - |          NA |
| PDQSort            | 256  | SingleElementMoved |       124,398.0 ns |       936.8 ns |     416.0 ns |  0.92 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | SingleElementMoved |       124,410.0 ns |       687.6 ns |     305.3 ns |  0.92 |    0.00 |    1 |         - |          NA |
| Ipnsort            | 256  | SingleElementMoved |       162,240.4 ns |       752.3 ns |     393.5 ns |  1.20 |    0.00 |    2 |         - |          NA |
| StdSort            | 256  | SingleElementMoved |       128,885.2 ns |       987.7 ns |     352.2 ns |  0.95 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 256  | SingleElementMoved |       107,036.2 ns |       604.7 ns |     268.5 ns |  0.79 |    0.00 |    1 |         - |          NA |
| DotnetSort         | 256  | SingleElementMoved |       115,879.7 ns |       354.3 ns |     157.3 ns |  0.86 |    0.00 |    1 |         - |          NA |
|      |                    |                    |                |              |       |         |      |           |             |
| **QuickSort**          | **256**  | **Sorted**             |       **114,618.8 ns** |       **447.7 ns** |     **234.2 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way      | 256  | Sorted             |       250,257.3 ns |       891.7 ns |     395.9 ns |  2.18 |    0.01 |    5 |         - |          NA |
| QuickSortMedian3   | 256  | Sorted             |       228,544.0 ns |     1,033.4 ns |     540.5 ns |  1.99 |    0.01 |    5 |         - |          NA |
| QuickSortMedian9   | 256  | Sorted             |       183,848.5 ns |     1,521.5 ns |     795.8 ns |  1.60 |    0.01 |    4 |         - |          NA |
| DualPivotQuickSort | 256  | Sorted             |       405,320.2 ns |     2,613.9 ns |   1,367.1 ns |  3.54 |    0.01 |    6 |         - |          NA |
| StableQuickSort    | 256  | Sorted             |       253,108.4 ns |     1,595.0 ns |     834.2 ns |  2.21 |    0.01 |    5 |         - |          NA |
| IntroSort          | 256  | Sorted             |        35,766.4 ns |       382.6 ns |     200.1 ns |  0.31 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet    | 256  | Sorted             |        91,333.0 ns |     1,372.2 ns |     609.3 ns |  0.80 |    0.01 |    3 |         - |          NA |
| PDQSort            | 256  | Sorted             |        37,367.5 ns |       502.9 ns |     263.0 ns |  0.33 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless  | 256  | Sorted             |        37,255.1 ns |       192.3 ns |     100.6 ns |  0.33 |    0.00 |    2 |         - |          NA |
| Ipnsort            | 256  | Sorted             |        18,284.4 ns |       317.1 ns |     165.9 ns |  0.16 |    0.00 |    1 |         - |          NA |
| StdSort            | 256  | Sorted             |        36,736.6 ns |       527.8 ns |     234.3 ns |  0.32 |    0.00 |    2 |         - |          NA |
| BlockQuickSort     | 256  | Sorted             |       100,445.8 ns |     1,023.0 ns |     535.1 ns |  0.88 |    0.00 |    3 |         - |          NA |
| DotnetSort         | 256  | Sorted             |        86,491.6 ns |     1,662.3 ns |     869.4 ns |  0.75 |    0.01 |    3 |         - |          NA |
|      |                    |                    |                |              |       |         |      |           |             |
| **QuickSort**          | **256**  | **Reversed**           |       **114,403.2 ns** |     **1,470.4 ns** |     **652.9 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| QuickSort3way      | 256  | Reversed           |       203,141.0 ns |     1,991.3 ns |   1,041.5 ns |  1.78 |    0.01 |    4 |         - |          NA |
| QuickSortMedian3   | 256  | Reversed           |       269,929.3 ns |     1,758.0 ns |     919.5 ns |  2.36 |    0.01 |    5 |         - |          NA |
| QuickSortMedian9   | 256  | Reversed           |       192,654.6 ns |       839.8 ns |     439.2 ns |  1.68 |    0.01 |    4 |         - |          NA |
| DualPivotQuickSort | 256  | Reversed           |       384,071.4 ns |     5,303.2 ns |   2,773.7 ns |  3.36 |    0.03 |    6 |         - |          NA |
| StableQuickSort    | 256  | Reversed           |       267,766.8 ns |     1,548.4 ns |     809.9 ns |  2.34 |    0.01 |    5 |         - |          NA |
| IntroSort          | 256  | Reversed           |        55,187.8 ns |     1,927.8 ns |   1,008.3 ns |  0.48 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet    | 256  | Reversed           |       142,968.0 ns |     1,093.2 ns |     485.4 ns |  1.25 |    0.01 |    3 |         - |          NA |
| PDQSort            | 256  | Reversed           |        55,920.7 ns |       833.1 ns |     369.9 ns |  0.49 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless  | 256  | Reversed           |        55,735.0 ns |       254.9 ns |     113.2 ns |  0.49 |    0.00 |    2 |         - |          NA |
| Ipnsort            | 256  | Reversed           |        19,288.1 ns |       433.5 ns |     226.7 ns |  0.17 |    0.00 |    1 |         - |          NA |
| StdSort            | 256  | Reversed           |        56,912.6 ns |     2,378.4 ns |   1,244.0 ns |  0.50 |    0.01 |    2 |         - |          NA |
| BlockQuickSort     | 256  | Reversed           |       100,464.2 ns |     5,165.9 ns |   2,701.9 ns |  0.88 |    0.02 |    3 |         - |          NA |
| DotnetSort         | 256  | Reversed           |       137,072.8 ns |     2,460.6 ns |   1,286.9 ns |  1.20 |    0.01 |    3 |         - |          NA |
|      |                    |                    |                |              |       |         |      |           |             |
| **QuickSort**          | **256**  | **PipeOrgan**          |     **1,142,190.9 ns** |     **8,429.7 ns** |   **4,408.9 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 256  | PipeOrgan          |       219,078.5 ns |     1,301.2 ns |     577.7 ns |  0.19 |    0.00 |    1 |         - |          NA |
| QuickSortMedian3   | 256  | PipeOrgan          |       252,208.5 ns |     2,261.5 ns |   1,182.8 ns |  0.22 |    0.00 |    1 |         - |          NA |
| QuickSortMedian9   | 256  | PipeOrgan          |       166,734.9 ns |       285.8 ns |     126.9 ns |  0.15 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort | 256  | PipeOrgan          |       171,380.4 ns |     2,462.3 ns |   1,287.9 ns |  0.15 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 256  | PipeOrgan          |       260,317.0 ns |     2,656.9 ns |   1,389.6 ns |  0.23 |    0.00 |    1 |         - |          NA |
| IntroSort          | 256  | PipeOrgan          |       159,495.7 ns |     2,923.0 ns |   1,528.8 ns |  0.14 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | PipeOrgan          |       276,516.5 ns |     1,451.9 ns |     759.4 ns |  0.24 |    0.00 |    1 |         - |          NA |
| PDQSort            | 256  | PipeOrgan          |       208,998.4 ns |     1,218.7 ns |     541.1 ns |  0.18 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | PipeOrgan          |       207,745.2 ns |     2,521.6 ns |   1,318.9 ns |  0.18 |    0.00 |    1 |         - |          NA |
| Ipnsort            | 256  | PipeOrgan          |       183,465.3 ns |     2,904.1 ns |   1,518.9 ns |  0.16 |    0.00 |    1 |         - |          NA |
| StdSort            | 256  | PipeOrgan          |       260,185.4 ns |     3,617.2 ns |   1,891.9 ns |  0.23 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 256  | PipeOrgan          |       268,470.0 ns |     1,541.8 ns |     684.6 ns |  0.24 |    0.00 |    1 |         - |          NA |
| DotnetSort         | 256  | PipeOrgan          |       266,416.3 ns |     1,191.0 ns |     424.7 ns |  0.23 |    0.00 |    1 |         - |          NA |
|      |                    |                    |                |              |       |         |      |           |             |
| **QuickSort**          | **1024** | **Random**             |     **1,040,032.9 ns** |     **4,297.0 ns** |   **2,247.4 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| QuickSort3way      | 1024 | Random             |       911,532.7 ns |     2,239.3 ns |   1,171.2 ns |  0.88 |    0.00 |    1 |         - |          NA |
| QuickSortMedian3   | 1024 | Random             |       928,357.1 ns |     3,795.7 ns |   1,353.6 ns |  0.89 |    0.00 |    1 |         - |          NA |
| QuickSortMedian9   | 1024 | Random             |       884,573.8 ns |     2,164.4 ns |     961.0 ns |  0.85 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort | 1024 | Random             |       860,387.5 ns |     1,839.0 ns |     816.5 ns |  0.83 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 1024 | Random             |     1,715,227.5 ns |     4,734.2 ns |   2,476.1 ns |  1.65 |    0.00 |    2 |         - |          NA |
| IntroSort          | 1024 | Random             |       973,497.3 ns |     3,760.8 ns |   1,669.8 ns |  0.94 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 1024 | Random             |       940,037.1 ns |     1,867.8 ns |     829.3 ns |  0.90 |    0.00 |    1 |         - |          NA |
| PDQSort            | 1024 | Random             |       955,944.4 ns |     3,801.8 ns |   1,688.0 ns |  0.92 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 1024 | Random             |       938,336.7 ns |    12,001.8 ns |   5,328.9 ns |  0.90 |    0.01 |    1 |         - |          NA |
| Ipnsort            | 1024 | Random             |       885,809.0 ns |    11,375.1 ns |   5,050.6 ns |  0.85 |    0.00 |    1 |         - |          NA |
| StdSort            | 1024 | Random             |       944,355.2 ns |     2,773.7 ns |   1,450.7 ns |  0.91 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 1024 | Random             |       887,852.7 ns |     6,383.5 ns |   2,834.3 ns |  0.85 |    0.00 |    1 |         - |          NA |
| DotnetSort         | 1024 | Random             |       927,465.9 ns |     5,252.0 ns |   2,746.9 ns |  0.89 |    0.00 |    1 |         - |          NA |
|      |                    |                    |                |              |       |         |      |           |             |
| **QuickSort**          | **1024** | **SingleElementMoved** |       **681,923.6 ns** |     **3,606.4 ns** |   **1,601.2 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 1024 | SingleElementMoved |     1,380,360.5 ns |     6,445.2 ns |   3,371.0 ns |  2.02 |    0.01 |    3 |         - |          NA |
| QuickSortMedian3   | 1024 | SingleElementMoved |     1,176,186.5 ns |    13,892.0 ns |   6,168.1 ns |  1.72 |    0.01 |    3 |         - |          NA |
| QuickSortMedian9   | 1024 | SingleElementMoved |       871,347.2 ns |     1,583.5 ns |     703.1 ns |  1.28 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort | 1024 | SingleElementMoved |     2,050,068.6 ns |     2,051.0 ns |   1,072.7 ns |  3.01 |    0.01 |    4 |         - |          NA |
| StableQuickSort    | 1024 | SingleElementMoved |     1,259,139.8 ns |    11,353.4 ns |   5,938.1 ns |  1.85 |    0.01 |    3 |         - |          NA |
| IntroSort          | 1024 | SingleElementMoved |       480,746.5 ns |     2,907.9 ns |   1,291.1 ns |  0.70 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 1024 | SingleElementMoved |       655,948.1 ns |     2,828.4 ns |   1,255.8 ns |  0.96 |    0.00 |    2 |         - |          NA |
| PDQSort            | 1024 | SingleElementMoved |       593,714.3 ns |     2,361.3 ns |   1,048.4 ns |  0.87 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless  | 1024 | SingleElementMoved |       577,420.8 ns |     3,620.7 ns |   1,893.7 ns |  0.85 |    0.00 |    2 |         - |          NA |
| Ipnsort            | 1024 | SingleElementMoved |       806,807.3 ns |     5,570.7 ns |   2,913.6 ns |  1.18 |    0.00 |    2 |         - |          NA |
| StdSort            | 1024 | SingleElementMoved |       605,254.2 ns |     4,087.8 ns |   1,815.0 ns |  0.89 |    0.00 |    2 |         - |          NA |
| BlockQuickSort     | 1024 | SingleElementMoved |       581,158.3 ns |     5,963.2 ns |   3,118.9 ns |  0.85 |    0.00 |    2 |         - |          NA |
| DotnetSort         | 1024 | SingleElementMoved |       660,012.6 ns |     6,797.5 ns |   3,018.1 ns |  0.97 |    0.00 |    2 |         - |          NA |
|      |                    |                    |                |              |       |         |      |           |             |
| **QuickSort**          | **1024** | **Sorted**             |       **610,945.1 ns** |     **1,281.1 ns** |     **670.0 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way      | 1024 | Sorted             |     1,809,472.4 ns |     8,559.8 ns |   4,476.9 ns |  2.96 |    0.01 |    6 |         - |          NA |
| QuickSortMedian3   | 1024 | Sorted             |     1,663,133.2 ns |     5,813.6 ns |   3,040.6 ns |  2.72 |    0.01 |    6 |         - |          NA |
| QuickSortMedian9   | 1024 | Sorted             |       908,059.8 ns |     2,672.3 ns |   1,397.7 ns |  1.49 |    0.00 |    4 |         - |          NA |
| DualPivotQuickSort | 1024 | Sorted             |     2,223,157.9 ns |     9,618.4 ns |   4,270.6 ns |  3.64 |    0.01 |    7 |         - |          NA |
| StableQuickSort    | 1024 | Sorted             |     1,272,062.4 ns |     2,254.8 ns |   1,179.3 ns |  2.08 |    0.00 |    5 |         - |          NA |
| IntroSort          | 1024 | Sorted             |       146,892.5 ns |       958.2 ns |     501.2 ns |  0.24 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet    | 1024 | Sorted             |       488,381.0 ns |     2,360.9 ns |   1,234.8 ns |  0.80 |    0.00 |    3 |         - |          NA |
| PDQSort            | 1024 | Sorted             |       145,119.1 ns |     1,626.0 ns |     850.4 ns |  0.24 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless  | 1024 | Sorted             |       144,800.3 ns |     1,363.2 ns |     713.0 ns |  0.24 |    0.00 |    2 |         - |          NA |
| Ipnsort            | 1024 | Sorted             |        73,020.8 ns |     1,005.4 ns |     525.8 ns |  0.12 |    0.00 |    1 |         - |          NA |
| StdSort            | 1024 | Sorted             |       143,747.6 ns |     1,442.8 ns |     754.6 ns |  0.24 |    0.00 |    2 |         - |          NA |
| BlockQuickSort     | 1024 | Sorted             |       527,477.3 ns |     3,231.6 ns |   1,434.9 ns |  0.86 |    0.00 |    3 |         - |          NA |
| DotnetSort         | 1024 | Sorted             |       506,483.6 ns |     1,685.3 ns |     748.3 ns |  0.83 |    0.00 |    3 |         - |          NA |
|      |                    |                    |                |              |       |         |      |           |             |
| **QuickSort**          | **1024** | **Reversed**           |       **591,547.6 ns** |     **2,942.0 ns** |   **1,538.7 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way      | 1024 | Reversed           |     1,353,768.3 ns |     6,370.4 ns |   3,331.8 ns |  2.29 |    0.01 |    6 |         - |          NA |
| QuickSortMedian3   | 1024 | Reversed           |     2,030,192.7 ns |     5,020.5 ns |   2,229.1 ns |  3.43 |    0.01 |    7 |         - |          NA |
| QuickSortMedian9   | 1024 | Reversed           |       920,109.2 ns |     6,739.2 ns |   3,524.8 ns |  1.56 |    0.01 |    5 |         - |          NA |
| DualPivotQuickSort | 1024 | Reversed           |     1,942,053.1 ns |     5,731.6 ns |   2,997.7 ns |  3.28 |    0.01 |    7 |         - |          NA |
| StableQuickSort    | 1024 | Reversed           |     1,260,918.7 ns |     6,134.0 ns |   3,208.2 ns |  2.13 |    0.01 |    6 |         - |          NA |
| IntroSort          | 1024 | Reversed           |       382,445.5 ns |     2,156.0 ns |   1,127.6 ns |  0.65 |    0.00 |    3 |         - |          NA |
| IntroSortDotnet    | 1024 | Reversed           |       810,580.5 ns |     1,027.4 ns |     456.1 ns |  1.37 |    0.00 |    5 |         - |          NA |
| PDQSort            | 1024 | Reversed           |       218,499.5 ns |     1,088.1 ns |     483.1 ns |  0.37 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless  | 1024 | Reversed           |       217,413.9 ns |       737.9 ns |     327.7 ns |  0.37 |    0.00 |    2 |         - |          NA |
| Ipnsort            | 1024 | Reversed           |        74,737.6 ns |       931.9 ns |     487.4 ns |  0.13 |    0.00 |    1 |         - |          NA |
| StdSort            | 1024 | Reversed           |       217,159.1 ns |       841.9 ns |     440.3 ns |  0.37 |    0.00 |    2 |         - |          NA |
| BlockQuickSort     | 1024 | Reversed           |       529,050.6 ns |     2,450.1 ns |   1,281.4 ns |  0.89 |    0.00 |    4 |         - |          NA |
| DotnetSort         | 1024 | Reversed           |       793,129.1 ns |     3,169.4 ns |   1,657.7 ns |  1.34 |    0.00 |    5 |         - |          NA |
|      |                    |                    |                |              |       |         |      |           |             |
| **QuickSort**          | **1024** | **PipeOrgan**          |    **17,531,835.8 ns** |    **85,816.1 ns** |  **44,883.5 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way      | 1024 | PipeOrgan          |     1,358,558.7 ns |     1,788.0 ns |     935.1 ns |  0.08 |    0.00 |    2 |         - |          NA |
| QuickSortMedian3   | 1024 | PipeOrgan          |     1,471,386.6 ns |     2,985.0 ns |   1,561.2 ns |  0.08 |    0.00 |    2 |         - |          NA |
| QuickSortMedian9   | 1024 | PipeOrgan          |       875,682.1 ns |     1,307.9 ns |     684.1 ns |  0.05 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort | 1024 | PipeOrgan          |       850,183.6 ns |     2,001.9 ns |     888.8 ns |  0.05 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 1024 | PipeOrgan          |     1,322,974.7 ns |     6,522.4 ns |   3,411.3 ns |  0.08 |    0.00 |    2 |         - |          NA |
| IntroSort          | 1024 | PipeOrgan          |     1,313,357.2 ns |     1,655.2 ns |     734.9 ns |  0.07 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet    | 1024 | PipeOrgan          |     1,653,981.2 ns |     6,674.2 ns |   3,490.8 ns |  0.09 |    0.00 |    2 |         - |          NA |
| PDQSort            | 1024 | PipeOrgan          |     1,084,519.3 ns |     3,323.7 ns |   1,738.3 ns |  0.06 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 1024 | PipeOrgan          |     1,058,441.0 ns |     4,179.3 ns |   1,855.6 ns |  0.06 |    0.00 |    1 |         - |          NA |
| Ipnsort            | 1024 | PipeOrgan          |       951,930.9 ns |     2,486.2 ns |   1,300.4 ns |  0.05 |    0.00 |    1 |         - |          NA |
| StdSort            | 1024 | PipeOrgan          |     1,515,936.9 ns |     9,652.7 ns |   4,285.9 ns |  0.09 |    0.00 |    2 |         - |          NA |
| BlockQuickSort     | 1024 | PipeOrgan          |     1,334,160.4 ns |     4,653.6 ns |   2,433.9 ns |  0.08 |    0.00 |    2 |         - |          NA |
| DotnetSort         | 1024 | PipeOrgan          |     1,601,333.1 ns |     2,334.6 ns |   1,221.0 ns |  0.09 |    0.00 |    2 |         - |          NA |
|      |                    |                    |                |              |       |         |      |           |             |
| **QuickSort**          | **8192** | **Random**             |    **10,629,268.0 ns** |     **5,192.7 ns** |   **2,305.6 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| QuickSort3way      | 8192 | Random             |     9,519,961.2 ns |    31,810.7 ns |  16,637.6 ns |  0.90 |    0.00 |    1 |         - |          NA |
| QuickSortMedian3   | 8192 | Random             |     9,806,852.5 ns |    12,452.6 ns |   5,529.0 ns |  0.92 |    0.00 |    1 |         - |          NA |
| QuickSortMedian9   | 8192 | Random             |     9,650,066.6 ns |    22,335.9 ns |  11,682.1 ns |  0.91 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort | 8192 | Random             |     9,129,831.9 ns |     8,038.7 ns |   3,569.2 ns |  0.86 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 8192 | Random             |    17,669,951.5 ns |    64,886.3 ns |  28,809.9 ns |  1.66 |    0.00 |    2 |         - |          NA |
| IntroSort          | 8192 | Random             |    11,176,770.5 ns |    15,616.7 ns |   5,569.1 ns |  1.05 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 8192 | Random             |     9,189,343.8 ns |     8,508.9 ns |   3,778.0 ns |  0.86 |    0.00 |    1 |         - |          NA |
| PDQSort            | 8192 | Random             |     9,862,481.5 ns |    14,689.1 ns |   5,238.3 ns |  0.93 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | Random             |     9,544,323.7 ns |    33,704.8 ns |  14,965.1 ns |  0.90 |    0.00 |    1 |         - |          NA |
| Ipnsort            | 8192 | Random             |     9,295,308.2 ns |    29,774.0 ns |  15,572.4 ns |  0.87 |    0.00 |    1 |         - |          NA |
| StdSort            | 8192 | Random             |     9,877,332.2 ns |    14,188.8 ns |   7,421.0 ns |  0.93 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 8192 | Random             |     9,365,043.0 ns |    16,456.2 ns |   8,606.9 ns |  0.88 |    0.00 |    1 |         - |          NA |
| DotnetSort         | 8192 | Random             |     8,959,942.4 ns |     9,630.8 ns |   3,434.4 ns |  0.84 |    0.00 |    1 |         - |          NA |
|      |                    |                    |                |              |       |         |      |           |             |
| **QuickSort**          | **8192** | **SingleElementMoved** |     **6,936,868.0 ns** |    **12,054.1 ns** |   **5,352.1 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| QuickSort3way      | 8192 | SingleElementMoved |    28,708,886.1 ns |    67,597.1 ns |  35,354.6 ns |  4.14 |    0.01 |    4 |         - |          NA |
| QuickSortMedian3   | 8192 | SingleElementMoved |    22,530,899.8 ns |    73,240.4 ns |  38,306.1 ns |  3.25 |    0.01 |    3 |         - |          NA |
| QuickSortMedian9   | 8192 | SingleElementMoved |     9,038,681.6 ns |    32,799.9 ns |  17,155.0 ns |  1.30 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort | 8192 | SingleElementMoved |    12,811,253.8 ns |    53,860.2 ns |  23,914.3 ns |  1.85 |    0.00 |    2 |         - |          NA |
| StableQuickSort    | 8192 | SingleElementMoved |    13,450,881.4 ns |   116,889.8 ns |  51,899.8 ns |  1.94 |    0.01 |    2 |         - |          NA |
| IntroSort          | 8192 | SingleElementMoved |     4,765,172.0 ns |     8,833.4 ns |   4,620.0 ns |  0.69 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 8192 | SingleElementMoved |     7,944,798.1 ns |     6,898.4 ns |   3,062.9 ns |  1.15 |    0.00 |    1 |         - |          NA |
| PDQSort            | 8192 | SingleElementMoved |     5,509,525.1 ns |     8,672.4 ns |   4,535.8 ns |  0.79 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | SingleElementMoved |     5,559,858.2 ns |    16,219.3 ns |   8,483.0 ns |  0.80 |    0.00 |    1 |         - |          NA |
| Ipnsort            | 8192 | SingleElementMoved |     8,654,603.8 ns |    20,642.1 ns |   9,165.2 ns |  1.25 |    0.00 |    1 |         - |          NA |
| StdSort            | 8192 | SingleElementMoved |     5,543,319.7 ns |    16,215.8 ns |   7,199.9 ns |  0.80 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 8192 | SingleElementMoved |     6,363,312.9 ns |    11,226.1 ns |   4,984.5 ns |  0.92 |    0.00 |    1 |         - |          NA |
| DotnetSort         | 8192 | SingleElementMoved |     7,935,895.4 ns |    30,032.0 ns |  15,707.3 ns |  1.14 |    0.00 |    1 |         - |          NA |
|      |                    |                    |                |              |       |         |      |           |             |
| **QuickSort**          | **8192** | **Sorted**             |     **6,646,632.1 ns** |    **16,231.4 ns** |   **7,206.8 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way      | 8192 | Sorted             |    39,433,857.0 ns |   175,197.6 ns |  77,788.8 ns |  5.93 |    0.01 |    6 |         - |          NA |
| QuickSortMedian3   | 8192 | Sorted             |    35,820,635.8 ns |    79,380.7 ns |  41,517.6 ns |  5.39 |    0.01 |    6 |         - |          NA |
| QuickSortMedian9   | 8192 | Sorted             |     8,827,412.5 ns |     9,374.3 ns |   4,902.9 ns |  1.33 |    0.00 |    4 |         - |          NA |
| DualPivotQuickSort | 8192 | Sorted             |    15,195,169.0 ns |    27,878.5 ns |  12,378.2 ns |  2.29 |    0.00 |    5 |         - |          NA |
| StableQuickSort    | 8192 | Sorted             |    13,380,823.0 ns |     6,652.7 ns |   3,479.5 ns |  2.01 |    0.00 |    5 |         - |          NA |
| IntroSort          | 8192 | Sorted             |     1,122,205.4 ns |     3,655.2 ns |   1,911.8 ns |  0.17 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet    | 8192 | Sorted             |     5,891,281.6 ns |    89,246.7 ns |  39,626.1 ns |  0.89 |    0.01 |    3 |         - |          NA |
| PDQSort            | 8192 | Sorted             |     1,215,811.9 ns |     2,220.2 ns |   1,161.2 ns |  0.18 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless  | 8192 | Sorted             |     1,182,733.0 ns |     4,357.2 ns |   1,934.6 ns |  0.18 |    0.00 |    2 |         - |          NA |
| Ipnsort            | 8192 | Sorted             |       583,390.8 ns |     1,753.6 ns |     778.6 ns |  0.09 |    0.00 |    1 |         - |          NA |
| StdSort            | 8192 | Sorted             |     1,169,991.2 ns |     4,756.7 ns |   2,487.8 ns |  0.18 |    0.00 |    2 |         - |          NA |
| BlockQuickSort     | 8192 | Sorted             |     5,877,437.3 ns |    14,652.9 ns |   6,506.0 ns |  0.88 |    0.00 |    3 |         - |          NA |
| DotnetSort         | 8192 | Sorted             |     5,387,604.9 ns |     5,820.6 ns |   2,584.4 ns |  0.81 |    0.00 |    3 |         - |          NA |
|      |                    |                    |                |              |       |         |      |           |             |
| **QuickSort**          | **8192** | **Reversed**           |     **6,549,692.5 ns** |     **8,701.5 ns** |   **3,863.5 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way      | 8192 | Reversed           |    28,372,367.9 ns |    48,011.8 ns |  25,111.1 ns |  4.33 |    0.00 |    7 |         - |          NA |
| QuickSortMedian3   | 8192 | Reversed           |    45,357,427.2 ns |    45,086.7 ns |  20,018.8 ns |  6.93 |    0.00 |    8 |         - |          NA |
| QuickSortMedian9   | 8192 | Reversed           |     8,850,620.6 ns |     7,134.2 ns |   2,544.1 ns |  1.35 |    0.00 |    5 |         - |          NA |
| DualPivotQuickSort | 8192 | Reversed           |    14,613,315.3 ns |    27,444.1 ns |  12,185.3 ns |  2.23 |    0.00 |    6 |         - |          NA |
| StableQuickSort    | 8192 | Reversed           |    13,407,369.2 ns |     7,460.1 ns |   3,312.3 ns |  2.05 |    0.00 |    6 |         - |          NA |
| IntroSort          | 8192 | Reversed           |     3,508,411.6 ns |     9,808.8 ns |   5,130.2 ns |  0.54 |    0.00 |    3 |         - |          NA |
| IntroSortDotnet    | 8192 | Reversed           |    10,308,758.8 ns |    20,209.5 ns |   8,973.1 ns |  1.57 |    0.00 |    5 |         - |          NA |
| PDQSort            | 8192 | Reversed           |     1,753,190.5 ns |     3,646.6 ns |   1,907.3 ns |  0.27 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless  | 8192 | Reversed           |     1,801,642.0 ns |     4,435.1 ns |   2,319.7 ns |  0.28 |    0.00 |    2 |         - |          NA |
| Ipnsort            | 8192 | Reversed           |       602,452.4 ns |     1,609.8 ns |     714.8 ns |  0.09 |    0.00 |    1 |         - |          NA |
| StdSort            | 8192 | Reversed           |     1,794,644.7 ns |     3,039.5 ns |   1,349.6 ns |  0.27 |    0.00 |    2 |         - |          NA |
| BlockQuickSort     | 8192 | Reversed           |     6,289,338.2 ns |     7,321.8 ns |   3,250.9 ns |  0.96 |    0.00 |    4 |         - |          NA |
| DotnetSort         | 8192 | Reversed           |     9,365,223.6 ns |    13,787.8 ns |   7,211.3 ns |  1.43 |    0.00 |    5 |         - |          NA |
|      |                    |                    |                |              |       |         |      |           |             |
| **QuickSort**          | **8192** | **PipeOrgan**          | **1,183,362,148.7 ns** | **1,266,125.8 ns** | **662,208.3 ns** | **1.000** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way      | 8192 | PipeOrgan          |    17,831,458.3 ns |    58,253.5 ns |  30,467.7 ns | 0.015 |    0.00 |    2 |         - |          NA |
| QuickSortMedian3   | 8192 | PipeOrgan          |    18,735,764.9 ns |    29,525.0 ns |  15,442.2 ns | 0.016 |    0.00 |    2 |         - |          NA |
| QuickSortMedian9   | 8192 | PipeOrgan          |     9,013,861.0 ns |     8,269.4 ns |   4,325.1 ns | 0.008 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort | 8192 | PipeOrgan          |     8,548,212.5 ns |    15,574.5 ns |   6,915.2 ns | 0.007 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 8192 | PipeOrgan          |    13,795,049.1 ns |    22,167.7 ns |   9,842.6 ns | 0.012 |    0.00 |    1 |         - |          NA |
| IntroSort          | 8192 | PipeOrgan          |    21,897,978.2 ns |    32,493.7 ns |  16,994.9 ns | 0.019 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet    | 8192 | PipeOrgan          |    22,068,858.4 ns |    88,819.4 ns |  46,454.2 ns | 0.019 |    0.00 |    2 |         - |          NA |
| PDQSort            | 8192 | PipeOrgan          |    11,654,976.9 ns |    14,299.0 ns |   5,099.2 ns | 0.010 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | PipeOrgan          |    11,772,673.5 ns |    14,322.5 ns |   6,359.3 ns | 0.010 |    0.00 |    1 |         - |          NA |
| Ipnsort            | 8192 | PipeOrgan          |    10,312,244.4 ns |    18,036.9 ns |   6,432.1 ns | 0.009 |    0.00 |    1 |         - |          NA |
| StdSort            | 8192 | PipeOrgan          |    20,026,868.7 ns |    67,378.1 ns |  29,916.3 ns | 0.017 |    0.00 |    2 |         - |          NA |
| BlockQuickSort     | 8192 | PipeOrgan          |    12,689,877.8 ns |    14,221.9 ns |   7,438.3 ns | 0.011 |    0.00 |    1 |         - |          NA |
| DotnetSort         | 8192 | PipeOrgan          |    21,534,847.9 ns |    39,411.2 ns |  17,498.8 ns | 0.018 |    0.00 |    2 |         - |          NA |

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
| **BalancedBinaryTreeSort** | **256**  | **Random**             |    **12,565.6 ns** |   **528.88 ns** |   **234.82 ns** |  **3.64** |    **0.11** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 256  | Random             |     3,452.5 ns |   207.23 ns |    92.01 ns |  1.00 |    0.03 |    1 |         - |          NA |
| SplaySort              | 256  | Random             |    22,530.5 ns |   401.84 ns |   178.42 ns |  6.53 |    0.16 |    4 |         - |          NA |
| TreapSort              | 256  | Random             |     9,214.2 ns |   397.46 ns |   207.88 ns |  2.67 |    0.09 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **SingleElementMoved** |    **15,523.9 ns** |   **918.30 ns** |   **480.29 ns** |  **0.31** |    **0.01** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 256  | SingleElementMoved |    50,531.6 ns |   386.95 ns |   171.81 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 256  | SingleElementMoved |     4,146.2 ns |    16.03 ns |     7.12 ns |  0.08 |    0.00 |    1 |         - |          NA |
| TreapSort              | 256  | SingleElementMoved |     5,756.8 ns |   106.68 ns |    47.37 ns |  0.11 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Sorted**             |    **14,461.6 ns** | **4,306.09 ns** | **1,911.93 ns** |  **0.19** |    **0.02** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 256  | Sorted             |    75,870.7 ns |   233.62 ns |   122.19 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 256  | Sorted             |     3,862.2 ns |   339.78 ns |   177.71 ns |  0.05 |    0.00 |    1 |         - |          NA |
| TreapSort              | 256  | Sorted             |     4,990.3 ns |    45.67 ns |    20.28 ns |  0.07 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Reversed**           |    **12,197.8 ns** |   **294.86 ns** |   **130.92 ns** |  **0.17** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 256  | Reversed           |    73,913.3 ns |   338.75 ns |   177.17 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 256  | Reversed           |     3,540.6 ns |    11.33 ns |     5.03 ns |  0.05 |    0.00 |    1 |         - |          NA |
| TreapSort              | 256  | Reversed           |     5,470.2 ns |   148.51 ns |    77.67 ns |  0.07 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **PipeOrgan**          |    **12,306.5 ns** |   **419.91 ns** |   **219.62 ns** |  **0.32** |    **0.01** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 256  | PipeOrgan          |    38,314.4 ns |   316.14 ns |   112.74 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 256  | PipeOrgan          |     4,350.6 ns |    89.94 ns |    32.08 ns |  0.11 |    0.00 |    1 |         - |          NA |
| TreapSort              | 256  | PipeOrgan          |     7,246.9 ns |   101.81 ns |    45.20 ns |  0.19 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Random**             |   **123,887.1 ns** | **7,481.33 ns** | **3,912.88 ns** |  **6.05** |    **0.19** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 1024 | Random             |    20,489.7 ns |   357.71 ns |   158.83 ns |  1.00 |    0.01 |    1 |         - |          NA |
| SplaySort              | 1024 | Random             |   160,840.2 ns | 3,757.50 ns | 1,965.24 ns |  7.85 |    0.11 |    4 |         - |          NA |
| TreapSort              | 1024 | Random             |    39,568.5 ns | 2,654.20 ns | 1,178.48 ns |  1.93 |    0.06 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **SingleElementMoved** |   **106,568.8 ns** | **1,407.30 ns** |   **736.05 ns** |  **0.14** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 1024 | SingleElementMoved |   780,873.4 ns |   664.48 ns |   295.03 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | SingleElementMoved |    16,342.3 ns |   150.92 ns |    67.01 ns |  0.02 |    0.00 |    1 |         - |          NA |
| TreapSort              | 1024 | SingleElementMoved |    29,582.1 ns |   593.44 ns |   310.38 ns |  0.04 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Sorted**             |    **98,846.2 ns** | **1,672.75 ns** |   **742.71 ns** |  **0.08** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 1024 | Sorted             | 1,191,163.7 ns | 1,925.24 ns | 1,006.94 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | Sorted             |    14,728.1 ns |    59.30 ns |    31.02 ns |  0.01 |    0.00 |    1 |         - |          NA |
| TreapSort              | 1024 | Sorted             |    23,825.6 ns |   431.43 ns |   191.56 ns |  0.02 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Reversed**           |    **59,650.0 ns** | **1,474.01 ns** |   **770.94 ns** |  **0.05** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 1024 | Reversed           | 1,152,122.0 ns | 1,029.20 ns |   456.97 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | Reversed           |    14,276.4 ns |   281.05 ns |   146.99 ns |  0.01 |    0.00 |    1 |         - |          NA |
| TreapSort              | 1024 | Reversed           |    23,457.4 ns |   436.75 ns |   228.43 ns |  0.02 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **PipeOrgan**          |    **93,383.4 ns** | **2,146.54 ns** |   **953.08 ns** |  **0.16** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 1024 | PipeOrgan          |   582,879.4 ns |   596.23 ns |   212.62 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | PipeOrgan          |    16,992.8 ns |   185.26 ns |    96.90 ns |  0.03 |    0.00 |    1 |         - |          NA |
| TreapSort              | 1024 | PipeOrgan          |    34,383.8 ns | 1,030.74 ns |   457.66 ns |  0.06 |    0.00 |    2 |         - |          NA |

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
