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
<summary>Benchmark results (2026-07-14 15:26 UTC)</summary>

Workflow run: https://github.com/guitarrapc/SortAlgorithms/actions/runs/29342864381

### AdaptiveBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.78GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method        | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **DropMergeSort** | **256**  | **Random**             |     **4,961.4 ns** |    **466.34 ns** |    **243.90 ns** |  **1.00** |    **0.06** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | Random             |     4,928.8 ns |    605.64 ns |    268.91 ns |  1.00 |    0.07 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **256**  | **SingleElementMoved** |     **1,125.9 ns** |    **618.38 ns** |    **323.43 ns** |  **1.07** |    **0.40** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | SingleElementMoved |     8,119.5 ns |  1,598.73 ns |    709.85 ns |  7.71 |    2.02 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **256**  | **Sorted**             |       **533.8 ns** |      **3.11 ns** |      **1.11 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | Sorted             |     7,614.9 ns |    253.01 ns |    132.33 ns | 14.27 |    0.24 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **256**  | **Reversed**           |     **8,087.6 ns** |    **292.98 ns** |    **153.23 ns** |  **1.00** |    **0.03** |    **2** |         **-** |          **NA** |
| PatienceSort  | 256  | Reversed           |     1,695.5 ns |    124.07 ns |     55.09 ns |  0.21 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **256**  | **PipeOrgan**          |     **7,825.1 ns** |    **121.75 ns** |     **43.42 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| PatienceSort  | 256  | PipeOrgan          |     5,330.7 ns |    326.82 ns |    170.93 ns |  0.68 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **Random**             |    **31,569.9 ns** |    **723.43 ns** |    **321.21 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| PatienceSort  | 1024 | Random             |    22,645.0 ns |    594.30 ns |    263.87 ns |  0.72 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **SingleElementMoved** |     **2,184.7 ns** |      **3.75 ns** |      **1.34 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | SingleElementMoved |    40,562.3 ns |  1,245.08 ns |    651.20 ns | 18.57 |    0.28 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **Sorted**             |     **2,098.6 ns** |    **341.11 ns** |    **178.41 ns** |  **1.01** |    **0.11** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | Sorted             |    43,211.7 ns |  7,302.51 ns |  3,819.36 ns | 20.71 |    2.33 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **Reversed**           |    **53,316.4 ns** |    **652.85 ns** |    **289.87 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| PatienceSort  | 1024 | Reversed           |     5,998.7 ns |    261.52 ns |    136.78 ns |  0.11 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **PipeOrgan**          |    **39,636.1 ns** |    **362.33 ns** |    **129.21 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PatienceSort  | 1024 | PipeOrgan          |    25,782.7 ns |    660.59 ns |    293.30 ns |  0.65 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **Random**             |   **538,137.3 ns** |  **2,532.23 ns** |  **1,324.40 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | Random             |   718,241.9 ns |  2,379.23 ns |  1,244.38 ns |  1.33 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **SingleElementMoved** |    **17,974.8 ns** |  **1,550.27 ns** |    **810.82 ns** |  **1.00** |    **0.06** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | SingleElementMoved |   743,096.4 ns | 10,905.61 ns |  4,842.16 ns | 41.41 |    1.74 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **Sorted**             |    **15,651.8 ns** |    **219.01 ns** |     **97.24 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | Sorted             |   739,574.6 ns | 25,349.22 ns | 13,258.13 ns | 47.25 |    0.85 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **Reversed**           | **1,124,798.8 ns** |  **7,337.20 ns** |  **3,837.50 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PatienceSort  | 8192 | Reversed           |    57,863.1 ns | 26,468.53 ns | 13,843.55 ns |  0.05 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **PipeOrgan**          |   **537,433.5 ns** |  **7,930.44 ns** |  **3,521.16 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | PipeOrgan          |   575,608.6 ns |  7,335.77 ns |  3,257.13 ns |  1.07 |    0.01 |    1 |         - |          NA |

### AdaptiveSlowBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.78GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method     | Size | Pattern            | Mean         | Error       | StdDev    | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------- |----- |------------------- |-------------:|------------:|----------:|------:|--------:|-----:|----------:|------------:|
| **StrandSort** | **256**  | **Random**             |   **7,737.6 ns** |   **870.80 ns** | **455.44 ns** |  **1.00** |    **0.08** |    **1** |         **-** |          **NA** |
|      |                    |              |             |           |       |         |      |           |             |
| **StrandSort** | **256**  | **SingleElementMoved** |     **819.1 ns** |     **6.16 ns** |   **2.73 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |           |       |         |      |           |             |
| **StrandSort** | **256**  | **Sorted**             |     **541.8 ns** |     **6.90 ns** |   **3.61 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |             |           |       |         |      |           |             |
| **StrandSort** | **256**  | **Reversed**           |  **52,107.5 ns** |   **132.36 ns** |  **47.20 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |           |       |         |      |           |             |
| **StrandSort** | **256**  | **PipeOrgan**          |  **27,617.6 ns** |   **301.89 ns** | **157.90 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |             |           |       |         |      |           |             |
| **StrandSort** | **1024** | **Random**             |  **56,342.5 ns** |   **729.89 ns** | **381.75 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |             |           |       |         |      |           |             |
| **StrandSort** | **1024** | **SingleElementMoved** |   **2,832.0 ns** |   **429.76 ns** | **224.77 ns** |  **1.01** |    **0.11** |    **1** |         **-** |          **NA** |
|      |                    |              |             |           |       |         |      |           |             |
| **StrandSort** | **1024** | **Sorted**             |   **1,586.0 ns** |    **13.12 ns** |   **4.68 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |           |       |         |      |           |             |
| **StrandSort** | **1024** | **Reversed**           | **773,096.6 ns** | **2,118.80 ns** | **940.76 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |           |       |         |      |           |             |
| **StrandSort** | **1024** | **PipeOrgan**          | **395,612.5 ns** | **1,600.80 ns** | **837.25 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |

### DistributionBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.78GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method              | Size | Pattern            | Mean         | Error        | StdDev      | Median       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------------- |----- |------------------- |-------------:|-------------:|------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **CountingSort**        | **256**  | **Random**             |   **1,928.4 ns** |     **39.93 ns** |    **17.73 ns** |   **1,926.9 ns** |  **1.88** |    **0.02** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | Random             |   1,025.3 ns |      9.60 ns |     5.02 ns |   1,026.2 ns |  1.00 |    0.01 |    1 |         - |          NA |
| PigeonSort          | 256  | Random             |   1,544.8 ns |     18.27 ns |     8.11 ns |   1,546.8 ns |  1.51 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | Random             |   1,002.8 ns |      9.88 ns |     5.17 ns |   1,004.4 ns |  0.98 |    0.01 |    1 |         - |          NA |
| BucketSort          | 256  | Random             |   2,982.9 ns |     46.12 ns |    16.45 ns |   2,980.8 ns |  2.91 |    0.02 |    3 |         - |          NA |
| BucketSortInteger   | 256  | Random             |   2,900.9 ns |     38.34 ns |    13.67 ns |   2,897.0 ns |  2.83 |    0.02 |    3 |         - |          NA |
| FlashSort           | 256  | Random             |   4,405.9 ns |    457.50 ns |   203.13 ns |   4,272.9 ns |  4.30 |    0.19 |    4 |         - |          NA |
| RadixLSD4Sort       | 256  | Random             |   5,683.4 ns |     21.22 ns |    11.10 ns |   5,684.5 ns |  5.54 |    0.03 |    5 |         - |          NA |
| RadixLSD256Sort     | 256  | Random             |   2,820.2 ns |    322.99 ns |   143.41 ns |   2,775.2 ns |  2.75 |    0.13 |    3 |         - |          NA |
| RadixLSD10Sort      | 256  | Random             |   4,397.9 ns |    464.36 ns |   242.87 ns |   4,234.5 ns |  4.29 |    0.22 |    4 |         - |          NA |
| RadixMSD4Sort       | 256  | Random             |  12,398.2 ns |    225.61 ns |   100.17 ns |  12,354.0 ns | 12.09 |    0.11 |    6 |         - |          NA |
| RadixMSD10Sort      | 256  | Random             |  13,864.0 ns |    343.61 ns |   179.71 ns |  13,895.8 ns | 13.52 |    0.18 |    6 |         - |          NA |
| AmericanFlagSort    | 256  | Random             |   5,518.6 ns |    211.71 ns |   110.73 ns |   5,480.0 ns |  5.38 |    0.10 |    5 |         - |          NA |
| SpreadSort          | 256  | Random             |   1,708.9 ns |     56.47 ns |    25.07 ns |   1,695.7 ns |  1.67 |    0.02 |    2 |         - |          NA |
|      |                    |              |              |             |              |       |         |      |           |             |
| **CountingSort**        | **256**  | **SingleElementMoved** |   **1,613.1 ns** |     **12.37 ns** |     **5.49 ns** |   **1,614.4 ns** |  **1.67** |    **0.01** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | SingleElementMoved |     968.2 ns |      3.19 ns |     1.42 ns |     967.7 ns |  1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 256  | SingleElementMoved |   1,587.5 ns |      6.31 ns |     2.80 ns |   1,587.2 ns |  1.64 |    0.00 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | SingleElementMoved |     972.8 ns |      7.62 ns |     3.98 ns |     972.7 ns |  1.00 |    0.00 |    1 |         - |          NA |
| BucketSort          | 256  | SingleElementMoved |   2,490.5 ns |     14.07 ns |     6.25 ns |   2,488.9 ns |  2.57 |    0.01 |    4 |         - |          NA |
| BucketSortInteger   | 256  | SingleElementMoved |   1,954.3 ns |     12.16 ns |     4.34 ns |   1,952.7 ns |  2.02 |    0.00 |    3 |         - |          NA |
| FlashSort           | 256  | SingleElementMoved |   4,934.7 ns |     37.66 ns |    13.43 ns |   4,932.0 ns |  5.10 |    0.01 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | SingleElementMoved |   5,789.4 ns |    322.01 ns |   168.42 ns |   5,690.1 ns |  5.98 |    0.16 |    5 |         - |          NA |
| RadixLSD256Sort     | 256  | SingleElementMoved |   2,668.3 ns |    243.48 ns |   108.10 ns |   2,720.4 ns |  2.76 |    0.10 |    4 |         - |          NA |
| RadixLSD10Sort      | 256  | SingleElementMoved |   4,219.3 ns |    381.50 ns |   199.53 ns |   4,168.0 ns |  4.36 |    0.19 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | SingleElementMoved |  11,450.8 ns |    356.83 ns |   186.63 ns |  11,496.9 ns | 11.83 |    0.18 |    6 |         - |          NA |
| RadixMSD10Sort      | 256  | SingleElementMoved |  13,962.6 ns |    309.60 ns |   137.46 ns |  13,989.7 ns | 14.42 |    0.13 |    7 |         - |          NA |
| AmericanFlagSort    | 256  | SingleElementMoved |   4,483.5 ns |    407.08 ns |   212.91 ns |   4,336.5 ns |  4.63 |    0.21 |    5 |         - |          NA |
| SpreadSort          | 256  | SingleElementMoved |   1,105.8 ns |     20.70 ns |     9.19 ns |   1,106.8 ns |  1.14 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |             |              |       |         |      |           |             |
| **CountingSort**        | **256**  | **Sorted**             |   **1,558.8 ns** |     **10.55 ns** |     **4.68 ns** |   **1,557.9 ns** |  **1.70** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | Sorted             |     915.5 ns |     17.25 ns |     6.15 ns |     913.1 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PigeonSort          | 256  | Sorted             |   1,409.8 ns |      7.60 ns |     3.97 ns |   1,409.8 ns |  1.54 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | Sorted             |     969.4 ns |     17.31 ns |     7.69 ns |     968.5 ns |  1.06 |    0.01 |    2 |         - |          NA |
| BucketSort          | 256  | Sorted             |   2,492.8 ns |    238.87 ns |   106.06 ns |   2,462.4 ns |  2.72 |    0.11 |    5 |         - |          NA |
| BucketSortInteger   | 256  | Sorted             |   1,969.5 ns |    230.82 ns |   120.72 ns |   1,887.6 ns |  2.15 |    0.13 |    4 |         - |          NA |
| FlashSort           | 256  | Sorted             |   4,973.5 ns |    453.28 ns |   237.07 ns |   4,821.9 ns |  5.43 |    0.25 |    6 |         - |          NA |
| RadixLSD4Sort       | 256  | Sorted             |   5,793.1 ns |    346.60 ns |   181.28 ns |   5,769.4 ns |  6.33 |    0.19 |    6 |         - |          NA |
| RadixLSD256Sort     | 256  | Sorted             |   2,408.0 ns |     25.92 ns |     9.24 ns |   2,412.1 ns |  2.63 |    0.02 |    5 |         - |          NA |
| RadixLSD10Sort      | 256  | Sorted             |   3,995.2 ns |     71.59 ns |    25.53 ns |   3,992.8 ns |  4.36 |    0.04 |    6 |         - |          NA |
| RadixMSD4Sort       | 256  | Sorted             |  11,345.0 ns |    396.11 ns |   207.17 ns |  11,335.8 ns | 12.39 |    0.23 |    7 |         - |          NA |
| RadixMSD10Sort      | 256  | Sorted             |  13,440.0 ns |    653.63 ns |   290.22 ns |  13,371.4 ns | 14.68 |    0.31 |    7 |         - |          NA |
| AmericanFlagSort    | 256  | Sorted             |   4,269.3 ns |      8.24 ns |     2.94 ns |   4,268.6 ns |  4.66 |    0.03 |    6 |         - |          NA |
| SpreadSort          | 256  | Sorted             |     356.6 ns |     58.64 ns |    26.04 ns |     340.9 ns |  0.39 |    0.03 |    1 |         - |          NA |
|      |                    |              |              |             |              |       |         |      |           |             |
| **CountingSort**        | **256**  | **Reversed**           |   **1,543.7 ns** |     **11.25 ns** |     **5.88 ns** |   **1,543.5 ns** |  **1.67** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | Reversed           |     922.4 ns |     10.80 ns |     4.79 ns |     920.5 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PigeonSort          | 256  | Reversed           |   1,428.3 ns |     11.62 ns |     5.16 ns |   1,427.3 ns |  1.55 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | Reversed           |   1,336.4 ns |     13.47 ns |     5.98 ns |   1,338.7 ns |  1.45 |    0.01 |    3 |         - |          NA |
| BucketSort          | 256  | Reversed           |   3,215.0 ns |     55.74 ns |    19.88 ns |   3,211.2 ns |  3.49 |    0.03 |    4 |         - |          NA |
| BucketSortInteger   | 256  | Reversed           |   3,073.3 ns |     11.80 ns |     4.21 ns |   3,072.3 ns |  3.33 |    0.02 |    4 |         - |          NA |
| FlashSort           | 256  | Reversed           |   4,560.8 ns |    418.11 ns |   218.68 ns |   4,554.3 ns |  4.94 |    0.23 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | Reversed           |   5,872.7 ns |    380.58 ns |   199.05 ns |   5,843.1 ns |  6.37 |    0.21 |    6 |         - |          NA |
| RadixLSD256Sort     | 256  | Reversed           |   2,650.7 ns |     45.35 ns |    16.17 ns |   2,655.2 ns |  2.87 |    0.02 |    4 |         - |          NA |
| RadixLSD10Sort      | 256  | Reversed           |   4,285.6 ns |    394.68 ns |   206.42 ns |   4,146.8 ns |  4.65 |    0.21 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | Reversed           |  12,120.2 ns |    434.78 ns |   193.04 ns |  12,138.3 ns | 13.14 |    0.21 |    7 |         - |          NA |
| RadixMSD10Sort      | 256  | Reversed           |  13,741.6 ns |    287.54 ns |   127.67 ns |  13,772.6 ns | 14.90 |    0.15 |    7 |         - |          NA |
| AmericanFlagSort    | 256  | Reversed           |   5,623.5 ns |    548.04 ns |   286.64 ns |   5,556.8 ns |  6.10 |    0.29 |    6 |         - |          NA |
| SpreadSort          | 256  | Reversed           |     528.9 ns |      4.34 ns |     2.27 ns |     528.6 ns |  0.57 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |              |       |         |      |           |             |
| **CountingSort**        | **256**  | **PipeOrgan**          |   **1,504.8 ns** |     **10.13 ns** |     **4.50 ns** |   **1,504.0 ns** |  **1.75** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | PipeOrgan          |     861.9 ns |     10.44 ns |     4.64 ns |     861.4 ns |  1.00 |    0.01 |    1 |         - |          NA |
| PigeonSort          | 256  | PipeOrgan          |   1,818.8 ns |    651.12 ns |   340.55 ns |   1,751.8 ns |  2.11 |    0.37 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | PipeOrgan          |   1,225.5 ns |    486.52 ns |   216.02 ns |   1,216.4 ns |  1.42 |    0.23 |    2 |         - |          NA |
| BucketSort          | 256  | PipeOrgan          |   2,951.3 ns |     56.43 ns |    20.12 ns |   2,952.0 ns |  3.42 |    0.03 |    4 |         - |          NA |
| BucketSortInteger   | 256  | PipeOrgan          |   2,526.9 ns |     22.99 ns |    10.21 ns |   2,527.2 ns |  2.93 |    0.02 |    4 |         - |          NA |
| FlashSort           | 256  | PipeOrgan          |   4,512.7 ns |     19.44 ns |     6.93 ns |   4,512.4 ns |  5.24 |    0.03 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | PipeOrgan          |   6,048.3 ns |    392.77 ns |   205.42 ns |   6,101.0 ns |  7.02 |    0.23 |    6 |         - |          NA |
| RadixLSD256Sort     | 256  | PipeOrgan          |   2,799.1 ns |    426.93 ns |   223.29 ns |   2,693.8 ns |  3.25 |    0.25 |    4 |         - |          NA |
| RadixLSD10Sort      | 256  | PipeOrgan          |   4,128.7 ns |    305.83 ns |   159.96 ns |   4,027.1 ns |  4.79 |    0.18 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | PipeOrgan          |  13,265.1 ns |    568.33 ns |   297.25 ns |  13,329.9 ns | 15.39 |    0.33 |    7 |         - |          NA |
| RadixMSD10Sort      | 256  | PipeOrgan          |  13,875.4 ns |    101.16 ns |    52.91 ns |  13,891.3 ns | 16.10 |    0.10 |    7 |         - |          NA |
| AmericanFlagSort    | 256  | PipeOrgan          |   6,312.9 ns |    695.42 ns |   363.72 ns |   6,401.8 ns |  7.32 |    0.40 |    6 |         - |          NA |
| SpreadSort          | 256  | PipeOrgan          |   1,673.5 ns |     70.50 ns |    31.30 ns |   1,663.7 ns |  1.94 |    0.04 |    3 |         - |          NA |
|      |                    |              |              |             |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **Random**             |   **6,421.3 ns** |    **333.49 ns** |   **148.07 ns** |   **6,441.0 ns** |  **1.63** |    **0.07** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Random             |   3,933.7 ns |    280.17 ns |   146.54 ns |   3,831.7 ns |  1.00 |    0.05 |    1 |         - |          NA |
| PigeonSort          | 1024 | Random             |   5,688.5 ns |     14.40 ns |     7.53 ns |   5,685.9 ns |  1.45 |    0.05 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | Random             |   3,786.7 ns |    479.04 ns |   250.55 ns |   3,630.3 ns |  0.96 |    0.07 |    1 |         - |          NA |
| BucketSort          | 1024 | Random             |  14,972.9 ns |    694.20 ns |   363.08 ns |  14,951.5 ns |  3.81 |    0.16 |    4 |         - |          NA |
| BucketSortInteger   | 1024 | Random             |  14,943.4 ns |    192.38 ns |    85.42 ns |  14,984.6 ns |  3.80 |    0.13 |    4 |         - |          NA |
| FlashSort           | 1024 | Random             |  17,895.8 ns |    211.95 ns |   110.85 ns |  17,896.9 ns |  4.55 |    0.16 |    4 |         - |          NA |
| RadixLSD4Sort       | 1024 | Random             |  24,825.3 ns |    214.13 ns |   111.99 ns |  24,842.3 ns |  6.32 |    0.22 |    5 |         - |          NA |
| RadixLSD256Sort     | 1024 | Random             |  10,273.5 ns |    319.03 ns |   166.86 ns |  10,319.2 ns |  2.61 |    0.10 |    3 |         - |          NA |
| RadixLSD10Sort      | 1024 | Random             |  22,751.5 ns |    317.58 ns |   141.01 ns |  22,738.2 ns |  5.79 |    0.20 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | Random             |  48,512.0 ns |    261.62 ns |   116.16 ns |  48,537.0 ns | 12.35 |    0.43 |    6 |         - |          NA |
| RadixMSD10Sort      | 1024 | Random             |  50,631.6 ns |    403.03 ns |   143.72 ns |  50,667.2 ns | 12.89 |    0.45 |    6 |         - |          NA |
| AmericanFlagSort    | 1024 | Random             |  24,120.9 ns |    278.68 ns |   145.76 ns |  24,148.3 ns |  6.14 |    0.21 |    5 |         - |          NA |
| SpreadSort          | 1024 | Random             |   9,271.9 ns |    471.11 ns |   209.17 ns |   9,228.2 ns |  2.36 |    0.10 |    3 |         - |          NA |
|      |                    |              |              |             |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **SingleElementMoved** |   **6,023.1 ns** |    **296.02 ns** |   **131.43 ns** |   **5,929.5 ns** |  **1.68** |    **0.04** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | SingleElementMoved |   3,576.9 ns |     39.12 ns |    17.37 ns |   3,573.4 ns |  1.00 |    0.01 |    1 |         - |          NA |
| PigeonSort          | 1024 | SingleElementMoved |   5,779.0 ns |    504.10 ns |   223.82 ns |   5,820.5 ns |  1.62 |    0.06 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | SingleElementMoved |   3,579.9 ns |    309.84 ns |   162.05 ns |   3,487.6 ns |  1.00 |    0.04 |    1 |         - |          NA |
| BucketSort          | 1024 | SingleElementMoved |   9,604.4 ns |    446.93 ns |   233.75 ns |   9,634.9 ns |  2.69 |    0.06 |    2 |         - |          NA |
| BucketSortInteger   | 1024 | SingleElementMoved |   8,024.3 ns |    331.97 ns |   173.63 ns |   8,080.6 ns |  2.24 |    0.05 |    2 |         - |          NA |
| FlashSort           | 1024 | SingleElementMoved |  19,763.0 ns |    583.46 ns |   305.16 ns |  19,761.1 ns |  5.53 |    0.08 |    3 |         - |          NA |
| RadixLSD4Sort       | 1024 | SingleElementMoved |  24,818.9 ns |    365.48 ns |   162.27 ns |  24,811.9 ns |  6.94 |    0.05 |    3 |         - |          NA |
| RadixLSD256Sort     | 1024 | SingleElementMoved |  10,129.2 ns |    336.47 ns |   175.98 ns |  10,188.8 ns |  2.83 |    0.05 |    2 |         - |          NA |
| RadixLSD10Sort      | 1024 | SingleElementMoved |  21,008.3 ns |    335.21 ns |   175.32 ns |  20,993.1 ns |  5.87 |    0.05 |    3 |         - |          NA |
| RadixMSD4Sort       | 1024 | SingleElementMoved |  44,136.9 ns |    392.30 ns |   205.18 ns |  44,132.9 ns | 12.34 |    0.08 |    4 |         - |          NA |
| RadixMSD10Sort      | 1024 | SingleElementMoved |  47,965.2 ns |    320.51 ns |   142.31 ns |  47,910.0 ns | 13.41 |    0.07 |    4 |         - |          NA |
| AmericanFlagSort    | 1024 | SingleElementMoved |  17,001.7 ns |     68.13 ns |    30.25 ns |  16,990.3 ns |  4.75 |    0.02 |    3 |         - |          NA |
| SpreadSort          | 1024 | SingleElementMoved |   6,871.9 ns |    296.68 ns |   155.17 ns |   6,860.9 ns |  1.92 |    0.04 |    2 |         - |          NA |
|      |                    |              |              |             |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **Sorted**             |   **5,886.7 ns** |    **353.23 ns** |   **184.75 ns** |   **5,865.9 ns** |  **1.73** |    **0.05** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Sorted             |   3,409.1 ns |      3.34 ns |     1.19 ns |   3,409.4 ns |  1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | Sorted             |   5,267.3 ns |    453.70 ns |   237.29 ns |   5,259.4 ns |  1.55 |    0.07 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | Sorted             |   3,427.2 ns |     18.56 ns |     8.24 ns |   3,424.0 ns |  1.01 |    0.00 |    2 |         - |          NA |
| BucketSort          | 1024 | Sorted             |   9,337.0 ns |    335.70 ns |   175.58 ns |   9,284.4 ns |  2.74 |    0.05 |    5 |         - |          NA |
| BucketSortInteger   | 1024 | Sorted             |   7,472.3 ns |    388.53 ns |   203.21 ns |   7,386.2 ns |  2.19 |    0.06 |    4 |         - |          NA |
| FlashSort           | 1024 | Sorted             |  18,820.8 ns |    295.71 ns |   154.66 ns |  18,834.8 ns |  5.52 |    0.04 |    6 |         - |          NA |
| RadixLSD4Sort       | 1024 | Sorted             |  24,008.5 ns |    152.35 ns |    67.64 ns |  24,020.1 ns |  7.04 |    0.02 |    6 |         - |          NA |
| RadixLSD256Sort     | 1024 | Sorted             |   9,944.0 ns |    339.39 ns |   177.51 ns |   9,971.1 ns |  2.92 |    0.05 |    5 |         - |          NA |
| RadixLSD10Sort      | 1024 | Sorted             |  20,696.4 ns |    160.88 ns |    84.15 ns |  20,704.7 ns |  6.07 |    0.02 |    6 |         - |          NA |
| RadixMSD4Sort       | 1024 | Sorted             |  43,752.5 ns |    382.95 ns |   200.29 ns |  43,795.4 ns | 12.83 |    0.06 |    7 |         - |          NA |
| RadixMSD10Sort      | 1024 | Sorted             |  47,471.8 ns |    171.16 ns |    89.52 ns |  47,470.1 ns | 13.92 |    0.03 |    7 |         - |          NA |
| AmericanFlagSort    | 1024 | Sorted             |  16,695.1 ns |     88.56 ns |    39.32 ns |  16,687.9 ns |  4.90 |    0.01 |    6 |         - |          NA |
| SpreadSort          | 1024 | Sorted             |     697.0 ns |      4.38 ns |     1.56 ns |     697.0 ns |  0.20 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **Reversed**           |   **5,927.8 ns** |    **464.98 ns** |   **243.19 ns** |   **5,889.9 ns** |  **1.72** |    **0.07** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Reversed           |   3,455.5 ns |     14.06 ns |     5.02 ns |   3,455.2 ns |  1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 1024 | Reversed           |   5,561.8 ns |    139.17 ns |    49.63 ns |   5,550.6 ns |  1.61 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | Reversed           |   3,089.2 ns |     16.99 ns |     6.06 ns |   3,087.2 ns |  0.89 |    0.00 |    1 |         - |          NA |
| BucketSort          | 1024 | Reversed           |  16,303.4 ns |    209.29 ns |   109.46 ns |  16,282.7 ns |  4.72 |    0.03 |    4 |         - |          NA |
| BucketSortInteger   | 1024 | Reversed           |  17,085.8 ns |    171.66 ns |    76.22 ns |  17,073.8 ns |  4.94 |    0.02 |    4 |         - |          NA |
| FlashSort           | 1024 | Reversed           |  21,730.0 ns | 12,867.65 ns | 6,730.03 ns |  17,346.6 ns |  6.29 |    1.84 |    5 |         - |          NA |
| RadixLSD4Sort       | 1024 | Reversed           |  24,279.5 ns |    314.56 ns |   139.67 ns |  24,275.9 ns |  7.03 |    0.04 |    6 |         - |          NA |
| RadixLSD256Sort     | 1024 | Reversed           |  10,009.5 ns |    361.56 ns |   189.10 ns |  10,051.4 ns |  2.90 |    0.05 |    3 |         - |          NA |
| RadixLSD10Sort      | 1024 | Reversed           |  20,766.4 ns |    663.94 ns |   347.25 ns |  20,704.2 ns |  6.01 |    0.10 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | Reversed           |  47,312.2 ns |    194.80 ns |   101.89 ns |  47,305.3 ns | 13.69 |    0.03 |    7 |         - |          NA |
| RadixMSD10Sort      | 1024 | Reversed           |  50,009.5 ns |  1,098.44 ns |   574.51 ns |  50,144.0 ns | 14.47 |    0.16 |    7 |         - |          NA |
| AmericanFlagSort    | 1024 | Reversed           |  23,753.7 ns |    941.39 ns |   492.37 ns |  23,752.7 ns |  6.87 |    0.14 |    6 |         - |          NA |
| SpreadSort          | 1024 | Reversed           |   5,546.0 ns |    301.27 ns |   157.57 ns |   5,454.7 ns |  1.60 |    0.04 |    2 |         - |          NA |
|      |                    |              |              |             |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **PipeOrgan**          |   **5,928.5 ns** |    **846.32 ns** |   **375.77 ns** |   **5,865.4 ns** |  **1.75** |    **0.13** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | PipeOrgan          |   3,393.6 ns |    343.49 ns |   179.65 ns |   3,289.0 ns |  1.00 |    0.07 |    1 |         - |          NA |
| PigeonSort          | 1024 | PipeOrgan          |   5,214.4 ns |    324.29 ns |   169.61 ns |   5,108.7 ns |  1.54 |    0.09 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | PipeOrgan          |   3,270.2 ns |      6.28 ns |     2.79 ns |   3,269.8 ns |  0.97 |    0.05 |    1 |         - |          NA |
| BucketSort          | 1024 | PipeOrgan          |  14,172.2 ns |     96.32 ns |    42.77 ns |  14,175.6 ns |  4.19 |    0.20 |    4 |         - |          NA |
| BucketSortInteger   | 1024 | PipeOrgan          |  12,195.9 ns |    249.62 ns |   110.83 ns |  12,241.5 ns |  3.60 |    0.17 |    4 |         - |          NA |
| FlashSort           | 1024 | PipeOrgan          |  17,859.5 ns |    434.32 ns |   227.16 ns |  17,824.1 ns |  5.27 |    0.26 |    5 |         - |          NA |
| RadixLSD4Sort       | 1024 | PipeOrgan          |  24,842.8 ns |    160.75 ns |    84.07 ns |  24,862.7 ns |  7.34 |    0.35 |    6 |         - |          NA |
| RadixLSD256Sort     | 1024 | PipeOrgan          |  10,147.1 ns |    466.31 ns |   207.04 ns |  10,277.5 ns |  3.00 |    0.15 |    4 |         - |          NA |
| RadixLSD10Sort      | 1024 | PipeOrgan          |  20,608.4 ns |    199.35 ns |    88.51 ns |  20,607.4 ns |  6.09 |    0.29 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | PipeOrgan          |  50,514.1 ns |    141.40 ns |    73.95 ns |  50,525.7 ns | 14.92 |    0.71 |    7 |         - |          NA |
| RadixMSD10Sort      | 1024 | PipeOrgan          |  50,349.0 ns |    445.18 ns |   232.84 ns |  50,339.6 ns | 14.87 |    0.71 |    7 |         - |          NA |
| AmericanFlagSort    | 1024 | PipeOrgan          |  26,090.1 ns |    250.36 ns |   130.94 ns |  26,036.5 ns |  7.71 |    0.37 |    6 |         - |          NA |
| SpreadSort          | 1024 | PipeOrgan          |   7,269.9 ns |     52.34 ns |    23.24 ns |   7,279.8 ns |  2.15 |    0.10 |    3 |         - |          NA |
|      |                    |              |              |             |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **Random**             |  **54,002.7 ns** |  **1,356.28 ns** |   **602.19 ns** |  **53,811.2 ns** |  **1.59** |    **0.02** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Random             |  33,958.6 ns |    197.28 ns |    70.35 ns |  33,962.9 ns |  1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 8192 | Random             |  47,291.9 ns |    730.48 ns |   382.05 ns |  47,051.3 ns |  1.39 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | Random             |  30,114.2 ns |    628.05 ns |   278.86 ns |  30,041.7 ns |  0.89 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | Random             | 222,910.5 ns |  6,607.40 ns | 3,455.80 ns | 222,224.8 ns |  6.56 |    0.10 |    5 |         - |          NA |
| BucketSortInteger   | 8192 | Random             | 246,572.4 ns |  1,514.16 ns |   672.30 ns | 246,634.6 ns |  7.26 |    0.02 |    5 |         - |          NA |
| FlashSort           | 8192 | Random             | 155,071.9 ns |  1,476.27 ns |   655.47 ns | 155,109.0 ns |  4.57 |    0.02 |    4 |         - |          NA |
| RadixLSD4Sort       | 8192 | Random             | 230,968.4 ns |    584.06 ns |   305.48 ns | 230,902.3 ns |  6.80 |    0.02 |    5 |         - |          NA |
| RadixLSD256Sort     | 8192 | Random             |  58,660.2 ns |  6,255.08 ns | 3,271.53 ns |  58,011.8 ns |  1.73 |    0.09 |    2 |         - |          NA |
| RadixLSD10Sort      | 8192 | Random             | 173,881.2 ns |  1,923.06 ns | 1,005.80 ns | 173,604.6 ns |  5.12 |    0.03 |    4 |         - |          NA |
| RadixMSD4Sort       | 8192 | Random             | 458,960.5 ns |    733.56 ns |   325.70 ns | 458,927.9 ns | 13.52 |    0.03 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | Random             | 432,012.1 ns |  2,756.09 ns | 1,441.49 ns | 431,964.8 ns | 12.72 |    0.05 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | Random             | 293,390.6 ns |  1,412.96 ns |   627.36 ns | 293,424.5 ns |  8.64 |    0.02 |    5 |         - |          NA |
| SpreadSort          | 8192 | Random             |  84,843.9 ns |    415.26 ns |   184.38 ns |  84,786.9 ns |  2.50 |    0.01 |    3 |         - |          NA |
|      |                    |              |              |             |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **SingleElementMoved** |  **46,900.0 ns** |    **727.33 ns** |   **380.41 ns** |  **46,756.5 ns** |  **1.58** |    **0.03** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | SingleElementMoved |  29,776.1 ns |  1,149.44 ns |   601.18 ns |  29,553.2 ns |  1.00 |    0.03 |    1 |         - |          NA |
| PigeonSort          | 8192 | SingleElementMoved |  44,763.4 ns |    779.87 ns |   407.89 ns |  44,711.1 ns |  1.50 |    0.03 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | SingleElementMoved |  28,438.8 ns |  2,291.32 ns | 1,017.36 ns |  28,143.0 ns |  0.96 |    0.04 |    1 |         - |          NA |
| BucketSort          | 8192 | SingleElementMoved |  67,320.5 ns |  1,089.39 ns |   483.70 ns |  67,213.3 ns |  2.26 |    0.05 |    3 |         - |          NA |
| BucketSortInteger   | 8192 | SingleElementMoved |  49,929.9 ns |    725.66 ns |   322.20 ns |  50,008.8 ns |  1.68 |    0.03 |    2 |         - |          NA |
| FlashSort           | 8192 | SingleElementMoved | 157,950.4 ns |    523.57 ns |   273.84 ns | 157,893.1 ns |  5.31 |    0.10 |    4 |         - |          NA |
| RadixLSD4Sort       | 8192 | SingleElementMoved | 236,666.2 ns |    994.95 ns |   520.38 ns | 236,563.5 ns |  7.95 |    0.15 |    5 |         - |          NA |
| RadixLSD256Sort     | 8192 | SingleElementMoved |  54,108.6 ns |  1,026.75 ns |   455.88 ns |  53,870.2 ns |  1.82 |    0.04 |    2 |         - |          NA |
| RadixLSD10Sort      | 8192 | SingleElementMoved | 167,964.1 ns |  2,968.71 ns | 1,318.13 ns | 168,147.7 ns |  5.64 |    0.11 |    4 |         - |          NA |
| RadixMSD4Sort       | 8192 | SingleElementMoved | 344,365.4 ns |    304.95 ns |   159.50 ns | 344,311.3 ns | 11.57 |    0.22 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | SingleElementMoved | 384,953.1 ns | 11,884.23 ns | 6,215.68 ns | 383,227.7 ns | 12.93 |    0.31 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | SingleElementMoved | 147,193.1 ns |    636.78 ns |   282.74 ns | 147,286.0 ns |  4.95 |    0.09 |    4 |         - |          NA |
| SpreadSort          | 8192 | SingleElementMoved |  46,805.7 ns |  1,198.77 ns |   532.26 ns |  46,941.6 ns |  1.57 |    0.03 |    2 |         - |          NA |
|      |                    |              |              |             |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **Sorted**             |  **46,914.7 ns** |  **1,528.59 ns** |   **799.48 ns** |  **46,790.7 ns** |  **1.71** |    **0.04** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Sorted             |  27,384.4 ns |  1,065.84 ns |   473.24 ns |  27,110.9 ns |  1.00 |    0.02 |    2 |         - |          NA |
| PigeonSort          | 8192 | Sorted             |  43,028.9 ns |  2,061.32 ns | 1,078.11 ns |  43,092.0 ns |  1.57 |    0.04 |    3 |         - |          NA |
| PigeonSortInteger   | 8192 | Sorted             |  27,806.5 ns |    537.06 ns |   238.46 ns |  27,899.4 ns |  1.02 |    0.02 |    2 |         - |          NA |
| BucketSort          | 8192 | Sorted             |  66,607.0 ns |    952.77 ns |   423.03 ns |  66,643.7 ns |  2.43 |    0.04 |    4 |         - |          NA |
| BucketSortInteger   | 8192 | Sorted             |  47,603.5 ns |  1,210.62 ns |   633.18 ns |  47,212.6 ns |  1.74 |    0.04 |    3 |         - |          NA |
| FlashSort           | 8192 | Sorted             | 152,270.6 ns |    228.74 ns |    81.57 ns | 152,261.5 ns |  5.56 |    0.09 |    5 |         - |          NA |
| RadixLSD4Sort       | 8192 | Sorted             | 235,335.9 ns |    683.99 ns |   357.74 ns | 235,274.6 ns |  8.60 |    0.14 |    6 |         - |          NA |
| RadixLSD256Sort     | 8192 | Sorted             |  53,525.3 ns |  1,298.52 ns |   679.15 ns |  53,482.8 ns |  1.96 |    0.04 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | Sorted             | 167,719.3 ns |  8,916.62 ns | 4,663.57 ns | 168,185.5 ns |  6.13 |    0.19 |    5 |         - |          NA |
| RadixMSD4Sort       | 8192 | Sorted             | 345,748.8 ns |  1,950.39 ns | 1,020.09 ns | 345,458.5 ns | 12.63 |    0.20 |    7 |         - |          NA |
| RadixMSD10Sort      | 8192 | Sorted             | 383,955.7 ns | 14,102.76 ns | 7,376.02 ns | 381,232.4 ns | 14.02 |    0.34 |    7 |         - |          NA |
| AmericanFlagSort    | 8192 | Sorted             | 144,554.9 ns |    414.65 ns |   184.11 ns | 144,495.0 ns |  5.28 |    0.08 |    5 |         - |          NA |
| SpreadSort          | 8192 | Sorted             |   5,327.3 ns |    513.39 ns |   268.51 ns |   5,159.3 ns |  0.19 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |             |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **Reversed**           |  **45,757.5 ns** |    **395.71 ns** |   **175.70 ns** |  **45,725.8 ns** |  **1.63** |    **0.02** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Reversed           |  28,097.5 ns |    665.80 ns |   295.62 ns |  27,956.2 ns |  1.00 |    0.01 |    1 |         - |          NA |
| PigeonSort          | 8192 | Reversed           |  42,070.4 ns |    659.64 ns |   292.89 ns |  42,096.7 ns |  1.50 |    0.02 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | Reversed           |  25,244.9 ns |    911.39 ns |   404.66 ns |  25,170.0 ns |  0.90 |    0.02 |    1 |         - |          NA |
| BucketSort          | 8192 | Reversed           | 259,106.8 ns |    943.93 ns |   419.11 ns | 259,086.6 ns |  9.22 |    0.09 |    5 |         - |          NA |
| BucketSortInteger   | 8192 | Reversed           | 309,250.0 ns |  1,698.52 ns |   754.15 ns | 309,305.7 ns | 11.01 |    0.11 |    5 |         - |          NA |
| FlashSort           | 8192 | Reversed           | 132,890.8 ns |  1,226.67 ns |   544.65 ns | 132,875.6 ns |  4.73 |    0.05 |    3 |         - |          NA |
| RadixLSD4Sort       | 8192 | Reversed           | 239,504.3 ns |  1,306.07 ns |   683.10 ns | 239,104.9 ns |  8.52 |    0.09 |    5 |         - |          NA |
| RadixLSD256Sort     | 8192 | Reversed           |  53,322.6 ns |  1,260.17 ns |   659.09 ns |  53,061.1 ns |  1.90 |    0.03 |    2 |         - |          NA |
| RadixLSD10Sort      | 8192 | Reversed           | 168,933.9 ns |  6,494.05 ns | 3,396.51 ns | 168,692.5 ns |  6.01 |    0.13 |    4 |         - |          NA |
| RadixMSD4Sort       | 8192 | Reversed           | 375,432.9 ns |    310.81 ns |   110.84 ns | 375,423.9 ns | 13.36 |    0.13 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | Reversed           | 393,971.1 ns |  2,088.27 ns |   927.21 ns | 394,090.4 ns | 14.02 |    0.14 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | Reversed           | 208,669.9 ns |  4,670.91 ns | 2,442.98 ns | 208,742.4 ns |  7.43 |    0.11 |    5 |         - |          NA |
| SpreadSort          | 8192 | Reversed           |  62,175.0 ns |    646.89 ns |   338.34 ns |  62,109.7 ns |  2.21 |    0.02 |    2 |         - |          NA |
|      |                    |              |              |             |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **PipeOrgan**          |  **46,562.6 ns** |  **1,847.09 ns** |   **966.07 ns** |  **46,323.1 ns** |  **1.50** |    **0.03** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | PipeOrgan          |  31,100.8 ns |    896.39 ns |   398.00 ns |  30,977.4 ns |  1.00 |    0.02 |    1 |         - |          NA |
| PigeonSort          | 8192 | PipeOrgan          |  41,122.8 ns |  1,045.64 ns |   464.27 ns |  41,044.5 ns |  1.32 |    0.02 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | PipeOrgan          |  26,439.1 ns |    167.13 ns |    74.21 ns |  26,476.7 ns |  0.85 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | PipeOrgan          | 205,480.7 ns |  1,233.27 ns |   645.03 ns | 205,247.6 ns |  6.61 |    0.08 |    5 |         - |          NA |
| BucketSortInteger   | 8192 | PipeOrgan          | 192,334.2 ns |  2,723.50 ns | 1,209.25 ns | 192,649.5 ns |  6.19 |    0.08 |    5 |         - |          NA |
| FlashSort           | 8192 | PipeOrgan          | 139,747.8 ns |  1,261.32 ns |   560.03 ns | 139,516.7 ns |  4.49 |    0.06 |    4 |         - |          NA |
| RadixLSD4Sort       | 8192 | PipeOrgan          | 244,405.2 ns | 13,441.05 ns | 5,967.91 ns | 242,001.1 ns |  7.86 |    0.20 |    5 |         - |          NA |
| RadixLSD256Sort     | 8192 | PipeOrgan          |  79,221.3 ns |  1,156.72 ns |   604.99 ns |  79,228.7 ns |  2.55 |    0.04 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | PipeOrgan          | 167,294.2 ns |  5,047.82 ns | 2,640.11 ns | 168,556.3 ns |  5.38 |    0.10 |    5 |         - |          NA |
| RadixMSD4Sort       | 8192 | PipeOrgan          | 411,050.5 ns | 11,155.72 ns | 4,953.21 ns | 410,334.5 ns | 13.22 |    0.22 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | PipeOrgan          | 405,817.0 ns | 21,612.31 ns | 9,596.00 ns | 403,513.4 ns | 13.05 |    0.33 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | PipeOrgan          | 230,970.8 ns |  1,523.05 ns |   796.59 ns | 231,238.5 ns |  7.43 |    0.09 |    5 |         - |          NA |
| SpreadSort          | 8192 | PipeOrgan          |  81,994.6 ns |  1,450.52 ns |   644.04 ns |  81,789.0 ns |  2.64 |    0.04 |    3 |         - |          NA |

### ExchangeBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.78GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method             | Size | Pattern            | Mean         | Error        | StdDev       | Ratio  | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------- |----- |------------------- |-------------:|-------------:|-------------:|-------:|--------:|-----:|----------:|------------:|
| **BubbleSort**         | **256**  | **Random**             |  **32,904.8 ns** |    **543.33 ns** |    **241.24 ns** |   **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Random             |  16,329.4 ns |    326.37 ns |    170.70 ns |   0.50 |    0.01 |    2 |         - |          NA |
| OddEvenSort        | 256  | Random             |  26,154.3 ns |    422.87 ns |    221.17 ns |   0.79 |    0.01 |    3 |         - |          NA |
| CombSort           | 256  | Random             |   3,704.4 ns |    364.72 ns |    190.76 ns |   0.11 |    0.01 |    1 |         - |          NA |
| CircleSort         | 256  | Random             |  17,753.1 ns |    418.18 ns |    149.13 ns |   0.54 |    0.01 |    2 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **BubbleSort**         | **256**  | **SingleElementMoved** |     **404.5 ns** |      **1.11 ns** |      **0.49 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 256  | SingleElementMoved |     308.3 ns |      2.08 ns |      1.09 ns |   0.76 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 256  | SingleElementMoved |  15,707.4 ns |    258.38 ns |    114.72 ns |  38.83 |    0.27 |    4 |         - |          NA |
| CombSort           | 256  | SingleElementMoved |   2,812.2 ns |     12.13 ns |      5.39 ns |   6.95 |    0.01 |    3 |         - |          NA |
| CircleSort         | 256  | SingleElementMoved |  15,174.2 ns |    142.03 ns |     74.28 ns |  37.51 |    0.18 |    4 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **BubbleSort**         | **256**  | **Sorted**             |     **245.0 ns** |      **0.77 ns** |      **0.34 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Sorted             |     168.6 ns |      0.47 ns |      0.25 ns |   0.69 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 256  | Sorted             |     213.4 ns |      1.02 ns |      0.53 ns |   0.87 |    0.00 |    2 |         - |          NA |
| CombSort           | 256  | Sorted             |   2,480.2 ns |      1.81 ns |      0.65 ns |  10.12 |    0.01 |    3 |         - |          NA |
| CircleSort         | 256  | Sorted             |   2,087.2 ns |      5.55 ns |      2.47 ns |   8.52 |    0.01 |    3 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **BubbleSort**         | **256**  | **Reversed**           |  **27,362.0 ns** |    **134.24 ns** |     **70.21 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Reversed           |  25,278.8 ns |    274.19 ns |    143.41 ns |   0.92 |    0.01 |    3 |         - |          NA |
| OddEvenSort        | 256  | Reversed           |  24,280.4 ns |    174.89 ns |     91.47 ns |   0.89 |    0.00 |    3 |         - |          NA |
| CombSort           | 256  | Reversed           |   3,142.9 ns |     28.90 ns |     10.31 ns |   0.11 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | Reversed           |   4,161.3 ns |     21.16 ns |      7.55 ns |   0.15 |    0.00 |    2 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **BubbleSort**         | **256**  | **PipeOrgan**          |  **26,444.4 ns** |    **311.37 ns** |    **138.25 ns** |   **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | PipeOrgan          |  17,063.2 ns |    281.81 ns |    147.39 ns |   0.65 |    0.01 |    2 |         - |          NA |
| OddEvenSort        | 256  | PipeOrgan          |  25,723.7 ns |    608.64 ns |    318.33 ns |   0.97 |    0.01 |    3 |         - |          NA |
| CombSort           | 256  | PipeOrgan          |   3,063.1 ns |    207.62 ns |     92.18 ns |   0.12 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | PipeOrgan          |  19,389.3 ns |    270.90 ns |    141.68 ns |   0.73 |    0.01 |    2 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Random**             | **601,950.9 ns** |  **4,057.61 ns** |  **2,122.21 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Random             | 312,580.1 ns |  1,327.10 ns |    694.10 ns |   0.52 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | Random             | 529,807.1 ns |  1,411.40 ns |    626.67 ns |   0.88 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | Random             |  31,787.3 ns |    359.84 ns |    188.20 ns |   0.05 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | Random             |  97,376.6 ns |    311.81 ns |    111.20 ns |   0.16 |    0.00 |    2 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **BubbleSort**         | **1024** | **SingleElementMoved** |   **1,533.3 ns** |      **2.03 ns** |      **0.90 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | SingleElementMoved |   1,155.3 ns |     27.09 ns |     12.03 ns |   0.75 |    0.01 |    1 |         - |          NA |
| OddEvenSort        | 1024 | SingleElementMoved | 244,502.4 ns | 55,429.01 ns | 28,990.44 ns | 159.46 |   17.85 |    5 |         - |          NA |
| CombSort           | 1024 | SingleElementMoved |  14,848.1 ns |    122.88 ns |     64.27 ns |   9.68 |    0.04 |    3 |         - |          NA |
| CircleSort         | 1024 | SingleElementMoved |  97,181.8 ns |    705.83 ns |    369.16 ns |  63.38 |    0.23 |    4 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Sorted**             |     **956.0 ns** |      **1.14 ns** |      **0.51 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Sorted             |     890.9 ns |    394.04 ns |    206.09 ns |   0.93 |    0.20 |    2 |         - |          NA |
| OddEvenSort        | 1024 | Sorted             |     805.4 ns |      0.53 ns |      0.19 ns |   0.84 |    0.00 |    1 |         - |          NA |
| CombSort           | 1024 | Sorted             |  13,134.0 ns |    285.21 ns |    126.63 ns |  13.74 |    0.12 |    4 |         - |          NA |
| CircleSort         | 1024 | Sorted             |   9,200.0 ns |    381.77 ns |    169.51 ns |   9.62 |    0.17 |    3 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Reversed**           | **413,799.1 ns** |    **457.52 ns** |    **239.29 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Reversed           | 385,815.5 ns |  1,031.97 ns |    458.20 ns |   0.93 |    0.00 |    2 |         - |          NA |
| OddEvenSort        | 1024 | Reversed           | 367,938.6 ns |    993.54 ns |    441.14 ns |   0.89 |    0.00 |    2 |         - |          NA |
| CombSort           | 1024 | Reversed           |  16,683.2 ns |    375.15 ns |    196.21 ns |   0.04 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | Reversed           |  18,591.1 ns |    333.31 ns |    174.33 ns |   0.04 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **BubbleSort**         | **1024** | **PipeOrgan**          | **383,529.7 ns** |  **1,919.88 ns** |    **852.44 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | PipeOrgan          | 253,098.2 ns |    480.27 ns |    213.24 ns |   0.66 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | PipeOrgan          | 360,454.2 ns |  2,760.52 ns |  1,225.69 ns |   0.94 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | PipeOrgan          |  16,889.3 ns |    259.26 ns |    135.60 ns |   0.04 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | PipeOrgan          | 102,861.4 ns |    782.10 ns |    347.26 ns |   0.27 |    0.00 |    2 |         - |          NA |

### HeapBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.78GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method           | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **HeapSort**         | **256**  | **Random**             |     **3,774.9 ns** |    **109.28 ns** |     **48.52 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | Random             |     3,500.9 ns |    323.78 ns |    169.34 ns |  0.93 |    0.04 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | Random             |     4,207.7 ns |    446.23 ns |    233.39 ns |  1.11 |    0.06 |    1 |         - |          NA |
| BottomupHeapSort | 256  | Random             |     4,167.8 ns |    381.35 ns |    199.45 ns |  1.10 |    0.05 |    1 |         - |          NA |
| WeakHeapSort     | 256  | Random             |     9,191.9 ns |    425.73 ns |    222.66 ns |  2.44 |    0.06 |    4 |         - |          NA |
| SmoothSort       | 256  | Random             |     5,152.8 ns |     96.57 ns |     50.51 ns |  1.37 |    0.02 |    2 |         - |          NA |
| TournamentSort   | 256  | Random             |     7,623.8 ns |    821.70 ns |    429.76 ns |  2.02 |    0.11 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **SingleElementMoved** |     **3,847.7 ns** |    **270.04 ns** |    **119.90 ns** |  **1.00** |    **0.04** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 256  | SingleElementMoved |     3,449.5 ns |    362.19 ns |    160.81 ns |  0.90 |    0.05 |    2 |         - |          NA |
| TernaryHeapSort  | 256  | SingleElementMoved |     4,204.7 ns |    231.78 ns |    102.91 ns |  1.09 |    0.04 |    2 |         - |          NA |
| BottomupHeapSort | 256  | SingleElementMoved |     4,172.9 ns |    377.05 ns |    197.20 ns |  1.09 |    0.06 |    2 |         - |          NA |
| WeakHeapSort     | 256  | SingleElementMoved |     7,924.2 ns |     52.54 ns |     27.48 ns |  2.06 |    0.06 |    4 |         - |          NA |
| SmoothSort       | 256  | SingleElementMoved |     1,732.1 ns |     21.19 ns |      9.41 ns |  0.45 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 256  | SingleElementMoved |     5,275.7 ns |    384.45 ns |    170.70 ns |  1.37 |    0.06 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **Sorted**             |     **3,916.5 ns** |    **435.50 ns** |    **193.36 ns** |  **1.00** |    **0.06** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 256  | Sorted             |     3,532.3 ns |    278.87 ns |    145.85 ns |  0.90 |    0.05 |    2 |         - |          NA |
| TernaryHeapSort  | 256  | Sorted             |     4,206.4 ns |    410.39 ns |    214.64 ns |  1.08 |    0.07 |    2 |         - |          NA |
| BottomupHeapSort | 256  | Sorted             |     4,096.3 ns |    284.68 ns |    148.89 ns |  1.05 |    0.06 |    2 |         - |          NA |
| WeakHeapSort     | 256  | Sorted             |     7,935.7 ns |    347.12 ns |    181.55 ns |  2.03 |    0.10 |    3 |         - |          NA |
| SmoothSort       | 256  | Sorted             |     1,267.0 ns |    100.02 ns |     44.41 ns |  0.32 |    0.02 |    1 |         - |          NA |
| TournamentSort   | 256  | Sorted             |     3,324.9 ns |    405.84 ns |    212.26 ns |  0.85 |    0.06 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **Reversed**           |     **4,022.1 ns** |    **463.99 ns** |    **242.68 ns** |  **1.00** |    **0.08** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | Reversed           |     3,395.3 ns |    240.57 ns |    106.81 ns |  0.85 |    0.05 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | Reversed           |     4,165.6 ns |    490.87 ns |    217.95 ns |  1.04 |    0.08 |    1 |         - |          NA |
| BottomupHeapSort | 256  | Reversed           |     4,381.2 ns |    419.50 ns |    219.41 ns |  1.09 |    0.08 |    1 |         - |          NA |
| WeakHeapSort     | 256  | Reversed           |     8,728.1 ns |    282.04 ns |    147.51 ns |  2.18 |    0.13 |    2 |         - |          NA |
| SmoothSort       | 256  | Reversed           |     4,871.3 ns |    485.70 ns |    254.03 ns |  1.21 |    0.09 |    1 |         - |          NA |
| TournamentSort   | 256  | Reversed           |     5,015.1 ns |    630.76 ns |    329.90 ns |  1.25 |    0.10 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **PipeOrgan**          |     **3,060.2 ns** |    **185.46 ns** |     **82.34 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | PipeOrgan          |     3,120.0 ns |    272.05 ns |    120.79 ns |  1.02 |    0.04 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | PipeOrgan          |     3,833.4 ns |    274.14 ns |    143.38 ns |  1.25 |    0.05 |    2 |         - |          NA |
| BottomupHeapSort | 256  | PipeOrgan          |     4,070.6 ns |    121.42 ns |     53.91 ns |  1.33 |    0.04 |    2 |         - |          NA |
| WeakHeapSort     | 256  | PipeOrgan          |     8,361.6 ns |    385.93 ns |    201.85 ns |  2.73 |    0.09 |    4 |         - |          NA |
| SmoothSort       | 256  | PipeOrgan          |     5,745.4 ns |    530.42 ns |    277.42 ns |  1.88 |    0.10 |    3 |         - |          NA |
| TournamentSort   | 256  | PipeOrgan          |     7,071.2 ns |    141.39 ns |     62.78 ns |  2.31 |    0.06 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Random**             |    **19,670.5 ns** |    **591.33 ns** |    **309.28 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Random             |    17,648.0 ns |    280.16 ns |    124.39 ns |  0.90 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | Random             |    20,499.3 ns |    650.18 ns |    340.06 ns |  1.04 |    0.02 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | Random             |    19,276.0 ns |    185.09 ns |     82.18 ns |  0.98 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | Random             |    48,016.2 ns |    202.41 ns |     89.87 ns |  2.44 |    0.04 |    3 |         - |          NA |
| SmoothSort       | 1024 | Random             |    26,931.0 ns |    473.92 ns |    247.87 ns |  1.37 |    0.02 |    2 |         - |          NA |
| TournamentSort   | 1024 | Random             |    60,336.5 ns | 24,089.14 ns | 12,599.08 ns |  3.07 |    0.61 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **SingleElementMoved** |    **21,772.4 ns** |    **934.55 ns** |    **414.95 ns** |  **1.00** |    **0.03** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | SingleElementMoved |    17,088.0 ns |    325.31 ns |    170.14 ns |  0.79 |    0.02 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | SingleElementMoved |    21,251.3 ns |  1,695.02 ns |    886.53 ns |  0.98 |    0.04 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | SingleElementMoved |    19,064.1 ns |    553.65 ns |    289.57 ns |  0.88 |    0.02 |    2 |         - |          NA |
| WeakHeapSort     | 1024 | SingleElementMoved |    40,753.4 ns |  1,228.00 ns |    545.24 ns |  1.87 |    0.04 |    4 |         - |          NA |
| SmoothSort       | 1024 | SingleElementMoved |     7,530.4 ns |    510.57 ns |    226.70 ns |  0.35 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 1024 | SingleElementMoved |    27,839.7 ns |  6,145.91 ns |  3,214.43 ns |  1.28 |    0.14 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Sorted**             |    **22,378.0 ns** |    **692.66 ns** |    **307.55 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Sorted             |    16,914.8 ns |    204.42 ns |     90.76 ns |  0.76 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | Sorted             |    22,805.6 ns |  1,391.57 ns |    617.87 ns |  1.02 |    0.03 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | Sorted             |    19,226.7 ns |    403.12 ns |    178.99 ns |  0.86 |    0.01 |    2 |         - |          NA |
| WeakHeapSort     | 1024 | Sorted             |    41,324.9 ns |    966.31 ns |    429.05 ns |  1.85 |    0.03 |    3 |         - |          NA |
| SmoothSort       | 1024 | Sorted             |     5,173.9 ns |    297.28 ns |    155.48 ns |  0.23 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 1024 | Sorted             |    15,438.9 ns |  1,076.03 ns |    477.76 ns |  0.69 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Reversed**           |    **19,291.1 ns** |    **416.16 ns** |    **184.78 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Reversed           |    18,643.7 ns |  1,641.28 ns |    858.42 ns |  0.97 |    0.04 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | Reversed           |    19,235.5 ns |    296.14 ns |    131.49 ns |  1.00 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | Reversed           |    19,727.5 ns |    380.20 ns |    198.85 ns |  1.02 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | Reversed           |    44,620.0 ns |    217.89 ns |     96.74 ns |  2.31 |    0.02 |    2 |         - |          NA |
| SmoothSort       | 1024 | Reversed           |    23,260.1 ns |    340.24 ns |    177.95 ns |  1.21 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 1024 | Reversed           |    25,869.0 ns |  2,750.71 ns |  1,438.67 ns |  1.34 |    0.07 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **PipeOrgan**          |    **16,099.6 ns** |    **495.22 ns** |    **219.88 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | PipeOrgan          |    16,203.5 ns |    400.97 ns |    209.72 ns |  1.01 |    0.02 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | PipeOrgan          |    18,469.7 ns |    479.83 ns |    213.05 ns |  1.15 |    0.02 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | PipeOrgan          |    19,214.3 ns |    294.67 ns |    130.83 ns |  1.19 |    0.02 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | PipeOrgan          |    43,844.7 ns |    859.57 ns |    381.66 ns |  2.72 |    0.04 |    3 |         - |          NA |
| SmoothSort       | 1024 | PipeOrgan          |    27,532.7 ns |    240.45 ns |     85.75 ns |  1.71 |    0.02 |    2 |         - |          NA |
| TournamentSort   | 1024 | PipeOrgan          |    41,113.3 ns |  4,196.55 ns |  1,863.29 ns |  2.55 |    0.11 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Random**             |   **497,684.0 ns** |    **872.08 ns** |    **387.21 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Random             |   486,620.0 ns |  2,029.34 ns |  1,061.39 ns |  0.98 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | Random             |   583,958.2 ns |  1,823.81 ns |    953.89 ns |  1.17 |    0.00 |    1 |         - |          NA |
| BottomupHeapSort | 8192 | Random             |   580,970.3 ns |  3,025.89 ns |  1,343.51 ns |  1.17 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | Random             |   886,108.5 ns |  2,272.04 ns |  1,188.32 ns |  1.78 |    0.00 |    2 |         - |          NA |
| SmoothSort       | 8192 | Random             |   816,356.7 ns |  2,961.84 ns |  1,315.07 ns |  1.64 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 8192 | Random             | 1,347,757.7 ns |  2,222.75 ns |    986.91 ns |  2.71 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **SingleElementMoved** |   **369,861.7 ns** |  **1,581.22 ns** |    **827.01 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 8192 | SingleElementMoved |   379,296.8 ns |  1,080.25 ns |    479.64 ns |  1.03 |    0.00 |    2 |         - |          NA |
| TernaryHeapSort  | 8192 | SingleElementMoved |   412,408.6 ns |  1,176.85 ns |    522.53 ns |  1.12 |    0.00 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | SingleElementMoved |   446,746.2 ns |    699.69 ns |    310.67 ns |  1.21 |    0.00 |    2 |         - |          NA |
| WeakHeapSort     | 8192 | SingleElementMoved |   432,589.5 ns |  2,000.73 ns |    888.34 ns |  1.17 |    0.00 |    2 |         - |          NA |
| SmoothSort       | 8192 | SingleElementMoved |    59,527.7 ns |  1,480.23 ns |    774.19 ns |  0.16 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 8192 | SingleElementMoved |   775,513.0 ns |  5,681.80 ns |  2,522.75 ns |  2.10 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Sorted**             |   **367,733.6 ns** |  **3,039.36 ns** |  **1,083.86 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Sorted             |   380,338.1 ns |  2,580.22 ns |  1,349.50 ns |  1.03 |    0.00 |    2 |         - |          NA |
| TernaryHeapSort  | 8192 | Sorted             |   415,134.0 ns |  3,824.58 ns |  2,000.33 ns |  1.13 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | Sorted             |   447,780.1 ns |  2,828.44 ns |  1,255.84 ns |  1.22 |    0.00 |    2 |         - |          NA |
| WeakHeapSort     | 8192 | Sorted             |   440,610.7 ns |  1,354.17 ns |    601.26 ns |  1.20 |    0.00 |    2 |         - |          NA |
| SmoothSort       | 8192 | Sorted             |    41,165.3 ns |    985.11 ns |    437.40 ns |  0.11 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 8192 | Sorted             |   511,654.6 ns | 11,012.21 ns |  5,759.60 ns |  1.39 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Reversed**           |   **396,485.1 ns** |  **1,799.59 ns** |    **641.75 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Reversed           |   352,741.7 ns |    634.62 ns |    331.92 ns |  0.89 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | Reversed           |   424,193.9 ns |  1,077.78 ns |    563.70 ns |  1.07 |    0.00 |    1 |         - |          NA |
| BottomupHeapSort | 8192 | Reversed           |   479,175.8 ns |    879.17 ns |    459.82 ns |  1.21 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | Reversed           |   471,465.0 ns |  1,401.06 ns |    732.78 ns |  1.19 |    0.00 |    1 |         - |          NA |
| SmoothSort       | 8192 | Reversed           |   507,559.1 ns |  2,964.16 ns |  1,550.31 ns |  1.28 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 8192 | Reversed           |   730,735.1 ns |  1,889.11 ns |    988.04 ns |  1.84 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **PipeOrgan**          |   **367,961.4 ns** |  **1,348.40 ns** |    **598.70 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | PipeOrgan          |   380,791.6 ns |  1,148.14 ns |    509.78 ns |  1.03 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | PipeOrgan          |   460,269.5 ns | 14,090.68 ns |  7,369.70 ns |  1.25 |    0.02 |    1 |         - |          NA |
| BottomupHeapSort | 8192 | PipeOrgan          |   454,362.6 ns |  1,255.97 ns |    557.66 ns |  1.23 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | PipeOrgan          |   501,623.9 ns |  1,999.80 ns |  1,045.94 ns |  1.36 |    0.00 |    1 |         - |          NA |
| SmoothSort       | 8192 | PipeOrgan          |   654,968.4 ns |  2,019.81 ns |  1,056.40 ns |  1.78 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 8192 | PipeOrgan          | 1,074,439.9 ns | 15,634.17 ns |  6,941.67 ns |  2.92 |    0.02 |    3 |         - |          NA |

### InsertionBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.78GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                 | Size | Pattern            | Mean         | Error        | StdDev      | Median       | Ratio  | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------- |----- |------------------- |-------------:|-------------:|------------:|-------------:|-------:|--------:|-----:|----------:|------------:|
| **InsertionSort**          | **256**  | **Random**             |   **8,699.4 ns** |    **927.51 ns** |   **411.82 ns** |   **8,632.3 ns** |   **1.00** |    **0.06** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Random             |   8,831.7 ns |    402.32 ns |   178.63 ns |   8,717.4 ns |   1.02 |    0.05 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | Random             |   9,405.0 ns |    266.20 ns |   139.23 ns |   9,434.5 ns |   1.08 |    0.05 |    2 |         - |          NA |
| GnomeSort              | 256  | Random             |  27,848.2 ns |    174.10 ns |    91.06 ns |  27,838.4 ns |   3.21 |    0.14 |    4 |         - |          NA |
| LibrarySort            | 256  | Random             |  16,098.8 ns |     81.37 ns |    36.13 ns |  16,103.1 ns |   1.85 |    0.08 |    3 |         - |          NA |
| MergeInsertionSort     | 256  | Random             |  32,772.0 ns |  1,501.40 ns |   785.26 ns |  32,853.4 ns |   3.77 |    0.18 |    4 |         - |          NA |
| ShellSortKnuth1973     | 256  | Random             |   3,080.1 ns |    302.12 ns |   158.01 ns |   2,984.4 ns |   0.35 |    0.02 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Random             |   2,988.1 ns |    221.13 ns |    98.18 ns |   2,949.8 ns |   0.34 |    0.02 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | Random             |   3,170.7 ns |     61.53 ns |    27.32 ns |   3,159.4 ns |   0.37 |    0.02 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | Random             |   3,170.5 ns |    552.52 ns |   288.98 ns |   2,974.8 ns |   0.37 |    0.04 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Random             |   3,111.1 ns |    209.44 ns |    92.99 ns |   3,069.0 ns |   0.36 |    0.02 |    1 |         - |          NA |
|      |                    |              |              |             |              |        |         |      |           |             |
| **InsertionSort**          | **256**  | **SingleElementMoved** |     **440.1 ns** |     **55.72 ns** |    **24.74 ns** |     **426.3 ns** |   **1.00** |    **0.07** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 256  | SingleElementMoved |     295.9 ns |     12.11 ns |     5.38 ns |     297.4 ns |   0.67 |    0.04 |    1 |         - |          NA |
| BinaryInsertSort       | 256  | SingleElementMoved |   1,004.7 ns |     74.00 ns |    26.39 ns |   1,013.2 ns |   2.29 |    0.13 |    3 |         - |          NA |
| GnomeSort              | 256  | SingleElementMoved |     464.8 ns |      1.94 ns |     0.86 ns |     464.4 ns |   1.06 |    0.05 |    2 |         - |          NA |
| LibrarySort            | 256  | SingleElementMoved |   8,535.5 ns |    414.91 ns |   217.01 ns |   8,576.4 ns |  19.44 |    1.08 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | SingleElementMoved |  24,650.8 ns |    575.21 ns |   300.85 ns |  24,715.2 ns |  56.16 |    2.88 |    6 |         - |          NA |
| ShellSortKnuth1973     | 256  | SingleElementMoved |   1,317.9 ns |    122.37 ns |    54.33 ns |   1,290.7 ns |   3.00 |    0.19 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 256  | SingleElementMoved |   1,286.3 ns |     10.44 ns |     4.63 ns |   1,285.7 ns |   2.93 |    0.15 |    4 |         - |          NA |
| ShellSortTokuda1992    | 256  | SingleElementMoved |   1,607.4 ns |     19.19 ns |     6.84 ns |   1,608.4 ns |   3.66 |    0.18 |    4 |         - |          NA |
| ShellSortCiura2001     | 256  | SingleElementMoved |   1,384.4 ns |     22.75 ns |    10.10 ns |   1,383.7 ns |   3.15 |    0.16 |    4 |         - |          NA |
| ShellSortLee2021       | 256  | SingleElementMoved |   1,567.2 ns |     17.64 ns |     7.83 ns |   1,569.0 ns |   3.57 |    0.18 |    4 |         - |          NA |
|      |                    |              |              |             |              |        |         |      |           |             |
| **InsertionSort**          | **256**  | **Sorted**             |     **323.5 ns** |      **1.03 ns** |     **0.46 ns** |     **323.6 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Sorted             |     211.5 ns |      1.38 ns |     0.61 ns |     211.5 ns |   0.65 |    0.00 |    1 |         - |          NA |
| BinaryInsertSort       | 256  | Sorted             |     208.1 ns |     64.65 ns |    33.81 ns |     211.5 ns |   0.64 |    0.10 |    1 |         - |          NA |
| GnomeSort              | 256  | Sorted             |     341.2 ns |    249.99 ns |   130.75 ns |     247.0 ns |   1.05 |    0.38 |    3 |         - |          NA |
| LibrarySort            | 256  | Sorted             |   7,218.9 ns |    103.36 ns |    54.06 ns |   7,234.9 ns |  22.31 |    0.16 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | Sorted             |  23,318.5 ns |  1,117.08 ns |   584.26 ns |  23,166.7 ns |  72.07 |    1.71 |    6 |         - |          NA |
| ShellSortKnuth1973     | 256  | Sorted             |   1,067.1 ns |      2.03 ns |     1.06 ns |   1,066.9 ns |   3.30 |    0.01 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Sorted             |   1,082.8 ns |     51.61 ns |    22.91 ns |   1,069.9 ns |   3.35 |    0.07 |    4 |         - |          NA |
| ShellSortTokuda1992    | 256  | Sorted             |   1,306.6 ns |      3.51 ns |     1.25 ns |   1,306.7 ns |   4.04 |    0.01 |    4 |         - |          NA |
| ShellSortCiura2001     | 256  | Sorted             |   1,144.5 ns |      1.34 ns |     0.59 ns |   1,144.3 ns |   3.54 |    0.00 |    4 |         - |          NA |
| ShellSortLee2021       | 256  | Sorted             |   1,308.6 ns |      1.48 ns |     0.66 ns |   1,308.4 ns |   4.04 |    0.01 |    4 |         - |          NA |
|      |                    |              |              |             |              |        |         |      |           |             |
| **InsertionSort**          | **256**  | **Reversed**           |  **20,113.7 ns** |  **5,118.96 ns** | **2,677.32 ns** |  **21,275.9 ns** |   **1.02** |    **0.19** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Reversed           |  18,668.7 ns |     64.84 ns |    23.12 ns |  18,670.3 ns |   0.94 |    0.13 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | Reversed           |  17,099.5 ns |    561.56 ns |   293.71 ns |  17,057.7 ns |   0.86 |    0.12 |    2 |         - |          NA |
| GnomeSort              | 256  | Reversed           |  57,827.4 ns |    321.29 ns |   142.65 ns |  57,802.8 ns |   2.92 |    0.39 |    4 |         - |          NA |
| LibrarySort            | 256  | Reversed           |  38,632.6 ns |  1,007.64 ns |   447.40 ns |  38,478.3 ns |   1.95 |    0.26 |    3 |         - |          NA |
| MergeInsertionSort     | 256  | Reversed           |  24,743.9 ns |    556.06 ns |   290.83 ns |  24,649.9 ns |   1.25 |    0.17 |    2 |         - |          NA |
| ShellSortKnuth1973     | 256  | Reversed           |   1,778.1 ns |     14.85 ns |     6.59 ns |   1,778.2 ns |   0.09 |    0.01 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Reversed           |   1,783.6 ns |     32.72 ns |    17.11 ns |   1,776.9 ns |   0.09 |    0.01 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | Reversed           |   1,917.2 ns |     68.97 ns |    24.60 ns |   1,906.0 ns |   0.10 |    0.01 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | Reversed           |   1,820.9 ns |     51.40 ns |    22.82 ns |   1,816.7 ns |   0.09 |    0.01 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Reversed           |   1,878.9 ns |     28.05 ns |    10.00 ns |   1,875.6 ns |   0.09 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |             |              |        |         |      |           |             |
| **InsertionSort**          | **256**  | **PipeOrgan**          |   **9,236.6 ns** |    **858.31 ns** |   **381.10 ns** |   **9,257.4 ns** |   **1.00** |    **0.05** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 256  | PipeOrgan          |   9,568.2 ns |    633.63 ns |   281.34 ns |   9,423.1 ns |   1.04 |    0.05 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | PipeOrgan          |  10,170.0 ns |    494.86 ns |   258.82 ns |  10,312.4 ns |   1.10 |    0.05 |    2 |         - |          NA |
| GnomeSort              | 256  | PipeOrgan          |  23,662.9 ns |    120.78 ns |    63.17 ns |  23,647.8 ns |   2.57 |    0.10 |    3 |         - |          NA |
| LibrarySort            | 256  | PipeOrgan          |  24,188.9 ns |    130.80 ns |    68.41 ns |  24,193.3 ns |   2.62 |    0.10 |    3 |         - |          NA |
| MergeInsertionSort     | 256  | PipeOrgan          |  22,371.9 ns |  1,091.05 ns |   570.64 ns |  22,418.3 ns |   2.43 |    0.11 |    3 |         - |          NA |
| ShellSortKnuth1973     | 256  | PipeOrgan          |   1,596.1 ns |    111.09 ns |    49.33 ns |   1,592.2 ns |   0.17 |    0.01 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | PipeOrgan          |   1,527.1 ns |     13.70 ns |     6.08 ns |   1,524.2 ns |   0.17 |    0.01 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | PipeOrgan          |   2,008.4 ns |     55.95 ns |    29.26 ns |   2,011.2 ns |   0.22 |    0.01 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | PipeOrgan          |   1,668.5 ns |     31.35 ns |    11.18 ns |   1,666.0 ns |   0.18 |    0.01 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | PipeOrgan          |   1,815.9 ns |    117.79 ns |    61.61 ns |   1,786.4 ns |   0.20 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |             |              |        |         |      |           |             |
| **InsertionSort**          | **1024** | **Random**             | **126,652.8 ns** |  **2,913.95 ns** | **1,524.05 ns** | **126,412.3 ns** |   **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Random             | 139,175.5 ns |  4,912.78 ns | 2,569.48 ns | 138,371.3 ns |   1.10 |    0.02 |    3 |         - |          NA |
| BinaryInsertSort       | 1024 | Random             | 147,512.9 ns |    863.22 ns |   451.48 ns | 147,431.9 ns |   1.16 |    0.01 |    3 |         - |          NA |
| GnomeSort              | 1024 | Random             | 424,468.3 ns |  1,977.11 ns |   877.85 ns | 424,470.7 ns |   3.35 |    0.04 |    4 |         - |          NA |
| LibrarySort            | 1024 | Random             |  82,150.6 ns |    790.21 ns |   281.80 ns |  82,090.8 ns |   0.65 |    0.01 |    2 |         - |          NA |
| MergeInsertionSort     | 1024 | Random             | 384,283.9 ns |  3,397.22 ns | 1,776.81 ns | 384,068.8 ns |   3.03 |    0.04 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Random             |  16,571.3 ns |    409.25 ns |   181.71 ns |  16,592.8 ns |   0.13 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Random             |  16,567.4 ns |    118.99 ns |    52.83 ns |  16,541.7 ns |   0.13 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Random             |  17,663.3 ns |  1,184.34 ns |   619.43 ns |  17,415.1 ns |   0.14 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Random             |  16,666.3 ns |    133.21 ns |    59.15 ns |  16,658.8 ns |   0.13 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Random             |  17,163.9 ns |    464.65 ns |   243.02 ns |  17,114.4 ns |   0.14 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |              |        |         |      |           |             |
| **InsertionSort**          | **1024** | **SingleElementMoved** |   **1,628.3 ns** |      **1.92 ns** |     **0.85 ns** |   **1,628.0 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | SingleElementMoved |   1,077.3 ns |      9.97 ns |     4.42 ns |   1,075.3 ns |   0.66 |    0.00 |    1 |         - |          NA |
| BinaryInsertSort       | 1024 | SingleElementMoved |   5,386.2 ns |  1,578.19 ns |   825.42 ns |   4,987.9 ns |   3.31 |    0.48 |    3 |         - |          NA |
| GnomeSort              | 1024 | SingleElementMoved |   1,921.2 ns |    451.98 ns |   200.68 ns |   1,806.1 ns |   1.18 |    0.12 |    2 |         - |          NA |
| LibrarySort            | 1024 | SingleElementMoved |  37,221.9 ns |    379.97 ns |   198.73 ns |  37,150.8 ns |  22.86 |    0.12 |    5 |         - |          NA |
| MergeInsertionSort     | 1024 | SingleElementMoved | 262,211.6 ns |  3,319.65 ns | 1,473.95 ns | 262,414.9 ns | 161.03 |    0.85 |    6 |         - |          NA |
| ShellSortKnuth1973     | 1024 | SingleElementMoved |   6,203.8 ns |    300.04 ns |   156.93 ns |   6,206.5 ns |   3.81 |    0.09 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | SingleElementMoved |   6,894.5 ns |    473.86 ns |   247.84 ns |   6,810.0 ns |   4.23 |    0.14 |    4 |         - |          NA |
| ShellSortTokuda1992    | 1024 | SingleElementMoved |   7,295.5 ns |     43.35 ns |    19.25 ns |   7,307.6 ns |   4.48 |    0.01 |    4 |         - |          NA |
| ShellSortCiura2001     | 1024 | SingleElementMoved |   7,266.3 ns |    449.37 ns |   235.03 ns |   7,102.8 ns |   4.46 |    0.14 |    4 |         - |          NA |
| ShellSortLee2021       | 1024 | SingleElementMoved |   7,265.5 ns |     31.39 ns |    13.94 ns |   7,266.3 ns |   4.46 |    0.01 |    4 |         - |          NA |
|      |                    |              |              |             |              |        |         |      |           |             |
| **InsertionSort**          | **1024** | **Sorted**             |   **1,528.8 ns** |     **18.74 ns** |     **6.68 ns** |   **1,525.7 ns** |   **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Sorted             |     807.4 ns |     12.46 ns |     4.44 ns |     806.0 ns |   0.53 |    0.00 |    2 |         - |          NA |
| BinaryInsertSort       | 1024 | Sorted             |     647.2 ns |      2.29 ns |     1.02 ns |     647.0 ns |   0.42 |    0.00 |    1 |         - |          NA |
| GnomeSort              | 1024 | Sorted             |     957.6 ns |      1.26 ns |     0.56 ns |     957.6 ns |   0.63 |    0.00 |    2 |         - |          NA |
| LibrarySort            | 1024 | Sorted             |  31,501.2 ns |    521.82 ns |   272.92 ns |  31,359.8 ns |  20.61 |    0.19 |    5 |         - |          NA |
| MergeInsertionSort     | 1024 | Sorted             | 258,816.3 ns |  5,022.15 ns | 2,626.68 ns | 258,764.1 ns | 169.30 |    1.76 |    6 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Sorted             |   5,416.4 ns |    410.13 ns |   214.51 ns |   5,374.9 ns |   3.54 |    0.13 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Sorted             |   6,041.2 ns |    369.64 ns |   193.33 ns |   6,002.8 ns |   3.95 |    0.12 |    4 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Sorted             |   6,461.4 ns |    314.71 ns |   164.60 ns |   6,456.4 ns |   4.23 |    0.10 |    4 |         - |          NA |
| ShellSortCiura2001     | 1024 | Sorted             |   6,625.5 ns |    272.50 ns |   142.52 ns |   6,670.4 ns |   4.33 |    0.09 |    4 |         - |          NA |
| ShellSortLee2021       | 1024 | Sorted             |   6,611.4 ns |     78.55 ns |    28.01 ns |   6,605.2 ns |   4.32 |    0.02 |    4 |         - |          NA |
|      |                    |              |              |             |              |        |         |      |           |             |
| **InsertionSort**          | **1024** | **Reversed**           | **250,021.5 ns** |  **1,655.59 ns** |   **865.90 ns** | **249,803.8 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Reversed           | 281,225.3 ns |    909.55 ns |   475.71 ns | 281,337.6 ns |   1.12 |    0.00 |    2 |         - |          NA |
| BinaryInsertSort       | 1024 | Reversed           | 235,052.7 ns |    994.53 ns |   441.58 ns | 234,859.6 ns |   0.94 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | Reversed           | 858,893.6 ns |  8,991.71 ns | 3,992.37 ns | 858,968.0 ns |   3.44 |    0.02 |    4 |         - |          NA |
| LibrarySort            | 1024 | Reversed           | 417,765.5 ns |  1,946.05 ns |   864.06 ns | 417,518.2 ns |   1.67 |    0.01 |    3 |         - |          NA |
| MergeInsertionSort     | 1024 | Reversed           | 268,683.1 ns | 13,253.45 ns | 6,931.81 ns | 268,761.8 ns |   1.07 |    0.03 |    2 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Reversed           |   8,559.8 ns |    339.63 ns |   177.64 ns |   8,628.0 ns |   0.03 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Reversed           |   8,983.6 ns |    209.26 ns |   109.45 ns |   9,007.8 ns |   0.04 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Reversed           |   9,923.3 ns |    372.71 ns |   165.48 ns |   9,972.4 ns |   0.04 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Reversed           |   9,507.6 ns |    468.57 ns |   245.07 ns |   9,484.5 ns |   0.04 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Reversed           |   9,993.9 ns |    453.19 ns |   237.03 ns |  10,029.9 ns |   0.04 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |              |        |         |      |           |             |
| **InsertionSort**          | **1024** | **PipeOrgan**          | **128,899.7 ns** |  **6,036.86 ns** | **3,157.39 ns** | **127,680.9 ns** |   **1.00** |    **0.03** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | PipeOrgan          | 144,333.4 ns |  4,080.22 ns | 2,134.03 ns | 144,968.3 ns |   1.12 |    0.03 |    2 |         - |          NA |
| BinaryInsertSort       | 1024 | PipeOrgan          | 130,124.0 ns |    421.59 ns |   187.19 ns | 130,124.2 ns |   1.01 |    0.02 |    2 |         - |          NA |
| GnomeSort              | 1024 | PipeOrgan          | 341,278.3 ns |  1,656.46 ns |   866.36 ns | 341,155.4 ns |   2.65 |    0.06 |    5 |         - |          NA |
| LibrarySort            | 1024 | PipeOrgan          | 273,980.4 ns |  2,858.61 ns | 1,269.24 ns | 274,631.8 ns |   2.13 |    0.05 |    4 |         - |          NA |
| MergeInsertionSort     | 1024 | PipeOrgan          | 225,927.6 ns | 19,077.32 ns | 9,977.81 ns | 223,123.8 ns |   1.75 |    0.08 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | PipeOrgan          |   8,308.4 ns |    448.54 ns |   234.59 ns |   8,373.5 ns |   0.06 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | PipeOrgan          |   8,449.2 ns |    227.10 ns |   100.83 ns |   8,382.3 ns |   0.07 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | PipeOrgan          |   9,181.5 ns |    364.68 ns |   190.74 ns |   9,259.5 ns |   0.07 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | PipeOrgan          |   9,039.2 ns |    469.49 ns |   245.55 ns |   9,000.7 ns |   0.07 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | PipeOrgan          |   9,323.7 ns |    500.16 ns |   261.60 ns |   9,358.2 ns |   0.07 |    0.00 |    1 |         - |          NA |

### IntKeyBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.78GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method             | Size | Pattern            | Mean           | Error         | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------- |----- |------------------- |---------------:|--------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **QuickSort**          | **256**  | **Random**             |     **3,023.1 ns** |     **116.27 ns** |     **51.62 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| QuickSort3way      | 256  | Random             |     3,120.7 ns |     119.04 ns |     42.45 ns |  1.03 |    0.02 |    1 |         - |          NA |
| QuickSortMedian3   | 256  | Random             |     4,346.3 ns |      71.32 ns |     25.43 ns |  1.44 |    0.02 |    2 |         - |          NA |
| QuickSortMedian9   | 256  | Random             |     4,084.2 ns |     283.01 ns |    148.02 ns |  1.35 |    0.05 |    2 |         - |          NA |
| DualPivotQuickSort | 256  | Random             |     2,587.2 ns |      46.47 ns |     20.63 ns |  0.86 |    0.01 |    1 |         - |          NA |
| StableQuickSort    | 256  | Random             |    11,208.2 ns |     352.74 ns |    156.62 ns |  3.71 |    0.08 |    3 |         - |          NA |
| IntroSort          | 256  | Random             |     2,198.6 ns |      85.68 ns |     30.56 ns |  0.73 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | Random             |     1,883.6 ns |      20.41 ns |      9.06 ns |  0.62 |    0.01 |    1 |         - |          NA |
| PDQSort            | 256  | Random             |     1,889.2 ns |      63.82 ns |     28.34 ns |  0.63 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | Random             |     3,402.6 ns |      92.60 ns |     33.02 ns |  1.13 |    0.02 |    1 |         - |          NA |
| StdSort            | 256  | Random             |     3,218.3 ns |      64.98 ns |     23.17 ns |  1.06 |    0.02 |    1 |         - |          NA |
| BlockQuickSort     | 256  | Random             |     2,993.1 ns |     388.47 ns |    203.18 ns |  0.99 |    0.07 |    1 |         - |          NA |
| DotnetSort         | 256  | Random             |     2,063.5 ns |      54.01 ns |     19.26 ns |  0.68 |    0.01 |    1 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **QuickSort**          | **256**  | **SingleElementMoved** |     **1,582.1 ns** |      **29.86 ns** |     **10.65 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 256  | SingleElementMoved |     5,036.2 ns |     500.60 ns |    261.82 ns |  3.18 |    0.16 |    4 |         - |          NA |
| QuickSortMedian3   | 256  | SingleElementMoved |     5,182.0 ns |     294.90 ns |    154.24 ns |  3.28 |    0.09 |    4 |         - |          NA |
| QuickSortMedian9   | 256  | SingleElementMoved |     4,332.5 ns |     359.75 ns |    188.16 ns |  2.74 |    0.11 |    4 |         - |          NA |
| DualPivotQuickSort | 256  | SingleElementMoved |     3,991.6 ns |      17.41 ns |      7.73 ns |  2.52 |    0.02 |    4 |         - |          NA |
| StableQuickSort    | 256  | SingleElementMoved |     9,042.1 ns |   1,163.93 ns |    608.76 ns |  5.72 |    0.37 |    5 |         - |          NA |
| IntroSort          | 256  | SingleElementMoved |       933.6 ns |      39.55 ns |     17.56 ns |  0.59 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | SingleElementMoved |     1,120.2 ns |      17.70 ns |      7.86 ns |  0.71 |    0.01 |    1 |         - |          NA |
| PDQSort            | 256  | SingleElementMoved |     1,176.3 ns |      21.37 ns |      9.49 ns |  0.74 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | SingleElementMoved |     1,468.8 ns |      20.48 ns |      7.30 ns |  0.93 |    0.01 |    2 |         - |          NA |
| StdSort            | 256  | SingleElementMoved |     2,741.1 ns |     117.20 ns |     41.79 ns |  1.73 |    0.03 |    3 |         - |          NA |
| BlockQuickSort     | 256  | SingleElementMoved |     1,510.8 ns |      44.21 ns |     23.12 ns |  0.95 |    0.02 |    2 |         - |          NA |
| DotnetSort         | 256  | SingleElementMoved |     1,134.1 ns |       8.50 ns |      3.77 ns |  0.72 |    0.00 |    1 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **QuickSort**          | **256**  | **Sorted**             |     **1,121.7 ns** |      **39.55 ns** |     **17.56 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| QuickSort3way      | 256  | Sorted             |     6,320.5 ns |      71.10 ns |     31.57 ns |  5.64 |    0.09 |    5 |         - |          NA |
| QuickSortMedian3   | 256  | Sorted             |     6,499.4 ns |     287.49 ns |    150.36 ns |  5.80 |    0.15 |    5 |         - |          NA |
| QuickSortMedian9   | 256  | Sorted             |     4,672.9 ns |     294.23 ns |    153.89 ns |  4.17 |    0.14 |    4 |         - |          NA |
| DualPivotQuickSort | 256  | Sorted             |     4,600.3 ns |      16.28 ns |      5.81 ns |  4.10 |    0.06 |    4 |         - |          NA |
| StableQuickSort    | 256  | Sorted             |     8,884.9 ns |     450.79 ns |    235.77 ns |  7.92 |    0.23 |    6 |         - |          NA |
| IntroSort          | 256  | Sorted             |       307.0 ns |       1.88 ns |      0.98 ns |  0.27 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | Sorted             |     1,048.1 ns |      16.43 ns |      8.59 ns |  0.93 |    0.02 |    3 |         - |          NA |
| PDQSort            | 256  | Sorted             |       299.6 ns |       1.72 ns |      0.90 ns |  0.27 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | Sorted             |       300.9 ns |       2.48 ns |      1.30 ns |  0.27 |    0.00 |    1 |         - |          NA |
| StdSort            | 256  | Sorted             |       707.8 ns |       4.54 ns |      2.02 ns |  0.63 |    0.01 |    2 |         - |          NA |
| BlockQuickSort     | 256  | Sorted             |     1,281.8 ns |       7.19 ns |      3.19 ns |  1.14 |    0.02 |    3 |         - |          NA |
| DotnetSort         | 256  | Sorted             |       914.0 ns |       7.19 ns |      2.56 ns |  0.81 |    0.01 |    3 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **QuickSort**          | **256**  | **Reversed**           |       **973.3 ns** |      **40.94 ns** |     **21.41 ns** |  **1.00** |    **0.03** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 256  | Reversed           |     5,291.0 ns |     258.80 ns |    114.91 ns |  5.44 |    0.16 |    4 |         - |          NA |
| QuickSortMedian3   | 256  | Reversed           |     7,622.0 ns |     238.25 ns |    124.61 ns |  7.83 |    0.20 |    5 |         - |          NA |
| QuickSortMedian9   | 256  | Reversed           |     4,949.5 ns |      34.29 ns |     12.23 ns |  5.09 |    0.11 |    4 |         - |          NA |
| DualPivotQuickSort | 256  | Reversed           |     4,582.0 ns |     244.30 ns |    108.47 ns |  4.71 |    0.14 |    4 |         - |          NA |
| StableQuickSort    | 256  | Reversed           |     9,225.8 ns |     535.66 ns |    280.16 ns |  9.48 |    0.33 |    6 |         - |          NA |
| IntroSort          | 256  | Reversed           |       636.2 ns |       3.95 ns |      1.76 ns |  0.65 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | Reversed           |     1,584.6 ns |      33.52 ns |     17.53 ns |  1.63 |    0.04 |    3 |         - |          NA |
| PDQSort            | 256  | Reversed           |       531.2 ns |       6.01 ns |      2.67 ns |  0.55 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | Reversed           |       918.2 ns |      12.29 ns |      5.46 ns |  0.94 |    0.02 |    2 |         - |          NA |
| StdSort            | 256  | Reversed           |     1,140.7 ns |     365.37 ns |    162.22 ns |  1.17 |    0.16 |    3 |         - |          NA |
| BlockQuickSort     | 256  | Reversed           |     1,607.0 ns |      25.99 ns |     11.54 ns |  1.65 |    0.04 |    3 |         - |          NA |
| DotnetSort         | 256  | Reversed           |     1,400.3 ns |      30.79 ns |     10.98 ns |  1.44 |    0.03 |    3 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **QuickSort**          | **256**  | **PipeOrgan**          |     **7,863.9 ns** |     **361.71 ns** |    **189.18 ns** |  **1.00** |    **0.03** |    **4** |         **-** |          **NA** |
| QuickSort3way      | 256  | PipeOrgan          |     5,507.3 ns |     205.57 ns |    107.52 ns |  0.70 |    0.02 |    3 |         - |          NA |
| QuickSortMedian3   | 256  | PipeOrgan          |     6,672.8 ns |     503.83 ns |    263.51 ns |  0.85 |    0.04 |    4 |         - |          NA |
| QuickSortMedian9   | 256  | PipeOrgan          |     4,305.9 ns |     486.24 ns |    254.31 ns |  0.55 |    0.03 |    2 |         - |          NA |
| DualPivotQuickSort | 256  | PipeOrgan          |     2,157.6 ns |      17.63 ns |      7.83 ns |  0.27 |    0.01 |    1 |         - |          NA |
| StableQuickSort    | 256  | PipeOrgan          |     9,337.5 ns |     419.45 ns |    219.38 ns |  1.19 |    0.04 |    4 |         - |          NA |
| IntroSort          | 256  | PipeOrgan          |     1,973.1 ns |      35.43 ns |     15.73 ns |  0.25 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | PipeOrgan          |     2,488.8 ns |      91.85 ns |     32.75 ns |  0.32 |    0.01 |    1 |         - |          NA |
| PDQSort            | 256  | PipeOrgan          |     1,758.1 ns |      32.26 ns |     14.32 ns |  0.22 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | PipeOrgan          |     3,173.9 ns |      73.56 ns |     26.23 ns |  0.40 |    0.01 |    1 |         - |          NA |
| StdSort            | 256  | PipeOrgan          |     3,885.0 ns |      16.86 ns |      7.48 ns |  0.49 |    0.01 |    2 |         - |          NA |
| BlockQuickSort     | 256  | PipeOrgan          |     4,552.0 ns |     403.01 ns |    210.78 ns |  0.58 |    0.03 |    2 |         - |          NA |
| DotnetSort         | 256  | PipeOrgan          |     2,931.6 ns |     457.21 ns |    239.13 ns |  0.37 |    0.03 |    1 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **QuickSort**          | **1024** | **Random**             |    **15,615.2 ns** |     **317.56 ns** |    **141.00 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 1024 | Random             |    18,243.9 ns |     664.12 ns |    347.35 ns |  1.17 |    0.02 |    2 |         - |          NA |
| QuickSortMedian3   | 1024 | Random             |    24,178.3 ns |   2,305.25 ns |  1,023.55 ns |  1.55 |    0.06 |    3 |         - |          NA |
| QuickSortMedian9   | 1024 | Random             |    22,714.5 ns |   4,966.16 ns |  2,597.40 ns |  1.45 |    0.16 |    3 |         - |          NA |
| DualPivotQuickSort | 1024 | Random             |    12,471.8 ns |     360.83 ns |    160.21 ns |  0.80 |    0.01 |    1 |         - |          NA |
| StableQuickSort    | 1024 | Random             |    83,971.7 ns |     941.31 ns |    417.95 ns |  5.38 |    0.05 |    4 |         - |          NA |
| IntroSort          | 1024 | Random             |    12,046.5 ns |     539.68 ns |    282.26 ns |  0.77 |    0.02 |    1 |         - |          NA |
| IntroSortDotnet    | 1024 | Random             |     9,874.8 ns |     320.62 ns |    142.36 ns |  0.63 |    0.01 |    1 |         - |          NA |
| PDQSort            | 1024 | Random             |     9,722.1 ns |     465.79 ns |    206.81 ns |  0.62 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless  | 1024 | Random             |    17,181.3 ns |     768.60 ns |    401.99 ns |  1.10 |    0.03 |    2 |         - |          NA |
| StdSort            | 1024 | Random             |    15,297.9 ns |     244.63 ns |    108.62 ns |  0.98 |    0.01 |    2 |         - |          NA |
| BlockQuickSort     | 1024 | Random             |    16,160.5 ns |     212.90 ns |     94.53 ns |  1.03 |    0.01 |    2 |         - |          NA |
| DotnetSort         | 1024 | Random             |    11,357.1 ns |     287.32 ns |    127.57 ns |  0.73 |    0.01 |    1 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **QuickSort**          | **1024** | **SingleElementMoved** |     **7,796.1 ns** |     **608.79 ns** |    **270.31 ns** |  **1.00** |    **0.05** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 1024 | SingleElementMoved |    34,972.7 ns |     638.29 ns |    283.40 ns |  4.49 |    0.15 |    5 |         - |          NA |
| QuickSortMedian3   | 1024 | SingleElementMoved |    31,626.0 ns |     431.36 ns |    153.83 ns |  4.06 |    0.14 |    5 |         - |          NA |
| QuickSortMedian9   | 1024 | SingleElementMoved |    21,529.7 ns |     566.64 ns |    251.59 ns |  2.76 |    0.10 |    4 |         - |          NA |
| DualPivotQuickSort | 1024 | SingleElementMoved |    23,266.1 ns |     424.43 ns |    221.99 ns |  2.99 |    0.10 |    4 |         - |          NA |
| StableQuickSort    | 1024 | SingleElementMoved |    42,221.6 ns |     283.59 ns |    148.32 ns |  5.42 |    0.18 |    6 |         - |          NA |
| IntroSort          | 1024 | SingleElementMoved |     4,522.6 ns |     461.33 ns |    241.29 ns |  0.58 |    0.04 |    1 |         - |          NA |
| IntroSortDotnet    | 1024 | SingleElementMoved |     6,346.9 ns |      77.00 ns |     27.46 ns |  0.81 |    0.03 |    2 |         - |          NA |
| PDQSort            | 1024 | SingleElementMoved |     5,266.8 ns |     381.39 ns |    199.47 ns |  0.68 |    0.03 |    1 |         - |          NA |
| PDQSortBranchless  | 1024 | SingleElementMoved |     6,716.7 ns |     337.42 ns |    176.48 ns |  0.86 |    0.04 |    2 |         - |          NA |
| StdSort            | 1024 | SingleElementMoved |    11,813.3 ns |     471.06 ns |    246.38 ns |  1.52 |    0.06 |    3 |         - |          NA |
| BlockQuickSort     | 1024 | SingleElementMoved |     8,836.6 ns |     536.45 ns |    280.57 ns |  1.13 |    0.05 |    2 |         - |          NA |
| DotnetSort         | 1024 | SingleElementMoved |     6,382.5 ns |      68.35 ns |     24.37 ns |  0.82 |    0.03 |    2 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **QuickSort**          | **1024** | **Sorted**             |     **6,213.1 ns** |     **944.40 ns** |    **493.94 ns** |  **1.01** |    **0.11** |    **4** |         **-** |          **NA** |
| QuickSort3way      | 1024 | Sorted             |    47,063.1 ns |     544.85 ns |    284.97 ns |  7.62 |    0.59 |    7 |         - |          NA |
| QuickSortMedian3   | 1024 | Sorted             |    43,330.8 ns |     175.67 ns |     78.00 ns |  7.01 |    0.54 |    7 |         - |          NA |
| QuickSortMedian9   | 1024 | Sorted             |    22,790.6 ns |   1,240.30 ns |    648.70 ns |  3.69 |    0.30 |    6 |         - |          NA |
| DualPivotQuickSort | 1024 | Sorted             |    24,672.4 ns |     174.89 ns |     91.47 ns |  3.99 |    0.31 |    6 |         - |          NA |
| StableQuickSort    | 1024 | Sorted             |    43,222.1 ns |     309.08 ns |    137.23 ns |  7.00 |    0.54 |    7 |         - |          NA |
| IntroSort          | 1024 | Sorted             |     1,111.5 ns |       3.67 ns |      1.92 ns |  0.18 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet    | 1024 | Sorted             |     5,080.3 ns |     424.33 ns |    221.93 ns |  0.82 |    0.07 |    3 |         - |          NA |
| PDQSort            | 1024 | Sorted             |     1,012.8 ns |       2.36 ns |      0.84 ns |  0.16 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless  | 1024 | Sorted             |     1,017.9 ns |       4.86 ns |      2.16 ns |  0.16 |    0.01 |    1 |         - |          NA |
| StdSort            | 1024 | Sorted             |     2,612.9 ns |      12.04 ns |      4.29 ns |  0.42 |    0.03 |    2 |         - |          NA |
| BlockQuickSort     | 1024 | Sorted             |     7,648.1 ns |     230.27 ns |    120.43 ns |  1.24 |    0.10 |    5 |         - |          NA |
| DotnetSort         | 1024 | Sorted             |     4,573.5 ns |      85.39 ns |     30.45 ns |  0.74 |    0.06 |    3 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **QuickSort**          | **1024** | **Reversed**           |     **4,683.6 ns** |     **435.62 ns** |    **193.42 ns** |  **1.00** |    **0.05** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 1024 | Reversed           |    38,874.4 ns |     841.17 ns |    439.95 ns |  8.31 |    0.32 |    5 |         - |          NA |
| QuickSortMedian3   | 1024 | Reversed           |    52,033.5 ns |     532.73 ns |    278.63 ns | 11.13 |    0.42 |    5 |         - |          NA |
| QuickSortMedian9   | 1024 | Reversed           |    23,245.8 ns |     383.84 ns |    170.43 ns |  4.97 |    0.19 |    4 |         - |          NA |
| DualPivotQuickSort | 1024 | Reversed           |    24,824.0 ns |     658.26 ns |    344.28 ns |  5.31 |    0.21 |    4 |         - |          NA |
| StableQuickSort    | 1024 | Reversed           |    45,242.8 ns |     712.12 ns |    372.45 ns |  9.67 |    0.37 |    5 |         - |          NA |
| IntroSort          | 1024 | Reversed           |     3,881.7 ns |      35.11 ns |     12.52 ns |  0.83 |    0.03 |    2 |         - |          NA |
| IntroSortDotnet    | 1024 | Reversed           |     8,123.3 ns |     118.99 ns |     62.23 ns |  1.74 |    0.07 |    3 |         - |          NA |
| PDQSort            | 1024 | Reversed           |     1,928.3 ns |      77.92 ns |     27.79 ns |  0.41 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless  | 1024 | Reversed           |     3,303.1 ns |      22.03 ns |      7.86 ns |  0.71 |    0.03 |    2 |         - |          NA |
| StdSort            | 1024 | Reversed           |     3,379.3 ns |      12.27 ns |      4.38 ns |  0.72 |    0.03 |    2 |         - |          NA |
| BlockQuickSort     | 1024 | Reversed           |     7,775.7 ns |      71.04 ns |     31.54 ns |  1.66 |    0.06 |    3 |         - |          NA |
| DotnetSort         | 1024 | Reversed           |     7,333.3 ns |     435.96 ns |    228.02 ns |  1.57 |    0.07 |    3 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **QuickSort**          | **1024** | **PipeOrgan**          |    **97,668.4 ns** |     **861.91 ns** |    **450.80 ns** |  **1.00** |    **0.01** |    **6** |         **-** |          **NA** |
| QuickSort3way      | 1024 | PipeOrgan          |    35,335.2 ns |     326.36 ns |    144.91 ns |  0.36 |    0.00 |    5 |         - |          NA |
| QuickSortMedian3   | 1024 | PipeOrgan          |    38,473.5 ns |   1,016.96 ns |    531.89 ns |  0.39 |    0.01 |    5 |         - |          NA |
| QuickSortMedian9   | 1024 | PipeOrgan          |    22,050.5 ns |     453.62 ns |    201.41 ns |  0.23 |    0.00 |    4 |         - |          NA |
| DualPivotQuickSort | 1024 | PipeOrgan          |    11,649.1 ns |     202.81 ns |     90.05 ns |  0.12 |    0.00 |    2 |         - |          NA |
| StableQuickSort    | 1024 | PipeOrgan          |    45,442.6 ns |     127.91 ns |     45.61 ns |  0.47 |    0.00 |    5 |         - |          NA |
| IntroSort          | 1024 | PipeOrgan          |    15,028.6 ns |     522.40 ns |    231.95 ns |  0.15 |    0.00 |    3 |         - |          NA |
| IntroSortDotnet    | 1024 | PipeOrgan          |    14,935.5 ns |     339.78 ns |    150.87 ns |  0.15 |    0.00 |    3 |         - |          NA |
| PDQSort            | 1024 | PipeOrgan          |     9,490.1 ns |     502.97 ns |    263.06 ns |  0.10 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 1024 | PipeOrgan          |    18,466.4 ns |     391.14 ns |    173.67 ns |  0.19 |    0.00 |    4 |         - |          NA |
| StdSort            | 1024 | PipeOrgan          |    21,011.1 ns |     566.14 ns |    296.10 ns |  0.22 |    0.00 |    4 |         - |          NA |
| BlockQuickSort     | 1024 | PipeOrgan          |    24,575.6 ns |     600.42 ns |    314.03 ns |  0.25 |    0.00 |    4 |         - |          NA |
| DotnetSort         | 1024 | PipeOrgan          |    15,089.4 ns |     955.03 ns |    499.50 ns |  0.15 |    0.00 |    3 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **QuickSort**          | **8192** | **Random**             |   **427,997.0 ns** |   **2,623.41 ns** |  **1,372.09 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| QuickSort3way      | 8192 | Random             |   425,898.1 ns |   2,859.75 ns |  1,495.70 ns |  1.00 |    0.00 |    1 |         - |          NA |
| QuickSortMedian3   | 8192 | Random             |   530,280.9 ns |   2,320.37 ns |  1,030.26 ns |  1.24 |    0.00 |    1 |         - |          NA |
| QuickSortMedian9   | 8192 | Random             |   512,542.7 ns |   1,408.07 ns |    736.45 ns |  1.20 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort | 8192 | Random             |   365,426.4 ns |     697.58 ns |    364.85 ns |  0.85 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 8192 | Random             | 1,149,952.0 ns |   2,283.58 ns |  1,013.92 ns |  2.69 |    0.01 |    2 |         - |          NA |
| IntroSort          | 8192 | Random             |   383,558.2 ns |   2,789.55 ns |  1,458.99 ns |  0.90 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 8192 | Random             |   351,114.0 ns |   1,439.76 ns |    753.02 ns |  0.82 |    0.00 |    1 |         - |          NA |
| PDQSort            | 8192 | Random             |   363,524.7 ns |   3,252.78 ns |  1,444.25 ns |  0.85 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | Random             |   466,361.7 ns |   1,794.64 ns |    938.63 ns |  1.09 |    0.00 |    1 |         - |          NA |
| StdSort            | 8192 | Random             |   403,532.2 ns |   1,144.94 ns |    598.82 ns |  0.94 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 8192 | Random             |   440,698.0 ns |   8,235.88 ns |  4,307.52 ns |  1.03 |    0.01 |    1 |         - |          NA |
| DotnetSort         | 8192 | Random             |   344,520.4 ns |   1,124.83 ns |    499.43 ns |  0.80 |    0.00 |    1 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **QuickSort**          | **8192** | **SingleElementMoved** |    **76,109.7 ns** |   **7,070.06 ns** |  **3,697.78 ns** |  **1.00** |    **0.06** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 8192 | SingleElementMoved |   750,150.0 ns |   8,048.29 ns |  3,573.49 ns |  9.88 |    0.45 |    8 |         - |          NA |
| QuickSortMedian3   | 8192 | SingleElementMoved |   572,906.9 ns |   4,245.05 ns |  2,220.24 ns |  7.54 |    0.34 |    7 |         - |          NA |
| QuickSortMedian9   | 8192 | SingleElementMoved |   213,120.9 ns |   4,648.05 ns |  2,431.02 ns |  2.81 |    0.13 |    5 |         - |          NA |
| DualPivotQuickSort | 8192 | SingleElementMoved |   155,093.5 ns |     584.80 ns |    259.65 ns |  2.04 |    0.09 |    4 |         - |          NA |
| StableQuickSort    | 8192 | SingleElementMoved |   433,968.2 ns |   2,609.67 ns |  1,158.71 ns |  5.71 |    0.26 |    6 |         - |          NA |
| IntroSort          | 8192 | SingleElementMoved |    42,835.2 ns |   4,410.54 ns |  2,306.80 ns |  0.56 |    0.04 |    1 |         - |          NA |
| IntroSortDotnet    | 8192 | SingleElementMoved |    64,179.9 ns |     712.79 ns |    372.80 ns |  0.84 |    0.04 |    2 |         - |          NA |
| PDQSort            | 8192 | SingleElementMoved |    44,680.5 ns |   1,829.40 ns |    956.81 ns |  0.59 |    0.03 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | SingleElementMoved |    53,291.5 ns |   1,009.62 ns |    448.28 ns |  0.70 |    0.03 |    1 |         - |          NA |
| StdSort            | 8192 | SingleElementMoved |    93,998.6 ns |     890.74 ns |    465.88 ns |  1.24 |    0.06 |    3 |         - |          NA |
| BlockQuickSort     | 8192 | SingleElementMoved |    92,743.1 ns |     659.37 ns |    292.76 ns |  1.22 |    0.06 |    3 |         - |          NA |
| DotnetSort         | 8192 | SingleElementMoved |    73,332.2 ns |   6,553.05 ns |  3,427.37 ns |  0.97 |    0.06 |    2 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **QuickSort**          | **8192** | **Sorted**             |    **61,383.4 ns** |   **4,448.24 ns** |  **1,975.05 ns** |  **1.00** |    **0.04** |    **4** |         **-** |          **NA** |
| QuickSort3way      | 8192 | Sorted             | 1,023,771.7 ns |   3,837.37 ns |  1,703.82 ns | 16.69 |    0.50 |    8 |         - |          NA |
| QuickSortMedian3   | 8192 | Sorted             |   889,770.7 ns |   5,040.56 ns |  2,238.04 ns | 14.51 |    0.43 |    8 |         - |          NA |
| QuickSortMedian9   | 8192 | Sorted             |   209,994.6 ns |   3,463.87 ns |  1,811.67 ns |  3.42 |    0.11 |    6 |         - |          NA |
| DualPivotQuickSort | 8192 | Sorted             |   175,869.3 ns |   1,696.81 ns |    887.47 ns |  2.87 |    0.09 |    6 |         - |          NA |
| StableQuickSort    | 8192 | Sorted             |   433,227.1 ns |   2,002.28 ns |    889.03 ns |  7.06 |    0.21 |    7 |         - |          NA |
| IntroSort          | 8192 | Sorted             |     8,529.4 ns |     383.12 ns |    170.11 ns |  0.14 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 8192 | Sorted             |    48,951.0 ns |     914.00 ns |    478.04 ns |  0.80 |    0.02 |    3 |         - |          NA |
| PDQSort            | 8192 | Sorted             |     8,151.8 ns |   1,228.99 ns |    545.68 ns |  0.13 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | Sorted             |     8,552.2 ns |   1,210.13 ns |    537.30 ns |  0.14 |    0.01 |    1 |         - |          NA |
| StdSort            | 8192 | Sorted             |    21,200.2 ns |     411.26 ns |    182.60 ns |  0.35 |    0.01 |    2 |         - |          NA |
| BlockQuickSort     | 8192 | Sorted             |    81,083.4 ns |   1,986.19 ns |    881.88 ns |  1.32 |    0.04 |    5 |         - |          NA |
| DotnetSort         | 8192 | Sorted             |    50,106.6 ns |   2,664.56 ns |  1,393.62 ns |  0.82 |    0.03 |    3 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **QuickSort**          | **8192** | **Reversed**           |    **52,578.9 ns** |   **6,155.74 ns** |  **3,219.57 ns** |  **1.00** |    **0.08** |    **4** |         **-** |          **NA** |
| QuickSort3way      | 8192 | Reversed           |   841,210.6 ns |   9,367.50 ns |  4,159.23 ns | 16.05 |    0.95 |    8 |         - |          NA |
| QuickSortMedian3   | 8192 | Reversed           | 1,178,534.4 ns | 138,962.04 ns | 72,679.83 ns | 22.49 |    1.86 |    9 |         - |          NA |
| QuickSortMedian9   | 8192 | Reversed           |   212,634.8 ns |   3,789.47 ns |  1,981.97 ns |  4.06 |    0.24 |    6 |         - |          NA |
| DualPivotQuickSort | 8192 | Reversed           |   179,940.5 ns |   1,725.33 ns |    902.38 ns |  3.43 |    0.20 |    6 |         - |          NA |
| StableQuickSort    | 8192 | Reversed           |   465,872.7 ns |   2,072.25 ns |  1,083.83 ns |  8.89 |    0.52 |    7 |         - |          NA |
| IntroSort          | 8192 | Reversed           |    35,045.5 ns |   1,000.78 ns |    444.35 ns |  0.67 |    0.04 |    3 |         - |          NA |
| IntroSortDotnet    | 8192 | Reversed           |    80,099.7 ns |     229.06 ns |    101.70 ns |  1.53 |    0.09 |    5 |         - |          NA |
| PDQSort            | 8192 | Reversed           |    14,585.9 ns |     255.75 ns |    113.56 ns |  0.28 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | Reversed           |    25,875.8 ns |     441.83 ns |    157.56 ns |  0.49 |    0.03 |    2 |         - |          NA |
| StdSort            | 8192 | Reversed           |    27,245.7 ns |   1,374.52 ns |    718.90 ns |  0.52 |    0.03 |    2 |         - |          NA |
| BlockQuickSort     | 8192 | Reversed           |    89,225.9 ns |  29,689.92 ns | 15,528.40 ns |  1.70 |    0.30 |    5 |         - |          NA |
| DotnetSort         | 8192 | Reversed           |    81,348.5 ns |   3,638.90 ns |  1,903.21 ns |  1.55 |    0.10 |    5 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **QuickSort**          | **8192** | **PipeOrgan**          | **5,428,272.3 ns** | **138,041.70 ns** | **72,198.48 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| QuickSort3way      | 8192 | PipeOrgan          |   509,877.9 ns |   2,739.58 ns |  1,216.39 ns |  0.09 |    0.00 |    2 |         - |          NA |
| QuickSortMedian3   | 8192 | PipeOrgan          |   511,366.1 ns |  31,379.01 ns | 16,411.83 ns |  0.09 |    0.00 |    2 |         - |          NA |
| QuickSortMedian9   | 8192 | PipeOrgan          |   277,555.5 ns |   4,419.43 ns |  2,311.45 ns |  0.05 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort | 8192 | PipeOrgan          |   149,553.3 ns |   1,915.04 ns |  1,001.60 ns |  0.03 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 8192 | PipeOrgan          |   472,612.7 ns |   2,092.18 ns |  1,094.25 ns |  0.09 |    0.00 |    2 |         - |          NA |
| IntroSort          | 8192 | PipeOrgan          |   332,735.8 ns |   5,715.98 ns |  2,989.57 ns |  0.06 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet    | 8192 | PipeOrgan          |   375,764.4 ns |   4,315.23 ns |  1,915.99 ns |  0.07 |    0.00 |    2 |         - |          NA |
| PDQSort            | 8192 | PipeOrgan          |   144,428.1 ns |   2,741.27 ns |  1,433.74 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | PipeOrgan          |   275,766.9 ns |   2,168.19 ns |    962.69 ns |  0.05 |    0.00 |    2 |         - |          NA |
| StdSort            | 8192 | PipeOrgan          |   435,577.7 ns |   2,840.07 ns |  1,485.41 ns |  0.08 |    0.00 |    2 |         - |          NA |
| BlockQuickSort     | 8192 | PipeOrgan          |   269,147.0 ns |   2,947.20 ns |  1,541.44 ns |  0.05 |    0.00 |    2 |         - |          NA |
| DotnetSort         | 8192 | PipeOrgan          |   366,936.2 ns |   7,532.51 ns |  3,939.65 ns |  0.07 |    0.00 |    2 |         - |          NA |

### MergeBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.78GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                   | Size | Pattern            | Mean           | Error        | StdDev      | Median         | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------------- |----- |------------------- |---------------:|-------------:|------------:|---------------:|------:|--------:|-----:|----------:|------------:|
| **MergeSort**                | **256**  | **Random**             |     **8,300.3 ns** |    **565.34 ns** |   **295.69 ns** |     **8,115.4 ns** |  **1.00** |    **0.05** |    **2** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Random             |     8,351.8 ns |    573.64 ns |   300.02 ns |     8,438.0 ns |  1.01 |    0.05 |    2 |         - |          NA |
| BottomupMergeSort        | 256  | Random             |     4,844.1 ns |    470.41 ns |   246.04 ns |     4,729.0 ns |  0.58 |    0.03 |    2 |         - |          NA |
| StdStableSort            | 256  | Random             |     3,009.6 ns |     67.26 ns |    23.98 ns |     3,006.8 ns |  0.36 |    0.01 |    1 |         - |          NA |
| RotateMergeSort          | 256  | Random             |     9,467.5 ns |    472.39 ns |   247.07 ns |     9,446.4 ns |  1.14 |    0.05 |    2 |         - |          NA |
| RotateMergeSortRecursive | 256  | Random             |    11,209.7 ns |    485.22 ns |   253.78 ns |    11,194.3 ns |  1.35 |    0.05 |    2 |         - |          NA |
| SymMergeSort             | 256  | Random             |     6,880.3 ns |    274.43 ns |   143.53 ns |     6,834.0 ns |  0.83 |    0.03 |    2 |         - |          NA |
| BlockMergeSort           | 256  | Random             |     5,847.3 ns |     57.14 ns |    25.37 ns |     5,840.2 ns |  0.71 |    0.02 |    2 |         - |          NA |
| NaturalMergeSort         | 256  | Random             |     5,276.1 ns |    513.63 ns |   268.64 ns |     5,129.0 ns |  0.64 |    0.04 |    2 |         - |          NA |
| TimSort                  | 256  | Random             |     3,794.6 ns |    154.11 ns |    54.96 ns |     3,768.0 ns |  0.46 |    0.02 |    2 |         - |          NA |
| PowerSort                | 256  | Random             |     2,363.7 ns |     53.70 ns |    23.84 ns |     2,353.3 ns |  0.29 |    0.01 |    1 |         - |          NA |
| ShiftSort                | 256  | Random             |     4,053.8 ns |    200.46 ns |    89.00 ns |     4,011.0 ns |  0.49 |    0.02 |    2 |         - |          NA |
| SpinSort                 | 256  | Random             |     2,209.0 ns |    477.86 ns |   212.17 ns |     2,088.2 ns |  0.27 |    0.03 |    1 |         - |          NA |
| SpinSortVariant          | 256  | Random             |     2,377.0 ns |     43.27 ns |    15.43 ns |     2,380.1 ns |  0.29 |    0.01 |    1 |         - |          NA |
| Glidesort                | 256  | Random             |     4,958.9 ns |    247.03 ns |   129.20 ns |     4,897.6 ns |  0.60 |    0.02 |    2 |         - |          NA |
| FlatStableSort           | 256  | Random             |     2,676.1 ns |    100.21 ns |    44.49 ns |     2,663.5 ns |  0.32 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |                |       |         |      |           |             |
| **MergeSort**                | **256**  | **SingleElementMoved** |     **4,532.0 ns** |    **518.62 ns** |   **271.25 ns** |     **4,497.8 ns** |  **1.00** |    **0.08** |    **9** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | SingleElementMoved |     5,362.6 ns |    434.17 ns |   227.08 ns |     5,215.2 ns |  1.19 |    0.08 |   10 |         - |          NA |
| BottomupMergeSort        | 256  | SingleElementMoved |     2,379.6 ns |    194.28 ns |    86.26 ns |     2,338.4 ns |  0.53 |    0.03 |    8 |         - |          NA |
| StdStableSort            | 256  | SingleElementMoved |     1,872.9 ns |     10.28 ns |     4.56 ns |     1,871.6 ns |  0.41 |    0.02 |    7 |         - |          NA |
| RotateMergeSort          | 256  | SingleElementMoved |       582.5 ns |      7.73 ns |     4.04 ns |       581.0 ns |  0.13 |    0.01 |    3 |         - |          NA |
| RotateMergeSortRecursive | 256  | SingleElementMoved |       719.6 ns |      7.17 ns |     3.75 ns |       718.4 ns |  0.16 |    0.01 |    4 |         - |          NA |
| SymMergeSort             | 256  | SingleElementMoved |       517.0 ns |      7.58 ns |     3.36 ns |       516.0 ns |  0.11 |    0.01 |    3 |         - |          NA |
| BlockMergeSort           | 256  | SingleElementMoved |     4,247.2 ns |     57.55 ns |    20.52 ns |     4,239.0 ns |  0.94 |    0.05 |    9 |         - |          NA |
| NaturalMergeSort         | 256  | SingleElementMoved |       598.8 ns |      5.65 ns |     2.01 ns |       598.7 ns |  0.13 |    0.01 |    3 |         - |          NA |
| TimSort                  | 256  | SingleElementMoved |       313.0 ns |     10.40 ns |     5.44 ns |       309.9 ns |  0.07 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | SingleElementMoved |       411.9 ns |      1.11 ns |     0.40 ns |       411.9 ns |  0.09 |    0.01 |    2 |         - |          NA |
| ShiftSort                | 256  | SingleElementMoved |       400.6 ns |     11.45 ns |     5.99 ns |       400.8 ns |  0.09 |    0.01 |    2 |         - |          NA |
| SpinSort                 | 256  | SingleElementMoved |       912.1 ns |     27.15 ns |    14.20 ns |       908.6 ns |  0.20 |    0.01 |    5 |         - |          NA |
| SpinSortVariant          | 256  | SingleElementMoved |       953.0 ns |     16.17 ns |     5.77 ns |       952.2 ns |  0.21 |    0.01 |    5 |         - |          NA |
| Glidesort                | 256  | SingleElementMoved |     1,270.6 ns |     11.64 ns |     5.17 ns |     1,269.2 ns |  0.28 |    0.02 |    6 |         - |          NA |
| FlatStableSort           | 256  | SingleElementMoved |     1,254.2 ns |     14.75 ns |     7.71 ns |     1,251.7 ns |  0.28 |    0.02 |    6 |         - |          NA |
|      |                    |                |              |             |                |       |         |      |           |             |
| **MergeSort**                | **256**  | **Sorted**             |     **3,858.4 ns** |      **6.68 ns** |     **2.38 ns** |     **3,858.7 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Sorted             |     4,948.0 ns |    311.85 ns |   163.11 ns |     4,837.7 ns |  1.28 |    0.04 |    9 |         - |          NA |
| BottomupMergeSort        | 256  | Sorted             |     1,957.9 ns |     23.36 ns |     8.33 ns |     1,955.0 ns |  0.51 |    0.00 |    7 |         - |          NA |
| StdStableSort            | 256  | Sorted             |     1,704.6 ns |     11.70 ns |     6.12 ns |     1,703.9 ns |  0.44 |    0.00 |    7 |         - |          NA |
| RotateMergeSort          | 256  | Sorted             |       347.3 ns |      1.96 ns |     0.87 ns |       346.9 ns |  0.09 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 256  | Sorted             |       465.7 ns |      1.46 ns |     0.65 ns |       465.9 ns |  0.12 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 256  | Sorted             |       470.4 ns |    343.37 ns |   179.59 ns |       341.4 ns |  0.12 |    0.04 |    5 |         - |          NA |
| BlockMergeSort           | 256  | Sorted             |     3,239.5 ns |      6.13 ns |     2.72 ns |     3,238.5 ns |  0.84 |    0.00 |    8 |         - |          NA |
| NaturalMergeSort         | 256  | Sorted             |       218.1 ns |      3.63 ns |     1.61 ns |       218.1 ns |  0.06 |    0.00 |    2 |         - |          NA |
| TimSort                  | 256  | Sorted             |       190.8 ns |      3.14 ns |     1.39 ns |       191.3 ns |  0.05 |    0.00 |    2 |         - |          NA |
| PowerSort                | 256  | Sorted             |       150.9 ns |      1.18 ns |     0.62 ns |       151.0 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Sorted             |       214.8 ns |      3.94 ns |     1.75 ns |       214.3 ns |  0.06 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 256  | Sorted             |       132.4 ns |      1.29 ns |     0.67 ns |       132.3 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 256  | Sorted             |       182.5 ns |      0.55 ns |     0.20 ns |       182.4 ns |  0.05 |    0.00 |    2 |         - |          NA |
| Glidesort                | 256  | Sorted             |       188.9 ns |      4.31 ns |     1.91 ns |       189.3 ns |  0.05 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 256  | Sorted             |     1,114.9 ns |     10.03 ns |     4.45 ns |     1,112.7 ns |  0.29 |    0.00 |    6 |         - |          NA |
|      |                    |                |              |             |                |       |         |      |           |             |
| **MergeSort**                | **256**  | **Reversed**           |     **8,758.7 ns** |    **398.63 ns** |   **208.49 ns** |     **8,803.4 ns** |  **1.00** |    **0.03** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Reversed           |     8,161.8 ns |    333.89 ns |   174.63 ns |     8,205.8 ns |  0.93 |    0.03 |    4 |         - |          NA |
| BottomupMergeSort        | 256  | Reversed           |     5,137.6 ns |    194.34 ns |   101.64 ns |     5,067.8 ns |  0.59 |    0.02 |    3 |         - |          NA |
| StdStableSort            | 256  | Reversed           |     2,480.8 ns |    292.75 ns |   153.11 ns |     2,485.6 ns |  0.28 |    0.02 |    2 |         - |          NA |
| RotateMergeSort          | 256  | Reversed           |     1,772.5 ns |      2.67 ns |     0.95 ns |     1,772.5 ns |  0.20 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 256  | Reversed           |     1,860.6 ns |      2.79 ns |     1.46 ns |     1,859.9 ns |  0.21 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 256  | Reversed           |     2,093.2 ns |    348.83 ns |   182.45 ns |     1,967.9 ns |  0.24 |    0.02 |    2 |         - |          NA |
| BlockMergeSort           | 256  | Reversed           |     9,402.7 ns |    325.23 ns |   170.10 ns |     9,422.0 ns |  1.07 |    0.03 |    4 |         - |          NA |
| NaturalMergeSort         | 256  | Reversed           |       339.9 ns |     38.95 ns |    17.29 ns |       330.3 ns |  0.04 |    0.00 |    1 |         - |          NA |
| TimSort                  | 256  | Reversed           |       267.7 ns |      6.34 ns |     3.32 ns |       267.9 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | Reversed           |       265.6 ns |     67.83 ns |    35.48 ns |       262.2 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Reversed           |       257.0 ns |      6.55 ns |     3.43 ns |       256.1 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 256  | Reversed           |       257.9 ns |      1.19 ns |     0.62 ns |       257.9 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 256  | Reversed           |       288.2 ns |      3.29 ns |     1.46 ns |       287.7 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 256  | Reversed           |       281.5 ns |      2.25 ns |     1.00 ns |       281.8 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | Reversed           |     2,895.2 ns |      8.88 ns |     3.17 ns |     2,894.5 ns |  0.33 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |                |       |         |      |           |             |
| **MergeSort**                | **256**  | **PipeOrgan**          |     **6,554.1 ns** |    **239.24 ns** |   **125.13 ns** |     **6,600.6 ns** |  **1.00** |    **0.03** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | PipeOrgan          |     6,725.4 ns |    252.74 ns |   132.19 ns |     6,745.0 ns |  1.03 |    0.03 |    6 |         - |          NA |
| BottomupMergeSort        | 256  | PipeOrgan          |     3,765.5 ns |     24.42 ns |     8.71 ns |     3,764.6 ns |  0.57 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 256  | PipeOrgan          |     2,298.4 ns |    445.81 ns |   233.17 ns |     2,193.4 ns |  0.35 |    0.03 |    4 |         - |          NA |
| RotateMergeSort          | 256  | PipeOrgan          |     4,199.4 ns |    300.42 ns |   157.13 ns |     4,113.0 ns |  0.64 |    0.03 |    5 |         - |          NA |
| RotateMergeSortRecursive | 256  | PipeOrgan          |     5,094.9 ns |    491.35 ns |   256.99 ns |     4,936.5 ns |  0.78 |    0.04 |    5 |         - |          NA |
| SymMergeSort             | 256  | PipeOrgan          |     2,522.8 ns |     13.58 ns |     4.84 ns |     2,522.4 ns |  0.39 |    0.01 |    4 |         - |          NA |
| BlockMergeSort           | 256  | PipeOrgan          |     6,503.0 ns |    269.93 ns |   141.18 ns |     6,578.9 ns |  0.99 |    0.03 |    6 |         - |          NA |
| NaturalMergeSort         | 256  | PipeOrgan          |       927.4 ns |     76.09 ns |    39.79 ns |       927.5 ns |  0.14 |    0.01 |    2 |         - |          NA |
| TimSort                  | 256  | PipeOrgan          |       846.5 ns |     13.98 ns |     6.21 ns |       843.9 ns |  0.13 |    0.00 |    2 |         - |          NA |
| PowerSort                | 256  | PipeOrgan          |       508.4 ns |      3.35 ns |     1.49 ns |       507.8 ns |  0.08 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | PipeOrgan          |       542.4 ns |      5.43 ns |     2.41 ns |       541.8 ns |  0.08 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 256  | PipeOrgan          |     1,918.4 ns |    310.04 ns |   162.16 ns |     1,884.2 ns |  0.29 |    0.02 |    4 |         - |          NA |
| SpinSortVariant          | 256  | PipeOrgan          |     1,876.4 ns |     13.41 ns |     4.78 ns |     1,874.8 ns |  0.29 |    0.01 |    4 |         - |          NA |
| Glidesort                | 256  | PipeOrgan          |     1,224.1 ns |     11.15 ns |     3.98 ns |     1,222.9 ns |  0.19 |    0.00 |    3 |         - |          NA |
| FlatStableSort           | 256  | PipeOrgan          |     2,314.2 ns |    287.46 ns |   150.35 ns |     2,372.1 ns |  0.35 |    0.02 |    4 |         - |          NA |
|      |                    |                |              |             |                |       |         |      |           |             |
| **MergeSort**                | **1024** | **Random**             |    **37,008.2 ns** |  **2,036.64 ns** | **1,065.20 ns** |    **36,653.6 ns** |  **1.00** |    **0.04** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Random             |    35,609.2 ns |    819.61 ns |   428.67 ns |    35,488.8 ns |  0.96 |    0.03 |    3 |         - |          NA |
| BottomupMergeSort        | 1024 | Random             |    22,521.8 ns |    589.36 ns |   308.25 ns |    22,433.6 ns |  0.61 |    0.02 |    2 |         - |          NA |
| StdStableSort            | 1024 | Random             |    15,426.3 ns |    239.65 ns |   106.40 ns |    15,462.5 ns |  0.42 |    0.01 |    1 |         - |          NA |
| RotateMergeSort          | 1024 | Random             |    69,032.6 ns |  5,751.92 ns | 3,008.37 ns |    67,771.3 ns |  1.87 |    0.09 |    4 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Random             |    66,751.0 ns |    689.95 ns |   246.04 ns |    66,866.3 ns |  1.80 |    0.05 |    4 |         - |          NA |
| SymMergeSort             | 1024 | Random             |    41,231.6 ns |    600.93 ns |   266.82 ns |    41,267.0 ns |  1.11 |    0.03 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | Random             |    33,094.8 ns |    578.16 ns |   302.39 ns |    33,019.1 ns |  0.89 |    0.02 |    3 |         - |          NA |
| NaturalMergeSort         | 1024 | Random             |    27,418.2 ns |  1,652.82 ns |   864.46 ns |    27,506.9 ns |  0.74 |    0.03 |    2 |         - |          NA |
| TimSort                  | 1024 | Random             |    19,491.2 ns |    356.88 ns |   158.46 ns |    19,568.5 ns |  0.53 |    0.01 |    2 |         - |          NA |
| PowerSort                | 1024 | Random             |    13,273.3 ns |    720.47 ns |   376.82 ns |    13,229.2 ns |  0.36 |    0.01 |    1 |         - |          NA |
| ShiftSort                | 1024 | Random             |    20,485.2 ns |    816.04 ns |   426.80 ns |    20,510.4 ns |  0.55 |    0.02 |    2 |         - |          NA |
| SpinSort                 | 1024 | Random             |    13,658.5 ns |    568.68 ns |   297.43 ns |    13,550.4 ns |  0.37 |    0.01 |    1 |         - |          NA |
| SpinSortVariant          | 1024 | Random             |    14,344.8 ns |    944.80 ns |   419.50 ns |    14,451.4 ns |  0.39 |    0.01 |    1 |         - |          NA |
| Glidesort                | 1024 | Random             |    24,654.3 ns |    665.25 ns |   347.94 ns |    24,585.8 ns |  0.67 |    0.02 |    2 |         - |          NA |
| FlatStableSort           | 1024 | Random             |    14,844.9 ns |    319.67 ns |   167.19 ns |    14,812.9 ns |  0.40 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |                |       |         |      |           |             |
| **MergeSort**                | **1024** | **SingleElementMoved** |    **17,376.2 ns** |    **291.83 ns** |   **104.07 ns** |    **17,348.8 ns** |  **1.00** |    **0.01** |    **9** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | SingleElementMoved |    20,892.4 ns |    361.05 ns |   188.84 ns |    20,820.5 ns |  1.20 |    0.01 |    9 |         - |          NA |
| BottomupMergeSort        | 1024 | SingleElementMoved |     7,653.0 ns |    697.49 ns |   364.80 ns |     7,728.9 ns |  0.44 |    0.02 |    8 |         - |          NA |
| StdStableSort            | 1024 | SingleElementMoved |     8,520.2 ns |    386.96 ns |   202.39 ns |     8,555.4 ns |  0.49 |    0.01 |    8 |         - |          NA |
| RotateMergeSort          | 1024 | SingleElementMoved |     1,943.6 ns |     16.22 ns |     5.78 ns |     1,942.9 ns |  0.11 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 1024 | SingleElementMoved |     2,619.2 ns |    415.34 ns |   217.23 ns |     2,528.0 ns |  0.15 |    0.01 |    4 |         - |          NA |
| SymMergeSort             | 1024 | SingleElementMoved |     1,693.8 ns |      8.84 ns |     3.15 ns |     1,692.4 ns |  0.10 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | SingleElementMoved |    19,936.6 ns |    230.51 ns |   102.35 ns |    19,962.4 ns |  1.15 |    0.01 |    9 |         - |          NA |
| NaturalMergeSort         | 1024 | SingleElementMoved |     2,232.4 ns |    627.17 ns |   278.47 ns |     2,026.4 ns |  0.13 |    0.02 |    3 |         - |          NA |
| TimSort                  | 1024 | SingleElementMoved |       829.8 ns |      5.85 ns |     2.60 ns |       830.0 ns |  0.05 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | SingleElementMoved |     1,382.7 ns |      6.71 ns |     2.98 ns |     1,382.4 ns |  0.08 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 1024 | SingleElementMoved |     1,380.7 ns |     46.63 ns |    20.70 ns |     1,372.4 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 1024 | SingleElementMoved |     4,204.3 ns |     29.81 ns |    13.24 ns |     4,196.5 ns |  0.24 |    0.00 |    6 |         - |          NA |
| SpinSortVariant          | 1024 | SingleElementMoved |     3,429.0 ns |     37.75 ns |    13.46 ns |     3,424.1 ns |  0.20 |    0.00 |    5 |         - |          NA |
| Glidesort                | 1024 | SingleElementMoved |     2,599.0 ns |      8.37 ns |     3.72 ns |     2,597.2 ns |  0.15 |    0.00 |    4 |         - |          NA |
| FlatStableSort           | 1024 | SingleElementMoved |     5,327.8 ns |     26.61 ns |    11.81 ns |     5,324.8 ns |  0.31 |    0.00 |    7 |         - |          NA |
|      |                    |                |              |             |                |       |         |      |           |             |
| **MergeSort**                | **1024** | **Sorted**             |    **15,679.5 ns** |    **257.03 ns** |   **114.12 ns** |    **15,627.8 ns** |  **1.00** |    **0.01** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Sorted             |    19,660.1 ns |    210.63 ns |    93.52 ns |    19,694.1 ns |  1.25 |    0.01 |    9 |         - |          NA |
| BottomupMergeSort        | 1024 | Sorted             |     5,936.5 ns |    252.21 ns |   111.98 ns |     5,884.8 ns |  0.38 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 1024 | Sorted             |     7,430.5 ns |     35.92 ns |    15.95 ns |     7,433.0 ns |  0.47 |    0.00 |    7 |         - |          NA |
| RotateMergeSort          | 1024 | Sorted             |     1,341.0 ns |      1.87 ns |     0.83 ns |     1,341.1 ns |  0.09 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Sorted             |     1,953.0 ns |    432.46 ns |   192.01 ns |     1,860.4 ns |  0.12 |    0.01 |    4 |         - |          NA |
| SymMergeSort             | 1024 | Sorted             |     1,316.7 ns |     11.71 ns |     5.20 ns |     1,314.6 ns |  0.08 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | Sorted             |    14,119.3 ns |    115.25 ns |    60.28 ns |    14,112.9 ns |  0.90 |    0.01 |    8 |         - |          NA |
| NaturalMergeSort         | 1024 | Sorted             |       589.6 ns |      4.56 ns |     2.03 ns |       589.4 ns |  0.04 |    0.00 |    1 |         - |          NA |
| TimSort                  | 1024 | Sorted             |       565.1 ns |     12.24 ns |     5.43 ns |       563.6 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Sorted             |       523.6 ns |      2.14 ns |     0.95 ns |       523.5 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Sorted             |       859.4 ns |    143.85 ns |    63.87 ns |       878.0 ns |  0.05 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 1024 | Sorted             |       465.4 ns |      3.83 ns |     2.00 ns |       465.8 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 1024 | Sorted             |       657.8 ns |      4.05 ns |     1.45 ns |       657.8 ns |  0.04 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Sorted             |       504.2 ns |     13.87 ns |     7.25 ns |       506.6 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Sorted             |     4,845.3 ns |     25.15 ns |     8.97 ns |     4,846.6 ns |  0.31 |    0.00 |    5 |         - |          NA |
|      |                    |                |              |             |                |       |         |      |           |             |
| **MergeSort**                | **1024** | **Reversed**           |    **36,349.5 ns** |    **457.62 ns** |   **203.19 ns** |    **36,454.4 ns** |  **1.00** |    **0.01** |    **5** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Reversed           |    33,323.5 ns |    427.57 ns |   223.63 ns |    33,328.8 ns |  0.92 |    0.01 |    5 |         - |          NA |
| BottomupMergeSort        | 1024 | Reversed           |    19,887.7 ns |    707.06 ns |   369.81 ns |    19,900.2 ns |  0.55 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 1024 | Reversed           |    10,884.4 ns |    322.55 ns |   168.70 ns |    10,933.0 ns |  0.30 |    0.00 |    3 |         - |          NA |
| RotateMergeSort          | 1024 | Reversed           |     8,618.2 ns |    576.91 ns |   301.74 ns |     8,497.8 ns |  0.24 |    0.01 |    2 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Reversed           |     9,044.6 ns |    415.15 ns |   217.13 ns |     9,084.8 ns |  0.25 |    0.01 |    2 |         - |          NA |
| SymMergeSort             | 1024 | Reversed           |     8,569.8 ns |    371.78 ns |   165.07 ns |     8,670.5 ns |  0.24 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 1024 | Reversed           |    39,793.2 ns |    398.61 ns |   176.98 ns |    39,814.6 ns |  1.09 |    0.01 |    5 |         - |          NA |
| NaturalMergeSort         | 1024 | Reversed           |     1,044.8 ns |     14.30 ns |     6.35 ns |     1,042.0 ns |  0.03 |    0.00 |    1 |         - |          NA |
| TimSort                  | 1024 | Reversed           |       852.3 ns |     26.00 ns |     9.27 ns |       848.2 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Reversed           |       811.4 ns |      3.52 ns |     1.84 ns |       811.5 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Reversed           |     1,143.3 ns |    314.94 ns |   139.83 ns |     1,182.0 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 1024 | Reversed           |       954.7 ns |      2.11 ns |     0.93 ns |       954.9 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 1024 | Reversed           |     1,057.6 ns |      1.37 ns |     0.61 ns |     1,057.6 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Reversed           |       860.9 ns |      5.15 ns |     2.29 ns |       860.7 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Reversed           |    12,668.6 ns |    275.67 ns |   144.18 ns |    12,656.5 ns |  0.35 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |             |                |       |         |      |           |             |
| **MergeSort**                | **1024** | **PipeOrgan**          |    **26,646.8 ns** |    **723.34 ns** |   **321.17 ns** |    **26,839.8 ns** |  **1.00** |    **0.02** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | PipeOrgan          |    26,901.3 ns |    592.85 ns |   310.07 ns |    26,951.4 ns |  1.01 |    0.02 |    7 |         - |          NA |
| BottomupMergeSort        | 1024 | PipeOrgan          |    14,220.3 ns |    194.75 ns |   101.86 ns |    14,184.2 ns |  0.53 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 1024 | PipeOrgan          |     9,935.0 ns |    619.26 ns |   323.89 ns |     9,897.0 ns |  0.37 |    0.01 |    4 |         - |          NA |
| RotateMergeSort          | 1024 | PipeOrgan          |    18,166.3 ns |    505.45 ns |   264.36 ns |    18,129.0 ns |  0.68 |    0.01 |    6 |         - |          NA |
| RotateMergeSortRecursive | 1024 | PipeOrgan          |    21,370.8 ns |    504.04 ns |   263.62 ns |    21,279.3 ns |  0.80 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 1024 | PipeOrgan          |    11,367.4 ns |    543.56 ns |   284.29 ns |    11,413.5 ns |  0.43 |    0.01 |    4 |         - |          NA |
| BlockMergeSort           | 1024 | PipeOrgan          |    32,397.1 ns |    280.46 ns |   146.69 ns |    32,333.9 ns |  1.22 |    0.01 |    8 |         - |          NA |
| NaturalMergeSort         | 1024 | PipeOrgan          |     2,379.3 ns |     20.99 ns |     9.32 ns |     2,374.0 ns |  0.09 |    0.00 |    2 |         - |          NA |
| TimSort                  | 1024 | PipeOrgan          |     2,542.8 ns |      6.27 ns |     2.24 ns |     2,543.0 ns |  0.10 |    0.00 |    2 |         - |          NA |
| PowerSort                | 1024 | PipeOrgan          |     1,649.8 ns |     16.80 ns |     5.99 ns |     1,651.7 ns |  0.06 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | PipeOrgan          |     2,114.1 ns |    526.65 ns |   233.84 ns |     1,940.5 ns |  0.08 |    0.01 |    1 |         - |          NA |
| SpinSort                 | 1024 | PipeOrgan          |     8,130.3 ns |    490.58 ns |   256.58 ns |     8,160.2 ns |  0.31 |    0.01 |    4 |         - |          NA |
| SpinSortVariant          | 1024 | PipeOrgan          |     7,591.0 ns |     18.60 ns |     8.26 ns |     7,586.7 ns |  0.28 |    0.00 |    4 |         - |          NA |
| Glidesort                | 1024 | PipeOrgan          |     4,513.5 ns |    384.88 ns |   201.30 ns |     4,381.6 ns |  0.17 |    0.01 |    3 |         - |          NA |
| FlatStableSort           | 1024 | PipeOrgan          |     8,867.5 ns |    326.48 ns |   170.75 ns |     8,845.9 ns |  0.33 |    0.01 |    4 |         - |          NA |
|      |                    |                |              |             |                |       |         |      |           |             |
| **MergeSort**                | **8192** | **Random**             |   **681,137.3 ns** |  **5,468.61 ns** | **2,860.19 ns** |   **682,045.7 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Random             |   614,221.4 ns |  3,387.65 ns | 1,771.81 ns |   613,850.2 ns |  0.90 |    0.00 |    1 |         - |          NA |
| BottomupMergeSort        | 8192 | Random             |   498,809.4 ns |  8,465.19 ns | 3,758.60 ns |   499,540.5 ns |  0.73 |    0.01 |    1 |         - |          NA |
| StdStableSort            | 8192 | Random             |   471,529.8 ns |    942.07 ns |   418.28 ns |   471,531.0 ns |  0.69 |    0.00 |    1 |         - |          NA |
| RotateMergeSort          | 8192 | Random             | 1,343,907.3 ns | 15,271.62 ns | 6,780.69 ns | 1,340,835.1 ns |  1.97 |    0.01 |    3 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Random             | 1,464,058.5 ns |  4,982.73 ns | 2,212.36 ns | 1,463,859.5 ns |  2.15 |    0.01 |    3 |         - |          NA |
| SymMergeSort             | 8192 | Random             | 1,018,378.4 ns |  2,930.12 ns | 1,300.99 ns | 1,017,710.3 ns |  1.50 |    0.01 |    2 |         - |          NA |
| BlockMergeSort           | 8192 | Random             |   701,099.8 ns |  2,227.25 ns | 1,164.89 ns |   701,171.9 ns |  1.03 |    0.00 |    1 |         - |          NA |
| NaturalMergeSort         | 8192 | Random             |   618,849.1 ns |  1,879.29 ns |   834.42 ns |   618,700.8 ns |  0.91 |    0.00 |    1 |         - |          NA |
| TimSort                  | 8192 | Random             |   565,067.3 ns |    784.78 ns |   410.45 ns |   564,852.7 ns |  0.83 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Random             |   427,606.8 ns |  3,622.23 ns | 1,894.50 ns |   427,520.3 ns |  0.63 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Random             |   561,821.5 ns |  1,484.36 ns |   659.07 ns |   561,497.8 ns |  0.82 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 8192 | Random             |   370,044.8 ns |  2,422.69 ns | 1,075.69 ns |   370,273.4 ns |  0.54 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 8192 | Random             |   371,414.4 ns |  3,071.19 ns | 1,606.29 ns |   371,012.6 ns |  0.55 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Random             |   586,899.3 ns |  2,403.42 ns | 1,067.13 ns |   586,694.3 ns |  0.86 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Random             |   398,108.7 ns |    656.65 ns |   291.56 ns |   398,102.0 ns |  0.58 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |             |                |       |         |      |           |             |
| **MergeSort**                | **8192** | **SingleElementMoved** |   **136,040.7 ns** |  **1,071.13 ns** |   **560.22 ns** |   **135,942.8 ns** |  **1.00** |    **0.01** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | SingleElementMoved |   169,121.7 ns |  1,488.29 ns |   778.40 ns |   169,160.4 ns |  1.24 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 8192 | SingleElementMoved |    56,354.8 ns |  1,709.36 ns |   894.03 ns |    55,973.3 ns |  0.41 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 8192 | SingleElementMoved |   110,341.3 ns |  1,116.04 ns |   583.71 ns |   110,377.5 ns |  0.81 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 8192 | SingleElementMoved |    14,253.6 ns |    158.90 ns |    83.11 ns |    14,264.9 ns |  0.10 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 8192 | SingleElementMoved |    17,848.8 ns |  1,343.61 ns |   596.57 ns |    17,590.6 ns |  0.13 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 8192 | SingleElementMoved |    12,729.0 ns |    288.15 ns |   150.71 ns |    12,786.7 ns |  0.09 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 8192 | SingleElementMoved |   149,684.2 ns |    476.01 ns |   211.35 ns |   149,661.9 ns |  1.10 |    0.00 |    6 |         - |          NA |
| NaturalMergeSort         | 8192 | SingleElementMoved |    15,900.9 ns |    229.57 ns |   101.93 ns |    15,896.9 ns |  0.12 |    0.00 |    2 |         - |          NA |
| TimSort                  | 8192 | SingleElementMoved |     5,521.3 ns |      7.48 ns |     2.67 ns |     5,522.1 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | SingleElementMoved |    10,577.0 ns |    681.14 ns |   356.25 ns |    10,590.0 ns |  0.08 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 8192 | SingleElementMoved |    10,650.4 ns |    552.23 ns |   288.83 ns |    10,662.9 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | SingleElementMoved |    23,164.4 ns |  1,547.03 ns |   809.13 ns |    23,331.2 ns |  0.17 |    0.01 |    2 |         - |          NA |
| SpinSortVariant          | 8192 | SingleElementMoved |    20,186.5 ns |  1,358.46 ns |   710.50 ns |    19,970.0 ns |  0.15 |    0.00 |    2 |         - |          NA |
| Glidesort                | 8192 | SingleElementMoved |    20,343.5 ns |    212.95 ns |    94.55 ns |    20,363.0 ns |  0.15 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 8192 | SingleElementMoved |    45,896.0 ns |    523.38 ns |   186.64 ns |    45,943.9 ns |  0.34 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |             |                |       |         |      |           |             |
| **MergeSort**                | **8192** | **Sorted**             |   **124,718.2 ns** |    **758.12 ns** |   **270.35 ns** |   **124,822.6 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Sorted             |   159,387.3 ns |  1,096.20 ns |   573.33 ns |   159,520.7 ns |  1.28 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 8192 | Sorted             |    45,356.6 ns |  1,173.74 ns |   613.89 ns |    45,122.3 ns |  0.36 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 8192 | Sorted             |   105,890.7 ns |  1,257.71 ns |   558.43 ns |   106,066.1 ns |  0.85 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 8192 | Sorted             |    11,293.0 ns |    363.76 ns |   161.51 ns |    11,350.2 ns |  0.09 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Sorted             |    15,206.0 ns |    114.65 ns |    40.88 ns |    15,199.4 ns |  0.12 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 8192 | Sorted             |    11,011.8 ns |    892.74 ns |   466.92 ns |    10,859.9 ns |  0.09 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 8192 | Sorted             |   110,662.9 ns |  1,019.37 ns |   533.15 ns |   110,815.0 ns |  0.89 |    0.00 |    5 |         - |          NA |
| NaturalMergeSort         | 8192 | Sorted             |     4,499.1 ns |    852.44 ns |   378.49 ns |     4,287.3 ns |  0.04 |    0.00 |    1 |         - |          NA |
| TimSort                  | 8192 | Sorted             |     4,083.0 ns |     15.70 ns |     5.60 ns |     4,084.8 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Sorted             |     4,408.1 ns |    614.69 ns |   321.49 ns |     4,419.9 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Sorted             |     5,262.2 ns |    339.30 ns |   150.65 ns |     5,179.5 ns |  0.04 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 8192 | Sorted             |     3,856.5 ns |    628.64 ns |   279.12 ns |     3,827.4 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 8192 | Sorted             |     5,176.4 ns |    298.91 ns |   132.72 ns |     5,099.8 ns |  0.04 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Sorted             |     3,742.5 ns |    424.77 ns |   188.60 ns |     3,795.3 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Sorted             |     3,821.3 ns |    450.36 ns |   235.54 ns |     3,671.8 ns |  0.03 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |             |                |       |         |      |           |             |
| **MergeSort**                | **8192** | **Reversed**           |   **305,065.7 ns** |  **1,938.22 ns** |   **860.58 ns** |   **305,422.5 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Reversed           |   273,131.7 ns |  1,771.33 ns |   926.44 ns |   273,080.5 ns |  0.90 |    0.00 |    5 |         - |          NA |
| BottomupMergeSort        | 8192 | Reversed           |   168,228.1 ns |  3,618.79 ns | 1,606.77 ns |   168,313.6 ns |  0.55 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 8192 | Reversed           |   130,708.5 ns |  1,226.63 ns |   544.63 ns |   130,663.7 ns |  0.43 |    0.00 |    3 |         - |          NA |
| RotateMergeSort          | 8192 | Reversed           |    83,566.9 ns |  1,553.42 ns |   689.73 ns |    83,535.6 ns |  0.27 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Reversed           |    88,696.0 ns |  2,547.42 ns | 1,131.07 ns |    88,210.6 ns |  0.29 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 8192 | Reversed           |    74,962.0 ns |    987.11 ns |   516.28 ns |    74,787.8 ns |  0.25 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 8192 | Reversed           |   340,331.0 ns |  2,697.66 ns | 1,197.78 ns |   339,985.3 ns |  1.12 |    0.00 |    5 |         - |          NA |
| NaturalMergeSort         | 8192 | Reversed           |     7,858.4 ns |    420.70 ns |   186.79 ns |     7,717.5 ns |  0.03 |    0.00 |    1 |         - |          NA |
| TimSort                  | 8192 | Reversed           |     6,565.5 ns |    448.07 ns |   234.35 ns |     6,534.4 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Reversed           |     6,579.2 ns |  1,198.16 ns |   531.99 ns |     6,379.0 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Reversed           |     6,353.6 ns |  1,147.22 ns |   509.37 ns |     6,130.5 ns |  0.02 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 8192 | Reversed           |     7,655.3 ns |    348.06 ns |   182.04 ns |     7,648.8 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 8192 | Reversed           |     8,780.1 ns |    460.98 ns |   204.68 ns |     8,694.6 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Reversed           |     6,340.1 ns |    248.55 ns |   129.99 ns |     6,259.0 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Reversed           |     7,104.4 ns |    514.70 ns |   228.53 ns |     6,939.0 ns |  0.02 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |             |                |       |         |      |           |             |
| **MergeSort**                | **8192** | **PipeOrgan**          |   **220,224.7 ns** |  **1,691.10 ns** |   **884.48 ns** |   **220,351.4 ns** |  **1.00** |    **0.01** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | PipeOrgan          |   223,326.1 ns |  1,844.61 ns |   964.77 ns |   223,671.8 ns |  1.01 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 8192 | PipeOrgan          |   115,664.2 ns |  1,076.91 ns |   563.24 ns |   115,696.4 ns |  0.53 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 8192 | PipeOrgan          |   127,732.3 ns |  1,685.19 ns |   881.39 ns |   127,748.3 ns |  0.58 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 8192 | PipeOrgan          |   158,980.8 ns |    584.84 ns |   208.56 ns |   159,037.1 ns |  0.72 |    0.00 |    6 |         - |          NA |
| RotateMergeSortRecursive | 8192 | PipeOrgan          |   186,504.4 ns |  1,571.50 ns |   821.92 ns |   186,185.5 ns |  0.85 |    0.00 |    6 |         - |          NA |
| SymMergeSort             | 8192 | PipeOrgan          |    99,459.5 ns |  1,600.15 ns |   836.91 ns |    99,566.3 ns |  0.45 |    0.00 |    5 |         - |          NA |
| BlockMergeSort           | 8192 | PipeOrgan          |   250,152.9 ns |  1,402.64 ns |   733.61 ns |   250,009.5 ns |  1.14 |    0.01 |    6 |         - |          NA |
| NaturalMergeSort         | 8192 | PipeOrgan          |    19,269.9 ns |    884.12 ns |   462.41 ns |    19,197.1 ns |  0.09 |    0.00 |    2 |         - |          NA |
| TimSort                  | 8192 | PipeOrgan          |    18,720.0 ns |    347.69 ns |   154.38 ns |    18,651.2 ns |  0.09 |    0.00 |    2 |         - |          NA |
| PowerSort                | 8192 | PipeOrgan          |    12,164.3 ns |    356.59 ns |   158.33 ns |    12,070.2 ns |  0.06 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | PipeOrgan          |    15,398.5 ns |    646.22 ns |   286.93 ns |    15,284.2 ns |  0.07 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | PipeOrgan          |    18,309.6 ns |    762.38 ns |   398.74 ns |    18,427.4 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSortVariant          | 8192 | PipeOrgan          |    19,071.7 ns |    646.04 ns |   286.84 ns |    19,136.5 ns |  0.09 |    0.00 |    2 |         - |          NA |
| Glidesort                | 8192 | PipeOrgan          |    34,371.9 ns |    310.46 ns |   110.71 ns |    34,356.7 ns |  0.16 |    0.00 |    3 |         - |          NA |
| FlatStableSort           | 8192 | PipeOrgan          |    79,601.5 ns |    829.78 ns |   433.99 ns |    79,646.1 ns |  0.36 |    0.00 |    4 |         - |          NA |

### NetworkBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.78GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                  | Size | Pattern            | Mean         | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------------ |----- |------------------- |-------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **BitonicSort**             | **256**  | **Random**             |  **11,250.2 ns** |    **382.13 ns** |    **169.67 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Random             |  22,505.0 ns |     79.97 ns |     35.51 ns |  2.00 |    0.03 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Random             |  16,522.3 ns |     90.31 ns |     40.10 ns |  1.47 |    0.02 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BitonicSort**             | **256**  | **SingleElementMoved** |  **10,225.7 ns** |    **370.30 ns** |    **193.67 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | SingleElementMoved |  23,095.5 ns |    284.22 ns |    148.65 ns |  2.26 |    0.04 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | SingleElementMoved |  16,711.0 ns |    157.59 ns |     69.97 ns |  1.63 |    0.03 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BitonicSort**             | **256**  | **Sorted**             |  **10,177.1 ns** |    **426.70 ns** |    **223.17 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Sorted             |  22,983.8 ns |    451.45 ns |    236.12 ns |  2.26 |    0.05 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Sorted             |  16,795.6 ns |    307.76 ns |    160.97 ns |  1.65 |    0.04 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BitonicSort**             | **256**  | **Reversed**           |  **10,091.2 ns** |    **500.61 ns** |    **222.27 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Reversed           |  22,862.7 ns |     73.98 ns |     38.69 ns |  2.27 |    0.05 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Reversed           |  16,795.0 ns |    124.35 ns |     55.21 ns |  1.67 |    0.03 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BitonicSort**             | **256**  | **PipeOrgan**          |   **9,249.6 ns** |    **773.90 ns** |    **343.61 ns** |  **1.00** |    **0.05** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | PipeOrgan          |  22,516.8 ns |    222.88 ns |    116.57 ns |  2.44 |    0.08 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | PipeOrgan          |  16,750.2 ns |    150.05 ns |     66.62 ns |  1.81 |    0.06 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Random**             |  **94,777.1 ns** |  **1,314.78 ns** |    **687.66 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Random             | 125,019.0 ns |  3,245.98 ns |  1,157.55 ns |  1.32 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Random             | 102,558.6 ns |    238.19 ns |    124.58 ns |  1.08 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BitonicSort**             | **1024** | **SingleElementMoved** |  **58,770.2 ns** |  **1,239.71 ns** |    **648.39 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | SingleElementMoved | 118,506.5 ns |    673.12 ns |    352.06 ns |  2.02 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | SingleElementMoved | 102,651.6 ns |    201.47 ns |     89.45 ns |  1.75 |    0.02 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Sorted**             |  **58,733.0 ns** |  **2,381.47 ns** |  **1,245.56 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Sorted             | 118,910.3 ns |    582.71 ns |    304.77 ns |  2.03 |    0.04 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Sorted             | 102,739.5 ns |    563.02 ns |    249.98 ns |  1.75 |    0.03 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Reversed**           |  **57,384.6 ns** |    **665.69 ns** |    **348.17 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Reversed           | 118,183.3 ns |    314.55 ns |    139.66 ns |  2.06 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Reversed           | 102,670.5 ns |    370.23 ns |    193.64 ns |  1.79 |    0.01 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BitonicSort**             | **1024** | **PipeOrgan**          |  **52,984.4 ns** |  **2,541.37 ns** |  **1,128.38 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | PipeOrgan          | 116,751.8 ns |  1,118.11 ns |    496.45 ns |  2.20 |    0.04 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | PipeOrgan          | 102,487.9 ns |    223.87 ns |    117.09 ns |  1.94 |    0.04 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Random**             | **547,538.9 ns** |  **3,692.53 ns** |  **1,639.51 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Random             | 817,391.4 ns |  1,771.22 ns |    786.43 ns |  1.49 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Random             | 645,786.6 ns |  7,340.57 ns |  3,839.26 ns |  1.18 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BitonicSort**             | **4096** | **SingleElementMoved** | **318,135.0 ns** |  **1,570.08 ns** |    **559.90 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | SingleElementMoved | 592,160.4 ns |  3,841.94 ns |  2,009.41 ns |  1.86 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | SingleElementMoved | 584,948.7 ns |    455.20 ns |    202.11 ns |  1.84 |    0.00 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Sorted**             | **321,030.5 ns** |  **3,906.92 ns** |  **2,043.40 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Sorted             | 591,014.3 ns |  1,907.19 ns |    997.50 ns |  1.84 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Sorted             | 585,959.2 ns |  2,194.19 ns |    974.23 ns |  1.83 |    0.01 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Reversed**           | **314,610.4 ns** |  **4,095.42 ns** |  **2,141.98 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Reversed           | 622,355.0 ns | 85,000.49 ns | 44,456.90 ns |  1.98 |    0.13 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Reversed           | 585,173.1 ns |    609.44 ns |    318.75 ns |  1.86 |    0.01 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BitonicSort**             | **4096** | **PipeOrgan**          | **299,941.1 ns** | **10,065.10 ns** |  **4,468.96 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | PipeOrgan          | 580,553.9 ns |  1,905.01 ns |    996.36 ns |  1.94 |    0.03 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | PipeOrgan          | 585,263.3 ns |  1,333.14 ns |    591.92 ns |  1.95 |    0.03 |    2 |         - |          NA |

### PartitionBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.78GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                       | Size | Pattern            | Mean           | Error         | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------------- |----- |------------------- |---------------:|--------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **QuickSort**                    | **256**  | **Random**             |     **2,568.2 ns** |      **16.09 ns** |      **5.74 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 256  | Random             |     3,222.6 ns |     343.78 ns |    179.80 ns |  1.25 |    0.07 |    1 |         - |          NA |
| QuickSortMedian3             | 256  | Random             |     4,356.1 ns |      58.06 ns |     20.71 ns |  1.70 |    0.01 |    2 |         - |          NA |
| QuickSortMedian9             | 256  | Random             |     3,819.0 ns |     284.72 ns |    126.42 ns |  1.49 |    0.05 |    2 |         - |          NA |
| DualPivotQuickSort           | 256  | Random             |     2,203.1 ns |      39.08 ns |     17.35 ns |  0.86 |    0.01 |    1 |         - |          NA |
| StableQuickSort              | 256  | Random             |    11,739.3 ns |     317.13 ns |    165.86 ns |  4.57 |    0.06 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Random             |     6,896.9 ns |      75.01 ns |     39.23 ns |  2.69 |    0.02 |    3 |         - |          NA |
| DestswapStableQuickSort      | 256  | Random             |     8,785.7 ns |     562.82 ns |    294.36 ns |  3.42 |    0.11 |    4 |         - |          NA |
| IntroSort                    | 256  | Random             |     1,936.1 ns |      24.78 ns |     11.00 ns |  0.75 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | Random             |     1,618.1 ns |      24.46 ns |     10.86 ns |  0.63 |    0.00 |    1 |         - |          NA |
| PDQSort                      | 256  | Random             |     1,716.9 ns |      61.20 ns |     27.17 ns |  0.67 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | Random             |     2,902.7 ns |      42.71 ns |     18.96 ns |  1.13 |    0.01 |    1 |         - |          NA |
| StdSort                      | 256  | Random             |     1,883.1 ns |     494.89 ns |    258.84 ns |  0.73 |    0.10 |    1 |         - |          NA |
| BlockQuickSort               | 256  | Random             |     2,326.2 ns |      52.13 ns |     23.15 ns |  0.91 |    0.01 |    1 |         - |          NA |
| DotnetSort                   | 256  | Random             |     1,859.0 ns |      43.38 ns |     19.26 ns |  0.72 |    0.01 |    1 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **SingleElementMoved** |     **1,236.6 ns** |      **17.72 ns** |      **7.87 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 256  | SingleElementMoved |     5,342.3 ns |     402.15 ns |    210.33 ns |  4.32 |    0.16 |    3 |         - |          NA |
| QuickSortMedian3             | 256  | SingleElementMoved |     5,242.3 ns |     454.42 ns |    201.77 ns |  4.24 |    0.15 |    3 |         - |          NA |
| QuickSortMedian9             | 256  | SingleElementMoved |     4,468.4 ns |     554.01 ns |    289.76 ns |  3.61 |    0.22 |    3 |         - |          NA |
| DualPivotQuickSort           | 256  | SingleElementMoved |     3,615.6 ns |       7.73 ns |      2.76 ns |  2.92 |    0.02 |    2 |         - |          NA |
| StableQuickSort              | 256  | SingleElementMoved |     8,855.0 ns |     503.04 ns |    263.10 ns |  7.16 |    0.21 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 256  | SingleElementMoved |     4,967.7 ns |     438.18 ns |    229.17 ns |  4.02 |    0.18 |    3 |         - |          NA |
| DestswapStableQuickSort      | 256  | SingleElementMoved |    10,632.8 ns |     307.13 ns |    160.63 ns |  8.60 |    0.13 |    5 |         - |          NA |
| IntroSort                    | 256  | SingleElementMoved |       869.0 ns |      34.26 ns |     15.21 ns |  0.70 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | SingleElementMoved |     1,087.8 ns |      26.62 ns |      9.49 ns |  0.88 |    0.01 |    1 |         - |          NA |
| PDQSort                      | 256  | SingleElementMoved |     1,107.2 ns |      23.01 ns |     10.22 ns |  0.90 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | SingleElementMoved |     1,477.6 ns |      62.69 ns |     27.83 ns |  1.19 |    0.02 |    1 |         - |          NA |
| StdSort                      | 256  | SingleElementMoved |     1,530.6 ns |      26.78 ns |     11.89 ns |  1.24 |    0.01 |    1 |         - |          NA |
| BlockQuickSort               | 256  | SingleElementMoved |     1,406.3 ns |      24.09 ns |     10.70 ns |  1.14 |    0.01 |    1 |         - |          NA |
| DotnetSort                   | 256  | SingleElementMoved |     1,003.2 ns |      18.00 ns |      6.42 ns |  0.81 |    0.01 |    1 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Sorted**             |       **871.5 ns** |      **10.65 ns** |      **4.73 ns** |  **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 256  | Sorted             |     7,249.5 ns |     207.83 ns |    108.70 ns |  8.32 |    0.12 |    6 |         - |          NA |
| QuickSortMedian3             | 256  | Sorted             |     6,269.6 ns |      81.46 ns |     36.17 ns |  7.19 |    0.05 |    6 |         - |          NA |
| QuickSortMedian9             | 256  | Sorted             |     4,774.2 ns |     476.98 ns |    249.47 ns |  5.48 |    0.27 |    5 |         - |          NA |
| DualPivotQuickSort           | 256  | Sorted             |     4,412.0 ns |     379.54 ns |    198.50 ns |  5.06 |    0.22 |    5 |         - |          NA |
| StableQuickSort              | 256  | Sorted             |     8,760.3 ns |     427.19 ns |    223.43 ns | 10.05 |    0.25 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Sorted             |     4,943.3 ns |     385.50 ns |    201.63 ns |  5.67 |    0.22 |    5 |         - |          NA |
| DestswapStableQuickSort      | 256  | Sorted             |    10,199.7 ns |     378.65 ns |    198.04 ns | 11.70 |    0.22 |    7 |         - |          NA |
| IntroSort                    | 256  | Sorted             |       300.6 ns |      12.84 ns |      5.70 ns |  0.34 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | Sorted             |     1,045.9 ns |      20.88 ns |      7.45 ns |  1.20 |    0.01 |    4 |         - |          NA |
| PDQSort                      | 256  | Sorted             |       371.6 ns |       1.35 ns |      0.60 ns |  0.43 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 256  | Sorted             |       374.4 ns |       3.70 ns |      1.64 ns |  0.43 |    0.00 |    2 |         - |          NA |
| StdSort                      | 256  | Sorted             |       490.1 ns |       2.05 ns |      0.91 ns |  0.56 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 256  | Sorted             |     1,216.6 ns |      17.11 ns |      7.60 ns |  1.40 |    0.01 |    4 |         - |          NA |
| DotnetSort                   | 256  | Sorted             |       877.7 ns |      23.57 ns |     12.33 ns |  1.01 |    0.01 |    4 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Reversed**           |       **989.0 ns** |      **21.71 ns** |      **9.64 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 256  | Reversed           |     5,105.3 ns |     295.58 ns |    154.60 ns |  5.16 |    0.15 |    5 |         - |          NA |
| QuickSortMedian3             | 256  | Reversed           |     7,402.9 ns |     490.85 ns |    217.94 ns |  7.49 |    0.22 |    6 |         - |          NA |
| QuickSortMedian9             | 256  | Reversed           |     4,957.2 ns |     402.64 ns |    210.59 ns |  5.01 |    0.21 |    5 |         - |          NA |
| DualPivotQuickSort           | 256  | Reversed           |     3,973.8 ns |     504.49 ns |    263.86 ns |  4.02 |    0.25 |    4 |         - |          NA |
| StableQuickSort              | 256  | Reversed           |     8,749.3 ns |     342.28 ns |    151.97 ns |  8.85 |    0.16 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Reversed           |     5,002.2 ns |     501.48 ns |    262.28 ns |  5.06 |    0.25 |    5 |         - |          NA |
| DestswapStableQuickSort      | 256  | Reversed           |    10,236.0 ns |     434.33 ns |    227.16 ns | 10.35 |    0.24 |    6 |         - |          NA |
| IntroSort                    | 256  | Reversed           |       566.1 ns |      51.58 ns |     22.90 ns |  0.57 |    0.02 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | Reversed           |     1,459.2 ns |      22.42 ns |     11.73 ns |  1.48 |    0.02 |    3 |         - |          NA |
| PDQSort                      | 256  | Reversed           |       552.4 ns |       6.73 ns |      3.52 ns |  0.56 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | Reversed           |       923.4 ns |      19.66 ns |      8.73 ns |  0.93 |    0.01 |    2 |         - |          NA |
| StdSort                      | 256  | Reversed           |       652.0 ns |       7.39 ns |      3.28 ns |  0.66 |    0.01 |    1 |         - |          NA |
| BlockQuickSort               | 256  | Reversed           |     1,449.3 ns |      16.97 ns |      7.54 ns |  1.47 |    0.02 |    3 |         - |          NA |
| DotnetSort                   | 256  | Reversed           |     1,551.2 ns |      43.32 ns |     19.23 ns |  1.57 |    0.02 |    3 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **PipeOrgan**          |     **7,772.6 ns** |      **85.78 ns** |     **38.09 ns** |  **1.00** |    **0.01** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 256  | PipeOrgan          |     4,975.2 ns |     498.54 ns |    260.75 ns |  0.64 |    0.03 |    4 |         - |          NA |
| QuickSortMedian3             | 256  | PipeOrgan          |     6,743.0 ns |     414.83 ns |    216.96 ns |  0.87 |    0.03 |    5 |         - |          NA |
| QuickSortMedian9             | 256  | PipeOrgan          |     4,130.9 ns |     260.93 ns |    115.85 ns |  0.53 |    0.01 |    4 |         - |          NA |
| DualPivotQuickSort           | 256  | PipeOrgan          |     1,701.8 ns |      25.68 ns |     11.40 ns |  0.22 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 256  | PipeOrgan          |     9,423.3 ns |     674.43 ns |    299.45 ns |  1.21 |    0.04 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 256  | PipeOrgan          |     4,988.2 ns |     725.89 ns |    379.66 ns |  0.64 |    0.05 |    4 |         - |          NA |
| DestswapStableQuickSort      | 256  | PipeOrgan          |    11,019.8 ns |     445.86 ns |    233.19 ns |  1.42 |    0.03 |    6 |         - |          NA |
| IntroSort                    | 256  | PipeOrgan          |     1,649.3 ns |      44.00 ns |     19.53 ns |  0.21 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | PipeOrgan          |     2,331.8 ns |     386.56 ns |    202.18 ns |  0.30 |    0.02 |    2 |         - |          NA |
| PDQSort                      | 256  | PipeOrgan          |     1,698.3 ns |      62.44 ns |     22.26 ns |  0.22 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | PipeOrgan          |     2,973.7 ns |      56.33 ns |     25.01 ns |  0.38 |    0.00 |    3 |         - |          NA |
| StdSort                      | 256  | PipeOrgan          |     2,293.8 ns |     393.94 ns |    174.91 ns |  0.30 |    0.02 |    2 |         - |          NA |
| BlockQuickSort               | 256  | PipeOrgan          |     4,268.1 ns |     462.38 ns |    205.30 ns |  0.55 |    0.02 |    4 |         - |          NA |
| DotnetSort                   | 256  | PipeOrgan          |     2,936.9 ns |     422.71 ns |    221.08 ns |  0.38 |    0.03 |    3 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Random**             |    **13,282.7 ns** |     **542.51 ns** |    **283.74 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Random             |    19,664.3 ns |   1,188.69 ns |    621.71 ns |  1.48 |    0.05 |    2 |         - |          NA |
| QuickSortMedian3             | 1024 | Random             |    23,064.0 ns |     206.77 ns |     91.81 ns |  1.74 |    0.04 |    2 |         - |          NA |
| QuickSortMedian9             | 1024 | Random             |    21,683.4 ns |   4,916.91 ns |  2,571.64 ns |  1.63 |    0.19 |    2 |         - |          NA |
| DualPivotQuickSort           | 1024 | Random             |    10,381.1 ns |     352.43 ns |    184.33 ns |  0.78 |    0.02 |    1 |         - |          NA |
| StableQuickSort              | 1024 | Random             |    84,601.4 ns |     500.84 ns |    222.38 ns |  6.37 |    0.13 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Random             |    34,651.9 ns |     426.31 ns |    189.28 ns |  2.61 |    0.05 |    3 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Random             |    39,549.4 ns |     485.16 ns |    215.41 ns |  2.98 |    0.06 |    3 |         - |          NA |
| IntroSort                    | 1024 | Random             |    11,431.4 ns |     698.63 ns |    365.40 ns |  0.86 |    0.03 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | Random             |     9,105.1 ns |     610.23 ns |    319.16 ns |  0.69 |    0.03 |    1 |         - |          NA |
| PDQSort                      | 1024 | Random             |     8,972.0 ns |     541.38 ns |    283.15 ns |  0.68 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | Random             |    13,748.9 ns |     138.83 ns |     72.61 ns |  1.04 |    0.02 |    1 |         - |          NA |
| StdSort                      | 1024 | Random             |     8,902.5 ns |     396.75 ns |    207.51 ns |  0.67 |    0.02 |    1 |         - |          NA |
| BlockQuickSort               | 1024 | Random             |    13,078.5 ns |     654.13 ns |    290.44 ns |  0.99 |    0.03 |    1 |         - |          NA |
| DotnetSort                   | 1024 | Random             |    10,634.1 ns |     855.61 ns |    447.50 ns |  0.80 |    0.04 |    1 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **SingleElementMoved** |     **5,655.5 ns** |      **88.13 ns** |     **46.10 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 1024 | SingleElementMoved |    39,872.7 ns |   1,928.64 ns |    856.33 ns |  7.05 |    0.15 |    4 |         - |          NA |
| QuickSortMedian3             | 1024 | SingleElementMoved |    31,506.0 ns |     314.27 ns |    164.37 ns |  5.57 |    0.05 |    3 |         - |          NA |
| QuickSortMedian9             | 1024 | SingleElementMoved |    21,799.2 ns |     636.29 ns |    282.52 ns |  3.85 |    0.06 |    2 |         - |          NA |
| DualPivotQuickSort           | 1024 | SingleElementMoved |    21,634.5 ns |     994.81 ns |    441.70 ns |  3.83 |    0.08 |    2 |         - |          NA |
| StableQuickSort              | 1024 | SingleElementMoved |    42,256.1 ns |     408.81 ns |    181.52 ns |  7.47 |    0.06 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | SingleElementMoved |    23,510.0 ns |   1,072.14 ns |    476.04 ns |  4.16 |    0.08 |    2 |         - |          NA |
| DestswapStableQuickSort      | 1024 | SingleElementMoved |    45,178.4 ns |   2,032.48 ns |  1,063.02 ns |  7.99 |    0.19 |    4 |         - |          NA |
| IntroSort                    | 1024 | SingleElementMoved |     4,478.5 ns |     721.87 ns |    377.55 ns |  0.79 |    0.06 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | SingleElementMoved |     6,107.0 ns |     308.69 ns |    161.45 ns |  1.08 |    0.03 |    1 |         - |          NA |
| PDQSort                      | 1024 | SingleElementMoved |     4,901.0 ns |      37.75 ns |     13.46 ns |  0.87 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | SingleElementMoved |     6,538.8 ns |     333.98 ns |    174.68 ns |  1.16 |    0.03 |    1 |         - |          NA |
| StdSort                      | 1024 | SingleElementMoved |     7,066.4 ns |      92.55 ns |     48.40 ns |  1.25 |    0.01 |    1 |         - |          NA |
| BlockQuickSort               | 1024 | SingleElementMoved |     8,016.7 ns |     292.47 ns |    152.97 ns |  1.42 |    0.03 |    1 |         - |          NA |
| DotnetSort                   | 1024 | SingleElementMoved |     5,817.1 ns |     704.89 ns |    368.67 ns |  1.03 |    0.06 |    1 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Sorted**             |     **4,195.1 ns** |     **238.71 ns** |    **124.85 ns** |  **1.00** |    **0.04** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Sorted             |    53,003.3 ns |     816.49 ns |    427.04 ns | 12.64 |    0.36 |    6 |         - |          NA |
| QuickSortMedian3             | 1024 | Sorted             |    43,833.2 ns |   1,198.41 ns |    626.79 ns | 10.46 |    0.32 |    6 |         - |          NA |
| QuickSortMedian9             | 1024 | Sorted             |    22,247.5 ns |     139.48 ns |     72.95 ns |  5.31 |    0.15 |    5 |         - |          NA |
| DualPivotQuickSort           | 1024 | Sorted             |    21,643.7 ns |     356.95 ns |    158.49 ns |  5.16 |    0.15 |    5 |         - |          NA |
| StableQuickSort              | 1024 | Sorted             |    42,728.7 ns |     478.96 ns |    250.50 ns | 10.19 |    0.28 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Sorted             |    22,895.7 ns |     534.05 ns |    237.12 ns |  5.46 |    0.16 |    5 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Sorted             |    44,233.1 ns |     697.22 ns |    309.57 ns | 10.55 |    0.30 |    6 |         - |          NA |
| IntroSort                    | 1024 | Sorted             |     1,107.9 ns |       3.26 ns |      1.70 ns |  0.26 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | Sorted             |     4,868.9 ns |     392.89 ns |    174.45 ns |  1.16 |    0.05 |    3 |         - |          NA |
| PDQSort                      | 1024 | Sorted             |     1,323.2 ns |       4.00 ns |      2.09 ns |  0.32 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | Sorted             |     1,617.7 ns |     340.16 ns |    177.91 ns |  0.39 |    0.04 |    2 |         - |          NA |
| StdSort                      | 1024 | Sorted             |     1,946.0 ns |     374.82 ns |    196.04 ns |  0.46 |    0.05 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | Sorted             |     6,484.8 ns |     243.33 ns |    127.27 ns |  1.55 |    0.05 |    4 |         - |          NA |
| DotnetSort                   | 1024 | Sorted             |     4,168.6 ns |      25.16 ns |     11.17 ns |  0.99 |    0.03 |    3 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Reversed**           |     **4,613.0 ns** |      **51.58 ns** |     **18.39 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Reversed           |    38,486.6 ns |     235.50 ns |    123.17 ns |  8.34 |    0.04 |    6 |         - |          NA |
| QuickSortMedian3             | 1024 | Reversed           |    52,511.3 ns |   1,804.11 ns |    801.04 ns | 11.38 |    0.17 |    6 |         - |          NA |
| QuickSortMedian9             | 1024 | Reversed           |    22,593.5 ns |     832.30 ns |    369.54 ns |  4.90 |    0.08 |    5 |         - |          NA |
| DualPivotQuickSort           | 1024 | Reversed           |    20,293.6 ns |     742.24 ns |    388.20 ns |  4.40 |    0.08 |    5 |         - |          NA |
| StableQuickSort              | 1024 | Reversed           |    42,578.8 ns |     684.82 ns |    358.17 ns |  9.23 |    0.08 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Reversed           |    22,734.8 ns |     211.81 ns |     75.53 ns |  4.93 |    0.02 |    5 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Reversed           |    43,725.4 ns |     408.05 ns |    181.18 ns |  9.48 |    0.05 |    6 |         - |          NA |
| IntroSort                    | 1024 | Reversed           |     3,152.9 ns |     534.31 ns |    279.45 ns |  0.68 |    0.06 |    2 |         - |          NA |
| IntroSortDotnet              | 1024 | Reversed           |     7,290.0 ns |      28.69 ns |     12.74 ns |  1.58 |    0.01 |    4 |         - |          NA |
| PDQSort                      | 1024 | Reversed           |     1,897.9 ns |       6.64 ns |      2.37 ns |  0.41 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | Reversed           |     3,052.6 ns |      17.06 ns |      7.57 ns |  0.66 |    0.00 |    2 |         - |          NA |
| StdSort                      | 1024 | Reversed           |     2,665.4 ns |     300.15 ns |    133.27 ns |  0.58 |    0.03 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | Reversed           |     7,503.4 ns |     340.50 ns |    178.09 ns |  1.63 |    0.04 |    4 |         - |          NA |
| DotnetSort                   | 1024 | Reversed           |     8,632.4 ns |     689.96 ns |    360.86 ns |  1.87 |    0.07 |    4 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **PipeOrgan**          |    **98,381.4 ns** |   **1,310.13 ns** |    **581.71 ns** |  **1.00** |    **0.01** |    **6** |         **-** |          **NA** |
| QuickSort3way                | 1024 | PipeOrgan          |    31,494.0 ns |     319.30 ns |    141.77 ns |  0.32 |    0.00 |    4 |         - |          NA |
| QuickSortMedian3             | 1024 | PipeOrgan          |    37,747.7 ns |     582.32 ns |    258.55 ns |  0.38 |    0.00 |    4 |         - |          NA |
| QuickSortMedian9             | 1024 | PipeOrgan          |    22,289.1 ns |     653.90 ns |    342.00 ns |  0.23 |    0.00 |    3 |         - |          NA |
| DualPivotQuickSort           | 1024 | PipeOrgan          |     9,447.5 ns |     628.21 ns |    328.57 ns |  0.10 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 1024 | PipeOrgan          |    45,399.2 ns |     169.69 ns |     75.34 ns |  0.46 |    0.00 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | PipeOrgan          |    22,413.7 ns |     532.38 ns |    236.38 ns |  0.23 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 1024 | PipeOrgan          |    49,416.0 ns |   1,058.89 ns |    553.82 ns |  0.50 |    0.01 |    5 |         - |          NA |
| IntroSort                    | 1024 | PipeOrgan          |    10,859.7 ns |     409.57 ns |    181.85 ns |  0.11 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | PipeOrgan          |    13,806.0 ns |     473.18 ns |    247.48 ns |  0.14 |    0.00 |    2 |         - |          NA |
| PDQSort                      | 1024 | PipeOrgan          |     8,766.0 ns |     209.11 ns |    109.37 ns |  0.09 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | PipeOrgan          |    16,226.9 ns |     288.79 ns |    151.04 ns |  0.16 |    0.00 |    2 |         - |          NA |
| StdSort                      | 1024 | PipeOrgan          |    13,397.5 ns |     489.56 ns |    256.05 ns |  0.14 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | PipeOrgan          |    23,203.4 ns |     413.65 ns |    216.35 ns |  0.24 |    0.00 |    3 |         - |          NA |
| DotnetSort                   | 1024 | PipeOrgan          |    16,880.9 ns |     226.03 ns |    100.36 ns |  0.17 |    0.00 |    2 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Random**             |   **423,001.5 ns** |   **5,352.88 ns** |  **2,799.66 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Random             |   440,253.5 ns |   2,778.55 ns |  1,233.69 ns |  1.04 |    0.01 |    2 |         - |          NA |
| QuickSortMedian3             | 8192 | Random             |   534,410.6 ns |   1,755.89 ns |    779.63 ns |  1.26 |    0.01 |    2 |         - |          NA |
| QuickSortMedian9             | 8192 | Random             |   517,363.8 ns |   2,144.50 ns |  1,121.61 ns |  1.22 |    0.01 |    2 |         - |          NA |
| DualPivotQuickSort           | 8192 | Random             |   349,156.6 ns |   1,539.70 ns |    805.29 ns |  0.83 |    0.01 |    2 |         - |          NA |
| StableQuickSort              | 8192 | Random             | 1,160,269.0 ns |     934.46 ns |    488.74 ns |  2.74 |    0.02 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Random             |   944,901.7 ns |   2,721.32 ns |  1,423.30 ns |  2.23 |    0.01 |    3 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Random             |   846,311.1 ns |   1,316.41 ns |    688.51 ns |  2.00 |    0.01 |    3 |         - |          NA |
| IntroSort                    | 8192 | Random             |   366,692.5 ns |   3,283.15 ns |  1,717.15 ns |  0.87 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 8192 | Random             |   355,973.5 ns |   1,782.14 ns |    932.10 ns |  0.84 |    0.01 |    2 |         - |          NA |
| PDQSort                      | 8192 | Random             |   343,788.3 ns |   2,178.97 ns |    967.48 ns |  0.81 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | Random             |   195,090.8 ns |   1,865.29 ns |    975.58 ns |  0.46 |    0.00 |    1 |         - |          NA |
| StdSort                      | 8192 | Random             |   338,237.4 ns |   1,451.83 ns |    644.62 ns |  0.80 |    0.01 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | Random             |   422,823.7 ns |   1,049.35 ns |    465.92 ns |  1.00 |    0.01 |    2 |         - |          NA |
| DotnetSort                   | 8192 | Random             |   334,796.1 ns |   2,153.03 ns |  1,126.08 ns |  0.79 |    0.01 |    2 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **SingleElementMoved** |    **54,714.8 ns** |   **2,502.19 ns** |  **1,110.99 ns** |  **1.00** |    **0.03** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 8192 | SingleElementMoved |   856,822.0 ns |   7,744.61 ns |  3,438.66 ns | 15.67 |    0.30 |    8 |         - |          NA |
| QuickSortMedian3             | 8192 | SingleElementMoved |   572,104.9 ns |   4,691.04 ns |  2,453.50 ns | 10.46 |    0.20 |    7 |         - |          NA |
| QuickSortMedian9             | 8192 | SingleElementMoved |   212,464.8 ns |   5,826.53 ns |  2,587.02 ns |  3.88 |    0.09 |    5 |         - |          NA |
| DualPivotQuickSort           | 8192 | SingleElementMoved |   141,187.2 ns |   5,308.67 ns |  2,776.54 ns |  2.58 |    0.07 |    4 |         - |          NA |
| StableQuickSort              | 8192 | SingleElementMoved |   434,495.5 ns |   4,025.60 ns |  1,787.39 ns |  7.94 |    0.15 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | SingleElementMoved |   227,152.7 ns |   3,151.70 ns |  1,399.37 ns |  4.15 |    0.08 |    5 |         - |          NA |
| DestswapStableQuickSort      | 8192 | SingleElementMoved |   394,837.0 ns |  10,234.51 ns |  5,352.85 ns |  7.22 |    0.16 |    6 |         - |          NA |
| IntroSort                    | 8192 | SingleElementMoved |    41,516.3 ns |   4,350.98 ns |  1,931.86 ns |  0.76 |    0.04 |    1 |         - |          NA |
| IntroSortDotnet              | 8192 | SingleElementMoved |    61,037.7 ns |   1,159.92 ns |    606.66 ns |  1.12 |    0.02 |    2 |         - |          NA |
| PDQSort                      | 8192 | SingleElementMoved |    42,276.5 ns |   1,085.29 ns |    567.63 ns |  0.77 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless            | 8192 | SingleElementMoved |    53,513.5 ns |     812.37 ns |    360.70 ns |  0.98 |    0.02 |    2 |         - |          NA |
| StdSort                      | 8192 | SingleElementMoved |    62,636.1 ns |   1,122.83 ns |    587.26 ns |  1.15 |    0.02 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | SingleElementMoved |    79,747.8 ns |     759.25 ns |    397.10 ns |  1.46 |    0.03 |    3 |         - |          NA |
| DotnetSort                   | 8192 | SingleElementMoved |    61,887.4 ns |   6,716.73 ns |  3,512.98 ns |  1.13 |    0.06 |    2 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Sorted**             |    **41,952.0 ns** |   **1,342.12 ns** |    **701.95 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Sorted             | 1,173,951.5 ns |   3,989.48 ns |  1,771.35 ns | 27.99 |    0.44 |    9 |         - |          NA |
| QuickSortMedian3             | 8192 | Sorted             |   889,833.3 ns |   6,630.57 ns |  3,467.92 ns | 21.22 |    0.34 |    8 |         - |          NA |
| QuickSortMedian9             | 8192 | Sorted             |   209,266.9 ns |   2,742.50 ns |  1,217.69 ns |  4.99 |    0.08 |    6 |         - |          NA |
| DualPivotQuickSort           | 8192 | Sorted             |   151,579.5 ns |   2,169.77 ns |    963.39 ns |  3.61 |    0.06 |    5 |         - |          NA |
| StableQuickSort              | 8192 | Sorted             |   431,282.3 ns |   1,863.53 ns |    827.42 ns | 10.28 |    0.16 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Sorted             |   226,327.5 ns |   3,634.19 ns |  1,900.75 ns |  5.40 |    0.09 |    6 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Sorted             |   395,627.3 ns |   3,629.53 ns |  1,898.32 ns |  9.43 |    0.15 |    7 |         - |          NA |
| IntroSort                    | 8192 | Sorted             |     9,159.8 ns |     851.60 ns |    445.40 ns |  0.22 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet              | 8192 | Sorted             |    47,871.2 ns |   1,070.12 ns |    475.14 ns |  1.14 |    0.02 |    3 |         - |          NA |
| PDQSort                      | 8192 | Sorted             |    10,357.0 ns |     374.96 ns |    166.49 ns |  0.25 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 8192 | Sorted             |    10,918.6 ns |     115.48 ns |     41.18 ns |  0.26 |    0.00 |    1 |         - |          NA |
| StdSort                      | 8192 | Sorted             |    14,537.7 ns |     790.37 ns |    350.93 ns |  0.35 |    0.01 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | Sorted             |    69,273.4 ns |   2,229.34 ns |    989.84 ns |  1.65 |    0.03 |    4 |         - |          NA |
| DotnetSort                   | 8192 | Sorted             |    46,769.3 ns |   6,487.23 ns |  3,392.95 ns |  1.12 |    0.08 |    3 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Reversed**           |    **45,779.4 ns** |   **1,111.48 ns** |    **581.33 ns** |  **1.00** |    **0.02** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Reversed           |   836,888.1 ns |   6,458.64 ns |  3,377.99 ns | 18.28 |    0.23 |   10 |         - |          NA |
| QuickSortMedian3             | 8192 | Reversed           | 1,123,797.8 ns |   3,631.73 ns |  1,612.51 ns | 24.55 |    0.29 |   11 |         - |          NA |
| QuickSortMedian9             | 8192 | Reversed           |   206,183.4 ns |   4,668.70 ns |  2,441.82 ns |  4.50 |    0.07 |    8 |         - |          NA |
| DualPivotQuickSort           | 8192 | Reversed           |   143,637.4 ns |   1,540.97 ns |    805.96 ns |  3.14 |    0.04 |    7 |         - |          NA |
| StableQuickSort              | 8192 | Reversed           |   433,246.9 ns |   1,521.35 ns |    675.49 ns |  9.47 |    0.11 |    9 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Reversed           |   225,774.6 ns |   3,530.13 ns |  1,846.33 ns |  4.93 |    0.07 |    8 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Reversed           |   390,548.9 ns |   2,611.77 ns |  1,366.00 ns |  8.53 |    0.11 |    9 |         - |          NA |
| IntroSort                    | 8192 | Reversed           |    24,789.7 ns |   2,045.28 ns |  1,069.72 ns |  0.54 |    0.02 |    3 |         - |          NA |
| IntroSortDotnet              | 8192 | Reversed           |    76,379.2 ns |   1,413.56 ns |    627.63 ns |  1.67 |    0.02 |    5 |         - |          NA |
| PDQSort                      | 8192 | Reversed           |    15,024.8 ns |     316.05 ns |    112.71 ns |  0.33 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 8192 | Reversed           |    23,220.8 ns |     958.82 ns |    425.72 ns |  0.51 |    0.01 |    3 |         - |          NA |
| StdSort                      | 8192 | Reversed           |    19,534.8 ns |   3,230.63 ns |  1,434.42 ns |  0.43 |    0.03 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | Reversed           |    76,206.3 ns |   1,586.66 ns |    704.49 ns |  1.66 |    0.02 |    5 |         - |          NA |
| DotnetSort                   | 8192 | Reversed           |    99,261.8 ns |   1,596.89 ns |    835.21 ns |  2.17 |    0.03 |    6 |         - |          NA |
|      |                    |                |               |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **PipeOrgan**          | **5,434,148.7 ns** | **126,142.20 ns** | **65,974.81 ns** |  **1.00** |    **0.02** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 8192 | PipeOrgan          |   451,729.3 ns |   2,716.82 ns |  1,420.95 ns |  0.08 |    0.00 |    4 |         - |          NA |
| QuickSortMedian3             | 8192 | PipeOrgan          |   498,357.7 ns |   9,678.02 ns |  4,297.10 ns |  0.09 |    0.00 |    4 |         - |          NA |
| QuickSortMedian9             | 8192 | PipeOrgan          |   281,307.5 ns |   2,134.72 ns |  1,116.50 ns |  0.05 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort           | 8192 | PipeOrgan          |   123,931.3 ns |   2,605.65 ns |  1,362.81 ns |  0.02 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 8192 | PipeOrgan          |   469,991.1 ns |   3,630.24 ns |  1,611.85 ns |  0.09 |    0.00 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | PipeOrgan          |   223,444.8 ns |   4,977.06 ns |  2,603.10 ns |  0.04 |    0.00 |    2 |         - |          NA |
| DestswapStableQuickSort      | 8192 | PipeOrgan          |   467,058.9 ns |  11,927.53 ns |  6,238.33 ns |  0.09 |    0.00 |    4 |         - |          NA |
| IntroSort                    | 8192 | PipeOrgan          |   281,032.3 ns |   4,847.77 ns |  2,535.48 ns |  0.05 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet              | 8192 | PipeOrgan          |   368,986.5 ns |   7,979.23 ns |  4,173.29 ns |  0.07 |    0.00 |    3 |         - |          NA |
| PDQSort                      | 8192 | PipeOrgan          |   118,930.5 ns |   5,192.06 ns |  2,715.55 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 8192 | PipeOrgan          |   201,993.9 ns |   2,554.59 ns |  1,336.10 ns |  0.04 |    0.00 |    2 |         - |          NA |
| StdSort                      | 8192 | PipeOrgan          |   294,973.2 ns |   6,569.85 ns |  3,436.16 ns |  0.05 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | PipeOrgan          |   256,295.0 ns |   2,119.87 ns |  1,108.73 ns |  0.05 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 8192 | PipeOrgan          |   373,292.5 ns |   9,309.95 ns |  4,869.28 ns |  0.07 |    0.00 |    3 |         - |          NA |

### SelectionBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.78GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method              | Size | Pattern            | Mean           | Error       | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------------- |----- |------------------- |---------------:|------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **SelectionSort**       | **256**  | **Random**             |    **26,254.4 ns** |    **385.4 ns** |    **201.58 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Random             |    19,429.9 ns |    142.9 ns |     63.44 ns |  0.74 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | Random             |    89,747.4 ns |  1,983.7 ns |  1,037.51 ns |  3.42 |    0.04 |    4 |         - |          NA |
| PancakeSort         | 256  | Random             |    43,601.9 ns |    821.7 ns |    429.78 ns |  1.66 |    0.02 |    3 |         - |          NA |
|      |                    |                |             |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **SingleElementMoved** |    **22,333.1 ns** |    **350.1 ns** |    **183.13 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | SingleElementMoved |    16,400.8 ns |    931.6 ns |    487.24 ns |  0.73 |    0.02 |    1 |         - |          NA |
| CycleSort           | 256  | SingleElementMoved |    51,677.2 ns |  1,811.0 ns |    947.18 ns |  2.31 |    0.04 |    3 |         - |          NA |
| PancakeSort         | 256  | SingleElementMoved |    19,856.8 ns |    234.1 ns |    122.42 ns |  0.89 |    0.01 |    2 |         - |          NA |
|      |                    |                |             |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **Sorted**             |    **21,918.1 ns** |    **159.5 ns** |     **70.83 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Sorted             |    11,214.0 ns |    484.3 ns |    215.03 ns |  0.51 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | Sorted             |    32,317.0 ns |    376.1 ns |    167.00 ns |  1.47 |    0.01 |    4 |         - |          NA |
| PancakeSort         | 256  | Sorted             |    15,421.6 ns |    100.6 ns |     52.59 ns |  0.70 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **Reversed**           |    **21,433.1 ns** |  **1,792.3 ns** |    **937.38 ns** |  **1.00** |    **0.06** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Reversed           |    16,443.1 ns |    231.6 ns |    121.15 ns |  0.77 |    0.03 |    1 |         - |          NA |
| CycleSort           | 256  | Reversed           |    45,198.0 ns |  2,341.7 ns |  1,224.74 ns |  2.11 |    0.11 |    3 |         - |          NA |
| PancakeSort         | 256  | Reversed           |    22,018.7 ns |    426.4 ns |    223.00 ns |  1.03 |    0.05 |    2 |         - |          NA |
|      |                    |                |             |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **PipeOrgan**          |    **24,913.2 ns** |    **355.6 ns** |    **186.01 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | PipeOrgan          |    21,609.0 ns |    291.1 ns |    152.27 ns |  0.87 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | PipeOrgan          |    60,045.8 ns |  2,818.5 ns |  1,474.11 ns |  2.41 |    0.06 |    3 |         - |          NA |
| PancakeSort         | 256  | PipeOrgan          |    33,747.0 ns |  1,026.3 ns |    455.69 ns |  1.35 |    0.02 |    2 |         - |          NA |
|      |                    |                |             |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Random**             |   **369,106.2 ns** |  **4,714.5 ns** |  **1,681.22 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Random             |   275,248.4 ns |    982.5 ns |    350.36 ns |  0.75 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | Random             | 1,678,906.0 ns |  5,534.7 ns |  2,894.77 ns |  4.55 |    0.02 |    4 |         - |          NA |
| PancakeSort         | 1024 | Random             |   634,901.9 ns | 10,711.7 ns |  4,756.08 ns |  1.72 |    0.01 |    3 |         - |          NA |
|      |                    |                |             |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **SingleElementMoved** |   **332,916.7 ns** |    **872.0 ns** |    **456.07 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | SingleElementMoved |   259,339.4 ns | 24,037.3 ns | 12,571.99 ns |  0.78 |    0.04 |    1 |         - |          NA |
| CycleSort           | 1024 | SingleElementMoved |   777,490.9 ns | 14,435.5 ns |  6,409.45 ns |  2.34 |    0.02 |    2 |         - |          NA |
| PancakeSort         | 1024 | SingleElementMoved |   294,034.7 ns |  2,904.0 ns |  1,289.37 ns |  0.88 |    0.00 |    1 |         - |          NA |
|      |                    |                |             |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Sorted**             |   **333,133.0 ns** |  **2,131.1 ns** |    **759.96 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Sorted             |   167,083.1 ns |    652.1 ns |    341.05 ns |  0.50 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | Sorted             |   498,379.7 ns |  4,891.8 ns |  2,171.99 ns |  1.50 |    0.01 |    4 |         - |          NA |
| PancakeSort         | 1024 | Sorted             |   224,026.9 ns |  1,443.0 ns |    754.72 ns |  0.67 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Reversed**           |   **315,065.7 ns** | **13,791.2 ns** |  **7,213.05 ns** |  **1.00** |    **0.03** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Reversed           |   248,736.2 ns |    422.1 ns |    220.75 ns |  0.79 |    0.02 |    1 |         - |          NA |
| CycleSort           | 1024 | Reversed           |   672,810.2 ns | 10,855.2 ns |  5,677.50 ns |  2.14 |    0.05 |    3 |         - |          NA |
| PancakeSort         | 1024 | Reversed           |   318,466.5 ns |  6,115.1 ns |  3,198.34 ns |  1.01 |    0.02 |    2 |         - |          NA |
|      |                    |                |             |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **PipeOrgan**          |   **350,161.1 ns** |  **3,328.7 ns** |  **1,477.96 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | PipeOrgan          |   311,728.2 ns |  3,406.0 ns |  1,781.39 ns |  0.89 |    0.01 |    1 |         - |          NA |
| CycleSort           | 1024 | PipeOrgan          |   951,507.9 ns | 21,508.0 ns | 11,249.08 ns |  2.72 |    0.03 |    3 |         - |          NA |
| PancakeSort         | 1024 | PipeOrgan          |   500,925.1 ns |  1,514.0 ns |    791.84 ns |  1.43 |    0.01 |    2 |         - |          NA |

### StringBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.78GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method             | Size | Pattern            | Mean               | Error          | StdDev         | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------- |----- |------------------- |-------------------:|---------------:|---------------:|------:|--------:|-----:|----------:|------------:|
| **QuickSort**          | **256**  | **Random**             |       **188,135.6 ns** |     **3,387.5 ns** |     **1,504.1 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| QuickSort3way      | 256  | Random             |       158,591.0 ns |     1,796.1 ns |       797.5 ns |  0.84 |    0.01 |    1 |         - |          NA |
| QuickSortMedian3   | 256  | Random             |       167,493.9 ns |     8,569.3 ns |     3,804.8 ns |  0.89 |    0.02 |    1 |         - |          NA |
| QuickSortMedian9   | 256  | Random             |       168,614.0 ns |     1,722.2 ns |       764.7 ns |  0.90 |    0.01 |    1 |         - |          NA |
| DualPivotQuickSort | 256  | Random             |       202,411.8 ns |    11,894.7 ns |     5,281.3 ns |  1.08 |    0.03 |    1 |         - |          NA |
| StableQuickSort    | 256  | Random             |       299,194.2 ns |     2,719.9 ns |     1,422.5 ns |  1.59 |    0.01 |    2 |         - |          NA |
| IntroSort          | 256  | Random             |       173,150.2 ns |     1,879.8 ns |       983.2 ns |  0.92 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | Random             |       158,808.4 ns |     3,282.9 ns |     1,457.6 ns |  0.84 |    0.01 |    1 |         - |          NA |
| PDQSort            | 256  | Random             |       215,194.2 ns |     1,138.2 ns |       405.9 ns |  1.14 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | Random             |       212,135.1 ns |     1,539.3 ns |       805.1 ns |  1.13 |    0.01 |    1 |         - |          NA |
| StdSort            | 256  | Random             |       200,329.2 ns |       822.0 ns |       365.0 ns |  1.06 |    0.01 |    1 |         - |          NA |
| BlockQuickSort     | 256  | Random             |       173,255.7 ns |     1,465.5 ns |       650.7 ns |  0.92 |    0.01 |    1 |         - |          NA |
| DotnetSort         | 256  | Random             |       165,973.0 ns |     7,247.3 ns |     3,217.9 ns |  0.88 |    0.02 |    1 |         - |          NA |
|      |                    |                    |                |                |       |         |      |           |             |
| **QuickSort**          | **256**  | **SingleElementMoved** |       **127,418.9 ns** |       **700.3 ns** |       **310.9 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| QuickSort3way      | 256  | SingleElementMoved |       200,591.9 ns |     2,742.6 ns |     1,434.4 ns |  1.57 |    0.01 |    2 |         - |          NA |
| QuickSortMedian3   | 256  | SingleElementMoved |       176,323.0 ns |       763.6 ns |       399.4 ns |  1.38 |    0.00 |    2 |         - |          NA |
| QuickSortMedian9   | 256  | SingleElementMoved |       168,301.6 ns |     4,894.1 ns |     2,559.7 ns |  1.32 |    0.02 |    2 |         - |          NA |
| DualPivotQuickSort | 256  | SingleElementMoved |       311,267.4 ns |     1,458.7 ns |       762.9 ns |  2.44 |    0.01 |    3 |         - |          NA |
| StableQuickSort    | 256  | SingleElementMoved |       236,828.8 ns |     4,117.5 ns |     1,828.2 ns |  1.86 |    0.01 |    2 |         - |          NA |
| IntroSort          | 256  | SingleElementMoved |        88,817.9 ns |     1,067.2 ns |       473.8 ns |  0.70 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | SingleElementMoved |       112,765.7 ns |     2,243.6 ns |     1,173.5 ns |  0.89 |    0.01 |    1 |         - |          NA |
| PDQSort            | 256  | SingleElementMoved |       119,643.8 ns |     1,029.9 ns |       457.3 ns |  0.94 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | SingleElementMoved |       124,746.9 ns |     1,057.4 ns |       553.1 ns |  0.98 |    0.00 |    1 |         - |          NA |
| StdSort            | 256  | SingleElementMoved |       125,671.8 ns |       667.0 ns |       296.2 ns |  0.99 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 256  | SingleElementMoved |       104,840.6 ns |       659.0 ns |       292.6 ns |  0.82 |    0.00 |    1 |         - |          NA |
| DotnetSort         | 256  | SingleElementMoved |       115,321.1 ns |     2,789.6 ns |     1,459.0 ns |  0.91 |    0.01 |    1 |         - |          NA |
|      |                    |                    |                |                |       |         |      |           |             |
| **QuickSort**          | **256**  | **Sorted**             |       **112,296.5 ns** |       **655.0 ns** |       **290.8 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 256  | Sorted             |       259,052.4 ns |     2,271.3 ns |     1,187.9 ns |  2.31 |    0.01 |    4 |         - |          NA |
| QuickSortMedian3   | 256  | Sorted             |       222,923.6 ns |     1,315.0 ns |       687.8 ns |  1.99 |    0.01 |    4 |         - |          NA |
| QuickSortMedian9   | 256  | Sorted             |       178,173.4 ns |     2,090.6 ns |     1,093.4 ns |  1.59 |    0.01 |    3 |         - |          NA |
| DualPivotQuickSort | 256  | Sorted             |       399,498.6 ns |     2,184.5 ns |     1,142.5 ns |  3.56 |    0.01 |    5 |         - |          NA |
| StableQuickSort    | 256  | Sorted             |       241,328.7 ns |     1,563.1 ns |       694.0 ns |  2.15 |    0.01 |    4 |         - |          NA |
| IntroSort          | 256  | Sorted             |        35,082.7 ns |       459.5 ns |       204.0 ns |  0.31 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | Sorted             |        88,309.2 ns |     1,506.4 ns |       787.9 ns |  0.79 |    0.01 |    2 |         - |          NA |
| PDQSort            | 256  | Sorted             |        36,542.6 ns |       570.6 ns |       253.4 ns |  0.33 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | Sorted             |        36,910.2 ns |       892.0 ns |       396.1 ns |  0.33 |    0.00 |    1 |         - |          NA |
| StdSort            | 256  | Sorted             |        40,286.4 ns |       462.8 ns |       242.0 ns |  0.36 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 256  | Sorted             |        99,620.0 ns |     1,028.1 ns |       456.5 ns |  0.89 |    0.00 |    2 |         - |          NA |
| DotnetSort         | 256  | Sorted             |        83,053.2 ns |       548.8 ns |       243.7 ns |  0.74 |    0.00 |    2 |         - |          NA |
|      |                    |                    |                |                |       |         |      |           |             |
| **QuickSort**          | **256**  | **Reversed**           |       **110,092.5 ns** |     **1,360.9 ns** |       **711.8 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 256  | Reversed           |       198,289.1 ns |     1,699.0 ns |       888.6 ns |  1.80 |    0.01 |    4 |         - |          NA |
| QuickSortMedian3   | 256  | Reversed           |       260,218.0 ns |     2,106.7 ns |     1,101.8 ns |  2.36 |    0.02 |    5 |         - |          NA |
| QuickSortMedian9   | 256  | Reversed           |       186,122.2 ns |       645.8 ns |       286.8 ns |  1.69 |    0.01 |    4 |         - |          NA |
| DualPivotQuickSort | 256  | Reversed           |       356,312.5 ns |       445.0 ns |       158.7 ns |  3.24 |    0.02 |    6 |         - |          NA |
| StableQuickSort    | 256  | Reversed           |       244,200.4 ns |     2,797.0 ns |     1,241.9 ns |  2.22 |    0.02 |    5 |         - |          NA |
| IntroSort          | 256  | Reversed           |        59,194.4 ns |       553.1 ns |       289.3 ns |  0.54 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | Reversed           |       137,872.3 ns |     3,861.6 ns |     2,019.7 ns |  1.25 |    0.02 |    3 |         - |          NA |
| PDQSort            | 256  | Reversed           |        53,769.6 ns |     1,938.0 ns |       860.5 ns |  0.49 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | Reversed           |        53,211.2 ns |       562.9 ns |       294.4 ns |  0.48 |    0.00 |    1 |         - |          NA |
| StdSort            | 256  | Reversed           |        55,232.3 ns |       339.9 ns |       177.8 ns |  0.50 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 256  | Reversed           |        92,404.5 ns |       663.5 ns |       294.6 ns |  0.84 |    0.01 |    2 |         - |          NA |
| DotnetSort         | 256  | Reversed           |       135,630.0 ns |     3,534.2 ns |     1,848.5 ns |  1.23 |    0.02 |    3 |         - |          NA |
|      |                    |                    |                |                |       |         |      |           |             |
| **QuickSort**          | **256**  | **PipeOrgan**          |     **1,103,018.9 ns** |     **3,378.0 ns** |     **1,499.8 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 256  | PipeOrgan          |       216,394.8 ns |       873.8 ns |       457.0 ns |  0.20 |    0.00 |    1 |         - |          NA |
| QuickSortMedian3   | 256  | PipeOrgan          |       249,690.8 ns |     3,119.9 ns |     1,631.8 ns |  0.23 |    0.00 |    1 |         - |          NA |
| QuickSortMedian9   | 256  | PipeOrgan          |       161,142.1 ns |     1,524.9 ns |       797.6 ns |  0.15 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort | 256  | PipeOrgan          |       170,537.2 ns |     1,451.4 ns |       759.1 ns |  0.15 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 256  | PipeOrgan          |       273,736.9 ns |     3,759.8 ns |     1,966.5 ns |  0.25 |    0.00 |    1 |         - |          NA |
| IntroSort          | 256  | PipeOrgan          |       149,931.3 ns |     2,258.4 ns |     1,002.8 ns |  0.14 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | PipeOrgan          |       259,134.4 ns |     3,821.5 ns |     1,998.7 ns |  0.23 |    0.00 |    1 |         - |          NA |
| PDQSort            | 256  | PipeOrgan          |       203,668.7 ns |     1,911.7 ns |       999.9 ns |  0.18 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | PipeOrgan          |       205,753.4 ns |       978.7 ns |       434.5 ns |  0.19 |    0.00 |    1 |         - |          NA |
| StdSort            | 256  | PipeOrgan          |       259,620.7 ns |     1,760.2 ns |       781.6 ns |  0.24 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 256  | PipeOrgan          |       252,652.8 ns |     1,486.8 ns |       777.6 ns |  0.23 |    0.00 |    1 |         - |          NA |
| DotnetSort         | 256  | PipeOrgan          |       269,872.7 ns |     3,123.7 ns |     1,386.9 ns |  0.24 |    0.00 |    1 |         - |          NA |
|      |                    |                    |                |                |       |         |      |           |             |
| **QuickSort**          | **1024** | **Random**             |     **1,035,420.5 ns** |     **3,796.1 ns** |     **1,985.5 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| QuickSort3way      | 1024 | Random             |       885,589.4 ns |     7,413.7 ns |     3,291.7 ns |  0.86 |    0.00 |    1 |         - |          NA |
| QuickSortMedian3   | 1024 | Random             |       893,734.5 ns |     3,188.2 ns |     1,667.5 ns |  0.86 |    0.00 |    1 |         - |          NA |
| QuickSortMedian9   | 1024 | Random             |       838,397.4 ns |     5,025.9 ns |     2,628.7 ns |  0.81 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort | 1024 | Random             |       859,398.5 ns |     1,680.6 ns |       746.2 ns |  0.83 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 1024 | Random             |     1,698,189.5 ns |    13,916.5 ns |     7,278.6 ns |  1.64 |    0.01 |    2 |         - |          NA |
| IntroSort          | 1024 | Random             |       898,428.7 ns |     1,966.4 ns |     1,028.4 ns |  0.87 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 1024 | Random             |       909,879.1 ns |     5,191.3 ns |     2,715.2 ns |  0.88 |    0.00 |    1 |         - |          NA |
| PDQSort            | 1024 | Random             |     1,034,916.5 ns |     2,374.9 ns |     1,054.5 ns |  1.00 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 1024 | Random             |     1,016,333.5 ns |    10,566.4 ns |     5,526.4 ns |  0.98 |    0.01 |    1 |         - |          NA |
| StdSort            | 1024 | Random             |       885,648.2 ns |     3,125.7 ns |     1,634.8 ns |  0.86 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 1024 | Random             |       944,296.8 ns |     3,753.8 ns |     1,666.7 ns |  0.91 |    0.00 |    1 |         - |          NA |
| DotnetSort         | 1024 | Random             |       933,889.1 ns |     3,365.7 ns |     1,760.3 ns |  0.90 |    0.00 |    1 |         - |          NA |
|      |                    |                    |                |                |       |         |      |           |             |
| **QuickSort**          | **1024** | **SingleElementMoved** |       **642,765.5 ns** |     **1,179.1 ns** |       **616.7 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 1024 | SingleElementMoved |     1,377,946.5 ns |    12,169.1 ns |     6,364.7 ns |  2.14 |    0.01 |    4 |         - |          NA |
| QuickSortMedian3   | 1024 | SingleElementMoved |     1,159,073.1 ns |     5,624.9 ns |     2,005.9 ns |  1.80 |    0.00 |    4 |         - |          NA |
| QuickSortMedian9   | 1024 | SingleElementMoved |       879,795.6 ns |     7,322.2 ns |     3,829.6 ns |  1.37 |    0.01 |    3 |         - |          NA |
| DualPivotQuickSort | 1024 | SingleElementMoved |     1,855,968.5 ns |     4,571.6 ns |     2,029.8 ns |  2.89 |    0.00 |    5 |         - |          NA |
| StableQuickSort    | 1024 | SingleElementMoved |     1,229,851.5 ns |     2,894.9 ns |     1,514.1 ns |  1.91 |    0.00 |    4 |         - |          NA |
| IntroSort          | 1024 | SingleElementMoved |       457,419.4 ns |     1,271.1 ns |       564.4 ns |  0.71 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 1024 | SingleElementMoved |       629,092.5 ns |     4,235.6 ns |     1,880.6 ns |  0.98 |    0.00 |    2 |         - |          NA |
| PDQSort            | 1024 | SingleElementMoved |       554,736.9 ns |     2,820.3 ns |     1,252.2 ns |  0.86 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless  | 1024 | SingleElementMoved |       560,701.9 ns |       822.3 ns |       365.1 ns |  0.87 |    0.00 |    2 |         - |          NA |
| StdSort            | 1024 | SingleElementMoved |       585,476.2 ns |     2,790.2 ns |     1,459.3 ns |  0.91 |    0.00 |    2 |         - |          NA |
| BlockQuickSort     | 1024 | SingleElementMoved |       559,498.9 ns |     2,351.2 ns |     1,043.9 ns |  0.87 |    0.00 |    2 |         - |          NA |
| DotnetSort         | 1024 | SingleElementMoved |       651,210.0 ns |     7,585.2 ns |     3,967.2 ns |  1.01 |    0.01 |    2 |         - |          NA |
|      |                    |                    |                |                |       |         |      |           |             |
| **QuickSort**          | **1024** | **Sorted**             |       **592,311.9 ns** |     **4,801.7 ns** |     **2,511.4 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 1024 | Sorted             |     1,929,750.7 ns |    44,698.3 ns |    23,378.1 ns |  3.26 |    0.04 |    5 |         - |          NA |
| QuickSortMedian3   | 1024 | Sorted             |     1,643,940.1 ns |     6,179.1 ns |     3,231.8 ns |  2.78 |    0.01 |    5 |         - |          NA |
| QuickSortMedian9   | 1024 | Sorted             |       877,805.9 ns |     5,152.4 ns |     2,694.8 ns |  1.48 |    0.01 |    3 |         - |          NA |
| DualPivotQuickSort | 1024 | Sorted             |     2,207,085.9 ns |     5,319.6 ns |     2,782.2 ns |  3.73 |    0.02 |    5 |         - |          NA |
| StableQuickSort    | 1024 | Sorted             |     1,250,608.6 ns |     2,868.5 ns |     1,500.3 ns |  2.11 |    0.01 |    4 |         - |          NA |
| IntroSort          | 1024 | Sorted             |       138,779.1 ns |     1,190.3 ns |       622.6 ns |  0.23 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 1024 | Sorted             |       491,017.5 ns |     1,820.5 ns |       808.3 ns |  0.83 |    0.00 |    2 |         - |          NA |
| PDQSort            | 1024 | Sorted             |       142,034.0 ns |     1,013.9 ns |       530.3 ns |  0.24 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 1024 | Sorted             |       143,491.3 ns |     3,299.8 ns |     1,725.9 ns |  0.24 |    0.00 |    1 |         - |          NA |
| StdSort            | 1024 | Sorted             |       157,733.4 ns |       666.1 ns |       348.4 ns |  0.27 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 1024 | Sorted             |       522,827.4 ns |     2,597.7 ns |     1,153.4 ns |  0.88 |    0.00 |    2 |         - |          NA |
| DotnetSort         | 1024 | Sorted             |       488,499.1 ns |     1,559.3 ns |       815.6 ns |  0.82 |    0.00 |    2 |         - |          NA |
|      |                    |                    |                |                |       |         |      |           |             |
| **QuickSort**          | **1024** | **Reversed**           |       **568,148.7 ns** |     **1,344.5 ns** |       **703.2 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way      | 1024 | Reversed           |     1,330,062.3 ns |     5,569.6 ns |     2,913.0 ns |  2.34 |    0.01 |    5 |         - |          NA |
| QuickSortMedian3   | 1024 | Reversed           |     1,971,917.3 ns |     8,550.0 ns |     3,049.0 ns |  3.47 |    0.01 |    6 |         - |          NA |
| QuickSortMedian9   | 1024 | Reversed           |       881,642.1 ns |     8,248.7 ns |     4,314.2 ns |  1.55 |    0.01 |    4 |         - |          NA |
| DualPivotQuickSort | 1024 | Reversed           |     1,853,301.6 ns |     8,724.7 ns |     4,563.2 ns |  3.26 |    0.01 |    6 |         - |          NA |
| StableQuickSort    | 1024 | Reversed           |     1,271,247.2 ns |     7,614.5 ns |     3,982.5 ns |  2.24 |    0.01 |    5 |         - |          NA |
| IntroSort          | 1024 | Reversed           |       407,399.2 ns |     3,168.0 ns |     1,656.9 ns |  0.72 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet    | 1024 | Reversed           |       817,323.7 ns |    13,711.1 ns |     7,171.2 ns |  1.44 |    0.01 |    4 |         - |          NA |
| PDQSort            | 1024 | Reversed           |       208,001.7 ns |     1,290.7 ns |       573.1 ns |  0.37 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 1024 | Reversed           |       210,772.8 ns |     1,835.7 ns |       960.1 ns |  0.37 |    0.00 |    1 |         - |          NA |
| StdSort            | 1024 | Reversed           |       211,257.2 ns |     1,264.7 ns |       661.5 ns |  0.37 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 1024 | Reversed           |       506,275.8 ns |     2,962.0 ns |     1,315.2 ns |  0.89 |    0.00 |    3 |         - |          NA |
| DotnetSort         | 1024 | Reversed           |       793,439.3 ns |     7,958.3 ns |     4,162.3 ns |  1.40 |    0.01 |    4 |         - |          NA |
|      |                    |                    |                |                |       |         |      |           |             |
| **QuickSort**          | **1024** | **PipeOrgan**          |    **17,162,701.8 ns** |   **191,300.5 ns** |   **100,053.9 ns** |  **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| QuickSort3way      | 1024 | PipeOrgan          |     1,341,254.3 ns |     7,704.2 ns |     4,029.5 ns |  0.08 |    0.00 |    3 |         - |          NA |
| QuickSortMedian3   | 1024 | PipeOrgan          |     1,474,918.4 ns |     6,777.8 ns |     3,544.9 ns |  0.09 |    0.00 |    3 |         - |          NA |
| QuickSortMedian9   | 1024 | PipeOrgan          |       840,862.2 ns |     6,384.8 ns |     2,276.9 ns |  0.05 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort | 1024 | PipeOrgan          |       858,004.6 ns |     2,654.0 ns |     1,178.4 ns |  0.05 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 1024 | PipeOrgan          |     1,305,137.9 ns |     6,955.5 ns |     3,637.9 ns |  0.08 |    0.00 |    3 |         - |          NA |
| IntroSort          | 1024 | PipeOrgan          |     1,282,081.6 ns |     5,203.9 ns |     2,721.7 ns |  0.07 |    0.00 |    3 |         - |          NA |
| IntroSortDotnet    | 1024 | PipeOrgan          |     1,622,491.8 ns |    31,944.2 ns |    16,707.4 ns |  0.09 |    0.00 |    3 |         - |          NA |
| PDQSort            | 1024 | PipeOrgan          |     1,067,934.2 ns |     6,049.9 ns |     2,686.2 ns |  0.06 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless  | 1024 | PipeOrgan          |     1,031,783.6 ns |    13,722.9 ns |     7,177.3 ns |  0.06 |    0.00 |    2 |         - |          NA |
| StdSort            | 1024 | PipeOrgan          |     1,515,969.7 ns |     6,625.7 ns |     3,465.4 ns |  0.09 |    0.00 |    3 |         - |          NA |
| BlockQuickSort     | 1024 | PipeOrgan          |     1,297,986.8 ns |     6,016.2 ns |     2,671.2 ns |  0.08 |    0.00 |    3 |         - |          NA |
| DotnetSort         | 1024 | PipeOrgan          |     1,620,376.5 ns |    17,809.1 ns |     7,907.4 ns |  0.09 |    0.00 |    3 |         - |          NA |
|      |                    |                    |                |                |       |         |      |           |             |
| **QuickSort**          | **8192** | **Random**             |    **10,593,024.1 ns** |    **49,133.6 ns** |    **21,815.6 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| QuickSort3way      | 8192 | Random             |     9,289,579.8 ns |    89,518.4 ns |    46,819.8 ns |  0.88 |    0.00 |    1 |         - |          NA |
| QuickSortMedian3   | 8192 | Random             |     9,472,388.7 ns |    44,247.9 ns |    23,142.5 ns |  0.89 |    0.00 |    1 |         - |          NA |
| QuickSortMedian9   | 8192 | Random             |     9,142,335.6 ns |   127,788.7 ns |    56,739.0 ns |  0.86 |    0.01 |    1 |         - |          NA |
| DualPivotQuickSort | 8192 | Random             |     9,151,853.5 ns |    77,932.1 ns |    40,760.0 ns |  0.86 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 8192 | Random             |    17,846,937.0 ns |    55,410.6 ns |    24,602.6 ns |  1.68 |    0.00 |    2 |         - |          NA |
| IntroSort          | 8192 | Random             |    10,103,129.3 ns |    57,027.2 ns |    25,320.4 ns |  0.95 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 8192 | Random             |     8,847,459.7 ns |    24,896.3 ns |    11,054.1 ns |  0.84 |    0.00 |    1 |         - |          NA |
| PDQSort            | 8192 | Random             |    10,661,843.1 ns |    26,139.7 ns |    13,671.6 ns |  1.01 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | Random             |    10,742,368.9 ns |   118,964.7 ns |    62,220.8 ns |  1.01 |    0.01 |    1 |         - |          NA |
| StdSort            | 8192 | Random             |     9,263,568.3 ns |   110,897.4 ns |    58,001.5 ns |  0.87 |    0.01 |    1 |         - |          NA |
| BlockQuickSort     | 8192 | Random             |     9,017,008.6 ns |    21,244.6 ns |    11,111.3 ns |  0.85 |    0.00 |    1 |         - |          NA |
| DotnetSort         | 8192 | Random             |     9,012,112.3 ns |    54,456.6 ns |    24,179.1 ns |  0.85 |    0.00 |    1 |         - |          NA |
|      |                    |                    |                |                |       |         |      |           |             |
| **QuickSort**          | **8192** | **SingleElementMoved** |     **6,747,832.1 ns** |    **33,263.9 ns** |    **17,397.7 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| QuickSort3way      | 8192 | SingleElementMoved |    28,445,528.6 ns |   180,480.0 ns |    94,394.5 ns |  4.22 |    0.02 |    4 |         - |          NA |
| QuickSortMedian3   | 8192 | SingleElementMoved |    22,217,429.3 ns |    26,515.4 ns |    13,868.1 ns |  3.29 |    0.01 |    3 |         - |          NA |
| QuickSortMedian9   | 8192 | SingleElementMoved |     8,872,245.8 ns |    50,961.2 ns |    26,653.7 ns |  1.31 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort | 8192 | SingleElementMoved |    12,331,763.9 ns |    51,476.3 ns |    26,923.1 ns |  1.83 |    0.01 |    2 |         - |          NA |
| StableQuickSort    | 8192 | SingleElementMoved |    13,232,216.0 ns |   347,682.6 ns |   181,844.7 ns |  1.96 |    0.03 |    2 |         - |          NA |
| IntroSort          | 8192 | SingleElementMoved |     4,590,845.5 ns |    13,270.1 ns |     5,892.0 ns |  0.68 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 8192 | SingleElementMoved |     7,421,352.2 ns |    23,403.8 ns |    12,240.6 ns |  1.10 |    0.00 |    1 |         - |          NA |
| PDQSort            | 8192 | SingleElementMoved |     5,365,452.7 ns |    20,807.6 ns |    10,882.8 ns |  0.80 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | SingleElementMoved |     5,402,480.3 ns |    35,637.8 ns |    18,639.3 ns |  0.80 |    0.00 |    1 |         - |          NA |
| StdSort            | 8192 | SingleElementMoved |     5,423,654.3 ns |    12,603.6 ns |     5,596.1 ns |  0.80 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 8192 | SingleElementMoved |     6,152,134.2 ns |     6,295.6 ns |     2,795.3 ns |  0.91 |    0.00 |    1 |         - |          NA |
| DotnetSort         | 8192 | SingleElementMoved |     7,503,037.5 ns |    50,200.0 ns |    26,255.6 ns |  1.11 |    0.00 |    1 |         - |          NA |
|      |                    |                    |                |                |       |         |      |           |             |
| **QuickSort**          | **8192** | **Sorted**             |     **6,452,115.4 ns** |    **37,346.1 ns** |    **16,581.9 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 8192 | Sorted             |    42,032,710.5 ns |   394,681.3 ns |   206,425.9 ns |  6.51 |    0.03 |    5 |         - |          NA |
| QuickSortMedian3   | 8192 | Sorted             |    35,324,845.8 ns |    81,720.7 ns |    36,284.5 ns |  5.47 |    0.01 |    5 |         - |          NA |
| QuickSortMedian9   | 8192 | Sorted             |     8,712,106.8 ns |    53,284.9 ns |    27,869.0 ns |  1.35 |    0.01 |    3 |         - |          NA |
| DualPivotQuickSort | 8192 | Sorted             |    15,612,897.2 ns |    48,523.3 ns |    21,544.7 ns |  2.42 |    0.01 |    4 |         - |          NA |
| StableQuickSort    | 8192 | Sorted             |    13,536,437.5 ns |   119,289.1 ns |    52,965.1 ns |  2.10 |    0.01 |    4 |         - |          NA |
| IntroSort          | 8192 | Sorted             |     1,117,311.5 ns |     5,522.6 ns |     2,888.4 ns |  0.17 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 8192 | Sorted             |     5,571,007.7 ns |    23,992.7 ns |    10,652.9 ns |  0.86 |    0.00 |    2 |         - |          NA |
| PDQSort            | 8192 | Sorted             |     1,172,823.9 ns |     6,720.5 ns |     2,983.9 ns |  0.18 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | Sorted             |     1,197,565.5 ns |     7,298.0 ns |     3,817.0 ns |  0.19 |    0.00 |    1 |         - |          NA |
| StdSort            | 8192 | Sorted             |     1,338,915.1 ns |     4,831.5 ns |     2,527.0 ns |  0.21 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 8192 | Sorted             |     5,895,527.5 ns |    29,650.5 ns |    13,165.0 ns |  0.91 |    0.00 |    2 |         - |          NA |
| DotnetSort         | 8192 | Sorted             |     5,476,438.0 ns |    22,929.6 ns |    10,180.9 ns |  0.85 |    0.00 |    2 |         - |          NA |
|      |                    |                    |                |                |       |         |      |           |             |
| **QuickSort**          | **8192** | **Reversed**           |     **6,418,430.1 ns** |    **10,678.5 ns** |     **5,585.1 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way      | 8192 | Reversed           |    28,091,621.6 ns |   126,201.1 ns |    66,005.6 ns |  4.38 |    0.01 |    6 |         - |          NA |
| QuickSortMedian3   | 8192 | Reversed           |    44,350,704.1 ns |    94,136.9 ns |    41,797.4 ns |  6.91 |    0.01 |    7 |         - |          NA |
| QuickSortMedian9   | 8192 | Reversed           |     8,424,600.8 ns |    33,052.6 ns |    14,675.6 ns |  1.31 |    0.00 |    4 |         - |          NA |
| DualPivotQuickSort | 8192 | Reversed           |    13,409,062.8 ns |    17,679.7 ns |     6,304.7 ns |  2.09 |    0.00 |    5 |         - |          NA |
| StableQuickSort    | 8192 | Reversed           |    13,592,261.7 ns |    98,451.4 ns |    43,713.0 ns |  2.12 |    0.01 |    5 |         - |          NA |
| IntroSort          | 8192 | Reversed           |     3,730,128.9 ns |     4,477.8 ns |     2,342.0 ns |  0.58 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet    | 8192 | Reversed           |     9,673,413.0 ns |    51,101.9 ns |    26,727.3 ns |  1.51 |    0.00 |    4 |         - |          NA |
| PDQSort            | 8192 | Reversed           |     1,720,248.0 ns |     6,660.8 ns |     3,483.7 ns |  0.27 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | Reversed           |     1,733,557.7 ns |    24,845.8 ns |    12,994.8 ns |  0.27 |    0.00 |    1 |         - |          NA |
| StdSort            | 8192 | Reversed           |     1,740,013.2 ns |    15,516.2 ns |     6,889.3 ns |  0.27 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 8192 | Reversed           |     5,744,061.8 ns |     8,647.1 ns |     3,839.4 ns |  0.89 |    0.00 |    3 |         - |          NA |
| DotnetSort         | 8192 | Reversed           |     9,474,237.8 ns |    45,983.6 ns |    24,050.3 ns |  1.48 |    0.00 |    4 |         - |          NA |
|      |                    |                    |                |                |       |         |      |           |             |
| **QuickSort**          | **8192** | **PipeOrgan**          | **1,080,666,365.3 ns** | **3,034,674.7 ns** | **1,587,193.6 ns** | **1.000** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way      | 8192 | PipeOrgan          |    17,761,806.6 ns |   125,084.8 ns |    65,421.7 ns | 0.016 |    0.00 |    3 |         - |          NA |
| QuickSortMedian3   | 8192 | PipeOrgan          |    18,833,811.7 ns |   127,272.7 ns |    56,509.9 ns | 0.017 |    0.00 |    3 |         - |          NA |
| QuickSortMedian9   | 8192 | PipeOrgan          |     8,832,066.0 ns |    44,030.7 ns |    19,549.9 ns | 0.008 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort | 8192 | PipeOrgan          |     8,603,205.2 ns |    13,461.6 ns |     5,977.0 ns | 0.008 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 8192 | PipeOrgan          |    13,973,807.5 ns |    18,459.9 ns |     9,654.9 ns | 0.013 |    0.00 |    2 |         - |          NA |
| IntroSort          | 8192 | PipeOrgan          |    20,288,707.4 ns |    23,940.1 ns |    10,629.6 ns | 0.019 |    0.00 |    3 |         - |          NA |
| IntroSortDotnet    | 8192 | PipeOrgan          |    21,725,539.8 ns |    68,021.3 ns |    35,576.5 ns | 0.020 |    0.00 |    3 |         - |          NA |
| PDQSort            | 8192 | PipeOrgan          |    11,433,411.2 ns |    43,364.3 ns |    19,254.0 ns | 0.011 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless  | 8192 | PipeOrgan          |    11,271,055.9 ns |    53,744.2 ns |    28,109.3 ns | 0.010 |    0.00 |    2 |         - |          NA |
| StdSort            | 8192 | PipeOrgan          |    19,760,716.5 ns |   189,752.1 ns |    99,244.0 ns | 0.018 |    0.00 |    3 |         - |          NA |
| BlockQuickSort     | 8192 | PipeOrgan          |    12,382,292.5 ns |    60,997.7 ns |    27,083.4 ns | 0.011 |    0.00 |    2 |         - |          NA |
| DotnetSort         | 8192 | PipeOrgan          |    21,576,747.3 ns |   147,043.4 ns |    65,288.2 ns | 0.020 |    0.00 |    3 |         - |          NA |

### TreeBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.78GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                 | Size | Pattern            | Mean           | Error       | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------- |----- |------------------- |---------------:|------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **BalancedBinaryTreeSort** | **256**  | **Random**             |    **12,673.1 ns** |   **507.75 ns** |   **265.56 ns** |  **3.55** |    **0.26** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 256  | Random             |     3,592.0 ns |   542.69 ns |   283.84 ns |  1.01 |    0.10 |    1 |         - |          NA |
| SplaySort              | 256  | Random             |    22,839.8 ns |   584.20 ns |   305.55 ns |  6.39 |    0.46 |    4 |         - |          NA |
| TreapSort              | 256  | Random             |     9,096.1 ns |   340.23 ns |   177.94 ns |  2.55 |    0.19 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **SingleElementMoved** |    **14,574.1 ns** | **1,676.43 ns** |   **876.80 ns** |  **0.29** |    **0.02** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 256  | SingleElementMoved |    50,551.7 ns |   248.94 ns |   130.20 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 256  | SingleElementMoved |     4,262.9 ns |   343.19 ns |   179.50 ns |  0.08 |    0.00 |    1 |         - |          NA |
| TreapSort              | 256  | SingleElementMoved |     5,880.2 ns |   312.40 ns |   163.39 ns |  0.12 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Sorted**             |    **14,091.8 ns** | **1,043.58 ns** |   **545.81 ns** |  **0.19** |    **0.01** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 256  | Sorted             |    75,872.4 ns |   224.10 ns |    99.50 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 256  | Sorted             |     3,696.0 ns |    12.25 ns |     5.44 ns |  0.05 |    0.00 |    1 |         - |          NA |
| TreapSort              | 256  | Sorted             |     5,057.0 ns |   359.65 ns |   188.11 ns |  0.07 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Reversed**           |    **12,386.7 ns** |   **503.17 ns** |   **263.17 ns** |  **0.17** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 256  | Reversed           |    73,894.8 ns |   591.29 ns |   309.25 ns |  1.00 |    0.01 |    4 |         - |          NA |
| SplaySort              | 256  | Reversed           |     3,547.3 ns |     8.57 ns |     3.06 ns |  0.05 |    0.00 |    1 |         - |          NA |
| TreapSort              | 256  | Reversed           |     5,453.2 ns |   492.86 ns |   257.78 ns |  0.07 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **PipeOrgan**          |    **12,337.3 ns** |   **370.77 ns** |   **164.62 ns** |  **0.32** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 256  | PipeOrgan          |    38,553.8 ns |   430.16 ns |   191.00 ns |  1.00 |    0.01 |    4 |         - |          NA |
| SplaySort              | 256  | PipeOrgan          |     4,462.7 ns |   315.13 ns |   164.82 ns |  0.12 |    0.00 |    1 |         - |          NA |
| TreapSort              | 256  | PipeOrgan          |     6,943.3 ns |   111.18 ns |    49.36 ns |  0.18 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Random**             |   **126,822.8 ns** | **5,520.24 ns** | **2,887.19 ns** |  **6.21** |    **0.15** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 1024 | Random             |    20,426.3 ns |   428.86 ns |   224.30 ns |  1.00 |    0.01 |    1 |         - |          NA |
| SplaySort              | 1024 | Random             |   160,887.4 ns | 5,432.48 ns | 2,412.05 ns |  7.88 |    0.14 |    4 |         - |          NA |
| TreapSort              | 1024 | Random             |    39,528.4 ns | 3,311.42 ns | 1,470.29 ns |  1.94 |    0.07 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **SingleElementMoved** |   **105,776.5 ns** | **1,505.25 ns** |   **787.28 ns** |  **0.14** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 1024 | SingleElementMoved |   781,172.2 ns | 1,179.48 ns |   523.70 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | SingleElementMoved |    16,492.3 ns |   277.09 ns |   144.92 ns |  0.02 |    0.00 |    1 |         - |          NA |
| TreapSort              | 1024 | SingleElementMoved |    29,603.9 ns | 1,304.62 ns |   682.34 ns |  0.04 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Sorted**             |    **98,190.0 ns** | **1,313.92 ns** |   **583.39 ns** |  **0.08** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 1024 | Sorted             | 1,191,249.2 ns | 1,263.99 ns |   561.22 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | Sorted             |    14,787.6 ns |   164.13 ns |    85.84 ns |  0.01 |    0.00 |    1 |         - |          NA |
| TreapSort              | 1024 | Sorted             |    23,847.7 ns |   617.47 ns |   274.16 ns |  0.02 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Reversed**           |    **59,811.0 ns** |   **503.11 ns** |   **263.13 ns** |  **0.05** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 1024 | Reversed           | 1,153,063.4 ns | 2,342.10 ns | 1,224.96 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | Reversed           |    14,179.6 ns |   110.76 ns |    49.18 ns |  0.01 |    0.00 |    1 |         - |          NA |
| TreapSort              | 1024 | Reversed           |    23,286.7 ns |   970.91 ns |   507.80 ns |  0.02 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **PipeOrgan**          |    **91,758.2 ns** | **1,691.90 ns** |   **751.22 ns** |  **0.16** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 1024 | PipeOrgan          |   583,594.0 ns | 1,222.53 ns |   542.81 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | PipeOrgan          |    17,289.3 ns |   282.86 ns |   147.94 ns |  0.03 |    0.00 |    1 |         - |          NA |
| TreapSort              | 1024 | PipeOrgan          |    33,914.0 ns | 1,084.82 ns |   481.67 ns |  0.06 |    0.00 |    2 |         - |          NA |

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
