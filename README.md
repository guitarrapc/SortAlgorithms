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
<summary>Benchmark results (2026-11-17 17:44 UTC)</summary>

Workflow run: https://github.com/guitarrapc/SortAlgorithms/actions/runs/44135744703

### DistributionBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.58 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method              | Size | Pattern            | Mean           | Error        | StdDev      | Ratio  | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------------- |----- |------------------- |---------------:|-------------:|------------:|-------:|--------:|-----:|----------:|------------:|
| **CountingSort**        | **256**  | **Random**             |     **1,305.1 ns** |     **36.49 ns** |    **19.09 ns** |   **1.44** |    **0.03** |    **1** |         **-** |          **NA** |
| CountingSortInteger | 256  | Random             |       909.2 ns |     19.62 ns |    10.26 ns |   1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 256  | Random             |     1,188.3 ns |    102.11 ns |    45.34 ns |   1.31 |    0.05 |    1 |         - |          NA |
| PigeonSortInteger   | 256  | Random             |     1,135.6 ns |     49.23 ns |    25.75 ns |   1.25 |    0.03 |    1 |         - |          NA |
| BucketSort          | 256  | Random             |     1,623.5 ns |     38.01 ns |    13.55 ns |   1.79 |    0.02 |    2 |         - |          NA |
| BucketSortInteger   | 256  | Random             |     2,020.8 ns |    129.87 ns |    57.66 ns |   2.22 |    0.06 |    3 |         - |          NA |
| FlashSort           | 256  | Random             |    11,038.6 ns |    225.19 ns |   117.78 ns |  12.14 |    0.18 |    7 |         - |          NA |
| RadixLSD4Sort       | 256  | Random             |     2,193.9 ns |     28.22 ns |    12.53 ns |   2.41 |    0.03 |    3 |         - |          NA |
| RotateLSDRadixSort  | 256  | Random             |    53,103.1 ns |    452.25 ns |   200.80 ns |  58.42 |    0.66 |    8 |         - |          NA |
| RadixLSD256Sort     | 256  | Random             |     1,034.3 ns |     27.81 ns |    12.35 ns |   1.14 |    0.02 |    1 |         - |          NA |
| Radsort             | 256  | Random             |     7,646.5 ns |    239.67 ns |   125.35 ns |   8.41 |    0.16 |    6 |         - |          NA |
| RadixLSD10Sort      | 256  | Random             |     3,195.9 ns |     38.54 ns |    17.11 ns |   3.52 |    0.04 |    4 |         - |          NA |
| RadixMSD4Sort       | 256  | Random             |     5,144.0 ns |     51.92 ns |    18.51 ns |   5.66 |    0.06 |    5 |         - |          NA |
| RadixMSD10Sort      | 256  | Random             |     4,681.0 ns |     41.15 ns |    14.67 ns |   5.15 |    0.06 |    5 |         - |          NA |
| AmericanFlagSort    | 256  | Random             |     2,306.5 ns |     34.74 ns |    15.43 ns |   2.54 |    0.03 |    3 |         - |          NA |
| SpreadSort          | 256  | Random             |     6,139.9 ns |    188.17 ns |    83.55 ns |   6.75 |    0.11 |    5 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **256**  | **SingleElementMoved** |     **1,253.8 ns** |     **16.14 ns** |     **7.17 ns** |   **1.61** |    **0.01** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | SingleElementMoved |       778.6 ns |      6.91 ns |     3.07 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 256  | SingleElementMoved |     1,123.2 ns |     11.07 ns |     4.91 ns |   1.44 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | SingleElementMoved |       625.8 ns |      8.65 ns |     3.84 ns |   0.80 |    0.01 |    1 |         - |          NA |
| BucketSort          | 256  | SingleElementMoved |     1,569.5 ns |     13.53 ns |     6.01 ns |   2.02 |    0.01 |    3 |         - |          NA |
| BucketSortInteger   | 256  | SingleElementMoved |     1,255.9 ns |    110.37 ns |    49.01 ns |   1.61 |    0.06 |    2 |         - |          NA |
| FlashSort           | 256  | SingleElementMoved |    14,691.1 ns |    110.76 ns |    57.93 ns |  18.87 |    0.10 |    8 |         - |          NA |
| RadixLSD4Sort       | 256  | SingleElementMoved |     2,288.6 ns |    311.20 ns |   162.76 ns |   2.94 |    0.20 |    4 |         - |          NA |
| RotateLSDRadixSort  | 256  | SingleElementMoved |    34,957.6 ns |    534.83 ns |   237.47 ns |  44.90 |    0.33 |    9 |         - |          NA |
| RadixLSD256Sort     | 256  | SingleElementMoved |     1,009.4 ns |     12.74 ns |     6.67 ns |   1.30 |    0.01 |    2 |         - |          NA |
| Radsort             | 256  | SingleElementMoved |     8,153.3 ns |    926.26 ns |   484.45 ns |  10.47 |    0.59 |    7 |         - |          NA |
| RadixLSD10Sort      | 256  | SingleElementMoved |     3,082.3 ns |     30.43 ns |    10.85 ns |   3.96 |    0.02 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | SingleElementMoved |     5,614.4 ns |    311.62 ns |   162.98 ns |   7.21 |    0.20 |    6 |         - |          NA |
| RadixMSD10Sort      | 256  | SingleElementMoved |     4,985.3 ns |    436.99 ns |   228.56 ns |   6.40 |    0.28 |    6 |         - |          NA |
| AmericanFlagSort    | 256  | SingleElementMoved |     1,601.8 ns |     34.87 ns |    18.24 ns |   2.06 |    0.02 |    3 |         - |          NA |
| SpreadSort          | 256  | SingleElementMoved |       913.2 ns |     59.80 ns |    26.55 ns |   1.17 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **256**  | **Sorted**             |     **1,227.9 ns** |     **72.91 ns** |    **32.37 ns** |   **1.65** |    **0.04** |    **5** |         **-** |          **NA** |
| CountingSortInteger | 256  | Sorted             |       743.0 ns |      5.80 ns |     2.57 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 256  | Sorted             |     1,125.7 ns |     43.83 ns |    22.93 ns |   1.52 |    0.03 |    5 |         - |          NA |
| PigeonSortInteger   | 256  | Sorted             |       539.3 ns |      5.05 ns |     2.24 ns |   0.73 |    0.00 |    2 |         - |          NA |
| BucketSort          | 256  | Sorted             |     1,481.8 ns |      7.98 ns |     3.54 ns |   1.99 |    0.01 |    6 |         - |          NA |
| BucketSortInteger   | 256  | Sorted             |     1,130.2 ns |     10.86 ns |     4.82 ns |   1.52 |    0.01 |    5 |         - |          NA |
| FlashSort           | 256  | Sorted             |    14,932.4 ns |    132.37 ns |    69.23 ns |  20.10 |    0.11 |   10 |         - |          NA |
| RadixLSD4Sort       | 256  | Sorted             |     2,142.1 ns |     39.03 ns |    17.33 ns |   2.88 |    0.02 |    7 |         - |          NA |
| RotateLSDRadixSort  | 256  | Sorted             |    34,140.8 ns |     93.14 ns |    41.35 ns |  45.95 |    0.16 |   11 |         - |          NA |
| RadixLSD256Sort     | 256  | Sorted             |       930.6 ns |     26.69 ns |     9.52 ns |   1.25 |    0.01 |    4 |         - |          NA |
| Radsort             | 256  | Sorted             |     7,500.7 ns |    120.56 ns |    53.53 ns |  10.09 |    0.07 |    9 |         - |          NA |
| RadixLSD10Sort      | 256  | Sorted             |     4,402.5 ns |    394.68 ns |   206.43 ns |   5.93 |    0.26 |    8 |         - |          NA |
| RadixMSD4Sort       | 256  | Sorted             |     5,402.5 ns |    202.40 ns |    89.87 ns |   7.27 |    0.12 |    8 |         - |          NA |
| RadixMSD10Sort      | 256  | Sorted             |     4,643.5 ns |    181.79 ns |    80.72 ns |   6.25 |    0.10 |    8 |         - |          NA |
| AmericanFlagSort    | 256  | Sorted             |     1,229.3 ns |     85.03 ns |    37.75 ns |   1.65 |    0.05 |    5 |         - |          NA |
| SpreadSort          | 256  | Sorted             |       296.9 ns |      5.52 ns |     2.89 ns |   0.40 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **256**  | **Reversed**           |     **1,183.0 ns** |     **10.68 ns** |     **4.74 ns** |   **1.51** |    **0.03** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 256  | Reversed           |       781.8 ns |     45.12 ns |    16.09 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 256  | Reversed           |     1,160.9 ns |     11.05 ns |     4.91 ns |   1.49 |    0.03 |    4 |         - |          NA |
| PigeonSortInteger   | 256  | Reversed           |       560.4 ns |      3.10 ns |     1.62 ns |   0.72 |    0.01 |    2 |         - |          NA |
| BucketSort          | 256  | Reversed           |     1,578.9 ns |     16.39 ns |     7.28 ns |   2.02 |    0.04 |    4 |         - |          NA |
| BucketSortInteger   | 256  | Reversed           |     1,152.3 ns |     16.98 ns |     7.54 ns |   1.47 |    0.03 |    4 |         - |          NA |
| FlashSort           | 256  | Reversed           |    11,968.0 ns |    245.90 ns |   128.61 ns |  15.31 |    0.33 |   10 |         - |          NA |
| RadixLSD4Sort       | 256  | Reversed           |     2,125.6 ns |     60.21 ns |    21.47 ns |   2.72 |    0.06 |    5 |         - |          NA |
| RotateLSDRadixSort  | 256  | Reversed           |    34,928.7 ns |    278.82 ns |   145.83 ns |  44.69 |    0.86 |   11 |         - |          NA |
| RadixLSD256Sort     | 256  | Reversed           |       901.5 ns |      7.50 ns |     3.33 ns |   1.15 |    0.02 |    3 |         - |          NA |
| Radsort             | 256  | Reversed           |     7,453.6 ns |    118.50 ns |    61.98 ns |   9.54 |    0.19 |    9 |         - |          NA |
| RadixLSD10Sort      | 256  | Reversed           |     3,070.4 ns |     49.97 ns |    17.82 ns |   3.93 |    0.08 |    6 |         - |          NA |
| RadixMSD4Sort       | 256  | Reversed           |     5,596.2 ns |    268.90 ns |   119.39 ns |   7.16 |    0.20 |    8 |         - |          NA |
| RadixMSD10Sort      | 256  | Reversed           |     4,578.7 ns |     32.59 ns |    11.62 ns |   5.86 |    0.11 |    7 |         - |          NA |
| AmericanFlagSort    | 256  | Reversed           |     1,395.5 ns |     48.30 ns |    25.26 ns |   1.79 |    0.05 |    4 |         - |          NA |
| SpreadSort          | 256  | Reversed           |       427.5 ns |     18.65 ns |     8.28 ns |   0.55 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **256**  | **PipeOrgan**          |     **1,227.7 ns** |    **108.63 ns** |    **48.23 ns** |   **1.49** |    **0.06** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | PipeOrgan          |       825.9 ns |     46.31 ns |    20.56 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 256  | PipeOrgan          |     1,128.5 ns |     14.33 ns |     6.36 ns |   1.37 |    0.03 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | PipeOrgan          |       654.6 ns |     34.47 ns |    15.30 ns |   0.79 |    0.03 |    1 |         - |          NA |
| BucketSort          | 256  | PipeOrgan          |     1,657.7 ns |    265.81 ns |   139.03 ns |   2.01 |    0.17 |    3 |         - |          NA |
| BucketSortInteger   | 256  | PipeOrgan          |     1,232.0 ns |     37.68 ns |    16.73 ns |   1.49 |    0.04 |    2 |         - |          NA |
| FlashSort           | 256  | PipeOrgan          |    11,769.6 ns |    176.10 ns |    92.10 ns |  14.26 |    0.34 |    8 |         - |          NA |
| RadixLSD4Sort       | 256  | PipeOrgan          |     2,146.4 ns |      7.32 ns |     2.61 ns |   2.60 |    0.06 |    4 |         - |          NA |
| RotateLSDRadixSort  | 256  | PipeOrgan          |    34,589.4 ns |    176.88 ns |    92.51 ns |  41.90 |    0.96 |    9 |         - |          NA |
| RadixLSD256Sort     | 256  | PipeOrgan          |       967.9 ns |      2.64 ns |     1.17 ns |   1.17 |    0.03 |    2 |         - |          NA |
| Radsort             | 256  | PipeOrgan          |     7,302.5 ns |     43.10 ns |    19.14 ns |   8.85 |    0.20 |    7 |         - |          NA |
| RadixLSD10Sort      | 256  | PipeOrgan          |     3,136.4 ns |     19.79 ns |     7.06 ns |   3.80 |    0.09 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | PipeOrgan          |     5,495.5 ns |     38.17 ns |    16.95 ns |   6.66 |    0.15 |    6 |         - |          NA |
| RadixMSD10Sort      | 256  | PipeOrgan          |     4,687.4 ns |     60.64 ns |    26.92 ns |   5.68 |    0.13 |    6 |         - |          NA |
| AmericanFlagSort    | 256  | PipeOrgan          |     1,760.8 ns |     71.35 ns |    31.68 ns |   2.13 |    0.06 |    3 |         - |          NA |
| SpreadSort          | 256  | PipeOrgan          |     1,326.7 ns |     12.50 ns |     4.46 ns |   1.61 |    0.04 |    2 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **256**  | **ManyDuplicates**     |     **1,233.0 ns** |     **34.44 ns** |    **15.29 ns** |   **1.14** |    **0.19** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | ManyDuplicates     |     1,105.5 ns |    343.38 ns |   179.60 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 256  | ManyDuplicates     |     1,372.4 ns |    327.60 ns |   171.34 ns |   1.27 |    0.26 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | ManyDuplicates     |       766.3 ns |    142.82 ns |    63.41 ns |   0.71 |    0.13 |    1 |         - |          NA |
| BucketSort          | 256  | ManyDuplicates     |     2,374.4 ns |     99.37 ns |    44.12 ns |   2.20 |    0.37 |    4 |         - |          NA |
| BucketSortInteger   | 256  | ManyDuplicates     |     1,495.4 ns |     88.50 ns |    39.29 ns |   1.39 |    0.23 |    2 |         - |          NA |
| FlashSort           | 256  | ManyDuplicates     |    10,892.2 ns |    199.73 ns |   104.46 ns |  10.11 |    1.69 |    7 |         - |          NA |
| RadixLSD4Sort       | 256  | ManyDuplicates     |     1,791.4 ns |     18.53 ns |     8.23 ns |   1.66 |    0.28 |    3 |         - |          NA |
| RotateLSDRadixSort  | 256  | ManyDuplicates     |    46,360.3 ns |    296.07 ns |   131.46 ns |  43.01 |    7.19 |    8 |         - |          NA |
| RadixLSD256Sort     | 256  | ManyDuplicates     |     1,072.0 ns |     29.96 ns |    15.67 ns |   0.99 |    0.17 |    2 |         - |          NA |
| Radsort             | 256  | ManyDuplicates     |     8,973.9 ns |    285.20 ns |   149.16 ns |   8.33 |    1.40 |    6 |         - |          NA |
| RadixLSD10Sort      | 256  | ManyDuplicates     |     2,228.9 ns |     28.24 ns |    14.77 ns |   2.07 |    0.35 |    4 |         - |          NA |
| RadixMSD4Sort       | 256  | ManyDuplicates     |     2,639.5 ns |     32.37 ns |    14.37 ns |   2.45 |    0.41 |    4 |         - |          NA |
| RadixMSD10Sort      | 256  | ManyDuplicates     |     2,942.5 ns |     29.04 ns |    15.19 ns |   2.73 |    0.46 |    4 |         - |          NA |
| AmericanFlagSort    | 256  | ManyDuplicates     |     2,565.5 ns |     56.83 ns |    20.27 ns |   2.38 |    0.40 |    4 |         - |          NA |
| SpreadSort          | 256  | ManyDuplicates     |     5,750.8 ns |    168.16 ns |    87.95 ns |   5.34 |    0.89 |    5 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **1024** | **Random**             |     **4,824.8 ns** |    **302.96 ns** |   **134.52 ns** |   **1.46** |    **0.05** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Random             |     3,313.0 ns |    149.05 ns |    77.95 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | Random             |     3,919.8 ns |     29.54 ns |    10.53 ns |   1.18 |    0.03 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | Random             |     2,528.3 ns |     35.42 ns |    15.73 ns |   0.76 |    0.02 |    1 |         - |          NA |
| BucketSort          | 1024 | Random             |     5,890.3 ns |     24.55 ns |     8.75 ns |   1.78 |    0.04 |    2 |         - |          NA |
| BucketSortInteger   | 1024 | Random             |     4,579.9 ns |     73.16 ns |    32.48 ns |   1.38 |    0.03 |    2 |         - |          NA |
| FlashSort           | 1024 | Random             |    44,820.6 ns |    488.23 ns |   255.35 ns |  13.54 |    0.30 |    8 |         - |          NA |
| RadixLSD4Sort       | 1024 | Random             |    10,368.8 ns |    374.03 ns |   195.62 ns |   3.13 |    0.09 |    4 |         - |          NA |
| RotateLSDRadixSort  | 1024 | Random             |   280,767.5 ns |  1,681.13 ns |   879.27 ns |  84.79 |    1.86 |    9 |         - |          NA |
| RadixLSD256Sort     | 1024 | Random             |     5,382.5 ns |    203.62 ns |    90.41 ns |   1.63 |    0.04 |    2 |         - |          NA |
| Radsort             | 1024 | Random             |    29,702.8 ns |    222.94 ns |    98.99 ns |   8.97 |    0.20 |    7 |         - |          NA |
| RadixLSD10Sort      | 1024 | Random             |    16,192.4 ns |    160.73 ns |    84.06 ns |   4.89 |    0.11 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | Random             |    22,643.8 ns |    161.88 ns |    84.67 ns |   6.84 |    0.15 |    6 |         - |          NA |
| RadixMSD10Sort      | 1024 | Random             |    23,275.8 ns |    135.82 ns |    71.04 ns |   7.03 |    0.15 |    6 |         - |          NA |
| AmericanFlagSort    | 1024 | Random             |    16,101.4 ns |    122.89 ns |    64.27 ns |   4.86 |    0.11 |    5 |         - |          NA |
| SpreadSort          | 1024 | Random             |     7,433.4 ns |    278.13 ns |   145.47 ns |   2.24 |    0.06 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **1024** | **SingleElementMoved** |     **4,624.0 ns** |     **23.58 ns** |     **8.41 ns** |   **1.56** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | SingleElementMoved |     2,955.9 ns |     27.89 ns |    14.59 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | SingleElementMoved |     3,852.8 ns |     19.22 ns |     8.54 ns |   1.30 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | SingleElementMoved |     2,437.3 ns |      6.53 ns |     2.90 ns |   0.82 |    0.00 |    1 |         - |          NA |
| BucketSort          | 1024 | SingleElementMoved |     5,934.4 ns |    246.08 ns |   128.71 ns |   2.01 |    0.04 |    3 |         - |          NA |
| BucketSortInteger   | 1024 | SingleElementMoved |     4,473.1 ns |     44.93 ns |    23.50 ns |   1.51 |    0.01 |    3 |         - |          NA |
| FlashSort           | 1024 | SingleElementMoved |    57,057.3 ns |    418.24 ns |   185.70 ns |  19.30 |    0.11 |    6 |         - |          NA |
| RadixLSD4Sort       | 1024 | SingleElementMoved |    16,890.6 ns |    142.59 ns |    74.58 ns |   5.71 |    0.04 |    5 |         - |          NA |
| RotateLSDRadixSort  | 1024 | SingleElementMoved |   176,534.8 ns |    709.22 ns |   314.90 ns |  59.72 |    0.29 |    7 |         - |          NA |
| RadixLSD256Sort     | 1024 | SingleElementMoved |     5,211.9 ns |     42.44 ns |    18.84 ns |   1.76 |    0.01 |    3 |         - |          NA |
| Radsort             | 1024 | SingleElementMoved |    19,492.8 ns |    196.41 ns |   102.73 ns |   6.59 |    0.04 |    5 |         - |          NA |
| RadixLSD10Sort      | 1024 | SingleElementMoved |    15,949.1 ns |     84.42 ns |    44.15 ns |   5.40 |    0.03 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | SingleElementMoved |    24,185.7 ns |    225.29 ns |   100.03 ns |   8.18 |    0.05 |    5 |         - |          NA |
| RadixMSD10Sort      | 1024 | SingleElementMoved |    22,301.9 ns |    257.29 ns |   114.24 ns |   7.55 |    0.05 |    5 |         - |          NA |
| AmericanFlagSort    | 1024 | SingleElementMoved |     8,732.0 ns |    284.91 ns |   149.01 ns |   2.95 |    0.05 |    4 |         - |          NA |
| SpreadSort          | 1024 | SingleElementMoved |     5,689.2 ns |    176.11 ns |    92.11 ns |   1.92 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **1024** | **Sorted**             |     **4,451.2 ns** |    **225.69 ns** |   **118.04 ns** |   **1.49** |    **0.09** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Sorted             |     2,988.0 ns |    332.69 ns |   174.00 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 1024 | Sorted             |     3,866.3 ns |      9.41 ns |     4.92 ns |   1.30 |    0.07 |    4 |         - |          NA |
| PigeonSortInteger   | 1024 | Sorted             |     2,236.1 ns |     12.68 ns |     5.63 ns |   0.75 |    0.04 |    2 |         - |          NA |
| BucketSort          | 1024 | Sorted             |     5,606.1 ns |      9.28 ns |     3.31 ns |   1.88 |    0.10 |    5 |         - |          NA |
| BucketSortInteger   | 1024 | Sorted             |     4,198.1 ns |     50.72 ns |    22.52 ns |   1.41 |    0.08 |    4 |         - |          NA |
| FlashSort           | 1024 | Sorted             |    57,723.8 ns |    251.62 ns |   111.72 ns |  19.37 |    1.03 |    8 |         - |          NA |
| RadixLSD4Sort       | 1024 | Sorted             |    16,240.9 ns |    296.87 ns |   155.27 ns |   5.45 |    0.29 |    7 |         - |          NA |
| RotateLSDRadixSort  | 1024 | Sorted             |   174,561.6 ns |    739.86 ns |   328.50 ns |  58.59 |    3.11 |    9 |         - |          NA |
| RadixLSD256Sort     | 1024 | Sorted             |     5,544.0 ns |     31.08 ns |    13.80 ns |   1.86 |    0.10 |    5 |         - |          NA |
| Radsort             | 1024 | Sorted             |    18,877.5 ns |     98.13 ns |    43.57 ns |   6.34 |    0.34 |    7 |         - |          NA |
| RadixLSD10Sort      | 1024 | Sorted             |    15,848.2 ns |    127.46 ns |    66.66 ns |   5.32 |    0.28 |    7 |         - |          NA |
| RadixMSD4Sort       | 1024 | Sorted             |    24,703.7 ns |    213.12 ns |   111.46 ns |   8.29 |    0.44 |    7 |         - |          NA |
| RadixMSD10Sort      | 1024 | Sorted             |    22,092.0 ns |    177.88 ns |    93.04 ns |   7.41 |    0.39 |    7 |         - |          NA |
| AmericanFlagSort    | 1024 | Sorted             |     7,403.8 ns |    264.16 ns |   138.16 ns |   2.48 |    0.14 |    6 |         - |          NA |
| SpreadSort          | 1024 | Sorted             |       487.7 ns |      6.17 ns |     2.74 ns |   0.16 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **1024** | **Reversed**           |     **4,339.7 ns** |     **45.80 ns** |    **20.34 ns** |   **1.48** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Reversed           |     2,940.2 ns |     43.46 ns |    19.30 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | Reversed           |     4,290.0 ns |    294.34 ns |   153.95 ns |   1.46 |    0.05 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | Reversed           |     2,251.6 ns |    264.38 ns |   117.38 ns |   0.77 |    0.04 |    1 |         - |          NA |
| BucketSort          | 1024 | Reversed           |     5,859.8 ns |    102.46 ns |    45.49 ns |   1.99 |    0.02 |    4 |         - |          NA |
| BucketSortInteger   | 1024 | Reversed           |     5,031.1 ns |  1,557.69 ns |   814.70 ns |   1.71 |    0.26 |    3 |         - |          NA |
| FlashSort           | 1024 | Reversed           |    47,406.8 ns |    266.32 ns |   139.29 ns |  16.12 |    0.11 |    8 |         - |          NA |
| RadixLSD4Sort       | 1024 | Reversed           |    12,885.7 ns |    351.54 ns |   183.86 ns |   4.38 |    0.06 |    6 |         - |          NA |
| RotateLSDRadixSort  | 1024 | Reversed           |   178,884.4 ns |  1,920.16 ns |   852.56 ns |  60.84 |    0.46 |    9 |         - |          NA |
| RadixLSD256Sort     | 1024 | Reversed           |     4,890.5 ns |     43.66 ns |    22.83 ns |   1.66 |    0.01 |    3 |         - |          NA |
| Radsort             | 1024 | Reversed           |    18,822.8 ns |    229.80 ns |   102.03 ns |   6.40 |    0.05 |    7 |         - |          NA |
| RadixLSD10Sort      | 1024 | Reversed           |    15,807.6 ns |    175.10 ns |    77.75 ns |   5.38 |    0.04 |    7 |         - |          NA |
| RadixMSD4Sort       | 1024 | Reversed           |    23,917.8 ns |    294.69 ns |   154.13 ns |   8.13 |    0.07 |    7 |         - |          NA |
| RadixMSD10Sort      | 1024 | Reversed           |    22,173.1 ns |    277.71 ns |   123.30 ns |   7.54 |    0.06 |    7 |         - |          NA |
| AmericanFlagSort    | 1024 | Reversed           |     8,750.8 ns |    234.29 ns |   122.54 ns |   2.98 |    0.04 |    5 |         - |          NA |
| SpreadSort          | 1024 | Reversed           |     4,459.5 ns |    293.61 ns |   153.56 ns |   1.52 |    0.05 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **1024** | **PipeOrgan**          |     **4,428.0 ns** |      **6.90 ns** |     **3.06 ns** |   **1.47** |    **0.00** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | PipeOrgan          |     3,019.1 ns |     16.54 ns |     7.34 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | PipeOrgan          |     3,861.2 ns |      8.77 ns |     4.58 ns |   1.28 |    0.00 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | PipeOrgan          |     2,447.2 ns |     14.10 ns |     6.26 ns |   0.81 |    0.00 |    1 |         - |          NA |
| BucketSort          | 1024 | PipeOrgan          |     5,732.3 ns |     11.09 ns |     4.92 ns |   1.90 |    0.00 |    3 |         - |          NA |
| BucketSortInteger   | 1024 | PipeOrgan          |     4,505.2 ns |     50.27 ns |    22.32 ns |   1.49 |    0.01 |    3 |         - |          NA |
| FlashSort           | 1024 | PipeOrgan          |    45,955.0 ns |    260.05 ns |   136.01 ns |  15.22 |    0.05 |    6 |         - |          NA |
| RadixLSD4Sort       | 1024 | PipeOrgan          |    11,155.2 ns |    319.95 ns |   167.34 ns |   3.69 |    0.05 |    4 |         - |          NA |
| RotateLSDRadixSort  | 1024 | PipeOrgan          |   176,139.9 ns |  1,512.33 ns |   671.49 ns |  58.34 |    0.25 |    7 |         - |          NA |
| RadixLSD256Sort     | 1024 | PipeOrgan          |     5,333.8 ns |    300.35 ns |   157.09 ns |   1.77 |    0.05 |    3 |         - |          NA |
| Radsort             | 1024 | PipeOrgan          |    19,116.6 ns |    274.80 ns |   122.01 ns |   6.33 |    0.04 |    5 |         - |          NA |
| RadixLSD10Sort      | 1024 | PipeOrgan          |    16,063.9 ns |    194.84 ns |   101.90 ns |   5.32 |    0.03 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | PipeOrgan          |    24,595.5 ns |    215.07 ns |   112.49 ns |   8.15 |    0.04 |    5 |         - |          NA |
| RadixMSD10Sort      | 1024 | PipeOrgan          |    22,556.1 ns |    338.86 ns |   150.46 ns |   7.47 |    0.05 |    5 |         - |          NA |
| AmericanFlagSort    | 1024 | PipeOrgan          |    11,398.7 ns |    287.04 ns |   150.13 ns |   3.78 |    0.05 |    4 |         - |          NA |
| SpreadSort          | 1024 | PipeOrgan          |     6,232.5 ns |    277.18 ns |   144.97 ns |   2.06 |    0.05 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **1024** | **ManyDuplicates**     |     **4,255.4 ns** |     **13.69 ns** |     **6.08 ns** |   **1.50** |    **0.01** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | ManyDuplicates     |     2,840.6 ns |     57.78 ns |    20.60 ns |   1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 1024 | ManyDuplicates     |     4,780.6 ns |     43.84 ns |    22.93 ns |   1.68 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | ManyDuplicates     |     2,366.5 ns |     39.41 ns |    14.05 ns |   0.83 |    0.01 |    1 |         - |          NA |
| BucketSort          | 1024 | ManyDuplicates     |     8,439.4 ns |    236.03 ns |   123.45 ns |   2.97 |    0.05 |    3 |         - |          NA |
| BucketSortInteger   | 1024 | ManyDuplicates     |     4,809.4 ns |     31.42 ns |    13.95 ns |   1.69 |    0.01 |    2 |         - |          NA |
| FlashSort           | 1024 | ManyDuplicates     |    41,634.5 ns |    167.02 ns |    87.35 ns |  14.66 |    0.10 |    5 |         - |          NA |
| RadixLSD4Sort       | 1024 | ManyDuplicates     |     6,793.7 ns |    251.06 ns |   131.31 ns |   2.39 |    0.05 |    3 |         - |          NA |
| RotateLSDRadixSort  | 1024 | ManyDuplicates     |   190,636.8 ns |  1,525.45 ns |   797.84 ns |  67.12 |    0.52 |    6 |         - |          NA |
| RadixLSD256Sort     | 1024 | ManyDuplicates     |     3,284.9 ns |     53.84 ns |    23.91 ns |   1.16 |    0.01 |    1 |         - |          NA |
| Radsort             | 1024 | ManyDuplicates     |    19,932.0 ns |     93.10 ns |    41.34 ns |   7.02 |    0.05 |    4 |         - |          NA |
| RadixLSD10Sort      | 1024 | ManyDuplicates     |     8,747.3 ns |    352.24 ns |   156.40 ns |   3.08 |    0.06 |    3 |         - |          NA |
| RadixMSD4Sort       | 1024 | ManyDuplicates     |     7,769.4 ns |    270.21 ns |   141.33 ns |   2.74 |    0.05 |    3 |         - |          NA |
| RadixMSD10Sort      | 1024 | ManyDuplicates     |     9,714.2 ns |    268.76 ns |   140.57 ns |   3.42 |    0.05 |    3 |         - |          NA |
| AmericanFlagSort    | 1024 | ManyDuplicates     |     7,236.2 ns |    266.37 ns |   139.31 ns |   2.55 |    0.05 |    3 |         - |          NA |
| SpreadSort          | 1024 | ManyDuplicates     |     6,069.8 ns |    264.97 ns |   138.58 ns |   2.14 |    0.05 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **4096** | **Random**             |    **19,156.9 ns** |    **155.69 ns** |    **69.13 ns** |   **1.41** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Random             |    13,548.7 ns |    212.60 ns |    75.82 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | Random             |    15,341.8 ns |    159.76 ns |    83.56 ns |   1.13 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 4096 | Random             |     9,965.5 ns |    293.06 ns |   153.27 ns |   0.74 |    0.01 |    1 |         - |          NA |
| BucketSort          | 4096 | Random             |    24,715.7 ns |    578.84 ns |   302.75 ns |   1.82 |    0.02 |    4 |         - |          NA |
| BucketSortInteger   | 4096 | Random             |    22,378.5 ns |  7,905.12 ns | 4,134.53 ns |   1.65 |    0.29 |    3 |         - |          NA |
| FlashSort           | 4096 | Random             |   177,423.1 ns |    607.28 ns |   269.64 ns |  13.10 |    0.07 |    8 |         - |          NA |
| RadixLSD4Sort       | 4096 | Random             |    49,328.4 ns |    139.82 ns |    62.08 ns |   3.64 |    0.02 |    6 |         - |          NA |
| RotateLSDRadixSort  | 4096 | Random             | 1,405,373.2 ns |  4,433.81 ns | 2,318.97 ns | 103.73 |    0.56 |    9 |         - |          NA |
| RadixLSD256Sort     | 4096 | Random             |    25,364.0 ns |  9,592.36 ns | 5,016.99 ns |   1.87 |    0.35 |    4 |         - |          NA |
| Radsort             | 4096 | Random             |    69,052.9 ns |    420.15 ns |   186.55 ns |   5.10 |    0.03 |    6 |         - |          NA |
| RadixLSD10Sort      | 4096 | Random             |    64,185.4 ns |    267.80 ns |   140.06 ns |   4.74 |    0.03 |    6 |         - |          NA |
| RadixMSD4Sort       | 4096 | Random             |    99,140.1 ns |    469.94 ns |   167.59 ns |   7.32 |    0.04 |    7 |         - |          NA |
| RadixMSD10Sort      | 4096 | Random             |    92,314.2 ns |    508.37 ns |   225.72 ns |   6.81 |    0.04 |    7 |         - |          NA |
| AmericanFlagSort    | 4096 | Random             |    58,867.7 ns |    360.57 ns |   160.09 ns |   4.35 |    0.02 |    6 |         - |          NA |
| SpreadSort          | 4096 | Random             |    30,265.3 ns |     99.89 ns |    52.25 ns |   2.23 |    0.01 |    5 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **4096** | **SingleElementMoved** |    **18,680.7 ns** |    **474.79 ns** |   **248.32 ns** |   **1.51** |    **0.08** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 4096 | SingleElementMoved |    12,384.2 ns |  1,273.62 ns |   666.13 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | SingleElementMoved |    15,300.7 ns |    145.61 ns |    76.16 ns |   1.24 |    0.06 |    3 |         - |          NA |
| PigeonSortInteger   | 4096 | SingleElementMoved |    10,207.7 ns |  1,040.51 ns |   544.21 ns |   0.83 |    0.06 |    1 |         - |          NA |
| BucketSort          | 4096 | SingleElementMoved |    23,801.5 ns |    282.34 ns |   147.67 ns |   1.93 |    0.10 |    3 |         - |          NA |
| BucketSortInteger   | 4096 | SingleElementMoved |    18,134.3 ns |    260.41 ns |   115.62 ns |   1.47 |    0.07 |    3 |         - |          NA |
| FlashSort           | 4096 | SingleElementMoved |   228,069.8 ns |  3,654.17 ns | 1,911.20 ns |  18.46 |    0.93 |    8 |         - |          NA |
| RadixLSD4Sort       | 4096 | SingleElementMoved |    65,395.8 ns |  1,667.00 ns |   871.87 ns |   5.29 |    0.27 |    6 |         - |          NA |
| RotateLSDRadixSort  | 4096 | SingleElementMoved |   857,474.9 ns |  1,916.96 ns |   851.14 ns |  69.41 |    3.46 |    9 |         - |          NA |
| RadixLSD256Sort     | 4096 | SingleElementMoved |    19,022.7 ns |    488.11 ns |   255.29 ns |   1.54 |    0.08 |    3 |         - |          NA |
| Radsort             | 4096 | SingleElementMoved |    43,753.3 ns |    766.76 ns |   340.45 ns |   3.54 |    0.18 |    5 |         - |          NA |
| RadixLSD10Sort      | 4096 | SingleElementMoved |    63,979.1 ns |    331.61 ns |   173.44 ns |   5.18 |    0.26 |    6 |         - |          NA |
| RadixMSD4Sort       | 4096 | SingleElementMoved |   100,580.0 ns |    980.42 ns |   435.31 ns |   8.14 |    0.41 |    7 |         - |          NA |
| RadixMSD10Sort      | 4096 | SingleElementMoved |    88,547.0 ns |    912.65 ns |   477.33 ns |   7.17 |    0.36 |    7 |         - |          NA |
| AmericanFlagSort    | 4096 | SingleElementMoved |    31,345.5 ns |    519.72 ns |   271.82 ns |   2.54 |    0.13 |    4 |         - |          NA |
| SpreadSort          | 4096 | SingleElementMoved |    22,659.7 ns |    356.77 ns |   158.41 ns |   1.83 |    0.09 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **4096** | **Sorted**             |    **17,910.0 ns** |    **982.79 ns** |   **514.02 ns** |   **1.53** |    **0.05** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Sorted             |    11,698.6 ns |    330.70 ns |   172.96 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 4096 | Sorted             |    15,185.1 ns |    131.67 ns |    58.46 ns |   1.30 |    0.02 |    4 |         - |          NA |
| PigeonSortInteger   | 4096 | Sorted             |     8,735.3 ns |    414.35 ns |   216.71 ns |   0.75 |    0.02 |    2 |         - |          NA |
| BucketSort          | 4096 | Sorted             |    22,741.9 ns |    351.58 ns |   156.10 ns |   1.94 |    0.03 |    5 |         - |          NA |
| BucketSortInteger   | 4096 | Sorted             |    17,117.5 ns |    202.52 ns |   105.92 ns |   1.46 |    0.02 |    4 |         - |          NA |
| FlashSort           | 4096 | Sorted             |   273,408.7 ns |  1,329.52 ns |   695.36 ns |  23.38 |    0.33 |    9 |         - |          NA |
| RadixLSD4Sort       | 4096 | Sorted             |    64,417.1 ns |  1,044.31 ns |   546.19 ns |   5.51 |    0.09 |    7 |         - |          NA |
| RotateLSDRadixSort  | 4096 | Sorted             |   853,327.8 ns |  6,640.79 ns | 2,948.55 ns |  72.96 |    1.05 |   10 |         - |          NA |
| RadixLSD256Sort     | 4096 | Sorted             |    17,963.0 ns |     69.67 ns |    30.94 ns |   1.54 |    0.02 |    4 |         - |          NA |
| Radsort             | 4096 | Sorted             |    42,652.8 ns |    209.26 ns |    92.91 ns |   3.65 |    0.05 |    6 |         - |          NA |
| RadixLSD10Sort      | 4096 | Sorted             |    63,188.0 ns |    440.06 ns |   195.39 ns |   5.40 |    0.08 |    7 |         - |          NA |
| RadixMSD4Sort       | 4096 | Sorted             |    99,652.2 ns |    463.48 ns |   165.28 ns |   8.52 |    0.12 |    8 |         - |          NA |
| RadixMSD10Sort      | 4096 | Sorted             |    89,306.2 ns |    352.18 ns |   184.20 ns |   7.64 |    0.11 |    8 |         - |          NA |
| AmericanFlagSort    | 4096 | Sorted             |    26,971.4 ns |    399.26 ns |   177.27 ns |   2.31 |    0.04 |    5 |         - |          NA |
| SpreadSort          | 4096 | Sorted             |     1,783.4 ns |     27.39 ns |    14.32 ns |   0.15 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **4096** | **Reversed**           |    **17,467.4 ns** |    **123.34 ns** |    **54.76 ns** |   **1.48** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Reversed           |    11,778.1 ns |    356.04 ns |   186.21 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | Reversed           |    16,363.6 ns |    176.13 ns |    92.12 ns |   1.39 |    0.02 |    3 |         - |          NA |
| PigeonSortInteger   | 4096 | Reversed           |     8,697.5 ns |    333.23 ns |   147.96 ns |   0.74 |    0.02 |    1 |         - |          NA |
| BucketSort          | 4096 | Reversed           |    23,755.9 ns |    145.19 ns |    64.47 ns |   2.02 |    0.03 |    4 |         - |          NA |
| BucketSortInteger   | 4096 | Reversed           |    17,135.1 ns |    234.60 ns |   122.70 ns |   1.46 |    0.02 |    3 |         - |          NA |
| FlashSort           | 4096 | Reversed           |   185,848.0 ns |  1,141.54 ns |   597.05 ns |  15.78 |    0.24 |    9 |         - |          NA |
| RadixLSD4Sort       | 4096 | Reversed           |    69,931.1 ns |  1,090.46 ns |   570.33 ns |   5.94 |    0.10 |    7 |         - |          NA |
| RotateLSDRadixSort  | 4096 | Reversed           |   869,240.5 ns |  2,870.01 ns | 1,501.07 ns |  73.82 |    1.11 |   10 |         - |          NA |
| RadixLSD256Sort     | 4096 | Reversed           |    17,945.5 ns |    265.28 ns |   138.75 ns |   1.52 |    0.03 |    3 |         - |          NA |
| Radsort             | 4096 | Reversed           |    41,926.4 ns |    622.98 ns |   325.83 ns |   3.56 |    0.06 |    6 |         - |          NA |
| RadixLSD10Sort      | 4096 | Reversed           |    62,727.0 ns |    247.26 ns |   109.79 ns |   5.33 |    0.08 |    7 |         - |          NA |
| RadixMSD4Sort       | 4096 | Reversed           |   101,441.1 ns |  1,273.89 ns |   454.28 ns |   8.61 |    0.13 |    8 |         - |          NA |
| RadixMSD10Sort      | 4096 | Reversed           |    87,484.4 ns |    678.54 ns |   301.28 ns |   7.43 |    0.11 |    8 |         - |          NA |
| AmericanFlagSort    | 4096 | Reversed           |    32,129.6 ns |    258.82 ns |   135.37 ns |   2.73 |    0.04 |    5 |         - |          NA |
| SpreadSort          | 4096 | Reversed           |    16,524.8 ns |     64.55 ns |    33.76 ns |   1.40 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **4096** | **PipeOrgan**          |    **17,977.9 ns** |     **89.39 ns** |    **46.75 ns** |   **1.34** |    **0.01** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 4096 | PipeOrgan          |    13,369.8 ns |    129.72 ns |    57.60 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | PipeOrgan          |    15,316.2 ns |    143.20 ns |    74.90 ns |   1.15 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 4096 | PipeOrgan          |     9,794.3 ns |    274.73 ns |   143.69 ns |   0.73 |    0.01 |    1 |         - |          NA |
| BucketSort          | 4096 | PipeOrgan          |    24,151.4 ns |    292.01 ns |   152.73 ns |   1.81 |    0.01 |    3 |         - |          NA |
| BucketSortInteger   | 4096 | PipeOrgan          |    19,481.7 ns |    296.37 ns |   131.59 ns |   1.46 |    0.01 |    2 |         - |          NA |
| FlashSort           | 4096 | PipeOrgan          |   184,057.8 ns |  1,580.56 ns |   826.66 ns |  13.77 |    0.08 |    7 |         - |          NA |
| RadixLSD4Sort       | 4096 | PipeOrgan          |    55,540.3 ns |    540.87 ns |   282.89 ns |   4.15 |    0.03 |    5 |         - |          NA |
| RotateLSDRadixSort  | 4096 | PipeOrgan          |   857,105.1 ns |  2,797.90 ns | 1,242.28 ns |  64.11 |    0.27 |    8 |         - |          NA |
| RadixLSD256Sort     | 4096 | PipeOrgan          |    19,821.1 ns |    179.86 ns |    94.07 ns |   1.48 |    0.01 |    2 |         - |          NA |
| Radsort             | 4096 | PipeOrgan          |    42,525.3 ns |    409.66 ns |   214.26 ns |   3.18 |    0.02 |    4 |         - |          NA |
| RadixLSD10Sort      | 4096 | PipeOrgan          |    63,532.9 ns |     92.75 ns |    33.08 ns |   4.75 |    0.02 |    5 |         - |          NA |
| RadixMSD4Sort       | 4096 | PipeOrgan          |   105,842.0 ns |    550.50 ns |   244.43 ns |   7.92 |    0.04 |    6 |         - |          NA |
| RadixMSD10Sort      | 4096 | PipeOrgan          |    89,111.4 ns |    787.41 ns |   411.83 ns |   6.67 |    0.04 |    6 |         - |          NA |
| AmericanFlagSort    | 4096 | PipeOrgan          |    46,200.5 ns |    350.63 ns |   183.38 ns |   3.46 |    0.02 |    4 |         - |          NA |
| SpreadSort          | 4096 | PipeOrgan          |    24,068.4 ns |    303.75 ns |   158.87 ns |   1.80 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **4096** | **ManyDuplicates**     |    **16,777.4 ns** |     **85.73 ns** |    **38.07 ns** |   **1.52** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 4096 | ManyDuplicates     |    11,011.1 ns |    310.20 ns |   162.24 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | ManyDuplicates     |    19,777.3 ns |    174.51 ns |    77.48 ns |   1.80 |    0.03 |    3 |         - |          NA |
| PigeonSortInteger   | 4096 | ManyDuplicates     |     8,744.2 ns |    291.43 ns |   152.42 ns |   0.79 |    0.02 |    1 |         - |          NA |
| BucketSort          | 4096 | ManyDuplicates     |    32,719.3 ns |    192.83 ns |    85.62 ns |   2.97 |    0.04 |    3 |         - |          NA |
| BucketSortInteger   | 4096 | ManyDuplicates     |    18,541.9 ns |    224.00 ns |    99.46 ns |   1.68 |    0.02 |    3 |         - |          NA |
| FlashSort           | 4096 | ManyDuplicates     |   165,574.6 ns |    532.37 ns |   236.37 ns |  15.04 |    0.21 |    4 |         - |          NA |
| RadixLSD4Sort       | 4096 | ManyDuplicates     |    26,944.3 ns |    229.17 ns |   101.75 ns |   2.45 |    0.04 |    3 |         - |          NA |
| RotateLSDRadixSort  | 4096 | ManyDuplicates     |   772,248.7 ns |  3,002.71 ns | 1,333.22 ns |  70.15 |    0.98 |    5 |         - |          NA |
| RadixLSD256Sort     | 4096 | ManyDuplicates     |    12,611.1 ns |    276.24 ns |   144.48 ns |   1.15 |    0.02 |    2 |         - |          NA |
| Radsort             | 4096 | ManyDuplicates     |    42,290.3 ns |    235.82 ns |   104.71 ns |   3.84 |    0.05 |    3 |         - |          NA |
| RadixLSD10Sort      | 4096 | ManyDuplicates     |    34,959.2 ns |    379.47 ns |   198.47 ns |   3.18 |    0.05 |    3 |         - |          NA |
| RadixMSD4Sort       | 4096 | ManyDuplicates     |    28,432.8 ns |    178.53 ns |    93.38 ns |   2.58 |    0.04 |    3 |         - |          NA |
| RadixMSD10Sort      | 4096 | ManyDuplicates     |    37,050.2 ns |    265.56 ns |   138.89 ns |   3.37 |    0.05 |    3 |         - |          NA |
| AmericanFlagSort    | 4096 | ManyDuplicates     |    25,579.2 ns |    332.86 ns |   147.79 ns |   2.32 |    0.03 |    3 |         - |          NA |
| SpreadSort          | 4096 | ManyDuplicates     |    23,529.3 ns |    596.85 ns |   312.17 ns |   2.14 |    0.04 |    3 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **8192** | **Random**             |    **39,505.7 ns** |    **197.16 ns** |    **87.54 ns** |   **1.41** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Random             |    27,950.1 ns |    373.54 ns |   195.37 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | Random             |    30,643.4 ns |    234.36 ns |   104.06 ns |   1.10 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | Random             |    20,025.5 ns |    440.10 ns |   230.18 ns |   0.72 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | Random             |    50,028.9 ns |    484.76 ns |   215.24 ns |   1.79 |    0.01 |    4 |         - |          NA |
| BucketSortInteger   | 8192 | Random             |    38,816.5 ns |    502.93 ns |   223.30 ns |   1.39 |    0.01 |    3 |         - |          NA |
| FlashSort           | 8192 | Random             |   363,006.8 ns |  1,048.16 ns |   465.39 ns |  12.99 |    0.09 |    8 |         - |          NA |
| RadixLSD4Sort       | 8192 | Random             |   114,018.9 ns |  1,674.83 ns |   743.63 ns |   4.08 |    0.04 |    5 |         - |          NA |
| RotateLSDRadixSort  | 8192 | Random             | 3,396,884.8 ns |  8,823.36 ns | 4,614.79 ns | 121.54 |    0.81 |    9 |         - |          NA |
| RadixLSD256Sort     | 8192 | Random             |    39,234.1 ns |    259.40 ns |   115.18 ns |   1.40 |    0.01 |    3 |         - |          NA |
| Radsort             | 8192 | Random             |    94,434.3 ns |    294.37 ns |   104.98 ns |   3.38 |    0.02 |    5 |         - |          NA |
| RadixLSD10Sort      | 8192 | Random             |   129,091.0 ns |  4,277.38 ns | 1,899.18 ns |   4.62 |    0.07 |    5 |         - |          NA |
| RadixMSD4Sort       | 8192 | Random             |   213,548.5 ns |    501.67 ns |   222.74 ns |   7.64 |    0.05 |    7 |         - |          NA |
| RadixMSD10Sort      | 8192 | Random             |   176,469.6 ns |    449.85 ns |   199.74 ns |   6.31 |    0.04 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | Random             |   116,528.6 ns |    683.05 ns |   303.28 ns |   4.17 |    0.03 |    5 |         - |          NA |
| SpreadSort          | 8192 | Random             |   103,495.4 ns |    301.49 ns |   157.69 ns |   3.70 |    0.02 |    5 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **8192** | **SingleElementMoved** |    **37,330.6 ns** |    **347.07 ns** |   **181.52 ns** |   **1.55** |    **0.01** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 8192 | SingleElementMoved |    24,081.9 ns |    409.36 ns |   214.10 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | SingleElementMoved |    30,235.6 ns |    341.42 ns |   151.59 ns |   1.26 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 8192 | SingleElementMoved |    19,454.1 ns |    352.61 ns |   184.42 ns |   0.81 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | SingleElementMoved |    49,711.4 ns |    433.12 ns |   226.53 ns |   2.06 |    0.02 |    4 |         - |          NA |
| BucketSortInteger   | 8192 | SingleElementMoved |    40,939.0 ns |    205.53 ns |    91.26 ns |   1.70 |    0.01 |    4 |         - |          NA |
| FlashSort           | 8192 | SingleElementMoved |   494,583.0 ns |  9,578.17 ns | 5,009.57 ns |  20.54 |    0.26 |    9 |         - |          NA |
| RadixLSD4Sort       | 8192 | SingleElementMoved |   158,628.4 ns |  1,882.94 ns |   984.81 ns |   6.59 |    0.07 |    7 |         - |          NA |
| RotateLSDRadixSort  | 8192 | SingleElementMoved | 2,177,326.3 ns |  2,663.19 ns | 1,392.90 ns |  90.42 |    0.76 |   10 |         - |          NA |
| RadixLSD256Sort     | 8192 | SingleElementMoved |    39,491.9 ns |    455.97 ns |   202.45 ns |   1.64 |    0.02 |    4 |         - |          NA |
| Radsort             | 8192 | SingleElementMoved |    61,541.3 ns |    352.61 ns |   184.42 ns |   2.56 |    0.02 |    5 |         - |          NA |
| RadixLSD10Sort      | 8192 | SingleElementMoved |   128,681.3 ns |  4,234.43 ns | 1,880.12 ns |   5.34 |    0.09 |    6 |         - |          NA |
| RadixMSD4Sort       | 8192 | SingleElementMoved |   214,425.8 ns |  1,353.28 ns |   600.86 ns |   8.90 |    0.08 |    8 |         - |          NA |
| RadixMSD10Sort      | 8192 | SingleElementMoved |   178,055.6 ns |  1,228.84 ns |   545.61 ns |   7.39 |    0.07 |    7 |         - |          NA |
| AmericanFlagSort    | 8192 | SingleElementMoved |    60,818.3 ns |    846.94 ns |   376.05 ns |   2.53 |    0.03 |    5 |         - |          NA |
| SpreadSort          | 8192 | SingleElementMoved |    48,944.3 ns |    297.06 ns |   131.90 ns |   2.03 |    0.02 |    4 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **8192** | **Sorted**             |    **37,076.7 ns** |  **1,016.75 ns** |   **531.78 ns** |   **1.59** |    **0.02** |    **5** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Sorted             |    23,392.1 ns |    387.19 ns |   171.92 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 8192 | Sorted             |    30,070.4 ns |    402.12 ns |   210.32 ns |   1.29 |    0.01 |    4 |         - |          NA |
| PigeonSortInteger   | 8192 | Sorted             |    17,395.4 ns |    445.52 ns |   233.02 ns |   0.74 |    0.01 |    2 |         - |          NA |
| BucketSort          | 8192 | Sorted             |    48,535.4 ns |    425.61 ns |   222.60 ns |   2.07 |    0.02 |    6 |         - |          NA |
| BucketSortInteger   | 8192 | Sorted             |    38,204.2 ns |    494.84 ns |   219.71 ns |   1.63 |    0.01 |    5 |         - |          NA |
| FlashSort           | 8192 | Sorted             |   470,411.3 ns |  4,242.54 ns | 1,883.71 ns |  20.11 |    0.16 |   10 |         - |          NA |
| RadixLSD4Sort       | 8192 | Sorted             |   163,016.9 ns |  1,670.93 ns |   873.93 ns |   6.97 |    0.06 |    8 |         - |          NA |
| RotateLSDRadixSort  | 8192 | Sorted             | 2,164,373.5 ns |  3,698.96 ns | 1,642.36 ns |  92.53 |    0.64 |   11 |         - |          NA |
| RadixLSD256Sort     | 8192 | Sorted             |    37,149.5 ns |    593.09 ns |   263.33 ns |   1.59 |    0.02 |    5 |         - |          NA |
| Radsort             | 8192 | Sorted             |    59,700.2 ns |    290.87 ns |   152.13 ns |   2.55 |    0.02 |    6 |         - |          NA |
| RadixLSD10Sort      | 8192 | Sorted             |   126,988.8 ns |    597.55 ns |   312.53 ns |   5.43 |    0.04 |    7 |         - |          NA |
| RadixMSD4Sort       | 8192 | Sorted             |   214,705.3 ns |  1,471.93 ns |   769.85 ns |   9.18 |    0.07 |    9 |         - |          NA |
| RadixMSD10Sort      | 8192 | Sorted             |   175,661.9 ns |  1,182.23 ns |   524.92 ns |   7.51 |    0.06 |    8 |         - |          NA |
| AmericanFlagSort    | 8192 | Sorted             |    53,011.0 ns |    523.56 ns |   232.46 ns |   2.27 |    0.02 |    6 |         - |          NA |
| SpreadSort          | 8192 | Sorted             |     3,519.8 ns |     30.51 ns |    13.55 ns |   0.15 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **8192** | **Reversed**           |    **34,399.6 ns** |    **196.09 ns** |   **102.56 ns** |   **1.45** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Reversed           |    23,655.2 ns |    345.46 ns |   153.39 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | Reversed           |    32,396.7 ns |     80.38 ns |    35.69 ns |   1.37 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 8192 | Reversed           |    17,414.4 ns |    265.19 ns |   117.75 ns |   0.74 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | Reversed           |    47,768.8 ns |    279.19 ns |   146.02 ns |   2.02 |    0.01 |    4 |         - |          NA |
| BucketSortInteger   | 8192 | Reversed           |    34,486.9 ns |    377.42 ns |   167.58 ns |   1.46 |    0.01 |    3 |         - |          NA |
| FlashSort           | 8192 | Reversed           |   402,670.0 ns | 17,370.01 ns | 9,084.85 ns |  17.02 |    0.38 |    8 |         - |          NA |
| RadixLSD4Sort       | 8192 | Reversed           |   160,689.6 ns |  2,313.33 ns | 1,209.91 ns |   6.79 |    0.06 |    6 |         - |          NA |
| RotateLSDRadixSort  | 8192 | Reversed           | 2,205,797.1 ns |  4,166.05 ns | 1,849.75 ns |  93.25 |    0.57 |    9 |         - |          NA |
| RadixLSD256Sort     | 8192 | Reversed           |    37,526.7 ns |    418.48 ns |   185.81 ns |   1.59 |    0.01 |    3 |         - |          NA |
| Radsort             | 8192 | Reversed           |    56,923.0 ns |    720.03 ns |   376.59 ns |   2.41 |    0.02 |    4 |         - |          NA |
| RadixLSD10Sort      | 8192 | Reversed           |   125,569.4 ns |    730.92 ns |   324.53 ns |   5.31 |    0.03 |    5 |         - |          NA |
| RadixMSD4Sort       | 8192 | Reversed           |   220,826.4 ns |  1,040.38 ns |   544.14 ns |   9.34 |    0.06 |    7 |         - |          NA |
| RadixMSD10Sort      | 8192 | Reversed           |   176,520.6 ns |    470.59 ns |   208.94 ns |   7.46 |    0.05 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | Reversed           |    63,486.9 ns |    322.83 ns |   143.34 ns |   2.68 |    0.02 |    4 |         - |          NA |
| SpreadSort          | 8192 | Reversed           |    56,931.3 ns |    346.61 ns |   153.90 ns |   2.41 |    0.02 |    4 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **8192** | **PipeOrgan**          |    **37,002.9 ns** |    **358.49 ns** |   **159.17 ns** |   **1.48** |    **0.02** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 8192 | PipeOrgan          |    25,013.0 ns |    493.24 ns |   257.97 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | PipeOrgan          |    30,276.2 ns |    495.44 ns |   259.12 ns |   1.21 |    0.02 |    3 |         - |          NA |
| PigeonSortInteger   | 8192 | PipeOrgan          |    19,632.2 ns |    399.06 ns |   177.19 ns |   0.78 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | PipeOrgan          |    47,202.1 ns |    303.76 ns |   158.87 ns |   1.89 |    0.02 |    4 |         - |          NA |
| BucketSortInteger   | 8192 | PipeOrgan          |    36,443.1 ns |    377.13 ns |   167.45 ns |   1.46 |    0.02 |    4 |         - |          NA |
| FlashSort           | 8192 | PipeOrgan          |   335,346.4 ns |  1,136.53 ns |   594.43 ns |  13.41 |    0.13 |   11 |         - |          NA |
| RadixLSD4Sort       | 8192 | PipeOrgan          |   135,656.7 ns |    999.53 ns |   522.77 ns |   5.42 |    0.06 |    8 |         - |          NA |
| RotateLSDRadixSort  | 8192 | PipeOrgan          | 2,179,324.0 ns |  9,546.65 ns | 3,404.43 ns |  87.14 |    0.85 |   12 |         - |          NA |
| RadixLSD256Sort     | 8192 | PipeOrgan          |    40,260.6 ns |    226.29 ns |   100.47 ns |   1.61 |    0.02 |    4 |         - |          NA |
| Radsort             | 8192 | PipeOrgan          |    59,277.5 ns |    902.23 ns |   471.88 ns |   2.37 |    0.03 |    5 |         - |          NA |
| RadixLSD10Sort      | 8192 | PipeOrgan          |   127,730.9 ns |  1,141.78 ns |   506.96 ns |   5.11 |    0.05 |    8 |         - |          NA |
| RadixMSD4Sort       | 8192 | PipeOrgan          |   221,075.3 ns |  1,071.61 ns |   475.80 ns |   8.84 |    0.09 |   10 |         - |          NA |
| RadixMSD10Sort      | 8192 | PipeOrgan          |   178,881.0 ns |  1,099.75 ns |   488.30 ns |   7.15 |    0.07 |    9 |         - |          NA |
| AmericanFlagSort    | 8192 | PipeOrgan          |    92,931.8 ns |  1,135.52 ns |   504.18 ns |   3.72 |    0.04 |    7 |         - |          NA |
| SpreadSort          | 8192 | PipeOrgan          |    74,983.7 ns |  1,107.23 ns |   579.10 ns |   3.00 |    0.04 |    6 |         - |          NA |
|      |                    |                |              |             |        |         |      |           |             |
| **CountingSort**        | **8192** | **ManyDuplicates**     |    **33,726.8 ns** |    **259.97 ns** |   **115.43 ns** |   **1.49** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | ManyDuplicates     |    22,632.1 ns |    494.57 ns |   258.67 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | ManyDuplicates     |    53,882.6 ns |    130.94 ns |    68.49 ns |   2.38 |    0.03 |    4 |         - |          NA |
| PigeonSortInteger   | 8192 | ManyDuplicates     |    17,332.6 ns |    111.16 ns |    58.14 ns |   0.77 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | ManyDuplicates     |    67,565.5 ns |    856.63 ns |   380.35 ns |   2.99 |    0.04 |    4 |         - |          NA |
| BucketSortInteger   | 8192 | ManyDuplicates     |    36,902.4 ns |     86.52 ns |    38.42 ns |   1.63 |    0.02 |    3 |         - |          NA |
| FlashSort           | 8192 | ManyDuplicates     |   335,798.3 ns |    970.70 ns |   507.70 ns |  14.84 |    0.16 |    5 |         - |          NA |
| RadixLSD4Sort       | 8192 | ManyDuplicates     |    54,126.8 ns |    152.71 ns |    79.87 ns |   2.39 |    0.03 |    4 |         - |          NA |
| RotateLSDRadixSort  | 8192 | ManyDuplicates     | 1,628,305.1 ns |  2,323.73 ns | 1,031.75 ns |  71.95 |    0.78 |    6 |         - |          NA |
| RadixLSD256Sort     | 8192 | ManyDuplicates     |    25,391.9 ns |    416.65 ns |   217.92 ns |   1.12 |    0.02 |    2 |         - |          NA |
| Radsort             | 8192 | ManyDuplicates     |    53,456.5 ns |    534.83 ns |   237.47 ns |   2.36 |    0.03 |    4 |         - |          NA |
| RadixLSD10Sort      | 8192 | ManyDuplicates     |    69,186.8 ns |    651.23 ns |   340.61 ns |   3.06 |    0.04 |    4 |         - |          NA |
| RadixMSD4Sort       | 8192 | ManyDuplicates     |    56,691.6 ns |    694.59 ns |   363.28 ns |   2.51 |    0.03 |    4 |         - |          NA |
| RadixMSD10Sort      | 8192 | ManyDuplicates     |    72,348.8 ns |    786.54 ns |   349.23 ns |   3.20 |    0.04 |    4 |         - |          NA |
| AmericanFlagSort    | 8192 | ManyDuplicates     |    50,194.4 ns |    133.71 ns |    59.37 ns |   2.22 |    0.02 |    4 |         - |          NA |
| SpreadSort          | 8192 | ManyDuplicates     |    46,932.4 ns |    551.78 ns |   288.59 ns |   2.07 |    0.03 |    4 |         - |          NA |

### ExchangeBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.58 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method             | Size | Pattern            | Mean         | Error        | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------- |----- |------------------- |-------------:|-------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **BubbleSort**         | **256**  | **Random**             |  **41,864.5 ns** |    **372.42 ns** |   **194.78 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Random             |  27,093.8 ns |    163.98 ns |    85.76 ns |  0.65 |    0.00 |    2 |         - |          NA |
| OddEvenSort        | 256  | Random             |  41,926.7 ns |    210.85 ns |    93.62 ns |  1.00 |    0.00 |    3 |         - |          NA |
| CombSort           | 256  | Random             |   7,881.5 ns |     51.06 ns |    22.67 ns |  0.19 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | Random             |  23,386.4 ns |    155.25 ns |    68.93 ns |  0.56 |    0.00 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **BubbleSort**         | **256**  | **SingleElementMoved** |   **4,503.4 ns** |     **57.40 ns** |    **20.47 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | SingleElementMoved |     301.1 ns |      7.32 ns |     3.25 ns |  0.07 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 256  | SingleElementMoved |   8,468.8 ns |    260.71 ns |   136.36 ns |  1.88 |    0.03 |    4 |         - |          NA |
| CombSort           | 256  | SingleElementMoved |   2,665.1 ns |     58.11 ns |    25.80 ns |  0.59 |    0.01 |    2 |         - |          NA |
| CircleSort         | 256  | SingleElementMoved |  10,166.0 ns |    143.41 ns |    63.67 ns |  2.26 |    0.02 |    5 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **BubbleSort**         | **256**  | **Sorted**             |     **238.7 ns** |      **8.40 ns** |     **3.73 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Sorted             |     154.0 ns |     12.86 ns |     6.73 ns |  0.65 |    0.03 |    1 |         - |          NA |
| OddEvenSort        | 256  | Sorted             |     199.0 ns |     10.54 ns |     5.51 ns |  0.83 |    0.02 |    2 |         - |          NA |
| CombSort           | 256  | Sorted             |   2,245.1 ns |     11.39 ns |     5.06 ns |  9.41 |    0.14 |    4 |         - |          NA |
| CircleSort         | 256  | Sorted             |   1,642.8 ns |     32.68 ns |    11.65 ns |  6.88 |    0.11 |    3 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **BubbleSort**         | **256**  | **Reversed**           |  **22,984.6 ns** |    **392.25 ns** |   **205.16 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Reversed           |  21,679.3 ns |    170.12 ns |    88.98 ns |  0.94 |    0.01 |    3 |         - |          NA |
| OddEvenSort        | 256  | Reversed           |  19,759.1 ns |    262.19 ns |   116.41 ns |  0.86 |    0.01 |    3 |         - |          NA |
| CombSort           | 256  | Reversed           |   2,412.7 ns |     68.16 ns |    24.31 ns |  0.10 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | Reversed           |   3,341.2 ns |    151.44 ns |    67.24 ns |  0.15 |    0.00 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **BubbleSort**         | **256**  | **PipeOrgan**          |  **26,859.2 ns** |    **234.72 ns** |   **104.22 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 256  | PipeOrgan          |  13,788.7 ns |    175.47 ns |    91.77 ns |  0.51 |    0.00 |    2 |         - |          NA |
| OddEvenSort        | 256  | PipeOrgan          |  21,835.6 ns |    171.11 ns |    75.98 ns |  0.81 |    0.00 |    3 |         - |          NA |
| CombSort           | 256  | PipeOrgan          |   2,454.6 ns |     42.66 ns |    18.94 ns |  0.09 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | PipeOrgan          |  15,247.2 ns |    135.16 ns |    70.69 ns |  0.57 |    0.00 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **BubbleSort**         | **256**  | **ManyDuplicates**     |  **41,132.1 ns** |    **174.00 ns** |    **77.26 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 256  | ManyDuplicates     |  26,676.3 ns |    309.85 ns |   137.58 ns |  0.65 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 256  | ManyDuplicates     |  43,754.1 ns |    328.68 ns |   171.91 ns |  1.06 |    0.00 |    4 |         - |          NA |
| CombSort           | 256  | ManyDuplicates     |   6,222.1 ns |    192.19 ns |    85.33 ns |  0.15 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | ManyDuplicates     |  20,022.8 ns |    204.70 ns |   107.06 ns |  0.49 |    0.00 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **BubbleSort**         | **1024** | **Random**             | **532,119.1 ns** |  **4,383.88 ns** | **2,292.86 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Random             | 296,981.5 ns |  1,636.44 ns |   726.59 ns |  0.56 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | Random             | 480,305.4 ns |  1,720.53 ns |   899.87 ns |  0.90 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | Random             |  42,452.9 ns |    146.20 ns |    76.47 ns |  0.08 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | Random             | 127,213.5 ns |    462.31 ns |   164.87 ns |  0.24 |    0.00 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **BubbleSort**         | **1024** | **SingleElementMoved** |  **66,706.7 ns** |     **99.67 ns** |    **52.13 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | SingleElementMoved |   1,104.2 ns |     12.71 ns |     5.64 ns |  0.02 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 1024 | SingleElementMoved | 122,268.1 ns |    891.58 ns |   395.87 ns |  1.83 |    0.01 |    4 |         - |          NA |
| CombSort           | 1024 | SingleElementMoved |  12,681.7 ns |    313.70 ns |   164.07 ns |  0.19 |    0.00 |    2 |         - |          NA |
| CircleSort         | 1024 | SingleElementMoved |  58,059.5 ns |    400.47 ns |   209.46 ns |  0.87 |    0.00 |    3 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **BubbleSort**         | **1024** | **Sorted**             |     **844.6 ns** |      **6.99 ns** |     **2.49 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Sorted             |     566.9 ns |      3.09 ns |     1.61 ns |  0.67 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 1024 | Sorted             |     732.8 ns |      8.76 ns |     3.89 ns |  0.87 |    0.00 |    2 |         - |          NA |
| CombSort           | 1024 | Sorted             |  11,601.4 ns |    247.47 ns |   129.43 ns | 13.74 |    0.15 |    4 |         - |          NA |
| CircleSort         | 1024 | Sorted             |   7,259.6 ns |    226.72 ns |   118.58 ns |  8.60 |    0.13 |    3 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **BubbleSort**         | **1024** | **Reversed**           | **340,146.9 ns** |  **2,516.25 ns** | **1,117.23 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Reversed           | 338,874.0 ns |    783.13 ns |   347.71 ns |  1.00 |    0.00 |    2 |         - |          NA |
| OddEvenSort        | 1024 | Reversed           | 317,638.2 ns |  2,919.04 ns | 1,526.71 ns |  0.93 |    0.01 |    2 |         - |          NA |
| CombSort           | 1024 | Reversed           |  12,742.1 ns |    311.56 ns |   162.95 ns |  0.04 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | Reversed           |  14,713.9 ns |    259.10 ns |   135.51 ns |  0.04 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **BubbleSort**         | **1024** | **PipeOrgan**          | **394,808.3 ns** |  **2,129.81 ns** |   **945.65 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | PipeOrgan          | 221,678.2 ns |  2,009.80 ns | 1,051.16 ns |  0.56 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | PipeOrgan          | 349,483.4 ns |  3,584.63 ns | 1,591.60 ns |  0.89 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | PipeOrgan          |  12,855.4 ns |    253.25 ns |   132.45 ns |  0.03 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | PipeOrgan          |  82,434.7 ns |    606.62 ns |   269.34 ns |  0.21 |    0.00 |    2 |         - |          NA |
|      |                    |              |              |             |       |         |      |           |             |
| **BubbleSort**         | **1024** | **ManyDuplicates**     | **529,080.4 ns** |  **2,654.99 ns** | **1,178.83 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | ManyDuplicates     | 291,054.6 ns |  1,259.29 ns |   658.63 ns |  0.55 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | ManyDuplicates     | 481,645.4 ns | 17,414.98 ns | 9,108.37 ns |  0.91 |    0.02 |    4 |         - |          NA |
| CombSort           | 1024 | ManyDuplicates     |  28,256.8 ns |    179.63 ns |    93.95 ns |  0.05 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | ManyDuplicates     |  98,594.3 ns |  1,038.19 ns |   460.96 ns |  0.19 |    0.00 |    2 |         - |          NA |

### HeapBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.58 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method           | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **HeapSort**         | **256**  | **Random**             |     **8,696.6 ns** |    **391.15 ns** |    **204.58 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | Random             |     8,767.3 ns |    259.52 ns |    135.73 ns |  1.01 |    0.03 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | Random             |    10,501.2 ns |    412.53 ns |    215.76 ns |  1.21 |    0.04 |    1 |         - |          NA |
| BottomupHeapSort | 256  | Random             |    10,761.1 ns |    334.88 ns |    175.15 ns |  1.24 |    0.03 |    1 |         - |          NA |
| WeakHeapSort     | 256  | Random             |    14,399.8 ns |    110.39 ns |     49.01 ns |  1.66 |    0.04 |    2 |         - |          NA |
| SmoothSort       | 256  | Random             |    13,484.0 ns |    275.42 ns |    122.29 ns |  1.55 |    0.04 |    2 |         - |          NA |
| PoplarHeapSort   | 256  | Random             |    13,781.5 ns |    266.88 ns |    118.50 ns |  1.59 |    0.04 |    2 |         - |          NA |
| TournamentSort   | 256  | Random             |    23,306.2 ns |    373.85 ns |    195.53 ns |  2.68 |    0.06 |    3 |         - |          NA |
| BinomialHeapSort | 256  | Random             |    27,596.2 ns |    598.11 ns |    312.82 ns |  3.17 |    0.08 |    3 |         - |          NA |
| PairingHeapSort  | 256  | Random             |    16,158.8 ns |     99.20 ns |     44.05 ns |  1.86 |    0.04 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **SingleElementMoved** |     **3,558.6 ns** |    **844.53 ns** |    **441.70 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 256  | SingleElementMoved |     4,134.9 ns |    425.02 ns |    222.29 ns |  1.18 |    0.15 |    3 |         - |          NA |
| TernaryHeapSort  | 256  | SingleElementMoved |     4,218.5 ns |    490.38 ns |    256.48 ns |  1.20 |    0.16 |    3 |         - |          NA |
| BottomupHeapSort | 256  | SingleElementMoved |     4,137.3 ns |    253.67 ns |    112.63 ns |  1.18 |    0.14 |    3 |         - |          NA |
| WeakHeapSort     | 256  | SingleElementMoved |     6,888.1 ns |    153.21 ns |     68.02 ns |  1.96 |    0.23 |    4 |         - |          NA |
| SmoothSort       | 256  | SingleElementMoved |     1,324.4 ns |    110.81 ns |     49.20 ns |  0.38 |    0.05 |    1 |         - |          NA |
| PoplarHeapSort   | 256  | SingleElementMoved |     2,930.6 ns |    253.39 ns |    112.51 ns |  0.83 |    0.10 |    2 |         - |          NA |
| TournamentSort   | 256  | SingleElementMoved |     5,710.1 ns |    634.35 ns |    281.65 ns |  1.63 |    0.21 |    4 |         - |          NA |
| BinomialHeapSort | 256  | SingleElementMoved |     6,073.3 ns |    131.29 ns |     58.29 ns |  1.73 |    0.20 |    4 |         - |          NA |
| PairingHeapSort  | 256  | SingleElementMoved |     4,339.6 ns |    169.69 ns |     75.34 ns |  1.24 |    0.15 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **Sorted**             |     **2,152.3 ns** |    **280.11 ns** |    **124.37 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 256  | Sorted             |     2,492.5 ns |     93.58 ns |     41.55 ns |  1.16 |    0.06 |    2 |         - |          NA |
| TernaryHeapSort  | 256  | Sorted             |     3,583.3 ns |    290.69 ns |    152.04 ns |  1.67 |    0.11 |    3 |         - |          NA |
| BottomupHeapSort | 256  | Sorted             |     3,426.4 ns |    130.48 ns |     57.93 ns |  1.60 |    0.09 |    3 |         - |          NA |
| WeakHeapSort     | 256  | Sorted             |     6,388.2 ns |     25.91 ns |     11.50 ns |  2.98 |    0.16 |    5 |         - |          NA |
| SmoothSort       | 256  | Sorted             |       913.7 ns |      6.03 ns |      2.15 ns |  0.43 |    0.02 |    1 |         - |          NA |
| PoplarHeapSort   | 256  | Sorted             |     2,577.0 ns |    247.75 ns |    129.58 ns |  1.20 |    0.09 |    2 |         - |          NA |
| TournamentSort   | 256  | Sorted             |     2,675.4 ns |    186.16 ns |     97.37 ns |  1.25 |    0.08 |    2 |         - |          NA |
| BinomialHeapSort | 256  | Sorted             |     5,193.8 ns |    160.75 ns |     71.37 ns |  2.42 |    0.13 |    4 |         - |          NA |
| PairingHeapSort  | 256  | Sorted             |     4,223.0 ns |    230.33 ns |    120.47 ns |  1.97 |    0.12 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **Reversed**           |     **2,570.8 ns** |    **114.58 ns** |     **59.93 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | Reversed           |     2,209.8 ns |    173.60 ns |     90.80 ns |  0.86 |    0.04 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | Reversed           |     3,510.1 ns |    292.23 ns |    152.84 ns |  1.37 |    0.06 |    2 |         - |          NA |
| BottomupHeapSort | 256  | Reversed           |     3,655.8 ns |    252.92 ns |    132.28 ns |  1.42 |    0.06 |    2 |         - |          NA |
| WeakHeapSort     | 256  | Reversed           |     7,426.7 ns |    100.21 ns |     52.41 ns |  2.89 |    0.07 |    4 |         - |          NA |
| SmoothSort       | 256  | Reversed           |     3,877.9 ns |    206.58 ns |     91.72 ns |  1.51 |    0.05 |    2 |         - |          NA |
| PoplarHeapSort   | 256  | Reversed           |     4,908.0 ns |    341.79 ns |    178.76 ns |  1.91 |    0.08 |    3 |         - |          NA |
| TournamentSort   | 256  | Reversed           |     4,015.3 ns |    361.52 ns |    160.52 ns |  1.56 |    0.07 |    2 |         - |          NA |
| BinomialHeapSort | 256  | Reversed           |     5,178.9 ns |    135.56 ns |     70.90 ns |  2.02 |    0.05 |    3 |         - |          NA |
| PairingHeapSort  | 256  | Reversed           |     1,995.6 ns |      5.33 ns |      2.37 ns |  0.78 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **PipeOrgan**          |     **2,470.7 ns** |    **180.33 ns** |     **80.07 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | PipeOrgan          |     2,513.9 ns |    242.02 ns |    126.58 ns |  1.02 |    0.06 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | PipeOrgan          |     3,344.0 ns |    189.64 ns |     84.20 ns |  1.35 |    0.05 |    2 |         - |          NA |
| BottomupHeapSort | 256  | PipeOrgan          |     3,442.6 ns |    136.61 ns |     60.66 ns |  1.39 |    0.05 |    2 |         - |          NA |
| WeakHeapSort     | 256  | PipeOrgan          |     6,914.3 ns |     81.93 ns |     36.38 ns |  2.80 |    0.08 |    3 |         - |          NA |
| SmoothSort       | 256  | PipeOrgan          |     3,974.4 ns |    249.77 ns |    130.63 ns |  1.61 |    0.07 |    2 |         - |          NA |
| PoplarHeapSort   | 256  | PipeOrgan          |     5,055.3 ns |    251.00 ns |    131.28 ns |  2.05 |    0.08 |    3 |         - |          NA |
| TournamentSort   | 256  | PipeOrgan          |     5,379.0 ns |    469.86 ns |    245.74 ns |  2.18 |    0.11 |    3 |         - |          NA |
| BinomialHeapSort | 256  | PipeOrgan          |     6,022.7 ns |    296.08 ns |    154.85 ns |  2.44 |    0.09 |    3 |         - |          NA |
| PairingHeapSort  | 256  | PipeOrgan          |     5,301.9 ns |     48.87 ns |     17.43 ns |  2.15 |    0.06 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **ManyDuplicates**     |     **8,496.9 ns** |    **373.18 ns** |    **195.18 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | ManyDuplicates     |     8,591.2 ns |    194.01 ns |    101.47 ns |  1.01 |    0.02 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | ManyDuplicates     |     9,919.1 ns |    226.14 ns |    100.41 ns |  1.17 |    0.03 |    1 |         - |          NA |
| BottomupHeapSort | 256  | ManyDuplicates     |    10,354.1 ns |    268.33 ns |    140.34 ns |  1.22 |    0.03 |    1 |         - |          NA |
| WeakHeapSort     | 256  | ManyDuplicates     |    12,559.0 ns |    351.49 ns |    183.84 ns |  1.48 |    0.04 |    2 |         - |          NA |
| SmoothSort       | 256  | ManyDuplicates     |    12,562.9 ns |    283.51 ns |    125.88 ns |  1.48 |    0.03 |    2 |         - |          NA |
| PoplarHeapSort   | 256  | ManyDuplicates     |    13,289.8 ns |    374.60 ns |    166.33 ns |  1.56 |    0.04 |    2 |         - |          NA |
| TournamentSort   | 256  | ManyDuplicates     |    22,883.6 ns |    218.95 ns |    114.52 ns |  2.69 |    0.06 |    3 |         - |          NA |
| BinomialHeapSort | 256  | ManyDuplicates     |    24,311.6 ns |    212.11 ns |     94.18 ns |  2.86 |    0.06 |    3 |         - |          NA |
| PairingHeapSort  | 256  | ManyDuplicates     |    15,207.0 ns |    138.40 ns |     61.45 ns |  1.79 |    0.04 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Random**             |    **43,078.9 ns** |    **323.09 ns** |    **168.98 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Random             |    43,812.0 ns |    327.39 ns |    171.23 ns |  1.02 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | Random             |    51,730.7 ns |     76.55 ns |     27.30 ns |  1.20 |    0.00 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | Random             |    52,023.8 ns |    268.69 ns |    140.53 ns |  1.21 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | Random             |    73,119.3 ns |    809.69 ns |    423.48 ns |  1.70 |    0.01 |    2 |         - |          NA |
| SmoothSort       | 1024 | Random             |    70,982.3 ns |    339.38 ns |    177.50 ns |  1.65 |    0.01 |    2 |         - |          NA |
| PoplarHeapSort   | 1024 | Random             |    67,998.0 ns |    567.23 ns |    296.67 ns |  1.58 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 1024 | Random             |   117,905.4 ns |  1,140.91 ns |    506.57 ns |  2.74 |    0.01 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | Random             |   156,602.9 ns |  2,668.71 ns |    951.69 ns |  3.64 |    0.02 |    4 |         - |          NA |
| PairingHeapSort  | 1024 | Random             |    81,036.0 ns |    593.34 ns |    263.45 ns |  1.88 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **SingleElementMoved** |    **18,923.1 ns** |  **4,702.77 ns** |  **2,459.64 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 1024 | SingleElementMoved |    21,840.1 ns |    320.90 ns |    142.48 ns |  1.17 |    0.15 |    3 |         - |          NA |
| TernaryHeapSort  | 1024 | SingleElementMoved |    18,864.5 ns |    464.70 ns |    206.33 ns |  1.01 |    0.13 |    3 |         - |          NA |
| BottomupHeapSort | 1024 | SingleElementMoved |    19,961.5 ns |    331.39 ns |    147.14 ns |  1.07 |    0.14 |    3 |         - |          NA |
| WeakHeapSort     | 1024 | SingleElementMoved |    34,335.4 ns |    331.38 ns |    147.13 ns |  1.84 |    0.23 |    5 |         - |          NA |
| SmoothSort       | 1024 | SingleElementMoved |     4,763.5 ns |    115.27 ns |     51.18 ns |  0.26 |    0.03 |    1 |         - |          NA |
| PoplarHeapSort   | 1024 | SingleElementMoved |    12,582.2 ns |    328.32 ns |    171.72 ns |  0.68 |    0.09 |    2 |         - |          NA |
| TournamentSort   | 1024 | SingleElementMoved |    31,636.6 ns |  3,557.90 ns |  1,860.85 ns |  1.70 |    0.24 |    5 |         - |          NA |
| BinomialHeapSort | 1024 | SingleElementMoved |    26,274.2 ns |    387.28 ns |    202.55 ns |  1.41 |    0.18 |    4 |         - |          NA |
| PairingHeapSort  | 1024 | SingleElementMoved |    17,321.3 ns |    103.43 ns |     54.10 ns |  0.93 |    0.12 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Sorted**             |    **11,882.8 ns** |    **496.25 ns** |    **220.34 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Sorted             |    12,019.8 ns |    386.54 ns |    202.17 ns |  1.01 |    0.02 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | Sorted             |    16,426.3 ns |    325.78 ns |    144.65 ns |  1.38 |    0.03 |    3 |         - |          NA |
| BottomupHeapSort | 1024 | Sorted             |    16,141.9 ns |    218.18 ns |     96.87 ns |  1.36 |    0.02 |    3 |         - |          NA |
| WeakHeapSort     | 1024 | Sorted             |    33,353.3 ns |    196.22 ns |    102.63 ns |  2.81 |    0.05 |    5 |         - |          NA |
| SmoothSort       | 1024 | Sorted             |     3,793.7 ns |    294.14 ns |    153.84 ns |  0.32 |    0.01 |    1 |         - |          NA |
| PoplarHeapSort   | 1024 | Sorted             |    11,247.8 ns |    243.27 ns |    127.23 ns |  0.95 |    0.02 |    2 |         - |          NA |
| TournamentSort   | 1024 | Sorted             |    12,790.2 ns |    534.98 ns |    279.81 ns |  1.08 |    0.03 |    2 |         - |          NA |
| BinomialHeapSort | 1024 | Sorted             |    22,786.7 ns |    257.13 ns |    114.17 ns |  1.92 |    0.03 |    4 |         - |          NA |
| PairingHeapSort  | 1024 | Sorted             |    17,449.9 ns |     68.35 ns |     24.37 ns |  1.47 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Reversed**           |    **12,525.1 ns** |    **328.08 ns** |    **145.67 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Reversed           |    12,444.1 ns |    432.66 ns |    226.29 ns |  0.99 |    0.02 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | Reversed           |    16,132.3 ns |    425.21 ns |    188.80 ns |  1.29 |    0.02 |    3 |         - |          NA |
| BottomupHeapSort | 1024 | Reversed           |    16,517.5 ns |    445.19 ns |    197.67 ns |  1.32 |    0.02 |    3 |         - |          NA |
| WeakHeapSort     | 1024 | Reversed           |    36,439.4 ns |    149.65 ns |     66.45 ns |  2.91 |    0.03 |    4 |         - |          NA |
| SmoothSort       | 1024 | Reversed           |    18,641.7 ns |    441.76 ns |    196.14 ns |  1.49 |    0.02 |    3 |         - |          NA |
| PoplarHeapSort   | 1024 | Reversed           |    22,506.4 ns |    548.30 ns |    286.77 ns |  1.80 |    0.03 |    3 |         - |          NA |
| TournamentSort   | 1024 | Reversed           |    20,461.4 ns |    755.25 ns |    335.34 ns |  1.63 |    0.03 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | Reversed           |    22,897.7 ns |    271.41 ns |    141.96 ns |  1.83 |    0.02 |    3 |         - |          NA |
| PairingHeapSort  | 1024 | Reversed           |     7,962.1 ns |    302.45 ns |    158.19 ns |  0.64 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **PipeOrgan**          |    **12,255.9 ns** |    **685.33 ns** |    **358.44 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | PipeOrgan          |    12,459.2 ns |    665.93 ns |    348.29 ns |  1.02 |    0.04 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | PipeOrgan          |    16,028.0 ns |    301.08 ns |    133.68 ns |  1.31 |    0.04 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | PipeOrgan          |    16,005.1 ns |    451.97 ns |    200.68 ns |  1.31 |    0.04 |    2 |         - |          NA |
| WeakHeapSort     | 1024 | PipeOrgan          |    36,812.0 ns |    543.55 ns |    284.29 ns |  3.01 |    0.09 |    4 |         - |          NA |
| SmoothSort       | 1024 | PipeOrgan          |    20,264.2 ns |    727.81 ns |    380.66 ns |  1.65 |    0.05 |    3 |         - |          NA |
| PoplarHeapSort   | 1024 | PipeOrgan          |    23,843.6 ns |    308.02 ns |    136.76 ns |  1.95 |    0.06 |    3 |         - |          NA |
| TournamentSort   | 1024 | PipeOrgan          |    26,694.2 ns |  2,963.60 ns |  1,315.86 ns |  2.18 |    0.12 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | PipeOrgan          |    25,682.1 ns |    247.78 ns |    129.60 ns |  2.10 |    0.06 |    3 |         - |          NA |
| PairingHeapSort  | 1024 | PipeOrgan          |    22,486.1 ns |    234.76 ns |    104.23 ns |  1.84 |    0.05 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **ManyDuplicates**     |    **42,224.1 ns** |    **636.01 ns** |    **332.65 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | ManyDuplicates     |    42,189.8 ns |    774.35 ns |    343.82 ns |  1.00 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | ManyDuplicates     |    46,622.7 ns |    250.19 ns |    130.85 ns |  1.10 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | ManyDuplicates     |    49,700.5 ns |    337.15 ns |    149.70 ns |  1.18 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | ManyDuplicates     |    58,877.1 ns |    110.78 ns |     49.19 ns |  1.39 |    0.01 |    1 |         - |          NA |
| SmoothSort       | 1024 | ManyDuplicates     |    63,846.2 ns |    399.03 ns |    208.70 ns |  1.51 |    0.01 |    1 |         - |          NA |
| PoplarHeapSort   | 1024 | ManyDuplicates     |    64,134.1 ns |    622.29 ns |    325.47 ns |  1.52 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 1024 | ManyDuplicates     |   113,724.3 ns |    947.06 ns |    495.33 ns |  2.69 |    0.02 |    2 |         - |          NA |
| BinomialHeapSort | 1024 | ManyDuplicates     |   122,534.2 ns |    927.74 ns |    411.92 ns |  2.90 |    0.02 |    2 |         - |          NA |
| PairingHeapSort  | 1024 | ManyDuplicates     |    69,655.5 ns |    582.51 ns |    258.64 ns |  1.65 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Random**             |   **205,195.4 ns** |  **3,213.81 ns** |  **1,426.95 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Random             |   206,186.2 ns |    831.34 ns |    369.12 ns |  1.00 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 4096 | Random             |   245,396.8 ns |  2,494.21 ns |  1,107.44 ns |  1.20 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 4096 | Random             |   244,422.5 ns |  1,720.18 ns |    763.77 ns |  1.19 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | Random             |   358,089.8 ns |  1,682.13 ns |    746.88 ns |  1.75 |    0.01 |    2 |         - |          NA |
| SmoothSort       | 4096 | Random             |   342,791.2 ns |  1,917.75 ns |  1,003.02 ns |  1.67 |    0.01 |    2 |         - |          NA |
| PoplarHeapSort   | 4096 | Random             |   319,628.6 ns |  1,518.55 ns |    541.53 ns |  1.56 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 4096 | Random             |   549,717.1 ns |  7,162.60 ns |  3,746.18 ns |  2.68 |    0.02 |    3 |         - |          NA |
| BinomialHeapSort | 4096 | Random             |   811,996.7 ns |  2,578.81 ns |  1,348.77 ns |  3.96 |    0.03 |    4 |         - |          NA |
| PairingHeapSort  | 4096 | Random             |   390,943.3 ns |  2,182.02 ns |    778.13 ns |  1.91 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **SingleElementMoved** |   **113,128.6 ns** |  **5,984.83 ns** |  **2,657.30 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 4096 | SingleElementMoved |   141,837.8 ns |  1,369.35 ns |    488.32 ns |  1.25 |    0.03 |    4 |         - |          NA |
| TernaryHeapSort  | 4096 | SingleElementMoved |   118,767.5 ns |  1,247.80 ns |    554.03 ns |  1.05 |    0.02 |    4 |         - |          NA |
| BottomupHeapSort | 4096 | SingleElementMoved |   131,706.2 ns |  3,350.75 ns |  1,752.51 ns |  1.16 |    0.03 |    4 |         - |          NA |
| WeakHeapSort     | 4096 | SingleElementMoved |   166,669.8 ns |    494.88 ns |    219.73 ns |  1.47 |    0.03 |    4 |         - |          NA |
| SmoothSort       | 4096 | SingleElementMoved |    18,715.0 ns |    223.04 ns |     99.03 ns |  0.17 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 4096 | SingleElementMoved |    54,875.6 ns |    686.22 ns |    304.69 ns |  0.49 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 4096 | SingleElementMoved |   220,549.1 ns | 12,156.46 ns |  6,358.06 ns |  1.95 |    0.07 |    5 |         - |          NA |
| BinomialHeapSort | 4096 | SingleElementMoved |   114,471.4 ns |    544.76 ns |    194.27 ns |  1.01 |    0.02 |    4 |         - |          NA |
| PairingHeapSort  | 4096 | SingleElementMoved |    69,200.7 ns |    253.72 ns |    112.65 ns |  0.61 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Sorted**             |    **92,994.6 ns** | **39,108.53 ns** | **20,454.52 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Sorted             |   116,430.8 ns |  6,297.61 ns |  3,293.77 ns |  1.30 |    0.24 |    4 |         - |          NA |
| TernaryHeapSort  | 4096 | Sorted             |    81,177.5 ns |    966.01 ns |    428.92 ns |  0.91 |    0.17 |    3 |         - |          NA |
| BottomupHeapSort | 4096 | Sorted             |    84,609.8 ns |  6,869.20 ns |  3,049.97 ns |  0.95 |    0.18 |    3 |         - |          NA |
| WeakHeapSort     | 4096 | Sorted             |   164,245.8 ns |  2,142.82 ns |  1,120.74 ns |  1.84 |    0.34 |    5 |         - |          NA |
| SmoothSort       | 4096 | Sorted             |    15,062.0 ns |    270.34 ns |    141.39 ns |  0.17 |    0.03 |    1 |         - |          NA |
| PoplarHeapSort   | 4096 | Sorted             |    49,748.5 ns |    254.40 ns |    133.06 ns |  0.56 |    0.10 |    2 |         - |          NA |
| TournamentSort   | 4096 | Sorted             |    86,407.9 ns | 29,633.98 ns | 15,499.14 ns |  0.97 |    0.24 |    3 |         - |          NA |
| BinomialHeapSort | 4096 | Sorted             |   101,940.1 ns |    288.42 ns |    128.06 ns |  1.14 |    0.21 |    4 |         - |          NA |
| PairingHeapSort  | 4096 | Sorted             |    68,559.3 ns |    417.93 ns |    185.57 ns |  0.77 |    0.14 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Reversed**           |    **83,988.3 ns** | **11,719.31 ns** |  **5,203.44 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Reversed           |   107,651.6 ns |  3,906.47 ns |  2,043.16 ns |  1.29 |    0.08 |    3 |         - |          NA |
| TernaryHeapSort  | 4096 | Reversed           |    78,731.0 ns |  2,893.62 ns |  1,031.89 ns |  0.94 |    0.05 |    2 |         - |          NA |
| BottomupHeapSort | 4096 | Reversed           |    82,883.0 ns |    660.78 ns |    235.64 ns |  0.99 |    0.06 |    2 |         - |          NA |
| WeakHeapSort     | 4096 | Reversed           |   175,820.8 ns |    930.77 ns |    486.81 ns |  2.10 |    0.12 |    4 |         - |          NA |
| SmoothSort       | 4096 | Reversed           |   102,708.4 ns |  1,199.88 ns |    427.89 ns |  1.23 |    0.07 |    3 |         - |          NA |
| PoplarHeapSort   | 4096 | Reversed           |   104,824.6 ns |  1,389.51 ns |    726.74 ns |  1.25 |    0.07 |    3 |         - |          NA |
| TournamentSort   | 4096 | Reversed           |   181,995.8 ns | 28,716.24 ns | 15,019.15 ns |  2.17 |    0.21 |    4 |         - |          NA |
| BinomialHeapSort | 4096 | Reversed           |   101,004.2 ns |  1,249.46 ns |    554.77 ns |  1.21 |    0.07 |    3 |         - |          NA |
| PairingHeapSort  | 4096 | Reversed           |    31,710.2 ns |     80.90 ns |     35.92 ns |  0.38 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **PipeOrgan**          |    **95,924.5 ns** | **24,211.48 ns** | **12,663.07 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 4096 | PipeOrgan          |   103,907.0 ns |  9,950.98 ns |  5,204.56 ns |  1.10 |    0.16 |    1 |         - |          NA |
| TernaryHeapSort  | 4096 | PipeOrgan          |    76,865.6 ns |  2,839.82 ns |  1,485.28 ns |  0.81 |    0.11 |    1 |         - |          NA |
| BottomupHeapSort | 4096 | PipeOrgan          |    80,118.6 ns |  1,539.03 ns |    683.34 ns |  0.85 |    0.12 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | PipeOrgan          |   177,697.9 ns |    956.55 ns |    424.71 ns |  1.88 |    0.26 |    2 |         - |          NA |
| SmoothSort       | 4096 | PipeOrgan          |   207,835.4 ns |    990.10 ns |    439.61 ns |  2.20 |    0.30 |    2 |         - |          NA |
| PoplarHeapSort   | 4096 | PipeOrgan          |   120,487.4 ns |  8,958.74 ns |  4,685.59 ns |  1.28 |    0.18 |    1 |         - |          NA |
| TournamentSort   | 4096 | PipeOrgan          |   357,588.3 ns |  9,205.14 ns |  4,814.47 ns |  3.79 |    0.52 |    3 |         - |          NA |
| BinomialHeapSort | 4096 | PipeOrgan          |   113,599.6 ns |    924.30 ns |    410.40 ns |  1.20 |    0.16 |    1 |         - |          NA |
| PairingHeapSort  | 4096 | PipeOrgan          |    92,001.5 ns |    844.31 ns |    374.88 ns |  0.98 |    0.13 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **ManyDuplicates**     |   **198,407.2 ns** |  **4,775.30 ns** |  **2,497.57 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 4096 | ManyDuplicates     |   199,140.6 ns |  1,491.69 ns |    662.32 ns |  1.00 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 4096 | ManyDuplicates     |   222,326.8 ns |  1,529.46 ns |    799.94 ns |  1.12 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 4096 | ManyDuplicates     |   234,587.4 ns |    436.60 ns |    193.85 ns |  1.18 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | ManyDuplicates     |   270,186.8 ns |  1,409.52 ns |    737.20 ns |  1.36 |    0.02 |    1 |         - |          NA |
| SmoothSort       | 4096 | ManyDuplicates     |   297,563.7 ns |    905.84 ns |    473.77 ns |  1.50 |    0.02 |    1 |         - |          NA |
| PoplarHeapSort   | 4096 | ManyDuplicates     |   301,465.1 ns |  1,426.46 ns |    746.07 ns |  1.52 |    0.02 |    1 |         - |          NA |
| TournamentSort   | 4096 | ManyDuplicates     |   510,603.1 ns |  3,036.05 ns |  1,587.91 ns |  2.57 |    0.03 |    2 |         - |          NA |
| BinomialHeapSort | 4096 | ManyDuplicates     |   546,809.7 ns |  4,153.92 ns |  1,844.37 ns |  2.76 |    0.03 |    2 |         - |          NA |
| PairingHeapSort  | 4096 | ManyDuplicates     |   306,771.7 ns |  4,723.64 ns |  2,470.56 ns |  1.55 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Random**             |   **442,005.0 ns** |  **5,052.46 ns** |  **2,642.54 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Random             |   443,855.3 ns |  2,427.58 ns |  1,077.86 ns |  1.00 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | Random             |   527,028.2 ns |    735.09 ns |    326.38 ns |  1.19 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 8192 | Random             |   523,492.7 ns |    932.24 ns |    487.58 ns |  1.18 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | Random             |   776,709.5 ns |  1,202.27 ns |    533.82 ns |  1.76 |    0.01 |    2 |         - |          NA |
| SmoothSort       | 8192 | Random             |   742,660.1 ns |  3,736.45 ns |  1,954.23 ns |  1.68 |    0.01 |    2 |         - |          NA |
| PoplarHeapSort   | 8192 | Random             |   687,057.7 ns |  1,892.02 ns |    989.56 ns |  1.55 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 8192 | Random             | 1,176,356.0 ns | 22,247.00 ns | 11,635.61 ns |  2.66 |    0.03 |    3 |         - |          NA |
| BinomialHeapSort | 8192 | Random             | 1,782,996.8 ns |  7,433.34 ns |  3,887.78 ns |  4.03 |    0.02 |    4 |         - |          NA |
| PairingHeapSort  | 8192 | Random             |   868,224.6 ns |  5,064.25 ns |  2,248.56 ns |  1.96 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **SingleElementMoved** |   **257,491.6 ns** |  **2,493.75 ns** |  **1,304.28 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 8192 | SingleElementMoved |   313,784.4 ns |  3,250.64 ns |  1,700.15 ns |  1.22 |    0.01 |    5 |         - |          NA |
| TernaryHeapSort  | 8192 | SingleElementMoved |   328,360.1 ns |  1,904.57 ns |    845.64 ns |  1.28 |    0.01 |    5 |         - |          NA |
| BottomupHeapSort | 8192 | SingleElementMoved |   332,646.9 ns |  3,302.19 ns |  1,727.11 ns |  1.29 |    0.01 |    5 |         - |          NA |
| WeakHeapSort     | 8192 | SingleElementMoved |   362,094.1 ns |    503.03 ns |    179.39 ns |  1.41 |    0.01 |    5 |         - |          NA |
| SmoothSort       | 8192 | SingleElementMoved |    37,432.4 ns |    201.86 ns |    105.58 ns |  0.15 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 8192 | SingleElementMoved |   114,760.7 ns |    768.54 ns |    341.24 ns |  0.45 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 8192 | SingleElementMoved |   485,102.9 ns |  7,232.12 ns |  3,782.54 ns |  1.88 |    0.02 |    6 |         - |          NA |
| BinomialHeapSort | 8192 | SingleElementMoved |   238,845.8 ns |  1,541.15 ns |    806.05 ns |  0.93 |    0.01 |    4 |         - |          NA |
| PairingHeapSort  | 8192 | SingleElementMoved |   141,579.8 ns |    322.38 ns |    114.97 ns |  0.55 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Sorted**             |   **252,684.8 ns** | **24,684.67 ns** | **12,910.56 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Sorted             |   309,450.1 ns |  1,482.89 ns |    658.41 ns |  1.23 |    0.06 |    5 |         - |          NA |
| TernaryHeapSort  | 8192 | Sorted             |   319,375.6 ns |  1,453.76 ns |    645.48 ns |  1.27 |    0.06 |    5 |         - |          NA |
| BottomupHeapSort | 8192 | Sorted             |   316,342.8 ns |  1,547.68 ns |    809.47 ns |  1.25 |    0.06 |    5 |         - |          NA |
| WeakHeapSort     | 8192 | Sorted             |   357,012.2 ns |    438.00 ns |    229.08 ns |  1.42 |    0.07 |    5 |         - |          NA |
| SmoothSort       | 8192 | Sorted             |    29,976.0 ns |    321.86 ns |    142.91 ns |  0.12 |    0.01 |    1 |         - |          NA |
| PoplarHeapSort   | 8192 | Sorted             |   105,000.1 ns |    539.56 ns |    239.57 ns |  0.42 |    0.02 |    2 |         - |          NA |
| TournamentSort   | 8192 | Sorted             |   392,646.4 ns | 51,646.66 ns | 27,012.20 ns |  1.56 |    0.12 |    5 |         - |          NA |
| BinomialHeapSort | 8192 | Sorted             |   213,078.2 ns |    830.02 ns |    295.99 ns |  0.85 |    0.04 |    4 |         - |          NA |
| PairingHeapSort  | 8192 | Sorted             |   140,036.9 ns |    533.35 ns |    236.81 ns |  0.56 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Reversed**           |   **287,713.8 ns** |  **1,037.53 ns** |    **460.67 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Reversed           |   276,112.3 ns |  7,371.64 ns |  3,855.51 ns |  0.96 |    0.01 |    3 |         - |          NA |
| TernaryHeapSort  | 8192 | Reversed           |   322,000.1 ns |    790.00 ns |    350.77 ns |  1.12 |    0.00 |    3 |         - |          NA |
| BottomupHeapSort | 8192 | Reversed           |   373,190.3 ns |  4,598.18 ns |  2,404.94 ns |  1.30 |    0.01 |    3 |         - |          NA |
| WeakHeapSort     | 8192 | Reversed           |   380,954.6 ns |  1,154.78 ns |    512.73 ns |  1.32 |    0.00 |    3 |         - |          NA |
| SmoothSort       | 8192 | Reversed           |   438,504.6 ns |    644.49 ns |    229.83 ns |  1.52 |    0.00 |    3 |         - |          NA |
| PoplarHeapSort   | 8192 | Reversed           |   354,085.8 ns |  1,934.27 ns |    689.78 ns |  1.23 |    0.00 |    3 |         - |          NA |
| TournamentSort   | 8192 | Reversed           |   534,896.4 ns |  5,509.51 ns |  2,881.58 ns |  1.86 |    0.01 |    4 |         - |          NA |
| BinomialHeapSort | 8192 | Reversed           |   212,237.3 ns |  1,151.58 ns |    511.31 ns |  0.74 |    0.00 |    2 |         - |          NA |
| PairingHeapSort  | 8192 | Reversed           |    63,452.6 ns |    504.86 ns |    264.05 ns |  0.22 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **PipeOrgan**          |   **282,229.9 ns** | **10,896.26 ns** |  **4,838.01 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 8192 | PipeOrgan          |   314,902.3 ns |  1,000.94 ns |    444.42 ns |  1.12 |    0.02 |    2 |         - |          NA |
| TernaryHeapSort  | 8192 | PipeOrgan          |   362,537.5 ns |  1,348.68 ns |    598.82 ns |  1.28 |    0.02 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | PipeOrgan          |   369,564.8 ns |  2,974.16 ns |  1,555.54 ns |  1.31 |    0.02 |    2 |         - |          NA |
| WeakHeapSort     | 8192 | PipeOrgan          |   387,919.1 ns |  1,346.09 ns |    704.03 ns |  1.37 |    0.02 |    2 |         - |          NA |
| SmoothSort       | 8192 | PipeOrgan          |   550,028.9 ns |  1,567.24 ns |    819.70 ns |  1.95 |    0.03 |    3 |         - |          NA |
| PoplarHeapSort   | 8192 | PipeOrgan          |   484,773.4 ns |  1,867.02 ns |    828.97 ns |  1.72 |    0.03 |    3 |         - |          NA |
| TournamentSort   | 8192 | PipeOrgan          |   866,914.3 ns | 12,535.23 ns |  5,565.72 ns |  3.07 |    0.05 |    4 |         - |          NA |
| BinomialHeapSort | 8192 | PipeOrgan          |   235,303.4 ns |  1,345.98 ns |    703.98 ns |  0.83 |    0.01 |    2 |         - |          NA |
| PairingHeapSort  | 8192 | PipeOrgan          |   188,102.5 ns |    934.18 ns |    414.78 ns |  0.67 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **ManyDuplicates**     |   **424,668.9 ns** |  **4,407.13 ns** |  **2,305.01 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | ManyDuplicates     |   431,094.1 ns |  7,986.47 ns |  3,546.04 ns |  1.02 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | ManyDuplicates     |   478,925.7 ns |  3,840.60 ns |  1,705.25 ns |  1.13 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 8192 | ManyDuplicates     |   509,280.7 ns |    957.00 ns |    500.53 ns |  1.20 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | ManyDuplicates     |   581,561.1 ns | 46,926.62 ns | 20,835.71 ns |  1.37 |    0.05 |    1 |         - |          NA |
| SmoothSort       | 8192 | ManyDuplicates     |   624,555.8 ns |  2,202.43 ns |    977.89 ns |  1.47 |    0.01 |    1 |         - |          NA |
| PoplarHeapSort   | 8192 | ManyDuplicates     |   643,923.1 ns |  1,360.98 ns |    604.29 ns |  1.52 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 8192 | ManyDuplicates     | 1,079,957.2 ns |  9,050.61 ns |  4,733.64 ns |  2.54 |    0.02 |    2 |         - |          NA |
| BinomialHeapSort | 8192 | ManyDuplicates     | 1,138,993.9 ns | 13,160.73 ns |  5,843.45 ns |  2.68 |    0.02 |    2 |         - |          NA |
| PairingHeapSort  | 8192 | ManyDuplicates     |   641,501.8 ns |  2,421.53 ns |  1,266.51 ns |  1.51 |    0.01 |    1 |         - |          NA |

### InsertionBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.58 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                 | Size | Pattern            | Mean         | Error        | StdDev      | Ratio  | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------- |----- |------------------- |-------------:|-------------:|------------:|-------:|--------:|-----:|----------:|------------:|
| **InsertionSort**          | **256**  | **Random**             |   **7,270.0 ns** |     **56.13 ns** |    **29.36 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Random             |   9,571.2 ns |  1,516.18 ns |   792.99 ns |   1.32 |    0.10 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | Random             |  11,735.5 ns |    276.20 ns |   144.46 ns |   1.61 |    0.02 |    3 |         - |          NA |
| GnomeSort              | 256  | Random             |  28,062.5 ns |    767.34 ns |   401.33 ns |   3.86 |    0.05 |    5 |         - |          NA |
| LibrarySort            | 256  | Random             |  22,787.9 ns |    383.27 ns |   200.46 ns |   3.13 |    0.03 |    4 |         - |          NA |
| MergeInsertionSort     | 256  | Random             |  20,523.2 ns |    546.94 ns |   286.06 ns |   2.82 |    0.04 |    4 |         - |          NA |
| ShellSortKnuth1973     | 256  | Random             |   9,447.8 ns |    273.44 ns |   143.01 ns |   1.30 |    0.02 |    2 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Random             |   9,581.5 ns |    306.82 ns |   136.23 ns |   1.32 |    0.02 |    2 |         - |          NA |
| ShellSortTokuda1992    | 256  | Random             |   9,448.3 ns |    288.93 ns |   151.12 ns |   1.30 |    0.02 |    2 |         - |          NA |
| ShellSortCiura2001     | 256  | Random             |   9,587.7 ns |    230.03 ns |   120.31 ns |   1.32 |    0.02 |    2 |         - |          NA |
| ShellSortLee2021       | 256  | Random             |   9,586.2 ns |    337.75 ns |   176.65 ns |   1.32 |    0.02 |    2 |         - |          NA |
|      |                    |              |              |             |        |         |      |           |             |
| **InsertionSort**          | **256**  | **SingleElementMoved** |     **203.4 ns** |      **9.49 ns** |     **4.96 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 256  | SingleElementMoved |     249.3 ns |     15.27 ns |     6.78 ns |   1.23 |    0.04 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | SingleElementMoved |     484.1 ns |     48.11 ns |    21.36 ns |   2.38 |    0.11 |    4 |         - |          NA |
| GnomeSort              | 256  | SingleElementMoved |     394.5 ns |     20.06 ns |    10.49 ns |   1.94 |    0.07 |    3 |         - |          NA |
| LibrarySort            | 256  | SingleElementMoved |  21,643.2 ns |    248.29 ns |   129.86 ns | 106.46 |    2.50 |    7 |         - |          NA |
| MergeInsertionSort     | 256  | SingleElementMoved |  10,162.7 ns |    297.00 ns |   155.34 ns |  49.99 |    1.35 |    6 |         - |          NA |
| ShellSortKnuth1973     | 256  | SingleElementMoved |   1,085.3 ns |     29.93 ns |    13.29 ns |   5.34 |    0.14 |    5 |         - |          NA |
| ShellSortSedgewick1986 | 256  | SingleElementMoved |   1,091.6 ns |     13.80 ns |     6.13 ns |   5.37 |    0.13 |    5 |         - |          NA |
| ShellSortTokuda1992    | 256  | SingleElementMoved |   1,316.7 ns |     48.23 ns |    21.42 ns |   6.48 |    0.18 |    5 |         - |          NA |
| ShellSortCiura2001     | 256  | SingleElementMoved |   1,163.5 ns |     47.01 ns |    20.87 ns |   5.72 |    0.16 |    5 |         - |          NA |
| ShellSortLee2021       | 256  | SingleElementMoved |   1,323.6 ns |     25.79 ns |    11.45 ns |   6.51 |    0.16 |    5 |         - |          NA |
|      |                    |              |              |             |        |         |      |           |             |
| **InsertionSort**          | **256**  | **Sorted**             |     **167.7 ns** |     **18.84 ns** |     **8.36 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Sorted             |     212.8 ns |      7.17 ns |     3.75 ns |   1.27 |    0.07 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | Sorted             |     223.0 ns |      4.53 ns |     2.01 ns |   1.33 |    0.07 |    2 |         - |          NA |
| GnomeSort              | 256  | Sorted             |     221.2 ns |      2.72 ns |     1.42 ns |   1.32 |    0.07 |    2 |         - |          NA |
| LibrarySort            | 256  | Sorted             |  21,294.6 ns |    256.12 ns |   113.72 ns | 127.31 |    6.47 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | Sorted             |   9,947.9 ns |    269.17 ns |   140.78 ns |  59.47 |    3.11 |    4 |         - |          NA |
| ShellSortKnuth1973     | 256  | Sorted             |     929.1 ns |      6.40 ns |     2.84 ns |   5.55 |    0.28 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Sorted             |     958.6 ns |     21.79 ns |     9.68 ns |   5.73 |    0.29 |    3 |         - |          NA |
| ShellSortTokuda1992    | 256  | Sorted             |   1,144.7 ns |      3.77 ns |     1.68 ns |   6.84 |    0.35 |    3 |         - |          NA |
| ShellSortCiura2001     | 256  | Sorted             |   1,017.9 ns |      8.44 ns |     3.75 ns |   6.09 |    0.31 |    3 |         - |          NA |
| ShellSortLee2021       | 256  | Sorted             |   1,162.8 ns |     16.04 ns |     7.12 ns |   6.95 |    0.35 |    3 |         - |          NA |
|      |                    |              |              |             |        |         |      |           |             |
| **InsertionSort**          | **256**  | **Reversed**           |  **11,850.2 ns** |    **163.53 ns** |    **72.61 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Reversed           |  15,694.7 ns |    158.05 ns |    82.66 ns |   1.32 |    0.01 |    5 |         - |          NA |
| BinaryInsertSort       | 256  | Reversed           |   4,902.8 ns |    250.54 ns |   131.04 ns |   0.41 |    0.01 |    2 |         - |          NA |
| GnomeSort              | 256  | Reversed           |  52,124.5 ns |    863.96 ns |   451.87 ns |   4.40 |    0.04 |    6 |         - |          NA |
| LibrarySort            | 256  | Reversed           |  17,650.6 ns |    274.10 ns |   143.36 ns |   1.49 |    0.01 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | Reversed           |   9,817.1 ns |    222.76 ns |   116.51 ns |   0.83 |    0.01 |    3 |         - |          NA |
| ShellSortKnuth1973     | 256  | Reversed           |   1,292.0 ns |     16.46 ns |     8.61 ns |   0.11 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Reversed           |   1,317.1 ns |      9.41 ns |     3.35 ns |   0.11 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | Reversed           |   1,394.5 ns |     25.19 ns |     8.98 ns |   0.12 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | Reversed           |   1,368.7 ns |     30.64 ns |    10.93 ns |   0.12 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Reversed           |   1,413.1 ns |      7.01 ns |     3.67 ns |   0.12 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |        |         |      |           |             |
| **InsertionSort**          | **256**  | **PipeOrgan**          |   **6,391.8 ns** |     **28.22 ns** |    **14.76 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 256  | PipeOrgan          |   8,043.5 ns |    268.91 ns |   140.64 ns |   1.26 |    0.02 |    4 |         - |          NA |
| BinaryInsertSort       | 256  | PipeOrgan          |   2,882.9 ns |    158.36 ns |    70.31 ns |   0.45 |    0.01 |    2 |         - |          NA |
| GnomeSort              | 256  | PipeOrgan          |  25,838.5 ns |    219.35 ns |   114.73 ns |   4.04 |    0.02 |    7 |         - |          NA |
| LibrarySort            | 256  | PipeOrgan          |  15,008.7 ns |    315.02 ns |   139.87 ns |   2.35 |    0.02 |    6 |         - |          NA |
| MergeInsertionSort     | 256  | PipeOrgan          |  10,174.2 ns |    294.59 ns |   154.07 ns |   1.59 |    0.02 |    5 |         - |          NA |
| ShellSortKnuth1973     | 256  | PipeOrgan          |   1,182.7 ns |      8.53 ns |     3.04 ns |   0.19 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | PipeOrgan          |   1,272.9 ns |     79.30 ns |    35.21 ns |   0.20 |    0.01 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | PipeOrgan          |   1,364.6 ns |     27.41 ns |    12.17 ns |   0.21 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | PipeOrgan          |   1,334.2 ns |     12.49 ns |     5.55 ns |   0.21 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | PipeOrgan          |   1,391.0 ns |     28.41 ns |    14.86 ns |   0.22 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |        |         |      |           |             |
| **InsertionSort**          | **256**  | **ManyDuplicates**     |   **7,341.6 ns** |    **257.42 ns** |   **134.63 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 256  | ManyDuplicates     |  12,267.1 ns |  5,567.41 ns | 2,911.87 ns |   1.67 |    0.38 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | ManyDuplicates     |  11,481.7 ns |    251.17 ns |   131.37 ns |   1.56 |    0.03 |    2 |         - |          NA |
| GnomeSort              | 256  | ManyDuplicates     |  27,367.4 ns |  1,012.82 ns |   529.72 ns |   3.73 |    0.09 |    3 |         - |          NA |
| LibrarySort            | 256  | ManyDuplicates     |  23,829.2 ns |    327.39 ns |   171.23 ns |   3.25 |    0.06 |    3 |         - |          NA |
| MergeInsertionSort     | 256  | ManyDuplicates     |  20,233.7 ns |    258.60 ns |   114.82 ns |   2.76 |    0.05 |    3 |         - |          NA |
| ShellSortKnuth1973     | 256  | ManyDuplicates     |   7,540.5 ns |    206.50 ns |    91.69 ns |   1.03 |    0.02 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | ManyDuplicates     |   7,976.1 ns |    124.36 ns |    55.22 ns |   1.09 |    0.02 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | ManyDuplicates     |   7,908.7 ns |    287.01 ns |   127.44 ns |   1.08 |    0.02 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | ManyDuplicates     |   7,745.5 ns |     90.65 ns |    40.25 ns |   1.06 |    0.02 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | ManyDuplicates     |   7,734.0 ns |    180.68 ns |    94.50 ns |   1.05 |    0.02 |    1 |         - |          NA |
|      |                    |              |              |             |        |         |      |           |             |
| **InsertionSort**          | **1024** | **Random**             |  **95,846.1 ns** |  **4,402.86 ns** | **1,954.90 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Random             | 122,101.1 ns |  4,079.03 ns | 2,133.41 ns |   1.27 |    0.03 |    3 |         - |          NA |
| BinaryInsertSort       | 1024 | Random             |  63,030.6 ns |    294.27 ns |   130.66 ns |   0.66 |    0.01 |    2 |         - |          NA |
| GnomeSort              | 1024 | Random             | 390,779.5 ns |  4,360.34 ns | 1,936.02 ns |   4.08 |    0.08 |    4 |         - |          NA |
| LibrarySort            | 1024 | Random             | 107,071.1 ns |    654.48 ns |   290.59 ns |   1.12 |    0.02 |    3 |         - |          NA |
| MergeInsertionSort     | 1024 | Random             | 107,167.1 ns |    921.87 ns |   409.32 ns |   1.12 |    0.02 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Random             |  51,558.7 ns |    289.76 ns |   128.65 ns |   0.54 |    0.01 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Random             |  52,289.1 ns |    393.92 ns |   206.03 ns |   0.55 |    0.01 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Random             |  52,415.4 ns |    213.09 ns |   111.45 ns |   0.55 |    0.01 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Random             |  51,523.6 ns |    180.82 ns |    94.57 ns |   0.54 |    0.01 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Random             |  52,209.5 ns |    396.60 ns |   176.09 ns |   0.54 |    0.01 |    1 |         - |          NA |
|      |                    |              |              |             |        |         |      |           |             |
| **InsertionSort**          | **1024** | **SingleElementMoved** |     **722.7 ns** |     **35.32 ns** |    **15.68 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | SingleElementMoved |     890.3 ns |     15.88 ns |     7.05 ns |   1.23 |    0.03 |    2 |         - |          NA |
| BinaryInsertSort       | 1024 | SingleElementMoved |   1,709.0 ns |     27.50 ns |    12.21 ns |   2.37 |    0.05 |    4 |         - |          NA |
| GnomeSort              | 1024 | SingleElementMoved |   1,375.1 ns |     14.09 ns |     6.25 ns |   1.90 |    0.04 |    3 |         - |          NA |
| LibrarySort            | 1024 | SingleElementMoved | 192,658.3 ns |    614.48 ns |   272.83 ns | 266.68 |    5.32 |    7 |         - |          NA |
| MergeInsertionSort     | 1024 | SingleElementMoved |  48,679.4 ns |    167.22 ns |    87.46 ns |  67.38 |    1.35 |    6 |         - |          NA |
| ShellSortKnuth1973     | 1024 | SingleElementMoved |   4,907.6 ns |     58.13 ns |    30.40 ns |   6.79 |    0.14 |    5 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | SingleElementMoved |   5,642.3 ns |    487.60 ns |   255.02 ns |   7.81 |    0.37 |    5 |         - |          NA |
| ShellSortTokuda1992    | 1024 | SingleElementMoved |   6,040.3 ns |    361.23 ns |   188.93 ns |   8.36 |    0.30 |    5 |         - |          NA |
| ShellSortCiura2001     | 1024 | SingleElementMoved |   5,882.2 ns |    282.80 ns |   147.91 ns |   8.14 |    0.25 |    5 |         - |          NA |
| ShellSortLee2021       | 1024 | SingleElementMoved |   5,884.8 ns |     76.36 ns |    39.94 ns |   8.15 |    0.17 |    5 |         - |          NA |
|      |                    |              |              |             |        |         |      |           |             |
| **InsertionSort**          | **1024** | **Sorted**             |     **567.9 ns** |      **4.46 ns** |     **1.59 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Sorted             |     710.7 ns |      6.20 ns |     3.24 ns |   1.25 |    0.01 |    2 |         - |          NA |
| BinaryInsertSort       | 1024 | Sorted             |     867.7 ns |     15.89 ns |     8.31 ns |   1.53 |    0.01 |    2 |         - |          NA |
| GnomeSort              | 1024 | Sorted             |     843.3 ns |      2.97 ns |     1.32 ns |   1.48 |    0.00 |    2 |         - |          NA |
| LibrarySort            | 1024 | Sorted             | 197,044.5 ns |    800.20 ns |   355.30 ns | 346.97 |    1.08 |    5 |         - |          NA |
| MergeInsertionSort     | 1024 | Sorted             |  48,058.9 ns |    185.50 ns |    97.02 ns |  84.62 |    0.27 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Sorted             |   4,633.4 ns |     39.44 ns |    17.51 ns |   8.16 |    0.04 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Sorted             |   5,344.8 ns |    271.68 ns |   142.09 ns |   9.41 |    0.24 |    3 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Sorted             |   5,669.8 ns |    245.30 ns |   128.30 ns |   9.98 |    0.22 |    3 |         - |          NA |
| ShellSortCiura2001     | 1024 | Sorted             |   5,574.1 ns |    237.53 ns |   124.23 ns |   9.82 |    0.21 |    3 |         - |          NA |
| ShellSortLee2021       | 1024 | Sorted             |   5,575.0 ns |     53.57 ns |    19.10 ns |   9.82 |    0.04 |    3 |         - |          NA |
|      |                    |              |              |             |        |         |      |           |             |
| **InsertionSort**          | **1024** | **Reversed**           | **177,818.5 ns** |    **326.23 ns** |   **144.85 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Reversed           | 244,558.1 ns |  1,309.31 ns |   581.34 ns |   1.38 |    0.00 |    5 |         - |          NA |
| BinaryInsertSort       | 1024 | Reversed           |  35,384.6 ns |    203.88 ns |    90.52 ns |   0.20 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | Reversed           | 751,891.7 ns |  3,027.98 ns | 1,344.44 ns |   4.23 |    0.01 |    6 |         - |          NA |
| LibrarySort            | 1024 | Reversed           | 196,798.1 ns |  1,546.98 ns |   686.87 ns |   1.11 |    0.00 |    4 |         - |          NA |
| MergeInsertionSort     | 1024 | Reversed           |  48,120.7 ns |    526.05 ns |   275.13 ns |   0.27 |    0.00 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Reversed           |   6,461.1 ns |    214.73 ns |   112.31 ns |   0.04 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Reversed           |   6,639.3 ns |    554.31 ns |   289.91 ns |   0.04 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Reversed           |   7,287.3 ns |    393.79 ns |   205.96 ns |   0.04 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Reversed           |   6,995.1 ns |    486.10 ns |   254.24 ns |   0.04 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Reversed           |   7,251.8 ns |    297.54 ns |   132.11 ns |   0.04 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |        |         |      |           |             |
| **InsertionSort**          | **1024** | **PipeOrgan**          |  **89,941.8 ns** |    **237.06 ns** |    **84.54 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | PipeOrgan          | 124,064.3 ns |  2,820.92 ns | 1,475.40 ns |   1.38 |    0.02 |    5 |         - |          NA |
| BinaryInsertSort       | 1024 | PipeOrgan          |  18,829.4 ns |    371.29 ns |   194.19 ns |   0.21 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | PipeOrgan          | 383,438.0 ns | 10,524.11 ns | 5,504.31 ns |   4.26 |    0.06 |    6 |         - |          NA |
| LibrarySort            | 1024 | PipeOrgan          |  91,782.2 ns |  1,292.35 ns |   573.81 ns |   1.02 |    0.01 |    4 |         - |          NA |
| MergeInsertionSort     | 1024 | PipeOrgan          |  48,978.9 ns |    174.12 ns |    77.31 ns |   0.54 |    0.00 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | PipeOrgan          |   6,006.3 ns |    130.02 ns |    57.73 ns |   0.07 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | PipeOrgan          |   6,720.5 ns |    365.32 ns |   162.21 ns |   0.07 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | PipeOrgan          |   7,103.6 ns |    260.74 ns |   136.37 ns |   0.08 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | PipeOrgan          |   6,936.0 ns |    234.86 ns |   104.28 ns |   0.08 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | PipeOrgan          |   7,250.7 ns |    213.35 ns |   111.59 ns |   0.08 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |             |        |         |      |           |             |
| **InsertionSort**          | **1024** | **ManyDuplicates**     |  **91,958.5 ns** |    **571.81 ns** |   **203.91 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | ManyDuplicates     | 119,574.8 ns |  3,847.60 ns | 2,012.37 ns |   1.30 |    0.02 |    3 |         - |          NA |
| BinaryInsertSort       | 1024 | ManyDuplicates     |  60,674.1 ns |    177.96 ns |    79.02 ns |   0.66 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | ManyDuplicates     | 378,122.9 ns |  2,179.17 ns |   967.57 ns |   4.11 |    0.01 |    4 |         - |          NA |
| LibrarySort            | 1024 | ManyDuplicates     | 112,156.2 ns |    731.98 ns |   261.03 ns |   1.22 |    0.00 |    3 |         - |          NA |
| MergeInsertionSort     | 1024 | ManyDuplicates     | 105,439.9 ns |    516.08 ns |   229.14 ns |   1.15 |    0.00 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | ManyDuplicates     |  35,413.9 ns |    197.48 ns |    87.68 ns |   0.39 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | ManyDuplicates     |  35,801.9 ns |    211.23 ns |   110.48 ns |   0.39 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | ManyDuplicates     |  37,024.7 ns |    991.27 ns |   518.45 ns |   0.40 |    0.01 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | ManyDuplicates     |  35,743.6 ns |    132.74 ns |    69.42 ns |   0.39 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | ManyDuplicates     |  35,506.5 ns |    142.86 ns |    74.72 ns |   0.39 |    0.00 |    1 |         - |          NA |

### MergeBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.58 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                   | Size | Pattern            | Mean           | Error        | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------------- |----- |------------------- |---------------:|-------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **MergeSort**                | **256**  | **Random**             |    **13,517.6 ns** |    **217.23 ns** |   **113.61 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Random             |    12,435.0 ns |    341.83 ns |   178.78 ns |  0.92 |    0.01 |    3 |         - |          NA |
| BottomupMergeSort        | 256  | Random             |     9,229.3 ns |     95.49 ns |    49.94 ns |  0.68 |    0.01 |    3 |         - |          NA |
| StdStableSort            | 256  | Random             |     5,364.6 ns |    252.66 ns |   132.15 ns |  0.40 |    0.01 |    2 |         - |          NA |
| RotateMergeSort          | 256  | Random             |    17,384.7 ns |    521.51 ns |   272.76 ns |  1.29 |    0.02 |    4 |         - |          NA |
| RotateMergeSortRecursive | 256  | Random             |    18,860.0 ns |    267.87 ns |   118.94 ns |  1.40 |    0.01 |    4 |         - |          NA |
| SymMergeSort             | 256  | Random             |    12,462.3 ns |    457.10 ns |   202.96 ns |  0.92 |    0.02 |    3 |         - |          NA |
| BlockMergeSort           | 256  | Random             |     9,788.4 ns |    313.45 ns |   163.94 ns |  0.72 |    0.01 |    3 |         - |          NA |
| NaturalMergeSort         | 256  | Random             |    11,610.3 ns |    237.22 ns |   124.07 ns |  0.86 |    0.01 |    3 |         - |          NA |
| PatienceSort             | 256  | Random             |    13,174.0 ns |    222.38 ns |    98.74 ns |  0.97 |    0.01 |    3 |         - |          NA |
| DropMergeSort            | 256  | Random             |     9,395.8 ns |    311.36 ns |   162.85 ns |  0.70 |    0.01 |    3 |         - |          NA |
| TimSort                  | 256  | Random             |     9,759.1 ns |    250.24 ns |   111.11 ns |  0.72 |    0.01 |    3 |         - |          NA |
| PowerSort                | 256  | Random             |     6,710.0 ns |    272.19 ns |   142.36 ns |  0.50 |    0.01 |    2 |         - |          NA |
| ShiftSort                | 256  | Random             |    10,710.4 ns |    297.13 ns |   131.93 ns |  0.79 |    0.01 |    3 |         - |          NA |
| SpinSort                 | 256  | Random             |     5,922.6 ns |    281.44 ns |   147.20 ns |  0.44 |    0.01 |    2 |         - |          NA |
| Glidesort                | 256  | Random             |     5,416.5 ns |     90.38 ns |    40.13 ns |  0.40 |    0.00 |    2 |         - |          NA |
| Driftsort                | 256  | Random             |     4,986.8 ns |    324.16 ns |   169.54 ns |  0.37 |    0.01 |    2 |         - |          NA |
| FlatStableSort           | 256  | Random             |     6,355.4 ns |    273.35 ns |   142.97 ns |  0.47 |    0.01 |    2 |         - |          NA |
| QuadSort                 | 256  | Random             |     3,528.0 ns |    343.09 ns |   179.44 ns |  0.26 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **256**  | **SingleElementMoved** |     **3,679.0 ns** |    **326.96 ns** |   **171.00 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | SingleElementMoved |     4,317.8 ns |    179.14 ns |    79.54 ns |  1.18 |    0.05 |    6 |         - |          NA |
| BottomupMergeSort        | 256  | SingleElementMoved |     1,380.6 ns |     68.39 ns |    30.36 ns |  0.38 |    0.02 |    4 |         - |          NA |
| StdStableSort            | 256  | SingleElementMoved |       568.2 ns |     10.52 ns |     4.67 ns |  0.15 |    0.01 |    2 |         - |          NA |
| RotateMergeSort          | 256  | SingleElementMoved |       738.4 ns |    285.75 ns |   149.45 ns |  0.20 |    0.04 |    2 |         - |          NA |
| RotateMergeSortRecursive | 256  | SingleElementMoved |       526.3 ns |     45.86 ns |    20.36 ns |  0.14 |    0.01 |    2 |         - |          NA |
| SymMergeSort             | 256  | SingleElementMoved |       411.1 ns |     18.43 ns |     9.64 ns |  0.11 |    0.01 |    1 |         - |          NA |
| BlockMergeSort           | 256  | SingleElementMoved |     2,263.0 ns |     40.30 ns |    14.37 ns |  0.62 |    0.03 |    5 |         - |          NA |
| NaturalMergeSort         | 256  | SingleElementMoved |       515.0 ns |     17.02 ns |     8.90 ns |  0.14 |    0.01 |    2 |         - |          NA |
| PatienceSort             | 256  | SingleElementMoved |     7,171.4 ns |    438.37 ns |   194.64 ns |  1.95 |    0.10 |    7 |         - |          NA |
| DropMergeSort            | 256  | SingleElementMoved |       535.8 ns |      8.11 ns |     4.24 ns |  0.15 |    0.01 |    2 |         - |          NA |
| TimSort                  | 256  | SingleElementMoved |       298.0 ns |     40.70 ns |    21.29 ns |  0.08 |    0.01 |    1 |         - |          NA |
| PowerSort                | 256  | SingleElementMoved |       341.8 ns |     13.29 ns |     5.90 ns |  0.09 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | SingleElementMoved |       508.4 ns |     15.93 ns |     7.07 ns |  0.14 |    0.01 |    2 |         - |          NA |
| SpinSort                 | 256  | SingleElementMoved |       696.4 ns |     86.22 ns |    45.10 ns |  0.19 |    0.01 |    2 |         - |          NA |
| Glidesort                | 256  | SingleElementMoved |       673.0 ns |      8.37 ns |     3.72 ns |  0.18 |    0.01 |    2 |         - |          NA |
| Driftsort                | 256  | SingleElementMoved |       574.0 ns |     49.88 ns |    22.15 ns |  0.16 |    0.01 |    2 |         - |          NA |
| FlatStableSort           | 256  | SingleElementMoved |       971.8 ns |     23.72 ns |    10.53 ns |  0.26 |    0.01 |    3 |         - |          NA |
| QuadSort                 | 256  | SingleElementMoved |       518.1 ns |     27.88 ns |    12.38 ns |  0.14 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **256**  | **Sorted**             |     **3,303.4 ns** |     **32.95 ns** |    **11.75 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Sorted             |     4,224.6 ns |    297.09 ns |   155.38 ns |  1.28 |    0.04 |    9 |         - |          NA |
| BottomupMergeSort        | 256  | Sorted             |     1,164.8 ns |     21.26 ns |    11.12 ns |  0.35 |    0.00 |    6 |         - |          NA |
| StdStableSort            | 256  | Sorted             |       469.9 ns |      4.23 ns |     1.88 ns |  0.14 |    0.00 |    4 |         - |          NA |
| RotateMergeSort          | 256  | Sorted             |       251.4 ns |     20.41 ns |    10.67 ns |  0.08 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 256  | Sorted             |       279.1 ns |      9.68 ns |     4.30 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 256  | Sorted             |       253.8 ns |      3.45 ns |     1.81 ns |  0.08 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 256  | Sorted             |     1,976.7 ns |     46.06 ns |    16.43 ns |  0.60 |    0.01 |    7 |         - |          NA |
| NaturalMergeSort         | 256  | Sorted             |       193.4 ns |     20.86 ns |     9.26 ns |  0.06 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 256  | Sorted             |     6,416.4 ns |    149.02 ns |    77.94 ns |  1.94 |    0.02 |   10 |         - |          NA |
| DropMergeSort            | 256  | Sorted             |       459.3 ns |      6.94 ns |     3.63 ns |  0.14 |    0.00 |    4 |         - |          NA |
| TimSort                  | 256  | Sorted             |       160.8 ns |     15.20 ns |     7.95 ns |  0.05 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | Sorted             |       137.1 ns |      7.64 ns |     3.39 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Sorted             |       339.5 ns |     12.11 ns |     6.33 ns |  0.10 |    0.00 |    3 |         - |          NA |
| SpinSort                 | 256  | Sorted             |       147.0 ns |      5.99 ns |     3.13 ns |  0.04 |    0.00 |    1 |         - |          NA |
| Glidesort                | 256  | Sorted             |       169.3 ns |      3.53 ns |     1.57 ns |  0.05 |    0.00 |    1 |         - |          NA |
| Driftsort                | 256  | Sorted             |       173.9 ns |     10.50 ns |     5.49 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | Sorted             |       885.2 ns |      8.45 ns |     4.42 ns |  0.27 |    0.00 |    5 |         - |          NA |
| QuadSort                 | 256  | Sorted             |       248.0 ns |      9.15 ns |     4.06 ns |  0.08 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **256**  | **Reversed**           |     **7,193.4 ns** |    **294.05 ns** |   **153.79 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Reversed           |     6,421.2 ns |    377.77 ns |   167.73 ns |  0.89 |    0.03 |    6 |         - |          NA |
| BottomupMergeSort        | 256  | Reversed           |     4,011.5 ns |    303.46 ns |   158.71 ns |  0.56 |    0.02 |    5 |         - |          NA |
| StdStableSort            | 256  | Reversed           |     3,172.7 ns |     18.87 ns |     6.73 ns |  0.44 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 256  | Reversed           |     1,618.6 ns |     14.02 ns |     6.22 ns |  0.23 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 256  | Reversed           |     1,596.2 ns |     25.10 ns |    11.15 ns |  0.22 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 256  | Reversed           |     1,867.7 ns |    288.82 ns |   151.06 ns |  0.26 |    0.02 |    3 |         - |          NA |
| BlockMergeSort           | 256  | Reversed           |     2,433.8 ns |     15.58 ns |     6.92 ns |  0.34 |    0.01 |    4 |         - |          NA |
| NaturalMergeSort         | 256  | Reversed           |       226.2 ns |      1.70 ns |     0.61 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 256  | Reversed           |     1,296.1 ns |     12.04 ns |     4.29 ns |  0.18 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 256  | Reversed           |     1,217.8 ns |     13.37 ns |     6.99 ns |  0.17 |    0.00 |    2 |         - |          NA |
| TimSort                  | 256  | Reversed           |       219.5 ns |     19.36 ns |     8.59 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | Reversed           |       186.7 ns |      4.98 ns |     2.61 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Reversed           |     3,441.4 ns |     24.26 ns |    10.77 ns |  0.48 |    0.01 |    5 |         - |          NA |
| SpinSort                 | 256  | Reversed           |       221.7 ns |     18.64 ns |     8.28 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 256  | Reversed           |       258.9 ns |      6.78 ns |     3.01 ns |  0.04 |    0.00 |    1 |         - |          NA |
| Driftsort                | 256  | Reversed           |       253.4 ns |      5.99 ns |     3.14 ns |  0.04 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | Reversed           |     3,137.7 ns |  1,523.32 ns |   796.72 ns |  0.44 |    0.10 |    5 |         - |          NA |
| QuadSort                 | 256  | Reversed           |       216.4 ns |      5.10 ns |     2.27 ns |  0.03 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **256**  | **PipeOrgan**          |     **5,406.2 ns** |    **262.07 ns** |   **137.07 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | PipeOrgan          |     5,472.6 ns |    350.89 ns |   183.52 ns |  1.01 |    0.04 |    5 |         - |          NA |
| BottomupMergeSort        | 256  | PipeOrgan          |     2,632.2 ns |     19.68 ns |     8.74 ns |  0.49 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 256  | PipeOrgan          |     1,927.4 ns |     11.72 ns |     6.13 ns |  0.36 |    0.01 |    4 |         - |          NA |
| RotateMergeSort          | 256  | PipeOrgan          |     3,501.8 ns |    409.83 ns |   214.35 ns |  0.65 |    0.04 |    5 |         - |          NA |
| RotateMergeSortRecursive | 256  | PipeOrgan          |     4,052.1 ns |    244.27 ns |   127.76 ns |  0.75 |    0.03 |    5 |         - |          NA |
| SymMergeSort             | 256  | PipeOrgan          |     2,723.3 ns |    223.68 ns |   116.99 ns |  0.50 |    0.02 |    4 |         - |          NA |
| BlockMergeSort           | 256  | PipeOrgan          |     2,404.8 ns |     15.57 ns |     6.91 ns |  0.45 |    0.01 |    4 |         - |          NA |
| NaturalMergeSort         | 256  | PipeOrgan          |       549.9 ns |     23.77 ns |    10.55 ns |  0.10 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 256  | PipeOrgan          |     4,640.5 ns |    227.77 ns |   119.13 ns |  0.86 |    0.03 |    5 |         - |          NA |
| DropMergeSort            | 256  | PipeOrgan          |     4,947.3 ns |     72.55 ns |    25.87 ns |  0.92 |    0.02 |    5 |         - |          NA |
| TimSort                  | 256  | PipeOrgan          |       608.6 ns |      8.55 ns |     4.47 ns |  0.11 |    0.00 |    2 |         - |          NA |
| PowerSort                | 256  | PipeOrgan          |       407.2 ns |     13.16 ns |     5.84 ns |  0.08 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | PipeOrgan          |     2,204.7 ns |     18.57 ns |     8.25 ns |  0.41 |    0.01 |    4 |         - |          NA |
| SpinSort                 | 256  | PipeOrgan          |     1,602.0 ns |     24.08 ns |    10.69 ns |  0.30 |    0.01 |    4 |         - |          NA |
| Glidesort                | 256  | PipeOrgan          |       998.6 ns |      6.21 ns |     2.76 ns |  0.18 |    0.00 |    3 |         - |          NA |
| Driftsort                | 256  | PipeOrgan          |       372.1 ns |     15.40 ns |     6.84 ns |  0.07 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | PipeOrgan          |     1,648.2 ns |     13.19 ns |     5.86 ns |  0.31 |    0.01 |    4 |         - |          NA |
| QuadSort                 | 256  | PipeOrgan          |       667.7 ns |     13.42 ns |     5.96 ns |  0.12 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **256**  | **ManyDuplicates**     |    **13,070.8 ns** |    **170.52 ns** |    **75.71 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | ManyDuplicates     |    12,052.5 ns |    183.78 ns |    96.12 ns |  0.92 |    0.01 |    3 |         - |          NA |
| BottomupMergeSort        | 256  | ManyDuplicates     |     9,007.7 ns |    132.17 ns |    69.13 ns |  0.69 |    0.01 |    3 |         - |          NA |
| StdStableSort            | 256  | ManyDuplicates     |     4,957.7 ns |    248.70 ns |   130.07 ns |  0.38 |    0.01 |    2 |         - |          NA |
| RotateMergeSort          | 256  | ManyDuplicates     |    16,410.0 ns |    192.44 ns |    68.63 ns |  1.26 |    0.01 |    4 |         - |          NA |
| RotateMergeSortRecursive | 256  | ManyDuplicates     |    18,130.5 ns |    651.78 ns |   289.39 ns |  1.39 |    0.02 |    4 |         - |          NA |
| SymMergeSort             | 256  | ManyDuplicates     |    12,009.1 ns |    251.57 ns |   111.70 ns |  0.92 |    0.01 |    3 |         - |          NA |
| BlockMergeSort           | 256  | ManyDuplicates     |     9,408.9 ns |    311.88 ns |   163.12 ns |  0.72 |    0.01 |    3 |         - |          NA |
| NaturalMergeSort         | 256  | ManyDuplicates     |    11,318.1 ns |    209.93 ns |   109.80 ns |  0.87 |    0.01 |    3 |         - |          NA |
| PatienceSort             | 256  | ManyDuplicates     |    10,985.0 ns |    260.86 ns |   115.82 ns |  0.84 |    0.01 |    3 |         - |          NA |
| DropMergeSort            | 256  | ManyDuplicates     |     8,135.0 ns |    330.43 ns |   146.71 ns |  0.62 |    0.01 |    3 |         - |          NA |
| TimSort                  | 256  | ManyDuplicates     |     9,297.7 ns |    319.07 ns |   166.88 ns |  0.71 |    0.01 |    3 |         - |          NA |
| PowerSort                | 256  | ManyDuplicates     |     6,311.6 ns |    185.59 ns |    82.40 ns |  0.48 |    0.01 |    2 |         - |          NA |
| ShiftSort                | 256  | ManyDuplicates     |    10,229.6 ns |    226.58 ns |   100.60 ns |  0.78 |    0.01 |    3 |         - |          NA |
| SpinSort                 | 256  | ManyDuplicates     |     5,392.6 ns |    218.47 ns |   114.27 ns |  0.41 |    0.01 |    2 |         - |          NA |
| Glidesort                | 256  | ManyDuplicates     |     5,236.4 ns |    400.06 ns |   209.24 ns |  0.40 |    0.02 |    2 |         - |          NA |
| Driftsort                | 256  | ManyDuplicates     |     4,718.5 ns |    417.43 ns |   218.32 ns |  0.36 |    0.02 |    2 |         - |          NA |
| FlatStableSort           | 256  | ManyDuplicates     |     5,601.1 ns |    378.66 ns |   168.13 ns |  0.43 |    0.01 |    2 |         - |          NA |
| QuadSort                 | 256  | ManyDuplicates     |     3,500.4 ns |    330.56 ns |   146.77 ns |  0.27 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **1024** | **Random**             |    **63,304.1 ns** |    **495.17 ns** |   **258.98 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Random             |    57,772.6 ns |    678.04 ns |   301.05 ns |  0.91 |    0.01 |    3 |         - |          NA |
| BottomupMergeSort        | 1024 | Random             |    44,414.2 ns |    785.63 ns |   410.90 ns |  0.70 |    0.01 |    3 |         - |          NA |
| StdStableSort            | 1024 | Random             |    29,490.0 ns |  1,125.32 ns |   588.56 ns |  0.47 |    0.01 |    2 |         - |          NA |
| RotateMergeSort          | 1024 | Random             |   100,496.8 ns |  1,272.66 ns |   565.07 ns |  1.59 |    0.01 |    4 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Random             |   110,155.8 ns |    500.90 ns |   222.40 ns |  1.74 |    0.01 |    4 |         - |          NA |
| SymMergeSort             | 1024 | Random             |    72,802.7 ns |    731.46 ns |   382.57 ns |  1.15 |    0.01 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | Random             |    49,267.8 ns |    160.92 ns |    57.38 ns |  0.78 |    0.00 |    3 |         - |          NA |
| NaturalMergeSort         | 1024 | Random             |    55,847.1 ns |    266.80 ns |   118.46 ns |  0.88 |    0.00 |    3 |         - |          NA |
| PatienceSort             | 1024 | Random             |    64,656.3 ns |    338.85 ns |   150.45 ns |  1.02 |    0.00 |    3 |         - |          NA |
| DropMergeSort            | 1024 | Random             |    43,569.2 ns |    454.88 ns |   237.91 ns |  0.69 |    0.00 |    3 |         - |          NA |
| TimSort                  | 1024 | Random             |    48,359.9 ns |    160.94 ns |    57.39 ns |  0.76 |    0.00 |    3 |         - |          NA |
| PowerSort                | 1024 | Random             |    34,881.2 ns |    346.31 ns |   153.77 ns |  0.55 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 1024 | Random             |    52,445.1 ns |    363.76 ns |   161.51 ns |  0.83 |    0.00 |    3 |         - |          NA |
| SpinSort                 | 1024 | Random             |    31,467.6 ns |    492.60 ns |   257.64 ns |  0.50 |    0.00 |    2 |         - |          NA |
| Glidesort                | 1024 | Random             |    23,040.0 ns |    280.09 ns |   146.49 ns |  0.36 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | Random             |    20,913.6 ns |    241.24 ns |   107.11 ns |  0.33 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Random             |    33,072.5 ns |    546.50 ns |   285.83 ns |  0.52 |    0.00 |    2 |         - |          NA |
| QuadSort                 | 1024 | Random             |    17,861.0 ns |    168.11 ns |    87.92 ns |  0.28 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **1024** | **SingleElementMoved** |    **13,974.6 ns** |    **269.02 ns** |   **140.70 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | SingleElementMoved |    17,166.9 ns |    251.07 ns |   131.31 ns |  1.23 |    0.01 |    8 |         - |          NA |
| BottomupMergeSort        | 1024 | SingleElementMoved |     5,196.3 ns |    291.48 ns |   152.45 ns |  0.37 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 1024 | SingleElementMoved |     2,711.7 ns |    276.06 ns |   144.38 ns |  0.19 |    0.01 |    3 |         - |          NA |
| RotateMergeSort          | 1024 | SingleElementMoved |     1,393.3 ns |     65.23 ns |    34.12 ns |  0.10 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 1024 | SingleElementMoved |     1,592.0 ns |     63.22 ns |    28.07 ns |  0.11 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 1024 | SingleElementMoved |     1,223.1 ns |     39.21 ns |    20.51 ns |  0.09 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 1024 | SingleElementMoved |     9,636.5 ns |    266.97 ns |   139.63 ns |  0.69 |    0.01 |    6 |         - |          NA |
| NaturalMergeSort         | 1024 | SingleElementMoved |     1,744.2 ns |     17.35 ns |     7.70 ns |  0.12 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 1024 | SingleElementMoved |    38,519.9 ns |    979.02 ns |   512.05 ns |  2.76 |    0.04 |    9 |         - |          NA |
| DropMergeSort            | 1024 | SingleElementMoved |     1,869.0 ns |     22.70 ns |    10.08 ns |  0.13 |    0.00 |    2 |         - |          NA |
| TimSort                  | 1024 | SingleElementMoved |       738.7 ns |     31.08 ns |    13.80 ns |  0.05 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | SingleElementMoved |     1,049.4 ns |     12.90 ns |     5.73 ns |  0.08 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 1024 | SingleElementMoved |     1,780.1 ns |     24.40 ns |    12.76 ns |  0.13 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 1024 | SingleElementMoved |     2,863.6 ns |    155.74 ns |    69.15 ns |  0.20 |    0.01 |    3 |         - |          NA |
| Glidesort                | 1024 | SingleElementMoved |     1,857.3 ns |     17.06 ns |     7.57 ns |  0.13 |    0.00 |    2 |         - |          NA |
| Driftsort                | 1024 | SingleElementMoved |     1,094.4 ns |     18.03 ns |     9.43 ns |  0.08 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 1024 | SingleElementMoved |     4,088.5 ns |     48.37 ns |    25.30 ns |  0.29 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 1024 | SingleElementMoved |     1,209.2 ns |     36.40 ns |    12.98 ns |  0.09 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **1024** | **Sorted**             |    **13,346.4 ns** |    **231.41 ns** |   **121.03 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Sorted             |    16,640.2 ns |    199.48 ns |   104.33 ns |  1.25 |    0.01 |    9 |         - |          NA |
| BottomupMergeSort        | 1024 | Sorted             |     4,509.3 ns |     14.72 ns |     6.54 ns |  0.34 |    0.00 |    6 |         - |          NA |
| StdStableSort            | 1024 | Sorted             |     2,413.3 ns |     16.59 ns |     5.92 ns |  0.18 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 1024 | Sorted             |       917.2 ns |      5.01 ns |     2.23 ns |  0.07 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Sorted             |     1,099.8 ns |      4.24 ns |     1.88 ns |  0.08 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 1024 | Sorted             |       905.4 ns |      7.78 ns |     3.45 ns |  0.07 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | Sorted             |     8,426.5 ns |    228.92 ns |   119.73 ns |  0.63 |    0.01 |    7 |         - |          NA |
| NaturalMergeSort         | 1024 | Sorted             |       532.5 ns |      7.49 ns |     3.32 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 1024 | Sorted             |    32,049.7 ns |  1,423.84 ns |   632.19 ns |  2.40 |    0.05 |   10 |         - |          NA |
| DropMergeSort            | 1024 | Sorted             |     1,725.2 ns |     15.16 ns |     6.73 ns |  0.13 |    0.00 |    4 |         - |          NA |
| TimSort                  | 1024 | Sorted             |       484.7 ns |      8.40 ns |     3.73 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Sorted             |       467.0 ns |      4.54 ns |     2.01 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Sorted             |     1,170.5 ns |      4.42 ns |     1.58 ns |  0.09 |    0.00 |    3 |         - |          NA |
| SpinSort                 | 1024 | Sorted             |       422.0 ns |      5.81 ns |     2.58 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Sorted             |       472.7 ns |     18.31 ns |     9.58 ns |  0.04 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | Sorted             |       513.6 ns |     16.84 ns |     7.48 ns |  0.04 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Sorted             |     3,873.6 ns |    269.27 ns |   140.83 ns |  0.29 |    0.01 |    6 |         - |          NA |
| QuadSort                 | 1024 | Sorted             |       688.7 ns |     10.98 ns |     4.88 ns |  0.05 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **1024** | **Reversed**           |    **30,139.6 ns** |  **1,195.23 ns** |   **530.69 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Reversed           |    26,527.1 ns |    499.36 ns |   221.72 ns |  0.88 |    0.02 |    6 |         - |          NA |
| BottomupMergeSort        | 1024 | Reversed           |    16,525.7 ns |    162.00 ns |    71.93 ns |  0.55 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 1024 | Reversed           |    13,696.9 ns |    320.38 ns |   142.25 ns |  0.45 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 1024 | Reversed           |     7,687.7 ns |    245.09 ns |   128.19 ns |  0.26 |    0.01 |    4 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Reversed           |     7,553.3 ns |    318.83 ns |   166.75 ns |  0.25 |    0.01 |    4 |         - |          NA |
| SymMergeSort             | 1024 | Reversed           |     7,587.2 ns |    299.92 ns |   133.17 ns |  0.25 |    0.01 |    4 |         - |          NA |
| BlockMergeSort           | 1024 | Reversed           |    11,851.5 ns |     71.67 ns |    37.48 ns |  0.39 |    0.01 |    5 |         - |          NA |
| NaturalMergeSort         | 1024 | Reversed           |       780.1 ns |     12.83 ns |     6.71 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 1024 | Reversed           |     4,283.8 ns |     10.01 ns |     5.24 ns |  0.14 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 1024 | Reversed           |     5,382.3 ns |     40.03 ns |    20.94 ns |  0.18 |    0.00 |    3 |         - |          NA |
| TimSort                  | 1024 | Reversed           |       731.7 ns |      5.49 ns |     2.44 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Reversed           |       721.1 ns |     21.38 ns |     9.49 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Reversed           |    14,781.0 ns |     55.95 ns |    29.26 ns |  0.49 |    0.01 |    5 |         - |          NA |
| SpinSort                 | 1024 | Reversed           |       764.4 ns |     18.61 ns |     9.73 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Reversed           |       899.1 ns |      4.06 ns |     1.80 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | Reversed           |       759.8 ns |      9.28 ns |     3.31 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Reversed           |     8,973.6 ns |    240.19 ns |   125.62 ns |  0.30 |    0.01 |    4 |         - |          NA |
| QuadSort                 | 1024 | Reversed           |       763.0 ns |      7.77 ns |     2.77 ns |  0.03 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **1024** | **PipeOrgan**          |    **22,192.7 ns** |    **898.61 ns** |   **469.99 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | PipeOrgan          |    22,123.0 ns |    350.34 ns |   155.55 ns |  1.00 |    0.02 |    6 |         - |          NA |
| BottomupMergeSort        | 1024 | PipeOrgan          |    11,359.9 ns |    775.40 ns |   405.55 ns |  0.51 |    0.02 |    4 |         - |          NA |
| StdStableSort            | 1024 | PipeOrgan          |     8,449.0 ns |    344.66 ns |   180.26 ns |  0.38 |    0.01 |    4 |         - |          NA |
| RotateMergeSort          | 1024 | PipeOrgan          |    14,810.3 ns |    186.26 ns |    97.42 ns |  0.67 |    0.01 |    5 |         - |          NA |
| RotateMergeSortRecursive | 1024 | PipeOrgan          |    17,200.5 ns |    359.17 ns |   159.48 ns |  0.78 |    0.02 |    5 |         - |          NA |
| SymMergeSort             | 1024 | PipeOrgan          |    11,605.5 ns |    400.68 ns |   209.56 ns |  0.52 |    0.01 |    4 |         - |          NA |
| BlockMergeSort           | 1024 | PipeOrgan          |    11,987.9 ns |    339.53 ns |   177.58 ns |  0.54 |    0.01 |    4 |         - |          NA |
| NaturalMergeSort         | 1024 | PipeOrgan          |     2,032.7 ns |     34.75 ns |    15.43 ns |  0.09 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 1024 | PipeOrgan          |    22,555.0 ns |    594.47 ns |   310.92 ns |  1.02 |    0.02 |    6 |         - |          NA |
| DropMergeSort            | 1024 | PipeOrgan          |    21,601.2 ns |    347.87 ns |   154.45 ns |  0.97 |    0.02 |    6 |         - |          NA |
| TimSort                  | 1024 | PipeOrgan          |     2,137.8 ns |    139.59 ns |    61.98 ns |  0.10 |    0.00 |    2 |         - |          NA |
| PowerSort                | 1024 | PipeOrgan          |     1,370.7 ns |      9.98 ns |     4.43 ns |  0.06 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | PipeOrgan          |     9,518.7 ns |    322.58 ns |   168.71 ns |  0.43 |    0.01 |    4 |         - |          NA |
| SpinSort                 | 1024 | PipeOrgan          |     6,966.2 ns |    317.77 ns |   141.09 ns |  0.31 |    0.01 |    4 |         - |          NA |
| Glidesort                | 1024 | PipeOrgan          |     3,936.0 ns |    272.16 ns |   142.35 ns |  0.18 |    0.01 |    3 |         - |          NA |
| Driftsort                | 1024 | PipeOrgan          |     1,194.0 ns |     13.38 ns |     5.94 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | PipeOrgan          |     7,181.2 ns |    279.46 ns |   146.16 ns |  0.32 |    0.01 |    4 |         - |          NA |
| QuadSort                 | 1024 | PipeOrgan          |     2,367.3 ns |     17.12 ns |     7.60 ns |  0.11 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **1024** | **ManyDuplicates**     |    **56,280.4 ns** |    **534.41 ns** |   **279.51 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | ManyDuplicates     |    51,905.9 ns |    823.16 ns |   365.49 ns |  0.92 |    0.01 |    4 |         - |          NA |
| BottomupMergeSort        | 1024 | ManyDuplicates     |    38,627.7 ns |    940.53 ns |   417.60 ns |  0.69 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 1024 | ManyDuplicates     |    22,842.9 ns |    262.20 ns |   137.13 ns |  0.41 |    0.00 |    3 |         - |          NA |
| RotateMergeSort          | 1024 | ManyDuplicates     |    83,643.8 ns |    938.90 ns |   416.88 ns |  1.49 |    0.01 |    5 |         - |          NA |
| RotateMergeSortRecursive | 1024 | ManyDuplicates     |    91,348.9 ns |  1,226.87 ns |   544.74 ns |  1.62 |    0.01 |    5 |         - |          NA |
| SymMergeSort             | 1024 | ManyDuplicates     |    63,026.2 ns |    773.86 ns |   404.74 ns |  1.12 |    0.01 |    4 |         - |          NA |
| BlockMergeSort           | 1024 | ManyDuplicates     |    46,346.0 ns |    173.44 ns |    77.01 ns |  0.82 |    0.00 |    4 |         - |          NA |
| NaturalMergeSort         | 1024 | ManyDuplicates     |    49,738.7 ns |    761.30 ns |   398.17 ns |  0.88 |    0.01 |    4 |         - |          NA |
| PatienceSort             | 1024 | ManyDuplicates     |    40,941.3 ns |    250.72 ns |   111.32 ns |  0.73 |    0.00 |    4 |         - |          NA |
| DropMergeSort            | 1024 | ManyDuplicates     |    31,165.1 ns |    383.64 ns |   170.34 ns |  0.55 |    0.00 |    3 |         - |          NA |
| TimSort                  | 1024 | ManyDuplicates     |    42,458.5 ns |  1,397.86 ns |   620.66 ns |  0.75 |    0.01 |    4 |         - |          NA |
| PowerSort                | 1024 | ManyDuplicates     |    28,356.7 ns |  1,029.24 ns |   456.99 ns |  0.50 |    0.01 |    3 |         - |          NA |
| ShiftSort                | 1024 | ManyDuplicates     |    45,026.7 ns |    278.54 ns |   123.67 ns |  0.80 |    0.00 |    4 |         - |          NA |
| SpinSort                 | 1024 | ManyDuplicates     |    25,447.8 ns |    698.65 ns |   365.41 ns |  0.45 |    0.01 |    3 |         - |          NA |
| Glidesort                | 1024 | ManyDuplicates     |    13,081.2 ns |    177.49 ns |    78.80 ns |  0.23 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | ManyDuplicates     |    11,611.7 ns |    337.71 ns |   149.94 ns |  0.21 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | ManyDuplicates     |    25,585.6 ns |    900.87 ns |   471.17 ns |  0.45 |    0.01 |    3 |         - |          NA |
| QuadSort                 | 1024 | ManyDuplicates     |    17,986.8 ns |    200.61 ns |   104.92 ns |  0.32 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **4096** | **Random**             |   **290,476.2 ns** |  **1,445.13 ns** |   **755.83 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Random             |   262,196.5 ns |  2,696.59 ns | 1,197.30 ns |  0.90 |    0.00 |    2 |         - |          NA |
| BottomupMergeSort        | 4096 | Random             |   207,359.9 ns |  2,000.02 ns | 1,046.05 ns |  0.71 |    0.00 |    2 |         - |          NA |
| StdStableSort            | 4096 | Random             |   149,093.9 ns |  2,826.22 ns | 1,478.17 ns |  0.51 |    0.00 |    2 |         - |          NA |
| RotateMergeSort          | 4096 | Random             |   536,526.6 ns |  2,274.55 ns | 1,009.91 ns |  1.85 |    0.01 |    4 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Random             |   585,231.1 ns |  2,372.23 ns | 1,053.28 ns |  2.01 |    0.01 |    4 |         - |          NA |
| SymMergeSort             | 4096 | Random             |   385,256.2 ns |  1,103.14 ns |   489.80 ns |  1.33 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 4096 | Random             |   243,134.7 ns |    638.64 ns |   283.56 ns |  0.84 |    0.00 |    2 |         - |          NA |
| NaturalMergeSort         | 4096 | Random             |   261,306.2 ns |  1,006.29 ns |   446.80 ns |  0.90 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 4096 | Random             |   304,198.0 ns |  1,184.45 ns |   525.90 ns |  1.05 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 4096 | Random             |   197,650.6 ns |  1,006.63 ns |   526.49 ns |  0.68 |    0.00 |    2 |         - |          NA |
| TimSort                  | 4096 | Random             |   228,690.8 ns |  1,448.06 ns |   757.36 ns |  0.79 |    0.00 |    2 |         - |          NA |
| PowerSort                | 4096 | Random             |   175,330.8 ns |  1,017.65 ns |   451.84 ns |  0.60 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 4096 | Random             |   247,149.9 ns |  1,244.81 ns |   651.06 ns |  0.85 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 4096 | Random             |   156,275.3 ns |  1,098.36 ns |   574.46 ns |  0.54 |    0.00 |    2 |         - |          NA |
| Glidesort                | 4096 | Random             |    94,532.0 ns |  1,045.08 ns |   464.02 ns |  0.33 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | Random             |    86,188.6 ns |  2,043.84 ns |   907.48 ns |  0.30 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Random             |   160,259.4 ns |  1,593.51 ns |   833.44 ns |  0.55 |    0.00 |    2 |         - |          NA |
| QuadSort                 | 4096 | Random             |    82,498.9 ns |    343.69 ns |   152.60 ns |  0.28 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **4096** | **SingleElementMoved** |    **55,439.0 ns** |    **335.02 ns** |   **175.22 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | SingleElementMoved |    68,403.0 ns |    494.91 ns |   219.74 ns |  1.23 |    0.01 |    9 |         - |          NA |
| BottomupMergeSort        | 4096 | SingleElementMoved |    20,430.7 ns |    559.00 ns |   292.37 ns |  0.37 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 4096 | SingleElementMoved |    12,921.1 ns |    440.68 ns |   230.48 ns |  0.23 |    0.00 |    4 |         - |          NA |
| RotateMergeSort          | 4096 | SingleElementMoved |     4,779.7 ns |    274.36 ns |   121.82 ns |  0.09 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 4096 | SingleElementMoved |     5,431.4 ns |    234.13 ns |   122.45 ns |  0.10 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 4096 | SingleElementMoved |     4,345.5 ns |    202.99 ns |    90.13 ns |  0.08 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 4096 | SingleElementMoved |    38,727.2 ns |    479.51 ns |   212.90 ns |  0.70 |    0.00 |    7 |         - |          NA |
| NaturalMergeSort         | 4096 | SingleElementMoved |     6,718.8 ns |    289.09 ns |   151.20 ns |  0.12 |    0.00 |    3 |         - |          NA |
| PatienceSort             | 4096 | SingleElementMoved |   263,936.4 ns | 11,963.83 ns | 6,257.32 ns |  4.76 |    0.11 |   10 |         - |          NA |
| DropMergeSort            | 4096 | SingleElementMoved |     7,506.0 ns |    412.66 ns |   215.83 ns |  0.14 |    0.00 |    3 |         - |          NA |
| TimSort                  | 4096 | SingleElementMoved |     2,457.8 ns |     43.24 ns |    19.20 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | SingleElementMoved |     3,847.7 ns |     32.84 ns |    11.71 ns |  0.07 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 4096 | SingleElementMoved |     6,698.4 ns |    261.54 ns |   136.79 ns |  0.12 |    0.00 |    3 |         - |          NA |
| SpinSort                 | 4096 | SingleElementMoved |     8,366.4 ns |    206.82 ns |   108.17 ns |  0.15 |    0.00 |    3 |         - |          NA |
| Glidesort                | 4096 | SingleElementMoved |     6,875.0 ns |    273.07 ns |   121.25 ns |  0.12 |    0.00 |    3 |         - |          NA |
| Driftsort                | 4096 | SingleElementMoved |     3,688.3 ns |     23.09 ns |     8.23 ns |  0.07 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 4096 | SingleElementMoved |    16,107.2 ns |    155.04 ns |    81.09 ns |  0.29 |    0.00 |    5 |         - |          NA |
| QuadSort                 | 4096 | SingleElementMoved |     3,785.4 ns |    159.19 ns |    70.68 ns |  0.07 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **4096** | **Sorted**             |    **53,119.4 ns** |    **202.42 ns** |    **89.87 ns** |  **1.00** |    **0.00** |    **9** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Sorted             |    66,393.6 ns |    350.57 ns |   155.66 ns |  1.25 |    0.00 |   10 |         - |          NA |
| BottomupMergeSort        | 4096 | Sorted             |    18,415.2 ns |    134.95 ns |    70.58 ns |  0.35 |    0.00 |    7 |         - |          NA |
| StdStableSort            | 4096 | Sorted             |    12,036.0 ns |    197.32 ns |   103.20 ns |  0.23 |    0.00 |    6 |         - |          NA |
| RotateMergeSort          | 4096 | Sorted             |     3,634.2 ns |     18.41 ns |     6.57 ns |  0.07 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Sorted             |     4,405.1 ns |     60.05 ns |    21.41 ns |  0.08 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 4096 | Sorted             |     3,674.6 ns |    283.01 ns |   148.02 ns |  0.07 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 4096 | Sorted             |    33,625.8 ns |    174.10 ns |    91.06 ns |  0.63 |    0.00 |    8 |         - |          NA |
| NaturalMergeSort         | 4096 | Sorted             |     1,832.8 ns |      9.70 ns |     4.31 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 4096 | Sorted             |   173,034.3 ns | 21,899.38 ns | 9,723.46 ns |  3.26 |    0.17 |   11 |         - |          NA |
| DropMergeSort            | 4096 | Sorted             |     6,854.8 ns |    302.12 ns |   158.01 ns |  0.13 |    0.00 |    5 |         - |          NA |
| TimSort                  | 4096 | Sorted             |     1,805.1 ns |     23.52 ns |     8.39 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | Sorted             |     1,805.7 ns |     63.31 ns |    28.11 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | Sorted             |     4,519.5 ns |     78.39 ns |    27.96 ns |  0.09 |    0.00 |    4 |         - |          NA |
| SpinSort                 | 4096 | Sorted             |     1,558.3 ns |     14.43 ns |     7.55 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 4096 | Sorted             |     1,534.0 ns |     27.17 ns |     9.69 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | Sorted             |     1,853.0 ns |     57.44 ns |    25.50 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Sorted             |    13,929.5 ns |    168.42 ns |    88.09 ns |  0.26 |    0.00 |    6 |         - |          NA |
| QuadSort                 | 4096 | Sorted             |     2,387.2 ns |     22.48 ns |     8.01 ns |  0.04 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **4096** | **Reversed**           |   **124,175.7 ns** |  **1,476.98 ns** |   **655.79 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Reversed           |   112,014.8 ns |  1,458.73 ns |   762.94 ns |  0.90 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 4096 | Reversed           |    70,989.9 ns |  2,620.56 ns | 1,370.61 ns |  0.57 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 4096 | Reversed           |    57,921.1 ns |    373.99 ns |   133.37 ns |  0.47 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 4096 | Reversed           |    35,073.2 ns |    233.38 ns |   122.06 ns |  0.28 |    0.00 |    4 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Reversed           |    34,560.0 ns |    236.48 ns |   105.00 ns |  0.28 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 4096 | Reversed           |    32,317.6 ns |    320.38 ns |   142.25 ns |  0.26 |    0.00 |    4 |         - |          NA |
| BlockMergeSort           | 4096 | Reversed           |    52,622.3 ns |    449.24 ns |   234.96 ns |  0.42 |    0.00 |    5 |         - |          NA |
| NaturalMergeSort         | 4096 | Reversed           |     2,836.8 ns |     13.64 ns |     7.14 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 4096 | Reversed           |    16,804.5 ns |    142.99 ns |    74.78 ns |  0.14 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 4096 | Reversed           |    24,290.3 ns |    355.52 ns |   185.94 ns |  0.20 |    0.00 |    3 |         - |          NA |
| TimSort                  | 4096 | Reversed           |     2,892.7 ns |    254.82 ns |   133.27 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | Reversed           |     2,767.6 ns |     15.66 ns |     5.59 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | Reversed           |    62,123.6 ns |    456.93 ns |   202.88 ns |  0.50 |    0.00 |    5 |         - |          NA |
| SpinSort                 | 4096 | Reversed           |     2,959.2 ns |      8.78 ns |     3.13 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Glidesort                | 4096 | Reversed           |     3,438.1 ns |     17.28 ns |     7.67 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | Reversed           |     2,914.1 ns |    220.40 ns |   115.27 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Reversed           |    35,753.7 ns |    229.40 ns |   119.98 ns |  0.29 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 4096 | Reversed           |     3,015.2 ns |     44.47 ns |    15.86 ns |  0.02 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **4096** | **PipeOrgan**          |    **90,457.6 ns** |  **1,110.15 ns** |   **492.91 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | PipeOrgan          |    91,665.6 ns |    825.02 ns |   366.31 ns |  1.01 |    0.01 |    8 |         - |          NA |
| BottomupMergeSort        | 4096 | PipeOrgan          |    46,124.6 ns |  1,889.05 ns |   988.01 ns |  0.51 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 4096 | PipeOrgan          |    37,469.0 ns |  3,107.83 ns | 1,108.28 ns |  0.41 |    0.01 |    6 |         - |          NA |
| RotateMergeSort          | 4096 | PipeOrgan          |    63,919.5 ns |    202.81 ns |   106.07 ns |  0.71 |    0.00 |    7 |         - |          NA |
| RotateMergeSortRecursive | 4096 | PipeOrgan          |    73,143.7 ns |    233.85 ns |   103.83 ns |  0.81 |    0.00 |    7 |         - |          NA |
| SymMergeSort             | 4096 | PipeOrgan          |    48,538.9 ns |    907.82 ns |   474.81 ns |  0.54 |    0.01 |    6 |         - |          NA |
| BlockMergeSort           | 4096 | PipeOrgan          |    48,978.2 ns |    186.32 ns |    82.73 ns |  0.54 |    0.00 |    6 |         - |          NA |
| NaturalMergeSort         | 4096 | PipeOrgan          |     7,951.1 ns |    547.54 ns |   243.11 ns |  0.09 |    0.00 |    3 |         - |          NA |
| PatienceSort             | 4096 | PipeOrgan          |   130,926.8 ns |  8,960.91 ns | 4,686.73 ns |  1.45 |    0.05 |    9 |         - |          NA |
| DropMergeSort            | 4096 | PipeOrgan          |    90,241.5 ns |  1,131.40 ns |   591.74 ns |  1.00 |    0.01 |    8 |         - |          NA |
| TimSort                  | 4096 | PipeOrgan          |     8,320.4 ns |    315.35 ns |   164.93 ns |  0.09 |    0.00 |    3 |         - |          NA |
| PowerSort                | 4096 | PipeOrgan          |     5,207.2 ns |    216.10 ns |   113.02 ns |  0.06 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | PipeOrgan          |    39,647.2 ns |    434.98 ns |   193.13 ns |  0.44 |    0.00 |    6 |         - |          NA |
| SpinSort                 | 4096 | PipeOrgan          |     6,581.6 ns |    350.47 ns |   183.30 ns |  0.07 |    0.00 |    2 |         - |          NA |
| Glidesort                | 4096 | PipeOrgan          |    15,032.7 ns |    228.43 ns |   119.47 ns |  0.17 |    0.00 |    4 |         - |          NA |
| Driftsort                | 4096 | PipeOrgan          |     4,609.9 ns |    267.56 ns |   139.94 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | PipeOrgan          |    28,364.4 ns |    322.44 ns |   143.17 ns |  0.31 |    0.00 |    5 |         - |          NA |
| QuadSort                 | 4096 | PipeOrgan          |     9,197.9 ns |    259.05 ns |   135.49 ns |  0.10 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **4096** | **ManyDuplicates**     |   **234,548.6 ns** |  **7,213.61 ns** | **3,772.86 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | ManyDuplicates     |   214,281.4 ns |  2,325.61 ns | 1,216.34 ns |  0.91 |    0.01 |    3 |         - |          NA |
| BottomupMergeSort        | 4096 | ManyDuplicates     |   161,276.7 ns |  1,489.24 ns |   531.08 ns |  0.69 |    0.01 |    3 |         - |          NA |
| StdStableSort            | 4096 | ManyDuplicates     |   100,105.7 ns |  3,382.44 ns | 1,769.08 ns |  0.43 |    0.01 |    3 |         - |          NA |
| RotateMergeSort          | 4096 | ManyDuplicates     |   365,969.1 ns |  2,027.49 ns | 1,060.42 ns |  1.56 |    0.02 |    5 |         - |          NA |
| RotateMergeSortRecursive | 4096 | ManyDuplicates     |   402,919.2 ns |  2,155.93 ns | 1,127.59 ns |  1.72 |    0.03 |    5 |         - |          NA |
| SymMergeSort             | 4096 | ManyDuplicates     |   287,900.3 ns |  1,709.52 ns |   759.04 ns |  1.23 |    0.02 |    4 |         - |          NA |
| BlockMergeSort           | 4096 | ManyDuplicates     |   213,865.6 ns |  1,611.70 ns |   715.60 ns |  0.91 |    0.01 |    3 |         - |          NA |
| NaturalMergeSort         | 4096 | ManyDuplicates     |   208,623.9 ns |  1,547.90 ns |   809.58 ns |  0.89 |    0.01 |    3 |         - |          NA |
| PatienceSort             | 4096 | ManyDuplicates     |   140,105.1 ns |    585.94 ns |   306.46 ns |  0.60 |    0.01 |    3 |         - |          NA |
| DropMergeSort            | 4096 | ManyDuplicates     |   117,690.9 ns |    478.43 ns |   170.61 ns |  0.50 |    0.01 |    3 |         - |          NA |
| TimSort                  | 4096 | ManyDuplicates     |   177,199.5 ns |  1,725.39 ns |   766.09 ns |  0.76 |    0.01 |    3 |         - |          NA |
| PowerSort                | 4096 | ManyDuplicates     |   120,023.1 ns |  1,191.01 ns |   622.92 ns |  0.51 |    0.01 |    3 |         - |          NA |
| ShiftSort                | 4096 | ManyDuplicates     |   188,911.8 ns |    931.41 ns |   487.15 ns |  0.81 |    0.01 |    3 |         - |          NA |
| SpinSort                 | 4096 | ManyDuplicates     |   108,988.4 ns |  1,091.95 ns |   571.11 ns |  0.46 |    0.01 |    3 |         - |          NA |
| Glidesort                | 4096 | ManyDuplicates     |    19,121.0 ns |    212.94 ns |   111.37 ns |  0.08 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | ManyDuplicates     |    17,150.0 ns |    314.44 ns |   139.61 ns |  0.07 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | ManyDuplicates     |   109,982.1 ns |  1,100.59 ns |   575.63 ns |  0.47 |    0.01 |    3 |         - |          NA |
| QuadSort                 | 4096 | ManyDuplicates     |    79,010.9 ns |    757.68 ns |   336.41 ns |  0.34 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **8192** | **Random**             |   **617,423.3 ns** |  **1,350.81 ns** |   **599.77 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Random             |   555,589.9 ns |  3,521.75 ns | 1,841.94 ns |  0.90 |    0.00 |    2 |         - |          NA |
| BottomupMergeSort        | 8192 | Random             |   443,443.3 ns |  2,545.44 ns | 1,331.32 ns |  0.72 |    0.00 |    2 |         - |          NA |
| StdStableSort            | 8192 | Random             |   339,820.8 ns |  3,887.80 ns | 2,033.39 ns |  0.55 |    0.00 |    2 |         - |          NA |
| RotateMergeSort          | 8192 | Random             | 1,209,363.5 ns |  6,077.87 ns | 3,178.84 ns |  1.96 |    0.01 |    4 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Random             | 1,323,017.3 ns |  8,011.20 ns | 3,557.02 ns |  2.14 |    0.01 |    4 |         - |          NA |
| SymMergeSort             | 8192 | Random             |   868,950.9 ns |  3,989.41 ns | 1,771.32 ns |  1.41 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 8192 | Random             |   526,732.0 ns |  3,950.39 ns | 2,066.13 ns |  0.85 |    0.00 |    2 |         - |          NA |
| NaturalMergeSort         | 8192 | Random             |   563,118.5 ns |  2,313.71 ns | 1,210.11 ns |  0.91 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 8192 | Random             |   656,966.6 ns |  1,758.93 ns |   780.98 ns |  1.06 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 8192 | Random             |   420,889.8 ns |  3,040.53 ns | 1,590.26 ns |  0.68 |    0.00 |    2 |         - |          NA |
| TimSort                  | 8192 | Random             |   498,582.2 ns |  3,333.48 ns | 1,188.75 ns |  0.81 |    0.00 |    2 |         - |          NA |
| PowerSort                | 8192 | Random             |   384,893.7 ns |  1,451.43 ns |   759.13 ns |  0.62 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 8192 | Random             |   531,466.2 ns |  4,944.47 ns | 2,195.38 ns |  0.86 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | Random             |   344,328.4 ns |  4,266.15 ns | 1,894.20 ns |  0.56 |    0.00 |    2 |         - |          NA |
| Glidesort                | 8192 | Random             |   190,355.0 ns |  1,028.57 ns |   537.96 ns |  0.31 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Random             |   173,214.2 ns |    609.52 ns |   270.63 ns |  0.28 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Random             |   358,227.8 ns |  1,691.83 ns |   751.18 ns |  0.58 |    0.00 |    2 |         - |          NA |
| QuadSort                 | 8192 | Random             |   177,065.2 ns |    963.44 ns |   503.90 ns |  0.29 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **8192** | **SingleElementMoved** |   **111,377.0 ns** |  **1,436.51 ns** |   **637.82 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | SingleElementMoved |   136,929.7 ns |    484.04 ns |   214.92 ns |  1.23 |    0.01 |    8 |         - |          NA |
| BottomupMergeSort        | 8192 | SingleElementMoved |    43,208.0 ns |    953.87 ns |   498.89 ns |  0.39 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 8192 | SingleElementMoved |    24,353.0 ns |    583.10 ns |   258.90 ns |  0.22 |    0.00 |    4 |         - |          NA |
| RotateMergeSort          | 8192 | SingleElementMoved |     9,278.8 ns |    153.04 ns |    67.95 ns |  0.08 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 8192 | SingleElementMoved |    10,522.6 ns |    384.38 ns |   201.04 ns |  0.09 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 8192 | SingleElementMoved |     8,708.1 ns |    141.19 ns |    62.69 ns |  0.08 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 8192 | SingleElementMoved |    76,399.6 ns |    610.88 ns |   271.24 ns |  0.69 |    0.00 |    6 |         - |          NA |
| NaturalMergeSort         | 8192 | SingleElementMoved |    13,315.3 ns |    315.72 ns |   140.18 ns |  0.12 |    0.00 |    3 |         - |          NA |
| PatienceSort             | 8192 | SingleElementMoved |   621,774.8 ns |  2,203.85 ns |   978.53 ns |  5.58 |    0.03 |    9 |         - |          NA |
| DropMergeSort            | 8192 | SingleElementMoved |    14,815.4 ns |    362.09 ns |   189.38 ns |  0.13 |    0.00 |    3 |         - |          NA |
| TimSort                  | 8192 | SingleElementMoved |     4,804.7 ns |    156.75 ns |    81.98 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | SingleElementMoved |     7,916.4 ns |    196.38 ns |    87.19 ns |  0.07 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 8192 | SingleElementMoved |    13,516.4 ns |    427.35 ns |   223.51 ns |  0.12 |    0.00 |    3 |         - |          NA |
| SpinSort                 | 8192 | SingleElementMoved |    14,153.1 ns |    299.11 ns |   156.44 ns |  0.13 |    0.00 |    3 |         - |          NA |
| Glidesort                | 8192 | SingleElementMoved |    13,616.9 ns |    230.67 ns |   102.42 ns |  0.12 |    0.00 |    3 |         - |          NA |
| Driftsort                | 8192 | SingleElementMoved |     7,711.9 ns |    494.04 ns |   258.39 ns |  0.07 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 8192 | SingleElementMoved |    22,441.1 ns |    455.62 ns |   238.30 ns |  0.20 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 8192 | SingleElementMoved |     7,306.4 ns |    329.49 ns |   172.33 ns |  0.07 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **8192** | **Sorted**             |   **106,239.7 ns** |    **925.44 ns** |   **410.90 ns** |  **1.00** |    **0.00** |    **9** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Sorted             |   133,238.4 ns |    295.05 ns |   105.22 ns |  1.25 |    0.00 |   10 |         - |          NA |
| BottomupMergeSort        | 8192 | Sorted             |    39,568.6 ns |    406.69 ns |   212.71 ns |  0.37 |    0.00 |    7 |         - |          NA |
| StdStableSort            | 8192 | Sorted             |    23,492.1 ns |    451.39 ns |   200.42 ns |  0.22 |    0.00 |    6 |         - |          NA |
| RotateMergeSort          | 8192 | Sorted             |     7,423.9 ns |    363.95 ns |   190.35 ns |  0.07 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Sorted             |     9,099.8 ns |    617.69 ns |   323.06 ns |  0.09 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 8192 | Sorted             |     7,179.5 ns |     22.28 ns |    11.65 ns |  0.07 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 8192 | Sorted             |    66,526.6 ns |    283.28 ns |   148.16 ns |  0.63 |    0.00 |    8 |         - |          NA |
| NaturalMergeSort         | 8192 | Sorted             |     3,603.1 ns |     37.95 ns |    16.85 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 8192 | Sorted             |   615,886.9 ns | 12,711.42 ns | 5,643.95 ns |  5.80 |    0.05 |   11 |         - |          NA |
| DropMergeSort            | 8192 | Sorted             |    13,706.4 ns |    271.57 ns |   142.04 ns |  0.13 |    0.00 |    5 |         - |          NA |
| TimSort                  | 8192 | Sorted             |     3,570.1 ns |     49.75 ns |    22.09 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Sorted             |     3,560.4 ns |     41.14 ns |    14.67 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Sorted             |     9,154.3 ns |    126.50 ns |    66.16 ns |  0.09 |    0.00 |    4 |         - |          NA |
| SpinSort                 | 8192 | Sorted             |     3,115.8 ns |     48.72 ns |    21.63 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Sorted             |     3,171.5 ns |    243.63 ns |   127.42 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Sorted             |     3,686.9 ns |    203.08 ns |   106.21 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Sorted             |     3,239.3 ns |    164.73 ns |    73.14 ns |  0.03 |    0.00 |    1 |         - |          NA |
| QuadSort                 | 8192 | Sorted             |     4,919.6 ns |    495.37 ns |   259.09 ns |  0.05 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **8192** | **Reversed**           |   **251,688.3 ns** |  **4,002.25 ns** | **2,093.26 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Reversed           |   227,851.3 ns |  1,506.11 ns |   668.72 ns |  0.91 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 8192 | Reversed           |   134,552.3 ns |  3,186.70 ns | 1,414.92 ns |  0.53 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 8192 | Reversed           |   205,886.2 ns |  1,922.44 ns |   853.57 ns |  0.82 |    0.01 |    6 |         - |          NA |
| RotateMergeSort          | 8192 | Reversed           |    74,339.1 ns |    577.23 ns |   256.30 ns |  0.30 |    0.00 |    4 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Reversed           |    73,684.3 ns |    415.21 ns |   184.35 ns |  0.29 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 8192 | Reversed           |    66,773.4 ns |    515.75 ns |   229.00 ns |  0.27 |    0.00 |    4 |         - |          NA |
| BlockMergeSort           | 8192 | Reversed           |   108,974.7 ns |  1,135.41 ns |   504.13 ns |  0.43 |    0.00 |    5 |         - |          NA |
| NaturalMergeSort         | 8192 | Reversed           |     5,700.2 ns |    292.66 ns |   153.06 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 8192 | Reversed           |    33,241.8 ns |    221.95 ns |    98.55 ns |  0.13 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 8192 | Reversed           |    50,370.4 ns |    250.48 ns |   131.00 ns |  0.20 |    0.00 |    3 |         - |          NA |
| TimSort                  | 8192 | Reversed           |     5,660.1 ns |    306.59 ns |   160.35 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Reversed           |     5,570.8 ns |    192.39 ns |    85.42 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Reversed           |   128,299.0 ns |    388.24 ns |   172.38 ns |  0.51 |    0.00 |    5 |         - |          NA |
| SpinSort                 | 8192 | Reversed           |     6,251.5 ns |    728.38 ns |   380.96 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Reversed           |     6,870.0 ns |    298.98 ns |   156.37 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Reversed           |     5,793.4 ns |    448.83 ns |   234.75 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Reversed           |     6,182.4 ns |    452.07 ns |   236.44 ns |  0.02 |    0.00 |    1 |         - |          NA |
| QuadSort                 | 8192 | Reversed           |     6,103.1 ns |    348.77 ns |   182.41 ns |  0.02 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **8192** | **PipeOrgan**          |   **185,082.0 ns** |  **1,271.73 ns** |   **665.14 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | PipeOrgan          |   185,012.8 ns |    994.74 ns |   441.67 ns |  1.00 |    0.00 |    6 |         - |          NA |
| BottomupMergeSort        | 8192 | PipeOrgan          |    92,396.2 ns |  3,162.10 ns | 1,653.84 ns |  0.50 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 8192 | PipeOrgan          |   117,727.0 ns |  1,040.40 ns |   461.94 ns |  0.64 |    0.00 |    6 |         - |          NA |
| RotateMergeSort          | 8192 | PipeOrgan          |   131,672.6 ns |    333.16 ns |   147.93 ns |  0.71 |    0.00 |    6 |         - |          NA |
| RotateMergeSortRecursive | 8192 | PipeOrgan          |   152,266.0 ns |  3,086.43 ns | 1,614.26 ns |  0.82 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 8192 | PipeOrgan          |    99,576.2 ns |  2,750.22 ns | 1,221.12 ns |  0.54 |    0.01 |    6 |         - |          NA |
| BlockMergeSort           | 8192 | PipeOrgan          |   101,077.4 ns |  4,075.97 ns | 1,809.76 ns |  0.55 |    0.01 |    6 |         - |          NA |
| NaturalMergeSort         | 8192 | PipeOrgan          |    16,080.6 ns |    220.61 ns |   115.38 ns |  0.09 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 8192 | PipeOrgan          |   490,438.7 ns |  6,642.56 ns | 3,474.18 ns |  2.65 |    0.02 |    7 |         - |          NA |
| DropMergeSort            | 8192 | PipeOrgan          |   181,437.2 ns |  1,088.12 ns |   483.13 ns |  0.98 |    0.00 |    6 |         - |          NA |
| TimSort                  | 8192 | PipeOrgan          |    16,417.8 ns |    102.58 ns |    45.55 ns |  0.09 |    0.00 |    2 |         - |          NA |
| PowerSort                | 8192 | PipeOrgan          |    10,413.7 ns |    448.08 ns |   234.35 ns |  0.06 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | PipeOrgan          |    81,089.7 ns |    427.93 ns |   223.82 ns |  0.44 |    0.00 |    6 |         - |          NA |
| SpinSort                 | 8192 | PipeOrgan          |    13,890.2 ns |    588.02 ns |   307.55 ns |  0.08 |    0.00 |    2 |         - |          NA |
| Glidesort                | 8192 | PipeOrgan          |    30,108.9 ns |    512.28 ns |   267.93 ns |  0.16 |    0.00 |    4 |         - |          NA |
| Driftsort                | 8192 | PipeOrgan          |     9,034.8 ns |    371.10 ns |   194.09 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | PipeOrgan          |    55,679.8 ns |    576.80 ns |   301.68 ns |  0.30 |    0.00 |    5 |         - |          NA |
| QuadSort                 | 8192 | PipeOrgan          |    21,435.0 ns |  5,966.22 ns | 3,120.45 ns |  0.12 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **MergeSort**                | **8192** | **ManyDuplicates**     |   **473,998.2 ns** |  **3,024.39 ns** | **1,581.81 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | ManyDuplicates     |   435,090.0 ns |  3,206.03 ns | 1,423.50 ns |  0.92 |    0.00 |    4 |         - |          NA |
| BottomupMergeSort        | 8192 | ManyDuplicates     |   328,675.6 ns |  6,992.61 ns | 3,657.27 ns |  0.69 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 8192 | ManyDuplicates     |   222,688.8 ns |  9,064.01 ns | 4,740.65 ns |  0.47 |    0.01 |    4 |         - |          NA |
| RotateMergeSort          | 8192 | ManyDuplicates     |   753,014.1 ns |  3,319.06 ns | 1,735.93 ns |  1.59 |    0.01 |    6 |         - |          NA |
| RotateMergeSortRecursive | 8192 | ManyDuplicates     |   828,520.3 ns |  2,754.17 ns | 1,440.49 ns |  1.75 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 8192 | ManyDuplicates     |   602,400.9 ns |  2,237.39 ns |   993.42 ns |  1.27 |    0.00 |    5 |         - |          NA |
| BlockMergeSort           | 8192 | ManyDuplicates     |   439,048.8 ns |  1,526.23 ns |   798.25 ns |  0.93 |    0.00 |    4 |         - |          NA |
| NaturalMergeSort         | 8192 | ManyDuplicates     |   423,631.0 ns |  1,807.10 ns |   802.37 ns |  0.89 |    0.00 |    4 |         - |          NA |
| PatienceSort             | 8192 | ManyDuplicates     |   280,290.7 ns |  1,068.94 ns |   559.08 ns |  0.59 |    0.00 |    4 |         - |          NA |
| DropMergeSort            | 8192 | ManyDuplicates     |   233,726.2 ns |  1,350.83 ns |   706.51 ns |  0.49 |    0.00 |    4 |         - |          NA |
| TimSort                  | 8192 | ManyDuplicates     |   358,426.3 ns |    936.01 ns |   489.55 ns |  0.76 |    0.00 |    4 |         - |          NA |
| PowerSort                | 8192 | ManyDuplicates     |   246,519.7 ns |  1,789.64 ns |   794.61 ns |  0.52 |    0.00 |    4 |         - |          NA |
| ShiftSort                | 8192 | ManyDuplicates     |   384,482.5 ns |  1,339.36 ns |   594.68 ns |  0.81 |    0.00 |    4 |         - |          NA |
| SpinSort                 | 8192 | ManyDuplicates     |   223,040.8 ns |  2,391.67 ns | 1,061.92 ns |  0.47 |    0.00 |    4 |         - |          NA |
| Glidesort                | 8192 | ManyDuplicates     |    30,904.1 ns |    555.59 ns |   246.69 ns |  0.07 |    0.00 |    2 |         - |          NA |
| Driftsort                | 8192 | ManyDuplicates     |    24,807.8 ns |    336.77 ns |   176.14 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | ManyDuplicates     |   236,037.0 ns |  1,863.51 ns |   974.65 ns |  0.50 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 8192 | ManyDuplicates     |   161,077.4 ns |    385.84 ns |   171.31 ns |  0.34 |    0.00 |    3 |         - |          NA |

### MergeSlowBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.58 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method     | Size | Pattern            | Mean         | Error       | StdDev      | Ratio | Rank | Allocated | Alloc Ratio |
| ----------- |----- |------------------- |-------------:|------------:|------------:|------:|-----:|----------:|------------:|
| **StrandSort** | **256**  | **Random**             |   **9,266.5 ns** |   **310.42 ns** |   **162.35 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **256**  | **SingleElementMoved** |     **732.0 ns** |    **13.42 ns** |     **5.96 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **256**  | **Sorted**             |     **476.5 ns** |     **8.09 ns** |     **4.23 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **256**  | **Reversed**           |  **38,561.8 ns** |   **693.05 ns** |   **362.48 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **256**  | **PipeOrgan**          |  **23,121.2 ns** |   **987.02 ns** |   **438.24 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **256**  | **ManyDuplicates**     |   **8,011.7 ns** |   **322.85 ns** |   **168.86 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **1024** | **Random**             |  **57,829.0 ns** |   **199.52 ns** |    **88.59 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **1024** | **SingleElementMoved** |   **2,209.7 ns** |    **26.11 ns** |    **11.59 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **1024** | **Sorted**             |   **1,395.7 ns** |     **8.85 ns** |     **3.93 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **1024** | **Reversed**           | **620,582.2 ns** | **7,091.43 ns** | **3,708.95 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **1024** | **PipeOrgan**          | **372,898.8 ns** | **2,682.65 ns** | **1,191.12 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **1024** | **ManyDuplicates**     |  **35,467.9 ns** |   **183.58 ns** |    **96.02 ns** |  **1.00** |    **1** |         **-** |          **NA** |

### NetworkBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.58 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                  | Size | Pattern            | Mean           | Error        | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------------ |----- |------------------- |---------------:|-------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **BitonicSort**             | **256**  | **Random**             |    **15,773.2 ns** |    **256.66 ns** |   **113.96 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Random             |    26,278.7 ns |    455.68 ns |   238.33 ns |  1.67 |    0.02 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Random             |    11,153.7 ns |    224.13 ns |    99.52 ns |  0.71 |    0.01 |    1 |         - |          NA |
| PairwiseSortingNetwork  | 256  | Random             |    12,011.1 ns |    273.22 ns |   121.31 ns |  0.76 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **SingleElementMoved** |     **7,915.3 ns** |    **350.15 ns** |   **183.13 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | SingleElementMoved |    17,991.0 ns |    186.23 ns |    97.40 ns |  2.27 |    0.05 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | SingleElementMoved |     3,766.7 ns |     39.15 ns |    17.38 ns |  0.48 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 256  | SingleElementMoved |     3,053.7 ns |    430.57 ns |   225.20 ns |  0.39 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **Sorted**             |     **7,480.3 ns** |    **196.07 ns** |    **87.06 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Sorted             |    17,888.8 ns |    178.96 ns |    93.60 ns |  2.39 |    0.03 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Sorted             |     3,846.1 ns |    316.05 ns |   165.30 ns |  0.51 |    0.02 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 256  | Sorted             |     2,822.8 ns |    324.15 ns |   169.54 ns |  0.38 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **Reversed**           |     **7,099.1 ns** |    **246.99 ns** |   **129.18 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | Reversed           |    17,876.5 ns |    118.69 ns |    52.70 ns |  2.52 |    0.04 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | Reversed           |     3,675.3 ns |    341.86 ns |   178.80 ns |  0.52 |    0.03 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 256  | Reversed           |     2,932.1 ns |    292.23 ns |   152.84 ns |  0.41 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **PipeOrgan**          |     **7,036.9 ns** |    **284.45 ns** |   **148.78 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | PipeOrgan          |    17,835.6 ns |    115.49 ns |    60.40 ns |  2.54 |    0.05 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | PipeOrgan          |     3,472.3 ns |    149.59 ns |    66.42 ns |  0.49 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 256  | PipeOrgan          |     2,777.8 ns |    352.02 ns |   184.11 ns |  0.39 |    0.03 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **256**  | **ManyDuplicates**     |    **15,022.8 ns** |    **559.11 ns** |   **248.25 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicRecursiveSort    | 256  | ManyDuplicates     |    25,588.1 ns |    918.31 ns |   407.74 ns |  1.70 |    0.04 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 256  | ManyDuplicates     |    10,350.2 ns |    281.07 ns |   147.00 ns |  0.69 |    0.01 |    1 |         - |          NA |
| PairwiseSortingNetwork  | 256  | ManyDuplicates     |    11,011.6 ns |    425.35 ns |   188.86 ns |  0.73 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Random**             |    **88,681.2 ns** |  **1,678.91 ns** |   **878.10 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Random             |   137,317.1 ns |    855.92 ns |   447.66 ns |  1.55 |    0.02 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Random             |    57,996.3 ns |    399.55 ns |   177.40 ns |  0.65 |    0.01 |    1 |         - |          NA |
| PairwiseSortingNetwork  | 1024 | Random             |    66,547.8 ns |    749.81 ns |   392.16 ns |  0.75 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **SingleElementMoved** |    **46,890.0 ns** |  **2,549.55 ns** | **1,333.46 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | SingleElementMoved |    93,462.3 ns |    988.33 ns |   516.92 ns |  1.99 |    0.05 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | SingleElementMoved |    23,077.4 ns |  1,314.89 ns |   687.72 ns |  0.49 |    0.02 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 1024 | SingleElementMoved |    17,197.5 ns |  1,889.31 ns |   988.15 ns |  0.37 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Sorted**             |    **45,814.9 ns** |  **1,990.93 ns** |   **883.98 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Sorted             |    93,020.5 ns |    362.18 ns |   160.81 ns |  2.03 |    0.04 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Sorted             |    23,148.1 ns |    246.94 ns |   109.64 ns |  0.51 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 1024 | Sorted             |    16,850.0 ns |  1,682.05 ns |   879.75 ns |  0.37 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **Reversed**           |    **43,102.9 ns** |  **2,492.85 ns** | **1,106.84 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | Reversed           |    92,547.5 ns |    156.99 ns |    55.98 ns |  2.15 |    0.05 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | Reversed           |    22,246.9 ns |    674.88 ns |   352.98 ns |  0.52 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 1024 | Reversed           |    16,795.0 ns |  1,016.28 ns |   531.54 ns |  0.39 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **PipeOrgan**          |    **45,790.2 ns** |  **2,229.08 ns** | **1,165.85 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | PipeOrgan          |    92,419.2 ns |    168.23 ns |    87.99 ns |  2.02 |    0.05 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | PipeOrgan          |    21,980.2 ns |    993.48 ns |   519.61 ns |  0.48 |    0.02 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 1024 | PipeOrgan          |    16,778.3 ns |  1,540.32 ns |   805.62 ns |  0.37 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **1024** | **ManyDuplicates**     |    **78,109.3 ns** |  **3,045.67 ns** | **1,592.94 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicRecursiveSort    | 1024 | ManyDuplicates     |   127,042.5 ns |  1,145.50 ns |   599.12 ns |  1.63 |    0.03 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 1024 | ManyDuplicates     |    50,195.0 ns |    702.69 ns |   367.52 ns |  0.64 |    0.01 |    1 |         - |          NA |
| PairwiseSortingNetwork  | 1024 | ManyDuplicates     |    54,642.4 ns |    653.75 ns |   341.92 ns |  0.70 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Random**             |   **458,703.8 ns** |  **2,885.31 ns** | **1,281.10 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Random             |   683,937.2 ns |  2,762.34 ns | 1,444.76 ns |  1.49 |    0.00 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Random             |   298,493.3 ns |  5,021.80 ns | 2,626.50 ns |  0.65 |    0.01 |    1 |         - |          NA |
| PairwiseSortingNetwork  | 4096 | Random             |   349,708.3 ns |  1,563.52 ns |   694.21 ns |  0.76 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **SingleElementMoved** |   **260,304.1 ns** |  **2,168.37 ns** | **1,134.10 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | SingleElementMoved |   459,692.8 ns |  1,865.78 ns |   828.42 ns |  1.77 |    0.01 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | SingleElementMoved |   128,527.8 ns |  2,041.77 ns | 1,067.89 ns |  0.49 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 4096 | SingleElementMoved |    96,822.0 ns |  4,152.11 ns | 2,171.63 ns |  0.37 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Sorted**             |   **260,074.1 ns** | **10,543.72 ns** | **4,681.48 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Sorted             |   459,794.8 ns |  2,080.59 ns |   923.80 ns |  1.77 |    0.03 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Sorted             |   131,166.7 ns |  4,238.41 ns | 1,881.88 ns |  0.50 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 4096 | Sorted             |    95,871.5 ns |  5,365.71 ns | 2,806.37 ns |  0.37 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **Reversed**           |   **248,596.4 ns** |  **9,299.78 ns** | **4,863.96 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | Reversed           |   459,978.8 ns |  2,249.46 ns | 1,176.51 ns |  1.85 |    0.03 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | Reversed           |   123,845.5 ns |  1,616.97 ns |   845.70 ns |  0.50 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 4096 | Reversed           |    96,430.4 ns |  2,409.71 ns | 1,069.93 ns |  0.39 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **PipeOrgan**          |   **259,735.7 ns** |  **4,661.69 ns** | **2,069.82 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | PipeOrgan          |   459,779.9 ns |  1,809.21 ns |   946.25 ns |  1.77 |    0.01 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | PipeOrgan          |   125,467.2 ns |  1,034.08 ns |   540.84 ns |  0.48 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 4096 | PipeOrgan          |    97,420.1 ns |  3,219.00 ns | 1,683.60 ns |  0.38 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **4096** | **ManyDuplicates**     |   **381,372.9 ns** |  **2,345.71 ns** | **1,226.85 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicRecursiveSort    | 4096 | ManyDuplicates     |   603,064.5 ns |  2,126.65 ns |   944.24 ns |  1.58 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 4096 | ManyDuplicates     |   242,280.8 ns |  1,139.70 ns |   506.03 ns |  0.64 |    0.00 |    1 |         - |          NA |
| PairwiseSortingNetwork  | 4096 | ManyDuplicates     |   251,059.6 ns |  3,122.51 ns | 1,386.41 ns |  0.66 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **8192** | **Random**             | **1,032,118.0 ns** |  **4,909.84 ns** | **2,180.00 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | Random             | 1,533,432.3 ns |  3,289.89 ns | 1,720.68 ns |  1.49 |    0.00 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | Random             |   654,829.6 ns |  2,817.87 ns | 1,251.15 ns |  0.63 |    0.00 |    1 |         - |          NA |
| PairwiseSortingNetwork  | 8192 | Random             |   792,570.7 ns |  3,145.61 ns | 1,645.21 ns |  0.77 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **8192** | **SingleElementMoved** |   **603,075.4 ns** |  **5,530.20 ns** | **2,455.44 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | SingleElementMoved | 1,035,891.2 ns |  3,943.88 ns | 1,751.11 ns |  1.72 |    0.01 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | SingleElementMoved |   300,162.2 ns |  6,310.71 ns | 3,300.62 ns |  0.50 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 8192 | SingleElementMoved |   221,336.2 ns |  6,176.24 ns | 2,742.29 ns |  0.37 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **8192** | **Sorted**             |   **600,085.6 ns** | **12,614.07 ns** | **5,600.72 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | Sorted             | 1,034,512.9 ns |  2,246.04 ns |   997.26 ns |  1.72 |    0.02 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | Sorted             |   306,149.1 ns | 11,456.62 ns | 5,992.04 ns |  0.51 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 8192 | Sorted             |   222,388.3 ns |  8,400.18 ns | 4,393.46 ns |  0.37 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **8192** | **Reversed**           |   **579,629.7 ns** | **17,268.59 ns** | **9,031.81 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | Reversed           | 1,035,214.0 ns |  1,460.25 ns |   648.36 ns |  1.79 |    0.03 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | Reversed           |   290,769.6 ns |  3,491.12 ns | 1,825.92 ns |  0.50 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 8192 | Reversed           |   227,356.7 ns |  2,564.33 ns | 1,341.20 ns |  0.39 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **8192** | **PipeOrgan**          |   **600,737.2 ns** |  **8,719.77 ns** | **4,560.61 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | PipeOrgan          | 1,036,226.6 ns |  4,749.75 ns | 2,484.21 ns |  1.73 |    0.01 |    4 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | PipeOrgan          |   294,209.9 ns |  1,816.96 ns |   806.74 ns |  0.49 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetwork  | 8192 | PipeOrgan          |   222,515.2 ns |  5,351.24 ns | 2,798.80 ns |  0.37 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **BitonicSort**             | **8192** | **ManyDuplicates**     |   **843,056.7 ns** |  **3,184.98 ns** | **1,665.81 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicRecursiveSort    | 8192 | ManyDuplicates     | 1,327,038.9 ns |  5,201.21 ns | 2,309.37 ns |  1.57 |    0.00 |    3 |         - |          NA |
| BatcherOddEvenMergeSort | 8192 | ManyDuplicates     |   520,775.0 ns |  5,582.87 ns | 2,919.95 ns |  0.62 |    0.00 |    1 |         - |          NA |
| PairwiseSortingNetwork  | 8192 | ManyDuplicates     |   539,356.1 ns |  6,079.14 ns | 3,179.51 ns |  0.64 |    0.00 |    1 |         - |          NA |

### PartitionBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.58 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                       | Size | Pattern            | Mean           | Error        | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------------- |----- |------------------- |---------------:|-------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **QuickSort**                    | **256**  | **Random**             |     **8,977.0 ns** |    **446.36 ns** |   **233.46 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 256  | Random             |     9,454.3 ns |    365.89 ns |   191.37 ns |  1.05 |    0.03 |    3 |         - |          NA |
| QuickSortMedian3             | 256  | Random             |     9,080.0 ns |    344.19 ns |   180.02 ns |  1.01 |    0.03 |    3 |         - |          NA |
| QuickSortMedian9             | 256  | Random             |     9,530.5 ns |    348.77 ns |   182.41 ns |  1.06 |    0.03 |    3 |         - |          NA |
| SampleSort                   | 256  | Random             |    10,974.4 ns |    282.72 ns |   125.53 ns |  1.22 |    0.03 |    3 |         - |          NA |
| DualPivotQuickSort           | 256  | Random             |     5,343.7 ns |     85.96 ns |    38.17 ns |  0.60 |    0.01 |    2 |         - |          NA |
| StableQuickSort              | 256  | Random             |    19,969.4 ns |    162.26 ns |    84.87 ns |  2.23 |    0.05 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Random             |    16,625.7 ns |    271.03 ns |   120.34 ns |  1.85 |    0.05 |    4 |         - |          NA |
| DestswapStableQuickSort      | 256  | Random             |    15,134.9 ns |    411.65 ns |   215.30 ns |  1.69 |    0.05 |    4 |         - |          NA |
| IntroSort                    | 256  | Random             |     6,953.7 ns |    174.47 ns |    91.25 ns |  0.78 |    0.02 |    2 |         - |          NA |
| IntroSortDotnet              | 256  | Random             |     6,315.9 ns |     81.76 ns |    36.30 ns |  0.70 |    0.02 |    2 |         - |          NA |
| PDQSort                      | 256  | Random             |     6,098.3 ns |    338.50 ns |   177.04 ns |  0.68 |    0.02 |    2 |         - |          NA |
| PDQSortBranchless            | 256  | Random             |     4,672.2 ns |    301.93 ns |   157.92 ns |  0.52 |    0.02 |    2 |         - |          NA |
| Ipnsort                      | 256  | Random             |     3,718.0 ns |    342.11 ns |   178.93 ns |  0.41 |    0.02 |    1 |         - |          NA |
| StdSort                      | 256  | Random             |     4,559.2 ns |    408.55 ns |   213.68 ns |  0.51 |    0.03 |    2 |         - |          NA |
| BlockQuickSort               | 256  | Random             |     4,737.3 ns |    215.42 ns |   112.67 ns |  0.53 |    0.02 |    2 |         - |          NA |
| FluxSort                     | 256  | Random             |     6,307.5 ns |    256.73 ns |   113.99 ns |  0.70 |    0.02 |    2 |         - |          NA |
| DotnetSort                   | 256  | Random             |     5,864.7 ns |    208.33 ns |   108.96 ns |  0.65 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **256**  | **SingleElementMoved** |     **1,107.4 ns** |     **80.83 ns** |    **42.27 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 256  | SingleElementMoved |     2,161.2 ns |    269.37 ns |   119.60 ns |  1.95 |    0.12 |    4 |         - |          NA |
| QuickSortMedian3             | 256  | SingleElementMoved |     1,695.8 ns |    203.10 ns |    90.18 ns |  1.53 |    0.09 |    3 |         - |          NA |
| QuickSortMedian9             | 256  | SingleElementMoved |     1,736.2 ns |    105.55 ns |    46.86 ns |  1.57 |    0.07 |    3 |         - |          NA |
| SampleSort                   | 256  | SingleElementMoved |     5,252.9 ns |    893.88 ns |   467.51 ns |  4.75 |    0.43 |    6 |         - |          NA |
| DualPivotQuickSort           | 256  | SingleElementMoved |       654.6 ns |     52.63 ns |    27.53 ns |  0.59 |    0.03 |    1 |         - |          NA |
| StableQuickSort              | 256  | SingleElementMoved |     6,659.2 ns |     47.60 ns |    21.14 ns |  6.02 |    0.22 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 256  | SingleElementMoved |     4,251.1 ns |    165.42 ns |    73.45 ns |  3.84 |    0.15 |    6 |         - |          NA |
| DestswapStableQuickSort      | 256  | SingleElementMoved |     3,921.6 ns |    259.41 ns |   135.68 ns |  3.55 |    0.17 |    6 |         - |          NA |
| IntroSort                    | 256  | SingleElementMoved |       628.6 ns |     83.06 ns |    36.88 ns |  0.57 |    0.04 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | SingleElementMoved |     1,126.6 ns |    142.79 ns |    63.40 ns |  1.02 |    0.07 |    2 |         - |          NA |
| PDQSort                      | 256  | SingleElementMoved |       884.8 ns |    118.53 ns |    52.63 ns |  0.80 |    0.05 |    2 |         - |          NA |
| PDQSortBranchless            | 256  | SingleElementMoved |     1,039.4 ns |     72.48 ns |    32.18 ns |  0.94 |    0.04 |    2 |         - |          NA |
| Ipnsort                      | 256  | SingleElementMoved |     2,955.2 ns |    180.06 ns |    79.95 ns |  2.67 |    0.12 |    5 |         - |          NA |
| StdSort                      | 256  | SingleElementMoved |       988.3 ns |    104.89 ns |    46.57 ns |  0.89 |    0.05 |    2 |         - |          NA |
| BlockQuickSort               | 256  | SingleElementMoved |     2,577.9 ns |    113.14 ns |    50.24 ns |  2.33 |    0.09 |    5 |         - |          NA |
| FluxSort                     | 256  | SingleElementMoved |     1,213.5 ns |     89.64 ns |    39.80 ns |  1.10 |    0.05 |    2 |         - |          NA |
| DotnetSort                   | 256  | SingleElementMoved |     1,094.0 ns |     85.18 ns |    44.55 ns |  0.99 |    0.05 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Sorted**             |       **664.1 ns** |     **32.92 ns** |    **14.62 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 256  | Sorted             |     1,403.8 ns |     33.25 ns |    11.86 ns |  2.11 |    0.05 |    7 |         - |          NA |
| QuickSortMedian3             | 256  | Sorted             |       921.9 ns |     18.34 ns |     8.14 ns |  1.39 |    0.03 |    6 |         - |          NA |
| QuickSortMedian9             | 256  | Sorted             |       998.6 ns |     23.26 ns |    10.33 ns |  1.50 |    0.03 |    6 |         - |          NA |
| SampleSort                   | 256  | Sorted             |     3,591.1 ns |    254.73 ns |   133.23 ns |  5.41 |    0.22 |    8 |         - |          NA |
| DualPivotQuickSort           | 256  | Sorted             |       454.5 ns |     42.31 ns |    22.13 ns |  0.68 |    0.03 |    4 |         - |          NA |
| StableQuickSort              | 256  | Sorted             |     6,830.1 ns |    180.73 ns |    94.53 ns | 10.29 |    0.25 |    9 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Sorted             |     3,963.7 ns |     49.00 ns |    17.47 ns |  5.97 |    0.12 |    8 |         - |          NA |
| DestswapStableQuickSort      | 256  | Sorted             |     3,537.1 ns |     43.66 ns |    19.39 ns |  5.33 |    0.11 |    8 |         - |          NA |
| IntroSort                    | 256  | Sorted             |       269.1 ns |     57.12 ns |    25.36 ns |  0.41 |    0.04 |    3 |         - |          NA |
| IntroSortDotnet              | 256  | Sorted             |       761.2 ns |      8.31 ns |     4.35 ns |  1.15 |    0.02 |    5 |         - |          NA |
| PDQSort                      | 256  | Sorted             |       316.3 ns |      6.68 ns |     2.96 ns |  0.48 |    0.01 |    3 |         - |          NA |
| PDQSortBranchless            | 256  | Sorted             |       362.8 ns |     32.37 ns |    16.93 ns |  0.55 |    0.03 |    3 |         - |          NA |
| Ipnsort                      | 256  | Sorted             |       130.2 ns |      3.42 ns |     1.52 ns |  0.20 |    0.00 |    1 |         - |          NA |
| StdSort                      | 256  | Sorted             |       345.2 ns |     16.76 ns |     7.44 ns |  0.52 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 256  | Sorted             |     1,479.7 ns |     17.53 ns |     7.78 ns |  2.23 |    0.05 |    7 |         - |          NA |
| FluxSort                     | 256  | Sorted             |       195.7 ns |      6.27 ns |     2.79 ns |  0.29 |    0.01 |    2 |         - |          NA |
| DotnetSort                   | 256  | Sorted             |       811.7 ns |    374.58 ns |   195.91 ns |  1.22 |    0.28 |    5 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Reversed**           |       **767.8 ns** |     **53.32 ns** |    **27.89 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 256  | Reversed           |     1,567.4 ns |     19.72 ns |     7.03 ns |  2.04 |    0.07 |    6 |         - |          NA |
| QuickSortMedian3             | 256  | Reversed           |     1,070.1 ns |    216.06 ns |    95.93 ns |  1.40 |    0.13 |    5 |         - |          NA |
| QuickSortMedian9             | 256  | Reversed           |     1,116.9 ns |     38.47 ns |    20.12 ns |  1.46 |    0.05 |    5 |         - |          NA |
| SampleSort                   | 256  | Reversed           |     3,923.7 ns |    283.22 ns |   148.13 ns |  5.12 |    0.25 |    7 |         - |          NA |
| DualPivotQuickSort           | 256  | Reversed           |       693.1 ns |     74.30 ns |    32.99 ns |  0.90 |    0.05 |    4 |         - |          NA |
| StableQuickSort              | 256  | Reversed           |     6,863.1 ns |     66.04 ns |    29.32 ns |  8.95 |    0.30 |    9 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Reversed           |     3,979.9 ns |    243.39 ns |   127.30 ns |  5.19 |    0.23 |    7 |         - |          NA |
| DestswapStableQuickSort      | 256  | Reversed           |     5,692.1 ns |     51.31 ns |    26.84 ns |  7.42 |    0.25 |    8 |         - |          NA |
| IntroSort                    | 256  | Reversed           |       416.3 ns |     39.71 ns |    17.63 ns |  0.54 |    0.03 |    3 |         - |          NA |
| IntroSortDotnet              | 256  | Reversed           |     1,102.8 ns |     16.71 ns |     8.74 ns |  1.44 |    0.05 |    5 |         - |          NA |
| PDQSort                      | 256  | Reversed           |       441.2 ns |      9.50 ns |     4.97 ns |  0.58 |    0.02 |    3 |         - |          NA |
| PDQSortBranchless            | 256  | Reversed           |       758.4 ns |     14.31 ns |     6.35 ns |  0.99 |    0.03 |    4 |         - |          NA |
| Ipnsort                      | 256  | Reversed           |       190.0 ns |      4.65 ns |     2.43 ns |  0.25 |    0.01 |    1 |         - |          NA |
| StdSort                      | 256  | Reversed           |       669.1 ns |     46.94 ns |    20.84 ns |  0.87 |    0.04 |    4 |         - |          NA |
| BlockQuickSort               | 256  | Reversed           |     1,653.9 ns |      9.26 ns |     4.84 ns |  2.16 |    0.07 |    6 |         - |          NA |
| FluxSort                     | 256  | Reversed           |       280.8 ns |     16.25 ns |     8.50 ns |  0.37 |    0.02 |    2 |         - |          NA |
| DotnetSort                   | 256  | Reversed           |     1,167.6 ns |     41.61 ns |    18.48 ns |  1.52 |    0.06 |    5 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **256**  | **PipeOrgan**          |     **5,762.0 ns** |    **255.88 ns** |   **133.83 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 256  | PipeOrgan          |     2,807.3 ns |    267.87 ns |   140.10 ns |  0.49 |    0.03 |    2 |         - |          NA |
| QuickSortMedian3             | 256  | PipeOrgan          |     2,482.5 ns |     84.27 ns |    37.41 ns |  0.43 |    0.01 |    2 |         - |          NA |
| QuickSortMedian9             | 256  | PipeOrgan          |     2,034.4 ns |     65.11 ns |    28.91 ns |  0.35 |    0.01 |    2 |         - |          NA |
| SampleSort                   | 256  | PipeOrgan          |     3,884.6 ns |    287.07 ns |   150.15 ns |  0.67 |    0.03 |    2 |         - |          NA |
| DualPivotQuickSort           | 256  | PipeOrgan          |     1,197.2 ns |     87.47 ns |    45.75 ns |  0.21 |    0.01 |    1 |         - |          NA |
| StableQuickSort              | 256  | PipeOrgan          |     6,504.1 ns |     72.53 ns |    32.20 ns |  1.13 |    0.02 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 256  | PipeOrgan          |     3,972.1 ns |    291.29 ns |   152.35 ns |  0.69 |    0.03 |    2 |         - |          NA |
| DestswapStableQuickSort      | 256  | PipeOrgan          |     5,862.6 ns |     60.52 ns |    21.58 ns |  1.02 |    0.02 |    3 |         - |          NA |
| IntroSort                    | 256  | PipeOrgan          |     1,146.4 ns |     28.53 ns |    12.67 ns |  0.20 |    0.00 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | PipeOrgan          |     1,683.3 ns |     45.24 ns |    20.09 ns |  0.29 |    0.01 |    2 |         - |          NA |
| PDQSort                      | 256  | PipeOrgan          |     1,329.4 ns |     26.53 ns |    11.78 ns |  0.23 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | PipeOrgan          |     2,430.9 ns |    229.90 ns |   120.24 ns |  0.42 |    0.02 |    2 |         - |          NA |
| Ipnsort                      | 256  | PipeOrgan          |     3,183.7 ns |    335.33 ns |   175.38 ns |  0.55 |    0.03 |    2 |         - |          NA |
| StdSort                      | 256  | PipeOrgan          |     3,440.7 ns |     58.57 ns |    26.01 ns |  0.60 |    0.01 |    2 |         - |          NA |
| BlockQuickSort               | 256  | PipeOrgan          |     4,846.2 ns |     57.42 ns |    25.49 ns |  0.84 |    0.02 |    3 |         - |          NA |
| FluxSort                     | 256  | PipeOrgan          |     3,092.1 ns |    263.73 ns |   137.94 ns |  0.54 |    0.03 |    2 |         - |          NA |
| DotnetSort                   | 256  | PipeOrgan          |     1,982.7 ns |    362.50 ns |   189.59 ns |  0.34 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **256**  | **ManyDuplicates**     |     **6,732.8 ns** |    **292.14 ns** |   **129.71 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 256  | ManyDuplicates     |     6,097.7 ns |    270.18 ns |   119.96 ns |  0.91 |    0.02 |    1 |         - |          NA |
| QuickSortMedian3             | 256  | ManyDuplicates     |     7,711.5 ns |    125.01 ns |    55.50 ns |  1.15 |    0.02 |    1 |         - |          NA |
| QuickSortMedian9             | 256  | ManyDuplicates     |     8,111.4 ns |    444.62 ns |   232.54 ns |  1.21 |    0.04 |    1 |         - |          NA |
| SampleSort                   | 256  | ManyDuplicates     |     9,824.8 ns |    346.91 ns |   181.44 ns |  1.46 |    0.04 |    2 |         - |          NA |
| DualPivotQuickSort           | 256  | ManyDuplicates     |     5,246.2 ns |    430.00 ns |   224.90 ns |  0.78 |    0.03 |    1 |         - |          NA |
| StableQuickSort              | 256  | ManyDuplicates     |    13,048.7 ns |    211.83 ns |   110.79 ns |  1.94 |    0.04 |    2 |         - |          NA |
| BidirectionalStableQuickSort | 256  | ManyDuplicates     |    10,765.0 ns |    401.75 ns |   210.13 ns |  1.60 |    0.04 |    2 |         - |          NA |
| DestswapStableQuickSort      | 256  | ManyDuplicates     |    11,964.4 ns |    247.47 ns |   109.88 ns |  1.78 |    0.04 |    2 |         - |          NA |
| IntroSort                    | 256  | ManyDuplicates     |     6,305.0 ns |    222.53 ns |    98.81 ns |  0.94 |    0.02 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | ManyDuplicates     |     5,933.8 ns |     98.93 ns |    43.92 ns |  0.88 |    0.02 |    1 |         - |          NA |
| PDQSort                      | 256  | ManyDuplicates     |     5,510.6 ns |    214.62 ns |    95.29 ns |  0.82 |    0.02 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | ManyDuplicates     |     4,076.7 ns |    259.52 ns |   135.73 ns |  0.61 |    0.02 |    1 |         - |          NA |
| Ipnsort                      | 256  | ManyDuplicates     |     3,717.7 ns |    291.29 ns |   152.35 ns |  0.55 |    0.02 |    1 |         - |          NA |
| StdSort                      | 256  | ManyDuplicates     |     4,152.1 ns |    270.15 ns |   141.29 ns |  0.62 |    0.02 |    1 |         - |          NA |
| BlockQuickSort               | 256  | ManyDuplicates     |     4,712.4 ns |    219.83 ns |   114.98 ns |  0.70 |    0.02 |    1 |         - |          NA |
| FluxSort                     | 256  | ManyDuplicates     |     6,021.4 ns |    239.91 ns |   125.48 ns |  0.89 |    0.02 |    1 |         - |          NA |
| DotnetSort                   | 256  | ManyDuplicates     |     5,618.8 ns |    499.36 ns |   261.17 ns |  0.83 |    0.04 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Random**             |    **41,446.3 ns** |    **242.92 ns** |   **107.86 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Random             |    44,072.0 ns |    617.02 ns |   220.04 ns |  1.06 |    0.01 |    2 |         - |          NA |
| QuickSortMedian3             | 1024 | Random             |    42,382.4 ns |    408.30 ns |   213.55 ns |  1.02 |    0.01 |    2 |         - |          NA |
| QuickSortMedian9             | 1024 | Random             |    45,039.4 ns |    763.50 ns |   339.00 ns |  1.09 |    0.01 |    2 |         - |          NA |
| SampleSort                   | 1024 | Random             |    51,377.0 ns |    302.24 ns |   134.19 ns |  1.24 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort           | 1024 | Random             |    28,478.2 ns |    464.42 ns |   206.20 ns |  0.69 |    0.00 |    1 |         - |          NA |
| StableQuickSort              | 1024 | Random             |   100,184.5 ns |  2,887.33 ns | 1,281.99 ns |  2.42 |    0.03 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Random             |    81,111.7 ns |  1,122.54 ns |   587.11 ns |  1.96 |    0.01 |    3 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Random             |    71,669.9 ns |    486.84 ns |   254.63 ns |  1.73 |    0.01 |    3 |         - |          NA |
| IntroSort                    | 1024 | Random             |    34,141.5 ns |    430.47 ns |   225.14 ns |  0.82 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | Random             |    32,350.7 ns |    224.38 ns |   117.35 ns |  0.78 |    0.00 |    1 |         - |          NA |
| PDQSort                      | 1024 | Random             |    30,866.2 ns |    263.82 ns |   137.98 ns |  0.74 |    0.00 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | Random             |    21,057.2 ns |    244.85 ns |   128.06 ns |  0.51 |    0.00 |    1 |         - |          NA |
| Ipnsort                      | 1024 | Random             |    19,072.2 ns |    133.25 ns |    59.16 ns |  0.46 |    0.00 |    1 |         - |          NA |
| StdSort                      | 1024 | Random             |    20,805.9 ns |    795.87 ns |   353.37 ns |  0.50 |    0.01 |    1 |         - |          NA |
| BlockQuickSort               | 1024 | Random             |    22,259.7 ns |    380.98 ns |   199.26 ns |  0.54 |    0.00 |    1 |         - |          NA |
| FluxSort                     | 1024 | Random             |    25,388.1 ns |    435.53 ns |   227.79 ns |  0.61 |    0.01 |    1 |         - |          NA |
| DotnetSort                   | 1024 | Random             |    29,758.1 ns |    403.60 ns |   211.09 ns |  0.72 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **SingleElementMoved** |     **4,191.9 ns** |    **126.04 ns** |    **65.92 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 1024 | SingleElementMoved |     8,120.3 ns |    255.43 ns |   133.60 ns |  1.94 |    0.04 |    4 |         - |          NA |
| QuickSortMedian3             | 1024 | SingleElementMoved |     6,557.9 ns |    405.48 ns |   212.07 ns |  1.56 |    0.05 |    4 |         - |          NA |
| QuickSortMedian9             | 1024 | SingleElementMoved |     7,256.3 ns |    363.36 ns |   190.04 ns |  1.73 |    0.05 |    4 |         - |          NA |
| SampleSort                   | 1024 | SingleElementMoved |    26,053.3 ns |    716.12 ns |   374.55 ns |  6.22 |    0.13 |    9 |         - |          NA |
| DualPivotQuickSort           | 1024 | SingleElementMoved |     3,048.9 ns |    287.71 ns |   150.48 ns |  0.73 |    0.04 |    3 |         - |          NA |
| StableQuickSort              | 1024 | SingleElementMoved |    33,304.1 ns |    251.39 ns |   131.48 ns |  7.95 |    0.12 |   10 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | SingleElementMoved |    21,367.8 ns |    282.26 ns |   147.63 ns |  5.10 |    0.08 |    8 |         - |          NA |
| DestswapStableQuickSort      | 1024 | SingleElementMoved |    17,725.9 ns |    648.12 ns |   338.98 ns |  4.23 |    0.10 |    7 |         - |          NA |
| IntroSort                    | 1024 | SingleElementMoved |     2,383.0 ns |     85.33 ns |    30.43 ns |  0.57 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 1024 | SingleElementMoved |     4,791.9 ns |    101.31 ns |    44.98 ns |  1.14 |    0.02 |    3 |         - |          NA |
| PDQSort                      | 1024 | SingleElementMoved |     3,335.7 ns |    124.08 ns |    64.90 ns |  0.80 |    0.02 |    3 |         - |          NA |
| PDQSortBranchless            | 1024 | SingleElementMoved |     4,152.7 ns |    115.44 ns |    60.38 ns |  0.99 |    0.02 |    3 |         - |          NA |
| Ipnsort                      | 1024 | SingleElementMoved |    14,522.4 ns |    125.98 ns |    65.89 ns |  3.47 |    0.05 |    6 |         - |          NA |
| StdSort                      | 1024 | SingleElementMoved |     3,652.5 ns |    424.13 ns |   221.83 ns |  0.87 |    0.05 |    3 |         - |          NA |
| BlockQuickSort               | 1024 | SingleElementMoved |    11,675.0 ns |    193.80 ns |   101.36 ns |  2.79 |    0.05 |    5 |         - |          NA |
| FluxSort                     | 1024 | SingleElementMoved |     1,367.3 ns |     33.02 ns |    14.66 ns |  0.33 |    0.01 |    1 |         - |          NA |
| DotnetSort                   | 1024 | SingleElementMoved |     5,279.7 ns |    199.24 ns |   104.21 ns |  1.26 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Sorted**             |     **3,166.8 ns** |     **63.79 ns** |    **28.32 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Sorted             |     6,432.5 ns |    457.61 ns |   239.34 ns |  2.03 |    0.07 |    6 |         - |          NA |
| QuickSortMedian3             | 1024 | Sorted             |     4,287.6 ns |     55.54 ns |    19.81 ns |  1.35 |    0.01 |    5 |         - |          NA |
| QuickSortMedian9             | 1024 | Sorted             |     4,592.5 ns |     57.14 ns |    29.88 ns |  1.45 |    0.02 |    5 |         - |          NA |
| SampleSort                   | 1024 | Sorted             |    15,708.6 ns |  1,214.09 ns |   634.99 ns |  4.96 |    0.19 |    7 |         - |          NA |
| DualPivotQuickSort           | 1024 | Sorted             |     2,643.3 ns |    327.23 ns |   171.15 ns |  0.83 |    0.05 |    5 |         - |          NA |
| StableQuickSort              | 1024 | Sorted             |    33,104.9 ns |    278.79 ns |   145.81 ns | 10.45 |    0.10 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Sorted             |    19,559.1 ns |    213.88 ns |    94.97 ns |  6.18 |    0.06 |    7 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Sorted             |    16,785.5 ns |    253.36 ns |   132.51 ns |  5.30 |    0.06 |    7 |         - |          NA |
| IntroSort                    | 1024 | Sorted             |       886.1 ns |     35.96 ns |    12.82 ns |  0.28 |    0.00 |    3 |         - |          NA |
| IntroSortDotnet              | 1024 | Sorted             |     3,664.6 ns |     99.77 ns |    44.30 ns |  1.16 |    0.02 |    5 |         - |          NA |
| PDQSort                      | 1024 | Sorted             |     1,183.9 ns |     30.13 ns |    13.38 ns |  0.37 |    0.01 |    4 |         - |          NA |
| PDQSortBranchless            | 1024 | Sorted             |     1,191.1 ns |      8.79 ns |     3.90 ns |  0.38 |    0.00 |    4 |         - |          NA |
| Ipnsort                      | 1024 | Sorted             |       492.1 ns |     12.79 ns |     5.68 ns |  0.16 |    0.00 |    1 |         - |          NA |
| StdSort                      | 1024 | Sorted             |     1,222.7 ns |      6.72 ns |     2.40 ns |  0.39 |    0.00 |    4 |         - |          NA |
| BlockQuickSort               | 1024 | Sorted             |     7,397.0 ns |    330.94 ns |   173.09 ns |  2.34 |    0.06 |    6 |         - |          NA |
| FluxSort                     | 1024 | Sorted             |       604.2 ns |     12.13 ns |     6.35 ns |  0.19 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 1024 | Sorted             |     3,150.0 ns |    145.71 ns |    64.70 ns |  0.99 |    0.02 |    5 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Reversed**           |     **3,523.1 ns** |    **181.99 ns** |    **95.18 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Reversed           |     6,933.9 ns |    473.49 ns |   247.65 ns |  1.97 |    0.08 |    6 |         - |          NA |
| QuickSortMedian3             | 1024 | Reversed           |     4,595.9 ns |     50.36 ns |    22.36 ns |  1.31 |    0.03 |    5 |         - |          NA |
| QuickSortMedian9             | 1024 | Reversed           |     4,833.3 ns |     45.80 ns |    20.34 ns |  1.37 |    0.03 |    5 |         - |          NA |
| SampleSort                   | 1024 | Reversed           |    16,693.5 ns |    573.95 ns |   300.19 ns |  4.74 |    0.14 |    7 |         - |          NA |
| DualPivotQuickSort           | 1024 | Reversed           |     3,139.2 ns |    360.47 ns |   188.53 ns |  0.89 |    0.06 |    4 |         - |          NA |
| StableQuickSort              | 1024 | Reversed           |    33,656.8 ns |    493.82 ns |   219.26 ns |  9.56 |    0.25 |    9 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Reversed           |    19,121.4 ns |    479.81 ns |   213.04 ns |  5.43 |    0.15 |    7 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Reversed           |    25,348.3 ns |    183.88 ns |    96.17 ns |  7.20 |    0.18 |    8 |         - |          NA |
| IntroSort                    | 1024 | Reversed           |     1,711.7 ns |    224.39 ns |   117.36 ns |  0.49 |    0.03 |    3 |         - |          NA |
| IntroSortDotnet              | 1024 | Reversed           |     5,743.9 ns |    360.41 ns |   188.50 ns |  1.63 |    0.06 |    5 |         - |          NA |
| PDQSort                      | 1024 | Reversed           |     1,670.9 ns |    115.52 ns |    51.29 ns |  0.47 |    0.02 |    3 |         - |          NA |
| PDQSortBranchless            | 1024 | Reversed           |     2,619.5 ns |     60.73 ns |    21.66 ns |  0.74 |    0.02 |    4 |         - |          NA |
| Ipnsort                      | 1024 | Reversed           |       846.8 ns |    224.14 ns |   117.23 ns |  0.24 |    0.03 |    1 |         - |          NA |
| StdSort                      | 1024 | Reversed           |     2,424.9 ns |     71.95 ns |    31.94 ns |  0.69 |    0.02 |    4 |         - |          NA |
| BlockQuickSort               | 1024 | Reversed           |     8,173.2 ns |    320.40 ns |   142.26 ns |  2.32 |    0.07 |    6 |         - |          NA |
| FluxSort                     | 1024 | Reversed           |     1,009.6 ns |    208.05 ns |   108.82 ns |  0.29 |    0.03 |    2 |         - |          NA |
| DotnetSort                   | 1024 | Reversed           |     5,746.7 ns |    779.81 ns |   407.86 ns |  1.63 |    0.12 |    5 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **PipeOrgan**          |    **84,555.1 ns** |    **751.30 ns** |   **333.58 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| QuickSort3way                | 1024 | PipeOrgan          |    15,928.7 ns |    856.98 ns |   380.50 ns |  0.19 |    0.00 |    4 |         - |          NA |
| QuickSortMedian3             | 1024 | PipeOrgan          |    13,103.1 ns |    835.72 ns |   437.10 ns |  0.15 |    0.00 |    3 |         - |          NA |
| QuickSortMedian9             | 1024 | PipeOrgan          |     9,891.7 ns |    835.36 ns |   436.91 ns |  0.12 |    0.00 |    3 |         - |          NA |
| SampleSort                   | 1024 | PipeOrgan          |    17,580.3 ns |    530.77 ns |   235.66 ns |  0.21 |    0.00 |    4 |         - |          NA |
| DualPivotQuickSort           | 1024 | PipeOrgan          |     6,267.6 ns |    630.50 ns |   329.77 ns |  0.07 |    0.00 |    2 |         - |          NA |
| StableQuickSort              | 1024 | PipeOrgan          |    33,621.2 ns |    425.21 ns |   222.39 ns |  0.40 |    0.00 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | PipeOrgan          |    19,609.4 ns |    486.88 ns |   254.65 ns |  0.23 |    0.00 |    4 |         - |          NA |
| DestswapStableQuickSort      | 1024 | PipeOrgan          |    28,979.5 ns |    581.29 ns |   304.02 ns |  0.34 |    0.00 |    5 |         - |          NA |
| IntroSort                    | 1024 | PipeOrgan          |     5,513.8 ns |    197.91 ns |    87.87 ns |  0.07 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet              | 1024 | PipeOrgan          |    11,176.4 ns |    272.67 ns |   142.61 ns |  0.13 |    0.00 |    3 |         - |          NA |
| PDQSort                      | 1024 | PipeOrgan          |     6,724.2 ns |    265.15 ns |   117.73 ns |  0.08 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | PipeOrgan          |    12,480.2 ns |    356.46 ns |   186.43 ns |  0.15 |    0.00 |    3 |         - |          NA |
| Ipnsort                      | 1024 | PipeOrgan          |    16,634.1 ns |    139.05 ns |    72.73 ns |  0.20 |    0.00 |    4 |         - |          NA |
| StdSort                      | 1024 | PipeOrgan          |    16,726.5 ns |    358.02 ns |   187.25 ns |  0.20 |    0.00 |    4 |         - |          NA |
| BlockQuickSort               | 1024 | PipeOrgan          |    22,807.6 ns |    423.16 ns |   221.32 ns |  0.27 |    0.00 |    4 |         - |          NA |
| FluxSort                     | 1024 | PipeOrgan          |     2,295.8 ns |     14.88 ns |     6.61 ns |  0.03 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 1024 | PipeOrgan          |    12,882.2 ns |    506.39 ns |   264.85 ns |  0.15 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **ManyDuplicates**     |    **25,938.2 ns** |    **287.89 ns** |   **127.82 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 1024 | ManyDuplicates     |    20,265.7 ns |    250.97 ns |   131.26 ns |  0.78 |    0.01 |    2 |         - |          NA |
| QuickSortMedian3             | 1024 | ManyDuplicates     |    29,928.8 ns |    147.04 ns |    65.28 ns |  1.15 |    0.01 |    2 |         - |          NA |
| QuickSortMedian9             | 1024 | ManyDuplicates     |    31,162.1 ns |    232.66 ns |   103.30 ns |  1.20 |    0.01 |    2 |         - |          NA |
| SampleSort                   | 1024 | ManyDuplicates     |    39,106.0 ns |    418.00 ns |   218.62 ns |  1.51 |    0.01 |    3 |         - |          NA |
| DualPivotQuickSort           | 1024 | ManyDuplicates     |    20,515.2 ns |     79.90 ns |    35.47 ns |  0.79 |    0.00 |    2 |         - |          NA |
| StableQuickSort              | 1024 | ManyDuplicates     |    47,368.1 ns |    143.19 ns |    63.58 ns |  1.83 |    0.01 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | ManyDuplicates     |    37,899.7 ns |    220.88 ns |   115.52 ns |  1.46 |    0.01 |    3 |         - |          NA |
| DestswapStableQuickSort      | 1024 | ManyDuplicates     |    37,834.5 ns |    249.64 ns |   110.84 ns |  1.46 |    0.01 |    3 |         - |          NA |
| IntroSort                    | 1024 | ManyDuplicates     |    25,860.5 ns |    297.16 ns |   155.42 ns |  1.00 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 1024 | ManyDuplicates     |    25,376.9 ns |    262.40 ns |   137.24 ns |  0.98 |    0.01 |    2 |         - |          NA |
| PDQSort                      | 1024 | ManyDuplicates     |    20,201.6 ns |    177.30 ns |    78.72 ns |  0.78 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | ManyDuplicates     |    10,868.1 ns |    233.00 ns |   103.45 ns |  0.42 |    0.00 |    1 |         - |          NA |
| Ipnsort                      | 1024 | ManyDuplicates     |    15,306.5 ns |    203.12 ns |   106.24 ns |  0.59 |    0.00 |    2 |         - |          NA |
| StdSort                      | 1024 | ManyDuplicates     |    15,168.5 ns |    100.71 ns |    44.72 ns |  0.58 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | ManyDuplicates     |    17,569.6 ns |    236.93 ns |   105.20 ns |  0.68 |    0.00 |    2 |         - |          NA |
| FluxSort                     | 1024 | ManyDuplicates     |    24,738.5 ns |    238.61 ns |   124.80 ns |  0.95 |    0.01 |    2 |         - |          NA |
| DotnetSort                   | 1024 | ManyDuplicates     |    23,230.6 ns |    394.36 ns |   206.26 ns |  0.90 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Random**             |   **190,101.8 ns** |  **1,553.98 ns** |   **812.76 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Random             |   202,893.0 ns |  2,524.02 ns | 1,320.11 ns |  1.07 |    0.01 |    2 |         - |          NA |
| QuickSortMedian3             | 4096 | Random             |   192,884.2 ns |  1,586.50 ns |   829.77 ns |  1.01 |    0.01 |    2 |         - |          NA |
| QuickSortMedian9             | 4096 | Random             |   206,432.0 ns |  2,636.54 ns | 1,378.96 ns |  1.09 |    0.01 |    2 |         - |          NA |
| SampleSort                   | 4096 | Random             |   234,565.2 ns |  1,616.62 ns |   845.52 ns |  1.23 |    0.01 |    2 |         - |          NA |
| DualPivotQuickSort           | 4096 | Random             |   138,928.1 ns |    987.91 ns |   516.69 ns |  0.73 |    0.00 |    2 |         - |          NA |
| StableQuickSort              | 4096 | Random             |   474,798.0 ns |  2,960.60 ns | 1,314.53 ns |  2.50 |    0.01 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Random             |   380,671.3 ns |  1,756.54 ns |   779.91 ns |  2.00 |    0.01 |    3 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Random             |   332,522.7 ns |    969.15 ns |   430.31 ns |  1.75 |    0.01 |    3 |         - |          NA |
| IntroSort                    | 4096 | Random             |   162,069.3 ns |  1,269.73 ns |   664.09 ns |  0.85 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet              | 4096 | Random             |   155,289.4 ns |    814.78 ns |   361.77 ns |  0.82 |    0.00 |    2 |         - |          NA |
| PDQSort                      | 4096 | Random             |   148,320.6 ns |    674.18 ns |   352.61 ns |  0.78 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 4096 | Random             |    91,320.9 ns |    385.43 ns |   171.14 ns |  0.48 |    0.00 |    1 |         - |          NA |
| Ipnsort                      | 4096 | Random             |    90,867.9 ns |    607.82 ns |   317.90 ns |  0.48 |    0.00 |    1 |         - |          NA |
| StdSort                      | 4096 | Random             |    90,664.3 ns |  1,346.82 ns |   704.41 ns |  0.48 |    0.00 |    1 |         - |          NA |
| BlockQuickSort               | 4096 | Random             |    97,838.1 ns |    907.73 ns |   403.04 ns |  0.51 |    0.00 |    1 |         - |          NA |
| FluxSort                     | 4096 | Random             |   108,742.7 ns |    216.02 ns |    95.92 ns |  0.57 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 4096 | Random             |   144,738.8 ns |  1,669.69 ns |   741.35 ns |  0.76 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **4096** | **SingleElementMoved** |    **18,377.3 ns** |    **660.66 ns** |   **293.34 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 4096 | SingleElementMoved |    33,343.9 ns |  1,265.78 ns |   662.03 ns |  1.81 |    0.04 |    4 |         - |          NA |
| QuickSortMedian3             | 4096 | SingleElementMoved |    26,492.9 ns |    291.88 ns |   129.60 ns |  1.44 |    0.02 |    4 |         - |          NA |
| QuickSortMedian9             | 4096 | SingleElementMoved |    30,491.4 ns |    528.03 ns |   276.17 ns |  1.66 |    0.03 |    4 |         - |          NA |
| SampleSort                   | 4096 | SingleElementMoved |   131,574.0 ns |  2,579.80 ns | 1,349.28 ns |  7.16 |    0.13 |    8 |         - |          NA |
| DualPivotQuickSort           | 4096 | SingleElementMoved |    15,210.2 ns |    713.13 ns |   316.63 ns |  0.83 |    0.02 |    3 |         - |          NA |
| StableQuickSort              | 4096 | SingleElementMoved |   159,920.7 ns |    805.43 ns |   357.61 ns |  8.70 |    0.13 |    9 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | SingleElementMoved |    98,271.0 ns |  5,025.68 ns | 2,628.53 ns |  5.35 |    0.16 |    7 |         - |          NA |
| DestswapStableQuickSort      | 4096 | SingleElementMoved |    79,229.5 ns |    913.72 ns |   405.70 ns |  4.31 |    0.07 |    6 |         - |          NA |
| IntroSort                    | 4096 | SingleElementMoved |     9,267.0 ns |    916.62 ns |   406.98 ns |  0.50 |    0.02 |    2 |         - |          NA |
| IntroSortDotnet              | 4096 | SingleElementMoved |    21,013.5 ns |    234.57 ns |   104.15 ns |  1.14 |    0.02 |    4 |         - |          NA |
| PDQSort                      | 4096 | SingleElementMoved |    12,722.8 ns |    213.06 ns |   111.43 ns |  0.69 |    0.01 |    3 |         - |          NA |
| PDQSortBranchless            | 4096 | SingleElementMoved |    14,336.4 ns |    426.17 ns |   222.89 ns |  0.78 |    0.02 |    3 |         - |          NA |
| Ipnsort                      | 4096 | SingleElementMoved |    70,431.0 ns |    435.28 ns |   227.66 ns |  3.83 |    0.06 |    6 |         - |          NA |
| StdSort                      | 4096 | SingleElementMoved |    13,055.6 ns |    193.13 ns |    85.75 ns |  0.71 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 4096 | SingleElementMoved |    52,599.6 ns |    276.28 ns |   144.50 ns |  2.86 |    0.04 |    5 |         - |          NA |
| FluxSort                     | 4096 | SingleElementMoved |     4,239.5 ns |    183.97 ns |    81.68 ns |  0.23 |    0.01 |    1 |         - |          NA |
| DotnetSort                   | 4096 | SingleElementMoved |    24,585.5 ns |    595.79 ns |   311.61 ns |  1.34 |    0.03 |    4 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Sorted**             |    **15,075.3 ns** |    **358.95 ns** |   **159.38 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Sorted             |    27,245.7 ns |  1,366.17 ns |   714.53 ns |  1.81 |    0.05 |    6 |         - |          NA |
| QuickSortMedian3             | 4096 | Sorted             |    19,658.6 ns |    248.60 ns |   130.02 ns |  1.30 |    0.02 |    5 |         - |          NA |
| QuickSortMedian9             | 4096 | Sorted             |    20,856.1 ns |    392.94 ns |   174.47 ns |  1.38 |    0.02 |    5 |         - |          NA |
| SampleSort                   | 4096 | Sorted             |    69,504.9 ns |  1,672.92 ns |   874.97 ns |  4.61 |    0.07 |    8 |         - |          NA |
| DualPivotQuickSort           | 4096 | Sorted             |    13,811.8 ns |  1,121.66 ns |   586.65 ns |  0.92 |    0.04 |    5 |         - |          NA |
| StableQuickSort              | 4096 | Sorted             |   159,677.1 ns |  1,572.22 ns |   698.07 ns | 10.59 |    0.11 |   10 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Sorted             |    94,920.4 ns |  1,327.00 ns |   589.20 ns |  6.30 |    0.07 |    9 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Sorted             |    78,036.0 ns |  2,070.09 ns | 1,082.70 ns |  5.18 |    0.08 |    8 |         - |          NA |
| IntroSort                    | 4096 | Sorted             |     3,310.0 ns |    276.33 ns |   144.53 ns |  0.22 |    0.01 |    3 |         - |          NA |
| IntroSortDotnet              | 4096 | Sorted             |    17,163.7 ns |    154.57 ns |    80.84 ns |  1.14 |    0.01 |    5 |         - |          NA |
| PDQSort                      | 4096 | Sorted             |     4,540.0 ns |     41.76 ns |    14.89 ns |  0.30 |    0.00 |    4 |         - |          NA |
| PDQSortBranchless            | 4096 | Sorted             |     4,557.1 ns |     55.49 ns |    19.79 ns |  0.30 |    0.00 |    4 |         - |          NA |
| Ipnsort                      | 4096 | Sorted             |     1,869.8 ns |    251.18 ns |   131.37 ns |  0.12 |    0.01 |    1 |         - |          NA |
| StdSort                      | 4096 | Sorted             |     4,639.7 ns |    240.76 ns |   106.90 ns |  0.31 |    0.01 |    4 |         - |          NA |
| BlockQuickSort               | 4096 | Sorted             |    34,594.3 ns |    371.94 ns |   194.53 ns |  2.29 |    0.03 |    7 |         - |          NA |
| FluxSort                     | 4096 | Sorted             |     2,210.2 ns |     54.00 ns |    19.26 ns |  0.15 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 4096 | Sorted             |    14,994.3 ns |    184.23 ns |    65.70 ns |  0.99 |    0.01 |    5 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Reversed**           |    **16,444.0 ns** |    **313.63 ns** |   **164.04 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Reversed           |    30,839.0 ns |  1,721.82 ns |   900.55 ns |  1.88 |    0.05 |    7 |         - |          NA |
| QuickSortMedian3             | 4096 | Reversed           |    20,705.0 ns |    324.66 ns |   169.80 ns |  1.26 |    0.02 |    6 |         - |          NA |
| QuickSortMedian9             | 4096 | Reversed           |    21,855.8 ns |    194.92 ns |   101.95 ns |  1.33 |    0.01 |    6 |         - |          NA |
| SampleSort                   | 4096 | Reversed           |    74,035.4 ns |  1,439.78 ns |   639.27 ns |  4.50 |    0.06 |    8 |         - |          NA |
| DualPivotQuickSort           | 4096 | Reversed           |    16,013.6 ns |    796.85 ns |   353.81 ns |  0.97 |    0.02 |    5 |         - |          NA |
| StableQuickSort              | 4096 | Reversed           |   160,213.3 ns |  1,399.93 ns |   621.58 ns |  9.74 |    0.10 |   10 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Reversed           |    93,825.8 ns |  3,288.23 ns | 1,719.81 ns |  5.71 |    0.11 |    9 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Reversed           |   110,464.4 ns |    731.71 ns |   382.70 ns |  6.72 |    0.07 |    9 |         - |          NA |
| IntroSort                    | 4096 | Reversed           |     5,429.7 ns |     38.26 ns |    13.65 ns |  0.33 |    0.00 |    3 |         - |          NA |
| IntroSortDotnet              | 4096 | Reversed           |    26,763.1 ns |    367.60 ns |   192.26 ns |  1.63 |    0.02 |    7 |         - |          NA |
| PDQSort                      | 4096 | Reversed           |     6,391.2 ns |    226.13 ns |   100.41 ns |  0.39 |    0.01 |    3 |         - |          NA |
| PDQSortBranchless            | 4096 | Reversed           |     9,981.1 ns |    291.80 ns |   152.62 ns |  0.61 |    0.01 |    4 |         - |          NA |
| Ipnsort                      | 4096 | Reversed           |     2,776.4 ns |     13.34 ns |     4.76 ns |  0.17 |    0.00 |    1 |         - |          NA |
| StdSort                      | 4096 | Reversed           |     9,242.7 ns |    266.71 ns |   139.49 ns |  0.56 |    0.01 |    4 |         - |          NA |
| BlockQuickSort               | 4096 | Reversed           |    37,252.2 ns |    413.82 ns |   216.43 ns |  2.27 |    0.02 |    7 |         - |          NA |
| FluxSort                     | 4096 | Reversed           |     3,499.3 ns |    328.88 ns |   172.01 ns |  0.21 |    0.01 |    2 |         - |          NA |
| DotnetSort                   | 4096 | Reversed           |    28,804.2 ns |  3,341.40 ns | 1,747.62 ns |  1.75 |    0.10 |    7 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **4096** | **PipeOrgan**          | **1,226,764.2 ns** |  **5,883.07 ns** | **3,076.96 ns** | **1.000** |    **0.00** |    **7** |         **-** |          **NA** |
| QuickSort3way                | 4096 | PipeOrgan          |    83,081.2 ns |  3,660.99 ns | 1,914.77 ns | 0.068 |    0.00 |    4 |         - |          NA |
| QuickSortMedian3             | 4096 | PipeOrgan          |    64,914.3 ns |  1,639.74 ns |   857.62 ns | 0.053 |    0.00 |    4 |         - |          NA |
| QuickSortMedian9             | 4096 | PipeOrgan          |    41,552.3 ns |    860.35 ns |   382.00 ns | 0.034 |    0.00 |    3 |         - |          NA |
| SampleSort                   | 4096 | PipeOrgan          |    74,913.9 ns |  1,763.67 ns |   922.44 ns | 0.061 |    0.00 |    4 |         - |          NA |
| DualPivotQuickSort           | 4096 | PipeOrgan          |    30,822.9 ns |    947.78 ns |   495.71 ns | 0.025 |    0.00 |    2 |         - |          NA |
| StableQuickSort              | 4096 | PipeOrgan          |   160,658.7 ns |    974.29 ns |   432.59 ns | 0.131 |    0.00 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | PipeOrgan          |    94,695.7 ns |  4,061.20 ns | 2,124.09 ns | 0.077 |    0.00 |    4 |         - |          NA |
| DestswapStableQuickSort      | 4096 | PipeOrgan          |   133,285.7 ns |    288.88 ns |   128.27 ns | 0.109 |    0.00 |    5 |         - |          NA |
| IntroSort                    | 4096 | PipeOrgan          |    26,760.0 ns |    733.96 ns |   325.88 ns | 0.022 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet              | 4096 | PipeOrgan          |    64,905.7 ns |    905.95 ns |   402.25 ns | 0.053 |    0.00 |    4 |         - |          NA |
| PDQSort                      | 4096 | PipeOrgan          |    32,548.1 ns |    528.36 ns |   234.59 ns | 0.027 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 4096 | PipeOrgan          |    59,520.8 ns |    224.89 ns |    99.85 ns | 0.049 |    0.00 |    4 |         - |          NA |
| Ipnsort                      | 4096 | PipeOrgan          |    83,600.5 ns |    505.10 ns |   224.27 ns | 0.068 |    0.00 |    4 |         - |          NA |
| StdSort                      | 4096 | PipeOrgan          |    85,899.4 ns |    981.20 ns |   435.66 ns | 0.070 |    0.00 |    4 |         - |          NA |
| BlockQuickSort               | 4096 | PipeOrgan          |    97,440.4 ns |  1,157.07 ns |   605.17 ns | 0.079 |    0.00 |    4 |         - |          NA |
| FluxSort                     | 4096 | PipeOrgan          |     8,972.8 ns |    224.10 ns |   117.21 ns | 0.007 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 4096 | PipeOrgan          |    71,244.3 ns |  2,271.64 ns | 1,188.11 ns | 0.058 |    0.00 |    4 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **4096** | **ManyDuplicates**     |   **101,181.2 ns** |    **960.86 ns** |   **502.55 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 4096 | ManyDuplicates     |    74,850.6 ns |    640.75 ns |   284.50 ns |  0.74 |    0.00 |    4 |         - |          NA |
| QuickSortMedian3             | 4096 | ManyDuplicates     |   113,071.9 ns |    323.44 ns |   143.61 ns |  1.12 |    0.01 |    5 |         - |          NA |
| QuickSortMedian9             | 4096 | ManyDuplicates     |   119,106.6 ns |    879.02 ns |   390.29 ns |  1.18 |    0.01 |    5 |         - |          NA |
| SampleSort                   | 4096 | ManyDuplicates     |   155,631.0 ns |  1,154.04 ns |   512.40 ns |  1.54 |    0.01 |    5 |         - |          NA |
| DualPivotQuickSort           | 4096 | ManyDuplicates     |    72,914.6 ns |    370.77 ns |   164.63 ns |  0.72 |    0.00 |    4 |         - |          NA |
| StableQuickSort              | 4096 | ManyDuplicates     |   182,888.9 ns |    896.70 ns |   398.14 ns |  1.81 |    0.01 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | ManyDuplicates     |   144,539.4 ns |    733.89 ns |   383.84 ns |  1.43 |    0.01 |    5 |         - |          NA |
| DestswapStableQuickSort      | 4096 | ManyDuplicates     |   140,330.2 ns |    293.04 ns |   153.27 ns |  1.39 |    0.01 |    5 |         - |          NA |
| IntroSort                    | 4096 | ManyDuplicates     |   104,967.5 ns |    628.75 ns |   279.17 ns |  1.04 |    0.01 |    5 |         - |          NA |
| IntroSortDotnet              | 4096 | ManyDuplicates     |   102,937.9 ns |    612.44 ns |   271.93 ns |  1.02 |    0.01 |    5 |         - |          NA |
| PDQSort                      | 4096 | ManyDuplicates     |    73,859.2 ns |    414.56 ns |   184.07 ns |  0.73 |    0.00 |    4 |         - |          NA |
| PDQSortBranchless            | 4096 | ManyDuplicates     |    32,002.3 ns |    227.62 ns |   101.07 ns |  0.32 |    0.00 |    1 |         - |          NA |
| Ipnsort                      | 4096 | ManyDuplicates     |    48,864.0 ns |     95.50 ns |    42.40 ns |  0.48 |    0.00 |    2 |         - |          NA |
| StdSort                      | 4096 | ManyDuplicates     |    42,534.3 ns |    359.92 ns |   188.25 ns |  0.42 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 4096 | ManyDuplicates     |    60,740.4 ns |    406.32 ns |   212.51 ns |  0.60 |    0.00 |    3 |         - |          NA |
| FluxSort                     | 4096 | ManyDuplicates     |    33,808.1 ns |    182.07 ns |    95.23 ns |  0.33 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 4096 | ManyDuplicates     |    94,414.6 ns |    506.82 ns |   265.08 ns |  0.93 |    0.01 |    5 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Random**             |   **404,227.4 ns** |  **3,157.99 ns** | **1,651.69 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Random             |   433,170.6 ns |  5,783.17 ns | 3,024.71 ns |  1.07 |    0.01 |    2 |         - |          NA |
| QuickSortMedian3             | 8192 | Random             |   410,080.8 ns |  1,470.65 ns |   769.18 ns |  1.01 |    0.00 |    2 |         - |          NA |
| QuickSortMedian9             | 8192 | Random             |   438,193.6 ns |  3,125.96 ns | 1,634.94 ns |  1.08 |    0.01 |    2 |         - |          NA |
| SampleSort                   | 8192 | Random             |   498,307.6 ns |  1,130.05 ns |   501.75 ns |  1.23 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort           | 8192 | Random             |   302,498.1 ns |  2,577.31 ns | 1,347.98 ns |  0.75 |    0.00 |    2 |         - |          NA |
| StableQuickSort              | 8192 | Random             | 1,023,584.1 ns |  1,592.73 ns |   707.18 ns |  2.53 |    0.01 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Random             |   816,475.2 ns |  4,070.74 ns | 1,807.43 ns |  2.02 |    0.01 |    3 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Random             |   710,357.0 ns |  1,504.39 ns |   786.82 ns |  1.76 |    0.01 |    3 |         - |          NA |
| IntroSort                    | 8192 | Random             |   350,043.4 ns |  1,267.62 ns |   662.99 ns |  0.87 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet              | 8192 | Random             |   336,653.5 ns |    993.35 ns |   519.54 ns |  0.83 |    0.00 |    2 |         - |          NA |
| PDQSort                      | 8192 | Random             |   323,131.4 ns |  6,572.54 ns | 2,918.25 ns |  0.80 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | Random             |   191,613.1 ns |  1,001.34 ns |   523.72 ns |  0.47 |    0.00 |    1 |         - |          NA |
| Ipnsort                      | 8192 | Random             |   194,981.6 ns |    576.93 ns |   301.75 ns |  0.48 |    0.00 |    1 |         - |          NA |
| StdSort                      | 8192 | Random             |   189,848.5 ns |    692.42 ns |   307.44 ns |  0.47 |    0.00 |    1 |         - |          NA |
| BlockQuickSort               | 8192 | Random             |   203,993.9 ns |  1,604.76 ns |   839.32 ns |  0.50 |    0.00 |    1 |         - |          NA |
| FluxSort                     | 8192 | Random             |   226,118.3 ns |  1,913.63 ns | 1,000.86 ns |  0.56 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 8192 | Random             |   314,868.6 ns |  1,545.48 ns |   686.20 ns |  0.78 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **SingleElementMoved** |    **38,387.2 ns** |    **976.20 ns** |   **510.57 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 8192 | SingleElementMoved |    69,980.6 ns |  4,701.19 ns | 2,458.81 ns |  1.82 |    0.06 |    3 |         - |          NA |
| QuickSortMedian3             | 8192 | SingleElementMoved |    54,650.1 ns |    395.33 ns |   206.77 ns |  1.42 |    0.02 |    3 |         - |          NA |
| QuickSortMedian9             | 8192 | SingleElementMoved |    63,203.1 ns |  1,146.14 ns |   599.45 ns |  1.65 |    0.03 |    3 |         - |          NA |
| SampleSort                   | 8192 | SingleElementMoved |   320,529.0 ns |  1,720.16 ns |   899.68 ns |  8.35 |    0.11 |    7 |         - |          NA |
| DualPivotQuickSort           | 8192 | SingleElementMoved |    32,855.2 ns |  2,067.57 ns | 1,081.38 ns |  0.86 |    0.03 |    3 |         - |          NA |
| StableQuickSort              | 8192 | SingleElementMoved |   347,804.9 ns |  2,211.12 ns | 1,156.46 ns |  9.06 |    0.12 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | SingleElementMoved |   207,270.6 ns |  3,443.94 ns | 1,529.13 ns |  5.40 |    0.08 |    6 |         - |          NA |
| DestswapStableQuickSort      | 8192 | SingleElementMoved |   164,741.8 ns |  1,981.85 ns |   879.95 ns |  4.29 |    0.06 |    5 |         - |          NA |
| IntroSort                    | 8192 | SingleElementMoved |    18,468.7 ns |    586.53 ns |   306.77 ns |  0.48 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 8192 | SingleElementMoved |    44,113.7 ns |    305.48 ns |   135.64 ns |  1.15 |    0.01 |    3 |         - |          NA |
| PDQSort                      | 8192 | SingleElementMoved |    25,051.1 ns |    466.82 ns |   244.15 ns |  0.65 |    0.01 |    3 |         - |          NA |
| PDQSortBranchless            | 8192 | SingleElementMoved |    28,572.0 ns |    464.95 ns |   243.18 ns |  0.74 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 8192 | SingleElementMoved |   154,435.9 ns |    697.94 ns |   309.89 ns |  4.02 |    0.05 |    5 |         - |          NA |
| StdSort                      | 8192 | SingleElementMoved |    25,870.8 ns |    321.08 ns |   167.93 ns |  0.67 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 8192 | SingleElementMoved |   111,391.6 ns |  1,450.93 ns |   758.87 ns |  2.90 |    0.04 |    4 |         - |          NA |
| FluxSort                     | 8192 | SingleElementMoved |     8,164.5 ns |    231.72 ns |   121.20 ns |  0.21 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 8192 | SingleElementMoved |    52,733.4 ns |    641.01 ns |   228.59 ns |  1.37 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Sorted**             |    **32,487.2 ns** |    **365.64 ns** |   **162.35 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Sorted             |    58,408.1 ns |  4,082.01 ns | 1,812.44 ns |  1.80 |    0.05 |    6 |         - |          NA |
| QuickSortMedian3             | 8192 | Sorted             |    41,310.3 ns |    183.79 ns |    81.60 ns |  1.27 |    0.01 |    5 |         - |          NA |
| QuickSortMedian9             | 8192 | Sorted             |    43,805.4 ns |    476.84 ns |   211.72 ns |  1.35 |    0.01 |    5 |         - |          NA |
| SampleSort                   | 8192 | Sorted             |   198,686.7 ns | 13,679.72 ns | 7,154.76 ns |  6.12 |    0.21 |    8 |         - |          NA |
| DualPivotQuickSort           | 8192 | Sorted             |    30,540.8 ns |  1,867.01 ns |   976.48 ns |  0.94 |    0.03 |    5 |         - |          NA |
| StableQuickSort              | 8192 | Sorted             |   347,144.7 ns |  2,287.65 ns | 1,015.73 ns | 10.69 |    0.06 |    9 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Sorted             |   206,211.9 ns |  6,107.44 ns | 3,194.31 ns |  6.35 |    0.10 |    8 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Sorted             |   170,410.6 ns |  1,849.93 ns |   967.55 ns |  5.25 |    0.04 |    8 |         - |          NA |
| IntroSort                    | 8192 | Sorted             |     6,351.3 ns |    241.45 ns |   107.20 ns |  0.20 |    0.00 |    3 |         - |          NA |
| IntroSortDotnet              | 8192 | Sorted             |    36,811.5 ns |    436.91 ns |   228.51 ns |  1.13 |    0.01 |    5 |         - |          NA |
| PDQSort                      | 8192 | Sorted             |     9,272.4 ns |    252.27 ns |   131.94 ns |  0.29 |    0.00 |    4 |         - |          NA |
| PDQSortBranchless            | 8192 | Sorted             |     9,282.8 ns |    363.18 ns |   189.95 ns |  0.29 |    0.01 |    4 |         - |          NA |
| Ipnsort                      | 8192 | Sorted             |     3,549.4 ns |     44.35 ns |    19.69 ns |  0.11 |    0.00 |    1 |         - |          NA |
| StdSort                      | 8192 | Sorted             |     9,271.2 ns |    355.84 ns |   186.11 ns |  0.29 |    0.01 |    4 |         - |          NA |
| BlockQuickSort               | 8192 | Sorted             |    74,323.8 ns |  1,010.80 ns |   528.67 ns |  2.29 |    0.02 |    7 |         - |          NA |
| FluxSort                     | 8192 | Sorted             |     4,369.4 ns |     92.47 ns |    32.98 ns |  0.13 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 8192 | Sorted             |    32,607.0 ns |  1,243.97 ns |   443.61 ns |  1.00 |    0.01 |    5 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Reversed**           |    **34,714.0 ns** |    **619.30 ns** |   **323.91 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Reversed           |    64,112.9 ns |  2,952.66 ns | 1,544.30 ns |  1.85 |    0.04 |    6 |         - |          NA |
| QuickSortMedian3             | 8192 | Reversed           |    43,619.5 ns |    643.52 ns |   336.57 ns |  1.26 |    0.01 |    5 |         - |          NA |
| QuickSortMedian9             | 8192 | Reversed           |    46,334.6 ns |    944.43 ns |   419.33 ns |  1.33 |    0.02 |    5 |         - |          NA |
| SampleSort                   | 8192 | Reversed           |   278,557.4 ns |  6,235.89 ns | 3,261.49 ns |  8.02 |    0.11 |    8 |         - |          NA |
| DualPivotQuickSort           | 8192 | Reversed           |    35,282.7 ns |  2,769.41 ns | 1,229.63 ns |  1.02 |    0.03 |    4 |         - |          NA |
| StableQuickSort              | 8192 | Reversed           |   351,950.3 ns |  9,958.28 ns | 4,421.54 ns | 10.14 |    0.15 |    9 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Reversed           |   201,618.6 ns |  7,577.18 ns | 3,364.32 ns |  5.81 |    0.10 |    8 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Reversed           |   236,264.5 ns |  1,939.81 ns |   861.29 ns |  6.81 |    0.06 |    8 |         - |          NA |
| IntroSort                    | 8192 | Reversed           |    11,333.7 ns |    985.01 ns |   515.18 ns |  0.33 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 8192 | Reversed           |    57,957.7 ns |    318.60 ns |   166.63 ns |  1.67 |    0.02 |    6 |         - |          NA |
| PDQSort                      | 8192 | Reversed           |    12,745.2 ns |    388.84 ns |   203.37 ns |  0.37 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | Reversed           |    19,652.2 ns |    355.74 ns |   157.95 ns |  0.57 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 8192 | Reversed           |     5,600.6 ns |    175.81 ns |    78.06 ns |  0.16 |    0.00 |    1 |         - |          NA |
| StdSort                      | 8192 | Reversed           |    18,290.4 ns |    451.05 ns |   200.27 ns |  0.53 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 8192 | Reversed           |    79,329.8 ns |    627.35 ns |   328.12 ns |  2.29 |    0.02 |    7 |         - |          NA |
| FluxSort                     | 8192 | Reversed           |     6,664.8 ns |     93.28 ns |    48.79 ns |  0.19 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 8192 | Reversed           |    59,765.8 ns |  3,337.10 ns | 1,481.69 ns |  1.72 |    0.04 |    6 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **PipeOrgan**          | **4,764,132.7 ns** |  **6,175.80 ns** | **3,230.07 ns** | **1.000** |    **0.00** |    **7** |         **-** |          **NA** |
| QuickSort3way                | 8192 | PipeOrgan          |   205,645.7 ns |  9,460.72 ns | 4,948.14 ns | 0.043 |    0.00 |    5 |         - |          NA |
| QuickSortMedian3             | 8192 | PipeOrgan          |   151,659.7 ns |  3,250.48 ns | 1,700.06 ns | 0.032 |    0.00 |    4 |         - |          NA |
| QuickSortMedian9             | 8192 | PipeOrgan          |    88,170.4 ns |  3,348.01 ns | 1,486.54 ns | 0.019 |    0.00 |    3 |         - |          NA |
| SampleSort                   | 8192 | PipeOrgan          |   302,720.2 ns |  3,424.64 ns | 1,520.56 ns | 0.064 |    0.00 |    6 |         - |          NA |
| DualPivotQuickSort           | 8192 | PipeOrgan          |    65,209.9 ns |  1,559.96 ns |   815.89 ns | 0.014 |    0.00 |    2 |         - |          NA |
| StableQuickSort              | 8192 | PipeOrgan          |   347,080.5 ns |  1,842.61 ns |   818.13 ns | 0.073 |    0.00 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | PipeOrgan          |   202,406.9 ns | 11,256.47 ns | 4,997.94 ns | 0.042 |    0.00 |    5 |         - |          NA |
| DestswapStableQuickSort      | 8192 | PipeOrgan          |   284,976.1 ns |    338.25 ns |   150.19 ns | 0.060 |    0.00 |    6 |         - |          NA |
| IntroSort                    | 8192 | PipeOrgan          |    63,288.3 ns |  4,448.64 ns | 2,326.72 ns | 0.013 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet              | 8192 | PipeOrgan          |   272,059.3 ns |  6,590.56 ns | 3,446.99 ns | 0.057 |    0.00 |    6 |         - |          NA |
| PDQSort                      | 8192 | PipeOrgan          |    71,432.9 ns |  1,593.16 ns |   833.25 ns | 0.015 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | PipeOrgan          |   130,702.3 ns |  1,537.44 ns |   682.63 ns | 0.027 |    0.00 |    4 |         - |          NA |
| Ipnsort                      | 8192 | PipeOrgan          |   185,499.2 ns |    449.81 ns |   199.72 ns | 0.039 |    0.00 |    5 |         - |          NA |
| StdSort                      | 8192 | PipeOrgan          |   267,242.8 ns |  2,974.71 ns | 1,555.83 ns | 0.056 |    0.00 |    6 |         - |          NA |
| BlockQuickSort               | 8192 | PipeOrgan          |   200,950.1 ns |    655.91 ns |   291.23 ns | 0.042 |    0.00 |    5 |         - |          NA |
| FluxSort                     | 8192 | PipeOrgan          |    17,925.3 ns |    229.36 ns |   119.96 ns | 0.004 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 8192 | PipeOrgan          |   280,255.1 ns |  7,638.97 ns | 3,995.33 ns | 0.059 |    0.00 |    6 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **ManyDuplicates**     |   **201,564.0 ns** |  **1,668.53 ns** |   **872.67 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| QuickSort3way                | 8192 | ManyDuplicates     |   146,572.1 ns |    530.53 ns |   235.56 ns |  0.73 |    0.00 |    5 |         - |          NA |
| QuickSortMedian3             | 8192 | ManyDuplicates     |   222,354.1 ns |    411.38 ns |   146.70 ns |  1.10 |    0.00 |    6 |         - |          NA |
| QuickSortMedian9             | 8192 | ManyDuplicates     |   234,760.9 ns |  1,634.06 ns |   725.53 ns |  1.16 |    0.01 |    6 |         - |          NA |
| SampleSort                   | 8192 | ManyDuplicates     |   311,039.6 ns |  2,669.86 ns | 1,396.39 ns |  1.54 |    0.01 |    6 |         - |          NA |
| DualPivotQuickSort           | 8192 | ManyDuplicates     |   145,544.3 ns |  2,286.27 ns | 1,195.76 ns |  0.72 |    0.01 |    5 |         - |          NA |
| StableQuickSort              | 8192 | ManyDuplicates     |   360,712.9 ns |  1,992.31 ns | 1,042.02 ns |  1.79 |    0.01 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | ManyDuplicates     |   283,612.1 ns |  1,900.98 ns |   844.05 ns |  1.41 |    0.01 |    6 |         - |          NA |
| DestswapStableQuickSort      | 8192 | ManyDuplicates     |   271,944.3 ns |  1,284.99 ns |   672.08 ns |  1.35 |    0.01 |    6 |         - |          NA |
| IntroSort                    | 8192 | ManyDuplicates     |   209,338.4 ns |  1,405.71 ns |   735.21 ns |  1.04 |    0.01 |    6 |         - |          NA |
| IntroSortDotnet              | 8192 | ManyDuplicates     |   208,178.3 ns |    969.24 ns |   430.35 ns |  1.03 |    0.00 |    6 |         - |          NA |
| PDQSort                      | 8192 | ManyDuplicates     |   144,649.3 ns |    563.70 ns |   250.29 ns |  0.72 |    0.00 |    5 |         - |          NA |
| PDQSortBranchless            | 8192 | ManyDuplicates     |    59,425.1 ns |    589.74 ns |   308.44 ns |  0.29 |    0.00 |    1 |         - |          NA |
| Ipnsort                      | 8192 | ManyDuplicates     |    93,722.5 ns |    518.13 ns |   230.05 ns |  0.46 |    0.00 |    3 |         - |          NA |
| StdSort                      | 8192 | ManyDuplicates     |    77,503.1 ns |    715.04 ns |   317.48 ns |  0.38 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | ManyDuplicates     |   114,329.1 ns |  1,362.55 ns |   604.98 ns |  0.57 |    0.00 |    4 |         - |          NA |
| FluxSort                     | 8192 | ManyDuplicates     |    56,291.2 ns |    327.97 ns |   171.53 ns |  0.28 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 8192 | ManyDuplicates     |   190,815.7 ns |  1,783.00 ns |   932.54 ns |  0.95 |    0.01 |    6 |         - |          NA |

### SelectionBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.58 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method              | Size | Pattern            | Mean           | Error       | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------------- |----- |------------------- |---------------:|------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **SelectionSort**       | **256**  | **Random**             |    **26,013.5 ns** |    **272.0 ns** |   **142.24 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Random             |    24,843.4 ns |    127.3 ns |    56.54 ns |  0.96 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | Random             |   106,695.8 ns |    330.1 ns |   146.59 ns |  4.10 |    0.02 |    3 |         - |          NA |
| PancakeSort         | 256  | Random             |    41,453.4 ns |    205.2 ns |   107.34 ns |  1.59 |    0.01 |    2 |         - |          NA |
| BingoSort           | 256  | Random             |    29,179.0 ns |    399.5 ns |   208.94 ns |  1.12 |    0.01 |    1 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **SelectionSort**       | **256**  | **SingleElementMoved** |    **19,381.6 ns** |    **162.6 ns** |    **72.18 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | SingleElementMoved |    16,295.0 ns |  1,493.7 ns |   532.67 ns |  0.84 |    0.03 |    1 |         - |          NA |
| CycleSort           | 256  | SingleElementMoved |    31,952.8 ns |    191.4 ns |    85.00 ns |  1.65 |    0.01 |    3 |         - |          NA |
| PancakeSort         | 256  | SingleElementMoved |    16,109.1 ns |    629.8 ns |   279.62 ns |  0.83 |    0.01 |    1 |         - |          NA |
| BingoSort           | 256  | SingleElementMoved |    19,516.5 ns |    103.1 ns |    53.94 ns |  1.01 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **SelectionSort**       | **256**  | **Sorted**             |    **18,995.6 ns** |    **210.4 ns** |    **93.43 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Sorted             |    14,100.8 ns |    159.4 ns |    83.39 ns |  0.74 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | Sorted             |    19,239.7 ns |    160.0 ns |    71.02 ns |  1.01 |    0.01 |    2 |         - |          NA |
| PancakeSort         | 256  | Sorted             |    13,095.2 ns |    240.9 ns |   125.98 ns |  0.69 |    0.01 |    1 |         - |          NA |
| BingoSort           | 256  | Sorted             |    19,256.6 ns |    128.7 ns |    57.12 ns |  1.01 |    0.01 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **SelectionSort**       | **256**  | **Reversed**           |    **16,791.2 ns** |    **318.1 ns** |   **141.26 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Reversed           |    14,180.8 ns |    192.3 ns |   100.57 ns |  0.84 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | Reversed           |    38,644.8 ns |  2,552.4 ns | 1,334.98 ns |  2.30 |    0.08 |    3 |         - |          NA |
| PancakeSort         | 256  | Reversed           |    13,164.6 ns |    260.4 ns |   115.61 ns |  0.78 |    0.01 |    1 |         - |          NA |
| BingoSort           | 256  | Reversed           |    22,406.8 ns |  3,606.6 ns | 1,886.34 ns |  1.33 |    0.11 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **SelectionSort**       | **256**  | **PipeOrgan**          |    **18,379.6 ns** |  **1,003.5 ns** |   **524.85 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | PipeOrgan          |    16,587.2 ns |    178.1 ns |    79.06 ns |  0.90 |    0.02 |    1 |         - |          NA |
| CycleSort           | 256  | PipeOrgan          |    56,016.1 ns |    799.2 ns |   418.02 ns |  3.05 |    0.08 |    3 |         - |          NA |
| PancakeSort         | 256  | PipeOrgan          |    28,707.4 ns |    301.7 ns |   157.80 ns |  1.56 |    0.04 |    2 |         - |          NA |
| BingoSort           | 256  | PipeOrgan          |    20,185.0 ns |  1,109.9 ns |   580.51 ns |  1.10 |    0.04 |    1 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **SelectionSort**       | **256**  | **ManyDuplicates**     |    **24,093.0 ns** |    **200.5 ns** |    **71.49 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | ManyDuplicates     |    23,369.0 ns |    233.8 ns |   122.29 ns |  0.97 |    0.01 |    2 |         - |          NA |
| CycleSort           | 256  | ManyDuplicates     |   107,256.3 ns |    686.7 ns |   304.91 ns |  4.45 |    0.02 |    4 |         - |          NA |
| PancakeSort         | 256  | ManyDuplicates     |    37,495.3 ns |    226.6 ns |   100.62 ns |  1.56 |    0.01 |    3 |         - |          NA |
| BingoSort           | 256  | ManyDuplicates     |     5,936.7 ns |    162.8 ns |    72.27 ns |  0.25 |    0.00 |    1 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Random**             |   **326,665.5 ns** |  **1,322.3 ns** |   **587.10 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Random             |   322,570.4 ns |  2,806.6 ns | 1,246.13 ns |  0.99 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | Random             | 1,449,165.9 ns |  3,372.3 ns | 1,497.30 ns |  4.44 |    0.01 |    3 |         - |          NA |
| PancakeSort         | 1024 | Random             |   552,018.0 ns |  2,861.1 ns | 1,270.36 ns |  1.69 |    0.00 |    2 |         - |          NA |
| BingoSort           | 1024 | Random             |   341,072.2 ns |  2,891.8 ns | 1,512.46 ns |  1.04 |    0.00 |    1 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **SelectionSort**       | **1024** | **SingleElementMoved** |   **293,196.3 ns** |  **1,416.9 ns** |   **629.13 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | SingleElementMoved |   237,849.5 ns |  1,564.7 ns |   818.37 ns |  0.81 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | SingleElementMoved |   487,731.6 ns |  9,190.1 ns | 4,080.44 ns |  1.66 |    0.01 |    2 |         - |          NA |
| PancakeSort         | 1024 | SingleElementMoved |   244,918.6 ns | 18,816.8 ns | 9,841.56 ns |  0.84 |    0.03 |    1 |         - |          NA |
| BingoSort           | 1024 | SingleElementMoved |   294,423.1 ns |  2,861.0 ns | 1,496.36 ns |  1.00 |    0.01 |    1 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Sorted**             |   **291,824.1 ns** |    **815.8 ns** |   **290.93 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Sorted             |   219,531.0 ns |  3,212.8 ns | 1,680.35 ns |  0.75 |    0.01 |    1 |         - |          NA |
| CycleSort           | 1024 | Sorted             |   292,749.4 ns |  2,234.4 ns | 1,168.61 ns |  1.00 |    0.00 |    2 |         - |          NA |
| PancakeSort         | 1024 | Sorted             |   196,887.3 ns |  1,548.3 ns |   809.81 ns |  0.67 |    0.00 |    1 |         - |          NA |
| BingoSort           | 1024 | Sorted             |   291,964.7 ns |    526.5 ns |   233.76 ns |  1.00 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Reversed**           |   **263,822.6 ns** | **10,782.6 ns** | **5,639.53 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Reversed           |   218,163.7 ns |  1,354.1 ns |   601.24 ns |  0.83 |    0.02 |    1 |         - |          NA |
| CycleSort           | 1024 | Reversed           |   576,075.2 ns | 10,332.0 ns | 5,403.86 ns |  2.18 |    0.05 |    4 |         - |          NA |
| PancakeSort         | 1024 | Reversed           |   205,698.3 ns | 14,155.1 ns | 7,403.39 ns |  0.78 |    0.03 |    1 |         - |          NA |
| BingoSort           | 1024 | Reversed           |   333,294.1 ns | 18,957.3 ns | 9,915.04 ns |  1.26 |    0.04 |    3 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **SelectionSort**       | **1024** | **PipeOrgan**          |   **302,200.8 ns** |  **3,230.2 ns** | **1,689.44 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | PipeOrgan          |   268,883.2 ns |  1,854.8 ns |   823.55 ns |  0.89 |    0.01 |    1 |         - |          NA |
| CycleSort           | 1024 | PipeOrgan          |   964,808.6 ns |  2,892.7 ns | 1,512.96 ns |  3.19 |    0.02 |    3 |         - |          NA |
| PancakeSort         | 1024 | PipeOrgan          |   439,667.5 ns |  2,668.8 ns | 1,395.82 ns |  1.45 |    0.01 |    2 |         - |          NA |
| BingoSort           | 1024 | PipeOrgan          |   337,216.3 ns |  7,943.3 ns | 4,154.48 ns |  1.12 |    0.01 |    1 |         - |          NA |
|      |                    |                |             |             |       |         |      |           |             |
| **SelectionSort**       | **1024** | **ManyDuplicates**     |   **314,459.5 ns** |  **3,379.5 ns** | **1,767.52 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | ManyDuplicates     |   311,016.3 ns |  3,276.6 ns | 1,454.84 ns |  0.99 |    0.01 |    2 |         - |          NA |
| CycleSort           | 1024 | ManyDuplicates     | 1,418,879.1 ns |  4,408.6 ns | 1,957.44 ns |  4.51 |    0.02 |    4 |         - |          NA |
| PancakeSort         | 1024 | ManyDuplicates     |   503,663.6 ns |  2,563.9 ns | 1,138.39 ns |  1.60 |    0.01 |    3 |         - |          NA |
| BingoSort           | 1024 | ManyDuplicates     |    20,296.9 ns |    109.6 ns |    57.34 ns |  0.06 |    0.00 |    1 |         - |          NA |

### TreeBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.58 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                 | Size | Pattern            | Mean         | Error       | StdDev    | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------- |----- |------------------- |-------------:|------------:|----------:|------:|--------:|-----:|----------:|------------:|
| **BalancedBinaryTreeSort** | **256**  | **Random**             |  **16,368.2 ns** |   **201.90 ns** |  **89.64 ns** |  **1.39** |    **0.02** |    **2** |         **-** |          **NA** |
| BTreeSort              | 256  | Random             |  15,883.6 ns |   239.32 ns | 125.17 ns |  1.35 |    0.02 |    2 |         - |          NA |
| BPlusTreeSort          | 256  | Random             |  15,320.2 ns |   321.47 ns | 168.13 ns |  1.30 |    0.02 |    2 |         - |          NA |
| CartesianTreeSort      | 256  | Random             |  15,428.4 ns |   227.82 ns | 119.16 ns |  1.31 |    0.02 |    2 |         - |          NA |
| BinaryTreeSort         | 256  | Random             |  11,770.4 ns |   234.39 ns | 122.59 ns |  1.00 |    0.00 |    1 |         - |          NA |
| SplaySort              | 256  | Random             |  30,556.3 ns |   210.64 ns | 110.17 ns |  2.60 |    0.03 |    3 |         - |          NA |
| TreapSort              | 256  | Random             |  17,982.6 ns |   166.38 ns |  87.02 ns |  1.53 |    0.02 |    2 |         - |          NA |
|      |                    |              |             |           |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **SingleElementMoved** |   **6,583.1 ns** |   **354.14 ns** | **185.22 ns** |  **0.12** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 256  | SingleElementMoved |   4,672.5 ns |    58.24 ns |  20.77 ns |  0.09 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 256  | SingleElementMoved |   4,329.7 ns |   310.69 ns | 162.50 ns |  0.08 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 256  | SingleElementMoved |   1,717.7 ns |    19.46 ns |   8.64 ns |  0.03 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | SingleElementMoved |  53,737.8 ns |   264.60 ns | 117.48 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 256  | SingleElementMoved |   3,350.2 ns |    54.46 ns |  24.18 ns |  0.06 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | SingleElementMoved |   4,686.1 ns |   313.44 ns | 163.93 ns |  0.09 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |           |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Sorted**             |   **6,052.4 ns** |   **219.64 ns** | **114.88 ns** |  **0.10** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 256  | Sorted             |   4,545.9 ns |   313.37 ns | 163.90 ns |  0.08 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 256  | Sorted             |   4,024.5 ns |   172.90 ns |  76.77 ns |  0.07 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 256  | Sorted             |   1,548.4 ns |     6.20 ns |   2.75 ns |  0.03 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | Sorted             |  58,593.0 ns |   258.95 ns | 114.97 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 256  | Sorted             |   3,050.6 ns |   201.28 ns |  89.37 ns |  0.05 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | Sorted             |   3,977.0 ns |   196.59 ns | 102.82 ns |  0.07 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |           |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Reversed**           |   **5,956.0 ns** |   **333.41 ns** | **174.38 ns** |  **0.10** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 256  | Reversed           |   7,888.9 ns |    69.79 ns |  30.99 ns |  0.13 |    0.00 |    4 |         - |          NA |
| BPlusTreeSort          | 256  | Reversed           |   6,582.1 ns |    78.92 ns |  41.28 ns |  0.11 |    0.00 |    4 |         - |          NA |
| CartesianTreeSort      | 256  | Reversed           |   1,512.5 ns |    11.09 ns |   4.92 ns |  0.02 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | Reversed           |  61,895.4 ns |   340.51 ns | 178.10 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 256  | Reversed           |   2,886.2 ns |    26.53 ns |   9.46 ns |  0.05 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | Reversed           |   4,027.0 ns |   132.33 ns |  58.75 ns |  0.07 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |           |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **PipeOrgan**          |   **5,324.6 ns** |   **167.99 ns** |  **87.86 ns** |  **0.18** |    **0.00** |    **3** |         **-** |          **NA** |
| BTreeSort              | 256  | PipeOrgan          |   5,365.2 ns |   246.56 ns | 128.95 ns |  0.18 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 256  | PipeOrgan          |   4,982.6 ns |   369.38 ns | 193.19 ns |  0.17 |    0.01 |    3 |         - |          NA |
| CartesianTreeSort      | 256  | PipeOrgan          |   1,722.2 ns |    14.01 ns |   4.99 ns |  0.06 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | PipeOrgan          |  29,239.6 ns |   405.31 ns | 179.96 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 256  | PipeOrgan          |   3,422.4 ns |   190.76 ns |  84.70 ns |  0.12 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | PipeOrgan          |   5,330.2 ns |   225.28 ns | 117.82 ns |  0.18 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |           |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **ManyDuplicates**     |  **15,610.9 ns** |   **108.88 ns** |  **48.34 ns** |  **1.49** |    **0.02** |    **2** |         **-** |          **NA** |
| BTreeSort              | 256  | ManyDuplicates     |  15,561.1 ns |   318.04 ns | 166.34 ns |  1.48 |    0.03 |    2 |         - |          NA |
| BPlusTreeSort          | 256  | ManyDuplicates     |  14,620.7 ns |   211.45 ns |  93.88 ns |  1.39 |    0.02 |    2 |         - |          NA |
| CartesianTreeSort      | 256  | ManyDuplicates     |  17,830.2 ns |   154.57 ns |  68.63 ns |  1.70 |    0.02 |    2 |         - |          NA |
| BinaryTreeSort         | 256  | ManyDuplicates     |  10,506.2 ns |   287.12 ns | 150.17 ns |  1.00 |    0.00 |    1 |         - |          NA |
| SplaySort              | 256  | ManyDuplicates     |  28,467.4 ns |   287.50 ns | 150.37 ns |  2.71 |    0.04 |    3 |         - |          NA |
| TreapSort              | 256  | ManyDuplicates     |  17,747.9 ns |   216.13 ns | 113.04 ns |  1.69 |    0.03 |    2 |         - |          NA |
|      |                    |              |             |           |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Random**             |  **74,175.9 ns** |   **506.01 ns** | **264.65 ns** |  **1.35** |    **0.01** |    **2** |         **-** |          **NA** |
| BTreeSort              | 1024 | Random             |  75,510.8 ns |   341.61 ns | 151.68 ns |  1.37 |    0.01 |    2 |         - |          NA |
| BPlusTreeSort          | 1024 | Random             |  73,442.4 ns |   452.82 ns | 236.84 ns |  1.34 |    0.01 |    2 |         - |          NA |
| CartesianTreeSort      | 1024 | Random             |  75,380.0 ns |   303.27 ns | 158.62 ns |  1.37 |    0.01 |    2 |         - |          NA |
| BinaryTreeSort         | 1024 | Random             |  54,978.9 ns |   409.58 ns | 214.22 ns |  1.00 |    0.00 |    1 |         - |          NA |
| SplaySort              | 1024 | Random             | 155,115.4 ns | 1,765.71 ns | 923.50 ns |  2.82 |    0.02 |    3 |         - |          NA |
| TreapSort              | 1024 | Random             |  81,747.8 ns |   909.54 ns | 475.71 ns |  1.49 |    0.01 |    2 |         - |          NA |
|      |                    |              |             |           |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **SingleElementMoved** |  **28,248.6 ns** |   **230.63 ns** | **120.62 ns** | **0.033** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 1024 | SingleElementMoved |  21,193.2 ns |   139.83 ns |  73.13 ns | 0.025 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 1024 | SingleElementMoved |  19,619.2 ns |    88.07 ns |  39.10 ns | 0.023 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 1024 | SingleElementMoved |   6,565.7 ns |   287.36 ns | 150.29 ns | 0.008 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | SingleElementMoved | 852,136.1 ns | 1,821.85 ns | 952.86 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | SingleElementMoved |  13,101.8 ns |   273.93 ns | 143.27 ns | 0.015 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | SingleElementMoved |  20,070.3 ns |   932.36 ns | 487.64 ns | 0.024 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |           |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Sorted**             |  **27,338.8 ns** |   **399.32 ns** | **208.85 ns** | **0.029** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 1024 | Sorted             |  20,242.2 ns |   304.16 ns | 159.08 ns | 0.022 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 1024 | Sorted             |  18,909.6 ns |   100.47 ns |  52.55 ns | 0.020 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 1024 | Sorted             |   5,943.3 ns |    15.83 ns |   5.65 ns | 0.006 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | Sorted             | 933,766.2 ns |   433.28 ns | 192.38 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | Sorted             |  12,100.0 ns |   347.12 ns | 154.12 ns | 0.013 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | Sorted             |  17,871.2 ns |   588.61 ns | 307.85 ns | 0.019 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |           |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Reversed**           |  **27,222.3 ns** |   **547.16 ns** | **286.17 ns** | **0.027** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 1024 | Reversed           |  31,337.8 ns |   195.10 ns |  86.63 ns | 0.032 |    0.00 |    4 |         - |          NA |
| BPlusTreeSort          | 1024 | Reversed           |  28,966.2 ns |   280.64 ns | 146.78 ns | 0.029 |    0.00 |    4 |         - |          NA |
| CartesianTreeSort      | 1024 | Reversed           |   5,854.9 ns |    14.15 ns |   5.05 ns | 0.006 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | Reversed           | 991,851.7 ns | 1,279.90 ns | 669.41 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | Reversed           |  11,522.8 ns |   264.51 ns | 138.35 ns | 0.012 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | Reversed           |  17,054.3 ns |   124.29 ns |  65.01 ns | 0.017 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |           |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **PipeOrgan**          |  **23,731.8 ns** |   **964.95 ns** | **504.69 ns** |  **0.05** |    **0.00** |    **3** |         **-** |          **NA** |
| BTreeSort              | 1024 | PipeOrgan          |  23,419.0 ns |   352.88 ns | 184.56 ns |  0.05 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 1024 | PipeOrgan          |  21,787.5 ns |   325.31 ns | 170.15 ns |  0.05 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 1024 | PipeOrgan          |   6,591.1 ns |   199.56 ns |  88.60 ns |  0.01 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | PipeOrgan          | 468,551.3 ns |   729.77 ns | 324.02 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | PipeOrgan          |  13,367.9 ns |   134.03 ns |  70.10 ns |  0.03 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | PipeOrgan          |  24,601.9 ns |   304.68 ns | 108.65 ns |  0.05 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |           |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **ManyDuplicates**     |  **65,132.7 ns** |   **381.75 ns** | **169.50 ns** |  **1.11** |    **0.01** |    **1** |         **-** |          **NA** |
| BTreeSort              | 1024 | ManyDuplicates     |  68,363.3 ns |   411.27 ns | 215.10 ns |  1.17 |    0.01 |    1 |         - |          NA |
| BPlusTreeSort          | 1024 | ManyDuplicates     |  65,704.4 ns |   188.56 ns |  98.62 ns |  1.12 |    0.01 |    1 |         - |          NA |
| CartesianTreeSort      | 1024 | ManyDuplicates     |  84,128.2 ns |   483.31 ns | 214.59 ns |  1.44 |    0.01 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | ManyDuplicates     |  58,444.9 ns |   648.59 ns | 287.98 ns |  1.00 |    0.00 |    1 |         - |          NA |
| SplaySort              | 1024 | ManyDuplicates     | 122,850.5 ns | 1,779.42 ns | 790.07 ns |  2.10 |    0.02 |    2 |         - |          NA |
| TreapSort              | 1024 | ManyDuplicates     |  78,499.1 ns |   501.49 ns | 262.29 ns |  1.34 |    0.01 |    1 |         - |          NA |

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
