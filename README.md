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
<summary>Benchmark results (2026-08-03 04:41 UTC)</summary>

Workflow run: https://github.com/guitarrapc/SortAlgorithms/actions/runs/30784297297

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

| Method        | Size | Pattern            | Mean         | Error        | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------- |----- |------------------- |-------------:|-------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **DropMergeSort** | **256**  | **Random**             |   **3,189.5 ns** |    **189.77 ns** |    **99.25 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | Random             |   8,429.9 ns |    564.11 ns |   295.04 ns |  2.65 |    0.12 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **DropMergeSort** | **256**  | **SingleElementMoved** |     **653.2 ns** |      **6.48 ns** |     **2.88 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | SingleElementMoved |  10,680.6 ns |  5,594.02 ns | 2,925.78 ns | 16.35 |    4.23 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **DropMergeSort** | **256**  | **Sorted**             |     **832.8 ns** |    **474.76 ns** |   **248.31 ns** |  **1.08** |    **0.44** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | Sorted             |   7,563.9 ns |    148.25 ns |    65.83 ns |  9.84 |    2.73 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **DropMergeSort** | **256**  | **Reversed**           |   **1,684.2 ns** |     **60.01 ns** |    **26.65 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | Reversed           |   1,477.0 ns |      7.52 ns |     3.34 ns |  0.88 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **DropMergeSort** | **256**  | **PipeOrgan**          |   **6,428.9 ns** |    **402.66 ns** |   **210.60 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | PipeOrgan          |   5,476.0 ns |    268.67 ns |   119.29 ns |  0.85 |    0.03 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **DropMergeSort** | **256**  | **ManyDuplicates**     |   **2,918.2 ns** |    **366.88 ns** |   **191.88 ns** |  **1.00** |    **0.09** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | ManyDuplicates     |   3,853.0 ns |    318.40 ns |   166.53 ns |  1.33 |    0.09 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **DropMergeSort** | **1024** | **Random**             |  **14,665.4 ns** |  **1,193.66 ns** |   **624.31 ns** |  **1.00** |    **0.06** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | Random             |  24,712.6 ns |  1,061.01 ns |   471.10 ns |  1.69 |    0.07 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **DropMergeSort** | **1024** | **SingleElementMoved** |   **2,681.2 ns** |     **51.98 ns** |    **23.08 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | SingleElementMoved |  40,044.1 ns |  1,567.62 ns |   696.03 ns | 14.94 |    0.27 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **DropMergeSort** | **1024** | **Sorted**             |   **2,214.3 ns** |     **13.88 ns** |     **6.16 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | Sorted             |  39,051.6 ns |    516.54 ns |   229.35 ns | 17.64 |    0.11 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **DropMergeSort** | **1024** | **Reversed**           |   **7,111.3 ns** |    **194.72 ns** |    **86.46 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| PatienceSort  | 1024 | Reversed           |   5,092.8 ns |     23.76 ns |     8.47 ns |  0.72 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **DropMergeSort** | **1024** | **PipeOrgan**          |  **27,070.1 ns** |    **538.25 ns** |   **238.99 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | PipeOrgan          |  27,345.9 ns |    876.12 ns |   389.00 ns |  1.01 |    0.02 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **DropMergeSort** | **1024** | **ManyDuplicates**     |  **12,614.9 ns** |    **417.28 ns** |   **185.27 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | ManyDuplicates     |  15,273.6 ns |    476.35 ns |   249.14 ns |  1.21 |    0.03 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **DropMergeSort** | **4096** | **Random**             |  **73,064.0 ns** |  **5,275.05 ns** | **2,342.15 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| PatienceSort  | 4096 | Random             | 158,205.2 ns | 10,435.30 ns | 4,633.34 ns |  2.17 |    0.09 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **DropMergeSort** | **4096** | **SingleElementMoved** |  **10,060.2 ns** |    **539.14 ns** |   **281.98 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| PatienceSort  | 4096 | SingleElementMoved | 238,036.7 ns | 20,360.66 ns | 9,040.26 ns | 23.68 |    1.04 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **DropMergeSort** | **4096** | **Sorted**             |   **8,808.8 ns** |    **469.65 ns** |   **208.53 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| PatienceSort  | 4096 | Sorted             | 219,416.7 ns | 14,727.81 ns | 7,702.93 ns | 24.92 |    0.99 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **DropMergeSort** | **4096** | **Reversed**           |  **30,996.1 ns** |    **610.58 ns** |   **319.34 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| PatienceSort  | 4096 | Reversed           |  20,070.0 ns |    442.51 ns |   196.48 ns |  0.65 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **DropMergeSort** | **4096** | **PipeOrgan**          | **111,685.8 ns** |    **490.42 ns** |   **256.50 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 4096 | PipeOrgan          | 164,160.6 ns | 16,559.15 ns | 8,660.76 ns |  1.47 |    0.07 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **DropMergeSort** | **4096** | **ManyDuplicates**     |  **57,396.0 ns** |  **3,765.01 ns** | **1,969.17 ns** |  **1.00** |    **0.05** |    **1** |         **-** |          **NA** |
| PatienceSort  | 4096 | ManyDuplicates     |  59,135.8 ns |  4,572.78 ns | 2,391.65 ns |  1.03 |    0.05 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **DropMergeSort** | **8192** | **Random**             | **472,749.1 ns** |  **3,146.52 ns** | **1,397.08 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | Random             | 819,096.5 ns |  5,925.26 ns | 2,630.85 ns |  1.73 |    0.01 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **DropMergeSort** | **8192** | **SingleElementMoved** |  **20,100.5 ns** |    **251.58 ns** |    **89.71 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | SingleElementMoved | 769,411.7 ns |  2,466.60 ns | 1,095.19 ns | 38.28 |    0.17 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **DropMergeSort** | **8192** | **Sorted**             |  **18,132.4 ns** |  **1,642.25 ns** |   **858.93 ns** |  **1.00** |    **0.06** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | Sorted             | 777,713.2 ns |  3,266.17 ns | 1,450.20 ns | 42.97 |    1.87 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **DropMergeSort** | **8192** | **Reversed**           |  **64,911.0 ns** |  **1,183.40 ns** |   **618.94 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| PatienceSort  | 8192 | Reversed           |  40,775.3 ns |  1,429.95 ns |   747.89 ns |  0.63 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **DropMergeSort** | **8192** | **PipeOrgan**          | **227,060.0 ns** |    **857.64 ns** |   **380.80 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | PipeOrgan          | 665,077.2 ns |  4,557.12 ns | 2,383.46 ns |  2.93 |    0.01 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **DropMergeSort** | **8192** | **ManyDuplicates**     | **124,202.1 ns** |  **7,221.03 ns** | **3,776.74 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | ManyDuplicates     | 154,556.6 ns |  2,713.06 ns | 1,204.62 ns |  1.25 |    0.04 |    2 |         - |          NA |

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

| Method     | Size | Pattern            | Mean         | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------- |----- |------------------- |-------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **StrandSort** | **256**  | **Random**             |   **6,217.6 ns** |     **64.31 ns** |     **28.55 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |              |              |       |         |      |           |             |
| **StrandSort** | **256**  | **SingleElementMoved** |     **816.1 ns** |     **59.32 ns** |     **26.34 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
|      |                    |              |              |              |       |         |      |           |             |
| **StrandSort** | **256**  | **Sorted**             |     **571.1 ns** |     **14.95 ns** |      **5.33 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |              |              |       |         |      |           |             |
| **StrandSort** | **256**  | **Reversed**           |  **47,521.1 ns** |    **223.18 ns** |     **99.09 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
|      |                    |              |              |              |       |         |      |           |             |
| **StrandSort** | **256**  | **PipeOrgan**          |  **27,622.4 ns** |  **1,301.88 ns** |    **680.91 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
|      |                    |              |              |              |       |         |      |           |             |
| **StrandSort** | **256**  | **ManyDuplicates**     |   **5,031.3 ns** |    **293.28 ns** |    **153.39 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
|      |                    |              |              |              |       |         |      |           |             |
| **StrandSort** | **1024** | **Random**             |  **56,625.1 ns** |    **334.42 ns** |    **148.49 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
|      |                    |              |              |              |       |         |      |           |             |
| **StrandSort** | **1024** | **SingleElementMoved** |   **2,713.2 ns** |     **30.27 ns** |     **10.80 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |              |              |       |         |      |           |             |
| **StrandSort** | **1024** | **Sorted**             |   **1,766.7 ns** |     **77.44 ns** |     **27.62 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
|      |                    |              |              |              |       |         |      |           |             |
| **StrandSort** | **1024** | **Reversed**           | **785,546.5 ns** | **47,034.39 ns** | **20,883.56 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
|      |                    |              |              |              |       |         |      |           |             |
| **StrandSort** | **1024** | **PipeOrgan**          | **437,355.3 ns** | **13,726.57 ns** |  **7,179.26 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
|      |                    |              |              |              |       |         |      |           |             |
| **StrandSort** | **1024** | **ManyDuplicates**     |  **31,896.0 ns** |    **138.06 ns** |     **72.21 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |

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
| **Radix16_C16**            | **4096**    | **False**        |     **71,595.5 ns** |   **1,135.2 ns** |    **593.7 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| Radix256_Shipped       | 4096    | False        |     73,356.5 ns |   1,887.5 ns |    987.2 ns |  1.02 |    0.02 |         - |          NA |
| Radix256_Cycle         | 4096    | False        |     70,250.7 ns |     732.3 ns |    325.2 ns |  0.98 |    0.01 |         - |          NA |
| Radix256_BinaryLeaf    | 4096    | False        |     69,847.5 ns |     529.5 ns |    235.1 ns |  0.98 |    0.01 |         - |          NA |
| Radix256_PerNodeRescan | 4096    | False        |     79,031.7 ns |     786.3 ns |    349.1 ns |  1.10 |    0.01 |         - |          NA |
|         |              |                 |              |             |       |         |           |             |
| **Radix16_C16**            | **4096**    | **True**         |     **94,769.3 ns** |     **967.7 ns** |    **429.6 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| Radix256_Shipped       | 4096    | True         |     49,562.6 ns |   1,042.1 ns |    462.7 ns |  0.52 |    0.01 |         - |          NA |
| Radix256_Cycle         | 4096    | True         |     47,819.6 ns |     911.4 ns |    476.7 ns |  0.50 |    0.01 |         - |          NA |
| Radix256_BinaryLeaf    | 4096    | True         |     59,586.4 ns |     523.4 ns |    232.4 ns |  0.63 |    0.00 |         - |          NA |
| Radix256_PerNodeRescan | 4096    | True         |     47,848.6 ns |     698.4 ns |    310.1 ns |  0.50 |    0.00 |         - |          NA |
|         |              |                 |              |             |       |         |           |             |
| **Radix16_C16**            | **8192**    | **False**        |    **174,574.8 ns** |   **1,616.1 ns** |    **717.6 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| Radix256_Shipped       | 8192    | False        |    144,143.9 ns |     882.7 ns |    461.6 ns |  0.83 |    0.00 |         - |          NA |
| Radix256_Cycle         | 8192    | False        |    140,196.9 ns |   1,024.3 ns |    454.8 ns |  0.80 |    0.00 |         - |          NA |
| Radix256_BinaryLeaf    | 8192    | False        |    139,422.6 ns |     892.1 ns |    396.1 ns |  0.80 |    0.00 |         - |          NA |
| Radix256_PerNodeRescan | 8192    | False        |    157,704.7 ns |     611.2 ns |    271.4 ns |  0.90 |    0.00 |         - |          NA |
|         |              |                 |              |             |       |         |           |             |
| **Radix16_C16**            | **8192**    | **True**         |    **212,295.2 ns** |   **2,252.2 ns** |  **1,178.0 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| Radix256_Shipped       | 8192    | True         |    117,759.7 ns |   6,448.6 ns |  3,372.8 ns |  0.55 |    0.02 |         - |          NA |
| Radix256_Cycle         | 8192    | True         |    110,505.1 ns |   1,968.4 ns |  1,029.5 ns |  0.52 |    0.01 |         - |          NA |
| Radix256_BinaryLeaf    | 8192    | True         |    151,793.5 ns |  10,307.0 ns |  5,390.7 ns |  0.72 |    0.02 |         - |          NA |
| Radix256_PerNodeRescan | 8192    | True         |    110,313.1 ns |   2,840.4 ns |  1,261.1 ns |  0.52 |    0.01 |         - |          NA |
|         |              |                 |              |             |       |         |           |             |
| **Radix16_C16**            | **65536**   | **False**        |  **2,327,949.9 ns** |   **4,487.6 ns** |  **2,347.1 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Radix256_Shipped       | 65536   | False        |  1,336,810.2 ns |     827.6 ns |    295.1 ns |  0.57 |    0.00 |         - |          NA |
| Radix256_Cycle         | 65536   | False        |  1,316,987.9 ns |   2,140.4 ns |    950.3 ns |  0.57 |    0.00 |         - |          NA |
| Radix256_BinaryLeaf    | 65536   | False        |  1,297,614.4 ns |   3,005.1 ns |  1,571.7 ns |  0.56 |    0.00 |         - |          NA |
| Radix256_PerNodeRescan | 65536   | False        |  1,463,781.4 ns |   4,258.1 ns |  1,890.6 ns |  0.63 |    0.00 |         - |          NA |
|         |              |                 |              |             |       |         |           |             |
| **Radix16_C16**            | **65536**   | **True**         |  **2,697,241.6 ns** |   **6,812.2 ns** |  **3,562.9 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Radix256_Shipped       | 65536   | True         |  1,690,407.4 ns |   3,305.0 ns |  1,728.6 ns |  0.63 |    0.00 |         - |          NA |
| Radix256_Cycle         | 65536   | True         |  1,721,898.4 ns |   2,692.7 ns |  1,195.6 ns |  0.64 |    0.00 |         - |          NA |
| Radix256_BinaryLeaf    | 65536   | True         |  1,873,078.6 ns |   2,609.8 ns |  1,365.0 ns |  0.69 |    0.00 |         - |          NA |
| Radix256_PerNodeRescan | 65536   | True         |  1,825,427.9 ns |   3,758.6 ns |  1,965.8 ns |  0.68 |    0.00 |         - |          NA |
|         |              |                 |              |             |       |         |           |             |
| **Radix16_C16**            | **1048576** | **False**        | **44,661,161.8 ns** |  **17,990.0 ns** |  **9,409.1 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Radix256_Shipped       | 1048576 | False        | 29,178,333.9 ns |  24,917.6 ns |  8,885.9 ns |  0.65 |    0.00 |         - |          NA |
| Radix256_Cycle         | 1048576 | False        | 29,266,543.6 ns |  85,133.1 ns | 44,526.3 ns |  0.66 |    0.00 |         - |          NA |
| Radix256_BinaryLeaf    | 1048576 | False        | 28,695,842.1 ns |  24,669.9 ns | 12,902.8 ns |  0.64 |    0.00 |         - |          NA |
| Radix256_PerNodeRescan | 1048576 | False        | 32,721,994.2 ns |   4,839.9 ns |  2,149.0 ns |  0.73 |    0.00 |         - |          NA |
|         |              |                 |              |             |       |         |           |             |
| **Radix16_C16**            | **1048576** | **True**         | **50,472,967.7 ns** |  **51,501.3 ns** | **22,866.9 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Radix256_Shipped       | 1048576 | True         | 28,222,681.5 ns |  31,971.6 ns | 16,721.8 ns |  0.56 |    0.00 |         - |          NA |
| Radix256_Cycle         | 1048576 | True         | 28,271,316.3 ns |  55,482.4 ns | 24,634.5 ns |  0.56 |    0.00 |         - |          NA |
| Radix256_BinaryLeaf    | 1048576 | True         | 37,865,449.8 ns | 103,244.7 ns | 53,999.0 ns |  0.75 |    0.00 |         - |          NA |
| Radix256_PerNodeRescan | 1048576 | True         | 29,096,592.1 ns |  18,293.5 ns |  9,567.9 ns |  0.58 |    0.00 |         - |          NA |

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
| **CountingSort**        | **256**  | **Random**             |   **1,723.1 ns** |     **11.22 ns** |     **4.98 ns** |  **1.54** |    **0.07** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | Random             |   1,118.5 ns |    118.68 ns |    52.69 ns |  1.00 |    0.07 |    2 |         - |          NA |
| PigeonSort          | 256  | Random             |   1,532.0 ns |      9.68 ns |     3.45 ns |  1.37 |    0.07 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | Random             |     705.0 ns |     34.90 ns |    15.49 ns |  0.63 |    0.03 |    1 |         - |          NA |
| BucketSort          | 256  | Random             |   2,197.4 ns |    311.67 ns |   163.01 ns |  1.97 |    0.17 |    2 |         - |          NA |
| BucketSortInteger   | 256  | Random             |   1,543.7 ns |     10.03 ns |     5.24 ns |  1.38 |    0.07 |    2 |         - |          NA |
| FlashSort           | 256  | Random             |   4,568.1 ns |    235.67 ns |   123.26 ns |  4.09 |    0.22 |    4 |         - |          NA |
| RadixLSD4Sort       | 256  | Random             |   3,008.1 ns |     59.13 ns |    21.09 ns |  2.70 |    0.13 |    3 |         - |          NA |
| RadixLSD256Sort     | 256  | Random             |   1,297.2 ns |      6.58 ns |     2.92 ns |  1.16 |    0.06 |    2 |         - |          NA |
| RadixLSD10Sort      | 256  | Random             |   4,168.5 ns |    309.34 ns |   161.79 ns |  3.73 |    0.23 |    4 |         - |          NA |
| RadixMSD4Sort       | 256  | Random             |   3,044.9 ns |    267.60 ns |   139.96 ns |  2.73 |    0.18 |    3 |         - |          NA |
| RadixMSD10Sort      | 256  | Random             |   4,059.4 ns |     70.25 ns |    31.19 ns |  3.64 |    0.18 |    4 |         - |          NA |
| AmericanFlagSort    | 256  | Random             |   2,818.3 ns |     13.88 ns |     6.16 ns |  2.52 |    0.12 |    3 |         - |          NA |
| SpreadSort          | 256  | Random             |   2,007.9 ns |    231.16 ns |   102.64 ns |  1.80 |    0.12 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **256**  | **SingleElementMoved** |   **1,598.1 ns** |     **10.99 ns** |     **4.88 ns** |  **1.48** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | SingleElementMoved |   1,077.0 ns |     13.34 ns |     6.98 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PigeonSort          | 256  | SingleElementMoved |   1,436.0 ns |     16.45 ns |     8.60 ns |  1.33 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | SingleElementMoved |     860.9 ns |     27.13 ns |    12.05 ns |  0.80 |    0.01 |    1 |         - |          NA |
| BucketSort          | 256  | SingleElementMoved |   1,966.5 ns |     24.53 ns |     8.75 ns |  1.83 |    0.01 |    4 |         - |          NA |
| BucketSortInteger   | 256  | SingleElementMoved |   1,627.5 ns |     27.08 ns |    12.02 ns |  1.51 |    0.01 |    3 |         - |          NA |
| FlashSort           | 256  | SingleElementMoved |   5,465.5 ns |    389.64 ns |   203.79 ns |  5.07 |    0.18 |    6 |         - |          NA |
| RadixLSD4Sort       | 256  | SingleElementMoved |   2,885.5 ns |    282.46 ns |   125.41 ns |  2.68 |    0.11 |    4 |         - |          NA |
| RadixLSD256Sort     | 256  | SingleElementMoved |   1,155.7 ns |     17.75 ns |     6.33 ns |  1.07 |    0.01 |    2 |         - |          NA |
| RadixLSD10Sort      | 256  | SingleElementMoved |   4,092.5 ns |    185.54 ns |    97.04 ns |  3.80 |    0.09 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | SingleElementMoved |   2,709.0 ns |     58.00 ns |    25.75 ns |  2.52 |    0.03 |    4 |         - |          NA |
| RadixMSD10Sort      | 256  | SingleElementMoved |   3,836.0 ns |     48.38 ns |    17.25 ns |  3.56 |    0.03 |    5 |         - |          NA |
| AmericanFlagSort    | 256  | SingleElementMoved |   2,328.7 ns |     15.81 ns |     7.02 ns |  2.16 |    0.01 |    4 |         - |          NA |
| SpreadSort          | 256  | SingleElementMoved |   1,152.4 ns |     29.49 ns |    15.42 ns |  1.07 |    0.01 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **256**  | **Sorted**             |   **1,504.5 ns** |     **10.13 ns** |     **4.50 ns** |  **1.67** |    **0.01** |    **5** |         **-** |          **NA** |
| CountingSortInteger | 256  | Sorted             |     899.7 ns |      6.76 ns |     3.53 ns |  1.00 |    0.01 |    3 |         - |          NA |
| PigeonSort          | 256  | Sorted             |   1,458.1 ns |      9.56 ns |     5.00 ns |  1.62 |    0.01 |    5 |         - |          NA |
| PigeonSortInteger   | 256  | Sorted             |     653.5 ns |     51.63 ns |    27.00 ns |  0.73 |    0.03 |    2 |         - |          NA |
| BucketSort          | 256  | Sorted             |   2,181.2 ns |    297.44 ns |   155.56 ns |  2.42 |    0.16 |    6 |         - |          NA |
| BucketSortInteger   | 256  | Sorted             |   1,500.3 ns |     12.44 ns |     6.50 ns |  1.67 |    0.01 |    5 |         - |          NA |
| FlashSort           | 256  | Sorted             |   5,399.3 ns |    231.02 ns |   102.57 ns |  6.00 |    0.11 |    9 |         - |          NA |
| RadixLSD4Sort       | 256  | Sorted             |   3,079.0 ns |     91.22 ns |    47.71 ns |  3.42 |    0.05 |    7 |         - |          NA |
| RadixLSD256Sort     | 256  | Sorted             |   1,178.7 ns |      8.38 ns |     3.72 ns |  1.31 |    0.01 |    4 |         - |          NA |
| RadixLSD10Sort      | 256  | Sorted             |   4,039.1 ns |     34.24 ns |    12.21 ns |  4.49 |    0.02 |    8 |         - |          NA |
| RadixMSD4Sort       | 256  | Sorted             |   2,620.2 ns |     18.30 ns |     8.12 ns |  2.91 |    0.01 |    7 |         - |          NA |
| RadixMSD10Sort      | 256  | Sorted             |   4,404.1 ns |    239.26 ns |   125.14 ns |  4.90 |    0.13 |    8 |         - |          NA |
| AmericanFlagSort    | 256  | Sorted             |   1,523.9 ns |     15.85 ns |     7.04 ns |  1.69 |    0.01 |    5 |         - |          NA |
| SpreadSort          | 256  | Sorted             |     509.9 ns |    285.69 ns |   149.42 ns |  0.57 |    0.16 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **256**  | **Reversed**           |   **1,499.5 ns** |     **10.02 ns** |     **3.57 ns** |  **1.52** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | Reversed           |     985.5 ns |      6.38 ns |     3.34 ns |  1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 256  | Reversed           |   1,497.8 ns |    283.23 ns |   148.14 ns |  1.52 |    0.14 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | Reversed           |     616.2 ns |      7.68 ns |     4.01 ns |  0.63 |    0.00 |    1 |         - |          NA |
| BucketSort          | 256  | Reversed           |   2,053.6 ns |    217.22 ns |    96.45 ns |  2.08 |    0.09 |    3 |         - |          NA |
| BucketSortInteger   | 256  | Reversed           |   1,573.5 ns |     19.24 ns |     6.86 ns |  1.60 |    0.01 |    3 |         - |          NA |
| FlashSort           | 256  | Reversed           |   4,930.0 ns |    379.62 ns |   198.55 ns |  5.00 |    0.19 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | Reversed           |   2,940.1 ns |    326.43 ns |   144.94 ns |  2.98 |    0.14 |    4 |         - |          NA |
| RadixLSD256Sort     | 256  | Reversed           |   1,154.9 ns |    170.68 ns |    75.78 ns |  1.17 |    0.07 |    2 |         - |          NA |
| RadixLSD10Sort      | 256  | Reversed           |   3,809.3 ns |     40.70 ns |    14.52 ns |  3.87 |    0.02 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | Reversed           |   3,672.3 ns |     29.04 ns |    10.36 ns |  3.73 |    0.02 |    5 |         - |          NA |
| RadixMSD10Sort      | 256  | Reversed           |   4,369.6 ns |     26.99 ns |     9.63 ns |  4.43 |    0.02 |    5 |         - |          NA |
| AmericanFlagSort    | 256  | Reversed           |   1,772.5 ns |      9.76 ns |     4.33 ns |  1.80 |    0.01 |    3 |         - |          NA |
| SpreadSort          | 256  | Reversed           |     524.9 ns |     14.75 ns |     6.55 ns |  0.53 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **256**  | **PipeOrgan**          |   **1,541.6 ns** |     **74.29 ns** |    **26.49 ns** |  **1.44** |    **0.02** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | PipeOrgan          |   1,069.5 ns |      4.06 ns |     1.80 ns |  1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 256  | PipeOrgan          |   1,369.9 ns |      6.84 ns |     2.44 ns |  1.28 |    0.00 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | PipeOrgan          |     710.7 ns |     32.43 ns |    11.56 ns |  0.66 |    0.01 |    1 |         - |          NA |
| BucketSort          | 256  | PipeOrgan          |   2,018.3 ns |     28.00 ns |     9.99 ns |  1.89 |    0.01 |    2 |         - |          NA |
| BucketSortInteger   | 256  | PipeOrgan          |   1,665.4 ns |     12.90 ns |     5.73 ns |  1.56 |    0.01 |    2 |         - |          NA |
| FlashSort           | 256  | PipeOrgan          |   5,215.0 ns |    310.66 ns |   162.48 ns |  4.88 |    0.14 |    4 |         - |          NA |
| RadixLSD4Sort       | 256  | PipeOrgan          |   2,967.4 ns |    303.61 ns |   158.80 ns |  2.77 |    0.14 |    3 |         - |          NA |
| RadixLSD256Sort     | 256  | PipeOrgan          |   1,142.0 ns |      6.07 ns |     2.69 ns |  1.07 |    0.00 |    2 |         - |          NA |
| RadixLSD10Sort      | 256  | PipeOrgan          |   3,933.1 ns |    182.40 ns |    80.99 ns |  3.68 |    0.07 |    3 |         - |          NA |
| RadixMSD4Sort       | 256  | PipeOrgan          |   3,423.8 ns |    418.90 ns |   219.09 ns |  3.20 |    0.19 |    3 |         - |          NA |
| RadixMSD10Sort      | 256  | PipeOrgan          |   4,221.8 ns |    288.66 ns |   150.97 ns |  3.95 |    0.13 |    3 |         - |          NA |
| AmericanFlagSort    | 256  | PipeOrgan          |   2,268.8 ns |     34.26 ns |    12.22 ns |  2.12 |    0.01 |    2 |         - |          NA |
| SpreadSort          | 256  | PipeOrgan          |   1,740.8 ns |     56.29 ns |    24.99 ns |  1.63 |    0.02 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **256**  | **ManyDuplicates**     |   **1,523.2 ns** |     **12.61 ns** |     **6.60 ns** |  **1.72** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | ManyDuplicates     |     885.2 ns |     33.16 ns |    11.83 ns |  1.00 |    0.02 |    2 |         - |          NA |
| PigeonSort          | 256  | ManyDuplicates     |   1,455.9 ns |      9.74 ns |     4.33 ns |  1.65 |    0.02 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | ManyDuplicates     |     624.8 ns |      4.49 ns |     1.99 ns |  0.71 |    0.01 |    1 |         - |          NA |
| BucketSort          | 256  | ManyDuplicates     |   3,100.8 ns |    247.37 ns |   109.84 ns |  3.50 |    0.12 |    5 |         - |          NA |
| BucketSortInteger   | 256  | ManyDuplicates     |   1,721.0 ns |      9.38 ns |     3.34 ns |  1.94 |    0.02 |    3 |         - |          NA |
| FlashSort           | 256  | ManyDuplicates     |   4,796.8 ns |    352.58 ns |   184.41 ns |  5.42 |    0.21 |    6 |         - |          NA |
| RadixLSD4Sort       | 256  | ManyDuplicates     |   2,298.9 ns |     22.16 ns |     9.84 ns |  2.60 |    0.03 |    4 |         - |          NA |
| RadixLSD256Sort     | 256  | ManyDuplicates     |   1,442.8 ns |    154.18 ns |    54.98 ns |  1.63 |    0.06 |    3 |         - |          NA |
| RadixLSD10Sort      | 256  | ManyDuplicates     |   2,937.8 ns |    263.82 ns |   137.98 ns |  3.32 |    0.15 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | ManyDuplicates     |   2,930.3 ns |     53.91 ns |    23.94 ns |  3.31 |    0.05 |    5 |         - |          NA |
| RadixMSD10Sort      | 256  | ManyDuplicates     |   3,702.0 ns |    144.95 ns |    51.69 ns |  4.18 |    0.07 |    5 |         - |          NA |
| AmericanFlagSort    | 256  | ManyDuplicates     |   3,352.9 ns |    318.18 ns |   166.42 ns |  3.79 |    0.18 |    5 |         - |          NA |
| SpreadSort          | 256  | ManyDuplicates     |   1,621.9 ns |     83.95 ns |    37.27 ns |  1.83 |    0.05 |    3 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **1024** | **Random**             |   **5,886.7 ns** |     **15.99 ns** |     **5.70 ns** |  **1.52** |    **0.00** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Random             |   3,877.2 ns |     29.45 ns |    10.50 ns |  1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | Random             |   5,717.2 ns |    270.56 ns |   141.51 ns |  1.47 |    0.03 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | Random             |   2,875.4 ns |    209.58 ns |    93.06 ns |  0.74 |    0.02 |    1 |         - |          NA |
| BucketSort          | 1024 | Random             |   8,229.2 ns |    249.71 ns |   110.87 ns |  2.12 |    0.03 |    4 |         - |          NA |
| BucketSortInteger   | 1024 | Random             |   5,981.9 ns |    341.81 ns |   178.77 ns |  1.54 |    0.04 |    3 |         - |          NA |
| FlashSort           | 1024 | Random             |  18,721.2 ns |    309.31 ns |   161.77 ns |  4.83 |    0.04 |    6 |         - |          NA |
| RadixLSD4Sort       | 1024 | Random             |  13,888.5 ns |    386.40 ns |   202.10 ns |  3.58 |    0.05 |    5 |         - |          NA |
| RadixLSD256Sort     | 1024 | Random             |   7,653.2 ns |    209.33 ns |   109.48 ns |  1.97 |    0.03 |    4 |         - |          NA |
| RadixLSD10Sort      | 1024 | Random             |  21,372.8 ns |  1,253.79 ns |   655.76 ns |  5.51 |    0.16 |    6 |         - |          NA |
| RadixMSD4Sort       | 1024 | Random             |  14,835.5 ns |    257.95 ns |   114.53 ns |  3.83 |    0.03 |    5 |         - |          NA |
| RadixMSD10Sort      | 1024 | Random             |  22,171.4 ns |    412.96 ns |   215.99 ns |  5.72 |    0.05 |    6 |         - |          NA |
| AmericanFlagSort    | 1024 | Random             |  18,237.3 ns |    124.29 ns |    65.00 ns |  4.70 |    0.02 |    6 |         - |          NA |
| SpreadSort          | 1024 | Random             |   9,143.2 ns |    442.98 ns |   231.69 ns |  2.36 |    0.06 |    4 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **1024** | **SingleElementMoved** |   **6,043.8 ns** |    **319.72 ns** |   **167.22 ns** |  **1.45** |    **0.06** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | SingleElementMoved |   4,176.5 ns |    261.05 ns |   136.54 ns |  1.00 |    0.04 |    2 |         - |          NA |
| PigeonSort          | 1024 | SingleElementMoved |   5,048.4 ns |    213.09 ns |    75.99 ns |  1.21 |    0.04 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | SingleElementMoved |   2,859.1 ns |    190.56 ns |    99.66 ns |  0.69 |    0.03 |    1 |         - |          NA |
| BucketSort          | 1024 | SingleElementMoved |   7,726.3 ns |    152.60 ns |    79.81 ns |  1.85 |    0.06 |    3 |         - |          NA |
| BucketSortInteger   | 1024 | SingleElementMoved |   6,249.3 ns |    284.20 ns |   148.64 ns |  1.50 |    0.06 |    3 |         - |          NA |
| FlashSort           | 1024 | SingleElementMoved |  21,580.3 ns |    391.23 ns |   173.71 ns |  5.17 |    0.16 |    5 |         - |          NA |
| RadixLSD4Sort       | 1024 | SingleElementMoved |  15,177.8 ns |    366.74 ns |   191.81 ns |  3.64 |    0.12 |    4 |         - |          NA |
| RadixLSD256Sort     | 1024 | SingleElementMoved |   6,480.4 ns |    292.60 ns |   153.04 ns |  1.55 |    0.06 |    3 |         - |          NA |
| RadixLSD10Sort      | 1024 | SingleElementMoved |  21,367.0 ns |    210.47 ns |    93.45 ns |  5.12 |    0.16 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | SingleElementMoved |  12,675.5 ns |    219.21 ns |    97.33 ns |  3.04 |    0.09 |    4 |         - |          NA |
| RadixMSD10Sort      | 1024 | SingleElementMoved |  19,484.0 ns |    178.33 ns |    93.27 ns |  4.67 |    0.14 |    5 |         - |          NA |
| AmericanFlagSort    | 1024 | SingleElementMoved |  12,773.8 ns |     48.57 ns |    21.57 ns |  3.06 |    0.09 |    4 |         - |          NA |
| SpreadSort          | 1024 | SingleElementMoved |   6,958.7 ns |    363.01 ns |   189.86 ns |  1.67 |    0.07 |    3 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **1024** | **Sorted**             |   **5,840.7 ns** |    **346.29 ns** |   **181.11 ns** |  **1.71** |    **0.05** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Sorted             |   3,418.8 ns |      7.93 ns |     2.83 ns |  1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 1024 | Sorted             |   5,374.2 ns |    282.83 ns |   147.93 ns |  1.57 |    0.04 |    4 |         - |          NA |
| PigeonSortInteger   | 1024 | Sorted             |   2,571.8 ns |     16.70 ns |     7.41 ns |  0.75 |    0.00 |    2 |         - |          NA |
| BucketSort          | 1024 | Sorted             |   8,173.1 ns |    186.69 ns |    97.64 ns |  2.39 |    0.03 |    4 |         - |          NA |
| BucketSortInteger   | 1024 | Sorted             |   6,762.5 ns |    736.95 ns |   327.21 ns |  1.98 |    0.09 |    4 |         - |          NA |
| FlashSort           | 1024 | Sorted             |  21,404.4 ns |    522.22 ns |   231.87 ns |  6.26 |    0.06 |    6 |         - |          NA |
| RadixLSD4Sort       | 1024 | Sorted             |  21,305.4 ns |    417.97 ns |   218.61 ns |  6.23 |    0.06 |    6 |         - |          NA |
| RadixLSD256Sort     | 1024 | Sorted             |   6,591.6 ns |    315.88 ns |   165.21 ns |  1.93 |    0.05 |    4 |         - |          NA |
| RadixLSD10Sort      | 1024 | Sorted             |  20,807.3 ns |    252.78 ns |   132.21 ns |  6.09 |    0.04 |    6 |         - |          NA |
| RadixMSD4Sort       | 1024 | Sorted             |  13,319.5 ns |    304.90 ns |   108.73 ns |  3.90 |    0.03 |    5 |         - |          NA |
| RadixMSD10Sort      | 1024 | Sorted             |  19,525.6 ns |    162.02 ns |    84.74 ns |  5.71 |    0.02 |    6 |         - |          NA |
| AmericanFlagSort    | 1024 | Sorted             |   9,752.1 ns |    296.96 ns |   155.32 ns |  2.85 |    0.04 |    4 |         - |          NA |
| SpreadSort          | 1024 | Sorted             |     619.8 ns |     17.08 ns |     7.58 ns |  0.18 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **1024** | **Reversed**           |   **5,814.9 ns** |    **386.16 ns** |   **201.97 ns** |  **1.53** |    **0.05** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Reversed           |   3,788.5 ns |     11.08 ns |     3.95 ns |  1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | Reversed           |   4,939.5 ns |    186.60 ns |    82.85 ns |  1.30 |    0.02 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | Reversed           |   2,543.8 ns |    279.80 ns |   146.34 ns |  0.67 |    0.04 |    1 |         - |          NA |
| BucketSort          | 1024 | Reversed           |   7,584.5 ns |     68.38 ns |    30.36 ns |  2.00 |    0.01 |    4 |         - |          NA |
| BucketSortInteger   | 1024 | Reversed           |   6,013.2 ns |    476.40 ns |   249.17 ns |  1.59 |    0.06 |    3 |         - |          NA |
| FlashSort           | 1024 | Reversed           |  18,563.0 ns |    222.11 ns |    98.62 ns |  4.90 |    0.02 |    6 |         - |          NA |
| RadixLSD4Sort       | 1024 | Reversed           |  16,077.3 ns |    396.14 ns |   207.19 ns |  4.24 |    0.05 |    6 |         - |          NA |
| RadixLSD256Sort     | 1024 | Reversed           |   6,256.4 ns |    427.55 ns |   223.62 ns |  1.65 |    0.06 |    3 |         - |          NA |
| RadixLSD10Sort      | 1024 | Reversed           |  21,701.3 ns |    359.10 ns |   187.82 ns |  5.73 |    0.05 |    6 |         - |          NA |
| RadixMSD4Sort       | 1024 | Reversed           |  16,732.5 ns |    151.60 ns |    67.31 ns |  4.42 |    0.02 |    6 |         - |          NA |
| RadixMSD10Sort      | 1024 | Reversed           |  22,077.7 ns |    143.06 ns |    74.82 ns |  5.83 |    0.02 |    6 |         - |          NA |
| AmericanFlagSort    | 1024 | Reversed           |  12,080.7 ns |    247.74 ns |   129.57 ns |  3.19 |    0.03 |    5 |         - |          NA |
| SpreadSort          | 1024 | Reversed           |   5,092.0 ns |    283.59 ns |   148.32 ns |  1.34 |    0.04 |    3 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **1024** | **PipeOrgan**          |   **5,842.5 ns** |    **289.21 ns** |   **151.26 ns** |  **1.42** |    **0.03** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | PipeOrgan          |   4,100.6 ns |     17.77 ns |     6.34 ns |  1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | PipeOrgan          |   5,060.8 ns |    325.30 ns |   170.14 ns |  1.23 |    0.04 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | PipeOrgan          |   2,947.3 ns |    382.45 ns |   200.03 ns |  0.72 |    0.05 |    1 |         - |          NA |
| BucketSort          | 1024 | PipeOrgan          |   7,653.6 ns |     15.85 ns |     7.04 ns |  1.87 |    0.00 |    3 |         - |          NA |
| BucketSortInteger   | 1024 | PipeOrgan          |   6,251.8 ns |    244.68 ns |   127.97 ns |  1.52 |    0.03 |    3 |         - |          NA |
| FlashSort           | 1024 | PipeOrgan          |  19,766.4 ns |    148.44 ns |    65.91 ns |  4.82 |    0.02 |    4 |         - |          NA |
| RadixLSD4Sort       | 1024 | PipeOrgan          |  14,458.5 ns |    274.50 ns |   121.88 ns |  3.53 |    0.03 |    4 |         - |          NA |
| RadixLSD256Sort     | 1024 | PipeOrgan          |   6,475.1 ns |    503.00 ns |   223.33 ns |  1.58 |    0.05 |    3 |         - |          NA |
| RadixLSD10Sort      | 1024 | PipeOrgan          |  21,205.2 ns |    355.59 ns |   185.98 ns |  5.17 |    0.04 |    4 |         - |          NA |
| RadixMSD4Sort       | 1024 | PipeOrgan          |  16,920.9 ns |    556.98 ns |   291.31 ns |  4.13 |    0.07 |    4 |         - |          NA |
| RadixMSD10Sort      | 1024 | PipeOrgan          |  21,142.5 ns |    645.17 ns |   337.44 ns |  5.16 |    0.08 |    4 |         - |          NA |
| AmericanFlagSort    | 1024 | PipeOrgan          |  14,844.8 ns |    113.41 ns |    50.35 ns |  3.62 |    0.01 |    4 |         - |          NA |
| SpreadSort          | 1024 | PipeOrgan          |   7,153.8 ns |     51.84 ns |    27.11 ns |  1.74 |    0.01 |    3 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **1024** | **ManyDuplicates**     |   **5,492.1 ns** |    **232.00 ns** |   **121.34 ns** |  **1.67** |    **0.04** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 1024 | ManyDuplicates     |   3,295.1 ns |     22.49 ns |     8.02 ns |  1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | ManyDuplicates     |   5,802.6 ns |    267.93 ns |   140.13 ns |  1.76 |    0.04 |    4 |         - |          NA |
| PigeonSortInteger   | 1024 | ManyDuplicates     |   2,529.8 ns |    270.68 ns |   141.57 ns |  0.77 |    0.04 |    1 |         - |          NA |
| BucketSort          | 1024 | ManyDuplicates     |  12,012.1 ns |    344.14 ns |   179.99 ns |  3.65 |    0.05 |    5 |         - |          NA |
| BucketSortInteger   | 1024 | ManyDuplicates     |   6,540.0 ns |     50.77 ns |    18.11 ns |  1.98 |    0.01 |    4 |         - |          NA |
| FlashSort           | 1024 | ManyDuplicates     |  19,859.0 ns |    239.18 ns |   106.20 ns |  6.03 |    0.03 |    6 |         - |          NA |
| RadixLSD4Sort       | 1024 | ManyDuplicates     |   9,070.4 ns |    337.97 ns |   176.77 ns |  2.75 |    0.05 |    5 |         - |          NA |
| RadixLSD256Sort     | 1024 | ManyDuplicates     |   4,219.9 ns |     18.87 ns |     6.73 ns |  1.28 |    0.00 |    3 |         - |          NA |
| RadixLSD10Sort      | 1024 | ManyDuplicates     |  11,441.6 ns |    227.75 ns |   101.12 ns |  3.47 |    0.03 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | ManyDuplicates     |  10,828.8 ns |    350.71 ns |   183.43 ns |  3.29 |    0.05 |    5 |         - |          NA |
| RadixMSD10Sort      | 1024 | ManyDuplicates     |  12,672.1 ns |    261.35 ns |   136.69 ns |  3.85 |    0.04 |    5 |         - |          NA |
| AmericanFlagSort    | 1024 | ManyDuplicates     |   9,886.9 ns |    296.43 ns |   155.04 ns |  3.00 |    0.04 |    5 |         - |          NA |
| SpreadSort          | 1024 | ManyDuplicates     |   6,744.3 ns |    302.58 ns |   158.26 ns |  2.05 |    0.05 |    4 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **4096** | **Random**             |  **24,755.4 ns** |    **272.64 ns** |   **142.60 ns** |  **1.57** |    **0.03** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Random             |  15,815.3 ns |    660.02 ns |   293.05 ns |  1.00 |    0.02 |    2 |         - |          NA |
| PigeonSort          | 4096 | Random             |  22,477.7 ns |    336.17 ns |   119.88 ns |  1.42 |    0.03 |    3 |         - |          NA |
| PigeonSortInteger   | 4096 | Random             |  11,402.4 ns |    450.76 ns |   200.14 ns |  0.72 |    0.02 |    1 |         - |          NA |
| BucketSort          | 4096 | Random             |  33,734.1 ns |  1,617.65 ns |   846.06 ns |  2.13 |    0.06 |    4 |         - |          NA |
| BucketSortInteger   | 4096 | Random             |  23,848.2 ns |    319.19 ns |   141.72 ns |  1.51 |    0.03 |    3 |         - |          NA |
| FlashSort           | 4096 | Random             | 103,927.2 ns |  2,508.11 ns | 1,311.79 ns |  6.57 |    0.14 |    6 |         - |          NA |
| RadixLSD4Sort       | 4096 | Random             |  65,354.4 ns |    442.82 ns |   196.62 ns |  4.13 |    0.07 |    5 |         - |          NA |
| RadixLSD256Sort     | 4096 | Random             |  25,354.7 ns |    258.63 ns |   114.83 ns |  1.60 |    0.03 |    3 |         - |          NA |
| RadixLSD10Sort      | 4096 | Random             |  85,153.6 ns |    716.35 ns |   318.06 ns |  5.39 |    0.09 |    5 |         - |          NA |
| RadixMSD4Sort       | 4096 | Random             |  72,577.2 ns |  1,506.89 ns |   788.13 ns |  4.59 |    0.09 |    5 |         - |          NA |
| RadixMSD10Sort      | 4096 | Random             |  86,524.3 ns |    548.22 ns |   243.41 ns |  5.47 |    0.09 |    5 |         - |          NA |
| AmericanFlagSort    | 4096 | Random             |  72,525.9 ns |  1,089.39 ns |   483.70 ns |  4.59 |    0.08 |    5 |         - |          NA |
| SpreadSort          | 4096 | Random             |  38,568.7 ns |    363.60 ns |   161.44 ns |  2.44 |    0.04 |    4 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **4096** | **SingleElementMoved** |  **24,073.3 ns** |    **393.87 ns** |   **174.88 ns** |  **1.42** |    **0.04** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 4096 | SingleElementMoved |  16,931.8 ns |    938.16 ns |   490.68 ns |  1.00 |    0.04 |    2 |         - |          NA |
| PigeonSort          | 4096 | SingleElementMoved |  20,135.2 ns |    881.41 ns |   461.00 ns |  1.19 |    0.04 |    2 |         - |          NA |
| PigeonSortInteger   | 4096 | SingleElementMoved |  11,310.1 ns |    318.61 ns |   113.62 ns |  0.67 |    0.02 |    1 |         - |          NA |
| BucketSort          | 4096 | SingleElementMoved |  30,316.0 ns |    654.52 ns |   290.61 ns |  1.79 |    0.05 |    2 |         - |          NA |
| BucketSortInteger   | 4096 | SingleElementMoved |  29,087.0 ns |    800.22 ns |   355.30 ns |  1.72 |    0.05 |    2 |         - |          NA |
| FlashSort           | 4096 | SingleElementMoved |  85,887.5 ns |    591.94 ns |   309.60 ns |  5.08 |    0.14 |    5 |         - |          NA |
| RadixLSD4Sort       | 4096 | SingleElementMoved |  92,746.6 ns |    558.81 ns |   292.27 ns |  5.48 |    0.15 |    5 |         - |          NA |
| RadixLSD256Sort     | 4096 | SingleElementMoved |  23,002.5 ns |    751.65 ns |   393.13 ns |  1.36 |    0.04 |    2 |         - |          NA |
| RadixLSD10Sort      | 4096 | SingleElementMoved |  84,869.4 ns |  1,839.47 ns |   816.73 ns |  5.02 |    0.14 |    5 |         - |          NA |
| RadixMSD4Sort       | 4096 | SingleElementMoved |  60,040.4 ns |    805.15 ns |   421.11 ns |  3.55 |    0.10 |    4 |         - |          NA |
| RadixMSD10Sort      | 4096 | SingleElementMoved |  78,515.4 ns |    693.25 ns |   362.58 ns |  4.64 |    0.13 |    5 |         - |          NA |
| AmericanFlagSort    | 4096 | SingleElementMoved |  48,276.6 ns |  1,065.15 ns |   557.10 ns |  2.85 |    0.08 |    3 |         - |          NA |
| SpreadSort          | 4096 | SingleElementMoved |  27,149.5 ns |    195.91 ns |    86.99 ns |  1.60 |    0.04 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **4096** | **Sorted**             |  **22,406.6 ns** |    **366.43 ns** |   **162.70 ns** |  **1.63** |    **0.01** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Sorted             |  13,733.1 ns |    168.86 ns |    74.97 ns |  1.00 |    0.01 |    3 |         - |          NA |
| PigeonSort          | 4096 | Sorted             |  21,308.8 ns |    823.50 ns |   365.64 ns |  1.55 |    0.03 |    4 |         - |          NA |
| PigeonSortInteger   | 4096 | Sorted             |  10,108.0 ns |    806.79 ns |   421.97 ns |  0.74 |    0.03 |    2 |         - |          NA |
| BucketSort          | 4096 | Sorted             |  35,670.4 ns |    886.95 ns |   463.89 ns |  2.60 |    0.03 |    5 |         - |          NA |
| BucketSortInteger   | 4096 | Sorted             |  25,060.0 ns |    773.26 ns |   404.43 ns |  1.82 |    0.03 |    4 |         - |          NA |
| FlashSort           | 4096 | Sorted             |  85,712.3 ns |    747.76 ns |   332.01 ns |  6.24 |    0.04 |    7 |         - |          NA |
| RadixLSD4Sort       | 4096 | Sorted             |  91,165.2 ns |  1,725.79 ns |   902.62 ns |  6.64 |    0.07 |    7 |         - |          NA |
| RadixLSD256Sort     | 4096 | Sorted             |  23,793.3 ns |    378.66 ns |   168.13 ns |  1.73 |    0.01 |    4 |         - |          NA |
| RadixLSD10Sort      | 4096 | Sorted             |  84,558.0 ns |    700.37 ns |   366.31 ns |  6.16 |    0.04 |    7 |         - |          NA |
| RadixMSD4Sort       | 4096 | Sorted             |  59,814.4 ns |    462.12 ns |   241.70 ns |  4.36 |    0.03 |    6 |         - |          NA |
| RadixMSD10Sort      | 4096 | Sorted             |  78,729.7 ns |    960.15 ns |   502.18 ns |  5.73 |    0.05 |    7 |         - |          NA |
| AmericanFlagSort    | 4096 | Sorted             |  35,180.1 ns |    743.05 ns |   329.92 ns |  2.56 |    0.03 |    5 |         - |          NA |
| SpreadSort          | 4096 | Sorted             |   2,258.6 ns |      8.62 ns |     4.51 ns |  0.16 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **4096** | **Reversed**           |  **22,655.1 ns** |  **1,070.01 ns** |   **559.64 ns** |  **1.48** |    **0.04** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Reversed           |  15,301.0 ns |    277.08 ns |    98.81 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PigeonSort          | 4096 | Reversed           |  19,972.3 ns |    969.33 ns |   506.98 ns |  1.31 |    0.03 |    3 |         - |          NA |
| PigeonSortInteger   | 4096 | Reversed           |  10,031.4 ns |    519.13 ns |   185.13 ns |  0.66 |    0.01 |    1 |         - |          NA |
| BucketSort          | 4096 | Reversed           |  30,635.2 ns |    629.70 ns |   329.34 ns |  2.00 |    0.02 |    4 |         - |          NA |
| BucketSortInteger   | 4096 | Reversed           |  23,880.2 ns |    546.82 ns |   242.79 ns |  1.56 |    0.02 |    3 |         - |          NA |
| FlashSort           | 4096 | Reversed           |  76,682.8 ns |    697.27 ns |   364.68 ns |  5.01 |    0.04 |    6 |         - |          NA |
| RadixLSD4Sort       | 4096 | Reversed           |  83,651.3 ns |  1,314.34 ns |   687.42 ns |  5.47 |    0.05 |    6 |         - |          NA |
| RadixLSD256Sort     | 4096 | Reversed           |  21,906.4 ns |    244.58 ns |    87.22 ns |  1.43 |    0.01 |    3 |         - |          NA |
| RadixLSD10Sort      | 4096 | Reversed           |  84,355.3 ns |  1,338.27 ns |   594.20 ns |  5.51 |    0.05 |    6 |         - |          NA |
| RadixMSD4Sort       | 4096 | Reversed           |  75,501.0 ns |    614.95 ns |   321.63 ns |  4.93 |    0.04 |    6 |         - |          NA |
| RadixMSD10Sort      | 4096 | Reversed           |  87,092.7 ns |    476.29 ns |   211.48 ns |  5.69 |    0.04 |    6 |         - |          NA |
| AmericanFlagSort    | 4096 | Reversed           |  45,125.8 ns |  1,181.69 ns |   618.05 ns |  2.95 |    0.04 |    5 |         - |          NA |
| SpreadSort          | 4096 | Reversed           |  20,097.1 ns |  1,341.76 ns |   595.75 ns |  1.31 |    0.04 |    3 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **4096** | **PipeOrgan**          |  **23,403.4 ns** |  **1,147.23 ns** |   **509.38 ns** |  **1.39** |    **0.03** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 4096 | PipeOrgan          |  16,840.9 ns |     28.55 ns |    10.18 ns |  1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | PipeOrgan          |  20,153.0 ns |  1,140.11 ns |   596.30 ns |  1.20 |    0.03 |    2 |         - |          NA |
| PigeonSortInteger   | 4096 | PipeOrgan          |  12,157.1 ns |  1,850.91 ns |   968.06 ns |  0.72 |    0.05 |    1 |         - |          NA |
| BucketSort          | 4096 | PipeOrgan          |  30,905.8 ns |    519.26 ns |   230.55 ns |  1.84 |    0.01 |    3 |         - |          NA |
| BucketSortInteger   | 4096 | PipeOrgan          |  25,228.9 ns |  1,066.03 ns |   557.55 ns |  1.50 |    0.03 |    2 |         - |          NA |
| FlashSort           | 4096 | PipeOrgan          |  99,156.3 ns |  4,372.80 ns | 2,287.06 ns |  5.89 |    0.13 |    4 |         - |          NA |
| RadixLSD4Sort       | 4096 | PipeOrgan          |  71,769.8 ns |    653.27 ns |   290.06 ns |  4.26 |    0.02 |    4 |         - |          NA |
| RadixLSD256Sort     | 4096 | PipeOrgan          |  23,742.2 ns |    691.53 ns |   361.68 ns |  1.41 |    0.02 |    2 |         - |          NA |
| RadixLSD10Sort      | 4096 | PipeOrgan          |  84,655.8 ns |  1,003.45 ns |   524.83 ns |  5.03 |    0.03 |    4 |         - |          NA |
| RadixMSD4Sort       | 4096 | PipeOrgan          |  75,985.3 ns |    790.72 ns |   413.56 ns |  4.51 |    0.02 |    4 |         - |          NA |
| RadixMSD10Sort      | 4096 | PipeOrgan          |  85,505.0 ns |    855.90 ns |   380.02 ns |  5.08 |    0.02 |    4 |         - |          NA |
| AmericanFlagSort    | 4096 | PipeOrgan          |  60,802.9 ns |    691.06 ns |   361.44 ns |  3.61 |    0.02 |    4 |         - |          NA |
| SpreadSort          | 4096 | PipeOrgan          |  30,855.2 ns |    761.12 ns |   398.08 ns |  1.83 |    0.02 |    3 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **4096** | **ManyDuplicates**     |  **21,938.5 ns** |    **321.16 ns** |   **167.97 ns** |  **1.68** |    **0.02** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 4096 | ManyDuplicates     |  13,028.6 ns |    180.22 ns |    80.02 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PigeonSort          | 4096 | ManyDuplicates     |  26,818.3 ns |    138.72 ns |    61.59 ns |  2.06 |    0.01 |    5 |         - |          NA |
| PigeonSortInteger   | 4096 | ManyDuplicates     |  10,123.7 ns |    302.88 ns |   158.41 ns |  0.78 |    0.01 |    1 |         - |          NA |
| BucketSort          | 4096 | ManyDuplicates     |  48,009.1 ns |    497.43 ns |   260.17 ns |  3.69 |    0.03 |    5 |         - |          NA |
| BucketSortInteger   | 4096 | ManyDuplicates     |  27,085.6 ns |    217.98 ns |    96.79 ns |  2.08 |    0.01 |    5 |         - |          NA |
| FlashSort           | 4096 | ManyDuplicates     |  72,925.8 ns |    994.56 ns |   441.59 ns |  5.60 |    0.05 |    6 |         - |          NA |
| RadixLSD4Sort       | 4096 | ManyDuplicates     |  36,447.1 ns |    848.54 ns |   443.80 ns |  2.80 |    0.04 |    5 |         - |          NA |
| RadixLSD256Sort     | 4096 | ManyDuplicates     |  16,466.2 ns |    738.09 ns |   327.72 ns |  1.26 |    0.02 |    3 |         - |          NA |
| RadixLSD10Sort      | 4096 | ManyDuplicates     |  45,881.4 ns |    730.72 ns |   382.18 ns |  3.52 |    0.03 |    5 |         - |          NA |
| RadixMSD4Sort       | 4096 | ManyDuplicates     |  40,421.6 ns |    468.13 ns |   207.85 ns |  3.10 |    0.02 |    5 |         - |          NA |
| RadixMSD10Sort      | 4096 | ManyDuplicates     |  49,296.5 ns |    682.36 ns |   356.89 ns |  3.78 |    0.03 |    5 |         - |          NA |
| AmericanFlagSort    | 4096 | ManyDuplicates     |  31,415.1 ns |    577.10 ns |   301.83 ns |  2.41 |    0.03 |    5 |         - |          NA |
| SpreadSort          | 4096 | ManyDuplicates     |  27,282.3 ns |  1,688.73 ns |   883.24 ns |  2.09 |    0.07 |    5 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **8192** | **Random**             |  **51,750.7 ns** |  **2,684.45 ns** | **1,191.91 ns** |  **1.52** |    **0.05** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Random             |  33,988.2 ns |  1,916.10 ns | 1,002.16 ns |  1.00 |    0.04 |    2 |         - |          NA |
| PigeonSort          | 8192 | Random             |  45,596.9 ns |    861.80 ns |   450.74 ns |  1.34 |    0.04 |    3 |         - |          NA |
| PigeonSortInteger   | 8192 | Random             |  22,929.3 ns |    715.85 ns |   317.84 ns |  0.68 |    0.02 |    1 |         - |          NA |
| BucketSort          | 8192 | Random             |  68,156.5 ns |    706.68 ns |   369.60 ns |  2.01 |    0.06 |    4 |         - |          NA |
| BucketSortInteger   | 8192 | Random             |  50,588.9 ns |    855.17 ns |   447.27 ns |  1.49 |    0.04 |    3 |         - |          NA |
| FlashSort           | 8192 | Random             | 216,675.7 ns |  2,437.87 ns | 1,082.43 ns |  6.38 |    0.18 |    7 |         - |          NA |
| RadixLSD4Sort       | 8192 | Random             | 152,387.6 ns |    853.67 ns |   379.04 ns |  4.49 |    0.12 |    6 |         - |          NA |
| RadixLSD256Sort     | 8192 | Random             |  51,672.3 ns |  1,049.12 ns |   548.71 ns |  1.52 |    0.04 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | Random             | 170,604.5 ns |  1,897.54 ns |   842.52 ns |  5.02 |    0.14 |    6 |         - |          NA |
| RadixMSD4Sort       | 8192 | Random             | 167,898.5 ns |  1,730.81 ns |   905.25 ns |  4.94 |    0.14 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | Random             | 174,703.0 ns |  1,311.95 ns |   686.18 ns |  5.14 |    0.14 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | Random             | 144,178.0 ns |    787.09 ns |   349.47 ns |  4.25 |    0.12 |    6 |         - |          NA |
| SpreadSort          | 8192 | Random             |  97,716.2 ns |    843.15 ns |   440.98 ns |  2.88 |    0.08 |    5 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **8192** | **SingleElementMoved** |  **48,737.8 ns** |    **627.12 ns** |   **328.00 ns** |  **1.45** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | SingleElementMoved |  33,687.2 ns |    339.81 ns |   150.88 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PigeonSort          | 8192 | SingleElementMoved |  39,271.2 ns |  1,197.26 ns |   626.19 ns |  1.17 |    0.02 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | SingleElementMoved |  22,782.8 ns |    562.62 ns |   249.81 ns |  0.68 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | SingleElementMoved |  61,007.9 ns |  1,168.68 ns |   611.24 ns |  1.81 |    0.02 |    3 |         - |          NA |
| BucketSortInteger   | 8192 | SingleElementMoved |  53,137.5 ns |  1,336.76 ns |   593.53 ns |  1.58 |    0.02 |    3 |         - |          NA |
| FlashSort           | 8192 | SingleElementMoved | 171,876.3 ns |  1,183.81 ns |   619.16 ns |  5.10 |    0.03 |    5 |         - |          NA |
| RadixLSD4Sort       | 8192 | SingleElementMoved | 212,765.6 ns |  1,579.66 ns |   826.19 ns |  6.32 |    0.04 |    6 |         - |          NA |
| RadixLSD256Sort     | 8192 | SingleElementMoved |  47,847.9 ns |  1,077.66 ns |   563.64 ns |  1.42 |    0.02 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | SingleElementMoved | 167,670.2 ns |  2,065.11 ns | 1,080.09 ns |  4.98 |    0.04 |    5 |         - |          NA |
| RadixMSD4Sort       | 8192 | SingleElementMoved | 138,059.0 ns |  1,344.88 ns |   703.40 ns |  4.10 |    0.03 |    5 |         - |          NA |
| RadixMSD10Sort      | 8192 | SingleElementMoved | 156,833.3 ns |  1,639.15 ns |   857.31 ns |  4.66 |    0.03 |    5 |         - |          NA |
| AmericanFlagSort    | 8192 | SingleElementMoved |  94,995.6 ns |    724.19 ns |   321.54 ns |  2.82 |    0.01 |    4 |         - |          NA |
| SpreadSort          | 8192 | SingleElementMoved |  56,547.2 ns |  1,300.36 ns |   577.37 ns |  1.68 |    0.02 |    3 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **8192** | **Sorted**             |  **45,407.0 ns** |    **626.04 ns** |   **277.96 ns** |  **1.62** |    **0.05** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Sorted             |  28,089.4 ns |  1,913.46 ns | 1,000.78 ns |  1.00 |    0.05 |    3 |         - |          NA |
| PigeonSort          | 8192 | Sorted             |  43,090.0 ns |  1,440.86 ns |   753.60 ns |  1.54 |    0.06 |    4 |         - |          NA |
| PigeonSortInteger   | 8192 | Sorted             |  20,064.3 ns |  1,139.36 ns |   595.91 ns |  0.72 |    0.03 |    2 |         - |          NA |
| BucketSort          | 8192 | Sorted             |  63,983.8 ns |  1,967.68 ns |   873.66 ns |  2.28 |    0.08 |    5 |         - |          NA |
| BucketSortInteger   | 8192 | Sorted             |  47,325.2 ns |    867.41 ns |   453.67 ns |  1.69 |    0.06 |    4 |         - |          NA |
| FlashSort           | 8192 | Sorted             | 218,772.4 ns |  8,304.22 ns | 4,343.27 ns |  7.80 |    0.29 |    7 |         - |          NA |
| RadixLSD4Sort       | 8192 | Sorted             | 202,551.6 ns |  2,449.00 ns | 1,280.88 ns |  7.22 |    0.24 |    7 |         - |          NA |
| RadixLSD256Sort     | 8192 | Sorted             |  49,241.1 ns |    324.09 ns |   143.90 ns |  1.75 |    0.06 |    4 |         - |          NA |
| RadixLSD10Sort      | 8192 | Sorted             | 167,622.8 ns |  2,577.65 ns | 1,348.16 ns |  5.97 |    0.20 |    6 |         - |          NA |
| RadixMSD4Sort       | 8192 | Sorted             | 138,846.9 ns |    436.10 ns |   228.09 ns |  4.95 |    0.16 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | Sorted             | 158,279.8 ns |  1,383.09 ns |   723.38 ns |  5.64 |    0.19 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | Sorted             |  68,650.9 ns |    767.24 ns |   340.66 ns |  2.45 |    0.08 |    5 |         - |          NA |
| SpreadSort          | 8192 | Sorted             |   4,619.5 ns |    326.48 ns |   170.76 ns |  0.16 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **8192** | **Reversed**           |  **45,242.6 ns** |    **766.33 ns** |   **400.81 ns** |  **1.46** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Reversed           |  30,908.7 ns |    469.00 ns |   208.24 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PigeonSort          | 8192 | Reversed           |  39,601.3 ns |  1,278.69 ns |   668.78 ns |  1.28 |    0.02 |    3 |         - |          NA |
| PigeonSortInteger   | 8192 | Reversed           |  20,504.3 ns |  1,103.93 ns |   577.38 ns |  0.66 |    0.02 |    1 |         - |          NA |
| BucketSort          | 8192 | Reversed           |  62,367.7 ns |  1,057.66 ns |   553.18 ns |  2.02 |    0.02 |    4 |         - |          NA |
| BucketSortInteger   | 8192 | Reversed           |  47,510.1 ns |  1,020.53 ns |   533.75 ns |  1.54 |    0.02 |    3 |         - |          NA |
| FlashSort           | 8192 | Reversed           | 185,601.4 ns | 12,415.22 ns | 6,493.40 ns |  6.01 |    0.20 |    6 |         - |          NA |
| RadixLSD4Sort       | 8192 | Reversed           | 205,463.1 ns |  2,655.71 ns | 1,388.99 ns |  6.65 |    0.06 |    6 |         - |          NA |
| RadixLSD256Sort     | 8192 | Reversed           |  46,763.1 ns |  1,557.53 ns |   814.62 ns |  1.51 |    0.03 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | Reversed           | 168,258.2 ns |  1,680.82 ns |   879.10 ns |  5.44 |    0.04 |    6 |         - |          NA |
| RadixMSD4Sort       | 8192 | Reversed           | 167,401.6 ns |    423.45 ns |   188.01 ns |  5.42 |    0.03 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | Reversed           | 174,475.0 ns |    935.44 ns |   489.25 ns |  5.65 |    0.04 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | Reversed           |  89,582.5 ns |  1,250.03 ns |   555.02 ns |  2.90 |    0.02 |    5 |         - |          NA |
| SpreadSort          | 8192 | Reversed           |  75,640.9 ns |    981.30 ns |   513.24 ns |  2.45 |    0.02 |    5 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **8192** | **PipeOrgan**          |  **46,950.6 ns** |  **1,448.24 ns** |   **643.03 ns** |  **1.30** |    **0.02** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | PipeOrgan          |  36,171.6 ns |    417.88 ns |   185.54 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PigeonSort          | 8192 | PipeOrgan          |  39,860.6 ns |  1,036.12 ns |   541.91 ns |  1.10 |    0.02 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | PipeOrgan          |  23,009.0 ns |    471.76 ns |   209.46 ns |  0.64 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | PipeOrgan          |  62,631.6 ns |    620.49 ns |   324.53 ns |  1.73 |    0.01 |    3 |         - |          NA |
| BucketSortInteger   | 8192 | PipeOrgan          |  50,896.7 ns |    614.78 ns |   219.24 ns |  1.41 |    0.01 |    2 |         - |          NA |
| FlashSort           | 8192 | PipeOrgan          | 162,601.8 ns |  1,234.90 ns |   645.88 ns |  4.50 |    0.03 |    6 |         - |          NA |
| RadixLSD4Sort       | 8192 | PipeOrgan          | 172,593.0 ns |  1,306.45 ns |   683.30 ns |  4.77 |    0.03 |    6 |         - |          NA |
| RadixLSD256Sort     | 8192 | PipeOrgan          |  48,752.8 ns |  1,378.23 ns |   611.94 ns |  1.35 |    0.02 |    2 |         - |          NA |
| RadixLSD10Sort      | 8192 | PipeOrgan          | 166,449.9 ns |  3,585.70 ns | 1,875.39 ns |  4.60 |    0.05 |    6 |         - |          NA |
| RadixMSD4Sort       | 8192 | PipeOrgan          | 169,323.7 ns |  2,028.74 ns | 1,061.07 ns |  4.68 |    0.04 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | PipeOrgan          | 172,630.4 ns |  1,456.37 ns |   761.71 ns |  4.77 |    0.03 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | PipeOrgan          | 122,438.7 ns |  1,162.48 ns |   608.00 ns |  3.39 |    0.02 |    5 |         - |          NA |
| SpreadSort          | 8192 | PipeOrgan          |  96,639.2 ns |  1,497.35 ns |   783.14 ns |  2.67 |    0.02 |    4 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **8192** | **ManyDuplicates**     |  **44,505.7 ns** |    **783.18 ns** |   **409.62 ns** |  **1.68** |    **0.02** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 8192 | ManyDuplicates     |  26,427.4 ns |    519.95 ns |   230.86 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PigeonSort          | 8192 | ManyDuplicates     |  75,636.5 ns |    814.36 ns |   425.93 ns |  2.86 |    0.03 |    5 |         - |          NA |
| PigeonSortInteger   | 8192 | ManyDuplicates     |  19,972.9 ns |    301.59 ns |   133.91 ns |  0.76 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | ManyDuplicates     |  96,974.8 ns |    835.79 ns |   437.13 ns |  3.67 |    0.03 |    5 |         - |          NA |
| BucketSortInteger   | 8192 | ManyDuplicates     |  54,451.9 ns |  1,150.66 ns |   510.90 ns |  2.06 |    0.02 |    4 |         - |          NA |
| FlashSort           | 8192 | ManyDuplicates     | 147,860.3 ns |    847.17 ns |   376.15 ns |  5.60 |    0.05 |    6 |         - |          NA |
| RadixLSD4Sort       | 8192 | ManyDuplicates     |  74,064.1 ns |  1,070.17 ns |   559.72 ns |  2.80 |    0.03 |    5 |         - |          NA |
| RadixLSD256Sort     | 8192 | ManyDuplicates     |  33,000.1 ns |  1,285.59 ns |   672.39 ns |  1.25 |    0.03 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | ManyDuplicates     |  91,887.6 ns |    827.12 ns |   432.60 ns |  3.48 |    0.03 |    5 |         - |          NA |
| RadixMSD4Sort       | 8192 | ManyDuplicates     |  79,570.5 ns |    932.91 ns |   414.22 ns |  3.01 |    0.03 |    5 |         - |          NA |
| RadixMSD10Sort      | 8192 | ManyDuplicates     |  98,133.5 ns |    667.21 ns |   348.96 ns |  3.71 |    0.03 |    5 |         - |          NA |
| AmericanFlagSort    | 8192 | ManyDuplicates     |  61,056.8 ns |    332.46 ns |   118.56 ns |  2.31 |    0.02 |    4 |         - |          NA |
| SpreadSort          | 8192 | ManyDuplicates     |  53,282.8 ns |  1,013.63 ns |   450.06 ns |  2.02 |    0.02 |    4 |         - |          NA |

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
| **BubbleSort**         | **256**  | **Random**             |  **28,128.0 ns** |   **322.99 ns** |   **115.18 ns** |   **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Random             |  17,087.8 ns |   915.31 ns |   326.41 ns |   0.61 |    0.01 |    2 |         - |          NA |
| OddEvenSort        | 256  | Random             |  22,137.9 ns |   367.22 ns |   192.07 ns |   0.79 |    0.01 |    2 |         - |          NA |
| CombSort           | 256  | Random             |   3,592.0 ns |   193.78 ns |    86.04 ns |   0.13 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | Random             |  18,672.7 ns |   161.26 ns |    71.60 ns |   0.66 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **SingleElementMoved** |     **413.5 ns** |     **1.47 ns** |     **0.65 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| CocktailShakerSort | 256  | SingleElementMoved |     369.0 ns |   103.62 ns |    54.20 ns |   0.89 |    0.12 |    1 |         - |          NA |
| OddEvenSort        | 256  | SingleElementMoved |  17,332.3 ns |   116.10 ns |    51.55 ns |  41.92 |    0.13 |    3 |         - |          NA |
| CombSort           | 256  | SingleElementMoved |   2,881.2 ns |    62.91 ns |    32.90 ns |   6.97 |    0.08 |    2 |         - |          NA |
| CircleSort         | 256  | SingleElementMoved |  15,659.0 ns |   155.04 ns |    81.09 ns |  37.87 |    0.19 |    3 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **Sorted**             |     **251.8 ns** |    **99.35 ns** |    **51.96 ns** |   **1.04** |    **0.28** |    **1** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Sorted             |     244.0 ns |    92.01 ns |    48.12 ns |   1.00 |    0.27 |    1 |         - |          NA |
| OddEvenSort        | 256  | Sorted             |     237.3 ns |   113.51 ns |    59.37 ns |   0.98 |    0.30 |    1 |         - |          NA |
| CombSort           | 256  | Sorted             |   2,923.1 ns |   394.13 ns |   206.14 ns |  12.03 |    2.36 |    3 |         - |          NA |
| CircleSort         | 256  | Sorted             |   2,283.8 ns |    79.08 ns |    41.36 ns |   9.40 |    1.74 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **Reversed**           |  **29,562.4 ns** |   **267.61 ns** |   **139.97 ns** |   **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Reversed           |  27,826.8 ns |   260.30 ns |   136.14 ns |   0.94 |    0.01 |    3 |         - |          NA |
| OddEvenSort        | 256  | Reversed           |  25,356.5 ns |   114.14 ns |    40.70 ns |   0.86 |    0.00 |    3 |         - |          NA |
| CombSort           | 256  | Reversed           |   2,877.6 ns |    48.48 ns |    17.29 ns |   0.10 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | Reversed           |   4,325.5 ns |    27.30 ns |     9.74 ns |   0.15 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **PipeOrgan**          |  **34,337.3 ns** |   **271.76 ns** |   **142.14 ns** |   **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 256  | PipeOrgan          |  18,045.7 ns |   458.07 ns |   239.58 ns |   0.53 |    0.01 |    2 |         - |          NA |
| OddEvenSort        | 256  | PipeOrgan          |  27,861.1 ns |   337.47 ns |   149.84 ns |   0.81 |    0.01 |    3 |         - |          NA |
| CombSort           | 256  | PipeOrgan          |   2,968.5 ns |    63.56 ns |    28.22 ns |   0.09 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | PipeOrgan          |  19,887.5 ns |   257.37 ns |   134.61 ns |   0.58 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **ManyDuplicates**     |  **29,158.5 ns** |   **452.21 ns** |   **236.52 ns** |   **1.00** |    **0.01** |    **5** |         **-** |          **NA** |
| CocktailShakerSort | 256  | ManyDuplicates     |  17,037.0 ns |    48.11 ns |    17.16 ns |   0.58 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 256  | ManyDuplicates     |  21,440.1 ns |   287.33 ns |   150.28 ns |   0.74 |    0.01 |    4 |         - |          NA |
| CombSort           | 256  | ManyDuplicates     |   3,267.5 ns |    32.60 ns |    17.05 ns |   0.11 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | ManyDuplicates     |  14,047.0 ns |   370.45 ns |   164.48 ns |   0.48 |    0.01 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Random**             | **533,447.9 ns** | **2,150.31 ns** |   **954.75 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Random             | 324,479.2 ns | 1,826.48 ns |   810.97 ns |   0.61 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | Random             | 472,503.2 ns | 2,893.85 ns | 1,284.89 ns |   0.89 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | Random             |  19,572.9 ns |   268.64 ns |   140.50 ns |   0.04 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | Random             | 101,731.0 ns | 1,845.34 ns |   965.15 ns |   0.19 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **SingleElementMoved** |   **1,697.7 ns** |     **4.43 ns** |     **1.58 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | SingleElementMoved |   1,293.1 ns |     5.22 ns |     2.32 ns |   0.76 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 1024 | SingleElementMoved | 251,651.7 ns |   471.26 ns |   209.24 ns | 148.23 |    0.17 |    5 |         - |          NA |
| CombSort           | 1024 | SingleElementMoved |  15,399.9 ns |   145.89 ns |    76.30 ns |   9.07 |    0.04 |    3 |         - |          NA |
| CircleSort         | 1024 | SingleElementMoved |  86,199.5 ns |   680.29 ns |   302.05 ns |  50.77 |    0.17 |    4 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Sorted**             |     **723.6 ns** |     **1.63 ns** |     **0.58 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Sorted             |     733.7 ns |     1.63 ns |     0.72 ns |   1.01 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 1024 | Sorted             |     742.8 ns |     1.65 ns |     0.73 ns |   1.03 |    0.00 |    1 |         - |          NA |
| CombSort           | 1024 | Sorted             |  14,536.2 ns |    93.30 ns |    41.42 ns |  20.09 |    0.06 |    3 |         - |          NA |
| CircleSort         | 1024 | Sorted             |   9,844.9 ns |   222.64 ns |   116.44 ns |  13.61 |    0.15 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Reversed**           | **437,377.9 ns** |   **496.64 ns** |   **220.51 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Reversed           | 436,001.9 ns |   917.98 ns |   480.12 ns |   1.00 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | Reversed           | 404,616.9 ns | 1,413.31 ns |   739.19 ns |   0.93 |    0.00 |    3 |         - |          NA |
| CombSort           | 1024 | Reversed           |  15,703.3 ns |   202.74 ns |   106.03 ns |   0.04 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | Reversed           |  19,279.0 ns |   259.72 ns |   115.32 ns |   0.04 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **PipeOrgan**          | **506,733.5 ns** | **1,180.59 ns** |   **617.47 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | PipeOrgan          | 283,986.1 ns | 1,479.34 ns |   773.73 ns |   0.56 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | PipeOrgan          | 448,957.2 ns |   912.67 ns |   405.23 ns |   0.89 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | PipeOrgan          |  16,057.0 ns |   142.25 ns |    50.73 ns |   0.03 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | PipeOrgan          | 106,625.1 ns |   724.76 ns |   379.06 ns |   0.21 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **ManyDuplicates**     | **539,526.0 ns** | **2,219.17 ns** |   **985.33 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | ManyDuplicates     | 318,926.8 ns | 2,575.56 ns | 1,143.57 ns |   0.59 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | ManyDuplicates     | 469,265.7 ns | 2,624.56 ns | 1,165.32 ns |   0.87 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | ManyDuplicates     |  16,844.8 ns |   195.66 ns |   102.34 ns |   0.03 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | ManyDuplicates     |  89,801.6 ns |   894.92 ns |   468.06 ns |   0.17 |    0.00 |    2 |         - |          NA |

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
| **HeapSort**         | **256**  | **Random**             |     **3,347.8 ns** |     **82.96 ns** |     **36.83 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | Random             |     3,436.2 ns |     66.58 ns |     29.56 ns |  1.03 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | Random             |     4,074.4 ns |    313.77 ns |    139.31 ns |  1.22 |    0.04 |    1 |         - |          NA |
| BottomupHeapSort | 256  | Random             |     4,407.5 ns |    247.75 ns |    129.58 ns |  1.32 |    0.04 |    1 |         - |          NA |
| WeakHeapSort     | 256  | Random             |    10,571.2 ns |    978.10 ns |    511.57 ns |  3.16 |    0.15 |    4 |         - |          NA |
| SmoothSort       | 256  | Random             |     6,128.3 ns |    682.76 ns |    243.48 ns |  1.83 |    0.07 |    2 |         - |          NA |
| TournamentSort   | 256  | Random             |     8,297.1 ns |    409.20 ns |    214.02 ns |  2.48 |    0.07 |    3 |         - |          NA |
| BinomialHeapSort | 256  | Random             |    15,283.3 ns |    460.27 ns |    204.36 ns |  4.57 |    0.07 |    5 |         - |          NA |
| PairingHeapSort  | 256  | Random             |    10,774.9 ns |    371.89 ns |    165.12 ns |  3.22 |    0.06 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **SingleElementMoved** |     **3,169.4 ns** |    **357.94 ns** |    **187.21 ns** |  **1.00** |    **0.08** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 256  | SingleElementMoved |     3,149.5 ns |    172.38 ns |     90.16 ns |  1.00 |    0.06 |    2 |         - |          NA |
| TernaryHeapSort  | 256  | SingleElementMoved |     4,256.0 ns |    259.06 ns |    135.49 ns |  1.35 |    0.08 |    3 |         - |          NA |
| BottomupHeapSort | 256  | SingleElementMoved |     4,452.5 ns |     65.05 ns |     28.88 ns |  1.41 |    0.08 |    3 |         - |          NA |
| WeakHeapSort     | 256  | SingleElementMoved |     8,727.8 ns |    197.64 ns |    103.37 ns |  2.76 |    0.15 |    5 |         - |          NA |
| SmoothSort       | 256  | SingleElementMoved |     1,742.9 ns |     39.68 ns |     14.15 ns |  0.55 |    0.03 |    1 |         - |          NA |
| TournamentSort   | 256  | SingleElementMoved |     5,730.7 ns |    719.58 ns |    376.35 ns |  1.81 |    0.15 |    4 |         - |          NA |
| BinomialHeapSort | 256  | SingleElementMoved |     7,393.7 ns |    350.63 ns |    183.38 ns |  2.34 |    0.14 |    5 |         - |          NA |
| PairingHeapSort  | 256  | SingleElementMoved |     5,510.6 ns |    223.48 ns |     99.23 ns |  1.74 |    0.10 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **Sorted**             |     **3,436.2 ns** |    **410.19 ns** |    **214.54 ns** |  **1.00** |    **0.08** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 256  | Sorted             |     3,630.8 ns |    330.14 ns |    172.67 ns |  1.06 |    0.08 |    2 |         - |          NA |
| TernaryHeapSort  | 256  | Sorted             |     4,216.3 ns |    300.17 ns |    133.28 ns |  1.23 |    0.08 |    2 |         - |          NA |
| BottomupHeapSort | 256  | Sorted             |     4,440.0 ns |    287.68 ns |    150.46 ns |  1.30 |    0.08 |    2 |         - |          NA |
| WeakHeapSort     | 256  | Sorted             |     8,844.0 ns |    207.45 ns |    108.50 ns |  2.58 |    0.15 |    5 |         - |          NA |
| SmoothSort       | 256  | Sorted             |     1,293.8 ns |     16.13 ns |      7.16 ns |  0.38 |    0.02 |    1 |         - |          NA |
| TournamentSort   | 256  | Sorted             |     4,502.3 ns |    263.69 ns |    137.92 ns |  1.31 |    0.08 |    2 |         - |          NA |
| BinomialHeapSort | 256  | Sorted             |     6,680.2 ns |    371.03 ns |    194.05 ns |  1.95 |    0.12 |    4 |         - |          NA |
| PairingHeapSort  | 256  | Sorted             |     5,393.7 ns |     20.60 ns |      9.15 ns |  1.57 |    0.09 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **Reversed**           |     **3,280.2 ns** |    **344.63 ns** |    **180.25 ns** |  **1.00** |    **0.07** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | Reversed           |     2,724.0 ns |    258.42 ns |    114.74 ns |  0.83 |    0.05 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | Reversed           |     4,389.2 ns |    118.42 ns |     52.58 ns |  1.34 |    0.07 |    2 |         - |          NA |
| BottomupHeapSort | 256  | Reversed           |     4,616.1 ns |    259.68 ns |    135.82 ns |  1.41 |    0.08 |    2 |         - |          NA |
| WeakHeapSort     | 256  | Reversed           |     9,619.4 ns |    233.25 ns |    103.56 ns |  2.94 |    0.15 |    4 |         - |          NA |
| SmoothSort       | 256  | Reversed           |     5,042.3 ns |    309.19 ns |    161.71 ns |  1.54 |    0.09 |    2 |         - |          NA |
| TournamentSort   | 256  | Reversed           |     5,151.3 ns |    349.90 ns |    155.36 ns |  1.57 |    0.09 |    2 |         - |          NA |
| BinomialHeapSort | 256  | Reversed           |     6,695.4 ns |    379.01 ns |    198.23 ns |  2.05 |    0.12 |    3 |         - |          NA |
| PairingHeapSort  | 256  | Reversed           |     2,690.2 ns |     64.12 ns |     33.53 ns |  0.82 |    0.04 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **PipeOrgan**          |     **3,154.5 ns** |    **334.84 ns** |    **175.13 ns** |  **1.00** |    **0.07** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | PipeOrgan          |     2,967.7 ns |     88.99 ns |     39.51 ns |  0.94 |    0.05 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | PipeOrgan          |     4,273.9 ns |    302.98 ns |    134.52 ns |  1.36 |    0.08 |    2 |         - |          NA |
| BottomupHeapSort | 256  | PipeOrgan          |     4,381.0 ns |     75.39 ns |     26.89 ns |  1.39 |    0.07 |    2 |         - |          NA |
| WeakHeapSort     | 256  | PipeOrgan          |     9,228.4 ns |    288.24 ns |    150.75 ns |  2.93 |    0.16 |    3 |         - |          NA |
| SmoothSort       | 256  | PipeOrgan          |     5,126.1 ns |    358.00 ns |    187.24 ns |  1.63 |    0.10 |    2 |         - |          NA |
| TournamentSort   | 256  | PipeOrgan          |     6,562.7 ns |    540.63 ns |    240.04 ns |  2.09 |    0.13 |    3 |         - |          NA |
| BinomialHeapSort | 256  | PipeOrgan          |     7,790.0 ns |    133.04 ns |     69.58 ns |  2.48 |    0.13 |    3 |         - |          NA |
| PairingHeapSort  | 256  | PipeOrgan          |     7,149.6 ns |    250.43 ns |    130.98 ns |  2.27 |    0.12 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **ManyDuplicates**     |     **3,355.0 ns** |    **219.71 ns** |    **114.91 ns** |  **1.00** |    **0.05** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | ManyDuplicates     |     3,422.2 ns |    145.70 ns |     64.69 ns |  1.02 |    0.04 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | ManyDuplicates     |     4,008.9 ns |    298.53 ns |    132.55 ns |  1.20 |    0.05 |    1 |         - |          NA |
| BottomupHeapSort | 256  | ManyDuplicates     |     4,450.9 ns |    344.14 ns |    179.99 ns |  1.33 |    0.07 |    1 |         - |          NA |
| WeakHeapSort     | 256  | ManyDuplicates     |     9,813.6 ns |    312.71 ns |    163.55 ns |  2.93 |    0.10 |    2 |         - |          NA |
| SmoothSort       | 256  | ManyDuplicates     |     5,110.3 ns |    241.95 ns |    126.54 ns |  1.52 |    0.06 |    1 |         - |          NA |
| TournamentSort   | 256  | ManyDuplicates     |     8,541.4 ns |    576.66 ns |    301.60 ns |  2.55 |    0.12 |    2 |         - |          NA |
| BinomialHeapSort | 256  | ManyDuplicates     |    13,725.0 ns |    460.99 ns |    164.39 ns |  4.09 |    0.14 |    3 |         - |          NA |
| PairingHeapSort  | 256  | ManyDuplicates     |    10,943.7 ns |    409.58 ns |    214.22 ns |  3.27 |    0.12 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Random**             |    **18,167.0 ns** |    **615.50 ns** |    **321.92 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Random             |    17,833.4 ns |    361.66 ns |    160.58 ns |  0.98 |    0.02 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | Random             |    20,085.5 ns |    645.34 ns |    286.53 ns |  1.11 |    0.02 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | Random             |    20,593.7 ns |    429.39 ns |    190.65 ns |  1.13 |    0.02 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | Random             |    53,194.2 ns |    310.92 ns |    138.05 ns |  2.93 |    0.05 |    4 |         - |          NA |
| SmoothSort       | 1024 | Random             |    27,468.3 ns |    415.11 ns |    184.31 ns |  1.51 |    0.03 |    2 |         - |          NA |
| TournamentSort   | 1024 | Random             |    42,711.3 ns |  5,179.92 ns |  2,709.20 ns |  2.35 |    0.15 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | Random             |    83,980.3 ns |  6,082.45 ns |  2,700.65 ns |  4.62 |    0.16 |    5 |         - |          NA |
| PairingHeapSort  | 1024 | Random             |    55,628.2 ns |  1,553.45 ns |    689.74 ns |  3.06 |    0.06 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **SingleElementMoved** |    **15,368.6 ns** |    **526.90 ns** |    **233.95 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | SingleElementMoved |    15,483.5 ns |    329.20 ns |    146.17 ns |  1.01 |    0.02 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | SingleElementMoved |    20,654.7 ns |    640.98 ns |    335.24 ns |  1.34 |    0.03 |    3 |         - |          NA |
| BottomupHeapSort | 1024 | SingleElementMoved |    20,119.2 ns |    322.60 ns |    143.24 ns |  1.31 |    0.02 |    3 |         - |          NA |
| WeakHeapSort     | 1024 | SingleElementMoved |    43,937.8 ns |    302.40 ns |    158.16 ns |  2.86 |    0.04 |    5 |         - |          NA |
| SmoothSort       | 1024 | SingleElementMoved |     7,567.2 ns |    206.91 ns |    108.22 ns |  0.49 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 1024 | SingleElementMoved |    27,759.5 ns |  3,047.52 ns |  1,593.91 ns |  1.81 |    0.10 |    4 |         - |          NA |
| BinomialHeapSort | 1024 | SingleElementMoved |    32,552.0 ns |    423.06 ns |    187.84 ns |  2.12 |    0.03 |    4 |         - |          NA |
| PairingHeapSort  | 1024 | SingleElementMoved |    22,169.0 ns |     51.43 ns |     18.34 ns |  1.44 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Sorted**             |    **16,726.5 ns** |    **637.11 ns** |    **333.22 ns** |  **1.00** |    **0.03** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Sorted             |    17,304.7 ns |    327.57 ns |    145.45 ns |  1.03 |    0.02 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | Sorted             |    20,281.5 ns |  2,582.22 ns |    920.85 ns |  1.21 |    0.06 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | Sorted             |    20,392.2 ns |    600.35 ns |    314.00 ns |  1.22 |    0.03 |    2 |         - |          NA |
| WeakHeapSort     | 1024 | Sorted             |    44,443.1 ns |    307.92 ns |    161.05 ns |  2.66 |    0.05 |    4 |         - |          NA |
| SmoothSort       | 1024 | Sorted             |     5,293.7 ns |    194.95 ns |    101.96 ns |  0.32 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 1024 | Sorted             |    21,193.3 ns |  1,694.84 ns |    886.44 ns |  1.27 |    0.06 |    2 |         - |          NA |
| BinomialHeapSort | 1024 | Sorted             |    29,324.4 ns |    385.46 ns |    171.15 ns |  1.75 |    0.03 |    3 |         - |          NA |
| PairingHeapSort  | 1024 | Sorted             |    22,433.4 ns |    256.45 ns |    134.13 ns |  1.34 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Reversed**           |    **15,318.4 ns** |    **444.21 ns** |    **232.33 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Reversed           |    15,460.2 ns |    471.21 ns |    246.45 ns |  1.01 |    0.02 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | Reversed           |    20,768.7 ns |    343.80 ns |    179.81 ns |  1.36 |    0.02 |    3 |         - |          NA |
| BottomupHeapSort | 1024 | Reversed           |    21,021.6 ns |    915.32 ns |    478.73 ns |  1.37 |    0.04 |    3 |         - |          NA |
| WeakHeapSort     | 1024 | Reversed           |    48,144.1 ns |     46.05 ns |     20.45 ns |  3.14 |    0.04 |    4 |         - |          NA |
| SmoothSort       | 1024 | Reversed           |    24,284.1 ns |    543.51 ns |    241.32 ns |  1.59 |    0.03 |    3 |         - |          NA |
| TournamentSort   | 1024 | Reversed           |    26,570.4 ns |  1,442.46 ns |    754.43 ns |  1.73 |    0.05 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | Reversed           |    29,141.6 ns |    360.04 ns |    159.86 ns |  1.90 |    0.03 |    3 |         - |          NA |
| PairingHeapSort  | 1024 | Reversed           |    10,752.3 ns |    292.91 ns |    153.20 ns |  0.70 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **PipeOrgan**          |    **15,063.1 ns** |    **809.57 ns** |    **423.42 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | PipeOrgan          |    15,292.8 ns |    262.02 ns |    116.34 ns |  1.02 |    0.03 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | PipeOrgan          |    20,419.4 ns |    902.24 ns |    471.89 ns |  1.36 |    0.05 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | PipeOrgan          |    20,296.2 ns |    480.18 ns |    213.20 ns |  1.35 |    0.04 |    2 |         - |          NA |
| WeakHeapSort     | 1024 | PipeOrgan          |    47,566.7 ns |    171.59 ns |     76.19 ns |  3.16 |    0.08 |    4 |         - |          NA |
| SmoothSort       | 1024 | PipeOrgan          |    26,656.5 ns |    746.00 ns |    390.17 ns |  1.77 |    0.05 |    3 |         - |          NA |
| TournamentSort   | 1024 | PipeOrgan          |    33,735.1 ns |  2,521.65 ns |  1,318.87 ns |  2.24 |    0.10 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | PipeOrgan          |    33,083.8 ns |    630.07 ns |    329.54 ns |  2.20 |    0.06 |    3 |         - |          NA |
| PairingHeapSort  | 1024 | PipeOrgan          |    29,326.1 ns |    310.99 ns |    138.08 ns |  1.95 |    0.05 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **ManyDuplicates**     |    **19,531.1 ns** |  **3,743.57 ns** |  **1,957.96 ns** |  **1.01** |    **0.13** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | ManyDuplicates     |    17,857.3 ns |    394.44 ns |    175.14 ns |  0.92 |    0.08 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | ManyDuplicates     |    19,190.3 ns |  1,472.21 ns |    769.99 ns |  0.99 |    0.10 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | ManyDuplicates     |    20,282.6 ns |    782.96 ns |    347.64 ns |  1.05 |    0.10 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | ManyDuplicates     |    49,014.5 ns |    679.10 ns |    355.18 ns |  2.53 |    0.23 |    4 |         - |          NA |
| SmoothSort       | 1024 | ManyDuplicates     |    24,522.0 ns |    617.16 ns |    274.02 ns |  1.27 |    0.11 |    2 |         - |          NA |
| TournamentSort   | 1024 | ManyDuplicates     |    39,184.8 ns |  2,845.17 ns |  1,488.08 ns |  2.02 |    0.20 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | ManyDuplicates     |    68,050.6 ns |  3,503.67 ns |  1,832.49 ns |  3.51 |    0.33 |    5 |         - |          NA |
| PairingHeapSort  | 1024 | ManyDuplicates     |    52,770.1 ns |  1,163.51 ns |    516.61 ns |  2.72 |    0.25 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Random**             |   **184,539.0 ns** |  **3,164.17 ns** |  **1,654.92 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Random             |   191,009.0 ns |  2,088.72 ns |  1,092.44 ns |  1.04 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 4096 | Random             |   138,014.7 ns | 25,323.29 ns | 13,244.57 ns |  0.75 |    0.07 |    1 |         - |          NA |
| BottomupHeapSort | 4096 | Random             |   140,511.7 ns |  6,621.99 ns |  2,940.21 ns |  0.76 |    0.02 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | Random             |   356,218.3 ns | 29,382.06 ns | 15,367.39 ns |  1.93 |    0.08 |    3 |         - |          NA |
| SmoothSort       | 4096 | Random             |   389,611.4 ns |  2,543.37 ns |  1,330.23 ns |  2.11 |    0.02 |    3 |         - |          NA |
| TournamentSort   | 4096 | Random             |   670,766.1 ns |  8,272.70 ns |  4,326.78 ns |  3.64 |    0.04 |    4 |         - |          NA |
| BinomialHeapSort | 4096 | Random             | 1,046,171.4 ns |  9,440.81 ns |  4,191.78 ns |  5.67 |    0.05 |    5 |         - |          NA |
| PairingHeapSort  | 4096 | Random             |   461,364.2 ns |  9,266.93 ns |  4,846.78 ns |  2.50 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **SingleElementMoved** |   **104,641.5 ns** |  **1,694.23 ns** |    **886.11 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 4096 | SingleElementMoved |   141,199.0 ns |  5,692.62 ns |  2,977.35 ns |  1.35 |    0.03 |    3 |         - |          NA |
| TernaryHeapSort  | 4096 | SingleElementMoved |   101,792.5 ns |  1,321.99 ns |    586.97 ns |  0.97 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 4096 | SingleElementMoved |   106,396.6 ns |    333.57 ns |    148.11 ns |  1.02 |    0.01 |    2 |         - |          NA |
| WeakHeapSort     | 4096 | SingleElementMoved |   214,208.0 ns |    855.81 ns |    447.60 ns |  2.05 |    0.02 |    4 |         - |          NA |
| SmoothSort       | 4096 | SingleElementMoved |    29,067.0 ns |    526.70 ns |    275.47 ns |  0.28 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 4096 | SingleElementMoved |   308,501.5 ns | 17,620.22 ns |  7,823.49 ns |  2.95 |    0.07 |    5 |         - |          NA |
| BinomialHeapSort | 4096 | SingleElementMoved |   142,969.2 ns |  1,682.91 ns |    747.22 ns |  1.37 |    0.01 |    3 |         - |          NA |
| PairingHeapSort  | 4096 | SingleElementMoved |    91,833.5 ns |    263.99 ns |    138.07 ns |  0.88 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Sorted**             |   **126,112.7 ns** |  **1,470.16 ns** |    **768.92 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Sorted             |   156,413.9 ns |  1,758.51 ns |    919.74 ns |  1.24 |    0.01 |    3 |         - |          NA |
| TernaryHeapSort  | 4096 | Sorted             |    98,553.1 ns |  6,555.88 ns |  3,428.85 ns |  0.78 |    0.03 |    2 |         - |          NA |
| BottomupHeapSort | 4096 | Sorted             |   100,614.7 ns |  2,181.83 ns |    968.74 ns |  0.80 |    0.01 |    2 |         - |          NA |
| WeakHeapSort     | 4096 | Sorted             |   216,003.0 ns |  1,050.94 ns |    549.66 ns |  1.71 |    0.01 |    4 |         - |          NA |
| SmoothSort       | 4096 | Sorted             |    21,242.5 ns |    485.11 ns |    215.39 ns |  0.17 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 4096 | Sorted             |   160,376.4 ns | 28,517.51 ns | 14,915.21 ns |  1.27 |    0.11 |    3 |         - |          NA |
| BinomialHeapSort | 4096 | Sorted             |   131,061.8 ns |    558.18 ns |    291.94 ns |  1.04 |    0.01 |    3 |         - |          NA |
| PairingHeapSort  | 4096 | Sorted             |    92,343.8 ns |  1,157.35 ns |    605.32 ns |  0.73 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Reversed**           |   **115,632.8 ns** |  **4,404.53 ns** |  **1,570.70 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Reversed           |   131,837.3 ns |  3,299.64 ns |  1,465.06 ns |  1.14 |    0.02 |    2 |         - |          NA |
| TernaryHeapSort  | 4096 | Reversed           |    98,173.6 ns |    428.87 ns |    190.42 ns |  0.85 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 4096 | Reversed           |   103,678.3 ns |  1,550.78 ns |    688.56 ns |  0.90 |    0.01 |    2 |         - |          NA |
| WeakHeapSort     | 4096 | Reversed           |   232,238.8 ns |    741.44 ns |    387.79 ns |  2.01 |    0.03 |    3 |         - |          NA |
| SmoothSort       | 4096 | Reversed           |   134,374.6 ns |  3,045.50 ns |  1,352.22 ns |  1.16 |    0.02 |    2 |         - |          NA |
| TournamentSort   | 4096 | Reversed           |   233,351.3 ns | 16,270.52 ns |  7,224.21 ns |  2.02 |    0.06 |    3 |         - |          NA |
| BinomialHeapSort | 4096 | Reversed           |   127,884.1 ns |    892.87 ns |    396.44 ns |  1.11 |    0.01 |    2 |         - |          NA |
| PairingHeapSort  | 4096 | Reversed           |    42,580.7 ns |    752.57 ns |    334.15 ns |  0.37 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **PipeOrgan**          |   **107,292.7 ns** |  **2,370.92 ns** |  **1,052.70 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 4096 | PipeOrgan          |   121,208.9 ns |  6,415.75 ns |  2,848.63 ns |  1.13 |    0.03 |    1 |         - |          NA |
| TernaryHeapSort  | 4096 | PipeOrgan          |    98,514.8 ns |  1,071.05 ns |    381.94 ns |  0.92 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 4096 | PipeOrgan          |   102,104.4 ns |  1,089.77 ns |    483.86 ns |  0.95 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | PipeOrgan          |   232,877.9 ns |    553.11 ns |    289.29 ns |  2.17 |    0.02 |    2 |         - |          NA |
| SmoothSort       | 4096 | PipeOrgan          |   284,784.8 ns | 10,492.45 ns |  4,658.71 ns |  2.65 |    0.05 |    3 |         - |          NA |
| TournamentSort   | 4096 | PipeOrgan          |   458,269.5 ns | 12,887.95 ns |  6,740.65 ns |  4.27 |    0.07 |    4 |         - |          NA |
| BinomialHeapSort | 4096 | PipeOrgan          |   142,482.2 ns |    322.45 ns |    143.17 ns |  1.33 |    0.01 |    1 |         - |          NA |
| PairingHeapSort  | 4096 | PipeOrgan          |   121,455.2 ns |    537.60 ns |    281.18 ns |  1.13 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **ManyDuplicates**     |   **173,374.3 ns** |  **2,061.13 ns** |    **915.16 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 4096 | ManyDuplicates     |   177,156.4 ns |  2,572.99 ns |  1,345.72 ns |  1.02 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 4096 | ManyDuplicates     |   100,650.2 ns |  1,580.29 ns |    701.66 ns |  0.58 |    0.00 |    1 |         - |          NA |
| BottomupHeapSort | 4096 | ManyDuplicates     |   113,297.5 ns |  2,337.18 ns |  1,037.72 ns |  0.65 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | ManyDuplicates     |   236,321.4 ns |    426.91 ns |    189.55 ns |  1.36 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 4096 | ManyDuplicates     |   323,460.4 ns |  5,930.72 ns |  3,101.88 ns |  1.87 |    0.02 |    4 |         - |          NA |
| TournamentSort   | 4096 | ManyDuplicates     |   610,926.5 ns |  3,720.48 ns |  1,326.76 ns |  3.52 |    0.02 |    6 |         - |          NA |
| BinomialHeapSort | 4096 | ManyDuplicates     |   721,148.2 ns |  8,432.91 ns |  4,410.58 ns |  4.16 |    0.03 |    6 |         - |          NA |
| PairingHeapSort  | 4096 | ManyDuplicates     |   410,583.0 ns |  4,528.76 ns |  2,010.80 ns |  2.37 |    0.02 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Random**             |   **535,052.1 ns** |  **6,316.48 ns** |  **3,303.64 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Random             |   523,887.4 ns |  4,467.01 ns |  1,983.38 ns |  0.98 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | Random             |   655,324.8 ns |  8,790.44 ns |  4,597.57 ns |  1.22 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | Random             |   659,752.6 ns |  2,330.70 ns |  1,219.00 ns |  1.23 |    0.01 |    2 |         - |          NA |
| WeakHeapSort     | 8192 | Random             |   979,103.8 ns |  1,390.84 ns |    617.54 ns |  1.83 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 8192 | Random             |   935,085.3 ns |  1,593.58 ns |    707.56 ns |  1.75 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 8192 | Random             | 1,499,504.3 ns | 13,381.73 ns |  6,998.90 ns |  2.80 |    0.02 |    4 |         - |          NA |
| BinomialHeapSort | 8192 | Random             | 2,326,561.9 ns | 15,306.11 ns |  8,005.39 ns |  4.35 |    0.03 |    5 |         - |          NA |
| PairingHeapSort  | 8192 | Random             | 1,116,328.4 ns |  2,794.40 ns |  1,461.53 ns |  2.09 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **SingleElementMoved** |   **366,314.8 ns** |  **3,858.76 ns** |  **2,018.20 ns** |  **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 8192 | SingleElementMoved |   394,444.9 ns |  6,334.86 ns |  2,812.72 ns |  1.08 |    0.01 |    4 |         - |          NA |
| TernaryHeapSort  | 8192 | SingleElementMoved |   397,799.1 ns |  1,140.85 ns |    506.55 ns |  1.09 |    0.01 |    4 |         - |          NA |
| BottomupHeapSort | 8192 | SingleElementMoved |   414,234.3 ns |  1,792.19 ns |    937.35 ns |  1.13 |    0.01 |    4 |         - |          NA |
| WeakHeapSort     | 8192 | SingleElementMoved |   467,279.5 ns |  1,111.81 ns |    493.65 ns |  1.28 |    0.01 |    4 |         - |          NA |
| SmoothSort       | 8192 | SingleElementMoved |    58,673.7 ns |    608.95 ns |    318.49 ns |  0.16 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 8192 | SingleElementMoved |   795,363.0 ns | 11,448.61 ns |  5,987.85 ns |  2.17 |    0.02 |    5 |         - |          NA |
| BinomialHeapSort | 8192 | SingleElementMoved |   296,113.8 ns |    639.18 ns |    283.80 ns |  0.81 |    0.00 |    3 |         - |          NA |
| PairingHeapSort  | 8192 | SingleElementMoved |   180,385.6 ns |    569.82 ns |    253.00 ns |  0.49 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Sorted**             |   **331,032.0 ns** |  **3,632.09 ns** |  **1,899.65 ns** |  **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Sorted             |   400,937.5 ns |  2,904.30 ns |  1,519.00 ns |  1.21 |    0.01 |    4 |         - |          NA |
| TernaryHeapSort  | 8192 | Sorted             |   353,729.0 ns |  9,986.31 ns |  4,433.98 ns |  1.07 |    0.01 |    4 |         - |          NA |
| BottomupHeapSort | 8192 | Sorted             |   408,734.3 ns |  1,922.99 ns |  1,005.76 ns |  1.23 |    0.01 |    4 |         - |          NA |
| WeakHeapSort     | 8192 | Sorted             |   471,879.0 ns |  1,636.60 ns |    855.97 ns |  1.43 |    0.01 |    4 |         - |          NA |
| SmoothSort       | 8192 | Sorted             |    42,035.3 ns |    384.34 ns |    170.65 ns |  0.13 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 8192 | Sorted             |   503,140.2 ns | 25,251.49 ns | 13,207.02 ns |  1.52 |    0.04 |    4 |         - |          NA |
| BinomialHeapSort | 8192 | Sorted             |   274,062.6 ns |  1,657.99 ns |    867.16 ns |  0.83 |    0.01 |    3 |         - |          NA |
| PairingHeapSort  | 8192 | Sorted             |   181,625.0 ns |  1,492.86 ns |    662.84 ns |  0.55 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Reversed**           |   **376,107.3 ns** | **16,215.54 ns** |  **8,481.04 ns** |  **1.00** |    **0.03** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Reversed           |   347,231.2 ns |  4,358.09 ns |  1,935.02 ns |  0.92 |    0.02 |    3 |         - |          NA |
| TernaryHeapSort  | 8192 | Reversed           |   412,767.4 ns |  1,555.07 ns |    690.46 ns |  1.10 |    0.02 |    3 |         - |          NA |
| BottomupHeapSort | 8192 | Reversed           |   480,716.5 ns |  5,432.67 ns |  2,841.39 ns |  1.28 |    0.03 |    3 |         - |          NA |
| WeakHeapSort     | 8192 | Reversed           |   504,017.5 ns |  1,100.92 ns |    488.82 ns |  1.34 |    0.03 |    3 |         - |          NA |
| SmoothSort       | 8192 | Reversed           |   574,036.5 ns |  1,455.39 ns |    761.20 ns |  1.53 |    0.03 |    3 |         - |          NA |
| TournamentSort   | 8192 | Reversed           |   688,905.5 ns |  8,562.95 ns |  4,478.59 ns |  1.83 |    0.04 |    4 |         - |          NA |
| BinomialHeapSort | 8192 | Reversed           |   268,895.7 ns |  2,012.35 ns |    893.50 ns |  0.72 |    0.02 |    2 |         - |          NA |
| PairingHeapSort  | 8192 | Reversed           |    84,797.6 ns |    856.53 ns |    380.30 ns |  0.23 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **PipeOrgan**          |   **379,618.1 ns** | **16,109.58 ns** |  **8,425.62 ns** |  **1.00** |    **0.03** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 8192 | PipeOrgan          |   405,585.5 ns |  1,956.79 ns |  1,023.44 ns |  1.07 |    0.02 |    3 |         - |          NA |
| TernaryHeapSort  | 8192 | PipeOrgan          |   465,360.5 ns |  1,601.56 ns |    711.10 ns |  1.23 |    0.03 |    3 |         - |          NA |
| BottomupHeapSort | 8192 | PipeOrgan          |   475,243.0 ns |  1,402.40 ns |    733.48 ns |  1.25 |    0.03 |    3 |         - |          NA |
| WeakHeapSort     | 8192 | PipeOrgan          |   509,527.5 ns |  2,161.94 ns |  1,130.74 ns |  1.34 |    0.03 |    3 |         - |          NA |
| SmoothSort       | 8192 | PipeOrgan          |   710,294.1 ns |  2,008.47 ns |  1,050.47 ns |  1.87 |    0.04 |    4 |         - |          NA |
| TournamentSort   | 8192 | PipeOrgan          | 1,122,454.0 ns |  7,120.19 ns |  3,723.99 ns |  2.96 |    0.06 |    5 |         - |          NA |
| BinomialHeapSort | 8192 | PipeOrgan          |   298,634.3 ns |  2,652.38 ns |  1,387.25 ns |  0.79 |    0.02 |    2 |         - |          NA |
| PairingHeapSort  | 8192 | PipeOrgan          |   244,775.7 ns |    917.58 ns |    407.41 ns |  0.65 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **ManyDuplicates**     |   **503,601.2 ns** |  **3,048.16 ns** |  **1,594.25 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | ManyDuplicates     |   508,767.4 ns |  4,666.86 ns |  2,072.11 ns |  1.01 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | ManyDuplicates     |   591,855.4 ns |  4,783.04 ns |  2,501.62 ns |  1.18 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 8192 | ManyDuplicates     |   608,550.0 ns |  2,941.20 ns |  1,305.91 ns |  1.21 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | ManyDuplicates     |   675,683.6 ns |  1,069.04 ns |    474.66 ns |  1.34 |    0.00 |    1 |         - |          NA |
| SmoothSort       | 8192 | ManyDuplicates     |   792,299.5 ns |  2,351.23 ns |  1,229.74 ns |  1.57 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 8192 | ManyDuplicates     | 1,386,821.0 ns |  5,897.14 ns |  3,084.32 ns |  2.75 |    0.01 |    3 |         - |          NA |
| BinomialHeapSort | 8192 | ManyDuplicates     | 1,554,298.7 ns |  6,281.84 ns |  2,789.17 ns |  3.09 |    0.01 |    3 |         - |          NA |
| PairingHeapSort  | 8192 | ManyDuplicates     |   957,384.0 ns |  3,379.40 ns |  1,767.49 ns |  1.90 |    0.01 |    2 |         - |          NA |

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

| Method                 | Size | Pattern            | Mean         | Error        | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------- |----- |------------------- |-------------:|-------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **InsertionSort**          | **256**  | **Random**             |   **7,019.1 ns** |    **176.35 ns** |    **78.30 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Random             |   7,368.5 ns |    301.81 ns |   157.85 ns |  1.05 |    0.02 |    3 |         - |          NA |
| BinaryInsertSort       | 256  | Random             |   5,617.2 ns |    379.61 ns |   198.54 ns |  0.80 |    0.03 |    2 |         - |          NA |
| GnomeSort              | 256  | Random             |  24,090.5 ns |    281.34 ns |   147.15 ns |  3.43 |    0.04 |    6 |         - |          NA |
| LibrarySort            | 256  | Random             |  16,207.9 ns |    326.66 ns |   170.85 ns |  2.31 |    0.03 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | Random             |  13,382.9 ns |    470.02 ns |   208.69 ns |  1.91 |    0.03 |    4 |         - |          NA |
| ShellSortKnuth1973     | 256  | Random             |   2,780.7 ns |    332.18 ns |   173.73 ns |  0.40 |    0.02 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Random             |   2,721.5 ns |     92.91 ns |    48.60 ns |  0.39 |    0.01 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | Random             |   2,823.5 ns |    306.07 ns |   160.08 ns |  0.40 |    0.02 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | Random             |   2,640.2 ns |    211.89 ns |    94.08 ns |  0.38 |    0.01 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Random             |   2,750.3 ns |     47.33 ns |    24.76 ns |  0.39 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **256**  | **SingleElementMoved** |     **441.5 ns** |      **3.77 ns** |     **1.67 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 256  | SingleElementMoved |     375.8 ns |    145.56 ns |    76.13 ns |  0.85 |    0.16 |    1 |         - |          NA |
| BinaryInsertSort       | 256  | SingleElementMoved |   1,137.1 ns |      6.01 ns |     2.67 ns |  2.58 |    0.01 |    4 |         - |          NA |
| GnomeSort              | 256  | SingleElementMoved |     683.7 ns |     17.47 ns |     9.14 ns |  1.55 |    0.02 |    3 |         - |          NA |
| LibrarySort            | 256  | SingleElementMoved |  15,433.0 ns |    294.64 ns |   154.10 ns | 34.96 |    0.35 |    6 |         - |          NA |
| MergeInsertionSort     | 256  | SingleElementMoved |  12,257.2 ns |    305.73 ns |   109.03 ns | 27.76 |    0.25 |    5 |         - |          NA |
| ShellSortKnuth1973     | 256  | SingleElementMoved |   1,331.7 ns |      5.83 ns |     2.59 ns |  3.02 |    0.01 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 256  | SingleElementMoved |   1,326.2 ns |      6.67 ns |     2.96 ns |  3.00 |    0.01 |    4 |         - |          NA |
| ShellSortTokuda1992    | 256  | SingleElementMoved |   1,614.3 ns |     40.93 ns |    18.17 ns |  3.66 |    0.04 |    4 |         - |          NA |
| ShellSortCiura2001     | 256  | SingleElementMoved |   1,411.0 ns |     46.12 ns |    24.12 ns |  3.20 |    0.05 |    4 |         - |          NA |
| ShellSortLee2021       | 256  | SingleElementMoved |   1,560.8 ns |     75.80 ns |    33.66 ns |  3.54 |    0.07 |    4 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **256**  | **Sorted**             |     **365.2 ns** |      **1.54 ns** |     **0.69 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Sorted             |     279.8 ns |      1.75 ns |     0.78 ns |  0.77 |    0.00 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | Sorted             |     191.9 ns |      2.53 ns |     1.12 ns |  0.53 |    0.00 |    1 |         - |          NA |
| GnomeSort              | 256  | Sorted             |     188.6 ns |      2.50 ns |     0.89 ns |  0.52 |    0.00 |    1 |         - |          NA |
| LibrarySort            | 256  | Sorted             |  15,729.2 ns |     51.45 ns |    22.84 ns | 43.07 |    0.10 |    6 |         - |          NA |
| MergeInsertionSort     | 256  | Sorted             |  12,349.3 ns |    317.03 ns |   165.81 ns | 33.81 |    0.43 |    5 |         - |          NA |
| ShellSortKnuth1973     | 256  | Sorted             |   1,193.3 ns |      3.13 ns |     1.39 ns |  3.27 |    0.01 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Sorted             |   1,193.9 ns |      3.86 ns |     1.71 ns |  3.27 |    0.01 |    4 |         - |          NA |
| ShellSortTokuda1992    | 256  | Sorted             |   1,463.6 ns |      0.71 ns |     0.32 ns |  4.01 |    0.01 |    4 |         - |          NA |
| ShellSortCiura2001     | 256  | Sorted             |   1,281.3 ns |      2.52 ns |     1.32 ns |  3.51 |    0.01 |    4 |         - |          NA |
| ShellSortLee2021       | 256  | Sorted             |   1,466.1 ns |      2.36 ns |     1.05 ns |  4.01 |    0.01 |    4 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **256**  | **Reversed**           |  **15,557.8 ns** |    **302.75 ns** |   **134.42 ns** |  **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Reversed           |  20,280.2 ns |    308.74 ns |   161.48 ns |  1.30 |    0.01 |    5 |         - |          NA |
| BinaryInsertSort       | 256  | Reversed           |   6,691.3 ns |    229.51 ns |   120.04 ns |  0.43 |    0.01 |    2 |         - |          NA |
| GnomeSort              | 256  | Reversed           |  67,931.5 ns |  1,365.16 ns |   714.00 ns |  4.37 |    0.06 |    6 |         - |          NA |
| LibrarySort            | 256  | Reversed           |  21,958.1 ns |    223.79 ns |   117.04 ns |  1.41 |    0.01 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | Reversed           |  12,312.2 ns |    248.86 ns |   130.16 ns |  0.79 |    0.01 |    3 |         - |          NA |
| ShellSortKnuth1973     | 256  | Reversed           |   1,912.3 ns |     36.69 ns |    16.29 ns |  0.12 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Reversed           |   2,041.8 ns |    258.21 ns |   135.05 ns |  0.13 |    0.01 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | Reversed           |   2,084.6 ns |     32.68 ns |    14.51 ns |  0.13 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | Reversed           |   1,968.2 ns |     16.06 ns |     7.13 ns |  0.13 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Reversed           |   2,264.1 ns |    426.90 ns |   152.24 ns |  0.15 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **256**  | **PipeOrgan**          |   **7,946.6 ns** |     **57.91 ns** |    **25.71 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 256  | PipeOrgan          |  10,334.6 ns |    280.37 ns |   146.64 ns |  1.30 |    0.02 |    4 |         - |          NA |
| BinaryInsertSort       | 256  | PipeOrgan          |   3,837.6 ns |    225.61 ns |   118.00 ns |  0.48 |    0.01 |    2 |         - |          NA |
| GnomeSort              | 256  | PipeOrgan          |  33,063.6 ns |    388.03 ns |   202.95 ns |  4.16 |    0.03 |    6 |         - |          NA |
| LibrarySort            | 256  | PipeOrgan          |  14,326.2 ns |    263.60 ns |   137.87 ns |  1.80 |    0.02 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | PipeOrgan          |  12,588.7 ns |    359.22 ns |   159.49 ns |  1.58 |    0.02 |    5 |         - |          NA |
| ShellSortKnuth1973     | 256  | PipeOrgan          |   1,813.7 ns |     31.88 ns |    14.16 ns |  0.23 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | PipeOrgan          |   1,854.5 ns |     24.52 ns |    10.89 ns |  0.23 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | PipeOrgan          |   2,137.3 ns |     19.38 ns |     6.91 ns |  0.27 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | PipeOrgan          |   2,006.1 ns |     15.13 ns |     6.72 ns |  0.25 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | PipeOrgan          |   2,164.9 ns |    120.92 ns |    53.69 ns |  0.27 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **256**  | **ManyDuplicates**     |   **6,701.6 ns** |     **19.04 ns** |     **6.79 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 256  | ManyDuplicates     |   7,346.0 ns |     49.11 ns |    25.69 ns |  1.10 |    0.00 |    3 |         - |          NA |
| BinaryInsertSort       | 256  | ManyDuplicates     |   5,341.2 ns |    274.71 ns |   143.68 ns |  0.80 |    0.02 |    2 |         - |          NA |
| GnomeSort              | 256  | ManyDuplicates     |  23,121.9 ns |    432.47 ns |   192.02 ns |  3.45 |    0.03 |    6 |         - |          NA |
| LibrarySort            | 256  | ManyDuplicates     |  16,049.3 ns |    206.50 ns |   108.00 ns |  2.39 |    0.02 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | ManyDuplicates     |  13,196.1 ns |    118.82 ns |    52.76 ns |  1.97 |    0.01 |    4 |         - |          NA |
| ShellSortKnuth1973     | 256  | ManyDuplicates     |   2,270.0 ns |     51.72 ns |    22.96 ns |  0.34 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | ManyDuplicates     |   2,226.1 ns |     16.25 ns |     5.80 ns |  0.33 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | ManyDuplicates     |   2,192.7 ns |     50.08 ns |    22.23 ns |  0.33 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | ManyDuplicates     |   2,205.4 ns |     88.60 ns |    39.34 ns |  0.33 |    0.01 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | ManyDuplicates     |   2,181.2 ns |    205.18 ns |   107.31 ns |  0.33 |    0.02 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **1024** | **Random**             | **117,283.0 ns** |    **711.14 ns** |   **371.94 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Random             | 135,036.5 ns |  2,806.58 ns | 1,467.89 ns |  1.15 |    0.01 |    5 |         - |          NA |
| BinaryInsertSort       | 1024 | Random             |  37,146.5 ns |  1,237.64 ns |   549.52 ns |  0.32 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | Random             | 386,275.5 ns |  3,381.06 ns | 1,501.21 ns |  3.29 |    0.02 |    6 |         - |          NA |
| LibrarySort            | 1024 | Random             |  72,082.3 ns |    830.83 ns |   434.54 ns |  0.61 |    0.00 |    3 |         - |          NA |
| MergeInsertionSort     | 1024 | Random             |  93,197.3 ns |  1,032.44 ns |   458.41 ns |  0.79 |    0.00 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Random             |  14,809.1 ns |    149.00 ns |    66.16 ns |  0.13 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Random             |  14,550.6 ns |    446.46 ns |   198.23 ns |  0.12 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Random             |  14,473.0 ns |    164.51 ns |    58.67 ns |  0.12 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Random             |  14,130.8 ns |    298.40 ns |   132.49 ns |  0.12 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Random             |  14,296.3 ns |    117.76 ns |    52.29 ns |  0.12 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **1024** | **SingleElementMoved** |   **1,859.1 ns** |     **10.88 ns** |     **4.83 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | SingleElementMoved |   1,179.0 ns |      8.42 ns |     3.74 ns |  0.63 |    0.00 |    1 |         - |          NA |
| BinaryInsertSort       | 1024 | SingleElementMoved |   5,987.5 ns |    335.69 ns |   175.57 ns |  3.22 |    0.09 |    3 |         - |          NA |
| GnomeSort              | 1024 | SingleElementMoved |   2,031.9 ns |      3.06 ns |     1.09 ns |  1.09 |    0.00 |    2 |         - |          NA |
| LibrarySort            | 1024 | SingleElementMoved |  77,688.0 ns |    280.11 ns |   146.50 ns | 41.79 |    0.13 |    4 |         - |          NA |
| MergeInsertionSort     | 1024 | SingleElementMoved |  75,592.6 ns |    253.70 ns |   132.69 ns | 40.66 |    0.12 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | SingleElementMoved |   6,649.7 ns |    251.36 ns |   111.60 ns |  3.58 |    0.06 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | SingleElementMoved |   7,290.1 ns |     20.01 ns |     8.88 ns |  3.92 |    0.01 |    3 |         - |          NA |
| ShellSortTokuda1992    | 1024 | SingleElementMoved |   7,851.5 ns |      7.20 ns |     2.57 ns |  4.22 |    0.01 |    3 |         - |          NA |
| ShellSortCiura2001     | 1024 | SingleElementMoved |   8,036.9 ns |    425.85 ns |   222.73 ns |  4.32 |    0.11 |    3 |         - |          NA |
| ShellSortLee2021       | 1024 | SingleElementMoved |   8,110.8 ns |    342.39 ns |   122.10 ns |  4.36 |    0.06 |    3 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **1024** | **Sorted**             |   **1,434.4 ns** |      **1.40 ns** |     **0.62 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Sorted             |   1,082.9 ns |      7.68 ns |     2.74 ns |  0.75 |    0.00 |    2 |         - |          NA |
| BinaryInsertSort       | 1024 | Sorted             |     728.8 ns |      1.07 ns |     0.48 ns |  0.51 |    0.00 |    1 |         - |          NA |
| GnomeSort              | 1024 | Sorted             |     765.0 ns |     77.14 ns |    40.34 ns |  0.53 |    0.03 |    1 |         - |          NA |
| LibrarySort            | 1024 | Sorted             |  79,059.6 ns |    197.43 ns |   103.26 ns | 55.12 |    0.07 |    5 |         - |          NA |
| MergeInsertionSort     | 1024 | Sorted             |  74,533.1 ns |    409.96 ns |   214.42 ns | 51.96 |    0.14 |    5 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Sorted             |   6,105.4 ns |    363.87 ns |   190.31 ns |  4.26 |    0.13 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Sorted             |   6,794.8 ns |    288.58 ns |   150.93 ns |  4.74 |    0.10 |    4 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Sorted             |   7,144.9 ns |     23.47 ns |    10.42 ns |  4.98 |    0.01 |    4 |         - |          NA |
| ShellSortCiura2001     | 1024 | Sorted             |   7,350.9 ns |  1,011.22 ns |   448.99 ns |  5.12 |    0.29 |    4 |         - |          NA |
| ShellSortLee2021       | 1024 | Sorted             |   7,236.4 ns |    202.60 ns |   105.96 ns |  5.04 |    0.07 |    4 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **1024** | **Reversed**           | **229,078.4 ns** |    **294.69 ns** |   **130.84 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Reversed           | 314,532.1 ns |    382.38 ns |   169.78 ns |  1.37 |    0.00 |    5 |         - |          NA |
| BinaryInsertSort       | 1024 | Reversed           |  44,804.5 ns |    310.79 ns |   110.83 ns |  0.20 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | Reversed           | 969,290.2 ns |  5,820.57 ns | 3,044.27 ns |  4.23 |    0.01 |    6 |         - |          NA |
| LibrarySort            | 1024 | Reversed           | 193,982.2 ns |    269.53 ns |   140.97 ns |  0.85 |    0.00 |    4 |         - |          NA |
| MergeInsertionSort     | 1024 | Reversed           |  76,298.5 ns |    554.09 ns |   289.80 ns |  0.33 |    0.00 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Reversed           |   9,336.5 ns |    500.95 ns |   222.42 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Reversed           |   9,484.0 ns |    302.32 ns |   158.12 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Reversed           |  10,448.1 ns |    325.23 ns |   170.10 ns |  0.05 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Reversed           |   9,968.6 ns |    283.01 ns |   148.02 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Reversed           |  10,409.7 ns |    241.13 ns |   126.12 ns |  0.05 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **1024** | **PipeOrgan**          | **116,303.1 ns** |    **636.98 ns** |   **333.15 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | PipeOrgan          | 158,719.6 ns |  2,638.83 ns | 1,171.66 ns |  1.36 |    0.01 |    5 |         - |          NA |
| BinaryInsertSort       | 1024 | PipeOrgan          |  25,046.1 ns |    733.00 ns |   383.37 ns |  0.22 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | PipeOrgan          | 485,855.2 ns | 14,699.90 ns | 7,688.33 ns |  4.18 |    0.06 |    6 |         - |          NA |
| LibrarySort            | 1024 | PipeOrgan          |  71,833.9 ns |    635.59 ns |   282.20 ns |  0.62 |    0.00 |    3 |         - |          NA |
| MergeInsertionSort     | 1024 | PipeOrgan          |  77,028.9 ns |    593.92 ns |   310.63 ns |  0.66 |    0.00 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | PipeOrgan          |   9,118.3 ns |    348.69 ns |   124.35 ns |  0.08 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | PipeOrgan          |   9,766.9 ns |    383.67 ns |   200.67 ns |  0.08 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | PipeOrgan          |  10,859.7 ns |    254.03 ns |   112.79 ns |  0.09 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | PipeOrgan          |  11,228.1 ns |  1,756.45 ns |   918.66 ns |  0.10 |    0.01 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | PipeOrgan          |  10,829.6 ns |    292.00 ns |   152.72 ns |  0.09 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **1024** | **ManyDuplicates**     | **113,869.8 ns** |    **286.63 ns** |   **127.27 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | ManyDuplicates     | 131,866.7 ns |  3,331.96 ns | 1,742.68 ns |  1.16 |    0.01 |    5 |         - |          NA |
| BinaryInsertSort       | 1024 | ManyDuplicates     |  35,837.5 ns |    841.65 ns |   440.20 ns |  0.31 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | ManyDuplicates     | 375,962.3 ns |  1,938.41 ns | 1,013.82 ns |  3.30 |    0.01 |    6 |         - |          NA |
| LibrarySort            | 1024 | ManyDuplicates     |  73,396.9 ns |  1,203.95 ns |   534.56 ns |  0.64 |    0.00 |    3 |         - |          NA |
| MergeInsertionSort     | 1024 | ManyDuplicates     |  94,343.8 ns |  3,196.56 ns | 1,419.29 ns |  0.83 |    0.01 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | ManyDuplicates     |  11,525.1 ns |    471.48 ns |   246.60 ns |  0.10 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | ManyDuplicates     |  11,180.7 ns |  1,407.57 ns |   736.19 ns |  0.10 |    0.01 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | ManyDuplicates     |  10,967.7 ns |    464.21 ns |   206.11 ns |  0.10 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | ManyDuplicates     |  10,890.8 ns |    661.41 ns |   345.93 ns |  0.10 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | ManyDuplicates     |  11,072.7 ns |    490.80 ns |   175.02 ns |  0.10 |    0.00 |    1 |         - |          NA |

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

| Method                   | Size | Pattern            | Mean           | Error        | StdDev       | Median         | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------------- |----- |------------------- |---------------:|-------------:|-------------:|---------------:|------:|--------:|-----:|----------:|------------:|
| **MergeSort**                | **256**  | **Random**             |     **8,754.7 ns** |    **461.75 ns** |    **241.50 ns** |     **8,594.7 ns** |  **1.00** |    **0.04** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Random             |     8,800.7 ns |    273.26 ns |    142.92 ns |     8,764.8 ns |  1.01 |    0.03 |    4 |         - |          NA |
| BottomupMergeSort        | 256  | Random             |     4,703.2 ns |    189.12 ns |     83.97 ns |     4,665.1 ns |  0.54 |    0.02 |    2 |         - |          NA |
| StdStableSort            | 256  | Random             |     2,735.9 ns |    409.52 ns |    181.83 ns |     2,665.8 ns |  0.31 |    0.02 |    1 |         - |          NA |
| RotateMergeSort          | 256  | Random             |    10,172.4 ns |    394.27 ns |    206.21 ns |    10,228.3 ns |  1.16 |    0.04 |    4 |         - |          NA |
| RotateMergeSortRecursive | 256  | Random             |    12,814.4 ns |    653.62 ns |    290.21 ns |    12,850.2 ns |  1.46 |    0.05 |    5 |         - |          NA |
| SymMergeSort             | 256  | Random             |     6,920.6 ns |    275.70 ns |    122.41 ns |     6,857.6 ns |  0.79 |    0.02 |    3 |         - |          NA |
| BlockMergeSort           | 256  | Random             |     5,143.7 ns |    363.57 ns |    190.16 ns |     5,037.4 ns |  0.59 |    0.03 |    2 |         - |          NA |
| NaturalMergeSort         | 256  | Random             |     5,236.0 ns |    413.17 ns |    216.10 ns |     5,248.2 ns |  0.60 |    0.03 |    2 |         - |          NA |
| TimSort                  | 256  | Random             |     4,181.3 ns |    311.56 ns |    162.95 ns |     4,118.9 ns |  0.48 |    0.02 |    2 |         - |          NA |
| PowerSort                | 256  | Random             |     2,322.8 ns |     81.17 ns |     36.04 ns |     2,306.1 ns |  0.27 |    0.01 |    1 |         - |          NA |
| ShiftSort                | 256  | Random             |     4,003.8 ns |    541.34 ns |    283.13 ns |     3,829.8 ns |  0.46 |    0.03 |    2 |         - |          NA |
| SpinSort                 | 256  | Random             |     2,290.4 ns |    141.14 ns |     62.66 ns |     2,273.2 ns |  0.26 |    0.01 |    1 |         - |          NA |
| Glidesort                | 256  | Random             |     3,616.6 ns |     88.37 ns |     31.51 ns |     3,615.7 ns |  0.41 |    0.01 |    2 |         - |          NA |
| Driftsort                | 256  | Random             |     4,628.6 ns |    380.94 ns |    199.24 ns |     4,546.0 ns |  0.53 |    0.03 |    2 |         - |          NA |
| FlatStableSort           | 256  | Random             |     2,805.7 ns |    448.77 ns |    199.26 ns |     2,745.2 ns |  0.32 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **256**  | **SingleElementMoved** |     **4,728.5 ns** |    **139.99 ns** |     **62.16 ns** |     **4,744.8 ns** |  **1.00** |    **0.02** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | SingleElementMoved |     5,628.5 ns |    409.12 ns |    181.65 ns |     5,530.1 ns |  1.19 |    0.04 |    8 |         - |          NA |
| BottomupMergeSort        | 256  | SingleElementMoved |     1,768.4 ns |     32.04 ns |     11.43 ns |     1,765.0 ns |  0.37 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 256  | SingleElementMoved |       745.6 ns |      6.93 ns |      3.62 ns |       745.5 ns |  0.16 |    0.00 |    3 |         - |          NA |
| RotateMergeSort          | 256  | SingleElementMoved |       605.3 ns |     15.43 ns |      5.50 ns |       603.6 ns |  0.13 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 256  | SingleElementMoved |       698.4 ns |    165.88 ns |     86.76 ns |       659.3 ns |  0.15 |    0.02 |    3 |         - |          NA |
| SymMergeSort             | 256  | SingleElementMoved |       547.7 ns |      1.61 ns |      0.72 ns |       547.7 ns |  0.12 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 256  | SingleElementMoved |     3,199.8 ns |    498.05 ns |    221.14 ns |     3,067.2 ns |  0.68 |    0.04 |    7 |         - |          NA |
| NaturalMergeSort         | 256  | SingleElementMoved |       615.8 ns |      5.48 ns |      1.95 ns |       615.8 ns |  0.13 |    0.00 |    3 |         - |          NA |
| TimSort                  | 256  | SingleElementMoved |       274.0 ns |      3.08 ns |      1.37 ns |       273.8 ns |  0.06 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | SingleElementMoved |       439.7 ns |     61.44 ns |     27.28 ns |       421.2 ns |  0.09 |    0.01 |    2 |         - |          NA |
| ShiftSort                | 256  | SingleElementMoved |       400.6 ns |      4.11 ns |      1.83 ns |       400.4 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 256  | SingleElementMoved |       946.1 ns |      6.48 ns |      2.88 ns |       944.6 ns |  0.20 |    0.00 |    4 |         - |          NA |
| Glidesort                | 256  | SingleElementMoved |     1,257.1 ns |     23.75 ns |     10.54 ns |     1,252.0 ns |  0.27 |    0.00 |    5 |         - |          NA |
| Driftsort                | 256  | SingleElementMoved |     1,317.4 ns |    177.86 ns |     93.02 ns |     1,299.1 ns |  0.28 |    0.02 |    5 |         - |          NA |
| FlatStableSort           | 256  | SingleElementMoved |     1,368.3 ns |    230.80 ns |    102.48 ns |     1,334.1 ns |  0.29 |    0.02 |    5 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **256**  | **Sorted**             |     **4,242.2 ns** |      **9.72 ns** |      **3.47 ns** |     **4,241.5 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Sorted             |     5,193.6 ns |     10.16 ns |      3.62 ns |     5,192.2 ns |  1.22 |    0.00 |    8 |         - |          NA |
| BottomupMergeSort        | 256  | Sorted             |     1,330.3 ns |    217.56 ns |    113.79 ns |     1,266.0 ns |  0.31 |    0.03 |    5 |         - |          NA |
| StdStableSort            | 256  | Sorted             |       752.2 ns |    250.52 ns |    131.03 ns |       663.9 ns |  0.18 |    0.03 |    4 |         - |          NA |
| RotateMergeSort          | 256  | Sorted             |       389.3 ns |    147.69 ns |     77.25 ns |       382.6 ns |  0.09 |    0.02 |    3 |         - |          NA |
| RotateMergeSortRecursive | 256  | Sorted             |       440.0 ns |      4.15 ns |      2.17 ns |       438.9 ns |  0.10 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 256  | Sorted             |       383.0 ns |      1.80 ns |      0.80 ns |       382.8 ns |  0.09 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 256  | Sorted             |     2,601.7 ns |      5.09 ns |      1.81 ns |     2,601.8 ns |  0.61 |    0.00 |    6 |         - |          NA |
| NaturalMergeSort         | 256  | Sorted             |       258.7 ns |      3.66 ns |      1.30 ns |       258.6 ns |  0.06 |    0.00 |    2 |         - |          NA |
| TimSort                  | 256  | Sorted             |       221.7 ns |     65.09 ns |     34.04 ns |       229.7 ns |  0.05 |    0.01 |    2 |         - |          NA |
| PowerSort                | 256  | Sorted             |       166.4 ns |      1.67 ns |      0.87 ns |       166.1 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Sorted             |       188.9 ns |      2.16 ns |      0.96 ns |       188.4 ns |  0.04 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 256  | Sorted             |       172.8 ns |     60.46 ns |     31.62 ns |       157.1 ns |  0.04 |    0.01 |    1 |         - |          NA |
| Glidesort                | 256  | Sorted             |       207.0 ns |      6.13 ns |      2.72 ns |       207.6 ns |  0.05 |    0.00 |    2 |         - |          NA |
| Driftsort                | 256  | Sorted             |       328.6 ns |    321.92 ns |    168.37 ns |       238.5 ns |  0.08 |    0.04 |    2 |         - |          NA |
| FlatStableSort           | 256  | Sorted             |     1,222.0 ns |      6.45 ns |      2.86 ns |     1,221.5 ns |  0.29 |    0.00 |    5 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **256**  | **Reversed**           |     **9,063.1 ns** |    **323.53 ns** |    **169.21 ns** |     **9,073.8 ns** |  **1.00** |    **0.02** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Reversed           |     8,356.4 ns |    173.74 ns |     77.14 ns |     8,363.0 ns |  0.92 |    0.02 |    6 |         - |          NA |
| BottomupMergeSort        | 256  | Reversed           |     4,934.1 ns |     58.27 ns |     20.78 ns |     4,931.2 ns |  0.54 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 256  | Reversed           |     6,486.3 ns |    432.76 ns |    226.34 ns |     6,341.0 ns |  0.72 |    0.03 |    5 |         - |          NA |
| RotateMergeSort          | 256  | Reversed           |     1,914.1 ns |     12.68 ns |      6.63 ns |     1,912.8 ns |  0.21 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 256  | Reversed           |     2,258.5 ns |     23.70 ns |      8.45 ns |     2,259.4 ns |  0.25 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 256  | Reversed           |     2,106.5 ns |     13.32 ns |      5.91 ns |     2,105.3 ns |  0.23 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 256  | Reversed           |     3,345.4 ns |    333.99 ns |    174.69 ns |     3,224.2 ns |  0.37 |    0.02 |    3 |         - |          NA |
| NaturalMergeSort         | 256  | Reversed           |       343.4 ns |     13.26 ns |      4.73 ns |       341.1 ns |  0.04 |    0.00 |    1 |         - |          NA |
| TimSort                  | 256  | Reversed           |       383.5 ns |     13.73 ns |      6.10 ns |       380.7 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | Reversed           |       253.8 ns |     32.16 ns |     16.82 ns |       255.2 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Reversed           |       306.7 ns |    110.70 ns |     49.15 ns |       298.0 ns |  0.03 |    0.01 |    1 |         - |          NA |
| SpinSort                 | 256  | Reversed           |       316.0 ns |    123.09 ns |     54.65 ns |       297.9 ns |  0.03 |    0.01 |    1 |         - |          NA |
| Glidesort                | 256  | Reversed           |       284.2 ns |      3.55 ns |      1.57 ns |       283.4 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 256  | Reversed           |       288.2 ns |      2.38 ns |      1.06 ns |       288.4 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | Reversed           |     2,893.2 ns |     52.71 ns |     18.80 ns |     2,885.5 ns |  0.32 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **256**  | **PipeOrgan**          |     **6,997.0 ns** |    **418.51 ns** |    **218.89 ns** |     **7,000.0 ns** |  **1.00** |    **0.04** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | PipeOrgan          |     7,211.0 ns |    344.30 ns |    180.07 ns |     7,216.7 ns |  1.03 |    0.04 |    8 |         - |          NA |
| BottomupMergeSort        | 256  | PipeOrgan          |     3,355.8 ns |    317.01 ns |    165.80 ns |     3,307.6 ns |  0.48 |    0.03 |    6 |         - |          NA |
| StdStableSort            | 256  | PipeOrgan          |     3,587.1 ns |     71.85 ns |     25.62 ns |     3,577.7 ns |  0.51 |    0.02 |    6 |         - |          NA |
| RotateMergeSort          | 256  | PipeOrgan          |     4,271.3 ns |    258.00 ns |    134.94 ns |     4,253.0 ns |  0.61 |    0.03 |    6 |         - |          NA |
| RotateMergeSortRecursive | 256  | PipeOrgan          |     5,480.5 ns |    413.97 ns |    216.51 ns |     5,555.1 ns |  0.78 |    0.04 |    7 |         - |          NA |
| SymMergeSort             | 256  | PipeOrgan          |     2,610.2 ns |    146.32 ns |     64.97 ns |     2,583.5 ns |  0.37 |    0.01 |    5 |         - |          NA |
| BlockMergeSort           | 256  | PipeOrgan          |     3,140.2 ns |     16.57 ns |      7.36 ns |     3,138.6 ns |  0.45 |    0.01 |    6 |         - |          NA |
| NaturalMergeSort         | 256  | PipeOrgan          |       737.5 ns |      7.04 ns |      3.13 ns |       736.3 ns |  0.11 |    0.00 |    2 |         - |          NA |
| TimSort                  | 256  | PipeOrgan          |       798.2 ns |      9.51 ns |      4.97 ns |       797.2 ns |  0.11 |    0.00 |    2 |         - |          NA |
| PowerSort                | 256  | PipeOrgan          |       669.4 ns |      9.11 ns |      4.05 ns |       669.3 ns |  0.10 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 256  | PipeOrgan          |       571.9 ns |    134.08 ns |     70.13 ns |       529.9 ns |  0.08 |    0.01 |    1 |         - |          NA |
| SpinSort                 | 256  | PipeOrgan          |     2,095.8 ns |    140.62 ns |     62.44 ns |     2,117.4 ns |  0.30 |    0.01 |    4 |         - |          NA |
| Glidesort                | 256  | PipeOrgan          |     1,265.4 ns |      9.85 ns |      5.15 ns |     1,264.1 ns |  0.18 |    0.01 |    3 |         - |          NA |
| Driftsort                | 256  | PipeOrgan          |       457.6 ns |      7.29 ns |      3.24 ns |       456.2 ns |  0.07 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | PipeOrgan          |     2,146.5 ns |     20.55 ns |      9.12 ns |     2,141.9 ns |  0.31 |    0.01 |    4 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **256**  | **ManyDuplicates**     |     **8,611.2 ns** |    **228.40 ns** |    **119.46 ns** |     **8,608.6 ns** |  **1.00** |    **0.02** |    **5** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | ManyDuplicates     |     8,451.5 ns |    254.12 ns |    112.83 ns |     8,494.0 ns |  0.98 |    0.02 |    5 |         - |          NA |
| BottomupMergeSort        | 256  | ManyDuplicates     |     4,687.5 ns |    371.96 ns |    194.54 ns |     4,602.8 ns |  0.54 |    0.02 |    3 |         - |          NA |
| StdStableSort            | 256  | ManyDuplicates     |     2,922.7 ns |    792.07 ns |    414.27 ns |     2,651.4 ns |  0.34 |    0.05 |    2 |         - |          NA |
| RotateMergeSort          | 256  | ManyDuplicates     |     9,561.7 ns |    263.66 ns |    137.90 ns |     9,545.6 ns |  1.11 |    0.02 |    5 |         - |          NA |
| RotateMergeSortRecursive | 256  | ManyDuplicates     |    11,688.2 ns |    389.45 ns |    203.69 ns |    11,680.5 ns |  1.36 |    0.03 |    6 |         - |          NA |
| SymMergeSort             | 256  | ManyDuplicates     |     6,488.5 ns |    427.46 ns |    223.57 ns |     6,432.1 ns |  0.75 |    0.03 |    4 |         - |          NA |
| BlockMergeSort           | 256  | ManyDuplicates     |     5,106.9 ns |    297.15 ns |    155.42 ns |     5,057.0 ns |  0.59 |    0.02 |    3 |         - |          NA |
| NaturalMergeSort         | 256  | ManyDuplicates     |     5,013.2 ns |    321.43 ns |    168.12 ns |     4,956.3 ns |  0.58 |    0.02 |    3 |         - |          NA |
| TimSort                  | 256  | ManyDuplicates     |     3,886.7 ns |    107.08 ns |     47.54 ns |     3,877.5 ns |  0.45 |    0.01 |    3 |         - |          NA |
| PowerSort                | 256  | ManyDuplicates     |     2,280.1 ns |     76.29 ns |     33.87 ns |     2,264.8 ns |  0.26 |    0.01 |    1 |         - |          NA |
| ShiftSort                | 256  | ManyDuplicates     |     4,099.6 ns |    457.92 ns |    239.50 ns |     4,046.5 ns |  0.48 |    0.03 |    3 |         - |          NA |
| SpinSort                 | 256  | ManyDuplicates     |     2,296.0 ns |    197.96 ns |     87.90 ns |     2,317.7 ns |  0.27 |    0.01 |    1 |         - |          NA |
| Glidesort                | 256  | ManyDuplicates     |     3,496.4 ns |     54.56 ns |     28.53 ns |     3,480.3 ns |  0.41 |    0.01 |    3 |         - |          NA |
| Driftsort                | 256  | ManyDuplicates     |     4,362.9 ns |     69.75 ns |     24.87 ns |     4,352.9 ns |  0.51 |    0.01 |    3 |         - |          NA |
| FlatStableSort           | 256  | ManyDuplicates     |     2,746.5 ns |    524.98 ns |    274.57 ns |     2,876.1 ns |  0.32 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **1024** | **Random**             |    **38,138.6 ns** |    **652.14 ns** |    **341.08 ns** |    **38,148.0 ns** |  **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Random             |    37,945.5 ns |    804.09 ns |    357.02 ns |    37,853.8 ns |  1.00 |    0.01 |    4 |         - |          NA |
| BottomupMergeSort        | 1024 | Random             |    21,426.4 ns |    400.62 ns |    209.53 ns |    21,429.8 ns |  0.56 |    0.01 |    2 |         - |          NA |
| StdStableSort            | 1024 | Random             |    13,932.9 ns |    365.79 ns |    191.31 ns |    13,955.9 ns |  0.37 |    0.01 |    1 |         - |          NA |
| RotateMergeSort          | 1024 | Random             |    62,384.8 ns |  2,439.36 ns |  1,275.83 ns |    62,236.8 ns |  1.64 |    0.03 |    5 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Random             |    75,231.7 ns |  1,019.29 ns |    452.57 ns |    75,459.9 ns |  1.97 |    0.02 |    6 |         - |          NA |
| SymMergeSort             | 1024 | Random             |    42,937.5 ns |    934.94 ns |    488.99 ns |    42,819.4 ns |  1.13 |    0.02 |    4 |         - |          NA |
| BlockMergeSort           | 1024 | Random             |    25,744.4 ns |    352.34 ns |    156.44 ns |    25,712.3 ns |  0.68 |    0.01 |    3 |         - |          NA |
| NaturalMergeSort         | 1024 | Random             |    27,286.2 ns |  2,721.27 ns |  1,208.26 ns |    27,148.5 ns |  0.72 |    0.03 |    3 |         - |          NA |
| TimSort                  | 1024 | Random             |    19,414.2 ns |    291.58 ns |    129.46 ns |    19,389.2 ns |  0.51 |    0.01 |    2 |         - |          NA |
| PowerSort                | 1024 | Random             |    12,678.4 ns |    546.62 ns |    285.89 ns |    12,677.7 ns |  0.33 |    0.01 |    1 |         - |          NA |
| ShiftSort                | 1024 | Random             |    18,678.1 ns |    371.81 ns |    194.46 ns |    18,649.9 ns |  0.49 |    0.01 |    2 |         - |          NA |
| SpinSort                 | 1024 | Random             |    11,933.6 ns |    466.70 ns |    207.22 ns |    11,838.0 ns |  0.31 |    0.01 |    1 |         - |          NA |
| Glidesort                | 1024 | Random             |    17,052.5 ns |    642.69 ns |    336.14 ns |    16,860.4 ns |  0.45 |    0.01 |    2 |         - |          NA |
| Driftsort                | 1024 | Random             |    21,192.0 ns |    362.75 ns |    189.72 ns |    21,093.9 ns |  0.56 |    0.01 |    2 |         - |          NA |
| FlatStableSort           | 1024 | Random             |    14,110.4 ns |    677.57 ns |    354.38 ns |    14,073.9 ns |  0.37 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **1024** | **SingleElementMoved** |    **18,704.4 ns** |    **201.85 ns** |    **105.57 ns** |    **18,713.5 ns** |  **1.00** |    **0.01** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | SingleElementMoved |    22,406.9 ns |    219.03 ns |    114.56 ns |    22,401.5 ns |  1.20 |    0.01 |    8 |         - |          NA |
| BottomupMergeSort        | 1024 | SingleElementMoved |     6,736.4 ns |    368.40 ns |    163.57 ns |     6,779.5 ns |  0.36 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 1024 | SingleElementMoved |     3,914.1 ns |    440.05 ns |    230.15 ns |     3,866.8 ns |  0.21 |    0.01 |    4 |         - |          NA |
| RotateMergeSort          | 1024 | SingleElementMoved |     2,187.8 ns |    427.66 ns |    223.67 ns |     2,034.6 ns |  0.12 |    0.01 |    2 |         - |          NA |
| RotateMergeSortRecursive | 1024 | SingleElementMoved |     2,257.1 ns |    236.78 ns |    123.84 ns |     2,198.8 ns |  0.12 |    0.01 |    2 |         - |          NA |
| SymMergeSort             | 1024 | SingleElementMoved |     1,824.2 ns |     19.28 ns |      8.56 ns |     1,826.5 ns |  0.10 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 1024 | SingleElementMoved |    14,268.0 ns |    390.79 ns |    204.39 ns |    14,298.9 ns |  0.76 |    0.01 |    7 |         - |          NA |
| NaturalMergeSort         | 1024 | SingleElementMoved |     2,330.3 ns |    276.21 ns |    144.47 ns |     2,231.3 ns |  0.12 |    0.01 |    2 |         - |          NA |
| TimSort                  | 1024 | SingleElementMoved |       848.2 ns |     19.66 ns |      8.73 ns |       844.7 ns |  0.05 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | SingleElementMoved |     1,707.2 ns |    190.22 ns |     84.46 ns |     1,736.7 ns |  0.09 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 1024 | SingleElementMoved |     1,466.4 ns |      4.73 ns |      2.48 ns |     1,466.9 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 1024 | SingleElementMoved |     4,679.6 ns |    293.64 ns |    153.58 ns |     4,597.3 ns |  0.25 |    0.01 |    5 |         - |          NA |
| Glidesort                | 1024 | SingleElementMoved |     3,107.5 ns |    235.28 ns |    123.05 ns |     3,030.4 ns |  0.17 |    0.01 |    3 |         - |          NA |
| Driftsort                | 1024 | SingleElementMoved |     1,341.2 ns |      9.85 ns |      3.51 ns |     1,339.8 ns |  0.07 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 1024 | SingleElementMoved |     5,828.8 ns |     33.68 ns |     12.01 ns |     5,825.7 ns |  0.31 |    0.00 |    6 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **1024** | **Sorted**             |    **17,157.8 ns** |     **95.86 ns** |     **42.56 ns** |    **17,173.5 ns** |  **1.00** |    **0.00** |    **9** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Sorted             |    21,216.0 ns |    382.85 ns |    169.99 ns |    21,121.1 ns |  1.24 |    0.01 |   10 |         - |          NA |
| BottomupMergeSort        | 1024 | Sorted             |     5,064.5 ns |    336.95 ns |    176.23 ns |     4,950.2 ns |  0.30 |    0.01 |    7 |         - |          NA |
| StdStableSort            | 1024 | Sorted             |     3,514.1 ns |     13.96 ns |      4.98 ns |     3,513.5 ns |  0.20 |    0.00 |    6 |         - |          NA |
| RotateMergeSort          | 1024 | Sorted             |     1,125.9 ns |     31.18 ns |     11.12 ns |     1,131.5 ns |  0.07 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Sorted             |     1,766.4 ns |      8.60 ns |      3.82 ns |     1,765.1 ns |  0.10 |    0.00 |    5 |         - |          NA |
| SymMergeSort             | 1024 | Sorted             |     1,506.3 ns |    222.55 ns |     98.81 ns |     1,465.5 ns |  0.09 |    0.01 |    4 |         - |          NA |
| BlockMergeSort           | 1024 | Sorted             |    11,660.9 ns |    249.38 ns |    110.73 ns |    11,712.8 ns |  0.68 |    0.01 |    8 |         - |          NA |
| NaturalMergeSort         | 1024 | Sorted             |       793.4 ns |      3.66 ns |      1.63 ns |       793.0 ns |  0.05 |    0.00 |    2 |         - |          NA |
| TimSort                  | 1024 | Sorted             |       600.7 ns |      2.46 ns |      1.29 ns |       600.3 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Sorted             |       585.9 ns |      1.89 ns |      0.84 ns |       586.0 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Sorted             |       617.8 ns |     11.13 ns |      3.97 ns |       617.2 ns |  0.04 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 1024 | Sorted             |       525.0 ns |      4.56 ns |      2.38 ns |       524.3 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Sorted             |       564.2 ns |     21.22 ns |      9.42 ns |       560.2 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | Sorted             |       739.5 ns |    163.99 ns |     72.81 ns |       792.9 ns |  0.04 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 1024 | Sorted             |     5,325.3 ns |    468.44 ns |    245.00 ns |     5,308.1 ns |  0.31 |    0.01 |    7 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **1024** | **Reversed**           |    **37,767.0 ns** |    **641.50 ns** |    **335.52 ns** |    **37,836.9 ns** |  **1.00** |    **0.01** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Reversed           |    34,288.1 ns |    527.93 ns |    276.12 ns |    34,230.1 ns |  0.91 |    0.01 |    7 |         - |          NA |
| BottomupMergeSort        | 1024 | Reversed           |    21,026.5 ns |    406.49 ns |    180.49 ns |    21,005.3 ns |  0.56 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 1024 | Reversed           |    26,948.4 ns |    296.18 ns |    154.91 ns |    26,901.4 ns |  0.71 |    0.01 |    6 |         - |          NA |
| RotateMergeSort          | 1024 | Reversed           |     9,031.7 ns |     45.10 ns |     16.08 ns |     9,027.5 ns |  0.24 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Reversed           |    10,622.7 ns |    316.90 ns |    140.70 ns |    10,653.3 ns |  0.28 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 1024 | Reversed           |     9,349.2 ns |    277.06 ns |    144.91 ns |     9,345.1 ns |  0.25 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | Reversed           |    16,061.1 ns |    159.64 ns |     56.93 ns |    16,084.1 ns |  0.43 |    0.00 |    4 |         - |          NA |
| NaturalMergeSort         | 1024 | Reversed           |     1,158.2 ns |      7.07 ns |      3.14 ns |     1,157.2 ns |  0.03 |    0.00 |    2 |         - |          NA |
| TimSort                  | 1024 | Reversed           |       934.6 ns |     82.49 ns |     36.63 ns |       911.8 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Reversed           |       906.1 ns |      4.64 ns |      1.65 ns |       906.8 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Reversed           |       920.3 ns |      4.70 ns |      2.09 ns |       919.4 ns |  0.02 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 1024 | Reversed           |       963.9 ns |      5.94 ns |      2.12 ns |       964.2 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Reversed           |       996.0 ns |    154.58 ns |     68.63 ns |       960.6 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | Reversed           |       967.4 ns |      5.02 ns |      2.62 ns |       966.9 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Reversed           |    11,957.8 ns |    373.95 ns |    195.58 ns |    12,050.1 ns |  0.32 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **1024** | **PipeOrgan**          |    **28,356.5 ns** |    **489.35 ns** |    **255.94 ns** |    **28,351.6 ns** |  **1.00** |    **0.01** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | PipeOrgan          |    28,472.7 ns |    466.01 ns |    206.91 ns |    28,507.7 ns |  1.00 |    0.01 |    7 |         - |          NA |
| BottomupMergeSort        | 1024 | PipeOrgan          |    13,613.3 ns |    318.50 ns |    166.58 ns |    13,596.4 ns |  0.48 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 1024 | PipeOrgan          |    15,567.8 ns |    221.11 ns |     98.17 ns |    15,598.7 ns |  0.55 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 1024 | PipeOrgan          |    18,660.8 ns |    220.14 ns |    115.14 ns |    18,629.1 ns |  0.66 |    0.01 |    5 |         - |          NA |
| RotateMergeSortRecursive | 1024 | PipeOrgan          |    23,160.4 ns |    121.01 ns |     63.29 ns |    23,167.1 ns |  0.82 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 1024 | PipeOrgan          |    11,791.9 ns |    789.70 ns |    413.03 ns |    11,648.9 ns |  0.42 |    0.01 |    5 |         - |          NA |
| BlockMergeSort           | 1024 | PipeOrgan          |    16,402.2 ns |    162.83 ns |     85.16 ns |    16,409.4 ns |  0.58 |    0.01 |    5 |         - |          NA |
| NaturalMergeSort         | 1024 | PipeOrgan          |     2,675.8 ns |      5.77 ns |      2.56 ns |     2,677.3 ns |  0.09 |    0.00 |    2 |         - |          NA |
| TimSort                  | 1024 | PipeOrgan          |     3,043.8 ns |    418.20 ns |    218.73 ns |     2,963.6 ns |  0.11 |    0.01 |    2 |         - |          NA |
| PowerSort                | 1024 | PipeOrgan          |     1,900.7 ns |    302.86 ns |    158.40 ns |     1,798.4 ns |  0.07 |    0.01 |    1 |         - |          NA |
| ShiftSort                | 1024 | PipeOrgan          |     1,927.1 ns |      3.19 ns |      1.14 ns |     1,927.2 ns |  0.07 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 1024 | PipeOrgan          |     9,243.1 ns |    756.96 ns |    395.90 ns |     9,401.7 ns |  0.33 |    0.01 |    4 |         - |          NA |
| Glidesort                | 1024 | PipeOrgan          |     4,864.3 ns |    275.67 ns |    144.18 ns |     4,763.9 ns |  0.17 |    0.01 |    3 |         - |          NA |
| Driftsort                | 1024 | PipeOrgan          |     1,549.1 ns |     22.91 ns |     10.17 ns |     1,549.1 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | PipeOrgan          |     9,450.5 ns |    456.27 ns |    202.59 ns |     9,434.5 ns |  0.33 |    0.01 |    4 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **1024** | **ManyDuplicates**     |    **37,122.9 ns** |  **1,493.81 ns** |    **781.29 ns** |    **37,229.8 ns** |  **1.00** |    **0.03** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | ManyDuplicates     |    35,439.8 ns |    543.84 ns |    241.47 ns |    35,487.1 ns |  0.96 |    0.02 |    3 |         - |          NA |
| BottomupMergeSort        | 1024 | ManyDuplicates     |    20,006.0 ns |    551.92 ns |    288.66 ns |    19,999.3 ns |  0.54 |    0.01 |    2 |         - |          NA |
| StdStableSort            | 1024 | ManyDuplicates     |    13,543.4 ns |  1,633.20 ns |    725.15 ns |    13,247.8 ns |  0.36 |    0.02 |    1 |         - |          NA |
| RotateMergeSort          | 1024 | ManyDuplicates     |    51,119.5 ns |  1,168.97 ns |    611.39 ns |    51,111.8 ns |  1.38 |    0.03 |    4 |         - |          NA |
| RotateMergeSortRecursive | 1024 | ManyDuplicates     |    57,636.0 ns |    666.04 ns |    348.35 ns |    57,578.5 ns |  1.55 |    0.03 |    4 |         - |          NA |
| SymMergeSort             | 1024 | ManyDuplicates     |    36,771.6 ns |    509.96 ns |    226.43 ns |    36,718.8 ns |  0.99 |    0.02 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | ManyDuplicates     |    26,533.5 ns |    441.42 ns |    195.99 ns |    26,500.3 ns |  0.72 |    0.01 |    2 |         - |          NA |
| NaturalMergeSort         | 1024 | ManyDuplicates     |    23,468.8 ns |    580.60 ns |    303.66 ns |    23,374.1 ns |  0.63 |    0.01 |    2 |         - |          NA |
| TimSort                  | 1024 | ManyDuplicates     |    19,110.0 ns |    800.01 ns |    418.42 ns |    18,967.3 ns |  0.51 |    0.01 |    2 |         - |          NA |
| PowerSort                | 1024 | ManyDuplicates     |    11,746.9 ns |    545.66 ns |    242.28 ns |    11,694.0 ns |  0.32 |    0.01 |    1 |         - |          NA |
| ShiftSort                | 1024 | ManyDuplicates     |    18,338.3 ns |    523.53 ns |    273.81 ns |    18,267.3 ns |  0.49 |    0.01 |    2 |         - |          NA |
| SpinSort                 | 1024 | ManyDuplicates     |    11,697.8 ns |    677.89 ns |    354.55 ns |    11,648.8 ns |  0.32 |    0.01 |    1 |         - |          NA |
| Glidesort                | 1024 | ManyDuplicates     |    16,086.0 ns |    264.79 ns |    138.49 ns |    16,029.0 ns |  0.43 |    0.01 |    2 |         - |          NA |
| Driftsort                | 1024 | ManyDuplicates     |    17,289.8 ns |    173.29 ns |     76.94 ns |    17,290.7 ns |  0.47 |    0.01 |    2 |         - |          NA |
| FlatStableSort           | 1024 | ManyDuplicates     |    11,903.1 ns |    717.86 ns |    318.73 ns |    11,823.8 ns |  0.32 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **4096** | **Random**             |   **192,033.3 ns** | **17,486.44 ns** |  **9,145.74 ns** |   **190,989.6 ns** |  **1.00** |    **0.06** |    **2** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Random             |   172,848.6 ns |  5,947.31 ns |  2,640.64 ns |   171,771.6 ns |  0.90 |    0.04 |    2 |         - |          NA |
| BottomupMergeSort        | 4096 | Random             |    96,824.4 ns |  1,593.33 ns |    707.45 ns |    96,798.3 ns |  0.51 |    0.02 |    1 |         - |          NA |
| StdStableSort            | 4096 | Random             |    79,490.0 ns |  6,805.60 ns |  3,559.46 ns |    79,341.6 ns |  0.41 |    0.03 |    1 |         - |          NA |
| RotateMergeSort          | 4096 | Random             |   626,644.0 ns |  5,800.65 ns |  3,033.85 ns |   625,378.1 ns |  3.27 |    0.15 |    4 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Random             |   673,913.3 ns |  4,803.24 ns |  2,512.19 ns |   673,756.8 ns |  3.52 |    0.16 |    4 |         - |          NA |
| SymMergeSort             | 4096 | Random             |   416,497.9 ns |  5,693.63 ns |  2,528.01 ns |   416,082.6 ns |  2.17 |    0.10 |    3 |         - |          NA |
| BlockMergeSort           | 4096 | Random             |   147,563.7 ns | 19,171.32 ns | 10,026.97 ns |   147,270.6 ns |  0.77 |    0.06 |    2 |         - |          NA |
| NaturalMergeSort         | 4096 | Random             |   135,854.8 ns |  7,983.16 ns |  3,544.57 ns |   136,401.0 ns |  0.71 |    0.04 |    2 |         - |          NA |
| TimSort                  | 4096 | Random             |    98,896.4 ns |  7,121.48 ns |  3,161.98 ns |    97,478.5 ns |  0.52 |    0.03 |    1 |         - |          NA |
| PowerSort                | 4096 | Random             |    64,579.6 ns |  1,364.41 ns |    605.81 ns |    64,561.1 ns |  0.34 |    0.02 |    1 |         - |          NA |
| ShiftSort                | 4096 | Random             |    89,659.4 ns |  4,849.81 ns |  2,153.34 ns |    88,922.4 ns |  0.47 |    0.02 |    1 |         - |          NA |
| SpinSort                 | 4096 | Random             |    63,040.6 ns |  3,439.80 ns |  1,527.29 ns |    62,754.0 ns |  0.33 |    0.02 |    1 |         - |          NA |
| Glidesort                | 4096 | Random             |    82,593.4 ns |    796.18 ns |    353.51 ns |    82,613.5 ns |  0.43 |    0.02 |    1 |         - |          NA |
| Driftsort                | 4096 | Random             |    97,736.7 ns |    646.42 ns |    338.09 ns |    97,680.4 ns |  0.51 |    0.02 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Random             |    68,968.3 ns |    749.87 ns |    267.41 ns |    69,029.2 ns |  0.36 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **4096** | **SingleElementMoved** |    **75,088.1 ns** |  **1,207.43 ns** |    **536.11 ns** |    **74,979.2 ns** |  **1.00** |    **0.01** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | SingleElementMoved |    90,564.3 ns |    339.36 ns |    177.49 ns |    90,589.4 ns |  1.21 |    0.01 |    9 |         - |          NA |
| BottomupMergeSort        | 4096 | SingleElementMoved |    26,973.5 ns |  1,305.61 ns |    682.86 ns |    26,720.5 ns |  0.36 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 4096 | SingleElementMoved |    18,340.8 ns |    551.21 ns |    244.74 ns |    18,226.9 ns |  0.24 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 4096 | SingleElementMoved |     8,016.8 ns |    454.03 ns |    237.46 ns |     8,118.3 ns |  0.11 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 4096 | SingleElementMoved |     7,928.9 ns |    265.79 ns |    118.01 ns |     7,862.0 ns |  0.11 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 4096 | SingleElementMoved |     7,076.8 ns |    309.23 ns |    137.30 ns |     6,977.2 ns |  0.09 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 4096 | SingleElementMoved |    57,835.5 ns |    869.38 ns |    386.01 ns |    57,676.2 ns |  0.77 |    0.01 |    7 |         - |          NA |
| NaturalMergeSort         | 4096 | SingleElementMoved |     7,791.1 ns |  1,125.27 ns |    499.63 ns |     7,599.7 ns |  0.10 |    0.01 |    3 |         - |          NA |
| TimSort                  | 4096 | SingleElementMoved |     3,163.7 ns |    232.59 ns |    121.65 ns |     3,078.6 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | SingleElementMoved |     5,867.6 ns |    388.51 ns |    172.50 ns |     5,779.7 ns |  0.08 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 4096 | SingleElementMoved |     5,665.6 ns |    198.30 ns |    103.72 ns |     5,613.6 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 4096 | SingleElementMoved |    13,985.6 ns |    285.40 ns |    126.72 ns |    14,001.4 ns |  0.19 |    0.00 |    4 |         - |          NA |
| Glidesort                | 4096 | SingleElementMoved |    11,835.7 ns |    424.95 ns |    188.68 ns |    11,870.3 ns |  0.16 |    0.00 |    4 |         - |          NA |
| Driftsort                | 4096 | SingleElementMoved |     5,399.4 ns |    381.22 ns |    169.27 ns |     5,319.8 ns |  0.07 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 4096 | SingleElementMoved |    24,663.9 ns |    343.77 ns |    179.80 ns |    24,587.3 ns |  0.33 |    0.00 |    6 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **4096** | **Sorted**             |    **68,966.3 ns** |    **610.60 ns** |    **319.36 ns** |    **68,963.1 ns** |  **1.00** |    **0.01** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Sorted             |    85,052.2 ns |    741.71 ns |    387.93 ns |    85,047.7 ns |  1.23 |    0.01 |    9 |         - |          NA |
| BottomupMergeSort        | 4096 | Sorted             |    20,140.9 ns |    628.93 ns |    279.25 ns |    20,005.7 ns |  0.29 |    0.00 |    6 |         - |          NA |
| StdStableSort            | 4096 | Sorted             |    18,281.5 ns |    770.85 ns |    403.17 ns |    18,029.3 ns |  0.27 |    0.01 |    6 |         - |          NA |
| RotateMergeSort          | 4096 | Sorted             |     4,661.8 ns |    401.73 ns |    210.11 ns |     4,670.5 ns |  0.07 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Sorted             |     7,399.2 ns |    334.79 ns |    148.65 ns |     7,330.7 ns |  0.11 |    0.00 |    5 |         - |          NA |
| SymMergeSort             | 4096 | Sorted             |     5,920.7 ns |    315.77 ns |    140.20 ns |     5,810.8 ns |  0.09 |    0.00 |    4 |         - |          NA |
| BlockMergeSort           | 4096 | Sorted             |    47,649.7 ns |    509.31 ns |    266.38 ns |    47,514.6 ns |  0.69 |    0.00 |    7 |         - |          NA |
| NaturalMergeSort         | 4096 | Sorted             |     3,201.6 ns |    240.21 ns |    106.65 ns |     3,156.1 ns |  0.05 |    0.00 |    2 |         - |          NA |
| TimSort                  | 4096 | Sorted             |     2,414.7 ns |     38.65 ns |     20.22 ns |     2,405.2 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | Sorted             |     2,423.5 ns |    322.89 ns |    168.88 ns |     2,381.5 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | Sorted             |     2,230.9 ns |      5.75 ns |      2.55 ns |     2,229.7 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 4096 | Sorted             |     2,024.4 ns |     10.50 ns |      5.49 ns |     2,023.1 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 4096 | Sorted             |     1,986.8 ns |     30.77 ns |     13.66 ns |     1,984.0 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | Sorted             |     2,480.9 ns |     68.49 ns |     24.42 ns |     2,471.9 ns |  0.04 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Sorted             |    20,603.6 ns |    739.17 ns |    328.20 ns |    20,596.5 ns |  0.30 |    0.00 |    6 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **4096** | **Reversed**           |   **156,558.0 ns** |  **1,546.11 ns** |    **808.65 ns** |   **156,488.5 ns** |  **1.00** |    **0.01** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Reversed           |   142,113.0 ns |    756.99 ns |    336.11 ns |   141,920.7 ns |  0.91 |    0.00 |    6 |         - |          NA |
| BottomupMergeSort        | 4096 | Reversed           |    90,218.3 ns |  3,215.03 ns |  1,681.52 ns |    90,196.0 ns |  0.58 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 4096 | Reversed           |   111,597.2 ns |    422.28 ns |    220.86 ns |   111,610.1 ns |  0.71 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 4096 | Reversed           |    43,165.5 ns |    928.53 ns |    485.64 ns |    43,017.7 ns |  0.28 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Reversed           |    48,711.4 ns |    800.20 ns |    418.52 ns |    48,629.6 ns |  0.31 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 4096 | Reversed           |    39,387.0 ns |    506.69 ns |    265.01 ns |    39,298.2 ns |  0.25 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 4096 | Reversed           |    73,908.9 ns |  1,033.96 ns |    540.78 ns |    74,057.5 ns |  0.47 |    0.00 |    3 |         - |          NA |
| NaturalMergeSort         | 4096 | Reversed           |     4,448.5 ns |    255.03 ns |    113.23 ns |     4,382.8 ns |  0.03 |    0.00 |    1 |         - |          NA |
| TimSort                  | 4096 | Reversed           |     3,620.0 ns |    304.84 ns |    135.35 ns |     3,543.9 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | Reversed           |     3,535.4 ns |     21.36 ns |      7.62 ns |     3,536.6 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | Reversed           |     3,425.4 ns |      4.47 ns |      1.98 ns |     3,424.8 ns |  0.02 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 4096 | Reversed           |     3,955.3 ns |    205.66 ns |     91.32 ns |     3,942.2 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 4096 | Reversed           |     3,691.6 ns |    240.88 ns |    106.95 ns |     3,622.9 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | Reversed           |     3,619.1 ns |     12.87 ns |      4.59 ns |     3,617.7 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Reversed           |    47,945.3 ns |    724.45 ns |    378.90 ns |    47,818.0 ns |  0.31 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **4096** | **PipeOrgan**          |   **115,825.0 ns** |  **1,036.18 ns** |    **541.94 ns** |   **115,781.8 ns** |  **1.00** |    **0.01** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | PipeOrgan          |   117,776.2 ns |  2,073.29 ns |  1,084.37 ns |   117,782.1 ns |  1.02 |    0.01 |    7 |         - |          NA |
| BottomupMergeSort        | 4096 | PipeOrgan          |    59,267.6 ns |  1,609.70 ns |    841.91 ns |    59,076.9 ns |  0.51 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 4096 | PipeOrgan          |    65,852.7 ns |  1,041.79 ns |    462.56 ns |    65,841.4 ns |  0.57 |    0.00 |    6 |         - |          NA |
| RotateMergeSort          | 4096 | PipeOrgan          |    80,246.7 ns |    846.01 ns |    442.48 ns |    80,365.1 ns |  0.69 |    0.00 |    6 |         - |          NA |
| RotateMergeSortRecursive | 4096 | PipeOrgan          |    99,005.9 ns |    595.76 ns |    311.59 ns |    99,100.0 ns |  0.85 |    0.00 |    7 |         - |          NA |
| SymMergeSort             | 4096 | PipeOrgan          |    49,885.0 ns |  1,165.51 ns |    609.59 ns |    49,934.2 ns |  0.43 |    0.01 |    6 |         - |          NA |
| BlockMergeSort           | 4096 | PipeOrgan          |    68,991.8 ns |    870.38 ns |    455.22 ns |    69,019.9 ns |  0.60 |    0.00 |    6 |         - |          NA |
| NaturalMergeSort         | 4096 | PipeOrgan          |    10,570.2 ns |    373.25 ns |    195.21 ns |    10,620.2 ns |  0.09 |    0.00 |    3 |         - |          NA |
| TimSort                  | 4096 | PipeOrgan          |    11,553.9 ns |    862.99 ns |    451.36 ns |    11,445.6 ns |  0.10 |    0.00 |    3 |         - |          NA |
| PowerSort                | 4096 | PipeOrgan          |     7,008.4 ns |    566.33 ns |    251.45 ns |     7,027.0 ns |  0.06 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 4096 | PipeOrgan          |     7,478.8 ns |    474.37 ns |    210.62 ns |     7,351.1 ns |  0.06 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 4096 | PipeOrgan          |     8,545.7 ns |    519.15 ns |    271.52 ns |     8,466.8 ns |  0.07 |    0.00 |    2 |         - |          NA |
| Glidesort                | 4096 | PipeOrgan          |    19,269.0 ns |    477.73 ns |    249.86 ns |    19,238.8 ns |  0.17 |    0.00 |    4 |         - |          NA |
| Driftsort                | 4096 | PipeOrgan          |     5,776.2 ns |    196.18 ns |     87.10 ns |     5,731.6 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | PipeOrgan          |    37,783.9 ns |    923.69 ns |    483.11 ns |    37,647.5 ns |  0.33 |    0.00 |    5 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **4096** | **ManyDuplicates**     |   **155,213.4 ns** |  **3,775.12 ns** |  **1,974.46 ns** |   **154,415.8 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | ManyDuplicates     |   150,924.4 ns |  1,880.89 ns |    983.74 ns |   151,320.3 ns |  0.97 |    0.01 |    3 |         - |          NA |
| BottomupMergeSort        | 4096 | ManyDuplicates     |    92,735.9 ns |  2,975.60 ns |  1,556.30 ns |    92,868.9 ns |  0.60 |    0.01 |    2 |         - |          NA |
| StdStableSort            | 4096 | ManyDuplicates     |    71,817.0 ns |  3,896.45 ns |  2,037.92 ns |    71,971.3 ns |  0.46 |    0.01 |    2 |         - |          NA |
| RotateMergeSort          | 4096 | ManyDuplicates     |   337,502.5 ns | 21,392.55 ns | 11,188.72 ns |   337,301.2 ns |  2.17 |    0.07 |    5 |         - |          NA |
| RotateMergeSortRecursive | 4096 | ManyDuplicates     |   281,828.7 ns | 10,128.32 ns |  3,611.86 ns |   282,744.4 ns |  1.82 |    0.03 |    5 |         - |          NA |
| SymMergeSort             | 4096 | ManyDuplicates     |   212,366.2 ns | 10,383.90 ns |  5,430.98 ns |   212,224.7 ns |  1.37 |    0.04 |    4 |         - |          NA |
| BlockMergeSort           | 4096 | ManyDuplicates     |   134,724.1 ns |  1,857.66 ns |    824.81 ns |   134,303.8 ns |  0.87 |    0.01 |    3 |         - |          NA |
| NaturalMergeSort         | 4096 | ManyDuplicates     |   113,543.3 ns |  1,335.09 ns |    592.79 ns |   113,575.7 ns |  0.73 |    0.01 |    3 |         - |          NA |
| TimSort                  | 4096 | ManyDuplicates     |    81,639.2 ns |    705.84 ns |    313.40 ns |    81,581.5 ns |  0.53 |    0.01 |    2 |         - |          NA |
| PowerSort                | 4096 | ManyDuplicates     |    56,956.8 ns |  2,018.79 ns |    896.36 ns |    57,008.5 ns |  0.37 |    0.01 |    1 |         - |          NA |
| ShiftSort                | 4096 | ManyDuplicates     |    85,609.9 ns |  5,847.02 ns |  3,058.10 ns |    84,484.7 ns |  0.55 |    0.02 |    2 |         - |          NA |
| SpinSort                 | 4096 | ManyDuplicates     |    54,643.1 ns |  2,825.35 ns |  1,254.47 ns |    54,469.4 ns |  0.35 |    0.01 |    1 |         - |          NA |
| Glidesort                | 4096 | ManyDuplicates     |    46,583.8 ns |    798.42 ns |    417.59 ns |    46,745.3 ns |  0.30 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | ManyDuplicates     |    43,461.0 ns |    796.50 ns |    416.58 ns |    43,453.4 ns |  0.28 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | ManyDuplicates     |    58,657.4 ns |  2,708.67 ns |  1,416.69 ns |    58,545.1 ns |  0.38 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **8192** | **Random**             |   **754,130.8 ns** | **11,326.18 ns** |  **5,028.89 ns** |   **755,362.7 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Random             |   662,792.4 ns |  3,817.65 ns |  1,695.06 ns |   662,272.3 ns |  0.88 |    0.01 |    3 |         - |          NA |
| BottomupMergeSort        | 8192 | Random             |   532,131.1 ns |  2,657.57 ns |  1,389.96 ns |   532,191.4 ns |  0.71 |    0.00 |    3 |         - |          NA |
| StdStableSort            | 8192 | Random             |   388,932.4 ns |  6,224.57 ns |  2,763.75 ns |   388,062.7 ns |  0.52 |    0.00 |    2 |         - |          NA |
| RotateMergeSort          | 8192 | Random             | 1,549,473.9 ns |  7,895.66 ns |  4,129.58 ns | 1,550,205.5 ns |  2.05 |    0.01 |    5 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Random             | 1,672,446.9 ns |  3,002.53 ns |  1,333.14 ns | 1,672,502.3 ns |  2.22 |    0.01 |    5 |         - |          NA |
| SymMergeSort             | 8192 | Random             | 1,133,532.8 ns |  1,651.45 ns |    863.74 ns | 1,133,555.6 ns |  1.50 |    0.01 |    4 |         - |          NA |
| BlockMergeSort           | 8192 | Random             |   661,986.0 ns |  7,535.93 ns |  3,941.44 ns |   661,838.0 ns |  0.88 |    0.01 |    3 |         - |          NA |
| NaturalMergeSort         | 8192 | Random             |   684,574.2 ns |  2,074.23 ns |  1,084.86 ns |   684,430.0 ns |  0.91 |    0.01 |    3 |         - |          NA |
| TimSort                  | 8192 | Random             |   580,877.9 ns |  4,903.42 ns |  2,564.58 ns |   580,731.9 ns |  0.77 |    0.01 |    3 |         - |          NA |
| PowerSort                | 8192 | Random             |   435,424.6 ns |  2,519.77 ns |  1,118.79 ns |   435,535.0 ns |  0.58 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 8192 | Random             |   595,272.8 ns |  2,665.65 ns |  1,394.19 ns |   595,152.6 ns |  0.79 |    0.01 |    3 |         - |          NA |
| SpinSort                 | 8192 | Random             |   362,925.7 ns |  6,344.56 ns |  3,318.33 ns |   362,212.4 ns |  0.48 |    0.01 |    2 |         - |          NA |
| Glidesort                | 8192 | Random             |   178,969.5 ns |  2,604.78 ns |  1,362.35 ns |   178,800.8 ns |  0.24 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Random             |   207,424.7 ns |  1,592.44 ns |    832.88 ns |   207,268.8 ns |  0.28 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Random             |   401,284.1 ns |  4,524.64 ns |  2,366.48 ns |   401,891.1 ns |  0.53 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **8192** | **SingleElementMoved** |   **149,743.0 ns** |  **1,355.72 ns** |    **709.07 ns** |   **149,642.5 ns** |  **1.00** |    **0.01** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | SingleElementMoved |   181,355.8 ns |  2,689.68 ns |  1,406.75 ns |   181,478.5 ns |  1.21 |    0.01 |    9 |         - |          NA |
| BottomupMergeSort        | 8192 | SingleElementMoved |    56,379.9 ns |  1,767.51 ns |    924.44 ns |    56,133.2 ns |  0.38 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 8192 | SingleElementMoved |    34,974.5 ns |    874.95 ns |    388.48 ns |    34,881.7 ns |  0.23 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 8192 | SingleElementMoved |    15,083.4 ns |    217.97 ns |     77.73 ns |    15,090.7 ns |  0.10 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 8192 | SingleElementMoved |    15,748.9 ns |    432.30 ns |    191.94 ns |    15,799.7 ns |  0.11 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 8192 | SingleElementMoved |    14,240.7 ns |    319.89 ns |    142.03 ns |    14,293.8 ns |  0.10 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 8192 | SingleElementMoved |   114,336.0 ns |    569.14 ns |    297.67 ns |   114,381.4 ns |  0.76 |    0.00 |    7 |         - |          NA |
| NaturalMergeSort         | 8192 | SingleElementMoved |    17,585.1 ns |    773.04 ns |    343.24 ns |    17,408.2 ns |  0.12 |    0.00 |    3 |         - |          NA |
| TimSort                  | 8192 | SingleElementMoved |     6,227.1 ns |    390.82 ns |    204.41 ns |     6,113.6 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | SingleElementMoved |    12,219.5 ns |  1,588.94 ns |    831.05 ns |    11,674.1 ns |  0.08 |    0.01 |    2 |         - |          NA |
| ShiftSort                | 8192 | SingleElementMoved |    11,459.5 ns |    358.59 ns |    187.55 ns |    11,518.7 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | SingleElementMoved |    24,411.6 ns |    842.16 ns |    440.47 ns |    24,447.0 ns |  0.16 |    0.00 |    4 |         - |          NA |
| Glidesort                | 8192 | SingleElementMoved |    23,650.4 ns |    786.34 ns |    411.27 ns |    23,561.3 ns |  0.16 |    0.00 |    4 |         - |          NA |
| Driftsort                | 8192 | SingleElementMoved |    10,363.9 ns |    371.42 ns |    194.26 ns |    10,389.8 ns |  0.07 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 8192 | SingleElementMoved |    49,225.6 ns |    964.15 ns |    504.27 ns |    48,990.0 ns |  0.33 |    0.00 |    6 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **8192** | **Sorted**             |   **137,439.8 ns** |    **533.23 ns** |    **278.89 ns** |   **137,510.0 ns** |  **1.00** |    **0.00** |    **9** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Sorted             |   172,284.9 ns |  2,907.59 ns |  1,520.73 ns |   171,715.5 ns |  1.25 |    0.01 |   10 |         - |          NA |
| BottomupMergeSort        | 8192 | Sorted             |    43,065.2 ns |    289.27 ns |    151.29 ns |    43,023.5 ns |  0.31 |    0.00 |    7 |         - |          NA |
| StdStableSort            | 8192 | Sorted             |    34,823.2 ns |    902.73 ns |    472.14 ns |    34,628.8 ns |  0.25 |    0.00 |    6 |         - |          NA |
| RotateMergeSort          | 8192 | Sorted             |     9,185.0 ns |    510.90 ns |    182.19 ns |     9,216.0 ns |  0.07 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Sorted             |    14,698.0 ns |    101.19 ns |     44.93 ns |    14,696.1 ns |  0.11 |    0.00 |    5 |         - |          NA |
| SymMergeSort             | 8192 | Sorted             |    12,530.9 ns |  1,626.95 ns |    850.93 ns |    12,038.4 ns |  0.09 |    0.01 |    4 |         - |          NA |
| BlockMergeSort           | 8192 | Sorted             |    92,826.1 ns |    219.90 ns |     97.64 ns |    92,826.7 ns |  0.68 |    0.00 |    8 |         - |          NA |
| NaturalMergeSort         | 8192 | Sorted             |     5,960.7 ns |    272.90 ns |    142.73 ns |     5,946.3 ns |  0.04 |    0.00 |    2 |         - |          NA |
| TimSort                  | 8192 | Sorted             |     4,566.0 ns |     18.48 ns |      8.20 ns |     4,563.5 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Sorted             |     4,702.0 ns |    631.97 ns |    280.60 ns |     4,555.2 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Sorted             |     4,674.9 ns |    318.13 ns |    166.39 ns |     4,577.1 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 8192 | Sorted             |     3,989.7 ns |     27.90 ns |      9.95 ns |     3,991.4 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Sorted             |     3,937.5 ns |     99.28 ns |     35.40 ns |     3,940.5 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Sorted             |     4,756.9 ns |    376.60 ns |    167.21 ns |     4,672.2 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Sorted             |     4,124.9 ns |     62.28 ns |     32.58 ns |     4,119.7 ns |  0.03 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **8192** | **Reversed**           |   **321,425.5 ns** |  **2,739.61 ns** |  **1,432.87 ns** |   **321,540.7 ns** |  **1.00** |    **0.01** |    **5** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Reversed           |   290,264.2 ns |  2,088.39 ns |    927.26 ns |   290,429.3 ns |  0.90 |    0.00 |    5 |         - |          NA |
| BottomupMergeSort        | 8192 | Reversed           |   188,964.2 ns |  2,319.72 ns |  1,213.26 ns |   189,186.8 ns |  0.59 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 8192 | Reversed           |   412,102.4 ns |  1,971.27 ns |  1,031.01 ns |   412,123.9 ns |  1.28 |    0.01 |    6 |         - |          NA |
| RotateMergeSort          | 8192 | Reversed           |    91,260.5 ns |    265.09 ns |    138.65 ns |    91,270.8 ns |  0.28 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Reversed           |   102,981.3 ns |  1,099.92 ns |    575.28 ns |   103,029.5 ns |  0.32 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 8192 | Reversed           |    82,132.7 ns |  1,168.99 ns |    611.41 ns |    82,193.1 ns |  0.26 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 8192 | Reversed           |   153,134.3 ns |    885.99 ns |    463.39 ns |   153,018.4 ns |  0.48 |    0.00 |    3 |         - |          NA |
| NaturalMergeSort         | 8192 | Reversed           |     8,816.9 ns |    321.70 ns |    168.26 ns |     8,723.7 ns |  0.03 |    0.00 |    1 |         - |          NA |
| TimSort                  | 8192 | Reversed           |     7,344.2 ns |    337.98 ns |    150.07 ns |     7,263.9 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Reversed           |     7,382.1 ns |    313.16 ns |    163.79 ns |     7,389.0 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Reversed           |     6,787.6 ns |     10.07 ns |      3.59 ns |     6,787.8 ns |  0.02 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 8192 | Reversed           |     7,562.4 ns |     28.33 ns |     10.10 ns |     7,560.9 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Reversed           |     7,277.8 ns |    315.59 ns |    165.06 ns |     7,182.5 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Reversed           |     7,284.2 ns |    194.56 ns |     86.39 ns |     7,291.6 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Reversed           |     7,743.8 ns |    257.05 ns |    114.13 ns |     7,668.6 ns |  0.02 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **8192** | **PipeOrgan**          |   **235,423.3 ns** |  **2,607.78 ns** |  **1,363.92 ns** |   **235,561.9 ns** |  **1.00** |    **0.01** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | PipeOrgan          |   237,516.4 ns |  1,556.56 ns |    814.11 ns |   237,439.8 ns |  1.01 |    0.01 |    7 |         - |          NA |
| BottomupMergeSort        | 8192 | PipeOrgan          |   122,205.7 ns |  2,888.79 ns |  1,510.89 ns |   122,210.6 ns |  0.52 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 8192 | PipeOrgan          |   225,378.7 ns |  2,785.74 ns |  1,456.99 ns |   224,904.5 ns |  0.96 |    0.01 |    7 |         - |          NA |
| RotateMergeSort          | 8192 | PipeOrgan          |   166,592.9 ns |  1,300.34 ns |    680.10 ns |   166,496.9 ns |  0.71 |    0.00 |    6 |         - |          NA |
| RotateMergeSortRecursive | 8192 | PipeOrgan          |   204,308.8 ns |  2,041.45 ns |    906.42 ns |   204,167.3 ns |  0.87 |    0.01 |    7 |         - |          NA |
| SymMergeSort             | 8192 | PipeOrgan          |   102,092.8 ns |  1,032.59 ns |    540.06 ns |   102,227.7 ns |  0.43 |    0.00 |    6 |         - |          NA |
| BlockMergeSort           | 8192 | PipeOrgan          |   139,736.2 ns |    751.61 ns |    333.72 ns |   139,642.4 ns |  0.59 |    0.00 |    6 |         - |          NA |
| NaturalMergeSort         | 8192 | PipeOrgan          |    21,472.6 ns |    391.83 ns |    139.73 ns |    21,490.4 ns |  0.09 |    0.00 |    3 |         - |          NA |
| TimSort                  | 8192 | PipeOrgan          |    22,368.3 ns |    242.86 ns |     86.60 ns |    22,378.9 ns |  0.10 |    0.00 |    3 |         - |          NA |
| PowerSort                | 8192 | PipeOrgan          |    13,969.9 ns |    869.60 ns |    386.11 ns |    14,015.1 ns |  0.06 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 8192 | PipeOrgan          |    15,086.9 ns |    669.07 ns |    297.07 ns |    15,053.0 ns |  0.06 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | PipeOrgan          |    18,402.6 ns |  1,176.92 ns |    522.56 ns |    18,260.8 ns |  0.08 |    0.00 |    3 |         - |          NA |
| Glidesort                | 8192 | PipeOrgan          |    37,906.2 ns |  1,163.64 ns |    516.66 ns |    37,696.8 ns |  0.16 |    0.00 |    4 |         - |          NA |
| Driftsort                | 8192 | PipeOrgan          |    11,512.2 ns |    274.21 ns |    143.42 ns |    11,493.5 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | PipeOrgan          |    74,275.2 ns |  1,306.03 ns |    683.08 ns |    74,287.4 ns |  0.32 |    0.00 |    5 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **MergeSort**                | **8192** | **ManyDuplicates**     |   **468,122.9 ns** | **17,119.38 ns** |  **7,601.11 ns** |   **468,075.4 ns** |  **1.00** |    **0.02** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | ManyDuplicates     |   479,308.1 ns | 12,309.05 ns |  6,437.87 ns |   477,254.8 ns |  1.02 |    0.02 |    6 |         - |          NA |
| BottomupMergeSort        | 8192 | ManyDuplicates     |   308,208.3 ns |  6,573.39 ns |  3,438.01 ns |   308,702.4 ns |  0.66 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 8192 | ManyDuplicates     |   236,621.1 ns |  8,448.07 ns |  4,418.50 ns |   237,034.5 ns |  0.51 |    0.01 |    4 |         - |          NA |
| RotateMergeSort          | 8192 | ManyDuplicates     |   950,482.7 ns |  6,316.11 ns |  3,303.45 ns |   949,982.3 ns |  2.03 |    0.03 |    8 |         - |          NA |
| RotateMergeSortRecursive | 8192 | ManyDuplicates     | 1,023,494.9 ns |  6,379.94 ns |  3,336.83 ns | 1,023,517.3 ns |  2.19 |    0.03 |    8 |         - |          NA |
| SymMergeSort             | 8192 | ManyDuplicates     |   766,342.4 ns |  2,684.48 ns |  1,191.92 ns |   766,533.5 ns |  1.64 |    0.03 |    7 |         - |          NA |
| BlockMergeSort           | 8192 | ManyDuplicates     |   541,186.5 ns |  4,654.84 ns |  2,434.57 ns |   540,758.3 ns |  1.16 |    0.02 |    6 |         - |          NA |
| NaturalMergeSort         | 8192 | ManyDuplicates     |   503,749.4 ns |  5,694.57 ns |  2,978.37 ns |   503,189.5 ns |  1.08 |    0.02 |    6 |         - |          NA |
| TimSort                  | 8192 | ManyDuplicates     |   386,131.3 ns |  7,748.46 ns |  4,052.59 ns |   385,962.4 ns |  0.83 |    0.01 |    5 |         - |          NA |
| PowerSort                | 8192 | ManyDuplicates     |   189,481.7 ns | 11,193.30 ns |  5,854.31 ns |   188,074.3 ns |  0.40 |    0.01 |    3 |         - |          NA |
| ShiftSort                | 8192 | ManyDuplicates     |   367,023.3 ns | 16,576.08 ns |  8,669.61 ns |   364,896.6 ns |  0.78 |    0.02 |    5 |         - |          NA |
| SpinSort                 | 8192 | ManyDuplicates     |   185,155.7 ns |  4,114.56 ns |  1,826.89 ns |   185,370.1 ns |  0.40 |    0.01 |    3 |         - |          NA |
| Glidesort                | 8192 | ManyDuplicates     |    91,343.7 ns |  1,449.96 ns |    758.36 ns |    91,508.0 ns |  0.20 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | ManyDuplicates     |    82,380.3 ns |    626.58 ns |    278.20 ns |    82,404.2 ns |  0.18 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | ManyDuplicates     |   151,741.5 ns |  4,005.42 ns |  1,778.43 ns |   151,649.9 ns |  0.32 |    0.01 |    2 |         - |          NA |

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
| **BitonicSort**             | **256**  | **Random**             |    **10,066.2 ns** |    **271.02 ns** |   **141.75 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Random             |    23,134.1 ns |    366.09 ns |   191.47 ns |  2.30 |    0.04 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Random             |    18,725.4 ns |    258.74 ns |   135.33 ns |  1.86 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **SingleElementMoved** |    **10,156.7 ns** |    **587.36 ns** |   **307.20 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | SingleElementMoved |    23,333.2 ns |    351.86 ns |   184.03 ns |  2.30 |    0.07 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | SingleElementMoved |    18,661.4 ns |    125.19 ns |    65.48 ns |  1.84 |    0.05 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **Sorted**             |     **9,705.5 ns** |    **920.00 ns** |   **481.18 ns** |  **1.00** |    **0.07** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Sorted             |    23,166.5 ns |    340.07 ns |   177.86 ns |  2.39 |    0.11 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Sorted             |    18,686.7 ns |    162.75 ns |    85.12 ns |  1.93 |    0.09 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **Reversed**           |     **9,750.1 ns** |    **198.97 ns** |    **88.34 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Reversed           |    23,214.2 ns |    292.77 ns |   153.13 ns |  2.38 |    0.03 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Reversed           |    18,623.4 ns |    142.78 ns |    74.68 ns |  1.91 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **PipeOrgan**          |     **9,971.9 ns** |    **632.04 ns** |   **330.57 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | PipeOrgan          |    23,212.1 ns |     75.37 ns |    33.47 ns |  2.33 |    0.07 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | PipeOrgan          |    18,614.1 ns |     46.93 ns |    24.55 ns |  1.87 |    0.06 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **ManyDuplicates**     |    **10,597.0 ns** |    **543.38 ns** |   **284.20 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | ManyDuplicates     |    23,046.1 ns |    243.58 ns |   108.15 ns |  2.18 |    0.06 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | ManyDuplicates     |    18,688.0 ns |    176.10 ns |    78.19 ns |  1.76 |    0.05 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Random**             |    **62,177.6 ns** |  **1,598.21 ns** |   **835.89 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Random             |   118,598.9 ns |    928.09 ns |   412.08 ns |  1.91 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Random             |   115,205.0 ns |    782.32 ns |   409.17 ns |  1.85 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **SingleElementMoved** |    **60,472.7 ns** |  **1,616.66 ns** |   **717.81 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | SingleElementMoved |   120,262.0 ns |    452.05 ns |   236.43 ns |  1.99 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | SingleElementMoved |   114,926.7 ns |    185.87 ns |    82.53 ns |  1.90 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Sorted**             |    **60,042.9 ns** |  **2,586.46 ns** | **1,148.40 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Sorted             |   119,433.6 ns |    823.45 ns |   430.68 ns |  1.99 |    0.04 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Sorted             |   115,088.2 ns |    196.63 ns |    87.30 ns |  1.92 |    0.04 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Reversed**           |    **59,388.7 ns** |  **1,407.00 ns** |   **735.89 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Reversed           |   120,935.7 ns |  3,380.31 ns | 1,767.97 ns |  2.04 |    0.04 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Reversed           |   115,131.3 ns |    225.18 ns |   117.77 ns |  1.94 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **PipeOrgan**          |    **59,468.1 ns** |  **1,160.45 ns** |   **515.25 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | PipeOrgan          |   119,984.9 ns |    660.65 ns |   293.33 ns |  2.02 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | PipeOrgan          |   114,955.2 ns |     74.81 ns |    33.22 ns |  1.93 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **ManyDuplicates**     |    **58,334.4 ns** |  **1,753.82 ns** |   **778.71 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | ManyDuplicates     |   117,418.9 ns |    555.14 ns |   246.49 ns |  2.01 |    0.03 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | ManyDuplicates     |   114,848.4 ns |    168.77 ns |    74.94 ns |  1.97 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Random**             |   **565,172.5 ns** |  **5,248.09 ns** | **2,744.85 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Random             |   829,386.0 ns |  2,521.30 ns | 1,318.69 ns |  1.47 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Random             |   684,149.8 ns |  1,450.18 ns |   758.47 ns |  1.21 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **SingleElementMoved** |   **342,578.3 ns** |  **6,149.96 ns** | **3,216.55 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | SingleElementMoved |   599,417.2 ns |  2,380.09 ns | 1,244.84 ns |  1.75 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | SingleElementMoved |   658,914.4 ns |    352.58 ns |   156.55 ns |  1.92 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Sorted**             |   **340,989.6 ns** |  **6,292.03 ns** | **2,793.70 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Sorted             |   592,534.4 ns |  1,331.98 ns |   696.65 ns |  1.74 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Sorted             |   658,750.8 ns |    625.36 ns |   277.66 ns |  1.93 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Reversed**           |   **337,227.6 ns** |  **1,788.00 ns** |   **793.88 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Reversed           |   597,276.4 ns |  1,056.58 ns |   552.61 ns |  1.77 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Reversed           |   659,237.9 ns |    433.25 ns |   192.37 ns |  1.95 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **PipeOrgan**          |   **340,051.5 ns** |  **5,005.32 ns** | **2,617.88 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | PipeOrgan          |   598,854.3 ns |    746.52 ns |   331.46 ns |  1.76 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | PipeOrgan          |   659,255.8 ns |    626.86 ns |   278.33 ns |  1.94 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **ManyDuplicates**     |   **456,250.6 ns** |  **2,237.01 ns** | **1,170.00 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | ManyDuplicates     |   708,717.0 ns |  2,764.04 ns | 1,445.65 ns |  1.55 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | ManyDuplicates     |   661,095.6 ns |  1,096.58 ns |   573.53 ns |  1.45 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **8192** | **Random**             | **1,318,137.0 ns** |  **7,632.93 ns** | **3,389.07 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | Random             | 1,952,984.2 ns |  1,501.85 ns |   666.83 ns |  1.48 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | Random             | 1,680,105.4 ns |  2,297.36 ns | 1,201.56 ns |  1.27 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **8192** | **SingleElementMoved** |   **795,647.5 ns** |  **3,400.32 ns** | **1,509.76 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | SingleElementMoved | 1,350,533.1 ns |  2,035.70 ns | 1,064.71 ns |  1.70 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | SingleElementMoved | 1,542,233.2 ns |  1,595.43 ns |   834.44 ns |  1.94 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **8192** | **Sorted**             |   **778,513.5 ns** | **14,655.46 ns** | **7,665.09 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | Sorted             | 1,333,983.7 ns |  2,982.51 ns | 1,559.91 ns |  1.71 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | Sorted             | 1,541,790.0 ns |  1,120.80 ns |   586.20 ns |  1.98 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **8192** | **Reversed**           |   **784,201.7 ns** |  **6,338.77 ns** | **3,315.30 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | Reversed           | 1,348,104.0 ns |  2,671.73 ns | 1,397.37 ns |  1.72 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | Reversed           | 1,542,785.2 ns |  1,243.73 ns |   650.50 ns |  1.97 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **8192** | **PipeOrgan**          |   **787,347.4 ns** |  **9,355.23 ns** | **4,892.97 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | PipeOrgan          | 1,349,286.5 ns |  1,796.71 ns |   939.71 ns |  1.71 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | PipeOrgan          | 1,542,012.0 ns |    899.75 ns |   470.59 ns |  1.96 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **8192** | **ManyDuplicates**     | **1,063,936.8 ns** |  **9,731.89 ns** | **5,089.97 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | ManyDuplicates     | 1,684,591.0 ns |  3,449.90 ns | 1,804.37 ns |  1.58 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | ManyDuplicates     | 1,595,267.7 ns |  7,068.46 ns | 3,696.94 ns |  1.50 |    0.01 |    2 |         - |          NA |

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

| Method                       | Size | Pattern            | Mean           | Error        | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------------- |----- |------------------- |---------------:|-------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **QuickSort**                    | **256**  | **Random**             |     **2,708.5 ns** |    **153.48 ns** |    **68.15 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 256  | Random             |     2,434.8 ns |     84.50 ns |    37.52 ns |  0.90 |    0.02 |    1 |         - |          NA |
| QuickSortMedian3             | 256  | Random             |     2,812.6 ns |    157.62 ns |    69.98 ns |  1.04 |    0.03 |    1 |         - |          NA |
| QuickSortMedian9             | 256  | Random             |     3,181.5 ns |    229.11 ns |   119.83 ns |  1.18 |    0.05 |    1 |         - |          NA |
| DualPivotQuickSort           | 256  | Random             |     2,261.3 ns |    204.88 ns |    90.97 ns |  0.84 |    0.04 |    1 |         - |          NA |
| StableQuickSort              | 256  | Random             |    11,328.8 ns |    158.82 ns |    83.07 ns |  4.18 |    0.10 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Random             |     7,553.9 ns |     75.29 ns |    33.43 ns |  2.79 |    0.07 |    2 |         - |          NA |
| DestswapStableQuickSort      | 256  | Random             |     6,912.0 ns |    189.52 ns |    99.12 ns |  2.55 |    0.07 |    2 |         - |          NA |
| IntroSort                    | 256  | Random             |     2,349.2 ns |    251.20 ns |   111.54 ns |  0.87 |    0.04 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | Random             |     1,859.8 ns |    309.25 ns |   137.31 ns |  0.69 |    0.05 |    1 |         - |          NA |
| PDQSort                      | 256  | Random             |     1,822.6 ns |     78.48 ns |    41.05 ns |  0.67 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | Random             |     2,793.9 ns |     41.69 ns |    18.51 ns |  1.03 |    0.03 |    1 |         - |          NA |
| Ipnsort                      | 256  | Random             |     3,676.6 ns |     75.02 ns |    33.31 ns |  1.36 |    0.03 |    1 |         - |          NA |
| StdSort                      | 256  | Random             |     2,768.1 ns |     30.99 ns |    11.05 ns |  1.02 |    0.02 |    1 |         - |          NA |
| BlockQuickSort               | 256  | Random             |     2,853.2 ns |     71.50 ns |    25.50 ns |  1.05 |    0.03 |    1 |         - |          NA |
| DotnetSort                   | 256  | Random             |     1,996.6 ns |     24.73 ns |     8.82 ns |  0.74 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **256**  | **SingleElementMoved** |     **1,210.7 ns** |    **100.01 ns** |    **44.40 ns** |  **1.00** |    **0.05** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 256  | SingleElementMoved |     1,041.0 ns |     29.48 ns |    13.09 ns |  0.86 |    0.03 |    1 |         - |          NA |
| QuickSortMedian3             | 256  | SingleElementMoved |     1,721.2 ns |     23.35 ns |    10.37 ns |  1.42 |    0.05 |    2 |         - |          NA |
| QuickSortMedian9             | 256  | SingleElementMoved |     2,204.9 ns |     18.95 ns |     8.41 ns |  1.82 |    0.06 |    3 |         - |          NA |
| DualPivotQuickSort           | 256  | SingleElementMoved |       864.9 ns |     50.64 ns |    22.48 ns |  0.72 |    0.03 |    1 |         - |          NA |
| StableQuickSort              | 256  | SingleElementMoved |     8,772.6 ns |    726.79 ns |   380.12 ns |  7.25 |    0.38 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 256  | SingleElementMoved |     5,309.2 ns |    391.57 ns |   204.80 ns |  4.39 |    0.22 |    5 |         - |          NA |
| DestswapStableQuickSort      | 256  | SingleElementMoved |     4,288.7 ns |    298.25 ns |   132.43 ns |  3.55 |    0.16 |    4 |         - |          NA |
| IntroSort                    | 256  | SingleElementMoved |       864.9 ns |     28.59 ns |    10.20 ns |  0.72 |    0.03 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | SingleElementMoved |     1,124.4 ns |     16.02 ns |     7.11 ns |  0.93 |    0.03 |    1 |         - |          NA |
| PDQSort                      | 256  | SingleElementMoved |     1,134.1 ns |     19.70 ns |    10.31 ns |  0.94 |    0.03 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | SingleElementMoved |     1,450.2 ns |     96.13 ns |    42.68 ns |  1.20 |    0.05 |    2 |         - |          NA |
| Ipnsort                      | 256  | SingleElementMoved |     3,703.3 ns |    346.75 ns |   181.36 ns |  3.06 |    0.17 |    4 |         - |          NA |
| StdSort                      | 256  | SingleElementMoved |     1,741.6 ns |      6.95 ns |     3.09 ns |  1.44 |    0.05 |    2 |         - |          NA |
| BlockQuickSort               | 256  | SingleElementMoved |     1,782.9 ns |     22.87 ns |    10.16 ns |  1.47 |    0.05 |    2 |         - |          NA |
| DotnetSort                   | 256  | SingleElementMoved |       998.1 ns |     25.80 ns |    13.49 ns |  0.83 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Sorted**             |       **808.2 ns** |     **21.32 ns** |     **9.46 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 256  | Sorted             |       732.6 ns |      5.16 ns |     2.70 ns |  0.91 |    0.01 |    3 |         - |          NA |
| QuickSortMedian3             | 256  | Sorted             |     1,210.7 ns |    103.85 ns |    46.11 ns |  1.50 |    0.06 |    4 |         - |          NA |
| QuickSortMedian9             | 256  | Sorted             |     1,362.7 ns |    186.47 ns |    82.79 ns |  1.69 |    0.10 |    4 |         - |          NA |
| DualPivotQuickSort           | 256  | Sorted             |       759.3 ns |    301.30 ns |   133.78 ns |  0.94 |    0.16 |    3 |         - |          NA |
| StableQuickSort              | 256  | Sorted             |     9,167.5 ns |    347.53 ns |   181.76 ns | 11.34 |    0.25 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Sorted             |     4,813.2 ns |    459.49 ns |   240.32 ns |  5.96 |    0.29 |    5 |         - |          NA |
| DestswapStableQuickSort      | 256  | Sorted             |     4,056.8 ns |     63.76 ns |    22.74 ns |  5.02 |    0.06 |    5 |         - |          NA |
| IntroSort                    | 256  | Sorted             |       361.7 ns |     59.60 ns |    31.17 ns |  0.45 |    0.04 |    2 |         - |          NA |
| IntroSortDotnet              | 256  | Sorted             |       962.4 ns |     12.68 ns |     6.63 ns |  1.19 |    0.02 |    3 |         - |          NA |
| PDQSort                      | 256  | Sorted             |       344.0 ns |     14.72 ns |     7.70 ns |  0.43 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 256  | Sorted             |       357.7 ns |      2.56 ns |     0.91 ns |  0.44 |    0.00 |    2 |         - |          NA |
| Ipnsort                      | 256  | Sorted             |       166.6 ns |      1.17 ns |     0.52 ns |  0.21 |    0.00 |    1 |         - |          NA |
| StdSort                      | 256  | Sorted             |       353.4 ns |      2.38 ns |     1.06 ns |  0.44 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 256  | Sorted             |     1,391.4 ns |     11.19 ns |     5.85 ns |  1.72 |    0.02 |    4 |         - |          NA |
| DotnetSort                   | 256  | Sorted             |       803.0 ns |     13.74 ns |     6.10 ns |  0.99 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Reversed**           |     **1,006.5 ns** |     **19.75 ns** |     **7.04 ns** |  **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 256  | Reversed           |       962.6 ns |      9.97 ns |     4.43 ns |  0.96 |    0.01 |    4 |         - |          NA |
| QuickSortMedian3             | 256  | Reversed           |     1,300.4 ns |    106.56 ns |    55.73 ns |  1.29 |    0.05 |    4 |         - |          NA |
| QuickSortMedian9             | 256  | Reversed           |     1,445.0 ns |     20.44 ns |    10.69 ns |  1.44 |    0.01 |    4 |         - |          NA |
| DualPivotQuickSort           | 256  | Reversed           |     1,025.4 ns |     39.78 ns |    20.80 ns |  1.02 |    0.02 |    4 |         - |          NA |
| StableQuickSort              | 256  | Reversed           |     8,421.0 ns |     44.13 ns |    19.59 ns |  8.37 |    0.06 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Reversed           |     5,108.2 ns |    396.54 ns |   207.40 ns |  5.08 |    0.20 |    5 |         - |          NA |
| DestswapStableQuickSort      | 256  | Reversed           |     7,290.1 ns |     20.00 ns |     8.88 ns |  7.24 |    0.05 |    6 |         - |          NA |
| IntroSort                    | 256  | Reversed           |       662.9 ns |    181.07 ns |    80.40 ns |  0.66 |    0.07 |    3 |         - |          NA |
| IntroSortDotnet              | 256  | Reversed           |     1,564.7 ns |    200.89 ns |    71.64 ns |  1.55 |    0.07 |    4 |         - |          NA |
| PDQSort                      | 256  | Reversed           |       554.8 ns |      4.63 ns |     1.65 ns |  0.55 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 256  | Reversed           |       906.5 ns |     10.66 ns |     3.80 ns |  0.90 |    0.01 |    4 |         - |          NA |
| Ipnsort                      | 256  | Reversed           |       233.8 ns |      2.66 ns |     0.95 ns |  0.23 |    0.00 |    1 |         - |          NA |
| StdSort                      | 256  | Reversed           |       841.9 ns |    110.16 ns |    48.91 ns |  0.84 |    0.05 |    4 |         - |          NA |
| BlockQuickSort               | 256  | Reversed           |     1,644.5 ns |     14.40 ns |     6.39 ns |  1.63 |    0.01 |    4 |         - |          NA |
| DotnetSort                   | 256  | Reversed           |     1,247.0 ns |    169.61 ns |    75.31 ns |  1.24 |    0.07 |    4 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **256**  | **PipeOrgan**          |     **7,451.7 ns** |    **306.44 ns** |   **136.06 ns** |  **1.00** |    **0.02** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 256  | PipeOrgan          |     2,482.3 ns |    108.33 ns |    48.10 ns |  0.33 |    0.01 |    2 |         - |          NA |
| QuickSortMedian3             | 256  | PipeOrgan          |     3,270.9 ns |    440.20 ns |   230.23 ns |  0.44 |    0.03 |    2 |         - |          NA |
| QuickSortMedian9             | 256  | PipeOrgan          |     2,598.2 ns |    102.40 ns |    53.56 ns |  0.35 |    0.01 |    2 |         - |          NA |
| DualPivotQuickSort           | 256  | PipeOrgan          |     1,537.4 ns |     37.53 ns |    19.63 ns |  0.21 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 256  | PipeOrgan          |     8,667.8 ns |    242.96 ns |   127.07 ns |  1.16 |    0.03 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 256  | PipeOrgan          |     5,331.9 ns |    376.28 ns |   196.80 ns |  0.72 |    0.03 |    3 |         - |          NA |
| DestswapStableQuickSort      | 256  | PipeOrgan          |     7,668.3 ns |    562.32 ns |   294.11 ns |  1.03 |    0.04 |    4 |         - |          NA |
| IntroSort                    | 256  | PipeOrgan          |     1,551.8 ns |    221.79 ns |   116.00 ns |  0.21 |    0.02 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | PipeOrgan          |     2,135.7 ns |     89.88 ns |    32.05 ns |  0.29 |    0.01 |    2 |         - |          NA |
| PDQSort                      | 256  | PipeOrgan          |     1,695.2 ns |     48.73 ns |    21.64 ns |  0.23 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | PipeOrgan          |     2,877.3 ns |    198.18 ns |    87.99 ns |  0.39 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 256  | PipeOrgan          |     3,983.1 ns |    366.63 ns |   191.75 ns |  0.53 |    0.03 |    3 |         - |          NA |
| StdSort                      | 256  | PipeOrgan          |     4,549.1 ns |     51.53 ns |    18.38 ns |  0.61 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 256  | PipeOrgan          |     5,044.7 ns |    380.76 ns |   199.14 ns |  0.68 |    0.03 |    3 |         - |          NA |
| DotnetSort                   | 256  | PipeOrgan          |     2,496.1 ns |     24.96 ns |     8.90 ns |  0.34 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **256**  | **ManyDuplicates**     |     **2,354.5 ns** |    **250.55 ns** |   **131.04 ns** |  **1.00** |    **0.07** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 256  | ManyDuplicates     |     1,819.6 ns |    127.27 ns |    66.56 ns |  0.77 |    0.05 |    1 |         - |          NA |
| QuickSortMedian3             | 256  | ManyDuplicates     |     2,814.3 ns |    319.75 ns |   167.24 ns |  1.20 |    0.09 |    1 |         - |          NA |
| QuickSortMedian9             | 256  | ManyDuplicates     |     2,788.8 ns |    107.74 ns |    47.84 ns |  1.19 |    0.06 |    1 |         - |          NA |
| DualPivotQuickSort           | 256  | ManyDuplicates     |     1,918.2 ns |    110.93 ns |    49.25 ns |  0.82 |    0.05 |    1 |         - |          NA |
| StableQuickSort              | 256  | ManyDuplicates     |     6,742.8 ns |    327.99 ns |   171.54 ns |  2.87 |    0.16 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 256  | ManyDuplicates     |     3,646.7 ns |     48.16 ns |    17.17 ns |  1.55 |    0.08 |    2 |         - |          NA |
| DestswapStableQuickSort      | 256  | ManyDuplicates     |     5,383.4 ns |    430.52 ns |   225.17 ns |  2.29 |    0.15 |    3 |         - |          NA |
| IntroSort                    | 256  | ManyDuplicates     |     2,191.5 ns |    151.52 ns |    79.25 ns |  0.93 |    0.06 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | ManyDuplicates     |     1,657.1 ns |     14.83 ns |     5.29 ns |  0.71 |    0.04 |    1 |         - |          NA |
| PDQSort                      | 256  | ManyDuplicates     |     1,636.3 ns |     65.65 ns |    29.15 ns |  0.70 |    0.04 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | ManyDuplicates     |     2,536.8 ns |    198.37 ns |    88.08 ns |  1.08 |    0.06 |    1 |         - |          NA |
| Ipnsort                      | 256  | ManyDuplicates     |     3,684.0 ns |     53.98 ns |    19.25 ns |  1.57 |    0.08 |    2 |         - |          NA |
| StdSort                      | 256  | ManyDuplicates     |     2,612.9 ns |    107.52 ns |    47.74 ns |  1.11 |    0.06 |    1 |         - |          NA |
| BlockQuickSort               | 256  | ManyDuplicates     |     2,552.5 ns |     81.79 ns |    36.31 ns |  1.09 |    0.06 |    1 |         - |          NA |
| DotnetSort                   | 256  | ManyDuplicates     |     1,732.4 ns |     14.86 ns |     5.30 ns |  0.74 |    0.04 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Random**             |    **13,621.7 ns** |    **311.95 ns** |   **138.51 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Random             |    12,910.7 ns |    432.83 ns |   226.38 ns |  0.95 |    0.02 |    1 |         - |          NA |
| QuickSortMedian3             | 1024 | Random             |    13,614.3 ns |    635.55 ns |   332.40 ns |  1.00 |    0.02 |    1 |         - |          NA |
| QuickSortMedian9             | 1024 | Random             |    14,830.5 ns |    451.15 ns |   235.96 ns |  1.09 |    0.02 |    1 |         - |          NA |
| DualPivotQuickSort           | 1024 | Random             |    11,305.5 ns |    520.38 ns |   272.17 ns |  0.83 |    0.02 |    1 |         - |          NA |
| StableQuickSort              | 1024 | Random             |    58,611.5 ns |    958.00 ns |   501.05 ns |  4.30 |    0.05 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Random             |    36,361.2 ns |    608.87 ns |   270.34 ns |  2.67 |    0.03 |    3 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Random             |    31,497.9 ns |    831.09 ns |   434.68 ns |  2.31 |    0.04 |    3 |         - |          NA |
| IntroSort                    | 1024 | Random             |    12,193.5 ns |    380.43 ns |   168.91 ns |  0.90 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | Random             |     9,902.3 ns |    410.59 ns |   214.75 ns |  0.73 |    0.02 |    1 |         - |          NA |
| PDQSort                      | 1024 | Random             |     9,531.9 ns |    561.47 ns |   293.66 ns |  0.70 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | Random             |    13,097.1 ns |    328.02 ns |   145.64 ns |  0.96 |    0.01 |    1 |         - |          NA |
| Ipnsort                      | 1024 | Random             |    19,217.0 ns |    254.40 ns |   112.96 ns |  1.41 |    0.02 |    2 |         - |          NA |
| StdSort                      | 1024 | Random             |    13,407.2 ns |    444.77 ns |   158.61 ns |  0.98 |    0.01 |    1 |         - |          NA |
| BlockQuickSort               | 1024 | Random             |    14,063.7 ns |    205.72 ns |    91.34 ns |  1.03 |    0.01 |    1 |         - |          NA |
| DotnetSort                   | 1024 | Random             |    11,084.4 ns |    485.15 ns |   253.74 ns |  0.81 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **SingleElementMoved** |     **5,652.1 ns** |    **334.61 ns** |   **175.01 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 1024 | SingleElementMoved |     5,529.0 ns |    372.31 ns |   194.73 ns |  0.98 |    0.04 |    1 |         - |          NA |
| QuickSortMedian3             | 1024 | SingleElementMoved |     8,006.3 ns |    146.85 ns |    65.20 ns |  1.42 |    0.04 |    2 |         - |          NA |
| QuickSortMedian9             | 1024 | SingleElementMoved |    10,814.6 ns |    330.28 ns |   172.74 ns |  1.92 |    0.06 |    2 |         - |          NA |
| DualPivotQuickSort           | 1024 | SingleElementMoved |     4,392.3 ns |     74.77 ns |    26.66 ns |  0.78 |    0.02 |    1 |         - |          NA |
| StableQuickSort              | 1024 | SingleElementMoved |    43,160.7 ns |    185.32 ns |    96.92 ns |  7.64 |    0.23 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | SingleElementMoved |    27,060.3 ns |    528.28 ns |   276.30 ns |  4.79 |    0.15 |    4 |         - |          NA |
| DestswapStableQuickSort      | 1024 | SingleElementMoved |    20,254.6 ns |    981.68 ns |   513.44 ns |  3.59 |    0.14 |    3 |         - |          NA |
| IntroSort                    | 1024 | SingleElementMoved |     4,112.7 ns |    318.32 ns |   166.49 ns |  0.73 |    0.04 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | SingleElementMoved |     5,778.1 ns |    357.54 ns |   187.00 ns |  1.02 |    0.04 |    1 |         - |          NA |
| PDQSort                      | 1024 | SingleElementMoved |     4,939.3 ns |    194.27 ns |    86.26 ns |  0.87 |    0.03 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | SingleElementMoved |     6,146.1 ns |    301.11 ns |   157.49 ns |  1.09 |    0.04 |    1 |         - |          NA |
| Ipnsort                      | 1024 | SingleElementMoved |    17,982.6 ns |     94.98 ns |    49.68 ns |  3.18 |    0.09 |    3 |         - |          NA |
| StdSort                      | 1024 | SingleElementMoved |     7,964.5 ns |    133.73 ns |    69.94 ns |  1.41 |    0.04 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | SingleElementMoved |     9,321.5 ns |    373.52 ns |   195.36 ns |  1.65 |    0.06 |    2 |         - |          NA |
| DotnetSort                   | 1024 | SingleElementMoved |     5,494.8 ns |    273.19 ns |   142.88 ns |  0.97 |    0.04 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Sorted**             |     **4,138.2 ns** |    **374.06 ns** |   **195.64 ns** |  **1.00** |    **0.06** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Sorted             |     4,281.4 ns |    921.31 ns |   481.86 ns |  1.04 |    0.12 |    3 |         - |          NA |
| QuickSortMedian3             | 1024 | Sorted             |     5,624.0 ns |    357.38 ns |   186.92 ns |  1.36 |    0.07 |    3 |         - |          NA |
| QuickSortMedian9             | 1024 | Sorted             |     6,122.9 ns |    327.77 ns |   171.43 ns |  1.48 |    0.08 |    3 |         - |          NA |
| DualPivotQuickSort           | 1024 | Sorted             |     3,792.5 ns |    243.50 ns |   108.12 ns |  0.92 |    0.05 |    3 |         - |          NA |
| StableQuickSort              | 1024 | Sorted             |    47,639.4 ns |  2,084.32 ns | 1,090.14 ns | 11.53 |    0.56 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Sorted             |    22,706.6 ns |    789.53 ns |   350.55 ns |  5.50 |    0.25 |    5 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Sorted             |    21,842.0 ns |    718.29 ns |   375.68 ns |  5.29 |    0.24 |    5 |         - |          NA |
| IntroSort                    | 1024 | Sorted             |     1,030.2 ns |     29.61 ns |    13.14 ns |  0.25 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 1024 | Sorted             |     4,766.6 ns |    342.03 ns |   151.86 ns |  1.15 |    0.06 |    3 |         - |          NA |
| PDQSort                      | 1024 | Sorted             |     1,334.7 ns |     25.47 ns |    11.31 ns |  0.32 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | Sorted             |     1,327.6 ns |      7.39 ns |     3.28 ns |  0.32 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 1024 | Sorted             |       583.7 ns |      3.17 ns |     1.13 ns |  0.14 |    0.01 |    1 |         - |          NA |
| StdSort                      | 1024 | Sorted             |     1,217.0 ns |     42.44 ns |    15.13 ns |  0.29 |    0.01 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | Sorted             |     7,741.5 ns |    152.74 ns |    67.82 ns |  1.87 |    0.08 |    4 |         - |          NA |
| DotnetSort                   | 1024 | Sorted             |     4,082.3 ns |    336.38 ns |   149.35 ns |  0.99 |    0.05 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Reversed**           |     **4,839.3 ns** |    **492.33 ns** |   **218.60 ns** |  **1.00** |    **0.06** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Reversed           |     4,808.5 ns |    548.66 ns |   286.96 ns |  1.00 |    0.07 |    4 |         - |          NA |
| QuickSortMedian3             | 1024 | Reversed           |     5,909.8 ns |    270.06 ns |   141.24 ns |  1.22 |    0.06 |    4 |         - |          NA |
| QuickSortMedian9             | 1024 | Reversed           |     6,400.3 ns |    277.96 ns |   145.38 ns |  1.32 |    0.06 |    4 |         - |          NA |
| DualPivotQuickSort           | 1024 | Reversed           |     4,902.2 ns |    336.07 ns |   149.22 ns |  1.01 |    0.05 |    4 |         - |          NA |
| StableQuickSort              | 1024 | Reversed           |    42,815.8 ns |    274.38 ns |   121.82 ns |  8.86 |    0.37 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Reversed           |    24,365.2 ns |    366.84 ns |   162.88 ns |  5.04 |    0.21 |    5 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Reversed           |    32,846.9 ns |    665.14 ns |   347.88 ns |  6.80 |    0.29 |    6 |         - |          NA |
| IntroSort                    | 1024 | Reversed           |     2,979.4 ns |     26.71 ns |    11.86 ns |  0.62 |    0.03 |    3 |         - |          NA |
| IntroSortDotnet              | 1024 | Reversed           |     7,212.3 ns |    377.80 ns |   197.60 ns |  1.49 |    0.07 |    4 |         - |          NA |
| PDQSort                      | 1024 | Reversed           |     2,260.8 ns |    546.40 ns |   242.61 ns |  0.47 |    0.05 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | Reversed           |     3,380.3 ns |    369.74 ns |   193.38 ns |  0.70 |    0.05 |    3 |         - |          NA |
| Ipnsort                      | 1024 | Reversed           |       909.4 ns |      4.26 ns |     1.89 ns |  0.19 |    0.01 |    1 |         - |          NA |
| StdSort                      | 1024 | Reversed           |     2,932.6 ns |     11.55 ns |     5.13 ns |  0.61 |    0.03 |    3 |         - |          NA |
| BlockQuickSort               | 1024 | Reversed           |     8,531.6 ns |    292.63 ns |   153.05 ns |  1.77 |    0.08 |    4 |         - |          NA |
| DotnetSort                   | 1024 | Reversed           |     6,832.2 ns |    929.57 ns |   412.74 ns |  1.41 |    0.10 |    4 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **PipeOrgan**          |   **108,720.7 ns** |    **538.92 ns** |   **239.28 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| QuickSort3way                | 1024 | PipeOrgan          |    16,169.4 ns |  2,343.69 ns | 1,225.79 ns |  0.15 |    0.01 |    2 |         - |          NA |
| QuickSortMedian3             | 1024 | PipeOrgan          |    16,024.9 ns |    507.38 ns |   265.37 ns |  0.15 |    0.00 |    2 |         - |          NA |
| QuickSortMedian9             | 1024 | PipeOrgan          |    12,069.5 ns |    399.64 ns |   209.02 ns |  0.11 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort           | 1024 | PipeOrgan          |     8,336.6 ns |  1,139.99 ns |   596.24 ns |  0.08 |    0.01 |    1 |         - |          NA |
| StableQuickSort              | 1024 | PipeOrgan          |    43,090.1 ns |    556.77 ns |   291.20 ns |  0.40 |    0.00 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | PipeOrgan          |    25,123.1 ns |    896.57 ns |   398.08 ns |  0.23 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 1024 | PipeOrgan          |    35,832.1 ns |    327.89 ns |   171.49 ns |  0.33 |    0.00 |    4 |         - |          NA |
| IntroSort                    | 1024 | PipeOrgan          |    12,733.3 ns |  2,397.16 ns | 1,253.76 ns |  0.12 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 1024 | PipeOrgan          |    14,305.6 ns |     71.03 ns |    31.54 ns |  0.13 |    0.00 |    2 |         - |          NA |
| PDQSort                      | 1024 | PipeOrgan          |     8,921.6 ns |    332.78 ns |   174.05 ns |  0.08 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | PipeOrgan          |    15,326.3 ns |    220.01 ns |    97.69 ns |  0.14 |    0.00 |    2 |         - |          NA |
| Ipnsort                      | 1024 | PipeOrgan          |    21,052.6 ns |    296.88 ns |   155.28 ns |  0.19 |    0.00 |    3 |         - |          NA |
| StdSort                      | 1024 | PipeOrgan          |    21,582.1 ns |    360.81 ns |   188.71 ns |  0.20 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 1024 | PipeOrgan          |    24,636.9 ns |    141.37 ns |    73.94 ns |  0.23 |    0.00 |    3 |         - |          NA |
| DotnetSort                   | 1024 | PipeOrgan          |    16,505.1 ns |    969.98 ns |   507.32 ns |  0.15 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **ManyDuplicates**     |     **9,732.0 ns** |    **741.36 ns** |   **329.17 ns** |  **1.00** |    **0.04** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 1024 | ManyDuplicates     |     7,932.1 ns |    226.44 ns |   118.43 ns |  0.82 |    0.03 |    2 |         - |          NA |
| QuickSortMedian3             | 1024 | ManyDuplicates     |    11,916.1 ns |    398.48 ns |   208.41 ns |  1.23 |    0.04 |    2 |         - |          NA |
| QuickSortMedian9             | 1024 | ManyDuplicates     |    12,814.6 ns |    593.70 ns |   310.52 ns |  1.32 |    0.05 |    2 |         - |          NA |
| DualPivotQuickSort           | 1024 | ManyDuplicates     |     7,996.0 ns |    235.77 ns |   104.68 ns |  0.82 |    0.03 |    2 |         - |          NA |
| StableQuickSort              | 1024 | ManyDuplicates     |    29,335.8 ns |    672.50 ns |   298.59 ns |  3.02 |    0.10 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | ManyDuplicates     |    14,136.8 ns |    265.07 ns |   117.69 ns |  1.45 |    0.05 |    2 |         - |          NA |
| DestswapStableQuickSort      | 1024 | ManyDuplicates     |    14,524.9 ns |    866.61 ns |   453.25 ns |  1.49 |    0.06 |    2 |         - |          NA |
| IntroSort                    | 1024 | ManyDuplicates     |    10,545.6 ns |    338.81 ns |   177.21 ns |  1.08 |    0.04 |    2 |         - |          NA |
| IntroSortDotnet              | 1024 | ManyDuplicates     |     8,200.2 ns |    201.91 ns |    89.65 ns |  0.84 |    0.03 |    2 |         - |          NA |
| PDQSort                      | 1024 | ManyDuplicates     |     6,075.3 ns |    322.70 ns |   168.78 ns |  0.62 |    0.03 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | ManyDuplicates     |     8,874.2 ns |    369.24 ns |   193.12 ns |  0.91 |    0.03 |    2 |         - |          NA |
| Ipnsort                      | 1024 | ManyDuplicates     |    18,079.4 ns |    146.75 ns |    76.75 ns |  1.86 |    0.06 |    3 |         - |          NA |
| StdSort                      | 1024 | ManyDuplicates     |    11,299.1 ns |    501.52 ns |   262.31 ns |  1.16 |    0.04 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | ManyDuplicates     |    12,280.5 ns |    154.06 ns |    68.40 ns |  1.26 |    0.04 |    2 |         - |          NA |
| DotnetSort                   | 1024 | ManyDuplicates     |     8,542.0 ns |    323.80 ns |   169.35 ns |  0.88 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Random**             |    **71,259.3 ns** |  **6,053.97 ns** | **3,166.34 ns** |  **1.00** |    **0.06** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Random             |    74,968.4 ns | 16,747.84 ns | 8,759.44 ns |  1.05 |    0.12 |    1 |         - |          NA |
| QuickSortMedian3             | 4096 | Random             |    65,164.5 ns |  1,852.69 ns |   822.61 ns |  0.92 |    0.04 |    1 |         - |          NA |
| QuickSortMedian9             | 4096 | Random             |    68,183.1 ns |  1,225.55 ns |   544.15 ns |  0.96 |    0.04 |    1 |         - |          NA |
| DualPivotQuickSort           | 4096 | Random             |    54,684.3 ns |  1,313.91 ns |   583.38 ns |  0.77 |    0.03 |    1 |         - |          NA |
| StableQuickSort              | 4096 | Random             |   570,729.2 ns |  3,742.36 ns | 1,957.33 ns |  8.02 |    0.34 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Random             |   435,075.5 ns |  2,255.30 ns | 1,001.37 ns |  6.12 |    0.26 |    4 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Random             |   159,435.3 ns |  9,304.89 ns | 4,131.43 ns |  2.24 |    0.11 |    3 |         - |          NA |
| IntroSort                    | 4096 | Random             |    62,797.2 ns |  2,583.07 ns | 1,146.90 ns |  0.88 |    0.04 |    1 |         - |          NA |
| IntroSortDotnet              | 4096 | Random             |    48,515.1 ns |  1,221.52 ns |   542.36 ns |  0.68 |    0.03 |    1 |         - |          NA |
| PDQSort                      | 4096 | Random             |    45,323.9 ns |    809.63 ns |   359.48 ns |  0.64 |    0.03 |    1 |         - |          NA |
| PDQSortBranchless            | 4096 | Random             |    62,538.8 ns |  1,289.55 ns |   674.46 ns |  0.88 |    0.04 |    1 |         - |          NA |
| Ipnsort                      | 4096 | Random             |    97,897.0 ns |    688.54 ns |   305.72 ns |  1.38 |    0.06 |    2 |         - |          NA |
| StdSort                      | 4096 | Random             |    62,955.4 ns |    925.13 ns |   483.86 ns |  0.89 |    0.04 |    1 |         - |          NA |
| BlockQuickSort               | 4096 | Random             |    69,068.2 ns |    883.86 ns |   392.44 ns |  0.97 |    0.04 |    1 |         - |          NA |
| DotnetSort                   | 4096 | Random             |    53,156.4 ns |  1,110.46 ns |   493.05 ns |  0.75 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **4096** | **SingleElementMoved** |    **25,144.3 ns** |    **716.81 ns** |   **318.27 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 4096 | SingleElementMoved |    26,757.8 ns |  2,161.93 ns |   959.91 ns |  1.06 |    0.04 |    1 |         - |          NA |
| QuickSortMedian3             | 4096 | SingleElementMoved |    35,596.5 ns |  1,118.75 ns |   496.73 ns |  1.42 |    0.02 |    1 |         - |          NA |
| QuickSortMedian9             | 4096 | SingleElementMoved |    47,805.2 ns |    653.65 ns |   341.87 ns |  1.90 |    0.03 |    2 |         - |          NA |
| DualPivotQuickSort           | 4096 | SingleElementMoved |    23,003.5 ns |    944.56 ns |   494.02 ns |  0.91 |    0.02 |    1 |         - |          NA |
| StableQuickSort              | 4096 | SingleElementMoved |   207,756.9 ns |  1,258.15 ns |   558.63 ns |  8.26 |    0.10 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | SingleElementMoved |   122,989.0 ns |    643.34 ns |   336.48 ns |  4.89 |    0.06 |    4 |         - |          NA |
| DestswapStableQuickSort      | 4096 | SingleElementMoved |    96,796.3 ns |  1,121.76 ns |   586.70 ns |  3.85 |    0.05 |    3 |         - |          NA |
| IntroSort                    | 4096 | SingleElementMoved |    19,212.2 ns |  1,680.61 ns |   746.20 ns |  0.76 |    0.03 |    1 |         - |          NA |
| IntroSortDotnet              | 4096 | SingleElementMoved |    27,584.7 ns |    172.56 ns |    90.25 ns |  1.10 |    0.01 |    1 |         - |          NA |
| PDQSort                      | 4096 | SingleElementMoved |    21,217.9 ns |    257.89 ns |   114.50 ns |  0.84 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 4096 | SingleElementMoved |    26,302.5 ns |    738.45 ns |   386.23 ns |  1.05 |    0.02 |    1 |         - |          NA |
| Ipnsort                      | 4096 | SingleElementMoved |    87,163.9 ns |    252.49 ns |   112.11 ns |  3.47 |    0.04 |    3 |         - |          NA |
| StdSort                      | 4096 | SingleElementMoved |    32,636.1 ns |    647.43 ns |   338.62 ns |  1.30 |    0.02 |    1 |         - |          NA |
| BlockQuickSort               | 4096 | SingleElementMoved |    44,302.2 ns |    793.79 ns |   352.45 ns |  1.76 |    0.02 |    2 |         - |          NA |
| DotnetSort                   | 4096 | SingleElementMoved |    27,224.6 ns |  1,028.30 ns |   456.57 ns |  1.08 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Sorted**             |    **19,634.9 ns** |    **759.90 ns** |   **337.40 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Sorted             |    18,861.6 ns |  2,066.83 ns |   917.69 ns |  0.96 |    0.05 |    3 |         - |          NA |
| QuickSortMedian3             | 4096 | Sorted             |    25,278.9 ns |    266.37 ns |   118.27 ns |  1.29 |    0.02 |    3 |         - |          NA |
| QuickSortMedian9             | 4096 | Sorted             |    27,553.3 ns |    613.01 ns |   320.61 ns |  1.40 |    0.03 |    3 |         - |          NA |
| DualPivotQuickSort           | 4096 | Sorted             |    20,294.6 ns |    959.80 ns |   502.00 ns |  1.03 |    0.03 |    3 |         - |          NA |
| StableQuickSort              | 4096 | Sorted             |   226,507.1 ns |  1,766.92 ns |   924.13 ns | 11.54 |    0.19 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Sorted             |   108,431.6 ns |  2,260.41 ns | 1,182.24 ns |  5.52 |    0.10 |    5 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Sorted             |    94,432.6 ns |  1,438.07 ns |   752.14 ns |  4.81 |    0.08 |    5 |         - |          NA |
| IntroSort                    | 4096 | Sorted             |     4,039.3 ns |    494.25 ns |   219.45 ns |  0.21 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 4096 | Sorted             |    22,332.6 ns |    119.15 ns |    42.49 ns |  1.14 |    0.02 |    3 |         - |          NA |
| PDQSort                      | 4096 | Sorted             |     5,173.1 ns |    210.90 ns |    93.64 ns |  0.26 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 4096 | Sorted             |     5,440.9 ns |    347.65 ns |   181.83 ns |  0.28 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 4096 | Sorted             |     2,252.4 ns |     11.62 ns |     4.14 ns |  0.11 |    0.00 |    1 |         - |          NA |
| StdSort                      | 4096 | Sorted             |     4,449.6 ns |     18.75 ns |     6.69 ns |  0.23 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 4096 | Sorted             |    36,267.4 ns |    872.74 ns |   456.46 ns |  1.85 |    0.04 |    4 |         - |          NA |
| DotnetSort                   | 4096 | Sorted             |    19,576.2 ns |  1,333.23 ns |   591.96 ns |  1.00 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Reversed**           |    **21,911.4 ns** |  **1,137.47 ns** |   **505.05 ns** |  **1.00** |    **0.03** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Reversed           |    22,526.4 ns |  1,766.74 ns |   784.44 ns |  1.03 |    0.04 |    4 |         - |          NA |
| QuickSortMedian3             | 4096 | Reversed           |    27,033.9 ns |  1,077.24 ns |   563.42 ns |  1.23 |    0.04 |    4 |         - |          NA |
| QuickSortMedian9             | 4096 | Reversed           |    28,761.3 ns |    782.49 ns |   347.43 ns |  1.31 |    0.03 |    4 |         - |          NA |
| DualPivotQuickSort           | 4096 | Reversed           |    25,595.3 ns |  1,924.69 ns | 1,006.65 ns |  1.17 |    0.05 |    4 |         - |          NA |
| StableQuickSort              | 4096 | Reversed           |   206,724.9 ns |  1,179.50 ns |   616.90 ns |  9.44 |    0.20 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Reversed           |   119,264.2 ns |  5,433.64 ns | 2,412.57 ns |  5.45 |    0.15 |    6 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Reversed           |   143,962.0 ns |  1,632.04 ns |   853.59 ns |  6.57 |    0.14 |    7 |         - |          NA |
| IntroSort                    | 4096 | Reversed           |    13,636.3 ns |    487.72 ns |   216.55 ns |  0.62 |    0.02 |    3 |         - |          NA |
| IntroSortDotnet              | 4096 | Reversed           |    35,022.8 ns |    974.67 ns |   509.77 ns |  1.60 |    0.04 |    5 |         - |          NA |
| PDQSort                      | 4096 | Reversed           |     8,336.2 ns |    440.60 ns |   230.44 ns |  0.38 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 4096 | Reversed           |    12,841.6 ns |    983.23 ns |   436.56 ns |  0.59 |    0.02 |    3 |         - |          NA |
| Ipnsort                      | 4096 | Reversed           |     3,552.6 ns |      9.62 ns |     3.43 ns |  0.16 |    0.00 |    1 |         - |          NA |
| StdSort                      | 4096 | Reversed           |    11,353.6 ns |    308.70 ns |   110.08 ns |  0.52 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 4096 | Reversed           |    39,762.4 ns |  1,071.45 ns |   475.73 ns |  1.82 |    0.04 |    5 |         - |          NA |
| DotnetSort                   | 4096 | Reversed           |    40,265.9 ns |  3,926.23 ns | 2,053.49 ns |  1.84 |    0.10 |    5 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **4096** | **PipeOrgan**          | **1,584,374.1 ns** |  **5,482.98 ns** | **2,434.47 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| QuickSort3way                | 4096 | PipeOrgan          |    84,299.1 ns |  9,900.08 ns | 5,177.93 ns |  0.05 |    0.00 |    3 |         - |          NA |
| QuickSortMedian3             | 4096 | PipeOrgan          |    82,484.1 ns |  2,628.43 ns | 1,374.72 ns |  0.05 |    0.00 |    3 |         - |          NA |
| QuickSortMedian9             | 4096 | PipeOrgan          |    54,681.1 ns |  1,238.51 ns |   647.76 ns |  0.03 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort           | 4096 | PipeOrgan          |    39,904.9 ns |  1,997.32 ns | 1,044.64 ns |  0.03 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 4096 | PipeOrgan          |   208,489.6 ns |    881.45 ns |   391.37 ns |  0.13 |    0.00 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | PipeOrgan          |   118,972.6 ns |  3,450.70 ns | 1,804.78 ns |  0.08 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 4096 | PipeOrgan          |   170,347.6 ns |    588.03 ns |   307.55 ns |  0.11 |    0.00 |    4 |         - |          NA |
| IntroSort                    | 4096 | PipeOrgan          |    77,745.9 ns |  2,211.30 ns | 1,156.55 ns |  0.05 |    0.00 |    3 |         - |          NA |
| IntroSortDotnet              | 4096 | PipeOrgan          |    84,700.9 ns |  2,168.76 ns | 1,134.30 ns |  0.05 |    0.00 |    3 |         - |          NA |
| PDQSort                      | 4096 | PipeOrgan          |    41,922.3 ns |  1,237.19 ns |   549.32 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 4096 | PipeOrgan          |    73,451.3 ns |    861.55 ns |   382.53 ns |  0.05 |    0.00 |    3 |         - |          NA |
| Ipnsort                      | 4096 | PipeOrgan          |   105,928.0 ns |    489.84 ns |   217.49 ns |  0.07 |    0.00 |    3 |         - |          NA |
| StdSort                      | 4096 | PipeOrgan          |   107,939.5 ns |  1,522.11 ns |   796.09 ns |  0.07 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 4096 | PipeOrgan          |   107,059.5 ns |    948.65 ns |   496.16 ns |  0.07 |    0.00 |    3 |         - |          NA |
| DotnetSort                   | 4096 | PipeOrgan          |    93,548.4 ns |  3,871.22 ns | 2,024.72 ns |  0.06 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **4096** | **ManyDuplicates**     |    **43,676.2 ns** |  **2,098.87 ns** | **1,097.75 ns** |  **1.00** |    **0.03** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 4096 | ManyDuplicates     |    32,202.8 ns |  1,663.17 ns |   738.46 ns |  0.74 |    0.02 |    2 |         - |          NA |
| QuickSortMedian3             | 4096 | ManyDuplicates     |    52,399.2 ns |  1,169.82 ns |   611.84 ns |  1.20 |    0.03 |    2 |         - |          NA |
| QuickSortMedian9             | 4096 | ManyDuplicates     |    56,513.4 ns |  1,786.64 ns |   793.28 ns |  1.29 |    0.03 |    2 |         - |          NA |
| DualPivotQuickSort           | 4096 | ManyDuplicates     |    27,905.5 ns |    915.69 ns |   478.92 ns |  0.64 |    0.02 |    2 |         - |          NA |
| StableQuickSort              | 4096 | ManyDuplicates     |   109,505.8 ns |  1,520.59 ns |   675.15 ns |  2.51 |    0.06 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | ManyDuplicates     |    53,822.2 ns |    610.67 ns |   271.14 ns |  1.23 |    0.03 |    2 |         - |          NA |
| DestswapStableQuickSort      | 4096 | ManyDuplicates     |    54,062.2 ns |  2,188.22 ns | 1,144.48 ns |  1.24 |    0.04 |    2 |         - |          NA |
| IntroSort                    | 4096 | ManyDuplicates     |    49,799.0 ns |    768.87 ns |   341.38 ns |  1.14 |    0.03 |    2 |         - |          NA |
| IntroSortDotnet              | 4096 | ManyDuplicates     |    37,671.0 ns |  1,320.91 ns |   586.49 ns |  0.86 |    0.02 |    2 |         - |          NA |
| PDQSort                      | 4096 | ManyDuplicates     |    22,139.1 ns |  1,134.50 ns |   593.37 ns |  0.51 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless            | 4096 | ManyDuplicates     |    30,328.9 ns |    497.20 ns |   260.05 ns |  0.69 |    0.02 |    2 |         - |          NA |
| Ipnsort                      | 4096 | ManyDuplicates     |    60,434.3 ns |  1,233.84 ns |   440.00 ns |  1.38 |    0.03 |    2 |         - |          NA |
| StdSort                      | 4096 | ManyDuplicates     |    33,553.6 ns |    460.29 ns |   240.74 ns |  0.77 |    0.02 |    2 |         - |          NA |
| BlockQuickSort               | 4096 | ManyDuplicates     |    52,782.8 ns |    647.29 ns |   287.40 ns |  1.21 |    0.03 |    2 |         - |          NA |
| DotnetSort                   | 4096 | ManyDuplicates     |    36,339.0 ns |    552.72 ns |   245.41 ns |  0.83 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Random**             |   **472,965.7 ns** |  **9,376.14 ns** | **4,903.90 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Random             |   414,896.5 ns |  4,178.63 ns | 2,185.50 ns |  0.88 |    0.01 |    3 |         - |          NA |
| QuickSortMedian3             | 8192 | Random             |   464,542.8 ns |  2,974.55 ns | 1,320.72 ns |  0.98 |    0.01 |    3 |         - |          NA |
| QuickSortMedian9             | 8192 | Random             |   505,509.2 ns |  4,934.08 ns | 2,190.76 ns |  1.07 |    0.01 |    3 |         - |          NA |
| DualPivotQuickSort           | 8192 | Random             |   333,003.6 ns |  3,563.13 ns | 1,863.58 ns |  0.70 |    0.01 |    3 |         - |          NA |
| StableQuickSort              | 8192 | Random             | 1,308,499.1 ns |  3,769.37 ns | 1,971.45 ns |  2.77 |    0.03 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Random             | 1,051,245.3 ns |  2,856.53 ns | 1,494.02 ns |  2.22 |    0.02 |    5 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Random             |   861,883.5 ns |  3,631.33 ns | 1,899.25 ns |  1.82 |    0.02 |    4 |         - |          NA |
| IntroSort                    | 8192 | Random             |   396,340.1 ns |  4,365.47 ns | 2,283.22 ns |  0.84 |    0.01 |    3 |         - |          NA |
| IntroSortDotnet              | 8192 | Random             |   328,612.3 ns | 15,251.80 ns | 6,771.90 ns |  0.69 |    0.01 |    3 |         - |          NA |
| PDQSort                      | 8192 | Random             |   323,254.8 ns |  7,453.58 ns | 3,309.44 ns |  0.68 |    0.01 |    3 |         - |          NA |
| PDQSortBranchless            | 8192 | Random             |   138,278.9 ns |  2,438.18 ns | 1,275.22 ns |  0.29 |    0.00 |    1 |         - |          NA |
| Ipnsort                      | 8192 | Random             |   213,569.3 ns |    948.40 ns |   421.09 ns |  0.45 |    0.00 |    2 |         - |          NA |
| StdSort                      | 8192 | Random             |   133,822.1 ns |  1,704.34 ns |   891.40 ns |  0.28 |    0.00 |    1 |         - |          NA |
| BlockQuickSort               | 8192 | Random             |   150,096.2 ns |  2,571.31 ns | 1,141.68 ns |  0.32 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 8192 | Random             |   333,388.2 ns |  9,063.94 ns | 4,740.62 ns |  0.70 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **SingleElementMoved** |    **53,915.9 ns** |  **1,387.27 ns** |   **725.57 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 8192 | SingleElementMoved |    57,197.6 ns |  1,022.48 ns |   364.63 ns |  1.06 |    0.01 |    1 |         - |          NA |
| QuickSortMedian3             | 8192 | SingleElementMoved |    75,077.7 ns |  1,207.15 ns |   631.36 ns |  1.39 |    0.02 |    1 |         - |          NA |
| QuickSortMedian9             | 8192 | SingleElementMoved |    98,887.7 ns |  1,621.75 ns |   720.07 ns |  1.83 |    0.03 |    2 |         - |          NA |
| DualPivotQuickSort           | 8192 | SingleElementMoved |    49,168.4 ns |  1,088.61 ns |   569.37 ns |  0.91 |    0.02 |    1 |         - |          NA |
| StableQuickSort              | 8192 | SingleElementMoved |   451,034.9 ns |    938.65 ns |   490.93 ns |  8.37 |    0.11 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | SingleElementMoved |   278,247.2 ns | 12,358.02 ns | 6,463.48 ns |  5.16 |    0.13 |    4 |         - |          NA |
| DestswapStableQuickSort      | 8192 | SingleElementMoved |   202,316.0 ns |  3,122.64 ns | 1,633.20 ns |  3.75 |    0.06 |    3 |         - |          NA |
| IntroSort                    | 8192 | SingleElementMoved |    40,160.2 ns |  3,059.64 ns | 1,358.50 ns |  0.74 |    0.03 |    1 |         - |          NA |
| IntroSortDotnet              | 8192 | SingleElementMoved |    60,221.0 ns |    437.73 ns |   194.36 ns |  1.12 |    0.01 |    1 |         - |          NA |
| PDQSort                      | 8192 | SingleElementMoved |    44,305.4 ns |    712.82 ns |   372.82 ns |  0.82 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 8192 | SingleElementMoved |    54,212.2 ns |  1,354.89 ns |   708.63 ns |  1.01 |    0.02 |    1 |         - |          NA |
| Ipnsort                      | 8192 | SingleElementMoved |   192,205.7 ns |  1,025.44 ns |   536.32 ns |  3.57 |    0.05 |    3 |         - |          NA |
| StdSort                      | 8192 | SingleElementMoved |    64,172.2 ns |    798.89 ns |   354.71 ns |  1.19 |    0.02 |    1 |         - |          NA |
| BlockQuickSort               | 8192 | SingleElementMoved |    94,914.7 ns |    631.04 ns |   330.05 ns |  1.76 |    0.02 |    2 |         - |          NA |
| DotnetSort                   | 8192 | SingleElementMoved |    58,071.4 ns |    940.51 ns |   335.40 ns |  1.08 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Sorted**             |    **41,340.8 ns** |    **490.78 ns** |   **217.91 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Sorted             |    40,686.1 ns |  3,444.59 ns | 1,801.58 ns |  0.98 |    0.04 |    3 |         - |          NA |
| QuickSortMedian3             | 8192 | Sorted             |    54,024.3 ns |  1,208.58 ns |   632.11 ns |  1.31 |    0.02 |    3 |         - |          NA |
| QuickSortMedian9             | 8192 | Sorted             |    58,176.6 ns |    928.69 ns |   485.72 ns |  1.41 |    0.01 |    3 |         - |          NA |
| DualPivotQuickSort           | 8192 | Sorted             |    45,476.5 ns |  1,123.74 ns |   587.74 ns |  1.10 |    0.01 |    3 |         - |          NA |
| StableQuickSort              | 8192 | Sorted             |   492,809.9 ns |  2,788.17 ns | 1,237.97 ns | 11.92 |    0.07 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Sorted             |   230,075.7 ns |  4,012.41 ns | 1,781.54 ns |  5.57 |    0.05 |    5 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Sorted             |   195,948.8 ns |  2,089.86 ns | 1,093.04 ns |  4.74 |    0.03 |    5 |         - |          NA |
| IntroSort                    | 8192 | Sorted             |     7,810.7 ns |    461.33 ns |   204.83 ns |  0.19 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet              | 8192 | Sorted             |    47,832.0 ns |    965.60 ns |   344.34 ns |  1.16 |    0.01 |    3 |         - |          NA |
| PDQSort                      | 8192 | Sorted             |    10,379.2 ns |    505.80 ns |   180.37 ns |  0.25 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | Sorted             |    10,317.8 ns |    358.88 ns |   159.35 ns |  0.25 |    0.00 |    2 |         - |          NA |
| Ipnsort                      | 8192 | Sorted             |     4,593.5 ns |     17.92 ns |     9.37 ns |  0.11 |    0.00 |    1 |         - |          NA |
| StdSort                      | 8192 | Sorted             |     9,053.9 ns |    447.36 ns |   198.63 ns |  0.22 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | Sorted             |    78,647.3 ns |    841.05 ns |   373.43 ns |  1.90 |    0.01 |    4 |         - |          NA |
| DotnetSort                   | 8192 | Sorted             |    41,654.6 ns |    602.07 ns |   214.70 ns |  1.01 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Reversed**           |    **47,024.1 ns** |    **916.37 ns** |   **406.88 ns** |  **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Reversed           |    49,162.2 ns |  5,661.45 ns | 2,513.72 ns |  1.05 |    0.05 |    4 |         - |          NA |
| QuickSortMedian3             | 8192 | Reversed           |    56,843.0 ns |  1,165.60 ns |   517.53 ns |  1.21 |    0.01 |    4 |         - |          NA |
| QuickSortMedian9             | 8192 | Reversed           |    60,902.5 ns |  1,378.01 ns |   720.73 ns |  1.30 |    0.02 |    4 |         - |          NA |
| DualPivotQuickSort           | 8192 | Reversed           |    55,282.3 ns |  1,753.50 ns |   917.11 ns |  1.18 |    0.02 |    4 |         - |          NA |
| StableQuickSort              | 8192 | Reversed           |   448,907.4 ns |  1,129.79 ns |   501.63 ns |  9.55 |    0.08 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Reversed           |   254,097.4 ns | 13,924.98 ns | 7,283.03 ns |  5.40 |    0.15 |    6 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Reversed           |   306,362.2 ns |  4,113.19 ns | 2,151.28 ns |  6.52 |    0.07 |    7 |         - |          NA |
| IntroSort                    | 8192 | Reversed           |    26,500.4 ns |    407.99 ns |   181.15 ns |  0.56 |    0.01 |    3 |         - |          NA |
| IntroSortDotnet              | 8192 | Reversed           |    76,035.7 ns |  1,295.50 ns |   677.57 ns |  1.62 |    0.02 |    5 |         - |          NA |
| PDQSort                      | 8192 | Reversed           |    16,473.1 ns |    882.99 ns |   461.82 ns |  0.35 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | Reversed           |    25,326.0 ns |    525.13 ns |   187.26 ns |  0.54 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 8192 | Reversed           |     7,188.0 ns |    254.21 ns |   132.96 ns |  0.15 |    0.00 |    1 |         - |          NA |
| StdSort                      | 8192 | Reversed           |    22,428.5 ns |    584.14 ns |   259.36 ns |  0.48 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 8192 | Reversed           |    86,051.4 ns |    799.24 ns |   418.02 ns |  1.83 |    0.02 |    5 |         - |          NA |
| DotnetSort                   | 8192 | Reversed           |    92,197.9 ns |  6,306.55 ns | 3,298.45 ns |  1.96 |    0.07 |    5 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **PipeOrgan**          | **6,157,415.1 ns** |  **9,954.52 ns** | **4,419.87 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| QuickSort3way                | 8192 | PipeOrgan          |   199,676.7 ns |  8,046.90 ns | 4,208.68 ns |  0.03 |    0.00 |    3 |         - |          NA |
| QuickSortMedian3             | 8192 | PipeOrgan          |   193,700.0 ns |  5,108.94 ns | 2,672.07 ns |  0.03 |    0.00 |    3 |         - |          NA |
| QuickSortMedian9             | 8192 | PipeOrgan          |   118,705.5 ns |  5,390.07 ns | 2,819.11 ns |  0.02 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort           | 8192 | PipeOrgan          |    84,159.8 ns |  1,246.09 ns |   553.27 ns |  0.01 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 8192 | PipeOrgan          |   452,087.2 ns |  1,219.74 ns |   637.95 ns |  0.07 |    0.00 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | PipeOrgan          |   258,575.7 ns | 10,745.78 ns | 5,620.25 ns |  0.04 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 8192 | PipeOrgan          |   364,954.0 ns |    795.38 ns |   416.00 ns |  0.06 |    0.00 |    4 |         - |          NA |
| IntroSort                    | 8192 | PipeOrgan          |   195,340.5 ns | 13,721.51 ns | 7,176.61 ns |  0.03 |    0.00 |    3 |         - |          NA |
| IntroSortDotnet              | 8192 | PipeOrgan          |   342,762.5 ns |  4,608.31 ns | 2,046.12 ns |  0.06 |    0.00 |    4 |         - |          NA |
| PDQSort                      | 8192 | PipeOrgan          |    91,177.0 ns |  1,531.61 ns |   801.06 ns |  0.01 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 8192 | PipeOrgan          |   162,567.4 ns |  2,594.38 ns | 1,356.91 ns |  0.03 |    0.00 |    3 |         - |          NA |
| Ipnsort                      | 8192 | PipeOrgan          |   236,873.3 ns |  2,601.92 ns | 1,155.27 ns |  0.04 |    0.00 |    3 |         - |          NA |
| StdSort                      | 8192 | PipeOrgan          |   283,997.9 ns | 11,576.59 ns | 6,054.78 ns |  0.05 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 8192 | PipeOrgan          |   225,351.7 ns |  1,908.92 ns |   998.40 ns |  0.04 |    0.00 |    3 |         - |          NA |
| DotnetSort                   | 8192 | PipeOrgan          |   356,270.4 ns |  5,278.96 ns | 2,761.00 ns |  0.06 |    0.00 |    4 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **ManyDuplicates**     |    **96,738.4 ns** |    **753.29 ns** |   **268.63 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 8192 | ManyDuplicates     |    67,262.2 ns | 10,451.96 ns | 4,640.73 ns |  0.70 |    0.04 |    2 |         - |          NA |
| QuickSortMedian3             | 8192 | ManyDuplicates     |   114,934.4 ns |  1,651.65 ns |   733.34 ns |  1.19 |    0.01 |    2 |         - |          NA |
| QuickSortMedian9             | 8192 | ManyDuplicates     |   122,367.3 ns |  1,036.44 ns |   369.61 ns |  1.26 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort           | 8192 | ManyDuplicates     |    61,522.3 ns |  3,963.15 ns | 2,072.80 ns |  0.64 |    0.02 |    2 |         - |          NA |
| StableQuickSort              | 8192 | ManyDuplicates     |   462,648.0 ns |  2,827.83 ns | 1,479.01 ns |  4.78 |    0.02 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | ManyDuplicates     |   244,821.1 ns | 11,011.31 ns | 4,889.09 ns |  2.53 |    0.05 |    3 |         - |          NA |
| DestswapStableQuickSort      | 8192 | ManyDuplicates     |   117,589.9 ns |  9,760.20 ns | 5,104.77 ns |  1.22 |    0.05 |    2 |         - |          NA |
| IntroSort                    | 8192 | ManyDuplicates     |   113,947.4 ns |  2,760.43 ns | 1,225.65 ns |  1.18 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 8192 | ManyDuplicates     |    82,710.2 ns |    847.43 ns |   376.27 ns |  0.85 |    0.00 |    2 |         - |          NA |
| PDQSort                      | 8192 | ManyDuplicates     |    44,063.3 ns |    981.59 ns |   435.83 ns |  0.46 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 8192 | ManyDuplicates     |    59,604.7 ns |  1,184.61 ns |   619.57 ns |  0.62 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 8192 | ManyDuplicates     |   118,467.3 ns |  1,060.59 ns |   554.71 ns |  1.22 |    0.01 |    2 |         - |          NA |
| StdSort                      | 8192 | ManyDuplicates     |    63,316.3 ns |  1,943.08 ns | 1,016.27 ns |  0.65 |    0.01 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | ManyDuplicates     |   103,063.8 ns |  2,285.97 ns | 1,195.61 ns |  1.07 |    0.01 |    2 |         - |          NA |
| DotnetSort                   | 8192 | ManyDuplicates     |    79,810.8 ns |  1,520.06 ns |   674.92 ns |  0.83 |    0.01 |    2 |         - |          NA |

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
| **Lsd256_CountPerPass** | **1024** | **1**           |   **4,289.7 ns** |  **1,339.53 ns** |    **700.60 ns** |  **1.02** |    **0.21** |         **-** |          **NA** |
| Lsd256_Histogram    | 1024 | 1           |   4,805.8 ns |     14.42 ns |      5.14 ns |  1.14 |    0.15 |         - |          NA |
| Lsd10_CountPerPass  | 1024 | 1           |  17,063.1 ns |    149.03 ns |     66.17 ns |  4.06 |    0.55 |         - |          NA |
| Lsd10_Histogram     | 1024 | 1           |  16,669.4 ns |     64.19 ns |     33.57 ns |  3.97 |    0.53 |         - |          NA |
| Lsd10_Quotient      | 1024 | 1           |  18,402.2 ns |  2,125.07 ns |  1,111.45 ns |  4.38 |    0.64 |         - |          NA |
|      |             |              |              |              |       |         |           |             |
| **Lsd256_CountPerPass** | **1024** | **2**           |   **6,531.5 ns** |    **244.53 ns** |    **127.89 ns** |  **1.00** |    **0.03** |         **-** |          **NA** |
| Lsd256_Histogram    | 1024 | 2           |   6,702.2 ns |     15.50 ns |      6.88 ns |  1.03 |    0.02 |         - |          NA |
| Lsd10_CountPerPass  | 1024 | 2           |  27,508.3 ns |    407.70 ns |    213.23 ns |  4.21 |    0.08 |         - |          NA |
| Lsd10_Histogram     | 1024 | 2           |  27,008.3 ns |    237.28 ns |    124.10 ns |  4.14 |    0.08 |         - |          NA |
| Lsd10_Quotient      | 1024 | 2           |  28,314.0 ns |    947.07 ns |    495.34 ns |  4.34 |    0.11 |         - |          NA |
|      |             |              |              |              |       |         |           |             |
| **Lsd256_CountPerPass** | **1024** | **3**           |   **8,935.6 ns** |     **23.34 ns** |      **8.32 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Lsd256_Histogram    | 1024 | 3           |   9,356.2 ns |    328.69 ns |    145.94 ns |  1.05 |    0.02 |         - |          NA |
| Lsd10_CountPerPass  | 1024 | 3           |  42,649.5 ns |    271.90 ns |    142.21 ns |  4.77 |    0.02 |         - |          NA |
| Lsd10_Histogram     | 1024 | 3           |  42,038.2 ns |    311.27 ns |    162.80 ns |  4.70 |    0.02 |         - |          NA |
| Lsd10_Quotient      | 1024 | 3           |  97,087.9 ns |  1,255.74 ns |    656.77 ns | 10.87 |    0.07 |         - |          NA |
|      |             |              |              |              |       |         |           |             |
| **Lsd256_CountPerPass** | **1024** | **4**           |  **11,945.0 ns** |    **241.11 ns** |    **107.05 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| Lsd256_Histogram    | 1024 | 4           |  10,975.5 ns |    410.24 ns |    214.56 ns |  0.92 |    0.02 |         - |          NA |
| Lsd10_CountPerPass  | 1024 | 4           |  53,599.0 ns |    299.73 ns |    156.77 ns |  4.49 |    0.04 |         - |          NA |
| Lsd10_Histogram     | 1024 | 4           |  52,418.3 ns |    382.35 ns |    169.76 ns |  4.39 |    0.04 |         - |          NA |
| Lsd10_Quotient      | 1024 | 4           |  63,030.5 ns |  1,636.86 ns |    856.11 ns |  5.28 |    0.08 |         - |          NA |
|      |             |              |              |              |       |         |           |             |
| **Lsd256_CountPerPass** | **8192** | **1**           |  **29,334.9 ns** |    **895.08 ns** |    **397.42 ns** |  **1.00** |    **0.02** |         **-** |          **NA** |
| Lsd256_Histogram    | 8192 | 1           |  36,960.1 ns |    796.79 ns |    416.73 ns |  1.26 |    0.02 |         - |          NA |
| Lsd10_CountPerPass  | 8192 | 1           | 133,915.0 ns |    620.88 ns |    275.67 ns |  4.57 |    0.06 |         - |          NA |
| Lsd10_Histogram     | 8192 | 1           | 130,375.4 ns |    501.54 ns |    262.31 ns |  4.45 |    0.06 |         - |          NA |
| Lsd10_Quotient      | 8192 | 1           | 265,124.4 ns | 10,216.47 ns |  5,343.41 ns |  9.04 |    0.21 |         - |          NA |
|      |             |              |              |              |       |         |           |             |
| **Lsd256_CountPerPass** | **8192** | **2**           |  **48,085.7 ns** |    **884.47 ns** |    **462.59 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| Lsd256_Histogram    | 8192 | 2           |  51,657.1 ns |    923.48 ns |    483.00 ns |  1.07 |    0.01 |         - |          NA |
| Lsd10_CountPerPass  | 8192 | 2           | 216,970.2 ns |  1,485.83 ns |    777.12 ns |  4.51 |    0.04 |         - |          NA |
| Lsd10_Histogram     | 8192 | 2           | 210,432.6 ns |  1,313.80 ns |    583.33 ns |  4.38 |    0.04 |         - |          NA |
| Lsd10_Quotient      | 8192 | 2           | 416,747.4 ns |  8,900.74 ns |  4,655.26 ns |  8.67 |    0.12 |         - |          NA |
|      |             |              |              |              |       |         |           |             |
| **Lsd256_CountPerPass** | **8192** | **3**           |  **68,038.9 ns** |    **552.53 ns** |    **288.98 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| Lsd256_Histogram    | 8192 | 3           |  69,626.3 ns |    866.39 ns |    453.14 ns |  1.02 |    0.01 |         - |          NA |
| Lsd10_CountPerPass  | 8192 | 3           | 341,277.6 ns |    876.76 ns |    458.56 ns |  5.02 |    0.02 |         - |          NA |
| Lsd10_Histogram     | 8192 | 3           | 330,461.0 ns |  1,398.40 ns |    620.90 ns |  4.86 |    0.02 |         - |          NA |
| Lsd10_Quotient      | 8192 | 3           | 739,963.3 ns | 20,605.32 ns | 10,776.98 ns | 10.88 |    0.16 |         - |          NA |
|      |             |              |              |              |       |         |           |             |
| **Lsd256_CountPerPass** | **8192** | **4**           |  **89,229.4 ns** |  **1,201.71 ns** |    **628.52 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| Lsd256_Histogram    | 8192 | 4           |  81,333.2 ns |  1,107.22 ns |    579.10 ns |  0.91 |    0.01 |         - |          NA |
| Lsd10_CountPerPass  | 8192 | 4           | 423,563.5 ns |    758.71 ns |    396.82 ns |  4.75 |    0.03 |         - |          NA |
| Lsd10_Histogram     | 8192 | 4           | 419,490.9 ns |    800.12 ns |    355.26 ns |  4.70 |    0.03 |         - |          NA |
| Lsd10_Quotient      | 8192 | 4           | 983,497.2 ns |  8,951.98 ns |  3,974.74 ns | 11.02 |    0.08 |         - |          NA |

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

| Method        | Size | Stride | Mean         | Error       | StdDev      | Ratio | RatioSD | Allocated | Alloc Ratio |
| -------------- |----- |------- |-------------:|------------:|------------:|------:|--------:|----------:|------------:|
| **Lsd4_NoSkip**   | **1024** | **1**      |  **18,683.4 ns** |   **451.73 ns** |   **236.26 ns** |  **1.00** |    **0.02** |         **-** |          **NA** |
| Lsd4_Skip     | 1024 | 1      |  14,016.8 ns |   300.65 ns |   157.24 ns |  0.75 |    0.01 |         - |          NA |
| Lsd256_NoSkip | 1024 | 1      |   7,168.4 ns |    83.23 ns |    43.53 ns |  0.38 |    0.01 |         - |          NA |
| Lsd256_Skip   | 1024 | 1      |   6,771.0 ns |    42.22 ns |    22.08 ns |  0.36 |    0.00 |         - |          NA |
| Lsd10_NoSkip  | 1024 | 1      |  21,302.8 ns |   209.03 ns |   109.33 ns |  1.14 |    0.01 |         - |          NA |
| Lsd10_Skip    | 1024 | 1      |  21,187.1 ns |   377.03 ns |   167.40 ns |  1.13 |    0.02 |         - |          NA |
|      |        |              |             |             |       |         |           |             |
| **Lsd4_NoSkip**   | **1024** | **65536**  |  **42,075.2 ns** |   **244.00 ns** |   **127.62 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Lsd4_Skip     | 1024 | 65536  |  23,556.3 ns |   254.25 ns |   112.89 ns |  0.56 |    0.00 |         - |          NA |
| Lsd256_NoSkip | 1024 | 65536  |  12,622.6 ns |   280.78 ns |   146.85 ns |  0.30 |    0.00 |         - |          NA |
| Lsd256_Skip   | 1024 | 65536  |   9,124.3 ns |   318.87 ns |   166.77 ns |  0.22 |    0.00 |         - |          NA |
| Lsd10_NoSkip  | 1024 | 65536  |  41,823.6 ns |   527.46 ns |   234.19 ns |  0.99 |    0.01 |         - |          NA |
| Lsd10_Skip    | 1024 | 65536  |  41,916.9 ns |   225.28 ns |   117.82 ns |  1.00 |    0.00 |         - |          NA |
|      |        |              |             |             |       |         |           |             |
| **Lsd4_NoSkip**   | **8192** | **1**      | **199,669.4 ns** |   **834.60 ns** |   **436.51 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Lsd4_Skip     | 8192 | 1      | 153,216.8 ns | 1,017.49 ns |   532.16 ns |  0.77 |    0.00 |         - |          NA |
| Lsd256_NoSkip | 8192 | 1      |  52,152.7 ns | 1,999.09 ns |   887.61 ns |  0.26 |    0.00 |         - |          NA |
| Lsd256_Skip   | 8192 | 1      |  50,987.1 ns | 1,198.56 ns |   626.87 ns |  0.26 |    0.00 |         - |          NA |
| Lsd10_NoSkip  | 8192 | 1      | 170,051.6 ns | 1,085.83 ns |   567.91 ns |  0.85 |    0.00 |         - |          NA |
| Lsd10_Skip    | 8192 | 1      | 169,961.5 ns | 2,144.85 ns | 1,121.80 ns |  0.85 |    0.01 |         - |          NA |
|      |        |              |             |             |       |         |           |             |
| **Lsd4_NoSkip**   | **8192** | **65536**  | **393,346.4 ns** | **2,007.55 ns** | **1,049.99 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Lsd4_Skip     | 8192 | 65536  | 222,774.5 ns | 1,032.04 ns |   539.78 ns |  0.57 |    0.00 |         - |          NA |
| Lsd256_NoSkip | 8192 | 65536  |  87,971.1 ns | 1,149.09 ns |   510.20 ns |  0.22 |    0.00 |         - |          NA |
| Lsd256_Skip   | 8192 | 65536  |  67,828.5 ns | 1,455.08 ns |   761.04 ns |  0.17 |    0.00 |         - |          NA |
| Lsd10_NoSkip  | 8192 | 65536  | 376,788.8 ns |   357.49 ns |   158.73 ns |  0.96 |    0.00 |         - |          NA |
| Lsd10_Skip    | 8192 | 65536  | 376,756.6 ns | 2,841.45 ns | 1,486.14 ns |  0.96 |    0.00 |         - |          NA |

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

| Method         | Size  | FullRange | Mean           | Error      | StdDev      | Ratio | Allocated | Alloc Ratio |
| --------------- |------ |---------- |---------------:|-----------:|------------:|------:|----------:|------------:|
| **Lsd4_Recompute** | **1024**  | **False**     |    **13,974.2 ns** |   **235.2 ns** |   **104.43 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_KeyCache  | 1024  | False     |    18,724.1 ns |   173.2 ns |    76.92 ns |  1.34 |         - |          NA |
|       |           |                |            |             |       |           |             |
| **Lsd4_Recompute** | **1024**  | **True**      |    **42,310.7 ns** |   **318.4 ns** |   **166.53 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_KeyCache  | 1024  | True      |    55,837.1 ns |   618.2 ns |   323.34 ns |  1.32 |         - |          NA |
|       |           |                |            |             |       |           |             |
| **Lsd4_Recompute** | **8192**  | **False**     |   **153,354.9 ns** | **1,115.2 ns** |   **495.17 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_KeyCache  | 8192  | False     |   203,180.4 ns | 1,456.1 ns |   761.56 ns |  1.32 |         - |          NA |
|       |           |                |            |             |       |           |             |
| **Lsd4_Recompute** | **8192**  | **True**      |   **332,824.4 ns** | **1,048.3 ns** |   **548.27 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_KeyCache  | 8192  | True      |   434,242.6 ns | 1,532.0 ns |   801.29 ns |  1.30 |         - |          NA |
|       |           |                |            |             |       |           |             |
| **Lsd4_Recompute** | **65536** | **False**     | **1,373,290.3 ns** | **2,921.8 ns** | **1,528.13 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_KeyCache  | 65536 | False     | 1,826,057.1 ns | 2,653.6 ns | 1,387.90 ns |  1.33 |         - |          NA |
|       |           |                |            |             |       |           |             |
| **Lsd4_Recompute** | **65536** | **True**      | **2,652,651.7 ns** | **4,214.4 ns** | **2,204.21 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_KeyCache  | 65536 | True      | 3,478,393.5 ns | 7,455.8 ns | 3,899.55 ns |  1.31 |         - |          NA |

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
| **Lsd4_Xor**          | **1024** | **False**         |  **20,364.1 ns** |   **303.03 ns** |   **158.49 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_Normalized   | 1024 | False         |  13,923.0 ns |   203.34 ns |    90.28 ns |  0.68 |         - |          NA |
| Lsd256_Xor        | 1024 | False         |   6,234.7 ns |    71.04 ns |    37.15 ns |  0.31 |         - |          NA |
| Lsd256_Normalized | 1024 | False         |   6,835.5 ns |   158.47 ns |    70.36 ns |  0.34 |         - |          NA |
| Lsd10_CopyBack    | 1024 | False         |  22,414.6 ns |   110.73 ns |    57.92 ns |  1.10 |         - |          NA |
| Lsd10_PingPong    | 1024 | False         |  21,700.2 ns |   338.35 ns |   176.96 ns |  1.07 |         - |          NA |
|      |               |              |             |             |       |           |             |
| **Lsd4_Xor**          | **1024** | **True**          |  **50,556.2 ns** |   **259.96 ns** |   **115.42 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_Normalized   | 1024 | True          |  13,963.8 ns |   275.11 ns |   143.89 ns |  0.28 |         - |          NA |
| Lsd256_Xor        | 1024 | True          |  11,061.2 ns |   305.33 ns |   159.69 ns |  0.22 |         - |          NA |
| Lsd256_Normalized | 1024 | True          |   6,556.8 ns |   448.43 ns |   234.54 ns |  0.13 |         - |          NA |
| Lsd10_CopyBack    | 1024 | True          |  22,245.0 ns |   194.10 ns |   101.52 ns |  0.44 |         - |          NA |
| Lsd10_PingPong    | 1024 | True          |  21,795.3 ns |   278.76 ns |   145.80 ns |  0.43 |         - |          NA |
|      |               |              |             |             |       |           |             |
| **Lsd4_Xor**          | **8192** | **False**         | **203,703.5 ns** | **1,252.74 ns** |   **556.23 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_Normalized   | 8192 | False         | 151,702.6 ns | 1,083.12 ns |   566.49 ns |  0.74 |         - |          NA |
| Lsd256_Xor        | 8192 | False         |  46,245.1 ns |   421.89 ns |   187.32 ns |  0.23 |         - |          NA |
| Lsd256_Normalized | 8192 | False         |  50,008.1 ns | 1,048.03 ns |   548.14 ns |  0.25 |         - |          NA |
| Lsd10_CopyBack    | 8192 | False         | 180,718.7 ns |   920.54 ns |   408.72 ns |  0.89 |         - |          NA |
| Lsd10_PingPong    | 8192 | False         | 165,829.2 ns | 3,268.82 ns | 1,451.38 ns |  0.81 |         - |          NA |
|      |               |              |             |             |       |           |             |
| **Lsd4_Xor**          | **8192** | **True**          | **419,527.2 ns** | **1,641.18 ns** |   **858.37 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_Normalized   | 8192 | True          | 152,934.3 ns | 1,843.79 ns |   964.34 ns |  0.36 |         - |          NA |
| Lsd256_Xor        | 8192 | True          |  79,945.5 ns |   580.58 ns |   257.78 ns |  0.19 |         - |          NA |
| Lsd256_Normalized | 8192 | True          |  49,602.8 ns |   854.48 ns |   446.91 ns |  0.12 |         - |          NA |
| Lsd10_CopyBack    | 8192 | True          | 178,927.8 ns |   456.09 ns |   238.54 ns |  0.43 |         - |          NA |
| Lsd10_PingPong    | 8192 | True          | 167,562.6 ns | 2,777.95 ns | 1,452.92 ns |  0.40 |         - |          NA |

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

| Method              | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **SelectionSort**       | **256**  | **Random**             |    **24,606.7 ns** |    **432.37 ns** |    **191.98 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Random             |    18,678.4 ns |    204.66 ns |    107.04 ns |  0.76 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | Random             |    73,064.7 ns |  1,268.75 ns |    663.58 ns |  2.97 |    0.03 |    4 |         - |          NA |
| PancakeSort         | 256  | Random             |    40,865.4 ns |    451.69 ns |    200.55 ns |  1.66 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **SingleElementMoved** |    **24,479.6 ns** |     **52.20 ns** |     **23.18 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | SingleElementMoved |    22,904.3 ns |    111.32 ns |     49.43 ns |  0.94 |    0.00 |    1 |         - |          NA |
| CycleSort           | 256  | SingleElementMoved |    56,707.2 ns |  2,346.71 ns |  1,227.37 ns |  2.32 |    0.05 |    2 |         - |          NA |
| PancakeSort         | 256  | SingleElementMoved |    21,552.2 ns |  1,900.37 ns |    993.93 ns |  0.88 |    0.04 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **Sorted**             |    **24,854.0 ns** |    **348.78 ns** |    **124.38 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Sorted             |    12,418.6 ns |    291.18 ns |    129.29 ns |  0.50 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | Sorted             |    24,374.4 ns |     97.17 ns |     43.15 ns |  0.98 |    0.00 |    3 |         - |          NA |
| PancakeSort         | 256  | Sorted             |    16,848.4 ns |     67.06 ns |     29.78 ns |  0.68 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **Reversed**           |    **23,794.9 ns** |  **2,398.40 ns** |  **1,254.41 ns** |  **1.00** |    **0.07** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Reversed           |    18,430.3 ns |    173.65 ns |     90.82 ns |  0.78 |    0.04 |    1 |         - |          NA |
| CycleSort           | 256  | Reversed           |    50,426.1 ns |  1,689.02 ns |    883.39 ns |  2.12 |    0.11 |    3 |         - |          NA |
| PancakeSort         | 256  | Reversed           |    17,050.9 ns |    207.77 ns |    108.67 ns |  0.72 |    0.04 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **PipeOrgan**          |    **23,643.7 ns** |    **947.15 ns** |    **495.38 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | PipeOrgan          |    21,392.2 ns |    222.86 ns |    116.56 ns |  0.91 |    0.02 |    1 |         - |          NA |
| CycleSort           | 256  | PipeOrgan          |    69,742.0 ns |    895.30 ns |    397.52 ns |  2.95 |    0.06 |    3 |         - |          NA |
| PancakeSort         | 256  | PipeOrgan          |    36,926.7 ns |    374.01 ns |    166.06 ns |  1.56 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **ManyDuplicates**     |    **24,428.5 ns** |    **375.45 ns** |    **166.70 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | ManyDuplicates     |    18,571.2 ns |    251.08 ns |    111.48 ns |  0.76 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | ManyDuplicates     |    69,374.0 ns |  1,063.82 ns |    556.40 ns |  2.84 |    0.03 |    4 |         - |          NA |
| PancakeSort         | 256  | ManyDuplicates     |    38,511.0 ns |    464.88 ns |    206.41 ns |  1.58 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Random**             |   **406,165.9 ns** |  **1,432.23 ns** |    **749.08 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Random             |   301,357.3 ns |  1,644.96 ns |    730.37 ns |  0.74 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | Random             | 1,550,496.9 ns |  4,660.80 ns |  2,069.42 ns |  3.82 |    0.01 |    4 |         - |          NA |
| PancakeSort         | 1024 | Random             |   690,508.3 ns |  1,504.79 ns |    668.14 ns |  1.70 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **SingleElementMoved** |   **375,676.6 ns** |  **1,240.25 ns** |    **648.67 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | SingleElementMoved |   354,639.3 ns |  1,239.13 ns |    550.18 ns |  0.94 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | SingleElementMoved |   878,252.4 ns |  7,522.28 ns |  3,934.30 ns |  2.34 |    0.01 |    2 |         - |          NA |
| PancakeSort         | 1024 | SingleElementMoved |   343,820.2 ns | 14,347.96 ns |  7,504.26 ns |  0.92 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Sorted**             |   **375,967.5 ns** |  **1,537.71 ns** |    **804.25 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Sorted             |   188,727.0 ns |    685.15 ns |    358.35 ns |  0.50 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | Sorted             |   376,056.5 ns |  4,026.48 ns |  1,787.78 ns |  1.00 |    0.00 |    3 |         - |          NA |
| PancakeSort         | 1024 | Sorted             |   253,524.4 ns |  3,826.73 ns |  1,364.65 ns |  0.67 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Reversed**           |   **344,034.3 ns** |  **8,418.16 ns** |  **4,402.86 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Reversed           |   281,102.7 ns |  1,485.81 ns |    659.71 ns |  0.82 |    0.01 |    1 |         - |          NA |
| CycleSort           | 1024 | Reversed           |   760,006.2 ns |  5,704.03 ns |  2,983.32 ns |  2.21 |    0.03 |    3 |         - |          NA |
| PancakeSort         | 1024 | Reversed           |   268,195.6 ns | 19,194.04 ns | 10,038.86 ns |  0.78 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **PipeOrgan**          |   **389,425.4 ns** |  **4,838.81 ns** |  **2,530.79 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | PipeOrgan          |   346,054.5 ns |  1,676.04 ns |    744.17 ns |  0.89 |    0.01 |    1 |         - |          NA |
| CycleSort           | 1024 | PipeOrgan          | 1,190,411.0 ns |  9,852.94 ns |  5,153.28 ns |  3.06 |    0.02 |    3 |         - |          NA |
| PancakeSort         | 1024 | PipeOrgan          |   567,740.6 ns |  2,042.77 ns |  1,068.41 ns |  1.46 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **ManyDuplicates**     |   **395,368.4 ns** |  **2,615.29 ns** |  **1,367.85 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | ManyDuplicates     |   295,220.5 ns |  1,615.01 ns |    844.68 ns |  0.75 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | ManyDuplicates     | 1,527,939.5 ns |  8,220.20 ns |  4,299.32 ns |  3.86 |    0.02 |    4 |         - |          NA |
| PancakeSort         | 1024 | ManyDuplicates     |   634,748.6 ns |  2,684.12 ns |  1,191.77 ns |  1.61 |    0.01 |    3 |         - |          NA |

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
| **BalancedBinaryTreeSort** | **256**  | **Random**             |    **13,848.9 ns** |   **410.31 ns** |   **214.60 ns** |  **3.99** |    **0.18** |    **4** |         **-** |          **NA** |
| CartesianTreeSort      | 256  | Random             |     6,539.7 ns |   272.25 ns |   142.39 ns |  1.88 |    0.09 |    2 |         - |          NA |
| BinaryTreeSort         | 256  | Random             |     3,480.3 ns |   314.03 ns |   164.24 ns |  1.00 |    0.06 |    1 |         - |          NA |
| SplaySort              | 256  | Random             |    23,086.1 ns |   876.26 ns |   389.07 ns |  6.65 |    0.30 |    5 |         - |          NA |
| TreapSort              | 256  | Random             |     9,159.6 ns |   372.21 ns |   165.26 ns |  2.64 |    0.12 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **SingleElementMoved** |    **14,303.6 ns** |   **691.31 ns** |   **361.57 ns** |  **0.29** |    **0.01** |    **4** |         **-** |          **NA** |
| CartesianTreeSort      | 256  | SingleElementMoved |     2,322.4 ns |    12.94 ns |     5.75 ns |  0.05 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | SingleElementMoved |    49,019.2 ns |   422.62 ns |   221.04 ns |  1.00 |    0.01 |    5 |         - |          NA |
| SplaySort              | 256  | SingleElementMoved |     4,585.8 ns |   339.17 ns |   177.39 ns |  0.09 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | SingleElementMoved |     6,051.9 ns |   472.75 ns |   247.26 ns |  0.12 |    0.00 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Sorted**             |    **13,412.0 ns** |   **599.95 ns** |   **313.78 ns** |  **0.18** |    **0.00** |    **4** |         **-** |          **NA** |
| CartesianTreeSort      | 256  | Sorted             |     2,107.2 ns |    64.51 ns |    23.00 ns |  0.03 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | Sorted             |    76,076.4 ns |   380.62 ns |   169.00 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 256  | Sorted             |     3,812.6 ns |    19.66 ns |     7.01 ns |  0.05 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | Sorted             |     5,210.7 ns |   361.40 ns |   189.02 ns |  0.07 |    0.00 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Reversed**           |    **12,267.0 ns** |   **302.61 ns** |   **158.27 ns** |  **0.15** |    **0.00** |    **4** |         **-** |          **NA** |
| CartesianTreeSort      | 256  | Reversed           |     2,046.3 ns |    90.15 ns |    32.15 ns |  0.03 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | Reversed           |    79,961.8 ns |   255.53 ns |   113.46 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 256  | Reversed           |     3,701.7 ns |    45.21 ns |    16.12 ns |  0.05 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | Reversed           |     5,428.9 ns |   528.77 ns |   276.56 ns |  0.07 |    0.00 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **PipeOrgan**          |    **12,530.2 ns** |   **453.36 ns** |   **237.11 ns** |  **0.33** |    **0.01** |    **4** |         **-** |          **NA** |
| CartesianTreeSort      | 256  | PipeOrgan          |     2,396.2 ns |   187.49 ns |    98.06 ns |  0.06 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | PipeOrgan          |    37,472.3 ns |   429.65 ns |   224.71 ns |  1.00 |    0.01 |    5 |         - |          NA |
| SplaySort              | 256  | PipeOrgan          |     4,620.2 ns |   329.46 ns |   172.32 ns |  0.12 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | PipeOrgan          |     7,836.6 ns |   241.45 ns |   126.28 ns |  0.21 |    0.00 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **ManyDuplicates**     |    **13,949.9 ns** |   **822.87 ns** |   **430.38 ns** |  **3.39** |    **0.13** |    **4** |         **-** |          **NA** |
| CartesianTreeSort      | 256  | ManyDuplicates     |    11,120.0 ns | 5,535.24 ns | 2,895.04 ns |  2.70 |    0.67 |    3 |         - |          NA |
| BinaryTreeSort         | 256  | ManyDuplicates     |     4,118.7 ns |   227.91 ns |   119.20 ns |  1.00 |    0.04 |    1 |         - |          NA |
| SplaySort              | 256  | ManyDuplicates     |    21,961.7 ns |   316.07 ns |   140.34 ns |  5.34 |    0.15 |    5 |         - |          NA |
| TreapSort              | 256  | ManyDuplicates     |     8,074.7 ns |   409.46 ns |   214.16 ns |  1.96 |    0.07 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Random**             |    **75,692.6 ns** | **9,085.60 ns** | **4,751.95 ns** |  **3.86** |    **0.25** |    **4** |         **-** |          **NA** |
| CartesianTreeSort      | 1024 | Random             |    31,891.2 ns |   724.23 ns |   321.56 ns |  1.63 |    0.05 |    2 |         - |          NA |
| BinaryTreeSort         | 1024 | Random             |    19,609.9 ns | 1,098.98 ns |   574.79 ns |  1.00 |    0.04 |    1 |         - |          NA |
| SplaySort              | 1024 | Random             |   126,875.6 ns | 5,049.85 ns | 2,641.17 ns |  6.47 |    0.22 |    5 |         - |          NA |
| TreapSort              | 1024 | Random             |    39,208.8 ns | 2,023.86 ns | 1,058.52 ns |  2.00 |    0.07 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **SingleElementMoved** |    **79,921.7 ns** | **5,501.11 ns** | **2,877.19 ns** |  **0.10** |    **0.00** |    **4** |         **-** |          **NA** |
| CartesianTreeSort      | 1024 | SingleElementMoved |     8,919.7 ns |   240.03 ns |   125.54 ns |  0.01 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | SingleElementMoved |   779,221.1 ns | 1,376.78 ns |   720.08 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | SingleElementMoved |    17,521.6 ns |   180.82 ns |    80.28 ns |  0.02 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | SingleElementMoved |    26,930.3 ns |   360.58 ns |   160.10 ns |  0.03 |    0.00 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Sorted**             |    **70,429.5 ns** | **6,540.40 ns** | **3,420.76 ns** | **0.058** |    **0.00** |    **4** |         **-** |          **NA** |
| CartesianTreeSort      | 1024 | Sorted             |     7,995.1 ns |    15.32 ns |     6.80 ns | 0.007 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | Sorted             | 1,205,265.4 ns | 1,001.92 ns |   524.02 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | Sorted             |    15,415.8 ns |   188.71 ns |    83.79 ns | 0.013 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | Sorted             |    22,723.9 ns |   356.46 ns |   186.44 ns | 0.019 |    0.00 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Reversed**           |    **60,904.7 ns** |   **997.28 ns** |   **521.60 ns** | **0.048** |    **0.00** |    **4** |         **-** |          **NA** |
| CartesianTreeSort      | 1024 | Reversed           |     7,970.4 ns |   193.29 ns |   101.09 ns | 0.006 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | Reversed           | 1,277,602.3 ns |   523.69 ns |   273.90 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | Reversed           |    14,769.6 ns |   265.67 ns |   138.95 ns | 0.012 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | Reversed           |    24,117.1 ns | 1,117.13 ns |   496.01 ns | 0.019 |    0.00 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **PipeOrgan**          |    **64,488.3 ns** | **3,130.64 ns** | **1,637.38 ns** |  **0.11** |    **0.00** |    **4** |         **-** |          **NA** |
| CartesianTreeSort      | 1024 | PipeOrgan          |     8,796.3 ns |   282.63 ns |   125.49 ns |  0.01 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | PipeOrgan          |   599,356.7 ns | 1,620.70 ns |   719.60 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | PipeOrgan          |    17,433.0 ns |    95.16 ns |    33.94 ns |  0.03 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | PipeOrgan          |    34,371.3 ns | 1,226.27 ns |   641.36 ns |  0.06 |    0.00 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **ManyDuplicates**     |    **74,903.4 ns** | **6,415.83 ns** | **3,355.60 ns** |  **2.12** |    **0.09** |    **2** |         **-** |          **NA** |
| CartesianTreeSort      | 1024 | ManyDuplicates     |    34,922.0 ns | 1,561.94 ns |   816.92 ns |  0.99 |    0.02 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | ManyDuplicates     |    35,301.1 ns |   663.00 ns |   346.76 ns |  1.00 |    0.01 |    1 |         - |          NA |
| SplaySort              | 1024 | ManyDuplicates     |   108,309.7 ns | 4,483.50 ns | 2,344.96 ns |  3.07 |    0.07 |    3 |         - |          NA |
| TreapSort              | 1024 | ManyDuplicates     |    39,346.0 ns | 2,062.09 ns | 1,078.51 ns |  1.11 |    0.03 |    1 |         - |          NA |

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
