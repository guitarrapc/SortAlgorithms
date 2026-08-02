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
<summary>Benchmark results (2026-08-02 18:55 UTC)</summary>

Workflow run: https://github.com/guitarrapc/SortAlgorithms/actions/runs/30761537820

### AdaptiveBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method        | Size | Pattern            | Mean         | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------- |----- |------------------- |-------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **DropMergeSort** | **256**  | **Random**             |   **3,156.7 ns** |    **410.79 ns** |    **214.85 ns** |  **1.00** |    **0.09** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | Random             |   7,850.8 ns |  2,186.39 ns |  1,143.52 ns |  2.50 |    0.38 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **256**  | **SingleElementMoved** |     **659.0 ns** |     **12.04 ns** |      **5.35 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | SingleElementMoved |   8,151.0 ns |  1,477.44 ns |    772.73 ns | 12.37 |    1.11 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **256**  | **Sorted**             |     **597.0 ns** |      **7.71 ns** |      **3.42 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | Sorted             |  13,602.5 ns |  1,122.57 ns |    587.12 ns | 22.78 |    0.94 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **256**  | **Reversed**           |   **1,572.8 ns** |    **182.02 ns** |     **80.82 ns** |  **1.00** |    **0.07** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | Reversed           |   1,468.8 ns |     10.52 ns |      4.67 ns |  0.94 |    0.04 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **256**  | **PipeOrgan**          |   **6,527.1 ns** |    **329.46 ns** |    **172.31 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | PipeOrgan          |   5,482.8 ns |    352.35 ns |    184.29 ns |  0.84 |    0.03 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **256**  | **ManyDuplicates**     |   **2,804.6 ns** |     **97.52 ns** |     **43.30 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | ManyDuplicates     |   4,018.9 ns |    246.16 ns |    128.75 ns |  1.43 |    0.05 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **Random**             |  **14,562.1 ns** |    **505.97 ns** |    **264.63 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | Random             |  24,160.2 ns |    838.83 ns |    372.45 ns |  1.66 |    0.04 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **SingleElementMoved** |   **2,499.9 ns** |      **4.24 ns** |      **1.88 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | SingleElementMoved |  40,390.9 ns |    980.61 ns |    512.88 ns | 16.16 |    0.19 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **Sorted**             |   **2,209.5 ns** |      **3.93 ns** |      **1.40 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | Sorted             |  39,163.0 ns |    486.03 ns |    215.80 ns | 17.72 |    0.09 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **Reversed**           |   **7,113.6 ns** |    **144.68 ns** |     **75.67 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| PatienceSort  | 1024 | Reversed           |   5,230.3 ns |    310.25 ns |    162.26 ns |  0.74 |    0.02 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **PipeOrgan**          |  **26,994.9 ns** |    **227.76 ns** |    **119.12 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | PipeOrgan          |  27,365.8 ns |  1,142.19 ns |    597.39 ns |  1.01 |    0.02 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **ManyDuplicates**     |  **12,714.8 ns** |    **192.78 ns** |     **85.59 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | ManyDuplicates     |  15,539.1 ns |    421.75 ns |    220.58 ns |  1.22 |    0.02 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **4096** | **Random**             |  **73,783.9 ns** | **10,453.01 ns** |  **5,467.13 ns** |  **1.00** |    **0.10** |    **1** |         **-** |          **NA** |
| PatienceSort  | 4096 | Random             | 157,446.4 ns | 24,627.92 ns | 10,934.95 ns |  2.14 |    0.20 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **4096** | **SingleElementMoved** |  **10,144.4 ns** |    **546.44 ns** |    **242.62 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| PatienceSort  | 4096 | SingleElementMoved | 268,865.0 ns | 83,081.93 ns | 43,453.46 ns | 26.52 |    4.09 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **4096** | **Sorted**             |   **9,449.3 ns** |  **1,525.39 ns** |    **797.81 ns** |  **1.01** |    **0.11** |    **1** |         **-** |          **NA** |
| PatienceSort  | 4096 | Sorted             | 213,725.5 ns | 19,488.44 ns |  8,652.99 ns | 22.75 |    1.89 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **4096** | **Reversed**           |  **31,040.1 ns** |    **360.25 ns** |    **159.95 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| PatienceSort  | 4096 | Reversed           |  20,688.7 ns |  1,083.74 ns |    481.19 ns |  0.67 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **4096** | **PipeOrgan**          | **111,639.0 ns** |    **327.20 ns** |    **145.28 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 4096 | PipeOrgan          | 179,731.0 ns | 21,752.57 ns | 11,377.01 ns |  1.61 |    0.10 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **4096** | **ManyDuplicates**     |  **56,818.0 ns** |  **1,958.79 ns** |  **1,024.48 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| PatienceSort  | 4096 | ManyDuplicates     |  59,918.4 ns |  1,136.55 ns |    504.63 ns |  1.05 |    0.02 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **Random**             | **473,623.5 ns** |  **3,892.67 ns** |  **2,035.94 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | Random             | 820,103.8 ns |  2,816.57 ns |  1,473.12 ns |  1.73 |    0.01 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **SingleElementMoved** |  **20,450.8 ns** |  **2,216.68 ns** |    **984.22 ns** |  **1.00** |    **0.06** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | SingleElementMoved | 771,056.2 ns |  2,943.99 ns |  1,307.15 ns | 37.77 |    1.59 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **Sorted**             |  **17,934.6 ns** |    **671.91 ns** |    **298.33 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | Sorted             | 782,359.3 ns |  5,890.22 ns |  3,080.70 ns | 43.63 |    0.68 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **Reversed**           |  **65,096.0 ns** |    **841.94 ns** |    **373.83 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| PatienceSort  | 8192 | Reversed           |  40,092.6 ns |    435.64 ns |    193.43 ns |  0.62 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **PipeOrgan**          | **226,468.0 ns** |  **1,653.19 ns** |    **734.03 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | PipeOrgan          | 664,822.7 ns |  3,477.06 ns |  1,543.84 ns |  2.94 |    0.01 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **ManyDuplicates**     | **120,640.0 ns** |    **990.29 ns** |    **439.69 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | ManyDuplicates     | 150,547.4 ns |  1,692.71 ns |    885.32 ns |  1.25 |    0.01 |    2 |         - |          NA |

### AdaptiveSlowBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method     | Size | Pattern            | Mean         | Error        | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------- |----- |------------------- |-------------:|-------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **StrandSort** | **256**  | **Random**             |   **6,581.4 ns** |    **256.59 ns** |   **134.20 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **256**  | **SingleElementMoved** |     **834.1 ns** |    **133.32 ns** |    **59.19 ns** |  **1.00** |    **0.09** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **256**  | **Sorted**             |     **563.4 ns** |     **11.37 ns** |     **5.05 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **256**  | **Reversed**           |  **48,048.1 ns** |    **558.66 ns** |   **292.19 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **256**  | **PipeOrgan**          |  **27,987.7 ns** |  **1,164.21 ns** |   **608.91 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **256**  | **ManyDuplicates**     |   **4,893.2 ns** |     **58.06 ns** |    **25.78 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **1024** | **Random**             |  **55,465.3 ns** |  **1,819.52 ns** |   **951.64 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **1024** | **SingleElementMoved** |   **2,842.4 ns** |     **46.14 ns** |    **20.49 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **1024** | **Sorted**             |   **1,850.9 ns** |    **361.93 ns** |   **189.29 ns** |  **1.01** |    **0.13** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **1024** | **Reversed**           | **768,198.8 ns** |  **8,313.93 ns** | **3,691.44 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **1024** | **PipeOrgan**          | **454,668.2 ns** | **17,703.83 ns** | **9,259.45 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **1024** | **ManyDuplicates**     |  **31,729.9 ns** |    **297.37 ns** |   **155.53 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |

### AmericanFlagRadixWidthBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                 | Size    | WideKeyRange | Mean            | Error        | StdDev      | Ratio | RatioSD | Allocated | Alloc Ratio |
| ----------------------- |-------- |------------- |----------------:|-------------:|------------:|------:|--------:|----------:|------------:|
| **Radix16_C16**            | **4096**    | **False**        |     **71,476.1 ns** |     **986.8 ns** |    **516.1 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| Radix256_Shipped       | 4096    | False        |     72,815.6 ns |   1,229.4 ns |    643.0 ns |  1.02 |    0.01 |         - |          NA |
| Radix256_Cycle         | 4096    | False        |     69,998.4 ns |     405.6 ns |    212.1 ns |  0.98 |    0.01 |         - |          NA |
| Radix256_BinaryLeaf    | 4096    | False        |     70,053.4 ns |     937.0 ns |    416.0 ns |  0.98 |    0.01 |         - |          NA |
| Radix256_PerNodeRescan | 4096    | False        |     79,246.7 ns |     995.9 ns |    520.9 ns |  1.11 |    0.01 |         - |          NA |
|         |              |                 |              |             |       |         |           |             |
| **Radix16_C16**            | **4096**    | **True**         |     **94,228.8 ns** |     **616.3 ns** |    **273.6 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Radix256_Shipped       | 4096    | True         |     49,228.7 ns |     369.3 ns |    164.0 ns |  0.52 |    0.00 |         - |          NA |
| Radix256_Cycle         | 4096    | True         |     47,520.9 ns |   1,833.3 ns |    814.0 ns |  0.50 |    0.01 |         - |          NA |
| Radix256_BinaryLeaf    | 4096    | True         |     60,460.5 ns |   1,750.4 ns |    915.5 ns |  0.64 |    0.01 |         - |          NA |
| Radix256_PerNodeRescan | 4096    | True         |     48,088.4 ns |     602.8 ns |    267.6 ns |  0.51 |    0.00 |         - |          NA |
|         |              |                 |              |             |       |         |           |             |
| **Radix16_C16**            | **8192**    | **False**        |    **176,736.3 ns** |   **2,849.6 ns** |  **1,490.4 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| Radix256_Shipped       | 8192    | False        |    144,042.5 ns |     956.8 ns |    500.4 ns |  0.82 |    0.01 |         - |          NA |
| Radix256_Cycle         | 8192    | False        |    140,189.6 ns |     447.9 ns |    198.9 ns |  0.79 |    0.01 |         - |          NA |
| Radix256_BinaryLeaf    | 8192    | False        |    138,975.4 ns |     446.6 ns |    233.6 ns |  0.79 |    0.01 |         - |          NA |
| Radix256_PerNodeRescan | 8192    | False        |    157,723.5 ns |     890.0 ns |    465.5 ns |  0.89 |    0.01 |         - |          NA |
|         |              |                 |              |             |       |         |           |             |
| **Radix16_C16**            | **8192**    | **True**         |    **211,880.1 ns** |   **2,402.1 ns** |  **1,256.3 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| Radix256_Shipped       | 8192    | True         |    116,359.4 ns |   3,819.6 ns |  1,997.7 ns |  0.55 |    0.01 |         - |          NA |
| Radix256_Cycle         | 8192    | True         |    111,682.7 ns |   3,810.5 ns |  1,992.9 ns |  0.53 |    0.01 |         - |          NA |
| Radix256_BinaryLeaf    | 8192    | True         |    154,460.5 ns |  14,721.2 ns |  7,699.5 ns |  0.73 |    0.03 |         - |          NA |
| Radix256_PerNodeRescan | 8192    | True         |    112,984.0 ns |   3,195.9 ns |  1,671.5 ns |  0.53 |    0.01 |         - |          NA |
|         |              |                 |              |             |       |         |           |             |
| **Radix16_C16**            | **65536**   | **False**        |  **2,328,876.1 ns** |   **5,324.0 ns** |  **1,898.6 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Radix256_Shipped       | 65536   | False        |  1,337,327.6 ns |   2,982.9 ns |  1,560.1 ns |  0.57 |    0.00 |         - |          NA |
| Radix256_Cycle         | 65536   | False        |  1,316,693.4 ns |   2,930.6 ns |  1,532.8 ns |  0.57 |    0.00 |         - |          NA |
| Radix256_BinaryLeaf    | 65536   | False        |  1,299,196.3 ns |   1,384.8 ns |    614.9 ns |  0.56 |    0.00 |         - |          NA |
| Radix256_PerNodeRescan | 65536   | False        |  1,464,376.0 ns |   6,909.2 ns |  3,067.7 ns |  0.63 |    0.00 |         - |          NA |
|         |              |                 |              |             |       |         |           |             |
| **Radix16_C16**            | **65536**   | **True**         |  **2,695,747.7 ns** |   **3,933.9 ns** |  **1,746.7 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Radix256_Shipped       | 65536   | True         |  1,688,373.5 ns |   2,861.0 ns |  1,270.3 ns |  0.63 |    0.00 |         - |          NA |
| Radix256_Cycle         | 65536   | True         |  1,722,839.9 ns |   4,043.1 ns |  2,114.6 ns |  0.64 |    0.00 |         - |          NA |
| Radix256_BinaryLeaf    | 65536   | True         |  1,873,212.8 ns |   3,362.8 ns |  1,758.8 ns |  0.69 |    0.00 |         - |          NA |
| Radix256_PerNodeRescan | 65536   | True         |  1,825,431.0 ns |   2,539.3 ns |  1,127.5 ns |  0.68 |    0.00 |         - |          NA |
|         |              |                 |              |             |       |         |           |             |
| **Radix16_C16**            | **1048576** | **False**        | **44,671,856.0 ns** |  **51,412.3 ns** | **22,827.4 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Radix256_Shipped       | 1048576 | False        | 29,173,389.4 ns | 121,350.3 ns | 53,880.3 ns |  0.65 |    0.00 |         - |          NA |
| Radix256_Cycle         | 1048576 | False        | 29,289,525.6 ns | 157,925.1 ns | 82,597.9 ns |  0.66 |    0.00 |         - |          NA |
| Radix256_BinaryLeaf    | 1048576 | False        | 28,700,089.7 ns |  21,756.1 ns | 11,378.9 ns |  0.64 |    0.00 |         - |          NA |
| Radix256_PerNodeRescan | 1048576 | False        | 32,739,920.4 ns |  69,044.3 ns | 30,656.1 ns |  0.73 |    0.00 |         - |          NA |
|         |              |                 |              |             |       |         |           |             |
| **Radix16_C16**            | **1048576** | **True**         | **50,477,120.2 ns** |  **89,828.3 ns** | **39,884.3 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Radix256_Shipped       | 1048576 | True         | 28,246,942.5 ns |  83,646.2 ns | 29,829.0 ns |  0.56 |    0.00 |         - |          NA |
| Radix256_Cycle         | 1048576 | True         | 28,287,251.1 ns | 176,287.0 ns | 92,201.5 ns |  0.56 |    0.00 |         - |          NA |
| Radix256_BinaryLeaf    | 1048576 | True         | 37,920,532.1 ns |  53,702.0 ns | 23,844.0 ns |  0.75 |    0.00 |         - |          NA |
| Radix256_PerNodeRescan | 1048576 | True         | 29,188,166.0 ns | 187,644.5 ns | 98,141.7 ns |  0.58 |    0.00 |         - |          NA |

### DistributionBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method              | Size | Pattern            | Mean         | Error        | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------------- |----- |------------------- |-------------:|-------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **CountingSort**        | **256**  | **Random**             |   **1,753.2 ns** |     **80.88 ns** |    **28.84 ns** |  **1.78** |    **0.03** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | Random             |     982.8 ns |      8.63 ns |     3.08 ns |  1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 256  | Random             |   1,532.9 ns |     17.96 ns |     7.98 ns |  1.56 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | Random             |     843.2 ns |    158.80 ns |    83.06 ns |  0.86 |    0.08 |    1 |         - |          NA |
| BucketSort          | 256  | Random             |   2,101.7 ns |     11.72 ns |     4.18 ns |  2.14 |    0.01 |    2 |         - |          NA |
| BucketSortInteger   | 256  | Random             |   1,570.5 ns |      7.13 ns |     3.17 ns |  1.60 |    0.01 |    2 |         - |          NA |
| FlashSort           | 256  | Random             |   4,628.5 ns |    332.88 ns |   174.10 ns |  4.71 |    0.17 |    4 |         - |          NA |
| RadixLSD4Sort       | 256  | Random             |   2,886.2 ns |     13.39 ns |     7.01 ns |  2.94 |    0.01 |    3 |         - |          NA |
| RadixLSD256Sort     | 256  | Random             |   1,306.1 ns |      7.47 ns |     3.32 ns |  1.33 |    0.00 |    2 |         - |          NA |
| RadixLSD10Sort      | 256  | Random             |   4,121.7 ns |     11.73 ns |     4.18 ns |  4.19 |    0.01 |    4 |         - |          NA |
| RadixMSD4Sort       | 256  | Random             |   2,903.7 ns |     28.93 ns |    10.32 ns |  2.95 |    0.01 |    3 |         - |          NA |
| RadixMSD10Sort      | 256  | Random             |   4,050.9 ns |     72.70 ns |    25.93 ns |  4.12 |    0.03 |    4 |         - |          NA |
| AmericanFlagSort    | 256  | Random             |   2,872.1 ns |    101.33 ns |    44.99 ns |  2.92 |    0.04 |    3 |         - |          NA |
| SpreadSort          | 256  | Random             |   1,860.3 ns |     43.74 ns |    15.60 ns |  1.89 |    0.02 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **256**  | **SingleElementMoved** |   **1,592.8 ns** |      **9.54 ns** |     **4.24 ns** |  **1.43** |    **0.02** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | SingleElementMoved |   1,110.5 ns |     43.33 ns |    19.24 ns |  1.00 |    0.02 |    2 |         - |          NA |
| PigeonSort          | 256  | SingleElementMoved |   1,388.1 ns |     76.54 ns |    33.98 ns |  1.25 |    0.03 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | SingleElementMoved |     709.5 ns |     48.84 ns |    21.69 ns |  0.64 |    0.02 |    1 |         - |          NA |
| BucketSort          | 256  | SingleElementMoved |   1,959.1 ns |      4.34 ns |     1.55 ns |  1.76 |    0.03 |    3 |         - |          NA |
| BucketSortInteger   | 256  | SingleElementMoved |   1,623.6 ns |     16.34 ns |     5.83 ns |  1.46 |    0.02 |    2 |         - |          NA |
| FlashSort           | 256  | SingleElementMoved |   5,447.4 ns |    291.12 ns |   152.26 ns |  4.91 |    0.15 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | SingleElementMoved |   2,991.1 ns |     85.82 ns |    30.60 ns |  2.69 |    0.05 |    3 |         - |          NA |
| RadixLSD256Sort     | 256  | SingleElementMoved |   1,178.6 ns |     99.44 ns |    44.15 ns |  1.06 |    0.04 |    2 |         - |          NA |
| RadixLSD10Sort      | 256  | SingleElementMoved |   4,587.0 ns |  1,801.55 ns |   799.90 ns |  4.13 |    0.68 |    4 |         - |          NA |
| RadixMSD4Sort       | 256  | SingleElementMoved |   2,700.8 ns |     13.28 ns |     4.73 ns |  2.43 |    0.04 |    3 |         - |          NA |
| RadixMSD10Sort      | 256  | SingleElementMoved |   3,942.9 ns |    335.03 ns |   175.23 ns |  3.55 |    0.16 |    4 |         - |          NA |
| AmericanFlagSort    | 256  | SingleElementMoved |   2,311.2 ns |     14.44 ns |     5.15 ns |  2.08 |    0.03 |    3 |         - |          NA |
| SpreadSort          | 256  | SingleElementMoved |   1,142.2 ns |     18.63 ns |     8.27 ns |  1.03 |    0.02 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **256**  | **Sorted**             |   **1,531.6 ns** |     **59.94 ns** |    **31.35 ns** |  **1.68** |    **0.05** |    **5** |         **-** |          **NA** |
| CountingSortInteger | 256  | Sorted             |     910.5 ns |     51.49 ns |    22.86 ns |  1.00 |    0.03 |    3 |         - |          NA |
| PigeonSort          | 256  | Sorted             |   1,472.0 ns |      6.12 ns |     2.72 ns |  1.62 |    0.04 |    5 |         - |          NA |
| PigeonSortInteger   | 256  | Sorted             |     623.8 ns |      3.15 ns |     1.40 ns |  0.69 |    0.02 |    2 |         - |          NA |
| BucketSort          | 256  | Sorted             |   2,079.5 ns |     14.16 ns |     6.29 ns |  2.29 |    0.05 |    6 |         - |          NA |
| BucketSortInteger   | 256  | Sorted             |   1,460.1 ns |      5.24 ns |     2.33 ns |  1.60 |    0.04 |    5 |         - |          NA |
| FlashSort           | 256  | Sorted             |   5,488.1 ns |    423.58 ns |   188.07 ns |  6.03 |    0.24 |    9 |         - |          NA |
| RadixLSD4Sort       | 256  | Sorted             |   2,813.7 ns |    103.31 ns |    45.87 ns |  3.09 |    0.09 |    7 |         - |          NA |
| RadixLSD256Sort     | 256  | Sorted             |   1,253.5 ns |    242.01 ns |   107.45 ns |  1.38 |    0.11 |    4 |         - |          NA |
| RadixLSD10Sort      | 256  | Sorted             |   4,061.3 ns |     45.72 ns |    16.31 ns |  4.46 |    0.10 |    8 |         - |          NA |
| RadixMSD4Sort       | 256  | Sorted             |   2,715.1 ns |    247.02 ns |   129.19 ns |  2.98 |    0.15 |    7 |         - |          NA |
| RadixMSD10Sort      | 256  | Sorted             |   3,829.2 ns |     32.65 ns |    11.64 ns |  4.21 |    0.10 |    8 |         - |          NA |
| AmericanFlagSort    | 256  | Sorted             |   1,511.1 ns |      3.51 ns |     1.25 ns |  1.66 |    0.04 |    5 |         - |          NA |
| SpreadSort          | 256  | Sorted             |     412.2 ns |      3.27 ns |     1.17 ns |  0.45 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **256**  | **Reversed**           |   **1,498.7 ns** |     **14.19 ns** |     **6.30 ns** |  **1.44** |    **0.04** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | Reversed           |   1,043.6 ns |     68.85 ns |    30.57 ns |  1.00 |    0.04 |    2 |         - |          NA |
| PigeonSort          | 256  | Reversed           |   1,412.9 ns |    200.97 ns |    89.23 ns |  1.35 |    0.09 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | Reversed           |     653.1 ns |    105.79 ns |    55.33 ns |  0.63 |    0.05 |    1 |         - |          NA |
| BucketSort          | 256  | Reversed           |   2,029.6 ns |    207.22 ns |   108.38 ns |  1.95 |    0.11 |    3 |         - |          NA |
| BucketSortInteger   | 256  | Reversed           |   1,578.1 ns |     15.49 ns |     6.88 ns |  1.51 |    0.04 |    3 |         - |          NA |
| FlashSort           | 256  | Reversed           |   4,768.5 ns |     51.32 ns |    18.30 ns |  4.57 |    0.13 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | Reversed           |   3,096.5 ns |    351.47 ns |   183.82 ns |  2.97 |    0.19 |    4 |         - |          NA |
| RadixLSD256Sort     | 256  | Reversed           |   1,111.7 ns |     36.76 ns |    19.22 ns |  1.07 |    0.03 |    2 |         - |          NA |
| RadixLSD10Sort      | 256  | Reversed           |   3,865.2 ns |    107.73 ns |    47.83 ns |  3.71 |    0.11 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | Reversed           |   3,803.8 ns |    350.72 ns |   183.44 ns |  3.65 |    0.19 |    5 |         - |          NA |
| RadixMSD10Sort      | 256  | Reversed           |   4,474.7 ns |    345.33 ns |   180.61 ns |  4.29 |    0.20 |    5 |         - |          NA |
| AmericanFlagSort    | 256  | Reversed           |   1,778.3 ns |     33.54 ns |    11.96 ns |  1.71 |    0.05 |    3 |         - |          NA |
| SpreadSort          | 256  | Reversed           |     582.7 ns |    117.25 ns |    52.06 ns |  0.56 |    0.05 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **256**  | **PipeOrgan**          |   **1,545.3 ns** |     **17.33 ns** |     **7.70 ns** |  **1.42** |    **0.01** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | PipeOrgan          |   1,091.9 ns |     12.42 ns |     5.52 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PigeonSort          | 256  | PipeOrgan          |   1,392.8 ns |    120.95 ns |    53.70 ns |  1.28 |    0.05 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | PipeOrgan          |     698.5 ns |      6.38 ns |     2.83 ns |  0.64 |    0.00 |    1 |         - |          NA |
| BucketSort          | 256  | PipeOrgan          |   2,821.7 ns |    566.85 ns |   296.47 ns |  2.58 |    0.26 |    4 |         - |          NA |
| BucketSortInteger   | 256  | PipeOrgan          |   1,663.5 ns |     12.81 ns |     6.70 ns |  1.52 |    0.01 |    2 |         - |          NA |
| FlashSort           | 256  | PipeOrgan          |   5,158.3 ns |    241.13 ns |   126.11 ns |  4.72 |    0.11 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | PipeOrgan          |   3,050.5 ns |    361.30 ns |   188.97 ns |  2.79 |    0.16 |    4 |         - |          NA |
| RadixLSD256Sort     | 256  | PipeOrgan          |   1,194.0 ns |      9.50 ns |     4.97 ns |  1.09 |    0.01 |    2 |         - |          NA |
| RadixLSD10Sort      | 256  | PipeOrgan          |   4,371.7 ns |     45.78 ns |    16.33 ns |  4.00 |    0.02 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | PipeOrgan          |   3,324.2 ns |     39.50 ns |    14.09 ns |  3.04 |    0.02 |    4 |         - |          NA |
| RadixMSD10Sort      | 256  | PipeOrgan          |   4,341.8 ns |    561.51 ns |   293.68 ns |  3.98 |    0.25 |    5 |         - |          NA |
| AmericanFlagSort    | 256  | PipeOrgan          |   2,252.4 ns |     11.37 ns |     5.05 ns |  2.06 |    0.01 |    3 |         - |          NA |
| SpreadSort          | 256  | PipeOrgan          |   1,859.2 ns |    340.17 ns |   177.91 ns |  1.70 |    0.15 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **256**  | **ManyDuplicates**     |   **1,525.1 ns** |     **17.98 ns** |     **7.99 ns** |  **1.74** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | ManyDuplicates     |     876.4 ns |     20.93 ns |     9.29 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PigeonSort          | 256  | ManyDuplicates     |   1,453.1 ns |      6.99 ns |     3.11 ns |  1.66 |    0.02 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | ManyDuplicates     |     629.0 ns |      4.86 ns |     2.16 ns |  0.72 |    0.01 |    1 |         - |          NA |
| BucketSort          | 256  | ManyDuplicates     |   3,138.9 ns |    250.10 ns |   130.81 ns |  3.58 |    0.15 |    5 |         - |          NA |
| BucketSortInteger   | 256  | ManyDuplicates     |   1,721.1 ns |     24.49 ns |    10.87 ns |  1.96 |    0.02 |    3 |         - |          NA |
| FlashSort           | 256  | ManyDuplicates     |   4,682.6 ns |    356.18 ns |   186.29 ns |  5.34 |    0.21 |    6 |         - |          NA |
| RadixLSD4Sort       | 256  | ManyDuplicates     |   2,319.2 ns |     16.16 ns |     5.76 ns |  2.65 |    0.03 |    4 |         - |          NA |
| RadixLSD256Sort     | 256  | ManyDuplicates     |   1,587.2 ns |    165.35 ns |    73.42 ns |  1.81 |    0.08 |    3 |         - |          NA |
| RadixLSD10Sort      | 256  | ManyDuplicates     |   2,898.2 ns |    171.35 ns |    76.08 ns |  3.31 |    0.09 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | ManyDuplicates     |   2,891.4 ns |     43.55 ns |    19.34 ns |  3.30 |    0.04 |    5 |         - |          NA |
| RadixMSD10Sort      | 256  | ManyDuplicates     |   3,646.6 ns |     14.63 ns |     5.22 ns |  4.16 |    0.04 |    5 |         - |          NA |
| AmericanFlagSort    | 256  | ManyDuplicates     |   3,348.0 ns |    228.56 ns |   119.54 ns |  3.82 |    0.13 |    5 |         - |          NA |
| SpreadSort          | 256  | ManyDuplicates     |   1,598.5 ns |     21.08 ns |     7.52 ns |  1.82 |    0.02 |    3 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **1024** | **Random**             |   **6,036.8 ns** |    **357.84 ns** |   **187.16 ns** |  **1.57** |    **0.05** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Random             |   3,850.3 ns |     33.34 ns |    11.89 ns |  1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | Random             |   5,735.3 ns |    351.10 ns |   183.63 ns |  1.49 |    0.05 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | Random             |   2,883.1 ns |    261.37 ns |   116.05 ns |  0.75 |    0.03 |    1 |         - |          NA |
| BucketSort          | 1024 | Random             |   7,955.6 ns |     35.89 ns |    15.93 ns |  2.07 |    0.01 |    4 |         - |          NA |
| BucketSortInteger   | 1024 | Random             |   5,896.1 ns |    234.95 ns |   122.88 ns |  1.53 |    0.03 |    3 |         - |          NA |
| FlashSort           | 1024 | Random             |  18,696.6 ns |    254.17 ns |   132.93 ns |  4.86 |    0.04 |    6 |         - |          NA |
| RadixLSD4Sort       | 1024 | Random             |  15,274.3 ns |     58.59 ns |    30.64 ns |  3.97 |    0.01 |    5 |         - |          NA |
| RadixLSD256Sort     | 1024 | Random             |   7,419.0 ns |    362.45 ns |   189.57 ns |  1.93 |    0.05 |    4 |         - |          NA |
| RadixLSD10Sort      | 1024 | Random             |  21,231.1 ns |    407.43 ns |   213.10 ns |  5.51 |    0.05 |    6 |         - |          NA |
| RadixMSD4Sort       | 1024 | Random             |  14,883.1 ns |    403.85 ns |   211.22 ns |  3.87 |    0.05 |    5 |         - |          NA |
| RadixMSD10Sort      | 1024 | Random             |  21,962.0 ns |    408.07 ns |   213.43 ns |  5.70 |    0.05 |    6 |         - |          NA |
| AmericanFlagSort    | 1024 | Random             |  18,475.1 ns |    193.96 ns |    86.12 ns |  4.80 |    0.03 |    6 |         - |          NA |
| SpreadSort          | 1024 | Random             |   9,174.6 ns |    468.84 ns |   245.21 ns |  2.38 |    0.06 |    4 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **1024** | **SingleElementMoved** |   **6,084.4 ns** |    **441.48 ns** |   **230.90 ns** |  **1.46** |    **0.07** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | SingleElementMoved |   4,172.6 ns |    246.94 ns |   129.15 ns |  1.00 |    0.04 |    2 |         - |          NA |
| PigeonSort          | 1024 | SingleElementMoved |   4,902.4 ns |     10.32 ns |     3.68 ns |  1.18 |    0.03 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | SingleElementMoved |   3,355.8 ns |  1,538.97 ns |   804.91 ns |  0.80 |    0.18 |    1 |         - |          NA |
| BucketSort          | 1024 | SingleElementMoved |   7,421.9 ns |     70.42 ns |    25.11 ns |  1.78 |    0.05 |    3 |         - |          NA |
| BucketSortInteger   | 1024 | SingleElementMoved |   6,204.4 ns |    247.51 ns |   129.45 ns |  1.49 |    0.05 |    3 |         - |          NA |
| FlashSort           | 1024 | SingleElementMoved |  21,440.4 ns |    248.05 ns |   110.14 ns |  5.14 |    0.15 |    5 |         - |          NA |
| RadixLSD4Sort       | 1024 | SingleElementMoved |  15,490.0 ns |    372.06 ns |   165.20 ns |  3.72 |    0.11 |    4 |         - |          NA |
| RadixLSD256Sort     | 1024 | SingleElementMoved |   6,458.3 ns |    275.79 ns |   144.24 ns |  1.55 |    0.06 |    3 |         - |          NA |
| RadixLSD10Sort      | 1024 | SingleElementMoved |  21,251.8 ns |    199.38 ns |   104.28 ns |  5.10 |    0.15 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | SingleElementMoved |  12,776.5 ns |    111.66 ns |    49.58 ns |  3.06 |    0.09 |    4 |         - |          NA |
| RadixMSD10Sort      | 1024 | SingleElementMoved |  19,488.3 ns |    141.74 ns |    62.93 ns |  4.67 |    0.13 |    5 |         - |          NA |
| AmericanFlagSort    | 1024 | SingleElementMoved |  12,991.3 ns |    104.37 ns |    46.34 ns |  3.12 |    0.09 |    4 |         - |          NA |
| SpreadSort          | 1024 | SingleElementMoved |   6,776.4 ns |     25.17 ns |    11.18 ns |  1.63 |    0.05 |    3 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **1024** | **Sorted**             |   **5,697.0 ns** |    **357.80 ns** |   **187.14 ns** |  **1.62** |    **0.05** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Sorted             |   3,507.7 ns |     18.22 ns |     8.09 ns |  1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 1024 | Sorted             |   5,293.4 ns |     66.18 ns |    23.60 ns |  1.51 |    0.01 |    4 |         - |          NA |
| PigeonSortInteger   | 1024 | Sorted             |   2,437.5 ns |      4.49 ns |     1.99 ns |  0.69 |    0.00 |    2 |         - |          NA |
| BucketSort          | 1024 | Sorted             |   8,151.1 ns |    251.35 ns |   131.46 ns |  2.32 |    0.04 |    5 |         - |          NA |
| BucketSortInteger   | 1024 | Sorted             |   5,436.5 ns |      7.91 ns |     2.82 ns |  1.55 |    0.00 |    4 |         - |          NA |
| FlashSort           | 1024 | Sorted             |  21,319.2 ns |    262.38 ns |   116.50 ns |  6.08 |    0.03 |    7 |         - |          NA |
| RadixLSD4Sort       | 1024 | Sorted             |  15,307.8 ns |    288.92 ns |   128.28 ns |  4.36 |    0.04 |    6 |         - |          NA |
| RadixLSD256Sort     | 1024 | Sorted             |   6,516.9 ns |    285.22 ns |   149.17 ns |  1.86 |    0.04 |    4 |         - |          NA |
| RadixLSD10Sort      | 1024 | Sorted             |  20,830.1 ns |    254.04 ns |   132.87 ns |  5.94 |    0.04 |    7 |         - |          NA |
| RadixMSD4Sort       | 1024 | Sorted             |  12,783.2 ns |    326.68 ns |   170.86 ns |  3.64 |    0.05 |    6 |         - |          NA |
| RadixMSD10Sort      | 1024 | Sorted             |  19,601.3 ns |     53.28 ns |    23.66 ns |  5.59 |    0.01 |    7 |         - |          NA |
| AmericanFlagSort    | 1024 | Sorted             |   9,602.4 ns |    260.21 ns |   136.10 ns |  2.74 |    0.04 |    5 |         - |          NA |
| SpreadSort          | 1024 | Sorted             |     903.7 ns |    547.70 ns |   286.46 ns |  0.26 |    0.08 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **1024** | **Reversed**           |   **5,507.7 ns** |     **60.91 ns** |    **21.72 ns** |  **1.40** |    **0.06** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Reversed           |   3,926.6 ns |    326.78 ns |   170.91 ns |  1.00 |    0.06 |    2 |         - |          NA |
| PigeonSort          | 1024 | Reversed           |   5,173.9 ns |    418.84 ns |   219.06 ns |  1.32 |    0.07 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | Reversed           |   2,448.7 ns |     59.43 ns |    26.39 ns |  0.62 |    0.03 |    1 |         - |          NA |
| BucketSort          | 1024 | Reversed           |   7,735.2 ns |    293.01 ns |   153.25 ns |  1.97 |    0.09 |    4 |         - |          NA |
| BucketSortInteger   | 1024 | Reversed           |   6,053.1 ns |    377.60 ns |   197.49 ns |  1.54 |    0.08 |    3 |         - |          NA |
| FlashSort           | 1024 | Reversed           |  18,642.6 ns |    238.34 ns |   124.66 ns |  4.76 |    0.19 |    6 |         - |          NA |
| RadixLSD4Sort       | 1024 | Reversed           |  15,948.6 ns |    375.59 ns |   196.44 ns |  4.07 |    0.17 |    6 |         - |          NA |
| RadixLSD256Sort     | 1024 | Reversed           |   6,057.3 ns |     58.30 ns |    20.79 ns |  1.55 |    0.06 |    3 |         - |          NA |
| RadixLSD10Sort      | 1024 | Reversed           |  20,879.0 ns |    817.06 ns |   427.34 ns |  5.33 |    0.24 |    6 |         - |          NA |
| RadixMSD4Sort       | 1024 | Reversed           |  16,918.9 ns |    584.54 ns |   305.72 ns |  4.32 |    0.19 |    6 |         - |          NA |
| RadixMSD10Sort      | 1024 | Reversed           |  22,031.9 ns |    285.80 ns |   149.48 ns |  5.62 |    0.23 |    6 |         - |          NA |
| AmericanFlagSort    | 1024 | Reversed           |  12,095.1 ns |    258.55 ns |   135.22 ns |  3.09 |    0.13 |    5 |         - |          NA |
| SpreadSort          | 1024 | Reversed           |   5,500.4 ns |    329.17 ns |   172.16 ns |  1.40 |    0.07 |    3 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **1024** | **PipeOrgan**          |   **5,841.4 ns** |    **350.25 ns** |   **183.19 ns** |  **1.39** |    **0.04** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | PipeOrgan          |   4,197.1 ns |     17.70 ns |     6.31 ns |  1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | PipeOrgan          |   5,162.9 ns |    396.84 ns |   207.56 ns |  1.23 |    0.05 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | PipeOrgan          |   2,794.9 ns |      4.64 ns |     2.43 ns |  0.67 |    0.00 |    1 |         - |          NA |
| BucketSort          | 1024 | PipeOrgan          |   7,656.2 ns |     51.02 ns |    26.68 ns |  1.82 |    0.01 |    3 |         - |          NA |
| BucketSortInteger   | 1024 | PipeOrgan          |   6,737.7 ns |    471.01 ns |   246.35 ns |  1.61 |    0.06 |    3 |         - |          NA |
| FlashSort           | 1024 | PipeOrgan          |  19,858.5 ns |    212.37 ns |   111.07 ns |  4.73 |    0.03 |    5 |         - |          NA |
| RadixLSD4Sort       | 1024 | PipeOrgan          |  14,365.1 ns |    548.33 ns |   286.79 ns |  3.42 |    0.06 |    4 |         - |          NA |
| RadixLSD256Sort     | 1024 | PipeOrgan          |   6,371.3 ns |    193.17 ns |   101.03 ns |  1.52 |    0.02 |    3 |         - |          NA |
| RadixLSD10Sort      | 1024 | PipeOrgan          |  21,349.6 ns |    291.77 ns |   152.60 ns |  5.09 |    0.04 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | PipeOrgan          |  16,447.9 ns |    655.02 ns |   342.59 ns |  3.92 |    0.08 |    4 |         - |          NA |
| RadixMSD10Sort      | 1024 | PipeOrgan          |  21,082.3 ns |    168.08 ns |    74.63 ns |  5.02 |    0.02 |    5 |         - |          NA |
| AmericanFlagSort    | 1024 | PipeOrgan          |  14,897.6 ns |    140.63 ns |    62.44 ns |  3.55 |    0.01 |    4 |         - |          NA |
| SpreadSort          | 1024 | PipeOrgan          |   7,225.5 ns |     42.46 ns |    18.85 ns |  1.72 |    0.00 |    3 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **1024** | **ManyDuplicates**     |   **5,539.6 ns** |    **351.40 ns** |   **183.79 ns** |  **1.39** |    **0.04** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | ManyDuplicates     |   3,982.0 ns |     28.72 ns |    10.24 ns |  1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | ManyDuplicates     |   5,699.2 ns |    132.38 ns |    58.78 ns |  1.43 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | ManyDuplicates     |   2,425.0 ns |      8.19 ns |     4.29 ns |  0.61 |    0.00 |    1 |         - |          NA |
| BucketSort          | 1024 | ManyDuplicates     |  12,460.7 ns |    211.06 ns |   110.39 ns |  3.13 |    0.03 |    4 |         - |          NA |
| BucketSortInteger   | 1024 | ManyDuplicates     |   6,671.0 ns |    367.92 ns |   192.43 ns |  1.68 |    0.05 |    3 |         - |          NA |
| FlashSort           | 1024 | ManyDuplicates     |  19,913.4 ns |    298.45 ns |   156.09 ns |  5.00 |    0.04 |    5 |         - |          NA |
| RadixLSD4Sort       | 1024 | ManyDuplicates     |   9,229.3 ns |    251.45 ns |   131.51 ns |  2.32 |    0.03 |    4 |         - |          NA |
| RadixLSD256Sort     | 1024 | ManyDuplicates     |   4,415.9 ns |    338.95 ns |   177.28 ns |  1.11 |    0.04 |    2 |         - |          NA |
| RadixLSD10Sort      | 1024 | ManyDuplicates     |  11,493.6 ns |    295.39 ns |   154.50 ns |  2.89 |    0.04 |    4 |         - |          NA |
| RadixMSD4Sort       | 1024 | ManyDuplicates     |  10,785.6 ns |    413.78 ns |   216.42 ns |  2.71 |    0.05 |    4 |         - |          NA |
| RadixMSD10Sort      | 1024 | ManyDuplicates     |  12,884.6 ns |    305.47 ns |   159.76 ns |  3.24 |    0.04 |    4 |         - |          NA |
| AmericanFlagSort    | 1024 | ManyDuplicates     |   9,893.8 ns |    327.76 ns |   171.42 ns |  2.48 |    0.04 |    4 |         - |          NA |
| SpreadSort          | 1024 | ManyDuplicates     |   6,713.4 ns |    282.55 ns |   147.78 ns |  1.69 |    0.04 |    3 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **4096** | **Random**             |  **25,353.7 ns** |    **247.73 ns** |   **129.57 ns** |  **1.60** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Random             |  15,891.6 ns |    259.54 ns |   115.24 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PigeonSort          | 4096 | Random             |  22,987.9 ns |  1,240.79 ns |   550.92 ns |  1.45 |    0.03 |    3 |         - |          NA |
| PigeonSortInteger   | 4096 | Random             |  12,238.1 ns |  1,283.30 ns |   671.19 ns |  0.77 |    0.04 |    1 |         - |          NA |
| BucketSort          | 4096 | Random             |  33,316.0 ns |    429.29 ns |   224.53 ns |  2.10 |    0.02 |    4 |         - |          NA |
| BucketSortInteger   | 4096 | Random             |  23,771.3 ns |    225.49 ns |   117.93 ns |  1.50 |    0.01 |    3 |         - |          NA |
| FlashSort           | 4096 | Random             |  77,155.4 ns |    415.55 ns |   217.34 ns |  4.86 |    0.04 |    5 |         - |          NA |
| RadixLSD4Sort       | 4096 | Random             |  65,162.1 ns |    517.72 ns |   229.87 ns |  4.10 |    0.03 |    5 |         - |          NA |
| RadixLSD256Sort     | 4096 | Random             |  26,291.1 ns |    552.08 ns |   245.13 ns |  1.65 |    0.02 |    3 |         - |          NA |
| RadixLSD10Sort      | 4096 | Random             |  84,891.7 ns |    727.18 ns |   322.87 ns |  5.34 |    0.04 |    5 |         - |          NA |
| RadixMSD4Sort       | 4096 | Random             |  71,186.3 ns |  1,112.15 ns |   581.68 ns |  4.48 |    0.05 |    5 |         - |          NA |
| RadixMSD10Sort      | 4096 | Random             |  86,613.6 ns |    915.24 ns |   478.69 ns |  5.45 |    0.05 |    5 |         - |          NA |
| AmericanFlagSort    | 4096 | Random             |  73,188.1 ns |  1,782.48 ns |   932.27 ns |  4.61 |    0.06 |    5 |         - |          NA |
| SpreadSort          | 4096 | Random             |  38,876.6 ns |    443.19 ns |   196.78 ns |  2.45 |    0.02 |    4 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **4096** | **SingleElementMoved** |  **25,490.8 ns** |  **1,725.91 ns** |   **902.68 ns** |  **1.52** |    **0.06** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 4096 | SingleElementMoved |  16,756.4 ns |    596.99 ns |   312.24 ns |  1.00 |    0.02 |    2 |         - |          NA |
| PigeonSort          | 4096 | SingleElementMoved |  19,954.4 ns |    840.23 ns |   373.07 ns |  1.19 |    0.03 |    2 |         - |          NA |
| PigeonSortInteger   | 4096 | SingleElementMoved |  11,320.4 ns |    302.21 ns |   158.06 ns |  0.68 |    0.01 |    1 |         - |          NA |
| BucketSort          | 4096 | SingleElementMoved |  31,296.5 ns |  1,135.06 ns |   593.66 ns |  1.87 |    0.05 |    2 |         - |          NA |
| BucketSortInteger   | 4096 | SingleElementMoved |  25,598.3 ns |  2,064.31 ns | 1,079.68 ns |  1.53 |    0.07 |    2 |         - |          NA |
| FlashSort           | 4096 | SingleElementMoved | 109,318.6 ns | 11,549.13 ns | 6,040.42 ns |  6.53 |    0.36 |    5 |         - |          NA |
| RadixLSD4Sort       | 4096 | SingleElementMoved |  93,248.3 ns |  1,397.61 ns |   730.98 ns |  5.57 |    0.11 |    5 |         - |          NA |
| RadixLSD256Sort     | 4096 | SingleElementMoved |  22,890.5 ns |    594.22 ns |   310.79 ns |  1.37 |    0.03 |    2 |         - |          NA |
| RadixLSD10Sort      | 4096 | SingleElementMoved |  84,468.9 ns |  1,146.70 ns |   599.74 ns |  5.04 |    0.09 |    5 |         - |          NA |
| RadixMSD4Sort       | 4096 | SingleElementMoved |  59,542.0 ns |    604.28 ns |   268.31 ns |  3.55 |    0.06 |    4 |         - |          NA |
| RadixMSD10Sort      | 4096 | SingleElementMoved |  79,098.9 ns |  2,362.07 ns | 1,048.77 ns |  4.72 |    0.10 |    5 |         - |          NA |
| AmericanFlagSort    | 4096 | SingleElementMoved |  48,568.0 ns |  1,270.32 ns |   664.40 ns |  2.90 |    0.06 |    3 |         - |          NA |
| SpreadSort          | 4096 | SingleElementMoved |  27,103.7 ns |    284.77 ns |   126.44 ns |  1.62 |    0.03 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **4096** | **Sorted**             |  **22,367.6 ns** |    **427.52 ns** |   **189.82 ns** |  **1.61** |    **0.03** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Sorted             |  13,895.2 ns |    425.11 ns |   222.34 ns |  1.00 |    0.02 |    3 |         - |          NA |
| PigeonSort          | 4096 | Sorted             |  21,413.4 ns |  1,030.30 ns |   538.86 ns |  1.54 |    0.04 |    4 |         - |          NA |
| PigeonSortInteger   | 4096 | Sorted             |   9,990.1 ns |    596.43 ns |   311.94 ns |  0.72 |    0.02 |    2 |         - |          NA |
| BucketSort          | 4096 | Sorted             |  31,833.9 ns |    694.08 ns |   308.17 ns |  2.29 |    0.04 |    5 |         - |          NA |
| BucketSortInteger   | 4096 | Sorted             |  22,209.7 ns |    850.45 ns |   377.60 ns |  1.60 |    0.03 |    4 |         - |          NA |
| FlashSort           | 4096 | Sorted             |  86,284.0 ns |    927.06 ns |   484.87 ns |  6.21 |    0.10 |    7 |         - |          NA |
| RadixLSD4Sort       | 4096 | Sorted             |  93,782.1 ns |  1,582.08 ns |   827.46 ns |  6.75 |    0.11 |    7 |         - |          NA |
| RadixLSD256Sort     | 4096 | Sorted             |  23,815.8 ns |    981.21 ns |   435.67 ns |  1.71 |    0.04 |    4 |         - |          NA |
| RadixLSD10Sort      | 4096 | Sorted             |  83,461.9 ns |    739.69 ns |   328.43 ns |  6.01 |    0.09 |    7 |         - |          NA |
| RadixMSD4Sort       | 4096 | Sorted             |  60,049.6 ns |    830.99 ns |   368.97 ns |  4.32 |    0.07 |    6 |         - |          NA |
| RadixMSD10Sort      | 4096 | Sorted             |  78,923.1 ns |    613.31 ns |   320.78 ns |  5.68 |    0.09 |    7 |         - |          NA |
| AmericanFlagSort    | 4096 | Sorted             |  35,063.4 ns |    242.61 ns |   107.72 ns |  2.52 |    0.04 |    5 |         - |          NA |
| SpreadSort          | 4096 | Sorted             |   2,255.9 ns |      4.11 ns |     1.82 ns |  0.16 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **4096** | **Reversed**           |  **22,281.1 ns** |    **677.66 ns** |   **354.43 ns** |  **1.45** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Reversed           |  15,351.9 ns |    323.65 ns |   115.42 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PigeonSort          | 4096 | Reversed           |  20,548.3 ns |  2,627.16 ns | 1,374.06 ns |  1.34 |    0.09 |    3 |         - |          NA |
| PigeonSortInteger   | 4096 | Reversed           |  10,090.0 ns |  1,036.33 ns |   542.02 ns |  0.66 |    0.03 |    1 |         - |          NA |
| BucketSort          | 4096 | Reversed           |  31,490.2 ns |    729.32 ns |   381.45 ns |  2.05 |    0.03 |    4 |         - |          NA |
| BucketSortInteger   | 4096 | Reversed           |  24,406.4 ns |  1,659.41 ns |   736.79 ns |  1.59 |    0.05 |    3 |         - |          NA |
| FlashSort           | 4096 | Reversed           |  76,171.2 ns |    302.18 ns |   134.17 ns |  4.96 |    0.04 |    6 |         - |          NA |
| RadixLSD4Sort       | 4096 | Reversed           |  78,495.1 ns |  1,223.27 ns |   543.14 ns |  5.11 |    0.05 |    6 |         - |          NA |
| RadixLSD256Sort     | 4096 | Reversed           |  22,245.1 ns |    673.52 ns |   352.27 ns |  1.45 |    0.02 |    3 |         - |          NA |
| RadixLSD10Sort      | 4096 | Reversed           |  84,008.1 ns |  2,087.82 ns | 1,091.97 ns |  5.47 |    0.08 |    6 |         - |          NA |
| RadixMSD4Sort       | 4096 | Reversed           |  75,912.2 ns |  1,153.14 ns |   603.12 ns |  4.95 |    0.05 |    6 |         - |          NA |
| RadixMSD10Sort      | 4096 | Reversed           |  87,016.0 ns |    254.10 ns |   112.82 ns |  5.67 |    0.04 |    6 |         - |          NA |
| AmericanFlagSort    | 4096 | Reversed           |  45,388.5 ns |    953.85 ns |   498.88 ns |  2.96 |    0.04 |    5 |         - |          NA |
| SpreadSort          | 4096 | Reversed           |  19,888.6 ns |    564.46 ns |   250.62 ns |  1.30 |    0.02 |    3 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **4096** | **PipeOrgan**          |  **23,573.9 ns** |    **788.71 ns** |   **350.19 ns** |  **1.30** |    **0.02** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 4096 | PipeOrgan          |  18,127.8 ns |    417.91 ns |   218.58 ns |  1.00 |    0.02 |    2 |         - |          NA |
| PigeonSort          | 4096 | PipeOrgan          |  19,738.3 ns |    278.52 ns |   123.66 ns |  1.09 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 4096 | PipeOrgan          |  11,352.2 ns |    519.27 ns |   230.56 ns |  0.63 |    0.01 |    1 |         - |          NA |
| BucketSort          | 4096 | PipeOrgan          |  31,182.0 ns |    946.97 ns |   420.46 ns |  1.72 |    0.03 |    2 |         - |          NA |
| BucketSortInteger   | 4096 | PipeOrgan          |  26,065.0 ns |    634.53 ns |   331.87 ns |  1.44 |    0.02 |    2 |         - |          NA |
| FlashSort           | 4096 | PipeOrgan          |  75,851.9 ns |  1,041.56 ns |   544.76 ns |  4.18 |    0.06 |    3 |         - |          NA |
| RadixLSD4Sort       | 4096 | PipeOrgan          |  72,732.5 ns |    851.49 ns |   445.35 ns |  4.01 |    0.05 |    3 |         - |          NA |
| RadixLSD256Sort     | 4096 | PipeOrgan          |  23,513.9 ns |    706.39 ns |   313.64 ns |  1.30 |    0.02 |    2 |         - |          NA |
| RadixLSD10Sort      | 4096 | PipeOrgan          |  86,563.0 ns |  3,106.10 ns | 1,624.55 ns |  4.78 |    0.10 |    3 |         - |          NA |
| RadixMSD4Sort       | 4096 | PipeOrgan          |  74,890.5 ns |    432.24 ns |   226.07 ns |  4.13 |    0.05 |    3 |         - |          NA |
| RadixMSD10Sort      | 4096 | PipeOrgan          |  85,362.1 ns |    629.09 ns |   329.03 ns |  4.71 |    0.06 |    3 |         - |          NA |
| AmericanFlagSort    | 4096 | PipeOrgan          |  61,065.7 ns |    859.31 ns |   449.44 ns |  3.37 |    0.04 |    3 |         - |          NA |
| SpreadSort          | 4096 | PipeOrgan          |  30,793.4 ns |    802.27 ns |   419.60 ns |  1.70 |    0.03 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **4096** | **ManyDuplicates**     |  **21,497.1 ns** |    **315.99 ns** |   **140.30 ns** |  **1.63** |    **0.03** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 4096 | ManyDuplicates     |  13,186.5 ns |    381.28 ns |   199.42 ns |  1.00 |    0.02 |    2 |         - |          NA |
| PigeonSort          | 4096 | ManyDuplicates     |  26,659.4 ns |    342.57 ns |   152.10 ns |  2.02 |    0.03 |    5 |         - |          NA |
| PigeonSortInteger   | 4096 | ManyDuplicates     |  10,073.4 ns |    467.09 ns |   166.57 ns |  0.76 |    0.02 |    1 |         - |          NA |
| BucketSort          | 4096 | ManyDuplicates     |  49,970.0 ns |  1,370.81 ns |   716.96 ns |  3.79 |    0.07 |    5 |         - |          NA |
| BucketSortInteger   | 4096 | ManyDuplicates     |  27,669.6 ns |  1,460.29 ns |   763.76 ns |  2.10 |    0.06 |    5 |         - |          NA |
| FlashSort           | 4096 | ManyDuplicates     |  72,885.5 ns |  1,068.09 ns |   474.24 ns |  5.53 |    0.09 |    6 |         - |          NA |
| RadixLSD4Sort       | 4096 | ManyDuplicates     |  36,535.4 ns |    477.11 ns |   249.54 ns |  2.77 |    0.04 |    5 |         - |          NA |
| RadixLSD256Sort     | 4096 | ManyDuplicates     |  16,675.8 ns |  1,183.56 ns |   619.02 ns |  1.26 |    0.05 |    3 |         - |          NA |
| RadixLSD10Sort      | 4096 | ManyDuplicates     |  45,584.2 ns |    236.14 ns |   104.85 ns |  3.46 |    0.05 |    5 |         - |          NA |
| RadixMSD4Sort       | 4096 | ManyDuplicates     |  40,587.1 ns |    649.39 ns |   288.33 ns |  3.08 |    0.05 |    5 |         - |          NA |
| RadixMSD10Sort      | 4096 | ManyDuplicates     |  48,940.5 ns |    595.04 ns |   311.22 ns |  3.71 |    0.06 |    5 |         - |          NA |
| AmericanFlagSort    | 4096 | ManyDuplicates     |  31,638.3 ns |    604.10 ns |   268.22 ns |  2.40 |    0.04 |    5 |         - |          NA |
| SpreadSort          | 4096 | ManyDuplicates     |  26,802.4 ns |    389.92 ns |   203.94 ns |  2.03 |    0.03 |    5 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **8192** | **Random**             |  **51,376.6 ns** |    **902.06 ns** |   **471.79 ns** |  **1.54** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Random             |  33,260.2 ns |    320.52 ns |   142.31 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PigeonSort          | 8192 | Random             |  45,380.2 ns |    548.53 ns |   243.55 ns |  1.36 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 8192 | Random             |  23,393.8 ns |  1,379.02 ns |   721.25 ns |  0.70 |    0.02 |    1 |         - |          NA |
| BucketSort          | 8192 | Random             |  68,291.7 ns |    625.97 ns |   327.39 ns |  2.05 |    0.01 |    4 |         - |          NA |
| BucketSortInteger   | 8192 | Random             |  49,573.3 ns |    229.06 ns |    81.69 ns |  1.49 |    0.01 |    3 |         - |          NA |
| FlashSort           | 8192 | Random             | 167,658.2 ns |  1,793.59 ns |   938.08 ns |  5.04 |    0.03 |    6 |         - |          NA |
| RadixLSD4Sort       | 8192 | Random             | 153,237.0 ns |    724.02 ns |   321.47 ns |  4.61 |    0.02 |    6 |         - |          NA |
| RadixLSD256Sort     | 8192 | Random             |  51,664.2 ns |    710.77 ns |   315.59 ns |  1.55 |    0.01 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | Random             | 171,505.6 ns |  1,503.59 ns |   786.41 ns |  5.16 |    0.03 |    6 |         - |          NA |
| RadixMSD4Sort       | 8192 | Random             | 168,790.4 ns |  2,593.36 ns | 1,356.38 ns |  5.07 |    0.04 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | Random             | 174,816.0 ns |    522.47 ns |   231.98 ns |  5.26 |    0.02 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | Random             | 143,929.5 ns |  1,063.74 ns |   556.36 ns |  4.33 |    0.02 |    6 |         - |          NA |
| SpreadSort          | 8192 | Random             |  96,820.5 ns |    993.99 ns |   519.88 ns |  2.91 |    0.02 |    5 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **8192** | **SingleElementMoved** |  **48,668.9 ns** |  **1,004.20 ns** |   **445.87 ns** |  **1.46** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | SingleElementMoved |  33,312.0 ns |    447.61 ns |   159.62 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PigeonSort          | 8192 | SingleElementMoved |  38,662.2 ns |    469.47 ns |   208.45 ns |  1.16 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | SingleElementMoved |  22,687.0 ns |    959.77 ns |   426.14 ns |  0.68 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | SingleElementMoved |  64,966.7 ns |    476.00 ns |   248.96 ns |  1.95 |    0.01 |    3 |         - |          NA |
| BucketSortInteger   | 8192 | SingleElementMoved |  53,044.3 ns |  1,182.04 ns |   618.23 ns |  1.59 |    0.02 |    3 |         - |          NA |
| FlashSort           | 8192 | SingleElementMoved | 172,160.0 ns |  1,073.46 ns |   561.44 ns |  5.17 |    0.03 |    5 |         - |          NA |
| RadixLSD4Sort       | 8192 | SingleElementMoved | 201,164.4 ns |  1,792.23 ns |   937.37 ns |  6.04 |    0.04 |    5 |         - |          NA |
| RadixLSD256Sort     | 8192 | SingleElementMoved |  48,152.4 ns |  1,127.65 ns |   589.78 ns |  1.45 |    0.02 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | SingleElementMoved | 167,997.6 ns |  2,795.48 ns | 1,462.09 ns |  5.04 |    0.05 |    5 |         - |          NA |
| RadixMSD4Sort       | 8192 | SingleElementMoved | 137,713.2 ns |  1,414.11 ns |   739.61 ns |  4.13 |    0.03 |    5 |         - |          NA |
| RadixMSD10Sort      | 8192 | SingleElementMoved | 157,737.7 ns |  1,761.81 ns |   921.46 ns |  4.74 |    0.03 |    5 |         - |          NA |
| AmericanFlagSort    | 8192 | SingleElementMoved |  94,460.5 ns |    622.27 ns |   325.46 ns |  2.84 |    0.02 |    4 |         - |          NA |
| SpreadSort          | 8192 | SingleElementMoved |  56,647.8 ns |  1,480.03 ns |   774.08 ns |  1.70 |    0.02 |    3 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **8192** | **Sorted**             |  **51,683.7 ns** |  **1,678.20 ns** |   **877.73 ns** |  **1.83** |    **0.04** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Sorted             |  28,207.1 ns |  1,228.27 ns |   545.36 ns |  1.00 |    0.03 |    3 |         - |          NA |
| PigeonSort          | 8192 | Sorted             |  42,268.5 ns |  1,276.81 ns |   566.91 ns |  1.50 |    0.03 |    4 |         - |          NA |
| PigeonSortInteger   | 8192 | Sorted             |  19,817.6 ns |    478.95 ns |   212.65 ns |  0.70 |    0.01 |    2 |         - |          NA |
| BucketSort          | 8192 | Sorted             |  63,610.7 ns |  2,279.38 ns | 1,192.16 ns |  2.26 |    0.06 |    5 |         - |          NA |
| BucketSortInteger   | 8192 | Sorted             |  48,270.0 ns |  5,741.35 ns | 2,549.19 ns |  1.71 |    0.09 |    4 |         - |          NA |
| FlashSort           | 8192 | Sorted             | 171,396.9 ns |    870.65 ns |   455.37 ns |  6.08 |    0.11 |    6 |         - |          NA |
| RadixLSD4Sort       | 8192 | Sorted             | 209,369.3 ns |  1,417.50 ns |   741.38 ns |  7.42 |    0.13 |    7 |         - |          NA |
| RadixLSD256Sort     | 8192 | Sorted             |  49,427.7 ns |  2,015.55 ns | 1,054.17 ns |  1.75 |    0.05 |    4 |         - |          NA |
| RadixLSD10Sort      | 8192 | Sorted             | 167,931.1 ns |  2,803.56 ns |   999.78 ns |  5.96 |    0.11 |    6 |         - |          NA |
| RadixMSD4Sort       | 8192 | Sorted             | 137,683.8 ns |  1,304.97 ns |   682.53 ns |  4.88 |    0.09 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | Sorted             | 158,038.3 ns |    687.21 ns |   245.07 ns |  5.60 |    0.10 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | Sorted             |  70,080.1 ns |    504.39 ns |   263.80 ns |  2.49 |    0.04 |    5 |         - |          NA |
| SpreadSort          | 8192 | Sorted             |   4,451.5 ns |     11.92 ns |     4.25 ns |  0.16 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **8192** | **Reversed**           |  **45,179.8 ns** |    **874.79 ns** |   **457.53 ns** |  **1.47** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Reversed           |  30,782.8 ns |    246.28 ns |    87.82 ns |  1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | Reversed           |  39,029.3 ns |  1,051.23 ns |   549.81 ns |  1.27 |    0.02 |    3 |         - |          NA |
| PigeonSortInteger   | 8192 | Reversed           |  19,769.6 ns |    498.85 ns |   221.49 ns |  0.64 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | Reversed           |  61,087.8 ns |  2,576.98 ns | 1,347.81 ns |  1.98 |    0.04 |    4 |         - |          NA |
| BucketSortInteger   | 8192 | Reversed           |  47,596.9 ns |  1,595.73 ns |   834.60 ns |  1.55 |    0.03 |    3 |         - |          NA |
| FlashSort           | 8192 | Reversed           | 153,014.0 ns |  1,083.74 ns |   481.19 ns |  4.97 |    0.02 |    6 |         - |          NA |
| RadixLSD4Sort       | 8192 | Reversed           | 211,377.5 ns |  2,452.48 ns | 1,282.69 ns |  6.87 |    0.04 |    6 |         - |          NA |
| RadixLSD256Sort     | 8192 | Reversed           |  46,636.2 ns |    826.20 ns |   432.12 ns |  1.52 |    0.01 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | Reversed           | 167,798.4 ns |  1,751.31 ns |   777.59 ns |  5.45 |    0.03 |    6 |         - |          NA |
| RadixMSD4Sort       | 8192 | Reversed           | 169,468.3 ns |  1,544.91 ns |   550.93 ns |  5.51 |    0.02 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | Reversed           | 176,554.4 ns |  1,329.75 ns |   695.49 ns |  5.74 |    0.03 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | Reversed           |  89,345.8 ns |  1,052.66 ns |   550.56 ns |  2.90 |    0.02 |    5 |         - |          NA |
| SpreadSort          | 8192 | Reversed           |  76,913.4 ns |  1,440.47 ns |   753.39 ns |  2.50 |    0.02 |    5 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **8192** | **PipeOrgan**          |  **46,366.0 ns** |  **1,028.73 ns** |   **538.05 ns** |  **1.31** |    **0.02** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | PipeOrgan          |  35,329.4 ns |    702.56 ns |   311.94 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PigeonSort          | 8192 | PipeOrgan          |  39,814.0 ns |  1,185.86 ns |   620.23 ns |  1.13 |    0.02 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | PipeOrgan          |  22,633.4 ns |    178.84 ns |    63.78 ns |  0.64 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | PipeOrgan          |  62,759.7 ns |    778.39 ns |   407.11 ns |  1.78 |    0.02 |    3 |         - |          NA |
| BucketSortInteger   | 8192 | PipeOrgan          |  51,442.5 ns |    947.52 ns |   495.57 ns |  1.46 |    0.02 |    2 |         - |          NA |
| FlashSort           | 8192 | PipeOrgan          | 162,435.3 ns |  1,119.21 ns |   496.93 ns |  4.60 |    0.04 |    6 |         - |          NA |
| RadixLSD4Sort       | 8192 | PipeOrgan          | 174,368.5 ns |  1,278.36 ns |   668.61 ns |  4.94 |    0.04 |    6 |         - |          NA |
| RadixLSD256Sort     | 8192 | PipeOrgan          |  47,839.2 ns |    762.22 ns |   398.66 ns |  1.35 |    0.02 |    2 |         - |          NA |
| RadixLSD10Sort      | 8192 | PipeOrgan          | 166,870.7 ns |  2,060.19 ns | 1,077.52 ns |  4.72 |    0.05 |    6 |         - |          NA |
| RadixMSD4Sort       | 8192 | PipeOrgan          | 170,944.9 ns |  1,374.01 ns |   610.07 ns |  4.84 |    0.04 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | PipeOrgan          | 171,523.0 ns |  2,430.96 ns | 1,271.44 ns |  4.86 |    0.05 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | PipeOrgan          | 122,265.2 ns |    375.58 ns |   166.76 ns |  3.46 |    0.03 |    5 |         - |          NA |
| SpreadSort          | 8192 | PipeOrgan          |  95,932.9 ns |    560.43 ns |   248.83 ns |  2.72 |    0.02 |    4 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **8192** | **ManyDuplicates**     |  **44,376.0 ns** |  **1,472.17 ns** |   **769.97 ns** |  **1.63** |    **0.03** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 8192 | ManyDuplicates     |  27,170.1 ns |    883.82 ns |   392.42 ns |  1.00 |    0.02 |    2 |         - |          NA |
| PigeonSort          | 8192 | ManyDuplicates     |  75,576.0 ns |    814.75 ns |   426.13 ns |  2.78 |    0.04 |    5 |         - |          NA |
| PigeonSortInteger   | 8192 | ManyDuplicates     |  20,326.5 ns |    556.01 ns |   290.80 ns |  0.75 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | ManyDuplicates     |  97,314.8 ns |    709.88 ns |   371.28 ns |  3.58 |    0.05 |    5 |         - |          NA |
| BucketSortInteger   | 8192 | ManyDuplicates     |  54,269.9 ns |    859.19 ns |   381.48 ns |  2.00 |    0.03 |    5 |         - |          NA |
| FlashSort           | 8192 | ManyDuplicates     | 147,942.6 ns |  1,015.36 ns |   531.05 ns |  5.45 |    0.08 |    6 |         - |          NA |
| RadixLSD4Sort       | 8192 | ManyDuplicates     |  73,078.1 ns |    848.06 ns |   443.55 ns |  2.69 |    0.04 |    5 |         - |          NA |
| RadixLSD256Sort     | 8192 | ManyDuplicates     |  33,148.9 ns |  1,040.41 ns |   544.16 ns |  1.22 |    0.02 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | ManyDuplicates     |  92,016.9 ns |  1,279.19 ns |   669.04 ns |  3.39 |    0.05 |    5 |         - |          NA |
| RadixMSD4Sort       | 8192 | ManyDuplicates     |  80,437.6 ns |    522.15 ns |   273.09 ns |  2.96 |    0.04 |    5 |         - |          NA |
| RadixMSD10Sort      | 8192 | ManyDuplicates     |  98,807.7 ns |  1,118.99 ns |   496.84 ns |  3.64 |    0.05 |    5 |         - |          NA |
| AmericanFlagSort    | 8192 | ManyDuplicates     |  61,322.3 ns |    394.49 ns |   206.33 ns |  2.26 |    0.03 |    5 |         - |          NA |
| SpreadSort          | 8192 | ManyDuplicates     |  53,272.1 ns |    924.25 ns |   483.40 ns |  1.96 |    0.03 |    5 |         - |          NA |

### ExchangeBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method             | Size | Pattern            | Mean         | Error       | StdDev      | Ratio  | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------- |----- |------------------- |-------------:|------------:|------------:|-------:|--------:|-----:|----------:|------------:|
| **BubbleSort**         | **256**  | **Random**             |  **27,986.0 ns** |   **228.52 ns** |   **101.46 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Random             |  16,790.4 ns |   205.57 ns |    91.27 ns |   0.60 |    0.00 |    2 |         - |          NA |
| OddEvenSort        | 256  | Random             |  21,371.3 ns |   967.49 ns |   506.01 ns |   0.76 |    0.02 |    2 |         - |          NA |
| CombSort           | 256  | Random             |   3,679.0 ns |   380.62 ns |   199.07 ns |   0.13 |    0.01 |    1 |         - |          NA |
| CircleSort         | 256  | Random             |  18,421.2 ns |   180.39 ns |    94.35 ns |   0.66 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **SingleElementMoved** |     **414.2 ns** |     **2.86 ns** |     **1.27 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 256  | SingleElementMoved |     318.1 ns |     3.48 ns |     1.82 ns |   0.77 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 256  | SingleElementMoved |  17,424.1 ns |   160.88 ns |    71.43 ns |  42.07 |    0.20 |    4 |         - |          NA |
| CombSort           | 256  | SingleElementMoved |   2,845.4 ns |    15.90 ns |     5.67 ns |   6.87 |    0.02 |    3 |         - |          NA |
| CircleSort         | 256  | SingleElementMoved |  15,503.6 ns |   142.34 ns |    63.20 ns |  37.43 |    0.18 |    4 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **Sorted**             |     **187.2 ns** |     **1.09 ns** |     **0.49 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Sorted             |     192.1 ns |    13.24 ns |     5.88 ns |   1.03 |    0.03 |    1 |         - |          NA |
| OddEvenSort        | 256  | Sorted             |     182.6 ns |     2.59 ns |     1.36 ns |   0.98 |    0.01 |    1 |         - |          NA |
| CombSort           | 256  | Sorted             |   2,779.7 ns |    38.86 ns |    13.86 ns |  14.85 |    0.08 |    3 |         - |          NA |
| CircleSort         | 256  | Sorted             |   2,184.1 ns |    24.69 ns |    10.96 ns |  11.67 |    0.06 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **Reversed**           |  **29,416.4 ns** |   **111.35 ns** |    **58.24 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Reversed           |  27,743.5 ns |   408.40 ns |   213.60 ns |   0.94 |    0.01 |    3 |         - |          NA |
| OddEvenSort        | 256  | Reversed           |  25,282.9 ns |   164.39 ns |    85.98 ns |   0.86 |    0.00 |    3 |         - |          NA |
| CombSort           | 256  | Reversed           |   3,477.3 ns | 1,562.06 ns |   816.99 ns |   0.12 |    0.03 |    1 |         - |          NA |
| CircleSort         | 256  | Reversed           |   4,301.4 ns |    48.84 ns |    17.42 ns |   0.15 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **PipeOrgan**          |  **34,225.1 ns** |   **151.86 ns** |    **79.43 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 256  | PipeOrgan          |  17,878.4 ns |   115.02 ns |    60.16 ns |   0.52 |    0.00 |    2 |         - |          NA |
| OddEvenSort        | 256  | PipeOrgan          |  28,107.2 ns |   561.00 ns |   293.42 ns |   0.82 |    0.01 |    3 |         - |          NA |
| CombSort           | 256  | PipeOrgan          |   3,075.6 ns |   253.40 ns |   132.53 ns |   0.09 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | PipeOrgan          |  19,799.8 ns |   376.85 ns |   134.39 ns |   0.58 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **ManyDuplicates**     |  **29,075.9 ns** |   **360.29 ns** |   **159.97 ns** |   **1.00** |    **0.01** |    **5** |         **-** |          **NA** |
| CocktailShakerSort | 256  | ManyDuplicates     |  17,176.8 ns |   215.45 ns |   112.68 ns |   0.59 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 256  | ManyDuplicates     |  21,485.1 ns |   833.08 ns |   435.72 ns |   0.74 |    0.01 |    4 |         - |          NA |
| CombSort           | 256  | ManyDuplicates     |   3,271.1 ns |    46.52 ns |    20.65 ns |   0.11 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | ManyDuplicates     |  13,618.8 ns |   276.00 ns |   144.35 ns |   0.47 |    0.01 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Random**             | **535,116.7 ns** | **3,587.56 ns** | **1,876.37 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Random             | 324,951.2 ns | 1,121.52 ns |   586.57 ns |   0.61 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | Random             | 472,676.3 ns | 3,330.23 ns | 1,741.77 ns |   0.88 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | Random             |  19,532.2 ns |   209.02 ns |    92.81 ns |   0.04 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | Random             | 100,665.9 ns | 1,839.42 ns |   962.05 ns |   0.19 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **SingleElementMoved** |   **1,700.9 ns** |    **10.60 ns** |     **4.71 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | SingleElementMoved |   1,294.0 ns |     4.04 ns |     1.79 ns |   0.76 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 1024 | SingleElementMoved | 251,562.3 ns |   819.73 ns |   428.74 ns | 147.90 |    0.45 |    5 |         - |          NA |
| CombSort           | 1024 | SingleElementMoved |  15,926.9 ns | 2,017.09 ns |   895.60 ns |   9.36 |    0.49 |    3 |         - |          NA |
| CircleSort         | 1024 | SingleElementMoved |  85,846.0 ns |   385.61 ns |   171.21 ns |  50.47 |    0.16 |    4 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Sorted**             |     **723.6 ns** |     **1.28 ns** |     **0.57 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Sorted             |     734.5 ns |     2.14 ns |     0.95 ns |   1.02 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 1024 | Sorted             |     743.3 ns |     1.22 ns |     0.64 ns |   1.03 |    0.00 |    1 |         - |          NA |
| CombSort           | 1024 | Sorted             |  14,519.0 ns |    78.14 ns |    40.87 ns |  20.07 |    0.06 |    3 |         - |          NA |
| CircleSort         | 1024 | Sorted             |   9,958.4 ns |   357.16 ns |   186.80 ns |  13.76 |    0.24 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Reversed**           | **437,704.5 ns** | **1,458.92 ns** |   **763.04 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Reversed           | 436,399.6 ns | 1,432.98 ns |   636.25 ns |   1.00 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | Reversed           | 408,843.6 ns | 3,408.78 ns | 1,782.86 ns |   0.93 |    0.00 |    3 |         - |          NA |
| CombSort           | 1024 | Reversed           |  15,682.9 ns |   269.02 ns |   119.44 ns |   0.04 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | Reversed           |  19,512.9 ns |   233.24 ns |   121.99 ns |   0.04 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **PipeOrgan**          | **506,652.6 ns** |   **975.22 ns** |   **433.00 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | PipeOrgan          | 284,761.4 ns | 3,592.30 ns | 1,878.84 ns |   0.56 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | PipeOrgan          | 448,737.5 ns |   762.66 ns |   398.89 ns |   0.89 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | PipeOrgan          |  16,040.9 ns |   195.19 ns |    86.66 ns |   0.03 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | PipeOrgan          | 105,773.2 ns |   837.64 ns |   438.10 ns |   0.21 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **ManyDuplicates**     | **539,399.3 ns** | **1,713.49 ns** |   **760.80 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | ManyDuplicates     | 319,263.6 ns | 2,399.48 ns | 1,254.97 ns |   0.59 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | ManyDuplicates     | 468,617.6 ns | 1,739.67 ns |   772.42 ns |   0.87 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | ManyDuplicates     |  16,836.9 ns |   132.74 ns |    69.43 ns |   0.03 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | ManyDuplicates     |  90,826.4 ns |   717.40 ns |   375.21 ns |   0.17 |    0.00 |    2 |         - |          NA |

### HeapBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method           | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **HeapSort**         | **256**  | **Random**             |     **3,403.4 ns** |    **176.27 ns** |     **78.26 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | Random             |     3,435.3 ns |     60.57 ns |     26.89 ns |  1.01 |    0.02 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | Random             |     4,108.7 ns |    430.89 ns |    225.36 ns |  1.21 |    0.07 |    1 |         - |          NA |
| BottomupHeapSort | 256  | Random             |     4,431.8 ns |    324.98 ns |    169.97 ns |  1.30 |    0.05 |    1 |         - |          NA |
| WeakHeapSort     | 256  | Random             |    10,221.7 ns |    263.03 ns |    137.57 ns |  3.00 |    0.07 |    4 |         - |          NA |
| SmoothSort       | 256  | Random             |     5,621.8 ns |    513.75 ns |    268.70 ns |  1.65 |    0.08 |    2 |         - |          NA |
| TournamentSort   | 256  | Random             |     8,359.3 ns |    212.39 ns |     94.30 ns |  2.46 |    0.06 |    3 |         - |          NA |
| BinomialHeapSort | 256  | Random             |    15,890.2 ns |    679.83 ns |    355.57 ns |  4.67 |    0.14 |    5 |         - |          NA |
| PairingHeapSort  | 256  | Random             |    10,942.4 ns |    516.46 ns |    270.12 ns |  3.22 |    0.10 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **SingleElementMoved** |     **3,164.0 ns** |    **305.08 ns** |    **159.56 ns** |  **1.00** |    **0.07** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 256  | SingleElementMoved |     3,226.1 ns |    355.02 ns |    185.68 ns |  1.02 |    0.07 |    2 |         - |          NA |
| TernaryHeapSort  | 256  | SingleElementMoved |     4,281.6 ns |    325.09 ns |    170.03 ns |  1.36 |    0.08 |    3 |         - |          NA |
| BottomupHeapSort | 256  | SingleElementMoved |     4,395.3 ns |    239.59 ns |    125.31 ns |  1.39 |    0.07 |    3 |         - |          NA |
| WeakHeapSort     | 256  | SingleElementMoved |     8,766.4 ns |     66.65 ns |     34.86 ns |  2.78 |    0.13 |    5 |         - |          NA |
| SmoothSort       | 256  | SingleElementMoved |     1,733.0 ns |     14.24 ns |      6.32 ns |  0.55 |    0.03 |    1 |         - |          NA |
| TournamentSort   | 256  | SingleElementMoved |     5,517.1 ns |    247.52 ns |    129.46 ns |  1.75 |    0.09 |    4 |         - |          NA |
| BinomialHeapSort | 256  | SingleElementMoved |     7,315.9 ns |     59.04 ns |     30.88 ns |  2.32 |    0.11 |    5 |         - |          NA |
| PairingHeapSort  | 256  | SingleElementMoved |     5,550.3 ns |    203.98 ns |     90.57 ns |  1.76 |    0.09 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **Sorted**             |     **3,345.2 ns** |    **183.58 ns** |     **81.51 ns** |  **1.00** |    **0.03** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 256  | Sorted             |     3,530.1 ns |     98.60 ns |     43.78 ns |  1.06 |    0.03 |    2 |         - |          NA |
| TernaryHeapSort  | 256  | Sorted             |     4,239.3 ns |    303.04 ns |    158.50 ns |  1.27 |    0.05 |    2 |         - |          NA |
| BottomupHeapSort | 256  | Sorted             |     4,428.4 ns |    280.27 ns |    146.59 ns |  1.32 |    0.05 |    2 |         - |          NA |
| WeakHeapSort     | 256  | Sorted             |     8,833.3 ns |    366.29 ns |    191.58 ns |  2.64 |    0.08 |    5 |         - |          NA |
| SmoothSort       | 256  | Sorted             |     1,290.4 ns |      9.78 ns |      4.34 ns |  0.39 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 256  | Sorted             |     4,408.9 ns |     99.98 ns |     35.65 ns |  1.32 |    0.03 |    2 |         - |          NA |
| BinomialHeapSort | 256  | Sorted             |     6,764.8 ns |    434.72 ns |    227.37 ns |  2.02 |    0.08 |    4 |         - |          NA |
| PairingHeapSort  | 256  | Sorted             |     5,615.6 ns |    415.65 ns |    217.39 ns |  1.68 |    0.07 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **Reversed**           |     **3,337.1 ns** |    **310.68 ns** |    **162.49 ns** |  **1.00** |    **0.06** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 256  | Reversed           |     2,667.9 ns |     62.20 ns |     22.18 ns |  0.80 |    0.04 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | Reversed           |     4,444.2 ns |    260.54 ns |    136.27 ns |  1.33 |    0.07 |    3 |         - |          NA |
| BottomupHeapSort | 256  | Reversed           |     4,546.2 ns |    226.56 ns |    100.60 ns |  1.37 |    0.07 |    3 |         - |          NA |
| WeakHeapSort     | 256  | Reversed           |     9,634.0 ns |    388.38 ns |    203.13 ns |  2.89 |    0.14 |    5 |         - |          NA |
| SmoothSort       | 256  | Reversed           |     4,892.5 ns |    194.63 ns |     86.42 ns |  1.47 |    0.07 |    3 |         - |          NA |
| TournamentSort   | 256  | Reversed           |     5,327.2 ns |    509.56 ns |    266.51 ns |  1.60 |    0.11 |    3 |         - |          NA |
| BinomialHeapSort | 256  | Reversed           |     6,700.4 ns |    339.59 ns |    177.61 ns |  2.01 |    0.10 |    4 |         - |          NA |
| PairingHeapSort  | 256  | Reversed           |     2,655.5 ns |     14.49 ns |      5.17 ns |  0.80 |    0.04 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **PipeOrgan**          |     **2,993.3 ns** |     **93.57 ns** |     **33.37 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | PipeOrgan          |     3,081.8 ns |    266.33 ns |    139.29 ns |  1.03 |    0.05 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | PipeOrgan          |     4,309.7 ns |    264.46 ns |    138.32 ns |  1.44 |    0.05 |    2 |         - |          NA |
| BottomupHeapSort | 256  | PipeOrgan          |     4,763.8 ns |    854.28 ns |    379.31 ns |  1.59 |    0.12 |    2 |         - |          NA |
| WeakHeapSort     | 256  | PipeOrgan          |     9,035.0 ns |     33.77 ns |     14.99 ns |  3.02 |    0.03 |    4 |         - |          NA |
| SmoothSort       | 256  | PipeOrgan          |     5,012.8 ns |    324.06 ns |    169.49 ns |  1.67 |    0.06 |    2 |         - |          NA |
| TournamentSort   | 256  | PipeOrgan          |     6,516.1 ns |    325.91 ns |    170.45 ns |  2.18 |    0.06 |    3 |         - |          NA |
| BinomialHeapSort | 256  | PipeOrgan          |     7,496.6 ns |     78.15 ns |     34.70 ns |  2.50 |    0.03 |    3 |         - |          NA |
| PairingHeapSort  | 256  | PipeOrgan          |     7,059.2 ns |    377.38 ns |    197.38 ns |  2.36 |    0.07 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **ManyDuplicates**     |     **3,425.9 ns** |    **320.34 ns** |    **167.54 ns** |  **1.00** |    **0.06** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | ManyDuplicates     |     3,472.7 ns |    211.85 ns |    110.80 ns |  1.02 |    0.05 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | ManyDuplicates     |     4,186.1 ns |    344.64 ns |    180.25 ns |  1.22 |    0.07 |    2 |         - |          NA |
| BottomupHeapSort | 256  | ManyDuplicates     |     4,426.3 ns |    323.37 ns |    169.13 ns |  1.29 |    0.07 |    2 |         - |          NA |
| WeakHeapSort     | 256  | ManyDuplicates     |     9,776.8 ns |    289.93 ns |    128.73 ns |  2.86 |    0.13 |    3 |         - |          NA |
| SmoothSort       | 256  | ManyDuplicates     |     5,113.5 ns |    259.66 ns |    135.81 ns |  1.50 |    0.08 |    2 |         - |          NA |
| TournamentSort   | 256  | ManyDuplicates     |     8,528.8 ns |    406.67 ns |    212.70 ns |  2.49 |    0.13 |    3 |         - |          NA |
| BinomialHeapSort | 256  | ManyDuplicates     |    13,718.7 ns |    529.69 ns |    277.04 ns |  4.01 |    0.19 |    4 |         - |          NA |
| PairingHeapSort  | 256  | ManyDuplicates     |    10,895.1 ns |    244.51 ns |    127.88 ns |  3.19 |    0.15 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Random**             |    **18,467.9 ns** |    **243.48 ns** |    **108.11 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Random             |    17,889.7 ns |    445.73 ns |    197.91 ns |  0.97 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | Random             |    20,167.0 ns |  1,087.82 ns |    568.95 ns |  1.09 |    0.03 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | Random             |    20,371.5 ns |    368.47 ns |    163.60 ns |  1.10 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | Random             |    53,394.0 ns |    341.03 ns |    151.42 ns |  2.89 |    0.02 |    4 |         - |          NA |
| SmoothSort       | 1024 | Random             |    27,996.2 ns |    913.60 ns |    477.83 ns |  1.52 |    0.03 |    2 |         - |          NA |
| TournamentSort   | 1024 | Random             |    40,340.7 ns |  1,826.46 ns |    810.96 ns |  2.18 |    0.04 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | Random             |    85,100.8 ns |  4,668.41 ns |  2,441.67 ns |  4.61 |    0.13 |    5 |         - |          NA |
| PairingHeapSort  | 1024 | Random             |    55,553.6 ns |  1,416.09 ns |    628.75 ns |  3.01 |    0.04 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **SingleElementMoved** |    **15,468.6 ns** |    **633.09 ns** |    **331.12 ns** |  **1.00** |    **0.03** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | SingleElementMoved |    15,528.7 ns |    493.93 ns |    258.34 ns |  1.00 |    0.03 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | SingleElementMoved |    20,540.8 ns |    264.56 ns |    117.47 ns |  1.33 |    0.03 |    3 |         - |          NA |
| BottomupHeapSort | 1024 | SingleElementMoved |    20,349.1 ns |    604.27 ns |    316.05 ns |  1.32 |    0.03 |    3 |         - |          NA |
| WeakHeapSort     | 1024 | SingleElementMoved |    43,781.4 ns |    186.88 ns |     82.98 ns |  2.83 |    0.06 |    5 |         - |          NA |
| SmoothSort       | 1024 | SingleElementMoved |     7,169.5 ns |    347.83 ns |    181.92 ns |  0.46 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 1024 | SingleElementMoved |    27,048.8 ns |  2,716.81 ns |  1,206.28 ns |  1.75 |    0.08 |    4 |         - |          NA |
| BinomialHeapSort | 1024 | SingleElementMoved |    32,152.4 ns |    188.52 ns |     98.60 ns |  2.08 |    0.04 |    4 |         - |          NA |
| PairingHeapSort  | 1024 | SingleElementMoved |    22,419.0 ns |    384.62 ns |    170.77 ns |  1.45 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Sorted**             |    **16,663.1 ns** |    **433.15 ns** |    **226.54 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Sorted             |    17,192.9 ns |    182.03 ns |     80.82 ns |  1.03 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | Sorted             |    19,652.7 ns |    716.85 ns |    374.93 ns |  1.18 |    0.03 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | Sorted             |    20,176.7 ns |    400.69 ns |    177.91 ns |  1.21 |    0.02 |    2 |         - |          NA |
| WeakHeapSort     | 1024 | Sorted             |    44,356.3 ns |    219.28 ns |    114.69 ns |  2.66 |    0.03 |    4 |         - |          NA |
| SmoothSort       | 1024 | Sorted             |     5,285.2 ns |    314.95 ns |    164.73 ns |  0.32 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 1024 | Sorted             |    20,654.3 ns |    797.24 ns |    416.97 ns |  1.24 |    0.03 |    2 |         - |          NA |
| BinomialHeapSort | 1024 | Sorted             |    29,398.1 ns |    334.70 ns |    175.05 ns |  1.76 |    0.02 |    3 |         - |          NA |
| PairingHeapSort  | 1024 | Sorted             |    22,086.6 ns |    163.04 ns |     85.27 ns |  1.33 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Reversed**           |    **15,288.6 ns** |    **460.53 ns** |    **240.86 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Reversed           |    15,394.9 ns |    688.11 ns |    359.89 ns |  1.01 |    0.03 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | Reversed           |    20,533.0 ns |    281.96 ns |    125.19 ns |  1.34 |    0.02 |    3 |         - |          NA |
| BottomupHeapSort | 1024 | Reversed           |    21,050.0 ns |    595.31 ns |    311.36 ns |  1.38 |    0.03 |    3 |         - |          NA |
| WeakHeapSort     | 1024 | Reversed           |    48,259.4 ns |    187.08 ns |     66.71 ns |  3.16 |    0.05 |    4 |         - |          NA |
| SmoothSort       | 1024 | Reversed           |    24,693.7 ns |    817.08 ns |    427.35 ns |  1.62 |    0.04 |    3 |         - |          NA |
| TournamentSort   | 1024 | Reversed           |    25,840.3 ns |  2,551.55 ns |  1,334.51 ns |  1.69 |    0.09 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | Reversed           |    28,892.2 ns |    214.96 ns |     95.44 ns |  1.89 |    0.03 |    3 |         - |          NA |
| PairingHeapSort  | 1024 | Reversed           |    10,692.5 ns |    404.09 ns |    211.35 ns |  0.70 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **PipeOrgan**          |    **15,440.7 ns** |    **305.40 ns** |    **135.60 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | PipeOrgan          |    15,211.8 ns |    523.49 ns |    232.43 ns |  0.99 |    0.02 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | PipeOrgan          |    20,254.4 ns |    567.31 ns |    296.71 ns |  1.31 |    0.02 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | PipeOrgan          |    20,397.7 ns |    557.57 ns |    247.56 ns |  1.32 |    0.02 |    2 |         - |          NA |
| WeakHeapSort     | 1024 | PipeOrgan          |    47,610.5 ns |    212.33 ns |    111.05 ns |  3.08 |    0.03 |    4 |         - |          NA |
| SmoothSort       | 1024 | PipeOrgan          |    26,504.8 ns |    357.97 ns |    158.94 ns |  1.72 |    0.02 |    3 |         - |          NA |
| TournamentSort   | 1024 | PipeOrgan          |    34,372.8 ns |  5,162.19 ns |  2,699.93 ns |  2.23 |    0.17 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | PipeOrgan          |    32,793.2 ns |    411.18 ns |    182.56 ns |  2.12 |    0.02 |    3 |         - |          NA |
| PairingHeapSort  | 1024 | PipeOrgan          |    29,412.1 ns |    265.20 ns |    138.71 ns |  1.90 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **ManyDuplicates**     |    **17,919.5 ns** |    **349.59 ns** |    **155.22 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | ManyDuplicates     |    17,778.6 ns |    143.09 ns |     63.53 ns |  0.99 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | ManyDuplicates     |    19,316.8 ns |    339.77 ns |    150.86 ns |  1.08 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | ManyDuplicates     |    19,859.1 ns |    517.20 ns |    229.64 ns |  1.11 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | ManyDuplicates     |    48,323.1 ns |    436.39 ns |    193.76 ns |  2.70 |    0.02 |    3 |         - |          NA |
| SmoothSort       | 1024 | ManyDuplicates     |    24,688.9 ns |    823.84 ns |    430.89 ns |  1.38 |    0.03 |    2 |         - |          NA |
| TournamentSort   | 1024 | ManyDuplicates     |    40,279.1 ns |  3,234.35 ns |  1,691.63 ns |  2.25 |    0.09 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | ManyDuplicates     |    66,417.7 ns |  2,719.62 ns |  1,207.53 ns |  3.71 |    0.07 |    4 |         - |          NA |
| PairingHeapSort  | 1024 | ManyDuplicates     |    52,482.4 ns |  1,662.73 ns |    869.64 ns |  2.93 |    0.05 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Random**             |   **183,723.0 ns** |  **1,910.22 ns** |    **999.08 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Random             |   190,656.8 ns |  3,975.81 ns |  2,079.43 ns |  1.04 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 4096 | Random             |   141,451.4 ns | 20,923.14 ns | 10,943.21 ns |  0.77 |    0.06 |    1 |         - |          NA |
| BottomupHeapSort | 4096 | Random             |   129,506.0 ns |  4,039.12 ns |  1,793.40 ns |  0.70 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | Random             |   347,729.2 ns | 40,913.06 ns | 21,398.32 ns |  1.89 |    0.11 |    3 |         - |          NA |
| SmoothSort       | 4096 | Random             |   388,287.8 ns |  3,389.03 ns |  1,504.75 ns |  2.11 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 4096 | Random             |   674,175.8 ns | 12,604.94 ns |  6,592.63 ns |  3.67 |    0.04 |    4 |         - |          NA |
| BinomialHeapSort | 4096 | Random             | 1,044,476.5 ns |  5,525.63 ns |  2,890.01 ns |  5.69 |    0.03 |    5 |         - |          NA |
| PairingHeapSort  | 4096 | Random             |   464,099.2 ns |  3,373.34 ns |  1,497.78 ns |  2.53 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **SingleElementMoved** |   **105,155.9 ns** |  **3,754.69 ns** |  **1,963.78 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 4096 | SingleElementMoved |   138,938.3 ns |  2,407.00 ns |  1,068.72 ns |  1.32 |    0.03 |    3 |         - |          NA |
| TernaryHeapSort  | 4096 | SingleElementMoved |   101,538.2 ns |  1,825.21 ns |    810.40 ns |  0.97 |    0.02 |    2 |         - |          NA |
| BottomupHeapSort | 4096 | SingleElementMoved |   105,944.9 ns |  2,287.31 ns |  1,015.58 ns |  1.01 |    0.02 |    2 |         - |          NA |
| WeakHeapSort     | 4096 | SingleElementMoved |   214,143.2 ns |  1,170.81 ns |    519.85 ns |  2.04 |    0.04 |    4 |         - |          NA |
| SmoothSort       | 4096 | SingleElementMoved |    29,769.6 ns |  1,634.92 ns |    855.10 ns |  0.28 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 4096 | SingleElementMoved |   300,667.0 ns | 20,930.55 ns |  9,293.29 ns |  2.86 |    0.10 |    5 |         - |          NA |
| BinomialHeapSort | 4096 | SingleElementMoved |   142,449.7 ns |    801.53 ns |    355.89 ns |  1.36 |    0.02 |    3 |         - |          NA |
| PairingHeapSort  | 4096 | SingleElementMoved |    90,483.8 ns |    494.29 ns |    219.47 ns |  0.86 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Sorted**             |   **126,003.6 ns** |  **3,038.00 ns** |  **1,588.93 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Sorted             |   156,852.0 ns |  2,442.98 ns |  1,084.70 ns |  1.24 |    0.02 |    3 |         - |          NA |
| TernaryHeapSort  | 4096 | Sorted             |    98,578.6 ns |  6,226.66 ns |  3,256.66 ns |  0.78 |    0.03 |    2 |         - |          NA |
| BottomupHeapSort | 4096 | Sorted             |   100,499.4 ns |  1,962.13 ns |    871.20 ns |  0.80 |    0.01 |    2 |         - |          NA |
| WeakHeapSort     | 4096 | Sorted             |   216,035.1 ns |  1,056.76 ns |    469.21 ns |  1.71 |    0.02 |    4 |         - |          NA |
| SmoothSort       | 4096 | Sorted             |    21,130.7 ns |    693.14 ns |    307.76 ns |  0.17 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 4096 | Sorted             |   151,948.1 ns | 19,773.39 ns |  8,779.51 ns |  1.21 |    0.07 |    3 |         - |          NA |
| BinomialHeapSort | 4096 | Sorted             |   130,948.1 ns |    258.58 ns |    114.81 ns |  1.04 |    0.01 |    3 |         - |          NA |
| PairingHeapSort  | 4096 | Sorted             |    90,238.7 ns |    738.18 ns |    327.76 ns |  0.72 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Reversed**           |   **136,866.5 ns** | **20,543.39 ns** | **10,744.59 ns** |  **1.01** |    **0.11** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Reversed           |   130,936.6 ns |  2,337.10 ns |  1,037.69 ns |  0.96 |    0.08 |    3 |         - |          NA |
| TernaryHeapSort  | 4096 | Reversed           |    97,605.7 ns |    837.48 ns |    371.85 ns |  0.72 |    0.06 |    2 |         - |          NA |
| BottomupHeapSort | 4096 | Reversed           |   103,591.5 ns |  1,085.89 ns |    482.14 ns |  0.76 |    0.06 |    2 |         - |          NA |
| WeakHeapSort     | 4096 | Reversed           |   233,157.4 ns |    812.84 ns |    425.13 ns |  1.71 |    0.14 |    4 |         - |          NA |
| SmoothSort       | 4096 | Reversed           |   131,796.6 ns |  2,207.20 ns |    980.01 ns |  0.97 |    0.08 |    3 |         - |          NA |
| TournamentSort   | 4096 | Reversed           |   239,307.3 ns | 23,015.82 ns | 12,037.72 ns |  1.76 |    0.16 |    4 |         - |          NA |
| BinomialHeapSort | 4096 | Reversed           |   127,633.5 ns |    208.57 ns |    109.09 ns |  0.94 |    0.07 |    3 |         - |          NA |
| PairingHeapSort  | 4096 | Reversed           |    42,568.6 ns |    856.85 ns |    448.15 ns |  0.31 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **PipeOrgan**          |   **104,569.8 ns** | **18,825.87 ns** |  **6,713.49 ns** |  **1.00** |    **0.09** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 4096 | PipeOrgan          |   122,354.6 ns |  7,566.77 ns |  3,359.69 ns |  1.17 |    0.08 |    1 |         - |          NA |
| TernaryHeapSort  | 4096 | PipeOrgan          |    98,820.7 ns |  1,677.65 ns |    744.89 ns |  0.95 |    0.06 |    1 |         - |          NA |
| BottomupHeapSort | 4096 | PipeOrgan          |   102,364.2 ns |  3,525.94 ns |  1,565.54 ns |  0.98 |    0.06 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | PipeOrgan          |   232,923.9 ns |    800.45 ns |    355.40 ns |  2.24 |    0.14 |    2 |         - |          NA |
| SmoothSort       | 4096 | PipeOrgan          |   280,914.4 ns |  3,064.03 ns |  1,360.45 ns |  2.70 |    0.17 |    3 |         - |          NA |
| TournamentSort   | 4096 | PipeOrgan          |   459,920.4 ns |  8,709.96 ns |  4,555.48 ns |  4.41 |    0.29 |    4 |         - |          NA |
| BinomialHeapSort | 4096 | PipeOrgan          |   142,891.5 ns |    206.10 ns |     91.51 ns |  1.37 |    0.09 |    1 |         - |          NA |
| PairingHeapSort  | 4096 | PipeOrgan          |   120,898.7 ns |  1,378.43 ns |    612.03 ns |  1.16 |    0.08 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **ManyDuplicates**     |   **174,350.8 ns** |  **2,709.96 ns** |  **1,417.36 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 4096 | ManyDuplicates     |   176,100.4 ns |  1,808.49 ns |    945.87 ns |  1.01 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 4096 | ManyDuplicates     |   101,286.7 ns |  2,950.86 ns |  1,310.20 ns |  0.58 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 4096 | ManyDuplicates     |   110,858.6 ns | 19,826.55 ns | 10,369.67 ns |  0.64 |    0.06 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | ManyDuplicates     |   236,361.4 ns |  3,986.39 ns |  2,084.96 ns |  1.36 |    0.02 |    3 |         - |          NA |
| SmoothSort       | 4096 | ManyDuplicates     |   322,971.9 ns |  2,118.41 ns |  1,107.97 ns |  1.85 |    0.02 |    4 |         - |          NA |
| TournamentSort   | 4096 | ManyDuplicates     |   610,695.8 ns |  1,828.88 ns |    812.03 ns |  3.50 |    0.03 |    6 |         - |          NA |
| BinomialHeapSort | 4096 | ManyDuplicates     |   720,916.9 ns |  7,447.69 ns |  3,306.82 ns |  4.14 |    0.04 |    6 |         - |          NA |
| PairingHeapSort  | 4096 | ManyDuplicates     |   411,339.3 ns |  2,638.81 ns |  1,380.15 ns |  2.36 |    0.02 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Random**             |   **529,255.7 ns** |  **5,887.03 ns** |  **3,079.03 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Random             |   523,908.5 ns |  8,561.67 ns |  4,477.92 ns |  0.99 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | Random             |   662,494.2 ns |  9,055.11 ns |  3,229.14 ns |  1.25 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | Random             |   658,545.5 ns |  2,382.88 ns |  1,058.01 ns |  1.24 |    0.01 |    2 |         - |          NA |
| WeakHeapSort     | 8192 | Random             |   980,548.8 ns |  2,305.64 ns |  1,205.90 ns |  1.85 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 8192 | Random             |   936,288.1 ns |  2,055.39 ns |  1,075.01 ns |  1.77 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 8192 | Random             | 1,493,757.5 ns |  7,457.13 ns |  3,311.01 ns |  2.82 |    0.02 |    4 |         - |          NA |
| BinomialHeapSort | 8192 | Random             | 2,327,400.0 ns | 13,446.19 ns |  5,970.19 ns |  4.40 |    0.03 |    5 |         - |          NA |
| PairingHeapSort  | 8192 | Random             | 1,115,875.2 ns |  5,113.68 ns |  2,674.55 ns |  2.11 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **SingleElementMoved** |   **318,196.2 ns** |  **9,367.25 ns** |  **4,159.12 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 8192 | SingleElementMoved |   394,426.4 ns |  4,459.68 ns |  2,332.50 ns |  1.24 |    0.02 |    4 |         - |          NA |
| TernaryHeapSort  | 8192 | SingleElementMoved |   398,184.5 ns |  2,015.13 ns |    894.73 ns |  1.25 |    0.02 |    4 |         - |          NA |
| BottomupHeapSort | 8192 | SingleElementMoved |   414,642.2 ns |  1,011.37 ns |    449.06 ns |  1.30 |    0.02 |    4 |         - |          NA |
| WeakHeapSort     | 8192 | SingleElementMoved |   467,145.1 ns |  1,028.79 ns |    456.79 ns |  1.47 |    0.02 |    4 |         - |          NA |
| SmoothSort       | 8192 | SingleElementMoved |    58,800.5 ns |  1,039.14 ns |    461.38 ns |  0.18 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 8192 | SingleElementMoved |   793,616.6 ns |  4,835.63 ns |  2,147.05 ns |  2.49 |    0.03 |    5 |         - |          NA |
| BinomialHeapSort | 8192 | SingleElementMoved |   297,192.2 ns |  3,992.94 ns |  1,772.89 ns |  0.93 |    0.01 |    3 |         - |          NA |
| PairingHeapSort  | 8192 | SingleElementMoved |   182,216.0 ns |  1,008.33 ns |    447.70 ns |  0.57 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Sorted**             |   **326,239.3 ns** |  **2,850.34 ns** |  **1,490.78 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Sorted             |   401,885.1 ns |  3,057.54 ns |  1,599.15 ns |  1.23 |    0.01 |    3 |         - |          NA |
| TernaryHeapSort  | 8192 | Sorted             |   354,224.2 ns |  8,560.82 ns |  3,801.06 ns |  1.09 |    0.01 |    3 |         - |          NA |
| BottomupHeapSort | 8192 | Sorted             |   407,851.9 ns |  1,231.44 ns |    546.77 ns |  1.25 |    0.01 |    3 |         - |          NA |
| WeakHeapSort     | 8192 | Sorted             |   470,821.6 ns |    926.68 ns |    484.67 ns |  1.44 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 8192 | Sorted             |    42,608.6 ns |    271.42 ns |    120.51 ns |  0.13 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 8192 | Sorted             |   503,807.9 ns |  9,051.21 ns |  4,733.96 ns |  1.54 |    0.02 |    3 |         - |          NA |
| BinomialHeapSort | 8192 | Sorted             |   274,651.1 ns |  1,796.72 ns |    939.72 ns |  0.84 |    0.00 |    3 |         - |          NA |
| PairingHeapSort  | 8192 | Sorted             |   184,365.0 ns |  1,579.77 ns |    701.43 ns |  0.57 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Reversed**           |   **377,495.1 ns** | **16,605.27 ns** |  **8,684.88 ns** |  **1.00** |    **0.03** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Reversed           |   346,830.0 ns |  3,774.12 ns |  1,675.73 ns |  0.92 |    0.02 |    3 |         - |          NA |
| TernaryHeapSort  | 8192 | Reversed           |   413,136.7 ns |  1,700.90 ns |    889.60 ns |  1.09 |    0.02 |    3 |         - |          NA |
| BottomupHeapSort | 8192 | Reversed           |   482,751.5 ns |  6,982.23 ns |  3,100.15 ns |  1.28 |    0.03 |    3 |         - |          NA |
| WeakHeapSort     | 8192 | Reversed           |   503,406.7 ns |    773.95 ns |    404.79 ns |  1.33 |    0.03 |    3 |         - |          NA |
| SmoothSort       | 8192 | Reversed           |   571,288.8 ns |  6,657.45 ns |  3,481.98 ns |  1.51 |    0.03 |    3 |         - |          NA |
| TournamentSort   | 8192 | Reversed           |   690,505.7 ns | 12,271.49 ns |  6,418.23 ns |  1.83 |    0.04 |    4 |         - |          NA |
| BinomialHeapSort | 8192 | Reversed           |   268,209.6 ns |    505.28 ns |    224.35 ns |  0.71 |    0.02 |    2 |         - |          NA |
| PairingHeapSort  | 8192 | Reversed           |    84,771.3 ns |    489.83 ns |    256.19 ns |  0.22 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **PipeOrgan**          |   **370,078.4 ns** | **18,938.16 ns** |  **9,905.02 ns** |  **1.00** |    **0.04** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 8192 | PipeOrgan          |   404,710.4 ns |  1,357.13 ns |    709.80 ns |  1.09 |    0.03 |    3 |         - |          NA |
| TernaryHeapSort  | 8192 | PipeOrgan          |   465,152.4 ns |  2,202.42 ns |    977.89 ns |  1.26 |    0.03 |    3 |         - |          NA |
| BottomupHeapSort | 8192 | PipeOrgan          |   474,440.6 ns |  2,280.17 ns |  1,192.57 ns |  1.28 |    0.03 |    3 |         - |          NA |
| WeakHeapSort     | 8192 | PipeOrgan          |   507,296.8 ns |    916.80 ns |    407.07 ns |  1.37 |    0.03 |    3 |         - |          NA |
| SmoothSort       | 8192 | PipeOrgan          |   710,451.4 ns |  3,992.36 ns |  2,088.08 ns |  1.92 |    0.05 |    4 |         - |          NA |
| TournamentSort   | 8192 | PipeOrgan          | 1,119,303.4 ns | 13,404.25 ns |  7,010.68 ns |  3.03 |    0.08 |    5 |         - |          NA |
| BinomialHeapSort | 8192 | PipeOrgan          |   297,590.4 ns |  1,110.67 ns |    493.14 ns |  0.80 |    0.02 |    2 |         - |          NA |
| PairingHeapSort  | 8192 | PipeOrgan          |   246,884.8 ns |    609.11 ns |    318.58 ns |  0.67 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **ManyDuplicates**     |   **506,680.8 ns** |  **5,224.14 ns** |  **2,732.33 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | ManyDuplicates     |   508,521.0 ns |  4,958.97 ns |  2,593.64 ns |  1.00 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | ManyDuplicates     |   591,470.2 ns |  5,894.64 ns |  3,083.01 ns |  1.17 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 8192 | ManyDuplicates     |   611,759.8 ns |  2,445.40 ns |  1,085.77 ns |  1.21 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | ManyDuplicates     |   675,229.1 ns |  3,100.18 ns |  1,376.50 ns |  1.33 |    0.01 |    1 |         - |          NA |
| SmoothSort       | 8192 | ManyDuplicates     |   791,812.9 ns |  2,897.10 ns |  1,515.24 ns |  1.56 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 8192 | ManyDuplicates     | 1,387,053.3 ns |  5,580.31 ns |  2,918.61 ns |  2.74 |    0.01 |    3 |         - |          NA |
| BinomialHeapSort | 8192 | ManyDuplicates     | 1,553,142.5 ns |  4,113.11 ns |  2,151.24 ns |  3.07 |    0.02 |    3 |         - |          NA |
| PairingHeapSort  | 8192 | ManyDuplicates     |   956,516.7 ns |  2,622.62 ns |  1,371.68 ns |  1.89 |    0.01 |    2 |         - |          NA |

### InsertionBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                 | Size | Pattern            | Mean         | Error       | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------- |----- |------------------- |-------------:|------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **InsertionSort**          | **256**  | **Random**             |   **7,054.3 ns** |   **242.70 ns** |   **126.94 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Random             |   7,264.3 ns |   321.05 ns |   114.49 ns |  1.03 |    0.02 |    3 |         - |          NA |
| BinaryInsertSort       | 256  | Random             |   5,499.3 ns |   216.82 ns |   113.40 ns |  0.78 |    0.02 |    2 |         - |          NA |
| GnomeSort              | 256  | Random             |  24,040.0 ns |   390.90 ns |   173.56 ns |  3.41 |    0.06 |    6 |         - |          NA |
| LibrarySort            | 256  | Random             |  16,254.9 ns |   381.13 ns |   169.22 ns |  2.30 |    0.05 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | Random             |  13,163.4 ns |    82.04 ns |    29.26 ns |  1.87 |    0.03 |    4 |         - |          NA |
| ShellSortKnuth1973     | 256  | Random             |   2,711.6 ns |   207.66 ns |    92.20 ns |  0.38 |    0.01 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Random             |   2,710.7 ns |   241.78 ns |   107.35 ns |  0.38 |    0.02 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | Random             |   2,706.6 ns |    22.59 ns |     8.06 ns |  0.38 |    0.01 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | Random             |   2,624.9 ns |   140.08 ns |    62.19 ns |  0.37 |    0.01 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Random             |   2,635.0 ns |   159.83 ns |    70.96 ns |  0.37 |    0.01 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **InsertionSort**          | **256**  | **SingleElementMoved** |     **446.0 ns** |     **3.71 ns** |     **1.94 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 256  | SingleElementMoved |     314.3 ns |     7.19 ns |     3.76 ns |  0.70 |    0.01 |    1 |         - |          NA |
| BinaryInsertSort       | 256  | SingleElementMoved |   1,139.6 ns |     8.49 ns |     4.44 ns |  2.56 |    0.01 |    4 |         - |          NA |
| GnomeSort              | 256  | SingleElementMoved |     699.5 ns |   147.95 ns |    77.38 ns |  1.57 |    0.16 |    3 |         - |          NA |
| LibrarySort            | 256  | SingleElementMoved |  15,332.2 ns |    49.73 ns |    22.08 ns | 34.37 |    0.15 |    6 |         - |          NA |
| MergeInsertionSort     | 256  | SingleElementMoved |  12,396.2 ns |   303.94 ns |   134.95 ns | 27.79 |    0.30 |    5 |         - |          NA |
| ShellSortKnuth1973     | 256  | SingleElementMoved |   1,333.0 ns |     6.67 ns |     2.96 ns |  2.99 |    0.01 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 256  | SingleElementMoved |   1,323.2 ns |     7.82 ns |     3.47 ns |  2.97 |    0.01 |    4 |         - |          NA |
| ShellSortTokuda1992    | 256  | SingleElementMoved |   1,606.1 ns |     4.87 ns |     2.16 ns |  3.60 |    0.02 |    4 |         - |          NA |
| ShellSortCiura2001     | 256  | SingleElementMoved |   1,421.0 ns |     8.19 ns |     3.63 ns |  3.19 |    0.02 |    4 |         - |          NA |
| ShellSortLee2021       | 256  | SingleElementMoved |   1,607.5 ns |    74.84 ns |    33.23 ns |  3.60 |    0.07 |    4 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **InsertionSort**          | **256**  | **Sorted**             |     **392.1 ns** |    **86.95 ns** |    **38.60 ns** |  **1.01** |    **0.13** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Sorted             |     280.7 ns |     1.74 ns |     0.91 ns |  0.72 |    0.06 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | Sorted             |     194.2 ns |     1.81 ns |     0.95 ns |  0.50 |    0.04 |    1 |         - |          NA |
| GnomeSort              | 256  | Sorted             |     314.7 ns |   124.81 ns |    55.42 ns |  0.81 |    0.15 |    2 |         - |          NA |
| LibrarySort            | 256  | Sorted             |  15,856.5 ns |    83.17 ns |    36.93 ns | 40.75 |    3.49 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | Sorted             |  12,573.0 ns |   354.38 ns |   157.35 ns | 32.31 |    2.79 |    4 |         - |          NA |
| ShellSortKnuth1973     | 256  | Sorted             |   1,212.2 ns |    34.37 ns |    17.98 ns |  3.12 |    0.27 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Sorted             |   1,196.7 ns |    21.88 ns |     7.80 ns |  3.08 |    0.26 |    3 |         - |          NA |
| ShellSortTokuda1992    | 256  | Sorted             |   1,465.2 ns |     7.11 ns |     2.53 ns |  3.77 |    0.32 |    3 |         - |          NA |
| ShellSortCiura2001     | 256  | Sorted             |   1,279.4 ns |     3.80 ns |     1.69 ns |  3.29 |    0.28 |    3 |         - |          NA |
| ShellSortLee2021       | 256  | Sorted             |   1,466.0 ns |     4.14 ns |     1.84 ns |  3.77 |    0.32 |    3 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **InsertionSort**          | **256**  | **Reversed**           |  **15,597.6 ns** |   **265.46 ns** |   **138.84 ns** |  **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Reversed           |  20,203.8 ns |   258.63 ns |   114.83 ns |  1.30 |    0.01 |    5 |         - |          NA |
| BinaryInsertSort       | 256  | Reversed           |   6,638.5 ns |   384.10 ns |   200.89 ns |  0.43 |    0.01 |    2 |         - |          NA |
| GnomeSort              | 256  | Reversed           |  67,658.3 ns | 1,681.43 ns |   879.42 ns |  4.34 |    0.06 |    6 |         - |          NA |
| LibrarySort            | 256  | Reversed           |  22,362.5 ns | 1,879.18 ns |   834.37 ns |  1.43 |    0.05 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | Reversed           |  12,547.9 ns |   398.36 ns |   208.35 ns |  0.80 |    0.01 |    3 |         - |          NA |
| ShellSortKnuth1973     | 256  | Reversed           |   2,041.7 ns |   567.28 ns |   251.88 ns |  0.13 |    0.02 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Reversed           |   1,865.8 ns |    39.98 ns |    17.75 ns |  0.12 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | Reversed           |   2,098.6 ns |    76.07 ns |    27.13 ns |  0.13 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | Reversed           |   2,136.1 ns |   313.60 ns |   164.02 ns |  0.14 |    0.01 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Reversed           |   2,042.3 ns |    21.12 ns |     7.53 ns |  0.13 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **InsertionSort**          | **256**  | **PipeOrgan**          |   **8,003.8 ns** |   **210.21 ns** |    **93.34 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 256  | PipeOrgan          |  10,330.7 ns |   361.98 ns |   189.32 ns |  1.29 |    0.03 |    4 |         - |          NA |
| BinaryInsertSort       | 256  | PipeOrgan          |   3,953.8 ns |   230.75 ns |   120.69 ns |  0.49 |    0.02 |    2 |         - |          NA |
| GnomeSort              | 256  | PipeOrgan          |  33,408.0 ns | 1,722.51 ns |   900.91 ns |  4.17 |    0.12 |    6 |         - |          NA |
| LibrarySort            | 256  | PipeOrgan          |  14,484.6 ns |   154.47 ns |    68.59 ns |  1.81 |    0.02 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | PipeOrgan          |  12,585.6 ns |   262.62 ns |   137.35 ns |  1.57 |    0.02 |    5 |         - |          NA |
| ShellSortKnuth1973     | 256  | PipeOrgan          |   1,823.7 ns |    41.79 ns |    18.56 ns |  0.23 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | PipeOrgan          |   1,908.5 ns |    80.37 ns |    35.68 ns |  0.24 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | PipeOrgan          |   2,140.9 ns |    21.81 ns |     9.68 ns |  0.27 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | PipeOrgan          |   2,031.6 ns |   406.18 ns |   212.44 ns |  0.25 |    0.03 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | PipeOrgan          |   2,181.7 ns |   149.26 ns |    66.27 ns |  0.27 |    0.01 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **InsertionSort**          | **256**  | **ManyDuplicates**     |   **6,984.4 ns** |   **650.61 ns** |   **288.87 ns** |  **1.00** |    **0.05** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 256  | ManyDuplicates     |   7,065.4 ns |   242.27 ns |   107.57 ns |  1.01 |    0.04 |    3 |         - |          NA |
| BinaryInsertSort       | 256  | ManyDuplicates     |   5,350.1 ns |   316.36 ns |   165.46 ns |  0.77 |    0.04 |    2 |         - |          NA |
| GnomeSort              | 256  | ManyDuplicates     |  23,361.6 ns |   370.10 ns |   164.33 ns |  3.35 |    0.13 |    6 |         - |          NA |
| LibrarySort            | 256  | ManyDuplicates     |  16,283.6 ns |   184.29 ns |    96.39 ns |  2.33 |    0.09 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | ManyDuplicates     |  13,287.6 ns |   332.50 ns |   173.90 ns |  1.91 |    0.07 |    4 |         - |          NA |
| ShellSortKnuth1973     | 256  | ManyDuplicates     |   2,255.0 ns |    18.33 ns |     6.54 ns |  0.32 |    0.01 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | ManyDuplicates     |   2,233.2 ns |    19.84 ns |     7.08 ns |  0.32 |    0.01 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | ManyDuplicates     |   2,179.6 ns |    17.76 ns |     6.33 ns |  0.31 |    0.01 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | ManyDuplicates     |   2,191.1 ns |    13.42 ns |     5.96 ns |  0.31 |    0.01 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | ManyDuplicates     |   2,144.3 ns |   129.57 ns |    57.53 ns |  0.31 |    0.01 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **InsertionSort**          | **1024** | **Random**             | **117,692.6 ns** | **2,186.36 ns** |   **970.76 ns** |  **1.00** |    **0.01** |    **5** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Random             | 135,505.5 ns | 3,994.36 ns | 1,773.52 ns |  1.15 |    0.02 |    5 |         - |          NA |
| BinaryInsertSort       | 1024 | Random             |  36,055.5 ns | 1,073.83 ns |   561.63 ns |  0.31 |    0.01 |    2 |         - |          NA |
| GnomeSort              | 1024 | Random             | 387,495.8 ns | 2,128.61 ns | 1,113.31 ns |  3.29 |    0.03 |    6 |         - |          NA |
| LibrarySort            | 1024 | Random             |  71,868.9 ns |   830.12 ns |   434.17 ns |  0.61 |    0.01 |    3 |         - |          NA |
| MergeInsertionSort     | 1024 | Random             |  94,632.7 ns | 2,647.40 ns | 1,384.64 ns |  0.80 |    0.01 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Random             |  14,705.7 ns |   381.86 ns |   169.55 ns |  0.12 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Random             |  14,595.6 ns |   350.99 ns |   155.84 ns |  0.12 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Random             |  14,483.1 ns |   190.34 ns |    84.51 ns |  0.12 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Random             |  14,326.2 ns |   237.86 ns |   105.61 ns |  0.12 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Random             |  14,401.1 ns |   443.75 ns |   197.03 ns |  0.12 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **InsertionSort**          | **1024** | **SingleElementMoved** |   **1,856.9 ns** |     **8.26 ns** |     **3.67 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | SingleElementMoved |   1,181.3 ns |    11.38 ns |     4.06 ns |  0.64 |    0.00 |    1 |         - |          NA |
| BinaryInsertSort       | 1024 | SingleElementMoved |   6,019.8 ns |   325.28 ns |   170.13 ns |  3.24 |    0.09 |    3 |         - |          NA |
| GnomeSort              | 1024 | SingleElementMoved |   2,044.4 ns |    54.67 ns |    24.27 ns |  1.10 |    0.01 |    2 |         - |          NA |
| LibrarySort            | 1024 | SingleElementMoved |  78,423.0 ns |   479.95 ns |   251.02 ns | 42.23 |    0.15 |    4 |         - |          NA |
| MergeInsertionSort     | 1024 | SingleElementMoved |  74,825.6 ns |   834.85 ns |   370.68 ns | 40.30 |    0.20 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | SingleElementMoved |   6,571.7 ns |   222.30 ns |    79.27 ns |  3.54 |    0.04 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | SingleElementMoved |   7,473.5 ns |   244.91 ns |   128.09 ns |  4.02 |    0.07 |    3 |         - |          NA |
| ShellSortTokuda1992    | 1024 | SingleElementMoved |   7,843.2 ns |    11.04 ns |     3.94 ns |  4.22 |    0.01 |    3 |         - |          NA |
| ShellSortCiura2001     | 1024 | SingleElementMoved |   8,062.4 ns |   223.99 ns |   117.15 ns |  4.34 |    0.06 |    3 |         - |          NA |
| ShellSortLee2021       | 1024 | SingleElementMoved |   8,063.6 ns |   187.94 ns |    83.45 ns |  4.34 |    0.04 |    3 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **InsertionSort**          | **1024** | **Sorted**             |   **1,451.9 ns** |    **47.66 ns** |    **21.16 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Sorted             |   1,083.7 ns |     2.81 ns |     1.00 ns |  0.75 |    0.01 |    2 |         - |          NA |
| BinaryInsertSort       | 1024 | Sorted             |     728.7 ns |     2.09 ns |     0.75 ns |  0.50 |    0.01 |    1 |         - |          NA |
| GnomeSort              | 1024 | Sorted             |     723.5 ns |     1.11 ns |     0.49 ns |  0.50 |    0.01 |    1 |         - |          NA |
| LibrarySort            | 1024 | Sorted             |  79,207.5 ns |   373.91 ns |   195.56 ns | 54.56 |    0.75 |    5 |         - |          NA |
| MergeInsertionSort     | 1024 | Sorted             |  74,875.9 ns |   134.10 ns |    59.54 ns | 51.58 |    0.70 |    5 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Sorted             |   6,067.0 ns |   227.81 ns |   119.15 ns |  4.18 |    0.10 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Sorted             |   6,788.2 ns |   254.65 ns |   133.19 ns |  4.68 |    0.11 |    4 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Sorted             |   7,263.9 ns |   287.13 ns |   127.49 ns |  5.00 |    0.11 |    4 |         - |          NA |
| ShellSortCiura2001     | 1024 | Sorted             |   7,261.7 ns |    58.20 ns |    25.84 ns |  5.00 |    0.07 |    4 |         - |          NA |
| ShellSortLee2021       | 1024 | Sorted             |   7,155.1 ns |     4.63 ns |     1.65 ns |  4.93 |    0.07 |    4 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **InsertionSort**          | **1024** | **Reversed**           | **229,027.1 ns** |   **365.72 ns** |   **162.38 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Reversed           | 314,942.2 ns | 1,338.07 ns |   477.17 ns |  1.38 |    0.00 |    5 |         - |          NA |
| BinaryInsertSort       | 1024 | Reversed           |  44,459.8 ns |   371.32 ns |   194.21 ns |  0.19 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | Reversed           | 971,577.9 ns | 4,582.69 ns | 2,396.84 ns |  4.24 |    0.01 |    6 |         - |          NA |
| LibrarySort            | 1024 | Reversed           | 192,446.7 ns |   593.67 ns |   310.50 ns |  0.84 |    0.00 |    4 |         - |          NA |
| MergeInsertionSort     | 1024 | Reversed           |  75,617.0 ns |   314.92 ns |   139.83 ns |  0.33 |    0.00 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Reversed           |   9,252.9 ns |   392.39 ns |   205.23 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Reversed           |   9,581.7 ns |   421.69 ns |   220.55 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Reversed           |  10,432.6 ns |   241.29 ns |   126.20 ns |  0.05 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Reversed           |  10,037.8 ns |   388.94 ns |   203.42 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Reversed           |  10,488.5 ns |   370.21 ns |   193.63 ns |  0.05 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **InsertionSort**          | **1024** | **PipeOrgan**          | **116,294.6 ns** |   **718.83 ns** |   **319.17 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | PipeOrgan          | 158,268.6 ns |   826.60 ns |   294.77 ns |  1.36 |    0.00 |    5 |         - |          NA |
| BinaryInsertSort       | 1024 | PipeOrgan          |  24,968.5 ns |   690.27 ns |   361.03 ns |  0.21 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | PipeOrgan          | 478,882.8 ns | 3,771.31 ns | 1,344.88 ns |  4.12 |    0.02 |    6 |         - |          NA |
| LibrarySort            | 1024 | PipeOrgan          |  71,584.4 ns |   604.67 ns |   316.25 ns |  0.62 |    0.00 |    3 |         - |          NA |
| MergeInsertionSort     | 1024 | PipeOrgan          |  76,231.3 ns |   673.93 ns |   299.23 ns |  0.66 |    0.00 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | PipeOrgan          |   9,110.1 ns |   381.21 ns |   199.38 ns |  0.08 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | PipeOrgan          |   9,732.5 ns |   382.00 ns |   199.79 ns |  0.08 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | PipeOrgan          |  10,823.1 ns |   324.02 ns |   169.47 ns |  0.09 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | PipeOrgan          |  10,469.7 ns |   314.93 ns |   164.72 ns |  0.09 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | PipeOrgan          |  10,941.7 ns |   370.97 ns |   194.02 ns |  0.09 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **InsertionSort**          | **1024** | **ManyDuplicates**     | **114,394.0 ns** |   **923.21 ns** |   **409.91 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | ManyDuplicates     | 130,797.2 ns | 2,075.37 ns |   921.47 ns |  1.14 |    0.01 |    5 |         - |          NA |
| BinaryInsertSort       | 1024 | ManyDuplicates     |  35,325.5 ns |   955.49 ns |   424.25 ns |  0.31 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | ManyDuplicates     | 376,071.3 ns | 3,215.78 ns | 1,681.92 ns |  3.29 |    0.02 |    6 |         - |          NA |
| LibrarySort            | 1024 | ManyDuplicates     |  74,667.1 ns |   614.73 ns |   272.95 ns |  0.65 |    0.00 |    3 |         - |          NA |
| MergeInsertionSort     | 1024 | ManyDuplicates     |  93,309.0 ns | 1,753.74 ns |   917.24 ns |  0.82 |    0.01 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | ManyDuplicates     |  11,439.4 ns |   362.30 ns |   160.86 ns |  0.10 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | ManyDuplicates     |  11,007.1 ns |   568.64 ns |   297.41 ns |  0.10 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | ManyDuplicates     |  10,998.8 ns |   478.32 ns |   212.38 ns |  0.10 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | ManyDuplicates     |  10,908.3 ns |   508.84 ns |   225.93 ns |  0.10 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | ManyDuplicates     |  11,038.1 ns |   401.69 ns |   210.09 ns |  0.10 |    0.00 |    1 |         - |          NA |

### MergeBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                   | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **MergeSort**                | **256**  | **Random**             |     **8,593.1 ns** |     **92.46 ns** |     **32.97 ns** |  **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Random             |     8,835.9 ns |    172.12 ns |     90.02 ns |  1.03 |    0.01 |    4 |         - |          NA |
| BottomupMergeSort        | 256  | Random             |     4,933.2 ns |    327.26 ns |    171.16 ns |  0.57 |    0.02 |    2 |         - |          NA |
| StdStableSort            | 256  | Random             |     2,763.2 ns |    378.58 ns |    198.00 ns |  0.32 |    0.02 |    1 |         - |          NA |
| RotateMergeSort          | 256  | Random             |    10,174.8 ns |    324.28 ns |    143.98 ns |  1.18 |    0.02 |    4 |         - |          NA |
| RotateMergeSortRecursive | 256  | Random             |    12,633.3 ns |    376.81 ns |    197.08 ns |  1.47 |    0.02 |    5 |         - |          NA |
| SymMergeSort             | 256  | Random             |     6,837.1 ns |     56.83 ns |     20.26 ns |  0.80 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 256  | Random             |     5,128.9 ns |    370.67 ns |    193.87 ns |  0.60 |    0.02 |    2 |         - |          NA |
| NaturalMergeSort         | 256  | Random             |     5,167.9 ns |    419.43 ns |    219.37 ns |  0.60 |    0.02 |    2 |         - |          NA |
| TimSort                  | 256  | Random             |     4,102.4 ns |    313.49 ns |    139.19 ns |  0.48 |    0.02 |    2 |         - |          NA |
| PowerSort                | 256  | Random             |     2,343.3 ns |     56.48 ns |     25.08 ns |  0.27 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Random             |     3,974.4 ns |    325.67 ns |    170.33 ns |  0.46 |    0.02 |    2 |         - |          NA |
| SpinSort                 | 256  | Random             |     2,327.7 ns |    226.85 ns |    118.65 ns |  0.27 |    0.01 |    1 |         - |          NA |
| Glidesort                | 256  | Random             |     3,793.6 ns |    397.67 ns |    207.99 ns |  0.44 |    0.02 |    2 |         - |          NA |
| Driftsort                | 256  | Random             |     4,548.3 ns |    330.89 ns |    173.06 ns |  0.53 |    0.02 |    2 |         - |          NA |
| FlatStableSort           | 256  | Random             |     2,701.6 ns |    135.46 ns |     60.15 ns |  0.31 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **SingleElementMoved** |     **4,771.6 ns** |    **354.75 ns** |    **185.54 ns** |  **1.00** |    **0.05** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | SingleElementMoved |     5,579.3 ns |    207.18 ns |     91.99 ns |  1.17 |    0.05 |    8 |         - |          NA |
| BottomupMergeSort        | 256  | SingleElementMoved |     1,664.2 ns |     34.56 ns |     15.34 ns |  0.35 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 256  | SingleElementMoved |       749.5 ns |     10.34 ns |      5.41 ns |  0.16 |    0.01 |    3 |         - |          NA |
| RotateMergeSort          | 256  | SingleElementMoved |       604.6 ns |      7.78 ns |      4.07 ns |  0.13 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 256  | SingleElementMoved |       712.6 ns |    116.30 ns |     41.47 ns |  0.15 |    0.01 |    3 |         - |          NA |
| SymMergeSort             | 256  | SingleElementMoved |       548.9 ns |      3.03 ns |      1.35 ns |  0.12 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 256  | SingleElementMoved |     3,061.0 ns |      7.15 ns |      3.17 ns |  0.64 |    0.02 |    7 |         - |          NA |
| NaturalMergeSort         | 256  | SingleElementMoved |       619.9 ns |      5.32 ns |      2.36 ns |  0.13 |    0.00 |    3 |         - |          NA |
| TimSort                  | 256  | SingleElementMoved |       277.6 ns |      3.15 ns |      1.12 ns |  0.06 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | SingleElementMoved |       438.8 ns |     45.39 ns |     20.15 ns |  0.09 |    0.01 |    2 |         - |          NA |
| ShiftSort                | 256  | SingleElementMoved |       401.3 ns |      2.84 ns |      1.26 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 256  | SingleElementMoved |     1,014.6 ns |    168.51 ns |     88.13 ns |  0.21 |    0.02 |    4 |         - |          NA |
| Glidesort                | 256  | SingleElementMoved |     1,246.7 ns |     13.69 ns |      6.08 ns |  0.26 |    0.01 |    5 |         - |          NA |
| Driftsort                | 256  | SingleElementMoved |     1,196.2 ns |     18.28 ns |      8.12 ns |  0.25 |    0.01 |    5 |         - |          NA |
| FlatStableSort           | 256  | SingleElementMoved |     1,294.6 ns |     50.17 ns |     17.89 ns |  0.27 |    0.01 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **Sorted**             |     **4,361.9 ns** |    **334.96 ns** |    **175.19 ns** |  **1.00** |    **0.05** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Sorted             |     5,307.6 ns |    235.10 ns |     83.84 ns |  1.22 |    0.05 |    9 |         - |          NA |
| BottomupMergeSort        | 256  | Sorted             |     1,254.1 ns |     13.79 ns |      6.12 ns |  0.29 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 256  | Sorted             |       663.0 ns |      8.42 ns |      4.40 ns |  0.15 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 256  | Sorted             |       301.4 ns |     20.54 ns |      9.12 ns |  0.07 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 256  | Sorted             |       442.1 ns |      2.87 ns |      1.02 ns |  0.10 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 256  | Sorted             |       426.0 ns |     59.49 ns |     31.12 ns |  0.10 |    0.01 |    4 |         - |          NA |
| BlockMergeSort           | 256  | Sorted             |     2,609.1 ns |     17.94 ns |      7.96 ns |  0.60 |    0.02 |    7 |         - |          NA |
| NaturalMergeSort         | 256  | Sorted             |       297.8 ns |    122.69 ns |     54.48 ns |  0.07 |    0.01 |    3 |         - |          NA |
| TimSort                  | 256  | Sorted             |       207.2 ns |     61.32 ns |     32.07 ns |  0.05 |    0.01 |    2 |         - |          NA |
| PowerSort                | 256  | Sorted             |       186.5 ns |     48.96 ns |     25.61 ns |  0.04 |    0.01 |    2 |         - |          NA |
| ShiftSort                | 256  | Sorted             |       199.3 ns |     26.01 ns |     11.55 ns |  0.05 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 256  | Sorted             |       150.0 ns |      1.40 ns |      0.73 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 256  | Sorted             |       249.7 ns |     93.28 ns |     48.79 ns |  0.06 |    0.01 |    2 |         - |          NA |
| Driftsort                | 256  | Sorted             |       214.5 ns |      1.60 ns |      0.71 ns |  0.05 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 256  | Sorted             |     1,225.7 ns |      5.78 ns |      2.57 ns |  0.28 |    0.01 |    6 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **Reversed**           |     **8,891.9 ns** |      **8.73 ns** |      **3.11 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Reversed           |     8,063.8 ns |     17.51 ns |      9.16 ns |  0.91 |    0.00 |    7 |         - |          NA |
| BottomupMergeSort        | 256  | Reversed           |     5,072.2 ns |    344.64 ns |    180.25 ns |  0.57 |    0.02 |    5 |         - |          NA |
| StdStableSort            | 256  | Reversed           |     6,438.8 ns |    241.47 ns |    126.29 ns |  0.72 |    0.01 |    6 |         - |          NA |
| RotateMergeSort          | 256  | Reversed           |     1,952.2 ns |     50.71 ns |     22.52 ns |  0.22 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 256  | Reversed           |     2,289.6 ns |     72.38 ns |     32.14 ns |  0.26 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 256  | Reversed           |     2,112.2 ns |     12.54 ns |      4.47 ns |  0.24 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 256  | Reversed           |     3,281.6 ns |    246.46 ns |    128.91 ns |  0.37 |    0.01 |    4 |         - |          NA |
| NaturalMergeSort         | 256  | Reversed           |       416.8 ns |     96.80 ns |     50.63 ns |  0.05 |    0.01 |    2 |         - |          NA |
| TimSort                  | 256  | Reversed           |       237.2 ns |      1.23 ns |      0.44 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | Reversed           |       303.0 ns |     64.40 ns |     33.68 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Reversed           |       263.1 ns |      5.39 ns |      1.92 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 256  | Reversed           |       320.6 ns |     99.06 ns |     51.81 ns |  0.04 |    0.01 |    1 |         - |          NA |
| Glidesort                | 256  | Reversed           |       283.2 ns |      2.66 ns |      1.18 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 256  | Reversed           |       292.2 ns |      2.57 ns |      1.14 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | Reversed           |     2,879.3 ns |     44.25 ns |     19.65 ns |  0.32 |    0.00 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **PipeOrgan**          |     **6,950.8 ns** |    **359.94 ns** |    **188.26 ns** |  **1.00** |    **0.04** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | PipeOrgan          |     7,082.3 ns |    435.46 ns |    227.75 ns |  1.02 |    0.04 |    8 |         - |          NA |
| BottomupMergeSort        | 256  | PipeOrgan          |     3,360.9 ns |    430.63 ns |    225.23 ns |  0.48 |    0.03 |    6 |         - |          NA |
| StdStableSort            | 256  | PipeOrgan          |     3,685.6 ns |    264.39 ns |    138.28 ns |  0.53 |    0.02 |    6 |         - |          NA |
| RotateMergeSort          | 256  | PipeOrgan          |     4,364.9 ns |    444.32 ns |    232.39 ns |  0.63 |    0.04 |    6 |         - |          NA |
| RotateMergeSortRecursive | 256  | PipeOrgan          |     5,366.5 ns |    340.08 ns |    177.87 ns |  0.77 |    0.03 |    7 |         - |          NA |
| SymMergeSort             | 256  | PipeOrgan          |     2,611.6 ns |     69.27 ns |     24.70 ns |  0.38 |    0.01 |    5 |         - |          NA |
| BlockMergeSort           | 256  | PipeOrgan          |     3,162.0 ns |     64.14 ns |     22.87 ns |  0.46 |    0.01 |    6 |         - |          NA |
| NaturalMergeSort         | 256  | PipeOrgan          |       746.3 ns |     29.92 ns |     13.28 ns |  0.11 |    0.00 |    2 |         - |          NA |
| TimSort                  | 256  | PipeOrgan          |       804.1 ns |     14.94 ns |      6.63 ns |  0.12 |    0.00 |    2 |         - |          NA |
| PowerSort                | 256  | PipeOrgan          |       519.7 ns |      4.67 ns |      2.08 ns |  0.07 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | PipeOrgan          |       682.5 ns |    129.19 ns |     67.57 ns |  0.10 |    0.01 |    2 |         - |          NA |
| SpinSort                 | 256  | PipeOrgan          |     2,062.0 ns |    271.61 ns |     96.86 ns |  0.30 |    0.01 |    4 |         - |          NA |
| Glidesort                | 256  | PipeOrgan          |     1,266.6 ns |      9.54 ns |      4.24 ns |  0.18 |    0.00 |    3 |         - |          NA |
| Driftsort                | 256  | PipeOrgan          |       474.6 ns |     51.00 ns |     22.64 ns |  0.07 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | PipeOrgan          |     2,152.2 ns |     25.53 ns |     11.33 ns |  0.31 |    0.01 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **ManyDuplicates**     |     **8,548.4 ns** |    **241.56 ns** |    **107.25 ns** |  **1.00** |    **0.02** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | ManyDuplicates     |     8,096.7 ns |     85.19 ns |     37.82 ns |  0.95 |    0.01 |    4 |         - |          NA |
| BottomupMergeSort        | 256  | ManyDuplicates     |     4,567.4 ns |    199.00 ns |    104.08 ns |  0.53 |    0.01 |    2 |         - |          NA |
| StdStableSort            | 256  | ManyDuplicates     |     2,601.2 ns |     41.70 ns |     18.52 ns |  0.30 |    0.00 |    1 |         - |          NA |
| RotateMergeSort          | 256  | ManyDuplicates     |     9,462.0 ns |    420.41 ns |    219.88 ns |  1.11 |    0.03 |    4 |         - |          NA |
| RotateMergeSortRecursive | 256  | ManyDuplicates     |    11,710.8 ns |    365.86 ns |    162.44 ns |  1.37 |    0.02 |    5 |         - |          NA |
| SymMergeSort             | 256  | ManyDuplicates     |     6,541.2 ns |    412.48 ns |    215.74 ns |  0.77 |    0.03 |    3 |         - |          NA |
| BlockMergeSort           | 256  | ManyDuplicates     |     5,086.4 ns |    226.70 ns |    118.57 ns |  0.60 |    0.01 |    2 |         - |          NA |
| NaturalMergeSort         | 256  | ManyDuplicates     |     5,035.1 ns |    318.53 ns |    166.60 ns |  0.59 |    0.02 |    2 |         - |          NA |
| TimSort                  | 256  | ManyDuplicates     |     3,967.7 ns |    299.85 ns |    156.83 ns |  0.46 |    0.02 |    2 |         - |          NA |
| PowerSort                | 256  | ManyDuplicates     |     2,301.9 ns |     71.14 ns |     31.59 ns |  0.27 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | ManyDuplicates     |     3,975.6 ns |    432.18 ns |    226.04 ns |  0.47 |    0.03 |    2 |         - |          NA |
| SpinSort                 | 256  | ManyDuplicates     |     2,352.4 ns |    303.82 ns |    158.90 ns |  0.28 |    0.02 |    1 |         - |          NA |
| Glidesort                | 256  | ManyDuplicates     |     3,470.4 ns |    100.45 ns |     44.60 ns |  0.41 |    0.01 |    2 |         - |          NA |
| Driftsort                | 256  | ManyDuplicates     |     4,445.0 ns |    269.20 ns |    140.80 ns |  0.52 |    0.02 |    2 |         - |          NA |
| FlatStableSort           | 256  | ManyDuplicates     |     2,532.9 ns |    434.60 ns |    192.97 ns |  0.30 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **Random**             |    **38,466.6 ns** |  **1,056.08 ns** |    **468.91 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Random             |    37,616.7 ns |    319.16 ns |    141.71 ns |  0.98 |    0.01 |    2 |         - |          NA |
| BottomupMergeSort        | 1024 | Random             |    21,426.9 ns |    616.61 ns |    273.78 ns |  0.56 |    0.01 |    1 |         - |          NA |
| StdStableSort            | 1024 | Random             |    13,887.2 ns |    437.34 ns |    194.18 ns |  0.36 |    0.01 |    1 |         - |          NA |
| RotateMergeSort          | 1024 | Random             |    64,393.0 ns |  4,731.37 ns |  2,474.60 ns |  1.67 |    0.06 |    3 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Random             |    75,806.6 ns |    672.42 ns |    351.69 ns |  1.97 |    0.02 |    3 |         - |          NA |
| SymMergeSort             | 1024 | Random             |    42,682.2 ns |    974.66 ns |    432.76 ns |  1.11 |    0.02 |    2 |         - |          NA |
| BlockMergeSort           | 1024 | Random             |    25,820.0 ns |    332.53 ns |    147.65 ns |  0.67 |    0.01 |    1 |         - |          NA |
| NaturalMergeSort         | 1024 | Random             |    24,429.5 ns |    394.54 ns |    175.18 ns |  0.64 |    0.01 |    1 |         - |          NA |
| TimSort                  | 1024 | Random             |    19,530.1 ns |    392.99 ns |    174.49 ns |  0.51 |    0.01 |    1 |         - |          NA |
| PowerSort                | 1024 | Random             |    12,583.4 ns |    520.44 ns |    272.20 ns |  0.33 |    0.01 |    1 |         - |          NA |
| ShiftSort                | 1024 | Random             |    18,671.4 ns |    521.69 ns |    272.85 ns |  0.49 |    0.01 |    1 |         - |          NA |
| SpinSort                 | 1024 | Random             |    12,402.7 ns |    461.37 ns |    241.30 ns |  0.32 |    0.01 |    1 |         - |          NA |
| Glidesort                | 1024 | Random             |    16,765.2 ns |    365.77 ns |    191.30 ns |  0.44 |    0.01 |    1 |         - |          NA |
| Driftsort                | 1024 | Random             |    21,377.4 ns |    232.28 ns |    121.49 ns |  0.56 |    0.01 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Random             |    13,982.2 ns |    699.64 ns |    310.65 ns |  0.36 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **SingleElementMoved** |    **18,790.0 ns** |    **194.92 ns** |     **86.55 ns** |  **1.00** |    **0.01** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | SingleElementMoved |    22,523.1 ns |    210.06 ns |     74.91 ns |  1.20 |    0.01 |    8 |         - |          NA |
| BottomupMergeSort        | 1024 | SingleElementMoved |     6,695.5 ns |    447.69 ns |    234.15 ns |  0.36 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 1024 | SingleElementMoved |     3,730.6 ns |     12.87 ns |      4.59 ns |  0.20 |    0.00 |    4 |         - |          NA |
| RotateMergeSort          | 1024 | SingleElementMoved |     2,026.3 ns |     13.29 ns |      5.90 ns |  0.11 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 1024 | SingleElementMoved |     2,266.7 ns |    382.62 ns |    169.89 ns |  0.12 |    0.01 |    2 |         - |          NA |
| SymMergeSort             | 1024 | SingleElementMoved |     1,815.1 ns |     14.11 ns |      6.26 ns |  0.10 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 1024 | SingleElementMoved |    14,284.2 ns |    278.64 ns |    123.72 ns |  0.76 |    0.01 |    7 |         - |          NA |
| NaturalMergeSort         | 1024 | SingleElementMoved |     2,221.3 ns |      3.65 ns |      1.30 ns |  0.12 |    0.00 |    2 |         - |          NA |
| TimSort                  | 1024 | SingleElementMoved |       847.6 ns |      6.14 ns |      3.21 ns |  0.05 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | SingleElementMoved |     1,516.3 ns |      5.76 ns |      2.56 ns |  0.08 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 1024 | SingleElementMoved |     1,482.8 ns |     40.76 ns |     18.10 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 1024 | SingleElementMoved |     4,689.5 ns |    382.06 ns |    199.82 ns |  0.25 |    0.01 |    5 |         - |          NA |
| Glidesort                | 1024 | SingleElementMoved |     3,137.2 ns |    301.82 ns |    157.86 ns |  0.17 |    0.01 |    3 |         - |          NA |
| Driftsort                | 1024 | SingleElementMoved |     1,334.1 ns |      3.16 ns |      1.13 ns |  0.07 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 1024 | SingleElementMoved |     6,031.1 ns |    365.93 ns |    191.39 ns |  0.32 |    0.01 |    6 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **Sorted**             |    **17,342.8 ns** |    **378.16 ns** |    **197.78 ns** |  **1.00** |    **0.02** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Sorted             |    21,086.4 ns |    191.17 ns |     99.99 ns |  1.22 |    0.01 |    9 |         - |          NA |
| BottomupMergeSort        | 1024 | Sorted             |     5,148.6 ns |    527.59 ns |    275.94 ns |  0.30 |    0.02 |    6 |         - |          NA |
| StdStableSort            | 1024 | Sorted             |     3,515.0 ns |     20.37 ns |      7.26 ns |  0.20 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 1024 | Sorted             |     1,140.6 ns |     11.94 ns |      6.25 ns |  0.07 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Sorted             |     1,763.4 ns |      2.35 ns |      1.04 ns |  0.10 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 1024 | Sorted             |     1,448.0 ns |     22.45 ns |      8.01 ns |  0.08 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | Sorted             |    11,680.4 ns |    231.69 ns |    121.18 ns |  0.67 |    0.01 |    7 |         - |          NA |
| NaturalMergeSort         | 1024 | Sorted             |       793.0 ns |      4.94 ns |      2.59 ns |  0.05 |    0.00 |    1 |         - |          NA |
| TimSort                  | 1024 | Sorted             |       694.9 ns |     10.23 ns |      4.54 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Sorted             |       588.2 ns |      9.18 ns |      4.80 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Sorted             |       614.0 ns |      3.56 ns |      1.58 ns |  0.04 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 1024 | Sorted             |       525.7 ns |      4.19 ns |      2.19 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Sorted             |       560.7 ns |     17.29 ns |      6.17 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | Sorted             |       651.6 ns |     12.09 ns |      4.31 ns |  0.04 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Sorted             |     5,118.5 ns |     40.99 ns |     14.62 ns |  0.30 |    0.00 |    6 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **Reversed**           |    **37,967.9 ns** |    **664.41 ns** |    **347.50 ns** |  **1.00** |    **0.01** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Reversed           |    34,593.8 ns |  1,432.49 ns |    749.22 ns |  0.91 |    0.02 |    6 |         - |          NA |
| BottomupMergeSort        | 1024 | Reversed           |    21,005.5 ns |    831.72 ns |    369.29 ns |  0.55 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 1024 | Reversed           |    26,715.8 ns |    200.03 ns |     88.82 ns |  0.70 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 1024 | Reversed           |     9,368.8 ns |    318.02 ns |    166.33 ns |  0.25 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Reversed           |    10,735.2 ns |    419.11 ns |    219.20 ns |  0.28 |    0.01 |    2 |         - |          NA |
| SymMergeSort             | 1024 | Reversed           |     9,298.2 ns |    375.87 ns |    196.59 ns |  0.24 |    0.01 |    2 |         - |          NA |
| BlockMergeSort           | 1024 | Reversed           |    16,150.2 ns |    222.27 ns |    116.25 ns |  0.43 |    0.00 |    3 |         - |          NA |
| NaturalMergeSort         | 1024 | Reversed           |     1,160.1 ns |      3.45 ns |      1.53 ns |  0.03 |    0.00 |    1 |         - |          NA |
| TimSort                  | 1024 | Reversed           |       918.9 ns |      3.83 ns |      1.37 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Reversed           |       906.9 ns |     10.06 ns |      3.59 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Reversed           |       930.6 ns |     25.35 ns |     11.26 ns |  0.02 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 1024 | Reversed           |       964.4 ns |      3.00 ns |      1.57 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Reversed           |       965.2 ns |      3.11 ns |      1.63 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | Reversed           |       969.3 ns |      6.55 ns |      2.91 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Reversed           |    11,891.3 ns |    250.64 ns |    131.09 ns |  0.31 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **PipeOrgan**          |    **28,180.6 ns** |    **675.31 ns** |    **299.84 ns** |  **1.00** |    **0.01** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | PipeOrgan          |    28,517.4 ns |    622.44 ns |    325.55 ns |  1.01 |    0.01 |    7 |         - |          NA |
| BottomupMergeSort        | 1024 | PipeOrgan          |    13,703.8 ns |    389.43 ns |    172.91 ns |  0.49 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 1024 | PipeOrgan          |    15,530.6 ns |    190.11 ns |     99.43 ns |  0.55 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 1024 | PipeOrgan          |    18,547.4 ns |    207.09 ns |     91.95 ns |  0.66 |    0.01 |    5 |         - |          NA |
| RotateMergeSortRecursive | 1024 | PipeOrgan          |    23,347.3 ns |    238.27 ns |    105.79 ns |  0.83 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 1024 | PipeOrgan          |    11,580.8 ns |    291.55 ns |    103.97 ns |  0.41 |    0.01 |    5 |         - |          NA |
| BlockMergeSort           | 1024 | PipeOrgan          |    16,359.5 ns |    154.34 ns |     80.72 ns |  0.58 |    0.01 |    5 |         - |          NA |
| NaturalMergeSort         | 1024 | PipeOrgan          |     2,795.9 ns |    331.49 ns |    173.37 ns |  0.10 |    0.01 |    2 |         - |          NA |
| TimSort                  | 1024 | PipeOrgan          |     2,893.2 ns |     93.13 ns |     33.21 ns |  0.10 |    0.00 |    2 |         - |          NA |
| PowerSort                | 1024 | PipeOrgan          |     1,798.8 ns |     10.79 ns |      4.79 ns |  0.06 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | PipeOrgan          |     1,924.4 ns |     11.79 ns |      5.24 ns |  0.07 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 1024 | PipeOrgan          |     9,201.3 ns |    493.55 ns |    176.00 ns |  0.33 |    0.01 |    4 |         - |          NA |
| Glidesort                | 1024 | PipeOrgan          |     4,960.6 ns |    316.01 ns |    165.28 ns |  0.18 |    0.01 |    3 |         - |          NA |
| Driftsort                | 1024 | PipeOrgan          |     1,549.0 ns |     20.38 ns |      9.05 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | PipeOrgan          |     9,362.1 ns |    269.47 ns |    140.94 ns |  0.33 |    0.01 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **ManyDuplicates**     |    **36,218.0 ns** |    **865.22 ns** |    **452.53 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | ManyDuplicates     |    35,333.7 ns |    796.09 ns |    353.47 ns |  0.98 |    0.01 |    3 |         - |          NA |
| BottomupMergeSort        | 1024 | ManyDuplicates     |    20,309.9 ns |    607.32 ns |    317.64 ns |  0.56 |    0.01 |    2 |         - |          NA |
| StdStableSort            | 1024 | ManyDuplicates     |    12,950.6 ns |    158.74 ns |     83.02 ns |  0.36 |    0.00 |    1 |         - |          NA |
| RotateMergeSort          | 1024 | ManyDuplicates     |    50,445.5 ns |  1,169.07 ns |    611.45 ns |  1.39 |    0.02 |    4 |         - |          NA |
| RotateMergeSortRecursive | 1024 | ManyDuplicates     |    58,238.2 ns |  1,014.16 ns |    530.43 ns |  1.61 |    0.02 |    4 |         - |          NA |
| SymMergeSort             | 1024 | ManyDuplicates     |    36,796.4 ns |    687.46 ns |    359.56 ns |  1.02 |    0.02 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | ManyDuplicates     |    26,608.0 ns |    681.69 ns |    356.54 ns |  0.73 |    0.01 |    2 |         - |          NA |
| NaturalMergeSort         | 1024 | ManyDuplicates     |    24,015.1 ns |    961.67 ns |    502.97 ns |  0.66 |    0.02 |    2 |         - |          NA |
| TimSort                  | 1024 | ManyDuplicates     |    18,811.8 ns |    375.31 ns |    166.64 ns |  0.52 |    0.01 |    2 |         - |          NA |
| PowerSort                | 1024 | ManyDuplicates     |    11,709.0 ns |    330.66 ns |    172.94 ns |  0.32 |    0.01 |    1 |         - |          NA |
| ShiftSort                | 1024 | ManyDuplicates     |    18,181.4 ns |    451.14 ns |    235.96 ns |  0.50 |    0.01 |    2 |         - |          NA |
| SpinSort                 | 1024 | ManyDuplicates     |    11,291.7 ns |    818.76 ns |    363.53 ns |  0.31 |    0.01 |    1 |         - |          NA |
| Glidesort                | 1024 | ManyDuplicates     |    16,283.1 ns |    524.34 ns |    274.24 ns |  0.45 |    0.01 |    2 |         - |          NA |
| Driftsort                | 1024 | ManyDuplicates     |    17,276.8 ns |    161.98 ns |     71.92 ns |  0.48 |    0.01 |    2 |         - |          NA |
| FlatStableSort           | 1024 | ManyDuplicates     |    11,815.3 ns |    464.31 ns |    206.16 ns |  0.33 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **Random**             |   **184,374.4 ns** | **22,721.21 ns** | **11,883.63 ns** |  **1.00** |    **0.09** |    **2** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Random             |   165,814.1 ns |  1,965.62 ns |    872.75 ns |  0.90 |    0.06 |    2 |         - |          NA |
| BottomupMergeSort        | 4096 | Random             |    97,537.1 ns |  1,035.78 ns |    459.89 ns |  0.53 |    0.03 |    1 |         - |          NA |
| StdStableSort            | 4096 | Random             |    80,911.2 ns |  8,170.15 ns |  4,273.15 ns |  0.44 |    0.04 |    1 |         - |          NA |
| RotateMergeSort          | 4096 | Random             |   632,435.3 ns | 10,502.80 ns |  4,663.31 ns |  3.44 |    0.22 |    4 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Random             |   673,395.0 ns |  8,202.70 ns |  4,290.17 ns |  3.67 |    0.23 |    4 |         - |          NA |
| SymMergeSort             | 4096 | Random             |   414,654.9 ns |  6,968.39 ns |  2,484.99 ns |  2.26 |    0.14 |    3 |         - |          NA |
| BlockMergeSort           | 4096 | Random             |   148,207.5 ns |  9,173.76 ns |  4,073.21 ns |  0.81 |    0.05 |    2 |         - |          NA |
| NaturalMergeSort         | 4096 | Random             |   132,323.5 ns |  6,890.47 ns |  3,059.41 ns |  0.72 |    0.05 |    2 |         - |          NA |
| TimSort                  | 4096 | Random             |    95,915.8 ns |  1,779.45 ns |    790.09 ns |  0.52 |    0.03 |    1 |         - |          NA |
| PowerSort                | 4096 | Random             |    65,609.4 ns |  1,175.63 ns |    521.99 ns |  0.36 |    0.02 |    1 |         - |          NA |
| ShiftSort                | 4096 | Random             |    89,466.2 ns |  1,379.81 ns |    492.05 ns |  0.49 |    0.03 |    1 |         - |          NA |
| SpinSort                 | 4096 | Random             |    61,425.6 ns |  1,952.90 ns |    867.10 ns |  0.33 |    0.02 |    1 |         - |          NA |
| Glidesort                | 4096 | Random             |    82,178.0 ns |    419.22 ns |    186.14 ns |  0.45 |    0.03 |    1 |         - |          NA |
| Driftsort                | 4096 | Random             |    98,048.9 ns |  1,520.24 ns |    795.11 ns |  0.53 |    0.03 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Random             |    68,320.6 ns |  1,614.74 ns |    716.95 ns |  0.37 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **SingleElementMoved** |    **74,851.4 ns** |  **1,086.23 ns** |    **482.29 ns** |  **1.00** |    **0.01** |    **9** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | SingleElementMoved |    90,141.7 ns |  1,131.06 ns |    591.57 ns |  1.20 |    0.01 |   10 |         - |          NA |
| BottomupMergeSort        | 4096 | SingleElementMoved |    26,597.7 ns |  1,538.57 ns |    683.13 ns |  0.36 |    0.01 |    7 |         - |          NA |
| StdStableSort            | 4096 | SingleElementMoved |    18,514.2 ns |  1,005.51 ns |    446.45 ns |  0.25 |    0.01 |    6 |         - |          NA |
| RotateMergeSort          | 4096 | SingleElementMoved |     7,729.8 ns |    379.35 ns |    198.40 ns |  0.10 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 4096 | SingleElementMoved |     8,057.6 ns |    422.96 ns |    221.22 ns |  0.11 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 4096 | SingleElementMoved |     6,964.4 ns |     12.50 ns |      4.46 ns |  0.09 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 4096 | SingleElementMoved |    57,894.4 ns |    745.54 ns |    389.93 ns |  0.77 |    0.01 |    8 |         - |          NA |
| NaturalMergeSort         | 4096 | SingleElementMoved |     7,544.1 ns |    536.47 ns |    238.20 ns |  0.10 |    0.00 |    3 |         - |          NA |
| TimSort                  | 4096 | SingleElementMoved |     3,181.6 ns |    270.32 ns |    141.38 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | SingleElementMoved |     5,850.5 ns |    230.38 ns |    120.49 ns |  0.08 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 4096 | SingleElementMoved |     5,722.7 ns |    253.10 ns |    132.38 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 4096 | SingleElementMoved |    15,143.9 ns |  1,669.58 ns |    873.22 ns |  0.20 |    0.01 |    5 |         - |          NA |
| Glidesort                | 4096 | SingleElementMoved |    11,755.5 ns |    237.67 ns |    124.30 ns |  0.16 |    0.00 |    4 |         - |          NA |
| Driftsort                | 4096 | SingleElementMoved |     5,341.2 ns |    486.55 ns |    216.03 ns |  0.07 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 4096 | SingleElementMoved |    25,306.1 ns |  1,414.97 ns |    740.06 ns |  0.34 |    0.01 |    7 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **Sorted**             |    **68,731.8 ns** |    **856.03 ns** |    **447.72 ns** |  **1.00** |    **0.01** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Sorted             |    85,416.8 ns |    527.14 ns |    234.06 ns |  1.24 |    0.01 |    9 |         - |          NA |
| BottomupMergeSort        | 4096 | Sorted             |    20,015.3 ns |    243.23 ns |    107.99 ns |  0.29 |    0.00 |    6 |         - |          NA |
| StdStableSort            | 4096 | Sorted             |    18,201.3 ns |    478.74 ns |    212.56 ns |  0.26 |    0.00 |    6 |         - |          NA |
| RotateMergeSort          | 4096 | Sorted             |     4,671.0 ns |    861.74 ns |    382.62 ns |  0.07 |    0.01 |    3 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Sorted             |     7,137.3 ns |    265.88 ns |    118.05 ns |  0.10 |    0.00 |    5 |         - |          NA |
| SymMergeSort             | 4096 | Sorted             |     5,891.9 ns |    255.19 ns |    113.30 ns |  0.09 |    0.00 |    4 |         - |          NA |
| BlockMergeSort           | 4096 | Sorted             |    47,210.6 ns |    270.23 ns |    119.98 ns |  0.69 |    0.00 |    7 |         - |          NA |
| NaturalMergeSort         | 4096 | Sorted             |     2,944.2 ns |     10.41 ns |      3.71 ns |  0.04 |    0.00 |    2 |         - |          NA |
| TimSort                  | 4096 | Sorted             |     2,281.1 ns |      3.62 ns |      1.89 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | Sorted             |     2,408.6 ns |    397.31 ns |    176.41 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | Sorted             |     2,341.7 ns |    130.82 ns |     68.42 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 4096 | Sorted             |     2,146.1 ns |     27.70 ns |     14.49 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 4096 | Sorted             |     1,986.3 ns |     39.04 ns |     20.42 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | Sorted             |     2,351.7 ns |     18.56 ns |      8.24 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Sorted             |    20,425.9 ns |    448.53 ns |    199.15 ns |  0.30 |    0.00 |    6 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **Reversed**           |   **157,303.6 ns** |  **1,938.09 ns** |  **1,013.66 ns** |  **1.00** |    **0.01** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Reversed           |   142,363.8 ns |  1,748.23 ns |    914.36 ns |  0.91 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 4096 | Reversed           |    90,010.3 ns |  1,550.03 ns |    810.69 ns |  0.57 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 4096 | Reversed           |   112,101.7 ns |    358.08 ns |    158.99 ns |  0.71 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 4096 | Reversed           |    43,731.5 ns |  3,796.04 ns |  1,685.47 ns |  0.28 |    0.01 |    2 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Reversed           |    48,351.2 ns |    350.10 ns |    183.11 ns |  0.31 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 4096 | Reversed           |    39,703.8 ns |    680.03 ns |    355.67 ns |  0.25 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 4096 | Reversed           |    73,408.9 ns |    987.92 ns |    516.70 ns |  0.47 |    0.00 |    3 |         - |          NA |
| NaturalMergeSort         | 4096 | Reversed           |     4,380.5 ns |     22.11 ns |      7.88 ns |  0.03 |    0.00 |    1 |         - |          NA |
| TimSort                  | 4096 | Reversed           |     3,543.1 ns |     15.26 ns |      5.44 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | Reversed           |     3,533.9 ns |      4.86 ns |      2.16 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | Reversed           |     3,514.2 ns |    325.47 ns |    144.51 ns |  0.02 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 4096 | Reversed           |     3,790.0 ns |     15.72 ns |      5.61 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Glidesort                | 4096 | Reversed           |     3,692.2 ns |    276.68 ns |    122.85 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | Reversed           |     3,611.9 ns |      7.02 ns |      2.50 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Reversed           |    47,170.4 ns |    180.48 ns |     94.40 ns |  0.30 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **PipeOrgan**          |   **116,263.6 ns** |  **1,787.22 ns** |    **793.54 ns** |  **1.00** |    **0.01** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | PipeOrgan          |   117,669.3 ns |  1,877.91 ns |    833.80 ns |  1.01 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 4096 | PipeOrgan          |    58,850.4 ns |  1,432.37 ns |    749.16 ns |  0.51 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 4096 | PipeOrgan          |    65,577.3 ns |    386.11 ns |    201.94 ns |  0.56 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 4096 | PipeOrgan          |    80,733.5 ns |    900.58 ns |    399.86 ns |  0.69 |    0.01 |    5 |         - |          NA |
| RotateMergeSortRecursive | 4096 | PipeOrgan          |    99,629.3 ns |    497.27 ns |    260.08 ns |  0.86 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 4096 | PipeOrgan          |    50,652.7 ns |  1,009.72 ns |    448.32 ns |  0.44 |    0.00 |    5 |         - |          NA |
| BlockMergeSort           | 4096 | PipeOrgan          |    68,626.0 ns |    968.04 ns |    506.30 ns |  0.59 |    0.01 |    5 |         - |          NA |
| NaturalMergeSort         | 4096 | PipeOrgan          |    10,553.0 ns |    380.73 ns |    169.04 ns |  0.09 |    0.00 |    2 |         - |          NA |
| TimSort                  | 4096 | PipeOrgan          |    11,640.7 ns |  1,662.03 ns |    737.95 ns |  0.10 |    0.01 |    2 |         - |          NA |
| PowerSort                | 4096 | PipeOrgan          |     6,898.3 ns |    299.47 ns |    156.63 ns |  0.06 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | PipeOrgan          |     7,510.8 ns |    637.30 ns |    333.32 ns |  0.06 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 4096 | PipeOrgan          |     8,622.2 ns |    618.91 ns |    274.80 ns |  0.07 |    0.00 |    1 |         - |          NA |
| Glidesort                | 4096 | PipeOrgan          |    18,901.3 ns |    288.39 ns |    102.84 ns |  0.16 |    0.00 |    3 |         - |          NA |
| Driftsort                | 4096 | PipeOrgan          |     5,982.8 ns |    359.98 ns |    159.83 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | PipeOrgan          |    37,792.5 ns |    758.95 ns |    396.95 ns |  0.33 |    0.00 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **ManyDuplicates**     |   **156,670.8 ns** |  **3,621.47 ns** |  **1,894.10 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | ManyDuplicates     |   151,017.4 ns |  2,900.52 ns |  1,287.85 ns |  0.96 |    0.01 |    3 |         - |          NA |
| BottomupMergeSort        | 4096 | ManyDuplicates     |    93,564.0 ns |  3,284.86 ns |  1,718.04 ns |  0.60 |    0.01 |    2 |         - |          NA |
| StdStableSort            | 4096 | ManyDuplicates     |    73,062.6 ns |  6,965.48 ns |  3,643.08 ns |  0.47 |    0.02 |    2 |         - |          NA |
| RotateMergeSort          | 4096 | ManyDuplicates     |   336,932.0 ns | 14,889.39 ns |  7,787.44 ns |  2.15 |    0.05 |    6 |         - |          NA |
| RotateMergeSortRecursive | 4096 | ManyDuplicates     |   283,896.7 ns | 27,659.41 ns | 12,280.95 ns |  1.81 |    0.08 |    5 |         - |          NA |
| SymMergeSort             | 4096 | ManyDuplicates     |   213,802.0 ns | 17,685.55 ns |  9,249.88 ns |  1.36 |    0.06 |    4 |         - |          NA |
| BlockMergeSort           | 4096 | ManyDuplicates     |   134,058.1 ns |  2,104.58 ns |    934.45 ns |  0.86 |    0.01 |    3 |         - |          NA |
| NaturalMergeSort         | 4096 | ManyDuplicates     |   115,924.4 ns |  4,512.50 ns |  2,360.13 ns |  0.74 |    0.02 |    3 |         - |          NA |
| TimSort                  | 4096 | ManyDuplicates     |    81,566.9 ns |    891.91 ns |    396.01 ns |  0.52 |    0.01 |    2 |         - |          NA |
| PowerSort                | 4096 | ManyDuplicates     |    57,538.6 ns |  1,746.55 ns |    913.48 ns |  0.37 |    0.01 |    1 |         - |          NA |
| ShiftSort                | 4096 | ManyDuplicates     |    85,410.3 ns |  4,067.70 ns |  2,127.49 ns |  0.55 |    0.01 |    2 |         - |          NA |
| SpinSort                 | 4096 | ManyDuplicates     |    55,099.1 ns |  2,675.83 ns |  1,399.51 ns |  0.35 |    0.01 |    1 |         - |          NA |
| Glidesort                | 4096 | ManyDuplicates     |    47,159.7 ns |  1,227.09 ns |    641.79 ns |  0.30 |    0.01 |    1 |         - |          NA |
| Driftsort                | 4096 | ManyDuplicates     |    43,502.7 ns |    891.77 ns |    395.95 ns |  0.28 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | ManyDuplicates     |    58,614.5 ns |  2,034.59 ns |  1,064.13 ns |  0.37 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **Random**             |   **756,393.2 ns** |  **4,088.59 ns** |  **2,138.41 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Random             |   660,829.4 ns |  3,121.62 ns |  1,386.02 ns |  0.87 |    0.00 |    3 |         - |          NA |
| BottomupMergeSort        | 8192 | Random             |   534,929.5 ns |  2,800.27 ns |  1,464.60 ns |  0.71 |    0.00 |    3 |         - |          NA |
| StdStableSort            | 8192 | Random             |   391,279.7 ns |  6,848.13 ns |  3,581.70 ns |  0.52 |    0.00 |    2 |         - |          NA |
| RotateMergeSort          | 8192 | Random             | 1,550,241.8 ns | 13,646.11 ns |  7,137.18 ns |  2.05 |    0.01 |    5 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Random             | 1,673,216.3 ns |  5,607.95 ns |  2,933.07 ns |  2.21 |    0.01 |    5 |         - |          NA |
| SymMergeSort             | 8192 | Random             | 1,134,440.3 ns |  2,597.29 ns |  1,358.44 ns |  1.50 |    0.00 |    4 |         - |          NA |
| BlockMergeSort           | 8192 | Random             |   654,212.6 ns |  6,897.34 ns |  2,459.66 ns |  0.86 |    0.00 |    3 |         - |          NA |
| NaturalMergeSort         | 8192 | Random             |   684,761.7 ns |  1,889.75 ns |    988.37 ns |  0.91 |    0.00 |    3 |         - |          NA |
| TimSort                  | 8192 | Random             |   581,468.1 ns |  3,408.66 ns |  1,782.80 ns |  0.77 |    0.00 |    3 |         - |          NA |
| PowerSort                | 8192 | Random             |   436,389.8 ns |  5,489.74 ns |  2,437.48 ns |  0.58 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 8192 | Random             |   592,859.0 ns |  3,958.40 ns |  1,757.55 ns |  0.78 |    0.00 |    3 |         - |          NA |
| SpinSort                 | 8192 | Random             |   359,738.3 ns |  3,535.07 ns |  1,569.59 ns |  0.48 |    0.00 |    2 |         - |          NA |
| Glidesort                | 8192 | Random             |   179,353.8 ns |  3,037.47 ns |  1,588.65 ns |  0.24 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Random             |   207,552.3 ns |  1,583.21 ns |    828.05 ns |  0.27 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Random             |   399,450.4 ns |  2,384.92 ns |  1,058.92 ns |  0.53 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **SingleElementMoved** |   **149,421.9 ns** |    **963.87 ns** |    **504.12 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | SingleElementMoved |   180,839.1 ns |  2,068.41 ns |  1,081.82 ns |  1.21 |    0.01 |    8 |         - |          NA |
| BottomupMergeSort        | 8192 | SingleElementMoved |    54,823.5 ns |  1,069.41 ns |    559.32 ns |  0.37 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 8192 | SingleElementMoved |    35,482.2 ns |  1,693.58 ns |    885.77 ns |  0.24 |    0.01 |    4 |         - |          NA |
| RotateMergeSort          | 8192 | SingleElementMoved |    15,272.2 ns |    251.44 ns |    111.64 ns |  0.10 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 8192 | SingleElementMoved |    15,834.8 ns |    712.21 ns |    316.23 ns |  0.11 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 8192 | SingleElementMoved |    14,023.7 ns |    348.11 ns |    124.14 ns |  0.09 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 8192 | SingleElementMoved |   114,645.1 ns |    614.38 ns |    321.33 ns |  0.77 |    0.00 |    6 |         - |          NA |
| NaturalMergeSort         | 8192 | SingleElementMoved |    17,582.4 ns |    456.75 ns |    202.80 ns |  0.12 |    0.00 |    2 |         - |          NA |
| TimSort                  | 8192 | SingleElementMoved |     6,197.3 ns |    260.52 ns |    136.26 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | SingleElementMoved |    11,824.9 ns |    362.31 ns |    189.49 ns |  0.08 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 8192 | SingleElementMoved |    11,466.3 ns |    357.08 ns |    158.55 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | SingleElementMoved |    23,831.5 ns |    960.63 ns |    426.53 ns |  0.16 |    0.00 |    3 |         - |          NA |
| Glidesort                | 8192 | SingleElementMoved |    23,677.9 ns |    615.75 ns |    322.05 ns |  0.16 |    0.00 |    3 |         - |          NA |
| Driftsort                | 8192 | SingleElementMoved |    10,372.5 ns |    425.92 ns |    189.11 ns |  0.07 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 8192 | SingleElementMoved |    48,559.3 ns |    250.16 ns |    111.07 ns |  0.32 |    0.00 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **Sorted**             |   **137,062.6 ns** |    **521.64 ns** |    **272.83 ns** |  **1.00** |    **0.00** |    **9** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Sorted             |   171,109.2 ns |    872.59 ns |    456.38 ns |  1.25 |    0.00 |   10 |         - |          NA |
| BottomupMergeSort        | 8192 | Sorted             |    43,551.3 ns |    562.59 ns |    294.24 ns |  0.32 |    0.00 |    7 |         - |          NA |
| StdStableSort            | 8192 | Sorted             |    35,076.4 ns |    941.39 ns |    417.98 ns |  0.26 |    0.00 |    6 |         - |          NA |
| RotateMergeSort          | 8192 | Sorted             |     9,157.0 ns |    624.60 ns |    222.74 ns |  0.07 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Sorted             |    14,671.2 ns |    423.74 ns |    151.11 ns |  0.11 |    0.00 |    5 |         - |          NA |
| SymMergeSort             | 8192 | Sorted             |    11,954.9 ns |    298.63 ns |    132.59 ns |  0.09 |    0.00 |    4 |         - |          NA |
| BlockMergeSort           | 8192 | Sorted             |    93,226.8 ns |    729.96 ns |    381.79 ns |  0.68 |    0.00 |    8 |         - |          NA |
| NaturalMergeSort         | 8192 | Sorted             |     5,946.0 ns |    317.29 ns |    165.95 ns |  0.04 |    0.00 |    2 |         - |          NA |
| TimSort                  | 8192 | Sorted             |     4,727.2 ns |    349.30 ns |    182.69 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Sorted             |     4,710.5 ns |    349.50 ns |    182.80 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Sorted             |     4,582.9 ns |    344.69 ns |    180.28 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 8192 | Sorted             |     4,194.5 ns |    422.28 ns |    220.86 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Sorted             |     3,997.4 ns |     63.45 ns |     28.17 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Sorted             |     4,865.9 ns |    637.45 ns |    283.03 ns |  0.04 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Sorted             |     4,274.0 ns |    362.86 ns |    189.78 ns |  0.03 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **Reversed**           |   **320,251.0 ns** |  **1,307.22 ns** |    **683.70 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Reversed           |   292,441.4 ns |  4,161.46 ns |  2,176.52 ns |  0.91 |    0.01 |    5 |         - |          NA |
| BottomupMergeSort        | 8192 | Reversed           |   186,644.4 ns |  2,680.77 ns |  1,402.10 ns |  0.58 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 8192 | Reversed           |   411,532.7 ns |  1,514.79 ns |    672.58 ns |  1.29 |    0.00 |    6 |         - |          NA |
| RotateMergeSort          | 8192 | Reversed           |    91,418.5 ns |    284.75 ns |    126.43 ns |  0.29 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Reversed           |   102,632.5 ns |    749.25 ns |    391.87 ns |  0.32 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 8192 | Reversed           |    81,745.5 ns |  1,254.85 ns |    656.31 ns |  0.26 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 8192 | Reversed           |   152,855.2 ns |    781.61 ns |    347.04 ns |  0.48 |    0.00 |    3 |         - |          NA |
| NaturalMergeSort         | 8192 | Reversed           |     8,813.7 ns |    243.74 ns |    127.48 ns |  0.03 |    0.00 |    1 |         - |          NA |
| TimSort                  | 8192 | Reversed           |     7,433.8 ns |    338.54 ns |    177.06 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Reversed           |     7,247.4 ns |    592.83 ns |    263.22 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Reversed           |     6,973.3 ns |    364.22 ns |    190.49 ns |  0.02 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 8192 | Reversed           |     7,839.1 ns |    344.90 ns |    180.39 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Reversed           |     7,425.1 ns |    395.87 ns |    175.77 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Reversed           |     7,292.5 ns |    327.88 ns |    171.49 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Reversed           |     8,019.4 ns |    436.24 ns |    193.69 ns |  0.03 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **PipeOrgan**          |   **235,024.7 ns** |  **1,221.59 ns** |    **542.39 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | PipeOrgan          |   237,690.7 ns |  2,747.00 ns |  1,219.68 ns |  1.01 |    0.01 |    7 |         - |          NA |
| BottomupMergeSort        | 8192 | PipeOrgan          |   122,040.1 ns |  3,241.49 ns |  1,695.36 ns |  0.52 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 8192 | PipeOrgan          |   225,172.4 ns |  1,404.05 ns |    734.35 ns |  0.96 |    0.00 |    7 |         - |          NA |
| RotateMergeSort          | 8192 | PipeOrgan          |   166,873.8 ns |    493.64 ns |    258.18 ns |  0.71 |    0.00 |    6 |         - |          NA |
| RotateMergeSortRecursive | 8192 | PipeOrgan          |   203,647.4 ns |  1,935.48 ns |    859.37 ns |  0.87 |    0.00 |    7 |         - |          NA |
| SymMergeSort             | 8192 | PipeOrgan          |   103,050.4 ns |  3,179.99 ns |  1,663.20 ns |  0.44 |    0.01 |    6 |         - |          NA |
| BlockMergeSort           | 8192 | PipeOrgan          |   139,320.6 ns |    869.30 ns |    454.66 ns |  0.59 |    0.00 |    6 |         - |          NA |
| NaturalMergeSort         | 8192 | PipeOrgan          |    22,187.2 ns |  2,502.49 ns |  1,308.85 ns |  0.09 |    0.01 |    3 |         - |          NA |
| TimSort                  | 8192 | PipeOrgan          |    22,781.7 ns |    663.44 ns |    294.57 ns |  0.10 |    0.00 |    3 |         - |          NA |
| PowerSort                | 8192 | PipeOrgan          |    13,934.8 ns |    409.31 ns |    214.08 ns |  0.06 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 8192 | PipeOrgan          |    14,765.7 ns |    100.94 ns |     44.82 ns |  0.06 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | PipeOrgan          |    19,007.6 ns |  2,290.66 ns |  1,198.06 ns |  0.08 |    0.00 |    3 |         - |          NA |
| Glidesort                | 8192 | PipeOrgan          |    37,849.0 ns |    552.17 ns |    245.17 ns |  0.16 |    0.00 |    4 |         - |          NA |
| Driftsort                | 8192 | PipeOrgan          |    11,388.5 ns |    348.13 ns |    182.08 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | PipeOrgan          |    76,576.3 ns |  1,142.18 ns |    507.14 ns |  0.33 |    0.00 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **ManyDuplicates**     |   **483,873.9 ns** | **32,980.41 ns** | **14,643.51 ns** |  **1.00** |    **0.04** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | ManyDuplicates     |   474,434.2 ns | 11,638.90 ns |  5,167.74 ns |  0.98 |    0.03 |    6 |         - |          NA |
| BottomupMergeSort        | 8192 | ManyDuplicates     |   302,248.4 ns |  8,928.78 ns |  3,964.43 ns |  0.63 |    0.02 |    4 |         - |          NA |
| StdStableSort            | 8192 | ManyDuplicates     |   233,843.0 ns |  7,791.72 ns |  4,075.22 ns |  0.48 |    0.02 |    3 |         - |          NA |
| RotateMergeSort          | 8192 | ManyDuplicates     |   948,501.0 ns |  4,512.47 ns |  2,360.11 ns |  1.96 |    0.06 |    8 |         - |          NA |
| RotateMergeSortRecursive | 8192 | ManyDuplicates     | 1,020,945.2 ns |  4,181.61 ns |  2,187.06 ns |  2.11 |    0.06 |    8 |         - |          NA |
| SymMergeSort             | 8192 | ManyDuplicates     |   768,639.7 ns |  2,261.92 ns |  1,183.03 ns |  1.59 |    0.04 |    7 |         - |          NA |
| BlockMergeSort           | 8192 | ManyDuplicates     |   542,834.9 ns |  5,710.27 ns |  2,986.58 ns |  1.12 |    0.03 |    6 |         - |          NA |
| NaturalMergeSort         | 8192 | ManyDuplicates     |   502,342.9 ns |  5,185.15 ns |  2,302.24 ns |  1.04 |    0.03 |    6 |         - |          NA |
| TimSort                  | 8192 | ManyDuplicates     |   385,368.8 ns |  8,579.90 ns |  4,487.45 ns |  0.80 |    0.02 |    5 |         - |          NA |
| PowerSort                | 8192 | ManyDuplicates     |   194,564.0 ns |  4,809.59 ns |  2,135.49 ns |  0.40 |    0.01 |    2 |         - |          NA |
| ShiftSort                | 8192 | ManyDuplicates     |   361,864.0 ns | 18,319.26 ns |  9,581.33 ns |  0.75 |    0.03 |    5 |         - |          NA |
| SpinSort                 | 8192 | ManyDuplicates     |   184,766.6 ns |  4,789.36 ns |  2,126.51 ns |  0.38 |    0.01 |    2 |         - |          NA |
| Glidesort                | 8192 | ManyDuplicates     |    90,676.5 ns |  2,633.79 ns |  1,377.52 ns |  0.19 |    0.01 |    1 |         - |          NA |
| Driftsort                | 8192 | ManyDuplicates     |    82,538.7 ns |    320.36 ns |    142.24 ns |  0.17 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | ManyDuplicates     |   170,711.4 ns | 20,738.48 ns | 10,846.63 ns |  0.35 |    0.02 |    2 |         - |          NA |

### NetworkBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                  | Size | Pattern            | Mean           | Error        | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------------ |----- |------------------- |---------------:|-------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **BitonicSort**             | **256**  | **Random**             |    **10,054.4 ns** |    **269.71 ns** |   **119.75 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Random             |    22,959.7 ns |    129.62 ns |    67.79 ns |  2.28 |    0.03 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Random             |    18,685.7 ns |    145.25 ns |    75.97 ns |  1.86 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **SingleElementMoved** |     **9,889.8 ns** |    **564.96 ns** |   **295.48 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | SingleElementMoved |    23,309.3 ns |    187.75 ns |    98.20 ns |  2.36 |    0.07 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | SingleElementMoved |    18,643.8 ns |    130.47 ns |    68.24 ns |  1.89 |    0.05 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **Sorted**             |     **9,494.7 ns** |    **375.12 ns** |   **166.56 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Sorted             |    23,030.4 ns |    224.74 ns |   117.54 ns |  2.43 |    0.04 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Sorted             |    18,632.5 ns |    124.74 ns |    55.38 ns |  1.96 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **Reversed**           |     **9,774.7 ns** |    **298.87 ns** |   **132.70 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Reversed           |    23,177.5 ns |    119.66 ns |    53.13 ns |  2.37 |    0.03 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Reversed           |    18,735.7 ns |    152.25 ns |    79.63 ns |  1.92 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **PipeOrgan**          |     **9,761.1 ns** |    **551.79 ns** |   **288.60 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | PipeOrgan          |    23,399.7 ns |    215.85 ns |    95.84 ns |  2.40 |    0.07 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | PipeOrgan          |    18,666.5 ns |     72.56 ns |    37.95 ns |  1.91 |    0.05 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **ManyDuplicates**     |    **10,001.8 ns** |    **175.80 ns** |    **91.95 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | ManyDuplicates     |    22,853.1 ns |    178.63 ns |    93.43 ns |  2.29 |    0.02 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | ManyDuplicates     |    18,678.6 ns |    172.60 ns |    90.28 ns |  1.87 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Random**             |    **59,489.0 ns** |  **2,188.95 ns** | **1,144.86 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Random             |   118,606.0 ns |    625.66 ns |   277.80 ns |  1.99 |    0.04 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Random             |   116,502.4 ns |  1,759.79 ns |   920.40 ns |  1.96 |    0.04 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **SingleElementMoved** |    **60,723.2 ns** |  **1,382.59 ns** |   **723.12 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | SingleElementMoved |   120,151.4 ns |    388.75 ns |   203.32 ns |  1.98 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | SingleElementMoved |   115,009.5 ns |    259.13 ns |   135.53 ns |  1.89 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Sorted**             |    **57,530.0 ns** |  **1,240.33 ns** |   **550.72 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Sorted             |   119,371.7 ns |    933.66 ns |   488.32 ns |  2.08 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Sorted             |   115,124.1 ns |    278.05 ns |   123.46 ns |  2.00 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Reversed**           |    **59,654.4 ns** |    **679.39 ns** |   **301.65 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Reversed           |   119,754.3 ns |    541.10 ns |   240.25 ns |  2.01 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Reversed           |   115,138.8 ns |    227.37 ns |   118.92 ns |  1.93 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **PipeOrgan**          |    **60,595.8 ns** |  **1,045.44 ns** |   **464.18 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | PipeOrgan          |   120,008.1 ns |    691.87 ns |   361.86 ns |  1.98 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | PipeOrgan          |   115,183.0 ns |    389.35 ns |   203.64 ns |  1.90 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **ManyDuplicates**     |    **60,483.9 ns** |  **2,441.20 ns** | **1,083.91 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | ManyDuplicates     |   117,493.9 ns |    893.98 ns |   467.57 ns |  1.94 |    0.03 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | ManyDuplicates     |   115,867.2 ns |  1,447.31 ns |   642.61 ns |  1.92 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Random**             |   **565,526.4 ns** |  **3,304.44 ns** | **1,728.29 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Random             |   830,653.9 ns |  3,191.20 ns | 1,669.06 ns |  1.47 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Random             |   684,877.9 ns |  2,712.08 ns | 1,204.18 ns |  1.21 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **SingleElementMoved** |   **342,207.0 ns** |  **5,053.48 ns** | **2,243.77 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | SingleElementMoved |   599,784.7 ns |  2,835.19 ns | 1,482.86 ns |  1.75 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | SingleElementMoved |   658,637.6 ns |    487.50 ns |   216.45 ns |  1.92 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Sorted**             |   **340,437.2 ns** |  **8,101.55 ns** | **3,597.14 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Sorted             |   594,751.2 ns |  8,303.34 ns | 3,686.73 ns |  1.75 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Sorted             |   659,892.3 ns |  4,648.91 ns | 2,064.14 ns |  1.94 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Reversed**           |   **336,358.7 ns** |  **5,828.00 ns** | **3,048.16 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Reversed           |   598,017.0 ns |  1,961.60 ns | 1,025.95 ns |  1.78 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Reversed           |   659,094.7 ns |    561.35 ns |   293.60 ns |  1.96 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **PipeOrgan**          |   **342,095.0 ns** |  **6,297.81 ns** | **3,293.88 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | PipeOrgan          |   599,992.9 ns |  2,880.98 ns | 1,506.81 ns |  1.75 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | PipeOrgan          |   659,377.3 ns |    730.62 ns |   382.13 ns |  1.93 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **ManyDuplicates**     |   **455,025.4 ns** |  **4,384.24 ns** | **1,946.63 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | ManyDuplicates     |   712,074.9 ns | 13,612.40 ns | 6,043.99 ns |  1.56 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | ManyDuplicates     |   661,130.8 ns |  1,196.04 ns |   426.52 ns |  1.45 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **8192** | **Random**             | **1,322,597.5 ns** | **16,044.48 ns** | **8,391.57 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | Random             | 1,957,609.6 ns |  4,488.85 ns | 2,347.76 ns |  1.48 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | Random             | 1,683,274.9 ns |  2,855.48 ns | 1,493.47 ns |  1.27 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **8192** | **SingleElementMoved** |   **790,691.2 ns** |  **5,183.12 ns** | **2,301.34 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | SingleElementMoved | 1,350,524.7 ns |  3,869.54 ns | 1,718.10 ns |  1.71 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | SingleElementMoved | 1,540,960.0 ns |    626.94 ns |   278.36 ns |  1.95 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **8192** | **Sorted**             |   **779,257.4 ns** | **17,061.29 ns** | **7,575.32 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | Sorted             | 1,333,681.1 ns |  2,996.88 ns | 1,567.43 ns |  1.71 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | Sorted             | 1,541,493.6 ns |  1,150.82 ns |   510.97 ns |  1.98 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **8192** | **Reversed**           |   **779,793.9 ns** |  **3,876.77 ns** | **1,721.31 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | Reversed           | 1,348,295.2 ns |  5,177.20 ns | 2,298.71 ns |  1.73 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | Reversed           | 1,542,990.1 ns |  1,152.69 ns |   602.88 ns |  1.98 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **8192** | **PipeOrgan**          |   **790,005.8 ns** |  **7,178.35 ns** | **3,754.42 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | PipeOrgan          | 1,351,018.7 ns |  4,457.03 ns | 2,331.11 ns |  1.71 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | PipeOrgan          | 1,542,513.9 ns |    928.31 ns |   412.18 ns |  1.95 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **8192** | **ManyDuplicates**     | **1,067,201.4 ns** | **10,127.99 ns** | **5,297.13 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | ManyDuplicates     | 1,686,148.2 ns |  4,257.53 ns | 2,226.77 ns |  1.58 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | ManyDuplicates     | 1,591,860.8 ns |  1,086.75 ns |   568.39 ns |  1.49 |    0.01 |    2 |         - |          NA |

### PartitionBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                       | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **QuickSort**                    | **256**  | **Random**             |     **2,695.0 ns** |    **118.91 ns** |     **52.80 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 256  | Random             |     2,450.7 ns |     99.95 ns |     44.38 ns |  0.91 |    0.02 |    1 |         - |          NA |
| QuickSortMedian3             | 256  | Random             |     2,844.6 ns |    373.43 ns |    165.80 ns |  1.06 |    0.06 |    1 |         - |          NA |
| QuickSortMedian9             | 256  | Random             |     3,215.6 ns |    347.56 ns |    181.78 ns |  1.19 |    0.07 |    1 |         - |          NA |
| DualPivotQuickSort           | 256  | Random             |     2,359.6 ns |     86.94 ns |     45.47 ns |  0.88 |    0.02 |    1 |         - |          NA |
| StableQuickSort              | 256  | Random             |    11,354.7 ns |    499.37 ns |    261.18 ns |  4.21 |    0.12 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Random             |     7,454.8 ns |    248.71 ns |    130.08 ns |  2.77 |    0.07 |    2 |         - |          NA |
| DestswapStableQuickSort      | 256  | Random             |     7,115.5 ns |    490.12 ns |    256.34 ns |  2.64 |    0.10 |    2 |         - |          NA |
| IntroSort                    | 256  | Random             |     2,542.3 ns |    668.21 ns |    349.49 ns |  0.94 |    0.12 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | Random             |     1,803.2 ns |     97.63 ns |     43.35 ns |  0.67 |    0.02 |    1 |         - |          NA |
| PDQSort                      | 256  | Random             |     1,782.3 ns |     23.39 ns |      8.34 ns |  0.66 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | Random             |     2,790.0 ns |     82.29 ns |     36.54 ns |  1.04 |    0.02 |    1 |         - |          NA |
| Ipnsort                      | 256  | Random             |     3,657.7 ns |     26.76 ns |     11.88 ns |  1.36 |    0.02 |    1 |         - |          NA |
| StdSort                      | 256  | Random             |     2,794.0 ns |     13.42 ns |      4.78 ns |  1.04 |    0.02 |    1 |         - |          NA |
| BlockQuickSort               | 256  | Random             |     2,849.2 ns |     62.89 ns |     22.43 ns |  1.06 |    0.02 |    1 |         - |          NA |
| DotnetSort                   | 256  | Random             |     2,172.1 ns |    334.96 ns |    175.19 ns |  0.81 |    0.06 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **SingleElementMoved** |     **1,193.1 ns** |     **80.69 ns** |     **35.83 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 256  | SingleElementMoved |     1,026.5 ns |     13.55 ns |      6.02 ns |  0.86 |    0.02 |    1 |         - |          NA |
| QuickSortMedian3             | 256  | SingleElementMoved |     1,728.0 ns |     84.80 ns |     37.65 ns |  1.45 |    0.05 |    3 |         - |          NA |
| QuickSortMedian9             | 256  | SingleElementMoved |     2,279.3 ns |    261.41 ns |    116.07 ns |  1.91 |    0.11 |    4 |         - |          NA |
| DualPivotQuickSort           | 256  | SingleElementMoved |       844.9 ns |     10.02 ns |      3.57 ns |  0.71 |    0.02 |    1 |         - |          NA |
| StableQuickSort              | 256  | SingleElementMoved |     8,745.4 ns |    411.69 ns |    215.32 ns |  7.34 |    0.26 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 256  | SingleElementMoved |     5,479.3 ns |    542.18 ns |    283.57 ns |  4.60 |    0.26 |    7 |         - |          NA |
| DestswapStableQuickSort      | 256  | SingleElementMoved |     4,423.4 ns |    354.30 ns |    185.31 ns |  3.71 |    0.18 |    6 |         - |          NA |
| IntroSort                    | 256  | SingleElementMoved |       866.3 ns |      8.42 ns |      3.74 ns |  0.73 |    0.02 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | SingleElementMoved |     1,124.8 ns |     13.68 ns |      6.07 ns |  0.94 |    0.03 |    1 |         - |          NA |
| PDQSort                      | 256  | SingleElementMoved |     1,140.6 ns |     30.36 ns |     15.88 ns |  0.96 |    0.03 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | SingleElementMoved |     1,431.5 ns |     87.12 ns |     38.68 ns |  1.20 |    0.04 |    2 |         - |          NA |
| Ipnsort                      | 256  | SingleElementMoved |     3,575.2 ns |     33.49 ns |     11.94 ns |  3.00 |    0.08 |    5 |         - |          NA |
| StdSort                      | 256  | SingleElementMoved |     1,747.2 ns |     21.32 ns |      9.47 ns |  1.47 |    0.04 |    3 |         - |          NA |
| BlockQuickSort               | 256  | SingleElementMoved |     1,789.9 ns |     25.77 ns |     11.44 ns |  1.50 |    0.04 |    3 |         - |          NA |
| DotnetSort                   | 256  | SingleElementMoved |       998.3 ns |     19.96 ns |      8.86 ns |  0.84 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Sorted**             |       **829.7 ns** |     **43.35 ns** |     **19.25 ns** |  **1.00** |    **0.03** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 256  | Sorted             |       736.0 ns |      7.88 ns |      2.81 ns |  0.89 |    0.02 |    3 |         - |          NA |
| QuickSortMedian3             | 256  | Sorted             |     1,178.7 ns |     20.80 ns |      9.24 ns |  1.42 |    0.03 |    4 |         - |          NA |
| QuickSortMedian9             | 256  | Sorted             |     1,306.5 ns |     62.42 ns |     32.65 ns |  1.58 |    0.05 |    4 |         - |          NA |
| DualPivotQuickSort           | 256  | Sorted             |       754.2 ns |    144.65 ns |     64.23 ns |  0.91 |    0.08 |    3 |         - |          NA |
| StableQuickSort              | 256  | Sorted             |     9,004.0 ns |     47.00 ns |     16.76 ns | 10.86 |    0.23 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Sorted             |     4,917.9 ns |    610.47 ns |    271.05 ns |  5.93 |    0.33 |    5 |         - |          NA |
| DestswapStableQuickSort      | 256  | Sorted             |     4,214.0 ns |    345.07 ns |    180.48 ns |  5.08 |    0.23 |    5 |         - |          NA |
| IntroSort                    | 256  | Sorted             |       338.7 ns |      5.92 ns |      2.63 ns |  0.41 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 256  | Sorted             |       963.2 ns |      9.70 ns |      5.07 ns |  1.16 |    0.03 |    3 |         - |          NA |
| PDQSort                      | 256  | Sorted             |       389.4 ns |    168.95 ns |     75.01 ns |  0.47 |    0.09 |    2 |         - |          NA |
| PDQSortBranchless            | 256  | Sorted             |       357.2 ns |      3.15 ns |      1.40 ns |  0.43 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 256  | Sorted             |       165.8 ns |      1.32 ns |      0.59 ns |  0.20 |    0.00 |    1 |         - |          NA |
| StdSort                      | 256  | Sorted             |       353.5 ns |      4.17 ns |      1.85 ns |  0.43 |    0.01 |    2 |         - |          NA |
| BlockQuickSort               | 256  | Sorted             |     1,392.6 ns |     13.18 ns |      5.85 ns |  1.68 |    0.04 |    4 |         - |          NA |
| DotnetSort                   | 256  | Sorted             |       801.8 ns |     10.16 ns |      5.31 ns |  0.97 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Reversed**           |       **993.3 ns** |     **23.62 ns** |     **10.49 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 256  | Reversed           |       959.0 ns |      9.13 ns |      4.78 ns |  0.97 |    0.01 |    3 |         - |          NA |
| QuickSortMedian3             | 256  | Reversed           |     1,254.9 ns |     19.06 ns |      6.80 ns |  1.26 |    0.01 |    4 |         - |          NA |
| QuickSortMedian9             | 256  | Reversed           |     1,429.9 ns |     26.07 ns |      9.30 ns |  1.44 |    0.02 |    4 |         - |          NA |
| DualPivotQuickSort           | 256  | Reversed           |     1,001.2 ns |     10.73 ns |      4.76 ns |  1.01 |    0.01 |    3 |         - |          NA |
| StableQuickSort              | 256  | Reversed           |     8,641.7 ns |    285.53 ns |    126.78 ns |  8.70 |    0.15 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Reversed           |     4,907.9 ns |     40.32 ns |     14.38 ns |  4.94 |    0.05 |    5 |         - |          NA |
| DestswapStableQuickSort      | 256  | Reversed           |     7,294.4 ns |    337.76 ns |    149.97 ns |  7.34 |    0.16 |    6 |         - |          NA |
| IntroSort                    | 256  | Reversed           |       495.3 ns |     28.14 ns |     12.49 ns |  0.50 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 256  | Reversed           |     1,421.0 ns |     43.49 ns |     19.31 ns |  1.43 |    0.02 |    4 |         - |          NA |
| PDQSort                      | 256  | Reversed           |       569.6 ns |     50.86 ns |     22.58 ns |  0.57 |    0.02 |    2 |         - |          NA |
| PDQSortBranchless            | 256  | Reversed           |       904.5 ns |      7.16 ns |      3.74 ns |  0.91 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 256  | Reversed           |       242.4 ns |     38.64 ns |     17.15 ns |  0.24 |    0.02 |    1 |         - |          NA |
| StdSort                      | 256  | Reversed           |       919.4 ns |    280.24 ns |    146.57 ns |  0.93 |    0.14 |    3 |         - |          NA |
| BlockQuickSort               | 256  | Reversed           |     1,643.0 ns |     31.29 ns |     13.89 ns |  1.65 |    0.02 |    4 |         - |          NA |
| DotnetSort                   | 256  | Reversed           |     1,217.2 ns |     13.33 ns |      5.92 ns |  1.23 |    0.01 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **PipeOrgan**          |     **7,548.1 ns** |    **401.53 ns** |    **210.01 ns** |  **1.00** |    **0.04** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 256  | PipeOrgan          |     2,479.7 ns |     65.23 ns |     28.96 ns |  0.33 |    0.01 |    2 |         - |          NA |
| QuickSortMedian3             | 256  | PipeOrgan          |     3,248.1 ns |    321.37 ns |    168.08 ns |  0.43 |    0.02 |    2 |         - |          NA |
| QuickSortMedian9             | 256  | PipeOrgan          |     2,582.1 ns |     83.55 ns |     37.10 ns |  0.34 |    0.01 |    2 |         - |          NA |
| DualPivotQuickSort           | 256  | PipeOrgan          |     1,602.1 ns |    137.88 ns |     61.22 ns |  0.21 |    0.01 |    1 |         - |          NA |
| StableQuickSort              | 256  | PipeOrgan          |     8,359.3 ns |     36.78 ns |     19.24 ns |  1.11 |    0.03 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 256  | PipeOrgan          |     4,990.1 ns |     55.74 ns |     19.88 ns |  0.66 |    0.02 |    4 |         - |          NA |
| DestswapStableQuickSort      | 256  | PipeOrgan          |     7,371.1 ns |     42.06 ns |     15.00 ns |  0.98 |    0.03 |    5 |         - |          NA |
| IntroSort                    | 256  | PipeOrgan          |     1,447.3 ns |     34.72 ns |     15.42 ns |  0.19 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | PipeOrgan          |     2,121.1 ns |     72.59 ns |     32.23 ns |  0.28 |    0.01 |    2 |         - |          NA |
| PDQSort                      | 256  | PipeOrgan          |     1,718.1 ns |     98.95 ns |     43.94 ns |  0.23 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | PipeOrgan          |     2,827.1 ns |     42.77 ns |     18.99 ns |  0.37 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 256  | PipeOrgan          |     3,867.2 ns |     64.45 ns |     28.62 ns |  0.51 |    0.01 |    3 |         - |          NA |
| StdSort                      | 256  | PipeOrgan          |     4,882.8 ns |    481.15 ns |    251.65 ns |  0.65 |    0.04 |    4 |         - |          NA |
| BlockQuickSort               | 256  | PipeOrgan          |     4,875.6 ns |     69.07 ns |     24.63 ns |  0.65 |    0.02 |    4 |         - |          NA |
| DotnetSort                   | 256  | PipeOrgan          |     2,486.5 ns |     55.90 ns |     19.94 ns |  0.33 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **ManyDuplicates**     |     **2,372.2 ns** |    **245.83 ns** |    **109.15 ns** |  **1.00** |    **0.06** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 256  | ManyDuplicates     |     1,806.4 ns |    205.23 ns |     91.12 ns |  0.76 |    0.05 |    1 |         - |          NA |
| QuickSortMedian3             | 256  | ManyDuplicates     |     2,737.2 ns |    198.78 ns |     88.26 ns |  1.16 |    0.06 |    1 |         - |          NA |
| QuickSortMedian9             | 256  | ManyDuplicates     |     2,807.6 ns |    166.59 ns |     73.97 ns |  1.19 |    0.06 |    1 |         - |          NA |
| DualPivotQuickSort           | 256  | ManyDuplicates     |     1,883.3 ns |    265.75 ns |    138.99 ns |  0.80 |    0.06 |    1 |         - |          NA |
| StableQuickSort              | 256  | ManyDuplicates     |     6,672.6 ns |    316.19 ns |    165.37 ns |  2.82 |    0.13 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 256  | ManyDuplicates     |     3,765.2 ns |    291.37 ns |    152.39 ns |  1.59 |    0.09 |    2 |         - |          NA |
| DestswapStableQuickSort      | 256  | ManyDuplicates     |     5,265.0 ns |    210.59 ns |    110.14 ns |  2.22 |    0.10 |    3 |         - |          NA |
| IntroSort                    | 256  | ManyDuplicates     |     2,118.3 ns |     38.31 ns |     17.01 ns |  0.89 |    0.04 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | ManyDuplicates     |     1,654.2 ns |     29.66 ns |     10.58 ns |  0.70 |    0.03 |    1 |         - |          NA |
| PDQSort                      | 256  | ManyDuplicates     |     1,668.8 ns |     58.66 ns |     26.04 ns |  0.70 |    0.03 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | ManyDuplicates     |     2,447.1 ns |     48.13 ns |     21.37 ns |  1.03 |    0.04 |    1 |         - |          NA |
| Ipnsort                      | 256  | ManyDuplicates     |     3,675.1 ns |    113.87 ns |     50.56 ns |  1.55 |    0.07 |    2 |         - |          NA |
| StdSort                      | 256  | ManyDuplicates     |     2,696.9 ns |    305.11 ns |    135.47 ns |  1.14 |    0.07 |    1 |         - |          NA |
| BlockQuickSort               | 256  | ManyDuplicates     |     2,576.9 ns |    121.95 ns |     54.15 ns |  1.09 |    0.05 |    1 |         - |          NA |
| DotnetSort                   | 256  | ManyDuplicates     |     1,786.6 ns |     94.26 ns |     41.85 ns |  0.75 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Random**             |    **13,846.6 ns** |    **505.18 ns** |    **264.22 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Random             |    13,041.8 ns |    796.12 ns |    416.39 ns |  0.94 |    0.03 |    1 |         - |          NA |
| QuickSortMedian3             | 1024 | Random             |    13,392.8 ns |    577.47 ns |    256.40 ns |  0.97 |    0.02 |    1 |         - |          NA |
| QuickSortMedian9             | 1024 | Random             |    14,634.3 ns |    337.99 ns |    150.07 ns |  1.06 |    0.02 |    1 |         - |          NA |
| DualPivotQuickSort           | 1024 | Random             |    11,233.9 ns |    718.77 ns |    375.93 ns |  0.81 |    0.03 |    1 |         - |          NA |
| StableQuickSort              | 1024 | Random             |    58,400.8 ns |    257.80 ns |    114.46 ns |  4.22 |    0.08 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Random             |    36,703.4 ns |    692.34 ns |    362.11 ns |  2.65 |    0.05 |    3 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Random             |    31,177.9 ns |    674.37 ns |    352.71 ns |  2.25 |    0.05 |    3 |         - |          NA |
| IntroSort                    | 1024 | Random             |    12,202.1 ns |    467.07 ns |    207.38 ns |  0.88 |    0.02 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | Random             |     9,900.1 ns |    361.22 ns |    160.39 ns |  0.72 |    0.02 |    1 |         - |          NA |
| PDQSort                      | 1024 | Random             |     9,538.6 ns |    471.69 ns |    246.70 ns |  0.69 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | Random             |    13,087.8 ns |    335.39 ns |    148.91 ns |  0.95 |    0.02 |    1 |         - |          NA |
| Ipnsort                      | 1024 | Random             |    19,373.4 ns |    238.54 ns |    124.76 ns |  1.40 |    0.03 |    2 |         - |          NA |
| StdSort                      | 1024 | Random             |    13,420.5 ns |    423.55 ns |    221.53 ns |  0.97 |    0.02 |    1 |         - |          NA |
| BlockQuickSort               | 1024 | Random             |    14,147.3 ns |    340.14 ns |    177.90 ns |  1.02 |    0.02 |    1 |         - |          NA |
| DotnetSort                   | 1024 | Random             |    11,123.2 ns |    602.27 ns |    315.00 ns |  0.80 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **SingleElementMoved** |     **5,592.6 ns** |    **331.43 ns** |    **173.34 ns** |  **1.00** |    **0.04** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 1024 | SingleElementMoved |     5,427.4 ns |    211.37 ns |     93.85 ns |  0.97 |    0.03 |    2 |         - |          NA |
| QuickSortMedian3             | 1024 | SingleElementMoved |     8,127.8 ns |    307.21 ns |    160.68 ns |  1.45 |    0.05 |    3 |         - |          NA |
| QuickSortMedian9             | 1024 | SingleElementMoved |    10,820.2 ns |    489.36 ns |    255.94 ns |  1.94 |    0.07 |    3 |         - |          NA |
| DualPivotQuickSort           | 1024 | SingleElementMoved |     4,164.2 ns |    452.61 ns |    236.72 ns |  0.75 |    0.05 |    1 |         - |          NA |
| StableQuickSort              | 1024 | SingleElementMoved |    43,133.3 ns |    302.77 ns |    158.35 ns |  7.72 |    0.23 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | SingleElementMoved |    27,032.0 ns |    398.84 ns |    208.60 ns |  4.84 |    0.15 |    5 |         - |          NA |
| DestswapStableQuickSort      | 1024 | SingleElementMoved |    20,882.9 ns |  1,635.28 ns |    855.28 ns |  3.74 |    0.18 |    4 |         - |          NA |
| IntroSort                    | 1024 | SingleElementMoved |     3,985.6 ns |     29.00 ns |     10.34 ns |  0.71 |    0.02 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | SingleElementMoved |     5,640.2 ns |     55.69 ns |     19.86 ns |  1.01 |    0.03 |    2 |         - |          NA |
| PDQSort                      | 1024 | SingleElementMoved |     4,975.3 ns |    260.13 ns |    136.05 ns |  0.89 |    0.03 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | SingleElementMoved |     6,045.6 ns |    206.43 ns |     91.65 ns |  1.08 |    0.04 |    2 |         - |          NA |
| Ipnsort                      | 1024 | SingleElementMoved |    17,925.7 ns |     89.76 ns |     46.95 ns |  3.21 |    0.10 |    4 |         - |          NA |
| StdSort                      | 1024 | SingleElementMoved |     7,856.7 ns |    217.35 ns |    113.68 ns |  1.41 |    0.05 |    3 |         - |          NA |
| BlockQuickSort               | 1024 | SingleElementMoved |     9,380.7 ns |    466.05 ns |    206.93 ns |  1.68 |    0.06 |    3 |         - |          NA |
| DotnetSort                   | 1024 | SingleElementMoved |     5,432.1 ns |    372.35 ns |    165.32 ns |  0.97 |    0.04 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Sorted**             |     **4,017.1 ns** |     **83.36 ns** |     **37.01 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Sorted             |     3,738.2 ns |     27.34 ns |      9.75 ns |  0.93 |    0.01 |    3 |         - |          NA |
| QuickSortMedian3             | 1024 | Sorted             |     5,583.8 ns |    227.56 ns |    119.02 ns |  1.39 |    0.03 |    3 |         - |          NA |
| QuickSortMedian9             | 1024 | Sorted             |     6,095.5 ns |    306.67 ns |    160.40 ns |  1.52 |    0.04 |    3 |         - |          NA |
| DualPivotQuickSort           | 1024 | Sorted             |     3,643.2 ns |    486.39 ns |    173.45 ns |  0.91 |    0.04 |    3 |         - |          NA |
| StableQuickSort              | 1024 | Sorted             |    46,876.7 ns |    683.95 ns |    303.68 ns | 11.67 |    0.12 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Sorted             |    22,755.9 ns |    538.92 ns |    281.87 ns |  5.67 |    0.08 |    5 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Sorted             |    19,617.9 ns |    385.93 ns |    171.36 ns |  4.88 |    0.06 |    5 |         - |          NA |
| IntroSort                    | 1024 | Sorted             |     1,022.8 ns |      4.72 ns |      2.10 ns |  0.25 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet              | 1024 | Sorted             |     4,689.8 ns |     57.93 ns |     25.72 ns |  1.17 |    0.01 |    3 |         - |          NA |
| PDQSort                      | 1024 | Sorted             |     1,325.3 ns |      3.76 ns |      1.67 ns |  0.33 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | Sorted             |     1,332.2 ns |      7.29 ns |      3.81 ns |  0.33 |    0.00 |    2 |         - |          NA |
| Ipnsort                      | 1024 | Sorted             |       585.6 ns |      4.52 ns |      2.01 ns |  0.15 |    0.00 |    1 |         - |          NA |
| StdSort                      | 1024 | Sorted             |     1,202.6 ns |      5.98 ns |      2.65 ns |  0.30 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | Sorted             |     7,553.4 ns |    453.88 ns |    237.39 ns |  1.88 |    0.06 |    4 |         - |          NA |
| DotnetSort                   | 1024 | Sorted             |     3,979.8 ns |     29.18 ns |     10.41 ns |  0.99 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Reversed**           |     **4,825.5 ns** |    **348.01 ns** |    **182.02 ns** |  **1.00** |    **0.05** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Reversed           |     4,708.8 ns |    389.13 ns |    203.52 ns |  0.98 |    0.05 |    4 |         - |          NA |
| QuickSortMedian3             | 1024 | Reversed           |     6,026.4 ns |    416.21 ns |    217.68 ns |  1.25 |    0.06 |    4 |         - |          NA |
| QuickSortMedian9             | 1024 | Reversed           |     6,363.2 ns |    251.32 ns |    131.44 ns |  1.32 |    0.05 |    4 |         - |          NA |
| DualPivotQuickSort           | 1024 | Reversed           |     5,115.9 ns |    446.26 ns |    233.40 ns |  1.06 |    0.06 |    4 |         - |          NA |
| StableQuickSort              | 1024 | Reversed           |    42,937.2 ns |    238.36 ns |    124.67 ns |  8.91 |    0.32 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Reversed           |    24,786.0 ns |    908.43 ns |    475.12 ns |  5.14 |    0.20 |    5 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Reversed           |    33,026.8 ns |    715.05 ns |    373.99 ns |  6.85 |    0.25 |    6 |         - |          NA |
| IntroSort                    | 1024 | Reversed           |     3,160.5 ns |    474.25 ns |    248.04 ns |  0.66 |    0.05 |    3 |         - |          NA |
| IntroSortDotnet              | 1024 | Reversed           |     7,261.5 ns |    409.83 ns |    214.35 ns |  1.51 |    0.07 |    4 |         - |          NA |
| PDQSort                      | 1024 | Reversed           |     2,069.4 ns |     11.19 ns |      4.97 ns |  0.43 |    0.02 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | Reversed           |     3,236.8 ns |      8.62 ns |      4.51 ns |  0.67 |    0.02 |    3 |         - |          NA |
| Ipnsort                      | 1024 | Reversed           |       911.7 ns |      1.36 ns |      0.71 ns |  0.19 |    0.01 |    1 |         - |          NA |
| StdSort                      | 1024 | Reversed           |     2,931.0 ns |      4.43 ns |      1.97 ns |  0.61 |    0.02 |    3 |         - |          NA |
| BlockQuickSort               | 1024 | Reversed           |     8,594.5 ns |    222.42 ns |    116.33 ns |  1.78 |    0.07 |    4 |         - |          NA |
| DotnetSort                   | 1024 | Reversed           |     6,791.7 ns |    622.92 ns |    325.80 ns |  1.41 |    0.08 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **PipeOrgan**          |   **108,757.3 ns** |    **387.42 ns** |    **202.63 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 1024 | PipeOrgan          |    19,517.9 ns | 10,585.40 ns |  5,536.37 ns |  0.18 |    0.05 |    1 |         - |          NA |
| QuickSortMedian3             | 1024 | PipeOrgan          |    16,380.1 ns |    365.82 ns |    191.33 ns |  0.15 |    0.00 |    1 |         - |          NA |
| QuickSortMedian9             | 1024 | PipeOrgan          |    12,367.8 ns |    620.55 ns |    324.56 ns |  0.11 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort           | 1024 | PipeOrgan          |     7,748.8 ns |    107.59 ns |     56.27 ns |  0.07 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 1024 | PipeOrgan          |    43,164.6 ns |    288.99 ns |    128.31 ns |  0.40 |    0.00 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | PipeOrgan          |    24,812.1 ns |    268.41 ns |    140.38 ns |  0.23 |    0.00 |    2 |         - |          NA |
| DestswapStableQuickSort      | 1024 | PipeOrgan          |    36,032.5 ns |    409.13 ns |    181.66 ns |  0.33 |    0.00 |    3 |         - |          NA |
| IntroSort                    | 1024 | PipeOrgan          |    10,779.7 ns |    724.93 ns |    321.87 ns |  0.10 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | PipeOrgan          |    14,445.3 ns |    230.21 ns |    102.22 ns |  0.13 |    0.00 |    1 |         - |          NA |
| PDQSort                      | 1024 | PipeOrgan          |     9,036.5 ns |    526.37 ns |    233.71 ns |  0.08 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | PipeOrgan          |    15,267.0 ns |    170.88 ns |     75.87 ns |  0.14 |    0.00 |    1 |         - |          NA |
| Ipnsort                      | 1024 | PipeOrgan          |    21,004.4 ns |    186.04 ns |     97.30 ns |  0.19 |    0.00 |    2 |         - |          NA |
| StdSort                      | 1024 | PipeOrgan          |    21,520.1 ns |    261.27 ns |    136.65 ns |  0.20 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | PipeOrgan          |    24,831.3 ns |    382.69 ns |    169.92 ns |  0.23 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 1024 | PipeOrgan          |    16,574.5 ns |  1,065.82 ns |    557.44 ns |  0.15 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **ManyDuplicates**     |     **9,795.0 ns** |    **473.62 ns** |    **247.71 ns** |  **1.00** |    **0.03** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 1024 | ManyDuplicates     |     7,864.1 ns |    499.37 ns |    261.18 ns |  0.80 |    0.03 |    2 |         - |          NA |
| QuickSortMedian3             | 1024 | ManyDuplicates     |    12,238.4 ns |    752.57 ns |    393.61 ns |  1.25 |    0.05 |    2 |         - |          NA |
| QuickSortMedian9             | 1024 | ManyDuplicates     |    12,394.0 ns |    393.50 ns |    174.72 ns |  1.27 |    0.03 |    2 |         - |          NA |
| DualPivotQuickSort           | 1024 | ManyDuplicates     |     7,822.9 ns |    247.18 ns |    129.28 ns |  0.80 |    0.02 |    2 |         - |          NA |
| StableQuickSort              | 1024 | ManyDuplicates     |    29,630.0 ns |    429.90 ns |    224.85 ns |  3.03 |    0.08 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | ManyDuplicates     |    14,137.2 ns |    346.28 ns |    123.49 ns |  1.44 |    0.04 |    2 |         - |          NA |
| DestswapStableQuickSort      | 1024 | ManyDuplicates     |    14,378.2 ns |    471.83 ns |    246.77 ns |  1.47 |    0.04 |    2 |         - |          NA |
| IntroSort                    | 1024 | ManyDuplicates     |    10,770.1 ns |    588.56 ns |    307.83 ns |  1.10 |    0.04 |    2 |         - |          NA |
| IntroSortDotnet              | 1024 | ManyDuplicates     |     8,158.9 ns |     89.53 ns |     39.75 ns |  0.83 |    0.02 |    2 |         - |          NA |
| PDQSort                      | 1024 | ManyDuplicates     |     6,024.2 ns |    366.45 ns |    191.66 ns |  0.62 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | ManyDuplicates     |     8,952.5 ns |    290.65 ns |    152.02 ns |  0.91 |    0.03 |    2 |         - |          NA |
| Ipnsort                      | 1024 | ManyDuplicates     |    18,083.7 ns |    236.88 ns |    105.18 ns |  1.85 |    0.05 |    3 |         - |          NA |
| StdSort                      | 1024 | ManyDuplicates     |    11,118.9 ns |    338.51 ns |    177.05 ns |  1.14 |    0.03 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | ManyDuplicates     |    12,116.8 ns |    266.10 ns |    139.17 ns |  1.24 |    0.03 |    2 |         - |          NA |
| DotnetSort                   | 1024 | ManyDuplicates     |     8,142.5 ns |     66.13 ns |     29.36 ns |  0.83 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Random**             |    **71,659.5 ns** |  **9,106.78 ns** |  **4,763.02 ns** |  **1.00** |    **0.09** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Random             |    73,464.7 ns | 13,270.38 ns |  6,940.67 ns |  1.03 |    0.11 |    1 |         - |          NA |
| QuickSortMedian3             | 4096 | Random             |    74,190.6 ns | 15,010.94 ns |  7,851.01 ns |  1.04 |    0.12 |    1 |         - |          NA |
| QuickSortMedian9             | 4096 | Random             |    68,223.4 ns |    424.86 ns |    188.64 ns |  0.96 |    0.06 |    1 |         - |          NA |
| DualPivotQuickSort           | 4096 | Random             |    55,833.6 ns |  3,431.44 ns |  1,794.71 ns |  0.78 |    0.06 |    1 |         - |          NA |
| StableQuickSort              | 4096 | Random             |   570,161.7 ns |  3,241.13 ns |  1,695.17 ns |  7.99 |    0.51 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Random             |   437,108.5 ns |  4,136.18 ns |  2,163.30 ns |  6.12 |    0.39 |    4 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Random             |   158,884.9 ns |  5,380.62 ns |  2,389.03 ns |  2.23 |    0.15 |    3 |         - |          NA |
| IntroSort                    | 4096 | Random             |    61,468.3 ns |    699.78 ns |    310.71 ns |  0.86 |    0.06 |    1 |         - |          NA |
| IntroSortDotnet              | 4096 | Random             |    48,922.3 ns |    835.90 ns |    371.14 ns |  0.69 |    0.04 |    1 |         - |          NA |
| PDQSort                      | 4096 | Random             |    46,560.0 ns |  1,990.30 ns |    883.71 ns |  0.65 |    0.04 |    1 |         - |          NA |
| PDQSortBranchless            | 4096 | Random             |    62,003.5 ns |  1,443.47 ns |    754.96 ns |  0.87 |    0.06 |    1 |         - |          NA |
| Ipnsort                      | 4096 | Random             |    97,995.7 ns |    458.43 ns |    203.55 ns |  1.37 |    0.09 |    2 |         - |          NA |
| StdSort                      | 4096 | Random             |    62,495.5 ns |    639.24 ns |    334.34 ns |  0.88 |    0.06 |    1 |         - |          NA |
| BlockQuickSort               | 4096 | Random             |    68,746.9 ns |    918.29 ns |    407.73 ns |  0.96 |    0.06 |    1 |         - |          NA |
| DotnetSort                   | 4096 | Random             |    53,275.8 ns |  2,024.65 ns |  1,058.93 ns |  0.75 |    0.05 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **SingleElementMoved** |    **25,761.1 ns** |  **1,042.42 ns** |    **545.20 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 4096 | SingleElementMoved |    26,643.3 ns |  1,058.20 ns |    553.46 ns |  1.03 |    0.03 |    1 |         - |          NA |
| QuickSortMedian3             | 4096 | SingleElementMoved |    35,154.0 ns |    954.44 ns |    423.78 ns |  1.37 |    0.03 |    1 |         - |          NA |
| QuickSortMedian9             | 4096 | SingleElementMoved |    47,644.6 ns |    865.90 ns |    384.47 ns |  1.85 |    0.04 |    2 |         - |          NA |
| DualPivotQuickSort           | 4096 | SingleElementMoved |    22,780.4 ns |    581.23 ns |    304.00 ns |  0.88 |    0.02 |    1 |         - |          NA |
| StableQuickSort              | 4096 | SingleElementMoved |   208,795.9 ns |  2,177.90 ns |    967.00 ns |  8.11 |    0.17 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | SingleElementMoved |   123,437.0 ns |  1,026.01 ns |    536.62 ns |  4.79 |    0.10 |    4 |         - |          NA |
| DestswapStableQuickSort      | 4096 | SingleElementMoved |    95,900.1 ns |    937.97 ns |    416.46 ns |  3.72 |    0.08 |    3 |         - |          NA |
| IntroSort                    | 4096 | SingleElementMoved |    18,566.2 ns |    296.31 ns |    105.67 ns |  0.72 |    0.02 |    1 |         - |          NA |
| IntroSortDotnet              | 4096 | SingleElementMoved |    28,118.1 ns |  1,033.73 ns |    540.66 ns |  1.09 |    0.03 |    1 |         - |          NA |
| PDQSort                      | 4096 | SingleElementMoved |    21,671.4 ns |    801.23 ns |    355.75 ns |  0.84 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless            | 4096 | SingleElementMoved |    26,647.9 ns |  1,157.90 ns |    605.60 ns |  1.03 |    0.03 |    1 |         - |          NA |
| Ipnsort                      | 4096 | SingleElementMoved |    87,669.3 ns |    772.12 ns |    403.83 ns |  3.40 |    0.07 |    3 |         - |          NA |
| StdSort                      | 4096 | SingleElementMoved |    32,777.8 ns |    924.84 ns |    483.71 ns |  1.27 |    0.03 |    1 |         - |          NA |
| BlockQuickSort               | 4096 | SingleElementMoved |    44,201.1 ns |    972.53 ns |    431.81 ns |  1.72 |    0.04 |    2 |         - |          NA |
| DotnetSort                   | 4096 | SingleElementMoved |    28,012.2 ns |  3,136.75 ns |  1,640.58 ns |  1.09 |    0.06 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Sorted**             |    **19,777.9 ns** |    **523.61 ns** |    **273.86 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Sorted             |    19,574.8 ns |  2,225.96 ns |  1,164.22 ns |  0.99 |    0.06 |    3 |         - |          NA |
| QuickSortMedian3             | 4096 | Sorted             |    25,492.7 ns |    530.24 ns |    277.33 ns |  1.29 |    0.02 |    3 |         - |          NA |
| QuickSortMedian9             | 4096 | Sorted             |    27,606.3 ns |    643.05 ns |    336.33 ns |  1.40 |    0.02 |    3 |         - |          NA |
| DualPivotQuickSort           | 4096 | Sorted             |    20,787.2 ns |  1,667.27 ns |    872.01 ns |  1.05 |    0.04 |    3 |         - |          NA |
| StableQuickSort              | 4096 | Sorted             |   226,409.6 ns |  1,295.03 ns |    677.33 ns | 11.45 |    0.15 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Sorted             |   107,420.4 ns |  1,676.47 ns |    744.37 ns |  5.43 |    0.08 |    5 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Sorted             |    93,693.9 ns |  2,645.35 ns |  1,383.57 ns |  4.74 |    0.09 |    5 |         - |          NA |
| IntroSort                    | 4096 | Sorted             |     4,055.3 ns |    447.68 ns |    234.15 ns |  0.21 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 4096 | Sorted             |    22,543.0 ns |    543.01 ns |    284.01 ns |  1.14 |    0.02 |    3 |         - |          NA |
| PDQSort                      | 4096 | Sorted             |     5,372.4 ns |  1,108.17 ns |    492.03 ns |  0.27 |    0.02 |    2 |         - |          NA |
| PDQSortBranchless            | 4096 | Sorted             |     5,241.1 ns |    396.16 ns |    207.20 ns |  0.27 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 4096 | Sorted             |     2,250.9 ns |     12.23 ns |      6.39 ns |  0.11 |    0.00 |    1 |         - |          NA |
| StdSort                      | 4096 | Sorted             |     4,453.1 ns |     52.01 ns |     18.55 ns |  0.23 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 4096 | Sorted             |    36,206.0 ns |    756.68 ns |    335.97 ns |  1.83 |    0.03 |    4 |         - |          NA |
| DotnetSort                   | 4096 | Sorted             |    21,577.4 ns |  5,558.94 ns |  2,907.43 ns |  1.09 |    0.14 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Reversed**           |    **21,866.7 ns** |    **594.00 ns** |    **263.74 ns** |  **1.00** |    **0.02** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Reversed           |    22,532.3 ns |  1,867.64 ns |    829.24 ns |  1.03 |    0.04 |    4 |         - |          NA |
| QuickSortMedian3             | 4096 | Reversed           |    27,005.1 ns |    753.77 ns |    394.24 ns |  1.24 |    0.02 |    4 |         - |          NA |
| QuickSortMedian9             | 4096 | Reversed           |    28,565.3 ns |    410.67 ns |    214.79 ns |  1.31 |    0.02 |    4 |         - |          NA |
| DualPivotQuickSort           | 4096 | Reversed           |    25,020.1 ns |  1,727.86 ns |    767.18 ns |  1.14 |    0.04 |    4 |         - |          NA |
| StableQuickSort              | 4096 | Reversed           |   207,032.9 ns |  1,155.06 ns |    604.12 ns |  9.47 |    0.11 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Reversed           |   116,668.2 ns |  1,854.80 ns |    970.10 ns |  5.34 |    0.07 |    6 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Reversed           |   144,188.9 ns |  3,273.61 ns |  1,712.16 ns |  6.59 |    0.10 |    7 |         - |          NA |
| IntroSort                    | 4096 | Reversed           |    13,375.8 ns |    495.84 ns |    220.15 ns |  0.61 |    0.01 |    3 |         - |          NA |
| IntroSortDotnet              | 4096 | Reversed           |    34,621.3 ns |    515.56 ns |    228.91 ns |  1.58 |    0.02 |    5 |         - |          NA |
| PDQSort                      | 4096 | Reversed           |     8,460.7 ns |    313.14 ns |    139.04 ns |  0.39 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 4096 | Reversed           |    12,758.1 ns |    286.38 ns |    127.15 ns |  0.58 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 4096 | Reversed           |     3,656.9 ns |    281.61 ns |    147.29 ns |  0.17 |    0.01 |    1 |         - |          NA |
| StdSort                      | 4096 | Reversed           |    11,354.4 ns |    413.74 ns |    183.70 ns |  0.52 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 4096 | Reversed           |    39,624.2 ns |    395.23 ns |    206.71 ns |  1.81 |    0.02 |    5 |         - |          NA |
| DotnetSort                   | 4096 | Reversed           |    41,518.8 ns |  5,086.67 ns |  2,660.43 ns |  1.90 |    0.12 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **PipeOrgan**          | **1,581,934.6 ns** |  **3,181.15 ns** |  **1,663.80 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| QuickSort3way                | 4096 | PipeOrgan          |    84,121.1 ns |  4,162.83 ns |  2,177.24 ns |  0.05 |    0.00 |    3 |         - |          NA |
| QuickSortMedian3             | 4096 | PipeOrgan          |    82,484.3 ns |  2,134.68 ns |  1,116.48 ns |  0.05 |    0.00 |    3 |         - |          NA |
| QuickSortMedian9             | 4096 | PipeOrgan          |    54,793.1 ns |  1,767.35 ns |    784.71 ns |  0.03 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort           | 4096 | PipeOrgan          |    39,741.1 ns |  1,886.12 ns |    986.48 ns |  0.03 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 4096 | PipeOrgan          |   208,298.0 ns |  1,510.01 ns |    789.77 ns |  0.13 |    0.00 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | PipeOrgan          |   119,890.7 ns |  3,898.84 ns |  2,039.17 ns |  0.08 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 4096 | PipeOrgan          |   171,882.2 ns |  2,168.52 ns |    962.84 ns |  0.11 |    0.00 |    4 |         - |          NA |
| IntroSort                    | 4096 | PipeOrgan          |    77,759.4 ns |  4,174.63 ns |  2,183.41 ns |  0.05 |    0.00 |    3 |         - |          NA |
| IntroSortDotnet              | 4096 | PipeOrgan          |    83,952.5 ns |  1,323.31 ns |    692.11 ns |  0.05 |    0.00 |    3 |         - |          NA |
| PDQSort                      | 4096 | PipeOrgan          |    41,950.5 ns |    977.30 ns |    511.15 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 4096 | PipeOrgan          |    73,368.8 ns |  1,278.42 ns |    567.63 ns |  0.05 |    0.00 |    3 |         - |          NA |
| Ipnsort                      | 4096 | PipeOrgan          |   106,345.4 ns |    882.85 ns |    391.99 ns |  0.07 |    0.00 |    3 |         - |          NA |
| StdSort                      | 4096 | PipeOrgan          |   108,350.3 ns |    914.16 ns |    478.12 ns |  0.07 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 4096 | PipeOrgan          |   107,966.9 ns |    839.24 ns |    438.94 ns |  0.07 |    0.00 |    3 |         - |          NA |
| DotnetSort                   | 4096 | PipeOrgan          |    91,582.4 ns |  2,077.63 ns |    922.48 ns |  0.06 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **ManyDuplicates**     |    **42,884.5 ns** |    **991.23 ns** |    **440.11 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 4096 | ManyDuplicates     |    32,787.6 ns |  1,968.46 ns |  1,029.54 ns |  0.76 |    0.02 |    2 |         - |          NA |
| QuickSortMedian3             | 4096 | ManyDuplicates     |    52,383.0 ns |  1,051.12 ns |    466.71 ns |  1.22 |    0.02 |    2 |         - |          NA |
| QuickSortMedian9             | 4096 | ManyDuplicates     |    55,431.9 ns |    681.94 ns |    302.78 ns |  1.29 |    0.01 |    2 |         - |          NA |
| DualPivotQuickSort           | 4096 | ManyDuplicates     |    28,026.7 ns |  1,520.53 ns |    795.27 ns |  0.65 |    0.02 |    2 |         - |          NA |
| StableQuickSort              | 4096 | ManyDuplicates     |   116,294.3 ns | 11,652.81 ns |  6,094.64 ns |  2.71 |    0.14 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | ManyDuplicates     |    53,706.7 ns |    825.99 ns |    366.75 ns |  1.25 |    0.01 |    2 |         - |          NA |
| DestswapStableQuickSort      | 4096 | ManyDuplicates     |    55,056.9 ns |  2,027.99 ns |  1,060.68 ns |  1.28 |    0.03 |    2 |         - |          NA |
| IntroSort                    | 4096 | ManyDuplicates     |    50,152.8 ns |    901.17 ns |    471.33 ns |  1.17 |    0.02 |    2 |         - |          NA |
| IntroSortDotnet              | 4096 | ManyDuplicates     |    37,400.5 ns |    367.65 ns |    163.24 ns |  0.87 |    0.01 |    2 |         - |          NA |
| PDQSort                      | 4096 | ManyDuplicates     |    21,859.6 ns |    687.88 ns |    305.42 ns |  0.51 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 4096 | ManyDuplicates     |    30,507.0 ns |    755.01 ns |    335.23 ns |  0.71 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 4096 | ManyDuplicates     |    59,946.5 ns |    430.13 ns |    190.98 ns |  1.40 |    0.01 |    2 |         - |          NA |
| StdSort                      | 4096 | ManyDuplicates     |    33,538.9 ns |    943.15 ns |    418.77 ns |  0.78 |    0.01 |    2 |         - |          NA |
| BlockQuickSort               | 4096 | ManyDuplicates     |    52,729.4 ns |    489.37 ns |    255.95 ns |  1.23 |    0.01 |    2 |         - |          NA |
| DotnetSort                   | 4096 | ManyDuplicates     |    36,053.3 ns |    572.89 ns |    254.36 ns |  0.84 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Random**             |   **471,609.5 ns** | **10,725.84 ns** |  **5,609.82 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Random             |   413,603.5 ns |  6,839.66 ns |  3,577.28 ns |  0.88 |    0.01 |    3 |         - |          NA |
| QuickSortMedian3             | 8192 | Random             |   467,176.0 ns |  8,369.85 ns |  4,377.60 ns |  0.99 |    0.01 |    3 |         - |          NA |
| QuickSortMedian9             | 8192 | Random             |   506,038.8 ns |  3,374.51 ns |  1,764.93 ns |  1.07 |    0.01 |    3 |         - |          NA |
| DualPivotQuickSort           | 8192 | Random             |   331,486.1 ns |  3,583.08 ns |  1,590.91 ns |  0.70 |    0.01 |    3 |         - |          NA |
| StableQuickSort              | 8192 | Random             | 1,309,009.4 ns |  2,753.96 ns |  1,222.77 ns |  2.78 |    0.03 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Random             | 1,049,691.4 ns |  1,817.93 ns |    807.17 ns |  2.23 |    0.02 |    5 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Random             |   860,335.8 ns |  2,928.02 ns |  1,531.41 ns |  1.82 |    0.02 |    4 |         - |          NA |
| IntroSort                    | 8192 | Random             |   394,559.5 ns |  1,889.71 ns |    988.36 ns |  0.84 |    0.01 |    3 |         - |          NA |
| IntroSortDotnet              | 8192 | Random             |   328,079.8 ns | 20,851.47 ns | 10,905.72 ns |  0.70 |    0.02 |    3 |         - |          NA |
| PDQSort                      | 8192 | Random             |   330,029.7 ns |  7,452.31 ns |  3,308.87 ns |  0.70 |    0.01 |    3 |         - |          NA |
| PDQSortBranchless            | 8192 | Random             |   138,589.0 ns |  2,424.96 ns |  1,268.30 ns |  0.29 |    0.00 |    1 |         - |          NA |
| Ipnsort                      | 8192 | Random             |   213,378.8 ns |    757.43 ns |    396.15 ns |  0.45 |    0.01 |    2 |         - |          NA |
| StdSort                      | 8192 | Random             |   133,896.3 ns |  2,084.86 ns |  1,090.42 ns |  0.28 |    0.00 |    1 |         - |          NA |
| BlockQuickSort               | 8192 | Random             |   150,952.3 ns |  3,305.87 ns |  1,729.03 ns |  0.32 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 8192 | Random             |   330,084.0 ns |  7,672.86 ns |  3,406.80 ns |  0.70 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **SingleElementMoved** |    **53,739.2 ns** |  **1,377.07 ns** |    **611.43 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 8192 | SingleElementMoved |    57,702.7 ns |  1,723.83 ns |    901.59 ns |  1.07 |    0.02 |    1 |         - |          NA |
| QuickSortMedian3             | 8192 | SingleElementMoved |    74,834.9 ns |    855.47 ns |    447.43 ns |  1.39 |    0.02 |    1 |         - |          NA |
| QuickSortMedian9             | 8192 | SingleElementMoved |    99,150.2 ns |    644.54 ns |    286.18 ns |  1.85 |    0.02 |    2 |         - |          NA |
| DualPivotQuickSort           | 8192 | SingleElementMoved |    49,642.9 ns |  1,013.30 ns |    529.98 ns |  0.92 |    0.01 |    1 |         - |          NA |
| StableQuickSort              | 8192 | SingleElementMoved |   450,086.2 ns |  1,800.94 ns |    642.23 ns |  8.38 |    0.09 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | SingleElementMoved |   273,747.3 ns | 13,788.71 ns |  7,211.76 ns |  5.09 |    0.14 |    4 |         - |          NA |
| DestswapStableQuickSort      | 8192 | SingleElementMoved |   201,941.3 ns |  3,379.50 ns |  1,500.52 ns |  3.76 |    0.05 |    3 |         - |          NA |
| IntroSort                    | 8192 | SingleElementMoved |    43,171.2 ns |  7,096.76 ns |  3,711.74 ns |  0.80 |    0.07 |    1 |         - |          NA |
| IntroSortDotnet              | 8192 | SingleElementMoved |    59,845.3 ns |    762.35 ns |    398.73 ns |  1.11 |    0.01 |    1 |         - |          NA |
| PDQSort                      | 8192 | SingleElementMoved |    44,081.0 ns |    681.01 ns |    302.37 ns |  0.82 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 8192 | SingleElementMoved |    54,130.2 ns |    444.61 ns |    232.54 ns |  1.01 |    0.01 |    1 |         - |          NA |
| Ipnsort                      | 8192 | SingleElementMoved |   198,392.8 ns | 18,410.84 ns |  8,174.53 ns |  3.69 |    0.15 |    3 |         - |          NA |
| StdSort                      | 8192 | SingleElementMoved |    64,344.3 ns |    603.88 ns |    315.84 ns |  1.20 |    0.01 |    1 |         - |          NA |
| BlockQuickSort               | 8192 | SingleElementMoved |    95,191.6 ns |    803.28 ns |    420.13 ns |  1.77 |    0.02 |    2 |         - |          NA |
| DotnetSort                   | 8192 | SingleElementMoved |    58,931.9 ns |  2,262.95 ns |  1,183.57 ns |  1.10 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Sorted**             |    **42,636.7 ns** |  **1,721.46 ns** |    **900.36 ns** |  **1.00** |    **0.03** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Sorted             |    40,195.5 ns |  1,377.44 ns |    611.59 ns |  0.94 |    0.02 |    3 |         - |          NA |
| QuickSortMedian3             | 8192 | Sorted             |    54,409.6 ns |  1,343.06 ns |    702.45 ns |  1.28 |    0.03 |    3 |         - |          NA |
| QuickSortMedian9             | 8192 | Sorted             |    58,197.8 ns |    504.60 ns |    263.92 ns |  1.37 |    0.03 |    3 |         - |          NA |
| DualPivotQuickSort           | 8192 | Sorted             |    44,712.7 ns |  1,156.75 ns |    605.00 ns |  1.05 |    0.02 |    3 |         - |          NA |
| StableQuickSort              | 8192 | Sorted             |   492,698.6 ns |  2,410.12 ns |  1,070.11 ns | 11.56 |    0.23 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Sorted             |   231,325.1 ns |  3,096.45 ns |  1,619.50 ns |  5.43 |    0.11 |    5 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Sorted             |   195,979.4 ns |  2,056.92 ns |    913.29 ns |  4.60 |    0.09 |    5 |         - |          NA |
| IntroSort                    | 8192 | Sorted             |     7,786.6 ns |    350.40 ns |    183.27 ns |  0.18 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 8192 | Sorted             |    47,853.9 ns |    849.02 ns |    444.06 ns |  1.12 |    0.02 |    3 |         - |          NA |
| PDQSort                      | 8192 | Sorted             |    10,281.4 ns |    418.47 ns |    185.80 ns |  0.24 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | Sorted             |    10,464.6 ns |    401.00 ns |    178.05 ns |  0.25 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 8192 | Sorted             |     4,718.6 ns |    377.64 ns |    197.51 ns |  0.11 |    0.00 |    1 |         - |          NA |
| StdSort                      | 8192 | Sorted             |     8,993.3 ns |    379.61 ns |    198.54 ns |  0.21 |    0.01 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | Sorted             |    79,920.8 ns |  2,981.92 ns |  1,323.99 ns |  1.88 |    0.05 |    4 |         - |          NA |
| DotnetSort                   | 8192 | Sorted             |    46,294.3 ns |  6,536.30 ns |  3,418.61 ns |  1.09 |    0.08 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Reversed**           |    **46,874.3 ns** |    **692.30 ns** |    **307.38 ns** |  **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Reversed           |    49,346.3 ns |  4,529.48 ns |  2,369.01 ns |  1.05 |    0.05 |    4 |         - |          NA |
| QuickSortMedian3             | 8192 | Reversed           |    56,762.2 ns |    945.12 ns |    494.32 ns |  1.21 |    0.01 |    4 |         - |          NA |
| QuickSortMedian9             | 8192 | Reversed           |    60,754.6 ns |    794.94 ns |    352.96 ns |  1.30 |    0.01 |    4 |         - |          NA |
| DualPivotQuickSort           | 8192 | Reversed           |    54,110.8 ns |  1,111.62 ns |    493.56 ns |  1.15 |    0.01 |    4 |         - |          NA |
| StableQuickSort              | 8192 | Reversed           |   449,226.4 ns |  1,019.47 ns |    533.20 ns |  9.58 |    0.06 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Reversed           |   251,685.1 ns |  3,489.85 ns |  1,825.26 ns |  5.37 |    0.05 |    6 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Reversed           |   310,934.8 ns |  3,910.17 ns |  2,045.10 ns |  6.63 |    0.06 |    7 |         - |          NA |
| IntroSort                    | 8192 | Reversed           |    27,040.0 ns |  1,461.46 ns |    648.90 ns |  0.58 |    0.01 |    3 |         - |          NA |
| IntroSortDotnet              | 8192 | Reversed           |    75,699.1 ns |    915.58 ns |    478.87 ns |  1.61 |    0.01 |    5 |         - |          NA |
| PDQSort                      | 8192 | Reversed           |    16,737.3 ns |  1,463.30 ns |    765.33 ns |  0.36 |    0.02 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | Reversed           |    25,368.5 ns |    790.89 ns |    351.16 ns |  0.54 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 8192 | Reversed           |     7,076.3 ns |      8.14 ns |      2.90 ns |  0.15 |    0.00 |    1 |         - |          NA |
| StdSort                      | 8192 | Reversed           |    22,744.3 ns |    923.36 ns |    482.94 ns |  0.49 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 8192 | Reversed           |    85,506.7 ns |    533.43 ns |    278.99 ns |  1.82 |    0.01 |    5 |         - |          NA |
| DotnetSort                   | 8192 | Reversed           |    93,873.2 ns |  4,004.09 ns |  1,777.84 ns |  2.00 |    0.04 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **PipeOrgan**          | **6,161,055.2 ns** |  **9,375.04 ns** |  **4,903.33 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 8192 | PipeOrgan          |   196,718.5 ns |  7,236.98 ns |  3,785.08 ns |  0.03 |    0.00 |    3 |         - |          NA |
| QuickSortMedian3             | 8192 | PipeOrgan          |   193,861.5 ns |  5,004.78 ns |  2,617.59 ns |  0.03 |    0.00 |    3 |         - |          NA |
| QuickSortMedian9             | 8192 | PipeOrgan          |   114,740.0 ns |  1,168.59 ns |    416.73 ns |  0.02 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort           | 8192 | PipeOrgan          |    85,722.4 ns |  3,328.45 ns |  1,740.84 ns |  0.01 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 8192 | PipeOrgan          |   451,409.1 ns |  2,012.78 ns |    717.78 ns |  0.07 |    0.00 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | PipeOrgan          |   261,157.2 ns | 14,084.34 ns |  7,366.38 ns |  0.04 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 8192 | PipeOrgan          |   365,596.9 ns |  3,159.05 ns |  1,652.25 ns |  0.06 |    0.00 |    3 |         - |          NA |
| IntroSort                    | 8192 | PipeOrgan          |   194,015.9 ns | 14,510.66 ns |  7,589.35 ns |  0.03 |    0.00 |    3 |         - |          NA |
| IntroSortDotnet              | 8192 | PipeOrgan          |   343,094.2 ns |  5,774.84 ns |  3,020.35 ns |  0.06 |    0.00 |    3 |         - |          NA |
| PDQSort                      | 8192 | PipeOrgan          |    91,372.0 ns |  2,665.81 ns |  1,183.64 ns |  0.01 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 8192 | PipeOrgan          |   162,110.8 ns |  1,304.40 ns |    579.16 ns |  0.03 |    0.00 |    3 |         - |          NA |
| Ipnsort                      | 8192 | PipeOrgan          |   236,839.9 ns |  1,172.14 ns |    613.05 ns |  0.04 |    0.00 |    3 |         - |          NA |
| StdSort                      | 8192 | PipeOrgan          |   286,358.8 ns |  9,022.77 ns |  4,719.09 ns |  0.05 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 8192 | PipeOrgan          |   224,972.8 ns |  1,856.81 ns |    971.15 ns |  0.04 |    0.00 |    3 |         - |          NA |
| DotnetSort                   | 8192 | PipeOrgan          |   356,679.1 ns | 13,017.82 ns |  5,779.99 ns |  0.06 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **ManyDuplicates**     |    **96,549.2 ns** |  **2,305.71 ns** |  **1,023.75 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 8192 | ManyDuplicates     |    69,434.1 ns | 10,618.64 ns |  5,553.75 ns |  0.72 |    0.05 |    2 |         - |          NA |
| QuickSortMedian3             | 8192 | ManyDuplicates     |   116,698.2 ns |  6,360.44 ns |  3,326.63 ns |  1.21 |    0.03 |    2 |         - |          NA |
| QuickSortMedian9             | 8192 | ManyDuplicates     |   124,581.9 ns |  6,304.29 ns |  3,297.26 ns |  1.29 |    0.03 |    2 |         - |          NA |
| DualPivotQuickSort           | 8192 | ManyDuplicates     |    61,662.3 ns |  2,847.30 ns |  1,489.19 ns |  0.64 |    0.02 |    2 |         - |          NA |
| StableQuickSort              | 8192 | ManyDuplicates     |   464,721.3 ns |  1,675.12 ns |    876.12 ns |  4.81 |    0.05 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | ManyDuplicates     |   243,620.2 ns |  6,417.81 ns |  2,849.55 ns |  2.52 |    0.04 |    3 |         - |          NA |
| DestswapStableQuickSort      | 8192 | ManyDuplicates     |   113,615.1 ns |  1,129.86 ns |    501.67 ns |  1.18 |    0.01 |    2 |         - |          NA |
| IntroSort                    | 8192 | ManyDuplicates     |   114,216.1 ns |  4,765.48 ns |  2,492.44 ns |  1.18 |    0.03 |    2 |         - |          NA |
| IntroSortDotnet              | 8192 | ManyDuplicates     |    83,621.1 ns |  1,183.18 ns |    525.34 ns |  0.87 |    0.01 |    2 |         - |          NA |
| PDQSort                      | 8192 | ManyDuplicates     |    44,067.5 ns |    871.77 ns |    387.07 ns |  0.46 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 8192 | ManyDuplicates     |    59,830.7 ns |    953.06 ns |    498.47 ns |  0.62 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 8192 | ManyDuplicates     |   118,281.7 ns |  2,399.54 ns |  1,065.41 ns |  1.23 |    0.02 |    2 |         - |          NA |
| StdSort                      | 8192 | ManyDuplicates     |    62,934.1 ns |    882.38 ns |    461.50 ns |  0.65 |    0.01 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | ManyDuplicates     |   102,342.5 ns |    738.59 ns |    327.94 ns |  1.06 |    0.01 |    2 |         - |          NA |
| DotnetSort                   | 8192 | ManyDuplicates     |    79,084.2 ns |  1,743.23 ns |    774.01 ns |  0.82 |    0.01 |    2 |         - |          NA |

### RadixHistogramPrecomputeBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method              | Size | RadixDigits | Mean         | Error        | StdDev       | Ratio | RatioSD | Allocated | Alloc Ratio |
| -------------------- |----- |------------ |-------------:|-------------:|-------------:|------:|--------:|----------:|------------:|
| **Lsd256_CountPerPass** | **1024** | **1**           |   **3,859.5 ns** |     **23.79 ns** |      **8.48 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Lsd256_Histogram    | 1024 | 1           |   4,893.8 ns |    422.69 ns |    187.68 ns |  1.27 |    0.05 |         - |          NA |
| Lsd10_CountPerPass  | 1024 | 1           |  16,877.7 ns |    112.58 ns |     49.99 ns |  4.37 |    0.02 |         - |          NA |
| Lsd10_Histogram     | 1024 | 1           |  16,820.1 ns |    135.03 ns |     59.95 ns |  4.36 |    0.02 |         - |          NA |
| Lsd10_Quotient      | 1024 | 1           |  38,843.6 ns |    771.97 ns |    403.75 ns | 10.06 |    0.10 |         - |          NA |
|      |             |              |              |              |       |         |           |             |
| **Lsd256_CountPerPass** | **1024** | **2**           |   **6,290.4 ns** |     **35.85 ns** |     **12.78 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Lsd256_Histogram    | 1024 | 2           |   7,169.5 ns |    258.87 ns |    135.39 ns |  1.14 |    0.02 |         - |          NA |
| Lsd10_CountPerPass  | 1024 | 2           |  27,072.6 ns |    223.36 ns |    116.82 ns |  4.30 |    0.02 |         - |          NA |
| Lsd10_Histogram     | 1024 | 2           |  26,835.7 ns |    192.77 ns |     85.59 ns |  4.27 |    0.02 |         - |          NA |
| Lsd10_Quotient      | 1024 | 2           |  29,391.7 ns |  1,085.64 ns |    567.81 ns |  4.67 |    0.09 |         - |          NA |
|      |             |              |              |              |       |         |           |             |
| **Lsd256_CountPerPass** | **1024** | **3**           |   **9,156.7 ns** |    **333.54 ns** |    **174.45 ns** |  **1.00** |    **0.03** |         **-** |          **NA** |
| Lsd256_Histogram    | 1024 | 3           |   9,123.2 ns |    305.20 ns |    159.62 ns |  1.00 |    0.02 |         - |          NA |
| Lsd10_CountPerPass  | 1024 | 3           |  42,413.5 ns |    489.77 ns |    174.66 ns |  4.63 |    0.09 |         - |          NA |
| Lsd10_Histogram     | 1024 | 3           |  41,470.1 ns |    221.20 ns |     78.88 ns |  4.53 |    0.08 |         - |          NA |
| Lsd10_Quotient      | 1024 | 3           |  51,493.8 ns |  1,439.54 ns |    752.91 ns |  5.63 |    0.13 |         - |          NA |
|      |             |              |              |              |       |         |           |             |
| **Lsd256_CountPerPass** | **1024** | **4**           |  **11,916.1 ns** |    **262.47 ns** |    **137.28 ns** |  **1.00** |    **0.02** |         **-** |          **NA** |
| Lsd256_Histogram    | 1024 | 4           |  11,594.6 ns |  1,474.04 ns |    770.95 ns |  0.97 |    0.06 |         - |          NA |
| Lsd10_CountPerPass  | 1024 | 4           |  52,849.7 ns |    361.49 ns |    189.06 ns |  4.44 |    0.05 |         - |          NA |
| Lsd10_Histogram     | 1024 | 4           |  54,056.7 ns |    419.54 ns |    219.43 ns |  4.54 |    0.05 |         - |          NA |
| Lsd10_Quotient      | 1024 | 4           |  61,125.6 ns |  2,431.89 ns |  1,271.93 ns |  5.13 |    0.12 |         - |          NA |
|      |             |              |              |              |       |         |           |             |
| **Lsd256_CountPerPass** | **8192** | **1**           |  **29,292.4 ns** |    **299.58 ns** |    **106.83 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Lsd256_Histogram    | 8192 | 1           |  37,065.8 ns |  1,253.64 ns |    655.68 ns |  1.27 |    0.02 |         - |          NA |
| Lsd10_CountPerPass  | 8192 | 1           | 133,676.7 ns |    828.55 ns |    433.35 ns |  4.56 |    0.02 |         - |          NA |
| Lsd10_Histogram     | 8192 | 1           | 130,722.7 ns |  1,903.81 ns |    845.30 ns |  4.46 |    0.03 |         - |          NA |
| Lsd10_Quotient      | 8192 | 1           | 234,418.1 ns | 28,314.39 ns | 14,808.98 ns |  8.00 |    0.48 |         - |          NA |
|      |             |              |              |              |       |         |           |             |
| **Lsd256_CountPerPass** | **8192** | **2**           |  **49,950.6 ns** |  **1,090.42 ns** |    **484.15 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| Lsd256_Histogram    | 8192 | 2           |  51,474.3 ns |  1,034.24 ns |    459.21 ns |  1.03 |    0.01 |         - |          NA |
| Lsd10_CountPerPass  | 8192 | 2           | 217,129.5 ns |    418.40 ns |    218.83 ns |  4.35 |    0.04 |         - |          NA |
| Lsd10_Histogram     | 8192 | 2           | 212,744.6 ns |  2,138.01 ns |  1,118.22 ns |  4.26 |    0.04 |         - |          NA |
| Lsd10_Quotient      | 8192 | 2           | 417,086.1 ns | 21,757.24 ns | 11,379.46 ns |  8.35 |    0.23 |         - |          NA |
|      |             |              |              |              |       |         |           |             |
| **Lsd256_CountPerPass** | **8192** | **3**           |  **69,154.0 ns** |    **842.12 ns** |    **373.91 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| Lsd256_Histogram    | 8192 | 3           |  69,658.4 ns |    712.25 ns |    316.24 ns |  1.01 |    0.01 |         - |          NA |
| Lsd10_CountPerPass  | 8192 | 3           | 338,425.2 ns |    622.87 ns |    276.56 ns |  4.89 |    0.03 |         - |          NA |
| Lsd10_Histogram     | 8192 | 3           | 328,905.0 ns |  2,503.55 ns |  1,309.40 ns |  4.76 |    0.03 |         - |          NA |
| Lsd10_Quotient      | 8192 | 3           | 455,913.2 ns | 16,926.90 ns |  8,853.09 ns |  6.59 |    0.13 |         - |          NA |
|      |             |              |              |              |       |         |           |             |
| **Lsd256_CountPerPass** | **8192** | **4**           |  **88,878.7 ns** |    **384.08 ns** |    **200.88 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Lsd256_Histogram    | 8192 | 4           |  82,337.5 ns |    669.83 ns |    350.33 ns |  0.93 |    0.00 |         - |          NA |
| Lsd10_CountPerPass  | 8192 | 4           | 423,108.8 ns |    969.96 ns |    430.67 ns |  4.76 |    0.01 |         - |          NA |
| Lsd10_Histogram     | 8192 | 4           | 420,475.3 ns |  1,220.51 ns |    638.35 ns |  4.73 |    0.01 |         - |          NA |
| Lsd10_Quotient      | 8192 | 4           | 846,951.4 ns | 22,692.17 ns | 11,868.44 ns |  9.53 |    0.13 |         - |          NA |

### RadixIdentitySkipBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method        | Size | Stride | Mean         | Error        | StdDev       | Ratio | RatioSD | Allocated | Alloc Ratio |
| -------------- |----- |------- |-------------:|-------------:|-------------:|------:|--------:|----------:|------------:|
| **Lsd4_NoSkip**   | **1024** | **1**      |  **17,611.9 ns** |    **224.48 ns** |     **99.67 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| Lsd4_Skip     | 1024 | 1      |  13,821.6 ns |    234.87 ns |    122.84 ns |  0.78 |    0.01 |         - |          NA |
| Lsd256_NoSkip | 1024 | 1      |   7,057.9 ns |    307.88 ns |    161.03 ns |  0.40 |    0.01 |         - |          NA |
| Lsd256_Skip   | 1024 | 1      |   6,814.2 ns |    119.44 ns |     53.03 ns |  0.39 |    0.00 |         - |          NA |
| Lsd10_NoSkip  | 1024 | 1      |  21,287.1 ns |     73.75 ns |     32.75 ns |  1.21 |    0.01 |         - |          NA |
| Lsd10_Skip    | 1024 | 1      |  21,015.0 ns |    365.75 ns |    191.29 ns |  1.19 |    0.01 |         - |          NA |
|      |        |              |              |              |       |         |           |             |
| **Lsd4_NoSkip**   | **1024** | **65536**  |  **48,337.3 ns** |    **197.93 ns** |     **87.88 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Lsd4_Skip     | 1024 | 65536  |  23,107.9 ns |    125.75 ns |     55.83 ns |  0.48 |    0.00 |         - |          NA |
| Lsd256_NoSkip | 1024 | 65536  |  11,980.2 ns |    283.19 ns |    125.74 ns |  0.25 |    0.00 |         - |          NA |
| Lsd256_Skip   | 1024 | 65536  |   9,198.0 ns |    325.74 ns |    170.37 ns |  0.19 |    0.00 |         - |          NA |
| Lsd10_NoSkip  | 1024 | 65536  |  41,969.8 ns |    178.34 ns |     93.28 ns |  0.87 |    0.00 |         - |          NA |
| Lsd10_Skip    | 1024 | 65536  |  41,268.2 ns |    150.18 ns |     66.68 ns |  0.85 |    0.00 |         - |          NA |
|      |        |              |              |              |       |         |           |             |
| **Lsd4_NoSkip**   | **8192** | **1**      | **195,061.2 ns** |    **705.77 ns** |    **313.37 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Lsd4_Skip     | 8192 | 1      | 153,369.2 ns |    510.62 ns |    226.72 ns |  0.79 |    0.00 |         - |          NA |
| Lsd256_NoSkip | 8192 | 1      |  51,630.9 ns |    723.23 ns |    378.26 ns |  0.26 |    0.00 |         - |          NA |
| Lsd256_Skip   | 8192 | 1      |  51,098.8 ns |    120.60 ns |     53.55 ns |  0.26 |    0.00 |         - |          NA |
| Lsd10_NoSkip  | 8192 | 1      | 168,960.4 ns |  1,583.21 ns |    828.05 ns |  0.87 |    0.00 |         - |          NA |
| Lsd10_Skip    | 8192 | 1      | 170,370.6 ns |  1,571.86 ns |    822.11 ns |  0.87 |    0.00 |         - |          NA |
|      |        |              |              |              |       |         |           |             |
| **Lsd4_NoSkip**   | **8192** | **65536**  | **395,325.5 ns** |    **923.38 ns** |    **409.99 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Lsd4_Skip     | 8192 | 65536  | 223,885.7 ns |    711.05 ns |    371.89 ns |  0.57 |    0.00 |         - |          NA |
| Lsd256_NoSkip | 8192 | 65536  |  87,528.4 ns |    845.73 ns |    442.33 ns |  0.22 |    0.00 |         - |          NA |
| Lsd256_Skip   | 8192 | 65536  |  67,554.0 ns |  1,553.89 ns |    689.94 ns |  0.17 |    0.00 |         - |          NA |
| Lsd10_NoSkip  | 8192 | 65536  | 406,884.3 ns | 67,921.37 ns | 35,524.19 ns |  1.03 |    0.08 |         - |          NA |
| Lsd10_Skip    | 8192 | 65536  | 376,113.7 ns |  1,007.81 ns |    447.48 ns |  0.95 |    0.00 |         - |          NA |

### RadixLSD4KeyCacheBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method         | Size  | FullRange | Mean           | Error      | StdDev     | Ratio | RatioSD | Allocated | Alloc Ratio |
| --------------- |------ |---------- |---------------:|-----------:|-----------:|------:|--------:|----------:|------------:|
| **Lsd4_Recompute** | **1024**  | **False**     |    **14,110.8 ns** |   **444.3 ns** |   **197.3 ns** |  **1.00** |    **0.02** |         **-** |          **NA** |
| Lsd4_KeyCache  | 1024  | False     |    19,094.0 ns |   221.0 ns |   115.6 ns |  1.35 |    0.02 |         - |          NA |
|       |           |                |            |            |       |         |           |             |
| **Lsd4_Recompute** | **1024**  | **True**      |    **41,545.1 ns** |   **340.0 ns** |   **150.9 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Lsd4_KeyCache  | 1024  | True      |    57,089.6 ns |   312.7 ns |   163.5 ns |  1.37 |    0.01 |         - |          NA |
|       |           |                |            |            |       |         |           |             |
| **Lsd4_Recompute** | **8192**  | **False**     |   **152,070.0 ns** |   **654.4 ns** |   **290.6 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Lsd4_KeyCache  | 8192  | False     |   200,899.4 ns |   812.4 ns |   360.7 ns |  1.32 |    0.00 |         - |          NA |
|       |           |                |            |            |       |         |           |             |
| **Lsd4_Recompute** | **8192**  | **True**      |   **332,035.4 ns** |   **295.5 ns** |   **105.4 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Lsd4_KeyCache  | 8192  | True      |   426,145.9 ns | 1,030.1 ns |   538.7 ns |  1.28 |    0.00 |         - |          NA |
|       |           |                |            |            |       |         |           |             |
| **Lsd4_Recompute** | **65536** | **False**     | **1,367,406.3 ns** | **3,630.7 ns** | **1,612.1 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Lsd4_KeyCache  | 65536 | False     | 1,822,157.8 ns | 9,564.8 ns | 4,246.8 ns |  1.33 |    0.00 |         - |          NA |
|       |           |                |            |            |       |         |           |             |
| **Lsd4_Recompute** | **65536** | **True**      | **2,656,022.1 ns** | **2,329.3 ns** | **1,034.2 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Lsd4_KeyCache  | 65536 | True      | 3,495,569.7 ns | 9,135.3 ns | 4,777.9 ns |  1.32 |    0.00 |         - |          NA |

### RadixRangeNormalizationBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method            | Size | StraddlesZero | Mean         | Error       | StdDev      | Ratio | Allocated | Alloc Ratio |
| ------------------ |----- |-------------- |-------------:|------------:|------------:|------:|----------:|------------:|
| **Lsd4_Xor**          | **1024** | **False**         |  **20,719.7 ns** |   **210.10 ns** |   **109.89 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_Normalized   | 1024 | False         |  15,186.8 ns |   243.84 ns |   108.27 ns |  0.73 |         - |          NA |
| Lsd256_Xor        | 1024 | False         |   6,259.6 ns |   230.12 ns |   120.36 ns |  0.30 |         - |          NA |
| Lsd256_Normalized | 1024 | False         |   6,675.2 ns |   363.55 ns |   190.14 ns |  0.32 |         - |          NA |
| Lsd10_CopyBack    | 1024 | False         |  23,818.1 ns |   160.42 ns |    83.90 ns |  1.15 |         - |          NA |
| Lsd10_PingPong    | 1024 | False         |  21,371.3 ns |   211.86 ns |   110.81 ns |  1.03 |         - |          NA |
|      |               |              |             |             |       |           |             |
| **Lsd4_Xor**          | **1024** | **True**          |  **53,756.9 ns** |   **195.17 ns** |   **102.08 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_Normalized   | 1024 | True          |  14,178.3 ns |   270.22 ns |   141.33 ns |  0.26 |         - |          NA |
| Lsd256_Xor        | 1024 | True          |  11,742.4 ns |   286.50 ns |   149.84 ns |  0.22 |         - |          NA |
| Lsd256_Normalized | 1024 | True          |   6,519.3 ns |   377.46 ns |   197.42 ns |  0.12 |         - |          NA |
| Lsd10_CopyBack    | 1024 | True          |  23,976.4 ns |    48.47 ns |    25.35 ns |  0.45 |         - |          NA |
| Lsd10_PingPong    | 1024 | True          |  21,349.6 ns |   237.78 ns |   124.36 ns |  0.40 |         - |          NA |
|      |               |              |             |             |       |           |             |
| **Lsd4_Xor**          | **8192** | **False**         | **198,132.6 ns** | **1,540.04 ns** |   **683.79 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_Normalized   | 8192 | False         | 152,124.9 ns |   642.09 ns |   285.09 ns |  0.77 |         - |          NA |
| Lsd256_Xor        | 8192 | False         |  46,552.4 ns |   624.51 ns |   326.63 ns |  0.23 |         - |          NA |
| Lsd256_Normalized | 8192 | False         |  48,312.0 ns |   898.79 ns |   470.08 ns |  0.24 |         - |          NA |
| Lsd10_CopyBack    | 8192 | False         | 179,480.6 ns | 1,057.36 ns |   553.02 ns |  0.91 |         - |          NA |
| Lsd10_PingPong    | 8192 | False         | 167,603.7 ns | 2,359.26 ns | 1,233.94 ns |  0.85 |         - |          NA |
|      |               |              |             |             |       |           |             |
| **Lsd4_Xor**          | **8192** | **True**          | **414,006.1 ns** | **3,383.08 ns** | **1,769.42 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_Normalized   | 8192 | True          | 154,650.2 ns |   986.14 ns |   515.77 ns |  0.37 |         - |          NA |
| Lsd256_Xor        | 8192 | True          |  80,335.7 ns |   142.85 ns |    63.43 ns |  0.19 |         - |          NA |
| Lsd256_Normalized | 8192 | True          |  48,374.1 ns |   310.70 ns |   162.50 ns |  0.12 |         - |          NA |
| Lsd10_CopyBack    | 8192 | True          | 177,630.2 ns | 1,151.77 ns |   602.40 ns |  0.43 |         - |          NA |
| Lsd10_PingPong    | 8192 | True          | 166,330.6 ns | 1,854.13 ns |   969.75 ns |  0.40 |         - |          NA |

### SelectionBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method              | Size | Pattern            | Mean           | Error        | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------------- |----- |------------------- |---------------:|-------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **SelectionSort**       | **256**  | **Random**             |    **24,728.9 ns** |    **346.82 ns** |   **181.40 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Random             |    18,652.7 ns |    225.94 ns |   118.17 ns |  0.75 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | Random             |    72,762.5 ns |  1,761.74 ns |   782.23 ns |  2.94 |    0.04 |    4 |         - |          NA |
| PancakeSort         | 256  | Random             |    40,884.9 ns |    278.94 ns |   145.89 ns |  1.65 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **SelectionSort**       | **256**  | **SingleElementMoved** |    **24,823.7 ns** |    **559.25 ns** |   **292.50 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | SingleElementMoved |    23,022.9 ns |    287.97 ns |   150.61 ns |  0.93 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | SingleElementMoved |    56,614.6 ns |  1,634.10 ns |   854.66 ns |  2.28 |    0.04 |    2 |         - |          NA |
| PancakeSort         | 256  | SingleElementMoved |    20,501.3 ns |    374.37 ns |   166.22 ns |  0.83 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **SelectionSort**       | **256**  | **Sorted**             |    **24,493.8 ns** |    **100.91 ns** |    **52.78 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Sorted             |    12,492.8 ns |    485.35 ns |   215.50 ns |  0.51 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | Sorted             |    24,426.2 ns |    220.55 ns |    97.92 ns |  1.00 |    0.00 |    3 |         - |          NA |
| PancakeSort         | 256  | Sorted             |    16,879.3 ns |    171.13 ns |    75.98 ns |  0.69 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **SelectionSort**       | **256**  | **Reversed**           |    **22,777.4 ns** |  **2,605.86 ns** | **1,362.92 ns** |  **1.00** |    **0.08** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Reversed           |    18,192.9 ns |     31.58 ns |    14.02 ns |  0.80 |    0.04 |    1 |         - |          NA |
| CycleSort           | 256  | Reversed           |    49,555.0 ns |    174.45 ns |    77.46 ns |  2.18 |    0.12 |    3 |         - |          NA |
| PancakeSort         | 256  | Reversed           |    17,028.3 ns |    253.48 ns |   132.57 ns |  0.75 |    0.04 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **SelectionSort**       | **256**  | **PipeOrgan**          |    **23,892.5 ns** |    **666.22 ns** |   **348.44 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | PipeOrgan          |    21,363.1 ns |    179.12 ns |    93.68 ns |  0.89 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | PipeOrgan          |    70,705.0 ns |  1,636.64 ns |   856.00 ns |  2.96 |    0.05 |    3 |         - |          NA |
| PancakeSort         | 256  | PipeOrgan          |    37,115.8 ns |    279.80 ns |   146.34 ns |  1.55 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **SelectionSort**       | **256**  | **ManyDuplicates**     |    **24,506.1 ns** |    **309.87 ns** |   **162.07 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | ManyDuplicates     |    18,445.4 ns |    285.58 ns |   126.80 ns |  0.75 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | ManyDuplicates     |    69,422.1 ns |  1,402.02 ns |   622.50 ns |  2.83 |    0.03 |    4 |         - |          NA |
| PancakeSort         | 256  | ManyDuplicates     |    38,530.3 ns |    367.70 ns |   192.31 ns |  1.57 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Random**             |   **406,191.7 ns** |  **1,961.32 ns** | **1,025.81 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Random             |   301,562.8 ns |    988.90 ns |   439.08 ns |  0.74 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | Random             | 1,552,063.4 ns |  6,727.11 ns | 3,518.41 ns |  3.82 |    0.01 |    4 |         - |          NA |
| PancakeSort         | 1024 | Random             |   691,336.8 ns |  3,643.48 ns | 1,905.61 ns |  1.70 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **SelectionSort**       | **1024** | **SingleElementMoved** |   **375,846.2 ns** |  **1,066.07 ns** |   **557.58 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | SingleElementMoved |   354,167.2 ns |  1,103.98 ns |   577.40 ns |  0.94 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | SingleElementMoved |   891,530.3 ns | 10,570.31 ns | 5,528.48 ns |  2.37 |    0.01 |    2 |         - |          NA |
| PancakeSort         | 1024 | SingleElementMoved |   350,080.8 ns | 12,382.27 ns | 6,476.17 ns |  0.93 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Sorted**             |   **375,509.4 ns** |    **704.24 ns** |   **312.69 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Sorted             |   188,693.7 ns |    797.93 ns |   417.33 ns |  0.50 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | Sorted             |   375,767.3 ns |  2,165.65 ns |   961.56 ns |  1.00 |    0.00 |    3 |         - |          NA |
| PancakeSort         | 1024 | Sorted             |   253,420.9 ns |  1,057.11 ns |   552.89 ns |  0.67 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Reversed**           |   **343,946.3 ns** | **11,511.47 ns** | **6,020.72 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Reversed           |   280,974.5 ns |    688.91 ns |   305.88 ns |  0.82 |    0.01 |    1 |         - |          NA |
| CycleSort           | 1024 | Reversed           |   757,786.6 ns |  1,609.08 ns |   714.44 ns |  2.20 |    0.04 |    3 |         - |          NA |
| PancakeSort         | 1024 | Reversed           |   262,861.1 ns | 13,789.56 ns | 6,122.65 ns |  0.76 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **SelectionSort**       | **1024** | **PipeOrgan**          |   **388,726.1 ns** |  **3,694.38 ns** | **1,932.23 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | PipeOrgan          |   345,633.8 ns |  1,162.43 ns |   607.97 ns |  0.89 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | PipeOrgan          | 1,187,622.2 ns |  9,583.42 ns | 5,012.31 ns |  3.06 |    0.02 |    3 |         - |          NA |
| PancakeSort         | 1024 | PipeOrgan          |   566,664.2 ns |  1,312.33 ns |   582.68 ns |  1.46 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **SelectionSort**       | **1024** | **ManyDuplicates**     |   **394,612.1 ns** |  **1,116.57 ns** |   **495.77 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | ManyDuplicates     |   294,997.0 ns |    697.36 ns |   364.73 ns |  0.75 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | ManyDuplicates     | 1,529,739.4 ns |  6,844.09 ns | 3,579.59 ns |  3.88 |    0.01 |    4 |         - |          NA |
| PancakeSort         | 1024 | ManyDuplicates     |   633,740.4 ns |  1,590.53 ns |   831.88 ns |  1.61 |    0.00 |    3 |         - |          NA |

### TreeBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                 | Size | Pattern            | Mean           | Error       | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------- |----- |------------------- |---------------:|------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **BalancedBinaryTreeSort** | **256**  | **Random**             |    **13,358.4 ns** |   **715.61 ns** |   **317.73 ns** |  **3.84** |    **0.19** |    **4** |         **-** |          **NA** |
| CartesianTreeSort      | 256  | Random             |     6,515.0 ns |   280.08 ns |   124.36 ns |  1.87 |    0.09 |    2 |         - |          NA |
| BinaryTreeSort         | 256  | Random             |     3,488.2 ns |   317.28 ns |   165.94 ns |  1.00 |    0.06 |    1 |         - |          NA |
| SplaySort              | 256  | Random             |    23,094.3 ns |   362.92 ns |   189.81 ns |  6.63 |    0.29 |    5 |         - |          NA |
| TreapSort              | 256  | Random             |     8,937.5 ns |   488.53 ns |   255.51 ns |  2.57 |    0.13 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **SingleElementMoved** |    **14,291.0 ns** |   **634.50 ns** |   **331.86 ns** |  **0.29** |    **0.01** |    **4** |         **-** |          **NA** |
| CartesianTreeSort      | 256  | SingleElementMoved |     2,340.3 ns |    25.24 ns |     9.00 ns |  0.05 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | SingleElementMoved |    48,992.8 ns |   261.86 ns |   116.27 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 256  | SingleElementMoved |     4,829.1 ns |   338.57 ns |   177.08 ns |  0.10 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | SingleElementMoved |     5,953.5 ns |   216.95 ns |   113.47 ns |  0.12 |    0.00 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Sorted**             |    **13,338.8 ns** |   **568.57 ns** |   **297.37 ns** |  **0.18** |    **0.00** |    **4** |         **-** |          **NA** |
| CartesianTreeSort      | 256  | Sorted             |     2,084.9 ns |    10.68 ns |     4.74 ns |  0.03 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | Sorted             |    75,995.2 ns |   367.49 ns |   163.17 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 256  | Sorted             |     4,057.8 ns |   329.56 ns |   172.37 ns |  0.05 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | Sorted             |     5,160.9 ns |   350.45 ns |   155.60 ns |  0.07 |    0.00 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Reversed**           |    **12,211.0 ns** |   **248.28 ns** |   **129.85 ns** |  **0.15** |    **0.00** |    **4** |         **-** |          **NA** |
| CartesianTreeSort      | 256  | Reversed           |     1,983.3 ns |     6.70 ns |     2.39 ns |  0.02 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | Reversed           |    79,712.8 ns |   281.40 ns |   147.18 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 256  | Reversed           |     3,686.3 ns |     8.89 ns |     3.17 ns |  0.05 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | Reversed           |     5,245.3 ns |   148.23 ns |    77.53 ns |  0.07 |    0.00 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **PipeOrgan**          |    **12,396.7 ns** |   **485.79 ns** |   **254.08 ns** |  **0.33** |    **0.01** |    **4** |         **-** |          **NA** |
| CartesianTreeSort      | 256  | PipeOrgan          |     2,217.2 ns |    10.49 ns |     4.66 ns |  0.06 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | PipeOrgan          |    37,619.1 ns |   486.63 ns |   254.52 ns |  1.00 |    0.01 |    5 |         - |          NA |
| SplaySort              | 256  | PipeOrgan          |     4,618.8 ns |   328.70 ns |   171.92 ns |  0.12 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | PipeOrgan          |     7,501.0 ns |   142.53 ns |    63.28 ns |  0.20 |    0.00 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **ManyDuplicates**     |    **13,862.8 ns** |   **909.73 ns** |   **403.93 ns** |  **3.33** |    **0.14** |    **3** |         **-** |          **NA** |
| CartesianTreeSort      | 256  | ManyDuplicates     |     7,451.5 ns |   353.52 ns |   184.90 ns |  1.79 |    0.07 |    2 |         - |          NA |
| BinaryTreeSort         | 256  | ManyDuplicates     |     4,165.8 ns |   271.72 ns |   142.11 ns |  1.00 |    0.05 |    1 |         - |          NA |
| SplaySort              | 256  | ManyDuplicates     |    21,780.6 ns |   657.97 ns |   292.14 ns |  5.23 |    0.18 |    4 |         - |          NA |
| TreapSort              | 256  | ManyDuplicates     |     8,479.2 ns |   561.50 ns |   293.67 ns |  2.04 |    0.09 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Random**             |    **80,057.6 ns** | **8,176.17 ns** | **4,276.29 ns** |  **4.11** |    **0.23** |    **4** |         **-** |          **NA** |
| CartesianTreeSort      | 1024 | Random             |    32,132.6 ns |   595.10 ns |   212.22 ns |  1.65 |    0.05 |    2 |         - |          NA |
| BinaryTreeSort         | 1024 | Random             |    19,502.6 ns | 1,073.78 ns |   561.61 ns |  1.00 |    0.04 |    1 |         - |          NA |
| SplaySort              | 1024 | Random             |   125,948.9 ns | 3,491.39 ns | 1,826.06 ns |  6.46 |    0.19 |    5 |         - |          NA |
| TreapSort              | 1024 | Random             |    39,478.9 ns | 2,625.41 ns | 1,373.14 ns |  2.03 |    0.09 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **SingleElementMoved** |    **78,091.1 ns** | **7,738.16 ns** | **4,047.21 ns** |  **0.10** |    **0.00** |    **4** |         **-** |          **NA** |
| CartesianTreeSort      | 1024 | SingleElementMoved |     8,844.0 ns |   203.56 ns |    90.38 ns |  0.01 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | SingleElementMoved |   778,008.0 ns | 4,428.06 ns | 2,315.96 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | SingleElementMoved |    17,534.2 ns |   306.68 ns |   109.37 ns |  0.02 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | SingleElementMoved |    26,954.0 ns |   454.54 ns |   237.73 ns |  0.03 |    0.00 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Sorted**             |    **68,983.0 ns** | **3,691.82 ns** | **1,639.19 ns** | **0.057** |    **0.00** |    **4** |         **-** |          **NA** |
| CartesianTreeSort      | 1024 | Sorted             |     8,018.5 ns |    38.97 ns |    17.30 ns | 0.007 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | Sorted             | 1,205,360.7 ns |   718.10 ns |   318.84 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | Sorted             |    15,434.2 ns |   115.64 ns |    60.48 ns | 0.013 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | Sorted             |    22,939.8 ns |   488.21 ns |   255.34 ns | 0.019 |    0.00 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Reversed**           |    **59,931.5 ns** |   **578.67 ns** |   **302.65 ns** | **0.047** |    **0.00** |    **4** |         **-** |          **NA** |
| CartesianTreeSort      | 1024 | Reversed           |     7,705.4 ns |   176.20 ns |    92.16 ns | 0.006 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | Reversed           | 1,278,066.0 ns | 1,271.02 ns |   453.26 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | Reversed           |    14,688.3 ns |   226.96 ns |   118.71 ns | 0.011 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | Reversed           |    23,420.1 ns |   834.86 ns |   436.65 ns | 0.018 |    0.00 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **PipeOrgan**          |    **66,618.8 ns** | **2,896.32 ns** | **1,514.83 ns** |  **0.11** |    **0.00** |    **4** |         **-** |          **NA** |
| CartesianTreeSort      | 1024 | PipeOrgan          |     8,716.8 ns |    82.35 ns |    36.56 ns |  0.01 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | PipeOrgan          |   600,899.0 ns | 4,660.17 ns | 2,437.36 ns |  1.00 |    0.01 |    5 |         - |          NA |
| SplaySort              | 1024 | PipeOrgan          |    17,437.3 ns |   108.00 ns |    47.95 ns |  0.03 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | PipeOrgan          |    34,871.3 ns |   860.73 ns |   450.18 ns |  0.06 |    0.00 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **ManyDuplicates**     |    **73,457.4 ns** | **3,688.77 ns** | **1,929.30 ns** |  **2.08** |    **0.06** |    **2** |         **-** |          **NA** |
| CartesianTreeSort      | 1024 | ManyDuplicates     |    34,806.0 ns | 1,318.99 ns |   585.64 ns |  0.98 |    0.02 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | ManyDuplicates     |    35,359.7 ns |   782.23 ns |   409.12 ns |  1.00 |    0.02 |    1 |         - |          NA |
| SplaySort              | 1024 | ManyDuplicates     |   106,410.5 ns | 2,523.44 ns |   899.88 ns |  3.01 |    0.04 |    3 |         - |          NA |
| TreapSort              | 1024 | ManyDuplicates     |    38,996.9 ns | 1,622.97 ns |   848.84 ns |  1.10 |    0.03 |    1 |         - |          NA |

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
