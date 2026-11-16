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
<summary>Benchmark results (2026-11-16 17:13 UTC)</summary>

Workflow run: https://github.com/guitarrapc/SortAlgorithms/actions/runs/43912034337

### DistributionBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V45 4.27GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.48 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method              | Size | Pattern            | Mean           | Error        | StdDev       | Ratio  | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------------- |----- |------------------- |---------------:|-------------:|-------------:|-------:|--------:|-----:|----------:|------------:|
| **CountingSort**        | **256**  | **Random**             |     **1,093.6 ns** |     **13.62 ns** |      **4.86 ns** |   **1.71** |    **0.02** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | Random             |       641.3 ns |     19.60 ns |      8.70 ns |   1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 256  | Random             |     1,556.7 ns |     37.97 ns |     19.86 ns |   2.43 |    0.04 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | Random             |       543.8 ns |     27.95 ns |     12.41 ns |   0.85 |    0.02 |    1 |         - |          NA |
| BucketSort          | 256  | Random             |     1,917.4 ns |    404.08 ns |    211.34 ns |   2.99 |    0.31 |    3 |         - |          NA |
| BucketSortInteger   | 256  | Random             |       995.5 ns |     19.76 ns |      8.78 ns |   1.55 |    0.02 |    2 |         - |          NA |
| FlashSort           | 256  | Random             |     7,912.9 ns |    206.96 ns |    108.24 ns |  12.34 |    0.22 |    7 |         - |          NA |
| RadixLSD4Sort       | 256  | Random             |     1,750.3 ns |    258.64 ns |    135.27 ns |   2.73 |    0.20 |    3 |         - |          NA |
| RotateLSDRadixSort  | 256  | Random             |    27,187.2 ns |    938.47 ns |    416.69 ns |  42.40 |    0.81 |    8 |         - |          NA |
| RadixLSD256Sort     | 256  | Random             |       699.3 ns |     27.67 ns |      9.87 ns |   1.09 |    0.02 |    1 |         - |          NA |
| Radsort             | 256  | Random             |     5,500.7 ns |    276.89 ns |    144.82 ns |   8.58 |    0.24 |    6 |         - |          NA |
| RadixLSD10Sort      | 256  | Random             |     2,732.0 ns |    236.23 ns |    123.55 ns |   4.26 |    0.19 |    4 |         - |          NA |
| RadixMSD4Sort       | 256  | Random             |     3,702.8 ns |    154.29 ns |     80.70 ns |   5.77 |    0.14 |    5 |         - |          NA |
| RadixMSD10Sort      | 256  | Random             |     4,074.7 ns |    378.32 ns |    197.87 ns |   6.35 |    0.30 |    5 |         - |          NA |
| AmericanFlagSort    | 256  | Random             |     1,713.6 ns |     36.12 ns |     12.88 ns |   2.67 |    0.04 |    3 |         - |          NA |
| SpreadSort          | 256  | Random             |     1,028.0 ns |    288.49 ns |    128.09 ns |   1.60 |    0.19 |    2 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **256**  | **SingleElementMoved** |     **1,111.1 ns** |     **34.09 ns** |     **17.83 ns** |   **1.79** |    **0.03** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | SingleElementMoved |       620.7 ns |     11.60 ns |      6.07 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 256  | SingleElementMoved |       986.8 ns |     31.87 ns |     16.67 ns |   1.59 |    0.03 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | SingleElementMoved |       487.3 ns |     80.98 ns |     42.35 ns |   0.79 |    0.06 |    1 |         - |          NA |
| BucketSort          | 256  | SingleElementMoved |     1,316.3 ns |     19.54 ns |      8.67 ns |   2.12 |    0.02 |    3 |         - |          NA |
| BucketSortInteger   | 256  | SingleElementMoved |       960.4 ns |     20.61 ns |      9.15 ns |   1.55 |    0.02 |    3 |         - |          NA |
| FlashSort           | 256  | SingleElementMoved |    10,952.5 ns |    204.64 ns |    107.03 ns |  17.65 |    0.23 |    8 |         - |          NA |
| RadixLSD4Sort       | 256  | SingleElementMoved |     1,728.8 ns |    120.81 ns |     63.18 ns |   2.79 |    0.10 |    4 |         - |          NA |
| RotateLSDRadixSort  | 256  | SingleElementMoved |    30,547.2 ns |    498.20 ns |    221.20 ns |  49.22 |    0.56 |    9 |         - |          NA |
| RadixLSD256Sort     | 256  | SingleElementMoved |       699.8 ns |     28.61 ns |     14.96 ns |   1.13 |    0.02 |    2 |         - |          NA |
| Radsort             | 256  | SingleElementMoved |     5,212.8 ns |    285.66 ns |    149.41 ns |   8.40 |    0.24 |    7 |         - |          NA |
| RadixLSD10Sort      | 256  | SingleElementMoved |     2,675.6 ns |    219.30 ns |    114.70 ns |   4.31 |    0.18 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | SingleElementMoved |     4,133.3 ns |     39.14 ns |     17.38 ns |   6.66 |    0.07 |    6 |         - |          NA |
| RadixMSD10Sort      | 256  | SingleElementMoved |     4,013.9 ns |    311.01 ns |    162.67 ns |   6.47 |    0.25 |    6 |         - |          NA |
| AmericanFlagSort    | 256  | SingleElementMoved |     1,425.8 ns |     25.51 ns |     11.33 ns |   2.30 |    0.03 |    3 |         - |          NA |
| SpreadSort          | 256  | SingleElementMoved |       646.7 ns |     50.13 ns |     22.26 ns |   1.04 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **256**  | **Sorted**             |     **1,048.1 ns** |     **91.33 ns** |     **40.55 ns** |   **1.79** |    **0.07** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 256  | Sorted             |       585.1 ns |      7.51 ns |      3.34 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 256  | Sorted             |       954.4 ns |     35.44 ns |     12.64 ns |   1.63 |    0.02 |    4 |         - |          NA |
| PigeonSortInteger   | 256  | Sorted             |       438.7 ns |      7.55 ns |      3.35 ns |   0.75 |    0.01 |    2 |         - |          NA |
| BucketSort          | 256  | Sorted             |     1,302.2 ns |     29.59 ns |     13.14 ns |   2.23 |    0.02 |    5 |         - |          NA |
| BucketSortInteger   | 256  | Sorted             |       900.5 ns |     26.17 ns |     11.62 ns |   1.54 |    0.02 |    4 |         - |          NA |
| FlashSort           | 256  | Sorted             |    10,658.0 ns |    120.49 ns |     63.02 ns |  18.22 |    0.14 |    9 |         - |          NA |
| RadixLSD4Sort       | 256  | Sorted             |     1,574.5 ns |      7.72 ns |      2.75 ns |   2.69 |    0.02 |    6 |         - |          NA |
| RotateLSDRadixSort  | 256  | Sorted             |    25,490.1 ns |    289.38 ns |    151.35 ns |  43.56 |    0.34 |   10 |         - |          NA |
| RadixLSD256Sort     | 256  | Sorted             |       659.8 ns |     13.02 ns |      6.81 ns |   1.13 |    0.01 |    3 |         - |          NA |
| Radsort             | 256  | Sorted             |     4,957.0 ns |    253.74 ns |    112.66 ns |   8.47 |    0.19 |    8 |         - |          NA |
| RadixLSD10Sort      | 256  | Sorted             |     2,500.0 ns |    188.17 ns |     98.42 ns |   4.27 |    0.16 |    7 |         - |          NA |
| RadixMSD4Sort       | 256  | Sorted             |     4,377.5 ns |    135.01 ns |     59.95 ns |   7.48 |    0.10 |    8 |         - |          NA |
| RadixMSD10Sort      | 256  | Sorted             |     3,879.7 ns |    278.37 ns |    145.59 ns |   6.63 |    0.24 |    8 |         - |          NA |
| AmericanFlagSort    | 256  | Sorted             |       934.8 ns |      5.63 ns |      2.50 ns |   1.60 |    0.01 |    4 |         - |          NA |
| SpreadSort          | 256  | Sorted             |       220.5 ns |      8.90 ns |      4.66 ns |   0.38 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **256**  | **Reversed**           |       **993.1 ns** |     **23.52 ns** |     **12.30 ns** |   **1.76** |    **0.03** |    **5** |         **-** |          **NA** |
| CountingSortInteger | 256  | Reversed           |       565.9 ns |     12.35 ns |      5.48 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 256  | Reversed           |       976.8 ns |     17.04 ns |      7.56 ns |   1.73 |    0.02 |    5 |         - |          NA |
| PigeonSortInteger   | 256  | Reversed           |       429.8 ns |     37.84 ns |     19.79 ns |   0.76 |    0.03 |    2 |         - |          NA |
| BucketSort          | 256  | Reversed           |     1,260.6 ns |     36.86 ns |     16.37 ns |   2.23 |    0.03 |    6 |         - |          NA |
| BucketSortInteger   | 256  | Reversed           |       926.7 ns |     85.10 ns |     44.51 ns |   1.64 |    0.08 |    5 |         - |          NA |
| FlashSort           | 256  | Reversed           |     9,300.6 ns |     95.62 ns |     50.01 ns |  16.44 |    0.17 |   11 |         - |          NA |
| RadixLSD4Sort       | 256  | Reversed           |     1,634.6 ns |     18.81 ns |      8.35 ns |   2.89 |    0.03 |    7 |         - |          NA |
| RotateLSDRadixSort  | 256  | Reversed           |    26,612.5 ns |    313.11 ns |    163.76 ns |  47.03 |    0.50 |   12 |         - |          NA |
| RadixLSD256Sort     | 256  | Reversed           |       740.4 ns |    195.47 ns |     86.79 ns |   1.31 |    0.14 |    4 |         - |          NA |
| Radsort             | 256  | Reversed           |     5,135.5 ns |    436.90 ns |    193.99 ns |   9.08 |    0.33 |   10 |         - |          NA |
| RadixLSD10Sort      | 256  | Reversed           |     2,471.3 ns |     48.53 ns |     21.55 ns |   4.37 |    0.05 |    8 |         - |          NA |
| RadixMSD4Sort       | 256  | Reversed           |     3,962.7 ns |     39.61 ns |     14.12 ns |   7.00 |    0.07 |    9 |         - |          NA |
| RadixMSD10Sort      | 256  | Reversed           |     3,824.9 ns |    216.26 ns |    113.11 ns |   6.76 |    0.20 |    9 |         - |          NA |
| AmericanFlagSort    | 256  | Reversed           |     1,018.4 ns |     40.24 ns |     17.86 ns |   1.80 |    0.03 |    5 |         - |          NA |
| SpreadSort          | 256  | Reversed           |       288.7 ns |     12.28 ns |      5.45 ns |   0.51 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **256**  | **PipeOrgan**          |     **1,062.5 ns** |     **22.96 ns** |     **12.01 ns** |   **1.74** |    **0.06** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | PipeOrgan          |       612.3 ns |     45.98 ns |     20.42 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 256  | PipeOrgan          |       957.8 ns |     42.47 ns |     18.86 ns |   1.57 |    0.06 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | PipeOrgan          |       487.2 ns |     20.70 ns |      9.19 ns |   0.80 |    0.03 |    1 |         - |          NA |
| BucketSort          | 256  | PipeOrgan          |     1,373.4 ns |     81.98 ns |     42.88 ns |   2.25 |    0.10 |    4 |         - |          NA |
| BucketSortInteger   | 256  | PipeOrgan          |       942.6 ns |     13.39 ns |      4.77 ns |   1.54 |    0.05 |    3 |         - |          NA |
| FlashSort           | 256  | PipeOrgan          |     9,318.0 ns |    248.65 ns |    130.05 ns |  15.23 |    0.51 |    8 |         - |          NA |
| RadixLSD4Sort       | 256  | PipeOrgan          |     1,740.7 ns |      8.92 ns |      3.18 ns |   2.85 |    0.09 |    4 |         - |          NA |
| RotateLSDRadixSort  | 256  | PipeOrgan          |    26,413.6 ns |    243.64 ns |    127.43 ns |  43.18 |    1.34 |    9 |         - |          NA |
| RadixLSD256Sort     | 256  | PipeOrgan          |       695.4 ns |     25.68 ns |     11.40 ns |   1.14 |    0.04 |    2 |         - |          NA |
| Radsort             | 256  | PipeOrgan          |     5,247.7 ns |    114.36 ns |     50.78 ns |   8.58 |    0.27 |    7 |         - |          NA |
| RadixLSD10Sort      | 256  | PipeOrgan          |     2,490.7 ns |     58.71 ns |     30.71 ns |   4.07 |    0.13 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | PipeOrgan          |     3,959.4 ns |    124.79 ns |     55.41 ns |   6.47 |    0.22 |    6 |         - |          NA |
| RadixMSD10Sort      | 256  | PipeOrgan          |     3,975.0 ns |    177.47 ns |     92.82 ns |   6.50 |    0.25 |    6 |         - |          NA |
| AmericanFlagSort    | 256  | PipeOrgan          |     1,510.9 ns |    256.18 ns |    133.99 ns |   2.47 |    0.22 |    4 |         - |          NA |
| SpreadSort          | 256  | PipeOrgan          |       967.9 ns |     97.45 ns |     50.97 ns |   1.58 |    0.09 |    3 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **256**  | **ManyDuplicates**     |     **1,010.8 ns** |     **31.21 ns** |     **13.86 ns** |   **1.97** |    **0.03** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 256  | ManyDuplicates     |       512.8 ns |      4.14 ns |      1.84 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 256  | ManyDuplicates     |       987.7 ns |     31.74 ns |     16.60 ns |   1.93 |    0.03 |    4 |         - |          NA |
| PigeonSortInteger   | 256  | ManyDuplicates     |       360.7 ns |     10.76 ns |      4.78 ns |   0.70 |    0.01 |    1 |         - |          NA |
| BucketSort          | 256  | ManyDuplicates     |     1,907.4 ns |     36.80 ns |     19.25 ns |   3.72 |    0.04 |    6 |         - |          NA |
| BucketSortInteger   | 256  | ManyDuplicates     |     1,085.9 ns |     30.09 ns |     10.73 ns |   2.12 |    0.02 |    4 |         - |          NA |
| FlashSort           | 256  | ManyDuplicates     |     7,672.6 ns |    273.72 ns |    143.16 ns |  14.96 |    0.27 |    9 |         - |          NA |
| RadixLSD4Sort       | 256  | ManyDuplicates     |     1,362.2 ns |     10.74 ns |      4.77 ns |   2.66 |    0.01 |    5 |         - |          NA |
| RotateLSDRadixSort  | 256  | ManyDuplicates     |    20,962.3 ns |    722.73 ns |    378.00 ns |  40.88 |    0.71 |   10 |         - |          NA |
| RadixLSD256Sort     | 256  | ManyDuplicates     |       698.9 ns |      7.83 ns |      3.48 ns |   1.36 |    0.01 |    3 |         - |          NA |
| Radsort             | 256  | ManyDuplicates     |     5,837.1 ns |    443.90 ns |    197.09 ns |  11.38 |    0.36 |    8 |         - |          NA |
| RadixLSD10Sort      | 256  | ManyDuplicates     |     1,783.9 ns |     16.30 ns |      7.24 ns |   3.48 |    0.02 |    6 |         - |          NA |
| RadixMSD4Sort       | 256  | ManyDuplicates     |     1,973.9 ns |     38.81 ns |     17.23 ns |   3.85 |    0.03 |    6 |         - |          NA |
| RadixMSD10Sort      | 256  | ManyDuplicates     |     2,421.5 ns |     46.54 ns |     20.66 ns |   4.72 |    0.04 |    7 |         - |          NA |
| AmericanFlagSort    | 256  | ManyDuplicates     |     1,752.9 ns |     17.81 ns |      6.35 ns |   3.42 |    0.02 |    6 |         - |          NA |
| SpreadSort          | 256  | ManyDuplicates     |       780.8 ns |    246.02 ns |    109.23 ns |   1.52 |    0.20 |    3 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **1024** | **Random**             |     **3,810.1 ns** |     **53.40 ns** |     **23.71 ns** |   **1.81** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Random             |     2,110.4 ns |     38.09 ns |     16.91 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | Random             |     3,284.1 ns |     49.86 ns |     22.14 ns |   1.56 |    0.02 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | Random             |     1,683.0 ns |     58.96 ns |     26.18 ns |   0.80 |    0.01 |    1 |         - |          NA |
| BucketSort          | 1024 | Random             |     5,026.8 ns |    278.99 ns |    145.92 ns |   2.38 |    0.07 |    4 |         - |          NA |
| BucketSortInteger   | 1024 | Random             |     3,364.8 ns |     41.63 ns |     18.48 ns |   1.59 |    0.01 |    3 |         - |          NA |
| FlashSort           | 1024 | Random             |    32,886.6 ns |    796.77 ns |    353.77 ns |  15.58 |    0.19 |    8 |         - |          NA |
| RadixLSD4Sort       | 1024 | Random             |     7,585.5 ns |    292.42 ns |    152.94 ns |   3.59 |    0.07 |    5 |         - |          NA |
| RotateLSDRadixSort  | 1024 | Random             |   142,597.5 ns |  4,668.08 ns |  2,441.50 ns |  67.57 |    1.20 |    9 |         - |          NA |
| RadixLSD256Sort     | 1024 | Random             |     3,804.5 ns |    680.84 ns |    302.30 ns |   1.80 |    0.13 |    3 |         - |          NA |
| Radsort             | 1024 | Random             |    14,378.9 ns |    122.07 ns |     43.53 ns |   6.81 |    0.05 |    6 |         - |          NA |
| RadixLSD10Sort      | 1024 | Random             |    12,874.3 ns |    170.79 ns |     75.83 ns |   6.10 |    0.06 |    6 |         - |          NA |
| RadixMSD4Sort       | 1024 | Random             |    14,968.5 ns |    259.82 ns |    135.89 ns |   7.09 |    0.08 |    6 |         - |          NA |
| RadixMSD10Sort      | 1024 | Random             |    19,160.6 ns |    420.99 ns |    220.19 ns |   9.08 |    0.12 |    7 |         - |          NA |
| AmericanFlagSort    | 1024 | Random             |    11,075.7 ns |    190.98 ns |     84.79 ns |   5.25 |    0.05 |    6 |         - |          NA |
| SpreadSort          | 1024 | Random             |     5,154.6 ns |    147.37 ns |     65.43 ns |   2.44 |    0.03 |    4 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **1024** | **SingleElementMoved** |     **3,869.1 ns** |    **248.63 ns** |    **130.04 ns** |   **1.87** |    **0.06** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | SingleElementMoved |     2,073.3 ns |     29.13 ns |     10.39 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | SingleElementMoved |     3,431.9 ns |    436.16 ns |    228.12 ns |   1.66 |    0.10 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | SingleElementMoved |     1,672.0 ns |     35.27 ns |     12.58 ns |   0.81 |    0.01 |    1 |         - |          NA |
| BucketSort          | 1024 | SingleElementMoved |     4,711.7 ns |    242.79 ns |    126.99 ns |   2.27 |    0.06 |    4 |         - |          NA |
| BucketSortInteger   | 1024 | SingleElementMoved |     3,060.1 ns |     22.15 ns |      7.90 ns |   1.48 |    0.01 |    3 |         - |          NA |
| FlashSort           | 1024 | SingleElementMoved |    40,987.6 ns |    334.37 ns |    148.46 ns |  19.77 |    0.11 |    8 |         - |          NA |
| RadixLSD4Sort       | 1024 | SingleElementMoved |     7,973.2 ns |    180.82 ns |     94.57 ns |   3.85 |    0.05 |    5 |         - |          NA |
| RotateLSDRadixSort  | 1024 | SingleElementMoved |   129,746.0 ns |  1,844.07 ns |    964.49 ns |  62.58 |    0.53 |    9 |         - |          NA |
| RadixLSD256Sort     | 1024 | SingleElementMoved |     3,715.0 ns |     22.97 ns |     10.20 ns |   1.79 |    0.01 |    3 |         - |          NA |
| Radsort             | 1024 | SingleElementMoved |    12,902.5 ns |    292.98 ns |    153.24 ns |   6.22 |    0.08 |    6 |         - |          NA |
| RadixLSD10Sort      | 1024 | SingleElementMoved |    12,501.4 ns |    283.61 ns |    148.34 ns |   6.03 |    0.07 |    6 |         - |          NA |
| RadixMSD4Sort       | 1024 | SingleElementMoved |    17,032.4 ns |    178.73 ns |     93.48 ns |   8.22 |    0.06 |    7 |         - |          NA |
| RadixMSD10Sort      | 1024 | SingleElementMoved |    18,572.8 ns |    278.62 ns |    145.72 ns |   8.96 |    0.08 |    7 |         - |          NA |
| AmericanFlagSort    | 1024 | SingleElementMoved |     7,749.1 ns |    182.68 ns |     95.55 ns |   3.74 |    0.05 |    5 |         - |          NA |
| SpreadSort          | 1024 | SingleElementMoved |     3,901.8 ns |    292.48 ns |    129.86 ns |   1.88 |    0.06 |    3 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **1024** | **Sorted**             |     **3,532.8 ns** |     **46.27 ns** |     **16.50 ns** |   **1.74** |    **0.01** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Sorted             |     2,031.5 ns |     15.24 ns |      5.43 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 1024 | Sorted             |     3,320.7 ns |     51.60 ns |     18.40 ns |   1.63 |    0.01 |    4 |         - |          NA |
| PigeonSortInteger   | 1024 | Sorted             |     1,544.1 ns |    111.92 ns |     49.69 ns |   0.76 |    0.02 |    2 |         - |          NA |
| BucketSort          | 1024 | Sorted             |     4,714.2 ns |    207.87 ns |    108.72 ns |   2.32 |    0.05 |    5 |         - |          NA |
| BucketSortInteger   | 1024 | Sorted             |     3,134.9 ns |    426.89 ns |    189.54 ns |   1.54 |    0.09 |    4 |         - |          NA |
| FlashSort           | 1024 | Sorted             |    43,358.1 ns |    293.47 ns |    130.30 ns |  21.34 |    0.08 |   10 |         - |          NA |
| RadixLSD4Sort       | 1024 | Sorted             |     7,268.2 ns |    179.50 ns |     93.88 ns |   3.58 |    0.04 |    7 |         - |          NA |
| RotateLSDRadixSort  | 1024 | Sorted             |   125,388.0 ns |  3,807.66 ns |  1,690.63 ns |  61.72 |    0.79 |   11 |         - |          NA |
| RadixLSD256Sort     | 1024 | Sorted             |     3,687.0 ns |     34.35 ns |     15.25 ns |   1.81 |    0.01 |    4 |         - |          NA |
| Radsort             | 1024 | Sorted             |    12,737.8 ns |    310.91 ns |    162.61 ns |   6.27 |    0.08 |    8 |         - |          NA |
| RadixLSD10Sort      | 1024 | Sorted             |    12,498.7 ns |    192.89 ns |    100.88 ns |   6.15 |    0.05 |    8 |         - |          NA |
| RadixMSD4Sort       | 1024 | Sorted             |    17,047.2 ns |    277.22 ns |    144.99 ns |   8.39 |    0.07 |    9 |         - |          NA |
| RadixMSD10Sort      | 1024 | Sorted             |    17,854.8 ns |    959.69 ns |    426.11 ns |   8.79 |    0.20 |    9 |         - |          NA |
| AmericanFlagSort    | 1024 | Sorted             |     5,931.8 ns |    244.35 ns |    127.80 ns |   2.92 |    0.06 |    6 |         - |          NA |
| SpreadSort          | 1024 | Sorted             |       321.9 ns |     16.17 ns |      7.18 ns |   0.16 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **1024** | **Reversed**           |     **3,583.7 ns** |    **101.80 ns** |     **45.20 ns** |   **1.71** |    **0.02** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Reversed           |     2,099.1 ns |     15.78 ns |      5.63 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | Reversed           |     3,342.4 ns |     28.61 ns |     12.70 ns |   1.59 |    0.01 |    4 |         - |          NA |
| PigeonSortInteger   | 1024 | Reversed           |     1,624.9 ns |    301.17 ns |    107.40 ns |   0.77 |    0.05 |    1 |         - |          NA |
| BucketSort          | 1024 | Reversed           |     4,612.3 ns |    241.35 ns |    126.23 ns |   2.20 |    0.06 |    5 |         - |          NA |
| BucketSortInteger   | 1024 | Reversed           |     3,099.0 ns |     19.12 ns |      6.82 ns |   1.48 |    0.00 |    4 |         - |          NA |
| FlashSort           | 1024 | Reversed           |    36,983.9 ns |    297.09 ns |    131.91 ns |  17.62 |    0.07 |   10 |         - |          NA |
| RadixLSD4Sort       | 1024 | Reversed           |     7,704.3 ns |    268.32 ns |    140.34 ns |   3.67 |    0.06 |    7 |         - |          NA |
| RotateLSDRadixSort  | 1024 | Reversed           |   135,259.0 ns |  1,994.64 ns |    885.63 ns |  64.44 |    0.43 |   11 |         - |          NA |
| RadixLSD256Sort     | 1024 | Reversed           |     3,540.2 ns |     14.53 ns |      5.18 ns |   1.69 |    0.00 |    4 |         - |          NA |
| Radsort             | 1024 | Reversed           |    14,119.6 ns |  3,645.23 ns |  1,906.52 ns |   6.73 |    0.86 |    8 |         - |          NA |
| RadixLSD10Sort      | 1024 | Reversed           |    12,394.7 ns |    135.72 ns |     70.98 ns |   5.90 |    0.04 |    8 |         - |          NA |
| RadixMSD4Sort       | 1024 | Reversed           |    17,530.4 ns |    331.31 ns |    147.10 ns |   8.35 |    0.07 |    9 |         - |          NA |
| RadixMSD10Sort      | 1024 | Reversed           |    17,661.0 ns |    704.57 ns |    368.50 ns |   8.41 |    0.17 |    9 |         - |          NA |
| AmericanFlagSort    | 1024 | Reversed           |     6,325.4 ns |    256.53 ns |    113.90 ns |   3.01 |    0.05 |    6 |         - |          NA |
| SpreadSort          | 1024 | Reversed           |     2,567.8 ns |     47.80 ns |     17.04 ns |   1.22 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **1024** | **PipeOrgan**          |     **3,622.1 ns** |     **47.72 ns** |     **21.19 ns** |   **1.61** |    **0.10** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | PipeOrgan          |     2,260.0 ns |    291.84 ns |    152.64 ns |   1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 1024 | PipeOrgan          |     3,391.8 ns |    277.27 ns |    123.11 ns |   1.51 |    0.11 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | PipeOrgan          |     1,934.8 ns |    505.69 ns |    224.53 ns |   0.86 |    0.11 |    1 |         - |          NA |
| BucketSort          | 1024 | PipeOrgan          |     4,848.1 ns |    347.20 ns |    181.59 ns |   2.15 |    0.15 |    3 |         - |          NA |
| BucketSortInteger   | 1024 | PipeOrgan          |     3,343.5 ns |    209.04 ns |    109.33 ns |   1.49 |    0.10 |    2 |         - |          NA |
| FlashSort           | 1024 | PipeOrgan          |    37,374.9 ns |    639.82 ns |    334.64 ns |  16.60 |    1.03 |    7 |         - |          NA |
| RadixLSD4Sort       | 1024 | PipeOrgan          |     8,101.1 ns |    631.14 ns |    330.10 ns |   3.60 |    0.26 |    4 |         - |          NA |
| RotateLSDRadixSort  | 1024 | PipeOrgan          |   134,586.9 ns |  3,517.31 ns |  1,839.62 ns |  59.78 |    3.74 |    8 |         - |          NA |
| RadixLSD256Sort     | 1024 | PipeOrgan          |     3,696.4 ns |     63.76 ns |     28.31 ns |   1.64 |    0.10 |    2 |         - |          NA |
| Radsort             | 1024 | PipeOrgan          |    13,479.4 ns |    351.32 ns |    183.75 ns |   5.99 |    0.37 |    5 |         - |          NA |
| RadixLSD10Sort      | 1024 | PipeOrgan          |    12,957.7 ns |    259.97 ns |    135.97 ns |   5.76 |    0.36 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | PipeOrgan          |    18,460.3 ns |    404.31 ns |    211.46 ns |   8.20 |    0.51 |    6 |         - |          NA |
| RadixMSD10Sort      | 1024 | PipeOrgan          |    18,685.7 ns |    516.87 ns |    270.33 ns |   8.30 |    0.52 |    6 |         - |          NA |
| AmericanFlagSort    | 1024 | PipeOrgan          |     9,125.5 ns |    289.50 ns |    128.54 ns |   4.05 |    0.25 |    4 |         - |          NA |
| SpreadSort          | 1024 | PipeOrgan          |     3,650.3 ns |    185.63 ns |     82.42 ns |   1.62 |    0.11 |    2 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **1024** | **ManyDuplicates**     |     **3,455.2 ns** |     **26.37 ns** |     **11.71 ns** |   **1.95** |    **0.03** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | ManyDuplicates     |     1,768.0 ns |     72.79 ns |     32.32 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | ManyDuplicates     |     3,457.0 ns |    237.80 ns |    105.58 ns |   1.96 |    0.06 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | ManyDuplicates     |     1,215.1 ns |     15.23 ns |      5.43 ns |   0.69 |    0.01 |    1 |         - |          NA |
| BucketSort          | 1024 | ManyDuplicates     |     7,271.9 ns |    288.02 ns |    150.64 ns |   4.11 |    0.11 |    4 |         - |          NA |
| BucketSortInteger   | 1024 | ManyDuplicates     |     4,013.1 ns |    174.58 ns |     91.31 ns |   2.27 |    0.06 |    3 |         - |          NA |
| FlashSort           | 1024 | ManyDuplicates     |    35,464.5 ns |    537.71 ns |    281.24 ns |  20.07 |    0.37 |    7 |         - |          NA |
| RadixLSD4Sort       | 1024 | ManyDuplicates     |     5,346.9 ns |    211.21 ns |     93.78 ns |   3.03 |    0.07 |    4 |         - |          NA |
| RotateLSDRadixSort  | 1024 | ManyDuplicates     |    91,572.4 ns |  6,041.89 ns |  3,160.03 ns |  51.81 |    1.90 |    8 |         - |          NA |
| RadixLSD256Sort     | 1024 | ManyDuplicates     |     2,062.7 ns |     15.85 ns |      5.65 ns |   1.17 |    0.02 |    2 |         - |          NA |
| Radsort             | 1024 | ManyDuplicates     |    11,939.9 ns |  1,519.14 ns |    794.54 ns |   6.76 |    0.44 |    6 |         - |          NA |
| RadixLSD10Sort      | 1024 | ManyDuplicates     |     7,352.5 ns |    304.62 ns |    159.32 ns |   4.16 |    0.11 |    4 |         - |          NA |
| RadixMSD4Sort       | 1024 | ManyDuplicates     |     6,364.8 ns |    325.92 ns |    170.46 ns |   3.60 |    0.11 |    4 |         - |          NA |
| RadixMSD10Sort      | 1024 | ManyDuplicates     |     9,114.8 ns |    251.26 ns |    131.41 ns |   5.16 |    0.11 |    5 |         - |          NA |
| AmericanFlagSort    | 1024 | ManyDuplicates     |     5,420.5 ns |    134.08 ns |     59.53 ns |   3.07 |    0.06 |    4 |         - |          NA |
| SpreadSort          | 1024 | ManyDuplicates     |     4,279.6 ns |     84.96 ns |     37.72 ns |   2.42 |    0.05 |    3 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **4096** | **Random**             |    **15,440.1 ns** |    **137.72 ns** |     **72.03 ns** |   **1.82** |    **0.04** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Random             |     8,486.7 ns |    332.60 ns |    173.95 ns |   1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 4096 | Random             |    13,004.2 ns |    123.41 ns |     54.79 ns |   1.53 |    0.03 |    2 |         - |          NA |
| PigeonSortInteger   | 4096 | Random             |     7,651.8 ns |     24.39 ns |      8.70 ns |   0.90 |    0.02 |    1 |         - |          NA |
| BucketSort          | 4096 | Random             |    19,499.6 ns |    378.95 ns |    198.20 ns |   2.30 |    0.05 |    3 |         - |          NA |
| BucketSortInteger   | 4096 | Random             |    13,808.4 ns |    914.03 ns |    478.06 ns |   1.63 |    0.06 |    2 |         - |          NA |
| FlashSort           | 4096 | Random             |   138,395.1 ns |  1,091.04 ns |    484.43 ns |  16.31 |    0.32 |    8 |         - |          NA |
| RadixLSD4Sort       | 4096 | Random             |    37,176.8 ns |    649.83 ns |    339.87 ns |   4.38 |    0.09 |    4 |         - |          NA |
| RotateLSDRadixSort  | 4096 | Random             | 1,150,348.5 ns | 57,233.49 ns | 29,934.22 ns | 135.60 |    4.22 |    9 |         - |          NA |
| RadixLSD256Sort     | 4096 | Random             |    13,561.7 ns |    795.87 ns |    353.37 ns |   1.60 |    0.05 |    2 |         - |          NA |
| Radsort             | 4096 | Random             |    36,305.1 ns |    618.97 ns |    274.83 ns |   4.28 |    0.09 |    4 |         - |          NA |
| RadixLSD10Sort      | 4096 | Random             |    56,846.6 ns |    553.19 ns |    289.33 ns |   6.70 |    0.13 |    6 |         - |          NA |
| RadixMSD4Sort       | 4096 | Random             |    70,548.3 ns |    637.15 ns |    333.24 ns |   8.32 |    0.16 |    7 |         - |          NA |
| RadixMSD10Sort      | 4096 | Random             |    77,612.6 ns |  1,081.59 ns |    480.23 ns |   9.15 |    0.18 |    7 |         - |          NA |
| AmericanFlagSort    | 4096 | Random             |    45,427.3 ns |    443.22 ns |    196.79 ns |   5.35 |    0.10 |    5 |         - |          NA |
| SpreadSort          | 4096 | Random             |    23,158.2 ns |  2,803.17 ns |  1,466.11 ns |   2.73 |    0.17 |    3 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **4096** | **SingleElementMoved** |    **14,810.1 ns** |    **289.92 ns** |    **128.73 ns** |   **1.79** |    **0.03** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 4096 | SingleElementMoved |     8,275.5 ns |    285.19 ns |    149.16 ns |   1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 4096 | SingleElementMoved |    13,183.7 ns |    262.44 ns |    137.26 ns |   1.59 |    0.03 |    2 |         - |          NA |
| PigeonSortInteger   | 4096 | SingleElementMoved |     7,016.0 ns |    524.49 ns |    274.32 ns |   0.85 |    0.03 |    1 |         - |          NA |
| BucketSort          | 4096 | SingleElementMoved |    18,671.5 ns |    220.66 ns |    115.41 ns |   2.26 |    0.04 |    3 |         - |          NA |
| BucketSortInteger   | 4096 | SingleElementMoved |    13,614.3 ns |    899.70 ns |    470.56 ns |   1.65 |    0.06 |    2 |         - |          NA |
| FlashSort           | 4096 | SingleElementMoved |   166,970.1 ns |  3,738.64 ns |  1,955.38 ns |  20.18 |    0.41 |    8 |         - |          NA |
| RadixLSD4Sort       | 4096 | SingleElementMoved |    38,238.6 ns |  5,446.36 ns |  2,848.55 ns |   4.62 |    0.33 |    5 |         - |          NA |
| RotateLSDRadixSort  | 4096 | SingleElementMoved |   666,959.0 ns | 37,793.25 ns | 19,766.60 ns |  80.62 |    2.64 |    9 |         - |          NA |
| RadixLSD256Sort     | 4096 | SingleElementMoved |    13,624.0 ns |    191.15 ns |     84.87 ns |   1.65 |    0.03 |    2 |         - |          NA |
| Radsort             | 4096 | SingleElementMoved |    29,664.0 ns |  1,124.06 ns |    587.90 ns |   3.59 |    0.09 |    4 |         - |          NA |
| RadixLSD10Sort      | 4096 | SingleElementMoved |    51,134.5 ns |  1,080.95 ns |    565.36 ns |   6.18 |    0.12 |    6 |         - |          NA |
| RadixMSD4Sort       | 4096 | SingleElementMoved |    75,320.2 ns |    974.02 ns |    432.47 ns |   9.10 |    0.16 |    7 |         - |          NA |
| RadixMSD10Sort      | 4096 | SingleElementMoved |    73,221.1 ns |  1,274.28 ns |    565.79 ns |   8.85 |    0.16 |    7 |         - |          NA |
| AmericanFlagSort    | 4096 | SingleElementMoved |    28,351.5 ns |    385.63 ns |    171.22 ns |   3.43 |    0.06 |    4 |         - |          NA |
| SpreadSort          | 4096 | SingleElementMoved |    15,284.5 ns |    988.56 ns |    438.93 ns |   1.85 |    0.06 |    2 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **4096** | **Sorted**             |    **14,398.6 ns** |    **556.32 ns** |    **247.01 ns** |   **1.80** |    **0.04** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Sorted             |     8,018.7 ns |    244.64 ns |    127.95 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 4096 | Sorted             |    13,398.3 ns |    322.27 ns |    168.55 ns |   1.67 |    0.03 |    4 |         - |          NA |
| PigeonSortInteger   | 4096 | Sorted             |     6,598.6 ns |    172.42 ns |     90.18 ns |   0.82 |    0.02 |    2 |         - |          NA |
| BucketSort          | 4096 | Sorted             |    17,952.4 ns |    367.71 ns |    192.32 ns |   2.24 |    0.04 |    5 |         - |          NA |
| BucketSortInteger   | 4096 | Sorted             |    11,772.9 ns |    529.15 ns |    234.94 ns |   1.47 |    0.04 |    4 |         - |          NA |
| FlashSort           | 4096 | Sorted             |   178,144.2 ns |  3,899.19 ns |  2,039.35 ns |  22.22 |    0.41 |    9 |         - |          NA |
| RadixLSD4Sort       | 4096 | Sorted             |    47,033.7 ns | 18,783.21 ns |  9,823.98 ns |   5.87 |    1.16 |    7 |         - |          NA |
| RotateLSDRadixSort  | 4096 | Sorted             |   638,754.3 ns | 25,571.35 ns | 11,353.84 ns |  79.68 |    1.78 |   10 |         - |          NA |
| RadixLSD256Sort     | 4096 | Sorted             |    13,381.0 ns |    300.27 ns |    157.04 ns |   1.67 |    0.03 |    4 |         - |          NA |
| Radsort             | 4096 | Sorted             |    28,670.3 ns |    651.20 ns |    340.59 ns |   3.58 |    0.07 |    6 |         - |          NA |
| RadixLSD10Sort      | 4096 | Sorted             |    50,442.7 ns |    691.00 ns |    306.81 ns |   6.29 |    0.10 |    7 |         - |          NA |
| RadixMSD4Sort       | 4096 | Sorted             |    73,720.0 ns |  4,083.53 ns |  1,813.11 ns |   9.20 |    0.25 |    8 |         - |          NA |
| RadixMSD10Sort      | 4096 | Sorted             |    72,108.0 ns |    793.40 ns |    414.97 ns |   8.99 |    0.14 |    8 |         - |          NA |
| AmericanFlagSort    | 4096 | Sorted             |    21,403.0 ns |    462.86 ns |    242.08 ns |   2.67 |    0.05 |    5 |         - |          NA |
| SpreadSort          | 4096 | Sorted             |     1,172.2 ns |    220.98 ns |    115.58 ns |   0.15 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **4096** | **Reversed**           |    **13,922.9 ns** |  **1,321.12 ns** |    **586.59 ns** |   **1.70** |    **0.07** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Reversed           |     8,171.4 ns |    301.13 ns |    157.50 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | Reversed           |    12,699.0 ns |    586.57 ns |    306.79 ns |   1.55 |    0.05 |    4 |         - |          NA |
| PigeonSortInteger   | 4096 | Reversed           |     6,514.1 ns |    292.38 ns |    152.92 ns |   0.80 |    0.02 |    1 |         - |          NA |
| BucketSort          | 4096 | Reversed           |    17,413.4 ns |    180.32 ns |     94.31 ns |   2.13 |    0.04 |    5 |         - |          NA |
| BucketSortInteger   | 4096 | Reversed           |    12,537.3 ns |    922.53 ns |    482.50 ns |   1.53 |    0.06 |    4 |         - |          NA |
| FlashSort           | 4096 | Reversed           |   142,392.3 ns |  4,779.02 ns |  2,499.52 ns |  17.43 |    0.43 |   10 |         - |          NA |
| RadixLSD4Sort       | 4096 | Reversed           |    36,653.8 ns |  2,919.88 ns |  1,296.44 ns |   4.49 |    0.17 |    7 |         - |          NA |
| RotateLSDRadixSort  | 4096 | Reversed           |   653,509.5 ns | 20,838.47 ns |  9,252.41 ns |  80.00 |    1.80 |   11 |         - |          NA |
| RadixLSD256Sort     | 4096 | Reversed           |    12,158.8 ns |    356.71 ns |    127.21 ns |   1.49 |    0.03 |    4 |         - |          NA |
| Radsort             | 4096 | Reversed           |    29,046.0 ns |    563.50 ns |    294.72 ns |   3.56 |    0.07 |    6 |         - |          NA |
| RadixLSD10Sort      | 4096 | Reversed           |    50,497.6 ns |    539.48 ns |    282.16 ns |   6.18 |    0.12 |    8 |         - |          NA |
| RadixMSD4Sort       | 4096 | Reversed           |    73,658.7 ns |  3,272.88 ns |  1,453.18 ns |   9.02 |    0.23 |    9 |         - |          NA |
| RadixMSD10Sort      | 4096 | Reversed           |    73,639.1 ns |    569.90 ns |    298.07 ns |   9.01 |    0.17 |    9 |         - |          NA |
| AmericanFlagSort    | 4096 | Reversed           |    24,661.4 ns |    371.56 ns |    194.33 ns |   3.02 |    0.06 |    6 |         - |          NA |
| SpreadSort          | 4096 | Reversed           |    10,004.3 ns |    215.25 ns |     95.57 ns |   1.22 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **4096** | **PipeOrgan**          |    **14,276.7 ns** |    **179.01 ns** |     **79.48 ns** |   **1.75** |    **0.02** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 4096 | PipeOrgan          |     8,179.4 ns |    214.45 ns |    112.16 ns |   1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 4096 | PipeOrgan          |    12,955.7 ns |    277.68 ns |    145.23 ns |   1.58 |    0.03 |    2 |         - |          NA |
| PigeonSortInteger   | 4096 | PipeOrgan          |     7,295.9 ns |    285.08 ns |    149.10 ns |   0.89 |    0.02 |    1 |         - |          NA |
| BucketSort          | 4096 | PipeOrgan          |    18,618.5 ns |    220.39 ns |    115.27 ns |   2.28 |    0.03 |    3 |         - |          NA |
| BucketSortInteger   | 4096 | PipeOrgan          |    12,974.4 ns |    588.00 ns |    307.53 ns |   1.59 |    0.04 |    2 |         - |          NA |
| FlashSort           | 4096 | PipeOrgan          |   133,623.5 ns |  1,554.18 ns |    690.07 ns |  16.34 |    0.23 |    8 |         - |          NA |
| RadixLSD4Sort       | 4096 | PipeOrgan          |    37,639.2 ns |  2,123.62 ns |    942.90 ns |   4.60 |    0.12 |    5 |         - |          NA |
| RotateLSDRadixSort  | 4096 | PipeOrgan          |   653,053.1 ns | 16,862.18 ns |  8,819.25 ns |  79.85 |    1.45 |    9 |         - |          NA |
| RadixLSD256Sort     | 4096 | PipeOrgan          |    13,969.7 ns |    152.14 ns |     67.55 ns |   1.71 |    0.02 |    2 |         - |          NA |
| Radsort             | 4096 | PipeOrgan          |    29,758.7 ns |    669.07 ns |    238.60 ns |   3.64 |    0.05 |    4 |         - |          NA |
| RadixLSD10Sort      | 4096 | PipeOrgan          |    53,366.5 ns |  3,634.59 ns |  1,613.78 ns |   6.53 |    0.20 |    6 |         - |          NA |
| RadixMSD4Sort       | 4096 | PipeOrgan          |    76,452.6 ns |  1,493.69 ns |    663.21 ns |   9.35 |    0.14 |    7 |         - |          NA |
| RadixMSD10Sort      | 4096 | PipeOrgan          |    75,069.6 ns |    868.00 ns |    453.98 ns |   9.18 |    0.13 |    7 |         - |          NA |
| AmericanFlagSort    | 4096 | PipeOrgan          |    36,169.6 ns |    750.55 ns |    392.55 ns |   4.42 |    0.07 |    5 |         - |          NA |
| SpreadSort          | 4096 | PipeOrgan          |    14,237.7 ns |    256.52 ns |    113.90 ns |   1.74 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **4096** | **ManyDuplicates**     |    **13,519.2 ns** |    **197.27 ns** |    **103.18 ns** |   **2.00** |    **0.02** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 4096 | ManyDuplicates     |     6,746.9 ns |     59.47 ns |     26.41 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | ManyDuplicates     |    16,345.4 ns |    311.08 ns |    138.12 ns |   2.42 |    0.02 |    4 |         - |          NA |
| PigeonSortInteger   | 4096 | ManyDuplicates     |     5,046.4 ns |     93.95 ns |     49.14 ns |   0.75 |    0.01 |    1 |         - |          NA |
| BucketSort          | 4096 | ManyDuplicates     |    29,018.8 ns |    286.94 ns |    150.08 ns |   4.30 |    0.03 |    5 |         - |          NA |
| BucketSortInteger   | 4096 | ManyDuplicates     |    15,398.5 ns |    389.69 ns |    173.02 ns |   2.28 |    0.03 |    4 |         - |          NA |
| FlashSort           | 4096 | ManyDuplicates     |   126,415.1 ns |  3,203.93 ns |  1,675.72 ns |  18.74 |    0.24 |    7 |         - |          NA |
| RadixLSD4Sort       | 4096 | ManyDuplicates     |    21,527.3 ns |    651.39 ns |    340.69 ns |   3.19 |    0.05 |    4 |         - |          NA |
| RotateLSDRadixSort  | 4096 | ManyDuplicates     |   607,208.3 ns | 10,431.82 ns |  4,631.79 ns |  90.00 |    0.72 |    8 |         - |          NA |
| RadixLSD256Sort     | 4096 | ManyDuplicates     |     8,387.8 ns |    232.97 ns |    121.85 ns |   1.24 |    0.02 |    3 |         - |          NA |
| Radsort             | 4096 | ManyDuplicates     |    20,999.6 ns |    421.70 ns |    220.56 ns |   3.11 |    0.03 |    4 |         - |          NA |
| RadixLSD10Sort      | 4096 | ManyDuplicates     |    27,935.0 ns |    287.64 ns |    150.44 ns |   4.14 |    0.03 |    5 |         - |          NA |
| RadixMSD4Sort       | 4096 | ManyDuplicates     |    21,057.4 ns |    109.57 ns |     39.07 ns |   3.12 |    0.01 |    4 |         - |          NA |
| RadixMSD10Sort      | 4096 | ManyDuplicates     |    35,069.5 ns |    465.48 ns |    206.68 ns |   5.20 |    0.03 |    6 |         - |          NA |
| AmericanFlagSort    | 4096 | ManyDuplicates     |    19,789.6 ns |    266.25 ns |    118.22 ns |   2.93 |    0.02 |    4 |         - |          NA |
| SpreadSort          | 4096 | ManyDuplicates     |    16,888.7 ns |    232.47 ns |    121.59 ns |   2.50 |    0.02 |    4 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **8192** | **Random**             |    **30,624.1 ns** |  **1,284.20 ns** |    **570.19 ns** |   **1.94** |    **0.04** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Random             |    15,749.0 ns |    344.42 ns |    180.14 ns |   1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 8192 | Random             |    26,341.7 ns |    411.41 ns |    215.18 ns |   1.67 |    0.02 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | Random             |    14,516.8 ns |  1,813.02 ns |    948.25 ns |   0.92 |    0.06 |    1 |         - |          NA |
| BucketSort          | 8192 | Random             |    38,313.7 ns |    959.24 ns |    501.70 ns |   2.43 |    0.04 |    3 |         - |          NA |
| BucketSortInteger   | 8192 | Random             |    26,629.3 ns |    515.96 ns |    269.86 ns |   1.69 |    0.02 |    2 |         - |          NA |
| FlashSort           | 8192 | Random             |   276,006.1 ns |  3,345.67 ns |  1,749.85 ns |  17.53 |    0.22 |    7 |         - |          NA |
| RadixLSD4Sort       | 8192 | Random             |    87,094.6 ns |  1,090.61 ns |    484.24 ns |   5.53 |    0.07 |    5 |         - |          NA |
| RotateLSDRadixSort  | 8192 | Random             | 2,786,279.4 ns | 60,194.43 ns | 31,482.85 ns | 176.94 |    2.67 |    8 |         - |          NA |
| RadixLSD256Sort     | 8192 | Random             |    26,702.7 ns |  1,443.39 ns |    754.92 ns |   1.70 |    0.05 |    2 |         - |          NA |
| Radsort             | 8192 | Random             |    50,423.7 ns |    711.38 ns |    315.86 ns |   3.20 |    0.04 |    4 |         - |          NA |
| RadixLSD10Sort      | 8192 | Random             |   105,312.7 ns |  1,182.84 ns |    618.65 ns |   6.69 |    0.08 |    5 |         - |          NA |
| RadixMSD4Sort       | 8192 | Random             |   152,187.6 ns |  3,158.89 ns |  1,652.16 ns |   9.66 |    0.14 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | Random             |   151,939.1 ns |  3,867.88 ns |  2,022.97 ns |   9.65 |    0.16 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | Random             |    88,668.9 ns |  1,868.81 ns |    829.76 ns |   5.63 |    0.08 |    5 |         - |          NA |
| SpreadSort          | 8192 | Random             |    50,322.6 ns |  2,075.93 ns |  1,085.75 ns |   3.20 |    0.07 |    4 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **8192** | **SingleElementMoved** |    **29,982.6 ns** |    **624.77 ns** |    **326.77 ns** |   **1.92** |    **0.03** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | SingleElementMoved |    15,632.8 ns |    456.26 ns |    238.63 ns |   1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 8192 | SingleElementMoved |    24,632.6 ns |    513.08 ns |    227.81 ns |   1.58 |    0.03 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | SingleElementMoved |    13,633.2 ns |  1,630.17 ns |    852.61 ns |   0.87 |    0.05 |    1 |         - |          NA |
| BucketSort          | 8192 | SingleElementMoved |    36,916.8 ns |    405.08 ns |    211.87 ns |   2.36 |    0.04 |    2 |         - |          NA |
| BucketSortInteger   | 8192 | SingleElementMoved |    24,788.7 ns |    684.91 ns |    304.10 ns |   1.59 |    0.03 |    2 |         - |          NA |
| FlashSort           | 8192 | SingleElementMoved |   329,584.7 ns |  4,822.27 ns |  2,522.14 ns |  21.09 |    0.34 |    6 |         - |          NA |
| RadixLSD4Sort       | 8192 | SingleElementMoved |    87,671.5 ns |  4,812.20 ns |  2,516.87 ns |   5.61 |    0.17 |    4 |         - |          NA |
| RotateLSDRadixSort  | 8192 | SingleElementMoved | 1,622,798.8 ns | 65,950.69 ns | 34,493.49 ns | 103.83 |    2.56 |    7 |         - |          NA |
| RadixLSD256Sort     | 8192 | SingleElementMoved |    27,072.7 ns |    414.43 ns |    216.75 ns |   1.73 |    0.03 |    2 |         - |          NA |
| Radsort             | 8192 | SingleElementMoved |    38,274.4 ns |  1,460.82 ns |    648.61 ns |   2.45 |    0.05 |    2 |         - |          NA |
| RadixLSD10Sort      | 8192 | SingleElementMoved |   100,525.7 ns |  1,324.58 ns |    588.12 ns |   6.43 |    0.10 |    4 |         - |          NA |
| RadixMSD4Sort       | 8192 | SingleElementMoved |   167,794.3 ns |  5,877.85 ns |  3,074.23 ns |  10.74 |    0.24 |    5 |         - |          NA |
| RadixMSD10Sort      | 8192 | SingleElementMoved |   148,395.0 ns |  1,878.97 ns |    834.27 ns |   9.49 |    0.14 |    5 |         - |          NA |
| AmericanFlagSort    | 8192 | SingleElementMoved |    55,995.9 ns |  1,267.24 ns |    662.79 ns |   3.58 |    0.07 |    3 |         - |          NA |
| SpreadSort          | 8192 | SingleElementMoved |    31,296.7 ns |  1,566.91 ns |    819.52 ns |   2.00 |    0.06 |    2 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **8192** | **Sorted**             |    **27,569.5 ns** |    **303.42 ns** |    **158.69 ns** |   **1.71** |    **0.02** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Sorted             |    16,097.7 ns |    310.16 ns |    162.22 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 8192 | Sorted             |    25,652.7 ns |    288.31 ns |    128.01 ns |   1.59 |    0.02 |    4 |         - |          NA |
| PigeonSortInteger   | 8192 | Sorted             |    12,970.5 ns |    771.07 ns |    403.28 ns |   0.81 |    0.02 |    2 |         - |          NA |
| BucketSort          | 8192 | Sorted             |    39,624.2 ns |  8,523.56 ns |  4,457.99 ns |   2.46 |    0.26 |    5 |         - |          NA |
| BucketSortInteger   | 8192 | Sorted             |    23,718.2 ns |    969.25 ns |    430.35 ns |   1.47 |    0.03 |    4 |         - |          NA |
| FlashSort           | 8192 | Sorted             |   340,753.7 ns |  7,195.96 ns |  3,195.05 ns |  21.17 |    0.27 |    8 |         - |          NA |
| RadixLSD4Sort       | 8192 | Sorted             |    84,306.2 ns |  2,164.94 ns |  1,132.31 ns |   5.24 |    0.08 |    6 |         - |          NA |
| RotateLSDRadixSort  | 8192 | Sorted             | 1,623,485.4 ns | 19,385.73 ns | 10,139.11 ns | 100.86 |    1.12 |    9 |         - |          NA |
| RadixLSD256Sort     | 8192 | Sorted             |    25,522.4 ns |    417.50 ns |    185.37 ns |   1.59 |    0.02 |    4 |         - |          NA |
| Radsort             | 8192 | Sorted             |    36,912.9 ns |  1,248.47 ns |    652.97 ns |   2.29 |    0.04 |    5 |         - |          NA |
| RadixLSD10Sort      | 8192 | Sorted             |    95,227.5 ns |    870.95 ns |    455.52 ns |   5.92 |    0.06 |    6 |         - |          NA |
| RadixMSD4Sort       | 8192 | Sorted             |   153,548.0 ns |  3,100.70 ns |  1,621.73 ns |   9.54 |    0.13 |    7 |         - |          NA |
| RadixMSD10Sort      | 8192 | Sorted             |   144,990.1 ns |  2,978.36 ns |  1,557.74 ns |   9.01 |    0.12 |    7 |         - |          NA |
| AmericanFlagSort    | 8192 | Sorted             |    41,426.7 ns |    412.82 ns |    215.91 ns |   2.57 |    0.03 |    5 |         - |          NA |
| SpreadSort          | 8192 | Sorted             |     2,160.4 ns |     37.33 ns |     16.57 ns |   0.13 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **8192** | **Reversed**           |    **27,769.2 ns** |    **327.16 ns** |    **145.26 ns** |   **1.69** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Reversed           |    16,394.3 ns |    373.50 ns |    195.35 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | Reversed           |    26,137.3 ns |    367.10 ns |    192.00 ns |   1.59 |    0.02 |    3 |         - |          NA |
| PigeonSortInteger   | 8192 | Reversed           |    12,817.1 ns |    316.21 ns |    140.40 ns |   0.78 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | Reversed           |    36,688.8 ns |    595.15 ns |    264.25 ns |   2.24 |    0.03 |    4 |         - |          NA |
| BucketSortInteger   | 8192 | Reversed           |    23,502.5 ns |    507.25 ns |    225.22 ns |   1.43 |    0.02 |    3 |         - |          NA |
| FlashSort           | 8192 | Reversed           |   298,381.3 ns |  9,280.95 ns |  4,854.11 ns |  18.20 |    0.35 |    8 |         - |          NA |
| RadixLSD4Sort       | 8192 | Reversed           |    88,321.5 ns |  1,209.30 ns |    536.94 ns |   5.39 |    0.07 |    6 |         - |          NA |
| RotateLSDRadixSort  | 8192 | Reversed           | 1,656,370.1 ns | 31,838.42 ns | 16,652.11 ns | 101.05 |    1.48 |    9 |         - |          NA |
| RadixLSD256Sort     | 8192 | Reversed           |    24,753.3 ns |  1,539.99 ns |    805.44 ns |   1.51 |    0.05 |    3 |         - |          NA |
| Radsort             | 8192 | Reversed           |    36,956.4 ns |    937.26 ns |    490.21 ns |   2.25 |    0.04 |    4 |         - |          NA |
| RadixLSD10Sort      | 8192 | Reversed           |    94,623.9 ns |  3,532.91 ns |  1,847.78 ns |   5.77 |    0.12 |    6 |         - |          NA |
| RadixMSD4Sort       | 8192 | Reversed           |   151,964.5 ns |  3,126.83 ns |  1,388.33 ns |   9.27 |    0.13 |    7 |         - |          NA |
| RadixMSD10Sort      | 8192 | Reversed           |   141,581.7 ns |  1,719.39 ns |    899.27 ns |   8.64 |    0.11 |    7 |         - |          NA |
| AmericanFlagSort    | 8192 | Reversed           |    46,773.1 ns |    564.83 ns |    250.79 ns |   2.85 |    0.03 |    5 |         - |          NA |
| SpreadSort          | 8192 | Reversed           |    38,866.1 ns |    448.13 ns |    198.97 ns |   2.37 |    0.03 |    4 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **8192** | **PipeOrgan**          |    **27,747.5 ns** |    **242.22 ns** |    **126.69 ns** |   **1.73** |    **0.02** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | PipeOrgan          |    16,060.1 ns |    317.23 ns |    165.92 ns |   1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 8192 | PipeOrgan          |    25,509.1 ns |    443.04 ns |    196.71 ns |   1.59 |    0.02 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | PipeOrgan          |    14,385.6 ns |    272.29 ns |    142.41 ns |   0.90 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | PipeOrgan          |    37,158.2 ns |    265.00 ns |    138.60 ns |   2.31 |    0.02 |    3 |         - |          NA |
| BucketSortInteger   | 8192 | PipeOrgan          |    26,943.5 ns |    356.37 ns |    158.23 ns |   1.68 |    0.02 |    2 |         - |          NA |
| FlashSort           | 8192 | PipeOrgan          |   254,886.1 ns |  7,473.49 ns |  3,908.78 ns |  15.87 |    0.28 |    8 |         - |          NA |
| RadixLSD4Sort       | 8192 | PipeOrgan          |    91,981.9 ns |    838.73 ns |    438.67 ns |   5.73 |    0.06 |    6 |         - |          NA |
| RotateLSDRadixSort  | 8192 | PipeOrgan          | 1,629,290.6 ns | 36,589.89 ns | 13,048.31 ns | 101.46 |    1.24 |    9 |         - |          NA |
| RadixLSD256Sort     | 8192 | PipeOrgan          |    27,176.8 ns |  2,117.19 ns |  1,107.33 ns |   1.69 |    0.07 |    2 |         - |          NA |
| Radsort             | 8192 | PipeOrgan          |    37,525.1 ns |    952.20 ns |    422.78 ns |   2.34 |    0.03 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | PipeOrgan          |   103,988.1 ns |  1,766.98 ns |    924.17 ns |   6.48 |    0.08 |    6 |         - |          NA |
| RadixMSD4Sort       | 8192 | PipeOrgan          |   153,684.5 ns |  3,521.38 ns |  1,841.75 ns |   9.57 |    0.14 |    7 |         - |          NA |
| RadixMSD10Sort      | 8192 | PipeOrgan          |   149,192.2 ns |  2,260.99 ns |  1,182.54 ns |   9.29 |    0.11 |    7 |         - |          NA |
| AmericanFlagSort    | 8192 | PipeOrgan          |    71,872.4 ns |  1,162.49 ns |    608.00 ns |   4.48 |    0.06 |    5 |         - |          NA |
| SpreadSort          | 8192 | PipeOrgan          |    54,005.9 ns |  1,523.54 ns |    676.46 ns |   3.36 |    0.05 |    4 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **8192** | **ManyDuplicates**     |    **25,871.5 ns** |    **771.75 ns** |    **342.66 ns** |   **1.99** |    **0.03** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 8192 | ManyDuplicates     |    13,010.8 ns |    188.77 ns |     98.73 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | ManyDuplicates     |    37,293.0 ns |    623.18 ns |    325.93 ns |   2.87 |    0.03 |    4 |         - |          NA |
| PigeonSortInteger   | 8192 | ManyDuplicates     |     9,693.1 ns |    402.84 ns |    178.86 ns |   0.75 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | ManyDuplicates     |    65,046.0 ns | 20,085.23 ns | 10,504.96 ns |   5.00 |    0.76 |    5 |         - |          NA |
| BucketSortInteger   | 8192 | ManyDuplicates     |    30,354.2 ns |    717.99 ns |    375.52 ns |   2.33 |    0.03 |    4 |         - |          NA |
| FlashSort           | 8192 | ManyDuplicates     |   241,366.7 ns |  8,004.58 ns |  4,186.55 ns |  18.55 |    0.33 |    6 |         - |          NA |
| RadixLSD4Sort       | 8192 | ManyDuplicates     |    41,950.7 ns |    390.89 ns |    139.40 ns |   3.22 |    0.03 |    4 |         - |          NA |
| RotateLSDRadixSort  | 8192 | ManyDuplicates     | 1,333,550.7 ns | 49,281.28 ns | 25,775.06 ns | 102.50 |    2.01 |    7 |         - |          NA |
| RadixLSD256Sort     | 8192 | ManyDuplicates     |    16,195.2 ns |    260.31 ns |    136.15 ns |   1.24 |    0.01 |    3 |         - |          NA |
| Radsort             | 8192 | ManyDuplicates     |    27,328.2 ns |    702.17 ns |    311.77 ns |   2.10 |    0.03 |    4 |         - |          NA |
| RadixLSD10Sort      | 8192 | ManyDuplicates     |    58,257.1 ns |  1,018.11 ns |    532.49 ns |   4.48 |    0.05 |    5 |         - |          NA |
| RadixMSD4Sort       | 8192 | ManyDuplicates     |    43,511.9 ns |    664.29 ns |    294.95 ns |   3.34 |    0.03 |    4 |         - |          NA |
| RadixMSD10Sort      | 8192 | ManyDuplicates     |    67,419.3 ns |  1,163.44 ns |    608.50 ns |   5.18 |    0.06 |    5 |         - |          NA |
| AmericanFlagSort    | 8192 | ManyDuplicates     |    38,441.5 ns |  1,049.03 ns |    548.66 ns |   2.95 |    0.05 |    4 |         - |          NA |
| SpreadSort          | 8192 | ManyDuplicates     |    33,110.0 ns |    580.22 ns |    303.47 ns |   2.54 |    0.03 |    4 |         - |          NA |

### ExchangeBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V45 4.27GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.48 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method             | Size | Pattern            | Mean         | Error        | StdDev       | Ratio  | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------- |----- |------------------- |-------------:|-------------:|-------------:|-------:|--------:|-----:|----------:|------------:|
| **BubbleSort**         | **256**  | **Random**             |  **18,151.8 ns** |    **444.56 ns** |    **197.39 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Random             |   9,220.3 ns |  1,055.35 ns |    551.97 ns |   0.51 |    0.03 |    2 |         - |          NA |
| OddEvenSort        | 256  | Random             |  13,057.1 ns |    424.19 ns |    188.34 ns |   0.72 |    0.01 |    3 |         - |          NA |
| CombSort           | 256  | Random             |   1,489.1 ns |     23.80 ns |      8.49 ns |   0.08 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | Random             |  10,439.4 ns |    378.60 ns |    198.02 ns |   0.58 |    0.01 |    2 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **BubbleSort**         | **256**  | **SingleElementMoved** |     **278.7 ns** |     **13.44 ns** |      **5.97 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 256  | SingleElementMoved |     219.0 ns |      3.16 ns |      1.13 ns |   0.79 |    0.02 |    1 |         - |          NA |
| OddEvenSort        | 256  | SingleElementMoved |   9,324.0 ns |    152.87 ns |     67.88 ns |  33.47 |    0.72 |    4 |         - |          NA |
| CombSort           | 256  | SingleElementMoved |   1,576.2 ns |     74.77 ns |     26.66 ns |   5.66 |    0.15 |    3 |         - |          NA |
| CircleSort         | 256  | SingleElementMoved |   9,155.1 ns |    342.18 ns |    178.97 ns |  32.86 |    0.90 |    4 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **BubbleSort**         | **256**  | **Sorted**             |     **129.7 ns** |      **1.23 ns** |      **0.55 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Sorted             |     105.5 ns |      2.42 ns |      1.07 ns |   0.81 |    0.01 |    1 |         - |          NA |
| OddEvenSort        | 256  | Sorted             |     189.7 ns |     38.70 ns |     20.24 ns |   1.46 |    0.15 |    3 |         - |          NA |
| CombSort           | 256  | Sorted             |   1,378.6 ns |     65.93 ns |     23.51 ns |  10.63 |    0.17 |    4 |         - |          NA |
| CircleSort         | 256  | Sorted             |   1,231.8 ns |     60.53 ns |     31.66 ns |   9.50 |    0.23 |    4 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **BubbleSort**         | **256**  | **Reversed**           |  **17,536.9 ns** |    **173.72 ns** |     **90.86 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Reversed           |  24,634.5 ns |    257.56 ns |    134.71 ns |   1.40 |    0.01 |    4 |         - |          NA |
| OddEvenSort        | 256  | Reversed           |  15,966.1 ns |  1,066.82 ns |    557.97 ns |   0.91 |    0.03 |    3 |         - |          NA |
| CombSort           | 256  | Reversed           |   2,038.2 ns |    599.49 ns |    313.54 ns |   0.12 |    0.02 |    1 |         - |          NA |
| CircleSort         | 256  | Reversed           |   2,506.9 ns |     13.43 ns |      4.79 ns |   0.14 |    0.00 |    2 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **BubbleSort**         | **256**  | **PipeOrgan**          |  **19,758.6 ns** |    **307.61 ns** |    **136.58 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | PipeOrgan          |  13,752.3 ns |    296.33 ns |    105.67 ns |   0.70 |    0.01 |    2 |         - |          NA |
| OddEvenSort        | 256  | PipeOrgan          |  18,414.2 ns |    787.78 ns |    412.02 ns |   0.93 |    0.02 |    3 |         - |          NA |
| CombSort           | 256  | PipeOrgan          |   1,669.8 ns |     20.51 ns |      7.31 ns |   0.08 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | PipeOrgan          |  11,432.5 ns |    345.01 ns |    153.19 ns |   0.58 |    0.01 |    2 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **BubbleSort**         | **256**  | **ManyDuplicates**     |  **18,799.3 ns** |    **610.13 ns** |    **319.11 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 256  | ManyDuplicates     |   8,518.0 ns |    372.47 ns |    194.81 ns |   0.45 |    0.01 |    2 |         - |          NA |
| OddEvenSort        | 256  | ManyDuplicates     |  12,397.9 ns |    301.44 ns |    107.50 ns |   0.66 |    0.01 |    3 |         - |          NA |
| CombSort           | 256  | ManyDuplicates     |   1,597.0 ns |    565.90 ns |    295.97 ns |   0.08 |    0.01 |    1 |         - |          NA |
| CircleSort         | 256  | ManyDuplicates     |   7,428.0 ns |    261.90 ns |    116.28 ns |   0.40 |    0.01 |    2 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Random**             | **291,387.2 ns** |  **4,242.34 ns** |  **1,883.63 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Random             | 148,172.6 ns |  2,076.63 ns |    922.04 ns |   0.51 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | Random             | 290,672.0 ns |  6,573.46 ns |  2,918.66 ns |   1.00 |    0.01 |    4 |         - |          NA |
| CombSort           | 1024 | Random             |   9,562.9 ns |  2,470.98 ns |  1,292.37 ns |   0.03 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | Random             |  54,938.2 ns |    713.80 ns |    373.33 ns |   0.19 |    0.00 |    2 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **BubbleSort**         | **1024** | **SingleElementMoved** |     **854.6 ns** |     **12.58 ns** |      **5.58 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | SingleElementMoved |     817.0 ns |      3.13 ns |      1.39 ns |   0.96 |    0.01 |    1 |         - |          NA |
| OddEvenSort        | 1024 | SingleElementMoved | 115,660.5 ns |  5,180.26 ns |  2,300.07 ns | 135.34 |    2.65 |    4 |         - |          NA |
| CombSort           | 1024 | SingleElementMoved |   7,589.4 ns |    631.44 ns |    280.36 ns |   8.88 |    0.31 |    2 |         - |          NA |
| CircleSort         | 1024 | SingleElementMoved |  56,188.2 ns |  1,933.59 ns |  1,011.31 ns |  65.75 |    1.19 |    3 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Sorted**             |     **466.3 ns** |      **3.39 ns** |      **1.51 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Sorted             |     323.0 ns |    199.32 ns |     88.50 ns |   0.69 |    0.18 |    1 |         - |          NA |
| OddEvenSort        | 1024 | Sorted             |     413.3 ns |      7.84 ns |      2.80 ns |   0.89 |    0.01 |    2 |         - |          NA |
| CombSort           | 1024 | Sorted             |   6,044.0 ns |    696.61 ns |    309.30 ns |  12.96 |    0.62 |    3 |         - |          NA |
| CircleSort         | 1024 | Sorted             |   5,111.9 ns |    155.36 ns |     68.98 ns |  10.96 |    0.14 |    3 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **BubbleSort**         | **1024** | **Reversed**           | **256,901.0 ns** |  **4,500.40 ns** |  **2,353.80 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Reversed           | 377,663.2 ns |  3,845.57 ns |  2,011.31 ns |   1.47 |    0.01 |    4 |         - |          NA |
| OddEvenSort        | 1024 | Reversed           | 237,371.3 ns |  5,882.33 ns |  3,076.57 ns |   0.92 |    0.01 |    3 |         - |          NA |
| CombSort           | 1024 | Reversed           |   9,318.2 ns |  1,263.47 ns |    660.82 ns |   0.04 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | Reversed           |  10,737.7 ns |    227.22 ns |     81.03 ns |   0.04 |    0.00 |    2 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **BubbleSort**         | **1024** | **PipeOrgan**          | **274,025.4 ns** |  **6,556.80 ns** |  **3,429.33 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | PipeOrgan          | 223,057.3 ns | 22,714.11 ns | 11,879.92 ns |   0.81 |    0.04 |    3 |         - |          NA |
| OddEvenSort        | 1024 | PipeOrgan          | 258,490.8 ns |  4,308.70 ns |  2,253.53 ns |   0.94 |    0.01 |    3 |         - |          NA |
| CombSort           | 1024 | PipeOrgan          |   8,989.5 ns |    364.70 ns |    161.93 ns |   0.03 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | PipeOrgan          |  64,527.8 ns | 13,683.22 ns |  7,156.59 ns |   0.24 |    0.02 |    2 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **BubbleSort**         | **1024** | **ManyDuplicates**     | **295,992.5 ns** |  **8,837.77 ns** |  **4,622.33 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | ManyDuplicates     | 147,123.0 ns |  4,092.89 ns |  2,140.66 ns |   0.50 |    0.01 |    3 |         - |          NA |
| OddEvenSort        | 1024 | ManyDuplicates     | 286,442.4 ns | 10,008.68 ns |  5,234.74 ns |   0.97 |    0.02 |    4 |         - |          NA |
| CombSort           | 1024 | ManyDuplicates     |   8,946.9 ns |  3,322.75 ns |  1,737.86 ns |   0.03 |    0.01 |    1 |         - |          NA |
| CircleSort         | 1024 | ManyDuplicates     |  49,299.6 ns |  1,687.04 ns |    749.06 ns |   0.17 |    0.00 |    2 |         - |          NA |

### HeapBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V45 4.27GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.48 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method           | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **HeapSort**         | **256**  | **Random**             |     **1,200.5 ns** |    **132.26 ns** |     **58.73 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | Random             |     1,170.8 ns |     96.54 ns |     42.86 ns |  0.98 |    0.06 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | Random             |     2,165.0 ns |    120.11 ns |     53.33 ns |  1.81 |    0.09 |    2 |         - |          NA |
| BottomupHeapSort | 256  | Random             |     2,296.5 ns |     58.75 ns |     20.95 ns |  1.92 |    0.09 |    2 |         - |          NA |
| WeakHeapSort     | 256  | Random             |     7,200.6 ns |    197.19 ns |    103.14 ns |  6.01 |    0.28 |    4 |         - |          NA |
| SmoothSort       | 256  | Random             |     2,372.7 ns |    152.30 ns |     67.62 ns |  1.98 |    0.10 |    2 |         - |          NA |
| PoplarHeapSort   | 256  | Random             |     3,669.8 ns |    309.72 ns |    161.99 ns |  3.06 |    0.19 |    3 |         - |          NA |
| TournamentSort   | 256  | Random             |     4,203.5 ns |    264.15 ns |    138.16 ns |  3.51 |    0.19 |    3 |         - |          NA |
| BinomialHeapSort | 256  | Random             |     9,735.2 ns |    640.15 ns |    284.23 ns |  8.13 |    0.43 |    5 |         - |          NA |
| PairingHeapSort  | 256  | Random             |     6,921.7 ns |    377.83 ns |    197.61 ns |  5.78 |    0.30 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **SingleElementMoved** |     **1,210.6 ns** |    **222.49 ns** |    **116.37 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 256  | SingleElementMoved |     1,235.3 ns |    124.50 ns |     55.28 ns |  1.03 |    0.10 |    2 |         - |          NA |
| TernaryHeapSort  | 256  | SingleElementMoved |     2,335.5 ns |    165.12 ns |     86.36 ns |  1.94 |    0.18 |    3 |         - |          NA |
| BottomupHeapSort | 256  | SingleElementMoved |     2,367.9 ns |     70.19 ns |     31.17 ns |  1.97 |    0.17 |    3 |         - |          NA |
| WeakHeapSort     | 256  | SingleElementMoved |     5,695.2 ns |    174.46 ns |     91.25 ns |  4.74 |    0.41 |    6 |         - |          NA |
| SmoothSort       | 256  | SingleElementMoved |       843.9 ns |     70.56 ns |     31.33 ns |  0.70 |    0.06 |    1 |         - |          NA |
| PoplarHeapSort   | 256  | SingleElementMoved |     2,471.6 ns |    142.61 ns |     74.59 ns |  2.06 |    0.18 |    3 |         - |          NA |
| TournamentSort   | 256  | SingleElementMoved |     3,237.5 ns |    332.82 ns |    174.07 ns |  2.69 |    0.27 |    4 |         - |          NA |
| BinomialHeapSort | 256  | SingleElementMoved |     4,785.7 ns |    472.88 ns |    247.33 ns |  3.98 |    0.39 |    5 |         - |          NA |
| PairingHeapSort  | 256  | SingleElementMoved |     3,646.8 ns |    513.56 ns |    268.60 ns |  3.04 |    0.33 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **Sorted**             |     **1,212.6 ns** |    **203.86 ns** |    **106.62 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 256  | Sorted             |     1,324.6 ns |    160.73 ns |     71.36 ns |  1.10 |    0.10 |    2 |         - |          NA |
| TernaryHeapSort  | 256  | Sorted             |     2,383.7 ns |    497.32 ns |    260.11 ns |  1.98 |    0.26 |    3 |         - |          NA |
| BottomupHeapSort | 256  | Sorted             |     2,459.6 ns |     78.59 ns |     34.89 ns |  2.04 |    0.17 |    3 |         - |          NA |
| WeakHeapSort     | 256  | Sorted             |     5,632.7 ns |    121.24 ns |     63.41 ns |  4.68 |    0.38 |    6 |         - |          NA |
| SmoothSort       | 256  | Sorted             |       669.0 ns |    116.29 ns |     60.82 ns |  0.56 |    0.07 |    1 |         - |          NA |
| PoplarHeapSort   | 256  | Sorted             |     1,971.2 ns |     37.72 ns |     13.45 ns |  1.64 |    0.13 |    3 |         - |          NA |
| TournamentSort   | 256  | Sorted             |     2,086.4 ns |      8.72 ns |      3.11 ns |  1.73 |    0.14 |    3 |         - |          NA |
| BinomialHeapSort | 256  | Sorted             |     4,090.8 ns |     81.65 ns |     29.12 ns |  3.40 |    0.27 |    5 |         - |          NA |
| PairingHeapSort  | 256  | Sorted             |     3,308.8 ns |    234.85 ns |    122.83 ns |  2.75 |    0.24 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **Reversed**           |     **1,249.8 ns** |    **140.01 ns** |     **73.23 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | Reversed           |     1,279.7 ns |    188.70 ns |     98.69 ns |  1.03 |    0.09 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | Reversed           |     2,449.2 ns |    331.43 ns |    173.35 ns |  1.97 |    0.17 |    2 |         - |          NA |
| BottomupHeapSort | 256  | Reversed           |     2,485.9 ns |    270.49 ns |    141.47 ns |  2.00 |    0.15 |    2 |         - |          NA |
| WeakHeapSort     | 256  | Reversed           |     6,555.8 ns |    174.62 ns |     91.33 ns |  5.26 |    0.30 |    4 |         - |          NA |
| SmoothSort       | 256  | Reversed           |     2,187.7 ns |    206.44 ns |     91.66 ns |  1.76 |    0.12 |    2 |         - |          NA |
| PoplarHeapSort   | 256  | Reversed           |     3,451.9 ns |    265.62 ns |    138.92 ns |  2.77 |    0.19 |    3 |         - |          NA |
| TournamentSort   | 256  | Reversed           |     3,248.4 ns |    262.71 ns |    137.40 ns |  2.61 |    0.18 |    3 |         - |          NA |
| BinomialHeapSort | 256  | Reversed           |     4,126.2 ns |    313.10 ns |    163.75 ns |  3.31 |    0.22 |    3 |         - |          NA |
| PairingHeapSort  | 256  | Reversed           |     1,472.8 ns |    215.54 ns |     95.70 ns |  1.18 |    0.10 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **PipeOrgan**          |     **1,059.6 ns** |    **101.04 ns** |     **44.86 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | PipeOrgan          |     1,158.5 ns |     46.65 ns |     16.64 ns |  1.09 |    0.04 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | PipeOrgan          |     2,248.4 ns |    258.94 ns |    135.43 ns |  2.13 |    0.14 |    2 |         - |          NA |
| BottomupHeapSort | 256  | PipeOrgan          |     2,408.4 ns |    193.80 ns |     86.05 ns |  2.28 |    0.11 |    2 |         - |          NA |
| WeakHeapSort     | 256  | PipeOrgan          |     6,122.6 ns |    231.53 ns |    121.10 ns |  5.79 |    0.24 |    4 |         - |          NA |
| SmoothSort       | 256  | PipeOrgan          |     2,310.4 ns |    335.18 ns |    175.31 ns |  2.18 |    0.18 |    2 |         - |          NA |
| PoplarHeapSort   | 256  | PipeOrgan          |     3,638.9 ns |    362.32 ns |    189.50 ns |  3.44 |    0.21 |    3 |         - |          NA |
| TournamentSort   | 256  | PipeOrgan          |     3,876.6 ns |    237.80 ns |    105.58 ns |  3.66 |    0.16 |    3 |         - |          NA |
| BinomialHeapSort | 256  | PipeOrgan          |     4,589.9 ns |    210.23 ns |     93.34 ns |  4.34 |    0.18 |    3 |         - |          NA |
| PairingHeapSort  | 256  | PipeOrgan          |     4,411.2 ns |    333.85 ns |    174.61 ns |  4.17 |    0.22 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **ManyDuplicates**     |     **1,377.9 ns** |    **140.86 ns** |     **62.54 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | ManyDuplicates     |     1,202.0 ns |    236.03 ns |    123.45 ns |  0.87 |    0.09 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | ManyDuplicates     |     2,224.0 ns |    466.36 ns |    207.07 ns |  1.62 |    0.16 |    2 |         - |          NA |
| BottomupHeapSort | 256  | ManyDuplicates     |     2,470.3 ns |    239.86 ns |    125.45 ns |  1.80 |    0.12 |    2 |         - |          NA |
| WeakHeapSort     | 256  | ManyDuplicates     |     7,686.4 ns |  3,441.90 ns |  1,528.22 ns |  5.59 |    1.07 |    4 |         - |          NA |
| SmoothSort       | 256  | ManyDuplicates     |     2,157.2 ns |    243.68 ns |    108.20 ns |  1.57 |    0.10 |    2 |         - |          NA |
| PoplarHeapSort   | 256  | ManyDuplicates     |     3,732.6 ns |    450.36 ns |    235.54 ns |  2.71 |    0.20 |    3 |         - |          NA |
| TournamentSort   | 256  | ManyDuplicates     |     4,299.1 ns |    452.95 ns |    236.90 ns |  3.13 |    0.21 |    3 |         - |          NA |
| BinomialHeapSort | 256  | ManyDuplicates     |     8,648.4 ns |    545.93 ns |    285.53 ns |  6.29 |    0.33 |    5 |         - |          NA |
| PairingHeapSort  | 256  | ManyDuplicates     |     6,856.5 ns |    449.59 ns |    235.14 ns |  4.99 |    0.27 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Random**             |     **6,281.0 ns** |    **716.21 ns** |    **318.00 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Random             |     6,154.0 ns |    703.78 ns |    312.48 ns |  0.98 |    0.07 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | Random             |    10,474.4 ns |  1,513.63 ns |    791.66 ns |  1.67 |    0.14 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | Random             |    10,854.1 ns |    364.00 ns |    161.62 ns |  1.73 |    0.08 |    2 |         - |          NA |
| WeakHeapSort     | 1024 | Random             |    38,638.2 ns |    753.65 ns |    394.17 ns |  6.16 |    0.29 |    4 |         - |          NA |
| SmoothSort       | 1024 | Random             |    12,134.0 ns |    991.82 ns |    440.37 ns |  1.94 |    0.11 |    2 |         - |          NA |
| PoplarHeapSort   | 1024 | Random             |    17,496.2 ns |  1,114.03 ns |    582.66 ns |  2.79 |    0.16 |    3 |         - |          NA |
| TournamentSort   | 1024 | Random             |    19,814.9 ns |    269.13 ns |     95.98 ns |  3.16 |    0.15 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | Random             |    57,945.7 ns |  9,525.09 ns |  4,981.80 ns |  9.25 |    0.86 |    5 |         - |          NA |
| PairingHeapSort  | 1024 | Random             |    34,546.9 ns |  1,566.77 ns |    819.45 ns |  5.51 |    0.28 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **SingleElementMoved** |     **5,960.2 ns** |    **796.24 ns** |    **416.45 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | SingleElementMoved |     6,967.9 ns |    741.99 ns |    329.45 ns |  1.17 |    0.09 |    3 |         - |          NA |
| TernaryHeapSort  | 1024 | SingleElementMoved |    11,101.3 ns |  1,222.32 ns |    639.30 ns |  1.87 |    0.16 |    4 |         - |          NA |
| BottomupHeapSort | 1024 | SingleElementMoved |    11,018.2 ns |    215.95 ns |     77.01 ns |  1.86 |    0.12 |    4 |         - |          NA |
| WeakHeapSort     | 1024 | SingleElementMoved |    28,510.9 ns |    337.59 ns |    176.57 ns |  4.80 |    0.30 |    7 |         - |          NA |
| SmoothSort       | 1024 | SingleElementMoved |     3,516.5 ns |    261.13 ns |    115.94 ns |  0.59 |    0.04 |    1 |         - |          NA |
| PoplarHeapSort   | 1024 | SingleElementMoved |    11,396.5 ns |    295.98 ns |    154.80 ns |  1.92 |    0.12 |    4 |         - |          NA |
| TournamentSort   | 1024 | SingleElementMoved |    14,621.7 ns |    447.05 ns |    198.49 ns |  2.46 |    0.16 |    5 |         - |          NA |
| BinomialHeapSort | 1024 | SingleElementMoved |    20,593.9 ns |    278.48 ns |    145.65 ns |  3.47 |    0.22 |    6 |         - |          NA |
| PairingHeapSort  | 1024 | SingleElementMoved |    13,995.2 ns |    374.13 ns |    166.11 ns |  2.36 |    0.15 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Sorted**             |     **5,868.2 ns** |    **943.83 ns** |    **419.07 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Sorted             |     6,237.0 ns |    496.58 ns |    259.72 ns |  1.07 |    0.08 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | Sorted             |    10,481.8 ns |    593.13 ns |    263.35 ns |  1.79 |    0.12 |    3 |         - |          NA |
| BottomupHeapSort | 1024 | Sorted             |    10,638.6 ns |    344.81 ns |    122.96 ns |  1.82 |    0.12 |    3 |         - |          NA |
| WeakHeapSort     | 1024 | Sorted             |    28,888.8 ns |    515.06 ns |    269.39 ns |  4.94 |    0.31 |    6 |         - |          NA |
| SmoothSort       | 1024 | Sorted             |     2,474.1 ns |    108.74 ns |     48.28 ns |  0.42 |    0.03 |    1 |         - |          NA |
| PoplarHeapSort   | 1024 | Sorted             |     9,026.7 ns |    170.00 ns |     75.48 ns |  1.54 |    0.10 |    3 |         - |          NA |
| TournamentSort   | 1024 | Sorted             |    10,040.4 ns |    397.56 ns |    207.93 ns |  1.72 |    0.11 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | Sorted             |    18,208.7 ns |    297.47 ns |    155.58 ns |  3.12 |    0.20 |    5 |         - |          NA |
| PairingHeapSort  | 1024 | Sorted             |    14,001.6 ns |    145.34 ns |     64.53 ns |  2.40 |    0.15 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Reversed**           |     **6,486.8 ns** |    **737.20 ns** |    **385.57 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Reversed           |     6,192.4 ns |    916.57 ns |    406.96 ns |  0.96 |    0.08 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | Reversed           |    10,262.0 ns |    549.16 ns |    243.83 ns |  1.59 |    0.09 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | Reversed           |    11,835.1 ns |    792.43 ns |    414.46 ns |  1.83 |    0.12 |    2 |         - |          NA |
| WeakHeapSort     | 1024 | Reversed           |    32,995.9 ns |    308.19 ns |    161.19 ns |  5.10 |    0.28 |    4 |         - |          NA |
| SmoothSort       | 1024 | Reversed           |    11,192.8 ns |    755.96 ns |    395.38 ns |  1.73 |    0.11 |    2 |         - |          NA |
| PoplarHeapSort   | 1024 | Reversed           |    15,613.4 ns |    636.24 ns |    282.49 ns |  2.41 |    0.14 |    3 |         - |          NA |
| TournamentSort   | 1024 | Reversed           |    14,560.0 ns |    235.11 ns |     83.84 ns |  2.25 |    0.12 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | Reversed           |    18,160.5 ns |    326.60 ns |    145.01 ns |  2.81 |    0.16 |    3 |         - |          NA |
| PairingHeapSort  | 1024 | Reversed           |     5,634.2 ns |    203.83 ns |    106.61 ns |  0.87 |    0.05 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **PipeOrgan**          |     **6,125.2 ns** |    **991.21 ns** |    **518.42 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | PipeOrgan          |     6,016.3 ns |    559.38 ns |    292.57 ns |  0.99 |    0.09 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | PipeOrgan          |    10,052.7 ns |    652.91 ns |    341.48 ns |  1.65 |    0.13 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | PipeOrgan          |    10,815.1 ns |    660.76 ns |    293.38 ns |  1.78 |    0.14 |    2 |         - |          NA |
| WeakHeapSort     | 1024 | PipeOrgan          |    33,120.7 ns |    311.79 ns |    138.44 ns |  5.44 |    0.41 |    5 |         - |          NA |
| SmoothSort       | 1024 | PipeOrgan          |    12,047.2 ns |    550.74 ns |    244.53 ns |  1.98 |    0.15 |    2 |         - |          NA |
| PoplarHeapSort   | 1024 | PipeOrgan          |    17,427.7 ns |    704.69 ns |    312.88 ns |  2.86 |    0.22 |    3 |         - |          NA |
| TournamentSort   | 1024 | PipeOrgan          |    24,926.2 ns |  6,946.97 ns |  3,633.40 ns |  4.09 |    0.64 |    4 |         - |          NA |
| BinomialHeapSort | 1024 | PipeOrgan          |    20,409.0 ns |    227.90 ns |    101.19 ns |  3.35 |    0.25 |    3 |         - |          NA |
| PairingHeapSort  | 1024 | PipeOrgan          |    18,587.0 ns |    111.43 ns |     49.47 ns |  3.05 |    0.23 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **ManyDuplicates**     |     **6,243.9 ns** |    **945.44 ns** |    **494.48 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | ManyDuplicates     |     6,117.4 ns |    929.44 ns |    486.11 ns |  0.98 |    0.10 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | ManyDuplicates     |     9,923.9 ns |    733.45 ns |    383.61 ns |  1.60 |    0.13 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | ManyDuplicates     |    12,837.5 ns |  3,580.90 ns |  1,872.88 ns |  2.07 |    0.32 |    2 |         - |          NA |
| WeakHeapSort     | 1024 | ManyDuplicates     |    33,014.9 ns |    352.98 ns |    156.72 ns |  5.32 |    0.39 |    5 |         - |          NA |
| SmoothSort       | 1024 | ManyDuplicates     |    10,916.9 ns |    695.86 ns |    363.95 ns |  1.76 |    0.14 |    2 |         - |          NA |
| PoplarHeapSort   | 1024 | ManyDuplicates     |    16,467.0 ns |    752.71 ns |    334.21 ns |  2.65 |    0.20 |    3 |         - |          NA |
| TournamentSort   | 1024 | ManyDuplicates     |    20,483.9 ns |    328.56 ns |    145.88 ns |  3.30 |    0.24 |    4 |         - |          NA |
| BinomialHeapSort | 1024 | ManyDuplicates     |    42,804.6 ns |  1,180.53 ns |    524.16 ns |  6.89 |    0.50 |    6 |         - |          NA |
| PairingHeapSort  | 1024 | ManyDuplicates     |    31,763.9 ns |    532.61 ns |    278.57 ns |  5.11 |    0.37 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Random**             |   **154,256.0 ns** | **12,188.37 ns** |  **6,374.75 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Random             |   156,972.8 ns | 11,733.20 ns |  6,136.69 ns |  1.02 |    0.05 |    1 |         - |          NA |
| TernaryHeapSort  | 4096 | Random             |   196,293.0 ns |  7,820.70 ns |  4,090.38 ns |  1.27 |    0.06 |    2 |         - |          NA |
| BottomupHeapSort | 4096 | Random             |   192,991.1 ns |  5,778.55 ns |  2,565.71 ns |  1.25 |    0.05 |    2 |         - |          NA |
| WeakHeapSort     | 4096 | Random             |   282,753.5 ns |  6,852.08 ns |  3,583.77 ns |  1.84 |    0.07 |    3 |         - |          NA |
| SmoothSort       | 4096 | Random             |   294,196.8 ns |  6,824.90 ns |  3,569.55 ns |  1.91 |    0.08 |    3 |         - |          NA |
| PoplarHeapSort   | 4096 | Random             |   251,451.2 ns |  7,331.02 ns |  3,834.26 ns |  1.63 |    0.07 |    3 |         - |          NA |
| TournamentSort   | 4096 | Random             |   476,419.5 ns |  9,841.41 ns |  4,369.65 ns |  3.09 |    0.12 |    4 |         - |          NA |
| BinomialHeapSort | 4096 | Random             |   725,416.7 ns | 19,439.07 ns | 10,167.01 ns |  4.71 |    0.19 |    5 |         - |          NA |
| PairingHeapSort  | 4096 | Random             |   324,700.4 ns | 12,588.57 ns |  6,584.07 ns |  2.11 |    0.09 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **SingleElementMoved** |    **93,038.6 ns** | **15,427.62 ns** |  **8,068.94 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 4096 | SingleElementMoved |   103,279.7 ns |  6,912.14 ns |  3,069.03 ns |  1.12 |    0.09 |    3 |         - |          NA |
| TernaryHeapSort  | 4096 | SingleElementMoved |   102,019.0 ns |  7,706.91 ns |  4,030.86 ns |  1.10 |    0.10 |    3 |         - |          NA |
| BottomupHeapSort | 4096 | SingleElementMoved |   121,622.2 ns | 15,692.32 ns |  8,207.39 ns |  1.32 |    0.13 |    3 |         - |          NA |
| WeakHeapSort     | 4096 | SingleElementMoved |   135,610.7 ns |  4,890.10 ns |  2,557.62 ns |  1.47 |    0.12 |    3 |         - |          NA |
| SmoothSort       | 4096 | SingleElementMoved |    13,967.9 ns |    224.70 ns |     99.77 ns |  0.15 |    0.01 |    1 |         - |          NA |
| PoplarHeapSort   | 4096 | SingleElementMoved |    49,091.1 ns |    222.86 ns |     98.95 ns |  0.53 |    0.04 |    2 |         - |          NA |
| TournamentSort   | 4096 | SingleElementMoved |   227,891.2 ns |  8,530.07 ns |  4,461.39 ns |  2.46 |    0.20 |    4 |         - |          NA |
| BinomialHeapSort | 4096 | SingleElementMoved |    86,029.2 ns |    836.12 ns |    371.24 ns |  0.93 |    0.07 |    3 |         - |          NA |
| PairingHeapSort  | 4096 | SingleElementMoved |    53,204.2 ns |    484.12 ns |    253.20 ns |  0.58 |    0.04 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Sorted**             |    **85,951.9 ns** |  **3,792.41 ns** |  **1,983.50 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Sorted             |   102,999.2 ns | 12,330.62 ns |  6,449.15 ns |  1.20 |    0.08 |    4 |         - |          NA |
| TernaryHeapSort  | 4096 | Sorted             |   109,272.1 ns |  9,183.32 ns |  4,803.05 ns |  1.27 |    0.06 |    4 |         - |          NA |
| BottomupHeapSort | 4096 | Sorted             |   104,848.2 ns |  3,103.06 ns |  1,377.78 ns |  1.22 |    0.03 |    4 |         - |          NA |
| WeakHeapSort     | 4096 | Sorted             |   139,869.5 ns |    912.81 ns |    477.42 ns |  1.63 |    0.04 |    4 |         - |          NA |
| SmoothSort       | 4096 | Sorted             |    10,094.3 ns |    614.18 ns |    321.23 ns |  0.12 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 4096 | Sorted             |    43,611.5 ns |    254.93 ns |    113.19 ns |  0.51 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 4096 | Sorted             |   123,271.5 ns |  4,434.30 ns |  2,319.22 ns |  1.43 |    0.04 |    4 |         - |          NA |
| BinomialHeapSort | 4096 | Sorted             |    79,363.3 ns |  1,029.77 ns |    457.23 ns |  0.92 |    0.02 |    4 |         - |          NA |
| PairingHeapSort  | 4096 | Sorted             |    56,510.8 ns |  1,890.26 ns |    988.64 ns |  0.66 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Reversed**           |    **96,827.4 ns** | **14,155.60 ns** |  **7,403.65 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Reversed           |    89,563.6 ns | 10,352.56 ns |  5,414.59 ns |  0.93 |    0.08 |    2 |         - |          NA |
| TernaryHeapSort  | 4096 | Reversed           |    96,375.7 ns |  6,313.23 ns |  2,803.11 ns |  1.00 |    0.07 |    2 |         - |          NA |
| BottomupHeapSort | 4096 | Reversed           |   125,115.5 ns |  4,431.49 ns |  1,967.61 ns |  1.30 |    0.09 |    3 |         - |          NA |
| WeakHeapSort     | 4096 | Reversed           |   157,734.1 ns |  3,220.57 ns |  1,684.42 ns |  1.64 |    0.11 |    4 |         - |          NA |
| SmoothSort       | 4096 | Reversed           |   158,184.7 ns |  6,689.42 ns |  3,498.70 ns |  1.64 |    0.12 |    4 |         - |          NA |
| PoplarHeapSort   | 4096 | Reversed           |    88,844.2 ns |  4,334.25 ns |  2,266.90 ns |  0.92 |    0.07 |    2 |         - |          NA |
| TournamentSort   | 4096 | Reversed           |   197,347.8 ns |  9,463.25 ns |  4,201.74 ns |  2.05 |    0.15 |    5 |         - |          NA |
| BinomialHeapSort | 4096 | Reversed           |    79,436.8 ns |  1,022.24 ns |    453.88 ns |  0.82 |    0.06 |    2 |         - |          NA |
| PairingHeapSort  | 4096 | Reversed           |    22,135.1 ns |    213.32 ns |    111.57 ns |  0.23 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **PipeOrgan**          |   **101,604.3 ns** |  **7,709.25 ns** |  **3,422.95 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 4096 | PipeOrgan          |   115,655.1 ns | 11,612.52 ns |  6,073.58 ns |  1.14 |    0.07 |    1 |         - |          NA |
| TernaryHeapSort  | 4096 | PipeOrgan          |   107,811.8 ns |  7,082.06 ns |  3,144.48 ns |  1.06 |    0.04 |    1 |         - |          NA |
| BottomupHeapSort | 4096 | PipeOrgan          |   110,830.9 ns |  5,421.16 ns |  2,407.03 ns |  1.09 |    0.04 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | PipeOrgan          |   163,498.3 ns |  5,493.95 ns |  2,873.44 ns |  1.61 |    0.06 |    2 |         - |          NA |
| SmoothSort       | 4096 | PipeOrgan          |   211,212.0 ns |  3,866.58 ns |  2,022.30 ns |  2.08 |    0.07 |    3 |         - |          NA |
| PoplarHeapSort   | 4096 | PipeOrgan          |   143,992.3 ns | 14,345.96 ns |  6,369.69 ns |  1.42 |    0.07 |    2 |         - |          NA |
| TournamentSort   | 4096 | PipeOrgan          |   340,439.0 ns |  9,092.35 ns |  4,755.48 ns |  3.35 |    0.11 |    4 |         - |          NA |
| BinomialHeapSort | 4096 | PipeOrgan          |    88,904.8 ns |  1,467.69 ns |    651.66 ns |  0.88 |    0.03 |    1 |         - |          NA |
| PairingHeapSort  | 4096 | PipeOrgan          |    77,938.9 ns |  1,237.45 ns |    647.21 ns |  0.77 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **ManyDuplicates**     |   **132,810.6 ns** |  **7,848.67 ns** |  **4,105.00 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 4096 | ManyDuplicates     |   137,174.2 ns |  8,576.24 ns |  4,485.54 ns |  1.03 |    0.04 |    1 |         - |          NA |
| TernaryHeapSort  | 4096 | ManyDuplicates     |   160,763.8 ns |  8,364.41 ns |  4,374.75 ns |  1.21 |    0.05 |    1 |         - |          NA |
| BottomupHeapSort | 4096 | ManyDuplicates     |   176,360.7 ns |  8,952.09 ns |  4,682.12 ns |  1.33 |    0.05 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | ManyDuplicates     |   193,379.9 ns |  2,113.90 ns |    938.58 ns |  1.46 |    0.04 |    1 |         - |          NA |
| SmoothSort       | 4096 | ManyDuplicates     |   247,008.8 ns |  8,084.26 ns |  4,228.22 ns |  1.86 |    0.06 |    1 |         - |          NA |
| PoplarHeapSort   | 4096 | ManyDuplicates     |   225,704.3 ns | 10,171.38 ns |  5,319.83 ns |  1.70 |    0.06 |    1 |         - |          NA |
| TournamentSort   | 4096 | ManyDuplicates     |   442,163.7 ns | 12,393.38 ns |  6,481.98 ns |  3.33 |    0.11 |    2 |         - |          NA |
| BinomialHeapSort | 4096 | ManyDuplicates     |   484,707.3 ns |  2,683.99 ns |  1,191.71 ns |  3.65 |    0.11 |    2 |         - |          NA |
| PairingHeapSort  | 4096 | ManyDuplicates     |   241,206.6 ns | 10,731.16 ns |  5,612.60 ns |  1.82 |    0.07 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Random**             |   **408,056.7 ns** |  **8,027.83 ns** |  **3,564.41 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Random             |   405,841.1 ns |  8,204.56 ns |  4,291.14 ns |  0.99 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | Random             |   495,168.9 ns | 12,956.46 ns |  6,776.48 ns |  1.21 |    0.02 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | Random             |   504,066.4 ns |  9,541.05 ns |  4,990.16 ns |  1.24 |    0.02 |    2 |         - |          NA |
| WeakHeapSort     | 8192 | Random             |   676,377.3 ns |  9,071.70 ns |  4,744.68 ns |  1.66 |    0.02 |    3 |         - |          NA |
| SmoothSort       | 8192 | Random             |   675,792.8 ns | 19,341.19 ns | 10,115.82 ns |  1.66 |    0.03 |    3 |         - |          NA |
| PoplarHeapSort   | 8192 | Random             |   621,453.3 ns | 27,391.42 ns | 14,326.24 ns |  1.52 |    0.04 |    3 |         - |          NA |
| TournamentSort   | 8192 | Random             | 1,049,691.2 ns | 10,761.46 ns |  5,628.45 ns |  2.57 |    0.02 |    4 |         - |          NA |
| BinomialHeapSort | 8192 | Random             | 1,677,677.3 ns | 28,488.32 ns | 14,899.94 ns |  4.11 |    0.05 |    5 |         - |          NA |
| PairingHeapSort  | 8192 | Random             |   795,097.2 ns | 16,633.39 ns |  7,385.33 ns |  1.95 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **SingleElementMoved** |   **249,355.5 ns** | **11,433.33 ns** |  **5,979.85 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 8192 | SingleElementMoved |   266,161.9 ns |  6,376.81 ns |  2,831.34 ns |  1.07 |    0.03 |    4 |         - |          NA |
| TernaryHeapSort  | 8192 | SingleElementMoved |   289,795.8 ns | 14,668.80 ns |  7,672.07 ns |  1.16 |    0.04 |    4 |         - |          NA |
| BottomupHeapSort | 8192 | SingleElementMoved |   325,012.7 ns | 18,859.83 ns |  8,373.88 ns |  1.30 |    0.04 |    4 |         - |          NA |
| WeakHeapSort     | 8192 | SingleElementMoved |   313,467.2 ns |  2,090.13 ns |  1,093.18 ns |  1.26 |    0.03 |    4 |         - |          NA |
| SmoothSort       | 8192 | SingleElementMoved |    29,301.1 ns |  1,078.38 ns |    564.01 ns |  0.12 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 8192 | SingleElementMoved |   106,523.1 ns |  1,361.16 ns |    604.36 ns |  0.43 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 8192 | SingleElementMoved |   564,303.2 ns |  9,043.63 ns |  4,015.43 ns |  2.26 |    0.05 |    5 |         - |          NA |
| BinomialHeapSort | 8192 | SingleElementMoved |   185,612.2 ns |  5,483.43 ns |  2,434.68 ns |  0.74 |    0.02 |    3 |         - |          NA |
| PairingHeapSort  | 8192 | SingleElementMoved |   108,092.5 ns |  1,878.69 ns |    982.59 ns |  0.43 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Sorted**             |   **241,504.5 ns** |  **6,212.28 ns** |  **2,758.29 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Sorted             |   271,695.0 ns | 16,498.17 ns |  8,628.86 ns |  1.13 |    0.04 |    5 |         - |          NA |
| TernaryHeapSort  | 8192 | Sorted             |   279,457.4 ns | 11,650.59 ns |  6,093.49 ns |  1.16 |    0.03 |    5 |         - |          NA |
| BottomupHeapSort | 8192 | Sorted             |   308,707.1 ns | 14,401.95 ns |  6,394.55 ns |  1.28 |    0.03 |    5 |         - |          NA |
| WeakHeapSort     | 8192 | Sorted             |   309,619.8 ns | 17,033.07 ns |  8,908.62 ns |  1.28 |    0.04 |    5 |         - |          NA |
| SmoothSort       | 8192 | Sorted             |    20,346.7 ns |    598.83 ns |    313.20 ns |  0.08 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 8192 | Sorted             |    90,045.4 ns |    550.33 ns |    244.35 ns |  0.37 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 8192 | Sorted             |   336,478.9 ns | 21,470.73 ns | 11,229.60 ns |  1.39 |    0.05 |    5 |         - |          NA |
| BinomialHeapSort | 8192 | Sorted             |   169,695.3 ns |  9,345.12 ns |  4,149.29 ns |  0.70 |    0.02 |    4 |         - |          NA |
| PairingHeapSort  | 8192 | Sorted             |   113,079.9 ns |  3,020.58 ns |  1,579.82 ns |  0.47 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Reversed**           |   **260,379.8 ns** | **11,622.77 ns** |  **5,160.58 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Reversed           |   235,626.9 ns |  8,349.38 ns |  3,707.18 ns |  0.91 |    0.02 |    3 |         - |          NA |
| TernaryHeapSort  | 8192 | Reversed           |   293,482.2 ns |  3,771.90 ns |  1,345.10 ns |  1.13 |    0.02 |    3 |         - |          NA |
| BottomupHeapSort | 8192 | Reversed           |   354,483.0 ns |  9,597.78 ns |  5,019.83 ns |  1.36 |    0.03 |    3 |         - |          NA |
| WeakHeapSort     | 8192 | Reversed           |   341,853.1 ns |  3,917.00 ns |  2,048.67 ns |  1.31 |    0.03 |    3 |         - |          NA |
| SmoothSort       | 8192 | Reversed           |   415,054.7 ns | 11,754.76 ns |  5,219.19 ns |  1.59 |    0.03 |    3 |         - |          NA |
| PoplarHeapSort   | 8192 | Reversed           |   315,247.3 ns | 13,278.96 ns |  6,945.15 ns |  1.21 |    0.03 |    3 |         - |          NA |
| TournamentSort   | 8192 | Reversed           |   516,280.3 ns | 16,006.26 ns |  8,371.59 ns |  1.98 |    0.05 |    4 |         - |          NA |
| BinomialHeapSort | 8192 | Reversed           |   169,547.2 ns |  1,081.97 ns |    565.89 ns |  0.65 |    0.01 |    2 |         - |          NA |
| PairingHeapSort  | 8192 | Reversed           |    44,444.6 ns |    523.35 ns |    232.37 ns |  0.17 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **PipeOrgan**          |   **274,001.5 ns** |  **8,229.94 ns** |  **4,304.42 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 8192 | PipeOrgan          |   287,071.0 ns | 22,168.31 ns | 11,594.45 ns |  1.05 |    0.04 |    2 |         - |          NA |
| TernaryHeapSort  | 8192 | PipeOrgan          |   332,752.5 ns |  8,270.88 ns |  4,325.83 ns |  1.21 |    0.02 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | PipeOrgan          |   348,882.0 ns |  6,353.55 ns |  2,821.01 ns |  1.27 |    0.02 |    2 |         - |          NA |
| WeakHeapSort     | 8192 | PipeOrgan          |   356,720.1 ns |  3,946.98 ns |  2,064.35 ns |  1.30 |    0.02 |    2 |         - |          NA |
| SmoothSort       | 8192 | PipeOrgan          |   495,461.9 ns | 15,751.14 ns |  6,993.60 ns |  1.81 |    0.04 |    2 |         - |          NA |
| PoplarHeapSort   | 8192 | PipeOrgan          |   421,069.0 ns | 18,557.63 ns |  9,706.00 ns |  1.54 |    0.04 |    2 |         - |          NA |
| TournamentSort   | 8192 | PipeOrgan          |   762,117.2 ns | 17,207.44 ns |  8,999.82 ns |  2.78 |    0.05 |    3 |         - |          NA |
| BinomialHeapSort | 8192 | PipeOrgan          |   181,374.6 ns |  7,761.36 ns |  4,059.34 ns |  0.66 |    0.02 |    1 |         - |          NA |
| PairingHeapSort  | 8192 | PipeOrgan          |   154,786.4 ns |  6,676.60 ns |  2,964.45 ns |  0.57 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **ManyDuplicates**     |   **362,189.9 ns** | **13,471.47 ns** |  **7,045.84 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | ManyDuplicates     |   353,652.4 ns | 10,276.73 ns |  5,374.93 ns |  0.98 |    0.02 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | ManyDuplicates     |   424,494.0 ns | 14,997.46 ns |  7,843.96 ns |  1.17 |    0.03 |    1 |         - |          NA |
| BottomupHeapSort | 8192 | ManyDuplicates     |   464,282.4 ns | 16,312.55 ns |  8,531.78 ns |  1.28 |    0.03 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | ManyDuplicates     |   453,153.2 ns | 15,197.72 ns |  7,948.70 ns |  1.25 |    0.03 |    1 |         - |          NA |
| SmoothSort       | 8192 | ManyDuplicates     |   566,678.1 ns | 13,821.93 ns |  6,137.02 ns |  1.57 |    0.03 |    2 |         - |          NA |
| PoplarHeapSort   | 8192 | ManyDuplicates     |   570,040.3 ns |  6,052.14 ns |  2,687.19 ns |  1.57 |    0.03 |    2 |         - |          NA |
| TournamentSort   | 8192 | ManyDuplicates     | 1,000,930.8 ns | 31,174.33 ns | 13,841.60 ns |  2.76 |    0.06 |    3 |         - |          NA |
| BinomialHeapSort | 8192 | ManyDuplicates     | 1,033,576.5 ns | 84,385.90 ns | 37,467.86 ns |  2.85 |    0.11 |    3 |         - |          NA |
| PairingHeapSort  | 8192 | ManyDuplicates     |   562,841.3 ns | 23,591.48 ns | 12,338.80 ns |  1.55 |    0.04 |    2 |         - |          NA |

### InsertionBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V45 4.27GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.48 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                 | Size | Pattern            | Mean         | Error        | StdDev       | Ratio  | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------- |----- |------------------- |-------------:|-------------:|-------------:|-------:|--------:|-----:|----------:|------------:|
| **InsertionSort**          | **256**  | **Random**             |   **5,042.1 ns** |    **248.14 ns** |    **129.78 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Random             |   5,195.0 ns |    181.16 ns |     80.44 ns |   1.03 |    0.03 |    4 |         - |          NA |
| BinaryInsertSort       | 256  | Random             |   2,742.4 ns |    237.01 ns |    123.96 ns |   0.54 |    0.03 |    3 |         - |          NA |
| GnomeSort              | 256  | Random             |  25,764.5 ns | 10,669.71 ns |  5,580.46 ns |   5.11 |    1.05 |    7 |         - |          NA |
| LibrarySort            | 256  | Random             |  10,487.3 ns |    494.56 ns |    258.66 ns |   2.08 |    0.07 |    6 |         - |          NA |
| MergeInsertionSort     | 256  | Random             |   7,581.5 ns |    225.24 ns |    100.01 ns |   1.50 |    0.04 |    5 |         - |          NA |
| ShellSortKnuth1973     | 256  | Random             |   1,309.0 ns |    182.47 ns |     95.44 ns |   0.26 |    0.02 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Random             |   1,592.2 ns |    413.88 ns |    216.47 ns |   0.32 |    0.04 |    2 |         - |          NA |
| ShellSortTokuda1992    | 256  | Random             |   1,250.4 ns |    104.15 ns |     46.24 ns |   0.25 |    0.01 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | Random             |   1,308.9 ns |    242.49 ns |    107.66 ns |   0.26 |    0.02 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Random             |   1,288.0 ns |    237.27 ns |    124.10 ns |   0.26 |    0.02 |    1 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **InsertionSort**          | **256**  | **SingleElementMoved** |     **188.4 ns** |     **55.73 ns** |     **29.15 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 256  | SingleElementMoved |     287.5 ns |     46.26 ns |     24.19 ns |   1.56 |    0.26 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | SingleElementMoved |     486.4 ns |      5.00 ns |      1.78 ns |   2.64 |    0.38 |    3 |         - |          NA |
| GnomeSort              | 256  | SingleElementMoved |     323.2 ns |     12.71 ns |      4.53 ns |   1.75 |    0.25 |    2 |         - |          NA |
| LibrarySort            | 256  | SingleElementMoved |  18,582.5 ns |    598.93 ns |    265.93 ns | 100.67 |   14.57 |    6 |         - |          NA |
| MergeInsertionSort     | 256  | SingleElementMoved |   7,547.0 ns |    336.33 ns |    149.33 ns |  40.89 |    5.94 |    5 |         - |          NA |
| ShellSortKnuth1973     | 256  | SingleElementMoved |     690.1 ns |    156.16 ns |     69.34 ns |   3.74 |    0.64 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 256  | SingleElementMoved |     695.4 ns |    100.08 ns |     52.34 ns |   3.77 |    0.61 |    4 |         - |          NA |
| ShellSortTokuda1992    | 256  | SingleElementMoved |     831.3 ns |    183.33 ns |     95.88 ns |   4.50 |    0.82 |    4 |         - |          NA |
| ShellSortCiura2001     | 256  | SingleElementMoved |     706.7 ns |    166.34 ns |     73.86 ns |   3.83 |    0.67 |    4 |         - |          NA |
| ShellSortLee2021       | 256  | SingleElementMoved |     712.6 ns |     35.79 ns |     12.76 ns |   3.86 |    0.56 |    4 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **InsertionSort**          | **256**  | **Sorted**             |     **122.5 ns** |     **28.83 ns** |     **15.08 ns** |   **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Sorted             |     103.6 ns |      0.75 ns |      0.33 ns |   0.86 |    0.10 |    1 |         - |          NA |
| BinaryInsertSort       | 256  | Sorted             |     133.7 ns |      2.92 ns |      1.04 ns |   1.11 |    0.13 |    2 |         - |          NA |
| GnomeSort              | 256  | Sorted             |     165.3 ns |     53.07 ns |     23.56 ns |   1.37 |    0.25 |    2 |         - |          NA |
| LibrarySort            | 256  | Sorted             |  18,400.8 ns |    122.38 ns |     54.34 ns | 152.23 |   18.28 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | Sorted             |   7,336.0 ns |    123.84 ns |     64.77 ns |  60.69 |    7.30 |    4 |         - |          NA |
| ShellSortKnuth1973     | 256  | Sorted             |     539.3 ns |     84.94 ns |     44.43 ns |   4.46 |    0.64 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Sorted             |     525.8 ns |    102.47 ns |     45.50 ns |   4.35 |    0.63 |    3 |         - |          NA |
| ShellSortTokuda1992    | 256  | Sorted             |     665.6 ns |    117.97 ns |     61.70 ns |   5.51 |    0.82 |    3 |         - |          NA |
| ShellSortCiura2001     | 256  | Sorted             |     562.5 ns |    133.02 ns |     69.57 ns |   4.65 |    0.78 |    3 |         - |          NA |
| ShellSortLee2021       | 256  | Sorted             |     640.2 ns |    112.43 ns |     58.80 ns |   5.30 |    0.79 |    3 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **InsertionSort**          | **256**  | **Reversed**           |   **9,209.8 ns** |    **232.85 ns** |    **121.78 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Reversed           |  10,608.0 ns |    504.35 ns |    263.78 ns |   1.15 |    0.03 |    4 |         - |          NA |
| BinaryInsertSort       | 256  | Reversed           |   3,707.3 ns |    213.71 ns |    111.77 ns |   0.40 |    0.01 |    2 |         - |          NA |
| GnomeSort              | 256  | Reversed           |  38,101.6 ns |  3,715.10 ns |  1,943.07 ns |   4.14 |    0.21 |    6 |         - |          NA |
| LibrarySort            | 256  | Reversed           |  13,942.6 ns |    173.17 ns |     90.57 ns |   1.51 |    0.02 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | Reversed           |   7,524.6 ns |    281.94 ns |    147.46 ns |   0.82 |    0.02 |    3 |         - |          NA |
| ShellSortKnuth1973     | 256  | Reversed           |   1,030.9 ns |     57.79 ns |     25.66 ns |   0.11 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Reversed           |   1,102.2 ns |    277.17 ns |    144.97 ns |   0.12 |    0.01 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | Reversed           |     987.9 ns |     60.25 ns |     26.75 ns |   0.11 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | Reversed           |   1,123.8 ns |    185.99 ns |     82.58 ns |   0.12 |    0.01 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Reversed           |   1,033.0 ns |    104.01 ns |     54.40 ns |   0.11 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **InsertionSort**          | **256**  | **PipeOrgan**          |   **5,200.0 ns** |    **134.37 ns** |     **59.66 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 256  | PipeOrgan          |   6,419.9 ns |  1,367.55 ns |    715.26 ns |   1.23 |    0.13 |    3 |         - |          NA |
| BinaryInsertSort       | 256  | PipeOrgan          |   1,844.9 ns |     86.79 ns |     38.54 ns |   0.35 |    0.01 |    2 |         - |          NA |
| GnomeSort              | 256  | PipeOrgan          |  17,643.2 ns |  1,262.01 ns |    660.06 ns |   3.39 |    0.13 |    6 |         - |          NA |
| LibrarySort            | 256  | PipeOrgan          |  12,964.5 ns |    330.04 ns |    172.62 ns |   2.49 |    0.04 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | PipeOrgan          |   7,916.2 ns |    431.16 ns |    225.51 ns |   1.52 |    0.04 |    4 |         - |          NA |
| ShellSortKnuth1973     | 256  | PipeOrgan          |     943.1 ns |    195.21 ns |    102.10 ns |   0.18 |    0.02 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | PipeOrgan          |   1,006.6 ns |    207.03 ns |    108.28 ns |   0.19 |    0.02 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | PipeOrgan          |   1,000.6 ns |    212.09 ns |     94.17 ns |   0.19 |    0.02 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | PipeOrgan          |   1,133.9 ns |    305.72 ns |    159.90 ns |   0.22 |    0.03 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | PipeOrgan          |   1,056.5 ns |    152.59 ns |     79.81 ns |   0.20 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **InsertionSort**          | **256**  | **ManyDuplicates**     |   **4,898.3 ns** |    **360.75 ns** |    **160.18 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 256  | ManyDuplicates     |   5,526.0 ns |    411.16 ns |    182.56 ns |   1.13 |    0.05 |    3 |         - |          NA |
| BinaryInsertSort       | 256  | ManyDuplicates     |   2,586.0 ns |    286.04 ns |    149.61 ns |   0.53 |    0.03 |    2 |         - |          NA |
| GnomeSort              | 256  | ManyDuplicates     |  17,753.7 ns |    936.38 ns |    489.74 ns |   3.63 |    0.14 |    6 |         - |          NA |
| LibrarySort            | 256  | ManyDuplicates     |  10,412.6 ns |    538.54 ns |    281.67 ns |   2.13 |    0.08 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | ManyDuplicates     |   7,169.3 ns |    191.40 ns |    100.11 ns |   1.46 |    0.05 |    4 |         - |          NA |
| ShellSortKnuth1973     | 256  | ManyDuplicates     |   1,160.6 ns |    307.29 ns |    160.72 ns |   0.24 |    0.03 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | ManyDuplicates     |   1,297.7 ns |    243.99 ns |    127.61 ns |   0.27 |    0.03 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | ManyDuplicates     |   1,057.8 ns |    161.29 ns |     84.36 ns |   0.22 |    0.02 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | ManyDuplicates     |   1,126.6 ns |    241.34 ns |    126.23 ns |   0.23 |    0.03 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | ManyDuplicates     |   1,000.1 ns |    115.89 ns |     60.61 ns |   0.20 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **InsertionSort**          | **1024** | **Random**             |  **76,094.7 ns** |    **499.64 ns** |    **261.32 ns** |   **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Random             |  85,160.2 ns |  2,557.20 ns |  1,337.47 ns |   1.12 |    0.02 |    5 |         - |          NA |
| BinaryInsertSort       | 1024 | Random             |  18,570.3 ns |  1,416.24 ns |    740.72 ns |   0.24 |    0.01 |    3 |         - |          NA |
| GnomeSort              | 1024 | Random             | 262,855.8 ns | 21,781.07 ns | 11,391.92 ns |   3.45 |    0.14 |    6 |         - |          NA |
| LibrarySort            | 1024 | Random             |  50,014.7 ns |  9,549.98 ns |  4,994.82 ns |   0.66 |    0.06 |    4 |         - |          NA |
| MergeInsertionSort     | 1024 | Random             |  41,807.8 ns |    939.88 ns |    417.31 ns |   0.55 |    0.01 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Random             |   8,766.1 ns |  2,735.92 ns |  1,430.94 ns |   0.12 |    0.02 |    2 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Random             |   8,207.5 ns |  1,567.09 ns |    819.62 ns |   0.11 |    0.01 |    2 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Random             |   6,447.8 ns |    311.07 ns |    138.12 ns |   0.08 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Random             |   6,350.2 ns |    416.64 ns |    184.99 ns |   0.08 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Random             |   6,411.2 ns |    564.35 ns |    295.17 ns |   0.08 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **InsertionSort**          | **1024** | **SingleElementMoved** |     **619.3 ns** |     **39.23 ns** |     **20.52 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | SingleElementMoved |     732.0 ns |     18.45 ns |      6.58 ns |   1.18 |    0.04 |    1 |         - |          NA |
| BinaryInsertSort       | 1024 | SingleElementMoved |   2,342.5 ns |    242.34 ns |    126.75 ns |   3.79 |    0.23 |    3 |         - |          NA |
| GnomeSort              | 1024 | SingleElementMoved |   1,142.6 ns |     37.79 ns |     19.77 ns |   1.85 |    0.07 |    2 |         - |          NA |
| LibrarySort            | 1024 | SingleElementMoved | 138,743.5 ns |  4,552.51 ns |  2,381.05 ns | 224.27 |    8.03 |    6 |         - |          NA |
| MergeInsertionSort     | 1024 | SingleElementMoved |  34,142.2 ns |    633.70 ns |    281.37 ns |  55.19 |    1.81 |    5 |         - |          NA |
| ShellSortKnuth1973     | 1024 | SingleElementMoved |   3,266.9 ns |    479.06 ns |    250.56 ns |   5.28 |    0.42 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | SingleElementMoved |   3,317.9 ns |    477.99 ns |    250.00 ns |   5.36 |    0.42 |    4 |         - |          NA |
| ShellSortTokuda1992    | 1024 | SingleElementMoved |   3,322.8 ns |    520.46 ns |    272.21 ns |   5.37 |    0.45 |    4 |         - |          NA |
| ShellSortCiura2001     | 1024 | SingleElementMoved |   3,565.1 ns |    772.56 ns |    404.06 ns |   5.76 |    0.64 |    4 |         - |          NA |
| ShellSortLee2021       | 1024 | SingleElementMoved |   3,673.2 ns |    749.00 ns |    391.74 ns |   5.94 |    0.63 |    4 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **InsertionSort**          | **1024** | **Sorted**             |     **432.3 ns** |    **112.81 ns** |     **59.00 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Sorted             |     412.7 ns |     52.55 ns |     27.49 ns |   0.97 |    0.13 |    1 |         - |          NA |
| BinaryInsertSort       | 1024 | Sorted             |     633.7 ns |    409.67 ns |    181.90 ns |   1.49 |    0.44 |    2 |         - |          NA |
| GnomeSort              | 1024 | Sorted             |     489.8 ns |     11.10 ns |      4.93 ns |   1.15 |    0.14 |    2 |         - |          NA |
| LibrarySort            | 1024 | Sorted             | 139,275.0 ns |  8,613.17 ns |  4,504.86 ns | 327.20 |   41.01 |    5 |         - |          NA |
| MergeInsertionSort     | 1024 | Sorted             |  32,933.9 ns |    325.15 ns |    144.37 ns |  77.37 |    9.42 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Sorted             |   2,524.3 ns |    700.39 ns |    366.32 ns |   5.93 |    1.09 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Sorted             |   2,479.3 ns |    658.97 ns |    344.65 ns |   5.82 |    1.05 |    3 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Sorted             |   2,675.9 ns |    840.55 ns |    439.63 ns |   6.29 |    1.24 |    3 |         - |          NA |
| ShellSortCiura2001     | 1024 | Sorted             |   2,702.6 ns |    692.31 ns |    362.09 ns |   6.35 |    1.12 |    3 |         - |          NA |
| ShellSortLee2021       | 1024 | Sorted             |   2,670.4 ns |    587.94 ns |    307.50 ns |   6.27 |    1.03 |    3 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **InsertionSort**          | **1024** | **Reversed**           | **132,647.8 ns** |  **1,696.09 ns** |    **753.08 ns** |   **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Reversed           | 147,385.6 ns |  5,152.97 ns |  2,287.95 ns |   1.11 |    0.02 |    5 |         - |          NA |
| BinaryInsertSort       | 1024 | Reversed           |  24,484.7 ns |    771.27 ns |    403.39 ns |   0.18 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | Reversed           | 484,055.0 ns | 15,826.62 ns |  7,027.12 ns |   3.65 |    0.05 |    6 |         - |          NA |
| LibrarySort            | 1024 | Reversed           | 106,979.3 ns |  2,467.41 ns |  1,290.50 ns |   0.81 |    0.01 |    4 |         - |          NA |
| MergeInsertionSort     | 1024 | Reversed           |  31,681.8 ns |    267.41 ns |    139.86 ns |   0.24 |    0.00 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Reversed           |   4,592.8 ns |    285.84 ns |    149.50 ns |   0.03 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Reversed           |   4,968.6 ns |    441.10 ns |    230.70 ns |   0.04 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Reversed           |   5,722.4 ns |    766.46 ns |    400.87 ns |   0.04 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Reversed           |   5,336.3 ns |    592.36 ns |    263.01 ns |   0.04 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Reversed           |   5,661.7 ns |    587.27 ns |    307.16 ns |   0.04 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **InsertionSort**          | **1024** | **PipeOrgan**          |  **70,856.0 ns** |  **3,385.78 ns** |  **1,770.83 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | PipeOrgan          |  73,494.7 ns |  3,839.94 ns |  2,008.37 ns |   1.04 |    0.04 |    4 |         - |          NA |
| BinaryInsertSort       | 1024 | PipeOrgan          |  12,187.8 ns |  1,112.49 ns |    581.86 ns |   0.17 |    0.01 |    2 |         - |          NA |
| GnomeSort              | 1024 | PipeOrgan          | 244,964.4 ns |  8,225.81 ns |  4,302.26 ns |   3.46 |    0.10 |    5 |         - |          NA |
| LibrarySort            | 1024 | PipeOrgan          |  73,043.3 ns |  4,457.26 ns |  2,331.23 ns |   1.03 |    0.04 |    4 |         - |          NA |
| MergeInsertionSort     | 1024 | PipeOrgan          |  33,732.6 ns |    525.59 ns |    233.36 ns |   0.48 |    0.01 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | PipeOrgan          |   4,997.3 ns |  1,063.09 ns |    556.02 ns |   0.07 |    0.01 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | PipeOrgan          |   5,145.8 ns |    937.91 ns |    490.55 ns |   0.07 |    0.01 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | PipeOrgan          |   5,369.1 ns |    611.85 ns |    320.01 ns |   0.08 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | PipeOrgan          |   5,500.9 ns |    605.02 ns |    316.44 ns |   0.08 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | PipeOrgan          |   5,859.5 ns |    672.57 ns |    351.77 ns |   0.08 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |              |        |         |      |           |             |
| **InsertionSort**          | **1024** | **ManyDuplicates**     |  **76,234.9 ns** |    **626.59 ns** |    **278.21 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | ManyDuplicates     |  78,616.4 ns |  4,083.63 ns |  2,135.82 ns |   1.03 |    0.03 |    4 |         - |          NA |
| BinaryInsertSort       | 1024 | ManyDuplicates     |  17,298.0 ns |    884.90 ns |    462.82 ns |   0.23 |    0.01 |    2 |         - |          NA |
| GnomeSort              | 1024 | ManyDuplicates     | 261,156.9 ns | 13,290.91 ns |  6,951.40 ns |   3.43 |    0.09 |    5 |         - |          NA |
| LibrarySort            | 1024 | ManyDuplicates     |  48,909.5 ns |  3,346.07 ns |  1,485.68 ns |   0.64 |    0.02 |    3 |         - |          NA |
| MergeInsertionSort     | 1024 | ManyDuplicates     |  42,298.6 ns |    797.43 ns |    354.06 ns |   0.55 |    0.00 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | ManyDuplicates     |   5,435.2 ns |    643.83 ns |    336.74 ns |   0.07 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | ManyDuplicates     |   5,297.3 ns |  1,058.27 ns |    553.50 ns |   0.07 |    0.01 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | ManyDuplicates     |   5,109.3 ns |    715.54 ns |    374.24 ns |   0.07 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | ManyDuplicates     |   5,015.1 ns |    729.57 ns |    381.58 ns |   0.07 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | ManyDuplicates     |   5,098.2 ns |    606.90 ns |    269.47 ns |   0.07 |    0.00 |    1 |         - |          NA |

### MergeBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V45 4.27GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.48 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                   | Size | Pattern            | Mean            | Error         | StdDev        | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------------- |----- |------------------- |----------------:|--------------:|--------------:|------:|--------:|-----:|----------:|------------:|
| **MergeSort**                | **256**  | **Random**             |     **5,439.20 ns** |    **169.634 ns** |     **75.318 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Random             |     5,215.62 ns |    365.054 ns |    190.930 ns |  0.96 |    0.04 |    3 |         - |          NA |
| BottomupMergeSort        | 256  | Random             |     2,799.64 ns |     67.502 ns |     35.305 ns |  0.51 |    0.01 |    1 |         - |          NA |
| StdStableSort            | 256  | Random             |     1,425.46 ns |     71.990 ns |     31.964 ns |  0.26 |    0.01 |    1 |         - |          NA |
| RotateMergeSort          | 256  | Random             |     6,760.03 ns |    475.689 ns |    211.209 ns |  1.24 |    0.04 |    4 |         - |          NA |
| RotateMergeSortRecursive | 256  | Random             |     6,819.69 ns |    156.618 ns |     81.914 ns |  1.25 |    0.02 |    4 |         - |          NA |
| SymMergeSort             | 256  | Random             |     3,999.52 ns |    168.317 ns |     88.033 ns |  0.74 |    0.02 |    2 |         - |          NA |
| BlockMergeSort           | 256  | Random             |     2,761.53 ns |     87.714 ns |     45.876 ns |  0.51 |    0.01 |    1 |         - |          NA |
| NaturalMergeSort         | 256  | Random             |     2,975.16 ns |    319.592 ns |    167.153 ns |  0.55 |    0.03 |    1 |         - |          NA |
| PatienceSort             | 256  | Random             |     2,696.19 ns |    146.264 ns |     64.942 ns |  0.50 |    0.01 |    1 |         - |          NA |
| DropMergeSort            | 256  | Random             |     1,662.36 ns |    257.464 ns |    134.659 ns |  0.31 |    0.02 |    1 |         - |          NA |
| TimSort                  | 256  | Random             |     2,093.94 ns |    222.695 ns |    116.474 ns |  0.39 |    0.02 |    1 |         - |          NA |
| PowerSort                | 256  | Random             |     1,193.68 ns |    227.956 ns |    101.214 ns |  0.22 |    0.02 |    1 |         - |          NA |
| ShiftSort                | 256  | Random             |     3,303.15 ns |    217.827 ns |    113.928 ns |  0.61 |    0.02 |    1 |         - |          NA |
| SpinSort                 | 256  | Random             |     1,285.06 ns |    210.297 ns |     93.373 ns |  0.24 |    0.02 |    1 |         - |          NA |
| Glidesort                | 256  | Random             |     1,878.27 ns |    228.176 ns |    119.341 ns |  0.35 |    0.02 |    1 |         - |          NA |
| Driftsort                | 256  | Random             |     2,302.49 ns |     40.957 ns |     18.185 ns |  0.42 |    0.01 |    1 |         - |          NA |
| FlatStableSort           | 256  | Random             |     1,414.44 ns |    106.645 ns |     47.351 ns |  0.26 |    0.01 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **MergeSort**                | **256**  | **SingleElementMoved** |     **2,998.09 ns** |     **75.782 ns** |     **33.648 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | SingleElementMoved |     3,459.99 ns |    261.240 ns |    136.633 ns |  1.15 |    0.04 |    8 |         - |          NA |
| BottomupMergeSort        | 256  | SingleElementMoved |     1,013.82 ns |     77.348 ns |     27.583 ns |  0.34 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 256  | SingleElementMoved |       383.88 ns |     11.342 ns |      5.036 ns |  0.13 |    0.00 |    3 |         - |          NA |
| RotateMergeSort          | 256  | SingleElementMoved |       305.16 ns |      6.843 ns |      3.579 ns |  0.10 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 256  | SingleElementMoved |       388.33 ns |      7.371 ns |      3.273 ns |  0.13 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 256  | SingleElementMoved |       228.15 ns |      8.861 ns |      3.934 ns |  0.08 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 256  | SingleElementMoved |     1,944.32 ns |    377.731 ns |    197.561 ns |  0.65 |    0.06 |    7 |         - |          NA |
| NaturalMergeSort         | 256  | SingleElementMoved |       368.40 ns |     41.936 ns |     21.934 ns |  0.12 |    0.01 |    3 |         - |          NA |
| PatienceSort             | 256  | SingleElementMoved |     4,592.58 ns |    470.162 ns |    245.904 ns |  1.53 |    0.08 |    9 |         - |          NA |
| DropMergeSort            | 256  | SingleElementMoved |       385.70 ns |     62.393 ns |     32.633 ns |  0.13 |    0.01 |    3 |         - |          NA |
| TimSort                  | 256  | SingleElementMoved |       145.31 ns |     25.922 ns |     11.510 ns |  0.05 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | SingleElementMoved |       256.60 ns |     34.729 ns |     18.164 ns |  0.09 |    0.01 |    2 |         - |          NA |
| ShiftSort                | 256  | SingleElementMoved |       300.12 ns |     21.652 ns |      9.614 ns |  0.10 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 256  | SingleElementMoved |       487.87 ns |     18.140 ns |      9.487 ns |  0.16 |    0.00 |    4 |         - |          NA |
| Glidesort                | 256  | SingleElementMoved |       756.39 ns |     49.112 ns |     21.806 ns |  0.25 |    0.01 |    5 |         - |          NA |
| Driftsort                | 256  | SingleElementMoved |       680.20 ns |     59.838 ns |     26.569 ns |  0.23 |    0.01 |    5 |         - |          NA |
| FlatStableSort           | 256  | SingleElementMoved |       943.88 ns |    640.101 ns |    284.209 ns |  0.31 |    0.09 |    5 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **MergeSort**                | **256**  | **Sorted**             |     **2,676.05 ns** |      **3.670 ns** |      **1.309 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Sorted             |     3,155.77 ns |     86.170 ns |     38.260 ns |  1.18 |    0.01 |    8 |         - |          NA |
| BottomupMergeSort        | 256  | Sorted             |       748.89 ns |      3.468 ns |      1.237 ns |  0.28 |    0.00 |    6 |         - |          NA |
| StdStableSort            | 256  | Sorted             |       317.38 ns |      2.788 ns |      0.994 ns |  0.12 |    0.00 |    4 |         - |          NA |
| RotateMergeSort          | 256  | Sorted             |       140.35 ns |      2.096 ns |      0.931 ns |  0.05 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 256  | Sorted             |       212.22 ns |     13.538 ns |      4.828 ns |  0.08 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 256  | Sorted             |       160.12 ns |     13.952 ns |      7.297 ns |  0.06 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 256  | Sorted             |     1,561.15 ns |     14.937 ns |      6.632 ns |  0.58 |    0.00 |    7 |         - |          NA |
| NaturalMergeSort         | 256  | Sorted             |       155.30 ns |      6.785 ns |      3.012 ns |  0.06 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 256  | Sorted             |     4,418.80 ns |    252.295 ns |    131.955 ns |  1.65 |    0.05 |    9 |         - |          NA |
| DropMergeSort            | 256  | Sorted             |       296.13 ns |     15.566 ns |      8.142 ns |  0.11 |    0.00 |    4 |         - |          NA |
| TimSort                  | 256  | Sorted             |       101.70 ns |      1.605 ns |      0.713 ns |  0.04 |    0.00 |    2 |         - |          NA |
| PowerSort                | 256  | Sorted             |        98.95 ns |      1.487 ns |      0.660 ns |  0.04 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 256  | Sorted             |       195.88 ns |     33.613 ns |     14.924 ns |  0.07 |    0.01 |    3 |         - |          NA |
| SpinSort                 | 256  | Sorted             |        78.58 ns |      1.591 ns |      0.567 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 256  | Sorted             |       117.97 ns |      4.735 ns |      2.103 ns |  0.04 |    0.00 |    2 |         - |          NA |
| Driftsort                | 256  | Sorted             |       138.45 ns |     14.142 ns |      7.397 ns |  0.05 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 256  | Sorted             |       607.75 ns |     18.444 ns |      8.189 ns |  0.23 |    0.00 |    5 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **MergeSort**                | **256**  | **Reversed**           |     **5,727.40 ns** |    **374.199 ns** |    **166.147 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Reversed           |     5,279.18 ns |    522.246 ns |    273.145 ns |  0.92 |    0.05 |    5 |         - |          NA |
| BottomupMergeSort        | 256  | Reversed           |     2,771.44 ns |     22.907 ns |     10.171 ns |  0.48 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 256  | Reversed           |     2,546.86 ns |     66.372 ns |     29.470 ns |  0.44 |    0.01 |    4 |         - |          NA |
| RotateMergeSort          | 256  | Reversed           |     1,410.29 ns |     25.758 ns |     13.472 ns |  0.25 |    0.01 |    3 |         - |          NA |
| RotateMergeSortRecursive | 256  | Reversed           |     1,216.26 ns |     33.142 ns |     11.819 ns |  0.21 |    0.01 |    3 |         - |          NA |
| SymMergeSort             | 256  | Reversed           |     1,356.69 ns |     26.604 ns |     11.812 ns |  0.24 |    0.01 |    3 |         - |          NA |
| BlockMergeSort           | 256  | Reversed           |     1,865.20 ns |     29.595 ns |     13.141 ns |  0.33 |    0.01 |    3 |         - |          NA |
| NaturalMergeSort         | 256  | Reversed           |       181.01 ns |      3.949 ns |      1.754 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 256  | Reversed           |       883.47 ns |      6.864 ns |      2.448 ns |  0.15 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 256  | Reversed           |       880.75 ns |     75.528 ns |     33.535 ns |  0.15 |    0.01 |    2 |         - |          NA |
| TimSort                  | 256  | Reversed           |       157.23 ns |     29.745 ns |     15.557 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | Reversed           |       162.06 ns |     25.369 ns |     13.268 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Reversed           |     2,656.80 ns |     65.934 ns |     29.275 ns |  0.46 |    0.01 |    4 |         - |          NA |
| SpinSort                 | 256  | Reversed           |       175.67 ns |      3.483 ns |      1.546 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 256  | Reversed           |       202.28 ns |     11.783 ns |      6.163 ns |  0.04 |    0.00 |    1 |         - |          NA |
| Driftsort                | 256  | Reversed           |       158.03 ns |      9.245 ns |      4.105 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | Reversed           |     1,649.52 ns |     36.926 ns |     16.395 ns |  0.29 |    0.01 |    3 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **MergeSort**                | **256**  | **PipeOrgan**          |     **4,357.38 ns** |    **404.572 ns** |    **211.599 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | PipeOrgan          |     4,048.86 ns |    167.075 ns |     74.182 ns |  0.93 |    0.05 |    8 |         - |          NA |
| BottomupMergeSort        | 256  | PipeOrgan          |     1,791.97 ns |     71.884 ns |     31.917 ns |  0.41 |    0.02 |    6 |         - |          NA |
| StdStableSort            | 256  | PipeOrgan          |     1,446.33 ns |     87.933 ns |     39.043 ns |  0.33 |    0.02 |    6 |         - |          NA |
| RotateMergeSort          | 256  | PipeOrgan          |     2,794.61 ns |     41.141 ns |     21.518 ns |  0.64 |    0.03 |    7 |         - |          NA |
| RotateMergeSortRecursive | 256  | PipeOrgan          |     2,993.83 ns |     54.054 ns |     24.000 ns |  0.69 |    0.03 |    7 |         - |          NA |
| SymMergeSort             | 256  | PipeOrgan          |     2,230.44 ns |    268.055 ns |    140.198 ns |  0.51 |    0.04 |    6 |         - |          NA |
| BlockMergeSort           | 256  | PipeOrgan          |     1,871.21 ns |      9.847 ns |      3.512 ns |  0.43 |    0.02 |    6 |         - |          NA |
| NaturalMergeSort         | 256  | PipeOrgan          |       365.17 ns |     14.613 ns |      6.488 ns |  0.08 |    0.00 |    3 |         - |          NA |
| PatienceSort             | 256  | PipeOrgan          |     3,263.97 ns |    330.023 ns |    172.608 ns |  0.75 |    0.05 |    7 |         - |          NA |
| DropMergeSort            | 256  | PipeOrgan          |     3,340.90 ns |    402.429 ns |    210.478 ns |  0.77 |    0.06 |    7 |         - |          NA |
| TimSort                  | 256  | PipeOrgan          |       476.24 ns |     45.029 ns |     16.058 ns |  0.11 |    0.01 |    4 |         - |          NA |
| PowerSort                | 256  | PipeOrgan          |       299.96 ns |     36.802 ns |     16.340 ns |  0.07 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 256  | PipeOrgan          |     1,702.61 ns |    322.094 ns |    168.461 ns |  0.39 |    0.04 |    6 |         - |          NA |
| SpinSort                 | 256  | PipeOrgan          |     1,080.80 ns |     43.686 ns |     19.397 ns |  0.25 |    0.01 |    6 |         - |          NA |
| Glidesort                | 256  | PipeOrgan          |       833.58 ns |     14.239 ns |      6.322 ns |  0.19 |    0.01 |    5 |         - |          NA |
| Driftsort                | 256  | PipeOrgan          |       231.39 ns |      5.973 ns |      2.130 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | PipeOrgan          |     1,263.74 ns |     87.347 ns |     38.783 ns |  0.29 |    0.02 |    6 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **MergeSort**                | **256**  | **ManyDuplicates**     |     **5,301.22 ns** |    **733.572 ns** |    **383.672 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | ManyDuplicates     |     4,842.85 ns |    206.326 ns |     91.610 ns |  0.92 |    0.06 |    3 |         - |          NA |
| BottomupMergeSort        | 256  | ManyDuplicates     |     2,824.17 ns |     32.612 ns |     14.480 ns |  0.54 |    0.03 |    2 |         - |          NA |
| StdStableSort            | 256  | ManyDuplicates     |     2,478.76 ns |    111.871 ns |     49.671 ns |  0.47 |    0.03 |    2 |         - |          NA |
| RotateMergeSort          | 256  | ManyDuplicates     |     5,719.66 ns |    162.557 ns |     72.176 ns |  1.08 |    0.07 |    3 |         - |          NA |
| RotateMergeSortRecursive | 256  | ManyDuplicates     |     6,156.40 ns |    410.526 ns |    214.713 ns |  1.17 |    0.08 |    3 |         - |          NA |
| SymMergeSort             | 256  | ManyDuplicates     |     3,622.09 ns |    347.513 ns |    181.756 ns |  0.69 |    0.05 |    2 |         - |          NA |
| BlockMergeSort           | 256  | ManyDuplicates     |     2,830.95 ns |    147.655 ns |     65.560 ns |  0.54 |    0.04 |    2 |         - |          NA |
| NaturalMergeSort         | 256  | ManyDuplicates     |     2,998.78 ns |    275.811 ns |    144.254 ns |  0.57 |    0.04 |    2 |         - |          NA |
| PatienceSort             | 256  | ManyDuplicates     |     2,140.89 ns |     69.610 ns |     24.824 ns |  0.41 |    0.03 |    2 |         - |          NA |
| DropMergeSort            | 256  | ManyDuplicates     |     1,377.99 ns |     51.613 ns |     18.406 ns |  0.26 |    0.02 |    1 |         - |          NA |
| TimSort                  | 256  | ManyDuplicates     |     1,845.25 ns |    106.366 ns |     55.631 ns |  0.35 |    0.02 |    2 |         - |          NA |
| PowerSort                | 256  | ManyDuplicates     |     1,092.86 ns |     43.490 ns |     19.310 ns |  0.21 |    0.01 |    1 |         - |          NA |
| ShiftSort                | 256  | ManyDuplicates     |     2,280.47 ns |     38.597 ns |     13.764 ns |  0.43 |    0.03 |    2 |         - |          NA |
| SpinSort                 | 256  | ManyDuplicates     |     1,213.08 ns |    153.044 ns |     80.045 ns |  0.23 |    0.02 |    1 |         - |          NA |
| Glidesort                | 256  | ManyDuplicates     |     1,709.71 ns |    229.992 ns |    120.290 ns |  0.32 |    0.03 |    2 |         - |          NA |
| Driftsort                | 256  | ManyDuplicates     |     2,283.36 ns |    201.513 ns |     89.473 ns |  0.43 |    0.03 |    2 |         - |          NA |
| FlatStableSort           | 256  | ManyDuplicates     |     1,433.85 ns |    259.818 ns |    135.890 ns |  0.27 |    0.03 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **MergeSort**                | **1024** | **Random**             |    **22,182.16 ns** |  **1,295.258 ns** |    **677.445 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Random             |    20,465.55 ns |    506.272 ns |    224.788 ns |  0.92 |    0.03 |    3 |         - |          NA |
| BottomupMergeSort        | 1024 | Random             |    17,769.99 ns |    314.220 ns |    139.516 ns |  0.80 |    0.02 |    3 |         - |          NA |
| StdStableSort            | 1024 | Random             |     6,844.26 ns |  1,197.656 ns |    531.767 ns |  0.31 |    0.02 |    1 |         - |          NA |
| RotateMergeSort          | 1024 | Random             |    38,485.07 ns |    771.055 ns |    342.353 ns |  1.74 |    0.05 |    4 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Random             |    39,842.48 ns |    515.550 ns |    228.907 ns |  1.80 |    0.05 |    4 |         - |          NA |
| SymMergeSort             | 1024 | Random             |    24,233.03 ns |    877.372 ns |    458.882 ns |  1.09 |    0.04 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | Random             |    13,375.31 ns |    486.604 ns |    216.055 ns |  0.60 |    0.02 |    2 |         - |          NA |
| NaturalMergeSort         | 1024 | Random             |    12,733.05 ns |  1,169.424 ns |    611.631 ns |  0.57 |    0.03 |    2 |         - |          NA |
| PatienceSort             | 1024 | Random             |    11,801.10 ns |    732.208 ns |    325.105 ns |  0.53 |    0.02 |    2 |         - |          NA |
| DropMergeSort            | 1024 | Random             |     7,065.61 ns |    512.192 ns |    267.886 ns |  0.32 |    0.01 |    1 |         - |          NA |
| TimSort                  | 1024 | Random             |    10,078.28 ns |    566.027 ns |    296.043 ns |  0.45 |    0.02 |    2 |         - |          NA |
| PowerSort                | 1024 | Random             |     5,841.34 ns |    678.914 ns |    355.085 ns |  0.26 |    0.02 |    1 |         - |          NA |
| ShiftSort                | 1024 | Random             |    24,295.78 ns | 13,659.678 ns |  7,144.276 ns |  1.10 |    0.31 |    3 |         - |          NA |
| SpinSort                 | 1024 | Random             |     5,963.30 ns |    421.109 ns |    220.248 ns |  0.27 |    0.01 |    1 |         - |          NA |
| Glidesort                | 1024 | Random             |     7,293.76 ns |    268.284 ns |    140.317 ns |  0.33 |    0.01 |    1 |         - |          NA |
| Driftsort                | 1024 | Random             |     9,886.00 ns |    283.354 ns |    125.811 ns |  0.45 |    0.01 |    2 |         - |          NA |
| FlatStableSort           | 1024 | Random             |     6,243.04 ns |    406.485 ns |    180.482 ns |  0.28 |    0.01 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **MergeSort**                | **1024** | **SingleElementMoved** |    **11,956.40 ns** |    **158.086 ns** |     **70.191 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | SingleElementMoved |    13,759.90 ns |    184.754 ns |     96.630 ns |  1.15 |    0.01 |    7 |         - |          NA |
| BottomupMergeSort        | 1024 | SingleElementMoved |     3,657.82 ns |    402.543 ns |    210.538 ns |  0.31 |    0.02 |    5 |         - |          NA |
| StdStableSort            | 1024 | SingleElementMoved |     2,274.45 ns |    209.797 ns |    109.728 ns |  0.19 |    0.01 |    4 |         - |          NA |
| RotateMergeSort          | 1024 | SingleElementMoved |       858.66 ns |     30.601 ns |     13.587 ns |  0.07 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 1024 | SingleElementMoved |     1,229.76 ns |     45.207 ns |     20.072 ns |  0.10 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 1024 | SingleElementMoved |       757.64 ns |     37.389 ns |     16.601 ns |  0.06 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 1024 | SingleElementMoved |     8,018.16 ns |    216.338 ns |    113.149 ns |  0.67 |    0.01 |    6 |         - |          NA |
| NaturalMergeSort         | 1024 | SingleElementMoved |     1,120.83 ns |    205.905 ns |     73.428 ns |  0.09 |    0.01 |    3 |         - |          NA |
| PatienceSort             | 1024 | SingleElementMoved |    22,292.10 ns |  1,161.055 ns |    607.254 ns |  1.86 |    0.05 |    8 |         - |          NA |
| DropMergeSort            | 1024 | SingleElementMoved |     1,232.07 ns |    204.457 ns |    106.935 ns |  0.10 |    0.01 |    3 |         - |          NA |
| TimSort                  | 1024 | SingleElementMoved |       494.88 ns |     42.928 ns |     19.060 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | SingleElementMoved |       840.18 ns |     16.054 ns |      7.128 ns |  0.07 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 1024 | SingleElementMoved |     1,138.92 ns |    373.124 ns |    195.151 ns |  0.10 |    0.02 |    3 |         - |          NA |
| SpinSort                 | 1024 | SingleElementMoved |     2,192.53 ns |    164.930 ns |     86.262 ns |  0.18 |    0.01 |    4 |         - |          NA |
| Glidesort                | 1024 | SingleElementMoved |     2,057.75 ns |    216.424 ns |     96.094 ns |  0.17 |    0.01 |    4 |         - |          NA |
| Driftsort                | 1024 | SingleElementMoved |       721.33 ns |     43.523 ns |     22.763 ns |  0.06 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 1024 | SingleElementMoved |     3,173.50 ns |     66.930 ns |     29.717 ns |  0.27 |    0.00 |    5 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **MergeSort**                | **1024** | **Sorted**             |    **10,708.42 ns** |    **122.394 ns** |     **54.344 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Sorted             |    13,002.09 ns |    241.380 ns |    126.246 ns |  1.21 |    0.01 |    8 |         - |          NA |
| BottomupMergeSort        | 1024 | Sorted             |     2,931.48 ns |     38.245 ns |     16.981 ns |  0.27 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 1024 | Sorted             |     1,617.31 ns |     92.265 ns |     40.966 ns |  0.15 |    0.00 |    4 |         - |          NA |
| RotateMergeSort          | 1024 | Sorted             |       540.51 ns |     20.359 ns |      7.260 ns |  0.05 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Sorted             |       932.50 ns |    120.248 ns |     62.892 ns |  0.09 |    0.01 |    3 |         - |          NA |
| SymMergeSort             | 1024 | Sorted             |       521.93 ns |      3.147 ns |      1.122 ns |  0.05 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 1024 | Sorted             |     6,886.87 ns |    413.041 ns |    216.028 ns |  0.64 |    0.02 |    6 |         - |          NA |
| NaturalMergeSort         | 1024 | Sorted             |       331.08 ns |     21.124 ns |      9.379 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 1024 | Sorted             |    22,149.95 ns |  2,100.485 ns |  1,098.594 ns |  2.07 |    0.10 |    9 |         - |          NA |
| DropMergeSort            | 1024 | Sorted             |     1,023.04 ns |     46.122 ns |     20.478 ns |  0.10 |    0.00 |    3 |         - |          NA |
| TimSort                  | 1024 | Sorted             |       313.08 ns |      7.887 ns |      3.502 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Sorted             |       287.73 ns |      9.390 ns |      4.169 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Sorted             |       552.28 ns |     15.371 ns |      6.825 ns |  0.05 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 1024 | Sorted             |       266.56 ns |     37.567 ns |     16.680 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Sorted             |       321.40 ns |     13.264 ns |      6.937 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | Sorted             |       350.10 ns |     13.284 ns |      6.948 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Sorted             |     2,473.54 ns |     51.816 ns |     23.007 ns |  0.23 |    0.00 |    5 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **MergeSort**                | **1024** | **Reversed**           |    **23,472.75 ns** |    **712.587 ns** |    **372.697 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Reversed           |    21,069.67 ns |    843.847 ns |    374.673 ns |  0.90 |    0.02 |    6 |         - |          NA |
| BottomupMergeSort        | 1024 | Reversed           |    11,358.11 ns |    397.358 ns |    207.826 ns |  0.48 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 1024 | Reversed           |     9,995.35 ns |    318.695 ns |    166.683 ns |  0.43 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 1024 | Reversed           |     6,465.41 ns |    284.599 ns |    148.851 ns |  0.28 |    0.01 |    4 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Reversed           |     5,848.43 ns |    378.348 ns |    167.989 ns |  0.25 |    0.01 |    4 |         - |          NA |
| SymMergeSort             | 1024 | Reversed           |     5,964.18 ns |    202.377 ns |    105.847 ns |  0.25 |    0.01 |    4 |         - |          NA |
| BlockMergeSort           | 1024 | Reversed           |     8,998.65 ns |    281.910 ns |    147.445 ns |  0.38 |    0.01 |    5 |         - |          NA |
| NaturalMergeSort         | 1024 | Reversed           |       506.19 ns |      6.275 ns |      2.786 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 1024 | Reversed           |     2,871.79 ns |     24.186 ns |     10.739 ns |  0.12 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 1024 | Reversed           |     4,093.96 ns |    404.282 ns |    211.447 ns |  0.17 |    0.01 |    3 |         - |          NA |
| TimSort                  | 1024 | Reversed           |       465.17 ns |      5.107 ns |      2.268 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Reversed           |       534.47 ns |      5.205 ns |      2.311 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Reversed           |    10,836.46 ns |     39.782 ns |     14.187 ns |  0.46 |    0.01 |    5 |         - |          NA |
| SpinSort                 | 1024 | Reversed           |       631.44 ns |      3.322 ns |      1.475 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Reversed           |       517.82 ns |     13.782 ns |      6.119 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | Reversed           |       509.32 ns |      5.314 ns |      2.359 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Reversed           |     6,524.46 ns |    245.190 ns |    128.239 ns |  0.28 |    0.01 |    4 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **MergeSort**                | **1024** | **PipeOrgan**          |    **17,727.01 ns** |     **82.797 ns** |     **36.763 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | PipeOrgan          |    17,046.27 ns |    469.864 ns |    245.748 ns |  0.96 |    0.01 |    7 |         - |          NA |
| BottomupMergeSort        | 1024 | PipeOrgan          |     7,532.74 ns |    354.032 ns |    185.166 ns |  0.42 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 1024 | PipeOrgan          |     6,348.78 ns |     98.760 ns |     43.850 ns |  0.36 |    0.00 |    6 |         - |          NA |
| RotateMergeSort          | 1024 | PipeOrgan          |    12,185.14 ns |    181.238 ns |     94.791 ns |  0.69 |    0.01 |    7 |         - |          NA |
| RotateMergeSortRecursive | 1024 | PipeOrgan          |    12,806.30 ns |    239.498 ns |    125.262 ns |  0.72 |    0.01 |    7 |         - |          NA |
| SymMergeSort             | 1024 | PipeOrgan          |     9,358.07 ns |    442.574 ns |    231.475 ns |  0.53 |    0.01 |    6 |         - |          NA |
| BlockMergeSort           | 1024 | PipeOrgan          |     9,051.27 ns |     47.074 ns |     16.787 ns |  0.51 |    0.00 |    6 |         - |          NA |
| NaturalMergeSort         | 1024 | PipeOrgan          |     1,273.90 ns |    193.592 ns |    101.252 ns |  0.07 |    0.01 |    2 |         - |          NA |
| PatienceSort             | 1024 | PipeOrgan          |    15,298.71 ns |  1,000.322 ns |    523.188 ns |  0.86 |    0.03 |    7 |         - |          NA |
| DropMergeSort            | 1024 | PipeOrgan          |    13,587.05 ns |    900.821 ns |    471.147 ns |  0.77 |    0.03 |    7 |         - |          NA |
| TimSort                  | 1024 | PipeOrgan          |     1,661.77 ns |     20.720 ns |      9.200 ns |  0.09 |    0.00 |    3 |         - |          NA |
| PowerSort                | 1024 | PipeOrgan          |       929.74 ns |     37.522 ns |     13.381 ns |  0.05 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | PipeOrgan          |     6,533.26 ns |    247.853 ns |    129.632 ns |  0.37 |    0.01 |    6 |         - |          NA |
| SpinSort                 | 1024 | PipeOrgan          |     4,955.88 ns |    416.966 ns |    185.136 ns |  0.28 |    0.01 |    5 |         - |          NA |
| Glidesort                | 1024 | PipeOrgan          |     3,198.79 ns |    159.669 ns |     83.510 ns |  0.18 |    0.00 |    4 |         - |          NA |
| Driftsort                | 1024 | PipeOrgan          |       779.56 ns |     17.294 ns |      7.679 ns |  0.04 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | PipeOrgan          |     5,161.23 ns |    271.579 ns |    142.041 ns |  0.29 |    0.01 |    5 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **MergeSort**                | **1024** | **ManyDuplicates**     |    **21,798.09 ns** |    **636.707 ns** |    **333.010 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | ManyDuplicates     |    19,114.51 ns |    337.304 ns |    149.765 ns |  0.88 |    0.01 |    4 |         - |          NA |
| BottomupMergeSort        | 1024 | ManyDuplicates     |    15,162.95 ns |    510.851 ns |    267.185 ns |  0.70 |    0.02 |    3 |         - |          NA |
| StdStableSort            | 1024 | ManyDuplicates     |    10,773.67 ns |    337.608 ns |    149.900 ns |  0.49 |    0.01 |    3 |         - |          NA |
| RotateMergeSort          | 1024 | ManyDuplicates     |    30,162.53 ns |    775.016 ns |    344.112 ns |  1.38 |    0.02 |    5 |         - |          NA |
| RotateMergeSortRecursive | 1024 | ManyDuplicates     |    29,760.93 ns |    756.856 ns |    395.850 ns |  1.37 |    0.03 |    5 |         - |          NA |
| SymMergeSort             | 1024 | ManyDuplicates     |    19,735.16 ns |    177.941 ns |     63.456 ns |  0.91 |    0.01 |    4 |         - |          NA |
| BlockMergeSort           | 1024 | ManyDuplicates     |    13,617.73 ns |    611.066 ns |    319.599 ns |  0.62 |    0.02 |    3 |         - |          NA |
| NaturalMergeSort         | 1024 | ManyDuplicates     |    12,577.32 ns |  1,209.714 ns |    632.704 ns |  0.58 |    0.03 |    3 |         - |          NA |
| PatienceSort             | 1024 | ManyDuplicates     |     8,681.39 ns |    676.968 ns |    300.578 ns |  0.40 |    0.01 |    2 |         - |          NA |
| DropMergeSort            | 1024 | ManyDuplicates     |     5,953.66 ns |    740.406 ns |    387.247 ns |  0.27 |    0.02 |    1 |         - |          NA |
| TimSort                  | 1024 | ManyDuplicates     |     8,890.03 ns |    630.439 ns |    329.732 ns |  0.41 |    0.02 |    2 |         - |          NA |
| PowerSort                | 1024 | ManyDuplicates     |     6,660.82 ns |    593.431 ns |    310.376 ns |  0.31 |    0.01 |    1 |         - |          NA |
| ShiftSort                | 1024 | ManyDuplicates     |    10,866.94 ns |    228.349 ns |    119.431 ns |  0.50 |    0.01 |    3 |         - |          NA |
| SpinSort                 | 1024 | ManyDuplicates     |     5,793.42 ns |    508.772 ns |    266.098 ns |  0.27 |    0.01 |    1 |         - |          NA |
| Glidesort                | 1024 | ManyDuplicates     |     6,601.49 ns |    216.947 ns |    113.467 ns |  0.30 |    0.01 |    1 |         - |          NA |
| Driftsort                | 1024 | ManyDuplicates     |     7,038.56 ns |    328.245 ns |    171.679 ns |  0.32 |    0.01 |    1 |         - |          NA |
| FlatStableSort           | 1024 | ManyDuplicates     |     6,910.16 ns |  1,331.377 ns |    696.336 ns |  0.32 |    0.03 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **MergeSort**                | **4096** | **Random**             |   **192,819.98 ns** | **16,598.195 ns** |  **8,681.177 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Random             |   204,959.66 ns | 49,414.652 ns | 21,940.412 ns |  1.06 |    0.12 |    5 |         - |          NA |
| BottomupMergeSort        | 4096 | Random             |   139,473.97 ns |  3,290.908 ns |  1,721.209 ns |  0.72 |    0.03 |    5 |         - |          NA |
| StdStableSort            | 4096 | Random             |    72,257.60 ns |  7,266.762 ns |  3,226.487 ns |  0.38 |    0.02 |    3 |         - |          NA |
| RotateMergeSort          | 4096 | Random             |   440,843.81 ns |  7,080.865 ns |  3,703.429 ns |  2.29 |    0.10 |    7 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Random             |   473,972.06 ns |  7,589.862 ns |  3,969.645 ns |  2.46 |    0.10 |    7 |         - |          NA |
| SymMergeSort             | 4096 | Random             |   323,411.92 ns | 57,405.362 ns | 30,024.114 ns |  1.68 |    0.16 |    6 |         - |          NA |
| BlockMergeSort           | 4096 | Random             |   140,370.42 ns |  7,614.692 ns |  3,380.970 ns |  0.73 |    0.03 |    5 |         - |          NA |
| NaturalMergeSort         | 4096 | Random             |   167,016.37 ns | 10,655.079 ns |  5,572.812 ns |  0.87 |    0.05 |    5 |         - |          NA |
| PatienceSort             | 4096 | Random             |   187,779.23 ns | 13,284.589 ns |  5,898.440 ns |  0.98 |    0.05 |    5 |         - |          NA |
| DropMergeSort            | 4096 | Random             |    96,692.22 ns | 18,327.826 ns |  9,585.807 ns |  0.50 |    0.05 |    4 |         - |          NA |
| TimSort                  | 4096 | Random             |   127,353.34 ns | 12,945.578 ns |  6,770.788 ns |  0.66 |    0.04 |    5 |         - |          NA |
| PowerSort                | 4096 | Random             |    63,609.59 ns |  5,716.693 ns |  2,538.247 ns |  0.33 |    0.02 |    3 |         - |          NA |
| ShiftSort                | 4096 | Random             |   158,416.71 ns | 10,989.864 ns |  5,747.911 ns |  0.82 |    0.04 |    5 |         - |          NA |
| SpinSort                 | 4096 | Random             |    63,095.17 ns |  7,827.016 ns |  4,093.681 ns |  0.33 |    0.02 |    3 |         - |          NA |
| Glidesort                | 4096 | Random             |    31,808.01 ns |    490.261 ns |    217.679 ns |  0.17 |    0.01 |    1 |         - |          NA |
| Driftsort                | 4096 | Random             |    40,610.13 ns |    765.965 ns |    340.093 ns |  0.21 |    0.01 |    2 |         - |          NA |
| FlatStableSort           | 4096 | Random             |    76,372.93 ns |  8,311.011 ns |  4,346.819 ns |  0.40 |    0.03 |    3 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **MergeSort**                | **4096** | **SingleElementMoved** |    **46,549.95 ns** |    **549.925 ns** |    **287.621 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | SingleElementMoved |    52,818.32 ns |  1,036.373 ns |    542.043 ns |  1.13 |    0.01 |    8 |         - |          NA |
| BottomupMergeSort        | 4096 | SingleElementMoved |    14,592.60 ns |  1,027.453 ns |    537.378 ns |  0.31 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 4096 | SingleElementMoved |    10,146.52 ns |    334.017 ns |    148.306 ns |  0.22 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 4096 | SingleElementMoved |     3,208.99 ns |    328.085 ns |    171.595 ns |  0.07 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 4096 | SingleElementMoved |     4,259.07 ns |    243.391 ns |    108.067 ns |  0.09 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 4096 | SingleElementMoved |     2,701.55 ns |     50.375 ns |     17.964 ns |  0.06 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 4096 | SingleElementMoved |    32,592.60 ns |    259.136 ns |    115.058 ns |  0.70 |    0.00 |    7 |         - |          NA |
| NaturalMergeSort         | 4096 | SingleElementMoved |     4,581.35 ns |    493.661 ns |    258.194 ns |  0.10 |    0.01 |    3 |         - |          NA |
| PatienceSort             | 4096 | SingleElementMoved |   224,619.84 ns | 11,549.399 ns |  5,128.005 ns |  4.83 |    0.11 |    9 |         - |          NA |
| DropMergeSort            | 4096 | SingleElementMoved |     4,635.03 ns |    692.030 ns |    361.945 ns |  0.10 |    0.01 |    3 |         - |          NA |
| TimSort                  | 4096 | SingleElementMoved |     1,667.64 ns |     79.942 ns |     41.811 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | SingleElementMoved |     3,234.96 ns |    564.784 ns |    295.393 ns |  0.07 |    0.01 |    2 |         - |          NA |
| ShiftSort                | 4096 | SingleElementMoved |     3,413.88 ns |    144.639 ns |     75.649 ns |  0.07 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 4096 | SingleElementMoved |     7,044.02 ns |    761.242 ns |    271.466 ns |  0.15 |    0.01 |    4 |         - |          NA |
| Glidesort                | 4096 | SingleElementMoved |     7,206.29 ns |    266.421 ns |    139.344 ns |  0.15 |    0.00 |    4 |         - |          NA |
| Driftsort                | 4096 | SingleElementMoved |     2,517.29 ns |     14.477 ns |      5.163 ns |  0.05 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 4096 | SingleElementMoved |    12,734.60 ns |    642.977 ns |    336.289 ns |  0.27 |    0.01 |    6 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **MergeSort**                | **4096** | **Sorted**             |    **42,145.31 ns** |    **209.003 ns** |     **92.799 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Sorted             |    51,307.21 ns |    208.299 ns |    108.944 ns |  1.22 |    0.00 |    8 |         - |          NA |
| BottomupMergeSort        | 4096 | Sorted             |    11,743.69 ns |    438.683 ns |    194.778 ns |  0.28 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 4096 | Sorted             |     8,254.96 ns |    379.744 ns |    168.609 ns |  0.20 |    0.00 |    4 |         - |          NA |
| RotateMergeSort          | 4096 | Sorted             |     2,183.87 ns |     14.801 ns |      7.741 ns |  0.05 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Sorted             |     3,543.52 ns |    358.306 ns |    187.401 ns |  0.08 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 4096 | Sorted             |     2,077.65 ns |     11.643 ns |      4.152 ns |  0.05 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 4096 | Sorted             |    26,788.31 ns |    702.923 ns |    312.102 ns |  0.64 |    0.01 |    6 |         - |          NA |
| NaturalMergeSort         | 4096 | Sorted             |     1,044.07 ns |     18.593 ns |      8.256 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 4096 | Sorted             |   221,079.42 ns | 17,024.814 ns |  8,904.307 ns |  5.25 |    0.20 |    9 |         - |          NA |
| DropMergeSort            | 4096 | Sorted             |     4,024.16 ns |    339.892 ns |    150.914 ns |  0.10 |    0.00 |    3 |         - |          NA |
| TimSort                  | 4096 | Sorted             |     1,000.80 ns |     16.374 ns |      7.270 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | Sorted             |     1,032.05 ns |     53.978 ns |     23.967 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | Sorted             |     2,079.57 ns |     72.641 ns |     25.904 ns |  0.05 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 4096 | Sorted             |     1,008.42 ns |      7.220 ns |      3.206 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Glidesort                | 4096 | Sorted             |     1,140.16 ns |    187.491 ns |     98.062 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | Sorted             |     1,069.32 ns |     44.676 ns |     19.837 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Sorted             |     9,753.85 ns |    372.394 ns |    194.769 ns |  0.23 |    0.00 |    4 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **MergeSort**                | **4096** | **Reversed**           |    **97,233.53 ns** |  **2,304.671 ns** |  **1,205.388 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Reversed           |    86,453.44 ns |  2,296.713 ns |  1,019.755 ns |  0.89 |    0.01 |    7 |         - |          NA |
| BottomupMergeSort        | 4096 | Reversed           |    47,268.65 ns |  1,444.780 ns |    641.491 ns |  0.49 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 4096 | Reversed           |    43,899.33 ns |  1,468.641 ns |    768.128 ns |  0.45 |    0.01 |    6 |         - |          NA |
| RotateMergeSort          | 4096 | Reversed           |    30,782.79 ns |    968.662 ns |    506.629 ns |  0.32 |    0.01 |    5 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Reversed           |    26,391.06 ns |    496.610 ns |    259.737 ns |  0.27 |    0.00 |    5 |         - |          NA |
| SymMergeSort             | 4096 | Reversed           |    25,268.15 ns |    299.250 ns |    132.869 ns |  0.26 |    0.00 |    5 |         - |          NA |
| BlockMergeSort           | 4096 | Reversed           |    39,562.67 ns |    740.656 ns |    387.377 ns |  0.41 |    0.01 |    6 |         - |          NA |
| NaturalMergeSort         | 4096 | Reversed           |     1,845.49 ns |     99.747 ns |     44.288 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 4096 | Reversed           |    11,347.78 ns |    304.199 ns |    135.066 ns |  0.12 |    0.00 |    3 |         - |          NA |
| DropMergeSort            | 4096 | Reversed           |    18,061.53 ns |    916.055 ns |    479.115 ns |  0.19 |    0.01 |    4 |         - |          NA |
| TimSort                  | 4096 | Reversed           |     1,737.96 ns |     18.714 ns |      8.309 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | Reversed           |     1,984.83 ns |     24.804 ns |     11.013 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | Reversed           |    44,808.67 ns |    340.098 ns |    177.878 ns |  0.46 |    0.01 |    6 |         - |          NA |
| SpinSort                 | 4096 | Reversed           |     2,518.98 ns |     41.630 ns |     18.484 ns |  0.03 |    0.00 |    2 |         - |          NA |
| Glidesort                | 4096 | Reversed           |     1,752.21 ns |     24.998 ns |     13.074 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | Reversed           |     1,744.13 ns |     22.938 ns |     10.184 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Reversed           |    25,995.46 ns |    408.269 ns |    213.532 ns |  0.27 |    0.00 |    5 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **MergeSort**                | **4096** | **PipeOrgan**          |    **73,558.35 ns** |  **2,876.957 ns** |  **1,504.704 ns** |  **1.00** |    **0.00** |    **9** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | PipeOrgan          |    69,823.62 ns |  2,091.221 ns |  1,093.749 ns |  0.95 |    0.02 |    9 |         - |          NA |
| BottomupMergeSort        | 4096 | PipeOrgan          |    30,301.13 ns |  1,376.904 ns |    611.354 ns |  0.41 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 4096 | PipeOrgan          |    26,922.65 ns |    848.234 ns |    302.488 ns |  0.37 |    0.01 |    6 |         - |          NA |
| RotateMergeSort          | 4096 | PipeOrgan          |    52,202.43 ns |    502.880 ns |    263.016 ns |  0.71 |    0.01 |    8 |         - |          NA |
| RotateMergeSortRecursive | 4096 | PipeOrgan          |    55,296.48 ns |    538.547 ns |    239.118 ns |  0.75 |    0.01 |    8 |         - |          NA |
| SymMergeSort             | 4096 | PipeOrgan          |    38,587.03 ns |    538.862 ns |    281.835 ns |  0.52 |    0.01 |    7 |         - |          NA |
| BlockMergeSort           | 4096 | PipeOrgan          |    37,056.55 ns |    490.732 ns |    217.888 ns |  0.50 |    0.01 |    7 |         - |          NA |
| NaturalMergeSort         | 4096 | PipeOrgan          |     4,893.99 ns |    580.323 ns |    303.520 ns |  0.07 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 4096 | PipeOrgan          |   129,299.97 ns |  8,896.995 ns |  4,653.300 ns |  1.76 |    0.07 |   10 |         - |          NA |
| DropMergeSort            | 4096 | PipeOrgan          |    56,950.72 ns |  2,091.350 ns |    928.573 ns |  0.77 |    0.02 |    8 |         - |          NA |
| TimSort                  | 4096 | PipeOrgan          |     6,461.74 ns |    203.097 ns |     90.177 ns |  0.09 |    0.00 |    3 |         - |          NA |
| PowerSort                | 4096 | PipeOrgan          |     3,868.83 ns |    615.234 ns |    321.779 ns |  0.05 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 4096 | PipeOrgan          |    25,925.96 ns |    176.385 ns |     78.316 ns |  0.35 |    0.01 |    6 |         - |          NA |
| SpinSort                 | 4096 | PipeOrgan          |     4,134.66 ns |    194.619 ns |     86.412 ns |  0.06 |    0.00 |    2 |         - |          NA |
| Glidesort                | 4096 | PipeOrgan          |    12,121.74 ns |    291.678 ns |    129.507 ns |  0.16 |    0.00 |    4 |         - |          NA |
| Driftsort                | 4096 | PipeOrgan          |     2,969.17 ns |    599.809 ns |    313.712 ns |  0.04 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | PipeOrgan          |    20,362.43 ns |    349.771 ns |    182.937 ns |  0.28 |    0.01 |    5 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **MergeSort**                | **4096** | **ManyDuplicates**     |   **110,218.81 ns** |  **9,749.034 ns** |  **5,098.933 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | ManyDuplicates     |    99,776.35 ns |  5,809.081 ns |  3,038.262 ns |  0.91 |    0.05 |    3 |         - |          NA |
| BottomupMergeSort        | 4096 | ManyDuplicates     |    89,967.81 ns |  6,902.651 ns |  3,610.220 ns |  0.82 |    0.05 |    3 |         - |          NA |
| StdStableSort            | 4096 | ManyDuplicates     |    53,148.53 ns |  2,113.414 ns |  1,105.357 ns |  0.48 |    0.02 |    3 |         - |          NA |
| RotateMergeSort          | 4096 | ManyDuplicates     |   270,951.51 ns | 11,651.067 ns |  6,093.734 ns |  2.46 |    0.12 |    5 |         - |          NA |
| RotateMergeSortRecursive | 4096 | ManyDuplicates     |   299,004.89 ns | 13,205.690 ns |  5,863.408 ns |  2.72 |    0.13 |    5 |         - |          NA |
| SymMergeSort             | 4096 | ManyDuplicates     |   198,017.02 ns |  7,262.253 ns |  3,798.299 ns |  1.80 |    0.08 |    4 |         - |          NA |
| BlockMergeSort           | 4096 | ManyDuplicates     |    75,708.65 ns |  1,580.792 ns |    701.882 ns |  0.69 |    0.03 |    3 |         - |          NA |
| NaturalMergeSort         | 4096 | ManyDuplicates     |    99,824.16 ns |  9,745.744 ns |  4,327.171 ns |  0.91 |    0.05 |    3 |         - |          NA |
| PatienceSort             | 4096 | ManyDuplicates     |    33,990.55 ns |  3,427.098 ns |  1,792.438 ns |  0.31 |    0.02 |    2 |         - |          NA |
| DropMergeSort            | 4096 | ManyDuplicates     |    27,135.38 ns |  1,816.723 ns |    950.181 ns |  0.25 |    0.01 |    2 |         - |          NA |
| TimSort                  | 4096 | ManyDuplicates     |    67,205.01 ns | 12,912.025 ns |  6,753.239 ns |  0.61 |    0.06 |    3 |         - |          NA |
| PowerSort                | 4096 | ManyDuplicates     |    34,907.60 ns |    603.917 ns |    268.143 ns |  0.32 |    0.01 |    2 |         - |          NA |
| ShiftSort                | 4096 | ManyDuplicates     |    58,412.08 ns |  5,850.427 ns |  3,059.886 ns |  0.53 |    0.03 |    3 |         - |          NA |
| SpinSort                 | 4096 | ManyDuplicates     |    28,718.92 ns |  1,892.914 ns |    990.031 ns |  0.26 |    0.01 |    2 |         - |          NA |
| Glidesort                | 4096 | ManyDuplicates     |    10,997.56 ns |    391.958 ns |    174.032 ns |  0.10 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | ManyDuplicates     |     9,348.97 ns |    272.840 ns |    142.701 ns |  0.08 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | ManyDuplicates     |    32,678.97 ns |  1,458.432 ns |    647.553 ns |  0.30 |    0.01 |    2 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **MergeSort**                | **8192** | **Random**             |   **526,837.23 ns** |  **7,356.518 ns** |  **3,847.601 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Random             |   470,289.16 ns |  8,230.157 ns |  4,304.531 ns |  0.89 |    0.01 |    3 |         - |          NA |
| BottomupMergeSort        | 8192 | Random             |   367,860.15 ns |  4,172.820 ns |  2,182.466 ns |  0.70 |    0.01 |    3 |         - |          NA |
| StdStableSort            | 8192 | Random             |   267,215.27 ns |  9,088.803 ns |  4,035.485 ns |  0.51 |    0.01 |    3 |         - |          NA |
| RotateMergeSort          | 8192 | Random             | 1,063,799.44 ns | 32,147.728 ns | 16,813.884 ns |  2.02 |    0.03 |    5 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Random             | 1,131,076.88 ns | 84,292.816 ns | 44,086.773 ns |  2.15 |    0.08 |    5 |         - |          NA |
| SymMergeSort             | 8192 | Random             |   776,503.51 ns | 97,905.609 ns | 51,206.527 ns |  1.47 |    0.09 |    4 |         - |          NA |
| BlockMergeSort           | 8192 | Random             |   441,989.15 ns | 14,646.448 ns |  7,660.376 ns |  0.84 |    0.01 |    3 |         - |          NA |
| NaturalMergeSort         | 8192 | Random             |   483,907.28 ns | 46,090.597 ns | 20,464.511 ns |  0.92 |    0.04 |    3 |         - |          NA |
| PatienceSort             | 8192 | Random             |   527,788.43 ns | 12,577.379 ns |  6,578.212 ns |  1.00 |    0.01 |    3 |         - |          NA |
| DropMergeSort            | 8192 | Random             |   352,164.50 ns |  4,755.015 ns |  2,486.965 ns |  0.67 |    0.01 |    3 |         - |          NA |
| TimSort                  | 8192 | Random             |   428,963.98 ns | 16,594.895 ns |  7,368.236 ns |  0.81 |    0.01 |    3 |         - |          NA |
| PowerSort                | 8192 | Random             |   328,699.99 ns | 68,859.132 ns | 36,014.657 ns |  0.62 |    0.06 |    3 |         - |          NA |
| ShiftSort                | 8192 | Random             |   471,598.43 ns |  7,613.168 ns |  2,714.929 ns |  0.90 |    0.01 |    3 |         - |          NA |
| SpinSort                 | 8192 | Random             |   275,617.31 ns |  4,966.372 ns |  2,205.100 ns |  0.52 |    0.01 |    3 |         - |          NA |
| Glidesort                | 8192 | Random             |    64,710.77 ns |  2,980.449 ns |  1,323.338 ns |  0.12 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Random             |    82,102.56 ns |  1,320.731 ns |    586.413 ns |  0.16 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 8192 | Random             |   293,884.71 ns | 11,575.170 ns |  5,139.447 ns |  0.56 |    0.01 |    3 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **MergeSort**                | **8192** | **SingleElementMoved** |    **92,621.32 ns** |  **1,672.036 ns** |    **596.264 ns** |  **1.00** |    **0.00** |   **10** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | SingleElementMoved |   109,906.41 ns |  3,237.366 ns |  1,693.205 ns |  1.19 |    0.02 |   10 |         - |          NA |
| BottomupMergeSort        | 8192 | SingleElementMoved |    28,699.02 ns |    733.051 ns |    383.400 ns |  0.31 |    0.00 |    8 |         - |          NA |
| StdStableSort            | 8192 | SingleElementMoved |    17,814.36 ns |    989.682 ns |    517.623 ns |  0.19 |    0.01 |    6 |         - |          NA |
| RotateMergeSort          | 8192 | SingleElementMoved |     5,914.19 ns |    331.507 ns |    173.385 ns |  0.06 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 8192 | SingleElementMoved |     8,435.37 ns |    617.734 ns |    323.087 ns |  0.09 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 8192 | SingleElementMoved |     5,329.19 ns |    314.778 ns |    139.763 ns |  0.06 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 8192 | SingleElementMoved |    64,023.65 ns |    421.204 ns |    220.298 ns |  0.69 |    0.00 |    9 |         - |          NA |
| NaturalMergeSort         | 8192 | SingleElementMoved |     8,444.26 ns |    211.996 ns |     75.600 ns |  0.09 |    0.00 |    3 |         - |          NA |
| PatienceSort             | 8192 | SingleElementMoved |   611,529.68 ns | 26,224.149 ns | 13,715.737 ns |  6.60 |    0.15 |   11 |         - |          NA |
| DropMergeSort            | 8192 | SingleElementMoved |    10,475.78 ns |  2,818.714 ns |  1,474.242 ns |  0.11 |    0.02 |    4 |         - |          NA |
| TimSort                  | 8192 | SingleElementMoved |     3,189.88 ns |    133.769 ns |     69.964 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | SingleElementMoved |     5,668.47 ns |    217.705 ns |     77.636 ns |  0.06 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 8192 | SingleElementMoved |     6,655.31 ns |    251.076 ns |    111.479 ns |  0.07 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | SingleElementMoved |    11,883.65 ns |  1,651.877 ns |    863.964 ns |  0.13 |    0.01 |    4 |         - |          NA |
| Glidesort                | 8192 | SingleElementMoved |    14,392.47 ns |    186.046 ns |     97.306 ns |  0.16 |    0.00 |    5 |         - |          NA |
| Driftsort                | 8192 | SingleElementMoved |     5,002.85 ns |    258.351 ns |    135.123 ns |  0.05 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 8192 | SingleElementMoved |    22,958.61 ns |    669.935 ns |    350.389 ns |  0.25 |    0.00 |    7 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **MergeSort**                | **8192** | **Sorted**             |    **84,119.35 ns** |    **676.719 ns** |    **300.467 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Sorted             |   101,283.70 ns |  2,574.514 ns |  1,143.100 ns |  1.20 |    0.01 |    8 |         - |          NA |
| BottomupMergeSort        | 8192 | Sorted             |    23,796.94 ns |    667.764 ns |    296.491 ns |  0.28 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 8192 | Sorted             |    15,585.72 ns |    333.293 ns |    174.319 ns |  0.19 |    0.00 |    4 |         - |          NA |
| RotateMergeSort          | 8192 | Sorted             |     4,253.20 ns |      9.649 ns |      5.047 ns |  0.05 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Sorted             |     6,915.07 ns |    672.621 ns |    351.794 ns |  0.08 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 8192 | Sorted             |     4,240.76 ns |    285.219 ns |    149.175 ns |  0.05 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 8192 | Sorted             |    51,021.21 ns |    682.007 ns |    356.703 ns |  0.61 |    0.00 |    6 |         - |          NA |
| NaturalMergeSort         | 8192 | Sorted             |     2,085.00 ns |     23.856 ns |      8.507 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 8192 | Sorted             |   599,948.69 ns | 23,673.668 ns | 12,381.786 ns |  7.13 |    0.14 |    9 |         - |          NA |
| DropMergeSort            | 8192 | Sorted             |     7,776.92 ns |    347.464 ns |    154.276 ns |  0.09 |    0.00 |    3 |         - |          NA |
| TimSort                  | 8192 | Sorted             |     2,051.44 ns |     16.125 ns |      8.433 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Sorted             |     2,016.23 ns |     17.980 ns |      7.983 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Sorted             |     3,910.12 ns |     25.404 ns |     13.287 ns |  0.05 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | Sorted             |     1,979.11 ns |     20.674 ns |      7.373 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Sorted             |     2,057.22 ns |     34.799 ns |     12.409 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Sorted             |     2,059.67 ns |     14.670 ns |      6.514 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Sorted             |     2,019.07 ns |     16.948 ns |      7.525 ns |  0.02 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **MergeSort**                | **8192** | **Reversed**           |   **198,280.40 ns** |  **2,986.373 ns** |  **1,561.931 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Reversed           |   173,763.87 ns |  6,495.925 ns |  3,397.495 ns |  0.88 |    0.02 |    7 |         - |          NA |
| BottomupMergeSort        | 8192 | Reversed           |    95,805.05 ns |  4,901.346 ns |  2,563.499 ns |  0.48 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 8192 | Reversed           |   177,184.34 ns |  2,631.374 ns |  1,376.259 ns |  0.89 |    0.01 |    7 |         - |          NA |
| RotateMergeSort          | 8192 | Reversed           |    58,999.79 ns |    396.076 ns |    175.860 ns |  0.30 |    0.00 |    5 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Reversed           |    57,065.53 ns |  1,734.772 ns |    770.249 ns |  0.29 |    0.00 |    5 |         - |          NA |
| SymMergeSort             | 8192 | Reversed           |    53,121.14 ns |  5,433.121 ns |  2,841.627 ns |  0.27 |    0.01 |    5 |         - |          NA |
| BlockMergeSort           | 8192 | Reversed           |    82,209.61 ns |  1,233.319 ns |    547.601 ns |  0.41 |    0.00 |    6 |         - |          NA |
| NaturalMergeSort         | 8192 | Reversed           |     3,607.52 ns |    122.170 ns |     63.897 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 8192 | Reversed           |    22,148.23 ns |    692.849 ns |    307.629 ns |  0.11 |    0.00 |    3 |         - |          NA |
| DropMergeSort            | 8192 | Reversed           |    36,833.25 ns |  1,556.648 ns |    814.157 ns |  0.19 |    0.00 |    4 |         - |          NA |
| TimSort                  | 8192 | Reversed           |     3,423.16 ns |     13.125 ns |      5.827 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Reversed           |     4,000.20 ns |    269.929 ns |    141.178 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Reversed           |    89,524.55 ns |  3,959.352 ns |  2,070.817 ns |  0.45 |    0.01 |    6 |         - |          NA |
| SpinSort                 | 8192 | Reversed           |     4,941.45 ns |    146.999 ns |     76.883 ns |  0.02 |    0.00 |    2 |         - |          NA |
| Glidesort                | 8192 | Reversed           |     3,589.62 ns |    345.763 ns |    180.841 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Reversed           |     3,556.17 ns |     20.680 ns |      9.182 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Reversed           |     3,997.25 ns |    258.501 ns |    135.201 ns |  0.02 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **MergeSort**                | **8192** | **PipeOrgan**          |   **144,370.86 ns** |  **2,861.418 ns** |  **1,270.487 ns** |  **1.00** |    **0.00** |    **9** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | PipeOrgan          |   137,814.66 ns |  3,318.126 ns |  1,735.444 ns |  0.95 |    0.01 |    9 |         - |          NA |
| BottomupMergeSort        | 8192 | PipeOrgan          |    63,465.67 ns |  2,271.032 ns |  1,187.794 ns |  0.44 |    0.01 |    7 |         - |          NA |
| StdStableSort            | 8192 | PipeOrgan          |    96,856.51 ns |  1,417.444 ns |    629.354 ns |  0.67 |    0.01 |    9 |         - |          NA |
| RotateMergeSort          | 8192 | PipeOrgan          |   105,675.51 ns |  2,062.447 ns |  1,078.700 ns |  0.73 |    0.01 |    9 |         - |          NA |
| RotateMergeSortRecursive | 8192 | PipeOrgan          |   112,146.10 ns |  1,037.313 ns |    369.916 ns |  0.78 |    0.01 |    9 |         - |          NA |
| SymMergeSort             | 8192 | PipeOrgan          |    78,787.75 ns |    714.144 ns |    317.084 ns |  0.55 |    0.00 |    8 |         - |          NA |
| BlockMergeSort           | 8192 | PipeOrgan          |    76,525.21 ns |    295.345 ns |    154.471 ns |  0.53 |    0.00 |    8 |         - |          NA |
| NaturalMergeSort         | 8192 | PipeOrgan          |    10,245.64 ns |    510.525 ns |    267.014 ns |  0.07 |    0.00 |    3 |         - |          NA |
| PatienceSort             | 8192 | PipeOrgan          |   434,079.96 ns |  8,397.924 ns |  4,392.277 ns |  3.01 |    0.04 |   10 |         - |          NA |
| DropMergeSort            | 8192 | PipeOrgan          |   114,987.66 ns |  2,985.916 ns |  1,561.692 ns |  0.80 |    0.01 |    9 |         - |          NA |
| TimSort                  | 8192 | PipeOrgan          |    12,584.20 ns |    309.353 ns |    137.355 ns |  0.09 |    0.00 |    4 |         - |          NA |
| PowerSort                | 8192 | PipeOrgan          |     7,109.27 ns |    587.147 ns |    307.089 ns |  0.05 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 8192 | PipeOrgan          |    52,974.63 ns |    675.476 ns |    353.287 ns |  0.37 |    0.00 |    7 |         - |          NA |
| SpinSort                 | 8192 | PipeOrgan          |     8,266.54 ns |    737.442 ns |    385.697 ns |  0.06 |    0.00 |    2 |         - |          NA |
| Glidesort                | 8192 | PipeOrgan          |    23,213.51 ns |    315.158 ns |    164.834 ns |  0.16 |    0.00 |    5 |         - |          NA |
| Driftsort                | 8192 | PipeOrgan          |     5,454.50 ns |    452.114 ns |    236.464 ns |  0.04 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | PipeOrgan          |    37,103.35 ns |    133.425 ns |     59.242 ns |  0.26 |    0.00 |    6 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **MergeSort**                | **8192** | **ManyDuplicates**     |   **375,291.62 ns** | **42,036.929 ns** | **18,664.657 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | ManyDuplicates     |   329,134.14 ns |  9,393.598 ns |  4,913.033 ns |  0.88 |    0.04 |    4 |         - |          NA |
| BottomupMergeSort        | 8192 | ManyDuplicates     |   234,167.18 ns |  2,814.826 ns |  1,249.800 ns |  0.63 |    0.03 |    4 |         - |          NA |
| StdStableSort            | 8192 | ManyDuplicates     |   159,403.13 ns |  8,227.940 ns |  4,303.372 ns |  0.43 |    0.02 |    3 |         - |          NA |
| RotateMergeSort          | 8192 | ManyDuplicates     |   658,701.39 ns | 25,634.761 ns | 11,381.993 ns |  1.76 |    0.09 |    6 |         - |          NA |
| RotateMergeSortRecursive | 8192 | ManyDuplicates     |   704,440.57 ns | 16,074.398 ns |  7,137.132 ns |  1.88 |    0.09 |    6 |         - |          NA |
| SymMergeSort             | 8192 | ManyDuplicates     |   504,318.62 ns |  7,909.067 ns |  4,136.595 ns |  1.35 |    0.06 |    5 |         - |          NA |
| BlockMergeSort           | 8192 | ManyDuplicates     |   349,626.91 ns |  3,023.723 ns |  1,581.466 ns |  0.93 |    0.04 |    4 |         - |          NA |
| NaturalMergeSort         | 8192 | ManyDuplicates     |   325,781.42 ns | 11,403.892 ns |  5,964.456 ns |  0.87 |    0.04 |    4 |         - |          NA |
| PatienceSort             | 8192 | ManyDuplicates     |   175,920.12 ns | 30,535.372 ns | 13,557.895 ns |  0.47 |    0.04 |    3 |         - |          NA |
| DropMergeSort            | 8192 | ManyDuplicates     |   131,412.91 ns |  9,436.322 ns |  4,189.785 ns |  0.35 |    0.02 |    3 |         - |          NA |
| TimSort                  | 8192 | ManyDuplicates     |   283,497.17 ns | 27,274.588 ns | 12,110.086 ns |  0.76 |    0.05 |    4 |         - |          NA |
| PowerSort                | 8192 | ManyDuplicates     |   140,110.86 ns |  7,770.706 ns |  3,450.242 ns |  0.37 |    0.02 |    3 |         - |          NA |
| ShiftSort                | 8192 | ManyDuplicates     |   287,057.04 ns |  8,783.803 ns |  4,594.099 ns |  0.77 |    0.04 |    4 |         - |          NA |
| SpinSort                 | 8192 | ManyDuplicates     |   133,582.59 ns |  9,857.927 ns |  5,155.886 ns |  0.36 |    0.02 |    3 |         - |          NA |
| Glidesort                | 8192 | ManyDuplicates     |    19,541.06 ns |    297.303 ns |    155.495 ns |  0.05 |    0.00 |    2 |         - |          NA |
| Driftsort                | 8192 | ManyDuplicates     |    15,208.02 ns |    376.845 ns |    197.097 ns |  0.04 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | ManyDuplicates     |   140,135.72 ns |  5,354.013 ns |  2,800.252 ns |  0.37 |    0.02 |    3 |         - |          NA |

### MergeSlowBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V45 4.27GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.48 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method     | Size | Pattern            | Mean         | Error        | StdDev      | Ratio | Rank | Allocated | Alloc Ratio |
| ----------- |----- |------------------- |-------------:|-------------:|------------:|------:|-----:|----------:|------------:|
| **StrandSort** | **256**  | **Random**             |   **3,196.7 ns** |    **327.38 ns** |   **171.23 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |      |           |             |
| **StrandSort** | **256**  | **SingleElementMoved** |     **661.0 ns** |    **278.97 ns** |   **145.91 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |      |           |             |
| **StrandSort** | **256**  | **Sorted**             |     **396.2 ns** |     **23.28 ns** |    **10.34 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |      |           |             |
| **StrandSort** | **256**  | **Reversed**           |  **29,395.2 ns** |    **849.64 ns** |   **444.38 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |      |           |             |
| **StrandSort** | **256**  | **PipeOrgan**          |  **16,767.2 ns** |  **1,001.87 ns** |   **524.00 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |      |           |             |
| **StrandSort** | **256**  | **ManyDuplicates**     |   **2,678.5 ns** |    **250.52 ns** |   **131.02 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |      |           |             |
| **StrandSort** | **1024** | **Random**             |  **29,046.1 ns** |    **686.12 ns** |   **304.64 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |      |           |             |
| **StrandSort** | **1024** | **SingleElementMoved** |   **1,530.1 ns** |    **170.23 ns** |    **89.04 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |      |           |             |
| **StrandSort** | **1024** | **Sorted**             |     **903.0 ns** |     **34.92 ns** |    **15.50 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |      |           |             |
| **StrandSort** | **1024** | **Reversed**           | **407,288.0 ns** | **14,237.21 ns** | **6,321.41 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |      |           |             |
| **StrandSort** | **1024** | **PipeOrgan**          | **220,235.8 ns** |  **4,362.00 ns** | **2,281.41 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |              |             |       |      |           |             |
| **StrandSort** | **1024** | **ManyDuplicates**     |  **16,518.6 ns** |    **322.04 ns** |   **168.43 ns** |  **1.00** |    **1** |         **-** |          **NA** |

### NetworkBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V45 4.27GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.48 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                  | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------------ |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **BitonicSort**             | **256**  | **Random**             |     **4,531.5 ns** |    **500.96 ns** |    **222.43 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Random             |    13,037.8 ns |    237.95 ns |    105.65 ns |  2.88 |    0.13 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Random             |     2,854.4 ns |    383.55 ns |    200.61 ns |  0.63 |    0.05 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 256  | Random             |     1,759.1 ns |    239.80 ns |    125.42 ns |  0.39 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **256**  | **SingleElementMoved** |     **4,790.5 ns** |    **699.37 ns** |    **365.78 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | SingleElementMoved |    13,390.6 ns |    315.26 ns |    139.98 ns |  2.81 |    0.20 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | SingleElementMoved |     3,006.6 ns |     57.60 ns |     20.54 ns |  0.63 |    0.04 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 256  | SingleElementMoved |     1,565.7 ns |     47.12 ns |     16.81 ns |  0.33 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **256**  | **Sorted**             |     **4,614.3 ns** |    **346.79 ns** |    **181.38 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Sorted             |    13,576.2 ns |     78.44 ns |     34.83 ns |  2.95 |    0.11 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Sorted             |     3,075.1 ns |     41.65 ns |     18.49 ns |  0.67 |    0.02 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 256  | Sorted             |     1,678.0 ns |     64.31 ns |     22.93 ns |  0.36 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **256**  | **Reversed**           |     **4,944.8 ns** |    **710.24 ns** |    **371.47 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Reversed           |    13,836.9 ns |    144.73 ns |     64.26 ns |  2.81 |    0.20 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Reversed           |     2,890.7 ns |    318.04 ns |    166.34 ns |  0.59 |    0.05 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 256  | Reversed           |     1,941.9 ns |     53.78 ns |     19.18 ns |  0.39 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **256**  | **PipeOrgan**          |     **4,593.6 ns** |    **314.40 ns** |    **164.43 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | PipeOrgan          |    13,761.0 ns |    195.86 ns |    102.44 ns |  3.00 |    0.10 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | PipeOrgan          |     3,044.5 ns |    327.33 ns |    171.20 ns |  0.66 |    0.04 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 256  | PipeOrgan          |     1,960.1 ns |    258.56 ns |    135.23 ns |  0.43 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **256**  | **ManyDuplicates**     |     **4,489.8 ns** |    **652.22 ns** |    **341.13 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | ManyDuplicates     |    12,942.5 ns |    306.20 ns |    135.95 ns |  2.90 |    0.21 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | ManyDuplicates     |     2,637.3 ns |     83.36 ns |     37.01 ns |  0.59 |    0.04 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 256  | ManyDuplicates     |     1,678.2 ns |    105.76 ns |     46.96 ns |  0.38 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Random**             |    **40,797.2 ns** |  **2,791.44 ns** |  **1,459.98 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Random             |    71,508.2 ns |  1,169.05 ns |    611.44 ns |  1.75 |    0.06 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Random             |    16,868.8 ns |    523.03 ns |    232.23 ns |  0.41 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 1024 | Random             |    12,569.7 ns |  1,602.22 ns |    837.99 ns |  0.31 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **1024** | **SingleElementMoved** |    **28,622.4 ns** |    **354.60 ns** |    **185.46 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | SingleElementMoved |    72,810.3 ns |    390.05 ns |    204.00 ns |  2.54 |    0.02 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | SingleElementMoved |    17,779.9 ns |    987.42 ns |    516.44 ns |  0.62 |    0.02 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 1024 | SingleElementMoved |     9,375.1 ns |  1,905.44 ns |    996.58 ns |  0.33 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Sorted**             |    **30,184.6 ns** |  **3,046.09 ns** |  **1,593.16 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Sorted             |    72,433.9 ns |  1,028.39 ns |    456.61 ns |  2.41 |    0.12 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Sorted             |    17,498.5 ns |    202.28 ns |     89.82 ns |  0.58 |    0.03 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 1024 | Sorted             |     8,299.4 ns |    337.73 ns |    149.95 ns |  0.28 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Reversed**           |    **30,434.6 ns** |  **2,355.46 ns** |  **1,045.84 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Reversed           |    71,585.2 ns |    532.62 ns |    278.57 ns |  2.35 |    0.08 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Reversed           |    16,158.5 ns |    158.78 ns |     70.50 ns |  0.53 |    0.02 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 1024 | Reversed           |    11,162.4 ns |  2,309.42 ns |  1,025.40 ns |  0.37 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **1024** | **PipeOrgan**          |    **29,021.1 ns** |  **1,178.92 ns** |    **523.45 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | PipeOrgan          |    72,175.5 ns |    601.42 ns |    314.55 ns |  2.49 |    0.04 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | PipeOrgan          |    18,222.2 ns |    256.88 ns |    114.06 ns |  0.63 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 1024 | PipeOrgan          |    10,624.9 ns |  1,990.89 ns |  1,041.28 ns |  0.37 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **1024** | **ManyDuplicates**     |    **31,585.5 ns** |  **2,959.91 ns** |  **1,548.09 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | ManyDuplicates     |    69,082.3 ns |  1,520.66 ns |    675.18 ns |  2.19 |    0.10 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | ManyDuplicates     |    16,218.2 ns |    668.73 ns |    296.92 ns |  0.51 |    0.03 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 1024 | ManyDuplicates     |    11,833.3 ns |  1,773.75 ns |    927.70 ns |  0.38 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Random**             |   **324,204.1 ns** |  **8,083.39 ns** |  **3,589.08 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Random             |   539,000.0 ns | 26,814.40 ns | 14,024.45 ns |  1.66 |    0.04 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Random             |   250,775.6 ns | 63,710.27 ns | 33,321.70 ns |  0.77 |    0.10 |    1 |         - |          NA |
| PairwiseSortingNetwork  | 4096 | Random             |   265,335.0 ns | 12,410.92 ns |  5,510.52 ns |  0.82 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **4096** | **SingleElementMoved** |   **158,780.3 ns** |  **6,647.90 ns** |  **3,476.98 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | SingleElementMoved |   346,695.1 ns |  3,339.15 ns |  1,746.44 ns |  2.18 |    0.05 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | SingleElementMoved |    99,640.5 ns |  2,057.62 ns |  1,076.18 ns |  0.63 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 4096 | SingleElementMoved |    48,903.1 ns |  5,605.72 ns |  2,931.90 ns |  0.31 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Sorted**             |   **165,790.6 ns** |  **5,541.22 ns** |  **2,898.17 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Sorted             |   364,590.3 ns |  5,088.00 ns |  2,661.12 ns |  2.20 |    0.04 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Sorted             |   100,390.5 ns |  1,385.43 ns |    724.61 ns |  0.61 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 4096 | Sorted             |    50,950.8 ns |  5,434.12 ns |  2,412.78 ns |  0.31 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Reversed**           |   **150,719.5 ns** | **14,837.59 ns** |  **7,760.35 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Reversed           |   360,388.5 ns |  4,898.95 ns |  2,562.24 ns |  2.40 |    0.11 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Reversed           |    90,065.2 ns |  1,874.08 ns |    980.18 ns |  0.60 |    0.03 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 4096 | Reversed           |    60,005.1 ns |  6,393.32 ns |  3,343.83 ns |  0.40 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **4096** | **PipeOrgan**          |   **158,331.6 ns** | **11,077.55 ns** |  **4,918.50 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | PipeOrgan          |   345,398.5 ns |  3,268.65 ns |  1,451.30 ns |  2.18 |    0.07 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | PipeOrgan          |    98,834.1 ns |  2,080.56 ns |  1,088.17 ns |  0.62 |    0.02 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 4096 | PipeOrgan          |    49,883.8 ns |  2,821.12 ns |  1,475.50 ns |  0.32 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **4096** | **ManyDuplicates**     |   **265,407.9 ns** | **23,221.16 ns** | **12,145.12 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | ManyDuplicates     |   430,335.7 ns | 14,386.32 ns |  7,524.32 ns |  1.62 |    0.07 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | ManyDuplicates     |   142,673.7 ns | 12,811.96 ns |  5,688.59 ns |  0.54 |    0.03 |    1 |         - |          NA |
| PairwiseSortingNetwork  | 4096 | ManyDuplicates     |   155,099.6 ns |  7,546.64 ns |  3,947.04 ns |  0.59 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **8192** | **Random**             |   **822,844.3 ns** | **47,883.65 ns** | **25,044.08 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | Random             | 1,300,442.2 ns | 53,063.47 ns | 27,753.22 ns |  1.58 |    0.06 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | Random             |   596,081.0 ns | 15,047.36 ns |  6,681.12 ns |  0.73 |    0.02 |    1 |         - |          NA |
| PairwiseSortingNetwork  | 8192 | Random             |   671,160.3 ns | 11,375.90 ns |  5,050.97 ns |  0.82 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **8192** | **SingleElementMoved** |   **358,075.8 ns** | **28,656.11 ns** | **12,723.49 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | SingleElementMoved |   816,970.1 ns | 22,874.96 ns | 10,156.62 ns |  2.28 |    0.08 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | SingleElementMoved |   225,902.7 ns |  6,229.57 ns |  2,765.97 ns |  0.63 |    0.02 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 8192 | SingleElementMoved |   118,386.9 ns |  7,167.01 ns |  3,748.48 ns |  0.33 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **8192** | **Sorted**             |   **356,982.8 ns** | **18,378.07 ns** |  **9,612.09 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | Sorted             |   826,944.6 ns | 10,049.36 ns |  5,256.01 ns |  2.32 |    0.06 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | Sorted             |   227,336.5 ns |  4,740.49 ns |  2,479.37 ns |  0.64 |    0.02 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 8192 | Sorted             |   109,498.7 ns |  7,013.73 ns |  3,668.32 ns |  0.31 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **8192** | **Reversed**           |   **377,000.7 ns** | **22,452.73 ns** | **11,743.21 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | Reversed           |   854,755.8 ns | 88,319.56 ns | 39,214.43 ns |  2.27 |    0.12 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | Reversed           |   208,386.1 ns |  4,005.62 ns |  1,778.52 ns |  0.55 |    0.02 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 8192 | Reversed           |   133,438.6 ns |  6,151.37 ns |  3,217.28 ns |  0.35 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **8192** | **PipeOrgan**          |   **386,155.3 ns** | **13,753.21 ns** |  **6,106.51 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | PipeOrgan          |   828,643.7 ns | 24,451.17 ns | 12,788.44 ns |  2.15 |    0.04 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | PipeOrgan          |   232,073.8 ns |  3,233.79 ns |  1,435.82 ns |  0.60 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 8192 | PipeOrgan          |   124,298.3 ns |  5,220.81 ns |  2,730.59 ns |  0.32 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**             | **8192** | **ManyDuplicates**     |   **598,880.1 ns** | **26,529.33 ns** | **13,875.35 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | ManyDuplicates     | 1,109,569.5 ns | 15,116.73 ns |  6,711.92 ns |  1.85 |    0.04 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | ManyDuplicates     |   437,552.6 ns | 13,497.76 ns |  7,059.59 ns |  0.73 |    0.02 |    1 |         - |          NA |
| PairwiseSortingNetwork  | 8192 | ManyDuplicates     |   437,337.0 ns | 13,401.25 ns |  7,009.11 ns |  0.73 |    0.02 |    1 |         - |          NA |

### PartitionBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V45 4.27GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.48 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                       | Size | Pattern            | Mean            | Error         | StdDev        | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------------- |----- |------------------- |----------------:|--------------:|--------------:|------:|--------:|-----:|----------:|------------:|
| **QuickSort**                    | **256**  | **Random**             |     **1,071.38 ns** |    **178.298 ns** |     **93.253 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 256  | Random             |     1,720.56 ns |    230.357 ns |    102.280 ns |  1.62 |    0.16 |    2 |         - |          NA |
| QuickSortMedian3             | 256  | Random             |     1,626.79 ns |    236.765 ns |    123.833 ns |  1.53 |    0.16 |    2 |         - |          NA |
| QuickSortMedian9             | 256  | Random             |     1,213.36 ns |     89.873 ns |     47.005 ns |  1.14 |    0.10 |    1 |         - |          NA |
| SampleSort                   | 256  | Random             |     2,798.87 ns |    107.522 ns |     47.740 ns |  2.63 |    0.21 |    3 |         - |          NA |
| DualPivotQuickSort           | 256  | Random             |     1,118.53 ns |    110.036 ns |     57.551 ns |  1.05 |    0.10 |    1 |         - |          NA |
| StableQuickSort              | 256  | Random             |     5,373.93 ns |    306.573 ns |    160.344 ns |  5.05 |    0.43 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Random             |     2,983.77 ns |    177.184 ns |     92.671 ns |  2.80 |    0.24 |    3 |         - |          NA |
| DestswapStableQuickSort      | 256  | Random             |     3,930.90 ns |    207.264 ns |    108.403 ns |  3.69 |    0.31 |    4 |         - |          NA |
| IntroSort                    | 256  | Random             |       989.31 ns |     67.063 ns |     35.075 ns |  0.93 |    0.08 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | Random             |       864.79 ns |     13.660 ns |      4.871 ns |  0.81 |    0.07 |    1 |         - |          NA |
| PDQSort                      | 256  | Random             |       967.58 ns |    104.022 ns |     46.186 ns |  0.91 |    0.08 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | Random             |     1,732.49 ns |     36.252 ns |     16.096 ns |  1.63 |    0.13 |    2 |         - |          NA |
| Ipnsort                      | 256  | Random             |     2,551.89 ns |     37.961 ns |     16.855 ns |  2.40 |    0.19 |    3 |         - |          NA |
| StdSort                      | 256  | Random             |     1,595.02 ns |     68.298 ns |     30.325 ns |  1.50 |    0.12 |    2 |         - |          NA |
| BlockQuickSort               | 256  | Random             |     1,679.08 ns |     77.484 ns |     34.404 ns |  1.58 |    0.13 |    2 |         - |          NA |
| DotnetSort                   | 256  | Random             |       858.13 ns |    117.463 ns |     52.154 ns |  0.81 |    0.08 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **QuickSort**                    | **256**  | **SingleElementMoved** |       **643.83 ns** |     **70.406 ns** |     **31.261 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 256  | SingleElementMoved |     1,295.93 ns |     39.334 ns |     17.464 ns |  2.02 |    0.09 |    2 |         - |          NA |
| QuickSortMedian3             | 256  | SingleElementMoved |     1,136.05 ns |    118.575 ns |     52.648 ns |  1.77 |    0.11 |    2 |         - |          NA |
| QuickSortMedian9             | 256  | SingleElementMoved |     1,079.49 ns |    140.404 ns |     62.340 ns |  1.68 |    0.12 |    2 |         - |          NA |
| SampleSort                   | 256  | SingleElementMoved |     2,703.69 ns |     95.875 ns |     42.569 ns |  4.21 |    0.20 |    3 |         - |          NA |
| DualPivotQuickSort           | 256  | SingleElementMoved |       463.22 ns |     55.265 ns |     28.905 ns |  0.72 |    0.05 |    1 |         - |          NA |
| StableQuickSort              | 256  | SingleElementMoved |     4,641.00 ns |    189.250 ns |     84.028 ns |  7.22 |    0.35 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 256  | SingleElementMoved |     3,053.52 ns |    232.142 ns |    121.415 ns |  4.75 |    0.28 |    3 |         - |          NA |
| DestswapStableQuickSort      | 256  | SingleElementMoved |     2,384.30 ns |    129.195 ns |     67.571 ns |  3.71 |    0.20 |    3 |         - |          NA |
| IntroSort                    | 256  | SingleElementMoved |       516.51 ns |     31.929 ns |     14.177 ns |  0.80 |    0.04 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | SingleElementMoved |       694.90 ns |     54.405 ns |     28.455 ns |  1.08 |    0.06 |    1 |         - |          NA |
| PDQSort                      | 256  | SingleElementMoved |       674.97 ns |     31.136 ns |     13.825 ns |  1.05 |    0.05 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | SingleElementMoved |       959.80 ns |     44.344 ns |     19.689 ns |  1.49 |    0.07 |    2 |         - |          NA |
| Ipnsort                      | 256  | SingleElementMoved |     2,499.13 ns |     60.272 ns |     26.761 ns |  3.89 |    0.18 |    3 |         - |          NA |
| StdSort                      | 256  | SingleElementMoved |     1,074.79 ns |     72.443 ns |     32.165 ns |  1.67 |    0.09 |    2 |         - |          NA |
| BlockQuickSort               | 256  | SingleElementMoved |     1,092.59 ns |     67.233 ns |     29.852 ns |  1.70 |    0.09 |    2 |         - |          NA |
| DotnetSort                   | 256  | SingleElementMoved |       587.92 ns |     17.568 ns |      7.800 ns |  0.92 |    0.04 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Sorted**             |       **471.80 ns** |     **34.662 ns** |     **18.129 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 256  | Sorted             |     1,083.95 ns |     31.479 ns |     13.977 ns |  2.30 |    0.09 |    7 |         - |          NA |
| QuickSortMedian3             | 256  | Sorted             |       682.40 ns |     48.517 ns |     25.376 ns |  1.45 |    0.07 |    6 |         - |          NA |
| QuickSortMedian9             | 256  | Sorted             |       702.04 ns |     75.332 ns |     33.448 ns |  1.49 |    0.08 |    6 |         - |          NA |
| SampleSort                   | 256  | Sorted             |     2,848.86 ns |    216.072 ns |     95.937 ns |  6.05 |    0.29 |    8 |         - |          NA |
| DualPivotQuickSort           | 256  | Sorted             |       337.27 ns |     12.475 ns |      5.539 ns |  0.72 |    0.03 |    4 |         - |          NA |
| StableQuickSort              | 256  | Sorted             |     4,651.42 ns |    256.431 ns |    134.118 ns |  9.87 |    0.44 |    9 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Sorted             |     2,982.85 ns |     28.519 ns |     12.663 ns |  6.33 |    0.23 |    8 |         - |          NA |
| DestswapStableQuickSort      | 256  | Sorted             |     2,469.80 ns |    187.057 ns |     83.055 ns |  5.24 |    0.25 |    8 |         - |          NA |
| IntroSort                    | 256  | Sorted             |       165.95 ns |     28.296 ns |     14.799 ns |  0.35 |    0.03 |    2 |         - |          NA |
| IntroSortDotnet              | 256  | Sorted             |       530.49 ns |    231.010 ns |    120.823 ns |  1.13 |    0.25 |    5 |         - |          NA |
| PDQSort                      | 256  | Sorted             |       219.67 ns |     13.790 ns |      7.212 ns |  0.47 |    0.02 |    3 |         - |          NA |
| PDQSortBranchless            | 256  | Sorted             |       203.80 ns |      1.973 ns |      0.703 ns |  0.43 |    0.02 |    3 |         - |          NA |
| Ipnsort                      | 256  | Sorted             |        98.48 ns |      6.584 ns |      2.923 ns |  0.21 |    0.01 |    1 |         - |          NA |
| StdSort                      | 256  | Sorted             |       219.18 ns |      3.704 ns |      1.321 ns |  0.47 |    0.02 |    3 |         - |          NA |
| BlockQuickSort               | 256  | Sorted             |       803.58 ns |     17.404 ns |      9.103 ns |  1.71 |    0.06 |    6 |         - |          NA |
| DotnetSort                   | 256  | Sorted             |       504.51 ns |     14.661 ns |      6.509 ns |  1.07 |    0.04 |    5 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Reversed**           |       **489.72 ns** |     **39.727 ns** |     **20.778 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 256  | Reversed           |     1,188.65 ns |     38.160 ns |     16.943 ns |  2.43 |    0.10 |    6 |         - |          NA |
| QuickSortMedian3             | 256  | Reversed           |       704.81 ns |     37.536 ns |     16.666 ns |  1.44 |    0.07 |    5 |         - |          NA |
| QuickSortMedian9             | 256  | Reversed           |       758.53 ns |     16.757 ns |      7.440 ns |  1.55 |    0.06 |    5 |         - |          NA |
| SampleSort                   | 256  | Reversed           |     2,929.60 ns |    114.771 ns |     50.959 ns |  5.99 |    0.26 |    7 |         - |          NA |
| DualPivotQuickSort           | 256  | Reversed           |       557.97 ns |     27.851 ns |     14.567 ns |  1.14 |    0.05 |    4 |         - |          NA |
| StableQuickSort              | 256  | Reversed           |     4,425.22 ns |    198.533 ns |    103.837 ns |  9.05 |    0.41 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Reversed           |     2,901.14 ns |    239.881 ns |    106.508 ns |  5.93 |    0.31 |    7 |         - |          NA |
| DestswapStableQuickSort      | 256  | Reversed           |     4,002.22 ns |    276.013 ns |    122.552 ns |  8.19 |    0.40 |    8 |         - |          NA |
| IntroSort                    | 256  | Reversed           |       267.25 ns |     34.271 ns |     15.217 ns |  0.55 |    0.04 |    2 |         - |          NA |
| IntroSortDotnet              | 256  | Reversed           |       837.20 ns |     28.909 ns |     12.836 ns |  1.71 |    0.07 |    5 |         - |          NA |
| PDQSort                      | 256  | Reversed           |       322.22 ns |     23.993 ns |     12.549 ns |  0.66 |    0.04 |    3 |         - |          NA |
| PDQSortBranchless            | 256  | Reversed           |       531.11 ns |     52.379 ns |     23.257 ns |  1.09 |    0.06 |    4 |         - |          NA |
| Ipnsort                      | 256  | Reversed           |       124.54 ns |      2.399 ns |      1.065 ns |  0.25 |    0.01 |    1 |         - |          NA |
| StdSort                      | 256  | Reversed           |       515.38 ns |     43.813 ns |     19.453 ns |  1.05 |    0.06 |    4 |         - |          NA |
| BlockQuickSort               | 256  | Reversed           |       946.99 ns |    113.693 ns |     50.480 ns |  1.94 |    0.12 |    5 |         - |          NA |
| DotnetSort                   | 256  | Reversed           |       858.27 ns |    144.582 ns |     75.619 ns |  1.76 |    0.16 |    5 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **QuickSort**                    | **256**  | **PipeOrgan**          |     **5,332.27 ns** |    **206.749 ns** |    **108.134 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 256  | PipeOrgan          |     2,024.30 ns |    253.093 ns |    112.375 ns |  0.38 |    0.02 |    2 |         - |          NA |
| QuickSortMedian3             | 256  | PipeOrgan          |     1,988.66 ns |    246.378 ns |    128.860 ns |  0.37 |    0.02 |    2 |         - |          NA |
| QuickSortMedian9             | 256  | PipeOrgan          |     1,282.93 ns |     62.353 ns |     32.612 ns |  0.24 |    0.01 |    2 |         - |          NA |
| SampleSort                   | 256  | PipeOrgan          |     2,817.45 ns |    112.543 ns |     49.970 ns |  0.53 |    0.01 |    3 |         - |          NA |
| DualPivotQuickSort           | 256  | PipeOrgan          |       892.69 ns |    123.278 ns |     54.736 ns |  0.17 |    0.01 |    1 |         - |          NA |
| StableQuickSort              | 256  | PipeOrgan          |     4,655.68 ns |    322.753 ns |    168.806 ns |  0.87 |    0.03 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 256  | PipeOrgan          |     3,092.35 ns |    207.484 ns |     92.124 ns |  0.58 |    0.02 |    3 |         - |          NA |
| DestswapStableQuickSort      | 256  | PipeOrgan          |     4,300.42 ns |    271.124 ns |    141.803 ns |  0.81 |    0.03 |    4 |         - |          NA |
| IntroSort                    | 256  | PipeOrgan          |       837.47 ns |     66.997 ns |     35.041 ns |  0.16 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | PipeOrgan          |     1,409.45 ns |    116.191 ns |     51.590 ns |  0.26 |    0.01 |    2 |         - |          NA |
| PDQSort                      | 256  | PipeOrgan          |     1,539.53 ns |    537.873 ns |    281.318 ns |  0.29 |    0.05 |    2 |         - |          NA |
| PDQSortBranchless            | 256  | PipeOrgan          |     1,759.96 ns |     33.745 ns |     14.983 ns |  0.33 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 256  | PipeOrgan          |     2,655.03 ns |     23.696 ns |     10.521 ns |  0.50 |    0.01 |    3 |         - |          NA |
| StdSort                      | 256  | PipeOrgan          |     2,639.02 ns |    177.857 ns |     93.023 ns |  0.50 |    0.02 |    3 |         - |          NA |
| BlockQuickSort               | 256  | PipeOrgan          |     3,117.58 ns |    334.912 ns |    175.166 ns |  0.58 |    0.03 |    3 |         - |          NA |
| DotnetSort                   | 256  | PipeOrgan          |     1,449.82 ns |     76.582 ns |     34.003 ns |  0.27 |    0.01 |    2 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **QuickSort**                    | **256**  | **ManyDuplicates**     |       **976.65 ns** |     **98.167 ns** |     **51.343 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 256  | ManyDuplicates     |       867.09 ns |    123.988 ns |     64.848 ns |  0.89 |    0.08 |    1 |         - |          NA |
| QuickSortMedian3             | 256  | ManyDuplicates     |     1,240.76 ns |    115.930 ns |     60.634 ns |  1.27 |    0.09 |    1 |         - |          NA |
| QuickSortMedian9             | 256  | ManyDuplicates     |     1,118.05 ns |    139.878 ns |     62.107 ns |  1.15 |    0.08 |    1 |         - |          NA |
| SampleSort                   | 256  | ManyDuplicates     |     2,713.53 ns |    237.995 ns |    124.476 ns |  2.79 |    0.19 |    3 |         - |          NA |
| DualPivotQuickSort           | 256  | ManyDuplicates     |       966.25 ns |    217.006 ns |    113.499 ns |  0.99 |    0.12 |    1 |         - |          NA |
| StableQuickSort              | 256  | ManyDuplicates     |     2,378.12 ns |    131.522 ns |     58.396 ns |  2.44 |    0.14 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 256  | ManyDuplicates     |     1,707.58 ns |    221.016 ns |    115.596 ns |  1.75 |    0.14 |    2 |         - |          NA |
| DestswapStableQuickSort      | 256  | ManyDuplicates     |     3,448.12 ns |    636.393 ns |    332.846 ns |  3.54 |    0.37 |    4 |         - |          NA |
| IntroSort                    | 256  | ManyDuplicates     |       907.57 ns |    109.114 ns |     48.447 ns |  0.93 |    0.07 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | ManyDuplicates     |       833.31 ns |    105.583 ns |     55.222 ns |  0.86 |    0.07 |    1 |         - |          NA |
| PDQSort                      | 256  | ManyDuplicates     |       714.65 ns |     31.875 ns |     11.367 ns |  0.73 |    0.04 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | ManyDuplicates     |     1,488.07 ns |     78.354 ns |     34.789 ns |  1.53 |    0.08 |    2 |         - |          NA |
| Ipnsort                      | 256  | ManyDuplicates     |     2,532.77 ns |     31.589 ns |     16.522 ns |  2.60 |    0.13 |    3 |         - |          NA |
| StdSort                      | 256  | ManyDuplicates     |     1,612.36 ns |     76.662 ns |     40.096 ns |  1.65 |    0.09 |    2 |         - |          NA |
| BlockQuickSort               | 256  | ManyDuplicates     |     1,634.72 ns |    274.863 ns |    143.759 ns |  1.68 |    0.16 |    2 |         - |          NA |
| DotnetSort                   | 256  | ManyDuplicates     |       763.07 ns |    101.518 ns |     53.096 ns |  0.78 |    0.06 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Random**             |     **5,122.83 ns** |    **382.638 ns** |    **169.894 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Random             |     8,178.85 ns |  1,343.451 ns |    596.501 ns |  1.60 |    0.12 |    3 |         - |          NA |
| QuickSortMedian3             | 1024 | Random             |     6,704.26 ns |    352.821 ns |    184.532 ns |  1.31 |    0.05 |    2 |         - |          NA |
| QuickSortMedian9             | 1024 | Random             |     5,485.45 ns |    464.478 ns |    242.931 ns |  1.07 |    0.06 |    1 |         - |          NA |
| SampleSort                   | 1024 | Random             |    12,055.17 ns |    972.160 ns |    508.458 ns |  2.36 |    0.12 |    4 |         - |          NA |
| DualPivotQuickSort           | 1024 | Random             |     4,911.57 ns |    854.237 ns |    446.782 ns |  0.96 |    0.09 |    1 |         - |          NA |
| StableQuickSort              | 1024 | Random             |    29,851.36 ns |  1,805.464 ns |    944.293 ns |  5.83 |    0.25 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Random             |    14,091.59 ns |  1,059.981 ns |    470.638 ns |  2.75 |    0.12 |    4 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Random             |    16,178.24 ns |    421.161 ns |    186.998 ns |  3.16 |    0.10 |    4 |         - |          NA |
| IntroSort                    | 1024 | Random             |     4,662.53 ns |    455.446 ns |    238.207 ns |  0.91 |    0.05 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | Random             |     4,397.13 ns |    286.248 ns |    149.713 ns |  0.86 |    0.04 |    1 |         - |          NA |
| PDQSort                      | 1024 | Random             |     4,204.28 ns |    615.734 ns |    322.041 ns |  0.82 |    0.06 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | Random             |     8,122.84 ns |    373.779 ns |    165.960 ns |  1.59 |    0.06 |    3 |         - |          NA |
| Ipnsort                      | 1024 | Random             |    13,344.30 ns |    213.076 ns |     94.607 ns |  2.61 |    0.08 |    4 |         - |          NA |
| StdSort                      | 1024 | Random             |     8,149.20 ns |    393.883 ns |    206.008 ns |  1.59 |    0.06 |    3 |         - |          NA |
| BlockQuickSort               | 1024 | Random             |     9,417.66 ns |    411.010 ns |    214.966 ns |  1.84 |    0.07 |    3 |         - |          NA |
| DotnetSort                   | 1024 | Random             |     4,372.81 ns |    393.308 ns |    205.708 ns |  0.85 |    0.05 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **QuickSort**                    | **1024** | **SingleElementMoved** |     **3,378.38 ns** |    **241.727 ns** |    **126.428 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 1024 | SingleElementMoved |     6,671.14 ns |    699.005 ns |    365.593 ns |  1.98 |    0.12 |    4 |         - |          NA |
| QuickSortMedian3             | 1024 | SingleElementMoved |     5,437.60 ns |    431.958 ns |    225.923 ns |  1.61 |    0.09 |    3 |         - |          NA |
| QuickSortMedian9             | 1024 | SingleElementMoved |     5,457.93 ns |    976.241 ns |    510.593 ns |  1.62 |    0.15 |    3 |         - |          NA |
| SampleSort                   | 1024 | SingleElementMoved |    11,152.91 ns |    786.997 ns |    411.615 ns |  3.31 |    0.17 |    5 |         - |          NA |
| DualPivotQuickSort           | 1024 | SingleElementMoved |     2,043.49 ns |    130.986 ns |     68.508 ns |  0.61 |    0.03 |    1 |         - |          NA |
| StableQuickSort              | 1024 | SingleElementMoved |    22,014.06 ns |    957.020 ns |    500.540 ns |  6.52 |    0.28 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | SingleElementMoved |    14,850.98 ns |    286.531 ns |    149.861 ns |  4.40 |    0.17 |    5 |         - |          NA |
| DestswapStableQuickSort      | 1024 | SingleElementMoved |    12,198.39 ns |  1,361.898 ns |    712.299 ns |  3.62 |    0.24 |    5 |         - |          NA |
| IntroSort                    | 1024 | SingleElementMoved |     2,638.67 ns |    103.087 ns |     53.917 ns |  0.78 |    0.03 |    2 |         - |          NA |
| IntroSortDotnet              | 1024 | SingleElementMoved |     3,655.95 ns |    291.203 ns |    152.305 ns |  1.08 |    0.06 |    2 |         - |          NA |
| PDQSort                      | 1024 | SingleElementMoved |     3,152.69 ns |    256.265 ns |    134.032 ns |  0.93 |    0.05 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | SingleElementMoved |     4,006.32 ns |    210.021 ns |     93.251 ns |  1.19 |    0.05 |    2 |         - |          NA |
| Ipnsort                      | 1024 | SingleElementMoved |    13,389.28 ns |    207.804 ns |    108.686 ns |  3.97 |    0.15 |    5 |         - |          NA |
| StdSort                      | 1024 | SingleElementMoved |     4,913.92 ns |    192.466 ns |     85.456 ns |  1.46 |    0.06 |    3 |         - |          NA |
| BlockQuickSort               | 1024 | SingleElementMoved |     6,383.33 ns |    237.049 ns |    123.981 ns |  1.89 |    0.08 |    4 |         - |          NA |
| DotnetSort                   | 1024 | SingleElementMoved |     3,618.99 ns |    300.007 ns |    133.205 ns |  1.07 |    0.05 |    2 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Sorted**             |     **2,455.04 ns** |    **376.496 ns** |    **167.167 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Sorted             |     5,083.67 ns |    205.992 ns |    107.738 ns |  2.08 |    0.14 |    6 |         - |          NA |
| QuickSortMedian3             | 1024 | Sorted             |     3,297.01 ns |    155.593 ns |     69.084 ns |  1.35 |    0.09 |    5 |         - |          NA |
| QuickSortMedian9             | 1024 | Sorted             |     3,656.73 ns |    130.154 ns |     57.789 ns |  1.50 |    0.10 |    5 |         - |          NA |
| SampleSort                   | 1024 | Sorted             |    11,573.46 ns |    656.473 ns |    343.348 ns |  4.73 |    0.33 |    7 |         - |          NA |
| DualPivotQuickSort           | 1024 | Sorted             |     1,977.44 ns |    158.772 ns |     83.041 ns |  0.81 |    0.06 |    4 |         - |          NA |
| StableQuickSort              | 1024 | Sorted             |    23,265.74 ns |    629.200 ns |    329.084 ns |  9.51 |    0.62 |    9 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Sorted             |    15,373.78 ns |    401.143 ns |    178.110 ns |  6.29 |    0.41 |    8 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Sorted             |    12,480.78 ns |    500.724 ns |    261.889 ns |  5.10 |    0.34 |    7 |         - |          NA |
| IntroSort                    | 1024 | Sorted             |       632.12 ns |     19.686 ns |     10.296 ns |  0.26 |    0.02 |    2 |         - |          NA |
| IntroSortDotnet              | 1024 | Sorted             |     2,590.93 ns |    132.544 ns |     69.323 ns |  1.06 |    0.07 |    5 |         - |          NA |
| PDQSort                      | 1024 | Sorted             |       796.00 ns |     14.337 ns |      6.366 ns |  0.33 |    0.02 |    3 |         - |          NA |
| PDQSortBranchless            | 1024 | Sorted             |       782.48 ns |     62.992 ns |     27.969 ns |  0.32 |    0.02 |    3 |         - |          NA |
| Ipnsort                      | 1024 | Sorted             |       293.73 ns |     23.669 ns |     10.509 ns |  0.12 |    0.01 |    1 |         - |          NA |
| StdSort                      | 1024 | Sorted             |       863.65 ns |     48.903 ns |     21.713 ns |  0.35 |    0.02 |    3 |         - |          NA |
| BlockQuickSort               | 1024 | Sorted             |     5,450.73 ns |    399.117 ns |    208.746 ns |  2.23 |    0.16 |    6 |         - |          NA |
| DotnetSort                   | 1024 | Sorted             |     2,954.09 ns |    330.978 ns |    173.108 ns |  1.21 |    0.10 |    5 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Reversed**           |     **2,501.39 ns** |     **56.091 ns** |     **24.905 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Reversed           |     5,352.17 ns |    186.302 ns |     82.719 ns |  2.14 |    0.04 |    6 |         - |          NA |
| QuickSortMedian3             | 1024 | Reversed           |     3,511.87 ns |    266.059 ns |    139.154 ns |  1.40 |    0.05 |    5 |         - |          NA |
| QuickSortMedian9             | 1024 | Reversed           |     3,622.15 ns |    443.187 ns |    231.795 ns |  1.45 |    0.09 |    5 |         - |          NA |
| SampleSort                   | 1024 | Reversed           |    12,745.56 ns |    389.489 ns |    203.710 ns |  5.10 |    0.09 |    7 |         - |          NA |
| DualPivotQuickSort           | 1024 | Reversed           |     2,697.90 ns |    295.147 ns |    154.368 ns |  1.08 |    0.06 |    4 |         - |          NA |
| StableQuickSort              | 1024 | Reversed           |    21,752.71 ns |    548.577 ns |    286.916 ns |  8.70 |    0.14 |    9 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Reversed           |    14,370.55 ns |    503.114 ns |    263.139 ns |  5.75 |    0.11 |    7 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Reversed           |    18,079.92 ns |    737.018 ns |    385.474 ns |  7.23 |    0.16 |    8 |         - |          NA |
| IntroSort                    | 1024 | Reversed           |     1,178.87 ns |     90.824 ns |     47.503 ns |  0.47 |    0.02 |    2 |         - |          NA |
| IntroSortDotnet              | 1024 | Reversed           |     4,582.15 ns |    317.385 ns |    165.999 ns |  1.83 |    0.06 |    6 |         - |          NA |
| PDQSort                      | 1024 | Reversed           |     1,151.97 ns |     14.922 ns |      5.321 ns |  0.46 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | Reversed           |     1,849.47 ns |     47.798 ns |     21.223 ns |  0.74 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 1024 | Reversed           |       464.60 ns |     22.736 ns |     10.095 ns |  0.19 |    0.00 |    1 |         - |          NA |
| StdSort                      | 1024 | Reversed           |     1,861.51 ns |     27.196 ns |      9.698 ns |  0.74 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 1024 | Reversed           |     6,094.61 ns |    429.310 ns |    224.538 ns |  2.44 |    0.09 |    6 |         - |          NA |
| DotnetSort                   | 1024 | Reversed           |     4,651.30 ns |    314.530 ns |    164.505 ns |  1.86 |    0.06 |    6 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **QuickSort**                    | **1024** | **PipeOrgan**          |    **76,311.45 ns** |  **1,091.586 ns** |    **570.921 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 1024 | PipeOrgan          |    11,410.64 ns |    460.956 ns |    204.667 ns |  0.15 |    0.00 |    2 |         - |          NA |
| QuickSortMedian3             | 1024 | PipeOrgan          |     9,931.62 ns |    719.279 ns |    376.197 ns |  0.13 |    0.00 |    2 |         - |          NA |
| QuickSortMedian9             | 1024 | PipeOrgan          |     6,010.09 ns |    380.608 ns |    199.065 ns |  0.08 |    0.00 |    1 |         - |          NA |
| SampleSort                   | 1024 | PipeOrgan          |    12,875.38 ns |    843.846 ns |    441.348 ns |  0.17 |    0.01 |    2 |         - |          NA |
| DualPivotQuickSort           | 1024 | PipeOrgan          |     4,855.77 ns |    474.580 ns |    248.215 ns |  0.06 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 1024 | PipeOrgan          |    22,169.94 ns |    332.346 ns |    173.823 ns |  0.29 |    0.00 |    2 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | PipeOrgan          |    15,620.34 ns |    388.742 ns |    203.320 ns |  0.20 |    0.00 |    2 |         - |          NA |
| DestswapStableQuickSort      | 1024 | PipeOrgan          |    20,020.03 ns |    750.345 ns |    392.445 ns |  0.26 |    0.01 |    2 |         - |          NA |
| IntroSort                    | 1024 | PipeOrgan          |     4,295.82 ns |    336.340 ns |    149.337 ns |  0.06 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | PipeOrgan          |     9,252.21 ns |  1,305.672 ns |    682.892 ns |  0.12 |    0.01 |    2 |         - |          NA |
| PDQSort                      | 1024 | PipeOrgan          |     5,307.37 ns |    463.859 ns |    242.607 ns |  0.07 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | PipeOrgan          |     9,234.57 ns |    308.780 ns |    161.498 ns |  0.12 |    0.00 |    2 |         - |          NA |
| Ipnsort                      | 1024 | PipeOrgan          |    15,480.74 ns |    240.942 ns |    126.017 ns |  0.20 |    0.00 |    2 |         - |          NA |
| StdSort                      | 1024 | PipeOrgan          |    16,450.60 ns |  3,362.333 ns |  1,758.565 ns |  0.22 |    0.02 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | PipeOrgan          |    17,000.04 ns |    264.405 ns |    138.289 ns |  0.22 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 1024 | PipeOrgan          |     8,934.11 ns |    315.771 ns |    140.204 ns |  0.12 |    0.00 |    2 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **QuickSort**                    | **1024** | **ManyDuplicates**     |     **3,904.25 ns** |    **316.120 ns** |    **165.337 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 1024 | ManyDuplicates     |     2,854.15 ns |    311.827 ns |    163.091 ns |  0.73 |    0.05 |    2 |         - |          NA |
| QuickSortMedian3             | 1024 | ManyDuplicates     |     5,481.78 ns |    416.091 ns |    217.624 ns |  1.41 |    0.08 |    4 |         - |          NA |
| QuickSortMedian9             | 1024 | ManyDuplicates     |     5,118.46 ns |    145.479 ns |     64.594 ns |  1.31 |    0.06 |    4 |         - |          NA |
| SampleSort                   | 1024 | ManyDuplicates     |    11,522.18 ns |    785.421 ns |    348.732 ns |  2.96 |    0.15 |    6 |         - |          NA |
| DualPivotQuickSort           | 1024 | ManyDuplicates     |     3,020.70 ns |    308.780 ns |    137.100 ns |  0.77 |    0.05 |    2 |         - |          NA |
| StableQuickSort              | 1024 | ManyDuplicates     |    11,615.46 ns |  1,069.391 ns |    559.312 ns |  2.98 |    0.18 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | ManyDuplicates     |     6,064.43 ns |    591.700 ns |    309.470 ns |  1.56 |    0.10 |    4 |         - |          NA |
| DestswapStableQuickSort      | 1024 | ManyDuplicates     |     7,248.89 ns |    348.633 ns |    154.795 ns |  1.86 |    0.08 |    5 |         - |          NA |
| IntroSort                    | 1024 | ManyDuplicates     |     4,134.53 ns |    580.255 ns |    257.637 ns |  1.06 |    0.08 |    3 |         - |          NA |
| IntroSortDotnet              | 1024 | ManyDuplicates     |     3,751.92 ns |    410.652 ns |    214.779 ns |  0.96 |    0.07 |    3 |         - |          NA |
| PDQSort                      | 1024 | ManyDuplicates     |     2,069.20 ns |    302.617 ns |    134.364 ns |  0.53 |    0.04 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | ManyDuplicates     |     5,225.67 ns |    254.484 ns |    133.100 ns |  1.34 |    0.06 |    4 |         - |          NA |
| Ipnsort                      | 1024 | ManyDuplicates     |    13,010.80 ns |    205.324 ns |    107.388 ns |  3.34 |    0.14 |    6 |         - |          NA |
| StdSort                      | 1024 | ManyDuplicates     |     7,334.76 ns |    462.866 ns |    205.515 ns |  1.88 |    0.09 |    5 |         - |          NA |
| BlockQuickSort               | 1024 | ManyDuplicates     |     8,102.95 ns |    238.963 ns |    124.982 ns |  2.08 |    0.09 |    5 |         - |          NA |
| DotnetSort                   | 1024 | ManyDuplicates     |     3,000.85 ns |    379.310 ns |    168.416 ns |  0.77 |    0.05 |    2 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Random**             |   **128,824.45 ns** |  **5,798.773 ns** |  **2,574.691 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Random             |   132,290.24 ns | 14,845.779 ns |  7,764.629 ns |  1.03 |    0.06 |    4 |         - |          NA |
| QuickSortMedian3             | 4096 | Random             |    80,921.76 ns | 37,232.954 ns | 19,473.555 ns |  0.63 |    0.14 |    3 |         - |          NA |
| QuickSortMedian9             | 4096 | Random             |   125,975.15 ns |  4,685.187 ns |  2,080.252 ns |  0.98 |    0.02 |    4 |         - |          NA |
| SampleSort                   | 4096 | Random             |   161,163.81 ns |  8,420.122 ns |  3,738.586 ns |  1.25 |    0.04 |    5 |         - |          NA |
| DualPivotQuickSort           | 4096 | Random             |    28,573.22 ns |  8,742.667 ns |  4,572.584 ns |  0.22 |    0.03 |    1 |         - |          NA |
| StableQuickSort              | 4096 | Random             |   361,954.04 ns |  3,235.191 ns |  1,436.445 ns |  2.81 |    0.05 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Random             |   335,429.54 ns |  4,239.604 ns |  2,217.395 ns |  2.60 |    0.05 |    6 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Random             |   185,775.20 ns |  7,170.277 ns |  3,183.648 ns |  1.44 |    0.04 |    5 |         - |          NA |
| IntroSort                    | 4096 | Random             |    73,611.16 ns | 23,030.414 ns | 12,045.352 ns |  0.57 |    0.09 |    3 |         - |          NA |
| IntroSortDotnet              | 4096 | Random             |    22,818.71 ns |  4,626.382 ns |  2,054.142 ns |  0.18 |    0.02 |    1 |         - |          NA |
| PDQSort                      | 4096 | Random             |    46,762.93 ns | 45,339.916 ns | 23,713.653 ns |  0.36 |    0.17 |    2 |         - |          NA |
| PDQSortBranchless            | 4096 | Random             |    46,876.98 ns | 12,585.517 ns |  5,588.048 ns |  0.36 |    0.04 |    2 |         - |          NA |
| Ipnsort                      | 4096 | Random             |    70,629.97 ns |    835.039 ns |    436.741 ns |  0.55 |    0.01 |    3 |         - |          NA |
| StdSort                      | 4096 | Random             |    39,783.48 ns |    530.482 ns |    277.453 ns |  0.31 |    0.01 |    2 |         - |          NA |
| BlockQuickSort               | 4096 | Random             |    46,745.50 ns |    859.515 ns |    449.543 ns |  0.36 |    0.01 |    2 |         - |          NA |
| DotnetSort                   | 4096 | Random             |    41,493.42 ns | 15,940.705 ns |  8,337.297 ns |  0.32 |    0.06 |    2 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **QuickSort**                    | **4096** | **SingleElementMoved** |    **15,551.03 ns** |    **892.512 ns** |    **466.801 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 4096 | SingleElementMoved |    29,049.45 ns |    760.690 ns |    397.856 ns |  1.87 |    0.06 |    1 |         - |          NA |
| QuickSortMedian3             | 4096 | SingleElementMoved |    23,732.96 ns |  1,892.999 ns |    990.075 ns |  1.53 |    0.07 |    1 |         - |          NA |
| QuickSortMedian9             | 4096 | SingleElementMoved |    24,765.76 ns |  1,287.167 ns |    571.510 ns |  1.59 |    0.06 |    1 |         - |          NA |
| SampleSort                   | 4096 | SingleElementMoved |    52,816.64 ns |  5,081.309 ns |  2,657.623 ns |  3.40 |    0.19 |    2 |         - |          NA |
| DualPivotQuickSort           | 4096 | SingleElementMoved |    11,967.14 ns |  2,500.246 ns |  1,110.125 ns |  0.77 |    0.07 |    1 |         - |          NA |
| StableQuickSort              | 4096 | SingleElementMoved |   105,296.23 ns |  2,804.741 ns |  1,245.322 ns |  6.78 |    0.20 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | SingleElementMoved |    72,855.49 ns |  2,384.288 ns |  1,247.029 ns |  4.69 |    0.15 |    2 |         - |          NA |
| DestswapStableQuickSort      | 4096 | SingleElementMoved |    54,472.29 ns |  2,171.221 ns |  1,135.591 ns |  3.51 |    0.12 |    2 |         - |          NA |
| IntroSort                    | 4096 | SingleElementMoved |    11,837.24 ns |    433.709 ns |    192.570 ns |  0.76 |    0.02 |    1 |         - |          NA |
| IntroSortDotnet              | 4096 | SingleElementMoved |    19,257.80 ns |  1,662.000 ns |    737.938 ns |  1.24 |    0.06 |    1 |         - |          NA |
| PDQSort                      | 4096 | SingleElementMoved |    13,866.90 ns |    949.576 ns |    496.647 ns |  0.89 |    0.04 |    1 |         - |          NA |
| PDQSortBranchless            | 4096 | SingleElementMoved |    17,617.53 ns |    557.420 ns |    247.498 ns |  1.13 |    0.04 |    1 |         - |          NA |
| Ipnsort                      | 4096 | SingleElementMoved |    62,985.11 ns |    418.865 ns |    219.075 ns |  4.05 |    0.11 |    2 |         - |          NA |
| StdSort                      | 4096 | SingleElementMoved |    20,804.62 ns |    709.012 ns |    314.806 ns |  1.34 |    0.04 |    1 |         - |          NA |
| BlockQuickSort               | 4096 | SingleElementMoved |    31,647.87 ns |    624.461 ns |    326.605 ns |  2.04 |    0.06 |    1 |         - |          NA |
| DotnetSort                   | 4096 | SingleElementMoved |    21,424.55 ns |    430.707 ns |    191.237 ns |  1.38 |    0.04 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Sorted**             |    **11,887.00 ns** |    **867.997 ns** |    **453.979 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Sorted             |    23,090.15 ns |  1,929.619 ns |  1,009.228 ns |  1.94 |    0.11 |    5 |         - |          NA |
| QuickSortMedian3             | 4096 | Sorted             |    15,490.42 ns |    272.539 ns |     97.190 ns |  1.30 |    0.05 |    4 |         - |          NA |
| QuickSortMedian9             | 4096 | Sorted             |    16,674.77 ns |  1,420.647 ns |    743.026 ns |  1.40 |    0.08 |    4 |         - |          NA |
| SampleSort                   | 4096 | Sorted             |    52,965.29 ns |  5,043.779 ns |  2,637.994 ns |  4.46 |    0.26 |    6 |         - |          NA |
| DualPivotQuickSort           | 4096 | Sorted             |    10,511.52 ns |    545.589 ns |    242.245 ns |  0.89 |    0.04 |    4 |         - |          NA |
| StableQuickSort              | 4096 | Sorted             |   106,173.17 ns |  3,952.606 ns |  1,754.982 ns |  8.94 |    0.35 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Sorted             |    69,184.52 ns |  5,175.279 ns |  2,706.771 ns |  5.83 |    0.30 |    7 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Sorted             |    54,531.69 ns |  2,822.282 ns |  1,476.108 ns |  4.59 |    0.20 |    6 |         - |          NA |
| IntroSort                    | 4096 | Sorted             |     2,276.61 ns |    315.930 ns |    140.275 ns |  0.19 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 4096 | Sorted             |    12,592.54 ns |    657.682 ns |    292.015 ns |  1.06 |    0.04 |    4 |         - |          NA |
| PDQSort                      | 4096 | Sorted             |     3,101.11 ns |    309.946 ns |    137.618 ns |  0.26 |    0.01 |    3 |         - |          NA |
| PDQSortBranchless            | 4096 | Sorted             |     3,115.31 ns |    407.891 ns |    181.106 ns |  0.26 |    0.02 |    3 |         - |          NA |
| Ipnsort                      | 4096 | Sorted             |     1,081.06 ns |     28.863 ns |     15.096 ns |  0.09 |    0.00 |    1 |         - |          NA |
| StdSort                      | 4096 | Sorted             |     3,233.12 ns |    530.445 ns |    277.433 ns |  0.27 |    0.02 |    3 |         - |          NA |
| BlockQuickSort               | 4096 | Sorted             |    25,574.24 ns |    571.382 ns |    253.697 ns |  2.15 |    0.08 |    5 |         - |          NA |
| DotnetSort                   | 4096 | Sorted             |    13,830.35 ns |    768.992 ns |    341.437 ns |  1.16 |    0.05 |    4 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Reversed**           |    **12,398.77 ns** |  **2,085.038 ns** |    **925.770 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Reversed           |    23,979.10 ns |    632.594 ns |    330.859 ns |  1.94 |    0.13 |    6 |         - |          NA |
| QuickSortMedian3             | 4096 | Reversed           |    17,069.84 ns |    201.432 ns |     89.437 ns |  1.38 |    0.09 |    5 |         - |          NA |
| QuickSortMedian9             | 4096 | Reversed           |    17,021.15 ns |    547.104 ns |    242.918 ns |  1.38 |    0.09 |    5 |         - |          NA |
| SampleSort                   | 4096 | Reversed           |    60,746.31 ns |  8,264.030 ns |  4,322.247 ns |  4.92 |    0.46 |    7 |         - |          NA |
| DualPivotQuickSort           | 4096 | Reversed           |    13,301.08 ns |  1,537.257 ns |    804.015 ns |  1.08 |    0.09 |    4 |         - |          NA |
| StableQuickSort              | 4096 | Reversed           |   104,994.63 ns |  3,935.815 ns |  2,058.507 ns |  8.51 |    0.57 |    9 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Reversed           |    66,945.65 ns |  1,147.727 ns |    600.283 ns |  5.42 |    0.35 |    7 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Reversed           |    80,577.21 ns |  3,020.101 ns |  1,579.571 ns |  6.53 |    0.44 |    8 |         - |          NA |
| IntroSort                    | 4096 | Reversed           |     4,019.70 ns |    581.097 ns |    303.925 ns |  0.33 |    0.03 |    2 |         - |          NA |
| IntroSortDotnet              | 4096 | Reversed           |    22,943.64 ns |  1,001.375 ns |    523.739 ns |  1.86 |    0.13 |    6 |         - |          NA |
| PDQSort                      | 4096 | Reversed           |     4,368.51 ns |    400.687 ns |    209.567 ns |  0.35 |    0.03 |    2 |         - |          NA |
| PDQSortBranchless            | 4096 | Reversed           |     7,072.54 ns |    231.570 ns |    121.116 ns |  0.57 |    0.04 |    3 |         - |          NA |
| Ipnsort                      | 4096 | Reversed           |     1,885.16 ns |    239.793 ns |    125.416 ns |  0.15 |    0.01 |    1 |         - |          NA |
| StdSort                      | 4096 | Reversed           |     7,359.40 ns |    270.794 ns |    120.234 ns |  0.60 |    0.04 |    3 |         - |          NA |
| BlockQuickSort               | 4096 | Reversed           |    27,851.25 ns |    175.178 ns |     77.780 ns |  2.26 |    0.15 |    6 |         - |          NA |
| DotnetSort                   | 4096 | Reversed           |    24,122.90 ns |    656.062 ns |    343.133 ns |  1.95 |    0.13 |    6 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **QuickSort**                    | **4096** | **PipeOrgan**          | **1,085,051.65 ns** | **12,943.524 ns** |  **5,747.005 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 4096 | PipeOrgan          |    60,123.08 ns |  1,664.832 ns |    739.196 ns |  0.06 |    0.00 |    2 |         - |          NA |
| QuickSortMedian3             | 4096 | PipeOrgan          |    50,927.72 ns |  1,903.500 ns |    845.166 ns |  0.05 |    0.00 |    2 |         - |          NA |
| QuickSortMedian9             | 4096 | PipeOrgan          |    27,678.27 ns |  1,294.922 ns |    574.953 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SampleSort                   | 4096 | PipeOrgan          |    60,030.91 ns |  7,166.062 ns |  3,747.989 ns |  0.06 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort           | 4096 | PipeOrgan          |    23,903.81 ns |  1,294.558 ns |    677.079 ns |  0.02 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 4096 | PipeOrgan          |   107,023.33 ns |  3,377.774 ns |  1,766.641 ns |  0.10 |    0.00 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | PipeOrgan          |    75,551.26 ns |  5,238.272 ns |  2,325.825 ns |  0.07 |    0.00 |    2 |         - |          NA |
| DestswapStableQuickSort      | 4096 | PipeOrgan          |    96,736.91 ns |  1,524.265 ns |    676.783 ns |  0.09 |    0.00 |    3 |         - |          NA |
| IntroSort                    | 4096 | PipeOrgan          |    21,418.84 ns |  1,964.123 ns |  1,027.274 ns |  0.02 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet              | 4096 | PipeOrgan          |    54,671.10 ns |    654.609 ns |    290.651 ns |  0.05 |    0.00 |    2 |         - |          NA |
| PDQSort                      | 4096 | PipeOrgan          |    27,268.43 ns |    972.684 ns |    431.878 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 4096 | PipeOrgan          |    45,620.78 ns |  1,015.714 ns |    531.238 ns |  0.04 |    0.00 |    2 |         - |          NA |
| Ipnsort                      | 4096 | PipeOrgan          |    76,446.68 ns |    778.938 ns |    407.400 ns |  0.07 |    0.00 |    2 |         - |          NA |
| StdSort                      | 4096 | PipeOrgan          |    66,318.58 ns |    840.974 ns |    439.846 ns |  0.06 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 4096 | PipeOrgan          |    76,245.95 ns |    816.488 ns |    427.039 ns |  0.07 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 4096 | PipeOrgan          |    50,544.23 ns |  4,209.728 ns |  1,869.145 ns |  0.05 |    0.00 |    2 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **QuickSort**                    | **4096** | **ManyDuplicates**     |    **23,213.94 ns** |  **3,040.315 ns** |  **1,590.144 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 4096 | ManyDuplicates     |    11,997.67 ns |  1,940.007 ns |  1,014.661 ns |  0.52 |    0.05 |    2 |         - |          NA |
| QuickSortMedian3             | 4096 | ManyDuplicates     |    25,362.18 ns |    898.804 ns |    399.075 ns |  1.10 |    0.07 |    3 |         - |          NA |
| QuickSortMedian9             | 4096 | ManyDuplicates     |    25,748.43 ns |  2,518.217 ns |  1,317.076 ns |  1.11 |    0.09 |    3 |         - |          NA |
| SampleSort                   | 4096 | ManyDuplicates     |    55,003.02 ns |  9,214.741 ns |  4,819.488 ns |  2.38 |    0.25 |    4 |         - |          NA |
| DualPivotQuickSort           | 4096 | ManyDuplicates     |    10,368.98 ns |    797.576 ns |    354.129 ns |  0.45 |    0.03 |    2 |         - |          NA |
| StableQuickSort              | 4096 | ManyDuplicates     |   118,078.14 ns |  6,184.495 ns |  3,234.611 ns |  5.11 |    0.36 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | ManyDuplicates     |    40,553.48 ns | 10,265.131 ns |  4,557.782 ns |  1.75 |    0.22 |    3 |         - |          NA |
| DestswapStableQuickSort      | 4096 | ManyDuplicates     |    29,523.44 ns |  2,145.564 ns |    952.644 ns |  1.28 |    0.09 |    3 |         - |          NA |
| IntroSort                    | 4096 | ManyDuplicates     |    21,120.70 ns |  3,055.961 ns |  1,598.327 ns |  0.91 |    0.09 |    3 |         - |          NA |
| IntroSortDotnet              | 4096 | ManyDuplicates     |    26,935.71 ns | 11,080.082 ns |  5,795.097 ns |  1.17 |    0.25 |    3 |         - |          NA |
| PDQSort                      | 4096 | ManyDuplicates     |     8,292.34 ns |  1,572.367 ns |    698.141 ns |  0.36 |    0.04 |    1 |         - |          NA |
| PDQSortBranchless            | 4096 | ManyDuplicates     |    16,995.29 ns |    209.345 ns |     92.950 ns |  0.74 |    0.05 |    3 |         - |          NA |
| Ipnsort                      | 4096 | ManyDuplicates     |    43,276.98 ns |    526.526 ns |    275.383 ns |  1.87 |    0.12 |    3 |         - |          NA |
| StdSort                      | 4096 | ManyDuplicates     |    24,195.82 ns |    438.997 ns |    194.917 ns |  1.05 |    0.07 |    3 |         - |          NA |
| BlockQuickSort               | 4096 | ManyDuplicates     |    34,323.85 ns |    678.206 ns |    301.128 ns |  1.48 |    0.10 |    3 |         - |          NA |
| DotnetSort                   | 4096 | ManyDuplicates     |    14,631.41 ns |  2,551.842 ns |  1,334.663 ns |  0.63 |    0.07 |    3 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Random**             |   **381,738.62 ns** | **14,144.408 ns** |  **7,397.799 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Random             |   397,095.92 ns |  7,829.977 ns |  4,095.229 ns |  1.04 |    0.02 |    3 |         - |          NA |
| QuickSortMedian3             | 8192 | Random             |   385,797.25 ns |  6,504.065 ns |  3,401.752 ns |  1.01 |    0.02 |    3 |         - |          NA |
| QuickSortMedian9             | 8192 | Random             |   398,976.32 ns |  8,903.392 ns |  4,656.646 ns |  1.05 |    0.02 |    3 |         - |          NA |
| SampleSort                   | 8192 | Random             |   467,757.08 ns | 10,078.993 ns |  5,271.508 ns |  1.23 |    0.03 |    3 |         - |          NA |
| DualPivotQuickSort           | 8192 | Random             |   265,767.35 ns |  6,132.605 ns |  3,207.471 ns |  0.70 |    0.01 |    3 |         - |          NA |
| StableQuickSort              | 8192 | Random             |   877,581.47 ns | 13,443.920 ns |  7,031.430 ns |  2.30 |    0.05 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Random             |   791,701.14 ns |  6,524.783 ns |  3,412.588 ns |  2.07 |    0.04 |    4 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Random             |   501,016.23 ns |  6,713.996 ns |  2,981.056 ns |  1.31 |    0.02 |    3 |         - |          NA |
| IntroSort                    | 8192 | Random             |   328,776.19 ns |  5,193.726 ns |  2,716.419 ns |  0.86 |    0.02 |    3 |         - |          NA |
| IntroSortDotnet              | 8192 | Random             |   300,511.44 ns | 11,014.710 ns |  5,760.906 ns |  0.79 |    0.02 |    3 |         - |          NA |
| PDQSort                      | 8192 | Random             |   293,299.78 ns | 16,082.756 ns |  8,411.592 ns |  0.77 |    0.03 |    3 |         - |          NA |
| PDQSortBranchless            | 8192 | Random             |    84,683.56 ns |  3,203.556 ns |  1,675.522 ns |  0.22 |    0.01 |    1 |         - |          NA |
| Ipnsort                      | 8192 | Random             |   152,752.70 ns |  1,959.930 ns |  1,025.081 ns |  0.40 |    0.01 |    2 |         - |          NA |
| StdSort                      | 8192 | Random             |   100,868.40 ns | 25,201.377 ns | 11,189.568 ns |  0.26 |    0.03 |    1 |         - |          NA |
| BlockQuickSort               | 8192 | Random             |   102,418.93 ns |  2,168.037 ns |    962.622 ns |  0.27 |    0.01 |    1 |         - |          NA |
| DotnetSort                   | 8192 | Random             |   281,572.12 ns | 10,775.065 ns |  5,635.567 ns |  0.74 |    0.02 |    3 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **QuickSort**                    | **8192** | **SingleElementMoved** |    **31,553.25 ns** |    **886.064 ns** |    **393.418 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 8192 | SingleElementMoved |    61,402.61 ns |  2,299.074 ns |  1,020.803 ns |  1.95 |    0.04 |    1 |         - |          NA |
| QuickSortMedian3             | 8192 | SingleElementMoved |    48,967.21 ns |  2,418.095 ns |  1,073.649 ns |  1.55 |    0.04 |    1 |         - |          NA |
| QuickSortMedian9             | 8192 | SingleElementMoved |    52,372.50 ns |  1,210.717 ns |    537.566 ns |  1.66 |    0.02 |    1 |         - |          NA |
| SampleSort                   | 8192 | SingleElementMoved |   261,869.57 ns |  5,352.116 ns |  2,799.260 ns |  8.30 |    0.13 |    3 |         - |          NA |
| DualPivotQuickSort           | 8192 | SingleElementMoved |    24,458.82 ns |  2,088.412 ns |  1,092.280 ns |  0.78 |    0.03 |    1 |         - |          NA |
| StableQuickSort              | 8192 | SingleElementMoved |   226,359.23 ns |  2,399.887 ns |  1,065.564 ns |  7.17 |    0.09 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | SingleElementMoved |   153,602.66 ns |  3,608.968 ns |  1,602.404 ns |  4.87 |    0.07 |    2 |         - |          NA |
| DestswapStableQuickSort      | 8192 | SingleElementMoved |   116,512.37 ns |  3,851.816 ns |  2,014.574 ns |  3.69 |    0.07 |    2 |         - |          NA |
| IntroSort                    | 8192 | SingleElementMoved |    24,037.75 ns |    554.519 ns |    290.024 ns |  0.76 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet              | 8192 | SingleElementMoved |    41,176.22 ns |  1,287.775 ns |    571.780 ns |  1.31 |    0.02 |    1 |         - |          NA |
| PDQSort                      | 8192 | SingleElementMoved |    28,054.17 ns |    979.005 ns |    434.684 ns |  0.89 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless            | 8192 | SingleElementMoved |    35,519.60 ns |    353.901 ns |    157.134 ns |  1.13 |    0.01 |    1 |         - |          NA |
| Ipnsort                      | 8192 | SingleElementMoved |   136,616.08 ns |  1,459.076 ns |    763.125 ns |  4.33 |    0.05 |    2 |         - |          NA |
| StdSort                      | 8192 | SingleElementMoved |    41,452.65 ns |    964.785 ns |    428.370 ns |  1.31 |    0.02 |    1 |         - |          NA |
| BlockQuickSort               | 8192 | SingleElementMoved |    66,704.33 ns |    864.192 ns |    451.989 ns |  2.11 |    0.03 |    1 |         - |          NA |
| DotnetSort                   | 8192 | SingleElementMoved |    45,670.15 ns |  2,891.921 ns |  1,512.531 ns |  1.45 |    0.05 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Sorted**             |    **25,147.86 ns** |  **1,165.449 ns** |    **517.466 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Sorted             |    49,183.84 ns |  1,823.049 ns |    953.490 ns |  1.96 |    0.05 |    5 |         - |          NA |
| QuickSortMedian3             | 8192 | Sorted             |    33,493.82 ns |  1,274.537 ns |    565.903 ns |  1.33 |    0.03 |    4 |         - |          NA |
| QuickSortMedian9             | 8192 | Sorted             |    36,023.39 ns |  2,553.604 ns |  1,335.584 ns |  1.43 |    0.06 |    4 |         - |          NA |
| SampleSort                   | 8192 | Sorted             |   262,657.28 ns |  9,750.711 ns |  5,099.810 ns | 10.45 |    0.28 |    8 |         - |          NA |
| DualPivotQuickSort           | 8192 | Sorted             |    21,424.44 ns |  3,213.966 ns |  1,427.021 ns |  0.85 |    0.06 |    4 |         - |          NA |
| StableQuickSort              | 8192 | Sorted             |   236,054.66 ns |  5,825.271 ns |  2,586.457 ns |  9.39 |    0.20 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Sorted             |   152,433.04 ns |  6,477.081 ns |  3,387.638 ns |  6.06 |    0.17 |    7 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Sorted             |   118,958.10 ns |  7,335.089 ns |  3,836.394 ns |  4.73 |    0.17 |    6 |         - |          NA |
| IntroSort                    | 8192 | Sorted             |     4,854.32 ns |    749.358 ns |    391.929 ns |  0.19 |    0.02 |    2 |         - |          NA |
| IntroSortDotnet              | 8192 | Sorted             |    27,742.83 ns |    756.862 ns |    336.051 ns |  1.10 |    0.02 |    4 |         - |          NA |
| PDQSort                      | 8192 | Sorted             |     6,071.51 ns |    484.437 ns |    215.093 ns |  0.24 |    0.01 |    3 |         - |          NA |
| PDQSortBranchless            | 8192 | Sorted             |     6,372.65 ns |    719.408 ns |    376.264 ns |  0.25 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 8192 | Sorted             |     2,146.15 ns |     94.540 ns |     49.446 ns |  0.09 |    0.00 |    1 |         - |          NA |
| StdSort                      | 8192 | Sorted             |     6,108.98 ns |    309.484 ns |    137.413 ns |  0.24 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 8192 | Sorted             |    56,198.99 ns |    698.172 ns |    365.158 ns |  2.24 |    0.04 |    5 |         - |          NA |
| DotnetSort                   | 8192 | Sorted             |    29,609.62 ns |    741.314 ns |    329.148 ns |  1.18 |    0.03 |    4 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Reversed**           |    **25,278.73 ns** |    **708.839 ns** |    **314.729 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Reversed           |    50,554.31 ns |  1,939.420 ns |  1,014.354 ns |  2.00 |    0.04 |    6 |         - |          NA |
| QuickSortMedian3             | 8192 | Reversed           |    34,733.21 ns |    396.910 ns |    176.230 ns |  1.37 |    0.02 |    5 |         - |          NA |
| QuickSortMedian9             | 8192 | Reversed           |    37,168.16 ns |  3,962.143 ns |  2,072.277 ns |  1.47 |    0.08 |    5 |         - |          NA |
| SampleSort                   | 8192 | Reversed           |   294,740.04 ns |  5,660.935 ns |  2,513.490 ns | 11.66 |    0.16 |    9 |         - |          NA |
| DualPivotQuickSort           | 8192 | Reversed           |    27,604.42 ns |  1,602.881 ns |    838.337 ns |  1.09 |    0.03 |    4 |         - |          NA |
| StableQuickSort              | 8192 | Reversed           |   224,665.07 ns |  2,303.949 ns |  1,022.967 ns |  8.89 |    0.11 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Reversed           |   146,090.59 ns |  1,659.889 ns |    591.932 ns |  5.78 |    0.07 |    7 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Reversed           |   168,988.39 ns |  2,745.301 ns |  1,435.845 ns |  6.69 |    0.09 |    7 |         - |          NA |
| IntroSort                    | 8192 | Reversed           |     7,828.97 ns |    935.950 ns |    489.520 ns |  0.31 |    0.02 |    2 |         - |          NA |
| IntroSortDotnet              | 8192 | Reversed           |    49,444.53 ns |  1,599.061 ns |    836.340 ns |  1.96 |    0.04 |    6 |         - |          NA |
| PDQSort                      | 8192 | Reversed           |     8,779.63 ns |    815.688 ns |    426.620 ns |  0.35 |    0.02 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | Reversed           |    13,405.29 ns |    331.703 ns |    147.278 ns |  0.53 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 8192 | Reversed           |     3,709.28 ns |    225.926 ns |    118.164 ns |  0.15 |    0.00 |    1 |         - |          NA |
| StdSort                      | 8192 | Reversed           |    14,070.12 ns |    577.406 ns |    256.372 ns |  0.56 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 8192 | Reversed           |    58,218.19 ns |    519.562 ns |    271.741 ns |  2.30 |    0.03 |    6 |         - |          NA |
| DotnetSort                   | 8192 | Reversed           |    53,262.71 ns |  3,363.341 ns |  1,759.092 ns |  2.11 |    0.07 |    6 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **QuickSort**                    | **8192** | **PipeOrgan**          | **4,180,635.82 ns** | **65,834.438 ns** | **34,432.684 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| QuickSort3way                | 8192 | PipeOrgan          |   143,122.87 ns |  8,400.566 ns |  4,393.658 ns |  0.03 |    0.00 |    4 |         - |          NA |
| QuickSortMedian3             | 8192 | PipeOrgan          |   121,858.90 ns |  5,388.715 ns |  2,392.623 ns |  0.03 |    0.00 |    4 |         - |          NA |
| QuickSortMedian9             | 8192 | PipeOrgan          |    60,536.88 ns |  3,880.397 ns |  2,029.523 ns |  0.01 |    0.00 |    2 |         - |          NA |
| SampleSort                   | 8192 | PipeOrgan          |   304,491.36 ns |  9,284.004 ns |  4,122.156 ns |  0.07 |    0.00 |    5 |         - |          NA |
| DualPivotQuickSort           | 8192 | PipeOrgan          |    67,879.67 ns | 26,289.683 ns | 13,750.012 ns |  0.02 |    0.00 |    2 |         - |          NA |
| StableQuickSort              | 8192 | PipeOrgan          |   219,512.67 ns |  4,034.466 ns |  2,110.104 ns |  0.05 |    0.00 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | PipeOrgan          |   177,034.59 ns | 75,832.172 ns | 33,669.954 ns |  0.04 |    0.01 |    4 |         - |          NA |
| DestswapStableQuickSort      | 8192 | PipeOrgan          |   200,279.14 ns |  5,804.637 ns |  3,035.937 ns |  0.05 |    0.00 |    5 |         - |          NA |
| IntroSort                    | 8192 | PipeOrgan          |    48,104.36 ns |  3,170.221 ns |  1,658.087 ns |  0.01 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet              | 8192 | PipeOrgan          |   263,755.05 ns |  1,885.522 ns |    837.183 ns |  0.06 |    0.00 |    5 |         - |          NA |
| PDQSort                      | 8192 | PipeOrgan          |    58,263.18 ns |  2,879.608 ns |  1,278.564 ns |  0.01 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | PipeOrgan          |    96,511.71 ns |  2,396.496 ns |  1,064.059 ns |  0.02 |    0.00 |    3 |         - |          NA |
| Ipnsort                      | 8192 | PipeOrgan          |   163,977.27 ns |  3,536.992 ns |  1,849.915 ns |  0.04 |    0.00 |    4 |         - |          NA |
| StdSort                      | 8192 | PipeOrgan          |   244,440.02 ns |  3,447.185 ns |  1,530.572 ns |  0.06 |    0.00 |    5 |         - |          NA |
| BlockQuickSort               | 8192 | PipeOrgan          |   153,661.53 ns |  2,198.088 ns |  1,149.642 ns |  0.04 |    0.00 |    4 |         - |          NA |
| DotnetSort                   | 8192 | PipeOrgan          |   236,451.97 ns | 15,504.395 ns |  8,109.099 ns |  0.06 |    0.00 |    5 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **QuickSort**                    | **8192** | **ManyDuplicates**     |   **110,081.08 ns** | **22,924.627 ns** | **11,990.023 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 8192 | ManyDuplicates     |    46,631.15 ns | 26,450.047 ns | 13,833.886 ns |  0.43 |    0.13 |    3 |         - |          NA |
| QuickSortMedian3             | 8192 | ManyDuplicates     |   138,202.06 ns |  8,243.253 ns |  3,660.055 ns |  1.27 |    0.14 |    5 |         - |          NA |
| QuickSortMedian9             | 8192 | ManyDuplicates     |   159,774.88 ns |  9,926.281 ns |  5,191.637 ns |  1.47 |    0.16 |    5 |         - |          NA |
| SampleSort                   | 8192 | ManyDuplicates     |   257,642.57 ns | 14,463.448 ns |  7,564.663 ns |  2.37 |    0.25 |    6 |         - |          NA |
| DualPivotQuickSort           | 8192 | ManyDuplicates     |    41,329.06 ns | 11,359.614 ns |  5,043.739 ns |  0.38 |    0.06 |    2 |         - |          NA |
| StableQuickSort              | 8192 | ManyDuplicates     |   275,995.08 ns |  6,111.252 ns |  3,196.303 ns |  2.53 |    0.26 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | ManyDuplicates     |   219,157.40 ns | 10,682.152 ns |  5,586.972 ns |  2.01 |    0.21 |    6 |         - |          NA |
| DestswapStableQuickSort      | 8192 | ManyDuplicates     |   125,190.41 ns |  4,946.290 ns |  2,196.184 ns |  1.15 |    0.12 |    5 |         - |          NA |
| IntroSort                    | 8192 | ManyDuplicates     |   149,718.03 ns |  8,104.068 ns |  4,238.585 ns |  1.37 |    0.15 |    5 |         - |          NA |
| IntroSortDotnet              | 8192 | ManyDuplicates     |   133,180.97 ns | 14,035.260 ns |  7,340.712 ns |  1.22 |    0.14 |    5 |         - |          NA |
| PDQSort                      | 8192 | ManyDuplicates     |    54,944.34 ns | 62,089.976 ns | 32,474.258 ns |  0.50 |    0.29 |    3 |         - |          NA |
| PDQSortBranchless            | 8192 | ManyDuplicates     |    33,192.23 ns |    853.330 ns |    446.308 ns |  0.30 |    0.03 |    1 |         - |          NA |
| Ipnsort                      | 8192 | ManyDuplicates     |    83,779.74 ns |    793.271 ns |    414.896 ns |  0.77 |    0.08 |    4 |         - |          NA |
| StdSort                      | 8192 | ManyDuplicates     |    47,213.61 ns |    770.326 ns |    402.895 ns |  0.43 |    0.04 |    3 |         - |          NA |
| BlockQuickSort               | 8192 | ManyDuplicates     |    65,784.58 ns |    885.835 ns |    463.309 ns |  0.60 |    0.06 |    3 |         - |          NA |
| DotnetSort                   | 8192 | ManyDuplicates     |   118,488.58 ns | 10,848.628 ns |  5,674.042 ns |  1.09 |    0.12 |    5 |         - |          NA |

### SelectionBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V45 4.27GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.48 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method              | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **SelectionSort**       | **256**  | **Random**             |    **13,802.0 ns** |    **381.97 ns** |    **199.78 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Random             |    11,429.1 ns |    234.57 ns |     83.65 ns |  0.83 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | Random             |    39,860.8 ns |  6,635.96 ns |  3,470.74 ns |  2.89 |    0.24 |    4 |         - |          NA |
| PancakeSort         | 256  | Random             |    23,681.4 ns |    736.85 ns |    385.38 ns |  1.72 |    0.04 |    3 |         - |          NA |
| BingoSort           | 256  | Random             |    14,077.9 ns |  1,385.97 ns |    615.38 ns |  1.02 |    0.04 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **SingleElementMoved** |    **10,862.7 ns** |     **37.64 ns** |     **13.42 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | SingleElementMoved |    10,127.1 ns |    195.58 ns |     86.84 ns |  0.93 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | SingleElementMoved |    24,832.1 ns |  1,254.29 ns |    656.02 ns |  2.29 |    0.06 |    2 |         - |          NA |
| PancakeSort         | 256  | SingleElementMoved |    11,062.9 ns |    303.23 ns |    158.60 ns |  1.02 |    0.01 |    1 |         - |          NA |
| BingoSort           | 256  | SingleElementMoved |    11,102.0 ns |    251.80 ns |    111.80 ns |  1.02 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **Sorted**             |    **10,899.9 ns** |    **127.17 ns** |     **45.35 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Sorted             |     8,644.0 ns |    376.05 ns |    196.68 ns |  0.79 |    0.02 |    1 |         - |          NA |
| CycleSort           | 256  | Sorted             |    11,193.0 ns |    345.20 ns |    153.27 ns |  1.03 |    0.01 |    1 |         - |          NA |
| PancakeSort         | 256  | Sorted             |     9,132.9 ns |    256.23 ns |    134.02 ns |  0.84 |    0.01 |    1 |         - |          NA |
| BingoSort           | 256  | Sorted             |    10,721.9 ns |     27.96 ns |      9.97 ns |  0.98 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **Reversed**           |    **11,983.2 ns** |    **461.81 ns** |    **241.54 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Reversed           |    10,120.9 ns |  2,471.15 ns |  1,292.46 ns |  0.84 |    0.10 |    1 |         - |          NA |
| CycleSort           | 256  | Reversed           |    24,972.9 ns |    662.81 ns |    346.66 ns |  2.08 |    0.05 |    3 |         - |          NA |
| PancakeSort         | 256  | Reversed           |    10,728.0 ns |    946.14 ns |    420.09 ns |  0.90 |    0.04 |    1 |         - |          NA |
| BingoSort           | 256  | Reversed           |    15,636.6 ns |  3,480.64 ns |  1,820.44 ns |  1.31 |    0.15 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **PipeOrgan**          |    **12,996.7 ns** |    **871.56 ns** |    **455.84 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | PipeOrgan          |    11,085.2 ns |    939.52 ns |    417.15 ns |  0.85 |    0.04 |    1 |         - |          NA |
| CycleSort           | 256  | PipeOrgan          |    35,879.3 ns |  1,837.45 ns |    961.02 ns |  2.76 |    0.11 |    3 |         - |          NA |
| PancakeSort         | 256  | PipeOrgan          |    18,133.3 ns |     93.35 ns |     33.29 ns |  1.40 |    0.05 |    2 |         - |          NA |
| BingoSort           | 256  | PipeOrgan          |    14,744.2 ns |    790.30 ns |    350.90 ns |  1.14 |    0.05 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **ManyDuplicates**     |    **14,243.7 ns** |    **461.51 ns** |    **241.38 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | ManyDuplicates     |    11,411.5 ns |    404.63 ns |    179.66 ns |  0.80 |    0.02 |    2 |         - |          NA |
| CycleSort           | 256  | ManyDuplicates     |    38,013.8 ns |  3,256.29 ns |  1,703.11 ns |  2.67 |    0.12 |    5 |         - |          NA |
| PancakeSort         | 256  | ManyDuplicates     |    22,504.2 ns |    280.06 ns |     99.87 ns |  1.58 |    0.03 |    4 |         - |          NA |
| BingoSort           | 256  | ManyDuplicates     |     2,014.3 ns |     44.74 ns |     15.95 ns |  0.14 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Random**             |   **178,689.8 ns** |  **4,303.28 ns** |  **2,250.70 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Random             |   163,841.2 ns | 15,492.34 ns |  8,102.79 ns |  0.92 |    0.04 |    1 |         - |          NA |
| CycleSort           | 1024 | Random             | 1,072,515.5 ns | 20,590.90 ns | 10,769.44 ns |  6.00 |    0.09 |    3 |         - |          NA |
| PancakeSort         | 1024 | Random             |   333,499.2 ns |  2,764.27 ns |  1,445.77 ns |  1.87 |    0.02 |    2 |         - |          NA |
| BingoSort           | 1024 | Random             |   197,287.9 ns |  8,996.83 ns |  3,994.65 ns |  1.10 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **SingleElementMoved** |   **144,931.9 ns** | **10,347.87 ns** |  **4,594.52 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | SingleElementMoved |   142,933.6 ns |  5,106.89 ns |  2,671.00 ns |  0.99 |    0.03 |    1 |         - |          NA |
| CycleSort           | 1024 | SingleElementMoved |   355,146.6 ns |  6,025.32 ns |  2,675.28 ns |  2.45 |    0.07 |    2 |         - |          NA |
| PancakeSort         | 1024 | SingleElementMoved |   163,680.1 ns | 12,818.53 ns |  6,704.34 ns |  1.13 |    0.06 |    1 |         - |          NA |
| BingoSort           | 1024 | SingleElementMoved |   153,079.9 ns | 11,681.31 ns |  6,109.55 ns |  1.06 |    0.05 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Sorted**             |   **138,048.6 ns** |    **512.63 ns** |    **182.81 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Sorted             |   128,088.9 ns |  1,393.93 ns |    729.05 ns |  0.93 |    0.01 |    1 |         - |          NA |
| CycleSort           | 1024 | Sorted             |   143,680.2 ns |  6,938.63 ns |  3,629.04 ns |  1.04 |    0.02 |    1 |         - |          NA |
| PancakeSort         | 1024 | Sorted             |   130,636.4 ns |  5,855.48 ns |  2,599.87 ns |  0.95 |    0.02 |    1 |         - |          NA |
| BingoSort           | 1024 | Sorted             |   142,374.3 ns | 12,493.14 ns |  5,547.03 ns |  1.03 |    0.04 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Reversed**           |   **152,522.5 ns** |  **9,060.23 ns** |  **4,738.67 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Reversed           |   138,654.5 ns | 11,183.41 ns |  5,849.14 ns |  0.91 |    0.04 |    1 |         - |          NA |
| CycleSort           | 1024 | Reversed           |   333,669.5 ns |  6,992.77 ns |  3,657.35 ns |  2.19 |    0.07 |    2 |         - |          NA |
| PancakeSort         | 1024 | Reversed           |   141,425.0 ns | 14,210.40 ns |  7,432.31 ns |  0.93 |    0.05 |    1 |         - |          NA |
| BingoSort           | 1024 | Reversed           |   172,245.7 ns |  6,864.56 ns |  3,047.91 ns |  1.13 |    0.04 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **PipeOrgan**          |   **165,170.2 ns** |  **9,744.59 ns** |  **4,326.66 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | PipeOrgan          |   154,022.6 ns |  6,618.85 ns |  3,461.78 ns |  0.93 |    0.03 |    1 |         - |          NA |
| CycleSort           | 1024 | PipeOrgan          |   573,176.5 ns | 15,493.56 ns |  8,103.43 ns |  3.47 |    0.10 |    3 |         - |          NA |
| PancakeSort         | 1024 | PipeOrgan          |   241,894.9 ns |  6,463.30 ns |  3,380.43 ns |  1.47 |    0.04 |    2 |         - |          NA |
| BingoSort           | 1024 | PipeOrgan          |   196,627.5 ns | 11,914.90 ns |  6,231.72 ns |  1.19 |    0.05 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **ManyDuplicates**     |   **165,235.9 ns** |  **3,819.79 ns** |  **1,696.01 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | ManyDuplicates     |   160,984.1 ns | 10,156.53 ns |  5,312.06 ns |  0.97 |    0.03 |    2 |         - |          NA |
| CycleSort           | 1024 | ManyDuplicates     | 1,019,169.5 ns | 22,240.60 ns |  9,874.97 ns |  6.17 |    0.08 |    4 |         - |          NA |
| PancakeSort         | 1024 | ManyDuplicates     |   298,177.9 ns |  5,804.02 ns |  2,577.02 ns |  1.80 |    0.02 |    3 |         - |          NA |
| BingoSort           | 1024 | ManyDuplicates     |    10,603.9 ns |  2,183.20 ns |    969.35 ns |  0.06 |    0.01 |    1 |         - |          NA |

### TreeBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V45 4.27GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.48 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                 | Size | Pattern            | Mean         | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------- |----- |------------------- |-------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **BalancedBinaryTreeSort** | **256**  | **Random**             |   **4,600.0 ns** |    **352.64 ns** |    **184.44 ns** |  **2.92** |    **0.11** |    **2** |         **-** |          **NA** |
| BTreeSort              | 256  | Random             |   5,235.5 ns |    433.37 ns |    226.66 ns |  3.33 |    0.14 |    2 |         - |          NA |
| BPlusTreeSort          | 256  | Random             |   4,231.8 ns |    310.99 ns |    162.65 ns |  2.69 |    0.10 |    2 |         - |          NA |
| CartesianTreeSort      | 256  | Random             |   3,789.6 ns |    194.35 ns |    101.65 ns |  2.41 |    0.06 |    2 |         - |          NA |
| BinaryTreeSort         | 256  | Random             |   1,573.4 ns |      9.46 ns |      3.37 ns |  1.00 |    0.00 |    1 |         - |          NA |
| SplaySort              | 256  | Random             |  13,357.9 ns |    431.70 ns |    191.68 ns |  8.49 |    0.12 |    3 |         - |          NA |
| TreapSort              | 256  | Random             |   4,291.8 ns |    429.12 ns |    224.44 ns |  2.73 |    0.13 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **SingleElementMoved** |   **4,940.3 ns** |     **96.23 ns** |     **42.73 ns** |  **0.14** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 256  | SingleElementMoved |   3,796.8 ns |    288.85 ns |    151.07 ns |  0.11 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 256  | SingleElementMoved |   3,295.4 ns |    177.98 ns |     79.02 ns |  0.09 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 256  | SingleElementMoved |   1,128.9 ns |      9.08 ns |      4.03 ns |  0.03 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | SingleElementMoved |  35,233.3 ns |    323.96 ns |    169.44 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 256  | SingleElementMoved |   2,587.7 ns |     26.88 ns |     11.94 ns |  0.07 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | SingleElementMoved |   3,223.1 ns |    314.55 ns |    164.52 ns |  0.09 |    0.00 |    3 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Sorted**             |   **4,820.7 ns** |    **279.81 ns** |    **146.34 ns** |  **0.09** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 256  | Sorted             |   3,484.0 ns |    192.26 ns |    100.55 ns |  0.06 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 256  | Sorted             |   3,015.1 ns |    114.75 ns |     50.95 ns |  0.05 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 256  | Sorted             |   1,066.3 ns |      6.88 ns |      3.06 ns |  0.02 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | Sorted             |  55,954.4 ns |    625.45 ns |    327.12 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 256  | Sorted             |   2,391.9 ns |     24.26 ns |     10.77 ns |  0.04 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | Sorted             |   2,923.8 ns |    128.04 ns |     66.97 ns |  0.05 |    0.00 |    3 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Reversed**           |   **5,566.4 ns** |    **371.73 ns** |    **194.42 ns** |  **0.10** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 256  | Reversed           |   5,708.7 ns |    251.86 ns |    131.73 ns |  0.10 |    0.00 |    4 |         - |          NA |
| BPlusTreeSort          | 256  | Reversed           |   5,229.5 ns |    275.56 ns |    144.12 ns |  0.09 |    0.00 |    4 |         - |          NA |
| CartesianTreeSort      | 256  | Reversed           |     993.8 ns |     29.79 ns |     10.62 ns |  0.02 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | Reversed           |  55,696.3 ns |    431.43 ns |    225.64 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 256  | Reversed           |   2,342.3 ns |    236.07 ns |    123.47 ns |  0.04 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | Reversed           |   2,963.2 ns |     56.05 ns |     29.32 ns |  0.05 |    0.00 |    3 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **PipeOrgan**          |   **4,138.5 ns** |    **352.60 ns** |    **184.42 ns** |  **0.15** |    **0.01** |    **3** |         **-** |          **NA** |
| BTreeSort              | 256  | PipeOrgan          |   3,968.2 ns |    137.25 ns |     48.94 ns |  0.15 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 256  | PipeOrgan          |   4,213.3 ns |    121.84 ns |     54.10 ns |  0.15 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 256  | PipeOrgan          |   1,265.3 ns |    290.39 ns |    128.94 ns |  0.05 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | PipeOrgan          |  27,284.6 ns |    203.54 ns |    106.46 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 256  | PipeOrgan          |   2,571.6 ns |     79.40 ns |     41.53 ns |  0.09 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | PipeOrgan          |   3,749.0 ns |    304.75 ns |    159.39 ns |  0.14 |    0.01 |    3 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **ManyDuplicates**     |   **4,764.4 ns** |    **307.82 ns** |    **161.00 ns** |  **1.78** |    **0.06** |    **2** |         **-** |          **NA** |
| BTreeSort              | 256  | ManyDuplicates     |   4,570.2 ns |    240.87 ns |    125.98 ns |  1.71 |    0.05 |    2 |         - |          NA |
| BPlusTreeSort          | 256  | ManyDuplicates     |   3,979.3 ns |    421.95 ns |    220.69 ns |  1.49 |    0.08 |    2 |         - |          NA |
| CartesianTreeSort      | 256  | ManyDuplicates     |   3,952.7 ns |    287.46 ns |    150.35 ns |  1.48 |    0.05 |    2 |         - |          NA |
| BinaryTreeSort         | 256  | ManyDuplicates     |   2,671.2 ns |     39.73 ns |     17.64 ns |  1.00 |    0.00 |    1 |         - |          NA |
| SplaySort              | 256  | ManyDuplicates     |  13,196.2 ns |    417.11 ns |    185.20 ns |  4.94 |    0.07 |    3 |         - |          NA |
| TreapSort              | 256  | ManyDuplicates     |   3,988.6 ns |    276.12 ns |    144.42 ns |  1.49 |    0.05 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Random**             |  **20,097.4 ns** |    **208.56 ns** |     **92.60 ns** |  **1.76** |    **0.04** |    **2** |         **-** |          **NA** |
| BTreeSort              | 1024 | Random             |  22,430.1 ns |    876.19 ns |    389.04 ns |  1.97 |    0.06 |    2 |         - |          NA |
| BPlusTreeSort          | 1024 | Random             |  18,947.1 ns |    776.34 ns |    406.04 ns |  1.66 |    0.05 |    2 |         - |          NA |
| CartesianTreeSort      | 1024 | Random             |  18,206.6 ns |  1,510.67 ns |    790.11 ns |  1.60 |    0.08 |    2 |         - |          NA |
| BinaryTreeSort         | 1024 | Random             |  11,402.4 ns |    564.44 ns |    295.21 ns |  1.00 |    0.00 |    1 |         - |          NA |
| SplaySort              | 1024 | Random             |  70,156.9 ns |  2,880.22 ns |  1,506.41 ns |  6.16 |    0.20 |    3 |         - |          NA |
| TreapSort              | 1024 | Random             |  17,835.3 ns |    626.82 ns |    278.31 ns |  1.57 |    0.04 |    2 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **SingleElementMoved** |  **23,750.0 ns** |    **562.86 ns** |    **294.39 ns** | **0.042** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 1024 | SingleElementMoved |  18,232.3 ns |    710.07 ns |    371.38 ns | 0.032 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 1024 | SingleElementMoved |  16,299.9 ns |    194.21 ns |     86.23 ns | 0.029 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 1024 | SingleElementMoved |   4,134.1 ns |    104.11 ns |     46.23 ns | 0.007 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | SingleElementMoved | 570,588.8 ns |  1,845.05 ns |    819.21 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | SingleElementMoved |  10,206.1 ns |     22.96 ns |     10.20 ns | 0.018 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | SingleElementMoved |  14,547.1 ns |    379.96 ns |    198.73 ns | 0.025 |    0.00 |    3 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Sorted**             |  **23,335.9 ns** |    **536.56 ns** |    **280.63 ns** | **0.027** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 1024 | Sorted             |  14,984.9 ns |    528.08 ns |    276.19 ns | 0.017 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 1024 | Sorted             |  14,618.7 ns |  1,248.28 ns |    652.87 ns | 0.017 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 1024 | Sorted             |   3,925.3 ns |    140.35 ns |     73.40 ns | 0.005 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | Sorted             | 870,182.2 ns | 35,769.11 ns | 18,707.93 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | Sorted             |   9,447.1 ns |     90.61 ns |     47.39 ns | 0.011 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | Sorted             |  12,575.1 ns |    214.80 ns |     95.37 ns | 0.014 |    0.00 |    3 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Reversed**           |  **23,505.8 ns** |    **893.77 ns** |    **467.46 ns** | **0.027** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 1024 | Reversed           |  25,316.0 ns |    371.16 ns |    194.12 ns | 0.029 |    0.00 |    4 |         - |          NA |
| BPlusTreeSort          | 1024 | Reversed           |  22,215.3 ns |    335.98 ns |    149.18 ns | 0.025 |    0.00 |    4 |         - |          NA |
| CartesianTreeSort      | 1024 | Reversed           |   4,175.3 ns |  1,139.08 ns |    505.76 ns | 0.005 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | Reversed           | 878,271.4 ns | 18,008.94 ns |  7,996.08 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | Reversed           |   9,872.7 ns |    161.04 ns |     84.23 ns | 0.011 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | Reversed           |  14,592.3 ns |    124.65 ns |     65.20 ns | 0.017 |    0.00 |    3 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **PipeOrgan**          |  **19,708.2 ns** |    **774.50 ns** |    **405.08 ns** |  **0.04** |    **0.00** |    **3** |         **-** |          **NA** |
| BTreeSort              | 1024 | PipeOrgan          |  18,000.7 ns |    269.77 ns |    119.78 ns |  0.04 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 1024 | PipeOrgan          |  20,825.1 ns |    894.72 ns |    397.26 ns |  0.05 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 1024 | PipeOrgan          |   4,473.3 ns |    236.25 ns |    123.56 ns |  0.01 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | PipeOrgan          | 443,069.8 ns |  6,083.06 ns |  3,181.56 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | PipeOrgan          |  10,445.8 ns |    115.60 ns |     51.33 ns |  0.02 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | PipeOrgan          |  17,249.4 ns |    380.31 ns |    168.86 ns |  0.04 |    0.00 |    3 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **ManyDuplicates**     |  **22,872.4 ns** |  **1,288.31 ns** |    **673.81 ns** |  **1.00** |    **0.03** |    **1** |         **-** |          **NA** |
| BTreeSort              | 1024 | ManyDuplicates     |  19,163.2 ns |    225.75 ns |    118.07 ns |  0.84 |    0.01 |    1 |         - |          NA |
| BPlusTreeSort          | 1024 | ManyDuplicates     |  17,350.9 ns |    639.31 ns |    334.37 ns |  0.76 |    0.02 |    1 |         - |          NA |
| CartesianTreeSort      | 1024 | ManyDuplicates     |  17,556.7 ns |    400.92 ns |    142.97 ns |  0.77 |    0.01 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | ManyDuplicates     |  22,847.6 ns |    661.08 ns |    345.76 ns |  1.00 |    0.00 |    1 |         - |          NA |
| SplaySort              | 1024 | ManyDuplicates     |  59,897.4 ns |    802.02 ns |    356.10 ns |  2.62 |    0.04 |    2 |         - |          NA |
| TreapSort              | 1024 | ManyDuplicates     |  18,387.5 ns |    576.16 ns |    301.34 ns |  0.80 |    0.02 |    1 |         - |          NA |

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
- [Drop-Merge Sort](./src/SortAlgorithm/Algorithms/Merge/DropMergeSort.cs)
- [Flat Stable Sort](./src/SortAlgorithm/Algorithms/Merge/FlatStableSort.cs)
- [Glidesort](./src/SortAlgorithm/Algorithms/Merge/Glidesort.cs)
- [Merge Sort](./src/SortAlgorithm/Algorithms/Merge/MergeSort.cs)
- [Natural Merge Sort](./src/SortAlgorithm/Algorithms/Merge/NaturalMergeSort.cs)
- [Patience Sort](./src/SortAlgorithm/Algorithms/Merge/PatienceSort.cs)
- [Pingpong Merge Sort](./src/SortAlgorithm/Algorithms/Merge/PingpongMergeSort.cs)
- [Power Sort](./src/SortAlgorithm/Algorithms/Merge/PowerSort.cs)
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
- [Bitonic Sort](./src/SortAlgorithm/Algorithms/Network/BitonicSort.cs)
  - Iterative
  - Recursive
- [Pairwise Sorting Network](./src/SortAlgorithm/Algorithms/Network/PairwiseSortingNetwork.cs)

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
