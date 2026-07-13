# SortAlgorithms

This repository shows implementation for the Major Sort Algorithm.
Aim not to use LINQ or similar ease to use, but memory unefficient technique.

## Benchmark

<!-- BENCHMARK_START -->
<details>
<summary>Benchmark results (2026-07-13 06:51 UTC)</summary>

Workflow run: https://github.com/guitarrapc/SortAlgorithms/actions/runs/29228570923

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

| Method        | Size | Pattern            | Mean           | Error        | StdDev       | Median         | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------- |----- |------------------- |---------------:|-------------:|-------------:|---------------:|------:|--------:|-----:|----------:|------------:|
| **DropMergeSort** | **256**  | **Random**             |     **5,230.9 ns** |    **265.42 ns** |    **138.82 ns** |     **5,222.1 ns** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | Random             |     5,061.5 ns |    598.06 ns |    265.54 ns |     4,905.0 ns |  0.97 |    0.05 |    1 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **DropMergeSort** | **256**  | **SingleElementMoved** |       **602.8 ns** |      **3.16 ns** |      **1.40 ns** |       **602.4 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | SingleElementMoved |     8,618.8 ns |  1,420.00 ns |    742.69 ns |     8,961.3 ns | 14.30 |    1.16 |    2 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **DropMergeSort** | **256**  | **Sorted**             |       **888.7 ns** |    **725.98 ns** |    **379.70 ns** |       **884.1 ns** |  **1.19** |    **0.70** |    **1** |         **-** |          **NA** |
| PatienceSort  | 256  | Sorted             |     7,593.0 ns |    322.18 ns |    143.05 ns |     7,529.0 ns | 10.17 |    4.10 |    2 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **DropMergeSort** | **256**  | **Reversed**           |     **8,039.2 ns** |    **411.90 ns** |    **182.89 ns** |     **7,926.0 ns** |  **1.00** |    **0.03** |    **2** |         **-** |          **NA** |
| PatienceSort  | 256  | Reversed           |     1,846.7 ns |    495.13 ns |    258.96 ns |     1,663.5 ns |  0.23 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **DropMergeSort** | **256**  | **PipeOrgan**          |     **7,569.0 ns** |    **286.39 ns** |    **127.16 ns** |     **7,655.9 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| PatienceSort  | 256  | PipeOrgan          |     7,081.0 ns |  4,840.15 ns |  2,531.49 ns |     5,385.5 ns |  0.94 |    0.32 |    1 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **DropMergeSort** | **1024** | **Random**             |    **27,629.4 ns** |  **5,733.86 ns** |  **2,998.92 ns** |    **26,622.5 ns** |  **1.01** |    **0.14** |    **2** |         **-** |          **NA** |
| PatienceSort  | 1024 | Random             |    22,061.7 ns |     85.62 ns |     38.02 ns |    22,078.2 ns |  0.81 |    0.08 |    1 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **DropMergeSort** | **1024** | **SingleElementMoved** |     **2,187.2 ns** |     **16.99 ns** |      **6.06 ns** |     **2,184.9 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | SingleElementMoved |    43,046.4 ns |  4,956.68 ns |  2,592.44 ns |    42,662.5 ns | 19.68 |    1.12 |    2 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **DropMergeSort** | **1024** | **Sorted**             |     **1,956.0 ns** |      **4.41 ns** |      **1.96 ns** |     **1,956.2 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 1024 | Sorted             |    44,779.4 ns |  9,639.26 ns |  5,041.52 ns |    43,586.8 ns | 22.89 |    2.43 |    2 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **DropMergeSort** | **1024** | **Reversed**           |    **52,564.9 ns** |    **184.07 ns** |     **81.73 ns** |    **52,556.4 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PatienceSort  | 1024 | Reversed           |     6,044.6 ns |    475.70 ns |    248.80 ns |     6,016.0 ns |  0.11 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **DropMergeSort** | **1024** | **PipeOrgan**          |    **39,814.4 ns** |    **748.64 ns** |    **332.40 ns** |    **39,672.1 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| PatienceSort  | 1024 | PipeOrgan          |    26,138.1 ns |    825.82 ns |    431.92 ns |    26,063.2 ns |  0.66 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **DropMergeSort** | **8192** | **Random**             |   **537,167.4 ns** |  **2,100.22 ns** |    **932.51 ns** |   **537,019.6 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | Random             |   724,036.7 ns |  5,185.20 ns |  2,711.96 ns |   722,969.1 ns |  1.35 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **DropMergeSort** | **8192** | **SingleElementMoved** |    **17,241.4 ns** |     **89.85 ns** |     **32.04 ns** |    **17,237.7 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | SingleElementMoved |   739,122.5 ns | 16,265.41 ns |  8,507.12 ns |   743,282.4 ns | 42.87 |    0.47 |    2 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **DropMergeSort** | **8192** | **Sorted**             |    **15,547.6 ns** |    **134.09 ns** |     **47.82 ns** |    **15,564.6 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | Sorted             |   742,944.0 ns | 21,976.20 ns |  9,757.57 ns |   746,407.2 ns | 47.79 |    0.60 |    2 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **DropMergeSort** | **8192** | **Reversed**           | **1,126,502.0 ns** |  **9,701.29 ns** |  **4,307.43 ns** | **1,126,813.3 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| PatienceSort  | 8192 | Reversed           |    45,724.9 ns |    831.06 ns |    434.66 ns |    45,469.1 ns |  0.04 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |                |       |         |      |           |             |
| **DropMergeSort** | **8192** | **PipeOrgan**          |   **527,938.0 ns** | **26,297.57 ns** | **13,754.14 ns** |   **532,736.8 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| PatienceSort  | 8192 | PipeOrgan          |   576,343.6 ns |  7,446.31 ns |  3,894.56 ns |   574,407.0 ns |  1.09 |    0.03 |    1 |         - |          NA |

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

| Method     | Size | Pattern            | Mean         | Error       | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------- |----- |------------------- |-------------:|------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **StrandSort** | **256**  | **Random**             |   **6,278.7 ns** |   **157.70 ns** |    **70.02 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **256**  | **SingleElementMoved** |     **822.0 ns** |    **16.88 ns** |     **7.49 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **256**  | **Sorted**             |     **541.3 ns** |     **6.54 ns** |     **2.90 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **256**  | **Reversed**           |  **52,546.1 ns** |   **587.21 ns** |   **260.73 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **256**  | **PipeOrgan**          |  **27,705.4 ns** |   **335.09 ns** |   **175.26 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **1024** | **Random**             |  **56,532.5 ns** |   **588.04 ns** |   **307.55 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **1024** | **SingleElementMoved** |   **2,633.0 ns** |    **17.50 ns** |     **7.77 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **1024** | **Sorted**             |   **1,704.5 ns** |   **240.58 ns** |   **125.83 ns** |  **1.00** |    **0.10** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **1024** | **Reversed**           | **764,202.0 ns** |   **329.13 ns** |   **117.37 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |         |      |           |             |
| **StrandSort** | **1024** | **PipeOrgan**          | **395,963.0 ns** | **2,987.45 ns** | **1,326.45 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |

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
| **CountingSort**        | **256**  | **Random**             |   **1,790.3 ns** |    **334.16 ns** |   **174.77 ns** |  **1.72** |    **0.16** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | Random             |   1,039.7 ns |      9.42 ns |     4.93 ns |  1.00 |    0.01 |    1 |         - |          NA |
| PigeonSort          | 256  | Random             |   1,514.6 ns |      1.45 ns |     0.64 ns |  1.46 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | Random             |   1,237.9 ns |    284.68 ns |   148.89 ns |  1.19 |    0.14 |    2 |         - |          NA |
| BucketSort          | 256  | Random             |   2,981.6 ns |     35.22 ns |    12.56 ns |  2.87 |    0.02 |    3 |         - |          NA |
| BucketSortInteger   | 256  | Random             |   3,008.0 ns |    377.02 ns |   197.19 ns |  2.89 |    0.18 |    3 |         - |          NA |
| FlashSort           | 256  | Random             |   5,853.8 ns |  1,306.63 ns |   683.39 ns |  5.63 |    0.62 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | Random             |   5,887.7 ns |    449.77 ns |   235.24 ns |  5.66 |    0.21 |    5 |         - |          NA |
| RadixLSD256Sort     | 256  | Random             |   2,740.6 ns |    256.63 ns |    91.52 ns |  2.64 |    0.08 |    3 |         - |          NA |
| RadixLSD10Sort      | 256  | Random             |   4,185.1 ns |     58.49 ns |    20.86 ns |  4.03 |    0.03 |    4 |         - |          NA |
| RadixMSD4Sort       | 256  | Random             |  12,093.9 ns |    347.37 ns |   181.68 ns | 11.63 |    0.17 |    6 |         - |          NA |
| RadixMSD10Sort      | 256  | Random             |  13,871.4 ns |    159.30 ns |    83.31 ns | 13.34 |    0.10 |    6 |         - |          NA |
| AmericanFlagSort    | 256  | Random             |   5,619.1 ns |    581.65 ns |   304.22 ns |  5.40 |    0.28 |    5 |         - |          NA |
| SpreadSort          | 256  | Random             |   1,679.1 ns |     13.93 ns |     4.97 ns |  1.62 |    0.01 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **256**  | **SingleElementMoved** |   **1,589.2 ns** |      **4.56 ns** |     **1.62 ns** |  **1.65** |    **0.01** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | SingleElementMoved |     962.7 ns |      9.21 ns |     3.28 ns |  1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 256  | SingleElementMoved |   1,497.3 ns |     14.64 ns |     5.22 ns |  1.56 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | SingleElementMoved |   1,077.2 ns |    305.30 ns |   159.68 ns |  1.12 |    0.16 |    1 |         - |          NA |
| BucketSort          | 256  | SingleElementMoved |   2,514.1 ns |     83.21 ns |    29.67 ns |  2.61 |    0.03 |    4 |         - |          NA |
| BucketSortInteger   | 256  | SingleElementMoved |   2,022.7 ns |     17.27 ns |     6.16 ns |  2.10 |    0.01 |    3 |         - |          NA |
| FlashSort           | 256  | SingleElementMoved |   5,079.9 ns |    305.07 ns |   159.56 ns |  5.28 |    0.16 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | SingleElementMoved |   5,888.4 ns |    361.34 ns |   188.99 ns |  6.12 |    0.19 |    5 |         - |          NA |
| RadixLSD256Sort     | 256  | SingleElementMoved |   2,726.8 ns |     43.57 ns |    15.54 ns |  2.83 |    0.02 |    4 |         - |          NA |
| RadixLSD10Sort      | 256  | SingleElementMoved |   4,080.5 ns |    259.23 ns |   135.58 ns |  4.24 |    0.13 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | SingleElementMoved |  11,409.5 ns |    472.41 ns |   247.08 ns | 11.85 |    0.25 |    6 |         - |          NA |
| RadixMSD10Sort      | 256  | SingleElementMoved |  13,245.5 ns |    361.04 ns |   128.75 ns | 13.76 |    0.13 |    6 |         - |          NA |
| AmericanFlagSort    | 256  | SingleElementMoved |   4,336.5 ns |     11.09 ns |     4.92 ns |  4.50 |    0.02 |    5 |         - |          NA |
| SpreadSort          | 256  | SingleElementMoved |   1,116.1 ns |     25.15 ns |    11.17 ns |  1.16 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **256**  | **Sorted**             |   **1,546.0 ns** |      **9.69 ns** |     **4.30 ns** |  **1.40** |    **0.18** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 256  | Sorted             |   1,122.4 ns |    266.64 ns |   139.46 ns |  1.02 |    0.18 |    3 |         - |          NA |
| PigeonSort          | 256  | Sorted             |   1,587.5 ns |    459.49 ns |   240.32 ns |  1.44 |    0.28 |    4 |         - |          NA |
| PigeonSortInteger   | 256  | Sorted             |     971.9 ns |     38.97 ns |    17.30 ns |  0.88 |    0.11 |    2 |         - |          NA |
| BucketSort          | 256  | Sorted             |   2,536.3 ns |    367.88 ns |   192.41 ns |  2.29 |    0.34 |    6 |         - |          NA |
| BucketSortInteger   | 256  | Sorted             |   1,879.1 ns |     11.18 ns |     5.85 ns |  1.70 |    0.22 |    5 |         - |          NA |
| FlashSort           | 256  | Sorted             |   4,938.7 ns |    420.85 ns |   220.11 ns |  4.47 |    0.61 |    7 |         - |          NA |
| RadixLSD4Sort       | 256  | Sorted             |   7,194.9 ns |     84.23 ns |    37.40 ns |  6.51 |    0.84 |    8 |         - |          NA |
| RadixLSD256Sort     | 256  | Sorted             |   2,784.1 ns |    248.57 ns |   110.37 ns |  2.52 |    0.34 |    6 |         - |          NA |
| RadixLSD10Sort      | 256  | Sorted             |   5,286.6 ns |    265.87 ns |   139.05 ns |  4.78 |    0.63 |    7 |         - |          NA |
| RadixMSD4Sort       | 256  | Sorted             |  11,257.7 ns |    451.16 ns |   235.96 ns | 10.18 |    1.33 |    9 |         - |          NA |
| RadixMSD10Sort      | 256  | Sorted             |  13,204.2 ns |    209.18 ns |   109.41 ns | 11.94 |    1.54 |    9 |         - |          NA |
| AmericanFlagSort    | 256  | Sorted             |   4,547.8 ns |    538.65 ns |   281.73 ns |  4.11 |    0.58 |    7 |         - |          NA |
| SpreadSort          | 256  | Sorted             |     340.2 ns |      0.81 ns |     0.36 ns |  0.31 |    0.04 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **256**  | **Reversed**           |   **1,591.7 ns** |    **205.25 ns** |    **91.13 ns** |  **1.73** |    **0.09** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | Reversed           |     920.9 ns |      8.49 ns |     3.77 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PigeonSort          | 256  | Reversed           |   1,525.4 ns |     17.72 ns |     6.32 ns |  1.66 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | Reversed           |     887.4 ns |     12.28 ns |     4.38 ns |  0.96 |    0.01 |    2 |         - |          NA |
| BucketSort          | 256  | Reversed           |   3,196.2 ns |     60.80 ns |    21.68 ns |  3.47 |    0.03 |    4 |         - |          NA |
| BucketSortInteger   | 256  | Reversed           |   3,197.4 ns |    392.42 ns |   205.24 ns |  3.47 |    0.21 |    4 |         - |          NA |
| FlashSort           | 256  | Reversed           |   4,334.5 ns |    298.65 ns |   156.20 ns |  4.71 |    0.16 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | Reversed           |   5,876.1 ns |    405.22 ns |   211.94 ns |  6.38 |    0.22 |    6 |         - |          NA |
| RadixLSD256Sort     | 256  | Reversed           |   2,668.2 ns |     87.52 ns |    38.86 ns |  2.90 |    0.04 |    4 |         - |          NA |
| RadixLSD10Sort      | 256  | Reversed           |   4,130.3 ns |    222.62 ns |    98.85 ns |  4.49 |    0.10 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | Reversed           |  12,068.9 ns |    312.51 ns |   163.45 ns | 13.11 |    0.17 |    7 |         - |          NA |
| RadixMSD10Sort      | 256  | Reversed           |  13,791.5 ns |    306.36 ns |   160.23 ns | 14.98 |    0.17 |    7 |         - |          NA |
| AmericanFlagSort    | 256  | Reversed           |   5,598.2 ns |    522.55 ns |   273.31 ns |  6.08 |    0.28 |    6 |         - |          NA |
| SpreadSort          | 256  | Reversed           |     525.5 ns |      4.27 ns |     2.23 ns |  0.57 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **256**  | **PipeOrgan**          |   **1,510.9 ns** |     **28.84 ns** |    **12.81 ns** |  **1.75** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | PipeOrgan          |     862.0 ns |      3.38 ns |     1.77 ns |  1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 256  | PipeOrgan          |   1,497.5 ns |     11.38 ns |     5.05 ns |  1.74 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | PipeOrgan          |   1,251.4 ns |    272.72 ns |   142.64 ns |  1.45 |    0.16 |    2 |         - |          NA |
| BucketSort          | 256  | PipeOrgan          |   2,947.9 ns |     39.06 ns |    17.34 ns |  3.42 |    0.02 |    4 |         - |          NA |
| BucketSortInteger   | 256  | PipeOrgan          |   2,525.8 ns |      9.53 ns |     4.23 ns |  2.93 |    0.01 |    4 |         - |          NA |
| FlashSort           | 256  | PipeOrgan          |   4,534.6 ns |     24.73 ns |     8.82 ns |  5.26 |    0.01 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | PipeOrgan          |   5,847.5 ns |     16.48 ns |     8.62 ns |  6.78 |    0.02 |    6 |         - |          NA |
| RadixLSD256Sort     | 256  | PipeOrgan          |   2,846.3 ns |    409.68 ns |   214.27 ns |  3.30 |    0.23 |    4 |         - |          NA |
| RadixLSD10Sort      | 256  | PipeOrgan          |   4,030.0 ns |     29.86 ns |    10.65 ns |  4.68 |    0.01 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | PipeOrgan          |  13,056.8 ns |    325.95 ns |   144.72 ns | 15.15 |    0.16 |    7 |         - |          NA |
| RadixMSD10Sort      | 256  | PipeOrgan          |  13,961.4 ns |    473.55 ns |   247.68 ns | 16.20 |    0.27 |    7 |         - |          NA |
| AmericanFlagSort    | 256  | PipeOrgan          |   5,920.6 ns |     66.04 ns |    29.32 ns |  6.87 |    0.03 |    6 |         - |          NA |
| SpreadSort          | 256  | PipeOrgan          |   1,688.2 ns |     25.90 ns |    13.55 ns |  1.96 |    0.02 |    3 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **1024** | **Random**             |   **6,502.0 ns** |     **27.60 ns** |    **14.44 ns** |  **1.61** |    **0.09** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Random             |   4,052.8 ns |    469.95 ns |   245.80 ns |  1.00 |    0.08 |    1 |         - |          NA |
| PigeonSort          | 1024 | Random             |   5,399.8 ns |      9.23 ns |     3.29 ns |  1.34 |    0.08 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | Random             |   3,608.5 ns |    393.53 ns |   205.82 ns |  0.89 |    0.07 |    1 |         - |          NA |
| BucketSort          | 1024 | Random             |  14,588.0 ns |    297.50 ns |   132.09 ns |  3.61 |    0.21 |    5 |         - |          NA |
| BucketSortInteger   | 1024 | Random             |  14,862.3 ns |    290.37 ns |   151.87 ns |  3.68 |    0.21 |    5 |         - |          NA |
| FlashSort           | 1024 | Random             |  17,747.1 ns |    317.53 ns |   166.08 ns |  4.39 |    0.26 |    5 |         - |          NA |
| RadixLSD4Sort       | 1024 | Random             |  24,607.6 ns |    319.80 ns |   141.99 ns |  6.09 |    0.35 |    5 |         - |          NA |
| RadixLSD256Sort     | 1024 | Random             |  10,368.7 ns |    375.40 ns |   196.34 ns |  2.57 |    0.15 |    4 |         - |          NA |
| RadixLSD10Sort      | 1024 | Random             |  20,637.7 ns |    208.48 ns |    92.57 ns |  5.11 |    0.30 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | Random             |  49,075.4 ns |    341.43 ns |   178.58 ns | 12.15 |    0.70 |    6 |         - |          NA |
| RadixMSD10Sort      | 1024 | Random             |  50,475.1 ns |    136.46 ns |    48.66 ns | 12.49 |    0.72 |    6 |         - |          NA |
| AmericanFlagSort    | 1024 | Random             |  24,239.1 ns |    737.67 ns |   385.81 ns |  6.00 |    0.36 |    5 |         - |          NA |
| SpreadSort          | 1024 | Random             |   9,388.1 ns |    343.24 ns |   152.40 ns |  2.32 |    0.14 |    4 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **1024** | **SingleElementMoved** |   **6,019.7 ns** |    **550.14 ns** |   **244.26 ns** |  **1.57** |    **0.11** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | SingleElementMoved |   3,848.5 ns |    423.88 ns |   221.70 ns |  1.00 |    0.08 |    1 |         - |          NA |
| PigeonSort          | 1024 | SingleElementMoved |   5,681.5 ns |    259.85 ns |   135.91 ns |  1.48 |    0.09 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | SingleElementMoved |   3,458.8 ns |      8.16 ns |     2.91 ns |  0.90 |    0.05 |    1 |         - |          NA |
| BucketSort          | 1024 | SingleElementMoved |   9,593.5 ns |    347.33 ns |   181.66 ns |  2.50 |    0.14 |    3 |         - |          NA |
| BucketSortInteger   | 1024 | SingleElementMoved |   7,489.8 ns |     89.47 ns |    31.91 ns |  1.95 |    0.11 |    2 |         - |          NA |
| FlashSort           | 1024 | SingleElementMoved |  19,681.8 ns |    432.15 ns |   191.88 ns |  5.13 |    0.29 |    4 |         - |          NA |
| RadixLSD4Sort       | 1024 | SingleElementMoved |  29,223.3 ns |    397.61 ns |   207.96 ns |  7.62 |    0.42 |    5 |         - |          NA |
| RadixLSD256Sort     | 1024 | SingleElementMoved |   9,850.3 ns |    757.70 ns |   396.29 ns |  2.57 |    0.17 |    3 |         - |          NA |
| RadixLSD10Sort      | 1024 | SingleElementMoved |  20,878.6 ns |    223.70 ns |    99.32 ns |  5.44 |    0.30 |    4 |         - |          NA |
| RadixMSD4Sort       | 1024 | SingleElementMoved |  44,115.5 ns |    330.57 ns |   172.89 ns | 11.50 |    0.64 |    6 |         - |          NA |
| RadixMSD10Sort      | 1024 | SingleElementMoved |  48,252.9 ns |    205.85 ns |    91.40 ns | 12.58 |    0.69 |    6 |         - |          NA |
| AmericanFlagSort    | 1024 | SingleElementMoved |  17,320.9 ns |    362.48 ns |   189.58 ns |  4.51 |    0.25 |    4 |         - |          NA |
| SpreadSort          | 1024 | SingleElementMoved |   6,892.1 ns |    375.08 ns |   196.17 ns |  1.80 |    0.11 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **1024** | **Sorted**             |   **6,692.6 ns** |    **207.28 ns** |   **108.41 ns** |  **1.88** |    **0.12** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Sorted             |   3,568.4 ns |    478.95 ns |   250.50 ns |  1.00 |    0.09 |    2 |         - |          NA |
| PigeonSort          | 1024 | Sorted             |   5,232.9 ns |    465.35 ns |   243.39 ns |  1.47 |    0.11 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | Sorted             |   3,413.8 ns |     12.41 ns |     4.42 ns |  0.96 |    0.06 |    2 |         - |          NA |
| BucketSort          | 1024 | Sorted             |   9,414.4 ns |    352.90 ns |   184.57 ns |  2.65 |    0.17 |    5 |         - |          NA |
| BucketSortInteger   | 1024 | Sorted             |   7,320.3 ns |     99.63 ns |    35.53 ns |  2.06 |    0.13 |    4 |         - |          NA |
| FlashSort           | 1024 | Sorted             |  19,007.9 ns |    411.71 ns |   146.82 ns |  5.35 |    0.34 |    6 |         - |          NA |
| RadixLSD4Sort       | 1024 | Sorted             |  24,471.5 ns |    345.36 ns |   180.63 ns |  6.89 |    0.43 |    6 |         - |          NA |
| RadixLSD256Sort     | 1024 | Sorted             |   9,965.2 ns |    372.62 ns |   194.89 ns |  2.80 |    0.18 |    5 |         - |          NA |
| RadixLSD10Sort      | 1024 | Sorted             |  20,870.2 ns |    180.17 ns |    80.00 ns |  5.87 |    0.37 |    6 |         - |          NA |
| RadixMSD4Sort       | 1024 | Sorted             |  43,373.3 ns |    137.91 ns |    72.13 ns | 12.20 |    0.77 |    7 |         - |          NA |
| RadixMSD10Sort      | 1024 | Sorted             |  47,829.9 ns |    332.22 ns |   147.51 ns | 13.46 |    0.85 |    7 |         - |          NA |
| AmericanFlagSort    | 1024 | Sorted             |  16,824.3 ns |    300.09 ns |   133.24 ns |  4.73 |    0.30 |    6 |         - |          NA |
| SpreadSort          | 1024 | Sorted             |     696.8 ns |      4.66 ns |     2.07 ns |  0.20 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **1024** | **Reversed**           |   **6,026.8 ns** |    **457.46 ns** |   **239.26 ns** |  **1.74** |    **0.07** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Reversed           |   3,459.3 ns |      9.12 ns |     3.25 ns |  1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 1024 | Reversed           |   5,242.0 ns |    392.48 ns |   205.28 ns |  1.52 |    0.06 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | Reversed           |   3,106.7 ns |      1.94 ns |     0.69 ns |  0.90 |    0.00 |    1 |         - |          NA |
| BucketSort          | 1024 | Reversed           |  16,190.6 ns |    123.49 ns |    64.59 ns |  4.68 |    0.02 |    4 |         - |          NA |
| BucketSortInteger   | 1024 | Reversed           |  17,156.8 ns |    312.83 ns |   138.90 ns |  4.96 |    0.04 |    4 |         - |          NA |
| FlashSort           | 1024 | Reversed           |  16,697.6 ns |    283.61 ns |   148.33 ns |  4.83 |    0.04 |    4 |         - |          NA |
| RadixLSD4Sort       | 1024 | Reversed           |  24,213.2 ns |    187.47 ns |    98.05 ns |  7.00 |    0.03 |    5 |         - |          NA |
| RadixLSD256Sort     | 1024 | Reversed           |  10,209.3 ns |    296.30 ns |   154.97 ns |  2.95 |    0.04 |    3 |         - |          NA |
| RadixLSD10Sort      | 1024 | Reversed           |  20,914.2 ns |    528.24 ns |   276.28 ns |  6.05 |    0.08 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | Reversed           |  47,043.1 ns |    172.30 ns |    90.12 ns | 13.60 |    0.03 |    6 |         - |          NA |
| RadixMSD10Sort      | 1024 | Reversed           |  49,774.8 ns |    316.93 ns |   165.76 ns | 14.39 |    0.05 |    6 |         - |          NA |
| AmericanFlagSort    | 1024 | Reversed           |  24,036.4 ns |    743.31 ns |   388.77 ns |  6.95 |    0.11 |    5 |         - |          NA |
| SpreadSort          | 1024 | Reversed           |   5,816.7 ns |    346.60 ns |   181.28 ns |  1.68 |    0.05 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **1024** | **PipeOrgan**          |   **5,748.3 ns** |    **252.95 ns** |   **112.31 ns** |  **1.75** |    **0.03** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | PipeOrgan          |   3,287.8 ns |     51.55 ns |    22.89 ns |  1.00 |    0.01 |    1 |         - |          NA |
| PigeonSort          | 1024 | PipeOrgan          |   5,178.5 ns |    258.93 ns |   135.43 ns |  1.58 |    0.04 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | PipeOrgan          |   3,421.7 ns |    468.56 ns |   245.06 ns |  1.04 |    0.07 |    1 |         - |          NA |
| BucketSort          | 1024 | PipeOrgan          |  14,229.7 ns |    349.74 ns |   182.92 ns |  4.33 |    0.06 |    5 |         - |          NA |
| BucketSortInteger   | 1024 | PipeOrgan          |  12,323.6 ns |    174.75 ns |    91.40 ns |  3.75 |    0.04 |    5 |         - |          NA |
| FlashSort           | 1024 | PipeOrgan          |  17,684.1 ns |    226.62 ns |   100.62 ns |  5.38 |    0.04 |    6 |         - |          NA |
| RadixLSD4Sort       | 1024 | PipeOrgan          |  25,851.4 ns |    336.52 ns |   176.00 ns |  7.86 |    0.07 |    7 |         - |          NA |
| RadixLSD256Sort     | 1024 | PipeOrgan          |  10,167.5 ns |    447.19 ns |   233.89 ns |  3.09 |    0.07 |    4 |         - |          NA |
| RadixLSD10Sort      | 1024 | PipeOrgan          |  20,522.2 ns |    190.93 ns |    99.86 ns |  6.24 |    0.05 |    6 |         - |          NA |
| RadixMSD4Sort       | 1024 | PipeOrgan          |  50,869.6 ns |    425.34 ns |   222.46 ns | 15.47 |    0.12 |    8 |         - |          NA |
| RadixMSD10Sort      | 1024 | PipeOrgan          |  49,610.2 ns |    175.31 ns |    77.84 ns | 15.09 |    0.10 |    8 |         - |          NA |
| AmericanFlagSort    | 1024 | PipeOrgan          |  26,198.5 ns |    580.17 ns |   257.60 ns |  7.97 |    0.09 |    7 |         - |          NA |
| SpreadSort          | 1024 | PipeOrgan          |   7,613.0 ns |    220.96 ns |   115.57 ns |  2.32 |    0.04 |    3 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **8192** | **Random**             |  **53,977.1 ns** |  **1,036.93 ns** |   **542.34 ns** |  **1.56** |    **0.03** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Random             |  34,653.0 ns |  1,453.36 ns |   760.14 ns |  1.00 |    0.03 |    1 |         - |          NA |
| PigeonSort          | 8192 | Random             |  47,552.2 ns |    894.75 ns |   467.97 ns |  1.37 |    0.03 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | Random             |  29,947.5 ns |  1,292.27 ns |   675.88 ns |  0.86 |    0.03 |    1 |         - |          NA |
| BucketSort          | 8192 | Random             | 222,043.0 ns |  8,801.75 ns | 4,603.49 ns |  6.41 |    0.18 |    5 |         - |          NA |
| BucketSortInteger   | 8192 | Random             | 246,413.2 ns |  1,495.42 ns |   782.14 ns |  7.11 |    0.15 |    5 |         - |          NA |
| FlashSort           | 8192 | Random             | 154,781.4 ns |  1,257.93 ns |   558.53 ns |  4.47 |    0.09 |    4 |         - |          NA |
| RadixLSD4Sort       | 8192 | Random             | 233,687.2 ns |  1,026.10 ns |   536.67 ns |  6.75 |    0.14 |    5 |         - |          NA |
| RadixLSD256Sort     | 8192 | Random             |  61,161.8 ns | 10,536.67 ns | 5,510.88 ns |  1.77 |    0.15 |    2 |         - |          NA |
| RadixLSD10Sort      | 8192 | Random             | 175,260.7 ns |  1,203.11 ns |   534.19 ns |  5.06 |    0.10 |    4 |         - |          NA |
| RadixMSD4Sort       | 8192 | Random             | 464,319.5 ns |  4,186.17 ns | 2,189.45 ns | 13.40 |    0.28 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | Random             | 431,042.1 ns |  2,074.04 ns | 1,084.76 ns | 12.44 |    0.25 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | Random             | 293,564.6 ns |  1,473.91 ns |   770.88 ns |  8.48 |    0.17 |    5 |         - |          NA |
| SpreadSort          | 8192 | Random             |  86,180.8 ns |  2,505.59 ns | 1,310.47 ns |  2.49 |    0.06 |    3 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **8192** | **SingleElementMoved** |  **49,040.7 ns** |  **1,010.27 ns** |   **528.39 ns** |  **1.62** |    **0.02** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | SingleElementMoved |  30,181.7 ns |    643.63 ns |   285.77 ns |  1.00 |    0.01 |    1 |         - |          NA |
| PigeonSort          | 8192 | SingleElementMoved |  43,792.3 ns |    761.49 ns |   338.10 ns |  1.45 |    0.02 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | SingleElementMoved |  27,630.0 ns |  1,352.65 ns |   600.59 ns |  0.92 |    0.02 |    1 |         - |          NA |
| BucketSort          | 8192 | SingleElementMoved |  67,577.8 ns |    338.75 ns |   177.17 ns |  2.24 |    0.02 |    3 |         - |          NA |
| BucketSortInteger   | 8192 | SingleElementMoved |  49,588.7 ns |  1,114.92 ns |   583.12 ns |  1.64 |    0.02 |    2 |         - |          NA |
| FlashSort           | 8192 | SingleElementMoved | 158,100.1 ns |    961.82 ns |   427.05 ns |  5.24 |    0.05 |    4 |         - |          NA |
| RadixLSD4Sort       | 8192 | SingleElementMoved | 237,345.2 ns |    654.66 ns |   290.67 ns |  7.86 |    0.07 |    5 |         - |          NA |
| RadixLSD256Sort     | 8192 | SingleElementMoved |  54,435.4 ns |  2,524.10 ns | 1,120.71 ns |  1.80 |    0.04 |    2 |         - |          NA |
| RadixLSD10Sort      | 8192 | SingleElementMoved | 162,607.4 ns |  1,499.28 ns |   665.69 ns |  5.39 |    0.05 |    4 |         - |          NA |
| RadixMSD4Sort       | 8192 | SingleElementMoved | 345,278.0 ns |  1,001.28 ns |   523.69 ns | 11.44 |    0.10 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | SingleElementMoved | 379,335.3 ns |    702.41 ns |   311.87 ns | 12.57 |    0.11 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | SingleElementMoved | 146,748.5 ns |    380.04 ns |   168.74 ns |  4.86 |    0.04 |    4 |         - |          NA |
| SpreadSort          | 8192 | SingleElementMoved |  46,666.8 ns |  1,155.11 ns |   604.14 ns |  1.55 |    0.02 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **8192** | **Sorted**             |  **46,397.7 ns** |    **832.00 ns** |   **369.41 ns** |  **1.64** |    **0.05** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Sorted             |  28,244.4 ns |  1,914.61 ns | 1,001.38 ns |  1.00 |    0.05 |    2 |         - |          NA |
| PigeonSort          | 8192 | Sorted             |  41,607.5 ns |  1,069.77 ns |   559.51 ns |  1.47 |    0.05 |    3 |         - |          NA |
| PigeonSortInteger   | 8192 | Sorted             |  28,117.7 ns |  1,906.57 ns |   997.17 ns |  1.00 |    0.05 |    2 |         - |          NA |
| BucketSort          | 8192 | Sorted             |  66,084.3 ns |  1,329.29 ns |   590.21 ns |  2.34 |    0.08 |    4 |         - |          NA |
| BucketSortInteger   | 8192 | Sorted             |  47,602.0 ns |    888.56 ns |   464.73 ns |  1.69 |    0.06 |    3 |         - |          NA |
| FlashSort           | 8192 | Sorted             | 152,739.2 ns |  1,237.71 ns |   549.55 ns |  5.41 |    0.18 |    5 |         - |          NA |
| RadixLSD4Sort       | 8192 | Sorted             | 233,885.3 ns |    972.21 ns |   508.48 ns |  8.29 |    0.27 |    6 |         - |          NA |
| RadixLSD256Sort     | 8192 | Sorted             |  53,110.8 ns |    584.55 ns |   305.73 ns |  1.88 |    0.06 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | Sorted             | 166,276.8 ns |  4,860.67 ns | 2,542.22 ns |  5.89 |    0.21 |    5 |         - |          NA |
| RadixMSD4Sort       | 8192 | Sorted             | 346,662.0 ns |  1,427.09 ns |   633.63 ns | 12.29 |    0.40 |    7 |         - |          NA |
| RadixMSD10Sort      | 8192 | Sorted             | 378,173.3 ns |  1,944.28 ns | 1,016.89 ns | 13.40 |    0.44 |    7 |         - |          NA |
| AmericanFlagSort    | 8192 | Sorted             | 144,928.7 ns |  1,002.09 ns |   524.11 ns |  5.14 |    0.17 |    5 |         - |          NA |
| SpreadSort          | 8192 | Sorted             |   5,216.3 ns |    253.67 ns |   112.63 ns |  0.18 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **8192** | **Reversed**           |  **45,465.3 ns** |    **517.42 ns** |   **270.62 ns** |  **1.60** |    **0.04** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Reversed           |  28,516.0 ns |  1,283.02 ns |   671.04 ns |  1.00 |    0.03 |    1 |         - |          NA |
| PigeonSort          | 8192 | Reversed           |  42,140.8 ns |    488.93 ns |   255.72 ns |  1.48 |    0.03 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | Reversed           |  25,180.0 ns |    613.49 ns |   320.87 ns |  0.88 |    0.02 |    1 |         - |          NA |
| BucketSort          | 8192 | Reversed           | 259,404.0 ns |  3,523.81 ns | 1,843.02 ns |  9.10 |    0.21 |    5 |         - |          NA |
| BucketSortInteger   | 8192 | Reversed           | 308,224.8 ns |  2,191.44 ns |   973.01 ns | 10.81 |    0.24 |    5 |         - |          NA |
| FlashSort           | 8192 | Reversed           | 133,614.2 ns |    876.23 ns |   458.28 ns |  4.69 |    0.10 |    3 |         - |          NA |
| RadixLSD4Sort       | 8192 | Reversed           | 236,269.2 ns |    506.99 ns |   225.11 ns |  8.29 |    0.18 |    5 |         - |          NA |
| RadixLSD256Sort     | 8192 | Reversed           |  52,927.2 ns |    898.27 ns |   398.84 ns |  1.86 |    0.04 |    2 |         - |          NA |
| RadixLSD10Sort      | 8192 | Reversed           | 169,847.5 ns |  1,526.94 ns |   798.62 ns |  5.96 |    0.13 |    4 |         - |          NA |
| RadixMSD4Sort       | 8192 | Reversed           | 371,496.7 ns |  2,598.86 ns | 1,153.91 ns | 13.03 |    0.29 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | Reversed           | 392,429.4 ns |  1,745.70 ns |   775.10 ns | 13.77 |    0.30 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | Reversed           | 208,380.3 ns |  2,953.90 ns | 1,544.95 ns |  7.31 |    0.17 |    5 |         - |          NA |
| SpreadSort          | 8192 | Reversed           |  61,611.4 ns |    577.52 ns |   302.05 ns |  2.16 |    0.05 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **CountingSort**        | **8192** | **PipeOrgan**          |  **45,203.7 ns** |  **1,079.71 ns** |   **564.71 ns** |  **1.69** |    **0.03** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | PipeOrgan          |  26,822.6 ns |    684.48 ns |   357.99 ns |  1.00 |    0.02 |    1 |         - |          NA |
| PigeonSort          | 8192 | PipeOrgan          |  41,858.9 ns |  1,296.70 ns |   678.20 ns |  1.56 |    0.03 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | PipeOrgan          |  27,160.5 ns |    655.53 ns |   291.06 ns |  1.01 |    0.02 |    1 |         - |          NA |
| BucketSort          | 8192 | PipeOrgan          | 207,842.2 ns |  2,549.61 ns | 1,132.04 ns |  7.75 |    0.11 |    4 |         - |          NA |
| BucketSortInteger   | 8192 | PipeOrgan          | 191,644.9 ns |    908.64 ns |   403.44 ns |  7.15 |    0.09 |    4 |         - |          NA |
| FlashSort           | 8192 | PipeOrgan          | 138,976.1 ns |    406.75 ns |   180.60 ns |  5.18 |    0.07 |    4 |         - |          NA |
| RadixLSD4Sort       | 8192 | PipeOrgan          | 236,365.1 ns |  2,210.15 ns |   981.32 ns |  8.81 |    0.12 |    4 |         - |          NA |
| RadixLSD256Sort     | 8192 | PipeOrgan          |  79,020.7 ns |  1,015.67 ns |   450.96 ns |  2.95 |    0.04 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | PipeOrgan          | 159,906.5 ns |  2,614.13 ns | 1,160.69 ns |  5.96 |    0.09 |    4 |         - |          NA |
| RadixMSD4Sort       | 8192 | PipeOrgan          | 402,333.1 ns |  1,336.53 ns |   699.03 ns | 15.00 |    0.19 |    5 |         - |          NA |
| RadixMSD10Sort      | 8192 | PipeOrgan          | 394,746.5 ns |  1,016.07 ns |   531.42 ns | 14.72 |    0.19 |    5 |         - |          NA |
| AmericanFlagSort    | 8192 | PipeOrgan          | 230,377.7 ns |  2,342.45 ns | 1,225.15 ns |  8.59 |    0.12 |    4 |         - |          NA |
| SpreadSort          | 8192 | PipeOrgan          |  81,379.3 ns |    598.38 ns |   312.97 ns |  3.03 |    0.04 |    3 |         - |          NA |

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

| Method             | Size | Pattern            | Mean         | Error        | StdDev       | Ratio  | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------- |----- |------------------- |-------------:|-------------:|-------------:|-------:|--------:|-----:|----------:|------------:|
| **BubbleSort**         | **256**  | **Random**             |  **47,662.9 ns** | **33,560.41 ns** | **17,552.75 ns** |   **1.12** |    **0.55** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Random             |  16,411.7 ns |    311.62 ns |    162.98 ns |   0.39 |    0.13 |    2 |         - |          NA |
| OddEvenSort        | 256  | Random             |  26,013.2 ns |    503.20 ns |    263.18 ns |   0.61 |    0.20 |    3 |         - |          NA |
| CombSort           | 256  | Random             |   3,370.5 ns |     71.66 ns |     25.56 ns |   0.08 |    0.03 |    1 |         - |          NA |
| CircleSort         | 256  | Random             |  17,736.1 ns |    233.04 ns |    103.47 ns |   0.42 |    0.14 |    2 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **BubbleSort**         | **256**  | **SingleElementMoved** |     **408.0 ns** |     **16.67 ns** |      **5.95 ns** |   **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 256  | SingleElementMoved |     307.5 ns |      2.13 ns |      0.94 ns |   0.75 |    0.01 |    1 |         - |          NA |
| OddEvenSort        | 256  | SingleElementMoved |  15,623.7 ns |    100.58 ns |     52.61 ns |  38.30 |    0.52 |    4 |         - |          NA |
| CombSort           | 256  | SingleElementMoved |   2,806.7 ns |      7.75 ns |      2.76 ns |   6.88 |    0.09 |    3 |         - |          NA |
| CircleSort         | 256  | SingleElementMoved |  15,150.4 ns |    242.30 ns |    126.73 ns |  37.14 |    0.57 |    4 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **BubbleSort**         | **256**  | **Sorted**             |     **277.5 ns** |     **67.63 ns** |     **35.37 ns** |   **1.01** |    **0.17** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Sorted             |     189.4 ns |     51.27 ns |     26.81 ns |   0.69 |    0.12 |    1 |         - |          NA |
| OddEvenSort        | 256  | Sorted             |     212.6 ns |      0.96 ns |      0.43 ns |   0.78 |    0.09 |    2 |         - |          NA |
| CombSort           | 256  | Sorted             |   2,481.4 ns |      1.29 ns |      0.46 ns |   9.07 |    1.07 |    4 |         - |          NA |
| CircleSort         | 256  | Sorted             |   2,082.5 ns |      4.76 ns |      1.70 ns |   7.61 |    0.90 |    4 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **BubbleSort**         | **256**  | **Reversed**           |  **27,574.7 ns** |    **595.45 ns** |    **264.38 ns** |   **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Reversed           |  25,181.8 ns |    364.82 ns |    190.81 ns |   0.91 |    0.01 |    3 |         - |          NA |
| OddEvenSort        | 256  | Reversed           |  24,363.0 ns |    229.14 ns |    119.85 ns |   0.88 |    0.01 |    3 |         - |          NA |
| CombSort           | 256  | Reversed           |   3,122.1 ns |     66.43 ns |     23.69 ns |   0.11 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | Reversed           |   4,388.4 ns |    461.50 ns |    241.37 ns |   0.16 |    0.01 |    2 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **BubbleSort**         | **256**  | **PipeOrgan**          |  **26,479.8 ns** |    **601.86 ns** |    **314.79 ns** |   **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | PipeOrgan          |  16,965.1 ns |    227.05 ns |    100.81 ns |   0.64 |    0.01 |    2 |         - |          NA |
| OddEvenSort        | 256  | PipeOrgan          |  25,556.5 ns |    265.14 ns |    117.73 ns |   0.97 |    0.01 |    3 |         - |          NA |
| CombSort           | 256  | PipeOrgan          |   3,052.2 ns |    112.93 ns |     40.27 ns |   0.12 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | PipeOrgan          |  19,551.5 ns |    228.78 ns |    119.66 ns |   0.74 |    0.01 |    2 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Random**             | **600,729.0 ns** |  **5,712.64 ns** |  **2,987.82 ns** |   **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Random             | 312,329.2 ns |  1,015.97 ns |    451.10 ns |   0.52 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | Random             | 530,947.2 ns |  4,754.46 ns |  2,486.67 ns |   0.88 |    0.01 |    4 |         - |          NA |
| CombSort           | 1024 | Random             |  31,745.4 ns |    719.98 ns |    319.68 ns |   0.05 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | Random             |  97,704.9 ns |    965.70 ns |    428.78 ns |   0.16 |    0.00 |    2 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **BubbleSort**         | **1024** | **SingleElementMoved** |   **1,534.0 ns** |      **3.45 ns** |      **1.23 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | SingleElementMoved |   1,147.7 ns |      2.97 ns |      1.32 ns |   0.75 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 1024 | SingleElementMoved | 222,860.0 ns |  1,075.08 ns |    562.29 ns | 145.28 |    0.36 |    5 |         - |          NA |
| CombSort           | 1024 | SingleElementMoved |  14,974.8 ns |     89.04 ns |     39.53 ns |   9.76 |    0.03 |    3 |         - |          NA |
| CircleSort         | 1024 | SingleElementMoved | 105,557.7 ns |    544.32 ns |    241.68 ns |  68.81 |    0.16 |    4 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Sorted**             |     **955.5 ns** |      **0.69 ns** |      **0.36 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Sorted             |     642.3 ns |      1.14 ns |      0.51 ns |   0.67 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 1024 | Sorted             |     806.9 ns |      2.01 ns |      0.89 ns |   0.84 |    0.00 |    2 |         - |          NA |
| CombSort           | 1024 | Sorted             |  13,134.6 ns |    180.05 ns |     94.17 ns |  13.75 |    0.09 |    4 |         - |          NA |
| CircleSort         | 1024 | Sorted             |   9,325.8 ns |    475.09 ns |    248.48 ns |   9.76 |    0.25 |    3 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Reversed**           | **414,416.8 ns** |  **1,245.70 ns** |    **553.10 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Reversed           | 386,214.3 ns |  1,371.79 ns |    717.47 ns |   0.93 |    0.00 |    2 |         - |          NA |
| OddEvenSort        | 1024 | Reversed           | 368,019.4 ns |    877.84 ns |    389.77 ns |   0.89 |    0.00 |    2 |         - |          NA |
| CombSort           | 1024 | Reversed           |  16,495.3 ns |    138.88 ns |     61.66 ns |   0.04 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | Reversed           |  18,602.3 ns |    116.43 ns |     60.90 ns |   0.04 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **BubbleSort**         | **1024** | **PipeOrgan**          | **383,325.9 ns** |    **371.82 ns** |    **194.47 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | PipeOrgan          | 253,124.5 ns |    411.36 ns |    215.15 ns |   0.66 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | PipeOrgan          | 359,594.5 ns |  1,053.99 ns |    551.26 ns |   0.94 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | PipeOrgan          |  16,893.8 ns |    164.02 ns |     72.83 ns |   0.04 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | PipeOrgan          | 103,149.3 ns |  1,432.68 ns |    636.12 ns |   0.27 |    0.00 |    2 |         - |          NA |

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

| Method           | Size | Pattern            | Mean         | Error      | StdDev     | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------- |----- |------------------- |-------------:|-----------:|-----------:|------:|--------:|-----:|----------:|------------:|
| **HeapSort**         | **256**  | **Random**             |     **3.826 μs** |  **0.3207 μs** |  **0.1424 μs** |  **1.00** |    **0.05** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | Random             |     3.400 μs |  0.0868 μs |  0.0310 μs |  0.89 |    0.03 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | Random             |     4.153 μs |  0.3546 μs |  0.1855 μs |  1.09 |    0.06 |    1 |         - |          NA |
| BottomupHeapSort | 256  | Random             |     4.170 μs |  0.3970 μs |  0.2076 μs |  1.09 |    0.06 |    1 |         - |          NA |
| WeakHeapSort     | 256  | Random             |     9.064 μs |  0.3455 μs |  0.1807 μs |  2.37 |    0.09 |    4 |         - |          NA |
| SmoothSort       | 256  | Random             |     5.334 μs |  0.4570 μs |  0.2390 μs |  1.40 |    0.07 |    2 |         - |          NA |
| TournamentSort   | 256  | Random             |     7.546 μs |  0.2715 μs |  0.1420 μs |  1.97 |    0.07 |    3 |         - |          NA |
|      |                    |              |            |            |       |         |      |           |             |
| **HeapSort**         | **256**  | **SingleElementMoved** |     **3.910 μs** |  **0.3564 μs** |  **0.1583 μs** |  **1.00** |    **0.05** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 256  | SingleElementMoved |     3.413 μs |  0.0942 μs |  0.0336 μs |  0.87 |    0.03 |    2 |         - |          NA |
| TernaryHeapSort  | 256  | SingleElementMoved |     4.358 μs |  0.3410 μs |  0.1783 μs |  1.12 |    0.06 |    2 |         - |          NA |
| BottomupHeapSort | 256  | SingleElementMoved |     4.065 μs |  0.0991 μs |  0.0440 μs |  1.04 |    0.04 |    2 |         - |          NA |
| WeakHeapSort     | 256  | SingleElementMoved |     7.998 μs |  0.3512 μs |  0.1837 μs |  2.05 |    0.09 |    4 |         - |          NA |
| SmoothSort       | 256  | SingleElementMoved |     1.725 μs |  0.0235 μs |  0.0104 μs |  0.44 |    0.02 |    1 |         - |          NA |
| TournamentSort   | 256  | SingleElementMoved |     5.401 μs |  0.3555 μs |  0.1579 μs |  1.38 |    0.06 |    3 |         - |          NA |
|      |                    |              |            |            |       |         |      |           |             |
| **HeapSort**         | **256**  | **Sorted**             |     **3.975 μs** |  **0.4493 μs** |  **0.2350 μs** |  **1.00** |    **0.08** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 256  | Sorted             |     3.671 μs |  0.5266 μs |  0.2754 μs |  0.93 |    0.08 |    2 |         - |          NA |
| TernaryHeapSort  | 256  | Sorted             |     4.242 μs |  0.3771 μs |  0.1972 μs |  1.07 |    0.07 |    2 |         - |          NA |
| BottomupHeapSort | 256  | Sorted             |     4.045 μs |  0.1196 μs |  0.0531 μs |  1.02 |    0.06 |    2 |         - |          NA |
| WeakHeapSort     | 256  | Sorted             |     8.017 μs |  0.4922 μs |  0.2574 μs |  2.02 |    0.13 |    3 |         - |          NA |
| SmoothSort       | 256  | Sorted             |     1.441 μs |  0.4112 μs |  0.2151 μs |  0.36 |    0.05 |    1 |         - |          NA |
| TournamentSort   | 256  | Sorted             |     3.290 μs |  0.3414 μs |  0.1516 μs |  0.83 |    0.06 |    2 |         - |          NA |
|      |                    |              |            |            |       |         |      |           |             |
| **HeapSort**         | **256**  | **Reversed**           |     **3.819 μs** |  **0.1083 μs** |  **0.0481 μs** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | Reversed           |     3.420 μs |  0.0646 μs |  0.0287 μs |  0.90 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | Reversed           |     4.385 μs |  0.4527 μs |  0.2368 μs |  1.15 |    0.06 |    1 |         - |          NA |
| BottomupHeapSort | 256  | Reversed           |     4.316 μs |  0.3101 μs |  0.1622 μs |  1.13 |    0.04 |    1 |         - |          NA |
| WeakHeapSort     | 256  | Reversed           |     8.806 μs |  0.3594 μs |  0.1880 μs |  2.31 |    0.05 |    2 |         - |          NA |
| SmoothSort       | 256  | Reversed           |     4.937 μs |  0.5616 μs |  0.2003 μs |  1.29 |    0.05 |    1 |         - |          NA |
| TournamentSort   | 256  | Reversed           |     4.869 μs |  0.2989 μs |  0.1327 μs |  1.28 |    0.04 |    1 |         - |          NA |
|      |                    |              |            |            |       |         |      |           |             |
| **HeapSort**         | **256**  | **PipeOrgan**          |     **3.138 μs** |  **0.3697 μs** |  **0.1933 μs** |  **1.00** |    **0.08** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | PipeOrgan          |     3.084 μs |  0.1108 μs |  0.0395 μs |  0.99 |    0.06 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | PipeOrgan          |     3.824 μs |  0.2635 μs |  0.1378 μs |  1.22 |    0.08 |    2 |         - |          NA |
| BottomupHeapSort | 256  | PipeOrgan          |     4.210 μs |  0.4270 μs |  0.2233 μs |  1.35 |    0.10 |    2 |         - |          NA |
| WeakHeapSort     | 256  | PipeOrgan          |     8.363 μs |  0.3130 μs |  0.1637 μs |  2.67 |    0.16 |    4 |         - |          NA |
| SmoothSort       | 256  | PipeOrgan          |     5.639 μs |  0.4639 μs |  0.2426 μs |  1.80 |    0.12 |    3 |         - |          NA |
| TournamentSort   | 256  | PipeOrgan          |     7.441 μs |  0.4669 μs |  0.2442 μs |  2.38 |    0.15 |    4 |         - |          NA |
|      |                    |              |            |            |       |         |      |           |             |
| **HeapSort**         | **1024** | **Random**             |    **19.678 μs** |  **0.4325 μs** |  **0.1920 μs** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Random             |    17.919 μs |  0.1214 μs |  0.0433 μs |  0.91 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | Random             |    20.635 μs |  0.4488 μs |  0.2347 μs |  1.05 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | Random             |    19.362 μs |  0.6365 μs |  0.2826 μs |  0.98 |    0.02 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | Random             |    48.427 μs |  0.4787 μs |  0.2504 μs |  2.46 |    0.03 |    3 |         - |          NA |
| SmoothSort       | 1024 | Random             |    26.984 μs |  0.3044 μs |  0.1592 μs |  1.37 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 1024 | Random             |    56.100 μs | 31.0976 μs | 16.2646 μs |  2.85 |    0.78 |    3 |         - |          NA |
|      |                    |              |            |            |       |         |      |           |             |
| **HeapSort**         | **1024** | **SingleElementMoved** |    **22.114 μs** |  **0.6103 μs** |  **0.2710 μs** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | SingleElementMoved |    17.001 μs |  0.3469 μs |  0.1540 μs |  0.77 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | SingleElementMoved |    20.939 μs |  1.0598 μs |  0.4706 μs |  0.95 |    0.02 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | SingleElementMoved |    19.431 μs |  0.4101 μs |  0.2145 μs |  0.88 |    0.01 |    2 |         - |          NA |
| WeakHeapSort     | 1024 | SingleElementMoved |    40.432 μs |  0.3455 μs |  0.1807 μs |  1.83 |    0.02 |    3 |         - |          NA |
| SmoothSort       | 1024 | SingleElementMoved |     7.204 μs |  0.0951 μs |  0.0422 μs |  0.33 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 1024 | SingleElementMoved |    24.578 μs |  0.7713 μs |  0.3425 μs |  1.11 |    0.02 |    2 |         - |          NA |
|      |                    |              |            |            |       |         |      |           |             |
| **HeapSort**         | **1024** | **Sorted**             |    **21.945 μs** |  **0.8987 μs** |  **0.3990 μs** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Sorted             |    16.954 μs |  0.3132 μs |  0.1638 μs |  0.77 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | Sorted             |    22.546 μs |  2.3921 μs |  1.2511 μs |  1.03 |    0.06 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | Sorted             |    19.178 μs |  0.1962 μs |  0.0871 μs |  0.87 |    0.02 |    2 |         - |          NA |
| WeakHeapSort     | 1024 | Sorted             |    41.424 μs |  1.3472 μs |  0.4804 μs |  1.89 |    0.04 |    3 |         - |          NA |
| SmoothSort       | 1024 | Sorted             |     5.217 μs |  0.4713 μs |  0.2465 μs |  0.24 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 1024 | Sorted             |    15.185 μs |  0.5057 μs |  0.2245 μs |  0.69 |    0.02 |    2 |         - |          NA |
|      |                    |              |            |            |       |         |      |           |             |
| **HeapSort**         | **1024** | **Reversed**           |    **19.165 μs** |  **0.3426 μs** |  **0.1521 μs** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Reversed           |    18.384 μs |  0.5643 μs |  0.2505 μs |  0.96 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | Reversed           |    19.377 μs |  0.3890 μs |  0.1727 μs |  1.01 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | Reversed           |    19.968 μs |  0.2986 μs |  0.1562 μs |  1.04 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | Reversed           |    44.556 μs |  0.3408 μs |  0.1782 μs |  2.32 |    0.02 |    2 |         - |          NA |
| SmoothSort       | 1024 | Reversed           |    23.319 μs |  0.5224 μs |  0.2732 μs |  1.22 |    0.02 |    1 |         - |          NA |
| TournamentSort   | 1024 | Reversed           |    26.520 μs |  2.5018 μs |  1.3085 μs |  1.38 |    0.07 |    1 |         - |          NA |
|      |                    |              |            |            |       |         |      |           |             |
| **HeapSort**         | **1024** | **PipeOrgan**          |    **16.122 μs** |  **0.5522 μs** |  **0.2888 μs** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | PipeOrgan          |    16.284 μs |  0.3268 μs |  0.1451 μs |  1.01 |    0.02 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | PipeOrgan          |    18.521 μs |  0.7336 μs |  0.3837 μs |  1.15 |    0.03 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | PipeOrgan          |    19.724 μs |  0.6489 μs |  0.3394 μs |  1.22 |    0.03 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | PipeOrgan          |    43.699 μs |  0.1829 μs |  0.0812 μs |  2.71 |    0.05 |    3 |         - |          NA |
| SmoothSort       | 1024 | PipeOrgan          |    27.773 μs |  0.2093 μs |  0.0929 μs |  1.72 |    0.03 |    2 |         - |          NA |
| TournamentSort   | 1024 | PipeOrgan          |    42.550 μs |  5.4735 μs |  2.8627 μs |  2.64 |    0.17 |    3 |         - |          NA |
|      |                    |              |            |            |       |         |      |           |             |
| **HeapSort**         | **8192** | **Random**             |   **498.642 μs** |  **2.6483 μs** |  **1.3851 μs** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Random             |   485.454 μs |  1.0077 μs |  0.4474 μs |  0.97 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | Random             |   587.610 μs | 10.5316 μs |  5.5083 μs |  1.18 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 8192 | Random             |   580.468 μs |  1.6682 μs |  0.7407 μs |  1.16 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | Random             |   887.777 μs |  3.6230 μs |  1.8949 μs |  1.78 |    0.01 |    2 |         - |          NA |
| SmoothSort       | 8192 | Random             |   817.083 μs |  1.9877 μs |  0.8825 μs |  1.64 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 8192 | Random             | 1,346.871 μs |  2.3448 μs |  1.2264 μs |  2.70 |    0.01 |    3 |         - |          NA |
|      |                    |              |            |            |       |         |      |           |             |
| **HeapSort**         | **8192** | **SingleElementMoved** |   **369.125 μs** |  **0.9746 μs** |  **0.5098 μs** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 8192 | SingleElementMoved |   378.929 μs |  1.1040 μs |  0.5774 μs |  1.03 |    0.00 |    2 |         - |          NA |
| TernaryHeapSort  | 8192 | SingleElementMoved |   413.475 μs |  0.8062 μs |  0.4217 μs |  1.12 |    0.00 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | SingleElementMoved |   446.182 μs |  1.5365 μs |  0.8036 μs |  1.21 |    0.00 |    2 |         - |          NA |
| WeakHeapSort     | 8192 | SingleElementMoved |   432.446 μs |  0.8502 μs |  0.3775 μs |  1.17 |    0.00 |    2 |         - |          NA |
| SmoothSort       | 8192 | SingleElementMoved |    58.421 μs |  0.6614 μs |  0.3459 μs |  0.16 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 8192 | SingleElementMoved |   771.294 μs |  3.2715 μs |  1.4526 μs |  2.09 |    0.00 |    3 |         - |          NA |
|      |                    |              |            |            |       |         |      |           |             |
| **HeapSort**         | **8192** | **Sorted**             |   **368.229 μs** |  **1.7571 μs** |  **0.9190 μs** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Sorted             |   378.151 μs |  0.8005 μs |  0.4187 μs |  1.03 |    0.00 |    2 |         - |          NA |
| TernaryHeapSort  | 8192 | Sorted             |   413.950 μs |  1.0255 μs |  0.4553 μs |  1.12 |    0.00 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | Sorted             |   447.139 μs |  0.4845 μs |  0.2151 μs |  1.21 |    0.00 |    2 |         - |          NA |
| WeakHeapSort     | 8192 | Sorted             |   440.648 μs |  1.2889 μs |  0.5723 μs |  1.20 |    0.00 |    2 |         - |          NA |
| SmoothSort       | 8192 | Sorted             |    41.782 μs |  0.5816 μs |  0.2074 μs |  0.11 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 8192 | Sorted             |   509.302 μs | 11.4477 μs |  5.9874 μs |  1.38 |    0.02 |    2 |         - |          NA |
|      |                    |              |            |            |       |         |      |           |             |
| **HeapSort**         | **8192** | **Reversed**           |   **396.268 μs** |  **1.3777 μs** |  **0.6117 μs** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Reversed           |   352.234 μs |  1.8329 μs |  0.8138 μs |  0.89 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | Reversed           |   424.683 μs |  1.1590 μs |  0.6062 μs |  1.07 |    0.00 |    1 |         - |          NA |
| BottomupHeapSort | 8192 | Reversed           |   478.194 μs |  0.5712 μs |  0.2988 μs |  1.21 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | Reversed           |   471.847 μs |  2.7241 μs |  1.4248 μs |  1.19 |    0.00 |    1 |         - |          NA |
| SmoothSort       | 8192 | Reversed           |   506.316 μs |  1.1822 μs |  0.5249 μs |  1.28 |    0.00 |    1 |         - |          NA |
| TournamentSort   | 8192 | Reversed           |   727.519 μs |  3.6207 μs |  1.8937 μs |  1.84 |    0.01 |    2 |         - |          NA |
|      |                    |              |            |            |       |         |      |           |             |
| **HeapSort**         | **8192** | **PipeOrgan**          |   **369.103 μs** |  **1.9730 μs** |  **1.0319 μs** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | PipeOrgan          |   380.716 μs |  1.1460 μs |  0.5088 μs |  1.03 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | PipeOrgan          |   455.247 μs |  2.3030 μs |  1.2045 μs |  1.23 |    0.00 |    1 |         - |          NA |
| BottomupHeapSort | 8192 | PipeOrgan          |   454.933 μs |  1.3615 μs |  0.7121 μs |  1.23 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | PipeOrgan          |   501.495 μs |  3.2322 μs |  1.6905 μs |  1.36 |    0.01 |    1 |         - |          NA |
| SmoothSort       | 8192 | PipeOrgan          |   655.476 μs |  0.8299 μs |  0.2959 μs |  1.78 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 8192 | PipeOrgan          | 1,063.350 μs |  2.1388 μs |  1.1187 μs |  2.88 |    0.01 |    3 |         - |          NA |

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

| Method                 | Size | Pattern            | Mean         | Error       | StdDev      | Median       | Ratio  | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------- |----- |------------------- |-------------:|------------:|------------:|-------------:|-------:|--------:|-----:|----------:|------------:|
| **InsertionSort**          | **256**  | **Random**             |   **8,576.0 ns** |   **465.78 ns** |   **206.81 ns** |   **8,618.1 ns** |   **1.00** |    **0.03** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Random             |   9,453.1 ns |   999.71 ns |   522.87 ns |   9,316.7 ns |   1.10 |    0.06 |    3 |         - |          NA |
| BinaryInsertSort       | 256  | Random             |   9,487.1 ns |   502.29 ns |   262.71 ns |   9,422.5 ns |   1.11 |    0.04 |    3 |         - |          NA |
| GnomeSort              | 256  | Random             |  27,973.3 ns |   229.18 ns |   119.87 ns |  27,989.4 ns |   3.26 |    0.08 |    5 |         - |          NA |
| LibrarySort            | 256  | Random             |  16,405.7 ns |   190.48 ns |    99.62 ns |  16,377.6 ns |   1.91 |    0.04 |    4 |         - |          NA |
| MergeInsertionSort     | 256  | Random             |  32,992.4 ns |   909.85 ns |   475.87 ns |  32,972.4 ns |   3.85 |    0.10 |    5 |         - |          NA |
| ShellSortKnuth1973     | 256  | Random             |   2,972.1 ns |    64.65 ns |    28.71 ns |   2,961.9 ns |   0.35 |    0.01 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Random             |   3,206.3 ns |   250.86 ns |   111.39 ns |   3,203.3 ns |   0.37 |    0.01 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | Random             |   4,345.3 ns | 1,854.92 ns |   970.16 ns |   4,900.2 ns |   0.51 |    0.11 |    2 |         - |          NA |
| ShellSortCiura2001     | 256  | Random             |   3,120.1 ns |   429.87 ns |   224.83 ns |   2,978.6 ns |   0.36 |    0.03 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Random             |   3,072.1 ns |    67.30 ns |    24.00 ns |   3,062.9 ns |   0.36 |    0.01 |    1 |         - |          NA |
|      |                    |              |             |             |              |        |         |      |           |             |
| **InsertionSort**          | **256**  | **SingleElementMoved** |     **426.3 ns** |     **7.51 ns** |     **2.68 ns** |     **425.2 ns** |   **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 256  | SingleElementMoved |     293.1 ns |     8.90 ns |     4.65 ns |     291.0 ns |   0.69 |    0.01 |    1 |         - |          NA |
| BinaryInsertSort       | 256  | SingleElementMoved |     974.4 ns |    14.45 ns |     5.15 ns |     972.9 ns |   2.29 |    0.02 |    3 |         - |          NA |
| GnomeSort              | 256  | SingleElementMoved |     465.5 ns |     2.41 ns |     1.07 ns |     465.1 ns |   1.09 |    0.01 |    2 |         - |          NA |
| LibrarySort            | 256  | SingleElementMoved |   8,477.3 ns |   909.76 ns |   403.94 ns |   8,265.4 ns |  19.89 |    0.90 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | SingleElementMoved |  25,083.7 ns |   615.10 ns |   321.71 ns |  25,010.8 ns |  58.85 |    0.79 |    6 |         - |          NA |
| ShellSortKnuth1973     | 256  | SingleElementMoved |   1,295.1 ns |     5.98 ns |     3.13 ns |   1,295.4 ns |   3.04 |    0.02 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 256  | SingleElementMoved |   1,280.0 ns |    20.94 ns |     9.30 ns |   1,284.3 ns |   3.00 |    0.03 |    4 |         - |          NA |
| ShellSortTokuda1992    | 256  | SingleElementMoved |   1,602.4 ns |     8.82 ns |     4.61 ns |   1,600.9 ns |   3.76 |    0.02 |    4 |         - |          NA |
| ShellSortCiura2001     | 256  | SingleElementMoved |   1,390.9 ns |    15.73 ns |     5.61 ns |   1,390.2 ns |   3.26 |    0.02 |    4 |         - |          NA |
| ShellSortLee2021       | 256  | SingleElementMoved |   1,808.2 ns |   309.87 ns |   162.07 ns |   1,838.6 ns |   4.24 |    0.36 |    4 |         - |          NA |
|      |                    |              |             |             |              |        |         |      |           |             |
| **InsertionSort**          | **256**  | **Sorted**             |     **324.1 ns** |     **0.92 ns** |     **0.48 ns** |     **324.2 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Sorted             |     401.9 ns |   301.08 ns |   157.47 ns |     515.2 ns |   1.24 |    0.46 |    4 |         - |          NA |
| BinaryInsertSort       | 256  | Sorted             |     173.3 ns |     1.72 ns |     0.76 ns |     173.1 ns |   0.53 |    0.00 |    1 |         - |          NA |
| GnomeSort              | 256  | Sorted             |     247.1 ns |     0.74 ns |     0.39 ns |     247.1 ns |   0.76 |    0.00 |    2 |         - |          NA |
| LibrarySort            | 256  | Sorted             |   6,771.9 ns |    23.30 ns |    10.35 ns |   6,773.6 ns |  20.89 |    0.04 |    6 |         - |          NA |
| MergeInsertionSort     | 256  | Sorted             |  23,927.8 ns |   857.70 ns |   448.59 ns |  23,956.2 ns |  73.82 |    1.31 |    7 |         - |          NA |
| ShellSortKnuth1973     | 256  | Sorted             |   1,068.7 ns |    15.60 ns |     5.56 ns |   1,066.4 ns |   3.30 |    0.02 |    5 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Sorted             |   1,069.8 ns |     1.43 ns |     0.75 ns |   1,069.7 ns |   3.30 |    0.01 |    5 |         - |          NA |
| ShellSortTokuda1992    | 256  | Sorted             |   1,307.0 ns |     1.88 ns |     0.98 ns |   1,306.8 ns |   4.03 |    0.01 |    5 |         - |          NA |
| ShellSortCiura2001     | 256  | Sorted             |   1,144.6 ns |     1.89 ns |     0.84 ns |   1,144.5 ns |   3.53 |    0.01 |    5 |         - |          NA |
| ShellSortLee2021       | 256  | Sorted             |   1,475.3 ns |   396.20 ns |   207.22 ns |   1,377.0 ns |   4.55 |    0.60 |    5 |         - |          NA |
|      |                    |              |             |             |              |        |         |      |           |             |
| **InsertionSort**          | **256**  | **Reversed**           |  **16,606.2 ns** |   **203.25 ns** |   **106.30 ns** |  **16,617.6 ns** |   **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Reversed           |  18,785.8 ns |   462.40 ns |   205.31 ns |  18,720.8 ns |   1.13 |    0.01 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | Reversed           |  16,977.4 ns |   459.47 ns |   240.31 ns |  16,928.4 ns |   1.02 |    0.01 |    2 |         - |          NA |
| GnomeSort              | 256  | Reversed           |  57,766.9 ns |   200.36 ns |    88.96 ns |  57,771.5 ns |   3.48 |    0.02 |    5 |         - |          NA |
| LibrarySort            | 256  | Reversed           |  38,774.6 ns |   537.69 ns |   281.22 ns |  38,720.3 ns |   2.34 |    0.02 |    4 |         - |          NA |
| MergeInsertionSort     | 256  | Reversed           |  24,773.3 ns |   536.59 ns |   280.65 ns |  24,857.2 ns |   1.49 |    0.02 |    3 |         - |          NA |
| ShellSortKnuth1973     | 256  | Reversed           |   1,779.4 ns |     9.70 ns |     4.31 ns |   1,778.1 ns |   0.11 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Reversed           |   1,778.4 ns |    33.30 ns |    14.79 ns |   1,771.4 ns |   0.11 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | Reversed           |   1,898.5 ns |    23.10 ns |    10.26 ns |   1,896.3 ns |   0.11 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | Reversed           |   1,792.3 ns |    13.19 ns |     5.86 ns |   1,789.3 ns |   0.11 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Reversed           |   1,874.6 ns |    13.48 ns |     4.81 ns |   1,874.4 ns |   0.11 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |              |        |         |      |           |             |
| **InsertionSort**          | **256**  | **PipeOrgan**          |   **8,678.5 ns** |   **413.21 ns** |   **216.12 ns** |   **8,676.9 ns** |   **1.00** |    **0.03** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 256  | PipeOrgan          |   9,751.9 ns |   544.09 ns |   241.58 ns |   9,862.1 ns |   1.12 |    0.04 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | PipeOrgan          |  10,138.8 ns |   448.82 ns |   234.74 ns |  10,216.4 ns |   1.17 |    0.04 |    2 |         - |          NA |
| GnomeSort              | 256  | PipeOrgan          |  23,704.7 ns |   219.92 ns |   115.02 ns |  23,700.4 ns |   2.73 |    0.07 |    3 |         - |          NA |
| LibrarySort            | 256  | PipeOrgan          |  24,130.0 ns |   228.70 ns |   101.54 ns |  24,151.3 ns |   2.78 |    0.07 |    3 |         - |          NA |
| MergeInsertionSort     | 256  | PipeOrgan          |  22,755.3 ns | 1,696.35 ns |   887.22 ns |  22,582.6 ns |   2.62 |    0.11 |    3 |         - |          NA |
| ShellSortKnuth1973     | 256  | PipeOrgan          |   1,548.1 ns |    26.15 ns |    11.61 ns |   1,543.2 ns |   0.18 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | PipeOrgan          |   1,524.3 ns |    16.25 ns |     5.79 ns |   1,525.1 ns |   0.18 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | PipeOrgan          |   1,746.6 ns |     7.63 ns |     3.39 ns |   1,744.8 ns |   0.20 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | PipeOrgan          |   1,655.7 ns |    16.08 ns |     7.14 ns |   1,653.6 ns |   0.19 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | PipeOrgan          |   1,777.5 ns |    46.42 ns |    20.61 ns |   1,772.6 ns |   0.20 |    0.01 |    1 |         - |          NA |
|      |                    |              |             |             |              |        |         |      |           |             |
| **InsertionSort**          | **1024** | **Random**             | **127,373.7 ns** | **4,191.59 ns** | **1,861.09 ns** | **126,675.2 ns** |   **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Random             | 140,645.0 ns | 4,078.33 ns | 2,133.04 ns | 141,734.4 ns |   1.10 |    0.02 |    3 |         - |          NA |
| BinaryInsertSort       | 1024 | Random             | 147,321.2 ns |   498.37 ns |   221.28 ns | 147,310.5 ns |   1.16 |    0.02 |    3 |         - |          NA |
| GnomeSort              | 1024 | Random             | 425,333.5 ns | 3,582.10 ns | 1,873.51 ns | 424,533.9 ns |   3.34 |    0.05 |    4 |         - |          NA |
| LibrarySort            | 1024 | Random             |  84,459.1 ns | 2,618.80 ns | 1,162.76 ns |  84,516.0 ns |   0.66 |    0.01 |    2 |         - |          NA |
| MergeInsertionSort     | 1024 | Random             | 382,307.3 ns | 2,403.03 ns | 1,256.83 ns | 382,575.8 ns |   3.00 |    0.04 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Random             |  16,404.8 ns |   213.64 ns |    94.86 ns |  16,423.8 ns |   0.13 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Random             |  16,580.9 ns |   266.36 ns |   118.27 ns |  16,593.4 ns |   0.13 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Random             |  17,125.7 ns |   186.18 ns |    82.67 ns |  17,121.4 ns |   0.13 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Random             |  16,848.3 ns |   264.95 ns |   117.64 ns |  16,838.7 ns |   0.13 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Random             |  16,961.7 ns |   310.40 ns |   137.82 ns |  16,919.5 ns |   0.13 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |              |        |         |      |           |             |
| **InsertionSort**          | **1024** | **SingleElementMoved** |   **1,628.4 ns** |     **3.13 ns** |     **1.12 ns** |   **1,628.2 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | SingleElementMoved |   1,442.0 ns |    57.74 ns |    25.64 ns |   1,436.7 ns |   0.89 |    0.01 |    1 |         - |          NA |
| BinaryInsertSort       | 1024 | SingleElementMoved |   4,814.8 ns |   289.54 ns |   151.44 ns |   4,709.5 ns |   2.96 |    0.09 |    2 |         - |          NA |
| GnomeSort              | 1024 | SingleElementMoved |   1,806.4 ns |     1.50 ns |     0.53 ns |   1,806.3 ns |   1.11 |    0.00 |    1 |         - |          NA |
| LibrarySort            | 1024 | SingleElementMoved |  37,211.2 ns |   516.17 ns |   269.97 ns |  37,078.4 ns |  22.85 |    0.16 |    4 |         - |          NA |
| MergeInsertionSort     | 1024 | SingleElementMoved | 263,255.3 ns | 3,951.93 ns | 1,754.68 ns | 263,555.9 ns | 161.66 |    1.01 |    5 |         - |          NA |
| ShellSortKnuth1973     | 1024 | SingleElementMoved |   6,041.4 ns |     3.86 ns |     2.02 ns |   6,041.4 ns |   3.71 |    0.00 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | SingleElementMoved |   6,822.2 ns |   380.89 ns |   199.21 ns |   6,696.4 ns |   4.19 |    0.12 |    3 |         - |          NA |
| ShellSortTokuda1992    | 1024 | SingleElementMoved |   7,741.6 ns |   298.76 ns |   156.26 ns |   7,724.1 ns |   4.75 |    0.09 |    3 |         - |          NA |
| ShellSortCiura2001     | 1024 | SingleElementMoved |   7,345.8 ns |   208.00 ns |   108.79 ns |   7,364.8 ns |   4.51 |    0.06 |    3 |         - |          NA |
| ShellSortLee2021       | 1024 | SingleElementMoved |   7,668.2 ns |   174.25 ns |    91.14 ns |   7,711.5 ns |   4.71 |    0.05 |    3 |         - |          NA |
|      |                    |              |             |             |              |        |         |      |           |             |
| **InsertionSort**          | **1024** | **Sorted**             |   **1,595.6 ns** |   **444.10 ns** |   **158.37 ns** |   **1,654.5 ns** |   **1.01** |    **0.14** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Sorted             |     805.2 ns |     0.92 ns |     0.41 ns |     805.1 ns |   0.51 |    0.06 |    1 |         - |          NA |
| BinaryInsertSort       | 1024 | Sorted             |     901.7 ns |    34.67 ns |    15.39 ns |     898.5 ns |   0.57 |    0.06 |    1 |         - |          NA |
| GnomeSort              | 1024 | Sorted             |     956.9 ns |     0.94 ns |     0.34 ns |     956.8 ns |   0.61 |    0.07 |    1 |         - |          NA |
| LibrarySort            | 1024 | Sorted             |  31,260.6 ns |   321.46 ns |   142.73 ns |  31,271.0 ns |  19.78 |    2.16 |    4 |         - |          NA |
| MergeInsertionSort     | 1024 | Sorted             | 258,721.5 ns | 4,834.22 ns | 2,528.39 ns | 258,928.8 ns | 163.75 |   17.92 |    5 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Sorted             |   5,365.6 ns |   363.66 ns |   190.20 ns |   5,238.0 ns |   3.40 |    0.39 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Sorted             |   6,096.6 ns |   379.62 ns |   168.56 ns |   6,152.5 ns |   3.86 |    0.43 |    3 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Sorted             |   6,477.2 ns |   369.17 ns |   193.08 ns |   6,446.2 ns |   4.10 |    0.46 |    3 |         - |          NA |
| ShellSortCiura2001     | 1024 | Sorted             |   6,204.1 ns |     4.61 ns |     1.65 ns |   6,203.7 ns |   3.93 |    0.43 |    3 |         - |          NA |
| ShellSortLee2021       | 1024 | Sorted             |   6,514.5 ns |   238.53 ns |   124.76 ns |   6,572.6 ns |   4.12 |    0.46 |    3 |         - |          NA |
|      |                    |              |             |             |              |        |         |      |           |             |
| **InsertionSort**          | **1024** | **Reversed**           | **250,215.6 ns** | **1,283.28 ns** |   **671.18 ns** | **250,450.9 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Reversed           | 281,049.1 ns | 2,221.27 ns |   986.26 ns | 280,543.0 ns |   1.12 |    0.00 |    2 |         - |          NA |
| BinaryInsertSort       | 1024 | Reversed           | 235,337.1 ns | 1,544.28 ns |   685.67 ns | 235,468.4 ns |   0.94 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | Reversed           | 845,857.0 ns |   791.00 ns |   282.08 ns | 845,951.8 ns |   3.38 |    0.01 |    4 |         - |          NA |
| LibrarySort            | 1024 | Reversed           | 413,012.3 ns |   487.67 ns |   216.53 ns | 412,921.7 ns |   1.65 |    0.00 |    3 |         - |          NA |
| MergeInsertionSort     | 1024 | Reversed           | 259,555.7 ns | 8,571.89 ns | 3,805.97 ns | 259,817.1 ns |   1.04 |    0.01 |    2 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Reversed           |   8,585.4 ns |   541.23 ns |   283.08 ns |   8,499.1 ns |   0.03 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Reversed           |   8,953.5 ns |   520.17 ns |   230.96 ns |   9,031.7 ns |   0.04 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Reversed           |   9,949.5 ns |   541.44 ns |   283.18 ns |   9,919.7 ns |   0.04 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Reversed           |   9,410.7 ns |   402.07 ns |   210.29 ns |   9,400.7 ns |   0.04 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Reversed           |   9,990.5 ns |   527.88 ns |   276.09 ns |  10,097.3 ns |   0.04 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |              |        |         |      |           |             |
| **InsertionSort**          | **1024** | **PipeOrgan**          | **128,291.7 ns** | **4,140.10 ns** | **2,165.35 ns** | **127,594.2 ns** |   **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | PipeOrgan          | 144,683.1 ns | 4,210.71 ns | 2,202.28 ns | 145,227.7 ns |   1.13 |    0.02 |    2 |         - |          NA |
| BinaryInsertSort       | 1024 | PipeOrgan          | 130,444.0 ns |   658.56 ns |   344.44 ns | 130,531.6 ns |   1.02 |    0.02 |    2 |         - |          NA |
| GnomeSort              | 1024 | PipeOrgan          | 340,409.5 ns |   854.20 ns |   379.27 ns | 340,305.9 ns |   2.65 |    0.04 |    5 |         - |          NA |
| LibrarySort            | 1024 | PipeOrgan          | 270,925.3 ns |   762.54 ns |   338.57 ns | 270,956.7 ns |   2.11 |    0.03 |    4 |         - |          NA |
| MergeInsertionSort     | 1024 | PipeOrgan          | 219,410.7 ns | 9,448.13 ns | 4,195.03 ns | 217,927.5 ns |   1.71 |    0.04 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | PipeOrgan          |   8,231.2 ns |   327.44 ns |   171.26 ns |   8,283.1 ns |   0.06 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | PipeOrgan          |   8,711.2 ns |   408.79 ns |   213.80 ns |   8,725.8 ns |   0.07 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | PipeOrgan          |   9,280.9 ns |   816.78 ns |   362.66 ns |   9,263.5 ns |   0.07 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | PipeOrgan          |   9,017.4 ns |   331.78 ns |   173.53 ns |   9,103.1 ns |   0.07 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | PipeOrgan          |   9,783.0 ns | 1,937.47 ns | 1,013.34 ns |   9,322.3 ns |   0.08 |    0.01 |    1 |         - |          NA |

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
| **QuickSort**          | **256**  | **Random**             |     **3,038.8 ns** |    **279.98 ns** |    **124.31 ns** |  **1.00** |    **0.05** |    **1** |         **-** |          **NA** |
| QuickSort3way      | 256  | Random             |     3,217.9 ns |    296.63 ns |    155.15 ns |  1.06 |    0.06 |    1 |         - |          NA |
| QuickSortMedian3   | 256  | Random             |     4,485.3 ns |    367.71 ns |    192.32 ns |  1.48 |    0.08 |    1 |         - |          NA |
| QuickSortMedian9   | 256  | Random             |     3,744.2 ns |     73.10 ns |     26.07 ns |  1.23 |    0.04 |    1 |         - |          NA |
| DualPivotQuickSort | 256  | Random             |     2,586.3 ns |     56.40 ns |     25.04 ns |  0.85 |    0.03 |    1 |         - |          NA |
| StableQuickSort    | 256  | Random             |    11,437.7 ns |    612.45 ns |    320.32 ns |  3.77 |    0.17 |    2 |         - |          NA |
| IntroSort          | 256  | Random             |     2,194.5 ns |     36.63 ns |     13.06 ns |  0.72 |    0.03 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | Random             |     1,895.5 ns |     65.53 ns |     29.10 ns |  0.62 |    0.02 |    1 |         - |          NA |
| PDQSort            | 256  | Random             |     1,902.1 ns |    101.92 ns |     45.25 ns |  0.63 |    0.03 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | Random             |     3,347.9 ns |    193.15 ns |     85.76 ns |  1.10 |    0.05 |    1 |         - |          NA |
| StdSort            | 256  | Random             |     3,226.2 ns |     61.84 ns |     22.05 ns |  1.06 |    0.04 |    1 |         - |          NA |
| BlockQuickSort     | 256  | Random             |     2,827.7 ns |     41.44 ns |     18.40 ns |  0.93 |    0.03 |    1 |         - |          NA |
| DotnetSort         | 256  | Random             |     2,051.4 ns |      9.18 ns |      3.27 ns |  0.68 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **256**  | **SingleElementMoved** |     **1,572.7 ns** |     **25.82 ns** |     **13.50 ns** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| QuickSort3way      | 256  | SingleElementMoved |     5,021.2 ns |    445.10 ns |    232.79 ns |  3.19 |    0.14 |    5 |         - |          NA |
| QuickSortMedian3   | 256  | SingleElementMoved |     5,213.7 ns |    410.82 ns |    214.87 ns |  3.32 |    0.13 |    5 |         - |          NA |
| QuickSortMedian9   | 256  | SingleElementMoved |     4,310.0 ns |    416.41 ns |    217.79 ns |  2.74 |    0.13 |    5 |         - |          NA |
| DualPivotQuickSort | 256  | SingleElementMoved |     4,089.8 ns |    308.93 ns |    161.58 ns |  2.60 |    0.10 |    5 |         - |          NA |
| StableQuickSort    | 256  | SingleElementMoved |     8,755.1 ns |    490.42 ns |    217.75 ns |  5.57 |    0.14 |    6 |         - |          NA |
| IntroSort          | 256  | SingleElementMoved |       922.4 ns |     24.50 ns |     10.88 ns |  0.59 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | SingleElementMoved |     1,126.8 ns |     14.65 ns |      6.51 ns |  0.72 |    0.01 |    2 |         - |          NA |
| PDQSort            | 256  | SingleElementMoved |     1,177.4 ns |     17.58 ns |      9.19 ns |  0.75 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless  | 256  | SingleElementMoved |     1,466.4 ns |     32.17 ns |     11.47 ns |  0.93 |    0.01 |    3 |         - |          NA |
| StdSort            | 256  | SingleElementMoved |     2,743.0 ns |     52.18 ns |     18.61 ns |  1.74 |    0.02 |    4 |         - |          NA |
| BlockQuickSort     | 256  | SingleElementMoved |     1,495.3 ns |     18.68 ns |      9.77 ns |  0.95 |    0.01 |    3 |         - |          NA |
| DotnetSort         | 256  | SingleElementMoved |     1,130.1 ns |     30.39 ns |     15.89 ns |  0.72 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **256**  | **Sorted**             |     **1,273.7 ns** |    **221.98 ns** |     **98.56 ns** |  **1.01** |    **0.11** |    **4** |         **-** |          **NA** |
| QuickSort3way      | 256  | Sorted             |     6,662.2 ns |    487.74 ns |    255.10 ns |  5.26 |    0.45 |    7 |         - |          NA |
| QuickSortMedian3   | 256  | Sorted             |     6,525.9 ns |    468.82 ns |    245.20 ns |  5.15 |    0.44 |    7 |         - |          NA |
| QuickSortMedian9   | 256  | Sorted             |     4,778.6 ns |    479.35 ns |    250.71 ns |  3.77 |    0.35 |    6 |         - |          NA |
| DualPivotQuickSort | 256  | Sorted             |     4,652.2 ns |    229.95 ns |    102.10 ns |  3.67 |    0.29 |    6 |         - |          NA |
| StableQuickSort    | 256  | Sorted             |     8,898.5 ns |    239.33 ns |     85.35 ns |  7.02 |    0.55 |    8 |         - |          NA |
| IntroSort          | 256  | Sorted             |       306.2 ns |      3.34 ns |      1.49 ns |  0.24 |    0.02 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | Sorted             |     1,042.2 ns |     20.97 ns |      9.31 ns |  0.82 |    0.06 |    3 |         - |          NA |
| PDQSort            | 256  | Sorted             |       600.5 ns |    245.06 ns |    128.17 ns |  0.47 |    0.10 |    2 |         - |          NA |
| PDQSortBranchless  | 256  | Sorted             |       302.4 ns |      2.76 ns |      1.22 ns |  0.24 |    0.02 |    1 |         - |          NA |
| StdSort            | 256  | Sorted             |       709.0 ns |      5.48 ns |      1.96 ns |  0.56 |    0.04 |    2 |         - |          NA |
| BlockQuickSort     | 256  | Sorted             |     1,670.0 ns |    314.79 ns |    164.64 ns |  1.32 |    0.16 |    5 |         - |          NA |
| DotnetSort         | 256  | Sorted             |       918.1 ns |     17.15 ns |      8.97 ns |  0.72 |    0.06 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **256**  | **Reversed**           |     **1,331.9 ns** |    **437.00 ns** |    **228.56 ns** |  **1.03** |    **0.25** |    **3** |         **-** |          **NA** |
| QuickSort3way      | 256  | Reversed           |     5,533.3 ns |    879.14 ns |    459.81 ns |  4.28 |    0.86 |    4 |         - |          NA |
| QuickSortMedian3   | 256  | Reversed           |     7,552.6 ns |    710.47 ns |    315.46 ns |  5.84 |    1.10 |    5 |         - |          NA |
| QuickSortMedian9   | 256  | Reversed           |     5,120.9 ns |    478.57 ns |    250.30 ns |  3.96 |    0.75 |    4 |         - |          NA |
| DualPivotQuickSort | 256  | Reversed           |     4,526.5 ns |     55.60 ns |     19.83 ns |  3.50 |    0.65 |    4 |         - |          NA |
| StableQuickSort    | 256  | Reversed           |     9,204.9 ns |    458.07 ns |    239.58 ns |  7.12 |    1.32 |    6 |         - |          NA |
| IntroSort          | 256  | Reversed           |       635.7 ns |      2.64 ns |      1.38 ns |  0.49 |    0.09 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | Reversed           |     1,602.1 ns |     47.27 ns |     24.72 ns |  1.24 |    0.23 |    3 |         - |          NA |
| PDQSort            | 256  | Reversed           |       534.9 ns |      9.36 ns |      4.16 ns |  0.41 |    0.08 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | Reversed           |       923.7 ns |     11.07 ns |      3.95 ns |  0.71 |    0.13 |    2 |         - |          NA |
| StdSort            | 256  | Reversed           |       924.6 ns |     11.45 ns |      5.08 ns |  0.72 |    0.13 |    2 |         - |          NA |
| BlockQuickSort     | 256  | Reversed           |     1,669.2 ns |    236.07 ns |    104.82 ns |  1.29 |    0.25 |    3 |         - |          NA |
| DotnetSort         | 256  | Reversed           |     1,538.7 ns |    374.78 ns |    196.02 ns |  1.19 |    0.26 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **256**  | **PipeOrgan**          |     **7,991.7 ns** |    **507.73 ns** |    **265.55 ns** |  **1.00** |    **0.04** |    **4** |         **-** |          **NA** |
| QuickSort3way      | 256  | PipeOrgan          |     5,679.4 ns |    220.11 ns |    115.12 ns |  0.71 |    0.03 |    3 |         - |          NA |
| QuickSortMedian3   | 256  | PipeOrgan          |     6,427.1 ns |     50.84 ns |     26.59 ns |  0.81 |    0.03 |    3 |         - |          NA |
| QuickSortMedian9   | 256  | PipeOrgan          |     4,111.9 ns |     94.86 ns |     33.83 ns |  0.52 |    0.02 |    2 |         - |          NA |
| DualPivotQuickSort | 256  | PipeOrgan          |     2,156.1 ns |     20.11 ns |      8.93 ns |  0.27 |    0.01 |    1 |         - |          NA |
| StableQuickSort    | 256  | PipeOrgan          |     9,389.5 ns |    387.76 ns |    202.80 ns |  1.18 |    0.04 |    4 |         - |          NA |
| IntroSort          | 256  | PipeOrgan          |     1,978.3 ns |     60.24 ns |     21.48 ns |  0.25 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | PipeOrgan          |     2,484.6 ns |     75.71 ns |     39.60 ns |  0.31 |    0.01 |    1 |         - |          NA |
| PDQSort            | 256  | PipeOrgan          |     1,855.1 ns |    320.19 ns |    167.46 ns |  0.23 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | PipeOrgan          |     3,475.1 ns |    391.74 ns |    173.94 ns |  0.44 |    0.02 |    2 |         - |          NA |
| StdSort            | 256  | PipeOrgan          |     3,963.5 ns |    314.03 ns |    164.25 ns |  0.50 |    0.03 |    2 |         - |          NA |
| BlockQuickSort     | 256  | PipeOrgan          |     4,576.6 ns |    525.79 ns |    275.00 ns |  0.57 |    0.04 |    2 |         - |          NA |
| DotnetSort         | 256  | PipeOrgan          |     2,420.0 ns |     82.95 ns |     29.58 ns |  0.30 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **1024** | **Random**             |    **15,409.7 ns** |    **438.10 ns** |    **194.52 ns** |  **1.00** |    **0.02** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 1024 | Random             |    18,222.1 ns |  1,251.94 ns |    654.79 ns |  1.18 |    0.04 |    2 |         - |          NA |
| QuickSortMedian3   | 1024 | Random             |    23,582.3 ns |    945.93 ns |    420.00 ns |  1.53 |    0.03 |    2 |         - |          NA |
| QuickSortMedian9   | 1024 | Random             |    19,804.3 ns |  1,792.30 ns |    795.79 ns |  1.29 |    0.05 |    2 |         - |          NA |
| DualPivotQuickSort | 1024 | Random             |    12,625.2 ns |    521.70 ns |    272.86 ns |  0.82 |    0.02 |    1 |         - |          NA |
| StableQuickSort    | 1024 | Random             |    84,228.9 ns |  2,555.67 ns |  1,134.73 ns |  5.47 |    0.10 |    3 |         - |          NA |
| IntroSort          | 1024 | Random             |    12,126.5 ns |    368.23 ns |    192.59 ns |  0.79 |    0.02 |    1 |         - |          NA |
| IntroSortDotnet    | 1024 | Random             |    10,016.8 ns |    439.73 ns |    229.99 ns |  0.65 |    0.02 |    1 |         - |          NA |
| PDQSort            | 1024 | Random             |    10,100.7 ns |    203.07 ns |    106.21 ns |  0.66 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless  | 1024 | Random             |    16,665.4 ns |    245.05 ns |    108.81 ns |  1.08 |    0.01 |    2 |         - |          NA |
| StdSort            | 1024 | Random             |    15,525.6 ns |    216.19 ns |    113.07 ns |  1.01 |    0.01 |    2 |         - |          NA |
| BlockQuickSort     | 1024 | Random             |    16,533.5 ns |    216.76 ns |     96.24 ns |  1.07 |    0.01 |    2 |         - |          NA |
| DotnetSort         | 1024 | Random             |    11,418.3 ns |    305.43 ns |    135.61 ns |  0.74 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **1024** | **SingleElementMoved** |     **8,186.5 ns** |  **1,287.41 ns** |    **673.34 ns** |  **1.01** |    **0.11** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 1024 | SingleElementMoved |    35,013.2 ns |    310.32 ns |    162.30 ns |  4.30 |    0.34 |    5 |         - |          NA |
| QuickSortMedian3   | 1024 | SingleElementMoved |    31,667.5 ns |    749.65 ns |    332.85 ns |  3.89 |    0.31 |    5 |         - |          NA |
| QuickSortMedian9   | 1024 | SingleElementMoved |    21,902.5 ns |    660.82 ns |    345.62 ns |  2.69 |    0.22 |    4 |         - |          NA |
| DualPivotQuickSort | 1024 | SingleElementMoved |    23,010.2 ns |    244.44 ns |    108.53 ns |  2.83 |    0.22 |    4 |         - |          NA |
| StableQuickSort    | 1024 | SingleElementMoved |    42,470.3 ns |    298.97 ns |    132.75 ns |  5.22 |    0.41 |    6 |         - |          NA |
| IntroSort          | 1024 | SingleElementMoved |     4,331.1 ns |     60.34 ns |     21.52 ns |  0.53 |    0.04 |    1 |         - |          NA |
| IntroSortDotnet    | 1024 | SingleElementMoved |     6,883.9 ns |    358.58 ns |    187.54 ns |  0.85 |    0.07 |    2 |         - |          NA |
| PDQSort            | 1024 | SingleElementMoved |     5,317.9 ns |    326.88 ns |    145.14 ns |  0.65 |    0.05 |    2 |         - |          NA |
| PDQSortBranchless  | 1024 | SingleElementMoved |     6,265.1 ns |     47.30 ns |     24.74 ns |  0.77 |    0.06 |    2 |         - |          NA |
| StdSort            | 1024 | SingleElementMoved |    12,098.6 ns |    530.02 ns |    277.21 ns |  1.49 |    0.12 |    3 |         - |          NA |
| BlockQuickSort     | 1024 | SingleElementMoved |     9,114.2 ns |    587.79 ns |    307.42 ns |  1.12 |    0.10 |    2 |         - |          NA |
| DotnetSort         | 1024 | SingleElementMoved |     6,994.6 ns |    543.00 ns |    284.00 ns |  0.86 |    0.08 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **1024** | **Sorted**             |     **5,856.0 ns** |    **852.06 ns** |    **378.32 ns** |  **1.00** |    **0.08** |    **3** |         **-** |          **NA** |
| QuickSort3way      | 1024 | Sorted             |    46,638.3 ns |    250.61 ns |    131.07 ns |  7.99 |    0.45 |    6 |         - |          NA |
| QuickSortMedian3   | 1024 | Sorted             |    44,226.4 ns |  1,674.56 ns |    875.83 ns |  7.58 |    0.45 |    6 |         - |          NA |
| QuickSortMedian9   | 1024 | Sorted             |    22,227.6 ns |    691.83 ns |    307.18 ns |  3.81 |    0.22 |    5 |         - |          NA |
| DualPivotQuickSort | 1024 | Sorted             |    24,701.0 ns |    465.24 ns |    243.33 ns |  4.23 |    0.24 |    5 |         - |          NA |
| StableQuickSort    | 1024 | Sorted             |    42,411.8 ns |    463.03 ns |    242.18 ns |  7.27 |    0.41 |    6 |         - |          NA |
| IntroSort          | 1024 | Sorted             |     1,112.5 ns |     13.53 ns |      4.82 ns |  0.19 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet    | 1024 | Sorted             |     5,067.7 ns |    380.16 ns |    198.83 ns |  0.87 |    0.06 |    3 |         - |          NA |
| PDQSort            | 1024 | Sorted             |     1,022.7 ns |     33.53 ns |     11.96 ns |  0.18 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless  | 1024 | Sorted             |     1,029.5 ns |     42.29 ns |     15.08 ns |  0.18 |    0.01 |    1 |         - |          NA |
| StdSort            | 1024 | Sorted             |     2,612.0 ns |      8.12 ns |      2.90 ns |  0.45 |    0.03 |    2 |         - |          NA |
| BlockQuickSort     | 1024 | Sorted             |     7,538.1 ns |    256.61 ns |    134.21 ns |  1.29 |    0.08 |    4 |         - |          NA |
| DotnetSort         | 1024 | Sorted             |     4,574.1 ns |     15.00 ns |      5.35 ns |  0.78 |    0.04 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **1024** | **Reversed**           |     **7,248.8 ns** |  **4,940.23 ns** |  **2,583.84 ns** |  **1.12** |    **0.54** |    **3** |         **-** |          **NA** |
| QuickSort3way      | 1024 | Reversed           |    39,139.3 ns |    613.16 ns |    218.66 ns |  6.04 |    1.98 |    5 |         - |          NA |
| QuickSortMedian3   | 1024 | Reversed           |    52,323.2 ns |  1,413.09 ns |    627.42 ns |  8.07 |    2.65 |    5 |         - |          NA |
| QuickSortMedian9   | 1024 | Reversed           |    23,091.1 ns |    324.27 ns |    143.98 ns |  3.56 |    1.17 |    4 |         - |          NA |
| DualPivotQuickSort | 1024 | Reversed           |    24,367.6 ns |    262.79 ns |    116.68 ns |  3.76 |    1.23 |    4 |         - |          NA |
| StableQuickSort    | 1024 | Reversed           |    45,506.3 ns |    819.00 ns |    428.35 ns |  7.02 |    2.30 |    5 |         - |          NA |
| IntroSort          | 1024 | Reversed           |     3,879.0 ns |     76.48 ns |     33.96 ns |  0.60 |    0.20 |    2 |         - |          NA |
| IntroSortDotnet    | 1024 | Reversed           |     8,061.6 ns |    467.57 ns |    244.55 ns |  1.24 |    0.41 |    3 |         - |          NA |
| PDQSort            | 1024 | Reversed           |     1,916.3 ns |     12.61 ns |      4.50 ns |  0.30 |    0.10 |    1 |         - |          NA |
| PDQSortBranchless  | 1024 | Reversed           |     3,308.7 ns |     27.46 ns |      9.79 ns |  0.51 |    0.17 |    2 |         - |          NA |
| StdSort            | 1024 | Reversed           |     3,369.3 ns |     15.94 ns |      5.69 ns |  0.52 |    0.17 |    2 |         - |          NA |
| BlockQuickSort     | 1024 | Reversed           |     8,268.5 ns |  1,251.69 ns |    555.76 ns |  1.28 |    0.43 |    3 |         - |          NA |
| DotnetSort         | 1024 | Reversed           |     7,417.6 ns |    471.22 ns |    246.46 ns |  1.14 |    0.38 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **1024** | **PipeOrgan**          |    **97,575.6 ns** |    **729.33 ns** |    **323.83 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way      | 1024 | PipeOrgan          |    35,217.9 ns |    587.85 ns |    209.63 ns |  0.36 |    0.00 |    4 |         - |          NA |
| QuickSortMedian3   | 1024 | PipeOrgan          |    38,472.1 ns |  1,643.14 ns |    859.40 ns |  0.39 |    0.01 |    4 |         - |          NA |
| QuickSortMedian9   | 1024 | PipeOrgan          |    22,157.3 ns |  1,057.04 ns |    552.85 ns |  0.23 |    0.01 |    3 |         - |          NA |
| DualPivotQuickSort | 1024 | PipeOrgan          |    11,493.0 ns |    541.47 ns |    283.20 ns |  0.12 |    0.00 |    2 |         - |          NA |
| StableQuickSort    | 1024 | PipeOrgan          |    45,602.1 ns |    516.95 ns |    229.53 ns |  0.47 |    0.00 |    4 |         - |          NA |
| IntroSort          | 1024 | PipeOrgan          |    15,480.2 ns |  1,585.53 ns |    829.26 ns |  0.16 |    0.01 |    3 |         - |          NA |
| IntroSortDotnet    | 1024 | PipeOrgan          |    14,987.2 ns |    449.97 ns |    199.79 ns |  0.15 |    0.00 |    3 |         - |          NA |
| PDQSort            | 1024 | PipeOrgan          |     9,479.5 ns |    385.56 ns |    201.66 ns |  0.10 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 1024 | PipeOrgan          |    18,593.2 ns |    504.59 ns |    224.04 ns |  0.19 |    0.00 |    3 |         - |          NA |
| StdSort            | 1024 | PipeOrgan          |    21,248.2 ns |    786.57 ns |    349.24 ns |  0.22 |    0.00 |    3 |         - |          NA |
| BlockQuickSort     | 1024 | PipeOrgan          |    24,987.4 ns |  1,846.56 ns |    965.79 ns |  0.26 |    0.01 |    3 |         - |          NA |
| DotnetSort         | 1024 | PipeOrgan          |    15,519.7 ns |  1,554.40 ns |    812.98 ns |  0.16 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **8192** | **Random**             |   **427,438.6 ns** |  **3,440.81 ns** |  **1,799.61 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| QuickSort3way      | 8192 | Random             |   427,560.2 ns |  3,032.29 ns |  1,081.34 ns |  1.00 |    0.00 |    1 |         - |          NA |
| QuickSortMedian3   | 8192 | Random             |   532,011.5 ns |  1,796.63 ns |    939.67 ns |  1.24 |    0.01 |    1 |         - |          NA |
| QuickSortMedian9   | 8192 | Random             |   514,544.0 ns |  6,475.15 ns |  2,875.01 ns |  1.20 |    0.01 |    1 |         - |          NA |
| DualPivotQuickSort | 8192 | Random             |   364,527.9 ns |  1,842.57 ns |    818.11 ns |  0.85 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 8192 | Random             | 1,149,637.5 ns |  3,838.32 ns |  1,704.24 ns |  2.69 |    0.01 |    2 |         - |          NA |
| IntroSort          | 8192 | Random             |   383,024.4 ns |  1,168.02 ns |    610.90 ns |  0.90 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 8192 | Random             |   351,209.6 ns |  1,451.63 ns |    644.53 ns |  0.82 |    0.00 |    1 |         - |          NA |
| PDQSort            | 8192 | Random             |   361,892.6 ns |  2,724.14 ns |  1,424.78 ns |  0.85 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | Random             |   465,912.8 ns |  2,159.93 ns |  1,129.68 ns |  1.09 |    0.00 |    1 |         - |          NA |
| StdSort            | 8192 | Random             |   403,246.8 ns |    967.53 ns |    429.59 ns |  0.94 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 8192 | Random             |   436,450.2 ns |    795.60 ns |    353.25 ns |  1.02 |    0.00 |    1 |         - |          NA |
| DotnetSort         | 8192 | Random             |   344,074.1 ns |  1,491.42 ns |    662.20 ns |  0.80 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **8192** | **SingleElementMoved** |    **76,829.0 ns** |  **3,476.93 ns** |  **1,818.50 ns** |  **1.00** |    **0.03** |    **3** |         **-** |          **NA** |
| QuickSort3way      | 8192 | SingleElementMoved |   749,866.8 ns |  2,553.02 ns |  1,133.56 ns |  9.77 |    0.22 |    9 |         - |          NA |
| QuickSortMedian3   | 8192 | SingleElementMoved |   573,207.5 ns |  7,934.62 ns |  4,149.96 ns |  7.46 |    0.18 |    8 |         - |          NA |
| QuickSortMedian9   | 8192 | SingleElementMoved |   210,474.7 ns |  4,296.60 ns |  2,247.20 ns |  2.74 |    0.07 |    6 |         - |          NA |
| DualPivotQuickSort | 8192 | SingleElementMoved |   156,783.9 ns |  4,403.17 ns |  2,302.95 ns |  2.04 |    0.05 |    5 |         - |          NA |
| StableQuickSort    | 8192 | SingleElementMoved |   434,777.0 ns |  1,806.71 ns |    802.19 ns |  5.66 |    0.13 |    7 |         - |          NA |
| IntroSort          | 8192 | SingleElementMoved |    41,993.8 ns |  2,676.11 ns |  1,399.66 ns |  0.55 |    0.02 |    1 |         - |          NA |
| IntroSortDotnet    | 8192 | SingleElementMoved |    64,113.7 ns |    712.21 ns |    316.23 ns |  0.83 |    0.02 |    3 |         - |          NA |
| PDQSort            | 8192 | SingleElementMoved |    43,918.8 ns |    764.89 ns |    339.62 ns |  0.57 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | SingleElementMoved |    53,315.7 ns |    998.39 ns |    443.29 ns |  0.69 |    0.02 |    2 |         - |          NA |
| StdSort            | 8192 | SingleElementMoved |    94,185.7 ns |  1,972.53 ns |  1,031.67 ns |  1.23 |    0.03 |    4 |         - |          NA |
| BlockQuickSort     | 8192 | SingleElementMoved |    92,637.7 ns |    651.86 ns |    340.93 ns |  1.21 |    0.03 |    4 |         - |          NA |
| DotnetSort         | 8192 | SingleElementMoved |    76,681.1 ns |    777.37 ns |    277.22 ns |  1.00 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **8192** | **Sorted**             |    **61,483.3 ns** |  **6,099.77 ns** |  **2,708.33 ns** |  **1.00** |    **0.06** |    **4** |         **-** |          **NA** |
| QuickSort3way      | 8192 | Sorted             | 1,025,230.3 ns |  8,103.37 ns |  3,597.95 ns | 16.70 |    0.72 |    8 |         - |          NA |
| QuickSortMedian3   | 8192 | Sorted             |   893,150.6 ns |  3,898.35 ns |  2,038.91 ns | 14.55 |    0.62 |    8 |         - |          NA |
| QuickSortMedian9   | 8192 | Sorted             |   208,331.6 ns |  2,166.52 ns |  1,133.13 ns |  3.39 |    0.15 |    6 |         - |          NA |
| DualPivotQuickSort | 8192 | Sorted             |   175,318.6 ns |  1,315.08 ns |    687.81 ns |  2.86 |    0.12 |    6 |         - |          NA |
| StableQuickSort    | 8192 | Sorted             |   431,637.2 ns |    395.34 ns |    206.77 ns |  7.03 |    0.30 |    7 |         - |          NA |
| IntroSort          | 8192 | Sorted             |     8,647.5 ns |    593.82 ns |    263.66 ns |  0.14 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet    | 8192 | Sorted             |    48,818.4 ns |  1,098.65 ns |    487.81 ns |  0.80 |    0.03 |    3 |         - |          NA |
| PDQSort            | 8192 | Sorted             |     8,256.2 ns |  1,487.08 ns |    777.77 ns |  0.13 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | Sorted             |     7,960.1 ns |    273.68 ns |    121.51 ns |  0.13 |    0.01 |    1 |         - |          NA |
| StdSort            | 8192 | Sorted             |    20,760.8 ns |    431.66 ns |    191.66 ns |  0.34 |    0.01 |    2 |         - |          NA |
| BlockQuickSort     | 8192 | Sorted             |    80,581.1 ns |  1,092.03 ns |    571.15 ns |  1.31 |    0.06 |    5 |         - |          NA |
| DotnetSort         | 8192 | Sorted             |    50,119.8 ns |  4,708.13 ns |  2,462.45 ns |  0.82 |    0.05 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **8192** | **Reversed**           |    **48,388.6 ns** |  **4,617.51 ns** |  **2,415.05 ns** |  **1.00** |    **0.07** |    **4** |         **-** |          **NA** |
| QuickSort3way      | 8192 | Reversed           |   840,025.6 ns |  7,397.78 ns |  3,869.18 ns | 17.40 |    0.83 |    8 |         - |          NA |
| QuickSortMedian3   | 8192 | Reversed           | 1,121,190.7 ns |  7,163.62 ns |  3,180.69 ns | 23.22 |    1.10 |    9 |         - |          NA |
| QuickSortMedian9   | 8192 | Reversed           |   212,171.7 ns |  2,421.30 ns |  1,075.07 ns |  4.39 |    0.21 |    6 |         - |          NA |
| DualPivotQuickSort | 8192 | Reversed           |   179,644.7 ns |  1,121.34 ns |    586.48 ns |  3.72 |    0.18 |    6 |         - |          NA |
| StableQuickSort    | 8192 | Reversed           |   464,484.0 ns |  1,856.14 ns |    970.80 ns |  9.62 |    0.46 |    7 |         - |          NA |
| IntroSort          | 8192 | Reversed           |    35,031.7 ns |  1,570.84 ns |    821.58 ns |  0.73 |    0.04 |    3 |         - |          NA |
| IntroSortDotnet    | 8192 | Reversed           |    80,759.6 ns |  1,456.10 ns |    761.57 ns |  1.67 |    0.08 |    5 |         - |          NA |
| PDQSort            | 8192 | Reversed           |    14,809.2 ns |    221.56 ns |     79.01 ns |  0.31 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | Reversed           |    25,658.7 ns |    544.67 ns |    241.84 ns |  0.53 |    0.03 |    2 |         - |          NA |
| StdSort            | 8192 | Reversed           |    26,585.5 ns |  2,088.87 ns |  1,092.52 ns |  0.55 |    0.03 |    2 |         - |          NA |
| BlockQuickSort     | 8192 | Reversed           |    78,106.7 ns |    500.55 ns |    222.25 ns |  1.62 |    0.08 |    5 |         - |          NA |
| DotnetSort         | 8192 | Reversed           |    81,539.7 ns |  3,628.29 ns |  1,897.67 ns |  1.69 |    0.09 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**          | **8192** | **PipeOrgan**          | **5,369,239.9 ns** | **32,150.27 ns** | **16,815.21 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way      | 8192 | PipeOrgan          |   508,480.8 ns |  1,077.02 ns |    478.20 ns |  0.09 |    0.00 |    2 |         - |          NA |
| QuickSortMedian3   | 8192 | PipeOrgan          |   493,874.3 ns |  3,511.12 ns |  1,252.10 ns |  0.09 |    0.00 |    2 |         - |          NA |
| QuickSortMedian9   | 8192 | PipeOrgan          |   278,672.5 ns |  5,955.74 ns |  3,114.97 ns |  0.05 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort | 8192 | PipeOrgan          |   147,098.7 ns |    931.46 ns |    487.17 ns |  0.03 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 8192 | PipeOrgan          |   469,387.8 ns |  2,554.22 ns |  1,134.09 ns |  0.09 |    0.00 |    2 |         - |          NA |
| IntroSort          | 8192 | PipeOrgan          |   330,011.2 ns |  4,010.59 ns |  2,097.61 ns |  0.06 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet    | 8192 | PipeOrgan          |   371,944.3 ns |  3,723.30 ns |  1,653.17 ns |  0.07 |    0.00 |    2 |         - |          NA |
| PDQSort            | 8192 | PipeOrgan          |   144,129.9 ns |  1,563.58 ns |    817.78 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | PipeOrgan          |   276,436.9 ns |  2,067.69 ns |    918.07 ns |  0.05 |    0.00 |    2 |         - |          NA |
| StdSort            | 8192 | PipeOrgan          |   434,871.3 ns |  2,778.47 ns |  1,453.19 ns |  0.08 |    0.00 |    2 |         - |          NA |
| BlockQuickSort     | 8192 | PipeOrgan          |   267,823.6 ns |  1,187.24 ns |    423.38 ns |  0.05 |    0.00 |    2 |         - |          NA |
| DotnetSort         | 8192 | PipeOrgan          |   359,342.6 ns | 10,790.01 ns |  5,643.38 ns |  0.07 |    0.00 |    2 |         - |          NA |

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

| Method                   | Size | Pattern            | Mean           | Error        | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------------- |----- |------------------- |---------------:|-------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **MergeSort**                | **256**  | **Random**             |     **8,363.5 ns** |    **213.40 ns** |   **111.61 ns** |  **1.00** |    **0.02** |    **5** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Random             |     8,269.2 ns |    481.30 ns |   251.73 ns |  0.99 |    0.03 |    5 |         - |          NA |
| BottomupMergeSort        | 256  | Random             |     4,638.4 ns |    108.68 ns |    38.76 ns |  0.55 |    0.01 |    3 |         - |          NA |
| StdStableSort            | 256  | Random             |     3,554.0 ns |  1,377.31 ns |   720.36 ns |  0.43 |    0.08 |    2 |         - |          NA |
| RotateMergeSort          | 256  | Random             |     9,548.8 ns |    392.35 ns |   205.21 ns |  1.14 |    0.03 |    5 |         - |          NA |
| RotateMergeSortRecursive | 256  | Random             |    11,164.4 ns |    251.18 ns |   131.37 ns |  1.34 |    0.02 |    5 |         - |          NA |
| SymMergeSort             | 256  | Random             |     6,497.0 ns |    101.02 ns |    52.84 ns |  0.78 |    0.01 |    4 |         - |          NA |
| BlockMergeSort           | 256  | Random             |     6,216.1 ns |    307.20 ns |   160.67 ns |  0.74 |    0.02 |    4 |         - |          NA |
| NaturalMergeSort         | 256  | Random             |     5,258.2 ns |    530.06 ns |   277.23 ns |  0.63 |    0.03 |    3 |         - |          NA |
| TimSort                  | 256  | Random             |     3,900.8 ns |    269.02 ns |   140.70 ns |  0.47 |    0.02 |    3 |         - |          NA |
| PowerSort                | 256  | Random             |     2,353.4 ns |     50.04 ns |    22.22 ns |  0.28 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Random             |     4,199.3 ns |    476.76 ns |   249.35 ns |  0.50 |    0.03 |    3 |         - |          NA |
| SpinSort                 | 256  | Random             |     2,190.1 ns |    539.35 ns |   239.47 ns |  0.26 |    0.03 |    1 |         - |          NA |
| SpinSortVariant          | 256  | Random             |     2,357.0 ns |     13.06 ns |     4.66 ns |  0.28 |    0.00 |    1 |         - |          NA |
| Glidesort                | 256  | Random             |     4,956.0 ns |    235.00 ns |   122.91 ns |  0.59 |    0.02 |    3 |         - |          NA |
| FlatStableSort           | 256  | Random             |     2,659.5 ns |     28.18 ns |    12.51 ns |  0.32 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **256**  | **SingleElementMoved** |     **4,413.5 ns** |    **324.49 ns** |   **169.71 ns** |  **1.00** |    **0.05** |    **9** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | SingleElementMoved |     5,475.1 ns |    469.43 ns |   245.52 ns |  1.24 |    0.07 |   10 |         - |          NA |
| BottomupMergeSort        | 256  | SingleElementMoved |     2,444.5 ns |    285.85 ns |   149.51 ns |  0.55 |    0.04 |    7 |         - |          NA |
| StdStableSort            | 256  | SingleElementMoved |     3,100.9 ns |    695.76 ns |   363.90 ns |  0.70 |    0.08 |    8 |         - |          NA |
| RotateMergeSort          | 256  | SingleElementMoved |       582.7 ns |      2.94 ns |     1.30 ns |  0.13 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 256  | SingleElementMoved |       719.6 ns |     23.00 ns |    10.21 ns |  0.16 |    0.01 |    4 |         - |          NA |
| SymMergeSort             | 256  | SingleElementMoved |       829.7 ns |    237.52 ns |   124.23 ns |  0.19 |    0.03 |    5 |         - |          NA |
| BlockMergeSort           | 256  | SingleElementMoved |     4,225.2 ns |     10.59 ns |     4.70 ns |  0.96 |    0.03 |    9 |         - |          NA |
| NaturalMergeSort         | 256  | SingleElementMoved |       598.6 ns |      7.16 ns |     3.18 ns |  0.14 |    0.00 |    3 |         - |          NA |
| TimSort                  | 256  | SingleElementMoved |       312.4 ns |      5.94 ns |     2.64 ns |  0.07 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | SingleElementMoved |       412.0 ns |      9.71 ns |     5.08 ns |  0.09 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 256  | SingleElementMoved |       399.0 ns |     21.15 ns |     9.39 ns |  0.09 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 256  | SingleElementMoved |       910.3 ns |     14.62 ns |     7.64 ns |  0.21 |    0.01 |    5 |         - |          NA |
| SpinSortVariant          | 256  | SingleElementMoved |       950.3 ns |      6.70 ns |     2.97 ns |  0.22 |    0.01 |    5 |         - |          NA |
| Glidesort                | 256  | SingleElementMoved |     1,274.7 ns |     14.46 ns |     5.16 ns |  0.29 |    0.01 |    6 |         - |          NA |
| FlatStableSort           | 256  | SingleElementMoved |     1,234.0 ns |      7.18 ns |     3.19 ns |  0.28 |    0.01 |    6 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **256**  | **Sorted**             |     **3,853.5 ns** |      **3.98 ns** |     **1.42 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Sorted             |     4,978.6 ns |    334.86 ns |   175.14 ns |  1.29 |    0.04 |    8 |         - |          NA |
| BottomupMergeSort        | 256  | Sorted             |     1,953.6 ns |      8.96 ns |     4.69 ns |  0.51 |    0.00 |    6 |         - |          NA |
| StdStableSort            | 256  | Sorted             |     1,698.7 ns |      6.21 ns |     2.76 ns |  0.44 |    0.00 |    6 |         - |          NA |
| RotateMergeSort          | 256  | Sorted             |       362.6 ns |     60.73 ns |    26.96 ns |  0.09 |    0.01 |    3 |         - |          NA |
| RotateMergeSortRecursive | 256  | Sorted             |       463.0 ns |      1.12 ns |     0.50 ns |  0.12 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 256  | Sorted             |       342.1 ns |      1.99 ns |     0.88 ns |  0.09 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 256  | Sorted             |     3,239.6 ns |      4.13 ns |     1.47 ns |  0.84 |    0.00 |    7 |         - |          NA |
| NaturalMergeSort         | 256  | Sorted             |       218.6 ns |      4.45 ns |     2.33 ns |  0.06 |    0.00 |    2 |         - |          NA |
| TimSort                  | 256  | Sorted             |       190.9 ns |      2.19 ns |     0.97 ns |  0.05 |    0.00 |    2 |         - |          NA |
| PowerSort                | 256  | Sorted             |       151.0 ns |      1.34 ns |     0.70 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Sorted             |       309.9 ns |    138.91 ns |    61.68 ns |  0.08 |    0.01 |    3 |         - |          NA |
| SpinSort                 | 256  | Sorted             |       132.6 ns |      1.64 ns |     0.86 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 256  | Sorted             |       181.4 ns |      0.67 ns |     0.35 ns |  0.05 |    0.00 |    2 |         - |          NA |
| Glidesort                | 256  | Sorted             |       188.8 ns |      6.64 ns |     2.37 ns |  0.05 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 256  | Sorted             |     1,120.2 ns |      4.13 ns |     1.47 ns |  0.29 |    0.00 |    5 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **256**  | **Reversed**           |     **8,722.1 ns** |    **320.02 ns** |   **167.37 ns** |  **1.00** |    **0.03** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Reversed           |     8,283.0 ns |    245.34 ns |   128.32 ns |  0.95 |    0.02 |    6 |         - |          NA |
| BottomupMergeSort        | 256  | Reversed           |     5,095.3 ns |    360.34 ns |   188.47 ns |  0.58 |    0.02 |    5 |         - |          NA |
| StdStableSort            | 256  | Reversed           |     2,502.2 ns |    459.89 ns |   240.53 ns |  0.29 |    0.03 |    3 |         - |          NA |
| RotateMergeSort          | 256  | Reversed           |     1,774.3 ns |      5.52 ns |     2.45 ns |  0.20 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 256  | Reversed           |     1,861.0 ns |      1.50 ns |     0.67 ns |  0.21 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 256  | Reversed           |     1,962.8 ns |     12.89 ns |     4.60 ns |  0.23 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 256  | Reversed           |     9,264.3 ns |    295.28 ns |   154.44 ns |  1.06 |    0.03 |    6 |         - |          NA |
| NaturalMergeSort         | 256  | Reversed           |       329.6 ns |      3.57 ns |     1.87 ns |  0.04 |    0.00 |    1 |         - |          NA |
| TimSort                  | 256  | Reversed           |       264.4 ns |      2.94 ns |     1.30 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | Reversed           |       226.6 ns |      1.06 ns |     0.47 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Reversed           |       252.8 ns |      2.09 ns |     0.93 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 256  | Reversed           |       257.6 ns |      1.24 ns |     0.65 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 256  | Reversed           |       286.1 ns |      2.04 ns |     1.07 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 256  | Reversed           |       281.7 ns |      5.23 ns |     2.74 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | Reversed           |     3,059.9 ns |    492.99 ns |   218.89 ns |  0.35 |    0.02 |    4 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **256**  | **PipeOrgan**          |     **6,310.6 ns** |     **17.67 ns** |     **9.24 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | PipeOrgan          |     6,940.8 ns |    207.12 ns |   108.33 ns |  1.10 |    0.02 |    8 |         - |          NA |
| BottomupMergeSort        | 256  | PipeOrgan          |     3,740.6 ns |     25.94 ns |     9.25 ns |  0.59 |    0.00 |    6 |         - |          NA |
| StdStableSort            | 256  | PipeOrgan          |     2,208.9 ns |    188.17 ns |    98.41 ns |  0.35 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 256  | PipeOrgan          |     4,215.4 ns |    417.84 ns |   185.53 ns |  0.67 |    0.03 |    6 |         - |          NA |
| RotateMergeSortRecursive | 256  | PipeOrgan          |     5,113.6 ns |    510.24 ns |   266.86 ns |  0.81 |    0.04 |    7 |         - |          NA |
| SymMergeSort             | 256  | PipeOrgan          |     2,544.5 ns |     71.96 ns |    31.95 ns |  0.40 |    0.00 |    5 |         - |          NA |
| BlockMergeSort           | 256  | PipeOrgan          |     6,344.7 ns |     11.43 ns |     5.98 ns |  1.01 |    0.00 |    8 |         - |          NA |
| NaturalMergeSort         | 256  | PipeOrgan          |       701.9 ns |     38.15 ns |    16.94 ns |  0.11 |    0.00 |    2 |         - |          NA |
| TimSort                  | 256  | PipeOrgan          |       868.4 ns |     53.47 ns |    23.74 ns |  0.14 |    0.00 |    3 |         - |          NA |
| PowerSort                | 256  | PipeOrgan          |       511.6 ns |      7.88 ns |     2.81 ns |  0.08 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | PipeOrgan          |       540.4 ns |      2.65 ns |     1.18 ns |  0.09 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 256  | PipeOrgan          |     1,822.1 ns |    166.89 ns |    74.10 ns |  0.29 |    0.01 |    5 |         - |          NA |
| SpinSortVariant          | 256  | PipeOrgan          |     2,068.7 ns |    387.03 ns |   171.84 ns |  0.33 |    0.03 |    5 |         - |          NA |
| Glidesort                | 256  | PipeOrgan          |     1,218.8 ns |      6.53 ns |     2.90 ns |  0.19 |    0.00 |    4 |         - |          NA |
| FlatStableSort           | 256  | PipeOrgan          |     2,061.8 ns |      6.13 ns |     3.20 ns |  0.33 |    0.00 |    5 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **1024** | **Random**             |    **37,602.8 ns** |  **2,225.08 ns** | **1,163.76 ns** |  **1.00** |    **0.04** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Random             |    35,387.4 ns |    627.20 ns |   278.48 ns |  0.94 |    0.03 |    3 |         - |          NA |
| BottomupMergeSort        | 1024 | Random             |    22,854.0 ns |    692.30 ns |   307.39 ns |  0.61 |    0.02 |    2 |         - |          NA |
| StdStableSort            | 1024 | Random             |    15,423.4 ns |    232.79 ns |   103.36 ns |  0.41 |    0.01 |    1 |         - |          NA |
| RotateMergeSort          | 1024 | Random             |    67,833.8 ns |  6,984.44 ns | 3,653.00 ns |  1.81 |    0.11 |    4 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Random             |    66,589.5 ns |    350.07 ns |   155.43 ns |  1.77 |    0.05 |    4 |         - |          NA |
| SymMergeSort             | 1024 | Random             |    40,909.2 ns |    293.11 ns |   130.14 ns |  1.09 |    0.03 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | Random             |    32,994.9 ns |    180.71 ns |    80.24 ns |  0.88 |    0.03 |    3 |         - |          NA |
| NaturalMergeSort         | 1024 | Random             |    26,151.3 ns |    809.31 ns |   359.34 ns |  0.70 |    0.02 |    2 |         - |          NA |
| TimSort                  | 1024 | Random             |    20,038.8 ns |    596.46 ns |   311.96 ns |  0.53 |    0.02 |    2 |         - |          NA |
| PowerSort                | 1024 | Random             |    13,212.4 ns |    967.59 ns |   429.61 ns |  0.35 |    0.01 |    1 |         - |          NA |
| ShiftSort                | 1024 | Random             |    20,070.0 ns |    758.53 ns |   396.73 ns |  0.53 |    0.02 |    2 |         - |          NA |
| SpinSort                 | 1024 | Random             |    13,567.2 ns |    851.13 ns |   445.16 ns |  0.36 |    0.02 |    1 |         - |          NA |
| SpinSortVariant          | 1024 | Random             |    14,232.0 ns |    715.45 ns |   374.20 ns |  0.38 |    0.01 |    1 |         - |          NA |
| Glidesort                | 1024 | Random             |    24,913.4 ns |  1,145.29 ns |   508.52 ns |  0.66 |    0.02 |    2 |         - |          NA |
| FlatStableSort           | 1024 | Random             |    14,710.1 ns |    265.30 ns |   117.80 ns |  0.39 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **1024** | **SingleElementMoved** |    **17,034.6 ns** |    **166.15 ns** |    **86.90 ns** |  **1.00** |    **0.01** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | SingleElementMoved |    21,071.9 ns |    409.20 ns |   214.02 ns |  1.24 |    0.01 |    8 |         - |          NA |
| BottomupMergeSort        | 1024 | SingleElementMoved |     7,260.0 ns |     24.31 ns |    10.79 ns |  0.43 |    0.00 |    7 |         - |          NA |
| StdStableSort            | 1024 | SingleElementMoved |     8,162.6 ns |    323.82 ns |   169.37 ns |  0.48 |    0.01 |    7 |         - |          NA |
| RotateMergeSort          | 1024 | SingleElementMoved |     1,940.4 ns |      9.12 ns |     4.05 ns |  0.11 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 1024 | SingleElementMoved |     2,460.0 ns |     62.54 ns |    27.77 ns |  0.14 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 1024 | SingleElementMoved |     1,698.2 ns |      6.89 ns |     3.06 ns |  0.10 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | SingleElementMoved |    19,791.2 ns |    124.45 ns |    44.38 ns |  1.16 |    0.01 |    8 |         - |          NA |
| NaturalMergeSort         | 1024 | SingleElementMoved |     2,021.6 ns |      5.00 ns |     2.22 ns |  0.12 |    0.00 |    3 |         - |          NA |
| TimSort                  | 1024 | SingleElementMoved |       830.7 ns |     10.00 ns |     5.23 ns |  0.05 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | SingleElementMoved |     1,373.9 ns |      3.94 ns |     1.75 ns |  0.08 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 1024 | SingleElementMoved |     1,372.7 ns |      7.49 ns |     2.67 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 1024 | SingleElementMoved |     4,455.3 ns |    852.64 ns |   378.58 ns |  0.26 |    0.02 |    5 |         - |          NA |
| SpinSortVariant          | 1024 | SingleElementMoved |     4,032.8 ns |  1,863.40 ns |   827.36 ns |  0.24 |    0.05 |    5 |         - |          NA |
| Glidesort                | 1024 | SingleElementMoved |     2,599.5 ns |     15.38 ns |     5.48 ns |  0.15 |    0.00 |    4 |         - |          NA |
| FlatStableSort           | 1024 | SingleElementMoved |     5,599.3 ns |    434.95 ns |   227.49 ns |  0.33 |    0.01 |    6 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **1024** | **Sorted**             |    **15,618.4 ns** |     **67.27 ns** |    **29.87 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Sorted             |    19,641.1 ns |    243.55 ns |   127.38 ns |  1.26 |    0.01 |    9 |         - |          NA |
| BottomupMergeSort        | 1024 | Sorted             |     6,041.5 ns |    267.75 ns |   140.04 ns |  0.39 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 1024 | Sorted             |     7,417.7 ns |     79.77 ns |    35.42 ns |  0.47 |    0.00 |    7 |         - |          NA |
| RotateMergeSort          | 1024 | Sorted             |     1,341.7 ns |      2.35 ns |     1.23 ns |  0.09 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Sorted             |     1,864.0 ns |     29.48 ns |    10.51 ns |  0.12 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 1024 | Sorted             |     1,311.7 ns |      1.15 ns |     0.41 ns |  0.08 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | Sorted             |    14,360.2 ns |    133.70 ns |    69.93 ns |  0.92 |    0.00 |    8 |         - |          NA |
| NaturalMergeSort         | 1024 | Sorted             |       703.8 ns |    333.89 ns |   174.63 ns |  0.05 |    0.01 |    1 |         - |          NA |
| TimSort                  | 1024 | Sorted             |       563.8 ns |      3.20 ns |     1.67 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Sorted             |       521.7 ns |      3.61 ns |     1.60 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Sorted             |       712.7 ns |      4.34 ns |     1.93 ns |  0.05 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 1024 | Sorted             |       458.9 ns |      5.67 ns |     2.52 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 1024 | Sorted             |       656.3 ns |      1.00 ns |     0.44 ns |  0.04 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Sorted             |       516.3 ns |     13.82 ns |     7.23 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Sorted             |     4,838.0 ns |      6.83 ns |     2.44 ns |  0.31 |    0.00 |    5 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **1024** | **Reversed**           |    **36,104.1 ns** |    **685.47 ns** |   **358.52 ns** |  **1.00** |    **0.01** |    **5** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Reversed           |    33,084.8 ns |    236.09 ns |   123.48 ns |  0.92 |    0.01 |    5 |         - |          NA |
| BottomupMergeSort        | 1024 | Reversed           |    20,287.6 ns |    501.44 ns |   262.26 ns |  0.56 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 1024 | Reversed           |    10,833.8 ns |    663.34 ns |   346.94 ns |  0.30 |    0.01 |    3 |         - |          NA |
| RotateMergeSort          | 1024 | Reversed           |     8,542.1 ns |    282.03 ns |   147.51 ns |  0.24 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Reversed           |     9,044.4 ns |    371.03 ns |   194.05 ns |  0.25 |    0.01 |    2 |         - |          NA |
| SymMergeSort             | 1024 | Reversed           |     8,537.9 ns |    320.98 ns |   167.88 ns |  0.24 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 1024 | Reversed           |    39,735.7 ns |    151.08 ns |    79.02 ns |  1.10 |    0.01 |    5 |         - |          NA |
| NaturalMergeSort         | 1024 | Reversed           |     1,042.3 ns |      9.76 ns |     5.10 ns |  0.03 |    0.00 |    1 |         - |          NA |
| TimSort                  | 1024 | Reversed           |       848.9 ns |     16.06 ns |     5.73 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Reversed           |       813.4 ns |      6.44 ns |     2.86 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Reversed           |       830.8 ns |      3.28 ns |     1.72 ns |  0.02 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 1024 | Reversed           |       957.2 ns |      3.78 ns |     1.35 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 1024 | Reversed           |     1,058.5 ns |      3.09 ns |     1.10 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Reversed           |       860.2 ns |      8.17 ns |     3.63 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Reversed           |    12,421.7 ns |    364.86 ns |   162.00 ns |  0.34 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **1024** | **PipeOrgan**          |    **26,596.9 ns** |    **476.04 ns** |   **248.98 ns** |  **1.00** |    **0.01** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | PipeOrgan          |    26,913.1 ns |    411.74 ns |   215.35 ns |  1.01 |    0.01 |    8 |         - |          NA |
| BottomupMergeSort        | 1024 | PipeOrgan          |    14,149.1 ns |    215.63 ns |   112.78 ns |  0.53 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 1024 | PipeOrgan          |     9,756.7 ns |    540.26 ns |   282.57 ns |  0.37 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 1024 | PipeOrgan          |    18,144.8 ns |    298.09 ns |   155.90 ns |  0.68 |    0.01 |    7 |         - |          NA |
| RotateMergeSortRecursive | 1024 | PipeOrgan          |    21,276.2 ns |    224.65 ns |   117.50 ns |  0.80 |    0.01 |    7 |         - |          NA |
| SymMergeSort             | 1024 | PipeOrgan          |    11,458.7 ns |    461.76 ns |   205.02 ns |  0.43 |    0.01 |    5 |         - |          NA |
| BlockMergeSort           | 1024 | PipeOrgan          |    32,485.1 ns |    332.66 ns |   173.99 ns |  1.22 |    0.01 |    9 |         - |          NA |
| NaturalMergeSort         | 1024 | PipeOrgan          |     2,386.7 ns |     18.71 ns |     8.31 ns |  0.09 |    0.00 |    3 |         - |          NA |
| TimSort                  | 1024 | PipeOrgan          |     2,536.9 ns |      4.96 ns |     1.77 ns |  0.10 |    0.00 |    3 |         - |          NA |
| PowerSort                | 1024 | PipeOrgan          |     1,612.8 ns |     18.56 ns |     6.62 ns |  0.06 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | PipeOrgan          |     1,938.3 ns |      3.58 ns |     1.28 ns |  0.07 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 1024 | PipeOrgan          |     8,110.2 ns |    432.03 ns |   191.82 ns |  0.30 |    0.01 |    5 |         - |          NA |
| SpinSortVariant          | 1024 | PipeOrgan          |     7,939.5 ns |    379.90 ns |   198.70 ns |  0.30 |    0.01 |    5 |         - |          NA |
| Glidesort                | 1024 | PipeOrgan          |     4,484.6 ns |    293.78 ns |   153.65 ns |  0.17 |    0.01 |    4 |         - |          NA |
| FlatStableSort           | 1024 | PipeOrgan          |     8,972.4 ns |    508.68 ns |   266.05 ns |  0.34 |    0.01 |    5 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **8192** | **Random**             |   **681,671.9 ns** | **14,831.11 ns** | **6,585.11 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Random             |   614,164.3 ns |  4,861.47 ns | 2,542.64 ns |  0.90 |    0.01 |    1 |         - |          NA |
| BottomupMergeSort        | 8192 | Random             |   497,121.4 ns |  3,907.67 ns | 2,043.79 ns |  0.73 |    0.01 |    1 |         - |          NA |
| StdStableSort            | 8192 | Random             |   471,507.8 ns |    604.71 ns |   268.49 ns |  0.69 |    0.01 |    1 |         - |          NA |
| RotateMergeSort          | 8192 | Random             | 1,340,747.6 ns |  3,401.61 ns | 1,779.11 ns |  1.97 |    0.02 |    3 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Random             | 1,463,192.1 ns |  3,286.39 ns | 1,459.18 ns |  2.15 |    0.02 |    3 |         - |          NA |
| SymMergeSort             | 8192 | Random             | 1,015,847.2 ns |  5,559.79 ns | 2,907.88 ns |  1.49 |    0.01 |    2 |         - |          NA |
| BlockMergeSort           | 8192 | Random             |   704,271.2 ns |  8,642.89 ns | 4,520.40 ns |  1.03 |    0.01 |    1 |         - |          NA |
| NaturalMergeSort         | 8192 | Random             |   620,657.9 ns |  4,359.95 ns | 1,935.85 ns |  0.91 |    0.01 |    1 |         - |          NA |
| TimSort                  | 8192 | Random             |   565,839.0 ns |  1,713.26 ns |   896.07 ns |  0.83 |    0.01 |    1 |         - |          NA |
| PowerSort                | 8192 | Random             |   426,719.3 ns |  1,363.36 ns |   605.34 ns |  0.63 |    0.01 |    1 |         - |          NA |
| ShiftSort                | 8192 | Random             |   560,273.8 ns |  1,935.32 ns | 1,012.21 ns |  0.82 |    0.01 |    1 |         - |          NA |
| SpinSort                 | 8192 | Random             |   370,752.3 ns |  2,406.03 ns | 1,258.40 ns |  0.54 |    0.01 |    1 |         - |          NA |
| SpinSortVariant          | 8192 | Random             |   370,221.4 ns |  1,712.73 ns |   760.46 ns |  0.54 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Random             |   585,069.8 ns |  3,736.11 ns | 1,954.06 ns |  0.86 |    0.01 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Random             |   396,032.8 ns |  2,041.59 ns | 1,067.79 ns |  0.58 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **8192** | **SingleElementMoved** |   **135,096.8 ns** |    **432.81 ns** |   **226.37 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | SingleElementMoved |   166,913.6 ns |  1,280.48 ns |   669.72 ns |  1.24 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 8192 | SingleElementMoved |    55,458.8 ns |  1,507.79 ns |   788.60 ns |  0.41 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 8192 | SingleElementMoved |   110,197.8 ns |    979.97 ns |   512.54 ns |  0.82 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 8192 | SingleElementMoved |    14,137.4 ns |    463.40 ns |   205.75 ns |  0.10 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 8192 | SingleElementMoved |    17,581.9 ns |    234.82 ns |    83.74 ns |  0.13 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 8192 | SingleElementMoved |    13,130.5 ns |    325.07 ns |   170.02 ns |  0.10 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 8192 | SingleElementMoved |   150,145.6 ns |  1,094.20 ns |   572.29 ns |  1.11 |    0.00 |    6 |         - |          NA |
| NaturalMergeSort         | 8192 | SingleElementMoved |    15,647.5 ns |    259.62 ns |   115.27 ns |  0.12 |    0.00 |    3 |         - |          NA |
| TimSort                  | 8192 | SingleElementMoved |     5,697.2 ns |    385.31 ns |   201.53 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | SingleElementMoved |    10,841.6 ns |  1,094.95 ns |   486.16 ns |  0.08 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 8192 | SingleElementMoved |    10,517.2 ns |    518.62 ns |   230.27 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | SingleElementMoved |    23,335.4 ns |  1,010.79 ns |   528.67 ns |  0.17 |    0.00 |    3 |         - |          NA |
| SpinSortVariant          | 8192 | SingleElementMoved |    19,831.8 ns |    617.90 ns |   323.17 ns |  0.15 |    0.00 |    3 |         - |          NA |
| Glidesort                | 8192 | SingleElementMoved |    20,824.1 ns |  1,064.92 ns |   556.97 ns |  0.15 |    0.00 |    3 |         - |          NA |
| FlatStableSort           | 8192 | SingleElementMoved |    46,178.9 ns |    875.50 ns |   457.90 ns |  0.34 |    0.00 |    4 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **8192** | **Sorted**             |   **124,744.6 ns** |    **825.17 ns** |   **431.58 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Sorted             |   159,466.8 ns |    721.43 ns |   377.32 ns |  1.28 |    0.01 |    7 |         - |          NA |
| BottomupMergeSort        | 8192 | Sorted             |    44,840.7 ns |    606.30 ns |   317.11 ns |  0.36 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 8192 | Sorted             |   105,818.6 ns |  1,353.13 ns |   600.80 ns |  0.85 |    0.01 |    6 |         - |          NA |
| RotateMergeSort          | 8192 | Sorted             |    11,016.6 ns |    752.50 ns |   334.12 ns |  0.09 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Sorted             |    15,860.4 ns |  1,084.02 ns |   566.96 ns |  0.13 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 8192 | Sorted             |    10,620.0 ns |    430.32 ns |   191.07 ns |  0.09 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 8192 | Sorted             |   111,327.3 ns |  1,068.21 ns |   558.70 ns |  0.89 |    0.01 |    6 |         - |          NA |
| NaturalMergeSort         | 8192 | Sorted             |     4,101.2 ns |      8.11 ns |     4.24 ns |  0.03 |    0.00 |    1 |         - |          NA |
| TimSort                  | 8192 | Sorted             |     4,085.1 ns |      9.00 ns |     3.21 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Sorted             |     4,018.2 ns |     16.34 ns |     5.83 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Sorted             |     5,496.8 ns |    646.01 ns |   286.83 ns |  0.04 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | Sorted             |     3,495.5 ns |     27.62 ns |    12.27 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 8192 | Sorted             |     5,565.6 ns |    550.39 ns |   244.38 ns |  0.04 |    0.00 |    2 |         - |          NA |
| Glidesort                | 8192 | Sorted             |     3,393.2 ns |     66.17 ns |    23.60 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Sorted             |     3,670.0 ns |     37.38 ns |    19.55 ns |  0.03 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **8192** | **Reversed**           |   **305,313.4 ns** |  **3,042.11 ns** | **1,591.08 ns** |  **1.00** |    **0.01** |    **5** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Reversed           |   273,623.5 ns |    751.20 ns |   333.54 ns |  0.90 |    0.00 |    5 |         - |          NA |
| BottomupMergeSort        | 8192 | Reversed           |   170,596.1 ns |  3,646.19 ns | 1,907.03 ns |  0.56 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 8192 | Reversed           |   131,001.7 ns |  3,487.96 ns | 1,824.27 ns |  0.43 |    0.01 |    3 |         - |          NA |
| RotateMergeSort          | 8192 | Reversed           |    83,078.4 ns |    296.68 ns |   131.73 ns |  0.27 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Reversed           |    87,768.5 ns |    670.50 ns |   350.69 ns |  0.29 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 8192 | Reversed           |    74,962.3 ns |  1,232.81 ns |   547.37 ns |  0.25 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 8192 | Reversed           |   337,622.3 ns |  2,005.14 ns | 1,048.73 ns |  1.11 |    0.01 |    5 |         - |          NA |
| NaturalMergeSort         | 8192 | Reversed           |     7,866.3 ns |    315.19 ns |   164.85 ns |  0.03 |    0.00 |    1 |         - |          NA |
| TimSort                  | 8192 | Reversed           |     6,493.2 ns |    415.27 ns |   217.19 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Reversed           |     6,669.1 ns |    347.74 ns |   181.87 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Reversed           |     6,039.8 ns |     68.75 ns |    24.52 ns |  0.02 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 8192 | Reversed           |     7,805.6 ns |    572.17 ns |   254.05 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SpinSortVariant          | 8192 | Reversed           |     8,960.9 ns |  1,276.97 ns |   566.98 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Reversed           |     6,475.8 ns |    404.81 ns |   211.72 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Reversed           |     7,132.4 ns |    340.61 ns |   178.14 ns |  0.02 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **8192** | **PipeOrgan**          |   **219,560.0 ns** |  **2,042.97 ns** | **1,068.52 ns** |  **1.00** |    **0.01** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | PipeOrgan          |   222,860.9 ns |  2,254.70 ns | 1,179.25 ns |  1.02 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 8192 | PipeOrgan          |   115,305.2 ns |  1,717.75 ns |   898.42 ns |  0.53 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 8192 | PipeOrgan          |   126,838.7 ns |  1,565.43 ns |   695.06 ns |  0.58 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 8192 | PipeOrgan          |   159,710.1 ns |  1,248.62 ns |   653.05 ns |  0.73 |    0.00 |    6 |         - |          NA |
| RotateMergeSortRecursive | 8192 | PipeOrgan          |   186,286.0 ns |  1,390.57 ns |   727.29 ns |  0.85 |    0.00 |    6 |         - |          NA |
| SymMergeSort             | 8192 | PipeOrgan          |    98,944.7 ns |  1,842.00 ns |   963.40 ns |  0.45 |    0.00 |    5 |         - |          NA |
| BlockMergeSort           | 8192 | PipeOrgan          |   249,790.6 ns |    846.33 ns |   442.65 ns |  1.14 |    0.01 |    6 |         - |          NA |
| NaturalMergeSort         | 8192 | PipeOrgan          |    19,007.5 ns |    952.03 ns |   422.71 ns |  0.09 |    0.00 |    2 |         - |          NA |
| TimSort                  | 8192 | PipeOrgan          |    19,032.9 ns |    670.61 ns |   350.74 ns |  0.09 |    0.00 |    2 |         - |          NA |
| PowerSort                | 8192 | PipeOrgan          |    12,249.5 ns |    371.74 ns |   132.57 ns |  0.06 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | PipeOrgan          |    15,521.7 ns |    938.40 ns |   416.65 ns |  0.07 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | PipeOrgan          |    17,667.9 ns |    690.58 ns |   361.19 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSortVariant          | 8192 | PipeOrgan          |    20,343.0 ns |  1,549.53 ns |   810.43 ns |  0.09 |    0.00 |    2 |         - |          NA |
| Glidesort                | 8192 | PipeOrgan          |    35,013.2 ns |  1,483.68 ns |   775.99 ns |  0.16 |    0.00 |    3 |         - |          NA |
| FlatStableSort           | 8192 | PipeOrgan          |    78,796.5 ns |    922.75 ns |   409.71 ns |  0.36 |    0.00 |    4 |         - |          NA |

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

| Method                  | Size | Pattern            | Mean       | Error      | StdDev    | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------------ |----- |------------------- |-----------:|-----------:|----------:|------:|--------:|-----:|----------:|------------:|
| **BitonicSort**             | **256**  | **Random**             |  **11.268 μs** |  **0.3253 μs** | **0.1701 μs** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Random             |  22.390 μs |  0.0917 μs | 0.0407 μs |  1.99 |    0.03 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Random             |  16.551 μs |  0.0780 μs | 0.0346 μs |  1.47 |    0.02 |    2 |         - |          NA |
|      |                    |            |            |           |       |         |      |           |             |
| **BitonicSort**             | **256**  | **SingleElementMoved** |  **10.274 μs** |  **0.7669 μs** | **0.4011 μs** |  **1.00** |    **0.05** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | SingleElementMoved |  23.565 μs |  1.5800 μs | 0.8264 μs |  2.30 |    0.11 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | SingleElementMoved |  16.717 μs |  0.1169 μs | 0.0519 μs |  1.63 |    0.06 |    2 |         - |          NA |
|      |                    |            |            |           |       |         |      |           |             |
| **BitonicSort**             | **256**  | **Sorted**             |  **10.144 μs** |  **0.5632 μs** | **0.2946 μs** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Sorted             |  22.904 μs |  0.0712 μs | 0.0373 μs |  2.26 |    0.06 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Sorted             |  16.808 μs |  0.0956 μs | 0.0425 μs |  1.66 |    0.05 |    2 |         - |          NA |
|      |                    |            |            |           |       |         |      |           |             |
| **BitonicSort**             | **256**  | **Reversed**           |   **9.978 μs** |  **0.6018 μs** | **0.3147 μs** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Reversed           |  22.930 μs |  0.2739 μs | 0.1433 μs |  2.30 |    0.07 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Reversed           |  16.678 μs |  0.0845 μs | 0.0442 μs |  1.67 |    0.05 |    2 |         - |          NA |
|      |                    |            |            |           |       |         |      |           |             |
| **BitonicSort**             | **256**  | **PipeOrgan**          |   **9.114 μs** |  **0.3550 μs** | **0.1576 μs** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | PipeOrgan          |  22.546 μs |  0.3700 μs | 0.1935 μs |  2.47 |    0.04 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | PipeOrgan          |  16.788 μs |  0.1797 μs | 0.0940 μs |  1.84 |    0.03 |    2 |         - |          NA |
|      |                    |            |            |           |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Random**             |  **94.725 μs** |  **0.8437 μs** | **0.3746 μs** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Random             | 123.715 μs |  1.2721 μs | 0.5648 μs |  1.31 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Random             | 102.424 μs |  0.3761 μs | 0.1967 μs |  1.08 |    0.00 |    1 |         - |          NA |
|      |                    |            |            |           |       |         |      |           |             |
| **BitonicSort**             | **1024** | **SingleElementMoved** |  **59.151 μs** |  **1.0467 μs** | **0.5475 μs** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | SingleElementMoved | 118.490 μs |  0.1770 μs | 0.0926 μs |  2.00 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | SingleElementMoved | 102.681 μs |  0.3849 μs | 0.2013 μs |  1.74 |    0.02 |    2 |         - |          NA |
|      |                    |            |            |           |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Sorted**             |  **58.749 μs** |  **1.1200 μs** | **0.4973 μs** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Sorted             | 118.904 μs |  0.4637 μs | 0.2425 μs |  2.02 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Sorted             | 102.674 μs |  0.2819 μs | 0.1252 μs |  1.75 |    0.01 |    2 |         - |          NA |
|      |                    |            |            |           |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Reversed**           |  **57.743 μs** |  **1.7308 μs** | **0.9052 μs** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Reversed           | 118.132 μs |  0.2696 μs | 0.1197 μs |  2.05 |    0.03 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Reversed           | 102.690 μs |  0.4768 μs | 0.2494 μs |  1.78 |    0.03 |    2 |         - |          NA |
|      |                    |            |            |           |       |         |      |           |             |
| **BitonicSort**             | **1024** | **PipeOrgan**          |  **54.358 μs** |  **2.9026 μs** | **1.5181 μs** |  **1.00** |    **0.04** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | PipeOrgan          | 116.602 μs |  0.4347 μs | 0.1930 μs |  2.15 |    0.06 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | PipeOrgan          | 102.586 μs |  0.3197 μs | 0.1419 μs |  1.89 |    0.05 |    2 |         - |          NA |
|      |                    |            |            |           |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Random**             | **545.281 μs** |  **2.0520 μs** | **1.0733 μs** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Random             | 816.868 μs |  1.2047 μs | 0.5349 μs |  1.50 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Random             | 637.722 μs |  1.2561 μs | 0.6570 μs |  1.17 |    0.00 |    1 |         - |          NA |
|      |                    |            |            |           |       |         |      |           |             |
| **BitonicSort**             | **4096** | **SingleElementMoved** | **322.317 μs** |  **4.0696 μs** | **2.1285 μs** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | SingleElementMoved | 591.249 μs |  0.9140 μs | 0.4058 μs |  1.83 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | SingleElementMoved | 586.030 μs |  2.0033 μs | 1.0477 μs |  1.82 |    0.01 |    2 |         - |          NA |
|      |                    |            |            |           |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Sorted**             | **320.595 μs** |  **5.8558 μs** | **3.0627 μs** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Sorted             | 591.077 μs |  1.6362 μs | 0.7265 μs |  1.84 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Sorted             | 585.106 μs |  0.4424 μs | 0.1964 μs |  1.83 |    0.02 |    2 |         - |          NA |
|      |                    |            |            |           |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Reversed**           | **315.559 μs** |  **4.2972 μs** | **1.9080 μs** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Reversed           | 590.274 μs |  4.2156 μs | 2.2048 μs |  1.87 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Reversed           | 585.212 μs |  0.7193 μs | 0.3762 μs |  1.85 |    0.01 |    2 |         - |          NA |
|      |                    |            |            |           |       |         |      |           |             |
| **BitonicSort**             | **4096** | **PipeOrgan**          | **300.010 μs** | **10.4409 μs** | **5.4608 μs** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | PipeOrgan          | 579.525 μs |  0.8963 μs | 0.3980 μs |  1.93 |    0.03 |    2 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | PipeOrgan          | 585.047 μs |  0.5566 μs | 0.2471 μs |  1.95 |    0.03 |    2 |         - |          NA |

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

| Method                       | Size | Pattern            | Mean           | Error       | StdDev      | Median         | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------------- |----- |------------------- |---------------:|------------:|------------:|---------------:|------:|--------:|-----:|----------:|------------:|
| **QuickSort**                    | **256**  | **Random**             |     **2,615.3 ns** |   **153.79 ns** |    **68.28 ns** |     **2,582.8 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 256  | Random             |     3,101.6 ns |    62.61 ns |    22.33 ns |     3,097.4 ns |  1.19 |    0.03 |    1 |         - |          NA |
| QuickSortMedian3             | 256  | Random             |     4,590.0 ns |   533.00 ns |   278.77 ns |     4,508.4 ns |  1.76 |    0.11 |    2 |         - |          NA |
| QuickSortMedian9             | 256  | Random             |     3,833.6 ns |   306.65 ns |   160.39 ns |     3,747.9 ns |  1.47 |    0.07 |    2 |         - |          NA |
| DualPivotQuickSort           | 256  | Random             |     2,395.9 ns |   545.55 ns |   285.33 ns |     2,206.7 ns |  0.92 |    0.11 |    1 |         - |          NA |
| StableQuickSort              | 256  | Random             |    11,979.9 ns |   351.57 ns |   183.88 ns |    11,953.1 ns |  4.58 |    0.13 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Random             |     6,806.4 ns |    88.93 ns |    39.49 ns |     6,792.1 ns |  2.60 |    0.06 |    3 |         - |          NA |
| DestswapStableQuickSort      | 256  | Random             |     8,774.0 ns |   541.42 ns |   283.17 ns |     8,700.8 ns |  3.36 |    0.13 |    4 |         - |          NA |
| IntroSort                    | 256  | Random             |     1,947.9 ns |    24.14 ns |    10.72 ns |     1,944.2 ns |  0.75 |    0.02 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | Random             |     1,611.0 ns |    14.63 ns |     6.49 ns |     1,612.4 ns |  0.62 |    0.01 |    1 |         - |          NA |
| PDQSort                      | 256  | Random             |     1,814.7 ns |   160.63 ns |    84.01 ns |     1,857.1 ns |  0.69 |    0.03 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | Random             |     2,955.5 ns |    44.97 ns |    19.97 ns |     2,959.9 ns |  1.13 |    0.03 |    1 |         - |          NA |
| StdSort                      | 256  | Random             |     1,711.3 ns |    54.41 ns |    28.46 ns |     1,702.8 ns |  0.65 |    0.02 |    1 |         - |          NA |
| BlockQuickSort               | 256  | Random             |     2,549.7 ns | 1,000.22 ns |   444.11 ns |     2,332.1 ns |  0.98 |    0.16 |    1 |         - |          NA |
| DotnetSort                   | 256  | Random             |     1,848.1 ns |    30.27 ns |    13.44 ns |     1,843.9 ns |  0.71 |    0.02 |    1 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **QuickSort**                    | **256**  | **SingleElementMoved** |     **1,230.1 ns** |    **22.92 ns** |    **10.18 ns** |     **1,226.3 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 256  | SingleElementMoved |     5,308.2 ns |   401.99 ns |   210.25 ns |     5,274.3 ns |  4.32 |    0.16 |    3 |         - |          NA |
| QuickSortMedian3             | 256  | SingleElementMoved |     5,172.5 ns |   304.51 ns |   159.27 ns |     5,064.7 ns |  4.21 |    0.13 |    3 |         - |          NA |
| QuickSortMedian9             | 256  | SingleElementMoved |     4,208.7 ns |   163.36 ns |    72.53 ns |     4,195.0 ns |  3.42 |    0.06 |    3 |         - |          NA |
| DualPivotQuickSort           | 256  | SingleElementMoved |     3,618.5 ns |    63.58 ns |    22.67 ns |     3,613.1 ns |  2.94 |    0.03 |    3 |         - |          NA |
| StableQuickSort              | 256  | SingleElementMoved |     8,800.5 ns |   442.70 ns |   231.54 ns |     8,740.6 ns |  7.15 |    0.19 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 256  | SingleElementMoved |     4,949.5 ns |   428.75 ns |   224.24 ns |     4,949.3 ns |  4.02 |    0.17 |    3 |         - |          NA |
| DestswapStableQuickSort      | 256  | SingleElementMoved |    10,314.7 ns |   428.67 ns |   190.33 ns |    10,258.4 ns |  8.39 |    0.16 |    4 |         - |          NA |
| IntroSort                    | 256  | SingleElementMoved |       867.0 ns |    19.52 ns |     8.67 ns |       866.4 ns |  0.70 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | SingleElementMoved |     1,077.0 ns |    10.70 ns |     5.60 ns |     1,076.3 ns |  0.88 |    0.01 |    2 |         - |          NA |
| PDQSort                      | 256  | SingleElementMoved |     1,101.9 ns |    25.95 ns |     9.26 ns |     1,101.0 ns |  0.90 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 256  | SingleElementMoved |     1,455.4 ns |    27.58 ns |    12.24 ns |     1,455.2 ns |  1.18 |    0.01 |    2 |         - |          NA |
| StdSort                      | 256  | SingleElementMoved |     1,678.9 ns |   321.33 ns |   142.67 ns |     1,620.5 ns |  1.36 |    0.11 |    2 |         - |          NA |
| BlockQuickSort               | 256  | SingleElementMoved |     1,579.8 ns |   166.60 ns |    73.97 ns |     1,609.4 ns |  1.28 |    0.06 |    2 |         - |          NA |
| DotnetSort                   | 256  | SingleElementMoved |     1,061.5 ns |   103.69 ns |    54.23 ns |     1,071.5 ns |  0.86 |    0.04 |    2 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Sorted**             |       **870.4 ns** |     **9.00 ns** |     **4.00 ns** |       **869.0 ns** |  **1.00** |    **0.01** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 256  | Sorted             |     6,794.4 ns |    48.72 ns |    17.37 ns |     6,792.6 ns |  7.81 |    0.04 |    7 |         - |          NA |
| QuickSortMedian3             | 256  | Sorted             |     6,527.8 ns |   277.65 ns |   123.28 ns |     6,569.4 ns |  7.50 |    0.14 |    7 |         - |          NA |
| QuickSortMedian9             | 256  | Sorted             |     4,823.1 ns |   548.19 ns |   286.71 ns |     4,715.5 ns |  5.54 |    0.31 |    6 |         - |          NA |
| DualPivotQuickSort           | 256  | Sorted             |     4,044.5 ns |    34.76 ns |    12.40 ns |     4,041.0 ns |  4.65 |    0.02 |    6 |         - |          NA |
| StableQuickSort              | 256  | Sorted             |     8,799.8 ns |   286.86 ns |   127.37 ns |     8,824.5 ns | 10.11 |    0.14 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Sorted             |     4,860.9 ns |   429.55 ns |   224.66 ns |     4,820.4 ns |  5.58 |    0.24 |    6 |         - |          NA |
| DestswapStableQuickSort      | 256  | Sorted             |    10,059.5 ns |   334.37 ns |   174.88 ns |    10,122.4 ns | 11.56 |    0.20 |    8 |         - |          NA |
| IntroSort                    | 256  | Sorted             |       386.4 ns |   246.89 ns |   129.13 ns |       299.5 ns |  0.44 |    0.14 |    2 |         - |          NA |
| IntroSortDotnet              | 256  | Sorted             |     1,026.2 ns |    42.54 ns |    22.25 ns |     1,023.0 ns |  1.18 |    0.02 |    5 |         - |          NA |
| PDQSort                      | 256  | Sorted             |       389.3 ns |    52.97 ns |    23.52 ns |       375.4 ns |  0.45 |    0.03 |    3 |         - |          NA |
| PDQSortBranchless            | 256  | Sorted             |       372.9 ns |     2.57 ns |     1.14 ns |       372.4 ns |  0.43 |    0.00 |    1 |         - |          NA |
| StdSort                      | 256  | Sorted             |       491.1 ns |     2.90 ns |     1.29 ns |       490.4 ns |  0.56 |    0.00 |    4 |         - |          NA |
| BlockQuickSort               | 256  | Sorted             |     1,209.2 ns |     6.70 ns |     3.50 ns |     1,208.8 ns |  1.39 |    0.01 |    5 |         - |          NA |
| DotnetSort                   | 256  | Sorted             |       867.3 ns |    14.54 ns |     5.18 ns |       867.2 ns |  1.00 |    0.01 |    5 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Reversed**           |       **976.5 ns** |    **30.08 ns** |    **13.35 ns** |       **980.9 ns** |  **1.00** |    **0.02** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 256  | Reversed           |     5,140.8 ns |   372.39 ns |   194.77 ns |     5,019.7 ns |  5.27 |    0.20 |    6 |         - |          NA |
| QuickSortMedian3             | 256  | Reversed           |     7,174.2 ns |   129.64 ns |    57.56 ns |     7,142.8 ns |  7.35 |    0.11 |    7 |         - |          NA |
| QuickSortMedian9             | 256  | Reversed           |     4,949.7 ns |   349.41 ns |   182.75 ns |     4,861.4 ns |  5.07 |    0.19 |    6 |         - |          NA |
| DualPivotQuickSort           | 256  | Reversed           |     3,798.2 ns |    48.40 ns |    21.49 ns |     3,792.4 ns |  3.89 |    0.05 |    5 |         - |          NA |
| StableQuickSort              | 256  | Reversed           |     8,689.9 ns |   351.64 ns |   183.91 ns |     8,678.3 ns |  8.90 |    0.21 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Reversed           |     4,953.2 ns |   385.40 ns |   201.57 ns |     4,914.1 ns |  5.07 |    0.21 |    6 |         - |          NA |
| DestswapStableQuickSort      | 256  | Reversed           |    10,074.9 ns |   426.55 ns |   223.10 ns |    10,034.8 ns | 10.32 |    0.25 |    8 |         - |          NA |
| IntroSort                    | 256  | Reversed           |       564.7 ns |    34.05 ns |    17.81 ns |       557.1 ns |  0.58 |    0.02 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | Reversed           |     1,445.3 ns |    19.74 ns |    10.33 ns |     1,443.8 ns |  1.48 |    0.02 |    4 |         - |          NA |
| PDQSort                      | 256  | Reversed           |       552.3 ns |     7.62 ns |     3.38 ns |       551.0 ns |  0.57 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | Reversed           |       909.1 ns |     9.16 ns |     4.79 ns |       908.9 ns |  0.93 |    0.01 |    3 |         - |          NA |
| StdSort                      | 256  | Reversed           |       728.5 ns |   226.48 ns |   100.56 ns |       709.0 ns |  0.75 |    0.10 |    2 |         - |          NA |
| BlockQuickSort               | 256  | Reversed           |     1,445.2 ns |     4.52 ns |     2.36 ns |     1,444.6 ns |  1.48 |    0.02 |    4 |         - |          NA |
| DotnetSort                   | 256  | Reversed           |     1,559.6 ns |   103.14 ns |    45.80 ns |     1,545.8 ns |  1.60 |    0.05 |    4 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **QuickSort**                    | **256**  | **PipeOrgan**          |     **7,934.0 ns** |   **446.44 ns** |   **198.22 ns** |     **8,014.8 ns** |  **1.00** |    **0.03** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 256  | PipeOrgan          |     5,176.1 ns |   388.20 ns |   203.03 ns |     5,045.6 ns |  0.65 |    0.03 |    3 |         - |          NA |
| QuickSortMedian3             | 256  | PipeOrgan          |     6,443.0 ns |   101.16 ns |    36.07 ns |     6,436.1 ns |  0.81 |    0.02 |    4 |         - |          NA |
| QuickSortMedian9             | 256  | PipeOrgan          |     4,223.7 ns |   387.38 ns |   202.61 ns |     4,118.1 ns |  0.53 |    0.03 |    3 |         - |          NA |
| DualPivotQuickSort           | 256  | PipeOrgan          |     1,702.0 ns |    39.31 ns |    17.45 ns |     1,693.6 ns |  0.21 |    0.01 |    1 |         - |          NA |
| StableQuickSort              | 256  | PipeOrgan          |     9,037.3 ns |    80.16 ns |    28.59 ns |     9,026.1 ns |  1.14 |    0.03 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 256  | PipeOrgan          |     5,098.2 ns |   808.54 ns |   422.88 ns |     5,157.9 ns |  0.64 |    0.05 |    3 |         - |          NA |
| DestswapStableQuickSort      | 256  | PipeOrgan          |    10,695.1 ns |   489.45 ns |   255.99 ns |    10,719.1 ns |  1.35 |    0.04 |    5 |         - |          NA |
| IntroSort                    | 256  | PipeOrgan          |     1,671.0 ns |    88.47 ns |    39.28 ns |     1,655.4 ns |  0.21 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | PipeOrgan          |     2,202.0 ns |    67.81 ns |    24.18 ns |     2,199.9 ns |  0.28 |    0.01 |    1 |         - |          NA |
| PDQSort                      | 256  | PipeOrgan          |     1,937.3 ns |   149.58 ns |    53.34 ns |     1,929.9 ns |  0.24 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | PipeOrgan          |     3,048.9 ns |    47.56 ns |    21.12 ns |     3,037.2 ns |  0.38 |    0.01 |    2 |         - |          NA |
| StdSort                      | 256  | PipeOrgan          |     2,222.5 ns |   259.36 ns |   115.16 ns |     2,201.0 ns |  0.28 |    0.02 |    1 |         - |          NA |
| BlockQuickSort               | 256  | PipeOrgan          |     4,376.6 ns |   601.29 ns |   314.49 ns |     4,187.2 ns |  0.55 |    0.04 |    3 |         - |          NA |
| DotnetSort                   | 256  | PipeOrgan          |     2,783.8 ns |    69.36 ns |    30.80 ns |     2,782.3 ns |  0.35 |    0.01 |    2 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Random**             |    **13,453.6 ns** |   **452.51 ns** |   **236.67 ns** |    **13,441.9 ns** |  **1.00** |    **0.02** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Random             |    20,281.3 ns | 1,920.36 ns | 1,004.39 ns |    20,354.3 ns |  1.51 |    0.07 |    2 |         - |          NA |
| QuickSortMedian3             | 1024 | Random             |    23,183.9 ns |   406.83 ns |   180.63 ns |    23,214.2 ns |  1.72 |    0.03 |    2 |         - |          NA |
| QuickSortMedian9             | 1024 | Random             |    23,517.5 ns | 3,332.54 ns | 1,742.98 ns |    23,680.0 ns |  1.75 |    0.13 |    2 |         - |          NA |
| DualPivotQuickSort           | 1024 | Random             |    10,255.5 ns |   311.41 ns |   162.88 ns |    10,286.9 ns |  0.76 |    0.02 |    1 |         - |          NA |
| StableQuickSort              | 1024 | Random             |    84,801.1 ns |   258.08 ns |    92.03 ns |    84,798.6 ns |  6.30 |    0.11 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Random             |    35,721.0 ns |   368.82 ns |   163.76 ns |    35,705.6 ns |  2.66 |    0.05 |    3 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Random             |    39,675.5 ns |   748.74 ns |   332.45 ns |    39,610.7 ns |  2.95 |    0.05 |    3 |         - |          NA |
| IntroSort                    | 1024 | Random             |    11,307.1 ns |   413.02 ns |   183.39 ns |    11,355.6 ns |  0.84 |    0.02 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | Random             |     9,107.9 ns |   595.47 ns |   311.44 ns |     9,091.8 ns |  0.68 |    0.02 |    1 |         - |          NA |
| PDQSort                      | 1024 | Random             |     9,163.4 ns |   362.91 ns |   189.81 ns |     9,277.9 ns |  0.68 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | Random             |    13,963.0 ns |   258.53 ns |   135.21 ns |    13,933.2 ns |  1.04 |    0.02 |    1 |         - |          NA |
| StdSort                      | 1024 | Random             |     8,897.6 ns |   464.39 ns |   242.89 ns |     8,893.5 ns |  0.66 |    0.02 |    1 |         - |          NA |
| BlockQuickSort               | 1024 | Random             |    12,854.7 ns |   124.19 ns |    55.14 ns |    12,880.5 ns |  0.96 |    0.02 |    1 |         - |          NA |
| DotnetSort                   | 1024 | Random             |    10,724.9 ns |   405.81 ns |   212.24 ns |    10,746.9 ns |  0.80 |    0.02 |    1 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **QuickSort**                    | **1024** | **SingleElementMoved** |     **5,697.5 ns** |    **96.68 ns** |    **42.93 ns** |     **5,712.6 ns** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 1024 | SingleElementMoved |    39,250.4 ns |   554.69 ns |   290.11 ns |    39,179.8 ns |  6.89 |    0.07 |    4 |         - |          NA |
| QuickSortMedian3             | 1024 | SingleElementMoved |    31,705.6 ns |   272.36 ns |   142.45 ns |    31,685.5 ns |  5.57 |    0.05 |    3 |         - |          NA |
| QuickSortMedian9             | 1024 | SingleElementMoved |    21,852.3 ns |   732.95 ns |   325.44 ns |    21,748.6 ns |  3.84 |    0.06 |    2 |         - |          NA |
| DualPivotQuickSort           | 1024 | SingleElementMoved |    21,150.3 ns |   493.26 ns |   257.99 ns |    21,094.5 ns |  3.71 |    0.05 |    2 |         - |          NA |
| StableQuickSort              | 1024 | SingleElementMoved |    42,307.4 ns |   437.58 ns |   228.86 ns |    42,296.0 ns |  7.43 |    0.06 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | SingleElementMoved |    22,635.3 ns |   369.49 ns |   164.05 ns |    22,596.4 ns |  3.97 |    0.04 |    2 |         - |          NA |
| DestswapStableQuickSort      | 1024 | SingleElementMoved |    42,552.0 ns |   367.50 ns |   163.17 ns |    42,622.5 ns |  7.47 |    0.06 |    4 |         - |          NA |
| IntroSort                    | 1024 | SingleElementMoved |     4,147.9 ns |    51.16 ns |    22.72 ns |     4,146.1 ns |  0.73 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | SingleElementMoved |     6,346.5 ns |   159.86 ns |    83.61 ns |     6,377.7 ns |  1.11 |    0.02 |    1 |         - |          NA |
| PDQSort                      | 1024 | SingleElementMoved |     5,007.4 ns |   287.64 ns |   150.44 ns |     4,930.1 ns |  0.88 |    0.03 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | SingleElementMoved |     6,463.2 ns |   483.45 ns |   252.86 ns |     6,597.4 ns |  1.13 |    0.04 |    1 |         - |          NA |
| StdSort                      | 1024 | SingleElementMoved |     6,749.8 ns |    45.98 ns |    20.42 ns |     6,741.3 ns |  1.18 |    0.01 |    1 |         - |          NA |
| BlockQuickSort               | 1024 | SingleElementMoved |     7,635.8 ns |    72.36 ns |    25.81 ns |     7,637.3 ns |  1.34 |    0.01 |    1 |         - |          NA |
| DotnetSort                   | 1024 | SingleElementMoved |     5,716.4 ns |   610.53 ns |   319.32 ns |     5,592.3 ns |  1.00 |    0.05 |    1 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Sorted**             |     **4,100.4 ns** |    **34.77 ns** |    **12.40 ns** |     **4,105.5 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Sorted             |    52,709.5 ns |   234.51 ns |   122.65 ns |    52,723.3 ns | 12.85 |    0.05 |    7 |         - |          NA |
| QuickSortMedian3             | 1024 | Sorted             |    43,132.3 ns |   154.68 ns |    68.68 ns |    43,158.2 ns | 10.52 |    0.03 |    6 |         - |          NA |
| QuickSortMedian9             | 1024 | Sorted             |    22,485.0 ns | 1,210.79 ns |   633.27 ns |    22,261.6 ns |  5.48 |    0.15 |    5 |         - |          NA |
| DualPivotQuickSort           | 1024 | Sorted             |    21,580.5 ns |   353.30 ns |   156.87 ns |    21,567.9 ns |  5.26 |    0.04 |    5 |         - |          NA |
| StableQuickSort              | 1024 | Sorted             |    42,867.4 ns |   501.60 ns |   262.34 ns |    42,878.7 ns | 10.45 |    0.07 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Sorted             |    22,950.5 ns |   879.51 ns |   460.00 ns |    23,078.3 ns |  5.60 |    0.11 |    5 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Sorted             |    43,083.0 ns |   272.82 ns |   142.69 ns |    43,120.5 ns | 10.51 |    0.04 |    6 |         - |          NA |
| IntroSort                    | 1024 | Sorted             |     1,314.7 ns |   353.94 ns |   185.12 ns |     1,344.5 ns |  0.32 |    0.04 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | Sorted             |     4,809.3 ns |    40.35 ns |    14.39 ns |     4,807.1 ns |  1.17 |    0.00 |    3 |         - |          NA |
| PDQSort                      | 1024 | Sorted             |     1,327.2 ns |    12.77 ns |     4.56 ns |     1,325.2 ns |  0.32 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | Sorted             |     1,325.1 ns |     4.34 ns |     1.93 ns |     1,325.9 ns |  0.32 |    0.00 |    1 |         - |          NA |
| StdSort                      | 1024 | Sorted             |     1,800.8 ns |     2.71 ns |     0.97 ns |     1,800.7 ns |  0.44 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | Sorted             |     6,270.6 ns |    98.05 ns |    43.54 ns |     6,251.4 ns |  1.53 |    0.01 |    4 |         - |          NA |
| DotnetSort                   | 1024 | Sorted             |     4,281.0 ns |   412.18 ns |   215.58 ns |     4,145.0 ns |  1.04 |    0.05 |    3 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Reversed**           |     **4,594.7 ns** |    **17.98 ns** |     **7.98 ns** |     **4,596.8 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Reversed           |    38,465.9 ns |   410.24 ns |   214.56 ns |    38,471.1 ns |  8.37 |    0.05 |    7 |         - |          NA |
| QuickSortMedian3             | 1024 | Reversed           |    52,598.0 ns | 1,152.03 ns |   602.53 ns |    52,577.9 ns | 11.45 |    0.13 |    8 |         - |          NA |
| QuickSortMedian9             | 1024 | Reversed           |    22,737.5 ns |   394.14 ns |   175.00 ns |    22,762.4 ns |  4.95 |    0.04 |    6 |         - |          NA |
| DualPivotQuickSort           | 1024 | Reversed           |    19,963.9 ns |   206.80 ns |    91.82 ns |    19,991.9 ns |  4.34 |    0.02 |    6 |         - |          NA |
| StableQuickSort              | 1024 | Reversed           |    42,492.0 ns |   334.12 ns |   148.35 ns |    42,459.9 ns |  9.25 |    0.03 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Reversed           |    22,997.8 ns |   634.48 ns |   281.71 ns |    23,101.2 ns |  5.01 |    0.06 |    6 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Reversed           |    43,154.9 ns |   307.63 ns |   136.59 ns |    43,198.6 ns |  9.39 |    0.03 |    7 |         - |          NA |
| IntroSort                    | 1024 | Reversed           |     3,174.1 ns |   602.17 ns |   314.95 ns |     2,969.9 ns |  0.69 |    0.06 |    3 |         - |          NA |
| IntroSortDotnet              | 1024 | Reversed           |     7,643.8 ns |   503.86 ns |   263.53 ns |     7,591.1 ns |  1.66 |    0.05 |    5 |         - |          NA |
| PDQSort                      | 1024 | Reversed           |     1,902.8 ns |    15.21 ns |     5.42 ns |     1,902.7 ns |  0.41 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | Reversed           |     3,046.7 ns |    13.80 ns |     6.13 ns |     3,049.6 ns |  0.66 |    0.00 |    3 |         - |          NA |
| StdSort                      | 1024 | Reversed           |     2,456.5 ns |   213.89 ns |    76.27 ns |     2,496.4 ns |  0.53 |    0.02 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | Reversed           |     7,338.7 ns |    42.97 ns |    19.08 ns |     7,345.4 ns |  1.60 |    0.00 |    5 |         - |          NA |
| DotnetSort                   | 1024 | Reversed           |     8,655.5 ns |   632.03 ns |   280.63 ns |     8,693.7 ns |  1.88 |    0.06 |    5 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **QuickSort**                    | **1024** | **PipeOrgan**          |    **97,986.6 ns** |   **307.88 ns** |   **161.02 ns** |    **97,997.6 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| QuickSort3way                | 1024 | PipeOrgan          |    31,484.7 ns |   668.67 ns |   349.73 ns |    31,424.4 ns |  0.32 |    0.00 |    4 |         - |          NA |
| QuickSortMedian3             | 1024 | PipeOrgan          |    37,463.3 ns |   240.37 ns |    85.72 ns |    37,493.8 ns |  0.38 |    0.00 |    4 |         - |          NA |
| QuickSortMedian9             | 1024 | PipeOrgan          |    22,268.3 ns |   483.26 ns |   252.75 ns |    22,217.5 ns |  0.23 |    0.00 |    3 |         - |          NA |
| DualPivotQuickSort           | 1024 | PipeOrgan          |     9,233.7 ns |   511.60 ns |   267.58 ns |     9,253.3 ns |  0.09 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 1024 | PipeOrgan          |    45,624.4 ns |   430.64 ns |   191.21 ns |    45,645.9 ns |  0.47 |    0.00 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | PipeOrgan          |    22,574.1 ns |   393.12 ns |   174.55 ns |    22,654.6 ns |  0.23 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 1024 | PipeOrgan          |    48,981.0 ns |   452.56 ns |   236.70 ns |    49,000.5 ns |  0.50 |    0.00 |    5 |         - |          NA |
| IntroSort                    | 1024 | PipeOrgan          |    11,516.1 ns | 1,666.98 ns |   871.86 ns |    11,180.7 ns |  0.12 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 1024 | PipeOrgan          |    13,753.7 ns |   406.33 ns |   212.52 ns |    13,742.2 ns |  0.14 |    0.00 |    2 |         - |          NA |
| PDQSort                      | 1024 | PipeOrgan          |     8,768.0 ns |   293.89 ns |   130.49 ns |     8,812.6 ns |  0.09 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | PipeOrgan          |    15,964.3 ns |   288.87 ns |   151.08 ns |    15,969.3 ns |  0.16 |    0.00 |    2 |         - |          NA |
| StdSort                      | 1024 | PipeOrgan          |    13,232.4 ns |   291.59 ns |   129.47 ns |    13,257.4 ns |  0.14 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | PipeOrgan          |    23,072.0 ns |   368.20 ns |   163.48 ns |    23,029.4 ns |  0.24 |    0.00 |    3 |         - |          NA |
| DotnetSort                   | 1024 | PipeOrgan          |    16,960.0 ns |   414.41 ns |   216.75 ns |    16,912.8 ns |  0.17 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Random**             |   **422,918.7 ns** | **4,733.19 ns** | **2,101.57 ns** |   **422,804.5 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Random             |   440,370.5 ns | 6,387.69 ns | 2,836.18 ns |   440,037.2 ns |  1.04 |    0.01 |    2 |         - |          NA |
| QuickSortMedian3             | 8192 | Random             |   533,053.6 ns | 2,749.37 ns | 1,220.74 ns |   533,269.2 ns |  1.26 |    0.01 |    2 |         - |          NA |
| QuickSortMedian9             | 8192 | Random             |   516,706.3 ns | 2,143.90 ns |   951.90 ns |   517,176.7 ns |  1.22 |    0.01 |    2 |         - |          NA |
| DualPivotQuickSort           | 8192 | Random             |   349,276.5 ns |   762.23 ns |   398.66 ns |   349,257.1 ns |  0.83 |    0.00 |    2 |         - |          NA |
| StableQuickSort              | 8192 | Random             | 1,162,673.6 ns | 1,028.04 ns |   456.45 ns | 1,162,715.2 ns |  2.75 |    0.01 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Random             |   942,750.6 ns | 2,522.02 ns | 1,319.07 ns |   942,680.2 ns |  2.23 |    0.01 |    3 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Random             |   840,616.8 ns |   820.04 ns |   364.10 ns |   840,552.7 ns |  1.99 |    0.01 |    3 |         - |          NA |
| IntroSort                    | 8192 | Random             |   365,398.4 ns | 2,514.91 ns | 1,315.35 ns |   365,227.8 ns |  0.86 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet              | 8192 | Random             |   355,969.2 ns | 2,617.89 ns | 1,162.36 ns |   355,876.1 ns |  0.84 |    0.00 |    2 |         - |          NA |
| PDQSort                      | 8192 | Random             |   344,336.8 ns | 1,473.40 ns |   770.62 ns |   344,709.5 ns |  0.81 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | Random             |   194,440.6 ns | 2,545.10 ns | 1,331.14 ns |   194,215.2 ns |  0.46 |    0.00 |    1 |         - |          NA |
| StdSort                      | 8192 | Random             |   337,953.0 ns | 1,302.73 ns |   681.35 ns |   338,281.5 ns |  0.80 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | Random             |   421,313.2 ns | 1,312.87 ns |   582.92 ns |   421,442.8 ns |  1.00 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 8192 | Random             |   335,688.9 ns | 2,282.98 ns | 1,194.04 ns |   336,120.9 ns |  0.79 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **QuickSort**                    | **8192** | **SingleElementMoved** |    **54,119.2 ns** | **1,185.94 ns** |   **526.56 ns** |    **53,861.8 ns** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 8192 | SingleElementMoved |   855,505.8 ns | 2,451.74 ns | 1,088.59 ns |   855,003.8 ns | 15.81 |    0.14 |    8 |         - |          NA |
| QuickSortMedian3             | 8192 | SingleElementMoved |   570,875.5 ns | 3,561.50 ns | 1,581.33 ns |   571,262.9 ns | 10.55 |    0.10 |    7 |         - |          NA |
| QuickSortMedian9             | 8192 | SingleElementMoved |   211,507.4 ns | 3,741.64 ns | 1,956.95 ns |   211,425.5 ns |  3.91 |    0.05 |    5 |         - |          NA |
| DualPivotQuickSort           | 8192 | SingleElementMoved |   140,585.4 ns | 2,580.53 ns | 1,349.67 ns |   141,014.6 ns |  2.60 |    0.03 |    4 |         - |          NA |
| StableQuickSort              | 8192 | SingleElementMoved |   433,567.6 ns |   907.20 ns |   323.52 ns |   433,615.9 ns |  8.01 |    0.07 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | SingleElementMoved |   228,987.7 ns | 4,697.24 ns | 2,456.75 ns |   229,364.7 ns |  4.23 |    0.06 |    5 |         - |          NA |
| DestswapStableQuickSort      | 8192 | SingleElementMoved |   377,831.3 ns |   829.26 ns |   368.20 ns |   377,713.7 ns |  6.98 |    0.06 |    6 |         - |          NA |
| IntroSort                    | 8192 | SingleElementMoved |    41,449.1 ns | 3,229.52 ns | 1,433.93 ns |    41,387.1 ns |  0.77 |    0.03 |    1 |         - |          NA |
| IntroSortDotnet              | 8192 | SingleElementMoved |    60,922.9 ns | 1,159.39 ns |   514.78 ns |    60,674.9 ns |  1.13 |    0.01 |    2 |         - |          NA |
| PDQSort                      | 8192 | SingleElementMoved |    43,024.0 ns | 1,618.31 ns |   846.41 ns |    42,663.1 ns |  0.80 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless            | 8192 | SingleElementMoved |    54,124.6 ns | 1,679.56 ns |   878.44 ns |    53,886.0 ns |  1.00 |    0.02 |    2 |         - |          NA |
| StdSort                      | 8192 | SingleElementMoved |    62,840.4 ns | 1,902.58 ns |   995.09 ns |    62,867.0 ns |  1.16 |    0.02 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | SingleElementMoved |    79,536.2 ns |   205.55 ns |    91.27 ns |    79,558.5 ns |  1.47 |    0.01 |    3 |         - |          NA |
| DotnetSort                   | 8192 | SingleElementMoved |    57,649.9 ns | 3,525.09 ns | 1,843.69 ns |    57,538.8 ns |  1.07 |    0.03 |    2 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Sorted**             |    **41,900.3 ns** |   **628.92 ns** |   **279.24 ns** |    **41,882.8 ns** |  **1.00** |    **0.01** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Sorted             | 1,171,861.8 ns | 6,248.85 ns | 3,268.27 ns | 1,171,826.7 ns | 27.97 |    0.19 |   10 |         - |          NA |
| QuickSortMedian3             | 8192 | Sorted             |   889,426.0 ns | 7,528.74 ns | 3,342.81 ns |   888,059.9 ns | 21.23 |    0.15 |    9 |         - |          NA |
| QuickSortMedian9             | 8192 | Sorted             |   211,200.3 ns | 4,965.80 ns | 2,597.21 ns |   211,700.4 ns |  5.04 |    0.07 |    7 |         - |          NA |
| DualPivotQuickSort           | 8192 | Sorted             |   152,173.6 ns | 3,336.09 ns | 1,744.84 ns |   152,100.3 ns |  3.63 |    0.05 |    6 |         - |          NA |
| StableQuickSort              | 8192 | Sorted             |   431,933.1 ns |   582.37 ns |   258.58 ns |   431,872.0 ns | 10.31 |    0.06 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Sorted             |   225,645.5 ns | 3,356.24 ns | 1,755.38 ns |   225,331.8 ns |  5.39 |    0.05 |    7 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Sorted             |   378,600.2 ns | 1,597.33 ns |   835.44 ns |   378,314.8 ns |  9.04 |    0.06 |    8 |         - |          NA |
| IntroSort                    | 8192 | Sorted             |     8,526.2 ns |   382.37 ns |   199.99 ns |     8,451.4 ns |  0.20 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet              | 8192 | Sorted             |    47,983.3 ns | 2,139.32 ns | 1,118.90 ns |    47,466.5 ns |  1.15 |    0.03 |    4 |         - |          NA |
| PDQSort                      | 8192 | Sorted             |    10,656.7 ns |   776.78 ns |   344.90 ns |    10,781.6 ns |  0.25 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | Sorted             |    10,407.7 ns |   547.11 ns |   286.15 ns |    10,217.2 ns |  0.25 |    0.01 |    2 |         - |          NA |
| StdSort                      | 8192 | Sorted             |    14,801.2 ns | 1,158.94 ns |   514.58 ns |    14,648.5 ns |  0.35 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 8192 | Sorted             |    68,479.3 ns |   414.96 ns |   184.25 ns |    68,416.8 ns |  1.63 |    0.01 |    5 |         - |          NA |
| DotnetSort                   | 8192 | Sorted             |    45,088.6 ns | 4,180.60 ns | 2,186.54 ns |    45,281.6 ns |  1.08 |    0.05 |    4 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Reversed**           |    **46,149.4 ns** | **1,412.55 ns** |   **627.18 ns** |    **46,103.9 ns** |  **1.00** |    **0.02** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Reversed           |   835,268.6 ns | 3,113.19 ns | 1,628.26 ns |   835,963.5 ns | 18.10 |    0.23 |   10 |         - |          NA |
| QuickSortMedian3             | 8192 | Reversed           | 1,125,021.3 ns | 3,312.41 ns | 1,470.73 ns | 1,125,040.3 ns | 24.38 |    0.31 |   11 |         - |          NA |
| QuickSortMedian9             | 8192 | Reversed           |   206,840.1 ns | 4,172.57 ns | 2,182.33 ns |   206,873.6 ns |  4.48 |    0.07 |    8 |         - |          NA |
| DualPivotQuickSort           | 8192 | Reversed           |   143,357.2 ns | 1,137.80 ns |   505.19 ns |   143,611.7 ns |  3.11 |    0.04 |    7 |         - |          NA |
| StableQuickSort              | 8192 | Reversed           |   432,914.7 ns | 2,367.29 ns | 1,051.09 ns |   432,460.3 ns |  9.38 |    0.12 |    9 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Reversed           |   225,702.8 ns | 2,485.70 ns | 1,300.07 ns |   225,519.5 ns |  4.89 |    0.07 |    8 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Reversed           |   383,414.7 ns |   991.20 ns |   440.10 ns |   383,499.0 ns |  8.31 |    0.11 |    9 |         - |          NA |
| IntroSort                    | 8192 | Reversed           |    24,705.4 ns | 2,232.94 ns | 1,167.87 ns |    24,228.8 ns |  0.54 |    0.02 |    3 |         - |          NA |
| IntroSortDotnet              | 8192 | Reversed           |    76,795.0 ns |   798.11 ns |   354.36 ns |    76,729.7 ns |  1.66 |    0.02 |    5 |         - |          NA |
| PDQSort                      | 8192 | Reversed           |    14,739.4 ns |   373.83 ns |   165.98 ns |    14,794.0 ns |  0.32 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 8192 | Reversed           |    23,422.9 ns |   940.06 ns |   417.39 ns |    23,388.4 ns |  0.51 |    0.01 |    3 |         - |          NA |
| StdSort                      | 8192 | Reversed           |    19,018.5 ns |   821.04 ns |   364.55 ns |    19,018.2 ns |  0.41 |    0.01 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | Reversed           |    75,497.7 ns |   903.19 ns |   401.02 ns |    75,379.5 ns |  1.64 |    0.02 |    5 |         - |          NA |
| DotnetSort                   | 8192 | Reversed           |    98,765.0 ns | 3,113.97 ns | 1,628.67 ns |    98,355.2 ns |  2.14 |    0.04 |    6 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **QuickSort**                    | **8192** | **PipeOrgan**          | **5,387,472.3 ns** | **6,726.82 ns** | **2,398.85 ns** | **5,386,420.1 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 8192 | PipeOrgan          |   452,706.6 ns | 2,070.25 ns | 1,082.78 ns |   452,931.6 ns |  0.08 |    0.00 |    4 |         - |          NA |
| QuickSortMedian3             | 8192 | PipeOrgan          |   495,279.3 ns | 1,873.63 ns |   979.95 ns |   495,505.6 ns |  0.09 |    0.00 |    4 |         - |          NA |
| QuickSortMedian9             | 8192 | PipeOrgan          |   279,656.4 ns | 1,800.00 ns |   799.21 ns |   279,536.0 ns |  0.05 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort           | 8192 | PipeOrgan          |   122,575.2 ns | 1,247.45 ns |   553.88 ns |   122,651.7 ns |  0.02 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 8192 | PipeOrgan          |   468,279.2 ns | 1,722.29 ns |   764.71 ns |   468,454.7 ns |  0.09 |    0.00 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | PipeOrgan          |   223,817.4 ns | 2,592.52 ns | 1,151.09 ns |   223,815.7 ns |  0.04 |    0.00 |    2 |         - |          NA |
| DestswapStableQuickSort      | 8192 | PipeOrgan          |   458,622.7 ns | 1,730.33 ns |   904.99 ns |   458,534.0 ns |  0.09 |    0.00 |    4 |         - |          NA |
| IntroSort                    | 8192 | PipeOrgan          |   266,971.4 ns | 9,401.22 ns | 4,917.02 ns |   268,256.0 ns |  0.05 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet              | 8192 | PipeOrgan          |   360,375.4 ns | 1,871.59 ns |   978.88 ns |   360,463.9 ns |  0.07 |    0.00 |    3 |         - |          NA |
| PDQSort                      | 8192 | PipeOrgan          |   116,104.8 ns | 4,335.88 ns | 2,267.75 ns |   115,433.5 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 8192 | PipeOrgan          |   201,656.6 ns | 1,858.24 ns |   971.90 ns |   201,961.1 ns |  0.04 |    0.00 |    2 |         - |          NA |
| StdSort                      | 8192 | PipeOrgan          |   292,260.5 ns | 3,954.56 ns | 2,068.31 ns |   292,801.5 ns |  0.05 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | PipeOrgan          |   257,062.6 ns | 1,527.75 ns |   799.04 ns |   257,254.2 ns |  0.05 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 8192 | PipeOrgan          |   369,232.0 ns | 2,632.59 ns | 1,168.88 ns |   369,136.4 ns |  0.07 |    0.00 |    3 |         - |          NA |

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

| Method              | Size | Pattern            | Mean        | Error     | StdDev    | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------------- |----- |------------------- |------------:|----------:|----------:|------:|--------:|-----:|----------:|------------:|
| **SelectionSort**       | **256**  | **Random**             |    **28.15 μs** |  **1.702 μs** |  **0.890 μs** |  **1.00** |    **0.04** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Random             |    19.65 μs |  0.343 μs |  0.179 μs |  0.70 |    0.02 |    1 |         - |          NA |
| CycleSort           | 256  | Random             |    90.55 μs |  3.429 μs |  1.523 μs |  3.22 |    0.11 |    4 |         - |          NA |
| PancakeSort         | 256  | Random             |    43.57 μs |  0.834 μs |  0.436 μs |  1.55 |    0.05 |    3 |         - |          NA |
|      |                    |             |           |           |       |         |      |           |             |
| **SelectionSort**       | **256**  | **SingleElementMoved** |    **22.07 μs** |  **0.173 μs** |  **0.091 μs** |  **1.00** |    **0.01** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | SingleElementMoved |    16.30 μs |  0.788 μs |  0.350 μs |  0.74 |    0.02 |    1 |         - |          NA |
| CycleSort           | 256  | SingleElementMoved |    50.52 μs |  1.853 μs |  0.969 μs |  2.29 |    0.04 |    3 |         - |          NA |
| PancakeSort         | 256  | SingleElementMoved |    19.88 μs |  0.267 μs |  0.140 μs |  0.90 |    0.01 |    2 |         - |          NA |
|      |                    |             |           |           |       |         |      |           |             |
| **SelectionSort**       | **256**  | **Sorted**             |    **22.06 μs** |  **0.247 μs** |  **0.129 μs** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Sorted             |    11.33 μs |  0.678 μs |  0.301 μs |  0.51 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | Sorted             |    31.93 μs |  0.106 μs |  0.047 μs |  1.45 |    0.01 |    4 |         - |          NA |
| PancakeSort         | 256  | Sorted             |    15.29 μs |  0.213 μs |  0.111 μs |  0.69 |    0.01 |    2 |         - |          NA |
|      |                    |             |           |           |       |         |      |           |             |
| **SelectionSort**       | **256**  | **Reversed**           |    **21.52 μs** |  **2.713 μs** |  **1.419 μs** |  **1.00** |    **0.09** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Reversed           |    16.34 μs |  0.175 μs |  0.078 μs |  0.76 |    0.05 |    1 |         - |          NA |
| CycleSort           | 256  | Reversed           |    44.29 μs |  0.510 μs |  0.267 μs |  2.07 |    0.13 |    2 |         - |          NA |
| PancakeSort         | 256  | Reversed           |    18.71 μs |  5.152 μs |  2.694 μs |  0.87 |    0.13 |    1 |         - |          NA |
|      |                    |             |           |           |       |         |      |           |             |
| **SelectionSort**       | **256**  | **PipeOrgan**          |    **24.74 μs** |  **0.501 μs** |  **0.262 μs** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | PipeOrgan          |    21.73 μs |  0.405 μs |  0.212 μs |  0.88 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | PipeOrgan          |    61.72 μs |  3.081 μs |  1.611 μs |  2.49 |    0.07 |    3 |         - |          NA |
| PancakeSort         | 256  | PipeOrgan          |    33.33 μs |  0.107 μs |  0.047 μs |  1.35 |    0.01 |    2 |         - |          NA |
|      |                    |             |           |           |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Random**             |   **368.40 μs** |  **1.269 μs** |  **0.664 μs** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Random             |   275.54 μs |  1.147 μs |  0.600 μs |  0.75 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | Random             | 1,658.27 μs |  6.020 μs |  2.673 μs |  4.50 |    0.01 |    4 |         - |          NA |
| PancakeSort         | 1024 | Random             |   621.72 μs |  1.513 μs |  0.540 μs |  1.69 |    0.00 |    3 |         - |          NA |
|      |                    |             |           |           |       |         |      |           |             |
| **SelectionSort**       | **1024** | **SingleElementMoved** |   **333.19 μs** |  **0.967 μs** |  **0.429 μs** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | SingleElementMoved |   247.30 μs |  5.258 μs |  2.750 μs |  0.74 |    0.01 |    1 |         - |          NA |
| CycleSort           | 1024 | SingleElementMoved |   765.82 μs |  5.662 μs |  2.961 μs |  2.30 |    0.01 |    2 |         - |          NA |
| PancakeSort         | 1024 | SingleElementMoved |   293.40 μs |  4.486 μs |  1.992 μs |  0.88 |    0.01 |    1 |         - |          NA |
|      |                    |             |           |           |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Sorted**             |   **332.67 μs** |  **0.967 μs** |  **0.506 μs** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Sorted             |   166.79 μs |  0.301 μs |  0.107 μs |  0.50 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | Sorted             |   495.27 μs |  1.680 μs |  0.746 μs |  1.49 |    0.00 |    4 |         - |          NA |
| PancakeSort         | 1024 | Sorted             |   224.37 μs |  1.085 μs |  0.482 μs |  0.67 |    0.00 |    2 |         - |          NA |
|      |                    |             |           |           |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Reversed**           |   **316.93 μs** | **24.999 μs** | **13.075 μs** |  **1.00** |    **0.06** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Reversed           |   248.74 μs |  0.847 μs |  0.443 μs |  0.79 |    0.03 |    1 |         - |          NA |
| CycleSort           | 1024 | Reversed           |   665.56 μs |  1.950 μs |  1.020 μs |  2.10 |    0.08 |    3 |         - |          NA |
| PancakeSort         | 1024 | Reversed           |   313.85 μs |  4.823 μs |  2.141 μs |  0.99 |    0.04 |    2 |         - |          NA |
|      |                    |             |           |           |       |         |      |           |             |
| **SelectionSort**       | **1024** | **PipeOrgan**          |   **347.48 μs** |  **1.937 μs** |  **1.013 μs** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | PipeOrgan          |   310.67 μs |  0.618 μs |  0.323 μs |  0.89 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | PipeOrgan          |   911.43 μs | 23.023 μs | 12.042 μs |  2.62 |    0.03 |    3 |         - |          NA |
| PancakeSort         | 1024 | PipeOrgan          |   500.33 μs |  0.515 μs |  0.184 μs |  1.44 |    0.00 |    2 |         - |          NA |

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

| Method             | Size | Pattern            | Mean            | Error        | StdDev     | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------- |----- |------------------- |----------------:|-------------:|-----------:|------:|--------:|-----:|----------:|------------:|
| **QuickSort**          | **256**  | **Random**             |       **188.06 μs** |     **3.373 μs** |   **1.764 μs** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| QuickSort3way      | 256  | Random             |       158.57 μs |     2.581 μs |   1.350 μs |  0.84 |    0.01 |    1 |         - |          NA |
| QuickSortMedian3   | 256  | Random             |       170.04 μs |    13.864 μs |   6.156 μs |  0.90 |    0.03 |    1 |         - |          NA |
| QuickSortMedian9   | 256  | Random             |       176.17 μs |     0.951 μs |   0.422 μs |  0.94 |    0.01 |    1 |         - |          NA |
| DualPivotQuickSort | 256  | Random             |       199.22 μs |     0.828 μs |   0.433 μs |  1.06 |    0.01 |    1 |         - |          NA |
| StableQuickSort    | 256  | Random             |       301.11 μs |     3.753 μs |   1.963 μs |  1.60 |    0.02 |    2 |         - |          NA |
| IntroSort          | 256  | Random             |       176.04 μs |     1.099 μs |   0.575 μs |  0.94 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | Random             |       158.82 μs |     1.792 μs |   0.937 μs |  0.84 |    0.01 |    1 |         - |          NA |
| PDQSort            | 256  | Random             |       216.29 μs |     2.264 μs |   1.005 μs |  1.15 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | Random             |       210.66 μs |     2.263 μs |   1.183 μs |  1.12 |    0.01 |    1 |         - |          NA |
| StdSort            | 256  | Random             |       193.13 μs |     3.540 μs |   1.851 μs |  1.03 |    0.01 |    1 |         - |          NA |
| BlockQuickSort     | 256  | Random             |       173.33 μs |     2.319 μs |   1.213 μs |  0.92 |    0.01 |    1 |         - |          NA |
| DotnetSort         | 256  | Random             |       163.19 μs |     6.752 μs |   2.998 μs |  0.87 |    0.02 |    1 |         - |          NA |
|      |                    |                 |              |            |       |         |      |           |             |
| **QuickSort**          | **256**  | **SingleElementMoved** |       **128.31 μs** |     **2.400 μs** |   **1.255 μs** |  **1.00** |    **0.01** |    **1** |         **-** |          **NA** |
| QuickSort3way      | 256  | SingleElementMoved |       200.86 μs |     0.880 μs |   0.460 μs |  1.57 |    0.01 |    2 |         - |          NA |
| QuickSortMedian3   | 256  | SingleElementMoved |       175.49 μs |     1.497 μs |   0.783 μs |  1.37 |    0.01 |    2 |         - |          NA |
| QuickSortMedian9   | 256  | SingleElementMoved |       168.80 μs |     2.669 μs |   1.396 μs |  1.32 |    0.02 |    2 |         - |          NA |
| DualPivotQuickSort | 256  | SingleElementMoved |       308.40 μs |     1.769 μs |   0.785 μs |  2.40 |    0.02 |    3 |         - |          NA |
| StableQuickSort    | 256  | SingleElementMoved |       237.63 μs |     0.793 μs |   0.352 μs |  1.85 |    0.02 |    2 |         - |          NA |
| IntroSort          | 256  | SingleElementMoved |        88.69 μs |     0.296 μs |   0.155 μs |  0.69 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | SingleElementMoved |       112.16 μs |     1.285 μs |   0.672 μs |  0.87 |    0.01 |    1 |         - |          NA |
| PDQSort            | 256  | SingleElementMoved |       120.50 μs |     1.146 μs |   0.599 μs |  0.94 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | SingleElementMoved |       122.34 μs |     1.603 μs |   0.838 μs |  0.95 |    0.01 |    1 |         - |          NA |
| StdSort            | 256  | SingleElementMoved |       127.04 μs |     2.739 μs |   1.216 μs |  0.99 |    0.01 |    1 |         - |          NA |
| BlockQuickSort     | 256  | SingleElementMoved |       104.01 μs |     0.317 μs |   0.141 μs |  0.81 |    0.01 |    1 |         - |          NA |
| DotnetSort         | 256  | SingleElementMoved |       114.33 μs |     4.005 μs |   2.094 μs |  0.89 |    0.02 |    1 |         - |          NA |
|      |                    |                 |              |            |       |         |      |           |             |
| **QuickSort**          | **256**  | **Sorted**             |       **112.96 μs** |     **0.494 μs** |   **0.259 μs** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 256  | Sorted             |       260.17 μs |     0.897 μs |   0.320 μs |  2.30 |    0.01 |    4 |         - |          NA |
| QuickSortMedian3   | 256  | Sorted             |       223.39 μs |     2.354 μs |   1.045 μs |  1.98 |    0.01 |    4 |         - |          NA |
| QuickSortMedian9   | 256  | Sorted             |       178.73 μs |     1.689 μs |   0.883 μs |  1.58 |    0.01 |    3 |         - |          NA |
| DualPivotQuickSort | 256  | Sorted             |       394.28 μs |     1.686 μs |   0.882 μs |  3.49 |    0.01 |    5 |         - |          NA |
| StableQuickSort    | 256  | Sorted             |       241.20 μs |     1.732 μs |   0.906 μs |  2.14 |    0.01 |    4 |         - |          NA |
| IntroSort          | 256  | Sorted             |        35.14 μs |     0.265 μs |   0.118 μs |  0.31 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | Sorted             |        89.01 μs |     2.331 μs |   1.219 μs |  0.79 |    0.01 |    2 |         - |          NA |
| PDQSort            | 256  | Sorted             |        36.57 μs |     0.580 μs |   0.303 μs |  0.32 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | Sorted             |        36.39 μs |     0.304 μs |   0.159 μs |  0.32 |    0.00 |    1 |         - |          NA |
| StdSort            | 256  | Sorted             |        40.34 μs |     0.563 μs |   0.294 μs |  0.36 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 256  | Sorted             |        99.96 μs |     0.804 μs |   0.420 μs |  0.88 |    0.00 |    2 |         - |          NA |
| DotnetSort         | 256  | Sorted             |        87.99 μs |     1.604 μs |   0.839 μs |  0.78 |    0.01 |    2 |         - |          NA |
|      |                    |                 |              |            |       |         |      |           |             |
| **QuickSort**          | **256**  | **Reversed**           |       **110.40 μs** |     **0.791 μs** |   **0.414 μs** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 256  | Reversed           |       197.60 μs |     3.019 μs |   1.579 μs |  1.79 |    0.01 |    4 |         - |          NA |
| QuickSortMedian3   | 256  | Reversed           |       260.22 μs |     2.085 μs |   1.091 μs |  2.36 |    0.01 |    5 |         - |          NA |
| QuickSortMedian9   | 256  | Reversed           |       185.08 μs |     0.756 μs |   0.395 μs |  1.68 |    0.01 |    4 |         - |          NA |
| DualPivotQuickSort | 256  | Reversed           |       347.85 μs |     1.217 μs |   0.541 μs |  3.15 |    0.01 |    6 |         - |          NA |
| StableQuickSort    | 256  | Reversed           |       249.05 μs |     1.764 μs |   0.783 μs |  2.26 |    0.01 |    5 |         - |          NA |
| IntroSort          | 256  | Reversed           |        59.09 μs |     0.742 μs |   0.388 μs |  0.54 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | Reversed           |       139.56 μs |     4.316 μs |   2.257 μs |  1.26 |    0.02 |    3 |         - |          NA |
| PDQSort            | 256  | Reversed           |        54.06 μs |     0.764 μs |   0.339 μs |  0.49 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | Reversed           |        53.26 μs |     0.834 μs |   0.436 μs |  0.48 |    0.00 |    1 |         - |          NA |
| StdSort            | 256  | Reversed           |        54.98 μs |     0.373 μs |   0.166 μs |  0.50 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 256  | Reversed           |        93.81 μs |     0.809 μs |   0.423 μs |  0.85 |    0.00 |    2 |         - |          NA |
| DotnetSort         | 256  | Reversed           |       135.27 μs |     2.321 μs |   1.214 μs |  1.23 |    0.01 |    3 |         - |          NA |
|      |                    |                 |              |            |       |         |      |           |             |
| **QuickSort**          | **256**  | **PipeOrgan**          |     **1,099.69 μs** |     **5.346 μs** |   **2.374 μs** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 256  | PipeOrgan          |       216.47 μs |     1.676 μs |   0.877 μs |  0.20 |    0.00 |    1 |         - |          NA |
| QuickSortMedian3   | 256  | PipeOrgan          |       248.80 μs |     3.862 μs |   2.020 μs |  0.23 |    0.00 |    1 |         - |          NA |
| QuickSortMedian9   | 256  | PipeOrgan          |       161.20 μs |     1.513 μs |   0.792 μs |  0.15 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort | 256  | PipeOrgan          |       169.74 μs |     1.214 μs |   0.539 μs |  0.15 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 256  | PipeOrgan          |       254.55 μs |     3.977 μs |   2.080 μs |  0.23 |    0.00 |    1 |         - |          NA |
| IntroSort          | 256  | PipeOrgan          |       149.69 μs |     1.174 μs |   0.521 μs |  0.14 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 256  | PipeOrgan          |       256.85 μs |     1.641 μs |   0.858 μs |  0.23 |    0.00 |    1 |         - |          NA |
| PDQSort            | 256  | PipeOrgan          |       204.41 μs |     3.357 μs |   1.756 μs |  0.19 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 256  | PipeOrgan          |       203.34 μs |     1.909 μs |   0.848 μs |  0.18 |    0.00 |    1 |         - |          NA |
| StdSort            | 256  | PipeOrgan          |       260.07 μs |     1.095 μs |   0.486 μs |  0.24 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 256  | PipeOrgan          |       253.08 μs |     3.406 μs |   1.782 μs |  0.23 |    0.00 |    1 |         - |          NA |
| DotnetSort         | 256  | PipeOrgan          |       264.08 μs |     5.247 μs |   2.744 μs |  0.24 |    0.00 |    1 |         - |          NA |
|      |                    |                 |              |            |       |         |      |           |             |
| **QuickSort**          | **1024** | **Random**             |     **1,056.70 μs** |     **2.700 μs** |   **0.963 μs** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| QuickSort3way      | 1024 | Random             |       890.64 μs |     5.519 μs |   2.886 μs |  0.84 |    0.00 |    1 |         - |          NA |
| QuickSortMedian3   | 1024 | Random             |       895.18 μs |     5.704 μs |   2.983 μs |  0.85 |    0.00 |    1 |         - |          NA |
| QuickSortMedian9   | 1024 | Random             |       833.22 μs |     2.062 μs |   0.915 μs |  0.79 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort | 1024 | Random             |       859.26 μs |     2.496 μs |   1.108 μs |  0.81 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 1024 | Random             |     1,704.85 μs |    10.214 μs |   5.342 μs |  1.61 |    0.00 |    2 |         - |          NA |
| IntroSort          | 1024 | Random             |       895.66 μs |     3.774 μs |   1.676 μs |  0.85 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 1024 | Random             |       908.83 μs |     9.535 μs |   4.234 μs |  0.86 |    0.00 |    1 |         - |          NA |
| PDQSort            | 1024 | Random             |     1,036.28 μs |     2.311 μs |   1.026 μs |  0.98 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 1024 | Random             |     1,023.16 μs |     3.447 μs |   1.803 μs |  0.97 |    0.00 |    1 |         - |          NA |
| StdSort            | 1024 | Random             |       887.23 μs |     3.983 μs |   1.769 μs |  0.84 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 1024 | Random             |       869.37 μs |     3.479 μs |   1.819 μs |  0.82 |    0.00 |    1 |         - |          NA |
| DotnetSort         | 1024 | Random             |       943.11 μs |     5.508 μs |   2.881 μs |  0.89 |    0.00 |    1 |         - |          NA |
|      |                    |                 |              |            |       |         |      |           |             |
| **QuickSort**          | **1024** | **SingleElementMoved** |       **642.36 μs** |     **2.380 μs** |   **1.057 μs** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 1024 | SingleElementMoved |     1,361.43 μs |    14.432 μs |   6.408 μs |  2.12 |    0.01 |    4 |         - |          NA |
| QuickSortMedian3   | 1024 | SingleElementMoved |     1,156.64 μs |     2.997 μs |   1.069 μs |  1.80 |    0.00 |    4 |         - |          NA |
| QuickSortMedian9   | 1024 | SingleElementMoved |       872.48 μs |     6.104 μs |   3.193 μs |  1.36 |    0.01 |    3 |         - |          NA |
| DualPivotQuickSort | 1024 | SingleElementMoved |     1,853.65 μs |     4.060 μs |   2.124 μs |  2.89 |    0.01 |    5 |         - |          NA |
| StableQuickSort    | 1024 | SingleElementMoved |     1,227.89 μs |     3.635 μs |   1.614 μs |  1.91 |    0.00 |    4 |         - |          NA |
| IntroSort          | 1024 | SingleElementMoved |       460.07 μs |     1.150 μs |   0.511 μs |  0.72 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 1024 | SingleElementMoved |       633.85 μs |     3.233 μs |   1.436 μs |  0.99 |    0.00 |    2 |         - |          NA |
| PDQSort            | 1024 | SingleElementMoved |       557.47 μs |     4.212 μs |   1.870 μs |  0.87 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless  | 1024 | SingleElementMoved |       561.01 μs |     1.992 μs |   0.884 μs |  0.87 |    0.00 |    2 |         - |          NA |
| StdSort            | 1024 | SingleElementMoved |       587.06 μs |     0.750 μs |   0.392 μs |  0.91 |    0.00 |    2 |         - |          NA |
| BlockQuickSort     | 1024 | SingleElementMoved |       559.52 μs |     1.034 μs |   0.541 μs |  0.87 |    0.00 |    2 |         - |          NA |
| DotnetSort         | 1024 | SingleElementMoved |       647.34 μs |     6.583 μs |   3.443 μs |  1.01 |    0.01 |    2 |         - |          NA |
|      |                    |                 |              |            |       |         |      |           |             |
| **QuickSort**          | **1024** | **Sorted**             |       **593.13 μs** |     **3.355 μs** |   **1.490 μs** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 1024 | Sorted             |     1,905.24 μs |    12.879 μs |   5.719 μs |  3.21 |    0.01 |    5 |         - |          NA |
| QuickSortMedian3   | 1024 | Sorted             |     1,639.70 μs |     6.710 μs |   3.509 μs |  2.76 |    0.01 |    5 |         - |          NA |
| QuickSortMedian9   | 1024 | Sorted             |       880.44 μs |     3.349 μs |   1.487 μs |  1.48 |    0.00 |    3 |         - |          NA |
| DualPivotQuickSort | 1024 | Sorted             |     2,204.60 μs |     8.111 μs |   3.601 μs |  3.72 |    0.01 |    5 |         - |          NA |
| StableQuickSort    | 1024 | Sorted             |     1,251.98 μs |     2.868 μs |   1.500 μs |  2.11 |    0.01 |    4 |         - |          NA |
| IntroSort          | 1024 | Sorted             |       138.99 μs |     1.334 μs |   0.698 μs |  0.23 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 1024 | Sorted             |       491.91 μs |     4.631 μs |   2.422 μs |  0.83 |    0.00 |    2 |         - |          NA |
| PDQSort            | 1024 | Sorted             |       141.65 μs |     0.756 μs |   0.335 μs |  0.24 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 1024 | Sorted             |       145.93 μs |     4.429 μs |   1.966 μs |  0.25 |    0.00 |    1 |         - |          NA |
| StdSort            | 1024 | Sorted             |       157.80 μs |     0.501 μs |   0.222 μs |  0.27 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 1024 | Sorted             |       523.83 μs |     1.867 μs |   0.829 μs |  0.88 |    0.00 |    2 |         - |          NA |
| DotnetSort         | 1024 | Sorted             |       457.66 μs |     3.103 μs |   1.623 μs |  0.77 |    0.00 |    2 |         - |          NA |
|      |                    |                 |              |            |       |         |      |           |             |
| **QuickSort**          | **1024** | **Reversed**           |       **575.64 μs** |     **5.021 μs** |   **2.626 μs** |  **1.00** |    **0.01** |    **3** |         **-** |          **NA** |
| QuickSort3way      | 1024 | Reversed           |     1,329.44 μs |     7.873 μs |   4.118 μs |  2.31 |    0.01 |    5 |         - |          NA |
| QuickSortMedian3   | 1024 | Reversed           |     1,970.26 μs |    13.878 μs |   6.162 μs |  3.42 |    0.02 |    6 |         - |          NA |
| QuickSortMedian9   | 1024 | Reversed           |       882.63 μs |     5.368 μs |   2.383 μs |  1.53 |    0.01 |    4 |         - |          NA |
| DualPivotQuickSort | 1024 | Reversed           |     1,852.90 μs |     3.634 μs |   1.613 μs |  3.22 |    0.01 |    6 |         - |          NA |
| StableQuickSort    | 1024 | Reversed           |     1,271.07 μs |    11.461 μs |   5.994 μs |  2.21 |    0.01 |    5 |         - |          NA |
| IntroSort          | 1024 | Reversed           |       365.08 μs |     1.930 μs |   1.010 μs |  0.63 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet    | 1024 | Reversed           |       809.96 μs |    11.145 μs |   5.829 μs |  1.41 |    0.01 |    4 |         - |          NA |
| PDQSort            | 1024 | Reversed           |       208.19 μs |     1.085 μs |   0.567 μs |  0.36 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 1024 | Reversed           |       209.78 μs |     1.466 μs |   0.767 μs |  0.36 |    0.00 |    1 |         - |          NA |
| StdSort            | 1024 | Reversed           |       210.21 μs |     1.396 μs |   0.730 μs |  0.37 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 1024 | Reversed           |       506.31 μs |     4.630 μs |   2.422 μs |  0.88 |    0.01 |    3 |         - |          NA |
| DotnetSort         | 1024 | Reversed           |       803.80 μs |     8.445 μs |   4.417 μs |  1.40 |    0.01 |    4 |         - |          NA |
|      |                    |                 |              |            |       |         |      |           |             |
| **QuickSort**          | **1024** | **PipeOrgan**          |    **17,115.16 μs** |    **79.420 μs** |  **41.538 μs** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way      | 1024 | PipeOrgan          |     1,353.60 μs |    45.649 μs |  23.875 μs |  0.08 |    0.00 |    3 |         - |          NA |
| QuickSortMedian3   | 1024 | PipeOrgan          |     1,472.83 μs |    10.684 μs |   5.588 μs |  0.09 |    0.00 |    3 |         - |          NA |
| QuickSortMedian9   | 1024 | PipeOrgan          |       841.75 μs |     5.059 μs |   2.646 μs |  0.05 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort | 1024 | PipeOrgan          |       854.89 μs |     1.749 μs |   0.915 μs |  0.05 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 1024 | PipeOrgan          |     1,308.42 μs |    11.115 μs |   5.813 μs |  0.08 |    0.00 |    3 |         - |          NA |
| IntroSort          | 1024 | PipeOrgan          |     1,278.80 μs |     7.587 μs |   3.968 μs |  0.07 |    0.00 |    3 |         - |          NA |
| IntroSortDotnet    | 1024 | PipeOrgan          |     1,603.79 μs |    16.663 μs |   8.715 μs |  0.09 |    0.00 |    3 |         - |          NA |
| PDQSort            | 1024 | PipeOrgan          |     1,062.09 μs |     7.607 μs |   3.377 μs |  0.06 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless  | 1024 | PipeOrgan          |     1,028.06 μs |     5.619 μs |   2.939 μs |  0.06 |    0.00 |    2 |         - |          NA |
| StdSort            | 1024 | PipeOrgan          |     1,521.65 μs |     7.036 μs |   3.680 μs |  0.09 |    0.00 |    3 |         - |          NA |
| BlockQuickSort     | 1024 | PipeOrgan          |     1,303.86 μs |     4.011 μs |   1.781 μs |  0.08 |    0.00 |    3 |         - |          NA |
| DotnetSort         | 1024 | PipeOrgan          |     1,623.61 μs |    10.292 μs |   5.383 μs |  0.09 |    0.00 |    3 |         - |          NA |
|      |                    |                 |              |            |       |         |      |           |             |
| **QuickSort**          | **8192** | **Random**             |    **10,588.11 μs** |    **48.787 μs** |  **21.662 μs** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| QuickSort3way      | 8192 | Random             |     9,240.14 μs |    58.578 μs |  30.637 μs |  0.87 |    0.00 |    1 |         - |          NA |
| QuickSortMedian3   | 8192 | Random             |     9,434.36 μs |    16.121 μs |   7.158 μs |  0.89 |    0.00 |    1 |         - |          NA |
| QuickSortMedian9   | 8192 | Random             |     9,056.43 μs |    27.810 μs |  14.545 μs |  0.86 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort | 8192 | Random             |     9,082.59 μs |    12.707 μs |   5.642 μs |  0.86 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 8192 | Random             |    17,677.53 μs |    32.369 μs |  16.930 μs |  1.67 |    0.00 |    2 |         - |          NA |
| IntroSort          | 8192 | Random             |    10,164.60 μs |   239.744 μs | 125.390 μs |  0.96 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet    | 8192 | Random             |     8,898.51 μs |   115.644 μs |  60.484 μs |  0.84 |    0.01 |    1 |         - |          NA |
| PDQSort            | 8192 | Random             |    10,655.63 μs |    39.448 μs |  17.515 μs |  1.01 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | Random             |    10,591.46 μs |    21.625 μs |  11.311 μs |  1.00 |    0.00 |    1 |         - |          NA |
| StdSort            | 8192 | Random             |     9,888.76 μs |    45.021 μs |  23.547 μs |  0.93 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 8192 | Random             |     9,956.35 μs |   133.151 μs |  59.120 μs |  0.94 |    0.01 |    1 |         - |          NA |
| DotnetSort         | 8192 | Random             |     9,022.87 μs |    33.265 μs |  17.398 μs |  0.85 |    0.00 |    1 |         - |          NA |
|      |                    |                 |              |            |       |         |      |           |             |
| **QuickSort**          | **8192** | **SingleElementMoved** |     **6,741.13 μs** |    **11.251 μs** |   **4.996 μs** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| QuickSort3way      | 8192 | SingleElementMoved |    29,235.10 μs |   867.567 μs | 453.755 μs |  4.34 |    0.06 |    4 |         - |          NA |
| QuickSortMedian3   | 8192 | SingleElementMoved |    22,269.00 μs |    62.687 μs |  32.787 μs |  3.30 |    0.01 |    3 |         - |          NA |
| QuickSortMedian9   | 8192 | SingleElementMoved |     8,908.64 μs |    49.959 μs |  22.182 μs |  1.32 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort | 8192 | SingleElementMoved |    12,395.15 μs |    37.062 μs |  19.384 μs |  1.84 |    0.00 |    2 |         - |          NA |
| StableQuickSort    | 8192 | SingleElementMoved |    13,230.51 μs |    26.496 μs |  11.764 μs |  1.96 |    0.00 |    2 |         - |          NA |
| IntroSort          | 8192 | SingleElementMoved |     4,597.02 μs |    12.988 μs |   6.793 μs |  0.68 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 8192 | SingleElementMoved |     7,427.94 μs |    12.944 μs |   6.770 μs |  1.10 |    0.00 |    1 |         - |          NA |
| PDQSort            | 8192 | SingleElementMoved |     5,352.82 μs |    12.057 μs |   6.306 μs |  0.79 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | SingleElementMoved |     5,431.13 μs |     6.991 μs |   3.104 μs |  0.81 |    0.00 |    1 |         - |          NA |
| StdSort            | 8192 | SingleElementMoved |     5,426.94 μs |    21.763 μs |  11.383 μs |  0.81 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 8192 | SingleElementMoved |     6,144.79 μs |     9.023 μs |   4.719 μs |  0.91 |    0.00 |    1 |         - |          NA |
| DotnetSort         | 8192 | SingleElementMoved |     7,531.81 μs |    44.392 μs |  23.218 μs |  1.12 |    0.00 |    1 |         - |          NA |
|      |                    |                 |              |            |       |         |      |           |             |
| **QuickSort**          | **8192** | **Sorted**             |     **6,454.87 μs** |    **31.683 μs** |  **14.067 μs** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way      | 8192 | Sorted             |    41,851.33 μs |   394.230 μs | 206.190 μs |  6.48 |    0.03 |    5 |         - |          NA |
| QuickSortMedian3   | 8192 | Sorted             |    35,288.15 μs |    84.283 μs |  37.422 μs |  5.47 |    0.01 |    5 |         - |          NA |
| QuickSortMedian9   | 8192 | Sorted             |     8,607.02 μs |    15.412 μs |   8.061 μs |  1.33 |    0.00 |    3 |         - |          NA |
| DualPivotQuickSort | 8192 | Sorted             |    15,653.05 μs |    74.592 μs |  33.119 μs |  2.43 |    0.01 |    4 |         - |          NA |
| StableQuickSort    | 8192 | Sorted             |    13,459.99 μs |    31.769 μs |  14.106 μs |  2.09 |    0.00 |    4 |         - |          NA |
| IntroSort          | 8192 | Sorted             |     1,116.12 μs |     7.070 μs |   3.698 μs |  0.17 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet    | 8192 | Sorted             |     5,564.63 μs |    14.705 μs |   6.529 μs |  0.86 |    0.00 |    2 |         - |          NA |
| PDQSort            | 8192 | Sorted             |     1,170.92 μs |     4.746 μs |   2.482 μs |  0.18 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | Sorted             |     1,197.06 μs |     4.168 μs |   2.180 μs |  0.19 |    0.00 |    1 |         - |          NA |
| StdSort            | 8192 | Sorted             |     1,297.84 μs |     7.207 μs |   3.770 μs |  0.20 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 8192 | Sorted             |     7,824.24 μs |    10.338 μs |   5.407 μs |  1.21 |    0.00 |    3 |         - |          NA |
| DotnetSort         | 8192 | Sorted             |     5,479.58 μs |    12.452 μs |   6.513 μs |  0.85 |    0.00 |    2 |         - |          NA |
|      |                    |                 |              |            |       |         |      |           |             |
| **QuickSort**          | **8192** | **Reversed**           |     **6,127.77 μs** |    **19.617 μs** |  **10.260 μs** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way      | 8192 | Reversed           |    28,100.04 μs |   153.708 μs |  68.247 μs |  4.59 |    0.01 |    6 |         - |          NA |
| QuickSortMedian3   | 8192 | Reversed           |    44,345.24 μs |   364.084 μs | 190.423 μs |  7.24 |    0.03 |    7 |         - |          NA |
| QuickSortMedian9   | 8192 | Reversed           |     8,430.73 μs |     8.309 μs |   3.689 μs |  1.38 |    0.00 |    4 |         - |          NA |
| DualPivotQuickSort | 8192 | Reversed           |    13,421.19 μs |    33.616 μs |  17.582 μs |  2.19 |    0.00 |    5 |         - |          NA |
| StableQuickSort    | 8192 | Reversed           |    13,750.39 μs |    25.379 μs |  13.273 μs |  2.24 |    0.00 |    5 |         - |          NA |
| IntroSort          | 8192 | Reversed           |     3,378.73 μs |     4.587 μs |   2.037 μs |  0.55 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet    | 8192 | Reversed           |     9,691.94 μs |    39.623 μs |  20.723 μs |  1.58 |    0.00 |    4 |         - |          NA |
| PDQSort            | 8192 | Reversed           |     1,723.12 μs |    18.261 μs |   9.551 μs |  0.28 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless  | 8192 | Reversed           |     1,661.07 μs |     7.508 μs |   3.333 μs |  0.27 |    0.00 |    1 |         - |          NA |
| StdSort            | 8192 | Reversed           |     1,745.65 μs |     7.704 μs |   4.029 μs |  0.28 |    0.00 |    1 |         - |          NA |
| BlockQuickSort     | 8192 | Reversed           |     5,699.45 μs |    14.022 μs |   6.226 μs |  0.93 |    0.00 |    3 |         - |          NA |
| DotnetSort         | 8192 | Reversed           |     9,501.86 μs |    36.897 μs |  16.382 μs |  1.55 |    0.00 |    4 |         - |          NA |
|      |                    |                 |              |            |       |         |      |           |             |
| **QuickSort**          | **8192** | **PipeOrgan**          | **1,081,812.36 μs** | **1,569.087 μs** | **696.684 μs** | **1.000** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way      | 8192 | PipeOrgan          |    17,793.29 μs |    98.229 μs |  51.376 μs | 0.016 |    0.00 |    3 |         - |          NA |
| QuickSortMedian3   | 8192 | PipeOrgan          |    18,966.14 μs |   417.484 μs | 218.352 μs | 0.018 |    0.00 |    3 |         - |          NA |
| QuickSortMedian9   | 8192 | PipeOrgan          |     8,750.09 μs |    43.501 μs |  22.752 μs | 0.008 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort | 8192 | PipeOrgan          |     8,592.22 μs |     8.308 μs |   3.689 μs | 0.008 |    0.00 |    1 |         - |          NA |
| StableQuickSort    | 8192 | PipeOrgan          |    13,957.78 μs |    34.368 μs |  17.975 μs | 0.013 |    0.00 |    2 |         - |          NA |
| IntroSort          | 8192 | PipeOrgan          |    20,082.07 μs |    24.110 μs |  10.705 μs | 0.019 |    0.00 |    3 |         - |          NA |
| IntroSortDotnet    | 8192 | PipeOrgan          |    21,508.91 μs |    68.893 μs |  30.589 μs | 0.020 |    0.00 |    3 |         - |          NA |
| PDQSort            | 8192 | PipeOrgan          |    11,463.51 μs |    50.029 μs |  26.166 μs | 0.011 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless  | 8192 | PipeOrgan          |    11,264.36 μs |    47.812 μs |  25.007 μs | 0.010 |    0.00 |    2 |         - |          NA |
| StdSort            | 8192 | PipeOrgan          |    19,692.34 μs |    37.636 μs |  16.710 μs | 0.018 |    0.00 |    3 |         - |          NA |
| BlockQuickSort     | 8192 | PipeOrgan          |    12,387.74 μs |    73.403 μs |  38.391 μs | 0.011 |    0.00 |    2 |         - |          NA |
| DotnetSort         | 8192 | PipeOrgan          |    21,590.90 μs |    52.310 μs |  23.226 μs | 0.020 |    0.00 |    3 |         - |          NA |

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

| Method                 | Size | Pattern            | Mean         | Error     | StdDev    | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------- |----- |------------------- |-------------:|----------:|----------:|------:|--------:|-----:|----------:|------------:|
| **BalancedBinaryTreeSort** | **256**  | **Random**             |    **12.864 μs** | **0.5248 μs** | **0.2745 μs** |  **3.63** |    **0.09** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 256  | Random             |     3.543 μs | 0.1526 μs | 0.0544 μs |  1.00 |    0.02 |    1 |         - |          NA |
| SplaySort              | 256  | Random             |    22.594 μs | 0.5064 μs | 0.2649 μs |  6.38 |    0.12 |    4 |         - |          NA |
| TreapSort              | 256  | Random             |     8.968 μs | 0.5512 μs | 0.2883 μs |  2.53 |    0.09 |    2 |         - |          NA |
|      |                    |              |           |           |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **SingleElementMoved** |    **15.049 μs** | **0.9806 μs** | **0.4354 μs** |  **0.30** |    **0.01** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 256  | SingleElementMoved |    50.730 μs | 0.4140 μs | 0.2165 μs |  1.00 |    0.01 |    4 |         - |          NA |
| SplaySort              | 256  | SingleElementMoved |     4.136 μs | 0.0510 μs | 0.0226 μs |  0.08 |    0.00 |    1 |         - |          NA |
| TreapSort              | 256  | SingleElementMoved |     5.968 μs | 0.3395 μs | 0.1776 μs |  0.12 |    0.00 |    2 |         - |          NA |
|      |                    |              |           |           |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Sorted**             |    **14.234 μs** | **0.8810 μs** | **0.4608 μs** |  **0.19** |    **0.01** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 256  | Sorted             |    75.886 μs | 0.3626 μs | 0.1610 μs |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 256  | Sorted             |     3.694 μs | 0.0199 μs | 0.0071 μs |  0.05 |    0.00 |    1 |         - |          NA |
| TreapSort              | 256  | Sorted             |     5.082 μs | 0.3535 μs | 0.1849 μs |  0.07 |    0.00 |    2 |         - |          NA |
|      |                    |              |           |           |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Reversed**           |    **12.073 μs** | **0.4302 μs** | **0.1910 μs** |  **0.16** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 256  | Reversed           |    73.859 μs | 0.5835 μs | 0.3052 μs |  1.00 |    0.01 |    4 |         - |          NA |
| SplaySort              | 256  | Reversed           |     3.711 μs | 0.0247 μs | 0.0088 μs |  0.05 |    0.00 |    1 |         - |          NA |
| TreapSort              | 256  | Reversed           |     5.441 μs | 0.4839 μs | 0.2531 μs |  0.07 |    0.00 |    2 |         - |          NA |
|      |                    |              |           |           |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **PipeOrgan**          |    **12.170 μs** | **0.5746 μs** | **0.3005 μs** |  **0.32** |    **0.01** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 256  | PipeOrgan          |    38.635 μs | 1.2192 μs | 0.5413 μs |  1.00 |    0.02 |    4 |         - |          NA |
| SplaySort              | 256  | PipeOrgan          |     4.367 μs | 0.2191 μs | 0.0973 μs |  0.11 |    0.00 |    1 |         - |          NA |
| TreapSort              | 256  | PipeOrgan          |     7.188 μs | 0.4145 μs | 0.1841 μs |  0.19 |    0.01 |    2 |         - |          NA |
|      |                    |              |           |           |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Random**             |   **126.499 μs** | **5.3949 μs** | **2.8216 μs** |  **6.31** |    **0.14** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 1024 | Random             |    20.050 μs | 0.2570 μs | 0.1141 μs |  1.00 |    0.01 |    1 |         - |          NA |
| SplaySort              | 1024 | Random             |   161.385 μs | 5.7492 μs | 3.0069 μs |  8.05 |    0.15 |    4 |         - |          NA |
| TreapSort              | 1024 | Random             |    40.586 μs | 2.6480 μs | 1.3850 μs |  2.02 |    0.07 |    2 |         - |          NA |
|      |                    |              |           |           |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **SingleElementMoved** |   **105.289 μs** | **2.3155 μs** | **1.0281 μs** |  **0.13** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 1024 | SingleElementMoved |   780.805 μs | 0.7658 μs | 0.3400 μs |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | SingleElementMoved |    16.353 μs | 0.2696 μs | 0.1410 μs |  0.02 |    0.00 |    1 |         - |          NA |
| TreapSort              | 1024 | SingleElementMoved |    28.615 μs | 0.5320 μs | 0.2782 μs |  0.04 |    0.00 |    2 |         - |          NA |
|      |                    |              |           |           |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Sorted**             |   **100.460 μs** | **1.9735 μs** | **1.0322 μs** |  **0.08** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 1024 | Sorted             | 1,190.848 μs | 0.7959 μs | 0.4163 μs |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | Sorted             |    14.849 μs | 0.1190 μs | 0.0622 μs |  0.01 |    0.00 |    1 |         - |          NA |
| TreapSort              | 1024 | Sorted             |    23.754 μs | 0.5499 μs | 0.2876 μs |  0.02 |    0.00 |    2 |         - |          NA |
|      |                    |              |           |           |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Reversed**           |    **59.309 μs** | **0.9164 μs** | **0.4793 μs** |  **0.05** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 1024 | Reversed           | 1,152.198 μs | 0.7499 μs | 0.3922 μs |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | Reversed           |    14.166 μs | 0.2912 μs | 0.1523 μs |  0.01 |    0.00 |    1 |         - |          NA |
| TreapSort              | 1024 | Reversed           |    23.289 μs | 0.4664 μs | 0.2440 μs |  0.02 |    0.00 |    2 |         - |          NA |
|      |                    |              |           |           |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **PipeOrgan**          |    **92.373 μs** | **1.9695 μs** | **1.0301 μs** |  **0.16** |    **0.00** |    **3** |         **-** |          **NA** |
| BinaryTreeSort         | 1024 | PipeOrgan          |   583.350 μs | 1.0704 μs | 0.3817 μs |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | PipeOrgan          |    17.025 μs | 0.2676 μs | 0.1188 μs |  0.03 |    0.00 |    1 |         - |          NA |
| TreapSort              | 1024 | PipeOrgan          |    34.138 μs | 1.5990 μs | 0.8363 μs |  0.06 |    0.00 |    2 |         - |          NA |

</details>


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
