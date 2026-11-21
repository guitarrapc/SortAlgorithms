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
<summary>Benchmark results (2026-11-21 08:26 UTC)</summary>

Workflow run: https://github.com/guitarrapc/SortAlgorithms/actions/runs/44882177098

### DistributionBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.46 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8
UnrollFactor=1  WarmupCount=2
```

| Method              | Size | Pattern            | Mean           | Error        | StdDev       | Ratio  | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------------- |----- |------------------- |---------------:|-------------:|-------------:|-------:|--------:|-----:|----------:|------------:|
| **CountingSort**        | **256**  | **Random**             |     **1,293.7 ns** |     **24.06 ns** |     **12.59 ns** |   **1.45** |    **0.02** |    **1** |         **-** |          **NA** |
| CountingSortInteger | 256  | Random             |       892.6 ns |     30.13 ns |     13.38 ns |   1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 256  | Random             |     1,146.0 ns |     29.36 ns |     13.04 ns |   1.28 |    0.02 |    1 |         - |          NA |
| PigeonSortInteger   | 256  | Random             |     1,113.2 ns |     73.39 ns |     38.38 ns |   1.25 |    0.04 |    1 |         - |          NA |
| BucketSort          | 256  | Random             |     1,428.8 ns |     36.20 ns |     16.07 ns |   1.60 |    0.03 |    1 |         - |          NA |
| BucketSortInteger   | 256  | Random             |     1,194.4 ns |     24.27 ns |      8.66 ns |   1.34 |    0.02 |    1 |         - |          NA |
| FlashSort           | 256  | Random             |     4,262.1 ns |    264.84 ns |    138.52 ns |   4.78 |    0.16 |    4 |         - |          NA |
| RadixLSD4Sort       | 256  | Random             |     2,225.8 ns |     19.85 ns |      7.08 ns |   2.49 |    0.04 |    2 |         - |          NA |
| RotateLSDRadixSort  | 256  | Random             |    53,042.7 ns |    488.08 ns |    216.71 ns |  59.43 |    0.86 |    6 |         - |          NA |
| RadixLSD256Sort     | 256  | Random             |     1,061.8 ns |     11.12 ns |      4.94 ns |   1.19 |    0.02 |    1 |         - |          NA |
| Radsort             | 256  | Random             |     7,492.5 ns |     28.86 ns |     15.09 ns |   8.40 |    0.12 |    5 |         - |          NA |
| RadixLSD10Sort      | 256  | Random             |     3,201.9 ns |     40.48 ns |     14.43 ns |   3.59 |    0.05 |    3 |         - |          NA |
| RadixMSD4Sort       | 256  | Random             |     5,250.2 ns |    273.28 ns |    142.93 ns |   5.88 |    0.17 |    4 |         - |          NA |
| RadixMSD10Sort      | 256  | Random             |     4,658.3 ns |     30.88 ns |     11.01 ns |   5.22 |    0.07 |    4 |         - |          NA |
| AmericanFlagSort    | 256  | Random             |     2,407.0 ns |     46.43 ns |     16.56 ns |   2.70 |    0.04 |    2 |         - |          NA |
| SpreadSort          | 256  | Random             |     6,008.5 ns |    161.95 ns |     71.90 ns |   6.73 |    0.12 |    4 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **256**  | **SingleElementMoved** |     **1,267.8 ns** |      **9.36 ns** |      **4.15 ns** |   **1.62** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | SingleElementMoved |       781.6 ns |     14.42 ns |      6.40 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 256  | SingleElementMoved |     1,118.1 ns |     26.99 ns |      9.62 ns |   1.43 |    0.02 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | SingleElementMoved |       626.4 ns |      2.55 ns |      0.91 ns |   0.80 |    0.01 |    1 |         - |          NA |
| BucketSort          | 256  | SingleElementMoved |     1,396.9 ns |     32.03 ns |     14.22 ns |   1.79 |    0.02 |    3 |         - |          NA |
| BucketSortInteger   | 256  | SingleElementMoved |     1,113.8 ns |      1.54 ns |      0.55 ns |   1.43 |    0.01 |    3 |         - |          NA |
| FlashSort           | 256  | SingleElementMoved |     3,206.4 ns |    180.97 ns |     80.35 ns |   4.10 |    0.10 |    6 |         - |          NA |
| RadixLSD4Sort       | 256  | SingleElementMoved |     2,095.8 ns |     85.39 ns |     37.91 ns |   2.68 |    0.05 |    5 |         - |          NA |
| RotateLSDRadixSort  | 256  | SingleElementMoved |    34,812.1 ns |    132.26 ns |     58.72 ns |  44.54 |    0.35 |    9 |         - |          NA |
| RadixLSD256Sort     | 256  | SingleElementMoved |       998.9 ns |     12.47 ns |      5.54 ns |   1.28 |    0.01 |    3 |         - |          NA |
| Radsort             | 256  | SingleElementMoved |     7,311.4 ns |     32.71 ns |     14.52 ns |   9.36 |    0.07 |    8 |         - |          NA |
| RadixLSD10Sort      | 256  | SingleElementMoved |     3,240.2 ns |    281.12 ns |    147.03 ns |   4.15 |    0.18 |    6 |         - |          NA |
| RadixMSD4Sort       | 256  | SingleElementMoved |     6,651.3 ns |     52.95 ns |     23.51 ns |   8.51 |    0.07 |    8 |         - |          NA |
| RadixMSD10Sort      | 256  | SingleElementMoved |     4,798.7 ns |    242.02 ns |    126.58 ns |   6.14 |    0.16 |    7 |         - |          NA |
| AmericanFlagSort    | 256  | SingleElementMoved |     1,679.7 ns |     16.23 ns |      7.20 ns |   2.15 |    0.02 |    4 |         - |          NA |
| SpreadSort          | 256  | SingleElementMoved |       821.9 ns |    129.95 ns |     67.97 ns |   1.05 |    0.08 |    2 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **256**  | **Sorted**             |     **1,203.5 ns** |     **53.77 ns** |     **28.12 ns** |   **1.57** |    **0.04** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | Sorted             |       769.0 ns |     33.27 ns |     14.77 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 256  | Sorted             |     1,111.3 ns |      9.21 ns |      4.09 ns |   1.45 |    0.03 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | Sorted             |       562.1 ns |     17.05 ns |      7.57 ns |   0.73 |    0.02 |    2 |         - |          NA |
| BucketSort          | 256  | Sorted             |     1,304.3 ns |     33.03 ns |     11.78 ns |   1.70 |    0.03 |    3 |         - |          NA |
| BucketSortInteger   | 256  | Sorted             |     1,047.1 ns |     12.40 ns |      6.49 ns |   1.36 |    0.03 |    3 |         - |          NA |
| FlashSort           | 256  | Sorted             |     2,771.8 ns |     21.62 ns |      9.60 ns |   3.61 |    0.06 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | Sorted             |     2,060.1 ns |     24.83 ns |     12.99 ns |   2.68 |    0.05 |    4 |         - |          NA |
| RotateLSDRadixSort  | 256  | Sorted             |    34,405.6 ns |    372.67 ns |    194.91 ns |  44.76 |    0.82 |    8 |         - |          NA |
| RadixLSD256Sort     | 256  | Sorted             |       889.4 ns |      7.79 ns |      3.46 ns |   1.16 |    0.02 |    3 |         - |          NA |
| Radsort             | 256  | Sorted             |     7,401.2 ns |     61.79 ns |     27.43 ns |   9.63 |    0.17 |    7 |         - |          NA |
| RadixLSD10Sort      | 256  | Sorted             |     3,048.1 ns |     52.27 ns |     18.64 ns |   3.97 |    0.07 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | Sorted             |     5,570.2 ns |    326.05 ns |    170.53 ns |   7.25 |    0.25 |    6 |         - |          NA |
| RadixMSD10Sort      | 256  | Sorted             |     4,705.4 ns |    270.15 ns |    141.29 ns |   6.12 |    0.20 |    6 |         - |          NA |
| AmericanFlagSort    | 256  | Sorted             |     1,280.9 ns |     12.63 ns |      5.61 ns |   1.67 |    0.03 |    3 |         - |          NA |
| SpreadSort          | 256  | Sorted             |       253.5 ns |      5.71 ns |      2.98 ns |   0.33 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **256**  | **Reversed**           |     **1,267.3 ns** |     **12.47 ns** |      **5.54 ns** |   **1.69** |    **0.01** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 256  | Reversed           |       751.8 ns |      4.32 ns |      1.92 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 256  | Reversed           |     1,160.6 ns |     17.89 ns |      7.94 ns |   1.54 |    0.01 |    4 |         - |          NA |
| PigeonSortInteger   | 256  | Reversed           |       559.3 ns |      4.28 ns |      2.24 ns |   0.74 |    0.00 |    2 |         - |          NA |
| BucketSort          | 256  | Reversed           |     1,382.3 ns |      3.78 ns |      1.35 ns |   1.84 |    0.00 |    4 |         - |          NA |
| BucketSortInteger   | 256  | Reversed           |     1,070.9 ns |     12.37 ns |      6.47 ns |   1.42 |    0.01 |    4 |         - |          NA |
| FlashSort           | 256  | Reversed           |     2,773.9 ns |    261.10 ns |    136.56 ns |   3.69 |    0.17 |    6 |         - |          NA |
| RadixLSD4Sort       | 256  | Reversed           |     2,230.6 ns |     85.78 ns |     38.09 ns |   2.97 |    0.05 |    5 |         - |          NA |
| RotateLSDRadixSort  | 256  | Reversed           |    35,033.9 ns |    381.26 ns |    169.28 ns |  46.60 |    0.24 |    9 |         - |          NA |
| RadixLSD256Sort     | 256  | Reversed           |       923.7 ns |      7.27 ns |      3.23 ns |   1.23 |    0.00 |    4 |         - |          NA |
| Radsort             | 256  | Reversed           |     7,501.6 ns |     37.16 ns |     19.44 ns |   9.98 |    0.03 |    8 |         - |          NA |
| RadixLSD10Sort      | 256  | Reversed           |     3,033.1 ns |     22.23 ns |      7.93 ns |   4.03 |    0.01 |    6 |         - |          NA |
| RadixMSD4Sort       | 256  | Reversed           |     5,509.1 ns |    124.55 ns |     55.30 ns |   7.33 |    0.07 |    7 |         - |          NA |
| RadixMSD10Sort      | 256  | Reversed           |     4,668.7 ns |    361.09 ns |    160.33 ns |   6.21 |    0.20 |    7 |         - |          NA |
| AmericanFlagSort    | 256  | Reversed           |     1,343.4 ns |     10.78 ns |      4.78 ns |   1.79 |    0.01 |    4 |         - |          NA |
| SpreadSort          | 256  | Reversed           |       397.9 ns |     20.76 ns |     10.86 ns |   0.53 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **256**  | **PipeOrgan**          |     **1,213.6 ns** |     **12.53 ns** |      **5.56 ns** |   **1.54** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | PipeOrgan          |       789.1 ns |     10.02 ns |      5.24 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 256  | PipeOrgan          |     1,112.7 ns |      5.87 ns |      2.09 ns |   1.41 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | PipeOrgan          |       625.2 ns |      5.01 ns |      2.62 ns |   0.79 |    0.01 |    1 |         - |          NA |
| BucketSort          | 256  | PipeOrgan          |     1,349.8 ns |     34.76 ns |     15.44 ns |   1.71 |    0.02 |    3 |         - |          NA |
| BucketSortInteger   | 256  | PipeOrgan          |     1,122.6 ns |      7.65 ns |      3.40 ns |   1.42 |    0.01 |    3 |         - |          NA |
| FlashSort           | 256  | PipeOrgan          |     3,460.0 ns |    213.61 ns |    111.72 ns |   4.39 |    0.14 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | PipeOrgan          |     2,154.6 ns |     17.36 ns |      7.71 ns |   2.73 |    0.02 |    4 |         - |          NA |
| RotateLSDRadixSort  | 256  | PipeOrgan          |    34,699.1 ns |    479.37 ns |    250.72 ns |  43.98 |    0.41 |    8 |         - |          NA |
| RadixLSD256Sort     | 256  | PipeOrgan          |       993.2 ns |      5.04 ns |      2.24 ns |   1.26 |    0.01 |    3 |         - |          NA |
| Radsort             | 256  | PipeOrgan          |     7,358.2 ns |     31.53 ns |     16.49 ns |   9.33 |    0.06 |    7 |         - |          NA |
| RadixLSD10Sort      | 256  | PipeOrgan          |     3,120.2 ns |     32.04 ns |     14.23 ns |   3.95 |    0.03 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | PipeOrgan          |     5,585.9 ns |    201.67 ns |    105.48 ns |   7.08 |    0.13 |    6 |         - |          NA |
| RadixMSD10Sort      | 256  | PipeOrgan          |     4,639.4 ns |     18.84 ns |      6.72 ns |   5.88 |    0.04 |    6 |         - |          NA |
| AmericanFlagSort    | 256  | PipeOrgan          |     1,873.9 ns |     39.19 ns |     17.40 ns |   2.37 |    0.03 |    4 |         - |          NA |
| SpreadSort          | 256  | PipeOrgan          |     1,383.9 ns |     51.49 ns |     22.86 ns |   1.75 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **256**  | **ManyDuplicates**     |     **1,212.3 ns** |     **22.46 ns** |      **9.97 ns** |   **1.36** |    **0.03** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | ManyDuplicates     |       891.3 ns |     36.94 ns |     16.40 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 256  | ManyDuplicates     |     1,533.6 ns |    366.37 ns |    191.62 ns |   1.72 |    0.21 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | ManyDuplicates     |       755.4 ns |    132.50 ns |     69.30 ns |   0.85 |    0.07 |    1 |         - |          NA |
| BucketSort          | 256  | ManyDuplicates     |     2,186.0 ns |     76.11 ns |     27.14 ns |   2.45 |    0.05 |    3 |         - |          NA |
| BucketSortInteger   | 256  | ManyDuplicates     |     1,334.1 ns |     73.16 ns |     38.26 ns |   1.50 |    0.05 |    2 |         - |          NA |
| FlashSort           | 256  | ManyDuplicates     |     3,746.5 ns |     74.19 ns |     32.94 ns |   4.20 |    0.08 |    4 |         - |          NA |
| RadixLSD4Sort       | 256  | ManyDuplicates     |     1,752.3 ns |     21.12 ns |      7.53 ns |   1.97 |    0.03 |    2 |         - |          NA |
| RotateLSDRadixSort  | 256  | ManyDuplicates     |    46,185.6 ns |    342.32 ns |    151.99 ns |  51.83 |    0.89 |    7 |         - |          NA |
| RadixLSD256Sort     | 256  | ManyDuplicates     |     1,031.4 ns |     19.00 ns |      8.44 ns |   1.16 |    0.02 |    2 |         - |          NA |
| Radsort             | 256  | ManyDuplicates     |     8,737.3 ns |    171.32 ns |     76.07 ns |   9.81 |    0.18 |    6 |         - |          NA |
| RadixLSD10Sort      | 256  | ManyDuplicates     |     2,268.4 ns |     21.73 ns |      7.75 ns |   2.55 |    0.04 |    3 |         - |          NA |
| RadixMSD4Sort       | 256  | ManyDuplicates     |     2,573.6 ns |     22.03 ns |      7.86 ns |   2.89 |    0.05 |    3 |         - |          NA |
| RadixMSD10Sort      | 256  | ManyDuplicates     |     2,952.7 ns |     32.63 ns |     14.49 ns |   3.31 |    0.06 |    3 |         - |          NA |
| AmericanFlagSort    | 256  | ManyDuplicates     |     2,453.7 ns |     84.88 ns |     44.39 ns |   2.75 |    0.07 |    3 |         - |          NA |
| SpreadSort          | 256  | ManyDuplicates     |     5,698.4 ns |    270.28 ns |    120.01 ns |   6.39 |    0.17 |    5 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **1024** | **Random**             |     **4,777.3 ns** |    **238.28 ns** |    **124.63 ns** |   **1.37** |    **0.07** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Random             |     3,494.7 ns |    331.84 ns |    173.56 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | Random             |     3,947.8 ns |     21.53 ns |     11.26 ns |   1.13 |    0.05 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | Random             |     2,529.4 ns |     14.50 ns |      7.58 ns |   0.73 |    0.03 |    1 |         - |          NA |
| BucketSort          | 1024 | Random             |     5,162.5 ns |     25.52 ns |      9.10 ns |   1.48 |    0.07 |    2 |         - |          NA |
| BucketSortInteger   | 1024 | Random             |     4,331.7 ns |     38.94 ns |     17.29 ns |   1.24 |    0.06 |    2 |         - |          NA |
| FlashSort           | 1024 | Random             |    15,413.9 ns |    150.82 ns |     78.88 ns |   4.42 |    0.21 |    5 |         - |          NA |
| RadixLSD4Sort       | 1024 | Random             |    10,636.2 ns |    323.43 ns |    143.60 ns |   3.05 |    0.15 |    4 |         - |          NA |
| RotateLSDRadixSort  | 1024 | Random             |   281,040.5 ns |  1,202.38 ns |    628.87 ns |  80.60 |    3.82 |    8 |         - |          NA |
| RadixLSD256Sort     | 1024 | Random             |     5,200.7 ns |     18.33 ns |      9.59 ns |   1.49 |    0.07 |    2 |         - |          NA |
| Radsort             | 1024 | Random             |    29,324.9 ns |    344.71 ns |    180.29 ns |   8.41 |    0.40 |    7 |         - |          NA |
| RadixLSD10Sort      | 1024 | Random             |    16,125.9 ns |    115.22 ns |     51.16 ns |   4.62 |    0.22 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | Random             |    22,720.1 ns |    659.29 ns |    344.82 ns |   6.52 |    0.32 |    6 |         - |          NA |
| RadixMSD10Sort      | 1024 | Random             |    22,436.9 ns |    229.64 ns |    101.96 ns |   6.43 |    0.31 |    6 |         - |          NA |
| AmericanFlagSort    | 1024 | Random             |    16,872.3 ns |    203.99 ns |    106.69 ns |   4.84 |    0.23 |    5 |         - |          NA |
| SpreadSort          | 1024 | Random             |     6,781.3 ns |    239.57 ns |    125.30 ns |   1.94 |    0.10 |    3 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **1024** | **SingleElementMoved** |     **4,898.0 ns** |     **29.31 ns** |     **13.02 ns** |   **1.67** |    **0.00** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | SingleElementMoved |     2,932.1 ns |      8.74 ns |      3.88 ns |   1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 1024 | SingleElementMoved |     3,846.7 ns |     10.72 ns |      5.60 ns |   1.31 |    0.00 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | SingleElementMoved |     2,463.9 ns |     11.59 ns |      4.13 ns |   0.84 |    0.00 |    1 |         - |          NA |
| BucketSort          | 1024 | SingleElementMoved |     5,086.8 ns |     16.55 ns |      7.35 ns |   1.73 |    0.00 |    3 |         - |          NA |
| BucketSortInteger   | 1024 | SingleElementMoved |     5,074.6 ns |    287.44 ns |    150.34 ns |   1.73 |    0.05 |    3 |         - |          NA |
| FlashSort           | 1024 | SingleElementMoved |    11,931.0 ns |    107.75 ns |     47.84 ns |   4.07 |    0.02 |    5 |         - |          NA |
| RadixLSD4Sort       | 1024 | SingleElementMoved |    11,963.3 ns |    346.72 ns |    181.34 ns |   4.08 |    0.06 |    5 |         - |          NA |
| RotateLSDRadixSort  | 1024 | SingleElementMoved |   176,130.1 ns |    495.09 ns |    219.82 ns |  60.07 |    0.10 |    8 |         - |          NA |
| RadixLSD256Sort     | 1024 | SingleElementMoved |     5,184.8 ns |    267.85 ns |    140.09 ns |   1.77 |    0.05 |    3 |         - |          NA |
| Radsort             | 1024 | SingleElementMoved |    19,400.2 ns |    152.07 ns |     67.52 ns |   6.62 |    0.02 |    7 |         - |          NA |
| RadixLSD10Sort      | 1024 | SingleElementMoved |    16,037.3 ns |    167.79 ns |     87.75 ns |   5.47 |    0.03 |    6 |         - |          NA |
| RadixMSD4Sort       | 1024 | SingleElementMoved |    24,134.6 ns |    230.00 ns |    120.29 ns |   8.23 |    0.04 |    7 |         - |          NA |
| RadixMSD10Sort      | 1024 | SingleElementMoved |    22,687.1 ns |    290.09 ns |    151.72 ns |   7.74 |    0.05 |    7 |         - |          NA |
| AmericanFlagSort    | 1024 | SingleElementMoved |     8,835.2 ns |    262.85 ns |    116.71 ns |   3.01 |    0.04 |    4 |         - |          NA |
| SpreadSort          | 1024 | SingleElementMoved |     4,615.3 ns |    214.27 ns |     95.14 ns |   1.57 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **1024** | **Sorted**             |     **7,299.5 ns** |    **303.79 ns** |    **158.89 ns** |   **2.54** |    **0.05** |    **5** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Sorted             |     2,869.5 ns |     19.89 ns |      8.83 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 1024 | Sorted             |     3,996.3 ns |    373.46 ns |    195.33 ns |   1.39 |    0.06 |    4 |         - |          NA |
| PigeonSortInteger   | 1024 | Sorted             |     2,149.4 ns |      6.34 ns |      2.81 ns |   0.75 |    0.00 |    2 |         - |          NA |
| BucketSort          | 1024 | Sorted             |     5,523.4 ns |    363.24 ns |    189.98 ns |   1.92 |    0.06 |    4 |         - |          NA |
| BucketSortInteger   | 1024 | Sorted             |     4,578.5 ns |    249.50 ns |    130.49 ns |   1.60 |    0.04 |    4 |         - |          NA |
| FlashSort           | 1024 | Sorted             |    10,988.1 ns |    289.85 ns |    151.60 ns |   3.83 |    0.05 |    6 |         - |          NA |
| RadixLSD4Sort       | 1024 | Sorted             |    13,093.8 ns |    444.12 ns |    232.28 ns |   4.56 |    0.08 |    6 |         - |          NA |
| RotateLSDRadixSort  | 1024 | Sorted             |   174,593.8 ns |  1,564.21 ns |    694.52 ns |  60.84 |    0.29 |    8 |         - |          NA |
| RadixLSD256Sort     | 1024 | Sorted             |     4,807.8 ns |     44.16 ns |     15.75 ns |   1.68 |    0.01 |    4 |         - |          NA |
| Radsort             | 1024 | Sorted             |    18,856.6 ns |    103.54 ns |     45.97 ns |   6.57 |    0.02 |    7 |         - |          NA |
| RadixLSD10Sort      | 1024 | Sorted             |    15,930.9 ns |     79.62 ns |     41.64 ns |   5.55 |    0.02 |    7 |         - |          NA |
| RadixMSD4Sort       | 1024 | Sorted             |    23,502.8 ns |    226.86 ns |    118.65 ns |   8.19 |    0.05 |    7 |         - |          NA |
| RadixMSD10Sort      | 1024 | Sorted             |    22,163.0 ns |    247.20 ns |    129.29 ns |   7.72 |    0.05 |    7 |         - |          NA |
| AmericanFlagSort    | 1024 | Sorted             |     7,349.7 ns |    290.41 ns |    151.89 ns |   2.56 |    0.05 |    5 |         - |          NA |
| SpreadSort          | 1024 | Sorted             |       490.3 ns |      3.54 ns |      1.57 ns |   0.17 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **1024** | **Reversed**           |     **4,297.2 ns** |     **18.03 ns** |      **6.43 ns** |   **1.48** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Reversed           |     2,903.6 ns |     27.26 ns |     12.10 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | Reversed           |     4,348.5 ns |    296.69 ns |    155.18 ns |   1.50 |    0.05 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | Reversed           |     2,169.9 ns |      6.18 ns |      2.75 ns |   0.75 |    0.00 |    1 |         - |          NA |
| BucketSort          | 1024 | Reversed           |     5,304.7 ns |     49.15 ns |     21.83 ns |   1.83 |    0.01 |    4 |         - |          NA |
| BucketSortInteger   | 1024 | Reversed           |     3,840.3 ns |      7.69 ns |      3.41 ns |   1.32 |    0.01 |    3 |         - |          NA |
| FlashSort           | 1024 | Reversed           |    10,545.1 ns |    171.72 ns |     76.24 ns |   3.63 |    0.03 |    7 |         - |          NA |
| RadixLSD4Sort       | 1024 | Reversed           |    11,947.6 ns |    313.69 ns |    164.07 ns |   4.11 |    0.06 |    7 |         - |          NA |
| RotateLSDRadixSort  | 1024 | Reversed           |   179,037.9 ns |  2,921.67 ns |  1,297.24 ns |  61.66 |    0.48 |    9 |         - |          NA |
| RadixLSD256Sort     | 1024 | Reversed           |     6,501.2 ns |    302.88 ns |    158.41 ns |   2.24 |    0.05 |    5 |         - |          NA |
| Radsort             | 1024 | Reversed           |    18,450.6 ns |     91.31 ns |     47.76 ns |   6.35 |    0.03 |    8 |         - |          NA |
| RadixLSD10Sort      | 1024 | Reversed           |    15,728.0 ns |     75.83 ns |     39.66 ns |   5.42 |    0.02 |    8 |         - |          NA |
| RadixMSD4Sort       | 1024 | Reversed           |    24,198.7 ns |    132.72 ns |     69.42 ns |   8.33 |    0.04 |    8 |         - |          NA |
| RadixMSD10Sort      | 1024 | Reversed           |    22,002.0 ns |    200.59 ns |    104.91 ns |   7.58 |    0.04 |    8 |         - |          NA |
| AmericanFlagSort    | 1024 | Reversed           |     8,333.8 ns |    222.40 ns |    116.32 ns |   2.87 |    0.04 |    6 |         - |          NA |
| SpreadSort          | 1024 | Reversed           |     3,720.5 ns |     25.08 ns |     11.13 ns |   1.28 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **1024** | **PipeOrgan**          |     **4,640.4 ns** |    **312.43 ns** |    **163.41 ns** |   **1.43** |    **0.11** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | PipeOrgan          |     3,256.1 ns |    436.50 ns |    228.30 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | PipeOrgan          |     4,150.8 ns |     70.79 ns |     37.02 ns |   1.28 |    0.08 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | PipeOrgan          |     2,473.5 ns |     45.25 ns |     16.14 ns |   0.76 |    0.05 |    1 |         - |          NA |
| BucketSort          | 1024 | PipeOrgan          |     5,214.0 ns |    222.00 ns |     98.57 ns |   1.61 |    0.11 |    3 |         - |          NA |
| BucketSortInteger   | 1024 | PipeOrgan          |     5,003.8 ns |      9.51 ns |      4.22 ns |   1.54 |    0.10 |    3 |         - |          NA |
| FlashSort           | 1024 | PipeOrgan          |    13,281.5 ns |    177.10 ns |     92.63 ns |   4.10 |    0.27 |    4 |         - |          NA |
| RadixLSD4Sort       | 1024 | PipeOrgan          |    10,573.5 ns |    184.35 ns |     96.42 ns |   3.26 |    0.22 |    4 |         - |          NA |
| RotateLSDRadixSort  | 1024 | PipeOrgan          |   176,149.1 ns |    507.46 ns |    225.32 ns |  54.33 |    3.58 |    6 |         - |          NA |
| RadixLSD256Sort     | 1024 | PipeOrgan          |     5,127.5 ns |     21.76 ns |     11.38 ns |   1.58 |    0.10 |    3 |         - |          NA |
| Radsort             | 1024 | PipeOrgan          |    19,090.5 ns |    112.40 ns |     58.79 ns |   5.89 |    0.39 |    5 |         - |          NA |
| RadixLSD10Sort      | 1024 | PipeOrgan          |    16,036.0 ns |     86.57 ns |     45.28 ns |   4.95 |    0.33 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | PipeOrgan          |    24,653.3 ns |    489.69 ns |    217.43 ns |   7.60 |    0.50 |    5 |         - |          NA |
| RadixMSD10Sort      | 1024 | PipeOrgan          |    22,604.5 ns |    144.55 ns |     75.60 ns |   6.97 |    0.46 |    5 |         - |          NA |
| AmericanFlagSort    | 1024 | PipeOrgan          |    11,733.3 ns |     90.51 ns |     40.19 ns |   3.62 |    0.24 |    4 |         - |          NA |
| SpreadSort          | 1024 | PipeOrgan          |     4,950.7 ns |     50.71 ns |     22.51 ns |   1.53 |    0.10 |    3 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **1024** | **ManyDuplicates**     |     **4,238.7 ns** |     **22.40 ns** |      **9.95 ns** |   **1.52** |    **0.01** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | ManyDuplicates     |     2,788.1 ns |     18.26 ns |      8.11 ns |   1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 1024 | ManyDuplicates     |     4,984.8 ns |    257.38 ns |    134.62 ns |   1.79 |    0.05 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | ManyDuplicates     |     2,389.1 ns |     42.96 ns |     19.08 ns |   0.86 |    0.01 |    1 |         - |          NA |
| BucketSort          | 1024 | ManyDuplicates     |     8,046.1 ns |    305.46 ns |    159.76 ns |   2.89 |    0.05 |    3 |         - |          NA |
| BucketSortInteger   | 1024 | ManyDuplicates     |     4,254.5 ns |    289.13 ns |    151.22 ns |   1.53 |    0.05 |    2 |         - |          NA |
| FlashSort           | 1024 | ManyDuplicates     |    13,601.3 ns |    219.79 ns |    114.96 ns |   4.88 |    0.04 |    4 |         - |          NA |
| RadixLSD4Sort       | 1024 | ManyDuplicates     |     6,813.3 ns |    273.36 ns |    142.97 ns |   2.44 |    0.05 |    3 |         - |          NA |
| RotateLSDRadixSort  | 1024 | ManyDuplicates     |   190,773.3 ns |  2,120.27 ns |    941.42 ns |  68.42 |    0.37 |    6 |         - |          NA |
| RadixLSD256Sort     | 1024 | ManyDuplicates     |     3,318.8 ns |     19.51 ns |      8.66 ns |   1.19 |    0.00 |    1 |         - |          NA |
| Radsort             | 1024 | ManyDuplicates     |    20,119.3 ns |     62.84 ns |     32.87 ns |   7.22 |    0.02 |    5 |         - |          NA |
| RadixLSD10Sort      | 1024 | ManyDuplicates     |     8,813.3 ns |    296.60 ns |    155.13 ns |   3.16 |    0.05 |    3 |         - |          NA |
| RadixMSD4Sort       | 1024 | ManyDuplicates     |     7,781.5 ns |    286.88 ns |    150.05 ns |   2.79 |    0.05 |    3 |         - |          NA |
| RadixMSD10Sort      | 1024 | ManyDuplicates     |     9,671.2 ns |    332.75 ns |    174.04 ns |   3.47 |    0.06 |    3 |         - |          NA |
| AmericanFlagSort    | 1024 | ManyDuplicates     |     7,250.4 ns |    215.21 ns |    112.56 ns |   2.60 |    0.04 |    3 |         - |          NA |
| SpreadSort          | 1024 | ManyDuplicates     |     5,313.3 ns |    105.43 ns |     55.14 ns |   1.91 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **4096** | **Random**             |    **19,367.6 ns** |    **179.78 ns** |     **79.83 ns** |   **1.45** |    **0.01** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Random             |    13,400.3 ns |     65.90 ns |     29.26 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | Random             |    15,811.4 ns |    153.63 ns |     68.21 ns |   1.18 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 4096 | Random             |     9,991.7 ns |    324.83 ns |    144.23 ns |   0.75 |    0.01 |    1 |         - |          NA |
| BucketSort          | 4096 | Random             |    21,219.2 ns |    132.99 ns |     69.55 ns |   1.58 |    0.01 |    2 |         - |          NA |
| BucketSortInteger   | 4096 | Random             |    17,275.8 ns |    167.99 ns |     87.86 ns |   1.29 |    0.01 |    2 |         - |          NA |
| FlashSort           | 4096 | Random             |    60,997.9 ns |    324.78 ns |    169.87 ns |   4.55 |    0.02 |    5 |         - |          NA |
| RadixLSD4Sort       | 4096 | Random             |    57,843.7 ns | 26,681.81 ns | 13,955.10 ns |   4.32 |    0.98 |    4 |         - |          NA |
| RotateLSDRadixSort  | 4096 | Random             | 1,406,897.1 ns |  5,178.12 ns |  2,708.26 ns | 104.99 |    0.29 |    7 |         - |          NA |
| RadixLSD256Sort     | 4096 | Random             |    19,415.8 ns |    292.95 ns |    153.22 ns |   1.45 |    0.01 |    2 |         - |          NA |
| Radsort             | 4096 | Random             |    68,593.5 ns |    605.27 ns |    316.57 ns |   5.12 |    0.02 |    5 |         - |          NA |
| RadixLSD10Sort      | 4096 | Random             |    64,429.5 ns |    197.38 ns |    103.24 ns |   4.81 |    0.01 |    5 |         - |          NA |
| RadixMSD4Sort       | 4096 | Random             |    97,817.5 ns |    161.93 ns |     57.75 ns |   7.30 |    0.02 |    6 |         - |          NA |
| RadixMSD10Sort      | 4096 | Random             |    89,323.7 ns |    571.65 ns |    253.81 ns |   6.67 |    0.02 |    6 |         - |          NA |
| AmericanFlagSort    | 4096 | Random             |    61,693.6 ns |    335.34 ns |    175.39 ns |   4.60 |    0.02 |    5 |         - |          NA |
| SpreadSort          | 4096 | Random             |    28,143.1 ns |    304.03 ns |    159.01 ns |   2.10 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **4096** | **SingleElementMoved** |    **18,560.8 ns** |    **221.82 ns** |    **116.02 ns** |   **1.55** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 4096 | SingleElementMoved |    11,939.1 ns |    252.75 ns |    132.19 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | SingleElementMoved |    15,320.0 ns |    124.85 ns |     65.30 ns |   1.28 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 4096 | SingleElementMoved |     9,773.7 ns |    204.72 ns |    107.07 ns |   0.82 |    0.01 |    1 |         - |          NA |
| BucketSort          | 4096 | SingleElementMoved |    20,864.4 ns |     58.00 ns |     20.68 ns |   1.75 |    0.02 |    3 |         - |          NA |
| BucketSortInteger   | 4096 | SingleElementMoved |    16,683.1 ns |    153.33 ns |     68.08 ns |   1.40 |    0.02 |    3 |         - |          NA |
| FlashSort           | 4096 | SingleElementMoved |    47,403.3 ns |    888.99 ns |    464.96 ns |   3.97 |    0.06 |    5 |         - |          NA |
| RadixLSD4Sort       | 4096 | SingleElementMoved |    66,391.4 ns |  1,001.78 ns |    523.95 ns |   5.56 |    0.07 |    6 |         - |          NA |
| RotateLSDRadixSort  | 4096 | SingleElementMoved |   857,801.1 ns |  4,323.57 ns |  2,261.31 ns |  71.86 |    0.78 |    8 |         - |          NA |
| RadixLSD256Sort     | 4096 | SingleElementMoved |    19,006.5 ns |    233.73 ns |    122.25 ns |   1.59 |    0.02 |    3 |         - |          NA |
| Radsort             | 4096 | SingleElementMoved |    44,498.3 ns |    198.35 ns |    103.74 ns |   3.73 |    0.04 |    5 |         - |          NA |
| RadixLSD10Sort      | 4096 | SingleElementMoved |    63,374.8 ns |    402.46 ns |    210.50 ns |   5.31 |    0.06 |    6 |         - |          NA |
| RadixMSD4Sort       | 4096 | SingleElementMoved |   103,786.9 ns |    331.94 ns |    147.38 ns |   8.69 |    0.09 |    7 |         - |          NA |
| RadixMSD10Sort      | 4096 | SingleElementMoved |    92,830.4 ns |    929.90 ns |    412.88 ns |   7.78 |    0.09 |    7 |         - |          NA |
| AmericanFlagSort    | 4096 | SingleElementMoved |    34,173.0 ns |    890.67 ns |    465.84 ns |   2.86 |    0.05 |    4 |         - |          NA |
| SpreadSort          | 4096 | SingleElementMoved |    18,246.2 ns |    218.62 ns |    114.34 ns |   1.53 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **4096** | **Sorted**             |    **18,844.7 ns** |    **873.26 ns** |    **456.73 ns** |   **1.63** |    **0.04** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Sorted             |    11,566.5 ns |    323.54 ns |    143.65 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 4096 | Sorted             |    15,160.8 ns |    139.68 ns |     73.06 ns |   1.31 |    0.02 |    4 |         - |          NA |
| PigeonSortInteger   | 4096 | Sorted             |     8,642.1 ns |    212.12 ns |    110.94 ns |   0.75 |    0.01 |    2 |         - |          NA |
| BucketSort          | 4096 | Sorted             |    19,944.8 ns |    159.51 ns |     70.82 ns |   1.72 |    0.02 |    4 |         - |          NA |
| BucketSortInteger   | 4096 | Sorted             |    15,408.0 ns |     44.94 ns |     19.95 ns |   1.33 |    0.02 |    4 |         - |          NA |
| FlashSort           | 4096 | Sorted             |    44,431.8 ns |    195.87 ns |    102.44 ns |   3.84 |    0.05 |    6 |         - |          NA |
| RadixLSD4Sort       | 4096 | Sorted             |    61,733.6 ns |  1,178.57 ns |    616.41 ns |   5.34 |    0.08 |    7 |         - |          NA |
| RotateLSDRadixSort  | 4096 | Sorted             |   851,749.2 ns |  2,277.99 ns |  1,191.43 ns |  73.65 |    0.86 |    9 |         - |          NA |
| RadixLSD256Sort     | 4096 | Sorted             |    17,880.2 ns |     52.44 ns |     27.43 ns |   1.55 |    0.02 |    4 |         - |          NA |
| Radsort             | 4096 | Sorted             |    41,703.2 ns |    248.19 ns |    129.81 ns |   3.61 |    0.04 |    6 |         - |          NA |
| RadixLSD10Sort      | 4096 | Sorted             |    62,866.6 ns |    135.23 ns |     70.73 ns |   5.44 |    0.06 |    7 |         - |          NA |
| RadixMSD4Sort       | 4096 | Sorted             |   101,356.5 ns |    590.38 ns |    262.13 ns |   8.76 |    0.10 |    8 |         - |          NA |
| RadixMSD10Sort      | 4096 | Sorted             |    87,489.9 ns |    363.22 ns |    161.27 ns |   7.57 |    0.09 |    8 |         - |          NA |
| AmericanFlagSort    | 4096 | Sorted             |    27,991.3 ns |    211.84 ns |     94.06 ns |   2.42 |    0.03 |    5 |         - |          NA |
| SpreadSort          | 4096 | Sorted             |     1,786.0 ns |     11.21 ns |      4.98 ns |   0.15 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **4096** | **Reversed**           |    **17,444.4 ns** |    **210.27 ns** |    **109.98 ns** |   **1.49** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Reversed           |    11,696.5 ns |    215.73 ns |    112.83 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | Reversed           |    16,354.8 ns |    196.90 ns |    102.98 ns |   1.40 |    0.02 |    3 |         - |          NA |
| PigeonSortInteger   | 4096 | Reversed           |     8,700.6 ns |    238.23 ns |    124.60 ns |   0.74 |    0.01 |    1 |         - |          NA |
| BucketSort          | 4096 | Reversed           |    21,335.0 ns |    442.48 ns |    231.42 ns |   1.82 |    0.03 |    3 |         - |          NA |
| BucketSortInteger   | 4096 | Reversed           |    15,997.1 ns |    193.34 ns |     85.84 ns |   1.37 |    0.01 |    3 |         - |          NA |
| FlashSort           | 4096 | Reversed           |    42,701.0 ns |  1,107.82 ns |    491.88 ns |   3.65 |    0.05 |    5 |         - |          NA |
| RadixLSD4Sort       | 4096 | Reversed           |    72,601.5 ns |    760.89 ns |    337.84 ns |   6.21 |    0.06 |    6 |         - |          NA |
| RotateLSDRadixSort  | 4096 | Reversed           |   868,844.9 ns |  2,618.23 ns |  1,369.39 ns |  74.29 |    0.69 |    8 |         - |          NA |
| RadixLSD256Sort     | 4096 | Reversed           |    17,972.5 ns |    238.44 ns |    124.71 ns |   1.54 |    0.02 |    3 |         - |          NA |
| Radsort             | 4096 | Reversed           |    41,286.2 ns |    323.84 ns |    143.79 ns |   3.53 |    0.03 |    5 |         - |          NA |
| RadixLSD10Sort      | 4096 | Reversed           |    62,495.2 ns |    151.57 ns |     67.30 ns |   5.34 |    0.05 |    6 |         - |          NA |
| RadixMSD4Sort       | 4096 | Reversed           |   101,576.5 ns |    415.47 ns |    184.47 ns |   8.69 |    0.08 |    7 |         - |          NA |
| RadixMSD10Sort      | 4096 | Reversed           |    87,600.1 ns |  1,396.63 ns |    620.11 ns |   7.49 |    0.08 |    7 |         - |          NA |
| AmericanFlagSort    | 4096 | Reversed           |    31,160.6 ns |    394.46 ns |    206.31 ns |   2.66 |    0.03 |    4 |         - |          NA |
| SpreadSort          | 4096 | Reversed           |    14,940.0 ns |    185.69 ns |     97.12 ns |   1.28 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **4096** | **PipeOrgan**          |    **17,876.4 ns** |    **160.42 ns** |     **83.90 ns** |   **1.45** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 4096 | PipeOrgan          |    12,359.0 ns |    174.91 ns |     77.66 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | PipeOrgan          |    15,385.0 ns |    177.85 ns |     93.02 ns |   1.24 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 4096 | PipeOrgan          |     9,818.4 ns |    324.98 ns |    169.97 ns |   0.79 |    0.01 |    1 |         - |          NA |
| BucketSort          | 4096 | PipeOrgan          |    20,713.3 ns |    312.25 ns |    163.31 ns |   1.68 |    0.02 |    3 |         - |          NA |
| BucketSortInteger   | 4096 | PipeOrgan          |    16,840.0 ns |     84.43 ns |     44.16 ns |   1.36 |    0.01 |    3 |         - |          NA |
| FlashSort           | 4096 | PipeOrgan          |    51,682.9 ns |    736.98 ns |    327.22 ns |   4.18 |    0.04 |    4 |         - |          NA |
| RadixLSD4Sort       | 4096 | PipeOrgan          |    56,093.7 ns |    319.59 ns |    141.90 ns |   4.54 |    0.03 |    4 |         - |          NA |
| RotateLSDRadixSort  | 4096 | PipeOrgan          |   856,464.9 ns |  1,957.87 ns |    869.31 ns |  69.30 |    0.42 |    6 |         - |          NA |
| RadixLSD256Sort     | 4096 | PipeOrgan          |    19,557.2 ns |    271.00 ns |    141.74 ns |   1.58 |    0.01 |    3 |         - |          NA |
| Radsort             | 4096 | PipeOrgan          |    42,975.8 ns |    272.45 ns |    142.50 ns |   3.48 |    0.02 |    4 |         - |          NA |
| RadixLSD10Sort      | 4096 | PipeOrgan          |    63,839.7 ns |    660.65 ns |    345.53 ns |   5.17 |    0.04 |    4 |         - |          NA |
| RadixMSD4Sort       | 4096 | PipeOrgan          |   103,214.7 ns |    464.10 ns |    206.07 ns |   8.35 |    0.05 |    5 |         - |          NA |
| RadixMSD10Sort      | 4096 | PipeOrgan          |    89,347.1 ns |    230.81 ns |    120.72 ns |   7.23 |    0.04 |    5 |         - |          NA |
| AmericanFlagSort    | 4096 | PipeOrgan          |    48,080.4 ns |    377.69 ns |    197.54 ns |   3.89 |    0.03 |    4 |         - |          NA |
| SpreadSort          | 4096 | PipeOrgan          |    20,630.2 ns |    368.87 ns |    192.92 ns |   1.67 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **4096** | **ManyDuplicates**     |    **16,980.8 ns** |    **183.67 ns** |     **96.06 ns** |   **1.52** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 4096 | ManyDuplicates     |    11,139.9 ns |    321.62 ns |    168.21 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | ManyDuplicates     |    19,651.7 ns |    180.40 ns |     80.10 ns |   1.76 |    0.03 |    3 |         - |          NA |
| PigeonSortInteger   | 4096 | ManyDuplicates     |     8,687.1 ns |    236.47 ns |    123.68 ns |   0.78 |    0.02 |    1 |         - |          NA |
| BucketSort          | 4096 | ManyDuplicates     |    30,277.5 ns |    184.32 ns |     96.40 ns |   2.72 |    0.04 |    4 |         - |          NA |
| BucketSortInteger   | 4096 | ManyDuplicates     |    16,335.4 ns |    172.47 ns |     90.21 ns |   1.47 |    0.02 |    3 |         - |          NA |
| FlashSort           | 4096 | ManyDuplicates     |    53,484.2 ns |    357.76 ns |    158.85 ns |   4.80 |    0.07 |    5 |         - |          NA |
| RadixLSD4Sort       | 4096 | ManyDuplicates     |    26,885.8 ns |    110.27 ns |     39.32 ns |   2.41 |    0.03 |    4 |         - |          NA |
| RotateLSDRadixSort  | 4096 | ManyDuplicates     |   770,308.1 ns |  2,710.89 ns |  1,417.85 ns |  69.16 |    0.99 |    6 |         - |          NA |
| RadixLSD256Sort     | 4096 | ManyDuplicates     |    12,586.0 ns |    264.22 ns |    117.31 ns |   1.13 |    0.02 |    2 |         - |          NA |
| Radsort             | 4096 | ManyDuplicates     |    42,181.8 ns |    827.65 ns |    367.48 ns |   3.79 |    0.06 |    4 |         - |          NA |
| RadixLSD10Sort      | 4096 | ManyDuplicates     |    34,412.7 ns |    150.92 ns |     67.01 ns |   3.09 |    0.04 |    4 |         - |          NA |
| RadixMSD4Sort       | 4096 | ManyDuplicates     |    28,081.2 ns |    165.76 ns |     86.69 ns |   2.52 |    0.04 |    4 |         - |          NA |
| RadixMSD10Sort      | 4096 | ManyDuplicates     |    36,465.7 ns |    129.47 ns |     67.71 ns |   3.27 |    0.05 |    4 |         - |          NA |
| AmericanFlagSort    | 4096 | ManyDuplicates     |    26,851.7 ns |    151.81 ns |     79.40 ns |   2.41 |    0.04 |    4 |         - |          NA |
| SpreadSort          | 4096 | ManyDuplicates     |    21,097.1 ns |    268.67 ns |    140.52 ns |   1.89 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **8192** | **Random**             |    **40,256.8 ns** |    **260.71 ns** |    **115.76 ns** |   **1.45** |    **0.01** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Random             |    27,805.6 ns |    460.43 ns |    240.82 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | Random             |    30,842.3 ns |    242.20 ns |    126.68 ns |   1.11 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | Random             |    19,772.4 ns |    138.71 ns |     61.59 ns |   0.71 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | Random             |    43,929.1 ns |    133.36 ns |     47.56 ns |   1.58 |    0.01 |    2 |         - |          NA |
| BucketSortInteger   | 8192 | Random             |    35,867.2 ns |    385.02 ns |    201.37 ns |   1.29 |    0.01 |    2 |         - |          NA |
| FlashSort           | 8192 | Random             |   133,295.4 ns |    338.09 ns |    176.83 ns |   4.79 |    0.04 |    3 |         - |          NA |
| RadixLSD4Sort       | 8192 | Random             |   114,552.8 ns |  2,151.78 ns |  1,125.42 ns |   4.12 |    0.05 |    3 |         - |          NA |
| RotateLSDRadixSort  | 8192 | Random             | 3,393,006.9 ns |  5,501.25 ns |  2,877.26 ns | 122.03 |    1.00 |    5 |         - |          NA |
| RadixLSD256Sort     | 8192 | Random             |    39,066.4 ns |    214.38 ns |     95.19 ns |   1.41 |    0.01 |    2 |         - |          NA |
| Radsort             | 8192 | Random             |    94,544.5 ns |    593.78 ns |    263.64 ns |   3.40 |    0.03 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | Random             |   128,249.6 ns |    352.39 ns |    156.46 ns |   4.61 |    0.04 |    3 |         - |          NA |
| RadixMSD4Sort       | 8192 | Random             |   214,092.7 ns |    911.83 ns |    325.17 ns |   7.70 |    0.06 |    4 |         - |          NA |
| RadixMSD10Sort      | 8192 | Random             |   182,637.7 ns |    288.43 ns |    128.06 ns |   6.57 |    0.05 |    4 |         - |          NA |
| AmericanFlagSort    | 8192 | Random             |   121,800.7 ns |    382.46 ns |    136.39 ns |   4.38 |    0.04 |    3 |         - |          NA |
| SpreadSort          | 8192 | Random             |    98,838.8 ns |    462.62 ns |    241.96 ns |   3.55 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **8192** | **SingleElementMoved** |    **37,057.9 ns** |    **537.19 ns** |    **238.52 ns** |   **1.54** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | SingleElementMoved |    24,080.2 ns |    439.06 ns |    229.64 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | SingleElementMoved |    30,230.5 ns |    345.57 ns |    153.43 ns |   1.26 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 8192 | SingleElementMoved |    19,344.9 ns |    102.39 ns |     36.51 ns |   0.80 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | SingleElementMoved |    43,534.6 ns |    506.35 ns |    224.82 ns |   1.81 |    0.02 |    3 |         - |          NA |
| BucketSortInteger   | 8192 | SingleElementMoved |    33,568.1 ns |  1,287.14 ns |    571.50 ns |   1.39 |    0.03 |    3 |         - |          NA |
| FlashSort           | 8192 | SingleElementMoved |    94,530.5 ns |  1,798.59 ns |    940.70 ns |   3.93 |    0.05 |    5 |         - |          NA |
| RadixLSD4Sort       | 8192 | SingleElementMoved |   163,782.3 ns |  1,163.51 ns |    516.60 ns |   6.80 |    0.06 |    7 |         - |          NA |
| RotateLSDRadixSort  | 8192 | SingleElementMoved | 2,174,109.6 ns |  3,633.02 ns |  1,613.08 ns |  90.29 |    0.81 |    8 |         - |          NA |
| RadixLSD256Sort     | 8192 | SingleElementMoved |    39,601.6 ns |    489.82 ns |    256.19 ns |   1.64 |    0.02 |    3 |         - |          NA |
| Radsort             | 8192 | SingleElementMoved |    61,362.4 ns |    288.89 ns |    151.10 ns |   2.55 |    0.02 |    4 |         - |          NA |
| RadixLSD10Sort      | 8192 | SingleElementMoved |   127,223.2 ns |  1,164.29 ns |    516.95 ns |   5.28 |    0.05 |    6 |         - |          NA |
| RadixMSD4Sort       | 8192 | SingleElementMoved |   215,301.9 ns |  2,489.36 ns |  1,105.29 ns |   8.94 |    0.09 |    7 |         - |          NA |
| RadixMSD10Sort      | 8192 | SingleElementMoved |   180,124.1 ns |  1,498.92 ns |    783.97 ns |   7.48 |    0.07 |    7 |         - |          NA |
| AmericanFlagSort    | 8192 | SingleElementMoved |    67,061.1 ns |    688.76 ns |    360.24 ns |   2.79 |    0.03 |    4 |         - |          NA |
| SpreadSort          | 8192 | SingleElementMoved |    39,984.0 ns |    219.97 ns |     97.67 ns |   1.66 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **8192** | **Sorted**             |    **36,509.1 ns** |    **571.37 ns** |    **298.84 ns** |   **1.57** |    **0.01** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Sorted             |    23,233.7 ns |    233.82 ns |    103.82 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 8192 | Sorted             |    30,479.4 ns |  1,222.82 ns |    639.56 ns |   1.31 |    0.03 |    4 |         - |          NA |
| PigeonSortInteger   | 8192 | Sorted             |    17,224.4 ns |    167.33 ns |     74.30 ns |   0.74 |    0.00 |    2 |         - |          NA |
| BucketSort          | 8192 | Sorted             |    41,726.1 ns |    271.65 ns |    142.08 ns |   1.80 |    0.01 |    4 |         - |          NA |
| BucketSortInteger   | 8192 | Sorted             |    31,008.7 ns |  1,127.48 ns |    500.61 ns |   1.33 |    0.02 |    4 |         - |          NA |
| FlashSort           | 8192 | Sorted             |    89,796.3 ns |    534.15 ns |    237.16 ns |   3.86 |    0.02 |    6 |         - |          NA |
| RadixLSD4Sort       | 8192 | Sorted             |   158,086.4 ns |    939.39 ns |    491.32 ns |   6.80 |    0.03 |    8 |         - |          NA |
| RotateLSDRadixSort  | 8192 | Sorted             | 2,164,089.6 ns |  2,901.78 ns |  1,288.41 ns |  93.15 |    0.39 |   10 |         - |          NA |
| RadixLSD256Sort     | 8192 | Sorted             |    37,413.4 ns |    748.58 ns |    391.52 ns |   1.61 |    0.02 |    4 |         - |          NA |
| Radsort             | 8192 | Sorted             |    57,065.4 ns |    353.22 ns |    184.74 ns |   2.46 |    0.01 |    5 |         - |          NA |
| RadixLSD10Sort      | 8192 | Sorted             |   125,554.6 ns |    578.91 ns |    257.04 ns |   5.40 |    0.02 |    7 |         - |          NA |
| RadixMSD4Sort       | 8192 | Sorted             |   215,617.1 ns |  3,931.10 ns |  1,745.43 ns |   9.28 |    0.08 |    9 |         - |          NA |
| RadixMSD10Sort      | 8192 | Sorted             |   179,012.4 ns |    996.09 ns |    520.97 ns |   7.70 |    0.04 |    8 |         - |          NA |
| AmericanFlagSort    | 8192 | Sorted             |    55,431.4 ns |    135.86 ns |     71.06 ns |   2.39 |    0.01 |    5 |         - |          NA |
| SpreadSort          | 8192 | Sorted             |     3,499.8 ns |     17.83 ns |      7.92 ns |   0.15 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **8192** | **Reversed**           |    **34,511.0 ns** |    **297.18 ns** |    **155.43 ns** |   **1.46** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Reversed           |    23,680.7 ns |    599.93 ns |    266.37 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | Reversed           |    32,405.5 ns |    260.20 ns |    136.09 ns |   1.37 |    0.02 |    3 |         - |          NA |
| PigeonSortInteger   | 8192 | Reversed           |    17,400.8 ns |    408.30 ns |    213.55 ns |   0.73 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | Reversed           |    42,006.4 ns |    310.13 ns |    162.21 ns |   1.77 |    0.02 |    3 |         - |          NA |
| BucketSortInteger   | 8192 | Reversed           |    31,674.6 ns |    128.29 ns |     56.96 ns |   1.34 |    0.01 |    3 |         - |          NA |
| FlashSort           | 8192 | Reversed           |    83,662.5 ns |  1,182.69 ns |    525.12 ns |   3.53 |    0.04 |    5 |         - |          NA |
| RadixLSD4Sort       | 8192 | Reversed           |   160,592.6 ns |  2,055.52 ns |  1,075.08 ns |   6.78 |    0.08 |    7 |         - |          NA |
| RotateLSDRadixSort  | 8192 | Reversed           | 2,204,631.0 ns |  6,613.51 ns |  2,936.44 ns |  93.11 |    0.98 |    9 |         - |          NA |
| RadixLSD256Sort     | 8192 | Reversed           |    38,216.5 ns |     47.82 ns |     21.23 ns |   1.61 |    0.02 |    3 |         - |          NA |
| Radsort             | 8192 | Reversed           |    56,740.0 ns |    590.80 ns |    309.00 ns |   2.40 |    0.03 |    4 |         - |          NA |
| RadixLSD10Sort      | 8192 | Reversed           |   126,254.6 ns |    683.92 ns |    303.66 ns |   5.33 |    0.06 |    6 |         - |          NA |
| RadixMSD4Sort       | 8192 | Reversed           |   218,389.3 ns |  1,554.94 ns |    554.51 ns |   9.22 |    0.10 |    8 |         - |          NA |
| RadixMSD10Sort      | 8192 | Reversed           |   176,900.1 ns |  1,621.43 ns |    848.04 ns |   7.47 |    0.08 |    7 |         - |          NA |
| AmericanFlagSort    | 8192 | Reversed           |    61,383.5 ns |    572.72 ns |    299.54 ns |   2.59 |    0.03 |    4 |         - |          NA |
| SpreadSort          | 8192 | Reversed           |    55,638.8 ns |  2,102.99 ns |  1,099.90 ns |   2.35 |    0.05 |    4 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **8192** | **PipeOrgan**          |    **35,713.5 ns** |    **356.11 ns** |    **186.25 ns** |   **1.43** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | PipeOrgan          |    25,005.2 ns |    243.13 ns |    107.95 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | PipeOrgan          |    30,282.0 ns |    537.39 ns |    281.06 ns |   1.21 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 8192 | PipeOrgan          |    19,640.2 ns |    296.04 ns |    131.44 ns |   0.79 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | PipeOrgan          |    40,934.8 ns |    539.13 ns |    239.38 ns |   1.64 |    0.01 |    3 |         - |          NA |
| BucketSortInteger   | 8192 | PipeOrgan          |    33,919.4 ns |    199.78 ns |     88.70 ns |   1.36 |    0.01 |    3 |         - |          NA |
| FlashSort           | 8192 | PipeOrgan          |   113,822.0 ns |  2,856.73 ns |  1,268.41 ns |   4.55 |    0.05 |    5 |         - |          NA |
| RadixLSD4Sort       | 8192 | PipeOrgan          |   132,497.3 ns |  1,343.07 ns |    702.45 ns |   5.30 |    0.03 |    5 |         - |          NA |
| RotateLSDRadixSort  | 8192 | PipeOrgan          | 2,174,452.1 ns |  2,026.65 ns |    899.85 ns |  86.96 |    0.35 |    8 |         - |          NA |
| RadixLSD256Sort     | 8192 | PipeOrgan          |    40,093.5 ns |    220.36 ns |     97.84 ns |   1.60 |    0.01 |    3 |         - |          NA |
| Radsort             | 8192 | PipeOrgan          |    58,805.7 ns |    252.10 ns |    131.85 ns |   2.35 |    0.01 |    4 |         - |          NA |
| RadixLSD10Sort      | 8192 | PipeOrgan          |   128,510.9 ns |    770.30 ns |    402.88 ns |   5.14 |    0.03 |    5 |         - |          NA |
| RadixMSD4Sort       | 8192 | PipeOrgan          |   218,777.1 ns |    471.72 ns |    209.45 ns |   8.75 |    0.04 |    7 |         - |          NA |
| RadixMSD10Sort      | 8192 | PipeOrgan          |   177,686.0 ns |  1,513.46 ns |    791.57 ns |   7.11 |    0.04 |    6 |         - |          NA |
| AmericanFlagSort    | 8192 | PipeOrgan          |    97,555.5 ns |    992.88 ns |    519.30 ns |   3.90 |    0.03 |    5 |         - |          NA |
| SpreadSort          | 8192 | PipeOrgan          |    67,177.7 ns |    351.75 ns |    183.97 ns |   2.69 |    0.01 |    4 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **8192** | **ManyDuplicates**     |    **33,798.6 ns** |    **177.92 ns** |     **93.05 ns** |   **1.53** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | ManyDuplicates     |    22,062.4 ns |    316.76 ns |    140.64 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | ManyDuplicates     |    54,381.1 ns |    436.21 ns |    193.68 ns |   2.46 |    0.02 |    5 |         - |          NA |
| PigeonSortInteger   | 8192 | ManyDuplicates     |    17,269.8 ns |    459.49 ns |    204.02 ns |   0.78 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | ManyDuplicates     |    60,532.8 ns |    504.44 ns |    223.97 ns |   2.74 |    0.02 |    5 |         - |          NA |
| BucketSortInteger   | 8192 | ManyDuplicates     |    31,928.1 ns |    308.17 ns |    136.83 ns |   1.45 |    0.01 |    3 |         - |          NA |
| FlashSort           | 8192 | ManyDuplicates     |   106,735.2 ns |    707.47 ns |    314.12 ns |   4.84 |    0.03 |    6 |         - |          NA |
| RadixLSD4Sort       | 8192 | ManyDuplicates     |    54,155.9 ns |    369.84 ns |    193.43 ns |   2.45 |    0.02 |    5 |         - |          NA |
| RotateLSDRadixSort  | 8192 | ManyDuplicates     | 1,627,805.9 ns |  2,672.11 ns |  1,186.43 ns |  73.78 |    0.44 |    7 |         - |          NA |
| RadixLSD256Sort     | 8192 | ManyDuplicates     |    25,115.3 ns |    488.08 ns |    255.28 ns |   1.14 |    0.01 |    2 |         - |          NA |
| Radsort             | 8192 | ManyDuplicates     |    57,573.8 ns | 17,774.77 ns |  7,892.11 ns |   2.61 |    0.33 |    5 |         - |          NA |
| RadixLSD10Sort      | 8192 | ManyDuplicates     |    69,005.3 ns |    636.41 ns |    332.86 ns |   3.13 |    0.02 |    5 |         - |          NA |
| RadixMSD4Sort       | 8192 | ManyDuplicates     |    57,418.1 ns |  2,080.22 ns |    741.83 ns |   2.60 |    0.03 |    5 |         - |          NA |
| RadixMSD10Sort      | 8192 | ManyDuplicates     |    73,143.0 ns |    491.29 ns |    175.20 ns |   3.32 |    0.02 |    5 |         - |          NA |
| AmericanFlagSort    | 8192 | ManyDuplicates     |    52,491.6 ns |    403.65 ns |    143.94 ns |   2.38 |    0.02 |    5 |         - |          NA |
| SpreadSort          | 8192 | ManyDuplicates     |    41,918.9 ns |    517.16 ns |    229.62 ns |   1.90 |    0.01 |    4 |         - |          NA |

### ExchangeBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.46 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8
UnrollFactor=1  WarmupCount=2
```

| Method             | Size | Pattern            | Mean         | Error       | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------- |----- |------------------- |-------------:|------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **BubbleSort**         | **256**  | **Random**             |  **41,878.7 ns** |   **233.86 ns** |   **122.31 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Random             |  27,108.3 ns |   120.91 ns |    63.24 ns |  0.65 |    0.00 |    2 |         - |          NA |
| OddEvenSort        | 256  | Random             |  42,051.0 ns |   224.17 ns |   117.25 ns |  1.00 |    0.00 |    3 |         - |          NA |
| CombSort           | 256  | Random             |   8,077.2 ns |   308.61 ns |   161.41 ns |  0.19 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | Random             |  22,942.2 ns |   120.99 ns |    53.72 ns |  0.55 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **256**  | **SingleElementMoved** |   **4,507.1 ns** |    **28.83 ns** |    **10.28 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | SingleElementMoved |     303.5 ns |    16.51 ns |     8.64 ns |  0.07 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 256  | SingleElementMoved |   8,530.1 ns |   314.76 ns |   164.63 ns |  1.89 |    0.03 |    4 |         - |          NA |
| CombSort           | 256  | SingleElementMoved |   2,664.0 ns |    48.51 ns |    17.30 ns |  0.59 |    0.00 |    2 |         - |          NA |
| CircleSort         | 256  | SingleElementMoved |  10,134.0 ns |   177.90 ns |    78.99 ns |  2.25 |    0.02 |    4 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **256**  | **Sorted**             |     **237.0 ns** |    **17.26 ns** |     **7.66 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Sorted             |     155.8 ns |     7.56 ns |     3.95 ns |  0.66 |    0.03 |    1 |         - |          NA |
| OddEvenSort        | 256  | Sorted             |     204.8 ns |     6.37 ns |     3.33 ns |  0.86 |    0.03 |    2 |         - |          NA |
| CombSort           | 256  | Sorted             |   2,242.8 ns |     3.94 ns |     1.75 ns |  9.47 |    0.30 |    4 |         - |          NA |
| CircleSort         | 256  | Sorted             |   1,652.7 ns |    38.13 ns |    13.60 ns |  6.98 |    0.23 |    3 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **256**  | **Reversed**           |  **22,767.2 ns** |   **116.90 ns** |    **61.14 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Reversed           |  21,597.0 ns |   425.71 ns |   222.65 ns |  0.95 |    0.01 |    3 |         - |          NA |
| OddEvenSort        | 256  | Reversed           |  19,665.3 ns |   219.72 ns |    97.56 ns |  0.86 |    0.00 |    3 |         - |          NA |
| CombSort           | 256  | Reversed           |   2,521.5 ns |   335.32 ns |   175.38 ns |  0.11 |    0.01 |    1 |         - |          NA |
| CircleSort         | 256  | Reversed           |   3,402.7 ns |   282.66 ns |   147.84 ns |  0.15 |    0.01 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **256**  | **PipeOrgan**          |  **26,839.3 ns** |   **122.07 ns** |    **63.84 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 256  | PipeOrgan          |  13,842.0 ns |   185.77 ns |    82.48 ns |  0.52 |    0.00 |    2 |         - |          NA |
| OddEvenSort        | 256  | PipeOrgan          |  21,688.3 ns |   279.58 ns |   124.13 ns |  0.81 |    0.00 |    3 |         - |          NA |
| CombSort           | 256  | PipeOrgan          |   2,461.1 ns |    42.45 ns |    18.85 ns |  0.09 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | PipeOrgan          |  15,140.8 ns |   159.08 ns |    83.20 ns |  0.56 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **256**  | **ManyDuplicates**     |  **41,089.7 ns** |   **312.39 ns** |   **163.38 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 256  | ManyDuplicates     |  26,590.8 ns |   258.60 ns |   135.25 ns |  0.65 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 256  | ManyDuplicates     |  43,663.8 ns |   226.61 ns |   100.61 ns |  1.06 |    0.00 |    4 |         - |          NA |
| CombSort           | 256  | ManyDuplicates     |   6,332.9 ns |   224.75 ns |   117.55 ns |  0.15 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | ManyDuplicates     |  19,876.7 ns |   248.16 ns |   129.79 ns |  0.48 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **1024** | **Random**             | **532,162.0 ns** | **2,457.97 ns** | **1,091.35 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Random             | 296,554.8 ns |   537.63 ns |   238.71 ns |  0.56 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | Random             | 480,750.1 ns | 2,275.02 ns | 1,189.88 ns |  0.90 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | Random             |  42,529.9 ns |   169.45 ns |    88.63 ns |  0.08 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | Random             | 127,734.5 ns |   895.11 ns |   468.16 ns |  0.24 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **1024** | **SingleElementMoved** |  **66,586.8 ns** |   **139.97 ns** |    **62.15 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | SingleElementMoved |   1,107.5 ns |     7.80 ns |     4.08 ns |  0.02 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 1024 | SingleElementMoved | 122,006.7 ns |   976.81 ns |   348.34 ns |  1.83 |    0.01 |    4 |         - |          NA |
| CombSort           | 1024 | SingleElementMoved |  12,656.7 ns |   290.28 ns |   151.82 ns |  0.19 |    0.00 |    2 |         - |          NA |
| CircleSort         | 1024 | SingleElementMoved |  57,761.7 ns |   396.41 ns |   176.01 ns |  0.87 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **1024** | **Sorted**             |     **857.5 ns** |    **32.01 ns** |    **14.21 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Sorted             |     569.7 ns |     5.36 ns |     2.81 ns |  0.66 |    0.01 |    1 |         - |          NA |
| OddEvenSort        | 1024 | Sorted             |     719.8 ns |     5.78 ns |     2.06 ns |  0.84 |    0.01 |    2 |         - |          NA |
| CombSort           | 1024 | Sorted             |  11,505.2 ns |   243.09 ns |   127.14 ns | 13.42 |    0.25 |    4 |         - |          NA |
| CircleSort         | 1024 | Sorted             |   7,345.8 ns |   256.96 ns |   134.39 ns |  8.57 |    0.20 |    3 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **1024** | **Reversed**           | **341,573.4 ns** | **5,828.27 ns** | **3,048.30 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Reversed           | 339,226.9 ns | 2,476.40 ns | 1,295.20 ns |  0.99 |    0.01 |    2 |         - |          NA |
| OddEvenSort        | 1024 | Reversed           | 318,557.9 ns | 4,262.07 ns | 2,229.14 ns |  0.93 |    0.01 |    2 |         - |          NA |
| CombSort           | 1024 | Reversed           |  12,790.2 ns |   303.82 ns |   158.91 ns |  0.04 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | Reversed           |  14,685.1 ns |   378.27 ns |   167.96 ns |  0.04 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **1024** | **PipeOrgan**          | **394,183.3 ns** | **1,499.11 ns** |   **665.61 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | PipeOrgan          | 221,448.7 ns | 1,375.90 ns |   719.62 ns |  0.56 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | PipeOrgan          | 348,659.9 ns | 1,549.27 ns |   687.88 ns |  0.88 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | PipeOrgan          |  12,809.5 ns |   155.33 ns |    81.24 ns |  0.03 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | PipeOrgan          |  81,256.9 ns |   348.77 ns |   154.86 ns |  0.21 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **1024** | **ManyDuplicates**     | **528,572.7 ns** | **3,706.49 ns** | **1,938.57 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | ManyDuplicates     | 291,935.3 ns | 4,022.50 ns | 1,786.01 ns |  0.55 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | ManyDuplicates     | 495,533.6 ns | 1,087.04 ns |   482.65 ns |  0.94 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | ManyDuplicates     |  28,003.3 ns |   625.55 ns |   327.18 ns |  0.05 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | ManyDuplicates     |  98,965.0 ns |   686.53 ns |   304.82 ns |  0.19 |    0.00 |    2 |         - |          NA |

### HeapBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.46 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8
UnrollFactor=1  WarmupCount=2
```

| Method           | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **HeapSort**         | **256**  | **Random**             |     **6,854.5 ns** |    **107.75 ns** |     **56.36 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | Random             |     6,915.4 ns |    391.65 ns |    204.84 ns |  1.01 |    0.03 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | Random             |    10,627.5 ns |    252.97 ns |    112.32 ns |  1.55 |    0.02 |    2 |         - |          NA |
| BottomupHeapSort | 256  | Random             |     7,005.1 ns |    140.44 ns |     62.36 ns |  1.02 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 256  | Random             |    12,459.6 ns |    238.47 ns |    105.88 ns |  1.82 |    0.02 |    2 |         - |          NA |
| SmoothSort       | 256  | Random             |    13,456.1 ns |    192.87 ns |     85.64 ns |  1.96 |    0.02 |    2 |         - |          NA |
| PoplarHeapSort   | 256  | Random             |    13,442.6 ns |     94.54 ns |     41.98 ns |  1.96 |    0.02 |    2 |         - |          NA |
| TournamentSort   | 256  | Random             |    13,351.6 ns |    327.01 ns |    171.03 ns |  1.95 |    0.03 |    2 |         - |          NA |
| BinomialHeapSort | 256  | Random             |    27,209.2 ns |    694.65 ns |    363.32 ns |  3.97 |    0.06 |    3 |         - |          NA |
| PairingHeapSort  | 256  | Random             |    16,148.2 ns |    281.39 ns |    147.17 ns |  2.36 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **SingleElementMoved** |     **5,770.9 ns** |    **148.65 ns** |     **77.75 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 256  | SingleElementMoved |     5,478.7 ns |    178.39 ns |     93.30 ns |  0.95 |    0.02 |    3 |         - |          NA |
| TernaryHeapSort  | 256  | SingleElementMoved |     4,080.6 ns |    625.23 ns |    327.01 ns |  0.71 |    0.05 |    3 |         - |          NA |
| BottomupHeapSort | 256  | SingleElementMoved |     5,720.6 ns |    170.11 ns |     88.97 ns |  0.99 |    0.02 |    3 |         - |          NA |
| WeakHeapSort     | 256  | SingleElementMoved |     5,445.9 ns |    219.80 ns |    114.96 ns |  0.94 |    0.02 |    3 |         - |          NA |
| SmoothSort       | 256  | SingleElementMoved |     1,274.6 ns |     78.47 ns |     34.84 ns |  0.22 |    0.01 |    1 |         - |          NA |
| PoplarHeapSort   | 256  | SingleElementMoved |     3,110.3 ns |    287.87 ns |    150.56 ns |  0.54 |    0.03 |    2 |         - |          NA |
| TournamentSort   | 256  | SingleElementMoved |     4,750.7 ns |    284.36 ns |    148.73 ns |  0.82 |    0.03 |    3 |         - |          NA |
| BinomialHeapSort | 256  | SingleElementMoved |     6,096.9 ns |    320.82 ns |    167.80 ns |  1.06 |    0.03 |    3 |         - |          NA |
| PairingHeapSort  | 256  | SingleElementMoved |     4,645.1 ns |    307.18 ns |    160.66 ns |  0.81 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **Sorted**             |     **5,626.5 ns** |    **205.86 ns** |    **107.67 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 256  | Sorted             |     5,209.3 ns |    121.44 ns |     53.92 ns |  0.93 |    0.02 |    4 |         - |          NA |
| TernaryHeapSort  | 256  | Sorted             |     3,414.5 ns |    362.95 ns |    189.83 ns |  0.61 |    0.03 |    3 |         - |          NA |
| BottomupHeapSort | 256  | Sorted             |     5,521.0 ns |    242.94 ns |    127.06 ns |  0.98 |    0.03 |    4 |         - |          NA |
| WeakHeapSort     | 256  | Sorted             |     4,917.1 ns |     41.63 ns |     14.85 ns |  0.87 |    0.02 |    4 |         - |          NA |
| SmoothSort       | 256  | Sorted             |       912.2 ns |      3.91 ns |      1.39 ns |  0.16 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 256  | Sorted             |     2,576.8 ns |     14.88 ns |      6.61 ns |  0.46 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 256  | Sorted             |     4,270.5 ns |    316.01 ns |    165.28 ns |  0.76 |    0.03 |    4 |         - |          NA |
| BinomialHeapSort | 256  | Sorted             |     5,153.5 ns |     61.03 ns |     21.76 ns |  0.92 |    0.02 |    4 |         - |          NA |
| PairingHeapSort  | 256  | Sorted             |     4,144.8 ns |     30.01 ns |     13.33 ns |  0.74 |    0.01 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **Reversed**           |     **5,174.6 ns** |    **271.39 ns** |    **141.94 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 256  | Reversed           |     5,666.9 ns |    258.87 ns |    135.40 ns |  1.10 |    0.04 |    3 |         - |          NA |
| TernaryHeapSort  | 256  | Reversed           |     2,749.7 ns |    212.18 ns |     94.21 ns |  0.53 |    0.02 |    2 |         - |          NA |
| BottomupHeapSort | 256  | Reversed           |     5,598.1 ns |     39.20 ns |     13.98 ns |  1.08 |    0.03 |    3 |         - |          NA |
| WeakHeapSort     | 256  | Reversed           |     5,651.5 ns |    149.82 ns |     78.36 ns |  1.09 |    0.03 |    3 |         - |          NA |
| SmoothSort       | 256  | Reversed           |     3,986.9 ns |    381.43 ns |    199.50 ns |  0.77 |    0.04 |    3 |         - |          NA |
| PoplarHeapSort   | 256  | Reversed           |     4,670.7 ns |    206.64 ns |    108.08 ns |  0.90 |    0.03 |    3 |         - |          NA |
| TournamentSort   | 256  | Reversed           |     4,381.4 ns |    198.27 ns |    103.70 ns |  0.85 |    0.03 |    3 |         - |          NA |
| BinomialHeapSort | 256  | Reversed           |     5,254.9 ns |    301.98 ns |    157.94 ns |  1.02 |    0.04 |    3 |         - |          NA |
| PairingHeapSort  | 256  | Reversed           |     2,003.8 ns |      7.59 ns |      2.71 ns |  0.39 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **PipeOrgan**          |     **5,405.6 ns** |    **236.07 ns** |    **123.47 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 256  | PipeOrgan          |     5,216.2 ns |    231.71 ns |    121.19 ns |  0.97 |    0.03 |    2 |         - |          NA |
| TernaryHeapSort  | 256  | PipeOrgan          |     3,092.5 ns |    217.65 ns |    113.83 ns |  0.57 |    0.02 |    1 |         - |          NA |
| BottomupHeapSort | 256  | PipeOrgan          |     5,438.1 ns |     11.24 ns |      4.01 ns |  1.01 |    0.02 |    2 |         - |          NA |
| WeakHeapSort     | 256  | PipeOrgan          |     5,353.0 ns |    292.36 ns |    152.91 ns |  0.99 |    0.03 |    2 |         - |          NA |
| SmoothSort       | 256  | PipeOrgan          |     3,968.7 ns |    129.20 ns |     57.36 ns |  0.73 |    0.02 |    2 |         - |          NA |
| PoplarHeapSort   | 256  | PipeOrgan          |     4,595.0 ns |    138.37 ns |     72.37 ns |  0.85 |    0.02 |    2 |         - |          NA |
| TournamentSort   | 256  | PipeOrgan          |     3,141.5 ns |    187.29 ns |     97.96 ns |  0.58 |    0.02 |    1 |         - |          NA |
| BinomialHeapSort | 256  | PipeOrgan          |     5,938.6 ns |    162.13 ns |     84.80 ns |  1.10 |    0.03 |    2 |         - |          NA |
| PairingHeapSort  | 256  | PipeOrgan          |     5,302.4 ns |    140.06 ns |     62.19 ns |  0.98 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **ManyDuplicates**     |     **6,752.1 ns** |    **349.01 ns** |    **182.54 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | ManyDuplicates     |     6,665.8 ns |    120.47 ns |     53.49 ns |  0.99 |    0.03 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | ManyDuplicates     |    10,033.2 ns |    222.78 ns |    116.52 ns |  1.49 |    0.04 |    2 |         - |          NA |
| BottomupHeapSort | 256  | ManyDuplicates     |     7,003.3 ns |     38.37 ns |     13.68 ns |  1.04 |    0.03 |    1 |         - |          NA |
| WeakHeapSort     | 256  | ManyDuplicates     |    10,697.0 ns |    155.75 ns |     69.16 ns |  1.59 |    0.04 |    2 |         - |          NA |
| SmoothSort       | 256  | ManyDuplicates     |    12,426.6 ns |    181.66 ns |     64.78 ns |  1.84 |    0.05 |    2 |         - |          NA |
| PoplarHeapSort   | 256  | ManyDuplicates     |    12,925.5 ns |    166.50 ns |     73.93 ns |  1.92 |    0.05 |    2 |         - |          NA |
| TournamentSort   | 256  | ManyDuplicates     |    13,323.5 ns |    345.23 ns |    153.29 ns |  1.97 |    0.05 |    2 |         - |          NA |
| BinomialHeapSort | 256  | ManyDuplicates     |    25,192.3 ns |    654.01 ns |    342.06 ns |  3.73 |    0.11 |    3 |         - |          NA |
| PairingHeapSort  | 256  | ManyDuplicates     |    15,404.2 ns |    165.60 ns |     86.61 ns |  2.28 |    0.06 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Random**             |    **30,530.9 ns** |    **300.19 ns** |    **157.00 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Random             |    30,919.7 ns |     27.20 ns |      9.70 ns |  1.01 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | Random             |    52,901.2 ns |    269.32 ns |    140.86 ns |  1.73 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | Random             |    32,279.6 ns |    111.57 ns |     49.54 ns |  1.06 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | Random             |    64,248.7 ns |    352.86 ns |    184.55 ns |  2.10 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 1024 | Random             |    70,870.8 ns |    315.15 ns |    164.83 ns |  2.32 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 1024 | Random             |    66,397.1 ns |     83.70 ns |     37.16 ns |  2.17 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 1024 | Random             |    65,350.4 ns |    266.17 ns |    139.21 ns |  2.14 |    0.01 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | Random             |   156,315.9 ns |    863.54 ns |    383.42 ns |  5.12 |    0.03 |    4 |         - |          NA |
| PairingHeapSort  | 1024 | Random             |    80,630.6 ns |    268.35 ns |     95.69 ns |  2.64 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **SingleElementMoved** |    **25,447.7 ns** |    **140.90 ns** |     **73.69 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 1024 | SingleElementMoved |    24,981.8 ns |    316.52 ns |    165.55 ns |  0.98 |    0.01 |    3 |         - |          NA |
| TernaryHeapSort  | 1024 | SingleElementMoved |    19,490.2 ns |    314.67 ns |    139.71 ns |  0.77 |    0.01 |    3 |         - |          NA |
| BottomupHeapSort | 1024 | SingleElementMoved |    24,907.9 ns |    181.16 ns |     80.43 ns |  0.98 |    0.00 |    3 |         - |          NA |
| WeakHeapSort     | 1024 | SingleElementMoved |    25,715.1 ns |    218.63 ns |     97.07 ns |  1.01 |    0.00 |    3 |         - |          NA |
| SmoothSort       | 1024 | SingleElementMoved |     4,949.2 ns |    285.43 ns |    149.28 ns |  0.19 |    0.01 |    1 |         - |          NA |
| PoplarHeapSort   | 1024 | SingleElementMoved |    12,977.8 ns |    208.79 ns |     92.70 ns |  0.51 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 1024 | SingleElementMoved |    21,579.9 ns |    254.06 ns |    112.80 ns |  0.85 |    0.00 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | SingleElementMoved |    26,228.4 ns |    481.38 ns |    213.74 ns |  1.03 |    0.01 |    3 |         - |          NA |
| PairingHeapSort  | 1024 | SingleElementMoved |    17,225.3 ns |    182.32 ns |     95.36 ns |  0.68 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Sorted**             |    **24,758.8 ns** |    **168.10 ns** |     **87.92 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Sorted             |    23,207.0 ns |    143.19 ns |     74.89 ns |  0.94 |    0.00 |    3 |         - |          NA |
| TernaryHeapSort  | 1024 | Sorted             |    15,605.4 ns |    438.07 ns |    229.12 ns |  0.63 |    0.01 |    3 |         - |          NA |
| BottomupHeapSort | 1024 | Sorted             |    24,348.9 ns |    277.83 ns |    145.31 ns |  0.98 |    0.01 |    3 |         - |          NA |
| WeakHeapSort     | 1024 | Sorted             |    24,581.4 ns |    288.61 ns |    128.14 ns |  0.99 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 1024 | Sorted             |     3,682.5 ns |     20.78 ns |      9.22 ns |  0.15 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 1024 | Sorted             |    11,535.0 ns |    247.82 ns |    110.03 ns |  0.47 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 1024 | Sorted             |    19,965.2 ns |    602.98 ns |    315.37 ns |  0.81 |    0.01 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | Sorted             |    22,924.0 ns |    161.13 ns |     84.27 ns |  0.93 |    0.00 |    3 |         - |          NA |
| PairingHeapSort  | 1024 | Sorted             |    16,977.3 ns |    219.98 ns |     97.67 ns |  0.69 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Reversed**           |    **22,784.5 ns** |    **102.67 ns** |     **36.61 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Reversed           |    24,930.4 ns |    202.39 ns |    105.86 ns |  1.09 |    0.00 |    3 |         - |          NA |
| TernaryHeapSort  | 1024 | Reversed           |    13,195.1 ns |    604.52 ns |    316.17 ns |  0.58 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | Reversed           |    25,053.7 ns |    325.51 ns |    170.25 ns |  1.10 |    0.01 |    3 |         - |          NA |
| WeakHeapSort     | 1024 | Reversed           |    27,615.6 ns |    139.72 ns |     62.04 ns |  1.21 |    0.00 |    3 |         - |          NA |
| SmoothSort       | 1024 | Reversed           |    19,223.1 ns |    561.29 ns |    249.22 ns |  0.84 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 1024 | Reversed           |    21,119.4 ns |    735.35 ns |    384.60 ns |  0.93 |    0.02 |    3 |         - |          NA |
| TournamentSort   | 1024 | Reversed           |    21,053.1 ns |    419.55 ns |    186.28 ns |  0.92 |    0.01 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | Reversed           |    22,958.5 ns |    356.82 ns |    158.43 ns |  1.01 |    0.01 |    3 |         - |          NA |
| PairingHeapSort  | 1024 | Reversed           |     8,063.8 ns |    366.45 ns |    191.66 ns |  0.35 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **PipeOrgan**          |    **24,395.3 ns** |    **279.41 ns** |    **146.14 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | PipeOrgan          |    23,114.4 ns |    277.07 ns |    144.91 ns |  0.95 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | PipeOrgan          |    14,601.0 ns |    522.91 ns |    232.18 ns |  0.60 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | PipeOrgan          |    24,377.5 ns |    281.74 ns |    147.35 ns |  1.00 |    0.01 |    2 |         - |          NA |
| WeakHeapSort     | 1024 | PipeOrgan          |    26,804.5 ns |    428.52 ns |    224.12 ns |  1.10 |    0.01 |    2 |         - |          NA |
| SmoothSort       | 1024 | PipeOrgan          |    20,456.4 ns |    496.21 ns |    259.53 ns |  0.84 |    0.01 |    2 |         - |          NA |
| PoplarHeapSort   | 1024 | PipeOrgan          |    21,898.8 ns |    594.20 ns |    310.78 ns |  0.90 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 1024 | PipeOrgan          |    14,908.8 ns |    380.17 ns |    168.80 ns |  0.61 |    0.01 |    1 |         - |          NA |
| BinomialHeapSort | 1024 | PipeOrgan          |    25,916.8 ns |    241.35 ns |    126.23 ns |  1.06 |    0.01 |    2 |         - |          NA |
| PairingHeapSort  | 1024 | PipeOrgan          |    22,523.5 ns |    367.09 ns |    192.00 ns |  0.92 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **ManyDuplicates**     |    **29,298.6 ns** |    **279.07 ns** |    **145.96 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | ManyDuplicates     |    29,198.2 ns |    645.69 ns |    286.69 ns |  1.00 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | ManyDuplicates     |    48,451.7 ns |    299.66 ns |    156.73 ns |  1.65 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | ManyDuplicates     |    29,854.5 ns |    146.60 ns |     65.09 ns |  1.02 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | ManyDuplicates     |    49,380.9 ns |    414.85 ns |    216.97 ns |  1.69 |    0.01 |    2 |         - |          NA |
| SmoothSort       | 1024 | ManyDuplicates     |    63,730.6 ns |    447.80 ns |    198.83 ns |  2.18 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 1024 | ManyDuplicates     |    62,153.9 ns |    280.39 ns |    146.65 ns |  2.12 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 1024 | ManyDuplicates     |    65,271.2 ns |    265.63 ns |    138.93 ns |  2.23 |    0.01 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | ManyDuplicates     |   122,215.7 ns |    892.17 ns |    466.62 ns |  4.17 |    0.02 |    4 |         - |          NA |
| PairingHeapSort  | 1024 | ManyDuplicates     |    69,317.1 ns |    228.75 ns |    101.57 ns |  2.37 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Random**             |   **130,994.1 ns** |  **1,564.96 ns** |    **694.85 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Random             |   132,193.9 ns |  1,838.89 ns |    961.77 ns |  1.01 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 4096 | Random             |   250,956.7 ns |    963.36 ns |    503.85 ns |  1.92 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 4096 | Random             |   143,426.2 ns |    415.05 ns |    217.08 ns |  1.09 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | Random             |   309,619.1 ns |    521.31 ns |    185.91 ns |  2.36 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 4096 | Random             |   342,345.5 ns |    809.03 ns |    359.22 ns |  2.61 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 4096 | Random             |   309,651.5 ns |    886.15 ns |    463.47 ns |  2.36 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 4096 | Random             |   313,258.5 ns |  1,117.39 ns |    584.42 ns |  2.39 |    0.01 |    3 |         - |          NA |
| BinomialHeapSort | 4096 | Random             |   810,621.0 ns |  2,034.49 ns |    903.32 ns |  6.19 |    0.03 |    4 |         - |          NA |
| PairingHeapSort  | 4096 | Random             |   392,039.0 ns |  2,091.74 ns |    928.74 ns |  2.99 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **SingleElementMoved** |   **102,402.6 ns** |    **795.98 ns** |    **353.42 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 4096 | SingleElementMoved |   105,145.4 ns |    670.23 ns |    350.54 ns |  1.03 |    0.00 |    4 |         - |          NA |
| TernaryHeapSort  | 4096 | SingleElementMoved |   120,117.6 ns |  4,652.70 ns |  2,065.83 ns |  1.17 |    0.02 |    4 |         - |          NA |
| BottomupHeapSort | 4096 | SingleElementMoved |   110,702.9 ns |    523.15 ns |    232.28 ns |  1.08 |    0.00 |    4 |         - |          NA |
| WeakHeapSort     | 4096 | SingleElementMoved |   123,981.0 ns |    375.18 ns |    166.58 ns |  1.21 |    0.00 |    4 |         - |          NA |
| SmoothSort       | 4096 | SingleElementMoved |    18,852.1 ns |    371.83 ns |    165.09 ns |  0.18 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 4096 | SingleElementMoved |    56,174.0 ns |    274.67 ns |    121.96 ns |  0.55 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 4096 | SingleElementMoved |   124,144.5 ns | 13,741.27 ns |  7,186.95 ns |  1.21 |    0.07 |    4 |         - |          NA |
| BinomialHeapSort | 4096 | SingleElementMoved |   114,234.3 ns |    579.38 ns |    257.25 ns |  1.12 |    0.00 |    4 |         - |          NA |
| PairingHeapSort  | 4096 | SingleElementMoved |    68,910.7 ns |    241.83 ns |    107.37 ns |  0.67 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Sorted**             |   **100,093.5 ns** |  **1,497.16 ns** |    **664.75 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Sorted             |    97,979.7 ns |    454.55 ns |    201.82 ns |  0.98 |    0.01 |    4 |         - |          NA |
| TernaryHeapSort  | 4096 | Sorted             |    76,398.7 ns |  1,555.30 ns |    813.45 ns |  0.76 |    0.01 |    3 |         - |          NA |
| BottomupHeapSort | 4096 | Sorted             |   108,682.2 ns |    346.37 ns |    153.79 ns |  1.09 |    0.01 |    4 |         - |          NA |
| WeakHeapSort     | 4096 | Sorted             |   120,618.6 ns |  1,086.92 ns |    482.60 ns |  1.21 |    0.01 |    4 |         - |          NA |
| SmoothSort       | 4096 | Sorted             |    15,062.7 ns |    118.95 ns |     62.21 ns |  0.15 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 4096 | Sorted             |    51,351.8 ns |    169.60 ns |     88.70 ns |  0.51 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 4096 | Sorted             |   102,981.0 ns |  7,986.76 ns |  4,177.23 ns |  1.03 |    0.04 |    4 |         - |          NA |
| BinomialHeapSort | 4096 | Sorted             |   102,192.2 ns |  2,125.07 ns |    943.55 ns |  1.02 |    0.01 |    4 |         - |          NA |
| PairingHeapSort  | 4096 | Sorted             |    68,599.9 ns |    420.35 ns |    186.64 ns |  0.69 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Reversed**           |    **97,362.0 ns** |    **728.63 ns** |    **259.84 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Reversed           |   102,483.2 ns |  3,837.38 ns |  2,007.02 ns |  1.05 |    0.02 |    3 |         - |          NA |
| TernaryHeapSort  | 4096 | Reversed           |    66,009.1 ns |  1,473.48 ns |    654.23 ns |  0.68 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 4096 | Reversed           |   111,148.9 ns |    223.58 ns |    116.94 ns |  1.14 |    0.00 |    3 |         - |          NA |
| WeakHeapSort     | 4096 | Reversed           |   130,793.1 ns |    916.34 ns |    479.26 ns |  1.34 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 4096 | Reversed           |   102,081.2 ns |  3,294.28 ns |  1,462.68 ns |  1.05 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 4096 | Reversed           |    96,998.4 ns |  1,378.21 ns |    611.94 ns |  1.00 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 4096 | Reversed           |   179,913.9 ns | 24,660.57 ns | 12,897.95 ns |  1.85 |    0.13 |    4 |         - |          NA |
| BinomialHeapSort | 4096 | Reversed           |   100,835.3 ns |    825.21 ns |    366.40 ns |  1.04 |    0.00 |    3 |         - |          NA |
| PairingHeapSort  | 4096 | Reversed           |    31,958.2 ns |    140.28 ns |     50.03 ns |  0.33 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **PipeOrgan**          |   **101,652.8 ns** |    **355.86 ns** |    **158.00 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 4096 | PipeOrgan          |   100,484.2 ns |  1,047.12 ns |    547.66 ns |  0.99 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 4096 | PipeOrgan          |    71,104.5 ns |  2,200.88 ns |  1,151.10 ns |  0.70 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 4096 | PipeOrgan          |   110,102.6 ns |    353.69 ns |    184.99 ns |  1.08 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | PipeOrgan          |   131,184.7 ns |    823.41 ns |    430.66 ns |  1.29 |    0.00 |    1 |         - |          NA |
| SmoothSort       | 4096 | PipeOrgan          |   208,541.6 ns |    754.83 ns |    335.15 ns |  2.05 |    0.00 |    2 |         - |          NA |
| PoplarHeapSort   | 4096 | PipeOrgan          |   109,437.6 ns |  7,074.94 ns |  3,141.32 ns |  1.08 |    0.03 |    1 |         - |          NA |
| TournamentSort   | 4096 | PipeOrgan          |    80,921.0 ns |  4,312.90 ns |  1,914.96 ns |  0.80 |    0.02 |    1 |         - |          NA |
| BinomialHeapSort | 4096 | PipeOrgan          |   113,129.3 ns |  1,004.15 ns |    445.85 ns |  1.11 |    0.00 |    1 |         - |          NA |
| PairingHeapSort  | 4096 | PipeOrgan          |    94,285.9 ns |    621.17 ns |    275.80 ns |  0.93 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **ManyDuplicates**     |   **121,080.6 ns** |    **548.43 ns** |    **286.84 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 4096 | ManyDuplicates     |   121,896.8 ns |    529.95 ns |    235.30 ns |  1.01 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 4096 | ManyDuplicates     |   228,505.1 ns |    763.24 ns |    338.88 ns |  1.89 |    0.00 |    2 |         - |          NA |
| BottomupHeapSort | 4096 | ManyDuplicates     |   127,636.0 ns |  1,047.01 ns |    547.61 ns |  1.05 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | ManyDuplicates     |   221,582.5 ns |    765.12 ns |    339.72 ns |  1.83 |    0.00 |    2 |         - |          NA |
| SmoothSort       | 4096 | ManyDuplicates     |   297,013.0 ns |    688.67 ns |    305.78 ns |  2.45 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 4096 | ManyDuplicates     |   291,171.1 ns |  1,045.00 ns |    546.55 ns |  2.40 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 4096 | ManyDuplicates     |   310,585.1 ns |    547.48 ns |    243.08 ns |  2.57 |    0.01 |    3 |         - |          NA |
| BinomialHeapSort | 4096 | ManyDuplicates     |   546,652.1 ns |  2,194.22 ns |    974.25 ns |  4.51 |    0.01 |    4 |         - |          NA |
| PairingHeapSort  | 4096 | ManyDuplicates     |   304,039.8 ns |    796.90 ns |    353.83 ns |  2.51 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Random**             |   **268,874.0 ns** |    **843.95 ns** |    **441.40 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Random             |   272,571.0 ns |    543.19 ns |    241.18 ns |  1.01 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | Random             |   541,820.0 ns |  1,316.80 ns |    688.71 ns |  2.02 |    0.00 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | Random             |   308,699.6 ns |    879.56 ns |    460.03 ns |  1.15 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | Random             |   673,360.4 ns |  1,484.08 ns |    658.94 ns |  2.50 |    0.00 |    3 |         - |          NA |
| SmoothSort       | 8192 | Random             |   741,210.4 ns |  1,771.55 ns |    926.55 ns |  2.76 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 8192 | Random             |   663,265.7 ns |    550.98 ns |    288.18 ns |  2.47 |    0.00 |    3 |         - |          NA |
| TournamentSort   | 8192 | Random             |   682,169.5 ns |  2,104.78 ns |    750.58 ns |  2.54 |    0.00 |    3 |         - |          NA |
| BinomialHeapSort | 8192 | Random             | 1,785,278.1 ns |  5,313.80 ns |  2,779.22 ns |  6.64 |    0.01 |    4 |         - |          NA |
| PairingHeapSort  | 8192 | Random             |   868,668.9 ns |  5,190.60 ns |  2,304.66 ns |  3.23 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **SingleElementMoved** |   **216,049.7 ns** |    **898.93 ns** |    **470.16 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 8192 | SingleElementMoved |   215,804.5 ns |  1,230.61 ns |    643.63 ns |  1.00 |    0.00 |    3 |         - |          NA |
| TernaryHeapSort  | 8192 | SingleElementMoved |   347,358.7 ns |  6,265.19 ns |  3,276.82 ns |  1.61 |    0.01 |    4 |         - |          NA |
| BottomupHeapSort | 8192 | SingleElementMoved |   244,035.2 ns |  2,050.12 ns |    910.27 ns |  1.13 |    0.00 |    3 |         - |          NA |
| WeakHeapSort     | 8192 | SingleElementMoved |   270,345.9 ns |  1,053.27 ns |    467.66 ns |  1.25 |    0.00 |    3 |         - |          NA |
| SmoothSort       | 8192 | SingleElementMoved |    37,623.1 ns |    554.14 ns |    289.83 ns |  0.17 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 8192 | SingleElementMoved |   117,478.6 ns |    539.28 ns |    239.45 ns |  0.54 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 8192 | SingleElementMoved |   381,276.4 ns |    897.21 ns |    398.37 ns |  1.76 |    0.00 |    4 |         - |          NA |
| BinomialHeapSort | 8192 | SingleElementMoved |   239,039.3 ns |  1,021.70 ns |    534.37 ns |  1.11 |    0.00 |    3 |         - |          NA |
| PairingHeapSort  | 8192 | SingleElementMoved |   140,621.5 ns |  1,220.12 ns |    541.74 ns |  0.65 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Sorted**             |   **214,605.3 ns** |  **1,661.36 ns** |    **868.92 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Sorted             |   199,984.7 ns |  1,094.91 ns |    486.15 ns |  0.93 |    0.00 |    4 |         - |          NA |
| TernaryHeapSort  | 8192 | Sorted             |   335,251.5 ns |  4,219.62 ns |  2,206.94 ns |  1.56 |    0.01 |    5 |         - |          NA |
| BottomupHeapSort | 8192 | Sorted             |   239,527.1 ns |    681.74 ns |    302.70 ns |  1.12 |    0.00 |    4 |         - |          NA |
| WeakHeapSort     | 8192 | Sorted             |   265,448.2 ns |  1,366.83 ns |    714.88 ns |  1.24 |    0.01 |    4 |         - |          NA |
| SmoothSort       | 8192 | Sorted             |    29,879.2 ns |    318.00 ns |    166.32 ns |  0.14 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 8192 | Sorted             |   108,127.7 ns |    422.26 ns |    187.49 ns |  0.50 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 8192 | Sorted             |   391,481.6 ns |  3,318.42 ns |  1,473.40 ns |  1.82 |    0.01 |    5 |         - |          NA |
| BinomialHeapSort | 8192 | Sorted             |   213,518.0 ns |  1,112.91 ns |    494.14 ns |  0.99 |    0.00 |    4 |         - |          NA |
| PairingHeapSort  | 8192 | Sorted             |   138,198.0 ns |  2,219.94 ns |    985.67 ns |  0.64 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Reversed**           |   **198,449.2 ns** |  **1,286.82 ns** |    **673.03 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Reversed           |   217,835.9 ns |  1,281.74 ns |    569.10 ns |  1.10 |    0.00 |    2 |         - |          NA |
| TernaryHeapSort  | 8192 | Reversed           |   327,295.5 ns |  6,260.26 ns |  2,779.59 ns |  1.65 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | Reversed           |   244,207.8 ns |    560.55 ns |    248.89 ns |  1.23 |    0.00 |    2 |         - |          NA |
| WeakHeapSort     | 8192 | Reversed           |   284,134.3 ns |    861.30 ns |    382.42 ns |  1.43 |    0.00 |    2 |         - |          NA |
| SmoothSort       | 8192 | Reversed           |   439,855.3 ns |  3,659.04 ns |  1,913.75 ns |  2.22 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 8192 | Reversed           |   322,084.6 ns |  1,788.65 ns |    935.50 ns |  1.62 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 8192 | Reversed           |   497,891.7 ns | 30,053.93 ns | 15,718.79 ns |  2.51 |    0.08 |    3 |         - |          NA |
| BinomialHeapSort | 8192 | Reversed           |   212,217.1 ns |  1,553.62 ns |    689.82 ns |  1.07 |    0.00 |    2 |         - |          NA |
| PairingHeapSort  | 8192 | Reversed           |    63,580.3 ns |  1,657.74 ns |    736.05 ns |  0.32 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **PipeOrgan**          |   **211,555.8 ns** |  **1,304.36 ns** |    **682.20 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | PipeOrgan          |   204,638.4 ns |  1,722.48 ns |    900.89 ns |  0.97 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | PipeOrgan          |   374,189.2 ns |  1,507.03 ns |    788.20 ns |  1.77 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | PipeOrgan          |   242,300.1 ns |    935.33 ns |    415.29 ns |  1.15 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | PipeOrgan          |   287,694.5 ns |  1,143.99 ns |    598.33 ns |  1.36 |    0.00 |    1 |         - |          NA |
| SmoothSort       | 8192 | PipeOrgan          |   550,346.2 ns |  4,336.56 ns |  2,268.10 ns |  2.60 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 8192 | PipeOrgan          |   452,585.0 ns |  1,817.31 ns |    806.90 ns |  2.14 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 8192 | PipeOrgan          |   412,855.4 ns | 23,453.50 ns | 12,266.63 ns |  1.95 |    0.05 |    2 |         - |          NA |
| BinomialHeapSort | 8192 | PipeOrgan          |   236,000.0 ns |  2,087.93 ns |  1,092.03 ns |  1.12 |    0.01 |    1 |         - |          NA |
| PairingHeapSort  | 8192 | PipeOrgan          |   188,805.4 ns |  1,767.27 ns |    784.68 ns |  0.89 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **ManyDuplicates**     |   **243,566.7 ns** |  **1,196.81 ns** |    **625.96 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | ManyDuplicates     |   248,205.5 ns |    657.15 ns |    234.35 ns |  1.02 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | ManyDuplicates     |   496,800.5 ns |    948.63 ns |    421.20 ns |  2.04 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | ManyDuplicates     |   274,450.5 ns |  1,925.25 ns |    854.82 ns |  1.13 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | ManyDuplicates     |   462,333.5 ns |  3,112.80 ns |  1,628.05 ns |  1.90 |    0.01 |    2 |         - |          NA |
| SmoothSort       | 8192 | ManyDuplicates     |   624,110.7 ns |  1,518.77 ns |    794.35 ns |  2.56 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 8192 | ManyDuplicates     |   616,218.2 ns |  2,973.19 ns |  1,555.03 ns |  2.53 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 8192 | ManyDuplicates     |   659,881.0 ns |  1,749.82 ns |    776.93 ns |  2.71 |    0.01 |    3 |         - |          NA |
| BinomialHeapSort | 8192 | ManyDuplicates     | 1,136,598.8 ns |  8,693.14 ns |  3,859.81 ns |  4.67 |    0.02 |    4 |         - |          NA |
| PairingHeapSort  | 8192 | ManyDuplicates     |   644,766.8 ns |  7,303.84 ns |  3,820.05 ns |  2.65 |    0.02 |    3 |         - |          NA |

### InsertionBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.46 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8
UnrollFactor=1  WarmupCount=2
```

| Method                 | Size | Pattern            | Mean         | Error       | StdDev      | Ratio  | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------- |----- |------------------- |-------------:|------------:|------------:|-------:|--------:|-----:|----------:|------------:|
| **InsertionSort**          | **256**  | **Random**             |   **7,470.3 ns** |   **172.41 ns** |    **90.18 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Random             |   7,796.1 ns |   603.59 ns |   215.25 ns |   1.04 |    0.03 |    1 |         - |          NA |
| BinaryInsertSort       | 256  | Random             |  11,497.0 ns |   266.08 ns |   118.14 ns |   1.54 |    0.02 |    2 |         - |          NA |
| GnomeSort              | 256  | Random             |  10,943.4 ns |   275.24 ns |   143.96 ns |   1.47 |    0.02 |    2 |         - |          NA |
| LibrarySort            | 256  | Random             |  22,886.8 ns |   484.98 ns |   253.66 ns |   3.06 |    0.05 |    3 |         - |          NA |
| MergeInsertionSort     | 256  | Random             |  20,497.6 ns |   135.42 ns |    60.13 ns |   2.74 |    0.03 |    3 |         - |          NA |
| ShellSortKnuth1973     | 256  | Random             |   7,947.1 ns |    86.70 ns |    38.49 ns |   1.06 |    0.01 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Random             |   8,220.2 ns |   177.86 ns |    93.02 ns |   1.10 |    0.02 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | Random             |   8,767.7 ns |   290.39 ns |   151.88 ns |   1.17 |    0.02 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | Random             |   8,453.2 ns |   354.06 ns |   185.18 ns |   1.13 |    0.03 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Random             |   8,830.5 ns |   227.22 ns |   118.84 ns |   1.18 |    0.02 |    1 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **256**  | **SingleElementMoved** |     **202.5 ns** |     **7.51 ns** |     **3.93 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 256  | SingleElementMoved |     260.9 ns |     5.32 ns |     2.78 ns |   1.29 |    0.03 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | SingleElementMoved |   1,022.5 ns |   175.34 ns |    91.71 ns |   5.05 |    0.44 |    4 |         - |          NA |
| GnomeSort              | 256  | SingleElementMoved |     323.9 ns |    10.18 ns |     5.32 ns |   1.60 |    0.04 |    3 |         - |          NA |
| LibrarySort            | 256  | SingleElementMoved |  21,728.8 ns |   706.74 ns |   369.64 ns | 107.34 |    2.60 |    6 |         - |          NA |
| MergeInsertionSort     | 256  | SingleElementMoved |  10,228.3 ns |   237.97 ns |   124.46 ns |  50.53 |    1.08 |    5 |         - |          NA |
| ShellSortKnuth1973     | 256  | SingleElementMoved |     807.7 ns |    38.37 ns |    17.04 ns |   3.99 |    0.11 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 256  | SingleElementMoved |     827.0 ns |    14.32 ns |     7.49 ns |   4.09 |    0.08 |    4 |         - |          NA |
| ShellSortTokuda1992    | 256  | SingleElementMoved |     973.6 ns |    21.08 ns |     9.36 ns |   4.81 |    0.10 |    4 |         - |          NA |
| ShellSortCiura2001     | 256  | SingleElementMoved |     893.5 ns |   140.49 ns |    62.38 ns |   4.41 |    0.30 |    4 |         - |          NA |
| ShellSortLee2021       | 256  | SingleElementMoved |     964.5 ns |    28.98 ns |    15.16 ns |   4.76 |    0.11 |    4 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **256**  | **Sorted**             |     **169.0 ns** |    **16.55 ns** |     **7.35 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Sorted             |     210.3 ns |     4.11 ns |     2.15 ns |   1.25 |    0.06 |    2 |         - |          NA |
| BinaryInsertSort       | 256  | Sorted             |     918.2 ns |    26.77 ns |     9.54 ns |   5.44 |    0.25 |    3 |         - |          NA |
| GnomeSort              | 256  | Sorted             |     237.1 ns |    18.00 ns |     7.99 ns |   1.41 |    0.08 |    2 |         - |          NA |
| LibrarySort            | 256  | Sorted             |  21,243.6 ns |   178.63 ns |    79.31 ns | 125.92 |    5.57 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | Sorted             |   9,816.8 ns |   262.10 ns |   137.08 ns |  58.19 |    2.68 |    4 |         - |          NA |
| ShellSortKnuth1973     | 256  | Sorted             |     635.0 ns |     6.31 ns |     2.25 ns |   3.76 |    0.17 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Sorted             |     665.3 ns |    22.86 ns |    10.15 ns |   3.94 |    0.18 |    3 |         - |          NA |
| ShellSortTokuda1992    | 256  | Sorted             |     809.7 ns |    17.76 ns |     9.29 ns |   4.80 |    0.22 |    3 |         - |          NA |
| ShellSortCiura2001     | 256  | Sorted             |     706.0 ns |    11.30 ns |     5.91 ns |   4.18 |    0.19 |    3 |         - |          NA |
| ShellSortLee2021       | 256  | Sorted             |     788.0 ns |    22.40 ns |     9.94 ns |   4.67 |    0.21 |    3 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **256**  | **Reversed**           |  **11,971.0 ns** |   **300.28 ns** |   **157.05 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Reversed           |   9,399.3 ns |   215.51 ns |   112.72 ns |   0.79 |    0.01 |    3 |         - |          NA |
| BinaryInsertSort       | 256  | Reversed           |   4,822.4 ns |   313.28 ns |   163.85 ns |   0.40 |    0.01 |    2 |         - |          NA |
| GnomeSort              | 256  | Reversed           |  21,308.5 ns |   264.47 ns |   138.32 ns |   1.78 |    0.02 |    6 |         - |          NA |
| LibrarySort            | 256  | Reversed           |  17,618.3 ns |   220.56 ns |    97.93 ns |   1.47 |    0.02 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | Reversed           |   9,841.7 ns |   278.05 ns |   145.43 ns |   0.82 |    0.02 |    3 |         - |          NA |
| ShellSortKnuth1973     | 256  | Reversed           |   1,172.1 ns |    22.12 ns |     9.82 ns |   0.10 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Reversed           |   1,087.2 ns |    44.50 ns |    23.27 ns |   0.09 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | Reversed           |   1,117.0 ns |     7.38 ns |     3.28 ns |   0.09 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | Reversed           |   1,134.3 ns |     7.86 ns |     3.49 ns |   0.09 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Reversed           |   1,160.0 ns |    18.97 ns |     8.42 ns |   0.10 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **256**  | **PipeOrgan**          |   **6,264.1 ns** |   **250.86 ns** |   **131.21 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 256  | PipeOrgan          |   4,932.7 ns |   270.90 ns |   141.69 ns |   0.79 |    0.03 |    3 |         - |          NA |
| BinaryInsertSort       | 256  | PipeOrgan          |   3,037.7 ns |   238.04 ns |   105.69 ns |   0.49 |    0.02 |    2 |         - |          NA |
| GnomeSort              | 256  | PipeOrgan          |   9,914.0 ns |   226.24 ns |   118.33 ns |   1.58 |    0.04 |    5 |         - |          NA |
| LibrarySort            | 256  | PipeOrgan          |  15,027.7 ns |   268.04 ns |   140.19 ns |   2.40 |    0.05 |    6 |         - |          NA |
| MergeInsertionSort     | 256  | PipeOrgan          |  10,121.5 ns |   246.05 ns |   128.69 ns |   1.62 |    0.04 |    5 |         - |          NA |
| ShellSortKnuth1973     | 256  | PipeOrgan          |   1,042.3 ns |    34.66 ns |    15.39 ns |   0.17 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | PipeOrgan          |   1,054.6 ns |    31.17 ns |    13.84 ns |   0.17 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | PipeOrgan          |   1,121.3 ns |    35.17 ns |    12.54 ns |   0.18 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | PipeOrgan          |   1,056.5 ns |    45.54 ns |    20.22 ns |   0.17 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | PipeOrgan          |   1,117.4 ns |    25.04 ns |    11.12 ns |   0.18 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **256**  | **ManyDuplicates**     |   **7,397.0 ns** |    **50.93 ns** |    **22.61 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 256  | ManyDuplicates     |   7,170.6 ns |   121.26 ns |    63.42 ns |   0.97 |    0.01 |    1 |         - |          NA |
| BinaryInsertSort       | 256  | ManyDuplicates     |  11,191.0 ns |   310.60 ns |   162.45 ns |   1.51 |    0.02 |    2 |         - |          NA |
| GnomeSort              | 256  | ManyDuplicates     |  10,691.2 ns |   255.52 ns |   133.64 ns |   1.45 |    0.02 |    2 |         - |          NA |
| LibrarySort            | 256  | ManyDuplicates     |  23,917.9 ns |   219.88 ns |   115.00 ns |   3.23 |    0.02 |    3 |         - |          NA |
| MergeInsertionSort     | 256  | ManyDuplicates     |  20,134.6 ns |   196.97 ns |    87.46 ns |   2.72 |    0.01 |    3 |         - |          NA |
| ShellSortKnuth1973     | 256  | ManyDuplicates     |   7,195.7 ns |    55.85 ns |    24.80 ns |   0.97 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | ManyDuplicates     |   7,556.2 ns |   275.68 ns |   144.18 ns |   1.02 |    0.02 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | ManyDuplicates     |   7,350.7 ns |   116.58 ns |    60.97 ns |   0.99 |    0.01 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | ManyDuplicates     |   7,484.0 ns |   111.84 ns |    49.66 ns |   1.01 |    0.01 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | ManyDuplicates     |   7,210.0 ns |    31.53 ns |    11.24 ns |   0.97 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **1024** | **Random**             |  **94,088.3 ns** |   **925.35 ns** |   **410.86 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Random             |  90,704.1 ns | 1,221.82 ns |   542.50 ns |   0.96 |    0.01 |    3 |         - |          NA |
| BinaryInsertSort       | 1024 | Random             |  62,898.0 ns |   431.37 ns |   225.61 ns |   0.67 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | Random             | 150,566.4 ns |   568.29 ns |   252.32 ns |   1.60 |    0.01 |    4 |         - |          NA |
| LibrarySort            | 1024 | Random             | 106,618.2 ns |   701.23 ns |   311.35 ns |   1.13 |    0.01 |    3 |         - |          NA |
| MergeInsertionSort     | 1024 | Random             | 106,707.8 ns |   845.48 ns |   375.40 ns |   1.13 |    0.01 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Random             |  43,741.7 ns |   303.47 ns |   134.74 ns |   0.46 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Random             |  46,184.7 ns |   197.00 ns |   103.03 ns |   0.49 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Random             |  47,644.5 ns |   230.73 ns |   120.67 ns |   0.51 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Random             |  47,257.1 ns |   206.50 ns |    91.69 ns |   0.50 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Random             |  47,952.8 ns |   122.66 ns |    54.46 ns |   0.51 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **1024** | **SingleElementMoved** |     **716.7 ns** |    **21.01 ns** |     **9.33 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | SingleElementMoved |     859.0 ns |     6.65 ns |     2.95 ns |   1.20 |    0.01 |    2 |         - |          NA |
| BinaryInsertSort       | 1024 | SingleElementMoved |   5,211.2 ns | 1,077.11 ns |   563.35 ns |   7.27 |    0.75 |    5 |         - |          NA |
| GnomeSort              | 1024 | SingleElementMoved |   1,126.7 ns |    13.02 ns |     6.81 ns |   1.57 |    0.02 |    3 |         - |          NA |
| LibrarySort            | 1024 | SingleElementMoved | 192,244.6 ns |   630.35 ns |   279.88 ns | 268.26 |    3.23 |    8 |         - |          NA |
| MergeInsertionSort     | 1024 | SingleElementMoved |  48,653.2 ns |   499.58 ns |   261.29 ns |  67.89 |    0.88 |    7 |         - |          NA |
| ShellSortKnuth1973     | 1024 | SingleElementMoved |   3,508.9 ns |    46.18 ns |    24.15 ns |   4.90 |    0.07 |    4 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | SingleElementMoved |   6,430.0 ns | 3,111.47 ns | 1,627.36 ns |   8.97 |    2.15 |    6 |         - |          NA |
| ShellSortTokuda1992    | 1024 | SingleElementMoved |   4,421.8 ns |   208.58 ns |   109.09 ns |   6.17 |    0.16 |    4 |         - |          NA |
| ShellSortCiura2001     | 1024 | SingleElementMoved |   4,145.6 ns |   188.25 ns |    98.46 ns |   5.78 |    0.15 |    4 |         - |          NA |
| ShellSortLee2021       | 1024 | SingleElementMoved |   4,288.6 ns |   223.86 ns |   117.08 ns |   5.98 |    0.17 |    4 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **1024** | **Sorted**             |     **588.3 ns** |     **2.97 ns** |     **1.32 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Sorted             |     716.5 ns |     5.84 ns |     2.59 ns |   1.22 |    0.00 |    2 |         - |          NA |
| BinaryInsertSort       | 1024 | Sorted             |   4,140.7 ns |    40.85 ns |    21.37 ns |   7.04 |    0.04 |    3 |         - |          NA |
| GnomeSort              | 1024 | Sorted             |     842.8 ns |     1.04 ns |     0.46 ns |   1.43 |    0.00 |    2 |         - |          NA |
| LibrarySort            | 1024 | Sorted             | 192,530.7 ns |   808.61 ns |   359.03 ns | 327.28 |    0.89 |    5 |         - |          NA |
| MergeInsertionSort     | 1024 | Sorted             |  48,048.1 ns |   235.81 ns |   104.70 ns |  81.68 |    0.24 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Sorted             |   3,097.4 ns |    23.74 ns |     8.47 ns |   5.27 |    0.02 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Sorted             |   3,471.1 ns |    17.16 ns |     6.12 ns |   5.90 |    0.02 |    3 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Sorted             |   3,856.8 ns |   289.31 ns |   151.32 ns |   6.56 |    0.24 |    3 |         - |          NA |
| ShellSortCiura2001     | 1024 | Sorted             |   3,900.5 ns |   249.03 ns |   130.25 ns |   6.63 |    0.21 |    3 |         - |          NA |
| ShellSortLee2021       | 1024 | Sorted             |   3,754.2 ns |    11.12 ns |     3.97 ns |   6.38 |    0.01 |    3 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **1024** | **Reversed**           | **178,080.1 ns** |   **961.68 ns** |   **342.94 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Reversed           | 145,979.4 ns | 1,041.99 ns |   371.58 ns |   0.82 |    0.00 |    4 |         - |          NA |
| BinaryInsertSort       | 1024 | Reversed           |  34,980.6 ns |   179.77 ns |    94.03 ns |   0.20 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | Reversed           | 300,595.4 ns | 3,467.98 ns | 1,813.82 ns |   1.69 |    0.01 |    5 |         - |          NA |
| LibrarySort            | 1024 | Reversed           | 152,189.3 ns | 1,038.55 ns |   461.12 ns |   0.85 |    0.00 |    4 |         - |          NA |
| MergeInsertionSort     | 1024 | Reversed           |  48,650.3 ns |   340.43 ns |   151.15 ns |   0.27 |    0.00 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Reversed           |   5,383.6 ns |   145.52 ns |    76.11 ns |   0.03 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Reversed           |   5,551.0 ns |    72.75 ns |    38.05 ns |   0.03 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Reversed           |   6,130.5 ns |   248.01 ns |   129.71 ns |   0.03 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Reversed           |   5,818.0 ns |   260.75 ns |   136.37 ns |   0.03 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Reversed           |   5,846.1 ns |    39.37 ns |    14.04 ns |   0.03 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **1024** | **PipeOrgan**          |  **92,271.8 ns** | **4,792.28 ns** | **2,506.45 ns** |   **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | PipeOrgan          |  73,624.8 ns |   200.43 ns |    88.99 ns |   0.80 |    0.02 |    4 |         - |          NA |
| BinaryInsertSort       | 1024 | PipeOrgan          |  20,390.2 ns |   340.06 ns |   177.86 ns |   0.22 |    0.01 |    2 |         - |          NA |
| GnomeSort              | 1024 | PipeOrgan          | 147,982.8 ns | 1,720.13 ns |   899.66 ns |   1.60 |    0.04 |    6 |         - |          NA |
| LibrarySort            | 1024 | PipeOrgan          |  91,500.8 ns | 1,202.78 ns |   534.04 ns |   0.99 |    0.03 |    5 |         - |          NA |
| MergeInsertionSort     | 1024 | PipeOrgan          |  49,544.6 ns |   655.16 ns |   342.66 ns |   0.54 |    0.01 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | PipeOrgan          |   5,224.6 ns |   167.78 ns |    87.75 ns |   0.06 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | PipeOrgan          |   5,462.6 ns |   197.03 ns |    87.48 ns |   0.06 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | PipeOrgan          |   5,619.6 ns |   109.65 ns |    48.69 ns |   0.06 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | PipeOrgan          |   5,777.6 ns |   228.41 ns |   119.46 ns |   0.06 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | PipeOrgan          |   5,735.7 ns |   107.77 ns |    56.36 ns |   0.06 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **1024** | **ManyDuplicates**     |  **91,621.1 ns** |   **349.87 ns** |   **124.77 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | ManyDuplicates     |  88,584.6 ns | 2,074.65 ns |   921.16 ns |   0.97 |    0.01 |    3 |         - |          NA |
| BinaryInsertSort       | 1024 | ManyDuplicates     |  59,629.6 ns |   362.00 ns |   160.73 ns |   0.65 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | ManyDuplicates     | 146,957.3 ns |   171.24 ns |    76.03 ns |   1.60 |    0.00 |    4 |         - |          NA |
| LibrarySort            | 1024 | ManyDuplicates     | 112,367.6 ns |   590.49 ns |   308.84 ns |   1.23 |    0.00 |    3 |         - |          NA |
| MergeInsertionSort     | 1024 | ManyDuplicates     | 104,967.2 ns |   595.13 ns |   264.24 ns |   1.15 |    0.00 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | ManyDuplicates     |  33,846.7 ns |   297.27 ns |   155.48 ns |   0.37 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | ManyDuplicates     |  34,873.2 ns |   130.97 ns |    58.15 ns |   0.38 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | ManyDuplicates     |  34,949.1 ns |    89.14 ns |    46.62 ns |   0.38 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | ManyDuplicates     |  34,491.6 ns |   613.48 ns |   272.39 ns |   0.38 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | ManyDuplicates     |  33,552.9 ns |   138.44 ns |    72.41 ns |   0.37 |    0.00 |    1 |         - |          NA |

### MergeBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.46 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8
UnrollFactor=1  WarmupCount=2
```

| Method                   | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **MergeSort**                | **256**  | **Random**             |    **12,581.8 ns** |    **275.64 ns** |    **144.17 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Random             |    12,021.2 ns |    215.98 ns |    112.96 ns |  0.96 |    0.01 |    3 |         - |          NA |
| BottomupMergeSort        | 256  | Random             |     9,109.4 ns |    311.12 ns |    162.72 ns |  0.72 |    0.01 |    3 |         - |          NA |
| StdStableSort            | 256  | Random             |     5,370.8 ns |    351.23 ns |    183.70 ns |  0.43 |    0.01 |    2 |         - |          NA |
| RotateMergeSort          | 256  | Random             |    18,631.6 ns |    503.04 ns |    263.10 ns |  1.48 |    0.03 |    4 |         - |          NA |
| RotateMergeSortRecursive | 256  | Random             |    22,820.5 ns |    582.64 ns |    304.73 ns |  1.81 |    0.03 |    5 |         - |          NA |
| SymMergeSort             | 256  | Random             |    12,441.1 ns |    227.28 ns |    118.87 ns |  0.99 |    0.01 |    3 |         - |          NA |
| BlockMergeSort           | 256  | Random             |     9,729.4 ns |    276.88 ns |    144.82 ns |  0.77 |    0.01 |    3 |         - |          NA |
| NaturalMergeSort         | 256  | Random             |     9,169.7 ns |    380.11 ns |    198.80 ns |  0.73 |    0.02 |    3 |         - |          NA |
| PatienceSort             | 256  | Random             |    13,223.0 ns |    208.93 ns |    109.27 ns |  1.05 |    0.01 |    3 |         - |          NA |
| DropMergeSort            | 256  | Random             |     9,372.7 ns |    350.68 ns |    183.41 ns |  0.75 |    0.02 |    3 |         - |          NA |
| TimSort                  | 256  | Random             |    10,240.7 ns |    419.07 ns |    186.07 ns |  0.81 |    0.02 |    3 |         - |          NA |
| PowerSort                | 256  | Random             |     6,508.8 ns |     94.30 ns |     41.87 ns |  0.52 |    0.01 |    2 |         - |          NA |
| ShiftSort                | 256  | Random             |    10,040.6 ns |    307.92 ns |    136.72 ns |  0.80 |    0.01 |    3 |         - |          NA |
| SpinSort                 | 256  | Random             |     5,735.7 ns |     70.61 ns |     25.18 ns |  0.46 |    0.01 |    2 |         - |          NA |
| Glidesort                | 256  | Random             |     5,845.1 ns |    482.76 ns |    252.50 ns |  0.46 |    0.02 |    2 |         - |          NA |
| Driftsort                | 256  | Random             |     4,861.6 ns |    304.06 ns |    159.03 ns |  0.39 |    0.01 |    2 |         - |          NA |
| FlatStableSort           | 256  | Random             |     6,536.6 ns |     54.93 ns |     24.39 ns |  0.52 |    0.01 |    2 |         - |          NA |
| QuadSort                 | 256  | Random             |     3,569.3 ns |    350.22 ns |    183.17 ns |  0.28 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **SingleElementMoved** |     **3,480.8 ns** |     **40.94 ns** |     **18.18 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | SingleElementMoved |     4,220.6 ns |     36.91 ns |     16.39 ns |  1.21 |    0.01 |    7 |         - |          NA |
| BottomupMergeSort        | 256  | SingleElementMoved |     1,346.6 ns |     73.98 ns |     32.85 ns |  0.39 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 256  | SingleElementMoved |       571.6 ns |     12.57 ns |      5.58 ns |  0.16 |    0.00 |    2 |         - |          NA |
| RotateMergeSort          | 256  | SingleElementMoved |       517.8 ns |     37.80 ns |     16.78 ns |  0.15 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 256  | SingleElementMoved |       702.4 ns |     33.33 ns |     14.80 ns |  0.20 |    0.00 |    2 |         - |          NA |
| SymMergeSort             | 256  | SingleElementMoved |       377.8 ns |     23.73 ns |     10.53 ns |  0.11 |    0.00 |    1 |         - |          NA |
| BlockMergeSort           | 256  | SingleElementMoved |     2,230.7 ns |     75.32 ns |     33.44 ns |  0.64 |    0.01 |    5 |         - |          NA |
| NaturalMergeSort         | 256  | SingleElementMoved |       345.8 ns |     17.02 ns |      7.56 ns |  0.10 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 256  | SingleElementMoved |     7,275.9 ns |    598.57 ns |    313.06 ns |  2.09 |    0.09 |    8 |         - |          NA |
| DropMergeSort            | 256  | SingleElementMoved |       562.2 ns |     32.35 ns |     16.92 ns |  0.16 |    0.00 |    2 |         - |          NA |
| TimSort                  | 256  | SingleElementMoved |       303.0 ns |     35.35 ns |     18.49 ns |  0.09 |    0.01 |    1 |         - |          NA |
| PowerSort                | 256  | SingleElementMoved |       339.2 ns |     20.51 ns |     10.73 ns |  0.10 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | SingleElementMoved |       406.1 ns |     14.99 ns |      7.84 ns |  0.12 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 256  | SingleElementMoved |       700.3 ns |    156.37 ns |     69.43 ns |  0.20 |    0.02 |    2 |         - |          NA |
| Glidesort                | 256  | SingleElementMoved |       670.5 ns |     64.46 ns |     28.62 ns |  0.19 |    0.01 |    2 |         - |          NA |
| Driftsort                | 256  | SingleElementMoved |       582.0 ns |     16.51 ns |      7.33 ns |  0.17 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 256  | SingleElementMoved |     1,086.0 ns |     54.59 ns |     24.24 ns |  0.31 |    0.01 |    3 |         - |          NA |
| QuadSort                 | 256  | SingleElementMoved |       531.6 ns |     36.65 ns |     16.27 ns |  0.15 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **Sorted**             |     **3,296.0 ns** |      **4.16 ns** |      **1.48 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Sorted             |     4,011.6 ns |      7.07 ns |      3.14 ns |  1.22 |    0.00 |    7 |         - |          NA |
| BottomupMergeSort        | 256  | Sorted             |     1,173.1 ns |     25.09 ns |     11.14 ns |  0.36 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 256  | Sorted             |       490.8 ns |      7.81 ns |      4.08 ns |  0.15 |    0.00 |    3 |         - |          NA |
| RotateMergeSort          | 256  | Sorted             |       237.3 ns |      2.22 ns |      0.99 ns |  0.07 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 256  | Sorted             |       409.9 ns |      4.79 ns |      2.50 ns |  0.12 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 256  | Sorted             |       249.7 ns |     17.65 ns |      7.84 ns |  0.08 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 256  | Sorted             |     1,997.9 ns |     12.61 ns |      5.60 ns |  0.61 |    0.00 |    5 |         - |          NA |
| NaturalMergeSort         | 256  | Sorted             |       169.7 ns |      4.34 ns |      1.93 ns |  0.05 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 256  | Sorted             |     6,185.9 ns |    572.48 ns |    299.42 ns |  1.88 |    0.09 |    8 |         - |          NA |
| DropMergeSort            | 256  | Sorted             |       456.5 ns |      4.36 ns |      1.94 ns |  0.14 |    0.00 |    3 |         - |          NA |
| TimSort                  | 256  | Sorted             |       139.2 ns |      2.45 ns |      1.09 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | Sorted             |       190.7 ns |     31.71 ns |     16.59 ns |  0.06 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Sorted             |       183.3 ns |      2.74 ns |      0.98 ns |  0.06 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 256  | Sorted             |       136.5 ns |     12.80 ns |      5.68 ns |  0.04 |    0.00 |    1 |         - |          NA |
| Glidesort                | 256  | Sorted             |       161.0 ns |      4.28 ns |      1.90 ns |  0.05 |    0.00 |    1 |         - |          NA |
| Driftsort                | 256  | Sorted             |       169.4 ns |      5.37 ns |      2.38 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | Sorted             |       996.9 ns |     18.77 ns |      9.82 ns |  0.30 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 256  | Sorted             |       263.3 ns |     16.40 ns |      7.28 ns |  0.08 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **Reversed**           |     **6,921.9 ns** |     **16.08 ns** |      **5.74 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Reversed           |     6,296.5 ns |    281.58 ns |    147.27 ns |  0.91 |    0.02 |    6 |         - |          NA |
| BottomupMergeSort        | 256  | Reversed           |     3,732.9 ns |     44.60 ns |     15.91 ns |  0.54 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 256  | Reversed           |     3,166.0 ns |     14.59 ns |      5.20 ns |  0.46 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 256  | Reversed           |     4,312.0 ns |     58.22 ns |     20.76 ns |  0.62 |    0.00 |    5 |         - |          NA |
| RotateMergeSortRecursive | 256  | Reversed           |     6,081.4 ns |    209.83 ns |     93.17 ns |  0.88 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 256  | Reversed           |     1,756.4 ns |      6.17 ns |      2.74 ns |  0.25 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 256  | Reversed           |     2,404.2 ns |     15.40 ns |      6.84 ns |  0.35 |    0.00 |    4 |         - |          NA |
| NaturalMergeSort         | 256  | Reversed           |       232.8 ns |      7.92 ns |      4.14 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 256  | Reversed           |     1,280.4 ns |     14.23 ns |      6.32 ns |  0.18 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 256  | Reversed           |     1,232.6 ns |     15.37 ns |      5.48 ns |  0.18 |    0.00 |    2 |         - |          NA |
| TimSort                  | 256  | Reversed           |       200.0 ns |      2.98 ns |      1.32 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | Reversed           |       196.4 ns |      2.97 ns |      1.06 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Reversed           |     3,222.8 ns |     44.15 ns |     19.60 ns |  0.47 |    0.00 |    5 |         - |          NA |
| SpinSort                 | 256  | Reversed           |       225.9 ns |     10.35 ns |      4.60 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 256  | Reversed           |       243.7 ns |     15.94 ns |      7.08 ns |  0.04 |    0.00 |    1 |         - |          NA |
| Driftsort                | 256  | Reversed           |       249.4 ns |     14.21 ns |      6.31 ns |  0.04 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | Reversed           |     1,735.9 ns |      7.55 ns |      3.35 ns |  0.25 |    0.00 |    3 |         - |          NA |
| QuadSort                 | 256  | Reversed           |       227.0 ns |     29.42 ns |     15.39 ns |  0.03 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **PipeOrgan**          |     **5,317.3 ns** |    **272.29 ns** |    **142.41 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | PipeOrgan          |     5,194.1 ns |     38.67 ns |     13.79 ns |  0.98 |    0.02 |    6 |         - |          NA |
| BottomupMergeSort        | 256  | PipeOrgan          |     2,508.7 ns |     24.36 ns |     10.82 ns |  0.47 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 256  | PipeOrgan          |     1,932.0 ns |     12.23 ns |      5.43 ns |  0.36 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 256  | PipeOrgan          |     4,331.9 ns |    165.35 ns |     86.48 ns |  0.82 |    0.03 |    6 |         - |          NA |
| RotateMergeSortRecursive | 256  | PipeOrgan          |     5,679.2 ns |    274.63 ns |    143.64 ns |  1.07 |    0.04 |    6 |         - |          NA |
| SymMergeSort             | 256  | PipeOrgan          |     2,723.3 ns |    217.43 ns |    113.72 ns |  0.51 |    0.02 |    5 |         - |          NA |
| BlockMergeSort           | 256  | PipeOrgan          |     2,431.6 ns |      5.62 ns |      2.50 ns |  0.46 |    0.01 |    5 |         - |          NA |
| NaturalMergeSort         | 256  | PipeOrgan          |       407.6 ns |     17.87 ns |      9.35 ns |  0.08 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 256  | PipeOrgan          |     4,565.1 ns |    276.83 ns |    144.79 ns |  0.86 |    0.03 |    6 |         - |          NA |
| DropMergeSort            | 256  | PipeOrgan          |     5,122.2 ns |    303.29 ns |    158.63 ns |  0.96 |    0.04 |    6 |         - |          NA |
| TimSort                  | 256  | PipeOrgan          |       619.8 ns |     12.17 ns |      6.37 ns |  0.12 |    0.00 |    2 |         - |          NA |
| PowerSort                | 256  | PipeOrgan          |       390.2 ns |      8.04 ns |      3.57 ns |  0.07 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | PipeOrgan          |     2,168.7 ns |    292.09 ns |    152.77 ns |  0.41 |    0.03 |    5 |         - |          NA |
| SpinSort                 | 256  | PipeOrgan          |     1,585.7 ns |     30.82 ns |     13.68 ns |  0.30 |    0.01 |    4 |         - |          NA |
| Glidesort                | 256  | PipeOrgan          |       988.2 ns |     15.25 ns |      6.77 ns |  0.19 |    0.00 |    3 |         - |          NA |
| Driftsort                | 256  | PipeOrgan          |       379.8 ns |     10.36 ns |      5.42 ns |  0.07 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | PipeOrgan          |     1,520.7 ns |      8.01 ns |      3.56 ns |  0.29 |    0.01 |    4 |         - |          NA |
| QuadSort                 | 256  | PipeOrgan          |       681.3 ns |     15.25 ns |      7.98 ns |  0.13 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **ManyDuplicates**     |    **12,318.5 ns** |    **342.65 ns** |    **179.21 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | ManyDuplicates     |    11,600.0 ns |    186.40 ns |     82.76 ns |  0.94 |    0.01 |    3 |         - |          NA |
| BottomupMergeSort        | 256  | ManyDuplicates     |     8,712.3 ns |    322.14 ns |    168.49 ns |  0.71 |    0.02 |    3 |         - |          NA |
| StdStableSort            | 256  | ManyDuplicates     |     4,936.5 ns |    323.32 ns |    169.10 ns |  0.40 |    0.01 |    2 |         - |          NA |
| RotateMergeSort          | 256  | ManyDuplicates     |    18,393.6 ns |    503.46 ns |    223.54 ns |  1.49 |    0.03 |    4 |         - |          NA |
| RotateMergeSortRecursive | 256  | ManyDuplicates     |    22,133.0 ns |    455.72 ns |    238.35 ns |  1.80 |    0.03 |    5 |         - |          NA |
| SymMergeSort             | 256  | ManyDuplicates     |    12,077.7 ns |    421.13 ns |    220.26 ns |  0.98 |    0.02 |    3 |         - |          NA |
| BlockMergeSort           | 256  | ManyDuplicates     |     9,405.6 ns |    189.58 ns |     84.18 ns |  0.76 |    0.01 |    3 |         - |          NA |
| NaturalMergeSort         | 256  | ManyDuplicates     |     8,970.9 ns |    190.43 ns |     84.55 ns |  0.73 |    0.01 |    3 |         - |          NA |
| PatienceSort             | 256  | ManyDuplicates     |    11,045.5 ns |    375.19 ns |    196.23 ns |  0.90 |    0.02 |    3 |         - |          NA |
| DropMergeSort            | 256  | ManyDuplicates     |     8,103.9 ns |    238.48 ns |    105.89 ns |  0.66 |    0.01 |    3 |         - |          NA |
| TimSort                  | 256  | ManyDuplicates     |     9,627.7 ns |    229.77 ns |    102.02 ns |  0.78 |    0.01 |    3 |         - |          NA |
| PowerSort                | 256  | ManyDuplicates     |     6,125.0 ns |     98.45 ns |     43.71 ns |  0.50 |    0.01 |    2 |         - |          NA |
| ShiftSort                | 256  | ManyDuplicates     |     9,661.5 ns |    240.56 ns |    125.82 ns |  0.78 |    0.01 |    3 |         - |          NA |
| SpinSort                 | 256  | ManyDuplicates     |     5,427.3 ns |    177.34 ns |     92.75 ns |  0.44 |    0.01 |    2 |         - |          NA |
| Glidesort                | 256  | ManyDuplicates     |     5,341.3 ns |    275.60 ns |    122.37 ns |  0.43 |    0.01 |    2 |         - |          NA |
| Driftsort                | 256  | ManyDuplicates     |     4,703.1 ns |    553.02 ns |    289.24 ns |  0.38 |    0.02 |    2 |         - |          NA |
| FlatStableSort           | 256  | ManyDuplicates     |     6,543.4 ns |     55.66 ns |     24.71 ns |  0.53 |    0.01 |    2 |         - |          NA |
| QuadSort                 | 256  | ManyDuplicates     |     3,601.2 ns |    365.43 ns |    191.12 ns |  0.29 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **Random**             |    **58,414.1 ns** |    **263.47 ns** |    **137.80 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Random             |    55,955.4 ns |    606.51 ns |    317.22 ns |  0.96 |    0.01 |    3 |         - |          NA |
| BottomupMergeSort        | 1024 | Random             |    42,757.2 ns |    188.39 ns |     98.53 ns |  0.73 |    0.00 |    3 |         - |          NA |
| StdStableSort            | 1024 | Random             |    29,657.4 ns |  1,329.98 ns |    590.52 ns |  0.51 |    0.01 |    2 |         - |          NA |
| RotateMergeSort          | 1024 | Random             |   108,069.0 ns |    933.24 ns |    488.10 ns |  1.85 |    0.01 |    4 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Random             |   127,308.1 ns |    555.38 ns |    246.59 ns |  2.18 |    0.01 |    4 |         - |          NA |
| SymMergeSort             | 1024 | Random             |    72,680.6 ns |    813.35 ns |    425.40 ns |  1.24 |    0.01 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | Random             |    49,329.9 ns |    234.27 ns |    122.53 ns |  0.84 |    0.00 |    3 |         - |          NA |
| NaturalMergeSort         | 1024 | Random             |    44,414.3 ns |    182.57 ns |     95.49 ns |  0.76 |    0.00 |    3 |         - |          NA |
| PatienceSort             | 1024 | Random             |    64,544.6 ns |    601.20 ns |    314.44 ns |  1.10 |    0.01 |    3 |         - |          NA |
| DropMergeSort            | 1024 | Random             |    43,498.0 ns |    534.04 ns |    279.31 ns |  0.74 |    0.00 |    3 |         - |          NA |
| TimSort                  | 1024 | Random             |    50,127.3 ns |    368.64 ns |    163.68 ns |  0.86 |    0.00 |    3 |         - |          NA |
| PowerSort                | 1024 | Random             |    34,974.2 ns |    206.98 ns |     91.90 ns |  0.60 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 1024 | Random             |    50,310.0 ns |    181.87 ns |     80.75 ns |  0.86 |    0.00 |    3 |         - |          NA |
| SpinSort                 | 1024 | Random             |    31,617.0 ns |    492.27 ns |    257.47 ns |  0.54 |    0.00 |    2 |         - |          NA |
| Glidesort                | 1024 | Random             |    24,212.0 ns |    273.78 ns |    121.56 ns |  0.41 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | Random             |    20,891.2 ns |    241.48 ns |    107.22 ns |  0.36 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Random             |    34,203.5 ns |    341.62 ns |    178.67 ns |  0.59 |    0.00 |    2 |         - |          NA |
| QuadSort                 | 1024 | Random             |    17,810.2 ns |    229.76 ns |    120.17 ns |  0.30 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **SingleElementMoved** |    **13,896.6 ns** |    **122.08 ns** |     **63.85 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | SingleElementMoved |    16,849.0 ns |    208.52 ns |     92.58 ns |  1.21 |    0.01 |    9 |         - |          NA |
| BottomupMergeSort        | 1024 | SingleElementMoved |     5,006.5 ns |     17.81 ns |      6.35 ns |  0.36 |    0.00 |    6 |         - |          NA |
| StdStableSort            | 1024 | SingleElementMoved |     2,634.3 ns |     31.95 ns |     14.19 ns |  0.19 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 1024 | SingleElementMoved |     1,489.4 ns |     60.56 ns |     26.89 ns |  0.11 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 1024 | SingleElementMoved |     2,178.4 ns |     57.44 ns |     25.50 ns |  0.16 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 1024 | SingleElementMoved |     1,222.4 ns |     46.42 ns |     24.28 ns |  0.09 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 1024 | SingleElementMoved |     9,697.4 ns |    252.74 ns |    132.19 ns |  0.70 |    0.01 |    7 |         - |          NA |
| NaturalMergeSort         | 1024 | SingleElementMoved |     1,052.2 ns |      9.48 ns |      4.96 ns |  0.08 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 1024 | SingleElementMoved |    37,469.5 ns |  1,003.15 ns |    445.41 ns |  2.70 |    0.03 |   10 |         - |          NA |
| DropMergeSort            | 1024 | SingleElementMoved |     1,869.8 ns |     25.08 ns |     11.14 ns |  0.13 |    0.00 |    4 |         - |          NA |
| TimSort                  | 1024 | SingleElementMoved |       744.4 ns |     60.84 ns |     27.01 ns |  0.05 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | SingleElementMoved |     1,071.5 ns |     23.64 ns |      8.43 ns |  0.08 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 1024 | SingleElementMoved |     1,221.4 ns |     10.82 ns |      5.66 ns |  0.09 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 1024 | SingleElementMoved |     2,954.8 ns |    309.46 ns |    161.85 ns |  0.21 |    0.01 |    5 |         - |          NA |
| Glidesort                | 1024 | SingleElementMoved |     1,858.2 ns |     28.37 ns |     12.59 ns |  0.13 |    0.00 |    4 |         - |          NA |
| Driftsort                | 1024 | SingleElementMoved |     1,076.9 ns |     25.16 ns |     11.17 ns |  0.08 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 1024 | SingleElementMoved |     4,465.6 ns |     19.81 ns |      8.80 ns |  0.32 |    0.00 |    6 |         - |          NA |
| QuadSort                 | 1024 | SingleElementMoved |     1,209.0 ns |     63.21 ns |     28.07 ns |  0.09 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **Sorted**             |    **13,337.3 ns** |    **228.27 ns** |    **119.39 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Sorted             |    16,217.3 ns |     70.35 ns |     31.24 ns |  1.22 |    0.01 |    8 |         - |          NA |
| BottomupMergeSort        | 1024 | Sorted             |     4,528.9 ns |     30.92 ns |     16.17 ns |  0.34 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 1024 | Sorted             |     2,399.6 ns |     26.93 ns |     11.96 ns |  0.18 |    0.00 |    4 |         - |          NA |
| RotateMergeSort          | 1024 | Sorted             |       954.1 ns |     16.89 ns |      8.83 ns |  0.07 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Sorted             |     1,529.4 ns |      8.37 ns |      3.71 ns |  0.11 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 1024 | Sorted             |       908.4 ns |     13.91 ns |      6.18 ns |  0.07 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 1024 | Sorted             |     8,489.3 ns |    310.92 ns |    162.62 ns |  0.64 |    0.01 |    6 |         - |          NA |
| NaturalMergeSort         | 1024 | Sorted             |       530.1 ns |      2.39 ns |      1.06 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 1024 | Sorted             |    31,446.1 ns |    277.81 ns |    123.35 ns |  2.36 |    0.02 |    9 |         - |          NA |
| DropMergeSort            | 1024 | Sorted             |     1,725.8 ns |     25.96 ns |     11.52 ns |  0.13 |    0.00 |    3 |         - |          NA |
| TimSort                  | 1024 | Sorted             |       482.3 ns |      2.79 ns |      1.46 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Sorted             |       490.6 ns |     11.79 ns |      5.24 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Sorted             |       618.6 ns |      7.76 ns |      3.45 ns |  0.05 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 1024 | Sorted             |       435.0 ns |      7.91 ns |      4.14 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Sorted             |       432.8 ns |      4.08 ns |      2.14 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | Sorted             |       510.3 ns |      8.98 ns |      3.99 ns |  0.04 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Sorted             |     4,030.8 ns |     20.64 ns |     10.80 ns |  0.30 |    0.00 |    5 |         - |          NA |
| QuadSort                 | 1024 | Sorted             |       696.1 ns |     14.66 ns |      6.51 ns |  0.05 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **Reversed**           |    **29,026.8 ns** |    **267.10 ns** |    **139.70 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Reversed           |    25,449.6 ns |    174.21 ns |     77.35 ns |  0.88 |    0.00 |    7 |         - |          NA |
| BottomupMergeSort        | 1024 | Reversed           |    15,408.9 ns |    113.37 ns |     50.34 ns |  0.53 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 1024 | Reversed           |    13,725.7 ns |    221.05 ns |     98.15 ns |  0.47 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 1024 | Reversed           |    20,540.7 ns |    256.88 ns |    114.06 ns |  0.71 |    0.00 |    6 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Reversed           |    27,986.7 ns |    237.26 ns |    124.09 ns |  0.96 |    0.01 |    7 |         - |          NA |
| SymMergeSort             | 1024 | Reversed           |     7,586.6 ns |    227.37 ns |    118.92 ns |  0.26 |    0.00 |    4 |         - |          NA |
| BlockMergeSort           | 1024 | Reversed           |    11,752.1 ns |    315.45 ns |    164.98 ns |  0.40 |    0.01 |    5 |         - |          NA |
| NaturalMergeSort         | 1024 | Reversed           |       760.7 ns |      7.17 ns |      2.56 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 1024 | Reversed           |     4,399.1 ns |    196.57 ns |    102.81 ns |  0.15 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 1024 | Reversed           |     5,480.7 ns |    136.20 ns |     71.23 ns |  0.19 |    0.00 |    3 |         - |          NA |
| TimSort                  | 1024 | Reversed           |       757.7 ns |      3.77 ns |      1.97 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Reversed           |       717.7 ns |      3.93 ns |      2.05 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Reversed           |    14,630.1 ns |     62.51 ns |     27.75 ns |  0.50 |    0.00 |    5 |         - |          NA |
| SpinSort                 | 1024 | Reversed           |       764.2 ns |     10.28 ns |      4.57 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Reversed           |       765.6 ns |     47.60 ns |     21.14 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | Reversed           |       781.8 ns |      4.81 ns |      2.14 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Reversed           |     7,018.6 ns |    296.39 ns |    131.60 ns |  0.24 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 1024 | Reversed           |       766.3 ns |      3.40 ns |      1.21 ns |  0.03 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **PipeOrgan**          |    **21,554.0 ns** |    **215.37 ns** |    **112.64 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | PipeOrgan          |    21,233.4 ns |    194.35 ns |    101.65 ns |  0.99 |    0.01 |    5 |         - |          NA |
| BottomupMergeSort        | 1024 | PipeOrgan          |    10,391.1 ns |    229.46 ns |    120.01 ns |  0.48 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 1024 | PipeOrgan          |     8,410.7 ns |    237.74 ns |    124.34 ns |  0.39 |    0.01 |    4 |         - |          NA |
| RotateMergeSort          | 1024 | PipeOrgan          |    19,865.7 ns |    310.30 ns |    162.29 ns |  0.92 |    0.01 |    5 |         - |          NA |
| RotateMergeSortRecursive | 1024 | PipeOrgan          |    25,101.0 ns |    436.19 ns |    228.13 ns |  1.16 |    0.01 |    5 |         - |          NA |
| SymMergeSort             | 1024 | PipeOrgan          |    11,621.8 ns |    132.13 ns |     58.67 ns |  0.54 |    0.00 |    4 |         - |          NA |
| BlockMergeSort           | 1024 | PipeOrgan          |    11,970.8 ns |    224.38 ns |    117.36 ns |  0.56 |    0.01 |    4 |         - |          NA |
| NaturalMergeSort         | 1024 | PipeOrgan          |     1,372.0 ns |      8.43 ns |      3.01 ns |  0.06 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 1024 | PipeOrgan          |    22,344.3 ns |    453.80 ns |    201.49 ns |  1.04 |    0.01 |    5 |         - |          NA |
| DropMergeSort            | 1024 | PipeOrgan          |    21,616.0 ns |    191.33 ns |    100.07 ns |  1.00 |    0.01 |    5 |         - |          NA |
| TimSort                  | 1024 | PipeOrgan          |     2,205.6 ns |    303.59 ns |    158.78 ns |  0.10 |    0.01 |    2 |         - |          NA |
| PowerSort                | 1024 | PipeOrgan          |     1,407.4 ns |    221.09 ns |     98.17 ns |  0.07 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | PipeOrgan          |     9,405.4 ns |    295.88 ns |    154.75 ns |  0.44 |    0.01 |    4 |         - |          NA |
| SpinSort                 | 1024 | PipeOrgan          |     7,026.1 ns |    322.15 ns |    168.49 ns |  0.33 |    0.01 |    4 |         - |          NA |
| Glidesort                | 1024 | PipeOrgan          |     3,705.6 ns |     19.32 ns |      8.58 ns |  0.17 |    0.00 |    3 |         - |          NA |
| Driftsort                | 1024 | PipeOrgan          |     1,353.9 ns |    276.90 ns |    144.82 ns |  0.06 |    0.01 |    1 |         - |          NA |
| FlatStableSort           | 1024 | PipeOrgan          |     6,374.2 ns |    269.38 ns |    140.89 ns |  0.30 |    0.01 |    4 |         - |          NA |
| QuadSort                 | 1024 | PipeOrgan          |     2,357.5 ns |     19.38 ns |      8.61 ns |  0.11 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **ManyDuplicates**     |    **52,244.0 ns** |    **588.85 ns** |    **307.98 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | ManyDuplicates     |    49,064.4 ns |    198.96 ns |     88.34 ns |  0.94 |    0.01 |    4 |         - |          NA |
| BottomupMergeSort        | 1024 | ManyDuplicates     |    37,333.6 ns |    387.35 ns |    202.59 ns |  0.71 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 1024 | ManyDuplicates     |    23,507.5 ns |  1,637.12 ns |    856.25 ns |  0.45 |    0.02 |    4 |         - |          NA |
| RotateMergeSort          | 1024 | ManyDuplicates     |    95,654.0 ns |    613.44 ns |    272.37 ns |  1.83 |    0.01 |    6 |         - |          NA |
| RotateMergeSortRecursive | 1024 | ManyDuplicates     |   112,028.9 ns |  1,322.03 ns |    586.99 ns |  2.14 |    0.02 |    6 |         - |          NA |
| SymMergeSort             | 1024 | ManyDuplicates     |    63,005.1 ns |    415.90 ns |    217.53 ns |  1.21 |    0.01 |    5 |         - |          NA |
| BlockMergeSort           | 1024 | ManyDuplicates     |    46,429.2 ns |    403.72 ns |    211.16 ns |  0.89 |    0.01 |    4 |         - |          NA |
| NaturalMergeSort         | 1024 | ManyDuplicates     |    38,664.6 ns |    234.96 ns |    104.32 ns |  0.74 |    0.00 |    4 |         - |          NA |
| PatienceSort             | 1024 | ManyDuplicates     |    40,978.0 ns |    574.04 ns |    300.23 ns |  0.78 |    0.01 |    4 |         - |          NA |
| DropMergeSort            | 1024 | ManyDuplicates     |    31,121.6 ns |    186.07 ns |     82.62 ns |  0.60 |    0.00 |    4 |         - |          NA |
| TimSort                  | 1024 | ManyDuplicates     |    43,624.4 ns |    231.91 ns |    102.97 ns |  0.84 |    0.00 |    4 |         - |          NA |
| PowerSort                | 1024 | ManyDuplicates     |    28,185.5 ns |    367.22 ns |    192.06 ns |  0.54 |    0.00 |    4 |         - |          NA |
| ShiftSort                | 1024 | ManyDuplicates     |    43,166.9 ns |    215.69 ns |    112.81 ns |  0.83 |    0.01 |    4 |         - |          NA |
| SpinSort                 | 1024 | ManyDuplicates     |    25,186.7 ns |    437.95 ns |    229.06 ns |  0.48 |    0.00 |    4 |         - |          NA |
| Glidesort                | 1024 | ManyDuplicates     |    14,011.3 ns |    187.81 ns |     83.39 ns |  0.27 |    0.00 |    2 |         - |          NA |
| Driftsort                | 1024 | ManyDuplicates     |    11,602.4 ns |    338.02 ns |    176.79 ns |  0.22 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | ManyDuplicates     |    28,683.4 ns |    346.34 ns |    181.14 ns |  0.55 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 1024 | ManyDuplicates     |    17,756.3 ns |    179.17 ns |     93.71 ns |  0.34 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **Random**             |   **266,005.9 ns** |    **525.91 ns** |    **233.51 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Random             |   252,133.0 ns |  2,424.23 ns |  1,267.92 ns |  0.95 |    0.00 |    2 |         - |          NA |
| BottomupMergeSort        | 4096 | Random             |   197,922.9 ns |    474.72 ns |    210.78 ns |  0.74 |    0.00 |    2 |         - |          NA |
| StdStableSort            | 4096 | Random             |   150,379.4 ns |  3,046.73 ns |  1,593.50 ns |  0.57 |    0.01 |    2 |         - |          NA |
| RotateMergeSort          | 4096 | Random             |   572,244.9 ns |  1,807.71 ns |    945.47 ns |  2.15 |    0.00 |    4 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Random             |   663,240.2 ns |  2,121.54 ns |    941.98 ns |  2.49 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 4096 | Random             |   385,922.4 ns |  1,023.17 ns |    535.14 ns |  1.45 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 4096 | Random             |   243,151.6 ns |  1,476.74 ns |    772.36 ns |  0.91 |    0.00 |    2 |         - |          NA |
| NaturalMergeSort         | 4096 | Random             |   207,560.3 ns |    916.09 ns |    406.75 ns |  0.78 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 4096 | Random             |   303,855.9 ns |    902.85 ns |    472.21 ns |  1.14 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 4096 | Random             |   198,579.1 ns |  1,638.53 ns |    856.99 ns |  0.75 |    0.00 |    2 |         - |          NA |
| TimSort                  | 4096 | Random             |   234,317.1 ns |    724.91 ns |    321.87 ns |  0.88 |    0.00 |    2 |         - |          NA |
| PowerSort                | 4096 | Random             |   174,673.5 ns |    918.68 ns |    407.90 ns |  0.66 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 4096 | Random             |   241,552.9 ns |    802.01 ns |    356.10 ns |  0.91 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 4096 | Random             |   156,055.3 ns |  1,060.86 ns |    554.85 ns |  0.59 |    0.00 |    2 |         - |          NA |
| Glidesort                | 4096 | Random             |    99,880.5 ns |    227.89 ns |    101.19 ns |  0.38 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | Random             |    86,035.7 ns |    572.22 ns |    254.07 ns |  0.32 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Random             |   162,987.5 ns |  2,305.20 ns |  1,023.52 ns |  0.61 |    0.00 |    2 |         - |          NA |
| QuadSort                 | 4096 | Random             |    82,583.9 ns |    310.94 ns |    138.06 ns |  0.31 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **SingleElementMoved** |    **55,102.6 ns** |    **301.23 ns** |    **157.55 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | SingleElementMoved |    66,771.9 ns |    259.22 ns |    115.10 ns |  1.21 |    0.00 |    9 |         - |          NA |
| BottomupMergeSort        | 4096 | SingleElementMoved |    20,051.3 ns |    325.28 ns |    170.13 ns |  0.36 |    0.00 |    6 |         - |          NA |
| StdStableSort            | 4096 | SingleElementMoved |    12,828.0 ns |    286.52 ns |    149.85 ns |  0.23 |    0.00 |    4 |         - |          NA |
| RotateMergeSort          | 4096 | SingleElementMoved |     5,021.0 ns |    345.10 ns |    153.23 ns |  0.09 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 4096 | SingleElementMoved |     7,524.6 ns |    298.54 ns |    156.14 ns |  0.14 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 4096 | SingleElementMoved |     4,414.4 ns |    256.85 ns |    134.34 ns |  0.08 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 4096 | SingleElementMoved |    38,723.2 ns |    314.12 ns |    164.29 ns |  0.70 |    0.00 |    7 |         - |          NA |
| NaturalMergeSort         | 4096 | SingleElementMoved |     3,789.7 ns |     20.87 ns |      7.44 ns |  0.07 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 4096 | SingleElementMoved |   260,911.1 ns |  6,173.50 ns |  2,741.07 ns |  4.74 |    0.05 |   10 |         - |          NA |
| DropMergeSort            | 4096 | SingleElementMoved |     7,388.1 ns |    347.76 ns |    154.41 ns |  0.13 |    0.00 |    3 |         - |          NA |
| TimSort                  | 4096 | SingleElementMoved |     2,453.3 ns |     31.39 ns |     13.94 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | SingleElementMoved |     3,841.1 ns |     36.54 ns |     13.03 ns |  0.07 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 4096 | SingleElementMoved |     4,495.4 ns |    286.32 ns |    149.75 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 4096 | SingleElementMoved |     8,424.7 ns |    336.30 ns |    175.89 ns |  0.15 |    0.00 |    3 |         - |          NA |
| Glidesort                | 4096 | SingleElementMoved |     6,931.8 ns |    352.48 ns |    184.36 ns |  0.13 |    0.00 |    3 |         - |          NA |
| Driftsort                | 4096 | SingleElementMoved |     3,685.6 ns |     27.22 ns |      9.71 ns |  0.07 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 4096 | SingleElementMoved |    16,494.9 ns |    113.67 ns |     59.45 ns |  0.30 |    0.00 |    5 |         - |          NA |
| QuadSort                 | 4096 | SingleElementMoved |     3,868.6 ns |     62.51 ns |     22.29 ns |  0.07 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **Sorted**             |    **52,983.2 ns** |    **225.81 ns** |    **118.10 ns** |  **1.00** |    **0.00** |    **9** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Sorted             |    65,345.8 ns |    759.07 ns |    397.01 ns |  1.23 |    0.01 |   10 |         - |          NA |
| BottomupMergeSort        | 4096 | Sorted             |    18,450.2 ns |    228.76 ns |    119.64 ns |  0.35 |    0.00 |    7 |         - |          NA |
| StdStableSort            | 4096 | Sorted             |    12,189.5 ns |    315.53 ns |    165.03 ns |  0.23 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 4096 | Sorted             |     3,696.9 ns |     14.85 ns |      5.30 ns |  0.07 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Sorted             |     6,315.8 ns |    321.80 ns |    168.31 ns |  0.12 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 4096 | Sorted             |     3,604.6 ns |    138.11 ns |     61.32 ns |  0.07 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 4096 | Sorted             |    33,710.0 ns |    291.35 ns |    129.36 ns |  0.64 |    0.00 |    8 |         - |          NA |
| NaturalMergeSort         | 4096 | Sorted             |     1,833.3 ns |     17.00 ns |      7.55 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 4096 | Sorted             |   175,472.9 ns |  7,339.18 ns |  2,617.22 ns |  3.31 |    0.05 |   11 |         - |          NA |
| DropMergeSort            | 4096 | Sorted             |     6,883.4 ns |    348.06 ns |    182.04 ns |  0.13 |    0.00 |    4 |         - |          NA |
| TimSort                  | 4096 | Sorted             |     1,805.0 ns |     11.19 ns |      4.97 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | Sorted             |     1,806.3 ns |     14.97 ns |      6.65 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | Sorted             |     2,296.5 ns |     13.26 ns |      5.89 ns |  0.04 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 4096 | Sorted             |     1,583.0 ns |     14.16 ns |      7.41 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 4096 | Sorted             |     1,521.7 ns |     19.85 ns |      8.82 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | Sorted             |     1,843.0 ns |     13.10 ns |      4.67 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Sorted             |    14,627.1 ns |     66.57 ns |     34.82 ns |  0.28 |    0.00 |    6 |         - |          NA |
| QuadSort                 | 4096 | Sorted             |     2,407.9 ns |     16.30 ns |      7.24 ns |  0.05 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **Reversed**           |   **119,163.8 ns** |    **955.03 ns** |    **424.04 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Reversed           |   104,538.0 ns |  1,437.84 ns |    638.41 ns |  0.88 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 4096 | Reversed           |    63,962.5 ns |    250.49 ns |    131.01 ns |  0.54 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 4096 | Reversed           |    58,440.8 ns |    622.83 ns |    325.75 ns |  0.49 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 4096 | Reversed           |    93,880.5 ns |  1,685.75 ns |    748.48 ns |  0.79 |    0.01 |    6 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Reversed           |   123,989.9 ns |  1,188.50 ns |    621.61 ns |  1.04 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 4096 | Reversed           |    32,417.7 ns |    191.16 ns |     99.98 ns |  0.27 |    0.00 |    4 |         - |          NA |
| BlockMergeSort           | 4096 | Reversed           |    52,366.5 ns |    184.47 ns |     65.79 ns |  0.44 |    0.00 |    5 |         - |          NA |
| NaturalMergeSort         | 4096 | Reversed           |     2,831.9 ns |      5.64 ns |      2.50 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 4096 | Reversed           |    16,812.3 ns |    218.54 ns |    114.30 ns |  0.14 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 4096 | Reversed           |    23,941.5 ns |     75.62 ns |     33.58 ns |  0.20 |    0.00 |    3 |         - |          NA |
| TimSort                  | 4096 | Reversed           |     2,781.1 ns |     19.46 ns |      6.94 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | Reversed           |     2,764.1 ns |      8.07 ns |      3.58 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | Reversed           |    65,402.7 ns |    523.89 ns |    274.00 ns |  0.55 |    0.00 |    5 |         - |          NA |
| SpinSort                 | 4096 | Reversed           |     3,026.1 ns |    214.09 ns |     95.06 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 4096 | Reversed           |     2,827.7 ns |     17.17 ns |      7.62 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | Reversed           |     2,851.1 ns |     13.04 ns |      5.79 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Reversed           |    31,287.9 ns |    139.20 ns |     61.80 ns |  0.26 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 4096 | Reversed           |     2,986.3 ns |     10.28 ns |      3.67 ns |  0.03 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **PipeOrgan**          |    **87,550.8 ns** |    **297.08 ns** |    **155.38 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | PipeOrgan          |    86,212.4 ns |    739.80 ns |    328.47 ns |  0.98 |    0.00 |    7 |         - |          NA |
| BottomupMergeSort        | 4096 | PipeOrgan          |    42,557.0 ns |    432.25 ns |    226.07 ns |  0.49 |    0.00 |    6 |         - |          NA |
| StdStableSort            | 4096 | PipeOrgan          |    36,539.9 ns |    778.32 ns |    407.07 ns |  0.42 |    0.00 |    6 |         - |          NA |
| RotateMergeSort          | 4096 | PipeOrgan          |    88,557.4 ns |  1,681.37 ns |    879.39 ns |  1.01 |    0.01 |    7 |         - |          NA |
| RotateMergeSortRecursive | 4096 | PipeOrgan          |   108,633.9 ns |    511.36 ns |    227.04 ns |  1.24 |    0.00 |    8 |         - |          NA |
| SymMergeSort             | 4096 | PipeOrgan          |    47,964.8 ns |    707.26 ns |    314.03 ns |  0.55 |    0.00 |    6 |         - |          NA |
| BlockMergeSort           | 4096 | PipeOrgan          |    48,962.1 ns |    220.89 ns |    115.53 ns |  0.56 |    0.00 |    6 |         - |          NA |
| NaturalMergeSort         | 4096 | PipeOrgan          |     5,373.8 ns |    243.49 ns |    127.35 ns |  0.06 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 4096 | PipeOrgan          |   135,641.0 ns |  9,310.60 ns |  4,869.62 ns |  1.55 |    0.05 |    9 |         - |          NA |
| DropMergeSort            | 4096 | PipeOrgan          |    89,423.1 ns |  1,082.08 ns |    565.95 ns |  1.02 |    0.01 |    7 |         - |          NA |
| TimSort                  | 4096 | PipeOrgan          |     8,247.2 ns |    276.91 ns |    144.83 ns |  0.09 |    0.00 |    3 |         - |          NA |
| PowerSort                | 4096 | PipeOrgan          |     5,227.0 ns |    251.62 ns |    131.60 ns |  0.06 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | PipeOrgan          |    40,716.6 ns |    262.19 ns |    137.13 ns |  0.47 |    0.00 |    6 |         - |          NA |
| SpinSort                 | 4096 | PipeOrgan          |     6,575.9 ns |    190.84 ns |     99.81 ns |  0.08 |    0.00 |    2 |         - |          NA |
| Glidesort                | 4096 | PipeOrgan          |    14,628.4 ns |    177.90 ns |     93.05 ns |  0.17 |    0.00 |    4 |         - |          NA |
| Driftsort                | 4096 | PipeOrgan          |     4,454.4 ns |     19.35 ns |      6.90 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | PipeOrgan          |    28,457.9 ns |    530.49 ns |    277.46 ns |  0.33 |    0.00 |    5 |         - |          NA |
| QuadSort                 | 4096 | PipeOrgan          |     9,233.9 ns |    242.79 ns |    107.80 ns |  0.11 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **ManyDuplicates**     |   **216,145.2 ns** |    **820.74 ns** |    **364.41 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | ManyDuplicates     |   202,398.9 ns |  1,172.27 ns |    613.12 ns |  0.94 |    0.00 |    4 |         - |          NA |
| BottomupMergeSort        | 4096 | ManyDuplicates     |   152,881.7 ns |  1,065.41 ns |    557.23 ns |  0.71 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 4096 | ManyDuplicates     |   101,054.2 ns |  4,645.86 ns |  2,429.88 ns |  0.47 |    0.01 |    4 |         - |          NA |
| RotateMergeSort          | 4096 | ManyDuplicates     |   428,221.7 ns |  2,910.65 ns |  1,522.33 ns |  1.98 |    0.01 |    6 |         - |          NA |
| RotateMergeSortRecursive | 4096 | ManyDuplicates     |   503,648.9 ns |  1,710.90 ns |    894.83 ns |  2.33 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 4096 | ManyDuplicates     |   287,638.8 ns |  1,877.91 ns |    982.18 ns |  1.33 |    0.00 |    5 |         - |          NA |
| BlockMergeSort           | 4096 | ManyDuplicates     |   213,779.7 ns |  1,586.16 ns |    829.59 ns |  0.99 |    0.00 |    4 |         - |          NA |
| NaturalMergeSort         | 4096 | ManyDuplicates     |   158,560.0 ns |    456.29 ns |    202.60 ns |  0.73 |    0.00 |    4 |         - |          NA |
| PatienceSort             | 4096 | ManyDuplicates     |   140,284.8 ns |    703.52 ns |    367.96 ns |  0.65 |    0.00 |    4 |         - |          NA |
| DropMergeSort            | 4096 | ManyDuplicates     |   117,876.9 ns |    472.88 ns |    209.96 ns |  0.55 |    0.00 |    4 |         - |          NA |
| TimSort                  | 4096 | ManyDuplicates     |   183,609.7 ns |    718.39 ns |    318.97 ns |  0.85 |    0.00 |    4 |         - |          NA |
| PowerSort                | 4096 | ManyDuplicates     |   120,467.5 ns |    963.01 ns |    503.67 ns |  0.56 |    0.00 |    4 |         - |          NA |
| ShiftSort                | 4096 | ManyDuplicates     |   183,724.8 ns |    284.11 ns |    126.15 ns |  0.85 |    0.00 |    4 |         - |          NA |
| SpinSort                 | 4096 | ManyDuplicates     |   108,666.2 ns |  1,118.06 ns |    496.43 ns |  0.50 |    0.00 |    4 |         - |          NA |
| Glidesort                | 4096 | ManyDuplicates     |    22,253.7 ns |    754.27 ns |    394.50 ns |  0.10 |    0.00 |    2 |         - |          NA |
| Driftsort                | 4096 | ManyDuplicates     |    16,849.1 ns |    188.02 ns |     83.48 ns |  0.08 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | ManyDuplicates     |   115,235.9 ns |    505.80 ns |    224.58 ns |  0.53 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 4096 | ManyDuplicates     |    76,966.2 ns |    498.46 ns |    221.32 ns |  0.36 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **Random**             |   **564,475.7 ns** |  **2,261.31 ns** |  **1,004.03 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Random             |   532,731.3 ns |  2,650.84 ns |  1,176.99 ns |  0.94 |    0.00 |    2 |         - |          NA |
| BottomupMergeSort        | 8192 | Random             |   422,144.5 ns |  1,195.03 ns |    530.60 ns |  0.75 |    0.00 |    2 |         - |          NA |
| StdStableSort            | 8192 | Random             |   341,645.1 ns |  7,575.94 ns |  3,962.36 ns |  0.61 |    0.01 |    2 |         - |          NA |
| RotateMergeSort          | 8192 | Random             | 1,283,949.7 ns |  6,870.61 ns |  3,593.46 ns |  2.27 |    0.01 |    4 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Random             | 1,486,080.5 ns |  1,950.61 ns |  1,020.21 ns |  2.63 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 8192 | Random             |   867,428.2 ns |  1,890.84 ns |    988.95 ns |  1.54 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 8192 | Random             |   525,700.0 ns |  2,278.01 ns |  1,011.45 ns |  0.93 |    0.00 |    2 |         - |          NA |
| NaturalMergeSort         | 8192 | Random             |   446,110.5 ns |  1,610.97 ns |    842.57 ns |  0.79 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 8192 | Random             |   658,224.2 ns |  3,981.14 ns |  1,767.65 ns |  1.17 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 8192 | Random             |   421,238.6 ns |  4,906.74 ns |  2,178.62 ns |  0.75 |    0.00 |    2 |         - |          NA |
| TimSort                  | 8192 | Random             |   514,241.6 ns |  1,791.81 ns |    795.57 ns |  0.91 |    0.00 |    2 |         - |          NA |
| PowerSort                | 8192 | Random             |   382,777.5 ns |  1,821.22 ns |    952.53 ns |  0.68 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 8192 | Random             |   529,746.7 ns |    991.40 ns |    440.19 ns |  0.94 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | Random             |   342,914.0 ns |  2,302.88 ns |  1,204.45 ns |  0.61 |    0.00 |    2 |         - |          NA |
| Glidesort                | 8192 | Random             |   202,011.6 ns |    446.53 ns |    198.26 ns |  0.36 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Random             |   173,612.0 ns |    578.21 ns |    256.73 ns |  0.31 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Random             |   369,460.6 ns |  1,197.98 ns |    626.57 ns |  0.65 |    0.00 |    2 |         - |          NA |
| QuadSort                 | 8192 | Random             |   176,613.3 ns |    433.29 ns |    154.51 ns |  0.31 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **SingleElementMoved** |   **109,877.3 ns** |    **448.19 ns** |    **199.00 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | SingleElementMoved |   133,823.7 ns |    472.12 ns |    246.93 ns |  1.22 |    0.00 |    8 |         - |          NA |
| BottomupMergeSort        | 8192 | SingleElementMoved |    42,165.5 ns |    276.07 ns |    122.58 ns |  0.38 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 8192 | SingleElementMoved |    24,444.2 ns |    662.63 ns |    294.21 ns |  0.22 |    0.00 |    4 |         - |          NA |
| RotateMergeSort          | 8192 | SingleElementMoved |     9,445.0 ns |    102.43 ns |     45.48 ns |  0.09 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 8192 | SingleElementMoved |    14,499.5 ns |    684.17 ns |    303.78 ns |  0.13 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 8192 | SingleElementMoved |     8,633.4 ns |    264.23 ns |    138.20 ns |  0.08 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 8192 | SingleElementMoved |    76,390.7 ns |    560.73 ns |    248.97 ns |  0.70 |    0.00 |    6 |         - |          NA |
| NaturalMergeSort         | 8192 | SingleElementMoved |     7,731.0 ns |    266.21 ns |    139.23 ns |  0.07 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 8192 | SingleElementMoved |   621,977.5 ns |  3,229.69 ns |  1,434.00 ns |  5.66 |    0.02 |    9 |         - |          NA |
| DropMergeSort            | 8192 | SingleElementMoved |    14,813.8 ns |    236.34 ns |    104.94 ns |  0.13 |    0.00 |    3 |         - |          NA |
| TimSort                  | 8192 | SingleElementMoved |     4,942.4 ns |    528.17 ns |    276.24 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | SingleElementMoved |     7,724.6 ns |    314.31 ns |    164.39 ns |  0.07 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 8192 | SingleElementMoved |     8,789.3 ns |    272.95 ns |    142.76 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | SingleElementMoved |    14,245.3 ns |    178.02 ns |     79.04 ns |  0.13 |    0.00 |    3 |         - |          NA |
| Glidesort                | 8192 | SingleElementMoved |    13,545.9 ns |    250.43 ns |    111.19 ns |  0.12 |    0.00 |    3 |         - |          NA |
| Driftsort                | 8192 | SingleElementMoved |     7,848.7 ns |     83.24 ns |     43.54 ns |  0.07 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 8192 | SingleElementMoved |    23,229.8 ns |    274.30 ns |    121.79 ns |  0.21 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 8192 | SingleElementMoved |     7,486.2 ns |    282.99 ns |    148.01 ns |  0.07 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **Sorted**             |   **106,436.4 ns** |    **390.90 ns** |    **173.56 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Sorted             |   131,601.1 ns |    911.90 ns |    476.94 ns |  1.24 |    0.00 |    9 |         - |          NA |
| BottomupMergeSort        | 8192 | Sorted             |    39,326.4 ns |    194.01 ns |     86.14 ns |  0.37 |    0.00 |    6 |         - |          NA |
| StdStableSort            | 8192 | Sorted             |    22,933.7 ns |    102.01 ns |     45.30 ns |  0.22 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 8192 | Sorted             |     7,498.1 ns |    231.25 ns |    102.68 ns |  0.07 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Sorted             |    12,470.1 ns |    286.32 ns |    127.13 ns |  0.12 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 8192 | Sorted             |     7,436.8 ns |    253.83 ns |    112.70 ns |  0.07 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 8192 | Sorted             |    66,267.4 ns |    264.33 ns |    138.25 ns |  0.62 |    0.00 |    7 |         - |          NA |
| NaturalMergeSort         | 8192 | Sorted             |     3,625.7 ns |     27.65 ns |      9.86 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 8192 | Sorted             |   621,004.5 ns | 32,087.43 ns | 14,247.02 ns |  5.83 |    0.13 |   10 |         - |          NA |
| DropMergeSort            | 8192 | Sorted             |    13,621.4 ns |    339.94 ns |    121.23 ns |  0.13 |    0.00 |    4 |         - |          NA |
| TimSort                  | 8192 | Sorted             |     3,569.4 ns |     36.09 ns |     16.02 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Sorted             |     3,751.2 ns |    496.86 ns |    259.87 ns |  0.04 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Sorted             |     4,528.8 ns |      9.55 ns |      3.41 ns |  0.04 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | Sorted             |     3,109.1 ns |     27.06 ns |      9.65 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Sorted             |     3,023.9 ns |     64.05 ns |     28.44 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Sorted             |     3,594.5 ns |     33.96 ns |     12.11 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Sorted             |     3,381.1 ns |    462.55 ns |    241.92 ns |  0.03 |    0.00 |    1 |         - |          NA |
| QuadSort                 | 8192 | Sorted             |     4,664.3 ns |     43.37 ns |     15.47 ns |  0.04 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **Reversed**           |   **241,981.0 ns** |  **1,323.02 ns** |    **691.96 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Reversed           |   211,133.6 ns |    835.36 ns |    370.90 ns |  0.87 |    0.00 |    6 |         - |          NA |
| BottomupMergeSort        | 8192 | Reversed           |   130,318.2 ns |  1,197.21 ns |    531.57 ns |  0.54 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 8192 | Reversed           |   206,791.2 ns |  2,280.09 ns |  1,192.53 ns |  0.85 |    0.01 |    6 |         - |          NA |
| RotateMergeSort          | 8192 | Reversed           |   199,431.9 ns |  1,504.33 ns |    667.93 ns |  0.82 |    0.00 |    6 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Reversed           |   259,797.3 ns |  1,646.12 ns |    730.89 ns |  1.07 |    0.00 |    6 |         - |          NA |
| SymMergeSort             | 8192 | Reversed           |    66,560.0 ns |    313.06 ns |    163.74 ns |  0.28 |    0.00 |    4 |         - |          NA |
| BlockMergeSort           | 8192 | Reversed           |   108,683.2 ns |    692.17 ns |    307.33 ns |  0.45 |    0.00 |    5 |         - |          NA |
| NaturalMergeSort         | 8192 | Reversed           |     5,639.6 ns |    297.12 ns |    131.92 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 8192 | Reversed           |    33,298.9 ns |    176.81 ns |     78.51 ns |  0.14 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 8192 | Reversed           |    50,463.3 ns |    689.76 ns |    306.26 ns |  0.21 |    0.00 |    3 |         - |          NA |
| TimSort                  | 8192 | Reversed           |     5,654.2 ns |    263.50 ns |    137.82 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Reversed           |     5,663.4 ns |    316.18 ns |    165.37 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Reversed           |   143,214.2 ns |    738.90 ns |    328.08 ns |  0.59 |    0.00 |    5 |         - |          NA |
| SpinSort                 | 8192 | Reversed           |     5,930.2 ns |    144.07 ns |     63.97 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Reversed           |     5,552.1 ns |     12.83 ns |      4.58 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Reversed           |     5,812.3 ns |    476.13 ns |    249.03 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Reversed           |     6,106.9 ns |    298.95 ns |    156.35 ns |  0.03 |    0.00 |    1 |         - |          NA |
| QuadSort                 | 8192 | Reversed           |     6,034.9 ns |    217.73 ns |    113.87 ns |  0.02 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **PipeOrgan**          |   **176,731.6 ns** |    **288.65 ns** |    **128.16 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | PipeOrgan          |   173,331.8 ns |    374.21 ns |    166.15 ns |  0.98 |    0.00 |    6 |         - |          NA |
| BottomupMergeSort        | 8192 | PipeOrgan          |    87,722.7 ns |    631.83 ns |    280.54 ns |  0.50 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 8192 | PipeOrgan          |   118,262.0 ns |  1,319.45 ns |    690.10 ns |  0.67 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 8192 | PipeOrgan          |   184,991.1 ns |    503.77 ns |    223.68 ns |  1.05 |    0.00 |    6 |         - |          NA |
| RotateMergeSortRecursive | 8192 | PipeOrgan          |   227,313.5 ns | 14,088.26 ns |  6,255.27 ns |  1.29 |    0.03 |    7 |         - |          NA |
| SymMergeSort             | 8192 | PipeOrgan          |    98,868.4 ns |  1,569.60 ns |    696.91 ns |  0.56 |    0.00 |    5 |         - |          NA |
| BlockMergeSort           | 8192 | PipeOrgan          |   100,420.3 ns |    591.31 ns |    262.54 ns |  0.57 |    0.00 |    5 |         - |          NA |
| NaturalMergeSort         | 8192 | PipeOrgan          |    10,565.0 ns |    321.64 ns |    142.81 ns |  0.06 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 8192 | PipeOrgan          |   488,018.4 ns |  1,917.03 ns |  1,002.64 ns |  2.76 |    0.01 |    8 |         - |          NA |
| DropMergeSort            | 8192 | PipeOrgan          |   180,987.5 ns |    502.85 ns |    223.27 ns |  1.02 |    0.00 |    6 |         - |          NA |
| TimSort                  | 8192 | PipeOrgan          |    16,217.7 ns |    131.55 ns |     58.41 ns |  0.09 |    0.00 |    2 |         - |          NA |
| PowerSort                | 8192 | PipeOrgan          |    10,376.2 ns |    243.90 ns |    127.56 ns |  0.06 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | PipeOrgan          |    81,791.1 ns |    320.16 ns |    167.45 ns |  0.46 |    0.00 |    5 |         - |          NA |
| SpinSort                 | 8192 | PipeOrgan          |    13,872.5 ns |    375.88 ns |    196.59 ns |  0.08 |    0.00 |    2 |         - |          NA |
| Glidesort                | 8192 | PipeOrgan          |    29,326.2 ns |    475.48 ns |    248.69 ns |  0.17 |    0.00 |    3 |         - |          NA |
| Driftsort                | 8192 | PipeOrgan          |     8,728.7 ns |     34.29 ns |     15.23 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | PipeOrgan          |    51,296.5 ns |    535.85 ns |    280.26 ns |  0.29 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 8192 | PipeOrgan          |    18,701.5 ns |     74.79 ns |     26.67 ns |  0.11 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **ManyDuplicates**     |   **437,255.3 ns** |  **1,359.55 ns** |    **603.65 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | ManyDuplicates     |   409,496.0 ns |    591.44 ns |    262.60 ns |  0.94 |    0.00 |    4 |         - |          NA |
| BottomupMergeSort        | 8192 | ManyDuplicates     |   308,094.3 ns |    886.05 ns |    393.41 ns |  0.70 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 8192 | ManyDuplicates     |   220,699.4 ns |  4,725.27 ns |  2,471.41 ns |  0.50 |    0.01 |    4 |         - |          NA |
| RotateMergeSort          | 8192 | ManyDuplicates     |   886,112.6 ns |  3,004.42 ns |  1,571.37 ns |  2.03 |    0.00 |    6 |         - |          NA |
| RotateMergeSortRecursive | 8192 | ManyDuplicates     | 1,042,666.4 ns |  2,117.30 ns |  1,107.39 ns |  2.38 |    0.00 |    6 |         - |          NA |
| SymMergeSort             | 8192 | ManyDuplicates     |   602,036.3 ns |  1,586.96 ns |    830.01 ns |  1.38 |    0.00 |    5 |         - |          NA |
| BlockMergeSort           | 8192 | ManyDuplicates     |   438,929.2 ns |  2,134.26 ns |  1,116.26 ns |  1.00 |    0.00 |    4 |         - |          NA |
| NaturalMergeSort         | 8192 | ManyDuplicates     |   319,221.4 ns |    738.45 ns |    327.87 ns |  0.73 |    0.00 |    4 |         - |          NA |
| PatienceSort             | 8192 | ManyDuplicates     |   280,300.6 ns |    480.61 ns |    251.37 ns |  0.64 |    0.00 |    4 |         - |          NA |
| DropMergeSort            | 8192 | ManyDuplicates     |   233,202.5 ns |  1,023.95 ns |    454.64 ns |  0.53 |    0.00 |    4 |         - |          NA |
| TimSort                  | 8192 | ManyDuplicates     |   370,840.5 ns |  1,556.88 ns |    691.26 ns |  0.85 |    0.00 |    4 |         - |          NA |
| PowerSort                | 8192 | ManyDuplicates     |   247,558.3 ns |  2,196.23 ns |  1,148.67 ns |  0.57 |    0.00 |    4 |         - |          NA |
| ShiftSort                | 8192 | ManyDuplicates     |   383,616.6 ns |  1,301.05 ns |    680.47 ns |  0.88 |    0.00 |    4 |         - |          NA |
| SpinSort                 | 8192 | ManyDuplicates     |   222,674.9 ns |  2,071.27 ns |  1,083.31 ns |  0.51 |    0.00 |    4 |         - |          NA |
| Glidesort                | 8192 | ManyDuplicates     |    34,907.8 ns |    249.17 ns |    130.32 ns |  0.08 |    0.00 |    2 |         - |          NA |
| Driftsort                | 8192 | ManyDuplicates     |    24,787.8 ns |    337.89 ns |    176.72 ns |  0.06 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | ManyDuplicates     |   247,323.1 ns |  1,976.66 ns |    877.65 ns |  0.57 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 8192 | ManyDuplicates     |   156,023.6 ns |    415.85 ns |    184.64 ns |  0.36 |    0.00 |    3 |         - |          NA |

### MergeSlowBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.46 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8
UnrollFactor=1  WarmupCount=2
```

| Method     | Size | Pattern            | Mean         | Error       | StdDev      | Ratio | Rank | Allocated | Alloc Ratio |
| ----------- |----- |------------------- |-------------:|------------:|------------:|------:|-----:|----------:|------------:|
| **StrandSort** | **256**  | **Random**             |   **9,229.3 ns** |   **320.88 ns** |   **167.83 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **256**  | **SingleElementMoved** |     **725.9 ns** |     **7.29 ns** |     **3.24 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **256**  | **Sorted**             |     **469.8 ns** |     **6.82 ns** |     **2.43 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **256**  | **Reversed**           |  **39,102.3 ns** | **1,877.13 ns** |   **669.40 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **256**  | **PipeOrgan**          |  **23,095.5 ns** |   **181.13 ns** |    **80.42 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **256**  | **ManyDuplicates**     |   **7,734.7 ns** |   **136.12 ns** |    **71.19 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **1024** | **Random**             |  **57,631.1 ns** |   **375.75 ns** |   **196.52 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **1024** | **SingleElementMoved** |   **2,208.0 ns** |   **144.73 ns** |    **64.26 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **1024** | **Sorted**             |   **1,390.5 ns** |    **10.55 ns** |     **4.68 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **1024** | **Reversed**           | **618,604.4 ns** | **3,766.39 ns** | **1,672.30 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **1024** | **PipeOrgan**          | **373,468.8 ns** | **2,729.42 ns** | **1,427.54 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **1024** | **ManyDuplicates**     |  **35,594.4 ns** |   **324.63 ns** |   **144.14 ns** |  **1.00** |    **1** |         **-** |          **NA** |

### NetworkBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.46 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8
UnrollFactor=1  WarmupCount=2
```

| Method                            | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ---------------------------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **BitonicSort**                       | **256**  | **Random**             |    **13,056.9 ns** |    **147.43 ns** |     **65.46 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 256  | Random             |     3,750.4 ns |    155.00 ns |     81.07 ns |  0.29 |    0.01 |    1 |         - |          NA |
| BitonicRecursiveSort              | 256  | Random             |    26,270.8 ns |    323.31 ns |    143.55 ns |  2.01 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 256  | Random             |    10,558.5 ns |    385.95 ns |    201.86 ns |  0.81 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 256  | Random             |     3,365.6 ns |     46.80 ns |     16.69 ns |  0.26 |    0.00 |    1 |         - |          NA |
| PairwiseSortingNetwork            | 256  | Random             |    12,102.9 ns |    273.17 ns |    121.29 ns |  0.93 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 256  | Random             |     3,412.5 ns |    277.38 ns |    145.08 ns |  0.26 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **256**  | **SingleElementMoved** |     **3,854.3 ns** |    **228.43 ns** |    **119.47 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicSortBranchless             | 256  | SingleElementMoved |     3,838.2 ns |    317.80 ns |    166.21 ns |  1.00 |    0.05 |    1 |         - |          NA |
| BitonicRecursiveSort              | 256  | SingleElementMoved |    18,192.4 ns |    123.76 ns |     54.95 ns |  4.72 |    0.13 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 256  | SingleElementMoved |     3,815.9 ns |    282.36 ns |    147.68 ns |  0.99 |    0.05 |    1 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 256  | SingleElementMoved |     3,357.9 ns |     11.03 ns |      3.93 ns |  0.87 |    0.02 |    1 |         - |          NA |
| PairwiseSortingNetwork            | 256  | SingleElementMoved |     3,062.5 ns |    404.15 ns |    211.38 ns |  0.80 |    0.06 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 256  | SingleElementMoved |     3,331.5 ns |     26.16 ns |     11.62 ns |  0.87 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **256**  | **Sorted**             |     **3,493.6 ns** |     **72.35 ns** |     **25.80 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 256  | Sorted             |     3,732.2 ns |     51.47 ns |     18.35 ns |  1.07 |    0.01 |    2 |         - |          NA |
| BitonicRecursiveSort              | 256  | Sorted             |    18,001.7 ns |    111.01 ns |     58.06 ns |  5.15 |    0.04 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 256  | Sorted             |     3,704.0 ns |     20.15 ns |      8.95 ns |  1.06 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 256  | Sorted             |     3,389.0 ns |    164.72 ns |     73.14 ns |  0.97 |    0.02 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 256  | Sorted             |     2,641.5 ns |     20.62 ns |      7.35 ns |  0.76 |    0.01 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 256  | Sorted             |     3,336.4 ns |     22.44 ns |      8.00 ns |  0.96 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **256**  | **Reversed**           |     **3,568.9 ns** |    **199.33 ns** |     **88.50 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicSortBranchless             | 256  | Reversed           |     3,730.8 ns |     23.41 ns |      8.35 ns |  1.05 |    0.02 |    1 |         - |          NA |
| BitonicRecursiveSort              | 256  | Reversed           |    17,978.3 ns |     42.54 ns |     18.89 ns |  5.04 |    0.11 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 256  | Reversed           |     3,428.5 ns |    188.20 ns |     83.56 ns |  0.96 |    0.03 |    1 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 256  | Reversed           |     3,393.5 ns |     21.59 ns |      7.70 ns |  0.95 |    0.02 |    1 |         - |          NA |
| PairwiseSortingNetwork            | 256  | Reversed           |     2,936.4 ns |    399.26 ns |    177.27 ns |  0.82 |    0.05 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 256  | Reversed           |     3,330.8 ns |     23.53 ns |      8.39 ns |  0.93 |    0.02 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **256**  | **PipeOrgan**          |     **3,750.7 ns** |    **291.84 ns** |    **129.58 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicSortBranchless             | 256  | PipeOrgan          |     3,750.8 ns |    143.77 ns |     75.19 ns |  1.00 |    0.04 |    1 |         - |          NA |
| BitonicRecursiveSort              | 256  | PipeOrgan          |    18,056.1 ns |    105.85 ns |     47.00 ns |  4.82 |    0.15 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 256  | PipeOrgan          |     3,630.3 ns |    488.13 ns |    255.30 ns |  0.97 |    0.07 |    1 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 256  | PipeOrgan          |     3,425.5 ns |    164.92 ns |     73.23 ns |  0.91 |    0.03 |    1 |         - |          NA |
| PairwiseSortingNetwork            | 256  | PipeOrgan          |     2,845.9 ns |    281.33 ns |    124.91 ns |  0.76 |    0.04 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 256  | PipeOrgan          |     3,433.6 ns |    290.02 ns |    151.69 ns |  0.92 |    0.05 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **256**  | **ManyDuplicates**     |    **11,508.0 ns** |    **331.76 ns** |    **147.30 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 256  | ManyDuplicates     |     3,747.4 ns |    196.45 ns |     87.23 ns |  0.33 |    0.01 |    1 |         - |          NA |
| BitonicRecursiveSort              | 256  | ManyDuplicates     |    25,643.1 ns |    323.69 ns |    169.30 ns |  2.23 |    0.03 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 256  | ManyDuplicates     |     9,973.4 ns |    318.15 ns |    166.40 ns |  0.87 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 256  | ManyDuplicates     |     3,411.0 ns |     82.13 ns |     36.47 ns |  0.30 |    0.00 |    1 |         - |          NA |
| PairwiseSortingNetwork            | 256  | ManyDuplicates     |    11,163.6 ns |    233.36 ns |    103.61 ns |  0.97 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 256  | ManyDuplicates     |     3,407.3 ns |    259.79 ns |    135.87 ns |  0.30 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **1024** | **Random**             |    **66,504.3 ns** |    **497.36 ns** |    **260.13 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 1024 | Random             |    22,743.1 ns |    191.15 ns |     99.97 ns |  0.34 |    0.00 |    1 |         - |          NA |
| BitonicRecursiveSort              | 1024 | Random             |   138,522.5 ns |  1,039.47 ns |    461.53 ns |  2.08 |    0.01 |    4 |         - |          NA |
| BatcherOddEvenMergeSort           | 1024 | Random             |    54,487.1 ns |    477.30 ns |    249.64 ns |  0.82 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 1024 | Random             |    20,633.7 ns |    170.48 ns |     75.69 ns |  0.31 |    0.00 |    1 |         - |          NA |
| PairwiseSortingNetwork            | 1024 | Random             |    66,654.5 ns |    489.14 ns |    255.83 ns |  1.00 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 1024 | Random             |    20,355.8 ns |     95.46 ns |     49.93 ns |  0.31 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **1024** | **SingleElementMoved** |    **21,500.8 ns** |    **555.27 ns** |    **246.54 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 1024 | SingleElementMoved |    22,682.6 ns |    355.00 ns |    185.67 ns |  1.06 |    0.01 |    2 |         - |          NA |
| BitonicRecursiveSort              | 1024 | SingleElementMoved |    93,846.2 ns |    387.94 ns |    172.25 ns |  4.37 |    0.05 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 1024 | SingleElementMoved |    22,532.3 ns |    603.20 ns |    215.11 ns |  1.05 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 1024 | SingleElementMoved |    20,514.0 ns |    106.57 ns |     55.74 ns |  0.95 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 1024 | SingleElementMoved |    16,237.4 ns |    185.79 ns |     66.26 ns |  0.76 |    0.01 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 1024 | SingleElementMoved |    20,376.9 ns |    147.13 ns |     76.95 ns |  0.95 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **1024** | **Sorted**             |    **21,012.1 ns** |    **331.85 ns** |    **173.56 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicSortBranchless             | 1024 | Sorted             |    22,699.0 ns |    389.07 ns |    172.75 ns |  1.08 |    0.01 |    1 |         - |          NA |
| BitonicRecursiveSort              | 1024 | Sorted             |    94,435.1 ns |    323.97 ns |    169.44 ns |  4.49 |    0.04 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 1024 | Sorted             |    23,117.4 ns |    194.75 ns |    101.86 ns |  1.10 |    0.01 |    1 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 1024 | Sorted             |    20,731.6 ns |    572.17 ns |    299.26 ns |  0.99 |    0.02 |    1 |         - |          NA |
| PairwiseSortingNetwork            | 1024 | Sorted             |    17,436.4 ns |  1,179.73 ns |    617.02 ns |  0.83 |    0.03 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 1024 | Sorted             |    20,451.5 ns |    106.40 ns |     47.24 ns |  0.97 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **1024** | **Reversed**           |    **21,359.8 ns** |    **449.81 ns** |    **199.72 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 1024 | Reversed           |    22,710.9 ns |    188.46 ns |     98.57 ns |  1.06 |    0.01 |    2 |         - |          NA |
| BitonicRecursiveSort              | 1024 | Reversed           |    93,450.7 ns |    389.65 ns |    173.01 ns |  4.38 |    0.04 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 1024 | Reversed           |    21,878.1 ns |    898.17 ns |    398.79 ns |  1.02 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 1024 | Reversed           |    20,680.8 ns |    189.20 ns |     98.96 ns |  0.97 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 1024 | Reversed           |    16,481.2 ns |    450.53 ns |    200.04 ns |  0.77 |    0.01 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 1024 | Reversed           |    20,431.2 ns |    144.73 ns |     64.26 ns |  0.96 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **1024** | **PipeOrgan**          |    **21,149.9 ns** |    **227.72 ns** |    **119.10 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 1024 | PipeOrgan          |    22,606.8 ns |    198.88 ns |     88.30 ns |  1.07 |    0.01 |    2 |         - |          NA |
| BitonicRecursiveSort              | 1024 | PipeOrgan          |    93,616.3 ns |    412.03 ns |    182.94 ns |  4.43 |    0.02 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 1024 | PipeOrgan          |    21,663.2 ns |    910.06 ns |    475.98 ns |  1.02 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 1024 | PipeOrgan          |    20,728.5 ns |    279.16 ns |    146.01 ns |  0.98 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 1024 | PipeOrgan          |    16,766.7 ns |  1,140.11 ns |    596.30 ns |  0.79 |    0.03 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 1024 | PipeOrgan          |    20,446.7 ns |    101.07 ns |     52.86 ns |  0.97 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **1024** | **ManyDuplicates**     |    **55,003.6 ns** |    **412.69 ns** |    **183.24 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 1024 | ManyDuplicates     |    22,571.4 ns |    285.96 ns |    149.56 ns |  0.41 |    0.00 |    1 |         - |          NA |
| BitonicRecursiveSort              | 1024 | ManyDuplicates     |   128,397.0 ns |    620.21 ns |    275.38 ns |  2.33 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 1024 | ManyDuplicates     |    48,447.4 ns |    553.24 ns |    289.35 ns |  0.88 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 1024 | ManyDuplicates     |    20,562.0 ns |    169.30 ns |     88.55 ns |  0.37 |    0.00 |    1 |         - |          NA |
| PairwiseSortingNetwork            | 1024 | ManyDuplicates     |    54,554.2 ns |    366.54 ns |    191.71 ns |  0.99 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 1024 | ManyDuplicates     |    20,419.5 ns |     98.82 ns |     51.68 ns |  0.37 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **4096** | **Random**             |   **330,761.6 ns** |  **1,312.73 ns** |    **686.58 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 4096 | Random             |   127,025.7 ns |    690.51 ns |    306.59 ns |  0.38 |    0.00 |    1 |         - |          NA |
| BitonicRecursiveSort              | 4096 | Random             |   692,325.6 ns |  2,244.07 ns |  1,173.69 ns |  2.09 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 4096 | Random             |   276,799.8 ns |    650.58 ns |    288.86 ns |  0.84 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 4096 | Random             |   116,604.9 ns |  2,648.58 ns |  1,385.26 ns |  0.35 |    0.00 |    1 |         - |          NA |
| PairwiseSortingNetwork            | 4096 | Random             |   349,401.2 ns |  1,311.52 ns |    685.95 ns |  1.06 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 4096 | Random             |   115,156.6 ns |    512.65 ns |    227.62 ns |  0.35 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **4096** | **SingleElementMoved** |   **117,482.0 ns** |  **1,201.87 ns** |    **533.64 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicSortBranchless             | 4096 | SingleElementMoved |   126,586.5 ns |    673.04 ns |    298.83 ns |  1.08 |    0.01 |    1 |         - |          NA |
| BitonicRecursiveSort              | 4096 | SingleElementMoved |   467,286.2 ns |  4,158.64 ns |  2,175.05 ns |  3.98 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 4096 | SingleElementMoved |   128,139.2 ns |  1,867.82 ns |    976.91 ns |  1.09 |    0.01 |    1 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 4096 | SingleElementMoved |   116,072.7 ns |    435.39 ns |    155.27 ns |  0.99 |    0.00 |    1 |         - |          NA |
| PairwiseSortingNetwork            | 4096 | SingleElementMoved |    96,622.4 ns |  3,054.95 ns |  1,597.80 ns |  0.82 |    0.01 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 4096 | SingleElementMoved |   114,991.5 ns |    334.54 ns |    148.54 ns |  0.98 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **4096** | **Sorted**             |   **116,448.1 ns** |  **1,609.54 ns** |    **714.65 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 4096 | Sorted             |   126,916.0 ns |    773.77 ns |    343.56 ns |  1.09 |    0.01 |    2 |         - |          NA |
| BitonicRecursiveSort              | 4096 | Sorted             |   465,457.9 ns |  1,435.90 ns |    512.05 ns |  4.00 |    0.02 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 4096 | Sorted             |   129,425.6 ns |    918.67 ns |    407.89 ns |  1.11 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 4096 | Sorted             |   116,314.9 ns |  1,085.55 ns |    481.99 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 4096 | Sorted             |    94,991.2 ns |  4,263.58 ns |  1,893.06 ns |  0.82 |    0.02 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 4096 | Sorted             |   115,025.7 ns |    300.14 ns |    133.26 ns |  0.99 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **4096** | **Reversed**           |   **119,397.1 ns** |  **1,836.67 ns** |    **815.49 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicSortBranchless             | 4096 | Reversed           |   126,845.2 ns |  1,644.29 ns |    730.07 ns |  1.06 |    0.01 |    1 |         - |          NA |
| BitonicRecursiveSort              | 4096 | Reversed           |   463,099.8 ns |  1,241.73 ns |    551.34 ns |  3.88 |    0.03 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 4096 | Reversed           |   125,478.4 ns |  2,988.95 ns |  1,563.28 ns |  1.05 |    0.01 |    1 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 4096 | Reversed           |   116,032.5 ns |  1,431.47 ns |    748.69 ns |  0.97 |    0.01 |    1 |         - |          NA |
| PairwiseSortingNetwork            | 4096 | Reversed           |    97,201.1 ns |  3,521.66 ns |  1,841.90 ns |  0.81 |    0.02 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 4096 | Reversed           |   115,003.3 ns |  1,235.49 ns |    646.18 ns |  0.96 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **4096** | **PipeOrgan**          |   **118,294.1 ns** |    **560.75 ns** |    **248.98 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicSortBranchless             | 4096 | PipeOrgan          |   128,270.1 ns |  7,613.93 ns |  3,380.63 ns |  1.08 |    0.03 |    1 |         - |          NA |
| BitonicRecursiveSort              | 4096 | PipeOrgan          |   465,508.2 ns |  2,569.60 ns |  1,343.95 ns |  3.94 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 4096 | PipeOrgan          |   124,995.5 ns |  6,233.74 ns |  2,767.82 ns |  1.06 |    0.02 |    1 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 4096 | PipeOrgan          |   116,486.8 ns |  1,003.25 ns |    445.45 ns |  0.98 |    0.00 |    1 |         - |          NA |
| PairwiseSortingNetwork            | 4096 | PipeOrgan          |    96,173.3 ns |  4,147.16 ns |  2,169.04 ns |  0.81 |    0.02 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 4096 | PipeOrgan          |   114,969.0 ns |    516.62 ns |    229.38 ns |  0.97 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **4096** | **ManyDuplicates**     |   **253,789.9 ns** |    **739.56 ns** |    **386.80 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 4096 | ManyDuplicates     |   126,612.2 ns |    682.04 ns |    302.83 ns |  0.50 |    0.00 |    1 |         - |          NA |
| BitonicRecursiveSort              | 4096 | ManyDuplicates     |   614,478.5 ns |  1,392.29 ns |    618.18 ns |  2.42 |    0.00 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 4096 | ManyDuplicates     |   232,314.3 ns |  1,663.32 ns |    869.95 ns |  0.92 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 4096 | ManyDuplicates     |   116,113.7 ns |  1,049.09 ns |    548.69 ns |  0.46 |    0.00 |    1 |         - |          NA |
| PairwiseSortingNetwork            | 4096 | ManyDuplicates     |   265,669.4 ns | 65,401.32 ns | 29,038.59 ns |  1.05 |    0.11 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 4096 | ManyDuplicates     |   114,978.2 ns |    930.96 ns |    413.35 ns |  0.45 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **8192** | **Random**             |   **732,457.8 ns** |  **3,641.42 ns** |  **1,616.81 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 8192 | Random             |   294,160.8 ns |    523.67 ns |    273.89 ns |  0.40 |    0.00 |    1 |         - |          NA |
| BitonicRecursiveSort              | 8192 | Random             | 1,550,870.6 ns |  3,073.37 ns |  1,607.43 ns |  2.12 |    0.00 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 8192 | Random             |   614,615.1 ns |  2,339.13 ns |  1,223.41 ns |  0.84 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 8192 | Random             |   269,672.7 ns |  3,430.46 ns |  1,523.15 ns |  0.37 |    0.00 |    1 |         - |          NA |
| PairwiseSortingNetwork            | 8192 | Random             |   791,333.9 ns |  1,831.12 ns |    957.71 ns |  1.08 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 8192 | Random             |   268,358.5 ns |  3,225.28 ns |  1,432.05 ns |  0.37 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **8192** | **SingleElementMoved** |   **269,184.1 ns** |  **1,122.89 ns** |    **400.43 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 8192 | SingleElementMoved |   294,170.6 ns |  1,400.04 ns |    621.63 ns |  1.09 |    0.00 |    2 |         - |          NA |
| BitonicRecursiveSort              | 8192 | SingleElementMoved | 1,049,285.5 ns |  1,713.86 ns |    896.38 ns |  3.90 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 8192 | SingleElementMoved |   297,483.2 ns |  4,870.71 ns |  2,162.63 ns |  1.11 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 8192 | SingleElementMoved |   270,566.2 ns |  3,200.49 ns |  1,421.04 ns |  1.01 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 8192 | SingleElementMoved |   222,714.2 ns |  5,250.75 ns |  2,746.25 ns |  0.83 |    0.01 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 8192 | SingleElementMoved |   268,427.2 ns |  1,423.16 ns |    744.34 ns |  1.00 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **8192** | **Sorted**             |   **269,314.1 ns** |    **719.08 ns** |    **319.28 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 8192 | Sorted             |   293,863.0 ns |  1,696.55 ns |    887.33 ns |  1.09 |    0.00 |    2 |         - |          NA |
| BitonicRecursiveSort              | 8192 | Sorted             | 1,050,247.8 ns |  2,313.84 ns |  1,210.18 ns |  3.90 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 8192 | Sorted             |   302,441.3 ns |  2,953.67 ns |  1,544.83 ns |  1.12 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 8192 | Sorted             |   270,849.3 ns |  1,876.72 ns |    981.56 ns |  1.01 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 8192 | Sorted             |   221,459.2 ns |  4,114.01 ns |  2,151.71 ns |  0.82 |    0.01 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 8192 | Sorted             |   267,876.5 ns |  1,951.27 ns |    866.38 ns |  0.99 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **8192** | **Reversed**           |   **276,037.7 ns** |  **3,535.60 ns** |  **1,569.83 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| BitonicSortBranchless             | 8192 | Reversed           |   294,023.6 ns |  1,044.54 ns |    463.78 ns |  1.07 |    0.01 |    1 |         - |          NA |
| BitonicRecursiveSort              | 8192 | Reversed           | 1,044,328.5 ns |  3,248.08 ns |  1,442.17 ns |  3.78 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSort           | 8192 | Reversed           |   286,873.4 ns |  4,599.94 ns |  2,405.86 ns |  1.04 |    0.01 |    1 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 8192 | Reversed           |   271,514.3 ns |  5,014.04 ns |  2,226.26 ns |  0.98 |    0.01 |    1 |         - |          NA |
| PairwiseSortingNetwork            | 8192 | Reversed           |   226,200.2 ns |  4,189.78 ns |  2,191.34 ns |  0.82 |    0.01 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 8192 | Reversed           |   268,000.4 ns |  2,109.26 ns |    936.53 ns |  0.97 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **8192** | **PipeOrgan**          |   **272,660.1 ns** |  **1,282.57 ns** |    **569.47 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 8192 | PipeOrgan          |   293,755.1 ns |    650.05 ns |    231.81 ns |  1.08 |    0.00 |    2 |         - |          NA |
| BitonicRecursiveSort              | 8192 | PipeOrgan          | 1,047,734.1 ns |  3,201.34 ns |  1,421.42 ns |  3.84 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 8192 | PipeOrgan          |   291,112.3 ns |  2,861.24 ns |  1,496.49 ns |  1.07 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 8192 | PipeOrgan          |   271,297.2 ns |  3,339.38 ns |  1,746.56 ns |  1.00 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 8192 | PipeOrgan          |   221,773.8 ns |  3,672.84 ns |  1,630.77 ns |  0.81 |    0.01 |    1 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 8192 | PipeOrgan          |   267,820.1 ns |  1,929.69 ns |  1,009.27 ns |  0.98 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **BitonicSort**                       | **8192** | **ManyDuplicates**     |   **542,889.9 ns** |    **643.57 ns** |    **285.75 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 8192 | ManyDuplicates     |   294,005.1 ns |  2,278.79 ns |  1,011.80 ns |  0.54 |    0.00 |    1 |         - |          NA |
| BitonicRecursiveSort              | 8192 | ManyDuplicates     | 1,348,163.0 ns |  2,696.06 ns |  1,197.07 ns |  2.48 |    0.00 |    3 |         - |          NA |
| BatcherOddEvenMergeSort           | 8192 | ManyDuplicates     |   503,273.6 ns |  2,484.45 ns |  1,299.42 ns |  0.93 |    0.00 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 8192 | ManyDuplicates     |   271,836.1 ns |  2,262.69 ns |  1,183.43 ns |  0.50 |    0.00 |    1 |         - |          NA |
| PairwiseSortingNetwork            | 8192 | ManyDuplicates     |   539,872.2 ns |  3,445.35 ns |  1,801.98 ns |  0.99 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 8192 | ManyDuplicates     |   267,440.8 ns |    259.54 ns |     92.55 ns |  0.49 |    0.00 |    1 |         - |          NA |

### PartitionBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.46 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8
UnrollFactor=1  WarmupCount=2
```

| Method                       | Size | Pattern            | Mean           | Error        | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------------- |----- |------------------- |---------------:|-------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **QuickSort**                    | **256**  | **Random**             |     **8,617.8 ns** |    **230.08 ns** |   **120.34 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 256  | Random             |     9,342.0 ns |    266.67 ns |   118.40 ns |  1.08 |    0.02 |    3 |         - |          NA |
| QuickSortMedian3             | 256  | Random             |     9,129.1 ns |    160.45 ns |    71.24 ns |  1.06 |    0.02 |    3 |         - |          NA |
| QuickSortMedian9             | 256  | Random             |     9,673.3 ns |    399.43 ns |   208.91 ns |  1.12 |    0.03 |    3 |         - |          NA |
| SampleSort                   | 256  | Random             |    11,030.8 ns |    280.23 ns |   146.57 ns |  1.28 |    0.02 |    3 |         - |          NA |
| DualPivotQuickSort           | 256  | Random             |     6,016.3 ns |    305.33 ns |   159.69 ns |  0.70 |    0.02 |    2 |         - |          NA |
| StableQuickSort              | 256  | Random             |    19,719.0 ns |    219.86 ns |   114.99 ns |  2.29 |    0.03 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Random             |    11,481.0 ns |    300.03 ns |   156.92 ns |  1.33 |    0.02 |    3 |         - |          NA |
| DestswapStableQuickSort      | 256  | Random             |    19,236.1 ns |    326.58 ns |   170.81 ns |  2.23 |    0.04 |    4 |         - |          NA |
| IntroSort                    | 256  | Random             |     7,169.6 ns |    149.24 ns |    78.05 ns |  0.83 |    0.01 |    2 |         - |          NA |
| IntroSortDotnet              | 256  | Random             |     6,335.6 ns |     92.80 ns |    41.20 ns |  0.74 |    0.01 |    2 |         - |          NA |
| PDQSort                      | 256  | Random             |     6,076.9 ns |    190.65 ns |    84.65 ns |  0.71 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 256  | Random             |     4,386.8 ns |    294.18 ns |   153.86 ns |  0.51 |    0.02 |    1 |         - |          NA |
| Ipnsort                      | 256  | Random             |     3,780.0 ns |    340.71 ns |   178.20 ns |  0.44 |    0.02 |    1 |         - |          NA |
| StdSort                      | 256  | Random             |     4,407.2 ns |    255.14 ns |   133.44 ns |  0.51 |    0.02 |    1 |         - |          NA |
| BlockQuickSort               | 256  | Random             |     4,761.6 ns |    409.11 ns |   213.97 ns |  0.55 |    0.02 |    1 |         - |          NA |
| FluxSort                     | 256  | Random             |     6,091.6 ns |    327.91 ns |   171.50 ns |  0.71 |    0.02 |    2 |         - |          NA |
| DotnetSort                   | 256  | Random             |     5,967.0 ns |    346.99 ns |   181.48 ns |  0.69 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **256**  | **SingleElementMoved** |     **1,135.3 ns** |    **155.84 ns** |    **69.20 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 256  | SingleElementMoved |     2,306.6 ns |    271.31 ns |   141.90 ns |  2.04 |    0.16 |    2 |         - |          NA |
| QuickSortMedian3             | 256  | SingleElementMoved |     1,748.3 ns |    168.65 ns |    74.88 ns |  1.54 |    0.10 |    2 |         - |          NA |
| QuickSortMedian9             | 256  | SingleElementMoved |     1,747.6 ns |    186.23 ns |    97.40 ns |  1.54 |    0.12 |    2 |         - |          NA |
| SampleSort                   | 256  | SingleElementMoved |     5,367.9 ns |    496.20 ns |   220.32 ns |  4.74 |    0.31 |    5 |         - |          NA |
| DualPivotQuickSort           | 256  | SingleElementMoved |       841.6 ns |     41.13 ns |    21.51 ns |  0.74 |    0.04 |    1 |         - |          NA |
| StableQuickSort              | 256  | SingleElementMoved |     6,301.4 ns |    419.05 ns |   219.17 ns |  5.57 |    0.35 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 256  | SingleElementMoved |     3,621.6 ns |     79.45 ns |    28.33 ns |  3.20 |    0.17 |    4 |         - |          NA |
| DestswapStableQuickSort      | 256  | SingleElementMoved |     8,917.3 ns |    357.48 ns |   186.97 ns |  7.88 |    0.45 |    6 |         - |          NA |
| IntroSort                    | 256  | SingleElementMoved |     2,115.0 ns |     84.33 ns |    37.44 ns |  1.87 |    0.11 |    2 |         - |          NA |
| IntroSortDotnet              | 256  | SingleElementMoved |     1,103.6 ns |    148.75 ns |    77.80 ns |  0.98 |    0.08 |    1 |         - |          NA |
| PDQSort                      | 256  | SingleElementMoved |       868.4 ns |     54.30 ns |    24.11 ns |  0.77 |    0.05 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | SingleElementMoved |     1,061.2 ns |    139.47 ns |    72.94 ns |  0.94 |    0.08 |    1 |         - |          NA |
| Ipnsort                      | 256  | SingleElementMoved |     2,856.0 ns |     67.80 ns |    30.11 ns |  2.52 |    0.14 |    3 |         - |          NA |
| StdSort                      | 256  | SingleElementMoved |       987.6 ns |     49.57 ns |    22.01 ns |  0.87 |    0.05 |    1 |         - |          NA |
| BlockQuickSort               | 256  | SingleElementMoved |     1,651.8 ns |    251.22 ns |   131.39 ns |  1.46 |    0.14 |    2 |         - |          NA |
| FluxSort                     | 256  | SingleElementMoved |     1,241.4 ns |    133.30 ns |    59.18 ns |  1.10 |    0.08 |    1 |         - |          NA |
| DotnetSort                   | 256  | SingleElementMoved |     1,059.8 ns |     83.17 ns |    36.93 ns |  0.94 |    0.06 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Sorted**             |       **710.8 ns** |     **18.68 ns** |     **9.77 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 256  | Sorted             |     1,412.9 ns |     27.78 ns |     9.91 ns |  1.99 |    0.03 |    5 |         - |          NA |
| QuickSortMedian3             | 256  | Sorted             |       924.5 ns |     21.51 ns |     9.55 ns |  1.30 |    0.02 |    4 |         - |          NA |
| QuickSortMedian9             | 256  | Sorted             |     1,016.9 ns |     29.61 ns |    13.15 ns |  1.43 |    0.03 |    4 |         - |          NA |
| SampleSort                   | 256  | Sorted             |     3,673.3 ns |    410.98 ns |   214.95 ns |  5.17 |    0.29 |    6 |         - |          NA |
| DualPivotQuickSort           | 256  | Sorted             |       610.8 ns |     21.63 ns |     9.60 ns |  0.86 |    0.02 |    4 |         - |          NA |
| StableQuickSort              | 256  | Sorted             |     5,882.0 ns |     44.93 ns |    19.95 ns |  8.28 |    0.11 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Sorted             |     3,432.6 ns |     34.28 ns |    12.22 ns |  4.83 |    0.06 |    6 |         - |          NA |
| DestswapStableQuickSort      | 256  | Sorted             |     7,426.2 ns |    115.20 ns |    51.15 ns | 10.45 |    0.15 |    8 |         - |          NA |
| IntroSort                    | 256  | Sorted             |       626.8 ns |      5.68 ns |     2.52 ns |  0.88 |    0.01 |    4 |         - |          NA |
| IntroSortDotnet              | 256  | Sorted             |       775.6 ns |     10.57 ns |     5.53 ns |  1.09 |    0.02 |    4 |         - |          NA |
| PDQSort                      | 256  | Sorted             |       334.3 ns |      5.83 ns |     3.05 ns |  0.47 |    0.01 |    3 |         - |          NA |
| PDQSortBranchless            | 256  | Sorted             |       322.4 ns |     11.77 ns |     5.23 ns |  0.45 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 256  | Sorted             |       149.8 ns |     25.72 ns |    13.45 ns |  0.21 |    0.02 |    1 |         - |          NA |
| StdSort                      | 256  | Sorted             |       360.0 ns |     16.98 ns |     7.54 ns |  0.51 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 256  | Sorted             |     1,397.7 ns |     18.27 ns |     6.51 ns |  1.97 |    0.03 |    5 |         - |          NA |
| FluxSort                     | 256  | Sorted             |       223.6 ns |     20.88 ns |    10.92 ns |  0.31 |    0.02 |    2 |         - |          NA |
| DotnetSort                   | 256  | Sorted             |       651.1 ns |     21.47 ns |     7.65 ns |  0.92 |    0.02 |    4 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Reversed**           |       **820.3 ns** |     **20.32 ns** |     **9.02 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 256  | Reversed           |     1,577.6 ns |     20.10 ns |     8.93 ns |  1.92 |    0.02 |    6 |         - |          NA |
| QuickSortMedian3             | 256  | Reversed           |       996.2 ns |     14.57 ns |     6.47 ns |  1.21 |    0.01 |    5 |         - |          NA |
| QuickSortMedian9             | 256  | Reversed           |     1,141.0 ns |     35.98 ns |    18.82 ns |  1.39 |    0.03 |    5 |         - |          NA |
| SampleSort                   | 256  | Reversed           |     3,943.5 ns |    433.54 ns |   226.75 ns |  4.81 |    0.27 |    7 |         - |          NA |
| DualPivotQuickSort           | 256  | Reversed           |       744.5 ns |      3.69 ns |     1.32 ns |  0.91 |    0.01 |    4 |         - |          NA |
| StableQuickSort              | 256  | Reversed           |     6,227.9 ns |    409.74 ns |   214.30 ns |  7.59 |    0.26 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Reversed           |     3,443.0 ns |     21.49 ns |     7.66 ns |  4.20 |    0.04 |    7 |         - |          NA |
| DestswapStableQuickSort      | 256  | Reversed           |     6,728.6 ns |    314.15 ns |   164.31 ns |  8.20 |    0.21 |    8 |         - |          NA |
| IntroSort                    | 256  | Reversed           |       662.1 ns |      4.63 ns |     2.42 ns |  0.81 |    0.01 |    4 |         - |          NA |
| IntroSortDotnet              | 256  | Reversed           |     1,091.6 ns |     15.03 ns |     6.67 ns |  1.33 |    0.02 |    5 |         - |          NA |
| PDQSort                      | 256  | Reversed           |       453.4 ns |     14.21 ns |     7.43 ns |  0.55 |    0.01 |    3 |         - |          NA |
| PDQSortBranchless            | 256  | Reversed           |       598.3 ns |     19.18 ns |    10.03 ns |  0.73 |    0.01 |    4 |         - |          NA |
| Ipnsort                      | 256  | Reversed           |       210.7 ns |     20.36 ns |     9.04 ns |  0.26 |    0.01 |    1 |         - |          NA |
| StdSort                      | 256  | Reversed           |       661.4 ns |     11.58 ns |     5.14 ns |  0.81 |    0.01 |    4 |         - |          NA |
| BlockQuickSort               | 256  | Reversed           |     1,692.9 ns |     14.56 ns |     5.19 ns |  2.06 |    0.02 |    6 |         - |          NA |
| FluxSort                     | 256  | Reversed           |       307.5 ns |     17.58 ns |     9.20 ns |  0.37 |    0.01 |    2 |         - |          NA |
| DotnetSort                   | 256  | Reversed           |     1,143.3 ns |     37.43 ns |    16.62 ns |  1.39 |    0.02 |    5 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **256**  | **PipeOrgan**          |     **5,837.5 ns** |    **192.51 ns** |   **100.68 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 256  | PipeOrgan          |     2,840.5 ns |    241.44 ns |   107.20 ns |  0.49 |    0.02 |    3 |         - |          NA |
| QuickSortMedian3             | 256  | PipeOrgan          |     2,553.7 ns |    275.85 ns |   144.28 ns |  0.44 |    0.02 |    3 |         - |          NA |
| QuickSortMedian9             | 256  | PipeOrgan          |     1,969.2 ns |    149.98 ns |    66.59 ns |  0.34 |    0.01 |    2 |         - |          NA |
| SampleSort                   | 256  | PipeOrgan          |     3,868.8 ns |    421.34 ns |   220.37 ns |  0.66 |    0.04 |    3 |         - |          NA |
| DualPivotQuickSort           | 256  | PipeOrgan          |     1,581.5 ns |    146.34 ns |    64.98 ns |  0.27 |    0.01 |    2 |         - |          NA |
| StableQuickSort              | 256  | PipeOrgan          |     6,065.7 ns |    444.53 ns |   232.50 ns |  1.04 |    0.04 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 256  | PipeOrgan          |     3,436.3 ns |     37.26 ns |    16.54 ns |  0.59 |    0.01 |    3 |         - |          NA |
| DestswapStableQuickSort      | 256  | PipeOrgan          |     8,997.6 ns |    284.72 ns |   148.91 ns |  1.54 |    0.03 |    5 |         - |          NA |
| IntroSort                    | 256  | PipeOrgan          |     3,112.0 ns |     52.26 ns |    18.63 ns |  0.53 |    0.01 |    3 |         - |          NA |
| IntroSortDotnet              | 256  | PipeOrgan          |     1,659.1 ns |     61.23 ns |    27.19 ns |  0.28 |    0.01 |    2 |         - |          NA |
| PDQSort                      | 256  | PipeOrgan          |     1,233.1 ns |    130.90 ns |    58.12 ns |  0.21 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | PipeOrgan          |     2,025.1 ns |     41.14 ns |    14.67 ns |  0.35 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 256  | PipeOrgan          |     3,086.0 ns |     56.97 ns |    20.32 ns |  0.53 |    0.01 |    3 |         - |          NA |
| StdSort                      | 256  | PipeOrgan          |     3,731.7 ns |    237.85 ns |   105.61 ns |  0.64 |    0.02 |    3 |         - |          NA |
| BlockQuickSort               | 256  | PipeOrgan          |     3,633.1 ns |    198.00 ns |   103.56 ns |  0.62 |    0.02 |    3 |         - |          NA |
| FluxSort                     | 256  | PipeOrgan          |     3,009.8 ns |     47.63 ns |    16.99 ns |  0.52 |    0.01 |    3 |         - |          NA |
| DotnetSort                   | 256  | PipeOrgan          |     1,796.6 ns |    100.57 ns |    44.65 ns |  0.31 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **256**  | **ManyDuplicates**     |     **6,723.0 ns** |    **299.66 ns** |   **156.73 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 256  | ManyDuplicates     |     6,075.5 ns |    125.20 ns |    65.48 ns |  0.90 |    0.02 |    1 |         - |          NA |
| QuickSortMedian3             | 256  | ManyDuplicates     |     7,826.4 ns |    224.89 ns |   117.62 ns |  1.16 |    0.03 |    1 |         - |          NA |
| QuickSortMedian9             | 256  | ManyDuplicates     |     8,257.8 ns |    318.44 ns |   166.55 ns |  1.23 |    0.04 |    1 |         - |          NA |
| SampleSort                   | 256  | ManyDuplicates     |     9,755.8 ns |    440.49 ns |   230.38 ns |  1.45 |    0.05 |    1 |         - |          NA |
| DualPivotQuickSort           | 256  | ManyDuplicates     |     5,246.0 ns |    187.57 ns |    98.10 ns |  0.78 |    0.02 |    1 |         - |          NA |
| StableQuickSort              | 256  | ManyDuplicates     |    12,588.3 ns |    571.65 ns |   298.98 ns |  1.87 |    0.06 |    1 |         - |          NA |
| BidirectionalStableQuickSort | 256  | ManyDuplicates     |     6,374.0 ns |    163.90 ns |    85.73 ns |  0.95 |    0.02 |    1 |         - |          NA |
| DestswapStableQuickSort      | 256  | ManyDuplicates     |    10,929.7 ns |    311.60 ns |   138.35 ns |  1.63 |    0.04 |    1 |         - |          NA |
| IntroSort                    | 256  | ManyDuplicates     |     6,315.1 ns |    134.11 ns |    59.55 ns |  0.94 |    0.02 |    1 |         - |          NA |
| IntroSortDotnet              | 256  | ManyDuplicates     |     6,036.8 ns |    154.04 ns |    80.56 ns |  0.90 |    0.02 |    1 |         - |          NA |
| PDQSort                      | 256  | ManyDuplicates     |     5,594.9 ns |    310.43 ns |   162.36 ns |  0.83 |    0.03 |    1 |         - |          NA |
| PDQSortBranchless            | 256  | ManyDuplicates     |     3,829.0 ns |    287.69 ns |   150.47 ns |  0.57 |    0.02 |    1 |         - |          NA |
| Ipnsort                      | 256  | ManyDuplicates     |     3,726.0 ns |    332.20 ns |   173.75 ns |  0.55 |    0.03 |    1 |         - |          NA |
| StdSort                      | 256  | ManyDuplicates     |     4,184.9 ns |    458.97 ns |   240.05 ns |  0.62 |    0.04 |    1 |         - |          NA |
| BlockQuickSort               | 256  | ManyDuplicates     |     4,430.6 ns |    395.15 ns |   206.67 ns |  0.66 |    0.03 |    1 |         - |          NA |
| FluxSort                     | 256  | ManyDuplicates     |     6,079.2 ns |    383.22 ns |   200.43 ns |  0.90 |    0.03 |    1 |         - |          NA |
| DotnetSort                   | 256  | ManyDuplicates     |     5,570.5 ns |    427.13 ns |   223.40 ns |  0.83 |    0.04 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Random**             |    **40,873.1 ns** |    **649.65 ns** |   **339.78 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Random             |    44,014.9 ns |    933.12 ns |   488.04 ns |  1.08 |    0.01 |    1 |         - |          NA |
| QuickSortMedian3             | 1024 | Random             |    42,806.8 ns |    847.78 ns |   443.41 ns |  1.05 |    0.01 |    1 |         - |          NA |
| QuickSortMedian9             | 1024 | Random             |    44,940.3 ns |    767.60 ns |   401.47 ns |  1.10 |    0.01 |    1 |         - |          NA |
| SampleSort                   | 1024 | Random             |    51,211.3 ns |    434.86 ns |   193.08 ns |  1.25 |    0.01 |    1 |         - |          NA |
| DualPivotQuickSort           | 1024 | Random             |    29,790.0 ns |    247.15 ns |   129.27 ns |  0.73 |    0.01 |    1 |         - |          NA |
| StableQuickSort              | 1024 | Random             |    98,331.2 ns |    543.15 ns |   241.16 ns |  2.41 |    0.02 |    2 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Random             |    53,697.8 ns |    324.22 ns |   143.95 ns |  1.31 |    0.01 |    1 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Random             |    87,280.0 ns |    532.62 ns |   236.48 ns |  2.14 |    0.02 |    2 |         - |          NA |
| IntroSort                    | 1024 | Random             |    34,246.6 ns |    279.42 ns |   146.14 ns |  0.84 |    0.01 |    1 |         - |          NA |
| IntroSortDotnet              | 1024 | Random             |    32,417.1 ns |    173.77 ns |    90.89 ns |  0.79 |    0.01 |    1 |         - |          NA |
| PDQSort                      | 1024 | Random             |    30,670.9 ns |    148.61 ns |    65.98 ns |  0.75 |    0.01 |    1 |         - |          NA |
| PDQSortBranchless            | 1024 | Random             |    19,476.2 ns |     75.26 ns |    33.42 ns |  0.48 |    0.00 |    1 |         - |          NA |
| Ipnsort                      | 1024 | Random             |    18,994.3 ns |    252.91 ns |   112.29 ns |  0.46 |    0.00 |    1 |         - |          NA |
| StdSort                      | 1024 | Random             |    20,292.8 ns |    138.38 ns |    61.44 ns |  0.50 |    0.00 |    1 |         - |          NA |
| BlockQuickSort               | 1024 | Random             |    21,846.9 ns |    357.47 ns |   186.96 ns |  0.53 |    0.01 |    1 |         - |          NA |
| FluxSort                     | 1024 | Random             |    25,670.7 ns |    191.11 ns |    99.96 ns |  0.63 |    0.01 |    1 |         - |          NA |
| DotnetSort                   | 1024 | Random             |    29,776.4 ns |    508.08 ns |   265.74 ns |  0.73 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **SingleElementMoved** |     **4,374.3 ns** |    **125.41 ns** |    **65.59 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 1024 | SingleElementMoved |     8,124.3 ns |    450.41 ns |   199.98 ns |  1.86 |    0.05 |    3 |         - |          NA |
| QuickSortMedian3             | 1024 | SingleElementMoved |     6,530.4 ns |    346.56 ns |   181.26 ns |  1.49 |    0.04 |    3 |         - |          NA |
| QuickSortMedian9             | 1024 | SingleElementMoved |     7,397.6 ns |    693.04 ns |   362.47 ns |  1.69 |    0.08 |    3 |         - |          NA |
| SampleSort                   | 1024 | SingleElementMoved |    26,027.4 ns |    747.83 ns |   391.13 ns |  5.95 |    0.12 |    6 |         - |          NA |
| DualPivotQuickSort           | 1024 | SingleElementMoved |     3,822.2 ns |    360.50 ns |   188.55 ns |  0.87 |    0.04 |    2 |         - |          NA |
| StableQuickSort              | 1024 | SingleElementMoved |    30,720.9 ns |    421.00 ns |   186.93 ns |  7.02 |    0.11 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | SingleElementMoved |    16,042.2 ns |    135.01 ns |    59.94 ns |  3.67 |    0.05 |    5 |         - |          NA |
| DestswapStableQuickSort      | 1024 | SingleElementMoved |    32,633.0 ns |    240.19 ns |   125.62 ns |  7.46 |    0.11 |    6 |         - |          NA |
| IntroSort                    | 1024 | SingleElementMoved |    11,196.8 ns |    331.35 ns |   173.30 ns |  2.56 |    0.05 |    4 |         - |          NA |
| IntroSortDotnet              | 1024 | SingleElementMoved |     4,692.9 ns |    111.09 ns |    49.33 ns |  1.07 |    0.02 |    2 |         - |          NA |
| PDQSort                      | 1024 | SingleElementMoved |     3,175.2 ns |    199.56 ns |   104.37 ns |  0.73 |    0.02 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | SingleElementMoved |     3,664.2 ns |    118.54 ns |    52.63 ns |  0.84 |    0.02 |    2 |         - |          NA |
| Ipnsort                      | 1024 | SingleElementMoved |    14,607.5 ns |    154.27 ns |    68.50 ns |  3.34 |    0.05 |    5 |         - |          NA |
| StdSort                      | 1024 | SingleElementMoved |     3,440.2 ns |    232.76 ns |   121.74 ns |  0.79 |    0.03 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | SingleElementMoved |     7,301.0 ns |    246.07 ns |   109.26 ns |  1.67 |    0.03 |    3 |         - |          NA |
| FluxSort                     | 1024 | SingleElementMoved |     1,364.4 ns |     85.76 ns |    44.86 ns |  0.31 |    0.01 |    1 |         - |          NA |
| DotnetSort                   | 1024 | SingleElementMoved |     5,202.9 ns |     93.01 ns |    41.30 ns |  1.19 |    0.02 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Sorted**             |     **3,314.3 ns** |     **53.06 ns** |    **27.75 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Sorted             |     6,388.8 ns |    517.44 ns |   270.63 ns |  1.93 |    0.08 |    5 |         - |          NA |
| QuickSortMedian3             | 1024 | Sorted             |     4,458.6 ns |    301.67 ns |   157.78 ns |  1.35 |    0.05 |    4 |         - |          NA |
| QuickSortMedian9             | 1024 | Sorted             |     4,599.7 ns |     36.04 ns |    16.00 ns |  1.39 |    0.01 |    4 |         - |          NA |
| SampleSort                   | 1024 | Sorted             |    15,353.2 ns |    541.10 ns |   283.00 ns |  4.63 |    0.09 |    6 |         - |          NA |
| DualPivotQuickSort           | 1024 | Sorted             |     3,286.7 ns |    335.82 ns |   175.64 ns |  0.99 |    0.05 |    4 |         - |          NA |
| StableQuickSort              | 1024 | Sorted             |    30,785.9 ns |    879.18 ns |   459.83 ns |  9.29 |    0.15 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Sorted             |    15,744.5 ns |    194.30 ns |    86.27 ns |  4.75 |    0.04 |    6 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Sorted             |    31,884.7 ns |    728.02 ns |   323.25 ns |  9.62 |    0.12 |    7 |         - |          NA |
| IntroSort                    | 1024 | Sorted             |     3,127.9 ns |     48.34 ns |    21.47 ns |  0.94 |    0.01 |    4 |         - |          NA |
| IntroSortDotnet              | 1024 | Sorted             |     3,807.0 ns |    313.06 ns |   163.74 ns |  1.15 |    0.05 |    4 |         - |          NA |
| PDQSort                      | 1024 | Sorted             |     1,200.6 ns |      4.52 ns |     2.01 ns |  0.36 |    0.00 |    3 |         - |          NA |
| PDQSortBranchless            | 1024 | Sorted             |     1,188.9 ns |      6.24 ns |     2.77 ns |  0.36 |    0.00 |    3 |         - |          NA |
| Ipnsort                      | 1024 | Sorted             |       488.5 ns |     17.22 ns |     7.65 ns |  0.15 |    0.00 |    1 |         - |          NA |
| StdSort                      | 1024 | Sorted             |     1,221.7 ns |      8.66 ns |     3.85 ns |  0.37 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 1024 | Sorted             |     6,326.1 ns |    196.13 ns |   102.58 ns |  1.91 |    0.03 |    5 |         - |          NA |
| FluxSort                     | 1024 | Sorted             |       597.0 ns |      5.37 ns |     1.92 ns |  0.18 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 1024 | Sorted             |     3,153.8 ns |     24.48 ns |     8.73 ns |  0.95 |    0.01 |    4 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Reversed**           |     **3,625.5 ns** |    **104.48 ns** |    **46.39 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Reversed           |     7,091.0 ns |    531.33 ns |   277.89 ns |  1.96 |    0.08 |    7 |         - |          NA |
| QuickSortMedian3             | 1024 | Reversed           |     4,730.5 ns |    205.21 ns |    91.11 ns |  1.30 |    0.03 |    6 |         - |          NA |
| QuickSortMedian9             | 1024 | Reversed           |     4,880.7 ns |     14.07 ns |     6.25 ns |  1.35 |    0.02 |    6 |         - |          NA |
| SampleSort                   | 1024 | Reversed           |    16,523.1 ns |    611.61 ns |   319.88 ns |  4.56 |    0.10 |    8 |         - |          NA |
| DualPivotQuickSort           | 1024 | Reversed           |     3,727.8 ns |    333.60 ns |   174.48 ns |  1.03 |    0.05 |    5 |         - |          NA |
| StableQuickSort              | 1024 | Reversed           |    31,103.9 ns |    499.72 ns |   221.88 ns |  8.58 |    0.12 |    9 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Reversed           |    15,944.4 ns |     76.75 ns |    40.14 ns |  4.40 |    0.05 |    8 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Reversed           |    30,805.8 ns |    381.22 ns |   199.39 ns |  8.50 |    0.11 |    9 |         - |          NA |
| IntroSort                    | 1024 | Reversed           |     3,216.7 ns |     26.35 ns |     9.40 ns |  0.89 |    0.01 |    5 |         - |          NA |
| IntroSortDotnet              | 1024 | Reversed           |     5,536.3 ns |    124.88 ns |    44.53 ns |  1.53 |    0.02 |    6 |         - |          NA |
| PDQSort                      | 1024 | Reversed           |     1,627.2 ns |     17.44 ns |     7.75 ns |  0.45 |    0.01 |    3 |         - |          NA |
| PDQSortBranchless            | 1024 | Reversed           |     2,028.5 ns |     12.43 ns |     5.52 ns |  0.56 |    0.01 |    4 |         - |          NA |
| Ipnsort                      | 1024 | Reversed           |       741.0 ns |     10.76 ns |     4.78 ns |  0.20 |    0.00 |    1 |         - |          NA |
| StdSort                      | 1024 | Reversed           |     2,355.0 ns |      7.59 ns |     3.37 ns |  0.65 |    0.01 |    4 |         - |          NA |
| BlockQuickSort               | 1024 | Reversed           |     8,436.1 ns |    222.76 ns |   116.51 ns |  2.33 |    0.04 |    7 |         - |          NA |
| FluxSort                     | 1024 | Reversed           |       910.0 ns |     16.01 ns |     7.11 ns |  0.25 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 1024 | Reversed           |     5,774.5 ns |    688.63 ns |   305.75 ns |  1.59 |    0.08 |    6 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **PipeOrgan**          |    **83,810.9 ns** |    **314.87 ns** |   **139.80 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| QuickSort3way                | 1024 | PipeOrgan          |    16,195.7 ns |  2,501.92 ns | 1,308.55 ns |  0.19 |    0.01 |    4 |         - |          NA |
| QuickSortMedian3             | 1024 | PipeOrgan          |    12,463.6 ns |    520.74 ns |   231.21 ns |  0.15 |    0.00 |    3 |         - |          NA |
| QuickSortMedian9             | 1024 | PipeOrgan          |     9,407.1 ns |    594.05 ns |   310.70 ns |  0.11 |    0.00 |    3 |         - |          NA |
| SampleSort                   | 1024 | PipeOrgan          |    16,924.7 ns |    629.34 ns |   329.16 ns |  0.20 |    0.00 |    4 |         - |          NA |
| DualPivotQuickSort           | 1024 | PipeOrgan          |     7,730.5 ns |    223.20 ns |   116.74 ns |  0.09 |    0.00 |    2 |         - |          NA |
| StableQuickSort              | 1024 | PipeOrgan          |    31,042.6 ns |    761.33 ns |   398.19 ns |  0.37 |    0.00 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | PipeOrgan          |    15,790.4 ns |    126.56 ns |    56.19 ns |  0.19 |    0.00 |    4 |         - |          NA |
| DestswapStableQuickSort      | 1024 | PipeOrgan          |    40,152.2 ns |    346.18 ns |   181.06 ns |  0.48 |    0.00 |    6 |         - |          NA |
| IntroSort                    | 1024 | PipeOrgan          |    20,424.7 ns |    287.34 ns |   150.28 ns |  0.24 |    0.00 |    4 |         - |          NA |
| IntroSortDotnet              | 1024 | PipeOrgan          |    11,122.0 ns |    293.95 ns |   153.74 ns |  0.13 |    0.00 |    3 |         - |          NA |
| PDQSort                      | 1024 | PipeOrgan          |     6,569.9 ns |    308.09 ns |   161.14 ns |  0.08 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | PipeOrgan          |    10,463.0 ns |    323.26 ns |   169.07 ns |  0.12 |    0.00 |    3 |         - |          NA |
| Ipnsort                      | 1024 | PipeOrgan          |    16,681.8 ns |     87.17 ns |    38.71 ns |  0.20 |    0.00 |    4 |         - |          NA |
| StdSort                      | 1024 | PipeOrgan          |    18,173.6 ns |    262.45 ns |   137.27 ns |  0.22 |    0.00 |    4 |         - |          NA |
| BlockQuickSort               | 1024 | PipeOrgan          |    18,014.4 ns |    237.31 ns |   105.37 ns |  0.21 |    0.00 |    4 |         - |          NA |
| FluxSort                     | 1024 | PipeOrgan          |     2,272.6 ns |     38.29 ns |    17.00 ns |  0.03 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 1024 | PipeOrgan          |    13,057.5 ns |    878.75 ns |   390.17 ns |  0.16 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **ManyDuplicates**     |    **26,530.4 ns** |    **264.40 ns** |   **138.28 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 1024 | ManyDuplicates     |    20,239.5 ns |    149.02 ns |    77.94 ns |  0.76 |    0.00 |    3 |         - |          NA |
| QuickSortMedian3             | 1024 | ManyDuplicates     |    29,878.3 ns |     78.19 ns |    34.72 ns |  1.13 |    0.01 |    3 |         - |          NA |
| QuickSortMedian9             | 1024 | ManyDuplicates     |    31,398.3 ns |    291.49 ns |   152.45 ns |  1.18 |    0.01 |    3 |         - |          NA |
| SampleSort                   | 1024 | ManyDuplicates     |    39,192.5 ns |    362.53 ns |   160.96 ns |  1.48 |    0.01 |    3 |         - |          NA |
| DualPivotQuickSort           | 1024 | ManyDuplicates     |    19,528.4 ns |    361.58 ns |   189.11 ns |  0.74 |    0.01 |    3 |         - |          NA |
| StableQuickSort              | 1024 | ManyDuplicates     |    46,282.6 ns |  1,131.78 ns |   591.94 ns |  1.74 |    0.02 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | ManyDuplicates     |    20,927.0 ns |    315.68 ns |   165.11 ns |  0.79 |    0.01 |    3 |         - |          NA |
| DestswapStableQuickSort      | 1024 | ManyDuplicates     |    35,581.7 ns |    208.03 ns |   108.81 ns |  1.34 |    0.01 |    3 |         - |          NA |
| IntroSort                    | 1024 | ManyDuplicates     |    24,852.9 ns |    420.05 ns |   219.70 ns |  0.94 |    0.01 |    3 |         - |          NA |
| IntroSortDotnet              | 1024 | ManyDuplicates     |    25,391.8 ns |    278.29 ns |   123.56 ns |  0.96 |    0.01 |    3 |         - |          NA |
| PDQSort                      | 1024 | ManyDuplicates     |    20,475.5 ns |     93.04 ns |    41.31 ns |  0.77 |    0.00 |    3 |         - |          NA |
| PDQSortBranchless            | 1024 | ManyDuplicates     |     9,521.4 ns |    268.34 ns |   140.35 ns |  0.36 |    0.01 |    1 |         - |          NA |
| Ipnsort                      | 1024 | ManyDuplicates     |    15,239.9 ns |    171.33 ns |    89.61 ns |  0.57 |    0.00 |    2 |         - |          NA |
| StdSort                      | 1024 | ManyDuplicates     |    15,146.1 ns |     94.12 ns |    41.79 ns |  0.57 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | ManyDuplicates     |    16,085.6 ns |    161.86 ns |    84.66 ns |  0.61 |    0.00 |    2 |         - |          NA |
| FluxSort                     | 1024 | ManyDuplicates     |    24,660.5 ns |    310.54 ns |   137.88 ns |  0.93 |    0.01 |    3 |         - |          NA |
| DotnetSort                   | 1024 | ManyDuplicates     |    23,319.7 ns |    326.39 ns |   144.92 ns |  0.88 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Random**             |   **185,668.8 ns** |  **1,812.11 ns** |   **947.77 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Random             |   202,898.8 ns |  2,497.78 ns | 1,306.38 ns |  1.09 |    0.01 |    2 |         - |          NA |
| QuickSortMedian3             | 4096 | Random             |   194,390.4 ns |  1,346.80 ns |   704.40 ns |  1.05 |    0.01 |    2 |         - |          NA |
| QuickSortMedian9             | 4096 | Random             |   206,596.6 ns |  2,292.25 ns | 1,017.77 ns |  1.11 |    0.01 |    2 |         - |          NA |
| SampleSort                   | 4096 | Random             |   234,413.7 ns |    333.21 ns |   118.83 ns |  1.26 |    0.01 |    2 |         - |          NA |
| DualPivotQuickSort           | 4096 | Random             |   144,012.4 ns |  1,014.33 ns |   530.52 ns |  0.78 |    0.00 |    2 |         - |          NA |
| StableQuickSort              | 4096 | Random             |   467,814.0 ns |  1,004.69 ns |   446.09 ns |  2.52 |    0.01 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Random             |   242,767.7 ns |  4,245.06 ns | 2,220.25 ns |  1.31 |    0.01 |    2 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Random             |   389,430.4 ns |  1,190.07 ns |   528.40 ns |  2.10 |    0.01 |    3 |         - |          NA |
| IntroSort                    | 4096 | Random             |   160,103.7 ns |    853.76 ns |   379.08 ns |  0.86 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet              | 4096 | Random             |   155,169.6 ns |    510.74 ns |   226.77 ns |  0.84 |    0.00 |    2 |         - |          NA |
| PDQSort                      | 4096 | Random             |   149,698.9 ns |  2,224.05 ns |   987.49 ns |  0.81 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 4096 | Random             |    83,585.7 ns |    354.05 ns |   157.20 ns |  0.45 |    0.00 |    1 |         - |          NA |
| Ipnsort                      | 4096 | Random             |    90,688.8 ns |    748.77 ns |   391.62 ns |  0.49 |    0.00 |    1 |         - |          NA |
| StdSort                      | 4096 | Random             |    89,060.2 ns |    323.25 ns |   115.28 ns |  0.48 |    0.00 |    1 |         - |          NA |
| BlockQuickSort               | 4096 | Random             |    95,688.2 ns |    342.32 ns |   151.99 ns |  0.52 |    0.00 |    1 |         - |          NA |
| FluxSort                     | 4096 | Random             |   109,849.2 ns |    371.68 ns |   165.03 ns |  0.59 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 4096 | Random             |   144,471.4 ns |  1,490.59 ns |   779.61 ns |  0.78 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **4096** | **SingleElementMoved** |    **19,095.8 ns** |    **429.84 ns** |   **224.82 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 4096 | SingleElementMoved |    34,656.6 ns |  3,263.72 ns | 1,449.11 ns |  1.82 |    0.07 |    3 |         - |          NA |
| QuickSortMedian3             | 4096 | SingleElementMoved |    26,435.8 ns |    304.39 ns |   135.15 ns |  1.38 |    0.02 |    3 |         - |          NA |
| QuickSortMedian9             | 4096 | SingleElementMoved |    30,202.3 ns |    330.65 ns |   172.94 ns |  1.58 |    0.02 |    3 |         - |          NA |
| SampleSort                   | 4096 | SingleElementMoved |   131,833.2 ns |  2,296.50 ns | 1,201.12 ns |  6.90 |    0.10 |    6 |         - |          NA |
| DualPivotQuickSort           | 4096 | SingleElementMoved |    17,414.4 ns |  1,866.30 ns |   976.11 ns |  0.91 |    0.05 |    3 |         - |          NA |
| StableQuickSort              | 4096 | SingleElementMoved |   149,379.9 ns |  3,136.09 ns | 1,640.24 ns |  7.82 |    0.12 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | SingleElementMoved |    71,789.6 ns |    701.13 ns |   311.31 ns |  3.76 |    0.04 |    5 |         - |          NA |
| DestswapStableQuickSort      | 4096 | SingleElementMoved |   141,902.5 ns |    988.73 ns |   439.00 ns |  7.43 |    0.09 |    6 |         - |          NA |
| IntroSort                    | 4096 | SingleElementMoved |    51,694.5 ns |    235.92 ns |   123.39 ns |  2.71 |    0.03 |    4 |         - |          NA |
| IntroSortDotnet              | 4096 | SingleElementMoved |    21,011.3 ns |    289.96 ns |   151.65 ns |  1.10 |    0.01 |    3 |         - |          NA |
| PDQSort                      | 4096 | SingleElementMoved |    11,977.9 ns |    265.22 ns |   138.72 ns |  0.63 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 4096 | SingleElementMoved |    13,873.0 ns |    277.58 ns |   145.18 ns |  0.73 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 4096 | SingleElementMoved |    70,335.9 ns |    338.57 ns |   150.33 ns |  3.68 |    0.04 |    5 |         - |          NA |
| StdSort                      | 4096 | SingleElementMoved |    13,039.5 ns |     98.73 ns |    51.64 ns |  0.68 |    0.01 |    2 |         - |          NA |
| BlockQuickSort               | 4096 | SingleElementMoved |    34,140.1 ns |    310.24 ns |   162.26 ns |  1.79 |    0.02 |    3 |         - |          NA |
| FluxSort                     | 4096 | SingleElementMoved |     4,281.6 ns |     70.35 ns |    31.24 ns |  0.22 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 4096 | SingleElementMoved |    24,523.6 ns |    310.78 ns |   162.54 ns |  1.28 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Sorted**             |    **15,627.9 ns** |    **372.29 ns** |   **194.71 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Sorted             |    27,837.3 ns |  1,434.72 ns |   750.39 ns |  1.78 |    0.05 |    5 |         - |          NA |
| QuickSortMedian3             | 4096 | Sorted             |    19,690.1 ns |     99.28 ns |    44.08 ns |  1.26 |    0.01 |    4 |         - |          NA |
| QuickSortMedian9             | 4096 | Sorted             |    20,946.6 ns |    223.72 ns |   117.01 ns |  1.34 |    0.02 |    4 |         - |          NA |
| SampleSort                   | 4096 | Sorted             |    70,142.7 ns |    877.47 ns |   458.93 ns |  4.49 |    0.06 |    6 |         - |          NA |
| DualPivotQuickSort           | 4096 | Sorted             |    15,528.9 ns |  1,357.67 ns |   710.09 ns |  0.99 |    0.04 |    4 |         - |          NA |
| StableQuickSort              | 4096 | Sorted             |   149,234.1 ns |  1,548.31 ns |   687.46 ns |  9.55 |    0.12 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Sorted             |    70,619.0 ns |    456.15 ns |   202.53 ns |  4.52 |    0.05 |    6 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Sorted             |   139,943.2 ns |  1,036.75 ns |   460.33 ns |  8.96 |    0.11 |    7 |         - |          NA |
| IntroSort                    | 4096 | Sorted             |    14,936.5 ns |     72.54 ns |    37.94 ns |  0.96 |    0.01 |    4 |         - |          NA |
| IntroSortDotnet              | 4096 | Sorted             |    17,224.0 ns |    412.67 ns |   215.84 ns |  1.10 |    0.02 |    4 |         - |          NA |
| PDQSort                      | 4096 | Sorted             |     4,561.6 ns |     51.94 ns |    18.52 ns |  0.29 |    0.00 |    3 |         - |          NA |
| PDQSortBranchless            | 4096 | Sorted             |     4,659.4 ns |    247.92 ns |   129.67 ns |  0.30 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 4096 | Sorted             |     1,800.0 ns |     12.66 ns |     4.51 ns |  0.12 |    0.00 |    1 |         - |          NA |
| StdSort                      | 4096 | Sorted             |     4,606.1 ns |     46.70 ns |    16.65 ns |  0.29 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 4096 | Sorted             |    30,497.8 ns |    348.47 ns |   182.26 ns |  1.95 |    0.03 |    5 |         - |          NA |
| FluxSort                     | 4096 | Sorted             |     2,197.9 ns |     15.51 ns |     5.53 ns |  0.14 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 4096 | Sorted             |    15,089.9 ns |    296.56 ns |   105.76 ns |  0.97 |    0.01 |    4 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Reversed**           |    **16,963.8 ns** |    **658.37 ns** |   **292.32 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Reversed           |    31,398.2 ns |  1,547.94 ns |   809.60 ns |  1.85 |    0.05 |    7 |         - |          NA |
| QuickSortMedian3             | 4096 | Reversed           |    20,848.5 ns |    155.26 ns |    81.21 ns |  1.23 |    0.02 |    6 |         - |          NA |
| QuickSortMedian9             | 4096 | Reversed           |    21,923.4 ns |    425.63 ns |   222.61 ns |  1.29 |    0.02 |    6 |         - |          NA |
| SampleSort                   | 4096 | Reversed           |    74,143.1 ns |  2,306.04 ns | 1,206.10 ns |  4.37 |    0.10 |    8 |         - |          NA |
| DualPivotQuickSort           | 4096 | Reversed           |    16,849.9 ns |  1,973.90 ns | 1,032.39 ns |  0.99 |    0.06 |    5 |         - |          NA |
| StableQuickSort              | 4096 | Reversed           |   151,396.1 ns |  1,493.07 ns |   780.90 ns |  8.93 |    0.15 |    9 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Reversed           |    70,705.5 ns |    724.08 ns |   321.50 ns |  4.17 |    0.07 |    8 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Reversed           |   134,879.8 ns |  2,057.76 ns | 1,076.25 ns |  7.95 |    0.14 |    9 |         - |          NA |
| IntroSort                    | 4096 | Reversed           |    15,699.4 ns |    542.78 ns |   283.88 ns |  0.93 |    0.02 |    5 |         - |          NA |
| IntroSortDotnet              | 4096 | Reversed           |    26,806.3 ns |    303.57 ns |   158.78 ns |  1.58 |    0.03 |    7 |         - |          NA |
| PDQSort                      | 4096 | Reversed           |     6,381.0 ns |    290.56 ns |   151.97 ns |  0.38 |    0.01 |    3 |         - |          NA |
| PDQSortBranchless            | 4096 | Reversed           |     7,898.8 ns |    335.23 ns |   175.33 ns |  0.47 |    0.01 |    4 |         - |          NA |
| Ipnsort                      | 4096 | Reversed           |     2,771.3 ns |     13.39 ns |     5.94 ns |  0.16 |    0.00 |    1 |         - |          NA |
| StdSort                      | 4096 | Reversed           |     9,126.9 ns |    267.51 ns |   139.91 ns |  0.54 |    0.01 |    4 |         - |          NA |
| BlockQuickSort               | 4096 | Reversed           |    37,509.4 ns |    363.99 ns |   190.37 ns |  2.21 |    0.04 |    7 |         - |          NA |
| FluxSort                     | 4096 | Reversed           |     3,341.7 ns |     28.61 ns |    10.20 ns |  0.20 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 4096 | Reversed           |    27,865.5 ns |  3,403.62 ns | 1,780.16 ns |  1.64 |    0.10 |    7 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **4096** | **PipeOrgan**          | **1,216,643.5 ns** |  **3,568.46 ns** | **1,866.37 ns** | **1.000** |    **0.00** |    **6** |         **-** |          **NA** |
| QuickSort3way                | 4096 | PipeOrgan          |    79,921.1 ns |  1,598.19 ns |   709.60 ns | 0.066 |    0.00 |    3 |         - |          NA |
| QuickSortMedian3             | 4096 | PipeOrgan          |    64,074.2 ns |    860.82 ns |   450.22 ns | 0.053 |    0.00 |    3 |         - |          NA |
| QuickSortMedian9             | 4096 | PipeOrgan          |    41,751.5 ns |  2,635.61 ns | 1,170.23 ns | 0.034 |    0.00 |    2 |         - |          NA |
| SampleSort                   | 4096 | PipeOrgan          |    75,320.1 ns |  2,212.75 ns | 1,157.31 ns | 0.062 |    0.00 |    3 |         - |          NA |
| DualPivotQuickSort           | 4096 | PipeOrgan          |    35,646.8 ns |  1,351.42 ns |   706.82 ns | 0.029 |    0.00 |    2 |         - |          NA |
| StableQuickSort              | 4096 | PipeOrgan          |   150,328.2 ns |  2,151.83 ns |   955.43 ns | 0.124 |    0.00 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | PipeOrgan          |    70,991.2 ns |    496.06 ns |   220.26 ns | 0.058 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 4096 | PipeOrgan          |   178,943.9 ns |    826.80 ns |   367.11 ns | 0.147 |    0.00 |    5 |         - |          NA |
| IntroSort                    | 4096 | PipeOrgan          |   104,881.5 ns |    521.91 ns |   231.73 ns | 0.086 |    0.00 |    4 |         - |          NA |
| IntroSortDotnet              | 4096 | PipeOrgan          |    65,315.2 ns |  1,643.53 ns |   859.60 ns | 0.054 |    0.00 |    3 |         - |          NA |
| PDQSort                      | 4096 | PipeOrgan          |    31,247.5 ns |    294.28 ns |   130.66 ns | 0.026 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 4096 | PipeOrgan          |    47,732.7 ns |    385.44 ns |   171.14 ns | 0.039 |    0.00 |    2 |         - |          NA |
| Ipnsort                      | 4096 | PipeOrgan          |    83,802.5 ns |    436.18 ns |   193.67 ns | 0.069 |    0.00 |    3 |         - |          NA |
| StdSort                      | 4096 | PipeOrgan          |   102,206.0 ns |    702.41 ns |   311.88 ns | 0.084 |    0.00 |    4 |         - |          NA |
| BlockQuickSort               | 4096 | PipeOrgan          |    81,370.6 ns |  1,383.29 ns |   614.19 ns | 0.067 |    0.00 |    3 |         - |          NA |
| FluxSort                     | 4096 | PipeOrgan          |     8,903.5 ns |    218.93 ns |   114.50 ns | 0.007 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 4096 | PipeOrgan          |    72,856.6 ns |  2,906.18 ns | 1,519.99 ns | 0.060 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **4096** | **ManyDuplicates**     |   **103,551.4 ns** |    **993.08 ns** |   **519.40 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 4096 | ManyDuplicates     |    74,944.0 ns |    675.52 ns |   353.31 ns |  0.72 |    0.00 |    4 |         - |          NA |
| QuickSortMedian3             | 4096 | ManyDuplicates     |   113,256.5 ns |    718.34 ns |   318.95 ns |  1.09 |    0.01 |    5 |         - |          NA |
| QuickSortMedian9             | 4096 | ManyDuplicates     |   119,040.1 ns |    467.39 ns |   207.52 ns |  1.15 |    0.01 |    5 |         - |          NA |
| SampleSort                   | 4096 | ManyDuplicates     |   154,916.4 ns |    868.57 ns |   454.28 ns |  1.50 |    0.01 |    5 |         - |          NA |
| DualPivotQuickSort           | 4096 | ManyDuplicates     |    73,176.2 ns |    524.71 ns |   274.43 ns |  0.71 |    0.00 |    4 |         - |          NA |
| StableQuickSort              | 4096 | ManyDuplicates     |   178,179.4 ns |  5,679.74 ns | 2,970.61 ns |  1.72 |    0.03 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | ManyDuplicates     |    78,157.4 ns |    743.47 ns |   388.85 ns |  0.75 |    0.01 |    4 |         - |          NA |
| DestswapStableQuickSort      | 4096 | ManyDuplicates     |   129,849.8 ns |    384.10 ns |   170.54 ns |  1.25 |    0.01 |    5 |         - |          NA |
| IntroSort                    | 4096 | ManyDuplicates     |    96,797.4 ns |    448.65 ns |   199.20 ns |  0.93 |    0.00 |    5 |         - |          NA |
| IntroSortDotnet              | 4096 | ManyDuplicates     |   103,106.2 ns |    320.93 ns |   142.50 ns |  1.00 |    0.00 |    5 |         - |          NA |
| PDQSort                      | 4096 | ManyDuplicates     |    73,782.8 ns |    461.22 ns |   241.23 ns |  0.71 |    0.00 |    4 |         - |          NA |
| PDQSortBranchless            | 4096 | ManyDuplicates     |    27,046.6 ns |    424.19 ns |   188.34 ns |  0.26 |    0.00 |    1 |         - |          NA |
| Ipnsort                      | 4096 | ManyDuplicates     |    48,710.8 ns |    240.47 ns |   106.77 ns |  0.47 |    0.00 |    3 |         - |          NA |
| StdSort                      | 4096 | ManyDuplicates     |    42,680.5 ns |    275.68 ns |   144.18 ns |  0.41 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 4096 | ManyDuplicates     |    55,954.0 ns |    429.53 ns |   224.65 ns |  0.54 |    0.00 |    3 |         - |          NA |
| FluxSort                     | 4096 | ManyDuplicates     |    34,474.0 ns |    276.90 ns |   144.83 ns |  0.33 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 4096 | ManyDuplicates     |    94,686.4 ns |    400.44 ns |   177.80 ns |  0.91 |    0.00 |    5 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Random**             |   **396,076.5 ns** |  **2,386.83 ns** | **1,248.36 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Random             |   432,981.1 ns |  4,967.46 ns | 2,598.08 ns |  1.09 |    0.01 |    2 |         - |          NA |
| QuickSortMedian3             | 8192 | Random             |   412,492.0 ns |  2,522.32 ns | 1,319.22 ns |  1.04 |    0.00 |    2 |         - |          NA |
| QuickSortMedian9             | 8192 | Random             |   438,325.9 ns |  3,080.49 ns | 1,611.16 ns |  1.11 |    0.01 |    2 |         - |          NA |
| SampleSort                   | 8192 | Random             |   498,469.5 ns |  1,166.74 ns |   518.04 ns |  1.26 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort           | 8192 | Random             |   313,692.1 ns |  3,901.01 ns | 2,040.30 ns |  0.79 |    0.01 |    2 |         - |          NA |
| StableQuickSort              | 8192 | Random             | 1,009,894.6 ns |  1,050.21 ns |   549.28 ns |  2.55 |    0.01 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Random             |   519,855.1 ns |  1,911.05 ns |   999.51 ns |  1.31 |    0.00 |    2 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Random             |   822,127.0 ns |  1,597.40 ns |   709.25 ns |  2.08 |    0.01 |    3 |         - |          NA |
| IntroSort                    | 8192 | Random             |   345,801.2 ns |  1,085.39 ns |   481.92 ns |  0.87 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet              | 8192 | Random             |   336,957.8 ns |  1,532.16 ns |   801.35 ns |  0.85 |    0.00 |    2 |         - |          NA |
| PDQSort                      | 8192 | Random             |   322,999.3 ns |    740.58 ns |   328.82 ns |  0.82 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | Random             |   173,126.4 ns |    980.76 ns |   435.46 ns |  0.44 |    0.00 |    1 |         - |          NA |
| Ipnsort                      | 8192 | Random             |   194,856.4 ns |  1,134.92 ns |   593.59 ns |  0.49 |    0.00 |    1 |         - |          NA |
| StdSort                      | 8192 | Random             |   186,901.1 ns |    984.06 ns |   514.68 ns |  0.47 |    0.00 |    1 |         - |          NA |
| BlockQuickSort               | 8192 | Random             |   199,978.0 ns |  1,160.88 ns |   607.16 ns |  0.50 |    0.00 |    1 |         - |          NA |
| FluxSort                     | 8192 | Random             |   226,588.8 ns |  1,886.53 ns |   837.63 ns |  0.57 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 8192 | Random             |   314,627.8 ns |  1,110.42 ns |   493.03 ns |  0.79 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **SingleElementMoved** |    **40,287.1 ns** |    **588.23 ns** |   **307.65 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 8192 | SingleElementMoved |    71,797.8 ns |  3,636.92 ns | 1,902.18 ns |  1.78 |    0.05 |    3 |         - |          NA |
| QuickSortMedian3             | 8192 | SingleElementMoved |    54,162.9 ns |    172.97 ns |    90.47 ns |  1.34 |    0.01 |    3 |         - |          NA |
| QuickSortMedian9             | 8192 | SingleElementMoved |    62,526.0 ns |    754.84 ns |   335.15 ns |  1.55 |    0.01 |    3 |         - |          NA |
| SampleSort                   | 8192 | SingleElementMoved |   318,354.8 ns |  1,519.52 ns |   794.74 ns |  7.90 |    0.06 |    6 |         - |          NA |
| DualPivotQuickSort           | 8192 | SingleElementMoved |    39,435.9 ns |  1,264.05 ns |   661.12 ns |  0.98 |    0.02 |    3 |         - |          NA |
| StableQuickSort              | 8192 | SingleElementMoved |   322,806.7 ns |  2,279.48 ns | 1,012.10 ns |  8.01 |    0.06 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | SingleElementMoved |   151,018.8 ns |    791.61 ns |   351.48 ns |  3.75 |    0.03 |    5 |         - |          NA |
| DestswapStableQuickSort      | 8192 | SingleElementMoved |   294,173.3 ns |  1,252.69 ns |   556.20 ns |  7.30 |    0.05 |    6 |         - |          NA |
| IntroSort                    | 8192 | SingleElementMoved |   108,543.5 ns |    405.40 ns |   180.00 ns |  2.69 |    0.02 |    4 |         - |          NA |
| IntroSortDotnet              | 8192 | SingleElementMoved |    44,312.9 ns |    193.44 ns |   101.17 ns |  1.10 |    0.01 |    3 |         - |          NA |
| PDQSort                      | 8192 | SingleElementMoved |    23,941.6 ns |    323.66 ns |   169.28 ns |  0.59 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | SingleElementMoved |    27,340.6 ns |    253.90 ns |   132.79 ns |  0.68 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 8192 | SingleElementMoved |   154,022.6 ns |  1,216.24 ns |   636.11 ns |  3.82 |    0.03 |    5 |         - |          NA |
| StdSort                      | 8192 | SingleElementMoved |    25,216.4 ns |    242.35 ns |   126.75 ns |  0.63 |    0.01 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | SingleElementMoved |    73,573.6 ns |  1,322.30 ns |   691.59 ns |  1.83 |    0.02 |    3 |         - |          NA |
| FluxSort                     | 8192 | SingleElementMoved |     8,303.2 ns |     37.64 ns |    16.71 ns |  0.21 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 8192 | SingleElementMoved |    53,056.9 ns |  1,367.31 ns |   607.09 ns |  1.32 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Sorted**             |    **33,646.0 ns** |    **429.83 ns** |   **224.81 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Sorted             |    58,033.8 ns |  3,207.43 ns | 1,677.55 ns |  1.72 |    0.05 |    5 |         - |          NA |
| QuickSortMedian3             | 8192 | Sorted             |    41,491.6 ns |    481.98 ns |   252.08 ns |  1.23 |    0.01 |    4 |         - |          NA |
| QuickSortMedian9             | 8192 | Sorted             |    44,112.1 ns |    397.24 ns |   207.76 ns |  1.31 |    0.01 |    4 |         - |          NA |
| SampleSort                   | 8192 | Sorted             |   199,158.9 ns |  6,692.50 ns | 2,971.51 ns |  5.92 |    0.09 |    7 |         - |          NA |
| DualPivotQuickSort           | 8192 | Sorted             |    36,802.5 ns |  2,805.34 ns | 1,467.25 ns |  1.09 |    0.04 |    4 |         - |          NA |
| StableQuickSort              | 8192 | Sorted             |   323,067.5 ns |  1,928.16 ns |   856.11 ns |  9.60 |    0.07 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Sorted             |   149,716.1 ns |  1,762.02 ns |   782.35 ns |  4.45 |    0.04 |    6 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Sorted             |   292,427.3 ns |  1,261.15 ns |   659.61 ns |  8.69 |    0.06 |    8 |         - |          NA |
| IntroSort                    | 8192 | Sorted             |    32,108.0 ns |    599.03 ns |   265.97 ns |  0.95 |    0.01 |    4 |         - |          NA |
| IntroSortDotnet              | 8192 | Sorted             |    36,683.4 ns |    461.77 ns |   241.51 ns |  1.09 |    0.01 |    4 |         - |          NA |
| PDQSort                      | 8192 | Sorted             |     9,195.3 ns |    453.57 ns |   237.22 ns |  0.27 |    0.01 |    3 |         - |          NA |
| PDQSortBranchless            | 8192 | Sorted             |     9,235.0 ns |    168.44 ns |    74.79 ns |  0.27 |    0.00 |    3 |         - |          NA |
| Ipnsort                      | 8192 | Sorted             |     3,656.5 ns |    364.48 ns |   190.63 ns |  0.11 |    0.01 |    1 |         - |          NA |
| StdSort                      | 8192 | Sorted             |     9,098.7 ns |    109.34 ns |    48.55 ns |  0.27 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 8192 | Sorted             |    65,968.0 ns |    186.45 ns |    82.79 ns |  1.96 |    0.01 |    5 |         - |          NA |
| FluxSort                     | 8192 | Sorted             |     4,501.1 ns |    301.06 ns |   157.46 ns |  0.13 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 8192 | Sorted             |    36,566.2 ns |  4,988.09 ns | 2,608.87 ns |  1.09 |    0.07 |    4 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Reversed**           |    **36,696.1 ns** |  **1,231.77 ns** |   **644.24 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Reversed           |    64,734.2 ns |  2,169.20 ns | 1,134.54 ns |  1.76 |    0.04 |    5 |         - |          NA |
| QuickSortMedian3             | 8192 | Reversed           |    43,746.7 ns |    338.28 ns |   176.93 ns |  1.19 |    0.02 |    4 |         - |          NA |
| QuickSortMedian9             | 8192 | Reversed           |    46,372.8 ns |    513.02 ns |   268.32 ns |  1.26 |    0.02 |    4 |         - |          NA |
| SampleSort                   | 8192 | Reversed           |   278,084.0 ns |  6,134.48 ns | 3,208.45 ns |  7.58 |    0.15 |    8 |         - |          NA |
| DualPivotQuickSort           | 8192 | Reversed           |    39,232.3 ns |  2,770.76 ns | 1,449.16 ns |  1.07 |    0.04 |    4 |         - |          NA |
| StableQuickSort              | 8192 | Reversed           |   330,270.4 ns |  3,952.94 ns | 1,755.13 ns |  9.00 |    0.16 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Reversed           |   150,915.7 ns |  2,074.36 ns | 1,084.93 ns |  4.11 |    0.07 |    7 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Reversed           |   282,569.7 ns |  1,900.37 ns |   843.78 ns |  7.70 |    0.13 |    8 |         - |          NA |
| IntroSort                    | 8192 | Reversed           |    33,918.8 ns |    677.00 ns |   354.08 ns |  0.92 |    0.02 |    4 |         - |          NA |
| IntroSortDotnet              | 8192 | Reversed           |    57,820.6 ns |    414.97 ns |   217.04 ns |  1.58 |    0.03 |    5 |         - |          NA |
| PDQSort                      | 8192 | Reversed           |    12,549.3 ns |    188.60 ns |    98.64 ns |  0.34 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | Reversed           |    15,513.0 ns |    196.34 ns |    87.18 ns |  0.42 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 8192 | Reversed           |     5,651.8 ns |    295.88 ns |   154.75 ns |  0.15 |    0.00 |    1 |         - |          NA |
| StdSort                      | 8192 | Reversed           |    18,162.3 ns |    227.19 ns |   118.83 ns |  0.50 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 8192 | Reversed           |    80,037.3 ns |    530.11 ns |   235.37 ns |  2.18 |    0.04 |    6 |         - |          NA |
| FluxSort                     | 8192 | Reversed           |     6,773.8 ns |    302.94 ns |   158.45 ns |  0.18 |    0.01 |    1 |         - |          NA |
| DotnetSort                   | 8192 | Reversed           |    59,460.1 ns |  5,994.22 ns | 3,135.09 ns |  1.62 |    0.08 |    5 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **PipeOrgan**          | **4,733,208.7 ns** | **11,677.91 ns** | **6,107.78 ns** | **1.000** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 8192 | PipeOrgan          |   204,168.3 ns | 13,830.29 ns | 7,233.51 ns | 0.043 |    0.00 |    3 |         - |          NA |
| QuickSortMedian3             | 8192 | PipeOrgan          |   149,336.2 ns |  4,145.74 ns | 2,168.30 ns | 0.032 |    0.00 |    3 |         - |          NA |
| QuickSortMedian9             | 8192 | PipeOrgan          |    89,490.2 ns |  4,249.80 ns | 2,222.73 ns | 0.019 |    0.00 |    2 |         - |          NA |
| SampleSort                   | 8192 | PipeOrgan          |   303,896.4 ns |  3,192.97 ns | 1,417.70 ns | 0.064 |    0.00 |    3 |         - |          NA |
| DualPivotQuickSort           | 8192 | PipeOrgan          |    77,028.9 ns |  2,037.78 ns | 1,065.80 ns | 0.016 |    0.00 |    2 |         - |          NA |
| StableQuickSort              | 8192 | PipeOrgan          |   324,504.8 ns |  2,422.08 ns | 1,266.80 ns | 0.069 |    0.00 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | PipeOrgan          |   150,829.1 ns |  1,729.68 ns |   767.99 ns | 0.032 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 8192 | PipeOrgan          |   376,935.8 ns |  1,847.01 ns |   820.08 ns | 0.080 |    0.00 |    3 |         - |          NA |
| IntroSort                    | 8192 | PipeOrgan          |   231,166.1 ns |  1,593.44 ns |   707.50 ns | 0.049 |    0.00 |    3 |         - |          NA |
| IntroSortDotnet              | 8192 | PipeOrgan          |   270,509.5 ns |  1,585.78 ns |   565.50 ns | 0.057 |    0.00 |    3 |         - |          NA |
| PDQSort                      | 8192 | PipeOrgan          |    67,722.7 ns |  1,000.99 ns |   444.44 ns | 0.014 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 8192 | PipeOrgan          |   102,131.2 ns |    897.51 ns |   469.41 ns | 0.022 |    0.00 |    2 |         - |          NA |
| Ipnsort                      | 8192 | PipeOrgan          |   185,462.5 ns |    467.44 ns |   244.48 ns | 0.039 |    0.00 |    3 |         - |          NA |
| StdSort                      | 8192 | PipeOrgan          |   249,204.7 ns |  2,390.32 ns | 1,250.18 ns | 0.053 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 8192 | PipeOrgan          |   158,269.4 ns |    788.78 ns |   412.55 ns | 0.033 |    0.00 |    3 |         - |          NA |
| FluxSort                     | 8192 | PipeOrgan          |    17,973.3 ns |    168.01 ns |    74.60 ns | 0.004 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 8192 | PipeOrgan          |   281,971.5 ns |  8,431.71 ns | 4,409.95 ns | 0.060 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **ManyDuplicates**     |   **205,605.1 ns** |  **1,427.78 ns** |   **633.94 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 8192 | ManyDuplicates     |   147,049.4 ns |    766.39 ns |   400.84 ns |  0.72 |    0.00 |    4 |         - |          NA |
| QuickSortMedian3             | 8192 | ManyDuplicates     |   222,803.7 ns |    816.39 ns |   362.48 ns |  1.08 |    0.00 |    5 |         - |          NA |
| QuickSortMedian9             | 8192 | ManyDuplicates     |   234,045.3 ns |    570.51 ns |   253.31 ns |  1.14 |    0.00 |    5 |         - |          NA |
| SampleSort                   | 8192 | ManyDuplicates     |   311,164.5 ns |  2,166.44 ns | 1,133.09 ns |  1.51 |    0.01 |    6 |         - |          NA |
| DualPivotQuickSort           | 8192 | ManyDuplicates     |   144,083.6 ns |  1,656.11 ns |   866.18 ns |  0.70 |    0.00 |    4 |         - |          NA |
| StableQuickSort              | 8192 | ManyDuplicates     |   345,338.0 ns |  4,898.67 ns | 2,562.10 ns |  1.68 |    0.01 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | ManyDuplicates     |   153,475.2 ns |  1,158.43 ns |   514.35 ns |  0.75 |    0.00 |    4 |         - |          NA |
| DestswapStableQuickSort      | 8192 | ManyDuplicates     |   252,456.4 ns |  1,630.36 ns |   723.89 ns |  1.23 |    0.00 |    5 |         - |          NA |
| IntroSort                    | 8192 | ManyDuplicates     |   192,968.3 ns |  1,209.31 ns |   536.94 ns |  0.94 |    0.00 |    5 |         - |          NA |
| IntroSortDotnet              | 8192 | ManyDuplicates     |   207,911.5 ns |    677.12 ns |   354.15 ns |  1.01 |    0.00 |    5 |         - |          NA |
| PDQSort                      | 8192 | ManyDuplicates     |   144,068.1 ns |    519.08 ns |   230.47 ns |  0.70 |    0.00 |    4 |         - |          NA |
| PDQSortBranchless            | 8192 | ManyDuplicates     |    48,865.3 ns |    260.24 ns |   115.55 ns |  0.24 |    0.00 |    1 |         - |          NA |
| Ipnsort                      | 8192 | ManyDuplicates     |    93,599.3 ns |    449.06 ns |   160.14 ns |  0.46 |    0.00 |    3 |         - |          NA |
| StdSort                      | 8192 | ManyDuplicates     |    77,505.7 ns |    638.36 ns |   227.65 ns |  0.38 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | ManyDuplicates     |   108,698.6 ns |  1,080.47 ns |   479.73 ns |  0.53 |    0.00 |    3 |         - |          NA |
| FluxSort                     | 8192 | ManyDuplicates     |    56,903.3 ns |    168.09 ns |    87.91 ns |  0.28 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 8192 | ManyDuplicates     |   190,726.8 ns |  1,148.37 ns |   600.62 ns |  0.93 |    0.00 |    5 |         - |          NA |

### SelectionBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.46 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8
UnrollFactor=1  WarmupCount=2
```

| Method              | Size | Pattern            | Mean         | Error       | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------------- |----- |------------------- |-------------:|------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **SelectionSort**       | **256**  | **Random**             |  **25,896.8 ns** |    **182.5 ns** |     **81.03 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Random             |  18,483.5 ns |    156.6 ns |     69.53 ns |  0.71 |    0.00 |    1 |         - |          NA |
| CycleSort           | 256  | Random             |  48,808.7 ns |    197.9 ns |    103.51 ns |  1.88 |    0.01 |    3 |         - |          NA |
| PancakeSort         | 256  | Random             |  32,640.7 ns |    179.5 ns |     79.70 ns |  1.26 |    0.00 |    2 |         - |          NA |
| BingoSort           | 256  | Random             |  30,751.1 ns |  4,732.5 ns |  2,475.21 ns |  1.19 |    0.09 |    2 |         - |          NA |
|      |                    |              |             |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **SingleElementMoved** |  **19,265.3 ns** |    **198.3 ns** |     **88.04 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | SingleElementMoved |  10,770.3 ns |    324.2 ns |    169.58 ns |  0.56 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | SingleElementMoved |  25,449.4 ns |    201.4 ns |    105.35 ns |  1.32 |    0.01 |    4 |         - |          NA |
| PancakeSort         | 256  | SingleElementMoved |  15,966.8 ns |    458.9 ns |    203.77 ns |  0.83 |    0.01 |    2 |         - |          NA |
| BingoSort           | 256  | SingleElementMoved |  19,607.2 ns |    112.4 ns |     58.77 ns |  1.02 |    0.01 |    3 |         - |          NA |
|      |                    |              |             |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **Sorted**             |  **19,082.0 ns** |    **192.5 ns** |     **85.48 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Sorted             |   8,164.1 ns |    264.0 ns |    138.06 ns |  0.43 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | Sorted             |  17,390.9 ns |    154.1 ns |     68.44 ns |  0.91 |    0.01 |    3 |         - |          NA |
| PancakeSort         | 256  | Sorted             |  13,093.6 ns |    225.2 ns |     99.97 ns |  0.69 |    0.01 |    2 |         - |          NA |
| BingoSort           | 256  | Sorted             |  19,206.5 ns |    442.9 ns |    231.65 ns |  1.01 |    0.01 |    3 |         - |          NA |
|      |                    |              |             |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **Reversed**           |  **17,100.8 ns** |    **599.2 ns** |    **266.05 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | Reversed           |   8,155.4 ns |    238.0 ns |    124.50 ns |  0.48 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | Reversed           |  30,262.6 ns |    191.1 ns |     99.93 ns |  1.77 |    0.03 |    4 |         - |          NA |
| PancakeSort         | 256  | Reversed           |  15,623.4 ns |  2,700.5 ns |  1,412.40 ns |  0.91 |    0.08 |    2 |         - |          NA |
| BingoSort           | 256  | Reversed           |  21,159.4 ns |    747.0 ns |    266.38 ns |  1.24 |    0.02 |    3 |         - |          NA |
|      |                    |              |             |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **PipeOrgan**          |  **18,561.6 ns** |    **634.9 ns** |    **332.08 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | PipeOrgan          |   8,961.9 ns |    465.9 ns |    243.67 ns |  0.48 |    0.01 |    1 |         - |          NA |
| CycleSort           | 256  | PipeOrgan          |  42,899.5 ns |    469.7 ns |    167.48 ns |  2.31 |    0.04 |    3 |         - |          NA |
| PancakeSort         | 256  | PipeOrgan          |  20,526.2 ns |    187.5 ns |     83.25 ns |  1.11 |    0.02 |    2 |         - |          NA |
| BingoSort           | 256  | PipeOrgan          |  20,894.8 ns |    426.0 ns |    222.80 ns |  1.13 |    0.02 |    2 |         - |          NA |
|      |                    |              |             |              |       |         |      |           |             |
| **SelectionSort**       | **256**  | **ManyDuplicates**     |  **24,259.2 ns** |    **328.6 ns** |    **171.87 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| DoubleSelectionSort | 256  | ManyDuplicates     |  19,440.6 ns |    339.0 ns |    177.29 ns |  0.80 |    0.01 |    2 |         - |          NA |
| CycleSort           | 256  | ManyDuplicates     |  50,091.7 ns |    229.0 ns |    119.76 ns |  2.06 |    0.01 |    5 |         - |          NA |
| PancakeSort         | 256  | ManyDuplicates     |  30,562.4 ns |    212.3 ns |    111.03 ns |  1.26 |    0.01 |    4 |         - |          NA |
| BingoSort           | 256  | ManyDuplicates     |   5,927.1 ns |    177.3 ns |     92.74 ns |  0.24 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Random**             | **326,365.8 ns** |    **633.0 ns** |    **225.73 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Random             | 194,484.8 ns |  1,020.7 ns |    453.18 ns |  0.60 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | Random             | 773,247.4 ns |  2,781.4 ns |  1,454.72 ns |  2.37 |    0.00 |    3 |         - |          NA |
| PancakeSort         | 1024 | Random             | 379,509.4 ns |    783.5 ns |    347.87 ns |  1.16 |    0.00 |    2 |         - |          NA |
| BingoSort           | 1024 | Random             | 341,465.7 ns |  2,695.3 ns |  1,196.74 ns |  1.05 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **SingleElementMoved** | **293,381.1 ns** |  **3,317.3 ns** |  **1,735.04 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | SingleElementMoved | 149,749.0 ns |    668.6 ns |    238.42 ns |  0.51 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | SingleElementMoved | 389,750.1 ns |    630.6 ns |    280.01 ns |  1.33 |    0.01 |    3 |         - |          NA |
| PancakeSort         | 1024 | SingleElementMoved | 245,479.8 ns | 21,619.3 ns | 11,307.34 ns |  0.84 |    0.04 |    2 |         - |          NA |
| BingoSort           | 1024 | SingleElementMoved | 293,661.7 ns |  1,324.1 ns |    587.93 ns |  1.00 |    0.01 |    2 |         - |          NA |
|      |                    |              |             |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Sorted**             | **292,130.5 ns** |  **1,476.8 ns** |    **655.71 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Sorted             | 124,295.1 ns |    783.4 ns |    347.82 ns |  0.43 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | Sorted             | 262,879.1 ns |    798.9 ns |    417.83 ns |  0.90 |    0.00 |    3 |         - |          NA |
| PancakeSort         | 1024 | Sorted             | 196,728.1 ns |    391.2 ns |    173.68 ns |  0.67 |    0.00 |    2 |         - |          NA |
| BingoSort           | 1024 | Sorted             | 292,529.6 ns |  1,461.7 ns |    649.02 ns |  1.00 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **Reversed**           | **262,091.6 ns** | **10,736.0 ns** |  **5,615.12 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | Reversed           | 124,990.1 ns |    121.0 ns |     53.74 ns |  0.48 |    0.01 |    1 |         - |          NA |
| CycleSort           | 1024 | Reversed           | 461,080.2 ns |    910.7 ns |    476.33 ns |  1.76 |    0.04 |    5 |         - |          NA |
| PancakeSort         | 1024 | Reversed           | 217,681.3 ns | 14,901.5 ns |  7,793.79 ns |  0.83 |    0.03 |    2 |         - |          NA |
| BingoSort           | 1024 | Reversed           | 345,321.0 ns | 19,776.1 ns | 10,343.29 ns |  1.32 |    0.05 |    4 |         - |          NA |
|      |                    |              |             |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **PipeOrgan**          | **302,078.4 ns** |  **2,658.7 ns** |  **1,390.57 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | PipeOrgan          | 154,934.8 ns |  1,631.5 ns |    853.33 ns |  0.51 |    0.00 |    1 |         - |          NA |
| CycleSort           | 1024 | PipeOrgan          | 678,673.3 ns |  1,299.4 ns |    679.61 ns |  2.25 |    0.01 |    3 |         - |          NA |
| PancakeSort         | 1024 | PipeOrgan          | 316,234.4 ns |    826.8 ns |    294.83 ns |  1.05 |    0.00 |    2 |         - |          NA |
| BingoSort           | 1024 | PipeOrgan          | 331,951.4 ns |  4,653.1 ns |  2,066.01 ns |  1.10 |    0.01 |    2 |         - |          NA |
|      |                    |              |             |              |       |         |      |           |             |
| **SelectionSort**       | **1024** | **ManyDuplicates**     | **312,830.1 ns** |    **400.3 ns** |    **142.73 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| DoubleSelectionSort | 1024 | ManyDuplicates     | 214,084.4 ns |  4,052.0 ns |  2,119.29 ns |  0.68 |    0.01 |    2 |         - |          NA |
| CycleSort           | 1024 | ManyDuplicates     | 765,235.7 ns |  1,347.2 ns |    704.63 ns |  2.45 |    0.00 |    4 |         - |          NA |
| PancakeSort         | 1024 | ManyDuplicates     | 358,497.9 ns |  2,946.3 ns |  1,308.16 ns |  1.15 |    0.00 |    3 |         - |          NA |
| BingoSort           | 1024 | ManyDuplicates     |  20,379.4 ns |    252.1 ns |    131.86 ns |  0.07 |    0.00 |    1 |         - |          NA |

### SelectionSimdBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.46 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8
UnrollFactor=1  WarmupCount=2
```

| Method                  | Size | Pattern            | Mean            | Error         | StdDev        | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------------ |----- |------------------- |----------------:|--------------:|--------------:|------:|--------:|-----:|----------:|------------:|
| **SelectionSort**           | **256**  | **Random**             |     **25,796.6 ns** |     **220.02 ns** |     **115.07 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| SelectionSortSimd       | 256  | Random             |      9,632.8 ns |     292.75 ns |     153.11 ns |  0.37 |    0.01 |    1 |         - |          NA |
| DoubleSelectionSort     | 256  | Random             |     18,483.8 ns |     322.19 ns |     143.05 ns |  0.72 |    0.01 |    2 |         - |          NA |
| DoubleSelectionSortSimd | 256  | Random             |     11,061.1 ns |     288.99 ns |     151.15 ns |  0.43 |    0.01 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **256**  | **SingleElementMoved** |     **19,327.4 ns** |     **128.86 ns** |      **67.40 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 256  | SingleElementMoved |      5,142.7 ns |     591.79 ns |     309.52 ns |  0.27 |    0.02 |    2 |         - |          NA |
| DoubleSelectionSort     | 256  | SingleElementMoved |     10,836.0 ns |     251.48 ns |     131.53 ns |  0.56 |    0.01 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 256  | SingleElementMoved |      4,119.4 ns |     272.02 ns |     142.27 ns |  0.21 |    0.01 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **256**  | **Sorted**             |     **19,043.9 ns** |     **212.52 ns** |      **94.36 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 256  | Sorted             |      3,624.0 ns |     257.19 ns |     134.52 ns |  0.19 |    0.01 |    2 |         - |          NA |
| DoubleSelectionSort     | 256  | Sorted             |      8,208.1 ns |     176.56 ns |      92.34 ns |  0.43 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 256  | Sorted             |      2,964.7 ns |     340.85 ns |     178.27 ns |  0.16 |    0.01 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **256**  | **Reversed**           |     **17,650.7 ns** |   **1,296.07 ns** |     **677.87 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 256  | Reversed           |      5,682.3 ns |      63.43 ns |      33.18 ns |  0.32 |    0.01 |    2 |         - |          NA |
| DoubleSelectionSort     | 256  | Reversed           |      8,196.3 ns |     295.60 ns |     154.60 ns |  0.46 |    0.02 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 256  | Reversed           |      4,315.8 ns |     203.88 ns |      90.53 ns |  0.24 |    0.01 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **256**  | **PipeOrgan**          |     **18,304.8 ns** |   **1,027.41 ns** |     **537.36 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 256  | PipeOrgan          |      7,091.9 ns |      55.63 ns |      24.70 ns |  0.39 |    0.01 |    2 |         - |          NA |
| DoubleSelectionSort     | 256  | PipeOrgan          |      9,273.5 ns |     184.47 ns |      96.48 ns |  0.51 |    0.01 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 256  | PipeOrgan          |      4,345.3 ns |     358.00 ns |     187.24 ns |  0.24 |    0.01 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **256**  | **ManyDuplicates**     |     **24,046.3 ns** |     **185.55 ns** |      **97.05 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| SelectionSortSimd       | 256  | ManyDuplicates     |      9,307.7 ns |     435.50 ns |     193.36 ns |  0.39 |    0.01 |    1 |         - |          NA |
| DoubleSelectionSort     | 256  | ManyDuplicates     |     19,359.1 ns |     166.76 ns |      74.04 ns |  0.81 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSortSimd | 256  | ManyDuplicates     |     10,760.6 ns |     196.77 ns |      87.37 ns |  0.45 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **1024** | **Random**             |    **330,282.5 ns** |   **9,166.65 ns** |   **4,794.34 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| SelectionSortSimd       | 1024 | Random             |     58,501.0 ns |     170.17 ns |      75.56 ns |  0.18 |    0.00 |    1 |         - |          NA |
| DoubleSelectionSort     | 1024 | Random             |    195,100.9 ns |   1,659.75 ns |     868.08 ns |  0.59 |    0.01 |    2 |         - |          NA |
| DoubleSelectionSortSimd | 1024 | Random             |     58,858.9 ns |     381.98 ns |     199.78 ns |  0.18 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **1024** | **SingleElementMoved** |    **293,219.1 ns** |   **1,721.62 ns** |     **900.44 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 1024 | SingleElementMoved |     32,941.8 ns |     448.83 ns |     234.74 ns |  0.11 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 1024 | SingleElementMoved |    142,837.5 ns |  15,071.62 ns |   7,882.75 ns |  0.49 |    0.03 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 1024 | SingleElementMoved |     22,277.3 ns |     418.76 ns |     219.02 ns |  0.08 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **1024** | **Sorted**             |    **292,110.8 ns** |   **2,204.74 ns** |     **978.92 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 1024 | Sorted             |     25,073.5 ns |     223.15 ns |      99.08 ns |  0.09 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 1024 | Sorted             |    124,568.3 ns |   1,371.86 ns |     609.11 ns |  0.43 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 1024 | Sorted             |     16,494.5 ns |     389.08 ns |     172.75 ns |  0.06 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **1024** | **Reversed**           |    **260,585.3 ns** |  **13,781.89 ns** |   **6,119.25 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| SelectionSortSimd       | 1024 | Reversed           |     39,153.8 ns |     217.37 ns |      96.51 ns |  0.15 |    0.00 |    1 |         - |          NA |
| DoubleSelectionSort     | 1024 | Reversed           |    125,569.2 ns |   2,449.68 ns |   1,281.23 ns |  0.48 |    0.01 |    2 |         - |          NA |
| DoubleSelectionSortSimd | 1024 | Reversed           |     38,284.8 ns |     198.93 ns |     104.04 ns |  0.15 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **1024** | **PipeOrgan**          |    **302,604.0 ns** |   **2,913.01 ns** |   **1,523.56 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 1024 | PipeOrgan          |     49,293.1 ns |     472.14 ns |     246.94 ns |  0.16 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 1024 | PipeOrgan          |    154,614.5 ns |   2,032.02 ns |     902.23 ns |  0.51 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 1024 | PipeOrgan          |     30,499.5 ns |     431.02 ns |     225.43 ns |  0.10 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **1024** | **ManyDuplicates**     |    **313,278.7 ns** |     **691.39 ns** |     **246.55 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| SelectionSortSimd       | 1024 | ManyDuplicates     |     56,800.6 ns |     311.58 ns |     162.96 ns |  0.18 |    0.00 |    1 |         - |          NA |
| DoubleSelectionSort     | 1024 | ManyDuplicates     |    213,302.5 ns |   1,913.11 ns |   1,000.59 ns |  0.68 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSortSimd | 1024 | ManyDuplicates     |     57,346.9 ns |     312.78 ns |     163.59 ns |  0.18 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **4096** | **Random**             |  **4,769,698.4 ns** |  **11,674.63 ns** |   **5,183.61 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| SelectionSortSimd       | 4096 | Random             |    528,589.1 ns |   1,505.50 ns |     668.45 ns |  0.11 |    0.00 |    1 |         - |          NA |
| DoubleSelectionSort     | 4096 | Random             |  2,540,272.3 ns |   9,393.71 ns |   4,170.86 ns |  0.53 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSortSimd | 4096 | Random             |    465,070.7 ns |   2,290.55 ns |   1,198.00 ns |  0.10 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **4096** | **SingleElementMoved** |  **4,608,713.0 ns** |  **10,732.01 ns** |   **4,765.08 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 4096 | SingleElementMoved |    325,342.2 ns |   1,257.89 ns |     657.90 ns |  0.07 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 4096 | SingleElementMoved |  2,284,244.4 ns | 186,526.50 ns |  82,818.92 ns |  0.50 |    0.02 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 4096 | SingleElementMoved |    203,892.5 ns |   2,031.21 ns |     901.87 ns |  0.04 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **4096** | **Sorted**             |  **4,603,003.7 ns** |   **4,876.83 ns** |   **2,550.67 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 4096 | Sorted             |    280,959.0 ns |     346.70 ns |     123.64 ns |  0.06 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 4096 | Sorted             |  1,949,797.1 ns |   5,917.59 ns |   2,627.45 ns |  0.42 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 4096 | Sorted             |    159,897.5 ns |   2,101.54 ns |   1,099.15 ns |  0.03 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **4096** | **Reversed**           |  **4,182,433.1 ns** |  **18,763.47 ns** |   **8,331.10 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| SelectionSortSimd       | 4096 | Reversed           |    486,805.4 ns |   2,117.29 ns |   1,107.38 ns |  0.12 |    0.00 |    1 |         - |          NA |
| DoubleSelectionSort     | 4096 | Reversed           |  1,958,784.4 ns |   5,724.75 ns |   2,994.15 ns |  0.47 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSortSimd | 4096 | Reversed           |    519,723.4 ns |   2,369.98 ns |   1,052.28 ns |  0.12 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **4096** | **PipeOrgan**          |  **4,625,181.0 ns** |   **9,228.88 ns** |   **4,097.68 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 4096 | PipeOrgan          |    606,797.7 ns |   1,620.76 ns |     847.69 ns |  0.13 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 4096 | PipeOrgan          |  2,376,144.0 ns |  24,778.84 ns |  12,959.81 ns |  0.51 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 4096 | PipeOrgan          |    404,408.0 ns |   1,557.17 ns |     555.30 ns |  0.09 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **4096** | **ManyDuplicates**     |  **4,693,985.5 ns** |  **10,768.81 ns** |   **5,632.29 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| SelectionSortSimd       | 4096 | ManyDuplicates     |    515,612.9 ns |   2,161.58 ns |     959.76 ns |  0.11 |    0.00 |    1 |         - |          NA |
| DoubleSelectionSort     | 4096 | ManyDuplicates     |  2,651,010.8 ns |  16,004.85 ns |   7,106.25 ns |  0.56 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSortSimd | 4096 | ManyDuplicates     |    450,342.1 ns |   1,919.52 ns |   1,003.94 ns |  0.10 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **8192** | **Random**             | **18,751,586.4 ns** |  **52,999.32 ns** |  **23,532.03 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 8192 | Random             |  1,888,632.5 ns |   8,463.61 ns |   3,757.90 ns |  0.10 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 8192 | Random             |  9,702,164.2 ns |  28,545.26 ns |  14,929.72 ns |  0.52 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 8192 | Random             |  1,512,145.1 ns |   2,289.95 ns |   1,016.75 ns |  0.08 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **8192** | **SingleElementMoved** | **18,451,828.0 ns** | **205,770.16 ns** | **107,621.77 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 8192 | SingleElementMoved |  1,179,610.7 ns |   1,686.51 ns |     748.82 ns |  0.06 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 8192 | SingleElementMoved |  8,640,999.3 ns | 935,747.75 ns | 489,414.17 ns |  0.47 |    0.03 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 8192 | SingleElementMoved |    702,424.2 ns |   4,861.82 ns |   2,158.68 ns |  0.04 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **8192** | **Sorted**             | **18,374,397.5 ns** |  **16,199.28 ns** |   **8,472.54 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 8192 | Sorted             |  1,052,989.4 ns |   2,835.72 ns |   1,483.14 ns |  0.06 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 8192 | Sorted             |  7,771,993.3 ns |  24,540.37 ns |  10,896.08 ns |  0.42 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 8192 | Sorted             |    565,386.0 ns |   5,909.56 ns |   2,107.41 ns |  0.03 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **8192** | **Reversed**           | **16,409,582.3 ns** | **106,547.58 ns** |  **55,726.44 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| SelectionSortSimd       | 8192 | Reversed           |  1,796,385.5 ns |   2,255.01 ns |   1,001.24 ns |  0.11 |    0.00 |    1 |         - |          NA |
| DoubleSelectionSort     | 8192 | Reversed           |  7,795,189.7 ns |   2,360.49 ns |   1,234.58 ns |  0.48 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSortSimd | 8192 | Reversed           |  1,945,726.8 ns |   3,890.84 ns |   1,727.56 ns |  0.12 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **8192** | **PipeOrgan**          | **18,286,549.5 ns** |  **76,709.66 ns** |  **34,059.57 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 8192 | PipeOrgan          |  2,245,034.7 ns |   3,555.90 ns |   1,578.84 ns |  0.12 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 8192 | PipeOrgan          |  9,266,731.7 ns | 120,223.36 ns |  62,879.14 ns |  0.51 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 8192 | PipeOrgan          |  1,377,959.5 ns |   3,102.20 ns |   1,622.51 ns |  0.08 |    0.00 |    1 |         - |          NA |
|      |                    |                 |               |               |       |         |      |           |             |
| **SelectionSort**           | **8192** | **ManyDuplicates**     | **18,581,729.3 ns** |  **56,653.92 ns** |  **29,631.09 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 8192 | ManyDuplicates     |  1,845,564.0 ns |   4,025.85 ns |   1,435.66 ns |  0.10 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 8192 | ManyDuplicates     |  9,908,835.2 ns |   6,644.82 ns |   3,475.37 ns |  0.53 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 8192 | ManyDuplicates     |  1,463,739.3 ns |   5,107.62 ns |   2,671.38 ns |  0.08 |    0.00 |    1 |         - |          NA |

### TreeBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.46 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8
UnrollFactor=1  WarmupCount=2
```

| Method                 | Size | Pattern            | Mean         | Error       | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------- |----- |------------------- |-------------:|------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **BalancedBinaryTreeSort** | **256**  | **Random**             |  **16,315.2 ns** |   **172.44 ns** |    **90.19 ns** |  **1.38** |    **0.01** |    **1** |         **-** |          **NA** |
| BTreeSort              | 256  | Random             |  15,840.3 ns |   254.91 ns |   133.33 ns |  1.34 |    0.01 |    1 |         - |          NA |
| BPlusTreeSort          | 256  | Random             |  15,325.5 ns |   205.70 ns |    73.35 ns |  1.29 |    0.01 |    1 |         - |          NA |
| CartesianTreeSort      | 256  | Random             |  12,861.3 ns |   241.42 ns |   107.19 ns |  1.09 |    0.01 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | Random             |  11,835.5 ns |   117.40 ns |    61.40 ns |  1.00 |    0.00 |    1 |         - |          NA |
| SplaySort              | 256  | Random             |  30,530.9 ns |   341.99 ns |   178.87 ns |  2.58 |    0.02 |    2 |         - |          NA |
| TreapSort              | 256  | Random             |  17,901.8 ns |   164.63 ns |    86.10 ns |  1.51 |    0.01 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **SingleElementMoved** |   **6,494.1 ns** |   **374.17 ns** |   **195.70 ns** |  **0.12** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 256  | SingleElementMoved |   4,673.0 ns |    76.53 ns |    33.98 ns |  0.09 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 256  | SingleElementMoved |   4,333.9 ns |   267.27 ns |   139.79 ns |  0.08 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 256  | SingleElementMoved |   1,596.0 ns |     9.02 ns |     3.22 ns |  0.03 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | SingleElementMoved |  53,658.0 ns |   213.79 ns |    94.92 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 256  | SingleElementMoved |   3,340.5 ns |    23.44 ns |     8.36 ns |  0.06 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | SingleElementMoved |   4,935.9 ns |   426.12 ns |   222.87 ns |  0.09 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Sorted**             |   **6,179.8 ns** |   **207.72 ns** |    **92.23 ns** |  **0.11** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 256  | Sorted             |   4,486.8 ns |    42.25 ns |    18.76 ns |  0.08 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 256  | Sorted             |   4,019.1 ns |    45.53 ns |    16.23 ns |  0.07 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 256  | Sorted             |   1,540.7 ns |     7.82 ns |     4.09 ns |  0.03 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | Sorted             |  58,581.3 ns |   137.11 ns |    60.88 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 256  | Sorted             |   2,988.2 ns |    22.53 ns |     8.03 ns |  0.05 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | Sorted             |   3,986.4 ns |   286.77 ns |   149.99 ns |  0.07 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Reversed**           |   **6,057.8 ns** |   **280.54 ns** |   **146.73 ns** |  **0.10** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 256  | Reversed           |   7,213.4 ns |    33.65 ns |    12.00 ns |  0.12 |    0.00 |    4 |         - |          NA |
| BPlusTreeSort          | 256  | Reversed           |   6,643.4 ns |    80.98 ns |    42.36 ns |  0.11 |    0.00 |    4 |         - |          NA |
| CartesianTreeSort      | 256  | Reversed           |   1,494.9 ns |     4.68 ns |     2.45 ns |  0.02 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | Reversed           |  61,948.4 ns |   414.47 ns |   216.77 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 256  | Reversed           |   2,898.0 ns |    15.32 ns |     5.46 ns |  0.05 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | Reversed           |   3,921.2 ns |   374.49 ns |   195.87 ns |  0.06 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **PipeOrgan**          |   **5,279.6 ns** |   **115.48 ns** |    **60.40 ns** |  **0.18** |    **0.00** |    **3** |         **-** |          **NA** |
| BTreeSort              | 256  | PipeOrgan          |   5,515.6 ns |   373.19 ns |   195.18 ns |  0.19 |    0.01 |    3 |         - |          NA |
| BPlusTreeSort          | 256  | PipeOrgan          |   4,905.9 ns |   208.22 ns |   108.91 ns |  0.17 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 256  | PipeOrgan          |   1,683.2 ns |     5.45 ns |     2.42 ns |  0.06 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | PipeOrgan          |  29,141.0 ns |   204.11 ns |    90.63 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 256  | PipeOrgan          |   3,382.9 ns |    17.25 ns |     6.15 ns |  0.12 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | PipeOrgan          |   5,770.2 ns |   250.48 ns |   131.00 ns |  0.20 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **ManyDuplicates**     |  **15,692.5 ns** |   **239.96 ns** |   **125.50 ns** |  **1.49** |    **0.02** |    **2** |         **-** |          **NA** |
| BTreeSort              | 256  | ManyDuplicates     |  15,473.3 ns |   253.29 ns |   132.48 ns |  1.47 |    0.02 |    2 |         - |          NA |
| BPlusTreeSort          | 256  | ManyDuplicates     |  14,696.3 ns |   207.93 ns |   108.75 ns |  1.40 |    0.02 |    2 |         - |          NA |
| CartesianTreeSort      | 256  | ManyDuplicates     |  15,602.1 ns |   114.33 ns |    50.76 ns |  1.48 |    0.02 |    2 |         - |          NA |
| BinaryTreeSort         | 256  | ManyDuplicates     |  10,508.7 ns |   243.51 ns |   127.36 ns |  1.00 |    0.00 |    1 |         - |          NA |
| SplaySort              | 256  | ManyDuplicates     |  28,505.7 ns |   290.34 ns |   128.91 ns |  2.71 |    0.03 |    3 |         - |          NA |
| TreapSort              | 256  | ManyDuplicates     |  17,679.3 ns |   113.46 ns |    50.38 ns |  1.68 |    0.02 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Random**             |  **74,392.7 ns** |   **411.80 ns** |   **215.38 ns** |  **1.35** |    **0.01** |    **1** |         **-** |          **NA** |
| BTreeSort              | 1024 | Random             |  75,274.8 ns |   634.92 ns |   332.07 ns |  1.37 |    0.01 |    1 |         - |          NA |
| BPlusTreeSort          | 1024 | Random             |  72,996.6 ns |   409.85 ns |   214.36 ns |  1.33 |    0.01 |    1 |         - |          NA |
| CartesianTreeSort      | 1024 | Random             |  64,854.1 ns |   642.95 ns |   336.27 ns |  1.18 |    0.01 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | Random             |  54,927.7 ns |   335.28 ns |   175.36 ns |  1.00 |    0.00 |    1 |         - |          NA |
| SplaySort              | 1024 | Random             | 155,154.7 ns |   549.81 ns |   196.07 ns |  2.82 |    0.01 |    2 |         - |          NA |
| TreapSort              | 1024 | Random             |  81,186.6 ns |   193.13 ns |    85.75 ns |  1.48 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **SingleElementMoved** |  **28,277.5 ns** |   **297.31 ns** |   **155.50 ns** | **0.033** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 1024 | SingleElementMoved |  21,150.2 ns |   133.28 ns |    59.18 ns | 0.025 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 1024 | SingleElementMoved |  19,798.4 ns |   154.80 ns |    80.96 ns | 0.023 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 1024 | SingleElementMoved |   6,037.2 ns |     8.26 ns |     2.94 ns | 0.007 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | SingleElementMoved | 851,440.8 ns | 4,098.14 ns | 1,819.60 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | SingleElementMoved |  13,170.3 ns |   279.12 ns |   145.98 ns | 0.015 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | SingleElementMoved |  19,774.7 ns |   264.93 ns |   117.63 ns | 0.023 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Sorted**             |  **27,437.8 ns** |   **491.44 ns** |   **257.03 ns** | **0.029** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 1024 | Sorted             |  20,305.8 ns |   141.84 ns |    62.98 ns | 0.022 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 1024 | Sorted             |  18,766.5 ns |   169.23 ns |    88.51 ns | 0.020 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 1024 | Sorted             |   5,913.6 ns |     7.46 ns |     3.31 ns | 0.006 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | Sorted             | 934,023.0 ns | 1,892.34 ns |   989.73 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | Sorted             |  12,017.5 ns |   213.26 ns |   111.54 ns | 0.013 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | Sorted             |  17,394.6 ns |   581.04 ns |   303.90 ns | 0.019 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Reversed**           |  **26,974.3 ns** |   **683.34 ns** |   **357.40 ns** | **0.027** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 1024 | Reversed           |  32,380.8 ns |   322.90 ns |   168.88 ns | 0.033 |    0.00 |    4 |         - |          NA |
| BPlusTreeSort          | 1024 | Reversed           |  28,609.3 ns |   123.35 ns |    54.77 ns | 0.029 |    0.00 |    4 |         - |          NA |
| CartesianTreeSort      | 1024 | Reversed           |   5,805.8 ns |    20.08 ns |     8.92 ns | 0.006 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | Reversed           | 991,636.3 ns |   488.18 ns |   216.76 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | Reversed           |  11,413.6 ns |   219.73 ns |    97.56 ns | 0.012 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | Reversed           |  16,851.0 ns |   283.33 ns |   101.04 ns | 0.017 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **PipeOrgan**          |  **24,079.9 ns** |   **647.42 ns** |   **338.61 ns** |  **0.05** |    **0.00** |    **3** |         **-** |          **NA** |
| BTreeSort              | 1024 | PipeOrgan          |  23,477.9 ns |   460.41 ns |   240.80 ns |  0.05 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 1024 | PipeOrgan          |  21,572.8 ns |   270.51 ns |   120.11 ns |  0.05 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 1024 | PipeOrgan          |   6,597.0 ns |   258.36 ns |   135.13 ns |  0.01 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | PipeOrgan          | 468,448.5 ns |   713.51 ns |   316.80 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | PipeOrgan          |  13,633.6 ns |   468.54 ns |   208.03 ns |  0.03 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | PipeOrgan          |  24,742.8 ns |   352.19 ns |   156.37 ns |  0.05 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **ManyDuplicates**     |  **64,875.8 ns** |   **245.77 ns** |   **109.13 ns** |  **1.12** |    **0.00** |    **1** |         **-** |          **NA** |
| BTreeSort              | 1024 | ManyDuplicates     |  68,512.3 ns |   260.76 ns |   115.78 ns |  1.18 |    0.00 |    1 |         - |          NA |
| BPlusTreeSort          | 1024 | ManyDuplicates     |  65,275.6 ns |   416.53 ns |   217.85 ns |  1.12 |    0.01 |    1 |         - |          NA |
| CartesianTreeSort      | 1024 | ManyDuplicates     |  82,921.6 ns |   807.98 ns |   358.75 ns |  1.43 |    0.01 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | ManyDuplicates     |  58,043.2 ns |   455.74 ns |   238.36 ns |  1.00 |    0.00 |    1 |         - |          NA |
| SplaySort              | 1024 | ManyDuplicates     | 123,176.4 ns |   856.97 ns |   380.50 ns |  2.12 |    0.01 |    2 |         - |          NA |
| TreapSort              | 1024 | ManyDuplicates     |  78,016.9 ns |   222.14 ns |   116.18 ns |  1.34 |    0.01 |    1 |         - |          NA |

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
- [Merge Sort (SIMD, Chhugani)](./src/SortAlgorithm/Algorithms/Merge/SimdMergeSort.cs)
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
- [Blit Sort](./src/SortAlgorithm/Algorithms/Partition/BlitSort.cs)
- [Block Quick Sort](./src/SortAlgorithm/Algorithms/Partition/BlockQuickSort.cs)
- [Crum Sort](./src/SortAlgorithm/Algorithms/Partition/CrumSort.cs)
- [Quick Sort (Destswap Stable)](./src/SortAlgorithm/Algorithms/Partition/DestswapStableQuickSort.cs)
- [Quick Sort (Dual Pivot)](./src/SortAlgorithm/Algorithms/Partition/DualPivotQuickSort.cs)
- [Flux Sort](./src/SortAlgorithm/Algorithms/Partition/FluxSort.cs)
- [Intro Sort](./src/SortAlgorithm/Algorithms/Partition/IntroSort.cs)
- [Intro Sort (Dotnet)](./src/SortAlgorithm/Algorithms/Partition/IntroSortDotnet.cs)
- [Ipnsort](./src/SortAlgorithm/Algorithms/Partition/Ipnsort.cs)
- [In-place Super Scalar Samplesort (IS⁴o)](./src/SortAlgorithm/Algorithms/Partition/Ips4oSort.cs)
- [Pattern-Defeating Quick Sort](./src/SortAlgorithm/Algorithms/Partition/PDQSort.cs)
- [PDQ Sort Branchless](./src/SortAlgorithm/Algorithms/Partition/PDQSortBranchless.cs)
- [Quick Sort](./src/SortAlgorithm/Algorithms/Partition/QuickSort.cs)
- [Quick Sort (3-Way)](./src/SortAlgorithm/Algorithms/Partition/QuickSort3way.cs)
- [Quick Sort (Median of 3)](./src/SortAlgorithm/Algorithms/Partition/QuickSortMedian3.cs)
- [Quick Sort (Median of 9)](./src/SortAlgorithm/Algorithms/Partition/QuickSortMedian9.cs)
- [Sample Sort](./src/SortAlgorithm/Algorithms/Partition/SampleSort.cs)
- [Quick Sort (SIMD, Bramas)](./src/SortAlgorithm/Algorithms/Partition/SimdQuickSort.cs)
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
- [Ska Sort](./src/SortAlgorithm/Algorithms/Distribution/SkaSort.cs)
- [Spread Sort](./src/SortAlgorithm/Algorithms/Distribution/SpreadSort.cs)

### Network
- [Batcher Odd-Even Merge Sort](./src/SortAlgorithm/Algorithms/Network/BatcherOddEvenMergeSort.cs)
- [Batcher Odd-Even Merge Sort Branchless](./src/SortAlgorithm/Algorithms/Network/BatcherOddEvenMergeSortBranchless.cs)
- [Bitonic Sort](./src/SortAlgorithm/Algorithms/Network/BitonicSort.cs)
  - Iterative
  - Recursive
- [Bitonic Sort (Any Length)](./src/SortAlgorithm/Algorithms/Network/BitonicSortAnyLength.cs)
- [Bitonic Sort (Any Length, SIMD)](./src/SortAlgorithm/Algorithms/Network/BitonicSortAnyLengthSimd.cs)
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
