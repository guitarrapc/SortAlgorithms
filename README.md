# SortAlgorithms

This repository shows implementation for the Major Sort Algorithm.
Aim not to use LINQ or similar ease to use, but memory unefficient technique.

## Table of Contents

- [Benchmark](#benchmark)
  - [DistributionBenchmark](#distributionbenchmark)
  - [ExchangeBenchmark](#exchangebenchmark)
  - [HeapBenchmark](#heapbenchmark)
  - [InsertionBenchmark](#insertionbenchmark)
  - [IntKeyBenchmark](#intkeybenchmark)
  - [MergeBenchmark](#mergebenchmark)
  - [MergeSlowBenchmark](#mergeslowbenchmark)
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
  - [Distribution](#distribution)
  - [Network](#network)
  - [Tree](#tree)
  - [Joke](#joke)

## Benchmark

<!-- BENCHMARK_START -->
<details>
<summary>Benchmark results (2026-11-20 19:55 UTC)</summary>

Workflow run: https://github.com/guitarrapc/SortAlgorithms/actions/runs/44803469267

### DistributionBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.52 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method              | Size | Pattern            | Mean           | Error        | StdDev      | Ratio  | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------------- |----- |------------------- |---------------:|-------------:|------------:|-------:|--------:|-----:|----------:|------------:|
| **CountingSort**        | **256**  | **Random**             |     **1,293.9 ns** |     **36.18 ns** |    **16.07 ns** |   **1.44** |    **0.04** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | Random             |       898.2 ns |     56.95 ns |    25.29 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 256  | Random             |     2,030.7 ns |     56.19 ns |    29.39 ns |   2.26 |    0.07 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | Random             |       706.4 ns |     23.57 ns |    10.47 ns |   0.79 |    0.02 |    1 |         - |          NA |
| BucketSort          | 256  | Random             |     1,417.5 ns |     36.95 ns |    16.41 ns |   1.58 |    0.04 |    2 |         - |          NA |
| BucketSortInteger   | 256  | Random             |     1,176.4 ns |     34.12 ns |    15.15 ns |   1.31 |    0.04 |    2 |         - |          NA |
| FlashSort           | 256  | Random             |     3,966.2 ns |    141.19 ns |    62.69 ns |   4.42 |    0.13 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | Random             |     2,228.5 ns |    150.33 ns |    66.75 ns |   2.48 |    0.09 |    3 |         - |          NA |
| RotateLSDRadixSort  | 256  | Random             |    53,052.1 ns |    439.51 ns |   195.14 ns |  59.10 |    1.55 |    7 |         - |          NA |
| RadixLSD256Sort     | 256  | Random             |     1,036.6 ns |     14.75 ns |     6.55 ns |   1.15 |    0.03 |    2 |         - |          NA |
| Radsort             | 256  | Random             |     8,475.8 ns |    316.85 ns |   165.72 ns |   9.44 |    0.30 |    6 |         - |          NA |
| RadixLSD10Sort      | 256  | Random             |     3,163.7 ns |     32.82 ns |    14.57 ns |   3.52 |    0.09 |    4 |         - |          NA |
| RadixMSD4Sort       | 256  | Random             |     5,151.5 ns |     47.79 ns |    17.04 ns |   5.74 |    0.15 |    5 |         - |          NA |
| RadixMSD10Sort      | 256  | Random             |     4,788.5 ns |    243.65 ns |   127.43 ns |   5.33 |    0.19 |    5 |         - |          NA |
| AmericanFlagSort    | 256  | Random             |     2,401.2 ns |     66.33 ns |    23.65 ns |   2.68 |    0.07 |    3 |         - |          NA |
| SpreadSort          | 256  | Random             |     6,068.4 ns |    150.58 ns |    78.76 ns |   6.76 |    0.19 |    5 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **256**  | **SingleElementMoved** |     **1,259.4 ns** |     **19.03 ns** |     **8.45 ns** |   **1.57** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | SingleElementMoved |       802.9 ns |     12.80 ns |     5.68 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 256  | SingleElementMoved |     1,167.6 ns |     42.72 ns |    22.34 ns |   1.45 |    0.03 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | SingleElementMoved |       659.4 ns |     54.72 ns |    28.62 ns |   0.82 |    0.03 |    1 |         - |          NA |
| BucketSort          | 256  | SingleElementMoved |     1,378.4 ns |     30.37 ns |    13.48 ns |   1.72 |    0.02 |    3 |         - |          NA |
| BucketSortInteger   | 256  | SingleElementMoved |     1,135.8 ns |     22.71 ns |    10.09 ns |   1.41 |    0.01 |    3 |         - |          NA |
| FlashSort           | 256  | SingleElementMoved |     3,227.4 ns |    138.79 ns |    72.59 ns |   4.02 |    0.09 |    6 |         - |          NA |
| RadixLSD4Sort       | 256  | SingleElementMoved |     2,294.2 ns |    157.06 ns |    69.74 ns |   2.86 |    0.08 |    5 |         - |          NA |
| RotateLSDRadixSort  | 256  | SingleElementMoved |    34,972.8 ns |    399.20 ns |   208.79 ns |  43.56 |    0.38 |    9 |         - |          NA |
| RadixLSD256Sort     | 256  | SingleElementMoved |       982.3 ns |     22.17 ns |     7.91 ns |   1.22 |    0.01 |    3 |         - |          NA |
| Radsort             | 256  | SingleElementMoved |     7,651.1 ns |    116.71 ns |    51.82 ns |   9.53 |    0.09 |    8 |         - |          NA |
| RadixLSD10Sort      | 256  | SingleElementMoved |     3,163.4 ns |    198.60 ns |   103.87 ns |   3.94 |    0.12 |    6 |         - |          NA |
| RadixMSD4Sort       | 256  | SingleElementMoved |     5,565.8 ns |     22.78 ns |    10.12 ns |   6.93 |    0.05 |    7 |         - |          NA |
| RadixMSD10Sort      | 256  | SingleElementMoved |     4,750.5 ns |    219.84 ns |    97.61 ns |   5.92 |    0.12 |    7 |         - |          NA |
| AmericanFlagSort    | 256  | SingleElementMoved |     1,655.3 ns |     23.49 ns |    10.43 ns |   2.06 |    0.02 |    4 |         - |          NA |
| SpreadSort          | 256  | SingleElementMoved |     1,308.3 ns |    515.68 ns |   269.71 ns |   1.63 |    0.32 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **256**  | **Sorted**             |     **1,221.4 ns** |     **15.01 ns** |     **6.67 ns** |   **1.66** |    **0.01** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 256  | Sorted             |       737.7 ns |      5.02 ns |     2.23 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 256  | Sorted             |     1,114.7 ns |      8.71 ns |     4.55 ns |   1.51 |    0.01 |    4 |         - |          NA |
| PigeonSortInteger   | 256  | Sorted             |       555.3 ns |     21.19 ns |     7.56 ns |   0.75 |    0.01 |    2 |         - |          NA |
| BucketSort          | 256  | Sorted             |     1,312.1 ns |      5.05 ns |     1.80 ns |   1.78 |    0.01 |    4 |         - |          NA |
| BucketSortInteger   | 256  | Sorted             |     1,034.4 ns |     27.79 ns |    12.34 ns |   1.40 |    0.02 |    4 |         - |          NA |
| FlashSort           | 256  | Sorted             |     2,857.9 ns |    228.19 ns |   119.35 ns |   3.87 |    0.15 |    6 |         - |          NA |
| RadixLSD4Sort       | 256  | Sorted             |     2,096.6 ns |     22.75 ns |     8.11 ns |   2.84 |    0.01 |    5 |         - |          NA |
| RotateLSDRadixSort  | 256  | Sorted             |    34,150.5 ns |    204.64 ns |    90.86 ns |  46.29 |    0.17 |    9 |         - |          NA |
| RadixLSD256Sort     | 256  | Sorted             |       890.3 ns |      7.87 ns |     3.49 ns |   1.21 |    0.01 |    4 |         - |          NA |
| Radsort             | 256  | Sorted             |     7,483.9 ns |    140.10 ns |    73.27 ns |  10.14 |    0.10 |    8 |         - |          NA |
| RadixLSD10Sort      | 256  | Sorted             |     3,021.7 ns |     36.56 ns |    16.23 ns |   4.10 |    0.02 |    6 |         - |          NA |
| RadixMSD4Sort       | 256  | Sorted             |     5,349.4 ns |     53.43 ns |    19.05 ns |   7.25 |    0.03 |    7 |         - |          NA |
| RadixMSD10Sort      | 256  | Sorted             |     4,574.5 ns |     25.80 ns |     9.20 ns |   6.20 |    0.02 |    7 |         - |          NA |
| AmericanFlagSort    | 256  | Sorted             |     1,251.5 ns |     15.89 ns |     7.05 ns |   1.70 |    0.01 |    4 |         - |          NA |
| SpreadSort          | 256  | Sorted             |       254.1 ns |      6.61 ns |     3.46 ns |   0.34 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **256**  | **Reversed**           |     **1,184.3 ns** |      **3.34 ns** |     **1.48 ns** |   **1.56** |    **0.01** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 256  | Reversed           |       757.4 ns |      7.89 ns |     4.12 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 256  | Reversed           |     1,148.9 ns |     12.02 ns |     4.29 ns |   1.52 |    0.01 |    4 |         - |          NA |
| PigeonSortInteger   | 256  | Reversed           |       578.6 ns |     14.51 ns |     6.44 ns |   0.76 |    0.01 |    2 |         - |          NA |
| BucketSort          | 256  | Reversed           |     1,392.9 ns |      5.88 ns |     2.61 ns |   1.84 |    0.01 |    4 |         - |          NA |
| BucketSortInteger   | 256  | Reversed           |     1,147.3 ns |    344.20 ns |   152.83 ns |   1.51 |    0.19 |    4 |         - |          NA |
| FlashSort           | 256  | Reversed           |     2,742.6 ns |    180.62 ns |    94.47 ns |   3.62 |    0.12 |    6 |         - |          NA |
| RadixLSD4Sort       | 256  | Reversed           |     2,091.6 ns |     58.22 ns |    25.85 ns |   2.76 |    0.03 |    5 |         - |          NA |
| RotateLSDRadixSort  | 256  | Reversed           |    34,912.0 ns |    232.40 ns |   121.55 ns |  46.09 |    0.28 |    9 |         - |          NA |
| RadixLSD256Sort     | 256  | Reversed           |       911.5 ns |      3.17 ns |     1.41 ns |   1.20 |    0.01 |    4 |         - |          NA |
| Radsort             | 256  | Reversed           |     7,233.6 ns |      8.19 ns |     3.64 ns |   9.55 |    0.05 |    8 |         - |          NA |
| RadixLSD10Sort      | 256  | Reversed           |     3,062.2 ns |     28.42 ns |    12.62 ns |   4.04 |    0.03 |    6 |         - |          NA |
| RadixMSD4Sort       | 256  | Reversed           |     5,704.5 ns |    331.77 ns |   173.52 ns |   7.53 |    0.22 |    7 |         - |          NA |
| RadixMSD10Sort      | 256  | Reversed           |     4,866.6 ns |    212.72 ns |    94.45 ns |   6.43 |    0.12 |    7 |         - |          NA |
| AmericanFlagSort    | 256  | Reversed           |     1,344.9 ns |     15.67 ns |     6.96 ns |   1.78 |    0.01 |    4 |         - |          NA |
| SpreadSort          | 256  | Reversed           |       374.2 ns |      6.81 ns |     3.02 ns |   0.49 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **256**  | **PipeOrgan**          |     **1,207.0 ns** |     **11.10 ns** |     **4.93 ns** |   **1.51** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | PipeOrgan          |       797.8 ns |     10.49 ns |     5.49 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 256  | PipeOrgan          |     1,105.9 ns |     14.26 ns |     5.09 ns |   1.39 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | PipeOrgan          |       651.9 ns |      6.37 ns |     3.33 ns |   0.82 |    0.01 |    1 |         - |          NA |
| BucketSort          | 256  | PipeOrgan          |     1,510.8 ns |     42.26 ns |    22.10 ns |   1.89 |    0.03 |    3 |         - |          NA |
| BucketSortInteger   | 256  | PipeOrgan          |     1,186.5 ns |     13.09 ns |     4.67 ns |   1.49 |    0.01 |    3 |         - |          NA |
| FlashSort           | 256  | PipeOrgan          |     3,417.0 ns |    171.40 ns |    76.10 ns |   4.28 |    0.09 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | PipeOrgan          |     2,215.7 ns |     24.11 ns |    12.61 ns |   2.78 |    0.02 |    4 |         - |          NA |
| RotateLSDRadixSort  | 256  | PipeOrgan          |    34,494.0 ns |    226.47 ns |   118.45 ns |  43.24 |    0.32 |    9 |         - |          NA |
| RadixLSD256Sort     | 256  | PipeOrgan          |       994.5 ns |      8.37 ns |     2.98 ns |   1.25 |    0.01 |    3 |         - |          NA |
| Radsort             | 256  | PipeOrgan          |     7,265.3 ns |     19.42 ns |     6.92 ns |   9.11 |    0.06 |    8 |         - |          NA |
| RadixLSD10Sort      | 256  | PipeOrgan          |     3,133.7 ns |     16.15 ns |     7.17 ns |   3.93 |    0.03 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | PipeOrgan          |     5,807.1 ns |    264.44 ns |   138.31 ns |   7.28 |    0.17 |    7 |         - |          NA |
| RadixMSD10Sort      | 256  | PipeOrgan          |     4,707.1 ns |     53.29 ns |    19.00 ns |   5.90 |    0.04 |    6 |         - |          NA |
| AmericanFlagSort    | 256  | PipeOrgan          |     1,894.9 ns |     41.07 ns |    14.64 ns |   2.38 |    0.02 |    4 |         - |          NA |
| SpreadSort          | 256  | PipeOrgan          |     1,374.1 ns |     66.26 ns |    29.42 ns |   1.72 |    0.04 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **256**  | **ManyDuplicates**     |     **1,224.9 ns** |     **26.92 ns** |     **9.60 ns** |   **1.54** |    **0.03** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | ManyDuplicates     |       795.8 ns |     29.12 ns |    12.93 ns |   1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 256  | ManyDuplicates     |     1,287.0 ns |    263.92 ns |   117.18 ns |   1.62 |    0.14 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | ManyDuplicates     |       714.5 ns |    143.92 ns |    75.27 ns |   0.90 |    0.09 |    1 |         - |          NA |
| BucketSort          | 256  | ManyDuplicates     |     2,185.7 ns |     67.78 ns |    35.45 ns |   2.75 |    0.06 |    4 |         - |          NA |
| BucketSortInteger   | 256  | ManyDuplicates     |     1,332.7 ns |     93.75 ns |    49.03 ns |   1.68 |    0.06 |    2 |         - |          NA |
| FlashSort           | 256  | ManyDuplicates     |     4,026.8 ns |     91.62 ns |    40.68 ns |   5.06 |    0.09 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | ManyDuplicates     |     1,803.5 ns |     16.50 ns |     8.63 ns |   2.27 |    0.04 |    3 |         - |          NA |
| RotateLSDRadixSort  | 256  | ManyDuplicates     |    46,621.4 ns |    655.23 ns |   290.93 ns |  58.60 |    0.95 |    8 |         - |          NA |
| RadixLSD256Sort     | 256  | ManyDuplicates     |     1,035.5 ns |     17.24 ns |     7.66 ns |   1.30 |    0.02 |    2 |         - |          NA |
| Radsort             | 256  | ManyDuplicates     |     8,974.3 ns |    239.73 ns |   125.38 ns |  11.28 |    0.23 |    7 |         - |          NA |
| RadixLSD10Sort      | 256  | ManyDuplicates     |     2,235.4 ns |     20.48 ns |     9.09 ns |   2.81 |    0.04 |    4 |         - |          NA |
| RadixMSD4Sort       | 256  | ManyDuplicates     |     2,683.4 ns |    343.43 ns |   179.62 ns |   3.37 |    0.22 |    4 |         - |          NA |
| RadixMSD10Sort      | 256  | ManyDuplicates     |     2,963.2 ns |    144.99 ns |    64.38 ns |   3.72 |    0.09 |    4 |         - |          NA |
| AmericanFlagSort    | 256  | ManyDuplicates     |     2,460.3 ns |    136.42 ns |    71.35 ns |   3.09 |    0.10 |    4 |         - |          NA |
| SpreadSort          | 256  | ManyDuplicates     |     5,640.0 ns |    242.73 ns |   126.95 ns |   7.09 |    0.18 |    6 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **1024** | **Random**             |     **4,930.8 ns** |    **186.26 ns** |    **97.42 ns** |   **1.49** |    **0.03** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Random             |     3,307.1 ns |     27.24 ns |     9.72 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | Random             |     3,930.0 ns |     39.47 ns |    17.53 ns |   1.19 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | Random             |     2,528.9 ns |     15.73 ns |     6.98 ns |   0.76 |    0.00 |    1 |         - |          NA |
| BucketSort          | 1024 | Random             |     5,163.7 ns |     56.77 ns |    29.69 ns |   1.56 |    0.01 |    2 |         - |          NA |
| BucketSortInteger   | 1024 | Random             |     4,203.5 ns |     33.84 ns |    17.70 ns |   1.27 |    0.01 |    2 |         - |          NA |
| FlashSort           | 1024 | Random             |    15,347.5 ns |    189.60 ns |    99.17 ns |   4.64 |    0.03 |    5 |         - |          NA |
| RadixLSD4Sort       | 1024 | Random             |    10,462.1 ns |    252.58 ns |   132.11 ns |   3.16 |    0.04 |    4 |         - |          NA |
| RotateLSDRadixSort  | 1024 | Random             |   281,882.5 ns |  1,323.83 ns |   692.39 ns |  85.24 |    0.30 |    8 |         - |          NA |
| RadixLSD256Sort     | 1024 | Random             |     5,426.7 ns |    269.73 ns |   141.07 ns |   1.64 |    0.04 |    2 |         - |          NA |
| Radsort             | 1024 | Random             |    28,983.8 ns |    189.19 ns |    84.00 ns |   8.76 |    0.03 |    7 |         - |          NA |
| RadixLSD10Sort      | 1024 | Random             |    16,074.3 ns |     77.43 ns |    40.50 ns |   4.86 |    0.02 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | Random             |    22,797.9 ns |    215.39 ns |    95.64 ns |   6.89 |    0.03 |    6 |         - |          NA |
| RadixMSD10Sort      | 1024 | Random             |    23,414.0 ns |    452.39 ns |   200.86 ns |   7.08 |    0.06 |    6 |         - |          NA |
| AmericanFlagSort    | 1024 | Random             |    16,992.2 ns |    209.17 ns |   109.40 ns |   5.14 |    0.03 |    5 |         - |          NA |
| SpreadSort          | 1024 | Random             |     6,807.5 ns |    164.51 ns |    86.04 ns |   2.06 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **1024** | **SingleElementMoved** |     **4,617.1 ns** |     **24.40 ns** |    **10.83 ns** |   **1.50** |    **0.08** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | SingleElementMoved |     3,077.4 ns |    348.56 ns |   182.30 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | SingleElementMoved |     3,863.5 ns |     25.75 ns |    11.43 ns |   1.26 |    0.07 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | SingleElementMoved |     2,446.0 ns |     18.98 ns |     8.43 ns |   0.80 |    0.04 |    1 |         - |          NA |
| BucketSort          | 1024 | SingleElementMoved |     5,103.0 ns |     15.22 ns |     5.43 ns |   1.66 |    0.09 |    3 |         - |          NA |
| BucketSortInteger   | 1024 | SingleElementMoved |     4,208.2 ns |     17.28 ns |     7.67 ns |   1.37 |    0.07 |    3 |         - |          NA |
| FlashSort           | 1024 | SingleElementMoved |    12,015.3 ns |    154.45 ns |    80.78 ns |   3.92 |    0.21 |    5 |         - |          NA |
| RadixLSD4Sort       | 1024 | SingleElementMoved |    11,620.3 ns |    429.62 ns |   224.70 ns |   3.79 |    0.22 |    5 |         - |          NA |
| RotateLSDRadixSort  | 1024 | SingleElementMoved |   178,095.4 ns |  2,589.52 ns | 1,354.37 ns |  58.04 |    3.17 |    8 |         - |          NA |
| RadixLSD256Sort     | 1024 | SingleElementMoved |     6,123.0 ns |    259.50 ns |   115.22 ns |   2.00 |    0.11 |    3 |         - |          NA |
| Radsort             | 1024 | SingleElementMoved |    20,175.8 ns |    160.51 ns |    83.95 ns |   6.58 |    0.36 |    7 |         - |          NA |
| RadixLSD10Sort      | 1024 | SingleElementMoved |    15,971.5 ns |     88.37 ns |    39.23 ns |   5.21 |    0.28 |    6 |         - |          NA |
| RadixMSD4Sort       | 1024 | SingleElementMoved |    24,020.9 ns |    245.19 ns |   128.24 ns |   7.83 |    0.43 |    7 |         - |          NA |
| RadixMSD10Sort      | 1024 | SingleElementMoved |    22,346.2 ns |    211.09 ns |   110.40 ns |   7.28 |    0.40 |    7 |         - |          NA |
| AmericanFlagSort    | 1024 | SingleElementMoved |     8,941.4 ns |    326.31 ns |   170.67 ns |   2.91 |    0.17 |    4 |         - |          NA |
| SpreadSort          | 1024 | SingleElementMoved |     4,465.5 ns |     11.89 ns |     6.22 ns |   1.46 |    0.08 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **1024** | **Sorted**             |     **4,796.4 ns** |    **199.19 ns** |   **104.18 ns** |   **1.67** |    **0.03** |    **5** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Sorted             |     2,876.5 ns |     26.66 ns |     9.51 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 1024 | Sorted             |     3,852.7 ns |     28.94 ns |    10.32 ns |   1.34 |    0.01 |    4 |         - |          NA |
| PigeonSortInteger   | 1024 | Sorted             |     2,153.8 ns |     11.59 ns |     5.15 ns |   0.75 |    0.00 |    2 |         - |          NA |
| BucketSort          | 1024 | Sorted             |     5,707.8 ns |    259.24 ns |   135.59 ns |   1.98 |    0.04 |    5 |         - |          NA |
| BucketSortInteger   | 1024 | Sorted             |     3,839.2 ns |     49.25 ns |    21.87 ns |   1.33 |    0.01 |    4 |         - |          NA |
| FlashSort           | 1024 | Sorted             |    10,909.8 ns |    223.82 ns |   117.06 ns |   3.79 |    0.04 |    7 |         - |          NA |
| RadixLSD4Sort       | 1024 | Sorted             |    12,341.2 ns |    630.08 ns |   329.55 ns |   4.29 |    0.11 |    7 |         - |          NA |
| RotateLSDRadixSort  | 1024 | Sorted             |   175,458.5 ns |  3,527.92 ns | 1,566.42 ns |  61.00 |    0.54 |   10 |         - |          NA |
| RadixLSD256Sort     | 1024 | Sorted             |     4,980.4 ns |    376.83 ns |   167.32 ns |   1.73 |    0.05 |    5 |         - |          NA |
| Radsort             | 1024 | Sorted             |    18,902.3 ns |    365.90 ns |   191.38 ns |   6.57 |    0.07 |    8 |         - |          NA |
| RadixLSD10Sort      | 1024 | Sorted             |    15,920.6 ns |    200.49 ns |   104.86 ns |   5.53 |    0.04 |    8 |         - |          NA |
| RadixMSD4Sort       | 1024 | Sorted             |    24,977.7 ns |    308.45 ns |   161.33 ns |   8.68 |    0.06 |    9 |         - |          NA |
| RadixMSD10Sort      | 1024 | Sorted             |    25,058.8 ns |  8,574.54 ns | 4,484.65 ns |   8.71 |    1.47 |    9 |         - |          NA |
| AmericanFlagSort    | 1024 | Sorted             |     8,837.6 ns |  1,439.08 ns |   752.66 ns |   3.07 |    0.25 |    6 |         - |          NA |
| SpreadSort          | 1024 | Sorted             |       488.4 ns |      4.66 ns |     2.07 ns |   0.17 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **1024** | **Reversed**           |     **4,399.4 ns** |    **187.22 ns** |    **97.92 ns** |   **1.51** |    **0.03** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Reversed           |     2,908.8 ns |     20.33 ns |     7.25 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | Reversed           |     4,130.9 ns |      4.83 ns |     2.14 ns |   1.42 |    0.00 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | Reversed           |     2,184.2 ns |      9.84 ns |     3.51 ns |   0.75 |    0.00 |    1 |         - |          NA |
| BucketSort          | 1024 | Reversed           |     5,272.7 ns |    337.51 ns |   176.53 ns |   1.81 |    0.06 |    3 |         - |          NA |
| BucketSortInteger   | 1024 | Reversed           |     4,051.0 ns |    325.19 ns |   170.08 ns |   1.39 |    0.06 |    3 |         - |          NA |
| FlashSort           | 1024 | Reversed           |    10,569.6 ns |    272.00 ns |   142.26 ns |   3.63 |    0.05 |    5 |         - |          NA |
| RadixLSD4Sort       | 1024 | Reversed           |    12,609.6 ns |    466.84 ns |   244.16 ns |   4.34 |    0.08 |    5 |         - |          NA |
| RotateLSDRadixSort  | 1024 | Reversed           |   178,585.7 ns |  1,816.69 ns |   806.62 ns |  61.40 |    0.30 |    7 |         - |          NA |
| RadixLSD256Sort     | 1024 | Reversed           |     4,930.1 ns |    120.64 ns |    63.09 ns |   1.69 |    0.02 |    3 |         - |          NA |
| Radsort             | 1024 | Reversed           |    18,831.1 ns |     81.59 ns |    36.23 ns |   6.47 |    0.02 |    6 |         - |          NA |
| RadixLSD10Sort      | 1024 | Reversed           |    15,965.1 ns |    427.30 ns |   189.73 ns |   5.49 |    0.06 |    6 |         - |          NA |
| RadixMSD4Sort       | 1024 | Reversed           |    23,786.2 ns |    221.54 ns |    98.36 ns |   8.18 |    0.04 |    6 |         - |          NA |
| RadixMSD10Sort      | 1024 | Reversed           |    22,067.9 ns |    170.57 ns |    89.21 ns |   7.59 |    0.03 |    6 |         - |          NA |
| AmericanFlagSort    | 1024 | Reversed           |     8,428.7 ns |    354.52 ns |   185.42 ns |   2.90 |    0.06 |    4 |         - |          NA |
| SpreadSort          | 1024 | Reversed           |     3,679.2 ns |     27.90 ns |    12.39 ns |   1.26 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **1024** | **PipeOrgan**          |     **4,440.7 ns** |     **50.28 ns** |    **22.32 ns** |   **1.45** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | PipeOrgan          |     3,056.0 ns |     71.50 ns |    31.75 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | PipeOrgan          |     3,970.1 ns |    107.73 ns |    47.83 ns |   1.30 |    0.02 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | PipeOrgan          |     2,437.1 ns |     11.55 ns |     4.12 ns |   0.80 |    0.01 |    1 |         - |          NA |
| BucketSort          | 1024 | PipeOrgan          |     5,168.4 ns |    342.99 ns |   179.39 ns |   1.69 |    0.06 |    3 |         - |          NA |
| BucketSortInteger   | 1024 | PipeOrgan          |     4,220.3 ns |     84.31 ns |    44.10 ns |   1.38 |    0.02 |    3 |         - |          NA |
| FlashSort           | 1024 | PipeOrgan          |    13,376.5 ns |    129.92 ns |    57.69 ns |   4.38 |    0.05 |    4 |         - |          NA |
| RadixLSD4Sort       | 1024 | PipeOrgan          |    10,661.1 ns |    269.03 ns |   140.71 ns |   3.49 |    0.05 |    4 |         - |          NA |
| RotateLSDRadixSort  | 1024 | PipeOrgan          |   176,312.2 ns |  1,067.77 ns |   474.09 ns |  57.70 |    0.57 |    7 |         - |          NA |
| RadixLSD256Sort     | 1024 | PipeOrgan          |     5,282.5 ns |    203.58 ns |    90.39 ns |   1.73 |    0.03 |    3 |         - |          NA |
| Radsort             | 1024 | PipeOrgan          |    19,418.0 ns |    200.96 ns |   105.10 ns |   6.35 |    0.07 |    6 |         - |          NA |
| RadixLSD10Sort      | 1024 | PipeOrgan          |    16,131.4 ns |    185.30 ns |    96.91 ns |   5.28 |    0.06 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | PipeOrgan          |    24,050.3 ns |    326.03 ns |   170.52 ns |   7.87 |    0.09 |    6 |         - |          NA |
| RadixMSD10Sort      | 1024 | PipeOrgan          |    22,661.5 ns |    158.98 ns |    83.15 ns |   7.42 |    0.08 |    6 |         - |          NA |
| AmericanFlagSort    | 1024 | PipeOrgan          |    11,601.1 ns |    306.21 ns |   160.15 ns |   3.80 |    0.06 |    4 |         - |          NA |
| SpreadSort          | 1024 | PipeOrgan          |     4,809.3 ns |     30.01 ns |    13.32 ns |   1.57 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **1024** | **ManyDuplicates**     |     **4,448.9 ns** |     **32.93 ns** |    **14.62 ns** |   **1.58** |    **0.01** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | ManyDuplicates     |     2,823.0 ns |     32.37 ns |    11.54 ns |   1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 1024 | ManyDuplicates     |     4,966.1 ns |    354.17 ns |   185.24 ns |   1.76 |    0.06 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | ManyDuplicates     |     2,357.9 ns |     40.95 ns |    14.60 ns |   0.84 |    0.01 |    1 |         - |          NA |
| BucketSort          | 1024 | ManyDuplicates     |     7,756.1 ns |    245.28 ns |   128.29 ns |   2.75 |    0.04 |    3 |         - |          NA |
| BucketSortInteger   | 1024 | ManyDuplicates     |     4,489.0 ns |     76.47 ns |    39.99 ns |   1.59 |    0.01 |    2 |         - |          NA |
| FlashSort           | 1024 | ManyDuplicates     |    13,637.2 ns |    203.55 ns |   106.46 ns |   4.83 |    0.04 |    4 |         - |          NA |
| RadixLSD4Sort       | 1024 | ManyDuplicates     |     6,965.8 ns |    293.45 ns |   153.48 ns |   2.47 |    0.05 |    3 |         - |          NA |
| RotateLSDRadixSort  | 1024 | ManyDuplicates     |   190,432.7 ns |    503.04 ns |   179.39 ns |  67.46 |    0.26 |    6 |         - |          NA |
| RadixLSD256Sort     | 1024 | ManyDuplicates     |     3,325.8 ns |     36.20 ns |    12.91 ns |   1.18 |    0.01 |    1 |         - |          NA |
| Radsort             | 1024 | ManyDuplicates     |    19,971.2 ns |    214.80 ns |    95.37 ns |   7.07 |    0.04 |    5 |         - |          NA |
| RadixLSD10Sort      | 1024 | ManyDuplicates     |     8,766.1 ns |    292.49 ns |   152.98 ns |   3.11 |    0.05 |    3 |         - |          NA |
| RadixMSD4Sort       | 1024 | ManyDuplicates     |     7,694.4 ns |    240.51 ns |   125.79 ns |   2.73 |    0.04 |    3 |         - |          NA |
| RadixMSD10Sort      | 1024 | ManyDuplicates     |     9,727.6 ns |    272.46 ns |   142.50 ns |   3.45 |    0.05 |    3 |         - |          NA |
| AmericanFlagSort    | 1024 | ManyDuplicates     |     7,385.1 ns |    257.06 ns |   134.44 ns |   2.62 |    0.05 |    3 |         - |          NA |
| SpreadSort          | 1024 | ManyDuplicates     |     5,357.4 ns |    169.65 ns |    75.33 ns |   1.90 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **4096** | **Random**             |    **19,084.6 ns** |    **142.05 ns** |    **74.30 ns** |   **1.43** |    **0.01** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Random             |    13,308.7 ns |    266.56 ns |   118.35 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | Random             |    15,314.1 ns |     61.80 ns |    27.44 ns |   1.15 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 4096 | Random             |     9,959.7 ns |    232.44 ns |   121.57 ns |   0.75 |    0.01 |    1 |         - |          NA |
| BucketSort          | 4096 | Random             |    21,836.2 ns |     79.48 ns |    35.29 ns |   1.64 |    0.01 |    2 |         - |          NA |
| BucketSortInteger   | 4096 | Random             |    17,400.9 ns |    297.67 ns |   155.69 ns |   1.31 |    0.02 |    2 |         - |          NA |
| FlashSort           | 4096 | Random             |    60,671.2 ns |    204.86 ns |    90.96 ns |   4.56 |    0.04 |    4 |         - |          NA |
| RadixLSD4Sort       | 4096 | Random             |    50,722.0 ns |    491.00 ns |   218.01 ns |   3.81 |    0.04 |    4 |         - |          NA |
| RotateLSDRadixSort  | 4096 | Random             | 1,401,969.8 ns |  2,271.23 ns |   809.94 ns | 105.35 |    0.88 |    6 |         - |          NA |
| RadixLSD256Sort     | 4096 | Random             |    19,517.7 ns |    330.35 ns |   146.68 ns |   1.47 |    0.02 |    2 |         - |          NA |
| Radsort             | 4096 | Random             |    68,495.9 ns |    563.38 ns |   294.66 ns |   5.15 |    0.05 |    4 |         - |          NA |
| RadixLSD10Sort      | 4096 | Random             |    64,091.8 ns |    489.73 ns |   217.44 ns |   4.82 |    0.04 |    4 |         - |          NA |
| RadixMSD4Sort       | 4096 | Random             |    99,573.6 ns | 11,127.47 ns | 4,940.67 ns |   7.48 |    0.35 |    5 |         - |          NA |
| RadixMSD10Sort      | 4096 | Random             |    89,659.8 ns |    235.40 ns |   123.12 ns |   6.74 |    0.06 |    5 |         - |          NA |
| AmericanFlagSort    | 4096 | Random             |    61,428.5 ns |    351.69 ns |   156.15 ns |   4.62 |    0.04 |    4 |         - |          NA |
| SpreadSort          | 4096 | Random             |    28,529.9 ns |    424.45 ns |   222.00 ns |   2.14 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **4096** | **SingleElementMoved** |    **18,552.0 ns** |    **258.64 ns** |   **135.27 ns** |   **1.56** |    **0.03** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 4096 | SingleElementMoved |    11,880.0 ns |    523.09 ns |   232.25 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | SingleElementMoved |    15,291.8 ns |     75.55 ns |    33.54 ns |   1.29 |    0.02 |    3 |         - |          NA |
| PigeonSortInteger   | 4096 | SingleElementMoved |     9,781.6 ns |    236.48 ns |   123.68 ns |   0.82 |    0.02 |    1 |         - |          NA |
| BucketSort          | 4096 | SingleElementMoved |    20,914.3 ns |    288.02 ns |   127.88 ns |   1.76 |    0.03 |    3 |         - |          NA |
| BucketSortInteger   | 4096 | SingleElementMoved |    19,040.3 ns |    278.87 ns |   145.85 ns |   1.60 |    0.03 |    3 |         - |          NA |
| FlashSort           | 4096 | SingleElementMoved |    47,296.2 ns |    784.74 ns |   410.43 ns |   3.98 |    0.08 |    5 |         - |          NA |
| RadixLSD4Sort       | 4096 | SingleElementMoved |    72,113.8 ns |    553.54 ns |   289.51 ns |   6.07 |    0.11 |    6 |         - |          NA |
| RotateLSDRadixSort  | 4096 | SingleElementMoved |   857,806.8 ns |  2,725.78 ns | 1,425.64 ns |  72.23 |    1.31 |    8 |         - |          NA |
| RadixLSD256Sort     | 4096 | SingleElementMoved |    19,192.4 ns |    273.12 ns |   142.85 ns |   1.62 |    0.03 |    3 |         - |          NA |
| Radsort             | 4096 | SingleElementMoved |    44,444.9 ns |    267.19 ns |   139.74 ns |   3.74 |    0.07 |    5 |         - |          NA |
| RadixLSD10Sort      | 4096 | SingleElementMoved |    63,551.6 ns |    480.48 ns |   251.30 ns |   5.35 |    0.10 |    6 |         - |          NA |
| RadixMSD4Sort       | 4096 | SingleElementMoved |   102,396.8 ns |    838.06 ns |   372.10 ns |   8.62 |    0.16 |    7 |         - |          NA |
| RadixMSD10Sort      | 4096 | SingleElementMoved |    90,572.7 ns |    427.78 ns |   189.94 ns |   7.63 |    0.14 |    7 |         - |          NA |
| AmericanFlagSort    | 4096 | SingleElementMoved |    34,100.5 ns |    500.37 ns |   222.17 ns |   2.87 |    0.05 |    4 |         - |          NA |
| SpreadSort          | 4096 | SingleElementMoved |    17,886.8 ns |    258.20 ns |   114.64 ns |   1.51 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **4096** | **Sorted**             |    **18,109.7 ns** |    **661.90 ns** |   **346.19 ns** |   **1.55** |    **0.03** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Sorted             |    11,711.6 ns |    315.88 ns |   140.25 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 4096 | Sorted             |    15,212.2 ns |    205.81 ns |   107.64 ns |   1.30 |    0.02 |    4 |         - |          NA |
| PigeonSortInteger   | 4096 | Sorted             |     8,743.3 ns |    318.01 ns |   166.32 ns |   0.75 |    0.02 |    2 |         - |          NA |
| BucketSort          | 4096 | Sorted             |    19,838.1 ns |    322.72 ns |   168.79 ns |   1.69 |    0.02 |    4 |         - |          NA |
| BucketSortInteger   | 4096 | Sorted             |    15,844.2 ns |    195.38 ns |   102.19 ns |   1.35 |    0.02 |    4 |         - |          NA |
| FlashSort           | 4096 | Sorted             |    44,768.3 ns |    903.39 ns |   472.49 ns |   3.82 |    0.06 |    6 |         - |          NA |
| RadixLSD4Sort       | 4096 | Sorted             |    61,760.4 ns |    990.39 ns |   517.99 ns |   5.27 |    0.07 |    7 |         - |          NA |
| RotateLSDRadixSort  | 4096 | Sorted             |   853,418.4 ns |  3,049.08 ns | 1,353.81 ns |  72.88 |    0.84 |    9 |         - |          NA |
| RadixLSD256Sort     | 4096 | Sorted             |    17,923.2 ns |    428.62 ns |   224.17 ns |   1.53 |    0.03 |    4 |         - |          NA |
| Radsort             | 4096 | Sorted             |    42,657.8 ns |    696.35 ns |   309.18 ns |   3.64 |    0.05 |    6 |         - |          NA |
| RadixLSD10Sort      | 4096 | Sorted             |    63,135.5 ns |    202.64 ns |    89.97 ns |   5.39 |    0.06 |    7 |         - |          NA |
| RadixMSD4Sort       | 4096 | Sorted             |   100,729.1 ns |    783.59 ns |   347.92 ns |   8.60 |    0.10 |    8 |         - |          NA |
| RadixMSD10Sort      | 4096 | Sorted             |    88,800.7 ns |    542.89 ns |   283.94 ns |   7.58 |    0.09 |    8 |         - |          NA |
| AmericanFlagSort    | 4096 | Sorted             |    28,086.4 ns |    361.54 ns |   189.09 ns |   2.40 |    0.03 |    5 |         - |          NA |
| SpreadSort          | 4096 | Sorted             |     1,798.0 ns |     19.80 ns |     8.79 ns |   0.15 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **4096** | **Reversed**           |    **17,358.4 ns** |    **209.34 ns** |   **109.49 ns** |   **1.47** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Reversed           |    11,787.5 ns |    320.46 ns |   167.61 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | Reversed           |    16,439.0 ns |    251.60 ns |   131.59 ns |   1.39 |    0.02 |    3 |         - |          NA |
| PigeonSortInteger   | 4096 | Reversed           |     8,649.1 ns |    238.47 ns |   105.88 ns |   0.73 |    0.01 |    1 |         - |          NA |
| BucketSort          | 4096 | Reversed           |    20,905.9 ns |    134.56 ns |    59.74 ns |   1.77 |    0.02 |    3 |         - |          NA |
| BucketSortInteger   | 4096 | Reversed           |    16,068.6 ns |    200.17 ns |   104.69 ns |   1.36 |    0.02 |    3 |         - |          NA |
| FlashSort           | 4096 | Reversed           |    43,230.8 ns |    299.29 ns |   132.88 ns |   3.67 |    0.05 |    5 |         - |          NA |
| RadixLSD4Sort       | 4096 | Reversed           |    70,326.6 ns |    822.32 ns |   430.09 ns |   5.97 |    0.09 |    6 |         - |          NA |
| RotateLSDRadixSort  | 4096 | Reversed           |   870,010.4 ns |  2,359.07 ns | 1,047.44 ns |  73.82 |    0.99 |    8 |         - |          NA |
| RadixLSD256Sort     | 4096 | Reversed           |    17,950.8 ns |    164.97 ns |    86.28 ns |   1.52 |    0.02 |    3 |         - |          NA |
| Radsort             | 4096 | Reversed           |    41,742.0 ns |    337.28 ns |   149.76 ns |   3.54 |    0.05 |    5 |         - |          NA |
| RadixLSD10Sort      | 4096 | Reversed           |    63,130.6 ns |    776.54 ns |   344.79 ns |   5.36 |    0.08 |    6 |         - |          NA |
| RadixMSD4Sort       | 4096 | Reversed           |   103,469.5 ns |    910.95 ns |   404.47 ns |   8.78 |    0.12 |    7 |         - |          NA |
| RadixMSD10Sort      | 4096 | Reversed           |    87,955.8 ns |    411.30 ns |   215.12 ns |   7.46 |    0.10 |    7 |         - |          NA |
| AmericanFlagSort    | 4096 | Reversed           |    31,239.2 ns |    744.66 ns |   389.47 ns |   2.65 |    0.05 |    4 |         - |          NA |
| SpreadSort          | 4096 | Reversed           |    14,856.0 ns |    275.34 ns |   144.01 ns |   1.26 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **4096** | **PipeOrgan**          |    **18,725.0 ns** |    **337.88 ns** |   **176.72 ns** |   **1.51** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 4096 | PipeOrgan          |    12,438.8 ns |    253.40 ns |   132.53 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | PipeOrgan          |    15,360.9 ns |    275.86 ns |   122.48 ns |   1.24 |    0.02 |    3 |         - |          NA |
| PigeonSortInteger   | 4096 | PipeOrgan          |     9,849.2 ns |    330.45 ns |   172.83 ns |   0.79 |    0.02 |    1 |         - |          NA |
| BucketSort          | 4096 | PipeOrgan          |    20,789.5 ns |    434.87 ns |   227.45 ns |   1.67 |    0.02 |    3 |         - |          NA |
| BucketSortInteger   | 4096 | PipeOrgan          |    17,954.7 ns |    175.73 ns |    78.03 ns |   1.44 |    0.02 |    3 |         - |          NA |
| FlashSort           | 4096 | PipeOrgan          |    51,581.5 ns |    409.13 ns |   213.98 ns |   4.15 |    0.04 |    4 |         - |          NA |
| RadixLSD4Sort       | 4096 | PipeOrgan          |    53,885.2 ns |    711.80 ns |   372.29 ns |   4.33 |    0.05 |    4 |         - |          NA |
| RotateLSDRadixSort  | 4096 | PipeOrgan          |   856,303.8 ns |  2,070.46 ns | 1,082.89 ns |  68.85 |    0.70 |    7 |         - |          NA |
| RadixLSD256Sort     | 4096 | PipeOrgan          |    19,622.3 ns |    325.19 ns |   170.08 ns |   1.58 |    0.02 |    3 |         - |          NA |
| Radsort             | 4096 | PipeOrgan          |    43,225.5 ns |    987.50 ns |   516.48 ns |   3.48 |    0.05 |    4 |         - |          NA |
| RadixLSD10Sort      | 4096 | PipeOrgan          |    66,485.0 ns |  3,941.81 ns | 2,061.64 ns |   5.35 |    0.17 |    5 |         - |          NA |
| RadixMSD4Sort       | 4096 | PipeOrgan          |   103,243.1 ns |  1,125.60 ns |   499.77 ns |   8.30 |    0.09 |    6 |         - |          NA |
| RadixMSD10Sort      | 4096 | PipeOrgan          |    90,207.0 ns |    917.59 ns |   407.41 ns |   7.25 |    0.08 |    6 |         - |          NA |
| AmericanFlagSort    | 4096 | PipeOrgan          |    47,991.5 ns |    198.71 ns |    88.23 ns |   3.86 |    0.04 |    4 |         - |          NA |
| SpreadSort          | 4096 | PipeOrgan          |    20,608.1 ns |    274.51 ns |   121.89 ns |   1.66 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **4096** | **ManyDuplicates**     |    **16,968.5 ns** |    **200.14 ns** |    **88.86 ns** |   **1.51** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 4096 | ManyDuplicates     |    11,221.4 ns |    352.85 ns |   156.67 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | ManyDuplicates     |    19,667.6 ns |    115.98 ns |    51.49 ns |   1.75 |    0.02 |    3 |         - |          NA |
| PigeonSortInteger   | 4096 | ManyDuplicates     |     8,752.5 ns |    337.60 ns |   149.90 ns |   0.78 |    0.02 |    1 |         - |          NA |
| BucketSort          | 4096 | ManyDuplicates     |    30,174.1 ns |    192.42 ns |    85.44 ns |   2.69 |    0.04 |    4 |         - |          NA |
| BucketSortInteger   | 4096 | ManyDuplicates     |    16,826.7 ns |    412.33 ns |   215.66 ns |   1.50 |    0.03 |    3 |         - |          NA |
| FlashSort           | 4096 | ManyDuplicates     |    53,440.3 ns |    218.96 ns |    97.22 ns |   4.76 |    0.06 |    5 |         - |          NA |
| RadixLSD4Sort       | 4096 | ManyDuplicates     |    27,090.8 ns |    343.50 ns |   179.66 ns |   2.41 |    0.04 |    4 |         - |          NA |
| RotateLSDRadixSort  | 4096 | ManyDuplicates     |   771,127.3 ns |  2,266.49 ns | 1,185.42 ns |  68.73 |    0.92 |    6 |         - |          NA |
| RadixLSD256Sort     | 4096 | ManyDuplicates     |    16,431.5 ns |  8,568.44 ns | 4,481.46 ns |   1.46 |    0.38 |    3 |         - |          NA |
| Radsort             | 4096 | ManyDuplicates     |    42,489.1 ns |    364.30 ns |   190.54 ns |   3.79 |    0.05 |    4 |         - |          NA |
| RadixLSD10Sort      | 4096 | ManyDuplicates     |    34,841.0 ns |    194.49 ns |   101.72 ns |   3.11 |    0.04 |    4 |         - |          NA |
| RadixMSD4Sort       | 4096 | ManyDuplicates     |    28,256.6 ns |    167.60 ns |    74.42 ns |   2.52 |    0.03 |    4 |         - |          NA |
| RadixMSD10Sort      | 4096 | ManyDuplicates     |    36,466.6 ns |    569.68 ns |   252.94 ns |   3.25 |    0.05 |    4 |         - |          NA |
| AmericanFlagSort    | 4096 | ManyDuplicates     |    26,658.1 ns |    290.86 ns |   152.13 ns |   2.38 |    0.03 |    4 |         - |          NA |
| SpreadSort          | 4096 | ManyDuplicates     |    21,244.7 ns |    415.24 ns |   217.18 ns |   1.89 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **8192** | **Random**             |    **39,996.5 ns** |    **857.85 ns** |   **448.67 ns** |   **1.41** |    **0.02** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Random             |    28,359.8 ns |    270.13 ns |   119.94 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | Random             |    30,806.1 ns |    287.73 ns |   127.75 ns |   1.09 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | Random             |    19,927.2 ns |    156.47 ns |    69.47 ns |   0.70 |    0.00 |    1 |         - |          NA |
| BucketSort          | 8192 | Random             |    46,005.8 ns |  2,928.15 ns | 1,044.21 ns |   1.62 |    0.03 |    2 |         - |          NA |
| BucketSortInteger   | 8192 | Random             |    36,946.3 ns |    791.22 ns |   413.82 ns |   1.30 |    0.01 |    2 |         - |          NA |
| FlashSort           | 8192 | Random             |   138,175.3 ns |  4,135.87 ns | 1,836.35 ns |   4.87 |    0.06 |    3 |         - |          NA |
| RadixLSD4Sort       | 8192 | Random             |   116,162.8 ns |  1,678.72 ns |   878.00 ns |   4.10 |    0.03 |    3 |         - |          NA |
| RotateLSDRadixSort  | 8192 | Random             | 3,396,628.1 ns |  9,876.22 ns | 5,165.45 ns | 119.77 |    0.51 |    5 |         - |          NA |
| RadixLSD256Sort     | 8192 | Random             |    41,081.4 ns |  1,681.68 ns |   879.55 ns |   1.45 |    0.03 |    2 |         - |          NA |
| Radsort             | 8192 | Random             |    95,874.7 ns |    964.09 ns |   428.06 ns |   3.38 |    0.02 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | Random             |   128,430.6 ns |  1,042.69 ns |   462.96 ns |   4.53 |    0.02 |    3 |         - |          NA |
| RadixMSD4Sort       | 8192 | Random             |   216,327.7 ns |    894.04 ns |   396.96 ns |   7.63 |    0.03 |    4 |         - |          NA |
| RadixMSD10Sort      | 8192 | Random             |   181,130.5 ns |  1,771.47 ns |   786.54 ns |   6.39 |    0.04 |    4 |         - |          NA |
| AmericanFlagSort    | 8192 | Random             |   122,400.3 ns |    842.50 ns |   440.65 ns |   4.32 |    0.02 |    3 |         - |          NA |
| SpreadSort          | 8192 | Random             |    98,758.8 ns |    587.83 ns |   261.00 ns |   3.48 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **8192** | **SingleElementMoved** |    **37,704.0 ns** |    **614.24 ns** |   **219.05 ns** |   **1.57** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | SingleElementMoved |    23,989.5 ns |    399.44 ns |   208.92 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | SingleElementMoved |    30,517.9 ns |    876.37 ns |   458.36 ns |   1.27 |    0.02 |    3 |         - |          NA |
| PigeonSortInteger   | 8192 | SingleElementMoved |    19,645.3 ns |    370.50 ns |   193.78 ns |   0.82 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | SingleElementMoved |    41,349.9 ns |    292.06 ns |   129.68 ns |   1.72 |    0.01 |    3 |         - |          NA |
| BucketSortInteger   | 8192 | SingleElementMoved |    33,580.0 ns |    454.82 ns |   237.88 ns |   1.40 |    0.01 |    3 |         - |          NA |
| FlashSort           | 8192 | SingleElementMoved |    95,193.5 ns |  1,219.34 ns |   637.74 ns |   3.97 |    0.04 |    5 |         - |          NA |
| RadixLSD4Sort       | 8192 | SingleElementMoved |   162,641.5 ns |  1,746.66 ns |   913.54 ns |   6.78 |    0.07 |    7 |         - |          NA |
| RotateLSDRadixSort  | 8192 | SingleElementMoved | 2,175,766.9 ns |  3,435.62 ns | 1,796.89 ns |  90.70 |    0.74 |    8 |         - |          NA |
| RadixLSD256Sort     | 8192 | SingleElementMoved |    40,011.1 ns |    475.40 ns |   211.08 ns |   1.67 |    0.02 |    3 |         - |          NA |
| Radsort             | 8192 | SingleElementMoved |    61,193.6 ns |  1,341.27 ns |   595.53 ns |   2.55 |    0.03 |    4 |         - |          NA |
| RadixLSD10Sort      | 8192 | SingleElementMoved |   127,478.4 ns |    405.29 ns |   179.95 ns |   5.31 |    0.04 |    6 |         - |          NA |
| RadixMSD4Sort       | 8192 | SingleElementMoved |   216,199.5 ns |  1,347.89 ns |   598.47 ns |   9.01 |    0.08 |    7 |         - |          NA |
| RadixMSD10Sort      | 8192 | SingleElementMoved |   180,493.3 ns |  1,932.99 ns | 1,010.99 ns |   7.52 |    0.07 |    7 |         - |          NA |
| AmericanFlagSort    | 8192 | SingleElementMoved |    67,864.4 ns |  1,312.04 ns |   582.55 ns |   2.83 |    0.03 |    4 |         - |          NA |
| SpreadSort          | 8192 | SingleElementMoved |    39,738.6 ns |    340.13 ns |   177.90 ns |   1.66 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **8192** | **Sorted**             |    **36,623.2 ns** |  **1,260.79 ns** |   **659.42 ns** |   **1.56** |    **0.03** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Sorted             |    23,460.3 ns |    276.52 ns |   144.63 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 8192 | Sorted             |    30,371.4 ns |    530.94 ns |   235.74 ns |   1.29 |    0.01 |    4 |         - |          NA |
| PigeonSortInteger   | 8192 | Sorted             |    17,143.2 ns |     84.23 ns |    30.04 ns |   0.73 |    0.00 |    2 |         - |          NA |
| BucketSort          | 8192 | Sorted             |    39,665.6 ns |    414.95 ns |   217.03 ns |   1.69 |    0.01 |    4 |         - |          NA |
| BucketSortInteger   | 8192 | Sorted             |    30,938.5 ns |    219.62 ns |   114.87 ns |   1.32 |    0.01 |    4 |         - |          NA |
| FlashSort           | 8192 | Sorted             |    90,046.5 ns |    935.20 ns |   489.13 ns |   3.84 |    0.03 |    6 |         - |          NA |
| RadixLSD4Sort       | 8192 | Sorted             |   158,514.7 ns |  1,668.03 ns |   872.41 ns |   6.76 |    0.05 |    8 |         - |          NA |
| RotateLSDRadixSort  | 8192 | Sorted             | 2,165,129.4 ns |  2,807.99 ns | 1,468.63 ns |  92.29 |    0.54 |   10 |         - |          NA |
| RadixLSD256Sort     | 8192 | Sorted             |    38,183.6 ns |    630.75 ns |   280.06 ns |   1.63 |    0.01 |    4 |         - |          NA |
| Radsort             | 8192 | Sorted             |    57,338.7 ns |    500.39 ns |   261.71 ns |   2.44 |    0.02 |    5 |         - |          NA |
| RadixLSD10Sort      | 8192 | Sorted             |   126,215.3 ns |    783.47 ns |   347.87 ns |   5.38 |    0.03 |    7 |         - |          NA |
| RadixMSD4Sort       | 8192 | Sorted             |   216,829.8 ns |    925.98 ns |   411.14 ns |   9.24 |    0.06 |    9 |         - |          NA |
| RadixMSD10Sort      | 8192 | Sorted             |   178,239.8 ns |    592.55 ns |   263.10 ns |   7.60 |    0.05 |    8 |         - |          NA |
| AmericanFlagSort    | 8192 | Sorted             |    55,575.6 ns |    235.24 ns |   123.03 ns |   2.37 |    0.01 |    5 |         - |          NA |
| SpreadSort          | 8192 | Sorted             |     3,518.6 ns |    142.48 ns |    63.26 ns |   0.15 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **8192** | **Reversed**           |    **34,580.3 ns** |    **393.49 ns** |   **205.80 ns** |   **1.46** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Reversed           |    23,760.7 ns |    621.59 ns |   325.10 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | Reversed           |    32,608.1 ns |    366.09 ns |   191.47 ns |   1.37 |    0.02 |    3 |         - |          NA |
| PigeonSortInteger   | 8192 | Reversed           |    17,483.7 ns |    425.57 ns |   222.58 ns |   0.74 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | Reversed           |    41,317.1 ns |    386.05 ns |   201.91 ns |   1.74 |    0.02 |    3 |         - |          NA |
| BucketSortInteger   | 8192 | Reversed           |    31,716.2 ns |    239.27 ns |   125.14 ns |   1.34 |    0.02 |    3 |         - |          NA |
| FlashSort           | 8192 | Reversed           |    83,895.7 ns |  1,003.34 ns |   445.49 ns |   3.53 |    0.05 |    5 |         - |          NA |
| RadixLSD4Sort       | 8192 | Reversed           |   161,946.1 ns |  1,002.32 ns |   524.23 ns |   6.82 |    0.09 |    7 |         - |          NA |
| RotateLSDRadixSort  | 8192 | Reversed           | 2,203,570.0 ns |  3,176.50 ns | 1,661.37 ns |  92.76 |    1.19 |    9 |         - |          NA |
| RadixLSD256Sort     | 8192 | Reversed           |    37,210.9 ns |    486.76 ns |   254.58 ns |   1.57 |    0.02 |    3 |         - |          NA |
| Radsort             | 8192 | Reversed           |    57,042.0 ns |    517.49 ns |   229.77 ns |   2.40 |    0.03 |    4 |         - |          NA |
| RadixLSD10Sort      | 8192 | Reversed           |   126,271.5 ns |    359.12 ns |   159.45 ns |   5.32 |    0.07 |    6 |         - |          NA |
| RadixMSD4Sort       | 8192 | Reversed           |   217,315.0 ns |  2,137.40 ns | 1,117.90 ns |   9.15 |    0.13 |    8 |         - |          NA |
| RadixMSD10Sort      | 8192 | Reversed           |   177,402.2 ns |  2,258.15 ns | 1,181.06 ns |   7.47 |    0.11 |    7 |         - |          NA |
| AmericanFlagSort    | 8192 | Reversed           |    61,902.4 ns |    984.39 ns |   514.86 ns |   2.61 |    0.04 |    4 |         - |          NA |
| SpreadSort          | 8192 | Reversed           |    56,847.2 ns |  1,067.32 ns |   558.23 ns |   2.39 |    0.04 |    4 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **8192** | **PipeOrgan**          |    **36,701.4 ns** |    **219.17 ns** |    **97.31 ns** |   **1.41** |    **0.01** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | PipeOrgan          |    26,088.2 ns |    449.28 ns |   234.98 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | PipeOrgan          |    30,414.5 ns |    199.55 ns |   104.37 ns |   1.17 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | PipeOrgan          |    20,029.2 ns |  1,021.17 ns |   453.41 ns |   0.77 |    0.02 |    1 |         - |          NA |
| BucketSort          | 8192 | PipeOrgan          |    42,159.7 ns |    371.02 ns |   164.74 ns |   1.62 |    0.01 |    2 |         - |          NA |
| BucketSortInteger   | 8192 | PipeOrgan          |    33,769.5 ns |    277.21 ns |   123.08 ns |   1.29 |    0.01 |    2 |         - |          NA |
| FlashSort           | 8192 | PipeOrgan          |   114,295.7 ns |  1,204.57 ns |   534.84 ns |   4.38 |    0.04 |    4 |         - |          NA |
| RadixLSD4Sort       | 8192 | PipeOrgan          |   128,285.6 ns |    832.09 ns |   369.46 ns |   4.92 |    0.04 |    4 |         - |          NA |
| RotateLSDRadixSort  | 8192 | PipeOrgan          | 2,908,001.7 ns |  4,601.01 ns | 2,042.88 ns | 111.48 |    0.95 |    7 |         - |          NA |
| RadixLSD256Sort     | 8192 | PipeOrgan          |    40,384.0 ns |    262.23 ns |   116.43 ns |   1.55 |    0.01 |    2 |         - |          NA |
| Radsort             | 8192 | PipeOrgan          |    59,397.6 ns |    514.67 ns |   269.18 ns |   2.28 |    0.02 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | PipeOrgan          |   127,856.0 ns |    998.30 ns |   522.13 ns |   4.90 |    0.05 |    4 |         - |          NA |
| RadixMSD4Sort       | 8192 | PipeOrgan          |   226,458.2 ns |  2,421.37 ns | 1,266.42 ns |   8.68 |    0.09 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | PipeOrgan          |   179,054.0 ns |    888.22 ns |   464.56 ns |   6.86 |    0.06 |    5 |         - |          NA |
| AmericanFlagSort    | 8192 | PipeOrgan          |    98,628.6 ns |    934.80 ns |   415.06 ns |   3.78 |    0.04 |    4 |         - |          NA |
| SpreadSort          | 8192 | PipeOrgan          |    68,420.8 ns |  1,261.61 ns |   560.16 ns |   2.62 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **8192** | **ManyDuplicates**     |    **34,333.2 ns** |    **185.39 ns** |    **96.96 ns** |   **1.52** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | ManyDuplicates     |    22,653.1 ns |    530.73 ns |   277.58 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | ManyDuplicates     |    54,543.6 ns |    880.95 ns |   391.15 ns |   2.41 |    0.03 |    5 |         - |          NA |
| PigeonSortInteger   | 8192 | ManyDuplicates     |    17,383.6 ns |    404.21 ns |   211.41 ns |   0.77 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | ManyDuplicates     |    60,499.7 ns |    410.12 ns |   214.50 ns |   2.67 |    0.03 |    5 |         - |          NA |
| BucketSortInteger   | 8192 | ManyDuplicates     |    32,124.5 ns |    417.24 ns |   218.23 ns |   1.42 |    0.02 |    3 |         - |          NA |
| FlashSort           | 8192 | ManyDuplicates     |   107,045.0 ns |    464.32 ns |   206.16 ns |   4.73 |    0.06 |    6 |         - |          NA |
| RadixLSD4Sort       | 8192 | ManyDuplicates     |    54,435.2 ns |    338.61 ns |   150.35 ns |   2.40 |    0.03 |    5 |         - |          NA |
| RotateLSDRadixSort  | 8192 | ManyDuplicates     | 1,628,249.3 ns |  6,990.54 ns | 3,656.19 ns |  71.89 |    0.85 |    7 |         - |          NA |
| RadixLSD256Sort     | 8192 | ManyDuplicates     |    25,358.1 ns |    378.42 ns |   168.02 ns |   1.12 |    0.01 |    2 |         - |          NA |
| Radsort             | 8192 | ManyDuplicates     |    53,807.0 ns |    507.03 ns |   180.81 ns |   2.38 |    0.03 |    5 |         - |          NA |
| RadixLSD10Sort      | 8192 | ManyDuplicates     |    69,370.8 ns |    678.86 ns |   301.42 ns |   3.06 |    0.04 |    5 |         - |          NA |
| RadixMSD4Sort       | 8192 | ManyDuplicates     |    55,906.5 ns |    401.94 ns |   210.22 ns |   2.47 |    0.03 |    5 |         - |          NA |
| RadixMSD10Sort      | 8192 | ManyDuplicates     |    73,530.5 ns |    508.48 ns |   225.77 ns |   3.25 |    0.04 |    5 |         - |          NA |
| AmericanFlagSort    | 8192 | ManyDuplicates     |    52,799.5 ns |    461.81 ns |   241.54 ns |   2.33 |    0.03 |    5 |         - |          NA |
| SpreadSort          | 8192 | ManyDuplicates     |    42,455.4 ns |    675.84 ns |   353.48 ns |   1.87 |    0.03 |    4 |         - |          NA |

### ExchangeBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.52 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method             | Size | Pattern            | Mean         | Error       | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------- |----- |------------------- |-------------:|------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **BubbleSort**         | **256**  | **Random**             |  **41,966.5 ns** |   **220.03 ns** |    **78.46 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Random             |  27,237.9 ns |   219.30 ns |    97.37 ns |  0.65 |    0.00 |    2 |         - |          NA |
| OddEvenSort        | 256  | Random             |  42,005.3 ns |   278.37 ns |   123.60 ns |  1.00 |    0.00 |    3 |         - |          NA |
| CombSort           | 256  | Random             |   7,887.7 ns |    86.50 ns |    38.41 ns |  0.19 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | Random             |  23,035.0 ns |   105.03 ns |    46.63 ns |  0.55 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **256**  | **SingleElementMoved** |   **4,514.7 ns** |    **66.99 ns** |    **23.89 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | SingleElementMoved |     326.5 ns |    14.65 ns |     6.50 ns |  0.07 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 256  | SingleElementMoved |   8,490.0 ns |   212.04 ns |    94.15 ns |  1.88 |    0.02 |    4 |         - |          NA |
| CombSort           | 256  | SingleElementMoved |   2,725.9 ns |   264.22 ns |   138.19 ns |  0.60 |    0.03 |    2 |         - |          NA |
| CircleSort         | 256  | SingleElementMoved |  10,220.2 ns |   110.44 ns |    49.03 ns |  2.26 |    0.01 |    5 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **256**  | **Sorted**             |     **223.4 ns** |    **17.44 ns** |     **7.75 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Sorted             |     175.4 ns |    13.87 ns |     7.25 ns |  0.79 |    0.04 |    1 |         - |          NA |
| OddEvenSort        | 256  | Sorted             |     179.4 ns |     3.61 ns |     1.60 ns |  0.80 |    0.03 |    1 |         - |          NA |
| CombSort           | 256  | Sorted             |   2,245.6 ns |     4.48 ns |     1.99 ns | 10.06 |    0.31 |    3 |         - |          NA |
| CircleSort         | 256  | Sorted             |   2,360.1 ns |     8.31 ns |     3.69 ns | 10.57 |    0.33 |    3 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **256**  | **Reversed**           |  **22,961.8 ns** |   **312.73 ns** |   **163.56 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Reversed           |  21,509.3 ns |   209.23 ns |    92.90 ns |  0.94 |    0.01 |    3 |         - |          NA |
| OddEvenSort        | 256  | Reversed           |  19,807.5 ns |   392.38 ns |   174.22 ns |  0.86 |    0.01 |    3 |         - |          NA |
| CombSort           | 256  | Reversed           |   2,453.0 ns |   143.29 ns |    63.62 ns |  0.11 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | Reversed           |   3,325.1 ns |    85.54 ns |    37.98 ns |  0.14 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **256**  | **PipeOrgan**          |  **26,788.7 ns** |   **200.98 ns** |    **89.23 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 256  | PipeOrgan          |  13,906.9 ns |   245.21 ns |   128.25 ns |  0.52 |    0.00 |    2 |         - |          NA |
| OddEvenSort        | 256  | PipeOrgan          |  21,890.8 ns |   315.60 ns |   165.06 ns |  0.82 |    0.01 |    3 |         - |          NA |
| CombSort           | 256  | PipeOrgan          |   2,427.5 ns |    77.73 ns |    34.51 ns |  0.09 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | PipeOrgan          |  15,111.0 ns |   273.12 ns |   121.27 ns |  0.56 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **256**  | **ManyDuplicates**     |  **41,070.1 ns** |   **322.89 ns** |   **143.36 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 256  | ManyDuplicates     |  26,736.6 ns |   148.89 ns |    77.87 ns |  0.65 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 256  | ManyDuplicates     |  43,900.9 ns |   322.90 ns |   168.88 ns |  1.07 |    0.01 |    4 |         - |          NA |
| CombSort           | 256  | ManyDuplicates     |   6,127.1 ns |   145.87 ns |    64.77 ns |  0.15 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | ManyDuplicates     |  19,960.8 ns |   235.34 ns |   104.49 ns |  0.49 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **1024** | **Random**             | **531,383.2 ns** | **3,140.09 ns** | **1,642.33 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Random             | 297,685.4 ns | 2,441.67 ns | 1,277.04 ns |  0.56 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | Random             | 480,977.9 ns | 3,183.63 ns | 1,413.55 ns |  0.91 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | Random             |  42,685.2 ns |   176.38 ns |    92.25 ns |  0.08 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | Random             | 127,238.8 ns |   646.34 ns |   286.98 ns |  0.24 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **1024** | **SingleElementMoved** |  **66,806.1 ns** |   **230.07 ns** |   **120.33 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | SingleElementMoved |   1,119.2 ns |   120.86 ns |    53.66 ns |  0.02 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 1024 | SingleElementMoved | 124,119.7 ns | 5,417.13 ns | 2,405.24 ns |  1.86 |    0.03 |    4 |         - |          NA |
| CombSort           | 1024 | SingleElementMoved |  12,657.8 ns |   289.90 ns |   151.62 ns |  0.19 |    0.00 |    2 |         - |          NA |
| CircleSort         | 1024 | SingleElementMoved |  58,902.8 ns |   360.09 ns |   159.88 ns |  0.88 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **1024** | **Sorted**             |     **854.5 ns** |    **15.83 ns** |     **7.03 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Sorted             |     597.4 ns |     9.54 ns |     4.99 ns |  0.70 |    0.01 |    1 |         - |          NA |
| OddEvenSort        | 1024 | Sorted             |     733.3 ns |    42.63 ns |    18.93 ns |  0.86 |    0.02 |    2 |         - |          NA |
| CombSort           | 1024 | Sorted             |  11,633.0 ns |   360.73 ns |   188.67 ns | 13.62 |    0.23 |    4 |         - |          NA |
| CircleSort         | 1024 | Sorted             |   7,500.3 ns |   428.20 ns |   223.95 ns |  8.78 |    0.26 |    3 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **1024** | **Reversed**           | **340,996.4 ns** | **2,095.11 ns** | **1,095.78 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Reversed           | 338,981.4 ns | 1,261.25 ns |   560.00 ns |  0.99 |    0.00 |    2 |         - |          NA |
| OddEvenSort        | 1024 | Reversed           | 319,023.4 ns | 2,715.41 ns | 1,420.21 ns |  0.94 |    0.00 |    2 |         - |          NA |
| CombSort           | 1024 | Reversed           |  12,791.9 ns |   260.72 ns |   136.36 ns |  0.04 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | Reversed           |  14,737.8 ns |   213.33 ns |   111.58 ns |  0.04 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **1024** | **PipeOrgan**          | **395,196.1 ns** | **1,577.86 ns** |   **700.58 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | PipeOrgan          | 222,144.6 ns | 2,826.64 ns | 1,478.39 ns |  0.56 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | PipeOrgan          | 349,430.9 ns | 1,624.43 ns |   721.26 ns |  0.88 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | PipeOrgan          |  12,972.5 ns |   486.74 ns |   216.11 ns |  0.03 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | PipeOrgan          |  81,355.8 ns |   171.57 ns |    76.18 ns |  0.21 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **1024** | **ManyDuplicates**     | **529,491.6 ns** | **5,449.47 ns** | **2,850.18 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | ManyDuplicates     | 291,141.6 ns |   951.24 ns |   422.36 ns |  0.55 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | ManyDuplicates     | 496,721.2 ns | 1,344.13 ns |   596.80 ns |  0.94 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | ManyDuplicates     |  28,173.6 ns |   204.92 ns |    73.07 ns |  0.05 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | ManyDuplicates     |  98,931.1 ns |   408.94 ns |   181.57 ns |  0.19 |    0.00 |    2 |         - |          NA |

### HeapBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.52 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method           | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **HeapSort**         | **256**  | **Random**             |     **6,768.2 ns** |    **135.26 ns** |     **60.06 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | Random             |     6,931.9 ns |    549.16 ns |    287.22 ns |  1.02 |    0.04 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | Random             |    10,633.6 ns |    119.11 ns |     62.30 ns |  1.57 |    0.02 |    2 |         - |          NA |
| BottomupHeapSort | 256  | Random             |     7,021.7 ns |    398.19 ns |    208.26 ns |  1.04 |    0.03 |    1 |         - |          NA |
| WeakHeapSort     | 256  | Random             |    12,290.6 ns |    386.37 ns |    171.55 ns |  1.82 |    0.03 |    2 |         - |          NA |
| SmoothSort       | 256  | Random             |    13,512.2 ns |    209.78 ns |     93.14 ns |  2.00 |    0.02 |    2 |         - |          NA |
| PoplarHeapSort   | 256  | Random             |    13,400.9 ns |    157.73 ns |     56.25 ns |  1.98 |    0.02 |    2 |         - |          NA |
| TournamentSort   | 256  | Random             |    13,495.7 ns |    190.98 ns |     84.80 ns |  1.99 |    0.02 |    2 |         - |          NA |
| BinomialHeapSort | 256  | Random             |    27,291.5 ns |    548.62 ns |    243.59 ns |  4.03 |    0.05 |    3 |         - |          NA |
| PairingHeapSort  | 256  | Random             |    16,200.5 ns |    208.43 ns |     92.55 ns |  2.39 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **SingleElementMoved** |     **5,859.8 ns** |    **308.47 ns** |    **161.33 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 256  | SingleElementMoved |     5,535.1 ns |    147.55 ns |     77.17 ns |  0.95 |    0.03 |    4 |         - |          NA |
| TernaryHeapSort  | 256  | SingleElementMoved |     3,940.0 ns |    576.60 ns |    301.57 ns |  0.67 |    0.05 |    3 |         - |          NA |
| BottomupHeapSort | 256  | SingleElementMoved |     5,777.8 ns |    296.98 ns |    155.33 ns |  0.99 |    0.04 |    4 |         - |          NA |
| WeakHeapSort     | 256  | SingleElementMoved |     5,578.1 ns |    407.53 ns |    213.15 ns |  0.95 |    0.04 |    4 |         - |          NA |
| SmoothSort       | 256  | SingleElementMoved |     1,295.7 ns |     76.93 ns |     34.16 ns |  0.22 |    0.01 |    1 |         - |          NA |
| PoplarHeapSort   | 256  | SingleElementMoved |     3,058.0 ns |    266.88 ns |    139.58 ns |  0.52 |    0.03 |    2 |         - |          NA |
| TournamentSort   | 256  | SingleElementMoved |     4,536.7 ns |    447.31 ns |    233.95 ns |  0.77 |    0.04 |    3 |         - |          NA |
| BinomialHeapSort | 256  | SingleElementMoved |     6,169.9 ns |    301.95 ns |    157.93 ns |  1.05 |    0.04 |    4 |         - |          NA |
| PairingHeapSort  | 256  | SingleElementMoved |     4,366.8 ns |    227.86 ns |    119.17 ns |  0.75 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **Sorted**             |     **5,635.2 ns** |    **211.96 ns** |    **110.86 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 256  | Sorted             |     5,284.5 ns |    327.12 ns |    171.09 ns |  0.94 |    0.03 |    4 |         - |          NA |
| TernaryHeapSort  | 256  | Sorted             |     3,249.1 ns |    278.56 ns |    123.68 ns |  0.58 |    0.02 |    3 |         - |          NA |
| BottomupHeapSort | 256  | Sorted             |     5,543.5 ns |    269.00 ns |    140.69 ns |  0.98 |    0.03 |    4 |         - |          NA |
| WeakHeapSort     | 256  | Sorted             |     4,989.2 ns |    251.83 ns |    131.71 ns |  0.89 |    0.03 |    4 |         - |          NA |
| SmoothSort       | 256  | Sorted             |       920.5 ns |     27.77 ns |     12.33 ns |  0.16 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 256  | Sorted             |     2,601.6 ns |     34.69 ns |     15.40 ns |  0.46 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 256  | Sorted             |     4,355.7 ns |    311.26 ns |    162.79 ns |  0.77 |    0.03 |    4 |         - |          NA |
| BinomialHeapSort | 256  | Sorted             |     5,282.5 ns |    305.57 ns |    159.82 ns |  0.94 |    0.03 |    4 |         - |          NA |
| PairingHeapSort  | 256  | Sorted             |     4,311.0 ns |    365.64 ns |    191.24 ns |  0.77 |    0.04 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **Reversed**           |     **5,185.0 ns** |    **265.07 ns** |    **138.64 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 256  | Reversed           |     5,685.5 ns |    238.93 ns |    124.97 ns |  1.10 |    0.04 |    3 |         - |          NA |
| TernaryHeapSort  | 256  | Reversed           |     2,768.5 ns |    282.56 ns |    147.79 ns |  0.53 |    0.03 |    2 |         - |          NA |
| BottomupHeapSort | 256  | Reversed           |     5,676.0 ns |    184.75 ns |     96.63 ns |  1.10 |    0.03 |    3 |         - |          NA |
| WeakHeapSort     | 256  | Reversed           |     5,697.0 ns |    304.64 ns |    159.33 ns |  1.10 |    0.04 |    3 |         - |          NA |
| SmoothSort       | 256  | Reversed           |     3,864.4 ns |    202.38 ns |    105.85 ns |  0.75 |    0.03 |    3 |         - |          NA |
| PoplarHeapSort   | 256  | Reversed           |     4,614.5 ns |    236.32 ns |    123.60 ns |  0.89 |    0.03 |    3 |         - |          NA |
| TournamentSort   | 256  | Reversed           |     4,320.0 ns |    209.23 ns |    109.43 ns |  0.83 |    0.03 |    3 |         - |          NA |
| BinomialHeapSort | 256  | Reversed           |     5,301.1 ns |    416.58 ns |    217.88 ns |  1.02 |    0.05 |    3 |         - |          NA |
| PairingHeapSort  | 256  | Reversed           |     1,988.9 ns |     26.02 ns |     11.55 ns |  0.38 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **PipeOrgan**          |     **5,402.8 ns** |    **287.99 ns** |    **150.62 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 256  | PipeOrgan          |     5,127.2 ns |    129.96 ns |     67.97 ns |  0.95 |    0.03 |    2 |         - |          NA |
| TernaryHeapSort  | 256  | PipeOrgan          |     3,043.6 ns |    268.21 ns |    119.09 ns |  0.56 |    0.03 |    1 |         - |          NA |
| BottomupHeapSort | 256  | PipeOrgan          |     5,587.5 ns |    351.68 ns |    183.94 ns |  1.03 |    0.04 |    2 |         - |          NA |
| WeakHeapSort     | 256  | PipeOrgan          |     5,354.4 ns |    402.26 ns |    210.39 ns |  0.99 |    0.04 |    2 |         - |          NA |
| SmoothSort       | 256  | PipeOrgan          |     4,034.5 ns |    247.37 ns |    129.38 ns |  0.75 |    0.03 |    2 |         - |          NA |
| PoplarHeapSort   | 256  | PipeOrgan          |     4,755.8 ns |    442.31 ns |    231.34 ns |  0.88 |    0.05 |    2 |         - |          NA |
| TournamentSort   | 256  | PipeOrgan          |     3,157.9 ns |    262.21 ns |    137.14 ns |  0.58 |    0.03 |    1 |         - |          NA |
| BinomialHeapSort | 256  | PipeOrgan          |     5,905.7 ns |     96.24 ns |     42.73 ns |  1.09 |    0.03 |    2 |         - |          NA |
| PairingHeapSort  | 256  | PipeOrgan          |     5,379.4 ns |    280.48 ns |    146.69 ns |  1.00 |    0.04 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **ManyDuplicates**     |     **6,652.2 ns** |    **243.51 ns** |    **127.36 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | ManyDuplicates     |     6,650.9 ns |    323.86 ns |    143.80 ns |  1.00 |    0.03 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | ManyDuplicates     |    10,044.8 ns |    430.92 ns |    225.38 ns |  1.51 |    0.04 |    2 |         - |          NA |
| BottomupHeapSort | 256  | ManyDuplicates     |     6,835.4 ns |    433.31 ns |    226.63 ns |  1.03 |    0.04 |    1 |         - |          NA |
| WeakHeapSort     | 256  | ManyDuplicates     |    10,783.5 ns |    383.36 ns |    200.51 ns |  1.62 |    0.04 |    2 |         - |          NA |
| SmoothSort       | 256  | ManyDuplicates     |    12,612.9 ns |    172.92 ns |     76.78 ns |  1.90 |    0.04 |    2 |         - |          NA |
| PoplarHeapSort   | 256  | ManyDuplicates     |    12,824.8 ns |    197.75 ns |     70.52 ns |  1.93 |    0.04 |    2 |         - |          NA |
| TournamentSort   | 256  | ManyDuplicates     |    13,459.4 ns |    366.26 ns |    191.56 ns |  2.02 |    0.05 |    2 |         - |          NA |
| BinomialHeapSort | 256  | ManyDuplicates     |    24,639.5 ns |    188.09 ns |     67.08 ns |  3.71 |    0.07 |    3 |         - |          NA |
| PairingHeapSort  | 256  | ManyDuplicates     |    15,333.8 ns |    229.86 ns |    102.06 ns |  2.31 |    0.04 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Random**             |    **30,746.0 ns** |    **262.48 ns** |    **137.28 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Random             |    31,027.7 ns |    518.98 ns |    271.44 ns |  1.01 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | Random             |    52,868.3 ns |    660.00 ns |    345.19 ns |  1.72 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | Random             |    32,444.7 ns |    305.20 ns |    159.63 ns |  1.06 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | Random             |    64,555.4 ns |    323.20 ns |    143.50 ns |  2.10 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 1024 | Random             |    71,315.9 ns |    589.02 ns |    261.53 ns |  2.32 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 1024 | Random             |    66,320.4 ns |    209.09 ns |    109.36 ns |  2.16 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 1024 | Random             |    66,285.0 ns |    958.93 ns |    501.54 ns |  2.16 |    0.02 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | Random             |   156,297.0 ns |    727.41 ns |    380.45 ns |  5.08 |    0.02 |    4 |         - |          NA |
| PairingHeapSort  | 1024 | Random             |    80,834.4 ns |  1,190.37 ns |    528.53 ns |  2.63 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **SingleElementMoved** |    **25,696.6 ns** |    **383.61 ns** |    **200.63 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 1024 | SingleElementMoved |    25,070.8 ns |    321.36 ns |    168.08 ns |  0.98 |    0.01 |    3 |         - |          NA |
| TernaryHeapSort  | 1024 | SingleElementMoved |    19,394.0 ns |  1,399.09 ns |    731.75 ns |  0.75 |    0.03 |    3 |         - |          NA |
| BottomupHeapSort | 1024 | SingleElementMoved |    24,992.1 ns |    305.42 ns |    159.74 ns |  0.97 |    0.01 |    3 |         - |          NA |
| WeakHeapSort     | 1024 | SingleElementMoved |    25,856.0 ns |    377.96 ns |    167.82 ns |  1.01 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 1024 | SingleElementMoved |     4,917.9 ns |    125.44 ns |     55.70 ns |  0.19 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 1024 | SingleElementMoved |    12,985.7 ns |    329.24 ns |    146.18 ns |  0.51 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 1024 | SingleElementMoved |    21,438.4 ns |    690.55 ns |    306.61 ns |  0.83 |    0.01 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | SingleElementMoved |    26,508.8 ns |    352.02 ns |    184.11 ns |  1.03 |    0.01 |    3 |         - |          NA |
| PairingHeapSort  | 1024 | SingleElementMoved |    18,133.3 ns |  2,901.88 ns |  1,288.45 ns |  0.71 |    0.05 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Sorted**             |    **25,045.0 ns** |    **331.79 ns** |    **173.53 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Sorted             |    23,336.1 ns |    236.28 ns |    123.58 ns |  0.93 |    0.01 |    4 |         - |          NA |
| TernaryHeapSort  | 1024 | Sorted             |    15,342.2 ns |    821.86 ns |    429.85 ns |  0.61 |    0.02 |    3 |         - |          NA |
| BottomupHeapSort | 1024 | Sorted             |    24,502.7 ns |    263.45 ns |    116.97 ns |  0.98 |    0.01 |    4 |         - |          NA |
| WeakHeapSort     | 1024 | Sorted             |    25,215.8 ns |    496.08 ns |    259.46 ns |  1.01 |    0.01 |    4 |         - |          NA |
| SmoothSort       | 1024 | Sorted             |     3,727.3 ns |     88.88 ns |     46.49 ns |  0.15 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 1024 | Sorted             |    11,640.1 ns |    209.22 ns |    109.43 ns |  0.46 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 1024 | Sorted             |    20,095.5 ns |    453.92 ns |    201.54 ns |  0.80 |    0.01 |    4 |         - |          NA |
| BinomialHeapSort | 1024 | Sorted             |    22,905.1 ns |    417.29 ns |    218.25 ns |  0.91 |    0.01 |    4 |         - |          NA |
| PairingHeapSort  | 1024 | Sorted             |    23,617.4 ns | 10,459.87 ns |  5,470.71 ns |  0.94 |    0.21 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Reversed**           |    **23,068.1 ns** |    **328.14 ns** |    **171.62 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Reversed           |    25,116.1 ns |    298.35 ns |    132.47 ns |  1.09 |    0.01 |    3 |         - |          NA |
| TernaryHeapSort  | 1024 | Reversed           |    13,146.5 ns |    573.11 ns |    254.46 ns |  0.57 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | Reversed           |    25,340.7 ns |    420.16 ns |    219.75 ns |  1.10 |    0.01 |    3 |         - |          NA |
| WeakHeapSort     | 1024 | Reversed           |    27,986.3 ns |    585.82 ns |    260.11 ns |  1.21 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 1024 | Reversed           |    19,089.2 ns |    620.37 ns |    221.23 ns |  0.83 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 1024 | Reversed           |    21,206.4 ns |    871.08 ns |    455.59 ns |  0.92 |    0.02 |    3 |         - |          NA |
| TournamentSort   | 1024 | Reversed           |    20,849.4 ns |  1,021.75 ns |    453.67 ns |  0.90 |    0.02 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | Reversed           |    22,931.1 ns |    545.92 ns |    194.68 ns |  0.99 |    0.01 |    3 |         - |          NA |
| PairingHeapSort  | 1024 | Reversed           |     8,109.4 ns |    326.34 ns |    170.68 ns |  0.35 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **PipeOrgan**          |    **24,378.6 ns** |    **438.78 ns** |    **229.49 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | PipeOrgan          |    23,358.0 ns |    328.48 ns |    171.80 ns |  0.96 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | PipeOrgan          |    14,645.8 ns |    830.01 ns |    368.53 ns |  0.60 |    0.02 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | PipeOrgan          |    24,607.4 ns |    259.66 ns |    135.81 ns |  1.01 |    0.01 |    2 |         - |          NA |
| WeakHeapSort     | 1024 | PipeOrgan          |    27,089.1 ns |    588.56 ns |    261.32 ns |  1.11 |    0.01 |    2 |         - |          NA |
| SmoothSort       | 1024 | PipeOrgan          |    20,739.0 ns |    919.16 ns |    480.74 ns |  0.85 |    0.02 |    2 |         - |          NA |
| PoplarHeapSort   | 1024 | PipeOrgan          |    21,597.6 ns |    278.98 ns |    123.87 ns |  0.89 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 1024 | PipeOrgan          |    14,982.7 ns |  1,068.14 ns |    558.66 ns |  0.61 |    0.02 |    1 |         - |          NA |
| BinomialHeapSort | 1024 | PipeOrgan          |    25,791.4 ns |    146.46 ns |     76.60 ns |  1.06 |    0.01 |    2 |         - |          NA |
| PairingHeapSort  | 1024 | PipeOrgan          |    22,481.9 ns |    246.50 ns |    128.93 ns |  0.92 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **ManyDuplicates**     |    **29,331.4 ns** |    **177.88 ns** |     **78.98 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | ManyDuplicates     |    28,888.3 ns |  1,203.50 ns |    629.46 ns |  0.98 |    0.02 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | ManyDuplicates     |    48,847.0 ns |    569.20 ns |    297.71 ns |  1.67 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | ManyDuplicates     |    30,214.9 ns |    315.95 ns |    165.25 ns |  1.03 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | ManyDuplicates     |    49,765.6 ns |    715.67 ns |    374.31 ns |  1.70 |    0.01 |    2 |         - |          NA |
| SmoothSort       | 1024 | ManyDuplicates     |    63,690.7 ns |    404.94 ns |    179.79 ns |  2.17 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 1024 | ManyDuplicates     |    62,324.4 ns |    347.15 ns |    123.80 ns |  2.12 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 1024 | ManyDuplicates     |    65,059.5 ns |  1,410.54 ns |    737.74 ns |  2.22 |    0.02 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | ManyDuplicates     |   122,522.4 ns |  2,044.80 ns |    907.90 ns |  4.18 |    0.03 |    4 |         - |          NA |
| PairingHeapSort  | 1024 | ManyDuplicates     |    69,487.0 ns |    446.46 ns |    233.51 ns |  2.37 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Random**             |   **130,897.9 ns** |    **527.35 ns** |    **234.15 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Random             |   132,474.9 ns |  1,562.98 ns |    817.47 ns |  1.01 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 4096 | Random             |   251,328.5 ns |  2,409.55 ns |  1,260.24 ns |  1.92 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 4096 | Random             |   143,976.2 ns |  1,094.05 ns |    485.76 ns |  1.10 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | Random             |   311,390.5 ns |  1,705.55 ns |    757.28 ns |  2.38 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 4096 | Random             |   342,247.4 ns |    811.75 ns |    360.42 ns |  2.61 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 4096 | Random             |   309,610.3 ns |    399.56 ns |    177.41 ns |  2.37 |    0.00 |    3 |         - |          NA |
| TournamentSort   | 4096 | Random             |   314,259.7 ns |  3,458.31 ns |  1,535.51 ns |  2.40 |    0.01 |    3 |         - |          NA |
| BinomialHeapSort | 4096 | Random             |   810,491.5 ns |  3,156.06 ns |  1,401.31 ns |  6.19 |    0.01 |    4 |         - |          NA |
| PairingHeapSort  | 4096 | Random             |   392,678.5 ns |  2,233.07 ns |    991.50 ns |  3.00 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **SingleElementMoved** |   **102,617.6 ns** |    **611.86 ns** |    **271.67 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 4096 | SingleElementMoved |   105,675.3 ns |    456.41 ns |    202.65 ns |  1.03 |    0.00 |    4 |         - |          NA |
| TernaryHeapSort  | 4096 | SingleElementMoved |   121,497.4 ns |  6,123.96 ns |  2,719.08 ns |  1.18 |    0.02 |    4 |         - |          NA |
| BottomupHeapSort | 4096 | SingleElementMoved |   111,552.8 ns |  1,306.50 ns |    580.09 ns |  1.09 |    0.01 |    4 |         - |          NA |
| WeakHeapSort     | 4096 | SingleElementMoved |   124,872.0 ns |    989.13 ns |    517.33 ns |  1.22 |    0.01 |    4 |         - |          NA |
| SmoothSort       | 4096 | SingleElementMoved |    18,737.1 ns |    241.26 ns |    126.18 ns |  0.18 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 4096 | SingleElementMoved |    56,377.4 ns |    422.81 ns |    221.14 ns |  0.55 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 4096 | SingleElementMoved |   124,889.5 ns | 12,037.86 ns |  6,296.03 ns |  1.22 |    0.06 |    4 |         - |          NA |
| BinomialHeapSort | 4096 | SingleElementMoved |   116,352.8 ns |  4,818.60 ns |  2,139.49 ns |  1.13 |    0.02 |    4 |         - |          NA |
| PairingHeapSort  | 4096 | SingleElementMoved |    69,231.6 ns |    309.32 ns |    137.34 ns |  0.67 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Sorted**             |   **100,607.9 ns** |  **1,114.45 ns** |    **494.82 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Sorted             |    98,250.8 ns |    692.23 ns |    307.35 ns |  0.98 |    0.01 |    4 |         - |          NA |
| TernaryHeapSort  | 4096 | Sorted             |    77,224.6 ns |  3,852.64 ns |  2,015.00 ns |  0.77 |    0.02 |    3 |         - |          NA |
| BottomupHeapSort | 4096 | Sorted             |   109,996.3 ns |  1,200.92 ns |    628.10 ns |  1.09 |    0.01 |    4 |         - |          NA |
| WeakHeapSort     | 4096 | Sorted             |   122,534.9 ns |  1,367.40 ns |    607.13 ns |  1.22 |    0.01 |    4 |         - |          NA |
| SmoothSort       | 4096 | Sorted             |    15,087.3 ns |    178.15 ns |     93.17 ns |  0.15 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 4096 | Sorted             |    51,521.2 ns |    475.50 ns |    248.69 ns |  0.51 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 4096 | Sorted             |   103,580.1 ns |  6,691.99 ns |  2,971.29 ns |  1.03 |    0.03 |    4 |         - |          NA |
| BinomialHeapSort | 4096 | Sorted             |   102,675.1 ns |  1,808.79 ns |    803.11 ns |  1.02 |    0.01 |    4 |         - |          NA |
| PairingHeapSort  | 4096 | Sorted             |    69,848.6 ns |    683.82 ns |    357.65 ns |  0.69 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Reversed**           |    **98,234.2 ns** |  **1,549.06 ns** |    **810.19 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Reversed           |   102,223.7 ns |  3,566.47 ns |  1,865.34 ns |  1.04 |    0.02 |    3 |         - |          NA |
| TernaryHeapSort  | 4096 | Reversed           |    65,976.6 ns |  1,002.75 ns |    524.46 ns |  0.67 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 4096 | Reversed           |   112,180.8 ns |  1,453.38 ns |    760.14 ns |  1.14 |    0.01 |    3 |         - |          NA |
| WeakHeapSort     | 4096 | Reversed           |   131,734.0 ns |    951.90 ns |    497.86 ns |  1.34 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 4096 | Reversed           |   103,243.4 ns |  1,591.59 ns |    706.67 ns |  1.05 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 4096 | Reversed           |    96,898.7 ns |  1,697.18 ns |    753.56 ns |  0.99 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 4096 | Reversed           |   177,055.4 ns | 32,345.55 ns | 16,917.35 ns |  1.80 |    0.16 |    4 |         - |          NA |
| BinomialHeapSort | 4096 | Reversed           |   100,843.9 ns |    843.60 ns |    374.56 ns |  1.03 |    0.01 |    3 |         - |          NA |
| PairingHeapSort  | 4096 | Reversed           |    31,711.3 ns |    246.38 ns |    109.39 ns |  0.32 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **PipeOrgan**          |   **102,091.9 ns** |  **1,452.46 ns** |    **644.90 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 4096 | PipeOrgan          |   101,422.1 ns |    882.97 ns |    461.81 ns |  0.99 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 4096 | PipeOrgan          |    73,195.3 ns |  3,900.73 ns |  1,731.95 ns |  0.72 |    0.02 |    1 |         - |          NA |
| BottomupHeapSort | 4096 | PipeOrgan          |   111,297.8 ns |  1,373.83 ns |    609.99 ns |  1.09 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | PipeOrgan          |   133,063.6 ns |    796.84 ns |    416.76 ns |  1.30 |    0.01 |    1 |         - |          NA |
| SmoothSort       | 4096 | PipeOrgan          |   208,101.9 ns |  2,373.52 ns |  1,053.86 ns |  2.04 |    0.02 |    2 |         - |          NA |
| PoplarHeapSort   | 4096 | PipeOrgan          |   109,816.5 ns |  7,589.54 ns |  3,969.48 ns |  1.08 |    0.04 |    1 |         - |          NA |
| TournamentSort   | 4096 | PipeOrgan          |    80,928.1 ns |  2,914.83 ns |  1,294.20 ns |  0.79 |    0.01 |    1 |         - |          NA |
| BinomialHeapSort | 4096 | PipeOrgan          |   113,410.9 ns |  2,072.34 ns |    920.13 ns |  1.11 |    0.01 |    1 |         - |          NA |
| PairingHeapSort  | 4096 | PipeOrgan          |    93,909.1 ns |  1,131.42 ns |    502.36 ns |  0.92 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **ManyDuplicates**     |   **120,998.8 ns** |    **520.52 ns** |    **231.11 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 4096 | ManyDuplicates     |   122,081.1 ns |    663.60 ns |    294.64 ns |  1.01 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 4096 | ManyDuplicates     |   228,956.3 ns |    606.35 ns |    269.23 ns |  1.89 |    0.00 |    2 |         - |          NA |
| BottomupHeapSort | 4096 | ManyDuplicates     |   128,226.3 ns |  1,878.20 ns |    982.33 ns |  1.06 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | ManyDuplicates     |   222,855.2 ns |    626.31 ns |    278.08 ns |  1.84 |    0.00 |    2 |         - |          NA |
| SmoothSort       | 4096 | ManyDuplicates     |   297,240.3 ns |  1,388.52 ns |    616.51 ns |  2.46 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 4096 | ManyDuplicates     |   291,703.4 ns |    839.91 ns |    439.29 ns |  2.41 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 4096 | ManyDuplicates     |   311,341.7 ns |  1,618.00 ns |    718.40 ns |  2.57 |    0.01 |    3 |         - |          NA |
| BinomialHeapSort | 4096 | ManyDuplicates     |   547,297.4 ns |  4,610.50 ns |  2,047.09 ns |  4.52 |    0.02 |    4 |         - |          NA |
| PairingHeapSort  | 4096 | ManyDuplicates     |   305,134.2 ns |  3,181.10 ns |  1,663.78 ns |  2.52 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Random**             |   **268,956.4 ns** |    **666.37 ns** |    **295.87 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Random             |   272,593.5 ns |  1,383.76 ns |    614.40 ns |  1.01 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | Random             |   542,813.0 ns |  2,528.12 ns |  1,122.50 ns |  2.02 |    0.00 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | Random             |   309,005.4 ns |  1,386.11 ns |    724.96 ns |  1.15 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | Random             |   677,430.4 ns |  6,292.36 ns |  3,291.03 ns |  2.52 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 8192 | Random             |   742,142.0 ns |  3,130.52 ns |  1,389.97 ns |  2.76 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 8192 | Random             |   664,276.0 ns |  1,520.93 ns |    795.48 ns |  2.47 |    0.00 |    3 |         - |          NA |
| TournamentSort   | 8192 | Random             |   692,548.2 ns |  1,288.57 ns |    572.14 ns |  2.57 |    0.00 |    3 |         - |          NA |
| BinomialHeapSort | 8192 | Random             | 1,781,305.6 ns |  9,607.34 ns |  4,265.72 ns |  6.62 |    0.02 |    4 |         - |          NA |
| PairingHeapSort  | 8192 | Random             |   879,473.1 ns | 30,722.40 ns | 13,640.94 ns |  3.27 |    0.05 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **SingleElementMoved** |   **217,372.1 ns** |  **1,917.98 ns** |  **1,003.14 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 8192 | SingleElementMoved |   216,213.2 ns |    703.77 ns |    312.48 ns |  0.99 |    0.00 |    3 |         - |          NA |
| TernaryHeapSort  | 8192 | SingleElementMoved |   349,271.3 ns |  2,031.40 ns |    901.95 ns |  1.61 |    0.01 |    4 |         - |          NA |
| BottomupHeapSort | 8192 | SingleElementMoved |   244,212.7 ns |    868.22 ns |    385.49 ns |  1.12 |    0.01 |    3 |         - |          NA |
| WeakHeapSort     | 8192 | SingleElementMoved |   272,230.7 ns |  1,484.14 ns |    776.23 ns |  1.25 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 8192 | SingleElementMoved |    37,507.3 ns |    636.02 ns |    282.40 ns |  0.17 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 8192 | SingleElementMoved |   118,152.2 ns |    932.17 ns |    413.89 ns |  0.54 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 8192 | SingleElementMoved |   382,412.7 ns |  1,615.54 ns |    844.96 ns |  1.76 |    0.01 |    4 |         - |          NA |
| BinomialHeapSort | 8192 | SingleElementMoved |   239,303.0 ns |  3,364.32 ns |  1,759.60 ns |  1.10 |    0.01 |    3 |         - |          NA |
| PairingHeapSort  | 8192 | SingleElementMoved |   140,352.0 ns |  1,787.56 ns |    793.69 ns |  0.65 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Sorted**             |   **214,748.2 ns** |  **2,389.18 ns** |  **1,249.59 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Sorted             |   202,180.7 ns |  2,689.88 ns |  1,194.32 ns |  0.94 |    0.01 |    4 |         - |          NA |
| TernaryHeapSort  | 8192 | Sorted             |   335,371.7 ns |  4,943.21 ns |  2,585.39 ns |  1.56 |    0.01 |    5 |         - |          NA |
| BottomupHeapSort | 8192 | Sorted             |   241,063.8 ns |  1,521.62 ns |    795.84 ns |  1.12 |    0.01 |    4 |         - |          NA |
| WeakHeapSort     | 8192 | Sorted             |   267,524.8 ns |  1,807.00 ns |    945.10 ns |  1.25 |    0.01 |    4 |         - |          NA |
| SmoothSort       | 8192 | Sorted             |    29,912.2 ns |    436.79 ns |    228.45 ns |  0.14 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 8192 | Sorted             |   108,905.2 ns |    504.48 ns |    179.90 ns |  0.51 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 8192 | Sorted             |   393,974.3 ns |  2,453.97 ns |  1,283.47 ns |  1.83 |    0.01 |    5 |         - |          NA |
| BinomialHeapSort | 8192 | Sorted             |   213,922.9 ns |  1,142.27 ns |    507.17 ns |  1.00 |    0.01 |    4 |         - |          NA |
| PairingHeapSort  | 8192 | Sorted             |   142,199.1 ns |  1,574.55 ns |    699.11 ns |  0.66 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Reversed**           |   **200,097.5 ns** |  **1,432.97 ns** |    **636.25 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Reversed           |   219,313.0 ns |  1,801.82 ns |    942.39 ns |  1.10 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 8192 | Reversed           |   329,187.1 ns |  5,418.49 ns |  2,833.97 ns |  1.65 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | Reversed           |   245,047.5 ns |  1,135.88 ns |    504.34 ns |  1.22 |    0.00 |    2 |         - |          NA |
| WeakHeapSort     | 8192 | Reversed           |   285,923.2 ns |    672.26 ns |    351.60 ns |  1.43 |    0.00 |    2 |         - |          NA |
| SmoothSort       | 8192 | Reversed           |   439,601.7 ns |  5,234.01 ns |  2,737.49 ns |  2.20 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 8192 | Reversed           |   320,974.2 ns |  5,703.67 ns |  2,983.13 ns |  1.60 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 8192 | Reversed           |   500,746.7 ns | 38,463.00 ns | 17,077.81 ns |  2.50 |    0.08 |    3 |         - |          NA |
| BinomialHeapSort | 8192 | Reversed           |   212,840.9 ns |  1,233.30 ns |    547.59 ns |  1.06 |    0.00 |    2 |         - |          NA |
| PairingHeapSort  | 8192 | Reversed           |    65,767.3 ns |  4,228.81 ns |  2,211.75 ns |  0.33 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **PipeOrgan**          |   **212,592.2 ns** |  **2,082.84 ns** |  **1,089.36 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | PipeOrgan          |   206,152.0 ns |  2,132.41 ns |  1,115.29 ns |  0.97 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | PipeOrgan          |   375,532.3 ns |  2,428.40 ns |  1,270.10 ns |  1.77 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | PipeOrgan          |   244,060.3 ns |  4,397.97 ns |  1,952.72 ns |  1.15 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | PipeOrgan          |   290,945.2 ns |  1,980.79 ns |  1,035.99 ns |  1.37 |    0.01 |    1 |         - |          NA |
| SmoothSort       | 8192 | PipeOrgan          |   551,601.8 ns |  2,071.46 ns |    919.74 ns |  2.59 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 8192 | PipeOrgan          |   454,040.3 ns |  1,899.67 ns |    843.47 ns |  2.14 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 8192 | PipeOrgan          |   415,350.6 ns | 27,319.24 ns | 14,288.49 ns |  1.95 |    0.06 |    2 |         - |          NA |
| BinomialHeapSort | 8192 | PipeOrgan          |   238,080.5 ns |  4,727.19 ns |  2,472.41 ns |  1.12 |    0.01 |    1 |         - |          NA |
| PairingHeapSort  | 8192 | PipeOrgan          |   186,555.5 ns |    910.10 ns |    404.09 ns |  0.88 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **ManyDuplicates**     |   **244,443.9 ns** |  **1,642.93 ns** |    **859.28 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | ManyDuplicates     |   248,420.7 ns |    841.55 ns |    373.65 ns |  1.02 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | ManyDuplicates     |   497,223.6 ns |  3,852.78 ns |  2,015.08 ns |  2.03 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | ManyDuplicates     |   274,897.2 ns |    652.52 ns |    289.72 ns |  1.12 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | ManyDuplicates     |   465,905.2 ns |  3,980.96 ns |  1,767.57 ns |  1.91 |    0.01 |    2 |         - |          NA |
| SmoothSort       | 8192 | ManyDuplicates     |   626,141.3 ns |  1,652.93 ns |    864.51 ns |  2.56 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 8192 | ManyDuplicates     |   616,357.0 ns |    354.85 ns |    126.54 ns |  2.52 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 8192 | ManyDuplicates     |   660,290.1 ns |  1,928.04 ns |    687.56 ns |  2.70 |    0.01 |    3 |         - |          NA |
| BinomialHeapSort | 8192 | ManyDuplicates     | 1,134,612.8 ns |  5,311.04 ns |  2,777.78 ns |  4.64 |    0.02 |    4 |         - |          NA |
| PairingHeapSort  | 8192 | ManyDuplicates     |   641,322.0 ns |  2,168.97 ns |    773.48 ns |  2.62 |    0.01 |    3 |         - |          NA |

### InsertionBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.52 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                 | Size | Pattern            | Mean         | Error       | StdDev      | Ratio  | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------- |----- |------------------- |-------------:|------------:|------------:|-------:|--------:|-----:|----------:|------------:|
| **InsertionSort**          | **256**  | **Random**             |   **7,245.0 ns** |    **37.14 ns** |    **16.49 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Random             |   7,610.9 ns |   409.80 ns |   214.33 ns |   1.05 |    0.03 |    1 |         - |          NA |
| BinaryInsertSort       | 256  | Random             |  11,611.6 ns |   203.30 ns |   106.33 ns |   1.60 |    0.01 |    2 |         - |          NA |
| GnomeSort              | 256  | Random             |  10,913.0 ns |   361.71 ns |   189.18 ns |   1.51 |    0.02 |    2 |         - |          NA |
| LibrarySort            | 256  | Random             |  22,814.4 ns |   320.59 ns |   142.35 ns |   3.15 |    0.02 |    3 |         - |          NA |
| MergeInsertionSort     | 256  | Random             |  20,844.1 ns |   271.94 ns |   120.74 ns |   2.88 |    0.02 |    3 |         - |          NA |
| ShellSortKnuth1973     | 256  | Random             |   8,060.6 ns |   225.37 ns |   100.07 ns |   1.11 |    0.01 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Random             |   8,244.2 ns |   323.24 ns |   169.06 ns |   1.14 |    0.02 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | Random             |   8,695.8 ns |   296.98 ns |   155.33 ns |   1.20 |    0.02 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | Random             |   8,630.5 ns |   270.72 ns |   141.59 ns |   1.19 |    0.02 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Random             |   8,815.1 ns |   435.11 ns |   227.57 ns |   1.22 |    0.03 |    1 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **256**  | **SingleElementMoved** |     **206.7 ns** |    **33.08 ns** |    **14.69 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 256  | SingleElementMoved |     297.8 ns |    92.31 ns |    48.28 ns |   1.45 |    0.24 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | SingleElementMoved |     979.6 ns |    55.10 ns |    24.46 ns |   4.76 |    0.31 |    3 |         - |          NA |
| GnomeSort              | 256  | SingleElementMoved |     307.2 ns |     4.22 ns |     1.87 ns |   1.49 |    0.09 |    2 |         - |          NA |
| LibrarySort            | 256  | SingleElementMoved |  21,692.5 ns |   353.52 ns |   184.90 ns | 105.34 |    6.47 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | SingleElementMoved |  10,109.2 ns |   383.01 ns |   200.32 ns |  49.09 |    3.13 |    4 |         - |          NA |
| ShellSortKnuth1973     | 256  | SingleElementMoved |     830.3 ns |   155.23 ns |    68.92 ns |   4.03 |    0.40 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 256  | SingleElementMoved |     802.6 ns |    19.36 ns |     6.90 ns |   3.90 |    0.24 |    3 |         - |          NA |
| ShellSortTokuda1992    | 256  | SingleElementMoved |     974.6 ns |    28.42 ns |    12.62 ns |   4.73 |    0.29 |    3 |         - |          NA |
| ShellSortCiura2001     | 256  | SingleElementMoved |     911.7 ns |    62.54 ns |    27.77 ns |   4.43 |    0.30 |    3 |         - |          NA |
| ShellSortLee2021       | 256  | SingleElementMoved |     984.1 ns |    36.00 ns |    15.98 ns |   4.78 |    0.30 |    3 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **256**  | **Sorted**             |     **154.5 ns** |    **20.64 ns** |     **9.16 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Sorted             |     212.9 ns |    20.46 ns |     7.29 ns |   1.38 |    0.08 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | Sorted             |     998.5 ns |   287.39 ns |   150.31 ns |   6.48 |    0.98 |    3 |         - |          NA |
| GnomeSort              | 256  | Sorted             |     237.1 ns |    29.46 ns |    15.41 ns |   1.54 |    0.12 |    2 |         - |          NA |
| LibrarySort            | 256  | Sorted             |  21,404.4 ns |   236.55 ns |   123.72 ns | 138.97 |    7.08 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | Sorted             |   9,908.3 ns |   269.73 ns |   141.07 ns |  64.33 |    3.37 |    4 |         - |          NA |
| ShellSortKnuth1973     | 256  | Sorted             |     643.0 ns |    31.18 ns |    13.84 ns |   4.17 |    0.23 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Sorted             |     642.1 ns |     8.66 ns |     3.85 ns |   4.17 |    0.21 |    3 |         - |          NA |
| ShellSortTokuda1992    | 256  | Sorted             |     783.1 ns |     6.91 ns |     3.07 ns |   5.08 |    0.26 |    3 |         - |          NA |
| ShellSortCiura2001     | 256  | Sorted             |     705.6 ns |     5.70 ns |     2.53 ns |   4.58 |    0.23 |    3 |         - |          NA |
| ShellSortLee2021       | 256  | Sorted             |     813.7 ns |    19.39 ns |    10.14 ns |   5.28 |    0.27 |    3 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **256**  | **Reversed**           |  **12,010.1 ns** |   **294.32 ns** |   **153.93 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Reversed           |   9,493.1 ns |   248.08 ns |   129.75 ns |   0.79 |    0.01 |    3 |         - |          NA |
| BinaryInsertSort       | 256  | Reversed           |   4,934.1 ns |   280.85 ns |   146.89 ns |   0.41 |    0.01 |    2 |         - |          NA |
| GnomeSort              | 256  | Reversed           |  21,455.6 ns |   135.90 ns |    48.46 ns |   1.79 |    0.02 |    6 |         - |          NA |
| LibrarySort            | 256  | Reversed           |  17,783.4 ns |   371.60 ns |   194.36 ns |   1.48 |    0.02 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | Reversed           |   9,807.8 ns |   226.15 ns |   118.28 ns |   0.82 |    0.01 |    3 |         - |          NA |
| ShellSortKnuth1973     | 256  | Reversed           |   1,164.6 ns |    26.79 ns |    14.01 ns |   0.10 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Reversed           |   1,112.7 ns |    92.01 ns |    40.85 ns |   0.09 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | Reversed           |   1,113.5 ns |     5.78 ns |     2.06 ns |   0.09 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | Reversed           |   1,136.0 ns |    12.52 ns |     5.56 ns |   0.09 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Reversed           |   1,177.6 ns |    48.68 ns |    21.61 ns |   0.10 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **256**  | **PipeOrgan**          |   **6,184.7 ns** |   **235.29 ns** |   **123.06 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 256  | PipeOrgan          |   4,989.8 ns |   315.78 ns |   165.16 ns |   0.81 |    0.03 |    3 |         - |          NA |
| BinaryInsertSort       | 256  | PipeOrgan          |   2,916.3 ns |   117.14 ns |    52.01 ns |   0.47 |    0.01 |    2 |         - |          NA |
| GnomeSort              | 256  | PipeOrgan          |   9,779.2 ns |   215.16 ns |    95.53 ns |   1.58 |    0.03 |    5 |         - |          NA |
| LibrarySort            | 256  | PipeOrgan          |  15,121.3 ns |   218.74 ns |   114.41 ns |   2.45 |    0.05 |    6 |         - |          NA |
| MergeInsertionSort     | 256  | PipeOrgan          |  10,091.1 ns |   136.77 ns |    60.73 ns |   1.63 |    0.03 |    5 |         - |          NA |
| ShellSortKnuth1973     | 256  | PipeOrgan          |   1,014.6 ns |    24.93 ns |    11.07 ns |   0.16 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | PipeOrgan          |   1,078.5 ns |   109.79 ns |    48.75 ns |   0.17 |    0.01 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | PipeOrgan          |   1,123.9 ns |    49.02 ns |    17.48 ns |   0.18 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | PipeOrgan          |   1,072.6 ns |    27.29 ns |    12.12 ns |   0.17 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | PipeOrgan          |   1,167.6 ns |   109.34 ns |    48.55 ns |   0.19 |    0.01 |    1 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **256**  | **ManyDuplicates**     |   **7,119.8 ns** |   **102.53 ns** |    **53.62 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 256  | ManyDuplicates     |   7,146.9 ns |    68.83 ns |    30.56 ns |   1.00 |    0.01 |    1 |         - |          NA |
| BinaryInsertSort       | 256  | ManyDuplicates     |  11,161.2 ns |   306.95 ns |   160.54 ns |   1.57 |    0.02 |    2 |         - |          NA |
| GnomeSort              | 256  | ManyDuplicates     |  10,609.6 ns |   232.64 ns |   103.29 ns |   1.49 |    0.02 |    2 |         - |          NA |
| LibrarySort            | 256  | ManyDuplicates     |  23,842.0 ns |   353.17 ns |   184.71 ns |   3.35 |    0.03 |    3 |         - |          NA |
| MergeInsertionSort     | 256  | ManyDuplicates     |  20,377.2 ns |   221.22 ns |    98.22 ns |   2.86 |    0.02 |    3 |         - |          NA |
| ShellSortKnuth1973     | 256  | ManyDuplicates     |   7,157.9 ns |    94.53 ns |    33.71 ns |   1.01 |    0.01 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | ManyDuplicates     |   7,406.7 ns |   102.13 ns |    45.35 ns |   1.04 |    0.01 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | ManyDuplicates     |   7,537.4 ns |   472.06 ns |   246.90 ns |   1.06 |    0.03 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | ManyDuplicates     |   7,439.4 ns |   292.59 ns |   153.03 ns |   1.04 |    0.02 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | ManyDuplicates     |   7,209.2 ns |    71.84 ns |    25.62 ns |   1.01 |    0.01 |    1 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **1024** | **Random**             |  **97,122.8 ns** | **5,404.64 ns** | **2,826.73 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Random             |  90,786.6 ns | 2,107.99 ns | 1,102.52 ns |   0.94 |    0.03 |    3 |         - |          NA |
| BinaryInsertSort       | 1024 | Random             |  63,134.8 ns |   332.20 ns |   147.50 ns |   0.65 |    0.02 |    2 |         - |          NA |
| GnomeSort              | 1024 | Random             | 150,945.5 ns |   419.97 ns |   186.47 ns |   1.56 |    0.04 |    4 |         - |          NA |
| LibrarySort            | 1024 | Random             | 106,877.4 ns | 1,732.41 ns |   769.20 ns |   1.10 |    0.03 |    3 |         - |          NA |
| MergeInsertionSort     | 1024 | Random             | 107,502.9 ns |   920.85 ns |   481.62 ns |   1.11 |    0.03 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Random             |  43,778.7 ns |   136.10 ns |    60.43 ns |   0.45 |    0.01 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Random             |  46,192.9 ns |   271.17 ns |   141.83 ns |   0.48 |    0.01 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Random             |  47,973.8 ns |   386.24 ns |   202.01 ns |   0.49 |    0.01 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Random             |  47,462.4 ns |   200.79 ns |   105.02 ns |   0.49 |    0.01 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Random             |  47,991.2 ns |   242.93 ns |   127.06 ns |   0.49 |    0.01 |    1 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **1024** | **SingleElementMoved** |     **796.0 ns** |   **152.34 ns** |    **79.67 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | SingleElementMoved |     889.2 ns |    67.89 ns |    35.51 ns |   1.13 |    0.11 |    1 |         - |          NA |
| BinaryInsertSort       | 1024 | SingleElementMoved |   4,405.3 ns |    46.22 ns |    20.52 ns |   5.58 |    0.48 |    3 |         - |          NA |
| GnomeSort              | 1024 | SingleElementMoved |   1,141.5 ns |    23.56 ns |    12.32 ns |   1.45 |    0.13 |    2 |         - |          NA |
| LibrarySort            | 1024 | SingleElementMoved | 192,742.6 ns |   282.82 ns |   147.92 ns | 244.10 |   21.16 |    5 |         - |          NA |
| MergeInsertionSort     | 1024 | SingleElementMoved |  48,811.5 ns |   423.40 ns |   221.45 ns |  61.82 |    5.37 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | SingleElementMoved |   3,503.7 ns |   167.68 ns |    74.45 ns |   4.44 |    0.40 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | SingleElementMoved |   4,048.9 ns |   274.50 ns |   143.57 ns |   5.13 |    0.48 |    3 |         - |          NA |
| ShellSortTokuda1992    | 1024 | SingleElementMoved |   4,419.8 ns |   407.98 ns |   213.38 ns |   5.60 |    0.55 |    3 |         - |          NA |
| ShellSortCiura2001     | 1024 | SingleElementMoved |   4,113.1 ns |    35.56 ns |    18.60 ns |   5.21 |    0.45 |    3 |         - |          NA |
| ShellSortLee2021       | 1024 | SingleElementMoved |   4,126.6 ns |    23.29 ns |    10.34 ns |   5.23 |    0.45 |    3 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **1024** | **Sorted**             |     **601.4 ns** |   **123.28 ns** |    **54.74 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Sorted             |     748.2 ns |     8.98 ns |     3.20 ns |   1.25 |    0.10 |    2 |         - |          NA |
| BinaryInsertSort       | 1024 | Sorted             |   4,764.9 ns |   827.66 ns |   432.88 ns |   7.97 |    0.92 |    4 |         - |          NA |
| GnomeSort              | 1024 | Sorted             |     861.7 ns |    64.68 ns |    28.72 ns |   1.44 |    0.12 |    2 |         - |          NA |
| LibrarySort            | 1024 | Sorted             | 192,590.6 ns |   970.08 ns |   507.37 ns | 322.34 |   24.89 |    6 |         - |          NA |
| MergeInsertionSort     | 1024 | Sorted             |  49,587.7 ns |   571.03 ns |   298.66 ns |  82.99 |    6.42 |    5 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Sorted             |   3,111.0 ns |    19.89 ns |    10.40 ns |   5.21 |    0.40 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Sorted             |   3,492.7 ns |    14.83 ns |     7.76 ns |   5.85 |    0.45 |    3 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Sorted             |   3,848.4 ns |   281.21 ns |   147.08 ns |   6.44 |    0.55 |    3 |         - |          NA |
| ShellSortCiura2001     | 1024 | Sorted             |   3,729.0 ns |    93.17 ns |    41.37 ns |   6.24 |    0.49 |    3 |         - |          NA |
| ShellSortLee2021       | 1024 | Sorted             |   3,736.8 ns |    76.74 ns |    27.37 ns |   6.25 |    0.49 |    3 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **1024** | **Reversed**           | **177,916.9 ns** |   **468.52 ns** |   **208.03 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Reversed           | 146,662.8 ns | 1,685.87 ns |   748.53 ns |   0.82 |    0.00 |    4 |         - |          NA |
| BinaryInsertSort       | 1024 | Reversed           |  34,878.7 ns |   447.86 ns |   198.85 ns |   0.20 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | Reversed           | 302,722.1 ns | 6,070.14 ns | 3,174.80 ns |   1.70 |    0.02 |    5 |         - |          NA |
| LibrarySort            | 1024 | Reversed           | 152,732.8 ns | 4,638.20 ns | 2,059.39 ns |   0.86 |    0.01 |    4 |         - |          NA |
| MergeInsertionSort     | 1024 | Reversed           |  48,428.0 ns |   340.01 ns |   177.83 ns |   0.27 |    0.00 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Reversed           |   5,574.1 ns |   198.96 ns |   104.06 ns |   0.03 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Reversed           |   5,557.5 ns |    24.52 ns |     8.74 ns |   0.03 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Reversed           |   6,123.5 ns |   281.91 ns |   147.44 ns |   0.03 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Reversed           |   5,637.4 ns |    24.06 ns |    10.68 ns |   0.03 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Reversed           |   5,869.7 ns |    68.10 ns |    24.29 ns |   0.03 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **1024** | **PipeOrgan**          |  **90,297.5 ns** |   **812.70 ns** |   **360.84 ns** |   **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | PipeOrgan          |  73,876.9 ns |   293.66 ns |   130.39 ns |   0.82 |    0.00 |    4 |         - |          NA |
| BinaryInsertSort       | 1024 | PipeOrgan          |  20,580.0 ns |   490.32 ns |   256.45 ns |   0.23 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | PipeOrgan          | 147,536.0 ns |   549.65 ns |   244.05 ns |   1.63 |    0.01 |    6 |         - |          NA |
| LibrarySort            | 1024 | PipeOrgan          |  91,493.5 ns |   546.59 ns |   285.88 ns |   1.01 |    0.00 |    5 |         - |          NA |
| MergeInsertionSort     | 1024 | PipeOrgan          |  60,142.3 ns | 6,566.02 ns | 3,434.16 ns |   0.67 |    0.04 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | PipeOrgan          |   5,010.0 ns |    27.98 ns |    12.42 ns |   0.06 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | PipeOrgan          |   5,606.2 ns |   209.12 ns |   109.37 ns |   0.06 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | PipeOrgan          |   5,823.8 ns |    75.16 ns |    33.37 ns |   0.06 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | PipeOrgan          |   5,776.3 ns |   117.25 ns |    61.33 ns |   0.06 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | PipeOrgan          |   5,967.6 ns |   446.88 ns |   233.73 ns |   0.07 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **1024** | **ManyDuplicates**     |  **93,282.6 ns** | **5,297.78 ns** | **2,352.25 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | ManyDuplicates     |  88,818.4 ns | 1,215.83 ns |   539.84 ns |   0.95 |    0.02 |    3 |         - |          NA |
| BinaryInsertSort       | 1024 | ManyDuplicates     |  59,862.4 ns |   282.48 ns |   147.74 ns |   0.64 |    0.01 |    2 |         - |          NA |
| GnomeSort              | 1024 | ManyDuplicates     | 147,828.3 ns | 1,675.05 ns |   743.73 ns |   1.59 |    0.04 |    4 |         - |          NA |
| LibrarySort            | 1024 | ManyDuplicates     | 112,404.0 ns |   490.88 ns |   217.95 ns |   1.21 |    0.03 |    3 |         - |          NA |
| MergeInsertionSort     | 1024 | ManyDuplicates     | 104,865.7 ns |   522.54 ns |   186.34 ns |   1.12 |    0.03 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | ManyDuplicates     |  33,759.5 ns |   236.13 ns |   123.50 ns |   0.36 |    0.01 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | ManyDuplicates     |  34,861.8 ns |   345.95 ns |   180.94 ns |   0.37 |    0.01 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | ManyDuplicates     |  34,980.2 ns |   252.38 ns |   132.00 ns |   0.38 |    0.01 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | ManyDuplicates     |  34,476.4 ns |   273.55 ns |   143.07 ns |   0.37 |    0.01 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | ManyDuplicates     |  33,403.9 ns |   257.72 ns |   134.79 ns |   0.36 |    0.01 |    1 |         - |          NA |

### MergeBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.52 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                   | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **MergeSort**                | **256**  | **Random**             |    **12,610.8 ns** |    **268.22 ns** |    **140.28 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Random             |    12,008.9 ns |    366.64 ns |    162.79 ns |  0.95 |    0.02 |    3 |         - |          NA |
| BottomupMergeSort        | 256  | Random             |     9,068.1 ns |    315.88 ns |    165.21 ns |  0.72 |    0.01 |    3 |         - |          NA |
| StdStableSort            | 256  | Random             |     5,550.7 ns |    566.48 ns |    296.28 ns |  0.44 |    0.02 |    2 |         - |          NA |
| RotateMergeSort          | 256  | Random             |    18,685.0 ns |    718.76 ns |    375.92 ns |  1.48 |    0.03 |    4 |         - |          NA |
| RotateMergeSortRecursive | 256  | Random             |    22,728.6 ns |    522.58 ns |    273.32 ns |  1.80 |    0.03 |    5 |         - |          NA |
| SymMergeSort             | 256  | Random             |    12,351.0 ns |    269.18 ns |    119.52 ns |  0.98 |    0.01 |    3 |         - |          NA |
| BlockMergeSort           | 256  | Random             |     9,697.2 ns |    278.51 ns |    123.66 ns |  0.77 |    0.01 |    3 |         - |          NA |
| NaturalMergeSort         | 256  | Random             |     9,254.0 ns |    302.10 ns |    158.00 ns |  0.73 |    0.01 |    3 |         - |          NA |
| PatienceSort             | 256  | Random             |    13,106.3 ns |    313.51 ns |    163.97 ns |  1.04 |    0.02 |    3 |         - |          NA |
| DropMergeSort            | 256  | Random             |     9,447.5 ns |    391.26 ns |    204.64 ns |  0.75 |    0.02 |    3 |         - |          NA |
| TimSort                  | 256  | Random             |    10,208.9 ns |    361.60 ns |    189.12 ns |  0.81 |    0.02 |    3 |         - |          NA |
| PowerSort                | 256  | Random             |     6,680.0 ns |     82.14 ns |     29.29 ns |  0.53 |    0.01 |    2 |         - |          NA |
| ShiftSort                | 256  | Random             |    10,116.5 ns |    387.23 ns |    202.53 ns |  0.80 |    0.02 |    3 |         - |          NA |
| SpinSort                 | 256  | Random             |     5,907.0 ns |    390.86 ns |    204.43 ns |  0.47 |    0.02 |    2 |         - |          NA |
| Glidesort                | 256  | Random             |     5,838.9 ns |    477.89 ns |    249.95 ns |  0.46 |    0.02 |    2 |         - |          NA |
| Driftsort                | 256  | Random             |     4,929.5 ns |    333.84 ns |    174.60 ns |  0.39 |    0.01 |    2 |         - |          NA |
| FlatStableSort           | 256  | Random             |     6,776.2 ns |     69.63 ns |     24.83 ns |  0.54 |    0.01 |    2 |         - |          NA |
| QuadSort                 | 256  | Random             |     3,540.3 ns |    425.51 ns |    188.93 ns |  0.28 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **SingleElementMoved** |     **3,607.2 ns** |    **351.76 ns** |    **183.98 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | SingleElementMoved |     4,255.4 ns |    191.28 ns |     84.93 ns |  1.18 |    0.06 |    6 |         - |          NA |
| BottomupMergeSort        | 256  | SingleElementMoved |     1,340.9 ns |     68.14 ns |     30.25 ns |  0.37 |    0.02 |    3 |         - |          NA |
| StdStableSort            | 256  | SingleElementMoved |       545.2 ns |     19.16 ns |      8.51 ns |  0.15 |    0.01 |    2 |         - |          NA |
| RotateMergeSort          | 256  | SingleElementMoved |       521.7 ns |     42.16 ns |     18.72 ns |  0.14 |    0.01 |    2 |         - |          NA |
| RotateMergeSortRecursive | 256  | SingleElementMoved |       714.1 ns |     61.32 ns |     27.23 ns |  0.20 |    0.01 |    2 |         - |          NA |
| SymMergeSort             | 256  | SingleElementMoved |       402.9 ns |     49.27 ns |     21.88 ns |  0.11 |    0.01 |    1 |         - |          NA |
| BlockMergeSort           | 256  | SingleElementMoved |     2,300.4 ns |    216.91 ns |     96.31 ns |  0.64 |    0.04 |    4 |         - |          NA |
| NaturalMergeSort         | 256  | SingleElementMoved |       359.9 ns |     10.80 ns |      4.80 ns |  0.10 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 256  | SingleElementMoved |     7,101.0 ns |    469.90 ns |    208.64 ns |  1.97 |    0.11 |    7 |         - |          NA |
| DropMergeSort            | 256  | SingleElementMoved |       527.4 ns |     34.62 ns |     18.11 ns |  0.15 |    0.01 |    2 |         - |          NA |
| TimSort                  | 256  | SingleElementMoved |       315.2 ns |     32.34 ns |     14.36 ns |  0.09 |    0.01 |    1 |         - |          NA |
| PowerSort                | 256  | SingleElementMoved |       366.1 ns |     33.17 ns |     17.35 ns |  0.10 |    0.01 |    1 |         - |          NA |
| ShiftSort                | 256  | SingleElementMoved |       399.8 ns |     10.68 ns |      4.74 ns |  0.11 |    0.01 |    1 |         - |          NA |
| SpinSort                 | 256  | SingleElementMoved |       676.4 ns |     38.04 ns |     16.89 ns |  0.19 |    0.01 |    2 |         - |          NA |
| Glidesort                | 256  | SingleElementMoved |       649.4 ns |      5.87 ns |      2.09 ns |  0.18 |    0.01 |    2 |         - |          NA |
| Driftsort                | 256  | SingleElementMoved |       554.9 ns |     28.10 ns |     12.48 ns |  0.15 |    0.01 |    2 |         - |          NA |
| FlatStableSort           | 256  | SingleElementMoved |     1,141.2 ns |     40.40 ns |     17.94 ns |  0.32 |    0.02 |    3 |         - |          NA |
| QuadSort                 | 256  | SingleElementMoved |       517.9 ns |     37.63 ns |     16.71 ns |  0.14 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **Sorted**             |     **3,319.4 ns** |    **111.07 ns** |     **49.32 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Sorted             |     4,013.7 ns |     49.03 ns |     17.49 ns |  1.21 |    0.02 |    7 |         - |          NA |
| BottomupMergeSort        | 256  | Sorted             |     1,172.0 ns |     16.70 ns |      7.41 ns |  0.35 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 256  | Sorted             |       476.2 ns |      4.84 ns |      2.15 ns |  0.14 |    0.00 |    3 |         - |          NA |
| RotateMergeSort          | 256  | Sorted             |       238.4 ns |     10.90 ns |      4.84 ns |  0.07 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 256  | Sorted             |       411.5 ns |     27.80 ns |     14.54 ns |  0.12 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 256  | Sorted             |       252.1 ns |     13.00 ns |      5.77 ns |  0.08 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 256  | Sorted             |     1,968.5 ns |      3.27 ns |      1.45 ns |  0.59 |    0.01 |    5 |         - |          NA |
| NaturalMergeSort         | 256  | Sorted             |       172.4 ns |      7.90 ns |      4.13 ns |  0.05 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 256  | Sorted             |     6,321.6 ns |    143.78 ns |     63.84 ns |  1.90 |    0.03 |    8 |         - |          NA |
| DropMergeSort            | 256  | Sorted             |       478.0 ns |     16.38 ns |      7.27 ns |  0.14 |    0.00 |    3 |         - |          NA |
| TimSort                  | 256  | Sorted             |       140.5 ns |      3.74 ns |      1.66 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | Sorted             |       136.1 ns |      6.17 ns |      2.74 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Sorted             |       182.5 ns |      5.92 ns |      2.63 ns |  0.05 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 256  | Sorted             |       120.3 ns |     13.56 ns |      7.09 ns |  0.04 |    0.00 |    1 |         - |          NA |
| Glidesort                | 256  | Sorted             |       159.4 ns |     11.46 ns |      4.09 ns |  0.05 |    0.00 |    1 |         - |          NA |
| Driftsort                | 256  | Sorted             |       174.6 ns |     10.18 ns |      4.52 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | Sorted             |     1,004.6 ns |     16.54 ns |      7.35 ns |  0.30 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 256  | Sorted             |       268.5 ns |      6.91 ns |      3.61 ns |  0.08 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **Reversed**           |     **6,957.0 ns** |      **8.53 ns** |      **3.79 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Reversed           |     6,460.0 ns |     70.35 ns |     31.23 ns |  0.93 |    0.00 |    6 |         - |          NA |
| BottomupMergeSort        | 256  | Reversed           |     3,723.3 ns |     20.78 ns |      9.22 ns |  0.54 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 256  | Reversed           |     3,185.6 ns |      8.88 ns |      3.17 ns |  0.46 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 256  | Reversed           |     4,312.9 ns |     27.24 ns |     12.10 ns |  0.62 |    0.00 |    5 |         - |          NA |
| RotateMergeSortRecursive | 256  | Reversed           |     6,206.7 ns |    548.30 ns |    286.77 ns |  0.89 |    0.04 |    6 |         - |          NA |
| SymMergeSort             | 256  | Reversed           |     1,743.9 ns |     47.16 ns |     16.82 ns |  0.25 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 256  | Reversed           |     2,435.7 ns |     14.13 ns |      7.39 ns |  0.35 |    0.00 |    4 |         - |          NA |
| NaturalMergeSort         | 256  | Reversed           |       231.1 ns |      9.97 ns |      5.22 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 256  | Reversed           |     1,263.4 ns |     10.40 ns |      5.44 ns |  0.18 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 256  | Reversed           |     1,237.7 ns |     36.22 ns |     16.08 ns |  0.18 |    0.00 |    2 |         - |          NA |
| TimSort                  | 256  | Reversed           |       219.0 ns |      7.89 ns |      4.13 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | Reversed           |       183.6 ns |      4.05 ns |      1.80 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Reversed           |     3,263.8 ns |    240.56 ns |    106.81 ns |  0.47 |    0.01 |    5 |         - |          NA |
| SpinSort                 | 256  | Reversed           |       222.0 ns |     13.73 ns |      6.10 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 256  | Reversed           |       253.4 ns |      7.52 ns |      3.93 ns |  0.04 |    0.00 |    1 |         - |          NA |
| Driftsort                | 256  | Reversed           |       250.9 ns |     13.48 ns |      7.05 ns |  0.04 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | Reversed           |     1,733.5 ns |     14.99 ns |      5.35 ns |  0.25 |    0.00 |    3 |         - |          NA |
| QuadSort                 | 256  | Reversed           |       216.8 ns |      9.37 ns |      4.16 ns |  0.03 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **PipeOrgan**          |     **5,344.5 ns** |    **307.90 ns** |    **161.04 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | PipeOrgan          |     5,302.8 ns |    270.79 ns |    141.63 ns |  0.99 |    0.04 |    6 |         - |          NA |
| BottomupMergeSort        | 256  | PipeOrgan          |     2,478.2 ns |     31.49 ns |     11.23 ns |  0.46 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 256  | PipeOrgan          |     1,911.6 ns |     14.12 ns |      7.39 ns |  0.36 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 256  | PipeOrgan          |     4,368.8 ns |    268.63 ns |    140.50 ns |  0.82 |    0.03 |    6 |         - |          NA |
| RotateMergeSortRecursive | 256  | PipeOrgan          |     5,732.4 ns |    276.34 ns |    144.53 ns |  1.07 |    0.04 |    6 |         - |          NA |
| SymMergeSort             | 256  | PipeOrgan          |     2,597.5 ns |     65.03 ns |     23.19 ns |  0.49 |    0.01 |    5 |         - |          NA |
| BlockMergeSort           | 256  | PipeOrgan          |     2,410.4 ns |     36.50 ns |     13.02 ns |  0.45 |    0.01 |    5 |         - |          NA |
| NaturalMergeSort         | 256  | PipeOrgan          |       403.2 ns |      7.79 ns |      4.07 ns |  0.08 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 256  | PipeOrgan          |     4,552.8 ns |    243.34 ns |    127.27 ns |  0.85 |    0.03 |    6 |         - |          NA |
| DropMergeSort            | 256  | PipeOrgan          |     5,005.0 ns |    271.11 ns |    120.38 ns |  0.94 |    0.03 |    6 |         - |          NA |
| TimSort                  | 256  | PipeOrgan          |       592.3 ns |     10.99 ns |      4.88 ns |  0.11 |    0.00 |    2 |         - |          NA |
| PowerSort                | 256  | PipeOrgan          |       392.0 ns |     11.37 ns |      5.94 ns |  0.07 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | PipeOrgan          |     2,073.5 ns |     17.44 ns |      9.12 ns |  0.39 |    0.01 |    5 |         - |          NA |
| SpinSort                 | 256  | PipeOrgan          |     1,570.1 ns |      9.96 ns |      3.55 ns |  0.29 |    0.01 |    4 |         - |          NA |
| Glidesort                | 256  | PipeOrgan          |       980.2 ns |     13.63 ns |      4.86 ns |  0.18 |    0.01 |    3 |         - |          NA |
| Driftsort                | 256  | PipeOrgan          |       355.8 ns |      9.51 ns |      4.97 ns |  0.07 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | PipeOrgan          |     1,476.8 ns |     31.21 ns |     16.32 ns |  0.28 |    0.01 |    4 |         - |          NA |
| QuadSort                 | 256  | PipeOrgan          |       672.4 ns |     21.33 ns |      9.47 ns |  0.13 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **ManyDuplicates**     |    **12,253.7 ns** |    **246.38 ns** |    **109.39 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | ManyDuplicates     |    11,760.3 ns |    233.64 ns |    122.20 ns |  0.96 |    0.01 |    3 |         - |          NA |
| BottomupMergeSort        | 256  | ManyDuplicates     |     8,837.4 ns |    209.86 ns |    109.76 ns |  0.72 |    0.01 |    3 |         - |          NA |
| StdStableSort            | 256  | ManyDuplicates     |     4,963.7 ns |    267.61 ns |    139.97 ns |  0.41 |    0.01 |    2 |         - |          NA |
| RotateMergeSort          | 256  | ManyDuplicates     |    17,999.0 ns |    365.58 ns |    162.32 ns |  1.47 |    0.02 |    4 |         - |          NA |
| RotateMergeSortRecursive | 256  | ManyDuplicates     |    22,079.3 ns |    711.46 ns |    372.11 ns |  1.80 |    0.03 |    5 |         - |          NA |
| SymMergeSort             | 256  | ManyDuplicates     |    12,132.8 ns |    587.66 ns |    307.36 ns |  0.99 |    0.03 |    3 |         - |          NA |
| BlockMergeSort           | 256  | ManyDuplicates     |     9,389.0 ns |    134.94 ns |     59.91 ns |  0.77 |    0.01 |    3 |         - |          NA |
| NaturalMergeSort         | 256  | ManyDuplicates     |     8,905.6 ns |    145.07 ns |     75.88 ns |  0.73 |    0.01 |    3 |         - |          NA |
| PatienceSort             | 256  | ManyDuplicates     |    11,132.3 ns |    240.03 ns |    106.57 ns |  0.91 |    0.01 |    3 |         - |          NA |
| DropMergeSort            | 256  | ManyDuplicates     |     8,097.3 ns |    216.15 ns |     95.97 ns |  0.66 |    0.01 |    3 |         - |          NA |
| TimSort                  | 256  | ManyDuplicates     |     9,671.2 ns |    291.36 ns |    152.39 ns |  0.79 |    0.01 |    3 |         - |          NA |
| PowerSort                | 256  | ManyDuplicates     |     6,056.3 ns |    221.59 ns |     98.39 ns |  0.49 |    0.01 |    2 |         - |          NA |
| ShiftSort                | 256  | ManyDuplicates     |     9,651.2 ns |    343.70 ns |    179.76 ns |  0.79 |    0.02 |    3 |         - |          NA |
| SpinSort                 | 256  | ManyDuplicates     |     5,447.5 ns |    381.38 ns |    199.47 ns |  0.44 |    0.02 |    2 |         - |          NA |
| Glidesort                | 256  | ManyDuplicates     |     5,353.7 ns |    309.22 ns |    137.29 ns |  0.44 |    0.01 |    2 |         - |          NA |
| Driftsort                | 256  | ManyDuplicates     |     4,671.7 ns |    484.19 ns |    214.98 ns |  0.38 |    0.02 |    2 |         - |          NA |
| FlatStableSort           | 256  | ManyDuplicates     |     6,262.4 ns |     47.82 ns |     21.23 ns |  0.51 |    0.00 |    2 |         - |          NA |
| QuadSort                 | 256  | ManyDuplicates     |     3,350.0 ns |    135.19 ns |     48.21 ns |  0.27 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **Random**             |    **58,725.9 ns** |    **354.05 ns** |    **185.17 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Random             |    55,838.8 ns |    703.49 ns |    367.94 ns |  0.95 |    0.01 |    3 |         - |          NA |
| BottomupMergeSort        | 1024 | Random             |    43,857.3 ns |    162.95 ns |     85.22 ns |  0.75 |    0.00 |    3 |         - |          NA |
| StdStableSort            | 1024 | Random             |    29,420.2 ns |  1,120.40 ns |    497.46 ns |  0.50 |    0.01 |    2 |         - |          NA |
| RotateMergeSort          | 1024 | Random             |   108,244.4 ns |  1,368.04 ns |    607.42 ns |  1.84 |    0.01 |    4 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Random             |   127,390.2 ns |  1,219.49 ns |    541.46 ns |  2.17 |    0.01 |    4 |         - |          NA |
| SymMergeSort             | 1024 | Random             |    72,524.2 ns |    660.35 ns |    345.38 ns |  1.23 |    0.01 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | Random             |    49,481.0 ns |    116.79 ns |     51.86 ns |  0.84 |    0.00 |    3 |         - |          NA |
| NaturalMergeSort         | 1024 | Random             |    44,507.5 ns |    336.86 ns |    149.57 ns |  0.76 |    0.00 |    3 |         - |          NA |
| PatienceSort             | 1024 | Random             |    64,696.2 ns |    227.16 ns |    118.81 ns |  1.10 |    0.00 |    3 |         - |          NA |
| DropMergeSort            | 1024 | Random             |    43,648.3 ns |    469.25 ns |    245.43 ns |  0.74 |    0.00 |    3 |         - |          NA |
| TimSort                  | 1024 | Random             |    50,247.4 ns |    480.75 ns |    251.44 ns |  0.86 |    0.00 |    3 |         - |          NA |
| PowerSort                | 1024 | Random             |    35,116.8 ns |    600.25 ns |    266.51 ns |  0.60 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 1024 | Random             |    50,422.5 ns |     87.89 ns |     39.03 ns |  0.86 |    0.00 |    3 |         - |          NA |
| SpinSort                 | 1024 | Random             |    31,463.9 ns |    408.56 ns |    213.68 ns |  0.54 |    0.00 |    2 |         - |          NA |
| Glidesort                | 1024 | Random             |    24,379.4 ns |    449.99 ns |    199.80 ns |  0.42 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | Random             |    20,975.5 ns |    150.97 ns |     67.03 ns |  0.36 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Random             |    34,380.7 ns |    292.48 ns |    152.97 ns |  0.59 |    0.00 |    2 |         - |          NA |
| QuadSort                 | 1024 | Random             |    17,885.5 ns |    360.80 ns |    188.70 ns |  0.30 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **SingleElementMoved** |    **13,861.9 ns** |    **217.89 ns** |    **113.96 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | SingleElementMoved |    16,765.0 ns |    263.77 ns |    117.12 ns |  1.21 |    0.01 |    8 |         - |          NA |
| BottomupMergeSort        | 1024 | SingleElementMoved |     5,311.9 ns |    173.47 ns |     90.73 ns |  0.38 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 1024 | SingleElementMoved |     2,618.2 ns |     24.21 ns |     10.75 ns |  0.19 |    0.00 |    4 |         - |          NA |
| RotateMergeSort          | 1024 | SingleElementMoved |     1,522.1 ns |     79.31 ns |     41.48 ns |  0.11 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 1024 | SingleElementMoved |     2,204.8 ns |    227.45 ns |    100.99 ns |  0.16 |    0.01 |    3 |         - |          NA |
| SymMergeSort             | 1024 | SingleElementMoved |     1,244.4 ns |     86.93 ns |     45.47 ns |  0.09 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 1024 | SingleElementMoved |     9,723.7 ns |    252.79 ns |    132.21 ns |  0.70 |    0.01 |    6 |         - |          NA |
| NaturalMergeSort         | 1024 | SingleElementMoved |     1,033.8 ns |     19.80 ns |      8.79 ns |  0.07 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 1024 | SingleElementMoved |    37,487.7 ns |    582.65 ns |    207.78 ns |  2.70 |    0.03 |    9 |         - |          NA |
| DropMergeSort            | 1024 | SingleElementMoved |     1,867.7 ns |      8.35 ns |      2.98 ns |  0.13 |    0.00 |    3 |         - |          NA |
| TimSort                  | 1024 | SingleElementMoved |       734.8 ns |     34.95 ns |     15.52 ns |  0.05 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | SingleElementMoved |     1,061.4 ns |     52.47 ns |     23.30 ns |  0.08 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 1024 | SingleElementMoved |     1,218.2 ns |      3.98 ns |      1.77 ns |  0.09 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 1024 | SingleElementMoved |     2,833.4 ns |     39.29 ns |     17.45 ns |  0.20 |    0.00 |    4 |         - |          NA |
| Glidesort                | 1024 | SingleElementMoved |     1,849.1 ns |     21.58 ns |      9.58 ns |  0.13 |    0.00 |    3 |         - |          NA |
| Driftsort                | 1024 | SingleElementMoved |     1,109.9 ns |     31.87 ns |     11.37 ns |  0.08 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 1024 | SingleElementMoved |     4,471.8 ns |     37.34 ns |     19.53 ns |  0.32 |    0.00 |    5 |         - |          NA |
| QuadSort                 | 1024 | SingleElementMoved |     1,389.9 ns |    329.96 ns |    172.57 ns |  0.10 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **Sorted**             |    **13,352.7 ns** |    **229.14 ns** |    **119.84 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Sorted             |    16,268.5 ns |    113.74 ns |     59.49 ns |  1.22 |    0.01 |    9 |         - |          NA |
| BottomupMergeSort        | 1024 | Sorted             |     4,548.4 ns |     13.70 ns |      6.08 ns |  0.34 |    0.00 |    6 |         - |          NA |
| StdStableSort            | 1024 | Sorted             |     2,416.5 ns |     18.42 ns |      8.18 ns |  0.18 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 1024 | Sorted             |       930.2 ns |      7.33 ns |      3.83 ns |  0.07 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Sorted             |     1,536.1 ns |     39.87 ns |     14.22 ns |  0.12 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 1024 | Sorted             |       929.6 ns |      8.88 ns |      3.94 ns |  0.07 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | Sorted             |     8,501.0 ns |    332.41 ns |    173.86 ns |  0.64 |    0.01 |    7 |         - |          NA |
| NaturalMergeSort         | 1024 | Sorted             |       524.5 ns |     47.25 ns |     20.98 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 1024 | Sorted             |    31,253.1 ns |  1,214.13 ns |    635.01 ns |  2.34 |    0.05 |   10 |         - |          NA |
| DropMergeSort            | 1024 | Sorted             |     1,728.0 ns |     29.31 ns |     13.01 ns |  0.13 |    0.00 |    4 |         - |          NA |
| TimSort                  | 1024 | Sorted             |       482.3 ns |      7.56 ns |      2.70 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Sorted             |       466.0 ns |      6.86 ns |      3.05 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Sorted             |       638.4 ns |     14.06 ns |      6.24 ns |  0.05 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 1024 | Sorted             |       441.6 ns |     19.86 ns |     10.39 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Sorted             |       435.2 ns |      9.05 ns |      4.73 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | Sorted             |       530.9 ns |     12.37 ns |      5.49 ns |  0.04 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Sorted             |     4,250.9 ns |    294.06 ns |    153.80 ns |  0.32 |    0.01 |    6 |         - |          NA |
| QuadSort                 | 1024 | Sorted             |       676.7 ns |     12.42 ns |      5.51 ns |  0.05 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **Reversed**           |    **29,133.6 ns** |    **447.27 ns** |    **233.93 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Reversed           |    25,598.3 ns |    449.71 ns |    235.21 ns |  0.88 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 1024 | Reversed           |    15,880.6 ns |     75.85 ns |     39.67 ns |  0.55 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 1024 | Reversed           |    13,699.0 ns |    314.30 ns |    139.55 ns |  0.47 |    0.01 |    4 |         - |          NA |
| RotateMergeSort          | 1024 | Reversed           |    20,489.1 ns |    131.90 ns |     68.99 ns |  0.70 |    0.01 |    5 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Reversed           |    27,925.0 ns |    192.77 ns |    100.82 ns |  0.96 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 1024 | Reversed           |     7,624.2 ns |    264.81 ns |    138.50 ns |  0.26 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | Reversed           |    11,640.6 ns |    252.29 ns |    131.95 ns |  0.40 |    0.01 |    4 |         - |          NA |
| NaturalMergeSort         | 1024 | Reversed           |       763.4 ns |      8.38 ns |      4.38 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 1024 | Reversed           |     4,266.7 ns |     16.99 ns |      7.54 ns |  0.15 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 1024 | Reversed           |     9,867.8 ns |    691.16 ns |    361.49 ns |  0.34 |    0.01 |    4 |         - |          NA |
| TimSort                  | 1024 | Reversed           |       766.2 ns |     41.11 ns |     18.25 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Reversed           |       716.1 ns |      6.50 ns |      3.40 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Reversed           |    14,581.7 ns |    169.28 ns |     88.54 ns |  0.50 |    0.00 |    4 |         - |          NA |
| SpinSort                 | 1024 | Reversed           |       775.5 ns |     48.50 ns |     21.53 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Reversed           |       791.6 ns |     32.32 ns |     14.35 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | Reversed           |       771.0 ns |     19.41 ns |      6.92 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Reversed           |     7,044.0 ns |    320.07 ns |    167.40 ns |  0.24 |    0.01 |    3 |         - |          NA |
| QuadSort                 | 1024 | Reversed           |       798.7 ns |     17.89 ns |      9.36 ns |  0.03 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **PipeOrgan**          |    **23,162.4 ns** |  **6,537.96 ns** |  **2,902.89 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | PipeOrgan          |    21,333.2 ns |    133.73 ns |     69.94 ns |  0.93 |    0.09 |    5 |         - |          NA |
| BottomupMergeSort        | 1024 | PipeOrgan          |    10,283.1 ns |    266.37 ns |    139.31 ns |  0.45 |    0.04 |    4 |         - |          NA |
| StdStableSort            | 1024 | PipeOrgan          |     8,520.8 ns |    381.00 ns |    199.27 ns |  0.37 |    0.04 |    4 |         - |          NA |
| RotateMergeSort          | 1024 | PipeOrgan          |    19,585.0 ns |    447.48 ns |    198.68 ns |  0.86 |    0.08 |    5 |         - |          NA |
| RotateMergeSortRecursive | 1024 | PipeOrgan          |    25,174.8 ns |    188.10 ns |     98.38 ns |  1.10 |    0.11 |    5 |         - |          NA |
| SymMergeSort             | 1024 | PipeOrgan          |    11,534.8 ns |    373.70 ns |    165.92 ns |  0.50 |    0.05 |    4 |         - |          NA |
| BlockMergeSort           | 1024 | PipeOrgan          |    12,102.9 ns |    283.96 ns |    148.52 ns |  0.53 |    0.05 |    4 |         - |          NA |
| NaturalMergeSort         | 1024 | PipeOrgan          |     1,376.6 ns |      8.11 ns |      4.24 ns |  0.06 |    0.01 |    1 |         - |          NA |
| PatienceSort             | 1024 | PipeOrgan          |    22,522.0 ns |    641.55 ns |    284.85 ns |  0.98 |    0.10 |    5 |         - |          NA |
| DropMergeSort            | 1024 | PipeOrgan          |    21,686.3 ns |    131.99 ns |     58.61 ns |  0.95 |    0.09 |    5 |         - |          NA |
| TimSort                  | 1024 | PipeOrgan          |     2,233.8 ns |    333.80 ns |    174.58 ns |  0.10 |    0.01 |    2 |         - |          NA |
| PowerSort                | 1024 | PipeOrgan          |     1,347.8 ns |     14.71 ns |      5.25 ns |  0.06 |    0.01 |    1 |         - |          NA |
| ShiftSort                | 1024 | PipeOrgan          |     9,376.0 ns |    337.32 ns |    149.77 ns |  0.41 |    0.04 |    4 |         - |          NA |
| SpinSort                 | 1024 | PipeOrgan          |     7,110.4 ns |    326.31 ns |    170.67 ns |  0.31 |    0.03 |    4 |         - |          NA |
| Glidesort                | 1024 | PipeOrgan          |     3,812.7 ns |    270.52 ns |    141.48 ns |  0.17 |    0.02 |    3 |         - |          NA |
| Driftsort                | 1024 | PipeOrgan          |     1,194.6 ns |      6.66 ns |      2.96 ns |  0.05 |    0.01 |    1 |         - |          NA |
| FlatStableSort           | 1024 | PipeOrgan          |     6,425.5 ns |    188.51 ns |     83.70 ns |  0.28 |    0.03 |    4 |         - |          NA |
| QuadSort                 | 1024 | PipeOrgan          |     2,403.8 ns |    137.22 ns |     71.77 ns |  0.10 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **ManyDuplicates**     |    **52,389.5 ns** |    **265.91 ns** |    **139.08 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | ManyDuplicates     |    49,269.8 ns |    454.48 ns |    201.79 ns |  0.94 |    0.00 |    4 |         - |          NA |
| BottomupMergeSort        | 1024 | ManyDuplicates     |    37,266.8 ns |    383.04 ns |    200.34 ns |  0.71 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 1024 | ManyDuplicates     |    23,195.5 ns |    970.67 ns |    430.99 ns |  0.44 |    0.01 |    4 |         - |          NA |
| RotateMergeSort          | 1024 | ManyDuplicates     |    95,397.8 ns |    973.07 ns |    432.05 ns |  1.82 |    0.01 |    5 |         - |          NA |
| RotateMergeSortRecursive | 1024 | ManyDuplicates     |   112,277.9 ns |  1,184.58 ns |    525.96 ns |  2.14 |    0.01 |    5 |         - |          NA |
| SymMergeSort             | 1024 | ManyDuplicates     |    62,912.2 ns |    915.24 ns |    478.69 ns |  1.20 |    0.01 |    4 |         - |          NA |
| BlockMergeSort           | 1024 | ManyDuplicates     |    46,472.8 ns |    207.34 ns |     92.06 ns |  0.89 |    0.00 |    4 |         - |          NA |
| NaturalMergeSort         | 1024 | ManyDuplicates     |    38,563.5 ns |    336.92 ns |    176.21 ns |  0.74 |    0.00 |    4 |         - |          NA |
| PatienceSort             | 1024 | ManyDuplicates     |    41,237.2 ns |    307.78 ns |    160.97 ns |  0.79 |    0.00 |    4 |         - |          NA |
| DropMergeSort            | 1024 | ManyDuplicates     |    31,099.0 ns |    361.16 ns |    188.89 ns |  0.59 |    0.00 |    4 |         - |          NA |
| TimSort                  | 1024 | ManyDuplicates     |    43,716.2 ns |    282.39 ns |    147.70 ns |  0.83 |    0.00 |    4 |         - |          NA |
| PowerSort                | 1024 | ManyDuplicates     |    28,111.8 ns |    411.69 ns |    182.79 ns |  0.54 |    0.00 |    4 |         - |          NA |
| ShiftSort                | 1024 | ManyDuplicates     |    43,224.2 ns |    260.56 ns |    136.28 ns |  0.83 |    0.00 |    4 |         - |          NA |
| SpinSort                 | 1024 | ManyDuplicates     |    25,505.2 ns |    581.03 ns |    303.89 ns |  0.49 |    0.01 |    4 |         - |          NA |
| Glidesort                | 1024 | ManyDuplicates     |    13,888.5 ns |    350.62 ns |    155.68 ns |  0.27 |    0.00 |    2 |         - |          NA |
| Driftsort                | 1024 | ManyDuplicates     |    11,551.6 ns |    365.52 ns |    191.18 ns |  0.22 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | ManyDuplicates     |    28,641.8 ns |    150.95 ns |     67.02 ns |  0.55 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 1024 | ManyDuplicates     |    17,853.7 ns |    186.04 ns |     97.30 ns |  0.34 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **Random**             |   **265,909.9 ns** |    **899.65 ns** |    **399.45 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Random             |   253,180.6 ns |  3,123.49 ns |  1,633.64 ns |  0.95 |    0.01 |    2 |         - |          NA |
| BottomupMergeSort        | 4096 | Random             |   197,939.5 ns |    921.52 ns |    409.16 ns |  0.74 |    0.00 |    2 |         - |          NA |
| StdStableSort            | 4096 | Random             |   148,544.8 ns |  4,163.75 ns |  2,177.72 ns |  0.56 |    0.01 |    2 |         - |          NA |
| RotateMergeSort          | 4096 | Random             |   571,966.3 ns |  4,390.70 ns |  2,296.42 ns |  2.15 |    0.01 |    4 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Random             |   664,094.1 ns |  2,373.03 ns |  1,053.64 ns |  2.50 |    0.01 |    4 |         - |          NA |
| SymMergeSort             | 4096 | Random             |   385,889.9 ns |  1,263.43 ns |    660.80 ns |  1.45 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 4096 | Random             |   242,962.0 ns |    590.12 ns |    262.01 ns |  0.91 |    0.00 |    2 |         - |          NA |
| NaturalMergeSort         | 4096 | Random             |   207,940.1 ns |    528.50 ns |    188.47 ns |  0.78 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 4096 | Random             |   303,804.1 ns |  1,067.58 ns |    474.01 ns |  1.14 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 4096 | Random             |   199,083.3 ns |  2,772.42 ns |  1,450.03 ns |  0.75 |    0.01 |    2 |         - |          NA |
| TimSort                  | 4096 | Random             |   234,815.5 ns |  1,005.49 ns |    446.44 ns |  0.88 |    0.00 |    2 |         - |          NA |
| PowerSort                | 4096 | Random             |   174,823.0 ns |    795.53 ns |    353.22 ns |  0.66 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 4096 | Random             |   242,295.0 ns |  1,450.94 ns |    758.87 ns |  0.91 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 4096 | Random             |   156,283.8 ns |    479.68 ns |    212.98 ns |  0.59 |    0.00 |    2 |         - |          NA |
| Glidesort                | 4096 | Random             |   100,597.2 ns |    902.90 ns |    400.90 ns |  0.38 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | Random             |    86,236.6 ns |  1,024.40 ns |    535.78 ns |  0.32 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Random             |   163,206.4 ns |  1,272.64 ns |    565.06 ns |  0.61 |    0.00 |    2 |         - |          NA |
| QuadSort                 | 4096 | Random             |    82,906.4 ns |  1,059.39 ns |    554.08 ns |  0.31 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **SingleElementMoved** |    **55,400.1 ns** |    **395.20 ns** |    **206.70 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | SingleElementMoved |    67,291.1 ns |    313.13 ns |    139.03 ns |  1.21 |    0.00 |    9 |         - |          NA |
| BottomupMergeSort        | 4096 | SingleElementMoved |    20,170.9 ns |    305.86 ns |    135.80 ns |  0.36 |    0.00 |    6 |         - |          NA |
| StdStableSort            | 4096 | SingleElementMoved |    12,832.1 ns |    254.17 ns |    132.93 ns |  0.23 |    0.00 |    4 |         - |          NA |
| RotateMergeSort          | 4096 | SingleElementMoved |     5,083.3 ns |    279.25 ns |    146.05 ns |  0.09 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 4096 | SingleElementMoved |     7,584.1 ns |    408.47 ns |    213.64 ns |  0.14 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 4096 | SingleElementMoved |     4,478.1 ns |    369.85 ns |    193.44 ns |  0.08 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 4096 | SingleElementMoved |    38,598.1 ns |    193.31 ns |     85.83 ns |  0.70 |    0.00 |    7 |         - |          NA |
| NaturalMergeSort         | 4096 | SingleElementMoved |     3,876.6 ns |    256.26 ns |    134.03 ns |  0.07 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 4096 | SingleElementMoved |   263,688.2 ns |  5,086.74 ns |  2,660.46 ns |  4.76 |    0.05 |   10 |         - |          NA |
| DropMergeSort            | 4096 | SingleElementMoved |     7,482.6 ns |    515.12 ns |    269.42 ns |  0.14 |    0.00 |    3 |         - |          NA |
| TimSort                  | 4096 | SingleElementMoved |     2,453.6 ns |     10.44 ns |      4.64 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | SingleElementMoved |     4,026.5 ns |    426.75 ns |    223.20 ns |  0.07 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 4096 | SingleElementMoved |     4,482.3 ns |    343.95 ns |    179.89 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 4096 | SingleElementMoved |     8,545.2 ns |    358.71 ns |    187.61 ns |  0.15 |    0.00 |    3 |         - |          NA |
| Glidesort                | 4096 | SingleElementMoved |     6,888.1 ns |    267.12 ns |    118.60 ns |  0.12 |    0.00 |    3 |         - |          NA |
| Driftsort                | 4096 | SingleElementMoved |     3,696.9 ns |     37.96 ns |     13.54 ns |  0.07 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 4096 | SingleElementMoved |    16,605.8 ns |    208.59 ns |    109.10 ns |  0.30 |    0.00 |    5 |         - |          NA |
| QuadSort                 | 4096 | SingleElementMoved |     3,977.6 ns |    267.43 ns |    139.87 ns |  0.07 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **Sorted**             |    **53,142.7 ns** |    **505.44 ns** |    **224.42 ns** |  **1.00** |    **0.00** |    **9** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Sorted             |    65,522.9 ns |    216.25 ns |    113.10 ns |  1.23 |    0.01 |   10 |         - |          NA |
| BottomupMergeSort        | 4096 | Sorted             |    18,512.3 ns |    196.98 ns |    103.02 ns |  0.35 |    0.00 |    7 |         - |          NA |
| StdStableSort            | 4096 | Sorted             |    12,083.7 ns |    161.53 ns |     71.72 ns |  0.23 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 4096 | Sorted             |     3,730.1 ns |      9.98 ns |      3.56 ns |  0.07 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Sorted             |     6,342.7 ns |    405.27 ns |    211.96 ns |  0.12 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 4096 | Sorted             |     3,607.7 ns |     46.97 ns |     16.75 ns |  0.07 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 4096 | Sorted             |    33,753.1 ns |    130.70 ns |     58.03 ns |  0.64 |    0.00 |    8 |         - |          NA |
| NaturalMergeSort         | 4096 | Sorted             |     1,832.5 ns |      9.88 ns |      3.52 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 4096 | Sorted             |   186,939.8 ns | 22,361.05 ns | 11,695.26 ns |  3.52 |    0.21 |   11 |         - |          NA |
| DropMergeSort            | 4096 | Sorted             |     6,935.5 ns |    290.32 ns |    151.84 ns |  0.13 |    0.00 |    4 |         - |          NA |
| TimSort                  | 4096 | Sorted             |     1,851.4 ns |     62.50 ns |     22.29 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | Sorted             |     1,786.2 ns |      7.89 ns |      3.50 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | Sorted             |     2,319.8 ns |     13.13 ns |      4.68 ns |  0.04 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 4096 | Sorted             |     1,550.9 ns |     12.40 ns |      5.51 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 4096 | Sorted             |     1,609.0 ns |    157.15 ns |     69.78 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | Sorted             |     1,862.7 ns |     46.33 ns |     20.57 ns |  0.04 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Sorted             |    14,822.5 ns |    241.71 ns |    126.42 ns |  0.28 |    0.00 |    6 |         - |          NA |
| QuadSort                 | 4096 | Sorted             |     2,431.3 ns |    167.40 ns |     74.32 ns |  0.05 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **Reversed**           |   **119,521.0 ns** |  **2,076.48 ns** |    **921.97 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Reversed           |   104,669.1 ns |    275.56 ns |     98.27 ns |  0.88 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 4096 | Reversed           |    63,787.3 ns |    116.60 ns |     51.77 ns |  0.53 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 4096 | Reversed           |    57,970.2 ns |    250.05 ns |    130.78 ns |  0.49 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 4096 | Reversed           |    93,678.4 ns |    265.56 ns |    117.91 ns |  0.78 |    0.01 |    6 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Reversed           |   124,122.5 ns |  1,630.93 ns |    724.14 ns |  1.04 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 4096 | Reversed           |    32,500.4 ns |    255.60 ns |    113.49 ns |  0.27 |    0.00 |    4 |         - |          NA |
| BlockMergeSort           | 4096 | Reversed           |    52,309.2 ns |    500.76 ns |    261.91 ns |  0.44 |    0.00 |    5 |         - |          NA |
| NaturalMergeSort         | 4096 | Reversed           |     2,852.4 ns |     71.92 ns |     25.65 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 4096 | Reversed           |    16,770.6 ns |    232.79 ns |    121.75 ns |  0.14 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 4096 | Reversed           |    24,215.5 ns |    272.85 ns |    142.71 ns |  0.20 |    0.00 |    3 |         - |          NA |
| TimSort                  | 4096 | Reversed           |     2,824.6 ns |     66.67 ns |     29.60 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | Reversed           |     2,937.6 ns |    249.86 ns |    110.94 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | Reversed           |    65,146.9 ns |    380.45 ns |    198.98 ns |  0.55 |    0.00 |    5 |         - |          NA |
| SpinSort                 | 4096 | Reversed           |     3,246.4 ns |    624.84 ns |    326.80 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 4096 | Reversed           |     2,845.3 ns |     81.34 ns |     36.11 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | Reversed           |     2,855.3 ns |     29.16 ns |     10.40 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Reversed           |    31,665.2 ns |    434.37 ns |    192.86 ns |  0.26 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 4096 | Reversed           |     2,991.3 ns |     12.74 ns |      4.54 ns |  0.03 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **PipeOrgan**          |    **88,220.4 ns** |    **586.73 ns** |    **306.87 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | PipeOrgan          |    86,370.1 ns |    377.35 ns |    167.55 ns |  0.98 |    0.00 |    7 |         - |          NA |
| BottomupMergeSort        | 4096 | PipeOrgan          |    42,439.1 ns |    185.13 ns |     82.20 ns |  0.48 |    0.00 |    6 |         - |          NA |
| StdStableSort            | 4096 | PipeOrgan          |    36,677.0 ns |    344.05 ns |    179.95 ns |  0.42 |    0.00 |    6 |         - |          NA |
| RotateMergeSort          | 4096 | PipeOrgan          |    88,159.4 ns |    580.78 ns |    303.76 ns |  1.00 |    0.00 |    7 |         - |          NA |
| RotateMergeSortRecursive | 4096 | PipeOrgan          |   108,430.5 ns |    447.19 ns |    198.55 ns |  1.23 |    0.00 |    8 |         - |          NA |
| SymMergeSort             | 4096 | PipeOrgan          |    48,531.2 ns |    685.24 ns |    358.39 ns |  0.55 |    0.00 |    6 |         - |          NA |
| BlockMergeSort           | 4096 | PipeOrgan          |    48,952.2 ns |    380.32 ns |    198.91 ns |  0.55 |    0.00 |    6 |         - |          NA |
| NaturalMergeSort         | 4096 | PipeOrgan          |     5,327.8 ns |    235.65 ns |    123.25 ns |  0.06 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 4096 | PipeOrgan          |   130,195.7 ns |  6,292.74 ns |  2,794.01 ns |  1.48 |    0.03 |    9 |         - |          NA |
| DropMergeSort            | 4096 | PipeOrgan          |    89,661.7 ns |    567.27 ns |    296.69 ns |  1.02 |    0.00 |    7 |         - |          NA |
| TimSort                  | 4096 | PipeOrgan          |     8,281.5 ns |    382.27 ns |    169.73 ns |  0.09 |    0.00 |    3 |         - |          NA |
| PowerSort                | 4096 | PipeOrgan          |     5,174.3 ns |     33.66 ns |     12.00 ns |  0.06 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | PipeOrgan          |    41,021.8 ns |  1,088.58 ns |    483.34 ns |  0.46 |    0.01 |    6 |         - |          NA |
| SpinSort                 | 4096 | PipeOrgan          |     6,733.7 ns |    400.72 ns |    209.59 ns |  0.08 |    0.00 |    2 |         - |          NA |
| Glidesort                | 4096 | PipeOrgan          |    14,859.7 ns |    268.17 ns |    140.26 ns |  0.17 |    0.00 |    4 |         - |          NA |
| Driftsort                | 4096 | PipeOrgan          |     4,590.5 ns |    277.18 ns |    144.97 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | PipeOrgan          |    28,294.3 ns |    469.09 ns |    208.28 ns |  0.32 |    0.00 |    5 |         - |          NA |
| QuadSort                 | 4096 | PipeOrgan          |     9,208.0 ns |    226.43 ns |    118.43 ns |  0.10 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **ManyDuplicates**     |   **216,256.3 ns** |  **2,597.99 ns** |  **1,358.80 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | ManyDuplicates     |   203,734.5 ns |  1,576.22 ns |    699.85 ns |  0.94 |    0.01 |    4 |         - |          NA |
| BottomupMergeSort        | 4096 | ManyDuplicates     |   152,885.8 ns |    821.43 ns |    292.93 ns |  0.71 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 4096 | ManyDuplicates     |    99,548.2 ns |  2,264.43 ns |  1,005.42 ns |  0.46 |    0.01 |    4 |         - |          NA |
| RotateMergeSort          | 4096 | ManyDuplicates     |   427,831.7 ns |  1,914.57 ns |  1,001.36 ns |  1.98 |    0.01 |    6 |         - |          NA |
| RotateMergeSortRecursive | 4096 | ManyDuplicates     |   504,382.6 ns |  2,325.27 ns |  1,216.16 ns |  2.33 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 4096 | ManyDuplicates     |   287,710.7 ns |  1,392.06 ns |    728.08 ns |  1.33 |    0.01 |    5 |         - |          NA |
| BlockMergeSort           | 4096 | ManyDuplicates     |   213,825.5 ns |    903.79 ns |    401.29 ns |  0.99 |    0.01 |    4 |         - |          NA |
| NaturalMergeSort         | 4096 | ManyDuplicates     |   158,705.7 ns |  2,281.41 ns |  1,012.96 ns |  0.73 |    0.01 |    4 |         - |          NA |
| PatienceSort             | 4096 | ManyDuplicates     |   139,918.5 ns |    761.72 ns |    398.39 ns |  0.65 |    0.00 |    4 |         - |          NA |
| DropMergeSort            | 4096 | ManyDuplicates     |   118,284.7 ns |    782.84 ns |    347.59 ns |  0.55 |    0.00 |    4 |         - |          NA |
| TimSort                  | 4096 | ManyDuplicates     |   183,996.4 ns |  1,082.41 ns |    480.60 ns |  0.85 |    0.01 |    4 |         - |          NA |
| PowerSort                | 4096 | ManyDuplicates     |   121,924.4 ns |  4,875.81 ns |  2,550.15 ns |  0.56 |    0.01 |    4 |         - |          NA |
| ShiftSort                | 4096 | ManyDuplicates     |   183,774.1 ns |    494.53 ns |    219.58 ns |  0.85 |    0.01 |    4 |         - |          NA |
| SpinSort                 | 4096 | ManyDuplicates     |   108,894.3 ns |    960.20 ns |    426.34 ns |  0.50 |    0.00 |    4 |         - |          NA |
| Glidesort                | 4096 | ManyDuplicates     |    22,084.9 ns |    286.85 ns |    127.36 ns |  0.10 |    0.00 |    2 |         - |          NA |
| Driftsort                | 4096 | ManyDuplicates     |    16,800.5 ns |    293.77 ns |    153.65 ns |  0.08 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | ManyDuplicates     |   116,143.4 ns |  1,813.29 ns |    805.11 ns |  0.54 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 4096 | ManyDuplicates     |    76,841.1 ns |    494.80 ns |    219.70 ns |  0.36 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **Random**             |   **564,968.0 ns** |  **3,287.21 ns** |  **1,459.54 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Random             |   533,502.3 ns |  1,528.94 ns |    678.86 ns |  0.94 |    0.00 |    2 |         - |          NA |
| BottomupMergeSort        | 8192 | Random             |   423,022.2 ns |  1,771.38 ns |    926.47 ns |  0.75 |    0.00 |    2 |         - |          NA |
| StdStableSort            | 8192 | Random             |   362,980.4 ns | 49,495.28 ns | 21,976.21 ns |  0.64 |    0.04 |    2 |         - |          NA |
| RotateMergeSort          | 8192 | Random             | 1,286,589.3 ns |  5,878.91 ns |  2,610.27 ns |  2.28 |    0.01 |    4 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Random             | 1,485,929.0 ns |  2,712.56 ns |  1,418.72 ns |  2.63 |    0.01 |    4 |         - |          NA |
| SymMergeSort             | 8192 | Random             |   869,816.6 ns |  2,587.43 ns |  1,148.84 ns |  1.54 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 8192 | Random             |   526,535.6 ns |  1,627.26 ns |    722.51 ns |  0.93 |    0.00 |    2 |         - |          NA |
| NaturalMergeSort         | 8192 | Random             |   445,811.7 ns |  1,369.65 ns |    716.36 ns |  0.79 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 8192 | Random             |   657,145.8 ns |  1,860.38 ns |    973.02 ns |  1.16 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 8192 | Random             |   421,404.8 ns |  3,987.01 ns |  1,770.26 ns |  0.75 |    0.00 |    2 |         - |          NA |
| TimSort                  | 8192 | Random             |   514,185.2 ns |    899.70 ns |    399.47 ns |  0.91 |    0.00 |    2 |         - |          NA |
| PowerSort                | 8192 | Random             |   383,725.8 ns |  1,768.10 ns |    924.75 ns |  0.68 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 8192 | Random             |   529,387.4 ns |    857.85 ns |    305.92 ns |  0.94 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | Random             |   343,705.8 ns |  1,670.82 ns |    873.87 ns |  0.61 |    0.00 |    2 |         - |          NA |
| Glidesort                | 8192 | Random             |   202,684.8 ns |  1,056.38 ns |    552.51 ns |  0.36 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Random             |   173,835.9 ns |  1,709.41 ns |    758.99 ns |  0.31 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Random             |   369,752.8 ns |  1,915.82 ns |  1,002.01 ns |  0.65 |    0.00 |    2 |         - |          NA |
| QuadSort                 | 8192 | Random             |   176,720.5 ns |    498.85 ns |    221.49 ns |  0.31 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **SingleElementMoved** |   **110,284.8 ns** |  **1,036.33 ns** |    **460.14 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | SingleElementMoved |   134,204.2 ns |  1,282.42 ns |    457.32 ns |  1.22 |    0.01 |    8 |         - |          NA |
| BottomupMergeSort        | 8192 | SingleElementMoved |    42,317.2 ns |    340.38 ns |    178.03 ns |  0.38 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 8192 | SingleElementMoved |    24,867.6 ns |  1,467.90 ns |    767.74 ns |  0.23 |    0.01 |    4 |         - |          NA |
| RotateMergeSort          | 8192 | SingleElementMoved |     9,409.8 ns |     61.76 ns |     22.03 ns |  0.09 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 8192 | SingleElementMoved |    14,332.1 ns |    367.02 ns |    162.96 ns |  0.13 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 8192 | SingleElementMoved |     8,474.0 ns |    190.76 ns |     99.77 ns |  0.08 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 8192 | SingleElementMoved |    76,400.3 ns |    328.03 ns |    145.65 ns |  0.69 |    0.00 |    6 |         - |          NA |
| NaturalMergeSort         | 8192 | SingleElementMoved |     7,815.1 ns |     44.03 ns |     19.55 ns |  0.07 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 8192 | SingleElementMoved |   622,772.6 ns |  2,856.55 ns |  1,268.33 ns |  5.65 |    0.02 |    9 |         - |          NA |
| DropMergeSort            | 8192 | SingleElementMoved |    14,739.3 ns |    309.68 ns |    161.97 ns |  0.13 |    0.00 |    3 |         - |          NA |
| TimSort                  | 8192 | SingleElementMoved |     4,861.8 ns |    175.55 ns |     91.82 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | SingleElementMoved |     8,022.0 ns |    773.14 ns |    404.37 ns |  0.07 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 8192 | SingleElementMoved |     8,869.1 ns |    455.17 ns |    202.10 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | SingleElementMoved |    14,678.6 ns |    541.54 ns |    283.24 ns |  0.13 |    0.00 |    3 |         - |          NA |
| Glidesort                | 8192 | SingleElementMoved |    13,805.8 ns |    672.03 ns |    298.39 ns |  0.13 |    0.00 |    3 |         - |          NA |
| Driftsort                | 8192 | SingleElementMoved |     7,794.4 ns |    236.91 ns |    123.91 ns |  0.07 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 8192 | SingleElementMoved |    23,433.0 ns |    455.50 ns |    238.24 ns |  0.21 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 8192 | SingleElementMoved |     7,411.0 ns |     65.10 ns |     28.90 ns |  0.07 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **Sorted**             |   **106,326.8 ns** |    **340.40 ns** |    **151.14 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Sorted             |   131,818.3 ns |    843.76 ns |    374.63 ns |  1.24 |    0.00 |    9 |         - |          NA |
| BottomupMergeSort        | 8192 | Sorted             |    39,514.5 ns |    572.23 ns |    254.07 ns |  0.37 |    0.00 |    6 |         - |          NA |
| StdStableSort            | 8192 | Sorted             |    23,193.1 ns |    765.95 ns |    400.61 ns |  0.22 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 8192 | Sorted             |     7,689.0 ns |     98.24 ns |     51.38 ns |  0.07 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Sorted             |    12,456.4 ns |    219.08 ns |     97.27 ns |  0.12 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 8192 | Sorted             |     7,280.1 ns |    364.33 ns |    190.55 ns |  0.07 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 8192 | Sorted             |    66,568.6 ns |    248.11 ns |    129.76 ns |  0.63 |    0.00 |    7 |         - |          NA |
| NaturalMergeSort         | 8192 | Sorted             |     3,603.7 ns |     20.49 ns |      9.10 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 8192 | Sorted             |   622,922.7 ns | 27,954.20 ns | 12,411.84 ns |  5.86 |    0.11 |   10 |         - |          NA |
| DropMergeSort            | 8192 | Sorted             |    13,590.2 ns |    340.32 ns |    151.10 ns |  0.13 |    0.00 |    4 |         - |          NA |
| TimSort                  | 8192 | Sorted             |     3,561.2 ns |     13.60 ns |      4.85 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Sorted             |     3,563.8 ns |      9.18 ns |      3.27 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Sorted             |     4,540.3 ns |     35.90 ns |     12.80 ns |  0.04 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | Sorted             |     3,171.9 ns |    194.11 ns |     86.19 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Sorted             |     3,093.7 ns |    110.90 ns |     39.55 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Sorted             |     3,690.4 ns |    367.42 ns |    163.14 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Sorted             |     3,375.9 ns |    472.84 ns |    247.30 ns |  0.03 |    0.00 |    1 |         - |          NA |
| QuadSort                 | 8192 | Sorted             |     4,828.0 ns |    156.80 ns |     82.01 ns |  0.05 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **Reversed**           |   **242,799.9 ns** |  **2,990.64 ns** |  **1,327.86 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Reversed           |   211,479.0 ns |  1,557.78 ns |    691.66 ns |  0.87 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 8192 | Reversed           |   130,860.6 ns |    374.01 ns |    166.06 ns |  0.54 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 8192 | Reversed           |   209,276.8 ns |  1,739.07 ns |    909.57 ns |  0.86 |    0.01 |    6 |         - |          NA |
| RotateMergeSort          | 8192 | Reversed           |   246,045.7 ns | 79,709.51 ns | 41,689.61 ns |  1.01 |    0.16 |    6 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Reversed           |   259,195.6 ns |    747.76 ns |    266.66 ns |  1.07 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 8192 | Reversed           |    67,011.8 ns |    378.40 ns |    168.01 ns |  0.28 |    0.00 |    4 |         - |          NA |
| BlockMergeSort           | 8192 | Reversed           |   109,455.3 ns |  2,005.92 ns |  1,049.14 ns |  0.45 |    0.00 |    5 |         - |          NA |
| NaturalMergeSort         | 8192 | Reversed           |     5,727.7 ns |    357.07 ns |    186.76 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 8192 | Reversed           |    33,421.1 ns |    573.87 ns |    300.14 ns |  0.14 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 8192 | Reversed           |    50,243.5 ns |    344.99 ns |    180.44 ns |  0.21 |    0.00 |    3 |         - |          NA |
| TimSort                  | 8192 | Reversed           |     5,773.6 ns |    400.47 ns |    209.45 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Reversed           |     5,619.4 ns |    264.12 ns |    138.14 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Reversed           |   143,249.3 ns |    583.69 ns |    259.16 ns |  0.59 |    0.00 |    5 |         - |          NA |
| SpinSort                 | 8192 | Reversed           |     6,081.1 ns |    414.44 ns |    184.01 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Reversed           |     5,740.4 ns |    338.22 ns |    176.89 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Reversed           |     5,598.0 ns |     17.07 ns |      6.09 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Reversed           |     6,131.0 ns |    303.82 ns |    158.91 ns |  0.03 |    0.00 |    1 |         - |          NA |
| QuadSort                 | 8192 | Reversed           |     6,073.4 ns |    279.40 ns |    146.13 ns |  0.03 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **PipeOrgan**          |   **176,529.9 ns** |    **165.25 ns** |     **58.93 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | PipeOrgan          |   174,390.6 ns |  1,550.04 ns |    810.70 ns |  0.99 |    0.00 |    6 |         - |          NA |
| BottomupMergeSort        | 8192 | PipeOrgan          |    87,409.7 ns |    702.03 ns |    311.71 ns |  0.50 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 8192 | PipeOrgan          |   119,676.0 ns |  5,292.37 ns |  2,768.01 ns |  0.68 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 8192 | PipeOrgan          |   184,527.8 ns |  1,424.43 ns |    745.00 ns |  1.05 |    0.00 |    6 |         - |          NA |
| RotateMergeSortRecursive | 8192 | PipeOrgan          |   225,070.2 ns |    694.29 ns |    308.27 ns |  1.27 |    0.00 |    7 |         - |          NA |
| SymMergeSort             | 8192 | PipeOrgan          |    99,551.8 ns |  2,773.04 ns |  1,231.25 ns |  0.56 |    0.01 |    5 |         - |          NA |
| BlockMergeSort           | 8192 | PipeOrgan          |   100,427.6 ns |  1,042.17 ns |    462.73 ns |  0.57 |    0.00 |    5 |         - |          NA |
| NaturalMergeSort         | 8192 | PipeOrgan          |    10,613.9 ns |    346.24 ns |    181.09 ns |  0.06 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 8192 | PipeOrgan          |   489,093.4 ns |  3,552.90 ns |  1,858.24 ns |  2.77 |    0.01 |    8 |         - |          NA |
| DropMergeSort            | 8192 | PipeOrgan          |   182,742.3 ns |  2,492.69 ns |  1,106.77 ns |  1.04 |    0.01 |    6 |         - |          NA |
| TimSort                  | 8192 | PipeOrgan          |    16,426.4 ns |    254.53 ns |    133.13 ns |  0.09 |    0.00 |    2 |         - |          NA |
| PowerSort                | 8192 | PipeOrgan          |    10,434.4 ns |    366.32 ns |    191.59 ns |  0.06 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | PipeOrgan          |    81,803.8 ns |    666.55 ns |    295.95 ns |  0.46 |    0.00 |    5 |         - |          NA |
| SpinSort                 | 8192 | PipeOrgan          |    13,809.1 ns |    348.80 ns |    182.43 ns |  0.08 |    0.00 |    2 |         - |          NA |
| Glidesort                | 8192 | PipeOrgan          |    29,324.1 ns |    363.60 ns |    190.17 ns |  0.17 |    0.00 |    3 |         - |          NA |
| Driftsort                | 8192 | PipeOrgan          |     9,082.1 ns |    448.08 ns |    234.35 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | PipeOrgan          |    51,793.7 ns |  1,373.08 ns |    718.15 ns |  0.29 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 8192 | PipeOrgan          |    18,762.4 ns |    178.43 ns |     79.22 ns |  0.11 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **ManyDuplicates**     |   **438,076.5 ns** |  **2,065.66 ns** |  **1,080.38 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | ManyDuplicates     |   410,144.3 ns |  1,863.60 ns |    974.70 ns |  0.94 |    0.00 |    4 |         - |          NA |
| BottomupMergeSort        | 8192 | ManyDuplicates     |   308,911.5 ns |  1,749.98 ns |    777.00 ns |  0.71 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 8192 | ManyDuplicates     |   219,216.5 ns |  5,405.53 ns |  2,400.09 ns |  0.50 |    0.01 |    4 |         - |          NA |
| RotateMergeSort          | 8192 | ManyDuplicates     |   885,298.2 ns |  4,026.69 ns |  1,435.96 ns |  2.02 |    0.01 |    6 |         - |          NA |
| RotateMergeSortRecursive | 8192 | ManyDuplicates     | 1,044,944.5 ns |  2,908.03 ns |  1,520.96 ns |  2.39 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 8192 | ManyDuplicates     |   602,438.5 ns |  1,716.22 ns |    762.01 ns |  1.38 |    0.00 |    5 |         - |          NA |
| BlockMergeSort           | 8192 | ManyDuplicates     |   438,689.8 ns |  1,242.85 ns |    551.83 ns |  1.00 |    0.00 |    4 |         - |          NA |
| NaturalMergeSort         | 8192 | ManyDuplicates     |   319,248.2 ns |    657.02 ns |    291.72 ns |  0.73 |    0.00 |    4 |         - |          NA |
| PatienceSort             | 8192 | ManyDuplicates     |   280,547.4 ns |    645.72 ns |    337.72 ns |  0.64 |    0.00 |    4 |         - |          NA |
| DropMergeSort            | 8192 | ManyDuplicates     |   233,229.8 ns |  1,126.98 ns |    589.43 ns |  0.53 |    0.00 |    4 |         - |          NA |
| TimSort                  | 8192 | ManyDuplicates     |   370,909.6 ns |  1,387.58 ns |    725.73 ns |  0.85 |    0.00 |    4 |         - |          NA |
| PowerSort                | 8192 | ManyDuplicates     |   246,457.6 ns |  1,972.19 ns |  1,031.50 ns |  0.56 |    0.00 |    4 |         - |          NA |
| ShiftSort                | 8192 | ManyDuplicates     |   383,667.3 ns |  1,433.00 ns |    636.26 ns |  0.88 |    0.00 |    4 |         - |          NA |
| SpinSort                 | 8192 | ManyDuplicates     |   222,286.1 ns |  1,176.75 ns |    615.46 ns |  0.51 |    0.00 |    4 |         - |          NA |
| Glidesort                | 8192 | ManyDuplicates     |    34,884.2 ns |    279.18 ns |    123.96 ns |  0.08 |    0.00 |    2 |         - |          NA |
| Driftsort                | 8192 | ManyDuplicates     |    24,914.7 ns |    577.05 ns |    301.81 ns |  0.06 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | ManyDuplicates     |   247,564.0 ns |  1,637.28 ns |    856.33 ns |  0.57 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 8192 | ManyDuplicates     |   157,281.3 ns |  4,841.51 ns |  2,149.66 ns |  0.36 |    0.00 |    3 |         - |          NA |

### MergeSlowBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.52 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method     | Size | Pattern            | Mean         | Error       | StdDev      | Ratio | Rank | Allocated | Alloc Ratio |
| ----------- |----- |------------------- |-------------:|------------:|------------:|------:|-----:|----------:|------------:|
| **StrandSort** | **256**  | **Random**             |   **9,307.0 ns** |   **325.50 ns** |   **170.24 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **256**  | **SingleElementMoved** |     **711.3 ns** |     **9.07 ns** |     **4.03 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **256**  | **Sorted**             |     **490.8 ns** |    **12.37 ns** |     **6.47 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **256**  | **Reversed**           |  **38,815.9 ns** | **1,248.15 ns** |   **652.81 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **256**  | **PipeOrgan**          |  **23,226.6 ns** |   **558.11 ns** |   **291.90 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **256**  | **ManyDuplicates**     |   **7,902.7 ns** |   **262.33 ns** |   **116.48 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **1024** | **Random**             |  **58,166.2 ns** |   **446.45 ns** |   **198.23 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **1024** | **SingleElementMoved** |   **2,181.2 ns** |    **10.30 ns** |     **4.57 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **1024** | **Sorted**             |   **1,396.1 ns** |     **8.36 ns** |     **4.37 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **1024** | **Reversed**           | **619,845.4 ns** | **4,705.65 ns** | **2,461.14 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **1024** | **PipeOrgan**          | **371,142.5 ns** | **1,125.26 ns** |   **499.62 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **1024** | **ManyDuplicates**     |  **35,417.6 ns** |   **238.48 ns** |   **124.73 ns** |  **1.00** |    **1** |         **-** |          **NA** |

### NetworkBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.52 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                            | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ---------------------------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **BitonicSort**                       | **256**  | **Random**             |    **13,037.3 ns** |    **313.74 ns** |    **139.30 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 256  | Random             |     3,730.5 ns |    161.94 ns |     71.90 ns |  0.29 |    0.01 |    1 |         - |          NA |
| BitonicRecursiveSort              | 256  | Random             |    26,495.8 ns |    573.69 ns |    300.05 ns |  2.03 |    0.03 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 256  | Random             |    10,467.0 ns |    304.47 ns |    135.19 ns |  0.80 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 256  | Random             |     3,438.6 ns |    234.11 ns |    103.95 ns |  0.26 |    0.01 |    1 |         - |          NA |
| PairwiseSortingNetwork            | 256  | Random             |    12,136.0 ns |    269.48 ns |    140.94 ns |  0.93 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 256  | Random             |     3,311.9 ns |     31.32 ns |     13.91 ns |  0.25 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **256**  | **SingleElementMoved** |     **3,808.1 ns** |    **192.37 ns** |    **100.61 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicSortBranchless             | 256  | SingleElementMoved |     3,705.0 ns |     45.31 ns |     16.16 ns |  0.97 |    0.02 |    1 |         - |          NA |
| BitonicRecursiveSort              | 256  | SingleElementMoved |    18,127.7 ns |    191.87 ns |     85.19 ns |  4.76 |    0.12 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 256  | SingleElementMoved |     3,785.7 ns |    292.59 ns |    129.91 ns |  0.99 |    0.04 |    1 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 256  | SingleElementMoved |     3,384.2 ns |     23.35 ns |      8.33 ns |  0.89 |    0.02 |    1 |         - |          NA |
| PairwiseSortingNetwork            | 256  | SingleElementMoved |     2,830.2 ns |    208.45 ns |     92.55 ns |  0.74 |    0.03 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 256  | SingleElementMoved |     3,312.1 ns |     35.59 ns |     15.80 ns |  0.87 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **256**  | **Sorted**             |     **3,611.3 ns** |    **365.64 ns** |    **191.24 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 256  | Sorted             |     3,729.2 ns |     15.52 ns |      5.53 ns |  1.04 |    0.05 |    2 |         - |          NA |
| BitonicRecursiveSort              | 256  | Sorted             |    18,030.0 ns |    228.83 ns |    119.68 ns |  5.00 |    0.25 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 256  | Sorted             |     3,722.0 ns |     28.80 ns |     10.27 ns |  1.03 |    0.05 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 256  | Sorted             |     3,524.4 ns |    384.36 ns |    201.03 ns |  0.98 |    0.07 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 256  | Sorted             |     2,660.9 ns |     26.36 ns |     11.70 ns |  0.74 |    0.04 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 256  | Sorted             |     3,314.1 ns |     27.78 ns |     12.33 ns |  0.92 |    0.05 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **256**  | **Reversed**           |     **3,660.8 ns** |    **305.84 ns** |    **159.96 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 256  | Reversed           |     3,729.5 ns |     49.27 ns |     21.88 ns |  1.02 |    0.04 |    2 |         - |          NA |
| BitonicRecursiveSort              | 256  | Reversed           |    18,022.7 ns |    125.27 ns |     65.52 ns |  4.93 |    0.20 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 256  | Reversed           |     3,486.2 ns |    221.29 ns |    115.74 ns |  0.95 |    0.05 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 256  | Reversed           |     3,375.5 ns |     65.55 ns |     29.11 ns |  0.92 |    0.04 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 256  | Reversed           |     2,879.6 ns |    521.28 ns |    272.64 ns |  0.79 |    0.08 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 256  | Reversed           |     3,333.0 ns |     30.54 ns |     10.89 ns |  0.91 |    0.04 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **256**  | **PipeOrgan**          |     **3,750.2 ns** |    **264.36 ns** |    **117.38 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicSortBranchless             | 256  | PipeOrgan          |     3,737.8 ns |     55.06 ns |     24.45 ns |  1.00 |    0.03 |    1 |         - |          NA |
| BitonicRecursiveSort              | 256  | PipeOrgan          |    18,022.8 ns |    335.84 ns |    175.65 ns |  4.81 |    0.14 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 256  | PipeOrgan          |     3,402.3 ns |     80.94 ns |     28.87 ns |  0.91 |    0.03 |    1 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 256  | PipeOrgan          |     3,393.2 ns |     18.24 ns |      6.50 ns |  0.91 |    0.03 |    1 |         - |          NA |
| PairwiseSortingNetwork            | 256  | PipeOrgan          |     2,825.9 ns |    322.58 ns |    143.23 ns |  0.75 |    0.04 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 256  | PipeOrgan          |     3,335.8 ns |    139.41 ns |     61.90 ns |  0.89 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **256**  | **ManyDuplicates**     |    **11,454.6 ns** |    **460.90 ns** |    **241.06 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 256  | ManyDuplicates     |     3,694.6 ns |     19.40 ns |      6.92 ns |  0.32 |    0.01 |    1 |         - |          NA |
| BitonicRecursiveSort              | 256  | ManyDuplicates     |    25,704.8 ns |    289.23 ns |    151.27 ns |  2.24 |    0.05 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 256  | ManyDuplicates     |     9,981.1 ns |    384.01 ns |    200.85 ns |  0.87 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 256  | ManyDuplicates     |     3,380.9 ns |     36.12 ns |     12.88 ns |  0.30 |    0.01 |    1 |         - |          NA |
| PairwiseSortingNetwork            | 256  | ManyDuplicates     |    11,290.4 ns |    282.08 ns |    125.24 ns |  0.99 |    0.02 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 256  | ManyDuplicates     |     3,306.3 ns |     17.68 ns |      7.85 ns |  0.29 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **1024** | **Random**             |    **66,791.7 ns** |    **472.86 ns** |    **247.32 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 1024 | Random             |    22,701.8 ns |    195.41 ns |    102.20 ns |  0.34 |    0.00 |    1 |         - |          NA |
| BitonicRecursiveSort              | 1024 | Random             |   138,447.1 ns |    868.24 ns |    385.50 ns |  2.07 |    0.01 |    4 |         - |          NA |
| BatcherOddEvenMergeSort           | 1024 | Random             |    54,443.9 ns |    328.19 ns |    171.65 ns |  0.82 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 1024 | Random             |    20,728.8 ns |    194.86 ns |    101.91 ns |  0.31 |    0.00 |    1 |         - |          NA |
| PairwiseSortingNetwork            | 1024 | Random             |    66,268.1 ns |    254.91 ns |    113.18 ns |  0.99 |    0.00 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 1024 | Random             |    20,673.6 ns |    295.22 ns |    154.41 ns |  0.31 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **1024** | **SingleElementMoved** |    **21,639.7 ns** |    **319.47 ns** |    **141.85 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 1024 | SingleElementMoved |    22,720.3 ns |    339.02 ns |    150.53 ns |  1.05 |    0.01 |    2 |         - |          NA |
| BitonicRecursiveSort              | 1024 | SingleElementMoved |    93,457.1 ns |    286.06 ns |    127.01 ns |  4.32 |    0.03 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 1024 | SingleElementMoved |    22,828.9 ns |  1,536.09 ns |    803.40 ns |  1.05 |    0.04 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 1024 | SingleElementMoved |    20,661.3 ns |    179.03 ns |     93.64 ns |  0.95 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 1024 | SingleElementMoved |    16,568.8 ns |    931.70 ns |    487.30 ns |  0.77 |    0.02 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 1024 | SingleElementMoved |    20,457.7 ns |    110.35 ns |     49.00 ns |  0.95 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **1024** | **Sorted**             |    **20,821.3 ns** |    **197.15 ns** |     **87.54 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 1024 | Sorted             |    22,685.2 ns |    240.35 ns |    106.72 ns |  1.09 |    0.01 |    2 |         - |          NA |
| BitonicRecursiveSort              | 1024 | Sorted             |    93,618.5 ns |    179.82 ns |     94.05 ns |  4.50 |    0.02 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 1024 | Sorted             |    23,133.2 ns |    178.71 ns |     93.47 ns |  1.11 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 1024 | Sorted             |    20,541.7 ns |    221.39 ns |    115.79 ns |  0.99 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 1024 | Sorted             |    16,205.8 ns |    468.49 ns |    208.01 ns |  0.78 |    0.01 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 1024 | Sorted             |    20,440.2 ns |    197.59 ns |    103.34 ns |  0.98 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **1024** | **Reversed**           |    **21,245.4 ns** |    **209.85 ns** |     **93.17 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 1024 | Reversed           |    22,496.9 ns |     59.86 ns |     21.35 ns |  1.06 |    0.00 |    2 |         - |          NA |
| BitonicRecursiveSort              | 1024 | Reversed           |    93,293.6 ns |    632.80 ns |    330.97 ns |  4.39 |    0.02 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 1024 | Reversed           |    22,069.4 ns |  1,469.92 ns |    768.79 ns |  1.04 |    0.03 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 1024 | Reversed           |    20,673.6 ns |    431.32 ns |    225.59 ns |  0.97 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 1024 | Reversed           |    16,901.0 ns |  1,121.85 ns |    586.75 ns |  0.80 |    0.03 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 1024 | Reversed           |    20,495.4 ns |    167.34 ns |     87.52 ns |  0.96 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **1024** | **PipeOrgan**          |    **21,330.5 ns** |    **624.97 ns** |    **277.49 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 1024 | PipeOrgan          |    22,768.7 ns |    249.63 ns |    130.56 ns |  1.07 |    0.01 |    2 |         - |          NA |
| BitonicRecursiveSort              | 1024 | PipeOrgan          |    93,402.9 ns |    186.23 ns |     97.40 ns |  4.38 |    0.05 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 1024 | PipeOrgan          |    21,977.0 ns |    812.38 ns |    424.89 ns |  1.03 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 1024 | PipeOrgan          |    20,696.0 ns |    459.39 ns |    203.97 ns |  0.97 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 1024 | PipeOrgan          |    17,044.7 ns |  1,665.00 ns |    870.83 ns |  0.80 |    0.04 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 1024 | PipeOrgan          |    20,509.5 ns |    118.71 ns |     62.09 ns |  0.96 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **1024** | **ManyDuplicates**     |    **55,103.2 ns** |    **430.31 ns** |    **191.06 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 1024 | ManyDuplicates     |    22,591.7 ns |    244.55 ns |    127.91 ns |  0.41 |    0.00 |    1 |         - |          NA |
| BitonicRecursiveSort              | 1024 | ManyDuplicates     |   128,794.9 ns |  1,446.28 ns |    642.16 ns |  2.34 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 1024 | ManyDuplicates     |    48,619.0 ns |    495.47 ns |    259.14 ns |  0.88 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 1024 | ManyDuplicates     |    20,578.1 ns |    312.54 ns |    163.46 ns |  0.37 |    0.00 |    1 |         - |          NA |
| PairwiseSortingNetwork            | 1024 | ManyDuplicates     |    54,550.9 ns |    531.73 ns |    236.09 ns |  0.99 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 1024 | ManyDuplicates     |    20,515.0 ns |    203.80 ns |    106.59 ns |  0.37 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **4096** | **Random**             |   **330,247.7 ns** |  **1,753.17 ns** |    **778.42 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 4096 | Random             |   127,364.0 ns |  2,019.41 ns |  1,056.19 ns |  0.39 |    0.00 |    1 |         - |          NA |
| BitonicRecursiveSort              | 4096 | Random             |   693,332.2 ns |  2,848.76 ns |  1,264.87 ns |  2.10 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 4096 | Random             |   277,438.5 ns |  7,048.88 ns |  2,513.70 ns |  0.84 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 4096 | Random             |   123,801.0 ns | 34,011.90 ns | 15,101.50 ns |  0.37 |    0.04 |    1 |         - |          NA |
| PairwiseSortingNetwork            | 4096 | Random             |   349,530.5 ns |  1,144.59 ns |    598.65 ns |  1.06 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 4096 | Random             |   114,905.4 ns |    631.68 ns |    280.47 ns |  0.35 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **4096** | **SingleElementMoved** |   **117,474.9 ns** |    **541.94 ns** |    **240.62 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 4096 | SingleElementMoved |   126,380.9 ns |    627.55 ns |    278.64 ns |  1.08 |    0.00 |    2 |         - |          NA |
| BitonicRecursiveSort              | 4096 | SingleElementMoved |   465,548.0 ns |    737.80 ns |    263.11 ns |  3.96 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 4096 | SingleElementMoved |   129,009.2 ns |  3,295.64 ns |  1,463.28 ns |  1.10 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 4096 | SingleElementMoved |   116,125.7 ns |    787.98 ns |    349.87 ns |  0.99 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 4096 | SingleElementMoved |    96,276.7 ns |  6,398.06 ns |  2,840.78 ns |  0.82 |    0.02 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 4096 | SingleElementMoved |   115,569.9 ns |  2,037.77 ns |  1,065.79 ns |  0.98 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **4096** | **Sorted**             |   **116,691.5 ns** |  **1,244.98 ns** |    **552.78 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 4096 | Sorted             |   126,488.9 ns |    848.13 ns |    376.58 ns |  1.08 |    0.01 |    2 |         - |          NA |
| BitonicRecursiveSort              | 4096 | Sorted             |   466,488.2 ns |  2,440.46 ns |  1,276.41 ns |  4.00 |    0.02 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 4096 | Sorted             |   130,419.2 ns |  1,754.16 ns |    778.86 ns |  1.12 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 4096 | Sorted             |   116,517.2 ns |  1,500.68 ns |    784.89 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 4096 | Sorted             |    96,005.2 ns |  4,447.49 ns |  1,974.71 ns |  0.82 |    0.02 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 4096 | Sorted             |   115,681.6 ns |  1,574.13 ns |    823.30 ns |  0.99 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **4096** | **Reversed**           |   **119,403.7 ns** |  **1,325.12 ns** |    **588.36 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicSortBranchless             | 4096 | Reversed           |   126,467.1 ns |    494.21 ns |    176.24 ns |  1.06 |    0.01 |    1 |         - |          NA |
| BitonicRecursiveSort              | 4096 | Reversed           |   464,704.1 ns |  2,118.83 ns |  1,108.19 ns |  3.89 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 4096 | Reversed           |   123,571.8 ns |  4,452.69 ns |  2,328.84 ns |  1.03 |    0.02 |    1 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 4096 | Reversed           |   116,219.3 ns |    965.47 ns |    428.67 ns |  0.97 |    0.01 |    1 |         - |          NA |
| PairwiseSortingNetwork            | 4096 | Reversed           |    96,573.0 ns |  2,063.44 ns |    916.18 ns |  0.81 |    0.01 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 4096 | Reversed           |   115,456.6 ns |  1,756.17 ns |    779.75 ns |  0.97 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **4096** | **PipeOrgan**          |   **118,065.4 ns** |    **922.40 ns** |    **409.55 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicSortBranchless             | 4096 | PipeOrgan          |   126,776.0 ns |  1,564.27 ns |    818.14 ns |  1.07 |    0.01 |    1 |         - |          NA |
| BitonicRecursiveSort              | 4096 | PipeOrgan          |   464,604.9 ns |  1,190.91 ns |    528.77 ns |  3.94 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 4096 | PipeOrgan          |   125,013.9 ns |  3,394.27 ns |  1,775.27 ns |  1.06 |    0.01 |    1 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 4096 | PipeOrgan          |   115,910.0 ns |    855.44 ns |    379.82 ns |  0.98 |    0.00 |    1 |         - |          NA |
| PairwiseSortingNetwork            | 4096 | PipeOrgan          |    96,200.1 ns |  4,842.64 ns |  2,532.79 ns |  0.81 |    0.02 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 4096 | PipeOrgan          |   114,629.5 ns |    257.26 ns |    114.23 ns |  0.97 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **4096** | **ManyDuplicates**     |   **253,531.7 ns** |  **1,424.62 ns** |    **745.10 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 4096 | ManyDuplicates     |   126,925.4 ns |    767.74 ns |    340.88 ns |  0.50 |    0.00 |    1 |         - |          NA |
| BitonicRecursiveSort              | 4096 | ManyDuplicates     |   616,942.6 ns |  2,938.19 ns |  1,536.73 ns |  2.43 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 4096 | ManyDuplicates     |   230,826.7 ns |  1,160.02 ns |    515.06 ns |  0.91 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 4096 | ManyDuplicates     |   115,982.6 ns |  1,046.95 ns |    547.58 ns |  0.46 |    0.00 |    1 |         - |          NA |
| PairwiseSortingNetwork            | 4096 | ManyDuplicates     |   252,881.4 ns |  2,095.92 ns |  1,096.21 ns |  1.00 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 4096 | ManyDuplicates     |   115,063.0 ns |    933.86 ns |    414.64 ns |  0.45 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **8192** | **Random**             |   **732,841.5 ns** |  **2,855.16 ns** |  **1,267.71 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 8192 | Random             |   293,512.1 ns |  1,132.16 ns |    502.68 ns |  0.40 |    0.00 |    1 |         - |          NA |
| BitonicRecursiveSort              | 8192 | Random             | 1,552,070.3 ns |  2,349.02 ns |  1,042.98 ns |  2.12 |    0.00 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 8192 | Random             |   614,789.9 ns |  2,800.79 ns |  1,243.57 ns |  0.84 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 8192 | Random             |   271,450.8 ns |  2,538.34 ns |  1,327.60 ns |  0.37 |    0.00 |    1 |         - |          NA |
| PairwiseSortingNetwork            | 8192 | Random             |   786,730.9 ns | 22,777.44 ns | 10,113.33 ns |  1.07 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 8192 | Random             |   268,913.7 ns |  3,480.51 ns |  1,820.38 ns |  0.37 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **8192** | **SingleElementMoved** |   **270,129.1 ns** |  **2,673.51 ns** |  **1,187.06 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 8192 | SingleElementMoved |   293,817.1 ns |    935.45 ns |    415.34 ns |  1.09 |    0.00 |    2 |         - |          NA |
| BitonicRecursiveSort              | 8192 | SingleElementMoved | 1,049,891.1 ns |  2,689.85 ns |  1,406.84 ns |  3.89 |    0.02 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 8192 | SingleElementMoved |   298,927.6 ns |  4,130.02 ns |  2,160.08 ns |  1.11 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 8192 | SingleElementMoved |   270,674.2 ns |  1,960.02 ns |  1,025.13 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 8192 | SingleElementMoved |   223,321.8 ns |  3,536.65 ns |  1,849.73 ns |  0.83 |    0.01 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 8192 | SingleElementMoved |   269,465.7 ns |  5,515.56 ns |  2,884.74 ns |  1.00 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **8192** | **Sorted**             |   **269,571.5 ns** |  **1,751.05 ns** |    **915.83 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 8192 | Sorted             |   293,678.5 ns |  2,086.78 ns |    926.54 ns |  1.09 |    0.00 |    2 |         - |          NA |
| BitonicRecursiveSort              | 8192 | Sorted             | 1,049,965.6 ns |  2,118.34 ns |    940.55 ns |  3.89 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 8192 | Sorted             |   302,533.7 ns |  4,510.58 ns |  2,359.12 ns |  1.12 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 8192 | Sorted             |   272,729.6 ns |  3,755.24 ns |  1,964.06 ns |  1.01 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 8192 | Sorted             |   221,207.3 ns |  5,397.49 ns |  2,822.99 ns |  0.82 |    0.01 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 8192 | Sorted             |   268,035.4 ns |  1,663.55 ns |    738.63 ns |  0.99 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **8192** | **Reversed**           |   **275,712.4 ns** |  **1,319.00 ns** |    **689.86 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicSortBranchless             | 8192 | Reversed           |   294,360.2 ns |  2,139.06 ns |    949.76 ns |  1.07 |    0.00 |    1 |         - |          NA |
| BitonicRecursiveSort              | 8192 | Reversed           | 1,046,476.7 ns |  6,021.42 ns |  2,673.55 ns |  3.80 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 8192 | Reversed           |   289,502.0 ns |  6,403.10 ns |  3,348.95 ns |  1.05 |    0.01 |    1 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 8192 | Reversed           |   271,422.2 ns |  2,037.57 ns |    904.69 ns |  0.98 |    0.00 |    1 |         - |          NA |
| PairwiseSortingNetwork            | 8192 | Reversed           |   226,219.4 ns |  5,252.79 ns |  2,747.31 ns |  0.82 |    0.01 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 8192 | Reversed           |   268,207.6 ns |  2,910.79 ns |  1,292.41 ns |  0.97 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **8192** | **PipeOrgan**          |   **272,996.9 ns** |  **2,158.12 ns** |    **958.22 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 8192 | PipeOrgan          |   293,346.0 ns |    967.59 ns |    345.05 ns |  1.07 |    0.00 |    2 |         - |          NA |
| BitonicRecursiveSort              | 8192 | PipeOrgan          | 1,046,995.1 ns |  1,557.21 ns |    691.41 ns |  3.84 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 8192 | PipeOrgan          |   291,064.8 ns |  4,295.40 ns |  2,246.58 ns |  1.07 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 8192 | PipeOrgan          |   269,573.2 ns |  1,790.91 ns |    795.17 ns |  0.99 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 8192 | PipeOrgan          |   222,386.4 ns |  2,336.61 ns |  1,037.47 ns |  0.81 |    0.00 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 8192 | PipeOrgan          |   268,392.3 ns |  1,235.70 ns |    548.66 ns |  0.98 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **8192** | **ManyDuplicates**     |   **542,848.0 ns** |  **1,453.57 ns** |    **760.24 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 8192 | ManyDuplicates     |   293,780.0 ns |    316.83 ns |    140.67 ns |  0.54 |    0.00 |    1 |         - |          NA |
| BitonicRecursiveSort              | 8192 | ManyDuplicates     | 1,348,055.3 ns |  3,319.68 ns |  1,473.96 ns |  2.48 |    0.00 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 8192 | ManyDuplicates     |   502,979.5 ns |  1,934.57 ns |  1,011.82 ns |  0.93 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 8192 | ManyDuplicates     |   271,001.2 ns |  1,558.15 ns |    691.83 ns |  0.50 |    0.00 |    1 |         - |          NA |
| PairwiseSortingNetwork            | 8192 | ManyDuplicates     |   539,894.1 ns |  3,668.05 ns |  1,918.46 ns |  0.99 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 8192 | ManyDuplicates     |   268,140.7 ns |  1,703.74 ns |    891.09 ns |  0.49 |    0.00 |    1 |         - |          NA |

### PartitionBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.52 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                       | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **QuickSort**                    | **256**  | **Random**             |     **8,522.6 ns** |    **228.03 ns** |    **101.25 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 256  | Random             |     9,443.0 ns |    281.90 ns |    125.17 ns |  1.11 |    0.02 |    2 |         - |          NA |
| QuickSortMedian3             | 256  | Random             |     9,233.7 ns |    306.95 ns |    160.54 ns |  1.08 |    0.02 |    2 |         - |          NA |
| QuickSortMedian9             | 256  | Random             |     9,625.9 ns |    414.48 ns |    216.78 ns |  1.13 |    0.03 |    2 |         - |          NA |
| SampleSort                   | 256  | Random             |    11,084.1 ns |    344.69 ns |    180.28 ns |  1.30 |    0.02 |    2 |         - |          NA |
| DualPivotQuickSort           | 256  | Random             |     5,830.8 ns |    224.03 ns |    117.17 ns |  0.68 |    0.02 |    2 |         - |          NA |
| StableQuickSort              | 256  | Random             |    19,786.2 ns |    255.93 ns |    133.86 ns |  2.32 |    0.03 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Random             |    11,568.8 ns |    251.55 ns |    131.57 ns |  1.36 |    0.02 |    2 |         - |          NA |
| DestswapStableQuickSort      | 256  | Random             |    19,122.7 ns |    208.00 ns |     92.35 ns |  2.24 |    0.03 |    3 |         - |          NA |
| IntroSort                    | 256  | Random             |     7,151.1 ns |    129.22 ns |     57.37 ns |  0.84 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 256  | Random             |     6,329.2 ns |     84.61 ns |     37.57 ns |  0.74 |    0.01 |    2 |         - |          NA |
| PDQSort                      | 256  | Random             |     5,989.6 ns |    276.49 ns |    144.61 ns |  0.70 |    0.02 |    2 |         - |          NA |
| PDQSortBranchless            | 256  | Random             |     4,385.2 ns |    285.45 ns |    149.30 ns |  0.51 |    0.02 |    1 |         - |          NA |
| Ipnsort                      | 256  | Random             |     3,816.2 ns |    305.46 ns |    159.76 ns |  0.45 |    0.02 |    1 |         - |          NA |
| StdSort                      | 256  | Random             |     4,401.7 ns |    281.65 ns |    147.31 ns |  0.52 |    0.02 |    1 |         - |          NA |
| BlockQuickSort               | 256  | Random             |     4,856.0 ns |    517.26 ns |    270.54 ns |  0.57 |    0.03 |    1 |         - |          NA |
| FluxSort                     | 256  | Random             |     5,985.2 ns |    108.20 ns |     48.04 ns |  0.70 |    0.01 |    2 |         - |          NA |
| DotnetSort                   | 256  | Random             |     5,812.3 ns |    186.66 ns |     97.63 ns |  0.68 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **SingleElementMoved** |     **1,100.8 ns** |    **103.26 ns** |     **54.01 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 256  | SingleElementMoved |     2,319.8 ns |    427.74 ns |    223.72 ns |  2.11 |    0.21 |    3 |         - |          NA |
| QuickSortMedian3             | 256  | SingleElementMoved |     1,763.7 ns |    209.95 ns |    109.81 ns |  1.61 |    0.12 |    2 |         - |          NA |
| QuickSortMedian9             | 256  | SingleElementMoved |     1,722.0 ns |    171.93 ns |     89.92 ns |  1.57 |    0.10 |    2 |         - |          NA |
| SampleSort                   | 256  | SingleElementMoved |     5,510.7 ns |    589.00 ns |    308.06 ns |  5.02 |    0.35 |    6 |         - |          NA |
| DualPivotQuickSort           | 256  | SingleElementMoved |       853.6 ns |     58.87 ns |     30.79 ns |  0.78 |    0.04 |    1 |         - |          NA |
| StableQuickSort              | 256  | SingleElementMoved |     6,203.1 ns |    196.31 ns |     87.16 ns |  5.65 |    0.26 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 256  | SingleElementMoved |     3,620.7 ns |    167.91 ns |     74.56 ns |  3.30 |    0.16 |    5 |         - |          NA |
| DestswapStableQuickSort      | 256  | SingleElementMoved |     7,355.4 ns |     82.93 ns |     36.82 ns |  6.70 |    0.30 |    6 |         - |          NA |
| IntroSort                    | 256  | SingleElementMoved |     2,174.4 ns |    226.84 ns |    118.64 ns |  1.98 |    0.13 |    3 |         - |          NA |
| IntroSortDotnet              | 256  | SingleElementMoved |     1,153.5 ns |    407.96 ns |    181.14 ns |  1.05 |    0.16 |    1 |         - |          NA |
| PDQSort                      | 256  | SingleElementMoved |       860.6 ns |     79.36 ns |     35.23 ns |  0.78 |    0.05 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | SingleElementMoved |     1,043.6 ns |     96.01 ns |     50.21 ns |  0.95 |    0.06 |    1 |         - |          NA |
| Ipnsort                      | 256  | SingleElementMoved |     3,033.9 ns |    342.87 ns |    179.33 ns |  2.76 |    0.20 |    4 |         - |          NA |
| StdSort                      | 256  | SingleElementMoved |       978.1 ns |    102.98 ns |     53.86 ns |  0.89 |    0.06 |    1 |         - |          NA |
| BlockQuickSort               | 256  | SingleElementMoved |     1,556.1 ns |     65.14 ns |     28.92 ns |  1.42 |    0.07 |    2 |         - |          NA |
| FluxSort                     | 256  | SingleElementMoved |     1,236.0 ns |     76.33 ns |     33.89 ns |  1.13 |    0.06 |    1 |         - |          NA |
| DotnetSort                   | 256  | SingleElementMoved |     1,079.0 ns |    123.80 ns |     64.75 ns |  0.98 |    0.07 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Sorted**             |       **697.0 ns** |     **21.99 ns** |     **11.50 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 256  | Sorted             |     1,416.4 ns |     43.30 ns |     15.44 ns |  2.03 |    0.04 |    5 |         - |          NA |
| QuickSortMedian3             | 256  | Sorted             |       919.0 ns |     10.20 ns |      4.53 ns |  1.32 |    0.02 |    4 |         - |          NA |
| QuickSortMedian9             | 256  | Sorted             |     1,031.8 ns |     34.13 ns |     15.16 ns |  1.48 |    0.03 |    4 |         - |          NA |
| SampleSort                   | 256  | Sorted             |     3,614.1 ns |    280.97 ns |    146.95 ns |  5.19 |    0.21 |    6 |         - |          NA |
| DualPivotQuickSort           | 256  | Sorted             |       628.6 ns |     50.17 ns |     26.24 ns |  0.90 |    0.04 |    4 |         - |          NA |
| StableQuickSort              | 256  | Sorted             |     6,279.5 ns |    142.60 ns |     74.58 ns |  9.01 |    0.17 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Sorted             |     3,481.6 ns |    122.01 ns |     54.17 ns |  5.00 |    0.11 |    6 |         - |          NA |
| DestswapStableQuickSort      | 256  | Sorted             |     7,233.3 ns |     69.94 ns |     36.58 ns | 10.38 |    0.17 |    7 |         - |          NA |
| IntroSort                    | 256  | Sorted             |       656.5 ns |     43.24 ns |     19.20 ns |  0.94 |    0.03 |    4 |         - |          NA |
| IntroSortDotnet              | 256  | Sorted             |       768.3 ns |     12.97 ns |      5.76 ns |  1.10 |    0.02 |    4 |         - |          NA |
| PDQSort                      | 256  | Sorted             |       333.3 ns |     20.82 ns |     10.89 ns |  0.48 |    0.02 |    3 |         - |          NA |
| PDQSortBranchless            | 256  | Sorted             |       338.8 ns |     25.65 ns |     13.41 ns |  0.49 |    0.02 |    3 |         - |          NA |
| Ipnsort                      | 256  | Sorted             |       153.1 ns |      5.47 ns |      2.86 ns |  0.22 |    0.01 |    1 |         - |          NA |
| StdSort                      | 256  | Sorted             |       338.2 ns |      8.33 ns |      3.70 ns |  0.49 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 256  | Sorted             |     1,265.9 ns |     58.95 ns |     26.18 ns |  1.82 |    0.04 |    5 |         - |          NA |
| FluxSort                     | 256  | Sorted             |       219.5 ns |     16.03 ns |      8.38 ns |  0.32 |    0.01 |    2 |         - |          NA |
| DotnetSort                   | 256  | Sorted             |       649.2 ns |     15.87 ns |      7.05 ns |  0.93 |    0.02 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Reversed**           |       **801.8 ns** |     **22.70 ns** |     **10.08 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 256  | Reversed           |     2,044.4 ns |  1,126.34 ns |    589.10 ns |  2.55 |    0.69 |    6 |         - |          NA |
| QuickSortMedian3             | 256  | Reversed           |     1,016.9 ns |     14.65 ns |      7.66 ns |  1.27 |    0.02 |    5 |         - |          NA |
| QuickSortMedian9             | 256  | Reversed           |     1,123.5 ns |     38.05 ns |     19.90 ns |  1.40 |    0.03 |    5 |         - |          NA |
| SampleSort                   | 256  | Reversed           |     3,952.2 ns |    395.11 ns |    206.65 ns |  4.93 |    0.25 |    7 |         - |          NA |
| DualPivotQuickSort           | 256  | Reversed           |       726.1 ns |     18.33 ns |      8.14 ns |  0.91 |    0.01 |    4 |         - |          NA |
| StableQuickSort              | 256  | Reversed           |     6,010.3 ns |    217.63 ns |     96.63 ns |  7.50 |    0.14 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Reversed           |     3,446.2 ns |     47.75 ns |     17.03 ns |  4.30 |    0.05 |    7 |         - |          NA |
| DestswapStableQuickSort      | 256  | Reversed           |     6,564.1 ns |     93.64 ns |     41.58 ns |  8.19 |    0.11 |    8 |         - |          NA |
| IntroSort                    | 256  | Reversed           |       688.2 ns |     13.08 ns |      5.81 ns |  0.86 |    0.01 |    4 |         - |          NA |
| IntroSortDotnet              | 256  | Reversed           |     1,102.0 ns |     48.26 ns |     21.43 ns |  1.37 |    0.03 |    5 |         - |          NA |
| PDQSort                      | 256  | Reversed           |       451.7 ns |      8.97 ns |      3.20 ns |  0.56 |    0.01 |    3 |         - |          NA |
| PDQSortBranchless            | 256  | Reversed           |       609.0 ns |     90.48 ns |     47.32 ns |  0.76 |    0.06 |    4 |         - |          NA |
| Ipnsort                      | 256  | Reversed           |       190.7 ns |      9.13 ns |      4.05 ns |  0.24 |    0.01 |    1 |         - |          NA |
| StdSort                      | 256  | Reversed           |       671.4 ns |     29.28 ns |     13.00 ns |  0.84 |    0.02 |    4 |         - |          NA |
| BlockQuickSort               | 256  | Reversed           |     1,689.4 ns |     48.18 ns |     21.39 ns |  2.11 |    0.04 |    6 |         - |          NA |
| FluxSort                     | 256  | Reversed           |       280.7 ns |     28.38 ns |     12.60 ns |  0.35 |    0.02 |    2 |         - |          NA |
| DotnetSort                   | 256  | Reversed           |       989.6 ns |     40.85 ns |     18.14 ns |  1.23 |    0.03 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **PipeOrgan**          |     **5,985.2 ns** |    **349.64 ns** |    **182.87 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 256  | PipeOrgan          |     2,894.9 ns |    379.59 ns |    198.53 ns |  0.48 |    0.03 |    3 |         - |          NA |
| QuickSortMedian3             | 256  | PipeOrgan          |     2,567.1 ns |    262.41 ns |    137.24 ns |  0.43 |    0.02 |    3 |         - |          NA |
| QuickSortMedian9             | 256  | PipeOrgan          |     2,050.6 ns |    199.08 ns |    104.13 ns |  0.34 |    0.02 |    2 |         - |          NA |
| SampleSort                   | 256  | PipeOrgan          |     3,819.8 ns |    328.94 ns |    172.04 ns |  0.64 |    0.03 |    3 |         - |          NA |
| DualPivotQuickSort           | 256  | PipeOrgan          |     1,543.0 ns |     15.20 ns |      5.42 ns |  0.26 |    0.01 |    2 |         - |          NA |
| StableQuickSort              | 256  | PipeOrgan          |     6,048.1 ns |    402.85 ns |    210.70 ns |  1.01 |    0.04 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 256  | PipeOrgan          |     3,458.7 ns |     34.39 ns |     12.26 ns |  0.58 |    0.02 |    3 |         - |          NA |
| DestswapStableQuickSort      | 256  | PipeOrgan          |     8,754.7 ns |     31.64 ns |     11.28 ns |  1.46 |    0.04 |    5 |         - |          NA |
| IntroSort                    | 256  | PipeOrgan          |     3,131.8 ns |     27.51 ns |     12.22 ns |  0.52 |    0.02 |    3 |         - |          NA |
| IntroSortDotnet              | 256  | PipeOrgan          |     1,654.6 ns |     28.15 ns |     10.04 ns |  0.28 |    0.01 |    2 |         - |          NA |
| PDQSort                      | 256  | PipeOrgan          |     1,221.6 ns |     27.50 ns |     12.21 ns |  0.20 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | PipeOrgan          |     1,996.1 ns |     46.86 ns |     20.81 ns |  0.33 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 256  | PipeOrgan          |     3,086.2 ns |    150.41 ns |     66.78 ns |  0.52 |    0.02 |    3 |         - |          NA |
| StdSort                      | 256  | PipeOrgan          |     3,698.1 ns |    198.31 ns |     88.05 ns |  0.62 |    0.02 |    3 |         - |          NA |
| BlockQuickSort               | 256  | PipeOrgan          |     3,622.9 ns |     56.45 ns |     20.13 ns |  0.61 |    0.02 |    3 |         - |          NA |
| FluxSort                     | 256  | PipeOrgan          |     3,002.2 ns |     21.58 ns |      7.70 ns |  0.50 |    0.01 |    3 |         - |          NA |
| DotnetSort                   | 256  | PipeOrgan          |     1,793.5 ns |     38.12 ns |     13.59 ns |  0.30 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **256**  | **ManyDuplicates**     |     **6,907.0 ns** |    **209.12 ns** |     **92.85 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 256  | ManyDuplicates     |     5,866.7 ns |    130.81 ns |     58.08 ns |  0.85 |    0.01 |    2 |         - |          NA |
| QuickSortMedian3             | 256  | ManyDuplicates     |     7,756.2 ns |    455.02 ns |    237.98 ns |  1.12 |    0.04 |    2 |         - |          NA |
| QuickSortMedian9             | 256  | ManyDuplicates     |     8,001.7 ns |    262.86 ns |    116.71 ns |  1.16 |    0.02 |    2 |         - |          NA |
| SampleSort                   | 256  | ManyDuplicates     |     9,750.5 ns |    252.97 ns |    112.32 ns |  1.41 |    0.02 |    3 |         - |          NA |
| DualPivotQuickSort           | 256  | ManyDuplicates     |     5,319.8 ns |    393.19 ns |    205.65 ns |  0.77 |    0.03 |    2 |         - |          NA |
| StableQuickSort              | 256  | ManyDuplicates     |    12,670.3 ns |    440.07 ns |    230.16 ns |  1.83 |    0.04 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 256  | ManyDuplicates     |     6,217.7 ns |     66.29 ns |     29.43 ns |  0.90 |    0.01 |    2 |         - |          NA |
| DestswapStableQuickSort      | 256  | ManyDuplicates     |    10,847.6 ns |    361.35 ns |    160.44 ns |  1.57 |    0.03 |    3 |         - |          NA |
| IntroSort                    | 256  | ManyDuplicates     |     6,304.8 ns |    387.50 ns |    202.67 ns |  0.91 |    0.03 |    2 |         - |          NA |
| IntroSortDotnet              | 256  | ManyDuplicates     |     6,095.2 ns |    171.56 ns |     76.17 ns |  0.88 |    0.02 |    2 |         - |          NA |
| PDQSort                      | 256  | ManyDuplicates     |     5,516.4 ns |    345.73 ns |    180.82 ns |  0.80 |    0.03 |    2 |         - |          NA |
| PDQSortBranchless            | 256  | ManyDuplicates     |     3,794.8 ns |    349.39 ns |    182.74 ns |  0.55 |    0.03 |    1 |         - |          NA |
| Ipnsort                      | 256  | ManyDuplicates     |     3,640.8 ns |    339.98 ns |    177.82 ns |  0.53 |    0.03 |    1 |         - |          NA |
| StdSort                      | 256  | ManyDuplicates     |     4,129.3 ns |    323.79 ns |    169.35 ns |  0.60 |    0.02 |    1 |         - |          NA |
| BlockQuickSort               | 256  | ManyDuplicates     |     4,350.9 ns |    496.54 ns |    259.70 ns |  0.63 |    0.04 |    1 |         - |          NA |
| FluxSort                     | 256  | ManyDuplicates     |     5,996.2 ns |    193.11 ns |    101.00 ns |  0.87 |    0.02 |    2 |         - |          NA |
| DotnetSort                   | 256  | ManyDuplicates     |     5,589.5 ns |    455.90 ns |    238.44 ns |  0.81 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Random**             |    **40,601.9 ns** |    **393.95 ns** |    **206.04 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Random             |    44,150.0 ns |    316.17 ns |    165.36 ns |  1.09 |    0.01 |    1 |         - |          NA |
| QuickSortMedian3             | 1024 | Random             |    42,837.3 ns |    494.71 ns |    219.65 ns |  1.06 |    0.01 |    1 |         - |          NA |
| QuickSortMedian9             | 1024 | Random             |    44,993.0 ns |    181.45 ns |     80.56 ns |  1.11 |    0.01 |    1 |         - |          NA |
| SampleSort                   | 1024 | Random             |    51,483.2 ns |    320.08 ns |    142.12 ns |  1.27 |    0.01 |    1 |         - |          NA |
| DualPivotQuickSort           | 1024 | Random             |    30,024.2 ns |    215.59 ns |    112.76 ns |  0.74 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 1024 | Random             |    98,159.3 ns |    527.08 ns |    234.03 ns |  2.42 |    0.01 |    2 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Random             |    53,838.0 ns |    410.99 ns |    214.96 ns |  1.33 |    0.01 |    1 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Random             |    87,414.7 ns |  1,125.00 ns |    499.51 ns |  2.15 |    0.02 |    2 |         - |          NA |
| IntroSort                    | 1024 | Random             |    34,210.6 ns |    379.89 ns |    198.69 ns |  0.84 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | Random             |    32,416.9 ns |    153.43 ns |     68.12 ns |  0.80 |    0.00 |    1 |         - |          NA |
| PDQSort                      | 1024 | Random             |    37,421.0 ns | 12,746.09 ns |  6,666.45 ns |  0.92 |    0.15 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | Random             |    19,500.2 ns |    264.33 ns |    138.25 ns |  0.48 |    0.00 |    1 |         - |          NA |
| Ipnsort                      | 1024 | Random             |    19,060.3 ns |    212.91 ns |     75.93 ns |  0.47 |    0.00 |    1 |         - |          NA |
| StdSort                      | 1024 | Random             |    20,277.7 ns |    131.00 ns |     58.17 ns |  0.50 |    0.00 |    1 |         - |          NA |
| BlockQuickSort               | 1024 | Random             |    21,857.7 ns |    366.22 ns |    191.54 ns |  0.54 |    0.01 |    1 |         - |          NA |
| FluxSort                     | 1024 | Random             |    25,695.4 ns |    284.88 ns |    149.00 ns |  0.63 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 1024 | Random             |    29,937.9 ns |    499.34 ns |    261.16 ns |  0.74 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **SingleElementMoved** |     **4,331.7 ns** |    **102.79 ns** |     **53.76 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 1024 | SingleElementMoved |     8,396.1 ns |    592.60 ns |    309.94 ns |  1.94 |    0.07 |    3 |         - |          NA |
| QuickSortMedian3             | 1024 | SingleElementMoved |     6,611.1 ns |    538.73 ns |    239.20 ns |  1.53 |    0.05 |    3 |         - |          NA |
| QuickSortMedian9             | 1024 | SingleElementMoved |     7,306.0 ns |    309.36 ns |    161.80 ns |  1.69 |    0.04 |    3 |         - |          NA |
| SampleSort                   | 1024 | SingleElementMoved |    25,768.6 ns |    796.85 ns |    353.80 ns |  5.95 |    0.10 |    6 |         - |          NA |
| DualPivotQuickSort           | 1024 | SingleElementMoved |     3,614.5 ns |    145.68 ns |     64.68 ns |  0.83 |    0.02 |    2 |         - |          NA |
| StableQuickSort              | 1024 | SingleElementMoved |    30,768.3 ns |    559.95 ns |    248.62 ns |  7.10 |    0.10 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | SingleElementMoved |    16,218.1 ns |    200.56 ns |    104.90 ns |  3.74 |    0.05 |    5 |         - |          NA |
| DestswapStableQuickSort      | 1024 | SingleElementMoved |    32,578.1 ns |    370.51 ns |    164.51 ns |  7.52 |    0.10 |    6 |         - |          NA |
| IntroSort                    | 1024 | SingleElementMoved |    11,097.3 ns |    278.12 ns |    145.46 ns |  2.56 |    0.04 |    4 |         - |          NA |
| IntroSortDotnet              | 1024 | SingleElementMoved |     4,921.2 ns |    346.30 ns |    181.12 ns |  1.14 |    0.04 |    2 |         - |          NA |
| PDQSort                      | 1024 | SingleElementMoved |     3,111.4 ns |    105.03 ns |     46.64 ns |  0.72 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | SingleElementMoved |     4,034.9 ns |    255.72 ns |    113.54 ns |  0.93 |    0.03 |    2 |         - |          NA |
| Ipnsort                      | 1024 | SingleElementMoved |    14,660.8 ns |    181.94 ns |     95.16 ns |  3.38 |    0.05 |    5 |         - |          NA |
| StdSort                      | 1024 | SingleElementMoved |     3,521.6 ns |    328.29 ns |    171.70 ns |  0.81 |    0.04 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | SingleElementMoved |     7,399.5 ns |    427.39 ns |    223.53 ns |  1.71 |    0.05 |    3 |         - |          NA |
| FluxSort                     | 1024 | SingleElementMoved |     1,410.0 ns |    140.11 ns |     62.21 ns |  0.33 |    0.01 |    1 |         - |          NA |
| DotnetSort                   | 1024 | SingleElementMoved |     5,164.6 ns |    141.32 ns |     62.75 ns |  1.19 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Sorted**             |     **3,312.0 ns** |     **19.60 ns** |     **10.25 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Sorted             |     6,271.3 ns |    341.73 ns |    178.73 ns |  1.89 |    0.05 |    5 |         - |          NA |
| QuickSortMedian3             | 1024 | Sorted             |     4,307.8 ns |     21.47 ns |      9.53 ns |  1.30 |    0.00 |    4 |         - |          NA |
| QuickSortMedian9             | 1024 | Sorted             |     4,575.2 ns |     20.53 ns |      9.12 ns |  1.38 |    0.00 |    4 |         - |          NA |
| SampleSort                   | 1024 | Sorted             |    15,563.3 ns |    565.50 ns |    295.77 ns |  4.70 |    0.09 |    6 |         - |          NA |
| DualPivotQuickSort           | 1024 | Sorted             |     3,275.9 ns |    256.69 ns |    134.25 ns |  0.99 |    0.04 |    4 |         - |          NA |
| StableQuickSort              | 1024 | Sorted             |    30,832.3 ns |    810.80 ns |    360.00 ns |  9.31 |    0.11 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Sorted             |    15,795.7 ns |     56.12 ns |     29.35 ns |  4.77 |    0.02 |    6 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Sorted             |    31,849.3 ns |    401.20 ns |    209.84 ns |  9.62 |    0.07 |    7 |         - |          NA |
| IntroSort                    | 1024 | Sorted             |     3,098.5 ns |     39.26 ns |     17.43 ns |  0.94 |    0.01 |    4 |         - |          NA |
| IntroSortDotnet              | 1024 | Sorted             |     3,700.8 ns |    254.86 ns |    113.16 ns |  1.12 |    0.03 |    4 |         - |          NA |
| PDQSort                      | 1024 | Sorted             |     1,180.9 ns |     33.31 ns |     14.79 ns |  0.36 |    0.00 |    3 |         - |          NA |
| PDQSortBranchless            | 1024 | Sorted             |     1,190.1 ns |     10.11 ns |      4.49 ns |  0.36 |    0.00 |    3 |         - |          NA |
| Ipnsort                      | 1024 | Sorted             |       487.4 ns |      9.45 ns |      4.20 ns |  0.15 |    0.00 |    1 |         - |          NA |
| StdSort                      | 1024 | Sorted             |     1,221.2 ns |      7.31 ns |      3.24 ns |  0.37 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 1024 | Sorted             |     6,389.1 ns |    303.36 ns |    158.66 ns |  1.93 |    0.05 |    5 |         - |          NA |
| FluxSort                     | 1024 | Sorted             |       601.3 ns |      9.06 ns |      3.23 ns |  0.18 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 1024 | Sorted             |     3,141.2 ns |     33.20 ns |     17.36 ns |  0.95 |    0.01 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Reversed**           |     **3,813.9 ns** |    **279.97 ns** |    **146.43 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Reversed           |     6,995.1 ns |    212.19 ns |    110.98 ns |  1.84 |    0.07 |    7 |         - |          NA |
| QuickSortMedian3             | 1024 | Reversed           |     4,773.8 ns |    241.43 ns |    126.27 ns |  1.25 |    0.05 |    6 |         - |          NA |
| QuickSortMedian9             | 1024 | Reversed           |     5,184.0 ns |    177.19 ns |     92.67 ns |  1.36 |    0.05 |    6 |         - |          NA |
| SampleSort                   | 1024 | Reversed           |    16,618.3 ns |    658.56 ns |    344.44 ns |  4.36 |    0.18 |    8 |         - |          NA |
| DualPivotQuickSort           | 1024 | Reversed           |     3,605.6 ns |     26.03 ns |      9.28 ns |  0.95 |    0.03 |    5 |         - |          NA |
| StableQuickSort              | 1024 | Reversed           |    31,171.9 ns |    640.60 ns |    284.43 ns |  8.18 |    0.30 |    9 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Reversed           |    15,821.3 ns |     54.04 ns |     28.26 ns |  4.15 |    0.15 |    8 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Reversed           |    30,511.5 ns |    671.68 ns |    351.30 ns |  8.01 |    0.30 |    9 |         - |          NA |
| IntroSort                    | 1024 | Reversed           |     3,297.6 ns |    120.63 ns |     53.56 ns |  0.87 |    0.03 |    5 |         - |          NA |
| IntroSortDotnet              | 1024 | Reversed           |     5,669.7 ns |    265.79 ns |    139.01 ns |  1.49 |    0.06 |    6 |         - |          NA |
| PDQSort                      | 1024 | Reversed           |     1,612.4 ns |     12.72 ns |      5.65 ns |  0.42 |    0.02 |    3 |         - |          NA |
| PDQSortBranchless            | 1024 | Reversed           |     2,039.8 ns |     55.42 ns |     24.61 ns |  0.54 |    0.02 |    4 |         - |          NA |
| Ipnsort                      | 1024 | Reversed           |       740.2 ns |     12.16 ns |      5.40 ns |  0.19 |    0.01 |    1 |         - |          NA |
| StdSort                      | 1024 | Reversed           |     2,383.4 ns |     15.12 ns |      6.71 ns |  0.63 |    0.02 |    4 |         - |          NA |
| BlockQuickSort               | 1024 | Reversed           |     8,393.3 ns |    288.44 ns |    150.86 ns |  2.20 |    0.09 |    7 |         - |          NA |
| FluxSort                     | 1024 | Reversed           |       911.4 ns |     22.16 ns |      9.84 ns |  0.24 |    0.01 |    2 |         - |          NA |
| DotnetSort                   | 1024 | Reversed           |     5,273.1 ns |    156.82 ns |     55.92 ns |  1.38 |    0.05 |    6 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **PipeOrgan**          |    **83,820.5 ns** |    **915.54 ns** |    **406.51 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| QuickSort3way                | 1024 | PipeOrgan          |    15,455.0 ns |  1,022.91 ns |    535.00 ns |  0.18 |    0.01 |    3 |         - |          NA |
| QuickSortMedian3             | 1024 | PipeOrgan          |    12,686.8 ns |    231.90 ns |    102.97 ns |  0.15 |    0.00 |    3 |         - |          NA |
| QuickSortMedian9             | 1024 | PipeOrgan          |     9,391.1 ns |    345.24 ns |    180.57 ns |  0.11 |    0.00 |    3 |         - |          NA |
| SampleSort                   | 1024 | PipeOrgan          |    16,966.3 ns |    869.92 ns |    454.99 ns |  0.20 |    0.01 |    3 |         - |          NA |
| DualPivotQuickSort           | 1024 | PipeOrgan          |     7,784.8 ns |    384.34 ns |    201.02 ns |  0.09 |    0.00 |    2 |         - |          NA |
| StableQuickSort              | 1024 | PipeOrgan          |    30,843.9 ns |    544.01 ns |    284.53 ns |  0.37 |    0.00 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | PipeOrgan          |    15,951.7 ns |    193.55 ns |    101.23 ns |  0.19 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 1024 | PipeOrgan          |    40,414.7 ns |    546.81 ns |    285.99 ns |  0.48 |    0.00 |    5 |         - |          NA |
| IntroSort                    | 1024 | PipeOrgan          |    20,165.9 ns |    292.95 ns |    153.22 ns |  0.24 |    0.00 |    3 |         - |          NA |
| IntroSortDotnet              | 1024 | PipeOrgan          |    10,951.4 ns |    265.53 ns |    117.90 ns |  0.13 |    0.00 |    3 |         - |          NA |
| PDQSort                      | 1024 | PipeOrgan          |     6,574.4 ns |    311.33 ns |    162.83 ns |  0.08 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | PipeOrgan          |    10,437.9 ns |    252.05 ns |    131.83 ns |  0.12 |    0.00 |    3 |         - |          NA |
| Ipnsort                      | 1024 | PipeOrgan          |    16,702.9 ns |     98.82 ns |     43.88 ns |  0.20 |    0.00 |    3 |         - |          NA |
| StdSort                      | 1024 | PipeOrgan          |    18,119.3 ns |    214.52 ns |    112.20 ns |  0.22 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 1024 | PipeOrgan          |    17,910.0 ns |    259.40 ns |    135.67 ns |  0.21 |    0.00 |    3 |         - |          NA |
| FluxSort                     | 1024 | PipeOrgan          |     2,260.1 ns |     10.89 ns |      4.83 ns |  0.03 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 1024 | PipeOrgan          |    12,890.7 ns |    455.39 ns |    202.20 ns |  0.15 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **1024** | **ManyDuplicates**     |    **26,567.7 ns** |    **300.37 ns** |    **157.10 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 1024 | ManyDuplicates     |    20,268.2 ns |    180.17 ns |     79.99 ns |  0.76 |    0.01 |    3 |         - |          NA |
| QuickSortMedian3             | 1024 | ManyDuplicates     |    29,920.8 ns |    215.68 ns |    112.80 ns |  1.13 |    0.01 |    3 |         - |          NA |
| QuickSortMedian9             | 1024 | ManyDuplicates     |    31,340.7 ns |    182.34 ns |     80.96 ns |  1.18 |    0.01 |    3 |         - |          NA |
| SampleSort                   | 1024 | ManyDuplicates     |    39,165.5 ns |    734.39 ns |    384.10 ns |  1.47 |    0.02 |    3 |         - |          NA |
| DualPivotQuickSort           | 1024 | ManyDuplicates     |    19,377.6 ns |    217.14 ns |     77.43 ns |  0.73 |    0.00 |    3 |         - |          NA |
| StableQuickSort              | 1024 | ManyDuplicates     |    46,222.4 ns |  1,133.91 ns |    593.06 ns |  1.74 |    0.02 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | ManyDuplicates     |    20,841.2 ns |    162.27 ns |     84.87 ns |  0.78 |    0.01 |    3 |         - |          NA |
| DestswapStableQuickSort      | 1024 | ManyDuplicates     |    35,286.8 ns |    212.22 ns |    111.00 ns |  1.33 |    0.01 |    3 |         - |          NA |
| IntroSort                    | 1024 | ManyDuplicates     |    24,875.7 ns |    284.09 ns |    148.59 ns |  0.94 |    0.01 |    3 |         - |          NA |
| IntroSortDotnet              | 1024 | ManyDuplicates     |    25,369.9 ns |    222.33 ns |    116.28 ns |  0.95 |    0.01 |    3 |         - |          NA |
| PDQSort                      | 1024 | ManyDuplicates     |    20,472.0 ns |    133.42 ns |     69.78 ns |  0.77 |    0.00 |    3 |         - |          NA |
| PDQSortBranchless            | 1024 | ManyDuplicates     |     9,642.2 ns |    403.56 ns |    211.07 ns |  0.36 |    0.01 |    1 |         - |          NA |
| Ipnsort                      | 1024 | ManyDuplicates     |    15,228.2 ns |    295.61 ns |    154.61 ns |  0.57 |    0.01 |    2 |         - |          NA |
| StdSort                      | 1024 | ManyDuplicates     |    15,291.3 ns |    790.42 ns |    350.95 ns |  0.58 |    0.01 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | ManyDuplicates     |    16,094.3 ns |    337.70 ns |    149.94 ns |  0.61 |    0.01 |    2 |         - |          NA |
| FluxSort                     | 1024 | ManyDuplicates     |    24,772.6 ns |    270.94 ns |    141.70 ns |  0.93 |    0.01 |    3 |         - |          NA |
| DotnetSort                   | 1024 | ManyDuplicates     |    23,190.0 ns |    255.60 ns |    113.49 ns |  0.87 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Random**             |   **185,388.1 ns** |  **1,422.29 ns** |    **631.50 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Random             |   202,653.8 ns |  2,235.45 ns |  1,169.18 ns |  1.09 |    0.01 |    2 |         - |          NA |
| QuickSortMedian3             | 4096 | Random             |   194,655.3 ns |  1,057.93 ns |    553.32 ns |  1.05 |    0.00 |    2 |         - |          NA |
| QuickSortMedian9             | 4096 | Random             |   206,481.6 ns |  1,797.44 ns |    940.10 ns |  1.11 |    0.01 |    2 |         - |          NA |
| SampleSort                   | 4096 | Random             |   234,585.8 ns |  2,062.93 ns |    915.95 ns |  1.27 |    0.01 |    2 |         - |          NA |
| DualPivotQuickSort           | 4096 | Random             |   143,790.5 ns |    479.48 ns |    212.89 ns |  0.78 |    0.00 |    2 |         - |          NA |
| StableQuickSort              | 4096 | Random             |   468,973.2 ns |  1,981.24 ns |  1,036.22 ns |  2.53 |    0.01 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Random             |   241,724.2 ns |  1,469.78 ns |    652.59 ns |  1.30 |    0.01 |    2 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Random             |   391,346.2 ns |  2,262.57 ns |  1,183.37 ns |  2.11 |    0.01 |    3 |         - |          NA |
| IntroSort                    | 4096 | Random             |   160,494.7 ns |    771.84 ns |    403.69 ns |  0.87 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet              | 4096 | Random             |   155,928.7 ns |    810.70 ns |    424.01 ns |  0.84 |    0.00 |    2 |         - |          NA |
| PDQSort                      | 4096 | Random             |   149,332.5 ns |    946.76 ns |    420.37 ns |  0.81 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 4096 | Random             |    83,397.1 ns |    490.71 ns |    217.88 ns |  0.45 |    0.00 |    1 |         - |          NA |
| Ipnsort                      | 4096 | Random             |    91,211.7 ns |  1,020.43 ns |    453.08 ns |  0.49 |    0.00 |    1 |         - |          NA |
| StdSort                      | 4096 | Random             |    89,240.5 ns |    500.40 ns |    222.18 ns |  0.48 |    0.00 |    1 |         - |          NA |
| BlockQuickSort               | 4096 | Random             |    95,643.3 ns |    873.76 ns |    387.95 ns |  0.52 |    0.00 |    1 |         - |          NA |
| FluxSort                     | 4096 | Random             |   109,935.6 ns |    483.43 ns |    252.84 ns |  0.59 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 4096 | Random             |   145,003.6 ns |    639.13 ns |    283.78 ns |  0.78 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **SingleElementMoved** |    **19,299.3 ns** |    **338.16 ns** |    **150.15 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 4096 | SingleElementMoved |    34,619.2 ns |  3,315.70 ns |  1,734.18 ns |  1.79 |    0.09 |    2 |         - |          NA |
| QuickSortMedian3             | 4096 | SingleElementMoved |    26,599.9 ns |    270.84 ns |    120.26 ns |  1.38 |    0.01 |    2 |         - |          NA |
| QuickSortMedian9             | 4096 | SingleElementMoved |    30,414.1 ns |    292.46 ns |    152.96 ns |  1.58 |    0.01 |    2 |         - |          NA |
| SampleSort                   | 4096 | SingleElementMoved |   133,759.9 ns |  3,192.23 ns |  1,669.60 ns |  6.93 |    0.10 |    5 |         - |          NA |
| DualPivotQuickSort           | 4096 | SingleElementMoved |    16,701.8 ns |  1,182.48 ns |    618.46 ns |  0.87 |    0.03 |    2 |         - |          NA |
| StableQuickSort              | 4096 | SingleElementMoved |   149,344.3 ns |  1,994.01 ns |  1,042.91 ns |  7.74 |    0.08 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | SingleElementMoved |    71,720.8 ns |    320.82 ns |    167.80 ns |  3.72 |    0.03 |    4 |         - |          NA |
| DestswapStableQuickSort      | 4096 | SingleElementMoved |   142,019.7 ns |  1,113.40 ns |    494.36 ns |  7.36 |    0.06 |    5 |         - |          NA |
| IntroSort                    | 4096 | SingleElementMoved |    51,649.7 ns |    148.97 ns |     66.14 ns |  2.68 |    0.02 |    3 |         - |          NA |
| IntroSortDotnet              | 4096 | SingleElementMoved |    21,131.3 ns |    329.42 ns |    172.29 ns |  1.09 |    0.01 |    2 |         - |          NA |
| PDQSort                      | 4096 | SingleElementMoved |    12,001.0 ns |    316.88 ns |    165.74 ns |  0.62 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 4096 | SingleElementMoved |    13,835.3 ns |    263.18 ns |    137.65 ns |  0.72 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 4096 | SingleElementMoved |    74,469.3 ns | 11,583.22 ns |  6,058.25 ns |  3.86 |    0.30 |    4 |         - |          NA |
| StdSort                      | 4096 | SingleElementMoved |    12,883.5 ns |    223.10 ns |    116.69 ns |  0.67 |    0.01 |    2 |         - |          NA |
| BlockQuickSort               | 4096 | SingleElementMoved |    34,206.2 ns |    443.19 ns |    231.80 ns |  1.77 |    0.02 |    2 |         - |          NA |
| FluxSort                     | 4096 | SingleElementMoved |     4,273.2 ns |     41.59 ns |     18.46 ns |  0.22 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 4096 | SingleElementMoved |    24,673.9 ns |    276.04 ns |    144.38 ns |  1.28 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Sorted**             |    **15,543.9 ns** |    **166.21 ns** |     **86.93 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Sorted             |    28,193.7 ns |  2,061.15 ns |  1,078.02 ns |  1.81 |    0.07 |    5 |         - |          NA |
| QuickSortMedian3             | 4096 | Sorted             |    19,694.7 ns |    175.05 ns |     77.72 ns |  1.27 |    0.01 |    4 |         - |          NA |
| QuickSortMedian9             | 4096 | Sorted             |    21,227.5 ns |    871.51 ns |    455.82 ns |  1.37 |    0.03 |    4 |         - |          NA |
| SampleSort                   | 4096 | Sorted             |    69,202.2 ns |  1,449.65 ns |    643.65 ns |  4.45 |    0.05 |    6 |         - |          NA |
| DualPivotQuickSort           | 4096 | Sorted             |    15,257.7 ns |    999.28 ns |    522.64 ns |  0.98 |    0.03 |    4 |         - |          NA |
| StableQuickSort              | 4096 | Sorted             |   148,866.6 ns |  2,099.49 ns |    932.18 ns |  9.58 |    0.08 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Sorted             |    70,513.9 ns |    238.93 ns |    106.09 ns |  4.54 |    0.02 |    6 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Sorted             |   140,586.7 ns |  1,727.83 ns |    767.17 ns |  9.04 |    0.07 |    7 |         - |          NA |
| IntroSort                    | 4096 | Sorted             |    14,976.6 ns |    249.63 ns |    110.84 ns |  0.96 |    0.01 |    4 |         - |          NA |
| IntroSortDotnet              | 4096 | Sorted             |    17,154.2 ns |    139.66 ns |     62.01 ns |  1.10 |    0.01 |    4 |         - |          NA |
| PDQSort                      | 4096 | Sorted             |     4,527.7 ns |     29.91 ns |     10.67 ns |  0.29 |    0.00 |    3 |         - |          NA |
| PDQSortBranchless            | 4096 | Sorted             |     4,671.5 ns |    268.40 ns |    140.38 ns |  0.30 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 4096 | Sorted             |     1,777.3 ns |     14.40 ns |      6.39 ns |  0.11 |    0.00 |    1 |         - |          NA |
| StdSort                      | 4096 | Sorted             |     4,722.7 ns |    315.09 ns |    164.80 ns |  0.30 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 4096 | Sorted             |    30,380.9 ns |    272.93 ns |    121.18 ns |  1.95 |    0.01 |    5 |         - |          NA |
| FluxSort                     | 4096 | Sorted             |     2,223.7 ns |     12.00 ns |      4.28 ns |  0.14 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 4096 | Sorted             |    14,994.7 ns |     89.66 ns |     31.97 ns |  0.96 |    0.01 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Reversed**           |    **17,188.9 ns** |    **513.22 ns** |    **268.43 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Reversed           |    31,499.9 ns |  2,233.38 ns |  1,168.10 ns |  1.83 |    0.07 |    7 |         - |          NA |
| QuickSortMedian3             | 4096 | Reversed           |    20,968.3 ns |    201.77 ns |    105.53 ns |  1.22 |    0.02 |    6 |         - |          NA |
| QuickSortMedian9             | 4096 | Reversed           |    22,051.9 ns |    362.39 ns |    189.54 ns |  1.28 |    0.02 |    6 |         - |          NA |
| SampleSort                   | 4096 | Reversed           |    73,479.7 ns |  2,282.51 ns |  1,013.45 ns |  4.28 |    0.08 |    9 |         - |          NA |
| DualPivotQuickSort           | 4096 | Reversed           |    17,550.3 ns |  1,608.87 ns |    714.35 ns |  1.02 |    0.04 |    5 |         - |          NA |
| StableQuickSort              | 4096 | Reversed           |   151,498.5 ns |  3,318.33 ns |  1,735.55 ns |  8.82 |    0.16 |   10 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Reversed           |    71,605.9 ns |    386.72 ns |    202.26 ns |  4.17 |    0.06 |    9 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Reversed           |   135,552.8 ns |  1,950.29 ns |  1,020.04 ns |  7.89 |    0.13 |   10 |         - |          NA |
| IntroSort                    | 4096 | Reversed           |    15,443.4 ns |    118.97 ns |     62.23 ns |  0.90 |    0.01 |    5 |         - |          NA |
| IntroSortDotnet              | 4096 | Reversed           |    31,684.1 ns | 12,502.32 ns |  6,538.95 ns |  1.84 |    0.36 |    7 |         - |          NA |
| PDQSort                      | 4096 | Reversed           |     6,452.7 ns |    330.74 ns |    172.98 ns |  0.38 |    0.01 |    3 |         - |          NA |
| PDQSortBranchless            | 4096 | Reversed           |     7,883.3 ns |    470.38 ns |    208.85 ns |  0.46 |    0.01 |    4 |         - |          NA |
| Ipnsort                      | 4096 | Reversed           |     2,774.8 ns |      8.48 ns |      3.76 ns |  0.16 |    0.00 |    1 |         - |          NA |
| StdSort                      | 4096 | Reversed           |     9,151.8 ns |    274.94 ns |    143.80 ns |  0.53 |    0.01 |    4 |         - |          NA |
| BlockQuickSort               | 4096 | Reversed           |    37,567.4 ns |    332.45 ns |    173.88 ns |  2.19 |    0.03 |    8 |         - |          NA |
| FluxSort                     | 4096 | Reversed           |     3,346.5 ns |     38.29 ns |     20.03 ns |  0.19 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 4096 | Reversed           |    29,479.7 ns |  4,029.90 ns |  2,107.71 ns |  1.72 |    0.12 |    7 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **PipeOrgan**          | **1,221,276.0 ns** | **24,820.50 ns** | **11,020.46 ns** | **1.000** |    **0.00** |    **7** |         **-** |          **NA** |
| QuickSort3way                | 4096 | PipeOrgan          |    81,280.8 ns |  6,057.39 ns |  3,168.13 ns | 0.067 |    0.00 |    3 |         - |          NA |
| QuickSortMedian3             | 4096 | PipeOrgan          |    63,832.6 ns |  1,642.73 ns |    859.18 ns | 0.052 |    0.00 |    3 |         - |          NA |
| QuickSortMedian9             | 4096 | PipeOrgan          |    41,659.7 ns |  1,209.73 ns |    537.13 ns | 0.034 |    0.00 |    2 |         - |          NA |
| SampleSort                   | 4096 | PipeOrgan          |    73,026.7 ns |  3,739.96 ns |  1,956.07 ns | 0.060 |    0.00 |    3 |         - |          NA |
| DualPivotQuickSort           | 4096 | PipeOrgan          |    35,343.3 ns |    799.11 ns |    354.81 ns | 0.029 |    0.00 |    2 |         - |          NA |
| StableQuickSort              | 4096 | PipeOrgan          |   148,701.9 ns |  1,859.67 ns |    663.18 ns | 0.122 |    0.00 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | PipeOrgan          |    71,300.9 ns |    534.68 ns |    279.65 ns | 0.058 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 4096 | PipeOrgan          |   187,936.0 ns |    977.16 ns |    433.86 ns | 0.154 |    0.00 |    6 |         - |          NA |
| IntroSort                    | 4096 | PipeOrgan          |   104,918.9 ns |    588.80 ns |    261.43 ns | 0.086 |    0.00 |    4 |         - |          NA |
| IntroSortDotnet              | 4096 | PipeOrgan          |    64,942.4 ns |    788.24 ns |    349.98 ns | 0.053 |    0.00 |    3 |         - |          NA |
| PDQSort                      | 4096 | PipeOrgan          |    31,348.7 ns |    832.79 ns |    435.57 ns | 0.026 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 4096 | PipeOrgan          |    47,546.0 ns |    829.08 ns |    433.62 ns | 0.039 |    0.00 |    2 |         - |          NA |
| Ipnsort                      | 4096 | PipeOrgan          |    83,545.2 ns |    399.43 ns |    177.35 ns | 0.068 |    0.00 |    3 |         - |          NA |
| StdSort                      | 4096 | PipeOrgan          |   101,602.4 ns |    808.76 ns |    359.09 ns | 0.083 |    0.00 |    4 |         - |          NA |
| BlockQuickSort               | 4096 | PipeOrgan          |    80,679.2 ns |  1,150.57 ns |    510.86 ns | 0.066 |    0.00 |    3 |         - |          NA |
| FluxSort                     | 4096 | PipeOrgan          |     9,036.8 ns |    338.27 ns |    176.92 ns | 0.007 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 4096 | PipeOrgan          |    70,938.2 ns |  2,150.02 ns |  1,124.50 ns | 0.058 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **4096** | **ManyDuplicates**     |   **103,147.6 ns** |    **698.44 ns** |    **310.11 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 4096 | ManyDuplicates     |    75,006.0 ns |    655.11 ns |    290.87 ns |  0.73 |    0.00 |    4 |         - |          NA |
| QuickSortMedian3             | 4096 | ManyDuplicates     |   113,396.7 ns |  1,103.55 ns |    489.98 ns |  1.10 |    0.01 |    5 |         - |          NA |
| QuickSortMedian9             | 4096 | ManyDuplicates     |   118,944.8 ns |    882.58 ns |    391.87 ns |  1.15 |    0.00 |    5 |         - |          NA |
| SampleSort                   | 4096 | ManyDuplicates     |   156,950.7 ns |  1,271.03 ns |    664.78 ns |  1.52 |    0.01 |    6 |         - |          NA |
| DualPivotQuickSort           | 4096 | ManyDuplicates     |    72,884.4 ns |    496.48 ns |    220.44 ns |  0.71 |    0.00 |    4 |         - |          NA |
| StableQuickSort              | 4096 | ManyDuplicates     |   178,967.2 ns |  2,075.70 ns |  1,085.63 ns |  1.74 |    0.01 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | ManyDuplicates     |    78,025.7 ns |    608.22 ns |    270.05 ns |  0.76 |    0.00 |    4 |         - |          NA |
| DestswapStableQuickSort      | 4096 | ManyDuplicates     |   130,334.7 ns |    591.27 ns |    262.53 ns |  1.26 |    0.00 |    5 |         - |          NA |
| IntroSort                    | 4096 | ManyDuplicates     |    97,241.5 ns |    754.75 ns |    335.11 ns |  0.94 |    0.00 |    5 |         - |          NA |
| IntroSortDotnet              | 4096 | ManyDuplicates     |   103,329.2 ns |    695.01 ns |    308.59 ns |  1.00 |    0.00 |    5 |         - |          NA |
| PDQSort                      | 4096 | ManyDuplicates     |    73,852.1 ns |    320.39 ns |    142.25 ns |  0.72 |    0.00 |    4 |         - |          NA |
| PDQSortBranchless            | 4096 | ManyDuplicates     |    26,786.2 ns |    482.62 ns |    252.42 ns |  0.26 |    0.00 |    1 |         - |          NA |
| Ipnsort                      | 4096 | ManyDuplicates     |    48,922.2 ns |    256.88 ns |    134.35 ns |  0.47 |    0.00 |    3 |         - |          NA |
| StdSort                      | 4096 | ManyDuplicates     |    42,581.6 ns |    179.97 ns |     94.13 ns |  0.41 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 4096 | ManyDuplicates     |    55,869.6 ns |    645.01 ns |    337.35 ns |  0.54 |    0.00 |    3 |         - |          NA |
| FluxSort                     | 4096 | ManyDuplicates     |    34,458.4 ns |    250.31 ns |    130.92 ns |  0.33 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 4096 | ManyDuplicates     |    94,459.8 ns |    148.53 ns |     65.95 ns |  0.92 |    0.00 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Random**             |   **394,569.9 ns** |  **1,644.53 ns** |    **860.12 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Random             |   432,788.7 ns |  3,272.88 ns |  1,711.78 ns |  1.10 |    0.00 |    2 |         - |          NA |
| QuickSortMedian3             | 8192 | Random             |   412,981.6 ns |  2,113.03 ns |    938.20 ns |  1.05 |    0.00 |    2 |         - |          NA |
| QuickSortMedian9             | 8192 | Random             |   438,879.2 ns |  1,767.12 ns |    924.24 ns |  1.11 |    0.00 |    2 |         - |          NA |
| SampleSort                   | 8192 | Random             |   499,468.4 ns |  1,637.58 ns |    727.10 ns |  1.27 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort           | 8192 | Random             |   312,211.6 ns |    843.05 ns |    374.32 ns |  0.79 |    0.00 |    2 |         - |          NA |
| StableQuickSort              | 8192 | Random             | 1,011,198.3 ns |  2,402.09 ns |  1,066.54 ns |  2.56 |    0.01 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Random             |   519,290.2 ns |  2,218.85 ns |  1,160.50 ns |  1.32 |    0.00 |    2 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Random             |   824,194.3 ns |  1,139.29 ns |    595.87 ns |  2.09 |    0.00 |    3 |         - |          NA |
| IntroSort                    | 8192 | Random             |   345,906.1 ns |    983.95 ns |    436.88 ns |  0.88 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet              | 8192 | Random             |   336,694.7 ns |    671.79 ns |    298.28 ns |  0.85 |    0.00 |    2 |         - |          NA |
| PDQSort                      | 8192 | Random             |   323,084.8 ns |  1,040.36 ns |    461.93 ns |  0.82 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | Random             |   173,393.4 ns |    653.55 ns |    290.18 ns |  0.44 |    0.00 |    1 |         - |          NA |
| Ipnsort                      | 8192 | Random             |   194,804.7 ns |    853.98 ns |    379.17 ns |  0.49 |    0.00 |    1 |         - |          NA |
| StdSort                      | 8192 | Random             |   186,904.8 ns |    918.99 ns |    480.65 ns |  0.47 |    0.00 |    1 |         - |          NA |
| BlockQuickSort               | 8192 | Random             |   199,998.2 ns |    499.00 ns |    221.56 ns |  0.51 |    0.00 |    1 |         - |          NA |
| FluxSort                     | 8192 | Random             |   226,691.8 ns |  1,398.17 ns |    731.27 ns |  0.57 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 8192 | Random             |   315,505.3 ns |  1,532.45 ns |    801.50 ns |  0.80 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **SingleElementMoved** |    **40,385.6 ns** |    **953.72 ns** |    **498.81 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 8192 | SingleElementMoved |    70,968.2 ns |  4,512.64 ns |  2,360.20 ns |  1.76 |    0.06 |    4 |         - |          NA |
| QuickSortMedian3             | 8192 | SingleElementMoved |    54,265.6 ns |    365.46 ns |    162.27 ns |  1.34 |    0.02 |    4 |         - |          NA |
| QuickSortMedian9             | 8192 | SingleElementMoved |    62,700.0 ns |    776.09 ns |    405.91 ns |  1.55 |    0.02 |    4 |         - |          NA |
| SampleSort                   | 8192 | SingleElementMoved |   319,673.2 ns |  4,188.79 ns |  2,190.82 ns |  7.92 |    0.11 |    7 |         - |          NA |
| DualPivotQuickSort           | 8192 | SingleElementMoved |    38,744.0 ns |  2,276.85 ns |  1,190.84 ns |  0.96 |    0.03 |    3 |         - |          NA |
| StableQuickSort              | 8192 | SingleElementMoved |   323,699.6 ns |  2,320.81 ns |  1,030.45 ns |  8.02 |    0.10 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | SingleElementMoved |   150,953.9 ns |    900.45 ns |    399.81 ns |  3.74 |    0.04 |    6 |         - |          NA |
| DestswapStableQuickSort      | 8192 | SingleElementMoved |   299,895.0 ns |  2,950.95 ns |  1,310.24 ns |  7.43 |    0.09 |    7 |         - |          NA |
| IntroSort                    | 8192 | SingleElementMoved |   108,636.5 ns |  1,171.65 ns |    612.79 ns |  2.69 |    0.03 |    5 |         - |          NA |
| IntroSortDotnet              | 8192 | SingleElementMoved |    44,063.3 ns |    364.19 ns |    161.70 ns |  1.09 |    0.01 |    3 |         - |          NA |
| PDQSort                      | 8192 | SingleElementMoved |    23,524.6 ns |     98.80 ns |     43.87 ns |  0.58 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | SingleElementMoved |    26,925.6 ns |    288.79 ns |    102.99 ns |  0.67 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 8192 | SingleElementMoved |   154,094.9 ns |  1,212.77 ns |    538.48 ns |  3.82 |    0.05 |    6 |         - |          NA |
| StdSort                      | 8192 | SingleElementMoved |    25,301.4 ns |    329.10 ns |    146.12 ns |  0.63 |    0.01 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | SingleElementMoved |    73,111.0 ns |  1,161.58 ns |    515.75 ns |  1.81 |    0.02 |    4 |         - |          NA |
| FluxSort                     | 8192 | SingleElementMoved |     8,764.4 ns |    417.88 ns |    218.56 ns |  0.22 |    0.01 |    1 |         - |          NA |
| DotnetSort                   | 8192 | SingleElementMoved |    53,167.7 ns |  1,310.95 ns |    582.07 ns |  1.32 |    0.02 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Sorted**             |    **33,558.4 ns** |    **398.54 ns** |    **208.44 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Sorted             |    59,185.5 ns |  3,790.27 ns |  1,982.38 ns |  1.76 |    0.06 |    5 |         - |          NA |
| QuickSortMedian3             | 8192 | Sorted             |    41,601.9 ns |    204.93 ns |    107.18 ns |  1.24 |    0.01 |    4 |         - |          NA |
| QuickSortMedian9             | 8192 | Sorted             |    43,925.3 ns |    407.25 ns |    213.00 ns |  1.31 |    0.01 |    4 |         - |          NA |
| SampleSort                   | 8192 | Sorted             |   199,812.6 ns | 11,195.50 ns |  5,855.46 ns |  5.95 |    0.17 |    7 |         - |          NA |
| DualPivotQuickSort           | 8192 | Sorted             |    38,131.1 ns |  2,240.75 ns |  1,171.96 ns |  1.14 |    0.03 |    4 |         - |          NA |
| StableQuickSort              | 8192 | Sorted             |   323,960.2 ns |  1,568.52 ns |    820.37 ns |  9.65 |    0.06 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Sorted             |   149,756.5 ns |  1,633.70 ns |    725.37 ns |  4.46 |    0.03 |    6 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Sorted             |   292,101.5 ns |  1,145.26 ns |    508.50 ns |  8.70 |    0.05 |    8 |         - |          NA |
| IntroSort                    | 8192 | Sorted             |    32,172.8 ns |    341.63 ns |    178.68 ns |  0.96 |    0.01 |    4 |         - |          NA |
| IntroSortDotnet              | 8192 | Sorted             |    36,738.9 ns |    502.28 ns |    262.70 ns |  1.09 |    0.01 |    4 |         - |          NA |
| PDQSort                      | 8192 | Sorted             |     9,137.5 ns |    173.07 ns |     90.52 ns |  0.27 |    0.00 |    3 |         - |          NA |
| PDQSortBranchless            | 8192 | Sorted             |     9,323.6 ns |    607.29 ns |    317.63 ns |  0.28 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 8192 | Sorted             |     3,727.4 ns |    536.24 ns |    280.46 ns |  0.11 |    0.01 |    1 |         - |          NA |
| StdSort                      | 8192 | Sorted             |     9,354.0 ns |    390.85 ns |    204.42 ns |  0.28 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 8192 | Sorted             |    65,931.3 ns |    557.12 ns |    291.38 ns |  1.96 |    0.01 |    5 |         - |          NA |
| FluxSort                     | 8192 | Sorted             |     4,347.6 ns |     88.41 ns |     31.53 ns |  0.13 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 8192 | Sorted             |    34,215.1 ns |  3,337.72 ns |  1,481.97 ns |  1.02 |    0.04 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Reversed**           |    **36,494.5 ns** |  **1,542.60 ns** |    **806.81 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Reversed           |    64,406.6 ns |  2,661.08 ns |  1,181.54 ns |  1.77 |    0.05 |    5 |         - |          NA |
| QuickSortMedian3             | 8192 | Reversed           |    43,829.3 ns |    162.36 ns |     72.09 ns |  1.20 |    0.02 |    4 |         - |          NA |
| QuickSortMedian9             | 8192 | Reversed           |    46,238.2 ns |    295.62 ns |    154.62 ns |  1.27 |    0.03 |    4 |         - |          NA |
| SampleSort                   | 8192 | Reversed           |   276,652.9 ns |  7,037.06 ns |  3,124.50 ns |  7.58 |    0.18 |    8 |         - |          NA |
| DualPivotQuickSort           | 8192 | Reversed           |    39,337.6 ns |  2,271.70 ns |  1,188.15 ns |  1.08 |    0.04 |    4 |         - |          NA |
| StableQuickSort              | 8192 | Reversed           |   331,056.1 ns |  5,465.51 ns |  2,858.57 ns |  9.08 |    0.20 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Reversed           |   150,975.6 ns |  1,751.49 ns |    777.67 ns |  4.14 |    0.09 |    7 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Reversed           |   283,622.9 ns |  3,034.30 ns |  1,347.25 ns |  7.77 |    0.16 |    8 |         - |          NA |
| IntroSort                    | 8192 | Reversed           |    33,901.2 ns |    785.60 ns |    410.88 ns |  0.93 |    0.02 |    4 |         - |          NA |
| IntroSortDotnet              | 8192 | Reversed           |    58,204.6 ns |    559.72 ns |    292.74 ns |  1.60 |    0.03 |    5 |         - |          NA |
| PDQSort                      | 8192 | Reversed           |    12,651.3 ns |    381.59 ns |    199.58 ns |  0.35 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | Reversed           |    15,516.6 ns |    186.21 ns |     82.68 ns |  0.43 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 8192 | Reversed           |     5,635.2 ns |    323.88 ns |    169.40 ns |  0.15 |    0.01 |    1 |         - |          NA |
| StdSort                      | 8192 | Reversed           |    18,274.9 ns |    271.46 ns |    141.98 ns |  0.50 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 8192 | Reversed           |    80,200.0 ns |    968.76 ns |    430.13 ns |  2.20 |    0.05 |    6 |         - |          NA |
| FluxSort                     | 8192 | Reversed           |     6,702.2 ns |    309.24 ns |    137.30 ns |  0.18 |    0.01 |    1 |         - |          NA |
| DotnetSort                   | 8192 | Reversed           |    58,460.2 ns |  2,150.83 ns |  1,124.93 ns |  1.60 |    0.04 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **PipeOrgan**          | **4,731,520.4 ns** |  **6,746.99 ns** |  **2,406.04 ns** | **1.000** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 8192 | PipeOrgan          |   208,290.0 ns | 13,969.45 ns |  7,306.29 ns | 0.044 |    0.00 |    3 |         - |          NA |
| QuickSortMedian3             | 8192 | PipeOrgan          |   152,824.9 ns |  4,272.16 ns |  2,234.42 ns | 0.032 |    0.00 |    3 |         - |          NA |
| QuickSortMedian9             | 8192 | PipeOrgan          |    88,332.0 ns |  2,076.02 ns |  1,085.80 ns | 0.019 |    0.00 |    2 |         - |          NA |
| SampleSort                   | 8192 | PipeOrgan          |   304,691.7 ns |  6,080.87 ns |  3,180.41 ns | 0.064 |    0.00 |    3 |         - |          NA |
| DualPivotQuickSort           | 8192 | PipeOrgan          |    76,314.4 ns |    796.50 ns |    416.59 ns | 0.016 |    0.00 |    2 |         - |          NA |
| StableQuickSort              | 8192 | PipeOrgan          |   323,306.8 ns |  2,396.27 ns |  1,253.30 ns | 0.068 |    0.00 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | PipeOrgan          |   150,932.1 ns |  2,976.50 ns |  1,321.58 ns | 0.032 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 8192 | PipeOrgan          |   377,436.2 ns |  3,914.59 ns |  1,738.10 ns | 0.080 |    0.00 |    3 |         - |          NA |
| IntroSort                    | 8192 | PipeOrgan          |   230,600.6 ns |  1,916.96 ns |    851.14 ns | 0.049 |    0.00 |    3 |         - |          NA |
| IntroSortDotnet              | 8192 | PipeOrgan          |   269,195.3 ns |  1,179.12 ns |    420.48 ns | 0.057 |    0.00 |    3 |         - |          NA |
| PDQSort                      | 8192 | PipeOrgan          |    68,002.9 ns |  2,870.15 ns |  1,501.14 ns | 0.014 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | PipeOrgan          |   102,030.1 ns |  1,688.72 ns |    749.80 ns | 0.022 |    0.00 |    2 |         - |          NA |
| Ipnsort                      | 8192 | PipeOrgan          |   185,456.0 ns |    431.43 ns |    225.65 ns | 0.039 |    0.00 |    3 |         - |          NA |
| StdSort                      | 8192 | PipeOrgan          |   249,482.7 ns |  2,364.15 ns |  1,236.49 ns | 0.053 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 8192 | PipeOrgan          |   157,435.4 ns |  1,448.23 ns |    643.02 ns | 0.033 |    0.00 |    3 |         - |          NA |
| FluxSort                     | 8192 | PipeOrgan          |    18,096.5 ns |    217.89 ns |     96.74 ns | 0.004 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 8192 | PipeOrgan          |   284,100.6 ns |  6,965.21 ns |  3,642.94 ns | 0.060 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **QuickSort**                    | **8192** | **ManyDuplicates**     |   **206,163.7 ns** |  **1,345.92 ns** |    **703.94 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 8192 | ManyDuplicates     |   146,899.7 ns |    563.14 ns |    250.04 ns |  0.71 |    0.00 |    4 |         - |          NA |
| QuickSortMedian3             | 8192 | ManyDuplicates     |   222,527.1 ns |    657.40 ns |    291.89 ns |  1.08 |    0.00 |    5 |         - |          NA |
| QuickSortMedian9             | 8192 | ManyDuplicates     |   234,985.3 ns |    927.93 ns |    412.01 ns |  1.14 |    0.00 |    5 |         - |          NA |
| SampleSort                   | 8192 | ManyDuplicates     |   312,585.6 ns |  3,195.03 ns |  1,671.06 ns |  1.52 |    0.01 |    6 |         - |          NA |
| DualPivotQuickSort           | 8192 | ManyDuplicates     |   144,533.0 ns |  1,089.30 ns |    483.66 ns |  0.70 |    0.00 |    4 |         - |          NA |
| StableQuickSort              | 8192 | ManyDuplicates     |   348,430.3 ns |  9,349.58 ns |  4,890.01 ns |  1.69 |    0.02 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | ManyDuplicates     |   153,059.8 ns |    647.94 ns |    287.69 ns |  0.74 |    0.00 |    4 |         - |          NA |
| DestswapStableQuickSort      | 8192 | ManyDuplicates     |   252,721.3 ns |  1,117.41 ns |    496.14 ns |  1.23 |    0.00 |    5 |         - |          NA |
| IntroSort                    | 8192 | ManyDuplicates     |   194,215.3 ns |  2,214.71 ns |  1,158.34 ns |  0.94 |    0.01 |    5 |         - |          NA |
| IntroSortDotnet              | 8192 | ManyDuplicates     |   207,948.7 ns |    628.92 ns |    279.25 ns |  1.01 |    0.00 |    5 |         - |          NA |
| PDQSort                      | 8192 | ManyDuplicates     |   144,062.8 ns |    393.50 ns |    174.72 ns |  0.70 |    0.00 |    4 |         - |          NA |
| PDQSortBranchless            | 8192 | ManyDuplicates     |    48,957.6 ns |    661.99 ns |    346.23 ns |  0.24 |    0.00 |    1 |         - |          NA |
| Ipnsort                      | 8192 | ManyDuplicates     |    94,165.9 ns |    587.18 ns |    307.10 ns |  0.46 |    0.00 |    3 |         - |          NA |
| StdSort                      | 8192 | ManyDuplicates     |    77,743.1 ns |    535.44 ns |    237.74 ns |  0.38 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | ManyDuplicates     |   108,724.6 ns |  1,067.63 ns |    474.04 ns |  0.53 |    0.00 |    3 |         - |          NA |
| FluxSort                     | 8192 | ManyDuplicates     |    57,126.5 ns |    413.46 ns |    183.58 ns |  0.28 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 8192 | ManyDuplicates     |   190,441.8 ns |  1,210.12 ns |    537.30 ns |  0.92 |    0.00 |    5 |         - |          NA |

### SelectionBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.52 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method              | Size | Pattern            | Mean         | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------------- |----- |------------------- |-------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **SelectionSort**       | **256**  | **Random**             |  **25,865.6 ns** |    **217.46 ns** |    **113.73 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Random             |  18,486.0 ns |    287.17 ns |    127.51 ns |  0.71 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | Random             |  48,879.6 ns |    125.62 ns |     65.70 ns |  1.89 |    0.01 |    3 |         - |          NA |
| PancakeSort         | 256  | Random             |  32,622.7 ns |    219.46 ns |    114.78 ns |  1.26 |    0.01 |    2 |         - |          NA |
| BingoSort           | 256  | Random             |  29,078.7 ns |    141.45 ns |     50.44 ns |  1.12 |    0.01 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **SingleElementMoved** |  **19,425.0 ns** |    **151.53 ns** |     **79.25 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | SingleElementMoved |  10,844.7 ns |    300.29 ns |    157.06 ns |  0.56 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | SingleElementMoved |  25,325.2 ns |    201.37 ns |    105.32 ns |  1.30 |    0.01 |    4 |         - |          NA |
| PancakeSort         | 256  | SingleElementMoved |  15,990.2 ns |    168.84 ns |     88.31 ns |  0.82 |    0.01 |    2 |         - |          NA |
| BingoSort           | 256  | SingleElementMoved |  19,526.1 ns |    162.06 ns |     84.76 ns |  1.01 |    0.01 |    3 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **Sorted**             |  **19,105.6 ns** |     **85.71 ns** |     **38.06 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Sorted             |   8,169.3 ns |    332.17 ns |    173.73 ns |  0.43 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | Sorted             |  17,448.6 ns |    115.12 ns |     51.11 ns |  0.91 |    0.00 |    3 |         - |          NA |
| PancakeSort         | 256  | Sorted             |  13,041.6 ns |    140.16 ns |     73.31 ns |  0.68 |    0.00 |    2 |         - |          NA |
| BingoSort           | 256  | Sorted             |  19,265.6 ns |    515.42 ns |    269.57 ns |  1.01 |    0.01 |    3 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **Reversed**           |  **16,833.2 ns** |    **166.54 ns** |     **59.39 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Reversed           |   8,245.7 ns |    137.98 ns |     72.17 ns |  0.49 |    0.00 |    1 |         - |          NA |
| CycleSort           | 256  | Reversed           |  30,465.2 ns |    303.36 ns |    158.66 ns |  1.81 |    0.01 |    4 |         - |          NA |
| PancakeSort         | 256  | Reversed           |  14,450.8 ns |    314.82 ns |    164.66 ns |  0.86 |    0.01 |    2 |         - |          NA |
| BingoSort           | 256  | Reversed           |  21,959.3 ns |  3,418.85 ns |  1,788.12 ns |  1.30 |    0.10 |    3 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **PipeOrgan**          |  **18,590.5 ns** |    **573.17 ns** |    **254.49 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | PipeOrgan          |   9,285.0 ns |    289.99 ns |    151.67 ns |  0.50 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | PipeOrgan          |  43,020.2 ns |    291.21 ns |    152.31 ns |  2.31 |    0.03 |    3 |         - |          NA |
| PancakeSort         | 256  | PipeOrgan          |  20,612.0 ns |    341.09 ns |    178.40 ns |  1.11 |    0.02 |    2 |         - |          NA |
| BingoSort           | 256  | PipeOrgan          |  20,207.8 ns |    993.13 ns |    519.42 ns |  1.09 |    0.03 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **ManyDuplicates**     |  **24,202.2 ns** |    **187.05 ns** |     **97.83 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | ManyDuplicates     |  19,504.2 ns |    185.29 ns |     82.27 ns |  0.81 |    0.00 |    2 |         - |          NA |
| CycleSort           | 256  | ManyDuplicates     |  50,007.1 ns |    301.17 ns |    133.72 ns |  2.07 |    0.01 |    5 |         - |          NA |
| PancakeSort         | 256  | ManyDuplicates     |  30,576.3 ns |    108.72 ns |     48.27 ns |  1.26 |    0.01 |    4 |         - |          NA |
| BingoSort           | 256  | ManyDuplicates     |   6,011.8 ns |    222.49 ns |    116.37 ns |  0.25 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Random**             | **326,846.1 ns** |  **1,235.09 ns** |    **548.39 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Random             | 194,436.0 ns |    878.00 ns |    389.84 ns |  0.59 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | Random             | 772,933.7 ns |  1,849.58 ns |    967.37 ns |  2.36 |    0.00 |    3 |         - |          NA |
| PancakeSort         | 1024 | Random             | 379,505.2 ns |  1,134.33 ns |    503.65 ns |  1.16 |    0.00 |    2 |         - |          NA |
| BingoSort           | 1024 | Random             | 341,318.5 ns |  2,036.78 ns |    904.34 ns |  1.04 |    0.00 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **SingleElementMoved** | **293,892.2 ns** |  **1,487.36 ns** |    **660.40 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | SingleElementMoved | 149,515.6 ns |    271.47 ns |    120.54 ns |  0.51 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | SingleElementMoved | 389,904.9 ns |    668.60 ns |    238.43 ns |  1.33 |    0.00 |    4 |         - |          NA |
| PancakeSort         | 1024 | SingleElementMoved | 240,996.9 ns | 11,714.02 ns |  6,126.66 ns |  0.82 |    0.02 |    2 |         - |          NA |
| BingoSort           | 1024 | SingleElementMoved | 293,334.9 ns |    878.22 ns |    389.93 ns |  1.00 |    0.00 |    3 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Sorted**             | **291,533.4 ns** |  **1,237.08 ns** |    **549.27 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Sorted             | 124,315.6 ns |    233.12 ns |    103.51 ns |  0.43 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | Sorted             | 263,067.5 ns |    868.71 ns |    385.71 ns |  0.90 |    0.00 |    3 |         - |          NA |
| PancakeSort         | 1024 | Sorted             | 196,898.5 ns |    521.34 ns |    231.48 ns |  0.68 |    0.00 |    2 |         - |          NA |
| BingoSort           | 1024 | Sorted             | 293,089.5 ns |  2,830.19 ns |  1,256.62 ns |  1.01 |    0.00 |    3 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Reversed**           | **265,892.8 ns** | **16,919.78 ns** |  **8,849.37 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Reversed           | 125,396.2 ns |  1,109.65 ns |    492.69 ns |  0.47 |    0.02 |    1 |         - |          NA |
| CycleSort           | 1024 | Reversed           | 461,782.1 ns |  3,992.22 ns |  1,772.57 ns |  1.74 |    0.06 |    5 |         - |          NA |
| PancakeSort         | 1024 | Reversed           | 214,925.6 ns |  8,630.99 ns |  4,514.18 ns |  0.81 |    0.03 |    2 |         - |          NA |
| BingoSort           | 1024 | Reversed           | 338,686.0 ns | 17,514.21 ns |  9,160.27 ns |  1.28 |    0.05 |    4 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **PipeOrgan**          | **338,910.9 ns** | **93,206.63 ns** | **48,748.87 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | PipeOrgan          | 165,415.4 ns | 35,865.91 ns | 15,924.69 ns |  0.50 |    0.08 |    1 |         - |          NA |
| CycleSort           | 1024 | PipeOrgan          | 679,301.2 ns |  4,055.05 ns |  2,120.87 ns |  2.04 |    0.26 |    3 |         - |          NA |
| PancakeSort         | 1024 | PipeOrgan          | 316,945.8 ns |  2,655.55 ns |  1,179.08 ns |  0.95 |    0.12 |    2 |         - |          NA |
| BingoSort           | 1024 | PipeOrgan          | 331,376.3 ns |  6,594.39 ns |  3,448.99 ns |  0.99 |    0.12 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **ManyDuplicates**     | **313,921.7 ns** |  **2,491.14 ns** |  **1,106.08 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | ManyDuplicates     | 213,476.4 ns |  1,809.39 ns |    803.38 ns |  0.68 |    0.00 |    2 |         - |          NA |
| CycleSort           | 1024 | ManyDuplicates     | 765,795.5 ns |  2,414.00 ns |  1,071.83 ns |  2.44 |    0.01 |    4 |         - |          NA |
| PancakeSort         | 1024 | ManyDuplicates     | 357,830.8 ns |  1,241.04 ns |    551.03 ns |  1.14 |    0.00 |    3 |         - |          NA |
| BingoSort           | 1024 | ManyDuplicates     |  20,220.9 ns |    187.53 ns |     83.26 ns |  0.06 |    0.00 |    1 |         - |          NA |

### SelectionSimdBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.52 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                  | Size | Pattern            | Mean            | Error         | StdDev        | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------------ |----- |------------------- |----------------:|--------------:|--------------:|------:|--------:|-----:|----------:|------------:|
| **SelectionSort**           | **256**  | **Random**             |     **25,846.7 ns** |     **218.87 ns** |     **114.47 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| SelectionSortSimd       | 256  | Random             |      9,645.6 ns |     332.12 ns |     173.70 ns |  0.37 |    0.01 |    1 |         - |          NA |
| DoubleSelectionSort     | 256  | Random             |     18,436.3 ns |     225.55 ns |     117.97 ns |  0.71 |    0.01 |    2 |         - |          NA |
| DoubleSelectionSortSimd | 256  | Random             |     11,062.4 ns |     382.27 ns |     199.93 ns |  0.43 |    0.01 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **256**  | **SingleElementMoved** |     **19,305.1 ns** |     **183.35 ns** |      **95.90 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 256  | SingleElementMoved |      5,046.6 ns |     619.05 ns |     323.78 ns |  0.26 |    0.02 |    2 |         - |          NA |
| DoubleSelectionSort     | 256  | SingleElementMoved |     10,849.0 ns |     266.92 ns |     139.61 ns |  0.56 |    0.01 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 256  | SingleElementMoved |      4,098.9 ns |     248.85 ns |     130.15 ns |  0.21 |    0.01 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **256**  | **Sorted**             |     **19,229.7 ns** |     **318.16 ns** |     **141.27 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 256  | Sorted             |      3,536.6 ns |      75.43 ns |      33.49 ns |  0.18 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 256  | Sorted             |      8,098.8 ns |     165.11 ns |      86.35 ns |  0.42 |    0.01 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 256  | Sorted             |      2,878.6 ns |      65.10 ns |      28.91 ns |  0.15 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **256**  | **Reversed**           |     **18,412.7 ns** |   **2,187.24 ns** |   **1,143.97 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 256  | Reversed           |      5,697.1 ns |     238.37 ns |     105.84 ns |  0.31 |    0.02 |    2 |         - |          NA |
| DoubleSelectionSort     | 256  | Reversed           |      8,184.4 ns |     307.24 ns |     160.69 ns |  0.45 |    0.03 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 256  | Reversed           |      4,328.0 ns |     279.97 ns |     146.43 ns |  0.24 |    0.02 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **256**  | **PipeOrgan**          |     **18,503.3 ns** |     **556.09 ns** |     **290.85 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 256  | PipeOrgan          |      6,887.8 ns |     101.06 ns |      44.87 ns |  0.37 |    0.01 |    2 |         - |          NA |
| DoubleSelectionSort     | 256  | PipeOrgan          |      9,060.2 ns |     226.62 ns |     100.62 ns |  0.49 |    0.01 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 256  | PipeOrgan          |      4,374.6 ns |     338.30 ns |     176.93 ns |  0.24 |    0.01 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **256**  | **ManyDuplicates**     |     **24,223.7 ns** |     **264.69 ns** |     **117.52 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| SelectionSortSimd       | 256  | ManyDuplicates     |      9,202.1 ns |     360.28 ns |     159.97 ns |  0.38 |    0.01 |    1 |         - |          NA |
| DoubleSelectionSort     | 256  | ManyDuplicates     |     19,492.6 ns |     193.58 ns |     101.24 ns |  0.80 |    0.01 |    2 |         - |          NA |
| DoubleSelectionSortSimd | 256  | ManyDuplicates     |     10,817.0 ns |     292.84 ns |     153.16 ns |  0.45 |    0.01 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **1024** | **Random**             |    **327,272.3 ns** |   **2,879.10 ns** |   **1,278.34 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| SelectionSortSimd       | 1024 | Random             |     58,348.8 ns |     181.12 ns |      80.42 ns |  0.18 |    0.00 |    1 |         - |          NA |
| DoubleSelectionSort     | 1024 | Random             |    194,474.1 ns |     900.75 ns |     399.94 ns |  0.59 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSortSimd | 1024 | Random             |     58,676.7 ns |     288.10 ns |     127.92 ns |  0.18 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **1024** | **SingleElementMoved** |    **292,853.7 ns** |     **901.73 ns** |     **321.57 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 1024 | SingleElementMoved |     33,257.2 ns |     248.55 ns |     130.00 ns |  0.11 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 1024 | SingleElementMoved |    149,634.4 ns |     484.67 ns |     215.20 ns |  0.51 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 1024 | SingleElementMoved |     22,268.6 ns |     602.03 ns |     314.88 ns |  0.08 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **1024** | **Sorted**             |    **291,453.7 ns** |     **689.73 ns** |     **306.25 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 1024 | Sorted             |     24,954.5 ns |     233.97 ns |     122.37 ns |  0.09 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 1024 | Sorted             |    124,501.0 ns |     715.95 ns |     317.89 ns |  0.43 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 1024 | Sorted             |     16,444.2 ns |     460.93 ns |     241.08 ns |  0.06 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **1024** | **Reversed**           |    **265,171.0 ns** |  **12,858.18 ns** |   **5,709.11 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| SelectionSortSimd       | 1024 | Reversed           |     39,488.8 ns |     477.88 ns |     249.94 ns |  0.15 |    0.00 |    1 |         - |          NA |
| DoubleSelectionSort     | 1024 | Reversed           |    124,640.4 ns |     462.10 ns |     205.18 ns |  0.47 |    0.01 |    2 |         - |          NA |
| DoubleSelectionSortSimd | 1024 | Reversed           |     38,642.7 ns |     515.22 ns |     228.76 ns |  0.15 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **1024** | **PipeOrgan**          |    **302,567.0 ns** |   **4,019.65 ns** |   **2,102.36 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 1024 | PipeOrgan          |     49,102.5 ns |     282.57 ns |     147.79 ns |  0.16 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 1024 | PipeOrgan          |    154,068.4 ns |     745.06 ns |     330.81 ns |  0.51 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 1024 | PipeOrgan          |     30,233.4 ns |     307.71 ns |     160.94 ns |  0.10 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **1024** | **ManyDuplicates**     |    **313,876.6 ns** |   **2,732.74 ns** |   **1,213.35 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| SelectionSortSimd       | 1024 | ManyDuplicates     |     56,881.0 ns |     253.85 ns |     132.77 ns |  0.18 |    0.00 |    1 |         - |          NA |
| DoubleSelectionSort     | 1024 | ManyDuplicates     |    212,165.7 ns |     537.95 ns |     238.85 ns |  0.68 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSortSimd | 1024 | ManyDuplicates     |     57,556.4 ns |     331.63 ns |     173.45 ns |  0.18 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **4096** | **Random**             |  **4,780,358.2 ns** |  **29,708.84 ns** |  **15,538.30 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| SelectionSortSimd       | 4096 | Random             |    531,213.8 ns |   1,408.58 ns |     736.71 ns |  0.11 |    0.00 |    1 |         - |          NA |
| DoubleSelectionSort     | 4096 | Random             |  2,537,613.5 ns |   6,584.37 ns |   2,348.05 ns |  0.53 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSortSimd | 4096 | Random             |    467,273.5 ns |   2,826.38 ns |   1,478.25 ns |  0.10 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **4096** | **SingleElementMoved** |  **4,608,736.1 ns** |   **6,346.26 ns** |   **2,263.14 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 4096 | SingleElementMoved |    324,991.2 ns |   2,366.17 ns |   1,050.59 ns |  0.07 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 4096 | SingleElementMoved |  2,322,141.2 ns |   5,972.11 ns |   2,651.65 ns |  0.50 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 4096 | SingleElementMoved |    202,780.1 ns |   1,673.03 ns |     742.83 ns |  0.04 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **4096** | **Sorted**             |  **4,619,183.7 ns** |  **25,561.18 ns** |  **13,368.99 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 4096 | Sorted             |    283,736.5 ns |   1,312.09 ns |     686.25 ns |  0.06 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 4096 | Sorted             |  1,949,541.7 ns |   5,261.70 ns |   2,336.23 ns |  0.42 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 4096 | Sorted             |    160,405.2 ns |   2,125.13 ns |   1,111.48 ns |  0.03 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **4096** | **Reversed**           |  **4,197,075.6 ns** |  **64,895.47 ns** |  **28,813.99 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| SelectionSortSimd       | 4096 | Reversed           |    487,942.1 ns |   2,191.35 ns |     972.97 ns |  0.12 |    0.00 |    1 |         - |          NA |
| DoubleSelectionSort     | 4096 | Reversed           |  1,973,809.6 ns |  32,592.84 ns |  17,046.69 ns |  0.47 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSortSimd | 4096 | Reversed           |    519,827.8 ns |   2,218.92 ns |   1,160.54 ns |  0.12 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **4096** | **PipeOrgan**          |  **4,631,808.8 ns** |  **31,937.92 ns** |  **16,704.15 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 4096 | PipeOrgan          |    607,172.4 ns |   2,072.05 ns |     920.00 ns |  0.13 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 4096 | PipeOrgan          |  2,373,429.1 ns |  19,496.94 ns |  10,197.28 ns |  0.51 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 4096 | PipeOrgan          |    405,026.6 ns |   4,283.32 ns |   2,240.26 ns |  0.09 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **4096** | **ManyDuplicates**     |  **4,696,598.9 ns** |  **25,530.87 ns** |  **13,353.14 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| SelectionSortSimd       | 4096 | ManyDuplicates     |    516,812.7 ns |   1,231.13 ns |     546.63 ns |  0.11 |    0.00 |    1 |         - |          NA |
| DoubleSelectionSort     | 4096 | ManyDuplicates     |  2,641,385.4 ns |  15,218.33 ns |   7,959.48 ns |  0.56 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSortSimd | 4096 | ManyDuplicates     |    450,144.2 ns |   1,380.79 ns |     613.08 ns |  0.10 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **8192** | **Random**             | **18,760,974.8 ns** |  **60,157.57 ns** |  **31,463.57 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 8192 | Random             |  1,884,340.5 ns |   4,797.09 ns |   2,508.97 ns |  0.10 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 8192 | Random             |  9,700,880.4 ns |  24,059.48 ns |  12,583.58 ns |  0.52 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 8192 | Random             |  1,512,347.9 ns |     924.52 ns |     329.69 ns |  0.08 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **8192** | **SingleElementMoved** | **18,429,510.0 ns** |  **95,732.97 ns** |  **50,070.19 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 8192 | SingleElementMoved |  1,179,946.1 ns |   3,124.52 ns |   1,634.18 ns |  0.06 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 8192 | SingleElementMoved |  9,110,207.1 ns | 367,887.62 ns | 192,412.34 ns |  0.49 |    0.01 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 8192 | SingleElementMoved |    702,078.9 ns |   5,028.27 ns |   2,629.88 ns |  0.04 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **8192** | **Sorted**             | **18,393,130.7 ns** |  **90,552.95 ns** |  **32,292.06 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 8192 | Sorted             |  1,053,198.9 ns |   5,845.34 ns |   2,595.37 ns |  0.06 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 8192 | Sorted             |  7,783,382.7 ns |  37,553.42 ns |  19,641.17 ns |  0.42 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 8192 | Sorted             |    565,232.4 ns |   2,653.71 ns |   1,178.26 ns |  0.03 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **8192** | **Reversed**           | **16,361,795.1 ns** | **112,956.62 ns** |  **50,153.44 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| SelectionSortSimd       | 8192 | Reversed           |  1,792,344.5 ns |   3,112.70 ns |   1,382.06 ns |  0.11 |    0.00 |    1 |         - |          NA |
| DoubleSelectionSort     | 8192 | Reversed           |  7,806,659.1 ns |  39,604.10 ns |  17,584.47 ns |  0.48 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSortSimd | 8192 | Reversed           |  1,954,284.2 ns |  17,132.89 ns |   7,607.11 ns |  0.12 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **8192** | **PipeOrgan**          | **18,364,153.2 ns** | **145,738.33 ns** |  **64,708.72 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 8192 | PipeOrgan          |  2,244,046.6 ns |   2,813.98 ns |   1,249.43 ns |  0.12 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 8192 | PipeOrgan          |  9,295,720.9 ns | 121,250.51 ns |  63,416.36 ns |  0.51 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 8192 | PipeOrgan          |  1,378,343.6 ns |   3,579.01 ns |   1,589.10 ns |  0.08 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **8192** | **ManyDuplicates**     | **18,568,488.8 ns** |  **40,698.37 ns** |  **21,286.03 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 8192 | ManyDuplicates     |  1,845,523.5 ns |   3,230.42 ns |   1,434.33 ns |  0.10 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 8192 | ManyDuplicates     |  9,921,569.4 ns |  38,637.41 ns |  20,208.11 ns |  0.53 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 8192 | ManyDuplicates     |  1,466,416.8 ns |   4,671.09 ns |   2,443.07 ns |  0.08 |    0.00 |    1 |         - |          NA |

### TreeBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.52 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                 | Size | Pattern            | Mean         | Error       | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------- |----- |------------------- |-------------:|------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **BalancedBinaryTreeSort** | **256**  | **Random**             |  **16,455.5 ns** |   **212.97 ns** |   **111.39 ns** |  **1.39** |    **0.01** |    **1** |         **-** |          **NA** |
| BTreeSort              | 256  | Random             |  15,922.4 ns |   250.09 ns |   130.80 ns |  1.35 |    0.02 |    1 |         - |          NA |
| BPlusTreeSort          | 256  | Random             |  15,286.4 ns |   193.77 ns |    86.04 ns |  1.29 |    0.01 |    1 |         - |          NA |
| CartesianTreeSort      | 256  | Random             |  12,856.8 ns |   317.87 ns |   166.25 ns |  1.09 |    0.02 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | Random             |  11,809.1 ns |   195.32 ns |   102.15 ns |  1.00 |    0.00 |    1 |         - |          NA |
| SplaySort              | 256  | Random             |  30,418.0 ns |   329.80 ns |   172.49 ns |  2.58 |    0.03 |    3 |         - |          NA |
| TreapSort              | 256  | Random             |  25,665.8 ns | 6,776.10 ns | 3,544.03 ns |  2.17 |    0.28 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **SingleElementMoved** |   **6,490.7 ns** |   **378.04 ns** |   **197.72 ns** |  **0.12** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 256  | SingleElementMoved |   4,739.7 ns |    78.79 ns |    28.10 ns |  0.09 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 256  | SingleElementMoved |   4,346.9 ns |   289.36 ns |   151.34 ns |  0.08 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 256  | SingleElementMoved |   1,601.2 ns |    13.46 ns |     5.98 ns |  0.03 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | SingleElementMoved |  53,799.2 ns |   460.24 ns |   240.71 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 256  | SingleElementMoved |   3,333.9 ns |    27.37 ns |     9.76 ns |  0.06 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | SingleElementMoved |   4,653.7 ns |   400.66 ns |   209.55 ns |  0.09 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Sorted**             |   **5,994.3 ns** |    **76.40 ns** |    **39.96 ns** |  **0.10** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 256  | Sorted             |   4,514.9 ns |   311.50 ns |   162.92 ns |  0.08 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 256  | Sorted             |   3,973.1 ns |    43.06 ns |    15.36 ns |  0.07 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 256  | Sorted             |   1,540.6 ns |    12.86 ns |     6.72 ns |  0.03 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | Sorted             |  58,610.7 ns |   198.38 ns |   103.76 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 256  | Sorted             |   2,961.5 ns |     9.89 ns |     3.53 ns |  0.05 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | Sorted             |   3,926.0 ns |   136.73 ns |    60.71 ns |  0.07 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Reversed**           |   **5,855.3 ns** |   **202.27 ns** |   **105.79 ns** |  **0.09** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 256  | Reversed           |   7,371.9 ns |   308.60 ns |   161.40 ns |  0.12 |    0.00 |    4 |         - |          NA |
| BPlusTreeSort          | 256  | Reversed           |   6,868.3 ns |    66.58 ns |    29.56 ns |  0.11 |    0.00 |    4 |         - |          NA |
| CartesianTreeSort      | 256  | Reversed           |   1,512.5 ns |    35.73 ns |    12.74 ns |  0.02 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | Reversed           |  61,799.1 ns |   270.37 ns |   141.41 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 256  | Reversed           |   2,880.0 ns |    38.36 ns |    20.06 ns |  0.05 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | Reversed           |   3,980.7 ns |   292.77 ns |   153.12 ns |  0.06 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **PipeOrgan**          |   **5,235.7 ns** |   **112.30 ns** |    **58.73 ns** |  **0.18** |    **0.00** |    **3** |         **-** |          **NA** |
| BTreeSort              | 256  | PipeOrgan          |   5,371.3 ns |   266.32 ns |   139.29 ns |  0.18 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 256  | PipeOrgan          |   4,970.7 ns |   213.29 ns |   111.56 ns |  0.17 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 256  | PipeOrgan          |   1,692.0 ns |    11.47 ns |     5.09 ns |  0.06 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | PipeOrgan          |  29,077.6 ns |   229.31 ns |   101.81 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 256  | PipeOrgan          |   3,579.9 ns |    28.16 ns |    10.04 ns |  0.12 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | PipeOrgan          |   5,438.5 ns |   427.31 ns |   223.49 ns |  0.19 |    0.01 |    3 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **ManyDuplicates**     |  **15,657.2 ns** |    **89.55 ns** |    **46.84 ns** |  **1.48** |    **0.02** |    **2** |         **-** |          **NA** |
| BTreeSort              | 256  | ManyDuplicates     |  15,418.1 ns |   270.05 ns |   141.24 ns |  1.45 |    0.02 |    2 |         - |          NA |
| BPlusTreeSort          | 256  | ManyDuplicates     |  14,693.1 ns |   163.92 ns |    72.78 ns |  1.38 |    0.02 |    2 |         - |          NA |
| CartesianTreeSort      | 256  | ManyDuplicates     |  15,641.8 ns |   189.00 ns |    83.92 ns |  1.47 |    0.02 |    2 |         - |          NA |
| BinaryTreeSort         | 256  | ManyDuplicates     |  10,612.4 ns |   252.24 ns |   131.93 ns |  1.00 |    0.00 |    1 |         - |          NA |
| SplaySort              | 256  | ManyDuplicates     |  28,313.4 ns |   378.42 ns |   168.02 ns |  2.67 |    0.04 |    3 |         - |          NA |
| TreapSort              | 256  | ManyDuplicates     |  17,696.4 ns |   188.10 ns |    98.38 ns |  1.67 |    0.02 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Random**             |  **74,443.2 ns** |   **398.52 ns** |   **208.43 ns** |  **1.36** |    **0.00** |    **1** |         **-** |          **NA** |
| BTreeSort              | 1024 | Random             |  75,522.9 ns |   359.01 ns |   187.77 ns |  1.38 |    0.00 |    1 |         - |          NA |
| BPlusTreeSort          | 1024 | Random             |  73,346.0 ns |   900.56 ns |   471.01 ns |  1.34 |    0.01 |    1 |         - |          NA |
| CartesianTreeSort      | 1024 | Random             |  64,101.8 ns |   321.04 ns |   142.54 ns |  1.17 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | Random             |  54,907.3 ns |   261.96 ns |   116.31 ns |  1.00 |    0.00 |    1 |         - |          NA |
| SplaySort              | 1024 | Random             | 155,020.2 ns |   730.45 ns |   324.32 ns |  2.82 |    0.01 |    2 |         - |          NA |
| TreapSort              | 1024 | Random             |  81,113.1 ns |   382.11 ns |   136.26 ns |  1.48 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **SingleElementMoved** |  **28,160.0 ns** |   **239.24 ns** |   **106.22 ns** | **0.033** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 1024 | SingleElementMoved |  21,090.5 ns |   257.02 ns |   134.43 ns | 0.025 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 1024 | SingleElementMoved |  19,649.3 ns |   244.37 ns |   108.50 ns | 0.023 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 1024 | SingleElementMoved |   6,246.4 ns |   270.97 ns |   141.73 ns | 0.007 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | SingleElementMoved | 850,905.1 ns | 7,548.71 ns | 3,948.12 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | SingleElementMoved |  13,178.0 ns |   222.93 ns |   116.59 ns | 0.015 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | SingleElementMoved |  19,644.7 ns |   361.02 ns |   160.29 ns | 0.023 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Sorted**             |  **27,453.8 ns** |   **203.70 ns** |   **106.54 ns** | **0.029** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 1024 | Sorted             |  20,208.3 ns |   179.52 ns |    79.71 ns | 0.022 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 1024 | Sorted             |  18,844.3 ns |   131.06 ns |    58.19 ns | 0.020 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 1024 | Sorted             |   6,075.9 ns |   272.07 ns |   142.30 ns | 0.007 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | Sorted             | 932,398.4 ns | 6,268.09 ns | 3,278.33 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | Sorted             |  12,008.4 ns |   236.06 ns |   123.46 ns | 0.013 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | Sorted             |  17,599.2 ns |   498.22 ns |   260.58 ns | 0.019 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Reversed**           |  **26,896.5 ns** |   **737.92 ns** |   **327.64 ns** | **0.027** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 1024 | Reversed           |  31,416.9 ns |   224.07 ns |    99.49 ns | 0.031 |    0.00 |    4 |         - |          NA |
| BPlusTreeSort          | 1024 | Reversed           |  28,481.1 ns |   322.75 ns |   143.30 ns | 0.029 |    0.00 |    4 |         - |          NA |
| CartesianTreeSort      | 1024 | Reversed           |   6,047.0 ns |   223.89 ns |   117.10 ns | 0.006 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | Reversed           | 997,479.6 ns | 1,829.62 ns |   956.93 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | Reversed           |  11,640.0 ns |   984.98 ns |   437.34 ns | 0.012 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | Reversed           |  17,132.7 ns |   430.36 ns |   225.08 ns | 0.017 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **PipeOrgan**          |  **23,892.6 ns** |   **833.38 ns** |   **435.87 ns** |  **0.05** |    **0.00** |    **3** |         **-** |          **NA** |
| BTreeSort              | 1024 | PipeOrgan          |  23,152.8 ns |   312.40 ns |   138.71 ns |  0.05 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 1024 | PipeOrgan          |  21,826.0 ns |   455.26 ns |   238.11 ns |  0.05 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 1024 | PipeOrgan          |   6,658.5 ns |   297.80 ns |   155.75 ns |  0.01 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | PipeOrgan          | 468,986.2 ns | 1,210.53 ns |   633.13 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | PipeOrgan          |  13,419.8 ns |   285.12 ns |   149.12 ns |  0.03 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | PipeOrgan          |  24,916.8 ns |   684.56 ns |   358.04 ns |  0.05 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **ManyDuplicates**     |  **65,367.9 ns** |   **328.63 ns** |   **145.91 ns** |  **1.12** |    **0.01** |    **1** |         **-** |          **NA** |
| BTreeSort              | 1024 | ManyDuplicates     |  68,335.3 ns |   306.68 ns |   160.40 ns |  1.17 |    0.01 |    1 |         - |          NA |
| BPlusTreeSort          | 1024 | ManyDuplicates     |  65,523.9 ns |   344.28 ns |   180.07 ns |  1.12 |    0.01 |    1 |         - |          NA |
| CartesianTreeSort      | 1024 | ManyDuplicates     |  76,707.3 ns |   662.69 ns |   346.60 ns |  1.32 |    0.01 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | ManyDuplicates     |  58,299.8 ns |   640.89 ns |   335.20 ns |  1.00 |    0.00 |    1 |         - |          NA |
| SplaySort              | 1024 | ManyDuplicates     | 122,922.6 ns |   570.22 ns |   253.18 ns |  2.11 |    0.01 |    2 |         - |          NA |
| TreapSort              | 1024 | ManyDuplicates     |  78,107.8 ns |   241.10 ns |   107.05 ns |  1.34 |    0.01 |    1 |         - |          NA |

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
- [Bingo Sort](./src/SortAlgorithm/Algorithms/Selection/BingoSort.cs)
- [Cycle Sort](./src/SortAlgorithm/Algorithms/Selection/CycleSort.cs)
- [Double Selection Sort](./src/SortAlgorithm/Algorithms/Selection/DoubleSelectionSort.cs)
- [Double Selection Sort (SIMD)](./src/SortAlgorithm/Algorithms/Selection/DoubleSelectionSortSimd.cs)
- [Pancake Sort](./src/SortAlgorithm/Algorithms/Selection/PancakeSort.cs)
- [Selection Sort](./src/SortAlgorithm/Algorithms/Selection/SelectionSort.cs)
- [Selection Sort (SIMD)](./src/SortAlgorithm/Algorithms/Selection/SelectionSortSimd.cs)

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
- [Drop-Merge Sort](./src/SortAlgorithm/Algorithms/Merge/DropMergeSort.cs)
- [Flat Stable Sort](./src/SortAlgorithm/Algorithms/Merge/FlatStableSort.cs)
- [Glidesort](./src/SortAlgorithm/Algorithms/Merge/Glidesort.cs)
- [Merge Sort](./src/SortAlgorithm/Algorithms/Merge/MergeSort.cs)
- [Natural Merge Sort](./src/SortAlgorithm/Algorithms/Merge/NaturalMergeSort.cs)
- [Patience Sort](./src/SortAlgorithm/Algorithms/Merge/PatienceSort.cs)
- [Pingpong Merge Sort](./src/SortAlgorithm/Algorithms/Merge/PingpongMergeSort.cs)
- [Power Sort](./src/SortAlgorithm/Algorithms/Merge/PowerSort.cs)
- [Quad Sort](./src/SortAlgorithm/Algorithms/Merge/QuadSort.cs)
- [Rotate Merge Sort](./src/SortAlgorithm/Algorithms/Merge/RotateMergeSort.cs)
  - Iterative
  - Recursive
- [Shift Sort](./src/SortAlgorithm/Algorithms/Merge/ShiftSort.cs)
- [Spin Sort](./src/SortAlgorithm/Algorithms/Merge/SpinSort.cs)
- [std::stable_sort (LLVM)](./src/SortAlgorithm/Algorithms/Merge/StdStableSort.cs)
- [Strand Sort](./src/SortAlgorithm/Algorithms/Merge/StrandSort.cs)
- [SymMerge Sort](./src/SortAlgorithm/Algorithms/Merge/SymMergeSort.cs)
- [Tim Sort](./src/SortAlgorithm/Algorithms/Merge/TimSort.cs)

### Heap
- [Binomial Heap Sort](./src/SortAlgorithm/Algorithms/Heap/BinomialHeapSort.cs)
- [Bottom-Up Heap Sort](./src/SortAlgorithm/Algorithms/Heap/BottomupHeapSort.cs)
- [Heap Sort](./src/SortAlgorithm/Algorithms/Heap/HeapSort.cs)
- [Min-Heap Sort](./src/SortAlgorithm/Algorithms/Heap/MinHeapSort.cs)
- [Pairing Heap Sort](./src/SortAlgorithm/Algorithms/Heap/PairingHeapSort.cs)
- [Poplar Heap Sort](./src/SortAlgorithm/Algorithms/Heap/PoplarHeapSort.cs)
- [Smooth Sort](./src/SortAlgorithm/Algorithms/Heap/SmoothSort.cs)
- [Ternary Heap Sort](./src/SortAlgorithm/Algorithms/Heap/TernaryHeapSort.cs)
- [Tournament Sort](./src/SortAlgorithm/Algorithms/Heap/TournamentSort.cs)
- [Weak Heap Sort](./src/SortAlgorithm/Algorithms/Heap/WeakHeapSort.cs)

### Partition
- [Quick Sort (Bidirectional Stable)](./src/SortAlgorithm/Algorithms/Partition/BidirectionalStableQuickSort.cs)
- [Block Quick Sort](./src/SortAlgorithm/Algorithms/Partition/BlockQuickSort.cs)
- [Quick Sort (Destswap Stable)](./src/SortAlgorithm/Algorithms/Partition/DestswapStableQuickSort.cs)
- [Quick Sort (Dual Pivot)](./src/SortAlgorithm/Algorithms/Partition/DualPivotQuickSort.cs)
- [Flux Sort](./src/SortAlgorithm/Algorithms/Partition/FluxSort.cs)
- [Intro Sort](./src/SortAlgorithm/Algorithms/Partition/IntroSort.cs)
- [Intro Sort (Dotnet)](./src/SortAlgorithm/Algorithms/Partition/IntroSortDotnet.cs)
- [Ipnsort](./src/SortAlgorithm/Algorithms/Partition/Ipnsort.cs)
- [Pattern-Defeating Quick Sort](./src/SortAlgorithm/Algorithms/Partition/PDQSort.cs)
- [PDQ Sort Branchless](./src/SortAlgorithm/Algorithms/Partition/PDQSortBranchless.cs)
- [Quick Sort](./src/SortAlgorithm/Algorithms/Partition/QuickSort.cs)
- [Quick Sort (3-Way)](./src/SortAlgorithm/Algorithms/Partition/QuickSort3way.cs)
- [Quick Sort (Median of 3)](./src/SortAlgorithm/Algorithms/Partition/QuickSortMedian3.cs)
- [Quick Sort (Median of 9)](./src/SortAlgorithm/Algorithms/Partition/QuickSortMedian9.cs)
- [Sample Sort](./src/SortAlgorithm/Algorithms/Partition/SampleSort.cs)
- [Quick Sort (Stable)](./src/SortAlgorithm/Algorithms/Partition/StableQuickSort.cs)
- [std::sort (LLVM)](./src/SortAlgorithm/Algorithms/Partition/StdSort.cs)

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
- [Radsort](./src/SortAlgorithm/Algorithms/Distribution/Radsort.cs)
- [Rotate LSD Radix Sort (Base 4)](./src/SortAlgorithm/Algorithms/Distribution/RotateLSDRadixSort.cs)
- [Spread Sort](./src/SortAlgorithm/Algorithms/Distribution/SpreadSort.cs)

### Network
- [Batcher Odd-Even Merge Sort](./src/SortAlgorithm/Algorithms/Network/BatcherOddEvenMergeSort.cs)
- [Batcher Odd-Even Merge Sort Branchless](./src/SortAlgorithm/Algorithms/Network/BatcherOddEvenMergeSortBranchless.cs)
- [Bitonic Sort](./src/SortAlgorithm/Algorithms/Network/BitonicSort.cs)
  - Iterative
  - Recursive
- [Bitonic Sort Branchless](./src/SortAlgorithm/Algorithms/Network/BitonicSortBranchless.cs)
- [Pairwise Sorting Network](./src/SortAlgorithm/Algorithms/Network/PairwiseSortingNetwork.cs)
- [Pairwise Sorting Network Branchless](./src/SortAlgorithm/Algorithms/Network/PairwiseSortingNetworkBranchless.cs)

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
- [Bozo Sort](./src/SortAlgorithm/Algorithms/Joke/BozoSort.cs)
- [Fun Sort](./src/SortAlgorithm/Algorithms/Joke/FunSort.cs)
- [Slow Sort](./src/SortAlgorithm/Algorithms/Joke/SlowSort.cs)
- [Stooge Sort](./src/SortAlgorithm/Algorithms/Joke/StoogeSort.cs)
<!-- ALGORITHMS_END -->
