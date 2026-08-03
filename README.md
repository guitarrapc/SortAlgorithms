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
<summary>Benchmark results (2026-08-03 16:20 UTC)</summary>

Workflow run: https://github.com/guitarrapc/SortAlgorithms/actions/runs/30830296658

### AdaptiveBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.87GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method        | Size | Pattern            | Mean         | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------- |----- |------------------- |-------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **DropMergeSort** | **256**  | **Random**             |   **3,146.2 ns** |    **390.09 ns** |    **204.02 ns** |  **1.00** |    **0.09** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | Random             |   8,283.9 ns |    412.86 ns |    215.93 ns |  2.64 |    0.17 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **256**  | **SingleElementMoved** |     **740.3 ns** |    **207.90 ns** |    **108.73 ns** |  **1.02** |    **0.19** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | SingleElementMoved |   8,234.6 ns |    223.30 ns |    116.79 ns | 11.32 |    1.47 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **256**  | **Sorted**             |     **777.3 ns** |     **51.50 ns** |     **22.87 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | Sorted             |  11,865.4 ns |  4,627.27 ns |  2,420.15 ns | 15.28 |    2.97 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **256**  | **Reversed**           |   **1,650.0 ns** |     **90.97 ns** |     **47.58 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | Reversed           |   1,492.0 ns |     91.90 ns |     40.80 ns |  0.90 |    0.03 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **256**  | **PipeOrgan**          |   **6,392.8 ns** |    **477.19 ns** |    **249.58 ns** |  **1.00** |    **0.05** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | PipeOrgan          |   5,617.1 ns |    438.53 ns |    229.36 ns |  0.88 |    0.05 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **256**  | **ManyDuplicates**     |   **2,847.5 ns** |     **87.76 ns** |     **45.90 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | ManyDuplicates     |   3,934.4 ns |    332.50 ns |    173.91 ns |  1.38 |    0.06 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **Random**             |  **14,240.3 ns** |    **517.22 ns** |    **229.65 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | Random             |  24,496.1 ns |  1,499.93 ns |    784.49 ns |  1.72 |    0.06 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **SingleElementMoved** |   **2,720.4 ns** |     **55.41 ns** |     **19.76 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | SingleElementMoved |  39,999.1 ns |  1,463.09 ns |    765.22 ns | 14.70 |    0.28 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **Sorted**             |   **2,210.0 ns** |      **3.71 ns** |      **1.65 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | Sorted             |  39,611.3 ns |  1,011.61 ns |    449.16 ns | 17.92 |    0.19 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **Reversed**           |   **7,091.3 ns** |    **311.71 ns** |    **163.03 ns** |  **1.00** |    **0.03** |    **2** |         **-** |          **NA** |
| PatienceSort  | 1024 | Reversed           |   5,103.2 ns |     31.75 ns |     11.32 ns |  0.72 |    0.02 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **PipeOrgan**          |  **26,855.9 ns** |    **306.50 ns** |    **160.31 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | PipeOrgan          |  27,558.0 ns |  1,072.25 ns |    560.81 ns |  1.03 |    0.02 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **1024** | **ManyDuplicates**     |  **12,533.3 ns** |    **339.10 ns** |    **150.56 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | ManyDuplicates     |  15,364.2 ns |    691.56 ns |    307.06 ns |  1.23 |    0.03 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **4096** | **Random**             |  **74,986.2 ns** |  **9,555.13 ns** |  **4,997.52 ns** |  **1.00** |    **0.09** |    **1** |         **-** |          **NA** |
| PatienceSort  | 4096 | Random             | 164,532.2 ns | 15,801.22 ns |  8,264.34 ns |  2.20 |    0.17 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **4096** | **SingleElementMoved** |   **9,981.6 ns** |    **383.30 ns** |    **136.69 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| PatienceSort  | 4096 | SingleElementMoved | 239,241.6 ns | 27,384.96 ns | 14,322.87 ns | 23.97 |    1.39 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **4096** | **Sorted**             |   **9,960.4 ns** |  **2,597.88 ns** |  **1,358.74 ns** |  **1.01** |    **0.18** |    **1** |         **-** |          **NA** |
| PatienceSort  | 4096 | Sorted             | 222,948.7 ns | 34,449.34 ns | 18,017.67 ns | 22.71 |    3.14 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **4096** | **Reversed**           |  **31,031.7 ns** |    **528.22 ns** |    **234.53 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| PatienceSort  | 4096 | Reversed           |  20,217.6 ns |    440.13 ns |    230.20 ns |  0.65 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **4096** | **PipeOrgan**          | **111,878.7 ns** |    **598.69 ns** |    **265.82 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 4096 | PipeOrgan          | 163,030.2 ns |  4,863.10 ns |  2,543.50 ns |  1.46 |    0.02 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **4096** | **ManyDuplicates**     |  **55,822.5 ns** |  **1,420.26 ns** |    **742.82 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| PatienceSort  | 4096 | ManyDuplicates     |  59,936.5 ns |  1,104.76 ns |    490.52 ns |  1.07 |    0.02 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **Random**             | **476,675.4 ns** |  **5,996.30 ns** |  **3,136.18 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | Random             | 819,672.0 ns |  3,292.39 ns |  1,721.98 ns |  1.72 |    0.01 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **SingleElementMoved** |  **21,786.0 ns** |  **3,895.62 ns** |  **2,037.49 ns** |  **1.01** |    **0.12** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | SingleElementMoved | 771,306.2 ns |  3,810.58 ns |  1,691.92 ns | 35.66 |    2.95 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **Sorted**             |  **18,319.2 ns** |  **2,445.93 ns** |  **1,279.27 ns** |  **1.00** |    **0.09** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | Sorted             | 782,640.0 ns | 11,109.23 ns |  5,810.34 ns | 42.90 |    2.71 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **Reversed**           |  **64,656.5 ns** |  **1,205.78 ns** |    **535.37 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| PatienceSort  | 8192 | Reversed           |  40,833.4 ns |  1,503.31 ns |    786.26 ns |  0.63 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **PipeOrgan**          | **226,807.9 ns** |  **2,131.90 ns** |  **1,115.02 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | PipeOrgan          | 665,138.3 ns |  5,039.35 ns |  2,635.68 ns |  2.93 |    0.02 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **DropMergeSort** | **8192** | **ManyDuplicates**     | **121,391.5 ns** |    **985.15 ns** |    **351.32 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | ManyDuplicates     | 153,572.7 ns |  4,606.56 ns |  2,045.34 ns |  1.27 |    0.02 |    2 |         - |          NA |

### AdaptiveSlowBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.87GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method     | Size | Pattern            | Mean         | Error        | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------- |----- |------------------- |-------------:|-------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **StrandSort** | **256**  | **Random**             |   **6,507.9 ns** |    **274.09 ns** |   **121.70 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **256**  | **SingleElementMoved** |     **823.9 ns** |     **87.49 ns** |    **38.85 ns** |  **1.00** |    **0.06** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **256**  | **Sorted**             |     **570.5 ns** |     **10.79 ns** |     **4.79 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **256**  | **Reversed**           |  **47,833.3 ns** |    **389.51 ns** |   **172.95 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **256**  | **PipeOrgan**          |  **28,397.6 ns** |  **2,209.47 ns** | **1,155.60 ns** |  **1.00** |    **0.05** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **256**  | **ManyDuplicates**     |   **4,968.1 ns** |    **255.51 ns** |   **133.64 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **1024** | **Random**             |  **56,051.4 ns** |  **4,535.60 ns** | **2,013.83 ns** |  **1.00** |    **0.05** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **1024** | **SingleElementMoved** |   **2,783.8 ns** |    **200.00 ns** |    **88.80 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **1024** | **Sorted**             |   **1,756.8 ns** |     **43.51 ns** |    **15.52 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **1024** | **Reversed**           | **768,379.3 ns** |  **5,365.90 ns** | **2,806.47 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **1024** | **PipeOrgan**          | **446,944.1 ns** | **19,290.16 ns** | **8,564.95 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |         |      |           |             |
| **StrandSort** | **1024** | **ManyDuplicates**     |  **31,963.9 ns** |    **226.71 ns** |   **118.58 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |

### AmericanFlagRadixWidthBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.87GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                 | Size    | WideKeyRange | Mean            | Error        | StdDev       | Ratio | RatioSD | Allocated | Alloc Ratio |
| ----------------------- |-------- |------------- |----------------:|-------------:|-------------:|------:|--------:|----------:|------------:|
| **Radix16_C16**            | **4096**    | **False**        |     **71,570.6 ns** |   **1,072.6 ns** |     **561.0 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| Radix256_Shipped       | 4096    | False        |     73,171.5 ns |   1,397.8 ns |     731.1 ns |  1.02 |    0.01 |         - |          NA |
| Radix256_Cycle         | 4096    | False        |     70,415.4 ns |     903.0 ns |     400.9 ns |  0.98 |    0.01 |         - |          NA |
| Radix256_BinaryLeaf    | 4096    | False        |     69,855.9 ns |     654.3 ns |     342.2 ns |  0.98 |    0.01 |         - |          NA |
| Radix256_PerNodeRescan | 4096    | False        |     79,014.9 ns |     819.1 ns |     428.4 ns |  1.10 |    0.01 |         - |          NA |
|         |              |                 |              |              |       |         |           |             |
| **Radix16_C16**            | **4096**    | **True**         |     **94,852.4 ns** |   **1,092.0 ns** |     **571.1 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| Radix256_Shipped       | 4096    | True         |     49,288.4 ns |     919.0 ns |     408.1 ns |  0.52 |    0.00 |         - |          NA |
| Radix256_Cycle         | 4096    | True         |     47,301.0 ns |     985.8 ns |     437.7 ns |  0.50 |    0.01 |         - |          NA |
| Radix256_BinaryLeaf    | 4096    | True         |     60,357.3 ns |   1,992.9 ns |   1,042.3 ns |  0.64 |    0.01 |         - |          NA |
| Radix256_PerNodeRescan | 4096    | True         |     48,747.8 ns |   2,015.8 ns |   1,054.3 ns |  0.51 |    0.01 |         - |          NA |
|         |              |                 |              |              |       |         |           |             |
| **Radix16_C16**            | **8192**    | **False**        |    **176,045.8 ns** |   **1,880.8 ns** |     **983.7 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| Radix256_Shipped       | 8192    | False        |    144,095.3 ns |     680.4 ns |     355.9 ns |  0.82 |    0.00 |         - |          NA |
| Radix256_Cycle         | 8192    | False        |    140,899.7 ns |     972.1 ns |     508.4 ns |  0.80 |    0.01 |         - |          NA |
| Radix256_BinaryLeaf    | 8192    | False        |    139,718.2 ns |   1,448.3 ns |     643.1 ns |  0.79 |    0.01 |         - |          NA |
| Radix256_PerNodeRescan | 8192    | False        |    157,978.7 ns |   1,244.9 ns |     651.1 ns |  0.90 |    0.01 |         - |          NA |
|         |              |                 |              |              |       |         |           |             |
| **Radix16_C16**            | **8192**    | **True**         |    **212,501.4 ns** |   **4,433.4 ns** |   **2,318.7 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| Radix256_Shipped       | 8192    | True         |    114,951.0 ns |   1,872.1 ns |     667.6 ns |  0.54 |    0.01 |         - |          NA |
| Radix256_Cycle         | 8192    | True         |    111,938.6 ns |   3,353.5 ns |   1,754.0 ns |  0.53 |    0.01 |         - |          NA |
| Radix256_BinaryLeaf    | 8192    | True         |    155,938.9 ns |  12,534.8 ns |   6,555.9 ns |  0.73 |    0.03 |         - |          NA |
| Radix256_PerNodeRescan | 8192    | True         |    111,741.8 ns |   2,245.0 ns |     996.8 ns |  0.53 |    0.01 |         - |          NA |
|         |              |                 |              |              |       |         |           |             |
| **Radix16_C16**            | **65536**   | **False**        |  **2,327,230.8 ns** |   **3,171.2 ns** |   **1,658.6 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Radix256_Shipped       | 65536   | False        |  1,336,824.2 ns |   1,948.5 ns |     865.1 ns |  0.57 |    0.00 |         - |          NA |
| Radix256_Cycle         | 65536   | False        |  1,315,843.3 ns |   1,302.7 ns |     578.4 ns |  0.57 |    0.00 |         - |          NA |
| Radix256_BinaryLeaf    | 65536   | False        |  1,299,609.3 ns |   6,602.1 ns |   3,453.0 ns |  0.56 |    0.00 |         - |          NA |
| Radix256_PerNodeRescan | 65536   | False        |  1,462,899.5 ns |   2,759.7 ns |   1,443.4 ns |  0.63 |    0.00 |         - |          NA |
|         |              |                 |              |              |       |         |           |             |
| **Radix16_C16**            | **65536**   | **True**         |  **2,696,254.5 ns** |   **6,225.4 ns** |   **3,256.0 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Radix256_Shipped       | 65536   | True         |  1,688,698.2 ns |   2,524.4 ns |   1,320.3 ns |  0.63 |    0.00 |         - |          NA |
| Radix256_Cycle         | 65536   | True         |  1,723,962.0 ns |   2,489.3 ns |   1,301.9 ns |  0.64 |    0.00 |         - |          NA |
| Radix256_BinaryLeaf    | 65536   | True         |  1,872,281.5 ns |   1,676.7 ns |     877.0 ns |  0.69 |    0.00 |         - |          NA |
| Radix256_PerNodeRescan | 65536   | True         |  1,824,920.3 ns |   1,750.2 ns |     915.4 ns |  0.68 |    0.00 |         - |          NA |
|         |              |                 |              |              |       |         |           |             |
| **Radix16_C16**            | **1048576** | **False**        | **44,649,229.9 ns** |  **29,462.8 ns** |  **13,081.7 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Radix256_Shipped       | 1048576 | False        | 29,167,198.6 ns |  10,118.4 ns |   3,608.3 ns |  0.65 |    0.00 |         - |          NA |
| Radix256_Cycle         | 1048576 | False        | 29,218,982.5 ns |  34,108.0 ns |  15,144.2 ns |  0.65 |    0.00 |         - |          NA |
| Radix256_BinaryLeaf    | 1048576 | False        | 28,660,559.7 ns |  15,112.8 ns |   6,710.2 ns |  0.64 |    0.00 |         - |          NA |
| Radix256_PerNodeRescan | 1048576 | False        | 32,722,984.7 ns |  11,578.3 ns |   5,140.8 ns |  0.73 |    0.00 |         - |          NA |
|         |              |                 |              |              |       |         |           |             |
| **Radix16_C16**            | **1048576** | **True**         | **50,495,871.9 ns** | **172,780.1 ns** |  **76,715.5 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Radix256_Shipped       | 1048576 | True         | 28,214,142.7 ns |  76,536.0 ns |  33,982.4 ns |  0.56 |    0.00 |         - |          NA |
| Radix256_Cycle         | 1048576 | True         | 28,263,476.7 ns |  43,138.7 ns |  19,153.9 ns |  0.56 |    0.00 |         - |          NA |
| Radix256_BinaryLeaf    | 1048576 | True         | 37,968,902.3 ns | 212,541.7 ns | 111,163.4 ns |  0.75 |    0.00 |         - |          NA |
| Radix256_PerNodeRescan | 1048576 | True         | 29,098,486.5 ns |  77,710.5 ns |  34,503.9 ns |  0.58 |    0.00 |         - |          NA |

### DistributionBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.87GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method              | Size | Pattern            | Mean         | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------------- |----- |------------------- |-------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **CountingSort**        | **256**  | **Random**             |   **1,706.9 ns** |      **9.84 ns** |      **4.37 ns** |  **1.71** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | Random             |     995.5 ns |      7.84 ns |      3.48 ns |  1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 256  | Random             |   1,551.4 ns |      8.93 ns |      4.67 ns |  1.56 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | Random             |     694.4 ns |      4.62 ns |      1.65 ns |  0.70 |    0.00 |    1 |         - |          NA |
| BucketSort          | 256  | Random             |   2,103.0 ns |     17.74 ns |      6.33 ns |  2.11 |    0.01 |    3 |         - |          NA |
| BucketSortInteger   | 256  | Random             |   1,533.7 ns |      5.03 ns |      1.79 ns |  1.54 |    0.01 |    3 |         - |          NA |
| FlashSort           | 256  | Random             |   4,487.2 ns |     88.59 ns |     31.59 ns |  4.51 |    0.03 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | Random             |   2,956.3 ns |    103.94 ns |     37.06 ns |  2.97 |    0.04 |    4 |         - |          NA |
| RadixLSD256Sort     | 256  | Random             |   1,460.2 ns |    153.41 ns |     68.11 ns |  1.47 |    0.06 |    3 |         - |          NA |
| RadixLSD10Sort      | 256  | Random             |   4,239.9 ns |    295.15 ns |    131.05 ns |  4.26 |    0.12 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | Random             |   2,918.1 ns |     44.23 ns |     19.64 ns |  2.93 |    0.02 |    4 |         - |          NA |
| RadixMSD10Sort      | 256  | Random             |   4,114.6 ns |    172.96 ns |     76.80 ns |  4.13 |    0.07 |    5 |         - |          NA |
| AmericanFlagSort    | 256  | Random             |   2,831.6 ns |     75.15 ns |     26.80 ns |  2.84 |    0.03 |    4 |         - |          NA |
| SpreadSort          | 256  | Random             |   1,925.8 ns |    235.57 ns |    104.59 ns |  1.93 |    0.10 |    3 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **CountingSort**        | **256**  | **SingleElementMoved** |   **1,724.9 ns** |    **379.87 ns** |    **198.68 ns** |  **1.61** |    **0.17** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | SingleElementMoved |   1,074.4 ns |     14.04 ns |      6.23 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PigeonSort          | 256  | SingleElementMoved |   1,407.9 ns |      9.56 ns |      3.41 ns |  1.31 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | SingleElementMoved |     697.0 ns |      6.57 ns |      2.92 ns |  0.65 |    0.00 |    1 |         - |          NA |
| BucketSort          | 256  | SingleElementMoved |   1,977.7 ns |     41.63 ns |     18.49 ns |  1.84 |    0.02 |    4 |         - |          NA |
| BucketSortInteger   | 256  | SingleElementMoved |   1,620.6 ns |     17.56 ns |      9.19 ns |  1.51 |    0.01 |    3 |         - |          NA |
| FlashSort           | 256  | SingleElementMoved |   5,346.9 ns |     16.15 ns |      5.76 ns |  4.98 |    0.03 |    6 |         - |          NA |
| RadixLSD4Sort       | 256  | SingleElementMoved |   3,084.1 ns |     95.85 ns |     42.56 ns |  2.87 |    0.04 |    4 |         - |          NA |
| RadixLSD256Sort     | 256  | SingleElementMoved |   1,144.6 ns |      7.98 ns |      3.55 ns |  1.07 |    0.01 |    2 |         - |          NA |
| RadixLSD10Sort      | 256  | SingleElementMoved |   4,115.4 ns |    395.58 ns |    206.90 ns |  3.83 |    0.18 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | SingleElementMoved |   2,724.7 ns |     78.71 ns |     34.95 ns |  2.54 |    0.03 |    4 |         - |          NA |
| RadixMSD10Sort      | 256  | SingleElementMoved |   3,932.5 ns |    236.78 ns |    123.84 ns |  3.66 |    0.11 |    5 |         - |          NA |
| AmericanFlagSort    | 256  | SingleElementMoved |   2,351.8 ns |     99.06 ns |     43.98 ns |  2.19 |    0.04 |    4 |         - |          NA |
| SpreadSort          | 256  | SingleElementMoved |   1,154.9 ns |     45.60 ns |     20.25 ns |  1.07 |    0.02 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **CountingSort**        | **256**  | **Sorted**             |   **1,495.8 ns** |      **5.46 ns** |      **2.43 ns** |  **1.66** |    **0.01** |    **5** |         **-** |          **NA** |
| CountingSortInteger | 256  | Sorted             |     900.8 ns |     14.02 ns |      5.00 ns |  1.00 |    0.01 |    3 |         - |          NA |
| PigeonSort          | 256  | Sorted             |   1,458.7 ns |      5.78 ns |      2.57 ns |  1.62 |    0.01 |    5 |         - |          NA |
| PigeonSortInteger   | 256  | Sorted             |     666.4 ns |     95.92 ns |     50.17 ns |  0.74 |    0.05 |    2 |         - |          NA |
| BucketSort          | 256  | Sorted             |   2,090.3 ns |     48.50 ns |     21.53 ns |  2.32 |    0.03 |    6 |         - |          NA |
| BucketSortInteger   | 256  | Sorted             |   1,465.8 ns |     11.47 ns |      5.09 ns |  1.63 |    0.01 |    5 |         - |          NA |
| FlashSort           | 256  | Sorted             |   5,343.7 ns |     29.18 ns |     10.41 ns |  5.93 |    0.03 |    9 |         - |          NA |
| RadixLSD4Sort       | 256  | Sorted             |   3,109.4 ns |    422.18 ns |    220.81 ns |  3.45 |    0.23 |    7 |         - |          NA |
| RadixLSD256Sort     | 256  | Sorted             |   1,177.3 ns |     10.57 ns |      5.53 ns |  1.31 |    0.01 |    4 |         - |          NA |
| RadixLSD10Sort      | 256  | Sorted             |   4,134.5 ns |    298.45 ns |    156.10 ns |  4.59 |    0.17 |    8 |         - |          NA |
| RadixMSD4Sort       | 256  | Sorted             |   2,620.3 ns |     15.66 ns |      6.95 ns |  2.91 |    0.02 |    7 |         - |          NA |
| RadixMSD10Sort      | 256  | Sorted             |   3,834.9 ns |     76.98 ns |     34.18 ns |  4.26 |    0.04 |    8 |         - |          NA |
| AmericanFlagSort    | 256  | Sorted             |   1,518.4 ns |      9.12 ns |      4.05 ns |  1.69 |    0.01 |    5 |         - |          NA |
| SpreadSort          | 256  | Sorted             |     411.4 ns |      4.10 ns |      2.15 ns |  0.46 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **CountingSort**        | **256**  | **Reversed**           |   **1,502.3 ns** |     **12.48 ns** |      **4.45 ns** |  **1.50** |    **0.00** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 256  | Reversed           |   1,002.3 ns |      3.89 ns |      1.73 ns |  1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 256  | Reversed           |   1,346.3 ns |     17.02 ns |      7.56 ns |  1.34 |    0.01 |    4 |         - |          NA |
| PigeonSortInteger   | 256  | Reversed           |     667.5 ns |    110.94 ns |     58.02 ns |  0.67 |    0.05 |    2 |         - |          NA |
| BucketSort          | 256  | Reversed           |   1,997.1 ns |    115.70 ns |     51.37 ns |  1.99 |    0.05 |    4 |         - |          NA |
| BucketSortInteger   | 256  | Reversed           |   1,675.1 ns |    155.48 ns |     81.32 ns |  1.67 |    0.08 |    4 |         - |          NA |
| FlashSort           | 256  | Reversed           |   4,977.3 ns |    283.68 ns |    148.37 ns |  4.97 |    0.14 |    6 |         - |          NA |
| RadixLSD4Sort       | 256  | Reversed           |   2,851.6 ns |    194.08 ns |    101.51 ns |  2.85 |    0.10 |    5 |         - |          NA |
| RadixLSD256Sort     | 256  | Reversed           |   1,095.5 ns |     18.07 ns |      6.45 ns |  1.09 |    0.01 |    3 |         - |          NA |
| RadixLSD10Sort      | 256  | Reversed           |   3,954.3 ns |     73.77 ns |     26.31 ns |  3.95 |    0.03 |    6 |         - |          NA |
| RadixMSD4Sort       | 256  | Reversed           |   3,678.7 ns |     48.79 ns |     17.40 ns |  3.67 |    0.02 |    6 |         - |          NA |
| RadixMSD10Sort      | 256  | Reversed           |   4,473.2 ns |    373.41 ns |    195.30 ns |  4.46 |    0.18 |    6 |         - |          NA |
| AmericanFlagSort    | 256  | Reversed           |   1,782.0 ns |     10.03 ns |      4.45 ns |  1.78 |    0.01 |    4 |         - |          NA |
| SpreadSort          | 256  | Reversed           |     529.7 ns |      4.74 ns |      2.48 ns |  0.53 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **CountingSort**        | **256**  | **PipeOrgan**          |   **1,533.0 ns** |     **11.04 ns** |      **5.77 ns** |  **1.41** |    **0.01** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | PipeOrgan          |   1,089.6 ns |      6.78 ns |      3.54 ns |  1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 256  | PipeOrgan          |   1,390.6 ns |     78.56 ns |     34.88 ns |  1.28 |    0.03 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | PipeOrgan          |     700.8 ns |     17.82 ns |      9.32 ns |  0.64 |    0.01 |    1 |         - |          NA |
| BucketSort          | 256  | PipeOrgan          |   2,024.7 ns |     11.12 ns |      3.97 ns |  1.86 |    0.01 |    2 |         - |          NA |
| BucketSortInteger   | 256  | PipeOrgan          |   1,658.3 ns |     11.98 ns |      4.27 ns |  1.52 |    0.01 |    2 |         - |          NA |
| FlashSort           | 256  | PipeOrgan          |   5,183.0 ns |    286.57 ns |    149.88 ns |  4.76 |    0.13 |    4 |         - |          NA |
| RadixLSD4Sort       | 256  | PipeOrgan          |   2,811.7 ns |     14.30 ns |      5.10 ns |  2.58 |    0.01 |    3 |         - |          NA |
| RadixLSD256Sort     | 256  | PipeOrgan          |   1,292.7 ns |    330.86 ns |    173.05 ns |  1.19 |    0.15 |    2 |         - |          NA |
| RadixLSD10Sort      | 256  | PipeOrgan          |   3,904.6 ns |     23.26 ns |     10.33 ns |  3.58 |    0.01 |    3 |         - |          NA |
| RadixMSD4Sort       | 256  | PipeOrgan          |   3,294.1 ns |     76.06 ns |     27.12 ns |  3.02 |    0.02 |    3 |         - |          NA |
| RadixMSD10Sort      | 256  | PipeOrgan          |   4,101.6 ns |     39.17 ns |     13.97 ns |  3.76 |    0.02 |    3 |         - |          NA |
| AmericanFlagSort    | 256  | PipeOrgan          |   2,302.4 ns |    185.08 ns |     82.18 ns |  2.11 |    0.07 |    2 |         - |          NA |
| SpreadSort          | 256  | PipeOrgan          |   1,722.4 ns |     33.88 ns |     15.04 ns |  1.58 |    0.01 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **CountingSort**        | **256**  | **ManyDuplicates**     |   **1,514.7 ns** |     **18.97 ns** |      **8.42 ns** |  **1.61** |    **0.07** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | ManyDuplicates     |     945.4 ns |     98.08 ns |     43.55 ns |  1.00 |    0.06 |    2 |         - |          NA |
| PigeonSort          | 256  | ManyDuplicates     |   1,453.8 ns |     14.57 ns |      6.47 ns |  1.54 |    0.07 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | ManyDuplicates     |     633.2 ns |      4.38 ns |      1.94 ns |  0.67 |    0.03 |    1 |         - |          NA |
| BucketSort          | 256  | ManyDuplicates     |   3,113.0 ns |    298.93 ns |    156.34 ns |  3.30 |    0.21 |    5 |         - |          NA |
| BucketSortInteger   | 256  | ManyDuplicates     |   1,844.5 ns |    254.05 ns |    132.87 ns |  1.95 |    0.16 |    3 |         - |          NA |
| FlashSort           | 256  | ManyDuplicates     |   4,537.5 ns |     23.07 ns |      8.23 ns |  4.81 |    0.20 |    6 |         - |          NA |
| RadixLSD4Sort       | 256  | ManyDuplicates     |   2,318.7 ns |     13.50 ns |      5.99 ns |  2.46 |    0.10 |    4 |         - |          NA |
| RadixLSD256Sort     | 256  | ManyDuplicates     |   1,307.3 ns |     48.40 ns |     21.49 ns |  1.39 |    0.06 |    3 |         - |          NA |
| RadixLSD10Sort      | 256  | ManyDuplicates     |   2,847.8 ns |      4.10 ns |      1.46 ns |  3.02 |    0.13 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | ManyDuplicates     |   2,919.0 ns |    118.00 ns |     52.39 ns |  3.09 |    0.14 |    5 |         - |          NA |
| RadixMSD10Sort      | 256  | ManyDuplicates     |   3,734.5 ns |    302.80 ns |    158.37 ns |  3.96 |    0.23 |    5 |         - |          NA |
| AmericanFlagSort    | 256  | ManyDuplicates     |   3,238.8 ns |     10.63 ns |      4.72 ns |  3.43 |    0.15 |    5 |         - |          NA |
| SpreadSort          | 256  | ManyDuplicates     |   1,741.8 ns |    226.69 ns |    118.56 ns |  1.85 |    0.14 |    3 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **Random**             |   **6,033.9 ns** |    **336.20 ns** |    **175.84 ns** |  **1.59** |    **0.04** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Random             |   3,789.2 ns |     46.57 ns |     16.61 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PigeonSort          | 1024 | Random             |   5,612.5 ns |      9.85 ns |      3.51 ns |  1.48 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | Random             |   2,830.7 ns |     15.69 ns |      5.59 ns |  0.75 |    0.00 |    1 |         - |          NA |
| BucketSort          | 1024 | Random             |   8,105.8 ns |     20.61 ns |      9.15 ns |  2.14 |    0.01 |    3 |         - |          NA |
| BucketSortInteger   | 1024 | Random             |   5,997.0 ns |    340.89 ns |    178.29 ns |  1.58 |    0.04 |    3 |         - |          NA |
| FlashSort           | 1024 | Random             |  18,496.8 ns |    164.88 ns |     86.23 ns |  4.88 |    0.03 |    6 |         - |          NA |
| RadixLSD4Sort       | 1024 | Random             |  14,021.4 ns |    138.13 ns |     72.25 ns |  3.70 |    0.02 |    5 |         - |          NA |
| RadixLSD256Sort     | 1024 | Random             |   6,854.8 ns |    116.66 ns |     61.02 ns |  1.81 |    0.02 |    3 |         - |          NA |
| RadixLSD10Sort      | 1024 | Random             |  21,182.7 ns |    241.59 ns |    107.27 ns |  5.59 |    0.03 |    6 |         - |          NA |
| RadixMSD4Sort       | 1024 | Random             |  14,900.3 ns |    262.83 ns |    137.47 ns |  3.93 |    0.04 |    5 |         - |          NA |
| RadixMSD10Sort      | 1024 | Random             |  21,831.0 ns |    291.82 ns |    152.63 ns |  5.76 |    0.04 |    6 |         - |          NA |
| AmericanFlagSort    | 1024 | Random             |  18,295.8 ns |    168.44 ns |     74.79 ns |  4.83 |    0.03 |    6 |         - |          NA |
| SpreadSort          | 1024 | Random             |   9,866.1 ns |    455.50 ns |    202.24 ns |  2.60 |    0.05 |    4 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **SingleElementMoved** |   **5,919.2 ns** |     **10.30 ns** |      **3.67 ns** |  **1.41** |    **0.00** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | SingleElementMoved |   4,212.7 ns |     11.66 ns |      4.16 ns |  1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | SingleElementMoved |   5,076.5 ns |    401.84 ns |    210.17 ns |  1.21 |    0.05 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | SingleElementMoved |   2,811.2 ns |     74.56 ns |     26.59 ns |  0.67 |    0.01 |    1 |         - |          NA |
| BucketSort          | 1024 | SingleElementMoved |   7,662.5 ns |    312.22 ns |    163.30 ns |  1.82 |    0.04 |    2 |         - |          NA |
| BucketSortInteger   | 1024 | SingleElementMoved |   6,328.5 ns |    460.68 ns |    240.94 ns |  1.50 |    0.05 |    2 |         - |          NA |
| FlashSort           | 1024 | SingleElementMoved |  21,518.4 ns |    480.70 ns |    213.43 ns |  5.11 |    0.05 |    4 |         - |          NA |
| RadixLSD4Sort       | 1024 | SingleElementMoved |  16,534.4 ns |    465.61 ns |    243.52 ns |  3.92 |    0.05 |    4 |         - |          NA |
| RadixLSD256Sort     | 1024 | SingleElementMoved |   6,410.8 ns |    262.53 ns |    137.31 ns |  1.52 |    0.03 |    2 |         - |          NA |
| RadixLSD10Sort      | 1024 | SingleElementMoved |  21,297.3 ns |    246.14 ns |    128.74 ns |  5.06 |    0.03 |    4 |         - |          NA |
| RadixMSD4Sort       | 1024 | SingleElementMoved |  12,835.1 ns |    175.76 ns |     91.92 ns |  3.05 |    0.02 |    3 |         - |          NA |
| RadixMSD10Sort      | 1024 | SingleElementMoved |  19,555.5 ns |    192.89 ns |    100.89 ns |  4.64 |    0.02 |    4 |         - |          NA |
| AmericanFlagSort    | 1024 | SingleElementMoved |  12,860.1 ns |    309.13 ns |    137.25 ns |  3.05 |    0.03 |    3 |         - |          NA |
| SpreadSort          | 1024 | SingleElementMoved |   6,766.7 ns |    204.57 ns |     90.83 ns |  1.61 |    0.02 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **Sorted**             |   **5,549.4 ns** |     **14.01 ns** |      **5.00 ns** |  **1.65** |    **0.00** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Sorted             |   3,354.5 ns |     12.56 ns |      5.57 ns |  1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 1024 | Sorted             |   5,396.8 ns |    328.22 ns |    171.66 ns |  1.61 |    0.05 |    4 |         - |          NA |
| PigeonSortInteger   | 1024 | Sorted             |   2,435.7 ns |      7.37 ns |      3.27 ns |  0.73 |    0.00 |    2 |         - |          NA |
| BucketSort          | 1024 | Sorted             |   7,964.8 ns |     36.60 ns |     19.14 ns |  2.37 |    0.01 |    4 |         - |          NA |
| BucketSortInteger   | 1024 | Sorted             |   5,735.1 ns |    354.00 ns |    185.15 ns |  1.71 |    0.05 |    4 |         - |          NA |
| FlashSort           | 1024 | Sorted             |  29,487.3 ns | 12,828.58 ns |  6,709.59 ns |  8.79 |    1.89 |    8 |         - |          NA |
| RadixLSD4Sort       | 1024 | Sorted             |  15,724.7 ns |    537.12 ns |    191.54 ns |  4.69 |    0.05 |    6 |         - |          NA |
| RadixLSD256Sort     | 1024 | Sorted             |   6,644.8 ns |    427.74 ns |    223.72 ns |  1.98 |    0.06 |    4 |         - |          NA |
| RadixLSD10Sort      | 1024 | Sorted             |  20,902.4 ns |    236.31 ns |    123.59 ns |  6.23 |    0.04 |    7 |         - |          NA |
| RadixMSD4Sort       | 1024 | Sorted             |  12,778.9 ns |    279.73 ns |    146.30 ns |  3.81 |    0.04 |    5 |         - |          NA |
| RadixMSD10Sort      | 1024 | Sorted             |  19,552.2 ns |    210.70 ns |     93.55 ns |  5.83 |    0.03 |    7 |         - |          NA |
| AmericanFlagSort    | 1024 | Sorted             |   9,548.2 ns |    461.38 ns |    204.85 ns |  2.85 |    0.06 |    4 |         - |          NA |
| SpreadSort          | 1024 | Sorted             |     727.9 ns |     17.86 ns |      7.93 ns |  0.22 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **Reversed**           |   **5,644.5 ns** |    **341.24 ns** |    **178.47 ns** |  **1.46** |    **0.07** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Reversed           |   3,863.1 ns |    314.68 ns |    164.58 ns |  1.00 |    0.06 |    2 |         - |          NA |
| PigeonSort          | 1024 | Reversed           |   5,014.0 ns |    317.14 ns |    165.87 ns |  1.30 |    0.06 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | Reversed           |   2,436.8 ns |      9.13 ns |      4.05 ns |  0.63 |    0.02 |    1 |         - |          NA |
| BucketSort          | 1024 | Reversed           |   7,436.5 ns |    289.71 ns |    151.52 ns |  1.93 |    0.08 |    3 |         - |          NA |
| BucketSortInteger   | 1024 | Reversed           |   5,802.4 ns |     50.98 ns |     18.18 ns |  1.50 |    0.06 |    3 |         - |          NA |
| FlashSort           | 1024 | Reversed           |  18,868.0 ns |    442.87 ns |    231.63 ns |  4.89 |    0.20 |    5 |         - |          NA |
| RadixLSD4Sort       | 1024 | Reversed           |  16,124.0 ns |    340.76 ns |    178.22 ns |  4.18 |    0.17 |    5 |         - |          NA |
| RadixLSD256Sort     | 1024 | Reversed           |   6,361.4 ns |    323.90 ns |    169.40 ns |  1.65 |    0.08 |    3 |         - |          NA |
| RadixLSD10Sort      | 1024 | Reversed           |  21,808.8 ns |    314.28 ns |    139.54 ns |  5.65 |    0.22 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | Reversed           |  16,616.4 ns |     91.46 ns |     40.61 ns |  4.31 |    0.17 |    5 |         - |          NA |
| RadixMSD10Sort      | 1024 | Reversed           |  21,590.1 ns |    115.38 ns |     60.35 ns |  5.60 |    0.22 |    5 |         - |          NA |
| AmericanFlagSort    | 1024 | Reversed           |  12,166.4 ns |    304.55 ns |    135.22 ns |  3.15 |    0.13 |    4 |         - |          NA |
| SpreadSort          | 1024 | Reversed           |   5,068.3 ns |    349.58 ns |    182.83 ns |  1.31 |    0.07 |    3 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **PipeOrgan**          |   **5,779.1 ns** |    **221.19 ns** |    **115.69 ns** |  **1.40** |    **0.03** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | PipeOrgan          |   4,117.4 ns |     16.86 ns |      6.01 ns |  1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | PipeOrgan          |   5,097.0 ns |    374.00 ns |    166.06 ns |  1.24 |    0.04 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | PipeOrgan          |   2,940.4 ns |    383.91 ns |    200.79 ns |  0.71 |    0.05 |    1 |         - |          NA |
| BucketSort          | 1024 | PipeOrgan          |   8,080.3 ns |    159.90 ns |     71.00 ns |  1.96 |    0.02 |    3 |         - |          NA |
| BucketSortInteger   | 1024 | PipeOrgan          |   6,419.9 ns |    349.54 ns |    182.82 ns |  1.56 |    0.04 |    3 |         - |          NA |
| FlashSort           | 1024 | PipeOrgan          |  19,776.2 ns |    118.42 ns |     52.58 ns |  4.80 |    0.01 |    5 |         - |          NA |
| RadixLSD4Sort       | 1024 | PipeOrgan          |  13,942.1 ns |    328.68 ns |    171.91 ns |  3.39 |    0.04 |    4 |         - |          NA |
| RadixLSD256Sort     | 1024 | PipeOrgan          |   6,318.4 ns |    263.66 ns |    137.90 ns |  1.53 |    0.03 |    3 |         - |          NA |
| RadixLSD10Sort      | 1024 | PipeOrgan          |  21,369.1 ns |    333.31 ns |    174.33 ns |  5.19 |    0.04 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | PipeOrgan          |  16,351.5 ns |    800.02 ns |    418.43 ns |  3.97 |    0.10 |    4 |         - |          NA |
| RadixMSD10Sort      | 1024 | PipeOrgan          |  21,212.4 ns |    460.08 ns |    240.63 ns |  5.15 |    0.06 |    5 |         - |          NA |
| AmericanFlagSort    | 1024 | PipeOrgan          |  14,941.0 ns |     57.27 ns |     29.95 ns |  3.63 |    0.01 |    4 |         - |          NA |
| SpreadSort          | 1024 | PipeOrgan          |   7,129.2 ns |     10.55 ns |      3.76 ns |  1.73 |    0.00 |    3 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **CountingSort**        | **1024** | **ManyDuplicates**     |   **5,539.2 ns** |    **329.07 ns** |    **172.11 ns** |  **1.68** |    **0.07** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 1024 | ManyDuplicates     |   3,305.1 ns |    229.72 ns |    120.15 ns |  1.00 |    0.05 |    2 |         - |          NA |
| PigeonSort          | 1024 | ManyDuplicates     |   5,819.0 ns |    424.26 ns |    221.89 ns |  1.76 |    0.09 |    4 |         - |          NA |
| PigeonSortInteger   | 1024 | ManyDuplicates     |   2,419.7 ns |      7.86 ns |      2.80 ns |  0.73 |    0.02 |    1 |         - |          NA |
| BucketSort          | 1024 | ManyDuplicates     |  12,028.3 ns |    198.20 ns |    103.66 ns |  3.64 |    0.12 |    5 |         - |          NA |
| BucketSortInteger   | 1024 | ManyDuplicates     |   6,692.9 ns |    287.28 ns |    150.25 ns |  2.03 |    0.08 |    4 |         - |          NA |
| FlashSort           | 1024 | ManyDuplicates     |  19,727.5 ns |    162.29 ns |     72.06 ns |  5.98 |    0.20 |    6 |         - |          NA |
| RadixLSD4Sort       | 1024 | ManyDuplicates     |   9,046.0 ns |    329.62 ns |    172.40 ns |  2.74 |    0.10 |    5 |         - |          NA |
| RadixLSD256Sort     | 1024 | ManyDuplicates     |   4,569.5 ns |    590.67 ns |    308.93 ns |  1.38 |    0.10 |    3 |         - |          NA |
| RadixLSD10Sort      | 1024 | ManyDuplicates     |  11,426.6 ns |    309.66 ns |    161.96 ns |  3.46 |    0.12 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | ManyDuplicates     |  10,790.8 ns |    409.95 ns |    214.41 ns |  3.27 |    0.12 |    5 |         - |          NA |
| RadixMSD10Sort      | 1024 | ManyDuplicates     |  12,756.7 ns |    259.62 ns |    135.79 ns |  3.86 |    0.13 |    5 |         - |          NA |
| AmericanFlagSort    | 1024 | ManyDuplicates     |   9,841.4 ns |    356.67 ns |    186.54 ns |  2.98 |    0.11 |    5 |         - |          NA |
| SpreadSort          | 1024 | ManyDuplicates     |   6,665.9 ns |    306.08 ns |    160.08 ns |  2.02 |    0.08 |    4 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **CountingSort**        | **4096** | **Random**             |  **24,962.9 ns** |  **1,770.08 ns** |    **785.93 ns** |  **1.60** |    **0.05** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Random             |  15,611.4 ns |    357.88 ns |    127.62 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PigeonSort          | 4096 | Random             |  22,673.5 ns |    477.33 ns |    211.94 ns |  1.45 |    0.02 |    3 |         - |          NA |
| PigeonSortInteger   | 4096 | Random             |  11,429.1 ns |    391.86 ns |    173.99 ns |  0.73 |    0.01 |    1 |         - |          NA |
| BucketSort          | 4096 | Random             |  33,810.4 ns |  1,140.82 ns |    506.53 ns |  2.17 |    0.03 |    4 |         - |          NA |
| BucketSortInteger   | 4096 | Random             |  24,149.3 ns |    954.70 ns |    499.33 ns |  1.55 |    0.03 |    3 |         - |          NA |
| FlashSort           | 4096 | Random             |  77,444.3 ns |    713.68 ns |    316.88 ns |  4.96 |    0.04 |    5 |         - |          NA |
| RadixLSD4Sort       | 4096 | Random             |  65,419.4 ns |    506.45 ns |    224.87 ns |  4.19 |    0.03 |    5 |         - |          NA |
| RadixLSD256Sort     | 4096 | Random             |  25,978.9 ns |    310.50 ns |    137.86 ns |  1.66 |    0.02 |    3 |         - |          NA |
| RadixLSD10Sort      | 4096 | Random             |  84,995.3 ns |  1,095.65 ns |    573.04 ns |  5.44 |    0.05 |    5 |         - |          NA |
| RadixMSD4Sort       | 4096 | Random             |  73,104.9 ns |  1,839.53 ns |    962.11 ns |  4.68 |    0.07 |    5 |         - |          NA |
| RadixMSD10Sort      | 4096 | Random             |  86,420.2 ns |  1,115.70 ns |    583.53 ns |  5.54 |    0.05 |    5 |         - |          NA |
| AmericanFlagSort    | 4096 | Random             |  72,848.1 ns |  1,644.19 ns |    859.94 ns |  4.67 |    0.06 |    5 |         - |          NA |
| SpreadSort          | 4096 | Random             |  38,845.4 ns |    444.68 ns |    197.44 ns |  2.49 |    0.02 |    4 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **CountingSort**        | **4096** | **SingleElementMoved** |  **24,377.7 ns** |    **549.96 ns** |    **287.64 ns** |  **1.44** |    **0.04** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 4096 | SingleElementMoved |  16,933.9 ns |    764.72 ns |    399.96 ns |  1.00 |    0.03 |    2 |         - |          NA |
| PigeonSort          | 4096 | SingleElementMoved |  20,127.7 ns |  1,030.18 ns |    538.80 ns |  1.19 |    0.04 |    2 |         - |          NA |
| PigeonSortInteger   | 4096 | SingleElementMoved |  11,491.4 ns |    934.73 ns |    415.03 ns |  0.68 |    0.03 |    1 |         - |          NA |
| BucketSort          | 4096 | SingleElementMoved |  30,293.7 ns |    612.48 ns |    271.94 ns |  1.79 |    0.04 |    2 |         - |          NA |
| BucketSortInteger   | 4096 | SingleElementMoved |  24,990.0 ns |    301.19 ns |    157.53 ns |  1.48 |    0.03 |    2 |         - |          NA |
| FlashSort           | 4096 | SingleElementMoved |  86,520.9 ns |  2,082.54 ns |    924.66 ns |  5.11 |    0.12 |    5 |         - |          NA |
| RadixLSD4Sort       | 4096 | SingleElementMoved |  93,388.1 ns |    534.69 ns |    279.65 ns |  5.52 |    0.12 |    5 |         - |          NA |
| RadixLSD256Sort     | 4096 | SingleElementMoved |  23,100.4 ns |    967.32 ns |    429.50 ns |  1.36 |    0.04 |    2 |         - |          NA |
| RadixLSD10Sort      | 4096 | SingleElementMoved |  85,400.8 ns |  1,691.74 ns |    751.14 ns |  5.05 |    0.12 |    5 |         - |          NA |
| RadixMSD4Sort       | 4096 | SingleElementMoved |  59,525.1 ns |    601.67 ns |    214.56 ns |  3.52 |    0.08 |    4 |         - |          NA |
| RadixMSD10Sort      | 4096 | SingleElementMoved |  79,129.1 ns |    670.16 ns |    350.50 ns |  4.68 |    0.10 |    5 |         - |          NA |
| AmericanFlagSort    | 4096 | SingleElementMoved |  48,020.9 ns |    240.08 ns |    106.60 ns |  2.84 |    0.06 |    3 |         - |          NA |
| SpreadSort          | 4096 | SingleElementMoved |  27,454.4 ns |    873.60 ns |    387.88 ns |  1.62 |    0.04 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **CountingSort**        | **4096** | **Sorted**             |  **22,917.5 ns** |  **1,540.97 ns** |    **805.96 ns** |  **1.66** |    **0.06** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Sorted             |  13,827.6 ns |    176.43 ns |     62.92 ns |  1.00 |    0.01 |    3 |         - |          NA |
| PigeonSort          | 4096 | Sorted             |  21,402.6 ns |  1,171.24 ns |    612.58 ns |  1.55 |    0.04 |    4 |         - |          NA |
| PigeonSortInteger   | 4096 | Sorted             |   9,815.6 ns |    440.86 ns |    195.75 ns |  0.71 |    0.01 |    2 |         - |          NA |
| BucketSort          | 4096 | Sorted             |  32,173.0 ns |    814.79 ns |    361.77 ns |  2.33 |    0.03 |    5 |         - |          NA |
| BucketSortInteger   | 4096 | Sorted             |  22,028.8 ns |    404.14 ns |    179.44 ns |  1.59 |    0.01 |    4 |         - |          NA |
| FlashSort           | 4096 | Sorted             |  86,072.9 ns |    869.16 ns |    385.91 ns |  6.22 |    0.04 |    7 |         - |          NA |
| RadixLSD4Sort       | 4096 | Sorted             |  86,505.2 ns |    907.51 ns |    402.94 ns |  6.26 |    0.04 |    7 |         - |          NA |
| RadixLSD256Sort     | 4096 | Sorted             |  23,796.4 ns |    518.28 ns |    230.12 ns |  1.72 |    0.02 |    4 |         - |          NA |
| RadixLSD10Sort      | 4096 | Sorted             |  84,678.9 ns |  1,433.29 ns |    749.64 ns |  6.12 |    0.06 |    7 |         - |          NA |
| RadixMSD4Sort       | 4096 | Sorted             |  59,886.9 ns |  1,150.66 ns |    510.90 ns |  4.33 |    0.04 |    6 |         - |          NA |
| RadixMSD10Sort      | 4096 | Sorted             |  79,344.1 ns |  1,178.91 ns |    523.44 ns |  5.74 |    0.04 |    7 |         - |          NA |
| AmericanFlagSort    | 4096 | Sorted             |  34,972.1 ns |    228.53 ns |    119.53 ns |  2.53 |    0.01 |    5 |         - |          NA |
| SpreadSort          | 4096 | Sorted             |   2,321.4 ns |    206.71 ns |     91.78 ns |  0.17 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **CountingSort**        | **4096** | **Reversed**           |  **22,601.0 ns** |    **464.96 ns** |    **243.18 ns** |  **1.47** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Reversed           |  15,410.1 ns |    127.17 ns |     45.35 ns |  1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | Reversed           |  19,797.2 ns |    941.05 ns |    417.83 ns |  1.28 |    0.03 |    3 |         - |          NA |
| PigeonSortInteger   | 4096 | Reversed           |   9,987.7 ns |    656.28 ns |    343.25 ns |  0.65 |    0.02 |    1 |         - |          NA |
| BucketSort          | 4096 | Reversed           |  30,663.1 ns |  1,142.60 ns |    597.60 ns |  1.99 |    0.04 |    4 |         - |          NA |
| BucketSortInteger   | 4096 | Reversed           |  23,257.5 ns |    369.52 ns |    131.78 ns |  1.51 |    0.01 |    3 |         - |          NA |
| FlashSort           | 4096 | Reversed           |  75,909.1 ns |    553.74 ns |    245.87 ns |  4.93 |    0.02 |    6 |         - |          NA |
| RadixLSD4Sort       | 4096 | Reversed           |  95,825.7 ns |  1,407.86 ns |    736.34 ns |  6.22 |    0.05 |    6 |         - |          NA |
| RadixLSD256Sort     | 4096 | Reversed           |  22,536.1 ns |    477.30 ns |    211.92 ns |  1.46 |    0.01 |    3 |         - |          NA |
| RadixLSD10Sort      | 4096 | Reversed           |  84,746.9 ns |  1,661.72 ns |    869.11 ns |  5.50 |    0.06 |    6 |         - |          NA |
| RadixMSD4Sort       | 4096 | Reversed           |  75,265.0 ns |    666.29 ns |    295.84 ns |  4.88 |    0.02 |    6 |         - |          NA |
| RadixMSD10Sort      | 4096 | Reversed           |  87,691.3 ns |  1,076.93 ns |    563.25 ns |  5.69 |    0.04 |    6 |         - |          NA |
| AmericanFlagSort    | 4096 | Reversed           |  45,156.5 ns |    663.95 ns |    347.26 ns |  2.93 |    0.02 |    5 |         - |          NA |
| SpreadSort          | 4096 | Reversed           |  19,752.8 ns |    571.74 ns |    253.86 ns |  1.28 |    0.02 |    3 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **CountingSort**        | **4096** | **PipeOrgan**          |  **23,510.4 ns** |    **376.71 ns** |    **167.26 ns** |  **1.35** |    **0.05** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 4096 | PipeOrgan          |  17,501.7 ns |  1,409.91 ns |    737.41 ns |  1.00 |    0.06 |    2 |         - |          NA |
| PigeonSort          | 4096 | PipeOrgan          |  20,010.9 ns |    587.49 ns |    307.27 ns |  1.15 |    0.05 |    2 |         - |          NA |
| PigeonSortInteger   | 4096 | PipeOrgan          |  11,767.9 ns |    554.67 ns |    246.28 ns |  0.67 |    0.03 |    1 |         - |          NA |
| BucketSort          | 4096 | PipeOrgan          |  30,965.0 ns |  1,430.03 ns |    747.93 ns |  1.77 |    0.08 |    3 |         - |          NA |
| BucketSortInteger   | 4096 | PipeOrgan          |  25,326.9 ns |  1,032.17 ns |    539.84 ns |  1.45 |    0.06 |    2 |         - |          NA |
| FlashSort           | 4096 | PipeOrgan          |  75,527.9 ns |  1,037.29 ns |    542.52 ns |  4.32 |    0.17 |    5 |         - |          NA |
| RadixLSD4Sort       | 4096 | PipeOrgan          |  76,284.1 ns |    668.84 ns |    349.81 ns |  4.37 |    0.17 |    5 |         - |          NA |
| RadixLSD256Sort     | 4096 | PipeOrgan          |  24,067.5 ns |  1,915.78 ns |  1,001.99 ns |  1.38 |    0.08 |    2 |         - |          NA |
| RadixLSD10Sort      | 4096 | PipeOrgan          |  85,309.9 ns |  1,084.77 ns |    567.35 ns |  4.88 |    0.19 |    5 |         - |          NA |
| RadixMSD4Sort       | 4096 | PipeOrgan          |  77,232.5 ns |  1,034.00 ns |    540.80 ns |  4.42 |    0.17 |    5 |         - |          NA |
| RadixMSD10Sort      | 4096 | PipeOrgan          |  85,281.4 ns |  1,171.46 ns |    520.14 ns |  4.88 |    0.19 |    5 |         - |          NA |
| AmericanFlagSort    | 4096 | PipeOrgan          |  60,853.0 ns |  1,013.07 ns |    449.81 ns |  3.48 |    0.14 |    4 |         - |          NA |
| SpreadSort          | 4096 | PipeOrgan          |  30,831.9 ns |  1,602.20 ns |    837.98 ns |  1.76 |    0.08 |    3 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **CountingSort**        | **4096** | **ManyDuplicates**     |  **21,907.2 ns** |    **665.51 ns** |    **295.49 ns** |  **1.67** |    **0.03** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 4096 | ManyDuplicates     |  13,112.0 ns |    367.13 ns |    163.01 ns |  1.00 |    0.02 |    2 |         - |          NA |
| PigeonSort          | 4096 | ManyDuplicates     |  27,607.3 ns |  3,235.25 ns |  1,436.47 ns |  2.11 |    0.11 |    5 |         - |          NA |
| PigeonSortInteger   | 4096 | ManyDuplicates     |   9,934.5 ns |    383.31 ns |    170.19 ns |  0.76 |    0.02 |    1 |         - |          NA |
| BucketSort          | 4096 | ManyDuplicates     |  48,708.6 ns |    928.54 ns |    412.28 ns |  3.72 |    0.05 |    5 |         - |          NA |
| BucketSortInteger   | 4096 | ManyDuplicates     |  27,663.9 ns |  1,840.00 ns |    962.36 ns |  2.11 |    0.07 |    5 |         - |          NA |
| FlashSort           | 4096 | ManyDuplicates     |  72,617.5 ns |  1,409.61 ns |    625.88 ns |  5.54 |    0.08 |    6 |         - |          NA |
| RadixLSD4Sort       | 4096 | ManyDuplicates     |  35,996.8 ns |    484.33 ns |    253.32 ns |  2.75 |    0.04 |    5 |         - |          NA |
| RadixLSD256Sort     | 4096 | ManyDuplicates     |  16,255.7 ns |    166.73 ns |     59.46 ns |  1.24 |    0.02 |    3 |         - |          NA |
| RadixLSD10Sort      | 4096 | ManyDuplicates     |  46,477.9 ns |  1,055.86 ns |    552.23 ns |  3.55 |    0.06 |    5 |         - |          NA |
| RadixMSD4Sort       | 4096 | ManyDuplicates     |  40,151.5 ns |    944.23 ns |    419.25 ns |  3.06 |    0.05 |    5 |         - |          NA |
| RadixMSD10Sort      | 4096 | ManyDuplicates     |  49,293.6 ns |  1,105.15 ns |    490.69 ns |  3.76 |    0.06 |    5 |         - |          NA |
| AmericanFlagSort    | 4096 | ManyDuplicates     |  31,882.8 ns |  1,616.00 ns |    845.20 ns |  2.43 |    0.07 |    5 |         - |          NA |
| SpreadSort          | 4096 | ManyDuplicates     |  26,940.2 ns |  1,237.55 ns |    549.48 ns |  2.05 |    0.05 |    5 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **Random**             |  **51,720.0 ns** |  **1,049.24 ns** |    **465.87 ns** |  **1.53** |    **0.06** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Random             |  33,869.1 ns |  2,563.98 ns |  1,341.01 ns |  1.00 |    0.05 |    2 |         - |          NA |
| PigeonSort          | 8192 | Random             |  45,565.1 ns |  1,388.00 ns |    616.28 ns |  1.35 |    0.05 |    3 |         - |          NA |
| PigeonSortInteger   | 8192 | Random             |  22,963.3 ns |    454.03 ns |    237.47 ns |  0.68 |    0.03 |    1 |         - |          NA |
| BucketSort          | 8192 | Random             |  68,900.0 ns |    651.64 ns |    289.33 ns |  2.04 |    0.07 |    4 |         - |          NA |
| BucketSortInteger   | 8192 | Random             |  50,706.9 ns |  1,514.83 ns |    672.59 ns |  1.50 |    0.06 |    3 |         - |          NA |
| FlashSort           | 8192 | Random             | 211,052.6 ns | 19,247.84 ns | 10,066.99 ns |  6.24 |    0.36 |    7 |         - |          NA |
| RadixLSD4Sort       | 8192 | Random             | 152,385.2 ns |  1,779.58 ns |    634.62 ns |  4.51 |    0.16 |    6 |         - |          NA |
| RadixLSD256Sort     | 8192 | Random             |  51,651.1 ns |  1,724.45 ns |    901.92 ns |  1.53 |    0.06 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | Random             | 172,732.1 ns |  2,060.40 ns |  1,077.63 ns |  5.11 |    0.19 |    6 |         - |          NA |
| RadixMSD4Sort       | 8192 | Random             | 168,517.2 ns |  2,433.71 ns |  1,272.88 ns |  4.98 |    0.18 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | Random             | 174,384.3 ns |  1,013.17 ns |    449.85 ns |  5.16 |    0.19 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | Random             | 144,316.3 ns |  1,300.75 ns |    577.54 ns |  4.27 |    0.16 |    6 |         - |          NA |
| SpreadSort          | 8192 | Random             |  97,625.1 ns |  1,105.03 ns |    577.95 ns |  2.89 |    0.11 |    5 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **SingleElementMoved** |  **48,290.6 ns** |    **793.69 ns** |    **415.11 ns** |  **1.41** |    **0.03** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | SingleElementMoved |  34,225.6 ns |  1,168.42 ns |    611.11 ns |  1.00 |    0.02 |    2 |         - |          NA |
| PigeonSort          | 8192 | SingleElementMoved |  39,891.2 ns |    586.78 ns |    260.54 ns |  1.17 |    0.02 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | SingleElementMoved |  22,755.2 ns |    310.18 ns |    162.23 ns |  0.67 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | SingleElementMoved |  60,409.4 ns |    575.55 ns |    301.02 ns |  1.77 |    0.03 |    2 |         - |          NA |
| BucketSortInteger   | 8192 | SingleElementMoved |  49,771.2 ns |    725.24 ns |    322.01 ns |  1.45 |    0.03 |    2 |         - |          NA |
| FlashSort           | 8192 | SingleElementMoved | 216,933.7 ns |  6,498.43 ns |  3,398.80 ns |  6.34 |    0.14 |    5 |         - |          NA |
| RadixLSD4Sort       | 8192 | SingleElementMoved | 210,449.1 ns |  2,068.78 ns |  1,082.01 ns |  6.15 |    0.11 |    5 |         - |          NA |
| RadixLSD256Sort     | 8192 | SingleElementMoved |  47,676.0 ns |    407.79 ns |    213.28 ns |  1.39 |    0.02 |    2 |         - |          NA |
| RadixLSD10Sort      | 8192 | SingleElementMoved | 167,509.5 ns |  2,433.21 ns |  1,080.36 ns |  4.90 |    0.09 |    4 |         - |          NA |
| RadixMSD4Sort       | 8192 | SingleElementMoved | 137,558.3 ns |    728.21 ns |    380.87 ns |  4.02 |    0.07 |    4 |         - |          NA |
| RadixMSD10Sort      | 8192 | SingleElementMoved | 157,670.0 ns |  2,557.75 ns |  1,135.66 ns |  4.61 |    0.08 |    4 |         - |          NA |
| AmericanFlagSort    | 8192 | SingleElementMoved |  94,505.9 ns |  1,095.09 ns |    572.76 ns |  2.76 |    0.05 |    3 |         - |          NA |
| SpreadSort          | 8192 | SingleElementMoved |  57,162.2 ns |  1,077.56 ns |    563.58 ns |  1.67 |    0.03 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **Sorted**             |  **45,834.0 ns** |  **1,526.52 ns** |    **677.78 ns** |  **1.61** |    **0.04** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Sorted             |  28,394.5 ns |  1,182.98 ns |    618.72 ns |  1.00 |    0.03 |    3 |         - |          NA |
| PigeonSort          | 8192 | Sorted             |  41,732.4 ns |    360.70 ns |    160.15 ns |  1.47 |    0.03 |    4 |         - |          NA |
| PigeonSortInteger   | 8192 | Sorted             |  19,915.4 ns |    373.38 ns |    165.78 ns |  0.70 |    0.02 |    2 |         - |          NA |
| BucketSort          | 8192 | Sorted             |  67,455.2 ns |  2,407.91 ns |  1,259.38 ns |  2.38 |    0.06 |    5 |         - |          NA |
| BucketSortInteger   | 8192 | Sorted             |  43,894.8 ns |  1,509.88 ns |    670.40 ns |  1.55 |    0.04 |    4 |         - |          NA |
| FlashSort           | 8192 | Sorted             | 171,131.2 ns |  1,260.85 ns |    659.45 ns |  6.03 |    0.12 |    6 |         - |          NA |
| RadixLSD4Sort       | 8192 | Sorted             | 195,702.5 ns |  2,911.67 ns |  1,522.86 ns |  6.90 |    0.15 |    6 |         - |          NA |
| RadixLSD256Sort     | 8192 | Sorted             |  49,293.8 ns |    631.90 ns |    280.57 ns |  1.74 |    0.04 |    4 |         - |          NA |
| RadixLSD10Sort      | 8192 | Sorted             | 169,263.7 ns |  3,377.48 ns |  1,766.49 ns |  5.96 |    0.13 |    6 |         - |          NA |
| RadixMSD4Sort       | 8192 | Sorted             | 138,787.9 ns |  1,182.26 ns |    618.35 ns |  4.89 |    0.10 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | Sorted             | 158,516.1 ns |  1,547.11 ns |    809.17 ns |  5.58 |    0.12 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | Sorted             |  69,023.2 ns |  1,036.84 ns |    460.37 ns |  2.43 |    0.05 |    5 |         - |          NA |
| SpreadSort          | 8192 | Sorted             |   4,461.4 ns |     21.87 ns |     11.44 ns |  0.16 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **Reversed**           |  **45,297.4 ns** |  **1,643.50 ns** |    **859.58 ns** |  **1.47** |    **0.03** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Reversed           |  30,854.5 ns |    667.83 ns |    296.52 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PigeonSort          | 8192 | Reversed           |  39,119.5 ns |    906.61 ns |    402.54 ns |  1.27 |    0.02 |    3 |         - |          NA |
| PigeonSortInteger   | 8192 | Reversed           |  20,204.8 ns |  1,128.56 ns |    590.26 ns |  0.65 |    0.02 |    1 |         - |          NA |
| BucketSort          | 8192 | Reversed           |  60,524.3 ns |  2,910.07 ns |  1,522.02 ns |  1.96 |    0.05 |    4 |         - |          NA |
| BucketSortInteger   | 8192 | Reversed           |  47,442.1 ns |  1,324.67 ns |    692.83 ns |  1.54 |    0.03 |    3 |         - |          NA |
| FlashSort           | 8192 | Reversed           | 200,428.7 ns |  4,585.53 ns |  2,398.32 ns |  6.50 |    0.09 |    6 |         - |          NA |
| RadixLSD4Sort       | 8192 | Reversed           | 214,627.6 ns |  1,732.13 ns |    905.94 ns |  6.96 |    0.07 |    6 |         - |          NA |
| RadixLSD256Sort     | 8192 | Reversed           |  47,134.7 ns |    947.82 ns |    495.73 ns |  1.53 |    0.02 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | Reversed           | 169,041.3 ns |  2,271.51 ns |  1,188.05 ns |  5.48 |    0.06 |    6 |         - |          NA |
| RadixMSD4Sort       | 8192 | Reversed           | 168,050.1 ns |    533.18 ns |    236.74 ns |  5.45 |    0.05 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | Reversed           | 175,260.7 ns |    780.36 ns |    408.14 ns |  5.68 |    0.05 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | Reversed           |  89,883.7 ns |  1,030.76 ns |    539.11 ns |  2.91 |    0.03 |    5 |         - |          NA |
| SpreadSort          | 8192 | Reversed           |  76,256.7 ns |  2,693.68 ns |  1,196.01 ns |  2.47 |    0.04 |    5 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **PipeOrgan**          |  **46,781.0 ns** |  **1,047.33 ns** |    **465.02 ns** |  **1.30** |    **0.02** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | PipeOrgan          |  35,864.0 ns |    922.75 ns |    482.62 ns |  1.00 |    0.02 |    2 |         - |          NA |
| PigeonSort          | 8192 | PipeOrgan          |  39,823.5 ns |  1,059.00 ns |    470.20 ns |  1.11 |    0.02 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | PipeOrgan          |  23,372.7 ns |  1,653.61 ns |    864.87 ns |  0.65 |    0.02 |    1 |         - |          NA |
| BucketSort          | 8192 | PipeOrgan          |  61,572.4 ns |  1,068.48 ns |    474.41 ns |  1.72 |    0.03 |    3 |         - |          NA |
| BucketSortInteger   | 8192 | PipeOrgan          |  50,223.0 ns |  1,343.16 ns |    702.50 ns |  1.40 |    0.03 |    2 |         - |          NA |
| FlashSort           | 8192 | PipeOrgan          | 161,450.5 ns |  1,149.97 ns |    510.59 ns |  4.50 |    0.06 |    6 |         - |          NA |
| RadixLSD4Sort       | 8192 | PipeOrgan          | 181,768.2 ns |    917.43 ns |    479.83 ns |  5.07 |    0.07 |    6 |         - |          NA |
| RadixLSD256Sort     | 8192 | PipeOrgan          |  48,758.5 ns |    896.87 ns |    469.08 ns |  1.36 |    0.02 |    2 |         - |          NA |
| RadixLSD10Sort      | 8192 | PipeOrgan          | 166,751.7 ns |  2,702.71 ns |  1,413.57 ns |  4.65 |    0.07 |    6 |         - |          NA |
| RadixMSD4Sort       | 8192 | PipeOrgan          | 169,369.3 ns |  1,809.41 ns |    946.35 ns |  4.72 |    0.07 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | PipeOrgan          | 171,943.2 ns |    923.22 ns |    409.91 ns |  4.80 |    0.06 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | PipeOrgan          | 121,886.4 ns |    714.39 ns |    373.64 ns |  3.40 |    0.04 |    5 |         - |          NA |
| SpreadSort          | 8192 | PipeOrgan          |  96,205.0 ns |    600.19 ns |    266.49 ns |  2.68 |    0.03 |    4 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **CountingSort**        | **8192** | **ManyDuplicates**     |  **46,141.5 ns** |  **1,616.12 ns** |    **845.26 ns** |  **1.70** |    **0.04** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 8192 | ManyDuplicates     |  27,076.6 ns |  1,293.54 ns |    574.34 ns |  1.00 |    0.03 |    2 |         - |          NA |
| PigeonSort          | 8192 | ManyDuplicates     |  75,514.8 ns |    785.12 ns |    410.63 ns |  2.79 |    0.06 |    4 |         - |          NA |
| PigeonSortInteger   | 8192 | ManyDuplicates     |  19,902.2 ns |    109.32 ns |     48.54 ns |  0.74 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | ManyDuplicates     |  97,039.6 ns |  1,183.45 ns |    525.46 ns |  3.59 |    0.07 |    4 |         - |          NA |
| BucketSortInteger   | 8192 | ManyDuplicates     |  53,836.8 ns |    617.66 ns |    274.25 ns |  1.99 |    0.04 |    4 |         - |          NA |
| FlashSort           | 8192 | ManyDuplicates     | 147,315.8 ns |  1,474.15 ns |    771.01 ns |  5.44 |    0.11 |    5 |         - |          NA |
| RadixLSD4Sort       | 8192 | ManyDuplicates     |  73,128.8 ns |  1,655.55 ns |    735.08 ns |  2.70 |    0.06 |    4 |         - |          NA |
| RadixLSD256Sort     | 8192 | ManyDuplicates     |  32,464.1 ns |    326.42 ns |    170.72 ns |  1.20 |    0.02 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | ManyDuplicates     |  91,521.7 ns |    575.53 ns |    301.01 ns |  3.38 |    0.07 |    4 |         - |          NA |
| RadixMSD4Sort       | 8192 | ManyDuplicates     |  79,673.0 ns |    978.21 ns |    511.62 ns |  2.94 |    0.06 |    4 |         - |          NA |
| RadixMSD10Sort      | 8192 | ManyDuplicates     |  98,993.7 ns |    939.88 ns |    491.58 ns |  3.66 |    0.07 |    4 |         - |          NA |
| AmericanFlagSort    | 8192 | ManyDuplicates     |  61,443.6 ns |    696.44 ns |    364.25 ns |  2.27 |    0.05 |    4 |         - |          NA |
| SpreadSort          | 8192 | ManyDuplicates     |  53,127.4 ns |    545.81 ns |    285.47 ns |  1.96 |    0.04 |    4 |         - |          NA |

### ExchangeBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.87GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method             | Size | Pattern            | Mean         | Error       | StdDev      | Ratio  | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------- |----- |------------------- |-------------:|------------:|------------:|-------:|--------:|-----:|----------:|------------:|
| **BubbleSort**         | **256**  | **Random**             |  **28,048.2 ns** |   **416.53 ns** |   **184.94 ns** |   **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Random             |  16,764.5 ns |   172.22 ns |    90.07 ns |   0.60 |    0.00 |    2 |         - |          NA |
| OddEvenSort        | 256  | Random             |  21,760.0 ns | 1,199.90 ns |   627.57 ns |   0.78 |    0.02 |    2 |         - |          NA |
| CombSort           | 256  | Random             |   3,617.6 ns |   221.38 ns |   115.79 ns |   0.13 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | Random             |  18,478.3 ns |   356.29 ns |   186.35 ns |   0.66 |    0.01 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **SingleElementMoved** |     **416.0 ns** |     **3.95 ns** |     **2.07 ns** |   **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 256  | SingleElementMoved |     318.2 ns |     2.78 ns |     1.45 ns |   0.76 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 256  | SingleElementMoved |  17,351.8 ns |    95.65 ns |    42.47 ns |  41.71 |    0.22 |    4 |         - |          NA |
| CombSort           | 256  | SingleElementMoved |   3,016.6 ns |     7.42 ns |     3.88 ns |   7.25 |    0.03 |    3 |         - |          NA |
| CircleSort         | 256  | SingleElementMoved |  20,755.4 ns |   147.80 ns |    77.30 ns |  49.90 |    0.29 |    4 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **Sorted**             |     **245.9 ns** |   **107.28 ns** |    **56.11 ns** |   **1.04** |    **0.30** |    **1** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Sorted             |     196.6 ns |     8.73 ns |     3.88 ns |   0.83 |    0.16 |    1 |         - |          NA |
| OddEvenSort        | 256  | Sorted             |     250.0 ns |   105.00 ns |    46.62 ns |   1.06 |    0.28 |    1 |         - |          NA |
| CombSort           | 256  | Sorted             |   2,862.8 ns |   200.89 ns |    89.20 ns |  12.13 |    2.37 |    3 |         - |          NA |
| CircleSort         | 256  | Sorted             |   2,204.6 ns |    56.98 ns |    25.30 ns |   9.34 |    1.81 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **Reversed**           |  **29,373.3 ns** |   **142.11 ns** |    **74.33 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Reversed           |  27,949.9 ns |   347.43 ns |   181.71 ns |   0.95 |    0.01 |    3 |         - |          NA |
| OddEvenSort        | 256  | Reversed           |  25,268.2 ns |   171.60 ns |    89.75 ns |   0.86 |    0.00 |    3 |         - |          NA |
| CombSort           | 256  | Reversed           |   2,889.8 ns |    50.08 ns |    17.86 ns |   0.10 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | Reversed           |   4,532.6 ns |   301.67 ns |   157.78 ns |   0.15 |    0.01 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **PipeOrgan**          |  **34,628.3 ns** |   **256.62 ns** |   **113.94 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 256  | PipeOrgan          |  17,815.1 ns |    78.08 ns |    34.67 ns |   0.51 |    0.00 |    2 |         - |          NA |
| OddEvenSort        | 256  | PipeOrgan          |  27,936.5 ns |   122.07 ns |    54.20 ns |   0.81 |    0.00 |    3 |         - |          NA |
| CombSort           | 256  | PipeOrgan          |   3,094.7 ns |    71.84 ns |    25.62 ns |   0.09 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | PipeOrgan          |  20,056.3 ns |   354.29 ns |   185.30 ns |   0.58 |    0.01 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **256**  | **ManyDuplicates**     |  **29,067.4 ns** |   **476.39 ns** |   **211.52 ns** |   **1.00** |    **0.01** |    **5** |         **-** |          **NA** |
| CocktailShakerSort | 256  | ManyDuplicates     |  17,168.6 ns |   213.21 ns |   111.51 ns |   0.59 |    0.01 |    3 |         - |          NA |
| OddEvenSort        | 256  | ManyDuplicates     |  21,555.1 ns |   342.47 ns |   152.06 ns |   0.74 |    0.01 |    4 |         - |          NA |
| CombSort           | 256  | ManyDuplicates     |   3,270.2 ns |    35.14 ns |    12.53 ns |   0.11 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | ManyDuplicates     |  13,969.6 ns |   298.85 ns |   132.69 ns |   0.48 |    0.01 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Random**             | **534,806.5 ns** | **3,530.75 ns** | **1,846.65 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Random             | 324,837.8 ns | 2,809.74 ns | 1,469.55 ns |   0.61 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | Random             | 473,532.1 ns | 4,195.98 ns | 2,194.58 ns |   0.89 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | Random             |  19,714.1 ns |   330.50 ns |   172.86 ns |   0.04 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | Random             | 100,984.2 ns | 1,438.18 ns |   752.20 ns |   0.19 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **SingleElementMoved** |   **1,716.3 ns** |    **45.27 ns** |    **20.10 ns** |   **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | SingleElementMoved |   1,294.5 ns |     2.46 ns |     1.09 ns |   0.75 |    0.01 |    1 |         - |          NA |
| OddEvenSort        | 1024 | SingleElementMoved | 252,664.8 ns | 2,916.57 ns | 1,525.42 ns | 147.23 |    1.81 |    5 |         - |          NA |
| CombSort           | 1024 | SingleElementMoved |  15,441.9 ns |   365.15 ns |   190.98 ns |   9.00 |    0.14 |    3 |         - |          NA |
| CircleSort         | 1024 | SingleElementMoved |  87,474.7 ns |   484.41 ns |   253.35 ns |  50.97 |    0.57 |    4 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Sorted**             |     **722.4 ns** |     **1.00 ns** |     **0.44 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Sorted             |     733.2 ns |     2.55 ns |     1.13 ns |   1.02 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 1024 | Sorted             |     905.8 ns |    53.97 ns |    19.25 ns |   1.25 |    0.02 |    2 |         - |          NA |
| CombSort           | 1024 | Sorted             |  14,633.0 ns |   152.41 ns |    79.71 ns |  20.26 |    0.10 |    4 |         - |          NA |
| CircleSort         | 1024 | Sorted             |   9,995.7 ns |   241.47 ns |   126.29 ns |  13.84 |    0.17 |    3 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Reversed**           | **437,864.6 ns** | **1,610.65 ns** |   **842.40 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Reversed           | 436,272.9 ns | 1,324.06 ns |   587.89 ns |   1.00 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | Reversed           | 408,015.8 ns | 2,288.83 ns | 1,197.10 ns |   0.93 |    0.00 |    3 |         - |          NA |
| CombSort           | 1024 | Reversed           |  15,622.0 ns |    92.71 ns |    41.16 ns |   0.04 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | Reversed           |  19,105.0 ns |   324.35 ns |   169.64 ns |   0.04 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **PipeOrgan**          | **506,955.9 ns** | **1,320.32 ns** |   **690.55 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | PipeOrgan          | 285,005.0 ns | 1,819.24 ns |   951.50 ns |   0.56 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | PipeOrgan          | 449,499.9 ns | 2,531.54 ns | 1,324.05 ns |   0.89 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | PipeOrgan          |  15,879.4 ns |   204.01 ns |    90.58 ns |   0.03 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | PipeOrgan          | 105,998.8 ns |   643.55 ns |   285.74 ns |   0.21 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **BubbleSort**         | **1024** | **ManyDuplicates**     | **540,152.8 ns** | **1,565.13 ns** |   **818.59 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | ManyDuplicates     | 318,852.2 ns | 1,242.26 ns |   649.73 ns |   0.59 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | ManyDuplicates     | 470,082.3 ns | 3,407.03 ns | 1,512.74 ns |   0.87 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | ManyDuplicates     |  16,868.4 ns |   376.47 ns |   196.90 ns |   0.03 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | ManyDuplicates     |  90,778.5 ns | 3,051.60 ns | 1,596.04 ns |   0.17 |    0.00 |    2 |         - |          NA |

### HeapBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.87GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method           | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **HeapSort**         | **256**  | **Random**             |     **3,408.8 ns** |    **210.23 ns** |     **93.34 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | Random             |     3,443.9 ns |     56.93 ns |     25.28 ns |  1.01 |    0.03 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | Random             |     4,150.5 ns |    422.91 ns |    221.19 ns |  1.22 |    0.07 |    1 |         - |          NA |
| BottomupHeapSort | 256  | Random             |     4,408.2 ns |    324.58 ns |    144.12 ns |  1.29 |    0.05 |    1 |         - |          NA |
| WeakHeapSort     | 256  | Random             |    10,289.5 ns |    385.91 ns |    201.84 ns |  3.02 |    0.09 |    4 |         - |          NA |
| SmoothSort       | 256  | Random             |     5,635.2 ns |    353.85 ns |    185.07 ns |  1.65 |    0.07 |    2 |         - |          NA |
| TournamentSort   | 256  | Random             |     8,167.6 ns |    406.81 ns |    180.63 ns |  2.40 |    0.08 |    3 |         - |          NA |
| BinomialHeapSort | 256  | Random             |    15,821.8 ns |    263.84 ns |    117.15 ns |  4.64 |    0.12 |    5 |         - |          NA |
| PairingHeapSort  | 256  | Random             |    11,969.6 ns |    314.42 ns |    164.45 ns |  3.51 |    0.10 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **SingleElementMoved** |     **3,214.0 ns** |    **323.13 ns** |    **169.00 ns** |  **1.00** |    **0.07** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 256  | SingleElementMoved |     3,073.8 ns |     28.65 ns |     10.22 ns |  0.96 |    0.05 |    2 |         - |          NA |
| TernaryHeapSort  | 256  | SingleElementMoved |     4,255.3 ns |    293.48 ns |    130.31 ns |  1.33 |    0.08 |    3 |         - |          NA |
| BottomupHeapSort | 256  | SingleElementMoved |     4,462.8 ns |    285.38 ns |    149.26 ns |  1.39 |    0.08 |    3 |         - |          NA |
| WeakHeapSort     | 256  | SingleElementMoved |     8,685.5 ns |     83.94 ns |     37.27 ns |  2.71 |    0.13 |    5 |         - |          NA |
| SmoothSort       | 256  | SingleElementMoved |     1,754.4 ns |     34.98 ns |     15.53 ns |  0.55 |    0.03 |    1 |         - |          NA |
| TournamentSort   | 256  | SingleElementMoved |     5,485.4 ns |    360.91 ns |    188.76 ns |  1.71 |    0.10 |    4 |         - |          NA |
| BinomialHeapSort | 256  | SingleElementMoved |     7,263.3 ns |     59.65 ns |     21.27 ns |  2.27 |    0.11 |    5 |         - |          NA |
| PairingHeapSort  | 256  | SingleElementMoved |     5,572.6 ns |    226.40 ns |    118.41 ns |  1.74 |    0.09 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **Sorted**             |     **3,412.8 ns** |    **266.54 ns** |    **139.41 ns** |  **1.00** |    **0.05** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 256  | Sorted             |     3,616.1 ns |    146.44 ns |     65.02 ns |  1.06 |    0.04 |    2 |         - |          NA |
| TernaryHeapSort  | 256  | Sorted             |     4,324.6 ns |    310.23 ns |    162.26 ns |  1.27 |    0.07 |    2 |         - |          NA |
| BottomupHeapSort | 256  | Sorted             |     4,332.4 ns |    146.48 ns |     65.04 ns |  1.27 |    0.05 |    2 |         - |          NA |
| WeakHeapSort     | 256  | Sorted             |     8,771.4 ns |    365.09 ns |    162.10 ns |  2.57 |    0.11 |    4 |         - |          NA |
| SmoothSort       | 256  | Sorted             |     1,297.2 ns |     15.61 ns |      6.93 ns |  0.38 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 256  | Sorted             |     4,579.5 ns |    346.26 ns |    181.10 ns |  1.34 |    0.07 |    2 |         - |          NA |
| BinomialHeapSort | 256  | Sorted             |     6,644.4 ns |    338.67 ns |    177.13 ns |  1.95 |    0.09 |    3 |         - |          NA |
| PairingHeapSort  | 256  | Sorted             |     5,404.7 ns |     33.21 ns |     11.84 ns |  1.59 |    0.06 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **Reversed**           |     **3,163.1 ns** |     **55.56 ns** |     **19.81 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | Reversed           |     2,714.4 ns |    192.82 ns |    100.85 ns |  0.86 |    0.03 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | Reversed           |     4,461.6 ns |    281.82 ns |    147.40 ns |  1.41 |    0.04 |    2 |         - |          NA |
| BottomupHeapSort | 256  | Reversed           |     4,686.0 ns |    332.46 ns |    173.88 ns |  1.48 |    0.05 |    2 |         - |          NA |
| WeakHeapSort     | 256  | Reversed           |     9,687.7 ns |    173.93 ns |     77.22 ns |  3.06 |    0.03 |    4 |         - |          NA |
| SmoothSort       | 256  | Reversed           |     5,122.3 ns |    428.41 ns |    224.06 ns |  1.62 |    0.07 |    2 |         - |          NA |
| TournamentSort   | 256  | Reversed           |     5,263.3 ns |    770.78 ns |    403.13 ns |  1.66 |    0.12 |    2 |         - |          NA |
| BinomialHeapSort | 256  | Reversed           |     6,579.4 ns |     72.30 ns |     37.81 ns |  2.08 |    0.02 |    3 |         - |          NA |
| PairingHeapSort  | 256  | Reversed           |     2,721.1 ns |    203.65 ns |     90.42 ns |  0.86 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **PipeOrgan**          |     **3,083.4 ns** |    **163.96 ns** |     **85.76 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | PipeOrgan          |     2,998.5 ns |     85.08 ns |     37.78 ns |  0.97 |    0.03 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | PipeOrgan          |     4,250.6 ns |    275.62 ns |    122.38 ns |  1.38 |    0.05 |    2 |         - |          NA |
| BottomupHeapSort | 256  | PipeOrgan          |     4,531.6 ns |    362.94 ns |    189.83 ns |  1.47 |    0.07 |    2 |         - |          NA |
| WeakHeapSort     | 256  | PipeOrgan          |     9,259.0 ns |    208.10 ns |    108.84 ns |  3.00 |    0.09 |    4 |         - |          NA |
| SmoothSort       | 256  | PipeOrgan          |     5,062.1 ns |    335.60 ns |    175.52 ns |  1.64 |    0.07 |    2 |         - |          NA |
| TournamentSort   | 256  | PipeOrgan          |     6,755.7 ns |    433.09 ns |    192.29 ns |  2.19 |    0.08 |    3 |         - |          NA |
| BinomialHeapSort | 256  | PipeOrgan          |     7,492.0 ns |     52.87 ns |     23.48 ns |  2.43 |    0.06 |    3 |         - |          NA |
| PairingHeapSort  | 256  | PipeOrgan          |     7,043.1 ns |    208.67 ns |     92.65 ns |  2.29 |    0.07 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **ManyDuplicates**     |     **3,386.6 ns** |    **245.90 ns** |    **128.61 ns** |  **1.00** |    **0.05** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | ManyDuplicates     |     3,463.6 ns |    251.94 ns |    131.77 ns |  1.02 |    0.05 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | ManyDuplicates     |     4,094.6 ns |    301.64 ns |    157.76 ns |  1.21 |    0.06 |    1 |         - |          NA |
| BottomupHeapSort | 256  | ManyDuplicates     |     4,376.7 ns |    229.23 ns |    119.89 ns |  1.29 |    0.06 |    1 |         - |          NA |
| WeakHeapSort     | 256  | ManyDuplicates     |     9,892.9 ns |    323.54 ns |    143.65 ns |  2.92 |    0.11 |    2 |         - |          NA |
| SmoothSort       | 256  | ManyDuplicates     |     5,120.6 ns |    268.75 ns |    140.56 ns |  1.51 |    0.07 |    1 |         - |          NA |
| TournamentSort   | 256  | ManyDuplicates     |     8,298.8 ns |    339.23 ns |    150.62 ns |  2.45 |    0.10 |    2 |         - |          NA |
| BinomialHeapSort | 256  | ManyDuplicates     |    13,957.3 ns |    533.31 ns |    236.79 ns |  4.13 |    0.16 |    3 |         - |          NA |
| PairingHeapSort  | 256  | ManyDuplicates     |    10,994.3 ns |    474.78 ns |    248.32 ns |  3.25 |    0.14 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Random**             |    **17,967.5 ns** |    **784.73 ns** |    **410.43 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Random             |    18,041.6 ns |    583.34 ns |    305.10 ns |  1.00 |    0.03 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | Random             |    20,057.8 ns |    701.79 ns |    311.60 ns |  1.12 |    0.03 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | Random             |    20,338.1 ns |    655.04 ns |    342.60 ns |  1.13 |    0.03 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | Random             |    53,531.5 ns |    622.80 ns |    276.53 ns |  2.98 |    0.06 |    4 |         - |          NA |
| SmoothSort       | 1024 | Random             |    27,331.7 ns |    258.99 ns |     92.36 ns |  1.52 |    0.03 |    2 |         - |          NA |
| TournamentSort   | 1024 | Random             |    42,713.2 ns |  5,695.57 ns |  2,978.89 ns |  2.38 |    0.16 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | Random             |    88,013.0 ns | 15,867.48 ns |  8,299.00 ns |  4.90 |    0.45 |    5 |         - |          NA |
| PairingHeapSort  | 1024 | Random             |    56,253.6 ns |  1,520.18 ns |    795.08 ns |  3.13 |    0.08 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **SingleElementMoved** |    **15,254.9 ns** |    **389.78 ns** |    **173.06 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | SingleElementMoved |    15,416.6 ns |    231.43 ns |    102.76 ns |  1.01 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | SingleElementMoved |    20,642.6 ns |    570.10 ns |    298.17 ns |  1.35 |    0.02 |    3 |         - |          NA |
| BottomupHeapSort | 1024 | SingleElementMoved |    20,279.9 ns |    496.31 ns |    259.58 ns |  1.33 |    0.02 |    3 |         - |          NA |
| WeakHeapSort     | 1024 | SingleElementMoved |    43,844.5 ns |    232.50 ns |    121.60 ns |  2.87 |    0.03 |    6 |         - |          NA |
| SmoothSort       | 1024 | SingleElementMoved |     7,089.2 ns |     66.17 ns |     29.38 ns |  0.46 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 1024 | SingleElementMoved |    27,322.6 ns |  2,868.68 ns |  1,273.71 ns |  1.79 |    0.08 |    4 |         - |          NA |
| BinomialHeapSort | 1024 | SingleElementMoved |    32,449.4 ns |    355.90 ns |    186.14 ns |  2.13 |    0.03 |    5 |         - |          NA |
| PairingHeapSort  | 1024 | SingleElementMoved |    22,214.3 ns |    223.48 ns |    116.88 ns |  1.46 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Sorted**             |    **16,628.2 ns** |    **238.22 ns** |    **105.77 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Sorted             |    17,178.3 ns |    300.66 ns |    133.49 ns |  1.03 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | Sorted             |    19,607.4 ns |  1,096.75 ns |    573.62 ns |  1.18 |    0.03 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | Sorted             |    20,854.2 ns |    882.31 ns |    461.47 ns |  1.25 |    0.03 |    2 |         - |          NA |
| WeakHeapSort     | 1024 | Sorted             |    44,283.5 ns |    177.09 ns |     78.63 ns |  2.66 |    0.02 |    4 |         - |          NA |
| SmoothSort       | 1024 | Sorted             |     5,313.3 ns |    356.31 ns |    186.36 ns |  0.32 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 1024 | Sorted             |    20,350.6 ns |    413.80 ns |    183.73 ns |  1.22 |    0.01 |    2 |         - |          NA |
| BinomialHeapSort | 1024 | Sorted             |    29,319.6 ns |    290.68 ns |    152.03 ns |  1.76 |    0.01 |    3 |         - |          NA |
| PairingHeapSort  | 1024 | Sorted             |    22,457.5 ns |    228.11 ns |    101.28 ns |  1.35 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Reversed**           |    **16,177.9 ns** |  **1,501.82 ns** |    **785.48 ns** |  **1.00** |    **0.06** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Reversed           |    15,481.9 ns |    317.28 ns |    165.94 ns |  0.96 |    0.04 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | Reversed           |    20,795.8 ns |    476.56 ns |    249.25 ns |  1.29 |    0.06 |    3 |         - |          NA |
| BottomupHeapSort | 1024 | Reversed           |    20,852.0 ns |    496.09 ns |    259.46 ns |  1.29 |    0.06 |    3 |         - |          NA |
| WeakHeapSort     | 1024 | Reversed           |    48,358.2 ns |    387.80 ns |    202.83 ns |  3.00 |    0.14 |    4 |         - |          NA |
| SmoothSort       | 1024 | Reversed           |    24,830.0 ns |    992.42 ns |    519.05 ns |  1.54 |    0.08 |    3 |         - |          NA |
| TournamentSort   | 1024 | Reversed           |    26,108.7 ns |  1,848.80 ns |    966.96 ns |  1.62 |    0.09 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | Reversed           |    28,969.8 ns |    106.54 ns |     55.72 ns |  1.79 |    0.08 |    3 |         - |          NA |
| PairingHeapSort  | 1024 | Reversed           |    10,634.9 ns |    412.48 ns |    183.15 ns |  0.66 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **PipeOrgan**          |    **15,328.6 ns** |    **876.27 ns** |    **458.31 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | PipeOrgan          |    15,220.0 ns |    445.71 ns |    197.90 ns |  0.99 |    0.03 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | PipeOrgan          |    20,315.2 ns |    371.69 ns |    165.03 ns |  1.33 |    0.04 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | PipeOrgan          |    20,163.1 ns |    460.37 ns |    204.41 ns |  1.32 |    0.04 |    2 |         - |          NA |
| WeakHeapSort     | 1024 | PipeOrgan          |    47,790.7 ns |    484.08 ns |    253.19 ns |  3.12 |    0.09 |    4 |         - |          NA |
| SmoothSort       | 1024 | PipeOrgan          |    26,911.3 ns |    558.38 ns |    292.04 ns |  1.76 |    0.05 |    3 |         - |          NA |
| TournamentSort   | 1024 | PipeOrgan          |    33,774.3 ns |  2,931.39 ns |  1,533.17 ns |  2.21 |    0.11 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | PipeOrgan          |    32,766.0 ns |    327.39 ns |    171.23 ns |  2.14 |    0.06 |    3 |         - |          NA |
| PairingHeapSort  | 1024 | PipeOrgan          |    29,347.8 ns |    396.08 ns |    175.86 ns |  1.92 |    0.06 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **ManyDuplicates**     |    **18,188.1 ns** |    **335.20 ns** |    **148.83 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | ManyDuplicates     |    17,803.1 ns |    302.17 ns |    134.17 ns |  0.98 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | ManyDuplicates     |    19,627.7 ns |    464.54 ns |    206.26 ns |  1.08 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | ManyDuplicates     |    20,271.5 ns |    588.91 ns |    308.01 ns |  1.11 |    0.02 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | ManyDuplicates     |    48,368.6 ns |    526.31 ns |    233.68 ns |  2.66 |    0.02 |    4 |         - |          NA |
| SmoothSort       | 1024 | ManyDuplicates     |    24,635.0 ns |    935.88 ns |    489.48 ns |  1.35 |    0.03 |    2 |         - |          NA |
| TournamentSort   | 1024 | ManyDuplicates     |    38,356.5 ns |    742.92 ns |    329.86 ns |  2.11 |    0.02 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | ManyDuplicates     |    69,079.5 ns |  2,220.06 ns |  1,161.13 ns |  3.80 |    0.07 |    5 |         - |          NA |
| PairingHeapSort  | 1024 | ManyDuplicates     |    52,399.0 ns |  2,289.41 ns |  1,197.41 ns |  2.88 |    0.07 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Random**             |   **183,587.8 ns** |  **1,527.59 ns** |    **678.26 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Random             |   190,816.8 ns |  1,536.11 ns |    803.41 ns |  1.04 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 4096 | Random             |   139,850.8 ns |  4,047.15 ns |  2,116.74 ns |  0.76 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 4096 | Random             |   128,647.4 ns |  6,198.78 ns |  2,752.29 ns |  0.70 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | Random             |   345,922.7 ns | 31,259.32 ns | 16,349.23 ns |  1.88 |    0.08 |    3 |         - |          NA |
| SmoothSort       | 4096 | Random             |   391,325.7 ns |  4,116.05 ns |  2,152.77 ns |  2.13 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 4096 | Random             |   667,132.5 ns |  2,401.13 ns |    856.27 ns |  3.63 |    0.01 |    4 |         - |          NA |
| BinomialHeapSort | 4096 | Random             | 1,042,792.1 ns |  7,176.67 ns |  3,753.54 ns |  5.68 |    0.03 |    5 |         - |          NA |
| PairingHeapSort  | 4096 | Random             |   466,402.7 ns | 12,352.22 ns |  6,460.45 ns |  2.54 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **SingleElementMoved** |   **106,002.7 ns** |  **3,197.34 ns** |  **1,672.27 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 4096 | SingleElementMoved |   139,397.6 ns |  5,765.34 ns |  3,015.38 ns |  1.32 |    0.03 |    3 |         - |          NA |
| TernaryHeapSort  | 4096 | SingleElementMoved |   101,286.3 ns |  2,872.19 ns |  1,275.27 ns |  0.96 |    0.02 |    2 |         - |          NA |
| BottomupHeapSort | 4096 | SingleElementMoved |   105,431.5 ns |  1,477.69 ns |    656.10 ns |  0.99 |    0.02 |    2 |         - |          NA |
| WeakHeapSort     | 4096 | SingleElementMoved |   213,880.9 ns |    464.35 ns |    242.87 ns |  2.02 |    0.03 |    4 |         - |          NA |
| SmoothSort       | 4096 | SingleElementMoved |    29,292.4 ns |    969.86 ns |    507.25 ns |  0.28 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 4096 | SingleElementMoved |   308,044.2 ns | 24,200.19 ns | 12,657.17 ns |  2.91 |    0.12 |    5 |         - |          NA |
| BinomialHeapSort | 4096 | SingleElementMoved |   143,206.4 ns |    834.74 ns |    370.63 ns |  1.35 |    0.02 |    3 |         - |          NA |
| PairingHeapSort  | 4096 | SingleElementMoved |    90,063.6 ns |    536.86 ns |    280.79 ns |  0.85 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Sorted**             |   **125,489.3 ns** |  **4,054.09 ns** |  **2,120.37 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Sorted             |   156,684.4 ns |  2,992.30 ns |  1,565.03 ns |  1.25 |    0.02 |    3 |         - |          NA |
| TernaryHeapSort  | 4096 | Sorted             |    99,000.9 ns |  5,549.82 ns |  2,902.67 ns |  0.79 |    0.03 |    2 |         - |          NA |
| BottomupHeapSort | 4096 | Sorted             |   102,847.2 ns |  2,064.89 ns |  1,079.98 ns |  0.82 |    0.02 |    2 |         - |          NA |
| WeakHeapSort     | 4096 | Sorted             |   215,768.0 ns |    982.43 ns |    513.83 ns |  1.72 |    0.03 |    4 |         - |          NA |
| SmoothSort       | 4096 | Sorted             |    21,290.4 ns |    442.58 ns |    196.51 ns |  0.17 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 4096 | Sorted             |   145,831.5 ns | 20,762.67 ns | 10,859.28 ns |  1.16 |    0.08 |    3 |         - |          NA |
| BinomialHeapSort | 4096 | Sorted             |   131,023.7 ns |    756.07 ns |    335.70 ns |  1.04 |    0.02 |    3 |         - |          NA |
| PairingHeapSort  | 4096 | Sorted             |    89,831.1 ns |    656.89 ns |    291.66 ns |  0.72 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Reversed**           |   **115,063.9 ns** |  **6,512.96 ns** |  **2,891.80 ns** |  **1.00** |    **0.03** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Reversed           |   131,366.0 ns |  1,960.98 ns |    870.69 ns |  1.14 |    0.03 |    2 |         - |          NA |
| TernaryHeapSort  | 4096 | Reversed           |    98,763.0 ns |  1,423.51 ns |    632.05 ns |  0.86 |    0.02 |    2 |         - |          NA |
| BottomupHeapSort | 4096 | Reversed           |   104,110.4 ns |  1,607.46 ns |    713.72 ns |  0.91 |    0.02 |    2 |         - |          NA |
| WeakHeapSort     | 4096 | Reversed           |   232,071.4 ns |    551.63 ns |    288.51 ns |  2.02 |    0.05 |    3 |         - |          NA |
| SmoothSort       | 4096 | Reversed           |   133,286.3 ns |  5,219.89 ns |  2,730.10 ns |  1.16 |    0.03 |    2 |         - |          NA |
| TournamentSort   | 4096 | Reversed           |   226,172.5 ns | 65,339.64 ns | 34,173.90 ns |  1.97 |    0.28 |    3 |         - |          NA |
| BinomialHeapSort | 4096 | Reversed           |   128,410.1 ns |  1,128.69 ns |    501.14 ns |  1.12 |    0.03 |    2 |         - |          NA |
| PairingHeapSort  | 4096 | Reversed           |    42,364.6 ns |    718.44 ns |    318.99 ns |  0.37 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **PipeOrgan**          |   **113,033.8 ns** | **18,511.96 ns** |  **9,682.11 ns** |  **1.01** |    **0.11** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 4096 | PipeOrgan          |   121,887.3 ns |  5,946.41 ns |  2,640.24 ns |  1.08 |    0.09 |    1 |         - |          NA |
| TernaryHeapSort  | 4096 | PipeOrgan          |    98,358.3 ns |  3,574.65 ns |  1,869.61 ns |  0.88 |    0.07 |    1 |         - |          NA |
| BottomupHeapSort | 4096 | PipeOrgan          |   100,961.5 ns |    892.91 ns |    318.42 ns |  0.90 |    0.07 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | PipeOrgan          |   232,955.7 ns |    929.27 ns |    486.02 ns |  2.07 |    0.16 |    2 |         - |          NA |
| SmoothSort       | 4096 | PipeOrgan          |   281,634.6 ns |  2,759.27 ns |  1,443.15 ns |  2.51 |    0.19 |    3 |         - |          NA |
| TournamentSort   | 4096 | PipeOrgan          |   459,273.4 ns |  8,097.91 ns |  4,235.36 ns |  4.09 |    0.31 |    4 |         - |          NA |
| BinomialHeapSort | 4096 | PipeOrgan          |   143,068.8 ns |    982.86 ns |    514.06 ns |  1.27 |    0.10 |    1 |         - |          NA |
| PairingHeapSort  | 4096 | PipeOrgan          |   120,427.9 ns |    625.20 ns |    326.99 ns |  1.07 |    0.08 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **ManyDuplicates**     |   **173,541.8 ns** |  **3,028.71 ns** |  **1,584.07 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 4096 | ManyDuplicates     |   176,470.8 ns |  2,678.32 ns |  1,400.81 ns |  1.02 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 4096 | ManyDuplicates     |   102,381.0 ns |  1,083.60 ns |    481.12 ns |  0.59 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 4096 | ManyDuplicates     |   112,390.8 ns |  2,578.93 ns |  1,145.06 ns |  0.65 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | ManyDuplicates     |   235,352.7 ns |  5,050.91 ns |  2,641.72 ns |  1.36 |    0.02 |    3 |         - |          NA |
| SmoothSort       | 4096 | ManyDuplicates     |   320,240.3 ns |  2,446.17 ns |  1,086.11 ns |  1.85 |    0.02 |    4 |         - |          NA |
| TournamentSort   | 4096 | ManyDuplicates     |   611,500.2 ns |  4,784.95 ns |  2,124.55 ns |  3.52 |    0.03 |    6 |         - |          NA |
| BinomialHeapSort | 4096 | ManyDuplicates     |   719,869.8 ns |  4,514.27 ns |  2,004.36 ns |  4.15 |    0.04 |    6 |         - |          NA |
| PairingHeapSort  | 4096 | ManyDuplicates     |   410,355.3 ns |  3,217.21 ns |  1,428.46 ns |  2.36 |    0.02 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Random**             |   **531,372.2 ns** |  **3,427.32 ns** |  **1,792.55 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Random             |   520,877.5 ns |  6,147.55 ns |  3,215.29 ns |  0.98 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | Random             |   655,921.4 ns |  5,750.65 ns |  3,007.70 ns |  1.23 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | Random             |   658,944.7 ns |  4,181.39 ns |  2,186.95 ns |  1.24 |    0.01 |    2 |         - |          NA |
| WeakHeapSort     | 8192 | Random             |   981,134.6 ns |  2,485.71 ns |  1,300.08 ns |  1.85 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 8192 | Random             |   937,843.2 ns |  1,887.89 ns |    987.40 ns |  1.76 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 8192 | Random             | 1,495,666.7 ns |  9,202.42 ns |  4,085.93 ns |  2.81 |    0.01 |    4 |         - |          NA |
| BinomialHeapSort | 8192 | Random             | 2,323,781.8 ns |  7,397.64 ns |  3,869.11 ns |  4.37 |    0.02 |    5 |         - |          NA |
| PairingHeapSort  | 8192 | Random             | 1,116,915.1 ns |  4,110.55 ns |  2,149.89 ns |  2.10 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **SingleElementMoved** |   **354,569.1 ns** | **37,067.30 ns** | **16,458.11 ns** |  **1.00** |    **0.06** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 8192 | SingleElementMoved |   392,779.0 ns |  2,669.74 ns |  1,396.32 ns |  1.11 |    0.05 |    4 |         - |          NA |
| TernaryHeapSort  | 8192 | SingleElementMoved |   397,871.0 ns |  2,408.05 ns |  1,259.46 ns |  1.12 |    0.05 |    4 |         - |          NA |
| BottomupHeapSort | 8192 | SingleElementMoved |   414,301.3 ns |  2,706.28 ns |  1,201.61 ns |  1.17 |    0.06 |    4 |         - |          NA |
| WeakHeapSort     | 8192 | SingleElementMoved |   468,124.4 ns |  1,745.12 ns |    912.73 ns |  1.32 |    0.06 |    4 |         - |          NA |
| SmoothSort       | 8192 | SingleElementMoved |    58,107.8 ns |    962.16 ns |    427.20 ns |  0.16 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 8192 | SingleElementMoved |   795,402.9 ns | 17,980.76 ns |  9,404.29 ns |  2.25 |    0.11 |    5 |         - |          NA |
| BinomialHeapSort | 8192 | SingleElementMoved |   296,726.2 ns |  1,062.08 ns |    378.75 ns |  0.84 |    0.04 |    3 |         - |          NA |
| PairingHeapSort  | 8192 | SingleElementMoved |   184,852.1 ns |  1,125.47 ns |    588.64 ns |  0.52 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Sorted**             |   **326,222.7 ns** |  **1,560.29 ns** |    **816.06 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Sorted             |   402,783.8 ns |  2,248.65 ns |  1,176.09 ns |  1.23 |    0.00 |    3 |         - |          NA |
| TernaryHeapSort  | 8192 | Sorted             |   360,840.8 ns | 38,534.78 ns | 17,109.68 ns |  1.11 |    0.05 |    3 |         - |          NA |
| BottomupHeapSort | 8192 | Sorted             |   408,879.8 ns |  1,371.90 ns |    717.53 ns |  1.25 |    0.00 |    3 |         - |          NA |
| WeakHeapSort     | 8192 | Sorted             |   471,558.2 ns |  1,235.22 ns |    646.04 ns |  1.45 |    0.00 |    3 |         - |          NA |
| SmoothSort       | 8192 | Sorted             |    43,335.4 ns |  1,043.41 ns |    463.28 ns |  0.13 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 8192 | Sorted             |   517,984.7 ns | 23,136.07 ns | 12,100.61 ns |  1.59 |    0.04 |    3 |         - |          NA |
| BinomialHeapSort | 8192 | Sorted             |   291,303.0 ns | 61,159.48 ns | 27,155.19 ns |  0.89 |    0.08 |    3 |         - |          NA |
| PairingHeapSort  | 8192 | Sorted             |   182,777.4 ns |  4,691.82 ns |  1,673.15 ns |  0.56 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Reversed**           |   **388,458.3 ns** |    **974.93 ns** |    **432.87 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Reversed           |   346,888.7 ns |  2,789.90 ns |  1,459.17 ns |  0.89 |    0.00 |    3 |         - |          NA |
| TernaryHeapSort  | 8192 | Reversed           |   412,976.2 ns |  1,400.17 ns |    732.31 ns |  1.06 |    0.00 |    3 |         - |          NA |
| BottomupHeapSort | 8192 | Reversed           |   482,427.4 ns |  3,619.14 ns |  1,606.92 ns |  1.24 |    0.00 |    3 |         - |          NA |
| WeakHeapSort     | 8192 | Reversed           |   504,523.7 ns |  1,825.83 ns |    651.11 ns |  1.30 |    0.00 |    3 |         - |          NA |
| SmoothSort       | 8192 | Reversed           |   572,890.9 ns |  2,503.09 ns |  1,111.39 ns |  1.47 |    0.00 |    3 |         - |          NA |
| TournamentSort   | 8192 | Reversed           |   688,496.5 ns |  7,762.70 ns |  4,060.04 ns |  1.77 |    0.01 |    4 |         - |          NA |
| BinomialHeapSort | 8192 | Reversed           |   269,221.2 ns |    800.37 ns |    418.61 ns |  0.69 |    0.00 |    2 |         - |          NA |
| PairingHeapSort  | 8192 | Reversed           |    85,714.0 ns |    577.11 ns |    301.84 ns |  0.22 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **PipeOrgan**          |   **381,238.2 ns** | **11,954.25 ns** |  **6,252.30 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 8192 | PipeOrgan          |   406,565.9 ns |  2,704.42 ns |  1,414.46 ns |  1.07 |    0.02 |    2 |         - |          NA |
| TernaryHeapSort  | 8192 | PipeOrgan          |   465,717.1 ns |  1,411.66 ns |    626.79 ns |  1.22 |    0.02 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | PipeOrgan          |   474,084.6 ns |  1,215.02 ns |    539.48 ns |  1.24 |    0.02 |    2 |         - |          NA |
| WeakHeapSort     | 8192 | PipeOrgan          |   507,657.2 ns |  1,259.70 ns |    559.31 ns |  1.33 |    0.02 |    2 |         - |          NA |
| SmoothSort       | 8192 | PipeOrgan          |   710,283.3 ns |  2,817.19 ns |  1,250.85 ns |  1.86 |    0.03 |    3 |         - |          NA |
| TournamentSort   | 8192 | PipeOrgan          | 1,123,083.4 ns |  5,830.90 ns |  3,049.67 ns |  2.95 |    0.05 |    4 |         - |          NA |
| BinomialHeapSort | 8192 | PipeOrgan          |   297,343.6 ns |  1,041.59 ns |    544.77 ns |  0.78 |    0.01 |    1 |         - |          NA |
| PairingHeapSort  | 8192 | PipeOrgan          |   248,068.2 ns |  1,308.66 ns |    684.46 ns |  0.65 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **ManyDuplicates**     |   **511,276.8 ns** |  **7,891.40 ns** |  **4,127.35 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | ManyDuplicates     |   510,252.3 ns |  3,251.85 ns |  1,700.78 ns |  1.00 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | ManyDuplicates     |   592,494.3 ns |  4,050.14 ns |  2,118.30 ns |  1.16 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 8192 | ManyDuplicates     |   608,882.8 ns |  3,913.72 ns |  1,395.67 ns |  1.19 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | ManyDuplicates     |   676,102.9 ns |  2,053.89 ns |    911.94 ns |  1.32 |    0.01 |    1 |         - |          NA |
| SmoothSort       | 8192 | ManyDuplicates     |   790,497.9 ns |  2,526.37 ns |  1,321.34 ns |  1.55 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 8192 | ManyDuplicates     | 1,388,189.6 ns |  6,246.77 ns |  3,267.18 ns |  2.72 |    0.02 |    3 |         - |          NA |
| BinomialHeapSort | 8192 | ManyDuplicates     | 1,553,691.3 ns |  6,743.35 ns |  3,526.90 ns |  3.04 |    0.02 |    3 |         - |          NA |
| PairingHeapSort  | 8192 | ManyDuplicates     |   959,283.0 ns |  1,065.19 ns |    557.11 ns |  1.88 |    0.01 |    2 |         - |          NA |

### InsertionBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.87GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                 | Size | Pattern            | Mean         | Error        | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------- |----- |------------------- |-------------:|-------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **InsertionSort**          | **256**  | **Random**             |   **6,993.0 ns** |    **221.68 ns** |   **115.94 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Random             |   7,345.5 ns |    382.93 ns |   200.28 ns |  1.05 |    0.03 |    3 |         - |          NA |
| BinaryInsertSort       | 256  | Random             |   5,501.1 ns |    392.25 ns |   205.15 ns |  0.79 |    0.03 |    2 |         - |          NA |
| GnomeSort              | 256  | Random             |  24,157.8 ns |    288.85 ns |   128.25 ns |  3.46 |    0.06 |    5 |         - |          NA |
| LibrarySort            | 256  | Random             |  16,394.5 ns |    322.60 ns |   143.24 ns |  2.34 |    0.04 |    4 |         - |          NA |
| MergeInsertionSort     | 256  | Random             |  13,823.0 ns |    610.07 ns |   270.87 ns |  1.98 |    0.05 |    4 |         - |          NA |
| ShellSortKnuth1973     | 256  | Random             |   2,639.9 ns |     37.27 ns |    13.29 ns |  0.38 |    0.01 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Random             |   2,672.3 ns |     97.26 ns |    43.19 ns |  0.38 |    0.01 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | Random             |   2,708.0 ns |     23.30 ns |     8.31 ns |  0.39 |    0.01 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | Random             |   2,668.7 ns |    201.99 ns |   105.64 ns |  0.38 |    0.02 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Random             |   2,684.5 ns |    319.42 ns |   167.06 ns |  0.38 |    0.02 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **256**  | **SingleElementMoved** |     **445.6 ns** |      **4.45 ns** |     **1.97 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 256  | SingleElementMoved |     313.1 ns |     15.79 ns |     7.01 ns |  0.70 |    0.02 |    1 |         - |          NA |
| BinaryInsertSort       | 256  | SingleElementMoved |   1,291.8 ns |     23.08 ns |    12.07 ns |  2.90 |    0.03 |    3 |         - |          NA |
| GnomeSort              | 256  | SingleElementMoved |     518.7 ns |      9.45 ns |     4.20 ns |  1.16 |    0.01 |    2 |         - |          NA |
| LibrarySort            | 256  | SingleElementMoved |  15,460.3 ns |    140.23 ns |    73.34 ns | 34.70 |    0.21 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | SingleElementMoved |  12,282.4 ns |    269.67 ns |   141.04 ns | 27.57 |    0.32 |    4 |         - |          NA |
| ShellSortKnuth1973     | 256  | SingleElementMoved |   1,332.6 ns |      4.95 ns |     2.59 ns |  2.99 |    0.01 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 256  | SingleElementMoved |   1,322.8 ns |      8.85 ns |     3.16 ns |  2.97 |    0.01 |    3 |         - |          NA |
| ShellSortTokuda1992    | 256  | SingleElementMoved |   1,607.2 ns |      8.29 ns |     2.96 ns |  3.61 |    0.02 |    3 |         - |          NA |
| ShellSortCiura2001     | 256  | SingleElementMoved |   1,415.4 ns |     43.99 ns |    19.53 ns |  3.18 |    0.04 |    3 |         - |          NA |
| ShellSortLee2021       | 256  | SingleElementMoved |   1,761.3 ns |     33.20 ns |    14.74 ns |  3.95 |    0.04 |    3 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **256**  | **Sorted**             |     **365.4 ns** |      **2.41 ns** |     **1.26 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Sorted             |     341.7 ns |    273.19 ns |   121.30 ns |  0.94 |    0.31 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | Sorted             |     193.3 ns |      2.02 ns |     0.72 ns |  0.53 |    0.00 |    1 |         - |          NA |
| GnomeSort              | 256  | Sorted             |     189.0 ns |      1.36 ns |     0.71 ns |  0.52 |    0.00 |    1 |         - |          NA |
| LibrarySort            | 256  | Sorted             |  15,727.8 ns |     99.20 ns |    51.88 ns | 43.04 |    0.19 |    6 |         - |          NA |
| MergeInsertionSort     | 256  | Sorted             |  12,227.5 ns |    253.22 ns |   112.43 ns | 33.46 |    0.31 |    5 |         - |          NA |
| ShellSortKnuth1973     | 256  | Sorted             |   1,192.1 ns |      1.90 ns |     0.84 ns |  3.26 |    0.01 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Sorted             |   1,194.8 ns |      5.02 ns |     2.23 ns |  3.27 |    0.01 |    4 |         - |          NA |
| ShellSortTokuda1992    | 256  | Sorted             |   1,473.5 ns |     39.51 ns |    17.54 ns |  4.03 |    0.05 |    4 |         - |          NA |
| ShellSortCiura2001     | 256  | Sorted             |   1,280.1 ns |      3.76 ns |     1.34 ns |  3.50 |    0.01 |    4 |         - |          NA |
| ShellSortLee2021       | 256  | Sorted             |   1,483.1 ns |     29.57 ns |    15.46 ns |  4.06 |    0.04 |    4 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **256**  | **Reversed**           |  **15,483.7 ns** |    **126.27 ns** |    **66.04 ns** |  **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Reversed           |  20,351.7 ns |    234.70 ns |   122.75 ns |  1.31 |    0.01 |    5 |         - |          NA |
| BinaryInsertSort       | 256  | Reversed           |   6,576.6 ns |     61.31 ns |    21.87 ns |  0.42 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 256  | Reversed           |  69,523.2 ns |  1,671.78 ns |   874.37 ns |  4.49 |    0.06 |    6 |         - |          NA |
| LibrarySort            | 256  | Reversed           |  21,969.4 ns |    262.32 ns |   116.47 ns |  1.42 |    0.01 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | Reversed           |  12,254.1 ns |    204.92 ns |   107.18 ns |  0.79 |    0.01 |    3 |         - |          NA |
| ShellSortKnuth1973     | 256  | Reversed           |   1,906.0 ns |     20.20 ns |     7.21 ns |  0.12 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Reversed           |   1,954.8 ns |    219.50 ns |    97.46 ns |  0.13 |    0.01 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | Reversed           |   2,133.2 ns |    120.37 ns |    53.45 ns |  0.14 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | Reversed           |   2,118.4 ns |     35.52 ns |    15.77 ns |  0.14 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Reversed           |   2,040.5 ns |      9.22 ns |     3.29 ns |  0.13 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **256**  | **PipeOrgan**          |   **7,954.2 ns** |     **11.65 ns** |     **5.17 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 256  | PipeOrgan          |  10,436.8 ns |    307.37 ns |   160.76 ns |  1.31 |    0.02 |    4 |         - |          NA |
| BinaryInsertSort       | 256  | PipeOrgan          |   3,908.6 ns |    265.67 ns |   138.95 ns |  0.49 |    0.02 |    2 |         - |          NA |
| GnomeSort              | 256  | PipeOrgan          |  33,537.6 ns |  1,696.91 ns |   887.52 ns |  4.22 |    0.11 |    6 |         - |          NA |
| LibrarySort            | 256  | PipeOrgan          |  14,554.9 ns |    293.45 ns |   130.29 ns |  1.83 |    0.02 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | PipeOrgan          |  12,579.3 ns |    212.81 ns |   111.30 ns |  1.58 |    0.01 |    5 |         - |          NA |
| ShellSortKnuth1973     | 256  | PipeOrgan          |   1,845.4 ns |     89.63 ns |    39.80 ns |  0.23 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | PipeOrgan          |   1,952.0 ns |    233.99 ns |   103.90 ns |  0.25 |    0.01 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | PipeOrgan          |   2,136.9 ns |     17.98 ns |     6.41 ns |  0.27 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | PipeOrgan          |   2,007.0 ns |     17.19 ns |     7.63 ns |  0.25 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | PipeOrgan          |   2,125.2 ns |     19.24 ns |     6.86 ns |  0.27 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **256**  | **ManyDuplicates**     |   **6,820.8 ns** |    **231.62 ns** |   **121.14 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 256  | ManyDuplicates     |   7,150.6 ns |    276.50 ns |   144.61 ns |  1.05 |    0.03 |    3 |         - |          NA |
| BinaryInsertSort       | 256  | ManyDuplicates     |   5,356.7 ns |    423.71 ns |   188.13 ns |  0.79 |    0.03 |    2 |         - |          NA |
| GnomeSort              | 256  | ManyDuplicates     |  23,331.2 ns |    426.36 ns |   222.99 ns |  3.42 |    0.06 |    6 |         - |          NA |
| LibrarySort            | 256  | ManyDuplicates     |  16,161.4 ns |    330.13 ns |   146.58 ns |  2.37 |    0.04 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | ManyDuplicates     |  13,218.6 ns |    200.97 ns |    89.23 ns |  1.94 |    0.03 |    4 |         - |          NA |
| ShellSortKnuth1973     | 256  | ManyDuplicates     |   2,320.4 ns |    153.24 ns |    68.04 ns |  0.34 |    0.01 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | ManyDuplicates     |   2,366.9 ns |     62.49 ns |    27.74 ns |  0.35 |    0.01 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | ManyDuplicates     |   2,203.4 ns |     89.35 ns |    39.67 ns |  0.32 |    0.01 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | ManyDuplicates     |   2,190.7 ns |     13.64 ns |     4.86 ns |  0.32 |    0.01 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | ManyDuplicates     |   2,122.4 ns |    120.44 ns |    53.48 ns |  0.31 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **1024** | **Random**             | **116,967.7 ns** |    **577.61 ns** |   **256.46 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Random             | 134,947.8 ns |  1,312.28 ns |   582.66 ns |  1.15 |    0.01 |    5 |         - |          NA |
| BinaryInsertSort       | 1024 | Random             |  36,209.3 ns |    904.58 ns |   401.64 ns |  0.31 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | Random             | 387,334.3 ns |  4,517.48 ns | 2,005.79 ns |  3.31 |    0.02 |    6 |         - |          NA |
| LibrarySort            | 1024 | Random             |  75,471.5 ns |  1,511.19 ns |   790.38 ns |  0.65 |    0.01 |    3 |         - |          NA |
| MergeInsertionSort     | 1024 | Random             |  93,914.8 ns |  3,758.12 ns | 1,965.57 ns |  0.80 |    0.02 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Random             |  14,756.5 ns |    380.83 ns |   199.18 ns |  0.13 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Random             |  14,420.6 ns |    134.86 ns |    59.88 ns |  0.12 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Random             |  14,453.3 ns |    159.68 ns |    70.90 ns |  0.12 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Random             |  14,372.9 ns |    545.12 ns |   242.04 ns |  0.12 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Random             |  14,434.4 ns |    194.11 ns |    86.19 ns |  0.12 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **1024** | **SingleElementMoved** |   **1,848.7 ns** |      **1.16 ns** |     **0.41 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | SingleElementMoved |   1,206.5 ns |    100.91 ns |    35.98 ns |  0.65 |    0.02 |    1 |         - |          NA |
| BinaryInsertSort       | 1024 | SingleElementMoved |   5,644.0 ns |    184.26 ns |    96.37 ns |  3.05 |    0.05 |    3 |         - |          NA |
| GnomeSort              | 1024 | SingleElementMoved |   2,042.9 ns |     20.83 ns |     9.25 ns |  1.11 |    0.00 |    2 |         - |          NA |
| LibrarySort            | 1024 | SingleElementMoved |  76,934.0 ns |     64.58 ns |    28.67 ns | 41.62 |    0.02 |    4 |         - |          NA |
| MergeInsertionSort     | 1024 | SingleElementMoved |  75,553.0 ns |    938.54 ns |   416.72 ns | 40.87 |    0.21 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | SingleElementMoved |   6,756.6 ns |    272.12 ns |   142.32 ns |  3.65 |    0.07 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | SingleElementMoved |   7,596.3 ns |     87.45 ns |    45.74 ns |  4.11 |    0.02 |    3 |         - |          NA |
| ShellSortTokuda1992    | 1024 | SingleElementMoved |   7,994.3 ns |    157.17 ns |    82.20 ns |  4.32 |    0.04 |    3 |         - |          NA |
| ShellSortCiura2001     | 1024 | SingleElementMoved |   7,722.4 ns |     28.22 ns |    12.53 ns |  4.18 |    0.01 |    3 |         - |          NA |
| ShellSortLee2021       | 1024 | SingleElementMoved |   7,911.5 ns |    473.11 ns |   210.07 ns |  4.28 |    0.11 |    3 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **1024** | **Sorted**             |   **1,436.4 ns** |      **1.43 ns** |     **0.75 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Sorted             |   1,083.8 ns |      1.30 ns |     0.58 ns |  0.75 |    0.00 |    2 |         - |          NA |
| BinaryInsertSort       | 1024 | Sorted             |     728.1 ns |      1.94 ns |     0.86 ns |  0.51 |    0.00 |    1 |         - |          NA |
| GnomeSort              | 1024 | Sorted             |     725.1 ns |      1.33 ns |     0.59 ns |  0.50 |    0.00 |    1 |         - |          NA |
| LibrarySort            | 1024 | Sorted             |  79,195.1 ns |    367.41 ns |   192.16 ns | 55.13 |    0.13 |    5 |         - |          NA |
| MergeInsertionSort     | 1024 | Sorted             |  74,331.9 ns |    368.70 ns |   192.84 ns | 51.75 |    0.13 |    5 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Sorted             |   5,945.3 ns |     72.02 ns |    25.68 ns |  4.14 |    0.02 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Sorted             |   6,857.5 ns |    330.13 ns |   172.66 ns |  4.77 |    0.11 |    4 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Sorted             |   7,146.3 ns |     17.14 ns |     8.97 ns |  4.98 |    0.01 |    4 |         - |          NA |
| ShellSortCiura2001     | 1024 | Sorted             |   7,219.9 ns |    262.97 ns |   137.54 ns |  5.03 |    0.09 |    4 |         - |          NA |
| ShellSortLee2021       | 1024 | Sorted             |   7,313.5 ns |    284.63 ns |   148.87 ns |  5.09 |    0.10 |    4 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **1024** | **Reversed**           | **229,561.1 ns** |    **989.50 ns** |   **517.53 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Reversed           | 314,932.4 ns |  1,679.67 ns |   878.50 ns |  1.37 |    0.00 |    5 |         - |          NA |
| BinaryInsertSort       | 1024 | Reversed           |  45,145.9 ns |    262.62 ns |   116.60 ns |  0.20 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | Reversed           | 967,813.6 ns |  4,526.09 ns | 2,367.23 ns |  4.22 |    0.01 |    6 |         - |          NA |
| LibrarySort            | 1024 | Reversed           | 192,997.6 ns |    453.60 ns |   237.24 ns |  0.84 |    0.00 |    4 |         - |          NA |
| MergeInsertionSort     | 1024 | Reversed           |  75,693.8 ns |  1,157.05 ns |   605.16 ns |  0.33 |    0.00 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Reversed           |   9,257.2 ns |    371.41 ns |   194.25 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Reversed           |   9,567.2 ns |    179.01 ns |    93.62 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Reversed           |  10,446.9 ns |    357.87 ns |   187.17 ns |  0.05 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Reversed           |  10,006.4 ns |    461.72 ns |   241.49 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Reversed           |  10,492.5 ns |    441.54 ns |   230.93 ns |  0.05 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **1024** | **PipeOrgan**          | **116,559.2 ns** |  **1,299.63 ns** |   **577.04 ns** |  **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | PipeOrgan          | 159,154.6 ns |  1,708.16 ns |   609.15 ns |  1.37 |    0.01 |    5 |         - |          NA |
| BinaryInsertSort       | 1024 | PipeOrgan          |  25,005.8 ns |    493.11 ns |   257.91 ns |  0.21 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | PipeOrgan          | 493,769.1 ns | 16,928.30 ns | 8,853.83 ns |  4.24 |    0.07 |    6 |         - |          NA |
| LibrarySort            | 1024 | PipeOrgan          |  71,822.5 ns |  1,022.08 ns |   534.57 ns |  0.62 |    0.01 |    3 |         - |          NA |
| MergeInsertionSort     | 1024 | PipeOrgan          |  76,418.8 ns |    815.61 ns |   362.13 ns |  0.66 |    0.00 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | PipeOrgan          |   9,315.2 ns |    217.24 ns |   113.62 ns |  0.08 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | PipeOrgan          |   9,660.7 ns |    299.90 ns |   156.86 ns |  0.08 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | PipeOrgan          |  10,876.8 ns |    362.72 ns |   161.05 ns |  0.09 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | PipeOrgan          |  10,469.7 ns |    259.27 ns |   135.60 ns |  0.09 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | PipeOrgan          |  10,793.2 ns |    280.86 ns |   146.89 ns |  0.09 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **InsertionSort**          | **1024** | **ManyDuplicates**     | **113,898.5 ns** |    **515.03 ns** |   **183.67 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | ManyDuplicates     | 130,852.9 ns |    940.18 ns |   417.45 ns |  1.15 |    0.00 |    5 |         - |          NA |
| BinaryInsertSort       | 1024 | ManyDuplicates     |  35,888.5 ns |  1,401.24 ns |   732.87 ns |  0.32 |    0.01 |    2 |         - |          NA |
| GnomeSort              | 1024 | ManyDuplicates     | 375,049.5 ns |  1,366.15 ns |   606.58 ns |  3.29 |    0.01 |    6 |         - |          NA |
| LibrarySort            | 1024 | ManyDuplicates     |  72,808.3 ns |    626.79 ns |   278.30 ns |  0.64 |    0.00 |    3 |         - |          NA |
| MergeInsertionSort     | 1024 | ManyDuplicates     |  92,996.6 ns |  1,087.12 ns |   482.69 ns |  0.82 |    0.00 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | ManyDuplicates     |  11,424.8 ns |    238.76 ns |   106.01 ns |  0.10 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | ManyDuplicates     |  10,813.9 ns |    397.61 ns |   207.96 ns |  0.09 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | ManyDuplicates     |  10,917.0 ns |    354.51 ns |   157.41 ns |  0.10 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | ManyDuplicates     |  10,941.2 ns |    597.75 ns |   265.40 ns |  0.10 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | ManyDuplicates     |  11,280.5 ns |    720.04 ns |   319.70 ns |  0.10 |    0.00 |    1 |         - |          NA |

### MergeBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.87GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                   | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **MergeSort**                | **256**  | **Random**             |     **8,614.6 ns** |     **88.01 ns** |     **39.08 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Random             |     8,491.1 ns |     22.49 ns |      9.99 ns |  0.99 |    0.00 |    3 |         - |          NA |
| BottomupMergeSort        | 256  | Random             |     4,830.8 ns |     90.65 ns |     32.33 ns |  0.56 |    0.00 |    2 |         - |          NA |
| StdStableSort            | 256  | Random             |     2,762.7 ns |    220.30 ns |    115.22 ns |  0.32 |    0.01 |    1 |         - |          NA |
| RotateMergeSort          | 256  | Random             |    10,520.3 ns |    371.33 ns |    194.21 ns |  1.22 |    0.02 |    4 |         - |          NA |
| RotateMergeSortRecursive | 256  | Random             |    12,318.6 ns |    398.80 ns |    208.58 ns |  1.43 |    0.02 |    4 |         - |          NA |
| SymMergeSort             | 256  | Random             |     7,153.1 ns |    163.86 ns |     72.75 ns |  0.83 |    0.01 |    3 |         - |          NA |
| BlockMergeSort           | 256  | Random             |     5,187.3 ns |    313.88 ns |    164.17 ns |  0.60 |    0.02 |    2 |         - |          NA |
| NaturalMergeSort         | 256  | Random             |     5,179.9 ns |    175.97 ns |     78.13 ns |  0.60 |    0.01 |    2 |         - |          NA |
| TimSort                  | 256  | Random             |     4,143.7 ns |    322.15 ns |    168.49 ns |  0.48 |    0.02 |    2 |         - |          NA |
| PowerSort                | 256  | Random             |     2,371.0 ns |    179.46 ns |     79.68 ns |  0.28 |    0.01 |    1 |         - |          NA |
| ShiftSort                | 256  | Random             |     4,508.3 ns |    367.83 ns |    192.38 ns |  0.52 |    0.02 |    2 |         - |          NA |
| SpinSort                 | 256  | Random             |     2,607.6 ns |    395.96 ns |    207.09 ns |  0.30 |    0.02 |    1 |         - |          NA |
| Glidesort                | 256  | Random             |     3,704.1 ns |    387.91 ns |    202.88 ns |  0.43 |    0.02 |    2 |         - |          NA |
| Driftsort                | 256  | Random             |     4,445.4 ns |     32.25 ns |     11.50 ns |  0.52 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 256  | Random             |     2,791.5 ns |    249.99 ns |    111.00 ns |  0.32 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **SingleElementMoved** |     **4,838.6 ns** |    **525.23 ns** |    **274.70 ns** |  **1.00** |    **0.07** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | SingleElementMoved |     5,581.7 ns |    235.09 ns |    122.96 ns |  1.16 |    0.06 |    7 |         - |          NA |
| BottomupMergeSort        | 256  | SingleElementMoved |     1,743.9 ns |     10.88 ns |      3.88 ns |  0.36 |    0.02 |    5 |         - |          NA |
| StdStableSort            | 256  | SingleElementMoved |       747.4 ns |      4.25 ns |      1.52 ns |  0.15 |    0.01 |    2 |         - |          NA |
| RotateMergeSort          | 256  | SingleElementMoved |       614.8 ns |      9.92 ns |      4.41 ns |  0.13 |    0.01 |    2 |         - |          NA |
| RotateMergeSortRecursive | 256  | SingleElementMoved |       641.8 ns |      6.64 ns |      2.95 ns |  0.13 |    0.01 |    2 |         - |          NA |
| SymMergeSort             | 256  | SingleElementMoved |       542.9 ns |     19.45 ns |      8.64 ns |  0.11 |    0.01 |    2 |         - |          NA |
| BlockMergeSort           | 256  | SingleElementMoved |     3,080.3 ns |    102.55 ns |     36.57 ns |  0.64 |    0.03 |    6 |         - |          NA |
| NaturalMergeSort         | 256  | SingleElementMoved |       615.6 ns |      4.32 ns |      1.92 ns |  0.13 |    0.01 |    2 |         - |          NA |
| TimSort                  | 256  | SingleElementMoved |       274.4 ns |      3.87 ns |      1.72 ns |  0.06 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | SingleElementMoved |       414.0 ns |      2.76 ns |      1.23 ns |  0.09 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 256  | SingleElementMoved |       457.8 ns |     24.57 ns |     10.91 ns |  0.09 |    0.01 |    2 |         - |          NA |
| SpinSort                 | 256  | SingleElementMoved |       949.7 ns |      5.09 ns |      2.26 ns |  0.20 |    0.01 |    3 |         - |          NA |
| Glidesort                | 256  | SingleElementMoved |     1,243.0 ns |     12.13 ns |      5.39 ns |  0.26 |    0.01 |    4 |         - |          NA |
| Driftsort                | 256  | SingleElementMoved |     1,333.6 ns |    302.19 ns |    158.05 ns |  0.28 |    0.03 |    4 |         - |          NA |
| FlatStableSort           | 256  | SingleElementMoved |     1,289.0 ns |     15.50 ns |      5.53 ns |  0.27 |    0.01 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **Sorted**             |     **4,430.0 ns** |    **296.57 ns** |    **155.11 ns** |  **1.00** |    **0.05** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Sorted             |     5,418.2 ns |    229.12 ns |    119.83 ns |  1.22 |    0.05 |    9 |         - |          NA |
| BottomupMergeSort        | 256  | Sorted             |     1,431.7 ns |     13.21 ns |      5.87 ns |  0.32 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 256  | Sorted             |       658.7 ns |      3.59 ns |      1.60 ns |  0.15 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 256  | Sorted             |       443.2 ns |    216.33 ns |    113.15 ns |  0.10 |    0.02 |    4 |         - |          NA |
| RotateMergeSortRecursive | 256  | Sorted             |       502.4 ns |    117.34 ns |     61.37 ns |  0.11 |    0.01 |    4 |         - |          NA |
| SymMergeSort             | 256  | Sorted             |       299.8 ns |      2.50 ns |      0.89 ns |  0.07 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 256  | Sorted             |     2,622.2 ns |     35.40 ns |     15.72 ns |  0.59 |    0.02 |    7 |         - |          NA |
| NaturalMergeSort         | 256  | Sorted             |       414.4 ns |     17.02 ns |      8.90 ns |  0.09 |    0.00 |    4 |         - |          NA |
| TimSort                  | 256  | Sorted             |       228.7 ns |     69.33 ns |     36.26 ns |  0.05 |    0.01 |    2 |         - |          NA |
| PowerSort                | 256  | Sorted             |       191.3 ns |     89.98 ns |     39.95 ns |  0.04 |    0.01 |    1 |         - |          NA |
| ShiftSort                | 256  | Sorted             |       514.2 ns |    256.86 ns |    134.34 ns |  0.12 |    0.03 |    4 |         - |          NA |
| SpinSort                 | 256  | Sorted             |       150.3 ns |      1.06 ns |      0.56 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 256  | Sorted             |       292.8 ns |     64.36 ns |     33.66 ns |  0.07 |    0.01 |    3 |         - |          NA |
| Driftsort                | 256  | Sorted             |       214.7 ns |      7.45 ns |      3.31 ns |  0.05 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 256  | Sorted             |     1,228.1 ns |     11.39 ns |      5.06 ns |  0.28 |    0.01 |    6 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **Reversed**           |     **9,055.2 ns** |    **359.06 ns** |    **187.80 ns** |  **1.00** |    **0.03** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Reversed           |     8,511.9 ns |    208.71 ns |     92.67 ns |  0.94 |    0.02 |    7 |         - |          NA |
| BottomupMergeSort        | 256  | Reversed           |     4,495.8 ns |     15.79 ns |      5.63 ns |  0.50 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 256  | Reversed           |     6,321.9 ns |     44.15 ns |     15.75 ns |  0.70 |    0.01 |    6 |         - |          NA |
| RotateMergeSort          | 256  | Reversed           |     2,034.9 ns |    197.08 ns |    103.07 ns |  0.22 |    0.01 |    3 |         - |          NA |
| RotateMergeSortRecursive | 256  | Reversed           |     2,255.3 ns |      6.69 ns |      2.97 ns |  0.25 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 256  | Reversed           |     2,276.7 ns |    110.48 ns |     49.05 ns |  0.25 |    0.01 |    3 |         - |          NA |
| BlockMergeSort           | 256  | Reversed           |     3,275.0 ns |    295.23 ns |    154.41 ns |  0.36 |    0.02 |    4 |         - |          NA |
| NaturalMergeSort         | 256  | Reversed           |       338.1 ns |      4.48 ns |      1.99 ns |  0.04 |    0.00 |    2 |         - |          NA |
| TimSort                  | 256  | Reversed           |       336.2 ns |     97.31 ns |     50.89 ns |  0.04 |    0.01 |    2 |         - |          NA |
| PowerSort                | 256  | Reversed           |       231.5 ns |      1.35 ns |      0.70 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Reversed           |     4,349.6 ns |     36.43 ns |     12.99 ns |  0.48 |    0.01 |    5 |         - |          NA |
| SpinSort                 | 256  | Reversed           |       247.2 ns |      2.37 ns |      1.05 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 256  | Reversed           |       280.5 ns |      5.68 ns |      2.03 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 256  | Reversed           |       292.0 ns |      5.75 ns |      2.55 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | Reversed           |     3,056.0 ns |    486.31 ns |    215.92 ns |  0.34 |    0.02 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **PipeOrgan**          |     **6,861.5 ns** |    **339.31 ns** |    **177.47 ns** |  **1.00** |    **0.03** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | PipeOrgan          |     6,779.2 ns |      7.29 ns |      2.60 ns |  0.99 |    0.02 |    8 |         - |          NA |
| BottomupMergeSort        | 256  | PipeOrgan          |     3,530.7 ns |    432.76 ns |    226.34 ns |  0.51 |    0.03 |    6 |         - |          NA |
| StdStableSort            | 256  | PipeOrgan          |     3,678.2 ns |    259.28 ns |    115.12 ns |  0.54 |    0.02 |    6 |         - |          NA |
| RotateMergeSort          | 256  | PipeOrgan          |     4,305.1 ns |    316.04 ns |    165.29 ns |  0.63 |    0.03 |    6 |         - |          NA |
| RotateMergeSortRecursive | 256  | PipeOrgan          |     5,318.5 ns |    313.64 ns |    164.04 ns |  0.78 |    0.03 |    7 |         - |          NA |
| SymMergeSort             | 256  | PipeOrgan          |     3,498.9 ns |    494.47 ns |    219.55 ns |  0.51 |    0.03 |    6 |         - |          NA |
| BlockMergeSort           | 256  | PipeOrgan          |     3,366.2 ns |    326.57 ns |    170.80 ns |  0.49 |    0.03 |    6 |         - |          NA |
| NaturalMergeSort         | 256  | PipeOrgan          |       766.8 ns |     97.91 ns |     43.47 ns |  0.11 |    0.01 |    2 |         - |          NA |
| TimSort                  | 256  | PipeOrgan          |       918.5 ns |    110.54 ns |     49.08 ns |  0.13 |    0.01 |    3 |         - |          NA |
| PowerSort                | 256  | PipeOrgan          |       517.5 ns |      5.13 ns |      2.28 ns |  0.08 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | PipeOrgan          |     2,668.7 ns |     34.28 ns |     15.22 ns |  0.39 |    0.01 |    5 |         - |          NA |
| SpinSort                 | 256  | PipeOrgan          |     2,094.3 ns |    210.35 ns |     75.01 ns |  0.31 |    0.01 |    5 |         - |          NA |
| Glidesort                | 256  | PipeOrgan          |     1,263.6 ns |     12.01 ns |      5.33 ns |  0.18 |    0.00 |    4 |         - |          NA |
| Driftsort                | 256  | PipeOrgan          |       501.9 ns |    133.74 ns |     69.95 ns |  0.07 |    0.01 |    1 |         - |          NA |
| FlatStableSort           | 256  | PipeOrgan          |     2,344.5 ns |    415.24 ns |    217.18 ns |  0.34 |    0.03 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **ManyDuplicates**     |     **8,573.8 ns** |    **205.24 ns** |    **107.34 ns** |  **1.00** |    **0.02** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | ManyDuplicates     |     8,182.7 ns |    274.77 ns |    143.71 ns |  0.95 |    0.02 |    4 |         - |          NA |
| BottomupMergeSort        | 256  | ManyDuplicates     |     4,804.2 ns |    358.57 ns |    187.54 ns |  0.56 |    0.02 |    2 |         - |          NA |
| StdStableSort            | 256  | ManyDuplicates     |     2,611.6 ns |     37.26 ns |     16.54 ns |  0.30 |    0.00 |    1 |         - |          NA |
| RotateMergeSort          | 256  | ManyDuplicates     |     9,785.7 ns |    297.27 ns |    131.99 ns |  1.14 |    0.02 |    4 |         - |          NA |
| RotateMergeSortRecursive | 256  | ManyDuplicates     |    11,311.1 ns |    536.41 ns |    280.55 ns |  1.32 |    0.03 |    4 |         - |          NA |
| SymMergeSort             | 256  | ManyDuplicates     |     6,526.7 ns |    443.17 ns |    196.77 ns |  0.76 |    0.02 |    3 |         - |          NA |
| BlockMergeSort           | 256  | ManyDuplicates     |     4,991.2 ns |     39.90 ns |     14.23 ns |  0.58 |    0.01 |    2 |         - |          NA |
| NaturalMergeSort         | 256  | ManyDuplicates     |     5,115.2 ns |    467.91 ns |    244.73 ns |  0.60 |    0.03 |    2 |         - |          NA |
| TimSort                  | 256  | ManyDuplicates     |     3,904.9 ns |    268.59 ns |    119.26 ns |  0.46 |    0.01 |    2 |         - |          NA |
| PowerSort                | 256  | ManyDuplicates     |     2,253.7 ns |     44.43 ns |     15.84 ns |  0.26 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | ManyDuplicates     |     4,485.4 ns |    239.62 ns |    125.33 ns |  0.52 |    0.02 |    2 |         - |          NA |
| SpinSort                 | 256  | ManyDuplicates     |     2,329.3 ns |    129.28 ns |     46.10 ns |  0.27 |    0.01 |    1 |         - |          NA |
| Glidesort                | 256  | ManyDuplicates     |     3,516.3 ns |     26.25 ns |      9.36 ns |  0.41 |    0.00 |    2 |         - |          NA |
| Driftsort                | 256  | ManyDuplicates     |     4,554.8 ns |    546.13 ns |    242.48 ns |  0.53 |    0.03 |    2 |         - |          NA |
| FlatStableSort           | 256  | ManyDuplicates     |     2,619.3 ns |    569.65 ns |    297.94 ns |  0.31 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **Random**             |    **38,214.8 ns** |    **952.01 ns** |    **422.70 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Random             |    37,511.7 ns |    256.33 ns |     91.41 ns |  0.98 |    0.01 |    3 |         - |          NA |
| BottomupMergeSort        | 1024 | Random             |    21,032.6 ns |    642.14 ns |    285.11 ns |  0.55 |    0.01 |    2 |         - |          NA |
| StdStableSort            | 1024 | Random             |    13,886.8 ns |    631.87 ns |    280.55 ns |  0.36 |    0.01 |    1 |         - |          NA |
| RotateMergeSort          | 1024 | Random             |    62,847.6 ns |  2,487.72 ns |  1,104.56 ns |  1.64 |    0.03 |    4 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Random             |    73,195.8 ns |  1,080.54 ns |    565.14 ns |  1.92 |    0.02 |    4 |         - |          NA |
| SymMergeSort             | 1024 | Random             |    42,049.2 ns |    694.65 ns |    363.31 ns |  1.10 |    0.01 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | Random             |    25,927.3 ns |    373.41 ns |    195.30 ns |  0.68 |    0.01 |    2 |         - |          NA |
| NaturalMergeSort         | 1024 | Random             |    24,623.9 ns |    631.60 ns |    330.34 ns |  0.64 |    0.01 |    2 |         - |          NA |
| TimSort                  | 1024 | Random             |    19,579.3 ns |    441.74 ns |    231.04 ns |  0.51 |    0.01 |    2 |         - |          NA |
| PowerSort                | 1024 | Random             |    12,639.2 ns |    296.17 ns |    131.50 ns |  0.33 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Random             |    21,362.9 ns |    287.22 ns |    127.53 ns |  0.56 |    0.01 |    2 |         - |          NA |
| SpinSort                 | 1024 | Random             |    12,095.8 ns |    837.84 ns |    438.21 ns |  0.32 |    0.01 |    1 |         - |          NA |
| Glidesort                | 1024 | Random             |    16,866.5 ns |    424.30 ns |    221.92 ns |  0.44 |    0.01 |    2 |         - |          NA |
| Driftsort                | 1024 | Random             |    21,365.9 ns |    354.14 ns |    185.22 ns |  0.56 |    0.01 |    2 |         - |          NA |
| FlatStableSort           | 1024 | Random             |    14,017.6 ns |    264.35 ns |    138.26 ns |  0.37 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **SingleElementMoved** |    **18,719.7 ns** |    **335.02 ns** |    **148.75 ns** |  **1.00** |    **0.01** |   **10** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | SingleElementMoved |    22,411.3 ns |    221.30 ns |    115.75 ns |  1.20 |    0.01 |   10 |         - |          NA |
| BottomupMergeSort        | 1024 | SingleElementMoved |     7,052.5 ns |    640.86 ns |    335.18 ns |  0.38 |    0.02 |    8 |         - |          NA |
| StdStableSort            | 1024 | SingleElementMoved |     3,744.3 ns |     16.11 ns |      5.75 ns |  0.20 |    0.00 |    6 |         - |          NA |
| RotateMergeSort          | 1024 | SingleElementMoved |     2,032.8 ns |     28.56 ns |     12.68 ns |  0.11 |    0.00 |    4 |         - |          NA |
| RotateMergeSortRecursive | 1024 | SingleElementMoved |     2,159.0 ns |     19.22 ns |      8.53 ns |  0.12 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 1024 | SingleElementMoved |     2,171.0 ns |  1,358.21 ns |    603.05 ns |  0.12 |    0.03 |    4 |         - |          NA |
| BlockMergeSort           | 1024 | SingleElementMoved |    14,315.6 ns |    195.83 ns |    102.42 ns |  0.76 |    0.01 |    9 |         - |          NA |
| NaturalMergeSort         | 1024 | SingleElementMoved |     2,224.3 ns |     15.94 ns |      7.08 ns |  0.12 |    0.00 |    4 |         - |          NA |
| TimSort                  | 1024 | SingleElementMoved |       901.9 ns |    148.57 ns |     77.70 ns |  0.05 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | SingleElementMoved |     1,664.7 ns |    141.56 ns |     62.85 ns |  0.09 |    0.00 |    3 |         - |          NA |
| ShiftSort                | 1024 | SingleElementMoved |     1,614.5 ns |     16.10 ns |      5.74 ns |  0.09 |    0.00 |    3 |         - |          NA |
| SpinSort                 | 1024 | SingleElementMoved |     4,680.2 ns |    366.15 ns |    191.51 ns |  0.25 |    0.01 |    7 |         - |          NA |
| Glidesort                | 1024 | SingleElementMoved |     3,148.6 ns |    319.18 ns |    166.94 ns |  0.17 |    0.01 |    5 |         - |          NA |
| Driftsort                | 1024 | SingleElementMoved |     1,335.4 ns |      8.72 ns |      3.87 ns |  0.07 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 1024 | SingleElementMoved |     5,998.3 ns |    382.81 ns |    200.22 ns |  0.32 |    0.01 |    8 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **Sorted**             |    **17,241.2 ns** |    **115.93 ns** |     **60.63 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Sorted             |    21,178.7 ns |    341.47 ns |    178.60 ns |  1.23 |    0.01 |    9 |         - |          NA |
| BottomupMergeSort        | 1024 | Sorted             |     5,862.9 ns |    286.61 ns |    149.90 ns |  0.34 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 1024 | Sorted             |     3,532.6 ns |     50.72 ns |     22.52 ns |  0.20 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 1024 | Sorted             |     1,165.6 ns |      5.56 ns |      2.47 ns |  0.07 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Sorted             |     1,763.8 ns |      3.55 ns |      1.27 ns |  0.10 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 1024 | Sorted             |     1,153.5 ns |      4.01 ns |      1.78 ns |  0.07 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | Sorted             |    11,688.8 ns |    218.56 ns |    114.31 ns |  0.68 |    0.01 |    7 |         - |          NA |
| NaturalMergeSort         | 1024 | Sorted             |       794.7 ns |      3.66 ns |      1.92 ns |  0.05 |    0.00 |    2 |         - |          NA |
| TimSort                  | 1024 | Sorted             |       596.9 ns |      9.22 ns |      4.10 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Sorted             |       608.1 ns |     91.30 ns |     40.54 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Sorted             |     1,508.2 ns |     14.14 ns |      6.28 ns |  0.09 |    0.00 |    4 |         - |          NA |
| SpinSort                 | 1024 | Sorted             |       544.1 ns |     53.88 ns |     28.18 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Sorted             |       570.3 ns |     36.79 ns |     16.33 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | Sorted             |       650.2 ns |     14.59 ns |      6.48 ns |  0.04 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Sorted             |     5,241.9 ns |    336.39 ns |    175.94 ns |  0.30 |    0.01 |    6 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **Reversed**           |    **37,635.0 ns** |    **661.92 ns** |    **346.20 ns** |  **1.00** |    **0.01** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Reversed           |    34,453.0 ns |    696.03 ns |    364.04 ns |  0.92 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 1024 | Reversed           |    21,350.8 ns |    155.76 ns |     55.55 ns |  0.57 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 1024 | Reversed           |    26,918.8 ns |    346.90 ns |    181.43 ns |  0.72 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 1024 | Reversed           |     9,271.0 ns |    254.82 ns |    113.14 ns |  0.25 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Reversed           |    10,868.2 ns |    272.45 ns |    142.50 ns |  0.29 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 1024 | Reversed           |     9,859.0 ns |    380.45 ns |    198.98 ns |  0.26 |    0.01 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | Reversed           |    16,093.7 ns |    260.31 ns |    115.58 ns |  0.43 |    0.00 |    4 |         - |          NA |
| NaturalMergeSort         | 1024 | Reversed           |     1,163.5 ns |      4.37 ns |      1.94 ns |  0.03 |    0.00 |    2 |         - |          NA |
| TimSort                  | 1024 | Reversed           |       964.2 ns |    135.31 ns |     70.77 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Reversed           |       906.9 ns |      5.31 ns |      2.36 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Reversed           |    18,742.8 ns |    275.52 ns |    144.10 ns |  0.50 |    0.01 |    4 |         - |          NA |
| SpinSort                 | 1024 | Reversed           |       964.1 ns |      3.16 ns |      1.66 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Reversed           |       965.8 ns |      6.22 ns |      2.76 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | Reversed           |       966.9 ns |      6.23 ns |      3.26 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Reversed           |    12,052.7 ns |    206.97 ns |    108.25 ns |  0.32 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **PipeOrgan**          |    **28,396.6 ns** |    **495.41 ns** |    **259.11 ns** |  **1.00** |    **0.01** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | PipeOrgan          |    28,566.7 ns |    863.21 ns |    451.47 ns |  1.01 |    0.02 |    8 |         - |          NA |
| BottomupMergeSort        | 1024 | PipeOrgan          |    14,291.1 ns |    438.01 ns |    229.09 ns |  0.50 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 1024 | PipeOrgan          |    15,519.9 ns |     78.23 ns |     40.92 ns |  0.55 |    0.00 |    6 |         - |          NA |
| RotateMergeSort          | 1024 | PipeOrgan          |    18,704.4 ns |    168.97 ns |     88.37 ns |  0.66 |    0.01 |    6 |         - |          NA |
| RotateMergeSortRecursive | 1024 | PipeOrgan          |    22,839.9 ns |    224.55 ns |     99.70 ns |  0.80 |    0.01 |    7 |         - |          NA |
| SymMergeSort             | 1024 | PipeOrgan          |    14,883.4 ns |    185.52 ns |     66.16 ns |  0.52 |    0.01 |    6 |         - |          NA |
| BlockMergeSort           | 1024 | PipeOrgan          |    16,296.3 ns |    152.70 ns |     79.86 ns |  0.57 |    0.01 |    6 |         - |          NA |
| NaturalMergeSort         | 1024 | PipeOrgan          |     2,732.4 ns |    236.89 ns |    105.18 ns |  0.10 |    0.00 |    2 |         - |          NA |
| TimSort                  | 1024 | PipeOrgan          |     2,972.7 ns |    316.12 ns |    140.36 ns |  0.10 |    0.00 |    2 |         - |          NA |
| PowerSort                | 1024 | PipeOrgan          |     1,796.4 ns |      7.73 ns |      2.76 ns |  0.06 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | PipeOrgan          |    11,777.3 ns |    149.95 ns |     78.43 ns |  0.41 |    0.00 |    5 |         - |          NA |
| SpinSort                 | 1024 | PipeOrgan          |     9,392.7 ns |    939.11 ns |    491.17 ns |  0.33 |    0.02 |    4 |         - |          NA |
| Glidesort                | 1024 | PipeOrgan          |     4,761.4 ns |     11.16 ns |      3.98 ns |  0.17 |    0.00 |    3 |         - |          NA |
| Driftsort                | 1024 | PipeOrgan          |     1,553.8 ns |      9.01 ns |      4.00 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | PipeOrgan          |     9,414.2 ns |    325.20 ns |    170.09 ns |  0.33 |    0.01 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **ManyDuplicates**     |    **35,776.0 ns** |  **1,202.03 ns** |    **628.69 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | ManyDuplicates     |    36,153.9 ns |  2,050.83 ns |  1,072.62 ns |  1.01 |    0.03 |    3 |         - |          NA |
| BottomupMergeSort        | 1024 | ManyDuplicates     |    21,052.6 ns |    559.02 ns |    292.38 ns |  0.59 |    0.01 |    2 |         - |          NA |
| StdStableSort            | 1024 | ManyDuplicates     |    14,623.4 ns |  2,718.66 ns |  1,421.91 ns |  0.41 |    0.04 |    2 |         - |          NA |
| RotateMergeSort          | 1024 | ManyDuplicates     |    50,527.4 ns |  1,064.48 ns |    472.64 ns |  1.41 |    0.03 |    4 |         - |          NA |
| RotateMergeSortRecursive | 1024 | ManyDuplicates     |    55,472.4 ns |  1,108.74 ns |    579.89 ns |  1.55 |    0.03 |    4 |         - |          NA |
| SymMergeSort             | 1024 | ManyDuplicates     |    36,775.6 ns |  1,195.39 ns |    625.21 ns |  1.03 |    0.02 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | ManyDuplicates     |    26,519.8 ns |    310.65 ns |    137.93 ns |  0.74 |    0.01 |    2 |         - |          NA |
| NaturalMergeSort         | 1024 | ManyDuplicates     |    23,493.9 ns |    805.09 ns |    421.08 ns |  0.66 |    0.02 |    2 |         - |          NA |
| TimSort                  | 1024 | ManyDuplicates     |    18,921.8 ns |    412.10 ns |    215.54 ns |  0.53 |    0.01 |    2 |         - |          NA |
| PowerSort                | 1024 | ManyDuplicates     |    11,791.0 ns |    763.24 ns |    338.88 ns |  0.33 |    0.01 |    1 |         - |          NA |
| ShiftSort                | 1024 | ManyDuplicates     |    20,752.4 ns |    544.50 ns |    194.17 ns |  0.58 |    0.01 |    2 |         - |          NA |
| SpinSort                 | 1024 | ManyDuplicates     |    11,262.6 ns |    748.94 ns |    391.71 ns |  0.31 |    0.01 |    1 |         - |          NA |
| Glidesort                | 1024 | ManyDuplicates     |    16,355.0 ns |    456.09 ns |    238.54 ns |  0.46 |    0.01 |    2 |         - |          NA |
| Driftsort                | 1024 | ManyDuplicates     |    17,334.2 ns |     34.88 ns |     12.44 ns |  0.48 |    0.01 |    2 |         - |          NA |
| FlatStableSort           | 1024 | ManyDuplicates     |    11,929.2 ns |    680.83 ns |    302.29 ns |  0.33 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **Random**             |   **190,131.7 ns** | **19,197.87 ns** | **10,040.86 ns** |  **1.00** |    **0.07** |    **2** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Random             |   165,741.6 ns |    947.42 ns |    420.66 ns |  0.87 |    0.04 |    2 |         - |          NA |
| BottomupMergeSort        | 4096 | Random             |    99,544.5 ns |  1,964.79 ns |    872.38 ns |  0.52 |    0.03 |    1 |         - |          NA |
| StdStableSort            | 4096 | Random             |    78,466.0 ns |  5,729.48 ns |  2,996.63 ns |  0.41 |    0.03 |    1 |         - |          NA |
| RotateMergeSort          | 4096 | Random             |   634,845.8 ns |  5,944.28 ns |  2,639.30 ns |  3.35 |    0.17 |    4 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Random             |   689,345.0 ns |  5,253.43 ns |  2,332.55 ns |  3.63 |    0.18 |    4 |         - |          NA |
| SymMergeSort             | 4096 | Random             |   430,089.3 ns |  6,192.03 ns |  2,749.30 ns |  2.27 |    0.11 |    3 |         - |          NA |
| BlockMergeSort           | 4096 | Random             |   141,402.3 ns | 11,023.84 ns |  5,765.68 ns |  0.75 |    0.05 |    2 |         - |          NA |
| NaturalMergeSort         | 4096 | Random             |   139,151.2 ns |  6,867.89 ns |  3,049.39 ns |  0.73 |    0.04 |    2 |         - |          NA |
| TimSort                  | 4096 | Random             |    96,668.6 ns |  2,485.68 ns |  1,103.66 ns |  0.51 |    0.03 |    1 |         - |          NA |
| PowerSort                | 4096 | Random             |    64,707.1 ns |  3,315.57 ns |  1,472.13 ns |  0.34 |    0.02 |    1 |         - |          NA |
| ShiftSort                | 4096 | Random             |   115,544.7 ns | 21,819.59 ns | 11,412.07 ns |  0.61 |    0.06 |    1 |         - |          NA |
| SpinSort                 | 4096 | Random             |    62,333.0 ns |  1,263.63 ns |    561.06 ns |  0.33 |    0.02 |    1 |         - |          NA |
| Glidesort                | 4096 | Random             |    82,420.5 ns |  1,527.92 ns |    799.13 ns |  0.43 |    0.02 |    1 |         - |          NA |
| Driftsort                | 4096 | Random             |    97,929.9 ns |  1,210.39 ns |    537.42 ns |  0.52 |    0.03 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Random             |    68,273.1 ns |    889.93 ns |    395.14 ns |  0.36 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **SingleElementMoved** |    **74,876.0 ns** |    **710.78 ns** |    **371.75 ns** |  **1.00** |    **0.01** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | SingleElementMoved |    90,681.6 ns |  1,607.01 ns |    713.52 ns |  1.21 |    0.01 |    8 |         - |          NA |
| BottomupMergeSort        | 4096 | SingleElementMoved |    29,224.8 ns |  1,582.64 ns |    827.75 ns |  0.39 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 4096 | SingleElementMoved |    18,508.1 ns |    214.70 ns |     95.33 ns |  0.25 |    0.00 |    4 |         - |          NA |
| RotateMergeSort          | 4096 | SingleElementMoved |     7,774.3 ns |    343.74 ns |    179.78 ns |  0.10 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 4096 | SingleElementMoved |     8,075.7 ns |    365.19 ns |    191.00 ns |  0.11 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 4096 | SingleElementMoved |     7,251.7 ns |    263.92 ns |    117.18 ns |  0.10 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 4096 | SingleElementMoved |    57,818.1 ns |    841.28 ns |    373.53 ns |  0.77 |    0.01 |    6 |         - |          NA |
| NaturalMergeSort         | 4096 | SingleElementMoved |     7,502.3 ns |    241.59 ns |    107.27 ns |  0.10 |    0.00 |    2 |         - |          NA |
| TimSort                  | 4096 | SingleElementMoved |     3,192.2 ns |    313.00 ns |    163.71 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | SingleElementMoved |     5,907.2 ns |    396.62 ns |    176.10 ns |  0.08 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 4096 | SingleElementMoved |     6,270.8 ns |    251.80 ns |    111.80 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 4096 | SingleElementMoved |    14,130.1 ns |    272.66 ns |    142.61 ns |  0.19 |    0.00 |    3 |         - |          NA |
| Glidesort                | 4096 | SingleElementMoved |    11,960.1 ns |    355.17 ns |    157.70 ns |  0.16 |    0.00 |    3 |         - |          NA |
| Driftsort                | 4096 | SingleElementMoved |     5,290.6 ns |    451.25 ns |    236.01 ns |  0.07 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 4096 | SingleElementMoved |    24,786.2 ns |    773.40 ns |    404.51 ns |  0.33 |    0.01 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **Sorted**             |    **68,938.2 ns** |    **269.96 ns** |    **141.19 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Sorted             |    84,825.4 ns |    501.98 ns |    262.54 ns |  1.23 |    0.00 |    8 |         - |          NA |
| BottomupMergeSort        | 4096 | Sorted             |    23,417.0 ns |  1,259.24 ns |    559.11 ns |  0.34 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 4096 | Sorted             |    18,735.5 ns |  1,178.85 ns |    616.56 ns |  0.27 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 4096 | Sorted             |     4,654.4 ns |      6.42 ns |      2.29 ns |  0.07 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Sorted             |     7,189.6 ns |    345.64 ns |    180.78 ns |  0.10 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 4096 | Sorted             |     4,732.3 ns |    578.23 ns |    256.74 ns |  0.07 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 4096 | Sorted             |    47,320.7 ns |    366.85 ns |    191.87 ns |  0.69 |    0.00 |    6 |         - |          NA |
| NaturalMergeSort         | 4096 | Sorted             |     2,945.8 ns |      7.81 ns |      2.78 ns |  0.04 |    0.00 |    2 |         - |          NA |
| TimSort                  | 4096 | Sorted             |     2,284.6 ns |      2.23 ns |      1.17 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | Sorted             |     2,270.1 ns |      3.28 ns |      1.72 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | Sorted             |     5,993.6 ns |    214.60 ns |     95.28 ns |  0.09 |    0.00 |    4 |         - |          NA |
| SpinSort                 | 4096 | Sorted             |     2,016.4 ns |      4.39 ns |      1.95 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 4096 | Sorted             |     1,987.2 ns |     40.83 ns |     14.56 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | Sorted             |     2,351.2 ns |     11.49 ns |      5.10 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Sorted             |    20,612.1 ns |    954.83 ns |    499.40 ns |  0.30 |    0.01 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **Reversed**           |   **156,649.1 ns** |    **888.61 ns** |    **394.55 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Reversed           |   143,234.6 ns |  1,048.97 ns |    548.63 ns |  0.91 |    0.00 |    5 |         - |          NA |
| BottomupMergeSort        | 4096 | Reversed           |    83,182.6 ns |  3,420.25 ns |  1,788.86 ns |  0.53 |    0.01 |    3 |         - |          NA |
| StdStableSort            | 4096 | Reversed           |   111,476.8 ns |    166.74 ns |     59.46 ns |  0.71 |    0.00 |    4 |         - |          NA |
| RotateMergeSort          | 4096 | Reversed           |    42,644.7 ns |    410.48 ns |    214.69 ns |  0.27 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Reversed           |    48,750.1 ns |    771.89 ns |    403.71 ns |  0.31 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 4096 | Reversed           |    41,788.3 ns |    721.39 ns |    320.30 ns |  0.27 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 4096 | Reversed           |    73,315.0 ns |    689.73 ns |    360.74 ns |  0.47 |    0.00 |    3 |         - |          NA |
| NaturalMergeSort         | 4096 | Reversed           |     4,385.2 ns |     44.43 ns |     15.85 ns |  0.03 |    0.00 |    1 |         - |          NA |
| TimSort                  | 4096 | Reversed           |     3,539.3 ns |     11.12 ns |      4.94 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | Reversed           |     3,657.9 ns |    353.20 ns |    184.73 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | Reversed           |    79,755.0 ns |    631.98 ns |    330.54 ns |  0.51 |    0.00 |    3 |         - |          NA |
| SpinSort                 | 4096 | Reversed           |     4,059.5 ns |    271.39 ns |    141.94 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 4096 | Reversed           |     3,618.1 ns |     30.47 ns |     10.87 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | Reversed           |     3,614.8 ns |     10.58 ns |      3.77 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Reversed           |    47,456.5 ns |  1,004.87 ns |    446.17 ns |  0.30 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **PipeOrgan**          |   **116,243.7 ns** |  **1,413.86 ns** |    **739.47 ns** |  **1.00** |    **0.01** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | PipeOrgan          |   117,258.8 ns |    906.78 ns |    402.62 ns |  1.01 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 4096 | PipeOrgan          |    56,352.0 ns |  2,571.26 ns |  1,141.65 ns |  0.48 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 4096 | PipeOrgan          |    66,421.1 ns |  1,491.23 ns |    779.94 ns |  0.57 |    0.01 |    6 |         - |          NA |
| RotateMergeSort          | 4096 | PipeOrgan          |    81,021.2 ns |    686.42 ns |    304.78 ns |  0.70 |    0.00 |    6 |         - |          NA |
| RotateMergeSortRecursive | 4096 | PipeOrgan          |    97,146.6 ns |    357.25 ns |    186.85 ns |  0.84 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 4096 | PipeOrgan          |    64,211.6 ns |  2,538.40 ns |  1,327.63 ns |  0.55 |    0.01 |    6 |         - |          NA |
| BlockMergeSort           | 4096 | PipeOrgan          |    68,445.4 ns |    919.53 ns |    480.93 ns |  0.59 |    0.01 |    6 |         - |          NA |
| NaturalMergeSort         | 4096 | PipeOrgan          |    10,709.0 ns |  1,065.93 ns |    473.28 ns |  0.09 |    0.00 |    3 |         - |          NA |
| TimSort                  | 4096 | PipeOrgan          |    11,218.1 ns |    290.93 ns |    129.17 ns |  0.10 |    0.00 |    3 |         - |          NA |
| PowerSort                | 4096 | PipeOrgan          |     6,968.6 ns |    300.30 ns |    133.33 ns |  0.06 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | PipeOrgan          |    48,723.7 ns |    398.22 ns |    176.81 ns |  0.42 |    0.00 |    6 |         - |          NA |
| SpinSort                 | 4096 | PipeOrgan          |     8,727.1 ns |  1,098.83 ns |    487.89 ns |  0.08 |    0.00 |    2 |         - |          NA |
| Glidesort                | 4096 | PipeOrgan          |    19,028.4 ns |    342.76 ns |    152.19 ns |  0.16 |    0.00 |    4 |         - |          NA |
| Driftsort                | 4096 | PipeOrgan          |     5,849.1 ns |     51.37 ns |     18.32 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | PipeOrgan          |    37,473.2 ns |    649.47 ns |    288.37 ns |  0.32 |    0.00 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **ManyDuplicates**     |   **155,046.5 ns** |  **3,969.40 ns** |  **1,762.44 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | ManyDuplicates     |   150,893.5 ns |  2,328.35 ns |  1,217.77 ns |  0.97 |    0.01 |    2 |         - |          NA |
| BottomupMergeSort        | 4096 | ManyDuplicates     |    91,121.3 ns |  3,043.06 ns |  1,591.58 ns |  0.59 |    0.01 |    1 |         - |          NA |
| StdStableSort            | 4096 | ManyDuplicates     |    71,301.7 ns |  5,262.70 ns |  2,752.49 ns |  0.46 |    0.02 |    1 |         - |          NA |
| RotateMergeSort          | 4096 | ManyDuplicates     |   343,784.2 ns | 34,189.27 ns | 15,180.25 ns |  2.22 |    0.09 |    4 |         - |          NA |
| RotateMergeSortRecursive | 4096 | ManyDuplicates     |   313,586.4 ns | 73,966.19 ns | 38,685.75 ns |  2.02 |    0.24 |    4 |         - |          NA |
| SymMergeSort             | 4096 | ManyDuplicates     |   211,176.6 ns | 10,755.71 ns |  4,775.60 ns |  1.36 |    0.03 |    3 |         - |          NA |
| BlockMergeSort           | 4096 | ManyDuplicates     |   133,711.7 ns |  1,525.27 ns |    677.23 ns |  0.86 |    0.01 |    2 |         - |          NA |
| NaturalMergeSort         | 4096 | ManyDuplicates     |   116,044.3 ns |  4,418.46 ns |  2,310.94 ns |  0.75 |    0.02 |    2 |         - |          NA |
| TimSort                  | 4096 | ManyDuplicates     |    82,800.7 ns |  4,246.36 ns |  2,220.93 ns |  0.53 |    0.01 |    1 |         - |          NA |
| PowerSort                | 4096 | ManyDuplicates     |    58,163.6 ns |  2,901.61 ns |  1,517.60 ns |  0.38 |    0.01 |    1 |         - |          NA |
| ShiftSort                | 4096 | ManyDuplicates     |    96,238.2 ns |  5,021.60 ns |  2,626.39 ns |  0.62 |    0.02 |    1 |         - |          NA |
| SpinSort                 | 4096 | ManyDuplicates     |    54,309.6 ns |  3,580.53 ns |  1,872.69 ns |  0.35 |    0.01 |    1 |         - |          NA |
| Glidesort                | 4096 | ManyDuplicates     |    47,158.4 ns |    910.54 ns |    404.29 ns |  0.30 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | ManyDuplicates     |    43,439.0 ns |    897.97 ns |    469.66 ns |  0.28 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | ManyDuplicates     |    59,570.8 ns |  3,150.29 ns |  1,647.66 ns |  0.38 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **Random**             |   **761,759.6 ns** | **11,291.98 ns** |  **5,905.93 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Random             |   663,184.4 ns |  3,782.38 ns |  1,978.26 ns |  0.87 |    0.01 |    2 |         - |          NA |
| BottomupMergeSort        | 8192 | Random             |   493,684.9 ns |  4,482.55 ns |  2,344.46 ns |  0.65 |    0.01 |    2 |         - |          NA |
| StdStableSort            | 8192 | Random             |   388,330.5 ns |  8,397.79 ns |  4,392.21 ns |  0.51 |    0.01 |    2 |         - |          NA |
| RotateMergeSort          | 8192 | Random             | 1,553,354.3 ns |  6,315.05 ns |  3,302.89 ns |  2.04 |    0.02 |    4 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Random             | 1,683,983.7 ns |  4,750.06 ns |  2,484.37 ns |  2.21 |    0.02 |    4 |         - |          NA |
| SymMergeSort             | 8192 | Random             | 1,116,615.3 ns |  3,568.11 ns |  1,866.19 ns |  1.47 |    0.01 |    3 |         - |          NA |
| BlockMergeSort           | 8192 | Random             |   670,321.0 ns | 50,841.02 ns | 22,573.73 ns |  0.88 |    0.03 |    2 |         - |          NA |
| NaturalMergeSort         | 8192 | Random             |   683,838.8 ns |  2,420.24 ns |  1,265.83 ns |  0.90 |    0.01 |    2 |         - |          NA |
| TimSort                  | 8192 | Random             |   583,485.3 ns |  6,741.52 ns |  3,525.94 ns |  0.77 |    0.01 |    2 |         - |          NA |
| PowerSort                | 8192 | Random             |   437,047.2 ns |  2,451.24 ns |  1,088.37 ns |  0.57 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 8192 | Random             |   634,344.2 ns |  6,333.78 ns |  3,312.69 ns |  0.83 |    0.01 |    2 |         - |          NA |
| SpinSort                 | 8192 | Random             |   366,583.2 ns |  8,508.66 ns |  4,450.19 ns |  0.48 |    0.01 |    2 |         - |          NA |
| Glidesort                | 8192 | Random             |   179,115.8 ns |  2,867.65 ns |  1,499.83 ns |  0.24 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Random             |   208,327.7 ns |  1,499.51 ns |    665.79 ns |  0.27 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Random             |   403,173.8 ns |  5,237.91 ns |  2,739.53 ns |  0.53 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **SingleElementMoved** |   **149,160.5 ns** |  **1,673.34 ns** |    **875.19 ns** |  **1.00** |    **0.01** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | SingleElementMoved |   180,971.1 ns |  2,027.94 ns |  1,060.65 ns |  1.21 |    0.01 |    8 |         - |          NA |
| BottomupMergeSort        | 8192 | SingleElementMoved |    60,406.6 ns |  1,236.98 ns |    646.96 ns |  0.40 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 8192 | SingleElementMoved |    35,170.5 ns |  2,681.93 ns |  1,190.79 ns |  0.24 |    0.01 |    4 |         - |          NA |
| RotateMergeSort          | 8192 | SingleElementMoved |    14,828.4 ns |    607.58 ns |    269.77 ns |  0.10 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 8192 | SingleElementMoved |    15,689.9 ns |    235.87 ns |    123.37 ns |  0.11 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 8192 | SingleElementMoved |    14,625.7 ns |    322.42 ns |    143.16 ns |  0.10 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 8192 | SingleElementMoved |   114,293.5 ns |    815.12 ns |    426.32 ns |  0.77 |    0.01 |    6 |         - |          NA |
| NaturalMergeSort         | 8192 | SingleElementMoved |    17,520.1 ns |    215.92 ns |     95.87 ns |  0.12 |    0.00 |    2 |         - |          NA |
| TimSort                  | 8192 | SingleElementMoved |     6,135.0 ns |    197.98 ns |     87.91 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | SingleElementMoved |    11,810.9 ns |    203.35 ns |    106.36 ns |  0.08 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 8192 | SingleElementMoved |    12,568.8 ns |    376.94 ns |    167.36 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | SingleElementMoved |    23,674.2 ns |    852.52 ns |    378.52 ns |  0.16 |    0.00 |    3 |         - |          NA |
| Glidesort                | 8192 | SingleElementMoved |    23,456.3 ns |    299.58 ns |    133.02 ns |  0.16 |    0.00 |    3 |         - |          NA |
| Driftsort                | 8192 | SingleElementMoved |    11,035.5 ns |  2,015.01 ns |  1,053.89 ns |  0.07 |    0.01 |    2 |         - |          NA |
| FlatStableSort           | 8192 | SingleElementMoved |    50,929.9 ns |    763.50 ns |    339.00 ns |  0.34 |    0.00 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **Sorted**             |   **137,292.8 ns** |    **669.56 ns** |    **297.29 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Sorted             |   171,606.3 ns |  1,237.58 ns |    549.49 ns |  1.25 |    0.00 |    9 |         - |          NA |
| BottomupMergeSort        | 8192 | Sorted             |    50,132.4 ns |  1,477.52 ns |    772.77 ns |  0.37 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 8192 | Sorted             |    34,658.8 ns |    625.42 ns |    327.11 ns |  0.25 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 8192 | Sorted             |     9,821.4 ns |    320.73 ns |    142.40 ns |  0.07 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Sorted             |    14,413.6 ns |    334.97 ns |    148.73 ns |  0.10 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 8192 | Sorted             |     9,332.6 ns |    470.72 ns |    209.00 ns |  0.07 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 8192 | Sorted             |    93,362.4 ns |    378.75 ns |    168.17 ns |  0.68 |    0.00 |    7 |         - |          NA |
| NaturalMergeSort         | 8192 | Sorted             |     5,960.9 ns |    375.79 ns |    166.85 ns |  0.04 |    0.00 |    2 |         - |          NA |
| TimSort                  | 8192 | Sorted             |     4,565.9 ns |     52.04 ns |     18.56 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Sorted             |     4,546.8 ns |     14.92 ns |      5.32 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Sorted             |    12,313.0 ns |  1,061.63 ns |    555.25 ns |  0.09 |    0.00 |    4 |         - |          NA |
| SpinSort                 | 8192 | Sorted             |     4,131.6 ns |    519.12 ns |    230.49 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Sorted             |     4,243.6 ns |    463.35 ns |    242.34 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Sorted             |     4,778.1 ns |    246.52 ns |    128.93 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Sorted             |     4,325.6 ns |    296.67 ns |    155.16 ns |  0.03 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **Reversed**           |   **319,625.0 ns** |  **3,219.77 ns** |  **1,684.00 ns** |  **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Reversed           |   292,485.3 ns |  4,843.10 ns |  2,533.03 ns |  0.92 |    0.01 |    4 |         - |          NA |
| BottomupMergeSort        | 8192 | Reversed           |   172,741.2 ns |  4,186.32 ns |  2,189.52 ns |  0.54 |    0.01 |    3 |         - |          NA |
| StdStableSort            | 8192 | Reversed           |   413,055.4 ns |  4,210.69 ns |  2,202.27 ns |  1.29 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 8192 | Reversed           |    91,446.6 ns |    801.50 ns |    419.20 ns |  0.29 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Reversed           |   103,130.7 ns |    862.37 ns |    382.90 ns |  0.32 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 8192 | Reversed           |    87,181.2 ns |  1,999.69 ns |  1,045.88 ns |  0.27 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 8192 | Reversed           |   152,938.8 ns |    937.81 ns |    490.49 ns |  0.48 |    0.00 |    3 |         - |          NA |
| NaturalMergeSort         | 8192 | Reversed           |     8,998.8 ns |    377.28 ns |    167.51 ns |  0.03 |    0.00 |    1 |         - |          NA |
| TimSort                  | 8192 | Reversed           |     7,441.5 ns |    295.03 ns |    154.31 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Reversed           |     7,178.0 ns |    239.09 ns |    125.05 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Reversed           |   164,084.3 ns |  1,416.05 ns |    740.62 ns |  0.51 |    0.00 |    3 |         - |          NA |
| SpinSort                 | 8192 | Reversed           |     7,881.6 ns |    340.11 ns |    177.89 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Reversed           |     7,299.6 ns |    434.60 ns |    227.31 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Reversed           |     7,475.8 ns |    393.00 ns |    174.50 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Reversed           |     7,828.8 ns |    262.54 ns |    137.31 ns |  0.02 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **PipeOrgan**          |   **235,536.8 ns** |  **1,704.53 ns** |    **891.50 ns** |  **1.00** |    **0.01** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | PipeOrgan          |   238,810.8 ns |  2,329.03 ns |  1,218.13 ns |  1.01 |    0.01 |    8 |         - |          NA |
| BottomupMergeSort        | 8192 | PipeOrgan          |   126,289.3 ns |  3,953.90 ns |  1,755.56 ns |  0.54 |    0.01 |    7 |         - |          NA |
| StdStableSort            | 8192 | PipeOrgan          |   225,685.6 ns |  2,023.73 ns |  1,058.45 ns |  0.96 |    0.01 |    8 |         - |          NA |
| RotateMergeSort          | 8192 | PipeOrgan          |   167,743.2 ns |  1,142.42 ns |    597.51 ns |  0.71 |    0.00 |    8 |         - |          NA |
| RotateMergeSortRecursive | 8192 | PipeOrgan          |   200,621.8 ns |  2,335.62 ns |  1,037.03 ns |  0.85 |    0.01 |    8 |         - |          NA |
| SymMergeSort             | 8192 | PipeOrgan          |   129,190.3 ns |  1,334.01 ns |    592.31 ns |  0.55 |    0.00 |    7 |         - |          NA |
| BlockMergeSort           | 8192 | PipeOrgan          |   139,401.8 ns |    905.84 ns |    473.77 ns |  0.59 |    0.00 |    7 |         - |          NA |
| NaturalMergeSort         | 8192 | PipeOrgan          |    21,966.4 ns |  1,844.93 ns |    819.16 ns |  0.09 |    0.00 |    3 |         - |          NA |
| TimSort                  | 8192 | PipeOrgan          |    22,823.8 ns |    527.49 ns |    234.21 ns |  0.10 |    0.00 |    3 |         - |          NA |
| PowerSort                | 8192 | PipeOrgan          |    14,086.5 ns |    540.88 ns |    240.15 ns |  0.06 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 8192 | PipeOrgan          |   100,268.8 ns |  1,252.18 ns |    654.91 ns |  0.43 |    0.00 |    6 |         - |          NA |
| SpinSort                 | 8192 | PipeOrgan          |    18,707.0 ns |    863.02 ns |    451.38 ns |  0.08 |    0.00 |    3 |         - |          NA |
| Glidesort                | 8192 | PipeOrgan          |    38,007.0 ns |    952.76 ns |    498.31 ns |  0.16 |    0.00 |    4 |         - |          NA |
| Driftsort                | 8192 | PipeOrgan          |    11,535.1 ns |    211.61 ns |     93.96 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | PipeOrgan          |    74,269.2 ns |  1,641.76 ns |    858.67 ns |  0.32 |    0.00 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **ManyDuplicates**     |   **477,376.4 ns** | **22,716.31 ns** | **11,881.07 ns** |  **1.00** |    **0.03** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | ManyDuplicates     |   487,175.6 ns | 15,644.30 ns |  8,182.27 ns |  1.02 |    0.03 |    4 |         - |          NA |
| BottomupMergeSort        | 8192 | ManyDuplicates     |   344,424.6 ns |  6,087.12 ns |  3,183.68 ns |  0.72 |    0.02 |    4 |         - |          NA |
| StdStableSort            | 8192 | ManyDuplicates     |   232,852.9 ns |  7,424.61 ns |  3,883.21 ns |  0.49 |    0.01 |    3 |         - |          NA |
| RotateMergeSort          | 8192 | ManyDuplicates     |   958,031.9 ns |  5,766.61 ns |  3,016.05 ns |  2.01 |    0.05 |    6 |         - |          NA |
| RotateMergeSortRecursive | 8192 | ManyDuplicates     | 1,026,037.5 ns |  3,032.06 ns |  1,585.82 ns |  2.15 |    0.05 |    6 |         - |          NA |
| SymMergeSort             | 8192 | ManyDuplicates     |   758,642.7 ns |  2,961.89 ns |  1,315.10 ns |  1.59 |    0.04 |    5 |         - |          NA |
| BlockMergeSort           | 8192 | ManyDuplicates     |   544,574.2 ns |  5,717.47 ns |  2,990.35 ns |  1.14 |    0.03 |    4 |         - |          NA |
| NaturalMergeSort         | 8192 | ManyDuplicates     |   502,029.5 ns |  3,520.78 ns |  1,841.44 ns |  1.05 |    0.02 |    4 |         - |          NA |
| TimSort                  | 8192 | ManyDuplicates     |   386,201.3 ns |  6,538.02 ns |  3,419.51 ns |  0.81 |    0.02 |    4 |         - |          NA |
| PowerSort                | 8192 | ManyDuplicates     |   192,040.2 ns |  8,893.50 ns |  4,651.47 ns |  0.40 |    0.01 |    2 |         - |          NA |
| ShiftSort                | 8192 | ManyDuplicates     |   415,154.9 ns |  7,036.42 ns |  3,124.21 ns |  0.87 |    0.02 |    4 |         - |          NA |
| SpinSort                 | 8192 | ManyDuplicates     |   185,496.3 ns |  6,428.32 ns |  3,362.13 ns |  0.39 |    0.01 |    2 |         - |          NA |
| Glidesort                | 8192 | ManyDuplicates     |    91,999.4 ns |  2,255.71 ns |  1,179.78 ns |  0.19 |    0.01 |    1 |         - |          NA |
| Driftsort                | 8192 | ManyDuplicates     |    83,405.8 ns |  1,320.60 ns |    690.70 ns |  0.17 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | ManyDuplicates     |   159,880.1 ns |  4,292.64 ns |  2,245.13 ns |  0.34 |    0.01 |    2 |         - |          NA |

### NetworkBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.87GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                  | Size | Pattern            | Mean           | Error        | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------------ |----- |------------------- |---------------:|-------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **BitonicSort**             | **256**  | **Random**             |    **10,143.5 ns** |    **496.54 ns** |   **259.70 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Random             |    22,911.2 ns |    151.72 ns |    79.35 ns |  2.26 |    0.05 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Random             |    18,704.4 ns |    367.27 ns |   163.07 ns |  1.85 |    0.05 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **SingleElementMoved** |     **9,853.3 ns** |    **484.62 ns** |   **253.46 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | SingleElementMoved |    24,089.8 ns |  1,630.97 ns |   853.03 ns |  2.45 |    0.10 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | SingleElementMoved |    18,761.2 ns |    267.06 ns |   139.68 ns |  1.91 |    0.05 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **Sorted**             |     **9,689.3 ns** |    **185.18 ns** |    **96.85 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Sorted             |    23,236.6 ns |    257.04 ns |   134.44 ns |  2.40 |    0.03 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Sorted             |    18,629.6 ns |     80.33 ns |    42.01 ns |  1.92 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **Reversed**           |     **9,809.2 ns** |    **513.95 ns** |   **268.81 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Reversed           |    23,350.6 ns |    164.06 ns |    72.84 ns |  2.38 |    0.06 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Reversed           |    18,677.9 ns |    133.28 ns |    59.18 ns |  1.91 |    0.05 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **PipeOrgan**          |     **9,887.7 ns** |    **575.90 ns** |   **301.21 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | PipeOrgan          |    23,180.5 ns |     78.08 ns |    40.84 ns |  2.35 |    0.07 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | PipeOrgan          |    18,684.8 ns |    209.08 ns |   109.35 ns |  1.89 |    0.05 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **ManyDuplicates**     |    **10,294.8 ns** |    **714.34 ns** |   **317.17 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | ManyDuplicates     |    22,905.3 ns |    306.19 ns |   135.95 ns |  2.23 |    0.07 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | ManyDuplicates     |    18,677.2 ns |    140.12 ns |    62.21 ns |  1.82 |    0.05 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Random**             |    **60,079.8 ns** |  **1,475.70 ns** |   **771.82 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Random             |   118,518.3 ns |    331.13 ns |   147.02 ns |  1.97 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Random             |   115,502.0 ns |  1,354.58 ns |   708.47 ns |  1.92 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **SingleElementMoved** |    **60,915.6 ns** |    **673.99 ns** |   **352.51 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | SingleElementMoved |   120,507.5 ns |    767.02 ns |   340.56 ns |  1.98 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | SingleElementMoved |   114,983.3 ns |    278.22 ns |   145.51 ns |  1.89 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Sorted**             |    **58,570.0 ns** |  **3,398.38 ns** | **1,211.89 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Sorted             |   119,847.8 ns |  1,467.85 ns |   767.71 ns |  2.05 |    0.04 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Sorted             |   115,292.1 ns |    591.30 ns |   309.26 ns |  1.97 |    0.04 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Reversed**           |    **60,211.8 ns** |    **804.85 ns** |   **420.95 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Reversed           |   119,727.4 ns |    360.88 ns |   160.23 ns |  1.99 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Reversed           |   115,025.5 ns |    154.72 ns |    80.92 ns |  1.91 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **PipeOrgan**          |    **60,292.2 ns** |  **1,525.72 ns** |   **797.98 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | PipeOrgan          |   119,996.2 ns |    428.69 ns |   190.34 ns |  1.99 |    0.03 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | PipeOrgan          |   115,020.3 ns |    337.33 ns |   176.43 ns |  1.91 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **ManyDuplicates**     |    **59,479.3 ns** |  **1,459.29 ns** |   **647.94 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | ManyDuplicates     |   117,442.7 ns |    812.98 ns |   425.21 ns |  1.97 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | ManyDuplicates     |   115,055.3 ns |    387.31 ns |   171.97 ns |  1.93 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Random**             |   **567,424.5 ns** |  **4,890.75 ns** | **2,557.95 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Random             |   830,923.4 ns |  4,130.81 ns | 2,160.50 ns |  1.46 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Random             |   684,893.6 ns |  4,168.45 ns | 2,180.18 ns |  1.21 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **SingleElementMoved** |   **342,083.9 ns** |  **3,314.66 ns** | **1,733.63 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | SingleElementMoved |   599,574.0 ns |  1,821.36 ns |   952.61 ns |  1.75 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | SingleElementMoved |   658,904.9 ns |    689.52 ns |   306.15 ns |  1.93 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Sorted**             |   **337,534.1 ns** | **10,605.24 ns** | **5,546.74 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Sorted             |   591,933.9 ns |  1,369.45 ns |   608.04 ns |  1.75 |    0.03 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Sorted             |   658,612.3 ns |    682.87 ns |   357.15 ns |  1.95 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Reversed**           |   **335,677.1 ns** |  **3,012.09 ns** | **1,337.38 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Reversed           |   598,403.4 ns |  3,482.63 ns | 1,546.31 ns |  1.78 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Reversed           |   658,929.7 ns |    715.96 ns |   317.89 ns |  1.96 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **PipeOrgan**          |   **338,883.3 ns** |  **2,497.60 ns** | **1,108.95 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | PipeOrgan          |   598,316.2 ns |  1,093.45 ns |   571.90 ns |  1.77 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | PipeOrgan          |   658,944.3 ns |    434.36 ns |   192.86 ns |  1.94 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **ManyDuplicates**     |   **456,453.7 ns** |  **5,188.81 ns** | **2,713.85 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | ManyDuplicates     |   709,847.7 ns |  5,332.03 ns | 2,367.45 ns |  1.56 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | ManyDuplicates     |   661,300.5 ns |  1,645.28 ns |   860.51 ns |  1.45 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **8192** | **Random**             | **1,317,395.5 ns** |  **7,544.46 ns** | **3,349.79 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | Random             | 1,953,769.4 ns |  8,315.61 ns | 2,965.43 ns |  1.48 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | Random             | 1,682,389.4 ns |  4,967.65 ns | 2,598.18 ns |  1.28 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **8192** | **SingleElementMoved** |   **793,243.2 ns** |  **7,132.17 ns** | **3,730.26 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | SingleElementMoved | 1,350,546.7 ns |  1,222.97 ns |   639.64 ns |  1.70 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | SingleElementMoved | 1,541,377.7 ns |  1,533.55 ns |   802.08 ns |  1.94 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **8192** | **Sorted**             |   **780,049.7 ns** | **15,092.14 ns** | **7,893.48 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | Sorted             | 1,333,789.1 ns |  3,127.73 ns | 1,635.86 ns |  1.71 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | Sorted             | 1,541,095.2 ns |  1,388.71 ns |   726.32 ns |  1.98 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **8192** | **Reversed**           |   **782,337.9 ns** |  **5,207.00 ns** | **2,723.36 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | Reversed           | 1,347,605.2 ns |  4,939.25 ns | 2,583.32 ns |  1.72 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | Reversed           | 1,542,875.0 ns |  1,905.33 ns |   996.52 ns |  1.97 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **8192** | **PipeOrgan**          |   **787,857.5 ns** |  **3,833.37 ns** | **1,702.04 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | PipeOrgan          | 1,348,564.7 ns |  2,143.65 ns | 1,121.17 ns |  1.71 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | PipeOrgan          | 1,542,306.9 ns |    764.08 ns |   339.26 ns |  1.96 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **8192** | **ManyDuplicates**     | **1,061,871.4 ns** |  **6,422.80 ns** | **2,851.76 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | ManyDuplicates     | 1,685,527.6 ns |  5,117.93 ns | 2,676.78 ns |  1.59 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | ManyDuplicates     | 1,591,260.3 ns |  2,272.86 ns | 1,009.16 ns |  1.50 |    0.00 |    2 |         - |          NA |

### PartitionBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.87GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                       | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **QuickSort**                    | **256**  | **Random**             |     **2,957.4 ns** |    **735.01 ns** |    **384.43 ns** |  **1.01** |    **0.17** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 256  | Random             |     2,445.7 ns |    156.10 ns |     69.31 ns |  0.84 |    0.10 |    1 |         - |          NA |
| QuickSortMedian3             | 256  | Random             |     2,907.4 ns |    276.08 ns |    144.40 ns |  1.00 |    0.12 |    1 |         - |          NA |
| QuickSortMedian9             | 256  | Random             |     3,172.6 ns |    313.09 ns |    163.75 ns |  1.09 |    0.13 |    1 |         - |          NA |
| DualPivotQuickSort           | 256  | Random             |     2,195.0 ns |     55.51 ns |     24.65 ns |  0.75 |    0.09 |    1 |         - |          NA |
| StableQuickSort              | 256  | Random             |    11,519.7 ns |    393.94 ns |    206.04 ns |  3.95 |    0.45 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Random             |     7,577.2 ns |    206.35 ns |    107.92 ns |  2.60 |    0.29 |    3 |         - |          NA |
| DestswapStableQuickSort      | 256  | Random             |     7,089.4 ns |    113.79 ns |     50.52 ns |  2.43 |    0.27 |    3 |         - |          NA |
| IntroSort                    | 256  | Random             |     2,265.2 ns |    183.33 ns |     81.40 ns |  0.78 |    0.09 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | Random             |     1,759.4 ns |     31.30 ns |     13.90 ns |  0.60 |    0.07 |    1 |         - |          NA |
| PDQSort                      | 256  | Random             |     1,823.6 ns |     66.32 ns |     29.45 ns |  0.63 |    0.07 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | Random             |     2,924.2 ns |     80.43 ns |     35.71 ns |  1.00 |    0.11 |    1 |         - |          NA |
| Ipnsort                      | 256  | Random             |     3,838.6 ns |    294.91 ns |    154.24 ns |  1.32 |    0.16 |    2 |         - |          NA |
| StdSort                      | 256  | Random             |     2,839.4 ns |    201.16 ns |    105.21 ns |  0.97 |    0.11 |    1 |         - |          NA |
| BlockQuickSort               | 256  | Random             |     2,850.1 ns |     41.17 ns |     14.68 ns |  0.98 |    0.11 |    1 |         - |          NA |
| DotnetSort                   | 256  | Random             |     2,105.8 ns |    131.45 ns |     46.88 ns |  0.72 |    0.08 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **SingleElementMoved** |     **1,176.3 ns** |     **34.87 ns** |     **15.48 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 256  | SingleElementMoved |     1,030.3 ns |     30.21 ns |     13.41 ns |  0.88 |    0.02 |    1 |         - |          NA |
| QuickSortMedian3             | 256  | SingleElementMoved |     1,764.9 ns |     83.66 ns |     37.15 ns |  1.50 |    0.03 |    2 |         - |          NA |
| QuickSortMedian9             | 256  | SingleElementMoved |     2,282.4 ns |    235.88 ns |    104.73 ns |  1.94 |    0.09 |    3 |         - |          NA |
| DualPivotQuickSort           | 256  | SingleElementMoved |       869.0 ns |     30.37 ns |     13.48 ns |  0.74 |    0.01 |    1 |         - |          NA |
| StableQuickSort              | 256  | SingleElementMoved |     8,966.1 ns |    130.71 ns |     68.36 ns |  7.62 |    0.11 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 256  | SingleElementMoved |     5,351.7 ns |    506.20 ns |    264.75 ns |  4.55 |    0.22 |    5 |         - |          NA |
| DestswapStableQuickSort      | 256  | SingleElementMoved |     4,756.9 ns |    475.24 ns |    248.56 ns |  4.04 |    0.21 |    5 |         - |          NA |
| IntroSort                    | 256  | SingleElementMoved |       982.8 ns |    304.91 ns |    159.48 ns |  0.84 |    0.13 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | SingleElementMoved |     1,291.9 ns |    215.91 ns |     95.87 ns |  1.10 |    0.08 |    1 |         - |          NA |
| PDQSort                      | 256  | SingleElementMoved |     1,139.1 ns |     29.00 ns |     12.88 ns |  0.97 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | SingleElementMoved |     1,417.8 ns |     33.82 ns |     15.02 ns |  1.21 |    0.02 |    1 |         - |          NA |
| Ipnsort                      | 256  | SingleElementMoved |     3,655.1 ns |     85.14 ns |     37.80 ns |  3.11 |    0.05 |    4 |         - |          NA |
| StdSort                      | 256  | SingleElementMoved |     1,753.4 ns |     46.14 ns |     16.46 ns |  1.49 |    0.02 |    2 |         - |          NA |
| BlockQuickSort               | 256  | SingleElementMoved |     1,796.6 ns |     25.27 ns |     11.22 ns |  1.53 |    0.02 |    2 |         - |          NA |
| DotnetSort                   | 256  | SingleElementMoved |     1,011.0 ns |     37.40 ns |     16.61 ns |  0.86 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Sorted**             |       **869.5 ns** |     **65.20 ns** |     **28.95 ns** |  **1.00** |    **0.04** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 256  | Sorted             |       734.0 ns |      9.07 ns |      3.24 ns |  0.84 |    0.03 |    4 |         - |          NA |
| QuickSortMedian3             | 256  | Sorted             |     1,329.9 ns |    288.64 ns |    150.96 ns |  1.53 |    0.17 |    5 |         - |          NA |
| QuickSortMedian9             | 256  | Sorted             |     1,316.0 ns |     68.90 ns |     30.59 ns |  1.51 |    0.06 |    5 |         - |          NA |
| DualPivotQuickSort           | 256  | Sorted             |       710.9 ns |    138.37 ns |     61.43 ns |  0.82 |    0.07 |    4 |         - |          NA |
| StableQuickSort              | 256  | Sorted             |     9,155.5 ns |    325.36 ns |    170.17 ns | 10.54 |    0.38 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Sorted             |     4,569.8 ns |     40.36 ns |     14.39 ns |  5.26 |    0.17 |    6 |         - |          NA |
| DestswapStableQuickSort      | 256  | Sorted             |     4,417.3 ns |    289.79 ns |    128.67 ns |  5.09 |    0.21 |    6 |         - |          NA |
| IntroSort                    | 256  | Sorted             |       365.4 ns |     57.76 ns |     30.21 ns |  0.42 |    0.04 |    2 |         - |          NA |
| IntroSortDotnet              | 256  | Sorted             |     1,003.2 ns |    151.63 ns |     67.33 ns |  1.15 |    0.08 |    4 |         - |          NA |
| PDQSort                      | 256  | Sorted             |       372.8 ns |     56.19 ns |     29.39 ns |  0.43 |    0.03 |    2 |         - |          NA |
| PDQSortBranchless            | 256  | Sorted             |       476.8 ns |    220.50 ns |    115.33 ns |  0.55 |    0.13 |    3 |         - |          NA |
| Ipnsort                      | 256  | Sorted             |       202.5 ns |     84.53 ns |     37.53 ns |  0.23 |    0.04 |    1 |         - |          NA |
| StdSort                      | 256  | Sorted             |       352.9 ns |      2.21 ns |      0.98 ns |  0.41 |    0.01 |    2 |         - |          NA |
| BlockQuickSort               | 256  | Sorted             |     1,392.1 ns |     16.18 ns |      5.77 ns |  1.60 |    0.05 |    5 |         - |          NA |
| DotnetSort                   | 256  | Sorted             |       843.0 ns |     63.14 ns |     33.02 ns |  0.97 |    0.05 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Reversed**           |     **1,037.2 ns** |     **95.97 ns** |     **50.20 ns** |  **1.00** |    **0.06** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 256  | Reversed           |     1,002.9 ns |     94.36 ns |     49.35 ns |  0.97 |    0.06 |    3 |         - |          NA |
| QuickSortMedian3             | 256  | Reversed           |     1,275.6 ns |     49.63 ns |     22.04 ns |  1.23 |    0.06 |    4 |         - |          NA |
| QuickSortMedian9             | 256  | Reversed           |     1,490.9 ns |    115.86 ns |     51.44 ns |  1.44 |    0.08 |    4 |         - |          NA |
| DualPivotQuickSort           | 256  | Reversed           |     1,009.3 ns |     26.52 ns |      9.46 ns |  0.98 |    0.04 |    3 |         - |          NA |
| StableQuickSort              | 256  | Reversed           |     8,728.9 ns |    148.84 ns |     77.84 ns |  8.43 |    0.39 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Reversed           |     5,060.2 ns |    268.85 ns |    140.61 ns |  4.89 |    0.25 |    5 |         - |          NA |
| DestswapStableQuickSort      | 256  | Reversed           |     7,245.0 ns |    327.86 ns |    145.57 ns |  7.00 |    0.34 |    6 |         - |          NA |
| IntroSort                    | 256  | Reversed           |       491.7 ns |     16.04 ns |      7.12 ns |  0.48 |    0.02 |    2 |         - |          NA |
| IntroSortDotnet              | 256  | Reversed           |     1,470.9 ns |    100.63 ns |     44.68 ns |  1.42 |    0.08 |    4 |         - |          NA |
| PDQSort                      | 256  | Reversed           |       568.3 ns |     30.50 ns |     15.95 ns |  0.55 |    0.03 |    2 |         - |          NA |
| PDQSortBranchless            | 256  | Reversed           |       901.3 ns |      4.86 ns |      2.16 ns |  0.87 |    0.04 |    3 |         - |          NA |
| Ipnsort                      | 256  | Reversed           |       274.9 ns |    107.55 ns |     56.25 ns |  0.27 |    0.05 |    1 |         - |          NA |
| StdSort                      | 256  | Reversed           |       806.7 ns |      7.42 ns |      3.30 ns |  0.78 |    0.04 |    3 |         - |          NA |
| BlockQuickSort               | 256  | Reversed           |     1,685.5 ns |     85.59 ns |     44.77 ns |  1.63 |    0.08 |    4 |         - |          NA |
| DotnetSort                   | 256  | Reversed           |     1,380.9 ns |    643.60 ns |    285.76 ns |  1.33 |    0.27 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **PipeOrgan**          |     **7,443.1 ns** |     **95.98 ns** |     **42.62 ns** |  **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 256  | PipeOrgan          |     2,828.6 ns |    413.17 ns |    216.10 ns |  0.38 |    0.03 |    2 |         - |          NA |
| QuickSortMedian3             | 256  | PipeOrgan          |     3,230.2 ns |    248.08 ns |    129.75 ns |  0.43 |    0.02 |    2 |         - |          NA |
| QuickSortMedian9             | 256  | PipeOrgan          |     2,565.4 ns |     36.91 ns |     16.39 ns |  0.34 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort           | 256  | PipeOrgan          |     1,536.8 ns |     46.94 ns |     16.74 ns |  0.21 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 256  | PipeOrgan          |     8,498.9 ns |    261.78 ns |    116.23 ns |  1.14 |    0.02 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 256  | PipeOrgan          |     5,162.6 ns |    300.81 ns |    157.33 ns |  0.69 |    0.02 |    3 |         - |          NA |
| DestswapStableQuickSort      | 256  | PipeOrgan          |     7,628.6 ns |     99.34 ns |     51.95 ns |  1.02 |    0.01 |    4 |         - |          NA |
| IntroSort                    | 256  | PipeOrgan          |     1,577.1 ns |    253.93 ns |    132.81 ns |  0.21 |    0.02 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | PipeOrgan          |     2,123.3 ns |     41.87 ns |     18.59 ns |  0.29 |    0.00 |    2 |         - |          NA |
| PDQSort                      | 256  | PipeOrgan          |     1,700.1 ns |     65.65 ns |     29.15 ns |  0.23 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | PipeOrgan          |     2,855.1 ns |    173.27 ns |     76.93 ns |  0.38 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 256  | PipeOrgan          |     3,866.0 ns |     21.33 ns |      9.47 ns |  0.52 |    0.00 |    3 |         - |          NA |
| StdSort                      | 256  | PipeOrgan          |     4,682.7 ns |    310.08 ns |    162.18 ns |  0.63 |    0.02 |    3 |         - |          NA |
| BlockQuickSort               | 256  | PipeOrgan          |     4,861.2 ns |     51.25 ns |     18.28 ns |  0.65 |    0.00 |    3 |         - |          NA |
| DotnetSort                   | 256  | PipeOrgan          |     2,503.2 ns |     47.89 ns |     17.08 ns |  0.34 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **ManyDuplicates**     |     **2,495.8 ns** |    **195.28 ns** |    **102.14 ns** |  **1.00** |    **0.05** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 256  | ManyDuplicates     |     1,813.9 ns |    115.32 ns |     51.20 ns |  0.73 |    0.03 |    1 |         - |          NA |
| QuickSortMedian3             | 256  | ManyDuplicates     |     2,729.9 ns |     99.61 ns |     44.23 ns |  1.10 |    0.05 |    1 |         - |          NA |
| QuickSortMedian9             | 256  | ManyDuplicates     |     2,849.7 ns |    224.13 ns |    117.22 ns |  1.14 |    0.06 |    1 |         - |          NA |
| DualPivotQuickSort           | 256  | ManyDuplicates     |     1,903.1 ns |     70.87 ns |     31.47 ns |  0.76 |    0.03 |    1 |         - |          NA |
| StableQuickSort              | 256  | ManyDuplicates     |     6,635.6 ns |    334.53 ns |    174.97 ns |  2.66 |    0.12 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 256  | ManyDuplicates     |     3,636.4 ns |     29.08 ns |     10.37 ns |  1.46 |    0.06 |    2 |         - |          NA |
| DestswapStableQuickSort      | 256  | ManyDuplicates     |     5,528.9 ns |    244.58 ns |    127.92 ns |  2.22 |    0.10 |    3 |         - |          NA |
| IntroSort                    | 256  | ManyDuplicates     |     2,117.1 ns |     28.83 ns |     10.28 ns |  0.85 |    0.03 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | ManyDuplicates     |     1,650.9 ns |     19.52 ns |      8.67 ns |  0.66 |    0.03 |    1 |         - |          NA |
| PDQSort                      | 256  | ManyDuplicates     |     1,614.7 ns |     96.58 ns |     42.88 ns |  0.65 |    0.03 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | ManyDuplicates     |     2,455.9 ns |     42.48 ns |     18.86 ns |  0.99 |    0.04 |    1 |         - |          NA |
| Ipnsort                      | 256  | ManyDuplicates     |     3,768.3 ns |     75.10 ns |     33.35 ns |  1.51 |    0.06 |    2 |         - |          NA |
| StdSort                      | 256  | ManyDuplicates     |     2,592.3 ns |     54.84 ns |     24.35 ns |  1.04 |    0.04 |    1 |         - |          NA |
| BlockQuickSort               | 256  | ManyDuplicates     |     2,527.1 ns |     27.02 ns |     12.00 ns |  1.01 |    0.04 |    1 |         - |          NA |
| DotnetSort                   | 256  | ManyDuplicates     |     1,746.4 ns |     12.36 ns |      4.41 ns |  0.70 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Random**             |    **13,812.9 ns** |    **302.17 ns** |    **134.17 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Random             |    13,276.8 ns |    890.64 ns |    465.82 ns |  0.96 |    0.03 |    1 |         - |          NA |
| QuickSortMedian3             | 1024 | Random             |    13,486.1 ns |    460.07 ns |    240.63 ns |  0.98 |    0.02 |    1 |         - |          NA |
| QuickSortMedian9             | 1024 | Random             |    14,631.9 ns |    171.46 ns |     61.14 ns |  1.06 |    0.01 |    1 |         - |          NA |
| DualPivotQuickSort           | 1024 | Random             |    11,542.4 ns |    804.34 ns |    420.69 ns |  0.84 |    0.03 |    1 |         - |          NA |
| StableQuickSort              | 1024 | Random             |    58,152.2 ns |    436.34 ns |    193.74 ns |  4.21 |    0.04 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Random             |    36,642.9 ns |    952.97 ns |    498.42 ns |  2.65 |    0.04 |    3 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Random             |    32,156.1 ns |  1,256.99 ns |    657.43 ns |  2.33 |    0.05 |    3 |         - |          NA |
| IntroSort                    | 1024 | Random             |    12,285.4 ns |    463.50 ns |    242.42 ns |  0.89 |    0.02 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | Random             |     9,933.4 ns |    530.62 ns |    277.52 ns |  0.72 |    0.02 |    1 |         - |          NA |
| PDQSort                      | 1024 | Random             |     9,380.5 ns |    400.12 ns |    177.66 ns |  0.68 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | Random             |    13,211.3 ns |    119.74 ns |     53.16 ns |  0.96 |    0.01 |    1 |         - |          NA |
| Ipnsort                      | 1024 | Random             |    19,359.7 ns |    161.25 ns |     84.34 ns |  1.40 |    0.01 |    2 |         - |          NA |
| StdSort                      | 1024 | Random             |    13,405.1 ns |    264.43 ns |    138.30 ns |  0.97 |    0.01 |    1 |         - |          NA |
| BlockQuickSort               | 1024 | Random             |    14,285.6 ns |    449.80 ns |    235.25 ns |  1.03 |    0.02 |    1 |         - |          NA |
| DotnetSort                   | 1024 | Random             |    11,156.1 ns |    409.46 ns |    214.16 ns |  0.81 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **SingleElementMoved** |     **5,690.3 ns** |    **491.53 ns** |    **257.08 ns** |  **1.00** |    **0.06** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 1024 | SingleElementMoved |     5,455.0 ns |    252.08 ns |    131.84 ns |  0.96 |    0.05 |    1 |         - |          NA |
| QuickSortMedian3             | 1024 | SingleElementMoved |     7,933.5 ns |    380.42 ns |    198.97 ns |  1.40 |    0.07 |    2 |         - |          NA |
| QuickSortMedian9             | 1024 | SingleElementMoved |    10,823.0 ns |    405.75 ns |    212.21 ns |  1.91 |    0.09 |    2 |         - |          NA |
| DualPivotQuickSort           | 1024 | SingleElementMoved |     4,236.1 ns |    359.49 ns |    159.62 ns |  0.75 |    0.04 |    1 |         - |          NA |
| StableQuickSort              | 1024 | SingleElementMoved |    43,092.2 ns |    233.45 ns |    122.10 ns |  7.59 |    0.33 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | SingleElementMoved |    26,214.8 ns |  1,577.14 ns |    700.26 ns |  4.62 |    0.23 |    4 |         - |          NA |
| DestswapStableQuickSort      | 1024 | SingleElementMoved |    21,524.1 ns |  1,055.97 ns |    552.29 ns |  3.79 |    0.19 |    3 |         - |          NA |
| IntroSort                    | 1024 | SingleElementMoved |     3,990.7 ns |     51.52 ns |     18.37 ns |  0.70 |    0.03 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | SingleElementMoved |     5,774.5 ns |    325.08 ns |    170.02 ns |  1.02 |    0.05 |    1 |         - |          NA |
| PDQSort                      | 1024 | SingleElementMoved |     4,931.5 ns |    173.77 ns |     77.16 ns |  0.87 |    0.04 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | SingleElementMoved |     6,119.2 ns |    288.78 ns |    151.04 ns |  1.08 |    0.05 |    1 |         - |          NA |
| Ipnsort                      | 1024 | SingleElementMoved |    17,878.4 ns |     87.99 ns |     39.07 ns |  3.15 |    0.14 |    3 |         - |          NA |
| StdSort                      | 1024 | SingleElementMoved |     7,950.4 ns |    206.88 ns |    108.20 ns |  1.40 |    0.06 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | SingleElementMoved |     9,335.3 ns |    189.87 ns |     99.31 ns |  1.64 |    0.07 |    2 |         - |          NA |
| DotnetSort                   | 1024 | SingleElementMoved |     5,393.0 ns |    247.28 ns |    109.79 ns |  0.95 |    0.04 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Sorted**             |     **4,043.8 ns** |     **97.36 ns** |     **34.72 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Sorted             |     3,753.4 ns |     26.98 ns |      9.62 ns |  0.93 |    0.01 |    3 |         - |          NA |
| QuickSortMedian3             | 1024 | Sorted             |     5,483.9 ns |     61.26 ns |     21.85 ns |  1.36 |    0.01 |    3 |         - |          NA |
| QuickSortMedian9             | 1024 | Sorted             |     6,149.3 ns |    413.45 ns |    216.24 ns |  1.52 |    0.05 |    3 |         - |          NA |
| DualPivotQuickSort           | 1024 | Sorted             |     3,528.1 ns |     39.28 ns |     14.01 ns |  0.87 |    0.01 |    3 |         - |          NA |
| StableQuickSort              | 1024 | Sorted             |    46,739.0 ns |    364.10 ns |    161.66 ns | 11.56 |    0.10 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Sorted             |    22,371.8 ns |    385.33 ns |    171.09 ns |  5.53 |    0.06 |    5 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Sorted             |    20,855.2 ns |    705.75 ns |    369.12 ns |  5.16 |    0.10 |    5 |         - |          NA |
| IntroSort                    | 1024 | Sorted             |     1,027.0 ns |     14.43 ns |      6.41 ns |  0.25 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet              | 1024 | Sorted             |     4,782.2 ns |    256.66 ns |    134.24 ns |  1.18 |    0.03 |    3 |         - |          NA |
| PDQSort                      | 1024 | Sorted             |     1,327.3 ns |      7.77 ns |      4.06 ns |  0.33 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | Sorted             |     1,328.8 ns |     18.83 ns |      6.72 ns |  0.33 |    0.00 |    2 |         - |          NA |
| Ipnsort                      | 1024 | Sorted             |       716.8 ns |    127.80 ns |     56.75 ns |  0.18 |    0.01 |    1 |         - |          NA |
| StdSort                      | 1024 | Sorted             |     1,203.4 ns |      3.67 ns |      1.63 ns |  0.30 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | Sorted             |     7,376.4 ns |    198.07 ns |    103.59 ns |  1.82 |    0.03 |    4 |         - |          NA |
| DotnetSort                   | 1024 | Sorted             |     4,047.6 ns |    281.40 ns |    124.94 ns |  1.00 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Reversed**           |     **4,648.1 ns** |    **105.66 ns** |     **46.91 ns** |  **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Reversed           |     4,576.3 ns |     63.50 ns |     22.64 ns |  0.98 |    0.01 |    4 |         - |          NA |
| QuickSortMedian3             | 1024 | Reversed           |     6,000.4 ns |    435.19 ns |    227.61 ns |  1.29 |    0.05 |    5 |         - |          NA |
| QuickSortMedian9             | 1024 | Reversed           |     6,435.0 ns |    312.50 ns |    163.45 ns |  1.38 |    0.04 |    5 |         - |          NA |
| DualPivotQuickSort           | 1024 | Reversed           |     5,016.0 ns |    557.54 ns |    291.61 ns |  1.08 |    0.06 |    4 |         - |          NA |
| StableQuickSort              | 1024 | Reversed           |    42,829.1 ns |    151.99 ns |     67.49 ns |  9.22 |    0.09 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Reversed           |    24,543.1 ns |    647.72 ns |    338.77 ns |  5.28 |    0.09 |    6 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Reversed           |    32,988.3 ns |    622.55 ns |    325.61 ns |  7.10 |    0.09 |    7 |         - |          NA |
| IntroSort                    | 1024 | Reversed           |     2,958.5 ns |     27.38 ns |      9.76 ns |  0.64 |    0.01 |    3 |         - |          NA |
| IntroSortDotnet              | 1024 | Reversed           |     7,204.5 ns |    235.67 ns |    123.26 ns |  1.55 |    0.03 |    5 |         - |          NA |
| PDQSort                      | 1024 | Reversed           |     2,069.0 ns |     12.28 ns |      5.45 ns |  0.45 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | Reversed           |     3,244.7 ns |     18.71 ns |      6.67 ns |  0.70 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 1024 | Reversed           |       911.0 ns |      6.52 ns |      2.90 ns |  0.20 |    0.00 |    1 |         - |          NA |
| StdSort                      | 1024 | Reversed           |     2,945.4 ns |     49.95 ns |     17.81 ns |  0.63 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 1024 | Reversed           |     8,259.9 ns |     57.27 ns |     25.43 ns |  1.78 |    0.02 |    5 |         - |          NA |
| DotnetSort                   | 1024 | Reversed           |     7,278.6 ns |  1,582.41 ns |    827.63 ns |  1.57 |    0.17 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **PipeOrgan**          |   **108,440.4 ns** |    **169.84 ns** |     **75.41 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 1024 | PipeOrgan          |    14,830.6 ns |    725.77 ns |    322.25 ns |  0.14 |    0.00 |    1 |         - |          NA |
| QuickSortMedian3             | 1024 | PipeOrgan          |    16,277.7 ns |    742.94 ns |    388.57 ns |  0.15 |    0.00 |    1 |         - |          NA |
| QuickSortMedian9             | 1024 | PipeOrgan          |    12,201.5 ns |    534.70 ns |    279.66 ns |  0.11 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort           | 1024 | PipeOrgan          |     8,099.1 ns |    513.01 ns |    268.31 ns |  0.07 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 1024 | PipeOrgan          |    43,411.3 ns |    227.60 ns |    101.05 ns |  0.40 |    0.00 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | PipeOrgan          |    24,906.6 ns |    447.88 ns |    198.86 ns |  0.23 |    0.00 |    2 |         - |          NA |
| DestswapStableQuickSort      | 1024 | PipeOrgan          |    36,942.9 ns |    323.09 ns |    168.98 ns |  0.34 |    0.00 |    3 |         - |          NA |
| IntroSort                    | 1024 | PipeOrgan          |    10,812.9 ns |    819.32 ns |    363.78 ns |  0.10 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | PipeOrgan          |    14,195.4 ns |    344.33 ns |    152.89 ns |  0.13 |    0.00 |    1 |         - |          NA |
| PDQSort                      | 1024 | PipeOrgan          |     9,028.9 ns |    277.96 ns |    145.38 ns |  0.08 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | PipeOrgan          |    15,494.7 ns |    215.44 ns |    112.68 ns |  0.14 |    0.00 |    1 |         - |          NA |
| Ipnsort                      | 1024 | PipeOrgan          |    21,062.8 ns |    276.35 ns |    144.54 ns |  0.19 |    0.00 |    2 |         - |          NA |
| StdSort                      | 1024 | PipeOrgan          |    21,578.8 ns |    260.70 ns |    136.35 ns |  0.20 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | PipeOrgan          |    24,528.7 ns |    236.12 ns |    123.50 ns |  0.23 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 1024 | PipeOrgan          |    16,258.1 ns |    832.80 ns |    369.77 ns |  0.15 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **ManyDuplicates**     |     **9,593.9 ns** |    **414.55 ns** |    **216.82 ns** |  **1.00** |    **0.03** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 1024 | ManyDuplicates     |     7,782.7 ns |    283.59 ns |    148.32 ns |  0.81 |    0.02 |    2 |         - |          NA |
| QuickSortMedian3             | 1024 | ManyDuplicates     |    11,744.5 ns |    293.82 ns |    104.78 ns |  1.22 |    0.03 |    2 |         - |          NA |
| QuickSortMedian9             | 1024 | ManyDuplicates     |    12,525.8 ns |    914.61 ns |    406.09 ns |  1.31 |    0.05 |    2 |         - |          NA |
| DualPivotQuickSort           | 1024 | ManyDuplicates     |     7,424.6 ns |    127.76 ns |     56.72 ns |  0.77 |    0.02 |    2 |         - |          NA |
| StableQuickSort              | 1024 | ManyDuplicates     |    29,376.6 ns |    567.78 ns |    252.10 ns |  3.06 |    0.07 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | ManyDuplicates     |    14,353.0 ns |    361.00 ns |    160.28 ns |  1.50 |    0.03 |    2 |         - |          NA |
| DestswapStableQuickSort      | 1024 | ManyDuplicates     |    14,954.7 ns |  1,494.65 ns |    781.73 ns |  1.56 |    0.08 |    2 |         - |          NA |
| IntroSort                    | 1024 | ManyDuplicates     |    11,672.6 ns |  2,200.99 ns |  1,151.16 ns |  1.22 |    0.12 |    2 |         - |          NA |
| IntroSortDotnet              | 1024 | ManyDuplicates     |     8,254.1 ns |    184.02 ns |     81.71 ns |  0.86 |    0.02 |    2 |         - |          NA |
| PDQSort                      | 1024 | ManyDuplicates     |     6,051.8 ns |    252.45 ns |    132.03 ns |  0.63 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | ManyDuplicates     |     8,790.3 ns |     63.39 ns |     22.61 ns |  0.92 |    0.02 |    2 |         - |          NA |
| Ipnsort                      | 1024 | ManyDuplicates     |    18,137.0 ns |    312.92 ns |    138.94 ns |  1.89 |    0.04 |    3 |         - |          NA |
| StdSort                      | 1024 | ManyDuplicates     |    11,121.1 ns |    193.04 ns |    100.96 ns |  1.16 |    0.03 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | ManyDuplicates     |    12,175.1 ns |    378.95 ns |    198.20 ns |  1.27 |    0.03 |    2 |         - |          NA |
| DotnetSort                   | 1024 | ManyDuplicates     |     9,212.0 ns |  1,793.44 ns |    938.00 ns |  0.96 |    0.09 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Random**             |    **64,311.3 ns** |  **2,465.55 ns** |  **1,094.72 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Random             |    77,714.6 ns | 12,290.08 ns |  6,427.95 ns |  1.21 |    0.10 |    1 |         - |          NA |
| QuickSortMedian3             | 4096 | Random             |    65,328.9 ns |  3,114.42 ns |  1,382.82 ns |  1.02 |    0.03 |    1 |         - |          NA |
| QuickSortMedian9             | 4096 | Random             |    72,762.0 ns |  9,864.71 ns |  4,379.99 ns |  1.13 |    0.07 |    1 |         - |          NA |
| DualPivotQuickSort           | 4096 | Random             |    54,871.9 ns |    910.85 ns |    404.43 ns |  0.85 |    0.01 |    1 |         - |          NA |
| StableQuickSort              | 4096 | Random             |   569,067.6 ns |  1,753.80 ns |    917.27 ns |  8.85 |    0.14 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Random             |   435,597.9 ns |    931.33 ns |    413.52 ns |  6.77 |    0.11 |    4 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Random             |   170,402.5 ns | 11,993.19 ns |  5,325.05 ns |  2.65 |    0.09 |    3 |         - |          NA |
| IntroSort                    | 4096 | Random             |    62,755.2 ns |  6,412.52 ns |  2,847.20 ns |  0.98 |    0.04 |    1 |         - |          NA |
| IntroSortDotnet              | 4096 | Random             |    48,805.0 ns |  2,268.48 ns |  1,007.22 ns |  0.76 |    0.02 |    1 |         - |          NA |
| PDQSort                      | 4096 | Random             |    45,297.5 ns |    827.57 ns |    295.12 ns |  0.70 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 4096 | Random             |    62,386.6 ns |    852.35 ns |    445.80 ns |  0.97 |    0.02 |    1 |         - |          NA |
| Ipnsort                      | 4096 | Random             |    98,229.0 ns |  1,033.84 ns |    459.03 ns |  1.53 |    0.03 |    2 |         - |          NA |
| StdSort                      | 4096 | Random             |    62,672.8 ns |    615.45 ns |    273.26 ns |  0.97 |    0.02 |    1 |         - |          NA |
| BlockQuickSort               | 4096 | Random             |    68,662.9 ns |    886.95 ns |    393.81 ns |  1.07 |    0.02 |    1 |         - |          NA |
| DotnetSort                   | 4096 | Random             |    53,286.9 ns |  1,116.87 ns |    495.90 ns |  0.83 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **SingleElementMoved** |    **25,774.2 ns** |  **1,618.74 ns** |    **846.63 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 4096 | SingleElementMoved |    26,376.8 ns |    552.96 ns |    197.19 ns |  1.02 |    0.03 |    1 |         - |          NA |
| QuickSortMedian3             | 4096 | SingleElementMoved |    35,777.5 ns |  1,307.72 ns |    683.96 ns |  1.39 |    0.05 |    1 |         - |          NA |
| QuickSortMedian9             | 4096 | SingleElementMoved |    47,939.4 ns |    608.38 ns |    318.19 ns |  1.86 |    0.06 |    2 |         - |          NA |
| DualPivotQuickSort           | 4096 | SingleElementMoved |    22,861.8 ns |    827.30 ns |    432.69 ns |  0.89 |    0.03 |    1 |         - |          NA |
| StableQuickSort              | 4096 | SingleElementMoved |   208,086.4 ns |  1,259.60 ns |    559.27 ns |  8.08 |    0.25 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | SingleElementMoved |   128,576.7 ns |  6,247.72 ns |  3,267.68 ns |  4.99 |    0.19 |    4 |         - |          NA |
| DestswapStableQuickSort      | 4096 | SingleElementMoved |   102,163.6 ns |  2,218.87 ns |  1,160.51 ns |  3.97 |    0.13 |    3 |         - |          NA |
| IntroSort                    | 4096 | SingleElementMoved |    19,342.2 ns |  1,492.97 ns |    780.85 ns |  0.75 |    0.04 |    1 |         - |          NA |
| IntroSortDotnet              | 4096 | SingleElementMoved |    28,129.1 ns |    996.91 ns |    442.64 ns |  1.09 |    0.04 |    1 |         - |          NA |
| PDQSort                      | 4096 | SingleElementMoved |    21,451.1 ns |    714.78 ns |    373.84 ns |  0.83 |    0.03 |    1 |         - |          NA |
| PDQSortBranchless            | 4096 | SingleElementMoved |    26,424.8 ns |  1,094.78 ns |    572.59 ns |  1.03 |    0.04 |    1 |         - |          NA |
| Ipnsort                      | 4096 | SingleElementMoved |    87,495.1 ns |    906.45 ns |    474.09 ns |  3.40 |    0.11 |    3 |         - |          NA |
| StdSort                      | 4096 | SingleElementMoved |    32,444.6 ns |    615.83 ns |    322.09 ns |  1.26 |    0.04 |    1 |         - |          NA |
| BlockQuickSort               | 4096 | SingleElementMoved |    44,192.9 ns |    632.55 ns |    330.83 ns |  1.72 |    0.05 |    2 |         - |          NA |
| DotnetSort                   | 4096 | SingleElementMoved |    27,205.8 ns |  1,491.07 ns |    662.05 ns |  1.06 |    0.04 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Sorted**             |    **19,296.4 ns** |    **287.88 ns** |    **150.57 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Sorted             |    19,095.0 ns |  1,850.05 ns |    967.61 ns |  0.99 |    0.05 |    3 |         - |          NA |
| QuickSortMedian3             | 4096 | Sorted             |    25,877.8 ns |    939.02 ns |    491.12 ns |  1.34 |    0.03 |    3 |         - |          NA |
| QuickSortMedian9             | 4096 | Sorted             |    27,617.7 ns |    458.09 ns |    203.39 ns |  1.43 |    0.01 |    3 |         - |          NA |
| DualPivotQuickSort           | 4096 | Sorted             |    21,287.2 ns |    508.59 ns |    225.82 ns |  1.10 |    0.01 |    3 |         - |          NA |
| StableQuickSort              | 4096 | Sorted             |   226,560.1 ns |  2,033.29 ns |    902.79 ns | 11.74 |    0.10 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Sorted             |   108,013.6 ns |  2,990.83 ns |  1,564.26 ns |  5.60 |    0.09 |    5 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Sorted             |    96,792.5 ns |  1,932.85 ns |  1,010.92 ns |  5.02 |    0.06 |    5 |         - |          NA |
| IntroSort                    | 4096 | Sorted             |     3,968.4 ns |    298.30 ns |    132.45 ns |  0.21 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 4096 | Sorted             |    22,438.1 ns |    863.02 ns |    383.19 ns |  1.16 |    0.02 |    3 |         - |          NA |
| PDQSort                      | 4096 | Sorted             |     5,256.9 ns |    300.19 ns |    157.00 ns |  0.27 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 4096 | Sorted             |     5,248.5 ns |    390.03 ns |    203.99 ns |  0.27 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 4096 | Sorted             |     2,255.7 ns |      8.67 ns |      4.54 ns |  0.12 |    0.00 |    1 |         - |          NA |
| StdSort                      | 4096 | Sorted             |     4,487.0 ns |    102.10 ns |     45.33 ns |  0.23 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 4096 | Sorted             |    36,044.0 ns |    484.72 ns |    215.22 ns |  1.87 |    0.02 |    4 |         - |          NA |
| DotnetSort                   | 4096 | Sorted             |    19,471.1 ns |    791.86 ns |    351.59 ns |  1.01 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Reversed**           |    **21,991.1 ns** |    **669.01 ns** |    **297.04 ns** |  **1.00** |    **0.02** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Reversed           |    22,155.5 ns |  1,548.54 ns |    687.56 ns |  1.01 |    0.03 |    4 |         - |          NA |
| QuickSortMedian3             | 4096 | Reversed           |    26,573.1 ns |    408.90 ns |    145.82 ns |  1.21 |    0.02 |    4 |         - |          NA |
| QuickSortMedian9             | 4096 | Reversed           |    28,799.9 ns |    520.20 ns |    230.97 ns |  1.31 |    0.02 |    4 |         - |          NA |
| DualPivotQuickSort           | 4096 | Reversed           |    25,908.7 ns |  1,290.63 ns |    675.02 ns |  1.18 |    0.03 |    4 |         - |          NA |
| StableQuickSort              | 4096 | Reversed           |   206,743.5 ns |  1,165.01 ns |    609.32 ns |  9.40 |    0.12 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Reversed           |   117,218.8 ns |  3,032.97 ns |  1,346.66 ns |  5.33 |    0.09 |    6 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Reversed           |   144,677.9 ns |  2,916.52 ns |  1,525.40 ns |  6.58 |    0.11 |    7 |         - |          NA |
| IntroSort                    | 4096 | Reversed           |    13,479.9 ns |    525.16 ns |    233.18 ns |  0.61 |    0.01 |    3 |         - |          NA |
| IntroSortDotnet              | 4096 | Reversed           |    34,830.0 ns |    788.39 ns |    412.34 ns |  1.58 |    0.03 |    5 |         - |          NA |
| PDQSort                      | 4096 | Reversed           |     8,176.2 ns |    342.20 ns |    122.03 ns |  0.37 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 4096 | Reversed           |    12,745.0 ns |    137.87 ns |     49.17 ns |  0.58 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 4096 | Reversed           |     3,792.2 ns |    293.34 ns |    153.42 ns |  0.17 |    0.01 |    1 |         - |          NA |
| StdSort                      | 4096 | Reversed           |    11,342.5 ns |    272.90 ns |    142.73 ns |  0.52 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 4096 | Reversed           |    40,094.3 ns |    740.50 ns |    387.30 ns |  1.82 |    0.03 |    5 |         - |          NA |
| DotnetSort                   | 4096 | Reversed           |    41,911.5 ns |  5,181.84 ns |  2,710.20 ns |  1.91 |    0.12 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **PipeOrgan**          | **1,584,380.4 ns** |  **8,396.63 ns** |  **4,391.60 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 4096 | PipeOrgan          |    81,185.3 ns |  5,405.73 ns |  2,400.18 ns |  0.05 |    0.00 |    3 |         - |          NA |
| QuickSortMedian3             | 4096 | PipeOrgan          |    82,592.7 ns |  3,256.51 ns |  1,703.22 ns |  0.05 |    0.00 |    3 |         - |          NA |
| QuickSortMedian9             | 4096 | PipeOrgan          |    54,329.0 ns |  1,034.63 ns |    459.38 ns |  0.03 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort           | 4096 | PipeOrgan          |    40,069.3 ns |  1,089.34 ns |    569.75 ns |  0.03 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 4096 | PipeOrgan          |   207,973.9 ns |  1,496.24 ns |    533.57 ns |  0.13 |    0.00 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | PipeOrgan          |   118,768.6 ns |  3,119.33 ns |  1,385.00 ns |  0.07 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 4096 | PipeOrgan          |   173,935.9 ns |    503.08 ns |    223.37 ns |  0.11 |    0.00 |    4 |         - |          NA |
| IntroSort                    | 4096 | PipeOrgan          |    77,165.9 ns |  7,104.20 ns |  3,154.31 ns |  0.05 |    0.00 |    3 |         - |          NA |
| IntroSortDotnet              | 4096 | PipeOrgan          |    83,461.3 ns |    957.97 ns |    425.34 ns |  0.05 |    0.00 |    3 |         - |          NA |
| PDQSort                      | 4096 | PipeOrgan          |    42,620.5 ns |    706.22 ns |    369.37 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 4096 | PipeOrgan          |    73,903.1 ns |    879.93 ns |    390.70 ns |  0.05 |    0.00 |    3 |         - |          NA |
| Ipnsort                      | 4096 | PipeOrgan          |   105,933.2 ns |    404.88 ns |    144.38 ns |  0.07 |    0.00 |    3 |         - |          NA |
| StdSort                      | 4096 | PipeOrgan          |   108,134.7 ns |    734.25 ns |    384.03 ns |  0.07 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 4096 | PipeOrgan          |   107,417.5 ns |  1,124.95 ns |    588.37 ns |  0.07 |    0.00 |    3 |         - |          NA |
| DotnetSort                   | 4096 | PipeOrgan          |    90,999.1 ns |  3,892.49 ns |  1,728.29 ns |  0.06 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **ManyDuplicates**     |    **43,962.0 ns** |  **2,794.23 ns** |  **1,461.44 ns** |  **1.00** |    **0.04** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 4096 | ManyDuplicates     |    33,168.9 ns |  2,949.91 ns |  1,542.86 ns |  0.76 |    0.04 |    2 |         - |          NA |
| QuickSortMedian3             | 4096 | ManyDuplicates     |    52,601.1 ns |  1,452.72 ns |    759.80 ns |  1.20 |    0.04 |    2 |         - |          NA |
| QuickSortMedian9             | 4096 | ManyDuplicates     |    55,812.2 ns |  1,875.14 ns |    832.57 ns |  1.27 |    0.04 |    2 |         - |          NA |
| DualPivotQuickSort           | 4096 | ManyDuplicates     |    27,681.1 ns |    909.28 ns |    403.73 ns |  0.63 |    0.02 |    2 |         - |          NA |
| StableQuickSort              | 4096 | ManyDuplicates     |   109,379.6 ns |  1,346.11 ns |    597.68 ns |  2.49 |    0.08 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | ManyDuplicates     |    54,217.1 ns |  1,731.28 ns |    768.70 ns |  1.23 |    0.04 |    2 |         - |          NA |
| DestswapStableQuickSort      | 4096 | ManyDuplicates     |    53,713.4 ns |    649.04 ns |    288.18 ns |  1.22 |    0.04 |    2 |         - |          NA |
| IntroSort                    | 4096 | ManyDuplicates     |    49,907.1 ns |  1,311.18 ns |    685.77 ns |  1.14 |    0.04 |    2 |         - |          NA |
| IntroSortDotnet              | 4096 | ManyDuplicates     |    37,787.2 ns |    946.26 ns |    337.45 ns |  0.86 |    0.03 |    2 |         - |          NA |
| PDQSort                      | 4096 | ManyDuplicates     |    22,069.7 ns |    670.76 ns |    350.82 ns |  0.50 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless            | 4096 | ManyDuplicates     |    30,413.2 ns |    883.65 ns |    462.17 ns |  0.69 |    0.02 |    2 |         - |          NA |
| Ipnsort                      | 4096 | ManyDuplicates     |    59,956.1 ns |    516.51 ns |    270.14 ns |  1.37 |    0.04 |    2 |         - |          NA |
| StdSort                      | 4096 | ManyDuplicates     |    33,645.6 ns |    744.88 ns |    330.73 ns |  0.77 |    0.02 |    2 |         - |          NA |
| BlockQuickSort               | 4096 | ManyDuplicates     |    52,551.8 ns |    557.88 ns |    291.78 ns |  1.20 |    0.04 |    2 |         - |          NA |
| DotnetSort                   | 4096 | ManyDuplicates     |    36,407.0 ns |    951.52 ns |    497.66 ns |  0.83 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Random**             |   **472,181.1 ns** | **11,780.90 ns** |  **6,161.64 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Random             |   420,076.7 ns |  8,199.30 ns |  4,288.39 ns |  0.89 |    0.01 |    3 |         - |          NA |
| QuickSortMedian3             | 8192 | Random             |   463,316.0 ns |  2,648.50 ns |  1,175.95 ns |  0.98 |    0.01 |    3 |         - |          NA |
| QuickSortMedian9             | 8192 | Random             |   504,733.3 ns |  4,113.25 ns |  2,151.31 ns |  1.07 |    0.01 |    3 |         - |          NA |
| DualPivotQuickSort           | 8192 | Random             |   332,272.1 ns |  8,082.30 ns |  4,227.20 ns |  0.70 |    0.01 |    3 |         - |          NA |
| StableQuickSort              | 8192 | Random             | 1,310,039.4 ns |  5,791.37 ns |  3,029.00 ns |  2.77 |    0.03 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Random             | 1,048,742.4 ns |  1,247.41 ns |    553.86 ns |  2.22 |    0.03 |    5 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Random             |   869,768.6 ns |  4,938.60 ns |  2,582.99 ns |  1.84 |    0.02 |    4 |         - |          NA |
| IntroSort                    | 8192 | Random             |   394,020.3 ns |  2,726.49 ns |  1,426.01 ns |  0.83 |    0.01 |    3 |         - |          NA |
| IntroSortDotnet              | 8192 | Random             |   330,992.0 ns |  9,441.11 ns |  4,191.91 ns |  0.70 |    0.01 |    3 |         - |          NA |
| PDQSort                      | 8192 | Random             |   324,373.8 ns | 21,878.04 ns | 11,442.64 ns |  0.69 |    0.02 |    3 |         - |          NA |
| PDQSortBranchless            | 8192 | Random             |   137,691.1 ns |  1,523.70 ns |    676.53 ns |  0.29 |    0.00 |    1 |         - |          NA |
| Ipnsort                      | 8192 | Random             |   213,358.4 ns |  1,160.56 ns |    607.00 ns |  0.45 |    0.01 |    2 |         - |          NA |
| StdSort                      | 8192 | Random             |   134,381.2 ns |  1,134.01 ns |    593.11 ns |  0.28 |    0.00 |    1 |         - |          NA |
| BlockQuickSort               | 8192 | Random             |   150,255.6 ns |  1,938.17 ns |  1,013.70 ns |  0.32 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 8192 | Random             |   333,281.8 ns |  4,091.40 ns |  1,816.61 ns |  0.71 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **SingleElementMoved** |    **54,091.5 ns** |  **1,242.85 ns** |    **650.03 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 8192 | SingleElementMoved |    60,541.3 ns |  7,400.84 ns |  3,870.78 ns |  1.12 |    0.07 |    1 |         - |          NA |
| QuickSortMedian3             | 8192 | SingleElementMoved |    74,844.6 ns |  1,149.62 ns |    601.27 ns |  1.38 |    0.02 |    1 |         - |          NA |
| QuickSortMedian9             | 8192 | SingleElementMoved |    99,084.6 ns |  1,743.86 ns |    912.07 ns |  1.83 |    0.03 |    2 |         - |          NA |
| DualPivotQuickSort           | 8192 | SingleElementMoved |    49,351.1 ns |    986.51 ns |    515.96 ns |  0.91 |    0.01 |    1 |         - |          NA |
| StableQuickSort              | 8192 | SingleElementMoved |   450,933.3 ns |    949.37 ns |    496.54 ns |  8.34 |    0.10 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | SingleElementMoved |   266,783.2 ns |    884.05 ns |    392.52 ns |  4.93 |    0.06 |    4 |         - |          NA |
| DestswapStableQuickSort      | 8192 | SingleElementMoved |   213,193.0 ns |    994.04 ns |    441.36 ns |  3.94 |    0.05 |    3 |         - |          NA |
| IntroSort                    | 8192 | SingleElementMoved |    41,168.9 ns |  5,238.46 ns |  2,739.82 ns |  0.76 |    0.05 |    1 |         - |          NA |
| IntroSortDotnet              | 8192 | SingleElementMoved |    60,187.2 ns |    930.28 ns |    486.55 ns |  1.11 |    0.02 |    1 |         - |          NA |
| PDQSort                      | 8192 | SingleElementMoved |    44,504.2 ns |  1,826.09 ns |    810.79 ns |  0.82 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless            | 8192 | SingleElementMoved |    53,784.1 ns |    570.30 ns |    253.22 ns |  0.99 |    0.01 |    1 |         - |          NA |
| Ipnsort                      | 8192 | SingleElementMoved |   191,366.4 ns |    934.08 ns |    414.74 ns |  3.54 |    0.04 |    3 |         - |          NA |
| StdSort                      | 8192 | SingleElementMoved |    64,687.4 ns |    457.53 ns |    203.15 ns |  1.20 |    0.01 |    1 |         - |          NA |
| BlockQuickSort               | 8192 | SingleElementMoved |    94,950.9 ns |  1,065.13 ns |    472.92 ns |  1.76 |    0.02 |    2 |         - |          NA |
| DotnetSort                   | 8192 | SingleElementMoved |    60,546.7 ns |  3,856.31 ns |  2,016.92 ns |  1.12 |    0.04 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Sorted**             |    **42,418.4 ns** |  **1,821.92 ns** |    **952.90 ns** |  **1.00** |    **0.03** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Sorted             |    41,345.6 ns |  4,052.30 ns |  2,119.43 ns |  0.98 |    0.05 |    3 |         - |          NA |
| QuickSortMedian3             | 8192 | Sorted             |    54,087.9 ns |    314.56 ns |    164.52 ns |  1.28 |    0.03 |    3 |         - |          NA |
| QuickSortMedian9             | 8192 | Sorted             |    58,208.4 ns |  1,018.43 ns |    532.66 ns |  1.37 |    0.03 |    3 |         - |          NA |
| DualPivotQuickSort           | 8192 | Sorted             |    44,729.7 ns |  2,406.63 ns |  1,258.72 ns |  1.05 |    0.04 |    3 |         - |          NA |
| StableQuickSort              | 8192 | Sorted             |   492,027.8 ns |  1,287.09 ns |    673.17 ns | 11.60 |    0.24 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Sorted             |   230,759.2 ns |  6,044.47 ns |  3,161.37 ns |  5.44 |    0.13 |    5 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Sorted             |   204,883.2 ns |  5,225.93 ns |  2,733.26 ns |  4.83 |    0.12 |    5 |         - |          NA |
| IntroSort                    | 8192 | Sorted             |     7,923.7 ns |    636.65 ns |    282.67 ns |  0.19 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 8192 | Sorted             |    48,116.9 ns |  1,916.03 ns |    850.73 ns |  1.13 |    0.03 |    3 |         - |          NA |
| PDQSort                      | 8192 | Sorted             |    10,414.4 ns |    447.59 ns |    198.73 ns |  0.25 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | Sorted             |    10,395.7 ns |    386.60 ns |    171.65 ns |  0.25 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 8192 | Sorted             |     4,608.7 ns |     33.68 ns |     12.01 ns |  0.11 |    0.00 |    1 |         - |          NA |
| StdSort                      | 8192 | Sorted             |     8,991.8 ns |    202.57 ns |    105.95 ns |  0.21 |    0.01 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | Sorted             |    78,318.4 ns |    979.00 ns |    512.03 ns |  1.85 |    0.04 |    4 |         - |          NA |
| DotnetSort                   | 8192 | Sorted             |    44,835.5 ns |  4,447.47 ns |  2,326.11 ns |  1.06 |    0.06 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Reversed**           |    **47,391.2 ns** |  **1,109.50 ns** |    **580.29 ns** |  **1.00** |    **0.02** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Reversed           |    49,423.7 ns |  3,430.21 ns |  1,794.07 ns |  1.04 |    0.04 |    4 |         - |          NA |
| QuickSortMedian3             | 8192 | Reversed           |    58,046.1 ns |  4,779.73 ns |  2,499.89 ns |  1.22 |    0.05 |    4 |         - |          NA |
| QuickSortMedian9             | 8192 | Reversed           |    60,260.1 ns |  1,236.06 ns |    548.82 ns |  1.27 |    0.02 |    4 |         - |          NA |
| DualPivotQuickSort           | 8192 | Reversed           |    54,281.2 ns |  2,016.80 ns |  1,054.83 ns |  1.15 |    0.02 |    4 |         - |          NA |
| StableQuickSort              | 8192 | Reversed           |   448,353.0 ns |    775.93 ns |    405.83 ns |  9.46 |    0.11 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Reversed           |   258,895.8 ns | 10,725.16 ns |  5,609.47 ns |  5.46 |    0.13 |    6 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Reversed           |   312,721.3 ns |  5,344.42 ns |  2,795.24 ns |  6.60 |    0.10 |    7 |         - |          NA |
| IntroSort                    | 8192 | Reversed           |    27,954.8 ns |  2,118.13 ns |  1,107.82 ns |  0.59 |    0.02 |    3 |         - |          NA |
| IntroSortDotnet              | 8192 | Reversed           |    75,713.0 ns |    505.06 ns |    264.16 ns |  1.60 |    0.02 |    5 |         - |          NA |
| PDQSort                      | 8192 | Reversed           |    16,190.1 ns |    102.87 ns |     36.68 ns |  0.34 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | Reversed           |    25,343.2 ns |    480.91 ns |    251.53 ns |  0.53 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 8192 | Reversed           |     7,213.7 ns |    339.89 ns |    177.77 ns |  0.15 |    0.00 |    1 |         - |          NA |
| StdSort                      | 8192 | Reversed           |    22,601.0 ns |    497.39 ns |    260.14 ns |  0.48 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 8192 | Reversed           |    85,842.5 ns |    742.41 ns |    388.30 ns |  1.81 |    0.02 |    5 |         - |          NA |
| DotnetSort                   | 8192 | Reversed           |    96,691.0 ns |  7,290.18 ns |  3,812.90 ns |  2.04 |    0.08 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **PipeOrgan**          | **6,161,249.2 ns** |  **9,536.14 ns** |  **4,234.10 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| QuickSort3way                | 8192 | PipeOrgan          |   197,077.8 ns |  7,253.82 ns |  3,793.89 ns |  0.03 |    0.00 |    3 |         - |          NA |
| QuickSortMedian3             | 8192 | PipeOrgan          |   195,760.4 ns |  4,766.70 ns |  2,493.08 ns |  0.03 |    0.00 |    3 |         - |          NA |
| QuickSortMedian9             | 8192 | PipeOrgan          |   115,875.5 ns |  3,110.39 ns |  1,381.03 ns |  0.02 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort           | 8192 | PipeOrgan          |    85,907.2 ns |  1,850.13 ns |    967.66 ns |  0.01 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 8192 | PipeOrgan          |   452,299.1 ns |  1,684.31 ns |    600.64 ns |  0.07 |    0.00 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | PipeOrgan          |   258,774.3 ns | 11,423.91 ns |  5,974.93 ns |  0.04 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 8192 | PipeOrgan          |   374,254.4 ns |  1,269.16 ns |    563.51 ns |  0.06 |    0.00 |    4 |         - |          NA |
| IntroSort                    | 8192 | PipeOrgan          |   191,379.5 ns | 14,295.66 ns |  7,476.91 ns |  0.03 |    0.00 |    3 |         - |          NA |
| IntroSortDotnet              | 8192 | PipeOrgan          |   343,537.5 ns |  5,412.28 ns |  2,830.73 ns |  0.06 |    0.00 |    4 |         - |          NA |
| PDQSort                      | 8192 | PipeOrgan          |    92,405.8 ns |  2,226.94 ns |  1,164.73 ns |  0.01 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 8192 | PipeOrgan          |   164,526.8 ns |  1,548.33 ns |    809.81 ns |  0.03 |    0.00 |    3 |         - |          NA |
| Ipnsort                      | 8192 | PipeOrgan          |   236,625.5 ns |    582.59 ns |    304.71 ns |  0.04 |    0.00 |    3 |         - |          NA |
| StdSort                      | 8192 | PipeOrgan          |   282,978.1 ns |  2,840.19 ns |  1,261.06 ns |  0.05 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 8192 | PipeOrgan          |   225,448.2 ns |  2,068.31 ns |  1,081.76 ns |  0.04 |    0.00 |    3 |         - |          NA |
| DotnetSort                   | 8192 | PipeOrgan          |   359,669.1 ns |  9,480.60 ns |  4,958.54 ns |  0.06 |    0.00 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **ManyDuplicates**     |    **96,580.3 ns** |  **2,102.86 ns** |    **933.68 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 8192 | ManyDuplicates     |    64,546.7 ns |  8,240.23 ns |  4,309.80 ns |  0.67 |    0.04 |    2 |         - |          NA |
| QuickSortMedian3             | 8192 | ManyDuplicates     |   118,416.4 ns |  9,339.92 ns |  4,884.96 ns |  1.23 |    0.05 |    3 |         - |          NA |
| QuickSortMedian9             | 8192 | ManyDuplicates     |   121,567.8 ns |  1,925.26 ns |    854.83 ns |  1.26 |    0.01 |    3 |         - |          NA |
| DualPivotQuickSort           | 8192 | ManyDuplicates     |    60,564.5 ns |  4,013.19 ns |  2,098.98 ns |  0.63 |    0.02 |    2 |         - |          NA |
| StableQuickSort              | 8192 | ManyDuplicates     |   462,936.2 ns |  2,581.48 ns |  1,350.16 ns |  4.79 |    0.04 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | ManyDuplicates     |   241,750.3 ns |  8,286.83 ns |  3,679.40 ns |  2.50 |    0.04 |    4 |         - |          NA |
| DestswapStableQuickSort      | 8192 | ManyDuplicates     |   119,875.6 ns |  3,113.54 ns |  1,382.43 ns |  1.24 |    0.02 |    3 |         - |          NA |
| IntroSort                    | 8192 | ManyDuplicates     |   114,983.6 ns |  2,649.40 ns |  1,176.35 ns |  1.19 |    0.02 |    3 |         - |          NA |
| IntroSortDotnet              | 8192 | ManyDuplicates     |    81,722.3 ns |    732.53 ns |    325.25 ns |  0.85 |    0.01 |    3 |         - |          NA |
| PDQSort                      | 8192 | ManyDuplicates     |    44,456.0 ns |  1,163.58 ns |    516.64 ns |  0.46 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 8192 | ManyDuplicates     |    59,855.0 ns |  1,509.98 ns |    670.44 ns |  0.62 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 8192 | ManyDuplicates     |   118,378.8 ns |    623.79 ns |    276.97 ns |  1.23 |    0.01 |    3 |         - |          NA |
| StdSort                      | 8192 | ManyDuplicates     |    63,122.9 ns |  1,545.91 ns |    808.54 ns |  0.65 |    0.01 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | ManyDuplicates     |   102,215.2 ns |  1,311.85 ns |    686.12 ns |  1.06 |    0.01 |    3 |         - |          NA |
| DotnetSort                   | 8192 | ManyDuplicates     |    79,301.0 ns |  1,311.55 ns |    582.34 ns |  0.82 |    0.01 |    3 |         - |          NA |

### RadixHistogramPrecomputeBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.87GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method              | Size | RadixDigits | Mean         | Error       | StdDev      | Ratio | RatioSD | Allocated | Alloc Ratio |
| -------------------- |----- |------------ |-------------:|------------:|------------:|------:|--------:|----------:|------------:|
| **Lsd256_CountPerPass** | **1024** | **1**           |   **3,838.0 ns** |    **31.82 ns** |    **14.13 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Lsd256_Histogram    | 1024 | 1           |   5,122.8 ns |   393.15 ns |   205.62 ns |  1.33 |    0.05 |         - |          NA |
| Lsd10_CountPerPass  | 1024 | 1           |  16,893.0 ns |   102.03 ns |    45.30 ns |  4.40 |    0.02 |         - |          NA |
| Lsd10_Histogram     | 1024 | 1           |  16,675.8 ns |   175.22 ns |    77.80 ns |  4.34 |    0.02 |         - |          NA |
| Lsd10_Quotient      | 1024 | 1           |  16,603.2 ns | 1,917.38 ns | 1,002.83 ns |  4.33 |    0.25 |         - |          NA |
|      |             |              |             |             |       |         |           |             |
| **Lsd256_CountPerPass** | **1024** | **2**           |   **7,193.0 ns** |    **42.46 ns** |    **18.85 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Lsd256_Histogram    | 1024 | 2           |   6,778.7 ns |   263.10 ns |   137.61 ns |  0.94 |    0.02 |         - |          NA |
| Lsd10_CountPerPass  | 1024 | 2           |  27,151.7 ns |   210.19 ns |    93.32 ns |  3.77 |    0.02 |         - |          NA |
| Lsd10_Histogram     | 1024 | 2           |  27,568.0 ns |    96.82 ns |    42.99 ns |  3.83 |    0.01 |         - |          NA |
| Lsd10_Quotient      | 1024 | 2           |  29,795.1 ns | 1,276.74 ns |   566.88 ns |  4.14 |    0.07 |         - |          NA |
|      |             |              |             |             |       |         |           |             |
| **Lsd256_CountPerPass** | **1024** | **3**           |   **9,257.8 ns** |   **180.97 ns** |    **94.65 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| Lsd256_Histogram    | 1024 | 3           |   9,271.0 ns |   395.29 ns |   206.75 ns |  1.00 |    0.02 |         - |          NA |
| Lsd10_CountPerPass  | 1024 | 3           |  42,277.9 ns |   361.39 ns |   189.02 ns |  4.57 |    0.05 |         - |          NA |
| Lsd10_Histogram     | 1024 | 3           |  42,131.3 ns |   183.43 ns |    81.45 ns |  4.55 |    0.05 |         - |          NA |
| Lsd10_Quotient      | 1024 | 3           |  43,784.7 ns |   582.22 ns |   304.51 ns |  4.73 |    0.06 |         - |          NA |
|      |             |              |             |             |       |         |           |             |
| **Lsd256_CountPerPass** | **1024** | **4**           |  **12,050.5 ns** |   **242.50 ns** |   **126.83 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| Lsd256_Histogram    | 1024 | 4           |  11,001.4 ns |   383.49 ns |   170.27 ns |  0.91 |    0.02 |         - |          NA |
| Lsd10_CountPerPass  | 1024 | 4           |  53,130.6 ns |   688.93 ns |   360.32 ns |  4.41 |    0.05 |         - |          NA |
| Lsd10_Histogram     | 1024 | 4           |  52,501.0 ns |   234.16 ns |   103.97 ns |  4.36 |    0.04 |         - |          NA |
| Lsd10_Quotient      | 1024 | 4           | 123,573.3 ns | 1,721.48 ns |   900.37 ns | 10.26 |    0.12 |         - |          NA |
|      |             |              |             |             |       |         |           |             |
| **Lsd256_CountPerPass** | **8192** | **1**           |  **29,206.5 ns** |   **212.31 ns** |    **94.27 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Lsd256_Histogram    | 8192 | 1           |  37,334.9 ns | 1,977.39 ns | 1,034.21 ns |  1.28 |    0.03 |         - |          NA |
| Lsd10_CountPerPass  | 8192 | 1           | 133,408.5 ns |   696.33 ns |   364.19 ns |  4.57 |    0.02 |         - |          NA |
| Lsd10_Histogram     | 8192 | 1           | 130,871.3 ns | 1,492.18 ns |   780.44 ns |  4.48 |    0.03 |         - |          NA |
| Lsd10_Quotient      | 8192 | 1           | 301,315.6 ns | 5,717.25 ns | 2,538.49 ns | 10.32 |    0.09 |         - |          NA |
|      |             |              |             |             |       |         |           |             |
| **Lsd256_CountPerPass** | **8192** | **2**           |  **48,822.8 ns** |   **753.32 ns** |   **394.00 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| Lsd256_Histogram    | 8192 | 2           |  52,231.3 ns | 1,081.86 ns |   565.83 ns |  1.07 |    0.01 |         - |          NA |
| Lsd10_CountPerPass  | 8192 | 2           | 216,466.2 ns | 1,449.19 ns |   643.45 ns |  4.43 |    0.04 |         - |          NA |
| Lsd10_Histogram     | 8192 | 2           | 209,741.8 ns | 1,744.45 ns |   912.38 ns |  4.30 |    0.04 |         - |          NA |
| Lsd10_Quotient      | 8192 | 2           | 478,026.1 ns | 7,053.89 ns | 3,689.32 ns |  9.79 |    0.10 |         - |          NA |
|      |             |              |             |             |       |         |           |             |
| **Lsd256_CountPerPass** | **8192** | **3**           |  **68,264.0 ns** | **1,224.35 ns** |   **640.36 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| Lsd256_Histogram    | 8192 | 3           |  70,080.6 ns | 1,132.73 ns |   592.44 ns |  1.03 |    0.01 |         - |          NA |
| Lsd10_CountPerPass  | 8192 | 3           | 340,971.9 ns | 1,501.70 ns |   785.42 ns |  5.00 |    0.05 |         - |          NA |
| Lsd10_Histogram     | 8192 | 3           | 330,880.0 ns | 1,714.52 ns |   896.73 ns |  4.85 |    0.04 |         - |          NA |
| Lsd10_Quotient      | 8192 | 3           | 758,169.6 ns | 6,854.17 ns | 3,584.86 ns | 11.11 |    0.11 |         - |          NA |
|      |             |              |             |             |       |         |           |             |
| **Lsd256_CountPerPass** | **8192** | **4**           |  **89,348.9 ns** |   **573.11 ns** |   **254.46 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Lsd256_Histogram    | 8192 | 4           |  82,965.8 ns |   815.12 ns |   361.92 ns |  0.93 |    0.00 |         - |          NA |
| Lsd10_CountPerPass  | 8192 | 4           | 422,848.9 ns |   817.60 ns |   427.62 ns |  4.73 |    0.01 |         - |          NA |
| Lsd10_Histogram     | 8192 | 4           | 421,684.5 ns |   415.55 ns |   184.51 ns |  4.72 |    0.01 |         - |          NA |
| Lsd10_Quotient      | 8192 | 4           | 962,747.3 ns | 8,818.80 ns | 3,915.60 ns | 10.78 |    0.05 |         - |          NA |

### RadixIdentitySkipBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.87GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method        | Size | Stride | Mean         | Error      | StdDev      | Ratio | Allocated | Alloc Ratio |
| -------------- |----- |------- |-------------:|-----------:|------------:|------:|----------:|------------:|
| **Lsd4_NoSkip**   | **1024** | **1**      |  **18,510.0 ns** |   **266.9 ns** |   **139.62 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_Skip     | 1024 | 1      |  13,964.1 ns |   302.3 ns |   134.21 ns |  0.75 |         - |          NA |
| Lsd256_NoSkip | 1024 | 1      |   7,153.4 ns |   290.0 ns |   151.70 ns |  0.39 |         - |          NA |
| Lsd256_Skip   | 1024 | 1      |   7,158.1 ns |   356.6 ns |   186.51 ns |  0.39 |         - |          NA |
| Lsd10_NoSkip  | 1024 | 1      |  21,020.6 ns |   222.9 ns |    98.97 ns |  1.14 |         - |          NA |
| Lsd10_Skip    | 1024 | 1      |  20,883.3 ns |   185.0 ns |    82.14 ns |  1.13 |         - |          NA |
|      |        |              |            |             |       |           |             |
| **Lsd4_NoSkip**   | **1024** | **65536**  |  **42,071.0 ns** |   **689.4 ns** |   **360.58 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_Skip     | 1024 | 65536  |  23,140.9 ns |   177.9 ns |    93.04 ns |  0.55 |         - |          NA |
| Lsd256_NoSkip | 1024 | 65536  |  12,040.7 ns |   227.2 ns |   118.83 ns |  0.29 |         - |          NA |
| Lsd256_Skip   | 1024 | 65536  |   9,246.0 ns |   298.0 ns |   132.30 ns |  0.22 |         - |          NA |
| Lsd10_NoSkip  | 1024 | 65536  |  41,518.3 ns |   288.4 ns |   128.06 ns |  0.99 |         - |          NA |
| Lsd10_Skip    | 1024 | 65536  |  41,760.5 ns |   285.3 ns |   126.68 ns |  0.99 |         - |          NA |
|      |        |              |            |             |       |           |             |
| **Lsd4_NoSkip**   | **8192** | **1**      | **197,884.3 ns** | **1,353.4 ns** |   **707.87 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_Skip     | 8192 | 1      | 152,418.0 ns |   758.3 ns |   396.59 ns |  0.77 |         - |          NA |
| Lsd256_NoSkip | 8192 | 1      |  51,826.5 ns | 1,107.2 ns |   579.10 ns |  0.26 |         - |          NA |
| Lsd256_Skip   | 8192 | 1      |  51,419.2 ns |   923.1 ns |   409.88 ns |  0.26 |         - |          NA |
| Lsd10_NoSkip  | 8192 | 1      | 169,424.3 ns | 2,984.4 ns | 1,325.09 ns |  0.86 |         - |          NA |
| Lsd10_Skip    | 8192 | 1      | 170,585.8 ns | 1,391.6 ns |   727.82 ns |  0.86 |         - |          NA |
|      |        |              |            |             |       |           |             |
| **Lsd4_NoSkip**   | **8192** | **65536**  | **394,283.8 ns** | **1,522.3 ns** |   **675.90 ns** |  **1.00** |         **-** |          **NA** |
| Lsd4_Skip     | 8192 | 65536  | 223,950.8 ns | 3,190.5 ns | 1,416.61 ns |  0.57 |         - |          NA |
| Lsd256_NoSkip | 8192 | 65536  |  87,792.3 ns | 1,201.3 ns |   628.31 ns |  0.22 |         - |          NA |
| Lsd256_Skip   | 8192 | 65536  |  67,620.9 ns |   899.4 ns |   470.42 ns |  0.17 |         - |          NA |
| Lsd10_NoSkip  | 8192 | 65536  | 377,929.2 ns |   892.1 ns |   466.58 ns |  0.96 |         - |          NA |
| Lsd10_Skip    | 8192 | 65536  | 376,770.1 ns |   867.6 ns |   385.24 ns |  0.96 |         - |          NA |

### RadixLSD4KeyCacheBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.87GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method         | Size  | FullRange | Mean           | Error       | StdDev      | Ratio | RatioSD | Allocated | Alloc Ratio |
| --------------- |------ |---------- |---------------:|------------:|------------:|------:|--------:|----------:|------------:|
| **Lsd4_Recompute** | **1024**  | **False**     |    **14,018.5 ns** |    **138.7 ns** |    **72.54 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| Lsd4_KeyCache  | 1024  | False     |    19,088.3 ns |  1,378.3 ns |   720.90 ns |  1.36 |    0.05 |         - |          NA |
|       |           |                |             |             |       |         |           |             |
| **Lsd4_Recompute** | **1024**  | **True**      |    **41,614.1 ns** |    **243.8 ns** |   **108.25 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Lsd4_KeyCache  | 1024  | True      |    57,780.1 ns |    229.5 ns |   120.01 ns |  1.39 |    0.00 |         - |          NA |
|       |           |                |             |             |       |         |           |             |
| **Lsd4_Recompute** | **8192**  | **False**     |   **153,746.9 ns** |  **2,675.0 ns** | **1,399.06 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| Lsd4_KeyCache  | 8192  | False     |   207,836.5 ns |  1,083.7 ns |   566.78 ns |  1.35 |    0.01 |         - |          NA |
|       |           |                |             |             |       |         |           |             |
| **Lsd4_Recompute** | **8192**  | **True**      |   **333,268.2 ns** |  **1,427.3 ns** |   **633.72 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Lsd4_KeyCache  | 8192  | True      |   433,124.4 ns |  2,201.6 ns | 1,151.50 ns |  1.30 |    0.00 |         - |          NA |
|       |           |                |             |             |       |         |           |             |
| **Lsd4_Recompute** | **65536** | **False**     | **1,372,871.0 ns** |  **2,747.5 ns** | **1,219.91 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Lsd4_KeyCache  | 65536 | False     | 1,824,264.9 ns |  4,216.4 ns | 1,872.10 ns |  1.33 |    0.00 |         - |          NA |
|       |           |                |             |             |       |         |           |             |
| **Lsd4_Recompute** | **65536** | **True**      | **2,650,009.3 ns** |  **4,482.3 ns** | **1,990.16 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Lsd4_KeyCache  | 65536 | True      | 3,496,233.5 ns | 22,354.0 ns | 9,925.33 ns |  1.32 |    0.00 |         - |          NA |

### RadixRangeNormalizationBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.87GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method            | Size | StraddlesZero | Mean         | Error       | StdDev      | Ratio | RatioSD | Allocated | Alloc Ratio |
| ------------------ |----- |-------------- |-------------:|------------:|------------:|------:|--------:|----------:|------------:|
| **Lsd4_Xor**          | **1024** | **False**         |  **20,224.2 ns** |   **106.80 ns** |    **38.09 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Lsd4_Normalized   | 1024 | False         |  13,894.8 ns |   325.08 ns |   170.02 ns |  0.69 |    0.01 |         - |          NA |
| Lsd256_Xor        | 1024 | False         |   6,441.2 ns |   340.62 ns |   178.15 ns |  0.32 |    0.01 |         - |          NA |
| Lsd256_Normalized | 1024 | False         |   6,622.4 ns |   341.57 ns |   178.65 ns |  0.33 |    0.01 |         - |          NA |
| Lsd10_CopyBack    | 1024 | False         |  22,136.4 ns |    71.18 ns |    31.61 ns |  1.09 |    0.00 |         - |          NA |
| Lsd10_PingPong    | 1024 | False         |  21,636.4 ns |   419.77 ns |   219.55 ns |  1.07 |    0.01 |         - |          NA |
|      |               |              |             |             |       |         |           |             |
| **Lsd4_Xor**          | **1024** | **True**          |  **50,653.6 ns** |   **173.77 ns** |    **77.16 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Lsd4_Normalized   | 1024 | True          |  13,894.4 ns |   364.64 ns |   161.90 ns |  0.27 |    0.00 |         - |          NA |
| Lsd256_Xor        | 1024 | True          |  10,977.4 ns |   222.12 ns |   116.17 ns |  0.22 |    0.00 |         - |          NA |
| Lsd256_Normalized | 1024 | True          |   6,421.1 ns |    37.43 ns |    13.35 ns |  0.13 |    0.00 |         - |          NA |
| Lsd10_CopyBack    | 1024 | True          |  23,175.8 ns |   242.10 ns |   126.62 ns |  0.46 |    0.00 |         - |          NA |
| Lsd10_PingPong    | 1024 | True          |  24,392.1 ns | 5,443.92 ns | 2,847.28 ns |  0.48 |    0.05 |         - |          NA |
|      |               |              |             |             |       |         |           |             |
| **Lsd4_Xor**          | **8192** | **False**         | **197,349.4 ns** | **1,148.67 ns** |   **600.78 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Lsd4_Normalized   | 8192 | False         | 156,388.5 ns | 1,396.64 ns |   730.47 ns |  0.79 |    0.00 |         - |          NA |
| Lsd256_Xor        | 8192 | False         |  46,680.0 ns | 1,201.21 ns |   628.26 ns |  0.24 |    0.00 |         - |          NA |
| Lsd256_Normalized | 8192 | False         |  48,819.3 ns |   848.80 ns |   376.87 ns |  0.25 |    0.00 |         - |          NA |
| Lsd10_CopyBack    | 8192 | False         | 182,379.6 ns | 1,456.30 ns |   761.67 ns |  0.92 |    0.00 |         - |          NA |
| Lsd10_PingPong    | 8192 | False         | 168,352.6 ns | 2,685.34 ns | 1,404.49 ns |  0.85 |    0.01 |         - |          NA |
|      |               |              |             |             |       |         |           |             |
| **Lsd4_Xor**          | **8192** | **True**          | **418,481.5 ns** | **1,067.52 ns** |   **473.98 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| Lsd4_Normalized   | 8192 | True          | 152,841.7 ns | 2,689.58 ns | 1,194.19 ns |  0.37 |    0.00 |         - |          NA |
| Lsd256_Xor        | 8192 | True          |  80,512.3 ns |   555.95 ns |   198.26 ns |  0.19 |    0.00 |         - |          NA |
| Lsd256_Normalized | 8192 | True          |  48,764.5 ns | 1,131.08 ns |   502.21 ns |  0.12 |    0.00 |         - |          NA |
| Lsd10_CopyBack    | 8192 | True          | 180,234.6 ns | 1,374.19 ns |   610.15 ns |  0.43 |    0.00 |         - |          NA |
| Lsd10_PingPong    | 8192 | True          | 166,695.3 ns | 2,709.97 ns | 1,417.37 ns |  0.40 |    0.00 |         - |          NA |

### SelectionBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.87GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method              | Size | Pattern            | Mean           | Error        | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------------- |----- |------------------- |---------------:|-------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **SelectionSort**       | **256**  | **Random**             |    **24,451.3 ns** |    **174.31 ns** |    **77.40 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Random             |    18,562.1 ns |    178.49 ns |    79.25 ns |  0.76 |    0.00 |    1 |         - |          NA |
| CycleSort           | 256  | Random             |    72,277.8 ns |  1,210.73 ns |   633.23 ns |  2.96 |    0.03 |    4 |         - |          NA |
| PancakeSort         | 256  | Random             |    40,938.0 ns |    147.67 ns |    77.23 ns |  1.67 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **SelectionSort**       | **256**  | **SingleElementMoved** |    **24,463.3 ns** |     **64.63 ns** |    **23.05 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | SingleElementMoved |    23,193.2 ns |    392.14 ns |   174.11 ns |  0.95 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | SingleElementMoved |    57,112.0 ns |  1,560.79 ns |   816.32 ns |  2.33 |    0.03 |    2 |         - |          NA |
| PancakeSort         | 256  | SingleElementMoved |    21,281.3 ns |  2,299.15 ns | 1,202.50 ns |  0.87 |    0.05 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **SelectionSort**       | **256**  | **Sorted**             |    **24,803.2 ns** |    **310.66 ns** |   **162.48 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Sorted             |    12,344.2 ns |    215.92 ns |   112.93 ns |  0.50 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | Sorted             |    24,511.5 ns |    111.40 ns |    39.73 ns |  0.99 |    0.01 |    3 |         - |          NA |
| PancakeSort         | 256  | Sorted             |    16,981.7 ns |    147.47 ns |    77.13 ns |  0.68 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **SelectionSort**       | **256**  | **Reversed**           |    **23,286.0 ns** |  **2,116.82 ns** | **1,107.14 ns** |  **1.00** |    **0.06** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Reversed           |    18,255.2 ns |    188.53 ns |    98.60 ns |  0.79 |    0.03 |    1 |         - |          NA |
| CycleSort           | 256  | Reversed           |    49,782.4 ns |    389.47 ns |   203.70 ns |  2.14 |    0.09 |    3 |         - |          NA |
| PancakeSort         | 256  | Reversed           |    17,053.6 ns |    361.61 ns |   160.56 ns |  0.73 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **SelectionSort**       | **256**  | **PipeOrgan**          |    **23,691.3 ns** |    **834.13 ns** |   **436.27 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | PipeOrgan          |    21,332.8 ns |    192.85 ns |   100.87 ns |  0.90 |    0.02 |    1 |         - |          NA |
| CycleSort           | 256  | PipeOrgan          |    69,419.8 ns |  1,294.02 ns |   676.80 ns |  2.93 |    0.06 |    3 |         - |          NA |
| PancakeSort         | 256  | PipeOrgan          |    36,949.7 ns |    355.25 ns |   185.80 ns |  1.56 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **SelectionSort**       | **256**  | **ManyDuplicates**     |    **24,574.0 ns** |    **369.66 ns** |   **193.34 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | ManyDuplicates     |    18,402.9 ns |    439.03 ns |   229.62 ns |  0.75 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | ManyDuplicates     |    69,277.1 ns |  1,312.71 ns |   686.57 ns |  2.82 |    0.03 |    4 |         - |          NA |
| PancakeSort         | 256  | ManyDuplicates     |    38,643.4 ns |    417.08 ns |   185.19 ns |  1.57 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Random**             |   **405,571.7 ns** |  **1,966.93 ns** |   **701.43 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Random             |   301,430.6 ns |    937.47 ns |   490.31 ns |  0.74 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | Random             | 1,550,907.1 ns |  5,720.63 ns | 2,992.00 ns |  3.82 |    0.01 |    4 |         - |          NA |
| PancakeSort         | 1024 | Random             |   689,770.8 ns |  2,985.98 ns | 1,561.73 ns |  1.70 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **SelectionSort**       | **1024** | **SingleElementMoved** |   **375,466.0 ns** |    **890.98 ns** |   **395.60 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | SingleElementMoved |   354,561.9 ns |  1,428.87 ns |   634.43 ns |  0.94 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | SingleElementMoved |   890,548.7 ns |  8,963.15 ns | 4,687.90 ns |  2.37 |    0.01 |    2 |         - |          NA |
| PancakeSort         | 1024 | SingleElementMoved |   348,867.2 ns |  8,424.14 ns | 4,405.99 ns |  0.93 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Sorted**             |   **376,182.2 ns** |  **1,253.08 ns** |   **655.38 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Sorted             |   188,626.5 ns |    359.43 ns |   159.59 ns |  0.50 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | Sorted             |   375,298.1 ns |    861.88 ns |   382.68 ns |  1.00 |    0.00 |    3 |         - |          NA |
| PancakeSort         | 1024 | Sorted             |   253,023.7 ns |    839.43 ns |   439.04 ns |  0.67 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Reversed**           |   **342,763.5 ns** |  **6,902.84 ns** | **3,610.32 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Reversed           |   281,445.3 ns |  1,455.47 ns |   761.24 ns |  0.82 |    0.01 |    1 |         - |          NA |
| CycleSort           | 1024 | Reversed           |   759,460.4 ns |  3,026.82 ns | 1,343.93 ns |  2.22 |    0.02 |    3 |         - |          NA |
| PancakeSort         | 1024 | Reversed           |   258,305.1 ns | 14,861.80 ns | 7,773.01 ns |  0.75 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **SelectionSort**       | **1024** | **PipeOrgan**          |   **387,806.0 ns** |  **2,149.41 ns** | **1,124.18 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | PipeOrgan          |   346,125.1 ns |  1,117.30 ns |   584.37 ns |  0.89 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | PipeOrgan          | 1,184,196.3 ns | 13,895.91 ns | 6,169.87 ns |  3.05 |    0.02 |    3 |         - |          NA |
| PancakeSort         | 1024 | PipeOrgan          |   566,774.4 ns |  1,696.74 ns |   753.36 ns |  1.46 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **SelectionSort**       | **1024** | **ManyDuplicates**     |   **394,805.1 ns** |    **760.73 ns** |   **271.29 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | ManyDuplicates     |   294,292.8 ns |    390.39 ns |   173.33 ns |  0.75 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | ManyDuplicates     | 1,530,316.4 ns | 14,943.94 ns | 6,635.20 ns |  3.88 |    0.02 |    4 |         - |          NA |
| PancakeSort         | 1024 | ManyDuplicates     |   634,344.7 ns |  1,627.31 ns |   722.53 ns |  1.61 |    0.00 |    3 |         - |          NA |

### TreeBenchmark

```
BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.87GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  Job-GKDVVL : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                 | Size | Pattern            | Mean           | Error       | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------- |----- |------------------- |---------------:|------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **BalancedBinaryTreeSort** | **256**  | **Random**             |     **9,063.7 ns** | **1,048.11 ns** |   **548.18 ns** |  **2.62** |    **0.17** |    **3** |         **-** |          **NA** |
| BTreeSort              | 256  | Random             |     8,559.6 ns |   103.24 ns |    45.84 ns |  2.48 |    0.07 |    3 |         - |          NA |
| BPlusTreeSort          | 256  | Random             |     7,883.3 ns |   441.11 ns |   230.71 ns |  2.28 |    0.09 |    3 |         - |          NA |
| CartesianTreeSort      | 256  | Random             |     6,527.0 ns |   296.67 ns |   155.17 ns |  1.89 |    0.07 |    2 |         - |          NA |
| BinaryTreeSort         | 256  | Random             |     3,458.6 ns |   208.01 ns |   108.79 ns |  1.00 |    0.04 |    1 |         - |          NA |
| SplaySort              | 256  | Random             |    21,748.2 ns |   369.70 ns |   193.36 ns |  6.29 |    0.19 |    4 |         - |          NA |
| TreapSort              | 256  | Random             |     8,320.3 ns |   438.43 ns |   229.31 ns |  2.41 |    0.09 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **SingleElementMoved** |     **8,800.5 ns** |   **145.04 ns** |    **75.86 ns** |  **0.18** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 256  | SingleElementMoved |     6,569.9 ns |   347.79 ns |   181.90 ns |  0.13 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 256  | SingleElementMoved |     5,821.6 ns |   302.26 ns |   158.09 ns |  0.12 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 256  | SingleElementMoved |     2,459.8 ns |   350.06 ns |   183.09 ns |  0.05 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | SingleElementMoved |    49,349.0 ns |   738.57 ns |   386.29 ns |  1.00 |    0.01 |    5 |         - |          NA |
| SplaySort              | 256  | SingleElementMoved |     4,256.5 ns |   368.88 ns |   163.79 ns |  0.09 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | SingleElementMoved |     5,696.5 ns |   199.84 ns |   104.52 ns |  0.12 |    0.00 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Sorted**             |     **8,301.3 ns** |   **279.51 ns** |   **124.11 ns** |  **0.11** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 256  | Sorted             |     5,781.5 ns |   412.30 ns |   183.06 ns |  0.08 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 256  | Sorted             |     5,273.2 ns |   392.07 ns |   205.06 ns |  0.07 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 256  | Sorted             |     2,121.0 ns |   133.04 ns |    59.07 ns |  0.03 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | Sorted             |    75,969.5 ns |   339.55 ns |   150.76 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 256  | Sorted             |     3,833.3 ns |    18.01 ns |     6.42 ns |  0.05 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | Sorted             |     5,016.4 ns |   108.94 ns |    38.85 ns |  0.07 |    0.00 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Reversed**           |     **8,031.5 ns** |    **48.41 ns** |    **21.50 ns** |  **0.10** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 256  | Reversed           |     9,519.6 ns |   398.27 ns |   208.30 ns |  0.12 |    0.00 |    4 |         - |          NA |
| BPlusTreeSort          | 256  | Reversed           |     8,775.7 ns |   301.12 ns |   157.49 ns |  0.11 |    0.00 |    4 |         - |          NA |
| CartesianTreeSort      | 256  | Reversed           |     1,992.0 ns |     3.96 ns |     1.76 ns |  0.02 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | Reversed           |    79,758.5 ns |   530.56 ns |   235.57 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 256  | Reversed           |     3,781.1 ns |   282.57 ns |   147.79 ns |  0.05 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | Reversed           |     5,003.4 ns |   381.83 ns |   199.71 ns |  0.06 |    0.00 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **PipeOrgan**          |     **7,252.9 ns** |   **396.55 ns** |   **207.40 ns** |  **0.19** |    **0.01** |    **3** |         **-** |          **NA** |
| BTreeSort              | 256  | PipeOrgan          |     6,874.4 ns |   435.63 ns |   227.84 ns |  0.18 |    0.01 |    3 |         - |          NA |
| BPlusTreeSort          | 256  | PipeOrgan          |     6,301.2 ns |   229.16 ns |    81.72 ns |  0.17 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 256  | PipeOrgan          |     2,216.2 ns |    17.98 ns |     6.41 ns |  0.06 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | PipeOrgan          |    37,461.3 ns |   296.05 ns |   154.84 ns |  1.00 |    0.01 |    4 |         - |          NA |
| SplaySort              | 256  | PipeOrgan          |     4,296.0 ns |    15.48 ns |     5.52 ns |  0.11 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | PipeOrgan          |     7,717.3 ns |   163.04 ns |    85.28 ns |  0.21 |    0.00 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **ManyDuplicates**     |     **9,012.1 ns** |   **336.75 ns** |   **176.13 ns** |  **2.23** |    **0.09** |    **2** |         **-** |          **NA** |
| BTreeSort              | 256  | ManyDuplicates     |     7,592.6 ns |   199.12 ns |   104.14 ns |  1.88 |    0.07 |    2 |         - |          NA |
| BPlusTreeSort          | 256  | ManyDuplicates     |     7,079.3 ns |   364.31 ns |   161.76 ns |  1.75 |    0.07 |    2 |         - |          NA |
| CartesianTreeSort      | 256  | ManyDuplicates     |     7,418.7 ns |   411.87 ns |   215.42 ns |  1.83 |    0.08 |    2 |         - |          NA |
| BinaryTreeSort         | 256  | ManyDuplicates     |     4,053.6 ns |   303.16 ns |   158.56 ns |  1.00 |    0.05 |    1 |         - |          NA |
| SplaySort              | 256  | ManyDuplicates     |    20,596.9 ns |   270.97 ns |   141.72 ns |  5.09 |    0.19 |    3 |         - |          NA |
| TreapSort              | 256  | ManyDuplicates     |     7,652.5 ns |   581.70 ns |   258.28 ns |  1.89 |    0.09 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Random**             |    **43,771.8 ns** | **2,630.73 ns** | **1,168.06 ns** |  **2.22** |    **0.07** |    **2** |         **-** |          **NA** |
| BTreeSort              | 1024 | Random             |    40,964.2 ns | 2,192.52 ns |   973.49 ns |  2.07 |    0.06 |    2 |         - |          NA |
| BPlusTreeSort          | 1024 | Random             |    36,528.0 ns | 1,409.63 ns |   737.27 ns |  1.85 |    0.05 |    2 |         - |          NA |
| CartesianTreeSort      | 1024 | Random             |    32,166.7 ns | 1,661.56 ns |   869.03 ns |  1.63 |    0.05 |    2 |         - |          NA |
| BinaryTreeSort         | 1024 | Random             |    19,763.2 ns |   785.78 ns |   410.98 ns |  1.00 |    0.03 |    1 |         - |          NA |
| SplaySort              | 1024 | Random             |   122,563.9 ns | 4,750.24 ns | 2,484.47 ns |  6.20 |    0.17 |    3 |         - |          NA |
| TreapSort              | 1024 | Random             |    35,532.8 ns | 1,096.15 ns |   573.31 ns |  1.80 |    0.04 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **SingleElementMoved** |    **39,234.0 ns** |   **369.12 ns** |   **193.06 ns** |  **0.05** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 1024 | SingleElementMoved |    29,090.7 ns |   187.83 ns |    98.24 ns |  0.04 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 1024 | SingleElementMoved |    27,346.4 ns |   165.17 ns |    86.38 ns |  0.04 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 1024 | SingleElementMoved |     8,961.8 ns |   371.81 ns |   194.46 ns |  0.01 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | SingleElementMoved |   777,872.8 ns | 1,563.75 ns |   817.87 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | SingleElementMoved |    16,730.2 ns |   178.96 ns |    93.60 ns |  0.02 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | SingleElementMoved |    25,408.4 ns |   749.40 ns |   391.95 ns |  0.03 |    0.00 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Sorted**             |    **36,655.8 ns** |   **411.79 ns** |   **215.37 ns** | **0.030** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 1024 | Sorted             |    26,604.1 ns |   400.34 ns |   209.39 ns | 0.022 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 1024 | Sorted             |    24,524.4 ns |   348.98 ns |   182.52 ns | 0.020 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 1024 | Sorted             |     7,994.0 ns |     9.68 ns |     4.30 ns | 0.007 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | Sorted             | 1,204,770.7 ns |   736.90 ns |   327.19 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | Sorted             |    15,360.4 ns |   126.37 ns |    66.10 ns | 0.013 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | Sorted             |    23,583.7 ns | 2,434.05 ns | 1,273.05 ns | 0.020 |    0.00 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Reversed**           |    **36,170.7 ns** |   **611.02 ns** |   **319.57 ns** | **0.028** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 1024 | Reversed           |    41,420.1 ns | 1,417.67 ns |   741.47 ns | 0.032 |    0.00 |    4 |         - |          NA |
| BPlusTreeSort          | 1024 | Reversed           |    37,527.1 ns |   287.55 ns |   150.39 ns | 0.029 |    0.00 |    4 |         - |          NA |
| CartesianTreeSort      | 1024 | Reversed           |     7,635.3 ns |    61.71 ns |    27.40 ns | 0.006 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | Reversed           | 1,277,365.4 ns | 3,500.45 ns | 1,554.22 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | Reversed           |    14,703.8 ns |   328.07 ns |   145.67 ns | 0.012 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | Reversed           |    22,091.8 ns |   399.92 ns |   209.17 ns | 0.017 |    0.00 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **PipeOrgan**          |    **31,885.5 ns** | **1,094.91 ns** |   **572.66 ns** |  **0.05** |    **0.00** |    **3** |         **-** |          **NA** |
| BTreeSort              | 1024 | PipeOrgan          |    30,555.2 ns |   444.45 ns |   197.34 ns |  0.05 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 1024 | PipeOrgan          |    28,387.6 ns | 1,174.05 ns |   614.05 ns |  0.05 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 1024 | PipeOrgan          |     8,643.5 ns |   224.32 ns |   117.33 ns |  0.01 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | PipeOrgan          |   598,905.5 ns |   765.05 ns |   400.14 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | PipeOrgan          |    16,974.7 ns |   253.91 ns |   132.80 ns |  0.03 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | PipeOrgan          |    31,571.8 ns |   676.32 ns |   353.73 ns |  0.05 |    0.00 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **ManyDuplicates**     |    **45,217.4 ns** | **1,748.86 ns** |   **776.50 ns** |  **1.29** |    **0.02** |    **2** |         **-** |          **NA** |
| BTreeSort              | 1024 | ManyDuplicates     |    33,384.9 ns |   863.78 ns |   383.53 ns |  0.95 |    0.01 |    1 |         - |          NA |
| BPlusTreeSort          | 1024 | ManyDuplicates     |    31,885.1 ns |   375.12 ns |   166.55 ns |  0.91 |    0.01 |    1 |         - |          NA |
| CartesianTreeSort      | 1024 | ManyDuplicates     |    34,751.1 ns | 1,726.43 ns |   766.55 ns |  0.99 |    0.02 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | ManyDuplicates     |    35,092.1 ns |   332.09 ns |   147.45 ns |  1.00 |    0.01 |    1 |         - |          NA |
| SplaySort              | 1024 | ManyDuplicates     |   101,166.7 ns | 3,376.16 ns | 1,765.79 ns |  2.88 |    0.05 |    3 |         - |          NA |
| TreapSort              | 1024 | ManyDuplicates     |    34,533.9 ns | 1,119.49 ns |   497.06 ns |  0.98 |    0.01 |    1 |         - |          NA |

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
- [B Plus Tree Sort](./src/SortAlgorithm/Algorithms/Tree/BPlusTreeSort.cs)
- [B Tree Sort](./src/SortAlgorithm/Algorithms/Tree/BTreeSort.cs)
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
