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
<summary>Benchmark results (2026-08-02 18:31 UTC)</summary>

Workflow run: https://github.com/guitarrapc/SortAlgorithms/actions/runs/30760616046

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
| **DropMergeSort** | **256**  | **Random**             |   **2,986.7 ns** |    **103.33 ns** |     **45.88 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | Random             |   5,350.2 ns |    599.25 ns |    266.07 ns |  1.79 |    0.09 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **256**  | **SingleElementMoved** |   **1,077.8 ns** |    **526.15 ns** |    **275.18 ns** |  **1.06** |    **0.36** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | SingleElementMoved |   7,988.4 ns |    205.21 ns |    107.33 ns |  7.84 |    1.82 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **256**  | **Sorted**             |     **642.4 ns** |     **92.63 ns** |     **48.45 ns** |  **1.00** |    **0.10** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | Sorted             |   7,767.2 ns |    166.18 ns |     73.78 ns | 12.15 |    0.83 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **256**  | **Reversed**           |   **1,526.6 ns** |     **45.51 ns** |     **20.21 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | Reversed           |   1,464.6 ns |     16.69 ns |      7.41 ns |  0.96 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **256**  | **PipeOrgan**          |   **6,324.4 ns** |    **501.33 ns** |    **178.78 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | PipeOrgan          |   5,455.6 ns |    255.49 ns |    133.63 ns |  0.86 |    0.03 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **256**  | **ManyDuplicates**     |   **3,001.7 ns** |    **392.24 ns** |    **205.15 ns** |  **1.00** |    **0.09** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | ManyDuplicates     |   3,786.5 ns |    187.46 ns |     83.23 ns |  1.27 |    0.08 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **Random**             |  **14,299.1 ns** |    **195.66 ns** |     **69.77 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | Random             |  24,786.0 ns |  1,155.80 ns |    604.51 ns |  1.73 |    0.04 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **SingleElementMoved** |   **2,503.0 ns** |      **5.88 ns** |      **2.10 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | SingleElementMoved |  40,223.7 ns |  2,050.16 ns |    910.28 ns | 16.07 |    0.34 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **Sorted**             |   **2,367.5 ns** |    **324.46 ns** |    **169.70 ns** |  **1.00** |    **0.10** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | Sorted             |  39,467.6 ns |    718.91 ns |    319.20 ns | 16.75 |    1.13 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **Reversed**           |   **6,987.9 ns** |    **363.70 ns** |    **161.48 ns** |  **1.00** |    **0.03** |    **2** |         **-** |          **NA** |
| PatienceSort  | 1024 | Reversed           |   5,273.3 ns |    432.62 ns |    226.27 ns |  0.75 |    0.03 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **PipeOrgan**          |  **27,240.1 ns** |    **885.87 ns** |    **463.32 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | PipeOrgan          |  27,272.4 ns |    686.91 ns |    359.27 ns |  1.00 |    0.02 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **ManyDuplicates**     |  **12,707.4 ns** |    **161.83 ns** |     **71.85 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | ManyDuplicates     |  14,874.7 ns |    535.83 ns |    237.91 ns |  1.17 |    0.02 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **4096** | **Random**             |  **71,554.4 ns** |  **3,335.07 ns** |  **1,480.79 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| PatienceSort  | 4096 | Random             | 151,665.0 ns | 18,864.31 ns |  9,866.40 ns |  2.12 |    0.14 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **4096** | **SingleElementMoved** |  **10,041.1 ns** |    **410.22 ns** |    **182.14 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| PatienceSort  | 4096 | SingleElementMoved | 246,442.2 ns | 33,871.39 ns | 17,715.39 ns | 24.55 |    1.72 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **4096** | **Sorted**             |   **8,809.0 ns** |    **376.44 ns** |    **196.89 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| PatienceSort  | 4096 | Sorted             | 218,437.2 ns | 24,712.58 ns | 12,925.16 ns | 24.81 |    1.48 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **4096** | **Reversed**           |  **30,787.0 ns** |    **574.75 ns** |    **255.19 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| PatienceSort  | 4096 | Reversed           |  20,596.5 ns |    825.82 ns |    431.92 ns |  0.67 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **4096** | **PipeOrgan**          | **112,547.5 ns** |  **3,336.73 ns** |  **1,481.53 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| PatienceSort  | 4096 | PipeOrgan          | 167,316.2 ns |  9,419.31 ns |  4,926.48 ns |  1.49 |    0.05 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **4096** | **ManyDuplicates**     |  **55,797.5 ns** |  **1,605.68 ns** |    **839.80 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| PatienceSort  | 4096 | ManyDuplicates     |  60,056.3 ns |    745.47 ns |    330.99 ns |  1.08 |    0.02 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **Random**             | **473,598.8 ns** |  **3,226.19 ns** |  **1,432.45 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | Random             | 815,895.2 ns |  3,903.04 ns |  2,041.37 ns |  1.72 |    0.01 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **SingleElementMoved** |  **19,824.3 ns** |    **175.32 ns** |     **62.52 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | SingleElementMoved | 770,379.0 ns |  3,750.23 ns |  1,961.44 ns | 38.86 |    0.15 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **Sorted**             |  **17,612.4 ns** |    **226.11 ns** |    **100.40 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | Sorted             | 778,341.0 ns |  4,754.65 ns |  2,111.09 ns | 44.19 |    0.26 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **Reversed**           |  **64,439.1 ns** |    **891.74 ns** |    **395.94 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| PatienceSort  | 8192 | Reversed           |  40,610.3 ns |  1,337.30 ns |    699.43 ns |  0.63 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **PipeOrgan**          | **226,713.4 ns** |  **1,955.73 ns** |    **868.36 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | PipeOrgan          | 667,490.0 ns |  2,863.33 ns |  1,497.58 ns |  2.94 |    0.01 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **ManyDuplicates**     | **123,453.4 ns** |  **3,718.20 ns** |  **1,944.69 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | ManyDuplicates     | 149,567.4 ns | 16,673.60 ns |  8,720.61 ns |  1.21 |    0.07 |    2 |         - |          NA |

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
| **StrandSort** | **256**  | **Random**             |   **7,447.1 ns** |  **1,386.73 ns** |   **725.28 ns** |  **1.01** |    **0.14** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **256**  | **SingleElementMoved** |     **798.2 ns** |      **9.20 ns** |     **4.08 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **256**  | **Sorted**             |     **591.5 ns** |     **59.78 ns** |    **26.54 ns** |  **1.00** |    **0.06** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **256**  | **Reversed**           |  **48,542.4 ns** |  **1,188.22 ns** |   **621.46 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **256**  | **PipeOrgan**          |  **28,152.6 ns** |  **1,065.65 ns** |   **557.36 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **256**  | **ManyDuplicates**     |   **4,882.2 ns** |     **70.89 ns** |    **31.48 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **1024** | **Random**             |  **54,657.3 ns** |    **730.17 ns** |   **324.20 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **1024** | **SingleElementMoved** |   **2,702.5 ns** |     **53.99 ns** |    **23.97 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **1024** | **Sorted**             |   **1,731.6 ns** |     **27.93 ns** |     **9.96 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **1024** | **Reversed**           | **770,645.8 ns** |  **6,787.78 ns** | **3,550.14 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **1024** | **PipeOrgan**          | **438,481.4 ns** | **19,196.28 ns** | **8,523.27 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **1024** | **ManyDuplicates**     |  **31,707.6 ns** |    **101.22 ns** |    **44.94 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |

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
| **Radix16_C16**            | **4096**    | **False**        |     **71,877.4 ns** |   **1,336.0 ns** |    **593.2 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| Radix256_Shipped       | 4096    | False        |     72,349.2 ns |     612.2 ns |    271.8 ns |  1.01 |    0.01 |         - |          NA |
| Radix256_Cycle         | 4096    | False        |     70,677.9 ns |     855.5 ns |    379.8 ns |  0.98 |    0.01 |         - |          NA |
| Radix256_BinaryLeaf    | 4096    | False        |     69,874.4 ns |   1,066.5 ns |    473.5 ns |  0.97 |    0.01 |         - |          NA |
| Radix256_PerNodeRescan | 4096    | False        |     79,252.0 ns |     463.0 ns |    242.2 ns |  1.10 |    0.01 |         - |          NA |
|         |              |                 |              |             |       |         |           |             |
| **Radix16_C16**            | **4096**    | **True**         |     **94,433.4 ns** |     **774.9 ns** |    **344.1 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Radix256_Shipped       | 4096    | True         |     49,347.4 ns |     750.2 ns |    392.4 ns |  0.52 |    0.00 |         - |          NA |
| Radix256_Cycle         | 4096    | True         |     47,321.3 ns |     872.6 ns |    387.4 ns |  0.50 |    0.00 |         - |          NA |
| Radix256_BinaryLeaf    | 4096    | True         |     59,873.0 ns |   2,444.1 ns |  1,085.2 ns |  0.63 |    0.01 |         - |          NA |
| Radix256_PerNodeRescan | 4096    | True         |     48,130.1 ns |   1,242.6 ns |    443.1 ns |  0.51 |    0.00 |         - |          NA |
|         |              |                 |              |             |       |         |           |             |
| **Radix16_C16**            | **8192**    | **False**        |    **175,998.1 ns** |   **1,962.8 ns** |    **871.5 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| Radix256_Shipped       | 8192    | False        |    144,210.7 ns |   1,037.5 ns |    460.7 ns |  0.82 |    0.00 |         - |          NA |
| Radix256_Cycle         | 8192    | False        |    142,488.4 ns |   7,316.2 ns |  3,248.4 ns |  0.81 |    0.02 |         - |          NA |
| Radix256_BinaryLeaf    | 8192    | False        |    139,714.8 ns |     377.2 ns |    167.5 ns |  0.79 |    0.00 |         - |          NA |
| Radix256_PerNodeRescan | 8192    | False        |    158,197.4 ns |     850.1 ns |    444.6 ns |  0.90 |    0.00 |         - |          NA |
|         |              |                 |              |             |       |         |           |             |
| **Radix16_C16**            | **8192**    | **True**         |    **211,342.2 ns** |   **3,409.0 ns** |  **1,783.0 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| Radix256_Shipped       | 8192    | True         |    116,636.3 ns |   3,358.6 ns |  1,756.6 ns |  0.55 |    0.01 |         - |          NA |
| Radix256_Cycle         | 8192    | True         |    112,471.7 ns |   3,136.2 ns |  1,640.3 ns |  0.53 |    0.01 |         - |          NA |
| Radix256_BinaryLeaf    | 8192    | True         |    155,151.5 ns |   9,449.6 ns |  4,942.3 ns |  0.73 |    0.02 |         - |          NA |
| Radix256_PerNodeRescan | 8192    | True         |    110,782.3 ns |   3,746.9 ns |  1,959.7 ns |  0.52 |    0.01 |         - |          NA |
|         |              |                 |              |             |       |         |           |             |
| **Radix16_C16**            | **65536**   | **False**        |  **2,322,739.1 ns** |   **6,117.1 ns** |  **2,716.0 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Radix256_Shipped       | 65536   | False        |  1,337,081.5 ns |   1,832.2 ns |    813.5 ns |  0.58 |    0.00 |         - |          NA |
| Radix256_Cycle         | 65536   | False        |  1,315,867.9 ns |   2,118.2 ns |    940.5 ns |  0.57 |    0.00 |         - |          NA |
| Radix256_BinaryLeaf    | 65536   | False        |  1,298,841.7 ns |   2,517.3 ns |  1,316.6 ns |  0.56 |    0.00 |         - |          NA |
| Radix256_PerNodeRescan | 65536   | False        |  1,460,701.1 ns |   1,525.0 ns |    677.1 ns |  0.63 |    0.00 |         - |          NA |
|         |              |                 |              |             |       |         |           |             |
| **Radix16_C16**            | **65536**   | **True**         |  **2,696,273.6 ns** |   **3,194.0 ns** |  **1,670.5 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Radix256_Shipped       | 65536   | True         |  1,689,322.9 ns |   3,282.5 ns |  1,716.8 ns |  0.63 |    0.00 |         - |          NA |
| Radix256_Cycle         | 65536   | True         |  1,723,739.6 ns |   1,661.7 ns |    869.1 ns |  0.64 |    0.00 |         - |          NA |
| Radix256_BinaryLeaf    | 65536   | True         |  1,874,530.5 ns |   2,965.4 ns |  1,316.6 ns |  0.70 |    0.00 |         - |          NA |
| Radix256_PerNodeRescan | 65536   | True         |  1,825,942.3 ns |   2,308.4 ns |  1,025.0 ns |  0.68 |    0.00 |         - |          NA |
|         |              |                 |              |             |       |         |           |             |
| **Radix16_C16**            | **1048576** | **False**        | **44,694,195.9 ns** |  **29,836.6 ns** | **13,247.6 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Radix256_Shipped       | 1048576 | False        | 29,201,538.7 ns |  45,338.0 ns | 23,712.6 ns |  0.65 |    0.00 |         - |          NA |
| Radix256_Cycle         | 1048576 | False        | 29,230,294.8 ns |  97,505.8 ns | 50,997.4 ns |  0.65 |    0.00 |         - |          NA |
| Radix256_BinaryLeaf    | 1048576 | False        | 28,692,341.9 ns |  29,311.3 ns | 10,452.7 ns |  0.64 |    0.00 |         - |          NA |
| Radix256_PerNodeRescan | 1048576 | False        | 32,789,615.9 ns | 190,504.4 ns | 84,585.2 ns |  0.73 |    0.00 |         - |          NA |
|         |              |                 |              |             |       |         |           |             |
| **Radix16_C16**            | **1048576** | **True**         | **50,475,254.2 ns** |  **24,700.4 ns** | **10,967.1 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Radix256_Shipped       | 1048576 | True         | 28,253,683.2 ns |  48,325.7 ns | 21,456.9 ns |  0.56 |    0.00 |         - |          NA |
| Radix256_Cycle         | 1048576 | True         | 28,315,119.9 ns | 177,534.4 ns | 92,853.9 ns |  0.56 |    0.00 |         - |          NA |
| Radix256_BinaryLeaf    | 1048576 | True         | 37,922,525.5 ns | 117,252.9 ns | 61,325.5 ns |  0.75 |    0.00 |         - |          NA |
| Radix256_PerNodeRescan | 1048576 | True         | 29,082,723.2 ns |  21,666.4 ns |  9,620.0 ns |  0.58 |    0.00 |         - |          NA |

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

| Method              | Size | Pattern            | Mean         | Error        | StdDev       | Median       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------------- |----- |------------------- |-------------:|-------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **CountingSort**        | **256**  | **Random**             |   **1,726.3 ns** |     **24.54 ns** |     **10.90 ns** |   **1,722.0 ns** |  **1.75** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | Random             |     988.4 ns |      6.35 ns |      3.32 ns |     986.4 ns |  1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 256  | Random             |   1,533.0 ns |     21.24 ns |      9.43 ns |   1,530.1 ns |  1.55 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | Random             |     693.7 ns |      2.75 ns |      1.22 ns |     693.9 ns |  0.70 |    0.00 |    1 |         - |          NA |
| BucketSort          | 256  | Random             |   2,096.4 ns |     11.40 ns |      4.07 ns |   2,095.8 ns |  2.12 |    0.01 |    3 |         - |          NA |
| BucketSortInteger   | 256  | Random             |   1,546.3 ns |      6.13 ns |      2.72 ns |   1,546.6 ns |  1.56 |    0.01 |    3 |         - |          NA |
| FlashSort           | 256  | Random             |   4,471.7 ns |     44.82 ns |     15.98 ns |   4,468.9 ns |  4.52 |    0.02 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | Random             |   2,861.4 ns |     10.91 ns |      4.84 ns |   2,862.3 ns |  2.90 |    0.01 |    4 |         - |          NA |
| RadixLSD256Sort     | 256  | Random             |   1,285.9 ns |      7.83 ns |      3.48 ns |   1,284.7 ns |  1.30 |    0.01 |    3 |         - |          NA |
| RadixLSD10Sort      | 256  | Random             |   4,132.1 ns |     10.54 ns |      3.76 ns |   4,131.3 ns |  4.18 |    0.01 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | Random             |   3,459.6 ns |    325.94 ns |    144.72 ns |   3,404.6 ns |  3.50 |    0.14 |    5 |         - |          NA |
| RadixMSD10Sort      | 256  | Random             |   4,011.1 ns |     68.06 ns |     24.27 ns |   4,016.6 ns |  4.06 |    0.03 |    5 |         - |          NA |
| AmericanFlagSort    | 256  | Random             |   2,863.1 ns |    219.49 ns |     97.45 ns |   2,823.2 ns |  2.90 |    0.09 |    4 |         - |          NA |
| SpreadSort          | 256  | Random             |   1,881.8 ns |     79.90 ns |     35.48 ns |   1,870.0 ns |  1.90 |    0.03 |    3 |         - |          NA |
|      |                    |              |              |              |              |       |         |      |           |             |
| **CountingSort**        | **256**  | **SingleElementMoved** |   **1,594.1 ns** |     **47.42 ns** |     **21.05 ns** |   **1,587.5 ns** |  **1.48** |    **0.02** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | SingleElementMoved |   1,073.7 ns |     13.09 ns |      5.81 ns |   1,071.9 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PigeonSort          | 256  | SingleElementMoved |   1,368.8 ns |     11.19 ns |      5.85 ns |   1,369.5 ns |  1.27 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | SingleElementMoved |     698.5 ns |      7.45 ns |      2.66 ns |     698.9 ns |  0.65 |    0.00 |    1 |         - |          NA |
| BucketSort          | 256  | SingleElementMoved |   1,971.1 ns |     33.12 ns |     11.81 ns |   1,967.7 ns |  1.84 |    0.01 |    3 |         - |          NA |
| BucketSortInteger   | 256  | SingleElementMoved |   1,618.6 ns |      5.70 ns |      2.53 ns |   1,618.9 ns |  1.51 |    0.01 |    2 |         - |          NA |
| FlashSort           | 256  | SingleElementMoved |   5,351.0 ns |     32.96 ns |     11.75 ns |   5,350.7 ns |  4.98 |    0.03 |    6 |         - |          NA |
| RadixLSD4Sort       | 256  | SingleElementMoved |   2,895.5 ns |    112.81 ns |     59.00 ns |   2,887.1 ns |  2.70 |    0.05 |    4 |         - |          NA |
| RadixLSD256Sort     | 256  | SingleElementMoved |   1,282.6 ns |    145.67 ns |     64.68 ns |   1,253.1 ns |  1.19 |    0.06 |    2 |         - |          NA |
| RadixLSD10Sort      | 256  | SingleElementMoved |   3,967.8 ns |    109.54 ns |     39.06 ns |   3,976.4 ns |  3.70 |    0.04 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | SingleElementMoved |   3,032.4 ns |    574.86 ns |    300.66 ns |   2,880.6 ns |  2.82 |    0.26 |    4 |         - |          NA |
| RadixMSD10Sort      | 256  | SingleElementMoved |   3,859.0 ns |     36.08 ns |     12.87 ns |   3,859.2 ns |  3.59 |    0.02 |    5 |         - |          NA |
| AmericanFlagSort    | 256  | SingleElementMoved |   2,312.1 ns |     20.11 ns |      7.17 ns |   2,310.7 ns |  2.15 |    0.01 |    3 |         - |          NA |
| SpreadSort          | 256  | SingleElementMoved |   1,173.1 ns |     43.73 ns |     19.42 ns |   1,166.7 ns |  1.09 |    0.02 |    2 |         - |          NA |
|      |                    |              |              |              |              |       |         |      |           |             |
| **CountingSort**        | **256**  | **Sorted**             |   **1,530.6 ns** |     **62.72 ns** |     **22.37 ns** |   **1,530.2 ns** |  **1.69** |    **0.02** |    **5** |         **-** |          **NA** |
| CountingSortInteger | 256  | Sorted             |     906.6 ns |      5.16 ns |      2.29 ns |     906.4 ns |  1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 256  | Sorted             |   1,447.5 ns |     13.89 ns |      6.17 ns |   1,445.3 ns |  1.60 |    0.01 |    5 |         - |          NA |
| PigeonSortInteger   | 256  | Sorted             |     623.6 ns |      7.66 ns |      3.40 ns |     623.9 ns |  0.69 |    0.00 |    2 |         - |          NA |
| BucketSort          | 256  | Sorted             |   2,205.5 ns |    325.65 ns |    170.32 ns |   2,089.8 ns |  2.43 |    0.18 |    6 |         - |          NA |
| BucketSortInteger   | 256  | Sorted             |   1,499.9 ns |    135.96 ns |     60.36 ns |   1,469.9 ns |  1.65 |    0.06 |    5 |         - |          NA |
| FlashSort           | 256  | Sorted             |   5,470.9 ns |    297.26 ns |    155.47 ns |   5,415.4 ns |  6.03 |    0.16 |    9 |         - |          NA |
| RadixLSD4Sort       | 256  | Sorted             |   2,812.7 ns |    169.45 ns |     75.24 ns |   2,795.6 ns |  3.10 |    0.08 |    7 |         - |          NA |
| RadixLSD256Sort     | 256  | Sorted             |   1,177.2 ns |     14.06 ns |      6.24 ns |   1,175.8 ns |  1.30 |    0.01 |    4 |         - |          NA |
| RadixLSD10Sort      | 256  | Sorted             |   4,155.8 ns |    321.26 ns |    168.02 ns |   4,067.5 ns |  4.58 |    0.18 |    8 |         - |          NA |
| RadixMSD4Sort       | 256  | Sorted             |   2,646.1 ns |     66.99 ns |     35.04 ns |   2,628.8 ns |  2.92 |    0.04 |    7 |         - |          NA |
| RadixMSD10Sort      | 256  | Sorted             |   3,817.6 ns |     61.23 ns |     21.83 ns |   3,813.9 ns |  4.21 |    0.02 |    8 |         - |          NA |
| AmericanFlagSort    | 256  | Sorted             |   1,517.9 ns |     10.19 ns |      5.33 ns |   1,515.1 ns |  1.67 |    0.01 |    5 |         - |          NA |
| SpreadSort          | 256  | Sorted             |     468.2 ns |    155.62 ns |     81.39 ns |     430.6 ns |  0.52 |    0.08 |    1 |         - |          NA |
|      |                    |              |              |              |              |       |         |      |           |             |
| **CountingSort**        | **256**  | **Reversed**           |   **1,498.0 ns** |     **11.98 ns** |      **5.32 ns** |   **1,499.1 ns** |  **1.49** |    **0.02** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | Reversed           |   1,007.8 ns |     35.58 ns |     15.80 ns |   1,003.2 ns |  1.00 |    0.02 |    2 |         - |          NA |
| PigeonSort          | 256  | Reversed           |   1,334.9 ns |     13.30 ns |      5.90 ns |   1,332.8 ns |  1.32 |    0.02 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | Reversed           |     619.9 ns |     13.89 ns |      7.26 ns |     615.8 ns |  0.62 |    0.01 |    1 |         - |          NA |
| BucketSort          | 256  | Reversed           |   1,994.1 ns |     81.65 ns |     36.25 ns |   2,014.9 ns |  1.98 |    0.04 |    2 |         - |          NA |
| BucketSortInteger   | 256  | Reversed           |   1,581.4 ns |     10.53 ns |      5.51 ns |   1,580.8 ns |  1.57 |    0.02 |    2 |         - |          NA |
| FlashSort           | 256  | Reversed           |   4,765.4 ns |     59.12 ns |     26.25 ns |   4,757.9 ns |  4.73 |    0.07 |    4 |         - |          NA |
| RadixLSD4Sort       | 256  | Reversed           |   2,867.4 ns |    163.11 ns |     72.42 ns |   2,852.9 ns |  2.85 |    0.08 |    3 |         - |          NA |
| RadixLSD256Sort     | 256  | Reversed           |   1,131.3 ns |     88.14 ns |     39.13 ns |   1,111.5 ns |  1.12 |    0.04 |    2 |         - |          NA |
| RadixLSD10Sort      | 256  | Reversed           |   3,896.7 ns |    210.26 ns |     93.36 ns |   3,849.1 ns |  3.87 |    0.10 |    4 |         - |          NA |
| RadixMSD4Sort       | 256  | Reversed           |   3,705.2 ns |    358.23 ns |    187.36 ns |   3,597.4 ns |  3.68 |    0.18 |    4 |         - |          NA |
| RadixMSD10Sort      | 256  | Reversed           |   4,332.6 ns |     18.50 ns |      6.60 ns |   4,330.7 ns |  4.30 |    0.06 |    4 |         - |          NA |
| AmericanFlagSort    | 256  | Reversed           |   1,770.3 ns |      8.64 ns |      3.08 ns |   1,769.6 ns |  1.76 |    0.03 |    2 |         - |          NA |
| SpreadSort          | 256  | Reversed           |     661.5 ns |    196.63 ns |     87.30 ns |     688.7 ns |  0.66 |    0.08 |    1 |         - |          NA |
|      |                    |              |              |              |              |       |         |      |           |             |
| **CountingSort**        | **256**  | **PipeOrgan**          |   **1,560.1 ns** |     **49.50 ns** |     **21.98 ns** |   **1,564.4 ns** |  **1.44** |    **0.02** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | PipeOrgan          |   1,086.9 ns |     12.31 ns |      6.44 ns |   1,087.6 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PigeonSort          | 256  | PipeOrgan          |   1,370.4 ns |     20.97 ns |      9.31 ns |   1,370.4 ns |  1.26 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | PipeOrgan          |     695.0 ns |      2.75 ns |      1.22 ns |     695.0 ns |  0.64 |    0.00 |    1 |         - |          NA |
| BucketSort          | 256  | PipeOrgan          |   2,028.8 ns |     57.95 ns |     25.73 ns |   2,031.5 ns |  1.87 |    0.02 |    2 |         - |          NA |
| BucketSortInteger   | 256  | PipeOrgan          |   1,662.3 ns |     10.01 ns |      5.24 ns |   1,660.9 ns |  1.53 |    0.01 |    2 |         - |          NA |
| FlashSort           | 256  | PipeOrgan          |   5,080.6 ns |     64.90 ns |     23.14 ns |   5,079.6 ns |  4.67 |    0.03 |    4 |         - |          NA |
| RadixLSD4Sort       | 256  | PipeOrgan          |   3,077.1 ns |    326.95 ns |    171.00 ns |   2,975.5 ns |  2.83 |    0.15 |    3 |         - |          NA |
| RadixLSD256Sort     | 256  | PipeOrgan          |   1,183.0 ns |     28.15 ns |     12.50 ns |   1,187.4 ns |  1.09 |    0.01 |    2 |         - |          NA |
| RadixLSD10Sort      | 256  | PipeOrgan          |   4,013.1 ns |    335.41 ns |    175.43 ns |   3,919.2 ns |  3.69 |    0.15 |    3 |         - |          NA |
| RadixMSD4Sort       | 256  | PipeOrgan          |   3,511.3 ns |    482.52 ns |    252.37 ns |   3,338.9 ns |  3.23 |    0.22 |    3 |         - |          NA |
| RadixMSD10Sort      | 256  | PipeOrgan          |   4,107.5 ns |     39.28 ns |     14.01 ns |   4,102.3 ns |  3.78 |    0.02 |    3 |         - |          NA |
| AmericanFlagSort    | 256  | PipeOrgan          |   2,245.2 ns |     10.68 ns |      3.81 ns |   2,245.7 ns |  2.07 |    0.01 |    2 |         - |          NA |
| SpreadSort          | 256  | PipeOrgan          |   1,830.5 ns |    276.95 ns |    122.97 ns |   1,784.5 ns |  1.68 |    0.11 |    2 |         - |          NA |
|      |                    |              |              |              |              |       |         |      |           |             |
| **CountingSort**        | **256**  | **ManyDuplicates**     |   **1,529.4 ns** |     **18.57 ns** |      **9.71 ns** |   **1,526.8 ns** |  **1.71** |    **0.02** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | ManyDuplicates     |     894.9 ns |     14.85 ns |      7.77 ns |     893.5 ns |  1.00 |    0.01 |    1 |         - |          NA |
| PigeonSort          | 256  | ManyDuplicates     |   1,507.2 ns |    171.95 ns |     76.34 ns |   1,478.9 ns |  1.68 |    0.08 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | ManyDuplicates     |     780.4 ns |    144.56 ns |     64.19 ns |     809.3 ns |  0.87 |    0.07 |    1 |         - |          NA |
| BucketSort          | 256  | ManyDuplicates     |   3,048.2 ns |     51.77 ns |     22.98 ns |   3,038.2 ns |  3.41 |    0.04 |    4 |         - |          NA |
| BucketSortInteger   | 256  | ManyDuplicates     |   1,723.0 ns |     10.98 ns |      4.88 ns |   1,722.3 ns |  1.93 |    0.02 |    2 |         - |          NA |
| FlashSort           | 256  | ManyDuplicates     |   4,543.9 ns |     15.88 ns |      5.66 ns |   4,545.0 ns |  5.08 |    0.04 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | ManyDuplicates     |   2,329.3 ns |     13.05 ns |      6.83 ns |   2,330.9 ns |  2.60 |    0.02 |    3 |         - |          NA |
| RadixLSD256Sort     | 256  | ManyDuplicates     |   1,332.7 ns |     53.58 ns |     23.79 ns |   1,332.6 ns |  1.49 |    0.03 |    2 |         - |          NA |
| RadixLSD10Sort      | 256  | ManyDuplicates     |   3,072.2 ns |    353.72 ns |    185.00 ns |   2,964.4 ns |  3.43 |    0.20 |    4 |         - |          NA |
| RadixMSD4Sort       | 256  | ManyDuplicates     |   2,888.6 ns |     17.05 ns |      7.57 ns |   2,891.9 ns |  3.23 |    0.03 |    4 |         - |          NA |
| RadixMSD10Sort      | 256  | ManyDuplicates     |   3,726.6 ns |    267.10 ns |    139.70 ns |   3,637.0 ns |  4.16 |    0.15 |    4 |         - |          NA |
| AmericanFlagSort    | 256  | ManyDuplicates     |   3,243.2 ns |     15.03 ns |      7.86 ns |   3,242.8 ns |  3.62 |    0.03 |    4 |         - |          NA |
| SpreadSort          | 256  | ManyDuplicates     |   1,751.2 ns |    189.91 ns |     99.33 ns |   1,746.2 ns |  1.96 |    0.11 |    2 |         - |          NA |
|      |                    |              |              |              |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **Random**             |   **6,084.2 ns** |    **329.65 ns** |    **172.41 ns** |   **6,134.1 ns** |  **1.53** |    **0.05** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Random             |   3,972.3 ns |    191.60 ns |     68.33 ns |   3,991.1 ns |  1.00 |    0.02 |    2 |         - |          NA |
| PigeonSort          | 1024 | Random             |   5,738.6 ns |    323.19 ns |    169.04 ns |   5,666.7 ns |  1.45 |    0.05 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | Random             |   2,834.3 ns |     15.81 ns |      7.02 ns |   2,830.8 ns |  0.71 |    0.01 |    1 |         - |          NA |
| BucketSort          | 1024 | Random             |   8,023.3 ns |     13.26 ns |      5.89 ns |   8,024.1 ns |  2.02 |    0.03 |    4 |         - |          NA |
| BucketSortInteger   | 1024 | Random             |   5,996.5 ns |    360.20 ns |    188.39 ns |   5,952.5 ns |  1.51 |    0.05 |    3 |         - |          NA |
| FlashSort           | 1024 | Random             |  18,631.0 ns |    297.32 ns |    132.01 ns |  18,611.0 ns |  4.69 |    0.08 |    6 |         - |          NA |
| RadixLSD4Sort       | 1024 | Random             |  13,901.0 ns |    276.29 ns |    122.68 ns |  13,956.8 ns |  3.50 |    0.06 |    5 |         - |          NA |
| RadixLSD256Sort     | 1024 | Random             |   7,772.0 ns |     72.31 ns |     37.82 ns |   7,761.1 ns |  1.96 |    0.03 |    4 |         - |          NA |
| RadixLSD10Sort      | 1024 | Random             |  21,225.6 ns |    213.79 ns |    111.81 ns |  21,232.3 ns |  5.34 |    0.09 |    6 |         - |          NA |
| RadixMSD4Sort       | 1024 | Random             |  14,826.9 ns |     93.47 ns |     41.50 ns |  14,814.3 ns |  3.73 |    0.06 |    5 |         - |          NA |
| RadixMSD10Sort      | 1024 | Random             |  22,170.1 ns |    495.34 ns |    259.07 ns |  22,207.3 ns |  5.58 |    0.11 |    6 |         - |          NA |
| AmericanFlagSort    | 1024 | Random             |  18,472.2 ns |    256.17 ns |    133.98 ns |  18,416.1 ns |  4.65 |    0.08 |    6 |         - |          NA |
| SpreadSort          | 1024 | Random             |   9,091.8 ns |    290.68 ns |    152.03 ns |   9,017.3 ns |  2.29 |    0.05 |    4 |         - |          NA |
|      |                    |              |              |              |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **SingleElementMoved** |   **6,206.7 ns** |    **395.89 ns** |    **207.06 ns** |   **6,072.3 ns** |  **1.46** |    **0.06** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | SingleElementMoved |   4,256.3 ns |    271.33 ns |    141.91 ns |   4,185.2 ns |  1.00 |    0.04 |    2 |         - |          NA |
| PigeonSort          | 1024 | SingleElementMoved |   4,910.5 ns |     56.66 ns |     20.20 ns |   4,901.7 ns |  1.15 |    0.04 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | SingleElementMoved |   2,951.2 ns |    366.95 ns |    191.92 ns |   2,855.3 ns |  0.69 |    0.05 |    1 |         - |          NA |
| BucketSort          | 1024 | SingleElementMoved |   7,452.1 ns |     11.90 ns |      6.23 ns |   7,450.6 ns |  1.75 |    0.05 |    3 |         - |          NA |
| BucketSortInteger   | 1024 | SingleElementMoved |   6,219.1 ns |     24.40 ns |      8.70 ns |   6,216.9 ns |  1.46 |    0.05 |    3 |         - |          NA |
| FlashSort           | 1024 | SingleElementMoved |  21,450.6 ns |    166.30 ns |     86.98 ns |  21,460.8 ns |  5.04 |    0.16 |    5 |         - |          NA |
| RadixLSD4Sort       | 1024 | SingleElementMoved |  15,201.1 ns |  2,335.24 ns |  1,036.86 ns |  14,576.3 ns |  3.57 |    0.25 |    4 |         - |          NA |
| RadixLSD256Sort     | 1024 | SingleElementMoved |   6,585.4 ns |    227.15 ns |    100.86 ns |   6,603.1 ns |  1.55 |    0.05 |    3 |         - |          NA |
| RadixLSD10Sort      | 1024 | SingleElementMoved |  21,256.5 ns |    535.95 ns |    237.97 ns |  21,256.2 ns |  5.00 |    0.16 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | SingleElementMoved |  12,813.1 ns |    179.30 ns |     93.78 ns |  12,826.3 ns |  3.01 |    0.10 |    4 |         - |          NA |
| RadixMSD10Sort      | 1024 | SingleElementMoved |  19,418.2 ns |    262.57 ns |    137.33 ns |  19,389.9 ns |  4.57 |    0.14 |    5 |         - |          NA |
| AmericanFlagSort    | 1024 | SingleElementMoved |  12,960.2 ns |    309.40 ns |    161.82 ns |  13,025.9 ns |  3.05 |    0.10 |    4 |         - |          NA |
| SpreadSort          | 1024 | SingleElementMoved |   6,788.0 ns |    181.46 ns |     80.57 ns |   6,760.1 ns |  1.60 |    0.05 |    3 |         - |          NA |
|      |                    |              |              |              |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **Sorted**             |   **5,739.6 ns** |    **324.87 ns** |    **169.91 ns** |   **5,699.7 ns** |  **1.68** |    **0.06** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Sorted             |   3,418.0 ns |    183.77 ns |     81.59 ns |   3,382.1 ns |  1.00 |    0.03 |    3 |         - |          NA |
| PigeonSort          | 1024 | Sorted             |   5,415.0 ns |    390.43 ns |    204.20 ns |   5,332.7 ns |  1.58 |    0.07 |    4 |         - |          NA |
| PigeonSortInteger   | 1024 | Sorted             |   2,433.4 ns |      6.33 ns |      2.81 ns |   2,432.9 ns |  0.71 |    0.02 |    2 |         - |          NA |
| BucketSort          | 1024 | Sorted             |   8,164.3 ns |     87.56 ns |     45.79 ns |   8,163.2 ns |  2.39 |    0.05 |    5 |         - |          NA |
| BucketSortInteger   | 1024 | Sorted             |   5,667.9 ns |    413.13 ns |    216.08 ns |   5,520.2 ns |  1.66 |    0.07 |    4 |         - |          NA |
| FlashSort           | 1024 | Sorted             |  21,344.2 ns |    261.28 ns |    136.66 ns |  21,282.4 ns |  6.25 |    0.14 |    7 |         - |          NA |
| RadixLSD4Sort       | 1024 | Sorted             |  17,125.5 ns |    321.98 ns |    168.40 ns |  17,132.4 ns |  5.01 |    0.12 |    7 |         - |          NA |
| RadixLSD256Sort     | 1024 | Sorted             |   6,705.1 ns |    260.64 ns |    136.32 ns |   6,637.9 ns |  1.96 |    0.06 |    4 |         - |          NA |
| RadixLSD10Sort      | 1024 | Sorted             |  21,136.8 ns |    335.41 ns |    175.43 ns |  21,085.3 ns |  6.19 |    0.14 |    7 |         - |          NA |
| RadixMSD4Sort       | 1024 | Sorted             |  12,670.9 ns |    307.97 ns |    161.07 ns |  12,704.6 ns |  3.71 |    0.09 |    6 |         - |          NA |
| RadixMSD10Sort      | 1024 | Sorted             |  19,697.6 ns |    175.36 ns |     91.72 ns |  19,679.1 ns |  5.77 |    0.13 |    7 |         - |          NA |
| AmericanFlagSort    | 1024 | Sorted             |   9,627.8 ns |    289.67 ns |    128.62 ns |   9,682.1 ns |  2.82 |    0.07 |    5 |         - |          NA |
| SpreadSort          | 1024 | Sorted             |     625.9 ns |     26.78 ns |     11.89 ns |     620.7 ns |  0.18 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |              |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **Reversed**           |   **5,683.8 ns** |    **368.83 ns** |    **192.91 ns** |   **5,587.5 ns** |  **1.26** |    **0.20** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Reversed           |   4,612.3 ns |  1,559.52 ns |    815.66 ns |   4,183.1 ns |  1.03 |    0.24 |    2 |         - |          NA |
| PigeonSort          | 1024 | Reversed           |   5,199.5 ns |    493.78 ns |    258.26 ns |   5,273.1 ns |  1.16 |    0.19 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | Reversed           |   2,429.0 ns |      5.82 ns |      2.59 ns |   2,428.9 ns |  0.54 |    0.08 |    1 |         - |          NA |
| BucketSort          | 1024 | Reversed           |   7,817.6 ns |    443.53 ns |    231.97 ns |   7,937.8 ns |  1.74 |    0.27 |    4 |         - |          NA |
| BucketSortInteger   | 1024 | Reversed           |   5,890.8 ns |    263.59 ns |    137.86 ns |   5,825.6 ns |  1.31 |    0.21 |    3 |         - |          NA |
| FlashSort           | 1024 | Reversed           |  18,583.1 ns |     93.07 ns |     41.32 ns |  18,575.2 ns |  4.13 |    0.64 |    6 |         - |          NA |
| RadixLSD4Sort       | 1024 | Reversed           |  15,939.1 ns |    393.66 ns |    205.89 ns |  15,819.6 ns |  3.55 |    0.55 |    6 |         - |          NA |
| RadixLSD256Sort     | 1024 | Reversed           |   6,012.9 ns |     58.53 ns |     20.87 ns |   6,009.9 ns |  1.34 |    0.21 |    3 |         - |          NA |
| RadixLSD10Sort      | 1024 | Reversed           |  21,226.8 ns |    468.03 ns |    244.79 ns |  21,175.1 ns |  4.72 |    0.73 |    6 |         - |          NA |
| RadixMSD4Sort       | 1024 | Reversed           |  16,610.1 ns |    173.18 ns |     90.58 ns |  16,575.0 ns |  3.70 |    0.57 |    6 |         - |          NA |
| RadixMSD10Sort      | 1024 | Reversed           |  22,600.8 ns |    177.42 ns |     78.77 ns |  22,596.2 ns |  5.03 |    0.78 |    6 |         - |          NA |
| AmericanFlagSort    | 1024 | Reversed           |  12,085.5 ns |    172.24 ns |     90.09 ns |  12,106.5 ns |  2.69 |    0.42 |    5 |         - |          NA |
| SpreadSort          | 1024 | Reversed           |   5,073.6 ns |    108.13 ns |     38.56 ns |   5,072.6 ns |  1.13 |    0.18 |    3 |         - |          NA |
|      |                    |              |              |              |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **PipeOrgan**          |   **5,880.6 ns** |    **356.68 ns** |    **186.55 ns** |   **5,840.5 ns** |  **1.39** |    **0.07** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | PipeOrgan          |   4,243.9 ns |    366.50 ns |    191.69 ns |   4,110.7 ns |  1.00 |    0.06 |    2 |         - |          NA |
| PigeonSort          | 1024 | PipeOrgan          |   5,124.9 ns |    496.02 ns |    259.43 ns |   5,008.6 ns |  1.21 |    0.08 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | PipeOrgan          |   2,790.7 ns |      5.03 ns |      2.23 ns |   2,790.8 ns |  0.66 |    0.03 |    1 |         - |          NA |
| BucketSort          | 1024 | PipeOrgan          |   7,587.0 ns |     44.17 ns |     19.61 ns |   7,589.6 ns |  1.79 |    0.07 |    3 |         - |          NA |
| BucketSortInteger   | 1024 | PipeOrgan          |   6,356.4 ns |    351.65 ns |    183.92 ns |   6,334.0 ns |  1.50 |    0.07 |    3 |         - |          NA |
| FlashSort           | 1024 | PipeOrgan          |  19,663.4 ns |    156.16 ns |     81.68 ns |  19,636.3 ns |  4.64 |    0.19 |    4 |         - |          NA |
| RadixLSD4Sort       | 1024 | PipeOrgan          |  14,735.5 ns |    247.70 ns |    129.55 ns |  14,760.4 ns |  3.48 |    0.15 |    4 |         - |          NA |
| RadixLSD256Sort     | 1024 | PipeOrgan          |   6,566.2 ns |    276.36 ns |    144.54 ns |   6,579.3 ns |  1.55 |    0.07 |    3 |         - |          NA |
| RadixLSD10Sort      | 1024 | PipeOrgan          |  21,178.8 ns |    242.72 ns |    107.77 ns |  21,210.8 ns |  5.00 |    0.21 |    4 |         - |          NA |
| RadixMSD4Sort       | 1024 | PipeOrgan          |  16,871.3 ns |    639.73 ns |    334.59 ns |  16,950.7 ns |  3.98 |    0.18 |    4 |         - |          NA |
| RadixMSD10Sort      | 1024 | PipeOrgan          |  21,134.8 ns |    528.97 ns |    276.66 ns |  21,131.2 ns |  4.99 |    0.22 |    4 |         - |          NA |
| AmericanFlagSort    | 1024 | PipeOrgan          |  14,896.5 ns |    246.72 ns |    129.04 ns |  14,939.0 ns |  3.52 |    0.15 |    4 |         - |          NA |
| SpreadSort          | 1024 | PipeOrgan          |   7,306.1 ns |    235.89 ns |    123.37 ns |   7,369.8 ns |  1.72 |    0.08 |    3 |         - |          NA |
|      |                    |              |              |              |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **ManyDuplicates**     |   **5,432.1 ns** |     **21.25 ns** |      **7.58 ns** |   **5,429.2 ns** |  **1.67** |    **0.02** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 1024 | ManyDuplicates     |   3,255.1 ns |    107.47 ns |     47.72 ns |   3,236.4 ns |  1.00 |    0.02 |    2 |         - |          NA |
| PigeonSort          | 1024 | ManyDuplicates     |   5,664.9 ns |     23.68 ns |      8.45 ns |   5,663.8 ns |  1.74 |    0.02 |    4 |         - |          NA |
| PigeonSortInteger   | 1024 | ManyDuplicates     |   2,635.9 ns |    573.83 ns |    300.12 ns |   2,438.6 ns |  0.81 |    0.09 |    1 |         - |          NA |
| BucketSort          | 1024 | ManyDuplicates     |  12,080.0 ns |    305.72 ns |    159.90 ns |  12,144.7 ns |  3.71 |    0.07 |    5 |         - |          NA |
| BucketSortInteger   | 1024 | ManyDuplicates     |   6,688.6 ns |    323.41 ns |    169.15 ns |   6,657.4 ns |  2.06 |    0.06 |    4 |         - |          NA |
| FlashSort           | 1024 | ManyDuplicates     |  19,701.9 ns |     28.10 ns |     10.02 ns |  19,700.1 ns |  6.05 |    0.08 |    6 |         - |          NA |
| RadixLSD4Sort       | 1024 | ManyDuplicates     |   9,427.9 ns |    359.91 ns |    188.24 ns |   9,461.0 ns |  2.90 |    0.07 |    5 |         - |          NA |
| RadixLSD256Sort     | 1024 | ManyDuplicates     |   4,362.1 ns |    250.63 ns |    131.08 ns |   4,345.6 ns |  1.34 |    0.04 |    3 |         - |          NA |
| RadixLSD10Sort      | 1024 | ManyDuplicates     |  11,412.0 ns |    260.31 ns |    115.58 ns |  11,448.5 ns |  3.51 |    0.06 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | ManyDuplicates     |  10,732.6 ns |    235.54 ns |    123.19 ns |  10,781.1 ns |  3.30 |    0.06 |    5 |         - |          NA |
| RadixMSD10Sort      | 1024 | ManyDuplicates     |  12,749.3 ns |    287.93 ns |    150.59 ns |  12,815.1 ns |  3.92 |    0.07 |    5 |         - |          NA |
| AmericanFlagSort    | 1024 | ManyDuplicates     |   9,841.4 ns |    342.30 ns |    179.03 ns |   9,875.1 ns |  3.02 |    0.07 |    5 |         - |          NA |
| SpreadSort          | 1024 | ManyDuplicates     |   6,555.1 ns |     26.33 ns |      9.39 ns |   6,555.1 ns |  2.01 |    0.03 |    4 |         - |          NA |
|      |                    |              |              |              |              |       |         |      |           |             |
| **CountingSort**        | **4096** | **Random**             |  **24,680.6 ns** |    **729.26 ns** |    **323.80 ns** |  **24,766.3 ns** |  **1.58** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Random             |  15,645.5 ns |     82.47 ns |     36.62 ns |  15,635.7 ns |  1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | Random             |  22,787.2 ns |  1,092.10 ns |    484.90 ns |  22,572.7 ns |  1.46 |    0.03 |    3 |         - |          NA |
| PigeonSortInteger   | 4096 | Random             |  11,305.0 ns |    302.98 ns |    134.52 ns |  11,378.8 ns |  0.72 |    0.01 |    1 |         - |          NA |
| BucketSort          | 4096 | Random             |  33,532.2 ns |    584.92 ns |    305.92 ns |  33,597.9 ns |  2.14 |    0.02 |    4 |         - |          NA |
| BucketSortInteger   | 4096 | Random             |  24,083.4 ns |    451.58 ns |    200.50 ns |  24,105.8 ns |  1.54 |    0.01 |    3 |         - |          NA |
| FlashSort           | 4096 | Random             |  77,286.8 ns |    736.95 ns |    327.21 ns |  77,253.7 ns |  4.94 |    0.02 |    5 |         - |          NA |
| RadixLSD4Sort       | 4096 | Random             |  66,478.2 ns |    718.64 ns |    375.86 ns |  66,409.5 ns |  4.25 |    0.02 |    5 |         - |          NA |
| RadixLSD256Sort     | 4096 | Random             |  25,743.2 ns |    238.71 ns |    105.99 ns |  25,739.3 ns |  1.65 |    0.01 |    3 |         - |          NA |
| RadixLSD10Sort      | 4096 | Random             |  85,354.1 ns |    597.43 ns |    265.26 ns |  85,383.1 ns |  5.46 |    0.02 |    5 |         - |          NA |
| RadixMSD4Sort       | 4096 | Random             |  71,073.4 ns |  1,421.33 ns |    743.38 ns |  71,295.9 ns |  4.54 |    0.05 |    5 |         - |          NA |
| RadixMSD10Sort      | 4096 | Random             |  87,222.0 ns |  1,493.90 ns |    781.34 ns |  86,931.0 ns |  5.57 |    0.05 |    5 |         - |          NA |
| AmericanFlagSort    | 4096 | Random             |  72,802.8 ns |    522.09 ns |    273.06 ns |  72,821.4 ns |  4.65 |    0.02 |    5 |         - |          NA |
| SpreadSort          | 4096 | Random             |  38,646.9 ns |    397.54 ns |    141.77 ns |  38,680.0 ns |  2.47 |    0.01 |    4 |         - |          NA |
|      |                    |              |              |              |              |       |         |      |           |             |
| **CountingSort**        | **4096** | **SingleElementMoved** |  **23,930.7 ns** |    **656.74 ns** |    **343.49 ns** |  **23,781.8 ns** |  **1.44** |    **0.02** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 4096 | SingleElementMoved |  16,635.3 ns |     58.43 ns |     20.84 ns |  16,635.7 ns |  1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | SingleElementMoved |  19,528.0 ns |    183.57 ns |     96.01 ns |  19,478.0 ns |  1.17 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 4096 | SingleElementMoved |  11,462.0 ns |    644.61 ns |    286.21 ns |  11,489.3 ns |  0.69 |    0.02 |    1 |         - |          NA |
| BucketSort          | 4096 | SingleElementMoved |  30,424.1 ns |    900.73 ns |    399.93 ns |  30,417.4 ns |  1.83 |    0.02 |    2 |         - |          NA |
| BucketSortInteger   | 4096 | SingleElementMoved |  27,839.8 ns |    689.79 ns |    306.27 ns |  27,688.5 ns |  1.67 |    0.02 |    2 |         - |          NA |
| FlashSort           | 4096 | SingleElementMoved | 106,561.4 ns |  8,023.03 ns |  4,196.20 ns | 108,708.4 ns |  6.41 |    0.24 |    5 |         - |          NA |
| RadixLSD4Sort       | 4096 | SingleElementMoved |  96,430.8 ns |    839.46 ns |    439.05 ns |  96,285.2 ns |  5.80 |    0.03 |    5 |         - |          NA |
| RadixLSD256Sort     | 4096 | SingleElementMoved |  23,456.0 ns |    572.45 ns |    299.40 ns |  23,452.8 ns |  1.41 |    0.02 |    2 |         - |          NA |
| RadixLSD10Sort      | 4096 | SingleElementMoved |  84,994.5 ns |    519.14 ns |    230.50 ns |  84,934.1 ns |  5.11 |    0.01 |    5 |         - |          NA |
| RadixMSD4Sort       | 4096 | SingleElementMoved |  59,442.5 ns |    621.78 ns |    325.20 ns |  59,488.0 ns |  3.57 |    0.02 |    4 |         - |          NA |
| RadixMSD10Sort      | 4096 | SingleElementMoved |  79,816.1 ns |  1,384.82 ns |    614.87 ns |  79,856.6 ns |  4.80 |    0.04 |    5 |         - |          NA |
| AmericanFlagSort    | 4096 | SingleElementMoved |  52,588.8 ns | 12,428.55 ns |  6,500.37 ns |  48,936.1 ns |  3.16 |    0.37 |    3 |         - |          NA |
| SpreadSort          | 4096 | SingleElementMoved |  27,015.4 ns |    432.97 ns |    192.24 ns |  27,021.9 ns |  1.62 |    0.01 |    2 |         - |          NA |
|      |                    |              |              |              |              |       |         |      |           |             |
| **CountingSort**        | **4096** | **Sorted**             |  **23,322.4 ns** |    **489.08 ns** |    **255.80 ns** |  **23,213.6 ns** |  **1.71** |    **0.02** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Sorted             |  13,669.3 ns |     87.63 ns |     31.25 ns |  13,676.3 ns |  1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 4096 | Sorted             |  21,789.4 ns |    657.58 ns |    343.93 ns |  21,639.3 ns |  1.59 |    0.02 |    4 |         - |          NA |
| PigeonSortInteger   | 4096 | Sorted             |   9,814.0 ns |    303.01 ns |    158.48 ns |   9,816.0 ns |  0.72 |    0.01 |    2 |         - |          NA |
| BucketSort          | 4096 | Sorted             |  36,795.5 ns |  1,103.97 ns |    490.17 ns |  36,927.9 ns |  2.69 |    0.03 |    5 |         - |          NA |
| BucketSortInteger   | 4096 | Sorted             |  22,103.6 ns |    675.89 ns |    300.10 ns |  21,963.1 ns |  1.62 |    0.02 |    4 |         - |          NA |
| FlashSort           | 4096 | Sorted             |  86,262.2 ns |    677.82 ns |    354.51 ns |  86,148.8 ns |  6.31 |    0.03 |    7 |         - |          NA |
| RadixLSD4Sort       | 4096 | Sorted             |  81,632.3 ns |  1,899.48 ns |    993.47 ns |  81,567.2 ns |  5.97 |    0.07 |    7 |         - |          NA |
| RadixLSD256Sort     | 4096 | Sorted             |  23,750.5 ns |    958.75 ns |    425.69 ns |  23,629.9 ns |  1.74 |    0.03 |    4 |         - |          NA |
| RadixLSD10Sort      | 4096 | Sorted             |  83,676.1 ns |  1,270.36 ns |    664.42 ns |  83,632.5 ns |  6.12 |    0.05 |    7 |         - |          NA |
| RadixMSD4Sort       | 4096 | Sorted             |  60,040.9 ns |  1,131.55 ns |    502.41 ns |  59,901.4 ns |  4.39 |    0.04 |    6 |         - |          NA |
| RadixMSD10Sort      | 4096 | Sorted             |  78,814.0 ns |    685.48 ns |    304.36 ns |  78,832.4 ns |  5.77 |    0.02 |    7 |         - |          NA |
| AmericanFlagSort    | 4096 | Sorted             |  35,012.8 ns |    877.96 ns |    459.19 ns |  35,075.0 ns |  2.56 |    0.03 |    5 |         - |          NA |
| SpreadSort          | 4096 | Sorted             |   2,354.6 ns |    184.82 ns |     65.91 ns |   2,389.7 ns |  0.17 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |              |              |       |         |      |           |             |
| **CountingSort**        | **4096** | **Reversed**           |  **22,742.8 ns** |    **837.20 ns** |    **437.87 ns** |  **22,500.3 ns** |  **1.47** |    **0.03** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Reversed           |  15,447.5 ns |    149.89 ns |     78.39 ns |  15,417.6 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PigeonSort          | 4096 | Reversed           |  19,636.1 ns |    473.31 ns |    247.55 ns |  19,565.3 ns |  1.27 |    0.02 |    3 |         - |          NA |
| PigeonSortInteger   | 4096 | Reversed           |  10,649.4 ns |    687.52 ns |    359.59 ns |  10,549.3 ns |  0.69 |    0.02 |    1 |         - |          NA |
| BucketSort          | 4096 | Reversed           |  30,553.3 ns |  1,058.09 ns |    469.80 ns |  30,498.1 ns |  1.98 |    0.03 |    4 |         - |          NA |
| BucketSortInteger   | 4096 | Reversed           |  23,643.5 ns |    705.84 ns |    369.17 ns |  23,543.4 ns |  1.53 |    0.02 |    3 |         - |          NA |
| FlashSort           | 4096 | Reversed           |  76,063.1 ns |    839.25 ns |    438.94 ns |  76,043.4 ns |  4.92 |    0.04 |    6 |         - |          NA |
| RadixLSD4Sort       | 4096 | Reversed           |  89,358.4 ns |  1,716.76 ns |    897.90 ns |  89,056.2 ns |  5.78 |    0.06 |    6 |         - |          NA |
| RadixLSD256Sort     | 4096 | Reversed           |  22,422.4 ns |    454.45 ns |    237.69 ns |  22,446.7 ns |  1.45 |    0.02 |    3 |         - |          NA |
| RadixLSD10Sort      | 4096 | Reversed           |  84,714.6 ns |    947.77 ns |    495.70 ns |  84,678.7 ns |  5.48 |    0.04 |    6 |         - |          NA |
| RadixMSD4Sort       | 4096 | Reversed           |  75,817.6 ns |  1,134.96 ns |    503.93 ns |  75,791.9 ns |  4.91 |    0.04 |    6 |         - |          NA |
| RadixMSD10Sort      | 4096 | Reversed           |  88,236.9 ns |    445.62 ns |    233.07 ns |  88,324.6 ns |  5.71 |    0.03 |    6 |         - |          NA |
| AmericanFlagSort    | 4096 | Reversed           |  44,972.7 ns |    812.24 ns |    360.64 ns |  45,009.6 ns |  2.91 |    0.03 |    5 |         - |          NA |
| SpreadSort          | 4096 | Reversed           |  19,889.1 ns |    229.23 ns |    101.78 ns |  19,879.2 ns |  1.29 |    0.01 |    3 |         - |          NA |
|      |                    |              |              |              |              |       |         |      |           |             |
| **CountingSort**        | **4096** | **PipeOrgan**          |  **23,169.3 ns** |    **223.53 ns** |     **79.71 ns** |  **23,202.0 ns** |  **1.32** |    **0.04** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 4096 | PipeOrgan          |  17,612.6 ns |  1,100.60 ns |    575.63 ns |  17,390.4 ns |  1.00 |    0.04 |    2 |         - |          NA |
| PigeonSort          | 4096 | PipeOrgan          |  20,075.4 ns |  1,006.34 ns |    526.33 ns |  19,887.2 ns |  1.14 |    0.04 |    2 |         - |          NA |
| PigeonSortInteger   | 4096 | PipeOrgan          |  11,365.7 ns |    497.09 ns |    220.71 ns |  11,362.5 ns |  0.65 |    0.02 |    1 |         - |          NA |
| BucketSort          | 4096 | PipeOrgan          |  33,201.3 ns |    944.43 ns |    419.33 ns |  33,232.2 ns |  1.89 |    0.06 |    3 |         - |          NA |
| BucketSortInteger   | 4096 | PipeOrgan          |  24,810.1 ns |    373.25 ns |    165.72 ns |  24,827.1 ns |  1.41 |    0.04 |    2 |         - |          NA |
| FlashSort           | 4096 | PipeOrgan          |  75,548.0 ns |    648.32 ns |    339.08 ns |  75,602.4 ns |  4.29 |    0.13 |    4 |         - |          NA |
| RadixLSD4Sort       | 4096 | PipeOrgan          |  71,979.8 ns |    727.78 ns |    380.64 ns |  72,005.5 ns |  4.09 |    0.13 |    4 |         - |          NA |
| RadixLSD256Sort     | 4096 | PipeOrgan          |  23,533.9 ns |    917.96 ns |    407.58 ns |  23,435.4 ns |  1.34 |    0.05 |    2 |         - |          NA |
| RadixLSD10Sort      | 4096 | PipeOrgan          |  85,006.7 ns |  1,583.68 ns |    828.30 ns |  85,149.0 ns |  4.83 |    0.15 |    4 |         - |          NA |
| RadixMSD4Sort       | 4096 | PipeOrgan          |  75,205.5 ns |  1,485.73 ns |    777.06 ns |  75,165.9 ns |  4.27 |    0.14 |    4 |         - |          NA |
| RadixMSD10Sort      | 4096 | PipeOrgan          |  85,081.7 ns |    245.68 ns |     87.61 ns |  85,116.8 ns |  4.84 |    0.15 |    4 |         - |          NA |
| AmericanFlagSort    | 4096 | PipeOrgan          |  61,327.0 ns |    831.80 ns |    435.05 ns |  61,310.4 ns |  3.49 |    0.11 |    4 |         - |          NA |
| SpreadSort          | 4096 | PipeOrgan          |  31,815.3 ns |    204.30 ns |     90.71 ns |  31,786.9 ns |  1.81 |    0.06 |    3 |         - |          NA |
|      |                    |              |              |              |              |       |         |      |           |             |
| **CountingSort**        | **4096** | **ManyDuplicates**     |  **22,499.6 ns** |    **330.79 ns** |    **146.87 ns** |  **22,446.5 ns** |  **1.71** |    **0.02** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 4096 | ManyDuplicates     |  13,143.9 ns |    264.59 ns |    117.48 ns |  13,180.9 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PigeonSort          | 4096 | ManyDuplicates     |  26,909.2 ns |    836.19 ns |    437.35 ns |  26,966.8 ns |  2.05 |    0.04 |    4 |         - |          NA |
| PigeonSortInteger   | 4096 | ManyDuplicates     |   9,933.4 ns |    235.15 ns |    122.99 ns |   9,985.8 ns |  0.76 |    0.01 |    1 |         - |          NA |
| BucketSort          | 4096 | ManyDuplicates     |  48,615.3 ns |    274.30 ns |    143.47 ns |  48,644.7 ns |  3.70 |    0.03 |    4 |         - |          NA |
| BucketSortInteger   | 4096 | ManyDuplicates     |  26,874.9 ns |    367.78 ns |    163.30 ns |  26,920.6 ns |  2.04 |    0.02 |    4 |         - |          NA |
| FlashSort           | 4096 | ManyDuplicates     |  72,775.6 ns |  1,027.16 ns |    456.07 ns |  72,877.8 ns |  5.54 |    0.06 |    5 |         - |          NA |
| RadixLSD4Sort       | 4096 | ManyDuplicates     |  36,001.7 ns |    848.14 ns |    376.58 ns |  35,880.2 ns |  2.74 |    0.04 |    4 |         - |          NA |
| RadixLSD256Sort     | 4096 | ManyDuplicates     |  16,618.9 ns |    883.92 ns |    462.31 ns |  16,554.3 ns |  1.26 |    0.03 |    3 |         - |          NA |
| RadixLSD10Sort      | 4096 | ManyDuplicates     |  45,683.7 ns |    633.21 ns |    281.15 ns |  45,602.9 ns |  3.48 |    0.04 |    4 |         - |          NA |
| RadixMSD4Sort       | 4096 | ManyDuplicates     |  40,859.2 ns |    605.54 ns |    268.86 ns |  40,883.1 ns |  3.11 |    0.03 |    4 |         - |          NA |
| RadixMSD10Sort      | 4096 | ManyDuplicates     |  50,167.2 ns |    860.42 ns |    450.02 ns |  50,038.2 ns |  3.82 |    0.05 |    4 |         - |          NA |
| AmericanFlagSort    | 4096 | ManyDuplicates     |  31,381.6 ns |    755.29 ns |    395.03 ns |  31,142.5 ns |  2.39 |    0.03 |    4 |         - |          NA |
| SpreadSort          | 4096 | ManyDuplicates     |  26,758.6 ns |    387.55 ns |    202.70 ns |  26,803.3 ns |  2.04 |    0.02 |    4 |         - |          NA |
|      |                    |              |              |              |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **Random**             |  **51,741.0 ns** |  **1,825.66 ns** |    **954.85 ns** |  **51,791.9 ns** |  **1.53** |    **0.05** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Random             |  33,937.5 ns |  1,655.61 ns |    865.91 ns |  33,815.1 ns |  1.00 |    0.03 |    2 |         - |          NA |
| PigeonSort          | 8192 | Random             |  45,310.1 ns |    438.31 ns |    194.61 ns |  45,293.6 ns |  1.34 |    0.03 |    3 |         - |          NA |
| PigeonSortInteger   | 8192 | Random             |  24,148.0 ns |  2,602.07 ns |  1,360.93 ns |  23,599.9 ns |  0.71 |    0.04 |    1 |         - |          NA |
| BucketSort          | 8192 | Random             |  68,463.1 ns |    546.13 ns |    285.64 ns |  68,552.1 ns |  2.02 |    0.05 |    4 |         - |          NA |
| BucketSortInteger   | 8192 | Random             |  49,811.0 ns |    903.77 ns |    472.69 ns |  49,683.5 ns |  1.47 |    0.04 |    3 |         - |          NA |
| FlashSort           | 8192 | Random             | 167,966.0 ns |    970.14 ns |    507.40 ns | 168,036.4 ns |  4.95 |    0.12 |    6 |         - |          NA |
| RadixLSD4Sort       | 8192 | Random             | 152,263.6 ns |    859.39 ns |    449.48 ns | 152,234.2 ns |  4.49 |    0.11 |    6 |         - |          NA |
| RadixLSD256Sort     | 8192 | Random             |  51,990.4 ns |    646.66 ns |    338.22 ns |  52,100.7 ns |  1.53 |    0.04 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | Random             | 171,166.3 ns |  2,657.02 ns |  1,389.67 ns | 170,634.0 ns |  5.05 |    0.13 |    6 |         - |          NA |
| RadixMSD4Sort       | 8192 | Random             | 166,482.1 ns |  1,716.03 ns |    897.52 ns | 166,190.0 ns |  4.91 |    0.12 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | Random             | 174,799.1 ns |  1,073.97 ns |    561.70 ns | 174,516.9 ns |  5.15 |    0.12 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | Random             | 144,389.9 ns |    617.57 ns |    274.20 ns | 144,370.5 ns |  4.26 |    0.10 |    6 |         - |          NA |
| SpreadSort          | 8192 | Random             |  97,645.0 ns |  1,270.39 ns |    564.06 ns |  97,682.4 ns |  2.88 |    0.07 |    5 |         - |          NA |
|      |                    |              |              |              |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **SingleElementMoved** |  **48,154.6 ns** |    **790.68 ns** |    **351.07 ns** |  **48,109.6 ns** |  **1.41** |    **0.03** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | SingleElementMoved |  34,106.5 ns |  1,331.38 ns |    696.34 ns |  33,994.3 ns |  1.00 |    0.03 |    2 |         - |          NA |
| PigeonSort          | 8192 | SingleElementMoved |  39,232.2 ns |  1,142.05 ns |    597.31 ns |  39,290.2 ns |  1.15 |    0.03 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | SingleElementMoved |  23,517.8 ns |  1,356.44 ns |    709.44 ns |  23,440.6 ns |  0.69 |    0.02 |    1 |         - |          NA |
| BucketSort          | 8192 | SingleElementMoved |  60,619.4 ns |  1,445.75 ns |    756.16 ns |  60,581.6 ns |  1.78 |    0.04 |    3 |         - |          NA |
| BucketSortInteger   | 8192 | SingleElementMoved |  52,617.2 ns |    688.80 ns |    305.83 ns |  52,591.0 ns |  1.54 |    0.03 |    3 |         - |          NA |
| FlashSort           | 8192 | SingleElementMoved | 222,323.7 ns |  4,560.86 ns |  2,385.42 ns | 222,882.0 ns |  6.52 |    0.14 |    6 |         - |          NA |
| RadixLSD4Sort       | 8192 | SingleElementMoved | 201,041.6 ns |  4,710.47 ns |  2,463.67 ns | 201,472.0 ns |  5.90 |    0.13 |    6 |         - |          NA |
| RadixLSD256Sort     | 8192 | SingleElementMoved |  48,839.1 ns |    576.37 ns |    255.91 ns |  48,800.5 ns |  1.43 |    0.03 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | SingleElementMoved | 167,129.7 ns |  3,236.89 ns |  1,692.96 ns | 167,775.1 ns |  4.90 |    0.10 |    5 |         - |          NA |
| RadixMSD4Sort       | 8192 | SingleElementMoved | 138,388.3 ns |    852.37 ns |    445.81 ns | 138,399.3 ns |  4.06 |    0.08 |    5 |         - |          NA |
| RadixMSD10Sort      | 8192 | SingleElementMoved | 159,530.8 ns |    761.75 ns |    398.41 ns | 159,504.2 ns |  4.68 |    0.09 |    5 |         - |          NA |
| AmericanFlagSort    | 8192 | SingleElementMoved |  94,262.7 ns |    134.83 ns |     48.08 ns |  94,259.8 ns |  2.76 |    0.05 |    4 |         - |          NA |
| SpreadSort          | 8192 | SingleElementMoved |  56,753.2 ns |    666.00 ns |    237.50 ns |  56,842.4 ns |  1.66 |    0.03 |    3 |         - |          NA |
|      |                    |              |              |              |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **Sorted**             |  **45,704.9 ns** |    **944.07 ns** |    **419.17 ns** |  **45,856.3 ns** |  **1.64** |    **0.02** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Sorted             |  27,949.6 ns |    574.57 ns |    300.51 ns |  27,859.2 ns |  1.00 |    0.01 |    3 |         - |          NA |
| PigeonSort          | 8192 | Sorted             |  42,640.1 ns |  2,107.64 ns |  1,102.33 ns |  42,104.2 ns |  1.53 |    0.04 |    4 |         - |          NA |
| PigeonSortInteger   | 8192 | Sorted             |  19,902.3 ns |  1,037.66 ns |    460.73 ns |  19,782.2 ns |  0.71 |    0.02 |    2 |         - |          NA |
| BucketSort          | 8192 | Sorted             |  63,872.9 ns |  2,056.18 ns |  1,075.42 ns |  63,730.7 ns |  2.29 |    0.04 |    5 |         - |          NA |
| BucketSortInteger   | 8192 | Sorted             |  45,334.3 ns |  3,772.58 ns |  1,675.05 ns |  45,043.2 ns |  1.62 |    0.06 |    4 |         - |          NA |
| FlashSort           | 8192 | Sorted             | 171,048.7 ns |  1,412.38 ns |    738.70 ns | 170,821.4 ns |  6.12 |    0.07 |    6 |         - |          NA |
| RadixLSD4Sort       | 8192 | Sorted             | 207,198.9 ns |  2,199.56 ns |  1,150.41 ns | 206,888.0 ns |  7.41 |    0.08 |    7 |         - |          NA |
| RadixLSD256Sort     | 8192 | Sorted             |  49,058.6 ns |    900.36 ns |    470.90 ns |  49,027.5 ns |  1.76 |    0.02 |    4 |         - |          NA |
| RadixLSD10Sort      | 8192 | Sorted             | 168,206.6 ns |  2,857.02 ns |  1,494.28 ns | 168,019.4 ns |  6.02 |    0.08 |    6 |         - |          NA |
| RadixMSD4Sort       | 8192 | Sorted             | 137,590.6 ns |    698.36 ns |    365.26 ns | 137,677.3 ns |  4.92 |    0.05 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | Sorted             | 159,037.9 ns |  1,075.20 ns |    477.40 ns | 159,006.4 ns |  5.69 |    0.06 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | Sorted             |  92,020.5 ns | 58,087.39 ns | 30,380.83 ns |  70,967.6 ns |  3.29 |    1.03 |    5 |         - |          NA |
| SpreadSort          | 8192 | Sorted             |   4,588.3 ns |    365.90 ns |    191.37 ns |   4,452.9 ns |  0.16 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |              |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **Reversed**           |  **45,399.1 ns** |    **803.86 ns** |    **420.43 ns** |  **45,287.5 ns** |  **1.48** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Reversed           |  30,716.5 ns |    505.80 ns |    224.58 ns |  30,651.0 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PigeonSort          | 8192 | Reversed           |  39,851.0 ns |  2,952.81 ns |  1,544.38 ns |  39,124.6 ns |  1.30 |    0.05 |    3 |         - |          NA |
| PigeonSortInteger   | 8192 | Reversed           |  20,375.4 ns |  1,459.88 ns |    763.54 ns |  20,252.4 ns |  0.66 |    0.02 |    1 |         - |          NA |
| BucketSort          | 8192 | Reversed           |  62,205.8 ns |  1,635.98 ns |    855.65 ns |  62,252.2 ns |  2.03 |    0.03 |    4 |         - |          NA |
| BucketSortInteger   | 8192 | Reversed           |  47,598.8 ns |  1,065.01 ns |    557.02 ns |  47,544.8 ns |  1.55 |    0.02 |    3 |         - |          NA |
| FlashSort           | 8192 | Reversed           | 153,523.0 ns |  1,039.78 ns |    543.83 ns | 153,578.5 ns |  5.00 |    0.04 |    6 |         - |          NA |
| RadixLSD4Sort       | 8192 | Reversed           | 195,321.4 ns |  2,340.79 ns |  1,224.28 ns | 195,517.3 ns |  6.36 |    0.06 |    6 |         - |          NA |
| RadixLSD256Sort     | 8192 | Reversed           |  46,944.4 ns |  1,394.02 ns |    729.10 ns |  46,666.1 ns |  1.53 |    0.02 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | Reversed           | 166,946.3 ns |  2,411.08 ns |  1,261.04 ns | 167,091.1 ns |  5.44 |    0.05 |    6 |         - |          NA |
| RadixMSD4Sort       | 8192 | Reversed           | 167,533.3 ns |  1,324.21 ns |    587.96 ns | 167,415.5 ns |  5.45 |    0.04 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | Reversed           | 174,958.1 ns |    532.53 ns |    236.45 ns | 174,929.6 ns |  5.70 |    0.04 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | Reversed           |  90,024.0 ns |    782.36 ns |    409.19 ns |  90,123.3 ns |  2.93 |    0.02 |    5 |         - |          NA |
| SpreadSort          | 8192 | Reversed           |  75,683.5 ns |  2,220.49 ns |  1,161.36 ns |  76,041.5 ns |  2.46 |    0.04 |    5 |         - |          NA |
|      |                    |              |              |              |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **PipeOrgan**          |  **47,227.8 ns** |    **346.33 ns** |    **153.77 ns** |  **47,200.6 ns** |  **1.40** |    **0.00** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | PipeOrgan          |  33,844.7 ns |    147.47 ns |     52.59 ns |  33,849.6 ns |  1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | PipeOrgan          |  39,327.7 ns |  1,446.37 ns |    756.48 ns |  39,040.5 ns |  1.16 |    0.02 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | PipeOrgan          |  23,509.2 ns |  1,411.90 ns |    738.45 ns |  23,471.5 ns |  0.69 |    0.02 |    1 |         - |          NA |
| BucketSort          | 8192 | PipeOrgan          |  61,886.1 ns |  1,679.16 ns |    878.23 ns |  61,944.1 ns |  1.83 |    0.02 |    4 |         - |          NA |
| BucketSortInteger   | 8192 | PipeOrgan          |  50,315.5 ns |    882.90 ns |    392.01 ns |  50,441.0 ns |  1.49 |    0.01 |    3 |         - |          NA |
| FlashSort           | 8192 | PipeOrgan          | 207,242.2 ns | 17,761.74 ns |  9,289.73 ns | 211,476.7 ns |  6.12 |    0.26 |    7 |         - |          NA |
| RadixLSD4Sort       | 8192 | PipeOrgan          | 177,859.0 ns |  2,448.08 ns |  1,086.96 ns | 177,659.8 ns |  5.26 |    0.03 |    7 |         - |          NA |
| RadixLSD256Sort     | 8192 | PipeOrgan          |  48,206.8 ns |  1,182.30 ns |    618.37 ns |  47,875.1 ns |  1.42 |    0.02 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | PipeOrgan          | 169,362.5 ns |  2,738.23 ns |  1,215.79 ns | 169,975.0 ns |  5.00 |    0.03 |    7 |         - |          NA |
| RadixMSD4Sort       | 8192 | PipeOrgan          | 170,435.0 ns |  1,510.60 ns |    790.07 ns | 170,433.1 ns |  5.04 |    0.02 |    7 |         - |          NA |
| RadixMSD10Sort      | 8192 | PipeOrgan          | 173,560.9 ns |    750.28 ns |    333.13 ns | 173,574.5 ns |  5.13 |    0.01 |    7 |         - |          NA |
| AmericanFlagSort    | 8192 | PipeOrgan          | 122,106.7 ns |    590.33 ns |    262.11 ns | 122,156.1 ns |  3.61 |    0.01 |    6 |         - |          NA |
| SpreadSort          | 8192 | PipeOrgan          |  96,391.4 ns |    675.60 ns |    353.35 ns |  96,445.1 ns |  2.85 |    0.01 |    5 |         - |          NA |
|      |                    |              |              |              |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **ManyDuplicates**     |  **43,749.9 ns** |    **944.13 ns** |    **419.20 ns** |  **43,813.2 ns** |  **1.63** |    **0.03** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 8192 | ManyDuplicates     |  26,804.8 ns |    989.63 ns |    517.60 ns |  26,598.9 ns |  1.00 |    0.03 |    2 |         - |          NA |
| PigeonSort          | 8192 | ManyDuplicates     |  76,289.5 ns |  1,309.62 ns |    684.96 ns |  76,359.5 ns |  2.85 |    0.06 |    5 |         - |          NA |
| PigeonSortInteger   | 8192 | ManyDuplicates     |  20,108.1 ns |    362.56 ns |    160.98 ns |  20,090.1 ns |  0.75 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | ManyDuplicates     |  97,433.2 ns |  1,174.63 ns |    614.35 ns |  97,501.9 ns |  3.64 |    0.07 |    5 |         - |          NA |
| BucketSortInteger   | 8192 | ManyDuplicates     |  54,374.5 ns |    584.79 ns |    259.65 ns |  54,261.9 ns |  2.03 |    0.04 |    5 |         - |          NA |
| FlashSort           | 8192 | ManyDuplicates     | 147,276.8 ns |    803.51 ns |    356.76 ns | 147,252.3 ns |  5.50 |    0.10 |    6 |         - |          NA |
| RadixLSD4Sort       | 8192 | ManyDuplicates     |  73,335.0 ns |  1,069.33 ns |    474.79 ns |  73,244.2 ns |  2.74 |    0.05 |    5 |         - |          NA |
| RadixLSD256Sort     | 8192 | ManyDuplicates     |  32,503.0 ns |    461.92 ns |    205.10 ns |  32,515.7 ns |  1.21 |    0.02 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | ManyDuplicates     |  91,304.1 ns |  1,454.85 ns |    760.92 ns |  91,268.8 ns |  3.41 |    0.07 |    5 |         - |          NA |
| RadixMSD4Sort       | 8192 | ManyDuplicates     |  80,210.2 ns |    724.73 ns |    379.05 ns |  80,232.4 ns |  2.99 |    0.06 |    5 |         - |          NA |
| RadixMSD10Sort      | 8192 | ManyDuplicates     |  98,665.4 ns |    261.20 ns |    115.97 ns |  98,676.9 ns |  3.68 |    0.07 |    5 |         - |          NA |
| AmericanFlagSort    | 8192 | ManyDuplicates     |  61,490.1 ns |    596.28 ns |    311.87 ns |  61,480.3 ns |  2.29 |    0.04 |    5 |         - |          NA |
| SpreadSort          | 8192 | ManyDuplicates     |  53,343.6 ns |    865.14 ns |    452.48 ns |  53,343.5 ns |  1.99 |    0.04 |    5 |         - |          NA |

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
| **BubbleSort**         | **256**  | **Random**             |  **28,163.3 ns** |   **328.62 ns** |   **171.88 ns** |   **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Random             |  16,683.3 ns |   182.69 ns |    95.55 ns |   0.59 |    0.00 |    2 |         - |          NA |
| OddEvenSort        | 256  | Random             |  21,911.0 ns |   290.02 ns |   128.77 ns |   0.78 |    0.01 |    2 |         - |          NA |
| CombSort           | 256  | Random             |   3,590.1 ns |   188.44 ns |    83.67 ns |   0.13 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | Random             |  18,462.7 ns |   379.84 ns |   198.67 ns |   0.66 |    0.01 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **SingleElementMoved** |     **484.1 ns** |   **107.79 ns** |    **56.38 ns** |   **1.01** |    **0.16** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 256  | SingleElementMoved |     383.0 ns |   151.70 ns |    67.35 ns |   0.80 |    0.16 |    1 |         - |          NA |
| OddEvenSort        | 256  | SingleElementMoved |  17,488.5 ns |   234.55 ns |   122.68 ns |  36.55 |    3.90 |    4 |         - |          NA |
| CombSort           | 256  | SingleElementMoved |   2,851.9 ns |    15.03 ns |     6.67 ns |   5.96 |    0.64 |    3 |         - |          NA |
| CircleSort         | 256  | SingleElementMoved |  15,391.3 ns |   125.94 ns |    65.87 ns |  32.16 |    3.43 |    4 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **Sorted**             |     **188.5 ns** |     **1.84 ns** |     **0.96 ns** |   **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Sorted             |     261.9 ns |   133.41 ns |    69.77 ns |   1.39 |    0.35 |    2 |         - |          NA |
| OddEvenSort        | 256  | Sorted             |     274.4 ns |   113.66 ns |    59.45 ns |   1.46 |    0.30 |    2 |         - |          NA |
| CombSort           | 256  | Sorted             |   2,910.4 ns |   364.75 ns |   190.77 ns |  15.44 |    0.96 |    4 |         - |          NA |
| CircleSort         | 256  | Sorted             |   2,336.1 ns |   284.67 ns |   148.89 ns |  12.39 |    0.75 |    3 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **Reversed**           |  **29,384.3 ns** |   **155.75 ns** |    **69.16 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Reversed           |  27,914.2 ns |   236.94 ns |   123.93 ns |   0.95 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 256  | Reversed           |  25,954.2 ns |   994.20 ns |   519.98 ns |   0.88 |    0.02 |    3 |         - |          NA |
| CombSort           | 256  | Reversed           |   3,004.4 ns |   312.28 ns |   163.33 ns |   0.10 |    0.01 |    1 |         - |          NA |
| CircleSort         | 256  | Reversed           |   4,475.7 ns |   288.97 ns |   128.31 ns |   0.15 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **PipeOrgan**          |  **34,655.0 ns** |   **671.23 ns** |   **351.07 ns** |   **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 256  | PipeOrgan          |  17,984.7 ns |   155.39 ns |    68.99 ns |   0.52 |    0.01 |    2 |         - |          NA |
| OddEvenSort        | 256  | PipeOrgan          |  28,096.2 ns |   673.30 ns |   298.95 ns |   0.81 |    0.01 |    3 |         - |          NA |
| CombSort           | 256  | PipeOrgan          |   3,011.0 ns |   198.64 ns |    88.20 ns |   0.09 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | PipeOrgan          |  20,426.0 ns |   208.38 ns |   108.99 ns |   0.59 |    0.01 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **ManyDuplicates**     |  **28,924.0 ns** |   **216.96 ns** |   **113.47 ns** |   **1.00** |    **0.01** |    **5** |         **-** |          **NA** |
| CocktailShakerSort | 256  | ManyDuplicates     |  17,094.4 ns |   227.41 ns |   118.94 ns |   0.59 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 256  | ManyDuplicates     |  21,253.4 ns |   206.11 ns |    91.52 ns |   0.73 |    0.00 |    4 |         - |          NA |
| CombSort           | 256  | ManyDuplicates     |   3,264.7 ns |    18.38 ns |     8.16 ns |   0.11 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | ManyDuplicates     |  13,835.5 ns |   286.97 ns |   150.09 ns |   0.48 |    0.01 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Random**             | **535,812.4 ns** | **8,438.55 ns** | **3,746.77 ns** |   **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Random             | 324,202.3 ns |   465.80 ns |   166.11 ns |   0.61 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | Random             | 472,430.7 ns | 1,716.21 ns |   897.61 ns |   0.88 |    0.01 |    4 |         - |          NA |
| CombSort           | 1024 | Random             |  19,531.2 ns |   280.03 ns |   124.34 ns |   0.04 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | Random             | 101,243.2 ns | 2,367.93 ns | 1,238.48 ns |   0.19 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **SingleElementMoved** |   **1,699.0 ns** |     **5.21 ns** |     **1.86 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | SingleElementMoved |   1,293.3 ns |     4.46 ns |     1.98 ns |   0.76 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 1024 | SingleElementMoved | 251,474.7 ns |   424.17 ns |   188.33 ns | 148.02 |    0.18 |    5 |         - |          NA |
| CombSort           | 1024 | SingleElementMoved |  15,373.6 ns |   117.28 ns |    52.07 ns |   9.05 |    0.03 |    3 |         - |          NA |
| CircleSort         | 1024 | SingleElementMoved |  86,395.9 ns |   513.02 ns |   227.78 ns |  50.85 |    0.14 |    4 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Sorted**             |     **723.6 ns** |     **5.40 ns** |     **1.92 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Sorted             |     791.3 ns |   100.12 ns |    52.37 ns |   1.09 |    0.07 |    1 |         - |          NA |
| OddEvenSort        | 1024 | Sorted             |     794.7 ns |   155.91 ns |    69.22 ns |   1.10 |    0.09 |    1 |         - |          NA |
| CombSort           | 1024 | Sorted             |  14,673.0 ns |   133.68 ns |    69.92 ns |  20.28 |    0.10 |    3 |         - |          NA |
| CircleSort         | 1024 | Sorted             |   9,852.5 ns |   392.05 ns |   205.05 ns |  13.62 |    0.27 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Reversed**           | **438,681.5 ns** | **1,995.47 ns** |   **886.00 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Reversed           | 437,465.9 ns | 2,847.79 ns | 1,489.45 ns |   1.00 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | Reversed           | 409,032.0 ns | 3,607.46 ns | 1,886.77 ns |   0.93 |    0.00 |    3 |         - |          NA |
| CombSort           | 1024 | Reversed           |  15,745.6 ns |   209.38 ns |    92.97 ns |   0.04 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | Reversed           |  19,144.8 ns |   312.52 ns |   138.76 ns |   0.04 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **PipeOrgan**          | **506,863.8 ns** | **2,064.51 ns** |   **916.65 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | PipeOrgan          | 283,955.7 ns | 1,578.17 ns |   825.41 ns |   0.56 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | PipeOrgan          | 449,290.5 ns | 1,029.36 ns |   457.04 ns |   0.89 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | PipeOrgan          |  15,877.2 ns |   204.13 ns |    90.63 ns |   0.03 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | PipeOrgan          | 108,247.1 ns |   760.76 ns |   337.78 ns |   0.21 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **ManyDuplicates**     | **540,169.1 ns** | **2,570.55 ns** | **1,344.45 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | ManyDuplicates     | 319,433.6 ns | 2,238.92 ns | 1,171.00 ns |   0.59 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | ManyDuplicates     | 469,603.7 ns | 3,240.51 ns | 1,694.85 ns |   0.87 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | ManyDuplicates     |  16,785.6 ns |   126.09 ns |    55.99 ns |   0.03 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | ManyDuplicates     |  92,014.3 ns |   839.12 ns |   438.88 ns |   0.17 |    0.00 |    2 |         - |          NA |

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
| **HeapSort**         | **256**  | **Random**             |     **3,503.1 ns** |    **365.05 ns** |    **162.08 ns** |  **1.00** |    **0.06** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | Random             |     3,515.7 ns |    337.40 ns |    149.81 ns |  1.01 |    0.06 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | Random             |     4,173.2 ns |    459.73 ns |    240.45 ns |  1.19 |    0.08 |    1 |         - |          NA |
| BottomupHeapSort | 256  | Random             |     4,405.6 ns |    177.90 ns |     78.99 ns |  1.26 |    0.06 |    1 |         - |          NA |
| WeakHeapSort     | 256  | Random             |    10,295.4 ns |    366.82 ns |    191.85 ns |  2.94 |    0.13 |    4 |         - |          NA |
| SmoothSort       | 256  | Random             |     5,530.3 ns |    328.28 ns |    171.70 ns |  1.58 |    0.08 |    2 |         - |          NA |
| TournamentSort   | 256  | Random             |     8,328.6 ns |    211.26 ns |     93.80 ns |  2.38 |    0.10 |    3 |         - |          NA |
| BinomialHeapSort | 256  | Random             |    15,536.2 ns |    522.39 ns |    273.22 ns |  4.44 |    0.19 |    5 |         - |          NA |
| PairingHeapSort  | 256  | Random             |    10,909.1 ns |    310.67 ns |    162.49 ns |  3.12 |    0.13 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **SingleElementMoved** |     **3,223.4 ns** |    **307.37 ns** |    **160.76 ns** |  **1.00** |    **0.07** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 256  | SingleElementMoved |     3,203.0 ns |    296.14 ns |    154.89 ns |  1.00 |    0.06 |    2 |         - |          NA |
| TernaryHeapSort  | 256  | SingleElementMoved |     4,233.6 ns |    295.67 ns |    131.28 ns |  1.32 |    0.07 |    3 |         - |          NA |
| BottomupHeapSort | 256  | SingleElementMoved |     4,478.2 ns |    481.02 ns |    251.58 ns |  1.39 |    0.10 |    3 |         - |          NA |
| WeakHeapSort     | 256  | SingleElementMoved |     8,435.2 ns |     23.87 ns |     10.60 ns |  2.62 |    0.12 |    5 |         - |          NA |
| SmoothSort       | 256  | SingleElementMoved |     1,746.5 ns |     66.86 ns |     23.84 ns |  0.54 |    0.03 |    1 |         - |          NA |
| TournamentSort   | 256  | SingleElementMoved |     5,643.4 ns |    787.10 ns |    411.67 ns |  1.75 |    0.14 |    4 |         - |          NA |
| BinomialHeapSort | 256  | SingleElementMoved |     7,649.1 ns |     76.31 ns |     39.91 ns |  2.38 |    0.11 |    5 |         - |          NA |
| PairingHeapSort  | 256  | SingleElementMoved |     5,611.2 ns |    418.98 ns |    219.13 ns |  1.74 |    0.10 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **Sorted**             |     **3,204.5 ns** |     **48.57 ns** |     **21.57 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 256  | Sorted             |     3,573.4 ns |    259.82 ns |    135.89 ns |  1.12 |    0.04 |    2 |         - |          NA |
| TernaryHeapSort  | 256  | Sorted             |     4,337.1 ns |    270.30 ns |    141.37 ns |  1.35 |    0.04 |    3 |         - |          NA |
| BottomupHeapSort | 256  | Sorted             |     4,468.0 ns |    295.77 ns |    154.69 ns |  1.39 |    0.05 |    3 |         - |          NA |
| WeakHeapSort     | 256  | Sorted             |     8,718.9 ns |    241.04 ns |    126.07 ns |  2.72 |    0.04 |    6 |         - |          NA |
| SmoothSort       | 256  | Sorted             |     1,429.2 ns |    288.68 ns |    150.99 ns |  0.45 |    0.04 |    1 |         - |          NA |
| TournamentSort   | 256  | Sorted             |     4,540.8 ns |    479.93 ns |    213.09 ns |  1.42 |    0.06 |    3 |         - |          NA |
| BinomialHeapSort | 256  | Sorted             |     6,786.8 ns |    433.01 ns |    226.47 ns |  2.12 |    0.07 |    5 |         - |          NA |
| PairingHeapSort  | 256  | Sorted             |     5,496.7 ns |    269.69 ns |    141.05 ns |  1.72 |    0.04 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **Reversed**           |     **3,283.3 ns** |    **170.24 ns** |     **89.04 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | Reversed           |     2,758.9 ns |    164.58 ns |     73.08 ns |  0.84 |    0.03 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | Reversed           |     4,473.6 ns |    400.43 ns |    209.43 ns |  1.36 |    0.07 |    2 |         - |          NA |
| BottomupHeapSort | 256  | Reversed           |     4,723.1 ns |    394.75 ns |    206.46 ns |  1.44 |    0.07 |    2 |         - |          NA |
| WeakHeapSort     | 256  | Reversed           |     9,698.4 ns |    259.07 ns |    135.50 ns |  2.96 |    0.09 |    4 |         - |          NA |
| SmoothSort       | 256  | Reversed           |     5,037.8 ns |    280.86 ns |    146.89 ns |  1.54 |    0.06 |    2 |         - |          NA |
| TournamentSort   | 256  | Reversed           |     5,123.6 ns |    327.43 ns |    171.25 ns |  1.56 |    0.06 |    2 |         - |          NA |
| BinomialHeapSort | 256  | Reversed           |     6,568.0 ns |     52.90 ns |     18.86 ns |  2.00 |    0.05 |    3 |         - |          NA |
| PairingHeapSort  | 256  | Reversed           |     2,645.9 ns |      7.04 ns |      3.68 ns |  0.81 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **PipeOrgan**          |     **3,196.1 ns** |    **297.29 ns** |    **155.49 ns** |  **1.00** |    **0.06** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | PipeOrgan          |     3,085.9 ns |    214.83 ns |    112.36 ns |  0.97 |    0.06 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | PipeOrgan          |     4,354.9 ns |    333.63 ns |    174.49 ns |  1.37 |    0.08 |    2 |         - |          NA |
| BottomupHeapSort | 256  | PipeOrgan          |     4,489.1 ns |    304.93 ns |    159.48 ns |  1.41 |    0.08 |    2 |         - |          NA |
| WeakHeapSort     | 256  | PipeOrgan          |     9,275.1 ns |    227.02 ns |    118.74 ns |  2.91 |    0.14 |    3 |         - |          NA |
| SmoothSort       | 256  | PipeOrgan          |     5,194.2 ns |    475.57 ns |    248.73 ns |  1.63 |    0.10 |    2 |         - |          NA |
| TournamentSort   | 256  | PipeOrgan          |     6,621.7 ns |    552.70 ns |    289.07 ns |  2.08 |    0.13 |    3 |         - |          NA |
| BinomialHeapSort | 256  | PipeOrgan          |     7,899.1 ns |    423.35 ns |    221.42 ns |  2.48 |    0.13 |    3 |         - |          NA |
| PairingHeapSort  | 256  | PipeOrgan          |     7,217.6 ns |    117.57 ns |     61.49 ns |  2.26 |    0.10 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **ManyDuplicates**     |     **3,380.9 ns** |    **268.00 ns** |    **140.17 ns** |  **1.00** |    **0.05** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | ManyDuplicates     |     3,377.1 ns |     39.40 ns |     17.49 ns |  1.00 |    0.04 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | ManyDuplicates     |     4,204.2 ns |    289.73 ns |    151.53 ns |  1.25 |    0.06 |    2 |         - |          NA |
| BottomupHeapSort | 256  | ManyDuplicates     |     4,492.9 ns |    329.29 ns |    172.22 ns |  1.33 |    0.07 |    2 |         - |          NA |
| WeakHeapSort     | 256  | ManyDuplicates     |     9,808.0 ns |    373.96 ns |    195.59 ns |  2.91 |    0.12 |    4 |         - |          NA |
| SmoothSort       | 256  | ManyDuplicates     |     5,207.2 ns |    357.06 ns |    186.75 ns |  1.54 |    0.08 |    2 |         - |          NA |
| TournamentSort   | 256  | ManyDuplicates     |     8,085.0 ns |     42.99 ns |     15.33 ns |  2.39 |    0.09 |    3 |         - |          NA |
| BinomialHeapSort | 256  | ManyDuplicates     |    13,826.2 ns |    576.25 ns |    301.39 ns |  4.10 |    0.18 |    5 |         - |          NA |
| PairingHeapSort  | 256  | ManyDuplicates     |    12,994.1 ns |    299.74 ns |    156.77 ns |  3.85 |    0.15 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Random**             |    **18,557.5 ns** |    **483.81 ns** |    **253.04 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Random             |    18,134.1 ns |    683.25 ns |    357.35 ns |  0.98 |    0.02 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | Random             |    20,263.6 ns |  1,148.18 ns |    509.80 ns |  1.09 |    0.03 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | Random             |    20,433.1 ns |    495.89 ns |    259.36 ns |  1.10 |    0.02 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | Random             |    53,615.5 ns |    931.44 ns |    487.16 ns |  2.89 |    0.04 |    4 |         - |          NA |
| SmoothSort       | 1024 | Random             |    27,746.8 ns |  1,004.60 ns |    525.42 ns |  1.50 |    0.03 |    2 |         - |          NA |
| TournamentSort   | 1024 | Random             |    39,754.0 ns |  1,410.80 ns |    626.40 ns |  2.14 |    0.04 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | Random             |    87,702.3 ns |  5,447.87 ns |  2,849.34 ns |  4.73 |    0.16 |    5 |         - |          NA |
| PairingHeapSort  | 1024 | Random             |    57,087.2 ns |  2,236.84 ns |    993.17 ns |  3.08 |    0.06 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **SingleElementMoved** |    **15,385.8 ns** |    **411.45 ns** |    **182.69 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | SingleElementMoved |    15,235.2 ns |    253.05 ns |    112.36 ns |  0.99 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | SingleElementMoved |    20,603.0 ns |    475.89 ns |    211.30 ns |  1.34 |    0.02 |    3 |         - |          NA |
| BottomupHeapSort | 1024 | SingleElementMoved |    20,289.2 ns |    328.19 ns |    145.72 ns |  1.32 |    0.02 |    3 |         - |          NA |
| WeakHeapSort     | 1024 | SingleElementMoved |    43,946.3 ns |    384.23 ns |    200.96 ns |  2.86 |    0.03 |    5 |         - |          NA |
| SmoothSort       | 1024 | SingleElementMoved |     7,109.5 ns |    143.76 ns |     63.83 ns |  0.46 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 1024 | SingleElementMoved |    28,237.2 ns |  3,120.63 ns |  1,632.15 ns |  1.84 |    0.10 |    4 |         - |          NA |
| BinomialHeapSort | 1024 | SingleElementMoved |    32,344.4 ns |    260.12 ns |    136.05 ns |  2.10 |    0.02 |    4 |         - |          NA |
| PairingHeapSort  | 1024 | SingleElementMoved |    22,626.9 ns |     97.96 ns |     51.24 ns |  1.47 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Sorted**             |    **16,623.9 ns** |    **407.08 ns** |    **180.75 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Sorted             |    17,282.9 ns |    301.22 ns |    133.74 ns |  1.04 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | Sorted             |    19,674.8 ns |    901.86 ns |    471.69 ns |  1.18 |    0.03 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | Sorted             |    20,365.5 ns |    680.34 ns |    355.83 ns |  1.23 |    0.02 |    2 |         - |          NA |
| WeakHeapSort     | 1024 | Sorted             |    44,305.9 ns |    343.18 ns |    152.37 ns |  2.67 |    0.03 |    4 |         - |          NA |
| SmoothSort       | 1024 | Sorted             |     5,346.2 ns |    468.60 ns |    245.08 ns |  0.32 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 1024 | Sorted             |    20,739.8 ns |  1,330.05 ns |    590.55 ns |  1.25 |    0.04 |    2 |         - |          NA |
| BinomialHeapSort | 1024 | Sorted             |    29,229.7 ns |    220.31 ns |     97.82 ns |  1.76 |    0.02 |    3 |         - |          NA |
| PairingHeapSort  | 1024 | Sorted             |    22,299.2 ns |    294.31 ns |    153.93 ns |  1.34 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Reversed**           |    **16,113.6 ns** |  **1,123.95 ns** |    **587.85 ns** |  **1.00** |    **0.05** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Reversed           |    15,435.5 ns |    646.67 ns |    338.22 ns |  0.96 |    0.04 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | Reversed           |    20,468.3 ns |    310.46 ns |    137.85 ns |  1.27 |    0.04 |    3 |         - |          NA |
| BottomupHeapSort | 1024 | Reversed           |    20,878.3 ns |    308.31 ns |    136.89 ns |  1.30 |    0.04 |    3 |         - |          NA |
| WeakHeapSort     | 1024 | Reversed           |    48,260.7 ns |    147.79 ns |     65.62 ns |  3.00 |    0.10 |    4 |         - |          NA |
| SmoothSort       | 1024 | Reversed           |    24,749.7 ns |    747.99 ns |    332.11 ns |  1.54 |    0.06 |    3 |         - |          NA |
| TournamentSort   | 1024 | Reversed           |    24,568.6 ns |  1,483.28 ns |    658.59 ns |  1.53 |    0.06 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | Reversed           |    29,095.0 ns |    239.97 ns |    106.55 ns |  1.81 |    0.06 |    3 |         - |          NA |
| PairingHeapSort  | 1024 | Reversed           |    10,665.8 ns |    261.01 ns |    136.51 ns |  0.66 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **PipeOrgan**          |    **15,177.5 ns** |    **981.44 ns** |    **513.31 ns** |  **1.00** |    **0.05** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | PipeOrgan          |    15,337.9 ns |    403.08 ns |    178.97 ns |  1.01 |    0.03 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | PipeOrgan          |    20,323.4 ns |    833.80 ns |    370.21 ns |  1.34 |    0.05 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | PipeOrgan          |    20,626.1 ns |    448.53 ns |    199.15 ns |  1.36 |    0.05 |    2 |         - |          NA |
| WeakHeapSort     | 1024 | PipeOrgan          |    47,716.4 ns |    171.24 ns |     76.03 ns |  3.15 |    0.10 |    4 |         - |          NA |
| SmoothSort       | 1024 | PipeOrgan          |    26,483.8 ns |    508.03 ns |    225.57 ns |  1.75 |    0.06 |    3 |         - |          NA |
| TournamentSort   | 1024 | PipeOrgan          |    33,614.3 ns |  2,307.76 ns |  1,207.00 ns |  2.22 |    0.10 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | PipeOrgan          |    32,947.3 ns |    368.74 ns |    192.86 ns |  2.17 |    0.07 |    3 |         - |          NA |
| PairingHeapSort  | 1024 | PipeOrgan          |    31,389.6 ns |    468.79 ns |    208.15 ns |  2.07 |    0.07 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **ManyDuplicates**     |    **18,121.4 ns** |    **564.33 ns** |    **295.16 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | ManyDuplicates     |    17,982.3 ns |    473.83 ns |    247.82 ns |  0.99 |    0.02 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | ManyDuplicates     |    19,720.6 ns |    777.75 ns |    406.78 ns |  1.09 |    0.03 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | ManyDuplicates     |    20,049.2 ns |    505.07 ns |    224.26 ns |  1.11 |    0.02 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | ManyDuplicates     |    48,459.6 ns |    501.14 ns |    222.51 ns |  2.67 |    0.04 |    4 |         - |          NA |
| SmoothSort       | 1024 | ManyDuplicates     |    24,426.1 ns |    552.72 ns |    245.41 ns |  1.35 |    0.02 |    2 |         - |          NA |
| TournamentSort   | 1024 | ManyDuplicates     |    39,043.0 ns |  2,450.21 ns |  1,281.50 ns |  2.16 |    0.07 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | ManyDuplicates     |    65,751.6 ns |    595.03 ns |    264.20 ns |  3.63 |    0.06 |    5 |         - |          NA |
| PairingHeapSort  | 1024 | ManyDuplicates     |    52,452.3 ns |    724.77 ns |    321.80 ns |  2.90 |    0.05 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Random**             |   **182,924.8 ns** |  **2,057.04 ns** |    **913.34 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Random             |   189,875.1 ns |  1,446.98 ns |    756.80 ns |  1.04 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 4096 | Random             |   130,456.7 ns | 12,573.88 ns |  5,582.88 ns |  0.71 |    0.03 |    1 |         - |          NA |
| BottomupHeapSort | 4096 | Random             |   127,302.9 ns |  6,787.98 ns |  3,013.91 ns |  0.70 |    0.02 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | Random             |   335,625.7 ns | 43,916.58 ns | 22,969.22 ns |  1.83 |    0.12 |    3 |         - |          NA |
| SmoothSort       | 4096 | Random             |   390,044.4 ns |  1,466.94 ns |    651.33 ns |  2.13 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 4096 | Random             |   671,659.9 ns | 10,550.96 ns |  5,518.35 ns |  3.67 |    0.03 |    4 |         - |          NA |
| BinomialHeapSort | 4096 | Random             | 1,043,584.3 ns |  6,394.95 ns |  2,839.40 ns |  5.71 |    0.03 |    5 |         - |          NA |
| PairingHeapSort  | 4096 | Random             |   463,644.2 ns |  7,874.39 ns |  4,118.46 ns |  2.53 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **SingleElementMoved** |   **104,213.8 ns** |  **1,392.74 ns** |    **618.38 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 4096 | SingleElementMoved |   140,902.5 ns |  9,886.00 ns |  5,170.57 ns |  1.35 |    0.05 |    3 |         - |          NA |
| TernaryHeapSort  | 4096 | SingleElementMoved |   100,355.2 ns |    619.03 ns |    274.85 ns |  0.96 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 4096 | SingleElementMoved |   107,266.0 ns |  3,535.93 ns |  1,849.36 ns |  1.03 |    0.02 |    2 |         - |          NA |
| WeakHeapSort     | 4096 | SingleElementMoved |   213,649.2 ns |    456.51 ns |    202.69 ns |  2.05 |    0.01 |    4 |         - |          NA |
| SmoothSort       | 4096 | SingleElementMoved |    29,512.7 ns |    656.19 ns |    343.20 ns |  0.28 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 4096 | SingleElementMoved |   312,922.2 ns |  8,309.96 ns |  4,346.27 ns |  3.00 |    0.04 |    5 |         - |          NA |
| BinomialHeapSort | 4096 | SingleElementMoved |   142,304.6 ns |    579.57 ns |    257.33 ns |  1.37 |    0.01 |    3 |         - |          NA |
| PairingHeapSort  | 4096 | SingleElementMoved |    90,887.0 ns |    313.52 ns |    139.21 ns |  0.87 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Sorted**             |   **126,138.3 ns** |  **2,666.30 ns** |  **1,394.53 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Sorted             |   151,529.2 ns | 13,898.44 ns |  7,269.15 ns |  1.20 |    0.06 |    3 |         - |          NA |
| TernaryHeapSort  | 4096 | Sorted             |   100,127.2 ns |  6,605.69 ns |  3,454.90 ns |  0.79 |    0.03 |    2 |         - |          NA |
| BottomupHeapSort | 4096 | Sorted             |   103,055.2 ns |  1,093.12 ns |    485.35 ns |  0.82 |    0.01 |    2 |         - |          NA |
| WeakHeapSort     | 4096 | Sorted             |   216,008.5 ns |    755.55 ns |    335.47 ns |  1.71 |    0.02 |    4 |         - |          NA |
| SmoothSort       | 4096 | Sorted             |    21,430.1 ns |    535.25 ns |    237.66 ns |  0.17 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 4096 | Sorted             |   153,470.4 ns | 17,616.74 ns |  7,821.94 ns |  1.22 |    0.06 |    3 |         - |          NA |
| BinomialHeapSort | 4096 | Sorted             |   131,208.1 ns |    967.55 ns |    506.05 ns |  1.04 |    0.01 |    3 |         - |          NA |
| PairingHeapSort  | 4096 | Sorted             |    90,939.0 ns |    384.98 ns |    170.93 ns |  0.72 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Reversed**           |   **109,728.0 ns** |  **8,441.59 ns** |  **3,748.12 ns** |  **1.00** |    **0.05** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Reversed           |   133,728.3 ns |  6,266.81 ns |  3,277.66 ns |  1.22 |    0.05 |    2 |         - |          NA |
| TernaryHeapSort  | 4096 | Reversed           |    98,327.2 ns |    791.99 ns |    351.65 ns |  0.90 |    0.03 |    2 |         - |          NA |
| BottomupHeapSort | 4096 | Reversed           |   104,887.0 ns |  1,663.46 ns |    738.59 ns |  0.96 |    0.03 |    2 |         - |          NA |
| WeakHeapSort     | 4096 | Reversed           |   232,593.0 ns |  1,699.77 ns |    889.01 ns |  2.12 |    0.07 |    3 |         - |          NA |
| SmoothSort       | 4096 | Reversed           |   132,435.4 ns |  1,674.06 ns |    743.29 ns |  1.21 |    0.04 |    2 |         - |          NA |
| TournamentSort   | 4096 | Reversed           |   238,762.4 ns | 12,884.24 ns |  6,738.70 ns |  2.18 |    0.09 |    3 |         - |          NA |
| BinomialHeapSort | 4096 | Reversed           |   128,075.7 ns |  1,049.89 ns |    466.16 ns |  1.17 |    0.04 |    2 |         - |          NA |
| PairingHeapSort  | 4096 | Reversed           |    42,130.5 ns |    395.45 ns |    141.02 ns |  0.38 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **PipeOrgan**          |   **111,194.7 ns** | **16,710.41 ns** |  **8,739.87 ns** |  **1.01** |    **0.10** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 4096 | PipeOrgan          |   123,692.5 ns |  9,707.63 ns |  4,310.25 ns |  1.12 |    0.09 |    1 |         - |          NA |
| TernaryHeapSort  | 4096 | PipeOrgan          |    98,205.3 ns |  1,035.55 ns |    459.79 ns |  0.89 |    0.06 |    1 |         - |          NA |
| BottomupHeapSort | 4096 | PipeOrgan          |   100,814.4 ns |  1,489.59 ns |    661.39 ns |  0.91 |    0.06 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | PipeOrgan          |   233,063.8 ns |    965.43 ns |    504.94 ns |  2.11 |    0.15 |    2 |         - |          NA |
| SmoothSort       | 4096 | PipeOrgan          |   279,196.1 ns |  4,251.35 ns |  1,887.63 ns |  2.52 |    0.18 |    3 |         - |          NA |
| TournamentSort   | 4096 | PipeOrgan          |   464,242.4 ns |  9,563.40 ns |  5,001.84 ns |  4.20 |    0.30 |    4 |         - |          NA |
| BinomialHeapSort | 4096 | PipeOrgan          |   143,214.3 ns |    864.84 ns |    452.33 ns |  1.29 |    0.09 |    1 |         - |          NA |
| PairingHeapSort  | 4096 | PipeOrgan          |   120,339.9 ns |  1,173.93 ns |    521.23 ns |  1.09 |    0.08 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **ManyDuplicates**     |   **174,378.7 ns** |  **2,176.45 ns** |  **1,138.33 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 4096 | ManyDuplicates     |   176,786.1 ns |  1,091.14 ns |    484.47 ns |  1.01 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 4096 | ManyDuplicates     |   102,915.7 ns |  7,039.20 ns |  3,125.45 ns |  0.59 |    0.02 |    1 |         - |          NA |
| BottomupHeapSort | 4096 | ManyDuplicates     |   112,653.5 ns |  2,777.03 ns |  1,233.02 ns |  0.65 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | ManyDuplicates     |   235,765.4 ns |  6,069.09 ns |  2,694.71 ns |  1.35 |    0.02 |    3 |         - |          NA |
| SmoothSort       | 4096 | ManyDuplicates     |   321,008.0 ns |  4,930.67 ns |  2,189.25 ns |  1.84 |    0.02 |    4 |         - |          NA |
| TournamentSort   | 4096 | ManyDuplicates     |   611,774.6 ns |  2,568.99 ns |  1,140.65 ns |  3.51 |    0.02 |    6 |         - |          NA |
| BinomialHeapSort | 4096 | ManyDuplicates     |   721,834.1 ns |  4,989.08 ns |  2,609.39 ns |  4.14 |    0.03 |    6 |         - |          NA |
| PairingHeapSort  | 4096 | ManyDuplicates     |   410,418.9 ns |  4,174.16 ns |  2,183.17 ns |  2.35 |    0.02 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Random**             |   **531,869.1 ns** |  **4,869.83 ns** |  **2,162.23 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Random             |   522,796.1 ns |  4,377.25 ns |  2,289.38 ns |  0.98 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | Random             |   658,628.2 ns | 11,650.45 ns |  5,172.87 ns |  1.24 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | Random             |   659,802.6 ns |  4,247.08 ns |  2,221.30 ns |  1.24 |    0.01 |    2 |         - |          NA |
| WeakHeapSort     | 8192 | Random             |   980,337.4 ns |  1,019.21 ns |    533.07 ns |  1.84 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 8192 | Random             |   939,117.0 ns |  1,984.54 ns |  1,037.95 ns |  1.77 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 8192 | Random             | 1,501,155.1 ns | 13,997.73 ns |  7,321.08 ns |  2.82 |    0.02 |    4 |         - |          NA |
| BinomialHeapSort | 8192 | Random             | 2,328,132.3 ns |  7,100.82 ns |  3,713.86 ns |  4.38 |    0.02 |    5 |         - |          NA |
| PairingHeapSort  | 8192 | Random             | 1,116,597.4 ns |    905.30 ns |    401.96 ns |  2.10 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **SingleElementMoved** |   **362,402.9 ns** |  **4,927.23 ns** |  **2,577.04 ns** |  **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 8192 | SingleElementMoved |   393,665.9 ns |  2,549.57 ns |  1,132.03 ns |  1.09 |    0.01 |    4 |         - |          NA |
| TernaryHeapSort  | 8192 | SingleElementMoved |   398,193.9 ns |  1,591.91 ns |    706.82 ns |  1.10 |    0.01 |    4 |         - |          NA |
| BottomupHeapSort | 8192 | SingleElementMoved |   415,398.8 ns |  2,663.16 ns |  1,392.88 ns |  1.15 |    0.01 |    4 |         - |          NA |
| WeakHeapSort     | 8192 | SingleElementMoved |   467,003.6 ns |    366.61 ns |    162.78 ns |  1.29 |    0.01 |    4 |         - |          NA |
| SmoothSort       | 8192 | SingleElementMoved |    58,749.1 ns |  1,045.83 ns |    546.99 ns |  0.16 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 8192 | SingleElementMoved |   790,919.4 ns |  8,035.11 ns |  3,567.64 ns |  2.18 |    0.02 |    5 |         - |          NA |
| BinomialHeapSort | 8192 | SingleElementMoved |   297,628.0 ns |    796.05 ns |    416.35 ns |  0.82 |    0.01 |    3 |         - |          NA |
| PairingHeapSort  | 8192 | SingleElementMoved |   183,038.0 ns |    947.34 ns |    420.63 ns |  0.51 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Sorted**             |   **326,463.5 ns** |  **1,315.17 ns** |    **583.95 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Sorted             |   402,144.5 ns |  2,893.72 ns |  1,513.47 ns |  1.23 |    0.00 |    3 |         - |          NA |
| TernaryHeapSort  | 8192 | Sorted             |   350,401.1 ns |  2,575.25 ns |  1,143.43 ns |  1.07 |    0.00 |    3 |         - |          NA |
| BottomupHeapSort | 8192 | Sorted             |   408,809.7 ns |    931.89 ns |    413.77 ns |  1.25 |    0.00 |    3 |         - |          NA |
| WeakHeapSort     | 8192 | Sorted             |   471,219.5 ns |    866.48 ns |    453.18 ns |  1.44 |    0.00 |    3 |         - |          NA |
| SmoothSort       | 8192 | Sorted             |    42,511.3 ns |    902.60 ns |    472.08 ns |  0.13 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 8192 | Sorted             |   505,315.0 ns | 22,187.05 ns | 11,604.26 ns |  1.55 |    0.03 |    3 |         - |          NA |
| BinomialHeapSort | 8192 | Sorted             |   275,124.8 ns |  1,041.78 ns |    544.87 ns |  0.84 |    0.00 |    3 |         - |          NA |
| PairingHeapSort  | 8192 | Sorted             |   180,898.3 ns |  1,495.40 ns |    782.12 ns |  0.55 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Reversed**           |   **382,317.6 ns** | **17,608.24 ns** |  **9,209.45 ns** |  **1.00** |    **0.03** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Reversed           |   346,866.2 ns |  2,749.29 ns |  1,437.93 ns |  0.91 |    0.02 |    3 |         - |          NA |
| TernaryHeapSort  | 8192 | Reversed           |   413,940.7 ns |  1,657.02 ns |    866.65 ns |  1.08 |    0.02 |    3 |         - |          NA |
| BottomupHeapSort | 8192 | Reversed           |   484,331.2 ns |  2,779.53 ns |  1,453.75 ns |  1.27 |    0.03 |    3 |         - |          NA |
| WeakHeapSort     | 8192 | Reversed           |   505,124.0 ns |  3,296.89 ns |  1,175.70 ns |  1.32 |    0.03 |    3 |         - |          NA |
| SmoothSort       | 8192 | Reversed           |   573,115.5 ns |  1,852.84 ns |    969.07 ns |  1.50 |    0.03 |    3 |         - |          NA |
| TournamentSort   | 8192 | Reversed           |   687,195.6 ns |  9,483.40 ns |  4,960.00 ns |  1.80 |    0.04 |    4 |         - |          NA |
| BinomialHeapSort | 8192 | Reversed           |   269,559.4 ns |  1,350.91 ns |    706.55 ns |  0.71 |    0.02 |    2 |         - |          NA |
| PairingHeapSort  | 8192 | Reversed           |    85,922.0 ns |    787.37 ns |    411.81 ns |  0.22 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **PipeOrgan**          |   **381,665.4 ns** | **20,550.76 ns** | **10,748.44 ns** |  **1.00** |    **0.04** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 8192 | PipeOrgan          |   405,656.8 ns |  2,169.12 ns |  1,134.49 ns |  1.06 |    0.03 |    3 |         - |          NA |
| TernaryHeapSort  | 8192 | PipeOrgan          |   465,448.9 ns |  1,660.50 ns |    868.48 ns |  1.22 |    0.03 |    3 |         - |          NA |
| BottomupHeapSort | 8192 | PipeOrgan          |   474,989.3 ns |  1,724.88 ns |    902.15 ns |  1.25 |    0.03 |    3 |         - |          NA |
| WeakHeapSort     | 8192 | PipeOrgan          |   508,185.5 ns |  1,452.11 ns |    759.48 ns |  1.33 |    0.04 |    3 |         - |          NA |
| SmoothSort       | 8192 | PipeOrgan          |   710,823.9 ns |  3,248.16 ns |  1,698.85 ns |  1.86 |    0.05 |    4 |         - |          NA |
| TournamentSort   | 8192 | PipeOrgan          | 1,121,376.9 ns | 13,793.52 ns |  7,214.28 ns |  2.94 |    0.08 |    5 |         - |          NA |
| BinomialHeapSort | 8192 | PipeOrgan          |   297,500.6 ns |    779.08 ns |    407.47 ns |  0.78 |    0.02 |    2 |         - |          NA |
| PairingHeapSort  | 8192 | PipeOrgan          |   245,800.3 ns |  2,134.34 ns |  1,116.30 ns |  0.64 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **ManyDuplicates**     |   **507,589.8 ns** |  **5,243.51 ns** |  **2,742.46 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | ManyDuplicates     |   508,002.6 ns |  3,141.18 ns |  1,642.90 ns |  1.00 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | ManyDuplicates     |   591,374.4 ns |  8,750.11 ns |  4,576.48 ns |  1.17 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 8192 | ManyDuplicates     |   607,811.6 ns |  1,534.06 ns |    802.34 ns |  1.20 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | ManyDuplicates     |   676,155.4 ns |  1,816.76 ns |    806.65 ns |  1.33 |    0.01 |    1 |         - |          NA |
| SmoothSort       | 8192 | ManyDuplicates     |   789,587.8 ns |  1,702.65 ns |    890.52 ns |  1.56 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 8192 | ManyDuplicates     | 1,388,989.7 ns |  9,274.03 ns |  3,307.21 ns |  2.74 |    0.02 |    3 |         - |          NA |
| BinomialHeapSort | 8192 | ManyDuplicates     | 1,555,138.8 ns |  1,897.22 ns |    842.38 ns |  3.06 |    0.02 |    3 |         - |          NA |
| PairingHeapSort  | 8192 | ManyDuplicates     |   959,775.9 ns |  3,700.05 ns |  1,935.20 ns |  1.89 |    0.01 |    2 |         - |          NA |

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
| **InsertionSort**          | **256**  | **Random**             |   **7,063.3 ns** |    **371.14 ns** |   **194.11 ns** |  **1.00** |    **0.04** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Random             |   7,378.6 ns |    317.62 ns |   166.12 ns |  1.05 |    0.03 |    3 |         - |          NA |
| BinaryInsertSort       | 256  | Random             |   5,560.3 ns |    526.00 ns |   275.11 ns |  0.79 |    0.04 |    2 |         - |          NA |
| GnomeSort              | 256  | Random             |  24,115.3 ns |     90.56 ns |    32.29 ns |  3.42 |    0.09 |    6 |         - |          NA |
| LibrarySort            | 256  | Random             |  17,150.2 ns |    257.88 ns |   114.50 ns |  2.43 |    0.06 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | Random             |  13,397.6 ns |    377.65 ns |   167.68 ns |  1.90 |    0.05 |    4 |         - |          NA |
| ShellSortKnuth1973     | 256  | Random             |   2,638.2 ns |     18.91 ns |     6.74 ns |  0.37 |    0.01 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Random             |   2,925.0 ns |    287.17 ns |   150.19 ns |  0.41 |    0.02 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | Random             |   2,708.0 ns |     23.96 ns |     8.55 ns |  0.38 |    0.01 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | Random             |   2,586.6 ns |     11.81 ns |     4.21 ns |  0.37 |    0.01 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Random             |   2,797.5 ns |    364.40 ns |   190.59 ns |  0.40 |    0.03 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **256**  | **SingleElementMoved** |     **443.6 ns** |      **4.16 ns** |     **1.85 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 256  | SingleElementMoved |     434.8 ns |    122.92 ns |    54.58 ns |  0.98 |    0.12 |    1 |         - |          NA |
| BinaryInsertSort       | 256  | SingleElementMoved |   1,187.3 ns |     83.02 ns |    43.42 ns |  2.68 |    0.09 |    2 |         - |          NA |
| GnomeSort              | 256  | SingleElementMoved |     516.0 ns |      0.98 ns |     0.43 ns |  1.16 |    0.00 |    1 |         - |          NA |
| LibrarySort            | 256  | SingleElementMoved |  15,542.0 ns |    121.14 ns |    63.36 ns | 35.04 |    0.19 |    4 |         - |          NA |
| MergeInsertionSort     | 256  | SingleElementMoved |  12,244.2 ns |    451.27 ns |   200.37 ns | 27.60 |    0.44 |    3 |         - |          NA |
| ShellSortKnuth1973     | 256  | SingleElementMoved |   1,332.4 ns |      3.61 ns |     1.60 ns |  3.00 |    0.01 |    2 |         - |          NA |
| ShellSortSedgewick1986 | 256  | SingleElementMoved |   1,311.7 ns |     48.47 ns |    21.52 ns |  2.96 |    0.05 |    2 |         - |          NA |
| ShellSortTokuda1992    | 256  | SingleElementMoved |   1,606.8 ns |      5.55 ns |     2.90 ns |  3.62 |    0.02 |    2 |         - |          NA |
| ShellSortCiura2001     | 256  | SingleElementMoved |   1,516.1 ns |    241.22 ns |   126.16 ns |  3.42 |    0.27 |    2 |         - |          NA |
| ShellSortLee2021       | 256  | SingleElementMoved |   1,719.9 ns |    244.97 ns |   128.12 ns |  3.88 |    0.27 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **256**  | **Sorted**             |     **366.1 ns** |      **1.57 ns** |     **0.70 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Sorted             |     280.9 ns |      1.32 ns |     0.69 ns |  0.77 |    0.00 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | Sorted             |     193.3 ns |      0.91 ns |     0.48 ns |  0.53 |    0.00 |    1 |         - |          NA |
| GnomeSort              | 256  | Sorted             |     190.4 ns |      0.80 ns |     0.35 ns |  0.52 |    0.00 |    1 |         - |          NA |
| LibrarySort            | 256  | Sorted             |  15,850.7 ns |    111.68 ns |    58.41 ns | 43.30 |    0.17 |    6 |         - |          NA |
| MergeInsertionSort     | 256  | Sorted             |  12,568.8 ns |    285.31 ns |   149.22 ns | 34.33 |    0.39 |    5 |         - |          NA |
| ShellSortKnuth1973     | 256  | Sorted             |   1,249.9 ns |    176.11 ns |    92.11 ns |  3.41 |    0.24 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Sorted             |   1,194.8 ns |      3.72 ns |     1.65 ns |  3.26 |    0.01 |    4 |         - |          NA |
| ShellSortTokuda1992    | 256  | Sorted             |   1,501.2 ns |     88.94 ns |    39.49 ns |  4.10 |    0.10 |    4 |         - |          NA |
| ShellSortCiura2001     | 256  | Sorted             |   1,280.4 ns |      2.78 ns |     1.45 ns |  3.50 |    0.01 |    4 |         - |          NA |
| ShellSortLee2021       | 256  | Sorted             |   1,498.1 ns |     84.43 ns |    37.49 ns |  4.09 |    0.10 |    4 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **256**  | **Reversed**           |  **15,515.8 ns** |    **119.79 ns** |    **62.65 ns** |  **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Reversed           |  20,407.7 ns |    565.31 ns |   251.00 ns |  1.32 |    0.02 |    5 |         - |          NA |
| BinaryInsertSort       | 256  | Reversed           |   6,671.1 ns |    379.45 ns |   198.46 ns |  0.43 |    0.01 |    2 |         - |          NA |
| GnomeSort              | 256  | Reversed           |  68,491.6 ns |  2,054.48 ns |   912.20 ns |  4.41 |    0.06 |    6 |         - |          NA |
| LibrarySort            | 256  | Reversed           |  22,047.6 ns |    191.11 ns |    84.85 ns |  1.42 |    0.01 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | Reversed           |  12,282.1 ns |    349.26 ns |   182.67 ns |  0.79 |    0.01 |    3 |         - |          NA |
| ShellSortKnuth1973     | 256  | Reversed           |   2,354.4 ns |  1,050.46 ns |   549.41 ns |  0.15 |    0.03 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Reversed           |   1,864.8 ns |     32.69 ns |    14.52 ns |  0.12 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | Reversed           |   2,085.0 ns |     29.08 ns |    12.91 ns |  0.13 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | Reversed           |   1,967.8 ns |     17.70 ns |     7.86 ns |  0.13 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Reversed           |   2,036.3 ns |      5.24 ns |     1.87 ns |  0.13 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **256**  | **PipeOrgan**          |   **8,216.3 ns** |    **252.88 ns** |   **132.26 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 256  | PipeOrgan          |  10,341.0 ns |    278.62 ns |   123.71 ns |  1.26 |    0.02 |    4 |         - |          NA |
| BinaryInsertSort       | 256  | PipeOrgan          |   4,036.8 ns |    516.36 ns |   270.07 ns |  0.49 |    0.03 |    2 |         - |          NA |
| GnomeSort              | 256  | PipeOrgan          |  33,593.2 ns |    698.16 ns |   365.15 ns |  4.09 |    0.07 |    6 |         - |          NA |
| LibrarySort            | 256  | PipeOrgan          |  14,335.5 ns |    130.68 ns |    68.35 ns |  1.75 |    0.03 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | PipeOrgan          |  12,465.7 ns |    304.66 ns |   135.27 ns |  1.52 |    0.03 |    5 |         - |          NA |
| ShellSortKnuth1973     | 256  | PipeOrgan          |   1,807.4 ns |     25.44 ns |    11.29 ns |  0.22 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | PipeOrgan          |   1,846.7 ns |    160.89 ns |    71.44 ns |  0.22 |    0.01 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | PipeOrgan          |   2,141.1 ns |     21.44 ns |     7.64 ns |  0.26 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | PipeOrgan          |   2,122.7 ns |    199.43 ns |   104.31 ns |  0.26 |    0.01 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | PipeOrgan          |   2,085.8 ns |    160.68 ns |    84.04 ns |  0.25 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **256**  | **ManyDuplicates**     |   **6,796.4 ns** |    **226.26 ns** |   **118.34 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 256  | ManyDuplicates     |   7,117.7 ns |    244.30 ns |   127.77 ns |  1.05 |    0.02 |    3 |         - |          NA |
| BinaryInsertSort       | 256  | ManyDuplicates     |   5,482.8 ns |    311.21 ns |   162.77 ns |  0.81 |    0.03 |    2 |         - |          NA |
| GnomeSort              | 256  | ManyDuplicates     |  23,379.6 ns |    363.09 ns |   161.22 ns |  3.44 |    0.06 |    6 |         - |          NA |
| LibrarySort            | 256  | ManyDuplicates     |  16,141.9 ns |    226.29 ns |   118.36 ns |  2.38 |    0.04 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | ManyDuplicates     |  13,226.7 ns |    366.16 ns |   191.51 ns |  1.95 |    0.04 |    4 |         - |          NA |
| ShellSortKnuth1973     | 256  | ManyDuplicates     |   2,330.7 ns |    103.54 ns |    54.15 ns |  0.34 |    0.01 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | ManyDuplicates     |   2,223.5 ns |     20.38 ns |     7.27 ns |  0.33 |    0.01 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | ManyDuplicates     |   2,166.8 ns |     19.06 ns |     6.80 ns |  0.32 |    0.01 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | ManyDuplicates     |   2,274.4 ns |    237.93 ns |   105.64 ns |  0.33 |    0.02 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | ManyDuplicates     |   2,110.0 ns |     73.21 ns |    32.50 ns |  0.31 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **1024** | **Random**             | **118,967.6 ns** |  **5,290.39 ns** | **2,766.98 ns** |  **1.00** |    **0.03** |    **5** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Random             | 135,569.8 ns |  2,371.22 ns | 1,240.19 ns |  1.14 |    0.03 |    5 |         - |          NA |
| BinaryInsertSort       | 1024 | Random             |  35,690.0 ns |  1,784.85 ns |   933.51 ns |  0.30 |    0.01 |    2 |         - |          NA |
| GnomeSort              | 1024 | Random             | 387,851.5 ns |  3,440.30 ns | 1,799.34 ns |  3.26 |    0.07 |    6 |         - |          NA |
| LibrarySort            | 1024 | Random             |  72,956.5 ns |  1,212.28 ns |   634.05 ns |  0.61 |    0.01 |    3 |         - |          NA |
| MergeInsertionSort     | 1024 | Random             |  91,953.5 ns |  1,500.99 ns |   666.45 ns |  0.77 |    0.02 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Random             |  14,671.4 ns |    346.30 ns |   153.76 ns |  0.12 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Random             |  16,442.1 ns |  3,594.66 ns | 1,880.08 ns |  0.14 |    0.02 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Random             |  14,519.2 ns |    423.77 ns |   188.16 ns |  0.12 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Random             |  14,264.2 ns |    324.63 ns |   144.14 ns |  0.12 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Random             |  14,491.1 ns |    309.02 ns |   137.21 ns |  0.12 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **1024** | **SingleElementMoved** |   **1,859.3 ns** |     **19.05 ns** |     **6.79 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | SingleElementMoved |   1,182.2 ns |      8.83 ns |     3.92 ns |  0.64 |    0.00 |    1 |         - |          NA |
| BinaryInsertSort       | 1024 | SingleElementMoved |   5,706.1 ns |    266.63 ns |   139.45 ns |  3.07 |    0.07 |    3 |         - |          NA |
| GnomeSort              | 1024 | SingleElementMoved |   2,032.2 ns |      2.59 ns |     0.92 ns |  1.09 |    0.00 |    2 |         - |          NA |
| LibrarySort            | 1024 | SingleElementMoved |  77,305.5 ns |    986.82 ns |   351.91 ns | 41.58 |    0.22 |    5 |         - |          NA |
| MergeInsertionSort     | 1024 | SingleElementMoved |  75,965.4 ns |    798.28 ns |   417.51 ns | 40.86 |    0.25 |    5 |         - |          NA |
| ShellSortKnuth1973     | 1024 | SingleElementMoved |   6,985.0 ns |    356.08 ns |   186.24 ns |  3.76 |    0.10 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | SingleElementMoved |   7,404.7 ns |    274.39 ns |   143.51 ns |  3.98 |    0.07 |    4 |         - |          NA |
| ShellSortTokuda1992    | 1024 | SingleElementMoved |   8,024.6 ns |     43.43 ns |    19.29 ns |  4.32 |    0.02 |    4 |         - |          NA |
| ShellSortCiura2001     | 1024 | SingleElementMoved |   8,127.7 ns |    136.07 ns |    71.17 ns |  4.37 |    0.04 |    4 |         - |          NA |
| ShellSortLee2021       | 1024 | SingleElementMoved |   8,080.9 ns |     37.63 ns |    19.68 ns |  4.35 |    0.02 |    4 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **1024** | **Sorted**             |   **1,446.2 ns** |     **39.96 ns** |    **17.74 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Sorted             |   1,083.3 ns |      2.18 ns |     0.78 ns |  0.75 |    0.01 |    2 |         - |          NA |
| BinaryInsertSort       | 1024 | Sorted             |     740.1 ns |     41.01 ns |    18.21 ns |  0.51 |    0.01 |    1 |         - |          NA |
| GnomeSort              | 1024 | Sorted             |     737.1 ns |     40.17 ns |    17.84 ns |  0.51 |    0.01 |    1 |         - |          NA |
| LibrarySort            | 1024 | Sorted             |  79,295.2 ns |    908.54 ns |   403.40 ns | 54.84 |    0.67 |    5 |         - |          NA |
| MergeInsertionSort     | 1024 | Sorted             |  74,655.0 ns |    578.93 ns |   302.79 ns | 51.63 |    0.62 |    5 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Sorted             |   6,030.3 ns |    233.91 ns |   122.34 ns |  4.17 |    0.09 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Sorted             |   6,784.7 ns |    234.16 ns |   122.47 ns |  4.69 |    0.10 |    4 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Sorted             |   7,154.5 ns |     25.25 ns |     9.00 ns |  4.95 |    0.06 |    4 |         - |          NA |
| ShellSortCiura2001     | 1024 | Sorted             |   7,324.6 ns |    184.62 ns |    96.56 ns |  5.07 |    0.09 |    4 |         - |          NA |
| ShellSortLee2021       | 1024 | Sorted             |   7,259.5 ns |    263.63 ns |   137.88 ns |  5.02 |    0.11 |    4 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **1024** | **Reversed**           | **229,571.5 ns** |    **952.34 ns** |   **498.09 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Reversed           | 314,836.1 ns |    820.47 ns |   364.29 ns |  1.37 |    0.00 |    5 |         - |          NA |
| BinaryInsertSort       | 1024 | Reversed           |  46,653.5 ns |    441.89 ns |   231.12 ns |  0.20 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | Reversed           | 970,893.9 ns |  4,768.69 ns | 2,494.12 ns |  4.23 |    0.01 |    6 |         - |          NA |
| LibrarySort            | 1024 | Reversed           | 192,430.7 ns |    532.82 ns |   236.58 ns |  0.84 |    0.00 |    4 |         - |          NA |
| MergeInsertionSort     | 1024 | Reversed           |  75,729.6 ns |    570.71 ns |   253.40 ns |  0.33 |    0.00 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Reversed           |   9,041.3 ns |     18.61 ns |     6.64 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Reversed           |  11,036.0 ns |  1,789.51 ns |   935.95 ns |  0.05 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Reversed           |  10,475.8 ns |    348.37 ns |   182.20 ns |  0.05 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Reversed           |  10,210.3 ns |    468.58 ns |   245.08 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Reversed           |  10,492.2 ns |    389.95 ns |   203.95 ns |  0.05 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **1024** | **PipeOrgan**          | **116,623.1 ns** |  **1,159.13 ns** |   **413.36 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | PipeOrgan          | 158,903.0 ns |  1,929.67 ns |   856.78 ns |  1.36 |    0.01 |    5 |         - |          NA |
| BinaryInsertSort       | 1024 | PipeOrgan          |  25,443.4 ns |    947.67 ns |   495.65 ns |  0.22 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | PipeOrgan          | 492,679.5 ns | 14,333.59 ns | 7,496.75 ns |  4.22 |    0.06 |    6 |         - |          NA |
| LibrarySort            | 1024 | PipeOrgan          |  71,402.0 ns |    639.61 ns |   334.53 ns |  0.61 |    0.00 |    3 |         - |          NA |
| MergeInsertionSort     | 1024 | PipeOrgan          |  76,134.2 ns |    456.32 ns |   238.66 ns |  0.65 |    0.00 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | PipeOrgan          |   9,382.2 ns |    310.36 ns |   162.32 ns |  0.08 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | PipeOrgan          |   9,652.8 ns |    383.75 ns |   200.71 ns |  0.08 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | PipeOrgan          |  10,827.7 ns |    246.87 ns |   129.12 ns |  0.09 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | PipeOrgan          |  10,548.4 ns |    352.62 ns |   156.56 ns |  0.09 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | PipeOrgan          |  10,745.3 ns |    275.60 ns |   144.14 ns |  0.09 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **1024** | **ManyDuplicates**     | **113,873.2 ns** |    **674.98 ns** |   **240.70 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | ManyDuplicates     | 133,055.4 ns |  7,158.76 ns | 3,744.17 ns |  1.17 |    0.03 |    5 |         - |          NA |
| BinaryInsertSort       | 1024 | ManyDuplicates     |  35,692.3 ns |  1,344.06 ns |   702.97 ns |  0.31 |    0.01 |    2 |         - |          NA |
| GnomeSort              | 1024 | ManyDuplicates     | 376,972.9 ns |  2,826.89 ns | 1,478.52 ns |  3.31 |    0.01 |    6 |         - |          NA |
| LibrarySort            | 1024 | ManyDuplicates     |  72,848.5 ns |    285.55 ns |   126.79 ns |  0.64 |    0.00 |    3 |         - |          NA |
| MergeInsertionSort     | 1024 | ManyDuplicates     |  92,817.9 ns |  2,495.17 ns | 1,107.87 ns |  0.82 |    0.01 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | ManyDuplicates     |  11,605.3 ns |    539.08 ns |   281.95 ns |  0.10 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | ManyDuplicates     |  10,822.1 ns |    485.69 ns |   254.03 ns |  0.10 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | ManyDuplicates     |  10,943.2 ns |    283.41 ns |   148.23 ns |  0.10 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | ManyDuplicates     |  11,005.0 ns |    396.08 ns |   207.16 ns |  0.10 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | ManyDuplicates     |  11,031.0 ns |    323.95 ns |   169.43 ns |  0.10 |    0.00 |    1 |         - |          NA |

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
| **MergeSort**                | **256**  | **Random**             |     **8,785.4 ns** |    **411.18 ns** |    **215.06 ns** |  **1.00** |    **0.03** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Random             |     8,748.0 ns |    481.79 ns |    251.99 ns |  1.00 |    0.04 |    3 |         - |          NA |
| BottomupMergeSort        | 256  | Random             |     4,741.4 ns |    272.17 ns |    120.85 ns |  0.54 |    0.02 |    2 |         - |          NA |
| StdStableSort            | 256  | Random             |     2,619.1 ns |     50.49 ns |     22.42 ns |  0.30 |    0.01 |    1 |         - |          NA |
| RotateMergeSort          | 256  | Random             |    10,251.1 ns |    332.13 ns |    173.71 ns |  1.17 |    0.03 |    3 |         - |          NA |
| RotateMergeSortRecursive | 256  | Random             |    12,634.4 ns |    334.17 ns |    148.38 ns |  1.44 |    0.04 |    4 |         - |          NA |
| SymMergeSort             | 256  | Random             |     7,271.4 ns |    142.31 ns |     63.19 ns |  0.83 |    0.02 |    3 |         - |          NA |
| BlockMergeSort           | 256  | Random             |     5,012.6 ns |    111.23 ns |     49.39 ns |  0.57 |    0.01 |    2 |         - |          NA |
| NaturalMergeSort         | 256  | Random             |     5,111.9 ns |    322.95 ns |    168.91 ns |  0.58 |    0.02 |    2 |         - |          NA |
| TimSort                  | 256  | Random             |     4,147.0 ns |    286.38 ns |    127.16 ns |  0.47 |    0.02 |    2 |         - |          NA |
| PowerSort                | 256  | Random             |     2,363.0 ns |    228.45 ns |    101.43 ns |  0.27 |    0.01 |    1 |         - |          NA |
| ShiftSort                | 256  | Random             |     3,865.2 ns |    234.48 ns |    104.11 ns |  0.44 |    0.01 |    2 |         - |          NA |
| SpinSort                 | 256  | Random             |     2,262.3 ns |     74.42 ns |     33.04 ns |  0.26 |    0.01 |    1 |         - |          NA |
| Glidesort                | 256  | Random             |     3,713.3 ns |    311.52 ns |    162.93 ns |  0.42 |    0.02 |    2 |         - |          NA |
| Driftsort                | 256  | Random             |     4,421.5 ns |     47.73 ns |     17.02 ns |  0.50 |    0.01 |    2 |         - |          NA |
| FlatStableSort           | 256  | Random             |     2,809.2 ns |    390.05 ns |    204.00 ns |  0.32 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **SingleElementMoved** |     **5,100.3 ns** |    **813.17 ns** |    **425.30 ns** |  **1.01** |    **0.11** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | SingleElementMoved |     5,687.9 ns |    429.54 ns |    224.66 ns |  1.12 |    0.09 |    8 |         - |          NA |
| BottomupMergeSort        | 256  | SingleElementMoved |     1,646.0 ns |     33.66 ns |     14.95 ns |  0.32 |    0.02 |    6 |         - |          NA |
| StdStableSort            | 256  | SingleElementMoved |       745.6 ns |      5.88 ns |      2.61 ns |  0.15 |    0.01 |    3 |         - |          NA |
| RotateMergeSort          | 256  | SingleElementMoved |       611.8 ns |     28.92 ns |     10.31 ns |  0.12 |    0.01 |    3 |         - |          NA |
| RotateMergeSortRecursive | 256  | SingleElementMoved |       630.4 ns |      3.08 ns |      1.10 ns |  0.12 |    0.01 |    3 |         - |          NA |
| SymMergeSort             | 256  | SingleElementMoved |       702.2 ns |     21.03 ns |     11.00 ns |  0.14 |    0.01 |    3 |         - |          NA |
| BlockMergeSort           | 256  | SingleElementMoved |     3,211.7 ns |    465.18 ns |    206.54 ns |  0.63 |    0.06 |    7 |         - |          NA |
| NaturalMergeSort         | 256  | SingleElementMoved |       632.3 ns |     47.32 ns |     21.01 ns |  0.12 |    0.01 |    3 |         - |          NA |
| TimSort                  | 256  | SingleElementMoved |       275.6 ns |      8.65 ns |      3.84 ns |  0.05 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | SingleElementMoved |       417.1 ns |      7.21 ns |      3.20 ns |  0.08 |    0.01 |    2 |         - |          NA |
| ShiftSort                | 256  | SingleElementMoved |       399.3 ns |      3.05 ns |      1.35 ns |  0.08 |    0.01 |    2 |         - |          NA |
| SpinSort                 | 256  | SingleElementMoved |       954.8 ns |     11.17 ns |      5.84 ns |  0.19 |    0.01 |    4 |         - |          NA |
| Glidesort                | 256  | SingleElementMoved |     1,242.0 ns |      5.89 ns |      2.10 ns |  0.24 |    0.02 |    5 |         - |          NA |
| Driftsort                | 256  | SingleElementMoved |     1,204.1 ns |      8.08 ns |      3.59 ns |  0.24 |    0.02 |    5 |         - |          NA |
| FlatStableSort           | 256  | SingleElementMoved |     1,280.1 ns |     34.02 ns |     12.13 ns |  0.25 |    0.02 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **Sorted**             |     **4,350.4 ns** |    **273.31 ns** |    **142.95 ns** |  **1.00** |    **0.04** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Sorted             |     5,442.5 ns |    354.84 ns |    185.59 ns |  1.25 |    0.06 |    7 |         - |          NA |
| BottomupMergeSort        | 256  | Sorted             |     1,253.8 ns |     14.10 ns |      6.26 ns |  0.29 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 256  | Sorted             |       658.7 ns |      3.64 ns |      1.62 ns |  0.15 |    0.00 |    3 |         - |          NA |
| RotateMergeSort          | 256  | Sorted             |       291.9 ns |      3.60 ns |      1.60 ns |  0.07 |    0.00 |    1 |         - |          NA |
| RotateMergeSortRecursive | 256  | Sorted             |       442.0 ns |      0.91 ns |      0.40 ns |  0.10 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 256  | Sorted             |       379.0 ns |      2.72 ns |      1.21 ns |  0.09 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 256  | Sorted             |     2,642.0 ns |     49.12 ns |     25.69 ns |  0.61 |    0.02 |    5 |         - |          NA |
| NaturalMergeSort         | 256  | Sorted             |       253.1 ns |      3.55 ns |      1.58 ns |  0.06 |    0.00 |    1 |         - |          NA |
| TimSort                  | 256  | Sorted             |       221.6 ns |     78.49 ns |     41.05 ns |  0.05 |    0.01 |    1 |         - |          NA |
| PowerSort                | 256  | Sorted             |       166.5 ns |      1.34 ns |      0.70 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Sorted             |       188.3 ns |      4.49 ns |      1.99 ns |  0.04 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 256  | Sorted             |       168.5 ns |     42.71 ns |     22.34 ns |  0.04 |    0.00 |    1 |         - |          NA |
| Glidesort                | 256  | Sorted             |       243.7 ns |     70.37 ns |     36.81 ns |  0.06 |    0.01 |    1 |         - |          NA |
| Driftsort                | 256  | Sorted             |       216.1 ns |      3.70 ns |      1.64 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | Sorted             |     1,311.5 ns |    235.65 ns |    123.25 ns |  0.30 |    0.03 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **Reversed**           |     **9,113.4 ns** |    **134.16 ns** |     **70.17 ns** |  **1.00** |    **0.01** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Reversed           |     8,454.3 ns |    193.51 ns |    101.21 ns |  0.93 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 256  | Reversed           |     4,959.7 ns |    302.10 ns |    158.01 ns |  0.54 |    0.02 |    4 |         - |          NA |
| StdStableSort            | 256  | Reversed           |     6,508.0 ns |    317.12 ns |    165.86 ns |  0.71 |    0.02 |    5 |         - |          NA |
| RotateMergeSort          | 256  | Reversed           |     1,919.2 ns |      6.47 ns |      3.38 ns |  0.21 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 256  | Reversed           |     2,251.3 ns |      6.82 ns |      3.03 ns |  0.25 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 256  | Reversed           |     2,200.0 ns |    279.53 ns |    124.11 ns |  0.24 |    0.01 |    2 |         - |          NA |
| BlockMergeSort           | 256  | Reversed           |     3,173.6 ns |      5.35 ns |      2.38 ns |  0.35 |    0.00 |    3 |         - |          NA |
| NaturalMergeSort         | 256  | Reversed           |       342.1 ns |      5.49 ns |      2.87 ns |  0.04 |    0.00 |    1 |         - |          NA |
| TimSort                  | 256  | Reversed           |       268.0 ns |     97.89 ns |     43.46 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | Reversed           |       242.8 ns |     52.57 ns |     23.34 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Reversed           |       258.4 ns |      3.89 ns |      1.73 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 256  | Reversed           |       247.4 ns |      2.32 ns |      1.21 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 256  | Reversed           |       285.9 ns |      1.89 ns |      0.84 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 256  | Reversed           |       328.7 ns |    104.26 ns |     46.29 ns |  0.04 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | Reversed           |     2,890.3 ns |     15.27 ns |      6.78 ns |  0.32 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **PipeOrgan**          |     **6,909.7 ns** |    **277.29 ns** |    **145.03 ns** |  **1.00** |    **0.03** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | PipeOrgan          |     7,059.0 ns |    267.33 ns |    139.82 ns |  1.02 |    0.03 |    7 |         - |          NA |
| BottomupMergeSort        | 256  | PipeOrgan          |     3,284.4 ns |    240.99 ns |    126.04 ns |  0.48 |    0.02 |    5 |         - |          NA |
| StdStableSort            | 256  | PipeOrgan          |     3,723.8 ns |    288.29 ns |    150.78 ns |  0.54 |    0.02 |    5 |         - |          NA |
| RotateMergeSort          | 256  | PipeOrgan          |     4,251.2 ns |    322.61 ns |    168.73 ns |  0.62 |    0.03 |    5 |         - |          NA |
| RotateMergeSortRecursive | 256  | PipeOrgan          |     5,262.8 ns |    101.62 ns |     36.24 ns |  0.76 |    0.02 |    6 |         - |          NA |
| SymMergeSort             | 256  | PipeOrgan          |     2,739.3 ns |    341.34 ns |    178.53 ns |  0.40 |    0.03 |    5 |         - |          NA |
| BlockMergeSort           | 256  | PipeOrgan          |     3,151.1 ns |     29.51 ns |     13.10 ns |  0.46 |    0.01 |    5 |         - |          NA |
| NaturalMergeSort         | 256  | PipeOrgan          |       738.0 ns |      5.59 ns |      2.48 ns |  0.11 |    0.00 |    2 |         - |          NA |
| TimSort                  | 256  | PipeOrgan          |       800.1 ns |      7.42 ns |      3.29 ns |  0.12 |    0.00 |    2 |         - |          NA |
| PowerSort                | 256  | PipeOrgan          |       525.5 ns |     31.86 ns |     14.15 ns |  0.08 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | PipeOrgan          |       511.9 ns |      7.26 ns |      3.22 ns |  0.07 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 256  | PipeOrgan          |     2,103.9 ns |    161.32 ns |     71.63 ns |  0.30 |    0.01 |    4 |         - |          NA |
| Glidesort                | 256  | PipeOrgan          |     1,274.4 ns |     26.16 ns |     11.62 ns |  0.18 |    0.00 |    3 |         - |          NA |
| Driftsort                | 256  | PipeOrgan          |       494.0 ns |     75.88 ns |     39.69 ns |  0.07 |    0.01 |    1 |         - |          NA |
| FlatStableSort           | 256  | PipeOrgan          |     2,144.0 ns |     15.79 ns |      7.01 ns |  0.31 |    0.01 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **ManyDuplicates**     |     **8,597.9 ns** |    **272.57 ns** |    **142.56 ns** |  **1.00** |    **0.02** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | ManyDuplicates     |     8,149.9 ns |    173.86 ns |     77.19 ns |  0.95 |    0.02 |    4 |         - |          NA |
| BottomupMergeSort        | 256  | ManyDuplicates     |     4,803.7 ns |    364.51 ns |    190.65 ns |  0.56 |    0.02 |    2 |         - |          NA |
| StdStableSort            | 256  | ManyDuplicates     |     3,301.1 ns |    781.91 ns |    347.17 ns |  0.38 |    0.04 |    2 |         - |          NA |
| RotateMergeSort          | 256  | ManyDuplicates     |     9,585.7 ns |    249.12 ns |    110.61 ns |  1.12 |    0.02 |    4 |         - |          NA |
| RotateMergeSortRecursive | 256  | ManyDuplicates     |    11,642.8 ns |    352.95 ns |    184.60 ns |  1.35 |    0.03 |    5 |         - |          NA |
| SymMergeSort             | 256  | ManyDuplicates     |     6,430.1 ns |    257.50 ns |    134.68 ns |  0.75 |    0.02 |    3 |         - |          NA |
| BlockMergeSort           | 256  | ManyDuplicates     |     5,139.4 ns |    457.21 ns |    203.00 ns |  0.60 |    0.02 |    2 |         - |          NA |
| NaturalMergeSort         | 256  | ManyDuplicates     |     4,999.4 ns |    384.75 ns |    170.83 ns |  0.58 |    0.02 |    2 |         - |          NA |
| TimSort                  | 256  | ManyDuplicates     |     3,967.7 ns |    329.58 ns |    172.37 ns |  0.46 |    0.02 |    2 |         - |          NA |
| PowerSort                | 256  | ManyDuplicates     |     2,288.3 ns |     27.06 ns |     12.02 ns |  0.27 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | ManyDuplicates     |     3,906.4 ns |    303.26 ns |    134.65 ns |  0.45 |    0.02 |    2 |         - |          NA |
| SpinSort                 | 256  | ManyDuplicates     |     2,350.9 ns |    256.06 ns |    133.92 ns |  0.27 |    0.02 |    1 |         - |          NA |
| Glidesort                | 256  | ManyDuplicates     |     3,633.8 ns |    349.24 ns |    182.66 ns |  0.42 |    0.02 |    2 |         - |          NA |
| Driftsort                | 256  | ManyDuplicates     |     4,600.9 ns |    325.44 ns |    170.21 ns |  0.54 |    0.02 |    2 |         - |          NA |
| FlatStableSort           | 256  | ManyDuplicates     |     2,629.5 ns |    524.17 ns |    274.15 ns |  0.31 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **Random**             |    **37,903.0 ns** |  **1,016.26 ns** |    **451.22 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Random             |    38,047.9 ns |    767.93 ns |    401.64 ns |  1.00 |    0.01 |    2 |         - |          NA |
| BottomupMergeSort        | 1024 | Random             |    21,345.1 ns |    507.85 ns |    265.61 ns |  0.56 |    0.01 |    1 |         - |          NA |
| StdStableSort            | 1024 | Random             |    13,937.0 ns |    722.13 ns |    320.63 ns |  0.37 |    0.01 |    1 |         - |          NA |
| RotateMergeSort          | 1024 | Random             |    63,609.0 ns |  1,861.81 ns |    973.76 ns |  1.68 |    0.03 |    3 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Random             |    75,856.4 ns |  1,827.92 ns |    956.04 ns |  2.00 |    0.03 |    3 |         - |          NA |
| SymMergeSort             | 1024 | Random             |    42,807.2 ns |    640.54 ns |    335.01 ns |  1.13 |    0.02 |    2 |         - |          NA |
| BlockMergeSort           | 1024 | Random             |    25,826.4 ns |    606.94 ns |    317.44 ns |  0.68 |    0.01 |    1 |         - |          NA |
| NaturalMergeSort         | 1024 | Random             |    24,545.4 ns |    355.58 ns |    157.88 ns |  0.65 |    0.01 |    1 |         - |          NA |
| TimSort                  | 1024 | Random             |    19,596.9 ns |    629.41 ns |    329.20 ns |  0.52 |    0.01 |    1 |         - |          NA |
| PowerSort                | 1024 | Random             |    12,551.2 ns |    565.23 ns |    295.63 ns |  0.33 |    0.01 |    1 |         - |          NA |
| ShiftSort                | 1024 | Random             |    18,401.6 ns |    158.72 ns |     56.60 ns |  0.49 |    0.01 |    1 |         - |          NA |
| SpinSort                 | 1024 | Random             |    12,512.0 ns |  1,177.10 ns |    615.64 ns |  0.33 |    0.02 |    1 |         - |          NA |
| Glidesort                | 1024 | Random             |    16,921.1 ns |    348.51 ns |    182.28 ns |  0.45 |    0.01 |    1 |         - |          NA |
| Driftsort                | 1024 | Random             |    21,247.2 ns |    187.75 ns |     98.20 ns |  0.56 |    0.01 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Random             |    14,304.9 ns |  1,095.41 ns |    572.92 ns |  0.38 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **SingleElementMoved** |    **18,993.3 ns** |    **401.33 ns** |    **209.91 ns** |  **1.00** |    **0.01** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | SingleElementMoved |    23,533.6 ns |  5,533.73 ns |  2,457.01 ns |  1.24 |    0.12 |    8 |         - |          NA |
| BottomupMergeSort        | 1024 | SingleElementMoved |     6,586.5 ns |    467.33 ns |    244.42 ns |  0.35 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 1024 | SingleElementMoved |     3,727.9 ns |     20.37 ns |      9.05 ns |  0.20 |    0.00 |    4 |         - |          NA |
| RotateMergeSort          | 1024 | SingleElementMoved |     2,022.9 ns |      5.14 ns |      2.28 ns |  0.11 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 1024 | SingleElementMoved |     2,142.2 ns |     14.05 ns |      6.24 ns |  0.11 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 1024 | SingleElementMoved |     1,818.1 ns |     11.93 ns |      5.30 ns |  0.10 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 1024 | SingleElementMoved |    14,496.4 ns |    683.70 ns |    303.57 ns |  0.76 |    0.02 |    7 |         - |          NA |
| NaturalMergeSort         | 1024 | SingleElementMoved |     2,230.4 ns |      9.10 ns |      4.04 ns |  0.12 |    0.00 |    2 |         - |          NA |
| TimSort                  | 1024 | SingleElementMoved |       879.9 ns |     60.80 ns |     31.80 ns |  0.05 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | SingleElementMoved |     1,516.5 ns |      4.66 ns |      2.07 ns |  0.08 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 1024 | SingleElementMoved |     1,461.8 ns |      4.72 ns |      1.68 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 1024 | SingleElementMoved |     4,557.8 ns |     35.61 ns |     12.70 ns |  0.24 |    0.00 |    5 |         - |          NA |
| Glidesort                | 1024 | SingleElementMoved |     3,149.0 ns |    293.89 ns |    153.71 ns |  0.17 |    0.01 |    3 |         - |          NA |
| Driftsort                | 1024 | SingleElementMoved |     1,345.6 ns |     11.47 ns |      5.09 ns |  0.07 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 1024 | SingleElementMoved |     5,911.8 ns |    286.54 ns |    149.87 ns |  0.31 |    0.01 |    6 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **Sorted**             |    **17,108.9 ns** |     **69.41 ns** |     **30.82 ns** |  **1.00** |    **0.00** |    **9** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Sorted             |    21,217.3 ns |    199.21 ns |    104.19 ns |  1.24 |    0.01 |   10 |         - |          NA |
| BottomupMergeSort        | 1024 | Sorted             |     4,971.0 ns |    323.63 ns |    143.69 ns |  0.29 |    0.01 |    7 |         - |          NA |
| StdStableSort            | 1024 | Sorted             |     3,508.2 ns |     10.97 ns |      3.91 ns |  0.21 |    0.00 |    6 |         - |          NA |
| RotateMergeSort          | 1024 | Sorted             |     1,127.8 ns |      6.85 ns |      3.04 ns |  0.07 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Sorted             |     1,765.1 ns |      4.05 ns |      1.44 ns |  0.10 |    0.00 |    5 |         - |          NA |
| SymMergeSort             | 1024 | Sorted             |     1,463.5 ns |     15.52 ns |      6.89 ns |  0.09 |    0.00 |    4 |         - |          NA |
| BlockMergeSort           | 1024 | Sorted             |    11,666.8 ns |    284.28 ns |    148.68 ns |  0.68 |    0.01 |    8 |         - |          NA |
| NaturalMergeSort         | 1024 | Sorted             |       795.1 ns |      2.68 ns |      1.40 ns |  0.05 |    0.00 |    2 |         - |          NA |
| TimSort                  | 1024 | Sorted             |       601.2 ns |     17.62 ns |      7.82 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Sorted             |       585.2 ns |      2.28 ns |      0.81 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Sorted             |       619.9 ns |      4.52 ns |      2.00 ns |  0.04 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 1024 | Sorted             |       525.4 ns |      3.10 ns |      1.38 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Sorted             |       567.4 ns |     10.92 ns |      5.71 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | Sorted             |       648.5 ns |      5.67 ns |      2.02 ns |  0.04 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Sorted             |     5,271.8 ns |    347.85 ns |    181.93 ns |  0.31 |    0.01 |    7 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **Reversed**           |    **37,922.1 ns** |    **500.82 ns** |    **261.94 ns** |  **1.00** |    **0.01** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Reversed           |    34,620.6 ns |    591.21 ns |    309.21 ns |  0.91 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 1024 | Reversed           |    21,278.6 ns |  1,095.44 ns |    572.94 ns |  0.56 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 1024 | Reversed           |    26,827.8 ns |    274.21 ns |    143.42 ns |  0.71 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 1024 | Reversed           |     9,153.7 ns |    343.23 ns |    179.52 ns |  0.24 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Reversed           |    10,725.8 ns |    395.76 ns |    206.99 ns |  0.28 |    0.01 |    2 |         - |          NA |
| SymMergeSort             | 1024 | Reversed           |     9,261.7 ns |    242.38 ns |    126.77 ns |  0.24 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 1024 | Reversed           |    16,120.5 ns |    141.97 ns |     63.04 ns |  0.43 |    0.00 |    3 |         - |          NA |
| NaturalMergeSort         | 1024 | Reversed           |     1,158.8 ns |      4.94 ns |      1.76 ns |  0.03 |    0.00 |    1 |         - |          NA |
| TimSort                  | 1024 | Reversed           |       912.8 ns |      6.08 ns |      2.70 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Reversed           |       931.7 ns |     65.61 ns |     29.13 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Reversed           |       923.2 ns |      2.40 ns |      1.07 ns |  0.02 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 1024 | Reversed           |       967.6 ns |     10.58 ns |      5.54 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Reversed           |       965.5 ns |      5.02 ns |      2.23 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | Reversed           |       965.8 ns |      3.44 ns |      1.22 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Reversed           |    11,968.5 ns |    134.50 ns |     59.72 ns |  0.32 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **PipeOrgan**          |    **28,464.9 ns** |    **775.14 ns** |    **344.17 ns** |  **1.00** |    **0.02** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | PipeOrgan          |    28,494.5 ns |    494.06 ns |    219.37 ns |  1.00 |    0.01 |    8 |         - |          NA |
| BottomupMergeSort        | 1024 | PipeOrgan          |    13,781.6 ns |    659.83 ns |    345.11 ns |  0.48 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 1024 | PipeOrgan          |    17,159.5 ns |  1,307.78 ns |    683.99 ns |  0.60 |    0.02 |    6 |         - |          NA |
| RotateMergeSort          | 1024 | PipeOrgan          |    18,450.1 ns |    232.88 ns |    121.80 ns |  0.65 |    0.01 |    6 |         - |          NA |
| RotateMergeSortRecursive | 1024 | PipeOrgan          |    23,130.6 ns |     55.40 ns |     24.60 ns |  0.81 |    0.01 |    7 |         - |          NA |
| SymMergeSort             | 1024 | PipeOrgan          |    11,654.7 ns |    452.80 ns |    236.82 ns |  0.41 |    0.01 |    6 |         - |          NA |
| BlockMergeSort           | 1024 | PipeOrgan          |    16,297.1 ns |     61.49 ns |     27.30 ns |  0.57 |    0.01 |    6 |         - |          NA |
| NaturalMergeSort         | 1024 | PipeOrgan          |     2,674.9 ns |     50.31 ns |     22.34 ns |  0.09 |    0.00 |    3 |         - |          NA |
| TimSort                  | 1024 | PipeOrgan          |     2,870.9 ns |     16.03 ns |      7.12 ns |  0.10 |    0.00 |    3 |         - |          NA |
| PowerSort                | 1024 | PipeOrgan          |     2,035.1 ns |    444.65 ns |    232.56 ns |  0.07 |    0.01 |    2 |         - |          NA |
| ShiftSort                | 1024 | PipeOrgan          |     1,927.0 ns |      9.19 ns |      4.08 ns |  0.07 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 1024 | PipeOrgan          |     8,919.8 ns |    554.00 ns |    245.98 ns |  0.31 |    0.01 |    5 |         - |          NA |
| Glidesort                | 1024 | PipeOrgan          |     4,918.5 ns |    329.02 ns |    172.09 ns |  0.17 |    0.01 |    4 |         - |          NA |
| Driftsort                | 1024 | PipeOrgan          |     1,539.6 ns |      8.26 ns |      4.32 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | PipeOrgan          |     9,422.8 ns |    207.08 ns |     91.95 ns |  0.33 |    0.00 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **ManyDuplicates**     |    **35,702.0 ns** |    **549.70 ns** |    **287.50 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | ManyDuplicates     |    35,383.7 ns |    783.29 ns |    347.79 ns |  0.99 |    0.01 |    3 |         - |          NA |
| BottomupMergeSort        | 1024 | ManyDuplicates     |    20,503.2 ns |  1,088.95 ns |    569.54 ns |  0.57 |    0.02 |    2 |         - |          NA |
| StdStableSort            | 1024 | ManyDuplicates     |    12,901.9 ns |    391.80 ns |    173.96 ns |  0.36 |    0.01 |    1 |         - |          NA |
| RotateMergeSort          | 1024 | ManyDuplicates     |    50,393.8 ns |  1,778.16 ns |    930.01 ns |  1.41 |    0.03 |    4 |         - |          NA |
| RotateMergeSortRecursive | 1024 | ManyDuplicates     |    58,099.3 ns |    514.63 ns |    269.16 ns |  1.63 |    0.01 |    4 |         - |          NA |
| SymMergeSort             | 1024 | ManyDuplicates     |    37,188.7 ns |  1,048.93 ns |    465.73 ns |  1.04 |    0.01 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | ManyDuplicates     |    26,690.1 ns |    638.63 ns |    334.02 ns |  0.75 |    0.01 |    2 |         - |          NA |
| NaturalMergeSort         | 1024 | ManyDuplicates     |    23,921.1 ns |    858.61 ns |    449.07 ns |  0.67 |    0.01 |    2 |         - |          NA |
| TimSort                  | 1024 | ManyDuplicates     |    18,921.9 ns |    435.52 ns |    227.78 ns |  0.53 |    0.01 |    2 |         - |          NA |
| PowerSort                | 1024 | ManyDuplicates     |    11,681.1 ns |    311.88 ns |    138.47 ns |  0.33 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | ManyDuplicates     |    18,056.1 ns |    670.17 ns |    297.56 ns |  0.51 |    0.01 |    2 |         - |          NA |
| SpinSort                 | 1024 | ManyDuplicates     |    11,516.7 ns |    430.25 ns |    191.03 ns |  0.32 |    0.01 |    1 |         - |          NA |
| Glidesort                | 1024 | ManyDuplicates     |    16,152.1 ns |    227.03 ns |    100.80 ns |  0.45 |    0.00 |    2 |         - |          NA |
| Driftsort                | 1024 | ManyDuplicates     |    17,381.6 ns |    155.61 ns |     81.39 ns |  0.49 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 1024 | ManyDuplicates     |    11,780.0 ns |    268.74 ns |     95.84 ns |  0.33 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **Random**             |   **182,783.7 ns** | **28,461.40 ns** | **14,885.86 ns** |  **1.01** |    **0.11** |    **2** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Random             |   166,782.0 ns |  1,566.87 ns |    695.70 ns |  0.92 |    0.07 |    2 |         - |          NA |
| BottomupMergeSort        | 4096 | Random             |    98,000.0 ns |  3,791.93 ns |  1,683.64 ns |  0.54 |    0.04 |    1 |         - |          NA |
| StdStableSort            | 4096 | Random             |    78,795.4 ns |  6,287.27 ns |  3,288.37 ns |  0.43 |    0.04 |    1 |         - |          NA |
| RotateMergeSort          | 4096 | Random             |   627,429.8 ns | 14,951.67 ns |  7,820.01 ns |  3.45 |    0.26 |    4 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Random             |   675,277.2 ns |  6,871.18 ns |  3,593.76 ns |  3.72 |    0.28 |    4 |         - |          NA |
| SymMergeSort             | 4096 | Random             |   417,866.2 ns |  4,451.09 ns |  2,328.00 ns |  2.30 |    0.17 |    3 |         - |          NA |
| BlockMergeSort           | 4096 | Random             |   149,217.6 ns | 11,975.36 ns |  6,263.34 ns |  0.82 |    0.07 |    2 |         - |          NA |
| NaturalMergeSort         | 4096 | Random             |   141,892.9 ns |  9,399.21 ns |  4,915.97 ns |  0.78 |    0.06 |    2 |         - |          NA |
| TimSort                  | 4096 | Random             |   101,864.5 ns | 11,062.95 ns |  5,786.14 ns |  0.56 |    0.05 |    1 |         - |          NA |
| PowerSort                | 4096 | Random             |    64,891.5 ns |  2,309.64 ns |  1,025.49 ns |  0.36 |    0.03 |    1 |         - |          NA |
| ShiftSort                | 4096 | Random             |    97,680.3 ns | 13,393.37 ns |  7,004.99 ns |  0.54 |    0.05 |    1 |         - |          NA |
| SpinSort                 | 4096 | Random             |    61,782.6 ns |  2,380.12 ns |  1,056.79 ns |  0.34 |    0.03 |    1 |         - |          NA |
| Glidesort                | 4096 | Random             |    82,648.2 ns |  1,665.75 ns |    871.22 ns |  0.45 |    0.03 |    1 |         - |          NA |
| Driftsort                | 4096 | Random             |    98,171.5 ns |  2,023.13 ns |  1,058.14 ns |  0.54 |    0.04 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Random             |    69,247.5 ns |  1,996.14 ns |  1,044.02 ns |  0.38 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **SingleElementMoved** |    **74,365.8 ns** |    **537.96 ns** |    **238.86 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | SingleElementMoved |    90,412.5 ns |  1,331.51 ns |    696.41 ns |  1.22 |    0.01 |    9 |         - |          NA |
| BottomupMergeSort        | 4096 | SingleElementMoved |    26,811.6 ns |  1,128.99 ns |    590.48 ns |  0.36 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 4096 | SingleElementMoved |    18,163.5 ns |    188.61 ns |     67.26 ns |  0.24 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 4096 | SingleElementMoved |     7,682.8 ns |    370.50 ns |    164.50 ns |  0.10 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 4096 | SingleElementMoved |     7,997.3 ns |    405.15 ns |    179.89 ns |  0.11 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 4096 | SingleElementMoved |     7,106.5 ns |    296.79 ns |    131.78 ns |  0.10 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 4096 | SingleElementMoved |    58,034.4 ns |  1,167.13 ns |    610.43 ns |  0.78 |    0.01 |    7 |         - |          NA |
| NaturalMergeSort         | 4096 | SingleElementMoved |     7,515.4 ns |    411.78 ns |    182.83 ns |  0.10 |    0.00 |    3 |         - |          NA |
| TimSort                  | 4096 | SingleElementMoved |     3,284.0 ns |     63.67 ns |     28.27 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | SingleElementMoved |     5,773.8 ns |     23.47 ns |      8.37 ns |  0.08 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 4096 | SingleElementMoved |     5,697.4 ns |    242.22 ns |    126.69 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 4096 | SingleElementMoved |    14,136.9 ns |    339.74 ns |    150.85 ns |  0.19 |    0.00 |    4 |         - |          NA |
| Glidesort                | 4096 | SingleElementMoved |    11,961.6 ns |    244.20 ns |    127.72 ns |  0.16 |    0.00 |    4 |         - |          NA |
| Driftsort                | 4096 | SingleElementMoved |     5,227.3 ns |    399.36 ns |    177.32 ns |  0.07 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 4096 | SingleElementMoved |    24,879.6 ns |    412.50 ns |    183.15 ns |  0.33 |    0.00 |    6 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **Sorted**             |    **68,894.3 ns** |    **524.90 ns** |    **274.53 ns** |  **1.00** |    **0.01** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Sorted             |    85,339.9 ns |    896.42 ns |    468.85 ns |  1.24 |    0.01 |    9 |         - |          NA |
| BottomupMergeSort        | 4096 | Sorted             |    20,293.2 ns |    689.31 ns |    306.06 ns |  0.29 |    0.00 |    6 |         - |          NA |
| StdStableSort            | 4096 | Sorted             |    18,275.5 ns |    391.37 ns |    173.77 ns |  0.27 |    0.00 |    6 |         - |          NA |
| RotateMergeSort          | 4096 | Sorted             |     4,677.1 ns |     17.16 ns |      6.12 ns |  0.07 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Sorted             |     7,230.4 ns |    263.47 ns |    137.80 ns |  0.10 |    0.00 |    5 |         - |          NA |
| SymMergeSort             | 4096 | Sorted             |     5,853.3 ns |    304.58 ns |    135.24 ns |  0.08 |    0.00 |    4 |         - |          NA |
| BlockMergeSort           | 4096 | Sorted             |    47,423.0 ns |    470.53 ns |    246.10 ns |  0.69 |    0.00 |    7 |         - |          NA |
| NaturalMergeSort         | 4096 | Sorted             |     2,943.0 ns |      3.92 ns |      1.40 ns |  0.04 |    0.00 |    2 |         - |          NA |
| TimSort                  | 4096 | Sorted             |     2,545.0 ns |    397.89 ns |    208.10 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | Sorted             |     2,261.0 ns |     28.53 ns |     14.92 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | Sorted             |     2,508.2 ns |    284.17 ns |    148.62 ns |  0.04 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 4096 | Sorted             |     2,014.0 ns |      6.46 ns |      3.38 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 4096 | Sorted             |     2,291.9 ns |    343.32 ns |    179.56 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | Sorted             |     2,348.4 ns |     26.98 ns |     11.98 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Sorted             |    20,517.3 ns |    524.19 ns |    274.16 ns |  0.30 |    0.00 |    6 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **Reversed**           |   **156,652.5 ns** |  **2,314.56 ns** |  **1,210.56 ns** |  **1.00** |    **0.01** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Reversed           |   142,647.0 ns |  1,094.96 ns |    486.17 ns |  0.91 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 4096 | Reversed           |    89,785.1 ns |  1,251.63 ns |    654.62 ns |  0.57 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 4096 | Reversed           |   111,986.2 ns |  1,090.38 ns |    570.29 ns |  0.71 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 4096 | Reversed           |    42,868.9 ns |    770.78 ns |    342.23 ns |  0.27 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Reversed           |    48,602.8 ns |    535.15 ns |    237.61 ns |  0.31 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 4096 | Reversed           |    39,398.5 ns |    880.57 ns |    460.55 ns |  0.25 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 4096 | Reversed           |    74,140.4 ns |  1,873.55 ns |    979.90 ns |  0.47 |    0.01 |    3 |         - |          NA |
| NaturalMergeSort         | 4096 | Reversed           |     4,534.2 ns |    351.18 ns |    125.24 ns |  0.03 |    0.00 |    1 |         - |          NA |
| TimSort                  | 4096 | Reversed           |     3,657.7 ns |    301.82 ns |    157.86 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | Reversed           |     3,665.1 ns |     63.03 ns |     27.99 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | Reversed           |     3,592.5 ns |    216.73 ns |     77.29 ns |  0.02 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 4096 | Reversed           |     3,974.2 ns |    422.52 ns |    220.98 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 4096 | Reversed           |     3,618.0 ns |     11.98 ns |      5.32 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | Reversed           |     3,787.1 ns |    363.67 ns |    190.21 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Reversed           |    47,873.5 ns |    601.10 ns |    314.39 ns |  0.31 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **PipeOrgan**          |   **116,305.5 ns** |  **1,071.11 ns** |    **560.21 ns** |  **1.00** |    **0.01** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | PipeOrgan          |   117,942.4 ns |  2,189.54 ns |  1,145.17 ns |  1.01 |    0.01 |    7 |         - |          NA |
| BottomupMergeSort        | 4096 | PipeOrgan          |    57,891.4 ns |  1,519.26 ns |    794.60 ns |  0.50 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 4096 | PipeOrgan          |    66,017.8 ns |  1,072.89 ns |    561.14 ns |  0.57 |    0.01 |    6 |         - |          NA |
| RotateMergeSort          | 4096 | PipeOrgan          |    80,052.0 ns |    574.79 ns |    300.63 ns |  0.69 |    0.00 |    6 |         - |          NA |
| RotateMergeSortRecursive | 4096 | PipeOrgan          |    99,554.0 ns |    850.92 ns |    445.05 ns |  0.86 |    0.01 |    7 |         - |          NA |
| SymMergeSort             | 4096 | PipeOrgan          |    49,671.2 ns |  1,320.84 ns |    690.83 ns |  0.43 |    0.01 |    6 |         - |          NA |
| BlockMergeSort           | 4096 | PipeOrgan          |    68,542.6 ns |    428.27 ns |    223.99 ns |  0.59 |    0.00 |    6 |         - |          NA |
| NaturalMergeSort         | 4096 | PipeOrgan          |    10,539.4 ns |    236.20 ns |    123.54 ns |  0.09 |    0.00 |    3 |         - |          NA |
| TimSort                  | 4096 | PipeOrgan          |    11,261.4 ns |    244.47 ns |    127.86 ns |  0.10 |    0.00 |    3 |         - |          NA |
| PowerSort                | 4096 | PipeOrgan          |     7,195.3 ns |    539.41 ns |    239.50 ns |  0.06 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 4096 | PipeOrgan          |     7,613.9 ns |    750.55 ns |    333.25 ns |  0.07 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 4096 | PipeOrgan          |     8,777.6 ns |    944.06 ns |    419.17 ns |  0.08 |    0.00 |    2 |         - |          NA |
| Glidesort                | 4096 | PipeOrgan          |    19,030.4 ns |    307.05 ns |    136.33 ns |  0.16 |    0.00 |    4 |         - |          NA |
| Driftsort                | 4096 | PipeOrgan          |     5,836.5 ns |    260.02 ns |    135.99 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | PipeOrgan          |    37,368.0 ns |    530.95 ns |    189.34 ns |  0.32 |    0.00 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **ManyDuplicates**     |   **156,508.3 ns** |  **4,142.42 ns** |  **2,166.56 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | ManyDuplicates     |   151,874.6 ns |  2,728.21 ns |  1,426.91 ns |  0.97 |    0.02 |    3 |         - |          NA |
| BottomupMergeSort        | 4096 | ManyDuplicates     |    90,311.0 ns |  1,316.56 ns |    584.56 ns |  0.58 |    0.01 |    2 |         - |          NA |
| StdStableSort            | 4096 | ManyDuplicates     |    72,918.7 ns |  3,940.92 ns |  2,061.18 ns |  0.47 |    0.01 |    2 |         - |          NA |
| RotateMergeSort          | 4096 | ManyDuplicates     |   326,169.1 ns | 42,335.19 ns | 22,142.12 ns |  2.08 |    0.14 |    5 |         - |          NA |
| RotateMergeSortRecursive | 4096 | ManyDuplicates     |   294,243.1 ns | 17,450.68 ns |  7,748.21 ns |  1.88 |    0.05 |    5 |         - |          NA |
| SymMergeSort             | 4096 | ManyDuplicates     |   201,440.6 ns | 12,819.27 ns |  6,704.72 ns |  1.29 |    0.04 |    4 |         - |          NA |
| BlockMergeSort           | 4096 | ManyDuplicates     |   135,688.4 ns |  4,271.59 ns |  2,234.12 ns |  0.87 |    0.02 |    3 |         - |          NA |
| NaturalMergeSort         | 4096 | ManyDuplicates     |   117,385.8 ns |  4,636.55 ns |  2,425.00 ns |  0.75 |    0.02 |    3 |         - |          NA |
| TimSort                  | 4096 | ManyDuplicates     |    81,581.9 ns |    625.54 ns |    277.75 ns |  0.52 |    0.01 |    2 |         - |          NA |
| PowerSort                | 4096 | ManyDuplicates     |    57,994.1 ns |  3,738.56 ns |  1,955.34 ns |  0.37 |    0.01 |    1 |         - |          NA |
| ShiftSort                | 4096 | ManyDuplicates     |    84,062.4 ns |  1,543.73 ns |    685.42 ns |  0.54 |    0.01 |    2 |         - |          NA |
| SpinSort                 | 4096 | ManyDuplicates     |    54,225.4 ns |  2,269.51 ns |  1,007.67 ns |  0.35 |    0.01 |    1 |         - |          NA |
| Glidesort                | 4096 | ManyDuplicates     |    47,005.1 ns |  1,184.87 ns |    526.09 ns |  0.30 |    0.01 |    1 |         - |          NA |
| Driftsort                | 4096 | ManyDuplicates     |    43,626.5 ns |    773.90 ns |    343.61 ns |  0.28 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | ManyDuplicates     |    58,941.7 ns |  2,323.65 ns |  1,215.32 ns |  0.38 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **Random**             |   **757,433.4 ns** |  **5,389.62 ns** |  **2,818.88 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Random             |   663,201.9 ns |  2,985.86 ns |  1,561.66 ns |  0.88 |    0.00 |    2 |         - |          NA |
| BottomupMergeSort        | 8192 | Random             |   531,236.3 ns |  4,331.69 ns |  1,923.30 ns |  0.70 |    0.00 |    2 |         - |          NA |
| StdStableSort            | 8192 | Random             |   389,022.4 ns | 14,910.56 ns |  7,798.51 ns |  0.51 |    0.01 |    2 |         - |          NA |
| RotateMergeSort          | 8192 | Random             | 1,544,131.0 ns | 12,605.72 ns |  6,593.04 ns |  2.04 |    0.01 |    4 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Random             | 1,673,692.0 ns |  2,611.07 ns |  1,365.64 ns |  2.21 |    0.01 |    4 |         - |          NA |
| SymMergeSort             | 8192 | Random             | 1,133,963.5 ns |  3,356.07 ns |  1,755.29 ns |  1.50 |    0.01 |    3 |         - |          NA |
| BlockMergeSort           | 8192 | Random             |   657,830.7 ns |  2,757.05 ns |  1,441.99 ns |  0.87 |    0.00 |    2 |         - |          NA |
| NaturalMergeSort         | 8192 | Random             |   684,662.7 ns |  3,651.34 ns |  1,621.22 ns |  0.90 |    0.00 |    2 |         - |          NA |
| TimSort                  | 8192 | Random             |   582,628.7 ns |  4,768.63 ns |  2,494.09 ns |  0.77 |    0.00 |    2 |         - |          NA |
| PowerSort                | 8192 | Random             |   472,667.3 ns | 49,765.95 ns | 26,028.55 ns |  0.62 |    0.03 |    2 |         - |          NA |
| ShiftSort                | 8192 | Random             |   634,574.7 ns | 90,439.66 ns | 47,301.69 ns |  0.84 |    0.06 |    2 |         - |          NA |
| SpinSort                 | 8192 | Random             |   364,334.1 ns |  3,706.15 ns |  1,645.55 ns |  0.48 |    0.00 |    2 |         - |          NA |
| Glidesort                | 8192 | Random             |   217,602.7 ns | 93,196.58 ns | 48,743.61 ns |  0.29 |    0.06 |    1 |         - |          NA |
| Driftsort                | 8192 | Random             |   213,319.7 ns |  2,370.68 ns |  1,239.91 ns |  0.28 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Random             |   406,551.1 ns |  5,357.02 ns |  2,378.55 ns |  0.54 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **SingleElementMoved** |   **166,215.3 ns** | **29,726.17 ns** | **15,547.36 ns** |  **1.01** |    **0.13** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | SingleElementMoved |   186,016.2 ns |  3,634.64 ns |  1,900.99 ns |  1.13 |    0.10 |    8 |         - |          NA |
| BottomupMergeSort        | 8192 | SingleElementMoved |    59,551.6 ns |  6,088.09 ns |  2,703.15 ns |  0.36 |    0.04 |    6 |         - |          NA |
| StdStableSort            | 8192 | SingleElementMoved |    35,721.3 ns |  2,280.89 ns |  1,192.95 ns |  0.22 |    0.02 |    4 |         - |          NA |
| RotateMergeSort          | 8192 | SingleElementMoved |    15,219.0 ns |    329.43 ns |    146.27 ns |  0.09 |    0.01 |    2 |         - |          NA |
| RotateMergeSortRecursive | 8192 | SingleElementMoved |    15,856.3 ns |     80.59 ns |     35.78 ns |  0.10 |    0.01 |    2 |         - |          NA |
| SymMergeSort             | 8192 | SingleElementMoved |    13,968.2 ns |    259.00 ns |    115.00 ns |  0.08 |    0.01 |    2 |         - |          NA |
| BlockMergeSort           | 8192 | SingleElementMoved |   114,442.7 ns |    694.88 ns |    308.53 ns |  0.69 |    0.06 |    7 |         - |          NA |
| NaturalMergeSort         | 8192 | SingleElementMoved |    17,965.2 ns |  1,295.76 ns |    677.71 ns |  0.11 |    0.01 |    2 |         - |          NA |
| TimSort                  | 8192 | SingleElementMoved |     6,207.2 ns |    385.61 ns |    137.51 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | SingleElementMoved |    11,718.9 ns |    205.78 ns |     91.37 ns |  0.07 |    0.01 |    2 |         - |          NA |
| ShiftSort                | 8192 | SingleElementMoved |    11,461.6 ns |    179.58 ns |     64.04 ns |  0.07 |    0.01 |    2 |         - |          NA |
| SpinSort                 | 8192 | SingleElementMoved |    24,199.7 ns |  1,016.47 ns |    531.63 ns |  0.15 |    0.01 |    3 |         - |          NA |
| Glidesort                | 8192 | SingleElementMoved |    23,806.4 ns |    634.27 ns |    281.62 ns |  0.14 |    0.01 |    3 |         - |          NA |
| Driftsort                | 8192 | SingleElementMoved |    10,281.5 ns |    253.87 ns |    132.78 ns |  0.06 |    0.01 |    2 |         - |          NA |
| FlatStableSort           | 8192 | SingleElementMoved |    48,919.9 ns |    723.20 ns |    378.25 ns |  0.30 |    0.03 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **Sorted**             |   **137,431.4 ns** |    **792.99 ns** |    **352.09 ns** |  **1.00** |    **0.00** |    **9** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Sorted             |   171,020.0 ns |    889.73 ns |    465.35 ns |  1.24 |    0.00 |   10 |         - |          NA |
| BottomupMergeSort        | 8192 | Sorted             |    44,735.8 ns |    845.88 ns |    442.41 ns |  0.33 |    0.00 |    7 |         - |          NA |
| StdStableSort            | 8192 | Sorted             |    34,981.5 ns |    881.19 ns |    460.88 ns |  0.25 |    0.00 |    6 |         - |          NA |
| RotateMergeSort          | 8192 | Sorted             |     9,354.8 ns |  1,392.29 ns |    618.18 ns |  0.07 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Sorted             |    14,517.3 ns |    100.79 ns |     44.75 ns |  0.11 |    0.00 |    5 |         - |          NA |
| SymMergeSort             | 8192 | Sorted             |    11,789.2 ns |    396.96 ns |    141.56 ns |  0.09 |    0.00 |    4 |         - |          NA |
| BlockMergeSort           | 8192 | Sorted             |    92,810.8 ns |    302.43 ns |    158.18 ns |  0.68 |    0.00 |    8 |         - |          NA |
| NaturalMergeSort         | 8192 | Sorted             |     6,045.0 ns |    493.60 ns |    258.16 ns |  0.04 |    0.00 |    2 |         - |          NA |
| TimSort                  | 8192 | Sorted             |     4,683.9 ns |     46.01 ns |     16.41 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Sorted             |     4,733.0 ns |    298.29 ns |    132.44 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Sorted             |     4,506.5 ns |    276.47 ns |    144.60 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 8192 | Sorted             |     4,073.3 ns |    256.76 ns |    134.29 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Sorted             |     4,294.3 ns |    532.07 ns |    236.24 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Sorted             |     4,735.9 ns |    328.65 ns |    145.92 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Sorted             |     4,318.9 ns |    222.30 ns |     98.70 ns |  0.03 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **Reversed**           |   **322,006.0 ns** |  **3,021.96 ns** |  **1,580.54 ns** |  **1.00** |    **0.01** |    **5** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Reversed           |   291,049.1 ns |  2,008.60 ns |  1,050.54 ns |  0.90 |    0.01 |    5 |         - |          NA |
| BottomupMergeSort        | 8192 | Reversed           |   187,144.0 ns |  3,514.09 ns |  1,837.94 ns |  0.58 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 8192 | Reversed           |   412,616.5 ns |  1,537.62 ns |    804.20 ns |  1.28 |    0.01 |    6 |         - |          NA |
| RotateMergeSort          | 8192 | Reversed           |    91,874.9 ns |  2,744.05 ns |  1,435.19 ns |  0.29 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Reversed           |   102,919.7 ns |  1,035.58 ns |    541.63 ns |  0.32 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 8192 | Reversed           |    81,506.9 ns |    570.57 ns |    253.34 ns |  0.25 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 8192 | Reversed           |   153,010.9 ns |    314.66 ns |    139.71 ns |  0.48 |    0.00 |    3 |         - |          NA |
| NaturalMergeSort         | 8192 | Reversed           |     9,005.2 ns |    602.08 ns |    267.33 ns |  0.03 |    0.00 |    1 |         - |          NA |
| TimSort                  | 8192 | Reversed           |     7,068.1 ns |     14.03 ns |      5.00 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Reversed           |     7,378.2 ns |    486.50 ns |    254.45 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Reversed           |     6,989.0 ns |    405.30 ns |    211.98 ns |  0.02 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 8192 | Reversed           |     7,796.6 ns |    484.79 ns |    215.25 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Reversed           |     7,270.1 ns |    364.61 ns |    161.89 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Reversed           |     7,300.4 ns |    425.69 ns |    189.01 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Reversed           |     7,775.7 ns |    382.89 ns |    170.01 ns |  0.02 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **PipeOrgan**          |   **234,780.9 ns** |  **2,993.95 ns** |  **1,565.89 ns** |  **1.00** |    **0.01** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | PipeOrgan          |   237,907.1 ns |  2,797.81 ns |  1,463.31 ns |  1.01 |    0.01 |    7 |         - |          NA |
| BottomupMergeSort        | 8192 | PipeOrgan          |   123,844.9 ns |  2,629.36 ns |  1,375.21 ns |  0.53 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 8192 | PipeOrgan          |   225,871.4 ns |  1,345.64 ns |    703.80 ns |  0.96 |    0.01 |    7 |         - |          NA |
| RotateMergeSort          | 8192 | PipeOrgan          |   166,319.8 ns |    801.80 ns |    419.36 ns |  0.71 |    0.00 |    6 |         - |          NA |
| RotateMergeSortRecursive | 8192 | PipeOrgan          |   203,996.4 ns |  1,031.89 ns |    539.70 ns |  0.87 |    0.01 |    7 |         - |          NA |
| SymMergeSort             | 8192 | PipeOrgan          |   101,988.2 ns |  1,512.73 ns |    791.19 ns |  0.43 |    0.00 |    5 |         - |          NA |
| BlockMergeSort           | 8192 | PipeOrgan          |   139,825.4 ns |    877.15 ns |    458.77 ns |  0.60 |    0.00 |    6 |         - |          NA |
| NaturalMergeSort         | 8192 | PipeOrgan          |    21,851.9 ns |  1,546.74 ns |    808.97 ns |  0.09 |    0.00 |    2 |         - |          NA |
| TimSort                  | 8192 | PipeOrgan          |    22,502.5 ns |    277.24 ns |    145.00 ns |  0.10 |    0.00 |    2 |         - |          NA |
| PowerSort                | 8192 | PipeOrgan          |    13,733.3 ns |    349.44 ns |    155.15 ns |  0.06 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | PipeOrgan          |    15,001.0 ns |    221.86 ns |     79.12 ns |  0.06 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 8192 | PipeOrgan          |    18,970.4 ns |  1,850.76 ns |    967.98 ns |  0.08 |    0.00 |    2 |         - |          NA |
| Glidesort                | 8192 | PipeOrgan          |    38,460.2 ns |  1,141.38 ns |    596.96 ns |  0.16 |    0.00 |    3 |         - |          NA |
| Driftsort                | 8192 | PipeOrgan          |    11,543.3 ns |    141.96 ns |     50.62 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | PipeOrgan          |    73,573.1 ns |    838.21 ns |    438.40 ns |  0.31 |    0.00 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **ManyDuplicates**     |   **476,072.4 ns** | **27,010.02 ns** | **14,126.76 ns** |  **1.00** |    **0.04** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | ManyDuplicates     |   480,737.6 ns | 18,254.58 ns |  8,105.15 ns |  1.01 |    0.03 |    6 |         - |          NA |
| BottomupMergeSort        | 8192 | ManyDuplicates     |   302,960.4 ns |  3,558.60 ns |  1,580.04 ns |  0.64 |    0.02 |    4 |         - |          NA |
| StdStableSort            | 8192 | ManyDuplicates     |   234,675.1 ns | 13,489.66 ns |  7,055.35 ns |  0.49 |    0.02 |    3 |         - |          NA |
| RotateMergeSort          | 8192 | ManyDuplicates     |   951,668.4 ns |  9,120.48 ns |  4,770.19 ns |  2.00 |    0.06 |    8 |         - |          NA |
| RotateMergeSortRecursive | 8192 | ManyDuplicates     | 1,026,837.5 ns | 13,733.62 ns |  7,182.95 ns |  2.16 |    0.06 |    8 |         - |          NA |
| SymMergeSort             | 8192 | ManyDuplicates     |   767,120.4 ns |  3,039.41 ns |  1,589.67 ns |  1.61 |    0.05 |    7 |         - |          NA |
| BlockMergeSort           | 8192 | ManyDuplicates     |   548,741.2 ns |  2,436.52 ns |  1,081.83 ns |  1.15 |    0.03 |    6 |         - |          NA |
| NaturalMergeSort         | 8192 | ManyDuplicates     |   501,211.3 ns |  3,875.27 ns |  2,026.84 ns |  1.05 |    0.03 |    6 |         - |          NA |
| TimSort                  | 8192 | ManyDuplicates     |   382,246.1 ns |  7,456.03 ns |  3,899.65 ns |  0.80 |    0.02 |    5 |         - |          NA |
| PowerSort                | 8192 | ManyDuplicates     |   191,792.1 ns | 10,738.69 ns |  5,616.54 ns |  0.40 |    0.02 |    2 |         - |          NA |
| ShiftSort                | 8192 | ManyDuplicates     |   365,881.5 ns |  9,430.49 ns |  4,932.33 ns |  0.77 |    0.02 |    5 |         - |          NA |
| SpinSort                 | 8192 | ManyDuplicates     |   182,509.1 ns |  1,430.58 ns |    635.19 ns |  0.38 |    0.01 |    2 |         - |          NA |
| Glidesort                | 8192 | ManyDuplicates     |    91,358.5 ns |  2,136.18 ns |  1,117.26 ns |  0.19 |    0.01 |    1 |         - |          NA |
| Driftsort                | 8192 | ManyDuplicates     |    82,514.1 ns |    702.49 ns |    367.42 ns |  0.17 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | ManyDuplicates     |   153,363.8 ns |  2,880.81 ns |  1,506.72 ns |  0.32 |    0.01 |    2 |         - |          NA |

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
| **BitonicSort**             | **256**  | **Random**             |    **10,115.6 ns** |    **422.00 ns** |   **220.72 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Random             |    23,285.7 ns |    234.83 ns |   122.82 ns |  2.30 |    0.05 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Random             |    18,718.0 ns |    204.87 ns |   107.15 ns |  1.85 |    0.04 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **SingleElementMoved** |     **9,851.3 ns** |    **353.15 ns** |   **184.70 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | SingleElementMoved |    23,043.9 ns |     63.94 ns |    28.39 ns |  2.34 |    0.04 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | SingleElementMoved |    18,680.2 ns |    120.25 ns |    53.39 ns |  1.90 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **Sorted**             |     **9,499.4 ns** |    **570.08 ns** |   **298.16 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Sorted             |    23,257.0 ns |    159.43 ns |    70.79 ns |  2.45 |    0.07 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Sorted             |    18,745.9 ns |    245.02 ns |   128.15 ns |  1.98 |    0.06 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **Reversed**           |     **9,939.5 ns** |    **539.82 ns** |   **282.34 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Reversed           |    23,191.3 ns |    193.22 ns |    85.79 ns |  2.33 |    0.06 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Reversed           |    18,715.6 ns |    161.30 ns |    71.62 ns |  1.88 |    0.05 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **PipeOrgan**          |     **9,636.2 ns** |    **268.55 ns** |   **140.46 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | PipeOrgan          |    23,279.7 ns |    316.08 ns |   140.34 ns |  2.42 |    0.04 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | PipeOrgan          |    18,696.8 ns |    188.75 ns |    98.72 ns |  1.94 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **ManyDuplicates**     |    **10,098.1 ns** |    **505.31 ns** |   **264.29 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | ManyDuplicates     |    22,816.2 ns |    141.40 ns |    73.95 ns |  2.26 |    0.06 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | ManyDuplicates     |    18,718.7 ns |    243.74 ns |   127.48 ns |  1.85 |    0.05 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Random**             |    **59,654.8 ns** |  **1,977.39 ns** | **1,034.21 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Random             |   118,687.7 ns |    919.91 ns |   408.45 ns |  1.99 |    0.03 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Random             |   115,101.4 ns |    536.79 ns |   238.34 ns |  1.93 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **SingleElementMoved** |    **61,961.5 ns** |  **2,762.51 ns** | **1,444.84 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | SingleElementMoved |   120,116.1 ns |    300.06 ns |   156.94 ns |  1.94 |    0.04 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | SingleElementMoved |   115,018.7 ns |    277.80 ns |   145.29 ns |  1.86 |    0.04 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Sorted**             |    **59,320.9 ns** |  **3,004.19 ns** | **1,571.25 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Sorted             |   119,205.8 ns |  1,093.41 ns |   389.92 ns |  2.01 |    0.05 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Sorted             |   115,265.7 ns |    301.22 ns |   133.75 ns |  1.94 |    0.05 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Reversed**           |    **59,264.6 ns** |  **1,215.10 ns** |   **539.51 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Reversed           |   121,481.3 ns |  4,718.23 ns | 2,094.92 ns |  2.05 |    0.04 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Reversed           |   115,097.0 ns |    202.67 ns |    89.99 ns |  1.94 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **PipeOrgan**          |    **60,763.3 ns** |  **3,276.05 ns** | **1,713.44 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | PipeOrgan          |   120,305.0 ns |    863.41 ns |   451.58 ns |  1.98 |    0.05 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | PipeOrgan          |   115,244.5 ns |    492.42 ns |   257.54 ns |  1.90 |    0.05 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **ManyDuplicates**     |    **60,559.7 ns** |  **2,196.36 ns** | **1,148.74 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | ManyDuplicates     |   117,477.3 ns |  1,096.91 ns |   573.71 ns |  1.94 |    0.04 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | ManyDuplicates     |   115,040.6 ns |    302.24 ns |   158.08 ns |  1.90 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Random**             |   **566,241.8 ns** |  **5,916.58 ns** | **3,094.49 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Random             |   830,257.5 ns |  4,102.79 ns | 2,145.84 ns |  1.47 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Random             |   684,489.9 ns |  1,021.91 ns |   364.42 ns |  1.21 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **SingleElementMoved** |   **343,078.2 ns** |  **5,144.01 ns** | **2,690.41 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | SingleElementMoved |   599,717.8 ns |  1,117.51 ns |   496.18 ns |  1.75 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | SingleElementMoved |   659,049.6 ns |  1,148.90 ns |   600.90 ns |  1.92 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Sorted**             |   **341,374.3 ns** |  **5,360.95 ns** | **2,380.29 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Sorted             |   592,045.3 ns |  1,390.24 ns |   617.27 ns |  1.73 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Sorted             |   658,727.2 ns |    496.93 ns |   220.64 ns |  1.93 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Reversed**           |   **336,876.6 ns** |  **5,265.53 ns** | **2,753.98 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Reversed           |   597,388.9 ns |  2,599.51 ns | 1,154.20 ns |  1.77 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Reversed           |   659,120.2 ns |    685.19 ns |   304.23 ns |  1.96 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **PipeOrgan**          |   **340,807.8 ns** |  **6,667.24 ns** | **3,487.09 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | PipeOrgan          |   598,394.5 ns |  1,075.55 ns |   477.55 ns |  1.76 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | PipeOrgan          |   659,692.7 ns |  1,184.19 ns |   619.35 ns |  1.94 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **ManyDuplicates**     |   **455,819.3 ns** |  **5,969.43 ns** | **3,122.13 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | ManyDuplicates     |   707,801.8 ns |  2,903.57 ns | 1,289.20 ns |  1.55 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | ManyDuplicates     |   661,696.2 ns |  1,345.94 ns |   703.95 ns |  1.45 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **8192** | **Random**             | **1,319,597.9 ns** |  **4,565.83 ns** | **2,388.02 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | Random             | 1,955,923.1 ns |  4,007.51 ns | 1,779.36 ns |  1.48 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | Random             | 1,680,577.9 ns |  2,501.12 ns | 1,308.13 ns |  1.27 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **8192** | **SingleElementMoved** |   **790,071.3 ns** | **11,970.74 ns** | **5,315.08 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | SingleElementMoved | 1,350,554.2 ns |  2,982.18 ns | 1,324.11 ns |  1.71 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | SingleElementMoved | 1,541,177.3 ns |    715.07 ns |   317.49 ns |  1.95 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **8192** | **Sorted**             |   **776,886.0 ns** | **18,592.00 ns** | **8,254.96 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | Sorted             | 1,333,026.7 ns |    589.97 ns |   210.39 ns |  1.72 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | Sorted             | 1,541,569.4 ns |  1,429.68 ns |   634.79 ns |  1.98 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **8192** | **Reversed**           |   **779,374.1 ns** |  **7,383.93 ns** | **3,278.51 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | Reversed           | 1,347,557.5 ns |  3,548.03 ns | 1,855.69 ns |  1.73 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | Reversed           | 1,542,381.3 ns |    685.15 ns |   304.21 ns |  1.98 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **8192** | **PipeOrgan**          |   **788,797.5 ns** |  **7,502.71 ns** | **3,924.06 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | PipeOrgan          | 1,348,755.4 ns |  1,681.37 ns |   746.54 ns |  1.71 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | PipeOrgan          | 1,542,400.4 ns |  1,333.89 ns |   592.26 ns |  1.96 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **8192** | **ManyDuplicates**     | **1,067,176.8 ns** |  **6,332.30 ns** | **3,311.91 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | ManyDuplicates     | 1,683,656.9 ns |  4,352.20 ns | 2,276.28 ns |  1.58 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | ManyDuplicates     | 1,594,565.9 ns |  5,111.73 ns | 2,673.53 ns |  1.49 |    0.00 |    2 |         - |          NA |

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
| **QuickSort**                    | **256**  | **Random**             |     **2,735.0 ns** |    **236.83 ns** |    **105.15 ns** |  **1.00** |    **0.05** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 256  | Random             |     2,467.4 ns |    218.22 ns |    114.13 ns |  0.90 |    0.05 |    1 |         - |          NA |
| QuickSortMedian3             | 256  | Random             |     2,817.2 ns |    126.30 ns |     56.08 ns |  1.03 |    0.04 |    1 |         - |          NA |
| QuickSortMedian9             | 256  | Random             |     3,550.7 ns |    885.38 ns |    463.07 ns |  1.30 |    0.17 |    1 |         - |          NA |
| DualPivotQuickSort           | 256  | Random             |     2,384.4 ns |    124.66 ns |     55.35 ns |  0.87 |    0.04 |    1 |         - |          NA |
| StableQuickSort              | 256  | Random             |    11,426.2 ns |    408.94 ns |    213.88 ns |  4.18 |    0.16 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Random             |     7,371.9 ns |    140.89 ns |     73.69 ns |  2.70 |    0.10 |    2 |         - |          NA |
| DestswapStableQuickSort      | 256  | Random             |     7,080.6 ns |    542.97 ns |    283.99 ns |  2.59 |    0.13 |    2 |         - |          NA |
| IntroSort                    | 256  | Random             |     2,297.0 ns |    284.37 ns |    126.26 ns |  0.84 |    0.05 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | Random             |     1,767.8 ns |     19.76 ns |      7.05 ns |  0.65 |    0.02 |    1 |         - |          NA |
| PDQSort                      | 256  | Random             |     1,797.5 ns |     52.03 ns |     23.10 ns |  0.66 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | Random             |     2,814.1 ns |     91.87 ns |     40.79 ns |  1.03 |    0.04 |    1 |         - |          NA |
| Ipnsort                      | 256  | Random             |     3,658.2 ns |     27.93 ns |     12.40 ns |  1.34 |    0.05 |    1 |         - |          NA |
| StdSort                      | 256  | Random             |     2,879.9 ns |    315.41 ns |    140.05 ns |  1.05 |    0.06 |    1 |         - |          NA |
| BlockQuickSort               | 256  | Random             |     2,859.4 ns |     57.52 ns |     25.54 ns |  1.05 |    0.04 |    1 |         - |          NA |
| DotnetSort                   | 256  | Random             |     1,992.7 ns |     19.25 ns |      6.86 ns |  0.73 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **SingleElementMoved** |     **1,191.7 ns** |     **42.34 ns** |     **22.14 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 256  | SingleElementMoved |     1,037.4 ns |     58.57 ns |     26.01 ns |  0.87 |    0.03 |    1 |         - |          NA |
| QuickSortMedian3             | 256  | SingleElementMoved |     1,766.4 ns |    145.31 ns |     76.00 ns |  1.48 |    0.07 |    2 |         - |          NA |
| QuickSortMedian9             | 256  | SingleElementMoved |     2,203.7 ns |     36.47 ns |     16.19 ns |  1.85 |    0.03 |    2 |         - |          NA |
| DualPivotQuickSort           | 256  | SingleElementMoved |       860.7 ns |     26.83 ns |     11.91 ns |  0.72 |    0.02 |    1 |         - |          NA |
| StableQuickSort              | 256  | SingleElementMoved |     8,756.2 ns |     99.42 ns |     52.00 ns |  7.35 |    0.14 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 256  | SingleElementMoved |     5,605.4 ns |    361.60 ns |    160.55 ns |  4.71 |    0.15 |    5 |         - |          NA |
| DestswapStableQuickSort      | 256  | SingleElementMoved |     4,312.0 ns |    152.47 ns |     54.37 ns |  3.62 |    0.08 |    4 |         - |          NA |
| IntroSort                    | 256  | SingleElementMoved |       918.9 ns |     90.20 ns |     40.05 ns |  0.77 |    0.03 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | SingleElementMoved |     1,114.7 ns |     12.34 ns |      5.48 ns |  0.94 |    0.02 |    1 |         - |          NA |
| PDQSort                      | 256  | SingleElementMoved |     1,177.9 ns |     84.09 ns |     43.98 ns |  0.99 |    0.04 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | SingleElementMoved |     1,417.1 ns |      8.91 ns |      3.95 ns |  1.19 |    0.02 |    1 |         - |          NA |
| Ipnsort                      | 256  | SingleElementMoved |     3,563.8 ns |     15.12 ns |      5.39 ns |  2.99 |    0.05 |    3 |         - |          NA |
| StdSort                      | 256  | SingleElementMoved |     1,824.1 ns |    152.50 ns |     79.76 ns |  1.53 |    0.07 |    2 |         - |          NA |
| BlockQuickSort               | 256  | SingleElementMoved |     1,912.8 ns |    272.33 ns |    142.43 ns |  1.61 |    0.12 |    2 |         - |          NA |
| DotnetSort                   | 256  | SingleElementMoved |       992.5 ns |     24.65 ns |     12.89 ns |  0.83 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Sorted**             |       **815.1 ns** |     **29.03 ns** |     **15.19 ns** |  **1.00** |    **0.02** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 256  | Sorted             |       752.1 ns |     52.03 ns |     23.10 ns |  0.92 |    0.03 |    4 |         - |          NA |
| QuickSortMedian3             | 256  | Sorted             |     1,300.7 ns |    119.87 ns |     53.22 ns |  1.60 |    0.07 |    4 |         - |          NA |
| QuickSortMedian9             | 256  | Sorted             |     1,318.8 ns |    169.53 ns |     75.27 ns |  1.62 |    0.09 |    4 |         - |          NA |
| DualPivotQuickSort           | 256  | Sorted             |       709.8 ns |     72.46 ns |     25.84 ns |  0.87 |    0.03 |    4 |         - |          NA |
| StableQuickSort              | 256  | Sorted             |     9,166.3 ns |    382.50 ns |    200.06 ns | 11.25 |    0.30 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Sorted             |     4,907.5 ns |    489.27 ns |    217.24 ns |  6.02 |    0.27 |    5 |         - |          NA |
| DestswapStableQuickSort      | 256  | Sorted             |     4,073.5 ns |     74.80 ns |     26.67 ns |  5.00 |    0.09 |    5 |         - |          NA |
| IntroSort                    | 256  | Sorted             |       337.8 ns |      6.70 ns |      2.98 ns |  0.41 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 256  | Sorted             |     1,107.1 ns |     16.09 ns |      7.15 ns |  1.36 |    0.03 |    4 |         - |          NA |
| PDQSort                      | 256  | Sorted             |       355.3 ns |      2.89 ns |      1.51 ns |  0.44 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 256  | Sorted             |       351.2 ns |     17.54 ns |      7.79 ns |  0.43 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 256  | Sorted             |       215.5 ns |     53.47 ns |     27.96 ns |  0.26 |    0.03 |    1 |         - |          NA |
| StdSort                      | 256  | Sorted             |       480.3 ns |    239.72 ns |    125.38 ns |  0.59 |    0.15 |    3 |         - |          NA |
| BlockQuickSort               | 256  | Sorted             |     1,383.5 ns |      7.54 ns |      3.35 ns |  1.70 |    0.03 |    4 |         - |          NA |
| DotnetSort                   | 256  | Sorted             |       939.8 ns |    234.57 ns |    122.69 ns |  1.15 |    0.14 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Reversed**           |       **997.6 ns** |     **18.31 ns** |      **6.53 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 256  | Reversed           |       961.9 ns |     11.50 ns |      5.10 ns |  0.96 |    0.01 |    3 |         - |          NA |
| QuickSortMedian3             | 256  | Reversed           |     1,301.0 ns |    115.91 ns |     51.47 ns |  1.30 |    0.05 |    4 |         - |          NA |
| QuickSortMedian9             | 256  | Reversed           |     1,494.3 ns |     80.98 ns |     35.95 ns |  1.50 |    0.03 |    4 |         - |          NA |
| DualPivotQuickSort           | 256  | Reversed           |     1,183.5 ns |    197.67 ns |    103.39 ns |  1.19 |    0.10 |    4 |         - |          NA |
| StableQuickSort              | 256  | Reversed           |     8,409.7 ns |     69.69 ns |     24.85 ns |  8.43 |    0.06 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Reversed           |     5,096.5 ns |    337.36 ns |    149.79 ns |  5.11 |    0.14 |    5 |         - |          NA |
| DestswapStableQuickSort      | 256  | Reversed           |     7,367.3 ns |    252.90 ns |    132.27 ns |  7.39 |    0.13 |    6 |         - |          NA |
| IntroSort                    | 256  | Reversed           |       518.5 ns |     52.64 ns |     27.53 ns |  0.52 |    0.03 |    2 |         - |          NA |
| IntroSortDotnet              | 256  | Reversed           |     1,440.8 ns |     82.43 ns |     36.60 ns |  1.44 |    0.04 |    4 |         - |          NA |
| PDQSort                      | 256  | Reversed           |       557.8 ns |     14.93 ns |      6.63 ns |  0.56 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 256  | Reversed           |     1,064.5 ns |    447.14 ns |    233.86 ns |  1.07 |    0.22 |    3 |         - |          NA |
| Ipnsort                      | 256  | Reversed           |       283.8 ns |     91.96 ns |     48.09 ns |  0.28 |    0.05 |    1 |         - |          NA |
| StdSort                      | 256  | Reversed           |       888.6 ns |    216.02 ns |    112.99 ns |  0.89 |    0.11 |    3 |         - |          NA |
| BlockQuickSort               | 256  | Reversed           |     1,820.3 ns |    437.73 ns |    228.94 ns |  1.82 |    0.22 |    4 |         - |          NA |
| DotnetSort                   | 256  | Reversed           |     1,239.5 ns |     88.16 ns |     39.14 ns |  1.24 |    0.04 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **PipeOrgan**          |     **7,454.3 ns** |    **223.55 ns** |     **99.26 ns** |  **1.00** |    **0.02** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 256  | PipeOrgan          |     2,805.3 ns |    552.43 ns |    288.93 ns |  0.38 |    0.04 |    3 |         - |          NA |
| QuickSortMedian3             | 256  | PipeOrgan          |     3,140.1 ns |    205.45 ns |     91.22 ns |  0.42 |    0.01 |    3 |         - |          NA |
| QuickSortMedian9             | 256  | PipeOrgan          |     2,606.8 ns |    281.82 ns |    125.13 ns |  0.35 |    0.02 |    3 |         - |          NA |
| DualPivotQuickSort           | 256  | PipeOrgan          |     1,540.1 ns |     40.92 ns |     18.17 ns |  0.21 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 256  | PipeOrgan          |     8,835.5 ns |    211.14 ns |    110.43 ns |  1.19 |    0.02 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 256  | PipeOrgan          |     5,081.1 ns |    272.04 ns |    142.28 ns |  0.68 |    0.02 |    4 |         - |          NA |
| DestswapStableQuickSort      | 256  | PipeOrgan          |     9,585.5 ns |  1,569.36 ns |    820.81 ns |  1.29 |    0.11 |    5 |         - |          NA |
| IntroSort                    | 256  | PipeOrgan          |     1,646.7 ns |    369.10 ns |    193.05 ns |  0.22 |    0.02 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | PipeOrgan          |     2,104.7 ns |     59.51 ns |     26.42 ns |  0.28 |    0.00 |    2 |         - |          NA |
| PDQSort                      | 256  | PipeOrgan          |     1,734.8 ns |    135.10 ns |     59.99 ns |  0.23 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | PipeOrgan          |     2,910.2 ns |    133.46 ns |     59.26 ns |  0.39 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 256  | PipeOrgan          |     4,025.0 ns |    374.22 ns |    195.72 ns |  0.54 |    0.03 |    4 |         - |          NA |
| StdSort                      | 256  | PipeOrgan          |     4,656.8 ns |    154.73 ns |     68.70 ns |  0.62 |    0.01 |    4 |         - |          NA |
| BlockQuickSort               | 256  | PipeOrgan          |     4,889.3 ns |    274.58 ns |    121.91 ns |  0.66 |    0.02 |    4 |         - |          NA |
| DotnetSort                   | 256  | PipeOrgan          |     2,564.0 ns |    183.70 ns |     81.56 ns |  0.34 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **ManyDuplicates**     |     **2,330.1 ns** |    **133.53 ns** |     **59.29 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 256  | ManyDuplicates     |     1,768.3 ns |     64.85 ns |     23.12 ns |  0.76 |    0.02 |    1 |         - |          NA |
| QuickSortMedian3             | 256  | ManyDuplicates     |     2,770.1 ns |    333.52 ns |    174.44 ns |  1.19 |    0.08 |    1 |         - |          NA |
| QuickSortMedian9             | 256  | ManyDuplicates     |     2,798.8 ns |    123.81 ns |     54.97 ns |  1.20 |    0.04 |    1 |         - |          NA |
| DualPivotQuickSort           | 256  | ManyDuplicates     |     1,918.1 ns |    191.73 ns |     85.13 ns |  0.82 |    0.04 |    1 |         - |          NA |
| StableQuickSort              | 256  | ManyDuplicates     |     6,777.6 ns |    277.30 ns |    145.03 ns |  2.91 |    0.09 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 256  | ManyDuplicates     |     3,749.5 ns |    293.15 ns |    153.32 ns |  1.61 |    0.07 |    2 |         - |          NA |
| DestswapStableQuickSort      | 256  | ManyDuplicates     |     5,332.7 ns |    284.62 ns |    126.37 ns |  2.29 |    0.07 |    3 |         - |          NA |
| IntroSort                    | 256  | ManyDuplicates     |     2,122.2 ns |     33.39 ns |     11.91 ns |  0.91 |    0.02 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | ManyDuplicates     |     1,649.5 ns |     20.45 ns |      7.29 ns |  0.71 |    0.02 |    1 |         - |          NA |
| PDQSort                      | 256  | ManyDuplicates     |     1,630.5 ns |     35.39 ns |     12.62 ns |  0.70 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | ManyDuplicates     |     2,470.5 ns |    105.61 ns |     46.89 ns |  1.06 |    0.03 |    1 |         - |          NA |
| Ipnsort                      | 256  | ManyDuplicates     |     3,795.0 ns |    275.81 ns |    144.25 ns |  1.63 |    0.07 |    2 |         - |          NA |
| StdSort                      | 256  | ManyDuplicates     |     2,702.4 ns |    296.51 ns |    155.08 ns |  1.16 |    0.07 |    1 |         - |          NA |
| BlockQuickSort               | 256  | ManyDuplicates     |     2,570.8 ns |     55.06 ns |     24.45 ns |  1.10 |    0.03 |    1 |         - |          NA |
| DotnetSort                   | 256  | ManyDuplicates     |     1,846.3 ns |     96.00 ns |     42.63 ns |  0.79 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Random**             |    **13,830.3 ns** |    **341.40 ns** |    **151.59 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Random             |    13,530.5 ns |    620.19 ns |    324.37 ns |  0.98 |    0.02 |    1 |         - |          NA |
| QuickSortMedian3             | 1024 | Random             |    13,441.7 ns |    494.44 ns |    258.60 ns |  0.97 |    0.02 |    1 |         - |          NA |
| QuickSortMedian9             | 1024 | Random             |    14,870.5 ns |    625.39 ns |    277.68 ns |  1.08 |    0.02 |    1 |         - |          NA |
| DualPivotQuickSort           | 1024 | Random             |    11,039.3 ns |    392.54 ns |    205.31 ns |  0.80 |    0.02 |    1 |         - |          NA |
| StableQuickSort              | 1024 | Random             |    58,356.4 ns |    528.17 ns |    276.24 ns |  4.22 |    0.05 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Random             |    36,267.8 ns |    294.75 ns |    130.87 ns |  2.62 |    0.03 |    3 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Random             |    31,157.9 ns |    598.94 ns |    265.93 ns |  2.25 |    0.03 |    3 |         - |          NA |
| IntroSort                    | 1024 | Random             |    12,216.7 ns |    527.24 ns |    275.75 ns |  0.88 |    0.02 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | Random             |    10,237.5 ns |    392.03 ns |    205.04 ns |  0.74 |    0.02 |    1 |         - |          NA |
| PDQSort                      | 1024 | Random             |     9,431.3 ns |    559.76 ns |    248.53 ns |  0.68 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | Random             |    13,097.4 ns |    240.83 ns |    106.93 ns |  0.95 |    0.01 |    1 |         - |          NA |
| Ipnsort                      | 1024 | Random             |    19,361.3 ns |    274.95 ns |    143.80 ns |  1.40 |    0.02 |    2 |         - |          NA |
| StdSort                      | 1024 | Random             |    13,426.5 ns |    306.93 ns |    160.53 ns |  0.97 |    0.01 |    1 |         - |          NA |
| BlockQuickSort               | 1024 | Random             |    14,037.6 ns |    220.75 ns |     98.01 ns |  1.02 |    0.01 |    1 |         - |          NA |
| DotnetSort                   | 1024 | Random             |    11,119.1 ns |    368.93 ns |    192.96 ns |  0.80 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **SingleElementMoved** |     **5,623.1 ns** |    **446.57 ns** |    **233.57 ns** |  **1.00** |    **0.05** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 1024 | SingleElementMoved |     5,506.7 ns |    413.73 ns |    183.70 ns |  0.98 |    0.05 |    1 |         - |          NA |
| QuickSortMedian3             | 1024 | SingleElementMoved |     7,977.7 ns |    171.57 ns |     76.18 ns |  1.42 |    0.06 |    2 |         - |          NA |
| QuickSortMedian9             | 1024 | SingleElementMoved |    10,767.9 ns |    338.39 ns |    176.98 ns |  1.92 |    0.08 |    2 |         - |          NA |
| DualPivotQuickSort           | 1024 | SingleElementMoved |     4,372.1 ns |    310.46 ns |    162.37 ns |  0.78 |    0.04 |    1 |         - |          NA |
| StableQuickSort              | 1024 | SingleElementMoved |    43,399.9 ns |    211.49 ns |     93.90 ns |  7.73 |    0.30 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | SingleElementMoved |    26,039.6 ns |    589.08 ns |    261.56 ns |  4.64 |    0.18 |    4 |         - |          NA |
| DestswapStableQuickSort      | 1024 | SingleElementMoved |    20,239.3 ns |    940.16 ns |    491.72 ns |  3.60 |    0.16 |    3 |         - |          NA |
| IntroSort                    | 1024 | SingleElementMoved |     3,990.3 ns |     34.93 ns |     12.46 ns |  0.71 |    0.03 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | SingleElementMoved |     5,779.0 ns |    409.14 ns |    213.99 ns |  1.03 |    0.05 |    1 |         - |          NA |
| PDQSort                      | 1024 | SingleElementMoved |     4,904.9 ns |     65.82 ns |     29.23 ns |  0.87 |    0.03 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | SingleElementMoved |     6,210.8 ns |    426.84 ns |    223.25 ns |  1.11 |    0.06 |    1 |         - |          NA |
| Ipnsort                      | 1024 | SingleElementMoved |    18,043.5 ns |    273.63 ns |    143.11 ns |  3.21 |    0.12 |    3 |         - |          NA |
| StdSort                      | 1024 | SingleElementMoved |     7,635.6 ns |     66.61 ns |     34.84 ns |  1.36 |    0.05 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | SingleElementMoved |     9,303.6 ns |    399.31 ns |    208.85 ns |  1.66 |    0.07 |    2 |         - |          NA |
| DotnetSort                   | 1024 | SingleElementMoved |     5,408.6 ns |    224.99 ns |    117.67 ns |  0.96 |    0.04 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Sorted**             |     **4,072.7 ns** |    **236.63 ns** |    **105.06 ns** |  **1.00** |    **0.03** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Sorted             |     3,934.7 ns |    568.78 ns |    252.54 ns |  0.97 |    0.06 |    3 |         - |          NA |
| QuickSortMedian3             | 1024 | Sorted             |     5,798.0 ns |    325.94 ns |    170.48 ns |  1.42 |    0.05 |    4 |         - |          NA |
| QuickSortMedian9             | 1024 | Sorted             |     6,152.9 ns |    281.37 ns |    147.16 ns |  1.51 |    0.05 |    4 |         - |          NA |
| DualPivotQuickSort           | 1024 | Sorted             |     3,702.7 ns |    274.08 ns |    143.35 ns |  0.91 |    0.04 |    3 |         - |          NA |
| StableQuickSort              | 1024 | Sorted             |    47,172.6 ns |    935.68 ns |    415.45 ns | 11.59 |    0.29 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Sorted             |    22,718.6 ns |    974.62 ns |    509.74 ns |  5.58 |    0.18 |    5 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Sorted             |    19,892.8 ns |  1,048.95 ns |    548.62 ns |  4.89 |    0.17 |    5 |         - |          NA |
| IntroSort                    | 1024 | Sorted             |     1,092.7 ns |    190.73 ns |     99.76 ns |  0.27 |    0.02 |    2 |         - |          NA |
| IntroSortDotnet              | 1024 | Sorted             |     4,798.5 ns |    352.76 ns |    184.50 ns |  1.18 |    0.05 |    3 |         - |          NA |
| PDQSort                      | 1024 | Sorted             |     1,327.3 ns |      6.23 ns |      2.77 ns |  0.33 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | Sorted             |     1,329.2 ns |      3.43 ns |      1.53 ns |  0.33 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 1024 | Sorted             |       585.7 ns |      1.60 ns |      0.57 ns |  0.14 |    0.00 |    1 |         - |          NA |
| StdSort                      | 1024 | Sorted             |     1,215.1 ns |     49.31 ns |     21.90 ns |  0.30 |    0.01 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | Sorted             |     7,239.3 ns |     25.52 ns |     13.35 ns |  1.78 |    0.04 |    4 |         - |          NA |
| DotnetSort                   | 1024 | Sorted             |     4,072.4 ns |    317.16 ns |    140.82 ns |  1.00 |    0.04 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Reversed**           |     **4,641.1 ns** |    **106.56 ns** |     **47.31 ns** |  **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Reversed           |     4,582.8 ns |     78.72 ns |     28.07 ns |  0.99 |    0.01 |    4 |         - |          NA |
| QuickSortMedian3             | 1024 | Reversed           |     5,961.9 ns |    344.11 ns |    179.97 ns |  1.28 |    0.04 |    5 |         - |          NA |
| QuickSortMedian9             | 1024 | Reversed           |     6,378.1 ns |    241.31 ns |    126.21 ns |  1.37 |    0.03 |    5 |         - |          NA |
| DualPivotQuickSort           | 1024 | Reversed           |     4,970.5 ns |    547.42 ns |    286.31 ns |  1.07 |    0.06 |    4 |         - |          NA |
| StableQuickSort              | 1024 | Reversed           |    42,835.9 ns |    302.98 ns |    134.52 ns |  9.23 |    0.09 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Reversed           |    24,472.9 ns |    274.08 ns |    121.70 ns |  5.27 |    0.06 |    6 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Reversed           |    33,781.5 ns |    657.18 ns |    343.72 ns |  7.28 |    0.10 |    7 |         - |          NA |
| IntroSort                    | 1024 | Reversed           |     3,251.6 ns |    322.70 ns |    168.78 ns |  0.70 |    0.03 |    3 |         - |          NA |
| IntroSortDotnet              | 1024 | Reversed           |     7,236.3 ns |    384.62 ns |    201.16 ns |  1.56 |    0.04 |    5 |         - |          NA |
| PDQSort                      | 1024 | Reversed           |     2,235.6 ns |     56.73 ns |     25.19 ns |  0.48 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | Reversed           |     3,253.4 ns |     22.40 ns |      7.99 ns |  0.70 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 1024 | Reversed           |       910.2 ns |      2.05 ns |      1.07 ns |  0.20 |    0.00 |    1 |         - |          NA |
| StdSort                      | 1024 | Reversed           |     2,939.7 ns |     17.09 ns |      8.94 ns |  0.63 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 1024 | Reversed           |     8,411.7 ns |     55.09 ns |     19.64 ns |  1.81 |    0.02 |    5 |         - |          NA |
| DotnetSort                   | 1024 | Reversed           |     6,891.7 ns |    869.61 ns |    454.82 ns |  1.49 |    0.09 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **PipeOrgan**          |   **108,601.5 ns** |    **230.89 ns** |    **102.52 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| QuickSort3way                | 1024 | PipeOrgan          |    15,218.9 ns |  1,220.29 ns |    638.24 ns |  0.14 |    0.01 |    2 |         - |          NA |
| QuickSortMedian3             | 1024 | PipeOrgan          |    16,239.5 ns |    324.70 ns |    144.17 ns |  0.15 |    0.00 |    2 |         - |          NA |
| QuickSortMedian9             | 1024 | PipeOrgan          |    11,999.0 ns |    418.23 ns |    185.70 ns |  0.11 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort           | 1024 | PipeOrgan          |     8,209.8 ns |    661.73 ns |    346.10 ns |  0.08 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 1024 | PipeOrgan          |    43,441.8 ns |    382.79 ns |    200.21 ns |  0.40 |    0.00 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | PipeOrgan          |    24,813.8 ns |    388.80 ns |    172.63 ns |  0.23 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 1024 | PipeOrgan          |    36,157.5 ns |    415.70 ns |    217.42 ns |  0.33 |    0.00 |    4 |         - |          NA |
| IntroSort                    | 1024 | PipeOrgan          |    10,799.9 ns |  1,108.79 ns |    492.31 ns |  0.10 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet              | 1024 | PipeOrgan          |    14,285.1 ns |    436.04 ns |    228.06 ns |  0.13 |    0.00 |    2 |         - |          NA |
| PDQSort                      | 1024 | PipeOrgan          |     8,786.3 ns |    293.22 ns |    153.36 ns |  0.08 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | PipeOrgan          |    15,363.4 ns |    197.54 ns |    103.32 ns |  0.14 |    0.00 |    2 |         - |          NA |
| Ipnsort                      | 1024 | PipeOrgan          |    21,154.6 ns |    468.02 ns |    244.78 ns |  0.19 |    0.00 |    3 |         - |          NA |
| StdSort                      | 1024 | PipeOrgan          |    21,565.0 ns |    213.64 ns |     94.86 ns |  0.20 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 1024 | PipeOrgan          |    24,361.7 ns |    120.31 ns |     53.42 ns |  0.22 |    0.00 |    3 |         - |          NA |
| DotnetSort                   | 1024 | PipeOrgan          |    17,026.9 ns |  1,368.35 ns |    715.67 ns |  0.16 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **ManyDuplicates**     |     **9,581.8 ns** |    **401.83 ns** |    **178.41 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 1024 | ManyDuplicates     |     7,642.2 ns |    271.76 ns |    120.66 ns |  0.80 |    0.02 |    2 |         - |          NA |
| QuickSortMedian3             | 1024 | ManyDuplicates     |    11,886.8 ns |    356.38 ns |    158.24 ns |  1.24 |    0.03 |    2 |         - |          NA |
| QuickSortMedian9             | 1024 | ManyDuplicates     |    12,440.8 ns |    468.86 ns |    245.22 ns |  1.30 |    0.03 |    2 |         - |          NA |
| DualPivotQuickSort           | 1024 | ManyDuplicates     |     7,747.2 ns |    628.31 ns |    278.97 ns |  0.81 |    0.03 |    2 |         - |          NA |
| StableQuickSort              | 1024 | ManyDuplicates     |    29,282.7 ns |    268.95 ns |    140.67 ns |  3.06 |    0.05 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | ManyDuplicates     |    14,196.1 ns |    487.83 ns |    216.60 ns |  1.48 |    0.03 |    2 |         - |          NA |
| DestswapStableQuickSort      | 1024 | ManyDuplicates     |    14,253.1 ns |    390.28 ns |    204.12 ns |  1.49 |    0.03 |    2 |         - |          NA |
| IntroSort                    | 1024 | ManyDuplicates     |    10,653.2 ns |    300.23 ns |    133.30 ns |  1.11 |    0.02 |    2 |         - |          NA |
| IntroSortDotnet              | 1024 | ManyDuplicates     |     8,155.9 ns |     86.83 ns |     38.55 ns |  0.85 |    0.02 |    2 |         - |          NA |
| PDQSort                      | 1024 | ManyDuplicates     |     6,112.3 ns |    287.68 ns |    150.46 ns |  0.64 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | ManyDuplicates     |     9,036.6 ns |    370.22 ns |    193.63 ns |  0.94 |    0.03 |    2 |         - |          NA |
| Ipnsort                      | 1024 | ManyDuplicates     |    18,179.6 ns |    236.01 ns |    104.79 ns |  1.90 |    0.03 |    3 |         - |          NA |
| StdSort                      | 1024 | ManyDuplicates     |    11,278.8 ns |    477.75 ns |    249.87 ns |  1.18 |    0.03 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | ManyDuplicates     |    12,076.8 ns |    231.50 ns |    121.08 ns |  1.26 |    0.02 |    2 |         - |          NA |
| DotnetSort                   | 1024 | ManyDuplicates     |     8,161.4 ns |    112.17 ns |     49.80 ns |  0.85 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Random**             |    **68,067.8 ns** |  **9,873.61 ns** |  **5,164.09 ns** |  **1.01** |    **0.10** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Random             |    76,835.5 ns | 17,046.56 ns |  8,915.68 ns |  1.13 |    0.15 |    1 |         - |          NA |
| QuickSortMedian3             | 4096 | Random             |    65,808.6 ns |  3,517.12 ns |  1,561.62 ns |  0.97 |    0.07 |    1 |         - |          NA |
| QuickSortMedian9             | 4096 | Random             |    68,037.1 ns |    783.86 ns |    348.04 ns |  1.00 |    0.07 |    1 |         - |          NA |
| DualPivotQuickSort           | 4096 | Random             |    54,545.9 ns |  1,251.54 ns |    555.69 ns |  0.81 |    0.06 |    1 |         - |          NA |
| StableQuickSort              | 4096 | Random             |   568,002.5 ns |  2,093.72 ns |    929.63 ns |  8.39 |    0.60 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Random             |   436,481.1 ns |  3,997.69 ns |  2,090.87 ns |  6.44 |    0.46 |    4 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Random             |   177,224.3 ns | 27,580.90 ns | 14,425.35 ns |  2.62 |    0.27 |    3 |         - |          NA |
| IntroSort                    | 4096 | Random             |    61,848.1 ns |    684.81 ns |    304.06 ns |  0.91 |    0.07 |    1 |         - |          NA |
| IntroSortDotnet              | 4096 | Random             |    48,303.2 ns |    882.58 ns |    391.87 ns |  0.71 |    0.05 |    1 |         - |          NA |
| PDQSort                      | 4096 | Random             |    46,308.4 ns |  1,966.47 ns |  1,028.50 ns |  0.68 |    0.05 |    1 |         - |          NA |
| PDQSortBranchless            | 4096 | Random             |    61,857.4 ns |    611.20 ns |    271.38 ns |  0.91 |    0.07 |    1 |         - |          NA |
| Ipnsort                      | 4096 | Random             |    97,963.9 ns |    594.70 ns |    264.05 ns |  1.45 |    0.10 |    2 |         - |          NA |
| StdSort                      | 4096 | Random             |    62,728.8 ns |  1,181.39 ns |    617.89 ns |  0.93 |    0.07 |    1 |         - |          NA |
| BlockQuickSort               | 4096 | Random             |    68,837.9 ns |  1,100.80 ns |    575.74 ns |  1.02 |    0.07 |    1 |         - |          NA |
| DotnetSort                   | 4096 | Random             |    53,743.5 ns |  1,763.37 ns |    782.95 ns |  0.79 |    0.06 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **SingleElementMoved** |    **25,448.0 ns** |    **961.19 ns** |    **426.77 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 4096 | SingleElementMoved |    26,566.3 ns |    589.89 ns |    261.92 ns |  1.04 |    0.02 |    1 |         - |          NA |
| QuickSortMedian3             | 4096 | SingleElementMoved |    35,453.9 ns |  1,011.64 ns |    449.18 ns |  1.39 |    0.03 |    1 |         - |          NA |
| QuickSortMedian9             | 4096 | SingleElementMoved |    47,546.2 ns |    825.20 ns |    431.60 ns |  1.87 |    0.03 |    2 |         - |          NA |
| DualPivotQuickSort           | 4096 | SingleElementMoved |    24,078.4 ns |  2,192.90 ns |  1,146.93 ns |  0.95 |    0.05 |    1 |         - |          NA |
| StableQuickSort              | 4096 | SingleElementMoved |   208,130.1 ns |  1,236.30 ns |    548.93 ns |  8.18 |    0.13 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | SingleElementMoved |   129,315.9 ns |    435.57 ns |    155.33 ns |  5.08 |    0.08 |    4 |         - |          NA |
| DestswapStableQuickSort      | 4096 | SingleElementMoved |    96,306.3 ns |  1,840.53 ns |    962.63 ns |  3.79 |    0.07 |    3 |         - |          NA |
| IntroSort                    | 4096 | SingleElementMoved |    19,163.4 ns |  2,139.27 ns |    949.85 ns |  0.75 |    0.04 |    1 |         - |          NA |
| IntroSortDotnet              | 4096 | SingleElementMoved |    27,683.2 ns |    666.22 ns |    348.44 ns |  1.09 |    0.02 |    1 |         - |          NA |
| PDQSort                      | 4096 | SingleElementMoved |    21,329.3 ns |    529.75 ns |    235.21 ns |  0.84 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless            | 4096 | SingleElementMoved |    26,403.6 ns |    391.34 ns |    204.68 ns |  1.04 |    0.02 |    1 |         - |          NA |
| Ipnsort                      | 4096 | SingleElementMoved |    87,376.0 ns |    358.83 ns |    159.32 ns |  3.43 |    0.05 |    3 |         - |          NA |
| StdSort                      | 4096 | SingleElementMoved |    32,599.2 ns |    752.27 ns |    334.01 ns |  1.28 |    0.02 |    1 |         - |          NA |
| BlockQuickSort               | 4096 | SingleElementMoved |    44,178.5 ns |    617.01 ns |    322.71 ns |  1.74 |    0.03 |    2 |         - |          NA |
| DotnetSort                   | 4096 | SingleElementMoved |    27,939.5 ns |  2,171.48 ns |  1,135.73 ns |  1.10 |    0.05 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Sorted**             |    **19,363.3 ns** |    **269.80 ns** |    **119.79 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Sorted             |    18,674.9 ns |  1,143.00 ns |    597.81 ns |  0.96 |    0.03 |    3 |         - |          NA |
| QuickSortMedian3             | 4096 | Sorted             |    25,971.6 ns |  1,444.59 ns |    755.55 ns |  1.34 |    0.04 |    3 |         - |          NA |
| QuickSortMedian9             | 4096 | Sorted             |    27,576.8 ns |    595.73 ns |    264.51 ns |  1.42 |    0.02 |    3 |         - |          NA |
| DualPivotQuickSort           | 4096 | Sorted             |    20,649.3 ns |    551.89 ns |    288.65 ns |  1.07 |    0.02 |    3 |         - |          NA |
| StableQuickSort              | 4096 | Sorted             |   225,931.0 ns |    612.42 ns |    271.92 ns | 11.67 |    0.07 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Sorted             |   107,590.6 ns |  2,750.35 ns |  1,438.49 ns |  5.56 |    0.08 |    5 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Sorted             |    93,043.9 ns |  1,683.98 ns |    880.75 ns |  4.81 |    0.05 |    5 |         - |          NA |
| IntroSort                    | 4096 | Sorted             |     4,355.3 ns |    745.75 ns |    331.12 ns |  0.22 |    0.02 |    2 |         - |          NA |
| IntroSortDotnet              | 4096 | Sorted             |    22,433.0 ns |    931.65 ns |    487.27 ns |  1.16 |    0.02 |    3 |         - |          NA |
| PDQSort                      | 4096 | Sorted             |     5,098.5 ns |     11.54 ns |      4.11 ns |  0.26 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 4096 | Sorted             |     5,112.5 ns |     51.24 ns |     18.27 ns |  0.26 |    0.00 |    2 |         - |          NA |
| Ipnsort                      | 4096 | Sorted             |     2,289.6 ns |     17.75 ns |      7.88 ns |  0.12 |    0.00 |    1 |         - |          NA |
| StdSort                      | 4096 | Sorted             |     4,728.1 ns |    356.84 ns |    158.44 ns |  0.24 |    0.01 |    2 |         - |          NA |
| BlockQuickSort               | 4096 | Sorted             |    36,132.8 ns |    409.50 ns |    181.82 ns |  1.87 |    0.01 |    4 |         - |          NA |
| DotnetSort                   | 4096 | Sorted             |    19,885.8 ns |  2,213.33 ns |    982.73 ns |  1.03 |    0.05 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Reversed**           |    **22,055.9 ns** |    **602.66 ns** |    **315.21 ns** |  **1.00** |    **0.02** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Reversed           |    23,445.9 ns |  2,649.54 ns |  1,385.76 ns |  1.06 |    0.06 |    4 |         - |          NA |
| QuickSortMedian3             | 4096 | Reversed           |    27,340.8 ns |  1,057.29 ns |    552.98 ns |  1.24 |    0.03 |    4 |         - |          NA |
| QuickSortMedian9             | 4096 | Reversed           |    28,788.8 ns |    310.87 ns |    162.59 ns |  1.31 |    0.02 |    4 |         - |          NA |
| DualPivotQuickSort           | 4096 | Reversed           |    24,841.9 ns |    723.33 ns |    378.32 ns |  1.13 |    0.02 |    4 |         - |          NA |
| StableQuickSort              | 4096 | Reversed           |   206,694.7 ns |  1,280.90 ns |    669.93 ns |  9.37 |    0.13 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Reversed           |   118,342.1 ns |  3,776.41 ns |  1,975.13 ns |  5.37 |    0.11 |    6 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Reversed           |   147,071.5 ns |  1,856.22 ns |    970.84 ns |  6.67 |    0.10 |    7 |         - |          NA |
| IntroSort                    | 4096 | Reversed           |    13,463.3 ns |    235.17 ns |    104.42 ns |  0.61 |    0.01 |    3 |         - |          NA |
| IntroSortDotnet              | 4096 | Reversed           |    34,708.5 ns |    609.14 ns |    217.22 ns |  1.57 |    0.02 |    5 |         - |          NA |
| PDQSort                      | 4096 | Reversed           |     8,396.6 ns |    651.03 ns |    340.50 ns |  0.38 |    0.02 |    2 |         - |          NA |
| PDQSortBranchless            | 4096 | Reversed           |    12,830.4 ns |    389.22 ns |    203.57 ns |  0.58 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 4096 | Reversed           |     3,623.0 ns |    259.95 ns |    115.42 ns |  0.16 |    0.01 |    1 |         - |          NA |
| StdSort                      | 4096 | Reversed           |    11,314.7 ns |    311.27 ns |    138.21 ns |  0.51 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 4096 | Reversed           |    40,031.1 ns |    893.09 ns |    467.10 ns |  1.82 |    0.03 |    5 |         - |          NA |
| DotnetSort                   | 4096 | Reversed           |    42,476.8 ns |  5,558.08 ns |  2,906.98 ns |  1.93 |    0.13 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **PipeOrgan**          | **1,583,336.9 ns** |  **4,176.92 ns** |  **2,184.61 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| QuickSort3way                | 4096 | PipeOrgan          |    84,681.0 ns |  6,555.01 ns |  3,428.40 ns |  0.05 |    0.00 |    3 |         - |          NA |
| QuickSortMedian3             | 4096 | PipeOrgan          |    82,492.6 ns |  3,114.42 ns |  1,628.90 ns |  0.05 |    0.00 |    3 |         - |          NA |
| QuickSortMedian9             | 4096 | PipeOrgan          |    54,447.6 ns |  1,357.83 ns |    602.88 ns |  0.03 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort           | 4096 | PipeOrgan          |    40,325.8 ns |  1,934.28 ns |  1,011.67 ns |  0.03 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 4096 | PipeOrgan          |   208,134.4 ns |  1,392.06 ns |    728.07 ns |  0.13 |    0.00 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | PipeOrgan          |   120,088.6 ns |  4,369.62 ns |  2,285.40 ns |  0.08 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 4096 | PipeOrgan          |   169,660.5 ns |  1,148.34 ns |    600.60 ns |  0.11 |    0.00 |    4 |         - |          NA |
| IntroSort                    | 4096 | PipeOrgan          |    77,037.8 ns |  6,039.00 ns |  2,681.35 ns |  0.05 |    0.00 |    3 |         - |          NA |
| IntroSortDotnet              | 4096 | PipeOrgan          |    83,611.7 ns |    603.75 ns |    268.07 ns |  0.05 |    0.00 |    3 |         - |          NA |
| PDQSort                      | 4096 | PipeOrgan          |    41,957.4 ns |    735.87 ns |    326.73 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 4096 | PipeOrgan          |    73,693.9 ns |  1,029.14 ns |    456.94 ns |  0.05 |    0.00 |    3 |         - |          NA |
| Ipnsort                      | 4096 | PipeOrgan          |   106,693.5 ns |  1,138.59 ns |    595.51 ns |  0.07 |    0.00 |    3 |         - |          NA |
| StdSort                      | 4096 | PipeOrgan          |   108,212.5 ns |    743.14 ns |    329.96 ns |  0.07 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 4096 | PipeOrgan          |   106,720.1 ns |  1,113.32 ns |    582.29 ns |  0.07 |    0.00 |    3 |         - |          NA |
| DotnetSort                   | 4096 | PipeOrgan          |    92,340.2 ns |  5,096.66 ns |  2,665.65 ns |  0.06 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **ManyDuplicates**     |    **44,259.2 ns** |  **2,030.25 ns** |  **1,061.86 ns** |  **1.00** |    **0.03** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 4096 | ManyDuplicates     |    33,241.6 ns |  2,321.04 ns |  1,213.95 ns |  0.75 |    0.03 |    2 |         - |          NA |
| QuickSortMedian3             | 4096 | ManyDuplicates     |    52,115.4 ns |  1,522.71 ns |    676.09 ns |  1.18 |    0.03 |    2 |         - |          NA |
| QuickSortMedian9             | 4096 | ManyDuplicates     |    56,949.8 ns |  2,538.04 ns |  1,327.45 ns |  1.29 |    0.04 |    2 |         - |          NA |
| DualPivotQuickSort           | 4096 | ManyDuplicates     |    27,470.4 ns |  1,009.39 ns |    448.18 ns |  0.62 |    0.02 |    2 |         - |          NA |
| StableQuickSort              | 4096 | ManyDuplicates     |   109,468.9 ns |  1,123.42 ns |    400.62 ns |  2.47 |    0.06 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | ManyDuplicates     |    54,183.9 ns |    901.94 ns |    400.47 ns |  1.22 |    0.03 |    2 |         - |          NA |
| DestswapStableQuickSort      | 4096 | ManyDuplicates     |    55,013.8 ns |  2,713.41 ns |  1,419.16 ns |  1.24 |    0.04 |    2 |         - |          NA |
| IntroSort                    | 4096 | ManyDuplicates     |    50,050.5 ns |  1,440.84 ns |    753.59 ns |  1.13 |    0.03 |    2 |         - |          NA |
| IntroSortDotnet              | 4096 | ManyDuplicates     |    37,976.4 ns |  1,680.52 ns |    746.16 ns |  0.86 |    0.02 |    2 |         - |          NA |
| PDQSort                      | 4096 | ManyDuplicates     |    21,789.9 ns |    490.56 ns |    217.81 ns |  0.49 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 4096 | ManyDuplicates     |    30,349.8 ns |    758.41 ns |    396.66 ns |  0.69 |    0.02 |    2 |         - |          NA |
| Ipnsort                      | 4096 | ManyDuplicates     |    60,358.4 ns |  1,152.04 ns |    602.54 ns |  1.36 |    0.03 |    2 |         - |          NA |
| StdSort                      | 4096 | ManyDuplicates     |    33,556.8 ns |    553.77 ns |    245.88 ns |  0.76 |    0.02 |    2 |         - |          NA |
| BlockQuickSort               | 4096 | ManyDuplicates     |    53,457.6 ns |    993.37 ns |    519.55 ns |  1.21 |    0.03 |    2 |         - |          NA |
| DotnetSort                   | 4096 | ManyDuplicates     |    36,649.3 ns |  1,574.29 ns |    823.38 ns |  0.83 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Random**             |   **476,947.6 ns** | **26,685.53 ns** | **13,957.05 ns** |  **1.00** |    **0.04** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Random             |   420,983.9 ns | 10,242.74 ns |  4,547.84 ns |  0.88 |    0.03 |    3 |         - |          NA |
| QuickSortMedian3             | 8192 | Random             |   466,484.2 ns |  4,406.28 ns |  2,304.57 ns |  0.98 |    0.03 |    3 |         - |          NA |
| QuickSortMedian9             | 8192 | Random             |   506,582.4 ns |  4,065.34 ns |  2,126.25 ns |  1.06 |    0.03 |    3 |         - |          NA |
| DualPivotQuickSort           | 8192 | Random             |   332,041.5 ns |  4,046.06 ns |  2,116.17 ns |  0.70 |    0.02 |    3 |         - |          NA |
| StableQuickSort              | 8192 | Random             | 1,310,877.4 ns |  8,122.39 ns |  3,606.39 ns |  2.75 |    0.08 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Random             | 1,050,427.8 ns |  1,992.19 ns |  1,041.95 ns |  2.20 |    0.06 |    5 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Random             |   860,479.2 ns |  3,697.95 ns |  1,641.91 ns |  1.81 |    0.05 |    4 |         - |          NA |
| IntroSort                    | 8192 | Random             |   395,053.2 ns |  1,461.64 ns |    764.47 ns |  0.83 |    0.02 |    3 |         - |          NA |
| IntroSortDotnet              | 8192 | Random             |   329,027.5 ns | 19,993.19 ns | 10,456.82 ns |  0.69 |    0.03 |    3 |         - |          NA |
| PDQSort                      | 8192 | Random             |   331,320.6 ns |  3,191.49 ns |  1,417.04 ns |  0.70 |    0.02 |    3 |         - |          NA |
| PDQSortBranchless            | 8192 | Random             |   138,133.9 ns |  2,837.50 ns |  1,484.07 ns |  0.29 |    0.01 |    1 |         - |          NA |
| Ipnsort                      | 8192 | Random             |   213,877.2 ns |  1,261.75 ns |    659.92 ns |  0.45 |    0.01 |    2 |         - |          NA |
| StdSort                      | 8192 | Random             |   133,549.9 ns |    874.57 ns |    388.32 ns |  0.28 |    0.01 |    1 |         - |          NA |
| BlockQuickSort               | 8192 | Random             |   151,836.2 ns |  2,032.15 ns |  1,062.85 ns |  0.32 |    0.01 |    1 |         - |          NA |
| DotnetSort                   | 8192 | Random             |   339,577.6 ns |  5,189.71 ns |  2,714.32 ns |  0.71 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **SingleElementMoved** |    **53,664.4 ns** |  **1,460.21 ns** |    **763.72 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 8192 | SingleElementMoved |    59,085.0 ns |  3,466.48 ns |  1,813.04 ns |  1.10 |    0.04 |    1 |         - |          NA |
| QuickSortMedian3             | 8192 | SingleElementMoved |    74,666.9 ns |    664.62 ns |    295.10 ns |  1.39 |    0.02 |    1 |         - |          NA |
| QuickSortMedian9             | 8192 | SingleElementMoved |    99,387.9 ns |  2,173.87 ns |    965.21 ns |  1.85 |    0.03 |    2 |         - |          NA |
| DualPivotQuickSort           | 8192 | SingleElementMoved |    49,443.8 ns |  1,761.67 ns |    921.39 ns |  0.92 |    0.02 |    1 |         - |          NA |
| StableQuickSort              | 8192 | SingleElementMoved |   449,768.2 ns |    734.71 ns |    326.22 ns |  8.38 |    0.11 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | SingleElementMoved |   266,363.9 ns |  1,269.93 ns |    563.86 ns |  4.96 |    0.07 |    4 |         - |          NA |
| DestswapStableQuickSort      | 8192 | SingleElementMoved |   202,938.3 ns |  3,509.53 ns |  1,835.55 ns |  3.78 |    0.06 |    3 |         - |          NA |
| IntroSort                    | 8192 | SingleElementMoved |    39,974.0 ns |  3,792.17 ns |  1,983.38 ns |  0.75 |    0.04 |    1 |         - |          NA |
| IntroSortDotnet              | 8192 | SingleElementMoved |    60,141.4 ns |    727.30 ns |    380.39 ns |  1.12 |    0.02 |    1 |         - |          NA |
| PDQSort                      | 8192 | SingleElementMoved |    43,811.1 ns |    154.63 ns |     68.65 ns |  0.82 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 8192 | SingleElementMoved |    54,356.9 ns |    862.58 ns |    451.15 ns |  1.01 |    0.02 |    1 |         - |          NA |
| Ipnsort                      | 8192 | SingleElementMoved |   192,338.8 ns |  1,659.31 ns |    867.85 ns |  3.58 |    0.05 |    3 |         - |          NA |
| StdSort                      | 8192 | SingleElementMoved |    64,389.9 ns |  1,261.57 ns |    659.83 ns |  1.20 |    0.02 |    1 |         - |          NA |
| BlockQuickSort               | 8192 | SingleElementMoved |    95,318.4 ns |  1,250.89 ns |    555.40 ns |  1.78 |    0.03 |    2 |         - |          NA |
| DotnetSort                   | 8192 | SingleElementMoved |    60,067.6 ns |  3,382.21 ns |  1,768.96 ns |  1.12 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Sorted**             |    **41,631.6 ns** |    **707.73 ns** |    **370.16 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Sorted             |    39,243.1 ns |    297.67 ns |    106.15 ns |  0.94 |    0.01 |    3 |         - |          NA |
| QuickSortMedian3             | 8192 | Sorted             |    53,982.8 ns |    819.70 ns |    363.95 ns |  1.30 |    0.01 |    3 |         - |          NA |
| QuickSortMedian9             | 8192 | Sorted             |    58,060.3 ns |    950.80 ns |    422.16 ns |  1.39 |    0.02 |    3 |         - |          NA |
| DualPivotQuickSort           | 8192 | Sorted             |    45,035.2 ns |    727.38 ns |    380.44 ns |  1.08 |    0.01 |    3 |         - |          NA |
| StableQuickSort              | 8192 | Sorted             |   493,322.1 ns |  1,821.65 ns |    952.76 ns | 11.85 |    0.10 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Sorted             |   231,479.9 ns |  6,056.00 ns |  3,167.41 ns |  5.56 |    0.09 |    5 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Sorted             |   195,505.4 ns |  2,867.63 ns |  1,499.83 ns |  4.70 |    0.05 |    5 |         - |          NA |
| IntroSort                    | 8192 | Sorted             |     7,831.7 ns |    556.88 ns |    247.26 ns |  0.19 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 8192 | Sorted             |    47,800.6 ns |    585.59 ns |    260.00 ns |  1.15 |    0.01 |    3 |         - |          NA |
| PDQSort                      | 8192 | Sorted             |    10,703.8 ns |    141.76 ns |     62.94 ns |  0.26 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | Sorted             |    10,629.2 ns |     78.08 ns |     40.84 ns |  0.26 |    0.00 |    2 |         - |          NA |
| Ipnsort                      | 8192 | Sorted             |     4,791.6 ns |    564.03 ns |    295.00 ns |  0.12 |    0.01 |    1 |         - |          NA |
| StdSort                      | 8192 | Sorted             |     8,944.9 ns |    339.03 ns |    150.53 ns |  0.21 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | Sorted             |    78,658.1 ns |  1,177.15 ns |    615.67 ns |  1.89 |    0.02 |    4 |         - |          NA |
| DotnetSort                   | 8192 | Sorted             |    44,630.0 ns |  7,826.51 ns |  4,093.42 ns |  1.07 |    0.09 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Reversed**           |    **46,915.9 ns** |    **998.16 ns** |    **443.19 ns** |  **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Reversed           |    50,901.1 ns |  4,623.69 ns |  2,418.28 ns |  1.09 |    0.05 |    4 |         - |          NA |
| QuickSortMedian3             | 8192 | Reversed           |    56,663.2 ns |  1,394.99 ns |    729.61 ns |  1.21 |    0.02 |    4 |         - |          NA |
| QuickSortMedian9             | 8192 | Reversed           |    60,906.3 ns |  1,003.48 ns |    524.84 ns |  1.30 |    0.02 |    4 |         - |          NA |
| DualPivotQuickSort           | 8192 | Reversed           |    54,493.3 ns |  1,998.75 ns |  1,045.39 ns |  1.16 |    0.02 |    4 |         - |          NA |
| StableQuickSort              | 8192 | Reversed           |   448,262.2 ns |  1,515.70 ns |    792.74 ns |  9.56 |    0.09 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Reversed           |   256,949.5 ns |  7,200.71 ns |  3,766.11 ns |  5.48 |    0.09 |    6 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Reversed           |   308,021.4 ns |  2,060.06 ns |  1,077.45 ns |  6.57 |    0.06 |    7 |         - |          NA |
| IntroSort                    | 8192 | Reversed           |    26,565.8 ns |    572.28 ns |    299.32 ns |  0.57 |    0.01 |    3 |         - |          NA |
| IntroSortDotnet              | 8192 | Reversed           |    75,087.7 ns |    437.26 ns |    228.70 ns |  1.60 |    0.01 |    5 |         - |          NA |
| PDQSort                      | 8192 | Reversed           |    16,936.6 ns |    825.59 ns |    431.80 ns |  0.36 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | Reversed           |    25,602.8 ns |  1,317.68 ns |    689.17 ns |  0.55 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 8192 | Reversed           |     7,338.2 ns |    399.51 ns |    208.95 ns |  0.16 |    0.00 |    1 |         - |          NA |
| StdSort                      | 8192 | Reversed           |    22,618.1 ns |    658.16 ns |    292.23 ns |  0.48 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 8192 | Reversed           |    85,786.1 ns |    552.13 ns |    288.77 ns |  1.83 |    0.02 |    5 |         - |          NA |
| DotnetSort                   | 8192 | Reversed           |    93,727.3 ns | 10,565.50 ns |  5,525.96 ns |  2.00 |    0.11 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **PipeOrgan**          | **6,159,169.5 ns** | **10,748.93 ns** |  **3,833.17 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| QuickSort3way                | 8192 | PipeOrgan          |   198,516.2 ns | 12,132.69 ns |  6,345.63 ns |  0.03 |    0.00 |    4 |         - |          NA |
| QuickSortMedian3             | 8192 | PipeOrgan          |   195,757.3 ns |  5,898.23 ns |  3,084.89 ns |  0.03 |    0.00 |    4 |         - |          NA |
| QuickSortMedian9             | 8192 | PipeOrgan          |   117,226.8 ns |  6,170.23 ns |  3,227.15 ns |  0.02 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort           | 8192 | PipeOrgan          |    85,899.1 ns |  2,961.42 ns |  1,548.88 ns |  0.01 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 8192 | PipeOrgan          |   450,620.1 ns |  1,387.01 ns |    615.84 ns |  0.07 |    0.00 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | PipeOrgan          |   254,744.6 ns |  3,691.09 ns |  1,930.51 ns |  0.04 |    0.00 |    4 |         - |          NA |
| DestswapStableQuickSort      | 8192 | PipeOrgan          |   364,450.3 ns |  3,159.04 ns |  1,652.24 ns |  0.06 |    0.00 |    5 |         - |          NA |
| IntroSort                    | 8192 | PipeOrgan          |   200,882.4 ns | 10,184.03 ns |  5,326.44 ns |  0.03 |    0.00 |    4 |         - |          NA |
| IntroSortDotnet              | 8192 | PipeOrgan          |   344,977.3 ns |  7,195.53 ns |  3,763.40 ns |  0.06 |    0.00 |    5 |         - |          NA |
| PDQSort                      | 8192 | PipeOrgan          |    90,844.2 ns |  1,652.38 ns |    733.67 ns |  0.01 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 8192 | PipeOrgan          |   162,471.3 ns |  3,124.89 ns |  1,634.38 ns |  0.03 |    0.00 |    3 |         - |          NA |
| Ipnsort                      | 8192 | PipeOrgan          |   236,361.1 ns |    639.27 ns |    334.35 ns |  0.04 |    0.00 |    4 |         - |          NA |
| StdSort                      | 8192 | PipeOrgan          |   283,614.3 ns |  3,944.05 ns |  1,751.18 ns |  0.05 |    0.00 |    4 |         - |          NA |
| BlockQuickSort               | 8192 | PipeOrgan          |   225,040.2 ns |  1,147.86 ns |    600.35 ns |  0.04 |    0.00 |    4 |         - |          NA |
| DotnetSort                   | 8192 | PipeOrgan          |   353,566.3 ns |  5,767.69 ns |  3,016.61 ns |  0.06 |    0.00 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **ManyDuplicates**     |    **99,632.9 ns** |  **7,108.92 ns** |  **3,718.10 ns** |  **1.00** |    **0.05** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 8192 | ManyDuplicates     |    72,134.1 ns | 13,514.24 ns |  7,068.21 ns |  0.72 |    0.07 |    2 |         - |          NA |
| QuickSortMedian3             | 8192 | ManyDuplicates     |   114,261.2 ns |  3,040.74 ns |  1,350.11 ns |  1.15 |    0.04 |    3 |         - |          NA |
| QuickSortMedian9             | 8192 | ManyDuplicates     |   120,529.9 ns |  1,774.03 ns |    787.68 ns |  1.21 |    0.04 |    3 |         - |          NA |
| DualPivotQuickSort           | 8192 | ManyDuplicates     |    62,067.4 ns |  6,422.98 ns |  3,359.34 ns |  0.62 |    0.04 |    2 |         - |          NA |
| StableQuickSort              | 8192 | ManyDuplicates     |   464,107.8 ns |  3,286.25 ns |  1,459.11 ns |  4.66 |    0.16 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | ManyDuplicates     |   241,233.9 ns |  5,752.79 ns |  2,554.27 ns |  2.42 |    0.09 |    4 |         - |          NA |
| DestswapStableQuickSort      | 8192 | ManyDuplicates     |   118,376.3 ns |  4,968.11 ns |  2,598.42 ns |  1.19 |    0.05 |    3 |         - |          NA |
| IntroSort                    | 8192 | ManyDuplicates     |   112,474.0 ns |  3,214.50 ns |  1,427.26 ns |  1.13 |    0.04 |    3 |         - |          NA |
| IntroSortDotnet              | 8192 | ManyDuplicates     |    81,528.5 ns |  1,328.97 ns |    590.07 ns |  0.82 |    0.03 |    2 |         - |          NA |
| PDQSort                      | 8192 | ManyDuplicates     |    44,393.6 ns |  1,269.61 ns |    563.71 ns |  0.45 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless            | 8192 | ManyDuplicates     |    61,584.6 ns |  8,727.40 ns |  3,875.02 ns |  0.62 |    0.04 |    2 |         - |          NA |
| Ipnsort                      | 8192 | ManyDuplicates     |   118,213.6 ns |    472.52 ns |    247.14 ns |  1.19 |    0.04 |    3 |         - |          NA |
| StdSort                      | 8192 | ManyDuplicates     |    63,164.6 ns |  1,526.42 ns |    798.35 ns |  0.63 |    0.02 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | ManyDuplicates     |   103,175.6 ns |    913.25 ns |    477.65 ns |  1.04 |    0.04 |    3 |         - |          NA |
| DotnetSort                   | 8192 | ManyDuplicates     |    78,852.7 ns |  1,335.64 ns |    593.03 ns |  0.79 |    0.03 |    2 |         - |          NA |

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

| Method              | Size | RadixDigits | Mean         | Error        | StdDev      | Ratio | RatioSD | Allocated | Alloc Ratio |
| -------------------- |----- |------------ |-------------:|-------------:|------------:|------:|--------:|----------:|------------:|
| **Lsd256_CountPerPass** | **1024** | **1**           |   **3,908.3 ns** |    **282.82 ns** |   **125.57 ns** |  **1.00** |    **0.04** |         **-** |          **NA** |
| Lsd256_Histogram    | 1024 | 1           |   5,034.5 ns |    443.64 ns |   232.03 ns |  1.29 |    0.07 |         - |          NA |
| Lsd10_CountPerPass  | 1024 | 1           |  16,871.9 ns |    134.96 ns |    70.59 ns |  4.32 |    0.13 |         - |          NA |
| Lsd10_Histogram     | 1024 | 1           |  16,633.6 ns |     65.56 ns |    29.11 ns |  4.26 |    0.12 |         - |          NA |
| Lsd10_Quotient      | 1024 | 1           |  36,399.0 ns |  1,108.00 ns |   579.50 ns |  9.32 |    0.31 |         - |          NA |
|      |             |              |              |             |       |         |           |             |
| **Lsd256_CountPerPass** | **1024** | **2**           |   **6,462.7 ns** |    **366.82 ns** |   **191.85 ns** |  **1.00** |    **0.04** |         **-** |          **NA** |
| Lsd256_Histogram    | 1024 | 2           |   6,838.7 ns |    255.17 ns |   133.46 ns |  1.06 |    0.03 |         - |          NA |
| Lsd10_CountPerPass  | 1024 | 2           |  27,234.2 ns |    376.99 ns |   197.17 ns |  4.22 |    0.12 |         - |          NA |
| Lsd10_Histogram     | 1024 | 2           |  27,523.6 ns |    178.16 ns |    93.18 ns |  4.26 |    0.12 |         - |          NA |
| Lsd10_Quotient      | 1024 | 2           |  27,193.1 ns |    497.52 ns |   260.21 ns |  4.21 |    0.12 |         - |          NA |
|      |             |              |              |             |       |         |           |             |
| **Lsd256_CountPerPass** | **1024** | **3**           |   **9,084.0 ns** |    **397.93 ns** |   **208.12 ns** |  **1.00** |    **0.03** |         **-** |          **NA** |
| Lsd256_Histogram    | 1024 | 3           |   9,208.9 ns |    428.80 ns |   190.39 ns |  1.01 |    0.03 |         - |          NA |
| Lsd10_CountPerPass  | 1024 | 3           |  42,618.2 ns |    281.80 ns |   125.12 ns |  4.69 |    0.10 |         - |          NA |
| Lsd10_Histogram     | 1024 | 3           |  41,966.3 ns |    367.22 ns |   192.06 ns |  4.62 |    0.10 |         - |          NA |
| Lsd10_Quotient      | 1024 | 3           |  44,014.3 ns |  3,719.34 ns | 1,945.29 ns |  4.85 |    0.23 |         - |          NA |
|      |             |              |              |             |       |         |           |             |
| **Lsd256_CountPerPass** | **1024** | **4**           |  **12,767.5 ns** |  **1,673.34 ns** |   **875.19 ns** |  **1.00** |    **0.09** |         **-** |          **NA** |
| Lsd256_Histogram    | 1024 | 4           |  10,964.1 ns |    322.43 ns |   168.64 ns |  0.86 |    0.06 |         - |          NA |
| Lsd10_CountPerPass  | 1024 | 4           |  53,301.5 ns |    164.70 ns |    73.13 ns |  4.19 |    0.26 |         - |          NA |
| Lsd10_Histogram     | 1024 | 4           |  52,668.2 ns |    244.00 ns |   127.62 ns |  4.14 |    0.26 |         - |          NA |
| Lsd10_Quotient      | 1024 | 4           |  59,408.4 ns |  1,399.21 ns |   621.26 ns |  4.67 |    0.30 |         - |          NA |
|      |             |              |              |             |       |         |           |             |
| **Lsd256_CountPerPass** | **8192** | **1**           |  **29,678.6 ns** |  **1,414.70 ns** |   **739.91 ns** |  **1.00** |    **0.03** |         **-** |          **NA** |
| Lsd256_Histogram    | 8192 | 1           |  37,026.7 ns |  1,158.83 ns |   606.09 ns |  1.25 |    0.03 |         - |          NA |
| Lsd10_CountPerPass  | 8192 | 1           | 133,944.5 ns |    973.24 ns |   509.02 ns |  4.52 |    0.10 |         - |          NA |
| Lsd10_Histogram     | 8192 | 1           | 130,791.6 ns |  1,499.05 ns |   784.03 ns |  4.41 |    0.10 |         - |          NA |
| Lsd10_Quotient      | 8192 | 1           | 285,152.6 ns |  8,504.09 ns | 4,447.80 ns |  9.61 |    0.26 |         - |          NA |
|      |             |              |              |             |       |         |           |             |
| **Lsd256_CountPerPass** | **8192** | **2**           |  **47,722.8 ns** |    **585.86 ns** |   **260.12 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| Lsd256_Histogram    | 8192 | 2           |  51,969.5 ns |  1,206.74 ns |   535.80 ns |  1.09 |    0.01 |         - |          NA |
| Lsd10_CountPerPass  | 8192 | 2           | 215,599.9 ns |    889.69 ns |   465.33 ns |  4.52 |    0.02 |         - |          NA |
| Lsd10_Histogram     | 8192 | 2           | 211,836.4 ns |  1,619.24 ns |   846.90 ns |  4.44 |    0.03 |         - |          NA |
| Lsd10_Quotient      | 8192 | 2           | 479,723.2 ns |  4,365.26 ns | 2,283.11 ns | 10.05 |    0.07 |         - |          NA |
|      |             |              |              |             |       |         |           |             |
| **Lsd256_CountPerPass** | **8192** | **3**           |  **68,040.3 ns** |    **648.31 ns** |   **287.86 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| Lsd256_Histogram    | 8192 | 3           |  70,659.1 ns |    832.22 ns |   369.51 ns |  1.04 |    0.01 |         - |          NA |
| Lsd10_CountPerPass  | 8192 | 3           | 338,358.6 ns |    790.09 ns |   350.81 ns |  4.97 |    0.02 |         - |          NA |
| Lsd10_Histogram     | 8192 | 3           | 330,993.5 ns |    967.01 ns |   505.77 ns |  4.86 |    0.02 |         - |          NA |
| Lsd10_Quotient      | 8192 | 3           | 728,679.6 ns | 15,231.44 ns | 7,966.34 ns | 10.71 |    0.12 |         - |          NA |
|      |             |              |              |             |       |         |           |             |
| **Lsd256_CountPerPass** | **8192** | **4**           |  **89,065.8 ns** |    **839.30 ns** |   **438.97 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| Lsd256_Histogram    | 8192 | 4           |  82,419.9 ns |  1,066.63 ns |   557.87 ns |  0.93 |    0.01 |         - |          NA |
| Lsd10_CountPerPass  | 8192 | 4           | 429,701.0 ns |  2,343.67 ns | 1,225.78 ns |  4.82 |    0.03 |         - |          NA |
| Lsd10_Histogram     | 8192 | 4           | 419,539.4 ns |    769.42 ns |   341.63 ns |  4.71 |    0.02 |         - |          NA |
| Lsd10_Quotient      | 8192 | 4           | 924,273.9 ns | 16,831.66 ns | 7,473.36 ns | 10.38 |    0.09 |         - |          NA |

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

| Method        | Size | Stride | Mean         | Error       | StdDev      | Ratio | Allocated | Alloc Ratio |
| -------------- |----- |------- |-------------:|------------:|------------:|------:|----------:|------------:|
| **Lsd4_NoSkip**   | **1024** | **1**      |  **18,032.7 ns** |   **231.89 ns** |   **121.28 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_Skip     | 1024 | 1      |  13,921.4 ns |   261.51 ns |   136.78 ns |  0.77 |         - |          NA |
| Lsd256_NoSkip | 1024 | 1      |   6,959.0 ns |   162.45 ns |    84.97 ns |  0.39 |         - |          NA |
| Lsd256_Skip   | 1024 | 1      |   7,065.6 ns |    51.78 ns |    18.46 ns |  0.39 |         - |          NA |
| Lsd10_NoSkip  | 1024 | 1      |  21,126.1 ns |   204.07 ns |    90.61 ns |  1.17 |         - |          NA |
| Lsd10_Skip    | 1024 | 1      |  21,335.0 ns |   248.52 ns |   110.34 ns |  1.18 |         - |          NA |
|      |        |              |             |             |       |           |             |
| **Lsd4_NoSkip**   | **1024** | **65536**  |  **41,346.7 ns** |   **170.34 ns** |    **89.09 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_Skip     | 1024 | 65536  |  23,440.3 ns |   287.76 ns |   150.50 ns |  0.57 |         - |          NA |
| Lsd256_NoSkip | 1024 | 65536  |  12,006.1 ns |   274.97 ns |   143.82 ns |  0.29 |         - |          NA |
| Lsd256_Skip   | 1024 | 65536  |   9,205.9 ns |   243.58 ns |   127.40 ns |  0.22 |         - |          NA |
| Lsd10_NoSkip  | 1024 | 65536  |  41,742.2 ns |   331.17 ns |   173.21 ns |  1.01 |         - |          NA |
| Lsd10_Skip    | 1024 | 65536  |  41,938.1 ns |   223.45 ns |   116.87 ns |  1.01 |         - |          NA |
|      |        |              |             |             |       |           |             |
| **Lsd4_NoSkip**   | **8192** | **1**      | **194,610.4 ns** | **1,338.43 ns** |   **594.27 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_Skip     | 8192 | 1      | 155,623.6 ns |   697.39 ns |   364.75 ns |  0.80 |         - |          NA |
| Lsd256_NoSkip | 8192 | 1      |  51,527.9 ns |   794.59 ns |   352.80 ns |  0.26 |         - |          NA |
| Lsd256_Skip   | 8192 | 1      |  51,186.8 ns | 1,040.38 ns |   544.14 ns |  0.26 |         - |          NA |
| Lsd10_NoSkip  | 8192 | 1      | 168,835.8 ns | 1,054.45 ns |   551.50 ns |  0.87 |         - |          NA |
| Lsd10_Skip    | 8192 | 1      | 172,296.0 ns | 2,839.43 ns | 1,485.07 ns |  0.89 |         - |          NA |
|      |        |              |             |             |       |           |             |
| **Lsd4_NoSkip**   | **8192** | **65536**  | **390,391.7 ns** | **1,086.77 ns** |   **568.40 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_Skip     | 8192 | 65536  | 223,064.7 ns | 2,241.20 ns | 1,172.19 ns |  0.57 |         - |          NA |
| Lsd256_NoSkip | 8192 | 65536  |  87,762.7 ns | 1,079.49 ns |   479.30 ns |  0.22 |         - |          NA |
| Lsd256_Skip   | 8192 | 65536  |  67,681.5 ns | 1,010.98 ns |   528.76 ns |  0.17 |         - |          NA |
| Lsd10_NoSkip  | 8192 | 65536  | 375,865.6 ns |   850.35 ns |   444.75 ns |  0.96 |         - |          NA |
| Lsd10_Skip    | 8192 | 65536  | 376,477.6 ns |   816.69 ns |   427.15 ns |  0.96 |         - |          NA |

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

| Method         | Size  | FullRange | Mean           | Error       | StdDev      | Ratio | Allocated | Alloc Ratio |
| --------------- |------ |---------- |---------------:|------------:|------------:|------:|----------:|------------:|
| **Lsd4_Recompute** | **1024**  | **False**     |    **14,272.7 ns** |    **157.2 ns** |    **82.23 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_KeyCache  | 1024  | False     |    19,800.6 ns |    186.8 ns |    82.93 ns |  1.39 |         - |          NA |
|       |           |                |             |             |       |           |             |
| **Lsd4_Recompute** | **1024**  | **True**      |    **41,553.2 ns** |    **200.4 ns** |    **71.47 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_KeyCache  | 1024  | True      |    53,502.5 ns |    329.7 ns |   172.42 ns |  1.29 |         - |          NA |
|       |           |                |             |             |       |           |             |
| **Lsd4_Recompute** | **8192**  | **False**     |   **152,435.7 ns** |  **1,345.7 ns** |   **703.83 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_KeyCache  | 8192  | False     |   201,357.5 ns |  1,219.9 ns |   638.04 ns |  1.32 |         - |          NA |
|       |           |                |             |             |       |           |             |
| **Lsd4_Recompute** | **8192**  | **True**      |   **332,071.3 ns** |    **942.8 ns** |   **493.11 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_KeyCache  | 8192  | True      |   440,744.7 ns |  1,457.7 ns |   762.41 ns |  1.33 |         - |          NA |
|       |           |                |             |             |       |           |             |
| **Lsd4_Recompute** | **65536** | **False**     | **1,363,649.3 ns** |  **4,579.5 ns** | **2,395.15 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_KeyCache  | 65536 | False     | 1,830,054.9 ns |  3,487.4 ns | 1,823.99 ns |  1.34 |         - |          NA |
|       |           |                |             |             |       |           |             |
| **Lsd4_Recompute** | **65536** | **True**      | **2,672,944.6 ns** |  **4,965.5 ns** | **2,204.69 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_KeyCache  | 65536 | True      | 3,502,782.4 ns | 13,067.1 ns | 5,801.88 ns |  1.31 |         - |          NA |

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
| **Lsd4_Xor**          | **1024** | **False**         |  **21,490.6 ns** |   **209.91 ns** |    **74.86 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_Normalized   | 1024 | False         |  14,204.3 ns |   201.95 ns |   105.62 ns |  0.66 |         - |          NA |
| Lsd256_Xor        | 1024 | False         |   6,207.9 ns |   170.30 ns |    75.61 ns |  0.29 |         - |          NA |
| Lsd256_Normalized | 1024 | False         |   6,598.2 ns |   155.12 ns |    81.13 ns |  0.31 |         - |          NA |
| Lsd10_CopyBack    | 1024 | False         |  22,143.1 ns |   133.80 ns |    59.41 ns |  1.03 |         - |          NA |
| Lsd10_PingPong    | 1024 | False         |  21,611.7 ns |   243.54 ns |   108.13 ns |  1.01 |         - |          NA |
|      |               |              |             |             |       |           |             |
| **Lsd4_Xor**          | **1024** | **True**          |  **50,597.4 ns** |   **352.80 ns** |   **156.65 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_Normalized   | 1024 | True          |  14,051.9 ns |    85.16 ns |    44.54 ns |  0.28 |         - |          NA |
| Lsd256_Xor        | 1024 | True          |  11,158.8 ns |   681.72 ns |   302.69 ns |  0.22 |         - |          NA |
| Lsd256_Normalized | 1024 | True          |   6,588.3 ns |   131.54 ns |    58.41 ns |  0.13 |         - |          NA |
| Lsd10_CopyBack    | 1024 | True          |  22,266.7 ns |   196.50 ns |   102.77 ns |  0.44 |         - |          NA |
| Lsd10_PingPong    | 1024 | True          |  21,420.6 ns |   278.86 ns |   145.85 ns |  0.42 |         - |          NA |
|      |               |              |             |             |       |           |             |
| **Lsd4_Xor**          | **8192** | **False**         | **196,664.3 ns** | **1,488.16 ns** |   **660.75 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_Normalized   | 8192 | False         | 153,091.2 ns |   901.63 ns |   400.33 ns |  0.78 |         - |          NA |
| Lsd256_Xor        | 8192 | False         |  47,770.6 ns | 1,229.82 ns |   643.22 ns |  0.24 |         - |          NA |
| Lsd256_Normalized | 8192 | False         |  49,182.1 ns |   968.44 ns |   506.52 ns |  0.25 |         - |          NA |
| Lsd10_CopyBack    | 8192 | False         | 177,568.7 ns | 1,185.06 ns |   526.18 ns |  0.90 |         - |          NA |
| Lsd10_PingPong    | 8192 | False         | 169,084.3 ns | 2,820.80 ns | 1,475.33 ns |  0.86 |         - |          NA |
|      |               |              |             |             |       |           |             |
| **Lsd4_Xor**          | **8192** | **True**          | **417,856.8 ns** | **2,246.21 ns** |   **997.33 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_Normalized   | 8192 | True          | 153,966.0 ns |   512.37 ns |   227.50 ns |  0.37 |         - |          NA |
| Lsd256_Xor        | 8192 | True          |  79,982.2 ns |   704.64 ns |   368.54 ns |  0.19 |         - |          NA |
| Lsd256_Normalized | 8192 | True          |  48,235.5 ns |   839.22 ns |   438.93 ns |  0.12 |         - |          NA |
| Lsd10_CopyBack    | 8192 | True          | 178,860.0 ns |   855.28 ns |   447.33 ns |  0.43 |         - |          NA |
| Lsd10_PingPong    | 8192 | True          | 168,905.1 ns | 3,117.79 ns | 1,630.67 ns |  0.40 |         - |          NA |

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

| Method              | Size | Pattern            | Mean           | Error       | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------------- |----- |------------------- |---------------:|------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **SelectionSort**       | **256**  | **Random**             |    **24,652.4 ns** |    **542.0 ns** |    **240.65 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Random             |    18,502.3 ns |    124.0 ns |     55.08 ns |  0.75 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | Random             |    72,848.8 ns |    970.6 ns |    430.96 ns |  2.96 |    0.03 |    4 |         - |          NA |
| PancakeSort         | 256  | Random             |    41,256.1 ns |    509.1 ns |    266.29 ns |  1.67 |    0.02 |    3 |         - |          NA |
|      |                    |                |             |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **SingleElementMoved** |    **24,503.6 ns** |    **331.0 ns** |    **146.98 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | SingleElementMoved |    22,951.9 ns |    215.5 ns |    112.72 ns |  0.94 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | SingleElementMoved |    56,714.2 ns |  1,628.9 ns |    851.93 ns |  2.31 |    0.04 |    2 |         - |          NA |
| PancakeSort         | 256  | SingleElementMoved |    21,690.5 ns |  2,546.7 ns |  1,331.95 ns |  0.89 |    0.05 |    1 |         - |          NA |
|      |                    |                |             |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **Sorted**             |    **24,912.9 ns** |    **850.0 ns** |    **377.39 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Sorted             |    15,293.9 ns |  8,372.1 ns |  4,378.76 ns |  0.61 |    0.17 |    1 |         - |          NA |
| CycleSort           | 256  | Sorted             |    24,582.1 ns |    274.2 ns |    143.44 ns |  0.99 |    0.01 |    3 |         - |          NA |
| PancakeSort         | 256  | Sorted             |    17,702.5 ns |  1,883.8 ns |    985.24 ns |  0.71 |    0.04 |    2 |         - |          NA |
|      |                    |                |             |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **Reversed**           |    **22,620.4 ns** |  **2,581.3 ns** |  **1,350.09 ns** |  **1.00** |    **0.08** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Reversed           |    18,314.7 ns |    289.1 ns |    151.20 ns |  0.81 |    0.04 |    1 |         - |          NA |
| CycleSort           | 256  | Reversed           |    49,726.6 ns |    206.8 ns |    108.14 ns |  2.20 |    0.12 |    2 |         - |          NA |
| PancakeSort         | 256  | Reversed           |    17,050.9 ns |    143.4 ns |     63.65 ns |  0.76 |    0.04 |    1 |         - |          NA |
|      |                    |                |             |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **PipeOrgan**          |    **23,738.8 ns** |    **751.3 ns** |    **392.93 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | PipeOrgan          |    21,660.8 ns |    259.9 ns |    135.93 ns |  0.91 |    0.02 |    1 |         - |          NA |
| CycleSort           | 256  | PipeOrgan          |    69,778.6 ns |  1,910.3 ns |    999.10 ns |  2.94 |    0.06 |    3 |         - |          NA |
| PancakeSort         | 256  | PipeOrgan          |    37,178.1 ns |    486.2 ns |    215.88 ns |  1.57 |    0.03 |    2 |         - |          NA |
|      |                    |                |             |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **ManyDuplicates**     |    **24,510.0 ns** |    **499.0 ns** |    **221.55 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | ManyDuplicates     |    18,408.9 ns |    168.3 ns |     88.03 ns |  0.75 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | ManyDuplicates     |    69,498.9 ns |    775.1 ns |    344.16 ns |  2.84 |    0.03 |    4 |         - |          NA |
| PancakeSort         | 256  | ManyDuplicates     |    38,745.4 ns |    378.8 ns |    198.11 ns |  1.58 |    0.02 |    3 |         - |          NA |
|      |                    |                |             |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Random**             |   **405,586.1 ns** |  **1,068.9 ns** |    **559.06 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Random             |   311,361.6 ns | 40,775.6 ns | 18,104.63 ns |  0.77 |    0.04 |    1 |         - |          NA |
| CycleSort           | 1024 | Random             | 1,550,044.2 ns | 11,534.1 ns |  6,032.54 ns |  3.82 |    0.01 |    4 |         - |          NA |
| PancakeSort         | 1024 | Random             |   690,014.7 ns |  1,850.7 ns |    967.95 ns |  1.70 |    0.00 |    3 |         - |          NA |
|      |                    |                |             |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **SingleElementMoved** |   **375,810.1 ns** |    **687.7 ns** |    **359.66 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | SingleElementMoved |   355,451.2 ns |  2,548.9 ns |  1,131.74 ns |  0.95 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | SingleElementMoved |   885,714.5 ns | 20,699.5 ns |  9,190.72 ns |  2.36 |    0.02 |    2 |         - |          NA |
| PancakeSort         | 1024 | SingleElementMoved |   346,731.0 ns | 16,385.0 ns |  8,569.65 ns |  0.92 |    0.02 |    1 |         - |          NA |
|      |                    |                |             |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Sorted**             |   **376,185.9 ns** |  **1,875.8 ns** |    **981.07 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Sorted             |   188,644.1 ns |    634.0 ns |    331.60 ns |  0.50 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | Sorted             |   375,492.6 ns |  1,973.3 ns |    703.69 ns |  1.00 |    0.00 |    3 |         - |          NA |
| PancakeSort         | 1024 | Sorted             |   253,248.1 ns |    984.4 ns |    514.86 ns |  0.67 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Reversed**           |   **343,042.9 ns** |  **5,957.5 ns** |  **3,115.89 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Reversed           |   280,687.3 ns |    187.2 ns |     83.11 ns |  0.82 |    0.01 |    1 |         - |          NA |
| CycleSort           | 1024 | Reversed           |   758,812.7 ns |  2,876.2 ns |  1,504.32 ns |  2.21 |    0.02 |    3 |         - |          NA |
| PancakeSort         | 1024 | Reversed           |   269,239.9 ns | 12,329.0 ns |  6,448.32 ns |  0.78 |    0.02 |    1 |         - |          NA |
|      |                    |                |             |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **PipeOrgan**          |   **389,185.8 ns** |  **3,832.4 ns** |  **2,004.43 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | PipeOrgan          |   345,734.1 ns |    879.5 ns |    460.01 ns |  0.89 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | PipeOrgan          | 1,189,595.2 ns |  9,915.5 ns |  5,185.99 ns |  3.06 |    0.02 |    3 |         - |          NA |
| PancakeSort         | 1024 | PipeOrgan          |   566,595.7 ns |  2,193.3 ns |  1,147.15 ns |  1.46 |    0.01 |    2 |         - |          NA |
|      |                    |                |             |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **ManyDuplicates**     |   **395,545.5 ns** |  **2,860.2 ns** |  **1,495.96 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | ManyDuplicates     |   294,853.2 ns |  1,415.8 ns |    628.61 ns |  0.75 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | ManyDuplicates     | 1,530,812.3 ns |  3,658.5 ns |  1,304.65 ns |  3.87 |    0.01 |    4 |         - |          NA |
| PancakeSort         | 1024 | ManyDuplicates     |   634,817.0 ns |  1,644.6 ns |    860.17 ns |  1.60 |    0.01 |    3 |         - |          NA |

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

| Method                 | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **BalancedBinaryTreeSort** | **256**  | **Random**             |    **13,805.4 ns** |    **820.34 ns** |    **429.06 ns** |  **4.01** |    **0.18** |    **4** |         **-** |          **NA** |
| CartesianTreeSort      | 256  | Random             |     6,560.0 ns |    297.97 ns |    155.84 ns |  1.91 |    0.08 |    2 |         - |          NA |
| BinaryTreeSort         | 256  | Random             |     3,443.8 ns |    312.84 ns |    138.90 ns |  1.00 |    0.05 |    1 |         - |          NA |
| SplaySort              | 256  | Random             |    23,217.2 ns |    588.64 ns |    307.87 ns |  6.75 |    0.25 |    5 |         - |          NA |
| TreapSort              | 256  | Random             |     9,101.6 ns |    235.88 ns |    104.73 ns |  2.65 |    0.10 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **SingleElementMoved** |    **14,317.1 ns** |    **483.62 ns** |    **252.94 ns** |  **0.29** |    **0.01** |    **4** |         **-** |          **NA** |
| CartesianTreeSort      | 256  | SingleElementMoved |     2,394.6 ns |    226.10 ns |    100.39 ns |  0.05 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | SingleElementMoved |    48,998.9 ns |    469.00 ns |    245.29 ns |  1.00 |    0.01 |    5 |         - |          NA |
| SplaySort              | 256  | SingleElementMoved |     4,360.6 ns |     30.94 ns |     11.04 ns |  0.09 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | SingleElementMoved |     6,359.0 ns |    420.73 ns |    186.81 ns |  0.13 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Sorted**             |    **13,130.6 ns** |    **526.83 ns** |    **275.54 ns** |  **0.17** |    **0.00** |    **4** |         **-** |          **NA** |
| CartesianTreeSort      | 256  | Sorted             |     2,079.1 ns |     13.48 ns |      4.81 ns |  0.03 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | Sorted             |    76,141.9 ns |    332.24 ns |    173.77 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 256  | Sorted             |     3,915.8 ns |    249.69 ns |    130.59 ns |  0.05 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | Sorted             |     5,240.7 ns |    513.31 ns |    227.91 ns |  0.07 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Reversed**           |    **12,192.9 ns** |    **152.76 ns** |     **79.89 ns** |  **0.15** |    **0.00** |    **4** |         **-** |          **NA** |
| CartesianTreeSort      | 256  | Reversed           |     1,984.0 ns |     10.17 ns |      4.52 ns |  0.02 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | Reversed           |    79,809.0 ns |    239.33 ns |    125.18 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 256  | Reversed           |     3,771.4 ns |     53.09 ns |     18.93 ns |  0.05 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | Reversed           |     5,604.1 ns |    405.28 ns |    211.97 ns |  0.07 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **PipeOrgan**          |    **12,724.8 ns** |    **672.59 ns** |    **298.63 ns** |  **0.34** |    **0.01** |    **4** |         **-** |          **NA** |
| CartesianTreeSort      | 256  | PipeOrgan          |     2,510.4 ns |  1,184.09 ns |    525.74 ns |  0.07 |    0.01 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | PipeOrgan          |    37,422.4 ns |    546.94 ns |    242.84 ns |  1.00 |    0.01 |    5 |         - |          NA |
| SplaySort              | 256  | PipeOrgan          |     4,496.1 ns |     24.96 ns |      8.90 ns |  0.12 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | PipeOrgan          |     7,531.7 ns |    101.97 ns |     45.28 ns |  0.20 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **ManyDuplicates**     |    **13,886.6 ns** |    **708.26 ns** |    **370.43 ns** |  **3.46** |    **0.09** |    **3** |         **-** |          **NA** |
| CartesianTreeSort      | 256  | ManyDuplicates     |     7,266.0 ns |    544.65 ns |    284.86 ns |  1.81 |    0.07 |    2 |         - |          NA |
| BinaryTreeSort         | 256  | ManyDuplicates     |     4,008.2 ns |     33.05 ns |     11.79 ns |  1.00 |    0.00 |    1 |         - |          NA |
| SplaySort              | 256  | ManyDuplicates     |    21,969.5 ns |    408.74 ns |    213.78 ns |  5.48 |    0.05 |    4 |         - |          NA |
| TreapSort              | 256  | ManyDuplicates     |     8,206.0 ns |    281.54 ns |    125.01 ns |  2.05 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Random**             |    **75,179.4 ns** |  **6,880.28 ns** |  **3,054.89 ns** |  **3.81** |    **0.17** |    **4** |         **-** |          **NA** |
| CartesianTreeSort      | 1024 | Random             |    31,533.4 ns |    128.55 ns |     45.84 ns |  1.60 |    0.03 |    2 |         - |          NA |
| BinaryTreeSort         | 1024 | Random             |    19,734.6 ns |    867.02 ns |    453.47 ns |  1.00 |    0.03 |    1 |         - |          NA |
| SplaySort              | 1024 | Random             |   127,815.5 ns |  5,411.87 ns |  2,402.90 ns |  6.48 |    0.18 |    5 |         - |          NA |
| TreapSort              | 1024 | Random             |    40,086.0 ns |  2,502.01 ns |  1,308.60 ns |  2.03 |    0.08 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **SingleElementMoved** |    **79,051.7 ns** |  **6,590.61 ns** |  **3,447.02 ns** |  **0.10** |    **0.00** |    **4** |         **-** |          **NA** |
| CartesianTreeSort      | 1024 | SingleElementMoved |     8,982.7 ns |    271.16 ns |    141.82 ns |  0.01 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | SingleElementMoved |   778,768.1 ns |    451.52 ns |    200.48 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | SingleElementMoved |    17,585.8 ns |    239.96 ns |    106.55 ns |  0.02 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | SingleElementMoved |    27,048.4 ns |    989.98 ns |    517.78 ns |  0.03 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Sorted**             |    **69,539.0 ns** |  **5,822.91 ns** |  **2,585.41 ns** | **0.058** |    **0.00** |    **4** |         **-** |          **NA** |
| CartesianTreeSort      | 1024 | Sorted             |     8,217.5 ns |    182.17 ns |     95.28 ns | 0.007 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | Sorted             | 1,204,635.4 ns |    538.45 ns |    281.62 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | Sorted             |    15,405.8 ns |    217.75 ns |    113.89 ns | 0.013 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | Sorted             |    22,665.4 ns |    653.55 ns |    341.82 ns | 0.019 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Reversed**           |    **59,642.7 ns** |    **657.80 ns** |    **292.07 ns** | **0.047** |    **0.00** |    **4** |         **-** |          **NA** |
| CartesianTreeSort      | 1024 | Reversed           |     7,876.0 ns |     57.31 ns |     29.97 ns | 0.006 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | Reversed           | 1,277,603.2 ns |    930.64 ns |    486.74 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | Reversed           |    14,820.3 ns |    175.52 ns |     77.93 ns | 0.012 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | Reversed           |    24,148.8 ns |  1,011.82 ns |    529.20 ns | 0.019 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **PipeOrgan**          |    **68,710.3 ns** |  **2,478.63 ns** |  **1,296.37 ns** |  **0.11** |    **0.00** |    **4** |         **-** |          **NA** |
| CartesianTreeSort      | 1024 | PipeOrgan          |     8,657.0 ns |    409.47 ns |    181.81 ns |  0.01 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | PipeOrgan          |   599,160.7 ns |    910.00 ns |    475.95 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | PipeOrgan          |    17,560.9 ns |    124.68 ns |     55.36 ns |  0.03 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | PipeOrgan          |    34,770.3 ns |    731.89 ns |    382.79 ns |  0.06 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **ManyDuplicates**     |    **75,564.5 ns** |  **6,499.19 ns** |  **3,399.20 ns** |  **2.16** |    **0.09** |    **2** |         **-** |          **NA** |
| CartesianTreeSort      | 1024 | ManyDuplicates     |    35,162.4 ns |    763.35 ns |    399.25 ns |  1.00 |    0.01 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | ManyDuplicates     |    35,058.7 ns |    301.80 ns |    107.63 ns |  1.00 |    0.00 |    1 |         - |          NA |
| SplaySort              | 1024 | ManyDuplicates     |   114,063.7 ns | 21,009.31 ns | 10,988.28 ns |  3.25 |    0.30 |    3 |         - |          NA |
| TreapSort              | 1024 | ManyDuplicates     |    39,213.1 ns |  1,913.61 ns |  1,000.85 ns |  1.12 |    0.03 |    1 |         - |          NA |

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
