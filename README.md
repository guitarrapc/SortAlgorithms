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
<summary>Benchmark results (2026-08-01 13:20 UTC)</summary>

Workflow run: https://github.com/guitarrapc/SortAlgorithms/actions/runs/30700719933

### AdaptiveBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method        | Size | Pattern            | Mean         | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------- |----- |------------------- |-------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **DropMergeSort** | **256**  | **Random**             |   **2,506.9 ns** |    **158.80 ns** |     **83.06 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | Random             |   3,987.0 ns |    348.54 ns |    154.76 ns |  1.59 |    0.08 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **256**  | **SingleElementMoved** |     **521.2 ns** |    **172.27 ns** |     **90.10 ns** |  **1.03** |    **0.25** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | SingleElementMoved |   5,975.2 ns |    286.74 ns |    149.97 ns | 11.80 |    2.11 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **256**  | **Sorted**             |     **614.0 ns** |    **175.48 ns** |     **77.91 ns** |  **1.01** |    **0.17** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | Sorted             |   6,269.2 ns |    440.68 ns |    195.66 ns | 10.36 |    1.31 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **256**  | **Reversed**           |   **1,166.1 ns** |     **13.19 ns** |      **5.86 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | Reversed           |   1,279.4 ns |    251.50 ns |    131.54 ns |  1.10 |    0.11 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **256**  | **PipeOrgan**          |   **4,569.0 ns** |    **388.63 ns** |    **172.55 ns** |  **1.00** |    **0.05** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | PipeOrgan          |   4,367.5 ns |    322.90 ns |    168.88 ns |  0.96 |    0.05 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **Random**             |  **10,937.6 ns** |    **419.53 ns** |    **186.28 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | Random             |  18,440.9 ns |    427.70 ns |    189.90 ns |  1.69 |    0.03 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **SingleElementMoved** |   **1,702.8 ns** |    **210.64 ns** |    **110.17 ns** |  **1.00** |    **0.09** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | SingleElementMoved |  30,449.0 ns |    417.07 ns |    185.18 ns | 17.95 |    1.12 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **Sorted**             |   **1,986.6 ns** |      **8.12 ns** |      **2.89 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | Sorted             |  31,928.8 ns |    537.63 ns |    238.71 ns | 16.07 |    0.11 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **Reversed**           |   **5,382.5 ns** |    **236.23 ns** |    **104.89 ns** |  **1.00** |    **0.03** |    **2** |         **-** |          **NA** |
| PatienceSort  | 1024 | Reversed           |   3,977.1 ns |    101.16 ns |     52.91 ns |  0.74 |    0.02 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **PipeOrgan**          |  **19,296.2 ns** |    **421.30 ns** |    **220.35 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | PipeOrgan          |  21,220.2 ns |    394.43 ns |    175.13 ns |  1.10 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **4096** | **Random**             |  **57,324.1 ns** |  **3,018.05 ns** |  **1,340.03 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| PatienceSort  | 4096 | Random             | 131,380.8 ns | 14,829.58 ns |  7,756.16 ns |  2.29 |    0.14 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **4096** | **SingleElementMoved** |   **5,975.4 ns** |    **345.04 ns** |    **153.20 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| PatienceSort  | 4096 | SingleElementMoved | 184,241.0 ns | 34,090.81 ns | 17,830.15 ns | 30.85 |    2.91 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **4096** | **Sorted**             |   **7,863.7 ns** |    **180.50 ns** |     **80.14 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| PatienceSort  | 4096 | Sorted             | 194,420.0 ns | 32,195.03 ns | 16,838.62 ns | 24.73 |    2.03 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **4096** | **Reversed**           |  **23,575.7 ns** |    **555.81 ns** |    **290.70 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| PatienceSort  | 4096 | Reversed           |  15,828.3 ns |     62.42 ns |     27.72 ns |  0.67 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **4096** | **PipeOrgan**          |  **80,374.3 ns** |  **1,154.71 ns** |    **603.93 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| PatienceSort  | 4096 | PipeOrgan          | 124,821.0 ns | 10,543.77 ns |  5,514.60 ns |  1.55 |    0.07 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **Random**             | **373,080.4 ns** |  **3,849.65 ns** |  **2,013.44 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | Random             | 632,801.4 ns |  2,376.36 ns |  1,055.12 ns |  1.70 |    0.01 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **SingleElementMoved** |  **11,930.8 ns** |    **673.01 ns** |    **298.82 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | SingleElementMoved | 606,129.4 ns |  4,170.91 ns |  1,851.91 ns | 50.83 |    1.17 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **Sorted**             |  **16,176.2 ns** |  **1,098.31 ns** |    **487.66 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | Sorted             | 598,488.3 ns |  4,608.39 ns |  2,410.28 ns | 37.03 |    1.04 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **Reversed**           |  **49,649.1 ns** |    **760.41 ns** |    **337.62 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| PatienceSort  | 8192 | Reversed           |  31,510.6 ns |    956.77 ns |    500.41 ns |  0.63 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **PipeOrgan**          | **161,869.7 ns** |    **666.68 ns** |    **296.01 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | PipeOrgan          | 497,742.9 ns |  3,685.09 ns |  1,636.20 ns |  3.07 |    0.01 |    2 |         - |          NA |

### AdaptiveSlowBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method     | Size | Pattern            | Mean         | Error       | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------- |----- |------------------- |-------------:|------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **StrandSort** | **256**  | **Random**             |   **4,744.3 ns** |   **203.51 ns** |    **90.36 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **256**  | **SingleElementMoved** |     **664.5 ns** |    **51.52 ns** |    **22.87 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **256**  | **Sorted**             |     **450.6 ns** |    **91.73 ns** |    **47.98 ns** |  **1.01** |    **0.14** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **256**  | **Reversed**           |  **37,119.3 ns** |   **531.81 ns** |   **236.13 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **256**  | **PipeOrgan**          |  **21,210.5 ns** |   **192.48 ns** |   **100.67 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **1024** | **Random**             |  **42,576.1 ns** |   **399.34 ns** |   **208.86 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **1024** | **SingleElementMoved** |   **2,059.7 ns** |     **8.74 ns** |     **4.57 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **1024** | **Sorted**             |   **1,279.6 ns** |    **44.03 ns** |    **19.55 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **1024** | **Reversed**           | **603,071.7 ns** | **5,151.62 ns** | **2,694.40 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **1024** | **PipeOrgan**          | **338,788.9 ns** | **1,928.95 ns** |   **856.47 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |

### DistributionBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method              | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **CountingSort**        | **256**  | **Random**             |     **1,167.4 ns** |     **11.53 ns** |      **6.03 ns** |  **1.52** |    **0.01** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | Random             |       770.2 ns |      4.87 ns |      2.16 ns |  1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 256  | Random             |     1,220.7 ns |     10.97 ns |      4.87 ns |  1.58 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | Random             |       835.7 ns |     37.84 ns |     16.80 ns |  1.09 |    0.02 |    1 |         - |          NA |
| BucketSort          | 256  | Random             |     6,228.7 ns |    300.59 ns |    157.21 ns |  8.09 |    0.19 |    6 |         - |          NA |
| BucketSortInteger   | 256  | Random             |     2,010.7 ns |    290.15 ns |    151.75 ns |  2.61 |    0.19 |    4 |         - |          NA |
| FlashSort           | 256  | Random             |     3,525.5 ns |     54.58 ns |     24.24 ns |  4.58 |    0.03 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | Random             |     4,140.1 ns |     58.10 ns |     25.80 ns |  5.38 |    0.03 |    5 |         - |          NA |
| RadixLSD256Sort     | 256  | Random             |     1,586.3 ns |     62.13 ns |     27.59 ns |  2.06 |    0.03 |    3 |         - |          NA |
| RadixLSD10Sort      | 256  | Random             |     3,400.9 ns |    140.78 ns |     73.63 ns |  4.42 |    0.09 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | Random             |     7,173.7 ns |    302.33 ns |    158.13 ns |  9.31 |    0.20 |    6 |         - |          NA |
| RadixMSD10Sort      | 256  | Random             |    10,432.8 ns |    168.87 ns |     74.98 ns | 13.55 |    0.10 |    7 |         - |          NA |
| AmericanFlagSort    | 256  | Random             |     3,450.8 ns |     82.01 ns |     29.25 ns |  4.48 |    0.04 |    5 |         - |          NA |
| SpreadSort          | 256  | Random             |     1,513.6 ns |    130.74 ns |     68.38 ns |  1.97 |    0.08 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **256**  | **SingleElementMoved** |     **1,106.2 ns** |     **13.88 ns** |      **6.16 ns** |  **1.38** |    **0.10** |    **1** |         **-** |          **NA** |
| CountingSortInteger | 256  | SingleElementMoved |       806.0 ns |    119.91 ns |     62.72 ns |  1.01 |    0.10 |    1 |         - |          NA |
| PigeonSort          | 256  | SingleElementMoved |     1,308.6 ns |     66.77 ns |     29.65 ns |  1.63 |    0.13 |    1 |         - |          NA |
| PigeonSortInteger   | 256  | SingleElementMoved |       947.5 ns |     27.18 ns |     14.22 ns |  1.18 |    0.09 |    1 |         - |          NA |
| BucketSort          | 256  | SingleElementMoved |     2,604.4 ns |    143.05 ns |     63.51 ns |  3.25 |    0.25 |    2 |         - |          NA |
| BucketSortInteger   | 256  | SingleElementMoved |     1,540.9 ns |     52.77 ns |     27.60 ns |  1.92 |    0.15 |    1 |         - |          NA |
| FlashSort           | 256  | SingleElementMoved |     4,259.9 ns |    252.56 ns |    132.10 ns |  5.31 |    0.42 |    4 |         - |          NA |
| RadixLSD4Sort       | 256  | SingleElementMoved |     3,494.0 ns |    285.93 ns |    126.95 ns |  4.36 |    0.36 |    3 |         - |          NA |
| RadixLSD256Sort     | 256  | SingleElementMoved |     1,549.5 ns |     19.20 ns |      8.52 ns |  1.93 |    0.14 |    1 |         - |          NA |
| RadixLSD10Sort      | 256  | SingleElementMoved |     3,355.6 ns |     23.53 ns |     10.45 ns |  4.19 |    0.31 |    3 |         - |          NA |
| RadixMSD4Sort       | 256  | SingleElementMoved |     6,895.5 ns |    299.28 ns |    156.53 ns |  8.60 |    0.66 |    5 |         - |          NA |
| RadixMSD10Sort      | 256  | SingleElementMoved |    10,389.2 ns |    148.52 ns |     77.68 ns | 12.96 |    0.96 |    6 |         - |          NA |
| AmericanFlagSort    | 256  | SingleElementMoved |     2,691.6 ns |     89.84 ns |     32.04 ns |  3.36 |    0.25 |    2 |         - |          NA |
| SpreadSort          | 256  | SingleElementMoved |       985.1 ns |     94.69 ns |     42.04 ns |  1.23 |    0.10 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **256**  | **Sorted**             |     **1,195.3 ns** |     **61.45 ns** |     **32.14 ns** |  **1.54** |    **0.04** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | Sorted             |       778.2 ns |     20.32 ns |     10.63 ns |  1.00 |    0.02 |    2 |         - |          NA |
| PigeonSort          | 256  | Sorted             |     1,220.0 ns |     10.46 ns |      4.64 ns |  1.57 |    0.02 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | Sorted             |       786.3 ns |      8.54 ns |      4.47 ns |  1.01 |    0.01 |    2 |         - |          NA |
| BucketSort          | 256  | Sorted             |     2,629.4 ns |     55.15 ns |     19.67 ns |  3.38 |    0.05 |    5 |         - |          NA |
| BucketSortInteger   | 256  | Sorted             |     1,515.3 ns |     68.13 ns |     30.25 ns |  1.95 |    0.04 |    4 |         - |          NA |
| FlashSort           | 256  | Sorted             |     4,226.0 ns |     22.46 ns |      8.01 ns |  5.43 |    0.07 |    7 |         - |          NA |
| RadixLSD4Sort       | 256  | Sorted             |     3,476.4 ns |     25.58 ns |      9.12 ns |  4.47 |    0.06 |    6 |         - |          NA |
| RadixLSD256Sort     | 256  | Sorted             |     1,485.8 ns |     28.66 ns |     14.99 ns |  1.91 |    0.03 |    4 |         - |          NA |
| RadixLSD10Sort      | 256  | Sorted             |     3,366.7 ns |    140.59 ns |     73.53 ns |  4.33 |    0.10 |    6 |         - |          NA |
| RadixMSD4Sort       | 256  | Sorted             |     6,678.6 ns |    297.96 ns |    155.84 ns |  8.58 |    0.22 |    8 |         - |          NA |
| RadixMSD10Sort      | 256  | Sorted             |    10,250.6 ns |     76.34 ns |     39.93 ns | 13.17 |    0.17 |    9 |         - |          NA |
| AmericanFlagSort    | 256  | Sorted             |     2,395.3 ns |    232.94 ns |    103.43 ns |  3.08 |    0.13 |    5 |         - |          NA |
| SpreadSort          | 256  | Sorted             |       411.1 ns |    137.78 ns |     61.18 ns |  0.53 |    0.07 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **256**  | **Reversed**           |     **1,098.4 ns** |      **9.96 ns** |      **5.21 ns** |  **1.57** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | Reversed           |       698.8 ns |      6.67 ns |      2.96 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PigeonSort          | 256  | Reversed           |     1,168.8 ns |     18.37 ns |      6.55 ns |  1.67 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | Reversed           |       852.6 ns |    252.43 ns |    112.08 ns |  1.22 |    0.15 |    2 |         - |          NA |
| BucketSort          | 256  | Reversed           |     8,768.0 ns |     10.95 ns |      3.90 ns | 12.55 |    0.05 |    7 |         - |          NA |
| BucketSortInteger   | 256  | Reversed           |     2,550.7 ns |    191.25 ns |     84.92 ns |  3.65 |    0.11 |    5 |         - |          NA |
| FlashSort           | 256  | Reversed           |     3,637.7 ns |     16.92 ns |      7.51 ns |  5.21 |    0.02 |    6 |         - |          NA |
| RadixLSD4Sort       | 256  | Reversed           |     3,492.6 ns |    248.14 ns |    129.78 ns |  5.00 |    0.18 |    6 |         - |          NA |
| RadixLSD256Sort     | 256  | Reversed           |     1,487.0 ns |     13.61 ns |      6.04 ns |  2.13 |    0.01 |    4 |         - |          NA |
| RadixLSD10Sort      | 256  | Reversed           |     3,408.3 ns |     18.37 ns |      6.55 ns |  4.88 |    0.02 |    6 |         - |          NA |
| RadixMSD4Sort       | 256  | Reversed           |     7,683.1 ns |    449.57 ns |    235.13 ns | 10.99 |    0.32 |    7 |         - |          NA |
| RadixMSD10Sort      | 256  | Reversed           |    10,891.0 ns |    220.44 ns |    115.30 ns | 15.58 |    0.17 |    8 |         - |          NA |
| AmericanFlagSort    | 256  | Reversed           |     3,255.0 ns |    118.41 ns |     52.57 ns |  4.66 |    0.07 |    6 |         - |          NA |
| SpreadSort          | 256  | Reversed           |       412.5 ns |      5.08 ns |      2.26 ns |  0.59 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **256**  | **PipeOrgan**          |     **1,200.4 ns** |     **16.04 ns** |      **7.12 ns** |  **1.63** |    **0.02** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | PipeOrgan          |       735.7 ns |     16.87 ns |      7.49 ns |  1.00 |    0.01 |    1 |         - |          NA |
| PigeonSort          | 256  | PipeOrgan          |     1,257.2 ns |    202.19 ns |    105.75 ns |  1.71 |    0.14 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | PipeOrgan          |       828.8 ns |      6.73 ns |      2.99 ns |  1.13 |    0.01 |    1 |         - |          NA |
| BucketSort          | 256  | PipeOrgan          |     5,946.0 ns |    382.38 ns |    199.99 ns |  8.08 |    0.27 |    5 |         - |          NA |
| BucketSortInteger   | 256  | PipeOrgan          |     2,044.4 ns |     18.24 ns |      9.54 ns |  2.78 |    0.03 |    3 |         - |          NA |
| FlashSort           | 256  | PipeOrgan          |     3,936.7 ns |    151.62 ns |     67.32 ns |  5.35 |    0.10 |    4 |         - |          NA |
| RadixLSD4Sort       | 256  | PipeOrgan          |     3,444.7 ns |     59.40 ns |     21.18 ns |  4.68 |    0.05 |    4 |         - |          NA |
| RadixLSD256Sort     | 256  | PipeOrgan          |     1,595.1 ns |     41.00 ns |     18.20 ns |  2.17 |    0.03 |    2 |         - |          NA |
| RadixLSD10Sort      | 256  | PipeOrgan          |     3,365.9 ns |     60.68 ns |     21.64 ns |  4.58 |    0.05 |    4 |         - |          NA |
| RadixMSD4Sort       | 256  | PipeOrgan          |     7,719.4 ns |    488.21 ns |    255.34 ns | 10.49 |    0.34 |    6 |         - |          NA |
| RadixMSD10Sort      | 256  | PipeOrgan          |    10,952.0 ns |     66.83 ns |     34.95 ns | 14.89 |    0.15 |    7 |         - |          NA |
| AmericanFlagSort    | 256  | PipeOrgan          |     3,359.3 ns |     40.47 ns |     14.43 ns |  4.57 |    0.05 |    4 |         - |          NA |
| SpreadSort          | 256  | PipeOrgan          |     1,346.3 ns |     49.16 ns |     25.71 ns |  1.83 |    0.04 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **Random**             |     **4,412.9 ns** |    **273.05 ns** |    **142.81 ns** |  **1.50** |    **0.05** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Random             |     2,944.7 ns |      7.48 ns |      3.32 ns |  1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 1024 | Random             |     4,534.3 ns |    240.12 ns |    125.59 ns |  1.54 |    0.04 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | Random             |     3,086.3 ns |    170.04 ns |     75.50 ns |  1.05 |    0.02 |    1 |         - |          NA |
| BucketSort          | 1024 | Random             |    38,920.7 ns |    225.81 ns |    100.26 ns | 13.22 |    0.03 |    7 |         - |          NA |
| BucketSortInteger   | 1024 | Random             |     8,852.4 ns |    344.02 ns |    179.93 ns |  3.01 |    0.06 |    4 |         - |          NA |
| FlashSort           | 1024 | Random             |    14,442.9 ns |     99.94 ns |     52.27 ns |  4.90 |    0.02 |    5 |         - |          NA |
| RadixLSD4Sort       | 1024 | Random             |    16,011.8 ns |    120.58 ns |     63.07 ns |  5.44 |    0.02 |    5 |         - |          NA |
| RadixLSD256Sort     | 1024 | Random             |     5,130.4 ns |    186.24 ns |     82.69 ns |  1.74 |    0.03 |    2 |         - |          NA |
| RadixLSD10Sort      | 1024 | Random             |    17,077.5 ns |     41.23 ns |     21.56 ns |  5.80 |    0.01 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | Random             |    26,946.2 ns |    352.63 ns |    184.43 ns |  9.15 |    0.06 |    6 |         - |          NA |
| RadixMSD10Sort      | 1024 | Random             |    40,474.8 ns |     70.64 ns |     31.36 ns | 13.74 |    0.02 |    7 |         - |          NA |
| AmericanFlagSort    | 1024 | Random             |    14,974.0 ns |    301.20 ns |    157.53 ns |  5.09 |    0.05 |    5 |         - |          NA |
| SpreadSort          | 1024 | Random             |     7,110.1 ns |    360.05 ns |    188.31 ns |  2.41 |    0.06 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **SingleElementMoved** |     **4,850.1 ns** |  **2,056.85 ns** |  **1,075.77 ns** |  **1.72** |    **0.37** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | SingleElementMoved |     2,830.1 ns |    267.33 ns |    118.70 ns |  1.00 |    0.05 |    1 |         - |          NA |
| PigeonSort          | 1024 | SingleElementMoved |     4,411.0 ns |    197.92 ns |     87.88 ns |  1.56 |    0.06 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | SingleElementMoved |     3,059.9 ns |    194.10 ns |    101.52 ns |  1.08 |    0.05 |    1 |         - |          NA |
| BucketSort          | 1024 | SingleElementMoved |     9,568.2 ns |      6.28 ns |      2.24 ns |  3.39 |    0.13 |    4 |         - |          NA |
| BucketSortInteger   | 1024 | SingleElementMoved |     5,859.6 ns |    183.06 ns |     95.75 ns |  2.07 |    0.08 |    3 |         - |          NA |
| FlashSort           | 1024 | SingleElementMoved |    16,471.2 ns |     73.10 ns |     32.46 ns |  5.83 |    0.22 |    5 |         - |          NA |
| RadixLSD4Sort       | 1024 | SingleElementMoved |    17,329.4 ns |    244.41 ns |    108.52 ns |  6.13 |    0.23 |    5 |         - |          NA |
| RadixLSD256Sort     | 1024 | SingleElementMoved |     5,682.0 ns |    289.16 ns |    151.23 ns |  2.01 |    0.09 |    3 |         - |          NA |
| RadixLSD10Sort      | 1024 | SingleElementMoved |    17,028.1 ns |     43.56 ns |     22.78 ns |  6.03 |    0.22 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | SingleElementMoved |    25,498.5 ns |    126.58 ns |     56.20 ns |  9.02 |    0.33 |    6 |         - |          NA |
| RadixMSD10Sort      | 1024 | SingleElementMoved |    39,565.9 ns |    474.51 ns |    248.18 ns | 14.00 |    0.52 |    7 |         - |          NA |
| AmericanFlagSort    | 1024 | SingleElementMoved |     9,953.7 ns |    217.04 ns |    113.51 ns |  3.52 |    0.14 |    4 |         - |          NA |
| SpreadSort          | 1024 | SingleElementMoved |     5,467.0 ns |    323.32 ns |    169.10 ns |  1.93 |    0.09 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **Sorted**             |     **4,486.0 ns** |    **115.85 ns** |     **60.59 ns** |  **1.55** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Sorted             |     2,891.9 ns |     47.36 ns |     16.89 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PigeonSort          | 1024 | Sorted             |     4,609.0 ns |    340.81 ns |    151.32 ns |  1.59 |    0.05 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | Sorted             |     2,711.1 ns |     10.80 ns |      4.79 ns |  0.94 |    0.01 |    2 |         - |          NA |
| BucketSort          | 1024 | Sorted             |     9,680.7 ns |     48.61 ns |     21.58 ns |  3.35 |    0.02 |    5 |         - |          NA |
| BucketSortInteger   | 1024 | Sorted             |     5,539.3 ns |    218.90 ns |    114.49 ns |  1.92 |    0.04 |    4 |         - |          NA |
| FlashSort           | 1024 | Sorted             |    16,913.5 ns |    117.32 ns |     52.09 ns |  5.85 |    0.04 |    6 |         - |          NA |
| RadixLSD4Sort       | 1024 | Sorted             |    16,851.1 ns |    492.87 ns |    257.78 ns |  5.83 |    0.09 |    6 |         - |          NA |
| RadixLSD256Sort     | 1024 | Sorted             |     4,610.0 ns |    317.49 ns |    166.06 ns |  1.59 |    0.05 |    3 |         - |          NA |
| RadixLSD10Sort      | 1024 | Sorted             |    17,524.6 ns |    685.75 ns |    358.66 ns |  6.06 |    0.12 |    6 |         - |          NA |
| RadixMSD4Sort       | 1024 | Sorted             |    25,139.5 ns |    103.82 ns |     46.10 ns |  8.69 |    0.05 |    7 |         - |          NA |
| RadixMSD10Sort      | 1024 | Sorted             |    39,563.3 ns |     98.03 ns |     43.52 ns | 13.68 |    0.07 |    8 |         - |          NA |
| AmericanFlagSort    | 1024 | Sorted             |     8,982.1 ns |    268.76 ns |    119.33 ns |  3.11 |    0.04 |    5 |         - |          NA |
| SpreadSort          | 1024 | Sorted             |       503.5 ns |     63.85 ns |     28.35 ns |  0.17 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **Reversed**           |     **4,172.3 ns** |    **327.43 ns** |    **171.25 ns** |  **1.59** |    **0.06** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Reversed           |     2,629.9 ns |     32.20 ns |     14.30 ns |  1.00 |    0.01 |    1 |         - |          NA |
| PigeonSort          | 1024 | Reversed           |     4,224.7 ns |    193.79 ns |     86.04 ns |  1.61 |    0.03 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | Reversed           |     2,836.3 ns |     49.12 ns |     21.81 ns |  1.08 |    0.01 |    1 |         - |          NA |
| BucketSort          | 1024 | Reversed           |    63,470.5 ns |    272.75 ns |    121.10 ns | 24.13 |    0.13 |    6 |         - |          NA |
| BucketSortInteger   | 1024 | Reversed           |    12,455.4 ns |    158.37 ns |     82.83 ns |  4.74 |    0.04 |    3 |         - |          NA |
| FlashSort           | 1024 | Reversed           |    14,285.2 ns |    170.93 ns |     89.40 ns |  5.43 |    0.04 |    3 |         - |          NA |
| RadixLSD4Sort       | 1024 | Reversed           |    19,157.3 ns |    189.22 ns |     98.97 ns |  7.28 |    0.05 |    3 |         - |          NA |
| RadixLSD256Sort     | 1024 | Reversed           |     5,069.5 ns |    261.10 ns |    136.56 ns |  1.93 |    0.05 |    2 |         - |          NA |
| RadixLSD10Sort      | 1024 | Reversed           |    16,988.7 ns |     84.07 ns |     37.33 ns |  6.46 |    0.04 |    3 |         - |          NA |
| RadixMSD4Sort       | 1024 | Reversed           |    28,554.0 ns |    584.12 ns |    259.35 ns | 10.86 |    0.11 |    4 |         - |          NA |
| RadixMSD10Sort      | 1024 | Reversed           |    41,441.7 ns |    131.89 ns |     68.98 ns | 15.76 |    0.08 |    5 |         - |          NA |
| AmericanFlagSort    | 1024 | Reversed           |    13,344.8 ns |    652.39 ns |    341.21 ns |  5.07 |    0.13 |    3 |         - |          NA |
| SpreadSort          | 1024 | Reversed           |     3,975.6 ns |    329.54 ns |    172.36 ns |  1.51 |    0.06 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **PipeOrgan**          |     **4,231.4 ns** |     **28.77 ns** |     **10.26 ns** |  **1.52** |    **0.00** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | PipeOrgan          |     2,788.2 ns |     12.16 ns |      5.40 ns |  1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 1024 | PipeOrgan          |     4,453.5 ns |    349.96 ns |    183.03 ns |  1.60 |    0.06 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | PipeOrgan          |     2,983.4 ns |     13.77 ns |      4.91 ns |  1.07 |    0.00 |    1 |         - |          NA |
| BucketSort          | 1024 | PipeOrgan          |    37,951.2 ns |    181.93 ns |     80.78 ns | 13.61 |    0.04 |    6 |         - |          NA |
| BucketSortInteger   | 1024 | PipeOrgan          |     9,117.3 ns |    359.71 ns |    159.71 ns |  3.27 |    0.05 |    3 |         - |          NA |
| FlashSort           | 1024 | PipeOrgan          |    15,073.3 ns |    197.38 ns |     70.39 ns |  5.41 |    0.03 |    4 |         - |          NA |
| RadixLSD4Sort       | 1024 | PipeOrgan          |    18,020.5 ns |    155.24 ns |     81.19 ns |  6.46 |    0.03 |    4 |         - |          NA |
| RadixLSD256Sort     | 1024 | PipeOrgan          |     5,165.7 ns |    243.65 ns |    127.44 ns |  1.85 |    0.04 |    2 |         - |          NA |
| RadixLSD10Sort      | 1024 | PipeOrgan          |    17,241.9 ns |    133.29 ns |     59.18 ns |  6.18 |    0.02 |    4 |         - |          NA |
| RadixMSD4Sort       | 1024 | PipeOrgan          |    28,961.5 ns |    418.19 ns |    218.72 ns | 10.39 |    0.08 |    5 |         - |          NA |
| RadixMSD10Sort      | 1024 | PipeOrgan          |    41,190.5 ns |    227.76 ns |    119.12 ns | 14.77 |    0.05 |    6 |         - |          NA |
| AmericanFlagSort    | 1024 | PipeOrgan          |    13,967.8 ns |    528.90 ns |    276.63 ns |  5.01 |    0.09 |    4 |         - |          NA |
| SpreadSort          | 1024 | PipeOrgan          |     5,766.9 ns |    286.18 ns |    149.68 ns |  2.07 |    0.05 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **4096** | **Random**             |    **18,011.6 ns** |    **551.61 ns** |    **244.92 ns** |  **1.48** |    **0.03** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Random             |    12,209.0 ns |    358.91 ns |    159.36 ns |  1.00 |    0.02 |    1 |         - |          NA |
| PigeonSort          | 4096 | Random             |    18,912.7 ns |    826.92 ns |    432.49 ns |  1.55 |    0.04 |    2 |         - |          NA |
| PigeonSortInteger   | 4096 | Random             |    12,528.7 ns |    324.01 ns |    115.55 ns |  1.03 |    0.02 |    1 |         - |          NA |
| BucketSort          | 4096 | Random             |   286,825.0 ns |    427.83 ns |    223.76 ns | 23.50 |    0.29 |    7 |         - |          NA |
| BucketSortInteger   | 4096 | Random             |    52,186.7 ns |  1,015.37 ns |    450.83 ns |  4.28 |    0.06 |    4 |         - |          NA |
| FlashSort           | 4096 | Random             |    59,637.7 ns |    635.75 ns |    282.28 ns |  4.89 |    0.06 |    4 |         - |          NA |
| RadixLSD4Sort       | 4096 | Random             |    73,787.2 ns |    549.29 ns |    287.29 ns |  6.04 |    0.08 |    4 |         - |          NA |
| RadixLSD256Sort     | 4096 | Random             |    18,919.5 ns |    241.44 ns |    107.20 ns |  1.55 |    0.02 |    2 |         - |          NA |
| RadixLSD10Sort      | 4096 | Random             |    70,015.7 ns |    561.07 ns |    249.12 ns |  5.74 |    0.07 |    4 |         - |          NA |
| RadixMSD4Sort       | 4096 | Random             |   107,245.7 ns |  1,425.31 ns |    632.85 ns |  8.79 |    0.12 |    5 |         - |          NA |
| RadixMSD10Sort      | 4096 | Random             |   161,849.1 ns |    569.16 ns |    297.68 ns | 13.26 |    0.16 |    6 |         - |          NA |
| AmericanFlagSort    | 4096 | Random             |    65,291.2 ns |  1,357.45 ns |    709.97 ns |  5.35 |    0.09 |    4 |         - |          NA |
| SpreadSort          | 4096 | Random             |    30,137.1 ns |    744.00 ns |    330.34 ns |  2.47 |    0.04 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **4096** | **SingleElementMoved** |    **16,409.9 ns** |    **696.04 ns** |    **309.04 ns** |  **1.46** |    **0.03** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 4096 | SingleElementMoved |    11,275.2 ns |    320.14 ns |    142.15 ns |  1.00 |    0.02 |    1 |         - |          NA |
| PigeonSort          | 4096 | SingleElementMoved |    17,711.8 ns |    564.45 ns |    295.22 ns |  1.57 |    0.03 |    2 |         - |          NA |
| PigeonSortInteger   | 4096 | SingleElementMoved |    12,074.9 ns |    317.04 ns |    140.77 ns |  1.07 |    0.02 |    1 |         - |          NA |
| BucketSort          | 4096 | SingleElementMoved |    37,872.3 ns |    301.74 ns |    157.82 ns |  3.36 |    0.04 |    3 |         - |          NA |
| BucketSortInteger   | 4096 | SingleElementMoved |    21,245.8 ns |    366.42 ns |    162.69 ns |  1.88 |    0.03 |    2 |         - |          NA |
| FlashSort           | 4096 | SingleElementMoved |    65,926.6 ns |    532.60 ns |    278.56 ns |  5.85 |    0.07 |    4 |         - |          NA |
| RadixLSD4Sort       | 4096 | SingleElementMoved |    83,872.0 ns |    520.13 ns |    272.04 ns |  7.44 |    0.09 |    5 |         - |          NA |
| RadixLSD256Sort     | 4096 | SingleElementMoved |    18,726.2 ns |    437.15 ns |    228.64 ns |  1.66 |    0.03 |    2 |         - |          NA |
| RadixLSD10Sort      | 4096 | SingleElementMoved |    67,924.1 ns |    725.53 ns |    322.14 ns |  6.03 |    0.08 |    4 |         - |          NA |
| RadixMSD4Sort       | 4096 | SingleElementMoved |   100,562.7 ns |    605.92 ns |    269.03 ns |  8.92 |    0.11 |    5 |         - |          NA |
| RadixMSD10Sort      | 4096 | SingleElementMoved |   155,888.2 ns |    439.90 ns |    230.08 ns | 13.83 |    0.16 |    6 |         - |          NA |
| AmericanFlagSort    | 4096 | SingleElementMoved |    43,262.4 ns |    731.74 ns |    382.71 ns |  3.84 |    0.06 |    3 |         - |          NA |
| SpreadSort          | 4096 | SingleElementMoved |    21,707.1 ns |    173.08 ns |     61.72 ns |  1.93 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **4096** | **Sorted**             |    **17,397.8 ns** |    **267.16 ns** |    **118.62 ns** |  **1.49** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Sorted             |    11,709.2 ns |    406.06 ns |    180.29 ns |  1.00 |    0.02 |    2 |         - |          NA |
| PigeonSort          | 4096 | Sorted             |    18,213.2 ns |    530.79 ns |    235.68 ns |  1.56 |    0.03 |    3 |         - |          NA |
| PigeonSortInteger   | 4096 | Sorted             |    10,977.1 ns |    739.54 ns |    328.36 ns |  0.94 |    0.03 |    2 |         - |          NA |
| BucketSort          | 4096 | Sorted             |    37,513.4 ns |  1,696.86 ns |    887.49 ns |  3.20 |    0.08 |    4 |         - |          NA |
| BucketSortInteger   | 4096 | Sorted             |    20,129.4 ns |    543.25 ns |    284.13 ns |  1.72 |    0.03 |    3 |         - |          NA |
| FlashSort           | 4096 | Sorted             |    67,813.2 ns |    678.82 ns |    301.40 ns |  5.79 |    0.09 |    5 |         - |          NA |
| RadixLSD4Sort       | 4096 | Sorted             |    79,706.6 ns |    721.46 ns |    320.33 ns |  6.81 |    0.10 |    5 |         - |          NA |
| RadixLSD256Sort     | 4096 | Sorted             |    16,564.9 ns |  1,103.79 ns |    577.31 ns |  1.41 |    0.05 |    3 |         - |          NA |
| RadixLSD10Sort      | 4096 | Sorted             |    68,484.4 ns |    629.91 ns |    279.69 ns |  5.85 |    0.09 |    5 |         - |          NA |
| RadixMSD4Sort       | 4096 | Sorted             |    99,385.5 ns |    881.44 ns |    461.01 ns |  8.49 |    0.13 |    6 |         - |          NA |
| RadixMSD10Sort      | 4096 | Sorted             |   156,618.4 ns |    727.36 ns |    380.42 ns | 13.38 |    0.19 |    7 |         - |          NA |
| AmericanFlagSort    | 4096 | Sorted             |    37,948.5 ns |    445.85 ns |    197.96 ns |  3.24 |    0.05 |    4 |         - |          NA |
| SpreadSort          | 4096 | Sorted             |     1,754.6 ns |      3.21 ns |      1.68 ns |  0.15 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **4096** | **Reversed**           |    **16,357.6 ns** |    **852.71 ns** |    **445.98 ns** |  **1.47** |    **0.08** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Reversed           |    11,187.2 ns |  1,489.96 ns |    661.55 ns |  1.00 |    0.08 |    1 |         - |          NA |
| PigeonSort          | 4096 | Reversed           |    17,337.9 ns |    261.95 ns |    116.31 ns |  1.55 |    0.08 |    2 |         - |          NA |
| PigeonSortInteger   | 4096 | Reversed           |    10,865.5 ns |    326.50 ns |    144.97 ns |  0.97 |    0.05 |    1 |         - |          NA |
| BucketSort          | 4096 | Reversed           |   486,669.8 ns |  1,287.70 ns |    673.49 ns | 43.62 |    2.23 |    6 |         - |          NA |
| BucketSortInteger   | 4096 | Reversed           |    77,957.4 ns |  1,182.56 ns |    525.06 ns |  6.99 |    0.36 |    3 |         - |          NA |
| FlashSort           | 4096 | Reversed           |    72,329.3 ns |  6,630.88 ns |  3,468.08 ns |  6.48 |    0.44 |    3 |         - |          NA |
| RadixLSD4Sort       | 4096 | Reversed           |    80,613.1 ns |  1,016.43 ns |    531.61 ns |  7.23 |    0.37 |    3 |         - |          NA |
| RadixLSD256Sort     | 4096 | Reversed           |    21,417.9 ns |  8,664.72 ns |  4,531.82 ns |  1.92 |    0.40 |    2 |         - |          NA |
| RadixLSD10Sort      | 4096 | Reversed           |    67,157.5 ns |    555.29 ns |    246.55 ns |  6.02 |    0.31 |    3 |         - |          NA |
| RadixMSD4Sort       | 4096 | Reversed           |   114,544.4 ns |    702.68 ns |    311.99 ns | 10.27 |    0.53 |    4 |         - |          NA |
| RadixMSD10Sort      | 4096 | Reversed           |   163,582.1 ns |    265.47 ns |    117.87 ns | 14.66 |    0.75 |    5 |         - |          NA |
| AmericanFlagSort    | 4096 | Reversed           |    64,761.6 ns | 18,805.39 ns |  9,835.58 ns |  5.80 |    0.88 |    3 |         - |          NA |
| SpreadSort          | 4096 | Reversed           |    15,488.1 ns |    223.41 ns |     99.20 ns |  1.39 |    0.07 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **4096** | **PipeOrgan**          |    **18,836.5 ns** |    **665.03 ns** |    **347.82 ns** |  **1.65** |    **0.04** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 4096 | PipeOrgan          |    11,423.0 ns |    380.01 ns |    168.73 ns |  1.00 |    0.02 |    1 |         - |          NA |
| PigeonSort          | 4096 | PipeOrgan          |    17,628.5 ns |    295.04 ns |    131.00 ns |  1.54 |    0.02 |    2 |         - |          NA |
| PigeonSortInteger   | 4096 | PipeOrgan          |    11,981.3 ns |    289.05 ns |    128.34 ns |  1.05 |    0.02 |    1 |         - |          NA |
| BucketSort          | 4096 | PipeOrgan          |   275,527.7 ns | 15,993.59 ns |  8,364.96 ns | 24.12 |    0.77 |    7 |         - |          NA |
| BucketSortInteger   | 4096 | PipeOrgan          |    51,325.5 ns |  1,508.30 ns |    788.87 ns |  4.49 |    0.09 |    4 |         - |          NA |
| FlashSort           | 4096 | PipeOrgan          |    57,294.2 ns |    339.16 ns |    177.39 ns |  5.02 |    0.07 |    4 |         - |          NA |
| RadixLSD4Sort       | 4096 | PipeOrgan          |    81,999.4 ns |    909.06 ns |    475.46 ns |  7.18 |    0.11 |    4 |         - |          NA |
| RadixLSD256Sort     | 4096 | PipeOrgan          |    19,224.9 ns |    865.64 ns |    384.35 ns |  1.68 |    0.04 |    2 |         - |          NA |
| RadixLSD10Sort      | 4096 | PipeOrgan          |    68,441.8 ns |    324.38 ns |    169.66 ns |  5.99 |    0.08 |    4 |         - |          NA |
| RadixMSD4Sort       | 4096 | PipeOrgan          |   114,575.5 ns |  1,169.29 ns |    519.17 ns | 10.03 |    0.14 |    5 |         - |          NA |
| RadixMSD10Sort      | 4096 | PipeOrgan          |   162,442.8 ns |    602.01 ns |    267.30 ns | 14.22 |    0.20 |    6 |         - |          NA |
| AmericanFlagSort    | 4096 | PipeOrgan          |    62,339.3 ns |  1,082.22 ns |    566.02 ns |  5.46 |    0.09 |    4 |         - |          NA |
| SpreadSort          | 4096 | PipeOrgan          |    23,610.2 ns |    511.05 ns |    182.24 ns |  2.07 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **Random**             |    **37,013.7 ns** |    **506.68 ns** |    **224.97 ns** |  **1.44** |    **0.02** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Random             |    25,750.8 ns |    574.27 ns |    254.98 ns |  1.00 |    0.01 |    1 |         - |          NA |
| PigeonSort          | 8192 | Random             |    37,576.0 ns |    695.33 ns |    308.73 ns |  1.46 |    0.02 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | Random             |    25,528.8 ns |  1,165.82 ns |    609.75 ns |  0.99 |    0.02 |    1 |         - |          NA |
| BucketSort          | 8192 | Random             |   802,541.9 ns |  3,625.01 ns |  1,609.53 ns | 31.17 |    0.29 |    7 |         - |          NA |
| BucketSortInteger   | 8192 | Random             |   160,455.7 ns |  2,071.07 ns |    919.57 ns |  6.23 |    0.07 |    4 |         - |          NA |
| FlashSort           | 8192 | Random             |   130,323.1 ns |    491.82 ns |    218.37 ns |  5.06 |    0.05 |    4 |         - |          NA |
| RadixLSD4Sort       | 8192 | Random             |   149,115.4 ns |    570.21 ns |    253.18 ns |  5.79 |    0.05 |    4 |         - |          NA |
| RadixLSD256Sort     | 8192 | Random             |    38,724.0 ns |  1,694.95 ns |    886.49 ns |  1.50 |    0.04 |    2 |         - |          NA |
| RadixLSD10Sort      | 8192 | Random             |   137,662.2 ns |    968.06 ns |    506.31 ns |  5.35 |    0.05 |    4 |         - |          NA |
| RadixMSD4Sort       | 8192 | Random             |   226,199.0 ns |  1,432.93 ns |    749.45 ns |  8.78 |    0.08 |    5 |         - |          NA |
| RadixMSD10Sort      | 8192 | Random             |   325,992.9 ns |  1,761.81 ns |    782.26 ns | 12.66 |    0.12 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | Random             |   139,336.2 ns |  1,552.48 ns |    811.98 ns |  5.41 |    0.06 |    4 |         - |          NA |
| SpreadSort          | 8192 | Random             |    75,134.0 ns |  1,594.29 ns |    707.87 ns |  2.92 |    0.04 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **SingleElementMoved** |    **33,565.1 ns** |    **606.16 ns** |    **269.14 ns** |  **1.45** |    **0.04** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | SingleElementMoved |    23,234.9 ns |  1,130.62 ns |    591.34 ns |  1.00 |    0.03 |    1 |         - |          NA |
| PigeonSort          | 8192 | SingleElementMoved |    35,026.4 ns |  1,838.93 ns |    961.80 ns |  1.51 |    0.05 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | SingleElementMoved |    23,775.3 ns |    498.01 ns |    260.47 ns |  1.02 |    0.03 |    1 |         - |          NA |
| BucketSort          | 8192 | SingleElementMoved |    73,255.6 ns |  1,374.71 ns |    719.00 ns |  3.15 |    0.08 |    3 |         - |          NA |
| BucketSortInteger   | 8192 | SingleElementMoved |    41,524.9 ns |  1,217.26 ns |    636.65 ns |  1.79 |    0.05 |    2 |         - |          NA |
| FlashSort           | 8192 | SingleElementMoved |   164,679.4 ns | 19,946.97 ns | 10,432.65 ns |  7.09 |    0.46 |    4 |         - |          NA |
| RadixLSD4Sort       | 8192 | SingleElementMoved |   159,781.2 ns |  1,003.30 ns |    524.75 ns |  6.88 |    0.16 |    4 |         - |          NA |
| RadixLSD256Sort     | 8192 | SingleElementMoved |    38,658.4 ns |  1,200.97 ns |    533.24 ns |  1.66 |    0.04 |    2 |         - |          NA |
| RadixLSD10Sort      | 8192 | SingleElementMoved |   138,904.2 ns |  1,244.37 ns |    650.83 ns |  5.98 |    0.14 |    4 |         - |          NA |
| RadixMSD4Sort       | 8192 | SingleElementMoved |   203,523.8 ns |  1,207.56 ns |    536.16 ns |  8.76 |    0.21 |    5 |         - |          NA |
| RadixMSD10Sort      | 8192 | SingleElementMoved |   312,475.1 ns |    394.59 ns |    206.38 ns | 13.46 |    0.32 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | SingleElementMoved |    86,722.4 ns |    356.60 ns |    158.33 ns |  3.73 |    0.09 |    3 |         - |          NA |
| SpreadSort          | 8192 | SingleElementMoved |    43,721.3 ns |  1,104.13 ns |    577.48 ns |  1.88 |    0.05 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **Sorted**             |    **35,553.9 ns** |  **1,054.68 ns** |    **551.62 ns** |  **1.45** |    **0.05** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Sorted             |    24,586.5 ns |  1,666.45 ns |    871.59 ns |  1.00 |    0.05 |    2 |         - |          NA |
| PigeonSort          | 8192 | Sorted             |    35,876.7 ns |  1,254.68 ns |    557.09 ns |  1.46 |    0.05 |    3 |         - |          NA |
| PigeonSortInteger   | 8192 | Sorted             |    21,570.2 ns |    776.45 ns |    344.75 ns |  0.88 |    0.03 |    2 |         - |          NA |
| BucketSort          | 8192 | Sorted             |    73,165.0 ns |  1,066.42 ns |    473.50 ns |  2.98 |    0.10 |    4 |         - |          NA |
| BucketSortInteger   | 8192 | Sorted             |    38,696.2 ns |    782.57 ns |    347.46 ns |  1.58 |    0.05 |    3 |         - |          NA |
| FlashSort           | 8192 | Sorted             |   134,974.2 ns |    909.97 ns |    404.03 ns |  5.50 |    0.18 |    5 |         - |          NA |
| RadixLSD4Sort       | 8192 | Sorted             |   161,827.6 ns |    885.36 ns |    463.06 ns |  6.59 |    0.22 |    5 |         - |          NA |
| RadixLSD256Sort     | 8192 | Sorted             |    34,265.5 ns |  1,279.23 ns |    669.06 ns |  1.40 |    0.05 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | Sorted             |   137,530.9 ns |    483.02 ns |    214.46 ns |  5.60 |    0.19 |    5 |         - |          NA |
| RadixMSD4Sort       | 8192 | Sorted             |   201,285.6 ns |    992.80 ns |    519.25 ns |  8.20 |    0.27 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | Sorted             |   312,993.3 ns |  1,951.07 ns |    866.29 ns | 12.74 |    0.43 |    7 |         - |          NA |
| AmericanFlagSort    | 8192 | Sorted             |    76,383.1 ns |    585.52 ns |    306.24 ns |  3.11 |    0.10 |    4 |         - |          NA |
| SpreadSort          | 8192 | Sorted             |     3,462.3 ns |     26.96 ns |      9.61 ns |  0.14 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **Reversed**           |    **32,796.3 ns** |  **2,126.98 ns** |    **944.39 ns** |  **1.53** |    **0.04** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Reversed           |    21,437.7 ns |    126.01 ns |     55.95 ns |  1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 8192 | Reversed           |    34,395.5 ns |    201.37 ns |     71.81 ns |  1.60 |    0.00 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | Reversed           |    21,542.4 ns |    415.44 ns |    217.28 ns |  1.00 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | Reversed           | 1,388,587.5 ns |  2,636.56 ns |  1,378.97 ns | 64.77 |    0.17 |    7 |         - |          NA |
| BucketSortInteger   | 8192 | Reversed           |   206,007.9 ns |    969.41 ns |    507.02 ns |  9.61 |    0.03 |    5 |         - |          NA |
| FlashSort           | 8192 | Reversed           |   150,891.8 ns | 10,200.80 ns |  5,335.22 ns |  7.04 |    0.24 |    4 |         - |          NA |
| RadixLSD4Sort       | 8192 | Reversed           |   164,357.0 ns |  1,277.59 ns |    668.21 ns |  7.67 |    0.03 |    4 |         - |          NA |
| RadixLSD256Sort     | 8192 | Reversed           |    36,808.1 ns |    508.59 ns |    225.82 ns |  1.72 |    0.01 |    2 |         - |          NA |
| RadixLSD10Sort      | 8192 | Reversed           |   135,676.9 ns |    871.00 ns |    386.73 ns |  6.33 |    0.02 |    4 |         - |          NA |
| RadixMSD4Sort       | 8192 | Reversed           |   231,072.6 ns |    707.35 ns |    369.96 ns | 10.78 |    0.03 |    5 |         - |          NA |
| RadixMSD10Sort      | 8192 | Reversed           |   328,713.4 ns |    750.14 ns |    392.34 ns | 15.33 |    0.04 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | Reversed           |   119,802.4 ns |  1,973.44 ns |  1,032.15 ns |  5.59 |    0.05 |    4 |         - |          NA |
| SpreadSort          | 8192 | Reversed           |    58,990.7 ns |    521.65 ns |    272.83 ns |  2.75 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **PipeOrgan**          |    **33,245.1 ns** |  **1,225.57 ns** |    **544.16 ns** |  **1.44** |    **0.03** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | PipeOrgan          |    23,166.9 ns |    762.68 ns |    338.63 ns |  1.00 |    0.02 |    1 |         - |          NA |
| PigeonSort          | 8192 | PipeOrgan          |    34,465.3 ns |  1,195.75 ns |    530.92 ns |  1.49 |    0.03 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | PipeOrgan          |    23,582.1 ns |    170.41 ns |     60.77 ns |  1.02 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | PipeOrgan          |   758,899.7 ns | 46,720.85 ns | 24,435.91 ns | 32.76 |    1.09 |    7 |         - |          NA |
| BucketSortInteger   | 8192 | PipeOrgan          |   127,191.3 ns |  1,844.44 ns |    964.68 ns |  5.49 |    0.08 |    4 |         - |          NA |
| FlashSort           | 8192 | PipeOrgan          |   124,502.4 ns |  1,180.79 ns |    524.28 ns |  5.38 |    0.08 |    4 |         - |          NA |
| RadixLSD4Sort       | 8192 | PipeOrgan          |   160,752.3 ns |    719.07 ns |    376.09 ns |  6.94 |    0.09 |    4 |         - |          NA |
| RadixLSD256Sort     | 8192 | PipeOrgan          |    39,697.7 ns |    773.12 ns |    404.36 ns |  1.71 |    0.03 |    2 |         - |          NA |
| RadixLSD10Sort      | 8192 | PipeOrgan          |   136,837.3 ns |    558.77 ns |    292.25 ns |  5.91 |    0.08 |    4 |         - |          NA |
| RadixMSD4Sort       | 8192 | PipeOrgan          |   233,775.6 ns |  1,155.19 ns |    604.19 ns | 10.09 |    0.14 |    5 |         - |          NA |
| RadixMSD10Sort      | 8192 | PipeOrgan          |   326,530.6 ns |    469.52 ns |    245.57 ns | 14.10 |    0.19 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | PipeOrgan          |   124,641.6 ns |  1,563.06 ns |    694.01 ns |  5.38 |    0.08 |    4 |         - |          NA |
| SpreadSort          | 8192 | PipeOrgan          |    74,773.0 ns |    455.67 ns |    238.32 ns |  3.23 |    0.04 |    3 |         - |          NA |

### ExchangeBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method             | Size | Pattern            | Mean         | Error       | StdDev      | Ratio  | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------- |----- |------------------- |-------------:|------------:|------------:|-------:|--------:|-----:|----------:|------------:|
| **BubbleSort**         | **256**  | **Random**             |  **36,587.3 ns** | **1,161.80 ns** |   **607.64 ns** |   **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Random             |  13,030.9 ns |   153.32 ns |    80.19 ns |   0.36 |    0.01 |    2 |         - |          NA |
| OddEvenSort        | 256  | Random             |  17,031.0 ns |   365.83 ns |   191.34 ns |   0.47 |    0.01 |    2 |         - |          NA |
| CombSort           | 256  | Random             |   2,760.8 ns |   173.34 ns |    76.96 ns |   0.08 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | Random             |  14,236.5 ns |   217.48 ns |   113.75 ns |   0.39 |    0.01 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **SingleElementMoved** |     **275.2 ns** |   **102.68 ns** |    **45.59 ns** |   **1.02** |    **0.21** |    **1** |         **-** |          **NA** |
| CocktailShakerSort | 256  | SingleElementMoved |     351.6 ns |   129.52 ns |    67.74 ns |   1.30 |    0.29 |    2 |         - |          NA |
| OddEvenSort        | 256  | SingleElementMoved |  11,160.5 ns |   411.78 ns |   215.37 ns |  41.36 |    5.40 |    4 |         - |          NA |
| CombSort           | 256  | SingleElementMoved |   2,258.5 ns |    14.01 ns |     4.99 ns |   8.37 |    1.09 |    3 |         - |          NA |
| CircleSort         | 256  | SingleElementMoved |  12,207.1 ns |   163.60 ns |    85.56 ns |  45.24 |    5.86 |    4 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **Sorted**             |     **214.3 ns** |     **1.66 ns** |     **0.74 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Sorted             |     154.3 ns |     1.03 ns |     0.54 ns |   0.72 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 256  | Sorted             |     175.9 ns |     0.90 ns |     0.40 ns |   0.82 |    0.00 |    1 |         - |          NA |
| CombSort           | 256  | Sorted             |   2,295.8 ns |    15.61 ns |     6.93 ns |  10.71 |    0.05 |    4 |         - |          NA |
| CircleSort         | 256  | Sorted             |   1,672.3 ns |    11.72 ns |     5.20 ns |   7.80 |    0.03 |    3 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **Reversed**           |  **23,004.0 ns** |   **243.59 ns** |   **127.40 ns** |   **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Reversed           |  21,629.7 ns |   146.99 ns |    65.27 ns |   0.94 |    0.01 |    3 |         - |          NA |
| OddEvenSort        | 256  | Reversed           |  19,705.4 ns |   221.71 ns |    98.44 ns |   0.86 |    0.01 |    3 |         - |          NA |
| CombSort           | 256  | Reversed           |   2,447.3 ns |   271.01 ns |   141.74 ns |   0.11 |    0.01 |    1 |         - |          NA |
| CircleSort         | 256  | Reversed           |   3,567.9 ns |   216.52 ns |   113.25 ns |   0.16 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **PipeOrgan**          |  **22,779.6 ns** |   **527.06 ns** |   **275.66 ns** |   **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | PipeOrgan          |  13,626.8 ns |   146.88 ns |    76.82 ns |   0.60 |    0.01 |    2 |         - |          NA |
| OddEvenSort        | 256  | PipeOrgan          |  19,675.9 ns |   409.76 ns |   214.31 ns |   0.86 |    0.01 |    3 |         - |          NA |
| CombSort           | 256  | PipeOrgan          |   2,448.4 ns |   261.61 ns |   116.16 ns |   0.11 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | PipeOrgan          |  15,503.6 ns |    88.65 ns |    39.36 ns |   0.68 |    0.01 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Random**             | **413,288.3 ns** | **1,180.11 ns** |   **617.22 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Random             | 251,707.7 ns | 1,084.15 ns |   567.03 ns |   0.61 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | Random             | 367,460.8 ns | 2,347.63 ns | 1,227.86 ns |   0.89 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | Random             |  15,177.9 ns |   215.46 ns |   112.69 ns |   0.04 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | Random             |  78,191.9 ns | 1,237.60 ns |   549.50 ns |   0.19 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **SingleElementMoved** |   **1,017.5 ns** |    **53.54 ns** |    **19.09 ns** |   **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | SingleElementMoved |   1,015.0 ns |    20.68 ns |     7.37 ns |   1.00 |    0.02 |    1 |         - |          NA |
| OddEvenSort        | 1024 | SingleElementMoved | 159,842.0 ns | 3,414.16 ns | 1,785.67 ns | 157.14 |    3.14 |    4 |         - |          NA |
| CombSort           | 1024 | SingleElementMoved |  12,135.1 ns |   299.26 ns |   132.87 ns |  11.93 |    0.24 |    2 |         - |          NA |
| CircleSort         | 1024 | SingleElementMoved |  68,763.4 ns | 1,539.69 ns |   805.29 ns |  67.60 |    1.37 |    3 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Sorted**             |     **838.5 ns** |     **1.63 ns** |     **0.72 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Sorted             |     574.2 ns |    18.07 ns |     8.02 ns |   0.68 |    0.01 |    1 |         - |          NA |
| OddEvenSort        | 1024 | Sorted             |     707.5 ns |     3.44 ns |     1.80 ns |   0.84 |    0.00 |    2 |         - |          NA |
| CombSort           | 1024 | Sorted             |  11,272.6 ns |   129.28 ns |    67.62 ns |  13.44 |    0.08 |    4 |         - |          NA |
| CircleSort         | 1024 | Sorted             |   7,494.0 ns |   304.74 ns |   159.38 ns |   8.94 |    0.18 |    3 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Reversed**           | **339,109.4 ns** |   **725.60 ns** |   **379.50 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Reversed           | 338,263.8 ns |   985.99 ns |   515.69 ns |   1.00 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | Reversed           | 313,879.0 ns | 1,256.74 ns |   558.00 ns |   0.93 |    0.00 |    3 |         - |          NA |
| CombSort           | 1024 | Reversed           |  12,501.2 ns |   217.48 ns |   113.75 ns |   0.04 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | Reversed           |  15,505.2 ns |   124.67 ns |    65.20 ns |   0.05 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **PipeOrgan**          | **322,232.3 ns** |   **753.89 ns** |   **334.73 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | PipeOrgan          | 221,379.5 ns | 3,381.76 ns | 1,768.73 ns |   0.69 |    0.01 |    3 |         - |          NA |
| OddEvenSort        | 1024 | PipeOrgan          | 316,407.6 ns | 3,029.53 ns | 1,584.50 ns |   0.98 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | PipeOrgan          |  12,529.3 ns |   187.02 ns |    83.04 ns |   0.04 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | PipeOrgan          |  84,977.8 ns | 1,878.42 ns |   982.45 ns |   0.26 |    0.00 |    2 |         - |          NA |

### HeapBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method           | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **HeapSort**         | **256**  | **Random**             |     **3,452.2 ns** |    **823.82 ns** |    **430.87 ns** |  **1.02** |    **0.18** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | Random             |     2,800.8 ns |    175.99 ns |     92.04 ns |  0.82 |    0.11 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | Random             |     3,256.9 ns |    303.90 ns |    158.95 ns |  0.96 |    0.13 |    1 |         - |          NA |
| BottomupHeapSort | 256  | Random             |     3,418.4 ns |    168.75 ns |     74.92 ns |  1.01 |    0.13 |    1 |         - |          NA |
| WeakHeapSort     | 256  | Random             |     7,918.2 ns |    202.89 ns |    106.11 ns |  2.33 |    0.31 |    3 |         - |          NA |
| SmoothSort       | 256  | Random             |     4,186.6 ns |    260.97 ns |    115.87 ns |  1.23 |    0.17 |    1 |         - |          NA |
| TournamentSort   | 256  | Random             |     6,316.0 ns |    364.99 ns |    190.89 ns |  1.86 |    0.25 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **SingleElementMoved** |     **2,536.1 ns** |    **113.85 ns** |     **50.55 ns** |  **1.00** |    **0.03** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 256  | SingleElementMoved |     2,732.2 ns |     93.42 ns |     41.48 ns |  1.08 |    0.02 |    2 |         - |          NA |
| TernaryHeapSort  | 256  | SingleElementMoved |     3,380.3 ns |    328.33 ns |    171.72 ns |  1.33 |    0.07 |    3 |         - |          NA |
| BottomupHeapSort | 256  | SingleElementMoved |     3,447.2 ns |    183.08 ns |     95.75 ns |  1.36 |    0.04 |    3 |         - |          NA |
| WeakHeapSort     | 256  | SingleElementMoved |     6,744.2 ns |    242.15 ns |    126.65 ns |  2.66 |    0.07 |    5 |         - |          NA |
| SmoothSort       | 256  | SingleElementMoved |     1,375.0 ns |     28.12 ns |     12.49 ns |  0.54 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 256  | SingleElementMoved |     4,676.0 ns |     68.37 ns |     24.38 ns |  1.84 |    0.03 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **Sorted**             |     **2,155.1 ns** |    **186.46 ns** |     **82.79 ns** |  **1.00** |    **0.05** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 256  | Sorted             |     2,449.0 ns |    123.48 ns |     54.82 ns |  1.14 |    0.05 |    2 |         - |          NA |
| TernaryHeapSort  | 256  | Sorted             |     3,536.1 ns |    260.17 ns |    115.52 ns |  1.64 |    0.08 |    3 |         - |          NA |
| BottomupHeapSort | 256  | Sorted             |     3,486.1 ns |    182.38 ns |     95.39 ns |  1.62 |    0.07 |    3 |         - |          NA |
| WeakHeapSort     | 256  | Sorted             |     6,709.6 ns |    269.93 ns |    141.18 ns |  3.12 |    0.13 |    4 |         - |          NA |
| SmoothSort       | 256  | Sorted             |       998.7 ns |    138.94 ns |     61.69 ns |  0.46 |    0.03 |    1 |         - |          NA |
| TournamentSort   | 256  | Sorted             |     2,759.7 ns |    287.90 ns |    150.57 ns |  1.28 |    0.08 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **Reversed**           |     **2,692.1 ns** |    **132.49 ns** |     **58.83 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | Reversed           |     2,648.1 ns |    208.30 ns |    108.94 ns |  0.98 |    0.04 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | Reversed           |     3,252.1 ns |    256.52 ns |    134.17 ns |  1.21 |    0.05 |    2 |         - |          NA |
| BottomupHeapSort | 256  | Reversed           |     3,545.1 ns |    147.69 ns |     77.24 ns |  1.32 |    0.04 |    2 |         - |          NA |
| WeakHeapSort     | 256  | Reversed           |     7,513.1 ns |    270.17 ns |    141.30 ns |  2.79 |    0.08 |    3 |         - |          NA |
| SmoothSort       | 256  | Reversed           |     3,831.4 ns |    248.76 ns |    110.45 ns |  1.42 |    0.05 |    2 |         - |          NA |
| TournamentSort   | 256  | Reversed           |     4,580.6 ns |    210.41 ns |    110.05 ns |  1.70 |    0.05 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **PipeOrgan**          |     **2,588.0 ns** |    **103.24 ns** |     **45.84 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | PipeOrgan          |     2,588.1 ns |     39.67 ns |     17.62 ns |  1.00 |    0.02 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | PipeOrgan          |     3,079.7 ns |    174.80 ns |     77.61 ns |  1.19 |    0.03 |    1 |         - |          NA |
| BottomupHeapSort | 256  | PipeOrgan          |     3,513.4 ns |    261.88 ns |    136.97 ns |  1.36 |    0.05 |    1 |         - |          NA |
| WeakHeapSort     | 256  | PipeOrgan          |     7,163.6 ns |    190.66 ns |     84.65 ns |  2.77 |    0.05 |    3 |         - |          NA |
| SmoothSort       | 256  | PipeOrgan          |     3,915.9 ns |    336.40 ns |    175.94 ns |  1.51 |    0.07 |    1 |         - |          NA |
| TournamentSort   | 256  | PipeOrgan          |     5,734.1 ns |    233.09 ns |    103.49 ns |  2.22 |    0.05 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Random**             |    **14,526.4 ns** |    **292.79 ns** |    **130.00 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Random             |    14,208.3 ns |    276.86 ns |    122.93 ns |  0.98 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | Random             |    15,354.5 ns |    641.61 ns |    335.58 ns |  1.06 |    0.02 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | Random             |    15,776.2 ns |    609.62 ns |    318.84 ns |  1.09 |    0.02 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | Random             |    41,484.4 ns |    492.34 ns |    257.50 ns |  2.86 |    0.03 |    4 |         - |          NA |
| SmoothSort       | 1024 | Random             |    21,851.8 ns |    747.57 ns |    390.99 ns |  1.50 |    0.03 |    2 |         - |          NA |
| TournamentSort   | 1024 | Random             |    32,443.7 ns |  2,355.09 ns |  1,231.76 ns |  2.23 |    0.08 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **SingleElementMoved** |    **12,824.7 ns** |    **310.86 ns** |    **138.02 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | SingleElementMoved |    13,118.3 ns |    356.97 ns |    158.50 ns |  1.02 |    0.02 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | SingleElementMoved |    15,069.7 ns |    759.71 ns |    397.34 ns |  1.18 |    0.03 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | SingleElementMoved |    15,867.9 ns |    629.40 ns |    329.19 ns |  1.24 |    0.03 |    2 |         - |          NA |
| WeakHeapSort     | 1024 | SingleElementMoved |    34,173.1 ns |    185.07 ns |     82.17 ns |  2.66 |    0.03 |    4 |         - |          NA |
| SmoothSort       | 1024 | SingleElementMoved |     5,683.8 ns |    231.16 ns |    120.90 ns |  0.44 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 1024 | SingleElementMoved |    22,801.0 ns |    536.64 ns |    238.27 ns |  1.78 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Sorted**             |    **12,528.9 ns** |    **404.78 ns** |    **211.71 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Sorted             |    11,919.7 ns |    295.20 ns |    154.40 ns |  0.95 |    0.02 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | Sorted             |    16,127.5 ns |  1,042.29 ns |    545.14 ns |  1.29 |    0.05 |    3 |         - |          NA |
| BottomupHeapSort | 1024 | Sorted             |    15,894.4 ns |    843.33 ns |    441.08 ns |  1.27 |    0.04 |    3 |         - |          NA |
| WeakHeapSort     | 1024 | Sorted             |    34,272.3 ns |    277.16 ns |    144.96 ns |  2.74 |    0.05 |    4 |         - |          NA |
| SmoothSort       | 1024 | Sorted             |     3,776.6 ns |    253.45 ns |    132.56 ns |  0.30 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 1024 | Sorted             |    12,727.6 ns |    743.50 ns |    388.87 ns |  1.02 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Reversed**           |    **13,372.1 ns** |    **498.12 ns** |    **260.53 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Reversed           |    13,279.1 ns |    343.75 ns |    152.63 ns |  0.99 |    0.02 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | Reversed           |    14,845.8 ns |    511.61 ns |    267.58 ns |  1.11 |    0.03 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | Reversed           |    16,251.9 ns |    517.46 ns |    270.64 ns |  1.22 |    0.03 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | Reversed           |    37,480.6 ns |    198.62 ns |    103.88 ns |  2.80 |    0.05 |    3 |         - |          NA |
| SmoothSort       | 1024 | Reversed           |    18,564.7 ns |    625.31 ns |    327.05 ns |  1.39 |    0.03 |    1 |         - |          NA |
| TournamentSort   | 1024 | Reversed           |    23,164.9 ns |  1,184.08 ns |    619.30 ns |  1.73 |    0.05 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **PipeOrgan**          |    **13,003.2 ns** |    **356.72 ns** |    **186.57 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | PipeOrgan          |    13,016.4 ns |    214.48 ns |     95.23 ns |  1.00 |    0.02 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | PipeOrgan          |    14,897.7 ns |    723.61 ns |    378.46 ns |  1.15 |    0.03 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | PipeOrgan          |    15,759.8 ns |    614.73 ns |    272.94 ns |  1.21 |    0.03 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | PipeOrgan          |    36,908.2 ns |    152.05 ns |     67.51 ns |  2.84 |    0.04 |    4 |         - |          NA |
| SmoothSort       | 1024 | PipeOrgan          |    20,304.2 ns |  1,134.60 ns |    593.42 ns |  1.56 |    0.05 |    2 |         - |          NA |
| TournamentSort   | 1024 | PipeOrgan          |    28,039.2 ns |    625.94 ns |    277.92 ns |  2.16 |    0.04 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Random**             |   **145,461.2 ns** |  **3,488.11 ns** |  **1,824.35 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Random             |   148,652.1 ns |  1,557.68 ns |    814.70 ns |  1.02 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 4096 | Random             |   108,531.3 ns | 23,869.75 ns | 12,484.34 ns |  0.75 |    0.08 |    1 |         - |          NA |
| BottomupHeapSort | 4096 | Random             |   101,336.6 ns |  8,207.01 ns |  3,643.97 ns |  0.70 |    0.02 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | Random             |   279,887.8 ns | 10,346.67 ns |  4,593.98 ns |  1.92 |    0.04 |    3 |         - |          NA |
| SmoothSort       | 4096 | Random             |   300,376.0 ns |  5,031.57 ns |  2,631.61 ns |  2.07 |    0.03 |    3 |         - |          NA |
| TournamentSort   | 4096 | Random             |   522,342.8 ns |  6,992.05 ns |  3,104.52 ns |  3.59 |    0.05 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **SingleElementMoved** |    **96,472.1 ns** |  **1,702.16 ns** |    **890.26 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 4096 | SingleElementMoved |   123,899.6 ns |  2,072.86 ns |    920.36 ns |  1.28 |    0.01 |    3 |         - |          NA |
| TernaryHeapSort  | 4096 | SingleElementMoved |    75,743.6 ns |  2,829.86 ns |  1,256.48 ns |  0.79 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 4096 | SingleElementMoved |    80,602.5 ns |  1,468.73 ns |    652.12 ns |  0.84 |    0.01 |    2 |         - |          NA |
| WeakHeapSort     | 4096 | SingleElementMoved |   166,439.2 ns |    293.06 ns |    153.28 ns |  1.73 |    0.02 |    4 |         - |          NA |
| SmoothSort       | 4096 | SingleElementMoved |    23,114.3 ns |  1,577.47 ns |    562.54 ns |  0.24 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 4096 | SingleElementMoved |   250,538.8 ns | 18,296.14 ns |  9,569.23 ns |  2.60 |    0.10 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Sorted**             |    **79,464.9 ns** |  **4,642.46 ns** |  **1,655.55 ns** |  **1.00** |    **0.03** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Sorted             |   107,918.8 ns |  2,474.10 ns |  1,294.00 ns |  1.36 |    0.03 |    3 |         - |          NA |
| TernaryHeapSort  | 4096 | Sorted             |    80,282.3 ns |    604.11 ns |    268.23 ns |  1.01 |    0.02 |    2 |         - |          NA |
| BottomupHeapSort | 4096 | Sorted             |    81,689.4 ns |  1,223.03 ns |    543.03 ns |  1.03 |    0.02 |    2 |         - |          NA |
| WeakHeapSort     | 4096 | Sorted             |   166,686.0 ns |    289.96 ns |    151.65 ns |  2.10 |    0.04 |    4 |         - |          NA |
| SmoothSort       | 4096 | Sorted             |    15,153.6 ns |    215.11 ns |     95.51 ns |  0.19 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 4096 | Sorted             |    81,075.5 ns | 22,720.40 ns | 11,883.21 ns |  1.02 |    0.14 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Reversed**           |   **118,732.1 ns** |  **1,919.19 ns** |  **1,003.77 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Reversed           |   111,461.6 ns |  1,626.13 ns |    722.01 ns |  0.94 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 4096 | Reversed           |    73,612.2 ns |  1,803.41 ns |    943.22 ns |  0.62 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 4096 | Reversed           |    81,181.9 ns |  1,806.34 ns |    802.02 ns |  0.68 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | Reversed           |   180,353.7 ns |    347.01 ns |    181.49 ns |  1.52 |    0.01 |    2 |         - |          NA |
| SmoothSort       | 4096 | Reversed           |    96,798.8 ns |  4,383.92 ns |  2,292.87 ns |  0.82 |    0.02 |    1 |         - |          NA |
| TournamentSort   | 4096 | Reversed           |   154,660.6 ns |  3,095.11 ns |  1,618.80 ns |  1.30 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **PipeOrgan**          |   **101,501.7 ns** |  **1,677.02 ns** |    **877.11 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 4096 | PipeOrgan          |   115,629.6 ns |  1,198.01 ns |    626.58 ns |  1.14 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 4096 | PipeOrgan          |    71,773.9 ns |  2,270.68 ns |  1,187.61 ns |  0.71 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 4096 | PipeOrgan          |    80,740.6 ns |  1,550.64 ns |    811.01 ns |  0.80 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | PipeOrgan          |   180,978.8 ns |    837.47 ns |    438.01 ns |  1.78 |    0.02 |    3 |         - |          NA |
| SmoothSort       | 4096 | PipeOrgan          |   197,355.2 ns |  4,125.58 ns |  2,157.76 ns |  1.94 |    0.03 |    3 |         - |          NA |
| TournamentSort   | 4096 | PipeOrgan          |   370,101.2 ns |  6,028.22 ns |  3,152.87 ns |  3.65 |    0.04 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Random**             |   **413,838.4 ns** |  **2,948.32 ns** |  **1,542.03 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Random             |   405,081.5 ns |  3,247.77 ns |  1,698.65 ns |  0.98 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | Random             |   509,211.8 ns |  4,619.89 ns |  2,416.29 ns |  1.23 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | Random             |   512,580.4 ns |  2,203.22 ns |  1,152.33 ns |  1.24 |    0.01 |    2 |         - |          NA |
| WeakHeapSort     | 8192 | Random             |   761,294.2 ns |  1,215.91 ns |    539.87 ns |  1.84 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 8192 | Random             |   737,258.9 ns |  4,029.16 ns |  1,788.97 ns |  1.78 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 8192 | Random             | 1,166,695.2 ns |  7,893.43 ns |  3,504.73 ns |  2.82 |    0.01 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **SingleElementMoved** |   **254,514.1 ns** |  **2,264.46 ns** |  **1,184.36 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 8192 | SingleElementMoved |   307,615.9 ns |  3,536.58 ns |  1,849.70 ns |  1.21 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 8192 | SingleElementMoved |   275,802.5 ns |  8,814.15 ns |  3,913.54 ns |  1.08 |    0.02 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | SingleElementMoved |   321,756.5 ns |  2,038.02 ns |  1,065.92 ns |  1.26 |    0.01 |    2 |         - |          NA |
| WeakHeapSort     | 8192 | SingleElementMoved |   364,056.9 ns |  3,354.36 ns |  1,754.39 ns |  1.43 |    0.01 |    2 |         - |          NA |
| SmoothSort       | 8192 | SingleElementMoved |    45,807.8 ns |    475.06 ns |    210.93 ns |  0.18 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 8192 | SingleElementMoved |   625,314.2 ns |  2,581.07 ns |  1,349.95 ns |  2.46 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Sorted**             |   **245,844.4 ns** |  **3,917.00 ns** |  **1,739.17 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Sorted             |   306,943.1 ns |  3,131.39 ns |  1,637.78 ns |  1.25 |    0.01 |    3 |         - |          NA |
| TernaryHeapSort  | 8192 | Sorted             |   321,115.8 ns |  3,273.26 ns |  1,711.98 ns |  1.31 |    0.01 |    3 |         - |          NA |
| BottomupHeapSort | 8192 | Sorted             |   316,404.4 ns |  1,565.07 ns |    818.56 ns |  1.29 |    0.01 |    3 |         - |          NA |
| WeakHeapSort     | 8192 | Sorted             |   365,667.0 ns |  1,078.33 ns |    563.99 ns |  1.49 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 8192 | Sorted             |    30,184.7 ns |  1,227.92 ns |    545.21 ns |  0.12 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 8192 | Sorted             |   387,866.7 ns | 44,681.47 ns | 23,369.27 ns |  1.58 |    0.09 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Reversed**           |   **317,979.7 ns** |  **2,744.37 ns** |  **1,435.36 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Reversed           |   280,262.2 ns |  3,089.48 ns |  1,615.86 ns |  0.88 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | Reversed           |   309,879.0 ns |  1,494.30 ns |    781.55 ns |  0.97 |    0.00 |    1 |         - |          NA |
| BottomupHeapSort | 8192 | Reversed           |   370,641.3 ns |  2,846.44 ns |  1,488.75 ns |  1.17 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | Reversed           |   393,443.5 ns |  2,831.49 ns |  1,480.92 ns |  1.24 |    0.01 |    1 |         - |          NA |
| SmoothSort       | 8192 | Reversed           |   437,925.1 ns |  5,286.40 ns |  2,764.89 ns |  1.38 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 8192 | Reversed           |   556,003.7 ns | 14,225.19 ns |  5,072.84 ns |  1.75 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **PipeOrgan**          |   **302,830.4 ns** |  **1,393.63 ns** |    **618.78 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | PipeOrgan          |   315,088.4 ns |  1,099.70 ns |    575.16 ns |  1.04 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | PipeOrgan          |   349,898.6 ns |  1,313.95 ns |    583.40 ns |  1.16 |    0.00 |    1 |         - |          NA |
| BottomupHeapSort | 8192 | PipeOrgan          |   369,494.9 ns |  1,729.52 ns |    904.57 ns |  1.22 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | PipeOrgan          |   395,353.2 ns |  1,036.28 ns |    460.12 ns |  1.31 |    0.00 |    1 |         - |          NA |
| SmoothSort       | 8192 | PipeOrgan          |   541,507.9 ns |  1,515.29 ns |    672.80 ns |  1.79 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 8192 | PipeOrgan          |   884,026.3 ns |  3,148.26 ns |  1,397.85 ns |  2.92 |    0.01 |    3 |         - |          NA |

### InsertionBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                 | Size | Pattern            | Mean         | Error       | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------- |----- |------------------- |-------------:|------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **InsertionSort**          | **256**  | **Random**             |   **5,481.0 ns** |   **307.98 ns** |   **161.08 ns** |  **1.00** |    **0.04** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Random             |   5,731.5 ns |    49.78 ns |    17.75 ns |  1.05 |    0.03 |    3 |         - |          NA |
| BinaryInsertSort       | 256  | Random             |   4,449.2 ns |   230.77 ns |   120.70 ns |  0.81 |    0.03 |    2 |         - |          NA |
| GnomeSort              | 256  | Random             |  18,884.0 ns |   328.97 ns |   172.06 ns |  3.45 |    0.10 |    5 |         - |          NA |
| LibrarySort            | 256  | Random             |  12,560.2 ns |   284.45 ns |   148.77 ns |  2.29 |    0.07 |    4 |         - |          NA |
| MergeInsertionSort     | 256  | Random             |  11,415.9 ns |   217.28 ns |   113.64 ns |  2.08 |    0.06 |    4 |         - |          NA |
| ShellSortKnuth1973     | 256  | Random             |   2,044.2 ns |    12.61 ns |     5.60 ns |  0.37 |    0.01 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Random             |   2,167.3 ns |   110.06 ns |    48.87 ns |  0.40 |    0.01 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | Random             |   2,220.1 ns |    34.02 ns |    17.79 ns |  0.41 |    0.01 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | Random             |   2,016.6 ns |    22.97 ns |     8.19 ns |  0.37 |    0.01 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Random             |   2,127.8 ns |   266.72 ns |   139.50 ns |  0.39 |    0.03 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **InsertionSort**          | **256**  | **SingleElementMoved** |     **366.1 ns** |     **1.98 ns** |     **1.03 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 256  | SingleElementMoved |     255.0 ns |    12.43 ns |     6.50 ns |  0.70 |    0.02 |    1 |         - |          NA |
| BinaryInsertSort       | 256  | SingleElementMoved |     888.5 ns |    38.81 ns |    17.23 ns |  2.43 |    0.04 |    3 |         - |          NA |
| GnomeSort              | 256  | SingleElementMoved |     378.5 ns |   114.57 ns |    59.92 ns |  1.03 |    0.15 |    2 |         - |          NA |
| LibrarySort            | 256  | SingleElementMoved |  12,002.8 ns |   291.46 ns |   129.41 ns | 32.79 |    0.34 |    4 |         - |          NA |
| MergeInsertionSort     | 256  | SingleElementMoved |  10,006.1 ns |   354.50 ns |   185.41 ns | 27.33 |    0.48 |    4 |         - |          NA |
| ShellSortKnuth1973     | 256  | SingleElementMoved |     953.5 ns |     9.36 ns |     3.34 ns |  2.60 |    0.01 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 256  | SingleElementMoved |     961.2 ns |     6.91 ns |     2.46 ns |  2.63 |    0.01 |    3 |         - |          NA |
| ShellSortTokuda1992    | 256  | SingleElementMoved |   1,205.4 ns |   100.32 ns |    44.54 ns |  3.29 |    0.11 |    3 |         - |          NA |
| ShellSortCiura2001     | 256  | SingleElementMoved |   1,099.6 ns |   151.76 ns |    67.38 ns |  3.00 |    0.17 |    3 |         - |          NA |
| ShellSortLee2021       | 256  | SingleElementMoved |   1,182.3 ns |    34.09 ns |    15.14 ns |  3.23 |    0.04 |    3 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **InsertionSort**          | **256**  | **Sorted**             |     **283.6 ns** |     **1.68 ns** |     **0.88 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Sorted             |     182.7 ns |     1.76 ns |     0.78 ns |  0.64 |    0.00 |    1 |         - |          NA |
| BinaryInsertSort       | 256  | Sorted             |     333.8 ns |   108.15 ns |    48.02 ns |  1.18 |    0.16 |    3 |         - |          NA |
| GnomeSort              | 256  | Sorted             |     216.3 ns |     1.00 ns |     0.44 ns |  0.76 |    0.00 |    1 |         - |          NA |
| LibrarySort            | 256  | Sorted             |  12,201.4 ns |   144.13 ns |    75.38 ns | 43.02 |    0.28 |    6 |         - |          NA |
| MergeInsertionSort     | 256  | Sorted             |   9,839.7 ns |   218.10 ns |   114.07 ns | 34.69 |    0.39 |    5 |         - |          NA |
| ShellSortKnuth1973     | 256  | Sorted             |     969.7 ns |    53.51 ns |    27.99 ns |  3.42 |    0.09 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Sorted             |     928.2 ns |     7.16 ns |     2.55 ns |  3.27 |    0.01 |    4 |         - |          NA |
| ShellSortTokuda1992    | 256  | Sorted             |   1,537.8 ns |   915.63 ns |   406.54 ns |  5.42 |    1.34 |    4 |         - |          NA |
| ShellSortCiura2001     | 256  | Sorted             |   1,033.1 ns |    89.68 ns |    39.82 ns |  3.64 |    0.13 |    4 |         - |          NA |
| ShellSortLee2021       | 256  | Sorted             |   1,278.0 ns |   267.81 ns |   140.07 ns |  4.51 |    0.47 |    4 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **InsertionSort**          | **256**  | **Reversed**           |  **11,910.0 ns** |   **165.42 ns** |    **73.45 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Reversed           |  13,852.0 ns |   185.40 ns |    82.32 ns |  1.16 |    0.01 |    3 |         - |          NA |
| BinaryInsertSort       | 256  | Reversed           |   5,309.5 ns |   166.50 ns |    87.08 ns |  0.45 |    0.01 |    2 |         - |          NA |
| GnomeSort              | 256  | Reversed           |  41,659.1 ns |   661.66 ns |   293.78 ns |  3.50 |    0.03 |    5 |         - |          NA |
| LibrarySort            | 256  | Reversed           |  17,148.7 ns |    84.73 ns |    30.22 ns |  1.44 |    0.01 |    4 |         - |          NA |
| MergeInsertionSort     | 256  | Reversed           |  10,042.5 ns |    71.87 ns |    37.59 ns |  0.84 |    0.01 |    3 |         - |          NA |
| ShellSortKnuth1973     | 256  | Reversed           |   1,356.0 ns |   140.68 ns |    62.46 ns |  0.11 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Reversed           |   1,266.6 ns |    18.84 ns |     8.36 ns |  0.11 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | Reversed           |   1,400.3 ns |    16.33 ns |     8.54 ns |  0.12 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | Reversed           |   1,336.1 ns |    15.05 ns |     6.68 ns |  0.11 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Reversed           |   1,367.6 ns |     9.12 ns |     4.77 ns |  0.11 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **InsertionSort**          | **256**  | **PipeOrgan**          |   **6,325.2 ns** |   **559.48 ns** |   **248.41 ns** |  **1.00** |    **0.05** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 256  | PipeOrgan          |   6,969.9 ns |   327.08 ns |   171.07 ns |  1.10 |    0.05 |    3 |         - |          NA |
| BinaryInsertSort       | 256  | PipeOrgan          |   2,980.6 ns |    75.09 ns |    33.34 ns |  0.47 |    0.02 |    2 |         - |          NA |
| GnomeSort              | 256  | PipeOrgan          |  20,268.5 ns |   214.87 ns |   112.38 ns |  3.21 |    0.11 |    5 |         - |          NA |
| LibrarySort            | 256  | PipeOrgan          |  11,269.2 ns |   201.76 ns |   105.53 ns |  1.78 |    0.06 |    4 |         - |          NA |
| MergeInsertionSort     | 256  | PipeOrgan          |  10,099.5 ns |   195.53 ns |    86.82 ns |  1.60 |    0.06 |    4 |         - |          NA |
| ShellSortKnuth1973     | 256  | PipeOrgan          |   1,213.6 ns |   131.27 ns |    58.29 ns |  0.19 |    0.01 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | PipeOrgan          |   1,437.5 ns |   285.50 ns |   149.32 ns |  0.23 |    0.02 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | PipeOrgan          |   1,580.3 ns |   545.79 ns |   285.46 ns |  0.25 |    0.04 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | PipeOrgan          |   1,318.2 ns |    39.75 ns |    17.65 ns |  0.21 |    0.01 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | PipeOrgan          |   1,368.4 ns |    21.81 ns |     7.78 ns |  0.22 |    0.01 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **InsertionSort**          | **1024** | **Random**             |  **90,904.4 ns** |   **856.06 ns** |   **305.28 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Random             | 104,609.1 ns |   684.27 ns |   357.89 ns |  1.15 |    0.01 |    5 |         - |          NA |
| BinaryInsertSort       | 1024 | Random             |  29,550.6 ns |   788.49 ns |   412.40 ns |  0.33 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | Random             | 300,983.0 ns | 2,382.74 ns | 1,246.22 ns |  3.31 |    0.02 |    6 |         - |          NA |
| LibrarySort            | 1024 | Random             |  55,986.4 ns |   427.74 ns |   223.72 ns |  0.62 |    0.00 |    3 |         - |          NA |
| MergeInsertionSort     | 1024 | Random             |  75,569.6 ns | 1,830.85 ns |   957.57 ns |  0.83 |    0.01 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Random             |  11,468.3 ns |   289.20 ns |   151.26 ns |  0.13 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Random             |  11,229.4 ns |   384.94 ns |   170.91 ns |  0.12 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Random             |  11,210.4 ns |   226.32 ns |   100.49 ns |  0.12 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Random             |  11,114.1 ns |   267.74 ns |   140.04 ns |  0.12 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Random             |  11,163.2 ns |   156.83 ns |    82.02 ns |  0.12 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **InsertionSort**          | **1024** | **SingleElementMoved** |   **1,441.6 ns** |     **4.43 ns** |     **1.58 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | SingleElementMoved |     981.0 ns |     3.91 ns |     1.74 ns |  0.68 |    0.00 |    1 |         - |          NA |
| BinaryInsertSort       | 1024 | SingleElementMoved |   4,541.0 ns |   235.79 ns |   123.32 ns |  3.15 |    0.08 |    3 |         - |          NA |
| GnomeSort              | 1024 | SingleElementMoved |   1,211.6 ns |     7.38 ns |     3.86 ns |  0.84 |    0.00 |    2 |         - |          NA |
| LibrarySort            | 1024 | SingleElementMoved |  59,916.8 ns |   459.82 ns |   240.50 ns | 41.56 |    0.16 |    4 |         - |          NA |
| MergeInsertionSort     | 1024 | SingleElementMoved |  60,868.8 ns |   336.11 ns |   149.24 ns | 42.22 |    0.11 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | SingleElementMoved |   5,043.6 ns |   338.41 ns |   176.99 ns |  3.50 |    0.12 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | SingleElementMoved |   5,550.4 ns |   262.72 ns |   137.41 ns |  3.85 |    0.09 |    3 |         - |          NA |
| ShellSortTokuda1992    | 1024 | SingleElementMoved |   6,059.3 ns |   333.08 ns |   174.21 ns |  4.20 |    0.11 |    3 |         - |          NA |
| ShellSortCiura2001     | 1024 | SingleElementMoved |   5,836.9 ns |   287.31 ns |   150.27 ns |  4.05 |    0.10 |    3 |         - |          NA |
| ShellSortLee2021       | 1024 | SingleElementMoved |   5,997.2 ns |   367.34 ns |   192.13 ns |  4.16 |    0.13 |    3 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **InsertionSort**          | **1024** | **Sorted**             |   **1,115.8 ns** |     **0.56 ns** |     **0.25 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Sorted             |     732.0 ns |    91.27 ns |    40.52 ns |  0.66 |    0.03 |    1 |         - |          NA |
| BinaryInsertSort       | 1024 | Sorted             |     845.6 ns |     0.99 ns |     0.44 ns |  0.76 |    0.00 |    1 |         - |          NA |
| GnomeSort              | 1024 | Sorted             |     839.4 ns |     3.28 ns |     1.46 ns |  0.75 |    0.00 |    1 |         - |          NA |
| LibrarySort            | 1024 | Sorted             |  61,297.3 ns |   359.41 ns |   187.98 ns | 54.93 |    0.16 |    4 |         - |          NA |
| MergeInsertionSort     | 1024 | Sorted             |  61,383.7 ns |   267.84 ns |   140.09 ns | 55.01 |    0.12 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Sorted             |   4,711.2 ns |   315.71 ns |   165.12 ns |  4.22 |    0.14 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Sorted             |   5,183.0 ns |   132.81 ns |    58.97 ns |  4.64 |    0.05 |    3 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Sorted             |   5,754.3 ns |   401.56 ns |   210.02 ns |  5.16 |    0.18 |    3 |         - |          NA |
| ShellSortCiura2001     | 1024 | Sorted             |   5,549.6 ns |   230.08 ns |   102.16 ns |  4.97 |    0.09 |    3 |         - |          NA |
| ShellSortLee2021       | 1024 | Sorted             |   5,735.9 ns |   285.07 ns |   149.09 ns |  5.14 |    0.13 |    3 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **InsertionSort**          | **1024** | **Reversed**           | **177,804.4 ns** |   **954.59 ns** |   **423.85 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Reversed           | 208,627.2 ns |   482.23 ns |   252.22 ns |  1.17 |    0.00 |    4 |         - |          NA |
| BinaryInsertSort       | 1024 | Reversed           |  37,787.1 ns |   316.23 ns |   165.39 ns |  0.21 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | Reversed           | 599,318.7 ns | 5,061.93 ns | 2,247.53 ns |  3.37 |    0.01 |    5 |         - |          NA |
| LibrarySort            | 1024 | Reversed           | 150,058.4 ns |   855.45 ns |   379.82 ns |  0.84 |    0.00 |    4 |         - |          NA |
| MergeInsertionSort     | 1024 | Reversed           |  61,501.0 ns |   754.39 ns |   394.56 ns |  0.35 |    0.00 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Reversed           |   6,581.7 ns |   668.54 ns |   349.66 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Reversed           |   6,941.7 ns |   834.70 ns |   436.57 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Reversed           |   7,279.6 ns |   580.77 ns |   257.86 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Reversed           |   7,362.1 ns |   663.19 ns |   346.86 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Reversed           |   7,284.9 ns |   645.96 ns |   337.85 ns |  0.04 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **InsertionSort**          | **1024** | **PipeOrgan**          |  **95,455.4 ns** | **9,548.85 ns** | **4,994.23 ns** |  **1.00** |    **0.07** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | PipeOrgan          | 107,622.2 ns | 4,091.64 ns | 2,140.01 ns |  1.13 |    0.06 |    4 |         - |          NA |
| BinaryInsertSort       | 1024 | PipeOrgan          |  20,556.2 ns |   435.62 ns |   227.84 ns |  0.22 |    0.01 |    2 |         - |          NA |
| GnomeSort              | 1024 | PipeOrgan          | 297,472.3 ns | 2,374.67 ns | 1,242.00 ns |  3.12 |    0.15 |    5 |         - |          NA |
| LibrarySort            | 1024 | PipeOrgan          |  55,931.0 ns | 1,048.11 ns |   548.18 ns |  0.59 |    0.03 |    3 |         - |          NA |
| MergeInsertionSort     | 1024 | PipeOrgan          |  62,058.0 ns |   490.30 ns |   256.44 ns |  0.65 |    0.03 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | PipeOrgan          |   6,196.6 ns |   526.61 ns |   275.43 ns |  0.07 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | PipeOrgan          |   6,547.0 ns |   336.98 ns |   176.25 ns |  0.07 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | PipeOrgan          |   7,306.9 ns |   591.98 ns |   309.62 ns |  0.08 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | PipeOrgan          |   7,207.1 ns |   495.17 ns |   258.98 ns |  0.08 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | PipeOrgan          |   7,233.0 ns |   631.43 ns |   280.36 ns |  0.08 |    0.00 |    1 |         - |          NA |

### MergeBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                   | Size | Pattern            | Mean           | Error        | StdDev       | Median         | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------------- |----- |------------------- |---------------:|-------------:|-------------:|---------------:|------:|--------:|-----:|----------:|------------:|
| **MergeSort**                | **256**  | **Random**             |     **6,661.8 ns** |    **243.00 ns** |    **127.09 ns** |     **6,611.7 ns** |  **1.00** |    **0.03** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Random             |     6,461.9 ns |    247.39 ns |    109.84 ns |     6,419.5 ns |  0.97 |    0.02 |    3 |         - |          NA |
| BottomupMergeSort        | 256  | Random             |     3,797.6 ns |    312.17 ns |    138.60 ns |     3,714.2 ns |  0.57 |    0.02 |    2 |         - |          NA |
| StdStableSort            | 256  | Random             |     2,163.7 ns |    289.90 ns |    151.62 ns |     2,075.2 ns |  0.32 |    0.02 |    1 |         - |          NA |
| RotateMergeSort          | 256  | Random             |     7,862.6 ns |    275.65 ns |    122.39 ns |     7,837.7 ns |  1.18 |    0.03 |    3 |         - |          NA |
| RotateMergeSortRecursive | 256  | Random             |    10,072.9 ns |  1,017.93 ns |    451.97 ns |     9,887.6 ns |  1.51 |    0.07 |    4 |         - |          NA |
| SymMergeSort             | 256  | Random             |     5,435.6 ns |    290.95 ns |    152.17 ns |     5,373.0 ns |  0.82 |    0.03 |    3 |         - |          NA |
| BlockMergeSort           | 256  | Random             |     3,896.9 ns |     93.31 ns |     41.43 ns |     3,907.6 ns |  0.59 |    0.01 |    2 |         - |          NA |
| NaturalMergeSort         | 256  | Random             |     4,083.1 ns |    390.79 ns |    173.51 ns |     3,972.2 ns |  0.61 |    0.03 |    2 |         - |          NA |
| TimSort                  | 256  | Random             |     3,189.1 ns |    136.65 ns |     71.47 ns |     3,163.0 ns |  0.48 |    0.01 |    2 |         - |          NA |
| PowerSort                | 256  | Random             |     1,807.2 ns |     17.50 ns |      7.77 ns |     1,803.2 ns |  0.27 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Random             |     3,077.1 ns |    166.52 ns |     73.94 ns |     3,043.7 ns |  0.46 |    0.01 |    2 |         - |          NA |
| SpinSort                 | 256  | Random             |     1,864.8 ns |    119.41 ns |     53.02 ns |     1,832.1 ns |  0.28 |    0.01 |    1 |         - |          NA |
| Glidesort                | 256  | Random             |     2,333.6 ns |     43.62 ns |     19.37 ns |     2,329.8 ns |  0.35 |    0.01 |    1 |         - |          NA |
| Driftsort                | 256  | Random             |     2,960.6 ns |     46.62 ns |     20.70 ns |     2,969.7 ns |  0.44 |    0.01 |    2 |         - |          NA |
| FlatStableSort           | 256  | Random             |     2,047.9 ns |     52.29 ns |     23.22 ns |     2,052.2 ns |  0.31 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **256**  | **SingleElementMoved** |     **3,636.8 ns** |     **72.66 ns** |     **32.26 ns** |     **3,660.3 ns** |  **1.00** |    **0.01** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | SingleElementMoved |     4,421.0 ns |    257.80 ns |    134.83 ns |     4,334.5 ns |  1.22 |    0.04 |    6 |         - |          NA |
| BottomupMergeSort        | 256  | SingleElementMoved |     1,261.1 ns |      5.83 ns |      2.59 ns |     1,261.8 ns |  0.35 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 256  | SingleElementMoved |       786.6 ns |    273.50 ns |    143.05 ns |       734.8 ns |  0.22 |    0.04 |    2 |         - |          NA |
| RotateMergeSort          | 256  | SingleElementMoved |       407.4 ns |      3.86 ns |      1.72 ns |       407.0 ns |  0.11 |    0.00 |    1 |         - |          NA |
| RotateMergeSortRecursive | 256  | SingleElementMoved |       596.5 ns |     97.69 ns |     51.09 ns |       592.3 ns |  0.16 |    0.01 |    1 |         - |          NA |
| SymMergeSort             | 256  | SingleElementMoved |       513.1 ns |    155.74 ns |     69.15 ns |       479.9 ns |  0.14 |    0.02 |    1 |         - |          NA |
| BlockMergeSort           | 256  | SingleElementMoved |     2,488.8 ns |    272.30 ns |    120.90 ns |     2,472.0 ns |  0.68 |    0.03 |    5 |         - |          NA |
| NaturalMergeSort         | 256  | SingleElementMoved |       486.1 ns |      8.26 ns |      2.95 ns |       485.1 ns |  0.13 |    0.00 |    1 |         - |          NA |
| TimSort                  | 256  | SingleElementMoved |       299.3 ns |    116.11 ns |     60.73 ns |       310.5 ns |  0.08 |    0.02 |    1 |         - |          NA |
| PowerSort                | 256  | SingleElementMoved |       327.4 ns |      2.87 ns |      1.27 ns |       326.9 ns |  0.09 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | SingleElementMoved |       402.6 ns |    179.93 ns |     94.10 ns |       386.5 ns |  0.11 |    0.02 |    1 |         - |          NA |
| SpinSort                 | 256  | SingleElementMoved |       884.2 ns |     33.39 ns |     17.46 ns |       878.4 ns |  0.24 |    0.00 |    3 |         - |          NA |
| Glidesort                | 256  | SingleElementMoved |       947.1 ns |     13.05 ns |      6.83 ns |       944.6 ns |  0.26 |    0.00 |    3 |         - |          NA |
| Driftsort                | 256  | SingleElementMoved |       916.3 ns |     19.88 ns |      7.09 ns |       917.0 ns |  0.25 |    0.00 |    3 |         - |          NA |
| FlatStableSort           | 256  | SingleElementMoved |     1,006.4 ns |     19.79 ns |      8.79 ns |     1,005.4 ns |  0.28 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **256**  | **Sorted**             |     **3,343.0 ns** |    **143.09 ns** |     **74.84 ns** |     **3,298.5 ns** |  **1.00** |    **0.03** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Sorted             |     4,048.9 ns |     10.68 ns |      3.81 ns |     4,048.6 ns |  1.21 |    0.03 |    7 |         - |          NA |
| BottomupMergeSort        | 256  | Sorted             |       962.6 ns |      6.07 ns |      2.70 ns |       962.2 ns |  0.29 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 256  | Sorted             |       472.4 ns |      3.91 ns |      2.04 ns |       471.5 ns |  0.14 |    0.00 |    3 |         - |          NA |
| RotateMergeSort          | 256  | Sorted             |       288.1 ns |      1.19 ns |      0.62 ns |       288.0 ns |  0.09 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 256  | Sorted             |       287.4 ns |      1.76 ns |      0.78 ns |       287.2 ns |  0.09 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 256  | Sorted             |       286.6 ns |      1.79 ns |      0.94 ns |       286.3 ns |  0.09 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 256  | Sorted             |     2,066.0 ns |     78.12 ns |     34.69 ns |     2,073.4 ns |  0.62 |    0.02 |    5 |         - |          NA |
| NaturalMergeSort         | 256  | Sorted             |       178.3 ns |     20.80 ns |     10.88 ns |       176.8 ns |  0.05 |    0.00 |    1 |         - |          NA |
| TimSort                  | 256  | Sorted             |       135.0 ns |      1.16 ns |      0.61 ns |       135.0 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | Sorted             |       122.2 ns |      2.16 ns |      1.13 ns |       121.9 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Sorted             |       147.1 ns |      4.21 ns |      2.20 ns |       146.3 ns |  0.04 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 256  | Sorted             |       111.8 ns |     10.84 ns |      4.81 ns |       114.9 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 256  | Sorted             |       186.6 ns |     67.59 ns |     35.35 ns |       171.2 ns |  0.06 |    0.01 |    1 |         - |          NA |
| Driftsort                | 256  | Sorted             |       168.6 ns |      3.30 ns |      1.47 ns |       168.0 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | Sorted             |       942.6 ns |     10.34 ns |      5.41 ns |       943.4 ns |  0.28 |    0.01 |    4 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **256**  | **Reversed**           |     **6,944.2 ns** |    **176.46 ns** |     **78.35 ns** |     **6,909.3 ns** |  **1.00** |    **0.01** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Reversed           |     6,418.8 ns |    388.30 ns |    203.09 ns |     6,335.1 ns |  0.92 |    0.03 |    8 |         - |          NA |
| BottomupMergeSort        | 256  | Reversed           |     3,821.4 ns |     36.12 ns |     16.04 ns |     3,816.2 ns |  0.55 |    0.01 |    7 |         - |          NA |
| StdStableSort            | 256  | Reversed           |     3,164.5 ns |     13.52 ns |      4.82 ns |     3,163.1 ns |  0.46 |    0.00 |    6 |         - |          NA |
| RotateMergeSort          | 256  | Reversed           |     1,729.8 ns |      2.87 ns |      1.27 ns |     1,730.0 ns |  0.25 |    0.00 |    4 |         - |          NA |
| RotateMergeSortRecursive | 256  | Reversed           |     1,577.0 ns |     48.63 ns |     17.34 ns |     1,570.9 ns |  0.23 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 256  | Reversed           |     1,638.8 ns |     11.72 ns |      5.20 ns |     1,637.3 ns |  0.24 |    0.00 |    4 |         - |          NA |
| BlockMergeSort           | 256  | Reversed           |     2,532.1 ns |    118.14 ns |     61.79 ns |     2,525.0 ns |  0.36 |    0.01 |    5 |         - |          NA |
| NaturalMergeSort         | 256  | Reversed           |       357.9 ns |    132.26 ns |     58.72 ns |       380.1 ns |  0.05 |    0.01 |    3 |         - |          NA |
| TimSort                  | 256  | Reversed           |       184.9 ns |      2.21 ns |      1.15 ns |       184.5 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | Reversed           |       254.4 ns |    104.84 ns |     54.83 ns |       277.4 ns |  0.04 |    0.01 |    2 |         - |          NA |
| ShiftSort                | 256  | Reversed           |       200.2 ns |      3.31 ns |      1.47 ns |       199.9 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 256  | Reversed           |       288.2 ns |     10.20 ns |      4.53 ns |       287.8 ns |  0.04 |    0.00 |    2 |         - |          NA |
| Glidesort                | 256  | Reversed           |       220.2 ns |      4.74 ns |      2.48 ns |       219.6 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 256  | Reversed           |       226.7 ns |      2.34 ns |      1.04 ns |       227.0 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | Reversed           |     2,248.8 ns |     10.42 ns |      4.63 ns |     2,247.3 ns |  0.32 |    0.00 |    5 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **256**  | **PipeOrgan**          |     **5,430.6 ns** |    **293.27 ns** |    **153.39 ns** |     **5,441.5 ns** |  **1.00** |    **0.04** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | PipeOrgan          |     5,601.5 ns |    443.54 ns |    231.98 ns |     5,598.8 ns |  1.03 |    0.05 |    8 |         - |          NA |
| BottomupMergeSort        | 256  | PipeOrgan          |     2,517.7 ns |     13.33 ns |      5.92 ns |     2,515.9 ns |  0.46 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 256  | PipeOrgan          |     1,987.3 ns |     95.89 ns |     50.15 ns |     1,961.5 ns |  0.37 |    0.01 |    4 |         - |          NA |
| RotateMergeSort          | 256  | PipeOrgan          |     3,289.6 ns |     34.31 ns |     15.23 ns |     3,284.6 ns |  0.61 |    0.02 |    6 |         - |          NA |
| RotateMergeSortRecursive | 256  | PipeOrgan          |     4,165.8 ns |    359.35 ns |    187.94 ns |     4,058.1 ns |  0.77 |    0.04 |    7 |         - |          NA |
| SymMergeSort             | 256  | PipeOrgan          |     2,015.7 ns |     28.32 ns |     12.57 ns |     2,012.2 ns |  0.37 |    0.01 |    4 |         - |          NA |
| BlockMergeSort           | 256  | PipeOrgan          |     2,460.3 ns |     66.24 ns |     29.41 ns |     2,446.4 ns |  0.45 |    0.01 |    5 |         - |          NA |
| NaturalMergeSort         | 256  | PipeOrgan          |       729.4 ns |    382.65 ns |    200.14 ns |       672.0 ns |  0.13 |    0.03 |    2 |         - |          NA |
| TimSort                  | 256  | PipeOrgan          |       617.5 ns |     78.61 ns |     41.11 ns |       604.5 ns |  0.11 |    0.01 |    2 |         - |          NA |
| PowerSort                | 256  | PipeOrgan          |       458.7 ns |    199.62 ns |     88.63 ns |       441.5 ns |  0.08 |    0.02 |    1 |         - |          NA |
| ShiftSort                | 256  | PipeOrgan          |       399.9 ns |      2.71 ns |      1.20 ns |       400.0 ns |  0.07 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 256  | PipeOrgan          |     1,652.0 ns |    237.37 ns |    124.15 ns |     1,640.8 ns |  0.30 |    0.02 |    4 |         - |          NA |
| Glidesort                | 256  | PipeOrgan          |       984.6 ns |      7.81 ns |      4.09 ns |       984.8 ns |  0.18 |    0.00 |    3 |         - |          NA |
| Driftsort                | 256  | PipeOrgan          |       354.8 ns |      3.29 ns |      1.46 ns |       354.4 ns |  0.07 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | PipeOrgan          |     1,652.4 ns |     20.73 ns |      9.20 ns |     1,649.0 ns |  0.30 |    0.01 |    4 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **1024** | **Random**             |    **28,358.4 ns** |    **430.73 ns** |    **225.28 ns** |    **28,303.0 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Random             |    27,681.4 ns |    471.68 ns |    246.70 ns |    27,696.7 ns |  0.98 |    0.01 |    3 |         - |          NA |
| BottomupMergeSort        | 1024 | Random             |    16,684.1 ns |    299.31 ns |    156.54 ns |    16,616.9 ns |  0.59 |    0.01 |    1 |         - |          NA |
| StdStableSort            | 1024 | Random             |    10,988.1 ns |    340.44 ns |    178.05 ns |    10,977.0 ns |  0.39 |    0.01 |    1 |         - |          NA |
| RotateMergeSort          | 1024 | Random             |    48,768.9 ns |  1,791.55 ns |    937.01 ns |    49,054.3 ns |  1.72 |    0.03 |    4 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Random             |    57,979.5 ns |    653.03 ns |    289.95 ns |    57,898.3 ns |  2.04 |    0.02 |    4 |         - |          NA |
| SymMergeSort             | 1024 | Random             |    33,135.3 ns |    653.06 ns |    289.96 ns |    32,982.9 ns |  1.17 |    0.01 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | Random             |    20,015.8 ns |    313.46 ns |    139.18 ns |    20,011.7 ns |  0.71 |    0.01 |    2 |         - |          NA |
| NaturalMergeSort         | 1024 | Random             |    23,009.2 ns | 10,560.81 ns |  5,523.51 ns |    20,150.4 ns |  0.81 |    0.18 |    2 |         - |          NA |
| TimSort                  | 1024 | Random             |    15,124.7 ns |    661.54 ns |    293.73 ns |    15,038.4 ns |  0.53 |    0.01 |    1 |         - |          NA |
| PowerSort                | 1024 | Random             |     9,588.4 ns |    118.66 ns |     52.68 ns |     9,578.2 ns |  0.34 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Random             |    14,405.2 ns |    197.35 ns |     87.63 ns |    14,368.0 ns |  0.51 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 1024 | Random             |     9,503.3 ns |    441.43 ns |    196.00 ns |     9,545.2 ns |  0.34 |    0.01 |    1 |         - |          NA |
| Glidesort                | 1024 | Random             |     9,677.3 ns |    344.27 ns |    152.86 ns |     9,699.0 ns |  0.34 |    0.01 |    1 |         - |          NA |
| Driftsort                | 1024 | Random             |    12,800.1 ns |    328.04 ns |    171.57 ns |    12,796.9 ns |  0.45 |    0.01 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Random             |    10,830.0 ns |    270.07 ns |    119.91 ns |    10,862.1 ns |  0.38 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **1024** | **SingleElementMoved** |    **14,490.5 ns** |    **228.27 ns** |    **119.39 ns** |    **14,501.3 ns** |  **1.00** |    **0.01** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | SingleElementMoved |    17,659.4 ns |    264.16 ns |    138.16 ns |    17,604.0 ns |  1.22 |    0.01 |    9 |         - |          NA |
| BottomupMergeSort        | 1024 | SingleElementMoved |     5,151.2 ns |    179.30 ns |     93.78 ns |     5,116.1 ns |  0.36 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 1024 | SingleElementMoved |     3,405.0 ns |     57.39 ns |     20.47 ns |     3,399.9 ns |  0.23 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 1024 | SingleElementMoved |     1,268.2 ns |     29.34 ns |     13.03 ns |     1,258.5 ns |  0.09 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 1024 | SingleElementMoved |     1,819.0 ns |     11.44 ns |      5.08 ns |     1,818.1 ns |  0.13 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 1024 | SingleElementMoved |     1,556.3 ns |    128.70 ns |     45.90 ns |     1,572.5 ns |  0.11 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | SingleElementMoved |    11,047.7 ns |    152.46 ns |     79.74 ns |    11,059.6 ns |  0.76 |    0.01 |    7 |         - |          NA |
| NaturalMergeSort         | 1024 | SingleElementMoved |     1,734.9 ns |      3.48 ns |      1.55 ns |     1,734.2 ns |  0.12 |    0.00 |    3 |         - |          NA |
| TimSort                  | 1024 | SingleElementMoved |       658.9 ns |      5.27 ns |      2.34 ns |       659.0 ns |  0.05 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | SingleElementMoved |     1,199.0 ns |      1.44 ns |      0.64 ns |     1,199.0 ns |  0.08 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 1024 | SingleElementMoved |     1,119.5 ns |      2.29 ns |      1.02 ns |     1,118.9 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 1024 | SingleElementMoved |     3,563.1 ns |     32.63 ns |     11.64 ns |     3,559.7 ns |  0.25 |    0.00 |    5 |         - |          NA |
| Glidesort                | 1024 | SingleElementMoved |     2,349.1 ns |      9.64 ns |      5.04 ns |     2,350.1 ns |  0.16 |    0.00 |    4 |         - |          NA |
| Driftsort                | 1024 | SingleElementMoved |     1,034.2 ns |      3.46 ns |      1.24 ns |     1,034.3 ns |  0.07 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 1024 | SingleElementMoved |     4,483.3 ns |     34.69 ns |     12.37 ns |     4,479.9 ns |  0.31 |    0.00 |    6 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **1024** | **Sorted**             |    **13,347.0 ns** |    **279.84 ns** |    **124.25 ns** |    **13,351.0 ns** |  **1.00** |    **0.01** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Sorted             |    16,491.6 ns |     97.77 ns |     43.41 ns |    16,476.2 ns |  1.24 |    0.01 |    7 |         - |          NA |
| BottomupMergeSort        | 1024 | Sorted             |     3,813.3 ns |     39.40 ns |     17.49 ns |     3,806.8 ns |  0.29 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 1024 | Sorted             |     2,473.2 ns |    176.28 ns |     78.27 ns |     2,438.4 ns |  0.19 |    0.01 |    3 |         - |          NA |
| RotateMergeSort          | 1024 | Sorted             |     1,247.5 ns |     15.26 ns |      6.78 ns |     1,244.1 ns |  0.09 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Sorted             |     1,153.1 ns |     15.03 ns |      5.36 ns |     1,150.7 ns |  0.09 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 1024 | Sorted             |     1,111.7 ns |      7.18 ns |      3.19 ns |     1,110.6 ns |  0.08 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 1024 | Sorted             |     9,145.6 ns |    139.11 ns |     72.76 ns |     9,105.5 ns |  0.69 |    0.01 |    5 |         - |          NA |
| NaturalMergeSort         | 1024 | Sorted             |       521.3 ns |     26.45 ns |     13.83 ns |       523.5 ns |  0.04 |    0.00 |    1 |         - |          NA |
| TimSort                  | 1024 | Sorted             |       524.1 ns |     87.48 ns |     45.76 ns |       538.7 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Sorted             |       452.7 ns |      6.40 ns |      3.35 ns |       451.9 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Sorted             |       483.0 ns |     10.28 ns |      3.67 ns |       480.9 ns |  0.04 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 1024 | Sorted             |       417.1 ns |     12.29 ns |      5.46 ns |       417.0 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Sorted             |       437.9 ns |     18.75 ns |      8.33 ns |       440.0 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | Sorted             |       499.7 ns |      3.16 ns |      1.40 ns |       499.3 ns |  0.04 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Sorted             |     3,903.8 ns |     19.66 ns |      7.01 ns |     3,902.4 ns |  0.29 |    0.00 |    4 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **1024** | **Reversed**           |    **29,257.1 ns** |    **628.55 ns** |    **328.74 ns** |    **29,189.7 ns** |  **1.00** |    **0.01** |    **5** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Reversed           |    26,733.2 ns |    419.69 ns |    219.51 ns |    26,770.5 ns |  0.91 |    0.01 |    5 |         - |          NA |
| BottomupMergeSort        | 1024 | Reversed           |    16,481.4 ns |    607.99 ns |    317.99 ns |    16,396.2 ns |  0.56 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 1024 | Reversed           |    13,669.6 ns |    212.51 ns |     94.35 ns |    13,685.6 ns |  0.47 |    0.01 |    3 |         - |          NA |
| RotateMergeSort          | 1024 | Reversed           |     8,152.0 ns |    204.86 ns |     90.96 ns |     8,161.0 ns |  0.28 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Reversed           |     7,511.5 ns |    300.15 ns |    156.98 ns |     7,471.5 ns |  0.26 |    0.01 |    2 |         - |          NA |
| SymMergeSort             | 1024 | Reversed           |     7,136.2 ns |    160.47 ns |     83.93 ns |     7,103.4 ns |  0.24 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 1024 | Reversed           |    12,439.4 ns |    271.26 ns |    141.87 ns |    12,452.6 ns |  0.43 |    0.01 |    3 |         - |          NA |
| NaturalMergeSort         | 1024 | Reversed           |       780.7 ns |     55.86 ns |     24.80 ns |       773.9 ns |  0.03 |    0.00 |    1 |         - |          NA |
| TimSort                  | 1024 | Reversed           |       713.7 ns |      3.69 ns |      1.64 ns |       714.5 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Reversed           |       705.1 ns |      2.61 ns |      1.16 ns |       705.0 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Reversed           |       716.0 ns |      1.33 ns |      0.59 ns |       715.9 ns |  0.02 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 1024 | Reversed           |       768.7 ns |     40.56 ns |     21.22 ns |       764.4 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Reversed           |       748.6 ns |      5.35 ns |      2.80 ns |       748.0 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | Reversed           |       756.3 ns |     13.43 ns |      5.96 ns |       753.0 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Reversed           |     9,391.2 ns |    131.25 ns |     58.27 ns |     9,404.6 ns |  0.32 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **1024** | **PipeOrgan**          |    **21,914.4 ns** |    **490.18 ns** |    **256.37 ns** |    **21,873.9 ns** |  **1.00** |    **0.02** |    **9** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | PipeOrgan          |    21,973.8 ns |    369.03 ns |    163.85 ns |    21,956.2 ns |  1.00 |    0.01 |    9 |         - |          NA |
| BottomupMergeSort        | 1024 | PipeOrgan          |    10,438.3 ns |     40.00 ns |     14.26 ns |    10,432.9 ns |  0.48 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 1024 | PipeOrgan          |     8,689.2 ns |    225.81 ns |    118.10 ns |     8,727.2 ns |  0.40 |    0.01 |    6 |         - |          NA |
| RotateMergeSort          | 1024 | PipeOrgan          |    14,655.1 ns |    224.54 ns |     99.70 ns |    14,687.1 ns |  0.67 |    0.01 |    7 |         - |          NA |
| RotateMergeSortRecursive | 1024 | PipeOrgan          |    17,823.7 ns |    143.16 ns |     63.57 ns |    17,806.0 ns |  0.81 |    0.01 |    8 |         - |          NA |
| SymMergeSort             | 1024 | PipeOrgan          |     9,191.4 ns |    212.64 ns |    111.21 ns |     9,174.1 ns |  0.42 |    0.01 |    6 |         - |          NA |
| BlockMergeSort           | 1024 | PipeOrgan          |    12,669.7 ns |    251.83 ns |    131.71 ns |    12,683.2 ns |  0.58 |    0.01 |    7 |         - |          NA |
| NaturalMergeSort         | 1024 | PipeOrgan          |     2,008.7 ns |      8.32 ns |      3.69 ns |     2,007.9 ns |  0.09 |    0.00 |    3 |         - |          NA |
| TimSort                  | 1024 | PipeOrgan          |     2,073.7 ns |     35.69 ns |     12.73 ns |     2,070.2 ns |  0.09 |    0.00 |    3 |         - |          NA |
| PowerSort                | 1024 | PipeOrgan          |     1,571.7 ns |    256.35 ns |    113.82 ns |     1,550.6 ns |  0.07 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 1024 | PipeOrgan          |     1,498.4 ns |     77.54 ns |     27.65 ns |     1,487.2 ns |  0.07 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 1024 | PipeOrgan          |     6,903.2 ns |    576.87 ns |    301.72 ns |     7,012.0 ns |  0.32 |    0.01 |    5 |         - |          NA |
| Glidesort                | 1024 | PipeOrgan          |     3,801.0 ns |    232.90 ns |    121.81 ns |     3,748.5 ns |  0.17 |    0.01 |    4 |         - |          NA |
| Driftsort                | 1024 | PipeOrgan          |     1,200.9 ns |      9.36 ns |      4.89 ns |     1,198.8 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | PipeOrgan          |     7,218.4 ns |    198.21 ns |    103.67 ns |     7,218.9 ns |  0.33 |    0.01 |    5 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **4096** | **Random**             |   **130,271.0 ns** |  **8,682.20 ns** |  **3,854.95 ns** |   **128,851.5 ns** |  **1.00** |    **0.04** |    **2** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Random             |   120,364.6 ns |  1,337.91 ns |    594.04 ns |   120,429.8 ns |  0.92 |    0.03 |    2 |         - |          NA |
| BottomupMergeSort        | 4096 | Random             |    75,554.7 ns |  2,979.22 ns |  1,322.79 ns |    75,510.5 ns |  0.58 |    0.02 |    1 |         - |          NA |
| StdStableSort            | 4096 | Random             |    60,501.2 ns |  2,058.57 ns |    914.02 ns |    60,875.4 ns |  0.46 |    0.01 |    1 |         - |          NA |
| RotateMergeSort          | 4096 | Random             |   486,070.6 ns |  7,400.02 ns |  3,870.35 ns |   485,255.7 ns |  3.73 |    0.11 |    4 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Random             |   523,833.8 ns |  4,173.13 ns |  2,182.63 ns |   523,115.6 ns |  4.02 |    0.11 |    4 |         - |          NA |
| SymMergeSort             | 4096 | Random             |   321,826.7 ns |  2,376.35 ns |  1,055.11 ns |   321,649.6 ns |  2.47 |    0.07 |    3 |         - |          NA |
| BlockMergeSort           | 4096 | Random             |   105,999.6 ns |  5,440.58 ns |  2,415.65 ns |   105,000.1 ns |  0.81 |    0.03 |    2 |         - |          NA |
| NaturalMergeSort         | 4096 | Random             |   107,113.2 ns |  5,160.19 ns |  2,698.88 ns |   105,593.0 ns |  0.82 |    0.03 |    2 |         - |          NA |
| TimSort                  | 4096 | Random             |    74,775.6 ns |  2,180.98 ns |    968.37 ns |    74,778.0 ns |  0.57 |    0.02 |    1 |         - |          NA |
| PowerSort                | 4096 | Random             |    50,623.7 ns |  1,529.03 ns |    678.90 ns |    50,435.9 ns |  0.39 |    0.01 |    1 |         - |          NA |
| ShiftSort                | 4096 | Random             |    70,962.4 ns |  1,545.83 ns |    551.26 ns |    70,781.1 ns |  0.55 |    0.02 |    1 |         - |          NA |
| SpinSort                 | 4096 | Random             |    48,495.3 ns |  1,023.39 ns |    454.39 ns |    48,606.1 ns |  0.37 |    0.01 |    1 |         - |          NA |
| Glidesort                | 4096 | Random             |    42,006.8 ns |  1,305.02 ns |    579.44 ns |    41,793.9 ns |  0.32 |    0.01 |    1 |         - |          NA |
| Driftsort                | 4096 | Random             |    53,208.6 ns |    969.27 ns |    430.36 ns |    53,138.7 ns |  0.41 |    0.01 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Random             |    53,401.8 ns |  2,906.72 ns |  1,290.60 ns |    52,923.0 ns |  0.41 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **4096** | **SingleElementMoved** |    **58,033.7 ns** |    **441.88 ns** |    **231.11 ns** |    **58,083.8 ns** |  **1.00** |    **0.01** |    **9** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | SingleElementMoved |    70,555.8 ns |    153.62 ns |     68.21 ns |    70,560.0 ns |  1.22 |    0.00 |   10 |         - |          NA |
| BottomupMergeSort        | 4096 | SingleElementMoved |    20,381.3 ns |    545.95 ns |    242.40 ns |    20,403.0 ns |  0.35 |    0.00 |    7 |         - |          NA |
| StdStableSort            | 4096 | SingleElementMoved |    17,235.0 ns |  1,507.46 ns |    788.43 ns |    16,746.8 ns |  0.30 |    0.01 |    7 |         - |          NA |
| RotateMergeSort          | 4096 | SingleElementMoved |     6,407.9 ns |  3,338.52 ns |  1,746.11 ns |     5,540.7 ns |  0.11 |    0.03 |    3 |         - |          NA |
| RotateMergeSortRecursive | 4096 | SingleElementMoved |     6,804.4 ns |    263.55 ns |     93.99 ns |     6,853.4 ns |  0.12 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 4096 | SingleElementMoved |     5,763.4 ns |    246.73 ns |    129.04 ns |     5,753.6 ns |  0.10 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 4096 | SingleElementMoved |    44,806.8 ns |    618.92 ns |    323.71 ns |    44,773.7 ns |  0.77 |    0.01 |    8 |         - |          NA |
| NaturalMergeSort         | 4096 | SingleElementMoved |     9,133.9 ns |  5,733.52 ns |  2,998.74 ns |     7,106.2 ns |  0.16 |    0.05 |    4 |         - |          NA |
| TimSort                  | 4096 | SingleElementMoved |     2,398.3 ns |     20.62 ns |      7.35 ns |     2,395.9 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | SingleElementMoved |     4,779.2 ns |    314.67 ns |    139.72 ns |     4,781.7 ns |  0.08 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 4096 | SingleElementMoved |     4,560.3 ns |    241.49 ns |    126.31 ns |     4,590.8 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 4096 | SingleElementMoved |    11,256.7 ns |    307.05 ns |    160.59 ns |    11,236.6 ns |  0.19 |    0.00 |    6 |         - |          NA |
| Glidesort                | 4096 | SingleElementMoved |     9,247.2 ns |    211.09 ns |     75.28 ns |     9,221.7 ns |  0.16 |    0.00 |    5 |         - |          NA |
| Driftsort                | 4096 | SingleElementMoved |     3,991.4 ns |      6.86 ns |      3.59 ns |     3,990.6 ns |  0.07 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 4096 | SingleElementMoved |    18,745.8 ns |    508.60 ns |    225.82 ns |    18,622.4 ns |  0.32 |    0.00 |    7 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **4096** | **Sorted**             |    **53,285.5 ns** |    **439.15 ns** |    **229.68 ns** |    **53,300.0 ns** |  **1.00** |    **0.01** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Sorted             |    66,502.1 ns |    469.25 ns |    245.42 ns |    66,453.2 ns |  1.25 |    0.01 |    7 |         - |          NA |
| BottomupMergeSort        | 4096 | Sorted             |    15,590.5 ns |    157.49 ns |     82.37 ns |    15,562.3 ns |  0.29 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 4096 | Sorted             |    12,456.6 ns |    373.51 ns |    165.84 ns |    12,463.1 ns |  0.23 |    0.00 |    3 |         - |          NA |
| RotateMergeSort          | 4096 | Sorted             |     4,453.9 ns |      4.80 ns |      1.71 ns |     4,453.2 ns |  0.08 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Sorted             |     4,698.7 ns |    250.74 ns |    131.14 ns |     4,627.7 ns |  0.09 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 4096 | Sorted             |     4,426.2 ns |     22.59 ns |     10.03 ns |     4,422.3 ns |  0.08 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 4096 | Sorted             |    37,088.4 ns |    620.96 ns |    324.77 ns |    37,070.6 ns |  0.70 |    0.01 |    5 |         - |          NA |
| NaturalMergeSort         | 4096 | Sorted             |     1,816.8 ns |      4.86 ns |      1.73 ns |     1,817.1 ns |  0.03 |    0.00 |    1 |         - |          NA |
| TimSort                  | 4096 | Sorted             |     1,774.8 ns |      2.15 ns |      0.96 ns |     1,774.6 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | Sorted             |     1,862.3 ns |     16.86 ns |      7.49 ns |     1,862.2 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | Sorted             |     1,769.3 ns |    151.29 ns |     67.17 ns |     1,736.1 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 4096 | Sorted             |     1,578.4 ns |     14.52 ns |      6.45 ns |     1,578.5 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 4096 | Sorted             |     1,543.8 ns |     17.59 ns |      9.20 ns |     1,544.0 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | Sorted             |     1,826.4 ns |     14.32 ns |      6.36 ns |     1,826.8 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Sorted             |    15,673.3 ns |    246.34 ns |     87.85 ns |    15,651.2 ns |  0.29 |    0.00 |    4 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **4096** | **Reversed**           |   **122,204.7 ns** |  **1,827.06 ns** |    **955.59 ns** |   **122,011.2 ns** |  **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Reversed           |   110,801.0 ns |  1,677.59 ns |    744.86 ns |   110,906.5 ns |  0.91 |    0.01 |    4 |         - |          NA |
| BottomupMergeSort        | 4096 | Reversed           |    70,021.2 ns |  1,868.39 ns |    829.58 ns |    69,939.6 ns |  0.57 |    0.01 |    3 |         - |          NA |
| StdStableSort            | 4096 | Reversed           |    58,252.0 ns |    706.75 ns |    369.64 ns |    58,142.6 ns |  0.48 |    0.00 |    3 |         - |          NA |
| RotateMergeSort          | 4096 | Reversed           |    37,067.8 ns |    592.77 ns |    310.03 ns |    37,013.4 ns |  0.30 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Reversed           |    34,857.3 ns |    812.21 ns |    424.80 ns |    34,721.7 ns |  0.29 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 4096 | Reversed           |    30,590.7 ns |    611.47 ns |    319.81 ns |    30,463.8 ns |  0.25 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 4096 | Reversed           |    56,717.0 ns |    313.24 ns |    139.08 ns |    56,769.4 ns |  0.46 |    0.00 |    3 |         - |          NA |
| NaturalMergeSort         | 4096 | Reversed           |     2,801.1 ns |     17.70 ns |      7.86 ns |     2,800.0 ns |  0.02 |    0.00 |    1 |         - |          NA |
| TimSort                  | 4096 | Reversed           |     2,754.8 ns |     16.29 ns |      7.23 ns |     2,752.0 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | Reversed           |     2,744.5 ns |     20.32 ns |      9.02 ns |     2,741.9 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | Reversed           |     2,695.5 ns |    144.57 ns |     64.19 ns |     2,668.4 ns |  0.02 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 4096 | Reversed           |     3,016.6 ns |    307.17 ns |    136.39 ns |     2,941.9 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Glidesort                | 4096 | Reversed           |     2,918.6 ns |     10.26 ns |      4.56 ns |     2,918.5 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | Reversed           |     2,866.4 ns |    165.80 ns |     73.62 ns |     2,812.5 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Reversed           |    36,527.5 ns |  1,608.95 ns |    841.51 ns |    36,082.7 ns |  0.30 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **4096** | **PipeOrgan**          |    **91,200.9 ns** |    **717.35 ns** |    **318.51 ns** |    **91,271.8 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | PipeOrgan          |    91,213.5 ns |  1,410.17 ns |    737.55 ns |    91,188.9 ns |  1.00 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 4096 | PipeOrgan          |    46,001.5 ns |  1,073.97 ns |    561.71 ns |    45,948.7 ns |  0.50 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 4096 | PipeOrgan          |    38,704.6 ns |  2,158.75 ns |  1,129.07 ns |    38,253.0 ns |  0.42 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 4096 | PipeOrgan          |    63,860.6 ns |    938.41 ns |    416.66 ns |    63,849.2 ns |  0.70 |    0.00 |    6 |         - |          NA |
| RotateMergeSortRecursive | 4096 | PipeOrgan          |    76,633.6 ns |    974.71 ns |    509.79 ns |    76,435.2 ns |  0.84 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 4096 | PipeOrgan          |    38,414.7 ns |  1,174.64 ns |    521.55 ns |    38,477.9 ns |  0.42 |    0.01 |    5 |         - |          NA |
| BlockMergeSort           | 4096 | PipeOrgan          |    53,124.7 ns |    463.65 ns |    205.86 ns |    53,076.2 ns |  0.58 |    0.00 |    5 |         - |          NA |
| NaturalMergeSort         | 4096 | PipeOrgan          |     7,972.1 ns |  1,038.84 ns |    461.25 ns |     7,715.4 ns |  0.09 |    0.00 |    2 |         - |          NA |
| TimSort                  | 4096 | PipeOrgan          |     8,079.9 ns |    223.24 ns |    116.76 ns |     8,107.0 ns |  0.09 |    0.00 |    2 |         - |          NA |
| PowerSort                | 4096 | PipeOrgan          |     5,346.0 ns |    337.64 ns |    149.92 ns |     5,277.3 ns |  0.06 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | PipeOrgan          |     5,677.0 ns |     37.37 ns |     13.33 ns |     5,671.9 ns |  0.06 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 4096 | PipeOrgan          |     6,889.8 ns |    443.36 ns |    196.85 ns |     6,902.1 ns |  0.08 |    0.00 |    2 |         - |          NA |
| Glidesort                | 4096 | PipeOrgan          |    14,673.1 ns |     69.53 ns |     30.87 ns |    14,668.6 ns |  0.16 |    0.00 |    3 |         - |          NA |
| Driftsort                | 4096 | PipeOrgan          |     4,563.3 ns |     46.07 ns |     20.45 ns |     4,559.0 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | PipeOrgan          |    28,977.3 ns |    304.97 ns |    135.41 ns |    28,945.0 ns |  0.32 |    0.00 |    4 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **8192** | **Random**             |   **536,698.9 ns** |  **6,798.29 ns** |  **3,018.48 ns** |   **535,475.5 ns** |  **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Random             |   524,548.9 ns |  4,421.69 ns |  2,312.63 ns |   524,237.0 ns |  0.98 |    0.01 |    4 |         - |          NA |
| BottomupMergeSort        | 8192 | Random             |   414,870.1 ns |  2,527.87 ns |  1,122.39 ns |   415,260.4 ns |  0.77 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 8192 | Random             |   292,650.2 ns |  5,484.95 ns |  2,868.74 ns |   292,952.5 ns |  0.55 |    0.01 |    3 |         - |          NA |
| RotateMergeSort          | 8192 | Random             | 1,192,826.4 ns |  7,972.14 ns |  4,169.58 ns | 1,192,122.2 ns |  2.22 |    0.01 |    6 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Random             | 1,296,991.5 ns |  3,652.07 ns |  1,910.11 ns | 1,296,524.1 ns |  2.42 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 8192 | Random             |   880,627.1 ns |  2,624.28 ns |  1,165.20 ns |   880,518.8 ns |  1.64 |    0.01 |    5 |         - |          NA |
| BlockMergeSort           | 8192 | Random             |   518,873.1 ns |  1,142.85 ns |    507.43 ns |   518,742.9 ns |  0.97 |    0.01 |    4 |         - |          NA |
| NaturalMergeSort         | 8192 | Random             |   531,186.2 ns |  1,088.36 ns |    483.24 ns |   531,058.0 ns |  0.99 |    0.01 |    4 |         - |          NA |
| TimSort                  | 8192 | Random             |   463,242.9 ns | 52,876.94 ns | 23,477.69 ns |   453,419.0 ns |  0.86 |    0.04 |    4 |         - |          NA |
| PowerSort                | 8192 | Random             |   336,765.6 ns |  2,408.86 ns |  1,069.55 ns |   336,533.4 ns |  0.63 |    0.00 |    3 |         - |          NA |
| ShiftSort                | 8192 | Random             |   463,515.9 ns |  3,656.94 ns |  1,912.65 ns |   463,372.6 ns |  0.86 |    0.01 |    4 |         - |          NA |
| SpinSort                 | 8192 | Random             |   277,686.5 ns |  5,545.74 ns |  2,900.53 ns |   277,088.5 ns |  0.52 |    0.01 |    3 |         - |          NA |
| Glidesort                | 8192 | Random             |    84,943.2 ns |    372.65 ns |    165.46 ns |    84,934.6 ns |  0.16 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Random             |   109,040.3 ns |  2,089.75 ns |  1,092.98 ns |   109,116.9 ns |  0.20 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 8192 | Random             |   313,744.7 ns |  4,450.79 ns |  2,327.85 ns |   313,112.2 ns |  0.58 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **8192** | **SingleElementMoved** |   **115,703.6 ns** |    **925.26 ns** |    **410.82 ns** |   **115,606.7 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | SingleElementMoved |   141,589.9 ns |  1,094.85 ns |    486.12 ns |   141,688.8 ns |  1.22 |    0.01 |    8 |         - |          NA |
| BottomupMergeSort        | 8192 | SingleElementMoved |    42,613.5 ns |    726.24 ns |    379.84 ns |    42,621.8 ns |  0.37 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 8192 | SingleElementMoved |    32,361.5 ns |    655.04 ns |    342.60 ns |    32,222.7 ns |  0.28 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 8192 | SingleElementMoved |     9,013.6 ns |    263.48 ns |    137.80 ns |     8,988.8 ns |  0.08 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 8192 | SingleElementMoved |    13,253.2 ns |    309.11 ns |    161.67 ns |    13,197.9 ns |  0.11 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 8192 | SingleElementMoved |    11,340.3 ns |    417.72 ns |    148.96 ns |    11,396.5 ns |  0.10 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 8192 | SingleElementMoved |    88,518.1 ns |    503.29 ns |    263.23 ns |    88,466.6 ns |  0.77 |    0.00 |    6 |         - |          NA |
| NaturalMergeSort         | 8192 | SingleElementMoved |    13,632.6 ns |    237.68 ns |     84.76 ns |    13,594.2 ns |  0.12 |    0.00 |    3 |         - |          NA |
| TimSort                  | 8192 | SingleElementMoved |     4,796.7 ns |    241.34 ns |    107.16 ns |     4,747.8 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | SingleElementMoved |     9,256.0 ns |    373.77 ns |    165.96 ns |     9,150.3 ns |  0.08 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 8192 | SingleElementMoved |     8,723.3 ns |    347.39 ns |    154.24 ns |     8,605.9 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | SingleElementMoved |    19,413.2 ns |  1,707.17 ns |    892.88 ns |    19,402.5 ns |  0.17 |    0.01 |    4 |         - |          NA |
| Glidesort                | 8192 | SingleElementMoved |    18,662.6 ns |    682.95 ns |    303.23 ns |    18,545.6 ns |  0.16 |    0.00 |    4 |         - |          NA |
| Driftsort                | 8192 | SingleElementMoved |     8,052.5 ns |    455.93 ns |    202.44 ns |     8,058.1 ns |  0.07 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 8192 | SingleElementMoved |    35,468.3 ns |    680.85 ns |    356.10 ns |    35,555.7 ns |  0.31 |    0.00 |    5 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **8192** | **Sorted**             |   **106,632.6 ns** |    **456.33 ns** |    **238.67 ns** |   **106,557.2 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Sorted             |   132,841.1 ns |    540.18 ns |    239.84 ns |   132,915.2 ns |  1.25 |    0.00 |    7 |         - |          NA |
| BottomupMergeSort        | 8192 | Sorted             |    34,414.3 ns |  1,025.14 ns |    536.17 ns |    34,348.8 ns |  0.32 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 8192 | Sorted             |    23,904.2 ns |  1,104.73 ns |    577.79 ns |    23,728.5 ns |  0.22 |    0.01 |    3 |         - |          NA |
| RotateMergeSort          | 8192 | Sorted             |     9,135.3 ns |    285.58 ns |    126.80 ns |     9,116.1 ns |  0.09 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Sorted             |     9,505.9 ns |    406.61 ns |    145.00 ns |     9,497.3 ns |  0.09 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 8192 | Sorted             |     9,248.8 ns |    298.17 ns |    155.95 ns |     9,299.7 ns |  0.09 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 8192 | Sorted             |    92,263.1 ns | 42,989.02 ns | 22,484.09 ns |    85,954.9 ns |  0.87 |    0.20 |    5 |         - |          NA |
| NaturalMergeSort         | 8192 | Sorted             |     3,589.4 ns |     21.21 ns |      9.42 ns |     3,586.2 ns |  0.03 |    0.00 |    1 |         - |          NA |
| TimSort                  | 8192 | Sorted             |     3,546.8 ns |     25.76 ns |     11.44 ns |     3,541.8 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Sorted             |     3,539.2 ns |     24.96 ns |      8.90 ns |     3,537.8 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Sorted             |     3,508.0 ns |    271.94 ns |    120.74 ns |     3,446.4 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 8192 | Sorted             |     3,197.4 ns |    315.15 ns |    139.93 ns |     3,126.0 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Sorted             |     3,102.8 ns |     39.97 ns |     17.75 ns |     3,106.9 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Sorted             |     3,721.2 ns |    244.61 ns |    127.94 ns |     3,639.1 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Sorted             |     3,206.6 ns |     23.34 ns |     10.36 ns |     3,210.9 ns |  0.03 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **8192** | **Reversed**           |   **251,367.7 ns** |  **5,474.09 ns** |  **2,863.06 ns** |   **250,452.2 ns** |  **1.00** |    **0.02** |    **5** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Reversed           |   225,461.9 ns |  3,283.54 ns |  1,457.91 ns |   225,226.0 ns |  0.90 |    0.01 |    5 |         - |          NA |
| BottomupMergeSort        | 8192 | Reversed           |   143,762.9 ns |  2,331.66 ns |  1,219.50 ns |   143,581.3 ns |  0.57 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 8192 | Reversed           |   206,464.1 ns |  1,532.67 ns |    801.62 ns |   206,362.9 ns |  0.82 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 8192 | Reversed           |    78,621.8 ns |    912.59 ns |    477.30 ns |    78,676.1 ns |  0.31 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Reversed           |    73,807.7 ns |    981.94 ns |    513.57 ns |    73,846.0 ns |  0.29 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 8192 | Reversed           |    63,691.3 ns |  1,258.27 ns |    558.68 ns |    63,586.4 ns |  0.25 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 8192 | Reversed           |   118,490.5 ns |    348.20 ns |    182.12 ns |   118,505.3 ns |  0.47 |    0.01 |    3 |         - |          NA |
| NaturalMergeSort         | 8192 | Reversed           |     5,652.5 ns |    175.97 ns |     92.04 ns |     5,659.7 ns |  0.02 |    0.00 |    1 |         - |          NA |
| TimSort                  | 8192 | Reversed           |     5,600.1 ns |     19.43 ns |      6.93 ns |     5,598.6 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Reversed           |     5,580.6 ns |    174.63 ns |     91.34 ns |     5,547.2 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Reversed           |     5,397.4 ns |    254.94 ns |    133.34 ns |     5,374.3 ns |  0.02 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 8192 | Reversed           |     5,967.7 ns |    279.07 ns |    123.91 ns |     5,901.1 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Reversed           |     5,646.5 ns |    227.36 ns |    118.92 ns |     5,582.0 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Reversed           |     5,563.5 ns |     65.88 ns |     23.49 ns |     5,552.3 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Reversed           |     6,197.8 ns |    325.40 ns |    144.48 ns |     6,110.8 ns |  0.02 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **8192** | **PipeOrgan**          |   **181,649.4 ns** |  **1,788.48 ns** |    **935.41 ns** |   **181,922.8 ns** |  **1.00** |    **0.01** |    **5** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | PipeOrgan          |   183,505.7 ns |  1,726.58 ns |    903.04 ns |   183,680.9 ns |  1.01 |    0.01 |    5 |         - |          NA |
| BottomupMergeSort        | 8192 | PipeOrgan          |    93,749.0 ns |  2,680.33 ns |  1,401.86 ns |    93,683.0 ns |  0.52 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 8192 | PipeOrgan          |   121,504.6 ns |    842.82 ns |    440.81 ns |   121,479.4 ns |  0.67 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 8192 | PipeOrgan          |   132,107.7 ns |    682.72 ns |    357.07 ns |   132,242.1 ns |  0.73 |    0.00 |    5 |         - |          NA |
| RotateMergeSortRecursive | 8192 | PipeOrgan          |   156,608.3 ns |  1,844.95 ns |    819.17 ns |   156,855.8 ns |  0.86 |    0.01 |    5 |         - |          NA |
| SymMergeSort             | 8192 | PipeOrgan          |    78,714.9 ns |    785.13 ns |    410.64 ns |    78,703.6 ns |  0.43 |    0.00 |    5 |         - |          NA |
| BlockMergeSort           | 8192 | PipeOrgan          |   108,495.0 ns |    531.62 ns |    278.05 ns |   108,489.1 ns |  0.60 |    0.00 |    5 |         - |          NA |
| NaturalMergeSort         | 8192 | PipeOrgan          |    16,277.3 ns |  2,771.45 ns |  1,230.54 ns |    15,693.5 ns |  0.09 |    0.01 |    2 |         - |          NA |
| TimSort                  | 8192 | PipeOrgan          |    16,444.9 ns |  1,136.50 ns |    594.41 ns |    16,398.5 ns |  0.09 |    0.00 |    2 |         - |          NA |
| PowerSort                | 8192 | PipeOrgan          |    10,780.6 ns |    452.48 ns |    200.90 ns |    10,807.4 ns |  0.06 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | PipeOrgan          |    11,457.3 ns |    262.42 ns |     93.58 ns |    11,488.8 ns |  0.06 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 8192 | PipeOrgan          |    14,375.6 ns |    364.99 ns |    162.06 ns |    14,373.5 ns |  0.08 |    0.00 |    2 |         - |          NA |
| Glidesort                | 8192 | PipeOrgan          |    29,813.2 ns |  1,525.79 ns |    798.02 ns |    29,535.8 ns |  0.16 |    0.00 |    3 |         - |          NA |
| Driftsort                | 8192 | PipeOrgan          |     8,987.5 ns |    394.35 ns |    175.09 ns |     9,022.5 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | PipeOrgan          |    57,230.8 ns |  1,109.82 ns |    492.77 ns |    57,081.4 ns |  0.32 |    0.00 |    4 |         - |          NA |

### NetworkBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                  | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------------ |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **BitonicSort**             | **256**  | **Random**             |     **8,426.5 ns** |  **1,304.71 ns** |    **682.39 ns** |  **1.01** |    **0.11** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Random             |    17,968.6 ns |    228.17 ns |    101.31 ns |  2.14 |    0.16 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Random             |    14,520.1 ns |    137.17 ns |     60.90 ns |  1.73 |    0.13 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **256**  | **SingleElementMoved** |     **7,617.5 ns** |    **277.12 ns** |    **144.94 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | SingleElementMoved |    18,107.4 ns |    159.90 ns |     71.00 ns |  2.38 |    0.04 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | SingleElementMoved |    14,492.2 ns |     37.98 ns |     19.87 ns |  1.90 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **256**  | **Sorted**             |     **7,441.2 ns** |    **401.89 ns** |    **210.20 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Sorted             |    17,815.2 ns |     58.03 ns |     25.77 ns |  2.40 |    0.06 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Sorted             |    14,536.4 ns |    124.77 ns |     65.26 ns |  1.95 |    0.05 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **256**  | **Reversed**           |     **7,465.3 ns** |    **225.67 ns** |    **118.03 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Reversed           |    18,064.4 ns |    265.96 ns |    139.10 ns |  2.42 |    0.04 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Reversed           |    14,528.7 ns |     50.51 ns |     26.42 ns |  1.95 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **256**  | **PipeOrgan**          |     **7,455.1 ns** |    **156.19 ns** |     **55.70 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | PipeOrgan          |    17,886.9 ns |     72.94 ns |     38.15 ns |  2.40 |    0.02 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | PipeOrgan          |    14,460.4 ns |    182.19 ns |     80.89 ns |  1.94 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Random**             |    **48,060.3 ns** |  **1,063.84 ns** |    **556.41 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Random             |    92,531.4 ns |    541.37 ns |    283.15 ns |  1.93 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Random             |    89,198.7 ns |    226.10 ns |    118.26 ns |  1.86 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **1024** | **SingleElementMoved** |    **47,311.2 ns** |  **1,630.01 ns** |    **852.53 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | SingleElementMoved |    93,219.9 ns |    547.87 ns |    243.26 ns |  1.97 |    0.03 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | SingleElementMoved |    89,339.6 ns |    315.39 ns |    164.95 ns |  1.89 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Sorted**             |    **45,375.7 ns** |  **3,044.83 ns** |  **1,592.50 ns** |  **1.00** |    **0.05** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Sorted             |    92,279.0 ns |    197.72 ns |    103.41 ns |  2.04 |    0.07 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Sorted             |    89,285.7 ns |    129.47 ns |     67.72 ns |  1.97 |    0.06 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Reversed**           |    **46,334.1 ns** |  **1,286.11 ns** |    **571.04 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Reversed           |    92,867.9 ns |    471.32 ns |    209.27 ns |  2.00 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Reversed           |    89,249.8 ns |    171.95 ns |     89.93 ns |  1.93 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **1024** | **PipeOrgan**          |    **46,596.8 ns** |    **958.24 ns** |    **501.18 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | PipeOrgan          |    93,008.1 ns |    246.09 ns |    109.27 ns |  2.00 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | PipeOrgan          |    89,246.5 ns |    179.63 ns |     93.95 ns |  1.92 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Random**             |   **447,851.2 ns** |  **2,028.48 ns** |  **1,060.93 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Random             |   650,020.4 ns |  1,384.90 ns |    614.91 ns |  1.45 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Random             |   530,322.9 ns |  1,343.18 ns |    702.51 ns |  1.18 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **4096** | **SingleElementMoved** |   **265,365.5 ns** |  **2,894.04 ns** |  **1,513.64 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | SingleElementMoved |   466,045.6 ns |  2,560.55 ns |  1,339.22 ns |  1.76 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | SingleElementMoved |   511,102.8 ns |    695.10 ns |    308.63 ns |  1.93 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Sorted**             |   **261,119.4 ns** |  **4,969.60 ns** |  **2,599.20 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Sorted             |   459,696.9 ns |    713.33 ns |    316.72 ns |  1.76 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Sorted             |   511,365.7 ns |    807.22 ns |    422.19 ns |  1.96 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Reversed**           |   **259,643.8 ns** |  **1,940.17 ns** |    **861.45 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Reversed           |   463,319.9 ns |  1,325.25 ns |    588.42 ns |  1.78 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Reversed           |   511,043.7 ns |    429.60 ns |    224.69 ns |  1.97 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **4096** | **PipeOrgan**          |   **266,359.8 ns** |  **4,000.25 ns** |  **2,092.21 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | PipeOrgan          |   463,945.6 ns |    954.32 ns |    423.72 ns |  1.74 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | PipeOrgan          |   510,878.9 ns |    271.32 ns |    120.47 ns |  1.92 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **8192** | **Random**             | **1,064,113.1 ns** |  **5,519.41 ns** |  **2,886.76 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | Random             | 1,533,697.2 ns |  1,535.37 ns |    681.72 ns |  1.44 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | Random             | 1,305,423.0 ns |  1,305.80 ns |    682.96 ns |  1.23 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **8192** | **SingleElementMoved** |   **615,000.8 ns** |  **6,789.03 ns** |  **3,550.80 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | SingleElementMoved | 1,048,899.4 ns |  2,267.66 ns |  1,006.85 ns |  1.71 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | SingleElementMoved | 1,195,503.5 ns |    860.11 ns |    449.86 ns |  1.94 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **8192** | **Sorted**             |   **594,702.3 ns** | **25,180.70 ns** | **13,169.99 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | Sorted             | 1,034,612.8 ns |  3,011.11 ns |  1,336.95 ns |  1.74 |    0.04 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | Sorted             | 1,196,167.0 ns |  1,087.01 ns |    568.53 ns |  2.01 |    0.04 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **8192** | **Reversed**           |   **605,127.0 ns** |  **7,222.42 ns** |  **3,206.80 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | Reversed           | 1,044,466.4 ns |  1,896.54 ns |    842.07 ns |  1.73 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | Reversed           | 1,197,876.1 ns |  6,506.24 ns |  2,888.81 ns |  1.98 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **8192** | **PipeOrgan**          |   **612,605.0 ns** |  **5,382.26 ns** |  **2,815.02 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | PipeOrgan          | 1,047,135.9 ns |  2,511.90 ns |  1,115.30 ns |  1.71 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | PipeOrgan          | 1,195,392.2 ns |    562.47 ns |    249.74 ns |  1.95 |    0.01 |    2 |         - |          NA |

### PartitionBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                       | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **QuickSort**                    | **256**  | **Random**             |     **2,056.3 ns** |     **16.20 ns** |      **5.78 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 256  | Random             |     1,926.1 ns |     60.45 ns |     26.84 ns |  0.94 |    0.01 |    1 |         - |          NA |
| QuickSortMedian3             | 256  | Random             |     2,242.1 ns |    119.73 ns |     53.16 ns |  1.09 |    0.02 |    1 |         - |          NA |
| QuickSortMedian9             | 256  | Random             |     2,561.7 ns |     66.62 ns |     29.58 ns |  1.25 |    0.01 |    1 |         - |          NA |
| DualPivotQuickSort           | 256  | Random             |     1,674.4 ns |     82.63 ns |     36.69 ns |  0.81 |    0.02 |    1 |         - |          NA |
| StableQuickSort              | 256  | Random             |     8,868.0 ns |    308.65 ns |    161.43 ns |  4.31 |    0.08 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Random             |     5,539.2 ns |    284.21 ns |    148.65 ns |  2.69 |    0.07 |    2 |         - |          NA |
| DestswapStableQuickSort      | 256  | Random             |     6,076.7 ns |    215.39 ns |    112.65 ns |  2.96 |    0.05 |    2 |         - |          NA |
| IntroSort                    | 256  | Random             |     1,874.8 ns |    241.11 ns |    126.10 ns |  0.91 |    0.06 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | Random             |     1,386.7 ns |     82.22 ns |     36.51 ns |  0.67 |    0.02 |    1 |         - |          NA |
| PDQSort                      | 256  | Random             |     1,392.2 ns |     43.45 ns |     19.29 ns |  0.68 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | Random             |     2,187.7 ns |     47.01 ns |     24.59 ns |  1.06 |    0.01 |    1 |         - |          NA |
| Ipnsort                      | 256  | Random             |     2,846.3 ns |     10.31 ns |      4.58 ns |  1.38 |    0.00 |    1 |         - |          NA |
| StdSort                      | 256  | Random             |     2,358.1 ns |    317.49 ns |    166.05 ns |  1.15 |    0.08 |    1 |         - |          NA |
| BlockQuickSort               | 256  | Random             |     2,271.8 ns |    161.08 ns |     71.52 ns |  1.10 |    0.03 |    1 |         - |          NA |
| DotnetSort                   | 256  | Random             |     1,553.9 ns |     16.54 ns |      7.34 ns |  0.76 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **SingleElementMoved** |     **1,008.4 ns** |    **294.24 ns** |    **130.64 ns** |  **1.01** |    **0.17** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 256  | SingleElementMoved |       808.5 ns |     32.07 ns |     14.24 ns |  0.81 |    0.09 |    1 |         - |          NA |
| QuickSortMedian3             | 256  | SingleElementMoved |     1,371.8 ns |     64.67 ns |     28.71 ns |  1.38 |    0.16 |    2 |         - |          NA |
| QuickSortMedian9             | 256  | SingleElementMoved |     1,648.9 ns |     42.14 ns |     15.03 ns |  1.66 |    0.19 |    2 |         - |          NA |
| DualPivotQuickSort           | 256  | SingleElementMoved |       805.3 ns |    385.41 ns |    201.58 ns |  0.81 |    0.21 |    1 |         - |          NA |
| StableQuickSort              | 256  | SingleElementMoved |     7,241.0 ns |    291.65 ns |    152.54 ns |  7.28 |    0.84 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 256  | SingleElementMoved |     3,870.3 ns |    276.80 ns |    144.77 ns |  3.89 |    0.46 |    4 |         - |          NA |
| DestswapStableQuickSort      | 256  | SingleElementMoved |     3,819.4 ns |    156.92 ns |     69.67 ns |  3.84 |    0.44 |    4 |         - |          NA |
| IntroSort                    | 256  | SingleElementMoved |       677.7 ns |     37.86 ns |     13.50 ns |  0.68 |    0.08 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | SingleElementMoved |       866.9 ns |      3.45 ns |      1.23 ns |  0.87 |    0.10 |    1 |         - |          NA |
| PDQSort                      | 256  | SingleElementMoved |       900.0 ns |     28.40 ns |     12.61 ns |  0.90 |    0.10 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | SingleElementMoved |     1,130.0 ns |     19.75 ns |      8.77 ns |  1.14 |    0.13 |    1 |         - |          NA |
| Ipnsort                      | 256  | SingleElementMoved |     2,811.1 ns |     70.18 ns |     31.16 ns |  2.83 |    0.32 |    3 |         - |          NA |
| StdSort                      | 256  | SingleElementMoved |     1,490.3 ns |    173.47 ns |     77.02 ns |  1.50 |    0.19 |    2 |         - |          NA |
| BlockQuickSort               | 256  | SingleElementMoved |     1,374.8 ns |     32.77 ns |     14.55 ns |  1.38 |    0.16 |    2 |         - |          NA |
| DotnetSort                   | 256  | SingleElementMoved |       837.0 ns |    114.20 ns |     59.73 ns |  0.84 |    0.11 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Sorted**             |       **763.5 ns** |    **109.10 ns** |     **48.44 ns** |  **1.00** |    **0.09** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 256  | Sorted             |       564.5 ns |      8.18 ns |      3.63 ns |  0.74 |    0.05 |    4 |         - |          NA |
| QuickSortMedian3             | 256  | Sorted             |       939.3 ns |     69.90 ns |     31.04 ns |  1.23 |    0.08 |    6 |         - |          NA |
| QuickSortMedian9             | 256  | Sorted             |       991.3 ns |     19.26 ns |     10.07 ns |  1.30 |    0.08 |    6 |         - |          NA |
| DualPivotQuickSort           | 256  | Sorted             |       638.1 ns |    132.95 ns |     69.53 ns |  0.84 |    0.10 |    4 |         - |          NA |
| StableQuickSort              | 256  | Sorted             |     6,661.2 ns |    401.30 ns |    209.89 ns |  8.76 |    0.59 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Sorted             |     4,028.7 ns |    215.87 ns |    112.90 ns |  5.30 |    0.35 |    7 |         - |          NA |
| DestswapStableQuickSort      | 256  | Sorted             |     3,362.1 ns |     26.86 ns |      9.58 ns |  4.42 |    0.27 |    7 |         - |          NA |
| IntroSort                    | 256  | Sorted             |       214.4 ns |      5.37 ns |      2.39 ns |  0.28 |    0.02 |    2 |         - |          NA |
| IntroSortDotnet              | 256  | Sorted             |       753.8 ns |      2.98 ns |      1.32 ns |  0.99 |    0.06 |    5 |         - |          NA |
| PDQSort                      | 256  | Sorted             |       304.5 ns |     14.69 ns |      7.68 ns |  0.40 |    0.03 |    3 |         - |          NA |
| PDQSortBranchless            | 256  | Sorted             |       359.2 ns |     73.19 ns |     38.28 ns |  0.47 |    0.06 |    3 |         - |          NA |
| Ipnsort                      | 256  | Sorted             |       128.4 ns |      3.35 ns |      1.19 ns |  0.17 |    0.01 |    1 |         - |          NA |
| StdSort                      | 256  | Sorted             |       483.4 ns |    194.28 ns |     69.28 ns |  0.64 |    0.09 |    4 |         - |          NA |
| BlockQuickSort               | 256  | Sorted             |     1,077.1 ns |     10.03 ns |      4.45 ns |  1.42 |    0.09 |    6 |         - |          NA |
| DotnetSort                   | 256  | Sorted             |       620.6 ns |      4.34 ns |      1.93 ns |  0.82 |    0.05 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Reversed**           |       **728.7 ns** |     **14.65 ns** |      **7.66 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 256  | Reversed           |       788.5 ns |     15.66 ns |      8.19 ns |  1.08 |    0.02 |    3 |         - |          NA |
| QuickSortMedian3             | 256  | Reversed           |       979.5 ns |     17.91 ns |      7.95 ns |  1.34 |    0.02 |    3 |         - |          NA |
| QuickSortMedian9             | 256  | Reversed           |     1,124.2 ns |     12.59 ns |      5.59 ns |  1.54 |    0.02 |    3 |         - |          NA |
| DualPivotQuickSort           | 256  | Reversed           |       846.5 ns |    145.20 ns |     75.94 ns |  1.16 |    0.10 |    3 |         - |          NA |
| StableQuickSort              | 256  | Reversed           |     7,323.1 ns |    244.97 ns |    128.12 ns | 10.05 |    0.19 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Reversed           |     3,739.7 ns |    324.35 ns |    169.64 ns |  5.13 |    0.23 |    4 |         - |          NA |
| DestswapStableQuickSort      | 256  | Reversed           |     5,830.6 ns |     44.28 ns |     15.79 ns |  8.00 |    0.08 |    5 |         - |          NA |
| IntroSort                    | 256  | Reversed           |       554.1 ns |    131.68 ns |     68.87 ns |  0.76 |    0.09 |    3 |         - |          NA |
| IntroSortDotnet              | 256  | Reversed           |     1,148.7 ns |    111.21 ns |     49.38 ns |  1.58 |    0.07 |    3 |         - |          NA |
| PDQSort                      | 256  | Reversed           |       399.0 ns |      2.57 ns |      1.14 ns |  0.55 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 256  | Reversed           |       664.1 ns |      3.75 ns |      1.96 ns |  0.91 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 256  | Reversed           |       248.8 ns |     75.24 ns |     39.35 ns |  0.34 |    0.05 |    1 |         - |          NA |
| StdSort                      | 256  | Reversed           |       561.9 ns |      3.79 ns |      1.35 ns |  0.77 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 256  | Reversed           |     1,293.8 ns |     15.72 ns |      6.98 ns |  1.78 |    0.02 |    3 |         - |          NA |
| DotnetSort                   | 256  | Reversed           |       949.5 ns |     39.53 ns |     20.67 ns |  1.30 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **PipeOrgan**          |     **5,648.4 ns** |    **219.40 ns** |    **114.75 ns** |  **1.00** |    **0.03** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 256  | PipeOrgan          |     2,123.0 ns |     64.90 ns |     33.94 ns |  0.38 |    0.01 |    1 |         - |          NA |
| QuickSortMedian3             | 256  | PipeOrgan          |     2,449.8 ns |    336.18 ns |    175.83 ns |  0.43 |    0.03 |    1 |         - |          NA |
| QuickSortMedian9             | 256  | PipeOrgan          |     2,047.9 ns |     23.32 ns |      8.32 ns |  0.36 |    0.01 |    1 |         - |          NA |
| DualPivotQuickSort           | 256  | PipeOrgan          |     1,203.8 ns |     28.39 ns |     12.60 ns |  0.21 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 256  | PipeOrgan          |     7,169.7 ns |    285.34 ns |    149.24 ns |  1.27 |    0.03 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 256  | PipeOrgan          |     3,584.8 ns |     27.13 ns |      9.67 ns |  0.63 |    0.01 |    2 |         - |          NA |
| DestswapStableQuickSort      | 256  | PipeOrgan          |     6,543.7 ns |    283.41 ns |    148.23 ns |  1.16 |    0.03 |    3 |         - |          NA |
| IntroSort                    | 256  | PipeOrgan          |     1,314.0 ns |     84.45 ns |     37.50 ns |  0.23 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | PipeOrgan          |     1,704.2 ns |    266.04 ns |    118.13 ns |  0.30 |    0.02 |    1 |         - |          NA |
| PDQSort                      | 256  | PipeOrgan          |     1,402.4 ns |     95.33 ns |     49.86 ns |  0.25 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | PipeOrgan          |     2,343.6 ns |    295.00 ns |    154.29 ns |  0.42 |    0.03 |    1 |         - |          NA |
| Ipnsort                      | 256  | PipeOrgan          |     3,099.3 ns |    148.19 ns |     65.80 ns |  0.55 |    0.02 |    2 |         - |          NA |
| StdSort                      | 256  | PipeOrgan          |     3,569.6 ns |     88.45 ns |     39.27 ns |  0.63 |    0.01 |    2 |         - |          NA |
| BlockQuickSort               | 256  | PipeOrgan          |     3,862.5 ns |    229.19 ns |    119.87 ns |  0.68 |    0.02 |    2 |         - |          NA |
| DotnetSort                   | 256  | PipeOrgan          |     1,919.8 ns |    155.40 ns |     81.28 ns |  0.34 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Random**             |    **10,649.1 ns** |    **278.00 ns** |    **123.43 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Random             |    10,437.4 ns |    254.02 ns |    132.86 ns |  0.98 |    0.02 |    1 |         - |          NA |
| QuickSortMedian3             | 1024 | Random             |    10,218.6 ns |    302.08 ns |    134.12 ns |  0.96 |    0.02 |    1 |         - |          NA |
| QuickSortMedian9             | 1024 | Random             |    11,600.7 ns |    224.47 ns |     99.67 ns |  1.09 |    0.01 |    1 |         - |          NA |
| DualPivotQuickSort           | 1024 | Random             |     8,472.5 ns |    260.98 ns |    115.88 ns |  0.80 |    0.01 |    1 |         - |          NA |
| StableQuickSort              | 1024 | Random             |    45,188.0 ns |    293.77 ns |    130.43 ns |  4.24 |    0.05 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Random             |    27,386.0 ns |    414.28 ns |    216.68 ns |  2.57 |    0.03 |    3 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Random             |    27,878.8 ns |    677.73 ns |    354.47 ns |  2.62 |    0.04 |    3 |         - |          NA |
| IntroSort                    | 1024 | Random             |     9,424.5 ns |    274.65 ns |    143.65 ns |  0.89 |    0.02 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | Random             |     7,706.8 ns |    256.60 ns |    134.21 ns |  0.72 |    0.01 |    1 |         - |          NA |
| PDQSort                      | 1024 | Random             |     7,255.8 ns |    283.50 ns |    148.27 ns |  0.68 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | Random             |    10,293.9 ns |    219.01 ns |    114.54 ns |  0.97 |    0.01 |    1 |         - |          NA |
| Ipnsort                      | 1024 | Random             |    14,941.1 ns |     98.59 ns |     51.57 ns |  1.40 |    0.02 |    2 |         - |          NA |
| StdSort                      | 1024 | Random             |    10,597.4 ns |    120.67 ns |     63.12 ns |  1.00 |    0.01 |    1 |         - |          NA |
| BlockQuickSort               | 1024 | Random             |    10,948.8 ns |    236.21 ns |    104.88 ns |  1.03 |    0.01 |    1 |         - |          NA |
| DotnetSort                   | 1024 | Random             |     8,505.3 ns |    242.36 ns |    126.76 ns |  0.80 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **SingleElementMoved** |     **4,110.7 ns** |    **139.07 ns** |     **61.75 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 1024 | SingleElementMoved |     4,215.6 ns |    168.61 ns |     88.18 ns |  1.03 |    0.02 |    2 |         - |          NA |
| QuickSortMedian3             | 1024 | SingleElementMoved |     6,178.7 ns |    189.24 ns |     98.98 ns |  1.50 |    0.03 |    3 |         - |          NA |
| QuickSortMedian9             | 1024 | SingleElementMoved |     8,260.8 ns |    281.57 ns |    147.26 ns |  2.01 |    0.04 |    3 |         - |          NA |
| DualPivotQuickSort           | 1024 | SingleElementMoved |     3,266.4 ns |    280.21 ns |    146.55 ns |  0.79 |    0.04 |    1 |         - |          NA |
| StableQuickSort              | 1024 | SingleElementMoved |    36,362.0 ns |    294.93 ns |    154.25 ns |  8.85 |    0.13 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | SingleElementMoved |    17,801.6 ns |    832.66 ns |    435.50 ns |  4.33 |    0.12 |    4 |         - |          NA |
| DestswapStableQuickSort      | 1024 | SingleElementMoved |    16,385.6 ns |    163.24 ns |     85.38 ns |  3.99 |    0.06 |    4 |         - |          NA |
| IntroSort                    | 1024 | SingleElementMoved |     3,188.9 ns |     19.36 ns |      6.91 ns |  0.78 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | SingleElementMoved |     4,405.8 ns |     50.63 ns |     18.06 ns |  1.07 |    0.02 |    2 |         - |          NA |
| PDQSort                      | 1024 | SingleElementMoved |     3,958.6 ns |    203.57 ns |     90.39 ns |  0.96 |    0.02 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | SingleElementMoved |     4,821.3 ns |    306.84 ns |    160.48 ns |  1.17 |    0.04 |    2 |         - |          NA |
| Ipnsort                      | 1024 | SingleElementMoved |    13,922.3 ns |    233.54 ns |    103.69 ns |  3.39 |    0.05 |    4 |         - |          NA |
| StdSort                      | 1024 | SingleElementMoved |     5,823.7 ns |     58.95 ns |     21.02 ns |  1.42 |    0.02 |    3 |         - |          NA |
| BlockQuickSort               | 1024 | SingleElementMoved |     7,206.9 ns |    339.82 ns |    177.73 ns |  1.75 |    0.05 |    3 |         - |          NA |
| DotnetSort                   | 1024 | SingleElementMoved |     4,319.2 ns |    488.28 ns |    255.38 ns |  1.05 |    0.06 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Sorted**             |     **3,132.3 ns** |     **19.87 ns** |      **8.82 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Sorted             |     3,038.9 ns |    143.74 ns |     63.82 ns |  0.97 |    0.02 |    4 |         - |          NA |
| QuickSortMedian3             | 1024 | Sorted             |     4,250.8 ns |     55.04 ns |     19.63 ns |  1.36 |    0.01 |    5 |         - |          NA |
| QuickSortMedian9             | 1024 | Sorted             |     4,714.5 ns |    239.74 ns |    125.39 ns |  1.51 |    0.04 |    5 |         - |          NA |
| DualPivotQuickSort           | 1024 | Sorted             |     3,135.5 ns |    411.02 ns |    214.97 ns |  1.00 |    0.06 |    4 |         - |          NA |
| StableQuickSort              | 1024 | Sorted             |    34,022.6 ns |  1,547.73 ns |    809.50 ns | 10.86 |    0.25 |    9 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Sorted             |    19,668.4 ns |    529.69 ns |    188.89 ns |  6.28 |    0.06 |    8 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Sorted             |    15,860.0 ns |    822.62 ns |    430.25 ns |  5.06 |    0.13 |    7 |         - |          NA |
| IntroSort                    | 1024 | Sorted             |       960.6 ns |      3.84 ns |      1.37 ns |  0.31 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet              | 1024 | Sorted             |     3,750.8 ns |    270.23 ns |    141.34 ns |  1.20 |    0.04 |    5 |         - |          NA |
| PDQSort                      | 1024 | Sorted             |     1,162.9 ns |      6.94 ns |      2.48 ns |  0.37 |    0.00 |    3 |         - |          NA |
| PDQSortBranchless            | 1024 | Sorted             |     1,163.3 ns |      7.56 ns |      2.69 ns |  0.37 |    0.00 |    3 |         - |          NA |
| Ipnsort                      | 1024 | Sorted             |       457.5 ns |     15.33 ns |      5.47 ns |  0.15 |    0.00 |    1 |         - |          NA |
| StdSort                      | 1024 | Sorted             |     1,224.0 ns |     26.60 ns |     11.81 ns |  0.39 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 1024 | Sorted             |     5,675.4 ns |     65.49 ns |     23.35 ns |  1.81 |    0.01 |    6 |         - |          NA |
| DotnetSort                   | 1024 | Sorted             |     3,079.5 ns |     30.13 ns |     10.74 ns |  0.98 |    0.00 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Reversed**           |     **3,451.6 ns** |     **42.19 ns** |     **15.04 ns** |  **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Reversed           |     4,031.9 ns |    658.09 ns |    344.19 ns |  1.17 |    0.09 |    4 |         - |          NA |
| QuickSortMedian3             | 1024 | Reversed           |     4,553.1 ns |     42.01 ns |     18.65 ns |  1.32 |    0.01 |    4 |         - |          NA |
| QuickSortMedian9             | 1024 | Reversed           |     5,208.8 ns |  1,419.28 ns |    630.17 ns |  1.51 |    0.17 |    4 |         - |          NA |
| DualPivotQuickSort           | 1024 | Reversed           |     3,421.3 ns |    200.66 ns |     89.10 ns |  0.99 |    0.02 |    4 |         - |          NA |
| StableQuickSort              | 1024 | Reversed           |    36,218.3 ns |    126.84 ns |     56.32 ns | 10.49 |    0.05 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Reversed           |    17,651.1 ns |    300.92 ns |    133.61 ns |  5.11 |    0.04 |    5 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Reversed           |    26,703.4 ns |    361.66 ns |    189.16 ns |  7.74 |    0.06 |    6 |         - |          NA |
| IntroSort                    | 1024 | Reversed           |     2,432.3 ns |     38.69 ns |     17.18 ns |  0.70 |    0.01 |    3 |         - |          NA |
| IntroSortDotnet              | 1024 | Reversed           |     5,451.7 ns |     26.88 ns |      9.59 ns |  1.58 |    0.01 |    4 |         - |          NA |
| PDQSort                      | 1024 | Reversed           |     1,476.5 ns |      5.23 ns |      1.86 ns |  0.43 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | Reversed           |     2,404.9 ns |     27.83 ns |     12.36 ns |  0.70 |    0.00 |    3 |         - |          NA |
| Ipnsort                      | 1024 | Reversed           |       705.8 ns |      2.32 ns |      1.03 ns |  0.20 |    0.00 |    1 |         - |          NA |
| StdSort                      | 1024 | Reversed           |     2,122.6 ns |      9.23 ns |      4.10 ns |  0.61 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 1024 | Reversed           |     6,506.9 ns |    239.05 ns |    125.03 ns |  1.89 |    0.04 |    4 |         - |          NA |
| DotnetSort                   | 1024 | Reversed           |     6,123.5 ns |  1,493.84 ns |    781.31 ns |  1.77 |    0.21 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **PipeOrgan**          |    **83,779.3 ns** |    **482.77 ns** |    **252.50 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 1024 | PipeOrgan          |    12,521.9 ns |    823.31 ns |    430.61 ns |  0.15 |    0.00 |    2 |         - |          NA |
| QuickSortMedian3             | 1024 | PipeOrgan          |    11,968.1 ns |    160.21 ns |     71.14 ns |  0.14 |    0.00 |    2 |         - |          NA |
| QuickSortMedian9             | 1024 | PipeOrgan          |     9,969.2 ns |    199.00 ns |    104.08 ns |  0.12 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort           | 1024 | PipeOrgan          |     6,153.2 ns |    267.13 ns |    139.72 ns |  0.07 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 1024 | PipeOrgan          |    36,312.5 ns |    258.93 ns |    135.43 ns |  0.43 |    0.00 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | PipeOrgan          |    18,186.6 ns |    834.93 ns |    436.68 ns |  0.22 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 1024 | PipeOrgan          |    31,683.9 ns |    183.49 ns |     81.47 ns |  0.38 |    0.00 |    4 |         - |          NA |
| IntroSort                    | 1024 | PipeOrgan          |     8,831.4 ns |    345.11 ns |    123.07 ns |  0.11 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet              | 1024 | PipeOrgan          |    11,010.4 ns |    215.14 ns |     95.52 ns |  0.13 |    0.00 |    2 |         - |          NA |
| PDQSort                      | 1024 | PipeOrgan          |     6,791.2 ns |    238.93 ns |    124.96 ns |  0.08 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | PipeOrgan          |    12,032.7 ns |    358.95 ns |    187.74 ns |  0.14 |    0.00 |    2 |         - |          NA |
| Ipnsort                      | 1024 | PipeOrgan          |    16,316.2 ns |    138.55 ns |     72.46 ns |  0.19 |    0.00 |    3 |         - |          NA |
| StdSort                      | 1024 | PipeOrgan          |    16,675.8 ns |    135.62 ns |     60.22 ns |  0.20 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 1024 | PipeOrgan          |    19,372.2 ns |    506.18 ns |    224.75 ns |  0.23 |    0.00 |    3 |         - |          NA |
| DotnetSort                   | 1024 | PipeOrgan          |    12,747.6 ns |    872.23 ns |    387.28 ns |  0.15 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Random**             |    **49,237.5 ns** |  **1,690.29 ns** |    **750.50 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Random             |    61,451.0 ns |  8,280.86 ns |  4,331.05 ns |  1.25 |    0.08 |    1 |         - |          NA |
| QuickSortMedian3             | 4096 | Random             |    53,435.9 ns |  7,749.63 ns |  4,053.21 ns |  1.09 |    0.08 |    1 |         - |          NA |
| QuickSortMedian9             | 4096 | Random             |    56,170.4 ns |  4,477.57 ns |  2,341.85 ns |  1.14 |    0.05 |    1 |         - |          NA |
| DualPivotQuickSort           | 4096 | Random             |    41,599.0 ns |  1,721.90 ns |    764.53 ns |  0.85 |    0.02 |    1 |         - |          NA |
| StableQuickSort              | 4096 | Random             |   441,152.0 ns |  2,381.79 ns |  1,057.53 ns |  8.96 |    0.13 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Random             |   335,502.2 ns |  3,142.96 ns |  1,395.49 ns |  6.82 |    0.10 |    3 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Random             |   204,917.3 ns | 15,307.24 ns |  8,005.98 ns |  4.16 |    0.16 |    2 |         - |          NA |
| IntroSort                    | 4096 | Random             |    48,266.4 ns |  1,501.83 ns |    666.82 ns |  0.98 |    0.02 |    1 |         - |          NA |
| IntroSortDotnet              | 4096 | Random             |    37,182.9 ns |    636.53 ns |    282.62 ns |  0.76 |    0.01 |    1 |         - |          NA |
| PDQSort                      | 4096 | Random             |    35,545.0 ns |  1,388.10 ns |    726.00 ns |  0.72 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless            | 4096 | Random             |    47,908.6 ns |    324.93 ns |    115.87 ns |  0.97 |    0.01 |    1 |         - |          NA |
| Ipnsort                      | 4096 | Random             |    75,923.2 ns |    461.59 ns |    204.95 ns |  1.54 |    0.02 |    1 |         - |          NA |
| StdSort                      | 4096 | Random             |    49,362.2 ns |  1,861.59 ns |    973.65 ns |  1.00 |    0.02 |    1 |         - |          NA |
| BlockQuickSort               | 4096 | Random             |    53,154.1 ns |    320.74 ns |    142.41 ns |  1.08 |    0.02 |    1 |         - |          NA |
| DotnetSort                   | 4096 | Random             |    41,313.8 ns |    906.21 ns |    402.36 ns |  0.84 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **SingleElementMoved** |    **19,632.9 ns** |  **1,102.10 ns** |    **576.42 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 4096 | SingleElementMoved |    21,001.1 ns |  1,333.14 ns |    591.92 ns |  1.07 |    0.04 |    1 |         - |          NA |
| QuickSortMedian3             | 4096 | SingleElementMoved |    27,370.3 ns |    910.07 ns |    404.08 ns |  1.40 |    0.04 |    1 |         - |          NA |
| QuickSortMedian9             | 4096 | SingleElementMoved |    36,365.6 ns |    896.06 ns |    468.66 ns |  1.85 |    0.06 |    2 |         - |          NA |
| DualPivotQuickSort           | 4096 | SingleElementMoved |    16,615.6 ns |    794.64 ns |    352.82 ns |  0.85 |    0.03 |    1 |         - |          NA |
| StableQuickSort              | 4096 | SingleElementMoved |   175,342.9 ns |    587.60 ns |    307.33 ns |  8.94 |    0.25 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | SingleElementMoved |    84,614.7 ns |  2,617.07 ns |  1,368.78 ns |  4.31 |    0.14 |    3 |         - |          NA |
| DestswapStableQuickSort      | 4096 | SingleElementMoved |    76,974.6 ns |    937.50 ns |    490.33 ns |  3.92 |    0.11 |    3 |         - |          NA |
| IntroSort                    | 4096 | SingleElementMoved |    14,282.0 ns |  1,320.05 ns |    586.11 ns |  0.73 |    0.03 |    1 |         - |          NA |
| IntroSortDotnet              | 4096 | SingleElementMoved |    21,285.0 ns |     97.69 ns |     34.84 ns |  1.08 |    0.03 |    1 |         - |          NA |
| PDQSort                      | 4096 | SingleElementMoved |    16,403.4 ns |     98.60 ns |     43.78 ns |  0.84 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless            | 4096 | SingleElementMoved |    20,381.0 ns |     54.28 ns |     24.10 ns |  1.04 |    0.03 |    1 |         - |          NA |
| Ipnsort                      | 4096 | SingleElementMoved |    67,474.2 ns |    326.98 ns |    145.18 ns |  3.44 |    0.10 |    3 |         - |          NA |
| StdSort                      | 4096 | SingleElementMoved |    24,784.1 ns |    640.29 ns |    334.88 ns |  1.26 |    0.04 |    1 |         - |          NA |
| BlockQuickSort               | 4096 | SingleElementMoved |    33,757.0 ns |    589.30 ns |    261.65 ns |  1.72 |    0.05 |    2 |         - |          NA |
| DotnetSort                   | 4096 | SingleElementMoved |    20,967.7 ns |    974.43 ns |    432.65 ns |  1.07 |    0.04 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Sorted**             |    **15,234.1 ns** |    **465.39 ns** |    **206.63 ns** |  **1.00** |    **0.02** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Sorted             |    13,964.3 ns |    225.49 ns |    100.12 ns |  0.92 |    0.01 |    4 |         - |          NA |
| QuickSortMedian3             | 4096 | Sorted             |    19,651.0 ns |    358.55 ns |    187.53 ns |  1.29 |    0.02 |    4 |         - |          NA |
| QuickSortMedian9             | 4096 | Sorted             |    21,149.5 ns |    629.34 ns |    329.16 ns |  1.39 |    0.03 |    4 |         - |          NA |
| DualPivotQuickSort           | 4096 | Sorted             |    16,684.8 ns |    711.39 ns |    372.07 ns |  1.10 |    0.03 |    4 |         - |          NA |
| StableQuickSort              | 4096 | Sorted             |   160,818.9 ns |    820.68 ns |    364.39 ns | 10.56 |    0.13 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Sorted             |    92,870.3 ns |    906.32 ns |    474.02 ns |  6.10 |    0.08 |    7 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Sorted             |    74,479.2 ns |  1,045.98 ns |    547.07 ns |  4.89 |    0.07 |    6 |         - |          NA |
| IntroSort                    | 4096 | Sorted             |     3,710.9 ns |     44.61 ns |     23.33 ns |  0.24 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet              | 4096 | Sorted             |    17,349.6 ns |    244.69 ns |    108.64 ns |  1.14 |    0.02 |    4 |         - |          NA |
| PDQSort                      | 4096 | Sorted             |     4,513.8 ns |      7.94 ns |      4.15 ns |  0.30 |    0.00 |    3 |         - |          NA |
| PDQSortBranchless            | 4096 | Sorted             |     4,527.7 ns |     41.19 ns |     18.29 ns |  0.30 |    0.00 |    3 |         - |          NA |
| Ipnsort                      | 4096 | Sorted             |     1,762.6 ns |      9.36 ns |      3.34 ns |  0.12 |    0.00 |    1 |         - |          NA |
| StdSort                      | 4096 | Sorted             |     4,760.3 ns |    202.75 ns |     90.02 ns |  0.31 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 4096 | Sorted             |    28,229.3 ns |    513.08 ns |    227.81 ns |  1.85 |    0.03 |    5 |         - |          NA |
| DotnetSort                   | 4096 | Sorted             |    18,936.1 ns | 10,511.02 ns |  5,497.46 ns |  1.24 |    0.34 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Reversed**           |    **16,244.3 ns** |    **511.59 ns** |    **267.57 ns** |  **1.00** |    **0.02** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Reversed           |    19,217.4 ns |  3,142.42 ns |  1,643.55 ns |  1.18 |    0.10 |    5 |         - |          NA |
| QuickSortMedian3             | 4096 | Reversed           |    20,878.9 ns |    388.21 ns |    172.37 ns |  1.29 |    0.02 |    5 |         - |          NA |
| QuickSortMedian9             | 4096 | Reversed           |    22,728.0 ns |  1,254.40 ns |    656.08 ns |  1.40 |    0.04 |    5 |         - |          NA |
| DualPivotQuickSort           | 4096 | Reversed           |    19,618.7 ns |  1,380.38 ns |    721.97 ns |  1.21 |    0.05 |    5 |         - |          NA |
| StableQuickSort              | 4096 | Reversed           |   175,640.9 ns |  1,287.08 ns |    673.17 ns | 10.82 |    0.17 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Reversed           |    82,016.1 ns |  1,414.26 ns |    739.68 ns |  5.05 |    0.09 |    6 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Reversed           |   118,279.9 ns |    962.17 ns |    503.23 ns |  7.28 |    0.12 |    7 |         - |          NA |
| IntroSort                    | 4096 | Reversed           |     9,987.2 ns |    559.28 ns |    248.32 ns |  0.61 |    0.02 |    4 |         - |          NA |
| IntroSortDotnet              | 4096 | Reversed           |    26,857.8 ns |    350.51 ns |    155.63 ns |  1.65 |    0.03 |    5 |         - |          NA |
| PDQSort                      | 4096 | Reversed           |     5,810.9 ns |    255.49 ns |    113.44 ns |  0.36 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 4096 | Reversed           |     9,389.3 ns |    457.45 ns |    203.11 ns |  0.58 |    0.01 |    4 |         - |          NA |
| Ipnsort                      | 4096 | Reversed           |     2,874.7 ns |     15.33 ns |      8.02 ns |  0.18 |    0.00 |    1 |         - |          NA |
| StdSort                      | 4096 | Reversed           |     7,664.8 ns |    292.05 ns |    152.75 ns |  0.47 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 4096 | Reversed           |    31,143.8 ns |  1,610.80 ns |    842.48 ns |  1.92 |    0.06 |    5 |         - |          NA |
| DotnetSort                   | 4096 | Reversed           |    32,102.2 ns |  3,573.80 ns |  1,869.16 ns |  1.98 |    0.11 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **PipeOrgan**          | **1,223,835.6 ns** |  **3,344.55 ns** |  **1,749.26 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 4096 | PipeOrgan          |    69,791.6 ns |  3,588.76 ns |  1,593.43 ns |  0.06 |    0.00 |    3 |         - |          NA |
| QuickSortMedian3             | 4096 | PipeOrgan          |    60,506.4 ns |  1,541.03 ns |    684.23 ns |  0.05 |    0.00 |    3 |         - |          NA |
| QuickSortMedian9             | 4096 | PipeOrgan          |    44,136.8 ns |  1,946.82 ns |  1,018.23 ns |  0.04 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort           | 4096 | PipeOrgan          |    30,989.0 ns |  1,387.30 ns |    615.97 ns |  0.03 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 4096 | PipeOrgan          |   176,199.1 ns |    324.38 ns |    169.66 ns |  0.14 |    0.00 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | PipeOrgan          |    86,683.4 ns |  4,178.27 ns |  2,185.32 ns |  0.07 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 4096 | PipeOrgan          |   151,576.6 ns |    427.65 ns |    189.88 ns |  0.12 |    0.00 |    4 |         - |          NA |
| IntroSort                    | 4096 | PipeOrgan          |    62,099.6 ns |  4,777.41 ns |  2,498.68 ns |  0.05 |    0.00 |    3 |         - |          NA |
| IntroSortDotnet              | 4096 | PipeOrgan          |    64,548.5 ns |  1,715.12 ns |    897.04 ns |  0.05 |    0.00 |    3 |         - |          NA |
| PDQSort                      | 4096 | PipeOrgan          |    33,339.4 ns |  1,622.65 ns |    848.68 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 4096 | PipeOrgan          |    57,309.6 ns |  2,237.74 ns |    993.57 ns |  0.05 |    0.00 |    3 |         - |          NA |
| Ipnsort                      | 4096 | PipeOrgan          |    82,347.7 ns |    374.41 ns |    195.82 ns |  0.07 |    0.00 |    3 |         - |          NA |
| StdSort                      | 4096 | PipeOrgan          |    84,048.4 ns |  1,316.81 ns |    688.72 ns |  0.07 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 4096 | PipeOrgan          |    83,344.0 ns |    992.59 ns |    440.72 ns |  0.07 |    0.00 |    3 |         - |          NA |
| DotnetSort                   | 4096 | PipeOrgan          |    71,036.9 ns |  3,520.29 ns |  1,563.03 ns |  0.06 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Random**             |   **363,302.0 ns** |  **6,690.20 ns** |  **3,499.10 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Random             |   318,529.7 ns | 19,141.48 ns |  8,498.94 ns |  0.88 |    0.02 |    3 |         - |          NA |
| QuickSortMedian3             | 8192 | Random             |   360,625.8 ns |  2,920.52 ns |  1,296.73 ns |  0.99 |    0.01 |    3 |         - |          NA |
| QuickSortMedian9             | 8192 | Random             |   389,361.3 ns |  2,609.71 ns |  1,364.93 ns |  1.07 |    0.01 |    3 |         - |          NA |
| DualPivotQuickSort           | 8192 | Random             |   257,961.0 ns |  4,664.74 ns |  2,439.75 ns |  0.71 |    0.01 |    3 |         - |          NA |
| StableQuickSort              | 8192 | Random             | 1,016,798.3 ns |  3,083.38 ns |  1,612.67 ns |  2.80 |    0.03 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Random             |   806,776.4 ns |  1,924.78 ns |  1,006.70 ns |  2.22 |    0.02 |    4 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Random             |   682,885.1 ns | 35,102.96 ns | 15,585.93 ns |  1.88 |    0.04 |    4 |         - |          NA |
| IntroSort                    | 8192 | Random             |   305,582.8 ns |  2,064.93 ns |  1,080.00 ns |  0.84 |    0.01 |    3 |         - |          NA |
| IntroSortDotnet              | 8192 | Random             |   249,670.8 ns |  1,800.68 ns |    642.14 ns |  0.69 |    0.01 |    3 |         - |          NA |
| PDQSort                      | 8192 | Random             |   259,824.3 ns |  3,184.46 ns |  1,413.92 ns |  0.72 |    0.01 |    3 |         - |          NA |
| PDQSortBranchless            | 8192 | Random             |   107,417.9 ns |  1,341.72 ns |    701.74 ns |  0.30 |    0.00 |    1 |         - |          NA |
| Ipnsort                      | 8192 | Random             |   165,464.2 ns |    426.84 ns |    189.52 ns |  0.46 |    0.00 |    2 |         - |          NA |
| StdSort                      | 8192 | Random             |   103,819.1 ns |    939.84 ns |    417.29 ns |  0.29 |    0.00 |    1 |         - |          NA |
| BlockQuickSort               | 8192 | Random             |   118,215.3 ns |  8,788.03 ns |  3,901.94 ns |  0.33 |    0.01 |    1 |         - |          NA |
| DotnetSort                   | 8192 | Random             |   258,544.8 ns |  7,226.69 ns |  3,779.70 ns |  0.71 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **SingleElementMoved** |    **40,266.9 ns** |  **1,546.23 ns** |    **686.54 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 8192 | SingleElementMoved |    45,027.9 ns |  1,290.80 ns |    573.12 ns |  1.12 |    0.02 |    1 |         - |          NA |
| QuickSortMedian3             | 8192 | SingleElementMoved |    57,854.5 ns |  2,229.92 ns |  1,166.29 ns |  1.44 |    0.04 |    1 |         - |          NA |
| QuickSortMedian9             | 8192 | SingleElementMoved |    75,334.9 ns |  1,144.17 ns |    598.43 ns |  1.87 |    0.03 |    2 |         - |          NA |
| DualPivotQuickSort           | 8192 | SingleElementMoved |    37,091.5 ns |  2,038.77 ns |  1,066.32 ns |  0.92 |    0.03 |    1 |         - |          NA |
| StableQuickSort              | 8192 | SingleElementMoved |   381,646.0 ns |  1,274.88 ns |    566.05 ns |  9.48 |    0.15 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | SingleElementMoved |   180,505.6 ns |  3,534.19 ns |  1,848.45 ns |  4.48 |    0.08 |    3 |         - |          NA |
| DestswapStableQuickSort      | 8192 | SingleElementMoved |   163,161.5 ns |  2,605.20 ns |  1,362.57 ns |  4.05 |    0.07 |    3 |         - |          NA |
| IntroSort                    | 8192 | SingleElementMoved |    31,227.9 ns |  2,315.85 ns |  1,211.23 ns |  0.78 |    0.03 |    1 |         - |          NA |
| IntroSortDotnet              | 8192 | SingleElementMoved |    46,805.6 ns |    516.64 ns |    229.39 ns |  1.16 |    0.02 |    1 |         - |          NA |
| PDQSort                      | 8192 | SingleElementMoved |    33,755.8 ns |    412.85 ns |    147.23 ns |  0.84 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 8192 | SingleElementMoved |    42,135.4 ns |  1,023.43 ns |    535.27 ns |  1.05 |    0.02 |    1 |         - |          NA |
| Ipnsort                      | 8192 | SingleElementMoved |   148,902.8 ns |    555.30 ns |    246.56 ns |  3.70 |    0.06 |    3 |         - |          NA |
| StdSort                      | 8192 | SingleElementMoved |    48,675.7 ns |    578.60 ns |    302.62 ns |  1.21 |    0.02 |    1 |         - |          NA |
| BlockQuickSort               | 8192 | SingleElementMoved |    72,714.9 ns |    756.89 ns |    395.87 ns |  1.81 |    0.03 |    2 |         - |          NA |
| DotnetSort                   | 8192 | SingleElementMoved |    45,742.5 ns |  2,351.17 ns |  1,229.71 ns |  1.14 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Sorted**             |    **33,373.0 ns** |  **2,760.01 ns** |  **1,225.46 ns** |  **1.00** |    **0.05** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Sorted             |    30,991.1 ns |  1,828.70 ns |    811.96 ns |  0.93 |    0.04 |    3 |         - |          NA |
| QuickSortMedian3             | 8192 | Sorted             |    41,597.7 ns |    930.83 ns |    486.84 ns |  1.25 |    0.04 |    3 |         - |          NA |
| QuickSortMedian9             | 8192 | Sorted             |    44,330.7 ns |  1,261.83 ns |    560.26 ns |  1.33 |    0.05 |    3 |         - |          NA |
| DualPivotQuickSort           | 8192 | Sorted             |    37,336.5 ns |    709.80 ns |    371.24 ns |  1.12 |    0.04 |    3 |         - |          NA |
| StableQuickSort              | 8192 | Sorted             |   349,347.6 ns |  1,906.30 ns |    846.41 ns | 10.48 |    0.34 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Sorted             |   203,966.6 ns |  2,244.45 ns |    996.55 ns |  6.12 |    0.20 |    6 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Sorted             |   157,392.7 ns |  1,769.58 ns |    925.52 ns |  4.72 |    0.16 |    5 |         - |          NA |
| IntroSort                    | 8192 | Sorted             |     7,817.9 ns |    874.39 ns |    388.24 ns |  0.23 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 8192 | Sorted             |    37,021.0 ns |    562.24 ns |    249.64 ns |  1.11 |    0.04 |    3 |         - |          NA |
| PDQSort                      | 8192 | Sorted             |     9,094.9 ns |    198.66 ns |    103.90 ns |  0.27 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | Sorted             |     9,223.8 ns |    329.35 ns |    172.26 ns |  0.28 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 8192 | Sorted             |     3,679.5 ns |     50.32 ns |     17.94 ns |  0.11 |    0.00 |    1 |         - |          NA |
| StdSort                      | 8192 | Sorted             |     9,330.7 ns |  1,030.87 ns |    457.71 ns |  0.28 |    0.02 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | Sorted             |    61,091.9 ns |    952.07 ns |    497.95 ns |  1.83 |    0.06 |    4 |         - |          NA |
| DotnetSort                   | 8192 | Sorted             |    36,469.5 ns |  6,501.61 ns |  3,400.47 ns |  1.09 |    0.10 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Reversed**           |    **35,392.2 ns** |  **1,174.48 ns** |    **614.28 ns** |  **1.00** |    **0.02** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Reversed           |    40,580.8 ns |  5,264.43 ns |  2,753.40 ns |  1.15 |    0.08 |    5 |         - |          NA |
| QuickSortMedian3             | 8192 | Reversed           |    43,931.0 ns |    685.09 ns |    304.18 ns |  1.24 |    0.02 |    5 |         - |          NA |
| QuickSortMedian9             | 8192 | Reversed           |    47,257.7 ns |    440.61 ns |    230.45 ns |  1.34 |    0.02 |    5 |         - |          NA |
| DualPivotQuickSort           | 8192 | Reversed           |    40,141.0 ns |  1,482.87 ns |    658.40 ns |  1.13 |    0.03 |    5 |         - |          NA |
| StableQuickSort              | 8192 | Reversed           |   381,020.5 ns |    771.31 ns |    342.47 ns | 10.77 |    0.18 |    9 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Reversed           |   177,446.1 ns |  2,651.27 ns |  1,386.67 ns |  5.02 |    0.09 |    7 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Reversed           |   249,469.7 ns |  1,144.60 ns |    598.65 ns |  7.05 |    0.12 |    8 |         - |          NA |
| IntroSort                    | 8192 | Reversed           |    20,982.7 ns |  4,319.54 ns |  2,259.20 ns |  0.59 |    0.06 |    4 |         - |          NA |
| IntroSortDotnet              | 8192 | Reversed           |    58,819.6 ns |  1,638.37 ns |    856.90 ns |  1.66 |    0.04 |    6 |         - |          NA |
| PDQSort                      | 8192 | Reversed           |    11,506.7 ns |    284.93 ns |    149.02 ns |  0.33 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | Reversed           |    18,589.8 ns |    236.01 ns |    123.44 ns |  0.53 |    0.01 |    4 |         - |          NA |
| Ipnsort                      | 8192 | Reversed           |     5,800.0 ns |  1,151.83 ns |    511.42 ns |  0.16 |    0.01 |    1 |         - |          NA |
| StdSort                      | 8192 | Reversed           |    15,096.2 ns |    185.58 ns |     66.18 ns |  0.43 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 8192 | Reversed           |    66,176.8 ns |    497.33 ns |    260.11 ns |  1.87 |    0.03 |    6 |         - |          NA |
| DotnetSort                   | 8192 | Reversed           |    71,093.9 ns |  5,859.81 ns |  3,064.79 ns |  2.01 |    0.09 |    6 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **PipeOrgan**          | **4,758,662.7 ns** |  **9,059.10 ns** |  **4,022.30 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 8192 | PipeOrgan          |   170,721.0 ns |  2,686.70 ns |  1,192.91 ns |  0.04 |    0.00 |    3 |         - |          NA |
| QuickSortMedian3             | 8192 | PipeOrgan          |   146,693.9 ns |  5,921.22 ns |  3,096.91 ns |  0.03 |    0.00 |    3 |         - |          NA |
| QuickSortMedian9             | 8192 | PipeOrgan          |    93,192.2 ns |  2,250.87 ns |    999.40 ns |  0.02 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort           | 8192 | PipeOrgan          |    66,195.8 ns |  1,736.10 ns |    908.02 ns |  0.01 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 8192 | PipeOrgan          |   385,124.8 ns |  4,975.57 ns |  2,602.32 ns |  0.08 |    0.00 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | PipeOrgan          |   183,125.1 ns |  3,118.52 ns |  1,384.64 ns |  0.04 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 8192 | PipeOrgan          |   327,948.4 ns |    823.93 ns |    430.93 ns |  0.07 |    0.00 |    4 |         - |          NA |
| IntroSort                    | 8192 | PipeOrgan          |   157,846.7 ns | 10,707.40 ns |  5,600.18 ns |  0.03 |    0.00 |    3 |         - |          NA |
| IntroSortDotnet              | 8192 | PipeOrgan          |   272,150.5 ns |  6,011.80 ns |  2,669.28 ns |  0.06 |    0.00 |    4 |         - |          NA |
| PDQSort                      | 8192 | PipeOrgan          |    71,860.8 ns |  1,145.14 ns |    598.93 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 8192 | PipeOrgan          |   126,784.5 ns |  1,040.80 ns |    544.36 ns |  0.03 |    0.00 |    3 |         - |          NA |
| Ipnsort                      | 8192 | PipeOrgan          |   183,309.1 ns |    828.47 ns |    433.31 ns |  0.04 |    0.00 |    3 |         - |          NA |
| StdSort                      | 8192 | PipeOrgan          |   213,144.3 ns |  3,661.89 ns |  1,625.90 ns |  0.04 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 8192 | PipeOrgan          |   176,649.3 ns |    813.20 ns |    425.32 ns |  0.04 |    0.00 |    3 |         - |          NA |
| DotnetSort                   | 8192 | PipeOrgan          |   278,874.4 ns | 13,545.08 ns |  7,084.34 ns |  0.06 |    0.00 |    4 |         - |          NA |

### SelectionBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method              | Size | Pattern            | Mean           | Error        | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------------- |----- |------------------- |---------------:|-------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **SelectionSort**       | **256**  | **Random**             |    **19,041.0 ns** |    **278.56 ns** |   **145.69 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Random             |    14,351.4 ns |    182.36 ns |    95.38 ns |  0.75 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | Random             |    56,468.2 ns |    780.82 ns |   346.69 ns |  2.97 |    0.03 |    4 |         - |          NA |
| PancakeSort         | 256  | Random             |    31,818.5 ns |    301.56 ns |   133.89 ns |  1.67 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **SelectionSort**       | **256**  | **SingleElementMoved** |    **19,217.1 ns** |    **280.59 ns** |   **146.75 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | SingleElementMoved |    14,503.3 ns |     53.31 ns |    23.67 ns |  0.75 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | SingleElementMoved |    35,191.0 ns |  1,995.68 ns | 1,043.78 ns |  1.83 |    0.05 |    3 |         - |          NA |
| PancakeSort         | 256  | SingleElementMoved |    15,873.8 ns |    215.74 ns |   112.84 ns |  0.83 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **SelectionSort**       | **256**  | **Sorted**             |    **19,164.4 ns** |     **56.88 ns** |    **20.28 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Sorted             |    14,150.2 ns |    100.67 ns |    44.70 ns |  0.74 |    0.00 |    1 |         - |          NA |
| CycleSort           | 256  | Sorted             |    27,986.0 ns |    214.76 ns |   112.32 ns |  1.46 |    0.01 |    3 |         - |          NA |
| PancakeSort         | 256  | Sorted             |    13,015.7 ns |    205.98 ns |   107.73 ns |  0.68 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **SelectionSort**       | **256**  | **Reversed**           |    **17,306.4 ns** |  **1,189.72 ns** |   **528.24 ns** |  **1.00** |    **0.04** |    **3** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Reversed           |     9,827.6 ns |    148.60 ns |    77.72 ns |  0.57 |    0.02 |    1 |         - |          NA |
| CycleSort           | 256  | Reversed           |    29,775.7 ns |    222.46 ns |    98.77 ns |  1.72 |    0.05 |    4 |         - |          NA |
| PancakeSort         | 256  | Reversed           |    13,202.6 ns |    216.57 ns |   113.27 ns |  0.76 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **SelectionSort**       | **256**  | **PipeOrgan**          |    **18,169.4 ns** |    **637.12 ns** |   **333.23 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | PipeOrgan          |    12,531.5 ns |    330.15 ns |   146.59 ns |  0.69 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | PipeOrgan          |    45,883.3 ns |  1,902.49 ns |   844.72 ns |  2.53 |    0.06 |    4 |         - |          NA |
| PancakeSort         | 256  | PipeOrgan          |    28,583.9 ns |    274.56 ns |   121.90 ns |  1.57 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Random**             |   **314,160.5 ns** |    **530.64 ns** |   **235.61 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Random             |   233,829.1 ns |    629.77 ns |   279.62 ns |  0.74 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | Random             | 1,199,420.0 ns |  8,685.84 ns | 4,542.86 ns |  3.82 |    0.01 |    4 |         - |          NA |
| PancakeSort         | 1024 | Random             |   534,769.8 ns |  1,477.84 ns |   656.17 ns |  1.70 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **SelectionSort**       | **1024** | **SingleElementMoved** |   **291,814.2 ns** |    **683.92 ns** |   **303.67 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | SingleElementMoved |   220,661.0 ns |  1,199.24 ns |   627.23 ns |  0.76 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | SingleElementMoved |   527,435.2 ns |  5,128.87 ns | 2,682.50 ns |  1.81 |    0.01 |    3 |         - |          NA |
| PancakeSort         | 1024 | SingleElementMoved |   267,363.2 ns | 12,053.92 ns | 6,304.43 ns |  0.92 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Sorted**             |   **291,571.9 ns** |  **1,139.18 ns** |   **595.81 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Sorted             |   218,263.1 ns |  2,171.17 ns | 1,135.57 ns |  0.75 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | Sorted             |   434,009.5 ns |  2,267.77 ns | 1,006.90 ns |  1.49 |    0.00 |    3 |         - |          NA |
| PancakeSort         | 1024 | Sorted             |   196,415.8 ns |    527.30 ns |   275.79 ns |  0.67 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Reversed**           |   **266,403.4 ns** | **17,348.37 ns** | **9,073.53 ns** |  **1.00** |    **0.05** |    **3** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Reversed           |   146,722.0 ns |  1,043.18 ns |   463.18 ns |  0.55 |    0.02 |    1 |         - |          NA |
| CycleSort           | 1024 | Reversed           |   444,621.2 ns |  4,259.00 ns | 2,227.54 ns |  1.67 |    0.05 |    4 |         - |          NA |
| PancakeSort         | 1024 | Reversed           |   197,253.2 ns |  1,142.40 ns |   407.39 ns |  0.74 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **SelectionSort**       | **1024** | **PipeOrgan**          |   **302,013.9 ns** |  **2,190.84 ns** | **1,145.85 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | PipeOrgan          |   216,566.5 ns |  7,648.10 ns | 4,000.10 ns |  0.72 |    0.01 |    1 |         - |          NA |
| CycleSort           | 1024 | PipeOrgan          |   786,099.6 ns | 10,058.72 ns | 5,260.91 ns |  2.60 |    0.02 |    4 |         - |          NA |
| PancakeSort         | 1024 | PipeOrgan          |   438,312.8 ns |  1,430.65 ns |   635.22 ns |  1.45 |    0.01 |    3 |         - |          NA |

### TreeBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                 | Size | Pattern            | Mean           | Error       | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------- |----- |------------------- |---------------:|------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **BalancedBinaryTreeSort** | **256**  | **Random**             |    **10,917.6 ns** |   **826.47 ns** |   **432.26 ns** |  **3.90** |    **0.18** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 256  | Random             |     2,803.1 ns |   153.78 ns |    80.43 ns |  1.00 |    0.04 |    1 |         - |          NA |
| SplaySort              | 256  | Random             |    20,459.9 ns |   514.98 ns |   269.34 ns |  7.30 |    0.21 |    4 |         - |          NA |
| TreapSort              | 256  | Random             |     6,939.2 ns |   519.24 ns |   271.57 ns |  2.48 |    0.11 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **SingleElementMoved** |    **10,934.1 ns** |   **204.36 ns** |    **90.74 ns** |  **0.25** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 256  | SingleElementMoved |    43,129.8 ns |   191.06 ns |    84.83 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 256  | SingleElementMoved |     3,398.4 ns |    14.48 ns |     5.16 ns |  0.08 |    0.00 |    1 |         - |          NA |
| TreapSort              | 256  | SingleElementMoved |     4,652.2 ns |   152.44 ns |    67.68 ns |  0.11 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Sorted**             |    **10,549.3 ns** |   **616.25 ns** |   **322.31 ns** |  **0.16** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 256  | Sorted             |    66,525.7 ns |   266.70 ns |   118.42 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 256  | Sorted             |     3,015.4 ns |   133.25 ns |    59.16 ns |  0.05 |    0.00 |    1 |         - |          NA |
| TreapSort              | 256  | Sorted             |     4,198.8 ns |   270.93 ns |   141.70 ns |  0.06 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Reversed**           |     **9,658.2 ns** |   **365.67 ns** |   **191.25 ns** |  **0.15** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 256  | Reversed           |    66,319.5 ns |   231.29 ns |   120.97 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 256  | Reversed           |     2,865.8 ns |    16.08 ns |     7.14 ns |  0.04 |    0.00 |    1 |         - |          NA |
| TreapSort              | 256  | Reversed           |     4,147.3 ns |   265.03 ns |   138.62 ns |  0.06 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **PipeOrgan**          |     **9,549.3 ns** |   **625.53 ns** |   **327.16 ns** |  **0.29** |    **0.01** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 256  | PipeOrgan          |    33,094.2 ns |   187.71 ns |    98.17 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 256  | PipeOrgan          |     3,493.2 ns |   299.42 ns |   132.95 ns |  0.11 |    0.00 |    1 |         - |          NA |
| TreapSort              | 256  | PipeOrgan          |     5,691.3 ns |   314.17 ns |   139.49 ns |  0.17 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Random**             |    **56,606.2 ns** | **6,053.72 ns** | **3,166.21 ns** |  **3.35** |    **0.18** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 1024 | Random             |    16,911.1 ns |   470.15 ns |   208.75 ns |  1.00 |    0.02 |    1 |         - |          NA |
| SplaySort              | 1024 | Random             |   116,473.2 ns | 4,194.11 ns | 2,193.60 ns |  6.89 |    0.15 |    4 |         - |          NA |
| TreapSort              | 1024 | Random             |    30,786.3 ns | 1,474.20 ns |   771.03 ns |  1.82 |    0.05 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **SingleElementMoved** |    **61,458.4 ns** | **4,945.22 ns** | **2,586.44 ns** |  **0.09** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 1024 | SingleElementMoved |   689,297.8 ns |   467.67 ns |   207.65 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | SingleElementMoved |    13,546.6 ns |   161.64 ns |    84.54 ns |  0.02 |    0.00 |    1 |         - |          NA |
| TreapSort              | 1024 | SingleElementMoved |    20,772.8 ns |   505.84 ns |   264.57 ns |  0.03 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Sorted**             |    **54,899.9 ns** | **6,005.39 ns** | **3,140.94 ns** |  **0.05** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 1024 | Sorted             | 1,055,139.5 ns | 1,681.96 ns |   879.70 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | Sorted             |    11,870.6 ns |   185.69 ns |    97.12 ns |  0.01 |    0.00 |    1 |         - |          NA |
| TreapSort              | 1024 | Sorted             |    17,896.3 ns |   431.95 ns |   225.92 ns |  0.02 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Reversed**           |    **47,125.7 ns** |   **425.35 ns** |   **188.86 ns** |  **0.04** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 1024 | Reversed           | 1,057,375.6 ns | 1,538.48 ns |   804.66 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | Reversed           |    11,406.8 ns |   312.33 ns |   163.36 ns |  0.01 |    0.00 |    1 |         - |          NA |
| TreapSort              | 1024 | Reversed           |    17,418.6 ns |   225.08 ns |    99.94 ns |  0.02 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **PipeOrgan**          |    **54,285.6 ns** | **2,406.88 ns** | **1,258.85 ns** |  **0.10** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 1024 | PipeOrgan          |   532,464.1 ns | 1,251.57 ns |   654.60 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | PipeOrgan          |    13,811.8 ns |   100.14 ns |    44.46 ns |  0.03 |    0.00 |    1 |         - |          NA |
| TreapSort              | 1024 | PipeOrgan          |    26,488.2 ns |   788.21 ns |   349.97 ns |  0.05 |    0.00 |    2 |         - |          NA |

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
- [std::stable_sort (LLVM)](./src/SortAlgorithm/Algorithms/Merge/StdStableSort.cs)
- [SymMerge Sort](./src/SortAlgorithm/Algorithms/Merge/SymMergeSort.cs)
- [Tim Sort](./src/SortAlgorithm/Algorithms/Merge/TimSort.cs)

### Heap
- [Binomial Heap Sort](./src/SortAlgorithm/Algorithms/Heap/BinomialHeapSort.cs)
- [Bottom-Up Heap Sort](./src/SortAlgorithm/Algorithms/Heap/BottomupHeapSort.cs)
- [Heap Sort](./src/SortAlgorithm/Algorithms/Heap/HeapSort.cs)
- [Min-Heap Sort](./src/SortAlgorithm/Algorithms/Heap/MinHeapSort.cs)
- [Pairing Heap Sort](./src/SortAlgorithm/Algorithms/Heap/PairingHeapSort.cs)
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
- [Cartesian Tree Sort](./src/SortAlgorithm/Algorithms/Tree/CartesianTreeSort.cs)
- [Splay Sort](./src/SortAlgorithm/Algorithms/Tree/SplaySort.cs)
- [Treap Sort](./src/SortAlgorithm/Algorithms/Tree/TreapSort.cs)

### Joke
- [Bogo Sort](./src/SortAlgorithm/Algorithms/Joke/BogoSort.cs)
- [Slow Sort](./src/SortAlgorithm/Algorithms/Joke/SlowSort.cs)
- [Stooge Sort](./src/SortAlgorithm/Algorithms/Joke/StoogeSort.cs)
<!-- ALGORITHMS_END -->
