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
<summary>Benchmark results (2026-07-13 08:39 UTC)</summary>

Workflow run: https://github.com/guitarrapc/SortAlgorithms/actions/runs/29234215979

### AdaptiveBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method        | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **DropMergeSort** | **256**  | **Random**             |     **5,029.2 ns** |    **600.53 ns** |    **314.09 ns** |  **1.00** |    **0.08** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | Random             |     5,353.5 ns |  1,666.47 ns |    871.60 ns |  1.07 |    0.18 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **256**  | **SingleElementMoved** |       **602.4 ns** |      **2.81 ns** |      **1.25 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | SingleElementMoved |    11,631.3 ns |  6,061.22 ns |  3,170.13 ns | 19.31 |    4.97 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **256**  | **Sorted**             |       **789.6 ns** |     **20.25 ns** |      **7.22 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | Sorted             |     7,449.8 ns |    207.71 ns |    108.64 ns |  9.44 |    0.15 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **256**  | **Reversed**           |     **7,844.8 ns** |    **413.36 ns** |    **216.19 ns** |  **1.00** |    **0.04** |    **2** |         **-** |          **NA** |
| PatienceSort  | 256  | Reversed           |     1,916.8 ns |     98.95 ns |     35.29 ns |  0.24 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **256**  | **PipeOrgan**          |     **7,829.4 ns** |    **306.80 ns** |    **160.46 ns** |  **1.00** |    **0.03** |    **2** |         **-** |          **NA** |
| PatienceSort  | 256  | PipeOrgan          |     5,322.5 ns |    370.85 ns |    193.96 ns |  0.68 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **Random**             |    **29,610.7 ns** |  **5,567.94 ns** |  **2,912.14 ns** |  **1.01** |    **0.14** |    **2** |         **-** |          **NA** |
| PatienceSort  | 1024 | Random             |    22,807.0 ns |  1,009.80 ns |    448.36 ns |  0.78 |    0.08 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **SingleElementMoved** |     **2,183.8 ns** |     **11.23 ns** |      **4.99 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | SingleElementMoved |    44,210.6 ns | 12,822.29 ns |  6,706.30 ns | 20.24 |    2.90 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **Sorted**             |     **2,064.9 ns** |    **287.37 ns** |    **150.30 ns** |  **1.00** |    **0.10** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | Sorted             |    42,177.9 ns |  7,179.37 ns |  3,754.95 ns | 20.52 |    2.20 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **Reversed**           |    **52,973.0 ns** |  **1,107.19 ns** |    **491.60 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| PatienceSort  | 1024 | Reversed           |     6,462.6 ns |    429.91 ns |    224.85 ns |  0.12 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **PipeOrgan**          |    **39,831.7 ns** |    **872.11 ns** |    **456.13 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| PatienceSort  | 1024 | PipeOrgan          |    26,092.6 ns |    865.70 ns |    384.38 ns |  0.66 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **Random**             |   **546,817.5 ns** |  **9,130.50 ns** |  **4,775.43 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | Random             |   716,253.8 ns |  3,386.66 ns |  1,503.70 ns |  1.31 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **SingleElementMoved** |    **17,655.3 ns** |    **624.58 ns** |    **277.32 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | SingleElementMoved |   741,900.4 ns | 16,378.04 ns |  8,566.03 ns | 42.03 |    0.77 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **Sorted**             |    **15,786.2 ns** |    **985.75 ns** |    **437.68 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | Sorted             |   733,523.5 ns |  9,693.24 ns |  4,303.86 ns | 46.50 |    1.18 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **Reversed**           | **1,171,601.6 ns** | **70,246.45 ns** | **31,189.86 ns** |  **1.00** |    **0.04** |    **2** |         **-** |          **NA** |
| PatienceSort  | 8192 | Reversed           |    46,841.3 ns |  1,208.10 ns |    536.40 ns |  0.04 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **PipeOrgan**          |   **525,281.0 ns** | **18,794.07 ns** |  **8,344.69 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | PipeOrgan          |   574,858.0 ns |  4,422.80 ns |  1,963.75 ns |  1.09 |    0.02 |    1 |         - |          NA |

### AdaptiveSlowBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method     | Size | Pattern            | Mean         | Error       | StdDev      | Median       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------- |----- |------------------- |-------------:|------------:|------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **StrandSort** | **256**  | **Random**             |  **11,121.5 ns** | **4,883.54 ns** | **2,554.19 ns** |  **12,464.9 ns** |  **1.07** |    **0.39** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |              |       |         |      |           |             |
| **StrandSort** | **256**  | **SingleElementMoved** |   **1,078.1 ns** |   **713.44 ns** |   **373.14 ns** |     **835.9 ns** |  **1.09** |    **0.47** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |              |       |         |      |           |             |
| **StrandSort** | **256**  | **Sorted**             |     **547.1 ns** |    **14.18 ns** |     **7.41 ns** |     **543.7 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |              |       |         |      |           |             |
| **StrandSort** | **256**  | **Reversed**           |  **52,292.8 ns** |   **340.96 ns** |   **178.33 ns** |  **52,253.9 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |              |       |         |      |           |             |
| **StrandSort** | **256**  | **PipeOrgan**          |  **27,859.0 ns** |   **506.17 ns** |   **264.74 ns** |  **27,894.8 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |              |       |         |      |           |             |
| **StrandSort** | **1024** | **Random**             |  **55,960.6 ns** |   **250.56 ns** |   **111.25 ns** |  **56,011.3 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |              |       |         |      |           |             |
| **StrandSort** | **1024** | **SingleElementMoved** |   **2,688.8 ns** |   **111.39 ns** |    **39.72 ns** |   **2,666.3 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |              |       |         |      |           |             |
| **StrandSort** | **1024** | **Sorted**             |   **1,637.8 ns** |    **72.00 ns** |    **31.97 ns** |   **1,656.9 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |              |       |         |      |           |             |
| **StrandSort** | **1024** | **Reversed**           | **764,148.6 ns** | **1,564.42 ns** |   **818.22 ns** | **764,094.8 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |              |       |         |      |           |             |
| **StrandSort** | **1024** | **PipeOrgan**          | **396,290.4 ns** | **1,488.48 ns** |   **660.89 ns** | **396,313.7 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |

### DistributionBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method              | Size | Pattern            | Mean         | Error        | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------------- |----- |------------------- |-------------:|-------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **CountingSort**        | **256**  | **Random**             |   **2,037.4 ns** |    **562.57 ns** |   **249.79 ns** |  **1.96** |    **0.23** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | Random             |   1,037.7 ns |     20.82 ns |     9.24 ns |  1.00 |    0.01 |    1 |         - |          NA |
| PigeonSort          | 256  | Random             |   1,754.2 ns |     97.57 ns |    43.32 ns |  1.69 |    0.04 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | Random             |     998.8 ns |     11.84 ns |     4.22 ns |  0.96 |    0.01 |    1 |         - |          NA |
| BucketSort          | 256  | Random             |   3,121.0 ns |    372.85 ns |   195.01 ns |  3.01 |    0.18 |    3 |         - |          NA |
| BucketSortInteger   | 256  | Random             |   2,899.8 ns |     41.71 ns |    14.87 ns |  2.79 |    0.03 |    3 |         - |          NA |
| FlashSort           | 256  | Random             |   4,256.8 ns |     22.96 ns |     8.19 ns |  4.10 |    0.03 |    4 |         - |          NA |
| RadixLSD4Sort       | 256  | Random             |   6,077.1 ns |    403.64 ns |   211.11 ns |  5.86 |    0.20 |    5 |         - |          NA |
| RadixLSD256Sort     | 256  | Random             |   2,744.7 ns |    157.18 ns |    69.79 ns |  2.65 |    0.07 |    3 |         - |          NA |
| RadixLSD10Sort      | 256  | Random             |   4,173.0 ns |     55.09 ns |    24.46 ns |  4.02 |    0.04 |    4 |         - |          NA |
| RadixMSD4Sort       | 256  | Random             |  12,054.1 ns |    266.16 ns |   118.17 ns | 11.62 |    0.14 |    6 |         - |          NA |
| RadixMSD10Sort      | 256  | Random             |  14,025.6 ns |    293.45 ns |   153.48 ns | 13.52 |    0.18 |    6 |         - |          NA |
| AmericanFlagSort    | 256  | Random             |   5,704.1 ns |    424.65 ns |   222.10 ns |  5.50 |    0.21 |    5 |         - |          NA |
| SpreadSort          | 256  | Random             |   1,686.5 ns |     24.09 ns |     8.59 ns |  1.63 |    0.02 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **256**  | **SingleElementMoved** |   **1,635.7 ns** |     **53.21 ns** |    **18.98 ns** |  **1.69** |    **0.02** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | SingleElementMoved |     967.2 ns |      1.33 ns |     0.47 ns |  1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 256  | SingleElementMoved |   1,513.4 ns |     23.21 ns |    10.30 ns |  1.56 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | SingleElementMoved |     985.4 ns |     15.48 ns |     8.09 ns |  1.02 |    0.01 |    1 |         - |          NA |
| BucketSort          | 256  | SingleElementMoved |   2,545.6 ns |      9.65 ns |     4.28 ns |  2.63 |    0.00 |    4 |         - |          NA |
| BucketSortInteger   | 256  | SingleElementMoved |   2,097.3 ns |    395.10 ns |   206.64 ns |  2.17 |    0.20 |    3 |         - |          NA |
| FlashSort           | 256  | SingleElementMoved |   5,084.5 ns |    368.92 ns |   192.95 ns |  5.26 |    0.19 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | SingleElementMoved |   5,857.9 ns |    382.11 ns |   199.85 ns |  6.06 |    0.20 |    5 |         - |          NA |
| RadixLSD256Sort     | 256  | SingleElementMoved |   2,443.6 ns |     66.24 ns |    23.62 ns |  2.53 |    0.02 |    4 |         - |          NA |
| RadixLSD10Sort      | 256  | SingleElementMoved |   4,333.8 ns |    485.37 ns |   253.86 ns |  4.48 |    0.25 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | SingleElementMoved |  11,415.3 ns |    372.95 ns |   195.06 ns | 11.80 |    0.19 |    6 |         - |          NA |
| RadixMSD10Sort      | 256  | SingleElementMoved |  14,098.3 ns |    666.16 ns |   348.42 ns | 14.58 |    0.34 |    7 |         - |          NA |
| AmericanFlagSort    | 256  | SingleElementMoved |   4,386.8 ns |    233.22 ns |   103.55 ns |  4.54 |    0.10 |    5 |         - |          NA |
| SpreadSort          | 256  | SingleElementMoved |   1,100.6 ns |     18.61 ns |     6.64 ns |  1.14 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **256**  | **Sorted**             |   **1,554.7 ns** |      **6.34 ns** |     **2.82 ns** |  **1.70** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | Sorted             |     912.7 ns |      7.26 ns |     3.22 ns |  1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 256  | Sorted             |   1,486.3 ns |     16.10 ns |     7.15 ns |  1.63 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | Sorted             |   1,065.1 ns |    269.46 ns |   140.93 ns |  1.17 |    0.15 |    2 |         - |          NA |
| BucketSort          | 256  | Sorted             |   2,440.7 ns |     31.88 ns |    11.37 ns |  2.67 |    0.01 |    5 |         - |          NA |
| BucketSortInteger   | 256  | Sorted             |   1,878.8 ns |     87.40 ns |    31.17 ns |  2.06 |    0.03 |    4 |         - |          NA |
| FlashSort           | 256  | Sorted             |   5,015.4 ns |    577.92 ns |   256.60 ns |  5.50 |    0.26 |    6 |         - |          NA |
| RadixLSD4Sort       | 256  | Sorted             |   5,806.6 ns |    378.05 ns |   197.73 ns |  6.36 |    0.21 |    6 |         - |          NA |
| RadixLSD256Sort     | 256  | Sorted             |   2,487.6 ns |    131.76 ns |    68.91 ns |  2.73 |    0.07 |    5 |         - |          NA |
| RadixLSD10Sort      | 256  | Sorted             |   4,089.8 ns |    334.36 ns |   174.88 ns |  4.48 |    0.18 |    6 |         - |          NA |
| RadixMSD4Sort       | 256  | Sorted             |  11,484.1 ns |    456.94 ns |   238.99 ns | 12.58 |    0.25 |    7 |         - |          NA |
| RadixMSD10Sort      | 256  | Sorted             |  13,876.9 ns |    231.96 ns |   121.32 ns | 15.20 |    0.14 |    8 |         - |          NA |
| AmericanFlagSort    | 256  | Sorted             |   4,269.6 ns |     10.83 ns |     3.86 ns |  4.68 |    0.02 |    6 |         - |          NA |
| SpreadSort          | 256  | Sorted             |     339.7 ns |      1.34 ns |     0.60 ns |  0.37 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **256**  | **Reversed**           |   **1,566.3 ns** |      **6.32 ns** |     **2.80 ns** |  **1.68** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | Reversed           |     932.3 ns |     13.95 ns |     7.30 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PigeonSort          | 256  | Reversed           |   1,421.8 ns |     12.72 ns |     5.65 ns |  1.53 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | Reversed           |     889.3 ns |      4.48 ns |     1.99 ns |  0.95 |    0.01 |    2 |         - |          NA |
| BucketSort          | 256  | Reversed           |   3,389.3 ns |    117.29 ns |    41.83 ns |  3.64 |    0.05 |    4 |         - |          NA |
| BucketSortInteger   | 256  | Reversed           |   3,073.0 ns |     15.01 ns |     5.35 ns |  3.30 |    0.02 |    4 |         - |          NA |
| FlashSort           | 256  | Reversed           |   4,583.2 ns |    375.42 ns |   196.35 ns |  4.92 |    0.20 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | Reversed           |   5,685.2 ns |     25.90 ns |    13.54 ns |  6.10 |    0.05 |    6 |         - |          NA |
| RadixLSD256Sort     | 256  | Reversed           |   2,594.0 ns |    231.57 ns |   102.82 ns |  2.78 |    0.11 |    4 |         - |          NA |
| RadixLSD10Sort      | 256  | Reversed           |   4,190.0 ns |    370.20 ns |   193.62 ns |  4.49 |    0.20 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | Reversed           |  12,299.2 ns |    455.80 ns |   238.39 ns | 13.19 |    0.26 |    7 |         - |          NA |
| RadixMSD10Sort      | 256  | Reversed           |  13,842.1 ns |    282.88 ns |   125.60 ns | 14.85 |    0.17 |    7 |         - |          NA |
| AmericanFlagSort    | 256  | Reversed           |   5,792.4 ns |    362.58 ns |   189.64 ns |  6.21 |    0.20 |    6 |         - |          NA |
| SpreadSort          | 256  | Reversed           |     527.2 ns |     10.34 ns |     5.41 ns |  0.57 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **256**  | **PipeOrgan**          |   **1,539.7 ns** |     **33.39 ns** |    **17.46 ns** |  **1.76** |    **0.03** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | PipeOrgan          |     874.1 ns |     35.83 ns |    12.78 ns |  1.00 |    0.02 |    1 |         - |          NA |
| PigeonSort          | 256  | PipeOrgan          |   1,462.4 ns |     36.39 ns |    16.16 ns |  1.67 |    0.03 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | PipeOrgan          |     936.9 ns |     13.26 ns |     5.89 ns |  1.07 |    0.02 |    1 |         - |          NA |
| BucketSort          | 256  | PipeOrgan          |   2,957.1 ns |     30.60 ns |    10.91 ns |  3.38 |    0.05 |    3 |         - |          NA |
| BucketSortInteger   | 256  | PipeOrgan          |   2,519.9 ns |     27.10 ns |     9.66 ns |  2.88 |    0.04 |    3 |         - |          NA |
| FlashSort           | 256  | PipeOrgan          |   4,533.2 ns |     46.01 ns |    16.41 ns |  5.19 |    0.07 |    4 |         - |          NA |
| RadixLSD4Sort       | 256  | PipeOrgan          |   6,115.4 ns |    333.19 ns |   174.26 ns |  7.00 |    0.21 |    5 |         - |          NA |
| RadixLSD256Sort     | 256  | PipeOrgan          |   2,602.3 ns |     97.07 ns |    43.10 ns |  2.98 |    0.06 |    3 |         - |          NA |
| RadixLSD10Sort      | 256  | PipeOrgan          |   4,049.6 ns |     77.62 ns |    27.68 ns |  4.63 |    0.07 |    4 |         - |          NA |
| RadixMSD4Sort       | 256  | PipeOrgan          |  13,091.5 ns |    372.09 ns |   165.21 ns | 14.98 |    0.27 |    6 |         - |          NA |
| RadixMSD10Sort      | 256  | PipeOrgan          |  14,118.3 ns |    436.20 ns |   228.14 ns | 16.15 |    0.33 |    6 |         - |          NA |
| AmericanFlagSort    | 256  | PipeOrgan          |   6,204.6 ns |    385.37 ns |   201.56 ns |  7.10 |    0.24 |    5 |         - |          NA |
| SpreadSort          | 256  | PipeOrgan          |   1,687.8 ns |     48.71 ns |    21.63 ns |  1.93 |    0.03 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **1024** | **Random**             |   **6,388.2 ns** |    **234.99 ns** |   **122.90 ns** |  **1.60** |    **0.10** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Random             |   3,994.2 ns |    491.28 ns |   256.95 ns |  1.00 |    0.08 |    1 |         - |          NA |
| PigeonSort          | 1024 | Random             |   5,837.7 ns |    442.83 ns |   231.61 ns |  1.47 |    0.10 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | Random             |   3,426.8 ns |      7.51 ns |     2.68 ns |  0.86 |    0.05 |    1 |         - |          NA |
| BucketSort          | 1024 | Random             |  15,299.2 ns |    599.71 ns |   313.66 ns |  3.84 |    0.24 |    4 |         - |          NA |
| BucketSortInteger   | 1024 | Random             |  14,956.8 ns |    179.43 ns |    93.84 ns |  3.76 |    0.22 |    4 |         - |          NA |
| FlashSort           | 1024 | Random             |  19,882.6 ns |    343.09 ns |   152.33 ns |  5.00 |    0.29 |    5 |         - |          NA |
| RadixLSD4Sort       | 1024 | Random             |  24,088.9 ns |    431.79 ns |   225.83 ns |  6.05 |    0.36 |    5 |         - |          NA |
| RadixLSD256Sort     | 1024 | Random             |  10,545.6 ns |    489.87 ns |   217.50 ns |  2.65 |    0.16 |    3 |         - |          NA |
| RadixLSD10Sort      | 1024 | Random             |  20,692.6 ns |    127.65 ns |    66.76 ns |  5.20 |    0.30 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | Random             |  48,453.7 ns |    470.48 ns |   246.07 ns | 12.17 |    0.71 |    6 |         - |          NA |
| RadixMSD10Sort      | 1024 | Random             |  51,052.1 ns |  1,296.13 ns |   575.49 ns | 12.83 |    0.76 |    6 |         - |          NA |
| AmericanFlagSort    | 1024 | Random             |  24,212.7 ns |    492.52 ns |   257.60 ns |  6.08 |    0.36 |    5 |         - |          NA |
| SpreadSort          | 1024 | Random             |   9,481.0 ns |    602.29 ns |   315.01 ns |  2.38 |    0.16 |    3 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **1024** | **SingleElementMoved** |   **6,212.2 ns** |    **408.04 ns** |   **213.41 ns** |  **1.74** |    **0.06** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | SingleElementMoved |   3,579.2 ns |     29.36 ns |    10.47 ns |  1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 1024 | SingleElementMoved |   5,733.5 ns |    620.51 ns |   324.54 ns |  1.60 |    0.09 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | SingleElementMoved |   3,439.0 ns |     84.79 ns |    37.65 ns |  0.96 |    0.01 |    1 |         - |          NA |
| BucketSort          | 1024 | SingleElementMoved |   9,544.9 ns |    333.22 ns |   174.28 ns |  2.67 |    0.05 |    3 |         - |          NA |
| BucketSortInteger   | 1024 | SingleElementMoved |   7,683.0 ns |    528.25 ns |   276.28 ns |  2.15 |    0.07 |    2 |         - |          NA |
| FlashSort           | 1024 | SingleElementMoved |  19,643.9 ns |    404.04 ns |   211.32 ns |  5.49 |    0.06 |    4 |         - |          NA |
| RadixLSD4Sort       | 1024 | SingleElementMoved |  24,622.2 ns |    428.22 ns |   223.97 ns |  6.88 |    0.06 |    4 |         - |          NA |
| RadixLSD256Sort     | 1024 | SingleElementMoved |  10,103.5 ns |    262.87 ns |   137.49 ns |  2.82 |    0.04 |    3 |         - |          NA |
| RadixLSD10Sort      | 1024 | SingleElementMoved |  21,038.6 ns |    310.64 ns |   137.93 ns |  5.88 |    0.04 |    4 |         - |          NA |
| RadixMSD4Sort       | 1024 | SingleElementMoved |  43,644.7 ns |     77.10 ns |    27.49 ns | 12.19 |    0.03 |    5 |         - |          NA |
| RadixMSD10Sort      | 1024 | SingleElementMoved |  48,176.3 ns |    740.91 ns |   264.22 ns | 13.46 |    0.08 |    5 |         - |          NA |
| AmericanFlagSort    | 1024 | SingleElementMoved |  17,272.3 ns |    335.23 ns |   175.33 ns |  4.83 |    0.05 |    4 |         - |          NA |
| SpreadSort          | 1024 | SingleElementMoved |   7,103.1 ns |    290.70 ns |   129.07 ns |  1.98 |    0.03 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **1024** | **Sorted**             |   **6,230.6 ns** |    **168.10 ns** |    **87.92 ns** |  **1.84** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Sorted             |   3,393.7 ns |     13.26 ns |     4.73 ns |  1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | Sorted             |   5,535.7 ns |    507.35 ns |   265.35 ns |  1.63 |    0.07 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | Sorted             |   3,414.1 ns |      8.99 ns |     3.99 ns |  1.01 |    0.00 |    2 |         - |          NA |
| BucketSort          | 1024 | Sorted             |   9,503.6 ns |    387.95 ns |   202.90 ns |  2.80 |    0.06 |    4 |         - |          NA |
| BucketSortInteger   | 1024 | Sorted             |   7,238.4 ns |    256.31 ns |   113.81 ns |  2.13 |    0.03 |    3 |         - |          NA |
| FlashSort           | 1024 | Sorted             |  18,821.0 ns |    223.79 ns |    99.37 ns |  5.55 |    0.03 |    5 |         - |          NA |
| RadixLSD4Sort       | 1024 | Sorted             |  24,803.4 ns |    374.97 ns |   166.49 ns |  7.31 |    0.05 |    5 |         - |          NA |
| RadixLSD256Sort     | 1024 | Sorted             |   9,991.3 ns |    481.82 ns |   252.00 ns |  2.94 |    0.07 |    4 |         - |          NA |
| RadixLSD10Sort      | 1024 | Sorted             |  21,020.6 ns |    573.50 ns |   299.95 ns |  6.19 |    0.08 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | Sorted             |  43,504.2 ns |    585.08 ns |   306.01 ns | 12.82 |    0.09 |    6 |         - |          NA |
| RadixMSD10Sort      | 1024 | Sorted             |  47,939.7 ns |    530.34 ns |   235.48 ns | 14.13 |    0.07 |    6 |         - |          NA |
| AmericanFlagSort    | 1024 | Sorted             |  16,889.7 ns |    289.01 ns |   151.16 ns |  4.98 |    0.04 |    5 |         - |          NA |
| SpreadSort          | 1024 | Sorted             |     700.0 ns |     10.31 ns |     4.58 ns |  0.21 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **1024** | **Reversed**           |   **6,014.7 ns** |    **413.57 ns** |   **216.31 ns** |  **1.75** |    **0.06** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Reversed           |   3,445.6 ns |     21.85 ns |     9.70 ns |  1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 1024 | Reversed           |   5,317.9 ns |    464.84 ns |   243.12 ns |  1.54 |    0.07 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | Reversed           |   3,400.5 ns |     54.59 ns |    19.47 ns |  0.99 |    0.01 |    1 |         - |          NA |
| BucketSort          | 1024 | Reversed           |  16,153.3 ns |     58.32 ns |    25.89 ns |  4.69 |    0.01 |    4 |         - |          NA |
| BucketSortInteger   | 1024 | Reversed           |  17,244.9 ns |    283.36 ns |   148.20 ns |  5.00 |    0.04 |    4 |         - |          NA |
| FlashSort           | 1024 | Reversed           |  16,699.1 ns |    411.75 ns |   215.35 ns |  4.85 |    0.06 |    4 |         - |          NA |
| RadixLSD4Sort       | 1024 | Reversed           |  24,601.3 ns |    424.51 ns |   222.03 ns |  7.14 |    0.06 |    5 |         - |          NA |
| RadixLSD256Sort     | 1024 | Reversed           |   9,899.2 ns |    417.79 ns |   218.51 ns |  2.87 |    0.06 |    3 |         - |          NA |
| RadixLSD10Sort      | 1024 | Reversed           |  20,734.1 ns |    420.12 ns |   219.73 ns |  6.02 |    0.06 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | Reversed           |  47,278.9 ns |    266.27 ns |   139.26 ns | 13.72 |    0.05 |    6 |         - |          NA |
| RadixMSD10Sort      | 1024 | Reversed           |  51,153.8 ns |  1,759.79 ns |   920.40 ns | 14.85 |    0.26 |    6 |         - |          NA |
| AmericanFlagSort    | 1024 | Reversed           |  23,816.0 ns |    737.10 ns |   385.52 ns |  6.91 |    0.11 |    5 |         - |          NA |
| SpreadSort          | 1024 | Reversed           |   5,886.3 ns |    604.36 ns |   268.34 ns |  1.71 |    0.07 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **1024** | **PipeOrgan**          |   **5,499.7 ns** |     **13.17 ns** |     **4.70 ns** |  **1.60** |    **0.15** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | PipeOrgan          |   3,474.4 ns |    674.38 ns |   352.71 ns |  1.01 |    0.13 |    1 |         - |          NA |
| PigeonSort          | 1024 | PipeOrgan          |   5,942.3 ns |    429.03 ns |   224.39 ns |  1.73 |    0.17 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | PipeOrgan          |   3,258.0 ns |     16.73 ns |     5.96 ns |  0.95 |    0.09 |    1 |         - |          NA |
| BucketSort          | 1024 | PipeOrgan          |  14,184.3 ns |    224.39 ns |    99.63 ns |  4.12 |    0.38 |    4 |         - |          NA |
| BucketSortInteger   | 1024 | PipeOrgan          |  12,169.1 ns |    228.16 ns |   101.30 ns |  3.53 |    0.32 |    4 |         - |          NA |
| FlashSort           | 1024 | PipeOrgan          |  17,852.1 ns |    259.45 ns |    92.52 ns |  5.18 |    0.47 |    5 |         - |          NA |
| RadixLSD4Sort       | 1024 | PipeOrgan          |  25,190.0 ns |    665.20 ns |   295.35 ns |  7.31 |    0.67 |    5 |         - |          NA |
| RadixLSD256Sort     | 1024 | PipeOrgan          |  10,261.2 ns |    383.46 ns |   200.56 ns |  2.98 |    0.28 |    4 |         - |          NA |
| RadixLSD10Sort      | 1024 | PipeOrgan          |  20,998.1 ns |    516.03 ns |   269.89 ns |  6.10 |    0.56 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | PipeOrgan          |  52,462.0 ns |  1,373.74 ns |   609.95 ns | 15.23 |    1.40 |    6 |         - |          NA |
| RadixMSD10Sort      | 1024 | PipeOrgan          |  50,791.2 ns |    865.09 ns |   384.11 ns | 14.75 |    1.35 |    6 |         - |          NA |
| AmericanFlagSort    | 1024 | PipeOrgan          |  26,281.0 ns |    915.75 ns |   478.95 ns |  7.63 |    0.71 |    5 |         - |          NA |
| SpreadSort          | 1024 | PipeOrgan          |   7,512.4 ns |    403.07 ns |   210.81 ns |  2.18 |    0.21 |    3 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **8192** | **Random**             |  **54,417.3 ns** |  **1,337.19 ns** |   **699.38 ns** |  **1.59** |    **0.02** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Random             |  34,196.4 ns |    318.73 ns |   113.66 ns |  1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 8192 | Random             |  47,979.8 ns |    912.20 ns |   405.02 ns |  1.40 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | Random             |  29,763.3 ns |    827.14 ns |   367.26 ns |  0.87 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | Random             | 224,077.6 ns |  6,016.86 ns | 3,146.94 ns |  6.55 |    0.09 |    5 |         - |          NA |
| BucketSortInteger   | 8192 | Random             | 246,324.1 ns |  2,593.24 ns | 1,356.31 ns |  7.20 |    0.04 |    5 |         - |          NA |
| FlashSort           | 8192 | Random             | 156,529.8 ns |  2,625.77 ns | 1,373.33 ns |  4.58 |    0.04 |    4 |         - |          NA |
| RadixLSD4Sort       | 8192 | Random             | 235,715.4 ns |    696.40 ns |   309.21 ns |  6.89 |    0.02 |    5 |         - |          NA |
| RadixLSD256Sort     | 8192 | Random             |  55,996.9 ns |  1,768.43 ns |   785.19 ns |  1.64 |    0.02 |    2 |         - |          NA |
| RadixLSD10Sort      | 8192 | Random             | 175,137.6 ns |  1,356.42 ns |   709.43 ns |  5.12 |    0.03 |    4 |         - |          NA |
| RadixMSD4Sort       | 8192 | Random             | 463,668.4 ns |  1,230.39 ns |   546.30 ns | 13.56 |    0.04 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | Random             | 449,873.1 ns | 11,734.75 ns | 6,137.50 ns | 13.16 |    0.17 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | Random             | 293,956.6 ns |  1,008.99 ns |   527.72 ns |  8.60 |    0.03 |    5 |         - |          NA |
| SpreadSort          | 8192 | Random             |  85,233.6 ns |    974.23 ns |   432.56 ns |  2.49 |    0.01 |    3 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **8192** | **SingleElementMoved** |  **51,728.7 ns** |  **1,402.57 ns** |   **733.57 ns** |  **1.79** |    **0.03** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | SingleElementMoved |  28,920.5 ns |    290.63 ns |   129.04 ns |  1.00 |    0.01 |    1 |         - |          NA |
| PigeonSort          | 8192 | SingleElementMoved |  44,327.5 ns |  1,193.14 ns |   529.76 ns |  1.53 |    0.02 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | SingleElementMoved |  27,834.5 ns |    346.13 ns |   123.43 ns |  0.96 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | SingleElementMoved |  67,530.5 ns |  1,154.70 ns |   512.70 ns |  2.34 |    0.02 |    3 |         - |          NA |
| BucketSortInteger   | 8192 | SingleElementMoved |  49,137.2 ns |  1,238.05 ns |   549.70 ns |  1.70 |    0.02 |    2 |         - |          NA |
| FlashSort           | 8192 | SingleElementMoved | 158,480.0 ns |  1,741.41 ns |   910.79 ns |  5.48 |    0.04 |    4 |         - |          NA |
| RadixLSD4Sort       | 8192 | SingleElementMoved | 237,138.3 ns |  3,946.27 ns | 2,063.97 ns |  8.20 |    0.08 |    5 |         - |          NA |
| RadixLSD256Sort     | 8192 | SingleElementMoved |  54,140.4 ns |    889.48 ns |   394.93 ns |  1.87 |    0.02 |    2 |         - |          NA |
| RadixLSD10Sort      | 8192 | SingleElementMoved | 167,739.4 ns |  8,057.20 ns | 4,214.07 ns |  5.80 |    0.14 |    4 |         - |          NA |
| RadixMSD4Sort       | 8192 | SingleElementMoved | 347,611.6 ns |  1,802.15 ns |   800.16 ns | 12.02 |    0.06 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | SingleElementMoved | 391,935.9 ns | 17,685.13 ns | 9,249.67 ns | 13.55 |    0.31 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | SingleElementMoved | 147,489.5 ns |    937.88 ns |   490.53 ns |  5.10 |    0.03 |    4 |         - |          NA |
| SpreadSort          | 8192 | SingleElementMoved |  46,520.5 ns |  1,223.96 ns |   543.44 ns |  1.61 |    0.02 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **8192** | **Sorted**             |  **46,705.6 ns** |  **1,670.15 ns** |   **873.52 ns** |  **1.65** |    **0.04** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Sorted             |  28,235.8 ns |    985.05 ns |   437.37 ns |  1.00 |    0.02 |    2 |         - |          NA |
| PigeonSort          | 8192 | Sorted             |  41,357.5 ns |    524.01 ns |   232.66 ns |  1.47 |    0.02 |    3 |         - |          NA |
| PigeonSortInteger   | 8192 | Sorted             |  28,158.1 ns |  1,241.99 ns |   551.45 ns |  1.00 |    0.02 |    2 |         - |          NA |
| BucketSort          | 8192 | Sorted             |  65,954.9 ns |  1,525.14 ns |   677.17 ns |  2.34 |    0.04 |    4 |         - |          NA |
| BucketSortInteger   | 8192 | Sorted             |  47,487.9 ns |    542.81 ns |   241.01 ns |  1.68 |    0.03 |    3 |         - |          NA |
| FlashSort           | 8192 | Sorted             | 153,380.7 ns |  1,117.32 ns |   496.10 ns |  5.43 |    0.08 |    5 |         - |          NA |
| RadixLSD4Sort       | 8192 | Sorted             | 236,135.7 ns |  1,124.10 ns |   587.93 ns |  8.36 |    0.12 |    6 |         - |          NA |
| RadixLSD256Sort     | 8192 | Sorted             |  53,065.3 ns |  1,014.86 ns |   450.60 ns |  1.88 |    0.03 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | Sorted             | 166,452.6 ns |  7,396.45 ns | 3,868.48 ns |  5.90 |    0.15 |    5 |         - |          NA |
| RadixMSD4Sort       | 8192 | Sorted             | 345,600.4 ns |  1,683.03 ns |   880.26 ns | 12.24 |    0.18 |    7 |         - |          NA |
| RadixMSD10Sort      | 8192 | Sorted             | 385,229.7 ns | 10,589.60 ns | 4,701.85 ns | 13.65 |    0.25 |    7 |         - |          NA |
| AmericanFlagSort    | 8192 | Sorted             | 144,758.9 ns |  1,047.08 ns |   464.91 ns |  5.13 |    0.08 |    5 |         - |          NA |
| SpreadSort          | 8192 | Sorted             |   5,379.3 ns |    618.97 ns |   274.83 ns |  0.19 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **8192** | **Reversed**           |  **46,834.8 ns** |  **2,207.98 ns** | **1,154.82 ns** |  **1.67** |    **0.04** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Reversed           |  28,102.8 ns |    752.14 ns |   333.96 ns |  1.00 |    0.02 |    1 |         - |          NA |
| PigeonSort          | 8192 | Reversed           |  41,545.9 ns |  1,810.34 ns |   946.84 ns |  1.48 |    0.04 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | Reversed           |  25,448.4 ns |  1,786.99 ns |   934.63 ns |  0.91 |    0.03 |    1 |         - |          NA |
| BucketSort          | 8192 | Reversed           | 259,921.7 ns |  1,803.42 ns |   943.22 ns |  9.25 |    0.11 |    5 |         - |          NA |
| BucketSortInteger   | 8192 | Reversed           | 309,253.1 ns |  2,417.76 ns | 1,073.50 ns | 11.01 |    0.13 |    5 |         - |          NA |
| FlashSort           | 8192 | Reversed           | 133,675.8 ns |  1,070.56 ns |   559.92 ns |  4.76 |    0.06 |    3 |         - |          NA |
| RadixLSD4Sort       | 8192 | Reversed           | 236,414.3 ns |    899.60 ns |   399.43 ns |  8.41 |    0.09 |    5 |         - |          NA |
| RadixLSD256Sort     | 8192 | Reversed           |  53,227.2 ns |  1,426.54 ns |   746.11 ns |  1.89 |    0.03 |    2 |         - |          NA |
| RadixLSD10Sort      | 8192 | Reversed           | 168,710.6 ns |  8,081.27 ns | 3,588.13 ns |  6.00 |    0.14 |    4 |         - |          NA |
| RadixMSD4Sort       | 8192 | Reversed           | 377,326.4 ns |  1,739.73 ns |   909.91 ns | 13.43 |    0.15 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | Reversed           | 412,429.4 ns | 16,185.59 ns | 8,465.37 ns | 14.68 |    0.33 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | Reversed           | 206,760.3 ns |  2,225.76 ns |   988.25 ns |  7.36 |    0.09 |    5 |         - |          NA |
| SpreadSort          | 8192 | Reversed           |  62,437.5 ns |  2,011.87 ns |   893.28 ns |  2.22 |    0.04 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **8192** | **PipeOrgan**          |  **46,708.7 ns** |  **2,279.71 ns** | **1,012.21 ns** |  **1.76** |    **0.06** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | PipeOrgan          |  26,624.2 ns |  1,436.33 ns |   751.23 ns |  1.00 |    0.04 |    1 |         - |          NA |
| PigeonSort          | 8192 | PipeOrgan          |  41,579.1 ns |  1,386.86 ns |   615.78 ns |  1.56 |    0.05 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | PipeOrgan          |  27,546.5 ns |  1,232.07 ns |   547.05 ns |  1.04 |    0.03 |    1 |         - |          NA |
| BucketSort          | 8192 | PipeOrgan          | 209,504.8 ns |  9,823.90 ns | 3,503.30 ns |  7.87 |    0.24 |    4 |         - |          NA |
| BucketSortInteger   | 8192 | PipeOrgan          | 191,699.8 ns |    466.52 ns |   207.14 ns |  7.21 |    0.19 |    4 |         - |          NA |
| FlashSort           | 8192 | PipeOrgan          | 139,425.0 ns |  1,546.29 ns |   686.56 ns |  5.24 |    0.14 |    4 |         - |          NA |
| RadixLSD4Sort       | 8192 | PipeOrgan          | 235,752.6 ns |  2,511.43 ns | 1,115.09 ns |  8.86 |    0.23 |    4 |         - |          NA |
| RadixLSD256Sort     | 8192 | PipeOrgan          |  78,900.3 ns |  1,353.83 ns |   601.11 ns |  2.97 |    0.08 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | PipeOrgan          | 161,984.0 ns |  4,244.25 ns | 2,219.83 ns |  6.09 |    0.18 |    4 |         - |          NA |
| RadixMSD4Sort       | 8192 | PipeOrgan          | 408,888.6 ns |  9,217.61 ns | 4,820.99 ns | 15.37 |    0.44 |    5 |         - |          NA |
| RadixMSD10Sort      | 8192 | PipeOrgan          | 404,117.4 ns | 16,486.44 ns | 7,320.08 ns | 15.19 |    0.47 |    5 |         - |          NA |
| AmericanFlagSort    | 8192 | PipeOrgan          | 230,522.3 ns |  2,313.67 ns | 1,027.28 ns |  8.66 |    0.23 |    4 |         - |          NA |
| SpreadSort          | 8192 | PipeOrgan          |  81,924.2 ns |    813.74 ns |   361.31 ns |  3.08 |    0.08 |    3 |         - |          NA |

### ExchangeBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method             | Size | Pattern            | Mean         | Error       | StdDev      | Ratio  | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------- |----- |------------------- |-------------:|------------:|------------:|-------:|--------:|-----:|----------:|------------:|
| **BubbleSort**         | **256**  | **Random**             |  **32,365.0 ns** |   **477.71 ns** |   **212.10 ns** |   **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Random             |  16,245.3 ns |   230.08 ns |   120.33 ns |   0.50 |    0.00 |    2 |         - |          NA |
| OddEvenSort        | 256  | Random             |  26,203.0 ns |   435.29 ns |   227.66 ns |   0.81 |    0.01 |    3 |         - |          NA |
| CombSort           | 256  | Random             |   3,360.4 ns |    51.59 ns |    18.40 ns |   0.10 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | Random             |  17,635.9 ns |   115.66 ns |    51.35 ns |   0.54 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **SingleElementMoved** |     **405.3 ns** |     **2.77 ns** |     **1.23 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 256  | SingleElementMoved |     308.7 ns |     1.71 ns |     0.90 ns |   0.76 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 256  | SingleElementMoved |  15,637.0 ns |   109.76 ns |    48.73 ns |  38.58 |    0.16 |    4 |         - |          NA |
| CombSort           | 256  | SingleElementMoved |   2,807.3 ns |     6.32 ns |     2.25 ns |   6.93 |    0.02 |    3 |         - |          NA |
| CircleSort         | 256  | SingleElementMoved |  15,119.5 ns |   260.89 ns |   136.45 ns |  37.30 |    0.33 |    4 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **Sorted**             |     **244.7 ns** |     **1.50 ns** |     **0.66 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Sorted             |     169.3 ns |     5.60 ns |     2.49 ns |   0.69 |    0.01 |    1 |         - |          NA |
| OddEvenSort        | 256  | Sorted             |     212.8 ns |     2.12 ns |     1.11 ns |   0.87 |    0.00 |    2 |         - |          NA |
| CombSort           | 256  | Sorted             |   2,555.5 ns |   319.17 ns |   141.71 ns |  10.44 |    0.54 |    3 |         - |          NA |
| CircleSort         | 256  | Sorted             |   2,079.0 ns |     2.80 ns |     1.46 ns |   8.50 |    0.02 |    3 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **Reversed**           |  **27,615.8 ns** |   **297.05 ns** |   **155.36 ns** |   **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Reversed           |  25,059.9 ns |   261.82 ns |   136.94 ns |   0.91 |    0.01 |    3 |         - |          NA |
| OddEvenSort        | 256  | Reversed           |  24,459.2 ns |   385.71 ns |   201.74 ns |   0.89 |    0.01 |    3 |         - |          NA |
| CombSort           | 256  | Reversed           |   3,129.1 ns |    53.25 ns |    18.99 ns |   0.11 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | Reversed           |   4,171.0 ns |    67.36 ns |    24.02 ns |   0.15 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **PipeOrgan**          |  **26,323.4 ns** |   **396.35 ns** |   **175.98 ns** |   **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | PipeOrgan          |  16,970.7 ns |   163.85 ns |    72.75 ns |   0.64 |    0.00 |    2 |         - |          NA |
| OddEvenSort        | 256  | PipeOrgan          |  39,613.0 ns | 2,122.58 ns | 1,110.15 ns |   1.50 |    0.04 |    4 |         - |          NA |
| CombSort           | 256  | PipeOrgan          |   3,017.0 ns |    19.14 ns |     6.82 ns |   0.11 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | PipeOrgan          |  19,378.4 ns |   297.61 ns |   132.14 ns |   0.74 |    0.01 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Random**             | **601,232.8 ns** | **2,848.44 ns** | **1,264.72 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Random             | 312,419.0 ns | 1,436.54 ns |   637.83 ns |   0.52 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | Random             | 532,214.9 ns | 5,885.02 ns | 3,077.98 ns |   0.89 |    0.01 |    4 |         - |          NA |
| CombSort           | 1024 | Random             |  31,838.3 ns |   490.05 ns |   256.30 ns |   0.05 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | Random             |  97,517.2 ns | 1,169.20 ns |   519.13 ns |   0.16 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **SingleElementMoved** |   **1,534.5 ns** |     **2.65 ns** |     **0.95 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | SingleElementMoved |   1,147.1 ns |     2.68 ns |     1.40 ns |   0.75 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 1024 | SingleElementMoved | 223,011.0 ns | 1,240.17 ns |   550.64 ns | 145.33 |    0.35 |    5 |         - |          NA |
| CombSort           | 1024 | SingleElementMoved |  15,001.7 ns |   277.11 ns |   144.93 ns |   9.78 |    0.09 |    3 |         - |          NA |
| CircleSort         | 1024 | SingleElementMoved |  83,739.1 ns |   647.17 ns |   338.48 ns |  54.57 |    0.21 |    4 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Sorted**             |     **955.4 ns** |     **0.81 ns** |     **0.36 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Sorted             |     642.9 ns |     2.49 ns |     1.30 ns |   0.67 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 1024 | Sorted             |     808.1 ns |     5.73 ns |     3.00 ns |   0.85 |    0.00 |    2 |         - |          NA |
| CombSort           | 1024 | Sorted             |  12,899.9 ns |   242.06 ns |   107.48 ns |  13.50 |    0.11 |    4 |         - |          NA |
| CircleSort         | 1024 | Sorted             |   9,280.0 ns |   464.72 ns |   243.06 ns |   9.71 |    0.24 |    3 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Reversed**           | **414,780.7 ns** | **1,613.44 ns** |   **843.86 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Reversed           | 386,492.1 ns | 1,286.46 ns |   672.84 ns |   0.93 |    0.00 |    2 |         - |          NA |
| OddEvenSort        | 1024 | Reversed           | 367,978.8 ns |   880.71 ns |   391.04 ns |   0.89 |    0.00 |    2 |         - |          NA |
| CombSort           | 1024 | Reversed           |  16,500.8 ns |    69.30 ns |    30.77 ns |   0.04 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | Reversed           |  18,834.0 ns |   844.31 ns |   441.59 ns |   0.05 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **PipeOrgan**          | **384,328.9 ns** | **3,035.27 ns** | **1,347.68 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | PipeOrgan          | 253,362.3 ns | 1,420.60 ns |   630.76 ns |   0.66 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | PipeOrgan          | 360,728.1 ns | 1,910.81 ns |   999.39 ns |   0.94 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | PipeOrgan          |  17,169.1 ns |   373.79 ns |   195.50 ns |   0.04 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | PipeOrgan          | 103,019.8 ns |   466.39 ns |   207.08 ns |   0.27 |    0.00 |    2 |         - |          NA |

### HeapBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method           | Size | Pattern            | Mean           | Error        | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------- |----- |------------------- |---------------:|-------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **HeapSort**         | **256**  | **Random**             |     **3,952.3 ns** |    **378.04 ns** |   **167.85 ns** |  **1.00** |    **0.06** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | Random             |     3,477.6 ns |    343.07 ns |   152.32 ns |  0.88 |    0.05 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | Random             |     4,053.3 ns |    262.19 ns |   116.41 ns |  1.03 |    0.05 |    1 |         - |          NA |
| BottomupHeapSort | 256  | Random             |     4,026.2 ns |    183.89 ns |    81.65 ns |  1.02 |    0.04 |    1 |         - |          NA |
| WeakHeapSort     | 256  | Random             |     9,132.6 ns |    389.66 ns |   203.80 ns |  2.31 |    0.10 |    3 |         - |          NA |
| SmoothSort       | 256  | Random             |     5,681.4 ns |    327.36 ns |   171.22 ns |  1.44 |    0.07 |    2 |         - |          NA |
| TournamentSort   | 256  | Random             |     7,868.2 ns |    400.22 ns |   209.32 ns |  1.99 |    0.09 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **HeapSort**         | **256**  | **SingleElementMoved** |     **4,004.3 ns** |    **280.10 ns** |   **146.50 ns** |  **1.00** |    **0.05** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 256  | SingleElementMoved |     3,463.0 ns |    248.42 ns |   129.93 ns |  0.87 |    0.04 |    2 |         - |          NA |
| TernaryHeapSort  | 256  | SingleElementMoved |     4,232.2 ns |    344.80 ns |   180.34 ns |  1.06 |    0.06 |    2 |         - |          NA |
| BottomupHeapSort | 256  | SingleElementMoved |     4,089.5 ns |    344.13 ns |   152.80 ns |  1.02 |    0.05 |    2 |         - |          NA |
| WeakHeapSort     | 256  | SingleElementMoved |     7,633.4 ns |     25.87 ns |    13.53 ns |  1.91 |    0.06 |    4 |         - |          NA |
| SmoothSort       | 256  | SingleElementMoved |     1,963.9 ns |    503.79 ns |   263.49 ns |  0.49 |    0.06 |    1 |         - |          NA |
| TournamentSort   | 256  | SingleElementMoved |     5,355.6 ns |    455.73 ns |   238.36 ns |  1.34 |    0.07 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **HeapSort**         | **256**  | **Sorted**             |     **3,917.4 ns** |    **267.61 ns** |   **139.96 ns** |  **1.00** |    **0.05** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 256  | Sorted             |     3,441.1 ns |     75.54 ns |    33.54 ns |  0.88 |    0.03 |    2 |         - |          NA |
| TernaryHeapSort  | 256  | Sorted             |     6,111.9 ns |  3,908.88 ns | 2,044.42 ns |  1.56 |    0.50 |    3 |         - |          NA |
| BottomupHeapSort | 256  | Sorted             |     4,190.6 ns |    415.04 ns |   217.07 ns |  1.07 |    0.06 |    2 |         - |          NA |
| WeakHeapSort     | 256  | Sorted             |     8,154.1 ns |    207.40 ns |   108.47 ns |  2.08 |    0.08 |    4 |         - |          NA |
| SmoothSort       | 256  | Sorted             |     1,240.0 ns |     31.18 ns |    11.12 ns |  0.32 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 256  | Sorted             |     3,270.9 ns |    259.07 ns |   135.50 ns |  0.84 |    0.04 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **HeapSort**         | **256**  | **Reversed**           |     **3,863.2 ns** |    **214.89 ns** |    **95.41 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | Reversed           |     3,372.2 ns |    242.98 ns |   127.08 ns |  0.87 |    0.04 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | Reversed           |     4,104.8 ns |    349.30 ns |   155.09 ns |  1.06 |    0.04 |    1 |         - |          NA |
| BottomupHeapSort | 256  | Reversed           |     4,377.1 ns |    494.79 ns |   258.78 ns |  1.13 |    0.07 |    1 |         - |          NA |
| WeakHeapSort     | 256  | Reversed           |     8,740.5 ns |    319.08 ns |   166.89 ns |  2.26 |    0.06 |    2 |         - |          NA |
| SmoothSort       | 256  | Reversed           |     4,873.1 ns |    515.44 ns |   269.59 ns |  1.26 |    0.07 |    1 |         - |          NA |
| TournamentSort   | 256  | Reversed           |     4,911.1 ns |    389.47 ns |   172.93 ns |  1.27 |    0.05 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **HeapSort**         | **256**  | **PipeOrgan**          |     **3,005.2 ns** |     **61.72 ns** |    **27.41 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | PipeOrgan          |     3,063.2 ns |    104.96 ns |    46.60 ns |  1.02 |    0.02 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | PipeOrgan          |     3,799.1 ns |    244.58 ns |   108.60 ns |  1.26 |    0.04 |    2 |         - |          NA |
| BottomupHeapSort | 256  | PipeOrgan          |     4,184.3 ns |    444.10 ns |   232.28 ns |  1.39 |    0.07 |    2 |         - |          NA |
| WeakHeapSort     | 256  | PipeOrgan          |     8,549.3 ns |    295.00 ns |   154.29 ns |  2.85 |    0.05 |    4 |         - |          NA |
| SmoothSort       | 256  | PipeOrgan          |     5,622.5 ns |    400.32 ns |   209.37 ns |  1.87 |    0.07 |    3 |         - |          NA |
| TournamentSort   | 256  | PipeOrgan          |     7,483.8 ns |    266.68 ns |   139.48 ns |  2.49 |    0.05 |    4 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **HeapSort**         | **1024** | **Random**             |    **19,710.9 ns** |    **513.72 ns** |   **268.68 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Random             |    17,973.8 ns |    896.16 ns |   397.90 ns |  0.91 |    0.02 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | Random             |    20,641.5 ns |    408.93 ns |   213.88 ns |  1.05 |    0.02 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | Random             |    19,478.0 ns |    411.93 ns |   215.45 ns |  0.99 |    0.02 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | Random             |    48,186.9 ns |    592.76 ns |   310.03 ns |  2.45 |    0.03 |    3 |         - |          NA |
| SmoothSort       | 1024 | Random             |    27,069.3 ns |    630.68 ns |   329.86 ns |  1.37 |    0.02 |    2 |         - |          NA |
| TournamentSort   | 1024 | Random             |    47,051.0 ns | 13,773.76 ns | 7,203.94 ns |  2.39 |    0.35 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **HeapSort**         | **1024** | **SingleElementMoved** |    **22,166.7 ns** |  **1,052.03 ns** |   **467.11 ns** |  **1.00** |    **0.03** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | SingleElementMoved |    17,124.8 ns |    266.33 ns |   118.25 ns |  0.77 |    0.02 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | SingleElementMoved |    20,804.7 ns |  1,118.53 ns |   496.63 ns |  0.94 |    0.03 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | SingleElementMoved |    19,404.0 ns |    682.05 ns |   356.73 ns |  0.88 |    0.02 |    2 |         - |          NA |
| WeakHeapSort     | 1024 | SingleElementMoved |    40,511.8 ns |    402.60 ns |   210.57 ns |  1.83 |    0.04 |    3 |         - |          NA |
| SmoothSort       | 1024 | SingleElementMoved |     7,202.9 ns |     59.50 ns |    31.12 ns |  0.33 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 1024 | SingleElementMoved |    24,888.5 ns |  2,273.79 ns | 1,009.58 ns |  1.12 |    0.05 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **HeapSort**         | **1024** | **Sorted**             |    **22,452.8 ns** |  **1,014.76 ns** |   **530.74 ns** |  **1.00** |    **0.03** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Sorted             |    16,902.2 ns |    117.47 ns |    52.16 ns |  0.75 |    0.02 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | Sorted             |    22,730.4 ns |  1,395.70 ns |   729.98 ns |  1.01 |    0.04 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | Sorted             |    19,431.3 ns |    589.19 ns |   308.16 ns |  0.87 |    0.02 |    2 |         - |          NA |
| WeakHeapSort     | 1024 | Sorted             |    41,143.1 ns |    395.98 ns |   207.10 ns |  1.83 |    0.04 |    3 |         - |          NA |
| SmoothSort       | 1024 | Sorted             |     5,213.7 ns |    627.63 ns |   328.26 ns |  0.23 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 1024 | Sorted             |    16,118.4 ns |  2,273.77 ns | 1,189.23 ns |  0.72 |    0.05 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **HeapSort**         | **1024** | **Reversed**           |    **19,407.7 ns** |    **827.37 ns** |   **432.73 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Reversed           |    18,378.5 ns |  1,522.31 ns |   675.92 ns |  0.95 |    0.04 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | Reversed           |    19,502.7 ns |    897.25 ns |   469.28 ns |  1.01 |    0.03 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | Reversed           |    19,956.3 ns |    390.61 ns |   204.30 ns |  1.03 |    0.02 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | Reversed           |    44,672.7 ns |    664.85 ns |   295.20 ns |  2.30 |    0.05 |    2 |         - |          NA |
| SmoothSort       | 1024 | Reversed           |    23,289.7 ns |    525.70 ns |   274.95 ns |  1.20 |    0.03 |    1 |         - |          NA |
| TournamentSort   | 1024 | Reversed           |    25,217.7 ns |  1,103.97 ns |   490.17 ns |  1.30 |    0.04 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **HeapSort**         | **1024** | **PipeOrgan**          |    **15,931.3 ns** |    **434.54 ns** |   **192.94 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | PipeOrgan          |    16,155.6 ns |    303.93 ns |   134.94 ns |  1.01 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | PipeOrgan          |    18,383.3 ns |    515.15 ns |   269.43 ns |  1.15 |    0.02 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | PipeOrgan          |    19,637.8 ns |    472.73 ns |   209.89 ns |  1.23 |    0.02 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | PipeOrgan          |    44,042.7 ns |    591.54 ns |   309.39 ns |  2.76 |    0.04 |    3 |         - |          NA |
| SmoothSort       | 1024 | PipeOrgan          |    27,848.4 ns |    570.99 ns |   298.64 ns |  1.75 |    0.03 |    2 |         - |          NA |
| TournamentSort   | 1024 | PipeOrgan          |    40,486.8 ns |  4,821.92 ns | 2,140.96 ns |  2.54 |    0.13 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **HeapSort**         | **8192** | **Random**             |   **498,941.8 ns** |    **933.72 ns** |   **488.35 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Random             |   487,577.5 ns |  1,223.99 ns |   436.49 ns |  0.98 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | Random             |   583,423.9 ns |  1,499.30 ns |   665.70 ns |  1.17 |    0.00 |    1 |         - |          NA |
| BottomupHeapSort | 8192 | Random             |   580,587.6 ns |  1,844.34 ns |   818.90 ns |  1.16 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | Random             |   887,284.2 ns |  1,343.57 ns |   702.71 ns |  1.78 |    0.00 |    2 |         - |          NA |
| SmoothSort       | 8192 | Random             |   818,217.0 ns |  3,133.90 ns | 1,639.09 ns |  1.64 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 8192 | Random             | 1,353,296.6 ns |  8,070.89 ns | 4,221.23 ns |  2.71 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **HeapSort**         | **8192** | **SingleElementMoved** |   **370,966.7 ns** |  **2,010.59 ns** |   **892.72 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 8192 | SingleElementMoved |   381,712.6 ns |  1,810.35 ns |   946.85 ns |  1.03 |    0.00 |    2 |         - |          NA |
| TernaryHeapSort  | 8192 | SingleElementMoved |   414,725.0 ns |  3,094.15 ns | 1,618.30 ns |  1.12 |    0.00 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | SingleElementMoved |   446,916.8 ns |  1,024.33 ns |   535.74 ns |  1.20 |    0.00 |    2 |         - |          NA |
| WeakHeapSort     | 8192 | SingleElementMoved |   433,223.3 ns |  1,317.42 ns |   689.04 ns |  1.17 |    0.00 |    2 |         - |          NA |
| SmoothSort       | 8192 | SingleElementMoved |    59,378.2 ns |  2,783.13 ns | 1,455.63 ns |  0.16 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 8192 | SingleElementMoved |   779,625.9 ns |  8,412.22 ns | 4,399.75 ns |  2.10 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **HeapSort**         | **8192** | **Sorted**             |   **370,047.1 ns** |  **2,765.79 ns** | **1,446.56 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Sorted             |   381,908.9 ns |  1,293.91 ns |   676.74 ns |  1.03 |    0.00 |    2 |         - |          NA |
| TernaryHeapSort  | 8192 | Sorted             |   414,391.6 ns |  2,641.32 ns | 1,172.76 ns |  1.12 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | Sorted             |   447,715.1 ns |    903.96 ns |   401.36 ns |  1.21 |    0.00 |    2 |         - |          NA |
| WeakHeapSort     | 8192 | Sorted             |   442,148.6 ns |  1,872.95 ns |   979.59 ns |  1.19 |    0.01 |    2 |         - |          NA |
| SmoothSort       | 8192 | Sorted             |    42,511.4 ns |  2,574.72 ns | 1,346.63 ns |  0.11 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 8192 | Sorted             |   518,654.6 ns |  8,134.06 ns | 4,254.27 ns |  1.40 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **HeapSort**         | **8192** | **Reversed**           |   **398,394.2 ns** |  **4,130.93 ns** | **2,160.56 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Reversed           |   355,589.6 ns |  3,946.57 ns | 1,752.30 ns |  0.89 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | Reversed           |   425,392.7 ns |  1,393.15 ns |   728.65 ns |  1.07 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 8192 | Reversed           |   479,786.0 ns |  1,276.97 ns |   566.98 ns |  1.20 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | Reversed           |   473,245.8 ns |  2,084.14 ns | 1,090.05 ns |  1.19 |    0.01 |    1 |         - |          NA |
| SmoothSort       | 8192 | Reversed           |   509,155.7 ns |  2,945.81 ns | 1,540.71 ns |  1.28 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 8192 | Reversed           |   732,362.6 ns |  2,479.23 ns | 1,100.79 ns |  1.84 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **HeapSort**         | **8192** | **PipeOrgan**          |   **370,120.1 ns** |  **1,461.36 ns** |   **764.32 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | PipeOrgan          |   381,889.3 ns |  1,833.73 ns |   814.19 ns |  1.03 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | PipeOrgan          |   453,720.0 ns |  1,223.30 ns |   543.15 ns |  1.23 |    0.00 |    1 |         - |          NA |
| BottomupHeapSort | 8192 | PipeOrgan          |   455,005.0 ns |  1,353.75 ns |   601.07 ns |  1.23 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | PipeOrgan          |   505,191.5 ns |  2,000.58 ns |   888.27 ns |  1.36 |    0.00 |    1 |         - |          NA |
| SmoothSort       | 8192 | PipeOrgan          |   662,110.2 ns |  8,246.69 ns | 4,313.18 ns |  1.79 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 8192 | PipeOrgan          | 1,072,592.2 ns | 10,611.84 ns | 5,550.20 ns |  2.90 |    0.02 |    3 |         - |          NA |

### InsertionBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                 | Size | Pattern            | Mean         | Error        | StdDev       | Median       | Ratio  | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------- |----- |------------------- |-------------:|-------------:|-------------:|-------------:|-------:|--------:|-----:|----------:|------------:|
| **InsertionSort**          | **256**  | **Random**             |   **8,628.5 ns** |    **514.94 ns** |    **228.64 ns** |   **8,608.2 ns** |   **1.00** |    **0.04** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Random             |   8,905.8 ns |    474.04 ns |    247.93 ns |   8,842.7 ns |   1.03 |    0.04 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | Random             |   9,427.3 ns |    330.95 ns |    173.09 ns |   9,516.2 ns |   1.09 |    0.03 |    2 |         - |          NA |
| GnomeSort              | 256  | Random             |  27,865.3 ns |    425.13 ns |    188.76 ns |  27,901.4 ns |   3.23 |    0.08 |    4 |         - |          NA |
| LibrarySort            | 256  | Random             |  16,401.3 ns |    446.69 ns |    233.63 ns |  16,395.3 ns |   1.90 |    0.05 |    3 |         - |          NA |
| MergeInsertionSort     | 256  | Random             |  32,390.2 ns |  2,345.98 ns |  1,226.99 ns |  32,483.4 ns |   3.76 |    0.16 |    4 |         - |          NA |
| ShellSortKnuth1973     | 256  | Random             |   2,972.0 ns |     44.66 ns |     19.83 ns |   2,967.6 ns |   0.34 |    0.01 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Random             |   2,971.2 ns |     94.89 ns |     42.13 ns |   2,968.6 ns |   0.34 |    0.01 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | Random             |   3,156.5 ns |     26.30 ns |      9.38 ns |   3,156.5 ns |   0.37 |    0.01 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | Random             |   3,017.5 ns |    231.62 ns |    102.84 ns |   2,967.6 ns |   0.35 |    0.01 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Random             |   3,183.6 ns |    349.23 ns |    182.65 ns |   3,088.6 ns |   0.37 |    0.02 |    1 |         - |          NA |
|      |                    |              |              |              |              |        |         |      |           |             |
| **InsertionSort**          | **256**  | **SingleElementMoved** |     **427.6 ns** |      **3.81 ns** |      **1.99 ns** |     **426.7 ns** |   **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 256  | SingleElementMoved |     288.6 ns |      1.79 ns |      0.64 ns |     288.7 ns |   0.67 |    0.00 |    1 |         - |          NA |
| BinaryInsertSort       | 256  | SingleElementMoved |     971.6 ns |      2.46 ns |      0.88 ns |     971.9 ns |   2.27 |    0.01 |    3 |         - |          NA |
| GnomeSort              | 256  | SingleElementMoved |     470.0 ns |      7.06 ns |      3.69 ns |     471.0 ns |   1.10 |    0.01 |    2 |         - |          NA |
| LibrarySort            | 256  | SingleElementMoved |   8,574.6 ns |    285.89 ns |    149.53 ns |   8,643.7 ns |  20.05 |    0.34 |    6 |         - |          NA |
| MergeInsertionSort     | 256  | SingleElementMoved |  25,015.2 ns |    465.30 ns |    243.36 ns |  24,935.1 ns |  58.50 |    0.59 |    7 |         - |          NA |
| ShellSortKnuth1973     | 256  | SingleElementMoved |   1,296.3 ns |      4.71 ns |      1.68 ns |   1,295.9 ns |   3.03 |    0.01 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 256  | SingleElementMoved |   1,283.0 ns |      9.49 ns |      3.39 ns |   1,281.8 ns |   3.00 |    0.02 |    4 |         - |          NA |
| ShellSortTokuda1992    | 256  | SingleElementMoved |   1,598.9 ns |     18.16 ns |      8.06 ns |   1,599.3 ns |   3.74 |    0.02 |    5 |         - |          NA |
| ShellSortCiura2001     | 256  | SingleElementMoved |   1,996.4 ns |  1,576.91 ns |    824.75 ns |   1,448.5 ns |   4.67 |    1.82 |    5 |         - |          NA |
| ShellSortLee2021       | 256  | SingleElementMoved |   1,562.4 ns |     24.45 ns |      8.72 ns |   1,562.3 ns |   3.65 |    0.02 |    5 |         - |          NA |
|      |                    |              |              |              |              |        |         |      |           |             |
| **InsertionSort**          | **256**  | **Sorted**             |     **323.6 ns** |      **0.71 ns** |      **0.31 ns** |     **323.6 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Sorted             |     212.6 ns |      2.09 ns |      0.93 ns |     212.3 ns |   0.66 |    0.00 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | Sorted             |     172.9 ns |      1.85 ns |      0.66 ns |     172.9 ns |   0.53 |    0.00 |    1 |         - |          NA |
| GnomeSort              | 256  | Sorted             |     244.3 ns |      1.70 ns |      0.61 ns |     244.2 ns |   0.75 |    0.00 |    2 |         - |          NA |
| LibrarySort            | 256  | Sorted             |   7,097.9 ns |     93.95 ns |     33.50 ns |   7,093.6 ns |  21.93 |    0.10 |    6 |         - |          NA |
| MergeInsertionSort     | 256  | Sorted             |  23,644.2 ns |  1,333.64 ns |    592.15 ns |  23,452.5 ns |  73.06 |    1.71 |    7 |         - |          NA |
| ShellSortKnuth1973     | 256  | Sorted             |   1,066.7 ns |      3.20 ns |      1.42 ns |   1,066.2 ns |   3.30 |    0.01 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Sorted             |   1,072.2 ns |      4.45 ns |      2.33 ns |   1,070.8 ns |   3.31 |    0.01 |    4 |         - |          NA |
| ShellSortTokuda1992    | 256  | Sorted             |   1,306.5 ns |      2.62 ns |      1.16 ns |   1,305.9 ns |   4.04 |    0.00 |    5 |         - |          NA |
| ShellSortCiura2001     | 256  | Sorted             |   1,464.0 ns |    277.99 ns |    145.39 ns |   1,522.6 ns |   4.52 |    0.42 |    5 |         - |          NA |
| ShellSortLee2021       | 256  | Sorted             |   1,309.2 ns |      2.74 ns |      0.98 ns |   1,308.9 ns |   4.05 |    0.00 |    5 |         - |          NA |
|      |                    |              |              |              |              |        |         |      |           |             |
| **InsertionSort**          | **256**  | **Reversed**           |  **16,721.8 ns** |    **338.66 ns** |    **120.77 ns** |  **16,675.8 ns** |   **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Reversed           |  18,783.2 ns |    250.32 ns |    130.92 ns |  18,751.5 ns |   1.12 |    0.01 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | Reversed           |  17,015.7 ns |    550.86 ns |    288.11 ns |  16,984.3 ns |   1.02 |    0.02 |    2 |         - |          NA |
| GnomeSort              | 256  | Reversed           |  57,958.3 ns |    202.70 ns |     90.00 ns |  57,941.7 ns |   3.47 |    0.02 |    5 |         - |          NA |
| LibrarySort            | 256  | Reversed           |  39,452.6 ns |    647.37 ns |    287.43 ns |  39,523.9 ns |   2.36 |    0.02 |    4 |         - |          NA |
| MergeInsertionSort     | 256  | Reversed           |  24,624.2 ns |    751.09 ns |    392.83 ns |  24,625.8 ns |   1.47 |    0.02 |    3 |         - |          NA |
| ShellSortKnuth1973     | 256  | Reversed           |   1,776.3 ns |     41.55 ns |     18.45 ns |   1,765.6 ns |   0.11 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Reversed           |   1,777.9 ns |     13.33 ns |      6.97 ns |   1,776.2 ns |   0.11 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | Reversed           |   2,853.0 ns |  2,355.15 ns |  1,231.79 ns |   2,072.4 ns |   0.17 |    0.07 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | Reversed           |   1,809.8 ns |     22.10 ns |     11.56 ns |   1,812.3 ns |   0.11 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Reversed           |   2,071.5 ns |    169.31 ns |     75.17 ns |   2,086.6 ns |   0.12 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |              |              |        |         |      |           |             |
| **InsertionSort**          | **256**  | **PipeOrgan**          |   **8,636.1 ns** |    **526.21 ns** |    **233.64 ns** |   **8,547.5 ns** |   **1.00** |    **0.04** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 256  | PipeOrgan          |   9,812.4 ns |    461.91 ns |    241.59 ns |   9,927.2 ns |   1.14 |    0.04 |    3 |         - |          NA |
| BinaryInsertSort       | 256  | PipeOrgan          |  10,124.5 ns |    432.54 ns |    226.23 ns |  10,259.6 ns |   1.17 |    0.04 |    3 |         - |          NA |
| GnomeSort              | 256  | PipeOrgan          |  23,740.9 ns |    351.38 ns |    183.78 ns |  23,747.0 ns |   2.75 |    0.07 |    4 |         - |          NA |
| LibrarySort            | 256  | PipeOrgan          |  24,177.0 ns |    530.06 ns |    277.23 ns |  24,141.3 ns |   2.80 |    0.08 |    4 |         - |          NA |
| MergeInsertionSort     | 256  | PipeOrgan          |  22,362.7 ns |    981.24 ns |    513.21 ns |  22,150.7 ns |   2.59 |    0.09 |    4 |         - |          NA |
| ShellSortKnuth1973     | 256  | PipeOrgan          |   1,554.7 ns |     19.91 ns |      7.10 ns |   1,555.1 ns |   0.18 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | PipeOrgan          |   1,525.9 ns |     16.82 ns |      7.47 ns |   1,524.5 ns |   0.18 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | PipeOrgan          |   2,817.5 ns |  1,681.50 ns |    879.46 ns |   3,393.0 ns |   0.33 |    0.10 |    2 |         - |          NA |
| ShellSortCiura2001     | 256  | PipeOrgan          |   1,710.7 ns |    268.63 ns |    119.27 ns |   1,657.3 ns |   0.20 |    0.01 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | PipeOrgan          |   1,770.7 ns |     13.11 ns |      5.82 ns |   1,768.2 ns |   0.21 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |              |              |        |         |      |           |             |
| **InsertionSort**          | **1024** | **Random**             | **128,615.8 ns** |  **6,960.09 ns** |  **3,090.32 ns** | **126,851.0 ns** |   **1.00** |    **0.03** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Random             | 141,196.8 ns |  7,650.16 ns |  3,396.72 ns | 143,156.6 ns |   1.10 |    0.03 |    3 |         - |          NA |
| BinaryInsertSort       | 1024 | Random             | 147,345.8 ns |    386.19 ns |    171.47 ns | 147,343.8 ns |   1.15 |    0.03 |    3 |         - |          NA |
| GnomeSort              | 1024 | Random             | 425,313.6 ns |  2,665.99 ns |    950.72 ns | 425,119.2 ns |   3.31 |    0.07 |    4 |         - |          NA |
| LibrarySort            | 1024 | Random             |  84,908.0 ns |    461.04 ns |    164.41 ns |  84,960.8 ns |   0.66 |    0.01 |    2 |         - |          NA |
| MergeInsertionSort     | 1024 | Random             | 394,427.7 ns | 27,202.26 ns | 14,227.31 ns | 386,632.0 ns |   3.07 |    0.12 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Random             |  16,873.2 ns |    484.24 ns |    253.27 ns |  16,875.1 ns |   0.13 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Random             |  17,108.1 ns |    623.83 ns |    326.28 ns |  17,008.6 ns |   0.13 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Random             |  17,463.9 ns |    638.15 ns |    283.34 ns |  17,307.3 ns |   0.14 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Random             |  16,911.3 ns |    519.65 ns |    271.79 ns |  16,910.3 ns |   0.13 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Random             |  17,113.3 ns |    621.69 ns |    325.16 ns |  16,922.0 ns |   0.13 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |              |              |        |         |      |           |             |
| **InsertionSort**          | **1024** | **SingleElementMoved** |   **1,629.9 ns** |      **5.98 ns** |      **2.65 ns** |   **1,629.3 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | SingleElementMoved |   1,083.1 ns |     30.63 ns |     10.92 ns |   1,078.0 ns |   0.66 |    0.01 |    1 |         - |          NA |
| BinaryInsertSort       | 1024 | SingleElementMoved |   4,887.6 ns |    483.37 ns |    252.81 ns |   4,708.0 ns |   3.00 |    0.15 |    3 |         - |          NA |
| GnomeSort              | 1024 | SingleElementMoved |   1,808.2 ns |      4.75 ns |      2.48 ns |   1,808.7 ns |   1.11 |    0.00 |    2 |         - |          NA |
| LibrarySort            | 1024 | SingleElementMoved |  37,199.1 ns |    327.16 ns |    171.11 ns |  37,174.7 ns |  22.82 |    0.10 |    5 |         - |          NA |
| MergeInsertionSort     | 1024 | SingleElementMoved | 261,538.0 ns |  8,354.19 ns |  3,709.31 ns | 261,741.9 ns | 160.46 |    2.14 |    6 |         - |          NA |
| ShellSortKnuth1973     | 1024 | SingleElementMoved |   6,604.9 ns |    356.58 ns |    186.50 ns |   6,600.2 ns |   4.05 |    0.11 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | SingleElementMoved |   6,962.4 ns |     59.83 ns |     21.33 ns |   6,969.2 ns |   4.27 |    0.01 |    4 |         - |          NA |
| ShellSortTokuda1992    | 1024 | SingleElementMoved |   7,398.5 ns |    309.98 ns |    162.13 ns |   7,292.0 ns |   4.54 |    0.09 |    4 |         - |          NA |
| ShellSortCiura2001     | 1024 | SingleElementMoved |   7,266.5 ns |    242.69 ns |    126.93 ns |   7,348.1 ns |   4.46 |    0.07 |    4 |         - |          NA |
| ShellSortLee2021       | 1024 | SingleElementMoved |   7,693.5 ns |    338.21 ns |    176.89 ns |   7,743.9 ns |   4.72 |    0.10 |    4 |         - |          NA |
|      |                    |              |              |              |              |        |         |      |           |             |
| **InsertionSort**          | **1024** | **Sorted**             |   **1,280.7 ns** |     **65.63 ns** |     **23.41 ns** |   **1,271.4 ns** |   **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Sorted             |     804.3 ns |      1.29 ns |      0.57 ns |     804.0 ns |   0.63 |    0.01 |    2 |         - |          NA |
| BinaryInsertSort       | 1024 | Sorted             |     647.6 ns |      2.06 ns |      1.08 ns |     647.3 ns |   0.51 |    0.01 |    1 |         - |          NA |
| GnomeSort              | 1024 | Sorted             |   1,616.7 ns |    542.52 ns |    283.75 ns |   1,511.0 ns |   1.26 |    0.21 |    3 |         - |          NA |
| LibrarySort            | 1024 | Sorted             |  31,202.5 ns |    166.22 ns |     73.80 ns |  31,199.1 ns |  24.37 |    0.40 |    5 |         - |          NA |
| MergeInsertionSort     | 1024 | Sorted             | 259,092.5 ns |  6,365.97 ns |  3,329.52 ns | 258,119.9 ns | 202.37 |    4.13 |    6 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Sorted             |   5,238.8 ns |      8.99 ns |      3.21 ns |   5,237.3 ns |   4.09 |    0.07 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Sorted             |   5,873.3 ns |      4.78 ns |      2.12 ns |   5,872.8 ns |   4.59 |    0.08 |    4 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Sorted             |   6,308.2 ns |      5.64 ns |      2.95 ns |   6,308.5 ns |   4.93 |    0.08 |    4 |         - |          NA |
| ShellSortCiura2001     | 1024 | Sorted             |   6,203.5 ns |      2.51 ns |      1.31 ns |   6,203.5 ns |   4.85 |    0.08 |    4 |         - |          NA |
| ShellSortLee2021       | 1024 | Sorted             |   7,004.4 ns |    272.99 ns |    142.78 ns |   6,993.1 ns |   5.47 |    0.14 |    4 |         - |          NA |
|      |                    |              |              |              |              |        |         |      |           |             |
| **InsertionSort**          | **1024** | **Reversed**           | **250,843.3 ns** |    **775.31 ns** |    **405.50 ns** | **250,963.3 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Reversed           | 280,309.7 ns |  1,563.68 ns |    817.84 ns | 280,126.2 ns |   1.12 |    0.00 |    2 |         - |          NA |
| BinaryInsertSort       | 1024 | Reversed           | 235,139.0 ns |  1,459.29 ns |    763.24 ns | 235,099.3 ns |   0.94 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | Reversed           | 869,806.6 ns | 19,297.36 ns | 10,092.89 ns | 872,153.2 ns |   3.47 |    0.04 |    4 |         - |          NA |
| LibrarySort            | 1024 | Reversed           | 417,266.1 ns |  1,838.78 ns |    816.43 ns | 417,537.0 ns |   1.66 |    0.00 |    3 |         - |          NA |
| MergeInsertionSort     | 1024 | Reversed           | 268,963.8 ns | 12,866.88 ns |  6,729.63 ns | 270,341.0 ns |   1.07 |    0.03 |    2 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Reversed           |   8,543.6 ns |    334.53 ns |    148.53 ns |   8,587.8 ns |   0.03 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Reversed           |   8,726.2 ns |     78.39 ns |     27.96 ns |   8,716.6 ns |   0.03 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Reversed           |  10,097.6 ns |    545.64 ns |    285.38 ns |  10,255.1 ns |   0.04 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Reversed           |   9,557.1 ns |    414.20 ns |    216.63 ns |   9,515.1 ns |   0.04 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Reversed           |   9,940.3 ns |    699.12 ns |    310.41 ns |   9,771.8 ns |   0.04 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |              |              |        |         |      |           |             |
| **InsertionSort**          | **1024** | **PipeOrgan**          | **129,229.2 ns** |  **6,862.20 ns** |  **3,046.86 ns** | **127,522.0 ns** |   **1.00** |    **0.03** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | PipeOrgan          | 143,781.2 ns |  4,362.78 ns |  2,281.82 ns | 144,131.7 ns |   1.11 |    0.03 |    2 |         - |          NA |
| BinaryInsertSort       | 1024 | PipeOrgan          | 131,116.6 ns |    875.03 ns |    457.66 ns | 131,026.3 ns |   1.02 |    0.02 |    2 |         - |          NA |
| GnomeSort              | 1024 | PipeOrgan          | 340,168.5 ns |    873.78 ns |    387.96 ns | 340,172.0 ns |   2.63 |    0.06 |    5 |         - |          NA |
| LibrarySort            | 1024 | PipeOrgan          | 274,891.5 ns |  4,798.96 ns |  2,509.95 ns | 273,763.7 ns |   2.13 |    0.05 |    4 |         - |          NA |
| MergeInsertionSort     | 1024 | PipeOrgan          | 220,118.5 ns |  8,160.23 ns |  4,267.96 ns | 220,266.0 ns |   1.70 |    0.05 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | PipeOrgan          |   8,189.7 ns |    493.17 ns |    257.94 ns |   8,060.8 ns |   0.06 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | PipeOrgan          |   8,690.0 ns |    528.79 ns |    276.57 ns |   8,760.4 ns |   0.07 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | PipeOrgan          |   9,145.2 ns |    423.51 ns |    221.51 ns |   9,111.1 ns |   0.07 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | PipeOrgan          |   9,057.1 ns |    335.84 ns |    149.11 ns |   9,065.5 ns |   0.07 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | PipeOrgan          |   9,284.8 ns |    574.22 ns |    300.33 ns |   9,208.9 ns |   0.07 |    0.00 |    1 |         - |          NA |

### IntKeyBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method             | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **QuickSort**          | **256**  | **Random**             |     **3,002.1 ns** |    **111.98 ns** |     **49.72 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| QuickSort3way      | 256  | Random             |     3,200.3 ns |    239.69 ns |    106.42 ns |  1.07 |    0.04 |    1 |         - |          NA |
| QuickSortMedian3   | 256  | Random             |     4,523.4 ns |    505.66 ns |    264.47 ns |  1.51 |    0.09 |    1 |         - |          NA |
| QuickSortMedian9   | 256  | Random             |     3,940.8 ns |    478.79 ns |    250.42 ns |  1.31 |    0.08 |    1 |         - |          NA |
| DualPivotQuickSort | 256  | Random             |     2,598.8 ns |     55.87 ns |     24.81 ns |  0.87 |    0.02 |    1 |         - |          NA |
| StableQuickSort    | 256  | Random             |    11,384.6 ns |    537.98 ns |    281.37 ns |  3.79 |    0.11 |    2 |         - |          NA |
| IntroSort          | 256  | Random             |     2,181.6 ns |     90.19 ns |     32.16 ns |  0.73 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | Random             |     1,880.7 ns |     26.55 ns |     11.79 ns |  0.63 |    0.01 |    1 |         - |          NA |
| PDQSort            | 256  | Random             |     1,897.6 ns |     81.81 ns |     36.33 ns |  0.63 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | Random             |     3,349.3 ns |    129.71 ns |     57.59 ns |  1.12 |    0.02 |    1 |         - |          NA |
| StdSort            | 256  | Random             |     3,214.7 ns |     57.56 ns |     20.53 ns |  1.07 |    0.02 |    1 |         - |          NA |
| BlockQuickSort     | 256  | Random             |     2,956.2 ns |    394.25 ns |    206.20 ns |  0.98 |    0.07 |    1 |         - |          NA |
| DotnetSort         | 256  | Random             |     2,052.7 ns |     53.21 ns |     18.97 ns |  0.68 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **256**  | **SingleElementMoved** |     **1,972.4 ns** |    **439.03 ns** |    **229.62 ns** |  **1.01** |    **0.16** |    **4** |         **-** |          **NA** |
| QuickSort3way      | 256  | SingleElementMoved |     5,177.8 ns |    668.54 ns |    349.66 ns |  2.66 |    0.34 |    6 |         - |          NA |
| QuickSortMedian3   | 256  | SingleElementMoved |     5,282.0 ns |    578.39 ns |    302.51 ns |  2.71 |    0.34 |    6 |         - |          NA |
| QuickSortMedian9   | 256  | SingleElementMoved |     4,251.1 ns |     94.82 ns |     42.10 ns |  2.18 |    0.25 |    6 |         - |          NA |
| DualPivotQuickSort | 256  | SingleElementMoved |     4,163.8 ns |    382.51 ns |    200.06 ns |  2.14 |    0.26 |    6 |         - |          NA |
| StableQuickSort    | 256  | SingleElementMoved |     8,748.0 ns |    342.70 ns |    179.24 ns |  4.49 |    0.51 |    7 |         - |          NA |
| IntroSort          | 256  | SingleElementMoved |       919.1 ns |     19.02 ns |      6.78 ns |  0.47 |    0.05 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | SingleElementMoved |     1,121.0 ns |     24.20 ns |      8.63 ns |  0.58 |    0.07 |    2 |         - |          NA |
| PDQSort            | 256  | SingleElementMoved |     1,164.4 ns |     21.91 ns |     11.46 ns |  0.60 |    0.07 |    2 |         - |          NA |
| PDQSortBranchless  | 256  | SingleElementMoved |     1,452.0 ns |     24.77 ns |      8.83 ns |  0.75 |    0.08 |    3 |         - |          NA |
| StdSort            | 256  | SingleElementMoved |     2,738.2 ns |     59.70 ns |     21.29 ns |  1.41 |    0.16 |    5 |         - |          NA |
| BlockQuickSort     | 256  | SingleElementMoved |     1,498.7 ns |     30.41 ns |     13.50 ns |  0.77 |    0.09 |    3 |         - |          NA |
| DotnetSort         | 256  | SingleElementMoved |     1,131.1 ns |     26.39 ns |     11.72 ns |  0.58 |    0.07 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **256**  | **Sorted**             |     **1,125.9 ns** |     **29.14 ns** |     **12.94 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| QuickSort3way      | 256  | Sorted             |     6,644.7 ns |     90.81 ns |     47.50 ns |  5.90 |    0.07 |    5 |         - |          NA |
| QuickSortMedian3   | 256  | Sorted             |     6,256.6 ns |     11.44 ns |      5.98 ns |  5.56 |    0.06 |    5 |         - |          NA |
| QuickSortMedian9   | 256  | Sorted             |     4,753.1 ns |    489.92 ns |    256.24 ns |  4.22 |    0.22 |    4 |         - |          NA |
| DualPivotQuickSort | 256  | Sorted             |     4,779.3 ns |    516.72 ns |    270.25 ns |  4.25 |    0.23 |    4 |         - |          NA |
| StableQuickSort    | 256  | Sorted             |     8,793.7 ns |    483.58 ns |    252.92 ns |  7.81 |    0.23 |    6 |         - |          NA |
| IntroSort          | 256  | Sorted             |       312.0 ns |     13.96 ns |      6.20 ns |  0.28 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | Sorted             |     1,036.1 ns |      4.81 ns |      2.13 ns |  0.92 |    0.01 |    3 |         - |          NA |
| PDQSort            | 256  | Sorted             |       301.3 ns |      8.37 ns |      3.71 ns |  0.27 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | Sorted             |       303.1 ns |      3.38 ns |      1.50 ns |  0.27 |    0.00 |    1 |         - |          NA |
| StdSort            | 256  | Sorted             |       708.0 ns |      1.51 ns |      0.67 ns |  0.63 |    0.01 |    2 |         - |          NA |
| BlockQuickSort     | 256  | Sorted             |     1,273.5 ns |      6.22 ns |      2.76 ns |  1.13 |    0.01 |    3 |         - |          NA |
| DotnetSort         | 256  | Sorted             |       933.0 ns |     13.32 ns |      5.92 ns |  0.83 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **256**  | **Reversed**           |       **992.0 ns** |    **117.28 ns** |     **52.07 ns** |  **1.00** |    **0.07** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 256  | Reversed           |     5,363.9 ns |    336.92 ns |    176.22 ns |  5.42 |    0.30 |    4 |         - |          NA |
| QuickSortMedian3   | 256  | Reversed           |     7,401.4 ns |    555.00 ns |    246.42 ns |  7.48 |    0.41 |    5 |         - |          NA |
| QuickSortMedian9   | 256  | Reversed           |     5,075.0 ns |    350.90 ns |    183.53 ns |  5.13 |    0.29 |    4 |         - |          NA |
| DualPivotQuickSort | 256  | Reversed           |     4,755.7 ns |    485.00 ns |    253.66 ns |  4.80 |    0.33 |    4 |         - |          NA |
| StableQuickSort    | 256  | Reversed           |     9,273.6 ns |    545.31 ns |    242.12 ns |  9.37 |    0.49 |    6 |         - |          NA |
| IntroSort          | 256  | Reversed           |       637.1 ns |      7.07 ns |      3.70 ns |  0.64 |    0.03 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | Reversed           |     1,590.0 ns |     14.58 ns |      7.63 ns |  1.61 |    0.07 |    3 |         - |          NA |
| PDQSort            | 256  | Reversed           |       534.5 ns |      4.92 ns |      2.18 ns |  0.54 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | Reversed           |       925.1 ns |     15.89 ns |      5.67 ns |  0.93 |    0.04 |    2 |         - |          NA |
| StdSort            | 256  | Reversed           |       931.3 ns |     12.69 ns |      6.64 ns |  0.94 |    0.04 |    2 |         - |          NA |
| BlockQuickSort     | 256  | Reversed           |     1,727.1 ns |    335.21 ns |    175.32 ns |  1.74 |    0.19 |    3 |         - |          NA |
| DotnetSort         | 256  | Reversed           |     1,416.0 ns |     29.33 ns |     13.02 ns |  1.43 |    0.07 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **256**  | **PipeOrgan**          |     **7,690.7 ns** |     **46.13 ns** |     **20.48 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way      | 256  | PipeOrgan          |     5,636.8 ns |    448.85 ns |    234.76 ns |  0.73 |    0.03 |    4 |         - |          NA |
| QuickSortMedian3   | 256  | PipeOrgan          |     6,410.6 ns |     33.08 ns |     11.80 ns |  0.83 |    0.00 |    4 |         - |          NA |
| QuickSortMedian9   | 256  | PipeOrgan          |     4,295.9 ns |    399.45 ns |    208.92 ns |  0.56 |    0.03 |    3 |         - |          NA |
| DualPivotQuickSort | 256  | PipeOrgan          |     2,170.3 ns |     31.67 ns |     14.06 ns |  0.28 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 256  | PipeOrgan          |     9,384.3 ns |    505.92 ns |    224.63 ns |  1.22 |    0.03 |    5 |         - |          NA |
| IntroSort          | 256  | PipeOrgan          |     2,134.1 ns |    477.99 ns |    250.00 ns |  0.28 |    0.03 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | PipeOrgan          |     2,484.1 ns |     42.47 ns |     18.86 ns |  0.32 |    0.00 |    1 |         - |          NA |
| PDQSort            | 256  | PipeOrgan          |     1,749.1 ns |     20.40 ns |      9.06 ns |  0.23 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | PipeOrgan          |     3,189.3 ns |     64.52 ns |     28.65 ns |  0.41 |    0.00 |    2 |         - |          NA |
| StdSort            | 256  | PipeOrgan          |     3,886.6 ns |     71.15 ns |     25.37 ns |  0.51 |    0.00 |    3 |         - |          NA |
| BlockQuickSort     | 256  | PipeOrgan          |     4,537.1 ns |    287.58 ns |    127.69 ns |  0.59 |    0.02 |    3 |         - |          NA |
| DotnetSort         | 256  | PipeOrgan          |     2,528.8 ns |    323.93 ns |    169.42 ns |  0.33 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **1024** | **Random**             |    **15,398.5 ns** |    **677.56 ns** |    **354.38 ns** |  **1.00** |    **0.03** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 1024 | Random             |    18,663.0 ns |  1,390.25 ns |    727.13 ns |  1.21 |    0.05 |    2 |         - |          NA |
| QuickSortMedian3   | 1024 | Random             |    26,416.1 ns |  4,200.56 ns |  2,196.97 ns |  1.72 |    0.14 |    3 |         - |          NA |
| QuickSortMedian9   | 1024 | Random             |    20,834.4 ns |  3,923.10 ns |  2,051.86 ns |  1.35 |    0.13 |    2 |         - |          NA |
| DualPivotQuickSort | 1024 | Random             |    12,618.2 ns |    279.10 ns |    123.92 ns |  0.82 |    0.02 |    1 |         - |          NA |
| StableQuickSort    | 1024 | Random             |    83,910.9 ns |    701.24 ns |    311.36 ns |  5.45 |    0.12 |    4 |         - |          NA |
| IntroSort          | 1024 | Random             |    11,912.7 ns |    191.45 ns |    100.13 ns |  0.77 |    0.02 |    1 |         - |          NA |
| IntroSortDotnet    | 1024 | Random             |    10,080.7 ns |    477.16 ns |    249.56 ns |  0.65 |    0.02 |    1 |         - |          NA |
| PDQSort            | 1024 | Random             |     9,731.5 ns |    486.60 ns |    254.50 ns |  0.63 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless  | 1024 | Random             |    16,475.2 ns |    300.39 ns |    157.11 ns |  1.07 |    0.03 |    2 |         - |          NA |
| StdSort            | 1024 | Random             |    15,524.6 ns |    318.52 ns |    141.43 ns |  1.01 |    0.02 |    2 |         - |          NA |
| BlockQuickSort     | 1024 | Random             |    16,414.7 ns |    363.28 ns |    190.00 ns |  1.07 |    0.03 |    2 |         - |          NA |
| DotnetSort         | 1024 | Random             |    11,493.3 ns |    638.52 ns |    333.96 ns |  0.75 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **1024** | **SingleElementMoved** |     **7,270.9 ns** |     **91.77 ns** |     **32.73 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 1024 | SingleElementMoved |    35,191.5 ns |    596.38 ns |    311.92 ns |  4.84 |    0.05 |    6 |         - |          NA |
| QuickSortMedian3   | 1024 | SingleElementMoved |    32,039.8 ns |    902.43 ns |    471.99 ns |  4.41 |    0.06 |    6 |         - |          NA |
| QuickSortMedian9   | 1024 | SingleElementMoved |    22,461.2 ns |    711.31 ns |    315.82 ns |  3.09 |    0.04 |    5 |         - |          NA |
| DualPivotQuickSort | 1024 | SingleElementMoved |    23,138.4 ns |    273.93 ns |    121.63 ns |  3.18 |    0.02 |    5 |         - |          NA |
| StableQuickSort    | 1024 | SingleElementMoved |    42,901.8 ns |    816.69 ns |    427.14 ns |  5.90 |    0.06 |    7 |         - |          NA |
| IntroSort          | 1024 | SingleElementMoved |     4,593.9 ns |    565.99 ns |    296.03 ns |  0.63 |    0.04 |    1 |         - |          NA |
| IntroSortDotnet    | 1024 | SingleElementMoved |     6,682.0 ns |    109.50 ns |     57.27 ns |  0.92 |    0.01 |    2 |         - |          NA |
| PDQSort            | 1024 | SingleElementMoved |     5,087.6 ns |     35.02 ns |     12.49 ns |  0.70 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 1024 | SingleElementMoved |     6,235.3 ns |     45.55 ns |     20.23 ns |  0.86 |    0.00 |    2 |         - |          NA |
| StdSort            | 1024 | SingleElementMoved |    11,768.8 ns |    516.22 ns |    269.99 ns |  1.62 |    0.04 |    4 |         - |          NA |
| BlockQuickSort     | 1024 | SingleElementMoved |     8,926.6 ns |    471.00 ns |    246.34 ns |  1.23 |    0.03 |    3 |         - |          NA |
| DotnetSort         | 1024 | SingleElementMoved |     6,383.0 ns |     66.77 ns |     34.92 ns |  0.88 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **1024** | **Sorted**             |     **5,674.0 ns** |    **768.25 ns** |    **401.81 ns** |  **1.00** |    **0.09** |    **4** |         **-** |          **NA** |
| QuickSort3way      | 1024 | Sorted             |    46,833.0 ns |    398.19 ns |    176.80 ns |  8.29 |    0.52 |    7 |         - |          NA |
| QuickSortMedian3   | 1024 | Sorted             |    43,546.5 ns |    425.74 ns |    151.82 ns |  7.71 |    0.48 |    7 |         - |          NA |
| QuickSortMedian9   | 1024 | Sorted             |    23,161.9 ns |  1,795.94 ns |    939.31 ns |  4.10 |    0.30 |    6 |         - |          NA |
| DualPivotQuickSort | 1024 | Sorted             |    24,775.9 ns |    382.25 ns |    199.92 ns |  4.38 |    0.27 |    6 |         - |          NA |
| StableQuickSort    | 1024 | Sorted             |    42,587.3 ns |    452.95 ns |    236.90 ns |  7.54 |    0.47 |    7 |         - |          NA |
| IntroSort          | 1024 | Sorted             |     1,111.6 ns |      3.47 ns |      1.24 ns |  0.20 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet    | 1024 | Sorted             |     5,061.7 ns |    418.02 ns |    218.63 ns |  0.90 |    0.07 |    4 |         - |          NA |
| PDQSort            | 1024 | Sorted             |     1,016.9 ns |      6.03 ns |      2.68 ns |  0.18 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless  | 1024 | Sorted             |     1,205.4 ns |    395.36 ns |    175.54 ns |  0.21 |    0.03 |    2 |         - |          NA |
| StdSort            | 1024 | Sorted             |     2,782.0 ns |    230.90 ns |    102.52 ns |  0.49 |    0.03 |    3 |         - |          NA |
| BlockQuickSort     | 1024 | Sorted             |     7,533.4 ns |    544.56 ns |    284.81 ns |  1.33 |    0.10 |    5 |         - |          NA |
| DotnetSort         | 1024 | Sorted             |     4,814.9 ns |    467.71 ns |    244.62 ns |  0.85 |    0.07 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **1024** | **Reversed**           |     **4,576.8 ns** |    **167.76 ns** |     **59.82 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| QuickSort3way      | 1024 | Reversed           |    38,815.3 ns |    837.39 ns |    371.81 ns |  8.48 |    0.13 |    6 |         - |          NA |
| QuickSortMedian3   | 1024 | Reversed           |    52,599.9 ns |  2,256.69 ns |  1,001.99 ns | 11.49 |    0.25 |    6 |         - |          NA |
| QuickSortMedian9   | 1024 | Reversed           |    22,899.6 ns |    319.80 ns |    167.26 ns |  5.00 |    0.07 |    5 |         - |          NA |
| DualPivotQuickSort | 1024 | Reversed           |    24,648.8 ns |    607.95 ns |    269.93 ns |  5.39 |    0.09 |    5 |         - |          NA |
| StableQuickSort    | 1024 | Reversed           |    45,318.2 ns |    590.54 ns |    308.87 ns |  9.90 |    0.14 |    6 |         - |          NA |
| IntroSort          | 1024 | Reversed           |     4,099.7 ns |    389.35 ns |    203.64 ns |  0.90 |    0.04 |    3 |         - |          NA |
| IntroSortDotnet    | 1024 | Reversed           |     8,041.4 ns |    695.70 ns |    363.87 ns |  1.76 |    0.08 |    4 |         - |          NA |
| PDQSort            | 1024 | Reversed           |     2,177.0 ns |    266.96 ns |    118.53 ns |  0.48 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless  | 1024 | Reversed           |     3,320.4 ns |     12.49 ns |      6.53 ns |  0.73 |    0.01 |    2 |         - |          NA |
| StdSort            | 1024 | Reversed           |     3,400.8 ns |     43.80 ns |     15.62 ns |  0.74 |    0.01 |    2 |         - |          NA |
| BlockQuickSort     | 1024 | Reversed           |     7,920.3 ns |    386.86 ns |    202.33 ns |  1.73 |    0.05 |    4 |         - |          NA |
| DotnetSort         | 1024 | Reversed           |     7,790.6 ns |    693.08 ns |    362.49 ns |  1.70 |    0.08 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **1024** | **PipeOrgan**          |    **97,255.9 ns** |    **643.14 ns** |    **285.56 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| QuickSort3way      | 1024 | PipeOrgan          |    38,697.6 ns |  6,171.78 ns |  3,227.96 ns |  0.40 |    0.03 |    4 |         - |          NA |
| QuickSortMedian3   | 1024 | PipeOrgan          |    37,873.1 ns |    631.22 ns |    330.14 ns |  0.39 |    0.00 |    4 |         - |          NA |
| QuickSortMedian9   | 1024 | PipeOrgan          |    22,589.9 ns |  1,734.87 ns |    907.37 ns |  0.23 |    0.01 |    3 |         - |          NA |
| DualPivotQuickSort | 1024 | PipeOrgan          |    11,474.1 ns |    489.98 ns |    256.27 ns |  0.12 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 1024 | PipeOrgan          |    45,916.0 ns |    724.30 ns |    378.82 ns |  0.47 |    0.00 |    5 |         - |          NA |
| IntroSort          | 1024 | PipeOrgan          |    15,002.1 ns |    394.10 ns |    206.12 ns |  0.15 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet    | 1024 | PipeOrgan          |    14,871.3 ns |    406.47 ns |    212.59 ns |  0.15 |    0.00 |    2 |         - |          NA |
| PDQSort            | 1024 | PipeOrgan          |     9,607.0 ns |    287.61 ns |    150.43 ns |  0.10 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 1024 | PipeOrgan          |    18,426.7 ns |    572.24 ns |    299.29 ns |  0.19 |    0.00 |    3 |         - |          NA |
| StdSort            | 1024 | PipeOrgan          |    20,914.2 ns |    766.97 ns |    340.54 ns |  0.22 |    0.00 |    3 |         - |          NA |
| BlockQuickSort     | 1024 | PipeOrgan          |    24,285.5 ns |    472.55 ns |    247.15 ns |  0.25 |    0.00 |    3 |         - |          NA |
| DotnetSort         | 1024 | PipeOrgan          |    15,088.4 ns |  1,225.44 ns |    640.93 ns |  0.16 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **8192** | **Random**             |   **431,338.4 ns** |  **3,885.41 ns** |  **2,032.15 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| QuickSort3way      | 8192 | Random             |   429,992.2 ns |  8,188.40 ns |  3,635.70 ns |  1.00 |    0.01 |    1 |         - |          NA |
| QuickSortMedian3   | 8192 | Random             |   533,339.9 ns |  1,924.18 ns |    854.35 ns |  1.24 |    0.01 |    1 |         - |          NA |
| QuickSortMedian9   | 8192 | Random             |   513,110.8 ns | 13,690.80 ns |  6,078.80 ns |  1.19 |    0.01 |    1 |         - |          NA |
| DualPivotQuickSort | 8192 | Random             |   365,073.1 ns |  1,308.68 ns |    684.46 ns |  0.85 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 8192 | Random             | 1,156,722.5 ns | 14,174.14 ns |  6,293.41 ns |  2.68 |    0.02 |    2 |         - |          NA |
| IntroSort          | 8192 | Random             |   386,581.7 ns |  4,743.16 ns |  2,480.76 ns |  0.90 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet    | 8192 | Random             |   352,298.1 ns |  2,705.99 ns |  1,415.29 ns |  0.82 |    0.00 |    1 |         - |          NA |
| PDQSort            | 8192 | Random             |   362,429.3 ns |  2,351.82 ns |  1,230.05 ns |  0.84 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | Random             |   465,810.4 ns |    951.86 ns |    497.84 ns |  1.08 |    0.00 |    1 |         - |          NA |
| StdSort            | 8192 | Random             |   403,666.1 ns |  1,291.81 ns |    675.64 ns |  0.94 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 8192 | Random             |   438,929.8 ns |  1,440.89 ns |    639.77 ns |  1.02 |    0.00 |    1 |         - |          NA |
| DotnetSort         | 8192 | Random             |   345,251.5 ns |  1,200.81 ns |    533.17 ns |  0.80 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **8192** | **SingleElementMoved** |    **80,258.7 ns** |  **4,838.68 ns** |  **2,530.72 ns** |  **1.00** |    **0.04** |    **3** |         **-** |          **NA** |
| QuickSort3way      | 8192 | SingleElementMoved |   753,750.3 ns | 10,128.29 ns |  5,297.29 ns |  9.40 |    0.29 |    8 |         - |          NA |
| QuickSortMedian3   | 8192 | SingleElementMoved |   571,264.7 ns |  4,010.59 ns |  2,097.61 ns |  7.12 |    0.22 |    7 |         - |          NA |
| QuickSortMedian9   | 8192 | SingleElementMoved |   214,029.3 ns |  6,931.54 ns |  3,625.33 ns |  2.67 |    0.09 |    5 |         - |          NA |
| DualPivotQuickSort | 8192 | SingleElementMoved |   156,435.3 ns |    936.85 ns |    415.97 ns |  1.95 |    0.06 |    4 |         - |          NA |
| StableQuickSort    | 8192 | SingleElementMoved |   434,684.8 ns |  5,172.25 ns |  2,705.19 ns |  5.42 |    0.17 |    6 |         - |          NA |
| IntroSort          | 8192 | SingleElementMoved |    41,420.2 ns |  2,893.31 ns |  1,284.65 ns |  0.52 |    0.02 |    1 |         - |          NA |
| IntroSortDotnet    | 8192 | SingleElementMoved |    64,111.3 ns |    566.12 ns |    251.36 ns |  0.80 |    0.02 |    3 |         - |          NA |
| PDQSort            | 8192 | SingleElementMoved |    44,394.6 ns |  1,003.99 ns |    525.10 ns |  0.55 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | SingleElementMoved |    53,421.1 ns |    709.13 ns |    314.86 ns |  0.67 |    0.02 |    2 |         - |          NA |
| StdSort            | 8192 | SingleElementMoved |    93,766.0 ns |  1,156.44 ns |    513.47 ns |  1.17 |    0.04 |    3 |         - |          NA |
| BlockQuickSort     | 8192 | SingleElementMoved |    93,244.4 ns |  1,708.51 ns |    758.59 ns |  1.16 |    0.04 |    3 |         - |          NA |
| DotnetSort         | 8192 | SingleElementMoved |    71,400.1 ns |  9,923.07 ns |  5,189.96 ns |  0.89 |    0.07 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **8192** | **Sorted**             |    **63,924.5 ns** |  **4,544.61 ns** |  **2,376.92 ns** |  **1.00** |    **0.05** |    **4** |         **-** |          **NA** |
| QuickSort3way      | 8192 | Sorted             | 1,023,824.7 ns |  5,909.02 ns |  2,623.64 ns | 16.04 |    0.57 |    8 |         - |          NA |
| QuickSortMedian3   | 8192 | Sorted             |   894,789.4 ns | 13,561.10 ns |  6,021.21 ns | 14.01 |    0.50 |    8 |         - |          NA |
| QuickSortMedian9   | 8192 | Sorted             |   210,406.1 ns |  4,098.20 ns |  2,143.44 ns |  3.30 |    0.12 |    6 |         - |          NA |
| DualPivotQuickSort | 8192 | Sorted             |   177,188.1 ns |    999.47 ns |    522.74 ns |  2.78 |    0.10 |    6 |         - |          NA |
| StableQuickSort    | 8192 | Sorted             |   432,722.2 ns |  2,601.64 ns |  1,360.71 ns |  6.78 |    0.24 |    7 |         - |          NA |
| IntroSort          | 8192 | Sorted             |     9,052.1 ns |    382.01 ns |    136.23 ns |  0.14 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet    | 8192 | Sorted             |    48,558.0 ns |    993.26 ns |    354.21 ns |  0.76 |    0.03 |    3 |         - |          NA |
| PDQSort            | 8192 | Sorted             |     8,043.7 ns |    655.76 ns |    291.16 ns |  0.13 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | Sorted             |     8,236.7 ns |  1,677.93 ns |    745.01 ns |  0.13 |    0.01 |    1 |         - |          NA |
| StdSort            | 8192 | Sorted             |    21,451.9 ns |  2,259.35 ns |  1,003.16 ns |  0.34 |    0.02 |    2 |         - |          NA |
| BlockQuickSort     | 8192 | Sorted             |    80,515.1 ns |  1,101.81 ns |    489.21 ns |  1.26 |    0.05 |    5 |         - |          NA |
| DotnetSort         | 8192 | Sorted             |    51,296.0 ns |  5,796.16 ns |  3,031.51 ns |  0.80 |    0.05 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **8192** | **Reversed**           |    **50,873.3 ns** |  **5,306.63 ns** |  **2,775.47 ns** |  **1.00** |    **0.07** |    **4** |         **-** |          **NA** |
| QuickSort3way      | 8192 | Reversed           |   842,671.9 ns |  7,061.77 ns |  3,693.44 ns | 16.61 |    0.90 |    8 |         - |          NA |
| QuickSortMedian3   | 8192 | Reversed           | 1,138,920.8 ns | 41,328.44 ns | 18,350.09 ns | 22.45 |    1.26 |    9 |         - |          NA |
| QuickSortMedian9   | 8192 | Reversed           |   213,721.0 ns |  4,624.35 ns |  2,053.24 ns |  4.21 |    0.23 |    6 |         - |          NA |
| DualPivotQuickSort | 8192 | Reversed           |   181,534.5 ns |  4,392.06 ns |  2,297.13 ns |  3.58 |    0.20 |    6 |         - |          NA |
| StableQuickSort    | 8192 | Reversed           |   466,934.0 ns |  3,167.00 ns |  1,406.17 ns |  9.20 |    0.50 |    7 |         - |          NA |
| IntroSort          | 8192 | Reversed           |    35,025.8 ns |  1,261.98 ns |    560.33 ns |  0.69 |    0.04 |    3 |         - |          NA |
| IntroSortDotnet    | 8192 | Reversed           |    80,601.0 ns |  1,279.20 ns |    669.05 ns |  1.59 |    0.09 |    5 |         - |          NA |
| PDQSort            | 8192 | Reversed           |    14,794.8 ns |    219.64 ns |     97.52 ns |  0.29 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | Reversed           |    25,799.8 ns |    748.73 ns |    332.44 ns |  0.51 |    0.03 |    2 |         - |          NA |
| StdSort            | 8192 | Reversed           |    26,528.3 ns |    799.71 ns |    418.26 ns |  0.52 |    0.03 |    2 |         - |          NA |
| BlockQuickSort     | 8192 | Reversed           |    78,893.2 ns |    808.79 ns |    359.11 ns |  1.56 |    0.08 |    5 |         - |          NA |
| DotnetSort         | 8192 | Reversed           |    79,262.4 ns |  1,155.95 ns |    412.22 ns |  1.56 |    0.08 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **8192** | **PipeOrgan**          | **5,438,565.9 ns** | **87,785.39 ns** | **45,913.46 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| QuickSort3way      | 8192 | PipeOrgan          |   511,151.2 ns |  5,928.34 ns |  2,632.22 ns |  0.09 |    0.00 |    2 |         - |          NA |
| QuickSortMedian3   | 8192 | PipeOrgan          |   508,870.7 ns | 32,106.39 ns | 16,792.26 ns |  0.09 |    0.00 |    2 |         - |          NA |
| QuickSortMedian9   | 8192 | PipeOrgan          |   277,259.4 ns |  3,301.58 ns |  1,726.79 ns |  0.05 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort | 8192 | PipeOrgan          |   149,057.3 ns |  2,100.78 ns |    932.76 ns |  0.03 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 8192 | PipeOrgan          |   470,121.8 ns |  2,811.84 ns |  1,248.48 ns |  0.09 |    0.00 |    2 |         - |          NA |
| IntroSort          | 8192 | PipeOrgan          |   333,312.2 ns |  7,119.59 ns |  3,161.14 ns |  0.06 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet    | 8192 | PipeOrgan          |   374,341.2 ns |  4,316.32 ns |  2,257.52 ns |  0.07 |    0.00 |    2 |         - |          NA |
| PDQSort            | 8192 | PipeOrgan          |   146,116.3 ns |  5,229.82 ns |  2,735.30 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | PipeOrgan          |   277,471.4 ns |  1,866.47 ns |    976.20 ns |  0.05 |    0.00 |    2 |         - |          NA |
| StdSort            | 8192 | PipeOrgan          |   435,980.6 ns |  3,412.93 ns |  1,785.03 ns |  0.08 |    0.00 |    2 |         - |          NA |
| BlockQuickSort     | 8192 | PipeOrgan          |   268,078.5 ns |  1,285.42 ns |    570.74 ns |  0.05 |    0.00 |    2 |         - |          NA |
| DotnetSort         | 8192 | PipeOrgan          |   362,291.7 ns |  6,457.71 ns |  2,867.26 ns |  0.07 |    0.00 |    2 |         - |          NA |

### MergeBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                   | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **MergeSort**                | **256**  | **Random**             |     **8,393.5 ns** |    **562.49 ns** |    **294.19 ns** |  **1.00** |    **0.05** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Random             |     8,147.9 ns |    315.21 ns |    164.86 ns |  0.97 |    0.04 |    3 |         - |          NA |
| BottomupMergeSort        | 256  | Random             |     4,691.3 ns |    271.72 ns |    142.12 ns |  0.56 |    0.02 |    2 |         - |          NA |
| StdStableSort            | 256  | Random             |     3,018.0 ns |     45.37 ns |     20.14 ns |  0.36 |    0.01 |    1 |         - |          NA |
| RotateMergeSort          | 256  | Random             |     9,580.3 ns |    501.82 ns |    262.46 ns |  1.14 |    0.05 |    3 |         - |          NA |
| RotateMergeSortRecursive | 256  | Random             |    11,201.8 ns |    268.92 ns |    119.40 ns |  1.34 |    0.05 |    3 |         - |          NA |
| SymMergeSort             | 256  | Random             |     6,495.9 ns |     36.22 ns |     16.08 ns |  0.77 |    0.03 |    2 |         - |          NA |
| BlockMergeSort           | 256  | Random             |     6,314.9 ns |     94.26 ns |     41.85 ns |  0.75 |    0.03 |    2 |         - |          NA |
| NaturalMergeSort         | 256  | Random             |     5,347.9 ns |    286.49 ns |    149.84 ns |  0.64 |    0.03 |    2 |         - |          NA |
| TimSort                  | 256  | Random             |     3,790.4 ns |     72.61 ns |     32.24 ns |  0.45 |    0.02 |    2 |         - |          NA |
| PowerSort                | 256  | Random             |     2,357.3 ns |     43.65 ns |     19.38 ns |  0.28 |    0.01 |    1 |         - |          NA |
| ShiftSort                | 256  | Random             |     4,031.3 ns |     66.50 ns |     29.53 ns |  0.48 |    0.02 |    2 |         - |          NA |
| SpinSort                 | 256  | Random             |     2,191.1 ns |    320.46 ns |    167.61 ns |  0.26 |    0.02 |    1 |         - |          NA |
| SpinSortVariant          | 256  | Random             |     2,397.4 ns |    174.46 ns |     77.46 ns |  0.29 |    0.01 |    1 |         - |          NA |
| Glidesort                | 256  | Random             |     5,112.3 ns |    437.52 ns |    228.83 ns |  0.61 |    0.03 |    2 |         - |          NA |
| FlatStableSort           | 256  | Random             |     2,678.1 ns |     45.44 ns |     20.17 ns |  0.32 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **SingleElementMoved** |     **4,299.6 ns** |     **62.35 ns** |     **22.23 ns** |  **1.00** |    **0.01** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | SingleElementMoved |     5,184.1 ns |     17.74 ns |      7.88 ns |  1.21 |    0.01 |    8 |         - |          NA |
| BottomupMergeSort        | 256  | SingleElementMoved |     2,622.2 ns |    160.70 ns |     71.35 ns |  0.61 |    0.02 |    6 |         - |          NA |
| StdStableSort            | 256  | SingleElementMoved |     1,919.1 ns |     46.29 ns |     24.21 ns |  0.45 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 256  | SingleElementMoved |       582.4 ns |      4.85 ns |      1.73 ns |  0.14 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 256  | SingleElementMoved |       897.4 ns |    178.86 ns |     93.55 ns |  0.21 |    0.02 |    3 |         - |          NA |
| SymMergeSort             | 256  | SingleElementMoved |       516.4 ns |      2.66 ns |      1.18 ns |  0.12 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 256  | SingleElementMoved |     4,219.1 ns |     11.02 ns |      4.89 ns |  0.98 |    0.00 |    7 |         - |          NA |
| NaturalMergeSort         | 256  | SingleElementMoved |       594.5 ns |      3.56 ns |      1.58 ns |  0.14 |    0.00 |    2 |         - |          NA |
| TimSort                  | 256  | SingleElementMoved |       603.7 ns |    282.85 ns |    125.59 ns |  0.14 |    0.03 |    2 |         - |          NA |
| PowerSort                | 256  | SingleElementMoved |       418.5 ns |      8.17 ns |      4.27 ns |  0.10 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | SingleElementMoved |       569.2 ns |    358.83 ns |    187.67 ns |  0.13 |    0.04 |    2 |         - |          NA |
| SpinSort                 | 256  | SingleElementMoved |       906.8 ns |     20.47 ns |      9.09 ns |  0.21 |    0.00 |    3 |         - |          NA |
| SpinSortVariant          | 256  | SingleElementMoved |       953.4 ns |     15.76 ns |      8.24 ns |  0.22 |    0.00 |    3 |         - |          NA |
| Glidesort                | 256  | SingleElementMoved |     1,254.6 ns |      4.73 ns |      2.10 ns |  0.29 |    0.00 |    4 |         - |          NA |
| FlatStableSort           | 256  | SingleElementMoved |     1,242.5 ns |     12.36 ns |      4.41 ns |  0.29 |    0.00 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **Sorted**             |     **3,855.5 ns** |     **11.31 ns** |      **4.03 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Sorted             |     4,820.3 ns |     15.40 ns |      5.49 ns |  1.25 |    0.00 |    8 |         - |          NA |
| BottomupMergeSort        | 256  | Sorted             |     1,987.7 ns |     60.86 ns |     21.70 ns |  0.52 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 256  | Sorted             |     1,702.2 ns |     22.46 ns |      9.97 ns |  0.44 |    0.00 |    6 |         - |          NA |
| RotateMergeSort          | 256  | Sorted             |       345.1 ns |      1.28 ns |      0.57 ns |  0.09 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 256  | Sorted             |       464.6 ns |      1.62 ns |      0.58 ns |  0.12 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 256  | Sorted             |       340.5 ns |      2.64 ns |      0.94 ns |  0.09 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 256  | Sorted             |     3,531.2 ns |    415.30 ns |    217.21 ns |  0.92 |    0.05 |    7 |         - |          NA |
| NaturalMergeSort         | 256  | Sorted             |       216.9 ns |      5.21 ns |      2.73 ns |  0.06 |    0.00 |    2 |         - |          NA |
| TimSort                  | 256  | Sorted             |       193.5 ns |      4.51 ns |      2.36 ns |  0.05 |    0.00 |    2 |         - |          NA |
| PowerSort                | 256  | Sorted             |       148.9 ns |      1.23 ns |      0.44 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Sorted             |       214.7 ns |      1.74 ns |      0.62 ns |  0.06 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 256  | Sorted             |       132.3 ns |      1.20 ns |      0.53 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 256  | Sorted             |       184.4 ns |      4.23 ns |      1.88 ns |  0.05 |    0.00 |    2 |         - |          NA |
| Glidesort                | 256  | Sorted             |       193.5 ns |     10.73 ns |      5.61 ns |  0.05 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 256  | Sorted             |     1,146.5 ns |     35.83 ns |     18.74 ns |  0.30 |    0.00 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **Reversed**           |     **8,621.9 ns** |    **336.53 ns** |    **176.01 ns** |  **1.00** |    **0.03** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Reversed           |     8,022.6 ns |    386.37 ns |    202.08 ns |  0.93 |    0.03 |    6 |         - |          NA |
| BottomupMergeSort        | 256  | Reversed           |     5,190.9 ns |    438.27 ns |    229.22 ns |  0.60 |    0.03 |    5 |         - |          NA |
| StdStableSort            | 256  | Reversed           |     2,578.1 ns |    406.58 ns |    180.52 ns |  0.30 |    0.02 |    4 |         - |          NA |
| RotateMergeSort          | 256  | Reversed           |     1,772.3 ns |      6.43 ns |      2.29 ns |  0.21 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 256  | Reversed           |     1,861.2 ns |      4.63 ns |      2.05 ns |  0.22 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 256  | Reversed           |     1,966.1 ns |      9.36 ns |      4.16 ns |  0.23 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 256  | Reversed           |     9,312.4 ns |    469.91 ns |    245.77 ns |  1.08 |    0.03 |    6 |         - |          NA |
| NaturalMergeSort         | 256  | Reversed           |       495.6 ns |    256.06 ns |    113.69 ns |  0.06 |    0.01 |    2 |         - |          NA |
| TimSort                  | 256  | Reversed           |       265.1 ns |      5.31 ns |      2.36 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | Reversed           |       229.1 ns |      3.79 ns |      1.68 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Reversed           |       255.7 ns |      5.63 ns |      2.95 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 256  | Reversed           |       256.5 ns |      2.32 ns |      1.21 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 256  | Reversed           |       288.5 ns |      5.91 ns |      2.62 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 256  | Reversed           |       279.5 ns |      5.16 ns |      2.29 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | Reversed           |     2,905.1 ns |     14.52 ns |      6.45 ns |  0.34 |    0.01 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **PipeOrgan**          |     **6,307.7 ns** |     **17.83 ns** |      **7.91 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | PipeOrgan          |     6,735.0 ns |    450.50 ns |    200.02 ns |  1.07 |    0.03 |    7 |         - |          NA |
| BottomupMergeSort        | 256  | PipeOrgan          |     3,897.8 ns |    484.68 ns |    253.50 ns |  0.62 |    0.04 |    6 |         - |          NA |
| StdStableSort            | 256  | PipeOrgan          |     2,273.0 ns |    250.27 ns |     89.25 ns |  0.36 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 256  | PipeOrgan          |     4,217.6 ns |    224.97 ns |     99.89 ns |  0.67 |    0.01 |    6 |         - |          NA |
| RotateMergeSortRecursive | 256  | PipeOrgan          |     4,886.0 ns |     40.81 ns |     14.55 ns |  0.77 |    0.00 |    6 |         - |          NA |
| SymMergeSort             | 256  | PipeOrgan          |     2,521.0 ns |     30.63 ns |     13.60 ns |  0.40 |    0.00 |    5 |         - |          NA |
| BlockMergeSort           | 256  | PipeOrgan          |     6,331.1 ns |      9.56 ns |      4.24 ns |  1.00 |    0.00 |    7 |         - |          NA |
| NaturalMergeSort         | 256  | PipeOrgan          |       712.5 ns |    100.78 ns |     44.75 ns |  0.11 |    0.01 |    2 |         - |          NA |
| TimSort                  | 256  | PipeOrgan          |       841.7 ns |     12.65 ns |      5.62 ns |  0.13 |    0.00 |    3 |         - |          NA |
| PowerSort                | 256  | PipeOrgan          |       505.0 ns |      6.45 ns |      2.86 ns |  0.08 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | PipeOrgan          |       541.0 ns |      9.13 ns |      4.78 ns |  0.09 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 256  | PipeOrgan          |     1,869.7 ns |    201.15 ns |    105.21 ns |  0.30 |    0.02 |    5 |         - |          NA |
| SpinSortVariant          | 256  | PipeOrgan          |     1,878.1 ns |     18.27 ns |      8.11 ns |  0.30 |    0.00 |    5 |         - |          NA |
| Glidesort                | 256  | PipeOrgan          |     1,220.9 ns |     12.65 ns |      5.62 ns |  0.19 |    0.00 |    4 |         - |          NA |
| FlatStableSort           | 256  | PipeOrgan          |     2,060.2 ns |     10.83 ns |      4.81 ns |  0.33 |    0.00 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **Random**             |    **36,268.1 ns** |    **591.50 ns** |    **262.63 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Random             |    35,796.9 ns |    764.55 ns |    339.47 ns |  0.99 |    0.01 |    3 |         - |          NA |
| BottomupMergeSort        | 1024 | Random             |    23,340.5 ns |  1,171.82 ns |    612.89 ns |  0.64 |    0.02 |    2 |         - |          NA |
| StdStableSort            | 1024 | Random             |    15,421.5 ns |    271.70 ns |    142.10 ns |  0.43 |    0.00 |    1 |         - |          NA |
| RotateMergeSort          | 1024 | Random             |    69,626.3 ns |  4,995.52 ns |  2,612.75 ns |  1.92 |    0.07 |    4 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Random             |    66,854.4 ns |    842.94 ns |    440.88 ns |  1.84 |    0.02 |    4 |         - |          NA |
| SymMergeSort             | 1024 | Random             |    41,984.7 ns |  1,303.83 ns |    681.93 ns |  1.16 |    0.02 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | Random             |    32,931.1 ns |    267.18 ns |    118.63 ns |  0.91 |    0.01 |    3 |         - |          NA |
| NaturalMergeSort         | 1024 | Random             |    26,417.0 ns |    868.34 ns |    454.16 ns |  0.73 |    0.01 |    2 |         - |          NA |
| TimSort                  | 1024 | Random             |    20,077.3 ns |  1,390.37 ns |    617.33 ns |  0.55 |    0.02 |    2 |         - |          NA |
| PowerSort                | 1024 | Random             |    13,196.1 ns |    606.81 ns |    317.37 ns |  0.36 |    0.01 |    1 |         - |          NA |
| ShiftSort                | 1024 | Random             |    19,828.9 ns |    466.58 ns |    244.03 ns |  0.55 |    0.01 |    2 |         - |          NA |
| SpinSort                 | 1024 | Random             |    13,476.7 ns |    989.23 ns |    517.38 ns |  0.37 |    0.01 |    1 |         - |          NA |
| SpinSortVariant          | 1024 | Random             |    14,641.3 ns |    930.25 ns |    486.54 ns |  0.40 |    0.01 |    1 |         - |          NA |
| Glidesort                | 1024 | Random             |    24,588.6 ns |    466.45 ns |    243.96 ns |  0.68 |    0.01 |    2 |         - |          NA |
| FlatStableSort           | 1024 | Random             |    14,858.0 ns |    467.11 ns |    207.40 ns |  0.41 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **SingleElementMoved** |    **17,160.1 ns** |    **265.42 ns** |    **138.82 ns** |  **1.00** |    **0.01** |    **9** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | SingleElementMoved |    20,926.3 ns |    154.89 ns |     81.01 ns |  1.22 |    0.01 |    9 |         - |          NA |
| BottomupMergeSort        | 1024 | SingleElementMoved |     7,443.8 ns |    443.66 ns |    232.04 ns |  0.43 |    0.01 |    8 |         - |          NA |
| StdStableSort            | 1024 | SingleElementMoved |     8,492.4 ns |    860.43 ns |    382.04 ns |  0.49 |    0.02 |    8 |         - |          NA |
| RotateMergeSort          | 1024 | SingleElementMoved |     2,054.2 ns |    309.66 ns |    161.96 ns |  0.12 |    0.01 |    3 |         - |          NA |
| RotateMergeSortRecursive | 1024 | SingleElementMoved |     2,446.4 ns |     27.99 ns |      9.98 ns |  0.14 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 1024 | SingleElementMoved |     1,700.2 ns |     12.21 ns |      5.42 ns |  0.10 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | SingleElementMoved |    20,100.5 ns |    401.77 ns |    178.39 ns |  1.17 |    0.01 |    9 |         - |          NA |
| NaturalMergeSort         | 1024 | SingleElementMoved |     2,026.9 ns |      7.97 ns |      3.54 ns |  0.12 |    0.00 |    3 |         - |          NA |
| TimSort                  | 1024 | SingleElementMoved |       827.7 ns |      3.92 ns |      2.05 ns |  0.05 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | SingleElementMoved |     1,394.5 ns |      5.29 ns |      2.35 ns |  0.08 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 1024 | SingleElementMoved |     1,364.3 ns |      5.74 ns |      2.55 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 1024 | SingleElementMoved |     4,378.0 ns |    443.03 ns |    231.71 ns |  0.26 |    0.01 |    6 |         - |          NA |
| SpinSortVariant          | 1024 | SingleElementMoved |     3,425.6 ns |     22.49 ns |      8.02 ns |  0.20 |    0.00 |    5 |         - |          NA |
| Glidesort                | 1024 | SingleElementMoved |     2,597.4 ns |     12.50 ns |      5.55 ns |  0.15 |    0.00 |    4 |         - |          NA |
| FlatStableSort           | 1024 | SingleElementMoved |     5,345.2 ns |     41.43 ns |     14.78 ns |  0.31 |    0.00 |    7 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **Sorted**             |    **15,670.4 ns** |    **232.79 ns** |    **121.75 ns** |  **1.00** |    **0.01** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Sorted             |    19,598.6 ns |    182.02 ns |     95.20 ns |  1.25 |    0.01 |    7 |         - |          NA |
| BottomupMergeSort        | 1024 | Sorted             |     5,847.3 ns |     14.48 ns |      7.57 ns |  0.37 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 1024 | Sorted             |     7,705.5 ns |    521.94 ns |    272.99 ns |  0.49 |    0.02 |    5 |         - |          NA |
| RotateMergeSort          | 1024 | Sorted             |     1,341.8 ns |      2.87 ns |      1.27 ns |  0.09 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Sorted             |     1,858.9 ns |      2.36 ns |      1.05 ns |  0.12 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 1024 | Sorted             |     1,313.7 ns |      5.51 ns |      1.97 ns |  0.08 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 1024 | Sorted             |    14,387.5 ns |    172.14 ns |     76.43 ns |  0.92 |    0.01 |    6 |         - |          NA |
| NaturalMergeSort         | 1024 | Sorted             |       590.7 ns |      8.54 ns |      3.79 ns |  0.04 |    0.00 |    1 |         - |          NA |
| TimSort                  | 1024 | Sorted             |       563.2 ns |      3.50 ns |      1.83 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Sorted             |       526.2 ns |     10.15 ns |      4.51 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Sorted             |       716.4 ns |     16.72 ns |      8.75 ns |  0.05 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 1024 | Sorted             |       462.4 ns |      3.12 ns |      1.39 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 1024 | Sorted             |       658.0 ns |      6.05 ns |      2.69 ns |  0.04 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Sorted             |       504.9 ns |      9.37 ns |      4.16 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Sorted             |     5,037.6 ns |    359.97 ns |    188.27 ns |  0.32 |    0.01 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **Reversed**           |    **35,913.1 ns** |    **297.85 ns** |    **132.25 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Reversed           |    33,501.9 ns |    787.21 ns |    411.73 ns |  0.93 |    0.01 |    4 |         - |          NA |
| BottomupMergeSort        | 1024 | Reversed           |    20,174.9 ns |  1,010.10 ns |    528.30 ns |  0.56 |    0.01 |    3 |         - |          NA |
| StdStableSort            | 1024 | Reversed           |    10,780.8 ns |    833.09 ns |    369.90 ns |  0.30 |    0.01 |    2 |         - |          NA |
| RotateMergeSort          | 1024 | Reversed           |     8,578.7 ns |    312.88 ns |    163.64 ns |  0.24 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Reversed           |     8,976.8 ns |    296.71 ns |    131.74 ns |  0.25 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 1024 | Reversed           |     8,603.6 ns |    479.78 ns |    250.94 ns |  0.24 |    0.01 |    2 |         - |          NA |
| BlockMergeSort           | 1024 | Reversed           |    41,813.4 ns |    231.12 ns |    120.88 ns |  1.16 |    0.01 |    4 |         - |          NA |
| NaturalMergeSort         | 1024 | Reversed           |     1,037.9 ns |      5.38 ns |      2.39 ns |  0.03 |    0.00 |    1 |         - |          NA |
| TimSort                  | 1024 | Reversed           |       846.5 ns |     12.22 ns |      5.43 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Reversed           |       812.0 ns |      7.62 ns |      3.98 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Reversed           |     1,038.6 ns |    199.60 ns |     88.62 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 1024 | Reversed           |       956.6 ns |      3.79 ns |      1.68 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 1024 | Reversed           |     1,058.7 ns |      7.45 ns |      3.31 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Reversed           |       859.7 ns |      6.08 ns |      2.70 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Reversed           |    12,591.7 ns |    422.03 ns |    187.38 ns |  0.35 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **PipeOrgan**          |    **26,206.1 ns** |    **436.58 ns** |    **228.34 ns** |  **1.00** |    **0.01** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | PipeOrgan          |    27,269.2 ns |    453.26 ns |    201.25 ns |  1.04 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 1024 | PipeOrgan          |    14,207.2 ns |    377.98 ns |    167.83 ns |  0.54 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 1024 | PipeOrgan          |     9,759.8 ns |    626.64 ns |    327.74 ns |  0.37 |    0.01 |    3 |         - |          NA |
| RotateMergeSort          | 1024 | PipeOrgan          |    18,052.9 ns |    326.68 ns |    170.86 ns |  0.69 |    0.01 |    5 |         - |          NA |
| RotateMergeSortRecursive | 1024 | PipeOrgan          |    21,529.6 ns |    387.74 ns |    172.16 ns |  0.82 |    0.01 |    5 |         - |          NA |
| SymMergeSort             | 1024 | PipeOrgan          |    11,610.7 ns |    793.30 ns |    414.91 ns |  0.44 |    0.02 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | PipeOrgan          |    32,216.1 ns |    145.02 ns |     51.72 ns |  1.23 |    0.01 |    6 |         - |          NA |
| NaturalMergeSort         | 1024 | PipeOrgan          |     2,374.6 ns |      8.63 ns |      3.08 ns |  0.09 |    0.00 |    1 |         - |          NA |
| TimSort                  | 1024 | PipeOrgan          |     2,558.2 ns |     25.58 ns |      9.12 ns |  0.10 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | PipeOrgan          |     2,002.0 ns |     89.92 ns |     39.92 ns |  0.08 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | PipeOrgan          |     1,939.3 ns |      7.40 ns |      3.28 ns |  0.07 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 1024 | PipeOrgan          |     8,439.4 ns |    657.78 ns |    344.03 ns |  0.32 |    0.01 |    3 |         - |          NA |
| SpinSortVariant          | 1024 | PipeOrgan          |     7,805.8 ns |    557.50 ns |    291.59 ns |  0.30 |    0.01 |    3 |         - |          NA |
| Glidesort                | 1024 | PipeOrgan          |     4,536.9 ns |    434.65 ns |    227.33 ns |  0.17 |    0.01 |    2 |         - |          NA |
| FlatStableSort           | 1024 | PipeOrgan          |     8,976.3 ns |    545.53 ns |    285.32 ns |  0.34 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **Random**             |   **678,984.6 ns** |  **1,635.23 ns** |    **726.05 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Random             |   616,897.0 ns |  3,045.92 ns |  1,593.07 ns |  0.91 |    0.00 |    1 |         - |          NA |
| BottomupMergeSort        | 8192 | Random             |   497,906.4 ns |  5,048.63 ns |  2,241.62 ns |  0.73 |    0.00 |    1 |         - |          NA |
| StdStableSort            | 8192 | Random             |   472,257.5 ns |  1,615.21 ns |    844.79 ns |  0.70 |    0.00 |    1 |         - |          NA |
| RotateMergeSort          | 8192 | Random             | 1,349,282.0 ns |  5,741.38 ns |  2,549.21 ns |  1.99 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Random             | 1,464,971.9 ns |  3,674.46 ns |  1,921.82 ns |  2.16 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 8192 | Random             | 1,019,130.6 ns |  3,728.98 ns |  1,655.69 ns |  1.50 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 8192 | Random             |   705,488.2 ns |  4,343.10 ns |  1,928.36 ns |  1.04 |    0.00 |    1 |         - |          NA |
| NaturalMergeSort         | 8192 | Random             |   622,650.7 ns |  6,300.64 ns |  2,797.52 ns |  0.92 |    0.00 |    1 |         - |          NA |
| TimSort                  | 8192 | Random             |   565,430.9 ns |  2,198.33 ns |    976.07 ns |  0.83 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Random             |   427,102.4 ns |  1,804.80 ns |    801.34 ns |  0.63 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Random             |   563,399.2 ns |  3,218.83 ns |  1,683.51 ns |  0.83 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 8192 | Random             |   370,814.1 ns |  3,054.86 ns |  1,356.38 ns |  0.55 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 8192 | Random             |   370,614.4 ns |  1,341.08 ns |    595.45 ns |  0.55 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Random             |   586,427.3 ns |  3,721.19 ns |  1,946.25 ns |  0.86 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Random             |   397,761.3 ns |  1,731.71 ns |    768.89 ns |  0.59 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **SingleElementMoved** |   **136,064.9 ns** |  **1,941.77 ns** |    **862.16 ns** |  **1.00** |    **0.01** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | SingleElementMoved |   167,679.3 ns |  1,561.53 ns |    693.33 ns |  1.23 |    0.01 |    7 |         - |          NA |
| BottomupMergeSort        | 8192 | SingleElementMoved |    56,401.7 ns |  1,074.57 ns |    562.02 ns |  0.41 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 8192 | SingleElementMoved |   109,741.5 ns |    947.51 ns |    420.70 ns |  0.81 |    0.01 |    6 |         - |          NA |
| RotateMergeSort          | 8192 | SingleElementMoved |    13,987.3 ns |    333.86 ns |    119.06 ns |  0.10 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 8192 | SingleElementMoved |    17,707.6 ns |    564.24 ns |    250.53 ns |  0.13 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 8192 | SingleElementMoved |    12,703.6 ns |    317.46 ns |    113.21 ns |  0.09 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 8192 | SingleElementMoved |   150,060.9 ns |    727.66 ns |    323.08 ns |  1.10 |    0.01 |    7 |         - |          NA |
| NaturalMergeSort         | 8192 | SingleElementMoved |    15,919.6 ns |    181.90 ns |     80.76 ns |  0.12 |    0.00 |    3 |         - |          NA |
| TimSort                  | 8192 | SingleElementMoved |     5,530.6 ns |     72.42 ns |     25.82 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | SingleElementMoved |    10,375.4 ns |    425.61 ns |    222.60 ns |  0.08 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 8192 | SingleElementMoved |    10,559.4 ns |    190.94 ns |     68.09 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | SingleElementMoved |    24,174.1 ns |  2,825.10 ns |  1,477.58 ns |  0.18 |    0.01 |    3 |         - |          NA |
| SpinSortVariant          | 8192 | SingleElementMoved |    19,642.4 ns |    726.74 ns |    380.10 ns |  0.14 |    0.00 |    3 |         - |          NA |
| Glidesort                | 8192 | SingleElementMoved |    20,142.7 ns |    429.75 ns |    190.81 ns |  0.15 |    0.00 |    3 |         - |          NA |
| FlatStableSort           | 8192 | SingleElementMoved |    46,071.9 ns |    812.65 ns |    360.82 ns |  0.34 |    0.00 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **Sorted**             |   **124,594.4 ns** |    **807.74 ns** |    **422.46 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Sorted             |   158,660.0 ns |    895.86 ns |    397.77 ns |  1.27 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 8192 | Sorted             |    46,974.0 ns |  1,282.29 ns |    670.67 ns |  0.38 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 8192 | Sorted             |   107,299.2 ns |  1,662.36 ns |    869.45 ns |  0.86 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 8192 | Sorted             |    11,160.4 ns |    263.36 ns |    116.93 ns |  0.09 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Sorted             |    15,634.8 ns |    101.12 ns |     44.90 ns |  0.13 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 8192 | Sorted             |    11,283.0 ns |  1,042.13 ns |    545.05 ns |  0.09 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 8192 | Sorted             |   114,791.9 ns | 12,239.52 ns |  5,434.42 ns |  0.92 |    0.04 |    5 |         - |          NA |
| NaturalMergeSort         | 8192 | Sorted             |     4,488.4 ns |    399.81 ns |    209.11 ns |  0.04 |    0.00 |    1 |         - |          NA |
| TimSort                  | 8192 | Sorted             |     4,387.7 ns |    822.74 ns |    430.31 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Sorted             |     4,140.1 ns |    418.14 ns |    185.66 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Sorted             |     5,538.7 ns |    573.83 ns |    254.78 ns |  0.04 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 8192 | Sorted             |     3,617.2 ns |    230.63 ns |    120.62 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 8192 | Sorted             |     5,369.4 ns |    650.44 ns |    231.95 ns |  0.04 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Sorted             |     3,576.1 ns |    109.46 ns |     48.60 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Sorted             |     4,303.0 ns |  1,593.88 ns |    707.69 ns |  0.03 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **Reversed**           |   **364,480.1 ns** | **12,181.95 ns** |  **4,344.20 ns** |  **1.00** |    **0.02** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Reversed           |   276,581.7 ns |  4,521.80 ns |  2,364.99 ns |  0.76 |    0.01 |    5 |         - |          NA |
| BottomupMergeSort        | 8192 | Reversed           |   171,067.1 ns |  2,638.35 ns |    940.86 ns |  0.47 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 8192 | Reversed           |   131,586.8 ns |  1,567.44 ns |    819.80 ns |  0.36 |    0.00 |    3 |         - |          NA |
| RotateMergeSort          | 8192 | Reversed           |    83,926.6 ns |  1,170.10 ns |    611.99 ns |  0.23 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Reversed           |    88,241.0 ns |  1,223.96 ns |    543.44 ns |  0.24 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 8192 | Reversed           |    77,006.0 ns |  3,166.46 ns |  1,656.12 ns |  0.21 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 8192 | Reversed           |   345,062.8 ns |  3,553.11 ns |  1,577.60 ns |  0.95 |    0.01 |    6 |         - |          NA |
| NaturalMergeSort         | 8192 | Reversed           |     7,824.4 ns |    482.75 ns |    172.16 ns |  0.02 |    0.00 |    1 |         - |          NA |
| TimSort                  | 8192 | Reversed           |     7,217.1 ns |  1,651.86 ns |    733.44 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Reversed           |     6,557.5 ns |    269.38 ns |    119.61 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Reversed           |     6,636.3 ns |    852.12 ns |    303.87 ns |  0.02 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 8192 | Reversed           |     7,684.9 ns |    450.31 ns |    235.52 ns |  0.02 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 8192 | Reversed           |     8,394.8 ns |    354.86 ns |    157.56 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Reversed           |     6,414.2 ns |    351.77 ns |    183.98 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Reversed           |     7,497.2 ns |    508.06 ns |    225.58 ns |  0.02 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **PipeOrgan**          |   **226,846.5 ns** | **13,546.16 ns** |  **7,084.90 ns** |  **1.00** |    **0.04** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | PipeOrgan          |   227,153.8 ns |  9,155.77 ns |  4,065.22 ns |  1.00 |    0.03 |    6 |         - |          NA |
| BottomupMergeSort        | 8192 | PipeOrgan          |   116,761.9 ns |  2,908.22 ns |  1,291.27 ns |  0.52 |    0.02 |    4 |         - |          NA |
| StdStableSort            | 8192 | PipeOrgan          |   130,396.4 ns |  1,404.05 ns |    734.34 ns |  0.58 |    0.02 |    4 |         - |          NA |
| RotateMergeSort          | 8192 | PipeOrgan          |   165,600.6 ns | 12,163.22 ns |  5,400.54 ns |  0.73 |    0.03 |    5 |         - |          NA |
| RotateMergeSortRecursive | 8192 | PipeOrgan          |   187,946.4 ns |  1,564.68 ns |    818.36 ns |  0.83 |    0.02 |    5 |         - |          NA |
| SymMergeSort             | 8192 | PipeOrgan          |    98,786.6 ns |  2,222.20 ns |    986.67 ns |  0.44 |    0.01 |    4 |         - |          NA |
| BlockMergeSort           | 8192 | PipeOrgan          |   261,146.8 ns | 18,380.51 ns |  9,613.36 ns |  1.15 |    0.05 |    6 |         - |          NA |
| NaturalMergeSort         | 8192 | PipeOrgan          |    18,977.1 ns |  2,302.86 ns |    821.22 ns |  0.08 |    0.00 |    2 |         - |          NA |
| TimSort                  | 8192 | PipeOrgan          |    18,757.2 ns |    314.41 ns |    112.12 ns |  0.08 |    0.00 |    2 |         - |          NA |
| PowerSort                | 8192 | PipeOrgan          |    12,396.5 ns |  1,042.93 ns |    463.07 ns |  0.05 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | PipeOrgan          |    16,105.7 ns |  1,570.33 ns |    821.31 ns |  0.07 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | PipeOrgan          |    17,549.9 ns |  1,066.28 ns |    557.68 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSortVariant          | 8192 | PipeOrgan          |    18,474.5 ns |    451.43 ns |    200.44 ns |  0.08 |    0.00 |    2 |         - |          NA |
| Glidesort                | 8192 | PipeOrgan          |    37,120.6 ns |  3,626.67 ns |  1,896.82 ns |  0.16 |    0.01 |    3 |         - |          NA |
| FlatStableSort           | 8192 | PipeOrgan          |    91,344.6 ns | 21,159.40 ns | 11,066.78 ns |  0.40 |    0.05 |    4 |         - |          NA |

### NetworkBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                  | Size | Pattern            | Mean         | Error        | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------------ |----- |------------------- |-------------:|-------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **BitonicSort**             | **256**  | **Random**             |  **11,328.2 ns** |    **548.03 ns** |   **286.63 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Random             |  22,722.8 ns |    298.80 ns |   156.28 ns |  2.01 |    0.05 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Random             |  16,651.7 ns |    130.41 ns |    68.21 ns |  1.47 |    0.04 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **SingleElementMoved** |  **10,131.2 ns** |    **280.39 ns** |   **146.65 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | SingleElementMoved |  23,114.7 ns |    454.16 ns |   201.65 ns |  2.28 |    0.04 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | SingleElementMoved |  16,799.8 ns |    244.92 ns |   128.10 ns |  1.66 |    0.03 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **Sorted**             |  **10,272.9 ns** |    **865.55 ns** |   **452.70 ns** |  **1.00** |    **0.06** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Sorted             |  22,955.7 ns |    186.23 ns |    82.69 ns |  2.24 |    0.09 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Sorted             |  16,848.4 ns |    388.08 ns |   172.31 ns |  1.64 |    0.07 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **Reversed**           |  **10,008.9 ns** |    **313.89 ns** |   **139.37 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Reversed           |  22,792.1 ns |    168.47 ns |    88.11 ns |  2.28 |    0.03 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Reversed           |  16,701.1 ns |     51.93 ns |    23.06 ns |  1.67 |    0.02 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **PipeOrgan**          |   **9,331.0 ns** |    **833.98 ns** |   **436.19 ns** |  **1.00** |    **0.06** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | PipeOrgan          |  22,437.1 ns |     89.93 ns |    39.93 ns |  2.41 |    0.11 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | PipeOrgan          |  16,701.8 ns |    108.32 ns |    48.10 ns |  1.79 |    0.08 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Random**             |  **96,174.4 ns** |  **2,922.62 ns** | **1,297.66 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Random             | 123,669.4 ns |  1,658.34 ns |   736.31 ns |  1.29 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Random             | 102,828.6 ns |  1,144.23 ns |   598.45 ns |  1.07 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **SingleElementMoved** |  **59,403.1 ns** |  **1,065.63 ns** |   **557.34 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | SingleElementMoved | 119,025.6 ns |  1,240.27 ns |   648.69 ns |  2.00 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | SingleElementMoved | 102,746.4 ns |    241.45 ns |   126.28 ns |  1.73 |    0.02 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Sorted**             |  **59,686.4 ns** |  **1,671.10 ns** |   **741.98 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Sorted             | 118,877.9 ns |  1,058.18 ns |   553.45 ns |  1.99 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Sorted             | 102,608.4 ns |    467.67 ns |   207.65 ns |  1.72 |    0.02 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Reversed**           |  **58,045.5 ns** |  **1,783.21 ns** |   **932.66 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Reversed           | 118,324.2 ns |    432.45 ns |   226.18 ns |  2.04 |    0.03 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Reversed           | 102,548.7 ns |    173.29 ns |    90.63 ns |  1.77 |    0.03 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **PipeOrgan**          |  **54,329.8 ns** |  **2,911.41 ns** | **1,522.72 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | PipeOrgan          | 116,748.8 ns |    558.55 ns |   292.13 ns |  2.15 |    0.06 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | PipeOrgan          | 102,613.0 ns |    431.70 ns |   225.79 ns |  1.89 |    0.05 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Random**             | **555,243.3 ns** | **13,538.57 ns** | **7,080.93 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Random             | 818,681.9 ns |  2,472.73 ns | 1,293.29 ns |  1.47 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Random             | 650,258.3 ns | 14,786.35 ns | 7,733.55 ns |  1.17 |    0.02 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **SingleElementMoved** | **326,697.3 ns** |  **4,919.30 ns** | **2,572.89 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | SingleElementMoved | 591,983.1 ns |  1,396.62 ns |   620.11 ns |  1.81 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | SingleElementMoved | 586,038.1 ns |  1,249.16 ns |   653.34 ns |  1.79 |    0.01 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Sorted**             | **322,720.1 ns** |  **7,832.94 ns** | **4,096.78 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Sorted             | 591,852.4 ns |    936.70 ns |   415.90 ns |  1.83 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Sorted             | 585,675.7 ns |    637.48 ns |   333.42 ns |  1.82 |    0.02 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Reversed**           | **315,461.8 ns** |  **4,078.58 ns** | **1,810.92 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Reversed           | 589,900.9 ns |  1,267.77 ns |   562.90 ns |  1.87 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Reversed           | 585,988.6 ns |  1,665.04 ns |   870.85 ns |  1.86 |    0.01 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **PipeOrgan**          | **297,691.3 ns** |  **8,944.79 ns** | **3,971.54 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | PipeOrgan          | 579,086.8 ns |  1,073.05 ns |   561.23 ns |  1.95 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | PipeOrgan          | 586,617.2 ns |  1,743.22 ns |   911.74 ns |  1.97 |    0.02 |    2 |         - |          NA |

### PartitionBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                       | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **QuickSort**                    | **256**  | **Random**             |     **2,631.8 ns** |    **254.16 ns** |    **112.85 ns** |  **1.00** |    **0.06** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 256  | Random             |     3,088.7 ns |     48.47 ns |     21.52 ns |  1.18 |    0.05 |    1 |         - |          NA |
| QuickSortMedian3             | 256  | Random             |     4,531.3 ns |    508.84 ns |    266.13 ns |  1.72 |    0.12 |    1 |         - |          NA |
| QuickSortMedian9             | 256  | Random             |     3,741.1 ns |     55.86 ns |     24.80 ns |  1.42 |    0.05 |    1 |         - |          NA |
| DualPivotQuickSort           | 256  | Random             |     2,199.5 ns |     26.58 ns |      9.48 ns |  0.84 |    0.03 |    1 |         - |          NA |
| StableQuickSort              | 256  | Random             |    11,580.3 ns |    462.99 ns |    242.15 ns |  4.41 |    0.19 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Random             |     6,792.7 ns |    137.54 ns |     61.07 ns |  2.58 |    0.10 |    2 |         - |          NA |
| DestswapStableQuickSort      | 256  | Random             |     8,745.9 ns |    355.93 ns |    158.03 ns |  3.33 |    0.14 |    3 |         - |          NA |
| IntroSort                    | 256  | Random             |     2,028.1 ns |    262.23 ns |    137.15 ns |  0.77 |    0.06 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | Random             |     1,636.4 ns |    115.39 ns |     51.23 ns |  0.62 |    0.03 |    1 |         - |          NA |
| PDQSort                      | 256  | Random             |     1,854.9 ns |    427.82 ns |    223.76 ns |  0.71 |    0.08 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | Random             |     3,130.4 ns |     59.82 ns |     26.56 ns |  1.19 |    0.05 |    1 |         - |          NA |
| StdSort                      | 256  | Random             |     1,856.2 ns |    332.84 ns |    147.78 ns |  0.71 |    0.06 |    1 |         - |          NA |
| BlockQuickSort               | 256  | Random             |     2,485.4 ns |     69.14 ns |     24.66 ns |  0.95 |    0.04 |    1 |         - |          NA |
| DotnetSort                   | 256  | Random             |     2,007.1 ns |    417.18 ns |    218.19 ns |  0.76 |    0.08 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **SingleElementMoved** |     **1,269.8 ns** |    **201.15 ns** |     **89.31 ns** |  **1.00** |    **0.09** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 256  | SingleElementMoved |     5,376.4 ns |    568.74 ns |    297.46 ns |  4.25 |    0.33 |    3 |         - |          NA |
| QuickSortMedian3             | 256  | SingleElementMoved |     5,247.7 ns |    456.25 ns |    238.63 ns |  4.15 |    0.30 |    3 |         - |          NA |
| QuickSortMedian9             | 256  | SingleElementMoved |     4,408.5 ns |    422.48 ns |    220.96 ns |  3.49 |    0.26 |    3 |         - |          NA |
| DualPivotQuickSort           | 256  | SingleElementMoved |     3,615.9 ns |     26.15 ns |     11.61 ns |  2.86 |    0.17 |    2 |         - |          NA |
| StableQuickSort              | 256  | SingleElementMoved |     8,787.6 ns |    511.06 ns |    267.30 ns |  6.95 |    0.46 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 256  | SingleElementMoved |     4,898.5 ns |    412.01 ns |    215.49 ns |  3.87 |    0.28 |    3 |         - |          NA |
| DestswapStableQuickSort      | 256  | SingleElementMoved |    10,725.2 ns |    296.01 ns |    154.82 ns |  8.48 |    0.51 |    5 |         - |          NA |
| IntroSort                    | 256  | SingleElementMoved |       863.3 ns |     17.86 ns |      9.34 ns |  0.68 |    0.04 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | SingleElementMoved |     1,091.5 ns |     22.45 ns |      9.97 ns |  0.86 |    0.05 |    1 |         - |          NA |
| PDQSort                      | 256  | SingleElementMoved |     1,099.1 ns |     14.60 ns |      5.21 ns |  0.87 |    0.05 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | SingleElementMoved |     1,454.6 ns |     39.82 ns |     14.20 ns |  1.15 |    0.07 |    1 |         - |          NA |
| StdSort                      | 256  | SingleElementMoved |     1,575.1 ns |    249.22 ns |    110.66 ns |  1.25 |    0.11 |    1 |         - |          NA |
| BlockQuickSort               | 256  | SingleElementMoved |     1,413.1 ns |     60.60 ns |     26.91 ns |  1.12 |    0.07 |    1 |         - |          NA |
| DotnetSort                   | 256  | SingleElementMoved |     1,018.2 ns |     24.87 ns |     11.04 ns |  0.80 |    0.05 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Sorted**             |       **867.8 ns** |     **10.35 ns** |      **4.60 ns** |  **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 256  | Sorted             |     7,164.7 ns |    154.80 ns |     68.73 ns |  8.26 |    0.08 |    6 |         - |          NA |
| QuickSortMedian3             | 256  | Sorted             |     6,558.8 ns |     56.37 ns |     25.03 ns |  7.56 |    0.05 |    6 |         - |          NA |
| QuickSortMedian9             | 256  | Sorted             |     4,800.1 ns |    464.62 ns |    243.01 ns |  5.53 |    0.27 |    5 |         - |          NA |
| DualPivotQuickSort           | 256  | Sorted             |     4,314.5 ns |    497.54 ns |    260.22 ns |  4.97 |    0.28 |    5 |         - |          NA |
| StableQuickSort              | 256  | Sorted             |     8,793.4 ns |    460.68 ns |    240.94 ns | 10.13 |    0.27 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Sorted             |     4,656.2 ns |      8.07 ns |      2.88 ns |  5.37 |    0.03 |    5 |         - |          NA |
| DestswapStableQuickSort      | 256  | Sorted             |    10,223.6 ns |    385.55 ns |    201.65 ns | 11.78 |    0.23 |    7 |         - |          NA |
| IntroSort                    | 256  | Sorted             |       299.0 ns |      2.92 ns |      1.53 ns |  0.34 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | Sorted             |     1,026.2 ns |     26.74 ns |     11.87 ns |  1.18 |    0.01 |    4 |         - |          NA |
| PDQSort                      | 256  | Sorted             |       373.7 ns |      5.39 ns |      2.40 ns |  0.43 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 256  | Sorted             |       376.0 ns |      4.03 ns |      2.11 ns |  0.43 |    0.00 |    2 |         - |          NA |
| StdSort                      | 256  | Sorted             |       490.8 ns |      1.43 ns |      0.63 ns |  0.57 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 256  | Sorted             |     1,204.8 ns |     22.42 ns |      8.00 ns |  1.39 |    0.01 |    4 |         - |          NA |
| DotnetSort                   | 256  | Sorted             |       871.0 ns |      6.21 ns |      3.25 ns |  1.00 |    0.01 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Reversed**           |       **973.5 ns** |     **19.09 ns** |      **9.98 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 256  | Reversed           |     5,187.1 ns |    530.77 ns |    235.67 ns |  5.33 |    0.23 |    5 |         - |          NA |
| QuickSortMedian3             | 256  | Reversed           |     7,559.5 ns |    433.03 ns |    192.27 ns |  7.77 |    0.20 |    6 |         - |          NA |
| QuickSortMedian9             | 256  | Reversed           |     4,828.8 ns |     79.97 ns |     28.52 ns |  4.96 |    0.06 |    5 |         - |          NA |
| DualPivotQuickSort           | 256  | Reversed           |     4,027.2 ns |    480.75 ns |    251.44 ns |  4.14 |    0.25 |    4 |         - |          NA |
| StableQuickSort              | 256  | Reversed           |     8,710.7 ns |    457.96 ns |    239.52 ns |  8.95 |    0.25 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Reversed           |     4,937.1 ns |    499.38 ns |    261.19 ns |  5.07 |    0.26 |    5 |         - |          NA |
| DestswapStableQuickSort      | 256  | Reversed           |    10,239.1 ns |    391.93 ns |    174.02 ns | 10.52 |    0.20 |    6 |         - |          NA |
| IntroSort                    | 256  | Reversed           |       564.6 ns |     48.33 ns |     21.46 ns |  0.58 |    0.02 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | Reversed           |     1,451.1 ns |     43.92 ns |     15.66 ns |  1.49 |    0.02 |    3 |         - |          NA |
| PDQSort                      | 256  | Reversed           |       548.7 ns |      4.86 ns |      2.16 ns |  0.56 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | Reversed           |       908.0 ns |     13.76 ns |      7.20 ns |  0.93 |    0.01 |    2 |         - |          NA |
| StdSort                      | 256  | Reversed           |       652.0 ns |     11.86 ns |      4.23 ns |  0.67 |    0.01 |    1 |         - |          NA |
| BlockQuickSort               | 256  | Reversed           |     1,435.7 ns |     13.66 ns |      6.06 ns |  1.47 |    0.02 |    3 |         - |          NA |
| DotnetSort                   | 256  | Reversed           |     1,543.3 ns |     25.28 ns |     11.23 ns |  1.59 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **PipeOrgan**          |     **7,598.9 ns** |    **101.10 ns** |     **44.89 ns** |  **1.00** |    **0.01** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 256  | PipeOrgan          |     4,928.3 ns |    511.22 ns |    226.99 ns |  0.65 |    0.03 |    4 |         - |          NA |
| QuickSortMedian3             | 256  | PipeOrgan          |     6,616.8 ns |    543.57 ns |    284.30 ns |  0.87 |    0.04 |    5 |         - |          NA |
| QuickSortMedian9             | 256  | PipeOrgan          |     4,397.2 ns |    579.89 ns |    303.29 ns |  0.58 |    0.04 |    4 |         - |          NA |
| DualPivotQuickSort           | 256  | PipeOrgan          |     1,727.4 ns |     55.45 ns |     24.62 ns |  0.23 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 256  | PipeOrgan          |     9,276.5 ns |    256.50 ns |    134.16 ns |  1.22 |    0.02 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 256  | PipeOrgan          |     4,648.0 ns |     36.24 ns |     12.92 ns |  0.61 |    0.00 |    4 |         - |          NA |
| DestswapStableQuickSort      | 256  | PipeOrgan          |    10,900.9 ns |    426.27 ns |    222.95 ns |  1.43 |    0.03 |    6 |         - |          NA |
| IntroSort                    | 256  | PipeOrgan          |     1,931.6 ns |    504.79 ns |    264.02 ns |  0.25 |    0.03 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | PipeOrgan          |     2,384.7 ns |    143.72 ns |     63.81 ns |  0.31 |    0.01 |    2 |         - |          NA |
| PDQSort                      | 256  | PipeOrgan          |     1,685.5 ns |     26.57 ns |     11.80 ns |  0.22 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | PipeOrgan          |     2,975.9 ns |     97.07 ns |     34.62 ns |  0.39 |    0.00 |    3 |         - |          NA |
| StdSort                      | 256  | PipeOrgan          |     2,319.7 ns |    408.08 ns |    213.43 ns |  0.31 |    0.03 |    2 |         - |          NA |
| BlockQuickSort               | 256  | PipeOrgan          |     4,189.7 ns |    113.86 ns |     40.60 ns |  0.55 |    0.01 |    4 |         - |          NA |
| DotnetSort                   | 256  | PipeOrgan          |     2,988.1 ns |    500.32 ns |    261.68 ns |  0.39 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Random**             |    **13,301.0 ns** |    **483.71 ns** |    **214.77 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Random             |    19,429.3 ns |  1,123.11 ns |    587.41 ns |  1.46 |    0.05 |    2 |         - |          NA |
| QuickSortMedian3             | 1024 | Random             |    23,830.0 ns |    870.44 ns |    455.26 ns |  1.79 |    0.04 |    3 |         - |          NA |
| QuickSortMedian9             | 1024 | Random             |    19,280.1 ns |    435.96 ns |    155.47 ns |  1.45 |    0.03 |    2 |         - |          NA |
| DualPivotQuickSort           | 1024 | Random             |    10,494.6 ns |    640.18 ns |    334.82 ns |  0.79 |    0.03 |    1 |         - |          NA |
| StableQuickSort              | 1024 | Random             |    84,433.5 ns |    389.39 ns |    172.89 ns |  6.35 |    0.10 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Random             |    34,706.1 ns |    541.98 ns |    240.64 ns |  2.61 |    0.04 |    4 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Random             |    39,812.1 ns |  1,046.50 ns |    547.34 ns |  2.99 |    0.06 |    4 |         - |          NA |
| IntroSort                    | 1024 | Random             |    11,304.8 ns |    632.52 ns |    330.82 ns |  0.85 |    0.03 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | Random             |     9,198.9 ns |    627.99 ns |    328.45 ns |  0.69 |    0.03 |    1 |         - |          NA |
| PDQSort                      | 1024 | Random             |     9,029.1 ns |    556.23 ns |    290.92 ns |  0.68 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | Random             |    13,580.7 ns |    438.22 ns |    194.57 ns |  1.02 |    0.02 |    1 |         - |          NA |
| StdSort                      | 1024 | Random             |     8,924.1 ns |    496.57 ns |    259.71 ns |  0.67 |    0.02 |    1 |         - |          NA |
| BlockQuickSort               | 1024 | Random             |    13,071.7 ns |    545.79 ns |    242.33 ns |  0.98 |    0.02 |    1 |         - |          NA |
| DotnetSort                   | 1024 | Random             |    10,718.4 ns |    537.75 ns |    281.26 ns |  0.81 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **SingleElementMoved** |     **5,930.6 ns** |    **448.85 ns** |    **234.75 ns** |  **1.00** |    **0.05** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 1024 | SingleElementMoved |    39,412.9 ns |    290.48 ns |    151.93 ns |  6.66 |    0.26 |    5 |         - |          NA |
| QuickSortMedian3             | 1024 | SingleElementMoved |    32,056.4 ns |    951.58 ns |    497.69 ns |  5.41 |    0.22 |    4 |         - |          NA |
| QuickSortMedian9             | 1024 | SingleElementMoved |    22,040.0 ns |    715.87 ns |    374.42 ns |  3.72 |    0.15 |    3 |         - |          NA |
| DualPivotQuickSort           | 1024 | SingleElementMoved |    21,344.9 ns |    581.95 ns |    258.39 ns |  3.60 |    0.14 |    3 |         - |          NA |
| StableQuickSort              | 1024 | SingleElementMoved |    43,196.5 ns |    566.40 ns |    251.48 ns |  7.29 |    0.28 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | SingleElementMoved |    23,110.9 ns |    720.33 ns |    319.83 ns |  3.90 |    0.16 |    3 |         - |          NA |
| DestswapStableQuickSort      | 1024 | SingleElementMoved |    43,563.9 ns |  1,528.95 ns |    799.67 ns |  7.36 |    0.31 |    5 |         - |          NA |
| IntroSort                    | 1024 | SingleElementMoved |     4,567.1 ns |    764.66 ns |    399.93 ns |  0.77 |    0.07 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | SingleElementMoved |     5,994.7 ns |    293.22 ns |    130.19 ns |  1.01 |    0.04 |    2 |         - |          NA |
| PDQSort                      | 1024 | SingleElementMoved |     4,898.7 ns |     68.45 ns |     30.39 ns |  0.83 |    0.03 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | SingleElementMoved |     6,196.0 ns |     48.78 ns |     21.66 ns |  1.05 |    0.04 |    2 |         - |          NA |
| StdSort                      | 1024 | SingleElementMoved |     7,103.7 ns |    369.79 ns |    193.41 ns |  1.20 |    0.06 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | SingleElementMoved |     7,862.7 ns |    292.10 ns |    129.69 ns |  1.33 |    0.05 |    2 |         - |          NA |
| DotnetSort                   | 1024 | SingleElementMoved |     6,103.3 ns |    521.78 ns |    186.07 ns |  1.03 |    0.05 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Sorted**             |     **4,234.6 ns** |    **430.14 ns** |    **224.97 ns** |  **1.00** |    **0.07** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Sorted             |    53,104.0 ns |    886.62 ns |    463.72 ns | 12.57 |    0.62 |    6 |         - |          NA |
| QuickSortMedian3             | 1024 | Sorted             |    43,551.3 ns |  1,128.92 ns |    590.45 ns | 10.31 |    0.52 |    6 |         - |          NA |
| QuickSortMedian9             | 1024 | Sorted             |    22,645.3 ns |  1,141.46 ns |    597.01 ns |  5.36 |    0.29 |    5 |         - |          NA |
| DualPivotQuickSort           | 1024 | Sorted             |    21,770.4 ns |    277.68 ns |    123.29 ns |  5.15 |    0.25 |    5 |         - |          NA |
| StableQuickSort              | 1024 | Sorted             |    42,485.1 ns |    549.03 ns |    287.15 ns | 10.06 |    0.49 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Sorted             |    22,610.6 ns |    738.84 ns |    386.43 ns |  5.35 |    0.27 |    5 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Sorted             |    44,727.8 ns |    679.93 ns |    355.62 ns | 10.59 |    0.52 |    6 |         - |          NA |
| IntroSort                    | 1024 | Sorted             |     1,107.7 ns |      3.74 ns |      1.66 ns |  0.26 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | Sorted             |     4,969.9 ns |    494.89 ns |    258.83 ns |  1.18 |    0.08 |    3 |         - |          NA |
| PDQSort                      | 1024 | Sorted             |     1,323.5 ns |      4.71 ns |      2.09 ns |  0.31 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | Sorted             |     1,384.8 ns |    228.53 ns |    101.47 ns |  0.33 |    0.03 |    1 |         - |          NA |
| StdSort                      | 1024 | Sorted             |     1,801.1 ns |     12.56 ns |      5.58 ns |  0.43 |    0.02 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | Sorted             |     6,256.3 ns |     42.77 ns |     15.25 ns |  1.48 |    0.07 |    4 |         - |          NA |
| DotnetSort                   | 1024 | Sorted             |     4,333.4 ns |    486.33 ns |    215.94 ns |  1.03 |    0.07 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Reversed**           |     **4,604.7 ns** |     **81.16 ns** |     **28.94 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Reversed           |    38,640.3 ns |    816.05 ns |    426.81 ns |  8.39 |    0.10 |    7 |         - |          NA |
| QuickSortMedian3             | 1024 | Reversed           |    52,123.6 ns |    392.21 ns |    174.14 ns | 11.32 |    0.07 |    7 |         - |          NA |
| QuickSortMedian9             | 1024 | Reversed           |    22,375.9 ns |    651.80 ns |    340.90 ns |  4.86 |    0.08 |    6 |         - |          NA |
| DualPivotQuickSort           | 1024 | Reversed           |    20,069.0 ns |    472.28 ns |    247.01 ns |  4.36 |    0.06 |    6 |         - |          NA |
| StableQuickSort              | 1024 | Reversed           |    42,854.2 ns |    612.27 ns |    320.23 ns |  9.31 |    0.08 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Reversed           |    23,099.6 ns |    548.80 ns |    287.03 ns |  5.02 |    0.07 |    6 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Reversed           |    43,860.6 ns |    781.73 ns |    347.09 ns |  9.53 |    0.09 |    7 |         - |          NA |
| IntroSort                    | 1024 | Reversed           |     2,947.6 ns |     79.86 ns |     35.46 ns |  0.64 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 1024 | Reversed           |     7,447.2 ns |    336.99 ns |    176.25 ns |  1.62 |    0.04 |    4 |         - |          NA |
| PDQSort                      | 1024 | Reversed           |     2,029.6 ns |    372.62 ns |    194.89 ns |  0.44 |    0.04 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | Reversed           |     3,049.5 ns |      8.27 ns |      2.95 ns |  0.66 |    0.00 |    2 |         - |          NA |
| StdSort                      | 1024 | Reversed           |     2,544.4 ns |    251.97 ns |    131.79 ns |  0.55 |    0.03 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | Reversed           |     7,399.2 ns |     53.87 ns |     28.18 ns |  1.61 |    0.01 |    4 |         - |          NA |
| DotnetSort                   | 1024 | Reversed           |     8,930.1 ns |    631.73 ns |    330.41 ns |  1.94 |    0.07 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **PipeOrgan**          |    **97,872.4 ns** |    **594.09 ns** |    **211.86 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| QuickSort3way                | 1024 | PipeOrgan          |    31,297.8 ns |    666.50 ns |    348.59 ns |  0.32 |    0.00 |    4 |         - |          NA |
| QuickSortMedian3             | 1024 | PipeOrgan          |    38,566.9 ns |  1,153.07 ns |    603.08 ns |  0.39 |    0.01 |    5 |         - |          NA |
| QuickSortMedian9             | 1024 | PipeOrgan          |    22,328.9 ns |    608.19 ns |    318.09 ns |  0.23 |    0.00 |    3 |         - |          NA |
| DualPivotQuickSort           | 1024 | PipeOrgan          |     9,245.3 ns |    387.73 ns |    202.79 ns |  0.09 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 1024 | PipeOrgan          |    45,728.7 ns |    548.27 ns |    286.75 ns |  0.47 |    0.00 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | PipeOrgan          |    22,422.3 ns |    381.49 ns |    169.38 ns |  0.23 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 1024 | PipeOrgan          |    50,578.8 ns |  1,376.18 ns |    719.77 ns |  0.52 |    0.01 |    5 |         - |          NA |
| IntroSort                    | 1024 | PipeOrgan          |    11,518.6 ns |  1,787.34 ns |    934.81 ns |  0.12 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 1024 | PipeOrgan          |    13,850.2 ns |    408.85 ns |    181.53 ns |  0.14 |    0.00 |    2 |         - |          NA |
| PDQSort                      | 1024 | PipeOrgan          |     8,806.1 ns |    435.24 ns |    193.25 ns |  0.09 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | PipeOrgan          |    16,103.6 ns |    224.41 ns |    117.37 ns |  0.16 |    0.00 |    2 |         - |          NA |
| StdSort                      | 1024 | PipeOrgan          |    13,215.3 ns |    448.50 ns |    234.58 ns |  0.14 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | PipeOrgan          |    23,190.1 ns |    506.69 ns |    265.01 ns |  0.24 |    0.00 |    3 |         - |          NA |
| DotnetSort                   | 1024 | PipeOrgan          |    16,145.7 ns |  2,351.02 ns |  1,229.63 ns |  0.16 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Random**             |   **421,666.1 ns** |  **3,280.54 ns** |  **1,715.79 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Random             |   443,973.8 ns |  4,160.80 ns |  1,847.42 ns |  1.05 |    0.01 |    2 |         - |          NA |
| QuickSortMedian3             | 8192 | Random             |   537,558.1 ns |  3,792.86 ns |  1,684.05 ns |  1.27 |    0.01 |    2 |         - |          NA |
| QuickSortMedian9             | 8192 | Random             |   526,887.2 ns | 14,957.92 ns |  7,823.28 ns |  1.25 |    0.02 |    2 |         - |          NA |
| DualPivotQuickSort           | 8192 | Random             |   350,094.2 ns |    998.78 ns |    522.38 ns |  0.83 |    0.00 |    2 |         - |          NA |
| StableQuickSort              | 8192 | Random             | 1,170,897.2 ns | 14,167.89 ns |  6,290.63 ns |  2.78 |    0.02 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Random             |   957,941.1 ns |  3,977.22 ns |  2,080.17 ns |  2.27 |    0.01 |    3 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Random             |   842,522.7 ns |  3,684.77 ns |  1,927.21 ns |  2.00 |    0.01 |    3 |         - |          NA |
| IntroSort                    | 8192 | Random             |   368,375.8 ns |  3,768.95 ns |  1,971.23 ns |  0.87 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 8192 | Random             |   355,936.2 ns |    843.15 ns |    374.36 ns |  0.84 |    0.00 |    2 |         - |          NA |
| PDQSort                      | 8192 | Random             |   345,493.9 ns |  2,012.64 ns |    893.62 ns |  0.82 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | Random             |   194,813.5 ns |  1,839.97 ns |    816.96 ns |  0.46 |    0.00 |    1 |         - |          NA |
| StdSort                      | 8192 | Random             |   339,658.5 ns |  4,681.68 ns |  2,448.61 ns |  0.81 |    0.01 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | Random             |   422,933.4 ns |  1,363.95 ns |    713.37 ns |  1.00 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 8192 | Random             |   338,062.8 ns |  4,232.16 ns |  2,213.50 ns |  0.80 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **SingleElementMoved** |    **55,014.7 ns** |  **1,047.83 ns** |    **548.04 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 8192 | SingleElementMoved |   869,068.5 ns | 14,562.28 ns |  7,616.35 ns | 15.80 |    0.20 |    7 |         - |          NA |
| QuickSortMedian3             | 8192 | SingleElementMoved |   586,720.6 ns | 34,126.21 ns | 17,848.67 ns | 10.67 |    0.32 |    6 |         - |          NA |
| QuickSortMedian9             | 8192 | SingleElementMoved |   211,687.5 ns |  4,879.36 ns |  2,166.47 ns |  3.85 |    0.05 |    4 |         - |          NA |
| DualPivotQuickSort           | 8192 | SingleElementMoved |   141,504.7 ns |  3,943.69 ns |  1,751.02 ns |  2.57 |    0.04 |    3 |         - |          NA |
| StableQuickSort              | 8192 | SingleElementMoved |   434,146.1 ns |  2,603.86 ns |  1,156.13 ns |  7.89 |    0.08 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | SingleElementMoved |   228,836.1 ns |  1,157.65 ns |    514.00 ns |  4.16 |    0.04 |    4 |         - |          NA |
| DestswapStableQuickSort      | 8192 | SingleElementMoved |   392,719.0 ns |  5,625.00 ns |  2,941.98 ns |  7.14 |    0.08 |    5 |         - |          NA |
| IntroSort                    | 8192 | SingleElementMoved |    44,157.2 ns |  6,397.15 ns |  3,345.83 ns |  0.80 |    0.06 |    1 |         - |          NA |
| IntroSortDotnet              | 8192 | SingleElementMoved |    61,926.6 ns |  1,208.44 ns |    632.04 ns |  1.13 |    0.02 |    1 |         - |          NA |
| PDQSort                      | 8192 | SingleElementMoved |    42,965.8 ns |  1,616.77 ns |    717.86 ns |  0.78 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 8192 | SingleElementMoved |    53,549.5 ns |    719.32 ns |    376.22 ns |  0.97 |    0.01 |    1 |         - |          NA |
| StdSort                      | 8192 | SingleElementMoved |    63,363.7 ns |  2,264.27 ns |  1,005.35 ns |  1.15 |    0.02 |    1 |         - |          NA |
| BlockQuickSort               | 8192 | SingleElementMoved |    80,078.0 ns |    767.21 ns |    401.27 ns |  1.46 |    0.02 |    2 |         - |          NA |
| DotnetSort                   | 8192 | SingleElementMoved |    59,338.0 ns |  5,576.15 ns |  2,475.84 ns |  1.08 |    0.04 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Sorted**             |    **42,821.6 ns** |  **1,445.81 ns** |    **756.19 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Sorted             | 1,183,278.0 ns | 20,253.58 ns | 10,593.01 ns | 27.64 |    0.52 |    9 |         - |          NA |
| QuickSortMedian3             | 8192 | Sorted             |   915,061.2 ns | 36,402.75 ns | 19,039.34 ns | 21.37 |    0.55 |    8 |         - |          NA |
| QuickSortMedian9             | 8192 | Sorted             |   217,042.3 ns | 12,265.97 ns |  6,415.34 ns |  5.07 |    0.16 |    6 |         - |          NA |
| DualPivotQuickSort           | 8192 | Sorted             |   155,565.8 ns |  3,280.49 ns |  1,715.76 ns |  3.63 |    0.07 |    5 |         - |          NA |
| StableQuickSort              | 8192 | Sorted             |   434,725.4 ns |  8,366.75 ns |  4,375.97 ns | 10.15 |    0.20 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Sorted             |   225,844.3 ns |  3,245.05 ns |  1,440.82 ns |  5.28 |    0.09 |    6 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Sorted             |   394,599.4 ns |  8,115.60 ns |  3,603.38 ns |  9.22 |    0.17 |    7 |         - |          NA |
| IntroSort                    | 8192 | Sorted             |     8,943.9 ns |  1,001.78 ns |    444.80 ns |  0.21 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet              | 8192 | Sorted             |    48,539.1 ns |  2,974.62 ns |  1,555.79 ns |  1.13 |    0.04 |    3 |         - |          NA |
| PDQSort                      | 8192 | Sorted             |    10,374.7 ns |    372.99 ns |    165.61 ns |  0.24 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 8192 | Sorted             |    10,722.5 ns |    865.39 ns |    308.61 ns |  0.25 |    0.01 |    1 |         - |          NA |
| StdSort                      | 8192 | Sorted             |    15,111.4 ns |    828.34 ns |    433.24 ns |  0.35 |    0.01 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | Sorted             |    68,240.2 ns |  1,825.41 ns |    810.49 ns |  1.59 |    0.03 |    4 |         - |          NA |
| DotnetSort                   | 8192 | Sorted             |    47,148.5 ns |  5,968.01 ns |  3,121.38 ns |  1.10 |    0.07 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Reversed**           |    **46,570.9 ns** |  **1,561.45 ns** |    **816.67 ns** |  **1.00** |    **0.02** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Reversed           |   840,728.7 ns | 19,215.56 ns | 10,050.11 ns | 18.06 |    0.36 |   10 |         - |          NA |
| QuickSortMedian3             | 8192 | Reversed           | 1,141,526.1 ns | 38,082.67 ns | 19,917.97 ns | 24.52 |    0.57 |   11 |         - |          NA |
| QuickSortMedian9             | 8192 | Reversed           |   205,276.0 ns |  3,073.93 ns |  1,607.72 ns |  4.41 |    0.08 |    8 |         - |          NA |
| DualPivotQuickSort           | 8192 | Reversed           |   143,746.1 ns |  2,644.46 ns |  1,383.11 ns |  3.09 |    0.06 |    7 |         - |          NA |
| StableQuickSort              | 8192 | Reversed           |   433,985.1 ns |  1,488.92 ns |    778.74 ns |  9.32 |    0.15 |    9 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Reversed           |   226,650.3 ns |  3,398.30 ns |  1,777.38 ns |  4.87 |    0.09 |    8 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Reversed           |   392,867.8 ns |  4,518.01 ns |  2,363.01 ns |  8.44 |    0.15 |    9 |         - |          NA |
| IntroSort                    | 8192 | Reversed           |    23,983.6 ns |    658.74 ns |    292.49 ns |  0.52 |    0.01 |    3 |         - |          NA |
| IntroSortDotnet              | 8192 | Reversed           |    77,060.7 ns |  1,591.08 ns |    832.17 ns |  1.66 |    0.03 |    5 |         - |          NA |
| PDQSort                      | 8192 | Reversed           |    14,435.2 ns |    178.61 ns |     79.30 ns |  0.31 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 8192 | Reversed           |    23,375.2 ns |  1,226.45 ns |    641.46 ns |  0.50 |    0.02 |    3 |         - |          NA |
| StdSort                      | 8192 | Reversed           |    19,245.1 ns |  1,146.64 ns |    599.71 ns |  0.41 |    0.01 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | Reversed           |    76,115.3 ns |    558.82 ns |    292.28 ns |  1.63 |    0.03 |    5 |         - |          NA |
| DotnetSort                   | 8192 | Reversed           |   102,276.4 ns |  8,159.02 ns |  4,267.32 ns |  2.20 |    0.09 |    6 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **PipeOrgan**          | **5,452,731.7 ns** | **90,289.67 ns** | **47,223.24 ns** |  **1.00** |    **0.01** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 8192 | PipeOrgan          |   459,083.3 ns | 12,478.90 ns |  6,526.71 ns |  0.08 |    0.00 |    4 |         - |          NA |
| QuickSortMedian3             | 8192 | PipeOrgan          |   502,524.9 ns |  9,827.20 ns |  4,363.34 ns |  0.09 |    0.00 |    4 |         - |          NA |
| QuickSortMedian9             | 8192 | PipeOrgan          |   285,986.2 ns |  7,275.24 ns |  3,805.09 ns |  0.05 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort           | 8192 | PipeOrgan          |   123,399.0 ns |  1,432.14 ns |    635.88 ns |  0.02 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 8192 | PipeOrgan          |   480,557.0 ns |  9,971.68 ns |  5,215.38 ns |  0.09 |    0.00 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | PipeOrgan          |   228,624.6 ns |  4,803.09 ns |  2,132.60 ns |  0.04 |    0.00 |    2 |         - |          NA |
| DestswapStableQuickSort      | 8192 | PipeOrgan          |   476,229.2 ns | 13,093.96 ns |  6,848.39 ns |  0.09 |    0.00 |    4 |         - |          NA |
| IntroSort                    | 8192 | PipeOrgan          |   282,642.8 ns | 12,464.57 ns |  6,519.21 ns |  0.05 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet              | 8192 | PipeOrgan          |   373,023.6 ns |  8,321.85 ns |  4,352.49 ns |  0.07 |    0.00 |    3 |         - |          NA |
| PDQSort                      | 8192 | PipeOrgan          |   118,407.7 ns |  4,195.29 ns |  2,194.22 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 8192 | PipeOrgan          |   201,485.7 ns |  2,031.98 ns |  1,062.77 ns |  0.04 |    0.00 |    2 |         - |          NA |
| StdSort                      | 8192 | PipeOrgan          |   293,082.3 ns |  3,780.67 ns |  1,678.64 ns |  0.05 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | PipeOrgan          |   256,372.4 ns |  1,001.23 ns |    444.55 ns |  0.05 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 8192 | PipeOrgan          |   368,266.3 ns | 14,079.35 ns |  6,251.32 ns |  0.07 |    0.00 |    3 |         - |          NA |

### SelectionBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method              | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **SelectionSort**       | **256**  | **Random**             |    **26,444.8 ns** |    **319.73 ns** |    **167.22 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Random             |    19,543.3 ns |    210.40 ns |     93.42 ns |  0.74 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | Random             |    91,020.7 ns |  3,110.87 ns |  1,381.24 ns |  3.44 |    0.05 |    4 |         - |          NA |
| PancakeSort         | 256  | Random             |    44,078.7 ns |  2,652.52 ns |  1,387.32 ns |  1.67 |    0.05 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **SingleElementMoved** |    **22,251.1 ns** |    **430.71 ns** |    **225.27 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | SingleElementMoved |    16,597.9 ns |    733.58 ns |    383.68 ns |  0.75 |    0.02 |    1 |         - |          NA |
| CycleSort           | 256  | SingleElementMoved |    50,442.4 ns |  1,942.45 ns |  1,015.94 ns |  2.27 |    0.05 |    3 |         - |          NA |
| PancakeSort         | 256  | SingleElementMoved |    19,903.7 ns |    243.45 ns |    108.09 ns |  0.89 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **Sorted**             |    **22,379.2 ns** |    **483.73 ns** |    **253.00 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Sorted             |    11,298.1 ns |    639.81 ns |    284.08 ns |  0.50 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | Sorted             |    32,185.6 ns |    466.97 ns |    244.24 ns |  1.44 |    0.02 |    4 |         - |          NA |
| PancakeSort         | 256  | Sorted             |    15,137.1 ns |     88.92 ns |     39.48 ns |  0.68 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **Reversed**           |    **21,187.3 ns** |  **2,950.46 ns** |  **1,543.15 ns** |  **1.00** |    **0.10** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Reversed           |    16,450.2 ns |    314.12 ns |    139.47 ns |  0.78 |    0.05 |    1 |         - |          NA |
| CycleSort           | 256  | Reversed           |    44,317.2 ns |    713.54 ns |    316.82 ns |  2.10 |    0.14 |    2 |         - |          NA |
| PancakeSort         | 256  | Reversed           |    18,937.8 ns |  5,307.82 ns |  2,776.10 ns |  0.90 |    0.14 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **PipeOrgan**          |    **24,918.8 ns** |    **852.33 ns** |    **445.79 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | PipeOrgan          |    21,525.9 ns |    213.30 ns |    111.56 ns |  0.86 |    0.02 |    1 |         - |          NA |
| CycleSort           | 256  | PipeOrgan          |    61,072.2 ns |  3,276.05 ns |  1,454.59 ns |  2.45 |    0.07 |    3 |         - |          NA |
| PancakeSort         | 256  | PipeOrgan          |    33,346.6 ns |    460.65 ns |    204.53 ns |  1.34 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Random**             |   **370,322.7 ns** |  **5,476.12 ns** |  **2,864.12 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Random             |   276,036.5 ns |  2,402.66 ns |  1,066.80 ns |  0.75 |    0.01 |    1 |         - |          NA |
| CycleSort           | 1024 | Random             | 1,680,200.5 ns | 14,761.26 ns |  7,720.42 ns |  4.54 |    0.04 |    4 |         - |          NA |
| PancakeSort         | 1024 | Random             |   636,973.1 ns |  6,466.93 ns |  2,871.36 ns |  1.72 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **SingleElementMoved** |   **333,376.8 ns** |  **3,076.84 ns** |  **1,366.14 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | SingleElementMoved |   246,716.9 ns |  5,803.18 ns |  3,035.17 ns |  0.74 |    0.01 |    1 |         - |          NA |
| CycleSort           | 1024 | SingleElementMoved |   794,402.7 ns | 24,087.99 ns | 12,598.48 ns |  2.38 |    0.04 |    3 |         - |          NA |
| PancakeSort         | 1024 | SingleElementMoved |   297,660.8 ns |  9,633.87 ns |  5,038.70 ns |  0.89 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Sorted**             |   **347,724.2 ns** | **37,939.81 ns** | **19,843.25 ns** |  **1.00** |    **0.07** |    **3** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Sorted             |   167,525.8 ns |  1,424.86 ns |    745.23 ns |  0.48 |    0.02 |    1 |         - |          NA |
| CycleSort           | 1024 | Sorted             |   496,286.7 ns |  4,529.14 ns |  1,615.14 ns |  1.43 |    0.07 |    4 |         - |          NA |
| PancakeSort         | 1024 | Sorted             |   224,179.8 ns |  1,003.67 ns |    445.64 ns |  0.65 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Reversed**           |   **335,975.6 ns** | **22,375.22 ns** | **11,702.67 ns** |  **1.00** |    **0.05** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Reversed           |   249,228.1 ns |    743.39 ns |    330.07 ns |  0.74 |    0.02 |    1 |         - |          NA |
| CycleSort           | 1024 | Reversed           |   674,478.0 ns | 12,144.74 ns |  6,351.93 ns |  2.01 |    0.07 |    3 |         - |          NA |
| PancakeSort         | 1024 | Reversed           |   320,122.7 ns |  7,952.24 ns |  3,530.84 ns |  0.95 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **PipeOrgan**          |   **356,732.9 ns** | **22,635.31 ns** | **11,838.70 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | PipeOrgan          |   311,989.2 ns |  5,436.04 ns |  2,413.64 ns |  0.88 |    0.03 |    1 |         - |          NA |
| CycleSort           | 1024 | PipeOrgan          |   955,984.2 ns | 30,666.08 ns | 13,615.93 ns |  2.68 |    0.09 |    3 |         - |          NA |
| PancakeSort         | 1024 | PipeOrgan          |   522,304.8 ns | 41,965.16 ns | 21,948.59 ns |  1.47 |    0.07 |    2 |         - |          NA |

### StringBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method             | Size | Pattern            | Mean               | Error          | StdDev        | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------- |----- |------------------- |-------------------:|---------------:|--------------:|------:|--------:|-----:|----------:|------------:|
| **QuickSort**          | **256**  | **Random**             |       **187,557.7 ns** |     **3,146.7 ns** |   **1,645.79 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| QuickSort3way      | 256  | Random             |       158,435.6 ns |     1,383.3 ns |     614.21 ns |  0.84 |    0.01 |    1 |         - |          NA |
| QuickSortMedian3   | 256  | Random             |       164,516.0 ns |     2,187.8 ns |     971.39 ns |  0.88 |    0.01 |    1 |         - |          NA |
| QuickSortMedian9   | 256  | Random             |       167,083.6 ns |     1,606.1 ns |     713.11 ns |  0.89 |    0.01 |    1 |         - |          NA |
| DualPivotQuickSort | 256  | Random             |       199,930.1 ns |     2,694.4 ns |   1,409.21 ns |  1.07 |    0.01 |    1 |         - |          NA |
| StableQuickSort    | 256  | Random             |       299,101.8 ns |     1,323.8 ns |     587.77 ns |  1.59 |    0.01 |    2 |         - |          NA |
| IntroSort          | 256  | Random             |       171,520.8 ns |     2,348.8 ns |   1,042.87 ns |  0.91 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | Random             |       159,625.1 ns |     3,211.7 ns |   1,679.78 ns |  0.85 |    0.01 |    1 |         - |          NA |
| PDQSort            | 256  | Random             |       231,220.7 ns |       556.2 ns |     246.96 ns |  1.23 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | Random             |       211,617.4 ns |       528.0 ns |     234.43 ns |  1.13 |    0.01 |    1 |         - |          NA |
| StdSort            | 256  | Random             |       191,754.0 ns |     1,339.5 ns |     700.59 ns |  1.02 |    0.01 |    1 |         - |          NA |
| BlockQuickSort     | 256  | Random             |       174,657.7 ns |     5,030.6 ns |   2,631.10 ns |  0.93 |    0.02 |    1 |         - |          NA |
| DotnetSort         | 256  | Random             |       164,069.3 ns |     3,264.0 ns |   1,707.14 ns |  0.87 |    0.01 |    1 |         - |          NA |
|      |                    |                    |                |               |       |         |      |           |             |
| **QuickSort**          | **256**  | **SingleElementMoved** |       **128,083.2 ns** |       **667.8 ns** |     **296.50 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| QuickSort3way      | 256  | SingleElementMoved |       199,945.1 ns |     1,713.1 ns |     895.99 ns |  1.56 |    0.01 |    2 |         - |          NA |
| QuickSortMedian3   | 256  | SingleElementMoved |       174,884.8 ns |     1,365.7 ns |     714.26 ns |  1.37 |    0.01 |    2 |         - |          NA |
| QuickSortMedian9   | 256  | SingleElementMoved |       167,669.1 ns |     2,891.5 ns |   1,283.83 ns |  1.31 |    0.01 |    2 |         - |          NA |
| DualPivotQuickSort | 256  | SingleElementMoved |       312,662.9 ns |     1,450.4 ns |     643.99 ns |  2.44 |    0.01 |    3 |         - |          NA |
| StableQuickSort    | 256  | SingleElementMoved |       236,843.9 ns |     1,819.7 ns |     807.97 ns |  1.85 |    0.01 |    2 |         - |          NA |
| IntroSort          | 256  | SingleElementMoved |        89,613.8 ns |     1,947.9 ns |     864.90 ns |  0.70 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | SingleElementMoved |       113,390.2 ns |       967.1 ns |     429.39 ns |  0.89 |    0.00 |    1 |         - |          NA |
| PDQSort            | 256  | SingleElementMoved |       121,347.9 ns |     1,533.7 ns |     802.15 ns |  0.95 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | SingleElementMoved |       122,342.5 ns |       769.4 ns |     274.37 ns |  0.96 |    0.00 |    1 |         - |          NA |
| StdSort            | 256  | SingleElementMoved |       126,862.1 ns |       329.8 ns |     172.50 ns |  0.99 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 256  | SingleElementMoved |       102,919.0 ns |       230.2 ns |      82.10 ns |  0.80 |    0.00 |    1 |         - |          NA |
| DotnetSort         | 256  | SingleElementMoved |       114,855.5 ns |     3,172.1 ns |   1,659.09 ns |  0.90 |    0.01 |    1 |         - |          NA |
|      |                    |                    |                |               |       |         |      |           |             |
| **QuickSort**          | **256**  | **Sorted**             |       **114,620.9 ns** |       **970.1 ns** |     **507.39 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 256  | Sorted             |       258,733.4 ns |       810.8 ns |     360.02 ns |  2.26 |    0.01 |    4 |         - |          NA |
| QuickSortMedian3   | 256  | Sorted             |       222,629.2 ns |     1,313.8 ns |     583.34 ns |  1.94 |    0.01 |    4 |         - |          NA |
| QuickSortMedian9   | 256  | Sorted             |       179,060.4 ns |     1,588.7 ns |     830.92 ns |  1.56 |    0.01 |    3 |         - |          NA |
| DualPivotQuickSort | 256  | Sorted             |       399,996.2 ns |     1,417.6 ns |     629.43 ns |  3.49 |    0.02 |    5 |         - |          NA |
| StableQuickSort    | 256  | Sorted             |       242,319.6 ns |     1,410.2 ns |     626.15 ns |  2.11 |    0.01 |    4 |         - |          NA |
| IntroSort          | 256  | Sorted             |        35,020.1 ns |       231.6 ns |     121.12 ns |  0.31 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | Sorted             |        88,773.6 ns |     2,747.8 ns |   1,437.16 ns |  0.77 |    0.01 |    2 |         - |          NA |
| PDQSort            | 256  | Sorted             |        36,531.0 ns |       489.5 ns |     217.36 ns |  0.32 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | Sorted             |        36,523.0 ns |       818.0 ns |     363.19 ns |  0.32 |    0.00 |    1 |         - |          NA |
| StdSort            | 256  | Sorted             |        41,414.6 ns |       394.1 ns |     174.99 ns |  0.36 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 256  | Sorted             |        99,941.0 ns |       697.3 ns |     364.70 ns |  0.87 |    0.00 |    2 |         - |          NA |
| DotnetSort         | 256  | Sorted             |        86,813.6 ns |       671.3 ns |     298.06 ns |  0.76 |    0.00 |    2 |         - |          NA |
|      |                    |                    |                |               |       |         |      |           |             |
| **QuickSort**          | **256**  | **Reversed**           |       **110,029.0 ns** |     **1,387.6 ns** |     **616.09 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 256  | Reversed           |       199,954.5 ns |     1,486.2 ns |     659.88 ns |  1.82 |    0.01 |    4 |         - |          NA |
| QuickSortMedian3   | 256  | Reversed           |       259,434.0 ns |     2,872.2 ns |   1,502.23 ns |  2.36 |    0.02 |    5 |         - |          NA |
| QuickSortMedian9   | 256  | Reversed           |       185,987.0 ns |     1,840.8 ns |     817.33 ns |  1.69 |    0.01 |    4 |         - |          NA |
| DualPivotQuickSort | 256  | Reversed           |       350,473.4 ns |     1,736.4 ns |     908.15 ns |  3.19 |    0.02 |    6 |         - |          NA |
| StableQuickSort    | 256  | Reversed           |       247,150.3 ns |     3,416.8 ns |   1,787.06 ns |  2.25 |    0.02 |    5 |         - |          NA |
| IntroSort          | 256  | Reversed           |        58,161.9 ns |       456.7 ns |     238.89 ns |  0.53 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | Reversed           |       140,575.5 ns |     3,119.9 ns |   1,631.77 ns |  1.28 |    0.02 |    3 |         - |          NA |
| PDQSort            | 256  | Reversed           |        53,533.3 ns |       945.2 ns |     494.33 ns |  0.49 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | Reversed           |        53,541.1 ns |       714.1 ns |     373.47 ns |  0.49 |    0.00 |    1 |         - |          NA |
| StdSort            | 256  | Reversed           |        55,338.9 ns |       500.0 ns |     222.02 ns |  0.50 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 256  | Reversed           |        91,997.4 ns |       188.4 ns |      83.66 ns |  0.84 |    0.00 |    2 |         - |          NA |
| DotnetSort         | 256  | Reversed           |       135,897.0 ns |     2,963.2 ns |   1,315.69 ns |  1.24 |    0.01 |    3 |         - |          NA |
|      |                    |                    |                |               |       |         |      |           |             |
| **QuickSort**          | **256**  | **PipeOrgan**          |     **1,110,176.3 ns** |    **24,593.2 ns** |  **12,862.71 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 256  | PipeOrgan          |       216,746.1 ns |     1,715.1 ns |     897.01 ns |  0.20 |    0.00 |    1 |         - |          NA |
| QuickSortMedian3   | 256  | PipeOrgan          |       251,146.0 ns |     1,623.9 ns |     849.34 ns |  0.23 |    0.00 |    1 |         - |          NA |
| QuickSortMedian9   | 256  | PipeOrgan          |       161,474.2 ns |     2,084.2 ns |   1,090.07 ns |  0.15 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort | 256  | PipeOrgan          |       169,681.4 ns |       419.4 ns |     219.34 ns |  0.15 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 256  | PipeOrgan          |       255,403.0 ns |    10,890.1 ns |   5,695.71 ns |  0.23 |    0.01 |    1 |         - |          NA |
| IntroSort          | 256  | PipeOrgan          |       151,027.7 ns |     1,535.0 ns |     802.84 ns |  0.14 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | PipeOrgan          |       259,416.2 ns |     1,590.3 ns |     831.77 ns |  0.23 |    0.00 |    1 |         - |          NA |
| PDQSort            | 256  | PipeOrgan          |       203,019.8 ns |     1,853.0 ns |     969.15 ns |  0.18 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | PipeOrgan          |       204,536.9 ns |     2,657.3 ns |   1,179.88 ns |  0.18 |    0.00 |    1 |         - |          NA |
| StdSort            | 256  | PipeOrgan          |       288,649.4 ns |     1,710.3 ns |     759.37 ns |  0.26 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 256  | PipeOrgan          |       255,011.5 ns |     1,071.2 ns |     475.61 ns |  0.23 |    0.00 |    1 |         - |          NA |
| DotnetSort         | 256  | PipeOrgan          |       264,194.7 ns |     6,926.5 ns |   3,622.68 ns |  0.24 |    0.00 |    1 |         - |          NA |
|      |                    |                    |                |               |       |         |      |           |             |
| **QuickSort**          | **1024** | **Random**             |     **1,026,624.5 ns** |     **9,375.3 ns** |   **4,903.44 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| QuickSort3way      | 1024 | Random             |       889,684.2 ns |     7,384.3 ns |   3,278.66 ns |  0.87 |    0.00 |    1 |         - |          NA |
| QuickSortMedian3   | 1024 | Random             |       898,590.4 ns |     7,707.8 ns |   4,031.31 ns |  0.88 |    0.01 |    1 |         - |          NA |
| QuickSortMedian9   | 1024 | Random             |       836,079.5 ns |     3,893.1 ns |   1,728.55 ns |  0.81 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort | 1024 | Random             |       859,740.7 ns |     3,806.6 ns |   1,990.95 ns |  0.84 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 1024 | Random             |     1,708,489.9 ns |    11,947.4 ns |   6,248.72 ns |  1.66 |    0.01 |    2 |         - |          NA |
| IntroSort          | 1024 | Random             |       905,039.9 ns |     7,507.7 ns |   3,333.45 ns |  0.88 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 1024 | Random             |       916,235.3 ns |     2,756.2 ns |   1,223.78 ns |  0.89 |    0.00 |    1 |         - |          NA |
| PDQSort            | 1024 | Random             |     1,039,418.7 ns |     3,474.9 ns |   1,817.41 ns |  1.01 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 1024 | Random             |     1,016,437.5 ns |     5,871.2 ns |   3,070.75 ns |  0.99 |    0.01 |    1 |         - |          NA |
| StdSort            | 1024 | Random             |       950,352.1 ns |     2,341.4 ns |   1,039.59 ns |  0.93 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 1024 | Random             |       947,113.1 ns |    12,105.8 ns |   5,375.04 ns |  0.92 |    0.01 |    1 |         - |          NA |
| DotnetSort         | 1024 | Random             |       939,387.6 ns |     8,896.2 ns |   4,652.87 ns |  0.92 |    0.01 |    1 |         - |          NA |
|      |                    |                    |                |               |       |         |      |           |             |
| **QuickSort**          | **1024** | **SingleElementMoved** |       **642,859.0 ns** |     **2,101.9 ns** |     **933.26 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 1024 | SingleElementMoved |     1,360,839.9 ns |    10,285.8 ns |   5,379.68 ns |  2.12 |    0.01 |    4 |         - |          NA |
| QuickSortMedian3   | 1024 | SingleElementMoved |     1,159,513.2 ns |     7,834.1 ns |   2,793.71 ns |  1.80 |    0.00 |    4 |         - |          NA |
| QuickSortMedian9   | 1024 | SingleElementMoved |       879,277.1 ns |     8,297.6 ns |   4,339.83 ns |  1.37 |    0.01 |    3 |         - |          NA |
| DualPivotQuickSort | 1024 | SingleElementMoved |     1,890,984.1 ns |     4,988.0 ns |   2,214.72 ns |  2.94 |    0.01 |    5 |         - |          NA |
| StableQuickSort    | 1024 | SingleElementMoved |     1,217,459.7 ns |     6,028.6 ns |   3,153.08 ns |  1.89 |    0.01 |    4 |         - |          NA |
| IntroSort          | 1024 | SingleElementMoved |       458,832.8 ns |     2,301.6 ns |   1,203.80 ns |  0.71 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 1024 | SingleElementMoved |       629,683.4 ns |     2,540.1 ns |   1,127.80 ns |  0.98 |    0.00 |    2 |         - |          NA |
| PDQSort            | 1024 | SingleElementMoved |       554,521.6 ns |     3,936.1 ns |   1,747.64 ns |  0.86 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless  | 1024 | SingleElementMoved |       562,600.1 ns |     2,402.9 ns |   1,256.75 ns |  0.88 |    0.00 |    2 |         - |          NA |
| StdSort            | 1024 | SingleElementMoved |       588,241.4 ns |     2,081.6 ns |   1,088.73 ns |  0.92 |    0.00 |    2 |         - |          NA |
| BlockQuickSort     | 1024 | SingleElementMoved |       561,788.6 ns |     4,725.5 ns |   2,471.52 ns |  0.87 |    0.00 |    2 |         - |          NA |
| DotnetSort         | 1024 | SingleElementMoved |       868,731.5 ns |     4,704.3 ns |   2,088.75 ns |  1.35 |    0.00 |    3 |         - |          NA |
|      |                    |                    |                |               |       |         |      |           |             |
| **QuickSort**          | **1024** | **Sorted**             |       **594,070.7 ns** |     **2,242.8 ns** |     **995.80 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 1024 | Sorted             |     1,907,643.8 ns |     4,931.2 ns |   2,189.49 ns |  3.21 |    0.01 |    5 |         - |          NA |
| QuickSortMedian3   | 1024 | Sorted             |     1,641,355.0 ns |    13,111.6 ns |   6,857.64 ns |  2.76 |    0.01 |    5 |         - |          NA |
| QuickSortMedian9   | 1024 | Sorted             |       882,084.3 ns |     3,754.4 ns |   1,963.60 ns |  1.48 |    0.00 |    3 |         - |          NA |
| DualPivotQuickSort | 1024 | Sorted             |     2,211,218.4 ns |    11,582.7 ns |   5,142.80 ns |  3.72 |    0.01 |    5 |         - |          NA |
| StableQuickSort    | 1024 | Sorted             |     1,255,649.3 ns |     2,563.7 ns |   1,340.85 ns |  2.11 |    0.00 |    4 |         - |          NA |
| IntroSort          | 1024 | Sorted             |       138,437.2 ns |     1,430.5 ns |     748.18 ns |  0.23 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 1024 | Sorted             |       491,054.5 ns |     2,154.1 ns |   1,126.64 ns |  0.83 |    0.00 |    2 |         - |          NA |
| PDQSort            | 1024 | Sorted             |       141,603.2 ns |       501.9 ns |     222.87 ns |  0.24 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 1024 | Sorted             |       143,870.8 ns |       617.5 ns |     274.16 ns |  0.24 |    0.00 |    1 |         - |          NA |
| StdSort            | 1024 | Sorted             |       161,407.1 ns |       560.1 ns |     248.69 ns |  0.27 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 1024 | Sorted             |       545,618.6 ns |     4,795.1 ns |   2,129.05 ns |  0.92 |    0.00 |    2 |         - |          NA |
| DotnetSort         | 1024 | Sorted             |       456,414.2 ns |     2,538.1 ns |   1,126.95 ns |  0.77 |    0.00 |    2 |         - |          NA |
|      |                    |                    |                |               |       |         |      |           |             |
| **QuickSort**          | **1024** | **Reversed**           |       **572,725.9 ns** |     **1,468.9 ns** |     **652.19 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way      | 1024 | Reversed           |     1,337,295.9 ns |    14,152.6 ns |   6,283.83 ns |  2.33 |    0.01 |    5 |         - |          NA |
| QuickSortMedian3   | 1024 | Reversed           |     1,971,373.8 ns |    11,869.2 ns |   6,207.84 ns |  3.44 |    0.01 |    6 |         - |          NA |
| QuickSortMedian9   | 1024 | Reversed           |       884,048.4 ns |     4,986.8 ns |   2,608.20 ns |  1.54 |    0.00 |    4 |         - |          NA |
| DualPivotQuickSort | 1024 | Reversed           |     1,856,372.0 ns |     6,656.8 ns |   3,481.64 ns |  3.24 |    0.01 |    6 |         - |          NA |
| StableQuickSort    | 1024 | Reversed           |     1,271,745.0 ns |    13,451.4 ns |   5,972.49 ns |  2.22 |    0.01 |    5 |         - |          NA |
| IntroSort          | 1024 | Reversed           |       366,974.4 ns |     1,288.3 ns |     673.82 ns |  0.64 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet    | 1024 | Reversed           |       814,301.1 ns |     8,181.8 ns |   4,279.26 ns |  1.42 |    0.01 |    4 |         - |          NA |
| PDQSort            | 1024 | Reversed           |       208,628.2 ns |       929.2 ns |     485.97 ns |  0.36 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 1024 | Reversed           |       210,733.2 ns |     1,172.4 ns |     613.17 ns |  0.37 |    0.00 |    1 |         - |          NA |
| StdSort            | 1024 | Reversed           |       210,750.3 ns |     1,941.4 ns |     861.98 ns |  0.37 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 1024 | Reversed           |       549,353.3 ns |     1,157.2 ns |     513.79 ns |  0.96 |    0.00 |    3 |         - |          NA |
| DotnetSort         | 1024 | Reversed           |       796,375.7 ns |    13,666.1 ns |   7,147.63 ns |  1.39 |    0.01 |    4 |         - |          NA |
|      |                    |                    |                |               |       |         |      |           |             |
| **QuickSort**          | **1024** | **PipeOrgan**          |    **17,143,445.8 ns** |    **72,276.2 ns** |  **37,801.86 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way      | 1024 | PipeOrgan          |     1,342,332.6 ns |     8,046.3 ns |   3,572.60 ns |  0.08 |    0.00 |    2 |         - |          NA |
| QuickSortMedian3   | 1024 | PipeOrgan          |     1,493,365.4 ns |    13,661.4 ns |   6,065.76 ns |  0.09 |    0.00 |    2 |         - |          NA |
| QuickSortMedian9   | 1024 | PipeOrgan          |       845,654.4 ns |     6,282.8 ns |   3,286.02 ns |  0.05 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort | 1024 | PipeOrgan          |       857,374.3 ns |     3,515.3 ns |   1,838.57 ns |  0.05 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 1024 | PipeOrgan          |     1,362,307.5 ns |    17,903.6 ns |   7,949.33 ns |  0.08 |    0.00 |    2 |         - |          NA |
| IntroSort          | 1024 | PipeOrgan          |     1,278,131.1 ns |     8,457.1 ns |   3,755.01 ns |  0.07 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet    | 1024 | PipeOrgan          |     1,599,290.1 ns |    14,561.4 ns |   6,465.34 ns |  0.09 |    0.00 |    2 |         - |          NA |
| PDQSort            | 1024 | PipeOrgan          |     1,111,692.5 ns |     4,853.7 ns |   2,538.58 ns |  0.06 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless  | 1024 | PipeOrgan          |     1,037,754.4 ns |     5,829.2 ns |   2,588.21 ns |  0.06 |    0.00 |    2 |         - |          NA |
| StdSort            | 1024 | PipeOrgan          |     1,520,376.6 ns |    10,035.8 ns |   4,455.96 ns |  0.09 |    0.00 |    2 |         - |          NA |
| BlockQuickSort     | 1024 | PipeOrgan          |     1,302,731.6 ns |     6,104.4 ns |   3,192.73 ns |  0.08 |    0.00 |    2 |         - |          NA |
| DotnetSort         | 1024 | PipeOrgan          |     1,626,007.5 ns |    13,994.9 ns |   6,213.81 ns |  0.09 |    0.00 |    2 |         - |          NA |
|      |                    |                    |                |               |       |         |      |           |             |
| **QuickSort**          | **8192** | **Random**             |    **10,605,497.5 ns** |    **63,607.9 ns** |  **33,268.16 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| QuickSort3way      | 8192 | Random             |     9,346,756.7 ns |    61,955.5 ns |  27,508.62 ns |  0.88 |    0.00 |    1 |         - |          NA |
| QuickSortMedian3   | 8192 | Random             |     9,466,502.3 ns |    34,996.7 ns |  15,538.73 ns |  0.89 |    0.00 |    1 |         - |          NA |
| QuickSortMedian9   | 8192 | Random             |     9,059,816.7 ns |    29,564.2 ns |  13,126.69 ns |  0.85 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort | 8192 | Random             |     9,063,875.7 ns |    21,376.2 ns |   9,491.17 ns |  0.85 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 8192 | Random             |    17,652,904.2 ns |    76,799.6 ns |  40,167.68 ns |  1.66 |    0.01 |    2 |         - |          NA |
| IntroSort          | 8192 | Random             |    10,133,682.8 ns |    53,260.2 ns |  27,856.11 ns |  0.96 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 8192 | Random             |     8,873,441.9 ns |    33,042.5 ns |  14,671.08 ns |  0.84 |    0.00 |    1 |         - |          NA |
| PDQSort            | 8192 | Random             |    10,682,407.3 ns |    25,845.9 ns |  13,517.92 ns |  1.01 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | Random             |    10,670,992.7 ns |    28,594.5 ns |  14,955.48 ns |  1.01 |    0.00 |    1 |         - |          NA |
| StdSort            | 8192 | Random             |     9,227,597.6 ns |    17,849.1 ns |   9,335.42 ns |  0.87 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 8192 | Random             |     9,031,163.8 ns |    19,230.6 ns |   6,857.82 ns |  0.85 |    0.00 |    1 |         - |          NA |
| DotnetSort         | 8192 | Random             |     9,067,877.8 ns |    47,760.1 ns |  24,979.45 ns |  0.86 |    0.00 |    1 |         - |          NA |
|      |                    |                    |                |               |       |         |      |           |             |
| **QuickSort**          | **8192** | **SingleElementMoved** |     **6,747,698.5 ns** |    **14,874.6 ns** |   **6,604.42 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| QuickSort3way      | 8192 | SingleElementMoved |    28,556,038.7 ns |   213,478.5 ns | 111,653.38 ns |  4.23 |    0.02 |    4 |         - |          NA |
| QuickSortMedian3   | 8192 | SingleElementMoved |    22,305,019.9 ns |    75,498.9 ns |  39,487.40 ns |  3.31 |    0.01 |    3 |         - |          NA |
| QuickSortMedian9   | 8192 | SingleElementMoved |     8,893,706.6 ns |    30,549.8 ns |  15,978.15 ns |  1.32 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort | 8192 | SingleElementMoved |    12,289,722.3 ns |    19,793.9 ns |   8,788.61 ns |  1.82 |    0.00 |    2 |         - |          NA |
| StableQuickSort    | 8192 | SingleElementMoved |    13,267,548.1 ns |    23,518.2 ns |  12,300.48 ns |  1.97 |    0.00 |    2 |         - |          NA |
| IntroSort          | 8192 | SingleElementMoved |     4,617,596.4 ns |    12,502.3 ns |   6,538.97 ns |  0.68 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 8192 | SingleElementMoved |     7,434,944.1 ns |    59,000.2 ns |  26,196.46 ns |  1.10 |    0.00 |    1 |         - |          NA |
| PDQSort            | 8192 | SingleElementMoved |     5,369,566.5 ns |    11,302.7 ns |   5,911.56 ns |  0.80 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | SingleElementMoved |     5,473,679.2 ns |    15,887.8 ns |   8,309.63 ns |  0.81 |    0.00 |    1 |         - |          NA |
| StdSort            | 8192 | SingleElementMoved |     5,414,500.8 ns |     7,239.3 ns |   3,786.29 ns |  0.80 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 8192 | SingleElementMoved |     6,165,404.7 ns |    13,809.2 ns |   7,222.49 ns |  0.91 |    0.00 |    1 |         - |          NA |
| DotnetSort         | 8192 | SingleElementMoved |     7,495,930.4 ns |    18,248.5 ns |   8,102.44 ns |  1.11 |    0.00 |    1 |         - |          NA |
|      |                    |                    |                |               |       |         |      |           |             |
| **QuickSort**          | **8192** | **Sorted**             |     **6,470,465.9 ns** |    **30,625.1 ns** |  **16,017.53 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 8192 | Sorted             |    42,038,881.4 ns |   132,952.6 ns |  69,536.76 ns |  6.50 |    0.02 |    5 |         - |          NA |
| QuickSortMedian3   | 8192 | Sorted             |    35,379,969.3 ns |    35,191.8 ns |  15,625.38 ns |  5.47 |    0.01 |    5 |         - |          NA |
| QuickSortMedian9   | 8192 | Sorted             |     8,600,242.4 ns |    31,411.4 ns |  13,946.87 ns |  1.33 |    0.00 |    3 |         - |          NA |
| DualPivotQuickSort | 8192 | Sorted             |    15,616,258.6 ns |    22,520.4 ns |   9,999.22 ns |  2.41 |    0.01 |    4 |         - |          NA |
| StableQuickSort    | 8192 | Sorted             |    13,487,717.7 ns |    25,300.1 ns |  13,232.45 ns |  2.08 |    0.01 |    4 |         - |          NA |
| IntroSort          | 8192 | Sorted             |     1,123,468.2 ns |     2,797.3 ns |   1,463.05 ns |  0.17 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 8192 | Sorted             |     5,580,089.9 ns |    21,312.6 ns |   9,462.91 ns |  0.86 |    0.00 |    2 |         - |          NA |
| PDQSort            | 8192 | Sorted             |     1,175,730.7 ns |     3,674.7 ns |   1,921.95 ns |  0.18 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | Sorted             |     1,202,604.3 ns |     4,780.5 ns |   2,500.29 ns |  0.19 |    0.00 |    1 |         - |          NA |
| StdSort            | 8192 | Sorted             |     1,301,884.3 ns |     2,692.1 ns |   1,195.31 ns |  0.20 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 8192 | Sorted             |     5,858,154.6 ns |    22,955.0 ns |  10,192.18 ns |  0.91 |    0.00 |    2 |         - |          NA |
| DotnetSort         | 8192 | Sorted             |     5,241,302.9 ns |    16,776.3 ns |   7,448.80 ns |  0.81 |    0.00 |    2 |         - |          NA |
|      |                    |                    |                |               |       |         |      |           |             |
| **QuickSort**          | **8192** | **Reversed**           |     **6,126,637.4 ns** |     **5,192.3 ns** |   **2,305.40 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way      | 8192 | Reversed           |    28,107,560.3 ns |    46,242.9 ns |  20,532.16 ns |  4.59 |    0.00 |    7 |         - |          NA |
| QuickSortMedian3   | 8192 | Reversed           |    44,757,553.0 ns |   101,599.9 ns |  53,138.72 ns |  7.31 |    0.01 |    8 |         - |          NA |
| QuickSortMedian9   | 8192 | Reversed           |     8,458,876.1 ns |    15,783.1 ns |   7,007.78 ns |  1.38 |    0.00 |    5 |         - |          NA |
| DualPivotQuickSort | 8192 | Reversed           |    13,300,846.0 ns |    26,825.0 ns |  11,910.46 ns |  2.17 |    0.00 |    6 |         - |          NA |
| StableQuickSort    | 8192 | Reversed           |    13,673,762.2 ns |    52,584.6 ns |  27,502.78 ns |  2.23 |    0.00 |    6 |         - |          NA |
| IntroSort          | 8192 | Reversed           |     3,735,363.0 ns |     6,701.3 ns |   3,504.89 ns |  0.61 |    0.00 |    3 |         - |          NA |
| IntroSortDotnet    | 8192 | Reversed           |     9,684,560.4 ns |    41,601.7 ns |  18,471.40 ns |  1.58 |    0.00 |    5 |         - |          NA |
| PDQSort            | 8192 | Reversed           |     2,315,362.2 ns |    10,836.2 ns |   5,667.54 ns |  0.38 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless  | 8192 | Reversed           |     1,729,580.7 ns |     9,392.2 ns |   4,912.29 ns |  0.28 |    0.00 |    1 |         - |          NA |
| StdSort            | 8192 | Reversed           |     1,747,747.4 ns |    23,303.5 ns |  10,346.90 ns |  0.29 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 8192 | Reversed           |     5,689,292.7 ns |     9,479.6 ns |   4,958.03 ns |  0.93 |    0.00 |    4 |         - |          NA |
| DotnetSort         | 8192 | Reversed           |     9,489,994.7 ns |    61,829.6 ns |  32,338.09 ns |  1.55 |    0.01 |    5 |         - |          NA |
|      |                    |                    |                |               |       |         |      |           |             |
| **QuickSort**          | **8192** | **PipeOrgan**          | **1,083,233,517.3 ns** | **1,945,775.4 ns** | **863,936.35 ns** | **1.000** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way      | 8192 | PipeOrgan          |    18,282,306.6 ns |   720,990.2 ns | 377,091.80 ns | 0.017 |    0.00 |    3 |         - |          NA |
| QuickSortMedian3   | 8192 | PipeOrgan          |    18,808,399.2 ns |    56,632.1 ns |  29,619.68 ns | 0.017 |    0.00 |    3 |         - |          NA |
| QuickSortMedian9   | 8192 | PipeOrgan          |     8,786,348.0 ns |    12,585.5 ns |   5,588.03 ns | 0.008 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort | 8192 | PipeOrgan          |     8,608,766.7 ns |     9,667.7 ns |   5,056.41 ns | 0.008 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 8192 | PipeOrgan          |    13,961,319.9 ns |    39,714.6 ns |  20,771.50 ns | 0.013 |    0.00 |    2 |         - |          NA |
| IntroSort          | 8192 | PipeOrgan          |    20,112,597.7 ns |    50,671.8 ns |  26,502.33 ns | 0.019 |    0.00 |    3 |         - |          NA |
| IntroSortDotnet    | 8192 | PipeOrgan          |    21,755,577.7 ns |    71,586.7 ns |  31,784.93 ns | 0.020 |    0.00 |    3 |         - |          NA |
| PDQSort            | 8192 | PipeOrgan          |    11,432,116.0 ns |    38,421.0 ns |  20,094.94 ns | 0.011 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless  | 8192 | PipeOrgan          |    11,267,256.4 ns |    49,833.9 ns |  22,126.57 ns | 0.010 |    0.00 |    2 |         - |          NA |
| StdSort            | 8192 | PipeOrgan          |    19,693,666.6 ns |    24,044.9 ns |  10,676.07 ns | 0.018 |    0.00 |    3 |         - |          NA |
| BlockQuickSort     | 8192 | PipeOrgan          |    12,416,068.4 ns |    76,370.2 ns |  33,908.82 ns | 0.011 |    0.00 |    2 |         - |          NA |
| DotnetSort         | 8192 | PipeOrgan          |    21,625,771.9 ns |   103,407.8 ns |  45,913.72 ns | 0.020 |    0.00 |    3 |         - |          NA |

### TreeBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                 | Size | Pattern            | Mean           | Error       | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------- |----- |------------------- |---------------:|------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **BalancedBinaryTreeSort** | **256**  | **Random**             |    **12,774.8 ns** |   **569.04 ns** |   **252.66 ns** |  **3.66** |    **0.15** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 256  | Random             |     3,492.7 ns |   276.48 ns |   144.61 ns |  1.00 |    0.05 |    1 |         - |          NA |
| SplaySort              | 256  | Random             |    23,163.2 ns |   320.84 ns |   142.46 ns |  6.64 |    0.25 |    4 |         - |          NA |
| TreapSort              | 256  | Random             |     8,973.6 ns |   348.69 ns |   182.37 ns |  2.57 |    0.11 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **SingleElementMoved** |    **14,828.2 ns** |   **471.02 ns** |   **209.14 ns** |  **0.29** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 256  | SingleElementMoved |    50,673.8 ns |   430.65 ns |   225.24 ns |  1.00 |    0.01 |    4 |         - |          NA |
| SplaySort              | 256  | SingleElementMoved |     4,138.7 ns |    19.63 ns |     8.72 ns |  0.08 |    0.00 |    1 |         - |          NA |
| TreapSort              | 256  | SingleElementMoved |     6,168.3 ns |   391.32 ns |   173.75 ns |  0.12 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Sorted**             |    **14,083.6 ns** | **1,105.59 ns** |   **578.24 ns** |  **0.19** |    **0.01** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 256  | Sorted             |    76,019.7 ns |   379.59 ns |   168.54 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 256  | Sorted             |     3,694.7 ns |    22.14 ns |     9.83 ns |  0.05 |    0.00 |    1 |         - |          NA |
| TreapSort              | 256  | Sorted             |     5,013.3 ns |   322.08 ns |   143.00 ns |  0.07 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Reversed**           |    **12,211.5 ns** |   **506.30 ns** |   **264.80 ns** |  **0.16** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 256  | Reversed           |    74,112.8 ns |   333.93 ns |   148.27 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 256  | Reversed           |     3,546.1 ns |    24.27 ns |     8.65 ns |  0.05 |    0.00 |    1 |         - |          NA |
| TreapSort              | 256  | Reversed           |     5,425.4 ns |   312.40 ns |   163.39 ns |  0.07 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **PipeOrgan**          |    **12,519.5 ns** | **1,020.45 ns** |   **533.71 ns** |  **0.33** |    **0.01** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 256  | PipeOrgan          |    38,266.4 ns |   268.67 ns |   140.52 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 256  | PipeOrgan          |     4,351.1 ns |    28.71 ns |    10.24 ns |  0.11 |    0.00 |    1 |         - |          NA |
| TreapSort              | 256  | PipeOrgan          |     7,400.6 ns |   472.86 ns |   247.32 ns |  0.19 |    0.01 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Random**             |   **126,232.7 ns** | **3,759.16 ns** | **1,966.11 ns** |  **6.28** |    **0.11** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 1024 | Random             |    20,113.2 ns |   399.01 ns |   208.69 ns |  1.00 |    0.01 |    1 |         - |          NA |
| SplaySort              | 1024 | Random             |   158,589.9 ns | 3,340.86 ns | 1,747.33 ns |  7.89 |    0.11 |    4 |         - |          NA |
| TreapSort              | 1024 | Random             |    39,922.0 ns | 1,233.83 ns |   547.83 ns |  1.99 |    0.03 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **SingleElementMoved** |   **106,556.8 ns** | **2,476.36 ns** | **1,295.18 ns** |  **0.14** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 1024 | SingleElementMoved |   780,974.7 ns |   990.87 ns |   439.95 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | SingleElementMoved |    16,528.4 ns |   291.53 ns |   152.48 ns |  0.02 |    0.00 |    1 |         - |          NA |
| TreapSort              | 1024 | SingleElementMoved |    28,699.6 ns |   627.46 ns |   278.60 ns |  0.04 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Sorted**             |    **99,503.5 ns** | **1,719.28 ns** |   **899.22 ns** |  **0.08** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 1024 | Sorted             | 1,191,656.8 ns | 1,324.58 ns |   692.78 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | Sorted             |    14,837.7 ns |   261.66 ns |   136.85 ns |  0.01 |    0.00 |    1 |         - |          NA |
| TreapSort              | 1024 | Sorted             |    23,751.2 ns |   245.48 ns |   128.39 ns |  0.02 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Reversed**           |    **60,081.1 ns** |   **575.86 ns** |   **301.18 ns** |  **0.05** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 1024 | Reversed           | 1,153,714.4 ns | 1,931.65 ns | 1,010.29 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | Reversed           |    14,207.2 ns |   307.57 ns |   160.87 ns |  0.01 |    0.00 |    1 |         - |          NA |
| TreapSort              | 1024 | Reversed           |    23,246.6 ns |   210.87 ns |   110.29 ns |  0.02 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **PipeOrgan**          |    **91,193.7 ns** | **2,648.24 ns** | **1,385.08 ns** |  **0.16** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 1024 | PipeOrgan          |   583,573.9 ns |   929.76 ns |   486.28 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | PipeOrgan          |    17,268.5 ns |   197.03 ns |   103.05 ns |  0.03 |    0.00 |    1 |         - |          NA |
| TreapSort              | 1024 | PipeOrgan          |    34,182.4 ns |   938.83 ns |   491.03 ns |  0.06 |    0.00 |    2 |         - |          NA |

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
